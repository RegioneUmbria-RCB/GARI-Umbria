Imports System.Net
Imports AgronicaCoreDataProvider
Imports AgronicaCoreVarieDAL
Imports Newtonsoft.Json

Public Class Pulizia
    Inherits DataProvider

    Private objParametri_Super_Server As AgronicaCoreParametri
    Private objParametri_Server As AgronicaCoreParametri
    Private objParametri_Utenti As AgronicaCoreParametri
    Private configServizio As Configurazione_Servizio
    Private Parametri_Extra As String
    'Private _http_Request As AgronicaCoreDTOStd.InData.AgronicaCoreUtility.Http = Nothing
    'Private _linkCoreAPI As String = "http://192.168.1.91:5001/Demetra/Import/AnalisiTerreno"
    Private _hdr As WebHeaderCollection = Nothing

    Sub New(ByVal _Configurazione_Servizio As AgronicaCoreVarieDAL.Configurazione_Servizio,
        ByVal _ObjParametri_Super_Server As AgronicaCoreParametri,
        ByVal _ObjParametri_Server As AgronicaCoreParametri,
        ByVal _ObjParametri_Utenti As AgronicaCoreParametri)

        objParametri_Super_Server = _ObjParametri_Super_Server
        objParametri_Server = _ObjParametri_Server
        objParametri_Utenti = _ObjParametri_Utenti
        configServizio = _Configurazione_Servizio
        Parametri_Extra = _Configurazione_Servizio.Parametri_Extra

    End Sub

    Public Function Esegui(ByRef Messaggio_di_Ritorno_Opzionale As String) As Boolean
        Dim nomeRoutine = "PuliziaTabelleAgronicaLog.Esegui"
        Dim result As Boolean = False
        Dim messaggioErrore As String = ""

        Try
            Dim parametriExtra As ParametriExtra = JsonConvert.DeserializeObject(Of ParametriExtra)(Parametri_Extra)

            If parametriExtra.GgRecordConservazioneLogInvio = 0 Then
                Throw New Exception("Obbligatorio impostare la proprietà GgRecordConservazioneLogInvio > 0")
            End If
            If String.IsNullOrEmpty(parametriExtra.CondizioniSostituzioneLogInvio) Then
                Throw New Exception("Obbligatorio impostare la proprietà CondizioniSostituzioneLogInvio")
            End If

            Dim tabellePulizia As Dictionary(Of String, Integer) = parametriExtra.TabelleLogDaPulire.
                ToDictionary(Function(d) d.NomeTabella, Function(d) If(d.GgConservazione = 0, parametriExtra.GgRecordConservazioneLogInvio, d.GgConservazione))
            tabellePulizia.Add("Agronica_Log_Invio_Chiamate", parametriExtra.GgRecordConservazioneLogInvio)

            Dim objChiamateR As New AgronicaCoreVarieDAL.Agronica_Log_Invio_Chiamate_R
            Dim elementiPulizia As DataTable = objChiamateR.EstraiRecordPuliziaLog(tabellePulizia,
                                                                                   parametriExtra.GgRecordConservazioneLogInvio,
                                                                                   parametriExtra.CondizioniSostituzioneLogInvio,
                                                                                   objParametri_Server)

            If elementiPulizia IsNot Nothing Then
                Dim objChiamateW As New AgronicaCoreVarieDAL.Agronica_Log_Invio_Chiamate_W
                objChiamateW.EseguiPuliziaLog(elementiPulizia, tabellePulizia, objParametri_Server)
            End If

            result = True
        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, "", messaggioErrore)
            Throw New Exception($"[{nomeRoutine}] : {messaggioErrore}")
        End Try

        Return result

    End Function

End Class
