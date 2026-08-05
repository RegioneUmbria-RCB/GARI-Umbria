var elencoRegimiFiscale = null;
var elencoEsigibilitaIva = null;
var chkYesNo = null;

function CaricaElenchiSezionali() {
    elencoEsigibilitaIva = [
        { EsigibilitaIva_cod: 0, EsigibilitaIva_des: TraduzioneMultiResx(resxObj, "NonSpecificata", 'Non Specificata') },
        { EsigibilitaIva_cod: 1, EsigibilitaIva_des: TraduzioneMultiResx(resxObj, "Immediata", 'Immediata') },
        { EsigibilitaIva_cod: 2, EsigibilitaIva_des: TraduzioneMultiResx(resxObj, "Differita", 'Differita') },
        { EsigibilitaIva_cod: 3, EsigibilitaIva_des: TraduzioneMultiResx(resxObj, "ScissionePagamenti", 'ScissionePagamenti') },
        { EsigibilitaIva_cod: 4, EsigibilitaIva_des: TraduzioneMultiResx(resxObj, "InversioneContabile", 'InversioneContabile') }
    ];

    chkYesNo = [
        { FatturazioneElettronica_Cod: 0, FatturazioneElettronica_Des: TraduzioneMultiResx(resxObj, "No", 'No') },
        { FatturazioneElettronica_Cod: 1, FatturazioneElettronica_Des: TraduzioneMultiResx(resxObj, "Si", 'Si') }
    ];
}