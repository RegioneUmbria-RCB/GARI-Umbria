namespace AgronicaNetCore.APP.BIZ.Services.LetturaDatiTabellaComune.Enums
{
    public enum TabellaComune
    {
        // SQL-based
        Specie,
        Varieta,
        Finalita,
        Lavorazioni,
        OperazioniCombinazioni,
        //OperazioniCausali, //non sono dati comuni, tabella di db_Server
        Avversita,
        GruppiAvversita,
        GruppiAvversitaAttive,
        InfestantiAttive,
        AvversitaSpecie,
        CategorieUnitaMisura,
        TipiMacchine,
        Nazioni,
        Regioni,
        Province,
        Comuni,
        SpecieZootecniche,
        DestinazioneUso,

        // WS-based (rilievi)
        MisureAvversita,
        MisureAvversitaPersonalizzate,
        MisureDanni,
        MisureDanniPersonalizzate,
        IndiciMaturita,
        IndiciMaturitaSpecieVegetali,
        MisureIndiciMaturita,
        IndiciMaturitaPersonalizzate,
        IndiciMaturitaSpecieVegetaliPersonalizzate,
        MisureIndiciMaturitaPersonalizzate,
        SpecieVegetaliStadiCrescita,
        SpecieVegetaliStadiCrescitaPersonalizzati,

        // SQL-based (impianti)
        ImpiantiIrrigazioni,
        RapportiContabili,

        // WS-based (disciplinari)
        Disciplinari 
    }
}
