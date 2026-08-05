Imports AgronicaCoreDataProvider

Public Class ParametriHeader2022
    Public idSezione As Integer = -1
    Public idSezioneSessione As Integer = 0
    Public richiedeAziendaSelezionata As Integer = 0
    Public funzioneIndietro As String = ""
    Public idIndietro As String = ""
    Public testoBreadcrum As String = ""
    Public objParametri_Super_Server As AgronicaCoreDataProvider.AgronicaCoreParametri
    Public objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri
    Public objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri
    Public sitoOspite As TipiEnumerativi.Enum_SiteRedirector = -1
    Public PATHPATH_GIASBASE As String = ""
    Public pathLogo As String = ""
End Class
