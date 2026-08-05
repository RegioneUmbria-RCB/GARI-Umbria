Imports System.ComponentModel
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.My.Resources
Imports AgronicaCoreIOTBIZ
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreWebService
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
' <System.Web.Script.Services.ScriptService()> _
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<System.Web.Script.Services.ScriptService()>
Public Class DatiReteAcqua
    Inherits System.Web.Services.WebService

    <WebMethod()>
    Public Function HelloWorld() As String
        Return "Hello World"
    End Function

    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function LeggiTipologiaDispositivi_CoreWS() As RispostaStandard
        Dim xRisp As New RispostaStandard

        Try
            Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
                xRisp.Sessione = False
                Return xRisp
            End If

            Dim PIVA_Superuser As String = objParametri_server.PivaSuperUser

            Dim IOTDAL As New AgronicaCoreIOTDAL.IOT

            Dim elenco = IOTDAL.LeggiTipologiaDispositivi(PIVA_Superuser, objParametri_server)

            Dim sorgenti = JArray.FromObject(elenco)

            xRisp.RispostaStringa = sorgenti.ToString()
            xRisp.RispostaOK = True

        Catch ex As Exception
            xRisp.RispostaOK = False
            xRisp.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return xRisp

    End Function

    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function LeggiDispositiviXSorgente_CoreWS(ByVal PIVA As String, ByVal TipoSorgente As Integer, ByVal lat As Decimal, ByVal lng As Decimal) As RispostaStandard
        Dim xRisp As New RispostaStandard

        Try
            Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
                xRisp.Sessione = False
                Return xRisp
            End If

            Dim PIVA_Superuser As String = objParametri_server.PivaSuperUser

            Dim jElencoDispositivi As New List(Of AgronicaCoreDTOStd.InData.IoT.ElencoDispositivi)

            'If iData.InData.ElencoDispositivi IsNot Nothing Then
            '    jElencoDispositivi = iData.InData.ElencoDispositivi
            'Else
            '    jElencoDispositivi.Add(New AgronicaCoreDTOStd.InData.IoT.ElencoDispositivi() With {.TipoSorgente = TipoSorgente})
            'End If

            Dim IOT As New AgronicaCoreIOTBIZ.InterfacciaIOT(objParametri_server)

            Dim stazArr As New JArray

            For Each jsc In jElencoDispositivi

                Dim _tipoSorgente As Integer = jsc.TipoSorgente

                Dim dispDT = IOT.LeggiDispositiviAutorizzati(PIVA_Superuser, PIVA, _tipoSorgente, lat, lng)

                If dispDT IsNot Nothing AndAlso dispDT.Rows.Count > 0 Then

                    Dim strFilter As String = ""

                    If jsc.elenco_dispositivi IsNot Nothing Then

                        Dim disp_list As New List(Of String)
                        For Each c In jsc.elenco_dispositivi
                            disp_list.Add(c.dispositivo_cod)
                        Next

                        strFilter = "Id_dispositivo in (" & String.Join(", ", disp_list) & ")"
                    End If

                    Dim dt_rows = dispDT.Select(strFilter)

                    For Each _row In dt_rows

                        Dim stazObj As New JObject

                        stazObj("tipo_sorgente") = _tipoSorgente
                        stazObj("id_dispositivo") = CInt(_row("id_dispositivo"))
                        stazObj("nome_dispositivo") = _row("nome_dispositivo").ToString
                        stazObj("fornitore") = _row("Fornitore").ToString
                        stazObj("rif_fornitore") = _row("RifFornitore").ToString
                        stazObj("flag_reale") = CBool(_row("FlagReale"))

                        If Not IsDBNull(_row("Lat")) AndAlso Not IsDBNull(_row("Lng")) Then
                            Dim latlng As New JObject
                            latlng("lat") = Convert.ToDecimal(_row("Lat"))  'Globalization.CultureInfo.InvariantCulture
                            latlng("lng") = Convert.ToDecimal(_row("Lng"))  'Globalization.CultureInfo.InvariantCulture

                            stazObj("geo") = latlng
                        End If

                        If Not IsDBNull(_row("UltimoAggiornamento")) Then
                            stazObj("ultimo_aggiornamento") = Convert.ToDateTime(_row("UltimoAggiornamento"))
                        End If

                        If Not IsDBNull(_row("Distanza")) Then
                            stazObj("distanza") = Convert.ToDecimal(_row("Distanza"))
                        End If

                        stazArr.Add(stazObj)
                    Next
                End If

            Next

            xRisp.RispostaStringa = stazArr.ToString()
            xRisp.RispostaOK = True

        Catch ex As Exception
            xRisp.RispostaOK = False
            xRisp.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return xRisp

    End Function

    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function LeggiAnagraficaDispositivi_CoreWS(ByVal PIVA As String) As RispostaStandard
        Dim xRisp As New RispostaStandard

        Try
            Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
                xRisp.Sessione = False
                Return xRisp
            End If

            Dim PIVA_Superuser As String = objParametri_server.PivaSuperUser

            Dim iot As New AgronicaCoreIOTDAL.IOT

            Dim stazDT = iot.LeggiAnagraficaDispositivi(PIVA_Superuser, PIVA, 0, objParametri_server)

            Dim stazArr = ArrayAnagDispositivi(stazDT)

            xRisp.RispostaStringa = stazArr.ToString()
            xRisp.RispostaOK = True

        Catch ex As Exception
            xRisp.RispostaOK = False
            xRisp.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try
        Return xRisp
    End Function

    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function DatiIOTElabora_CoreWS(ByVal TipoSorgente As Integer, ByVal Sorgente As Integer, ByVal dataInizio As DateTime, ByVal dataFine As DateTime, ByVal frequenzaDati As String, ByVal LinguaCodiceISO As String) As RispostaStandard
        Dim xRisp As New RispostaStandard

        Try
            Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
                xRisp.Sessione = False
                Return xRisp
            End If

            Dim PIVA_Superuser As String = objParametri_server.PivaSuperUser

            'Prelevo il parametro della lingua
            Dim lang As String = "it"
            If LinguaCodiceISO <> "" Then
                lang = LinguaCodiceISO
            End If
            'imposto la lingua nel thread corrente per ottenere le voci tradotte dai file di risorse
            Threading.Thread.CurrentThread.CurrentUICulture = New Globalization.CultureInfo(lang)

            Dim _periodi As New List(Of Periodo)
            _periodi.Add(New Periodo With {
                         .DataInizio = dataInizio,
                         .DataFine = dataFine,
                         .Output = Nothing
                         })

            If Math.Abs(DateDiff(DateInterval.Day, dataFine, dataInizio)) > 500 Then
                xRisp.RispostaOK = False
                'r.Errore = "Intervallo Date maggiore di " & CStr(LimiteGG) & " giorni non consentito"
                xRisp.Errore = String.Format(
                    "Intervallo Date maggiore di {0} giorni non consentito",
                    CStr(500)
                )
                Return xRisp
            End If

            Dim iot As New AgronicaCoreIOTBIZ.InterfacciaIOT(objParametri_server)

            Dim granul As AgronicaCoreIOTDAL.IOT.enum_GranularitaDati = AgronicaCoreIOTDAL.IOT.enum_GranularitaDati.Orari
            If frequenzaDati = "G" Then
                granul = AgronicaCoreIOTDAL.IOT.enum_GranularitaDati.Giornalieri
            End If

            If _periodi.Count = 1 Then
                'i18n
                Dim mOutput = iot.LeggiDati(TipoSorgente, Sorgente, dataInizio, dataFine, granul, 0, 0)

                If mOutput IsNot Nothing Then
                    'i18n
                    xRisp.RispostaStringa = JsonConvert.SerializeObject(ConvertiOutput(mOutput, granul))
                    xRisp.RispostaOK = True

                Else
                    xRisp.RispostaOK = False
                    'r.Errore = "Non sono presenti dati meteo nel periodo " & dataInizio.ToString("d MMMM yyyy") & " - " & dataFine.ToString("d MMMM yyyy")
                    xRisp.Errore = String.Format(
                        "Non sono presenti dati meteo nel periodo {0} - {1}",
                        dataInizio.ToString("d MMMM yyyy"),
                        dataFine.ToString("d MMMM yyyy")
                    )

                End If

            End If
        Catch ex As Exception
            xRisp.RispostaOK = False
            xRisp.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return xRisp
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiTipologiaDispositivi(ByVal InData As Object) As RispostaStandard
        Dim xRisp As New RispostaStandard

        Try

            InData = JsonConvert.DeserializeObject(Of CoreWS_Generic(Of AgronicaCoreDTOStd.InData.IoT.TipologiaDispositivi))(JsonConvert.SerializeObject(InData))
            Dim iData As CoreWS_Generic(Of AgronicaCoreDTOStd.InData.IoT.TipologiaDispositivi) = JsonConvert.DeserializeObject(Of CoreWS_Generic(Of AgronicaCoreDTOStd.InData.IoT.TipologiaDispositivi))(JsonConvert.SerializeObject(InData))

            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(iData.objP.objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(iData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(iData.objP.objP_utenti)

            Dim PIVA_Superuser As String = objParametri_Server.PivaSuperUser

            Dim IOTDAL As New AgronicaCoreIOTDAL.IOT

            Dim elenco = IOTDAL.LeggiTipologiaDispositivi(PIVA_Superuser, objParametri_Server)

            Dim sorgenti = JArray.FromObject(elenco)

            xRisp.RispostaStringa = sorgenti.ToString()
            xRisp.RispostaOK = True

        Catch ex As Exception

            xRisp.RispostaOK = False
            xRisp.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return xRisp
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiDispositiviXSorgente(InData As Object) As RispostaStandard
        Dim xRisp As New RispostaStandard

        Try

            'InData = JsonConvert.DeserializeObject(Of CoreWSRequest(Of CoreWS_Generic(Of AgronicaCoreDTOStd.InData.IoT.DispositiviXSorgente)))(JsonConvert.SerializeObject(InData))
            Dim iData As CoreWS_Generic(Of AgronicaCoreDTOStd.InData.IoT.DispositiviXSorgente) = JsonConvert.DeserializeObject(Of CoreWS_Generic(Of AgronicaCoreDTOStd.InData.IoT.DispositiviXSorgente))(JsonConvert.SerializeObject(InData))

            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(iData.objP.objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(iData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(iData.objP.objP_utenti)

            'Dim objParams As JObject '= MeteoWSParametri.Decrypt(inputParams)

            Dim PIVA_Superuser As String = objParametri_Server.PivaSuperUser
            Dim PIVA As String = iData.InData.PIVA
            Dim TipoSorgente As Integer = iData.InData.TipoSorgente

            Dim lat As Decimal = 0
            Dim lng As Decimal = 0
            If iData.InData.DistanzaDa IsNot Nothing Then
                lat = iData.InData.DistanzaDa.lat 'Convert.ToDecimal(objParams("DistanzaDa")("lat"))
                lng = iData.InData.DistanzaDa.lng 'Convert.ToDecimal(objParams("DistanzaDa")("lng"))
            End If

            Dim jElencoDispositivi As New List(Of AgronicaCoreDTOStd.InData.IoT.ElencoDispositivi)

            If iData.InData.ElencoDispositivi IsNot Nothing Then
                jElencoDispositivi = iData.InData.ElencoDispositivi
            Else
                jElencoDispositivi.Add(New AgronicaCoreDTOStd.InData.IoT.ElencoDispositivi() With {.TipoSorgente = TipoSorgente})
            End If

            'Dim objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri = getObjParametri()

            Dim IOT As New AgronicaCoreIOTBIZ.InterfacciaIOT(objParametri_Server)

            Dim stazArr As New JArray

            For Each jsc In jElencoDispositivi

                Dim _tipoSorgente As Integer = jsc.TipoSorgente

                Dim dispDT = IOT.LeggiDispositiviAutorizzati(PIVA_Superuser, PIVA, _tipoSorgente, lat, lng)

                If dispDT IsNot Nothing AndAlso dispDT.Rows.Count > 0 Then

                    Dim strFilter As String = ""

                    If jsc.elenco_dispositivi IsNot Nothing Then

                        Dim disp_list As New List(Of String)
                        For Each c In jsc.elenco_dispositivi
                            disp_list.Add(c.dispositivo_cod)
                        Next

                        strFilter = "Id_dispositivo in (" & String.Join(", ", disp_list) & ")"
                    End If

                    Dim dt_rows = dispDT.Select(strFilter)

                    For Each _row In dt_rows

                        Dim stazObj As New JObject

                        stazObj("tipo_sorgente") = _tipoSorgente
                        stazObj("id_dispositivo") = CInt(_row("id_dispositivo"))
                        stazObj("nome_dispositivo") = _row("nome_dispositivo").ToString
                        stazObj("fornitore") = _row("Fornitore").ToString
                        stazObj("rif_fornitore") = _row("RifFornitore").ToString
                        stazObj("flag_reale") = CBool(_row("FlagReale"))

                        If Not IsDBNull(_row("Lat")) AndAlso Not IsDBNull(_row("Lng")) Then
                            Dim latlng As New JObject
                            latlng("lat") = Convert.ToDecimal(_row("Lat"))  'Globalization.CultureInfo.InvariantCulture
                            latlng("lng") = Convert.ToDecimal(_row("Lng"))  'Globalization.CultureInfo.InvariantCulture

                            stazObj("geo") = latlng
                        End If

                        If Not IsDBNull(_row("UltimoAggiornamento")) Then
                            stazObj("ultimo_aggiornamento") = Convert.ToDateTime(_row("UltimoAggiornamento"))
                        End If

                        If Not IsDBNull(_row("Distanza")) Then
                            stazObj("distanza") = Convert.ToDecimal(_row("Distanza"))
                        End If

                        stazArr.Add(stazObj)
                    Next
                End If

            Next

            xRisp.RispostaStringa = stazArr.ToString()
            xRisp.RispostaOK = True

        Catch ex As Exception

            xRisp.RispostaOK = False
            xRisp.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return xRisp
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiAnagraficaDispositivi(ByVal InData As Object) As RispostaStandard

        Dim xRisp As New RispostaStandard

        Try

            InData = JsonConvert.DeserializeObject(Of CoreWS_Generic(Of AgronicaCoreDTOStd.InData.IoT.LeggiAnagraficaDispositivi))(JsonConvert.SerializeObject(InData))
            Dim iData As CoreWS_Generic(Of AgronicaCoreDTOStd.InData.IoT.LeggiAnagraficaDispositivi) = JsonConvert.DeserializeObject(Of CoreWS_Generic(Of AgronicaCoreDTOStd.InData.IoT.LeggiAnagraficaDispositivi))(JsonConvert.SerializeObject(InData))

            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(iData.objP.objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(iData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(iData.objP.objP_utenti)

            Dim PIVA_Superuser As String = objParametri_Server.PivaSuperUser
            Dim PIVA As String = iData.InData.PIVA

            'Dim objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri = getObjParametri()

            Dim iot As New AgronicaCoreIOTDAL.IOT

            Dim stazDT = iot.LeggiAnagraficaDispositivi(PIVA_Superuser, PIVA, 0, objParametri_Server)

            Dim stazArr = ArrayAnagDispositivi(stazDT)

            xRisp.RispostaStringa = stazArr.ToString()
            xRisp.RispostaOK = True

        Catch ex As Exception

            xRisp.RispostaOK = False
            xRisp.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return xRisp
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiDispositivo(InData As Object) As RispostaStandard

        Dim risp As New RispostaStandard With {.RispostaOK = False}

        Dim iData As CoreWS_Generic(Of AgronicaCoreDTOStd.InData.IoT.LeggiDispositivoIn) = JsonConvert.DeserializeObject(Of CoreWS_Generic(Of AgronicaCoreDTOStd.InData.IoT.LeggiDispositivoIn))(JsonConvert.SerializeObject(InData))

        Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(iData.objP.objP_super_server)
        Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(iData.objP.objP_server)
        Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(iData.objP.objP_utenti)

        Try

            'Dim piva As String = HttpContext.Current.Session("_piva")

            'If String.IsNullOrEmpty(piva) Then
            '    Dim objParametriAgenda As New AgronicaCoreModello.ParametriAgenda_Temp.ParametriAgenda
            '    piva = objParametriAgenda.Piva
            'End If

            Dim IOT As New AgronicaCoreIOTBIZ.InterfacciaIOT(objParametri_Server)

            Dim res = IOT.LeggiDispositivo(iData.InData)

            Dim objLoc As New JObject From {
                {"coordinates", New JObject From {
                {"lat", res.Lat},
                {"lng", res.Lng}
            }}}

            risp.RispostaStringa = objLoc.ToString()

            risp.RispostaOK = True

        Catch ex As Exception

            risp.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            risp.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return risp
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function DatiIOTElabora(ByVal InData As Object) As rispostaStandard(Of AgronicaCoreDTOStd.InData.IoT.RisultatoIOT)

        Dim r As New rispostaStandard(Of AgronicaCoreDTOStd.InData.IoT.RisultatoIOT) With {
            .RispostaStringa = New AgronicaCoreDTOStd.InData.IoT.RisultatoIOT()
        }

        Dim iData As CoreWS_Generic(Of AgronicaCoreDTOStd.InData.IoT.RichiediDatiIOT) = JsonConvert.DeserializeObject(Of CoreWS_Generic(Of AgronicaCoreDTOStd.InData.IoT.RichiediDatiIOT))(JsonConvert.SerializeObject(InData))

        Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(iData.objP.objP_super_server)
        Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(iData.objP.objP_server)
        Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(iData.objP.objP_utenti)

        Dim PIVA_Superuser As String = objParametri_Server.PivaSuperUser


        Dim tipoSorgente As Integer = iData.InData.TipoSorgente
        Dim sorgente As Integer = iData.InData.Sorgente
        Dim dataInizio As DateTime = iData.InData.dataInizio
        Dim dataFine As DateTime = iData.InData.dataFine
        Dim frequenzaDati As String = iData.InData.frequenzaDati

        'Prelevo il parametro della lingua
        Dim linguaCodiceISO As String = "it"
        If iData.InData.LinguaCodiceISO <> "" Then
            linguaCodiceISO = iData.InData.LinguaCodiceISO
        End If
        'imposto la lingua nel thread corrente per ottenere le voci tradotte dai file di risorse
        Threading.Thread.CurrentThread.CurrentUICulture = New Globalization.CultureInfo(linguaCodiceISO)

        Dim _periodi As New List(Of Periodo)
        _periodi.Add(New Periodo With {
                     .DataInizio = dataInizio,
                     .DataFine = dataFine,
                     .Output = Nothing
                     })

        Try



            If Math.Abs(DateDiff(DateInterval.Day, dataFine, dataInizio)) > 500 Then
                r.RispostaOK = False
                'r.Errore = "Intervallo Date maggiore di " & CStr(LimiteGG) & " giorni non consentito"
                r.Errore = String.Format(
                    "Intervallo Date maggiore di {0} giorni non consentito",
                    CStr(500)
                )
                Return r
            End If

            Dim iot As New AgronicaCoreIOTBIZ.InterfacciaIOT(objParametri_Server)

            Dim granul As AgronicaCoreIOTDAL.IOT.enum_GranularitaDati = AgronicaCoreIOTDAL.IOT.enum_GranularitaDati.Orari
            If frequenzaDati = "G" Then
                granul = AgronicaCoreIOTDAL.IOT.enum_GranularitaDati.Giornalieri
            End If

            If _periodi.Count = 1 Then
                'i18n
                Dim mOutput = iot.LeggiDati(tipoSorgente, sorgente, dataInizio, dataFine, granul, 0, 0)

                If mOutput IsNot Nothing Then
                    'i18n
                    r.RispostaStringa = ConvertiOutput(mOutput, granul)
                    r.RispostaOK = True

                Else

                    r.RispostaOK = False
                    'r.Errore = "Non sono presenti dati meteo nel periodo " & dataInizio.ToString("d MMMM yyyy") & " - " & dataFine.ToString("d MMMM yyyy")
                    r.Errore = String.Format(
                        "Non sono presenti dati meteo nel periodo {0} - {1}",
                        dataInizio.ToString("d MMMM yyyy"),
                        dataFine.ToString("d MMMM yyyy")
                    )

                End If

            End If

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = Gias.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiElencoTipiSensorePerDispositivo(ByVal InData As Object) As rispostaStandard(Of List(Of AgronicaCoreDTOStd.InData.IoT.TipologiaSensoriPerDispositivo))
        Dim risp As New rispostaStandard(Of List(Of AgronicaCoreDTOStd.InData.IoT.TipologiaSensoriPerDispositivo)) With {.RispostaOK = False}

        Dim iData As CoreWS_Generic(Of AgronicaCoreDTOStd.InData.IoT.LeggiDispositivoIn) = JsonConvert.DeserializeObject(Of CoreWS_Generic(Of AgronicaCoreDTOStd.InData.IoT.LeggiDispositivoIn))(JsonConvert.SerializeObject(InData))

        Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(iData.objP.objP_super_server)
        Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(iData.objP.objP_server)
        Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(iData.objP.objP_utenti)

        Try
            Dim iot As New AgronicaCoreIOTBIZ.InterfacciaIOT(objParametri_Server)
            Dim dt = iot.LeggiElencoTipiSensorePerDispositivo(iData.InData.id_dispositivo)

            risp.RispostaStringa = New List(Of AgronicaCoreDTOStd.InData.IoT.TipologiaSensoriPerDispositivo)
            For Each row In dt.Rows
                risp.RispostaStringa.Add(New AgronicaCoreDTOStd.InData.IoT.TipologiaSensoriPerDispositivo() With {
                                            .id_sensore = row("Id_Sensore"),
                                            .id_tiposensore = row("Id_TipoSensore"),
                                            .tipo = row("Tipo"),
                                            .UM = row("UM")
                                         })
            Next

        Catch ex As Exception
            risp.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            risp.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try
        Return risp
    End Function



    Private Function ConvertiOutput(ByVal iot_out As IOT_Output, ByVal granul As AgronicaCoreMeteoDAL.MeteoNT.enum_GranularitaDati) As AgronicaCoreDTOStd.InData.IoT.RisultatoIOT

        If iot_out Is Nothing Then
            Return Nothing
        End If

        Dim om As New OutputModelloIOT
        'i18n argomento "nomeOut"
        If granul = AgronicaCoreMeteoDAL.MeteoNT.enum_GranularitaDati.Giornalieri Then
            om.aggiungiColonna("DataOra", GetType(Date), Gias.Data, "dd/MM/yyyy")
        Else
            om.aggiungiColonna("DataOra", GetType(DateTime), Gias.DataOra, "dd/MM/yyyy HH\""h\""")
        End If

        Dim coltitle As String
        For Each sens In iot_out.Sensori.Rows

            coltitle = sens("Sensore").ToString
            If Not String.IsNullOrEmpty(sens("UM").ToString) Then
                coltitle &= " (" & sens("UM").ToString & ")"
            End If
            'i18n Questi nomi di colonna sono definiti su db
            om.aggiungiColonna(sens("NomeColonna").ToString, GetType(Decimal), coltitle, "0.00")
        Next

        For Each row In iot_out.Dati.Rows

            om.AddField(row("DataOra"))

            For Each sens In iot_out.Sensori.Rows
                om.AddField(row(sens("NomeColonna").ToString))
            Next

            om.Commit()

        Next

        Return New AgronicaCoreDTOStd.InData.IoT.RisultatoIOT With {
            .Chart_Type = "",
            .Meteo_Table = om.Output(),
            .Meteo_Charts = iot_out.Charts.ToString(),
            .Meteo_RiepilogoPeriodo = "",
            .Meteo_RiepilogoSensori = JsonConvert.SerializeObject(iot_out.RiepilogoSensori, New JsonSerializerSettings With {.NullValueHandling = NullValueHandling.Ignore})
        }
    End Function

    Private Function ArrayAnagDispositivi(ByVal dispDT As DataTable) As JArray

        Dim dispArr As New JArray

        If dispDT IsNot Nothing Then

            Dim disp_id As Integer = 0

            Dim dispObj As JObject = Nothing
            Dim sensArr = New JArray

            For Each r In dispDT.Rows

                Dim e_disp_id As Integer = CInt(r("Id_Dispositivo"))

                If e_disp_id <> disp_id Then

                    If dispObj IsNot Nothing Then
                        dispArr.Add(dispObj)
                    End If

                    disp_id = e_disp_id

                    dispObj = New JObject

                    dispObj("Disp_Id") = e_disp_id
                    dispObj("Disp_Nome") = r("Dispositivo").ToString
                    If Not IsDBNull(r("Lat_Dec")) AndAlso Not IsDBNull(r("Lng_Dec")) Then
                        Dim geoObj As New JObject
                        geoObj("Lat") = CDec(r("Lat_Dec"))
                        geoObj("Lng") = CDec(r("Lng_Dec"))
                        dispObj("Disp_Geo") = geoObj
                    End If
                    dispObj("Fornitore") = r("Fornitore").ToString
                    dispObj("RifFornitore") = r("RifFornitore").ToString
                    dispObj("Proprietario") = CBool(r("Proprietario"))
                    dispObj("FlagReale") = CBool(r("FlagReale"))

                    sensArr = New JArray
                    dispObj("Sensori") = sensArr
                End If

                Dim sensObj As New JObject
                sensObj("Id_Sensore") = CInt(r("Id_Sensore"))
                sensObj("Sensore") = r("Sensore").ToString
                sensObj("Tipo_Sensore") = r("Tipo_Sensore").ToString
                sensObj("UM") = r("UM").ToString

                Dim objOutputConfig As New JObject
                If Not String.IsNullOrEmpty(r("OutputConfig").ToString()) Then
                    objOutputConfig = JObject.Parse(r("OutputConfig").ToString())
                End If
                sensObj("OutputConfig") = objOutputConfig

                sensObj("Id_DispositivoOrigine") = CInt(r("Id_DispositivoOrigine"))
                sensObj("DispositivoOrigine") = r("DispositivoOrigine").ToString
                If r("Last_Update") IsNot Nothing AndAlso Not IsDBNull(r("Last_Update")) Then
                    sensObj("Last_Update") = CDate(r("Last_Update"))
                End If

                sensArr.Add(sensObj)
            Next

            If dispObj IsNot Nothing Then
                dispArr.Add(dispObj)
            End If

        End If

        Return dispArr
    End Function

End Class