
Public Class G2G_Configurazione_FiltriReq_Agenda

    Public listaLavCod As List(Of Integer)

    Public listaCauMovEsclusi As List(Of String)

End Class

Public Class G2G_Configurazione_FiltriReq_Pratiche

    Public listaServiziWWorkflow As List(Of G2G_Configurazione_FiltriReq_Pratiche_Servizi)
    Public BloccaOperazioni As Blocca_Operazioni_Pratiche

End Class

Public Class Blocca_Operazioni_Pratiche
    Public BloccaOperazioni As Boolean
    Public Blocco_Flag As Integer
End Class

Public Class G2G_Configurazione_FiltriReq_Pratiche_Servizi

    Public Property Servizio_Cod As Integer
    Public Property PassaggiDiStatoEsclusi As List(Of G2G_Configurazione_FiltriReq_Pratiche_PassaggiStato)

End Class

Public Class G2G_Configurazione_FiltriReq_Pratiche_PassaggiStato

    Public Property Stato_Iniziale As Integer
    Public Property Stato_Finale As Integer

End Class

Public Class G2G_Configurazione_FiltriReq_Allegati

    Public listaAllegati_Documenti_CatCod As List(Of Integer)

    Public listaAlert_TipoEntita_Cod As List(Of Integer)

End Class

Public Class G2G_Configurazione_FiltriReq_Analisi

    Public listaAnalisi_Testata_Tipo As List(Of Integer)

End Class

Public Class G2G_Configurazione_FiltriReq_PianiConcimazione

    Public listaPianoConcimazione_Tipo As List(Of Integer)

End Class

Public Class G2G_Configurazione_FiltriReq_Ricette

    Public listaRicette_Tipo As List(Of Integer)

End Class

Public Class G2G_Configurazione_FiltriReq_Contatti

    Public listaCodRapporto As List(Of Integer)

    Public listaImprese As List(Of String)

    Public dataValidita As Date

End Class

Public Class G2G_Configurazione_FiltriReq_Macchine

    Public listaImprese As List(Of String)

    Public dataValidita As Date

End Class

Public Class G2G_Configurazione_FiltriReq_Materie_Prime_Campionature

    Public listaImprese As List(Of String)
    Public dataValidita As Date

End Class

Public Class G2G_Configurazione_FiltriReq_Analisi_Condivise

    Public listaImprese As List(Of String)
    Public dataValidita As Date

End Class

Public Class G2G_Configurazione_FiltriReq_Attivita

    Public listaImprese As List(Of String)
    Public dataValidita_Inizio As Date
    Public dataValidita_Fine As Date
    Public ListaLav_Cod As List(Of Integer)

End Class


Public Class G2G_Configurazione_FiltriReq_Materie_Prime

    Public listaImprese As List(Of String)
    Public dataValidita As Date

End Class