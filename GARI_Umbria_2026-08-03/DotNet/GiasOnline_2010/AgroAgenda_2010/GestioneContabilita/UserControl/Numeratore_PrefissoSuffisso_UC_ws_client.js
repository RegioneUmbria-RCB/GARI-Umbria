var indirizzohttp = "./Numeratore_Tipo.aspx";

function CaricaNumeratoriPF(options) {

    var param = kendo.stringify({piva: $(cIdPiva).val() });
    ajaxAgronica(indirizzohttp + "/CaricaNumeratoriPS",
        param,
        function (risposta) {
            let risp = JSON.parse(risposta.RispostaStringa);
            options.success(risp);
        }, null);
}

function CaricaDropDownNumeratoriTipo()
{
    if (elencoNumeratoriTipo == null || elencoNumeratoriTipo == undefined)
    {
        var param = kendo.stringify({ piva: $(cIdPiva).val() });
        ajaxAgronica(indirizzohttp + "/CaricaDropDownNumeratoriTipo",
            param,
            function (risposta) {
                let risp = JSON.parse(risposta.RispostaStringa);
                elencoNumeratoriTipo = risp;
            }, null);
    }
}


function SubmitNumeratorePF(options) {


    var grid = $("#tab_numeratore_prefisso_suffisso").data("kendoGrid");
    var currentData = grid.dataSource.data();

    // controllo che tutte le righe CREATE e MODIFICATE siano complete
    var nrErr = controllaRigheNumeratorePS(options.data.created) +
        controllaRigheNumeratorePS(options.data.updated);

    if (nrErr > 0) {
        return;
    }

    // Non ci sono errori, procedo con aggiornamenti
    var updatedRecords = [];
    var newRecords = [];
    var deletedRecords = [];
    var allRecords = [];

    // modificate / inserite
    for (let i = 0; i < currentData.length; i++) {

        if (currentData[i].isNew()) {
            newRecords.push(currentData[i].toJSON());
        } else if (currentData[i].dirty) {
            updatedRecords.push(currentData[i].toJSON());
        }
        allRecords.push(currentData[i].toJSON());
    }

    // cancellate
    for (let i = 0; i < grid.dataSource._destroyed.length; i++) {

        deletedRecords.push(grid.dataSource._destroyed[i].toJSON());
    }

    if (newRecords.length > 0 || updatedRecords.length > 0 || deletedRecords.length > 0) {

        var righeInserite = kendoEscapeOggetto(newRecords);
        var righeModificate = kendoEscapeOggetto(updatedRecords);
        var righeCancellate = kendoEscapeOggetto(deletedRecords);
        var tutteLeRighe = kendoEscapeOggetto(allRecords);

        var objParametri = new Object();
        objParametri.Piva = $(cIdPiva).val();
        objParametri.RigheInserite = righeInserite;
        objParametri.RigheModificate = righeModificate;
        objParametri.RigheCancellate = righeCancellate;
        objParametri.TutteLeRighe = tutteLeRighe;

        var paramEscaped = kendoEscapeOggetto(objParametri);
        var param = "{paramString: '" + paramEscaped + "'}";
        var salvataggioAmmesso = false;

        if (deletedRecords.length > 0) {
            ajaxAgronicaSync(indirizzohttp + "/CheckPreSalva_NumeratoriPS",
                param, false,
                function (risposta) {

                    if (risposta.RispostaStringa == "") {
                        SalvaNumeratoriPF(param);
                    } else {

                        var kendoConfirm = $("<div></div>").kendoConfirm({
                            title: "Attenzione",
                            messages: { okText: "Sì", cancel: "No" },
                            content: risposta.RispostaStringa
                        }).data("kendoConfirm");
                        kendoConfirm.result.done(function() {
                            SalvaNumeratoriPF(param);
                        });

                        kendoConfirm.result.fail(function() {
                            erroreSubmitGriglia(grid);
                        });

                        kendoConfirm.open();
                    }

                }, function (errore) {
                    MessaggioErrore_Bootstrap(errore.RispostaStringa + ' ' + errore.Errore, "DIV_Messaggi");
                }
            );
        }
        else
        {
            SalvaNumeratoriPF(param);
        }
       
    }
}

function SalvaNumeratoriPF(param)
{

    var allOk = false;    
    var grid = $("#tab_numeratore_prefisso_suffisso").data("kendoGrid");

    ajaxAgronicaSync(indirizzohttp + "/AggiornaNumeratoriPS",
        param, false,
        function (risposta) {
            allOk = true;
            grid.dataSource._destroyed = [];
            grid.dataSource.read();
            grid.refresh();
            MessaggioTuttoOK_Bootstrap("Salvataggio effettuato correttamente", "DIV_Messaggi");
        }, function (errore) {
            MessaggioErrore_Bootstrap(errore.RispostaStringa + ' ' + errore.Errore, "DIV_Messaggi");
        }
    );

    if (!allOk) {
        erroreSubmitGriglia(grid);
    }

}

// Controllo campi obbligatori
function controllaRigheNumeratorePS(righe) {

    var nrErr = 0;

    for (let x = 0; x < righe.length; x++) {

        let item = righe[x];

        if (item.Descrizione == "") {
            MessaggioErrore_Bootstrap("La descrizione è obbligatoria", "DIV_Messaggi");
            nrErr++;
            break;
        }            
    
        if (item.Lunghezza_Centro > 0 && item.CarattereFormattazione == "") {
            MessaggioErrore_Bootstrap("Se Lunghezza Centro > 0 è necessario specificare il Carattere Formattazione", "DIV_Messaggi");
            nrErr++;
            break;
        }
    }

    return nrErr;
}