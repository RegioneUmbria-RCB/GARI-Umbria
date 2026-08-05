Imports AgronicaCoreDataProviderSTD.TipiEnumerativi
Imports AgronicaCoreModelsSTD.anagrafiche

Public Class CoreWS_Scrivi_Macchina
    Inherits APICallsBasic

    Public macchina As ParcoMacchine
    Public tipoOperazione As enum_TipoOperazioneDB

    Public Sub New(ByVal objP_super_server As String, ByVal objP_server As String, ByVal objP_utenti As String, ByVal macchina As ParcoMacchine, ByVal tipoOperazione As enum_TipoOperazioneDB)

        MyBase.New(objP_super_server, objP_server, objP_utenti)

        Me.macchina = macchina
        Me.tipoOperazione = tipoOperazione

    End Sub

End Class
