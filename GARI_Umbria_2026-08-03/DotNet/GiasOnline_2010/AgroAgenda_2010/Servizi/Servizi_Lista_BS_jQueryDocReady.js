var objLista_Pratiche;
var objServizi;

var Cmb_Imprese;
var CmbImprese_New;
var CmbServizi_New;
var Data_Inizio_New;
var Data_Fine_New;
var Cmb_Data_Apertura;
var kendoDialogNuoviServizi;
var kendoDialogPassaggioStato;
var kendoDialogCambiaServizio;
var CmbProcedura;
var Cmb_Stato_Destinazione;
var Txt_Data_Riferimento;
var CmbCambiaServizio;

var DSS_cmb_pacchettiAcquistati;
var Txt_Data_Scadenza;

var objLista_DSS_Selezionati;

var UtenteAbilitato_Provisioning_R;
var UtenteAbilitato_W;

var divKendo_Pratiche = "kendoPratiche";
var divKendoDSS = "kendoDSS";

var UtenteAbilitatoZespri;
var UtenteAbilitatoRegione;
var UtenteAbilitatoBloccaSblocca;
var visualizzaKPIN_Block_Name;
var UtenteAbilitatoFiltroAvanzatoPratiche;
var UtenteAbilitatoUndoPratiche;

var dataSwitch;
var TxtValiditaPratica;

var praticheSwitch;
var kendoFiltroPratiche;

var serviziSwitch;
var ddlServizi;
var isSuperuser;
var piva;
var primo_ddlAzienda_OnDataBound = 0;



jQuery(document).ready(function () {

    docReady();

});


async function docReady() {
    WaitFrame.show();

    piva = $("#" + id_HD_Piva).val()

    $("#btn_CercaPratiche").click(function () {
        cercaPratiche();
    });

    $("#btn_AggiungiDSS").click(function () {
        AggiungiDSS();
    });


    let resp = await Permesso_Elenco_Pratiche();

    UtenteAbilitato_Provisioning_R = await LeggiPermessoUtenteP($('#' + id_HD_Username).val(), 362, 0);

    UtenteAbilitato_W = await LeggiPermessoUtenteP($('#' + id_HD_Username).val(), 556, 2);


    UtenteAbilitatoZespri = await LeggiPermessoUtenteP($('#' + id_HD_Username).val(), 250, 2);

    UtenteAbilitatoRegione = await LeggiPermessoUtenteP($('#' + id_HD_Username).val(), 290, 2);

    UtenteAbilitatoBloccaSblocca = await LeggiPermessoUtenteP($('#' + id_HD_Username).val(), 388, 2);

    UtenteAbilitatoFiltroAvanzatoPratiche = await LeggiPermessoUtenteP($('#' + id_HD_Username).val(), 404, 2);

    UtenteAbilitatoUndoPratiche = await LeggiPermessoUtenteP($('#' + id_HD_Username).val(), 450, 2);;

    isSuperuser = await ws_isSuperUSer();

    visualizzaKPIN_Block_Name = false;

    var impostazione193 = await LeggiImpostazione_Superuser(193);
    if (impostazione193 == "1") {
        visualizzaKPIN_Block_Name = true;
    }

    if (UtenteAbilitatoFiltroAvanzatoPratiche) {
        $("#FiltroAvanzatoPratiche").show();
    }

    objServizi = JSON.parse(resp.RispostaStringa);

    TxtValiditaPratica = $("#TxtValiditaPratica").kendoDatePicker({
        value: kendo.toString(new Date(), "dd/MM/yyyy"),
        dateInput: true
    }).data("kendoDatePicker");

    dataSwitch = $("#data-switch").kendoSwitch({
        messages: {
            checked: "SI",
            unchecked: "NO"
        }
    }).data("kendoSwitch");

    praticheSwitch = $("#pratiche-switch").kendoSwitch({
        messages: {
            checked: "SI",
            unchecked: "NO"
        }
    }).data("kendoSwitch");

    serviziSwitch = $("#servizi-switch").kendoSwitch({
        messages: {
            checked: "SI",
            unchecked: "NO"
        }
    }).data("kendoSwitch");

    kendoFiltroPratiche = CreaKendoFiltroPratiche("kendoFiltroPratiche");

    ddlServizi = $("#ddlServizi").kendoMultiSelect({
        dataTextField: "Servizio_Des",
        dataValueField: "Servizio_Cod",
        dataSource: objServizi,
        filter: "contains"
    }).data("kendoMultiSelect");

    WaitFrame.show();


    if (piva !== "") {
        //Cmb_Imprese.value(piva);
        objLista_Pratiche = await ws_carica_pratiche(piva);
        CreaKendoPratiche();
    } else {
        //objLista_Pratiche = await ws_carica_pratiche("-1");
        //CreaKendoPratiche();
    }

    WaitFrame.hide();

    Cmb_Imprese = await Carica_Cmb_Imprese();

    if (piva === "") {
        Cmb_Imprese.value("-1");
    }

    if (piva !== "") {
        Cmb_Imprese.value(piva);
    }

    window.addEventListener('message', event => {

        let kWin = $('#GestionePassaggioDiStatoWindow').data("kendoWindow");
        if (kWin) {
            let urlKWin = kWin.options.content.url;

            if (verificaOriginSecondaria(window, urlKWin, event) && (typeof event.data == "string") && event.data.includes("RispostaStringa")) {
                let respMsg = JSON.parse(event.data);
                switch (respMsg.Tipo) {
                    case 'Profilazione_PassaggioStato':
                        chiudiWindowPassaggioDiStato(respMsg.RispostaStringa);
                        break;
                    default:
                        console.log("Evento non gestito")
                }
            }
        }
        //alert("message")
    });

    WaitFrame.hide();
}