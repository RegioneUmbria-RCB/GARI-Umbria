var Piva;
var Cod_Contatto;
var Analisi_Testata_Cod;
var Area = 0;
var Tipologia = 0;
var Cmb_TipoDocumento;
var gridAllegati;

//DOCUMENT READY
$(document).ready(function () {

    Piva = obj_Allegati.Piva;
    Cod_Contatto = obj_Allegati.Cod_Contatto;
    Analisi_Testata_Cod = obj_Allegati.Analisi_Testata_Cod;
    Area = obj_Allegati.Area;
    Tipologia = obj_Allegati.Tipologia;
    if (kendoServer) $("#kendoAllegati").val(obj_Allegati.dati_Allegati);
    inizializzaKendoAllegati('gridAllegati');

    if (operazione != 0) {

        if (window.File && window.FileReader && window.FileList && window.Blob) {
            document.getElementById('File_Allegato').addEventListener('change', handleFileSelect, false);
        } else {
            alert('Il tuo browser non supporta alcune API necessarie al caricamento degli allegati.');
        }

        $("#Txt_Num_Documento").val(obj_Allegati.Num_Documento);
        $("#Txt_Ente_Rilascio").val(obj_Allegati.Ente_Rilascio);
        $("#Txt_Data_Rilascio").kendoDatePicker({
            max: new Date(2100, 11, 31),
            value: obj_Allegati.Data_Rilascio
        });
        $("#Txt_Data_Scadenza").kendoDatePicker({
            max: new Date(2100, 11, 31),
            value: obj_Allegati.Data_Scadenza
        });

        inizializzaCmb_TipoDocumento();

        $("#btn_salva_allegato").click(function () {
            if (fromPatentiniContatto)
                SalvaPatentino(true)
            else
                SalvaAllegato(true);
        });

        $("#btn_modifica_allegato").click(function () {
            if (fromPatentiniContatto)
                SalvaPatentino(false)
            else
                SalvaAllegato(false);
        });

        $("#btn_annulla_allegato").click(function () {
            location.reload();
        });

    }

});


// IL BOTTONE FINTO MI SIMULA IL CLICK DEL PULSANTE VERO
function openFileDialogFinto() {
    $("#File_Allegato").click();
}

// SCRIVO IL PERCORSO DEL FILE SULLA TEXTBOX
function scriviPercorsoFileSuTxt() {
    var nomeFile = $("#File_Allegato").val().replace("C:\\fakepath\\", "");
    $('#Txt_Documento_Allegato').val(nomeFile);

}

var handleFileSelect = function (evt) {
    var file = evt.target.files[0];
    if (file) {
        var reader = new FileReader();
        reader.onload = function (readerEvt) {
            var binaryString = readerEvt.target.result;
            $('#File_Caricato').val(btoa(binaryString));
        };
        reader.readAsBinaryString(file);


    }
};