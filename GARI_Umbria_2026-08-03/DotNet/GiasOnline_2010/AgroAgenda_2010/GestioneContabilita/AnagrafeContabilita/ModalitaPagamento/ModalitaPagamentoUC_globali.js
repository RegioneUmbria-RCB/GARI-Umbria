var elencoModalitaPagamento = null;
var elencoTemplateModalitaPagamento = null;
var elencoOpzioni = null;
var elencoRisorseCausale = null;
var elencoTipologiaCausale = null;

function CaricaElenchiModalitaPagamento() {
    elencoOpzioni = [
        { opzione_cod: 0, opzione_des: '' },
        { opzione_cod: 1, opzione_des: TraduzioneMultiResx(resxObj, "FineMese", 'Fine Mese') },
        { opzione_cod: 2, opzione_des: TraduzioneMultiResx(resxObj, "DataFatturaFineMese", 'Data Fattura Fine Mese') }
    ];

    elencoRisorseCausale = [
        { risorsa_cod: "0", risorsa_des: '' },
        { risorsa_cod: "1", risorsa_des: TraduzioneMultiResx(resxObj, "RisorsaFinanziaria", 'Risorsa Finanziaria') },
        { risorsa_cod: "2", risorsa_des: TraduzioneMultiResx(resxObj, "LiquiditàImmediata", 'Liquidità Immediata') }
    ]

    elencoTipologiaCausale = [
        {
            tipologia_causale_cod: 0,
            tipologia_causale_des: TraduzioneMultiResx(resxObj, "NonImpostata", 'Non Impostata'),
            risorsa_cod: [0]
        },
        {
            tipologia_causale_cod: 1,
            tipologia_causale_des: TraduzioneMultiResx(resxObj, "RIBA", 'RIBA'),
            risorsa_cod: [1]
        },
        {
            tipologia_causale_cod: 2,
            tipologia_causale_des: TraduzioneMultiResx(resxObj, "Bonifico", 'Bonifico'),
            risorsa_cod: [1]
        },
        {
            tipologia_causale_cod: 3,
            tipologia_causale_des: TraduzioneMultiResx(resxObj, "Contanti", 'Contanti'),
            risorsa_cod: [2]
        },
        {
            tipologia_causale_cod: 4,
            tipologia_causale_des: TraduzioneMultiResx(resxObj, "RimessaDiretta", 'Rimessa Diretta'),
            risorsa_cod: [1]
        },
        {
            tipologia_causale_cod: 5,
            tipologia_causale_des: TraduzioneMultiResx(resxObj, "Assegno", 'Assegno'),
            risorsa_cod: [1]
        },
        {
            tipologia_causale_cod: 6,
            tipologia_causale_des: TraduzioneMultiResx(resxObj, "CartaCredito", 'Carta Credito'),
            risorsa_cod: [1]
        },
        {
            tipologia_causale_cod: 7,
            tipologia_causale_des: TraduzioneMultiResx(resxObj, "Bancomat", 'Bancomat'),
            risorsa_cod: [2]
        },
        {
            tipologia_causale_cod: 8,
            tipologia_causale_des: TraduzioneMultiResx(resxObj, "RID", 'RID'),
            risorsa_cod: [1]
        },
        {
            tipologia_causale_cod: 9,
            tipologia_causale_des: TraduzioneMultiResx(resxObj, "MAV", 'MAV'),
            risorsa_cod: [1]
        },
        {
            tipologia_causale_cod: 10,
            tipologia_causale_des: TraduzioneMultiResx(resxObj, "Contrassegno", 'Contrassegno'),
            risorsa_cod: [1, 2]
        }
    ];
}