Imports System.Web.Script.Serialization
Imports System.Web.Services
Imports System.Xml
Imports AgroAgenda_2010.Resources
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreModelliPrevisionaliBIZ
Imports AgronicaCoreVarieBIZ
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports AgronicaCoreWebService.Metos_WS
Imports AgronicaCoreMeteoBiz
Imports AgronicaCoreModello.ParametriAgenda_Temp
Imports AgronicaCoreMeteoCommon
Imports AgronicaCoreWebService.MeteoNT
Imports AgronicaCoreMeteoCommon.OutputRisultatoIndicatori
Imports AgronicaCoreDTOStd.InData.Agenda
Imports AgronicaCoreModelliPrevisionaliCommon
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreEFatturaDAL

Public Class MeteoWS
    Inherits System.Web.UI.Page


    Private Shared Function GetDoorkey() As String
        Dim Doorkey As String = "Y4h8u3B5w2"
        Return Doorkey
    End Function

    Protected Overrides Sub InitializeCulture()
        MyBase.InitializeCulture()
        Lingua.Gias_InizializzaCultura_DaSession()

    End Sub


    <WebMethod(EnableSession:=True)>
    Public Shared Function SpecieCaricaComboModelli() As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Lingua.Gias_InizializzaCultura_DaSession()

        Try

            Dim xWS As New AgronicaCoreWebService.Meteo

            Dim objParametriAgenda As New AgronicaCoreModello.ParametriAgenda_Temp.ParametriAgenda

            Dim piva As String = HttpContext.Current.Session("_piva")

            If String.IsNullOrEmpty(piva) Then
                piva = objParametriAgenda.Piva
            End If

            Dim ss As String = xWS.SpeciexModelli_Inizializza(piva, objParametri_Server)

            ss = ss.Replace("{""SpeciexModelli_InizializzaResult"":", "")
            ss = ss.Substring(0, ss.Length - 1)

            Dim jss As New JavaScriptSerializer
            Dim obj = jss.Deserialize(Of RispostaStandard)(ss)

            r.RispostaStringa = obj.RispostaStringa

            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r
    End Function


    <WebMethod(EnableSession:=True)>
    Public Shared Function SpecieCaricaCombo() As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Lingua.Gias_InizializzaCultura_DaSession()

        Try

            Dim cmbSpecie As New AgronicaControlli_2010.ComboSpecie_con_Tutte
            cmbSpecie.ddl_Specie = New DropDownList()
            cmbSpecie.CaricaComboSpecie()

            r.RispostaOK = True
            r.RispostaStringa = AgronicaCoreUtility.CaricaListControl.itemsCollection_AsJsonArray(cmbSpecie.ddl_Specie.Items, "veg_cod", "veg_des")

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r
    End Function


    Private Shared Function SpeciePsDefaultValue(ByVal vegCod As String, ByVal Dt As DataTable) As String

        Dim rval As String

        rval = (
            From dd In Dt.AsEnumerable()
            Where dd("Veg_cod") = vegCod
            Select CStr(dd("Soglia_Minima"))
        ).FirstOrDefault()

        Return rval


    End Function


    <WebMethod(EnableSession:=True)>
    Public Shared Function SorgentiMeteo() As RispostaStandard

        Dim risp As New RispostaStandard With {.RispostaOK = False}

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Super_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Super_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            risp.Sessione = False
            Return risp
        End If

        Lingua.Gias_InizializzaCultura_DaSession()

        Try

            Dim objParams As New JObject(New JProperty("PIVA_Superuser", objParametri_Server.PivaSuperUser))

            'Dim objMeteoNT As New AgronicaCoreWebService.MeteoNT
            Dim objMeteoSuite As New InterfacciaMeteoSuite(objParametri_Server, objParametri_Super_Server)

            'Dim json_risp = JObject.Parse(objMeteoNT.ElencoSorgentiMeteo(objParams, objParametri_Server))
            Dim json_risp = JObject.Parse(objMeteoSuite.ElencoSorgentiMeteo(objParams))

            Dim elenco = JArray.Parse(json_risp("RispostaStringa"))

            'Alias gestito diversamente (da tabella in MeteoSuite)
            'For Each s As JObject In elenco

            '    Try

            '        Dim resx As String = "SorgenteMeteo_" & CInt(s("sorgente_cod")).ToString("000")
            '        Dim out_des As String = AgronicaAgenda_2010.ResourceManager.GetString(resx, AgronicaAgenda_2010.Culture)
            '        If out_des IsNot Nothing Then

            '            s("sorgente_des") = out_des
            '        End If

            '    Catch ex As Exception

            '    End Try
            'Next

            risp.RispostaStringa = elenco.ToString()
            risp.RispostaOK = True

        Catch ex As Exception

            'uso questa funzione per ottenere il Messaggio..:
            risp.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return risp
    End Function


    <WebMethod(EnableSession:=True)>
    Public Shared Function RicaricaOrigineDati(ByVal TipoSorgenteDati As Integer, ByVal latcentro As Double, ByVal longcentro As Double) As RispostaStandard
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

            Dim PivaVisibilita_ClientGiasImpostata As String = HttpContext.Current.Session("_piva")


            Dim objMeteo As New AgronicaCoreWebService.Meteo
            objMeteo.Chiama_WS_Meteo_CaricaCombo_StazioniMeteoConDistanza(
                    cmbSogenti, objParametri_Server.PivaSuperUser,
                    longcentro, latcentro,
                    False, "", "", "", "", HttpContext.Current.Session("ASG_Utente_Username"), PivaVisibilita_ClientGiasImpostata,
                    HttpContext.Current.Session("ASG_Utente_Username_Crypt"),
                    HttpContext.Current.Session("ASG_Utente_Password_Crypt"),
                    GetDoorkey(),
                    TipoSorgenteDati,
                    Nothing,
                    objParametri_Server
                )

            r.RispostaOK = True
            r.RispostaStringa = AgronicaCoreUtility.CaricaListControl.itemsCollection_AsJsonArray(cmbSogenti.Items, "sorgente_cod", "sorgente_des")

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r
    End Function


    <WebMethod(EnableSession:=True)>
    Public Shared Function RicaricaOrigineDati2(ByVal TipoSorgenteDati As Integer, ByVal latcentro As Double, ByVal longcentro As Double) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Dim objParametri_Super_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Super_Server")
        If IsNothing(objParametri_Super_Server) Then
            r.Sessione = False
            Return r
        End If

        Lingua.Gias_InizializzaCultura_DaSession()

        Try

            Dim piva As String = HttpContext.Current.Session("_piva")

            If String.IsNullOrEmpty(piva) Then
                Dim objParametriAgenda As New AgronicaCoreModello.ParametriAgenda_Temp.ParametriAgenda
                piva = objParametriAgenda.Piva
            End If

            '*************************************************************
            'GABRIELE METEONT
            'Dim PivaVisibilita_ClientGiasImpostata As String = HttpContext.Current.Session("_piva")
            'Dim objMeteo As New AgronicaCoreWebService.Meteo
            'Dim DT_Stazioni = objMeteo.Chiama_WS_Meteo_CaricaCombo_StazioniMeteoConDistanza2(
            '    objParametri_Server.PivaSuperUser,
            '    longcentro, latcentro,
            '    "", "", HttpContext.Current.Session("ASG_Utente_Username"), PivaVisibilita_ClientGiasImpostata,
            '    HttpContext.Current.Session("ASG_Utente_Username_Crypt"),
            '    HttpContext.Current.Session("ASG_Utente_Password_Crypt"),
            '    GetDoorkey(),
            '    TipoSorgenteDati,
            '    objParametri_Server)
            '*************************************************************

            'Dim objMeteo As New AgronicaCoreWebService.MeteoNT
            'Dim DT_Stazioni = objMeteo.ElencoStazioniConDistanza(objParametri_Server.PivaSuperUser,
            '                                                     GetDoorkey(),
            '                                                     HttpContext.Current.Session("ASG_Utente_Username"),
            '                                                     HttpContext.Current.Session("ASG_Utente_Username_Crypt"),
            '                                                     HttpContext.Current.Session("ASG_Utente_Password_Crypt"),
            '                                                     piva,
            '                                                     objParametri_Server,
            '                                                     latcentro,
            '                                                     longcentro,
            '                                                     TipoSorgenteDati)
            Dim objMeteo As New InterfacciaMeteoSuite(objParametri_Server, objParametri_Super_Server)
            Dim DT_Stazioni = objMeteo.ElencoStazioniConDistanza(objParametri_Server.PivaSuperUser,
                                                                 GetDoorkey(),
                                                                 HttpContext.Current.Session("ASG_Utente_Username"),
                                                                 HttpContext.Current.Session("ASG_Utente_Username_Crypt"),
                                                                 HttpContext.Current.Session("ASG_Utente_Password_Crypt"),
                                                                 piva,
                                                                 latcentro,
                                                                 longcentro,
                                                                 TipoSorgenteDati)

            Dim arrStazioni As New JArray

            Dim distanza As String
            Dim descrizione As String
            Dim nome As String
            Dim sorgente As String
            Dim aggiornamento As String
            Dim fornitore As String
            Dim rif_fornitore As String

            If Not DT_Stazioni Is Nothing AndAlso DT_Stazioni.Rows.Count > 0 Then

                'l'ordinamento (NULL LAST) viene dalla query
                For Each src_row In DT_Stazioni.Rows

                    Dim objStazione As New JObject

                    distanza = AgronicaAgenda_2010.Distanza + " "
                    If IsDBNull(src_row("Distanza")) OrElse CInt(src_row("Distanza")) = CInt("-1") Then
                        distanza &= AgronicaAgenda_2010.NonDisponibile.ToLower()
                    Else
                        distanza &= CInt(CInt(src_row("Distanza")) / 1000) & " km"
                    End If

                    descrizione = src_row("descrizione")
                    nome = src_row("descrizione")
                    sorgente = nome
                    aggiornamento = ""

                    '*************************************************************
                    'GABRIELE METEONT
                    'If CDate(src_row("data_ultimo_agg")) > AGRODATAINIZIO Then
                    '    aggiornamento = " (agg. al " & CDate(src_row("data_ultimo_agg")).ToShortDateString() & ")"
                    'End If
                    '*************************************************************

                    If Not IsDBNull(src_row("data_ultimo_agg")) Then
                        aggiornamento = " (" & AgronicaAgenda_2010.AggiornamentoAlAbbr & " " & CDate(src_row("data_ultimo_agg")).ToShortDateString() & ")"
                    End If

                    '*************************************************************
                    'GABRIELE METEONT
                    'fornitore = src_row("Stazione_Cod_Fornitore").ToString
                    '*************************************************************
                    fornitore = src_row("Fornitore").ToString
                    If Not String.IsNullOrEmpty(fornitore) Then
                        rif_fornitore = src_row("rif_fornitore").ToString
                        If Not String.IsNullOrEmpty(rif_fornitore) Then
                            Dim idx = rif_fornitore.IndexOf(":")
                            If idx > 1 Then
                                rif_fornitore = Left(rif_fornitore, idx - 1)
                            End If
                            rif_fornitore = " - " & rif_fornitore
                        Else
                            rif_fornitore = ""
                        End If

                        fornitore = "[" & fornitore & rif_fornitore & "] "
                    End If

                    '*************************************************************
                    'GABRIELE METEONT
                    'descrizione &= aggiornamento & fornitore & " (" & distanza & ")"
                    '*************************************************************
                    descrizione = fornitore & descrizione & aggiornamento & " (" & distanza & ")"
                    nome = fornitore & nome & aggiornamento
                    If Not String.IsNullOrEmpty(fornitore) Then
                        sorgente &= " "
                    End If
                    sorgente &= fornitore

                    objStazione("sorgente_des") = descrizione
                    '*************************************************************
                    'GABRIELE METEONT
                    'objStazione("sorgente_cod") = src_row("nome").ToString
                    '*************************************************************
                    objStazione("sorgente_cod") = src_row("id").ToString
                    objStazione("nome") = nome
                    objStazione("sorgente") = sorgente
                    objStazione("distanza") = distanza

                    objStazione("stazione") = src_row("descrizione").ToString
                    objStazione("fornitore") = src_row("fornitore").ToString
                    objStazione("rif_fornitore") = src_row("rif_fornitore").ToString

                    arrStazioni.Add(objStazione)
                Next

            End If

            r.RispostaOK = True
            r.RispostaStringa = arrStazioni.ToString()

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)


        End Try

        Return r
    End Function


    <WebMethod(EnableSession:=True)>
    Public Shared Function DatiMeteo_Elabora(DataDa As String, DataA As String, FrequenzaDati As String, TipoSorgente As String, Sorgente As String, SogliaTemp As Decimal, SogliaFabbisognoFreddo As Decimal, CfrStorico As String) As rispostaStandard(Of RisultatoMeteo)

        Dim r As New rispostaStandard(Of RisultatoMeteo)

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Super_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Super_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) OrElse IsNothing(objParametri_Super_Server) Then
            r.Sessione = False
            Return r
        End If

        Try
            Dim objParams As New JObject

            objParams("TipoSorgente") = TipoSorgente
            objParams("Sorgente") = Sorgente
            objParams("DataInizio") = DataDa
            objParams("DataFine") = DataA
            objParams("FrequenzaDati") = FrequenzaDati
            objParams("SogliaTemp") = SogliaTemp
            objParams("SogliaFreddo") = SogliaFabbisognoFreddo
            objParams("CfrStorico") = CfrStorico

            Dim linguaSession As Lingua = CType(System.Web.HttpContext.Current.Session("LinguaCorrente"), Lingua)
            objParams("LinguaCodiceISO") = linguaSession.CodiceISO

            'Dim objMeteoNT As New AgronicaCoreWebService.MeteoNT
            'Dim ss As String = objMeteoNT.DatiMeteoElabora(objParams, objParametri_Server)

            Dim objMeteoSuite As New InterfacciaMeteoSuite(objParametri_Server, objParametri_Super_Server)
            Dim ss As String = objMeteoSuite.DatiMeteoElabora(objParams)

            Dim jss = New JavaScriptSerializer With {
                .MaxJsonLength = Integer.MaxValue
            }
            r = jss.Deserialize(Of rispostaStandard(Of RisultatoMeteo))(ss)

            r.RispostaStringa.Meteo_Charts = EnhanceChart(TipoSorgente, Sorgente, r.RispostaStringa.Meteo_Charts)

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r
    End Function


    Private Shared Function EnhanceChart(tipoSorgente As String, sorgente As String, jsonChart As String) As String

        'Arricchisco i grafici se devo....

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            Return jsonChart
        End If

        Dim piva As String = HttpContext.Current.Session("_piva")

        If String.IsNullOrEmpty(piva) Then
            Dim objParametriAgenda As New AgronicaCoreModello.ParametriAgenda_Temp.ParametriAgenda
            piva = objParametriAgenda.Piva
        End If

        Try

            Dim SxC As New AgronicaCoreMeteoDAL.DSS_Stazioni_X_Controllo_Reader

            Dim filtroAggiuntivo As New AgronicaCoreMeteoDAL.DSS_Stazioni_X_Controllo_Reader.Filter With {
                .TipoSorgente = tipoSorgente,
                .StazioneCod = sorgente
            }

            Dim dtSxC = SxC.Leggi(piva, filtroAggiuntivo, objParametri_Server)

            'Dim SxC As New AgronicaCoreMeteoDAL.DSS_Stazioni_X_Controllo_R

            'Dim xFiltroAggiuntivo = "Tipo_Sorgente = " & tipoSorgente & " AND Stazione_Cod = " & sorgente

            'Dim dtSxC = SxC.Leggi(piva, xFiltroAggiuntivo, "", objParametri_Server)

            If dtSxC IsNot Nothing AndAlso dtSxC.Rows.Count > 0 Then

                Dim parametri = dtSxC.Rows(0)("Parametri").ToString
                If Not String.IsNullOrEmpty(parametri) Then

                    Dim parObj = JObject.Parse(parametri)
                    If parObj("sensore_15") IsNot Nothing Then

                        Dim soglia_inf As Decimal = parObj("sensore_15")("soglia_inf")
                        Dim soglia_sup As Decimal = parObj("sensore_15")("soglia_sup")

                        Dim needSerialize As Boolean = False

                        Dim chartArr = JArray.Parse(jsonChart)
                        For Each chart As JObject In chartArr

                            Dim axis As JArray = chart("axis")
                            If axis.Count = 1 Then
                                'unica asse verticale

                                Dim series As JArray = chart("series")
                                Dim ser As JObject = series(0)
                                'tutte le serie hanno la stessa asse verticale

                                If CInt(ser("tipo_sensore")) = 15 Then

                                    Dim axis_with_bands As New JObject(New JProperty("axes", axis))

                                    Dim bands As New JArray From {
                                        New JObject(New JProperty("StopValue", soglia_inf), New JProperty("Color", "rgb(255, 0, 0)"), New JProperty("Opacity", 0.2)),
                                        New JObject(New JProperty("StopValue", soglia_sup), New JProperty("Color", "rgb(0, 128, 0)"), New JProperty("Opacity", 0.2)),
                                        New JObject(New JProperty("StopValue", Integer.MaxValue), New JProperty("Color", "rgb(255, 255, 0)"), New JProperty("Opacity", 0.2))
                                    }

                                    Dim oBands = New JObject(New JProperty("Bands", bands))

                                    axis_with_bands.Add(New JProperty("bands", oBands))

                                    chart("axis") = axis_with_bands

                                    needSerialize = True
                                End If
                            End If
                        Next

                        If needSerialize Then

                            jsonChart = chartArr.ToString(Newtonsoft.Json.Formatting.None)
                        End If
                    End If
                End If
            End If

        Catch ex As Exception

        End Try

        Return jsonChart
    End Function


    <WebMethod(EnableSession:=True)>
    Public Shared Function DatiMeteo_ElaboraRiepilogo(ByVal piva As String) As rispostaStandard(Of AgronicaCoreMeteoBiz.RisultatoMeteoRiepilogo)

        Dim risp As New rispostaStandard(Of AgronicaCoreMeteoBiz.RisultatoMeteoRiepilogo) With {
            .RispostaStringa = New AgronicaCoreMeteoBiz.RisultatoMeteoRiepilogo,
            .RispostaOK = False
        }

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Super_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Super_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) OrElse IsNothing(objParametri_Super_Server) Then
            risp.Sessione = False
            Return risp
        End If

        Lingua.Gias_InizializzaCultura_DaSession()

        Try

            'Dim SxM As New AgronicaCoreMeteoDAL.DSS_Stazioni_X_Monitor_R

            'Dim dtSxM = SxM.Leggi(piva, "", "", objParametri_Server)

            Dim SxM As New AgronicaCoreMeteoDAL.DSS_Stazioni_X_Monitor_Reader

            Dim dtSxM = SxM.Leggi(piva, objParametri_Server)

            If dtSxM IsNot Nothing AndAlso dtSxM.Rows.Count > 0 Then

                'Dim objMeteoNT As New AgronicaCoreWebService.MeteoNT
                Dim objMeteoSuite As New InterfacciaMeteoSuite(objParametri_Server, objParametri_Super_Server)
                Dim jss = New JavaScriptSerializer With {
                    .MaxJsonLength = Integer.MaxValue
                }

                For Each row In dtSxM.Rows

                    Dim objParams As New JObject(
                        New JProperty("PivaSuperuser", objParametri_Server.PivaSuperUser),
                        New JProperty("Piva", piva),
                        New JProperty("TipoSorgente", CInt(row("Tipo_Sorgente"))),
                        New JProperty("Sorgente", CInt(row("Stazione_Cod"))),
                        New JProperty("NumOre", CInt(row("Monitor_Ore")))
                        )

                    'Dim ss As String = objMeteoNT.DatiMeteoElaboraRiepilogo(objParams, objParametri_Server)
                    Dim ss As String = objMeteoSuite.DatiMeteoElaboraRiepilogo(objParams)

                    Dim tmp_r = jss.Deserialize(Of rispostaStandard(Of RisultatoMeteoRiepilogo.Stazione))(ss)

                    If tmp_r.RispostaOK Then

                        risp.RispostaStringa.Stazioni.Add(tmp_r.RispostaStringa)
                    End If
                Next
            End If

            risp.RispostaOK = True

        Catch ex As Exception

            'uso questa funzione per ottenere il Messaggio..:
            risp.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return risp
    End Function


    <WebMethod(EnableSession:=True)>
    Public Shared Function DatiMeteo_ElaboraMonitoraggioSuolo(ByVal piva As String) As rispostaStandard(Of AgronicaCoreMeteoBiz.RisultatoMeteoMonitoraggioSuolo)

        Dim risp As New rispostaStandard(Of AgronicaCoreMeteoBiz.RisultatoMeteoMonitoraggioSuolo) With {
            .RispostaStringa = New AgronicaCoreMeteoBiz.RisultatoMeteoMonitoraggioSuolo,
            .RispostaOK = False
        }

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Super_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Super_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) OrElse IsNothing(objParametri_Super_Server) Then
            risp.Sessione = False
            Return risp
        End If

        Lingua.Gias_InizializzaCultura_DaSession()

        Try

            'Dim SxC As New AgronicaCoreMeteoDAL.DSS_Stazioni_X_Controllo_R

            'Dim dtSxC = SxC.Leggi(piva, "", "", objParametri_Server)

            Dim SxC As New AgronicaCoreMeteoDAL.DSS_Stazioni_X_Controllo_Reader

            Dim dtSxC = SxC.Leggi(piva, Nothing, objParametri_Server)

            If dtSxC IsNot Nothing AndAlso dtSxC.Rows.Count > 0 Then

                'Dim objMeteoNT As New AgronicaCoreWebService.MeteoNT
                Dim objMeteoSuite As New InterfacciaMeteoSuite(objParametri_Server, objParametri_Super_Server)
                Dim jss = New JavaScriptSerializer With {
                    .MaxJsonLength = Integer.MaxValue
                }

                For Each row In dtSxC.Rows

                    Dim par = JObject.Parse(row("Parametri").ToString)

                    Dim objParams As New JObject(
                        New JProperty("PivaSuperuser", objParametri_Server.PivaSuperUser),
                        New JProperty("Piva", piva),
                        New JProperty("TipoSorgente", CInt(row("Tipo_Sorgente"))),
                        New JProperty("Sorgente", CInt(row("Stazione_Cod"))),
                        New JProperty("Parametri", par("sensore_15"))
                        )

                    'Dim ss As String = objMeteoNT.DatiMeteoElaboraMonitoraggioSuolo(objParams, objParametri_Server)
                    Dim ss As String = objMeteoSuite.DatiMeteoElaboraMonitoraggioSuolo(objParams)

                    Dim tmp_r = jss.Deserialize(Of rispostaStandard(Of RisultatoMeteoMonitoraggioSuolo.Stazione))(ss)

                    If tmp_r.RispostaOK Then

                        risp.RispostaStringa.Stazioni.Add(tmp_r.RispostaStringa)
                    End If
                Next
            End If

            risp.RispostaOK = True

        Catch ex As Exception

            'uso questa funzione per ottenere il Messaggio..:
            risp.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return risp
    End Function


    <WebMethod(EnableSession:=True)>
    Public Shared Function ModelliPrevisionali_Elabora(ByVal DataDa As String, ByVal DataA As String,
                                                       ByVal TipoSorgente As String, ByVal Sorgente As String,
                                                       ByVal ModelloPrevisionale As String, ByVal Veg_Cod As String, ByVal Av_Cod As String,
                                                       ByVal Algoritmo As String, ByVal ParametriAggiuntivi As String) As rispostaStandard(Of cRisultatoModello)

        Dim r As New rispostaStandard(Of cRisultatoModello)

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Super_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Super_Server")

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) OrElse IsNothing(objParametri_Super_Server) Then
            r.Sessione = False
            Return r
        End If

        Lingua.Gias_InizializzaCultura_DaSession()
        Dim linguaSession As Lingua = CType(System.Web.HttpContext.Current.Session("LinguaCorrente"), Lingua)

        Try

            Dim objParams As New JObject

            objParams("PivaSuperuser") = objParametri_Server.PivaSuperUser
            objParams("DataInizio") = DataDa
            objParams("DataFine") = DataA
            objParams("TipoSorgente") = TipoSorgente
            objParams("Sorgente") = Sorgente
            objParams("Mod_Cod") = ModelloPrevisionale
            objParams("Veg_Cod") = Veg_Cod
            objParams("Av_Cod") = Av_Cod
            objParams("Algoritmo") = Algoritmo
            objParams("LinguaCodiceISO") = linguaSession.CodiceISO
            objParams("ParametriElaborazioneAggiuntivi") = JObject.Parse(ParametriAggiuntivi)

            'Dim objMeteo As New AgronicaCoreWebService.MeteoNT
            'Dim ss As String = objMeteo.ModelliPrevisionaliElabora(objParams, objParametri_Server)
            Dim objDifesa As New DSS_Difesa_ModelliPrevisionali(objParametri_Server, objParametri_Super_Server)
            Dim ss As String = objDifesa.ModelliPrevisionaliElabora(objParams)
            Dim jss = New JavaScriptSerializer With {
                .MaxJsonLength = Integer.MaxValue
            }
            r = jss.Deserialize(Of rispostaStandard(Of cRisultatoModello))(ss)

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function


    <WebMethod(EnableSession:=True)>
    Public Shared Function StazioniDaAnagrafica(ByVal piva As String) As RispostaStandard

        Dim r As New RispostaStandard
        r.RispostaOK = False

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Lingua.Gias_InizializzaCultura_DaSession()

        Dim jsonArray As String = "["

        Try

            'Dalla piva leggo la/le stazioni meteo associate di default
            Dim objLettura As New AgronicaCoreMeteoDAL.DSS_Centri_Aziendali_Agronica_Stazioni_Meteo_R

            Dim xml = objLettura.Leggi_ElencoStazioni(piva, objParametri_Server)

            If Not String.IsNullOrEmpty(xml) Then

                Dim flagPrimo = True

                Dim xD As XDocument = XDocument.Parse(xml)
                For Each staz In xD.Elements("ElencoStazioni").Elements("Stazione").ToList

                    Dim TipoSorgente As String = staz.Element("tipo_sorgente").Value()
                    Dim Sorgente As String = staz.Element("Stazione_Cod").Value()

                    If Not flagPrimo Then
                        jsonArray &= ", "
                    End If
                    flagPrimo = False

                    Dim jss = New JavaScriptSerializer()
                    jsonArray &= jss.Serialize(New With {
                                               .Tipo_Sorgente = TipoSorgente,
                                               .Stazione_Cod = Sorgente
                                               })
                Next

            End If

            r.RispostaOK = True

        Catch ex As Exception

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        jsonArray &= "]"
        r.RispostaStringa = jsonArray

        Return r

    End Function


    Private Shared Function OrderTipoSorgenteMeteo(ByVal ts As Integer) As Integer
        Dim retval As Integer = -1
        Select Case ts
            Case enum_Meteo_Tiposorgente.Aziendali
                retval = 0
            Case enum_Meteo_Tiposorgente.RetiPartner
                retval = 1
            Case enum_Meteo_Tiposorgente.Gias_RER
                retval = 2
            Case enum_Meteo_Tiposorgente.Gias_RER_Quadranti
                retval = 3
        End Select
        Return retval
    End Function


    <WebMethod(EnableSession:=True)>
    Public Shared Function ModelliPrevisionaliDatoVeg_Cod(ByVal Veg_Cod As String) As rispostaStandard(Of List(Of AgronicaCoreModelliPrevisionaliBIZ.cModelloPrevisionale))
        Return ModelliPrevisionaliDatoVeg_Cod_av_cod(Veg_Cod, 0)
    End Function


    <WebMethod(EnableSession:=True)>
    Public Shared Function ModelliPrevisionaliDatoVeg_Cod_av_cod(ByVal Veg_Cod As String, ByVal Av_Cod As String) As rispostaStandard(Of List(Of AgronicaCoreModelliPrevisionaliBIZ.cModelloPrevisionale))

        Dim r As New rispostaStandard(Of List(Of AgronicaCoreModelliPrevisionaliBIZ.cModelloPrevisionale))

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Lingua.Gias_InizializzaCultura_DaSession()

        Try

            Dim xWS As New AgronicaCoreWebService.Meteo

            Dim ss As String = xWS.ModelliPrevisionali_Lista(Veg_Cod, Av_Cod, objParametri_Server)

            Dim jss = New JavaScriptSerializer()
            ss = ss.Replace("{""ModelliPrevisionali_ElencoResult"":", "")
            ss = ss.Substring(0, ss.Length - 1)
            r = jss.Deserialize(Of rispostaStandard(Of List(Of AgronicaCoreModelliPrevisionaliBIZ.cModelloPrevisionale)))(ss)


            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)


        End Try

        Return r
    End Function


    <WebMethod(EnableSession:=True)>
    Public Shared Function AvModAlg_Inizializza(ByVal Veg_Cod As String) As rispostaStandard(Of List(Of AgronicaCoreModelliPrevisionaliBIZ.cAvModAlg))

        Dim r As New rispostaStandard(Of List(Of AgronicaCoreModelliPrevisionaliBIZ.cAvModAlg))

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If


        Try

            Dim xWS As New AgronicaCoreWebService.Meteo

            Dim objParametriAgenda As New AgronicaCoreModello.ParametriAgenda_Temp.ParametriAgenda

            Dim piva As String = HttpContext.Current.Session("_piva")

            If String.IsNullOrEmpty(piva) Then
                piva = objParametriAgenda.Piva
            End If

            Dim ss As String = xWS.AvModAlg_Inizializza(piva, Veg_Cod, objParametri_Server)

            Dim jss = New JavaScriptSerializer()
            r = jss.Deserialize(Of rispostaStandard(Of List(Of AgronicaCoreModelliPrevisionaliBIZ.cAvModAlg)))(ss)

            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)


        End Try

        Return r
    End Function


    <WebMethod(EnableSession:=True)>
    Public Shared Function LeggiModelliAutorizzati(ByVal Veg_Cod As Integer) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Super_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Super_Server")

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) OrElse IsNothing(objParametri_Super_Server) Then
            r.Sessione = False
            Return r
        End If

        Try

            Dim objParametriAgenda As New AgronicaCoreModello.ParametriAgenda_Temp.ParametriAgenda

            Dim piva As String = HttpContext.Current.Session("_piva")

            If String.IsNullOrEmpty(piva) Then
                piva = objParametriAgenda.Piva
            End If

            Dim objParams As New JObject
            objParams("Doorkey") = GetDoorkey()
            objParams("PIVA_Superuser") = objParametri_Server.PivaSuperUser
            objParams("PIVA") = piva
            objParams("Veg_Cod") = Veg_Cod

            'Dim objMeteo As New AgronicaCoreWebService.MeteoNT
            'Dim json = JObject.Parse(objMeteo.LeggiModelliAutorizzati(objParams, objParametri_Server))
            Dim objDifesa As New DSS_Difesa_ModelliPrevisionali(objParametri_Server, objParametri_Super_Server)
            Dim json = JObject.Parse(objDifesa.LeggiModelliAutorizzati(objParams))
            Dim jarr = JArray.Parse(json("RispostaStringa").ToString)

            Dim elencomodelli_helper As New ElencoModelliAutorizzati
            r.RispostaStringa = elencomodelli_helper.Genera(jarr, objParametri_Server)
            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r
    End Function


    <WebMethod(EnableSession:=True)>
    Public Shared Function LeggiModelliXImpostazioni() As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Super_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Super_Server")

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) OrElse IsNothing(objParametri_Super_Server) Then
            r.Sessione = False
            Return r
        End If

        Try

            Dim objParametriAgenda As New AgronicaCoreModello.ParametriAgenda_Temp.ParametriAgenda

            Dim objParams As New JObject
            objParams("Doorkey") = GetDoorkey()
            objParams("PIVA_Superuser") = objParametri_Server.PivaSuperUser

            'Dim objMeteo As New AgronicaCoreWebService.MeteoNT
            Dim objDifesa As New DSS_Difesa_ModelliPrevisionali(objParametri_Server, objParametri_Super_Server)

            'Dim json = JObject.Parse(objMeteo.LeggiTuttiModelliAutorizzati(objParams, objParametri_Server))
            Dim json = JObject.Parse(objDifesa.LeggiTuttiModelliAutorizzati(objParams))

            Dim jarr = JArray.Parse(json("RispostaStringa").ToString)

            Dim elencomodelli_helper As New ElencoModelliAutorizzati

            r.RispostaStringa = elencomodelli_helper.Genera(jarr, objParametri_Server)

            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r
    End Function


    <WebMethod(EnableSession:=True)>
    Public Shared Function ScriviModelliXImpostazioni(listaImpostazioni As List(Of AgronicaCoreMeteoDAL.DSS_ModelliImpostazioni.impostazioneModello)) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try
            r.RispostaOK = (New AgronicaCoreMeteoDAL.DSS_ModelliImpostazioni).Upsert(listaImpostazioni, objParametri_Server)

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r
    End Function


    <WebMethod(EnableSession:=True)>
    Public Shared Function ModelliPrevisionali_Elenco_Algoritmi(ByVal modello As String) As rispostaStandard(Of List(Of AgronicaCoreModelliPrevisionaliBIZ.cAlgoritmo))

        Dim r As New rispostaStandard(Of List(Of AgronicaCoreModelliPrevisionaliBIZ.cAlgoritmo))

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Lingua.Gias_InizializzaCultura_DaSession()

        Try

            Dim xWS As New AgronicaCoreWebService.Meteo

            Dim ss As String = xWS.ModelliPrevisionali_Elenco_Algoritmi(modello, objParametri_Server)

            Dim jss = New JavaScriptSerializer()
            ss = ss.Replace("{""ModelliPrevisionali_Elenco_AlgoritmiResult"":", "")
            ss = ss.Substring(0, ss.Length - 1)
            r = jss.Deserialize(Of rispostaStandard(Of List(Of AgronicaCoreModelliPrevisionaliBIZ.cAlgoritmo)))(ss)


            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)


        End Try

        Return r
    End Function


    <WebMethod(EnableSession:=True)>
    Public Shared Function ModelliPrevisionali_Elenco_Avversita(ByVal Veg_Cod As String) As rispostaStandard(Of List(Of AgronicaCoreModelliPrevisionaliBIZ.cAvversita))

        Dim r As New rispostaStandard(Of List(Of AgronicaCoreModelliPrevisionaliBIZ.cAvversita))

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Lingua.Gias_InizializzaCultura_DaSession()

        Try

            Dim xWS As New AgronicaCoreWebService.Meteo

            Dim ss As String = xWS.ModelliPrevisionali_Elenco_Avversita(Veg_Cod, objParametri_Server)

            Dim jss = New JavaScriptSerializer()
            ss = ss.Replace("{""ModelliPrevisionali_Elenco_AvversitaResult"":", "")
            ss = ss.Substring(0, ss.Length - 1)
            r = jss.Deserialize(Of rispostaStandard(Of List(Of AgronicaCoreModelliPrevisionaliBIZ.cAvversita)))(ss)


            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)


        End Try

        Return r
    End Function


    <WebMethod(EnableSession:=True)>
    Public Shared Function ParametriAnalisi_Elenco() As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Lingua.Gias_InizializzaCultura_DaSession()

        Try

            Dim xLetturaElaborato As New AgronicaCoreContabDAL.ElaborazioneDati_R

            Dim DataDa As New DateTime()
            Dim DataA As New DateTime()
            Dim dtOperazioni As DataTable = xLetturaElaborato.OperazioneAgendaElaborabiliLista(DataDa, DataA, "", "", objParametri_Server)

            Dim elenco = (From dd In dtOperazioni.AsEnumerable
                          Select New With {
                              .lav_cod = dd("DSS_Analisi_tipologia_elaborazione") & "|" & dd("lav_cod"),
                              .lav_des = dd("LAV_DES")
                              }).ToList()

            Dim serializerSettings As New JsonSerializerSettings With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}
            r.RispostaStringa = JsonConvert.SerializeObject(elenco, Newtonsoft.Json.Formatting.None, serializerSettings)
            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)


        End Try

        Return r
    End Function


    ''' <summary>
    ''' Avvia lettura ed elaborazione dei dati
    ''' </summary>
    ''' <param name="DataDa"></param>
    ''' <param name="DataA"></param>
    ''' <param name="TipoElaborazione">Tipo di elaborazione, da quaderno, analisi PDC ecc...</param>
    ''' <param name="SchemaDiElaborazione">Lo schema varia in base al tipo di elaborazione, es..: sul quaderno coicide con il Lav_Cod</param>
    ''' <returns></returns>
    <WebMethod(EnableSession:=True)>
    Public Shared Function KendoPivotInizializza_Curve(ByVal DataDa As String, ByVal DataA As String, ByVal DSS_Analisi_tipologia_elaborazione As Integer, ByVal SchemaDiElaborazione As Integer, ByVal IDTestataTemp As Integer) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Lingua.Gias_InizializzaCultura_DaSession()

        Try

            Dim xLetturaElaborato As New AgronicaCoreContabDAL.ElaborazioneDati_R

            Dim dtDatiElaboratiLetti As DataTable

            Select Case DSS_Analisi_tipologia_elaborazione
                Case enum_DataAnalisiBi_DSS_Analisi_tipologia_elaborazione.OperazioniDaQuaderno
                    dtDatiElaboratiLetti = xLetturaElaborato.ElaborazioneDatiQuadernoDiCampagna(DataDa, DataA, SchemaDiElaborazione, "", "", objParametri_Server, IDTestataTemp)

                Case enum_DataAnalisiBi_DSS_Analisi_tipologia_elaborazione.AnalisiJoinPDC
                    dtDatiElaboratiLetti = xLetturaElaborato.AnalisiLetteConJoinPDC(DataDa, DataA, SchemaDiElaborazione, "", "", objParametri_Server, IDTestataTemp)


                Case enum_DataAnalisiBi_DSS_Analisi_tipologia_elaborazione.AnalsiDelTerreno
                    dtDatiElaboratiLetti = xLetturaElaborato.AnalisiLetteSenzaPDC(DataDa, DataA, "", "", objParametri_Server, IDTestataTemp)

                Case Else
                    r.RispostaOK = False
                    r.Errore = DirectCast(System.Web.HttpContext.GetLocalResourceObject("~/DataAnalisiBI/Meteo/Meteo.aspx", "TipoDiElaborazioneNonRiconosciuto"), String)
                    Return r
            End Select

            If Not dtDatiElaboratiLetti Is Nothing AndAlso dtDatiElaboratiLetti.Rows.Count = 0 Then
                r.RispostaOK = False
                r.Errore = DirectCast(System.Web.HttpContext.GetLocalResourceObject("~/DataAnalisiBI/Meteo/Meteo.aspx", "NessunDatoTrovatoProvareConCriteriRicercaDiversi"), String)
                Return r
            End If

            Dim misure = Nothing

            If DSS_Analisi_tipologia_elaborazione = enum_DataAnalisiBi_DSS_Analisi_tipologia_elaborazione.OperazioniDaQuaderno AndAlso (SchemaDiElaborazione = LAVCOD_RILIEVO_AVVERSITA_CAMPO OrElse SchemaDiElaborazione = LAVCOD_DANNI_RACCOLTA) Then

                misure = (From dd In dtDatiElaboratiLetti.AsEnumerable
                          Select New With {
                              Key .misura_cod = dd("indGenericCod").ToString.Replace("-", "_") & "_" & dd("udm_cod").ToString,
                              Key .misura_des = dd("indGenericDes").ToString & " - " & dd("Udm_Des").ToString
                              }).Distinct().ToList()

            Else

                misure = (From dd In dtDatiElaboratiLetti.AsEnumerable
                          Select New With {
                              Key .misura_cod = dd("indGenericCod").ToString.Replace("-", "_"),
                              Key .misura_des = dd("indGenericDes").ToString
                              }).Distinct().ToList()

            End If

            Dim dtCurve2 As New DataTable

            Dim l As New List(Of ColonneNome)
            Dim c As ColonneNome

            dtCurve2.Columns.Add(New DataColumn("Data", GetType(Date)))
            c = New ColonneNome("Data", AgronicaAgenda_2010.Data, "date")
            c._FormatoParticolare = "#=kendo.toString(Data, 'dd/MM/yyyy')#"
            c._Filtrabile = False
            c._cssHeader = "disable-reorder"
            l.Add(c)

            Dim col_data As ColonneNome = c
            Dim data_operazione_fine As Boolean = False
            If dtDatiElaboratiLetti.Columns.Contains("DataOperazioneFine") Then

                data_operazione_fine = True

                dtCurve2.Columns.Add(New DataColumn("Data_Fine", GetType(Date)))
                c = New ColonneNome("Data_Fine", "Data_Fine", "date")
                c._hidden = True
                l.Add(c)

                col_data._FormatoParticolare = "#=kendo.toString(Data, 'dd/MM/yyyy')# - #=kendo.toString(Data_Fine, 'dd/MM/yyyy')#"

            End If

            dtCurve2.Columns.Add(New DataColumn("ChiaveImpianto", GetType(String)))
            c = New ColonneNome("ChiaveImpianto", "ChiaveImpianto", "string")
            c._hidden = True
            l.Add(c)

            dtCurve2.Columns.Add(New DataColumn("Impresa", GetType(String)))
            c = New ColonneNome("Impresa", AgronicaAgenda_2010.Impresa, "string")
            c._FiltrabileConCheck = True
            c._cssHeader = "disable-reorder"
            l.Add(c)

            dtCurve2.Columns.Add(New DataColumn("Azienda", GetType(String)))
            c = New ColonneNome("Azienda", AgronicaAgenda_2010.CentroAziendale, "string")
            c._FiltrabileConCheck = True
            c._cssHeader = "disable-reorder"
            l.Add(c)

            Dim descrizione_appezzamento As Boolean = False
            If dtDatiElaboratiLetti.Columns.Contains("Appezzamento_Descrizione") Then

                descrizione_appezzamento = True

                dtCurve2.Columns.Add(New DataColumn("Appezzamento", GetType(String)))
                c = New ColonneNome("Appezzamento", AgronicaAgenda_2010.Appezzamento, "string")
                c._FiltrabileConCheck = True
                c._cssHeader = "disable-reorder"
                l.Add(c)

            End If

            dtCurve2.Columns.Add(New DataColumn("kPIN", GetType(String)))
            c = New ColonneNome("kPIN", "kPIN", "string")
            c._FiltrabileConCheck = True
            c._cssHeader = "disable-reorder"
            l.Add(c)

            dtCurve2.Columns.Add(New DataColumn("BlockName", GetType(String)))
            c = New ColonneNome("BlockName", "BlockName", "string")
            c._FiltrabileConCheck = True
            c._cssHeader = "disable-reorder"
            l.Add(c)

            dtCurve2.Columns.Add(New DataColumn("Specie", GetType(String)))
            c = New ColonneNome("Specie", AgronicaAgenda_2010.Specie, "string")
            c._FiltrabileConCheck = True
            c._cssHeader = "disable-reorder"
            l.Add(c)

            dtCurve2.Columns.Add(New DataColumn("Varieta", GetType(String)))
            c = New ColonneNome("Varieta", AgronicaAgenda_2010.Varietà, "string")
            c._FiltrabileConCheck = True
            c._cssHeader = "disable-reorder"
            l.Add(c)

            dtCurve2.Columns.Add(New DataColumn("sup_imp", GetType(Decimal)))
            c = New ColonneNome("sup_imp", DirectCast(System.Web.HttpContext.GetLocalResourceObject("~/DataAnalisiBI/Meteo/Meteo.aspx", "SuperficieImpianto"), String), "number")
            'c._formatNr = "0.0000"
            c._FormatoParticolare = "# if (sup_imp < 0) { ## } else { ##: kendo.toString(sup_imp, '0.0000') ## }#"
            c._css = "allineadestra"
            c._cssHeader = "allineadestra disable-reorder"
            c._Filtrabile = True
            c._FiltrabileConCheck = False
            l.Add(c)

            Dim data_raccolta As Boolean = False
            If dtDatiElaboratiLetti.Columns.Contains("Data_Raccolta") Then

                data_raccolta = True

                dtCurve2.Columns.Add(New DataColumn("Data_Raccolta", GetType(Date)))
                c = New ColonneNome("Data_Raccolta", AgronicaAgenda_2010.DataRaccolta, "date")
                c._FormatoParticolare = "#= Data_Raccolta !== null ? kendo.toString(Data_Raccolta, 'dd/MM/yyyy') : ''#"
                c._Filtrabile = True
                c._FiltrabileConCheck = False
                c._cssHeader = "disable-reorder"
                l.Add(c)

            End If

            Dim descrizione_analisi As Boolean = False
            If dtDatiElaboratiLetti.Columns.Contains("Descrizione_Analisi") Then

                descrizione_analisi = True

                dtCurve2.Columns.Add(New DataColumn("Analisi", GetType(String)))
                c = New ColonneNome("Analisi", AgronicaAgenda_2010.Analisi, "string")
                c._FiltrabileConCheck = True
                c._cssHeader = "disable-reorder"
                l.Add(c)

            End If

            Dim col_name As String
            For Each m In misure

                col_name = "Col_" & m.misura_cod
                dtCurve2.Columns.Add(New DataColumn(col_name, GetType(Decimal)))
                c = New ColonneNome(col_name, m.misura_des, "number")
                c._formatNr = "0.00"
                c._css = "allineadestra"
                c._cssHeader = "allineadestra"
                c._Filtrabile = True
                c._FiltrabileConCheck = False
                l.Add(c)

            Next

            Dim dr As DataRow = dtCurve2.NewRow()
            Dim chk_data As DateTime = dtDatiElaboratiLetti.Rows(0)("DataOperazione")
            Dim chk_chiave As String = dtDatiElaboratiLetti.Rows(0)("PIVA") & "_" & dtDatiElaboratiLetti.Rows(0)("sa_cod") & "_" & dtDatiElaboratiLetti.Rows(0)("appezza") & "_" & dtDatiElaboratiLetti.Rows(0)("id_reg") & "_" & dtDatiElaboratiLetti.Rows(0)("chiaveAnalisi")

            dr("Data") = chk_data
            If data_operazione_fine Then
                dr("Data_Fine") = dtDatiElaboratiLetti.Rows(0)("DataOperazioneFine")
            End If
            dr("ChiaveImpianto") = chk_chiave
            dr("Impresa") = dtDatiElaboratiLetti.Rows(0)("RagioneSociale")
            dr("Azienda") = dtDatiElaboratiLetti.Rows(0)("Sa_Nome")
            If descrizione_appezzamento Then
                dr("Appezzamento") = dtDatiElaboratiLetti.Rows(0)("Appezzamento_Descrizione")
            End If
            dr("kPIN") = dtDatiElaboratiLetti.Rows(0)("kPIN")
            dr("BlockName") = dtDatiElaboratiLetti.Rows(0)("BlockName")
            dr("Specie") = dtDatiElaboratiLetti.Rows(0)("Veg_Des")
            dr("Varieta") = dtDatiElaboratiLetti.Rows(0)("Cul_Des")
            dr("sup_imp") = dtDatiElaboratiLetti.Rows(0)("sup_imp")
            If data_raccolta Then
                dr("Data_Raccolta") = dtDatiElaboratiLetti.Rows(0)("Data_Raccolta")
            End If
            If descrizione_analisi Then
                dr("Analisi") = dtDatiElaboratiLetti.Rows(0)("Descrizione_Analisi")
            End If

            For Each row In dtDatiElaboratiLetti.Rows
                Dim dt_data As DateTime = row("DataOperazione")
                Dim dt_chiave As String = row("PIVA") & "_" & row("sa_cod") & "_" & row("appezza") & "_" & row("id_reg") & "_" & row("chiaveAnalisi")

                If dt_data <> chk_data OrElse dt_chiave <> chk_chiave Then
                    dtCurve2.Rows.Add(dr)
                    dr = dtCurve2.NewRow()
                    chk_data = dt_data
                    chk_chiave = dt_chiave
                    dr("Data") = chk_data
                    If data_operazione_fine Then
                        dr("Data_Fine") = row("DataOperazioneFine")
                    End If
                    dr("ChiaveImpianto") = chk_chiave
                    dr("Impresa") = row("RagioneSociale")
                    dr("Azienda") = row("Sa_Nome")
                    If descrizione_appezzamento Then
                        dr("Appezzamento") = row("Appezzamento_Descrizione")
                    End If
                    dr("kPIN") = row("kPIN")
                    dr("BlockName") = row("BlockName")
                    dr("Specie") = row("Veg_Des")
                    dr("Varieta") = row("Cul_Des")
                    dr("sup_imp") = row("sup_imp")
                    If data_raccolta Then
                        dr("Data_Raccolta") = row("Data_Raccolta")
                    End If
                    If descrizione_analisi Then
                        dr("Analisi") = row("Descrizione_Analisi")
                    End If
                End If

                If DSS_Analisi_tipologia_elaborazione = enum_DataAnalisiBi_DSS_Analisi_tipologia_elaborazione.OperazioniDaQuaderno AndAlso (SchemaDiElaborazione = LAVCOD_RILIEVO_AVVERSITA_CAMPO OrElse SchemaDiElaborazione = LAVCOD_DANNI_RACCOLTA) Then

                    col_name = row("indGenericCod").ToString.Replace("-", "_") & "_" & row("udm_cod").ToString
                Else

                    col_name = row("indGenericCod").ToString.Replace("-", "_")
                End If

                col_name = "Col_" & col_name
                dr(col_name) = CDec(row("Qta"))

            Next
            dtCurve2.Rows.Add(dr)

            'ritorno la tabella trasformata in json (aggiustando anche le colonne)
            Dim js As New AgronicaCoreDataProvider.JSON_DataTable
            js.Editabile_Deafault = False
            r.RispostaStringa = js.JSON_DataTable_Kendo(dtCurve2, l, AssegnaAutomaticamenteTipoFiltro_daTipoDato:=True, tipoFiltroKendo:=TipoFiltroKendo_colonne.CasellaDiscesa)

            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)


        End Try

        Return r
    End Function


    <WebMethod(EnableSession:=True)>
    Public Shared Function KendoPivotConfronta_Curve(ByVal DataDa As String,
                                                     ByVal DataA As String,
                                                     ByVal CfrStorico As String) As RispostaStandard

        Dim risp As New RispostaStandard
        risp.RispostaOK = False

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            risp.Sessione = False
            Return risp
        End If

        Lingua.Gias_InizializzaCultura_DaSession()

        Try

            Dim dDataDa As DateTime = DataDa
            Dim dDataA As DateTime = DataA

            Dim dataDaList As New List(Of DateTime)
            Dim dataAList As New List(Of DateTime)
            dataDaList.Add(dDataDa)
            dataAList.Add(dDataA)

            Dim annoIntervallo As Integer = dDataDa.Year
            If annoIntervallo <> dDataA.Year Then
                'Messaggio errore -> Intervallo a cavallo di due anni... (altrimenti cosa devo fare?)
            End If

            'Controllare funzionamento con Anno BISESTILE...

            For Each annoConfronto As Integer In CfrStorico.Split("|")
                If annoConfronto <> annoIntervallo Then
                    dataDaList.Add(New Date(annoConfronto, dDataDa.Month, dDataDa.Day))
                    dataAList.Add(New Date(annoConfronto, dDataA.Month, dDataA.Day))
                End If
            Next

            If dataDaList.Count = 1 Then
                'Non ho nulla da confrontare...
            End If

            'Dim xLetturaElaborato As New AgronicaCoreContabDAL.ElaborazioneDati_R

            ''*****************************************************************************
            'Dim piva As String = "F2000000000"
            'Dim sa_cod As Integer = 127270914
            'Dim appezza As Integer = 127270918 '127270921
            'Dim id_reg As Integer = 127270913
            ''*****************************************************************************

            'Dim dtCurveCfr As New DataTable

            'Dim l As New List(Of ColonneNome)
            'Dim c As ColonneNome

            'dtCurveCfr.Columns.Add(New DataColumn("Data", GetType(Date)))
            'c = New ColonneNome("Data", "Data", "date")
            'c._FormatoParticolare = "#=kendo.toString(Data, 'dd/MM/yyyy')#"
            'c._Filtrabile = False
            'l.Add(c)

            'Dim idx As Integer = 0
            'Dim errorFlag As Boolean = False
            ''Dim MsgErrore As String = ""
            ''Dim anno0 As Integer = dataDaList(idx).Year
            'Dim dtCurve As New DataTable
            'While idx < dataDaList.Count And Not errorFlag

            '    Try

            '        dtCurve = xLetturaElaborato.CurveMaturazione(dataDaList(idx), dataAList(idx), "", "", objParametri_Server)

            '    Catch ex As Exception

            '        errorFlag = True

            '    End Try

            '    If Not errorFlag Then

            '        Dim s_anno As String = dataDaList(idx).Year

            '        Dim curveFiltro = (From drc In dtCurve.AsEnumerable
            '                           Where drc("PIVA") = piva And drc("sa_cod") = sa_cod And drc("appezza") = appezza And drc("id_reg") = id_reg
            '                           Select drc).ToList()

            '        Dim misure = (From drf In curveFiltro.AsEnumerable
            '                      Select New With {
            '                          Key .misura_cod = drf("indGenericCod").ToString.Replace("-", "_"),
            '                          Key .misura_des = drf("indGenericDes")
            '                          }).Distinct().ToList()

            '        Dim col_name As String
            '        For Each m In misure
            '            col_name = "Col_" & m.misura_cod & "_" & s_anno
            '            dtCurveCfr.Columns.Add(New DataColumn(col_name, GetType(Decimal)))
            '            c = New ColonneNome(col_name, m.misura_des, "number")
            '            c._formatNr = "0.00"
            '            c._css = "allineadestra"
            '            c._cssHeader = "allineadestra"
            '            c._Filtrabile = False
            '            c._gruppoColonne = s_anno
            '            l.Add(c)
            '        Next

            '        Dim dr As DataRow = dtCurveCfr.NewRow()
            '        Dim chk_data As DateTime = curveFiltro(0)("DataOperazione")

            '        dr("Data") = chk_data
            '        For Each row In curveFiltro
            '            Dim dt_data As DateTime = row("DataOperazione")

            '            If dt_data <> chk_data Then
            '                dtCurveCfr.Rows.Add(dr)
            '                dr = dtCurveCfr.NewRow()
            '                chk_data = dt_data
            '                dr("Data") = chk_data
            '            End If

            '            col_name = row("indGenericCod")
            '            col_name = "Col_" & col_name.Replace("-", "_") & "_" & s_anno
            '            dr(col_name) = CDec(row("Qta"))

            '        Next
            '        dtCurveCfr.Rows.Add(dr)

            '    End If

            '    idx += 1

            'End While

            ''Dim lista = (From elem In dtCurveCfr.AsEnumerable
            ''             Select elem).OrderBy(Function(e) e("Data")).ToList()

            'Dim dataView As New DataView(dtCurveCfr)
            'dataView.Sort = "Data ASC"
            'Dim dataTable As DataTable = dataView.ToTable()

            ''ritorno la tabella trasformata in json (aggiustando anche le colonne)
            'Dim js As New AgronicaCoreDataProvider.JSON_DataTable
            'js.Editabile_Deafault = False
            'risp.RispostaStringa = js.JSON_DataTable_Kendo(dataTable, l, AssegnaAutomaticamenteTipoFiltro_daTipoDato:=True, tipoFiltroKendo:=TipoFiltroKendo_colonne.CasellaDiscesa)

            'risp.RispostaOK = True

        Catch ex As Exception

            risp.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            risp.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)


        End Try

        Return risp

    End Function


    <WebMethod(EnableSession:=True)>
    Public Shared Function RicercaDatiMeteo(ByVal DataDa As String,
                                            ByVal DataA As String,
                                            ByVal TipoSorgente As String,
                                            ByVal Sorgente As String,
                                            ByVal SogliaTemp As Decimal) As RispostaStandard

        Dim risp As New RispostaStandard
        risp.RispostaOK = False

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            risp.Sessione = False
            Return risp
        End If

        Lingua.Gias_InizializzaCultura_DaSession()

        Try
            Dim dtMeteo As New DataTable
            Dim MsgErrore As String = ""

            If CaricaDatiMeteo(DataDa, DataA, TipoSorgente, Sorgente, SogliaTemp, dtMeteo, MsgErrore, objParametri_Server) Then

                Try

                    'creo la lista delle colonne da visualizzare
                    Dim l As New List(Of ColonneNome)
                    Dim c As ColonneNome

                    c = New ColonneNome("Data", AgronicaAgenda_2010.Data, "date")
                    c._FormatoParticolare = "#=kendo.toString(Data, 'dd/MM/yyyy')#"
                    c._Filtrabile = False
                    l.Add(c)
                    c = New ColonneNome("Temp_min", DirectCast(System.Web.HttpContext.GetLocalResourceObject("~/DataAnalisiBI/Meteo/Meteo.aspx", "TemperaturaMinimaAbbr"), String), "number")
                    c._formatNr = "0.00"
                    c._css = "allineadestra"
                    c._cssHeader = "allineadestra"
                    c._Filtrabile = False
                    l.Add(c)
                    c = New ColonneNome("Temp_max", DirectCast(System.Web.HttpContext.GetLocalResourceObject("~/DataAnalisiBI/Meteo/Meteo.aspx", "TemperaturaMassimaAbbr"), String), "number")
                    c._formatNr = "0.00"
                    c._css = "allineadestra"
                    c._cssHeader = "allineadestra"
                    c._Filtrabile = False
                    l.Add(c)
                    c = New ColonneNome("Temp_media", DirectCast(System.Web.HttpContext.GetLocalResourceObject("~/DataAnalisiBI/Meteo/Meteo.aspx", "TemperaturaMediaAbbr"), String), "number")
                    c._formatNr = "0.00"
                    c._css = "allineadestra"
                    c._cssHeader = "allineadestra"
                    c._Filtrabile = False
                    l.Add(c)
                    c = New ColonneNome("SommaTermica", DirectCast(System.Web.HttpContext.GetLocalResourceObject("~/DataAnalisiBI/Meteo/Meteo.aspx", "SommaTermica"), String), "number")
                    c._formatNr = "0.00"
                    c._css = "allineadestra"
                    c._cssHeader = "allineadestra"
                    c._Filtrabile = False
                    l.Add(c)
                    c = New ColonneNome("Um_Rel", DirectCast(System.Web.HttpContext.GetLocalResourceObject("~/DataAnalisiBI/Meteo/Meteo.aspx", "UmiditàRelativa"), String), "number")
                    c._formatNr = "0.00"
                    c._css = "allineadestra"
                    c._cssHeader = "allineadestra"
                    c._Filtrabile = False
                    l.Add(c)
                    c = New ColonneNome("Pioggia", AgronicaAgenda_2010.Precipitazioni & " (mm)", "number")
                    c._formatNr = "0.00"
                    c._css = "allineadestra"
                    c._cssHeader = "allineadestra"
                    c._Filtrabile = False
                    l.Add(c)

                    'ritorno la tabella trasformata in json (aggiustando anche le colonne)
                    Dim js As New AgronicaCoreDataProvider.JSON_DataTable
                    js.Editabile_Deafault = False
                    risp.RispostaStringa = js.JSON_DataTable_Kendo(dtMeteo, l, AssegnaAutomaticamenteTipoFiltro_daTipoDato:=True, tipoFiltroKendo:=TipoFiltroKendo_colonne.CasellaDiscesa)
                    risp.RispostaOK = True

                Catch ex As Exception

                    risp.Errore = Resources.AgronicaAgenda_2010.SiÈVerificatoIlSeguenteErroreDuranteIlCari & vbCrLf & ex.Message

                End Try

            Else

                risp.Errore = MsgErrore

            End If

        Catch ex As Exception

            'uso questa funzione per ottenere il Messaggio..:
            risp.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return risp

    End Function

    Private Shared Function addDTColumn(ByRef dt As DataTable, ByVal colname As String, ByRef cols As List(Of ColonneNome), ByVal colout As String) As ColonneNome
        dt.Columns.Add(New DataColumn(colname, GetType(Decimal)))
        Dim c As New ColonneNome(colname, colout, "number")
        c._formatNr = "0.00"
        c._css = "allineadestra"
        c._cssHeader = "allineadestra"
        c._Filtrabile = False
        cols.Add(c)
        Return c
    End Function


    <WebMethod(EnableSession:=True)>
    Public Shared Function ConfrontaDatiMeteo(ByVal DataDa As String,
                                              ByVal DataA As String,
                                              ByVal TipoSorgente As String,
                                              ByVal Sorgente As String,
                                              ByVal SogliaTemp As Decimal,
                                              ByVal CfrStorico As String) As RispostaStandard

        Dim risp As New RispostaStandard
        risp.RispostaOK = False

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            risp.Sessione = False
            Return risp
        End If

        Lingua.Gias_InizializzaCultura_DaSession()

        Try
            Dim dDataDa As DateTime = DataDa
            Dim dDataA As DateTime = DataA

            Dim dataDaList As New List(Of DateTime)
            Dim dataAList As New List(Of DateTime)
            dataDaList.Add(dDataDa)
            dataAList.Add(dDataA)

            Dim annoIntervallo As Integer = dDataDa.Year
            If annoIntervallo <> dDataA.Year Then
                'Messaggio errore -> Intervallo a cavallo di due anni... (altrimenti cosa devo fare?)
            End If

            'Controllare funzionamento con Anno BISESTILE...

            For Each annoConfronto As Integer In CfrStorico.Split("|")
                If annoConfronto <> annoIntervallo Then
                    dataDaList.Add(New Date(annoConfronto, dDataDa.Month, dDataDa.Day))
                    dataAList.Add(New Date(annoConfronto, dDataA.Month, dDataA.Day))
                End If
            Next

            If dataDaList.Count = 1 Then
                'Non ho nulla da confrontare...
            End If

            ''***************************************************************
            ''DEBUG
            ''***************************************************************
            'dataDaList.Clear()
            'dataAList.Clear()
            'Dim ddd As New Date(2017, 2, 15)
            'dataDaList.Add(ddd)
            'dataAList.Add(ddd.AddDays(45))
            'ddd = ddd.AddDays(46)
            'dataDaList.Add(ddd)
            'dataAList.Add(ddd.AddDays(45))
            'ddd = ddd.AddDays(46)
            'dataDaList.Add(ddd)
            'dataAList.Add(ddd.AddDays(45))
            ''***************************************************************
            ''***************************************************************
            ''***************************************************************


            Dim dtCfrMeteo As New DataTable
            dtCfrMeteo.Columns.Add(New DataColumn("Data", GetType(Date)))

            Dim l As New List(Of ColonneNome)
            Dim c As New ColonneNome("Data", AgronicaAgenda_2010.Data, "date")
            c._FormatoParticolare = "#=kendo.toString(Data, 'dd/MM')#"
            c._Filtrabile = False
            l.Add(c)

            Dim idx As Integer = 0
            Dim errorFlag As Boolean = False
            Dim MsgErrore As String = ""
            Dim anno0 As Integer = dataDaList(idx).Year
            While idx < dataDaList.Count

                Dim dtMeteo As New DataTable

                If CaricaDatiMeteo(dataDaList(idx), dataAList(idx), TipoSorgente, Sorgente, SogliaTemp, dtMeteo, MsgErrore, objParametri_Server) Then

                    ''*************************************************************
                    ''DEBUG
                    ''*************************************************************
                    'If idx > 0 Then
                    '    Dim startDate As New DateTime(2017 + idx, dataDaList(0).Month, dataDaList(0).Day)
                    '    For Each row In dtMeteo.Rows
                    '        row.Item("Data") = startDate
                    '        startDate = startDate.AddDays(1)
                    '    Next
                    'End If
                    ''*************************************************************
                    ''*************************************************************
                    ''*************************************************************

                    Dim s_anno As String = dataDaList(idx).Year
                    ''*************************************************************
                    ''DEBUG
                    ''*************************************************************
                    'If idx > 0 Then
                    '    s_anno = CStr(2017 + idx)
                    'End If
                    ''*************************************************************
                    ''*************************************************************
                    ''*************************************************************
                    Dim fTempMedia As String = "Temp_media_" & s_anno
                    Dim fTempMin As String = "Temp_min_" & s_anno
                    Dim fTempMax As String = "Temp_max_" & s_anno
                    Dim fUmRel As String = "Um_Rel_" & s_anno
                    Dim fPioggia As String = "Pioggia_" & s_anno
                    Dim fSommaTermica As String = "SommaTermica_" & s_anno

                    addDTColumn(dtCfrMeteo, fTempMin, l, DirectCast(System.Web.HttpContext.GetLocalResourceObject("~/DataAnalisiBI/Meteo/Meteo.aspx", "TemperaturaMinimaAbbr"), String))._gruppoColonne = s_anno
                    addDTColumn(dtCfrMeteo, fTempMax, l, DirectCast(System.Web.HttpContext.GetLocalResourceObject("~/DataAnalisiBI/Meteo/Meteo.aspx", "TemperaturaMassimaAbbr"), String))._gruppoColonne = s_anno
                    addDTColumn(dtCfrMeteo, fTempMedia, l, DirectCast(System.Web.HttpContext.GetLocalResourceObject("~/DataAnalisiBI/Meteo/Meteo.aspx", "TemperaturaMediaAbbr"), String))._gruppoColonne = s_anno
                    addDTColumn(dtCfrMeteo, fSommaTermica, l, DirectCast(System.Web.HttpContext.GetLocalResourceObject("~/DataAnalisiBI/Meteo/Meteo.aspx", "SommaTermica"), String))._gruppoColonne = s_anno
                    addDTColumn(dtCfrMeteo, fUmRel, l, DirectCast(System.Web.HttpContext.GetLocalResourceObject("~/DataAnalisiBI/Meteo/Meteo.aspx", "UmiditàRelativa"), String))._gruppoColonne = s_anno
                    addDTColumn(dtCfrMeteo, fPioggia, l, AgronicaAgenda_2010.Precipitazioni)._gruppoColonne = s_anno

                    For Each row In dtMeteo.Rows

                        Dim m As Integer = CDate(row.Item("Data")).Month
                        Dim g As Integer = CDate(row.Item("Data")).Day
                        Dim dt0 As New Date(anno0, m, g)

                        Dim drl() As System.Data.DataRow
                        drl = dtCfrMeteo.Select(String.Format("Data = #{0}#", dt0.ToString("MM-dd-yyyy")))
                        Dim fAdd As Boolean = False
                        Dim dr As DataRow
                        If drl.Length = 0 Then
                            dr = dtCfrMeteo.NewRow
                            dr.Item("Data") = dt0
                            fAdd = True
                        Else
                            dr = drl(0)
                        End If

                        dr.Item(fTempMedia) = row.Item("Temp_media")
                        dr.Item(fTempMin) = row.Item("Temp_min")
                        dr.Item(fTempMax) = row.Item("Temp_max")
                        dr.Item(fUmRel) = row.Item("Um_Rel")
                        dr.Item(fPioggia) = row.Item("Pioggia")
                        dr.Item(fSommaTermica) = row.Item("SommaTermica")

                        If fAdd Then
                            dtCfrMeteo.Rows.Add(dr)
                        End If

                    Next

                Else
                    errorFlag = True
                    Exit While
                End If

                idx += 1

            End While

            If errorFlag Then
                risp.Errore = MsgErrore
            Else

                'ritorno la tabella trasformata in json (aggiustando anche le colonne)
                Dim js As New AgronicaCoreDataProvider.JSON_DataTable
                js.Editabile_Deafault = False
                risp.RispostaStringa = js.JSON_DataTable_Kendo(dtCfrMeteo, l, AssegnaAutomaticamenteTipoFiltro_daTipoDato:=True, tipoFiltroKendo:=TipoFiltroKendo_colonne.CasellaDiscesa)
                risp.RispostaOK = True

            End If

        Catch ex As Exception

            'uso questa funzione per ottenere il Messaggio..:
            risp.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return risp

    End Function


    Private Shared Function CaricaDatiMeteo(ByVal DataDa As DateTime,
                                            ByVal DataA As DateTime,
                                            ByVal TipoSorgente As String,
                                            ByVal Sorgente As String,
                                            ByVal SogliaTemp As Decimal,
                                            ByRef dtMeteo As DataTable,
                                            ByRef MsgErrore As String,
                                            ByVal objParametri_Server As AgronicaCoreParametri) As Boolean

        MsgErrore = ""
        Dim objMeteo As New AgronicaCoreWebService.Meteo

        Dim rval As String = objMeteo.MaxIntervalloDatiMeteo(objParametri_Server)

        Dim jss = New JavaScriptSerializer()
        rval = rval.Replace("{""ModelliPrevisionali_IntervalloDatiMeteoResult"":", "")
        rval = rval.Substring(0, rval.Length - 1)
        Dim r As RispostaStandard = jss.Deserialize(Of RispostaStandard)(rval)

        Dim MaxDays As Integer = CInt(r.RispostaStringa)

        If Math.Abs(DateDiff(DateInterval.Day, DataA, DataDa)) > MaxDays Then
            MsgErrore = String.Format(DirectCast(System.Web.HttpContext.GetLocalResourceObject("~/DataAnalisiBI/Meteo/Meteo.aspx", "IntervalloDateMaggioreDiNGiorniNonConsentito"), String), CStr(MaxDays))
            Return False
        End If

        If Sorgente = "" Then
            MsgErrore = Resources.AgronicaAgenda_2010.LeCoordinateOIlQuadranteSceltoPerIlCaricam
            Return False
        End If

        Dim flag_tabella_leggi As String = "H"
        Dim messaggioErrore As String = ""
        'chiamata web service.
        Dim XMLs_NodiDato As XmlNodeList = Carica_Dati_Sazione_WS(objMeteo, DataDa, DataA, TipoSorgente, Sorgente, MsgErrore, flag_tabella_leggi)

        'Leggi i nodi dato e Carica il datagrid
        If IsNothing(XMLs_NodiDato) OrElse XMLs_NodiDato.Count = 0 Then
            MsgErrore = AgronicaAgenda_2010.NonSonoPresentiDatiSullePioggePerLInterval & " - " & AgronicaAgenda_2010.Dal & " " & DataDa.ToString & " - " & AgronicaAgenda_2010.Al & " " & DataA.ToString
            Return False
        End If

        dtMeteo = New DataTable

        dtMeteo.Columns.Add(New DataColumn("Data", GetType(Date)))
        dtMeteo.Columns.Add(New DataColumn("Temp_media", GetType(Decimal)))
        dtMeteo.Columns.Add(New DataColumn("Temp_min", GetType(Decimal)))
        dtMeteo.Columns.Add(New DataColumn("Temp_max", GetType(Decimal)))
        dtMeteo.Columns.Add(New DataColumn("Um_Rel", GetType(Decimal)))
        dtMeteo.Columns.Add(New DataColumn("Pioggia", GetType(Decimal)))
        dtMeteo.Columns.Add(New DataColumn("SommaTermica", GetType(Decimal)))

        Dim oData As New DateTime(0)
        Dim hh_cnt As Integer = 0

        Dim t_media As Decimal
        Dim t_min As Decimal
        Dim t_max As Decimal
        Dim um_rel As Decimal
        Dim mm_pioggia As Decimal
        Dim somma_termica As Decimal = 0

        Dim i As Integer
        Dim xml_dato As XmlElement

        For i = 0 To XMLs_NodiDato.Count - 1

            xml_dato = XMLs_NodiDato.Item(i)

            Dim data As Date = xml_dato.GetAttribute("gg")

            If data <> oData Then
                If hh_cnt > 0 Then
                    'carico la struttura del datatable
                    Dim dr As DataRow = dtMeteo.NewRow

                    t_media = t_media / hh_cnt
                    somma_termica += Math.Max(0D, t_media - SogliaTemp)

                    dr.Item("Data") = oData
                    dr.Item("Temp_media") = t_media
                    dr.Item("Temp_min") = t_min
                    dr.Item("Temp_max") = t_max
                    dr.Item("Um_Rel") = um_rel / hh_cnt
                    dr.Item("Pioggia") = mm_pioggia
                    dr.Item("SommaTermica") = somma_termica

                    dtMeteo.Rows.Add(dr)
                End If

                oData = data
                hh_cnt = 0
                t_media = 0
                t_min = 1000
                t_max = -1000
                um_rel = 0
                mm_pioggia = 0
            End If

            t_media += CDec(xml_dato.GetAttribute("t"))
            t_min = Math.Min(t_min, CDec(xml_dato.GetAttribute("t_min")))
            t_max = Math.Max(t_max, CDec(xml_dato.GetAttribute("t_max")))
            um_rel += CDec(xml_dato.GetAttribute("u"))
            mm_pioggia += CDec(xml_dato.GetAttribute("mm"))

            hh_cnt += 1

        Next 'XMLs_NodiDato

        If hh_cnt > 0 Then
            'carico la struttura del datatable
            Dim dr As DataRow = dtMeteo.NewRow

            t_media = t_media / hh_cnt
            somma_termica += Math.Max(0D, t_media - SogliaTemp)

            dr.Item("Data") = oData
            dr.Item("Temp_media") = t_media
            dr.Item("Temp_min") = t_min
            dr.Item("Temp_max") = t_max
            dr.Item("Um_Rel") = um_rel / hh_cnt
            dr.Item("Pioggia") = mm_pioggia
            dr.Item("SommaTermica") = somma_termica

            dtMeteo.Rows.Add(dr)
        End If

        Return True

    End Function


    Private Shared Function Carica_Dati_Sazione_WS(ByVal objMeteo As AgronicaCoreWebService.Meteo, ByVal Data_Da As Date, ByVal Data_A As Date, ByVal Sorgente As String, ByVal nomeStazione As String, ByRef MessaggioErrore As String, ByRef flag_tabella_leggi As String) As XmlNodeList

        Dim Str_XML_Parametri As String = Carica_Dati_Sazione_WS_StringaWS(objMeteo, Data_Da, Data_A, Sorgente, nomeStazione, MessaggioErrore, flag_tabella_leggi)


        If Str_XML_Parametri = "" Then
            MessaggioErrore = Resources.AgronicaAgenda_2010.XmlParametriVuotaImpossibileChiamareIlWebS & " - " & MessaggioErrore
            Return Nothing
        Else

            Dim Str_XML_Risultato As String = Carica_Dati_Sazione_WS_Chiamata(objMeteo, Str_XML_Parametri, MessaggioErrore)

            If Str_XML_Risultato = "" Then

                MessaggioErrore = Resources.AgronicaAgenda_2010.NonÈStatoRecuperatoAlcunRisultatoDalWebSer & " - " & MessaggioErrore

            Else

                Dim Num_Quadrante As Integer
                Dim Numero_Nodi_Dato As Integer
                Dim Codice_Errore As Integer
                Dim Messaggio_Errore As String
                Dim XMLs_NodiDato As XmlNodeList

                Try

                    'Elabora la risposta del WS
                    objMeteo.Leggi_XML_Risultato(Str_XML_Risultato,
                                                    Num_Quadrante,
                                                    Numero_Nodi_Dato,
                                                    Codice_Errore,
                                                    Messaggio_Errore,
                                                    XMLs_NodiDato)



                Catch ex As Exception
                    MessaggioErrore &= Resources.AgronicaAgenda_2010.SiÈVerificatoIlSeguenteErroreDuranteLElabo &
                    vbCrLf & ex.Message
                    Return Nothing
                End Try

                If Codice_Errore <> 0 Then
                    MessaggioErrore = Resources.AgronicaAgenda_2010.SiÈVerificatoIlSeguenteErroreDuranteIlRecu & vbCrLf & Messaggio_Errore
                    Return Nothing
                Else

                    Return XMLs_NodiDato

                End If

            End If
        End If

    End Function


    Private Shared Function Carica_Dati_Sazione_WS_Chiamata(ByVal objMeteo As AgronicaCoreWebService.Meteo, ByVal Str_XML_Parametri As String, ByRef MessaggioErrore As String) As String

        Dim Str_XML_Risultato As String

        Try

            Str_XML_Risultato = objMeteo.Chiama_WS_Meteo_DatiCompleti(Str_XML_Parametri)

        Catch ex As Exception
            MessaggioErrore = Resources.AgronicaAgenda_2010.NonÈStatoRecuperatoAlcunRisultatoDalWebSer & ex.Message
            Return ""
        End Try

        Return Str_XML_Risultato

    End Function


    Private Shared Function Carica_Dati_Sazione_WS_StringaWS(
                                                            ByVal objMeteo As AgronicaCoreWebService.Meteo,
                                                            ByVal Data_Da As Date,
                                                            ByVal Data_A As Date,
                                                            ByVal tipoSorgente As String,
                                                            ByVal sorgente As String,
                                                            ByRef MessaggioErrore As String,
                                                            ByRef flag_tabella_leggi As String) As String

        Return AgronicaCoreWebService.Meteo.Carica_Dati_Sazione_WS_StringaWS(
            Resources.AgronicaAgenda_2010.SiÈVerificatoIlSeguenteErroreDuranteLaPrep,
            HttpContext.Current.Session("ASG_Utente_Username_Crypt"),
            HttpContext.Current.Session("ASG_Utente_Password_Crypt"),
            objMeteo,
            Data_Da,
            Data_A,
            tipoSorgente,
            sorgente,
            MessaggioErrore,
            flag_tabella_leggi)
    End Function


    <WebMethod(EnableSession:=True)>
    Public Shared Function LeggiAnagraficaStazioni() As RispostaStandard

        Dim risp As New RispostaStandard With {.RispostaOK = False}

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Super_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Super_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            risp.Sessione = False
            Return risp
        End If

        Try
            Dim piva As String = HttpContext.Current.Session("_piva")

            If String.IsNullOrEmpty(piva) Then
                Dim objParametriAgenda As New AgronicaCoreModello.ParametriAgenda_Temp.ParametriAgenda
                piva = objParametriAgenda.Piva
            End If

            Dim objParams As New JObject
            objParams("PIVA_Superuser") = objParametri_Server.PivaSuperUser
            objParams("PIVA") = piva

            'Dim linguaSession As Lingua = CType(System.Web.HttpContext.Current.Session("LinguaCorrente"), Lingua)
            'objParams("LinguaCodiceISO") = linguaSession.CodiceISO

            'Dim objMeteoNT As New AgronicaCoreWebService.MeteoNT
            Dim objMeteoNSuite As New InterfacciaMeteoSuite(objParametri_Server, objParametri_Super_Server)

            'Dim json = JObject.Parse(objMeteoNT.LeggiAnagraficaStazioni(objParams, objParametri_Server))
            Dim json = JObject.Parse(objMeteoNSuite.LeggiAnagraficaStazioni(objParams))

            risp.RispostaStringa = json("RispostaStringa").ToString

            risp.RispostaOK = True

        Catch ex As Exception

            'uso questa funzione per ottenere il Messaggio..:
            risp.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return risp
    End Function


    <WebMethod(EnableSession:=True)>
    Public Shared Function AggiornaAnagraficaStazione(ByVal jsonAnag As String) As RispostaStandard

        Dim risp As New RispostaStandard With {.RispostaOK = False}

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Super_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Super_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            risp.Sessione = False
            Return risp
        End If

        Try
            Dim objParametriAgenda As New AgronicaCoreModello.ParametriAgenda_Temp.ParametriAgenda

            Dim piva As String = HttpContext.Current.Session("_piva")

            If String.IsNullOrEmpty(piva) Then
                piva = objParametriAgenda.Piva
            End If

            Dim objParams As New JObject
            objParams("PIVA_Superuser") = objParametri_Server.PivaSuperUser
            objParams("PIVA") = piva
            objParams("Anagrafica") = JObject.Parse(jsonAnag)

            objParams("Anagrafica")("Staz_Note_Visibilita") = objParametri_Server.SuperUserUsername & " (" & objParametriAgenda.RagSoc & ")"

            'Dim linguaSession As Lingua = CType(System.Web.HttpContext.Current.Session("LinguaCorrente"), Lingua)
            'objParams("LinguaCodiceISO") = linguaSession.CodiceISO

            'Dim objMeteoNT As New AgronicaCoreWebService.MeteoNT
            Dim objMeteoSuite As New InterfacciaMeteoSuite(objParametri_Server, objParametri_Super_Server)

            'Dim json = JObject.Parse(objMeteoNT.AggiornaAnagraficaStazione(objParams, objParametri_Server))
            Dim json = JObject.Parse(objMeteoSuite.AggiornaAnagraficaStazione(objParams))

            risp.RispostaStringa = json("RispostaStringa").ToString
            risp.RispostaOK = True

        Catch ex As Exception

            'uso questa funzione per ottenere il Messaggio..:
            risp.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return risp
    End Function


    <WebMethod(EnableSession:=True)>
    Public Shared Function ControllaEliminaStazione(ByVal Id_Stazione As Integer) As RispostaStandard

        Dim risp As New RispostaStandard With {.RispostaOK = False}

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Super_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Super_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            risp.Sessione = False
            Return risp
        End If

        Try
            Dim objParametriAgenda As New AgronicaCoreModello.ParametriAgenda_Temp.ParametriAgenda

            Dim piva As String = HttpContext.Current.Session("_piva")

            If String.IsNullOrEmpty(piva) Then
                piva = objParametriAgenda.Piva
            End If

            Dim applic As New AgronicaCoreMeteoDAL.DSS_Stazioni_X_Applicazioni_Reader

            Dim dt_Associa As DataTable = applic.LeggiApplicazioni(Id_Stazione, objParametri_Server)

            If dt_Associa Is Nothing OrElse dt_Associa.Rows.Count = 0 Then

                Dim objParams As New JObject
                objParams("PIVA_Superuser") = objParametri_Server.PivaSuperUser
                objParams("PIVA") = piva
                objParams("Stazione_Cod") = Id_Stazione

                'Dim linguaSession As Lingua = CType(System.Web.HttpContext.Current.Session("LinguaCorrente"), Lingua)
                'objParams("LinguaCodiceISO") = linguaSession.CodiceISO

                'Dim objMeteoNT As New AgronicaCoreWebService.MeteoNT
                Dim objMeteoSuite As New InterfacciaMeteoSuite(objParametri_Server, objParametri_Super_Server)

                'Dim json = JObject.Parse(objMeteoNT.VerificaEliminaStazione(objParams, objParametri_Server))
                Dim json = JObject.Parse(objMeteoSuite.VerificaEliminaStazione(objParams))

                risp.RispostaStringa = json("RispostaStringa").ToString
                risp.RispostaOK = True
            End If

        Catch ex As Exception

            'uso questa funzione per ottenere il Messaggio..:
            risp.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return risp
    End Function


    <WebMethod(EnableSession:=True)>
    Public Shared Function LeggiStazioniXSorgente(ByVal TipoSorgente As Integer, ByVal Lat As Decimal, ByVal Lng As Decimal) As RispostaStandard

        Dim risp As New RispostaStandard With {.RispostaOK = False}

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_SuperServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Super_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) OrElse IsNothing(objParametri_SuperServer) Then
            risp.Sessione = False
            Return risp
        End If

        Lingua.Gias_InizializzaCultura_DaSession()

        Try

            Dim piva As String = HttpContext.Current.Session("_piva")

            If String.IsNullOrEmpty(piva) Then
                Dim objParametriAgenda As New AgronicaCoreModello.ParametriAgenda_Temp.ParametriAgenda
                piva = objParametriAgenda.Piva
            End If

            Dim objParams As New JObject
            objParams("PIVA_Superuser") = objParametri_Server.PivaSuperUser
            objParams("PIVA") = piva
            objParams("TipoSorgente") = TipoSorgente
            objParams("DistanzaDa") = New JObject(New JProperty("lat", Lat), New JProperty("lng", Lng))

            'Dim objMeteoNT As New AgronicaCoreWebService.MeteoNT
            Dim objMeteoSuite As New InterfacciaMeteoSuite(objParametri_Server, objParametri_SuperServer)

            'Dim json = JObject.Parse(objMeteoNT.LeggiStazioniXSorgente(objParams, objParametri_Server))
            Dim json = JObject.Parse(objMeteoSuite.LeggiStazioniXSorgente(objParams))

            risp.RispostaStringa = json("RispostaStringa").ToString

            risp.RispostaOK = True

        Catch ex As Exception

            risp.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            risp.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return risp
    End Function


    <WebMethod(EnableSession:=True)>
    Public Shared Function LocalizzaStazione(stazione_cod As Integer) As RispostaStandard

        Dim risp As New RispostaStandard With {.RispostaOK = False}

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Super_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Super_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            risp.Sessione = False
            Return risp
        End If

        Lingua.Gias_InizializzaCultura_DaSession()

        Try

            'Dim piva As String = HttpContext.Current.Session("_piva")

            'If String.IsNullOrEmpty(piva) Then
            '    Dim objParametriAgenda As New AgronicaCoreModello.ParametriAgenda_Temp.ParametriAgenda
            '    piva = objParametriAgenda.Piva
            'End If

            'Dim objMeteoNT As New AgronicaCoreWebService.MeteoNT
            Dim objMeteoSuite As New InterfacciaMeteoSuite(objParametri_Server, objParametri_Super_Server)

            'Dim json = JObject.Parse(objMeteoNT.LeggiStazione(stazione_cod, False, objParametri_Server))
            Dim json = JObject.Parse(objMeteoSuite.LeggiStazione(stazione_cod, False))

            Dim objStaz = JObject.Parse(json("RispostaStringa"))

            Dim objLoc As New JObject From {
                {"coordinates", New JObject From {
                {"lat", objStaz("Lat")},
                {"lng", objStaz("Lng")}
            }}}

            risp.RispostaStringa = objLoc.ToString()

            risp.RispostaOK = True

        Catch ex As Exception

            risp.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            risp.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return risp
    End Function


    Private Shared Sub CompletaStazioni(ByRef stazioni As _elenco_stazioni, ByVal piva As String, ByRef objParametri_Server As AgronicaCoreParametri, ByRef objParametri_Super_Server As AgronicaCoreParametri)

        Dim j_stazioni As New JArray
        Dim j_staz As JArray
        For Each kvp In stazioni.DictOfTipoSorgente()

            j_staz = New JArray
            For Each s In kvp.Value
                j_staz.Add(New JObject(New JProperty("stazione_cod", s)))
            Next

            j_stazioni.Add(New JObject(New JProperty("tipo_sorgente", kvp.Key), New JProperty("elenco_stazioni", j_staz)))
        Next

        Dim objParams As New JObject
        objParams("PIVA_Superuser") = objParametri_Server.PivaSuperUser
        objParams("PIVA") = piva
        objParams("ElencoStazioni") = j_stazioni

        'Dim linguaSession As Lingua = CType(System.Web.HttpContext.Current.Session("LinguaCorrente"), Lingua)
        'objParams("LinguaCodiceISO") = linguaSession.CodiceISO

        'Dim objMeteoNT As New AgronicaCoreWebService.MeteoNT
        Dim objMeteoSuite As New InterfacciaMeteoSuite(objParametri_Server, objParametri_Super_Server)

        'Dim s_risp As String = objMeteoNT.LeggiElencoStazioni(objParams, objParametri_Server)
        Dim s_risp As String = objMeteoSuite.LeggiElencoStazioni(objParams)
        Dim j_risp = JObject.Parse(s_risp)
        j_stazioni = JArray.Parse(j_risp("RispostaStringa"))

        For Each jStazObj In j_stazioni

            Dim t_sorgente As Integer = CInt(jStazObj("tipo_sorgente"))
            j_staz = jStazObj("elenco_stazioni")

            For Each s In j_staz

                Dim _ks As New _key_stazione With {.tipo_sorgente = t_sorgente, .stazione_cod = CInt(s("stazione_cod"))}

                Dim staz = stazioni.StazioneFromKey(_ks)

                If staz IsNot Nothing Then
                    staz.stazione_name = s("stazione_nome").ToString()
                    staz.rif_fornitore = s("rif_fornitore").ToString()
                    staz.flag_reale = CBool(s("flag_reale"))
                    staz.sensori_des = s("sensori").ToString()
                End If
            Next
        Next
    End Sub


    <WebMethod(EnableSession:=True)>
    Public Shared Function LeggiStazione(ByVal tipo_sorgente As Integer, ByVal stazione_cod As Integer) As RispostaStandard

        Dim risp As New RispostaStandard With {.RispostaOK = False}

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Super_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Super_Server")

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            risp.Sessione = False
            Return risp
        End If

        Try

            Dim objParametriAgenda = New ParametriAgenda

            Dim piva As String = HttpContext.Current.Session("_piva")

            If String.IsNullOrEmpty(piva) Then
                piva = objParametriAgenda.Piva
            End If

            Dim elenco As New _elenco_stazioni

            Dim _ks = _key_stazione.TryNew(tipo_sorgente, stazione_cod)

            elenco.TryAdd(New _stazione(_ks))

            CompletaStazioni(elenco, piva, objParametri_Server, objParametri_Super_Server)

            If elenco.GetList.Any Then

                risp.RispostaStringa = JsonConvert.SerializeObject(elenco.GetList()(0), Newtonsoft.Json.Formatting.None, New JsonSerializerSettings With {
                                                               .ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                                                               .NullValueHandling = NullValueHandling.Ignore
                                                               })
                risp.RispostaOK = True
            End If

        Catch ex As Exception

            risp.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            risp.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return risp
    End Function


    Private Shared Function StazioniXModelli(ByVal tipo_sorgente As Integer, ByVal stazione_cod As Integer,
                                             ByRef objParametri_Server As AgronicaCoreParametri,
                                             ByRef objParametri_Super_Server As AgronicaCoreParametri) As List(Of _stazione_x_modelli)

        Dim objParametriAgenda = New ParametriAgenda

        Dim piva As String = HttpContext.Current.Session("_piva")

        If String.IsNullOrEmpty(piva) Then
            piva = objParametriAgenda.Piva
        End If

        Dim objSxM As New AgronicaCoreMeteoDAL.DSS_Stazioni_X_Modelli_Reader

        Dim filtroAggiuntivo As AgronicaCoreMeteoDAL.DSS_Stazioni_X_Modelli_Reader.Filter = Nothing

        If tipo_sorgente >= 0 Then

            filtroAggiuntivo = New AgronicaCoreMeteoDAL.DSS_Stazioni_X_Modelli_Reader.Filter With {
                .TipoSorgente = tipo_sorgente,
                .StazioneCod = stazione_cod
            }
        End If

        Dim dt_SxM = objSxM.Leggi(objParametriAgenda.Piva, filtroAggiuntivo, objParametri_Server)


        'Dim objSxM As New AgronicaCoreMeteoDAL.DSS_Stazioni_X_Modelli_R

        'Dim xFiltroAggiuntivo As String = ""

        'If tipo_sorgente >= 0 Then

        '    xFiltroAggiuntivo = "s.Tipo_Sorgente = " & tipo_sorgente

        '    If stazione_cod > 0 Then

        '        xFiltroAggiuntivo &= " AND s.Stazione_Cod = " & stazione_cod
        '    End If
        'End If

        'Dim dt_SxM = objSxM.Leggi(objParametriAgenda.Piva, xFiltroAggiuntivo, "", objParametri_Server)

        Dim elenco As New _elenco_stazioni
        Dim dictModelli As New Dictionary(Of _key_modello, List(Of _modello))

        For Each _sm In dt_SxM.Rows

            Dim _ks = _key_stazione.TryNew(_sm("Tipo_Sorgente"), _sm("Stazione_Cod"))

            If _ks IsNot Nothing Then

                Dim staz As _stazione_x_modelli = elenco.TryAdd(New _stazione_x_modelli(_ks))

                Dim _km = _key_modello.TryNew(_sm("Mod_Cod"), _sm("Veg_Cod"), _sm("Avv_Cod"), _sm("Alg_Cod"), _sm("ParametriElaborazione"))

                If _km IsNot Nothing Then

                    Dim tmpdt As New Date(1999, 1, 1)
                    Dim periodo_des As String = "Periodo calcolo: "
                    Dim gg As Integer = CInt(_sm("InizioPeriodo_gg"))
                    periodo_des &= tmpdt.AddDays(gg - 1).ToString("m")
                    periodo_des &= " - "
                    gg = CInt(_sm("FinePeriodo_gg"))
                    periodo_des &= tmpdt.AddDays(gg - 1).ToString("m")

                    Dim validita_des As String = "Validità: "
                    Dim m As Integer = CInt(_sm("Validita_minuti"))
                    If m < 60 Then
                        validita_des &= m.ToString() & " minuti"
                    Else
                        Dim h As Integer = m / 60
                        m -= h * 60
                        validita_des &= h.ToString() & " ore"
                        If m > 0 Then
                            validita_des &= " " & m.ToString() & " minuti"
                        End If
                    End If

                    staz.modelli.Add(New _modello With {
                                     .key_modello = _km,
                                     .veg_des = "",
                                     .mod_name = "",
                                     .avv_name = "",
                                     .param_des = _sm("ParametriElaborazione"),
                                     .periodo_des = periodo_des,
                                     .validita_des = validita_des,
                                     .flag_completo = False
                                     })

                    If Not dictModelli.ContainsKey(_km) Then
                        dictModelli.Add(_km, New List(Of _modello))
                    End If

                    dictModelli(_km).Add(staz.modelli.Last())
                End If
            End If
        Next

        CompletaStazioni(elenco, piva, objParametri_Server, objParametri_Super_Server)

        Dim j_modelli As New JArray

        For Each mk In dictModelli.Keys

            j_modelli.Add(New JObject(
                          New JProperty("mod_cod", mk.mod_cod),
                          New JProperty("veg_cod", mk.veg_cod),
                          New JProperty("avv_cod", mk.avv_cod),
                          New JProperty("alg_cod", mk.alg_cod),
                          New JProperty("params", mk.params)))
        Next

        Dim objParams As New JObject
        objParams("Doorkey") = GetDoorkey()
        objParams("PIVA_Superuser") = objParametri_Server.PivaSuperUser
        objParams("PIVA") = piva
        objParams("Modelli") = j_modelli

        'Dim objMeteoNT As New AgronicaCoreWebService.MeteoNT
        Dim objDifesa As New DSS_Difesa_ModelliPrevisionali(objParametri_Server, objParametri_Super_Server)

        'Dim ss As String = objMeteoNT.CompletaOutputModelli(objParams, objParametri_Server)
        Dim ss As String = objDifesa.CompletaOutputModelli(objParams)
        Dim jrisp = JObject.Parse(ss)
        ss = jrisp("RispostaStringa").ToString

        If Not String.IsNullOrEmpty(ss) Then

            j_modelli = JArray.Parse(ss)

            For Each jModObj In j_modelli

                Dim _km As New _key_modello With {
                    .mod_cod = CInt(jModObj("mod_cod")),
                    .veg_cod = CInt(jModObj("veg_cod")),
                    .avv_cod = CInt(jModObj("avv_cod")),
                    .alg_cod = CInt(jModObj("alg_cod")),
                    .params = jModObj("params").ToString
                }

                If dictModelli.ContainsKey(_km) Then

                    Dim mod_des = jModObj("mod_des").ToString
                    Dim alg_des = jModObj("alg_des").ToString
                    If Not String.IsNullOrEmpty(alg_des) Then
                        mod_des &= " (" & alg_des & ")"
                    End If
                    Dim mod_des_agg = jModObj("mod_des_agg").ToString
                    If Not String.IsNullOrEmpty(mod_des_agg) Then
                        mod_des &= " [" & mod_des_agg & "]"
                    End If

                    For Each oModello In dictModelli(_km)

                        oModello.veg_des = jModObj("veg_des").ToString
                        oModello.mod_name = mod_des
                        oModello.avv_name = jModObj("avv_des").ToString
                        oModello.param_des = jModObj("param_des").ToString
                        oModello.flag_completo = True
                    Next
                End If
            Next
        End If

        Dim listaStazioni = elenco.GetList(Of _stazione_x_modelli)

        For Each staz In listaStazioni

            Dim iMod As Integer = 0
            While iMod < staz.modelli.Count
                If Not staz.modelli(iMod).flag_completo Then
                    staz.modelli.RemoveAt(iMod)
                Else
                    iMod += 1
                End If
            End While

            If staz.modelli.Any() Then
                staz.modelli.Sort(Function(ByVal m0 As _modello, ByVal m1 As _modello)
                                      Return m0.veg_des.CompareTo(m1.veg_des)
                                  End Function)
            End If
        Next

        Return listaStazioni
    End Function


    <WebMethod(EnableSession:=True)>
    Public Shared Function LeggiStazioniXModelli(ByVal tipo_sorgente As Integer, ByVal stazione_cod As Integer) As RispostaStandard

        Dim risp As New RispostaStandard With {.RispostaOK = False}

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Super_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Super_Server")

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) OrElse IsNothing(objParametri_Super_Server) Then
            risp.Sessione = False
            Return risp
        End If

        Try

            Dim listaStazioni = StazioniXModelli(tipo_sorgente, stazione_cod, objParametri_Server, objParametri_Super_Server)

            risp.RispostaStringa = JsonConvert.SerializeObject(listaStazioni, Newtonsoft.Json.Formatting.None, New JsonSerializerSettings With {
                                                               .ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                                                               .NullValueHandling = NullValueHandling.Ignore
                                                               })

            risp.RispostaOK = True

        Catch ex As Exception

            risp.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            risp.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return risp
    End Function


    <WebMethod(EnableSession:=True)>
    Public Shared Function RegistraModelloXStazione(ByVal tipo_sorgente As Integer, ByVal stazione_cod As Integer, ByVal modello As Object) As RispostaStandard

        Dim risp As New RispostaStandard With {.RispostaOK = False}

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Super_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Super_Server")

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) OrElse IsNothing(objParametri_Super_Server) Then
            risp.Sessione = False
            Return risp
        End If

        Try

            Dim piva As String = HttpContext.Current.Session("_piva")

            If String.IsNullOrEmpty(piva) Then
                Dim objParametriAgenda = New ParametriAgenda
                piva = objParametriAgenda.Piva
            End If

            Dim mod_cod As Integer = modello("mod_cod")
            Dim veg_cod As Integer = modello("veg_cod")
            Dim avv_cod As Integer = modello("avv_cod")
            Dim alg_cod As Integer = modello("alg_cod")
            Dim inizio_gg As Integer = modello("inizioperiodo_gg")
            Dim fine_gg As Integer = modello("fineperiodo_gg")
            Dim val_minuti As Integer = modello("validita_minuti")
            Dim objParam As New JObject()
            If modello.containsKey("param") Then

                For Each k In modello("param").keys()
                    objParam.Add(New JProperty(k, modello("param")(k)))
                Next
            End If
            Dim ParametriElaborazione As String = JsonConvert.SerializeObject(objParam)

            Dim modelliW As New AgronicaCoreMeteoDAL.DSS_Stazioni_X_Modelli_W

            Dim bAdd As Boolean = modelliW.Scrivi(piva, tipo_sorgente, stazione_cod, mod_cod, veg_cod, avv_cod, alg_cod, ParametriElaborazione, inizio_gg, fine_gg, val_minuti, objParametri_Server)

            If bAdd Then

                Dim stazione = StazioniXModelli(tipo_sorgente, stazione_cod, objParametri_Server, objParametri_Super_Server)

                If stazione.Count >= 1 Then

                    risp.RispostaStringa = JsonConvert.SerializeObject(stazione.First().modelli, Newtonsoft.Json.Formatting.None, New JsonSerializerSettings With {
                                                               .ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                                                               .NullValueHandling = NullValueHandling.Ignore
                                                               })
                    risp.RispostaOK = True
                End If
            End If

        Catch ex As Exception

            risp.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            risp.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return risp
    End Function


    <WebMethod(EnableSession:=True)>
    Public Shared Function EliminaModelloXStazione(ByVal tipo_sorgente As Integer, ByVal stazione_cod As Integer, ByVal modello As Object) As RispostaStandard

        Dim risp As New RispostaStandard With {.RispostaOK = False}

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Super_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Super_Server")

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) OrElse IsNothing(objParametri_Super_Server) Then
            risp.Sessione = False
            Return risp
        End If

        Try

            Dim piva As String = HttpContext.Current.Session("_piva")

            If String.IsNullOrEmpty(piva) Then
                Dim objParametriAgenda = New ParametriAgenda
                piva = objParametriAgenda.Piva
            End If

            Dim mod_cod As Integer = modello("mod_cod")
            Dim veg_cod As Integer = modello("veg_cod")
            Dim avv_cod As Integer = modello("avv_cod")
            Dim alg_cod As Integer = modello("alg_cod")
            Dim param As String = modello("params")

            Dim modelliW As New AgronicaCoreMeteoDAL.DSS_Stazioni_X_Modelli_W

            Dim bDel As Boolean = modelliW.Cancella(piva, tipo_sorgente, stazione_cod, mod_cod, veg_cod, avv_cod, alg_cod, param, objParametri_Server)

            If bDel Then

                Dim stazione = StazioniXModelli(tipo_sorgente, stazione_cod, objParametri_Server, objParametri_Super_Server)

                If stazione.Count >= 1 Then

                    risp.RispostaStringa = JsonConvert.SerializeObject(stazione.First().modelli, Newtonsoft.Json.Formatting.None, New JsonSerializerSettings With {
                                                               .ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                                                               .NullValueHandling = NullValueHandling.Ignore
                                                               })
                Else

                    risp.RispostaStringa = (New JArray).ToString
                End If

                risp.RispostaOK = True
            End If

        Catch ex As Exception

            risp.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            risp.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return risp
    End Function


    Private Shared Function StazioniXIrriga(ByVal tipo_sorgente As Integer, ByVal stazione_cod As Integer, ByRef objParametri_Server As AgronicaCoreParametri, ByRef objParametri_Super_Server As AgronicaCoreParametri) As List(Of _stazione_x_centri)

        Dim objParametriAgenda = New ParametriAgenda

        Dim piva As String = HttpContext.Current.Session("_piva")

        If String.IsNullOrEmpty(piva) Then
            piva = objParametriAgenda.Piva
        End If

        Dim objSxI As New AgronicaCoreMeteoDAL.DSS_Stazioni_X_Irriga_Reader

        Dim filtroAggiuntivo As AgronicaCoreMeteoDAL.DSS_Stazioni_X_Irriga_Reader.Filter = Nothing

        If tipo_sorgente >= 0 Then

            filtroAggiuntivo = New AgronicaCoreMeteoDAL.DSS_Stazioni_X_Irriga_Reader.Filter With {
                .CodiceCentro = -1,
                .TipoSorgente = tipo_sorgente,
                .StazioneCod = stazione_cod
            }
        End If

        Dim dt_SxI = objSxI.Leggi(objParametriAgenda.Piva, filtroAggiuntivo, objParametri_Server)

        'Dim objSxI As New AgronicaCoreMeteoDAL.DSS_Stazioni_X_Irriga_R

        'Dim xFiltroAggiuntivo As String = ""

        'If tipo_sorgente >= 0 Then

        '    xFiltroAggiuntivo = "s.Tipo_Sorgente = " & tipo_sorgente

        '    If stazione_cod > 0 Then

        '        xFiltroAggiuntivo &= " AND s.Stazione_Cod = " & stazione_cod
        '    End If
        'End If

        'Dim dt_SxI = objSxI.Leggi(objParametriAgenda.Piva, xFiltroAggiuntivo, "", objParametri_Server)

        Dim elenco As New _elenco_stazioni

        For Each _si In dt_SxI.Rows

            Dim _ks = _key_stazione.TryNew(_si("Tipo_Sorgente"), _si("Stazione_Cod"))

            If _ks IsNot Nothing Then

                Dim staz As _stazione_x_centri = elenco.TryAdd(New _stazione_x_centri(_ks))

                staz.centri.Add(New _centro With {
                                .sa_cod = _si("Sa_Cod"),
                                .sa_nome = _si("Sa_Nome")
                                })
            End If
        Next

        If Not elenco.GetList().Any() AndAlso tipo_sorgente >= 0 AndAlso stazione_cod > 0 Then
            elenco.TryAdd(New _stazione_x_centri(_key_stazione.TryNew(tipo_sorgente, stazione_cod)))
        End If

        CompletaStazioni(elenco, piva, objParametri_Server, objParametri_Super_Server)

        Return elenco.GetList(Of _stazione_x_centri)
    End Function


    <WebMethod(EnableSession:=True)>
    Public Shared Function LeggiStazioniXIrriga(ByVal tipo_sorgente As Integer, ByVal stazione_cod As Integer) As RispostaStandard

        Dim risp As New RispostaStandard With {.RispostaOK = False}

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Super_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Super_Server")

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            risp.Sessione = False
            Return risp
        End If

        Try

            Dim listaStazioni = StazioniXIrriga(tipo_sorgente, stazione_cod, objParametri_Server, objParametri_Super_Server)

            risp.RispostaStringa = JsonConvert.SerializeObject(listaStazioni, Newtonsoft.Json.Formatting.None, New JsonSerializerSettings With {
                                                               .ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                                                               .NullValueHandling = NullValueHandling.Ignore
                                                               })
            risp.RispostaOK = True

        Catch ex As Exception

            risp.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            risp.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return risp
    End Function


    <WebMethod(EnableSession:=True)>
    Public Shared Function RegistraCentriXStazione(ByVal tipo_sorgente As Integer, ByVal stazione_cod As Integer, ByVal centri As Integer()) As RispostaStandard

        Dim risp As New RispostaStandard With {.RispostaOK = False}

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Super_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Super_Server")

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) OrElse IsNothing(objParametri_Super_Server) Then
            risp.Sessione = False
            Return risp
        End If

        Try

            Dim piva As String = HttpContext.Current.Session("_piva")

            If String.IsNullOrEmpty(piva) Then
                Dim objParametriAgenda = New ParametriAgenda
                piva = objParametriAgenda.Piva
            End If

            Dim irrigaR As New AgronicaCoreMeteoDAL.DSS_Stazioni_X_Irriga_Reader
            Dim oCod = irrigaR.LeggiBlocco(piva, tipo_sorgente, stazione_cod, objParametri_Server)

            Dim oCentri As New List(Of Integer)
            For Each rcod In oCod.Rows
                oCentri.Add(CInt(rcod("Sa_Cod")))
            Next

            Dim listaIns = centri.ToList.Except(oCentri).ToList
            Dim listaDel = oCentri.Except(centri.ToList).ToList

            If listaIns.Any OrElse listaDel.Any Then

                Dim keys2ins As New List(Of AgronicaCoreMeteoDAL.DSS_Stazioni_X_Irriga_W._key)
                Dim keys2del As New List(Of AgronicaCoreMeteoDAL.DSS_Stazioni_X_Irriga_W._key)

                For Each c In listaIns
                    keys2ins.Add(New AgronicaCoreMeteoDAL.DSS_Stazioni_X_Irriga_W._key With {
                             .PIVA = piva,
                             .Tipo_Sorgente = tipo_sorgente,
                             .Stazione_Cod = stazione_cod,
                             .Sa_Cod = c
                             })
                Next

                For Each c In listaDel
                    keys2del.Add(New AgronicaCoreMeteoDAL.DSS_Stazioni_X_Irriga_W._key With {
                             .PIVA = piva,
                             .Tipo_Sorgente = tipo_sorgente,
                             .Stazione_Cod = stazione_cod,
                             .Sa_Cod = c
                             })
                Next

                Dim irrigaW As New AgronicaCoreMeteoDAL.DSS_Stazioni_X_Irriga_W

                Dim bResult As Boolean = irrigaW.ScriviBlocco(keys2ins, keys2del, objParametri_Server)

                If bResult Then

                    Dim stazione = StazioniXIrriga(tipo_sorgente, stazione_cod, objParametri_Server, objParametri_Super_Server)

                    If stazione.Count >= 1 Then

                        risp.RispostaStringa = JsonConvert.SerializeObject(stazione.First(), Newtonsoft.Json.Formatting.None, New JsonSerializerSettings With {
                                                               .ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                                                               .NullValueHandling = NullValueHandling.Ignore
                                                               })
                        risp.RispostaOK = True
                    End If
                End If
            Else

                risp.RispostaStringa = ""
                risp.RispostaOK = True
            End If

        Catch ex As Exception

            risp.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            risp.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return risp
    End Function


    <WebMethod(EnableSession:=True)>
    Public Shared Function LeggiStazioniXMonitor() As RispostaStandard

        Dim risp As New RispostaStandard With {.RispostaOK = False}

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Super_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Super_Server")

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            risp.Sessione = False
            Return risp
        End If

        Try
            Dim objParametriAgenda = New ParametriAgenda

            Dim piva As String = HttpContext.Current.Session("_piva")

            If String.IsNullOrEmpty(piva) Then
                piva = objParametriAgenda.Piva
            End If

            'Dim objSxM As New AgronicaCoreMeteoDAL.DSS_Stazioni_X_Monitor_R

            'Dim dt_SxM = objSxM.Leggi(piva, "", "", objParametri_Server)

            Dim objSxM As New AgronicaCoreMeteoDAL.DSS_Stazioni_X_Monitor_Reader

            Dim dt_SxM = objSxM.Leggi(piva, objParametri_Server)

            Dim elenco As New _elenco_stazioni

            For Each _sm In dt_SxM.Rows

                Dim _ks = _key_stazione.TryNew(_sm("Tipo_Sorgente"), _sm("Stazione_Cod"))

                If _ks IsNot Nothing Then

                    elenco.TryAdd(New _stazione_x_monitor(_ks, CInt(_sm("Monitor_Ore"))))
                End If
            Next

            CompletaStazioni(elenco, piva, objParametri_Server, objParametri_Super_Server)

            risp.RispostaStringa = JsonConvert.SerializeObject(elenco.GetList, Newtonsoft.Json.Formatting.None, New JsonSerializerSettings With {
                                                               .ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                                                               .NullValueHandling = NullValueHandling.Ignore
                                                               })
            risp.RispostaOK = True

        Catch ex As Exception

            risp.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            risp.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return risp
    End Function


    <WebMethod(EnableSession:=True)>
    Public Shared Function RegistraStazioneXMonitor(ByVal tipo_sorgente As Integer, ByVal stazione_cod As Integer, ByVal monitor_ore As Integer, ByVal is_new As Boolean) As RispostaStandard

        Dim risp As New RispostaStandard With {.RispostaOK = False}

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            risp.Sessione = False
            Return risp
        End If

        Try

            Dim piva As String = HttpContext.Current.Session("_piva")

            If String.IsNullOrEmpty(piva) Then
                Dim objParametriAgenda = New ParametriAgenda
                piva = objParametriAgenda.Piva
            End If

            Dim monitorW As New AgronicaCoreMeteoDAL.DSS_Stazioni_X_Monitor_W

            Dim flagRes As Boolean = monitorW.Scrivi(piva, tipo_sorgente, stazione_cod, monitor_ore, is_new, objParametri_Server)

            If flagRes Then

                risp.RispostaStringa = ""
                risp.RispostaOK = True
            End If

        Catch ex As Exception

            risp.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            risp.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return risp
    End Function


    <WebMethod(EnableSession:=True)>
    Public Shared Function EliminaStazioneXMonitor(ByVal tipo_sorgente As Integer, ByVal stazione_cod As Integer) As RispostaStandard

        Dim risp As New RispostaStandard With {.RispostaOK = False}

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            risp.Sessione = False
            Return risp
        End If

        Try

            Dim piva As String = HttpContext.Current.Session("_piva")

            If String.IsNullOrEmpty(piva) Then
                Dim objParametriAgenda = New ParametriAgenda
                piva = objParametriAgenda.Piva
            End If

            Dim monitorW As New AgronicaCoreMeteoDAL.DSS_Stazioni_X_Monitor_W

            Dim bDel As Boolean = monitorW.Cancella(piva, tipo_sorgente, stazione_cod, objParametri_Server)

            If bDel Then

                risp.RispostaStringa = ""
                risp.RispostaOK = True
            End If

        Catch ex As Exception

            risp.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            risp.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return risp
    End Function


    <WebMethod(EnableSession:=True)>
    Public Shared Function LeggiStazioniXMonitoraggioSuolo() As RispostaStandard
        Return LeggiStazioniXControllo("sensore_15")
    End Function


    Private Shared Function LeggiStazioniXControllo(ByVal gruppoParametri As String) As RispostaStandard

        Dim risp As New RispostaStandard With {.RispostaOK = False}

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Super_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Super_Server")

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            risp.Sessione = False
            Return risp
        End If

        Try
            Dim objParametriAgenda = New ParametriAgenda

            Dim piva As String = HttpContext.Current.Session("_piva")

            If String.IsNullOrEmpty(piva) Then
                piva = objParametriAgenda.Piva
            End If

            'Dim objSxC As New AgronicaCoreMeteoDAL.DSS_Stazioni_X_Controllo_R

            'Dim dt_SxC = objSxC.Leggi(piva, "", "", objParametri_Server)

            Dim objSxC As New AgronicaCoreMeteoDAL.DSS_Stazioni_X_Controllo_Reader

            Dim dt_SxC = objSxC.Leggi(piva, Nothing, objParametri_Server)

            Dim elenco As New _elenco_stazioni

            For Each _sm In dt_SxC.Rows

                Dim _ks = _key_stazione.TryNew(_sm("Tipo_Sorgente"), _sm("Stazione_Cod"))

                If _ks IsNot Nothing Then

                    elenco.TryAdd(New _stazione_x_controllo(_ks, _sm("Parametri").ToString, gruppoParametri))
                End If
            Next

            CompletaStazioni(elenco, piva, objParametri_Server, objParametri_Super_Server)

            risp.RispostaStringa = JsonConvert.SerializeObject(elenco.GetList, Newtonsoft.Json.Formatting.None, New JsonSerializerSettings With {
                                                               .ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                                                               .NullValueHandling = NullValueHandling.Ignore
                                                               })
            risp.RispostaOK = True

        Catch ex As Exception

            risp.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            risp.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return risp
    End Function


    <WebMethod(EnableSession:=True)>
    Public Shared Function RegistraStazioneXMonitoraggioSuolo(ByVal tipo_sorgente As Integer, ByVal stazione_cod As Integer, ByVal parametri As String, ByVal is_new As Boolean) As RispostaStandard
        Return RegistraStazioneXControllo(tipo_sorgente, stazione_cod, parametri, "sensore_15", is_new)
    End Function


    Private Shared Function RegistraStazioneXControllo(ByVal tipo_sorgente As Integer, ByVal stazione_cod As Integer, ByVal parametri As String, ByVal gruppoParametri As String, ByVal is_new As Boolean) As RispostaStandard

        Dim risp As New RispostaStandard With {.RispostaOK = False}

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            risp.Sessione = False
            Return risp
        End If

        Try

            Dim piva As String = HttpContext.Current.Session("_piva")

            If String.IsNullOrEmpty(piva) Then
                Dim objParametriAgenda = New ParametriAgenda
                piva = objParametriAgenda.Piva
            End If

            Dim objPar = New JObject

            If Not is_new Then

                Dim objSxC As New AgronicaCoreMeteoDAL.DSS_Stazioni_X_Controllo_Reader

                Dim filtroAggiuntivo As New AgronicaCoreMeteoDAL.DSS_Stazioni_X_Controllo_Reader.Filter With {
                    .TipoSorgente = tipo_sorgente,
                    .StazioneCod = stazione_cod
                }

                Dim dt_SxC = objSxC.Leggi(piva, filtroAggiuntivo, objParametri_Server)

                'Dim objSxC As New AgronicaCoreMeteoDAL.DSS_Stazioni_X_Controllo_R

                'Dim dt_SxC = objSxC.Leggi(piva, "Tipo_Sorgente = " & tipo_sorgente & " AND Stazione_Cod = " & stazione_cod, "", objParametri_Server)

                If dt_SxC IsNot Nothing And dt_SxC.Rows.Count > 0 Then

                    Try
                        objPar = JObject.Parse(dt_SxC.Rows(0)("Parametri"))

                    Catch ex As Exception

                        objPar = New JObject
                    End Try
                End If
            End If

            objPar(gruppoParametri) = JObject.Parse(parametri)

            parametri = objPar.ToString(Newtonsoft.Json.Formatting.None)

            Dim controlloW As New AgronicaCoreMeteoDAL.DSS_Stazioni_X_Controllo_W

            Dim flagRes As Boolean = controlloW.Scrivi(piva, tipo_sorgente, stazione_cod, parametri, is_new, objParametri_Server)

            If flagRes Then

                risp.RispostaStringa = ""
                risp.RispostaOK = True
            End If

        Catch ex As Exception

            risp.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            risp.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return risp
    End Function


    <WebMethod(EnableSession:=True)>
    Public Shared Function EliminaStazioneXControllo(ByVal tipo_sorgente As Integer, ByVal stazione_cod As Integer) As RispostaStandard

        Dim risp As New RispostaStandard With {.RispostaOK = False}

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            risp.Sessione = False
            Return risp
        End If

        Try

            Dim piva As String = HttpContext.Current.Session("_piva")

            If String.IsNullOrEmpty(piva) Then
                Dim objParametriAgenda = New ParametriAgenda
                piva = objParametriAgenda.Piva
            End If

            Dim controlloW As New AgronicaCoreMeteoDAL.DSS_Stazioni_X_Controllo_W

            Dim bDel As Boolean = controlloW.Cancella(piva, tipo_sorgente, stazione_cod, objParametri_Server)

            If bDel Then

                risp.RispostaStringa = ""
                risp.RispostaOK = True
            End If

        Catch ex As Exception

            risp.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            risp.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return risp
    End Function




    <WebMethod(EnableSession:=True)>
    Public Shared Function ModelliPrevisionali_ElaboraIndicatori(ByVal piva As String, ByVal parExtra As String) As rispostaStandard(Of OutputRisultatoIndicatori)

        Dim risp As New rispostaStandard(Of OutputRisultatoIndicatori)

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        Dim objParametri_Super_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Super_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            risp.Sessione = False
            Return risp
        End If

        Lingua.Gias_InizializzaCultura_DaSession()

        Try

            Dim modelliHlp As New ModelliCommon(objParametri_Server, objParametri_Super_Server)

            risp.RispostaStringa = modelliHlp.CalcolaIndicatori(piva, parExtra)
            risp.RispostaOK = True

        Catch ex As Exception

            risp.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            risp.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return risp
    End Function


    Public Class SorgenteMeteo
        Public Tipo As Integer
        Public Stazione As Integer
    End Class

    <WebMethod(EnableSession:=True)>
    Public Shared Function ModelliPrevisionali_Indicatore(DataDa As String, DataA As String,
                                                          SorgentiMeteo As List(Of SorgenteMeteo),
                                                          ModelloPrevisionale As String, Veg_Cod As String, Av_Cod As String,
                                                          Algoritmo As String, ParametriAggiuntivi As String) As rispostaStandard(Of List(Of IndicatoreXSorgente))

        Dim risp As New rispostaStandard(Of List(Of IndicatoreXSorgente))

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Super_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Super_Server")

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) OrElse IsNothing(objParametri_Super_Server) Then
            risp.Sessione = False
            Return risp
        End If

        Lingua.Gias_InizializzaCultura_DaSession()

        Dim linguaSession As Lingua = CType(System.Web.HttpContext.Current.Session("LinguaCorrente"), Lingua)

        Try

            Dim objParams As New JObject

            objParams("DataInizio") = DataDa
            objParams("DataFine") = DataA
            objParams("Mod_Cod") = ModelloPrevisionale
            objParams("Veg_Cod") = Veg_Cod
            objParams("Av_Cod") = Av_Cod
            objParams("Algoritmo") = Algoritmo
            objParams("SorgentiMeteo") = JArray.FromObject(SorgentiMeteo, JsonSerializer.CreateDefault)
            objParams("LinguaCodiceISO") = linguaSession.CodiceISO
            objParams("ParametriElaborazioneAggiuntivi") = JObject.Parse(ParametriAggiuntivi)

            'Dim objMeteo As New AgronicaCoreWebService.MeteoNT
            'Dim ss As String = objMeteo.ModelliPrevisionaliElabora_Indicatore(objParams, objParametri_Server)
            Dim objDifesa As New DSS_Difesa_ModelliPrevisionali(objParametri_Server, objParametri_Super_Server)
            Dim ss As String = objDifesa.ModelliPrevisionaliElabora_Indicatore(objParams)
            Dim jss = New JavaScriptSerializer With {
                .MaxJsonLength = Integer.MaxValue
            }

            risp = jss.Deserialize(Of rispostaStandard(Of List(Of IndicatoreXSorgente)))(ss)

        Catch ex As Exception

            risp.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            risp.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return risp
    End Function


#Region "Gestione alias"



    <WebMethod(EnableSession:=True)>
    Public Shared Function LeggiElencoAlias() As rispostaStandard(Of List(Of StazioneAlias))

        Dim risp As New rispostaStandard(Of List(Of StazioneAlias))

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Super_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Super_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) OrElse IsNothing(objParametri_Super_Server) Then
            risp.Sessione = False
            Return risp
        End If

        Lingua.Gias_InizializzaCultura_DaSession()

        Try
            Dim PIVA_Superuser = objParametri_Server.PivaSuperUser

            Dim objParametriAgenda As New AgronicaCoreModello.ParametriAgenda_Temp.ParametriAgenda

            Dim piva As String = HttpContext.Current.Session("_piva")

            If String.IsNullOrEmpty(piva) Then
                piva = objParametriAgenda.Piva
            End If

            'Dim objMeteo As New AgronicaCoreWebService.MeteoNT
            Dim objMeteo As New InterfacciaMeteoSuite(objParametri_Server, objParametri_Super_Server)

            'Dim elenco = objMeteo.LeggiElencoAlias(objParametri_Server.PivaSuperUser, piva, objParametri_Server)
            Dim elenco = objMeteo.LeggiElencoAlias(objParametri_Server.PivaSuperUser, piva)

            risp.RispostaStringa = elenco
            risp.RispostaOK = True

        Catch ex As Exception

            risp.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            risp.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return risp
    End Function



    <WebMethod(EnableSession:=True)>
    Public Shared Function LeggiStazioneXAlias(Lat As Decimal, Lng As Decimal) As rispostaStandard(Of StazioneAliasNew)

        Dim risp As New rispostaStandard(Of StazioneAliasNew) With {.RispostaOK = False}

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Super_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Super_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) OrElse IsNothing(objParametri_Super_Server) Then
            risp.Sessione = False
            Return risp
        End If

        Lingua.Gias_InizializzaCultura_DaSession()

        Try

            Dim PIVA_Superuser = objParametri_Server.PivaSuperUser

            Dim objParametriAgenda As New AgronicaCoreModello.ParametriAgenda_Temp.ParametriAgenda

            Dim piva As String = HttpContext.Current.Session("_piva")

            If String.IsNullOrEmpty(piva) Then
                piva = objParametriAgenda.Piva
            End If

            'Dim objMeteoNT As New AgronicaCoreWebService.MeteoNT
            Dim objMeteoSuite As New InterfacciaMeteoSuite(objParametri_Server, objParametri_Super_Server)

            'risp.RispostaStringa = objMeteoNT.LeggiStazioneXAlias(PIVA_Superuser, piva, Lat, Lng, objParametri_Server)
            risp.RispostaStringa = objMeteoSuite.LeggiStazioneXAlias(PIVA_Superuser, piva, Lat, Lng)

            risp.RispostaOK = True

        Catch ex As Exception

            risp.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            risp.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return risp
    End Function



    <WebMethod(EnableSession:=True)>
    Public Shared Function UpsertAlias(id As Integer, value As String) As rispostaStandard(Of StazioneAlias)

        Dim risp As New rispostaStandard(Of StazioneAlias) With {.RispostaOK = False}

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Super_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Super_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) OrElse IsNothing(objParametri_Super_Server) Then
            risp.Sessione = False
            Return risp
        End If

        Lingua.Gias_InizializzaCultura_DaSession()

        Try

            Dim PIVA_Superuser = objParametri_Server.PivaSuperUser

            Dim objParametriAgenda As New AgronicaCoreModello.ParametriAgenda_Temp.ParametriAgenda

            Dim piva As String = HttpContext.Current.Session("_piva")

            If String.IsNullOrEmpty(piva) Then
                piva = objParametriAgenda.Piva
            End If

            'Dim objMeteoNT As New AgronicaCoreWebService.MeteoNT
            Dim objMeteoSuite As New InterfacciaMeteoSuite(objParametri_Server, objParametri_Super_Server)

            'risp.RispostaStringa = objMeteoNT.UpsertAlias(PIVA_Superuser, piva, id, value, objParametri_Server)
            risp.RispostaStringa = objMeteoSuite.UpsertAlias(PIVA_Superuser, piva, id, value)

            risp.RispostaOK = True


        Catch ex As Exception

            risp.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            risp.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return risp
    End Function



    <WebMethod(EnableSession:=True)>
    Public Shared Function DeleteAlias(id As Integer) As RispostaStandard

        Dim risp As New RispostaStandard With {.RispostaOK = False}

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Super_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Super_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) OrElse IsNothing(objParametri_Super_Server) Then
            risp.Sessione = False
            Return risp
        End If

        Lingua.Gias_InizializzaCultura_DaSession()

        Try

            Dim PIVA_Superuser = objParametri_Server.PivaSuperUser

            Dim objParametriAgenda As New AgronicaCoreModello.ParametriAgenda_Temp.ParametriAgenda

            Dim piva As String = HttpContext.Current.Session("_piva")

            If String.IsNullOrEmpty(piva) Then
                piva = objParametriAgenda.Piva
            End If

            'Dim objMeteoNT As New AgronicaCoreWebService.MeteoNT
            Dim objMeteoSuite As New InterfacciaMeteoSuite(objParametri_Server, objParametri_Super_Server)

            'If objMeteoNT.DeleteAlias(PIVA_Superuser, piva, id, objParametri_Server) Then
            If objMeteoSuite.DeleteAlias(PIVA_Superuser, piva, id) Then

                risp.RispostaStringa = ""

                risp.RispostaOK = True

            End If

        Catch ex As Exception

            risp.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            risp.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return risp
    End Function



#End Region



#Region "DSS Difesa"

    <WebMethod(EnableSession:=True)>
    Public Shared Function ControllaDSSDifesa(ByVal parametri As RisultatoElaborazioneIndicatori.Indicatore.ParametriElaborazione) As RispostaStandard

        Dim risp As New RispostaStandard

        Try

            Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
                risp.Sessione = False
                Return risp
            End If

            Dim Data_Esecuzione As Date = New Date(Now.Year, Now.Month, Now.Day, 0, 0, 0)

            Dim Obj As New AgronicaCoreMeteoDAL.DSS_Difesa_R

            Dim DT = Obj.Leggi(parametri.Stazione_Cod, parametri.Tipo_Sorgente, parametri.Avv_Cod, parametri.Veg_Cod,
                                parametri.Alg_Cod, AgronicaCoreModelsSTD.meteo.Consiglio_Irrigazione.Provider_DSS_Irrigazione.GIAS_Irriframe, parametri.Mod_Cod,
                                Data_Esecuzione, AGRODATAINIZIO, AGRODATAFINE, "", "", objParametri_Server)

            risp.RispostaOK = True
            risp.RispostaStringa = If(Not IsNothing(DT) AndAlso DT.Rows.Count > 0, DT.Rows(0)("ID").ToString(), "0")

        Catch ex As Exception

            risp.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            risp.Errore = "Errore durante l'operazione: " & vbCrLf &
            AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return risp
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function SalvaDSSDifesa(ByVal parametri As RisultatoElaborazioneIndicatori.Indicatore.ParametriElaborazione,
                                          ByVal dataInizio As Date,
                                          ByVal dataFine As Date,
                                          ByVal risultatoStr As String) As RispostaStandard

        Dim risp As New RispostaStandard

        Try
            Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
                risp.Sessione = False
                Return risp
            End If

            Dim Data_Esecuzione As Date = New Date(Now.Year, Now.Month, Now.Day, 0, 0, 0)

            Dim Obj As New AgronicaCoreMeteoDAL.DSS_Difesa_W

            Dim ScritturaOk = Obj.Scrivi(parametri.Stazione_Cod, parametri.Tipo_Sorgente, parametri.Avv_Cod,
                                        parametri.Veg_Cod, parametri.Alg_Cod, AgronicaCoreModelsSTD.meteo.Consiglio_Irrigazione.Provider_DSS_Irrigazione.GIAS_Irriframe,
                                        parametri.Mod_Cod, Data_Esecuzione, risultatoStr, dataInizio, dataFine, objParametri_Server)

            risp.RispostaOK = ScritturaOk
            risp.RispostaStringa = ""

        Catch ex As Exception

            risp.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            risp.Errore = "Errore durante l'operazione: " & vbCrLf &
            AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return risp
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function LeggiIntervalloMinTrattamento(Fr_Cod As Integer, Veg_Cod As Integer, Av_Cod As Integer, Data As Date, FormulatiXAllegatiNormative_IDRiga As Integer)
        Dim r As New RispostaStandard

        Dim ObjParametri_Super_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Super_Server")
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")

        Try

            Dim strErr As String = ""
            Dim dt As DataTable = AgronicaCoreWebService.DosiEtichetta_WS.DosiEtichetta_Elenco(Fr_Cod:=Fr_Cod,
                                                                                   Veg_Cod:=Veg_Cod,
                                                                                   Tipo_Richiesto:=0,
                                                                                   Av_Cod:=Av_Cod,
                                                                                   Av_Gru:=0,
                                                                                   Data:=CStr(Data),
                                                                                   Grfi_cod:=0,
                                                                                   FormulatiXAllegatiNormative_IDRiga:=FormulatiXAllegatiNormative_IDRiga,
                                                                                   Copertura:="",
                                                                                   ObjParametri_Super_Server,
                                                                                   objParametri_Server,
                                                                                   objParametri_Utenti,
                                                                                   strErr)

            Dim IntervalloTrattamenti_Min As Integer = 0

            If dt.Rows.Count > 1 Then

                Dim dr = dt.Select("FormulatiXAllegatiNormative_IDRiga = " & FormulatiXAllegatiNormative_IDRiga)
                If dr.Length > 0 Then
                    IntervalloTrattamenti_Min = dr(0)("IntervalloTrattamenti_Min")
                Else
                    IntervalloTrattamenti_Min = dt.Rows(0)("IntervalloTrattamenti_Min")
                End If

            ElseIf dt.Rows.Count > 0 Then

                IntervalloTrattamenti_Min = dt.Rows(0)("IntervalloTrattamenti_Min")

            End If

            r.RispostaOK = True
            r.RispostaStringa = JsonConvert.SerializeObject(IntervalloTrattamenti_Min)

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = ex.Message

        End Try

        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function LeggiSogliaMinimaPerPioggeCumulative()
        Dim r As New RispostaStandard

        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")

        Dim Impostazione_Cod As Integer = enum_Impostazioni_Utenti.SogliaMinimaPioggeGiornaliere '901
        Dim result As Integer = 0

        Try

            Dim objUtentiImpostazioni As New AgronicaCoreUtentiBIZ.Utenti_Impostazioni_R
            Dim dictImpostazioni = objUtentiImpostazioni.LeggiImpostazioniScalare(objParametri_Utenti.UtenteUsername, New List(Of Integer) From {Impostazione_Cod}, objParametri_Utenti)

            If dictImpostazioni.Count > 0 AndAlso dictImpostazioni.ContainsKey(Impostazione_Cod) Then

                Integer.TryParse(dictImpostazioni(Impostazione_Cod), result)

            End If

            r.RispostaOK = True
            r.RispostaStringa = JsonConvert.SerializeObject(result)

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = ex.Message

        End Try

        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function LeggiImpostazioneVisualizzaRiepilogoCopertura()
        Dim r As New RispostaStandard

        Dim ObjParametri_Super_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Super_Server")
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")

        Dim objParametriAgenda = New ParametriAgenda

        Dim result As Boolean = False

        Try
            Dim PossoLeggereDaImpianti As Boolean = True

            Dim objImpreseImpostazioni As New AgronicaCoreAnagrafeDAL.Imprese_Impostazioni_R

            Dim ImpostazioneValore = objImpreseImpostazioni.LeggiScalareMulticentroAziendaSuperUser(
                    objParametriAgenda.Piva, New List(Of Integer)({0}),
                    enum_Impostazioni_Utenti.DSS_Difesa_Riepilogo_Copertura_Trattamento, "0",
                    objParametri_Utenti, objParametri_Server)

            If Not ImpostazioneValore = "0" Then
                result = True
            End If

            r.RispostaOK = True
            r.RispostaStringa = JsonConvert.SerializeObject(result)

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = ex.Message

        End Try

        Return r
    End Function

#End Region


    <WebMethod(EnableSession:=True)>
    Public Shared Function LeggiPathNetCoreApi()
        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try
            Dim objConfigurazioneSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
            Dim pathNetCoreApi = objConfigurazioneSiti.Leggi_Valore(0, "GiasOnline_NetCore_API", "", "", objParametri_Server)

            r.RispostaOK = True
            r.RispostaStringa = pathNetCoreApi

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = ex.Message
        End Try

        Return r
    End Function


End Class
