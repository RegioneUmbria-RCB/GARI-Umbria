//DOCUMENT READY
$(document).ready(function () {

    $.logThis("DocReady: INIZIO");
    //inizializzazione della pagina la prima volta che viene caricata
    $(".preArea").show();
    // creare la dropdownlist 
    creaKendoDropDownList("cmbAzienda", { read: RiempicmbAzienda },"rag_soc", "piva");
    creaKendoDropDownList("cmbMotivoRichiesta", { read: RiempicmbMotivoRichiesta }, "TipoMotivo_Des", "TipoMotivo_Cod");
    creaKendoDropDownList("cmbRichiestaBancheDati", { read: Riempicmb_RichiestaInsBancaDati }, "RichiestaInsBancaDati_Des", "RichiestaInsBancaDati_Cod");

    //Nascondo la text e la lable lbl_text_nbc
    $('input[name$="text_nbc"]').hide();
    $('label[id$="lbl_text_nbc"]').hide();

    var ddlAzienda = $("#cmbAzienda").data("kendoDropDownList");
    ddlAzienda.bind("change", Azienda_Change);

    var ddlMotivoRichiesta = $("#cmbMotivoRichiesta").data("kendoDropDownList");
    ddlMotivoRichiesta.bind("change", motivo_Richiesta_Change);

    var ddlRichiestaBancheDati = $("#cmbRichiestaBancheDati").data("kendoDropDownList");
    ddlRichiestaBancheDati.bind("change", bancheDati_Change);
    
    //eventi di click pulsanti
    $("#btn_invia_email").click(function () {
        Invia_Email();
    });
    
    $.logThis("DocReady: FINE");

});
