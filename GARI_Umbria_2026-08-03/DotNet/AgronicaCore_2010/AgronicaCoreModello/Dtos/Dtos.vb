Imports AgronicaCoreDataProvider.TipiEnumerativi

Public Class CopiaOperazioniDto
    Public data As String
    Public id_agenda_checked As String
    Public id_agenda As String
    Public lav_cod_checked As String
    Public lav_cod As String
    Public piva As String
    Public sa_cod As String
    Public LAV_COD_COPIABILI As Integer()
End Class

Public Class CopiaOperazioneResult
    Public data As String
    Public Id_Agenda As String
    Public Lav_Cod As String
    Public Piva As String
    Public Sa_Cod As String
    Public RagSoc As String
    Public SaNome As String
    Public QueryStringIdAgendaChecked As String
End Class

Public Class PaginaLinkGestioneMagazziniQueryStringDto
    Public k As String
    Public c As String
    Public mode As String
End Class