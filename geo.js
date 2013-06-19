var map;

rad = function(x) {return x*Math.PI/180;}

distHaversine = function(p1, p2) {
  var R = 6371; // earth's mean radius in km
  var dLat  = rad(p2.lat() - p1.lat());
  var dLong = rad(p2.lng() - p1.lng());

  var a = Math.sin(dLat/2) * Math.sin(dLat/2) +
          Math.cos(rad(p1.lat())) * Math.cos(rad(p2.lat())) * Math.sin(dLong/2) * Math.sin(dLong/2);
  var c = 2 * Math.atan2(Math.sqrt(a), Math.sqrt(1-a));
  var d = R * c;

  return d.toFixed(3);
}

function setPosition(position)
{
    var lat = position.coords.latitude;
    var lng = position.coords.longitude;
    
     
    map = new google.maps.Map(document.getElementById('map_canvas'), {
       center: new google.maps.LatLng(lat, lng),
       zoom: 11,
        mapTypeId: google.maps.MapTypeId.TERRAIN,
      styles: [
        {
          stylers: [
            { visibility: "off" }
          ]
        },{
          featureType: "water",
          elementType: "geometry",
          stylers: [
            { visibility: "on" }
          ]
        }
      ]
     });

    directionsDisplay = new google.maps.DirectionsRenderer();
    directionsDisplay.setMap(map);
    
    google.maps.event.addListener(map,'click',map_clicked);
    
    Service.addClaim(username,lat, lng,RenderTile);
}

function drawPath(start, end) 
{
  var path = new google.maps.Polyline({
    path: [start,end],
    strokeColor: "gold",
    strokeOpacity: 1.0,
    strokeWeight: 2
  });

  path.setMap(map);
}

  
function positionNotFound()
{
    /*
    var fakePosition = new Object();
    var fakeCoords = new Object();
    var fakeLat = 37.8283392170757;
    var fakeLon = -122.364825598755;
    
    fakeCoords.latitude = fakeLat;
    fakeCoords.longitude = fakeLon;
    fakePosition.coords = fakeCoords;
    
    setPosition(fakePosition);
    */
}

var username;
function initialize() 
{
    if(!username)
    {
        username = prompt("Who does #2 work for?","Ted");
    }
   
    if(navigator && navigator.geolocation)
    {
        navigator.geolocation.getCurrentPosition(setPosition,positionNotFound);
    }else
    {
        positionNotFound();
    }
    
    Service.getUserStats(username,processStats);
}

var start = false;
var end = false;
function map_clicked(event)
{
    
    if(!start)
    {
        alert('start: ' + event.latLng);
        start = event.latLng;
        return;
    }
    
    if(start && !end)
    {
        alert('End: ' + event.latLng);
        end = event.latLng;
    
        drawPath(start,end);   
        alert(distHaversine(start,end));
        start = false;
        end = false; 
    }
    
    
    //placeMarker(event.latLng);
}

var timer;
function placeMarker(latlng)
{   
    Service.addClaim(username,latlng.lat(), latlng.lng(),RenderTile);
    
    if(timer){clearTimeout(timer); timer=null;}
    timer = setTimeout('Service.getClaims(renderClaims)',1000);
}

function RenderTile(xml)
{
    var tile = xml.getElementsByTagName("tile");
        
    var north = tile[0].getAttribute('north');
    var south = tile[0].getAttribute('south');
    var west = tile[0].getAttribute('west');
    var east = tile[0].getAttribute('east');
    
    //alert('nw:' + north + ',' + west + ':' + 'se:' + south + ',' + east);
    
    
    var rect = new google.maps.Rectangle({
        bounds: new google.maps.LatLngBounds(
            new google.maps.LatLng(north,west),
            new google.maps.LatLng(south,east)
        ),
        map: map 
    })
    
    tiles.push(rect); 
}

function drawBox(north, south, east, west, color)
{
    var rect = new google.maps.Rectangle({
        bounds: new google.maps.LatLngBounds(
            new google.maps.LatLng(north,west),
            new google.maps.LatLng(south,east)
        ),
        map: map 
    })
    
    google.maps.event.addListener(rect,'click',map_clicked);
}

var tiles = new Array();

function drawTile(tile)
{
    
    var rect = new google.maps.Rectangle({
        bounds: new google.maps.LatLngBounds(
            new google.maps.LatLng(tile.north,tile.west),
            new google.maps.LatLng(tile.south,tile.east)
        ),
        fillColor: tile.color,
        strokeWeight:0,
        map: map 
    })
    google.maps.event.addListener(rect,'click',map_clicked);
    
  if(tiles) 
  {
    for (i in tiles) 
    {
      if(
        tiles[i].bounds.getCenter().lat() == rect.bounds.getCenter().lat()
        && tiles[i].bounds.getCenter().lng() == rect.bounds.getCenter().lng()
        )
      {
        tiles[i].setMap(null);
        tiles.splice(i,1);
      }
    }
  }
  
    tiles.push(rect);  
    
}
function renderClaims(xml)
{
    var t = new Object();
    
    var claims = xml.getElementsByTagName('claim');
    
    for(var i=0; i<claims.length; i++)
    {
        var owners = claims[i].getElementsByTagName('owner');
        var tile   = claims[i].getElementsByTagName('tile');
        
        t.caption = '';
        for(var j=0; j<owners.length; j++)
        {
            t.caption += owners[j].getAttribute('name') + '(' + owners[j].getAttribute('share') + ')';
            
            t.color = 'blue';
            if(owners[j].getAttribute('name') == username)
            {
                t.color = 'green';
                if(owners[j].getAttribute('share') < 1)
                {
                    t.color = 'yellow';
                }else if(owners[j].getAttribute('share') < .5)
                {
                    t.color = 'orange';
                }else if (owners[j].getAttribute('share') < .25)
                {
                    t.color = 'red';
                }
                continue;
            }
        }
        t.north = tile[0].getAttribute('north');
        t.south = tile[0].getAttribute('south');
        t.west = tile[0].getAttribute('west');
        t.east = tile[0].getAttribute('east');
        
        drawTile(t);  
    }

}
function processStats(xml)
{
    //alert('here');
    var stats = xml.getElementsByTagName("user");
    
    document.getElementById('spnAccountBalance').innerHTML = stats[0].getAttribute('balance');
    document.getElementById('spnTurfCount').innerHTML = stats[0].getAttribute('shares');
}