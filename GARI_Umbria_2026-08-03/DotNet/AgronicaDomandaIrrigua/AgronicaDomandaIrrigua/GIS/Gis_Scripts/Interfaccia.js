var interfaccia = {
    cosaStoDisegnando: google.maps.drawing.OverlayType.POLYGON,
    scala_colori_Array: new Array(),
    loading: function (val) {
        if (val == true) {
            try {
                WaitFrame.show();
            } catch (e) {
                $('#WaitFrame').show();
            }

        } else {
            try {
                WaitFrame.hide();
            } catch (e) {
                $('#WaitFrame').hide();
            }
            
        }
    },
    initDatePiker: function () {
        //date
        try {
            $('.datepiker').datepicker({
                dateFormat: 'dd/mm/yy',
                disabled: false,
                changeMonth: true,
                changeYear: true
            });
            $.datepicker.regional['it'];
        } catch (e) {

        }
       
    }
    ,
    collassaDatiTabella: function (tabella, hidden) {
        var rval = "";
        if ($(tabella).html() != null) {
            var rows = $(tabella + " tr"); // skip the header row        
            rows.each(function (index) {
                var combo = $(this).find('td:eq(0) input');
                if (combo.is(':checked')) {
                    var toAdd = $(hidden).val() + combo.attr('id').replace('ckSp_', '') + ",";
                    $(hidden).val(toAdd);
                }
            });
            rval = $(hidden).val() + "-1";
            $(hidden).val("");
        }
        return rval;
    }
    ,
    leggiColoreDaDiv: function (chiaveDiv, valore) {
        if (chiaveDiv == "") {
            return placeLivelli[shape.glayerDoveDisegno].colore_1;
        }
        console.warn('leggiColoreDaDiv');
        var f = 0.0;
        var fValore = numeri.arrotonda(parseFloat(valore), 2);
        //var objDiv = $('[id="' + chiaveDiv + '"]').children();
        var objDiv = $('[id="' + chiaveDiv + '"]').find("td");
        var rval = "";
        var i = 0;
        var el = 0;
        for (var i = 0; i < objDiv.length; i++) {
            if (!$(objDiv[i]).attr('valore_associato')) {
                if (parseFloat(valore) <= parseFloat($(objDiv[i]).html())) {
                    el = i;
                    var rgba = $(objDiv[i]).first().css('background-color');
                    var a = rgba.toString().replace(')', '').replace('rgba(', '').split(',');
                    rval = colori.RGB2Color(parseInt(a[0].replace(' ', '')), parseInt(a[1].replace(' ', '')), parseInt(a[2].replace(' ', '')));
                    return rval;
                }
            }
            else {
                if (parseFloat(valore) == parseFloat($(objDiv[i]).attr('valore_associato'))) {
                    el = i;
                    var rgba = $(objDiv[i]).first().css('background-color');
                    var a = rgba.toString().replace(')', '').replace('rgba(', '').split(',');
                    rval = colori.RGB2Color(parseInt(a[0].replace(' ', '')), parseInt(a[1].replace(' ', '')), parseInt(a[2].replace(' ', '')));
                }
            }
        }
        if (rval != "")
            return rval;
        else
            return "CCCCCC";

        //        var max = 0;
        //        for (var i = 0; i < this.scala_colori_Array.length; i++) {

        //            if (this.scala_colori_Array[i].id == chiaveDiv && this.scala_colori_Array[i].nomePadre == nome) {
        //                max = i;

        //                f = parseFloat(this.scala_colori_Array[i].valore_min);

        //                var a = this.scala_colori_Array[i].colore.toString().replace(')', '').replace('rgba(', '').split(',');
        //                var rval = colori.RGB2Color(parseInt(a[0].replace(' ', '')), parseInt(a[1].replace(' ', '')), parseInt(a[2].replace(' ', '')));

        //                if (f != 999999990 && f >= fValore) {
        //                    return rval;
        //                }
        //            }
        //        }

        //        if (!this.scala_colori_Array)
        //            return '#ffffff';
        //        if (this.scala_colori_Array.length == 0)
        //            return '#ffffff';
        //        var b = this.scala_colori_Array[max].colore.toString().replace(')', '').replace('rgba(', '').split(',');


        //        return colori.RGB2Color(parseInt(b[0].replace(' ', '')), parseInt(b[1].replace(' ', '')), parseInt(b[2].replace(' ', '')));
    }
,
    //    leggiColoreDaDiv: function (chiaveDiv, valore, nome) {
    //        console.warn('leggiColoreDaDiv');
    //        var f = 0.0;
    //        var fValore = numeri.arrotonda(parseFloat(valore), 2);


    //        var max = 0;
    //        for (var i = 0; i < this.scala_colori_Array.length; i++) {

    //            if (this.scala_colori_Array[i].id == chiaveDiv && this.scala_colori_Array[i].nomePadre == nome) {
    //                max = i;

    //                f = parseFloat(this.scala_colori_Array[i].valore_min);

    //                var a = this.scala_colori_Array[i].colore.toString().replace(')', '').replace('rgba(', '').split(',');
    //                var rval = colori.RGB2Color(parseInt(a[0].replace(' ', '')), parseInt(a[1].replace(' ', '')), parseInt(a[2].replace(' ', '')));

    //                if (f != 999999990 && f >= fValore) {
    //                    return rval;
    //                }
    //            }
    //        }

    //        if (!this.scala_colori_Array)
    //            return '#ffffff';
    //        if (this.scala_colori_Array.length == 0)
    //            return '#ffffff';
    //        var b = this.scala_colori_Array[max].colore.toString().replace(')', '').replace('rgba(', '').split(',');


    //        return colori.RGB2Color(parseInt(b[0].replace(' ', '')), parseInt(b[1].replace(' ', '')), parseInt(b[2].replace(' ', '')));
    //    }
    //,
    leggiRaggioDaDiv: function (chiaveDiv, valore) {
        if (chiaveDiv == "") {
            //return 'FFFFFF';
            return 100;
        }
        console.warn('leggiColoreDaDiv');
        var f = 0.0;
        var fValore = numeri.arrotonda(parseFloat(valore), 2);
        var objDiv = $('[id="' + chiaveDiv + '"]').children();
        //var objDiv = $('[id="' + chiaveDiv + '"]').find("td"); Come per il colore????
        var rval = "";
        var i = 0;
        var el = 0;
        for (var i = 0; i < objDiv.length; i++) {
            if (parseFloat(valore) == parseFloat($(objDiv[i]).attr('valore_associato'))) {
                el = i;
                var rgba = $(objDiv[i]).first().css('background-color');
                var a = rgba.toString().replace(')', '').replace('rgba(', '').split(',');
                rval = colori.RGB2Color(parseInt(a[0].replace(' ', '')), parseInt(a[1].replace(' ', '')), parseInt(a[2].replace(' ', '')));
            }
        }



        var r_min = 100;
        var r_max = 1000;
        var r = (r_max - r_min) / objDiv.length * (el + 1) + r_min;


        return r;
    }
,
    CreaPannelloColore: function (descrizione, datiViste) {
        if (datiViste != undefined) {

            this.CreaPannelloColore_Disponi(datiViste.length);

            var html = "";
            //            console.log('CreaPannelloColore');
            //            console.log(datiViste);
            for (var i = 0; i < datiViste.length; i++) {
                //utility.log(datiViste[i]); todo, verifica appidrate che non va!
                html = html + this.CreaPannelloColore_Disegna(datiViste[i].nome, datiViste[i].colore_primario, datiViste[i].colore_secondario, datiViste[i].v_min, datiViste[i].v_max, datiViste[i].varianza, datiViste[i].tilelabels, datiViste[i].tilelayerpadre);
            }

            $('#scala_colori').html(html);

            this.selezionaDivVista();
        }

    }
    ,
    selezionaDivVista: function () {

        $('.pannello_scala_colori').hide();
        $("[id='" + $("#cmbViste option:selected").text() + "']:eq(0)").show(); //'  Vanni, 08/05/2015 11:39:14: aggiunto :eq(0) per mostrare solo la prima

    }
,
    CreaPannelloColore_Disponi: function (conteggio) {

        var kW = $("#scala_colori_contenitore").data("kendoWindow");

        var cssPosition = "relative";
        var cssLeft = 0;
        var cssWidth = 0;

        var pp = $("#map").offset();


        if (kW === undefined) {

            cssPosition = "absolute";
            cssLeft = pp.left;            
            cssWidth = $('#map').width() - 50 - $('#dialog_Ricerca').width();
            
        } else {
            cssWidth = $("#scala_colori_contenitore_wnd_title").width() - 20;
        }


        $('#scala_colori_contenitore').width(cssWidth);
        //$('#scala_colori').width(cssWidth);
        
        $("#scala_colori_contenitore").css({
            position: cssPosition,
            bottom: 0,
            left: cssLeft
        });

        $("#SetBack").css({
            position: cssPosition,
            bottom: 70,
            left: cssLeft
        });


    }
    ,

    GetApp: function (v_min, var_valor, i, listaTileLabels) {
        var app = "";
        var ll_len = listaTileLabels.length;
        if (ll_len == 0) {
            app = numeri.arrotonda((v_min + (var_valor * i)), 2);
        } else {


            if (i < ll_len) {
                app = listaTileLabels[i].label;
            }
        }

        return app;
    },
    CreaPannelloColore_Disegna: function (id, colore1, colore2, v_min, v_max, varianza, listaTileLabels, nomePadre) {
        //        console.log('CreaPannelloColore_Disegna' + colore1 + '  ' + colore2 + '  ' + v_min + '  ' + v_max + ' ' + varianza);
        //calcolo l'altezza di ogni div
        var altezza = parseInt($('#scala_colori').width() / varianza);

        var range = [{
            0: parseInt(colori.hexToRgb(colore1).split("|")[0]),
            1: parseInt(colori.hexToRgb(colore2).split("|")[0])
        }, {
            0: parseInt(colori.hexToRgb(colore1).split("|")[1]),
            1: parseInt(colori.hexToRgb(colore2).split("|")[1])
        }, {
            0: parseInt(colori.hexToRgb(colore1).split("|")[2]),
            1: parseInt(colori.hexToRgb(colore2).split("|")[2])
        }];

        v_min = parseFloat(v_min);
        v_max = parseFloat(v_max);
        var no_label = (v_max < v_min);

        var var_valor = parseFloat((v_max - v_min) / (varianza - 1));
        var i = 0;

        var var_r = (range[0]['1'] - range[0]['0']) / (varianza - 1);
        var var_g = (range[1]['1'] - range[1]['0']) / (varianza - 1);
        var var_b = (range[2]['1'] - range[2]['0']) / (varianza - 1);

        //var html = "<div id='" + id + "' class='pannello_scala_colori'>";
        var html = "<div id='" + id + "' class='pannello_scala_colori'><table style='width: 100%;'><tr style='height: 30px;'>";
        for (i = 0; i < varianza; i++) {

            var r = parseInt(range[0]['0'] + parseInt(var_r * i));
            var g = parseInt(range[1]['0'] + parseInt(var_g * i));
            var b = parseInt(range[2]['0'] + parseInt(var_b * i));
            //var bw = (r * 0.299 + g * 0.587 + b * 0.114) > 186 ? '#000000' : '#FFFFFF';
            var bw = (r * 0.299 + g * 0.587 + b * 0.114) > 160 ? '#000000' : '#FFFFFF';
            var colore = 'rgba(' + r + ', ' + g + ', ' + b + ', ' + CanaleAlfaDidascalia + ')';
           
            //html = html + "<div  style='height:30px; width:" + altezza + "px; background-color:" + colore + "; float:left;'";
            //if (!!listaTileLabels[i])
            //    html = html + " valore_associato='" + listaTileLabels[i].valore_associato + "'";
            //html = html + ">";
            html = html + "<td style='width: 1%; background-color:" + colore + "; color: " + bw + "'";
            if (!!listaTileLabels[i])
                html = html + " valore_associato='" + listaTileLabels[i].valore_associato + "'";
            html = html + ">";

            //ottengo la scritta da posizionare ..
            if (!no_label) {
                var app = "";
                app = this.GetApp(v_min, var_valor, i, listaTileLabels);
                html = html + app;
            }

            //html = html + "</div>";
            html = html + "</td>";

            interfaccia.scala_colori_Array.push({
                id: id,
                valore_min: app,
                colore: colore,
                nomePadre: nomePadre
            });


        }


        //html = html + "</div><div style='clear:both'></div>";
        html = html + "</tr></table></div>";
        return html;
    }

,


    switchDrawingMode: function (cosaVoglioDisegnare) {
        utility.log("switchDrawingMode(" + cosaVoglioDisegnare + ")");

        //    if (!verificaSePossoDisegnareQuesto(cosaVoglioDisegnare))
        //        return false;

        this.cosaStoDisegnando = cosaVoglioDisegnare;
        ripulisciImmaginiCosaDisegno();

        switch (cosaVoglioDisegnare) {
            case google.maps.drawing.OverlayType.POLYGON:

                $("#chkMP").removeAttr("checked");
                $("#img_poligono").show();
                break;

            case google.maps.drawing.OverlayType.MARKER:
                $("#chkMP").prop("checked", "checked");
                $("#img_multipoint").show();
                break;
        }

        mappa.drawingManager.setDrawingMode(this.cosaStoDisegnando);
        utility.log("drawingManager.setDrawingMode(" + this.cosaStoDisegnando + ");")
        $("#divCosaDisegno").hide();
    }
 ,
    /* Attiva Disattiva label */
    CreaPoligono: function () {
        if ($("#hiddenMultipoint").val() != "") {
            dialogMultipoint();
        }
        else {
            if (shape.glayerDoveDisegnoSuTipologiaStandard == "-1") {
                shape.settaLayerDoveDisegnare(undefined, undefined, undefined, "19", "Impianto32.png", "IMPIANTI");
            }
            mappa.drawingManager.setDrawingMode(interfaccia.cosaStoDisegnando);
        }
    }
    ,
    chekLayers: function (bAutofit) {

        if (bAutofit == undefined)
            bAutofit = true;

        var latlngAutoFit = new google.maps.LatLngBounds();


        //scorro tutto il 
        var idAttivi = new Array();
        $('.chkLayer').each(function () {
            if ($(this).is(':checked')) {
                idAttivi.push($(this).attr('id'));
            }
        });

        var trovatoDettagli = false;
        for (var i = 0; i < mappa.Livelli.length; i++) {
            for (var j = 0; j < idAttivi.length; j++) {
                var el = mappa.Livelli[i];
                if (el.id == idAttivi[j]) {
                    if (!!el.datiViste) {
                        trovatoDettagli = true;
                    }

                    //se l'ho trovato aggiungo i punti per l auotfit

                    try {
                        for (var k = 0; k < el.circle.length; k++) {
                            latlngAutoFit.extend(new google.maps.LatLng(el.circle[k].center.A, el.circle[k].center.F));
                        }

                        for (var k = 0; k < el.punti.length; k++) {
                            latlngAutoFit.extend(new google.maps.LatLng(el.punti[k].position.lat(), el.punti[k].position.lng()));
                        }
                        
                        for (var k = 0; k < el.poligoni.length; k++) {

                            //' VAnni: 23/10/2018: correzione api 3.34
                            let tmp_ltlng = el.poligoni[k].latLngs.b;
                            if (tmp_ltlng !== undefined) {
                                tmp_ltlng = el.poligoni[k].latLngs.b[0].b[0];
                            } else {
                                tmp_ltlng = el.poligoni[k].latLngs.j[0].j[0];
                            }
                            latlngAutoFit.extend(new google.maps.LatLng(tmp_ltlng.lat(), tmp_ltlng.lng() ) );
                        }

                    }

                    catch (e) {

                    }
                }
            }
        }


        if (trovatoDettagli == true)
        { $('#scala_colori_contenitore').show(); }
        else
        { $('#scala_colori_contenitore').hide(); }

        if (bAutofit) {

            try {

                if (!latlngAutoFit.isEmpty())
                    mappa.elemenotMappa.setCenter(latlngAutoFit.getCenter(), mappa.elemenotMappa.fitBounds(latlngAutoFit));


            }
            catch (e) {

            }
        }

    }
}