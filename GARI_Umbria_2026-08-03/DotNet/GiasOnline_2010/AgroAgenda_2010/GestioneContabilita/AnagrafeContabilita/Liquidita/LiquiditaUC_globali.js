var elencoIstitutiCredito = null;
var elencoRisorseCausale = null;
var chkYesNo = null;

function CaricaElenchiLiquidita() {
    elencoRisorseCausale = [
        { risorsa_cod: "0", risorsa_des: '' },
        { risorsa_cod: "1", risorsa_des: TraduzioneMultiResx(resxObj, "RisorsaFinanziaria", 'Risorsa Finanziaria') }
    ];

    chkYesNo = [
        { Default_Cod: 0, Default_Des: TraduzioneMultiResx(resxObj, "No", 'No') },
        { Default_Cod: 1, Default_Des: TraduzioneMultiResx(resxObj, "Si", 'Si') }
    ];
}
