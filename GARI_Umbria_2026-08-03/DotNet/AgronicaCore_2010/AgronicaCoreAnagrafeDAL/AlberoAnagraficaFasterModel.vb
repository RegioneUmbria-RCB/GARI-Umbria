Public Class AlberoAnagraficaFasterModel

    Public Class AlberoAnagraficaFasterModelCfg


        Public Enum enum_Contesto
            enum_Contesto_Generico = 0
            enum_Contesto_GIS = 1
        End Enum

        Public Property ApplicaFiltroUtentiVisibilitaAppoggio As Boolean = False
        Public Property Flag_Planning As Boolean = False
        Public Property Flag_Anagrafica As Boolean = False
        Public Property Flag_Contatti As Boolean = False
        Public Property Flag_Analisi As Boolean = False
        Public Property Flag_PianoConcimazione As Boolean = False
        Public Property Flag_Esercizio As Boolean = False
        Public Property Flag_ParcoMacchine As Boolean = False
        Public Property Flag_CatastoAziendale As Boolean = False
        Public Property Flag_CatastoAppezzamento As Boolean = False
        Public Property Flag_Fabbricati As Boolean = False
        Public Property Flag_PortafoglioProdotti As Boolean = False
        Public Property Flag_Singola_Selezione As Boolean = True
        Public Property Flag_Appezzamenti_Filtra_Tecnico As Boolean = True
        Public Property Flag_Agenda As Boolean = False
        Public Property FiltroImpiantiIdTestataTemp As Integer = 0
        Public Property ordinaDataUltimoImpianto As Boolean = False
        Public Property visualizzaRiferimentoAlfanumericoImpianto As Boolean = False
        Public Property TipoOperazioneColturale As String
        Public Property Piva As String
        Public Property Sa_Cod As String
        Public Property Veg_Cod As Integer = 0
        Public Property Cul_Cod As Integer
        Public Property dataInizio As DateTime
        Public Property dataFine As DateTime
        Public Property DatiSportelloSementieri As String
        Public Property ParametriAgendaData As DateTime
        Public Property Elenco_Icone_SpecieVegetali As String
        Public Property PivaPadre As String = ""
        Public Property Valore_Albero As String
        Public Property FlagModalitaSementieri As Boolean = False
        Public Property GruppoOperazioneColturale As String
        Public Property Flag_Ricette As Boolean = False
        Public Property TipologiaLayer_Cod As Integer = 1
        Public Property Contesto As enum_Contesto = enum_Contesto.enum_Contesto_Generico
        Public Property Codice_Fiscale_Tecnico As String
    End Class

End Class
