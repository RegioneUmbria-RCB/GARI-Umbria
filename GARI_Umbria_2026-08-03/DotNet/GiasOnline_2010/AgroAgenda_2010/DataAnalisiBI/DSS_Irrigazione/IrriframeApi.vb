
Imports System.IO
Imports System.Net
Imports System.Runtime.Serialization
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreMeteoBiz
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

Public Class IrriframeApi

    Private ReadOnly _persistent As Boolean
    Private ReadOnly _baseUrl As String

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

    Private Function _fail(msg As String) As JObject
        Return New JObject(
            New JProperty("Status", -1),
            New JProperty("Message", msg)
            )
    End Function

    Private Function _success(respObj As JObject) As JObject
        If respObj Is Nothing Then
            respObj = New JObject
        End If
        respObj.Add("Status", 0)
        Return respObj
    End Function

    Private Function _getAdvice(objPar As JObject, authToken As String) As JObject

        Try

            Dim advBody = New _adviceBody(objPar, _persistent)

            Dim runData As Date = Date.Now().Date()
            If advBody.RunData.HasValue Then

                runData = advBody.RunData
            End If

            Dim meteo = _getMeteo(objPar, runData)
            'se volessi usare i dati meteo Irriframe -> meteo = Nothing

            If meteo Is Nothing Then

                Return _fail("Dati meteo non disponibili")
            End If

            If advBody.Crop.ColturaProtetta AndAlso advBody.Crop.Id_GreenHouseType = 0 Then
                Return _fail("Il tipo di protezione non è sopportato")
            End If

            advBody.AddMeteoData(meteo)

            Dim service As String = "AdviceByPostExt"

            If Not _persistent Then

                service = "AdviceByPost"
            End If

            Dim adv_resp = _apiResponse(service, "POST", advBody.Serialize(), authToken)

            If ({HttpStatusCode.InternalServerError, HttpStatusCode.Unauthorized, HttpStatusCode.BadRequest, HttpStatusCode.NotFound, HttpStatusCode.NotAcceptable, HttpStatusCode.ExpectationFailed}).Contains(adv_resp.StatusCode) Then

                '401 UNAUTHORIZED Chiamante non abilitato o User non proprietario del Plot
                '400 BAD REQUEST Chiamata incompleta (Mancanza QueryString...)
                '404 NOTFOUND appezzamento non esistente
                '417 EXPECTATIONFAILED: “Coltura scaduta
                '406 NotAcceptable {"Message":"dati mancanti per il calcolo"}

                Return _fail(adv_resp.Content("Message").ToString().Trim())
            End If

            Dim advResp = _buildAdviceResponse(adv_resp.Content, meteo)

            If advResp IsNot Nothing Then

                Return _success(advResp)
            End If

        Catch ex As Exception

            Return _fail(ex.Message)
        End Try

        Return _fail("Errore WS...")
    End Function

    Public Function Persistent() As Boolean
        Return _persistent
    End Function

    Public Function GetAdvice(objPar As JObject) As JObject

        Dim token As String = ""

        Try

            Dim body_token As New JObject(
                New JProperty("email", "agronica@irriframe.it"),
                New JProperty("password", "av2608agro@^")
                )

            Dim resp_token = _apiResponse("token", "POST", body_token.ToString)

            If resp_token.StatusCode <> HttpStatusCode.OK Then

                Return _fail(resp_token.Content("Message").ToString.Trim())
            End If

            token = resp_token.Content("token")

        Catch ex As Exception

            Return _fail(ex.Message)
        End Try

        If String.IsNullOrEmpty(token) Then

            Return _fail("Utente non autorizzato")
        End If

        Return _getAdvice(objPar, token)
    End Function

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

    Public Function GetAdvicePersist(objPar As JObject, authToken As String) As JObject

        Return _getAdvice(objPar, authToken)
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

    Private Function _getMeteo(objPar As JObject, runData As Date) As JObject

        Dim staz As New StazioneIrriga(objPar)

        '*** Stazioni_Meteo -> Leggere i dati da DataFaseStart a RunData (oggi) ***
        Dim objParams As New JObject
        objParams("TipoSorgente") = staz.TipoSorgente
        objParams("Sorgente") = staz.Sorgente
        objParams("DataInizio") = Convert.ToDateTime(objPar("DataFaseStart")).ToLocalTime
        objParams("DataFine") = runData.AddDays(20).Date()

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Super_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Super_Server")

        'Dim objMeteoNT As New AgronicaCoreWebService.MeteoNT
        Dim objMeteoSuite As New InterfacciaMeteoSuite(objParametri_Server, objParametri_Super_Server)

        'Dim ss As String = objMeteoNT.DatiMeteoElaboraIrrigazione(objParams, objParametri_Server)
        Dim ss As String = objMeteoSuite.DatiMeteoElaboraIrrigazione(objParams)
        Dim obj = JsonConvert.DeserializeObject(ss)

        If CBool(obj("RispostaOK")) Then

            Return JObject.Parse(obj("RispostaStringa"))
        End If

        Return Nothing
    End Function

    Private Function _buildAdviceResponse(objContent As Object, meteo As JObject) As JObject

        Dim dataEsecuzione As DateTime = Date.Parse(objContent("DataEsecuzione").ToString())

        Dim irriPrev = JArray.FromObject(objContent("IrrigazioniPreviste"))
        For Each irri As JObject In irriPrev
            irri("Data") = Date.Parse(irri("DataPrevistaIrri"))
            irri("VolumeMM") = Decimal.Parse(irri("VolumeIrriMM"))
            irri.Remove("DataPrevistaIrri")
            irri.Remove("VolumeIrriMM")
        Next

        If irriPrev.Count = 0 Then

            ' vedo se ho un consiglio altrove...
            Try
                Dim volumemm = Decimal.Parse(objContent("VolumeIrriMm"))
                If volumemm > 0 Then

                    Dim strDT As String = objContent("DataPrevistaIrriDt")
                    If Not String.IsNullOrEmpty(strDT) Then

                        Dim dt As Date = Date.Parse(strDT)
                        If dt > Date.MinValue Then

                            irriPrev.Add(New JObject(
                                         New JProperty("Data", Date.Parse(objContent("DataPrevistaIrriDt"))),
                                         New JProperty("VolumeMM", volumemm)
                                         ))
                        End If
                    End If
                End If
            Catch ex As Exception

            End Try
        End If

        Dim output As New AgronicaCoreModelliPrevisionaliBIZ.OutputModello

        output.aggiungiColonna("Data", GetType(Date), "Data", "dd/MM/yyyy")
        output.aggiungiColonna("Prec", GetType(Decimal), "Piogge", "0.00")
        output.aggiungiColonna("Irri", GetType(Decimal), "Irrigazioni", "0.00")
        output.aggiungiColonna("Umid", GetType(Decimal), "Umidità terreno", "0.00")
        output.aggiungiColonna("SoInf", GetType(Decimal), "Soglia di intervento", "0.00")
        output.aggiungiColonna("SoSup", GetType(Decimal), "Soglia superiore", "0.00")
        output.aggiungiColonna("SoInfPerc", GetType(Decimal), "Soglia inferiore %", "0.00")._hidden = True
        output.aggiungiColonna("SoSupPerc", GetType(Decimal), "Soglia superiore %", "0.00")._hidden = True
        output.aggiungiColonna("IrriPrev", GetType(Decimal), "Irrigazioni previste", "0.00")._hidden = True

        Dim UmTerrFields As New List(Of String)
        Dim meteoData As JArray = Nothing

        If meteo IsNot Nothing Then

            meteoData = meteo("data")

            Dim model = meteo("model")
            For Each o As JObject In model

                Dim field = o("field").ToString
                If field.StartsWith("UT_") Then

                    UmTerrFields.Add(field)
                    output.aggiungiColonna(field, GetType(Decimal), o("label"), "0.00")
                End If
            Next
        End If

        Dim csvList = Irriframe.CSVParser.GetList(objContent("Csv").ToString())

        Dim idx_meteo As Integer = 0
        Dim idx_irri As Integer = 0

        For Each wbe In csvList

            Dim curr_dt As Date = wbe.Data
            Dim irri As Decimal = wbe.Irri
            Dim irriAdv As Decimal = 0

            If idx_irri < irriPrev.Count Then

                Dim irri_dt = CDate(irriPrev(idx_irri)("Data"))

                If irri_dt.Year = curr_dt.Year AndAlso irri_dt.Month = curr_dt.Month AndAlso irri_dt.Day = curr_dt.Day Then

                    irri = 0
                    irriAdv = CDec(irriPrev(idx_irri)("VolumeMM"))

                    idx_irri += 1
                End If
            End If

            output.AddField(curr_dt)
            output.AddField(wbe.Prec)
            output.AddField(irri)
            output.AddField(wbe.Umid)
            output.AddField(wbe.SoInf)
            output.AddField(wbe.SoSup)
            output.AddField(wbe.SoInfPerc)
            output.AddField(wbe.SoSupPerc)
            output.AddField(irriAdv)

            If UmTerrFields.Any() Then

                Dim dtm As Date = Date.MinValue
                While idx_meteo < meteoData.Count AndAlso dtm < curr_dt
                    dtm = CDate(meteoData(idx_meteo)("Date"))
                    idx_meteo += 1
                End While

                For Each f In UmTerrFields
                    If dtm = curr_dt Then

                        If meteoData(idx_meteo - 1)(f) IsNot Nothing Then

                            output.AddField(CDec(meteoData(idx_meteo - 1)(f)))
                        End If
                    Else

                        output.AddField(DBNull.Value)
                    End If
                Next
            End If

            output.Commit()
        Next

        Dim PlotId As Integer
        Dim CropId As Integer
        Try
            PlotId = objContent("IdPlot")
            CropId = objContent("IdCHU")
        Catch ex As Exception
            PlotId = 0
            CropId = 0
        End Try

        Dim ConsumoMM As New Decimal?()

        If objContent("ConsumoColturaMM") IsNot Nothing Then
            ConsumoMM = objContent("ConsumoColturaMM")
        End If

        Return New JObject(
            New JProperty("DataEsecuzione", dataEsecuzione.ToString("u")),
            New JProperty("InfoProvider", objContent("InfoProvider").ToString),
            New JProperty("IrrigazioniPreviste", irriPrev),
            New JProperty("ConsumoMM", ConsumoMM),
            New JProperty("FertiResidualN", objContent("FertiResidualN")),
            New JProperty("FertiResidualP", objContent("FertiResidualP")),
            New JProperty("FertiResidualK", objContent("FertiResidualK")),
            New JProperty("FertiRecipeN", objContent("FertiRecipeN")),
            New JProperty("FertiRecipeP", objContent("FertiRecipeP")),
            New JProperty("FertiRecipeK", objContent("FertiRecipeK")),
            New JProperty("FertiGivenN", objContent("FertiGivenN")),
            New JProperty("FertiGivenP", objContent("FertiGivenP")),
            New JProperty("FertiGivenK", objContent("FertiGivenK")),
            New JProperty("NextFertiDate", objContent("NextFertiDate")),
            New JProperty("Table", output.Output()),
            New JProperty("IrriframeData", New JObject(
                New JProperty("PlotId", PlotId),
                New JProperty("CropId", CropId))
                )
            )
    End Function




    <DataContract>
    Public Class _adviceBody

        <DataContract>
        Public Class _plot
            <DataMember>
            Public CUAA As String
            <DataMember>
            Public Description As String
            <DataMember>
            Public Latitude As Decimal
            <DataMember>
            Public Longitude As Decimal
            <DataMember>
            Public Slope As Integer
            <DataMember>
            Public Horizon1Clay As Decimal?
            <DataMember>
            Public Horizon1Sand As Decimal?
            <DataMember>
            Public IrriSysCode As Integer?
            <DataMember>
            Public Superficie As Integer?
            'Horizon1Height
            'Horizon1Stone
            'IrriFlowRate Pluviometria impianto mm/h (Values: between 0.1 and 1000)
        End Class

        <DataContract>
        Public Class _crop
            <DataMember>
            Public Id_CropType As Integer
            <DataMember>
            Public DataFaseStart As Date?
            <DataMember>
            Public DistaTraFilaPiante As Decimal?
            <DataMember>
            Public DistaSuFilaPiante As Decimal?
            <DataMember>
            Public ConduInterfilare As String
            <DataMember>
            Public ClasseVigore As Integer? 'Debole / Medio / Vigoroso / Molto vigoroso
            <DataMember>
            Public AnnoImpianto As Integer?
            <DataMember>
            Public ColturaProtetta As Boolean?
            <DataMember>
            Public Id_GreenHouseType As Integer?
        End Class

        <DataContract>
        Public Class _irrigation
            <DataMember>
            Public DataIrri As String
            <DataMember>
            Public VolumeMM As Decimal
            Public Sub New(d As String, v As Decimal)
                DataIrri = d
                VolumeMM = v
            End Sub
        End Class

        <DataContract>
        Public Class _soilMoisture
            <DataMember(Name:="Date")>
            Public Data As Date
            <DataMember>
            Public SoilMoisturePercVol As Decimal
            Public Sub New(d As Date, p As Decimal)
                Data = d
                SoilMoisturePercVol = p
            End Sub
        End Class

        <DataMember>
        Public IdPlot As Integer?
        <DataMember>
        Public Plot As _plot
        <DataMember>
        Public Crop As _crop
        <DataMember>
        Public IsCsv As Integer
        <DataMember>
        Public IsIrriUser As Integer
        <DataMember>
        Public RunData As Date?
        <DataMember>
        Public Irrigations As List(Of _irrigation)
        <DataMember>
        Public MeteoGList As JArray
        <DataMember>
        Public SoilMoistures As List(Of _soilMoisture)

        <DataContract>
        Public Structure _FertiRecipe
            <DataMember>
            Public N_Ammesso As Decimal
            <DataMember>
            Public P_Ammesso As Decimal
            <DataMember>
            Public K_Ammesso As Decimal
        End Structure

        <DataMember>
        Public FertiRecipe As _FertiRecipe?

        <DataContract>
        Public Structure _FertiIrriG
            <DataMember>
            Public Data As DateTime
            <DataMember>
            Public TitoloN As Decimal
            <DataMember>
            Public TitoloP As Decimal
            <DataMember>
            Public TitoloK As Decimal
            <DataMember>
            Public DoseKg As Decimal
            <DataMember>
            Public NomeCommercialeDescri As String
        End Structure

        <DataMember>
        Public FertIrriGList As List(Of _FertiIrriG)


        Private Function IsValidProperty(tok As JToken) As Boolean
            If tok Is Nothing OrElse tok.Type = JTokenType.Null Then
                Return False
            End If
            Return True
        End Function

        Public Sub New(objPar As JObject, persistent As Boolean)

            IsCsv = 1

            If persistent Then

                IdPlot = CInt(objPar("PersistData")("Irri_PlotId"))
            End If

            If objPar("RunData") IsNot Nothing Then

                RunData = objPar("RunData")
            End If

            Plot = New _plot With {
                .CUAA = objPar("CUAA").ToString,
                .Description = objPar("Impianto").ToString,
                .Latitude = 0,
                .Longitude = 0
            }

            If objPar("Sup_Imp") IsNot Nothing Then
                ' Ha -> Metri quadrati
                Plot.Superficie = CDec(objPar("Sup_Imp")) * 10000
            End If

            If IsValidProperty(objPar("LatLng")) AndAlso objPar("LatLng").HasValues Then
                Plot.Latitude = CDec(objPar("LatLng")("Lat")) 'Decimal.Parse(objPar("Lat"), CultureInfo.InvariantCulture)
                Plot.Longitude = CDec(objPar("LatLng")("Lng")) 'Decimal.Parse(objPar("Lng"), CultureInfo.InvariantCulture)
            End If

            Dim Pendenza As Decimal = CDec(objPar("Pendenza"))
            If Pendenza < 5 Then
                Plot.Slope = 2
            ElseIf Pendenza <= 10 Then
                Plot.Slope = 7
            ElseIf Pendenza <= 20 Then
                Plot.Slope = 15
            Else
                Plot.Slope = 22
            End If

            If IsValidProperty(objPar("Argilla")) Then
                Plot.Horizon1Clay = CDec(objPar("Argilla"))
            End If
            If IsValidProperty(objPar("Sabbia")) Then
                Plot.Horizon1Sand = CDec(objPar("Sabbia"))
            End If

            Dim IrriSysCode As Integer = CInt(objPar("Irri_Imp_Cod"))
            If IrriSysCode > 0 Then
                Plot.IrriSysCode = IrriSysCode
            End If

            Crop = New _crop With {
                .Id_CropType = CInt(objPar("Irri_Veg_Cod"))
            }

            If Not String.IsNullOrEmpty(objPar("DataFaseStart").ToString()) Then
                Crop.DataFaseStart = Convert.ToDateTime(objPar("DataFaseStart")).ToLocalTime
            End If
            If objPar("DataInizioImpianto") IsNot Nothing Then
                Crop.AnnoImpianto = CDate(objPar("DataInizioImpianto")).Year
            End If

            If objPar("ColturaProtetta") IsNot Nothing Then
                Crop.ColturaProtetta = CBool(objPar("ColturaProtetta"))
            End If


            If objPar("Id_GreenHouseType") IsNot Nothing Then
                Crop.Id_GreenHouseType = CInt(objPar("Id_GreenHouseType"))
            End If

            If objPar("GruppVegetale").ToString.ToLower.Trim = "arboree" Then

                Dim traFila As Decimal = CDec(objPar("TraFila"))
                If traFila > 0 Then
                    Crop.DistaTraFilaPiante = traFila
                End If

                Dim suFila As Decimal = CDec(objPar("SuFila"))
                If suFila > 0 Then
                    Crop.DistaSuFilaPiante = suFila
                End If

                Dim condTraFila As String = objPar("CondTraFila").ToString
                If Not String.IsNullOrEmpty(condTraFila) Then
                    If condTraFila.ToLower.Trim = "inerbito" Then
                        condTraFila = "I"
                    Else
                        condTraFila = "L"
                    End If
                    Crop.ConduInterfilare = condTraFila ' "I" 'Inerbito / Lavorato
                End If

                Dim ClasseVigore As Integer = CInt(objPar("Vigoria"))
                If ClasseVigore > 0 Then
                    Crop.ClasseVigore = ClasseVigore 'Debole / Medio / Vigoroso / Molto vigoroso
                End If
            End If



            Dim _irrigations As New JArray

            If Not String.IsNullOrEmpty(objPar("Irrigazioni").ToString()) Then

                _irrigations = JArray.Parse(objPar("Irrigazioni").ToString())
            End If

            IsIrriUser = 1

            If objPar("IrriChoice") IsNot Nothing Then

                Try

                    Dim irriChoice = CInt(objPar("IrriChoice"))

                    Select Case irriChoice

                        Case 1 'Irrigazioni_GIAS

                            Irrigations = New List(Of _irrigation)

                            For Each _irri As JObject In _irrigations

                                Irrigations.Add(New _irrigation(_irri("DataIrri").ToString, CDec(_irri("VolumeMM"))))
                            Next

                        Case 2 'Irrigazioni_IF

                            Irrigations = Nothing

                        Case 3 'Bilancio_IF

                            IsIrriUser = 0

                    End Select

                Catch ex As Exception

                End Try

            End If


            FertiRecipe = Nothing
            If objPar("FertiRecipe") IsNot Nothing Then
                FertiRecipe = objPar("FertiRecipe").ToObject(Of _FertiRecipe)()
            End If

            If objPar("FertIrriGList") IsNot Nothing Then
                FertIrriGList = objPar("FertIrriGList").ToObject(Of List(Of _FertiIrriG))
            End If


            'If Not persistent Then

            '    If _irrigations.Count > 0 Then

            '        Irrigations = New List(Of _irrigation)
            '    End If

            'Else

            '    If irriUser = 1 Then

            '        Irrigations = New List(Of _irrigation)
            '    End If

            'End If

            'If Irrigations IsNot Nothing Then

            '    For Each _irri As JObject In _irrigations

            '        Irrigations.Add(New _irrigation(_irri("DataIrri").ToString, CDec(_irri("VolumeMM"))))
            '    Next
            'End If
        End Sub

        Public Sub AddMeteoData(meteo As JObject)

            If meteo Is Nothing Then
                Return
            End If

            Dim meteoModel As JArray = meteo("model")

            Dim UmTerr As String = ""
            Dim map As New List(Of String)

            For Each elem As JObject In meteoModel

                Dim field = elem("field").ToString

                If field.StartsWith("UT_") Then

                    If String.IsNullOrEmpty(UmTerr) Then

                        UmTerr = field
                    End If
                Else

                    map.Add(field)
                End If
            Next

            Dim meteoData As JArray = meteo("data")

            If String.IsNullOrEmpty(UmTerr) Then

                MeteoGList = meteoData
            Else

                MeteoGList = New JArray
                SoilMoistures = New List(Of _soilMoisture)

                For Each md As JObject In meteoData

                    Dim md1 = New JObject
                    For Each m In map
                        md1.Add(m, md(m))
                    Next

                    If md(UmTerr) IsNot Nothing Then

                        Dim vUmTerr As Decimal = CDec(md(UmTerr))

                        md1.Add("UmTerr", vUmTerr)

                        If vUmTerr > 0 Then

                            SoilMoistures.Add(New _soilMoisture(CDate(md1("Date")), vUmTerr))
                        End If
                    End If

                    MeteoGList.Add(md1)
                Next
            End If

        End Sub

        Public Function Serialize() As String

            Dim jss As New JsonSerializerSettings() With {
                .NullValueHandling = NullValueHandling.Ignore,
                .DateFormatString = "yyyy-MM-dd"
            }

            Return JsonConvert.SerializeObject(Me, jss)
        End Function

    End Class








    Private Class StazioneIrriga

        Public ReadOnly TipoSorgente As Integer
        Public ReadOnly Sorgente As Integer

        Public Sub New(objPar As JObject)

            TipoSorgente = enum_Meteo_Tiposorgente.Pubbliche
            Sorgente = 0

            Dim strStazioni = objPar("Stazioni_Meteo").ToString()

            If CBool(objPar("UsaMeteo")) AndAlso Not String.IsNullOrEmpty(strStazioni) Then

                'Dati meteo proprietari

                Dim arrStaz = JArray.Parse(strStazioni)

                If arrStaz.Count > 0 Then

                    Dim objStaz = arrStaz(0)

                    TipoSorgente = objStaz("Sorgente")
                    Sorgente = objStaz("Stazione")
                End If

                Return
            End If

            'Dati meteo pubblici Hypermeteo

            If objPar("LatLng") Is Nothing OrElse objPar("LatLng").Type <> JTokenType.Object Then
                Return
            End If

            If objPar("LatLng")("Lat") Is Nothing OrElse objPar("LatLng")("Lng") Is Nothing Then
                Return
            End If

            Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
            Dim objParametri_SuperServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Super_Server")
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
                            New JProperty("lat", objPar("LatLng")("Lat")),
                            New JProperty("lng", objPar("LatLng")("Lng"))
                            )

            'Dim objMeteoNT As New AgronicaCoreWebService.MeteoNT
            Dim objMeteoSuite As New InterfacciaMeteoSuite(objParametri_Server, objParametri_SuperServer)

            'Dim json = JObject.Parse(objMeteoNT.LeggiStazioniXSorgente(objP, objParametri_Server))
            Dim json = JObject.Parse(objMeteoSuite.LeggiStazioniXSorgente(objP))

            Dim arrStazioni = JArray.Parse(json("RispostaStringa").ToString())

            If arrStazioni.Type <> JTokenType.Array Then
                Return
            End If

            If arrStazioni.Count = 0 Then
                Return
            End If

            Sorgente = arrStazioni(0)("id_stazione")
        End Sub

    End Class

End Class

