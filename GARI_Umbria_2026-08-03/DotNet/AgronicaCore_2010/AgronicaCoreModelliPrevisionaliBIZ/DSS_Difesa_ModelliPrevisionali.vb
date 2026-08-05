Imports System.Collections.Concurrent
Imports System.Threading.Tasks
Imports System.Web
Imports System.Web.Script.Serialization
Imports System.Xml.Serialization
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreMetaSchemaDAL
Imports AgronicaCoreMeteoBiz
Imports AgronicaCoreModelliPrevisionaliBIZ.DSS_Difesa_ModelliPrevisionali.OutputRisultatoIndicatori
Imports AgronicaCoreModelliPrevisionaliBIZ.RisultatoElaborazioneIndicatori.Indicatore
Imports AgronicaCoreModelliPrevisionaliDAL
Imports AgronicaCoreUtentiDAL
Imports AgronicaCoreUtility
Imports InData.Engine.DSSDifesa
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports OutData.Engine.DSSDifesa

Public Class DSS_Difesa_ModelliPrevisionali

    Private Class ModelloCacheInfo
        Public ID As Integer
        Public Code As String
        Public Description As String
    End Class

    Private _objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri
    Private _objParametri_Super_Server As AgronicaCoreDataProvider.AgronicaCoreParametri

    Private _interfacciaMeteoSuite As InterfacciaMeteoSuite

    Private _VegModCachedMap As Dictionary(Of Integer, List(Of ModelloCacheInfo))
    Private _URL_NetCoreDataExchange_API As String
    Private _AccessToken As String
    Private _CallNetCoreUtility As CallNetCore

    Public Sub New(objParametriServer As AgronicaCoreParametri, objParametriSuperServer As AgronicaCoreParametri)

        _objParametri_Server = objParametriServer
        _objParametri_Super_Server = objParametriSuperServer
    End Sub


    Public Function LeggiModelliAutorizzati(ByVal objParams As JObject) As String

        If Not ControllaUtilizzoNuovoEngineDSSDifesa() Then
            Dim objMeteo As New AgronicaCoreWebService.MeteoNT
            Return objMeteo.LeggiModelliAutorizzati(objParams, _objParametri_Server)
        End If

        Dim PIVA As String = objParams("PIVA")
        Dim Veg_Cod As Integer = objParams("Veg_Cod")

        Dim objModelli As New DSS_ModelliPrevisionaliAutorizzati_R
        Dim dtModelli = objModelli.LeggiAutorizzati(PIVA, Veg_Cod, _objParametri_Server)

        LeggiDescrizioneModelli(dtModelli)

        Dim jss = New JavaScriptSerializer With {
                .MaxJsonLength = Integer.MaxValue
            }
        Dim objRisposta As New With {.RispostaStringa = JsonConvert.SerializeObject(dtModelli, New JsonSerializerSettings With {.NullValueHandling = NullValueHandling.Ignore}).ToString()}
        Return jss.Serialize(objRisposta)

    End Function

    Public Function LeggiTuttiModelliAutorizzati(ByVal objParams As JObject) As String

        If Not ControllaUtilizzoNuovoEngineDSSDifesa() Then
            Dim objMeteo As New AgronicaCoreWebService.MeteoNT
            Return objMeteo.LeggiTuttiModelliAutorizzati(objParams, _objParametri_Server)
        End If

        Dim objModelli As New DSS_ModelliPrevisionaliAutorizzati_R
        Dim dtModelli = objModelli.LeggiTutti(_objParametri_Server)

        LeggiDescrizioneModelli(dtModelli)

        Dim jss = New JavaScriptSerializer With {
                .MaxJsonLength = Integer.MaxValue
            }
        Dim objRisposta As New With {.RispostaStringa = dtModelli}
        Return jss.Serialize(objRisposta)

    End Function

    Private Sub LeggiDescrizioneModelli(ByRef dtModelli As DataTable)

        If dtModelli.Rows.Count > 0 Then

            Dim dictVegModelIDs As New Dictionary(Of Integer, List(Of Integer))

            For i = 0 To dtModelli.Rows.Count - 1
                Dim drModello = dtModelli.Rows(i)
                Dim Veg_Cod As Integer = drModello("Veg_Cod")

                If Not dictVegModelIDs.ContainsKey(Veg_Cod) Then
                    dictVegModelIDs.Add(Veg_Cod, New List(Of Integer))
                End If

                Dim Mod_Id As Integer = drModello("Mod_Cod")
                dictVegModelIDs(Veg_Cod).Add(Mod_Id)
            Next

            Dim Veg_Codes As List(Of Integer) = dictVegModelIDs.Keys.ToList()

            For Each Veg_Cod In Veg_Codes
                Dim Mod_IDs = dictVegModelIDs(Veg_Cod)

                For Each Mod_Id In Mod_IDs
                    Dim drModello = dtModelli.Select("Veg_Cod = " + Veg_Cod.ToString + " and Mod_Cod = " + Mod_Id.ToString).FirstOrDefault()

                    If Not drModello Is Nothing Then
                        Dim Mod_Des As String = GetModelDescriptionById(Veg_Cod, Mod_Id)
                        drModello("Mod_Des") = If(Not String.IsNullOrEmpty(Mod_Des), Mod_Des, "")
                    End If
                Next
            Next

        End If
    End Sub

    Public Function CompletaOutputModelli(ByVal objParams As JObject) As String

        If Not ControllaUtilizzoNuovoEngineDSSDifesa() Then
            Dim objMeteo As New AgronicaCoreWebService.MeteoNT
            Return objMeteo.CompletaOutputModelli(objParams, _objParametri_Server)
        End If

        Dim PIVA_Superuser As String = objParams("PIVA_Superuser")
        Dim PIVA As String = objParams("PIVA")
        Dim Modelli As JArray = objParams("Modelli")


        Dim objModelli As New DSS_ModelliPrevisionaliAutorizzati_R
        Dim dtModelli = objModelli.LeggiAutorizzati(PIVA, 0, _objParametri_Server)

        Dim out_modelli As New JArray

        If Not dtModelli Is Nothing AndAlso dtModelli.Rows.Count > 0 AndAlso Not Modelli Is Nothing Then

            For Each in_m As JObject In Modelli

                Dim mod_id As Integer = in_m("mod_cod")
                Dim veg_cod As Integer = in_m("veg_cod")
                'Dim avv_cod As Integer = in_m("avv_cod")
                'Dim alg_cod As Integer = in_m("alg_cod")

                'Dim rows = dtModelli.Select("Mod_Cod = " & mod_id & " AND Veg_Cod = " & veg_cod & " AND Av_Cod = " & avv_cod & " AND Alg_Cod = " & alg_cod)
                Dim rows = dtModelli.Select("Mod_Cod = " & mod_id.ToString & " AND Veg_Cod = " & veg_cod.ToString)

                If rows.Any Then
                    Dim out_m As JObject = in_m.DeepClone()

                    Dim Mod_Des As String = rows(0)("Mod_Des").ToString
                    If String.IsNullOrEmpty(Mod_Des) Then
                        Mod_Des = GetModelDescriptionById(veg_cod, mod_id)
                        If String.IsNullOrEmpty(Mod_Des) Then
                            Mod_Des = ""
                        End If
                    End If

                    out_m.Add("veg_des", rows(0)("Veg_Des").ToString)
                    out_m.Add("mod_des", Mod_Des)
                    out_m.Add("mod_des_agg", rows(0)("Mod_Des_Agg").ToString)
                    out_m.Add("alg_des", rows(0)("Alg_Des").ToString)
                    out_m.Add("avv_des", rows(0)("Av_Des_Lat").ToString)
                    out_m.Add("param_des", "")

                    out_modelli.Add(out_m)
                End If

            Next

        End If

        Dim jss = New JavaScriptSerializer With {
                .MaxJsonLength = Integer.MaxValue
            }
        Dim objRisposta As New With {.RispostaStringa = out_modelli.ToString}
        Return jss.Serialize(objRisposta)

    End Function


    Public Function ModelliPrevisionaliElabora(ByVal objParams As JObject) As String

        If Not ControllaUtilizzoNuovoEngineDSSDifesa() Then
            Dim objMeteo As New AgronicaCoreWebService.MeteoNT
            Return objMeteo.ModelliPrevisionaliElabora(objParams, _objParametri_Server)
        End If

        'Dim response = ChiamataDSSDifesa_CalcoloRischio(objParams, 1)

        Dim Tipo_Sorgente = CInt(objParams("TipoSorgente"))
        Dim Station_Id = CInt(objParams("Sorgente"))
        Dim Mod_Id = CInt(objParams("Mod_Cod"))
        Dim Veg_Cod = CInt(objParams("Veg_Cod"))
        Dim DataInizio = Convert.ToDateTime(objParams("DataInizio").ToString)
        Dim DataFine = Convert.ToDateTime(objParams("DataFine").ToString)

        Dim Coordinates As CalcoloRischioDSSDifesaRequest.GeoCoordinates = Nothing
        Dim Mod_Code As String = GetModelCodeById(Veg_Cod, Mod_Id)
        Dim ModelliCodici As New List(Of String)() From {Mod_Code}

        'If Not objParams("Mod_Cod") Is Nothing Then

        '    Dim Mod_Id = CInt(objParams("Mod_Cod"))
        '    Dim Mod_Code = GetModelCodeById(Veg_Cod, Mod_Id)
        '    ModelliCodici.Add(Mod_Code)

        'ElseIf Not objParams("Modelli") Is Nothing Then

        '    Dim Modelli As JArray = objParams("Modelli")

        '    For Each in_m As JObject In Modelli
        '        Dim Mod_Id = CInt(in_m("mod_cod"))
        '        Dim Mod_Code = GetModelCodeById(Veg_Cod, Mod_Id)
        '        ModelliCodici.Add(Mod_Code)
        '    Next
        'End If

        Dim response = ChiamataDSSDifesa_CalcoloRischio(Veg_Cod,
                                                Tipo_Sorgente, Station_Id, Coordinates,
                                                DataInizio, DataFine,
                                                ModelliCodici, 1)



        Dim Modello_Titolo As String = ""
        Dim Modello_Descrizione As String = ""
        Dim Modello_Tabella1 As String = Nothing
        Dim Modello_Tabella2 As String = Nothing

        If response.Status.ToLower() = "ok" AndAlso response.Result.RisultatiModelli.Count > 0 Then

            Dim risultatoModello = response.Result.RisultatiModelli(0)
            'Modello_Titolo = risultatoModello.ModelGroup
            'Modello_Descrizione = risultatoModello.ModelDescription
            Modello_Titolo = risultatoModello.ModelDescription
            If Not risultatoModello.OutputAnalitico Is Nothing AndAlso Not risultatoModello.OutputAnalitico.Modello_Tabella1 Is Nothing Then
                Modello_Tabella1 = risultatoModello.OutputAnalitico.Modello_Tabella1.
                    Replace("rows", "kendo_rows").
                    Replace("models", "kendo_model").
                    Replace("columns", "kendo_columns")
            End If

        End If

        Dim risultato As New cRisultatoModello With {
            .Modello_Titolo = Modello_Titolo,
            .Modello_Descrizione = Modello_Descrizione,
            .Modello_Tabella1 = Modello_Tabella1,
            .Modello_Tabella2 = Modello_Tabella2,
            .Modello_WarningMsg = Nothing,
            .Modello_Disclaimer = Nothing,
            .Modello_TabellaMeteo = Nothing,
            .Modello_EngineId = 1,
            .Modello_CodiceEsterno = Mod_Code
            }

        Dim jss = New JavaScriptSerializer With {
                .MaxJsonLength = Integer.MaxValue
            }
        Dim objRisposta As New With {.RispostaStringa = risultato}
        Return jss.Serialize(objRisposta)

    End Function

    Public Function ModelliPrevisionaliElabora_Indicatore(objParams As JObject) As String

        If Not ControllaUtilizzoNuovoEngineDSSDifesa() Then
            Dim objMeteo As New AgronicaCoreWebService.MeteoNT
            Return objMeteo.ModelliPrevisionaliElabora_Indicatore(objParams, _objParametri_Server)
        End If

        'Prelevo il parametro della lingua
        Dim linguaCodiceISO As String = "it"
        If objParams("LinguaCodiceISO") IsNot Nothing Then
            linguaCodiceISO = objParams("LinguaCodiceISO").ToString()
        End If
        'imposto la lingua nel thread corrente per ottenere le voci tradotte dai file di risorse
        Threading.Thread.CurrentThread.CurrentUICulture = New System.Globalization.CultureInfo(linguaCodiceISO)

        Dim Veg_Cod = CInt(objParams("Veg_Cod"))
        Dim Mod_Id = CInt(objParams("Mod_Cod"))
        Dim DataInizio = Convert.ToDateTime(objParams("DataInizio").ToString)
        Dim DataFine = Convert.ToDateTime(objParams("DataFine").ToString)
        Dim ElencoSorgenti = JArray.FromObject(objParams("SorgentiMeteo"))

        Dim Mod_Code As String = GetModelCodeById(Veg_Cod, Mod_Id)

        Dim objParam1 As JObject = objParams.DeepClone()
        objParam1.Add("Sorgente")

        Dim elabList As New ConcurrentBag(Of IndicatoreXSorgente)

        Parallel.ForEach(ElencoSorgenti, New ParallelOptions With {.MaxDegreeOfParallelism = 10},
                         Sub(pcm)
                             Dim tipo As Integer = pcm("Tipo")
                             Dim stazione As Integer = pcm("Stazione")

                             ElaboraRisultatoIndicatoreXSorgente(pcm, Veg_Cod, Mod_Code, DataInizio, DataFine, elabList)
                         End Sub)

        Dim resultList As List(Of IndicatoreXSorgente) = elabList.ToList()

        Dim jss = New JavaScriptSerializer With {
                .MaxJsonLength = Integer.MaxValue
            }
        Dim objRisposta As New With {.RispostaStringa = resultList}
        Return jss.Serialize(objRisposta)

    End Function

    Private Sub ElaboraRisultatoIndicatoreXSorgente(ByRef pcm As JToken,
                                        ByVal Veg_Cod As Integer, ByVal Mod_Code As String,
                                        ByVal DataInizio As Date, ByVal DataFine As Date,
                                        ByRef elabList As ICollection(Of IndicatoreXSorgente))

        Dim tipo As Integer = CInt(pcm("Tipo"))
        Dim stazione As Integer = CInt(pcm("Stazione"))

        Dim IndXSorg As New IndicatoreXSorgente
        IndXSorg.TipoSorgente = tipo
        IndXSorg.StazioneSorgente = stazione

        If String.IsNullOrEmpty(Mod_Code) Then
            IndXSorg.RisultatoElaborazione = New RisultatoElaborazione
            IndXSorg.RisultatoElaborazione.Status = RisultatoElaborazione.Stato_Elaborazione._Error
            IndXSorg.RisultatoElaborazione.StatusMsg = "Modello non valido"

        Else
            Dim response = ChiamataDSSDifesa_CalcoloRischio(Veg_Cod,
                                                        tipo, stazione, Nothing,
                                                        DataInizio, DataFine,
                                                        New List(Of String)() From {Mod_Code}, 0)
            Dim RisultatoModello = response.Result.RisultatiModelli(0)

            IndXSorg.RisultatoElaborazione = Map_RisultatoModello_To_RisultatoElaborazioneIndicatori(RisultatoModello)
            IndXSorg.RisultatoElaborazione.DataInizio = DataInizio
            IndXSorg.RisultatoElaborazione.DataFine = DataFine

        End If

        elabList.Add(IndXSorg)
    End Sub

    'Public Function ModelliPrevisionaliElaboraIndicatori(ByVal objParams As JObject, ByVal objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri) As String

    '    If Not DSSDifesa_CheckUseNewEngine(objParametri_Server) Then
    '        Dim objMeteo As New AgronicaCoreWebService.MeteoNT
    '        Return objMeteo.ModelliPrevisionaliElaboraIndicatori(objParams, objParametri_Server)
    '    End If

    '    Dim risElaborazioneIndicatori As New RisultatoElaborazioneIndicatori

    '    Dim ElencoModelli = objParams("ElencoModelli")

    '    If Not ElencoModelli Is Nothing AndAlso ElencoModelli.Count > 0 Then
    '        For Each mdl In ElencoModelli



    '        Next
    '    End If

    '    Dim RispostaStringa = JsonConvert.SerializeObject(risElaborazioneIndicatori, New JsonSerializerSettings With {.NullValueHandling = NullValueHandling.Ignore}).ToString()

    '    Dim jss = New JavaScriptSerializer With {
    '            .MaxJsonLength = Integer.MaxValue
    '        }
    '    Dim objRisposta As New With {.RispostaStringa = RispostaStringa}
    '    Return jss.Serialize(objRisposta)

    'End Function

    'Public Function ModelliPrevisionaliElaboraIndicatoriV2(ByVal objParams As JObject, ByVal objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri) As String

    '    If Not DSSDifesa_CheckUseNewEngine(objParametri_Server) Then
    '        Dim objMeteo As New AgronicaCoreWebService.MeteoNT
    '        Return objMeteo.ModelliPrevisionaliElaboraIndicatoriV2(objParams, objParametri_Server)
    '    End If


    'End Function

    Public Class OutputRisultatoIndicatori
        Public Class OutputIndicatore

            Public ChiaveRichiesta As Integer
            Public Parametri As RisultatoElaborazioneIndicatori.Indicatore.ParametriElaborazione
            Public Risultato As RisultatoElaborazioneIndicatori.Indicatore.RisultatoElaborazione
            Public Stazione As String
            Public Modello As String
            Public Specie As String
            Public Avversita As String
            Public DescrParametri As String
            Public AuxData As Object

            Public Sub New()

            End Sub
            Public Sub New(ByVal indic As RisultatoElaborazioneIndicatori.Indicatore)
                Parametri = indic.Parametri
                Risultato = indic.Risultato
            End Sub
        End Class

        <XmlElement([Namespace]:="http://www.outputrisultatoindicatori.com")>
        Public Stato As RisultatoElaborazioneIndicatori.Stato_Elaborazione

        Public Indicatori As List(Of OutputIndicatore)

    End Class

    Public Function ModelliPrevisionaliElaboraIndicatoriV3(ByVal objParams As JObject) As String

        If Not ControllaUtilizzoNuovoEngineDSSDifesa() Then
            Dim objMeteo As New AgronicaCoreWebService.MeteoNT
            Return objMeteo.ModelliPrevisionaliElaboraIndicatoriV3(objParams, _objParametri_Server)
        End If

        Dim outputIndicatori As New OutputRisultatoIndicatori
        outputIndicatori.Stato = RisultatoElaborazioneIndicatori.Stato_Elaborazione.InAttesa

        Dim Request As JObject = objParams("Request")
        Dim ElencoModelli As JArray = Request("ElencoModelli")
        Dim ParametriExtra As JObject = Request("ParametriExtra")
        Dim Forecast_gg As Integer = Request("Forecast_gg")

        Dim elabList As New ConcurrentBag(Of OutputRisultatoIndicatori.OutputIndicatore)

        Dim dictSV As New Dictionary(Of Integer, String)

        'Parallel.ForEach(ElencoModelli, New ParallelOptions With {.MaxDegreeOfParallelism = 10},
        '                 Sub(mdl)
        '                     ElaboraOutputIndicatore(mdl, ParametriExtra, Forecast_gg, dictSV, newDSSDifesaUrl, bearerToken, elabList)
        '                 End Sub)

        For Each mdl In ElencoModelli
            ElaboraOutputRisultatoIndicatore(mdl, ParametriExtra, Forecast_gg, dictSV, elabList)
        Next

        outputIndicatori.Indicatori = elabList.ToList()
        outputIndicatori.Stato = RisultatoElaborazioneIndicatori.Stato_Elaborazione.Pronti

        Dim jss = New JavaScriptSerializer With {
                .MaxJsonLength = Integer.MaxValue
            }
        Dim objRisposta As New With {.RispostaStringa = outputIndicatori}
        Return jss.Serialize(objRisposta)

    End Function

    Private Sub ElaboraOutputRisultatoIndicatore(ByRef mdl As JToken, ByRef ParametriExtra As JObject, ByVal Forecast_gg As Integer,
                                                 ByRef dictSV As Dictionary(Of Integer, String),
                                                 ByRef elabList As ConcurrentBag(Of OutputRisultatoIndicatori.OutputIndicatore))

        'Campi serializzati per AgronicaCoreModelliPrevisionaliCommon.ModelloConfigurato
        Dim ChiaveRichiesta As Integer = CInt(mdl("ChiaveRichiesta"))
        Dim Tipo_Sorgente As Integer = CInt(mdl("Tipo_Sorgente"))
        Dim Stazione_Cod As Integer = CInt(mdl("Stazione_Cod"))
        Dim Mod_Id As Integer = CInt(mdl("Mod_Cod"))
        Dim Veg_Cod As Integer = CInt(mdl("Veg_Cod"))
        Dim Avv_Cod As Integer = CInt(mdl("Avv_Cod"))
        Dim Alg_Cod As Integer = CInt(mdl("Alg_Cod"))
        Dim ParametriElaborazione As JObject = mdl("ParametriElaborazione")
        Dim InizioPeriodo_gg As Integer = mdl("InizioPeriodo_gg")
        Dim FinePeriodo_gg As Integer = mdl("FinePeriodo_gg")
        Dim Validita_minuti As Integer = mdl("Validita_minuti")
        Dim DatiMeteoInizio_gg As Integer = mdl("DatiMeteoInizio_gg")
        Dim DatiMeteoFine_gg As Integer = mdl("DatiMeteoFine_gg")

        'Dim Mod_Des As String
        'Dim Mod_Id As Integer
        'If Integer.TryParse(Mod_Cod, Mod_Id) Then
        '    Dim Mod_Info = GetModelloInfoById(Veg_Cod, Mod_Id, objParametri_Server)
        '    Mod_Cod = Mod_Info.Code
        '    Mod_Des = Mod_Info.Description
        'Else
        '    Dim Mod_Info = GetModelloInfoByCod(Veg_Cod, Mod_Cod, objParametri_Server)
        '    Mod_Id = Mod_Info.ID
        '    Mod_Des = Mod_Info.Code
        'End If
        Dim Mod_Info = GetModelloInfoById(Veg_Cod, Mod_Id)
        Dim Mod_Code = If(Not Mod_Info Is Nothing AndAlso Not Mod_Info.Code Is Nothing, Mod_Info.Code, "")
        Dim Mod_Des = If(Not Mod_Info Is Nothing AndAlso Not Mod_Info.Description Is Nothing, Mod_Info.Description, "")

        If Not dictSV.ContainsKey(Veg_Cod) Then
            Dim objSV_R As New SpecieVegetali_R
            Dim dtSV = objSV_R.Leggi(Veg_Cod, 0, "", "", AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", _objParametri_Server)
            If dtSV.Rows.Count > 0 Then
                dictSV(Veg_Cod) = dtSV(0).Item("Veg_Des")
            End If
        End If

        Dim startDate = New Date(Date.Now().Year, 1, 1)
        Dim endDate = Date.Now().Date
        Dim currentYear As Integer = Date.Now().Year

        If ParametriExtra IsNot Nothing Then
            startDate = Convert.ToDateTime(ParametriExtra("DataInizio").ToString).ToLocalTime
            endDate = Convert.ToDateTime(ParametriExtra("DataFine").ToString).ToLocalTime
            Forecast_gg = 0
        End If

        ' if no start date was passed use model's start day
        If IsNothing(ParametriExtra) Then
            startDate = New Date(currentYear, 1, 1).AddDays(DatiMeteoInizio_gg - 1)
            If startDate > Date.Now.Date Then
                startDate = startDate.AddYears(-1)
            End If
        ElseIf startDate > endDate Then
            startDate = startDate.AddYears(-1)
        End If

        ' if no end date was passed use model's end day
        If IsNothing(ParametriExtra) Then
            Dim span As Integer = DatiMeteoFine_gg - DatiMeteoInizio_gg + 1
            If span < 0 Then
                Dim ydd As Integer = If(Date.IsLeapYear(startDate.Year), 366, 365)
                span = ydd - DatiMeteoFine_gg + DatiMeteoInizio_gg
            End If

            endDate = startDate.AddDays(span)
            If endDate > Date.Now.Date Then
                endDate = Date.Now.Date
            End If
        End If

        Dim flag_validita_gg As Boolean = True

        If IsNothing(ParametriExtra) Then
            Dim mdlStartDoy As Integer = InizioPeriodo_gg
            Dim mdlEndDoy As Integer = FinePeriodo_gg
            Dim endDoy As Integer = endDate.DayOfYear

            If mdlStartDoy < mdlEndDoy Then
                flag_validita_gg = mdlStartDoy <= endDoy And endDoy <= mdlEndDoy
            Else
                'Periodo a cavallo dell'anno
                flag_validita_gg = mdlStartDoy <= endDoy Or endDoy <= mdlEndDoy
            End If
        End If

        Dim outputIndicatore As New OutputIndicatore
        outputIndicatore.ChiaveRichiesta = ChiaveRichiesta
        outputIndicatore.Parametri = New ParametriElaborazione With {
            .Tipo_Sorgente = Tipo_Sorgente,
            .Stazione_Cod = Stazione_Cod,
            .Veg_Cod = Veg_Cod,
            .Mod_Cod = Mod_Id,
            .Avv_Cod = Avv_Cod,
            .Alg_Cod = Alg_Cod,
            .ParametriElaborazione = ParametriElaborazione.ToString()
        }
        outputIndicatore.Stazione = Stazione_Cod
        outputIndicatore.Modello = If(Not String.IsNullOrEmpty(Mod_Des), Mod_Des, "")
        outputIndicatore.Specie = dictSV(Veg_Cod)
        outputIndicatore.Avversita = ""
        outputIndicatore.DescrParametri = ""
        outputIndicatore.AuxData = ""

        If String.IsNullOrEmpty(Mod_Code) Then
            outputIndicatore.Risultato = New RisultatoElaborazioneIndicatori.Indicatore.RisultatoElaborazione
            outputIndicatore.Risultato.Status = RisultatoElaborazione.Stato_Elaborazione._Error
            outputIndicatore.Risultato.StatusMsg = "Modello non valido"

        ElseIf flag_validita_gg Then

            Dim response = ChiamataDSSDifesa_CalcoloRischio(Veg_Cod,
                                                            Tipo_Sorgente, Stazione_Cod, Nothing,
                                                            startDate, endDate,
                                                            New List(Of String)() From {Mod_Code}, 0)

            If response.Status.ToLower() = "ok" AndAlso response.Result.RisultatiModelli.Count > 0 Then

                Dim RisultatoModello = response.Result.RisultatiModelli(0)
                outputIndicatore.Risultato = Map_RisultatoModello_To_RisultatoElaborazioneIndicatori(RisultatoModello)
                outputIndicatore.Risultato.DataInizio = startDate
                outputIndicatore.Risultato.DataFine = endDate

                If Not RisultatoModello.OutputSintetico Is Nothing Then
                    outputIndicatore.Avversita = RisultatoModello.OutputSintetico.Disease
                    outputIndicatore.AuxData = RisultatoModello.OutputSintetico.WarningMessage
                End If
            Else
                outputIndicatore.Risultato = New RisultatoElaborazioneIndicatori.Indicatore.RisultatoElaborazione
                outputIndicatore.Risultato.Status = RisultatoElaborazione.Stato_Elaborazione._Error
            End If
        Else
            outputIndicatore.Risultato = New RisultatoElaborazioneIndicatori.Indicatore.RisultatoElaborazione
            outputIndicatore.Risultato.Status = RisultatoElaborazione.Stato_Elaborazione._OutOfRange
        End If

        elabList.Add(outputIndicatore)

    End Sub


    Private Function Map_RisultatoModello_To_RisultatoElaborazioneIndicatori(ByVal RisultatoModello As RisultatoModello) As RisultatoElaborazioneIndicatori.Indicatore.RisultatoElaborazione

        Dim RisultatoElaborazione = New RisultatoElaborazioneIndicatori.Indicatore.RisultatoElaborazione

        If RisultatoModello.OutputSintetico Is Nothing Then
            RisultatoElaborazione.Status = RisultatoElaborazione.Stato_Elaborazione._Error
            RisultatoElaborazione.StatusMsg = RisultatoModello.MessaggioErrore
        End If

        Dim outputStatus As Integer = RisultatoModello.OutputSintetico.Status

        If outputStatus = 0 And Not RisultatoModello.OutputSintetico.Bands Is Nothing Then
            Dim bands As New List(Of RisultatoElaborazioneIndicatori.Indicatore.RisultatoElaborazione.Band)
            Dim RisultatoBands = RisultatoModello.OutputSintetico.Bands
            If Not RisultatoModello.OutputSintetico.Bands Is Nothing Then
                If Not RisultatoBands(0) Is Nothing Then
                    bands.Add(New RisultatoElaborazioneIndicatori.Indicatore.RisultatoElaborazione.Band With {.Value = RisultatoBands(0).Max, .Color = RisultatoBands(0).Color})
                End If
                If Not RisultatoBands(1) Is Nothing Then
                    bands.Add(New RisultatoElaborazioneIndicatori.Indicatore.RisultatoElaborazione.Band With {.Value = RisultatoBands(1).Max, .Color = RisultatoBands(1).Color})
                End If
            End If

            RisultatoElaborazione = New RisultatoElaborazioneIndicatori.Indicatore.RisultatoElaborazione
            RisultatoElaborazione.Scale_Min = 0
            RisultatoElaborazione.Scale_Max = 100
            RisultatoElaborazione.Bands = bands
            RisultatoElaborazione.Value = RisultatoModello.OutputSintetico.RiskIndex

            RisultatoElaborazione.Status = RisultatoElaborazione.Stato_Elaborazione._Valid
            RisultatoElaborazione.StatusMsg = RisultatoModello.OutputSintetico.StatusVerbose
            RisultatoElaborazione.AuxMsg = RisultatoModello.OutputSintetico.WarningMessage
        Else
            RisultatoElaborazione.Status = RisultatoElaborazione.Stato_Elaborazione._Error
            RisultatoElaborazione.StatusMsg = RisultatoModello.MessaggioErrore
        End If

        Return RisultatoElaborazione

    End Function


    Private Function ChiamataDSSDifesa_LeggiModelli(ByVal objParams As JObject) As LeggiModelliDSSDifesaResponse

        Dim Veg_Cod As Integer = objParams("Veg_Cod")
        Dim Modelli As JArray = objParams("Modelli")
        Dim ModelliCodici As New List(Of String)

        If Not Modelli Is Nothing Then
            For Each in_m As JObject In Modelli
                Dim Mod_Id As Integer = CInt(in_m("mod_cod"))
                Dim Mod_Code = GetModelCodeById(Veg_Cod, Mod_Id)

                ModelliCodici.Add(Mod_Code)
            Next
        End If

        Dim request As New LeggiModelliDSSDifesaRequest
        request.CropCode = Veg_Cod
        request.VarCode = ""
        request.ModelliCodici = ModelliCodici

        Dim payload = JsonConvert.SerializeObject(request)

        Return ChiamataDSSDifesa_LeggiModelli(Veg_Cod, ModelliCodici)

    End Function

    Private Function ChiamataDSSDifesa_LeggiModelli(ByVal Veg_Cod As Integer, ByRef ModelliCodici As List(Of String)) As LeggiModelliDSSDifesaResponse

        Dim request As New LeggiModelliDSSDifesaRequest
        request.CropCode = Veg_Cod
        request.VarCode = ""
        request.ModelliCodici = ModelliCodici

        Dim payload = JsonConvert.SerializeObject(request)
        Dim response = ChiamataNetCoreDataExchangePost("LeggiModelliDifesa", payload)

        Return JsonConvert.DeserializeObject(Of LeggiModelliDSSDifesaResponse)(response)

    End Function

    Private Function ChiamataDSSDifesa_CalcoloRischio(ByVal objParams As JObject, ByVal livelloDettaglio As Integer) As CalcoloRischioDSSDifesaResponse

        Dim Tipo_Sorgente = CInt(objParams("TipoSorgente"))
        Dim Station_Id = CInt(objParams("Sorgente"))
        Dim Coordinates As CalcoloRischioDSSDifesaRequest.GeoCoordinates = Nothing
        Dim DataInizio = Convert.ToDateTime(objParams("DataInizio").ToString)
        Dim DataFine = Convert.ToDateTime(objParams("DataFine").ToString)
        Dim Veg_Cod = CInt(objParams("Veg_Cod"))
        Dim ModelliCodici As New List(Of String)

        If Not objParams("Mod_Cod") Is Nothing Then

            Dim Mod_Id = CInt(objParams("Mod_Cod"))
            Dim Mod_Code = GetModelCodeById(Veg_Cod, Mod_Id)
            ModelliCodici.Add(Mod_Code)

        ElseIf Not objParams("Modelli") Is Nothing Then

            Dim Modelli As JArray = objParams("Modelli")

            For Each in_m As JObject In Modelli
                Dim Mod_Id = CInt(in_m("mod_cod"))
                Dim Mod_Code = GetModelCodeById(Veg_Cod, Mod_Id)
                ModelliCodici.Add(Mod_Code)
            Next
        End If

        Return ChiamataDSSDifesa_CalcoloRischio(Veg_Cod,
                                                Tipo_Sorgente, Station_Id, Coordinates,
                                                DataInizio, DataFine,
                                                ModelliCodici,
                                                livelloDettaglio)

    End Function

    Private Function ChiamataDSSDifesa_CalcoloRischio(ByVal Veg_Cod As Integer,
                                                      ByVal Tipo_Sorgente As Integer, ByVal Station_Id As Integer,
                                                      ByVal Coordinates As CalcoloRischioDSSDifesaRequest.GeoCoordinates,
                                                      ByVal DataInizio As Date, ByVal DataFine As Date,
                                                      ByRef ModelliCodici As List(Of String),
                                                      ByVal LivelloDettaglio As Integer) As CalcoloRischioDSSDifesaResponse

        'VerificaCodiceStazione(Tipo_Sorgente, Station_Cod, Coordinates, objParametri_Server)

        Dim Station_Cod As String = ""

        If ControllaUtilizzoNuovoEngineMeteoSuite() Then

            If Not Tipo_Sorgente = enum_Meteo_Tiposorgente.Gias_RER And
                Not Tipo_Sorgente = enum_Meteo_Tiposorgente.Gias_RER_Quadranti Then

                Station_Cod = GetStationCodeById(Station_Id)

            ElseIf Coordinates Is Nothing Then

                Coordinates = GetCoordinateStazioneMeteoSuite(Station_Id)

            End If

        ElseIf Coordinates Is Nothing Then

            Coordinates = GetCoordinateStazioneNT(Tipo_Sorgente, Station_Id)

        End If

        Dim request As New CalcoloRischioDSSDifesaRequest
        request.StationCod = Station_Cod
        request.Coordinates = Coordinates
        request.DataInizio = DataInizio.ToString("o")
        request.DataFine = DataFine.ToString("o")
        request.CropCode = Veg_Cod
        request.VarCode = Nothing
        request.ModelliCodici = ModelliCodici
        request.LivelloDettaglio = LivelloDettaglio

        Dim payload = JsonConvert.SerializeObject(request)
        Dim response = ChiamataNetCoreDataExchangePost("CalcoloRischioPerModelli", payload)

        Return JsonConvert.DeserializeObject(Of CalcoloRischioDSSDifesaResponse)(response)

    End Function


    Private Function ChiamataNetCoreDataExchangePost(endpoint As String, payLoad As String) As Object

        If String.IsNullOrEmpty(_URL_NetCoreDataExchange_API) Then
            _URL_NetCoreDataExchange_API = LeggiUrlNetCoreDataExchangeAPI()
        End If

        Dim urlCompleto As String = _URL_NetCoreDataExchange_API +
            If(_URL_NetCoreDataExchange_API.EndsWith("/"), "DSSDifesa/", "/DSSDifesa/") + endpoint

        Dim content As New System.Net.Http.StringContent(payLoad, Text.Encoding.UTF8, "application/json")

        'If String.IsNullOrEmpty(_BearerToken) Then
        '    _BearerToken = GetBearerToken()
        'End If

        'If String.IsNullOrEmpty(_BearerToken) Then
        '    Dim objParametri_Super_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Super_Server")
        '    Dim objUtentiToken As New Utenti_Token_R
        '    Dim accessToken = objUtentiToken.Leggi_Token("", objParametri_Super_Server)

        '    Dim callNetCore As New CallNetCore()
        '    Return callNetCore.CallNetCorePost(urlCompleto, accessToken, payLoad, objParametri_Server)
        'End If

        'Dim client As New HttpClient()
        'client.DefaultRequestHeaders.Add("Authorization", "Bearer " + _BearerToken)

        'Dim hr As System.Net.Http.HttpResponseMessage = client.PostAsync(urlCompleto, content).Result
        'hr.EnsureSuccessStatusCode()
        'Dim response = hr.Content.ReadAsStringAsync().Result

        'Return response

        If _AccessToken Is Nothing Then
            'Dim objParametri_Super_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Super_Server")

            Dim objParametri_Super_Server As AgronicaCoreParametri = _objParametri_Super_Server
            If String.IsNullOrEmpty(objParametri_Super_Server.SuperUserUsername) Or
                String.IsNullOrEmpty(objParametri_Super_Server.UtenteUsername) Then

                objParametri_Super_Server = objParametri_Super_Server.CreateDeepCopy(_objParametri_Super_Server)
                If String.IsNullOrEmpty(objParametri_Super_Server.SuperUserUsername) Then
                    objParametri_Super_Server.SuperUserUsername = _objParametri_Server.SuperUserUsername
                End If
                If String.IsNullOrEmpty(objParametri_Super_Server.UtenteUsername) Then
                    objParametri_Super_Server.UtenteUsername = _objParametri_Server.UtenteUsername
                End If
            End If

            Dim objUtentiToken As New Utenti_Token_R
            _AccessToken = objUtentiToken.Leggi_Token("", objParametri_Super_Server)
        End If

        If _CallNetCoreUtility Is Nothing Then
            _CallNetCoreUtility = New CallNetCore()
        End If

        Return _CallNetCoreUtility.CallNetCorePost(urlCompleto, _AccessToken, payLoad, _objParametri_Server)

    End Function

    Private Function LeggiUrlNetCoreDataExchangeAPI() As String

        Dim objConfigSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
        Dim url As String = objConfigSiti.Leggi_Valore(0, "GiasOnline_NetCoreDataExchange_API", "", "", _objParametri_Server)

        Dim callNetCore As New CallNetCore
        callNetCore.AggiustaUrl(url, _objParametri_Server)

        Return url
    End Function

    'Private Function GetBearerToken() As String

    '    Dim objParametri_Super_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Super_Server")

    '    Dim bearerToken As String = ""
    '    Dim authCookieString As String = ""

    '    'Dim authCookieArray As String() = HttpContext.Current.Request.Headers.GetValues("auth_cookie")
    '    'If Not IsNothing(authCookieArray) AndAlso authCookieArray.Any() Then
    '    '    authCookieString = authCookieArray(0)
    '    'End If

    '    Dim authCookie = HttpContext.Current.Request.Cookies("auth_cookie")
    '    If Not IsNothing(authCookie) Then
    '        authCookieString = authCookie.Value
    '    End If

    '    'If String.IsNullOrWhiteSpace(authCookieString) Then
    '    '    Dim authorization As String() = HttpContext.Current.Request.Headers.GetValues("Authorization")

    '    '    If Not IsNothing(authorization) AndAlso authorization.Any() > 0 Then
    '    '        bearerToken = authorization.Last().Substring("Bearer ".Length).Trim()
    '    '    End If
    '    'Else
    '    '    Dim utentiTokenRead As New AgronicaCoreUtentiDAL.Utenti_TokenJWT_R
    '    '    Dim DT As DataTable = utentiTokenRead.LeggiConAuthCookie(authCookieString, objParametri_Super_Server)
    '    '    If DT IsNot Nothing AndAlso DT.Rows.Count > 0 Then
    '    '        bearerToken = DT.Rows(0)("BearerToken")
    '    '    End If
    '    'End If

    '    If Not String.IsNullOrWhiteSpace(authCookieString) Then

    '        Dim utentiTokenRead As New AgronicaCoreUtentiDAL.Utenti_TokenJWT_R
    '        Dim DT As DataTable = utentiTokenRead.LeggiConAuthCookie(authCookieString, objParametri_Super_Server)
    '        If DT IsNot Nothing AndAlso DT.Rows.Count > 0 Then
    '            bearerToken = DT.Rows(0)("BearerToken")
    '        End If

    '    End If

    '    Return bearerToken

    'End Function


    'Private Sub VerificaCodiceStazione(ByVal Tipo_Sorgente As Integer, ByRef Station_Cod As String,
    '                                   ByRef Coordinates As CalcoloRischioDSSDifesaRequest.GeoCoordinates,
    '                                   ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri)

    '    Dim Station_Cod_Int As Integer
    '    If Integer.TryParse(Station_Cod, Station_Cod_Int) Then
    '        If Coordinates Is Nothing Then
    '            Coordinates = GetCoordinateStazioneNT(Tipo_Sorgente, Station_Cod_Int, objParametri_Server)
    '        Else
    '            Station_Cod = Nothing
    '        End If
    '    End If

    'End Sub

    Private Function GetModelCodeById(ByVal Veg_Cod As Integer, ByVal Mod_Id As Integer) As String
        Dim Model_Info = GetModelloInfoById(Veg_Cod, Mod_Id)
        Return If(Not Model_Info Is Nothing AndAlso Not Model_Info.Code Is Nothing, Model_Info.Code, "")
    End Function

    Private Function GetModelDescriptionById(ByVal Veg_Cod As Integer, ByVal Mod_Id As Integer) As String
        Dim Model_Info = GetModelloInfoById(Veg_Cod, Mod_Id)
        Return If(Not Model_Info Is Nothing AndAlso Not Model_Info.Description Is Nothing, Model_Info.Description, "")
    End Function

    Private Function GetModelloInfoById(ByVal Veg_Cod As Integer, ByVal Mod_Id As Integer) As ModelloCacheInfo
        Return GetModelloInfo(Veg_Cod, Nothing, Mod_Id)
    End Function

    Private Function GetModelloInfo(ByVal Veg_Cod As Integer, ByVal Mod_Cod As String, ByVal Mod_Id As Integer) As ModelloCacheInfo

        If _VegModCachedMap Is Nothing Then
            _VegModCachedMap = New Dictionary(Of Integer, List(Of ModelloCacheInfo))
        End If

        If Not _VegModCachedMap.ContainsKey(Veg_Cod) Then
            _VegModCachedMap(Veg_Cod) = New List(Of ModelloCacheInfo)
        End If
        Dim ModInfoList = _VegModCachedMap(Veg_Cod)

        If (Not String.IsNullOrEmpty(Mod_Cod) AndAlso Not ModInfoList.Where(Function(x) x.Code = Mod_Cod).Any) OrElse
            Not ModInfoList.Where(Function(x) x.ID = Mod_Id).Any Then

            Dim response = ChiamataDSSDifesa_LeggiModelli(Veg_Cod, New List(Of String))

            If Not response?.Modelli Is Nothing Then
                For Each Modello In response.Modelli
                    If Not ModInfoList.Where(Function(x) x.ID = Modello.Id And x.Code = Modello.Code).Any Then
                        ModInfoList.Add(New ModelloCacheInfo With {.ID = Modello.Id, .Code = Modello.Code, .Description = Modello.Description})
                    End If
                Next
            End If
        End If

        If Not String.IsNullOrEmpty(Mod_Cod) Then
            If ModInfoList.Where(Function(x) x.Code = Mod_Cod).Any Then
                Return ModInfoList.Where(Function(x) x.Code = Mod_Cod).FirstOrDefault
            End If
        ElseIf ModInfoList.Where(Function(x) x.ID = Mod_Id).Any Then
            Return ModInfoList.Where(Function(x) x.ID = Mod_Id).FirstOrDefault
        End If

        Return New ModelloCacheInfo

    End Function

    Private Function GetStationCodeById(ByVal Station_Id As Integer) As String

        If _interfacciaMeteoSuite Is Nothing Then
            _interfacciaMeteoSuite = New InterfacciaMeteoSuite(_objParametri_Server, _objParametri_Super_Server)
        End If

        Dim strStationCode As String = ""

        Dim jsonRisp = JObject.Parse(_interfacciaMeteoSuite.LeggiStazione(Station_Id, False))
        Dim objStaz = JObject.Parse(jsonRisp("RispostaStringa"))

        If Not objStaz Is Nothing Then
            strStationCode = objStaz("RifFornitore").ToString()
        End If

        Return strStationCode
    End Function

    Private Function GetCoordinateStazioneMeteoSuite(ByVal Station_Id As Integer) As CalcoloRischioDSSDifesaRequest.GeoCoordinates

        If _interfacciaMeteoSuite Is Nothing Then
            _interfacciaMeteoSuite = New InterfacciaMeteoSuite(_objParametri_Server, _objParametri_Super_Server)
        End If

        Dim Coordinates As New CalcoloRischioDSSDifesaRequest.GeoCoordinates

        Dim jsonRisp = JObject.Parse(_interfacciaMeteoSuite.LeggiStazione(Station_Id, False))
        Dim objStaz = JObject.Parse(jsonRisp("RispostaStringa"))

        If Not objStaz Is Nothing Then
            Coordinates.Latitude = objStaz("Lat")
            Coordinates.Longitude = objStaz("Lng")
        End If

        Return Coordinates
    End Function

    Private Function GetCoordinateStazioneNT(ByVal Tipo_Sorgente As Integer, ByVal Station_Id As Integer) As CalcoloRischioDSSDifesaRequest.GeoCoordinates

        Dim Coordinates As New CalcoloRischioDSSDifesaRequest.GeoCoordinates

        If (Tipo_Sorgente = enum_Meteo_Tiposorgente.Gias_RER Or Tipo_Sorgente = enum_Meteo_Tiposorgente.Gias_RER_Quadranti) Then

            Dim objP As New JObject
            objP("PIVA_Superuser") = _objParametri_Server.PivaSuperUser
            objP("PIVA") = ""
            objP("TipoSorgente") = Tipo_Sorgente

            'Dim objMeteoNT As New AgronicaCoreWebService.MeteoNT
            'Dim jsonRisposta = JObject.Parse(objMeteoNT.LeggiStazioniXSorgente(objP, _objParametri_Server))
            Dim objMeteoSuite As New InterfacciaMeteoSuite(_objParametri_Server, _objParametri_Super_Server)
            Dim jsonRisposta = JObject.Parse(objMeteoSuite.LeggiStazioniXSorgente(objP))
            Dim arrStazioni = JArray.Parse(jsonRisposta("RispostaStringa").ToString())
            Dim objStaz = arrStazioni.Where(Function(s) CInt(s("id_stazione")) = Station_Id).FirstOrDefault()

            If Not objStaz Is Nothing AndAlso Not objStaz("Lat") Is Nothing AndAlso objStaz("Lat") Is Nothing Then
                Coordinates.Latitude = objStaz("Lat")
                Coordinates.Longitude = objStaz("Lng")
            End If

        Else

            'Dim objMeteoNT As New AgronicaCoreWebService.MeteoNT
            'Dim jsonRisposta = JObject.Parse(objMeteoNT.LeggiStazione(Station_Id, False, _objParametri_Server))
            Dim objMeteoSuite As New InterfacciaMeteoSuite(_objParametri_Server, _objParametri_Super_Server)
            Dim jsonRisposta = JObject.Parse(objMeteoSuite.LeggiStazione(Station_Id, False))
            Dim objStaz = JObject.Parse(jsonRisposta("RispostaStringa"))

            If Not objStaz Is Nothing AndAlso Not objStaz("Lat") Is Nothing AndAlso Not objStaz("Lat") Is Nothing Then
                Coordinates.Latitude = objStaz("Lat")
                Coordinates.Longitude = objStaz("Lng")
            End If

        End If

        Return Coordinates
    End Function


    Private Function ControllaUtilizzoNuovoEngineDSSDifesa() As Boolean

        Dim objConfigSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
        Dim useNewDSSDifesaValue As String = objConfigSiti.Leggi_Valore(0, "isActiveEngine_DSSDifesa", "", "", _objParametri_Server)

        Return useNewDSSDifesaValue = "1"
    End Function

    Private Function ControllaUtilizzoNuovoEngineMeteoSuite() As Boolean

        Dim objConfigSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
        Dim useNewDSSDifesaValue As String = objConfigSiti.Leggi_Valore(0, "isActiveEngine_MeteoSuite", "", "", _objParametri_Server)

        Return useNewDSSDifesaValue = "1"
    End Function

End Class
