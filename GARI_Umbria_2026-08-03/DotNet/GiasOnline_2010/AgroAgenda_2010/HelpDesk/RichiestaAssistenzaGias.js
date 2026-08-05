
function RiempicmbMotivoRichiesta(options) {
    options.success(Elenco_Motivo_Richiesta);
}

function Riempicmb_RichiestaInsBancaDati(options) {
    options.success(Elenco_RichiestaInsBancaDati);
}

function Azienda_Change(e) {
    var dataItem = e.sender.dataItem();
}

function motivo_Richiesta_Change(e)
{
    var dataItem = e.sender.dataItem();
    var ddlRichiestaBancheDati = $("#cmbRichiestaBancheDati").data("kendoDropDownList");
    if (dataItem.TipoMotivo_Cod == 0) {
        ddlRichiestaBancheDati.wrapper.show();
        $('input[name$="text_nbc"]').hide();
        $('label[id$="lbl_text_nbc"]').hide();
        $('label[id$="lbl_RichiestaBancheDati"]').show();
    }
    else
    {
        ddlRichiestaBancheDati.wrapper.hide();
        $('input[name$="text_nbc"]').show();
        $('label[id$="lbl_text_nbc"]').show();
        $('label[id$="lbl_RichiestaBancheDati"]').hide();
        $('input[name$="text_nbc"]').val("");
    }

}

function bancheDati_Change(e)
{
    var dataItem = e.sender.dataItem();
}

function Invia_Email()
{   var ddlAzienda = $("#cmbAzienda").data("kendoDropDownList");
    var dataItem1 = ddlAzienda.dataItem();
    var ddlTipoMotivo = $("#cmbMotivoRichiesta").data("kendoDropDownList");
    var dataItem2 = ddlTipoMotivo.dataItem();
    var ddlRichiestaBancheDati = $("#cmbRichiestaBancheDati").data("kendoDropDownList");
    var dataItem = ddlRichiestaBancheDati.dataItem()
    var AziendaRichiedente = dataItem1.rag_soc;
    var flag = dataItem.Flag;
    var subject = dataItem.Subject;
    var body = dataItem.Body;
    var address = "";
    var DigitaRichiesta = $('input[name$="text_nbc"]').val();

        //Selettore email
        switch (dataItem2.TipoMotivo_Cod) {
                                    case 0:
                                        //Richiesta assistenza campagna
                                        if (flag == 0) {
                                            address = MailAssistenza;
                                        }
                                        else { address = MailBancheDati; }
                                        subject = dataItem.Subject;
                                        body = dataItem.Body;
                                        break;
                                    case 1:
                                        //Richiesta assistenza campagna
                                        address = MailCampagna;
                                        subject = dataItem2.TipoMotivo_Des;
                                        body = BodNomeRichiedente + DigitaRichiesta;
                                    case 2:
                                        //Richiesta assistenza conferimenti,controllo di gestione , laboratorio analisi e qualità
                                        address = MailConf_Gest_LabAnalisi_Qual;
                                        subject = dataItem2.TipoMotivo_Des;
                                        body = BodNomeRichiedente + DigitaRichiesta;
                                        break;
                                    case 3:
                                        //Richiesta assistenza Cantine e costi
                                        address = MailCantine;
                                        subject = dataItem2.TipoMotivo_Des;
                                        body = BodNomeRichiedente + DigitaRichiesta;
                                        break;
                                    case 4:
                                        //Altro
                                        address = MailAltro;
                                        subject = "Richiesta generica";
                                        body = BodNomeRichiedente + DigitaRichiesta;
                                        break;
                                    default:
                                        break;     
                                }                             
                var url = 'mailto:' + address + '?subject=' + subject + " da parte dell'azienda: " + AziendaRichiedente + '&body=' + body + "%0D%0A %0D%0A %0D%0A";
                window.location.href = url;//spedisco mail precompilata
                $('input[name$="text_nbc"]').val("");
}






