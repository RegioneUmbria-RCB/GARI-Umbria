
Imports System.Net
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreMeteoBiz
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq


Public Class IrriframeAPI_V2

    Public Class Response
        Public Status As Integer
        Public Message As String
        Public Result As Irriframe.WBOutput

        Public Shared Function Fail(msg As String) As Response
            Return New Response With {
                .Status = -1,
                .Message = msg,
                .Result = Nothing
            }
        End Function

        Public Shared Function Success(obj As Irriframe.WBOutput)
            Return New Response With {
                .Status = 0,
                .Message = "",
                .Result = obj}
        End Function
    End Class

    Private ReadOnly _persistent As Boolean
    Private ReadOnly _baseUrl As String

    Public ReadOnly Property Persistent As Boolean
        Get
            Return _persistent
        End Get
    End Property


    Public Sub New(ByRef objParametriServer As AgronicaCoreParametri)

        _persistent = False
        _baseUrl = "https://www.irriframe.it/irriframeApi/api"

        Dim objConfigSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R

        Dim DTConfigSiti As DataTable = objConfigSiti.Leggi(0, "GiasOnline_WS_Irriframe", "", "", objParametriServer)

        If DTConfigSiti IsNot Nothing AndAlso DTConfigSiti.Rows.Count > 0 Then

            Dim strVal = DTConfigSiti.Rows(0)("Valore")

            If Not String.IsNullOrEmpty(strVal) Then

                Try

                    Dim jobj = JObject.Parse(strVal)

                    _persistent = CBool(jobj("persistent"))
                    _baseUrl = jobj("baseUrl").ToString

                Catch ex As Exception

                End Try
            End If
        End If

        If Debugger.IsAttached Then
            '_baseUrl = "http://www2.irriframe.it/irriframeapi/api"
            _persistent = False
            '_baseUrl = "http://localhost/irriframeapi/api"
        End If

    End Sub


    Public Function GetAdvice(input As Irriframe.WBInput) As Response

        Dim token As String = ""

        Try

            Dim body_token As New JObject(
                New JProperty("email", "agronica@irriframe.it"),
                New JProperty("password", "av2608agro@^")
                )

            Dim resp_token = _apiResponse("token", "POST", body_token.ToString)

            If resp_token.StatusCode <> HttpStatusCode.OK Then

                Return Response.Fail(resp_token.Content("Message").ToString.Trim())
            End If

            token = resp_token.Content("token")

        Catch ex As Exception

            Return Response.Fail(ex.Message)
        End Try

        If String.IsNullOrEmpty(token) Then

            Return Response.Fail("Utente non autorizzato")
        End If

        Return _getAdvice(input, token)
    End Function


    Public Function GetAdvicePersist(input As Irriframe.WBInput, authToken As String) As Response

        Return _getAdvice(input, authToken)
    End Function



    Private Function _getAdvice(input As Irriframe.WBInput, authToken As String) As Response

        Try
            Dim advBody As New Irriframe.AdviceBody(input, _persistent)

            If advBody.Crop.ColturaProtetta.HasValue AndAlso
                advBody.Crop.ColturaProtetta.Value AndAlso
                (Not advBody.Crop.Id_GreenHouseType.HasValue OrElse advBody.Crop.Id_GreenHouseType.Value = 0) Then

                Return Response.Fail("Il tipo di protezione per la coltura non è sopportato")
            End If

            Dim jmeteo = _getMeteo(input, advBody.RunData)
            'se volessi usare i dati meteo Irriframe -> jmeteo = Nothing

            If jmeteo Is Nothing Then

                Return Response.Fail("Dati meteo non disponibili")
            End If

            advBody.MeteoGList = JsonConvert.DeserializeObject(Of List(Of Irriframe.MeteoG))(jmeteo("data").ToString())

            Dim meteoModel = {(New With {.field = String.Empty, .label = String.Empty})}.ToList()
            meteoModel = JsonConvert.DeserializeAnonymousType(jmeteo("model").ToString(), meteoModel)

            Dim UmTerrFields = meteoModel.FindAll(Function(e) e.field.ToUpper().StartsWith("UT_"))
            Dim UmTerrData As New SortedList(Of DateTime, Dictionary(Of String, Decimal))

            If UmTerrFields.Count > 0 Then

                advBody.SoilMoistures = New List(Of Irriframe.SoilMoisture)

                Dim dt As Date
                Dim ut_val As Decimal?
                Dim first As Boolean

                For Each md As JObject In jmeteo("data")

                    dt = md("Date")
                    first = True
                    Dim vDict As New Dictionary(Of String, Decimal)

                    For Each ut In UmTerrFields

                        ut_val = md(ut.field)

                        If ut_val.HasValue Then

                            If first AndAlso ut_val.Value > 0 Then

                                advBody.SoilMoistures.Add(New Irriframe.SoilMoisture With {.Data = dt, .SoilMoisturePercVol = ut_val})
                            End If

                            vDict.Add(ut.field, ut_val)
                        End If

                        first = False
                    Next

                    If vDict.Count > 0 Then

                        UmTerrData.Add(dt, vDict)
                    End If
                Next
            End If

            Dim service As String = "AdviceByPostExt"

            If Not _persistent Then

                service = "AdviceByPost"
            End If

            Dim jss As New JsonSerializerSettings() With {
                .NullValueHandling = NullValueHandling.Ignore,
                .DateFormatString = "yyyy-MM-dd"
            }

            Dim payload = JsonConvert.SerializeObject(advBody, jss)

            Dim adv_resp = _apiResponse(service, "POST", payload, authToken)

            If {HttpStatusCode.InternalServerError,
                HttpStatusCode.Unauthorized,
                HttpStatusCode.BadRequest,
                HttpStatusCode.NotFound,
                HttpStatusCode.NotAcceptable,
                HttpStatusCode.ExpectationFailed}.Contains(adv_resp.StatusCode) Then

                '401 UNAUTHORIZED Chiamante non abilitato o User non proprietario del Plot
                '400 BAD REQUEST Chiamata incompleta (Mancanza QueryString...)
                '404 NOTFOUND appezzamento non esistente
                '417 EXPECTATIONFAILED: “Coltura scaduta
                '406 NotAcceptable {"Message":"dati mancanti per il calcolo"}

                Return Response.Fail(adv_resp.Content("Message").ToString().Trim())
            End If

            Dim strResp = JsonConvert.SerializeObject(adv_resp.Content)

            Dim objResp = JsonConvert.DeserializeObject(Of Irriframe.AdviceResponse)(strResp)

            Dim output As New Irriframe.WBOutput With {
                .DataEsecuzione = objResp.DataEsecuzione, 'dataEsecuzione.ToString("u")),
                .IrrigazioniPreviste = New List(Of Irriframe.IrrigazionePrevistaOut),
                .TurniIrriguiPrevisti = New List(Of Irriframe.TurnoIrriguoPrevisto),
                .InfoProvider = objResp.InfoProvider,
                .ConsumoMM = objResp.ConsumoColturaMM,
                .FertiResidualN = objResp.FertiResidualN,
                .FertiResidualP = objResp.FertiResidualP,
                .FertiResidualK = objResp.FertiResidualK,
                .FertiRecipeN = objResp.FertiRecipeN,
                .FertiRecipeP = objResp.FertiRecipeP,
                .FertiRecipeK = objResp.FertiRecipeK,
                .FertiGivenN = objResp.FertiGivenN,
                .FertiGivenP = objResp.FertiGivenP,
                .FertiGivenK = objResp.FertiGivenK,
                .NextFertiDate = objResp.NextFertiDate,
                .IrriframeData = New Irriframe.WBOutput.IFData With {.PlotId = 0, .CropId = 0}
            }

            If objResp.IdPlot.HasValue And objResp.IdCHU.HasValue Then
                output.IrriframeData.PlotId = objResp.IdPlot.Value
                output.IrriframeData.CropId = objResp.IdCHU.Value
            End If

            If objResp.IrrigazioniPreviste IsNot Nothing Then

                For Each irri In objResp.IrrigazioniPreviste

                    output.IrrigazioniPreviste.Add(New Irriframe.IrrigazionePrevistaOut With {
                                                   .Data = irri.DataPrevistaIrri,
                                                   .VolumeMM = irri.VolumeIrriMM
                                                   })
                Next
            End If

            If output.IrrigazioniPreviste.Count = 0 Then
                ' vedo se ho un consiglio altrove...

                If objResp.VolumeIrriMm.HasValue AndAlso objResp.VolumeIrriMm.Value > 0 Then

                    If objResp.DataPrevistaIrriDt.HasValue AndAlso objResp.DataPrevistaIrriDt.Value > Date.MinValue Then

                        output.IrrigazioniPreviste.Add(New Irriframe.IrrigazionePrevistaOut With {
                                                       .Data = objResp.DataPrevistaIrriDt.Value,
                                                       .VolumeMM = objResp.VolumeIrriMm.Value
                                                       })
                    End If
                End If
            End If

            If input.Mac_Cod > 0 And input.Portata > 0 And output.IrrigazioniPreviste.Count > 0 Then

                output.TurniIrriguiPrevisti = _getTurniIrrigui(input, output.IrrigazioniPreviste(0))
            End If

            Dim moutput As New AgronicaCoreModelliPrevisionaliBIZ.OutputModello

            moutput.aggiungiColonna("Data", GetType(Date), "Data", "dd/MM/yyyy")
            moutput.aggiungiColonna("Prec", GetType(Decimal), "Piogge", "0.00")
            moutput.aggiungiColonna("Irri", GetType(Decimal), "Irrigazioni", "0.00")
            moutput.aggiungiColonna("Umid", GetType(Decimal), "Umidità terreno", "0.00")
            moutput.aggiungiColonna("SoInf", GetType(Decimal), "Soglia di intervento", "0.00")
            moutput.aggiungiColonna("SoSup", GetType(Decimal), "Soglia superiore", "0.00")
            moutput.aggiungiColonna("SoInfPerc", GetType(Decimal), "Soglia inferiore %", "0.00")._hidden = True
            moutput.aggiungiColonna("SoSupPerc", GetType(Decimal), "Soglia superiore %", "0.00")._hidden = True
            moutput.aggiungiColonna("IrriPrev", GetType(Decimal), "Irrigazioni previste", "0.00")._hidden = True

            For Each ut In UmTerrFields

                moutput.aggiungiColonna(ut.field, GetType(Decimal), ut.label, "0.00")
            Next

            Dim csvList = Irriframe.CSVParser.GetList(objResp.Csv)

            Dim idx_irri As Integer = 0

            For Each wbe In csvList

                Dim curr_dt As Date = wbe.Data
                Dim irri As Decimal = wbe.Irri
                Dim irriAdv As Decimal = 0

                If idx_irri < output.IrrigazioniPreviste.Count Then

                    Dim irri_dt = output.IrrigazioniPreviste(idx_irri).Data

                    If irri_dt.DayOfYear = curr_dt.DayOfYear Then

                        irri = 0
                        irriAdv = output.IrrigazioniPreviste(idx_irri).VolumeMM

                        idx_irri += 1
                    End If
                End If

                moutput.AddField(curr_dt)
                moutput.AddField(wbe.Prec)
                moutput.AddField(irri)
                moutput.AddField(wbe.Umid)
                moutput.AddField(wbe.SoInf)
                moutput.AddField(wbe.SoSup)
                moutput.AddField(wbe.SoInfPerc)
                moutput.AddField(wbe.SoSupPerc)
                moutput.AddField(irriAdv)

                If UmTerrFields.Any() Then

                    Dim vDict As New Dictionary(Of String, Decimal)
                    Dim found = UmTerrData.TryGetValue(curr_dt, vDict)
                    Dim val As Decimal

                    For Each f In UmTerrFields

                        If found AndAlso vDict.TryGetValue(f.field, val) Then

                            moutput.AddField(val)
                        Else

                            moutput.AddField(DBNull.Value)
                        End If
                    Next
                End If

                moutput.Commit()
            Next

            output.Table = moutput.Output()


            If output.TurniIrriguiPrevisti.Count > 0 Then

                Dim moutput2 As New AgronicaCoreModelliPrevisionaliBIZ.OutputModello

                moutput2.aggiungiColonna("Data", GetType(Date), "Data", "dd/MM/yyyy")
                moutput2.aggiungiColonna("Prec", GetType(Decimal), "Piogge", "0.00")
                moutput2.aggiungiColonna("Irri", GetType(Decimal), "Irrigazioni", "0.00")
                moutput2.aggiungiColonna("Umid", GetType(Decimal), "Umidità terreno", "0.00")
                moutput2.aggiungiColonna("SoInf", GetType(Decimal), "Soglia di intervento", "0.00")
                moutput2.aggiungiColonna("SoSup", GetType(Decimal), "Soglia superiore", "0.00")
                moutput2.aggiungiColonna("SoInfPerc", GetType(Decimal), "Soglia inferiore %", "0.00")._hidden = True
                moutput2.aggiungiColonna("SoSupPerc", GetType(Decimal), "Soglia superiore %", "0.00")._hidden = True
                moutput2.aggiungiColonna("IrriPrev", GetType(Decimal), "Irrigazioni previste", "0.00")._hidden = True

                For Each ut In UmTerrFields

                    moutput2.aggiungiColonna(ut.field, GetType(Decimal), ut.label, "0.00")
                Next

                csvList = Irriframe.CSVParser.GetList(objResp.Csv)

                idx_irri = 0

                For Each wbe In csvList

                    Dim curr_dt As Date = wbe.Data
                    Dim irri As Decimal = wbe.Irri
                    Dim irriAdv As Decimal = 0

                    If idx_irri < output.TurniIrriguiPrevisti.Count Then

                        Dim irri_dt = output.TurniIrriguiPrevisti(idx_irri).Data

                        While irri_dt.DayOfYear = curr_dt.DayOfYear

                            irri = 0
                            irriAdv = output.TurniIrriguiPrevisti(idx_irri).VolumeMM

                            idx_irri += 1

                            If idx_irri >= output.TurniIrriguiPrevisti.Count Then
                                Exit While
                            End If

                            irri_dt = output.TurniIrriguiPrevisti(idx_irri).Data
                        End While
                    End If

                    moutput2.AddField(curr_dt)
                    moutput2.AddField(wbe.Prec)
                    moutput2.AddField(irri)
                    moutput2.AddField(wbe.Umid)
                    moutput2.AddField(wbe.SoInf)
                    moutput2.AddField(wbe.SoSup)
                    moutput2.AddField(wbe.SoInfPerc)
                    moutput2.AddField(wbe.SoSupPerc)
                    moutput2.AddField(irriAdv)

                    If UmTerrFields.Any() Then

                        Dim vDict As New Dictionary(Of String, Decimal)
                        Dim found = UmTerrData.TryGetValue(curr_dt, vDict)
                        Dim val As Decimal

                        For Each f In UmTerrFields

                            If found AndAlso vDict.TryGetValue(f.field, val) Then

                                moutput2.AddField(val)
                            Else

                                moutput2.AddField(DBNull.Value)
                            End If
                        Next
                    End If

                    moutput2.Commit()
                Next

                output.TableTurni = moutput2.Output()
            Else

                output.TableTurni = ""
            End If


            Return Response.Success(output)

        Catch ex As Exception

            Return Response.Fail(ex.Message)
        End Try

        Return Response.Fail("Errore WS...")
    End Function


    Private Function _apiResponse(service As String, method As String, jsonBody As String, Optional token As String = "") As AgronicaCoreUtility.Http.JsonRestResponse

        Dim hdrs As WebHeaderCollection = Nothing
        If Not String.IsNullOrEmpty(token) Then
            hdrs = New WebHeaderCollection
            hdrs.Add("Authorization", "Bearer " & token)
        End If

        Dim http As New AgronicaCoreUtility.Http

        Dim resp = http.CallWS_RestSharp_JSON(_baseUrl, service, method, jsonBody, hdrs)

        Return resp
    End Function


    Private Function _getMeteo(input As Irriframe.WBInput, runData As Date?) As JObject

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Super_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Super_Server")

        'Dim objMeteoNT As New AgronicaCoreWebService.MeteoNT
        Dim objMeteoSuite As New InterfacciaMeteoSuite(objParametri_Server, objParametri_Super_Server)

        Dim TipoSorgente As Integer = enum_Meteo_Tiposorgente.Pubbliche
        Dim Sorgente As Integer = 0

        If input.UsaMeteo Then
            'Dati meteo proprietari

            If input.StazioneMeteo IsNot Nothing Then

                TipoSorgente = input.StazioneMeteo.Sorgente
                Sorgente = input.StazioneMeteo.Stazione
            End If
        Else

            'Dati meteo pubblici Hypermeteo

            If input.Coord IsNot Nothing Then

                Dim piva As String = HttpContext.Current.Session("_piva")

                If String.IsNullOrEmpty(piva) Then
                    Dim objParametriAgenda As New AgronicaCoreModello.ParametriAgenda_Temp.ParametriAgenda
                    piva = objParametriAgenda.Piva
                End If

                Dim objP As New JObject
                objP("PIVA_Superuser") = objParametri_Server.PivaSuperUser
                objP("PIVA") = piva
                objP("TipoSorgente") = TipoSorgente
                objP("DistanzaDa") = New JObject(
                    New JProperty("lat", input.Coord.Lat),
                    New JProperty("lng", input.Coord.Lng))

                'Dim json = JObject.Parse(objMeteoNT.LeggiStazioniXSorgente(objP, objParametri_Server))
                Dim json = JObject.Parse(objMeteoSuite.LeggiStazioniXSorgente(objP))

                Dim arrStazioni = JArray.Parse(json("RispostaStringa").ToString())

                If arrStazioni.Type = JTokenType.Array Then

                    If arrStazioni.Count > 0 Then

                        Sorgente = arrStazioni(0)("id_stazione")
                    End If
                End If
            End If
        End If

        If Sorgente = 0 Then

            Return Nothing
        End If

        '*** Stazioni_Meteo -> Leggere i dati da DataFaseStart a RunData (oggi) ***
        Dim objParams As New JObject
        objParams("TipoSorgente") = TipoSorgente
        objParams("Sorgente") = Sorgente
        objParams("DataInizio") = input.DataFaseStart.ToLocalTime
        objParams("DataFine") = If(runData, DateTime.Now()).AddDays(20).Date()

        'Dim ss As String = objMeteoNT.DatiMeteoElaboraIrrigazione(objParams, objParametri_Server)
        Dim ss As String = objMeteoSuite.DatiMeteoElaboraIrrigazione(objParams)
        Dim obj = JsonConvert.DeserializeObject(ss)

        If CBool(obj("RispostaOK")) Then

            Return JObject.Parse(obj("RispostaStringa"))
        End If

        Return Nothing
    End Function

    Private Function _getTurniIrrigui(input As Irriframe.WBInput, irrig As Irriframe.IrrigazionePrevistaOut) As List(Of Irriframe.TurnoIrriguoPrevisto)

        Dim turniIrrigOut As New List(Of Irriframe.TurnoIrriguoPrevisto)

        If IsNothing(input) OrElse Not input.Mac_Cod.HasValue OrElse input.Mac_Cod = 0 OrElse Not input.Portata.HasValue OrElse input.Portata <= 0 Then
            Return turniIrrigOut
        End If

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")

        Dim objParcoMacchineR As New AgronicaCoreContabDAL.Parco_Macchine_R
        Dim dtRateiTempo As DataTable = objParcoMacchineR.Leggi_RateiTempo_Macchina(input.Mac_Cod, objParametri_Server)

        If IsNothing(dtRateiTempo) OrElse dtRateiTempo.Rows.Count = 0 Then
            Return turniIrrigOut
        End If

        Dim turniIrrigDisponibili As New List(Of Irriframe.TurnoIrriguoDisponibile)

        For Each row In dtRateiTempo.Rows
            Dim DataInizio As Date = row("DataInizio")
            Dim DataFine As Date = row("DataFine")
            Dim OraInizio As DateTime = row("OraInizio")
            Dim OraFine As DateTime = row("OraFine")
            Dim Rotazione As Integer = row("Rotazione")

            Dim DataInizio_ As Date = New Date(Now.Year, DataInizio.Month, DataInizio.Day)
            Dim DataFine_ As Date = New Date(Now.Year, DataFine.Month, DataFine.Day)
            Dim OraInizio_ As DateTime = New DateTime(1900, 1, 1, OraInizio.Hour, OraInizio.Minute, 0)
            Dim OraFine_ As DateTime = New DateTime(1900, 1, 1, OraFine.Hour, OraFine.Minute, 0)
            Dim ElapsedTime = DateTime.Parse(OraFine_.ToString()).Subtract(DateTime.Parse(OraInizio_.ToString()))

            Dim DurataTurnoM As Integer = ElapsedTime.Hours * 60 + ElapsedTime.Minutes
            Dim DataTurno As Date = If(irrig.Data > DataInizio_, irrig.Data, DataInizio_)
            Dim DataOraTurno As DateTime

            While DataTurno <= DataFine_
                DataOraTurno = New DateTime(DataTurno.Year, DataTurno.Month, DataTurno.Day, OraInizio.Hour, OraInizio.Minute, 0)
                turniIrrigDisponibili.Add(New Irriframe.TurnoIrriguoDisponibile With {.Data = DataOraTurno, .DurataM = DurataTurnoM})

                If Rotazione = 0 Then
                    Exit While
                End If

                DataTurno = DataTurno.AddDays(Rotazione)
            End While
        Next

        If turniIrrigDisponibili.Count > 0 Then

            ' Ordinamento turni disponibili per data
            turniIrrigDisponibili.Sort(Function(x, y) DateTime.Compare(x.Data, y.Data))

            Dim TurnoIrrigDisp As Irriframe.TurnoIrriguoDisponibile

            ' Combino eventuali turni disponibili sovrapposti 
            For i As Integer = turniIrrigDisponibili.Count - 2 To 0 Step -1

                TurnoIrrigDisp = turniIrrigDisponibili.ElementAt(i)

                If TurnoIrrigDisp.Data.AddMinutes(TurnoIrrigDisp.DurataM) >= turniIrrigDisponibili.ElementAt(i + 1).Data Then

                    Dim TurnoIrrigDisp2 As Irriframe.TurnoIrriguoDisponibile = turniIrrigDisponibili.ElementAt(i + 1)

                    Dim endDate1 = TurnoIrrigDisp.Data.AddMinutes(TurnoIrrigDisp.DurataM)
                    Dim endDate2 = TurnoIrrigDisp2.Data.AddMinutes(TurnoIrrigDisp2.DurataM)
                    Dim endDateMax = If(endDate1 > endDate2, endDate1, endDate2)
                    Dim combinedDuration = DateDiff(DateInterval.Minute, TurnoIrrigDisp.Data, endDateMax)

                    turniIrrigDisponibili.RemoveAt(i + 1)
                    turniIrrigDisponibili.RemoveAt(i)
                    turniIrrigDisponibili.Insert(i, New Irriframe.TurnoIrriguoDisponibile With {.Data = TurnoIrrigDisp.Data, .DurataM = combinedDuration})
                End If
            Next

            ' Formula di conversione da quantità in mm a tempo in ore: ore = (mm * m^2) / (litri/ora) 
            Dim TempoIrrigTot As Decimal = (irrig.VolumeMM * input.Superficie_ha * 10000) / input.Portata
            Dim TempoIrrigTotM As Integer = TempoIrrigTot * 60

            Dim TempoIrrigRimasto = TempoIrrigTotM
            Dim TempoTurnoIrrigM As Integer
            Dim TempoTurnoIrrigH As Decimal
            Dim VolumeMMTurnoIrrig As Decimal

            For i As Integer = 0 To turniIrrigDisponibili.Count - 1

                If TempoIrrigRimasto <= 0 Then
                    Exit For
                End If

                TurnoIrrigDisp = turniIrrigDisponibili.ElementAt(i)

                If TempoIrrigRimasto > TurnoIrrigDisp.DurataM Then
                    TempoTurnoIrrigM = TurnoIrrigDisp.DurataM
                    TempoIrrigRimasto = TempoIrrigRimasto - TurnoIrrigDisp.DurataM
                Else
                    TempoTurnoIrrigM = TempoIrrigRimasto
                    TempoIrrigRimasto = 0
                End If

                TempoTurnoIrrigH = TempoTurnoIrrigM / 60
                ' Formula inversa: mm = (ore * litri/ora) / m^2
                VolumeMMTurnoIrrig = (TempoTurnoIrrigH * input.Portata) / (input.Superficie_ha * 10000)

                turniIrrigOut.Add(New Irriframe.TurnoIrriguoPrevisto With {.Data = TurnoIrrigDisp.Data, .DurataM = TempoTurnoIrrigM, .VolumeMM = VolumeMMTurnoIrrig})
            Next

        End If

        Return turniIrrigOut

    End Function

End Class


#If False Then
    
Public Class IrriframeApi

    Public Function CreaCredenziali(username As String) As JObject

        Try

            Dim token = _getMasterToken()

            Dim userCredentials As New JObject(
                New JProperty("email", username),
                New JProperty("password", "password")
                )

            Dim resp = _apiResponse("UserExt", "POST", userCredentials.ToString, token)

            Return JObject.FromObject(resp.Content)

        Catch ex As Exception

            Return Nothing
        End Try

        Return Nothing
    End Function

    Public Function LeggiCredenziali(user_id As Integer, username As String) As JObject

        Try

            Dim token = _getMasterToken()

            Dim resp = _apiResponse("UserExt/" & user_id.ToString(), "GET", "", token)

            Return JObject.FromObject(resp.Content)

        Catch ex As Exception

            Return Nothing
        End Try

        Return Nothing
    End Function


    Public Class Irri2Save
        Public Id_Plot As Integer
        Public DataIrri As Date
        Public VolumeMM As Decimal
        Public User As Integer
    End Class

    Public Function SaveIrrigation(i2save As Irri2Save, authToken As String) As JObject

        Try

            Dim irriBody As New JObject(
                New JProperty("Id_Plot", i2save.Id_Plot),
                New JProperty("DataIrri", i2save.DataIrri.ToLocalTime.ToString("yyyy-MM-dd")),
                New JProperty("Volumemm", i2save.VolumeMM),
                New JProperty("User", i2save.User)
                )

            Dim resp = _apiResponse("RegIrrigation", "POST", irriBody.ToString, authToken)

            If {HttpStatusCode.Created, HttpStatusCode.OK, HttpStatusCode.Accepted}.Contains(resp.StatusCode) Then

                Return _success(Nothing)
            Else

                Return _fail(resp.Content("Message").ToString().Trim())
            End If

        Catch ex As Exception

            Return _fail(ex.Message)
        End Try

        Return _fail("")
    End Function

    Private Function _getMasterToken() As String

        Dim masterCredentials As New JObject(
            New JProperty("email", "agronica@irriframe.it"),
            New JProperty("password", "av2608agro@^")
            )

        Try
            Dim respToken = _apiResponse("token", "POST", masterCredentials.ToString)

            If respToken.StatusCode = HttpStatusCode.OK Then

                Return respToken.Content("token")
            Else

                'Throw New Exception(resp_token.Content("Message").ToString.Trim())
                Return ""
            End If

        Catch ex As Exception

            Return ""
        End Try

        Return ""
    End Function
  

End Class


#End If
