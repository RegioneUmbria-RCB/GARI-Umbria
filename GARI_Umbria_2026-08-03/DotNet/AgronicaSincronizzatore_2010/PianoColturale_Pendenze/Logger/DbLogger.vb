

Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi

Public Class DbLogger
    Implements IDisposable

    Private _logInvioAnagrafeR As AgronicaCoreVarieDAL.Agronica_Log_Invio_Anagrafe_R
    Private _logInvioAnagrafeW As AgronicaCoreVarieDAL.Agronica_Log_Invio_Anagrafe_W
    Private _logInvioChiamateW As AgronicaCoreVarieDAL.Agronica_Log_Invio_Chiamate_W

    Public Sub New()
        _logInvioAnagrafeR = New AgronicaCoreVarieDAL.Agronica_Log_Invio_Anagrafe_R()
        _logInvioAnagrafeW = New AgronicaCoreVarieDAL.Agronica_Log_Invio_Anagrafe_W()
        _logInvioChiamateW = New AgronicaCoreVarieDAL.Agronica_Log_Invio_Chiamate_W()
    End Sub

    Public Sub ScriviLogInvioAnagrafe(ByVal Tipo As String, ByVal Piva As String, ByVal Sa_Cod As Integer, ByVal Appezza As Integer,
                                      ByVal Stato_Anagrafe As Integer, ByVal Note_Anagrafe As String,
                                      ByVal Dati_Inviati_Chiamata As String, ByVal Esito_Chiamata As String, ByVal Dati_Ricevuti_Chiamata As String,
                                      ByRef ObjParametri_Server As AgronicaCoreParametri)

        Dim Tipo_Esportazione As enum_Esportazioni_Sistema_Cod = enum_Esportazioni_Sistema_Cod.Agea_Import_Pendenze
        Dim ID_Log_Invio As Integer

        Dim dtLogAnagrafe = _logInvioAnagrafeR.Leggi(Tipo_Esportazione, 0, 0, "", "", Piva, Sa_Cod, Appezza, 0, 0, ObjParametri_Server)

        If dtLogAnagrafe.Rows.Count > 0 Then

            ID_Log_Invio = dtLogAnagrafe.Rows(0)("ID_Log_Invio")
            _logInvioChiamateW.Modifica(ID_Log_Invio, Tipo_Esportazione, Dati_Inviati_Chiamata, Esito_Chiamata, Dati_Ricevuti_Chiamata, "0", "UPD", ObjParametri_Server)
            _logInvioAnagrafeW.Modifica(Tipo_Esportazione, ID_Log_Invio, Stato_Anagrafe, Tipo, "", Piva, Sa_Cod, Appezza, 0, 0, Note_Anagrafe, Date.Now, "", ObjParametri_Server)

        Else

            ID_Log_Invio = _logInvioChiamateW.Scrivi(Tipo_Esportazione, Dati_Inviati_Chiamata, Nothing, Esito_Chiamata, Dati_Ricevuti_Chiamata, "0", "UPD", ObjParametri_Server)
            _logInvioAnagrafeW.Scrivi(Tipo_Esportazione, ID_Log_Invio, Stato_Anagrafe, Tipo, "", Piva, Sa_Cod, Appezza, 0, 0, Note_Anagrafe, DateTime.Now, ObjParametri_Server)

        End If

    End Sub

    Public Sub CancellaLogInvioAnagrafe(ByVal Piva As String, ByVal Sa_Cod As Integer, ByVal Appezza As Integer,
                                        ByRef ObjParametri_Server As AgronicaCoreParametri)

        Dim Tipo_Esportazione As enum_Esportazioni_Sistema_Cod = enum_Esportazioni_Sistema_Cod.Agea_Import_Pendenze
        Dim ID_Log_Invio As Integer

        Dim dtLogAnagrafe = _logInvioAnagrafeR.Leggi(Tipo_Esportazione, 0, 0, "", "", Piva, Sa_Cod, Appezza, 0, 0, ObjParametri_Server)

        If dtLogAnagrafe.Rows.Count > 0 Then

            ID_Log_Invio = dtLogAnagrafe.Rows(0)("ID_Log_Invio")
            _logInvioChiamateW.Cancella(ID_Log_Invio, Tipo_Esportazione, ObjParametri_Server)
            _logInvioAnagrafeW.Cancella(ID_Log_Invio, "", Piva, Sa_Cod, Appezza, "", ObjParametri_Server)

        End If

    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        _logInvioAnagrafeR = Nothing
        _logInvioAnagrafeW = Nothing
        _logInvioChiamateW = Nothing
    End Sub
End Class
