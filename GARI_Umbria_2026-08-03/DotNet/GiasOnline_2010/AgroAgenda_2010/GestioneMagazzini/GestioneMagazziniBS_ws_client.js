
function CercaGiacenze(options) {

    var Sa_Cod = getSaCod(true);
    var Elem_Cod = parseInt(Get_KendoDDLValue("ddlCategoria", 0));
    var CodiceProdotto = $(cIdPro_Cod).val();
    var Data = $(cIdDataOperazione).val();

    var Fabbricato = getFabbricatoSelezionato(true);
    var Fabbricato_Cod = Fabbricato.Id_Destinazione;
    if (Fabbricato_Cod != 0) {
        Sa_Cod = Fabbricato.Sa_Cod;
    }


    let filtroLotto = null;

    if (getKendoSwitch("CheckBoxFiltraPerLotto") === true) {
        filtroLotto = $("#TxtLotto").val();
    }


    var parametri = {
        Sa_Cod: Sa_Cod,
        strFabbricato_Cod: Fabbricato_Cod,
        Tipo_Fabbricato: 20,
        Elem_Cod: Elem_Cod,
        NomeProdotto: $("#TxtProdotto").val(),
        CodArticolo: $("#TxtCodArticolo").val(),
        CodiceProdotto: CodiceProdotto,
        Lotto: filtroLotto,
        Data: Data,
        VisualizzaGiacenzeZero: getKendoSwitch("CheckBoxGiacenze0"),
        CifreArrotondamento: Get_KendoDDLValue("ddlArrotondamento", 2),
        Kendo: true,
        ValorizzaProdotto: getKendoSwitch("CheckBoxValorizzaProdotto")
    };

    var url = "GestioneMagazziniBS.aspx/CaricaGiacenze";

    chiamataAjaxSincronaAsincrona(url, parametri, options);

}

function CercaMovimenti(options, parametriPerLettura) {

    var parametri = null;

    if (typeof parametriPerLettura !== "undefined") {

        parametri = parametriPerLettura[0];

    } else {
    
        var Sa_Cod = getSaCod(true);
        var Elem_Cod = parseInt(Get_KendoDDLValue("ddlCategoria", 0));
        var Pro_Cod = $(cIdPro_Cod).val();
        var DataInizio = $(cIdDataInizio).val();
        var DataFine = $(cIdDataFine).val();

        var Fabbricato = getFabbricatoSelezionato(true);
        var Fabbricato_Cod = Fabbricato.Id_Destinazione;
        if (Fabbricato_Cod != 0) {
            Sa_Cod = Fabbricato.Sa_Cod;
        }

        let filtroLotto = null;

        if (getKendoSwitch("CheckBoxFiltraPerLotto") === true) {
            filtroLotto = $("#TxtLotto").val();
        }

        parametri = {
            Sa_Cod: Sa_Cod,
            strFabbricato_Cod: Fabbricato_Cod,
            Tipo_Fabbricato: 20,
            Elem_Cod: Elem_Cod,
            NomeProdotto: $("#TxtProdotto").val(),
            Cod_Articolo: $("#TxtCodArticolo").val(),
            Pro_Cod: Pro_Cod,
            Mat_Cod: "",
            Lotto: filtroLotto,
            Cal_Cod: "",
            Cod_Progetto: "",
            Udm_Cod: "",
            DataInizio: DataInizio,
            DataFine: DataFine,
            VisualizzaCarichi: $("#CheckBoxCarichi").is(":checked"),
            VisualizzaScarichi: $("#CheckBoxScarichi").is(":checked"),
            CifreArrotondamento: Get_KendoDDLValue("ddlArrotondamento", 2),
            Kendo: true
        };

    }

    var url = "GestioneMagazziniBS.aspx/CaricaMovimenti";

    chiamataAjaxSincronaAsincrona(url, parametri, options);

}

function chiamataAjaxSincronaAsincrona(url, parametri, options) {

    if (ricercaSincrona === true) {

        //Chiamata Sincrona

        let tabellaMagLocale = tabellaMag;
        let paginaTabellaMagLocale = paginaTabellaMag;
        let dimensionePaginaTabellaMagLocale = dimensionePaginaTabellaMag;

        WaitFrame.show();

        setTimeout(function () {

            ajaxAgronicaSync(url,
                JSON.stringify(parametri),
                false,
                function (risposta) {
                    var risp = JSON.parse(risposta.RispostaStringa);
                    options.success(risp);
                },
                null,
                null,
                false);

            WaitFrame.hide();

            if (tabellaMagLocale &&
                KendoGrid(tabellaMagLocale) !== undefined &&
                KendoGrid(tabellaMagLocale).dataSource !== undefined &&
                paginaTabellaMagLocale > 0) {

                //Imposto dimensione pagina
                if (dimensionePaginaTabellaMagLocale) {

                    KendoGrid(tabellaMagLocale).dataSource.pageSize(dimensionePaginaTabellaMagLocale);

                    //Imposto pagina
                    if (KendoGrid(tabellaMagLocale).dataSource.totalPages() >= paginaTabellaMagLocale) {

                        KendoGrid(tabellaMagLocale).dataSource.page(paginaTabellaMagLocale);

                    }

                }                

            }

        }, 150

        );

    } else {

        //Chiamata Asincrona

        ajaxAgronica(url,
            JSON.stringify(parametri),
            function (risposta) {
                var risp = JSON.parse(risposta.RispostaStringa);
                options.success(risp);
            },
            null);

    }

}

function TrasferimentoMovContabili() {

    //controllo se ho selezionato il magazzino
    let codMagazzino = Get_KendoDDLValue("ddlMagazzini_Trasferimento");
    if (codMagazzino === "" || codMagazzino === "0") {
        kendo.alert(TraduzioneMultiResx(gestioneMagazziniResx, "SelezionareMagazzinoDiDestinazione", "È necessario selezionare il magazzino di destinazione"));
        return false;
    }

    var dati = [];
    var grid = KendoGrid("tabMovimenti");
    grid.select().each(function () {
        let item = grid.dataItem(this);
        dati.push(item);
    });

    let chiave = new Array();
    for (let i = 0; i < dati.length; i++) {
        if (kendo_grid) {
            chiave.push(dati[i].chiave_movimenti.replace(/-/g, "/"));
        } else {
            chiave.push(dati[i].chiave_movimenti);
        }
    }

    ajaxAgronica("./GestioneMagazziniBS.aspx/TrasferimentoMovContabili",
        JSON.stringify({ chiaveMagazzino: codMagazzino, listaChiaviMov: chiave }),
        function (risposta) {
            $("#chiudiTrasferimentoMovContabili").click();
            setTimeout(function () {
                btn_ricerca_movimenti_click();
            }, 200);
        }, function(risposta) {
            console.debug(risposta);
        });
}

function Gestione_Trasferimento_DettagliContabili() {

    SalvaParametri();

    //controllo che tutti i dettagli selezionati siano dello stesso tipo e di tipo ddt
    let dati = [];
    const grid = KendoGrid("tabMovimenti");
    if (grid !== undefined && grid !== null) {
        grid.select().each(function () {
            let item = grid.dataItem(this);
            dati.push(item);
        });
    }

    if (dati.length === 0) {
        kendo.alert(TraduzioneMultiResx(gestioneMagazziniResx, "SelezionareAlmenoUnMovimento", "Non è stato selezionato alcun movimento!"));
        return false;
    }

    let distinctDesc = new Set();

    for (let i = 0; i < dati.length; i++) {
        //controllo se sono dei lav_cod gestiti
        if (parseInt(dati[i].Lav_Cod) !== 1025 && parseInt(dati[i].Lav_Cod) !== 1000) {
            kendo.alert(TraduzioneMultiResx(gestioneMagazziniResx, "TrasferimentoConsentitoPerDttEFatture", "Il Trasferimento è consentito solamente per DDT e Fatture!"));
            return false;
        }

        distinctDesc.add(dati[i].Doc_Numero + " - " + dati[i].Pro_Des);
    }

    //identifico le chiavi selezionati
    $("#dialogTrasferimentoMovContabili").modal("show");
    let s = "";
    for (const aDesc of distinctDesc) {
        s = s + "<span>" + aDesc + "</span><br>";
    }
    $("#nomiMovimentidaspostare").html(s);

    let fabSelezionato = KendoDDL("ddlMagazzini");
    $("#fabbricato_selezionato").html(fabSelezionato.text());

    //ajaxAgronicaSync("GestioneMagazziniBS.aspx/CaricaMagazzini_XTrasferimento",
    //    kendo.stringify({ fabbricato_cod: fabSelezionato.value() }),
    //    false,
    //    function (risposta) {
    //        $(cIdMagazzinoDestinazione + " select").empty();
    //        $(cIdMagazzinoDestinazione + " select").append(risposta.RispostaStringa);
    //        $(".selectpicker").selectpicker("refresh");
    //    }, null);

    //TODO: anziché fare questo genero direttamente la ddl Kendo, con la stessa ultima lettura, tanto dovrebbe usare lo stesso centro
    //oppure tutti i centri

    creaKendoDropDownList("ddlMagazzini_Trasferimento", { read: RiempiElencoMagazziniPerTrasferimento }, "Ubic_Des", "key_Dest");

}


function Gestione_Operazione_Menu(v) {

    SalvaParametri();

    var fabCod = getFabbricatoCod(true);

    if ((fabCod) || (fabCod == 0)) {
        ajaxAgronica("GestioneMagazziniBS.aspx/Gestione_Operazione_Menu",
            JSON.stringify({
                CodiceMenu: v,
                Sa_Cod: getSaCod(true),
                Fabbricato_Cod: fabCod,
                Mat_Cod: 0,
                Pro_Cod: $(cIdPro_Cod).val()
            }),
            function (risposta) {
                WaitFrame.show();
                if (risposta.ParametroDue === false)
                    window.location = risposta.RispostaStringa;
                else {
                    WaitFrame.hide();
                    window.open(risposta.RispostaStringa);
                }
            }, null);
    }
    else {
        kendo.alert(TraduzioneMultiResx(gestioneMagazziniResx, "SelezionareUnMagazzinoDisponibile", "Selezionare un magazzino disponibile"));
        return false;
    }
}

function EliminazioneConfermata(chiave) {

    SalvaParametri();

    ajaxAgronicaSync("GestioneMagazziniBS.aspx/EliminaMovimento",
        JSON.stringify({ chiave: chiave }),
        false,
        function (risposta) {
            popolaGiacenze("tabGiacenze");
            popolaMovimenti("tabMovimenti", null);
            //ScritturaOK(risposta.RispostaStringa);
            MessaggioOK_Kendo(risposta.RispostaStringa);
        },
        function (risposta) {
            //MessaggioErrore(risposta.RispostaStringa);
            MessaggioErrore_Kendo(risposta.RispostaStringa);
        }
    );
}

function AlienazioneConfermata() {
    var dati = [];
    var grid = KendoGrid("tabGiacenze");
    if (grid !== undefined && grid !== null) {
        grid.select().each(function () {
            var item = grid.dataItem(this);
            dati.push(item);
        });
    }

    if (dati.length === 0) {
        kendo.alert(TraduzioneMultiResx(gestioneMagazziniResx, "SelezionareAlmenoUnProdotto", "Non è stato selezionato alcun prodotto!"));
        return false;
    }
    else {

        var errore = false;

        for (var i = 0; i < dati.length; i++) {

            var parametri = {
                piva: dati[i].piva,
                Sa_Cod: parseInt(dati[i].Sa_Cod),
                Fabbricato_Cod: parseInt(dati[i].Fabbricato_Cod),
                Tipo_Destinazione: parseInt(dati[i].Tipo_Destinazione),
                Elem_Cod: parseInt(dati[i].Cat_Cod),
                Pro_Cod: parseInt(dati[i].Pro_Cod),
                Prodotto_Des: dati[i].Pro_Des,
                Mat_Cod: parseInt(dati[i].Mat_Cod),
                Lotto: dati[i].Lotto_Acc,
                Cal_Cod: dati[i].Cal_Cod,
                Cod_Progetto: dati[i].Cod_Progetto,
                Fase_Cod: dati[i].Fase_Cod,
                Udm_Cod: parseInt(dati[i].Udm_Cod),
                Qta_No_Arrotondamenti: kendo.parseFloat(dati[i].Qta),
                Data: $(cIdDataOperazione).val()
            };

            ajaxAgronicaSync("GestioneMagazziniBS.aspx/AlienaRiga",
                JSON.stringify(parametri),
                true,
                function (risposta) {
                    $.logThis(risposta.RispostaStringa);
                },
                function (risposta) {
                    errore = true;
                    $.logThis(risposta.Errore);
                }
            );

            if (errore === true) {
                break;
            }
        }

        if (errore === false) {
            popolaGiacenze("tabGiacenze");
            popolaMovimenti("tabMovimenti", null);
            MessaggioOK_Kendo(TraduzioneMultiResx(gestioneMagazziniResx, "ProdottiAlienatiCorrettamente", "Prodotti alienati correttamente"));
        } else {
            MessaggioErrore_Kendo(TraduzioneMultiResx(gestioneMagazziniResx, "ErroreAlienazioneInterrotta", "Si è verificato un errore. Alienazione interrotta."));
        }
    }
}

//function DoPostBack_ControlliSiNo(str) {

//    if (str.startsWith("del_elem")) {

//        //proseguo con la cancellazione
//        var chiave = str.split("|")[1];

//        SalvaParametriDiv("#frmInputRicerca", false);

//        ajaxAgronicaSync("GestioneMagazziniBS.aspx/EliminaMovimento",
//            JSON.stringify({ chiave: chiave }),
//            false,
//            function (risposta) {
//                popolaGiacenze("tabGiacenze");
//                popolaMovimenti("tabMovimenti", null);
//                ScritturaOK(risposta.RispostaStringa);
//            },
//            function (risposta) {
//                MessaggioErrore(risposta.RispostaStringa);
//            }
//        );

//    } else if (str.startsWith("aliena_giacenze")) {

//        // procedo con l'alienazione
    
//        var dati = [];  
//        var grid = KendoGrid("tabGiacenze");
//        if (grid !== undefined && grid !== null) {
//            grid.select().each(function () {
//                var item = grid.dataItem(this);
//                dati.push(item);
//            });
//        }

//        if (dati.length === 0) {
//            alert(TraduzioneMultiResx(gestioneMagazziniResx, "SelezionareAlmenoUnProdotto", "Non è stato selezionato alcun prodotto!")); 
//            return false;
//        }
//        else {

//            var errore = false;

//            for (var i = 0; i < dati.length; i++) {

//                var parametri = {
//                    piva: dati[i].piva,
//                    Sa_Cod: dati[i].Sa_Cod,
//                    Fabbricato_Cod: dati[i].Fabbricato_Cod,
//                    Elem_Cod: dati[i].Cat_Cod,
//                    Pro_Cod: dati[i].Pro_Cod,
//                    Prodotto_Des: dati[i].Pro_Des,
//                    Mat_Cod: dati[i].Mat_Cod,
//                    Lotto: dati[i].Lotto_Acc,
//                    Cal_Cod: dati[i].Cal_Cod,
//                    Cod_Progetto: dati[i].Cod_Progetto,
//                    Fase_Cod: dati[i].Fase_Cod,
//                    Udm_Cod: dati[i].Udm_Cod,
//                    Qta_No_Arrotondamenti: dati[i].Qta,
//                    Data: $(cIdDataOperazione).val()
//                };

//                ajaxAgronicaSync("GestioneMagazziniBS.aspx/AlienaRiga",
//                    JSON.stringify(parametri),
//                    true,
//                    function (risposta) {
//                        $.logThis(risposta.RispostaStringa);
//                    },
//                    function (risposta) {
//                        errore = true;
//                        $.logThis(risposta.Errore);
//                    }
//                );

//                if (errore === true) {
//                    break;
//                }
//            }

//            if (errore === false) {
//                popolaGiacenze("tabGiacenze");
//                popolaMovimenti("tabMovimenti", null);
//                ScritturaOK(TraduzioneMultiResx(gestioneMagazziniResx, "ProdottiAlienatiCorrettamente", "Prodotti alienati correttamente"));
//            } else {
//                MessaggioErrore(TraduzioneMultiResx(gestioneMagazziniResx, "ErroreAlienazioneInterrotta", "Si è verificato un errore. Alienazione interrotta."));
//            }
//        }

//    }
//}

function Gestione_Alienazione_Giacenze() {

    var dati = [];
    var grid = KendoGrid("tabGiacenze");
    if (grid !== undefined && grid !== null) {
        grid.select().each(function () {
            var item = grid.dataItem(this);
            dati.push(item);
        });
    }

    if (dati.length === 0) {
        kendo.alert(TraduzioneMultiResx(gestioneMagazziniResx, "SelezionareAlmenoUnProdotto", "Non è stato selezionato alcun prodotto!"));
        return false;
    }

    SalvaParametri();

    var data = $(cIdDataOperazione).val();

    kendo.confirm(kendo.format(
            TraduzioneMultiResx(gestioneMagazziniResx, "ConfermaAlienazioneGiacenzeProdotti", 'Sei sicuro di voler alienare le giacenze dei prodotti selezionati? Verrà creato un movimento di scarico alla data {0} pari al quantitativo in giacenza.'),
            data
        ))
        .done(function (data) {
            console.log("Conferma Alienazione");
            AlienazioneConfermata();
        })
        .fail(function () {
            console.log("Annulla alienazione");
        });
}

function LeggiCentri(options) {
    try {
        let tipoValue = 2; // Tipo_Value = 2 ---> ex CaricaCombo_CentriAziendali2

        let resp = RicercaCentriAziendali($(cIdPiva).val(), true, tipoValue, true, true,
            TraduzioneMultiResx(gestioneMagazziniResx, "TuttiICentriAziendali", "Tutti i Centri Aziendali").toUpperCase(),
            0);
        options.success(resp);

    } catch (err) {
        let msgErr = "Errore in lettura centri: " + err;
        console.error(err);
        alert(msgErr);
        options.error(msgErr);
    }
}

function LeggiCategorie(options) {
    try {
        let resp = Ricerca_Categorie_Magazzino(true,
            TraduzioneMultiResx(gestioneMagazziniResx, "TutteLeCategorie", "Tutte le categorie").toUpperCase(),
            0);

        //devo eliminare alcune categorie non gestite
        let cat = resp.filter(function (dataItem) { return ![1, 4, 195, 300, 500, 501].includes(dataItem.Elem_Cod); });

        options.success(cat);
    } catch (err) {
        let msgErr = "Errore in lettura categorie: " + err;
        console.error(err);
        alert(msgErr);
        options.error(msgErr);
    }
}
