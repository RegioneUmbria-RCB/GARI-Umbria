Imports AgronicaCoreRegVinoDAL

Public Class Vigne
    Inherits TeleregistriManager

    Dim reader As xDBVigneSiRPV_R
    Dim writer As xDBVigneSiRPV_W

    Public Sub New(user As String, pwd As String, urlS As String, urlA As String, server As AgronicaCoreDataProvider.AgronicaCoreParametri, utenti As AgronicaCoreDataProvider.AgronicaCoreParametri, _Configurazione_Servizio As AgronicaCoreVarieDAL.Configurazione_Servizio, CertificateFile As String)
        MyBase.New(user, pwd, urlS, urlA, server, utenti, _Configurazione_Servizio, CertificateFile)
    End Sub

    Public Overrides Sub inizializzaReaderWriter()
        reader = New xDBVigneSiRPV_R
        writer = New xDBVigneSiRPV_W
    End Sub

    Public Overrides Sub checkResult()

    End Sub

    Public Overrides Function eliminaTuttoInput() As Boolean

    End Function

    Public Overrides Function eliminaTuttoOutput() As Boolean

    End Function

    Public Overrides Function inserisciAggiornaTuttoInput(tipoRichiesta As Integer) As Boolean

    End Function

    Public Overrides Function inserisciAggiornaTuttoOutput() As Boolean

    End Function
End Class
