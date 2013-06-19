<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="turf.aspx.cs" Inherits="TurfWars._Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Turf Wars</title>
    <meta name="viewport" content="initial-scale=1.0, user-scalable=no" />
    <script type="text/javascript" src="http://maps.googleapis.com/maps/api/js?key=AIzaSyDV1OyiZyY7J_8J0Zb3YZ0zBJDkodRXnOs&sensor=true"> </script>
    <script src="geo.js" type="text/javascript" language="javascript"></script>
    <script src="service.js" type="text/javascript" language="javascript"></script>
    <link href="TurfWars.css" rel="stylesheet" type="text/css" />
    
</head>
<body onload="initialize()">
    <form id="form1" runat="server">
    <div class="turfWars">
        <div id="map_canvas"></div>
        <div id="divStats">
            <span class="label">Account Balance: <span id="spnAccountBalance"></span></span><br />
            <span class="label">Turfs: <span id="spnTurfCount"></span></span><br />
            <span class"button" onclick="startClaiming();">make claim</span>
        </div>
    </div>
    </form>
</body>
</html>
