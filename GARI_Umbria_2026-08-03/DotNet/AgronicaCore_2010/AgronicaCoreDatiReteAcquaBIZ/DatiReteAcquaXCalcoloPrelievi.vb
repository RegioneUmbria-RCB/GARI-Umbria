Imports Newtonsoft.Json.Linq
Imports Newtonsoft.Json
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreDataProvider

Public Class DatiReteAcquaXCalcoloPrelievi
    Private _objMeteoNT As AgronicaCoreWebService.MeteoNT
    Private _objDatiIOT As AgronicaCoreWebService.DatiIOT
    Private _objParametri_SuperServer As AgronicaCoreParametri
    Private _objParametri_Server As AgronicaCoreParametri
    Private _objParametri_Utenti As AgronicaCoreParametri
    Private _linguaSession As Lingua

    Public Sub New(ByVal lang As Lingua,
                   ByRef objParametri_SuperServer As AgronicaCoreParametri,
                   ByRef objParametri_Server As AgronicaCoreParametri,
                   ByRef objParametri_Utenti As AgronicaCoreParametri)
        _objMeteoNT = New AgronicaCoreWebService.MeteoNT
        _objDatiIOT = New AgronicaCoreWebService.DatiIOT
        _objParametri_SuperServer = objParametri_SuperServer
        _objParametri_Server = objParametri_Server
        _objParametri_Utenti = objParametri_Utenti
        _linguaSession = lang
    End Sub

    Public Function RecuperaDatiContatori(ByVal id_contatore As String, ByVal DataDa As DateTime, ByVal DataA As DateTime) As List(Of DatiAcquaXCalcoloConfronto)
        Dim ret As New List(Of DatiAcquaXCalcoloConfronto)
        Try

            Dim sensor_struct = LeggiStrutturaSensoriDispositivo(id_contatore)

            Dim objParamDatiIoT As New AgronicaCoreDTOStd.InData.IoT.RichiediDatiIOT

            objParamDatiIoT.TipoSorgente = 1
            objParamDatiIoT.Sorgente = id_contatore
            objParamDatiIoT.dataInizio = DataDa.ToString("yyyy-MM-dd")
            objParamDatiIoT.dataFine = DataA.ToString("yyyy-MM-dd")
            objParamDatiIoT.frequenzaDati = "G"
            objParamDatiIoT.LinguaCodiceISO = _linguaSession.CodiceISO

            Dim deserilizedResponse = JsonConvert.DeserializeObject(Of rispostaStandard(Of Risposta_DatiReteAcqua))(_objDatiIOT.DatiIOTElabora(objParamDatiIoT, _objParametri_SuperServer, _objParametri_Server, _objParametri_Utenti))
            If deserilizedResponse.RispostaOK = True Then
                Dim tmp_table = JObject.Parse(deserilizedResponse.RispostaStringa.Meteo_Table)
                Dim temp_water_data As New List(Of TempWaterData)
                Dim jr = JArray.Parse(tmp_table("kendo_rows").ToString)

                For i = 0 To jr.Count - 1

                    Dim obj = JObject.Parse(jr(i).ToString)

                    temp_water_data.Add(New TempWaterData() With {
                                        .data = DateTime.Parse(obj.Property("DataOra").Value().ToString()),
                                        .week = AgronicaCoreUtility.DataOra.GetWeekNumberFromDate(.data, Globalization.CalendarWeekRule.FirstFullWeek),
                                        .TotalWaterVolume = Decimal.Parse(obj.Property("S_" + sensor_struct.Find(Function(x) x.id_tiposensore = 2).id_sensore.ToString()).Value().ToString()),
                                        .DailyWaterVolume = Decimal.Parse(obj.Property("S_" + sensor_struct.Find(Function(x) x.id_tiposensore = 3).id_sensore.ToString()).Value().ToString())
                                        })
                Next

                Dim o As New DatiAcquaXCalcoloConfronto
                Dim week As Integer = -1
                Dim c As Long = 0
                For Each meteo In temp_water_data.OrderBy(Of Integer)(Function(x) x.week)
                    If week <> meteo.week Then
                        If o IsNot Nothing Then
                            If o.settimana <> -1 Then
                                ret.Add(o)
                            End If
                        End If
                        o = New DatiAcquaXCalcoloConfronto()
                        o.anno = meteo.data.Year
                        o.settimana = meteo.week
                        week = meteo.week
                        c += 1
                    End If
                    o.CumulatoFornitore.Add(meteo.TotalWaterVolume)
                    o.CumulatoFornitoreProgressivo.Add(meteo.DailyWaterVolume)
                Next
                If c > ret.Count Then
                    ret.Add(o)
                End If
            Else
                Throw New Exception(deserilizedResponse.Errore)
            End If
        Catch ex As Exception
            ret = Nothing
        End Try
        Return ret
    End Function

    Private Function LeggiStrutturaSensoriDispositivo(ByVal id_contatore As String) As List(Of AgronicaCoreDTOStd.InData.IoT.TipologiaSensoriPerDispositivo)
        Dim sensorList As List(Of AgronicaCoreDTOStd.InData.IoT.TipologiaSensoriPerDispositivo)

        'recupero la struttura dei sensori per la stazione in modo da recuperare dai dati solo la parte di pluviometro e temperatura (con i due sensori derivati max e min)
        Dim objParamStructStation As New AgronicaCoreDTOStd.InData.IoT.LeggiDispositivoIn
        objParamStructStation.id_dispositivo = id_contatore
        Try

            Dim dummy = JsonConvert.DeserializeObject(Of rispostaStandard(Of List(Of AgronicaCoreDTOStd.InData.IoT.TipologiaSensoriPerDispositivo)))(_objDatiIOT.LeggiTipoSensoriPerDispositivo(objParamStructStation, _objParametri_SuperServer, _objParametri_Server, _objParametri_Utenti))

            sensorList = dummy.RispostaStringa

            'tiposensore=2 -> totalwatervolume
            'tiposensore=3 -> dailywatervolume

            If sensorList.Any(Function(x) x.id_tiposensore = 2) = False Or sensorList.Any(Function(x) x.id_tiposensore = 3) = False Then
                Throw New Exception("Il dispositivo " + id_contatore + " non ha la coppia di sensori TotalWaterVolume e/o DailyWaterVolume. Non è possibile recuperare i dati di prelievo dalla rete idrica necessari al calcolo. operazione interrotta")
            End If
        Catch ex As Exception
            sensorList = Nothing
        End Try

        Return sensorList
    End Function

    Private Class TempWaterData
        Public Property data As DateTime
        Public Property week As Integer
        Public Property TotalWaterVolume As Decimal
        Public Property DailyWaterVolume As Decimal
    End Class
End Class
