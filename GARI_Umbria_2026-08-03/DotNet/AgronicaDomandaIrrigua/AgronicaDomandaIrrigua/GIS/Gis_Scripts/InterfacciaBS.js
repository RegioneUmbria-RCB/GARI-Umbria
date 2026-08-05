/* gestione del click sul più per espandere e chiudere il pannello dei tool */

//https://github.com/perfectline/geopoint
GeoPoint = function (lon, lat) {


    switch (typeof (lon)) {

        case 'number':

            this.lonDeg = this.dec2deg(lon, this.MAX_LON);
            this.lonDec = lon;

            break;

        case 'string':

            if (this.decode(lon)) {
                this.lonDeg = lon;
            }

            this.lonDec = this.deg2dec(lon, this.MAX_LON);

            break;
    }

    switch (typeof (lat)) {

        case 'number':

            this.latDeg = this.dec2deg(lat, this.MAX_LAT);
            this.latDec = lat;

            break;

        case 'string':

            if (this.decode(lat)) {
                this.latDeg = lat;
            }

            this.latDec = this.deg2dec(lat, this.MAX_LAT);

            break;

    }
};

GeoPoint.prototype = {

    CHAR_DEG: "\u00B0",
    CHAR_MIN: "\u0027",
    CHAR_SEC: "\u0022",
    CHAR_SEP: "\u0020",

    MAX_LON: 180,
    MAX_LAT: 90,

    // decimal
    lonDec: NaN,
    latDec: NaN,

    // degrees
    lonDeg: NaN,
    latDeg: NaN,

    dec2deg: function (value, max) {

        var sign = value < 0 ? -1 : 1;

        var abs = Math.abs(Math.round(value * 1000000));

        if (abs > (max * 1000000)) {
            return NaN;
        }

        var dec = abs % 1000000 / 1000000;
        var deg = Math.floor(abs / 1000000) * sign;
        var min = Math.floor(dec * 60);
        var sec = (dec - min / 60) * 3600;

        var result = "";

        result += deg;
        result += this.CHAR_DEG;
        result += this.CHAR_SEP;
        result += min;
        result += this.CHAR_MIN;
        result += this.CHAR_SEP;
        result += sec.toFixed(2);
        result += this.CHAR_SEC;

        return result;

    },

    dec2dms: function (value, max) {
        var sign = value < 0 ? -1 : 1;

        var abs = Math.abs(Math.round(value * 1000000));

        if (abs > (max * 1000000)) {
            return null;
        }

        var dec = abs % 1000000 / 1000000;
        let deg = Math.floor(abs / 1000000) * sign;
        let min = Math.floor(dec * 60);
        let sec = (dec - min / 60) * 3600;
        return {
            d: deg,
            m: min,
            s: sec
        }
    },

    deg2dec: function (value) {

        var matches = this.decode(value);

        if (!matches) {
            return NaN;
        }

        var deg = parseFloat(matches[1]);
        var min = parseFloat(matches[2]);
        var sec = parseFloat(matches[3]);

        if (isNaN(deg) || isNaN(min) || isNaN(sec)) {
            return NaN;
        }

        let sign = deg < 0 ? -1 : 1;
        let abs = Math.abs(deg);
        return sign * (abs + (min / 60.0) + (sec / 3600));
    },

    decode: function (value) {
        var pattern = "";

        // deg
        pattern += "(-?\\d+)";
        pattern += this.CHAR_DEG;
        pattern += "\\s*";

        // min
        pattern += "(\\d+)";
        pattern += this.CHAR_MIN;
        pattern += "\\s*";

        // sec
        pattern += "(\\d+(?:\\.\\d+)?)";
        pattern += this.CHAR_SEC;

        return value.match(new RegExp(pattern));
    },

    getLonDec: function () {
        return this.lonDec;
    },

    getLatDec: function () {
        return this.latDec;
    },

    getLonDeg: function () {
        return this.lonDeg;
    },

    getLatDeg: function () {
        return this.latDeg;
    }

};



function AggiornaLatLon() {
    //var tipo = "";
    //tipo = $("#tipoGrado").val();

    //var Lat_Degrees, Lat_Minutes, Lat_Seconds, Lon_Degrees, Lon_Minutes, Lon_Seconds, sLat, sLon;
    //var Lat = 0;  
    //var Lon = 0;


    //if (tipo == "TG") {

    //    Lat_Degrees = $("#Lat_Degrees").val();
    //    Lat_Minutes = $("#Lat_Minutes").val();
    //    Lat_Seconds = $("#Lat_Seconds").val();

    //    Lon_Degrees = $("#Lon_Degrees").val();
    //    Lon_Minutes = $("#Lon_Minutes").val();
    //    Lon_Seconds = $("#Lon_Seconds").val();

    //    if (Lat_Degrees != "" && Lat_Minutes != "" && Lat_Seconds != "" &&
    //        Lon_Degrees != "" && Lon_Minutes != "" && Lon_Seconds != "") {


    //        var point1 = new GeoPoint(Lon_Degrees + '° ' + Lon_Minutes + '\' ' + Lon_Seconds + '"', Lat_Degrees + '° ' + Lat_Minutes + '\' ' + Lat_Seconds + '"');

    //        $("#lat_cerca").val(point1.getLatDec());
    //        $("#long_cerca").val(point1.getLonDec());

    //    }

    //} else {

    //    sLat = $("#lat_cerca").val();
    //    sLon = $("#long_cerca").val();

    //    Lat = parseFloat(sLat);
    //    Lon = parseFloat(sLon);

    //    if (Lat != "" && Lon != "") {

    //        var point2 = new GeoPoint(Lat, Lon);

    //        var sDegMinSec_Long = point2.lonDeg;
    //        interpretaLatLong(sDegMinSec_Long, "Lat");

    //        var sDegMinSec_Lat = point2.latDeg;
    //        interpretaLatLong(sDegMinSec_Lat, "Lon");

    //    }

    //}
}

            
function interpretaLatLong(sLatLong, DoveImpostareIlValore) {

    //var ss1 = "";
    //ss1 = sLatLong.toString();
    //var vLatLong = ss1.split(" ");

    //$("#" + DoveImpostareIlValore + "_Degrees").val(vLatLong[0].replace( "°", ""));
    //$("#" + DoveImpostareIlValore + "_Minutes").val(vLatLong[1].replace( "'", ""));
    //$("#" + DoveImpostareIlValore + "_Seconds").val(vLatLong[2].replace( "\"", ""));

}


function clickPiu_Indirizzi() {

    var indirBottomAperto = 20;
    var indirBottomChiuso = 0;

    var indirHeighAperto = 140;
    var indirHeighChiuso = 35;


    if ($("#espandi_indirizzo").attr("alt") == 'piu') {

        /* era chiuso, lo apri */
        $("#espandi_indirizzo").attr("alt", 'meno');
        var src = $("#espandi_indirizzo").attr('title').split("|")[1];
        $("#espandi_indirizzo").attr("src", src);

        $("#dialog_Ricerca").css({
            height: indirHeighAperto,
            bottom: indirBottomAperto
        }).show(); -$("#dialog_Ricerca").css({
            bottom: indirBottomAperto
        }).show();
        $("#sitebar_indirizzo").css({
            bottom: indirBottomAperto,
            height: indirHeighAperto - 5
        });

    } else {
        /* era aperto, lo chiudi */
        $("#espandi_indirizzo").attr("alt", 'piu');
        var src = $("#espandi_indirizzo").attr('title').split("|")[0];
        $("#espandi_indirizzo").attr("src", src);

        $("#dialog_Ricerca").css({
            bottom: indirBottomChiuso,
            height: indirHeighChiuso
        }).show();

        $("#sitebar_indirizzo").css({
            height: 20
        });
        PosizionaRicercaIndirizzo();
    }
}


function CoordFromGPS() {

    if (navigator.geolocation) {

        navigator.geolocation.getCurrentPosition(function (position) {

            let lat = position.coords.latitude;
            let lng = position.coords.longitude;

            //$("#lat_cerca").val(pos.lat);
            //$("#long_cerca").val(pos.lng);
            //AggiornaLatLon();
            //ricercaCoordinate();

            $("#find_lat").data("kendoCoordMaskedTextBox").setDecimalValue(lat);
            $("#find_lng").data("kendoCoordMaskedTextBox").setDecimalValue(lng);

            //' VAnni: 9/3/2020: se mi trovo in modalità inserimento punto attivata allora inserisco i dati nel campo hidden (esempi)
            //   "(44.170385569408005, 12.264000353467504)|"
            //   "(44.170385569408005, 12.264000353467504)|(44.17201391739323, 12.267230633761956)|(44.169827613952855, 12.271672774621587)|"
            if ($("#multipointHeader").hasClass("green")) {
                $("#hiddenMultipoint").val($("#hiddenMultipoint").val() + lat.toString() + ", " + lng.toString() + "|");
            }

            mappa.CercaCoordinate(lat, lng);

        }, function () {
            //handleLocationError(true, infoWindow, map.getCenter());
        });
    } else {
        // Browser doesn't support Geolocation
        //handleLocationError(false, infoWindow, map.getCenter());
    }


}

function IndirizzoLatLong() {

    //var sLat = $("#lat_cerca").val();
    //var sLon = $("#long_cerca").val();

    //if (sLat != '' && sLon != '') {

    //    var Lat = parseFloat(sLat);
    //    var Lon = parseFloat(sLon);
                    
    //    mappa.identificaIndirizziLatLong(Lat, Lon, "#address");

                    
    //}

    //GABRIELE
    let lat = $("#find_lat").data("kendoCoordMaskedTextBox").decimalValue();
    let lng = $("#find_lng").data("kendoCoordMaskedTextBox").decimalValue();
    mappa.identificaIndirizziLatLong(lat, lng, "#address");
}



//supporto toolbar

function clickRighello() {

    if ($("#righello").is(":checked")) {
        $("#righello").prop("checked", "");

    } else {
        $("#righello").prop("checked", "checked");

    }
    addruler($('#righello'));
}

/* AB */
function clickAB() {
    if ($("#chkAB").is(":checked")) {
        $("#chkAB").prop("checked", "");
    } else {
        $("#chkAB").prop("checked", "checked");
    }
    addAB($('#chkAB'), $('#hiddenA'), $('#hiddenB'), null, null, null, null);
}


/* MULTIPOINT */
function clickMP() {
    if ($("#chkMP").is(":checked")) {
        $("#chkMP").prop("checked", "");
    } else {
        $("#chkMP").prop("checked", "checked");
    }

}


function gminus() {
    var app = 0;
    app = parseInt($("#txtAggregaEntitaGrafichePerPianificazioneScartoM").val());
    if (app > 0)
        app -= 1;
    $("#txtAggregaEntitaGrafichePerPianificazioneScartoM").val(app);
}

function gplus() {
    var app = 0;
    app = parseInt($("#txtAggregaEntitaGrafichePerPianificazioneScartoM").val());
    if (app < 20)
        app += 1;
    $("#txtAggregaEntitaGrafichePerPianificazioneScartoM").val(app);
}

