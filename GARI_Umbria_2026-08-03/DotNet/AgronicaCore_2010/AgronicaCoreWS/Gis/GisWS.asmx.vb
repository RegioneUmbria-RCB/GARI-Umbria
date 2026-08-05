Imports Newtonsoft.Json

Imports System.IO
Imports System.Net.Http
Imports System.Web.Services
Imports Agronica.Helpers.GDALHelper.Agronica.Helpers.GDALHelper
Imports AgronicaAlgoritmiProiezione
Imports AgronicaControlliGIS
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDTOStd.InData.Gis
Imports AgronicaCoreDTOStd.InData.Gis.MUZ
Imports AgronicaCoreGisDAL
Imports AgronicaCoreModello
Imports AgronicaCoreModelsSTD.anagrafiche
Imports AgronicaCoreModelsSTD.exceptions
Imports AgronicaCoreModelsSTD.Gis
Imports AgronicaCoreModelsSTD.Gis.MUZ
Imports AgronicaCoreModelsSTD.Gis.PermessiLayer
Imports AgronicaCoreUtentiDAL
Imports AgronicaCoreVarieBIZ
Imports DocumentFormat.OpenXml.Drawing.Diagrams
Imports InData.DatiPrevisionaliColture
Imports InData.Gis
Imports Newtonsoft.Json.Linq

' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
' <System.Web.Script.Services.ScriptService()> _
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<System.Web.Script.Services.ScriptService()>
Public Class GisWS
    Inherits System.Web.Services.WebService

    'qui saranno riportati i ws che ora sono nella pagina gis agenda.

    ''' <summary>
    ''' Ottiene un poligono "aumentato" chiamando la funzione "STBuffer" di SQL Server
    ''' </summary>
    ''' <param name="objP_super_server"></param>
    ''' <param name="objP_server"></param>
    ''' <param name="objP_utenti"></param>
    ''' <param name="InData"></param>
    ''' <returns></returns>
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function STBufferGeoJsonPolygon(ByVal objP_super_server As String,
                                           ByVal objP_server As String,
                                           ByVal objP_utenti As String,
                                           InData As STBufferGeoJsonPolygonInData) As rispostaStandard(Of STBufferGeoJsonPolygonOutData)

        Dim r As New rispostaStandard(Of STBufferGeoJsonPolygonOutData)

        Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
        Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)

        Try

            Dim leggiLingua As New Lingue_Read
            Dim dtLingua As DataTable = leggiLingua.Leggi_datoCODGIAS(objParametri_Server.Lingua_Cod,
                                                                      AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                      "", "", objParametri_Utenti)

            Dim linguaCodiceISO As String = dtLingua.Rows(0)("CodiceISO")
            Threading.Thread.CurrentThread.CurrentUICulture = New Globalization.CultureInfo(linguaCodiceISO)

            r = AgronicaControlliGIS.V_M.STBufferGeoJsonPolygon(objParametri_Server, InData)

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function ModificaImpianto2019(ByVal objP_super_server As String,
                                         ByVal objP_server As String,
                                         ByVal objP_utenti As String,
                                         InData As CoreWSGisEndPoints_ModificaImpianto2019InData) As rispostaStandard(Of CoreWSGisEndPoints_ModificaImpianto2019OutData)
        Dim r As New rispostaStandard(Of CoreWSGisEndPoints_ModificaImpianto2019OutData)

        Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
        Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)

        Try

            Dim leggiLingua As New Lingue_Read
            Dim dtLingua As DataTable = leggiLingua.Leggi_datoCODGIAS(objParametri_Server.Lingua_Cod,
                                                                      AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                      "", "", objParametri_Utenti)

            Dim linguaCodiceISO As String = dtLingua.Rows(0)("CodiceISO")
            Threading.Thread.CurrentThread.CurrentUICulture = New Globalization.CultureInfo(linguaCodiceISO)

            Dim obj As New CoreWSGisEndPoints_ModificaImpianto2019OutData

            Dim Codice_Fiscale_Tecnico As String = GetCodiceFiscaleTecnico(objParametri_Server, objParametri_Utenti)

            LeggiDatiAccessoriTmp(objParametri_Server, objParametri_Utenti, InData)

            Dim msg As RispostaStandard =
                AgronicaControlliGIS.V_M.ModificaImpianto2019(
                    InData.entita_cod,
                    InData.nome_appezza,
                    InData.sup_imp,
                    InData.data_inizio,
                    InData.data_fine,
                    InData.specie,
                    InData.varieta,
                    InData.finalita,
                    InData.tipologia,
                    InData.hiddenPunti_modifica,
                    InData.via_stringa,
                    Codice_Fiscale_Tecnico,
                    InData.lotto,
                    InData.codice_socio,
                    "",
                    "",
                    "",
                    InData.CodiciAnagrafeAggiuntivi,
                    InData.ModificaAnagrafica,
                    objParametri_Server,
                    objParametri_Utenti
                )

            obj.messaggio = msg.RispostaStringa
            r.RispostaOK = msg.RispostaOK
            r.RispostaStringa = obj

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r
    End Function

    Private Shared Sub LeggiDatiAccessoriTmp(objParametri_Server As AgronicaCoreParametri,
                                             objParametri_Utenti As AgronicaCoreParametri,
                                             InData As CoreWSGisEndPoints_ModificaImpianto2019InData)

        Dim piva As String = ""
        Dim sa_cod As Integer
        Dim appezza As Integer
        Dim id_reg As Integer
        Dim objImpianto As AgronicaCoreAnagrafeBIZ.Appezzamento_Piccolo = Nothing

        Dim tipoentita_cod As Integer = 0
        Dim id_agenda As Integer = 0
        Dim id_Mov_Det As Integer = 0
        Dim analisi_campione_cod As Integer = 0
        Dim LayerElementiGrafici_Cod As Integer = 0


        Dim objLeggiAppezza As New AgronicaCoreAnagrafeDAL.Appezzamento_Read

        AgronicaControlliGIS.V_M.GetEntitaGiasFromEntitaGrafica(
            InData.entita_cod,
            piva,
            sa_cod,
            appezza,
            id_reg,
            0,
            0,
            objParametri_Server,
            objParametri_Utenti,
            0,
            tipoentita_cod,
            id_agenda,
            id_Mov_Det,
            analisi_campione_cod,
            objImpianto,
            LayerElementiGrafici_Cod,
            ""
            )


        Dim dtAppezza As DataTable =
            objLeggiAppezza.Leggi(piva, sa_cod, appezza, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)

        If dtAppezza.Rows.Count > 0 Then
            InData.nome_appezza = dtAppezza(0)("App_Nome").ToString()
            InData.via_stringa = dtAppezza(0)("Via_Stringa").ToString()
        End If


    End Sub

    Private Shared Function GetCodiceFiscaleTecnico(objParametri_Server As AgronicaCoreParametri,
                                                    objParametri_Utenti As AgronicaCoreParametri) As String

        Dim LeggiCFGDatiIniziali As New AgronicaCoreVarieDAL.Configurazione_Siti_R
        Dim GIS_EscludiFiltroCodiceFiscaleTecnico As String =
            LeggiCFGDatiIniziali.Leggi_Valore(0, "GIS_EscludiFiltroCodiceFiscaleTecnico", "", "", objParametri_Server)

        Dim condizionePIVA As String = ""
        Dim Codice_Fiscale_Tecnico As String = "CF TEC"
        Dim Ragione_Sociale_Codice_Fiscale_Tecnico As String = "CF TEC"

        If GIS_EscludiFiltroCodiceFiscaleTecnico = "" Or GIS_EscludiFiltroCodiceFiscaleTecnico = "false" Then

            Dim xUtenteCorrente As String = CType(objParametri_Server, AgronicaCoreDataProvider.AgronicaCoreParametri).UtenteUsername
            Dim objUtentiDAL As New AgronicaCoreUtentiDAL.Utenti_xGruppi_Utente_R

            Codice_Fiscale_Tecnico = objUtentiDAL.Leggi_IdentificativoGruppoUtenti_Singolo(xUtenteCorrente, objParametri_Utenti)
        End If

        Return Codice_Fiscale_Tecnico

    End Function


    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function LeggiChiaveAlbero(ByVal InData As Object) As rispostaStandard(Of String)

        Dim r As New rispostaStandard(Of String)

        Dim data = DeserializzaInData(Of Int32)(InData)
        Dim leggiGisEntita As New AgronicaCoreGisDAL.GIS_Entita_R


        Try
            Dim dtEntita = leggiGisEntita.LeggiEntitaCod_x_ChiaveAlbero(data.InData, data.Server)

            Dim key = ""
            If dtEntita.Rows.Count = 1 Then
                Dim row = dtEntita.Rows(0)

                Call Albero.ChiaveAlbero_Codifica_x_json_faster(
                    key,
                    IIf(IsDBNull(row("TipoNodoAlberoAnagrafe")), 0, row("TipoNodoAlberoAnagrafe")),
                    IIf(IsDBNull(row("Piva")), "0", row("Piva")),
                    IIf(IsDBNull(row("Sa_Cod")), 0, row("Sa_Cod")),
                    IIf(IsDBNull(row("Campo_Cod")), 0, row("Campo_Cod")),
                    IIf(IsDBNull(row("Appezza")), 0, row("Appezza")),
                    IIf(IsDBNull(row("Id_Imp")), 0, row("Id_Imp")),
                    0,
                    IIf(IsDBNull(row("PROV")), "0", row("PROV")),
                    IIf(IsDBNull(row("COM")), "0", row("COM")),
                    IIf(IsDBNull(row("SEZIONE")) Or CInt(row("SEZIONE")) = -1, "0", row("SEZIONE")),
                    IIf(IsDBNull(row("FOGLIO")) Or CInt(row("FOGLIO")) = -1, "0", row("FOGLIO")),
                    IIf(IsDBNull(row("NUMERO")) Or CInt(row("NUMERO")) = -1, "0", row("NUMERO")),
                    IIf(IsDBNull(row("SUBALTERNO")) Or CInt(row("SUBALTERNO")) = -1, "0", row("SUBALTERNO")),
                    "0",
                    IIf(IsDBNull(row("Fabbricato_Cod")), 0, row("Fabbricato_Cod")),
                    0,
                    "0",
                    "0",
                    "0",
                    "0",
                    IIf(IsDBNull(row("analisi_Campione_Cod")), 0, row("analisi_Campione_Cod")),
                    0,
                    0,
                    IIf(IsDBNull(row("programmazione_cod")), 0, row("programmazione_cod")),
                    IIf(IsDBNull(row("Programmazione_Entita_Cod")), 0, row("Programmazione_Entita_Cod")),
                    IIf(IsDBNull(row("ID_Agenda")), 0, row("ID_Agenda")),
                    "",
                    0,
                    IIf(IsDBNull(row("Ricetta_Operazione_cod")), 0, row("Ricetta_Operazione_cod"))
                )

            End If

            r.RispostaOK = True
            r.RispostaStringa = key

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function SalvaNuovoElementoGraficoDaChiaveAlbero(ByVal objP_super_server As String,
                                                            ByVal objP_server As String,
                                                            ByVal objP_utenti As String,
                                                            InData As CoreWSGisEndPoints_SalvaNuovoElementoGraficoDaChiaveAlberoInData) As rispostaStandard(Of CoreWSGisEndPoints_SalvaNuovoElementoGraficoDaChiaveAlberoOutData)
        Dim r As New rispostaStandard(Of CoreWSGisEndPoints_SalvaNuovoElementoGraficoDaChiaveAlberoOutData)

        Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
        Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)

        Try

            Dim leggiLingua As New Lingue_Read
            Dim dtLingua As DataTable = leggiLingua.Leggi_datoCODGIAS(objParametri_Server.Lingua_Cod,
                                                                      AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                      "", "", objParametri_Utenti)
            Dim linguaCodiceISO As String = dtLingua.Rows(0)("CodiceISO")
            Threading.Thread.CurrentThread.CurrentUICulture = New Globalization.CultureInfo(linguaCodiceISO)

            Dim msg As String =
                AgronicaControlliGIS.V_M.SalvaNuovoElementoGraficoDaChiaveAlbero(InData.ChiaveAlbero, InData.hiddenPunti_Nuovo, InData.Area, InData.ElementoGrafico_Des, objParametri_Server)

            r.RispostaOK = True
            r.RispostaStringa = New CoreWSGisEndPoints_SalvaNuovoElementoGraficoDaChiaveAlberoOutData
            r.RispostaStringa.Messaggio = msg

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function TestData(ByVal objP_super_server As String,
                             ByVal objP_server As String,
                             ByVal objP_utenti As String,
                             InData As GisDataReadParam) As rispostaStandard(Of GisDataReadRval(Of AgronicaCoreModelsSTD.Gis.GeoJSONAgroGisProp))
        Dim r As New rispostaStandard(Of GisDataReadRval(Of AgronicaCoreModelsSTD.Gis.GeoJSONAgroGisProp))

        Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
        Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)

        Try

            Dim leggiLingua As New Lingue_Read
            Dim dtLingua As DataTable = leggiLingua.Leggi_datoCODGIAS(objParametri_Server.Lingua_Cod,
                                                                      AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                      "", "", objParametri_Utenti)

            Dim linguaCodiceISO As String = dtLingua.Rows(0)("CodiceISO")
            Threading.Thread.CurrentThread.CurrentUICulture = New Globalization.CultureInfo(linguaCodiceISO)

            Dim obj As New GisDataReadRval(Of AgronicaCoreModelsSTD.Gis.GeoJSONAgroGisProp)
            Dim fileContents As String

            'piccolo test google
            'fileContents = My.Computer.FileSystem.ReadAllText(Server.MapPath("." & "\SmallTest.json"))
            Dim cfgAlbero As New ConfigurazioneAlbero
            fileContents =
                AgronicaControlliGIS.V_M.AggiornaLayer_3(objParametri_Server, objParametri_Utenti, InData, cfgAlbero)
            obj.myGeoJson = JsonConvert.DeserializeObject(Of GeoJson(Of AgronicaCoreModelsSTD.Gis.GeoJSONAgroGisProp))(fileContents)

            r.RispostaOK = True
            r.RispostaStringa = obj

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r
    End Function

    Public Class GisDataReadRval(Of T)
        Public Property myGeoJson As GeoJson(Of T)
    End Class

    Public Class GisDataReadRval_New(Of T)
        Public Property myGeoJson As GeoJson_New(Of T)
    End Class

#Region "Nuovi Web Service Configurazione Albero GIS 2022"

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function ConfiguraAlbero(InData As Object) As rispostaStandard(Of CfgAlbero_CfgGisUtente)

        Dim r As New rispostaStandard(Of CfgAlbero_CfgGisUtente)

        Dim objAlberoConfiguraGis As New CfgAlbero_CfgGisUtente

        Try

            Dim objParametri = DeserializzaInData(Of ImpostaConfigurazioneAlbero)(InData)
            Dim objImpostaCfgAlbero = objParametri.InData

            Gias_InizializzaCultura_DaParams(objParametri.Server, objParametri.Utenti)

            '================================================================================
            'Lettura Parametri Da Database
            '================================================================================

            If objImpostaCfgAlbero.LeggiConfigurazioneDaDatabase Then

                LeggiCfgGisDaDatabase(objParametri.Server, objParametri.Utenti, objImpostaCfgAlbero.CfgGisUtente)

            End If

            '================================================================================
            'Impostazione Parametri Configurazione Albero
            '================================================================================

            '--------------------------------------------------------------------------------
            'Default
            '--------------------------------------------------------------------------------

            objAlberoConfiguraGis.CfgAlbero.Flag_Agenda = False

            objAlberoConfiguraGis.CfgAlbero.Flag_Esplodi_Tutto = True

            objAlberoConfiguraGis.CfgAlbero.ParametriAgendaData = AGRODATAINIZIO

            objAlberoConfiguraGis.CfgAlbero.dataInizio = objParametri.Server.FinestraTemporaleInizio

            objAlberoConfiguraGis.CfgAlbero.dataFine = objParametri.Server.FinestraTemporaleFine

            objAlberoConfiguraGis.CfgAlbero.Contesto = ConfigurazioneAlbero.enum_Contesto.GIS

            '--------------------------------------------------------------------------------
            'Da Configurazione Siti
            '--------------------------------------------------------------------------------

            Dim LeggiCFGDatiIniziali As New AgronicaCoreVarieDAL.Configurazione_Siti_R

            Dim AlberoAnagrafica2017_LetturaViaSQLJson As String = LeggiCFGDatiIniziali.Leggi_Valore(0,
                                                                                                     "AlberoAnagrafica2017_LetturaViaSQLJson",
                                                                                                     "",
                                                                                                     "",
                                                                                                     objParametri.Server)

            If String.IsNullOrEmpty(AlberoAnagrafica2017_LetturaViaSQLJson) Then
                AlberoAnagrafica2017_LetturaViaSQLJson = "false"
            End If

            objAlberoConfiguraGis.CfgAlbero.LetturaViaSQLJson = AlberoAnagrafica2017_LetturaViaSQLJson

            Dim GIS_EscludiFiltroCodiceFiscaleTecnico As String = LeggiCFGDatiIniziali.Leggi_Valore(0,
                                                                                                    "GIS_EscludiFiltroCodiceFiscaleTecnico",
                                                                                                    "",
                                                                                                    "",
                                                                                                    objParametri.Server)

            If GIS_EscludiFiltroCodiceFiscaleTecnico = "" OrElse GIS_EscludiFiltroCodiceFiscaleTecnico = "false" Then
                objAlberoConfiguraGis.CfgAlbero.Flag_Appezzamenti_Filtra_Tecnico = True
            Else
                objAlberoConfiguraGis.CfgAlbero.Flag_Appezzamenti_Filtra_Tecnico = False
            End If

            '--------------------------------------------------------------------------------
            'Da Parametri Input
            '--------------------------------------------------------------------------------

            '1) Configurazione Utente

            objAlberoConfiguraGis.CfgAlbero.Flag_Planning = objImpostaCfgAlbero.CfgGisUtente.ckMostraPlanning

            objAlberoConfiguraGis.CfgAlbero.Flag_Ricette = objImpostaCfgAlbero.CfgGisUtente.ckMostraRicette

            objAlberoConfiguraGis.CfgAlbero.Flag_Fabbricati = objImpostaCfgAlbero.CfgGisUtente.ckMostraFabbricati

            objAlberoConfiguraGis.CfgAlbero.Flag_Analisi = objImpostaCfgAlbero.CfgGisUtente.chkMostraAnalisi

            objAlberoConfiguraGis.CfgAlbero.Flag_Anagrafica = objImpostaCfgAlbero.CfgGisUtente.chkMostraAnagrafica

            objAlberoConfiguraGis.CfgAlbero.Flag_CatastoAziendale = objImpostaCfgAlbero.CfgGisUtente.chkMostraCatasto

            objAlberoConfiguraGis.CfgAlbero.Flag_CatastoAppezzamento = objImpostaCfgAlbero.CfgGisUtente.chkMostraCatastoAppezzamento

            objAlberoConfiguraGis.CfgAlbero.GruppoOperazioneColturale = objImpostaCfgAlbero.CfgGisUtente.GruppoOperazioneColturale

            objAlberoConfiguraGis.CfgAlbero.TipoOperazioneColturale = objImpostaCfgAlbero.CfgGisUtente.TipoOperazioneColturale

            '2) Altra Natura

            objAlberoConfiguraGis.CfgAlbero.DatiSportelloSementieri = objImpostaCfgAlbero.DatiSportelloSementieri

            objAlberoConfiguraGis.CfgAlbero.FiltroImpiantiIdTestataTemp = objImpostaCfgAlbero.FiltroImpiantiIdTestataTemp

            objAlberoConfiguraGis.CfgAlbero.Elenco_Icone_SpecieVegetali = objImpostaCfgAlbero.Elenco_Icone_SpecieVegetali

            If Not IsNothing(objImpostaCfgAlbero.Piva) Then

                objAlberoConfiguraGis.CfgAlbero.Piva = objImpostaCfgAlbero.Piva

                If Not String.IsNullOrEmpty(objImpostaCfgAlbero.Sa_Cod) Then

                    objAlberoConfiguraGis.CfgAlbero.Sa_Cod = objImpostaCfgAlbero.Sa_Cod

                End If

            End If

            '================================================================================
            'Restituisco i parametri di configurazione utente
            '================================================================================

            objAlberoConfiguraGis.CfgGisUtente = objImpostaCfgAlbero.CfgGisUtente

            r.RispostaStringa = objAlberoConfiguraGis

            r.RispostaOK = True

        Catch ex As Exception
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.RispostaOK = False
        End Try

        Return r

    End Function

    Private Sub LeggiCfgGisDaDatabase(ByRef objParametri_Server As AgronicaCoreParametri,
                                      ByRef objParametri_Utenti As AgronicaCoreParametri,
                                      ByRef CfgGisUtente As ConfigurazioneGisUtente,
                                      Optional ByVal leggiDefault As Boolean = False)

        '--------------------------------------------------------------------------------
        'Imposta Default
        '--------------------------------------------------------------------------------

        Dim toRead As DataTable

        Dim utentiImpostazioni As New Utenti_Impostazioni_Read

        toRead = utentiImpostazioni.Leggi(enum_Impostazioni_Utenti.GIS_PARAMETRI_SETUP,
                                          2,
                                          AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                          "",
                                          "",
                                          objParametri_Utenti)

        SetConfigurationDefault(CfgGisUtente, toRead)

        If Not leggiDefault Then
            LeggiCfgUtenteDaDatabase(objParametri_Server, objParametri_Utenti, CfgGisUtente)
        End If

    End Sub

    Private Sub LeggiCfgUtenteDaDatabase(ByRef objParametri_Server As AgronicaCoreParametri,
                                         ByRef objParametri_Utenti As AgronicaCoreParametri,
                                         ByRef CfgGisUtente As ConfigurazioneGisUtente)

        Dim toRead As DataTable

        Dim utentiImpostazioni As New Utenti_Impostazioni_Read

        '--------------------------------------------------------------------------------
        'Lettura Da Database
        '--------------------------------------------------------------------------------

        toRead = utentiImpostazioni.Leggi(enum_Impostazioni_Utenti.UTENTE_GIS,
                                          1,
                                          AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                          "",
                                          "",
                                          objParametri_Utenti)

        If toRead.Rows.Count > 0 Then

            Dim valori As String() = toRead(0)("Impostazione_Valore_1").Split(ParametriUtenteGis.separatoreParametri)

            Dim dict As New Dictionary(Of String, String)

            For Each couple In valori

                Dim k_v As String() = couple.Split(ParametriUtenteGis.separatoreValori)
                If k_v(0) = ParametriUtenteGis.srvGisTipoRender_ServerSide OrElse
                   k_v(0) = ParametriUtenteGis.SistemaDiRiferimentoPredefinito OrElse
                   k_v(0) = ParametriUtenteGis.iAutoZoomSuVisualizzazioneTotale OrElse
                   k_v(0) = ParametriUtenteGis.LivelloClusterizzazione Then
                    dict.Add(k_v(0), k_v(1))
                Else
                    dict.Add(k_v(0), CBool(k_v(1)))
                End If

            Next

            Dim options As String() = {ParametriUtenteGis.chkMostraAnalisi,
                                       ParametriUtenteGis.chkMostraAnagrafica,
                                       ParametriUtenteGis.chkMostraCatasto,
                                       ParametriUtenteGis.chkMostraCatastoAppezzamento,
                                       ParametriUtenteGis.ckMostraOperazioniAgenda,
                                       ParametriUtenteGis.ckMostraPlanning,
                                       ParametriUtenteGis.ckMostraRicette,
                                       ParametriUtenteGis.ckMostraFabbricati,
                                       ParametriUtenteGis.ckGrigliaTiles_Sviluppo,
                                       ParametriUtenteGis.srvGisTipoRender_ServerSide,
                                       ParametriUtenteGis.ckViewModal,
                                       ParametriUtenteGis.ckAvversitaUsaPuntoInterno,
                                       ParametriUtenteGis.ckAvversitaPuntoPoligono,
                                       ParametriUtenteGis.ckGestioneAnalisiMappeLegacy,
                                       ParametriUtenteGis.SistemaDiRiferimentoPredefinito,
                                       ParametriUtenteGis.iAutoZoomSuVisualizzazioneTotale,
                                       ParametriUtenteGis.LivelloClusterizzazione,
                                       ParametriUtenteGis.chkMostraHeatmap}

            For Each opt In options

                If dict.ContainsKey(opt) Then

                    Select Case opt

                        Case ParametriUtenteGis.srvGisTipoRender_ServerSide
                            If dict(opt) = ConfigurazioneGisUtente.enum_TipoRender_ServerSide.Parziale.ToString("D") Then
                                CfgGisUtente.srvGisTipoRender_ServerSide = ConfigurazioneGisUtente.enum_TipoRender_ServerSide.Parziale
                            End If

                        Case ParametriUtenteGis.SistemaDiRiferimentoPredefinito
                            If IsNumeric(dict(opt)) Then
                                CfgGisUtente.SistemaDiRiferimentoPredefinito = CInt(dict(opt))
                            End If

                        Case ParametriUtenteGis.chkMostraAnalisi
                            CfgGisUtente.chkMostraAnalisi = OttieniValoreParametroGis(dict(opt), opt, objParametri_Utenti)

                        Case ParametriUtenteGis.chkMostraAnagrafica
                            CfgGisUtente.chkMostraAnagrafica = OttieniValoreParametroGis(dict(opt), opt, objParametri_Utenti)

                        Case ParametriUtenteGis.chkMostraCatasto
                            CfgGisUtente.chkMostraCatasto = OttieniValoreParametroGis(dict(opt), opt, objParametri_Utenti)

                        Case ParametriUtenteGis.chkMostraCatastoAppezzamento
                            CfgGisUtente.chkMostraCatastoAppezzamento = OttieniValoreParametroGis(dict(opt), opt, objParametri_Utenti)

                        Case ParametriUtenteGis.ckMostraOperazioniAgenda
                            CfgGisUtente.ckMostraOperazioniAgenda = OttieniValoreParametroGis(dict(opt), opt, objParametri_Utenti)

                        Case ParametriUtenteGis.ckMostraPlanning
                            CfgGisUtente.ckMostraPlanning = OttieniValoreParametroGis(dict(opt), opt, objParametri_Utenti)

                        Case ParametriUtenteGis.ckMostraRicette
                            CfgGisUtente.ckMostraRicette = OttieniValoreParametroGis(dict(opt), opt, objParametri_Utenti)

                        Case ParametriUtenteGis.ckMostraFabbricati
                            CfgGisUtente.ckMostraFabbricati = OttieniValoreParametroGis(dict(opt), opt, objParametri_Utenti)

                        Case ParametriUtenteGis.ckGrigliaTiles_Sviluppo
                            CfgGisUtente.ckGrigliaTiles_Sviluppo = OttieniValoreParametroGis(dict(opt), opt, objParametri_Utenti)

                        Case ParametriUtenteGis.ckViewModal
                            CfgGisUtente.ckViewModal = OttieniValoreParametroGis(dict(opt), opt, objParametri_Utenti)

                        Case ParametriUtenteGis.ckAvversitaUsaPuntoInterno
                            CfgGisUtente.ckAvversitaUsaPuntoInterno = OttieniValoreParametroGis(dict(opt), opt, objParametri_Utenti)

                        Case ParametriUtenteGis.ckAvversitaPuntoPoligono
                            CfgGisUtente.ckAvversitaPuntoPoligono = OttieniValoreParametroGis(dict(opt), opt, objParametri_Utenti)

                        Case ParametriUtenteGis.ckGestioneAnalisiMappeLegacy
                            CfgGisUtente.ckGestioneAnalisiMappeLegacy = OttieniValoreParametroGis(dict(opt), opt, objParametri_Utenti)

                        Case ParametriUtenteGis.SistemaDiRiferimentoPredefinito
                            CfgGisUtente.SistemaDiRiferimentoPredefinito = OttieniValoreParametroGis(dict(opt), opt, objParametri_Utenti)

                        Case ParametriUtenteGis.iAutoZoomSuVisualizzazioneTotale
                            If IsNumeric(dict(opt)) Then
                                CfgGisUtente.iAutoZoomSuVisualizzazioneTotale = CInt(dict(opt))
                            End If

                        Case ParametriUtenteGis.LivelloClusterizzazione
                            If IsNumeric(dict(opt)) Then
                                CfgGisUtente.LivelloClusterizzazione = CInt(dict(opt))
                            End If

                        Case ParametriUtenteGis.chkMostraHeatmap
                            CfgGisUtente.chkMostraHeatmap = OttieniValoreParametroGis(dict(opt), opt, objParametri_Utenti)

                    End Select

                End If

            Next

        End If

        toRead = utentiImpostazioni.Leggi(enum_Impostazioni_Utenti.UTENTE_Menu_Agenda_Selezione_Tipo_Operazioni,
                                          1,
                                          AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                          "",
                                          "",
                                          objParametri_Utenti)

        If toRead.Rows.Count > 0 Then

            CfgGisUtente.TipoOperazioneColturale = toRead(0)("Impostazione_Valore_1")

        End If

        toRead = utentiImpostazioni.Leggi(enum_Impostazioni_Utenti.UTENTE_Menu_Agenda_Selezione_GruppoOperazioni,
                                          1,
                                          AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                          "",
                                          "",
                                          objParametri_Utenti)

        If toRead.Rows.Count > 0 Then

            CfgGisUtente.GruppoOperazioneColturale = toRead(0)("Impostazione_Valore_1")

        End If

    End Sub

    Private Sub SetConfigurationDefault(ByRef cfgGisUtente As ConfigurazioneGisUtente, ByVal confDaDB As DataTable)
        If confDaDB IsNot Nothing AndAlso confDaDB.Rows.Count > 0 Then

            Dim cfgObj = JObject.Parse(confDaDB.Rows(0)("Impostazione_Valore_1").ToString)

            cfgGisUtente.chkMostraAnalisi = If(cfgObj.ContainsKey("chkMostraAnalisi"), CBool(cfgObj("chkMostraAnalisi")), False)
            cfgGisUtente.chkMostraAnagrafica = If(cfgObj.ContainsKey("chkMostraAnagrafica"), CBool(cfgObj("chkMostraAnagrafica")), True)
            cfgGisUtente.chkMostraCatasto = If(cfgObj.ContainsKey("chkMostraCatasto"), CBool(cfgObj("chkMostraCatasto")), True)
            cfgGisUtente.chkMostraCatastoAppezzamento = If(cfgObj.ContainsKey("chkMostraCatastoAppezzamento"), CBool(cfgObj("chkMostraCatastoAppezzamento")), False)
            cfgGisUtente.ckMostraOperazioniAgenda = If(cfgObj.ContainsKey("ckMostraOperazioniAgenda"), CBool(cfgObj("ckMostraOperazioniAgenda")), False)
            cfgGisUtente.ckMostraPlanning = If(cfgObj.ContainsKey("ckMostraPlanning"), CBool(cfgObj("ckMostraPlanning")), True)
            cfgGisUtente.ckMostraRicette = If(cfgObj.ContainsKey("ckMostraRicette"), CBool(cfgObj("ckMostraRicette")), False)
            cfgGisUtente.ckMostraFabbricati = If(cfgObj.ContainsKey("ckMostraFabbricati"), CBool(cfgObj("ckMostraFabbricati")), False)
            cfgGisUtente.ckGrigliaTiles_Sviluppo = If(cfgObj.ContainsKey("ckGrigliaTiles_Sviluppo"), CBool(cfgObj("ckGrigliaTiles_Sviluppo")), False)
            cfgGisUtente.srvGisTipoRender_ServerSide = [Enum].Parse(GetType(ConfigurazioneGisUtente.enum_TipoRender_ServerSide), If(cfgObj.ContainsKey("srvGisTipoRender_ServerSide"), cfgObj("srvGisTipoRender_ServerSide").ToString, ConfigurazioneGisUtente.enum_TipoRender_ServerSide.Completa)) 'ConfigurazioneGisUtente.enum_TipoRender_ServerSide.Completa
            cfgGisUtente.ckViewModal = If(cfgObj.ContainsKey("ckViewModal"), CBool(cfgObj("ckViewModal")), False)
            cfgGisUtente.ckGestioneAnalisiMappeLegacy = If(cfgObj.ContainsKey("ckGestioneAnalisiMappeLegacy"), CBool(cfgObj("ckGestioneAnalisiMappeLegacy")), False)
            cfgGisUtente.SistemaDiRiferimentoPredefinito = If(cfgObj.ContainsKey("SistemaDiRiferimentoPredefinito"), CInt(cfgObj("SistemaDiRiferimentoPredefinito")), 0)
            cfgGisUtente.TipoOperazioneColturale = If(cfgObj.ContainsKey("TipoOperazioneColturale"), cfgObj("TipoOperazioneColturale").ToString, "")
            cfgGisUtente.GruppoOperazioneColturale = If(cfgObj.ContainsKey("GruppoOperazioneColturale"), cfgObj("GruppoOperazioneColturale").ToString, "")
            cfgGisUtente.iAutoZoomSuVisualizzazioneTotale = If(cfgObj.ContainsKey("iAutoZoomSuVisualizzazioneTotale"), CInt(cfgObj("iAutoZoomSuVisualizzazioneTotale")), -1)
            cfgGisUtente.ckAvversitaUsaPuntoInterno = If(cfgObj.ContainsKey("ckAvversitaUsaPuntoInterno"), CBool(cfgObj("ckAvversitaUsaPuntoInterno")), False)
            cfgGisUtente.ckAvversitaPuntoPoligono = If(cfgObj.ContainsKey("ckAvversitaPuntoPoligono"), CBool(cfgObj("ckAvversitaPuntoPoligono")), False)
            cfgGisUtente.LivelloClusterizzazione = If(cfgObj.ContainsKey("LivelloClusterizzazione"), CInt(cfgObj("LivelloClusterizzazione")), 15)
            cfgGisUtente.chkMostraHeatmap = If(cfgObj.ContainsKey("chkMostraHeatmap"), CBool(cfgObj("chkMostraHeatmap")), False)
        Else
            setDefaultGisUtente(cfgGisUtente)
        End If
    End Sub

    Private Sub setDefaultGisUtente(ByRef cfgGisUtente As ConfigurazioneGisUtente)
        cfgGisUtente.chkMostraAnalisi = False
        cfgGisUtente.chkMostraAnagrafica = True
        cfgGisUtente.chkMostraCatasto = True
        cfgGisUtente.chkMostraCatastoAppezzamento = False
        cfgGisUtente.ckMostraOperazioniAgenda = False
        cfgGisUtente.ckMostraPlanning = True
        cfgGisUtente.ckMostraRicette = False
        cfgGisUtente.ckMostraFabbricati = False
        cfgGisUtente.ckGrigliaTiles_Sviluppo = False
        cfgGisUtente.srvGisTipoRender_ServerSide = ConfigurazioneGisUtente.enum_TipoRender_ServerSide.Completa
        cfgGisUtente.ckViewModal = False
        cfgGisUtente.ckGestioneAnalisiMappeLegacy = False
        cfgGisUtente.SistemaDiRiferimentoPredefinito = 0
        cfgGisUtente.TipoOperazioneColturale = ""
        cfgGisUtente.GruppoOperazioneColturale = ""
        cfgGisUtente.iAutoZoomSuVisualizzazioneTotale = -1
        cfgGisUtente.ckAvversitaUsaPuntoInterno = False
        cfgGisUtente.ckAvversitaPuntoPoligono = False
        cfgGisUtente.LivelloClusterizzazione = 15
        cfgGisUtente.chkMostraHeatmap = False
    End Sub

    Private Function OttieniValoreParametroGis(ByVal ValoreOpzione As String,
                                               ByVal Opzione As String,
                                               ByRef objParametri_Utenti As AgronicaCoreParametri) As Boolean

        Return CBool(ValoreOpzione) AndAlso LeggiCfgGisDaDatabaseRiscontroPermessiUtente(Opzione, objParametri_Utenti)

    End Function

    Private Function LeggiCfgGisDaDatabaseRiscontroPermessiUtente(ByVal Opzione As String,
                                                                  ByRef objParametri_Utenti As AgronicaCoreParametri) As Boolean

        Dim PermessoDaVerificare As enum_Security_Attivita

        Select Case Opzione

            Case ParametriUtenteGis.chkMostraAnalisi
                PermessoDaVerificare = enum_Security_Attivita.Gest_Analisi_AccessoMenu

            Case ParametriUtenteGis.chkMostraAnagrafica
                PermessoDaVerificare = enum_Security_Attivita.Anagrafica_Appezzamento

            Case ParametriUtenteGis.chkMostraCatasto
                PermessoDaVerificare = enum_Security_Attivita.Anagrafica_ParticellaCatastale

            Case ParametriUtenteGis.chkMostraCatastoAppezzamento
                PermessoDaVerificare = enum_Security_Attivita.Anagrafica_ParticellaCatastale

            Case ParametriUtenteGis.ckMostraOperazioniAgenda
                PermessoDaVerificare = enum_Security_Attivita.Agenda_AccessoMenu

            Case ParametriUtenteGis.ckMostraPlanning
                PermessoDaVerificare = enum_Security_Attivita.Gest_Pianificazione_Produzione_Vegetale

            Case ParametriUtenteGis.ckMostraRicette
                PermessoDaVerificare = enum_Security_Attivita.Gest_Ricette

            Case ParametriUtenteGis.ckMostraFabbricati
                PermessoDaVerificare = enum_Security_Attivita.Anagrafica_Fabbricato

            Case ParametriUtenteGis.ckGrigliaTiles_Sviluppo
                Return True

            Case ParametriUtenteGis.srvGisTipoRender_ServerSide
                Return True

            Case ParametriUtenteGis.ckViewModal
                Return True

            Case ParametriUtenteGis.ckAvversitaUsaPuntoInterno
                Return True

            Case ParametriUtenteGis.ckAvversitaPuntoPoligono
                Return True

            Case ParametriUtenteGis.ckGestioneAnalisiMappeLegacy
                Return True

            Case ParametriUtenteGis.SistemaDiRiferimentoPredefinito
                Return True

            Case ParametriUtenteGis.chkMostraHeatmap
                Return True

            Case Else
                Return False

        End Select

        Dim ObjUtenti As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
        Dim permessi As Boolean = ObjUtenti.Controlla_Permessi_Utente(objParametri_Utenti.UtenteUsername,
                                                                      Id_Servizio_GiasOnline,
                                                                      PermessoDaVerificare,
                                                                      enum_Security_Operazione.Modifica,
                                                                      Date.Now,
                                                                      "",
                                                                      objParametri_Utenti)

        Return permessi

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function CfgGIS_Leggi(InData As Object) As rispostaStandard(Of ConfigurazioneGisUtente)

        Dim r As New rispostaStandard(Of ConfigurazioneGisUtente)

        Dim objCfgGis As New ConfigurazioneGisUtente

        Try

            Dim objParametri = DeserializzaInData(Of String)(InData)

            Gias_InizializzaCultura_DaParams(objParametri.Server, objParametri.Utenti)

            LeggiCfgGisDaDatabase(objParametri.Server, objParametri.Utenti, objCfgGis)

            r.RispostaStringa = objCfgGis

            r.RispostaOK = True

        Catch ex As Exception
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.RispostaOK = False
        End Try

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function CfgGIS_Leggi_Default(InData As Object) As rispostaStandard(Of ConfigurazioneGisUtente)

        Dim r As New rispostaStandard(Of ConfigurazioneGisUtente)

        Dim objCfgGis As New ConfigurazioneGisUtente

        Try

            Dim objParametri = DeserializzaInData(Of String)(InData)

            Gias_InizializzaCultura_DaParams(objParametri.Server, objParametri.Utenti)

            LeggiCfgGisDaDatabase(objParametri.Server, objParametri.Utenti, objCfgGis, True)

            r.RispostaStringa = objCfgGis

            r.RispostaOK = True

        Catch ex As Exception
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.RispostaOK = False
        End Try

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function CfgGIS_Salva(InData As Object) As RispostaStandard

        Dim r As New RispostaStandard

        Try

            Dim objParametri = DeserializzaInData(Of ScriviConfigurazioneGisUtente)(InData)
            Dim objScriviCfgGis = objParametri.InData

            Gias_InizializzaCultura_DaParams(objParametri.Server, objParametri.Utenti)

            Dim impostazioniUtente As String = ""

            Dim dt_impostazioni As DataTable = Nothing
            Dim dt_tipo_op As DataTable = Nothing
            Dim dt_gruppo_op As DataTable = Nothing

            Dim tipoImpostazione As Int32 = 0
            Dim tipoUtente As Int32 = 0

            Dim codes As New List(Of Integer)
            Dim toSave As New List(Of String)

            If objParametri.InData.salvaDefault Then

                If Not objParametri.Server.UtenteUsername = objParametri.Server.SuperUserUsername Then
                    Throw New Exception("INVALID_USER")
                End If

                tipoImpostazione = enum_Impostazioni_Utenti.GIS_PARAMETRI_SETUP
                tipoUtente = 2
                impostazioniUtente = (JObject.FromObject(objScriviCfgGis))("CfgGisUtente").ToString
            Else

                tipoImpostazione = enum_Impostazioni_Utenti.UTENTE_GIS
                tipoUtente = 1
                impostazioniUtente = CaricaImpostazioniUtenteFormattate(objScriviCfgGis)
            End If

            If impostazioniUtente.Equals("") Or tipoImpostazione = 0 Or tipoUtente = 0 Then
                Throw New Exception("Errore nel caricamento dei dati di input.")
            End If

            codes.Add(tipoImpostazione)
            toSave.Add(impostazioniUtente)

            If Not objParametri.InData.salvaDefault AndAlso objScriviCfgGis.MemorizzaOperazioneColturale Then

                codes.Add(enum_Impostazioni_Utenti.UTENTE_Menu_Agenda_Selezione_Tipo_Operazioni)
                toSave.Add(objScriviCfgGis.CfgGisUtente.TipoOperazioneColturale)

                codes.Add(enum_Impostazioni_Utenti.UTENTE_Menu_Agenda_Selezione_GruppoOperazioni)
                toSave.Add(objScriviCfgGis.CfgGisUtente.GruppoOperazioneColturale)

            End If

            Dim utentiImpostazioni_R As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
            Dim utentiImpostazioni_W As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_W

            Dim i_s As Integer = 0

            For Each code In codes

                Dim dt As DataTable
                dt = utentiImpostazioni_R.Leggi(code,
                                                tipoUtente,
                                                AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                "",
                                                "",
                                                objParametri.Utenti)

                If dt.Rows.Count > 0 Then

                    If code = enum_Impostazioni_Utenti.UTENTE_GIS AndAlso Not objScriviCfgGis.MemorizzaSistemaDiRiferimentoPredefinito Then
                        Dim valoreSistemaRiferimentoPredefinitoPrecedente = determinaValoreSistemaRiferimentoPredefinitoPrecedente(dt)
                        If Not IsNothing(valoreSistemaRiferimentoPredefinitoPrecedente) Then
                            toSave(i_s) = CaricaImpostazioniUtenteFormattate(objScriviCfgGis, valoreSistemaRiferimentoPredefinitoPrecedente)
                        End If
                    End If

                    utentiImpostazioni_W.Modifica(code,
                                                  toSave(i_s),
                                                  "",
                                                  "",
                                                  "",
                                                  AGRODATAINIZIO,
                                                  AGRODATAFINE,
                                                  objParametri.Utenti,
                                                  objParametri.InData.salvaDefault)

                Else

                    utentiImpostazioni_W.Scrivi(code,
                                                toSave(i_s),
                                                "",
                                                "",
                                                "",
                                                AGRODATAINIZIO,
                                                AGRODATAFINE,
                                                objParametri.Utenti,
                                                objParametri.InData.salvaDefault)

                End If

                i_s += 1

            Next

            r.RispostaOK = True

        Catch ex As Exception
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.RispostaOK = False
        End Try

        Return r

    End Function

    Private Function CaricaImpostazioniUtenteFormattate(ByRef objScriviCfgGis As ScriviConfigurazioneGisUtente,
                                                        Optional ByVal SistemaDiRiferimentoPredefinito As Integer? = Nothing
                                                        ) As String

        Dim formato = "{0}^{1}" &
            "§{2}^{3}" &
            "§{4}^{5}" &
            "§{6}^{7}" &
            "§{8}^{9}" &
            "§{10}^{11}" &
            "§{12}^{13}" &
            "§{14}^{15}" &
            "§{16}^{17}" &
            "§{18}^{19}" &
            "§{20}^{21}" &
            "§{22}^{23}" &
            "§{24}^{25}" &
            "§{26}^{27}" &
            "§{28}^{29}" &
            "§{30}^{31}" &
            "§{32}^{33}"

        If IsNothing(SistemaDiRiferimentoPredefinito) Then
            SistemaDiRiferimentoPredefinito = objScriviCfgGis.CfgGisUtente.SistemaDiRiferimentoPredefinito
        End If

        Dim impostazioni = String.Format(formato,
                                         ParametriUtenteGis.ckMostraOperazioniAgenda,
                                         objScriviCfgGis.CfgGisUtente.ckMostraOperazioniAgenda,
                                         ParametriUtenteGis.ckMostraPlanning,
                                         objScriviCfgGis.CfgGisUtente.ckMostraPlanning,
                                         ParametriUtenteGis.ckMostraRicette,
                                         objScriviCfgGis.CfgGisUtente.ckMostraRicette,
                                         ParametriUtenteGis.ckMostraFabbricati,
                                         objScriviCfgGis.CfgGisUtente.ckMostraFabbricati,
                                         ParametriUtenteGis.chkMostraAnalisi,
                                         objScriviCfgGis.CfgGisUtente.chkMostraAnalisi,
                                         ParametriUtenteGis.chkMostraAnagrafica,
                                         objScriviCfgGis.CfgGisUtente.chkMostraAnagrafica,
                                         ParametriUtenteGis.chkMostraCatasto,
                                         objScriviCfgGis.CfgGisUtente.chkMostraCatasto,
                                         ParametriUtenteGis.chkMostraCatastoAppezzamento,
                                         objScriviCfgGis.CfgGisUtente.chkMostraCatastoAppezzamento,
                                         ParametriUtenteGis.SistemaDiRiferimentoPredefinito,
                                         SistemaDiRiferimentoPredefinito,
                                         ParametriUtenteGis.ckGrigliaTiles_Sviluppo,
                                         objScriviCfgGis.CfgGisUtente.ckGrigliaTiles_Sviluppo,
                                         ParametriUtenteGis.srvGisTipoRender_ServerSide,
                                         objScriviCfgGis.CfgGisUtente.srvGisTipoRender_ServerSide.ToString("D"),
                                         ParametriUtenteGis.ckViewModal,
                                         objScriviCfgGis.CfgGisUtente.ckViewModal,
                                         ParametriUtenteGis.iAutoZoomSuVisualizzazioneTotale,
                                         objScriviCfgGis.CfgGisUtente.iAutoZoomSuVisualizzazioneTotale,
                                         ParametriUtenteGis.ckAvversitaUsaPuntoInterno,
                                         objScriviCfgGis.CfgGisUtente.ckAvversitaUsaPuntoInterno,
                                         ParametriUtenteGis.ckAvversitaPuntoPoligono,
                                         objScriviCfgGis.CfgGisUtente.ckAvversitaPuntoPoligono,
                                         ParametriUtenteGis.ckGestioneAnalisiMappeLegacy,
                                         objScriviCfgGis.CfgGisUtente.ckGestioneAnalisiMappeLegacy,
                                         ParametriUtenteGis.LivelloClusterizzazione,
                                         objScriviCfgGis.CfgGisUtente.LivelloClusterizzazione,
                                         ParametriUtenteGis.chkMostraHeatmap,
                                         objScriviCfgGis.CfgGisUtente.chkMostraHeatmap)

        Return impostazioni

    End Function

    Private Function determinaValoreSistemaRiferimentoPredefinitoPrecedente(ByVal dt As DataTable) As String

        Dim valoreSistemaRiferimentoPredefinitoPrecedente = Nothing

        Dim impostazioniUtenteGis = dt.Rows(0).Item("Impostazione_Valore_1").ToString().Split("§")

        If impostazioniUtenteGis.Count > 0 Then

            For i As Integer = 0 To impostazioniUtenteGis.Count - 1

                If impostazioniUtenteGis(i).StartsWith(ParametriUtenteGis.SistemaDiRiferimentoPredefinito) Then

                    Dim elencoSistemaRiferimentoPredefinitoPrecedente = impostazioniUtenteGis(i).Split("^")

                    If elencoSistemaRiferimentoPredefinitoPrecedente.Count = 2 Then
                        valoreSistemaRiferimentoPredefinitoPrecedente = elencoSistemaRiferimentoPredefinitoPrecedente(1)
                    End If

                    Exit For

                End If

            Next

        End If

        Return valoreSistemaRiferimentoPredefinitoPrecedente

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function CfgGIS_Leggi_Generali(InData As Object) As rispostaStandard(Of ConfigurazioniGisGenerali)

        Dim r As New rispostaStandard(Of ConfigurazioniGisGenerali)

        Dim objCfgGisGenerali As New ConfigurazioniGisGenerali

        Try

            Dim objParametri = DeserializzaInData(Of LeggiConfigurazioniGeneraliGis)(InData)
            Dim objLeggiCfgGenerali = objParametri.InData

            Gias_InizializzaCultura_DaParams(objParametri.Server, objParametri.Utenti)

            'Annata Agraria

            Dim leggiAnnataAgraria As New Utenti_Impostazioni_Read
            Dim d1, d2 As Date
            leggiAnnataAgraria.AnnataAgraria(Now.Date, d1, d2, objParametri.Utenti)

            objCfgGisGenerali.AnnataAgrariaInizio = d1
            objCfgGisGenerali.AnnataAgrariaFine = d2

            'Demo Gis Attivo

            Dim ObjUtenti As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
            Dim permessiAnalisiDatiSat = ObjUtenti.Controlla_Permessi_Utente(objParametri.Utenti.UtenteUsername,
                                                                             Id_Servizio_GiasOnline,
                                                                             enum_Security_Attivita.GIS_SAT_AnalisiDatiSatellitari,
                                                                             enum_Security_Operazione.Modifica,
                                                                             Date.Now,
                                                                             "",
                                                                             objParametri.Utenti)

            If permessiAnalisiDatiSat Then
                objCfgGisGenerali.DemoGisAttivo = True
            Else
                objCfgGisGenerali.DemoGisAttivo = False
            End If

            'Prosecco Gis Attivo

            If Not IsNothing(ConfigurationSettings.AppSettings("proseccoGis")) AndAlso ConfigurationSettings.AppSettings("proseccoGis") = True Then
                objCfgGisGenerali.ProseccoGisAttivo = True
            Else
                objCfgGisGenerali.ProseccoGisAttivo = False
            End If

            'Wms Attivo

            Dim permessiWMS As Boolean = ObjUtenti.Controlla_Permessi_Utente(objParametri.Utenti.UtenteUsername,
                                                                             Id_Servizio_GiasOnline,
                                                                             enum_Security_Attivita.GIS_VisualizzazioneGestioneWMS,
                                                                             enum_Security_Operazione.Modifica,
                                                                             Date.Now,
                                                                             "",
                                                                             objParametri.Utenti)

            If permessiWMS Then
                objCfgGisGenerali.WmsAttivo = True
            Else
                objCfgGisGenerali.WmsAttivo = False
            End If

            'Gis Global Map Base Path

            Dim objCfgSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
            objCfgGisGenerali.Gis_Global_ws_mappe_2013_basePath = objCfgSiti.Leggi_Valore(0,
                                                                                          "GiasOnline_WS_Mappe_2013",
                                                                                          "",
                                                                                          "",
                                                                                          objParametri.Server)

            'Inizializza richiedi percorsi e inizializzazione parametri cartografici

            Dim objVM As New V_M

            Dim ParametriFiltro = objLeggiCfgGenerali.ParametriFiltroPercorsi

            If Not String.IsNullOrEmpty(ParametriFiltro) Then
                objCfgGisGenerali.RichiediPercorsiInizializza = True
                Dim r2 = objVM.InizializzazioneLetturaParametriCartografici(ParametriFiltro, objParametri.Server)
                objCfgGisGenerali.srvGisParamCartograficiInizializza = JsonConvert.DeserializeObject(Of srvGisParametriCartograficiInizializzazione)(r2.RispostaStringa)
            Else
                objCfgGisGenerali.RichiediPercorsiInizializza = False
            End If

            'Filtrone impostato

            objCfgGisGenerali.IDTestataTemp = 0

            If Not String.IsNullOrEmpty(objLeggiCfgGenerali.Filtrone) Then
                objCfgGisGenerali.filtroneImpostato = True

                If EsistonoRigheImpiantoInParametri(objLeggiCfgGenerali.Impianti) Then

                    PassaRisultatoDaFiltroneAdAlbero(objLeggiCfgGenerali, objCfgGisGenerali, objParametri.Server)

                End If

            End If

            'Default combo elemento grafico generazione poligoni

            objCfgGisGenerali.GIS_cmbElementoGrafico_GenerazionePoligoni_DefaultValue = objCfgSiti.Leggi_Valore(0,
                                                                                                                "GIS_cmbElementoGrafico_GenerazionePoligoni_DefaultValue",
                                                                                                                "",
                                                                                                                "",
                                                                                                                objParametri.Server)

            If String.IsNullOrEmpty(objCfgGisGenerali.GIS_cmbElementoGrafico_GenerazionePoligoni_DefaultValue) Then
                objCfgGisGenerali.GIS_cmbElementoGrafico_GenerazionePoligoni_DefaultValue = "3"
            End If

            '--------------------------------------------------------------------------------
            'Sezione InizializzaProprietajs
            '--------------------------------------------------------------------------------

            Dim permessi As New PermessiUtenteCartografia(objParametri.Server, objParametri.Utenti)

            objCfgGisGenerali.CiSonoVecchiDatiNonImportati = objVM.ControllaSeCiSonoVecchiDatiNonImportati(objParametri.Server)

            'Abilita Precision Farming

            objCfgGisGenerali.AbilitaPF = permessi.permessoPrecisionFarming

            'Codice fiscale tecnico

            Dim objUtentiDAL As New Utenti_xGruppi_Utente_R
            objCfgGisGenerali.Codice_Fiscale_Tecnico = objUtentiDAL.Leggi_IdentificativoGruppoUtenti_Singolo(objParametri.Server.UtenteUsername, objParametri.Utenti)

            'Finestra temporale GIS

            Dim gp As New GisPurpose(objLeggiCfgGenerali.Sementi, objLeggiCfgGenerali.SementiMappaturaLibera)

            Dim FinestraTemporale_GIS_inizio = gp.SementiSportelloDataInizio(objParametri.Server.FinestraTemporaleInizio)
            Dim FinestraTemporale_GIS_fine = gp.SementiSportelloDataFine(objParametri.Server.FinestraTemporaleFine)

            objCfgGisGenerali.sFinestraTemporale_GIS_Inizio = If(FinestraTemporale_GIS_inizio <> AGRODATAINIZIO, FinestraTemporale_GIS_inizio, "")
            objCfgGisGenerali.sFinestraTemporale_GIS_Fine = If(FinestraTemporale_GIS_fine <> AGRODATAFINE, FinestraTemporale_GIS_fine, "")

            '--------------------------------------------------------------------------------
            'Sezione ImpostaVisibilitaPulsanti
            '--------------------------------------------------------------------------------

            objCfgGisGenerali.MostraNuovoDisegno = True

            objCfgGisGenerali.MostraPulsanteSalva = True

            objCfgGisGenerali.MostraElimina = True

            objCfgGisGenerali.MostraEsporta = True

            objCfgGisGenerali.MostraImporta = True

            objCfgGisGenerali.MostraAB = True

            objCfgGisGenerali.permessoAnalisiMeteo = permessi.permessoAnalisiMeteo

            objCfgGisGenerali.permessoVisite = permessi.permessoVisite

            '--------------------------------------------------------------------------------
            'Sezione MostraNascondiPulsanti
            '--------------------------------------------------------------------------------

            objCfgGisGenerali.permessoCatasto = permessi.permessoCatasto

            objCfgGisGenerali.permessoPrecisionFarming = permessi.permessoPrecisionFarming

            objCfgGisGenerali.permessoEsportaDati = permessi.permessoEsportaDati

            objCfgGisGenerali.permessoBufferZone = permessi.permessoBufferZone

            '================================================================================
            'Risultato
            '================================================================================

            r.RispostaStringa = objCfgGisGenerali

            r.RispostaOK = True

        Catch ex As Exception
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.RispostaOK = False
        End Try

        Return r

    End Function

    Private Function EsistonoRigheImpiantoInParametri(ByVal Impianti As List(Of Impianto2010))

        Return (Not IsNothing(Impianti)) AndAlso (Impianti.Count > 0)

    End Function

    Private Sub PassaRisultatoDaFiltroneAdAlbero(ByRef objLeggiCfgGenerali As LeggiConfigurazioniGeneraliGis,
                                                 ByRef objCfgGisGenerali As ConfigurazioniGisGenerali,
                                                 ByRef objParametri_Server As AgronicaCoreParametri)

        'Passa il risultato di ricerca dal filtrone all'albero

        'TODO_AG: Valutare se è possibile gestire la scrittura della tabella __tmp_FiltroImpianti fuori da questa chiamata.
        '         In questo nella funzione resterà solo la parte che prevedere IDTestataTemp valorizzato.
        '         Non servirà più:
        '         - Lista Impianti di input
        '         - Classe UtilityGis

        Dim ImpostaVisibilitaTotale As Boolean = False

        If String.IsNullOrEmpty(objLeggiCfgGenerali.IDTestataTemp) Then

            Dim xAgrosequenze As New Agro_Sequenze
            'Lavez - 12/07/2024 - normalizzazione chiamate a stack counter
            objCfgGisGenerali.IDTestataTemp = xAgrosequenze.NuovoId_Tabella("IDTestataTemp", 0, AgronicaCoreDataProvider.CostantiPersonalizzate.UpperBoundTabelle_Per_SequenzaTabelle_Topcode, objParametri_Server)
            'objCfgGisGenerali.IDTestataTemp = xAgrosequenze.Agronica_SequenzaTabelle_NuovoID("IDTestataTemp", objParametri_Server)

            Dim OperazioneCorrente_FiltroImpianti As String = ""

            FiltroneImpiantiDbTemp.OperazioneCorrente_EstraiFiltrone(objCfgGisGenerali.IDTestataTemp,
                                                                     objLeggiCfgGenerali.Impianti,
                                                                     OperazioneCorrente_FiltroImpianti,
                                                                     objParametri_Server)

            FiltroneImpiantiDbTemp.PopolaTabellaFiltroImpianti(OperazioneCorrente_FiltroImpianti, objParametri_Server)

            Dim listaImpiantiTMP = (From ii In objLeggiCfgGenerali.Impianti Select ii.Piva).Distinct.ToList

            If listaImpiantiTMP.Count = 1 Then
                objCfgGisGenerali.Piva = objLeggiCfgGenerali.Impianti.First.Piva
            End If

            If listaImpiantiTMP.Count > 1 Then
                ImpostaVisibilitaTotale = True
            End If

        Else

            objCfgGisGenerali.IDTestataTemp = objLeggiCfgGenerali.IDTestataTemp

            Dim leggiFiltroImpianti As New AgronicaCoreVarieDAL.__tmp_FiltroImpianti_R
            Dim dtImpianti As DataTable = leggiFiltroImpianti.ConteggioRecordDistinctPivaDaIDTestataTemp(objCfgGisGenerali.IDTestataTemp, objParametri_Server)

            Dim conteggioRecordTmp As Integer = dtImpianti.Rows.Count

            If conteggioRecordTmp = 1 Then
                objCfgGisGenerali.Piva = dtImpianti.Rows(0)("piva")
            End If

            If conteggioRecordTmp > 1 Then
                ImpostaVisibilitaTotale = True
            End If

        End If

        If ImpostaVisibilitaTotale Then
            objCfgGisGenerali.Piva = ""
        End If

    End Sub

    ''' <summary>
    ''' Inizializza le configurazioni per i sistemi di riferimento cartografia
    ''' </summary>
    ''' <returns>Restituisce un obj Cfg_GestioneSistemaRif_Out</returns>
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function CfgGestioneSistemaDiRiferimentoPredefinito(ByVal InData As Object) As rispostaStandard(Of Cfg_GestioneSistemaRif_Out)
        Dim r As New rispostaStandard(Of Cfg_GestioneSistemaRif_Out)

        Dim objParametri = DeserializzaInData(Of String)(InData)

        Dim obj_Cfg_GestioneSistemaRif_Out As New Cfg_GestioneSistemaRif_Out

        Gias_InizializzaCultura_DaParams(objParametri.Server, objParametri.Utenti)

        Try

            Dim carica As New AgronicaCoreGisDAL.GIS_SistemiRiferimentoCartografia_R
            Dim dt1 As DataTable = carica.Leggi(0,
                                                AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                "",
                                                "",
                                                objParametri.Server)

            Dim utentiImpostazioni_R As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
            Dim dt As DataTable
            dt = utentiImpostazioni_R.Leggi(enum_Impostazioni_Utenti.UTENTE_GIS,
                                            1,
                                            AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                            "",
                                            "",
                                            objParametri.Utenti)

            Dim valorePredefinito As String = "-1"
            If dt.Rows.Count > 0 Then
                Dim imposta As String = dt(0)("Impostazione_Valore_1")
                Dim vImposta As String() = imposta.Split("§")
                For Each impostazione In vImposta
                    If impostazione.Contains("SistemaDiRiferimentoPredefinito") Then
                        valorePredefinito = impostazione.Split("^")(1)
                    End If
                Next
            End If

            Dim primo As Boolean = True
            Dim elenco As New List(Of Elemento_CfgSistemaRif_Out)

            For Each it In dt1.Rows
                Dim elem As New Elemento_CfgSistemaRif_Out

                If primo Then
                    Dim primo_elem As New Elemento_CfgSistemaRif_Out With {
                        .Text = "Nessuna Trasformazione",
                        .Value = "-1"
                    }

                    elenco.Add(primo_elem)
                    primo = False

                End If

                elem.Text = it("Descrizione")
                elem.Value = it("GeoRiferimento_Cod")

                elenco.Add(elem)

            Next

            obj_Cfg_GestioneSistemaRif_Out.Elenco = elenco
            obj_Cfg_GestioneSistemaRif_Out.Selezione = valorePredefinito

            r.RispostaStringa = obj_Cfg_GestioneSistemaRif_Out

        Catch ex As Exception

            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.RispostaOK = False

        End Try

        Return r

    End Function

#End Region

#Region "Nuove Chiamate GIS"
    'lavez - esempio di implementazione SqlSpatialConverter
    'PS: questo tool restituisce in formato geojson l'elemento geometry dei singoli elementi da inserire nel GeoJson
    '<WebMethod()>
    '<Script.Services.ScriptMethod()>
    'Public Function Test(ByVal wktstring As String) As RispostaStandard
    '    Dim r As New RispostaStandard
    '    Try
    '        Dim t As New SqlSpatialConverter(Of WktParser)(wktstring)
    '        r.RispostaOK = True
    '        r.RispostaStringa = JsonConvert.SerializeObject(t.ToGeoJson(), New Converters.StringEnumConverter)
    '    Catch ex As Exception
    '        r.RispostaOK = False
    '        r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
    '    End Try
    '    Return r
    'End Function

    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function LeggiElencoEntitaGeoJson(ByVal objP_super_server As String,
                                             ByVal objP_server As String,
                                             ByVal objP_utenti As String,
                                             InData As Object
                                             ) As rispostaStandard(Of GisDataReadRval_New(Of AgronicaCoreModelsSTD.Gis.GeoJSONAgroGisProp))

        Dim r As New rispostaStandard(Of GisDataReadRval_New(Of AgronicaCoreModelsSTD.Gis.GeoJSONAgroGisProp))

        Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_super_server)
        Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
        Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)

        Dim iData As GisDataReadParam = JsonConvert.DeserializeObject(Of GisDataReadParam)(JsonConvert.SerializeObject(InData))

        Try

            Dim leggiLingua As New Lingue_Read
            Dim dtLingua As DataTable = leggiLingua.Leggi_datoCODGIAS(objParametri_Server.Lingua_Cod,
                                                                      AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                      "",
                                                                      "",
                                                                      objParametri_Utenti)

            Dim linguaCodiceISO As String = dtLingua.Rows(0)("CodiceISO")
            Threading.Thread.CurrentThread.CurrentUICulture = New Globalization.CultureInfo(linguaCodiceISO)

            Dim obj As New GisDataReadRval_New(Of AgronicaCoreModelsSTD.Gis.GeoJSONAgroGisProp)

            'piccolo test google
            'fileContents = My.Computer.FileSystem.ReadAllText(Server.MapPath("." & "\SmallTest.json"))

            If iData.cfgAlbero Is Nothing Then
                iData.cfgAlbero = New ConfigurazioneAlbero
            End If

            iData.cfgAlbero.TipologiaLayer_Cod = IIf((iData.TipologiaLayerSelezionata = "" Or iData.TipologiaLayerSelezionata Is Nothing), 1, iData.TipologiaLayerSelezionata)

            'Impostazione finestra temporale

            If Not IsNothing(iData.filtroTemporale) Then

                objParametri_Server.FinestraTemporaleInizio = iData.filtroTemporale.DataInizio
                objParametri_Server.FinestraTemporaleFine = iData.filtroTemporale.DataFine

                '====================================================================================================
                'Al momento i limiti non vengono applicati in...
                '- LeggiGetQueryCondizioniWhere_PorzioneFiltroTemporale (GIS_Entita.vb)
                'Ma vengono usati in...
                '- Internal_SQL_GisEntitaConFiltroTemporaleGetQuery_filtroTemporale (GIS_Entita.vb)
                '====================================================================================================

                '----------------------------------------------------------------------------------------------------
                'LIMITE INFERIORE (legato a FinestraTemporaleFine)
                '----------------------------------------------------------------------------------------------------
                'SQL default : Tabella.Validita_Inizio <= Agro_SQL_SaveDate(FinestraTemporaleFine)
                'SQL c/limite: Tabella.Validita_Inizio & limite_inferiore & Agro_SQL_SaveDate(FinestraTemporaleFine)

                Dim limiteInferiore = GIS_Utility.GetLimiteByTipoOperatoreData("<=", iData.filtroTemporale.TipoOperatoreDataFine)
                HttpContext.Current.Session("limite_inferiore") = limiteInferiore

                '----------------------------------------------------------------------------------------------------
                'LIMITE SUPERIORE (legato a FinestraTemporaleInizio)
                '----------------------------------------------------------------------------------------------------
                'SQL default : Tabella.Validita_Fine >= Agro_SQL_SaveDate(FinestraTemporaleInizio)
                'SQL c/limite: Tabella.Validita_Fine & limite_superiore & Agro_SQL_SaveDate(FinestraTemporaleInizio)

                Dim limiteSuperiore = GIS_Utility.GetLimiteByTipoOperatoreData(">=", iData.filtroTemporale.TipoOperatoreDataInizio)
                HttpContext.Current.Session("limite_superiore") = limiteSuperiore

            End If

            'Lettura GeoJson

            obj.myGeoJson = V_M.AggiornaLayer_3_New(objParametri_Super_Server, objParametri_Server, objParametri_Utenti, iData, iData.cfgAlbero)

            r.RispostaOK = True
            r.RispostaStringa = obj

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r

    End Function

#End Region

#Region "Funzionalità Comuni GIS 2022"

    Friend Class ParametriUtenteGis
        Public Const separatoreParametri = "§"
        Public Const separatoreValori = "^"
        Public Const chkMostraAnalisi = "chkMostraAnalisi"
        Public Const chkMostraAnagrafica = "chkMostraAnagrafica"
        Public Const chkMostraCatasto = "chkMostraCatasto"
        Public Const chkMostraCatastoAppezzamento = "chkMostraCatastoAppezzamento"
        Public Const ckMostraOperazioniAgenda = "ckMostraOperazioniAgenda"
        Public Const ckMostraPlanning = "ckMostraPlanning"
        Public Const ckMostraRicette = "ckMostraRicette"
        Public Const ckMostraFabbricati = "ckMostraFabbricati"
        Public Const ckGrigliaTiles_Sviluppo = "ckGrigliaTiles_Sviluppo"
        Public Const srvGisTipoRender_ServerSide = "srvGisTipoRender_ServerSide"
        Public Const ckViewModal = "ckViewModal"
        Public Const ckAvversitaUsaPuntoInterno = "ckAvversitaUsaPuntoInterno"
        Public Const ckAvversitaPuntoPoligono = "ckAvversitaPuntoPoligono"
        Public Const ckGestioneAnalisiMappeLegacy = "ckGestioneAnalisiMappeLegacy"
        Public Const SistemaDiRiferimentoPredefinito = "SistemaDiRiferimentoPredefinito"
        Public Const iAutoZoomSuVisualizzazioneTotale = "iAutoZoomSuVisualizzazioneTotale"
        Public Const LivelloClusterizzazione = "LivelloClusterizzazione"
        Public Const chkMostraHeatmap = "chkMostraHeatmap"
    End Class

    'Classe per la "deserializzazione" dell'oggetto in entrata dalle nuove chiamate API ai WebMethod
    Private Class ObjParametri(Of T)

        Public Super_Server As AgronicaCoreParametri
        Public Server As AgronicaCoreParametri
        Public Utenti As AgronicaCoreParametri
        Public InData As T

    End Class

    Private Shared Sub Gias_InizializzaCultura_DaParams(ByRef objParametri_Server As AgronicaCoreParametri,
                                                        ByRef objParametri_Utenti As AgronicaCoreParametri)

        Dim leggiLingua As New Lingue_Read

        Dim dtLingua As DataTable = leggiLingua.Leggi_datoCODGIAS(objParametri_Server.Lingua_Cod,
                                                                  AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                  "",
                                                                  "",
                                                                  objParametri_Utenti)

        Dim linguaCodiceISO As String = dtLingua.Rows(0)("CodiceISO")

        Threading.Thread.CurrentThread.CurrentUICulture = New Globalization.CultureInfo(linguaCodiceISO)

    End Sub

    'Viene utilizzata per la "deserializzazione" dell'oggetto in entrata dalle nuove chiamate API ai WebMethod
    Private Function DeserializzaInData(Of T)(ByVal InData As Object) As ObjParametri(Of T)

        Dim JsonSettings As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}

        Dim objInData As CoreWS_Generic(Of T) = JsonConvert.DeserializeObject(Of CoreWS_Generic(Of T))(JsonConvert.SerializeObject(InData), JsonSettings)

        Dim objParametri As New ObjParametri(Of T)

        objParametri.Super_Server = Utility.convertStringtoOBJparametri(objInData.objP.objP_super_server)
        objParametri.Server = Utility.convertStringtoOBJparametri(objInData.objP.objP_server)
        objParametri.Utenti = Utility.convertStringtoOBJparametri(objInData.objP.objP_utenti)

        objParametri.InData = objInData.InData

        Return objParametri

    End Function

#End Region

#Region "Nuovi Web Service GIS 2022"

    ''' <summary>
    ''' Popola il GIS con le varie entita
    ''' </summary>
    ''' <returns>Lista di TipoEntita_Out</returns>
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function GIS_TipoEntita_Popola(ByVal InData As Object) As rispostaStandard(Of List(Of TipoEntita_Out))
        Dim r As New rispostaStandard(Of List(Of TipoEntita_Out))

        Dim objParametri = DeserializzaInData(Of String)(InData)

        Gias_InizializzaCultura_DaParams(objParametri.Server, objParametri.Utenti)

        Try
            r = V_M.GIS_TipoEntita_Popola_2022(objParametri.Server)

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function

    ''' <summary>
    ''' Scrive sul DB l'xml di passaggio tra i vari siti
    ''' </summary>
    ''' <param name="Entita_Cod"></param>
    ''' <returns>URL di redirect per Sito Analisi + obj contenente parametri Analisi2010</returns>
    ''' <remarks></remarks>    
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function ApriSitoAnalisixVisualizzazione(ByVal InData As Object) As rispostaStandard(Of ApriSitoAnalisi_Out)
        Dim r As New rispostaStandard(Of ApriSitoAnalisi_Out)

        Dim objParametri = DeserializzaInData(Of LeggiEntita_In)(InData)
        Dim objEntita As LeggiEntita_In = objParametri.InData

        Gias_InizializzaCultura_DaParams(objParametri.Server, objParametri.Utenti)

        Try
            r = V_M.ApriSitoAnalisixVisualizzazione_2022(objEntita,
                                                         objParametri.Server,
                                                         objParametri.Utenti,
                                                         objParametri.Super_Server)

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function

    ''' <summary>
    ''' Carica uno o più obj di tipo Impianto all’interno del GIS
    ''' </summary>
    ''' <param name="Piva"></param>
    ''' <param name="Sa_Cod"></param>
    ''' <param name="Appezza"></param>
    ''' <param name="Id_Reg"></param>
    ''' <returns>Restituisce un obj di tipo SingoloObjImpianto_Out</returns>
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function CaricaSingoloOggetto_Impianto(ByVal InData As Object) As rispostaStandard(Of SingoloObjImpianto_Out)
        Dim r As New rispostaStandard(Of SingoloObjImpianto_Out)

        Dim objParametri = DeserializzaInData(Of ChiaveImpianto_In)(InData)
        Dim obj_SingoloImpianto As ChiaveImpianto_In = objParametri.InData

        Gias_InizializzaCultura_DaParams(objParametri.Server, objParametri.Utenti)

        Try
            r = V_M.CaricaSingoloOggetto_Impianto_2022(obj_SingoloImpianto,
                                                       objParametri.Server,
                                                       objParametri.Utenti)

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = AgronicaControlliGIS.My.Resources.AgronicaControlliGIS.V_M_Errore_Operazione & ": " & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function


    ''' <summary>
    ''' Genera URL per il redirect verso una pagina di Analisi (meteo, modelli o rilievi)
    ''' </summary>
    ''' <param name="Lat"></param>
    ''' <param name="Long"></param>
    ''' <param name="ChiaveAlbero"></param>
    ''' <param name="Veg_Cod"></param>
    ''' <param name="Flag_TipoAnalisi"></param>
    ''' <returns>obj AnalisiMeteo_Out</returns>
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function AnalisiMeteoBs(ByVal InData As Object) As rispostaStandard(Of AnalisiMeteo_Out)
        Dim r As New rispostaStandard(Of AnalisiMeteo_Out)

        Dim objParametri = DeserializzaInData(Of AnalisiMeteo_In)(InData)
        Dim objAnalisiBs As AnalisiMeteo_In = objParametri.InData

        Gias_InizializzaCultura_DaParams(objParametri.Server, objParametri.Utenti)

        Try
            r = V_M.AnalisiMeteoBs_2022(objAnalisiBs,
                                        objParametri.Server,
                                        objParametri.Utenti,
                                        objParametri.Super_Server)

        Catch ex As Exception

            r.RispostaOK = False

            'Uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function

    ''' <summary>
    ''' Esegue un Update dell'Appezzamento passato
    ''' </summary>
    ''' <param name="ChiaveAlbero"></param>
    ''' <param name="DistBZ_CorpiIdrici"></param>
    ''' <param name="DistBZ_AreeResPub"></param>
    ''' <param name="DistBZ_Allevamenti"></param>
    ''' <param name="DistBZ_VegNatNonColt"></param>
    ''' <param name="SupBZ_Riduzione"></param>
    ''' <returns>Messaggio di corretta esecuzione o meno</returns>
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function BufferZone_Aggiorna(ByVal InData As Object) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri = DeserializzaInData(Of BufferZone_In)(InData)
        Dim objBufferZone As BufferZone_In = objParametri.InData

        Gias_InizializzaCultura_DaParams(objParametri.Server, objParametri.Utenti)

        Try
            r = V_M.BufferZone_Aggiorna_2022(objBufferZone,
                                             objParametri.Server)

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r

    End Function

    ''' <summary>
    ''' Legge e ritorna un oggetto di tipo Appezzamento, calcolandone la BufferZone 
    ''' </summary>
    ''' <param name="Entita_Cod"></param>
    ''' <param name="ChiaveAlbero"></param>
    ''' <returns>obj LeggiBufferZone_Out</returns>
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function BufferZone_Leggi(ByVal InData As Object) As rispostaStandard(Of LeggiBufferZone_Out)
        Dim r As New rispostaStandard(Of LeggiBufferZone_Out)

        Dim objParametri = DeserializzaInData(Of BufferZone_In)(InData)
        Dim objBufferZone As BufferZone_In = objParametri.InData

        Gias_InizializzaCultura_DaParams(objParametri.Server, objParametri.Utenti)

        Try
            r = V_M.BufferZone_Leggi_2022(objBufferZone,
                                          objParametri.Server)

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r

    End Function

    ''' <summary>
    ''' Carica i codici degli elementi Impresa utilizzando la chiave (= Piva + Id_Cod)
    ''' </summary>
    ''' <param name="Piva"></param>
    ''' <param name="Id_Cod"></param>
    ''' <returns>Val_Cod</returns>
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function CaricaImpreseCodici(ByVal InData As Object) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri = DeserializzaInData(Of CaricaImpreseCod_In)(InData)
        Dim objCaricaImpreseCod As CaricaImpreseCod_In = objParametri.InData

        Gias_InizializzaCultura_DaParams(objParametri.Server, objParametri.Utenti)

        Try
            r = V_M.CaricaImpreseCodici_2022(objCaricaImpreseCod,
                                             objParametri.Server)

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r

    End Function

    ''' <summary>
    ''' Gestisce la colorazione automatica
    ''' </summary>
    ''' <param name="TipologiaLayer_cod"></param>
    ''' <returns>str_RISPOSTA</returns>
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function ColorazioneAutomatica(ByVal InData As Object) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri = DeserializzaInData(Of Integer)(InData)
        Dim TipologiaLayer_cod As Integer = objParametri.InData

        Gias_InizializzaCultura_DaParams(objParametri.Server, objParametri.Utenti)

        Try
            r = V_M.ColorazioneAutomatica_2022(TipologiaLayer_cod,
                                               objParametri.Server)

        Catch ex As Exception

            r.RispostaOK = False
            r.RispostaStringa = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r

    End Function

    ''' <summary>
    ''' Ritorna il CodiceFiscaleTecnico aggiornato leggendolo da DB in Utenti_xGruppi_Utente
    ''' </summary>
    ''' <param name="Codice_Fiscale_Tecnico"></param>
    ''' <returns>CodiceFiscaleTecnico (in RispostaStringa)</returns>
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Get_CodiceFiscaleTecnico(ByVal InData As Object) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri = DeserializzaInData(Of String)(InData)
        Dim CodiceFiscaleTecnico As String = objParametri.InData

        Gias_InizializzaCultura_DaParams(objParametri.Server, objParametri.Utenti)

        Try
            r = V_M.Get_CodiceFiscaleTecnico_2022(CodiceFiscaleTecnico,
                                                  objParametri.Server,
                                                  objParametri.Utenti)

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r

    End Function

    ''' <summary>
    ''' Restituisce la ChiaveAlbero come oggetto ChiaveAlbero dato l'Id_Agenda
    ''' </summary>
    ''' <param name="Id_Agenda"></param>
    ''' <returns>obj ChiaveAlbero</returns>
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function GetChiave_ID_Agenda_Selezionato(ByVal InData As Object) As rispostaStandard(Of ChiaveAlbero)
        Dim r As New rispostaStandard(Of ChiaveAlbero)

        Dim objParametri = DeserializzaInData(Of String)(InData)
        Dim Id_Agenda As String = objParametri.InData

        Gias_InizializzaCultura_DaParams(objParametri.Server, objParametri.Utenti)

        Try
            r = V_M.GetChiave_ID_Agenda_Selezionato_2022(Id_Agenda,
                                                         objParametri.Server)

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r

    End Function

    ''' <summary>
    ''' Ritorna l'elenco delle operazioni di agenda disposte per l'utente corrente
    ''' </summary>
    ''' <returns>Lista di obj GIS_Return_ElencoOpGraficabili contenenti operazione d'agenda</returns>
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function ElencoOperazioniAgendaGraficabili(ByVal InData As Object) As rispostaStandard(Of List(Of ObjInputHTML_Out))
        Dim r As New rispostaStandard(Of List(Of ObjInputHTML_Out))

        Dim objParametri = DeserializzaInData(Of String)(InData)

        Gias_InizializzaCultura_DaParams(objParametri.Server, objParametri.Utenti)

        Try
            r = V_M.ElencoOperazioniAgendaGraficabili_2022(objParametri.Server)

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r

    End Function

    ''' <summary>
    ''' Salva uno o più nuovi elementi MultiPoint e restituisce la lista di essi (sicuramente non pronta)
    ''' </summary>
    ''' <param name="ChiaveAlbero"></param>
    ''' <param name="hiddenPunti_M"></param>
    ''' <param name="DialogMultipointDes"></param>
    ''' <returns>obj SalvaNuovoMultiPoint_Out</returns>
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function SalvaNuovoMultipoint(ByVal InData As Object) As rispostaStandard(Of SalvaNuovoMultiPoint_Out)
        Dim r As New rispostaStandard(Of SalvaNuovoMultiPoint_Out)

        Dim objParametri = DeserializzaInData(Of SalvaNuovoMultiPoint_In)(InData)
        Dim objSalvaNuovoMultiPoint As SalvaNuovoMultiPoint_In = objParametri.InData

        Gias_InizializzaCultura_DaParams(objParametri.Server, objParametri.Utenti)

        Try
            r = V_M.SalvaNuovoMultipoint_2022(objSalvaNuovoMultiPoint,
                                              objParametri.Server)

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r

    End Function

    ''' <summary>
    ''' A seconda del caso, estrae App_Nome o Programmazione_Des dell'Appezzamento
    ''' </summary>
    ''' <param name="ChiaveAlbero"></param>
    ''' <returns>App_nome OR Programmazione_Des (in RispostaStandard)</returns>
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function CaricaAppNomeProgrammazione_des(ByVal InData As Object) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri = DeserializzaInData(Of ChiaveAlbero)(InData)
        Dim objChiaveAlbero As ChiaveAlbero = objParametri.InData

        Gias_InizializzaCultura_DaParams(objParametri.Server, objParametri.Utenti)

        Try
            r = V_M.CaricaAppNomeProgrammazione_des_2022(objChiaveAlbero,
                                                         objParametri.Server)

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r

    End Function

    ''' <summary>
    ''' Ritorna il metodo di sementi utilizzato
    ''' </summary>
    ''' <param name="Sementi"></param>
    ''' <param name="Sementi_MappaturaLibera"></param>
    ''' <returns>(gp.Mode()).ToString</returns>
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiGisPurpose(ByVal InData As Object) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri = DeserializzaInData(Of LeggiGisPurpose_In)(InData)
        Dim objLeggiGisPurpose As LeggiGisPurpose_In = objParametri.InData

        Try
            r = V_M.LeggiGisPurpose_2022(objLeggiGisPurpose)

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r

    End Function

    ''' <summary>
    ''' Verifica la cancellazione dell'entità tramite Entita_Cod
    ''' </summary>
    ''' <param name="Entita_Cod"></param>
    ''' <returns>Ritorna un bool a seconda della corretta esecuzione o meno</returns>
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function VerificaCancella(ByVal InData As Object) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri = DeserializzaInData(Of LeggiEntita_In)(InData)
        Dim objLeggiEntita_In As LeggiEntita_In = objParametri.InData

        Gias_InizializzaCultura_DaParams(objParametri.Server, objParametri.Utenti)

        Try
            r = V_M.VerificaCancella_2022(objLeggiEntita_In,
                                          objParametri.Server)

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r

    End Function

    ''' <summary>
    ''' Dato un poligono, ne calcola il rateo variabile e ne crea gli elementi di conseguenza
    ''' </summary>
    ''' <param name="ChiaveAlbero"></param>
    ''' <param name="DescrizioneDelPiano"></param>
    ''' <param name="cellsize"></param>
    ''' <param name="DataRiferimentoPerLetturaDatiSentinel"></param>
    ''' <param name="oCfgLetturaPF"></param>
    ''' <returns>Ritorna il numero degli elementi memorizzati</returns>
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function pfRateoSrv(ByVal InData As Object) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri = DeserializzaInData(Of PfRateoSrv_In)(InData)
        Dim objPfRateoSrv As PfRateoSrv_In = objParametri.InData

        Gias_InizializzaCultura_DaParams(objParametri.Server, objParametri.Utenti)

        Try
            Dim isGestioneLegacy As Boolean = False
            Dim utentiImpostazioni As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read

            Dim listaImpostazioni = utentiImpostazioni.Leggi(TipiEnumerativi.enum_Impostazioni_Utenti.UTENTE_GIS,
                                                         1,
                                                         AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                         "",
                                                         "",
                                                         objParametri.Utenti)

            If listaImpostazioni.Rows.Count > 0 Then

                Dim valori As String() = listaImpostazioni(0)("Impostazione_Valore_1").Split("§")

                Dim gestioneLegacyParam = valori.Where(Function(s) s.StartsWith("ckGestioneAnalisiMappeLegacy"))

                If gestioneLegacyParam.Count > 0 Then
                    isGestioneLegacy = CBool(gestioneLegacyParam.FirstOrDefault.Split("^")(1))
                End If
            Else
                Throw New Exception("Errore nella lettura dell'impostazione utente.")
            End If

            If objPfRateoSrv.DataRiferimento_LetturaDatiSentinel = AGRODATAINIZIO Then
                objPfRateoSrv.DataRiferimento_LetturaDatiSentinel = DateTime.UtcNow
            End If

            If isGestioneLegacy Or (objPfRateoSrv.DataRiferimento_LetturaDatiSentinel.CompareTo(CostantiPersonalizzate.AGRODATAINIZIO) = 0) Then

                r = V_M.pfRateo_2022(objPfRateoSrv,
                                 objParametri.Server,
                                 objParametri.Utenti,
                                 objParametri.Super_Server)
            Else
                Dim xCFGr_biz As New AgronicaCoreVarieBIZ.Configurazione_Siti_BIZ_R
                Dim xConfig As New AgronicaCoreVarieDAL.Configurazione_Siti_R

                Dim Codice_Fiscale_Tecnico As String = ""

                Dim objUtentiDAL As New AgronicaCoreUtentiDAL.Utenti_xGruppi_Utente_R
                Codice_Fiscale_Tecnico = objUtentiDAL.Leggi_IdentificativoGruppoUtenti_Singolo(objParametri.Utenti.UtenteUsername,
                                                                                               objParametri.Utenti)

                Dim xWrite As New AgronicaCorePrecisionFarmingBIZ.PianoConcimazioneRateoVariabile_W

                Dim dt = xConfig.Leggi(0, MappePrescrizione_IsActive_ConfKey, "", "", objParametri.Server)

                Dim isMappePrescrizioneEngineActive = Not dt Is Nothing And dt.Rows.Count > 0 And
                                  dt.Rows(0)("Valore").ToString().ToLower() = "true"
                If isMappePrescrizioneEngineActive Then
                    r.RispostaOK = xWrite.ElaboraRateoConEngineMappePrescrizione(objPfRateoSrv,
                                     Codice_Fiscale_Tecnico,
                                     objParametri.Server,
                                     objParametri.Utenti,
                                     objParametri.Super_Server)

                    If Not r.RispostaOK Then
                        Throw New Exception("Errore nell'elaborazione del piano di concimazione via il nuovo Engine Mappe.")
                    End If

                    r.RispostaStringa = "Richiesta accodata con successo ai nuovi Engine."
                ElseIf xCFGr_biz.LeggiSeUsareWSMappe2024oGEE(objParametri.Server) Then
                    r.RispostaOK = xWrite.ElaboraRateoConWSMappe(objPfRateoSrv,
                                     Codice_Fiscale_Tecnico,
                                     objParametri.Server,
                                     objParametri.Utenti,
                                     objParametri.Super_Server)

                    If Not r.RispostaOK Then
                        Throw New Exception("Errore nell'elaborazione del piano di concimazione via WS_Mappe_2024.")
                    End If

                    r.RispostaStringa = "Richiesta accodata con successo."
                Else
                    r.RispostaOK = xWrite.ElaboraRateoConGEE(objPfRateoSrv,
                                                         Codice_Fiscale_Tecnico,
                                                         objParametri.Server,
                                                         objParametri.Utenti,
                                                         objParametri.Super_Server)

                    If Not r.RispostaOK Then
                        Throw New Exception("Errore nell'elaborazione del piano di concimazione via Google Earth Engine.")
                    End If

                    r.RispostaStringa = "Richiesta accodata con successo."
                End If
            End If


        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r

    End Function

    <WebMethod()>
    Public Function pfIndiceRischioSrv(ByVal InData As Object) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri = DeserializzaInData(Of pfIndiciRischio_In)(InData)
        Dim objPfIndiceRischioSrv As pfIndiciRischio_In = objParametri.InData

        Gias_InizializzaCultura_DaParams(objParametri.Server, objParametri.Utenti)
        Try
            Dim Codice_Fiscale_Tecnico As String = ""

            Dim objUtentiDAL As New AgronicaCoreUtentiDAL.Utenti_xGruppi_Utente_R
            Codice_Fiscale_Tecnico = objUtentiDAL.Leggi_IdentificativoGruppoUtenti_Singolo(objParametri.Utenti.UtenteUsername,
                                                                                               objParametri.Utenti)

            Dim xWrite As New AgronicaCorePrecisionFarmingBIZ.IndiciDiRischio_W
            r.RispostaOK = xWrite.ElaboraRischioConWSMappe(objPfIndiceRischioSrv,
                                     Codice_Fiscale_Tecnico,
                                     objParametri.Server,
                                     objParametri.Utenti,
                                     objParametri.Super_Server)

            If Not r.RispostaOK Then
                Throw New Exception("Errore nell'elaborazione del piano di concimazione via WS_Mappe_2024.")
            End If

            r.RispostaStringa = "Richiesta eseguita con successo."

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r
    End Function

    <WebMethod()>
    Public Function pfPrescriptionUpload(ByVal InData As Object) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri = DeserializzaInData(Of pfPrescriptionUploadCoreWS_In)(InData)
        Dim objpfPrescriptionUpload As pfPrescriptionUploadCoreWS_In = objParametri.InData

        Gias_InizializzaCultura_DaParams(objParametri.Server, objParametri.Utenti)
        Try
            Dim xConfig As New AgronicaCoreVarieDAL.Configurazione_Siti_R
            Dim dt = xConfig.Leggi(0, MappePrescrizione_IsActive_ConfKey, "", "", objParametri.Server)

            Dim isMappePrescrizioneEngineActive = Not dt Is Nothing And dt.Rows.Count > 0 And
                                                  dt.Rows(0)("Valore").ToString().ToLower() = "true"
            If isMappePrescrizioneEngineActive Then
                Dim outputAllegatiCod = 0
                ManageFileMappePrescrizione(
                    objpfPrescriptionUpload.fileName,
                    objpfPrescriptionUpload.ChiaveAlbero.piva,
                    objpfPrescriptionUpload.ChiaveAlbero.Sa_Cod,
                    objpfPrescriptionUpload.ChiaveAlbero.appezza,
                    objpfPrescriptionUpload.ChiaveAlbero.Id_Imp,
                    objpfPrescriptionUpload.ChiaveAlbero.RicettaOperazione_Cod,
                    objpfPrescriptionUpload.fileContent,
                    objParametri.Server,
                    objParametri.Super_Server,
                    outputAllegatiCod
                    )
                    
                If outputAllegatiCod < 0 Then
                    Throw New Exception("Errore in memorizzazione geojson su tabella allegati_documenti")
                End If
                
                r.RispostaOK = True
                r.RispostaStringa = "Mappa di prescrizione caricata"
            Else
                r = V_M.pfPrescriptionUpload_2022(objpfPrescriptionUpload,
                                                  objParametri.Server,
                                                  objParametri.Utenti,
                                                  objParametri.Super_Server)
            End If
        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r
    End Function

    <WebMethod()>
    Public Function pfPrescriptionUpdate(ByVal InData As Object) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri = DeserializzaInData(Of pfPrescriptionUpdate_In)(InData)
        Dim objpfPrescriptionUpdate As pfPrescriptionUpdate_In = objParametri.InData

        Gias_InizializzaCultura_DaParams(objParametri.Server, objParametri.Utenti)
        Try
            Dim xConfig As New AgronicaCoreVarieDAL.Configurazione_Siti_R
            Dim dt = xConfig.Leggi(0, MappePrescrizione_IsActive_ConfKey, "", "", objParametri.Server)

            Dim isMappePrescrizioneEngineActive = Not dt Is Nothing And dt.Rows.Count > 0 And
                                                  dt.Rows(0)("Valore").ToString().ToLower() = "true"
            r = V_M.pfPrescriptionUpdate_2022(objpfPrescriptionUpdate,
                                              objParametri.Server,
                                              objParametri.Utenti,
                                              objParametri.Super_Server,
                                              isMappePrescrizioneEngineActive)
        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r
    End Function

    ''' <summary>
    ''' Restituisce l'immagine PNG della mappa di prescrizione codificata in Base64
    ''' </summary>
    ''' <returns>Stringa Base64 dell'immagine PNG</returns>
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function MappaPrescrizioneImage(ByVal InData As Object) As rispostaStandard(Of String)
        Dim r As New rispostaStandard(Of String)

        Dim objParametri = DeserializzaInData(Of MappaPrescrizioneImage_In)(InData)
        Dim objMappaPrescrizioneImage As MappaPrescrizioneImage_In = objParametri.InData

        Gias_InizializzaCultura_DaParams(objParametri.Server, objParametri.Utenti)

        Try
            Dim allegatiLeggi As New AgronicaCoreAnagrafeDAL.Allegati_Documenti_R
            Dim dtAllegato = allegatiLeggi.Leggi(
                objMappaPrescrizioneImage.AllegatoCod,
                AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                "",
                "",
                objParametri.Server)

            If dtAllegato Is Nothing OrElse dtAllegato.Rows.Count = 0 Then
                Throw New Exception("Nessun allegato trovato per AllegatoCod = " & objMappaPrescrizioneImage.AllegatoCod)
            End If

            Dim riga = dtAllegato.Rows(0)
            Dim sottoCartella As String = riga("Sottocartella").ToString()
            Dim nomeFile As String = riga("Allegati_Documenti_NomeFile").ToString()

            Dim agroWebConfig As New AgronicaCoreGestioneRichieste.AgroWebConfig
            agroWebConfig.impostaDaDB(objParametri.Super_Server, objParametri.Server)
            Dim percorsoFile As String = Path.Combine(
                agroWebConfig.GestioneAllegati_Repository.TrimEnd("\"),
                sottoCartella,
                nomeFile)

            If Not File.Exists(percorsoFile) Then
                Throw New FileNotFoundException("File raster non trovato: " & percorsoFile)
            End If

            r.RispostaStringa = GisRasterUtility.RasterToPngBase64(percorsoFile, ColorRamp.GreenToRed)
            r.RispostaOK = True
        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r
    End Function

    ''' <summary>
    ''' Restituisce l'immagine PNG della mappa di prescrizione e il bounding box WGS84
    ''' necessari per renderizzare un GroundOverlay su Google Maps SDK.
    ''' Il PNG viene generato tramite GDAL se non già presente su disco, altrimenti viene letto dalla cache.
    ''' </summary>
    ''' <returns>Oggetto MappaPrescrizioneGroundOverlay_Out con PngBase64 e Bounds</returns>
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function MappaPrescrizioneGroundOverlay(ByVal InData As Object) As rispostaStandard(Of MappaPrescrizioneGroundOverlay_Out)
        Dim r As New rispostaStandard(Of MappaPrescrizioneGroundOverlay_Out)

        Dim objParametri = DeserializzaInData(Of Integer)(InData)
        Dim allegatoCod As Integer = objParametri.InData

        Gias_InizializzaCultura_DaParams(objParametri.Server, objParametri.Utenti)

        Try
            Dim allegatiLeggi As New AgronicaCoreAnagrafeDAL.Allegati_Documenti_R
            Dim dtAllegato = allegatiLeggi.Leggi(
                allegatoCod,
                AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                "",
                "",
                objParametri.Server)

            If dtAllegato Is Nothing OrElse dtAllegato.Rows.Count = 0 Then
                Throw New Exception("Nessun allegato trovato per AllegatoCod = " & allegatoCod)
            End If

            Dim riga = dtAllegato.Rows(0)
            Dim sottoCartella As String = riga("Sottocartella").ToString()
            Dim nomeFile As String = riga("Allegati_Documenti_NomeFile").ToString()

            Dim agroWebConfig As New AgronicaCoreGestioneRichieste.AgroWebConfig
            agroWebConfig.impostaDaDB(objParametri.Super_Server, objParametri.Server)
            Dim percorsoFile As String = Path.Combine(
                agroWebConfig.GestioneAllegati_Repository.TrimEnd("\"),
                sottoCartella,
                nomeFile)

            If Not File.Exists(percorsoFile) Then
                Throw New FileNotFoundException("File raster non trovato: " & percorsoFile)
            End If

            Dim pngBase64 As String = GisRasterUtility.RasterToPngBase64(percorsoFile, ColorRamp.GreenToRed)

            Dim bounds As RasterBounds = GisRasterUtility.GetRasterBounds(percorsoFile)

            r.RispostaStringa = New MappaPrescrizioneGroundOverlay_Out() With {
                .PngBase64 = pngBase64,
                .Bounds = bounds
            }
            r.RispostaOK = True
        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r
    End Function

    ''' <summary>
    ''' Legge la posizione del Centro richiesto
    ''' </summary>
    ''' <param name="Piva"></param>
    ''' <param name="Sa_Cod"></param>
    ''' <returns>Ritorna la posizione del Centro come obj CoordsFromViaCentro_Out</returns>
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function CoordinateFromViaCentro(ByVal InData As Object) As rispostaStandard(Of CoordsFromViaCentro_Out)
        Dim r As New rispostaStandard(Of CoordsFromViaCentro_Out)

        Dim objParametri = DeserializzaInData(Of CoordinateFromImpresa_In)(InData)
        Dim objCoordFromImpresa As CoordinateFromImpresa_In = objParametri.InData

        Gias_InizializzaCultura_DaParams(objParametri.Server, objParametri.Utenti)

        Try
            r = V_M.CoordinateFromViaCentro_2022(objCoordFromImpresa,
                                                 objParametri.Server)

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function

    ''' <summary>
    ''' Esegue un controllo sull'esistenza dell'elemento Catasto passato, identificato per ChiaveAlbero
    ''' </summary>
    ''' <param name="ChiaveAlbero"></param>
    ''' <returns>Restituisce un True/False a seconda della corretta esecuzione</returns>
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function ControllaSeEsisteCatasto(ByVal InData As Object) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri = DeserializzaInData(Of LeggiEntita_In)(InData)
        Dim ChiaveAlbero As ChiaveAlbero = objParametri.InData.ChiaveAlbero

        Gias_InizializzaCultura_DaParams(objParametri.Server, objParametri.Utenti)

        Try
            r = V_M.ControllaSeEsisteCatasto_2022(ChiaveAlbero,
                                                  objParametri.Server)

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function

    ''' <summary>
    ''' Ritorna una tabella degli elementi grafici presenti come obj Caricaplace_TabHTML_Out
    ''' </summary>
    ''' <param name="Layer_ElementiGrafici_Cod"></param>
    ''' <returns>Restituisce un obj Caricaplace_TabHTML_Out</returns>
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Caricaplace_tabella_Dettagli(ByVal InData As Object) As rispostaStandard(Of Caricaplace_TabHTML_Out)
        Dim r As New rispostaStandard(Of Caricaplace_TabHTML_Out)

        Dim objParametri = DeserializzaInData(Of String)(InData)
        Dim LayerElementiGrafici_Cod As Integer = objParametri.InData

        Gias_InizializzaCultura_DaParams(objParametri.Server, objParametri.Utenti)

        Try
            r = V_M.Caricaplace_tabella_Dettagli_2022(LayerElementiGrafici_Cod,
                                                      objParametri.Server)

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function

    ''' <summary>
    ''' Ritorna una tabella degli elementi grafici presenti (Kendo DataTable)
    ''' </summary>
    ''' <param name="Layer_ElementiGrafici_Cod"></param>
    ''' <returns>js.JSON_DataTable_Kendo() (in RispostaStandard)</returns>
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Caricaplace_tabella_Dettagli2(ByVal InData As Object) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri = DeserializzaInData(Of String)(InData)
        Dim LayerElementiGrafici_Cod As Integer = objParametri.InData

        Gias_InizializzaCultura_DaParams(objParametri.Server, objParametri.Utenti)

        Try
            r = V_M.Caricaplace_tabella_Dettagli2_2022(LayerElementiGrafici_Cod,
                                                       objParametri.Server)

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function

    ''' <summary>
    ''' Popola la DDL Tipologia_Layer da DB
    ''' </summary>
    ''' <returns>Elenco di tag option per DDL Tipologia_Layer</returns>
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Carica_ddlTipologiaLayer(ByVal InData As Object) As rispostaStandard(Of List(Of ObjOptionHTML_Out))
        Dim r As New rispostaStandard(Of List(Of ObjOptionHTML_Out))

        Dim objParametri = DeserializzaInData(Of String)(InData)

        Gias_InizializzaCultura_DaParams(objParametri.Server, objParametri.Utenti)

        Try
            r = V_M.Carica_ddlTipologiaLayer_2022(objParametri.Server)

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function

    ''' <summary>
    ''' Controllo sulle operazioni d'agenda possibili
    ''' </summary>
    ''' <param name="ChiaveAlbero"></param>
    ''' <returns>True/False</returns>
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function ControllaSeEsistonoOperazioni(ByVal InData As Object) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri = DeserializzaInData(Of ChiaveAlbero)(InData)
        Dim ChiaveAlbero As ChiaveAlbero = objParametri.InData

        Gias_InizializzaCultura_DaParams(objParametri.Server, objParametri.Utenti)

        Try
            r = V_M.ControllaSeEsistonoOperazioni_2022(ChiaveAlbero,
                                                       objParametri.Server)

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function

    ''' <summary>
    ''' Ritorna le proprietà di un'entità del GIS (appezzamento, ricetta, catasto...) cercando
    ''' per Entita_Cod
    ''' </summary>
    ''' <param name="Entita_Cod"></param>
    ''' <param name="Tipo_GetProp"></param>
    ''' <returns>Al momento ritorna una stringa contenente l'obj JSON</returns>
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function GetProprieta(ByVal InData As Object) As rispostaStandard(Of GetProprieta_Out)
        Dim r As New rispostaStandard(Of GetProprieta_Out)

        Dim objParametri = DeserializzaInData(Of GetProprieta_In)(InData)
        Dim obj_GetProp As GetProprieta_In = objParametri.InData

        Gias_InizializzaCultura_DaParams(objParametri.Server, objParametri.Utenti)

        Try
            r = V_M.GetProprieta_2022(obj_GetProp,
                                      objParametri.Server,
                                      objParametri.Utenti)

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function

    ''' <summary>
    ''' Ritorna un obj contenente le liste di Codici riguardanti Ricette e AllegatiDoc
    ''' </summary>
    ''' <param name="Lista_RicettaOp_Cod"></param>
    ''' <param name="TipoCodici"></param>
    ''' <returns>obj Lista_DatiPrecision_XmlAllegati_Out</returns>
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function CaricaDatiPrecisionXmlDaAllegati(ByVal InData As Object) As rispostaStandard(Of AgronicaCoreModelsSTD.Gis.GisDataReadRval_New(Of AgronicaCoreModelsSTD.Gis.GeoJSONAgroGisProp))
        Dim r As New rispostaStandard(Of AgronicaCoreModelsSTD.Gis.GisDataReadRval_New(Of AgronicaCoreModelsSTD.Gis.GeoJSONAgroGisProp))

        Dim objParametri = DeserializzaInData(Of CaricaDatiPrecision_In)(InData)
        Dim obj_CaricaDatiPrecision As CaricaDatiPrecision_In = objParametri.InData

        Gias_InizializzaCultura_DaParams(objParametri.Server, objParametri.Utenti)

        Try
            r = V_M.CaricaDatiPrecisionXmlDaAllegati_2022(obj_CaricaDatiPrecision,
                                                          objParametri.Server,
                                                          objParametri.Utenti)

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf _
                & AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function

    ''' <summary>
    ''' Verifica la vicinanza con il poligono selezionato 
    ''' (può essere chiamata come VerificaInterferenze o CampoPiuVicino)
    ''' </summary>
    ''' <param name="Veg_Cod"></param>
    ''' <param name="Grva_Cod"></param>
    ''' <param name="Data_Inizio"></param>
    ''' <param name="Data_Fine"></param>
    ''' <param name="HiddenPunti_Nuovo"></param>
    ''' <param name="CampoPiuVicino"></param>
    ''' <param name="Sementi"></param>
    ''' <param name="SementiMappaturaLibera"></param>
    ''' <returns>Obj VerificaInterferenze_Out</returns>
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function VerificaInterferenze(ByVal InData As Object) As rispostaStandard(Of VerificaInterferenze_Out)
        Dim r As New rispostaStandard(Of VerificaInterferenze_Out)

        Dim objParametri = DeserializzaInData(Of VerificaInterferenze_In)(InData)
        Dim obj_VerificaInterferenze As VerificaInterferenze_In = objParametri.InData

        Gias_InizializzaCultura_DaParams(objParametri.Server, objParametri.Utenti)

        Try
            r = V_M.VerificaInterferenze_2022(obj_VerificaInterferenze,
                                              objParametri.Server,
                                              objParametri.Utenti)

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function

    ''' <summary>
    ''' Restituisce le entità vicine a quella passata, utilizzando (VerificaVicini) o meno (VerificaVicini_2) Entita_Cod
    ''' </summary>
    ''' <param name="Veg_Cod"></param>
    ''' <param name="Grva_Cod"></param>
    ''' <param name="Data_Inizio"></param>
    ''' <param name="Data_Fine"></param>
    ''' <param name="HiddenPunti_Nuovo/Modifica"></param>
    ''' <param name="Entita_Cod"></param>
    ''' <param name="Sementi"></param>
    ''' <param name="SementiMappaturaLibera"></param>
    ''' <returns>Restituisce un obj VerificaVicini_Out</returns>
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function VerificaVicini(ByVal InData As Object) As rispostaStandard(Of VerificaVicini_Out)
        Dim r As New rispostaStandard(Of VerificaVicini_Out)

        Dim objParametri = DeserializzaInData(Of VerificaVicini_In)(InData)
        Dim obj_VerificaVicini As VerificaVicini_In = objParametri.InData

        Gias_InizializzaCultura_DaParams(objParametri.Server, objParametri.Utenti)

        Try
            r = V_M.VerificaVicini_2022(obj_VerificaVicini,
                                        objParametri.Server,
                                        objParametri.Utenti)

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function

    ''' <summary>
    ''' Restituisce i dati sulle Entità tracciate come interferenze
    ''' </summary>
    ''' <param name="Codice_Fiscale_Tecnico"></param>
    ''' <param name="Veg_Cod"></param>
    ''' <param name="Grva_Cod"></param>
    ''' <param name="Data_Inizio"></param>
    ''' <param name="Data_Fine"></param>
    ''' <param name="Sementi"></param>
    ''' <param name="SementiMappaturaLibera"></param>
    ''' <param name="HiddenPunti"></param>
    ''' <param name="Entita_Cod"></param>
    ''' <returns>Restituisce una lista delle interferenze come lista di obj Entita_Info_Out</returns>
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function InfoInterferenze(ByVal InData As Object) As rispostaStandard(Of List(Of Entita_Info_Out))
        Dim r As New rispostaStandard(Of List(Of Entita_Info_Out))

        Dim objParametri = DeserializzaInData(Of InfoInterferenze_In)(InData)
        Dim obj_InfoInterferenze As InfoInterferenze_In = objParametri.InData

        Gias_InizializzaCultura_DaParams(objParametri.Server, objParametri.Utenti)

        Try
            r = V_M.InfoInterferenze_2022(obj_InfoInterferenze,
                                          objParametri.Server,
                                          objParametri.Utenti)

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function

    ''' <summary>
    ''' Cancella una o più entità del GIS
    ''' </summary>
    ''' <param name="Entita_Cod"></param>
    ''' <param name="Sementi"></param>
    ''' <param name="SementiMappaturaLibera"></param>
    ''' <param name="DatiPassaggio"></param>
    ''' <param name="ASG_Utente_Username"></param>
    ''' <param name="ASG_IdServizio"></param>
    ''' <param name="Elimina_Grafica"></param>
    ''' <param name="Elimina_Impianto"></param>
    ''' <param name="Elimina_PrecisionFarming"></param>
    ''' <param name="Elimina_PrecisionFarmingABLine"></param>
    ''' <param name="Elimina_DatoGiasPalm"></param>
    ''' <returns>Messaggio di esito</returns>
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Cancella(ByVal InData As Object) As rispostaStandard(Of Cancella_Out)
        Dim r As New rispostaStandard(Of Cancella_Out)

        Dim objParametri = DeserializzaInData(Of Cancella_In)(InData)
        Dim obj_Cancella As Cancella_In = objParametri.InData

        Gias_InizializzaCultura_DaParams(objParametri.Server, objParametri.Utenti)

        Try

            r = V_M.Cancella_2022(obj_Cancella,
                                  objParametri.Server,
                                  objParametri.Utenti)

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.ErroriGias = New List(Of ErroreGias) From {Gestione_Eccezioni_2015.ErrorHandler(ex)}

        End Try

        Return r

    End Function

    ''' <summary>
    ''' Cancella una lista di Entita_Grafiche nel GIS con una lista di Entita_Cod
    ''' </summary>
    ''' <param name="Lista_Entita_Cod">List(Of Entita_Cod)</param>
    ''' <returns>Numero entita eliminate e non (obj Cancella_EntitaGraf_Out)</returns>
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function CancellaEntitaGrafiche(ByVal InData As Object) As rispostaStandard(Of Cancella_EntitaGraf_Out)
        Dim r As New rispostaStandard(Of Cancella_EntitaGraf_Out)

        Dim objParametri = DeserializzaInData(Of List(Of String))(InData)
        Dim lista_Entita_Cod As List(Of String) = objParametri.InData

        Gias_InizializzaCultura_DaParams(objParametri.Server, objParametri.Utenti)

        Try
            r = V_M.CancellaEntitaGrafiche_2022(lista_Entita_Cod,
                                                objParametri.Server,
                                                objParametri.Utenti)

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function

    ''' <summary>
    ''' Cancella un elenco di hiddenPunti (entita grafiche del GIS)
    ''' </summary>
    ''' <param name="lista_Entita_Cod">List(Of Entita_Cod)</param>
    ''' <returns>Messaggio con numero elementi cancellati</returns>
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function CancellaMultipoint(ByVal InData As Object) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri = DeserializzaInData(Of List(Of String))(InData)
        Dim Lista_HiddenPunti As List(Of String) = objParametri.InData

        Gias_InizializzaCultura_DaParams(objParametri.Server, objParametri.Utenti)

        'ATTENZIONE: Questa funzione si comporta esattamente come CancellaEntitaGrafiche poichè richiama la funzione generale presente nel V_M
        'Cancella() (la versione senza Session) passandogli esattamente gli stessi params (cicla una Lista di Entita_Cod, il resto li inizializza uguale):
        'la differenza che si potrebbe aggiustare è tenerle separate poichè sul finale fanno diverse operazioni ma nei parametri passati utilizzare
        'semplicemente una Lista di Entita_Cod invece di uno stringone da Splittare

        Try
            r = V_M.CancellaMultipoint_2022(Lista_HiddenPunti,
                                            objParametri.Server,
                                            objParametri.Utenti)

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function

    ''' <summary>
    ''' Verifica che il test sull'orientamento del poligono vada a buon fine
    ''' </summary>
    ''' <param name="Poligono"></param>
    ''' <returns>Messaggio di esito</returns>
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function VerificaPoligono(ByVal InData As Object) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri = DeserializzaInData(Of String)(InData)
        Dim Poligono As String = objParametri.InData

        Gias_InizializzaCultura_DaParams(objParametri.Server, objParametri.Utenti)

        Try
            r = V_M.VerificaPoligono_2022(Poligono,
                                          objParametri.Server)

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function

    ''' <summary>
    ''' Esegue un salvataggio di un dato cartografico nel GIS
    ''' </summary>
    ''' <param name="ChiaveAlbero"></param>
    ''' <param name="hiddenPunti_A"></param>
    ''' <param name="hiddenPunti_B"></param>
    ''' <param name="hiddenPunti_AB"></param>
    ''' <returns>obj SalvaNuovoAB_Out</returns>
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function SalvaNuovoAB(ByVal InData As Object) As rispostaStandard(Of SalvaNuovoAB_Out)
        Dim r As New rispostaStandard(Of SalvaNuovoAB_Out)

        Dim objParametri = DeserializzaInData(Of SalvaNuovoAB_In)(InData)
        Dim obj_SalvaNuovoAB As SalvaNuovoAB_In = objParametri.InData

        Gias_InizializzaCultura_DaParams(objParametri.Server, objParametri.Utenti)

        Try
            r = V_M.SalvaNuovoAB_2022(obj_SalvaNuovoAB,
                                      objParametri.Server,
                                      objParametri.Utenti)

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function

    ''' <summary>
    ''' Restituisce Validita_Inizio e Validita_Fine
    ''' </summary>
    ''' <param name="Sementi"></param>
    ''' <param name="SementiMappaturaLibera"></param>
    ''' <returns>Restituisce Validita Inizio e Fine (come obj DateDefault_Out)</returns>
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function CaricaDateDefault(ByVal InData As Object) As rispostaStandard(Of DateDefault_Out)
        Dim r As New rispostaStandard(Of DateDefault_Out)

        Dim objParametri = DeserializzaInData(Of CaricaDate_Default_In)(InData)
        Dim obj_CaricaDate_Def As CaricaDate_Default_In = objParametri.InData

        Gias_InizializzaCultura_DaParams(objParametri.Server, objParametri.Utenti)

        Try
            r = V_M.CaricaDateDefault_2022(obj_CaricaDate_Def,
                                           objParametri.Server)

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function

    ''' <summary>
    ''' Esegue un controllo delle specie permesse
    ''' </summary>
    ''' <param name="SementiMappaturaLibera"></param>
    ''' <param name="Veg_Cod"></param>
    ''' <param name="Finalita"></param>
    ''' <returns>True/False</returns>
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function SementiMappaturaLiberaSpecieVegetalePermessa(ByVal InData As Object) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri = DeserializzaInData(Of SementiMappaturaLibera_SpecieVegPermessa_In)(InData)
        'ATTENZIONE: Al momento (compresa la funzione originale) data_inizio e data_fine non vengono usate mai
        Dim obj_SML_SpecieVegPerm As SementiMappaturaLibera_SpecieVegPermessa_In = objParametri.InData

        Gias_InizializzaCultura_DaParams(objParametri.Server, objParametri.Utenti)

        Try
            r = V_M.SementiMappaturaLiberaSpecieVegetalePermessa_2022(obj_SML_SpecieVegPerm,
                                                                      objParametri.Server)

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function

    ''' <summary>
    ''' Controlla permessi utente per poter disegnare elemento sementi
    ''' </summary>
    ''' <param name="DatiPassaggio"></param>
    ''' <returns>Messaggio esito</returns>
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function SementiSportelloPossoDisegnare(ByVal InData As Object) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri = DeserializzaInData(Of String)(InData)
        Dim DatiPassaggio As String = objParametri.InData

        Gias_InizializzaCultura_DaParams(objParametri.Server, objParametri.Utenti)

        Try
            r = V_M.SementiSportelloPossoDisegnare_2022(DatiPassaggio)

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function

    ''' <summary>
    ''' Inizializza il calendario base partendo dalla EntitaCod
    ''' </summary>
    ''' <param name="EntitaCod"></param>
    ''' <returns></returns>
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function CustomMapOverlayBaseEntitaGIS_InizializzaCalendario(ByVal InData As Object) As rispostaStandard(Of Lista_GisSat_SentinelOverlay_Out)
        Dim objParametri = DeserializzaInData(Of Integer)(InData)

        Gias_InizializzaCultura_DaParams(objParametri.Server, objParametri.Utenti)

        Dim resp As New rispostaStandard(Of Lista_GisSat_SentinelOverlay_Out)
        Dim xReadConfigurazione As New AgronicaCoreImpostazioniUtenteBIZ.Impostazioni_Utente_R

        Try
            Dim ObjUtenti As New AgronicaCoreUtentiDAL.Utenti_Permessi_R

            Dim permessiOperazione As Boolean = ObjUtenti.Controlla_Permessi_Utente(
                objParametri.Utenti.UtenteUsername,
                enum_Id_Servizio.GiasOnline,
                enum_Security_Attivita.GIS_SAT_AnalisiDatiSatellitari,
                enum_Security_Operazione.Modifica,
                Date.Now,
                "",
                objParametri.Utenti
                )

            If Not permessiOperazione Then
                Throw New Exception("INVALID_USER")
            End If

            Dim gestioneLegacy As Boolean = False
            Dim impostazioniDaSalvare As String = "" ' Variabile per la riscrittura

            Dim configurazioniUtente = xReadConfigurazione.LeggiImpostazioniUtente(enum_Impostazioni_Utenti.UTENTE_GIS, objParametri.Utenti, 1)

            If Not String.IsNullOrEmpty(configurazioniUtente) Then
                Dim impostazioneMappeSatellitari = configurazioniUtente.Split("§").Where(Function(s) s.StartsWith(ParametriUtenteGis.ckGestioneAnalisiMappeLegacy))

                If impostazioneMappeSatellitari.Count > 0 Then
                    gestioneLegacy = CBool(impostazioneMappeSatellitari.FirstOrDefault.Split("^")(1))
                End If
            Else
                ' L'impostazione non esiste, impostiamo il default a False e facciamo la riscrittura.
                Dim newLegacySetting As String = ParametriUtenteGis.ckGestioneAnalisiMappeLegacy & "^" & gestioneLegacy.ToString()

                Dim utentiImpostazioni_W As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_W
                utentiImpostazioni_W.Modifica(
                    enum_Impostazioni_Utenti.UTENTE_GIS,
                    configurazioniUtente & "§" & newLegacySetting,
                    "",
                    "",
                    "",
                    AGRODATAINIZIO,
                    AGRODATAFINE,
                    objParametri.Utenti,
                    False
                    )
            End If

            Dim objElGrafico As New AgronicaCoreGisDAL.GIS_ElementiGrafici_R
            Dim dt = objElGrafico.Leggi(
                objParametri.Server.PivaSuperUser,
                0,
                objParametri.InData,
                TipiEnumerativi.enum_Gis_LayerElementiGrafici_std.IMPIANTI,
                AgronicaCoreParametri.enumFromatoCartograficoConvertito.WKT,
                AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                "",
                "",
                objParametri.Server,
                0,
                "ElementoGrafico_Cod , Entita_Cod"
                )

            If dt.Rows.Count <= 0 Then
                Throw New Exception(String.Format("Nessun poligono trovato per la entita ({0})", objParametri.InData))
            End If

            Dim wkt = dt.Rows(0)("geodata")
            If wkt = "" Then
                Throw New Exception(String.Format("Nessun poligono wkt trovato per la entita ({0})", objParametri.InData))
            End If

            If gestioneLegacy Then
                resp = V_M.CustomMapOverlayBase_InizializzaCalendario_2022(
                    wkt,
                    objParametri.Server,
                    objParametri.Utenti,
                    objParametri.Super_Server,
                    False
                    )
            Else
                Dim xReadEsecuzioniGEE As New AgronicaCoreGisBIZ.ProiezioniLayer_R

                resp.RispostaStringa = xReadEsecuzioniGEE.LeggiEsecuzioniAnalisiMappeSatellitari(wkt, objParametri.Server, objParametri.InData)
                resp.RispostaOK = True
            End If
        Catch ex As Exception

            resp.RispostaOK = False
            resp.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return resp
    End Function


    ''' <summary>
    ''' Inizializza il calendario base(?)
    ''' </summary>
    ''' <param name="PoligonoWKT"></param>
    ''' <returns></returns>
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function CustomMapOverlayBase_InizializzaCalendario(ByVal InData As Object) As rispostaStandard(Of Lista_GisSat_SentinelOverlay_Out)
        Dim objParametri = DeserializzaInData(Of String)(InData)

        Gias_InizializzaCultura_DaParams(objParametri.Server, objParametri.Utenti)

        Dim resp As New rispostaStandard(Of Lista_GisSat_SentinelOverlay_Out)
        Dim xReadConfigurazione As New AgronicaCoreImpostazioniUtenteBIZ.Impostazioni_Utente_R

        Try
            Dim ObjUtenti As New AgronicaCoreUtentiDAL.Utenti_Permessi_R

            Dim permessiOperazione As Boolean = ObjUtenti.Controlla_Permessi_Utente(objParametri.Utenti.UtenteUsername,
                                                                                    enum_Id_Servizio.GiasOnline,
                                                                                    enum_Security_Attivita.GIS_SAT_AnalisiDatiSatellitari,
                                                                                    enum_Security_Operazione.Modifica,
                                                                                    Date.Now,
                                                                                    "",
                                                                                    objParametri.Utenti)

            If Not permessiOperazione Then
                Throw New Exception("INVALID_USER")
            End If

            Dim configurazioniUtente = xReadConfigurazione.LeggiImpostazioniUtente(enum_Impostazioni_Utenti.UTENTE_GIS, objParametri.Utenti, 1)

            Dim impostazioneMappeSatellitari = configurazioniUtente.Split("§").Where(Function(s) s.StartsWith(ParametriUtenteGis.ckGestioneAnalisiMappeLegacy))

            Dim gestioneLegacy As Boolean = False

            If impostazioneMappeSatellitari.Count > 0 Then
                gestioneLegacy = CBool(impostazioneMappeSatellitari.FirstOrDefault.Split("^")(1))
            End If

            If gestioneLegacy Then
                resp = V_M.CustomMapOverlayBase_InizializzaCalendario_2022(objParametri.InData,
                                                                           objParametri.Server,
                                                                           objParametri.Utenti,
                                                                           objParametri.Super_Server,
                                                                           False)
            Else

                Dim xReadEsecuzioniGEE As New AgronicaCoreGisBIZ.ProiezioniLayer_R

                resp.RispostaStringa = xReadEsecuzioniGEE.LeggiEsecuzioniAnalisiMappeSatellitari(objParametri.InData, objParametri.Server)

                resp.RispostaOK = True

            End If
        Catch ex As Exception

            resp.RispostaOK = False
            resp.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return resp
    End Function

    ''' <summary>
    ''' Aggiorna elenco tipologie
    ''' </summary>
    ''' <returns></returns>
    '<WebMethod()>
    '<Script.Services.ScriptMethod()>
    'Public Function AggiornaElencoTipologie3(ByVal InData As Object) As rispostaStandard(Of ElencoTipologieLayer)
    '    Dim r As New rispostaStandard(Of ElencoTipologieLayer)

    '    Dim objParametri = DeserializzaInData(Of AggiornaElencoTipologie_In)(InData)

    '    Gias_InizializzaCultura_DaParams(objParametri.Server, objParametri.Utenti)

    '    Try
    '        r.RispostaStringa = V_M.AggiornaElencoTipologie3("http://www.opengis.net/gml",
    '                                                         objParametri.InData,
    '                                                         objParametri.Server,
    '                                                         objParametri.Utenti)

    '    Catch ex As Exception

    '        r.RispostaOK = False
    '        r.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

    '    End Try

    '    Return r

    'End Function

    ''' <summary>
    ''' Aggiorna elenco tipologie
    ''' </summary>
    ''' <returns></returns>
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function AggiornaElencoTipologie4(ByVal InData As Object) As rispostaStandard(Of ElencoTipologieLayer)
        Dim r As New rispostaStandard(Of ElencoTipologieLayer)

        Dim objParametri = DeserializzaInData(Of AggiornaElencoTipologie_In)(InData)

        Gias_InizializzaCultura_DaParams(objParametri.Server, objParametri.Utenti)

        Try
            r.RispostaStringa = V_M.AggiornaElencoTipologie4(objParametri.InData,
                                                             objParametri.Server,
                                                             objParametri.Utenti)

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function

    ''' <summary>
    ''' Salva i colori da applicare ai Layer
    ''' </summary>
    ''' <param name="Tipologia">tipo di salvataggio da eseguire</param>
    ''' <param name="Dati">lista dei dati di ogni layer</param>
    ''' <returns>"OK"</returns>
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function SalvaColoriLayer2(ByVal InData As Object) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri = DeserializzaInData(Of SalvaColoriLayer2_In)(InData)
        Dim obj_SalvaColoriLayer2 As SalvaColoriLayer2_In = objParametri.InData

        Gias_InizializzaCultura_DaParams(objParametri.Server, objParametri.Utenti)

        Try
            r = V_M.SalvaColoriLayer2_2022(obj_SalvaColoriLayer2, objParametri.Server)

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function

    ''' <summary>
    ''' Salva la visibilità dei layer
    ''' </summary>
    ''' <param name="TipologiaLayer_Cod">Indica la tipologia di appartenenza del layer</param>
    ''' <param name="DatiVisibilitaLayer">Lista dei dati di visibilità di ogni layer</param>
    ''' <returns>"OK"</returns>
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function SalvaVisibilitaLayer(ByVal InData As Object) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri = DeserializzaInData(Of SalvaVisibilitaLayer_In)(InData)
        Dim objSalvaVisibilitaLayer As SalvaVisibilitaLayer_In = objParametri.InData

        Gias_InizializzaCultura_DaParams(objParametri.Server, objParametri.Utenti)

        Try
            r = V_M.SalvaVisibilitaLayer(objSalvaVisibilitaLayer, objParametri.Server)

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function

    ''' <summary>
    ''' Salva i flag di visibilità dei layer
    ''' </summary>
    ''' <param name="TipologiaLayer_Cod">Indica la tipologia di appartenenza del layer</param>
    ''' <param name="DatiVisibilitaLayer">Lista dei dati di visibilità di ogni layer</param>
    ''' <returns>"OK"</returns>
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function SalvaFlagVisibilitaLayer(ByVal InData As Object) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri = DeserializzaInData(Of SalvaFlagVisibilitaLayer_In)(InData)
        Dim objSalvaFlagVisibilitaLayer As SalvaFlagVisibilitaLayer_In = objParametri.InData

        Gias_InizializzaCultura_DaParams(objParametri.Server, objParametri.Utenti)

        Try
            r = V_M.SalvaFlagVisibilitaLayer(objSalvaFlagVisibilitaLayer, objParametri.Server)

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function

    ''' <summary>
    ''' Scrittura nuovo layer personalizzato
    ''' </summary>
    ''' <param name="NomeLayer">Nome layer</param>
    ''' <param name="MostraDescrizioneAssociata">Indica se mostrare la descrizione associata al layer</param>
    ''' <returns>"OK"</returns>
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function ScriviNuovoLayerPersonalizzato(ByVal InData As Object) As rispostaStandard(Of ScriviNuovoLayerPersonalizzato_Out)

        Dim r As New rispostaStandard(Of ScriviNuovoLayerPersonalizzato_Out)

        Dim objParametri = DeserializzaInData(Of ScriviNuovoLayerPersonalizzato_In)(InData)
        Dim objScriviNuovoLayerPersonalizzato As ScriviNuovoLayerPersonalizzato_In = objParametri.InData

        Gias_InizializzaCultura_DaParams(objParametri.Server, objParametri.Utenti)

        Try
            r = V_M.ScriviNuovoLayerPersonalizzato(objScriviNuovoLayerPersonalizzato, objParametri.Server)

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function

    ''' <summary>
    ''' Scrittura nuovo layer personalizzato
    ''' </summary>
    ''' <param name="NomeLayer">Nome layer</param>
    ''' <param name="MostraDescrizioneAssociata">Indica se mostrare la descrizione associata al layer</param>
    ''' <returns>"OK"</returns>
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function DeleteCustomLayer(ByVal InData As Object) As rispostaStandard(Of Boolean)

        Dim r As New rispostaStandard(Of Boolean)

        Dim objParametri = DeserializzaInData(Of Integer)(InData)
        Dim layerCod As Integer = objParametri.InData

        Gias_InizializzaCultura_DaParams(objParametri.Server, objParametri.Utenti)

        Try
            r = V_M.DeleteCustomLayer(layerCod, objParametri.Server, objParametri.Utenti)

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function

    ''' <summary>
    ''' <para>
    ''' Salva nuovo elemento grafico da chiave albero con appezzamento.
    ''' </para>
    ''' <para>
    ''' Per ulteriori informazioni, vedi classe <seealso cref="SalvaNuovoElementoGraficoDaChiaveAlberoConAppezza_In"/>.
    ''' </para>
    ''' </summary>
    ''' <param name="InData">Dati salvataggio nuovo elemento grafico
    ''' </param>
    ''' <returns>
    ''' Messaggio di esito
    ''' </returns>
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function SalvaNuovoElementoGraficoDaChiaveAlberoConAppezza_2022(ByVal InData As Object) As rispostaStandard(Of Obj_SalvaGrafica)

        Dim r As New rispostaStandard(Of Obj_SalvaGrafica)

        Dim objParametri = DeserializzaInData(Of SalvaNuovoElementoGraficoDaChiaveAlberoConAppezza_In)(InData)
        Dim objSalvaNuovoElementoGrafico As SalvaNuovoElementoGraficoDaChiaveAlberoConAppezza_In = objParametri.InData

        Gias_InizializzaCultura_DaParams(objParametri.Server, objParametri.Utenti)

        Try
            r = V_M.SalvaNuovoElementoGraficoDaChiaveAlberoConAppezza_2022(objSalvaNuovoElementoGrafico,
                                                                           objParametri.Server)

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function

    ''' <summary>
    ''' <para>
    ''' Recupero informazioni WMS
    ''' </para>
    ''' </summary>
    ''' <param name="InData">Indirizzo web servizio recupero informazioni WMS</param>
    ''' <returns>
    ''' Messaggio di esito
    ''' </returns>
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function WmsGetFeature(ByVal InData As Object) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametri = DeserializzaInData(Of String)(InData)
        Dim url = objParametri.InData

        Gias_InizializzaCultura_DaParams(objParametri.Server, objParametri.Utenti)

        Try
            r = V_M.WmsGetFeature(url)

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function

    ''' <summary>
    ''' Legge elenco struttura attributi da anagrafica layer
    ''' </summary>
    ''' <returns></returns>
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiElencoStrutturaAttributiLayer(ByVal InData As Object) As rispostaStandard(Of DatiStrutturaAttributiLayer)

        Dim r As New rispostaStandard(Of DatiStrutturaAttributiLayer)

        Dim objParametri = DeserializzaInData(Of String)(InData)

        Gias_InizializzaCultura_DaParams(objParametri.Server, objParametri.Utenti)

        Try
            r.RispostaStringa = V_M.LeggiElencoStrutturaAttributiLayer(objParametri.InData, objParametri.Server, objParametri.Utenti)

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function

    ''' <summary>
    ''' Salvataggio layer personalizzato
    ''' </summary>
    ''' <returns></returns>
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function SalvaEntitaConAttributi(ByVal InData As Object) As rispostaStandard(Of SalvaEntitaConAttributi_Out)

        Dim r As New rispostaStandard(Of SalvaEntitaConAttributi_Out)

        Dim objParametri = DeserializzaInData(Of SalvaEntitaConAttributi_In)(InData)

        Gias_InizializzaCultura_DaParams(objParametri.Server, objParametri.Utenti)

        Try
            r = V_M.SalvaEntitaConAttributi(objParametri.InData, objParametri.Server)

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function

    ''' <summary>
    ''' Verifica esistenza entità per impianti
    ''' </summary>
    ''' <returns></returns>
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function VerificaEsistenzaEntitaPerImpianti(ByVal InData As Object) As rispostaStandard(Of VerificaEsistenzaEntitaPerImpianti_Out)

        Dim r As New rispostaStandard(Of VerificaEsistenzaEntitaPerImpianti_Out)

        Dim objParametri = DeserializzaInData(Of VerificaEsistenzaEntitaPerImpianti_In)(InData)

        Gias_InizializzaCultura_DaParams(objParametri.Server, objParametri.Utenti)

        Try
            r = V_M.VerificaEsistenzaEntitaPerImpianti(objParametri.InData, objParametri.Server)

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function

    ''' <summary>
    ''' Aggiorna filtro impianti
    ''' </summary>
    ''' <returns></returns>
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function AggiornaFiltroImpianti(ByVal InData As Object) As rispostaStandard(Of AggiornaFiltroImpianti_Out)

        Dim r As New rispostaStandard(Of AggiornaFiltroImpianti_Out)

        Dim objParametri = DeserializzaInData(Of AggiornaFiltroImpianti_In)(InData)

        Gias_InizializzaCultura_DaParams(objParametri.Server, objParametri.Utenti)

        Try
            r = V_M.AggiornaFiltroImpianti(objParametri.InData, objParametri.Server)

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiElencoPermessiLayerUtenti(ByVal InData As Object) As rispostaStandard(Of LeggiPermessiLayerUtenti_Out)

        Dim r As New rispostaStandard(Of LeggiPermessiLayerUtenti_Out)

        Dim objParametri = DeserializzaInData(Of LeggiPermessiLayerUtenti_In)(InData)

        Gias_InizializzaCultura_DaParams(objParametri.Server, objParametri.Utenti)

        Try
            Dim rPermessi = New AgronicaCoreGisBIZ.GIS_LayerElementiGrafici_Permessi_Utente_GruppiUtente_R()
            r.RispostaStringa = rPermessi.LeggiPermessiLayerUtente(objParametri.InData.Layer_Cod, objParametri.Server, objParametri.Utenti)
            r.RispostaOK = True
            r.Errore = ""

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function SalvaPermessiLayerUtenti(ByVal InData As Object) As RispostaStandard

        Dim r As New RispostaStandard()

        Dim objParametri = DeserializzaInData(Of SalvaPermessiLayerUtenti_In)(InData)

        Gias_InizializzaCultura_DaParams(objParametri.Server, objParametri.Utenti)

        Try
            Dim rPermessi = New AgronicaCoreGisBIZ.GIS_LayerElementiGrafici_Permessi_Utente_GruppiUtente_W()
            r.RispostaOK = rPermessi.AggiornaPermessiXUtente(objParametri.InData.Layer_Cod, objParametri.InData.utenti_permessi, objParametri.Server, objParametri.Utenti)
            r.RispostaStringa = "Permessi utente salvati"
            r.Errore = ""

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiElencoPermessiLayerGruppiUtente(ByVal InData As Object) As rispostaStandard(Of LeggiPermessiLayerGruppiUtente_Out)

        Dim r As New rispostaStandard(Of LeggiPermessiLayerGruppiUtente_Out)

        Dim objParametri = DeserializzaInData(Of LeggiPermessiLayerGruppiUtente_In)(InData)

        Gias_InizializzaCultura_DaParams(objParametri.Server, objParametri.Utenti)

        Try
            Dim rPermessi = New AgronicaCoreGisBIZ.GIS_LayerElementiGrafici_Permessi_Utente_GruppiUtente_R()
            r.RispostaStringa = rPermessi.LeggiPermessiLayerGruppiUtente(objParametri.InData.Layer_Cod, objParametri.Server, objParametri.Utenti)
            r.RispostaOK = True
            r.Errore = ""

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function SalvaPermessiLayerGruppiUtente(ByVal InData As Object) As RispostaStandard

        Dim r As New RispostaStandard()

        Dim objParametri = DeserializzaInData(Of SalvaPermessiLayerGruppiUtente_In)(InData)

        Gias_InizializzaCultura_DaParams(objParametri.Server, objParametri.Utenti)

        Try
            Dim rPermessi = New AgronicaCoreGisBIZ.GIS_LayerElementiGrafici_Permessi_Utente_GruppiUtente_W()
            r.RispostaOK = rPermessi.AggiornaPermessiXGruppiUtente(objParametri.InData.Layer_Cod, objParametri.InData.gruppiutente_permessi, objParametri.Server, objParametri.Utenti)
            r.RispostaStringa = "Permessi gruppi utente salvati"
            r.Errore = ""

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function

    ''' <summary>
    ''' Legge l'elenco degli attributi di un layer
    ''' </summary>
    ''' <returns></returns>
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiImpostazioniAvanzateLayer(ByVal InData As Object) As rispostaStandard(Of LeggiImpostazioniAvanzateLayer)
        Dim r As New rispostaStandard(Of LeggiImpostazioniAvanzateLayer)

        Dim objParametri = DeserializzaInData(Of LeggiImpostazioniAvanzateLayer_In)(InData)

        Gias_InizializzaCultura_DaParams(objParametri.Server, objParametri.Utenti)

        Try
            r.RispostaStringa = V_M.LeggiImpostazioniAvanzateLayer(objParametri.InData,
                                                                   objParametri.Server,
                                                                   objParametri.Utenti)
            r.RispostaOK = True
        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function

    ''' <summary>
    ''' Gestisce le operazioni CRUD sugli attributi dei layer
    ''' </summary>
    ''' <returns></returns>
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function AttributoLayer(ByVal InData As Object) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri = DeserializzaInData(Of AttributoLayer_In)(InData)

        Gias_InizializzaCultura_DaParams(objParametri.Server, objParametri.Utenti)

        Try
            r.RispostaOK = V_M.AttributoLayer(objParametri.InData,
                                                   objParametri.Server,
                                                   objParametri.Utenti)

            If r.RispostaOK Then
                r.RispostaStringa = "Operazione terminata con successo"
            Else
                r.RispostaStringa = "Impossibile terminare l'operazione"
            End If
        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function

    ''' <summary>
    ''' Gestisce la visualizzazione etichetta dell'attributo layer
    ''' </summary>
    ''' <returns></returns>
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function ImpostaVisualizzazioneEtichetta(ByVal InData As Object) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri = DeserializzaInData(Of ImpostaVisualizzazioneEtichetta_In)(InData)

        Gias_InizializzaCultura_DaParams(objParametri.Server, objParametri.Utenti)

        Try
            r.RispostaOK = V_M.ImpostaVisualizzazioneEtichetta(objParametri.InData,
                                                               objParametri.Server,
                                                               objParametri.Utenti)

            If r.RispostaOK Then
                r.RispostaStringa = "Operazione terminata con successo"
            Else
                r.RispostaStringa = "Impossibile terminare l'operazione"
            End If
        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function
    ''' <summary>
    ''' Gestisce il valore del campo CampoChiave dell'attributo layer
    ''' </summary>
    ''' <returns></returns>
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function ImpostaCampoChiaveLayer(ByVal InData As Object) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri = DeserializzaInData(Of ImpostaCampoChiaveLayer_In)(InData)

        Gias_InizializzaCultura_DaParams(objParametri.Server, objParametri.Utenti)

        Try
            r.RispostaOK = V_M.ImpostaCampoChiaveLayer(objParametri.InData,
                                                       objParametri.Server,
                                                       objParametri.Utenti)

            If r.RispostaOK Then
                r.RispostaStringa = "Operazione terminata con successo"
            Else
                r.RispostaStringa = "Impossibile terminare l'operazione"
            End If
        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function

    ''' <summary>
    ''' Gestisce l'attivazione dell'attributo layer
    ''' </summary>
    ''' <returns></returns>
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function ImpostaAttivazioneAttributoLayer(ByVal InData As Object) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri = DeserializzaInData(Of AttivaAttributoLayer_In)(InData)

        Gias_InizializzaCultura_DaParams(objParametri.Server, objParametri.Utenti)

        Try
            r.RispostaOK = V_M.ImpostaAttivazioneAttributoLayer(objParametri.InData,
                                                               objParametri.Server,
                                                               objParametri.Utenti)

            If r.RispostaOK Then
                r.RispostaStringa = "Operazione terminata con successo"
            Else
                r.RispostaStringa = "Impossibile terminare l'operazione"
            End If
        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function

    ''' <summary>
    ''' Per la creazione di nuovi utenti tramite importazione, imposta layer Gis se permessi cartografia
    ''' La funzione gestisce una transazione, in caso di eccezione imposta il valore sul rispostaStandard (sarà restituito dalla funzione)
    ''' </summary>
    ''' <returns></returns>
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function ImpostaLayerGisSePermessiCartografia(InData As CoreWS_Generic(Of String)) As RispostaStandard
        Dim r As New RispostaStandard

        Dim obj_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
        Dim utenteDestinazione As String = InData.InData

        Dim gis As New AgronicaCoreGisBIZ.GIS_LayerElementiGrafici_Permessi_Utente_GruppiUtente_W

        Try

            Dim msgErr = gis.ImpostaLayerGisSePermessiCartografia(utenteDestinazione, obj_Server)

            If msgErr <> "" Then
                r.RispostaStringa = msgErr
                r.RispostaOK = False
            Else
                r.RispostaOK = True
                r.RispostaStringa = "..."
            End If

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function

    ''' <summary>
    ''' Gestisce l'aggiornamento dei dati dei layer grafici
    ''' </summary>
    ''' <returns></returns>
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function AggiornaElementoGraficoPerTipoOggetto(ByVal InData As Object) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri = DeserializzaInData(Of AggiornaElementoGraficoPerTipoOggetto_In)(InData)

        Gias_InizializzaCultura_DaParams(objParametri.Server, objParametri.Utenti)

        Try
            r.RispostaOK = V_M.AggiornaElementoGraficoPerTipoOggetto(objParametri.InData,
                                                                     objParametri.Server,
                                                                     objParametri.Utenti)

            If r.RispostaOK Then
                r.RispostaStringa = "Operazione terminata con successo"
            Else
                r.RispostaStringa = "Impossibile terminare l'operazione"
            End If
        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function

    ''' <summary>
    ''' Legge la lista dei tipi oggetto di un dato elemento grafico
    ''' </summary>
    ''' <returns></returns>
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiTipiOggettoPerElementoGrafico(ByVal InData As Object) As rispostaStandard(Of LeggiTipiOggettoPerElementoGrafico)
        Dim r As New rispostaStandard(Of LeggiTipiOggettoPerElementoGrafico)

        Dim objParametri = DeserializzaInData(Of ElementoGraficoPerTipoOggetto_In)(InData)

        Gias_InizializzaCultura_DaParams(objParametri.Server, objParametri.Utenti)

        Try
            r.RispostaOK = True
            r.RispostaStringa = V_M.LeggiTipiOggettoPerElementoGrafico(objParametri.InData,
                                                                  objParametri.Server,
                                                                  objParametri.Utenti)

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function

    ''' <summary>
    ''' Legge la lista degli allegati dato un codice di ricetta di destinazione
    ''' </summary>
    ''' <returns></returns>
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiAllegatiDaRicettaDestinazione(ByVal InData As Object) As rispostaStandard(Of LeggiAllegatiDaRicettaDestinazione)
        Dim r As New rispostaStandard(Of LeggiAllegatiDaRicettaDestinazione)

        Dim objParametri = DeserializzaInData(Of LeggiAllegatiDaRicettaDestinazione_In)(InData)

        Gias_InizializzaCultura_DaParams(objParametri.Server, objParametri.Utenti)

        Try
            r.RispostaOK = True
            r.RispostaStringa = V_M.LeggiAllegatiDaRicettaDestinazione(objParametri.InData,
                                                                  objParametri.Server,
                                                                  objParametri.Utenti)

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function

    ''' <summary>
    ''' Verifica l'esistenza di un Impianto e/o appezzamento
    ''' </summary>
    ''' <returns></returns>
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function VerificaEsistenzaEntitaAnagrafiche(ByVal InData As Object) As rispostaStandard(Of VerificaEsistenzaEntitaAnagrafiche)
        Dim r As New rispostaStandard(Of VerificaEsistenzaEntitaAnagrafiche)

        Dim objParametri = DeserializzaInData(Of VerificaEsistenzaEntitaAnagrafiche_In)(InData)

        Gias_InizializzaCultura_DaParams(objParametri.Server, objParametri.Utenti)

        Try
            r.RispostaOK = True
            r.RispostaStringa = V_M.VerificaEsistenzaEntitaAnagrafiche(objParametri.InData,
                                                                       objParametri.Server,
                                                                       objParametri.Utenti)

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function PosizioniRilevate(ByVal InData As Object) As rispostaStandard(Of GisDataReadRval_New(Of AgronicaCoreModelsSTD.Gis.GeoJSONAgroGisProp))

        Dim objParametri = DeserializzaInData(Of LettureTecniciInCampo_In)(InData)

        Gias_InizializzaCultura_DaParams(objParametri.Server, objParametri.Utenti)

        Dim r As New rispostaStandard(Of GisDataReadRval_New(Of AgronicaCoreModelsSTD.Gis.GeoJSONAgroGisProp))

        Try

            r.RispostaOK = True

            r.RispostaStringa = New GisDataReadRval_New(Of AgronicaCoreModelsSTD.Gis.GeoJSONAgroGisProp)
            r.RispostaStringa.myGeoJson = V_M.LettureTecniciInCampo(objParametri.InData,
                                                                    False,
                                                                    objParametri.Server,
                                                                    objParametri.Utenti)
        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function UltimaPosizione(ByVal InData As Object) As rispostaStandard(Of GisDataReadRval_New(Of AgronicaCoreModelsSTD.Gis.GeoJSONAgroGisProp))

        Dim objParametri = DeserializzaInData(Of LettureTecniciInCampo_In)(InData)

        Gias_InizializzaCultura_DaParams(objParametri.Server, objParametri.Utenti)

        Dim r As New rispostaStandard(Of GisDataReadRval_New(Of AgronicaCoreModelsSTD.Gis.GeoJSONAgroGisProp))

        Try

            r.RispostaOK = True

            r.RispostaStringa = New GisDataReadRval_New(Of AgronicaCoreModelsSTD.Gis.GeoJSONAgroGisProp)
            r.RispostaStringa.myGeoJson = V_M.LettureTecniciInCampo(objParametri.InData,
                                                                    True,
                                                                    objParametri.Server,
                                                                    objParametri.Utenti)
        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LetturaDatiElaboratiSuSensoreListaValori(ByVal InData As Object) As RispostaStandard

        Dim objParametri = DeserializzaInData(Of LetturaDatiElaboratiSuSensoreListaValori_In)(InData)

        Gias_InizializzaCultura_DaParams(objParametri.Server, objParametri.Utenti)

        Dim r As New RispostaStandard

        Try

            Dim isGestioneLegacy As Boolean = False
            Dim utentiImpostazioni As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read

            Dim listaImpostazioni = utentiImpostazioni.Leggi(TipiEnumerativi.enum_Impostazioni_Utenti.UTENTE_GIS,
                                                         1,
                                                         AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                         "",
                                                         "",
                                                         objParametri.Utenti)

            If listaImpostazioni.Rows.Count > 0 Then

                Dim valori As String() = listaImpostazioni(0)("Impostazione_Valore_1").Split("§")

                Dim gestioneLegacyParam = valori.Where(Function(s) s.StartsWith("ckGestioneAnalisiMappeLegacy"))

                If gestioneLegacyParam.Count > 0 Then
                    isGestioneLegacy = CBool(gestioneLegacyParam.FirstOrDefault.Split("^")(1))
                End If
            Else
                Throw New Exception("Errore nella lettura dell'impostazione utente.")
            End If

            If isGestioneLegacy Then
                r = V_M.LetturaDatiElaboratiSuSensoreListaValori_2022(objParametri.InData.PoligonoWKT,
                                                                      objParametri.InData.Zoom,
                                                                      objParametri.InData.Sensore,
                                                                      objParametri.InData.DataInizio,
                                                                      objParametri.InData.DataFine,
                                                                      objParametri.Server,
                                                                      objParametri.Utenti)

            Else 

                Dim cfgRead As New AgronicaCoreVarieDAL.Configurazione_Siti_R
                Dim dt = cfgRead.Leggi(0, SAT_IsActive_ConfKey, "", "", objParametri.Server)

                Dim isSatActive = Not dt Is Nothing And dt.Rows.Count > 0 And
                                  dt.Rows(0)("Valore").ToString().ToLower() = "true"
                Dim xRead As New AgronicaCoreDatiSensoriBIZ.DatiSensori_R

                If isSatActive Then
                    r = xRead.LetturaVegIndexes_SAT(objParametri.InData.LayerElementiGrafici_Cod,
                                                    objParametri.InData.PoligonoWKT,
                                                    objParametri.InData.Zoom,
                                                    objParametri.InData.Sensore,
                                                    objParametri.InData.DataInizio,
                                                    objParametri.InData.DataFine,
                                                    objParametri.Server,
                                                    objParametri.Super_Server)
                Else
                    r = xRead.LetturaVegIndexes_GEE(objParametri.InData.LayerElementiGrafici_Cod,
                                                    objParametri.InData.PoligonoWKT,
                                                    objParametri.InData.Zoom,
                                                    objParametri.InData.Sensore,
                                                    objParametri.InData.DataInizio,
                                                    objParametri.InData.DataFine,
                                                    objParametri.Server)
                End If
            End If

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LayerTilesDescrizione(ByVal InData As Object) As RispostaStandard

        Dim param = DeserializzaInData(Of LayerTilesDescrizione_In)(InData)

        Gias_InizializzaCultura_DaParams(param.Server, param.Utenti)

        Dim resp As New RispostaStandard
        Dim objBiz As New AgronicaCoreGisBIZ.GIS_LayerTilesDescrizione_W

        Try
            Select Case param.InData.tipoOperazione
                Case enum_Tipo_Operazione.INSERT

                    resp = objBiz.SalvaLayerTilesDescrizione(param.InData.tipologiaLabel, True, param.Server)

                Case enum_Tipo_Operazione.UPDATE

                    resp = objBiz.AggiornaLayerTilesDescrizione(param.InData.tipologiaLabel, True, param.Server)

                Case enum_Tipo_Operazione.DELETE

                    resp = objBiz.EliminaLayerTilesDescrizione(param.InData.tipologiaLabel, True, param.Server)

                Case Else

                    Throw New Exception("Tipo operazione non riconosciuto.")

            End Select

        Catch ex As Exception

            resp.RispostaOK = False
            resp.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return resp
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiLayerTilesDescrizione(ByVal InData As Object) As rispostaStandard(Of List(Of TipologiaLabel))

        Dim param = DeserializzaInData(Of LeggiLayerTilesDescrizione_In)(InData)

        Gias_InizializzaCultura_DaParams(param.Server, param.Utenti)

        Dim resp As New rispostaStandard(Of List(Of TipologiaLabel))
        Dim objBiz As New AgronicaCoreGisBIZ.GIS_LayerTilesDescrizione_R

        Try

            resp = objBiz.LeggiLayerTilesDescrizione(param.InData.LayerTiles_Cod,
                                                     param.InData.TipologiaLayer_cod,
                                                     param.InData.LayerElementiGrafici_Cod,
                                                     param.InData.LayerTilesDescrizione_Cod,
                                                     param.Server)

        Catch ex As Exception

            resp.RispostaOK = False
            resp.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return resp
    End Function

#End Region

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiSistemiRiferimento(ByVal InData As Object) As rispostaStandard(Of String)

        Dim objParametri = DeserializzaInData(Of LeggiSistemiRiferimento_In)(InData)

        Gias_InizializzaCultura_DaParams(objParametri.Server, objParametri.Utenti)

        Dim resp As New rispostaStandard(Of String)
        Dim objBiz As New AgronicaCoreGisBIZ.SR_TR

        Try
            resp.RispostaStringa = objBiz.LeggiSistemiRiferimento(objParametri.InData.codice_tipologia,
                                                                  objParametri.Server, objParametri.Utenti)
            resp.RispostaOK = True
        Catch ex As Exception

            resp.RispostaOK = False
            resp.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return resp
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function CaricaFileShape(ByVal InData As Object) As RispostaStandard

        Dim objParametri = DeserializzaInData(Of CaricaFileCompletoCoreWS_In)(InData)

        Gias_InizializzaCultura_DaParams(objParametri.Server, objParametri.Utenti)

        Dim resp As New RispostaStandard

        Try
            Dim fileList As String() = Nothing

            If Not objParametri.InData.tipologiaShape_cod = Tipo_Importazione_FileShape.Importa_Raster Then
                Dim catasto As TipoFileCatasto

                If objParametri.InData.datiCatasto IsNot Nothing Then
                    catasto = objParametri.InData.datiCatasto.fileCatasto
                Else
                    catasto = Nothing
                End If

                fileList = V_M.LeggiContenutoZipFileShape(objParametri.InData.fileZip,
                                                          objParametri.InData.fileZipName,
                                                          objParametri.InData.tipologiaShape_cod,
                                                          catasto,
                                                          objParametri.Server)
            End If

            If objParametri.InData.tipologiaShape_cod = Tipo_Importazione_FileShape.Importa_Raster Then

                fileList = V_M.SalvaFileRaster(objParametri.InData.fileZip,
                                               objParametri.InData.fileZipName,
                                               objParametri.Server)
            End If

            If fileList Is Nothing OrElse fileList.Count = 0 Then
                Throw New Exception("Non è stato trovato nessun file idoneo all'importazione.")
            End If

            For Each fileName In fileList
                resp.RispostaOK =
                    AgronicaControlliGIS.V_M.salvaElementiGraficiDaFileShape(fileName,
                                                                             objParametri.InData.layer_cod,
                                                                             objParametri.InData.datiImpianto,
                                                                             objParametri.InData.datiCatasto,
                                                                             objParametri.InData.tipologiaShape_cod,
                                                                             objParametri.InData.codice_sistemaRiferimento,
                                                                             objParametri.InData.progressivoGIAS,
                                                                             objParametri.Server,
                                                                             objParametri.Utenti,
                                                                             objParametri.InData.Validita_Inizio,
                                                                             objParametri.InData.Validita_Fine,
                                                                             objParametri.InData.Description,
                                                                             objParametri.InData.PixelSize)

                If Not resp.RispostaOK Then
                    Throw New Exception("Errore nell'importazione.")
                End If
            Next

            resp.RispostaStringa = "Importazione terminata con successo."

        Catch ex As Exception

            resp.RispostaOK = False
            resp.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return resp
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiAlgoritmiProiezione(ByVal InData As Object) As rispostaStandard(Of ElencoAlgoritmi)

        Dim objParametri = DeserializzaInData(Of String)(InData)

        Gias_InizializzaCultura_DaParams(objParametri.Server, objParametri.Utenti)

        Dim resp As New rispostaStandard(Of ElencoAlgoritmi)

        Dim xRead As New AgronicaCoreMetaSchemaBIZ.Proiezioni_R

        Try

            resp.RispostaStringa = xRead.LeggiElencoAlgoritmi(objParametri.Server)

            resp.RispostaOK = True

        Catch ex As Exception

            resp.RispostaOK = False
            resp.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return resp
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function ActivateAlgorithm(ByVal InData As Object) As RispostaStandard
        Dim nomeRoutine As String = "AgronicaCoreWS.GisWS.ActivateAlgorithm()"
        Dim objParametri = DeserializzaInData(Of ConfigurazioneProiezione)(InData)

        Gias_InizializzaCultura_DaParams(objParametri.Server, objParametri.Utenti)

        Dim resp As New RispostaStandard

        Try
            Dim risultatoOperazioneStringa As String

            Dim xWrite As New AgronicaCoreGisBIZ.ProiezioniLayer_W

            If objParametri.InData.ConfigurazioneProiezione_Cod > 0 Then

                Dim xRead As New AgronicaCoreGisBIZ.GIS_Permessi_Configurazione_R
                Dim permessiConfigurazione = xRead.LeggiPermessiDaUtente(objParametri.Utenti, objParametri.Server, objParametri.InData.ConfigurazioneProiezione_Cod)

                If permessiConfigurazione Is Nothing _
                   OrElse permessiConfigurazione.elencoPermessiUtente.Count = 0 _
                   OrElse permessiConfigurazione.elencoPermessiUtente.FirstOrDefault.elencoPermessiConfigurazione Is Nothing _
                   OrElse permessiConfigurazione.elencoPermessiUtente.FirstOrDefault.elencoPermessiConfigurazione.Count = 0 _
                   OrElse Not CBool(permessiConfigurazione.elencoPermessiUtente.FirstOrDefault.elencoPermessiConfigurazione.FirstOrDefault.Flag_Amministrazione) Then

                    resp.RispostaOK = False
                    resp.Errore = "NOT_ALLOWED"

                    Return resp

                End If

                risultatoOperazioneStringa = xWrite.ActivateAlgoritm(
                    objParametri.InData.ConfigurazioneProiezione_Cod,
                    objParametri.InData.AlgoritmoProiezione_Cod,
                    objParametri.Server
                    )
            Else
                Throw New GiasException("Invalid Configuration Cod, not able to perform activation")
            End If

            Select Case risultatoOperazioneStringa
                Case "OK", "CONF_INUSO", "CONF_INESISTENTE"
                    resp.RispostaOK = True
                    resp.RispostaStringa = risultatoOperazioneStringa
                Case Else
                    Throw New Exception(String.Format("ERROR: invalid result state in {0}", nomeRoutine))
            End Select

        Catch ex As GiasException
            resp.RispostaOK = True
            resp.Errore = ex.Message
        Catch ex As Exception
            resp.RispostaOK = False
            resp.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return resp
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function SaveAlgorithmConfigurationCfg(ByVal InData As Object) As RispostaStandard
        Dim nomeRoutine As String = "AgronicaCoreWS.GisWS.SaveAlgorithmConfigurationCfg()"
        Dim objParametri = DeserializzaInData(Of ConfigurazioneProiezione)(InData)

        Gias_InizializzaCultura_DaParams(objParametri.Server, objParametri.Utenti)

        Dim resp As New RispostaStandard

        Try
            Dim risultatoOperazioneStringa As String

            Dim xWrite As New AgronicaCoreGisBIZ.ProiezioniLayer_W

            If objParametri.InData.ConfigurazioneProiezione_Cod > 0 Then

                Dim xRead As New AgronicaCoreGisBIZ.GIS_Permessi_Configurazione_R
                Dim permessiConfigurazione = xRead.LeggiPermessiDaUtente(objParametri.Utenti, objParametri.Server, objParametri.InData.ConfigurazioneProiezione_Cod)

                If permessiConfigurazione Is Nothing _
                   OrElse permessiConfigurazione.elencoPermessiUtente.Count = 0 _
                   OrElse permessiConfigurazione.elencoPermessiUtente.FirstOrDefault.elencoPermessiConfigurazione Is Nothing _
                   OrElse permessiConfigurazione.elencoPermessiUtente.FirstOrDefault.elencoPermessiConfigurazione.Count = 0 _
                   OrElse Not CBool(permessiConfigurazione.elencoPermessiUtente.FirstOrDefault.elencoPermessiConfigurazione.FirstOrDefault.Flag_Amministrazione) Then

                    resp.RispostaOK = False
                    resp.Errore = "NOT_ALLOWED"

                    Return resp
                End If

                risultatoOperazioneStringa = xWrite.SaveAlgorithmConfigurationCfg(
                    objParametri.InData.ConfigurazioneProiezione_Cod,
                    objParametri.InData.AlgoritmoProiezione_Cod,
                    objParametri.InData.cfg,
                    objParametri.Server
                    )

                resp.RispostaOK = True
                resp.RispostaStringa = risultatoOperazioneStringa
            End If

        Catch ex As GiasException
            resp.RispostaOK = False
            resp.Errore = ex.Message
        Catch ex As Exception
            resp.RispostaOK = False
            resp.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return resp
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function SalvaConfigurazioneProiezione(ByVal InData As Object) As RispostaStandard

        Dim objParametri = DeserializzaInData(Of ConfigurazioneProiezione)(InData)

        Gias_InizializzaCultura_DaParams(objParametri.Server, objParametri.Utenti)

        Dim resp As New RispostaStandard

        Try
            Dim risultatoOperazioneStringa As String

            Dim xWrite As New AgronicaCoreGisBIZ.ProiezioniLayer_W

            If objParametri.InData.ConfigurazioneProiezione_Cod > 0 Then

                Dim xRead As New AgronicaCoreGisBIZ.GIS_Permessi_Configurazione_R
                Dim permessiConfigurazione = xRead.LeggiPermessiDaUtente(objParametri.Utenti, objParametri.Server, objParametri.InData.ConfigurazioneProiezione_Cod)

                If permessiConfigurazione Is Nothing _
                   OrElse permessiConfigurazione.elencoPermessiUtente.Count = 0 _
                   OrElse permessiConfigurazione.elencoPermessiUtente.FirstOrDefault.elencoPermessiConfigurazione Is Nothing _
                   OrElse permessiConfigurazione.elencoPermessiUtente.FirstOrDefault.elencoPermessiConfigurazione.Count = 0 _
                   OrElse Not CBool(permessiConfigurazione.elencoPermessiUtente.FirstOrDefault.elencoPermessiConfigurazione.FirstOrDefault.Flag_Amministrazione) Then

                    resp.RispostaOK = False
                    resp.Errore = "NOT_ALLOWED"

                    Return resp

                End If

                risultatoOperazioneStringa = xWrite.ModificaConfigurazioneProiezione(
                    objParametri.InData.ConfigurazioneProiezione_Cod,
                    objParametri.InData.AlgoritmoProiezione_Cod,
                    objParametri.InData.ConfigurazioneProiezione_Des,
                    objParametri.InData.ConfigurazioneProiezione_GUID,
                    objParametri.InData.Layer1,
                    objParametri.InData.Layer2,
                    objParametri.InData.LayerRisultato,
                    objParametri.Server,
                    objParametri.Utenti
                    )
            Else

                risultatoOperazioneStringa = xWrite.SalvaConfigurazioneProiezione(
                    objParametri.InData.AlgoritmoProiezione_Cod,
                    objParametri.InData.ConfigurazioneProiezione_Des,
                    objParametri.InData.ConfigurazioneProiezione_GUID,
                    objParametri.InData.Layer1,
                    objParametri.InData.Layer2,
                    objParametri.InData.LayerRisultato,
                    objParametri.Server,
                    objParametri.Utenti
                    )

            End If

            Select Case risultatoOperazioneStringa
                Case "OK"
                    resp.RispostaOK = True
                    resp.RispostaStringa = "Operazione completata con successo."
                Case "CONF_ESISTENTE"
                    Throw New Exception("Configurazione già presente a sistema.")
                Case "CONF_INESISTENTE"
                    Throw New Exception("Configurazione non presente a sistema.")
                Case "CONF_INUSO"
                    Throw New Exception("Configurazione già attiva e/o esegiuta.")
                Case Else
                    Throw New Exception("Errore nel salvataggio della configurazione.")
            End Select


        Catch ex As Exception

            resp.RispostaOK = False
            resp.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return resp
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiConfigurazioniProiezione(ByVal InData As Object) As rispostaStandard(Of ElencoConfigurazioniProiezione)

        Dim objParametri = DeserializzaInData(Of String)(InData)

        Gias_InizializzaCultura_DaParams(objParametri.Server, objParametri.Utenti)

        Dim resp As New rispostaStandard(Of ElencoConfigurazioniProiezione)
        Dim xRead As New AgronicaCoreGisBIZ.ProiezioniLayer_R
        Dim xReadPermessi As New AgronicaCoreGisBIZ.GIS_Permessi_Configurazione_R

        Try

            Dim elencoConfigurazioni = xRead.LeggiElencoConfigurazioni(objParametri.Server, objParametri.Utenti)

            For Each configurazione In elencoConfigurazioni.elencoConfigurazioniProiezione
                Dim elencoPermessi = xReadPermessi.LeggiPermessiDaUtente(objParametri.Utenti,
                                                                         objParametri.Server,
                                                                         configurazione.ConfigurazioneProiezione_Cod)

                'Non ho bisogno di verificare se sono null in quanto l'elenco delle configurazioni contiene solo configurazioni
                '   per le quali l'utente (o uno dei gruppi a cui appartiene) ha permessi collegati
                configurazione.canManage = CBool(elencoPermessi.
                                                 elencoPermessiUtente.FirstOrDefault.
                                                 elencoPermessiConfigurazione.FirstOrDefault.
                                                 Flag_Amministrazione)
            Next

            resp.RispostaOK = True
            resp.RispostaStringa = elencoConfigurazioni

        Catch ex As Exception

            resp.RispostaOK = False
            resp.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return resp
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiConfigurazioniProiezioneFiltrati(ByVal InData As Object) As rispostaStandard(Of ElencoConfigurazioniProiezione)

        Dim a As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}
        Dim params As CoreWS_Generic(Of String) =
            JsonConvert.DeserializeObject(Of CoreWS_Generic(Of String))(JsonConvert.SerializeObject(InData), a)

        Dim resp As New rispostaStandard(Of ElencoConfigurazioniProiezione)
        Dim xRead As New AgronicaCoreGisBIZ.ProiezioniLayer_R
        Dim xReadPermessi As New AgronicaCoreGisBIZ.GIS_Permessi_Configurazione_R

        If params.objP.objP_server = "" Then
            resp.Errore = "objP_server non valorizzato"
            Return resp
        End If

        If params.objP.objP_utenti = "" Then
            resp.Errore = "objP_utenti non valorizzato"
            Return resp
        End If

        Try
            Dim objParametri_Super_Server As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(params.objP.objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(params.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(params.objP.objP_utenti)

            Gias_InizializzaCultura_DaParams(objParametri_Server, objParametri_Utenti)

            Dim elencoConfigurazioni = xRead.LeggiElencoConfigurazioni(objParametri_Server, objParametri_Utenti, cfgFilter:=params.InData)

            For Each configurazione In elencoConfigurazioni.elencoConfigurazioniProiezione
                Dim elencoPermessi = xReadPermessi.LeggiPermessiDaUtente(objParametri_Utenti,
                                                                         objParametri_Server,
                                                                         configurazione.ConfigurazioneProiezione_Cod)

                'Non ho bisogno di verificare se sono null in quanto l'elenco delle configurazioni contiene solo configurazioni
                '   per le quali l'utente (o uno dei gruppi a cui appartiene) ha permessi collegati
                configurazione.canManage = CBool(elencoPermessi.
                                                 elencoPermessiUtente.FirstOrDefault.
                                                 elencoPermessiConfigurazione.FirstOrDefault.
                                                 Flag_Amministrazione)
            Next

            resp.RispostaOK = True
            resp.RispostaStringa = elencoConfigurazioni

        Catch ex As Exception

            resp.RispostaOK = False
            resp.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return resp
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiConfigurazioniProiezioneSuLayer(ByVal InData As Object) As rispostaStandard(Of ElencoConfigurazioniSuLayer)

        Dim objParametri = DeserializzaInData(Of LeggiElencoConfigurazioni_In)(InData)

        Gias_InizializzaCultura_DaParams(objParametri.Server, objParametri.Utenti)

        Dim resp As New rispostaStandard(Of ElencoConfigurazioniSuLayer)
        Dim xRead As New AgronicaCoreGisBIZ.ProiezioniLayer_R

        Try

            Dim elencoConfigurazioni = xRead.LeggiElencoConfigurazioniSuLayer(objParametri.InData.LayerElementiGrafici_Cod,
                                                                              objParametri.InData.TipologiaLayer_cod,
                                                                              objParametri.InData.Entita_Cod,
                                                                              objParametri.Server)

            resp.RispostaOK = True
            resp.RispostaStringa = elencoConfigurazioni

        Catch ex As Exception

            resp.RispostaOK = False
            resp.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return resp
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiPermessiUtenteConfigurazioni(ByVal InData As Object) As rispostaStandard(Of ElencoPermessiConfigurazione)

        Dim objParametri = DeserializzaInData(Of Int32)(InData)

        Gias_InizializzaCultura_DaParams(objParametri.Server, objParametri.Utenti)

        Dim resp As New rispostaStandard(Of ElencoPermessiConfigurazione)
        Dim xRead As New AgronicaCoreGisBIZ.GIS_Permessi_Configurazione_R

        Try

            Dim elencoPermessiConfigurazioni = xRead.LeggiPermessiUtenteByConfigurazioneCod(objParametri.InData, objParametri.Server)

            resp.RispostaOK = True
            resp.RispostaStringa = elencoPermessiConfigurazioni

        Catch ex As Exception

            resp.RispostaOK = False
            resp.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return resp
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiPermessiGruppiUtenteConfigurazioni(ByVal InData As Object) As rispostaStandard(Of ElencoPermessiConfigurazione)

        Dim objParametri = DeserializzaInData(Of Int32)(InData)

        Gias_InizializzaCultura_DaParams(objParametri.Server, objParametri.Utenti)

        Dim resp As New rispostaStandard(Of ElencoPermessiConfigurazione)
        Dim xRead As New AgronicaCoreGisBIZ.GIS_Permessi_Configurazione_R

        Try

            Dim elencoPermessiConfigurazioni = xRead.LeggiPermessiGruppiUtenteByConfigurazioneCod(objParametri.InData,
                                                                                                  objParametri.Server,
                                                                                                  objParametri.Utenti)

            resp.RispostaOK = True
            resp.RispostaStringa = elencoPermessiConfigurazioni

        Catch ex As Exception

            resp.RispostaOK = False
            resp.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return resp
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiPermessiConfigurazioniDaUtente(ByVal InData As Object) As rispostaStandard(Of ElencoPermessiUtente)

        Dim objParametri = DeserializzaInData(Of String)(InData)

        Gias_InizializzaCultura_DaParams(objParametri.Server, objParametri.Utenti)

        Dim resp As New rispostaStandard(Of ElencoPermessiUtente)
        Dim xRead As New AgronicaCoreGisBIZ.GIS_Permessi_Configurazione_R

        Try

            Dim elencoPermessiConfigurazioni = xRead.LeggiPermessiDaUtente(objParametri.Utenti, objParametri.Server)

            resp.RispostaOK = True
            resp.RispostaStringa = elencoPermessiConfigurazioni

        Catch ex As Exception

            resp.RispostaOK = False
            resp.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return resp
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function OperazioniPermessiProiezioni(ByVal InData As Object) As RispostaStandard

        Dim objParametri = DeserializzaInData(Of ModifichePermessiConfigurazione)(InData)

        Gias_InizializzaCultura_DaParams(objParametri.Server, objParametri.Utenti)

        Dim resp As New RispostaStandard
        Dim xWrite As New AgronicaCoreGisBIZ.GIS_Permessi_Configurazione_W

        Try
            resp.RispostaOK = xWrite.OperazioniPermessiProiezioni(objParametri.InData, objParametri.Server)

            If Not resp.RispostaOK Then
                Throw New Exception("Errore nella gestione dei permessi della configurazione di proiezione.")
            End If

            resp.RispostaStringa = "Operazione completata con successo."

        Catch ex As Exception

            resp.RispostaOK = False
            resp.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return resp
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function AttivaDisattivaConfigurazione(ByVal InData As Object) As rispostaStandard(Of AttivaDisattivaConfigurazione_Out)

        Dim objParametri = DeserializzaInData(Of AttivazioneConfigurazioneAlgoritmiCartografici)(InData)

        Gias_InizializzaCultura_DaParams(objParametri.Server, objParametri.Utenti)

        Dim resp As New rispostaStandard(Of AttivaDisattivaConfigurazione_Out)
        resp.RispostaStringa = New AttivaDisattivaConfigurazione_Out

        Dim xWrite As New AgronicaCoreGisBIZ.ProiezioniLayer_W
        Dim xRead As New AgronicaCoreGisBIZ.GIS_Permessi_Configurazione_R
        Dim proiezioniRead As New AgronicaCoreGisDAL.ProiezioniLayer_R
        Dim seq As New Agro_Sequenze

        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False

        Dim idRequest As Integer = -1 'Lavez - 07/11/2024 - garbage per riunificazione metodo AttivaDisattivaConfigurazioneSuEntita

        Dim permessiConfigurazione = xRead.LeggiPermessiDaUtente(objParametri.Utenti, objParametri.Server, objParametri.InData.configurazioneProiezione_Cod)

        Try
            'Lavez - 07/11/2024 - assegnazione groupid per parallelismo
            Dim groupId = seq.NuovoId_Tabella("GIS_GroupID", 0, CostantiPersonalizzate.UpperBoundTabelle_Per_SequenzaTabelle_Topcode, objParametri.Server)

            If objParametri.InData.layer_cod <> 0 Then

                If permessiConfigurazione Is Nothing _
                   OrElse permessiConfigurazione.elencoPermessiUtente.Count = 0 _
                   OrElse permessiConfigurazione.elencoPermessiUtente.FirstOrDefault.elencoPermessiConfigurazione Is Nothing _
                   OrElse permessiConfigurazione.elencoPermessiUtente.FirstOrDefault.elencoPermessiConfigurazione.Count = 0 _
                   OrElse Not CBool(permessiConfigurazione.elencoPermessiUtente.FirstOrDefault.elencoPermessiConfigurazione.FirstOrDefault.Flag_GestioneInteroLayer) Then

                    resp.RispostaOK = True
                    resp.RispostaStringa.Risultato = AttivaDisattivaConfigurazioneEnum.NonAutorizzato

                    Return resp

                End If

                resp.RispostaOK = xWrite.AttivaDisattivaConfigurazioneSuLayer(objParametri.InData.configurazioneProiezione_Cod,
                                                                              objParametri.InData.layer_cod,
                                                                              objParametri.InData.tipologia_layer_cod,
                                                                              objParametri.InData.isAttivo,
                                                                              objParametri.Server,
                                                                              groupId)
            Else

                If permessiConfigurazione Is Nothing _
                   OrElse permessiConfigurazione.elencoPermessiUtente.Count = 0 _
                   OrElse permessiConfigurazione.elencoPermessiUtente.FirstOrDefault.elencoPermessiConfigurazione Is Nothing _
                   OrElse permessiConfigurazione.elencoPermessiUtente.FirstOrDefault.elencoPermessiConfigurazione.Count = 0 _
                   OrElse Not (CBool(permessiConfigurazione.elencoPermessiUtente.FirstOrDefault.elencoPermessiConfigurazione.FirstOrDefault.Flag_Inserimento) _
                                And CBool(permessiConfigurazione.elencoPermessiUtente.FirstOrDefault.elencoPermessiConfigurazione.FirstOrDefault.Flag_Modifica)) Then

                    resp.RispostaOK = True
                    resp.RispostaStringa.Risultato = AttivaDisattivaConfigurazioneEnum.NonAutorizzato

                    Return resp

                End If

                AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale,
                                                                                        FlagTransazioneLocale,
                                                                                        objParametri.Server)

                For Each entitaAlgoritmo In objParametri.InData.listaEntita

                    Dim codiceEsecuzione = proiezioniRead.VerificaEsistenzaEsecuzione(objParametri.InData.configurazioneProiezione_Cod,
                                                                         entitaAlgoritmo.entita_cod_1,
                                                                         entitaAlgoritmo.entita_cod_2,
                                                                         entitaAlgoritmo.entita_cod_risultato,
                                                                        objParametri.Server)
                    If codiceEsecuzione <> 0 Then
                        resp.RispostaOK = True
                        resp.RispostaStringa.Risultato = AttivaDisattivaConfigurazioneEnum.GiaAttivato
                        Return resp
                    End If

                    resp.RispostaOK = xWrite.AttivaDisattivaConfigurazioneSuEntita(objParametri.InData.configurazioneProiezione_Cod,
                                                                                   entitaAlgoritmo.entita_cod_1,
                                                                                   entitaAlgoritmo.entita_cod_2,
                                                                                   entitaAlgoritmo.entita_cod_risultato,
                                                                                   objParametri.InData.isAttivo,
                                                                                   idRequest,
                                                                                   objParametri.Server,
                                                                                   False,
                                                                                   groupId:=groupId)
                Next

            End If

            If Not resp.RispostaOK Then
                Throw New Exception("Errore nell'attivazione/disattivazione della configurazione.")
            End If

            If Not objParametri.Server.objTransazione Is Nothing Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri.Server)
            End If

            resp.RispostaStringa.Risultato = AttivaDisattivaConfigurazioneEnum.AttivatoCorrettamente

        Catch ex As Exception

            If Not objParametri.Server.objTransazione Is Nothing Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri.Server)
            End If

            resp.RispostaOK = False
            resp.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        Finally
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri.Server)
        End Try

        Return resp
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiLogEsecuzioniConfigurazione(ByVal InData As Object) As rispostaStandard(Of ElencoLogEsecuzioniConfigurazioniProiezione)

        Dim objParametri = DeserializzaInData(Of Int32)(InData)

        Gias_InizializzaCultura_DaParams(objParametri.Server, objParametri.Utenti)

        Dim resp As New rispostaStandard(Of ElencoLogEsecuzioniConfigurazioniProiezione)

        Dim xRead As New AgronicaCoreGisBIZ.ProiezioniLayer_R

        Try

            resp.RispostaStringa = xRead.LeggiLogEsecuzioni(objParametri.InData, objParametri.Server)

            resp.RispostaOK = True

        Catch ex As Exception

            resp.RispostaOK = False
            resp.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return resp
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiMaschereLayerRaster(ByVal InData As Object) As rispostaStandard(Of ElencoMaschereLayerRaster)

        Dim objParametri = DeserializzaInData(Of MaschereLayerFiltro_In)(InData)

        Gias_InizializzaCultura_DaParams(objParametri.Server, objParametri.Utenti)

        Dim resp As New rispostaStandard(Of ElencoMaschereLayerRaster)

        Dim xRead As New AgronicaCoreGisBIZ.MaschereLayerRaster_R

        Try

            resp.RispostaStringa = xRead.LeggiElencoMaschere(objParametri.InData.LayerElementiGrafici_Raster_Cod,
                                                             objParametri.InData.TipologiaLayer_Raster_Cod,
                                                             objParametri.Utenti,
                                                             objParametri.Server)

            resp.RispostaOK = True

        Catch ex As Exception

            resp.RispostaOK = False
            resp.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return resp
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function OperazioniMaschereLayerRaster(ByVal InData As Object) As RispostaStandard

        Dim objParametri = DeserializzaInData(Of OperazioneMascheraLayerRaster_In)(InData)

        Gias_InizializzaCultura_DaParams(objParametri.Server, objParametri.Utenti)

        Dim resp As New RispostaStandard

        Dim xWrite As New AgronicaCoreGisBIZ.MaschereLayerRaster_W

        Try

            Select Case objParametri.InData.codice_operazione
                Case TipoOperazioneMaschera.INSERT
                    resp.RispostaOK = xWrite.InserisciMascheraLayerRaster(objParametri.InData.Maschera_Des,
                                                                          objParametri.InData.LayerElementiGrafici_Raster_Cod,
                                                                          objParametri.InData.TipologiaLayer_Raster_Cod,
                                                                          objParametri.InData.LayerElementiGrafici_Cod,
                                                                          objParametri.InData.TipologiaLayer_Cod,
                                                                          objParametri.Utenti,
                                                                          objParametri.Server,
                                                                          objParametri.InData.Validita_Inizio,
                                                                          objParametri.InData.Validita_Fine)
                Case TipoOperazioneMaschera.UPDATE
                    resp.RispostaOK = xWrite.AggiornaMascheraLayerRaster(objParametri.InData.Maschera_Cod,
                                                                         objParametri.InData.Maschera_Des,
                                                                         objParametri.InData.LayerElementiGrafici_Raster_Cod,
                                                                         objParametri.InData.TipologiaLayer_Raster_Cod,
                                                                         objParametri.InData.LayerElementiGrafici_Cod,
                                                                         objParametri.InData.TipologiaLayer_Cod,
                                                                         objParametri.Utenti,
                                                                         objParametri.Server,
                                                                         objParametri.InData.Validita_Inizio,
                                                                         objParametri.InData.Validita_Fine)
                Case TipoOperazioneMaschera.DELETE
                    resp.RispostaOK = xWrite.EliminaMascheraLayerRaster(objParametri.InData.Maschera_Cod,
                                                                        objParametri.Utenti,
                                                                        objParametri.Server)

                Case Else
                    Throw New Exception("Tipologia operazione non consentita.")
            End Select

            If Not resp.RispostaOK Then
                Throw New Exception("Errore nell'esecuzione dell'operazione sulla maschera.")
            End If

            resp.RispostaStringa = "Operazione completata con successo."

        Catch ex As Exception

            resp.RispostaOK = False
            resp.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return resp
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiPermessiUtenteMaschera(ByVal InData As Object) As rispostaStandard(Of ElencoPermessiMaschera)

        Dim objParametri = DeserializzaInData(Of Int32)(InData)

        Gias_InizializzaCultura_DaParams(objParametri.Server, objParametri.Utenti)

        Dim resp As New rispostaStandard(Of ElencoPermessiMaschera)
        Dim xRead As New AgronicaCoreGisBIZ.Permessi_Maschera_R

        Try

            Dim elencoPermessiMaschera = xRead.LeggiPermessiUtenteByMascheraCod(objParametri.InData, objParametri.Server)

            resp.RispostaOK = True
            resp.RispostaStringa = elencoPermessiMaschera

        Catch ex As Exception

            resp.RispostaOK = False
            resp.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return resp
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiPermessiGruppiUtenteMaschera(ByVal InData As Object) As rispostaStandard(Of ElencoPermessiMaschera)

        Dim objParametri = DeserializzaInData(Of Int32)(InData)

        Gias_InizializzaCultura_DaParams(objParametri.Server, objParametri.Utenti)

        Dim resp As New rispostaStandard(Of ElencoPermessiMaschera)
        Dim xRead As New AgronicaCoreGisBIZ.Permessi_Maschera_R

        Try

            Dim elencoPermessiMaschera = xRead.LeggiPermessiGruppiUtenteByMascheraCod(objParametri.InData,
                                                                                            objParametri.Server,
                                                                                            objParametri.Utenti)

            resp.RispostaOK = True
            resp.RispostaStringa = elencoPermessiMaschera

        Catch ex As Exception

            resp.RispostaOK = False
            resp.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return resp
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function OperazioniPermessiMaschera(ByVal InData As Object) As RispostaStandard

        Dim objParametri = DeserializzaInData(Of ModifichePermessiMaschera)(InData)

        Gias_InizializzaCultura_DaParams(objParametri.Server, objParametri.Utenti)

        Dim resp As New RispostaStandard
        Dim xWrite As New AgronicaCoreGisBIZ.Permessi_Maschera_W

        Try
            resp.RispostaOK = xWrite.OperazioniPermessiMaschera(objParametri.InData, objParametri.Server)

            If Not resp.RispostaOK Then
                Throw New Exception("Errore nella gestione dei permessi della maschera.")
            End If

            resp.RispostaStringa = "Operazione completata con successo."

        Catch ex As Exception

            resp.RispostaOK = False
            resp.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return resp
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function AttivaDisattivaMaschera(ByVal InData As Object) As RispostaStandard

        Dim objParametri = DeserializzaInData(Of AttivazioneMascheraLayerRaster)(InData)

        Gias_InizializzaCultura_DaParams(objParametri.Server, objParametri.Utenti)

        Dim resp As New RispostaStandard
        Dim xWrite As New AgronicaCoreGisBIZ.MaschereLayerRaster_W
        Dim xRead As New AgronicaCoreGisBIZ.Permessi_Maschera_R

        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False

        Try

            Dim permessiMaschera = xRead.LeggiPermessiDaUtente(objParametri.Utenti, objParametri.Server, objParametri.InData.Maschera_Cod)

            Dim ObjUtenti As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
            Dim attivaMascheraFiltro As Boolean = False
            If objParametri.InData.Maschera_Cod = -1 Then
                attivaMascheraFiltro = ObjUtenti.Controlla_Permessi_Utente(objParametri.Utenti.UtenteUsername,
                                                                   TipiEnumerativi.enum_Id_Servizio.GiasOnline,
                                                                   TipiEnumerativi.enum_Security_Attivita.GIS_Gestione_Parametri_Maschere_Raster,
                                                                   TipiEnumerativi.enum_Security_Operazione.Modifica,
                                                                   Date.Now,
                                                                   "",
                                                                   objParametri.Utenti)
            End If

            If (Not attivaMascheraFiltro) And (
                permessiMaschera Is Nothing _
                   OrElse permessiMaschera.elencoPermessiUtente.Count = 0 _
                   OrElse permessiMaschera.elencoPermessiUtente.FirstOrDefault.elencoPermessiMaschera Is Nothing _
                   OrElse permessiMaschera.elencoPermessiUtente.FirstOrDefault.elencoPermessiMaschera.Count = 0 _
                   OrElse Not CBool(permessiMaschera.elencoPermessiUtente.FirstOrDefault.elencoPermessiMaschera.FirstOrDefault.Flag_Attivazione)
                   ) Then

                resp.RispostaOK = False
                resp.Errore = "NOT_ALLOWED"

                Return resp
            End If

            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale,
                                                                                    FlagTransazioneLocale,
                                                                                    objParametri.Server)

            resp.RispostaOK = xWrite.AttivaDisattivaMaschera(objParametri.InData.Maschera_Cod,
                                                             objParametri.InData.isAttivo,
                                                             objParametri.Server)

            If Not resp.RispostaOK Then
                Throw New Exception("Errore nell'attivazione/disattivazione della maschera.")
            End If

            If Not objParametri.Server.objTransazione Is Nothing Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri.Server)
            End If

            resp.RispostaStringa = "Operazione completata con successo."

        Catch ex As Exception

            If Not objParametri.Server.objTransazione Is Nothing Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri.Server)
            End If

            resp.RispostaOK = False
            resp.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        Finally
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri.Server)
        End Try

        Return resp
    End Function

    <WebMethod()>
    Public Function ElencoEntitaCodXRecuperoStaticMaps(InData As Object) As RispostaStandard
        Dim r As New RispostaStandard
        InData = JsonConvert.DeserializeObject(Of CoreWS_Generic(Of AgronicaCoreDTOStd.InData.Gis.ElencoEntitaCodXRecuperoStaticMaps))(JsonConvert.SerializeObject(InData))

        Try

            Dim iData As CoreWS_Generic(Of AgronicaCoreDTOStd.InData.Gis.ElencoEntitaCodXRecuperoStaticMaps) = JsonConvert.DeserializeObject(Of CoreWS_Generic(Of AgronicaCoreDTOStd.InData.Gis.ElencoEntitaCodXRecuperoStaticMaps))(JsonConvert.SerializeObject(InData))

            If iData.objP.objP_server = "" Then
                r.Errore = "objP_server non valorizzato"
                Return r
            End If

            If iData.objP.objP_utenti = "" Then
                r.Errore = "objP_utenti non valorizzato"
                Return r
            End If

            If iData.objP.objP_super_server = "" Then
                r.Errore = "objP_super_server non valorizzato"
                Return r
            End If

            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(iData.objP.objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(iData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(iData.objP.objP_utenti)

            Dim AppezzamentoBIZ As New AgronicaCoreAnagrafeBIZ.Appezzamento_W

            For Each entita In iData.InData.entitaList
                AppezzamentoBIZ.GeneraGMapJPG(entita, objParametri_Server)
            Next

            r.RispostaOK = True
            r.RispostaStringa = "elenco aggiornato"
        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try
        Return r

    End Function

    '<WebMethod()>
    '<Script.Services.ScriptMethod()>
    'Public Function TestElaborazioneAlgoritmo(ByVal InData As Object) As RispostaStandard

    '    Dim objParametri = DeserializzaInData(Of AttivazioneMascheraLayerRaster)(InData)

    '    Gias_InizializzaCultura_DaParams(objParametri.Server, objParametri.Utenti)

    '    Dim resp As New RispostaStandard
    '    Dim statoElaborazione As Int32
    '    Dim disattiva As Boolean = False

    '    Dim xRead As New AgronicaCoreGisBIZ.ProiezioniLayer_R
    '    Dim xWrite As New AgronicaCoreGisBIZ.ProiezioniLayer_W

    '    Dim DT As DataTable

    '    Dim algoritmoFactory As New Algoritmo_Factory

    '    DT = xRead.LeggiListaEsecuzioniAlgoritmi(objParametri.Server)

    '    If DT IsNot Nothing Then

    '        For Each row In DT.Rows

    '            Dim FlagTransazioneLocale As Boolean = False
    '            Dim FlagConnessioneLocale As Boolean = False

    '            Try
    '                AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale,
    '                                                                                        FlagTransazioneLocale,
    '                                                                                        objParametri.Server)

    '                Dim cfg As New JObject
    '                cfg.Add("GEE_BaseUrl_Async", "")
    '                cfg.Add("GEE_BaseUrl_Sync", "")
    '                cfg.Add("JSON_Token", "")

    '                Dim algoritmo As AgronicaAlgoritmiProiezione.IAlgoritmoProiezione
    '                algoritmo = algoritmoFactory.GetAlgoritmoDaCodice(CInt(row("LayerAnalysisConfig_Algorithm_Cod")),
    '                                                                  JObject.Parse(Newtonsoft.Json.JsonConvert.SerializeObject(cfg)),
    '                                                                  objParametri.Server)

    '                Try
    '                    resp.RispostaOK = algoritmo.Esegui(CInt(row("LayerAnalysisConfig_Cod")),
    '                                                       CInt(row("Entita_cod_1")),
    '                                                       CInt(row("Entita_cod_2")),
    '                                                       CInt(row("Entita_cod_Risultato")),
    '                                                       CInt(row("GIS_LayerAnalysisConfig_Exec_Log_Cod")),
    '                                                       objParametri.Server,
    '                                                       objParametri.Utenti,
    '                                                       objParametri.Super_Server,
    '                                                       True)

    '                    If Not resp.RispostaOK Then
    '                        disattiva = True
    '                    End If

    '                    statoElaborazione = 1
    '                Catch ex As Exception

    '                    statoElaborazione = 0
    '                    If CInt(row("Errori_Elaborazione")) + 1 = 5 Then
    '                        disattiva = True
    '                    End If
    '                End Try

    '                resp.RispostaOK = xWrite.AggiornaStatusEsecuzioneAlgoritmo(CInt(row("GIS_LayerAnalysisConfig_Exec_Log_Cod")),
    '                                                                statoElaborazione,
    '                                                                (CBool(statoElaborazione) Or disattiva),
    '                                                                objParametri.Server)

    '                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri.Server)

    '            Catch ex As Exception
    '                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri.Server)

    '                Throw ex
    '            Finally
    '                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri.Server)
    '            End Try

    '        Next
    '    End If

    '    Return resp

    'End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function NuovaElaborazionePianoConcimazioneGEE(ByVal InData As Object) As RispostaStandard

        Dim objParametri = DeserializzaInData(Of PianoConcimazioneGEE)(InData)

        Gias_InizializzaCultura_DaParams(objParametri.Server, objParametri.Utenti)

        Dim resp As New RispostaStandard
        Dim messaggistica As New AgronicaCoreMessaggisticaBIZ.Messaggi_Esecuzione_W

        If objParametri.InData.Messaggi.Equals("") Then
            Dim objPfRateoSrv = Newtonsoft.Json.JsonConvert.DeserializeObject(Of PfRateoSrv_In)(objParametri.InData.PfRateoSrv_In)

            Dim OUTPUT_Allegati_Documenti_Cod As Int32

            Dim ScriviSuAllegati As New AgronicaCoreAnagrafeBIZ.Allegati_Documenti_W

            resp = ScriviSuAllegati.PrecisionFarmingScriviSuAllegati(TipiEnumerativi.enum_CategorieDocumenti.PrecisionFarming_MappaPrescrizione,
                                                                     objPfRateoSrv.DescrizionePiano,
                                                                     "",
                                                                     objParametri.Server,
                                                                     objPfRateoSrv.ChiaveAlbero.Piva,
                                                                     objPfRateoSrv.ChiaveAlbero.Sa_Cod,
                                                                     objPfRateoSrv.ChiaveAlbero.Appezza,
                                                                     objPfRateoSrv.ChiaveAlbero.RicettaOperazione_Cod,
                                                                     objPfRateoSrv.ChiaveAlbero.Id_Imp,
                                                                     "",
                                                                     Nothing,
                                                                     System.Text.RegularExpressions.Regex.Unescape(objParametri.InData.Risultato_Elaborazione).Replace("###", "§"),
                                                                     OUTPUT_Allegati_Documenti_Cod
                                                                    )

            If Not resp.RispostaOK Then
                Return resp
            End If

            resp.RispostaOK = messaggistica.AccodaMessaggioEsecuzione(objParametri.InData.UtenteRichiedente, "Piano di concimazione calcolato con successo.", objParametri.Server)

            If Not resp.RispostaOK Then
                resp.RispostaStringa = ""
                resp.Errore = "Errore nell'accodamento del messaggio"
            End If
        Else
            resp.RispostaOK = messaggistica.AccodaMessaggioEsecuzione(objParametri.InData.UtenteRichiedente, objParametri.InData.Messaggi, objParametri.Server)

            If resp.RispostaOK Then
                resp.RispostaStringa = "Messaggio di errore accodato correttamente.."
            Else
                resp.Errore = "Errore nell'accodamento del messaggio di errore"
            End If
        End If

        Return resp

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function NuovaElaborazionePiattaformaGEE(ByVal InData As Object) As RispostaStandard

        Dim objParametri = DeserializzaInData(Of NuovaElaborazioneGEE)(InData)

        Gias_InizializzaCultura_DaParams(objParametri.Server, objParametri.Utenti)

        Dim resp As New RispostaStandard
        Dim xWrite As New AgronicaCoreGisBIZ.ProiezioniLayer_W

        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False

        Try

            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale,
                                                                                    FlagTransazioneLocale,
                                                                                    objParametri.Server)

            Dim xRead As New AgronicaCoreGisBIZ.ProiezioniLayer_R



            Dim esiste As Boolean = xRead.VerificaEsistenzaEsecuzione(objParametri.InData.elaborazione.GIS_LayerAnalysisConfig_Exec_Log_Cod_GUID, objParametri.Server)

            If Not esiste Then
                Throw New Exception("Esecuzione non trovata.")
            End If



            'DEBUG DEBUG DEBUG
            'Intersezione
            'objParametri.InData.elaborazione.Risultato_Elaborazione = "{""geoJsonPolygon"": {""type"": ""FeatureCollection"",""features"": [{""type"": ""Feature"",""geometry"": null,""properties"": {""GIS_LayerAnalysisConfig_Exec_Log_GUID"": ""3a7ceb23-f0b4-4f59-96d1-e1a53da1928c"",""LayerAnalysisConfig_Cod_GUID"": ""09db9b11-6556-4b7f-a1a5-3819961f9ff7"",""AttivaSuTuttiLayer"": false,""LayerAnalysisConfig_Response"": [{""Entita_GUID_1"": ""af3dc5e4-b895-4dee-ae00-11e2d618838d"",""Entita_GUID_2"": null,""Entita_GUID_Risultato"": null,""RisultatoElaborazione"": {""CoveragePercentage"": 12.3}}]}}]}}"

            'Mappe Satellitari
            'objParametri.InData.elaborazione.Risultato_Elaborazione = "{""geoJsonPolygon"": {""type"": ""FeatureCollection"",""features"": [{""type"": ""Feature"",""geometry"": null,""properties"": {""GIS_LayerAnalysisConfig_Exec_Log_GUID"": ""1747f2ec-6f5f-4ef5-9d0f-31c33c81b766"",""LayerAnalysisConfig_Cod_GUID"": ""44f94552-4184-4281-bdcb-0187d1c0b6b1"",""AttivaSuTuttiLayer"": false,""LayerAnalysisConfig_Response"": [{""Entita_GUID_1"": ""ad52a7f1-71e8-4565-bb9b-a4d928a6c05f"",""Entita_GUID_2"": null,""Entita_GUID_Risultato"": null,""RisultatoElaborazione"": [{""DataRiferimento"": ""17/06/2023"",""Passaggi"": [{""Tile"": {""Tile"": ""T32TQQ"",""PoligonoWktBoundingBox"": null,""GEORiferimento_COD"": ""13""},""url"": ""S2B_MSIL1C_20230617T100559_N0509_R022_T32TQQ_20230617T121117"",""Sensore"": [{""CodiceSensore"": ""NDVI"",""Descrizione"": ""NDVI"",""DatiRilevati"": null},{""CodiceSensore"": ""NDWI_GAO"",""Descrizione"": ""NDWI_GAO"",""DatiRilevati"": null},{""CodiceSensore"": ""NDWI_McFeeters"",""Descrizione"": ""NDWI_McFeeters"",""DatiRilevati"": null},{""CodiceSensore"": ""RGB"",""Descrizione"": ""RGB"",""DatiRilevati"": null}]}]},{""DataRiferimento"": ""12/06/2023"",""Passaggi"": [{""Tile"": {""Tile"": ""T32TQQ"",""PoligonoWktBoundingBox"": null,""GEORiferimento_COD"": ""13""},""url"": ""S2A_MSIL1C_20230612T100601_N0509_R022_T32TQQ_20230612T135006"",""Sensore"": [{""CodiceSensore"": ""NDVI"",""Descrizione"": ""NDVI"",""DatiRilevati"": null},{""CodiceSensore"": ""NDWI_GAO"",""Descrizione"": ""NDWI_GAO"",""DatiRilevati"": null},{""CodiceSensore"": ""NDWI_McFeeters"",""Descrizione"": ""NDWI_McFeeters"",""DatiRilevati"": null},{""CodiceSensore"": ""RGB"",""Descrizione"": ""RGB"",""DatiRilevati"": null}]}]},{""DataRiferimento"": ""02/06/2023"",""Passaggi"": [{""Tile"": {""Tile"": ""T32TQQ"",""PoligonoWktBoundingBox"": null,""GEORiferimento_COD"": ""13""},""url"": ""S2A_MSIL1C_20230602T100601_N0509_R022_T32TQQ_20230602T135148"",""Sensore"": [{""CodiceSensore"": ""NDVI"",""Descrizione"": ""NDVI"",""DatiRilevati"": null},{""CodiceSensore"": ""NDWI_GAO"",""Descrizione"": ""NDWI_GAO"",""DatiRilevati"": null},{""CodiceSensore"": ""NDWI_McFeeters"",""Descrizione"": ""NDWI_McFeeters"",""DatiRilevati"": null},{""CodiceSensore"": ""RGB"",""Descrizione"": ""RGB"",""DatiRilevati"": null}]}]},{""DataRiferimento"": ""23/05/2023"",""Passaggi"": [{""Tile"": {""Tile"": ""T32TQQ"",""PoligonoWktBoundingBox"": null,""GEORiferimento_COD"": ""13""},""url"": ""S2A_MSIL1C_20230523T100601_N0509_R022_T32TQQ_20230523T135147"",""Sensore"": [{""CodiceSensore"": ""NDVI"",""Descrizione"": ""NDVI"",""DatiRilevati"": null},{""CodiceSensore"": ""NDWI_GAO"",""Descrizione"": ""NDWI_GAO"",""DatiRilevati"": null},{""CodiceSensore"": ""NDWI_McFeeters"",""Descrizione"": ""NDWI_McFeeters"",""DatiRilevati"": null},{""CodiceSensore"": ""RGB"",""Descrizione"": ""RGB"",""DatiRilevati"": null}]}]}]}]}}]}}"
            'DEBUG DEBUG DEBUG

            Dim outputObj = JObject.Parse(System.Text.RegularExpressions.Regex.Unescape(objParametri.InData.elaborazione.Risultato_Elaborazione))


            'Lavez - 14/05/2025 - Ticket 175749

            Dim featureList = CType(outputObj("geoJsonPolygon")("features"), JArray)
            If featureList.Count > 0 Then
                For Each feature In featureList
                    If xRead.VerificaEntitaRisultatoGEE(CType(feature("properties")("LayerAnalysisConfig_Response"), JArray), objParametri.Server) = True Then


                        resp.RispostaOK = xWrite.InserisciLogEsecuzioneAlgoritmoGUID(feature("properties"),
                                                                                     objParametri.InData.elaborazione.Risultato_Elaborazione,
                                                                                     objParametri.InData.elaborazione.Messaggi,
                                                                                     objParametri.Server)

                        If Not resp.RispostaOK Then
                            Throw New Exception("Errore nell'inserimento del log esecuzione.")
                        End If

                        resp.RispostaOK = EseguiOperazioniPostLog(objParametri.InData.elaborazione.GIS_LayerAnalysisConfig_Exec_Log_Cod_GUID,
                                                                  objParametri.InData.elaborazione.Risultato_Elaborazione,
                                                                  objParametri.Server, objParametri.Utenti)
                    Else
                        resp.RispostaStringa = "Una o più Entita non sono più presenti in GIAS. Elaborazione non salvata ma chiusa con stato OK per non bloccare la catena di notifiche"

                    End If
                Next

                resp.RispostaStringa = "Operazione completata con successo."
            Else
                resp.RispostaStringa = "Elaborazione senza dettagli. Elaborazione non salvata ma chiusa con stato OK per non bloccare la catena di notifiche"

            End If


            If Not objParametri.Server.objTransazione Is Nothing Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri.Server)
            End If
        Catch ex As Exception

            If Not objParametri.Server.objTransazione Is Nothing Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri.Server)
            End If

            resp.RispostaOK = False
            resp.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        Finally
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri.Server)
        End Try

        Return resp
    End Function
    
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function NuovaElaborazionePiattaformaSAT(ByVal InData As Object) As RispostaStandard

        Dim objParametri = DeserializzaInData(Of SatCloudEvent)(InData)

        Gias_InizializzaCultura_DaParams(objParametri.Server, objParametri.Utenti)

        Dim resp As New RispostaStandard
        Dim xWrite As New AgronicaCoreGisBIZ.ProiezioniLayer_W

        Dim flagTransazioneLocale = False
        Dim flagConnessioneLocale = False

        Try

            ConnessioniTransazioni.ApriConnessioneXCoreBiz(flagConnessioneLocale, flagTransazioneLocale, objParametri.Server)

            Dim xRead As New AgronicaCoreGisBIZ.ProiezioniLayer_R
            Dim executionDt = xRead.LeggiEsecuzioneDaGUID(objParametri.InData.Data.PolygonId, objParametri.Server)
            If executionDt is Nothing Or executionDt.Rows.Count <> 1 Then
                Throw New Exception("Esecuzione non trovata.")
            End If

            Dim execution = executionDt.Rows(0)
            
            ' Phase 1: Fetch SAT configuration (base URL, tenant, apiKey)
            Dim cfgRead As New AgronicaCoreVarieDAL.Configurazione_Siti_R

            Dim satBaseUrl = cfgRead.Leggi_Valore_ServerESuperServer(0, SAT_BaseUrl_ConfKey, "", "", objParametri.Server, objParametri.Super_Server)
            Dim satTenant = cfgRead.Leggi_Valore(0, SAT_TenantName_ConfKey, "", "", objParametri.Server)
            Dim satApiKey = cfgRead.Leggi_Valore(0, SAT_ApiKey_ConfKey, "", "", objParametri.Server)

            Dim satPixelCoverageThreshold As Decimal = 1
            Dim satPixelCoverageThresholdRaw = cfgRead.Leggi_Valore(0, SAT_PixelCoverageThreshold_ConfKey, "", "", objParametri.Server)
            Dim parsedPixelCoverage As Decimal
            If Not String.IsNullOrEmpty(satPixelCoverageThresholdRaw) AndAlso
               Decimal.TryParse(satPixelCoverageThresholdRaw, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, parsedPixelCoverage) Then
                satPixelCoverageThreshold = parsedPixelCoverage
            End If

            ' Phase 2: Retrieve required values from the cloud event
            Dim polygonId As String = objParametri.InData.Data.PolygonId
            Dim indexCode As String = objParametri.InData.Data.IndexCode
            Dim lastDate As Date = objParametri.InData.Data.LastDate

            If String.IsNullOrEmpty(polygonId) Then
                Throw New Exception("PolygonId non valorizzato")
            End If
            If String.IsNullOrEmpty(indexCode) Then
                Throw New Exception("indexCode non valorizzato")
            End If
            
            Dim xReadEntita As New AgronicaCoreGisBIZ.GIS_Entita_R
            Dim dtValidita = xReadEntita.LeggiValiditaImpiantoDaEntitaCod(CInt(execution("Entita_cod_1")), objParametri.Server)

            Dim dateTo As String = lastDate.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ")
            Dim dateFrom As String = IIf(
                dtValidita.Rows.Count > 0,
                CDate(dtValidita.Rows(0)("Validita_Inizio")).ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ"),
                dateTo
                )

            ' Phase 3: Call SAT API and parse the response
            Dim satRawResponse As String = FetchPolygonIndexStats(satBaseUrl, satTenant, satApiKey, polygonId, indexCode, dateFrom, dateTo)
            Dim satParsed As RisultatoElaborazione = ParseIndexesStatsResponse(satRawResponse, satPixelCoverageThreshold)

            ' Phase 4: Persist the result into the execution log details so that the UI can read it later
            Dim dtEsecuzione As DataTable = xRead.LeggiEsecuzioneDaGUID(polygonId, objParametri.Server)
            
            Dim xReadProiezione As New ProiezioniLayer_R
            Dim dtLayerAnalysisConfig = xReadProiezione.LeggiConfigurazioneDaCodice(dtEsecuzione.Rows(0)("LayerAnalysisConfig_Cod").ToString(), objParametri.Server)
            
            Dim entityReader = New GIS_Entita_R
            Dim entitaGuid = entityReader.LeggiEntitaGuidDaEntitaCod(CInt(dtEsecuzione.Rows(0)("Entita_cod_1")), objParametri.Server)

            If dtLayerAnalysisConfig Is Nothing Or dtLayerAnalysisConfig.Rows.Count = 0 Then
                Throw New Exception("Layer analysis config not found")
            End If
            
            Dim layerAnalysisConfigGuid = dtLayerAnalysisConfig.Rows(0)("LayerAnalysisConfig_GUID").ToString()
            Dim featureProperties As New JObject From {
                {"GIS_LayerAnalysisConfig_Exec_Log_GUID", polygonId},
                {"LayerAnalysisConfig_GUID", layerAnalysisConfigGuid},
                {"AttivaSuTuttiLayer", False},
                {"LayerAnalysisConfig_Response", New JArray(New JObject From {
                    {"Entita_GUID_1", entitaGuid.Rows(0)("Entita_GUID").ToString()},
                    {"Entita_GUID_2", Nothing},
                    {"Entita_GUID_Risultato", Nothing},
                    {"RisultatoElaborazione", JToken.FromObject(satParsed.RisultatoElaborazione)}
                })}
            }

            Dim resultJson As String = JsonConvert.SerializeObject(New JObject From {
                {"geoJsonPolygon", New JObject From {
                    {"type", "FeatureCollection"},
                    {"features", New JArray(New JObject From {
                        {"type", "Feature"},
                        {"geometry", Nothing},
                        {"properties", featureProperties}
                    })}
                }}
            })

            resp.RispostaOK = xWrite.InserisciLogEsecuzioneAlgoritmoGuidSat(featureProperties, resultJson, "", objParametri.Server)
            If Not resp.RispostaOK Then
                Throw New Exception("Errore nell'inserimento del log esecuzione SAT.")
            End If

            resp.RispostaStringa = "Operazione completata con successo."

            If Not objParametri.Server.objTransazione Is Nothing Then
                ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(flagTransazioneLocale, objParametri.Server)
            End If
        Catch ex As Exception

            If Not objParametri.Server.objTransazione Is Nothing Then
                ConnessioniTransazioni.ChiudiTransazione(2, objParametri.Server)
            End If

            resp.RispostaOK = False
            resp.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        Finally
            ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(flagConnessioneLocale, objParametri.Server)
        End Try

        Return resp
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function NuovaElaborazioneMappePrescrizione(ByVal InData As Object) As RispostaStandard

        Dim objParametri = DeserializzaInData(Of MappePrescrizioneCloudEvent)(InData)

        Gias_InizializzaCultura_DaParams(objParametri.Server, objParametri.Utenti)

        Dim resp As New RispostaStandard
        Dim flagConnessioneLocale = False
        Dim flagTransazioneLocale = False

        Try
            Dim requestId As String = objParametri.InData.RequestId
            If String.IsNullOrEmpty(requestId) Then
                Throw New Exception("request_id non valorizzato nel payload CloudEvent.")
            End If

            Dim status As String = objParametri.InData.Data?.Status
            If Not String.Equals(status, "DONE", StringComparison.OrdinalIgnoreCase) Then
                resp.RispostaOK = True
                resp.RispostaStringa = "Webhook ricevuto. Status=" & status & ". Nessuna azione eseguita."
                Return resp
            End If

            Dim assetLocation As String = objParametri.InData.Data?.Result?.AssetLocation
            If String.IsNullOrEmpty(assetLocation) Then
                Throw New Exception("asset_location non valorizzato nel payload.")
            End If

            ConnessioniTransazioni.ApriConnessioneXCoreBiz(flagConnessioneLocale, flagTransazioneLocale, objParametri.Server)

            Dim leggiRichiesta As New RichiesteEngine_Impianto_Ricetta_R
            Dim dtRichiesta = leggiRichiesta.LeggiByRequestId(requestId, objParametri.Server)
            If dtRichiesta Is Nothing OrElse dtRichiesta.Rows.Count = 0 Then
                Throw New Exception("Nessuna richiesta trovata per request_id: " & requestId)
            End If

            Dim rigaRichiesta = dtRichiesta.Rows(0)
            Dim piva As String = rigaRichiesta("piva").ToString()
            Dim saCod = CInt(rigaRichiesta("sa_cod"))
            Dim appezza = CInt(rigaRichiesta("appezza"))
            Dim idImp = CInt(rigaRichiesta("id_imp"))
            Dim ricettaOperazioneCod = CInt(rigaRichiesta("ricetta_operazione_cod"))

            Dim allegatiLeggi As New AgronicaCoreAnagrafeDAL.Allegati_EntitaxDocumenti_R
            Dim dtEsistente = allegatiLeggi.Leggi(0,
                                                  piva,
                                                  saCod,
                                                  0,
                                                  appezza,
                                                  idImp,
                                                  0,
                                                  "",
                                                  "",
                                                  "",
                                                  0,
                                                  0,
                                                  "",
                                                  "",
                                                  "",
                                                  0,
                                                  0,
                                                  0,
                                                  $"Allegati_Documenti.Allegati_Documenti_CatCod = {CInt(TipiEnumerativi.enum_CategorieDocumenti.PrecisionFarming_MappaPrescrizione)}",
                                                  "",
                                                  objParametri.Server)
            If dtEsistente IsNot Nothing AndAlso dtEsistente.Rows.Count > 0 Then
                resp.RispostaOK = True
                resp.RispostaStringa = "Raster già presente in documentale. Operazione saltata."
                Return resp
            End If

            Dim confReader As New AgronicaCoreVarieDAL.Configurazione_Siti_R
            Dim urlBase = confReader.Leggi_Valore_ServerESuperServer(0, MappePrescrizione_BaseUrl_ConfKey, "", "", objParametri.Server, objParametri.Super_Server)
            Dim apiKey = confReader.Leggi_Valore(0, MappePrescrizione_ApiKey_ConfKey, "", "", objParametri.Server)
            If String.IsNullOrEmpty(urlBase) Then
                Throw New Exception("Configurazione URL Engine Mappe Prescrizione non trovata.")
            End If
            If String.IsNullOrEmpty(apiKey) Then
                Throw New Exception("Configurazione API Key Engine Mappe Prescrizione non trovata.")
            End If

            Dim downloadUrl As String = "https://" & urlBase & "/" & assetLocation.TrimStart("/")
            Dim rasterBytes As Byte()
            Using httpClient As New HttpClient()
                httpClient.Timeout = TimeSpan.FromMinutes(5)
                httpClient.DefaultRequestHeaders.Add("Authorization", $"APIKEY {apiKey}")
                Dim httpResp = httpClient.GetAsync(downloadUrl).GetAwaiter().GetResult()
                If Not httpResp.IsSuccessStatusCode Then
                    Throw New Exception("Download raster fallito: status " & httpResp.StatusCode & ", url: " & downloadUrl)
                End If
                rasterBytes = httpResp.Content.ReadAsByteArrayAsync().GetAwaiter().GetResult()
            End Using

            If rasterBytes Is Nothing OrElse rasterBytes.Length = 0 Then
                Throw New Exception("Il file raster scaricato è vuoto (0 bytes).")
            End If

            Dim outputAllegatiCod = 0
            ManageFileMappePrescrizione(
                requestId,
                piva,
                saCod,
                appezza,
                idImp,
                ricettaOperazioneCod,
                rasterBytes,
                objParametri.Server,
                objParametri.Super_Server,
                outputAllegatiCod
                )

            Dim writeNotifications = confReader.Leggi_Valore(0, MappePrescrizione_WriteNotifications_ConfKey, "", "", objParametri.Server)
            If String.Equals(writeNotifications, "true", StringComparison.OrdinalIgnoreCase) Then
                Dim messaggistica As New AgronicaCoreMessaggisticaBIZ.Messaggi_Esecuzione_W
                messaggistica.AccodaMessaggioEsecuzione(objParametri.Server.UtenteUsername, "Mappa di prescrizione archiviata con successo.", objParametri.Server)
            End If

            If objParametri.Server.objTransazione IsNot Nothing Then
                ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(flagTransazioneLocale, objParametri.Server)
            End If

            resp.RispostaOK = True
            resp.RispostaStringa = "Elaborazione completata. Allegato_Cod " & outputAllegatiCod

        Catch ex As Exception
            If objParametri.Server.objTransazione IsNot Nothing Then
                ConnessioniTransazioni.ChiudiTransazione(2, objParametri.Server)
            End If
            resp.RispostaOK = False
            resp.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        Finally
            ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(flagConnessioneLocale, objParametri.Server)
        End Try

        Return resp
    End Function

    Private Function EseguiOperazioniPostLog(ByVal GIS_LayerAnalysisConfig_Exec_Log_Cod_GUID As String,
                                             ByVal risultato_Elaborazione As String,
                                             ByRef objParametri_Server As AgronicaCoreParametri,
                                             ByRef objParametri_Utenti As AgronicaCoreParametri) As Boolean

        Dim resp As Boolean = True
        Dim newIDEntita As Int32 = 0

        Dim xRead As New AgronicaCoreGisBIZ.ProiezioniLayer_R
        Dim xWrite As New AgronicaCoreGisBIZ.ProiezioniLayer_W

        Dim alg_ext As New AgronicaAlgoritmiProiezione.Algorithm_Extension
        Dim msg As String = ""

        Dim esecuzione As DataTable = xRead.LeggiEsecuzioneDaGUID(GIS_LayerAnalysisConfig_Exec_Log_Cod_GUID, objParametri_Server)
        Dim configurazione As DataTable = xRead.LeggiConfigurazioneDaCodice(CInt(esecuzione.Rows(0)("LayerAnalysisConfig_Cod")), objParametri_Server)

        Select Case CInt(configurazione.Rows(0)("LayerAnalysisConfig_Algorithm_Cod"))
            Case TipiEnumerativi.enum_AlgoritmoProiezione.Potential_Deforestation_Yearly
                resp = xWrite.SalvaElementoGraficoIntersezioneDaRisultatoGEE(CInt(esecuzione.Rows(0)("GIS_LayerAnalysisConfig_Exec_Log_Cod")),
                                                                             CInt(esecuzione.Rows(0)("LayerAnalysisConfig_Cod")),
                                                                             JObject.Parse(risultato_Elaborazione),
                                                                             newIDEntita,
                                                                             objParametri_Server,
                                                                             objParametri_Utenti)

                If Not resp Then
                    Throw New Exception("Errore nel salvataggio dell'elemento grafico dell'intersezione.")
                End If

                If esecuzione.Rows(0)("ParametriElaborazione") IsNot DBNull.Value AndAlso Not CStr(esecuzione.Rows(0)("ParametriElaborazione")).Equals("") Then
                    'Lavez - 05/08/2024 - gestione estensioni algoritmi di proiezione
                    Dim algExt As New Algorithm_Extension
                    algExt.AlgorithmRaster_Extension(CInt(esecuzione.Rows(0)("LayerAnalysisConfig_Cod")),
                                                     CInt(esecuzione.Rows(0)("GIS_LayerAnalysisConfig_Exec_Log_Cod")),
                                                     GIS_LayerAnalysisConfig_Exec_Log_Cod_GUID,
                                                     CStr(esecuzione.Rows(0)("ParametriElaborazione")),
                                                     JObject.Parse(risultato_Elaborazione),
                                                     objParametri_Server)
                End If

            Case TipiEnumerativi.enum_AlgoritmoProiezione.Caricamento_Su_Piattaforma_GEE

                resp = xWrite.ValorizzaParametriVisibilitaEntita(CInt(esecuzione.Rows(0)("Entita_cod_1")), JObject.Parse(risultato_Elaborazione), objParametri_Server)

                If Not resp Then
                    Throw New Exception("Errore nel salvataggio dei parametri di visibilità dell'entità.")
                End If

                If esecuzione.Rows(0)("ParametriElaborazione") IsNot DBNull.Value AndAlso Not CStr(esecuzione.Rows(0)("ParametriElaborazione")).Equals("") Then
                    'Lavez - 05/08/2024 - gestione estensioni algoritmi di proiezione
                    Dim algExt As New Algorithm_Extension
                    algExt.AlgorithmVector_Extension(CInt(esecuzione.Rows(0)("LayerAnalysisConfig_Cod")),
                                                     CInt(esecuzione.Rows(0)("GIS_LayerAnalysisConfig_Exec_Log_Cod")),
                                                     GIS_LayerAnalysisConfig_Exec_Log_Cod_GUID,
                                                     CStr(esecuzione.Rows(0)("ParametriElaborazione")),
                                                     risultato_Elaborazione,
                                                     msg,
                                                     objParametri_Server)
                End If

        End Select

        Return resp
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiAllegatiLayer(ByVal InData As Object) As rispostaStandard(Of ElencoAllegatiLayer_Out)

        Dim objParametri = DeserializzaInData(Of LeggiDatiLayer_In)(InData)

        Gias_InizializzaCultura_DaParams(objParametri.Server, objParametri.Utenti)

        Dim resp As New rispostaStandard(Of ElencoAllegatiLayer_Out)
        Dim xRead As New AgronicaCoreGisBIZ.GIS_LayerElementiGrafici

        Try
            Dim ObjUtenti As New AgronicaCoreUtentiDAL.Utenti_Permessi_R

            Dim permessiOperazione As Boolean = ObjUtenti.Controlla_Permessi_Utente(
                                                objParametri.Utenti.UtenteUsername,
                                                enum_Id_Servizio.GiasOnline,
                                                enum_Security_Attivita.Cartografia_Esporta_Dati,
                                                enum_Security_Operazione.Lettura,
                                                Date.Now,
                                                "",
                                                objParametri.Utenti)

            If Not permessiOperazione Then
                Throw New Exception("INVALID_USER")
            End If

            resp.RispostaOK = True
            resp.RispostaStringa = xRead.LeggiElencoAllegatiLayer(objParametri.InData.LayerElementiGrafici_Cod,
                                                                  objParametri.InData.TipologiaLayer_Cod,
                                                                  objParametri.Server,
                                                                  objParametri.Utenti)
        Catch ex As Exception

            resp.RispostaOK = False
            resp.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return resp
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiAllegatoDocumento(ByVal InData As Object) As rispostaStandard(Of AllegatoFile)

        Dim objParametri = DeserializzaInData(Of Int32)(InData)

        Gias_InizializzaCultura_DaParams(objParametri.Server, objParametri.Utenti)

        Dim resp As New rispostaStandard(Of AllegatoFile)
        Dim xRead As New AgronicaCoreGisBIZ.GIS_LayerElementiGrafici

        Try

            Dim ObjUtenti As New AgronicaCoreUtentiDAL.Utenti_Permessi_R

            Dim permessiOperazione As Boolean = ObjUtenti.Controlla_Permessi_Utente(
                                                objParametri.Utenti.UtenteUsername,
                                                enum_Id_Servizio.GiasOnline,
                                                enum_Security_Attivita.Cartografia_Esporta_Dati,
                                                enum_Security_Operazione.Lettura,
                                                Date.Now,
                                                "",
                                                objParametri.Utenti)

            If Not permessiOperazione Then
                Throw New Exception("INVALID_USER")
            End If

            resp.RispostaOK = True
            resp.RispostaStringa = xRead.LeggiAllegatoFile(objParametri.InData, objParametri.Server, objParametri.Utenti)

        Catch ex As Exception

            resp.RispostaOK = False
            resp.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return resp
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function EsportaShapeEntita(ByVal InData As Object) As RispostaStandard

        Dim objParametri = DeserializzaInData(Of EsportaShapeEntita_In)(InData)

        Gias_InizializzaCultura_DaParams(objParametri.Server, objParametri.Utenti)

        Dim resp As New RispostaStandard
        Dim xRead As New AgronicaCoreGisBIZ.GIS_Entita_R
        Dim xWriteEntita As New AgronicaCoreGisBIZ.GIS_Entita_W
        Dim xWrite As New AgronicaCoreGisBIZ.ProiezioniLayer_W
        Dim messaggistica As New AgronicaCoreMessaggisticaBIZ.Messaggi_Esecuzione_W

        Try

            Dim ObjUtenti As New AgronicaCoreUtentiDAL.Utenti_Permessi_R

            Dim permessiOperazione As Boolean = ObjUtenti.Controlla_Permessi_Utente(
                                            objParametri.Utenti.UtenteUsername,
                                            enum_Id_Servizio.GiasOnline,
                                            enum_Security_Attivita.Cartografia_Esporta_Dati,
                                            enum_Security_Operazione.Modifica,
                                            Date.Now,
                                            "",
                                            objParametri.Utenti)


            Dim permessoEsportazioneBulk As Boolean = ObjUtenti.Controlla_Permessi_Utente(
                                            objParametri.Utenti.UtenteUsername,
                                            enum_Id_Servizio.GiasOnline,
                                            enum_Security_Attivita.GisBulkExportSuLayer,
                                            enum_Security_Operazione.Modifica,
                                            Date.Now,
                                            "",
                                            objParametri.Utenti)


            If Not permessiOperazione Then
                Throw New Exception("INVALID_USER")
            End If

            If objParametri.InData.fullLayerExport And Not permessoEsportazioneBulk Then
                Throw New Exception("INVALID_USER")
            End If

            If objParametri.InData.fullLayerExport Then
                resp.RispostaOK = xWrite.AccodaEsecuzioneEsportazione(objParametri.InData, objParametri.Server)
            Else
                xWriteEntita.CleanCompletedExportEntitiesRecords(objParametri.Server)
                Dim algoritmoEsportazioneBulk As New EsportazioneBulkLayer(enum_AlgoritmoProiezione.EsportazioneBulkLayer,
                                                                           enum_TipoAlgoritmoProiezione.EsportazioneBulkLayer)

                Dim parametriEsecuzione As JObject = JObject.FromObject(objParametri.InData)

                Dim elencoEntita As New List(Of Integer)
                For Each token In CType(parametriEsecuzione("elencoEntita"), JArray)
                    elencoEntita.Add(CInt(token))
                Next

                Dim idEsp = xWriteEntita.SaveExportEntities(elencoEntita, objParametri.Server)
                objParametri.InData.idEsp = idEsp
                objParametri.InData.elencoEntita = New List(Of Integer)

                resp.RispostaOK = xWrite.AccodaEsecuzioneEsportazione(objParametri.InData, objParametri.Server)
            End If

            If Not resp.RispostaOK Then
                Throw New Exception("Errore durante l'accodamento della richiesta di esportazione delle entità.")
            End If

            resp.RispostaStringa = String.Format("Elaborazione avviata. Vedrai comparire in tabella la riga dell'estrazione ""{0}"" per il download. L'operazione può richiedere da pochi secondi ad alcune ore in base al numero di poligoni, ti consigliamo di ritornare su questa scheda successivamente.", objParametri.InData.descrizioneEsportazione)

        Catch ex As Exception

            resp.RispostaOK = False
            resp.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

            If Not ex.Message.Equals("INVALID_USER") Then
                messaggistica.AccodaMessaggioEsecuzione(objParametri.Server.UtenteUsername, ex.Message, objParametri.Server)
            End If
        End Try

        Return resp
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function ModificaEstrazioneShape(ByVal InData As Object) As RispostaStandard

        Dim objParametri = DeserializzaInData(Of AllegatoLayerModifica)(InData)

        Gias_InizializzaCultura_DaParams(objParametri.Server, objParametri.Utenti)

        Dim resp As New RispostaStandard
        Dim xWrite As New AgronicaCoreGisBIZ.GIS_LayerElementiGrafici

        Try
            Dim ObjUtenti As New AgronicaCoreUtentiDAL.Utenti_Permessi_R

            Dim permessiOperazione As Boolean = ObjUtenti.Controlla_Permessi_Utente(
                                                objParametri.Utenti.UtenteUsername,
                                                enum_Id_Servizio.GiasOnline,
                                                enum_Security_Attivita.Cartografia_Esporta_Dati,
                                                enum_Security_Operazione.Modifica,
                                                Date.Now,
                                                "",
                                                objParametri.Utenti)

            If Not permessiOperazione Then
                Throw New Exception("INVALID_USER")
            End If

            resp.RispostaOK = xWrite.ModificaEstrazioneShape(objParametri.InData, objParametri.Server)
            resp.RispostaStringa = "Operazione completata con successo."

        Catch ex As Exception

            resp.RispostaOK = False
            resp.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return resp
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiBaseUrlMappeSatellitari(ByVal InData As Object) As rispostaStandard(Of EndpointMappeSatellitari)

        Dim objParametri = DeserializzaInData(Of String)(InData)

        Gias_InizializzaCultura_DaParams(objParametri.Server, objParametri.Utenti)

        Dim resp As New rispostaStandard(Of EndpointMappeSatellitari)

        Dim xRead As New AgronicaCoreVarieBIZ.Configurazione_Siti_BIZ_R

        Try
            resp.RispostaOK = True
            resp.RispostaStringa = xRead.leggiConfigurazioneEndpointMappeSatellitari(objParametri.Utenti, objParametri.Server)

        Catch ex As Exception

            resp.RispostaOK = False
            resp.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return resp
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function PermessiUtenteSuSingoloLayer(ByVal InData As Object) As rispostaStandard(Of LeggiPermessiLayerUtenti_Out)

        Dim objParametri = DeserializzaInData(Of PermessiUtenteSuSingoloLayer_In)(InData)

        Gias_InizializzaCultura_DaParams(objParametri.Server, objParametri.Utenti)

        Dim resp As New rispostaStandard(Of LeggiPermessiLayerUtenti_Out)
        Dim xReadPermessi As New AgronicaCoreGisBIZ.GIS_Permessi_Layer_R

        Try

            Dim elencoPermessi = xReadPermessi.LeggiPermessiDaUtente(objParametri.InData.LayerElementiGrafici_Cod,
                                                                     objParametri.Utenti,
                                                                     objParametri.Server,
                                                                     objParametri.InData.user)

            resp.RispostaOK = True
            resp.RispostaStringa = elencoPermessi

        Catch ex As Exception

            resp.RispostaOK = False
            resp.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return resp
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function EliminazioneTotaleDatiLayer(ByVal InData As Object) As RispostaStandard

        Dim objParametri = DeserializzaInData(Of Int32)(InData)

        Gias_InizializzaCultura_DaParams(objParametri.Server, objParametri.Utenti)

        Dim resp As New RispostaStandard
        Dim xWrite As New AgronicaCoreGisBIZ.GIS_LayerElementiGrafici

        Try

            resp = xWrite.EliminazioneTotaleDatiLayer(objParametri.InData, objParametri.Server)

            If resp.RispostaOK Then resp.RispostaStringa = "Operazione completata con successo."

        Catch ex As Exception

            resp.RispostaOK = False
            resp.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return resp
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function OperazioniBookmark(ByVal InData As Object) As RispostaStandard

        Dim objParametri = DeserializzaInData(Of OperazioniBookmark_In)(InData)

        Gias_InizializzaCultura_DaParams(objParametri.Server, objParametri.Utenti)

        Dim resp As New RispostaStandard
        Dim xWrite As New AgronicaCoreGisBIZ.GIS_Bookmark_W

        Try

            Select Case objParametri.InData.TipoOperazione
                Case enum_Tipo_Operazione_Bookmark.INSERT

                    If objParametri.InData.bookmark.Bookmark_Cod <> 0 Then
                        Throw New Exception("Impossibile specificare un ID per un nuovo Bookmark.")
                    End If

                    resp = xWrite.SalvaNuovoBookmark(objParametri.InData.bookmark,
                                                     objParametri.Server)

                Case enum_Tipo_Operazione_Bookmark.UPDATE

                    If objParametri.InData.bookmark.Bookmark_Cod = 0 Then
                        Throw New Exception("Impossibile modificare un Bookmark senza un ID.")
                    End If

                    resp = xWrite.AggiornaBookmark(objParametri.InData.bookmark,
                                                   objParametri.Server)

                Case enum_Tipo_Operazione_Bookmark.DELETE

                    If objParametri.InData.bookmark.Bookmark_Cod = 0 Then
                        Throw New Exception("Impossibile eliminare un Bookmark senza un ID.")
                    End If

                    resp = xWrite.EliminaBookmark(objParametri.InData.bookmark.Bookmark_Cod, objParametri.Server)

                Case Else
                    Throw New Exception("Operazione non riconosciuta.")
            End Select

            If resp.RispostaOK Then resp.RispostaStringa = "Operazione completata con successo."

        Catch ex As Exception

            resp.RispostaOK = False
            resp.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return resp
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiBookmark(ByVal InData As Object) As rispostaStandard(Of List(Of Bookmark))

        Dim objParametri = DeserializzaInData(Of Int32)(InData)

        Gias_InizializzaCultura_DaParams(objParametri.Server, objParametri.Utenti)

        Dim resp As New rispostaStandard(Of List(Of Bookmark))

        Dim xRead As New AgronicaCoreGisBIZ.GIS_Bookmark_R

        Try
            resp.RispostaOK = True
            resp.RispostaStringa = xRead.leggi(objParametri.InData, objParametri.Server)

        Catch ex As Exception

            resp.RispostaOK = False
            resp.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return resp
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiDatiMUZVisibili(ByVal InData As Object) As rispostaStandard(Of List(Of DatiMUZVisibili_Out))

        Dim objParametri = DeserializzaInData(Of LeggiDatiMUZVisibili_In)(InData)

        Gias_InizializzaCultura_DaParams(objParametri.Server, objParametri.Utenti)

        Dim resp As New rispostaStandard(Of List(Of DatiMUZVisibili_Out))

        Dim xRead As New AgronicaCoreGisBIZ.GIS_MUZ_R

        Try
            resp.RispostaOK = True
            resp.RispostaStringa = xRead.LeggiDatiMUZVisibili(objParametri.InData.piva,
                                                              objParametri.InData.lista_MUZ_Cod,
                                                              objParametri.Server)

        Catch ex As Exception

            resp.RispostaOK = False
            resp.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return resp
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiListaGruppiMUZ(ByVal InData As Object) As rispostaStandard(Of List(Of GruppoAreaOmogenea))

        Dim objParametri = DeserializzaInData(Of String)(InData)

        Gias_InizializzaCultura_DaParams(objParametri.Server, objParametri.Utenti)

        Dim resp As New rispostaStandard(Of List(Of GruppoAreaOmogenea))

        Dim xRead As New AgronicaCoreGisBIZ.GIS_MUZ_R

        Try
            resp.RispostaOK = True
            resp.RispostaStringa = xRead.LeggiListaGruppiMUZ(objParametri.InData,
                                                             objParametri.Server)

        Catch ex As Exception

            resp.RispostaOK = False
            resp.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return resp
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function OperazioniGruppiMUZ(ByVal InData As Object) As RispostaStandard

        Dim objParametri = DeserializzaInData(Of OperazioniGruppiMUZ_In)(InData)

        Gias_InizializzaCultura_DaParams(objParametri.Server, objParametri.Utenti)

        Dim resp As New RispostaStandard

        Dim xWrite As New AgronicaCoreGisBIZ.GIS_MUZ_W

        Try
            resp.RispostaOK = xWrite.EseguiOperazioneGruppiMUZ(objParametri.InData, objParametri.Server)

            If Not resp.RispostaOK Then
                Throw New Exception("Impossibile eseguire l'operazione desiderata.")
            End If

            resp.RispostaStringa = "Operazione completata con successo."

        Catch ex As Exception

            resp.RispostaOK = False
            resp.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return resp
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function OperazioniMUZ(ByVal InData As Object) As RispostaStandard

        Dim objParametri = DeserializzaInData(Of MUZ)(InData)

        Gias_InizializzaCultura_DaParams(objParametri.Server, objParametri.Utenti)

        Dim resp As New RispostaStandard

        Dim xWrite As New AgronicaCoreGisBIZ.GIS_MUZ_W

        Try
            resp.RispostaOK = xWrite.EseguiOperazioneMUZ(objParametri.InData, objParametri.Server)

            If Not resp.RispostaOK Then
                Throw New Exception("Impossibile eseguire l'operazione desiderata.")
            End If

            resp.RispostaStringa = "Operazione completata con successo."

        Catch ex As Exception

            resp.RispostaOK = False
            resp.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return resp
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiMUZ(ByVal InData As Object) As rispostaStandard(Of List(Of MUZ))

        Dim objParametri = DeserializzaInData(Of LeggiMUZ_In)(InData)

        Gias_InizializzaCultura_DaParams(objParametri.Server, objParametri.Utenti)

        Dim resp As New rispostaStandard(Of List(Of MUZ))

        Dim xRead As New AgronicaCoreGisBIZ.GIS_MUZ_R

        Try
            resp.RispostaOK = True
            resp.RispostaStringa = xRead.LeggiMUZ(objParametri.InData.Area_Cod, objParametri.InData.Piva, objParametri.Server)

        Catch ex As Exception

            resp.RispostaOK = False
            resp.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return resp
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function RasterInfoClick(ByVal InData As Object) As rispostaStandard(Of List(Of RasterInfoClick_Out))

        Dim objParametri = DeserializzaInData(Of RasterInfoClick_In)(InData)

        Gias_InizializzaCultura_DaParams(objParametri.Server, objParametri.Utenti)

        Dim resp As New rispostaStandard(Of List(Of RasterInfoClick_Out))

        Dim xRead As New AgronicaCoreGisBIZ.GIS_ElementiGrafici_R

        Try
            resp.RispostaOK = True
            resp.RispostaStringa = xRead.LeggiInfoRaster(objParametri.InData.layerElementiGrafici_Cod,
                                                         objParametri.InData.PoligonoWKT,
                                                         objParametri.Server)

        Catch ex As Exception

            resp.RispostaOK = False
            resp.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return resp
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function CaricaPaletteSLD(ByVal InData As Object) As RispostaStandard

        Dim objParametri = DeserializzaInData(Of CaricaPaletteSLD_In)(InData)

        Gias_InizializzaCultura_DaParams(objParametri.Server, objParametri.Utenti)

        Dim resp As New RispostaStandard

        Dim xWrite As New AgronicaCoreGisBIZ.GIS_LayerElementiGrafici

        Try
            resp.RispostaOK = xWrite.SalvaPaletteSLDDaXML(objParametri.InData.LayerElementiGrafici_Cod,
                                                          objParametri.InData.TipologiaLayer_Cod,
                                                          objParametri.InData.filePalette,
                                                          objParametri.Server)

            If Not resp.RispostaOK Then
                resp.Errore = "Errore nel caricamento della palette per il raster desiderato."
            Else
                resp.RispostaStringa = "Operazione completata con successo."
            End If

        Catch ex As Exception

            resp.RispostaOK = False
            resp.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return resp
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function VerificaEsistenzaPalette(ByVal InData As Object) As rispostaStandard(Of Boolean)

        Dim objParametri = DeserializzaInData(Of VerificaEsistenzaPalette_In)(InData)

        Gias_InizializzaCultura_DaParams(objParametri.Server, objParametri.Utenti)

        Dim resp As New rispostaStandard(Of Boolean)

        Dim xWrite As New AgronicaCoreGisBIZ.GIS_LayerElementiGrafici

        Try
            resp.RispostaOK = True
            resp.RispostaStringa = xWrite.VerificaEsistenzaPalette(objParametri.InData.LayerElementiGrafici_Cod,
                                                                   objParametri.InData.TipologiaLayer_Cod,
                                                                   objParametri.Server)
        Catch ex As Exception

            resp.RispostaOK = False
            resp.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return resp
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function getEnvelopeWKT(ByVal InData As Object) As rispostaStandard(Of String)
        Dim a As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}
        Dim params As CoreWS_Generic(Of Integer) =
            JsonConvert.DeserializeObject(Of CoreWS_Generic(Of Integer))(JsonConvert.SerializeObject(InData), a)

        Dim resp As New rispostaStandard(Of String)
        Dim xRead As New V_M

        If params.objP.objP_server = "" Then
            resp.Errore = "objP_server non valorizzato"
            Return resp
        End If

        If params.objP.objP_utenti = "" Then
            resp.Errore = "objP_utenti non valorizzato"
            Return resp
        End If

        Try
            Dim objParametri_Super_Server As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(params.objP.objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(params.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(params.objP.objP_utenti)

            Dim wkt = xRead.getEnvelopeWKT(params.InData, objParametri_Server)

            resp.RispostaOK = True
            resp.RispostaStringa = wkt

        Catch ex As Exception

            resp.RispostaOK = False
            resp.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return resp
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function IncludiEsludiImpresaDaControlloComplianceISCC(ByVal InData As Object) As rispostaStandard(Of Boolean)
        Dim a As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}
        Dim params As CoreWS_Generic(Of IncludiEsludiImpresaISCC_In) =
            JsonConvert.DeserializeObject(Of CoreWS_Generic(Of IncludiEsludiImpresaISCC_In))(JsonConvert.SerializeObject(InData), a)

        Dim resp As New rispostaStandard(Of Boolean)
        Dim xBIZ As New AgronicaCoreGisBIZ.Compliance_ISCC

        If params.objP.objP_server = "" Then
            resp.Errore = "objP_server non valorizzato"
            Return resp
        End If

        If params.objP.objP_utenti = "" Then
            resp.Errore = "objP_utenti non valorizzato"
            Return resp
        End If

        Try
            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(params.objP.objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(params.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(params.objP.objP_utenti)

            resp.RispostaOK = True
            resp.RispostaStringa = xBIZ.IncludiEsludiAziendeControlloISCC(params.InData, objParametri_Server)
            'resp.RispostaStringa = If(resp.RispostaOK, "Aggiornamento effettuato correttamente.", "Errore in aggiornamento flag compliance iscc per le aziende indicate.")

        Catch ex As Exception

            resp.RispostaOK = False
            resp.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return resp
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function SottomettiElaborazioneMassivaGISCheckList(ByVal InData As Object) As rispostaStandard(Of Boolean)
        Dim a As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}
        Dim params As CoreWS_Generic(Of SottomettiElaborazioneMassivaGISCheckList_In) =
            JsonConvert.DeserializeObject(Of CoreWS_Generic(Of SottomettiElaborazioneMassivaGISCheckList_In))(JsonConvert.SerializeObject(InData), a)

        Dim resp As New rispostaStandard(Of Boolean)
        Dim xBIZ As New AgronicaCoreGisBIZ.Compliance_ISCC

        If params.objP.objP_server = "" Then
            resp.Errore = "objP_server non valorizzato"
            Return resp
        End If

        If params.objP.objP_utenti = "" Then
            resp.Errore = "objP_utenti non valorizzato"
            Return resp
        End If

        Try
            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(params.objP.objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(params.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(params.objP.objP_utenti)

            resp.RispostaOK = True
            If (params.InData.elaboraAziendeInVisibilita) Then
                Dim ObjUtenti As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
                Dim abilitazione = ObjUtenti.Controlla_Permessi_Utente(objParametri_Utenti.UtenteUsername,
                                                                             Id_Servizio_GiasOnline,
                                                                             enum_Security_Attivita.CalcoloMassivo_Compliance_ISCC_AziendeInVisibilita,
                                                                             enum_Security_Operazione.Modifica,
                                                                             Date.Now,
                                                                             "",
                                                                             objParametri_Utenti)
                If abilitazione Then
                    resp.RispostaStringa = xBIZ.SottomettiRichiestaDiElaborazioneAsincrona(params.InData,
                                                                                           objParametri_Server)
                Else
                    resp.RispostaOK = False
                    resp.Errore = "Utente non abilitato alla funzione"
                End If
            Else
                resp.RispostaStringa = xBIZ.SottomettiElaborazioneMassiva(
                    params.InData.checkListType,
                    params.InData.dataRiferimento,
                    params.InData.elencoPiva,
                    objParametri_Server
                    )
            End If

            'resp.RispostaStringa = If(resp.RispostaOK, "Richiesta sottomessa correttamente.", "Errore in sottomissione richiesta aggiornamento\ricalcolo compliance ISCC")

        Catch ex As Exception

            resp.RispostaOK = False
            resp.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, False, source:=False)

        End Try

        Return resp
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function VerificaAziendaAbilitataISCC(ByVal InData As Object) As rispostaStandard(Of Boolean)
        Dim a As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}
        Dim params As CoreWS_Generic(Of VerificaAziendaAbilitataISCC_In) =
            JsonConvert.DeserializeObject(Of CoreWS_Generic(Of VerificaAziendaAbilitataISCC_In))(JsonConvert.SerializeObject(InData), a)

        Dim resp As New rispostaStandard(Of Boolean)
        Dim xBIZ As New AgronicaCoreGisBIZ.Compliance_ISCC

        If params.objP.objP_server = "" Then
            resp.Errore = "objP_server non valorizzato"
            Return resp
        End If

        If params.objP.objP_utenti = "" Then
            resp.Errore = "objP_utenti non valorizzato"
            Return resp
        End If

        Try
            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(params.objP.objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(params.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(params.objP.objP_utenti)

            resp.RispostaOK = True
            resp.RispostaStringa = xBIZ.VerificaAziendaAbilitataISCC(params.InData.checkListType, params.InData.piva, objParametri_Server)

        Catch ex As Exception

            resp.RispostaOK = False
            resp.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return resp
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiElencoElaborazioniMassiveGISCheckList(ByVal InData As Object) As rispostaStandard(Of LeggiElencoElaborazioniMassive_Out)
        Dim a As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}
        Dim params As CoreWS_Generic(Of LeggiElencoElaborazioniMassive_In) =
            JsonConvert.DeserializeObject(Of CoreWS_Generic(Of LeggiElencoElaborazioniMassive_In))(JsonConvert.SerializeObject(InData), a)

        Dim resp As New rispostaStandard(Of LeggiElencoElaborazioniMassive_Out)
        Dim xBIZ As New AgronicaCoreGisBIZ.Compliance_ISCC

        If params.objP.objP_server = "" Then
            resp.Errore = "objP_server non valorizzato"
            Return resp
        End If

        If params.objP.objP_utenti = "" Then
            resp.Errore = "objP_utenti non valorizzato"
            Return resp
        End If

        Try
            Dim objParametri_Super_Server As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(params.objP.objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(params.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(params.objP.objP_utenti)

            resp.RispostaOK = True
            resp.RispostaStringa = xBIZ.LeggiElencoElaborazioniXTipoCheckList(
                params.InData.checkListType,
                params.InData.dataLetturaInizio,
                params.InData.dataLetturaFine,
                params.InData.piva,
                objParametri_Server
                )

        Catch ex As Exception

            resp.RispostaOK = False
            resp.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return resp
    End Function

    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function LeggiDateImpiantiFiltroTemporale(ByVal objP_super_server As String,
                                             ByVal objP_server As String,
                                             ByVal objP_utenti As String,
                                             InData As Object
                                             ) As rispostaStandard(Of Date)

        Dim r As New rispostaStandard(Of Date)

        Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
        Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)

        Dim iData As LeggiDateImpiantiPerFiltroTemporale_In = JsonConvert.DeserializeObject(Of LeggiDateImpiantiPerFiltroTemporale_In)(JsonConvert.SerializeObject(InData))

        Try

            Dim leggiLingua As New Lingue_Read
            Dim dtLingua As DataTable = leggiLingua.Leggi_datoCODGIAS(objParametri_Server.Lingua_Cod,
                                                                      AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                      "",
                                                                      "",
                                                                      objParametri_Utenti)

            Dim linguaCodiceISO As String = dtLingua.Rows(0)("CodiceISO")
            Threading.Thread.CurrentThread.CurrentUICulture = New Globalization.CultureInfo(linguaCodiceISO)

            Dim obj As New GisDataReadRval_New(Of AgronicaCoreModelsSTD.Gis.GeoJSONAgroGisProp)

            ' Comportamento elaborazione, in ordine:
            ' 1. cerco impianti validi alla data di elaborazione, se ok ritorno la data di elaborazione
            ' 2. cerco impianti con data fine minore rispetto alla data di elaborazione e seleziono la maggiore, se ok ritorno il valore
            ' 3. cerco impianti con data inizio maggiore rispetto alla data di elaborazione e seleziono la minore, se ok ritorno il valore

            Dim filtroTemporale = V_M.GetDataElaborazione(objParametri_Server, objParametri_Utenti, iData)

            If (Not String.IsNullOrEmpty(filtroTemporale)) Then
                r.RispostaOK = True
                r.RispostaStringa = filtroTemporale
            Else
                r.RispostaOK = True
                r.RispostaStringa = Date.Now
                r.Errore = AgronicaCoreDataProvider.My.Resources.Gias.NoImpiantiSuAziendaSelezionata
            End If

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = ex.Message

        End Try

        Return r
    End Function

    ''' <summary>
    ''' Parses the response from the Indexes Service endpoint and transforms it into the RisultatoElaborazione format for the Agronica frontend.
    ''' This method implements the business logic for parsing PolygonIndexStatsResponse as specified in the design specification.
    ''' </summary>
    ''' <param name="inputData">The input data containing the PolygonIndexStatsResponse in JSON format.</param>
    ''' <param name="pixelCoverageThreshold">Minimum pixel coverage (0–1) below which a day is considered cloudy; read from site configuration key 'pixelCoverageThreshold_SAT', default 1.</param>
    ''' <returns>A rispostaStandard containing the RisultatoElaborazione object or an error message.</returns>
    Private Function ParseIndexesStatsResponse(ByVal inputData As String, ByVal pixelCoverageThreshold As Decimal) As RisultatoElaborazione
        Dim result = New RisultatoElaborazione With {.RisultatoElaborazione = New List(Of RigaRisultato)()}
        Try
            ' Deserialize the input data
            Dim inputResponse = JsonConvert.DeserializeObject(Of PolygonIndexStatsResponse)(inputData)

            ' Step 1: Validate Input
            If inputResponse Is Nothing Then
                Throw New Exception("Input response non valido")
            End If

            If String.IsNullOrEmpty(inputResponse.polygon_id) OrElse String.IsNullOrEmpty(inputResponse.index_code) OrElse inputResponse.values Is Nothing Then
                Throw New Exception("Response non contiene campi richiesti: polygon_id, index_code, values")
            End If

            If Not TypeOf inputResponse.values Is List(Of PolygonIndexStatsValue) Then
                Throw New Exception("Field 'values' deve essere un array")
            End If

            If inputResponse.values.Count = 0 Then
                Return result
            End If

            ' Step 2: Extract Base Data
            Dim extractedIndexCode As String = inputResponse.index_code
            Dim extractedDataPoints As List(Of PolygonIndexStatsValue) = inputResponse.values

            ' Step 3: Parse all timestamps upfront
            Dim parsedList As New List(Of (Dp As PolygonIndexStatsValue, Dt As DateTime))
            For Each dataPoint As PolygonIndexStatsValue In extractedDataPoints
                Dim dt As DateTime
                If Not DateTime.TryParse(dataPoint.point_in_time, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.RoundtripKind, dt) Then
                    Throw New Exception("Errore parsing timestamp: formato non riconosciuto per '" & dataPoint.point_in_time & "'")
                End If
                parsedList.Add((dataPoint, dt))
            Next

            ' Step 4: Group by calendar date, keep the entry with the highest pixel_coverage per day
            Dim bestPerDay = parsedList.GroupBy(Function(p) p.Dt.Date) _
                                        .Select(Function(g) g.OrderByDescending(Function(p) If(p.Dp.pixel_coverage, 0.0)).First())

            ' Step 5: Build result rows
            For Each entry In bestPerDay
                Dim formattedDate As String = entry.Dt.ToString("dd/MM/yyyy")
                Dim formattedDateSAT As String = entry.Dt.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss") & "Z"

                ' Create sensor object
                Dim sensorObject As New Sensore With {
                    .CodiceSensore = extractedIndexCode,
                    .Descrizione = extractedIndexCode,
                    .DatiRilevati = Nothing
                }

                ' Create passaggio object
                ' Preserve legacy behavior: "url" is an empty string on cloudy days 
                Dim url = IIf(entry.Dp.pixel_coverage.HasValue And CDec(entry.Dp.pixel_coverage) >= pixelCoverageThreshold, extractedIndexCode, "")
                Dim passaggioObject As New Passaggio With {
                    .Sensore = New List(Of Sensore) From {sensorObject},
                    .url = url
                }

                ' Create riga risultato
                Dim rigaRisultato As New RigaRisultato With {
                    .DataRiferimento = formattedDate,
                    .DataRiferimentoSAT = formattedDateSAT,
                    .Passaggi = New List(Of Passaggio) From {passaggioObject}
                }

                ' Add to results array
                result.RisultatoElaborazione.Add(rigaRisultato)
            Next

        Catch ex As Exception
            Throw New Exception("Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False))
        End Try

        Return result
    End Function

    ''' <summary>
    ''' Fetches polygon index statistics from SAT service as per design specification section 'Processing Logic'.
    ''' </summary>
    ''' <param name="host">SAT base url</param>
    ''' <param name="tenant">name of the current tenant</param>
    ''' <param name="apiKey">SAT api key of the current tenant</param>
    ''' <param name="polygonId">SAT identifier for the polygon</param>
    ''' <param name="indexCode">index such as ndvi, ndwi, ...</param>
    ''' <param name="dateFrom">initial date </param>
    ''' <param name="dateTo">final date</param>
    ''' <returns>A rispostaStandard containing the PolygonIndexStatsResult or an error message.</returns>
    Public Function FetchPolygonIndexStats(
        host As String,
        tenant As String,
        apiKey As String,
        polygonId As String,
        indexCode As String,
        dateFrom As String,
        dateTo As String
    ) As String
        Try
            Dim parsedDateFrom As DateTime
            Dim parsedDateTo As DateTime

            ' Phase 1: Input Validation & Normalization
            If String.IsNullOrEmpty(host) Then Throw New ArgumentException("host non può essere vuoto")
            If String.IsNullOrEmpty(tenant) Then Throw New ArgumentException("tenant non può essere vuoto")
            If String.IsNullOrEmpty(apiKey) Then Throw New ArgumentException("apiKey non può essere vuota")
            If String.IsNullOrEmpty(polygonId) Then Throw New ArgumentException("polygonId non può essere vuoto")
            If String.IsNullOrEmpty(indexCode) Then Throw New ArgumentException("indexCode non può essere vuoto")
            If Not DateTime.TryParse(dateFrom, parsedDateFrom) Then Throw New ArgumentException("dateFrom deve essere in formato ISO-8601")
            If Not DateTime.TryParse(dateTo, parsedDateTo) Then Throw New ArgumentException("dateTo deve essere in formato ISO-8601")

            ' Phase 2: HTTP Request Construction & API Call
            Dim requestUrl = "https://" & host & "/indexes/" & tenant & "/public/v1/polygons/" & polygonId & "/indexes/" & indexCode & "/stats" &
                             "?from=" & Uri.EscapeDataString(dateFrom) & "&to=" & Uri.EscapeDataString(dateTo)
            Using client As New Net.Http.HttpClient()
                client.Timeout = TimeSpan.FromSeconds(60)
                Dim request As New Net.Http.HttpRequestMessage(Net.Http.HttpMethod.Get, requestUrl)
                request.Headers.Add("Authorization", "APIKEY " & apiKey)
                request.Headers.Add("Accept", "application/json")

                Dim response As Net.Http.HttpResponseMessage = client.SendAsync(request).Result

                ' Phase 3: Response Processing & Error Handling
                Select Case response.StatusCode
                    Case Net.HttpStatusCode.OK
                        Dim responseContent As String = response.Content.ReadAsStringAsync().Result
                        return responseContent
                    Case Net.HttpStatusCode.NotFound
                        Throw new Exception($"Risorsa non trovata: poligono {polygonId} o indice {indexCode} non esiste per tenant {tenant}")
                    Case Net.HttpStatusCode.Unauthorized
                        Throw new Exception("Chiave API non valida o scaduta")
                    Case Net.HttpStatusCode.Forbidden
                        Throw new Exception($"Accesso negato al tenant {tenant} o alla risorsa {polygonId}")
                    Case Net.HttpStatusCode.BadRequest
                        Throw new Exception("Richiesta malformata")
                    Case Net.HttpStatusCode.InternalServerError
                        Throw new Exception("Errore interno del servizio SAT")
                    Case Net.HttpStatusCode.ServiceUnavailable
                        Throw new Exception("Servizio SAT temporaneamente non disponibile")
                    Case Else
                        Throw new Exception("Errore HTTP non atteso: " & response.StatusCode)
                End Select
            End Using

        Catch ex As Exception
            Throw New Exception("Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False))
        End Try
    End Function
    
    Private Function ManageFileMappePrescrizione(requestId as String,
                                                 piva as String,
                                                 saCod as Int32,
                                                 appezza as Int32,
                                                 idImp as Int32,
                                                 ricettaOperazioneCod as Int32,
                                                 fileRaster as Byte(),
                                                 objParametriServer As AgronicaCoreParametri,
                                                 objParametriSuperServer As AgronicaCoreParametri,
                                                 ByRef outputAllegatiCod As Int32
                                                 ) As Boolean 
        Dim nomeFile As String = "prescrizione_" & requestId & ".tif"

        Dim scriviAllegati As New AgronicaCoreAnagrafeBIZ.Allegati_Documenti_W
        Dim resp = scriviAllegati.PrecisionFarmingScriviSuAllegati(
            TipiEnumerativi.enum_CategorieDocumenti.PrecisionFarming_MappaPrescrizione,
            "Mappa Prescrizione " & requestId,
            nomeFile,
            objParametriServer,
            piva,
            saCod,
            appezza,
            ricettaOperazioneCod,
            idImp,
            "",
            fileRaster,
            "",
            outputAllegatiCod,
            False,
            True,
            objParametriSuperServer)

        If Not resp.RispostaOK Then
            Throw New Exception("Errore nell'archiviazione del raster: " & resp.Errore)
        End If
        
        Return True
    End Function

#Region "Data Classes for Indexes Stats Parser"

    ''' <summary>
    ''' Represents the response from the Indexes Service Polygon Stats endpoint.
    ''' </summary>
    Public Class PolygonIndexStatsResponse
        Public Property polygon_id As String
        Public Property index_code As String
        Public Property values As List(Of PolygonIndexStatsValue)
    End Class

    ''' <summary>
    ''' Represents a single value in the PolygonIndexStatsResponse.
    ''' </summary>
    Public Class PolygonIndexStatsValue
        Public Property point_in_time As String
        Public Property min As Double?
        Public Property max As Double?
        Public Property pixel_count As Double?
        Public Property pixel_coverage As Double?
        Public Property avg As Double?
        Public Property stddev As Double?
    End Class

    ''' <summary>
    ''' Represents the output structure for RisultatoElaborazione.
    ''' </summary>
    Public Class RisultatoElaborazione
        Public Property RisultatoElaborazione As List(Of RigaRisultato)
    End Class

    ''' <summary>
    ''' Represents a single row in the RisultatoElaborazione.
    ''' </summary>
    Public Class RigaRisultato
        Public Property DataRiferimento As String
        Public Property DataRiferimentoSAT As String
        Public Property Passaggi As List(Of Passaggio)
    End Class

    ''' <summary>
    ''' Represents a passaggio in the RigaRisultato.
    ''' </summary>
    Public Class Passaggio
        Public Property Sensore As List(Of Sensore)
        Public Property Url As String
    End Class

    ''' <summary>
    ''' Represents a sensore in the Passaggio.
    ''' </summary>
    Public Class Sensore
        Public Property CodiceSensore As String
        Public Property Descrizione As String
        Public Property DatiRilevati As Object
    End Class

#End Region

End Class