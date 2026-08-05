
Imports System.IO
Imports System.Text
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreMeteoDAL
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreVarieDAL
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq


Public Class ModelliPrevisionaliCalcolatore

    Public Sub New()
    End Sub

    Protected Function _creaModello(meteoHelper As MeteoDSS_Helper, pm As ParametriModello, flagIndicatori As Boolean, objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As AbstractModello

        Dim xModello As AbstractModello = Nothing

        Select Case pm.Mod_Cod

            Case enum_ModelliPrevisionali.Calcola_Ticchiolatura_A_Scab

                If Not flagIndicatori Then
                    xModello = New TicchiolaturaMelo(meteoHelper.DatiMeteo, pm.Mod_Cod, pm.Alg_Cod, objParametri)
                End If

            Case enum_ModelliPrevisionali.Ritardo_Variabile

                If Not flagIndicatori Then
                    xModello = New RitardoVariabile(meteoHelper.DatiMeteo, pm.Mod_Cod, pm.Veg_Cod, pm.Avv_Cod, pm.Alg_Cod, objParametri)
                End If

            Case enum_ModelliPrevisionali.Agronomica30_BatteriosiKiwi_PSA

                xModello = New Agronomica30_BatteriosiKiwi_PSA(meteoHelper.DatiMeteo)

            Case enum_ModelliPrevisionali.Agronomica30_Peronospora

                xModello = New Agronomica30_Peronospora(meteoHelper.DatiMeteo)

            Case enum_ModelliPrevisionali.Agronomica30_OidioVite

                xModello = New Agronomica30_OidioVite(meteoHelper.DatiMeteo)

            Case enum_ModelliPrevisionali.Agronomica30_BotriteVite

                xModello = New Agronomica30_BotriteVite(meteoHelper.DatiMeteo)

            Case enum_ModelliPrevisionali.Agronomica30_TicchiolaturaMelo

                xModello = New Agronomica30_TicchiolaturaMelo(meteoHelper.DatiMeteo)

            Case enum_ModelliPrevisionali.Agronomica30_MaculaturaPero

                xModello = New Agronomica30_MaculaturaPero(meteoHelper.DatiMeteo)

            Case enum_ModelliPrevisionali.Agronomica30_MISP_IPI_Pomodoro

                xModello = New Agronomica30_Pom_MISP_IPI(meteoHelper.DatiMeteo, pm.Params)

            Case enum_ModelliPrevisionali.Agronomica30_MRV_Eulia,
                 enum_ModelliPrevisionali.Agronomica30_MRV_CydiaMolesta,
                 enum_ModelliPrevisionali.Agronomica30_MRV_Carpocapsa,
                 enum_ModelliPrevisionali.Agronomica30_MRV_Helicoverpa,
                 enum_ModelliPrevisionali.Agronomica30_MRV_Tignoletta,
                 enum_ModelliPrevisionali.MRV_PiralideMais,
                 enum_ModelliPrevisionali.MRV_NottuaMais

                xModello = New Agronomica30_RitardoVariabile(meteoHelper.DatiMeteo, pm.Mod_Cod, meteoHelper.Lat, meteoHelper.Lng, meteoHelper.Timezone)

            Case enum_ModelliPrevisionali.Agronomica30_ColpoDiFuoco

                xModello = New Agronomica30_ColpoDiFuoco(meteoHelper.DatiMeteo, pm.Veg_Cod)

            Case enum_ModelliPrevisionali.UniCatt_Mais_AFLA,
                 enum_ModelliPrevisionali.UniCatt_Mais_FER

                xModello = New UniCatt_Mais(meteoHelper.DatiMeteo, pm.Mod_Cod, pm.Params)

            Case enum_ModelliPrevisionali.UniCatt_Frumento_Fusariosi

                If Not flagIndicatori Then
                    xModello = New UniCatt_Frumento(meteoHelper.DatiMeteo, pm.Params)
                End If

            Case enum_ModelliPrevisionali.Racca_PeroPom,
                 enum_ModelliPrevisionali.Racca_AlterPom,
                 enum_ModelliPrevisionali.Racca_OidioPom,
                 enum_ModelliPrevisionali.Racca_BotriPom,
                 enum_ModelliPrevisionali.Racca_PeroBiet,
                 enum_ModelliPrevisionali.Racca_OidioBiet,
                 enum_ModelliPrevisionali.Racca_CercoBiet,
                 enum_ModelliPrevisionali.Racca_PeroPat,
                 enum_ModelliPrevisionali.Racca_AlterPat,
                 enum_ModelliPrevisionali.Racca_ScleroSoia,
                 enum_ModelliPrevisionali.Racca_BrusoneRiso,
                 enum_ModelliPrevisionali.Racca_ElmintosporiosiMais,
                 enum_ModelliPrevisionali.Racca_BipolarisMaidis,
                 enum_ModelliPrevisionali.Racca_AntracnosiOlivo

                xModello = New Racca(meteoHelper.DatiMeteo, pm.Mod_Cod, pm.Veg_Cod, pm.Avv_Cod, pm.Params)

            Case enum_ModelliPrevisionali.Racca_MaisMicotox

                If Not flagIndicatori Then
                    xModello = New Racca(meteoHelper.DatiMeteo, pm.Mod_Cod, pm.Veg_Cod, pm.Avv_Cod, pm.Params)
                End If

            Case enum_ModelliPrevisionali.ProvvisorioOlivo

                If Not flagIndicatori Then
                    xModello = New Agronomica30_RitardoVariabile(meteoHelper.DatiMeteo, enum_ModelliPrevisionali.Agronomica30_MRV_CydiaMolesta, meteoHelper.Lat, meteoHelper.Lng, meteoHelper.Timezone)
                End If

            Case enum_ModelliPrevisionali.BetaCoProB_Cercosporiosi

                xModello = New BetaCoProB_Cercosporiosi(meteoHelper.DatiMeteo, pm.Params)

            Case enum_ModelliPrevisionali.RaccaFrumento_RuggineBruna,
                 enum_ModelliPrevisionali.RaccaFrumento_RuggineGialla,
                 enum_ModelliPrevisionali.RaccaFrumento_RuggineNera,
                 enum_ModelliPrevisionali.RaccaFrumento_Stagonosporiosi,
                 enum_ModelliPrevisionali.RaccaFrumento_Septoria,
                 enum_ModelliPrevisionali.RaccaFrumento_Fusariosi,
                 enum_ModelliPrevisionali.RaccaFrumento_FusariosiSpiga,
                 enum_ModelliPrevisionali.RaccaFrumento_Fusariosi2,
                 enum_ModelliPrevisionali.RaccaFrumento_Fusariosi3,
                 enum_ModelliPrevisionali.RaccaFrumento_MarciumeRosa

                xModello = New RaccaFrumento(meteoHelper.DatiMeteo, pm.Mod_Cod, pm.Veg_Cod, pm.Avv_Cod, pm.Params)

        End Select

        Return xModello
    End Function


    ''' <summary>
    ''' Calcolo di un modello previsionale secondo i parametri passati
    ''' </summary>    
    ''' <param name="objParametri"></param>
    ''' <param name=""></param>
    ''' <returns></returns>
    Public Function ElaboraModelloDaParametri(pcm As ParametriCalcoloModello, objParametri As AgronicaCoreParametri) As rispostaStandard(Of cRisultatoModello)

        Dim rval As New rispostaStandard(Of cRisultatoModello)

        'per enum_ModelliPrevisionali.Ritardo_Variabile
        ' VAnni: 3/4/2017: qualsiasi periodo imposto la partenza sui inizio anno.
        'DataInizio = DateSerial(Year(DataInizio), 1, 1)

        If RaccaFrumento.Contains(pcm.Modello.Mod_Cod) Then

            Dim ris = RaccaFrumento.VerificaParametri(pcm)

            If Not ris.Result Then

                rval.RispostaOK = False
                rval.Errore = ris.Message
                Return rval
            End If
        End If

        Dim helper = New MeteoDSS_Helper(objParametri)

        If Not helper.Leggi(pcm.Meteo) Then
            rval.RispostaOK = False
            rval.Errore = helper.StatusMsg
            Return rval
        End If

        If Not VerificaMeteoXModello(helper, pcm.Modello.Mod_Cod, rval.Errore) Then
            rval.RispostaOK = False
            Return rval
        End If

        Dim xModello As AbstractModello = _creaModello(helper, pcm.Modello, False, objParametri)

        If xModello Is Nothing Then

            rval.RispostaOK = False
            rval.Errore = "Algoritmo non previsto."

            Return rval
        End If


        '++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        '+++ DEBUG ONLY +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        '++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        'Dim dbg = xModello.ElaboraIndicatore(helper)

        'Dim piogge = helper.ElencoPiogge
        '++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        '++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        '++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        Return xModello.ElaboraModello()
    End Function

    Public Function ModelliEsponiParams(ByVal veg_cod As Integer, ByVal mod_cod As Integer, ByVal params As String) As String

        Dim str_ppp As String = ""

        Select Case mod_cod

            Case enum_ModelliPrevisionali.Racca_BrusoneRiso

                str_ppp = Racca.EsponiParams(mod_cod, params)

            Case enum_ModelliPrevisionali.Agronomica30_MISP_IPI_Pomodoro

                str_ppp = Agronomica30_Pom_MISP_IPI.EsponiParams(params)

            Case enum_ModelliPrevisionali.UniCatt_Mais_AFLA,
                 enum_ModelliPrevisionali.UniCatt_Mais_FER

                str_ppp = UniCatt_Mais.EsponiParams(params)

            Case enum_ModelliPrevisionali.UniCatt_Frumento_Fusariosi

                str_ppp = UniCatt_Frumento.EsponiParams(params)

            Case enum_ModelliPrevisionali.BetaCoProB_Cercosporiosi

                str_ppp = BetaCoProB_Cercosporiosi.EsponiParams(params)

            Case enum_ModelliPrevisionali.RaccaFrumento_RuggineBruna,
                 enum_ModelliPrevisionali.RaccaFrumento_RuggineGialla,
                 enum_ModelliPrevisionali.RaccaFrumento_RuggineNera,
                 enum_ModelliPrevisionali.RaccaFrumento_Stagonosporiosi,
                 enum_ModelliPrevisionali.RaccaFrumento_Septoria,
                 enum_ModelliPrevisionali.RaccaFrumento_Fusariosi,
                 enum_ModelliPrevisionali.RaccaFrumento_FusariosiSpiga,
                 enum_ModelliPrevisionali.RaccaFrumento_Fusariosi2,
                 enum_ModelliPrevisionali.RaccaFrumento_Fusariosi3,
                 enum_ModelliPrevisionali.RaccaFrumento_MarciumeRosa

                str_ppp = RaccaFrumento.EsponiParams(params)

        End Select

        Return str_ppp
    End Function


    Public Function ElaboreIndicatoreDaParametri(pcm As ParametriCalcoloModello, objParametri As AgronicaCoreParametri) As rispostaStandard(Of RisultatoElaborazioneIndicatori.Indicatore.RisultatoElaborazione)

        Dim rval As New rispostaStandard(Of RisultatoElaborazioneIndicatori.Indicatore.RisultatoElaborazione)

        'per enum_ModelliPrevisionali.Ritardo_Variabile
        ' VAnni: 3/4/2017: qualsiasi periodo imposto la partenza sui inizio anno.
        'DataInizio = DateSerial(Year(DataInizio), 1, 1)

        Dim helper = New MeteoDSS_Helper(objParametri)

        If Not helper.Leggi(pcm.Meteo) Then
            rval.RispostaOK = False
            rval.Errore = helper.StatusMsg
            Return rval
        End If

        If Not VerificaMeteoXModello(helper, pcm.Modello.Mod_Cod, rval.Errore) Then
            rval.RispostaOK = False
            Return rval
        End If

        Dim xModello As AbstractModello = _creaModello(helper, pcm.Modello, False, objParametri)

        If xModello Is Nothing Then

            rval.RispostaOK = False
            rval.Errore = "Algoritmo non previsto."

            Return rval
        End If

        rval.RispostaStringa = xModello.ElaboraIndicatore(helper)

        Return rval
    End Function

    Public Function ElaboreIndicatore(pcm As ParametriCalcoloModello, objParametri As AgronicaCoreParametri) As RisultatoElaborazioneIndicatori.Indicatore.RisultatoElaborazione

        Dim helper = New MeteoDSS_Helper(objParametri)

        If Not helper.Leggi(pcm.Meteo) Then
            Return Nothing
        End If

        Dim dummy As String = ""
        If Not VerificaMeteoXModello(helper, pcm.Modello.Mod_Cod, dummy) Then
            Return Nothing
        End If

        Dim xModello As AbstractModello = _creaModello(helper, pcm.Modello, False, objParametri)

        If xModello Is Nothing Then
            Return Nothing
        End If

        Return xModello.ElaboraIndicatore(helper)
    End Function


    Protected Function VerificaMeteoXModello(helper As MeteoDSS_Helper, mod_cod As Integer, ByRef msg As String) As Boolean

        If helper.DatiMeteo Is Nothing Then

            Return False
        End If

        If helper.DatiMeteo.SensoreBagnatura Then

            Return True
        End If

        Dim UnneededBagn As Integer() = {
            enum_ModelliPrevisionali.Agronomica30_MISP_IPI_Pomodoro,
            enum_ModelliPrevisionali.Ritardo_Variabile,
            enum_ModelliPrevisionali.Agronomica30_MRV_Eulia,
            enum_ModelliPrevisionali.Agronomica30_MRV_CydiaMolesta,
            enum_ModelliPrevisionali.Agronomica30_MRV_Helicoverpa,
            enum_ModelliPrevisionali.Agronomica30_ColpoDiFuoco,
            enum_ModelliPrevisionali.BetaCoProB_Cercosporiosi
        }
        'enum_ModelliPrevisionali.Agronomica30_MRV_Tignoletta,
        'enum_ModelliPrevisionali.Agronomica30_MRV_Carpocapsa,
        'enum_ModelliPrevisionali.MRV_PiralideMais
        'enum_ModelliPrevisionali.MRV_NottuaMais


        If UnneededBagn.Contains(mod_cod) Then

            Return True
        End If

        Dim OptionalBagn As Integer() = {
            enum_ModelliPrevisionali.Agronomica30_BatteriosiKiwi_PSA,
            enum_ModelliPrevisionali.Agronomica30_Peronospora,
            enum_ModelliPrevisionali.Agronomica30_BotriteVite,
            enum_ModelliPrevisionali.Agronomica30_OidioVite,
            enum_ModelliPrevisionali.Agronomica30_MaculaturaPero,
            enum_ModelliPrevisionali.Agronomica30_TicchiolaturaMelo
        }

        If OptionalBagn.Contains(mod_cod) Then

            Return True
        End If

        msg = My.Resources.AgronicaCoreModelliPrevisionaliBIZ.ModelliPrevisionaliCalcolatore_mancanzaMisurePerDatiMeteo

        Return False
    End Function

End Class



Public Class ModelliPrevisionaliCalcolatore_GSB
    Inherits ModelliPrevisionaliCalcolatore

    Private ReadOnly _tipoSincro As enum_Tipi_Servizi_Background
    Private _objParametri As AgronicaCoreParametri
    Private ReadOnly _cartellaPolling As String
    Private ReadOnly _cartellaLog As String
    Private _msgRitorno As StringBuilder

    Public ReadOnly Property MessaggioRitorno As String
        Get
            Return _msgRitorno.ToString()
        End Get
    End Property

    Public Sub New(cfg_srv As Configurazione_Servizio)
        MyBase.New()

        _tipoSincro = cfg_srv.Tipo_Sincro

        Dim obV As JObject = JsonConvert.DeserializeObject(cfg_srv.Parametri_Extra)

        _cartellaPolling = obV("CartellaPolling")
        _cartellaLog = obV("CartellaLog")

        _objParametri = New AgronicaCoreParametri With {
            .StringaConnessione = obV("StringaConnesioneMeteoSuite")
        }

        _msgRitorno = New StringBuilder
    End Sub

    Public Function Avvia_Importazione_Sincronizzazione_Modelli() As Boolean

        Try

            'Dim nomeFile, elencoFiles() As String

            ''esiste un solo file che attiva il tutto..
            'elencoFiles = System.IO.Directory.GetFiles(_cartellaPolling, "*.json")

            'For Each nomeFile In elencoFiles

            '    _spostaFile(nomeFile)

            'Next

            '_msgRitorno.AppendLine()

            '_creaFileSystemWatcher()

            '_msgRitorno.AppendLine()

            'eseguo il GSB da tabella... 
            _elaboraIndicatoriGSB()

        Catch ex As Exception

            _msgRitorno.Append(AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True))

            Return False
        End Try

        Return True
    End Function

    Private Sub _creaFileSystemWatcher()

        Dim creaWatcher As Boolean = True

        Dim pid = Process.GetCurrentProcess().Id

        'Cerco nella cartelle polling il file sentinella...

        Dim filename As String = _cartellaPolling
        If Right(filename, 1) <> "\" Then filename &= "\"
        filename &= "Process.Watcher"

        Try

            Dim reader = File.OpenText(filename)
            Dim file_pid As Integer = 0

            If Integer.TryParse(reader.ReadToEnd(), file_pid) Then

                If file_pid = pid Then
                    creaWatcher = False
                End If

            End If

            reader.Close()

        Catch ex As Exception

            'Il file non esiste???

        End Try

        If Not creaWatcher Then

            _msgRitorno.AppendLine("Attesa di nuovi modelli con cartella di polling " & _cartellaPolling & " già attiva per il processo...")

            Return

        End If

        Try

            Dim stream = File.CreateText(filename)
            stream.WriteLine(pid.ToString.ToCharArray())
            stream.Close()

        Catch ex As Exception

            _msgRitorno.AppendLine("ERRORE: Creazione file sentinella su cartella polling " & _cartellaPolling)

        End Try

        'Imposto il file system watcher...

        Dim fsw As FileSystemWatcher

        Try
            fsw = New FileSystemWatcher
            fsw.Path = _cartellaPolling
            fsw.Filter = "*.json"
            fsw.NotifyFilter = (NotifyFilters.LastAccess Or NotifyFilters.LastWrite Or NotifyFilters.FileName Or NotifyFilters.DirectoryName)

            'Add event handlers...
            'AddHandler fsw.Changed, AddressOf OnChanged
            AddHandler fsw.Created, AddressOf _elaboraIndicatoriGSB_Fs
            'AddHandler fsw.Deleted, AddressOf OnChanged
            'AddHandler fsw.Renamed, AddressOf _D_Changed

            'Begin watching.
            fsw.EnableRaisingEvents = True

            _msgRitorno.AppendLine("Attesa attiva di nuovi modelli con cartella di polling: " & _cartellaPolling)

        Catch ex As Exception

            _msgRitorno.AppendLine("ERRORE: Impossibile gestire Attesa Attiva di nuovi modelli con cartella di polling:  " & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True))

        End Try

    End Sub

    Private Sub _elaboraIndicatoriGSB_Fs(sender As Object, e As FileSystemEventArgs)

        _spostaFile(e.FullPath)

        _elaboraIndicatoriGSB()
    End Sub

    Private Sub _spostaFile(ByVal SourceFile As String)

        If Not File.Exists(SourceFile) Then Throw New Exception("Il file " & SourceFile & " non esiste.")

        Try

            Dim srcFilename = System.IO.Path.GetFileName(SourceFile)
            Dim filePath = Path.Combine(_cartellaLog, srcFilename & "_" & Now().ToString("yyyyMMddhhmmss"))

            If Not Directory.Exists(filePath) Then
                Directory.CreateDirectory(filePath)
            End If

            Dim TargetFile = filePath & "\" & srcFilename
            Dim Counter As Integer = 0
            Dim Suffisso As String = ""
            Dim spostato As Boolean = False

            While Not spostato

                Try

                    File.Move(SourceFile, TargetFile & Suffisso)

                    spostato = True

                Catch ex As Exception

                    Suffisso = "." & Counter.ToString("X8")
                    Counter += 1

                    spostato = False

                End Try

            End While

        Catch ex As Exception

        End Try
    End Sub

    Private Function _elaboraIndicatoriGSB() As Boolean

        If _tipoSincro = enum_Tipi_Servizi_Background.DSS_ModelliPrevisionali_Elaborazione Then
            Return _elaboraIndicatori_Vecchio()
        End If

        Dim res1 = _elaboraIndicatori()
        Dim res2 = ElaboraIndicatoriRichiesti()

        If Not res2 Then

            Dim objDAL As New DSS_Modelli_Richieste_GSB

            objDAL.EliminaRichiesti(Date.Now.Date.AddDays(-7), _objParametri)
        End If

        Return res1 OrElse res2
    End Function





    Private Function _elaboraIndicatori_Vecchio() As Boolean

        'legge ed elabora tutti i modelli ancora da elaborare, depositando i risultati in tabella.
        Dim letturaModelli As New AgronicaCoreMeteoDAL.DSS_Agronica_Stazioni_Meteo_ModelliPrevisionali_SRV_R
        Dim scritturaModelli As New AgronicaCoreMeteoDAL.DSS_Agronica_Stazioni_Meteo_ModelliPrevisionali_SRV_W

        Dim dtModelli As DataTable = letturaModelli.LeggiModelliDaElaborare("", "", _objParametri)

        Dim ListaModelliParametri As List(Of ParametriCalcoloModello) = (
        From dtt In dtModelli.AsEnumerable
        Select New ParametriCalcoloModello With {
            .Meteo = New ParametriMeteoDSS With {
            .Sorgente = CInt(dtt("Tipo_Sorgente")),
            .Stazione = CInt(dtt("Stazione_Cod"))
            },
            .Modello = New ParametriModello With {
            .Mod_Cod = CInt(dtt("Mod_Cod")),
            .Veg_Cod = CInt(dtt("Veg_Cod")),
            .Avv_Cod = CInt(dtt("Av_Cod")),
            .Alg_Cod = CInt(dtt("Algoritmo_Cod")),
            .Params = CStr(dtt("ParametriElaborazione"))
            }
            }
            ).ToList()

        Dim defDataA As DateTime = Now.Date()
        Dim defDataDa As DateTime = CDate("01/01/" & defDataA.Year)
        Dim dataDa As DateTime
        Dim dataA As DateTime

        _msgRitorno.AppendLine("Elaborazione eseguita su " & ListaModelliParametri.Count & " elementi.")
        _msgRitorno.AppendLine("Note:")

        Dim rvalModelloCorrente As rispostaStandard(Of cRisultatoModelloIndicatori.Indicatore)
        For Each pcm As ParametriCalcoloModello In ListaModelliParametri

            dataA = defDataA
            dataDa = defDataDa

            If Not String.IsNullOrEmpty(pcm.Modello.Params) Then

                Dim objParam = JObject.Parse(pcm.Modello.Params)

                If objParam("dataInizio") IsNot Nothing Then
                    dataDa = CDate(objParam("dataInizio")).ToLocalTime()
                End If
                If objParam("dataFine") IsNot Nothing Then
                    dataA = CDate(objParam("dataFine")).ToLocalTime()
                End If

            End If

            'singola elaborazione
            pcm.Meteo.DataInizio = dataDa
            pcm.Meteo.DataFine = dataA

            rvalModelloCorrente = _indicatore_vecchio(pcm)

            'serializzo qualsiasi risultato, così avrò feedback su errori in fase di consultazione web ..?
            Dim risSerializzato As String = JsonConvert.SerializeObject(rvalModelloCorrente.RispostaStringa)

            If Not rvalModelloCorrente.RispostaOK Then
                _msgRitorno.AppendLine(">>> Errore <<<")
                _msgRitorno.AppendLine("Tipo Sorgente: " & pcm.Meteo.Sorgente & " - Stazione meteo: " & pcm.Meteo.Stazione & " - Mod_Cod: " & pcm.Modello.Mod_Cod & " - Veg_Cod: " & pcm.Modello.Veg_Cod & " - Av_Cod: " & pcm.Modello.Avv_Cod)
                _msgRitorno.AppendLine(rvalModelloCorrente.Errore)
                _msgRitorno.AppendLine()
            End If

            scritturaModelli.ImpostaRisultatoSuModello(
                pcm.Meteo.Sorgente,
                pcm.Meteo.Stazione,
                pcm.Modello.Mod_Cod,
                pcm.Modello.Alg_Cod,
                pcm.Modello.Params,
                risSerializzato,
                _objParametri)

        Next

        'in ogni caso restituisco true
        Return True
    End Function

    Private Function _indicatore_vecchio(pcm As ParametriCalcoloModello) As rispostaStandard(Of cRisultatoModelloIndicatori.Indicatore)

        Dim rval As New rispostaStandard(Of cRisultatoModelloIndicatori.Indicatore) With {
            .RispostaStringa = New cRisultatoModelloIndicatori.Indicatore
        }

        Dim helper = New MeteoDSS_Helper(_objParametri)

        If Not helper.Leggi(pcm.Meteo) Then

            rval.RispostaOK = False
            rval.Errore = helper.StatusMsg

            rval.RispostaStringa.Status = cRisultatoModelloIndicatori.Indicatore.enum_Status.Status_Error
            rval.RispostaStringa.StatusMsg = rval.Errore

            Return rval
        End If

        If Not VerificaMeteoXModello(helper, pcm.Modello.Mod_Cod, rval.Errore) Then

            rval.RispostaOK = False

            rval.RispostaStringa.Status = cRisultatoModelloIndicatori.Indicatore.enum_Status.Status_Error
            rval.RispostaStringa.StatusMsg = rval.Errore

            Return rval
        End If

        Dim xModello As AbstractModello = _creaModello(helper, pcm.Modello, True, _objParametri)

        If xModello Is Nothing Then

            rval.RispostaOK = False
            rval.Errore = "Indicatore DSS non previsto per il modello"
            rval.RispostaStringa.Status = cRisultatoModelloIndicatori.Indicatore.enum_Status.Status_Error
            rval.RispostaStringa.StatusMsg = rval.Errore

            Return rval
        End If

        Return xModello.ElaboraIndicatore_Vecchio(pcm.Meteo.DataInizio, pcm.Meteo.DataFine)
    End Function



    Private Function _elaboraIndicatori() As Boolean

        Dim defDataA As DateTime = Now.Date()
        Dim defDataDa As DateTime = CDate("01/01/" & defDataA.Year)
        Dim dataDa As DateTime
        Dim dataA As DateTime
        Dim forecast As Boolean = False

        'legge ed elabora tutti i modelli ancora da elaborare, depositando i risultati in tabella.
        Dim objDAL As New AgronicaCoreMeteoDAL.DSS_ModelliPrevisionali_Elaborazione_GSB

        Dim dtModelli As DataTable = objDAL.LeggiModelliDaElaborare(_objParametri)

        _msgRitorno.AppendLine("Elaborazione eseguita su " & dtModelli.Rows.Count & " elementi.")
        _msgRitorno.AppendLine("Note:")

        Dim dictMeteo As New Dictionary(Of ParametriMeteoDSS, List(Of ParametriModello))

        For Each row In dtModelli.Rows

            dataDa = defDataDa
            dataA = defDataA

            Dim params As String = CStr(row("ParametriElaborazione"))

            If Not String.IsNullOrEmpty(params) Then

                Dim objParam = JObject.Parse(params)

                If objParam("dataInizio") IsNot Nothing Then
                    dataDa = CDate(objParam("dataInizio")).ToLocalTime()
                End If
                If objParam("dataFine") IsNot Nothing Then
                    dataA = CDate(objParam("dataFine")).ToLocalTime()
                End If

                Dim objForecast = objParam("forecast")
                If objForecast IsNot Nothing AndAlso objForecast.Type = JTokenType.Boolean Then
                    forecast = CBool(objForecast)
                End If
            End If

            Dim par_meteo As New ParametriMeteoDSS With {
                .Sorgente = CInt(row("Tipo_Sorgente")),
                .Stazione = CInt(row("Stazione_Cod")),
                .DataInizio = dataDa,
                .DataFine = dataA,
                .Forecast = forecast
            }
            Dim par_mod As New ParametriModello With {
                .Mod_Cod = CInt(row("Mod_Cod")),
                .Veg_Cod = CInt(row("Veg_Cod")),
                .Avv_Cod = CInt(row("Avv_Cod")),
                .Alg_Cod = CInt(row("Alg_Cod")),
                .Params = params,
                .ParamsOrig = params
            }



            If RaccaFrumento.Contains(par_mod.Mod_Cod) Then

                Dim pcm As New ParametriCalcoloModello With {
                    .Meteo = par_meteo,
                    .Modello = par_mod
                }

                RaccaFrumento.AggiustaParametri(pcm)
            End If



            Dim list As New List(Of ParametriModello)

            If Not dictMeteo.TryGetValue(par_meteo, list) Then

                list = New List(Of ParametriModello)

                dictMeteo.Add(par_meteo, list)
            End If

            list.Add(par_mod)
        Next

        For Each kvp In dictMeteo

            Dim par_meteo As ParametriMeteoDSS = kvp.Key
            Dim helper = New MeteoDSS_Helper(_objParametri)

            Dim datiLetti As Boolean = helper.Leggi(par_meteo)

            For Each par_mod In kvp.Value

                Dim risElab As New RisultatoElaborazioneIndicatori.Indicatore.RisultatoElaborazione

                If Not datiLetti Then

                    risElab.Errore(helper.StatusMsg)
                Else

                    Dim msg As String = ""
                    If Not VerificaMeteoXModello(helper, par_mod.Mod_Cod, msg) Then

                        risElab.Errore(msg)
                    Else

                        Dim xModello As AbstractModello = _creaModello(helper, par_mod, True, _objParametri)

                        If xModello Is Nothing Then

                            risElab.Errore("Indicatore DSS non previsto per il modello")
                        Else

                            risElab = xModello.ElaboraIndicatore(helper)
                        End If
                    End If
                End If

                'serializzo qualsiasi risultato, così avrò feedback su errori in fase di consultazione web ..?
                Dim risSerializzato As String = JsonConvert.SerializeObject(risElab)

                If risElab.Status = RisultatoElaborazioneIndicatori.Indicatore.RisultatoElaborazione.Stato_Elaborazione._Error Then
                    _msgRitorno.AppendLine(">>> Errore <<<")
                    _msgRitorno.AppendLine("Tipo Sorgente: " & par_meteo.Sorgente &
                                           " - Stazione meteo: " & par_meteo.Stazione &
                                           " - Mod_Cod: " & par_mod.Mod_Cod &
                                           " - Veg_Cod: " & par_mod.Veg_Cod &
                                           " - Av_Cod: " & par_mod.Avv_Cod)
                    _msgRitorno.AppendLine(risElab.StatusMsg)
                    _msgRitorno.AppendLine()
                End If

                Dim chiave As New DSS_ModelliPrevisionali_Elaborazione_GSB.ChiaveElaborazione With {
                    .Tipo_Sorgente = par_meteo.Sorgente,
                    .Stazione_Cod = par_meteo.Stazione,
                    .Mod_Cod = par_mod.Mod_Cod,
                    .Veg_Cod = par_mod.Veg_Cod,
                    .Avv_Cod = par_mod.Avv_Cod,
                    .Alg_Cod = par_mod.Alg_Cod,
                    .ParametriElaborazione = par_mod.ParamsOrig
                }

                objDAL.ScriviRisultatoModello(chiave, risSerializzato, _objParametri)
            Next
        Next

        'in ogni caso restituisco true
        Return True
    End Function



    Private Class ModelloRichiesto
        Public Chiave As DSS_Modelli_Richieste_GSB.ChiaveTabella
        Public Parametri As ParametriModello
    End Class

    Private Function ElaboraIndicatoriRichiesti() As Boolean

        'legge ed elabora tutti i modelli ancora da elaborare, depositando i risultati in tabella.
        Dim objDAL As New DSS_Modelli_Richieste_GSB

        Dim listaChiavi = objDAL.LeggiRichiesti(_objParametri)

        If listaChiavi.Count = 0 Then
            Return False
        End If

        Dim dictMeteo As New Dictionary(Of ParametriMeteoDSS, List(Of ModelloRichiesto))

        For Each chiave In listaChiavi

            Dim par_meteo As New ParametriMeteoDSS With {
                .Sorgente = chiave.Meteo_Sorgente,
                .Stazione = chiave.Meteo_Stazione,
                .DataInizio = chiave.Meteo_DataInizio,
                .DataFine = chiave.Meteo_DataFine.AddDays(chiave.Meteo_Forecast),
                .Forecast = chiave.Meteo_Forecast > 0
            }

            Dim mod_rich As New ModelloRichiesto With {
                .Chiave = chiave,
                .Parametri = New ParametriModello With {
                    .Mod_Cod = chiave.Mod_Cod,
                    .Veg_Cod = chiave.Veg_Cod,
                    .Avv_Cod = chiave.Avv_Cod,
                    .Alg_Cod = chiave.Alg_Cod,
                    .Params = chiave.ParametriModello
                }
            }

            If RaccaFrumento.Contains(mod_rich.Parametri.Mod_Cod) Then

                Dim pcm As New ParametriCalcoloModello With {
                    .Meteo = par_meteo,
                    .Modello = mod_rich.Parametri
                }

                RaccaFrumento.AggiustaParametri(pcm)
            End If


            Dim list As New List(Of ModelloRichiesto)

            If Not dictMeteo.TryGetValue(par_meteo, list) Then

                list = New List(Of ModelloRichiesto)

                dictMeteo.Add(par_meteo, list)
            End If

            list.Add(mod_rich)
        Next


        For Each kvp In dictMeteo

            Dim par_meteo As ParametriMeteoDSS = kvp.Key
            Dim helper = New MeteoDSS_Helper(_objParametri)

            Dim datiLetti As Boolean = helper.Leggi(par_meteo)

            For Each mod_rich In kvp.Value

                Dim risElab As New RisultatoElaborazioneIndicatori.Indicatore.RisultatoElaborazione

                If Not datiLetti Then

                    risElab.Errore(helper.StatusMsg)
                Else

                    Dim msg As String = ""
                    If Not VerificaMeteoXModello(helper, mod_rich.Parametri.Mod_Cod, msg) Then

                        risElab.Errore(msg)
                    Else

                        Dim xModello As AbstractModello = _creaModello(helper, mod_rich.Parametri, True, _objParametri)

                        If xModello Is Nothing Then

                            risElab.Errore("Indicatore DSS non previsto per il modello")
                        Else

                            risElab = xModello.ElaboraIndicatore(helper)

                            If risElab.Status <> RisultatoElaborazioneIndicatori.Indicatore.RisultatoElaborazione.Stato_Elaborazione._Error AndAlso
                                risElab.Status <> RisultatoElaborazioneIndicatori.Indicatore.RisultatoElaborazione.Stato_Elaborazione._OutOfRange Then

                                If helper.ElencoPiogge.Any() Then

                                    Dim lastDT = helper.DatiMeteo.Last().DataOra.Date
                                    lastDT = lastDT.AddMonths(-1)

                                    For Each ip In helper.ElencoPiogge

                                        If ip.InizioEvento < lastDT Then
                                            Exit For
                                        End If

                                        risElab.EventiPioggia.Add(
                                            New RisultatoElaborazioneIndicatori.Indicatore.RisultatoElaborazione.EventoPioggia() With
                                            {
                                            .InizioEvento = ip.InizioEvento.ToString("yyyy-MM-ddTHH:mm:ss"),
                                            .Cumulato_mm = ip.Cumulato_mm,
                                            .Durata_hh = ip.Durata_hh
                                            })
                                    Next
                                End If
                            End If
                        End If
                    End If
                End If

                'serializzo qualsiasi risultato, così avrò feedback su errori in fase di consultazione web ..?
                Dim risSerializzato As String = JsonConvert.SerializeObject(risElab)

                If risElab.Status = RisultatoElaborazioneIndicatori.Indicatore.RisultatoElaborazione.Stato_Elaborazione._Error Then
                    _msgRitorno.AppendLine(">>> Errore <<<")
                    _msgRitorno.AppendLine("Tipo Sorgente: " & par_meteo.Sorgente &
                                           " - Stazione meteo: " & par_meteo.Stazione &
                                           " - Mod_Cod: " & mod_rich.Parametri.Mod_Cod &
                                           " - Veg_Cod: " & mod_rich.Parametri.Veg_Cod &
                                           " - Av_Cod: " & mod_rich.Parametri.Avv_Cod)
                    _msgRitorno.AppendLine(risElab.StatusMsg)
                    _msgRitorno.AppendLine()
                End If

                objDAL.ScriviRisultato(mod_rich.Chiave, risSerializzato, _objParametri)
            Next
        Next

        'in ogni caso restituisco true
        Return True
    End Function

End Class