
//
// ReportPercorsi.js
//



var waTablePercorsi;


function ElaboraReport() {
    
    ReportGPF();

}

function ElaboraPosizioneAttuale() {
    ElaboraPosizioneAttualeGPF();
}

function ElaboraPosizioniRilevate() {
    ReportPosizioniRilevate();
}

function ElaboraRaccolte() {

    ReportPercorsi();

}

function Esporta(waTableGetData) {
    WaitFrame.show();

    var dati = waTablePercorsi.getData(false, true);
    var ssxml = EsportaLatoClient(dati)

    WaitFrame.hide();
    return ssxml;
}
