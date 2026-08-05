Imports System.Globalization
Imports System.Web.Script.Serialization
Imports System.Web.Services
Imports AgroAgenda_2010.Resources
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDatiReteAcquaBIZ
Imports AgronicaCoreMeteoBiz
Imports AgronicaCoreVarieBIZ
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

Public Class DatiReteAcqua_WS
    Inherits System.Web.UI.Page
    Protected Overrides Sub InitializeCulture()
        MyBase.InitializeCulture()
        Lingua.Gias_InizializzaCultura_DaSession()

    End Sub

    Private Shared Function GetDoorkey() As String
        Dim Doorkey As String = "Y4h8u3B5w2"
        Return Doorkey
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function RicercaElencoGruppiConsegnaDaCoordinate(ByVal latcentro As Double, ByVal longcentro As Double) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Lingua.Gias_InizializzaCultura_DaSession()

        Try

            'Inserire il codice QUI..
            Dim cmbSogenti As New DropDownList

            Dim PivaVisibilita_ClientGiasImpostata As String = "ACMO"


            Dim objMeteo As New AgronicaCoreWebService.Meteo
            objMeteo.Chiama_WS_Meteo_CaricaCombo_StazioniMeteoConDistanza(
                    cmbSogenti, objParametri_Server.PivaSuperUser,
                    longcentro, latcentro,
                    False, "", "", "", "", HttpContext.Current.Session("ASG_Utente_Username"), PivaVisibilita_ClientGiasImpostata,
                    HttpContext.Current.Session("ASG_Utente_Username_Crypt"),
                    HttpContext.Current.Session("ASG_Utente_Password_Crypt"),
                    GetDoorkey(),
                    1,      'solo le stazioni in visibilità
                    Nothing,
                    objParametri_Server
                )

            r.RispostaOK = True
            r.RispostaStringa = AgronicaCoreUtility.CaricaListControl.itemsCollection_AsJsonArray(cmbSogenti.Items, "sorgente_cod", "sorgente_des")

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.Errore = r.Errore.Replace(vbCrLf, "</br>")
        End Try

        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function ElencoGruppiConsegna() As rispostaStandard(Of Risposta_ElencoGruppiConsegna)
        Dim risp As New rispostaStandard(Of Risposta_ElencoGruppiConsegna) With {.RispostaOK = False}
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            risp.Sessione = False
            Return risp
        End If

        Try
            Dim objZone As New AgronicaCoreAnagrafeDAL.Zone_R
            risp.RispostaStringa = New Risposta_ElencoGruppiConsegna()
            Dim dt = objZone.Leggi(0, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, " Piva_SuperUser='" + objParametri_Server.PivaSuperUser + "' ", " Zona_Cod ASC ", objParametri_Server)
            For Each row In dt.Rows
                risp.RispostaStringa.elenco.Add(New GruppoConsegna() With {
                                .id = row("Zona_Cod"),
                                .desc = row("Descrizione")
                           })
            Next
            risp.RispostaOK = True

        Catch ex As Exception
            risp.RispostaOK = False
            risp.RispostaStringa.elenco = New List(Of GruppoConsegna)
            'uso questa funzione per ottenere il Messaggio..:
            risp.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            risp.Errore = risp.Errore.Replace(vbCrLf, "</br>")
        End Try
        Return risp
    End Function



    <WebMethod(EnableSession:=True)>
    Public Shared Function RicercaElencoGruppiConsegna(ByVal piva As String, ByVal latcentro As Double, ByVal lngcentro As Double) As RispostaStandard
        Dim risp As New RispostaStandard With {.RispostaOK = False}

        Dim objParametri_Super_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Super_Server")
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            risp.Sessione = False
            Return risp
        End If

        Try
            If String.IsNullOrEmpty(piva) Then
                Dim objParametriAgenda As New AgronicaCoreModello.ParametriAgenda_Temp.ParametriAgenda
                piva = objParametriAgenda.Piva
            End If

            Dim objParams As New AgronicaCoreDTOStd.InData.IoT.DispositiviXSorgente
            objParams.PIVA = piva
            objParams.TipoSorgente = 1
            objParams.DistanzaDa = New AgronicaCoreDTOStd.InData.IoT.DistanzaDa() With {.lat = latcentro, .lng = lngcentro}

            'Dim objMeteoNT As New AgronicaCoreWebService.MeteoNT
            Dim objIOT As New AgronicaCoreWebService.DatiIOT

            Dim json = JObject.Parse(objIOT.LeggiDispositiviXSorgente(objParams, objParametri_Super_Server, objParametri_Server, objParametri_Utenti))

            risp.RispostaStringa = json("RispostaStringa").ToString

            risp.RispostaOK = True

        Catch ex As Exception

            'uso questa funzione per ottenere il Messaggio..:
            risp.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            risp.Errore = risp.Errore.Replace(vbCrLf, "</br>")
        End Try

        Return risp
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function LocalizzaDispositivo(ByVal id_dispositivo As Integer, ByVal needSensors As Boolean) As RispostaStandard
        Dim risp As New RispostaStandard With {.RispostaOK = False}

        Dim objParametri_Super_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Super_Server")
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            risp.Sessione = False
            Return risp
        End If

        Try
            Dim objParams As New AgronicaCoreDTOStd.InData.IoT.LeggiDispositivoIn
            objParams.id_dispositivo = id_dispositivo
            objParams.needSensors = needSensors
            Dim objIOT As New AgronicaCoreWebService.DatiIOT
            Dim json = JObject.Parse(objIOT.LocalizzaDispositivo(objParams, objParametri_Super_Server, objParametri_Server, objParametri_Utenti))

            risp.RispostaStringa = json("RispostaStringa").ToString

            risp.RispostaOK = True

        Catch ex As Exception
            'uso questa funzione per ottenere il Messaggio..:
            risp.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            risp.Errore = risp.Errore.Replace(vbCrLf, "</br>")
        End Try
        Return risp
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Leggi_Dati(DataDa As String, DataA As String, FrequenzaDati As String, TipoSorgente As String, Sorgente As String, SorgenteDesc As String) As rispostaStandard(Of AgronicaCoreDTOStd.InData.IoT.RisultatoIOT)
        Dim r As New rispostaStandard(Of AgronicaCoreDTOStd.InData.IoT.RisultatoIOT)

        Dim objParametri_Super_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Super_Server")
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try
            Dim objParams As New AgronicaCoreDTOStd.InData.IoT.RichiediDatiIOT

            objParams.TipoSorgente = TipoSorgente
            objParams.Sorgente = Sorgente
            objParams.dataInizio = DataDa
            objParams.dataFine = DataA
            objParams.frequenzaDati = FrequenzaDati

            Dim linguaSession As Lingua = CType(System.Web.HttpContext.Current.Session("LinguaCorrente"), Lingua)
            objParams.LinguaCodiceISO = linguaSession.CodiceISO

            'Dim objMeteoNT As New AgronicaCoreWebService.MeteoNT
            Dim ObjIOT As New AgronicaCoreWebService.DatiIOT

            Dim ss As String = ObjIOT.DatiIOTElabora(objParams, objParametri_Super_Server, objParametri_Server, objParametri_Utenti)

            Dim jss = New JavaScriptSerializer With {
                .MaxJsonLength = Integer.MaxValue
            }

            Dim tmp = JsonConvert.DeserializeObject(Of rispostaStandard(Of AgronicaCoreDTOStd.InData.IoT.RisultatoIOT))(ss)
            If tmp.RispostaOK = False Then
                Throw New Exception(tmp.Errore & vbCrLf)
            End If

            Dim tmp_table = JObject.Parse(tmp.RispostaStringa.Meteo_Table)


            Dim new_row = JToken.Parse("{""field"":  ""gruppo"", ""title"": ""GruppoConsegna"" , ""filterable"": false , ""headerAttributes"": { ""class"": ""allineadestra"" } , ""attributes"": { ""class"": ""allineadestra"" }  }")

            tmp_table.SelectToken("kendo_columns")(0).AddAfterSelf(new_row)

            Dim o As JObject = JObject.Parse(tmp_table.SelectToken("kendo_model").ToString)

            o.Add(New JProperty("gruppo", New JObject(New JProperty("editable", False), New JProperty("type", "string"))))

            tmp_table("kendo_model") = o

            Dim i As Integer
            'dati per grafico


            Dim jr = JArray.Parse(tmp_table("kendo_rows").ToString)
            Dim jc = JArray.Parse(tmp_table("kendo_columns").ToString)
            Dim id_daily_water = ""
            Dim id_press_water = ""
            For i = 0 To jc.Count - 1
                Dim p = JObject.Parse(jc(i).ToString)
                If p.SelectToken("title").ToString().Contains("DAILY WATER VOLUME") Then
                    id_daily_water = p.SelectToken("field")
                    Exit For
                End If
            Next
            If id_daily_water = "" Then
                For i = 0 To jc.Count - 1
                    Dim p = JObject.Parse(jc(i).ToString)
                    If p.SelectToken("title").ToString().Contains("WATER PRESSURE") Then
                        id_press_water = p.SelectToken("field")
                        Exit For
                    End If
                Next
            End If


            Dim chart
            If id_daily_water <> "" Then
                chart = New DatiReteAcqua_Chart(Of Date)
                chart.title = "Volume Acqua Giornaliero"
                chart.xValuesDateType = True
                chart.yValues.Add(New Chart_LineWithValues() With {
                                        .color = "#0000FF",
                                        .name = ""
                                  })
            Else
                chart = New DatiReteAcqua_Chart(Of String)
                chart.title = "Pressione Acqua"
                chart.xValuesDateType = False
                chart.yValues.Add(New Chart_LineWithValues() With {
                                        .color = "#0000FF",
                                        .name = ""
                                  })
            End If

            For i = 0 To jr.Count - 1
                Dim p = JObject.Parse(jr(i).ToString)
                p.Add(New JProperty("gruppo", SorgenteDesc))
                jr(i) = p

                If id_daily_water <> "" Then
                    'chart
                    Dim x = Date.Parse(Date.Parse(p.SelectToken("DataOra").ToString()).ToString("dd/MM/yyyy"))
                    Dim y = Decimal.Parse(IIf(p.SelectToken(id_daily_water).ToString() = "", "0", p.SelectToken(id_daily_water).ToString()))
                    If chart.xValues.Contains(x) = False Then
                        chart.xValues.Add(x)
                        chart.yValues(0).data.Add(y)
                    Else
                        chart.yValues(0).data(chart.yValues(0).data.Count - 1) = y
                    End If
                Else
                    Dim x = Date.Parse(p.SelectToken("DataOra").ToString()).ToString("dd/MM/yyyy hh:mm:ss")
                    Dim y = Decimal.Parse(IIf(p.SelectToken(id_press_water).ToString() = "", "0", p.SelectToken(id_press_water).ToString()))

                    chart.xValues.Add(x)
                    chart.yValues(0).data.Add(y)
                End If
            Next




            tmp_table("kendo_rows") = jr
            tmp.RispostaStringa.Chart_Type = IIf(id_daily_water <> "", "water", "pressure")
            tmp.RispostaStringa.Meteo_Table = tmp_table.ToString
            tmp.RispostaStringa.Meteo_Charts = JsonConvert.SerializeObject(chart)
            r = tmp

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

            r.Errore = r.Errore.Replace(vbCrLf, "</br>")

        End Try

        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Leggi_Anagrafe(id_device As String) As rispostaStandard(Of Risposta_DatiReteAcquaAnagrafe)
        Dim r As New rispostaStandard(Of Risposta_DatiReteAcquaAnagrafe)

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try
            Dim parco_macc As New AgronicaCoreContabBIZ.Parco_Macchine_R
            Dim data = parco_macc.Leggi_MacchinaCaratteristiche_Da_ChiaveAPI(id_device, objParametri_Server)

            Dim lst As New List(Of DatiReteAcquaCaratteristica)
            Dim i As Integer
            Dim img As String = ""
            If data IsNot Nothing Then
                For i = 0 To data.Keys.Count - 1
                    If data.Keys(i) = "Image" Then
                        img = data.Values(i)
                    Else
                        lst.Add(New DatiReteAcquaCaratteristica() With {.Id = data.Keys(i), .Value = data.Values(i)})
                    End If
                Next
            End If

            r.RispostaOK = True
            r.RispostaStringa = New Risposta_DatiReteAcquaAnagrafe() With {.data = lst, .image = img}

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.Errore = r.Errore.Replace(vbCrLf, "</br>")
        End Try

        Return r
    End Function


    <WebMethod(EnableSession:=True)>
    Public Shared Function Leggi_DatiStorici(veg_cod As Integer, settimana As Integer, anno As String, gruppoconsegna As String) As rispostaStandard(Of Risposta_DatiReteAcquaPrelieviStorici)
        Dim r As New rispostaStandard(Of Risposta_DatiReteAcquaPrelieviStorici)

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try
            Dim iotdatistorici As New AgronicaCoreDatiReteAcquaBIZ.DatiReteAcquaStoriciPrelieviTR10_R

            Dim res = iotdatistorici.LeggiDatiStorici(objParametri_Server.PivaSuperUser, veg_cod, settimana, anno, gruppoconsegna, objParametri_Server)
            If res IsNot Nothing Then
                r.RispostaOK = True
                r.RispostaStringa = res
            End If

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.Errore = r.Errore.Replace(vbCrLf, "</br>")
        End Try

        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function AggiornaDatiStorici(kendoGrid As String) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try
            Dim iotbiz As New AgronicaCoreDatiReteAcquaBIZ.DatiReteAcquaStoriciPrelieviTR10_W

            Dim ar = JArray.Parse(kendoGrid)
            Dim dati_specie As New Dictionary(Of Integer, Tuple(Of Decimal, Decimal))
            Dim jo As JObject = ar(0)
            Dim veg_cod = CType(jo.Properties.ToList()(3).Name.Replace("SU_", ""), Integer)
            Dim anno = CType(jo.Properties.ToList()(2).Value, Integer)
            Dim gruppoconsegna = CType(jo.Properties.ToList()(1).Value, Integer)
            For Each o As JObject In ar
                dati_specie.Add(CInt(o.Properties.ToList()(0).Value), New Tuple(Of Decimal, Decimal)(CType(o.Properties.ToList()(3).Value, Decimal), CType(o.Properties.ToList()(4).Value, Decimal)))
            Next

            r.RispostaOK = iotbiz.AggiornaDatiStorici(objParametri_Server.PivaSuperUser, veg_cod, anno, gruppoconsegna, dati_specie, objParametri_Server)
            r.RispostaStringa = ""

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.Errore = r.Errore.Replace(vbCrLf, "</br>")
        End Try

        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function AggiungiSpecieDatiStorici(veg_cod As String, anno As Integer, gruppoconsegna As Integer) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try
            If veg_cod = 0 Then
                Throw New Exception("Valorizzare codice specie")
            End If

            Dim iotbiz As New AgronicaCoreDatiReteAcquaBIZ.DatiReteAcquaStoriciPrelieviTR10_W

            r.RispostaOK = iotbiz.AggiungiSpecieDatiStorici(objParametri_Server.PivaSuperUser, veg_cod, anno, gruppoconsegna, objParametri_Server)
            r.RispostaStringa = ""

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.Errore = r.Errore.Replace(vbCrLf, "</br>")
        End Try

        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Leggi_ParametriGenerali(settimana As Integer) As rispostaStandard(Of Risposta_DatiReteAcquaPars)
        Dim r As New rispostaStandard(Of Risposta_DatiReteAcquaPars)

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try
            Dim iotparametri As New AgronicaCoreDatiReteAcquaBIZ.DatiReteAcqua_Parametri_R

            Dim res = iotparametri.LeggiParametriGenerali(objParametri_Server.PivaSuperUser, settimana, objParametri_Server)
            If res IsNot Nothing Then
                r.RispostaOK = True
                r.RispostaStringa = res
            End If

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.Errore = r.Errore.Replace(vbCrLf, "</br>")
        End Try

        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Leggi_CoeffXSpecie(veg_cod As Integer, settimana As Integer) As rispostaStandard(Of Risposta_DatiReteAcquaPars)
        Dim r As New rispostaStandard(Of Risposta_DatiReteAcquaPars)

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try
            Dim iotparametri As New AgronicaCoreDatiReteAcquaBIZ.DatiReteAcqua_Parametri_R

            Dim res = iotparametri.LeggiCoefficientiXSpecie(objParametri_Server.PivaSuperUser, veg_cod, settimana, objParametri_Server)
            If res IsNot Nothing Then
                r.RispostaOK = True
                r.RispostaStringa = res
            End If

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.Errore = r.Errore.Replace(vbCrLf, "</br>")
        End Try

        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function InizializzaParametriGenerali() As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try
            Dim iotbiz As New AgronicaCoreDatiReteAcquaBIZ.DatiReteAcqua_Parametri_W

            r.RispostaOK = iotbiz.InizializzaParametriGenerali(objParametri_Server.PivaSuperUser, objParametri_Server)
            r.RispostaStringa = ""

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.Errore = r.Errore.Replace(vbCrLf, "</br>")
        End Try

        Return r
    End Function


    <WebMethod(EnableSession:=True)>
    Public Shared Function ModificaParametriGenerali(kendoGrid As String) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try
            Dim iotbiz As New AgronicaCoreDatiReteAcquaBIZ.DatiReteAcqua_Parametri_W

            Dim ar = JArray.Parse(kendoGrid)

            Dim dati_gen As New List(Of DatiReteAcqua_ParametriGenerali)
            Dim jo As JObject = ar(0)
            For Each o As JObject In ar
                dati_gen.Add(New DatiReteAcqua_ParametriGenerali() With {
                    .Settimana = CInt(o.Properties.ToList()(0).Value),
                    .RadiazioneExtraterrestre = CType(o.Properties.ToList()(1).Value, Decimal)
                })
            Next

            r.RispostaOK = iotbiz.ModificaParametriGenerali(objParametri_Server.PivaSuperUser, dati_gen, objParametri_Server)
            r.RispostaStringa = ""


        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.Errore = r.Errore.Replace(vbCrLf, "</br>")
        End Try

        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function AggiungiCoefficientiXSpecie(veg_cod As String) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try
            If veg_cod = 0 Then
                Throw New Exception("Valorizzare codice specie")
            End If

            Dim iotbiz As New AgronicaCoreDatiReteAcquaBIZ.DatiReteAcqua_Parametri_W

            r.RispostaOK = iotbiz.AggiungiCoefficientiXSpecie(objParametri_Server.PivaSuperUser, veg_cod, objParametri_Server)
            r.RispostaStringa = ""

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.Errore = r.Errore.Replace(vbCrLf, "</br>")
        End Try

        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function ModificaCoefficientiXSpecie(kendoGrid As String) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try
            Dim iotbiz As New AgronicaCoreDatiReteAcquaBIZ.DatiReteAcqua_Parametri_W

            Dim ar = JArray.Parse(kendoGrid)
            Dim dati_specie As New List(Of CoefficientiXSpecieVegetale)
            Dim jo As JObject = ar(0)
            Dim veg_cod = CType(jo.Properties.ToList()(1).Value, Integer)
            'Dim anno = CType(jo.Properties.ToList()(1).Value, Integer)
            For Each o As JObject In ar
                dati_specie.Add(New CoefficientiXSpecieVegetale() With {
                                .Settimana = CInt(o.Properties.ToList()(0).Value),
                                .Kc = CType(o.Properties.ToList()(3).Value, Decimal),
                                .DFc = CType(o.Properties.ToList()(4).Value, Decimal)
                                })
            Next

            r.RispostaOK = iotbiz.ModificaCoefficientiXSpecie(objParametri_Server.PivaSuperUser, veg_cod, dati_specie, objParametri_Server)
            r.RispostaStringa = ""

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.Errore = r.Errore.Replace(vbCrLf, "</br>")
        End Try

        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function CalcolaConfrontoPrelieviOsservatiAttesi(ByVal piva As String,
                                                          ByVal id_contatore As String,
                                                          ByVal id_stazione_user As Integer,
                                                          ByVal latcentro As Double,
                                                          ByVal longcentro As Double,
                                                          ByVal DataDa As String,
                                                          ByVal DataA As String,
                                                          ByVal VisualizzaStorico As Boolean) As rispostaStandard(Of Risposta_DatiReteAcquaCalcoloConfronto)

        Dim r As New rispostaStandard(Of Risposta_DatiReteAcquaCalcoloConfronto)

        Dim objParametri_SuperServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Super_Server")
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try
            Dim id_stazione = ""
            Dim jss = New JavaScriptSerializer With {
                .MaxJsonLength = Integer.MaxValue
            }

            '0 - recupero la stazione meteo più vicina
            If String.IsNullOrEmpty(piva) Then
                Dim objParametriAgenda As New AgronicaCoreModello.ParametriAgenda_Temp.ParametriAgenda
                piva = objParametriAgenda.Piva
            End If

            If id_stazione_user = 0 Then
                'recupero la stazione da lat\lng
                Dim objParams As New JObject
                objParams("PIVA_Superuser") = objParametri_Server.PivaSuperUser
                objParams("PIVA") = piva
                objParams("TipoSorgente") = 1
                objParams("DistanzaDa") = New JObject(New JProperty("lat", latcentro), New JProperty("lng", longcentro))

                'Dim objMeteoNT As New AgronicaCoreWebService.MeteoNT
                Dim objMeteoSuite As New InterfacciaMeteoSuite(objParametri_Server, objParametri_SuperServer)

                'Dim json = JObject.Parse(objMeteoNT.LeggiStazioniXSorgente(objParams, objParametri_Server))
                Dim json = JObject.Parse(objMeteoSuite.LeggiStazioniXSorgente(objParams))

                Dim ar = JArray.Parse(json.ToObject(Of RispostaStandard).RispostaStringa)
                Dim i = 0


                'recupero la prima stazione non acmotec
                For i = 0 To ar.Count - 1
                    If ar(i).SelectToken("fornitore").Value(Of String) <> "Acmotec" Then
                        id_stazione = ar(i).SelectToken("id_stazione").Value(Of String)
                        Exit For
                    End If
                Next

                If id_stazione = "" Then
                    Throw New Exception("Nessuna stazione meteo trovata. Impossibile eseguire il calcolo" & vbCrLf)
                End If
            End If

            Dim linguaSession As Lingua = CType(System.Web.HttpContext.Current.Session("LinguaCorrente"), Lingua)
            Dim yearCal As Integer = Date.Now.Year
            Dim dataCalcDa As Date = New Date(Date.Now.Year, 1, 1)
            Dim dataCalcA As Date = Date.Now

            If Date.Parse(DataDa).Year <> Date.Now.Year Then
                'anno diverso da quello corrente
                yearCal = Date.Parse(DataDa).Year
                dataCalcDa = New Date(yearCal, 1, 1)
                dataCalcA = New Date(yearCal, 12, 31)
            End If

            Dim datimeteo = New DatiMeteoXCalcoloPrelievi(linguaSession, objParametri_Server, objParametri_SuperServer).RecuperaDatiMeteo(IIf(id_stazione = "", id_stazione_user, id_stazione), dataCalcDa, dataCalcA)

            If datimeteo Is Nothing Then
                Throw New Exception("Nessun dato meteo recuperato per la stazione\periodo indicati" & vbCrLf)
            End If

            Dim dati_contatori = New DatiReteAcquaXCalcoloPrelievi(linguaSession, objParametri_SuperServer, objParametri_Server, objParametri_Utenti).RecuperaDatiContatori(id_contatore, dataCalcDa, dataCalcA)
            If dati_contatori Is Nothing OrElse dati_contatori.Count <= 0 Then
                Throw New Exception("Nessun dato presente per il gruppo di consegna nel periodo ( " + dataCalcDa.ToString() + " / " + dataCalcA.ToString() + " )" & vbCrLf)
            End If

            Dim objappxparticelle = New AgronicaCoreAnagrafeDAL.AppezzaxParticelle_R
            Dim gerarchiaMacc = New AgronicaCoreContabDAL.GerarchiaMacchine_R
            Dim objMacchine_Dal As New AgronicaCoreContabDAL.Parco_Macchine_R

            Dim parco_macchine = objMacchine_Dal.Leggi_MacchinaXChiaveAPI(id_contatore, "", "", objParametri_Server)
            If parco_macchine.Rows.Count <= 0 Then
                Throw New Exception("Nessun contatore trovato per la chiave: " + id_contatore & vbCrLf)
            End If


            Dim id_zona = -1

            'recupero il comizio irriguo dalla gerarchia

            Dim dt = gerarchiaMacc.LeggiConDettagliPadreFiglio(0, parco_macchine.Rows(0)("Mac_Cod"), "f.Class_Code='05.07'", "", objParametri_Server)
            If dt.Rows.Count <= 0 Then
                Throw New Exception("Contatore (" + id_contatore + ") non associato a nessuna zona/comizio" & vbCrLf)
            Else
                id_zona = dt.Rows(0)("Codice_Padre")
            End If

            Dim iotparametri As New AgronicaCoreDatiReteAcquaBIZ.DatiReteAcqua_Parametri_R

            Dim CheckSuperficiXSpecieVegetale = objappxparticelle.Leggi_TotaleSuperficiXSpecie_AppezzamentiXParticelleXZona(
                                            objParametri_Server.PivaSuperUser,
                                            id_zona,
                                            dataCalcDa,
                                            dataCalcA,
                                            "",
                                            "",
                                            objParametri_Server)

            If CheckSuperficiXSpecieVegetale.Rows.Count <= 0 Then
                Throw New Exception("Nessuna coltura associata al comizio " + id_zona.ToString() + " nel periodo ( " + dataCalcDa.ToString() + " / " + dataCalcA.ToString() + " )" & vbCrLf & " impossibile proseguire" & vbCrLf)
            End If

            Dim msgCheckParametriSpecieVegetali As String = "Sono presenti delle specie vegetali per cui non è presente la prametrizzazione per il modello : " & vbCrLf
            Dim chkBreak As Boolean = False
            For Each row In CheckSuperficiXSpecieVegetale.Rows
                Dim resSpecie = iotparametri.LeggiCoefficientiXSpecieDT(objParametri_Server.PivaSuperUser, row("Veg_Cod"), 19, objParametri_Server)
                If resSpecie.Rows.Count <= 0 Then
                    chkBreak = True
                    msgCheckParametriSpecieVegetali += " - " & row("Veg_Des") & vbCrLf
                End If
            Next
            msgCheckParametriSpecieVegetali += "Impossibile eseguire il modello" & vbCrLf

            If chkBreak Then
                Throw New Exception(msgCheckParametriSpecieVegetali)
            End If

            'parametri calcolo
            'Dim parametricalcolo As New List(Of ParametriCalcolo)
            Dim DatiPrelieviCalcolo As New List(Of DatiPrelieviCalcolo)

            Dim datiGraficoPrelievi As New DatiReteAcqua_Chart(Of Integer) With {.title = "Prelievi comizio " + id_zona.ToString() + " ( Anno: " + yearCal.ToString() + ")"}
            datiGraficoPrelievi.yValues.Add(New Chart_LineWithValues() With {.name = "Prelievo cumulato unitario previsto FULL", .color = "#0000FF"})
            datiGraficoPrelievi.yValues.Add(New Chart_LineWithValues() With {.name = "Prelievo cumulato unitario previsto DEFICIT", .color = "#00FF00"})
            datiGraficoPrelievi.yValues.Add(New Chart_LineWithValues() With {.name = "Prelievo cumulato unitario osservato", .color = "#FF0000"})

            Dim datiGraficoVolume As New DatiReteAcqua_Chart(Of Integer) With {.title = "Volume totale comizio " + id_zona.ToString() + " ( Anno: " + yearCal.ToString() + ")"}
            datiGraficoVolume.yValues.Add(New Chart_LineWithValues() With {.name = "Volume cumulato osservato", .color = "#FF0000"})

            If VisualizzaStorico Then
                datiGraficoPrelievi.yValues.Add(New Chart_LineWithValues() With {.dashType = "dash", .name = "Prelievo cumulato unitario FULL (Tr 10 anni)", .color = "#8F00FF"})
                datiGraficoPrelievi.yValues.Add(New Chart_LineWithValues() With {.dashType = "dash", .name = "Prelievo cumulato unitario DEFICIT (Tr 10 anni)", .color = "#FFA500"})

                datiGraficoVolume.yValues.Add(New Chart_LineWithValues() With {.dashType = "dash", .name = "Volume cumulato FULL (Tr 10 anni)", .color = "#8F00FF"})
                datiGraficoVolume.yValues.Add(New Chart_LineWithValues() With {.dashType = "dash", .name = "Volume cumulato DEFICIT (Tr 10 anni)", .color = "#FFA500"})
            End If





            Dim iotstorico As New AgronicaCoreDatiReteAcquaBIZ.DatiReteAcquaStoriciPrelieviTR10_R
            Dim dt_specie As New AgronicaCoreMetaSchemaDAL.SpecieVegetali_R

            Dim genpars = iotparametri.LeggiParametriGeneraliDT(objParametri_Server.PivaSuperUser, 0, objParametri_Server)

            Dim PrelievoIdricoCumulatoUnitarioStorico_FULL As Decimal = 0
            Dim PrelievoIdricoCumulatoUnitarioStorico_DEFICIT As Decimal = 0

            Dim DeltaCumulatoFornitorePeriodoPrecedenteEstrazione As Decimal = dati_contatori(0).GetCumulatoFornitore()


            For Each row In genpars.Rows
                Dim o = New DatiPrelieviCalcolo
                o.anno = yearCal
                o.settimana = row("week")
                o.radiazione_extra = row("RadiazioneExtraterrestre")

                'dati meteo nella settimana
                Dim meteoSettimana = datimeteo.Find(Function(x) x.anno = o.anno And x.settimana = o.settimana)
                Dim Et0Rif = 0.0
                Dim Pioggia = 0.0
                If meteoSettimana IsNot Nothing Then
                    Et0Rif = meteoSettimana.GetET0_Rif(o.radiazione_extra)
                    Pioggia = meteoSettimana.GetPioggia()
                End If

                'recupero l'elenco delle colture e relative superfici nella settimana
                Dim strDat = AgronicaCoreUtility.DataOra.GetFirstDayOfWeek(o.anno, o.settimana, CalendarWeekRule.FirstFullWeek)
                Dim endDat = strDat.AddDays(6)
                Dim SuperficiXSpecieVegetale = objappxparticelle.Leggi_TotaleSuperficiXSpecie_AppezzamentiXParticelleXZona(
                                            objParametri_Server.PivaSuperUser,
                                            id_zona,
                                            strDat,
                                            endDat,
                                            "",
                                            "",
                                            objParametri_Server)
                For Each specie In SuperficiXSpecieVegetale.Rows
                    Dim resSpecie = iotparametri.LeggiCoefficientiXSpecieDT(objParametri_Server.PivaSuperUser, specie("Veg_Cod"), o.settimana, objParametri_Server)
                    Dim dtVeg_des = dt_specie.Leggi(specie("Veg_Cod"), 0, "", "", AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_Server)
                    Dim veg_des = IIf(dtVeg_des.Rows.Count > 0, dtVeg_des.Rows(0)("veg_des"), "")
                    For Each rowSpecie In resSpecie.Rows
                        Dim dati_sett_precedente = DatiPrelieviCalcolo.FirstOrDefault(Function(x) x.anno = o.anno And x.settimana = o.settimana - 1)
                        Dim prev_settimana_precedente = 0.0
                        If dati_sett_precedente IsNot Nothing Then
                            Dim dati_sett_precedente_specie = dati_sett_precedente.DatiXSpecie.FirstOrDefault(Function(x) x.veg_cod = specie("Veg_Cod"))
                            If dati_sett_precedente_specie IsNot Nothing Then
                                prev_settimana_precedente = dati_sett_precedente_specie.PrevisioneCumulataFullIrrigation
                            End If
                        End If

                        o.DatiXSpecie.Add(New DatiPrelieviCalcoloXSpecie(o.settimana,
                                                                         specie("Veg_Cod"),
                                                                         Et0Rif,
                                                                         rowSpecie("Kc"),
                                                                         Pioggia,
                                                                         rowSpecie("DFc"),
                                                                         specie("Sup_Irrigua"),
                                                                         prev_settimana_precedente) With {.veg_des = veg_des})
                        If VisualizzaStorico Then
                            Dim storicoSpecie = iotstorico.LeggiDatiStoriciXSpecieDT(objParametri_Server.PivaSuperUser, specie("Veg_Cod"), o.settimana, o.anno, id_zona, objParametri_Server)
                            For Each stor In storicoSpecie.Rows
                                o.DatiStoriciXSpecie.Add(New DatiPrelieviStoriciXSpecie(specie("Veg_Cod"), rowSpecie("DFc"), stor("FullReturn"), stor("DeficitReturn"), specie("Sup_Irrigua")))
                            Next
                        End If
                    Next
                Next
                If meteoSettimana IsNot Nothing Then
                    o.temp_max = meteoSettimana.GetTemperaturaMassima()
                    o.temp_min = meteoSettimana.GetTemperaturaMinima()
                    o.temp_media = meteoSettimana.GetTemperaturaMedia()
                End If
                o.evapotraspirazione_riferimento = Et0Rif
                o.pioggia = Pioggia

                Dim dati_contatori_settimana = dati_contatori.Find(Function(x) x.anno = o.anno And x.settimana = o.settimana)
                If dati_contatori_settimana Is Nothing Then
                    o.PrelievoIdricoCumulatoFornitore = 0
                    o.PrelievoIdricoSettimanaleFornitore = 0
                    o.PrelievoIdricoCumulatoUnitarioOsservato = 0
                    o.FabbisognoIdricoCumulato_FULL = 0
                    o.FabbisognoIdricoCumulato_DEFICIT = 0
                    o.PrelievoIdricoCumulatoUnitarioPrevisto_FULL = 0
                    o.PrelievoIdricoCumulatoUnitarioPrevisto_DEFICIT = 0
                    o.PrelievoIdricoCumulatoUnitarioStorico_FULL = 0
                    o.PrelievoIdricoCumulatoUnitarioStorico_DEFICIT = 0
                Else
                    If o.GetSumArea() > 0 Then
                        o.PrelievoIdricoCumulatoFornitore = dati_contatori_settimana.GetCumulatoFornitore() - DeltaCumulatoFornitorePeriodoPrecedenteEstrazione
                        o.PrelievoIdricoSettimanaleFornitore = dati_contatori_settimana.GetCumulatoFornitoreProgressivo()
                        o.PrelievoIdricoCumulatoUnitarioOsservato = o.PrelievoIdricoCumulatoFornitore / o.GetSumArea()

                        For Each dati_specie_settimana In o.DatiXSpecie
                            o.FabbisognoIdricoCumulato_FULL += dati_specie_settimana.PrevisioneCumulataFullIrrigation() * dati_specie_settimana.SuperficieTotaleColtura()
                            o.FabbisognoIdricoCumulato_DEFICIT += dati_specie_settimana.PrevisioneCumulataDeficitIrrigation() * dati_specie_settimana.SuperficieTotaleColtura()
                        Next
                        o.PrelievoIdricoCumulatoUnitarioPrevisto_FULL = o.FabbisognoIdricoCumulato_FULL / o.GetSumArea()
                        o.PrelievoIdricoCumulatoUnitarioPrevisto_DEFICIT = o.FabbisognoIdricoCumulato_DEFICIT / o.GetSumArea()
                    Else
                        o.PrelievoIdricoCumulatoFornitore = 0
                        o.PrelievoIdricoSettimanaleFornitore = 0
                        o.PrelievoIdricoCumulatoUnitarioOsservato = 0
                        o.FabbisognoIdricoCumulato_FULL = 0
                        o.FabbisognoIdricoCumulato_DEFICIT = 0
                        o.PrelievoIdricoCumulatoUnitarioPrevisto_FULL = 0
                        o.PrelievoIdricoCumulatoUnitarioPrevisto_DEFICIT = 0
                    End If
                    o.PrelievoIdricoCumulatoUnitarioStorico_FULL = 0
                    o.PrelievoIdricoCumulatoUnitarioStorico_DEFICIT = 0
                    o.PrelievoIdricoCumulatoStorico_FULL = 0
                    o.PrelievoIdricoCumulatoStorico_DEFICIT = 0
                    If VisualizzaStorico Then
                        Dim SuperficieTotale As Decimal = 0
                        For Each dati_storico_settimana In o.DatiStoriciXSpecie
                            SuperficieTotale += dati_storico_settimana.SuperficieColturale()
                            o.PrelievoIdricoCumulatoStorico_FULL += dati_storico_settimana.PrelievoStoricoFULL() * dati_storico_settimana.SuperficieColturale()
                            o.PrelievoIdricoCumulatoStorico_DEFICIT += dati_storico_settimana.PrelievoStoricoDEFICIT() * dati_storico_settimana.SuperficieColturale()
                        Next
                        If SuperficieTotale > 0 Then
                            o.PrelievoIdricoCumulatoUnitarioStorico_FULL = o.PrelievoIdricoCumulatoStorico_FULL / SuperficieTotale
                            o.PrelievoIdricoCumulatoUnitarioStorico_DEFICIT = o.PrelievoIdricoCumulatoStorico_DEFICIT / SuperficieTotale
                        End If
                    End If
                End If
                DatiPrelieviCalcolo.Add(o)

                'grafico prelievi
                datiGraficoPrelievi.xValues.Add(o.settimana)
                datiGraficoPrelievi.yValues(0).data.Add(o.PrelievoIdricoCumulatoUnitarioPrevisto_FULL)
                datiGraficoPrelievi.yValues(1).data.Add(o.PrelievoIdricoCumulatoUnitarioPrevisto_DEFICIT)
                datiGraficoPrelievi.yValues(2).data.Add(o.PrelievoIdricoCumulatoUnitarioOsservato)
                If VisualizzaStorico Then
                    datiGraficoPrelievi.yValues(3).data.Add(o.PrelievoIdricoCumulatoUnitarioStorico_FULL)
                    datiGraficoPrelievi.yValues(4).data.Add(o.PrelievoIdricoCumulatoUnitarioStorico_DEFICIT)
                End If

                'grafico volume
                datiGraficoVolume.xValues.Add(o.settimana)
                datiGraficoVolume.yValues(0).data.Add(o.PrelievoIdricoCumulatoFornitore)
                If VisualizzaStorico Then
                    datiGraficoVolume.yValues(1).data.Add(o.PrelievoIdricoCumulatoStorico_FULL)
                    datiGraficoVolume.yValues(2).data.Add(o.PrelievoIdricoCumulatoStorico_DEFICIT)
                End If
            Next

            'genero i 3 elementi Kendo per visualizzare la tabella

            If DatiPrelieviCalcolo.Count <= 0 Then
                Throw New Exception("Calcolo non eseguito, verificare le parametrizzazioni per l'anno di esecuzione" & vbCrLf)
            End If

            Dim table As New JObject(New JProperty("kendo_columns", GetKendoColumnsDefinition(DatiPrelieviCalcolo(0).DatiXSpecie)),
                                    New JProperty("kendo_model", GetKendoModelDefinition(DatiPrelieviCalcolo(0).DatiXSpecie)),
                                    New JProperty("kendo_rows", GetKendoRows(DatiPrelieviCalcolo, id_zona)))




            'Dim tmp = jss.Deserialize(Of rispostaStandard(Of Risposta_DatiReteAcqua))(ss)
            'If tmp.RispostaOK = True Then
            '    Dim tmp_table = JObject.Parse(tmp.RispostaStringa.Meteo_Table)


            Dim d As New Risposta_DatiReteAcquaCalcoloConfronto() With {
                .Table = JsonConvert.SerializeObject(table),
                .Charts = New List(Of Risposta_DatiReteAcquaCalcoloConfronto_Charts)}

            d.Charts.Add(New Risposta_DatiReteAcquaCalcoloConfronto_Charts() With {
                            .ChartID = "modello",
                            .ChartData = JsonConvert.SerializeObject(datiGraficoPrelievi)
                         })

            d.Charts.Add(New Risposta_DatiReteAcquaCalcoloConfronto_Charts() With {
                            .ChartID = "volume",
                            .ChartData = JsonConvert.SerializeObject(datiGraficoVolume)
                         })

            r.RispostaOK = True
            r.RispostaStringa = d
            'Else
            '    r.RispostaOK = tmp.RispostaOK
            '    r.Errore = tmp.Errore
            'End If




        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, False, source:=False)
            r.Errore = r.Errore.Replace(vbCrLf, "</br>")
        End Try

        Return r
    End Function

    Private Shared Function GetKendoColumnsDefinition(ByVal datispecie As List(Of DatiPrelieviCalcoloXSpecie)) As JArray
        'colonne
        Dim KendoColumns As New JArray
        'gruppo_consegna
        KendoColumns.Add(New JObject(New JProperty("field", "gruppo_consegna"),
                                     New JProperty("title", "gruppo di consegna"),
                                     New JProperty("width", 130),
                                     New JProperty("filterable", False)))
        'anno
        KendoColumns.Add(New JObject(New JProperty("field", "anno"),
                                     New JProperty("title", "anno"),
                                     New JProperty("width", 55),
                                     New JProperty("filterable", False)))
        'settimana
        KendoColumns.Add(New JObject(New JProperty("field", "settimana"),
                                     New JProperty("title", "settimana"),
                                     New JProperty("width", 70),
                                     New JProperty("filterable", False)))
        'pioggia
        KendoColumns.Add(New JObject(New JProperty("field", "pioggia"),
                                     New JProperty("title", "pioggia"),
                                     New JProperty("width", 60),
                                     New JProperty("filterable", False),
                                     New JProperty("format", "{0:0.00}")))
        'temp_max
        KendoColumns.Add(New JObject(New JProperty("field", "temp_max"),
                                     New JProperty("title", "temp. massima"),
                                     New JProperty("width", 120),
                                     New JProperty("filterable", False),
                                     New JProperty("format", "{0:0.00}")))
        'temp_med
        KendoColumns.Add(New JObject(New JProperty("field", "temp_med"),
                                     New JProperty("title", "temp. media"),
                                     New JProperty("width", 120),
                                     New JProperty("filterable", False),
                                     New JProperty("format", "{0:0.00}")))
        'temp_min
        KendoColumns.Add(New JObject(New JProperty("field", "temp_min"),
                                     New JProperty("title", "temp. minima"),
                                     New JProperty("width", 120),
                                     New JProperty("filterable", False),
                                     New JProperty("format", "{0:0.00}")))
        'radiazione_entra
        KendoColumns.Add(New JObject(New JProperty("field", "radiazione_extra"),
                                     New JProperty("title", "Rad. Extraterrestre"),
                                     New JProperty("width", 150),
                                     New JProperty("filterable", False),
                                     New JProperty("format", "{0:0.0}")))
        'evapotraspirazione_rif
        KendoColumns.Add(New JObject(New JProperty("field", "evapotraspirazione_riferimento"),
                                     New JProperty("title", "Evapotraspirazione"),
                                     New JProperty("width", 150),
                                     New JProperty("filterable", False),
                                     New JProperty("format", "{0:0.0}")))

        For Each dati_specie In datispecie
            ''''loop x specie
            'Coeff evapotrasp
            'Coeff deficit
            'fabbisogno irriguo
            'fabbisonocumulato_full
            'fabbisonocumulato_deficit

            KendoColumns.Add(New JObject(New JProperty("field", "SU_" + dati_specie.veg_cod.ToString()),
                                     New JProperty("title", "Sup. " + dati_specie.veg_des),
                                     New JProperty("width", 120),
                                     New JProperty("filterable", False),
                                     New JProperty("format", "{0:0.00}")))

            KendoColumns.Add(New JObject(New JProperty("field", "CE_" + dati_specie.veg_cod.ToString()),
                                     New JProperty("title", "Etc " + dati_specie.veg_des),
                                     New JProperty("width", 120),
                                     New JProperty("filterable", False),
                                     New JProperty("format", "{0:0.00}")))

            KendoColumns.Add(New JObject(New JProperty("field", "CD_" + dati_specie.veg_cod.ToString()),
                                     New JProperty("title", "Coeff Deficit " + dati_specie.veg_des),
                                     New JProperty("width", 150),
                                     New JProperty("filterable", False),
                                     New JProperty("format", "{0:0.00}")))

            KendoColumns.Add(New JObject(New JProperty("field", "FI_" + dati_specie.veg_cod.ToString()),
                                     New JProperty("title", "IR, " + dati_specie.veg_des),
                                     New JProperty("width", 140),
                                     New JProperty("filterable", False),
                                     New JProperty("format", "{0:0.00}")))

            KendoColumns.Add(New JObject(New JProperty("field", "FI_CUM_FULL_" + dati_specie.veg_cod.ToString()),
                                     New JProperty("title", "IR_cum_F, " + dati_specie.veg_des),
                                     New JProperty("width", 140),
                                     New JProperty("filterable", False),
                                     New JProperty("format", "{0:0.00}")))

            KendoColumns.Add(New JObject(New JProperty("field", "FI_CUM_DEF_" + dati_specie.veg_cod.ToString()),
                                     New JProperty("title", "IR_cum_D, " + dati_specie.veg_des),
                                     New JProperty("width", 140),
                                     New JProperty("filterable", False),
                                     New JProperty("format", "{0:0.00}")))
        Next

        'prelievo cumulato fornitore
        KendoColumns.Add(New JObject(New JProperty("field", "prel_cum_fornitore"),
                                    New JProperty("title", "Prelievo Cumulato Fornitore"),
                                    New JProperty("width", 180),
                                    New JProperty("filterable", False),
                                     New JProperty("format", "{0:0.00}")))

        'prelievo cumulato settimanale fornitore
        KendoColumns.Add(New JObject(New JProperty("field", "prel_cum_sett_fornitore"),
                                    New JProperty("title", "Prelievo Cumulato Settimanale"),
                                    New JProperty("width", 180),
                                    New JProperty("filterable", False),
                                     New JProperty("format", "{0:0.00}")))
        'cumulato unitario
        KendoColumns.Add(New JObject(New JProperty("field", "prel_cum_unitario_oss"),
                                    New JProperty("title", "Cumulato unitario osservato"),
                                    New JProperty("width", 180),
                                    New JProperty("filterable", False),
                                    New JProperty("format", "{0:0.0}")))
        'prel cumulato full
        KendoColumns.Add(New JObject(New JProperty("field", "prel_cum_full"),
                                    New JProperty("title", "Prelievo Cumulato FULL Tr10anni"),
                                    New JProperty("width", 220),
                                    New JProperty("filterable", False),
                                    New JProperty("format", "{0:0.0}")))
        'prel cumulato deficit
        KendoColumns.Add(New JObject(New JProperty("field", "prel_cum_deficit"),
                                   New JProperty("title", "Prelievo Cumulato DEFICIT Tr10anni"),
                                   New JProperty("width", 220),
                                   New JProperty("filterable", False),
                                   New JProperty("format", "{0:0.0}")))
        'prel cumulato unitario full
        KendoColumns.Add(New JObject(New JProperty("field", "prel_cum_uni_full"),
                                   New JProperty("title", "Prelievo Cumulato Unitario previsto FULL"),
                                   New JProperty("width", 260),
                                   New JProperty("filterable", False),
                                   New JProperty("format", "{0:0.0}")))
        'prel cumulato unitario deficit
        KendoColumns.Add(New JObject(New JProperty("field", "prel_cum_uni_deficit"),
                                   New JProperty("title", "Prelievo Cumulato Unitario previsto DEFICIT"),
                                   New JProperty("width", 260),
                                   New JProperty("filterable", False),
                                   New JProperty("format", "{0:0.0}")))

        Return KendoColumns

    End Function

    Private Shared Function GetKendoModelDefinition(ByVal datispecie As List(Of DatiPrelieviCalcoloXSpecie)) As JObject
        'colonne
        Dim KendoModel As New JObject
        'gruppo_consegna
        KendoModel.Add(New JProperty("gruppo_consegna", New JObject(New JProperty("editable", False),
                                     New JProperty("type", "string"))))
        'anno
        KendoModel.Add(New JProperty("anno", New JObject(New JProperty("editable", False),
                                     New JProperty("type", "number"))))
        'settimana
        KendoModel.Add(New JProperty("settimana", New JObject(New JProperty("editable", False),
                                     New JProperty("type", "number"))))
        'pioggia
        KendoModel.Add(New JProperty("pioggia", New JObject(New JProperty("editable", False),
                                     New JProperty("type", "number"))))
        'temp_max
        KendoModel.Add(New JProperty("temp_max", New JObject(New JProperty("editable", False),
                                     New JProperty("type", "number"))))
        'temp_med
        KendoModel.Add(New JProperty("temp_med", New JObject(New JProperty("editable", False),
                                     New JProperty("type", "number"))))
        'temp_min
        KendoModel.Add(New JProperty("temp_min", New JObject(New JProperty("editable", False),
                                     New JProperty("type", "number"))))
        'radiazione_entra
        KendoModel.Add(New JProperty("radiazione_extra", New JObject(New JProperty("editable", False),
                                     New JProperty("type", "number"))))
        'evapotraspirazione_rif
        KendoModel.Add(New JProperty("evapotraspirazione_riferimento", New JObject(New JProperty("editable", False),
                                     New JProperty("type", "number"))))

        For Each dati_specie In datispecie
            ''''loop x specie
            'Coeff evapotrasp
            'Coeff deficit
            'fabbisogno irriguo
            'fabbisonocumulato_full
            'fabbisonocumulato_deficit

            KendoModel.Add(New JProperty("SU_" + dati_specie.veg_cod.ToString(), New JObject(New JProperty("editable", False),
                                     New JProperty("type", "number"))))

            KendoModel.Add(New JProperty("CE_" + dati_specie.veg_cod.ToString(), New JObject(New JProperty("editable", False),
                                     New JProperty("type", "number"))))

            KendoModel.Add(New JProperty("CD_" + dati_specie.veg_cod.ToString(), New JObject(New JProperty("editable", False),
                                     New JProperty("type", "number"))))

            KendoModel.Add(New JProperty("FI_" + dati_specie.veg_cod.ToString(), New JObject(New JProperty("editable", False),
                                     New JProperty("type", "number"))))

            KendoModel.Add(New JProperty("FI_CUM_FULL_" + dati_specie.veg_cod.ToString(), New JObject(New JProperty("editable", False),
                                     New JProperty("type", "number"))))

            KendoModel.Add(New JProperty("FI_CUM_DEF_" + dati_specie.veg_cod.ToString(), New JObject(New JProperty("editable", False),
                                     New JProperty("type", "number"))))

        Next

        'prelievo cumulato fornitore
        KendoModel.Add(New JProperty("prel_cum_fornitore", New JObject(New JProperty("editable", False),
                                     New JProperty("type", "number"))))

        'prelievo cumulato settimanale fornitore
        KendoModel.Add(New JProperty("prel_cum_sett_fornitore", New JObject(New JProperty("editable", False),
                                     New JProperty("type", "number"))))
        'cumulato unitario
        KendoModel.Add(New JProperty("prel_cum_unitario_oss", New JObject(New JProperty("editable", False),
                                     New JProperty("type", "number"))))
        'prel cumulato full
        KendoModel.Add(New JProperty("prel_cum_full", New JObject(New JProperty("editable", False),
                                     New JProperty("type", "number"))))
        'prel cumulato deficit
        KendoModel.Add(New JProperty("prel_cum_deficit", New JObject(New JProperty("editable", False),
                                     New JProperty("type", "number"))))
        'prel cumulato unitario full
        KendoModel.Add(New JProperty("prel_cum_uni_full", New JObject(New JProperty("editable", False),
                                     New JProperty("type", "number"))))
        'prel cumulato unitario deficit
        KendoModel.Add(New JProperty("prel_cum_uni_deficit", New JObject(New JProperty("editable", False),
                                     New JProperty("type", "number"))))

        Return KendoModel

    End Function

    Private Shared Function GetKendoRows(ByVal dati As List(Of DatiPrelieviCalcolo), ByVal id_gruppo As String) As JArray
        'colonne
        Dim KendoRows As New JArray

        For Each dat In dati
            Dim KendoRow As New JObject
            'gruppo_consegna
            KendoRow.Add(New JProperty("gruppo_consegna", id_gruppo))
            'anno
            KendoRow.Add(New JProperty("anno", dat.anno))
            'settimana
            KendoRow.Add(New JProperty("settimana", dat.settimana))
            'pioggia
            KendoRow.Add(New JProperty("pioggia", dat.pioggia))
            'temp_max
            KendoRow.Add(New JProperty("temp_max", dat.temp_max))
            'temp_med
            KendoRow.Add(New JProperty("temp_med", dat.temp_media))
            'temp_min
            KendoRow.Add(New JProperty("temp_min", dat.temp_min))
            'radiazione_entra
            KendoRow.Add(New JProperty("radiazione_extra", dat.radiazione_extra))
            'evapotraspirazione_rif
            KendoRow.Add(New JProperty("evapotraspirazione_riferimento", dat.evapotraspirazione_riferimento))

            For Each dati_specie In dat.DatiXSpecie
                ''''loop x specie
                'Coeff evapotrasp
                'Coeff deficit
                'fabbisogno irriguo
                'fabbisonocumulato_full
                'fabbisonocumulato_deficit
                KendoRow.Add(New JProperty("SU_" + dati_specie.veg_cod.ToString(), dati_specie.SuperficieTotaleColtura))

                KendoRow.Add(New JProperty("CE_" + dati_specie.veg_cod.ToString(), dati_specie.CoeffEvapotraspirazione))

                KendoRow.Add(New JProperty("CD_" + dati_specie.veg_cod.ToString(), dati_specie.CoefficienteDeficit))

                KendoRow.Add(New JProperty("FI_" + dati_specie.veg_cod.ToString(), dati_specie.FabbisognoIdrico))

                KendoRow.Add(New JProperty("FI_CUM_FULL_" + dati_specie.veg_cod.ToString(), dati_specie.PrevisioneCumulataFullIrrigation))

                KendoRow.Add(New JProperty("FI_CUM_DEF_" + dati_specie.veg_cod.ToString(), dati_specie.PrevisioneCumulataDeficitIrrigation))

            Next

            'prelievo cumulato fornitore
            KendoRow.Add(New JProperty("prel_cum_fornitore", dat.PrelievoIdricoCumulatoFornitore))
            'prelievo cumulato settimanale fornitore
            KendoRow.Add(New JProperty("prel_cum_sett_fornitore", dat.PrelievoIdricoSettimanaleFornitore))
            'cumulato unitario
            KendoRow.Add(New JProperty("prel_cum_unitario_oss", dat.PrelievoIdricoCumulatoUnitarioOsservato))
            'prel cumulato full
            KendoRow.Add(New JProperty("prel_cum_full", dat.FabbisognoIdricoCumulato_FULL))
            'prel cumulato deficit
            KendoRow.Add(New JProperty("prel_cum_deficit", dat.FabbisognoIdricoCumulato_DEFICIT))
            'prel cumulato unitario full
            KendoRow.Add(New JProperty("prel_cum_uni_full", dat.PrelievoIdricoCumulatoUnitarioPrevisto_FULL))
            'prel cumulato unitario deficit
            KendoRow.Add(New JProperty("prel_cum_uni_deficit", dat.PrelievoIdricoCumulatoUnitarioPrevisto_DEFICIT))

            KendoRows.Add(KendoRow)
        Next

        Return KendoRows

    End Function



    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub



#Region "Stubs"
    Public Class GruppiConsegna
        Public Property Codice As String
        Public Property Descrizione As String

        Public Sub New()
            Codice = ""
            Descrizione = ""
        End Sub
    End Class
#End Region

End Class
