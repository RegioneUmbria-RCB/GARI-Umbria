
/* ClientJs/Shape.js */

var shape = {
    selectedShape: null,
    selectedShapeArray: new Array(),
    selectedPointsArray: new Array(),
    selectedShape_Copia: null,
    selectedColor: null,
    gTipologiaLayerDoveDisegno: "-1",
    glayerDoveDisegno: "-1",
    glayerDoveDisegnoSuTipologiaStandard: "-1",

    selezionaIndiceLayerDoveScrivoDaOggettoGrafico: function () {
        utility.warn('selezionaIndiceLayerDoveScrivoDaOggettoGrafico');
        let s = this.selectedShape;
        if (!!s) {
            this.settaLayerDoveDisegnare(
                s.layerDiAppartenenza, 
                s.icona32, 
                s.layerDiAppartenenza_nome,
                s.StandardEntita_layerDiAppartenenza,
                s.StandardEntita_layerDiAppartenenza_Icona32,
                s.StandardEntita_layerDiAppartenenza_Des
            );
        }
    },
    settaLayerDoveDisegnare_ui_testata: function (ico32DoveDisegno, nomeLayDoveDisegno) {
        if (!ico32DoveDisegno.startsWith("../AB")) {
            ico32DoveDisegno = "../AB_Immagini/icone/" + ico32DoveDisegno;
        }        
        $("#LayerDoveDisegno").attr('src', ico32DoveDisegno);
        $("#imgpop_up_impianto_icolayer").attr('src', ico32DoveDisegno);
        //$("#testoLayerDoveDisegno").html(nomeLayDoveDisegno);
        $("#navBarLayerAttivoDescrizione").html(nomeLayDoveDisegno);
        $("#pop_up_impianto_nomelayer").html("Layer: " + nomeLayDoveDisegno);
        $("#testoLayerDoveDisegno").html("<span style='font-weight: bold'>Disegna su: </span>" + nomeLayDoveDisegno);
    },
    LayerUiEvidenza: function (idLayer) {
        $("#chkLayerRow_" + idLayer).addClass("layerSelezionato");
    },
    LayerUiPulisciEvidenza: function () {
        $(".chkLayerRow").each(function () {
            $(this).removeClass("layerSelezionato");
        });
    },
    settaLayerDoveDisegnare: function (idLayer, ico32, nomeLay,
        StandardEntita_layerDiAppartenenza, StandardEntita_layerDiAppartenenza_Icona32, StandardEntita_layerDiAppartenenza_Des) {

        var prosegui = false;

        var lTipologiaLayer = TipologiaLayer();

        //' VAnni: 29/3/2021: chiaramente prosegue soltanto se si tratta di un layer differente (o in tipologia layer differente)
        if (idLayer === undefined || this.glayerDoveDisegno !== idLayer || this.gTipologiaLayerDoveDisegno !== lTipologiaLayer ) {
            prosegui = true;
        }

        if (!prosegui) {
            return false;
        }

        this.gTipologiaLayerDoveDisegno = lTipologiaLayer;

        //Evidenzio il layer        
        this.LayerUiPulisciEvidenza();


        let tl = parseInt(this.gTipologiaLayerDoveDisegno);

        //parte 1: gestione dei layer su qualsiasi tipologia

        //sarà undefined soltanto se sto selezionando "nuovo disegno" (proviene da interfaccia.creaPoligono())
        //in tal caso imposto la variabile solo se sono nella tipologia standard
        if (tl === Enum_tipologia_layer.Standard_Entità.value && idLayer === undefined) {
            $("#chkLayerRow_" + StandardEntita_layerDiAppartenenza).addClass("layerSelezionato");
            this.glayerDoveDisegno = StandardEntita_layerDiAppartenenza.toString();
        } else {

            //se c'è un layer da impostare provvedo..
            if (idLayer !== undefined) {
                this.LayerUiEvidenza(idLayer);
                this.glayerDoveDisegno = idLayer.toString();
            }
        }


        if (!$("#pulsantieraEsterna_tool").hasClass("layerSelezionato")) {
            $("#pulsantieraEsterna_tool").addClass("layerSelezionato");
        }        
        //fine evidenza


        utility.warn("settaLayerDoveDisegnare(idLayer:=" + idLayer + ", ico32:=" + ico32 + ", nomeLay:=" + nomeLay + ")");
        
        //parte 2: gestione dei layer corrispondenti a quello selezionato su tipologia standard

        //sulla tipologia std per LEGGE non possono che coincidere i due layer
        if (tl === Enum_tipologia_layer.Standard_Entità.value && idLayer !== undefined) {
            StandardEntita_layerDiAppartenenza = idLayer.toString();
            StandardEntita_layerDiAppartenenza_Des = nomeLay;
            StandardEntita_layerDiAppartenenza_Icona32 = ico32;
        }


        if (StandardEntita_layerDiAppartenenza !== undefined) {
            this.glayerDoveDisegnoSuTipologiaStandard = StandardEntita_layerDiAppartenenza;        
            this.settaLayerDoveDisegnare_ui_testata(StandardEntita_layerDiAppartenenza_Icona32, StandardEntita_layerDiAppartenenza_Des);
        }            
        

        this.decidiCosaPossoDisegnare();

        
        if (nomeLay !== undefined) {
            this.ImpostaParametri_x_CmbViste(nomeLay);
        }

        mappa.ImpostaShape(true, false);
    }
    ,
    ImpostaParametri_x_CmbViste: function (nomeLay) {
        utility.warn('ImpostaParametri_x_CmbViste');
        var queryTiles = layers.filter(function (x) { return x['nome'] === nomeLay });

        var i = 0;
        xComboCorrenti = '';
        xCreazioneCombo = '';
        if (!queryTiles[0].tiles)
            return 0;
        for (var i = 0; i < queryTiles[0].tiles.length; i++) {
            xComboCorrenti = xComboCorrenti + queryTiles[0].tiles[i].nome + "§";
            xCreazioneCombo = xCreazioneCombo + queryTiles[0].tiles[i].nome + "§ 0|";
        }

        ID_Overlay = queryTiles[0].id;
        //xCreazioneCombo = queryTiles[0].
        //        console.log('xComboCorrenti');
        //        console.log(xComboCorrenti);
        cmbViste_change();
        mappa.RicreaPannelloColore();

        //mappa.ImpostaShape(true);


        //       {        
       
        

        //        var xCreazioneCombo = 'Version§ 5.10.038|GPS_Status§ 2|Status_Txt§ DGPS|Swath§ 22|Height§ -5,476|DateClosed§ 19/03/2012 00:00:00|TimeClosed§ 09:43:49am|AppldRate§ 13298,249|Moisture§ |Material§ Default_AppMaterial|MaterialID§ -1|Speed§ 0,013';

        //        PopolaComboPannelloColore(xCreazioneCombo, xComboCorrenti);
    }
    ,
    nascondoMenuCosaDisegno: function () {
        $("#selimg_poligono").hide();
        $("#selimg_multipoint").hide();
    }
    ,
    mostraMenuCosaDisegnoPreparaStrumenti: function () {
        this.nascondoMenuCosaDisegno();
        this.decidiCosaPossoDisegnare();

        // questo si può sempre disegnare?
        $("#selimg_poligono").show();
        $("#divCosaDisegno").show();

        for (var i = 0; i < mappa.cosaPossoDisegnare.length; i++) {
            switch (mappa.cosaPossoDisegnare[i]) {
                case google.maps.drawing.OverlayType.POLYGON:
                    $("#selimg_poligono").show();
                    break;
                case google.maps.drawing.OverlayType.MARKER:
                    $("#selimg_multipoint").show();
                    break;
            }
        }
    },
    decidiCosaPossoDisegnare: function () {

        let tl = parseInt(TipologiaLayer());
        mappa.cosaPossoDisegnare = new Array();


        utility.log("decidiCosaPossoDisegnare - glayerDoveDisegnoSuTipologiaStandard = " + this.glayerDoveDisegnoSuTipologiaStandard + " - gisTipoOggettoXLayer.Length = " + mappa.gisTipoOggettoXLayer.Length);

        //a seconda della tipologia layer decido cosa posso disegnare.
        switch (tl) {
            case Enum_tipologia_layer.Gruppo_Colturale.value:
                mappa.cosaPossoDisegnare.push(google.maps.drawing.OverlayType.POLYGON);
                break;

            default:

                // per layer
                for (var i = 0; i < mappa.gisTipoOggettoXLayer.length; i++) {

                    utility.log("gisTipoOggettoXLayer[" + i + "].layer = " + mappa.gisTipoOggettoXLayer[i].layer);
                    utility.log("gisTipoOggettoXLayer[" + i + "].GIS_TipoOggetto_Cod = " + mappa.gisTipoOggettoXLayer[i].GIS_TipoOggetto_Cod);

                    if (mappa.gisTipoOggettoXLayer[i].layer == this.glayerDoveDisegnoSuTipologiaStandard) {

                        var lChePossoDisegnare = mappa.gisTipoOggettoXLayer[i].GIS_TipoOggetto_Cod;
                        if (!mappa.cosaPossoDisegnare.indexOf(lChePossoDisegnare) > -1) {

                            mappa.cosaPossoDisegnare.push(lChePossoDisegnare)
                            utility.log("cosaPossoDisegnare.push(" + lChePossoDisegnare + ")");
                        }
                    }
                }
                //per operazione di agenda
                break;
        }


    }
}



