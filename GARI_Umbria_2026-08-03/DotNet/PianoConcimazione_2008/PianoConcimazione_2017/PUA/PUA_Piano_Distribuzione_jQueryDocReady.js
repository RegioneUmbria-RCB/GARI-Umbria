
var gridPD;
var necessarioRicalcolo = false;
var modalita;
var pua_bloccato = false;
var pua_tipo = 1;

var lblMAS = 'MAS';

//visto che molte ddl sono influenzate solo dal regolamento, e qui dentro non lo posso cambiare,
//è inutile che tutte le le volte rifaccia la chiamata ws

var cmbStatoImpiantoNome = "cmbStatoImpianto";
var ddlStatoImpianto = [];

var cmbAnalisiTerrenoNome = "cmbAnalisiTerreno";
var ddlAnalisiTerreno = [];

var cmbFinalitaRERNome = "cmbFinalitaRer";
var ddlFinalitaRER = [];

var cmbPrecessioneNome = "cmbPrecessione";
var ddlPrecessione = [];

var cmbCicloNome = "cmbCiclo";
var ddlCiclo = [];

var cmbUbicazioneNome = "cmbUbicazione";
var ddlUbicazione = [];

var cmbTipoAcquaNome = "cmbTipoAcqua";
var ddlTipoAcqua = [];

var Cmb_Mod_Ciclo;
var Cmb_Mod_Precessione;
var Cmb_Mod_Ubicazione;
var Cmb_Mod_TipoAcqua;

var Cmb_Mod_FinalitaRer;
var Txt_Mod_BPerc;
var Txt_Mod_Resa;

var Txt_Mod_Mas;
var Txt_Mod_ResaRif;
var Txt_Mod_FattoreCorrettivo;

var Window_Modifica;
var selected_add;
var obj_ModificaMultipla;

//DOCUMENT READY
$(document).ready(function () {

    $.logThis("DocReady: INIZIO");

    modalita = parseInt($(cIdModalita).val());

    pua_bloccato = parseInt($(cIdBloccoFlag).val());

    pua_tipo = parseInt($(cIdPuaTipo).val());

    regCod = parseInt($(cIdRegCod).val());

    $(cIdAssegnaDefault).val('');

    if (regCod === 78) {
        lblMAS = 'CBPA';
    }

    popolaGrigliaPianoDistribuzione("gridPD",false);

    if (regCod === 78) {
        $("#DivMAS").html(lblMAS);
    }

    $("#Btn_ApriBrogliaccio").click(function () {
        MostraBrogliaccio();
    });

    $("#Btn_ApriQdC").click(function () {
        MostraQdC();
    });

    $("#Btn_ApriAnalisi").click(function () {
        MostraAnalisi();
    });

    $("#BtnVaiBilancio").hide();
    $("#BtnVaiPianoDistibuzione").show();
    

    if (modalita === Enum_Modalita.PianoDistribuzione) {

        $("#BtnVaiBilancio").show();
        $("#BtnVaiPianoDistibuzione").hide();

        $("#Btn_ModificaMultipla").click(function () {
            $("#ModificaMultipla").show();
            modificaMultipla();
        });

        $("#Btn_AssociaNMas").click(function () {
            associaNMas();
        });

        $("#Btn_AssociaN").click(function () {
            associaN();
        });

        $("#Btn_AssegnaDefault").click(function () {
            $(cIdAssegnaDefault).val('true');
            popolaGrigliaPianoDistribuzione("gridPD");
        });

        riempiDdlGeneriche();

        Window_Modifica = $("#ModificaMultipla").kendoDialog({
            width: "350px",
            //height: "88%",
            modal: true,
            title: "Modifica appezzamenti selezionati",
            visible: false,
            open: async function (e) {
                this.center();
                await openModificaMultipla();
            },
            actions: [
                { text: "Applica Modifiche", primary: true, action: applicaModifiche },
                { text: "Annulla tutto" }
            ]
            //close: DialogDatiAziendaCloseEvent
        }).data("kendoDialog");

    }

    $.logThis("DocReady: FINE");

});
