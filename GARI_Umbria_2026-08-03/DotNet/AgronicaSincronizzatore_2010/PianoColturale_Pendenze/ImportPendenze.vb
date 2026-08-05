Imports AgronicaCoreAnagrafeDAL
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDTOStd.InData.DataExchange
Imports AgronicaCoreMetaSchemaDAL
Imports AgronicaCoreModelsSTD.anagrafiche
Imports AgronicaCoreModelsSTD.exceptions
Imports AgronicaCoreVarieDAL
Imports NetTopologySuite.Algorithm
Imports Newtonsoft.Json

Public Class ImportPendenze
    Implements IDisposable

    Private _objParametriSuperServer As AgronicaCoreParametri
    Private _objParametriServer As AgronicaCoreParametri
    Private _objParametriUtenti As AgronicaCoreParametri

    Private _parametriTask As ParametriApiPendenze

    Private _logName As String = DateTime.Now.ToString("yyyy-MM-dd") + ".log"
    Private _commitRow As Integer = 10
    Private _logger As LoggerManager
    Private _helper As Helper

    Private _agronicacorelogDir As String
    Private _agronicacorelogFile As String

    'Cache per i valori di PROV e COM letti da Istat
    Private _dictIstatProvCom As Dictionary(Of String, Tuple(Of String, String))
    Private _objIstat As Istat_R

    Public Sub New(ByVal configurazioneServizio As Configurazione_Servizio,
                   ByVal objParametriSuperServer As AgronicaCoreParametri,
                   ByVal objParametriServer As AgronicaCoreParametri,
                   ByVal objParametriUtenti As AgronicaCoreParametri)
        _objParametriSuperServer = objParametriSuperServer
        _objParametriServer = objParametriServer
        _objParametriUtenti = objParametriUtenti
        _parametriTask = JsonConvert.DeserializeObject(Of ParametriApiPendenze)(configurazioneServizio.Parametri_Extra)

        _logger = New LoggerManager(configurazioneServizio.DirectoryLOG,
                                    _logName,
                                    "",
                                    _parametriTask.LOG_level,
                                    _parametriTask.LOG_DB,
                                    _commitRow)

        _helper = New Helper()

        _agronicacorelogDir = configurazioneServizio.DirectoryLOG
        _agronicacorelogFile = _logName

        _dictIstatProvCom = New Dictionary(Of String, Tuple(Of String, String))
        _objIstat = New Istat_R

    End Sub

    Public Sub ImportaPendenze()

        Dim dtAppezzamenti As DataTable = Nothing
        Dim objAppezzamentoRead As New AgronicaCoreAnagrafeDAL.Appezzamento_Read


        'Dim objAppezzaxParticelleR As New AppezzaxParticelle_R
        'Dim dtAppezzaxParticelle As DataTable


        Dim dtWKT As DataTable = Nothing
        Dim objGisElementiGraficiR As New AgronicaCoreGisDAL.GIS_ElementiGrafici_R

        Dim Piva As String
        Dim Sa_Cod As Integer
        Dim Appezza As Integer
        Dim Entita_Cod As Integer
        Dim Sup_App As Double
        Dim wkt As String

        Dim logErroriCount = 0
        Dim logSuccessCount = 0


        Try
            Dim ObjParametri_SuperServer As AgronicaCoreParametri = _objParametriSuperServer.CreateDeepCopy(_objParametriSuperServer)
            Dim ObjParametri_Server As AgronicaCoreParametri = _objParametriServer.CreateDeepCopy(_objParametriServer)
            Dim ObjParametri_Utenti As AgronicaCoreParametri = _objParametriUtenti.CreateDeepCopy(_objParametriUtenti)

            ObjParametri_SuperServer.LogDirectory = _agronicacorelogDir
            ObjParametri_Server.LogDirectory = _agronicacorelogDir
            ObjParametri_Utenti.LogDirectory = _agronicacorelogDir
            ObjParametri_SuperServer.LogFileName = _agronicacorelogFile
            ObjParametri_Server.LogFileName = _agronicacorelogFile
            ObjParametri_Utenti.LogFileName = _agronicacorelogFile

            ObjParametri_Server.UtenteUsername = "Servizio importazione pendenze"

            ''Vengono letti solo gli appezzamenti validi in data odierna
            'ObjParametri_Server.FinestraTemporaleInizio = Today
            'ObjParametri_Server.FinestraTemporaleFine = Today

            'Lettura appezzamenti da aggiornare
            Dim PivaAzienda As String = If(Not String.IsNullOrEmpty(_parametriTask.PIVA_Azienda), _parametriTask.PIVA_Azienda.Trim(), "")
            dtAppezzamenti = objAppezzamentoRead.LeggiPerAggiornamentoPendenza(PivaAzienda,
                                                                               ObjParametri_Server,
                                                                               _parametriTask.Filtro_AxiendePrioritarie,
                                                                               _parametriTask.ElencoProvince)

            If IsNothing(dtAppezzamenti) OrElse dtAppezzamenti.Rows.Count = 0 Then
                Return
            End If

            _logger.AppendLog(LogLevel.BASE, String.Format("Avvio importazione pendenze per {0} appezzamenti. ", dtAppezzamenti.Rows.Count))

            Dim pendenzeController As New ReUmbriaApiProvider(_parametriTask.API_pendenze_url_base,
                                                         _parametriTask.API_pendende_url_login,
                                                         _parametriTask.API_pendenze_login,
                                                         _parametriTask.API_pendenze_p,
                                                         _parametriTask.API_pendenze_url_particelle,
                                                         _parametriTask.API_pendenze_epsg,
                                                         _parametriTask.Use_POST_API)

            For Each rowAppezzamento In dtAppezzamenti.Rows

                Piva = rowAppezzamento.Item("Piva")
                Sa_Cod = rowAppezzamento.Item("Sa_Cod")
                Appezza = rowAppezzamento.Item("Appezza")
                Entita_Cod = rowAppezzamento.Item("Entita_Cod")
                Sup_App = rowAppezzamento.Item("Sup_App")

                wkt = ""


                _logger.AppendLog(LogLevel.BASE, "")
                Try
                    dtWKT = objGisElementiGraficiR.LeggiWKTConGUID(Entita_Cod, "", ObjParametri_Server)
                    If dtWKT.Rows.Count = 0 Then
                        Throw New GiasException(String.Format("Nessun WKT ritornato per Entita_Cod {0}", Entita_Cod.ToString()))
                    End If

                    wkt = dtWKT.Rows(0).Item("wkt")

                    Select Case _parametriTask.TipoLetturaParticelle
                        Case enum_TipoLetturaParticelle.GIS
                            ElaboraPendenzeGIS(Piva,
                                                Sa_Cod,
                                                Appezza,
                                                wkt,
                                                Sup_App,
                                                _parametriTask.GIS_IntersectionTollerance,
                                                logSuccessCount,
                                                logErroriCount,
                                                ObjParametri_SuperServer,
                                                ObjParametri_Server,
                                                ObjParametri_Utenti)
                        Case enum_TipoLetturaParticelle.AGEA
                            ElaboraPendenzeAGEA(Piva,
                                                Sa_Cod,
                                                Appezza,
                                                wkt,
                                                pendenzeController,
                                                logSuccessCount,
                                                logErroriCount,
                                                ObjParametri_SuperServer,
                                                ObjParametri_Server,
                                                ObjParametri_Utenti)
                        Case enum_TipoLetturaParticelle.AGEA_GIS
                            If Not ElaboraPendenzeAGEA(Piva,
                                                Sa_Cod,
                                                Appezza,
                                                wkt,
                                                pendenzeController,
                                                logSuccessCount,
                                                logErroriCount,
                                                ObjParametri_SuperServer,
                                                ObjParametri_Server,
                                                ObjParametri_Utenti) Then
                                ElaboraPendenzeGIS(Piva,
                                                Sa_Cod,
                                                Appezza,
                                                wkt,
                                                Sup_App,
                                                _parametriTask.GIS_IntersectionTollerance,
                                                logSuccessCount,
                                                logErroriCount,
                                                ObjParametri_SuperServer,
                                                ObjParametri_Server,
                                                ObjParametri_Utenti)
                            End If
                        Case Else
                            Throw New Exception("TipoLetturaParticelle non valido")
                    End Select
                Catch ex As Exception
                    _logger.AppendLog(LogLevel.ERRORI, String.Format("Errore elaborazione pendenza appezzamento (Piva: {0}, Sa_Cod: {1}, Appezza: {2}): {3}", Piva, Sa_Cod.ToString(), Appezza.ToString(), ex.Message), LogType.Errore)
                    _logger.LogImportazionePendenza(Piva, Sa_Cod, Appezza, "", "", 0, False, ex.Message, ObjParametri_Server)

                End Try

            Next

        Catch ex As Exception
            _logger.AppendLog(LogLevel.BASE, ex.Message, LogType.Errore)
            _logger.AppendLog(LogLevel.BASE, "Importazione pendenze interrotta")
        End Try

        If Not IsNothing(dtAppezzamenti) Then
            _logger.AppendLog(LogLevel.BASE, "")
            Dim errorLogCountReport As String = If(logErroriCount > 0, String.Format(", errori in importazione per {0} appezzamenti", logErroriCount.ToString()), "")
            _logger.AppendLog(LogLevel.BASE, String.Format("Importazione pendenze terminata: importazione eseguita con successo per {0} appezzamenti su {1}{2}", logSuccessCount.ToString(), dtAppezzamenti.Rows.Count, errorLogCountReport))
        End If

        _logger.AppendLog(LogLevel.BASE, "")
        _logger.AppendLog(LogLevel.BASE, "=================================================================================================")
        _logger.AppendLog(LogLevel.BASE, "")

        _logger.Flush()

    End Sub

    Private Sub ElaboraPendenzeGIS(ByVal Piva As String,
                                        ByVal Sa_Cod As Integer,
                                        ByVal Appezza As Integer,
                                        ByVal wktAppezza As String,
                                        ByVal Sup_App As Double,
                                        ByVal GisIntersectionTollerance As Double,
                                        ByRef logSuccessCount As Integer,
                                        ByRef logErroriCount As Integer,
                                        ByRef ObjParametri_SuperServer As AgronicaCoreParametri,
                                        ByRef ObjParametri_Server As AgronicaCoreParametri,
                                        ByRef ObjParametri_Utenti As AgronicaCoreParametri
                                        )

        Dim objGisElementiGraficiR As New AgronicaCoreGisDAL.GIS_ElementiGrafici_R
        Dim objParticella As New AgronicaCoreAnagrafeBIZ.Particella_R
        Dim objParticellaW As New AgronicaCoreAnagrafeBIZ.Particella_W
        Dim objImpresexParticelleR As New AgronicaCoreAnagrafeDAL.ImpresexParticelle2_R
        Dim objImpresexParticelleW As New AgronicaCoreAnagrafeDAL.ImpresexParticelle2_W
        Dim objAppezzamentoR As New AgronicaCoreAnagrafeBIZ.Appezzamento_R
        Dim objAppezzamentoW As New AgronicaCoreAnagrafeBIZ.Appezzamento_W

        Dim appezzamento As AgronicaCoreModelsSTD.anagrafiche.Appezzamento

        Dim nuoveParticellePerAppezza As New List(Of AgronicaCoreModelsSTD.anagrafiche.CatastoAppezzamento)
        Dim nuoveParticellePerImpresa As New List(Of AgronicaCoreModelsSTD.anagrafiche.CatastoCentroAziendale)
        Dim particelleDaCreare As New List(Of AgronicaCoreDTOStd.InData.Anagrafica.ScriviCatasto)
        Dim lsImpresexParticellePeriodi As New List(Of ImpresexParticelle2_Periodo)
        Dim particellaCatasto As AgronicaCoreDTOStd.InData.Anagrafica.ScriviCatasto
        Dim lastYearDay As Date = New Date(Date.Now.Year, 12, 31)

        Dim logDbNote As String = ""

        '- recupera l'elenco delle intersezioni sul layer 3 (catasto)
        Dim dtIntersezioni = objGisElementiGraficiR.LeggiIntersezioneCatastoPoligono(Piva,
                                                                                     Sa_Cod,
                                                                                     Appezza,
                                                                                     0,
                                                                                     TipiEnumerativi.enum_GIS2012_TipoEntita.APPEZZAMENTI,
                                                                                     TipiEnumerativi.enum_GIS2012_TipoEntita.CATASTO,
                                                                                     ObjParametri_Server)
        If dtIntersezioni.Rows.Count > 0 Then
            _logger.AppendLog(LogLevel.AGG_CATASTO, String.Format("Trovate {3} particelle che intersecano l'appezzamento (Piva: {0}, Sa_Cod: {1}, Appezza: {2})", Piva, Sa_Cod.ToString(), Appezza.ToString(), dtIntersezioni.Rows.Count.ToString()), LogType.Informazione)
            For Each intersect In dtIntersezioni.Rows

                Dim dtWKTParticella = objGisElementiGraficiR.LeggiWKTConGUID(intersect("Entita_Cod"), "", ObjParametri_Server)
                If dtWKTParticella.Rows.Count = 0 Then
                    _logger.AppendLog(LogLevel.AGG_CATASTO, String.Format("Nessun WKT trovato per Entita_Cod {0}. particella ignorata", intersect("Entita_Cod").ToString()), LogType.Informazione)
                    Continue For
                End If

                Dim wktParticella = dtWKTParticella.Rows(0).Item("wkt")

                _logger.AppendLog(LogLevel.AGG_CATASTO, String.Format("Elaborazione particella {3}-{4}-{5}-{6}-{7}-{8} (Piva: {0}, Sa_Cod: {1}, Appezza: {2})", Piva, Sa_Cod.ToString(), Appezza.ToString(), intersect("PROV").ToString(), intersect("COM").ToString(), intersect("SEZIONE").ToString(), intersect("FOGLIO").ToString(), intersect("NUMERO").ToString(), intersect("SUBALTERNO").ToString()), LogType.Informazione)
                Dim aree = Helper.getAreeParticella(wktAppezza, wktParticella, ObjParametri_Server, _logger)

                If aree.areaIntersezione > 0 AndAlso
                    CheckTolleranza(aree.areaIntersezione,
                                     Sup_App,
                                     GisIntersectionTollerance) Then

                    Dim updateParticella As Boolean = False
                    '0- recupero il codice della classe della pendenza
                    Dim classePendenza = LeggiobjParticelleCatastali_Acclivita(intersect("PROV"), intersect("COM"), "0", "0", "0", "0", ObjParametri_Server)

                    Dim particella = objParticella.Leggi_Particella_Anagrafica(Piva:=Piva,
                                                           Sa_Cod:=Sa_Cod,
                                                           Prov:=intersect("PROV"),
                                                           Com:=intersect("COM"),
                                                           Sezione:=intersect("SEZIONE"),
                                                           Foglio:=intersect("FOGLIO"),
                                                           Numero:=intersect("NUMERO"),
                                                           Subalterno:=intersect("SUBALTERNO"),
                                                           Leggi_Metodi_Produzione:=True,
                                                           Leggi_Macrousi:=True,
                                                           Leggi_Zone:=True,
                                                           Leggi_Classamento:=True,
                                                           ObjParametri_Server)
                    If particella.particella Is Nothing Then
                        'la particella non ha possesso sull'azienda corrente
                        Dim CentroPK As New AgronicaCoreModelsSTD.anagrafiche.CentroAziendale.PK(Sa_Cod, Piva)
                        particellaCatasto = Helper.MapParticellaToCatasto(CentroPK, intersect("PROV"), intersect("COM"), intersect("SEZIONE"), intersect("FOGLIO"), intersect("NUMERO"), intersect("SUBALTERNO"), aree.areaParticella, classePendenza, Nothing)
                        updateParticella = True
                    Else
                        'Lavez - 09/10/2025 - nessun aggiornamento della classePendenza per evitare di cancellare il dettaglio agea
                        'If (particella.particella.zonizzazione IsNot Nothing AndAlso
                        '    particella.particella.zonizzazione.Any(Function(x) {TipiEnumerativi.enum_Zone.A_LEGGERA_PENDENZA_NO_MAGGIORAZIONE,
                        '                                                        TipiEnumerativi.enum_Zone.B_PENDENZA_ACCENTUATA_MAGGIORAZIONE_20perc}.Contains(x.zona.codice))) Then
                        '    Dim zone = particella.particella.zonizzazione.Where(Function(x) {TipiEnumerativi.enum_Zone.A_LEGGERA_PENDENZA_NO_MAGGIORAZIONE,
                        '                                                        TipiEnumerativi.enum_Zone.B_PENDENZA_ACCENTUATA_MAGGIORAZIONE_20perc}.Contains(x.zona.codice)).ToList()

                        '    Select Case classePendenza
                        '        Case "A"
                        '            If zone(0).zona.codice <> TipiEnumerativi.enum_Zone.A_LEGGERA_PENDENZA_NO_MAGGIORAZIONE Then
                        '                zone(0).zona.codice = TipiEnumerativi.enum_Zone.A_LEGGERA_PENDENZA_NO_MAGGIORAZIONE
                        '                updateParticella = True
                        '            End If
                        '        Case "B"
                        '            If zone(0).zona.codice <> TipiEnumerativi.enum_Zone.B_PENDENZA_ACCENTUATA_MAGGIORAZIONE_20perc Then
                        '                zone(0).zona.codice = TipiEnumerativi.enum_Zone.B_PENDENZA_ACCENTUATA_MAGGIORAZIONE_20perc
                        '                updateParticella = True
                        '            End If
                        '        Case Else
                        '            Throw New Exception("Classe pendenza non mappata")
                        '    End Select

                        'End If
                        particellaCatasto = New AgronicaCoreDTOStd.InData.Anagrafica.ScriviCatasto(Nothing, particella)
                    End If

                    If updateParticella Then
                        _logger.AppendLog(LogLevel.AGG_CATASTO, String.Format("Aggiornamento classe pendenza per la particella {2}-{3}-{4}-{5}-{6}-{7} (Piva: {0}, Sa_Cod: {1})", Piva, Sa_Cod.ToString(), intersect("PROV").ToString(), intersect("COM").ToString(), intersect("SEZIONE").ToString(), intersect("FOGLIO").ToString(), intersect("NUMERO").ToString(), intersect("SUBALTERNO").ToString()), LogType.Informazione)
                        objParticellaW.ParticellaCatasto_Scrivi(particellaCatasto.newValue,
                                                                Nothing,
                                                                ObjParametri_Server,
                                                                ObjParametri_Utenti,
                                                                True,
                                                                "Servizio aggiornamento pendenza")
                    End If

                    Dim dtImpresexParticelle = objImpresexParticelleR.LeggiPeriodiValidita(Piva, Sa_Cod,
                                                        intersect("PROV"),
                                                        intersect("COM"),
                                                        intersect("SEZIONE"),
                                                        intersect("FOGLIO"),
                                                        intersect("NUMERO"),
                                                        intersect("SUBALTERNO"),
                                                        lsImpresexParticellePeriodi,
                                                        "", "Validita_Fine DESC", ObjParametri_Server)
                    If dtImpresexParticelle.Rows.Count > 0 Then
                        If dtImpresexParticelle(0)("Validita_Fine") < lastYearDay Then
                            'Validità inizio ImpresexParticelle settato al giorno dopo la validità fine dell'ultima possesso scaduto 
                            Dim ImpresexParticelle_ValiditaInizio As Date = dtImpresexParticelle(0)("Validita_Fine")
                            ImpresexParticelle_ValiditaInizio = ImpresexParticelle_ValiditaInizio.AddDays(1)


                            particellaCatasto.newValue.possessiParticella(0).validita.inizio = ImpresexParticelle_ValiditaInizio

                            nuoveParticellePerImpresa.Add(particellaCatasto.newValue)
                            _logger.AppendLog(LogLevel.AGG_CATASTO, String.Format("Estesa conduzione per la particella {2}-{3}-{4}-{5}-{6}-{7} (Piva: {0}, Sa_Cod: {1})", Piva, Sa_Cod.ToString(), intersect("PROV").ToString(), intersect("COM").ToString(), intersect("SEZIONE").ToString(), intersect("FOGLIO").ToString(), intersect("NUMERO").ToString(), intersect("SUBALTERNO").ToString()), LogType.Informazione)
                        End If
                    Else
                        nuoveParticellePerImpresa.Add(particellaCatasto.newValue)
                        _logger.AppendLog(LogLevel.AGG_CATASTO, String.Format("Aggiunta conduzione per la particella {2}-{3}-{4}-{5}-{6}-{7} (Piva: {0}, Sa_Cod: {1})", Piva, Sa_Cod.ToString(), intersect("PROV").ToString(), intersect("COM").ToString(), intersect("SEZIONE").ToString(), intersect("FOGLIO").ToString(), intersect("NUMERO").ToString(), intersect("SUBALTERNO").ToString()), LogType.Informazione)
                    End If

                    If aree.areaIntersezione > 0 Then
                        Dim catastoAppezzamento = New AgronicaCoreModelsSTD.anagrafiche.CatastoAppezzamento() With {
                              .particella = New AgronicaCoreModelsSTD.anagrafiche.ParticelleCatastali.PK(
                                  intersect("PROV"),
                                  intersect("COM"),
                                  intersect("SEZIONE"),
                                  intersect("FOGLIO"),
                                  intersect("NUMERO"),
                                  intersect("SUBALTERNO")),
                              .area = aree.areaIntersezione,
                              .flag_cancellazione = False}

                        nuoveParticellePerAppezza.Add(catastoAppezzamento)
                    End If
                Else
                    _logger.AppendLog(String.Format("Particella con superficie 0 o con intersezione inferiore alla soglia di tolleranza. Particella {3}-{4}-{5}-{6}-{7}-{8} (Piva: {0}, Sa_Cod: {1}, Appezza: {2})", Piva, Sa_Cod.ToString(), Appezza.ToString(), intersect("PROV").ToString(), intersect("COM").ToString(), intersect("SEZIONE").ToString(), intersect("FOGLIO").ToString(), intersect("NUMERO").ToString(), intersect("SUBALTERNO").ToString()), LogType.Informazione)
                End If
            Next

            'Creazione particelle catastali per impresa
            If nuoveParticellePerImpresa.Count > 0 Then
                _logger.AppendLog(LogLevel.AGG_CATASTO, String.Format("Nuove imprese per particelle: "))

                For Each pxi In nuoveParticellePerImpresa
                    objImpresexParticelleW.Scrivi_2(Piva,
                                                    Sa_Cod,
                                                    pxi.particella.primaryKey.Prov,
                                                    pxi.particella.primaryKey.Com,
                                                    pxi.particella.primaryKey.Sezione,
                                                    pxi.particella.primaryKey.Foglio,
                                                    pxi.particella.primaryKey.Numero,
                                                    pxi.particella.primaryKey.Subalterno,
                                                    "",
                                                    pxi.possessiParticella(0).titolo_Di_Possesso.codice,
                                                    pxi.possessiParticella(0).Area,
                                                    pxi.possessiParticella(0).validita.inizio,
                                                    pxi.possessiParticella(0).validita.fine,
                                                    ObjParametri_Server)

                    _logger.AppendLog(LogLevel.AGG_CATASTO, String.Format("-  Piva: {0}, Sa_Cod: {1}, PROV: {2}, COM: {3}, SEZIONE: {4}, FOGLIO: {5}, NUMERO: {6}, SUBALTERNO: {7}",
                                                        Piva,
                                                        Sa_Cod,
                                                        pxi.particella.primaryKey.Prov,
                                                        pxi.particella.primaryKey.Com,
                                                        pxi.particella.primaryKey.Sezione,
                                                        pxi.particella.primaryKey.Foglio,
                                                        pxi.particella.primaryKey.Numero,
                                                        pxi.particella.primaryKey.Subalterno))
                Next
            End If

            Dim flgAggiornaAppezza As Boolean = False
            'Aggiornamento appezzamento
            appezzamento = objAppezzamentoR.Leggi_Appezzamento_Anagrafica(Piva,
                                                                      Sa_Cod,
                                                                      Appezza,
                                                                      0,
                                                                      False,
                                                                      False,
                                                                      True, 'nuoveParticellePerAppezza.Count > 0,
                                                                      New Date, False,
                                                                      False,
                                                                      False,
                                                                      ObjParametri_SuperServer,
                                                                      ObjParametri_Server,
                                                                      ObjParametri_Utenti)

            If nuoveParticellePerAppezza.Count > 0 Then

                _logger.AppendLog(LogLevel.ALL, String.Format("Nuove particelle per appezzamento: "))

                For Each pxa In nuoveParticellePerAppezza
                    'If Not appezzamento.catastoAppezzamento.Where(Function(x) x.particella.Prov = particellaCatasto.particella.Prov And
                    '                                                  x.particella.Com = particellaCatasto.particella.Com And
                    '                                                  x.particella.Sezione = particellaCatasto.particella.Sezione And
                    '                                                  x.particella.Foglio = particellaCatasto.particella.Foglio And
                    '                                                  x.particella.Numero = particellaCatasto.particella.Numero And
                    '                                                  x.particella.Subalterno = particellaCatasto.particella.Subalterno).Any() Then
                    appezzamento.catastoAppezzamento.Add(pxa)

                    _logger.AppendLog(LogLevel.ALL, String.Format("-  Piva: {0}, Sa_Cod: {1}, Appezza: {2}, PROV: {3}, COM: {4}, SEZIONE: {5}, FOGLIO: {6}, NUMERO: {7}, SUBALTERNO: {8}",
                                                        Piva,
                                                        Sa_Cod,
                                                        Appezza,
                                                        pxa.particella.Prov,
                                                        pxa.particella.Com,
                                                        pxa.particella.Sezione,
                                                        pxa.particella.Foglio,
                                                        pxa.particella.Numero,
                                                        pxa.particella.Subalterno))
                    'End If
                Next
                flgAggiornaAppezza = True
            Else
                If appezzamento.catastoAppezzamento IsNot Nothing AndAlso appezzamento.catastoAppezzamento.Count > 0 Then
                    'reset appezzamenti per particell
                    appezzamento.catastoAppezzamento = Nothing

                    _logger.AppendLog(LogLevel.ALL, String.Format("- Reset appezzamenti per particelle Piva: {0}, Sa_Cod: {1}, Appezza: {2}",
                                                        Piva,
                                                        Sa_Cod,
                                                        Appezza))
                    flgAggiornaAppezza = True
                End If
            End If

            If flgAggiornaAppezza Then
                _logger.AppendLog(LogLevel.ALL, String.Format("Appezzamento (Piva: {0}, Sa_Cod: {1}, Appezza: {2}) aggiorno con catasto GIS", Piva, Sa_Cod.ToString(), Appezza.ToString()))
                Dim scritturaOK = objAppezzamentoW.Appezzamento_ScriviModifica(appezzamento, ObjParametri_Server, ObjParametri_Utenti)

                If scritturaOK Then
                    _logger.AppendLog(LogLevel.ALL, String.Format("Appezzamento (Piva: {0}, Sa_Cod: {1}, Appezza: {2}) aggiornato", Piva, Sa_Cod.ToString(), Appezza.ToString()))
                    _logger.LogImportazionePendenza(Piva, Sa_Cod, Appezza, "", "", 0, True, logDbNote, ObjParametri_Server)
                    logSuccessCount += 1
                Else
                    _logger.AppendLog(LogLevel.ERRORI, String.Format("Aggiornamento fallito per appezzamento (Piva: {0}, Sa_Cod: {1}, Appezza: {2}), appezzamento non aggiornat", Piva, Sa_Cod.ToString(), Appezza.ToString()), LogType.Errore)
                    _logger.LogImportazionePendenza(Piva, Sa_Cod, Appezza, "", "", 0, False, "", ObjParametri_Server)
                    logErroriCount += 1
                End If
            Else
                _logger.AppendLog(LogLevel.ALL, String.Format("Appezzamento (Piva: {0}, Sa_Cod: {1}, Appezza: {2}) nessun aggiornamento da effettuare", Piva, Sa_Cod.ToString(), Appezza.ToString()))
                _logger.LogImportazionePendenza(Piva, Sa_Cod, Appezza, "", "", 0, True, logDbNote, ObjParametri_Server)
                logSuccessCount += 1
            End If

        Else
            _logger.AppendLog(LogLevel.ERRORI, String.Format("Nessuna intersezione trovata sul layer catasto per appezzamento (Piva: {0}, Sa_Cod: {1}, Appezza: {2}), pendenza appezzamento non aggiornata", Piva, Sa_Cod.ToString(), Appezza.ToString()), LogType.Errore)
            _logger.LogImportazionePendenza(Piva, Sa_Cod, Appezza, "", "", 0, False, "Nessuna intersezione trovata sul layer catasto per appezzamento", ObjParametri_Server)
        End If
    End Sub

    Private Function CheckTolleranza(ByVal AreaIntersezione As Double,
                                     ByVal AreaAppezzamento As Double,
                                     ByVal Tolleranza As Double) As Boolean
        If ((Math.Round(AreaIntersezione, 4) * 100) / AreaAppezzamento) > Tolleranza Then
            Return True
        Else
            Return False
        End If
    End Function

    Private Function ElaboraPendenzeAGEA(ByVal Piva As String,
                                    ByVal Sa_Cod As Integer,
                                    ByVal Appezza As Integer,
                                    ByVal wkt As String,
                                    ByRef pendenzeController As ReUmbriaApiProvider,
                                    ByRef logSuccessCount As Integer,
                                    ByRef logErroriCount As Integer,
                                    ByRef ObjParametri_SuperServer As AgronicaCoreParametri,
                                    ByRef ObjParametri_Server As AgronicaCoreParametri,
                                    ByRef ObjParametri_Utenti As AgronicaCoreParametri
                                    )

        Dim objParticelleCatastaliR As New AgronicaCoreAnagrafeDAL.ParticelleCatastali_R
        Dim objImpresexParticelleR As New AgronicaCoreAnagrafeDAL.ImpresexParticelle2_R
        Dim objImpresexParticelleW As New AgronicaCoreAnagrafeDAL.ImpresexParticelle2_W
        Dim objAppezzamentoR As New AgronicaCoreAnagrafeBIZ.Appezzamento_R
        Dim objAppezzamentoW As New AgronicaCoreAnagrafeBIZ.Appezzamento_W
        Dim objParticelleCatastaliW As New AgronicaCoreAnagrafeBIZ.Particella_W

        Dim appezzamento As AgronicaCoreModelsSTD.anagrafiche.Appezzamento

        Dim poligono As String = ""
        Dim pendenzaDTO As AgronicaCoreDTOStd.InData.DataExchange.Pendenza = Nothing
        Dim logPendenzaDTO As AgronicaCoreDTOStd.InData.DataExchange.Pendenza = Nothing
        Dim logPendenzaStatusCode As Integer = 0

        Dim ret As Boolean = False

        Try

            Dim PROV As String = ""
            Dim COM As String = ""
            Dim numeroParticella As Integer = 0
            Dim logDbNote As String = ""
            Dim nuoveParticellePerAppezza As New List(Of AgronicaCoreModelsSTD.anagrafiche.CatastoAppezzamento)
            Dim scritturaOK As Boolean = False
            'Dim deleteParticellePerAppezza As New List(Of AgronicaCoreModelsSTD.anagrafiche.CatastoAppezzamento)
            Dim nuoveParticellePerImpresa As New List(Of AgronicaCoreModelsSTD.anagrafiche.CatastoCentroAziendale)
            Dim particelleDaCreare As New List(Of AgronicaCoreDTOStd.InData.Anagrafica.ScriviCatasto)

            Dim lsImpresexParticellePeriodi As New List(Of ImpresexParticelle2_Periodo)
            Dim lastYearDay As Date = New Date(Date.Now.Year, 12, 31)

            poligono = Helper.GetCartography(wkt, 4326, _parametriTask.API_pendenze_epsg)

            'Chiamata ad API per lettura pendenza
            pendenzaDTO = pendenzeController.LeggiPendenza(poligono, logPendenzaStatusCode)

            logPendenzaDTO = pendenzaDTO

            If IsNothing(pendenzaDTO) Then
                Throw New InvalidPendenzeDTOException(String.Format("Ritornato DTO Pendenza nullo"))
            ElseIf IsNothing(pendenzaDTO.particelle) OrElse IsNothing(pendenzaDTO.riepilogo) OrElse IsNothing(pendenzaDTO.properties) Then
                Throw New InvalidPendenzeDTOException(String.Format("Ritornato DTO Pendenza non valido: campi inner class mancanti"))
            ElseIf pendenzaDTO.particelle.Count = 0 Then
                Throw New InvalidPendenzeDTOException(String.Format("Ritornato DTO Pendenza non valido: elenco particelle vuoto"))
            End If

            For Each pendenzaParticella In pendenzaDTO.particelle

                If Not LeggiProvComIstat(pendenzaParticella.codNazionale, PROV, COM, ObjParametri_Server) Then
                    Throw New IstatCodeNotFoundException(String.Format("Codice ISTAT {0} non trovato, per cartografia {1}", pendenzaParticella.codNazionale, poligono))
                End If

                If Not Integer.TryParse(pendenzaParticella.particella, numeroParticella) Then
                    Throw New InvalidPendenzeDTOException(String.Format("Ritornato DTO Pendenza non valido per cartografia {0}: campo 'particella' non numerico per particella", poligono))
                End If

                Dim CentroPK As New AgronicaCoreModelsSTD.anagrafiche.CentroAziendale.PK(Sa_Cod, Piva)
                Dim particellaCatasto = Helper.MapPendenzaParticellaToCatasto(CentroPK, PROV, COM, numeroParticella, pendenzaParticella, Nothing)
                Dim catastoAppezzamento = New AgronicaCoreModelsSTD.anagrafiche.CatastoAppezzamento() With {
                                                  .particella = New AgronicaCoreModelsSTD.anagrafiche.ParticelleCatastali.PK(
                                                      particellaCatasto.newValue.particella.primaryKey.Prov,
                                                      particellaCatasto.newValue.particella.primaryKey.Com,
                                                      particellaCatasto.newValue.particella.primaryKey.Sezione,
                                                      particellaCatasto.newValue.particella.primaryKey.Foglio,
                                                      particellaCatasto.newValue.particella.primaryKey.Numero,
                                                      particellaCatasto.newValue.particella.primaryKey.Subalterno),
                                                  .area = Math.Round(pendenzaParticella.areaIntersecata, 0) / 10000,
                                                  .flag_cancellazione = False}

                nuoveParticellePerAppezza.Add(catastoAppezzamento)

                If objParticelleCatastaliR.Leggi(0,
                                                 particellaCatasto.newValue.particella.primaryKey.Prov,
                                                 particellaCatasto.newValue.particella.primaryKey.Com,
                                                 particellaCatasto.newValue.particella.primaryKey.Sezione,
                                                 particellaCatasto.newValue.particella.primaryKey.Foglio,
                                                 particellaCatasto.newValue.particella.primaryKey.Numero,
                                                 particellaCatasto.newValue.particella.primaryKey.Subalterno,
                                                 AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                                 "", "", ObjParametri_Server
                                                 ).Rows.Count = 0 Then
                    particelleDaCreare.Add(particellaCatasto)

                Else
                    Dim dtImpresexParticelle = objImpresexParticelleR.LeggiPeriodiValidita(Piva, Sa_Cod,
                                                         particellaCatasto.newValue.particella.primaryKey.Prov,
                                                         particellaCatasto.newValue.particella.primaryKey.Com,
                                                         particellaCatasto.newValue.particella.primaryKey.Sezione,
                                                         particellaCatasto.newValue.particella.primaryKey.Foglio,
                                                         particellaCatasto.newValue.particella.primaryKey.Numero,
                                                         particellaCatasto.newValue.particella.primaryKey.Subalterno,
                                                         lsImpresexParticellePeriodi,
                                                         "", "Validita_Fine DESC", ObjParametri_Server)
                    If dtImpresexParticelle.Rows.Count > 0 Then
                        If dtImpresexParticelle(0)("Validita_Fine") < lastYearDay Then
                            'Validità inizio ImpresexParticelle settato al giorno dopo la validità fine dell'ultima possesso scaduto 
                            Dim ImpresexParticelle_ValiditaInizio As Date = dtImpresexParticelle(0)("Validita_Fine")
                            ImpresexParticelle_ValiditaInizio = ImpresexParticelle_ValiditaInizio.AddDays(1)
                            particellaCatasto.newValue.possessiParticella(0).validita.inizio = ImpresexParticelle_ValiditaInizio

                            nuoveParticellePerImpresa.Add(particellaCatasto.newValue)
                        End If
                    Else
                        nuoveParticellePerImpresa.Add(particellaCatasto.newValue)
                    End If
                End If
            Next

            'Creazione particelli catastali mancanti
            If particelleDaCreare.Count > 0 Then
                _logger.AppendLog(LogLevel.AGG_CATASTO, String.Format("Rilevate nuove particelle catastali da creare per appezzamento (Piva: {0}, Sa_Cod: {1}, Appezza: {2}): ", Piva, Sa_Cod.ToString(), Appezza.ToString()))

                For Each particellaCatasto In particelleDaCreare
                    _logger.AppendLog(LogLevel.AGG_CATASTO, String.Format("-  particella: PROV: {0}, COM: {1}, SEZIONE: {2}, FOGLIO: {3}, NUMERO: {4}, SUBALTERNO: {5}, Area: {6}, Zonizzazione: {7}",
                                                        particellaCatasto.newValue.particella.primaryKey.Prov,
                                                        particellaCatasto.newValue.particella.primaryKey.Com,
                                                        particellaCatasto.newValue.particella.primaryKey.Sezione,
                                                        particellaCatasto.newValue.particella.primaryKey.Foglio.ToString(),
                                                        particellaCatasto.newValue.particella.primaryKey.Numero.ToString(),
                                                        particellaCatasto.newValue.particella.primaryKey.Subalterno,
                                                        particellaCatasto.newValue.particella.Area.ToString(),
                                                        If(Not IsNothing(particellaCatasto.newValue.particella.zonizzazione) AndAlso particellaCatasto.newValue.particella.zonizzazione.Count > 0,
                                                            particellaCatasto.newValue.particella.zonizzazione(0).zona.codice.ToString(),
                                                            "")))
                Next

                scritturaOK = objParticelleCatastaliW.ParticelleCatastali_Scrivi(particelleDaCreare, ObjParametri_Server, ObjParametri_Utenti, False, NoteLog:=NOTELOG_ANAGRAFE_NG)

                If scritturaOK Then
                    _logger.AppendLog(LogLevel.AGG_CATASTO, "Particelle catastali create")
                Else
                    'Se fallisce la creazione delle particelle blocco aggiornamento appezzamento
                    Throw New GiasException("Procedura di scrittua particelle catastali fallita")
                End If
            End If

            'Creazione particelle catastali per impresa
            If nuoveParticellePerImpresa.Count > 0 Then
                _logger.AppendLog(LogLevel.AGG_CATASTO, String.Format("Nuove imprese per particelle: "))

                For Each particellaCatasto In nuoveParticellePerImpresa
                    objImpresexParticelleW.Scrivi_2(Piva,
                                                    Sa_Cod,
                                                    particellaCatasto.particella.primaryKey.Prov,
                                                    particellaCatasto.particella.primaryKey.Com,
                                                    particellaCatasto.particella.primaryKey.Sezione,
                                                    particellaCatasto.particella.primaryKey.Foglio,
                                                    particellaCatasto.particella.primaryKey.Numero,
                                                    particellaCatasto.particella.primaryKey.Subalterno,
                                                    "",
                                                    particellaCatasto.possessiParticella(0).titolo_Di_Possesso.codice,
                                                    particellaCatasto.possessiParticella(0).Area,
                                                    particellaCatasto.possessiParticella(0).validita.inizio,
                                                    particellaCatasto.possessiParticella(0).validita.fine,
                                                    ObjParametri_Server)

                    _logger.AppendLog(LogLevel.AGG_CATASTO, String.Format("-  Piva: {0}, Sa_Cod: {1}, PROV: {2}, COM: {3}, SEZIONE: {4}, FOGLIO: {5}, NUMERO: {6}, SUBALTERNO: {7}",
                                                        Piva,
                                                        Sa_Cod,
                                                        particellaCatasto.particella.primaryKey.Prov,
                                                        particellaCatasto.particella.primaryKey.Com,
                                                        particellaCatasto.particella.primaryKey.Sezione,
                                                        particellaCatasto.particella.primaryKey.Foglio,
                                                        particellaCatasto.particella.primaryKey.Numero,
                                                        particellaCatasto.particella.primaryKey.Subalterno))
                Next
            End If

            'Aggiornamento appezzamento
            appezzamento = objAppezzamentoR.Leggi_Appezzamento_Anagrafica(Piva,
                                                                          Sa_Cod,
                                                                          Appezza,
                                                                          0,
                                                                          False,
                                                                          False,
                                                                          True, 'nuoveParticellePerAppezza.Count > 0,
                                                                          New Date, False,
                                                                          False,
                                                                          False,
                                                                          ObjParametri_SuperServer,
                                                                          ObjParametri_Server,
                                                                          ObjParametri_Utenti)
            appezzamento.pendenza = pendenzaDTO.riepilogo.pendenzaPct

            If nuoveParticellePerAppezza.Count > 0 Then
                _logger.AppendLog(LogLevel.ALL, String.Format("Nuovi appezzamenti per particelle: "))

                For Each particellaCatasto In nuoveParticellePerAppezza
                    appezzamento.catastoAppezzamento.Add(particellaCatasto)

                    _logger.AppendLog(LogLevel.ALL, String.Format("-  Piva: {0}, Sa_Cod: {1}, Appezza: {2}, PROV: {3}, COM: {4}, SEZIONE: {5}, FOGLIO: {6}, NUMERO: {7}, SUBALTERNO: {8}",
                                                        Piva,
                                                        Sa_Cod,
                                                        Appezza,
                                                        particellaCatasto.particella.Prov,
                                                        particellaCatasto.particella.Com,
                                                        particellaCatasto.particella.Sezione,
                                                        particellaCatasto.particella.Foglio,
                                                        particellaCatasto.particella.Numero,
                                                        particellaCatasto.particella.Subalterno))
                Next
            End If

            If nuoveParticellePerAppezza.Count = 0 Then
                'Non si aggiorna il catasto (tabella AppezzamentiXParticelle) se non ci sono modifiche
                appezzamento.catastoAppezzamento = Nothing
            Else
                'Altrimenti si verifica che la somma delle aree del catasto sia allineata alla superficie dell'appezzamento
                Dim maxAreaCatasto As AgronicaCoreModelsSTD.anagrafiche.CatastoAppezzamento = Nothing
                Dim totSupCatastoApp As Decimal = 0
                For Each catastoApp In appezzamento.catastoAppezzamento
                    totSupCatastoApp += catastoApp.area
                    If IsNothing(maxAreaCatasto) OrElse catastoApp.area > maxAreaCatasto.area Then
                        maxAreaCatasto = catastoApp
                    End If
                Next
                If totSupCatastoApp <> appezzamento.superficie And Not IsNothing(maxAreaCatasto) Then
                    Dim areaCatastoRev = maxAreaCatasto.area + (appezzamento.superficie - totSupCatastoApp)
                    If areaCatastoRev > 0 Then
                        logDbNote = String.Format("Area catasto su particella Prov:{0},Com:{1},Sezione:{2},Foglio:{3},Numero:{4},Subalterno:{5} ripartita da {6} a {7} per allineare a superficie appezzamento",
                                                  maxAreaCatasto.particella.Prov,
                                                  maxAreaCatasto.particella.Com,
                                                  maxAreaCatasto.particella.Sezione,
                                                  maxAreaCatasto.particella.Foglio.ToString(),
                                                  maxAreaCatasto.particella.Numero.ToString(),
                                                  maxAreaCatasto.particella.Subalterno,
                                                  maxAreaCatasto.area,
                                                  areaCatastoRev)
                        maxAreaCatasto.area = areaCatastoRev
                    Else
                        Throw New GiasException("Impossibile allineare somma aree catasto a superficie totale appezzamento")
                    End If
                End If
            End If

            scritturaOK = objAppezzamentoW.Appezzamento_ScriviModifica(appezzamento, ObjParametri_Server, ObjParametri_Utenti)

            If scritturaOK Then
                _logger.AppendLog(LogLevel.ALL, String.Format("Appezzamento (Piva: {0}, Sa_Cod: {1}, Appezza: {2}) aggiornato: pendenza: {3}.", Piva, Sa_Cod.ToString(), Appezza.ToString(), appezzamento.pendenza.ToString()))
                _logger.LogImportazionePendenza(Piva, Sa_Cod, Appezza, poligono, logPendenzaDTO, logPendenzaStatusCode, True, logDbNote, ObjParametri_Server)
                logSuccessCount += 1
                ret = True
            Else
                _logger.AppendLog(LogLevel.ERRORI, String.Format("Aggiornamento fallito per appezzamento (Piva: {0}, Sa_Cod: {1}, Appezza: {2}), pendenza appezzamento non aggiornata", Piva, Sa_Cod.ToString(), Appezza.ToString()), LogType.Errore)
                _logger.LogImportazionePendenza(Piva, Sa_Cod, Appezza, poligono, logPendenzaDTO, logPendenzaStatusCode, False, "", ObjParametri_Server)
                logErroriCount += 1
                ret = False
            End If
        Catch ex As ReUmbriaApiTimeoutException
            _logger.AppendLog(LogLevel.ERRORI, String.Format("Errore di aggiornamento per appezzamento (Piva: {0}, Sa_Cod: {1}, Appezza: {2}): {3}", Piva, Sa_Cod.ToString(), Appezza.ToString(), ex.Message), LogType.Errore)
            _logger.AppendLog(LogLevel.ERRORI, "Importazione pendenze interrotta per chiamate ad Api in time-out")
            _logger.LogImportazionePendenza(Piva, Sa_Cod, Appezza, poligono, logPendenzaDTO, logPendenzaStatusCode, False, "Errore in chiamata ad Api Regione Umbria", ObjParametri_Server)
            'logErroriCount += 1

            ret = False

        Catch ex As ReUmbriaApiProviderException
            _logger.AppendLog(LogLevel.ERRORI, String.Format("Errore di aggiornamento per appezzamento (Piva: {0}, Sa_Cod: {1}, Appezza: {2}): {3}", Piva, Sa_Cod.ToString(), Appezza.ToString(), ex.Message), LogType.Errore)
            _logger.LogImportazionePendenza(Piva, Sa_Cod, Appezza, poligono, logPendenzaDTO, logPendenzaStatusCode, False, "Errore in chiamata ad Api Regione Umbria", ObjParametri_Server)
            logErroriCount += 1

            ret = False

        Catch ex As Exception
            _logger.AppendLog(LogLevel.ERRORI, String.Format("Errore di aggiornamento per appezzamento (Piva: {0}, Sa_Cod: {1}, Appezza: {2}): {3}", Piva, Sa_Cod.ToString(), Appezza.ToString(), ex.Message), LogType.Errore)
            _logger.LogImportazionePendenza(Piva, Sa_Cod, Appezza, poligono, logPendenzaDTO, logPendenzaStatusCode, False, ex.Message, ObjParametri_Server)
            logErroriCount += 1

            ret = False

        End Try
        Return ret
    End Function

    Private Function LeggiobjParticelleCatastali_Acclivita(ByVal PROV As String,
                                                           ByVal COM As String,
                                                           ByVal SEZIONE As String,
                                                           ByVal FOGLIO As String,
                                                           ByVal NUMERO As String,
                                                           ByVal SUBALTERNO As String,
                                                           ByRef objParametri_Server As AgronicaCoreParametri
                                                           ) As String
        Dim ret As String = "A"      'default A no maggiorazione

        Dim objParticelleCatastali_Acclivita As New AgronicaCoreMetaSchemaDAL.ParticelleCatastali_Acclivita_R

        Dim dt = objParticelleCatastali_Acclivita.Leggi(PROV,
                                                        COM,
                                                        SEZIONE,
                                                        FOGLIO,
                                                        NUMERO,
                                                        SUBALTERNO,
                                                        "",
                                                        "",
                                                        objParametri_Server)
        If dt.Rows.Count > 0 Then
            ret = dt.Rows(0)("FG_Zona_Acclivita")
        End If
        Return ret
    End Function

    Private Function LeggiProvComIstat(ByVal codNazionaleIstat As String, ByRef PROV As String, ByRef COM As String, ByRef ObjParametri_Server As AgronicaCoreParametri) As Boolean

        If Not _dictIstatProvCom.ContainsKey(codNazionaleIstat) Then
            Dim dtIstat = _objIstat.Leggi("", "", "", "", "",
                                         AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                         "", "",
                                         ObjParametri_Server,
                                         codNazionaleIstat)
            If dtIstat.Rows.Count > 0 Then
                Dim rowIstat = dtIstat.Rows(0)
                _dictIstatProvCom(codNazionaleIstat) = New Tuple(Of String, String)(rowIstat.Item("PROV"), rowIstat.Item("COM"))
            Else
                Return False
            End If
        End If

        Dim tuplaProvCom = _dictIstatProvCom(codNazionaleIstat)
        PROV = tuplaProvCom.Item1
        COM = tuplaProvCom.Item2

        Return True
    End Function

    Public Sub Dispose() Implements IDisposable.Dispose
        _helper.Dispose()
        _logger.Dispose()
        _helper = Nothing
        _logger = Nothing
        _objParametriSuperServer = Nothing
        _objParametriServer = Nothing
        _objParametriUtenti = Nothing
        _parametriTask = Nothing
    End Sub
End Class
