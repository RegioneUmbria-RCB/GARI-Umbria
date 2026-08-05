Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDTOStd.SmartTractors_HubIoT
Imports Newtonsoft.Json

Public Class Settings_R
    Inherits LogProvider

    'Public Function LeggiImpostazioni(ByVal PivaSuperUSer As String,
    '                                  ByVal piva As String,
    '                                  ByRef objParametri As AgronicaCoreParametri
    '                                 ) As Settings

    '    Dim NomeRoutine As String = "AgronicaCoreHubIoTBIZ.Settings_R.LeggiImpostazioni()"
    '    Dim MessaggioErrore As String = ""
    '    Dim obj As Settings = Nothing

    '    Try
    '        Dim cfgReader As New AgronicaCoreHubIoTDAL.HubIoT_Settings_R
    '        Dim dtRes = cfgReader.Leggi(PivaSuperUSer, piva, "", "", objParametri)
    '        If dtRes.Rows.Count <= 0 Then
    '            Throw New Exception("Nessuna configurazione trovata.")
    '        End If

    '        obj = New Settings() With {
    '                    .PivaSuperUser = dtRes.Rows(0)("PivaSuperUser").ToString(),
    '                    .piva = dtRes.Rows(0)("Piva").ToString(),
    '                    .mappaturaDatiMappaPrescrizione = JsonConvert.DeserializeObject(Of List(Of ElencoColonneDBFPrescriptionMap))(dtRes.Rows(0)("Parametri").ToString())
    '                }

    '    Catch ex As Exception
    '        obj = Nothing
    '        MessaggioErrore = ex.Message
    '        Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
    '        Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
    '    End Try

    '    Return obj
    'End Function

    Public Shared Function CalcolaNumeroThreadPerElaborazioneGSB(ByRef objParametri As AgronicaCoreParametri) As Integer
        Dim numThread As Integer = 1
        Try
            Dim reader As New AgronicaCoreHubIoTDAL.HubIoT_ParametriConnessioni_R

            Dim dtres = reader.Leggi(objParametri.PivaSuperUser, "", "", "", objParametri)

            numThread = dtres.Rows.Count

        Catch ex As Exception
            numThread = 1
            Throw New Exception(ex.Message, ex)
        End Try
        Return numThread
    End Function
    Public Shared Function GetElencoPartiteIvaDaSplittareIntasks(ByRef objParametri As AgronicaCoreParametri) As List(Of String)
        Dim lista As New List(Of String)
        Try
            Dim reader As New AgronicaCoreHubIoTDAL.HubIoT_ParametriConnessioni_R

            Dim dtres = reader.Leggi(objParametri.PivaSuperUser, "", "", "", objParametri)
            For Each row In dtres.Rows
                lista.Add(row("Piva"))
            Next

        Catch ex As Exception
            lista.Clear()
            Throw New Exception(ex.Message, ex)
        End Try
        Return lista
    End Function

    Public Shared Function GetElencoOrgIDPerConfigurazione(ByVal PivaSuperUser As String,
                                                           ByVal Piva As String,
                                                           ByRef objParametri As AgronicaCoreParametri) As Integer

    End Function

End Class
Public Class Settings_W
    Inherits LogProvider

End Class
