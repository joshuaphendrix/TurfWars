var Service =
{
    request: function(action, args,callbackFun)
    {
        var HttpRequest = false;
        if (window.XMLHttpRequest)
        {
            HttpRequest = new XMLHttpRequest();
        }
        else
        {
            HttpRequest = new ActiveXObject("Microsoft.XMLHTTP");
        }
        if (!HttpRequest) return;

        var url = 'Service.asmx/'+action;
        
        HttpRequest.open("POST",url);
        HttpRequest.setRequestHeader('Content-type', 'application/x-www-form-urlencoded;charset=UTF-8;');
     
        HttpRequest.onreadystatechange = function()
        {
            if (HttpRequest.readyState == 4 && HttpRequest.status == 200)
            {
                var xml = HttpRequest.responseXML;
                
                callbackFun(xml);
            }
        }
        
        HttpRequest.send(args);  
    }, 
    addClaim: function(user,lat,lon,callbackFun)
    {
        var args = "user=" +user+ "&lat=" +lat+ "&lon=" + lon;
        
        this.request('addClaim',args,callbackFun);
    },
    getUserStats: function(user, callbackFun)
    {
        var args = "user=" + user;
        this.request('getUserStats',args,callbackFun);
    },
    getClaims: function(callbackFun)
    {
        this.request('getClaims','',callbackFun);
    }

};
