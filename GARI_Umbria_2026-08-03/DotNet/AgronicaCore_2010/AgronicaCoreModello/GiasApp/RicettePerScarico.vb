
Imports AgronicaCoreEntityFramework_POCO

Public Class RicettePerScarico

    Public Property Ricette As List(Of APP_Ricette)

    Public Property RicetteOperazioni As List(Of APP_Ricette_Operazioni)

    Public Property RicetteDettagli As List(Of APP_Ricette_Dettagli)

    Public Property RicetteDettaglioTecnico As List(Of APP_Ricette_Dettaglio_Tecnico)

    Public Property RicetteXNote As List(Of APP_RicettexNote)

    Public Property RicetteDestinazioni As List(Of APP_Ricette_Destinazioni)

    ' Attività scarico tempi

    Public Property Attivita As List(Of APP_CDG_Generale)

    Public Property AttivitaMovimenti As List(Of APP_CDG_Movimenti)

    Public Property AttivitaOperazioni As List(Of APP_Riferimenti_Interventi_Cdg)
    Public Property Documenti As List(Of DocumentoPerScarico)
    Public Property guid As String
    Public Property versione As String
    Public Property cancellato As Boolean

    Public Property posizione As String
    Public Property riferimentoPianificata As String
    Public Property isPianificata As Boolean

End Class
