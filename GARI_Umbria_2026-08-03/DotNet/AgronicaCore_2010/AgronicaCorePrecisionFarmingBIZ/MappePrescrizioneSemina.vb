Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDTOStd.InData
Imports AgronicaCoreDTOStd.OutData
Imports Newtonsoft.Json.Linq

''' <summary>
''' Gestisce il flusso delle operazioni di semina via Engine Mappe Prescrizione.
''' </summary>
Public Class MappePrescrizioneSemina
    ''' <summary>
    ''' Entry point per il flusso di semina via Engine Mappe Prescrizione.
    ''' Legge tutti i dati necessari, costruisce il payload e lo accoda all'Engine.
    ''' </summary>
    Public Shared Function GestisciRichiestaMappePrescrizioneSemina(objPfRateoSrv As Gis.PfRateoSrv_In,
                                                                    codiceFiscaleTecnico As String,
                                                                    ByRef objParametriServer As AgronicaCoreParametri,
                                                                    ByRef objParametriUtenti As AgronicaCoreParametri,
                                                                    ByRef objParametriSuperServer As _
                                                                       AgronicaCoreParametri) As Boolean

        Dim flagTransazioneLocale = False
        Dim flagConnessioneLocale = False

        Try

            Dim piva = objPfRateoSrv.ChiaveAlbero.Piva
            Dim saCod = objPfRateoSrv.ChiaveAlbero.Sa_Cod
            Dim appezza = objPfRateoSrv.ChiaveAlbero.Appezza
            Dim idImp = objPfRateoSrv.ChiaveAlbero.Id_Imp
            Dim ricettaOperazioneCod = CInt(objPfRateoSrv.ChiaveAlbero.RicettaOperazione_Cod)

            ' --- lettura della ricetta ---
            Dim leggiDettagli As New AgronicaCoreContabDAL.Ricette_Dettagli_R
            Dim dtRicetta As DataTable =
                    leggiDettagli.Leggi(
                        0,
                        ricettaOperazioneCod,
                        0,
                        "",
                        0,
                        0,
                        0,
                        0,
                        CostantiPersonalizzate.AGRODATAINIZIO,
                        CostantiPersonalizzate.AGRODATAFINE,
                        enumSelezioneVariabile.Selezione_TabellaCompleta,
                        "",
                        "",
                        objParametriServer)

            If dtRicetta Is Nothing OrElse dtRicetta.Rows.Count = 0 Then
                Throw New Exception("Impossibile recuperare i dettagli della ricetta per Ricetta_Operazione_Cod=" &
                                    ricettaOperazioneCod)
            End If

            Dim dataRichiesta = CDate(dtRicetta.Rows(0)("Validita_Inizio"))

            ' --- lettura geometria impianto ---
            Dim gisEstrattore As New AgronicaCoreGisDAL.GIS_EstrattoreGrafica_R
            Dim dtGeometria = gisEstrattore.LetturaGeometriaImpianto(piva, saCod, appezza, idImp, objParametriServer)
            If dtGeometria Is Nothing OrElse dtGeometria.Rows.Count = 0 Then
                Throw New Exception("Non esiste la cartografia per l'impianto associato alla ricetta.")
            End If

            Dim wktImpianto = dtGeometria.Rows(0)("Poligono_GeoEntity_WKT").ToString()

            ' --- lettura coltura impianto ---
            Dim datiImpianto = AgronicaCoreAnagrafeBIZ.Impianto_R.Leggi_SpecieVarieta(piva, saCod, appezza, idImp,
                                                                                      objParametriServer)
            If datiImpianto Is Nothing Then
                Throw New Exception("Impossibile recuperare la coltura dell'impianto.")
            End If

            ' --- lettura MUZ dell'appezzamento ---
            Dim muzReader As New AgronicaCoreGisDAL.GIS_MUZ_R
            Dim dtMuz = muzReader.LeggiMUZDaAppezzamento(piva, saCod, appezza, objParametriServer)

            If dtMuz Is Nothing OrElse dtMuz.Rows.Count = 0 Then
                ' MUZ non disponibili nel DB locale: fallback sull'engine SAT 
                Dim entitaCod = CInt(dtGeometria.Rows(0)("Entita_Cod"))
                
                Dim muzGenerate = GenerazioneMuzDaEngineSat.GeneraMuz(
                    wktImpianto, entitaCod, dataRichiesta, objParametriServer)
                
                dtMuz = GenerazioneMuzDaEngineSat.PersistiMuzDaEngineSat(
                    muzGenerate, piva, saCod, appezza, datiImpianto.ValiditaInizio, datiImpianto.ValiditaFine,
                    objParametriServer, objParametriUtenti, objParametriSuperServer)
            End If

            Dim areaCods = (From row As DataRow In dtMuz.Rows Select CInt(row("Area_Cod"))).ToList()

            ' --- lettura impianti ultimi 3 anni ---
            Dim impiantiAnni = EstrazioneImpiantiUltimi3Anni(piva, saCod, appezza, idImp, objParametriServer)

            ' --- lettura specie appezzamento da codici anagrafe ---
            Dim specieAppezzamento = EstrazioneSpecieAppezzamento(piva, saCod, appezza, objParametriServer)

            ' --- lettura analisi MUZ ---
            Dim analisiMuz = EstraiAnalisiMuz(areaCods, objParametriServer)

            ' --- costruzione payload ---
            Dim payload = CostruisciPayloadSemina(wktImpianto,
                                                  dataRichiesta,
                                                  datiImpianto,
                                                  dtMuz,
                                                  impiantiAnni,
                                                  specieAppezzamento,
                                                  analisiMuz)

            ' --- invio all'Engine e salvataggio requestId ---
            Dim fmisContext = MappePrescrizioneFertilizzazione.CreaFmisContext(objParametriServer,
                                                                               objParametriUtenti,
                                                                               objParametriSuperServer)
            Dim requestId = AccodaRichiestaEngineSemina(payload, fmisContext, objParametriServer, objParametriSuperServer)

            ConnessioniTransazioni.ApriConnessioneXCoreBiz(flagConnessioneLocale, flagTransazioneLocale,
                                                           objParametriServer)

            Dim scriviRichiesta As New AgronicaCoreGisDAL.RichiesteEngine_Impianto_Ricetta_W
            scriviRichiesta.Scrivi(
                requestId,
                piva,
                saCod,
                appezza,
                idImp,
                ricettaOperazioneCod,
                objParametriServer)

            If objParametriServer.objTransazione IsNot Nothing Then
                ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(flagTransazioneLocale, objParametriServer)
            End If

            Return True

        Catch ex As Exception
            If Not objParametriServer.objTransazione Is Nothing Then
                ConnessioniTransazioni.ChiudiTransazione(2, objParametriServer)
            End If
            Throw
        Finally
            ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(flagConnessioneLocale, objParametriServer)
        End Try
    End Function

    ''' <summary>
    ''' Costruisce il payload JSON per la richiesta di semina all'Engine Mappe Prescrizione.
    ''' </summary>
    ''' <param name="wktImpianto">WKT della geometria dell'impianto.</param>
    ''' <param name="dataRichiesta">Data dell'operazione della ricetta.</param>
    ''' <param name="datiImpianto">Dati coltura/varietà dell'impianto.</param>
    ''' <param name="dtMuz">DataTable MUZ dell'appezzamento (Area_Cod, Poligono_GeoEntity_WKT).</param>
    ''' <param name="impiantiAnni">Impianti degli ultimi 3 anni.</param>
    ''' <param name="specieAppezzamento">Codici specie precedenti da anagrafe appezzamento.</param>
    ''' <param name="analisiMuz">Analisi pedologiche per ogni MUZ.</param>
    Private Shared Function CostruisciPayloadSemina(wktImpianto As String,
                                                    dataRichiesta As Date,
                                                    datiImpianto As Gis.Impianti.DatiImpianto_Out,
                                                    dtMuz As DataTable,
                                                    impiantiAnni As Gis.Impianti.ImpiantiUltimi3Anni_Out,
                                                    specieAppezzamento As Gis.Specie.SpecieAppezzamento_Out,
                                                    analisiMuz As List(Of Gis.MUZ.AnalisiMuz_Out)) As JObject

        Dim currentYear = Date.Today.Year

        Dim payload As New JObject()
        payload("job_type") = "semina"
        payload("data_richiesta") = dataRichiesta.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")

        Dim geometryImpianto As New JObject()
        geometryImpianto("wkt") = wktImpianto
        geometryImpianto("srid") = 4326
        payload("geometry") = geometryImpianto

        Dim cultura As New JObject()
        cultura("id") = datiImpianto.VegCod.ToString()
        cultura("varieta") = datiImpianto.CulCod.ToString()
        payload("cultura") = cultura

        Dim caratteristicheSeme As New JObject()
        caratteristicheSeme("peso_semi_gr_per_1000") = Nothing
        caratteristicheSeme("densita_riferimento_semi_m2") = Nothing
        caratteristicheSeme("germinabilita") = 1
        payload("caratteristiche_seme") = caratteristicheSeme

        ' --- dati_storici: una voce per ogni MUZ ---
        Dim datiStorici As New JArray()
        For Each muzRow As DataRow In dtMuz.Rows
            Dim areaCod = CInt(muzRow("Area_Cod"))
            Dim wktMuz = muzRow("Poligono_GeoEntity_WKT").ToString()

            Dim muzEntry As New JObject()
            muzEntry("muz_id") = areaCod.ToString()

            Dim geometryMuz As New JObject()
            geometryMuz("wkt") = wktMuz
            geometryMuz("srid") = 4326
            muzEntry("geometry") = geometryMuz

            muzEntry("MSAVI_1") = CreaArrayMsavi(impiantiAnni.year_1,
                                                 specieAppezzamento.VegCod1,
                                                 currentYear - 1)
            muzEntry("MSAVI_2") = CreaArrayMsavi(impiantiAnni.year_2,
                                                 specieAppezzamento.VegCod2,
                                                 currentYear - 2)
            muzEntry("MSAVI_3") = CreaArrayMsavi(impiantiAnni.year_3,
                                                 specieAppezzamento.VegCod3,
                                                 currentYear - 3)

            muzEntry("dose_q_ha_override") = 120

            datiStorici.Add(muzEntry)
        Next
        payload("dati_storici") = datiStorici

        Dim opzioniFlusso As New JObject()
        opzioniFlusso("modalita_automatica") = False
        payload("opzioni_flusso") = opzioniFlusso

        ' --- dati_pedologici ---
        Dim datiPedologici As New JArray()
        For Each muz In analisiMuz
            Dim pedRow As New JObject()
            pedRow("muz_id") = muz.muz_id.ToString()
            pedRow("sabbia_perc") = muz.sabbia_perc
            pedRow("limo_perc") = muz.limo_perc
            pedRow("argilla_perc") = muz.argilla_perc
            pedRow("tessitura") = ""
            pedRow("pH") = muz.pH
            pedRow("EC") = Nothing
            pedRow("SO") = muz.SO
            pedRow("rapporto_C_N") = muz.rapporto_C_N
            datiPedologici.Add(pedRow)
        Next
        payload("dati_pedologici") = datiPedologici

        Return payload
    End Function

    ''' <summary>
    ''' Costruisce l'array MSAVI_x per un anno dato, con 0 o 1 elemento.
    ''' Se l'impianto dell'anno è Nothing vengono usati i dati da codice anagrafe e date calcolate.
    ''' </summary>
    Private Shared Function CreaArrayMsavi(impiantoAnno As Gis.Impianti.ImpiantoAnno_Out,
                                           vegCodFallback As Integer?,
                                           targetYear As Integer) As JArray

        Dim arr As New JArray()

        ' Zero elements when neither impianto nor fallback culture code is available
        If impiantoAnno Is Nothing AndAlso (vegCodFallback Is Nothing OrElse vegCodFallback = 0) Then
            Return arr
        End If

        Dim entry As New JObject()

        If impiantoAnno IsNot Nothing Then
            entry("coltura") = impiantoAnno.veg_cod.ToString()
            entry("periodo_da") = impiantoAnno.Validita_Inizio.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")
            entry("periodo_a") = impiantoAnno.Validita_Fine.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")
        Else
            entry("coltura") = vegCodFallback.Value.ToString()
            entry("periodo_da") = New Date(targetYear, 1, 1).ToString("yyyy-MM-ddTHH:mm:ss.fffZ")
            entry("periodo_a") = New Date(targetYear, 12, 31).ToString("yyyy-MM-ddTHH:mm:ss.fffZ")
        End If

        entry("classificazione") = "prevalente"
        entry("msavi") = New JArray()
        entry("time") = New JArray()

        arr.Add(entry)
        Return arr
    End Function

    ''' <summary>
    ''' Accoda la richiesta di semina all'Engine Mappe Prescrizione usando il model code specifico
    ''' per la semina. Restituisce il requestId della risposta.
    ''' </summary>
    Private Shared Function AccodaRichiestaEngineSemina(payload As JObject,
                                                        fmisContext As String,
                                                        ByRef objParametriServer As AgronicaCoreParametri,
                                                        ByRef objParametriSuperServer As AgronicaCoreParametri) As String

        Dim confReader As New AgronicaCoreVarieDAL.Configurazione_Siti_R

        Dim urlEngine = confReader.Leggi_Valore_ServerESuperServer(0, CostantiPersonalizzate.MappePrescrizione_BaseUrl_ConfKey,
                                                "", "", objParametriServer, objParametriSuperServer)
        If String.IsNullOrEmpty(urlEngine) Then
            Throw New Exception("MissingConfigurationException: URL Engine Mappe Prescrizione non configurato.")
        End If

        Dim tenant = confReader.Leggi_Valore(0, CostantiPersonalizzate.MappePrescrizione_TenantName_ConfKey,
                                             "", "", objParametriServer)
        If String.IsNullOrEmpty(tenant) Then
            Throw New Exception("MissingConfigurationException: Tenant Engine Mappe Prescrizione non configurato.")
        End If

        Dim apiKey = confReader.Leggi_Valore(0, CostantiPersonalizzate.MappePrescrizione_ApiKey_ConfKey,
                                             "", "", objParametriServer)
        If String.IsNullOrEmpty(apiKey) Then
            Throw New Exception("MissingConfigurationException: API Key Engine Mappe Prescrizione non configurata.")
        End If

        Dim modelCode = confReader.Leggi_Valore(0, CostantiPersonalizzate.MappePrescrizione_ModelCode_Semina_ConfKey,
                                                "", "", objParametriServer)
        If String.IsNullOrEmpty(modelCode) Then
            Throw New Exception(
                "MissingConfigurationException: Model code semina Engine Mappe Prescrizione non configurato.")
        End If

        Dim endpoint = String.Format("https://{0}/mappe-prescrizione/{1}/public/v1/models/{2}/executionrequest",
                                     urlEngine, tenant, modelCode)

        Return MappePrescrizioneFertilizzazione.InviaRichiestaConRetry(endpoint, apiKey, fmisContext, payload)
    End Function

    ''' <summary>
    ''' Estrae le analisi MUZ per le aree omogenee specificate, recuperando i parametri
    ''' Sabbia, Limo, Argilla, pH, Sostanza Organica e Rapporto C/N.
    ''' </summary>
    ''' <param name="areaCods">Codici delle aree omogenee (MUZ) da interrogare.</param>
    ''' <param name="objParametri">Parametri di connessione al database.</param>
    Private Shared Function EstraiAnalisiMuz(areaCods As List(Of Integer),
                                            ByRef objParametri As AgronicaCoreParametri) As List(Of Gis.MUZ.AnalisiMuz_Out)
        Const routing = "AgronicaCorePrecisionFarmingBIZ.MappePrescrizioneSemina.EstraiAnalisiMuz()"

        Dim paramsCods As New List(Of Integer) From {
                CInt(TipiEnumerativi.enum_AnalisiParametri.AnalisiParametri_Sabbia),
                CInt(TipiEnumerativi.enum_AnalisiParametri.AnalisiParametri_Limo),
                CInt(TipiEnumerativi.enum_AnalisiParametri.AnalisiParametri_Argilla),
                CInt(TipiEnumerativi.enum_AnalisiParametri.AnalisiParametri_pH),
                CInt(TipiEnumerativi.enum_AnalisiParametri.AnalisiParametri_SostanzaOrganica),
                CInt(TipiEnumerativi.enum_AnalisiParametri.AnalisiParametri_RapportoCN)
                }

        Try
            Dim muzReader As New AgronicaCoreGisDAL.GIS_MUZ_R
            Dim dt = muzReader.LeggiAnalisiMuz(areaCods, paramsCods, objParametri)
            Dim result As New Dictionary(Of Integer, Gis.MUZ.AnalisiMuz_Out)

            For Each row As DataRow In dt.Rows
                Dim areaCod = CInt(row("Area_Cod"))
                If Not result.ContainsKey(areaCod) Then
                    result(areaCod) = New AgronicaCoreDTOStd.OutData.Gis.MUZ.AnalisiMuz_Out With {.muz_id = areaCod}
                End If

                Dim item = result(areaCod)
                Dim paramCod = CInt(row("Analisi_Parametro_Cod"))
                Dim valore = CDbl(row("Analisi_Dettaglio_Valore_1"))

                Select Case CType(paramCod, TipiEnumerativi.enum_AnalisiParametri)
                    Case TipiEnumerativi.enum_AnalisiParametri.AnalisiParametri_Sabbia : item.sabbia_perc = valore
                    Case TipiEnumerativi.enum_AnalisiParametri.AnalisiParametri_Limo : item.limo_perc = valore
                    Case TipiEnumerativi.enum_AnalisiParametri.AnalisiParametri_Argilla : item.argilla_perc = valore
                    Case TipiEnumerativi.enum_AnalisiParametri.AnalisiParametri_pH : item.pH = valore
                    Case TipiEnumerativi.enum_AnalisiParametri.AnalisiParametri_SostanzaOrganica : item.SO = valore
                    Case TipiEnumerativi.enum_AnalisiParametri.AnalisiParametri_RapportoCN : item.rapporto_C_N = valore
                End Select
            Next

            Return result.Values.ToList()
        Catch ex As Exception
            Throw New Exception("[" & routing & "] : " & ex.Message)
        End Try
    End Function

    ''' <summary>
    ''' Estrae gli impianti degli ultimi 3 anni per l'appezzamento specificato,
    ''' assegnando ciascun anno agronomico al rispettivo campo year_1/year_2/year_3.
    ''' Se per un dato anno sono presenti più impianti, viene selezionato quello con
    ''' Validita_Fine maggiore. Se non sono presenti impianti per un anno, quel campo
    ''' viene impostato a Nothing.
    ''' </summary>
    ''' <param name="piva">Partita IVA dell'azienda.</param>
    ''' <param name="saCod">Codice SA.</param>
    ''' <param name="appezza">Codice appezzamento.</param>
    ''' <param name="idImp">ID impianto corrente da escludere dalla lettura.</param>
    ''' <param name="objParametri">Parametri di connessione al database.</param>
    Private Shared Function EstrazioneImpiantiUltimi3Anni(piva As String,
                                                         saCod As Integer,
                                                         appezza As Integer,
                                                         idImp As Integer,
                                                         ByRef objParametri As AgronicaCoreParametri) _
        As Gis.Impianti.ImpiantiUltimi3Anni_Out
        Const routing = "AgronicaCorePrecisionFarmingBIZ.MappePrescrizioneSemina.EstrazioneImpiantiUltimi3Anni()"

        Try
            Dim dal As New AgronicaCoreGisDAL.GIS_Entita_R
            Dim dt = dal.LeggiImpiantiAppezzamentoUltimiAnni(piva, saCod, appezza, idImp, 3, objParametri)

            Dim currentYear = Date.Today.Year
            Dim result As New Gis.Impianti.ImpiantiUltimi3Anni_Out

            For offset = 1 To 3
                Dim targetYear = currentYear - offset
                Dim yearStart As New Date(targetYear, 1, 1)
                Dim yearEnd As New Date(targetYear, 12, 31)

                Dim best As DataRow = Nothing
                For Each row As DataRow In dt.Rows
                    Dim validitaInizio = CDate(row("Validita_Inizio"))
                    Dim validitaFine = CDate(row("Validita_Fine"))
                    If validitaInizio >= yearStart AndAlso validitaFine <= yearEnd Then
                        If best Is Nothing OrElse validitaFine > CDate(best("Validita_Fine")) Then
                            best = row
                        End If
                    End If
                Next

                Dim impiantoAnno As Gis.Impianti.ImpiantoAnno_Out = Nothing
                If best IsNot Nothing Then
                    impiantoAnno = New Gis.Impianti.ImpiantoAnno_Out With {
                        .piva = CStr(best("PIVA")),
                        .sa_cod = CInt(best("SA_COD")),
                        .appezza = CInt(best("APPEZZA")),
                        .id_imp = CInt(best("ID_REG")),
                        .Validita_Inizio = CDate(best("Validita_Inizio")),
                        .Validita_Fine = CDate(best("Validita_Fine")),
                        .cul_cod = CInt(best("CUL_COD")),
                        .veg_cod = CInt(best("Veg_Cod"))
                        }
                End If

                Select Case offset
                    Case 1 : result.year_1 = impiantoAnno
                    Case 2 : result.year_2 = impiantoAnno
                    Case 3 : result.year_3 = impiantoAnno
                End Select
            Next

            Return result
        Catch ex As Exception
            Throw New Exception("[" & routing & "] : " & ex.Message)
        End Try
    End Function

    ''' <summary>
    ''' Estrae gli identificativi delle specie colturali coltivate nei 3 anni precedenti
    ''' all'anno corrente sull'appezzamento dato, leggendo i codici anagrafe
    ''' Coltura_Appezzamento_Precedente_1/2/3.
    ''' </summary>
    ''' <param name="piva">Partita IVA dell'impresa.</param>
    ''' <param name="saCod">Codice del centro aziendale.</param>
    ''' <param name="appezza">Codice dell'appezzamento.</param>
    ''' <param name="objParametri">Parametri di connessione al database.</param>
    Private Shared Function EstrazioneSpecieAppezzamento(piva As String,
                                                        saCod As Integer,
                                                        appezza As Integer,
                                                        ByRef objParametri As AgronicaCoreParametri) _
        As Gis.Specie.SpecieAppezzamento_Out
        Const routing = "AgronicaCorePrecisionFarmingBIZ.MappePrescrizioneSemina.EstrazioneSpecieAppezzamento()"

        Try
            Dim idCods As New List(Of TipiEnumerativi.enum_CodiciAnagrafe) From {
                    TipiEnumerativi.enum_CodiciAnagrafe.Coltura_Appezzamento_Precedente_1,
                    TipiEnumerativi.enum_CodiciAnagrafe.Coltura_Appezzamento_Precedente_2,
                    TipiEnumerativi.enum_CodiciAnagrafe.Coltura_Appezzamento_Precedente_3
                    }

            Dim biz As New AgronicaCoreAnagrafeBIZ.Appezzamento_R
            Dim dt = biz.LeggiCodiciPerIdCods(piva, saCod, appezza, idCods, objParametri)

            Dim result As New Gis.Specie.SpecieAppezzamento_Out With {
                    .Piva = piva,
                    .SaCod = saCod,
                    .Appezza = appezza
                    }

            For Each row As DataRow In dt.Rows
                Dim idCod = CType(CInt(row("id_cod")), TipiEnumerativi.enum_CodiciAnagrafe)
                Dim valCod As Integer = Nothing
                If Not IsDBNull(row("val_cod")) AndAlso IsNumeric(row("val_cod")) Then
                    valCod = CInt(row("val_cod"))
                End If

                Select Case idCod
                    Case TipiEnumerativi.enum_CodiciAnagrafe.Coltura_Appezzamento_Precedente_1
                        result.VegCod1 = valCod
                    Case TipiEnumerativi.enum_CodiciAnagrafe.Coltura_Appezzamento_Precedente_2
                        result.VegCod2 = valCod
                    Case TipiEnumerativi.enum_CodiciAnagrafe.Coltura_Appezzamento_Precedente_3
                        result.VegCod3 = valCod
                End Select
            Next

            Return result
        Catch ex As Exception
            Throw New Exception("[" & routing & "] : " & ex.Message)
        End Try
    End Function
End Class
