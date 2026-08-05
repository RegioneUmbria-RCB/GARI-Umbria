<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="GisControl.ascx.vb" Inherits="AgroAgenda_2010.GisControl" %>
<style type="text/css">
    #contenitore_tool
    {
        width: 65px;
    }
    #sidebar
    {
        width: 150px;
        padding: 0.5em;
        border-color: #32CD32;
        border-style: dashed;
        border-width: 1px;
        z-index: 10000;
        background-color: rgba(255, 255, 255, 0.5);
    }
    #sitebar_indirizzo
    {
        width: 228px;
        height: 75px;
        padding: 0.5em;
        border-color: #32CD32;
        border-style: dashed;
        border-width: 1px;
        z-index: 10000;
        background-color: rgba(255, 255, 255, 0.5);
    }
    
    
    #color-palette
    {
        clear: both;
    }
    .color-button
    {
        width: 14px;
        height: 14px;
        font-size: 0;
        margin: 2px;
        float: left;
        cursor: pointer;
    }
    #delete-button
    {
        margin-top: 5px;
    }
    
</style>
<script type="text/javascript" src="https://maps.googleapis.com/maps/api/js?key=AIzaSyA2cJIYR1WcTQgp1VudoETCNvbfVIoG-ZQ&libraries=drawing,geometry"></script>
<script type="text/javascript" src="https://www.google.com/jsapi"></script>



<script type="text/javascript">
    var map;

    var poligoni = [];
    var punti = [];

    function pulisciMappa() {

        cancellaOggettiGrafici(poligoni);
        cancellaOggettiGrafici(punti);
    }


    function cancellaOggettiGrafici(oggetti) {
        for (var i = 0; i < oggetti.length; i++) {
            oggetti[i].setMap(null);
        }
        oggetti.length = 0;
    }

    function WKT_ToGoogleMapsString(sWKT) {

        var wkt = sWKT; //this is your WKT string

        var rval = "";

        //using regex, we will get the indivudal Rings
        var regex = /\(([^()]+)\)/g;
        var Rings = [];
        var results;
        while (results = regex.exec(wkt)) {
            Rings.push(results[1]);
        }       

        var polyLen = Rings.length;

        //now we need to draw the polygon for each of inner rings, but reversed
        for (var i = 0; i < polyLen; i++) {
           rval = rval + Gis_ASCX_AddPoints(Rings[i]);
       }

       return rval;
    }

    function Gis_ASCX_AddPoints(data) {

        var rval = ""

        //first spilt the string into individual points
        var pointsData = data.split(",");


        //iterate over each points data and create a latlong
        //& add it to the cords array
        var len = pointsData.length;
        for (var i = 0; i < len; i++) {
            var xy = pointsData[i].split(" ");

            var pt = '';
            if (xy.length == 2) {
                pt = '{ "lat": ' + xy[1] + ', "lng": ' + xy[0] + '}';
            } else {
                pt = '{ "lat": ' + xy[2] + ', "lng": ' + xy[1] + '}';
            }
            rval = rval + pt;
            if (i < len - 1) {
                rval = rval + ","
            }
        }

        return "[" + rval + "]";

    }
        
    function createQuakeEventMarker(quakeEventLatlng) {
        return new google.maps.Marker({ position: quakeEventLatlng, map: map });
    }

    function setupMap(lat, lng, mapZoom, showOverviewControl) {
        var mapLatlng = new google.maps.LatLng(lat, lng);
        var myOptions = {
            zoom: mapZoom,
            center: mapLatlng,
            overviewMapControl: showOverviewControl,
            zoomControl: true,
            streetViewControl: false,
            zoomControlOptions: {
                style: google.maps.ZoomControlStyle.SMALL,
                position: google.maps.ControlPosition.LEFT_TOP
            },
            mapTypeId: google.maps.MapTypeId.SATELLITE
        };
        map = new google.maps.Map(document.getElementById("map_canvas"), myOptions);
        google.maps.event.trigger(map, 'resize');
    }


    function Gis_ASCX_initializeLatLon(Lat, Long, mapZoom, showOverviewControl) {

        if (map == null) {
            setupMap(Lat, Long, mapZoom, showOverviewControl);            
        }

//        var latlngAutoFit = new google.maps.LatLngBounds();
//        latlngAutoFit.extend(new google.maps.LatLng(Lat, Long));
//        map.setCenter(latlngAutoFit.getCenter(), map.fitBounds(latlngAutoFit));
        map.setZoom(mapZoom);

    }

    function Gis_ASCX_Marker(Lat, Long) {

        pulisciMappa();

        var quakeEventLatlng = new google.maps.LatLng(Lat, Long);
        var marker = createQuakeEventMarker(quakeEventLatlng);
        marker.setAnimation(google.maps.Animation.DROP);

        punti.push(marker);
        
        var latlngAutoFit = new google.maps.LatLngBounds();
        latlngAutoFit.extend(new google.maps.LatLng(Lat, Long));
        
        map.setCenter(latlngAutoFit.getCenter(), map.fitBounds(latlngAutoFit));
        map.setZoom(16);

    }


    function Gis_ASCX_Poligono(polyCoords) {

        pulisciMappa();

        var latlngAutoFit = new google.maps.LatLngBounds();
        for (cCoord in polyCoords) {
            latlngAutoFit.extend(new google.maps.LatLng( polyCoords[cCoord].lat, polyCoords[cCoord].lng));
        }
        

        // Construct the polygon.
        var abc = new google.maps.Polygon({
            paths: polyCoords,
            strokeColor: '#FF0000',
            strokeOpacity: 0.8,
            strokeWeight: 2,
            fillColor: '#FF0000',
            fillOpacity: 0.35
        });

        poligoni.push(abc);
        abc.setMap(map);

        
        map.setCenter(latlngAutoFit.getCenter(), map.fitBounds(latlngAutoFit));
        map.setZoom(16);

    }



    // Initialize the map when the jQuery Mobile pageshow event is triggered
//    $(document).on("pageshow", ".details-page", function () {
//        if (map == null) {
//            initialize();
//        }
//    });

</script>
<div id="map_canvas" style="width: 100%; height: 100%">
</div>
