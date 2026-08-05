
Imports AgronicaCoreAnagrafeDAL
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreMeteoBiz
Imports AgronicaCoreMeteoCommon
Imports AgronicaCoreMeteoDAL
Imports AgronicaCoreModelliPrevisionaliBIZ
Imports AgronicaCoreVarieBIZ
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

Public Class ModelliCommon

    Private ReadOnly _objParametri_Server As AgronicaCoreParametri
    Private ReadOnly _objParametri_Super_Server As AgronicaCoreParametri

    Public Sub New(objParametriServer As AgronicaCoreParametri, objParametriSuperServer As AgronicaCoreParametri)

        _objParametri_Server = objParametriServer
        _objParametri_Super_Server = objParametriSuperServer
    End Sub

    Public Function CalcolaIndicatori(piva As String, parExtra As String) As OutputRisultatoIndicatori

        Dim listaModelli As New List(Of ModelloConfigurato)

        Dim SxM As New DSS_Stazioni_X_Modelli_Reader

        Dim dtSxM = SxM.Leggi(piva, Nothing, _objParametri_Server)

        If dtSxM.Rows.Count > 0 Then

            Dim jsonConf As String = JsonConvert.SerializeObject(dtSxM)
            listaModelli = JsonConvert.DeserializeObject(Of List(Of ModelloConfigurato))(jsonConf)
        Else

            'non ho configurazione
            listaModelli = ModelliDaImpianti(piva)
        End If

        If listaModelli.Count = 0 Then

            Return New OutputRisultatoIndicatori With {.Stato = RisultatoElaborazioneIndicatori.Stato_Elaborazione.NonDisponibili}
        End If

        'Imposto un ChiaveRichiesta per riuscire a gestire i risultati in seguito
        Dim ChiaveRichiesta As Integer = 1
        For Each modello In listaModelli
            modello.ChiaveRichiesta = ChiaveRichiesta
            ChiaveRichiesta += 1
        Next

        'Non serializzo IdGroup, DescrGroup ed AuxData che non mi servono per la richiesta di elaborazione
        Dim jsonList = JsonConvert.SerializeObject(listaModelli)
        Dim arrModelli = JsonConvert.DeserializeObject(Of JArray)(jsonList)

        'Sostituisco la stringa ParametriElaborazione con un oggetto
        For Each obj As JObject In arrModelli
            Dim obj_pe = JObject.Parse(obj("ParametriElaborazione").ToString())
            obj("ParametriElaborazione").Replace(obj_pe)
        Next

        Dim objRequest = New JObject(New JProperty("ElencoModelli", arrModelli))
        If Not String.IsNullOrEmpty(parExtra) Then
            objRequest.Add("ParametriExtra", JObject.Parse(parExtra))
        End If

        Dim Forecast_gg As Integer = ForecastDaImpostazioni(piva)

        objRequest.Add("Forecast_gg", Forecast_gg)

        Dim objParams As New JObject(New JProperty("Doorkey", GetDoorkey()),
                                         New JProperty("PivaSuperuser", _objParametri_Server.PivaSuperUser),
                                         New JProperty("Piva", piva),
                                         New JProperty("Request", objRequest))

        'Dim objMeteo As New AgronicaCoreWebService.MeteoNT
        Dim objDifesa As New DSS_Difesa_ModelliPrevisionali(_objParametri_Server, _objParametri_Super_Server)

        'Dim json As String = objMeteo.ModelliPrevisionaliElaboraIndicatoriV3(objParams, _objParametri_Server)
        Dim json As String = objDifesa.ModelliPrevisionaliElaboraIndicatoriV3(objParams)

        Dim OutRisIndic = JsonConvert.DeserializeObject(Of rispostaStandard(Of OutputRisultatoIndicatori))(json).RispostaStringa

        'Trasformo il risultato per essere passato al client con le info che servono
        For Each indic In OutRisIndic.Indicatori

            Dim idx As Integer = 0

            While idx < listaModelli.Count

                If listaModelli(idx).ChiaveRichiesta = indic.ChiaveRichiesta Then

                    If listaModelli(idx).IdGroup > 0 Then

                        indic.Stazione = listaModelli(idx).DescrGroup
                    End If

                    indic.AuxData = listaModelli(idx).AuxData

                    listaModelli.RemoveAt(idx)

                    Exit While
                End If

                idx += 1
            End While
        Next

        Return OutRisIndic
    End Function


    Private Function ForecastDaImpostazioni(piva As String) As Integer

        Dim objImpreseImpostazioni As New Imprese_Impostazioni_R

        Dim dtImpostazioneForecast = objImpreseImpostazioni.Leggi(
                piva,
                CostantiPersonalizzate.SACOD_NOFILTRO,
                enum_Impostazioni_Utenti.SUPERUSER_ELABORAZIONE_DSS_FORECAST,
                "",
                "",
                _objParametri_Server)

        If dtImpostazioneForecast IsNot Nothing AndAlso dtImpostazioneForecast.Rows.Count > 0 Then
            Try
                Dim impVal As Integer

                If Not Integer.TryParse(dtImpostazioneForecast.Rows(0).Field(Of String)("Impostazione_Valore"), impVal) Then
                    impVal = 0
                End If

                Return impVal

            Catch ex As Exception

            End Try
        End If

        Return 3
    End Function


    Private Function ModelliDaImpianti(piva As String) As List(Of ModelloConfigurato)

        Dim PossoLeggereDaImpianti As Boolean = True

        Dim objImpreseImpostazioni As New Imprese_Impostazioni_R

        Dim dtImpostazioneImpianti = objImpreseImpostazioni.Leggi(
                    piva,
                    CostantiPersonalizzate.SACOD_NOFILTRO,
                    enum_Impostazioni_Utenti.SUPERUSER_ELABORAZIONE_DSS_DA_IMPIANTI,
                    enumSelezioneVariabile.Selezione_JoinCompleta,
                    "",
                    "",
                    _objParametri_Server)

        If dtImpostazioneImpianti IsNot Nothing AndAlso dtImpostazioneImpianti.Rows.Count > 0 Then
            Try
                Dim impVal As Integer

                If Not Integer.TryParse(dtImpostazioneImpianti.Rows(0).Field(Of String)("Impostazione_Valore"), impVal) Then
                    impVal = 0
                End If

                PossoLeggereDaImpianti = impVal = 1

            Catch ex As Exception

            End Try
        End If

        If Not PossoLeggereDaImpianti Then

            Return New List(Of ModelloConfigurato)
        End If


        ' popola la tabella con i modelli (autorizzati) per le colture in anagrafica
        Dim rispModAut = LeggiModelliAutorizzati(0, piva)

        If String.IsNullOrEmpty(rispModAut.RispostaStringa) Then

            Return New List(Of ModelloConfigurato)
        End If

        Dim modAut = JArray.Parse(rispModAut.RispostaStringa)

        If Not modAut.Any() Then

            Return New List(Of ModelloConfigurato)
        End If


        ' leggo le impostazioni disponibili
        Dim modImpReader = New DSS_ModelliImpostazioni
        Dim dtModImp = modImpReader.Leggi("", "", _objParametri_Server)

        Dim dictModelli As New Dictionary(Of Integer, List(Of ModelloAutorizzato))

        For Each vegObj As JObject In modAut

            Dim veg_cod As Integer = vegObj("Veg_Cod")

            Dim lModAut As New List(Of ModelloAutorizzato)

            For Each modObj As JObject In vegObj("Modelli")

                If Not CBool(modObj("HasIndic")) Then

                    Continue For
                End If

                Dim mod_cod As Integer = modObj("Mod_Cod")
                Dim avv_cod As Integer = modObj("Avv_Cod")
                Dim alg_cod As Integer = modObj("Alg_Cod")

                Dim datiMeteoInizio_gg As Integer = 1
                Dim datiMeteoFine_gg As Integer = 365
                Dim idx As Integer = 0

                While idx < dtModImp.Rows.Count
                    Dim imp = dtModImp.Rows(idx)
                    If CInt(imp("Mod_Cod")) = mod_cod And
                        CInt(imp("Veg_Cod")) = veg_cod And
                        CInt(imp("Avv_Cod")) = avv_cod And
                        CInt(imp("Alg_Cod")) = alg_cod Then

                        datiMeteoInizio_gg = CInt(imp("DatiMeteoInizio_gg"))
                        datiMeteoFine_gg = CInt(imp("DatiMeteoFine_gg"))

                        idx = dtModImp.Rows.Count
                    End If
                    idx += 1
                End While

                lModAut.Add(
                    New ModelloAutorizzato With {
                    .Mod_Cod = mod_cod,
                    .Avv_Cod = avv_cod,
                    .Alg_Cod = alg_cod,
                    .ParametriElaborazione = JsonConvert.SerializeObject(New JObject),
                    .InizioPeriodo_gg = 1,
                    .FinePeriodo_gg = 300,
                    .Validita_minuti = 60,
                    .DatiMeteoInizio_gg = datiMeteoInizio_gg,
                    .DatiMeteoFine_gg = datiMeteoFine_gg
                    })
            Next

            If lModAut.Any Then

                dictModelli.Add(veg_cod, lModAut)
            End If

        Next

        Dim objRead As New Reg_Impianti_Read

        Dim dtImp = objRead.Leggi_x_DSS_Difesa(piva, dictModelli.Keys.ToArray(), _objParametri_Server)

        If dtImp.Rows.Count = 0 Then

            Return New List(Of ModelloConfigurato)
        End If

        ' mappo ogni Lat/Lng in Stazione_Cod

        Dim json As String = JsonConvert.SerializeObject(dtImp)
        Dim listaImpianti = JsonConvert.DeserializeObject(Of List(Of ImpiantoLocalizzato))(json)

        Dim listaStazioni As New List(Of StazioneDaCoordParam)

        For Each imp As ImpiantoLocalizzato In listaImpianti
            listaStazioni.Add(
                New StazioneDaCoordParam With {
                .Id = imp.Id,
                .Lat = imp.Lat,
                .Lng = imp.Lng,
                .Id_Stazione = 0
                })
        Next

        listaStazioni = StazioniDaLatLng(listaStazioni, piva)

        If listaStazioni.Count = 0 Then

            Return New List(Of ModelloConfigurato)
        End If

        Dim dictStaz As New Dictionary(Of Integer, Integer)
        For Each s In listaStazioni
            dictStaz.Add(s.Id, s.Id_Stazione)
        Next

        Dim listaModelli As New List(Of ModelloConfigurato)

        ' ad ogni impianto associo i modelli e la stazione

        For Each il As ImpiantoLocalizzato In listaImpianti

            Dim id_stazione As Integer

            If dictStaz.TryGetValue(il.Id, id_stazione) And dictModelli.ContainsKey(il.Veg_Cod) Then

                For Each m As ModelloAutorizzato In dictModelli(il.Veg_Cod)

                    Dim auxData = New With {
                        .ChiaveImpianto = New With {il.PIva, il.Sa_Cod, il.Appezza, il.Id_Reg},
                        .Appezzamento = il.App_Des,
                        .Specie = il.Veg_Des,
                        .Varieta = il.Cul_Des
                    }

                    listaModelli.Add(
                        New ModelloConfigurato With {
                        .Tipo_Sorgente = enum_Meteo_Tiposorgente.Pubbliche,
                        .Stazione_Cod = id_stazione,
                        .Mod_Cod = m.Mod_Cod,
                        .Veg_Cod = il.Veg_Cod,
                        .Avv_Cod = m.Avv_Cod,
                        .Alg_Cod = m.Alg_Cod,
                        .ParametriElaborazione = m.ParametriElaborazione,
                        .InizioPeriodo_gg = m.InizioPeriodo_gg,
                        .FinePeriodo_gg = m.FinePeriodo_gg,
                        .Validita_minuti = m.Validita_minuti,
                        .DatiMeteoInizio_gg = m.DatiMeteoInizio_gg,
                        .DatiMeteoFine_gg = m.DatiMeteoFine_gg,
                        .IdGroup = il.Id,
                        .DescrGroup = il.App_Des,
                        .AuxData = auxData
                        })
                Next
            End If

        Next

        listaModelli.Sort(Function(m1 As ModelloConfigurato, m2 As ModelloConfigurato)
                              If m1.Tipo_Sorgente < m2.Tipo_Sorgente Then
                                  Return -1
                              ElseIf m1.Tipo_Sorgente > m2.Tipo_Sorgente Then
                                  Return 1
                              End If
                              If m1.Stazione_Cod < m2.Stazione_Cod Then
                                  Return -1
                              ElseIf m1.Stazione_Cod > m2.Stazione_Cod Then
                                  Return 1
                              End If
                              If m1.Mod_Cod < m2.Mod_Cod Then
                                  Return -1
                              ElseIf m1.Mod_Cod > m2.Mod_Cod Then
                                  Return 1
                              End If
                              If m1.Veg_Cod < m2.Veg_Cod Then
                                  Return -1
                              ElseIf m1.Veg_Cod > m2.Veg_Cod Then
                                  Return 1
                              End If
                              Return 0
                          End Function)

        Return listaModelli
    End Function


    Private Function LeggiModelliAutorizzati(Veg_Cod As Integer, piva As String) As RispostaStandard

        Dim r As New RispostaStandard

        If IsNothing(_objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try

            Dim objParametriAgenda As New AgronicaCoreModello.ParametriAgenda_Temp.ParametriAgenda

            If String.IsNullOrEmpty(piva) Then
                piva = objParametriAgenda.Piva
            End If

            Dim objParams As New JObject
            objParams("Doorkey") = GetDoorkey()
            objParams("PIVA_Superuser") = _objParametri_Server.PivaSuperUser
            objParams("PIVA") = piva
            objParams("Veg_Cod") = Veg_Cod

            'Dim objMeteo As New AgronicaCoreWebService.MeteoNT
            Dim objDifesa As New DSS_Difesa_ModelliPrevisionali(_objParametri_Server, _objParametri_Super_Server)

            'Dim json = JObject.Parse(objMeteo.LeggiModelliAutorizzati(objParams, _objParametri_Server))
            Dim json = JObject.Parse(objDifesa.LeggiModelliAutorizzati(objParams))

            Dim jarr = JArray.Parse(json("RispostaStringa").ToString)

            Dim elencomodelli_helper As New ElencoModelliAutorizzati

            r.RispostaStringa = elencomodelli_helper.Genera(jarr, _objParametri_Server)

            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r
    End Function


    Private Class StazioneDaCoordParam
        Public Id As Integer
        Public Lat As Decimal
        Public Lng As Decimal
        Public Id_Stazione As Integer
    End Class


    Private Function StazioniDaLatLng(lista As List(Of StazioneDaCoordParam), piva As String) As List(Of StazioneDaCoordParam)

        Dim outList As New List(Of StazioneDaCoordParam)

        If IsNothing(_objParametri_Server) Then
            Return outList
        End If

        Try

            Dim objParametriAgenda As New AgronicaCoreModello.ParametriAgenda_Temp.ParametriAgenda

            If String.IsNullOrEmpty(piva) Then
                piva = objParametriAgenda.Piva
            End If

            Dim objParams As New JObject
            objParams("Doorkey") = GetDoorkey()
            objParams("PIVA_Superuser") = _objParametri_Server.PivaSuperUser
            objParams("PIVA") = piva
            objParams("ElencoPosizioni") = JArray.FromObject(lista).ToString()

            'Dim objMeteo As New AgronicaCoreWebService.MeteoNT
            Dim objMeteo As New InterfacciaMeteoSuite(_objParametri_Server, _objParametri_Super_Server)

            'Dim json = JObject.Parse(objMeteo.StazioniDaLatLng(objParams, _objParametri_Server))
            Dim json = JObject.Parse(objMeteo.StazioniDaLatLng(objParams))

            outList = JsonConvert.DeserializeObject(Of List(Of StazioneDaCoordParam))(json("RispostaStringa"))

        Catch ex As Exception

            'r.RispostaOK = False

            ''uso questa funzione per ottenere il Messaggio..:
            'r.Errore = "Errore durante l'operazione: " & vbCrLf &
            'AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

            Return New List(Of StazioneDaCoordParam)
        End Try

        Return outList
    End Function


    Private Function GetDoorkey() As String
        Dim Doorkey As String = "Y4h8u3B5w2"
        Return Doorkey
    End Function


End Class



'Ex CoreWS Meteo.asmx.vb : ModelliPrevisionali_ElaboraIndicatori
#If False Then



            'Dim listaModelli As New List(Of ModelloConfigurato)

            'Dim SxM As New AgronicaCoreMeteoDAL.DSS_Stazioni_X_Modelli_Reader

            'Dim dtSxM = SxM.Leggi(piva, Nothing, objParametri_Server)

            'Dim GroupByStation As Boolean = True

            'Dim objImpreseImpostazioni = New AgronicaCoreAnagrafeDAL.Imprese_Impostazioni_R

            'If dtSxM.Rows.Count > 0 Then

            '    Dim jsonConf As String = JsonConvert.SerializeObject(dtSxM)
            '    listaModelli = JsonConvert.DeserializeObject(Of List(Of ModelloConfigurato))(jsonConf)
            'Else
            '    'non ho configurazione

            '    Dim OkImpostazioneImpianti As Boolean = False

            '    Dim dtImpostazioneImpianti = objImpreseImpostazioni.Leggi(
            '        piva,
            '        CostantiPersonalizzate.SACOD_NOFILTRO,
            '        enum_Impostazioni_Utenti.SUPERUSER_ELABORAZIONE_DSS_DA_IMPIANTI,
            '        enumSelezioneVariabile.Selezione_JoinCompleta,
            '        "",
            '        "",
            '        objParametri_Server)

            '    If dtImpostazioneImpianti IsNot Nothing AndAlso dtImpostazioneImpianti.Rows.Count > 0 Then
            '        Try
            '            Dim impVal As Integer

            '            If Not Integer.TryParse(dtImpostazioneImpianti.Rows(0).Field(Of String)("Impostazione_Valore"), impVal) Then
            '                impVal = 0
            '            End If

            '            OkImpostazioneImpianti = impVal = 1

            '        Catch ex As Exception

            '        End Try
            '    End If

            '    If OkImpostazioneImpianti Then
            '        ' l'impostazione me lo permette

            '        listaModelli = ModelliDaImpianti(piva, objParametri_Server)
            '        GroupByStation = False
            '    End If
            'End If

            'Dim arrModelli As JArray = JArray.FromObject(listaModelli, JsonSerializer.CreateDefault)

            'For Each obj As JObject In arrModelli
            '    Dim obj_pe = JObject.Parse(obj("ParametriElaborazione").ToString())
            '    obj("ParametriElaborazione").Replace(obj_pe)
            'Next

            'Dim objRequest = New JObject(New JProperty("ElencoModelli", arrModelli))
            'If Not String.IsNullOrEmpty(parExtra) Then
            '    objRequest.Add("ParametriExtra", JObject.Parse(parExtra))
            'End If

            ''Quando si chiama il ModelliPrevisionaliElaboraIndicatoriV3 deve diventare Forecast_gg
            'Dim Forecast_gg As Integer = 3

            'Dim dtImpostazioneForecast = objImpreseImpostazioni.Leggi(
            '    piva,
            '    CostantiPersonalizzate.SACOD_NOFILTRO,
            '    enum_Impostazioni_Utenti.SUPERUSER_ELABORAZIONE_DSS_FORECAST,
            '    "",
            '    "",
            '    objParametri_Server)

            'If dtImpostazioneForecast IsNot Nothing AndAlso dtImpostazioneForecast.Rows.Count > 0 Then
            '    Try
            '        Dim impVal As Integer

            '        If Not Integer.TryParse(dtImpostazioneForecast.Rows(0).Field(Of String)("Impostazione_Valore"), impVal) Then
            '            impVal = 0
            '        End If

            '        Forecast_gg = impVal

            '    Catch ex As Exception

            '    End Try
            'End If

            'objRequest.Add("Forecast_gg", Forecast_gg)

            'Dim objParams As New JObject(New JProperty("Doorkey", GetDoorkey()),
            '                             New JProperty("PivaSuperuser", objParametri_Server.PivaSuperUser),
            '                             New JProperty("Piva", piva),
            '                             New JProperty("Request", objRequest))

            'Dim objMeteo As New AgronicaCoreWebService.MeteoNT

            'Dim bUseV2 As Boolean = True

            'If bUseV2 Then

            '    'Dim json As String = objMeteo.ModelliPrevisionaliElaboraIndicatoriV2(objParams, objParametri_Server)
            '    Dim json As String = objMeteo.ModelliPrevisionaliElaboraIndicatoriV3(objParams, objParametri_Server)

            '    Dim j_s_s = New JavaScriptSerializer With {
            '        .MaxJsonLength = Integer.MaxValue
            '    }

            '    risp.RispostaStringa = j_s_s.Deserialize(Of rispostaStandard(Of OutputRisultatoIndicatori))(json).RispostaStringa
            '    risp.RispostaOK = True

            '    Return risp
            'End If

            ''OLD
            'Dim ss As String = objMeteo.ModelliPrevisionaliElaboraIndicatori(objParams, objParametri_Server)

            'Dim jss = New JavaScriptSerializer With {
            '    .MaxJsonLength = Integer.MaxValue
            '}

            'Dim rei = jss.Deserialize(Of rispostaStandard(Of RisultatoElaborazioneIndicatori))(ss).RispostaStringa

            'Dim output As New OutputRisultatoIndicatori(rei)

            'If output.Stato = RisultatoElaborazioneIndicatori.Stato_Elaborazione.Pronti Then

            '    Dim elenco As New _elenco_stazioni
            '    Dim dictModelli As New Dictionary(Of _key_modello, List(Of OutputRisultatoIndicatori.OutputIndicatore))
            '    Dim j_modelli As New JArray

            '    For Each oi In output.Indicatori

            '        Dim ks = New _key_stazione With {
            '            .tipo_sorgente = oi.Parametri.Tipo_Sorgente,
            '            .stazione_cod = oi.Parametri.Stazione_Cod
            '        }

            '        Dim staz As _stazione_x_indicatori = elenco.TryAdd(New _stazione_x_indicatori(ks))
            '        staz.indicatori.Add(oi)

            '        Dim km As New _key_modello With {
            '            .mod_cod = oi.Parametri.Mod_Cod,
            '            .veg_cod = oi.Parametri.Veg_Cod,
            '            .avv_cod = oi.Parametri.Avv_Cod,
            '            .alg_cod = oi.Parametri.Alg_Cod,
            '            .params = oi.Parametri.ParametriElaborazione
            '        }

            '        If Not dictModelli.ContainsKey(km) Then

            '            dictModelli.Add(km, New List(Of OutputRisultatoIndicatori.OutputIndicatore))

            '            j_modelli.Add(New JObject(
            '              New JProperty("mod_cod", km.mod_cod),
            '              New JProperty("veg_cod", km.veg_cod),
            '              New JProperty("avv_cod", km.avv_cod),
            '              New JProperty("alg_cod", km.alg_cod),
            '              New JProperty("params", km.params)))
            '        End If
            '        dictModelli(km).Add(oi)
            '    Next

            '    CompletaStazioni(elenco, piva, objParametri_Server)

            '    Dim listaStazioni = elenco.GetList(Of _stazione_x_indicatori)

            '    For Each staz In listaStazioni
            '        For Each indic In staz.indicatori
            '            indic.Stazione = staz.stazione_name
            '        Next
            '    Next

            '    Dim objParamsModelli As New JObject(
            '    New JProperty("Doorkey", GetDoorkey()),
            '    New JProperty("PIVA_Superuser", objParametri_Server.PivaSuperUser),
            '    New JProperty("PIVA", piva),
            '    New JProperty("Modelli", j_modelli))

            '    Dim smodelli As String = objMeteo.CompletaOutputModelli(objParamsModelli, objParametri_Server)
            '    Dim jrisp = JObject.Parse(smodelli)
            '    smodelli = jrisp("RispostaStringa").ToString

            '    If Not String.IsNullOrEmpty(smodelli) Then

            '        j_modelli = JArray.Parse(smodelli)

            '        For Each jModObj In j_modelli

            '            Dim km As New _key_modello With {
            '                .mod_cod = CInt(jModObj("mod_cod")),
            '                .veg_cod = CInt(jModObj("veg_cod")),
            '                .avv_cod = CInt(jModObj("avv_cod")),
            '                .alg_cod = CInt(jModObj("alg_cod")),
            '                .params = jModObj("params").ToString
            '            }

            '            If dictModelli.ContainsKey(km) Then
            '                Dim mod_des = jModObj("mod_des").ToString
            '                Dim alg_des = jModObj("alg_des").ToString
            '                If Not String.IsNullOrEmpty(alg_des) Then
            '                    mod_des &= " (" & alg_des & ")"
            '                End If
            '                Dim mod_des_agg = jModObj("mod_des_agg").ToString
            '                If Not String.IsNullOrEmpty(mod_des_agg) Then
            '                    mod_des &= " [" & mod_des_agg & "]"
            '                End If

            '                For Each oi In dictModelli(km)
            '                    oi.Modello = mod_des
            '                    oi.Specie = jModObj("veg_des").ToString()
            '                    oi.Avversita = jModObj("avv_des").ToString()
            '                    oi.DescrParametri = jModObj("param_des").ToString()
            '                Next
            '            End If
            '        Next
            '    End If
            'End If

            'risp.RispostaStringa = output
            'risp.RispostaOK = True






    'Private Shared Function GetDoorkey() As String
    '    Dim Doorkey As String = "Y4h8u3B5w2"
    '    Return Doorkey
    'End Function


    'Private Shared Sub CompletaStazioni(ByRef stazioni As _elenco_stazioni, ByVal piva As String, ByRef objParametri_Server As AgronicaCoreParametri)

    '    Dim j_stazioni As New JArray
    '    Dim j_staz As JArray
    '    For Each kvp In stazioni.DictOfTipoSorgente()

    '        j_staz = New JArray
    '        For Each s In kvp.Value
    '            j_staz.Add(New JObject(New JProperty("stazione_cod", s)))
    '        Next

    '        j_stazioni.Add(New JObject(New JProperty("tipo_sorgente", kvp.Key), New JProperty("elenco_stazioni", j_staz)))
    '    Next

    '    Dim objParams As New JObject
    '    objParams("PIVA_Superuser") = objParametri_Server.PivaSuperUser
    '    objParams("PIVA") = piva
    '    objParams("ElencoStazioni") = j_stazioni

    '    'Dim linguaSession As Lingua = CType(System.Web.HttpContext.Current.Session("LinguaCorrente"), Lingua)
    '    'objParams("LinguaCodiceISO") = linguaSession.CodiceISO

    '    Dim objMeteoNT As New AgronicaCoreWebService.MeteoNT

    '    Dim s_risp As String = objMeteoNT.LeggiElencoStazioni(objParams, objParametri_Server)
    '    Dim j_risp = JObject.Parse(s_risp)
    '    j_stazioni = JArray.Parse(j_risp("RispostaStringa"))

    '    For Each jStazObj In j_stazioni

    '        Dim t_sorgente As Integer = CInt(jStazObj("tipo_sorgente"))
    '        j_staz = jStazObj("elenco_stazioni")

    '        For Each s In j_staz

    '            Dim _ks As New _key_stazione With {.tipo_sorgente = t_sorgente, .stazione_cod = CInt(s("stazione_cod"))}

    '            Dim staz = stazioni.StazioneFromKey(_ks)

    '            If staz IsNot Nothing Then
    '                staz.stazione_name = s("stazione_nome").ToString()
    '                staz.rif_fornitore = s("rif_fornitore").ToString()
    '                staz.flag_reale = CBool(s("flag_reale"))
    '                staz.sensori_des = s("sensori").ToString()
    '            End If
    '        Next
    '    Next
    'End Sub


    'Private Shared Function ModelliDaImpianti(piva As String, objParametri_Server As AgronicaCoreParametri) As List(Of ModelloConfigurato)

    '    ' popola la tabella con i modelli (autorizzati) per le colture in anagrafica
    '    Dim rispModAut = LeggiModelliAutorizzati(0, piva, objParametri_Server)

    '    If String.IsNullOrEmpty(rispModAut.RispostaStringa) Then
    '        Return New List(Of ModelloConfigurato)
    '    End If

    '    Dim modAut = JArray.Parse(rispModAut.RispostaStringa)

    '    If Not modAut.Any() Then
    '        Return New List(Of ModelloConfigurato)
    '    End If


    '    ' leggo le impostazioni disponibili
    '    Dim modImpReader = New AgronicaCoreMeteoDAL.DSS_ModelliImpostazioni
    '    Dim dtModImp = modImpReader.Leggi("", "", objParametri_Server)

    '    Dim dictModelli As New Dictionary(Of Integer, List(Of ModelloAutorizzato))

    '    For Each vegObj As JObject In modAut

    '        Dim veg_cod As Integer = vegObj("Veg_Cod")

    '        Dim lModAut As New List(Of ModelloAutorizzato)

    '        For Each modObj As JObject In vegObj("Modelli")

    '            If Not CBool(modObj("HasIndic")) Then

    '                Continue For
    '            End If

    '            Dim mod_cod As Integer = modObj("Mod_Cod")
    '            Dim avv_cod As Integer = modObj("Avv_Cod")
    '            Dim alg_cod As Integer = modObj("Alg_Cod")

    '            Dim datiMeteoInizio_gg As Integer = 1
    '            Dim datiMeteoFine_gg As Integer = 365
    '            Dim idx As Integer = 0

    '            While idx < dtModImp.Rows.Count
    '                Dim imp = dtModImp.Rows(idx)
    '                If CInt(imp("Mod_Cod")) = mod_cod And
    '                    CInt(imp("Veg_Cod")) = veg_cod And
    '                    CInt(imp("Avv_Cod")) = avv_cod And
    '                    CInt(imp("Alg_Cod")) = alg_cod Then

    '                    datiMeteoInizio_gg = CInt(imp("DatiMeteoInizio_gg"))
    '                    datiMeteoFine_gg = CInt(imp("DatiMeteoFine_gg"))

    '                    idx = dtModImp.Rows.Count
    '                End If
    '                idx += 1
    '            End While

    '            lModAut.Add(
    '                New ModelloAutorizzato With {
    '                .Mod_Cod = mod_cod,
    '                .Avv_Cod = avv_cod,
    '                .Alg_Cod = alg_cod,
    '                .ParametriElaborazione = JsonConvert.SerializeObject(New JObject),
    '                .InizioPeriodo_gg = 1,
    '                .FinePeriodo_gg = 300,
    '                .Validita_minuti = 60,
    '                .DatiMeteoInizio_gg = datiMeteoInizio_gg,
    '                .DatiMeteoFine_gg = datiMeteoFine_gg
    '                })
    '        Next

    '        If lModAut.Any Then

    '            dictModelli.Add(veg_cod, lModAut)
    '        End If

    '    Next

    '    Dim objRead As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read

    '    Dim dtImp = objRead.Leggi_x_DSS_Difesa(piva, dictModelli.Keys.ToArray(), objParametri_Server)

    '    If dtImp.Rows.Count = 0 Then
    '        Return New List(Of ModelloConfigurato)
    '    End If

    '    ' mappo ogni Lat/Lng in Stazione_Cod

    '    Dim json As String = JsonConvert.SerializeObject(dtImp)
    '    Dim listaImpianti = JsonConvert.DeserializeObject(Of List(Of ImpiantoLocalizzato))(json)

    '    Dim listaStazioni As New List(Of StazioneDaCoordParam)

    '    For Each imp As ImpiantoLocalizzato In listaImpianti
    '        listaStazioni.Add(
    '            New StazioneDaCoordParam With {
    '            .Id = imp.Id,
    '            .Lat = imp.Lat,
    '            .Lng = imp.Lng,
    '            .Id_Stazione = 0
    '            })
    '    Next

    '    listaStazioni = StazioniDaLatLng(listaStazioni, piva, objParametri_Server)

    '    If listaStazioni.Count = 0 Then
    '        Return New List(Of ModelloConfigurato)
    '    End If

    '    Dim dictStaz As New Dictionary(Of Integer, Integer)
    '    For Each s In listaStazioni
    '        dictStaz.Add(s.Id, s.Id_Stazione)
    '    Next

    '    Dim listaModelli As New List(Of ModelloConfigurato)

    '    ' ad ogni impianto associo i modelli e la stazione

    '    For Each il As ImpiantoLocalizzato In listaImpianti

    '        Dim id_stazione As Integer

    '        If dictStaz.TryGetValue(il.Id, id_stazione) And dictModelli.ContainsKey(il.Veg_Cod) Then

    '            For Each m As ModelloAutorizzato In dictModelli(il.Veg_Cod)

    '                Dim auxData = New With {il.PIva, il.Sa_Cod, il.Appezza, il.Id_Reg}

    '                listaModelli.Add(
    '                    New ModelloConfigurato With {
    '                    .Tipo_Sorgente = enum_Meteo_Tiposorgente.Pubbliche,
    '                    .Stazione_Cod = id_stazione,
    '                    .Mod_Cod = m.Mod_Cod,
    '                    .Veg_Cod = il.Veg_Cod,
    '                    .Avv_Cod = m.Avv_Cod,
    '                    .Alg_Cod = m.Alg_Cod,
    '                    .ParametriElaborazione = m.ParametriElaborazione,
    '                    .InizioPeriodo_gg = m.InizioPeriodo_gg,
    '                    .FinePeriodo_gg = m.FinePeriodo_gg,
    '                    .Validita_minuti = m.Validita_minuti,
    '                    .DatiMeteoInizio_gg = m.DatiMeteoInizio_gg,
    '                    .DatiMeteoFine_gg = m.DatiMeteoFine_gg,
    '                    .IdGroup = il.Id,
    '                    .DescrGroup = il.App_Des,
    '                    .AuxData = auxData
    '                    })
    '            Next
    '        End If

    '    Next

    '    listaModelli.Sort(Function(m1 As ModelloConfigurato, m2 As ModelloConfigurato)
    '                          If m1.Tipo_Sorgente < m2.Tipo_Sorgente Then
    '                              Return -1
    '                          ElseIf m1.Tipo_Sorgente > m2.Tipo_Sorgente Then
    '                              Return 1
    '                          End If
    '                          If m1.Stazione_Cod < m2.Stazione_Cod Then
    '                              Return -1
    '                          ElseIf m1.Stazione_Cod > m2.Stazione_Cod Then
    '                              Return 1
    '                          End If
    '                          If m1.Mod_Cod < m2.Mod_Cod Then
    '                              Return -1
    '                          ElseIf m1.Mod_Cod > m2.Mod_Cod Then
    '                              Return 1
    '                          End If
    '                          If m1.Veg_Cod < m2.Veg_Cod Then
    '                              Return -1
    '                          ElseIf m1.Veg_Cod > m2.Veg_Cod Then
    '                              Return 1
    '                          End If
    '                          Return 0
    '                      End Function)

    '    Return listaModelli
    'End Function

    'Public Class StazioneDaCoordParam
    '    Public Id As Integer
    '    Public Lat As Decimal
    '    Public Lng As Decimal
    '    Public Id_Stazione As Integer
    'End Class


    'Private Shared Function StazioniDaLatLng(lista As List(Of StazioneDaCoordParam),
    '                                         piva As String,
    '                                         objParametri_Server As AgronicaCoreParametri) As List(Of StazioneDaCoordParam)

    '    Dim outList As New List(Of StazioneDaCoordParam)

    '    If IsNothing(objParametri_Server) Then
    '        Return outList
    '    End If

    '    Try

    '        Dim objParametriAgenda As New AgronicaCoreModello.ParametriAgenda_Temp.ParametriAgenda

    '        If String.IsNullOrEmpty(piva) Then
    '            piva = objParametriAgenda.Piva
    '        End If

    '        Dim objParams As New JObject
    '        objParams("Doorkey") = GetDoorkey()
    '        objParams("PIVA_Superuser") = objParametri_Server.PivaSuperUser
    '        objParams("PIVA") = piva
    '        objParams("ElencoPosizioni") = JArray.FromObject(lista).ToString()

    '        Dim objMeteo As New AgronicaCoreWebService.MeteoNT

    '        Dim json = JObject.Parse(objMeteo.StazioniDaLatLng(objParams, objParametri_Server))

    '        outList = JsonConvert.DeserializeObject(Of List(Of StazioneDaCoordParam))(json("RispostaStringa"))

    '    Catch ex As Exception

    '        'r.RispostaOK = False

    '        ''uso questa funzione per ottenere il Messaggio..:
    '        'r.Errore = "Errore durante l'operazione: " & vbCrLf &
    '        'AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

    '        Return New List(Of StazioneDaCoordParam)
    '    End Try

    '    Return outList
    'End Function

    'Private Shared Function LeggiModelliAutorizzati(ByVal Veg_Cod As Integer,
    '                                                piva As String,
    '                                                objParametri_Server As AgronicaCoreParametri) As RispostaStandard

    '    Dim r As New RispostaStandard

    '    If IsNothing(objParametri_Server) Then
    '        r.Sessione = False
    '        Return r
    '    End If

    '    Try

    '        Dim objParametriAgenda As New AgronicaCoreModello.ParametriAgenda_Temp.ParametriAgenda

    '        If String.IsNullOrEmpty(piva) Then
    '            piva = objParametriAgenda.Piva
    '        End If

    '        Dim objParams As New JObject
    '        objParams("Doorkey") = GetDoorkey()
    '        objParams("PIVA_Superuser") = objParametri_Server.PivaSuperUser
    '        objParams("PIVA") = piva
    '        objParams("Veg_Cod") = Veg_Cod

    '        Dim objMeteo As New AgronicaCoreWebService.MeteoNT

    '        Dim json = JObject.Parse(objMeteo.LeggiModelliAutorizzati(objParams, objParametri_Server))

    '        Dim jarr = JArray.Parse(json("RispostaStringa").ToString)

    '        Dim elencomodelli_helper As New ElencoModelliAutorizzati

    '        r.RispostaStringa = elencomodelli_helper.Genera(jarr, objParametri_Server)

    '        r.RispostaOK = True

    '    Catch ex As Exception

    '        r.RispostaOK = False

    '        'uso questa funzione per ottenere il Messaggio..:
    '        r.Errore = "Errore durante l'operazione: " & vbCrLf &
    '            AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

    '    End Try

    '    Return r
    'End Function

#End If


'EX GiasOnline MeteoWS.aspx.vb : ModelliPrevisionali_ElaboraIndicatori
#If False Then

        'Try

        '    'Dim SxM As New AgronicaCoreMeteoDAL.DSS_Stazioni_X_Modelli_R

        '    'Dim dtSxM = SxM.Leggi(piva, "", "", objParametri_Server)

        '    Dim listaModelli As New List(Of ModelloConfigurato)

        '    Dim SxM As New AgronicaCoreMeteoDAL.DSS_Stazioni_X_Modelli_Reader

        '    Dim dtSxM = SxM.Leggi(piva, Nothing, objParametri_Server)

        '    Dim GroupByStation As Boolean = True

        '    Dim objImpreseImpostazioni = New AgronicaCoreAnagrafeDAL.Imprese_Impostazioni_R

        '    If dtSxM.Rows.Count > 0 Then

        '        Dim json As String = JsonConvert.SerializeObject(dtSxM)
        '        listaModelli = JsonConvert.DeserializeObject(Of List(Of ModelloConfigurato))(json)
        '    Else
        '        'non ho configurazione

        '        Dim OkImpostazioneImpianti As Boolean = True

        '        Dim dtImpostazioneImpianti = objImpreseImpostazioni.Leggi(
        '            piva,
        '            SACOD_NOFILTRO,
        '            enum_Impostazioni_Utenti.SUPERUSER_ELABORAZIONE_DSS_DA_IMPIANTI,
        '            "",
        '            "",
        '            objParametri_Server)

        '        If dtImpostazioneImpianti IsNot Nothing AndAlso dtImpostazioneImpianti.Rows.Count > 0 Then
        '            Try
        '                Dim impVal As Integer

        '                If Not Integer.TryParse(dtImpostazioneImpianti.Rows(0).Field(Of String)("Impostazione_Valore"), impVal) Then
        '                    impVal = 0
        '                End If

        '                OkImpostazioneImpianti = impVal = 1

        '            Catch ex As Exception

        '            End Try
        '        End If

        '        If OkImpostazioneImpianti Then
        '            ' l'impostazione me lo permette

        '            listaModelli = ModelliDaImpianti(piva, objParametri_Server)
        '            GroupByStation = False
        '        End If
        '    End If

        '    Dim arrModelli As JArray = JArray.FromObject(listaModelli, JsonSerializer.CreateDefault)

        '    For Each obj As JObject In arrModelli
        '        Dim obj_pe = JObject.Parse(obj("ParametriElaborazione").ToString())
        '        obj("ParametriElaborazione").Replace(obj_pe)
        '    Next

        '    Dim objRequest = New JObject(New JProperty("ElencoModelli", arrModelli))
        '    If Not String.IsNullOrEmpty(parExtra) Then
        '        objRequest.Add("ParametriExtra", JObject.Parse(parExtra))
        '    End If

        '    'Quando si chiama il ModelliPrevisionaliElaboraIndicatoriV3 deve diventare Forecast_gg
        '    Dim Forecast_gg As Integer = 3

        '    Dim dtImpostazioneForecast = objImpreseImpostazioni.Leggi(
        '        piva,
        '        SACOD_NOFILTRO,
        '        enum_Impostazioni_Utenti.SUPERUSER_ELABORAZIONE_DSS_FORECAST,
        '        "",
        '        "",
        '        objParametri_Server)

        '    If dtImpostazioneForecast IsNot Nothing AndAlso dtImpostazioneForecast.Rows.Count > 0 Then
        '        Try
        '            Dim impVal As Integer

        '            If Not Integer.TryParse(dtImpostazioneForecast.Rows(0).Field(Of String)("Impostazione_Valore"), impVal) Then
        '                impVal = 0
        '            End If

        '            Forecast_gg = impVal

        '        Catch ex As Exception

        '        End Try
        '    End If

        '    objRequest.Add("Forecast_gg", Forecast_gg)

        '    Dim objParams As New JObject(New JProperty("Doorkey", GetDoorkey()),
        '                                 New JProperty("PivaSuperuser", objParametri_Server.PivaSuperUser),
        '                                 New JProperty("Piva", piva),
        '                                 New JProperty("Request", objRequest))

        '    Dim objMeteo As New AgronicaCoreWebService.MeteoNT

        '    Dim bUseV2 As Boolean = True

        '    If bUseV2 Then

        '        'Dim json As String = objMeteo.ModelliPrevisionaliElaboraIndicatoriV2(objParams, objParametri_Server)
        '        Dim json As String = objMeteo.ModelliPrevisionaliElaboraIndicatoriV3(objParams, objParametri_Server)

        '        Dim j_s_s = New JavaScriptSerializer With {
        '            .MaxJsonLength = Integer.MaxValue
        '        }

        '        risp.RispostaStringa = j_s_s.Deserialize(Of rispostaStandard(Of OutputRisultatoIndicatori))(json).RispostaStringa
        '        risp.RispostaOK = True

        '        Return risp
        '    End If


        '    Dim ss As String = objMeteo.ModelliPrevisionaliElaboraIndicatori(objParams, objParametri_Server)

        '    Dim jss = New JavaScriptSerializer With {
        '        .MaxJsonLength = Integer.MaxValue
        '    }

        '    Dim rei = jss.Deserialize(Of rispostaStandard(Of RisultatoElaborazioneIndicatori))(ss).RispostaStringa

        '    Dim output As New OutputRisultatoIndicatori(rei)

        '    If output.Stato = RisultatoElaborazioneIndicatori.Stato_Elaborazione.Pronti Then

        '        If GroupByStation Then

        '            output = PreparaIndicatoriXStazione(output, piva, objParametri_Server)
        '        Else

        '            output = PreparaIndicatoriXImpianti(output, listaModelli, piva, objParametri_Server)
        '        End If
        '    End If

        '    risp.RispostaStringa = output
        '    risp.RispostaOK = True

        'Catch ex As Exception

        '    risp.RispostaOK = False

        '    'uso questa funzione per ottenere il Messaggio..:
        '    risp.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
        '        AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        'End Try

        

    'Private Shared Function ModelliDaImpianti(piva As String, objParametri_Server As AgronicaCoreParametri) As List(Of ModelloConfigurato)

    '    ' popola la tabella con i modelli (autorizzati) per le colture in anagrafica
    '    Dim rispModAut = LeggiModelliAutorizzati(0)

    '    If String.IsNullOrEmpty(rispModAut.RispostaStringa) Then
    '        Return New List(Of ModelloConfigurato)
    '    End If

    '    Dim modAut = JArray.Parse(rispModAut.RispostaStringa)

    '    If Not modAut.Any() Then
    '        Return New List(Of ModelloConfigurato)
    '    End If


    '    ' leggo le impostazioni disponibili
    '    Dim modImpReader = New AgronicaCoreMeteoDAL.DSS_ModelliImpostazioni
    '    Dim dtModImp = modImpReader.Leggi("", "", objParametri_Server)

    '    Dim dictModelli As New Dictionary(Of Integer, List(Of ModelloAutorizzato))

    '    For Each vegObj As JObject In modAut

    '        Dim veg_cod As Integer = vegObj("Veg_Cod")

    '        Dim lModAut As New List(Of ModelloAutorizzato)

    '        For Each modObj As JObject In vegObj("Modelli")

    '            If Not CBool(modObj("HasIndic")) Then

    '                Continue For
    '            End If

    '            Dim mod_cod As Integer = modObj("Mod_Cod")
    '            Dim avv_cod As Integer = modObj("Avv_Cod")
    '            Dim alg_cod As Integer = modObj("Alg_Cod")

    '            Dim datiMeteoInizio_gg As Integer = 1
    '            Dim datiMeteoFine_gg As Integer = 365
    '            Dim idx As Integer = 0

    '            While idx < dtModImp.Rows.Count
    '                Dim imp = dtModImp.Rows(idx)
    '                If CInt(imp("Mod_Cod")) = mod_cod And
    '                    CInt(imp("Veg_Cod")) = veg_cod And
    '                    CInt(imp("Avv_Cod")) = avv_cod And
    '                    CInt(imp("Alg_Cod")) = alg_cod Then

    '                    datiMeteoInizio_gg = CInt(imp("DatiMeteoInizio_gg"))
    '                    datiMeteoFine_gg = CInt(imp("DatiMeteoFine_gg"))

    '                    idx = dtModImp.Rows.Count
    '                End If
    '                idx += 1
    '            End While

    '            lModAut.Add(
    '                New ModelloAutorizzato With {
    '                .Mod_Cod = mod_cod,
    '                .Avv_Cod = avv_cod,
    '                .Alg_Cod = alg_cod,
    '                .ParametriElaborazione = JsonConvert.SerializeObject(New JObject),
    '                .InizioPeriodo_gg = 1,
    '                .FinePeriodo_gg = 300,
    '                .Validita_minuti = 60,
    '                .DatiMeteoInizio_gg = datiMeteoInizio_gg,
    '                .DatiMeteoFine_gg = datiMeteoFine_gg
    '                })
    '        Next

    '        If lModAut.Any Then

    '            dictModelli.Add(veg_cod, lModAut)
    '        End If

    '    Next

    '    Dim objRead As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read

    '    Dim dtImp = objRead.Leggi_x_DSS_Difesa(piva, dictModelli.Keys.ToArray(), objParametri_Server)

    '    If dtImp.Rows.Count = 0 Then
    '        Return New List(Of ModelloConfigurato)
    '    End If

    '    ' mappo ogni Lat/Lng in Stazione_Cod

    '    Dim json As String = JsonConvert.SerializeObject(dtImp)
    '    Dim listaImpianti = JsonConvert.DeserializeObject(Of List(Of ImpiantoLocalizzato))(json)

    '    Dim listaStazioni As New List(Of StazioneDaCoordParam)

    '    For Each imp As ImpiantoLocalizzato In listaImpianti
    '        listaStazioni.Add(
    '            New StazioneDaCoordParam With {
    '            .Id = imp.Id,
    '            .Lat = imp.Lat,
    '            .Lng = imp.Lng,
    '            .Id_Stazione = 0
    '            })
    '    Next

    '    listaStazioni = StazioniDaLatLng(listaStazioni)

    '    If listaStazioni.Count = 0 Then
    '        Return New List(Of ModelloConfigurato)
    '    End If

    '    Dim dictStaz As New Dictionary(Of Integer, Integer)
    '    For Each s In listaStazioni
    '        dictStaz.Add(s.Id, s.Id_Stazione)
    '    Next

    '    Dim listaModelli As New List(Of ModelloConfigurato)

    '    ' ad ogni impianto associo i modelli e la stazione

    '    For Each il As ImpiantoLocalizzato In listaImpianti

    '        Dim id_stazione As Integer

    '        If dictStaz.TryGetValue(il.Id, id_stazione) And dictModelli.ContainsKey(il.Veg_Cod) Then

    '            For Each m As ModelloAutorizzato In dictModelli(il.Veg_Cod)

    '                Dim auxData = New With {il.PIva, il.Sa_Cod, il.Appezza, il.Id_Reg}

    '                listaModelli.Add(
    '                    New ModelloConfigurato With {
    '                    .Tipo_Sorgente = enum_Meteo_Tiposorgente.Pubbliche,
    '                    .Stazione_Cod = id_stazione,
    '                    .Mod_Cod = m.Mod_Cod,
    '                    .Veg_Cod = il.Veg_Cod,
    '                    .Avv_Cod = m.Avv_Cod,
    '                    .Alg_Cod = m.Alg_Cod,
    '                    .ParametriElaborazione = m.ParametriElaborazione,
    '                    .InizioPeriodo_gg = m.InizioPeriodo_gg,
    '                    .FinePeriodo_gg = m.FinePeriodo_gg,
    '                    .Validita_minuti = m.Validita_minuti,
    '                    .DatiMeteoInizio_gg = m.DatiMeteoInizio_gg,
    '                    .DatiMeteoFine_gg = m.DatiMeteoFine_gg,
    '                    .IdGroup = il.Id,
    '                    .DescrGroup = il.App_Des,
    '                    .AuxData = auxData
    '                    })
    '            Next
    '        End If

    '    Next

    '    listaModelli.Sort(Function(m1 As ModelloConfigurato, m2 As ModelloConfigurato)
    '                          If m1.Tipo_Sorgente < m2.Tipo_Sorgente Then
    '                              Return -1
    '                          ElseIf m1.Tipo_Sorgente > m2.Tipo_Sorgente Then
    '                              Return 1
    '                          End If
    '                          If m1.Stazione_Cod < m2.Stazione_Cod Then
    '                              Return -1
    '                          ElseIf m1.Stazione_Cod > m2.Stazione_Cod Then
    '                              Return 1
    '                          End If
    '                          If m1.Mod_Cod < m2.Mod_Cod Then
    '                              Return -1
    '                          ElseIf m1.Mod_Cod > m2.Mod_Cod Then
    '                              Return 1
    '                          End If
    '                          If m1.Veg_Cod < m2.Veg_Cod Then
    '                              Return -1
    '                          ElseIf m1.Veg_Cod > m2.Veg_Cod Then
    '                              Return 1
    '                          End If
    '                          Return 0
    '                      End Function)

    '    Return listaModelli
    'End Function


    'Public Class StazioneDaCoordParam
    '    Public Id As Integer
    '    Public Lat As Decimal
    '    Public Lng As Decimal
    '    Public Id_Stazione As Integer
    'End Class


    'Private Shared Function StazioniDaLatLng(lista As List(Of StazioneDaCoordParam)) As List(Of StazioneDaCoordParam)

    '    Dim outList As New List(Of StazioneDaCoordParam)

    '    Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")

    '    If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
    '        Return outList
    '    End If

    '    Try

    '        Dim objParametriAgenda As New AgronicaCoreModello.ParametriAgenda_Temp.ParametriAgenda

    '        Dim piva As String = HttpContext.Current.Session("_piva")

    '        If String.IsNullOrEmpty(piva) Then
    '            piva = objParametriAgenda.Piva
    '        End If

    '        Dim objParams As New JObject
    '        objParams("Doorkey") = GetDoorkey()
    '        objParams("PIVA_Superuser") = objParametri_Server.PivaSuperUser
    '        objParams("PIVA") = piva
    '        objParams("ElencoPosizioni") = JArray.FromObject(lista).ToString()

    '        Dim objMeteo As New AgronicaCoreWebService.MeteoNT

    '        Dim json = JObject.Parse(objMeteo.StazioniDaLatLng(objParams, objParametri_Server))

    '        outList = JsonConvert.DeserializeObject(Of List(Of StazioneDaCoordParam))(json("RispostaStringa"))

    '    Catch ex As Exception

    '        'r.RispostaOK = False

    '        ''uso questa funzione per ottenere il Messaggio..:
    '        'r.Errore = "Errore durante l'operazione: " & vbCrLf &
    '        'AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

    '        Return New List(Of StazioneDaCoordParam)
    '    End Try

    '    Return outList
    'End Function




    'Private Shared Function PreparaIndicatoriXStazione(output As OutputRisultatoIndicatori, piva As String, objParametri_Server As AgronicaCoreParametri) As OutputRisultatoIndicatori

    '    Dim elenco As New _elenco_stazioni
    '    Dim dictModelli As New Dictionary(Of _key_modello, List(Of OutputRisultatoIndicatori.OutputIndicatore))
    '    Dim lista As List(Of OutputRisultatoIndicatori.OutputIndicatore)

    '    For Each oi In output.Indicatori

    '        Dim ks = New _key_stazione With {
    '            .tipo_sorgente = oi.Parametri.Tipo_Sorgente,
    '            .stazione_cod = oi.Parametri.Stazione_Cod
    '        }

    '        Dim staz As _stazione_x_indicatori = elenco.TryAdd(New _stazione_x_indicatori(ks))
    '        staz.indicatori.Add(oi)

    '        Dim km As New _key_modello With {
    '            .mod_cod = oi.Parametri.Mod_Cod,
    '            .veg_cod = oi.Parametri.Veg_Cod,
    '            .avv_cod = oi.Parametri.Avv_Cod,
    '            .alg_cod = oi.Parametri.Alg_Cod,
    '            .params = oi.Parametri.ParametriElaborazione
    '        }

    '        If Not dictModelli.TryGetValue(km, lista) Then

    '            lista = New List(Of OutputRisultatoIndicatori.OutputIndicatore)
    '            dictModelli.Add(km, lista)
    '        End If

    '        lista.Add(oi)
    '    Next

    '    CompletaStazioni(elenco, piva, objParametri_Server)

    '    Dim listaStazioni = elenco.GetList(Of _stazione_x_indicatori)

    '    For Each staz In listaStazioni
    '        For Each indic In staz.indicatori
    '            indic.Stazione = staz.stazione_name
    '        Next
    '    Next

    '    Dim arr_modelli = JArray.Parse(JsonConvert.SerializeObject(dictModelli.Keys))

    '    Dim objParamsModelli As New JObject(
    '            New JProperty("Doorkey", GetDoorkey()),
    '            New JProperty("PIVA_Superuser", objParametri_Server.PivaSuperUser),
    '            New JProperty("PIVA", piva),
    '            New JProperty("Modelli", arr_modelli))

    '    Dim objMeteo As New AgronicaCoreWebService.MeteoNT

    '    Dim smodelli As String = objMeteo.CompletaOutputModelli(objParamsModelli, objParametri_Server)
    '    Dim jrisp = JObject.Parse(smodelli)
    '    smodelli = jrisp("RispostaStringa").ToString

    '    If Not String.IsNullOrEmpty(smodelli) Then

    '        arr_modelli = JArray.Parse(smodelli)

    '        For Each jModObj In arr_modelli

    '            Dim km As New _key_modello With {
    '                        .mod_cod = CInt(jModObj("mod_cod")),
    '                        .veg_cod = CInt(jModObj("veg_cod")),
    '                        .avv_cod = CInt(jModObj("avv_cod")),
    '                        .alg_cod = CInt(jModObj("alg_cod")),
    '                        .params = jModObj("params").ToString
    '                    }

    '            If dictModelli.ContainsKey(km) Then
    '                Dim mod_des = jModObj("mod_des").ToString
    '                Dim alg_des = jModObj("alg_des").ToString
    '                If Not String.IsNullOrEmpty(alg_des) Then
    '                    mod_des &= " (" & alg_des & ")"
    '                End If
    '                Dim mod_des_agg = jModObj("mod_des_agg").ToString
    '                If Not String.IsNullOrEmpty(mod_des_agg) Then
    '                    mod_des &= " [" & mod_des_agg & "]"
    '                End If

    '                For Each oi In dictModelli(km)
    '                    oi.Modello = mod_des
    '                    oi.Specie = jModObj("veg_des").ToString()
    '                    oi.Avversita = jModObj("avv_des").ToString()
    '                    oi.DescrParametri = jModObj("param_des").ToString()
    '                Next
    '            End If
    '        Next
    '    End If

    '    Return output
    'End Function


    'Private Shared Function PreparaIndicatoriXImpianti(output As OutputRisultatoIndicatori, listaModelli As List(Of ModelloConfigurato), piva As String, objParametri_Server As AgronicaCoreParametri) As OutputRisultatoIndicatori

    '    Dim dictModelli As New Dictionary(Of _key_modello, List(Of OutputRisultatoIndicatori.OutputIndicatore))
    '    Dim lista As List(Of OutputRisultatoIndicatori.OutputIndicatore)

    '    For Each oi In output.Indicatori

    '        Dim km As New _key_modello With {
    '            .mod_cod = oi.Parametri.Mod_Cod,
    '            .veg_cod = oi.Parametri.Veg_Cod,
    '            .avv_cod = oi.Parametri.Avv_Cod,
    '            .alg_cod = oi.Parametri.Alg_Cod,
    '            .params = oi.Parametri.ParametriElaborazione
    '        }

    '        If Not dictModelli.TryGetValue(km, lista) Then

    '            lista = New List(Of OutputRisultatoIndicatori.OutputIndicatore)
    '            dictModelli.Add(km, lista)
    '        End If

    '        lista.Add(oi)

    '        Dim tipo_sorgente = oi.Parametri.Tipo_Sorgente
    '        Dim stazione_cod = oi.Parametri.Stazione_Cod

    '        oi.Stazione = ""

    '        Dim idx As Integer = 0
    '        While idx < listaModelli.Count

    '            Dim modConf = listaModelli(idx)

    '            If tipo_sorgente = modConf.Tipo_Sorgente And
    '                stazione_cod = modConf.Stazione_Cod And
    '                km.mod_cod = modConf.Mod_Cod And
    '                km.veg_cod = modConf.Veg_Cod And
    '                km.avv_cod = modConf.Avv_Cod And
    '                km.alg_cod = modConf.Alg_Cod And
    '                km.params.Trim().CompareTo(modConf.ParametriElaborazione.Trim()) = 0 Then

    '                oi.Stazione = modConf.DescrGroup ' considero l'impianto come "stazione" per raggruppamento...

    '                listaModelli.RemoveAt(idx)

    '                idx = listaModelli.Count
    '            End If

    '            idx += 1
    '        End While
    '    Next

    '    Dim arr_modelli = JArray.Parse(JsonConvert.SerializeObject(dictModelli.Keys))

    '    Dim objParamsModelli As New JObject(
    '        New JProperty("Doorkey", GetDoorkey()),
    '        New JProperty("PIVA_Superuser", objParametri_Server.PivaSuperUser),
    '        New JProperty("PIVA", piva),
    '        New JProperty("Modelli", arr_modelli))

    '    Dim objMeteo As New AgronicaCoreWebService.MeteoNT

    '    Dim smodelli As String = objMeteo.CompletaOutputModelli(objParamsModelli, objParametri_Server)
    '    Dim jrisp = JObject.Parse(smodelli)
    '    smodelli = jrisp("RispostaStringa").ToString

    '    If Not String.IsNullOrEmpty(smodelli) Then

    '        arr_modelli = JArray.Parse(smodelli)

    '        For Each jModObj In arr_modelli

    '            Dim km As New _key_modello With {
    '                .mod_cod = CInt(jModObj("mod_cod")),
    '                .veg_cod = CInt(jModObj("veg_cod")),
    '                .avv_cod = CInt(jModObj("avv_cod")),
    '                .alg_cod = CInt(jModObj("alg_cod")),
    '                .params = jModObj("params").ToString
    '            }

    '            If dictModelli.ContainsKey(km) Then
    '                Dim mod_des = jModObj("mod_des").ToString
    '                Dim alg_des = jModObj("alg_des").ToString
    '                If Not String.IsNullOrEmpty(alg_des) Then
    '                    mod_des &= " (" & alg_des & ")"
    '                End If
    '                Dim mod_des_agg = jModObj("mod_des_agg").ToString
    '                If Not String.IsNullOrEmpty(mod_des_agg) Then
    '                    mod_des &= " [" & mod_des_agg & "]"
    '                End If

    '                For Each oi In dictModelli(km)
    '                    oi.Modello = mod_des
    '                    oi.Specie = jModObj("veg_des").ToString()
    '                    oi.Avversita = jModObj("avv_des").ToString()
    '                    oi.DescrParametri = jModObj("param_des").ToString()
    '                Next
    '            End If
    '        Next
    '    End If

    '    Return output
    'End Function

#End If