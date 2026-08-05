
Imports AgronicaCoreModelloSTD

Public Class CoreWS_DocumentiScrivi_APP
    Inherits APICallsBasic

    Public Documento As DocumentoPerScarico

    Public Sub New(ByVal objP_super_server As String, ByVal objP_server As String, ByVal objP_utenti As String, ByVal Documento As DocumentoPerScarico)
        MyBase.New(objP_super_server, objP_server, objP_utenti)

        Me.Documento = Documento

    End Sub

End Class
