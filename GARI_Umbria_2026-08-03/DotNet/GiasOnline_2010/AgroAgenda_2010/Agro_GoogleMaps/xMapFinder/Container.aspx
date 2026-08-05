<%@ Page Language="VB" AutoEventWireup="false" Inherits="AgroAgenda_2010.Map_Container" CodeBehind="Container.aspx.vb" %>


<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" lang="en">
<head id="Head1" runat="server">
    <title>Ricerca Indirizzo</title>


    <!-- /////////////////////////////////////////////// -->
    <base target="_self" />


</head>
<body onload="load()">
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>

        <asp:UpdatePanel ID="updt1" runat="server">
            <ContentTemplate>
                <div style="left: 33px; width: 850px; position: absolute; top: 26px; height: 430px">
                    <div id="mapcanvas" style="width: 500px; height: 420px; left: 9px; position: absolute; top: 31px; border-right: 3px double; border-top: 3px double; border-left: 3px double; border-bottom: 3px double;">
                    </div>
                    <div style="font-size: 8pt; left: 9px; width: 124px; font-family: verdana, sans-serif; position: absolute; top: 8px; height: 15px">
                        Localita' / Indirizzo :
                    </div>
                    <input id="TxtIndirizzo" name="TxtIndirizzo" style="width: 289px; left: 136px; position: absolute; top: 6px; font-size: 8pt; font-family: verdana, sans-serif; height: 12px;" type="text" />
                    <input id="BtnCerca" name="BtnCerca" type="button" value="CERCA" style="left: 445px; position: absolute; top: 5px; width: 70px; font-size: 8pt; font-family: verdana, sans-serif;"
                        onclick="CercaIndirizzo()" />
                    &nbsp;&nbsp;
                </div>

                <%--<asp:UpdatePanel ID="UpdatePanelToolBar" runat="server" />--%>

                <input id="TxtRitorno" name="TxtRitorno" style="left: 32px; width: 140px; position: absolute; top: 672px"
                    type="hidden" value="AAA~BBB~CCC" runat="server" />

                <!--<input id="BtnConferma" name="BtnConferma" type="button" value="OK" style="left: 480px; position: absolute; 
        top: 492px; width: 70px; font-size: 8pt; font-family: verdana, sans-serif;" />-->

                <asp:Button ID="btnConfermaNet" runat="server" Text="Ok" Style="left: 480px; position: absolute; top: 492px; width: 70px; font-size: 8pt; font-family: verdana, sans-serif;" />

                <asp:Label ID="messaggi" runat="server" Style="left: 44px; position: absolute; top: 495px; width: 437px; font-size: 8pt; font-family: verdana, sans-serif;"></asp:Label>

                <script src="<%= srv_gm %>" type="text/javascript">
                </script>


                <script type="text/javascript">
                    function QueryString() {
                        // PROPERTIES
                        this.arg = new Array;
                        this.status = false;

                        // METHODS
                        this.clear = Clear;
                        this.get = Get;
                        this.getAll = GetAll;
                        this.getStatus = GetStatus;
                        this.read = Read;
                        this.set = Set;
                        this.write = Write;

                        // FUNCTIONS

                        // Clears the array, this.arg, of all query string data
                        function Clear() {
                            this.arg = new Array;
                        }

                        // Returns a named value from the query string
                        function Get(sName) {
                            return this.arg[sName];
                        }

                        // Return all data as an associative array
                        function GetAll() {
                            return this.arg;
                        }

                        function GetStatus() {
                            return this.status;
                        }

                        // Reads the query string into an array named this.arg
                        function Read(sUrl) {
                            var aArgsTemp, aTemp, sQuery;
                            // You can pass in a URL query string
                            if (sUrl) {
                                sQuery = sUrl.substr(sUrl.lastIndexOf("?") + 1, sUrl.length);
                            }
                            // Or read it from the browser location
                            else {
                                sQuery = window.location.search.substr(1, window.location.search.length);
                            }
                            // Check that query string exists and contains data
                            // If not (length < 1) then return
                            if (sQuery.length < 1) { return; }
                            // Else set this.status to true and proceed
                            else { this.status = true; }
                            //
                            aArgsTemp = sQuery.split("&");
                            for (var i = 0; i < aArgsTemp.length; i++) {
                                aTemp = aArgsTemp[i].split("=");
                                this.arg[aTemp[0]] = aTemp[1];
                            }
                        }

                        // Overwrites an existing named value in the array, this.arg
                        // You can also pass null to delete from array
                        function Set(sName, sValue) {
                            if (sValue == null) { delete this.arg[sName]; }
                            else { this.arg[sName] = sValue; }
                        }

                        // Writes out a string from the data in this.arg array
                        // This string can be used to pass a new query string to the browser
                        // when navigating to the next page. This allows a page
                        // to create and pass data to another page via JavaScript.
                        function Write() {
                            var sQuery = new String("");
                            for (var sName in this.arg) {
                                if (sQuery != "") { sQuery += "&"; }
                                if (this.arg[sName]) { sQuery += sName + "=" + this.arg[sName]; }
                            }
                            if (sQuery.length > 0) { return "?" + sQuery; }
                            else { return sQuery; }
                        }
                    }
                </script>



                <script type="text/javascript">


                    // =========================================================================
                    var map
                    function createMarker(point, label) {
                        var marker = new GMarker(point);
                        GEvent.addListener(marker, "click", function () {
                            marker.openInfoWindowHtml(label);
                        });
                        return marker;
                    };

                    // =========================================================================

                    function MapUpdate() {
                        var center = map.getCenter();
                        var bounds = map.getBounds();
                        risultato = '';
                        risultato = risultato + center.lat();
                        risultato = risultato + '~';
                        risultato = risultato + center.lng();
                        risultato = risultato + '~';
                        risultato = risultato + map.getZoom();
                        risultato = risultato + '~';
                        risultato = risultato + bounds.getNorthEast().lat();
                        risultato = risultato + '~';
                        risultato = risultato + bounds.getSouthWest().lng();
                        risultato = risultato + '~';
                        risultato = risultato + bounds.getSouthWest().lat();
                        risultato = risultato + '~';
                        risultato = risultato + bounds.getNorthEast().lng();
                        document.getElementById("TxtRitorno").value = risultato;

                    };

                    // =========================================================================

                    function CercaIndirizzo() {
                        var geocoder = new google.maps.Geocoder();
                        var risultato;

                        var indirizzo = document.getElementById("TxtIndirizzo").value;
                        geocoder.geocode({ 'address': indirizzo }, function (results, status) {

                            if (status == google.maps.GeocoderStatus.OK) {
                                marker = new google.maps.Marker({
                                    map: map,
                                    position: results[0].geometry.location,
                                    title: 'Questo è l\'indirizzo inserito'
                                });

                                var newOptions =
                                {
                                    zoom: 17,
                                    position: results[0].geometry.location,
                                    mapTypeId: google.maps.MapTypeId.SATELLITE,
                                    minZoom: 5
                                }
                                map.setOptions(newOptions);
                                map.setCenter(results[0].geometry.location);
                                MapUpdate(map);
                                google.maps.event.addListener(map, "idle", function () { MapUpdate(map) });



                            }
                            else {
                                alert("Indirizzo \"" + indirizzo + "\" non trovato. Errore : " + status);
                            }

                        });
                    }
                    // =========================================================================

                    function load() {
                        var des;
                        var adr = "<%= srv_adr %>";

                        var lat = 42.504503;
                        var lng = 12.700195;
                        var myOptions =
                        {
                            zoom: 5,
                            center: new google.maps.LatLng(lat, lng),
                            mapTypeId: google.maps.MapTypeId.SATELLITE,
                            minZoom: 5
                        }
                        map = new google.maps.Map(document.getElementById("mapcanvas"), myOptions);
                        var qs = document.location.search;


                        // Create a new QueryString object
                        var myQuery = new QueryString();

                        // Read query string from browser into the new QueryString object, name myQuery
                        myQuery.read();



                        adr = adr.replace(/%20/g, " ");


                        if (adr != '') {
                            document.getElementById("TxtIndirizzo").value = adr.replace(/%20/g, " ");
                            CercaIndirizzo();
                        }


                    }

    // =========================================================================


                </script>
            </ContentTemplate>
        </asp:UpdatePanel>
    </form>
</body>
</html>

