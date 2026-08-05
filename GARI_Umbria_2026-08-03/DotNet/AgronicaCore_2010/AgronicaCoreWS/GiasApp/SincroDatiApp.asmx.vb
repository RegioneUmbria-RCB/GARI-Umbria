Imports System.ComponentModel
Imports System.Globalization
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.Windows.Interop
Imports System.Xml
Imports AgronicaCoreAnagrafeBIZ
Imports AgronicaCoreContabBIZ
Imports AgronicaCoreContabDAL
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.My.Resources
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDemetraBIZ
Imports AgronicaCoreDTOStd.InData.Anagrafica
Imports AgronicaCoreDTOStd.InData.Demetra
Imports AgronicaCoreDTOStd.InData.GiasApp
Imports AgronicaCoreModello
Imports AgronicaCoreModello.AppHelper
Imports AgronicaCoreModello.OperazioneAgenda_Temp
Imports AgronicaCoreModelsSTD
Imports AgronicaCoreModelsSTD.anagrafiche
Imports AgronicaCoreModelsSTD.attivita
Imports AgronicaCoreModelsSTD.attivita.risorse
Imports AgronicaCoreModelsSTD.exceptions
Imports AgronicaCoreScadenziario_BIZ
Imports AgronicaCoreUtentiDAL
Imports AgronicaCoreUtility
Imports AgronicaCoreVarieBIZ
Imports InData.Agenda
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
'Imports AgronicaCoreDTOStd.InData.importazioni
'Imports AgronicaCoreModelsSTD.metaschema
'Imports ClosedXML.Excel.XLPredefinedFormat
'Imports AgronicaCoreContabBIZ
'Imports AgronicaCoreEntityFramework_POCO
'Imports AgronicaCoreModelsSTD.documenti
'Imports AgronicaCoreModelsSTD.Zoo

' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
' <System.Web.Script.Services.ScriptService()>
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<System.Web.Script.Services.ScriptService()>
Public Class SincroDatiApp
    Inherits System.Web.Services.WebService


    <WebMethod()>
    Public Function LeggiImprese(ByVal tipo As String, ByVal objP_super_server As String, ByVal objP_server As String, ByVal objP_utenti As String) As rispostaStandard(Of List(Of Impresa))

        Dim r As New rispostaStandard(Of List(Of Impresa))

        Try

            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)

            'Dim objUtentiVisibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
            'Dim dtImpreseVisibili = objUtentiVisibilita.Leggi(enum_TipoEntita.Impresa, "", "", objParametri_Server)
            'Dim impreseVisibili = dtImpreseVisibili IsNot Nothing AndAlso dtImpreseVisibili.Rows.Count > 0

            Dim objUtentiProfili As New AgronicaCoreUtentiDAL.Utenti_Profili_Read
            Dim dtProfili As DataTable = objUtentiProfili.Leggi(objParametri_Utenti.UtenteUsername, 5, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_Utenti)
            Dim impreseVisibili = dtProfili.Rows.Count > 0 AndAlso Not String.IsNullOrEmpty(dtProfili.Rows(0)("Descrizione_1"))

            Dim objUtentiPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
            Dim gestioneAziende = objUtentiPermessi.Controlla_Permessi_Utente(objParametri_Utenti.UtenteUsername, enum_Id_Servizio.GiasOnline, enum_Security_Attivita.GiasAPP_Aziende, enum_Security_Operazione.Lettura, Date.Now, "", objParametri_Utenti)
            Dim utenteSuperuser = objParametri_Utenti.UtenteUsername = objParametri_Utenti.SuperUserUsername

            ' blocco per evitare di vedere tutte le imprese su app se non superuser
            If gestioneAziende AndAlso Not impreseVisibili AndAlso Not utenteSuperuser Then
                r.RispostaStringa = New List(Of Impresa)
                r.RispostaOK = True
                Return r
            End If

            Dim objImprese As New Impresa_R
            Dim imprese = objImprese.Leggi_Imprese_APP(tipo, objParametri_Server)

            r.RispostaStringa = imprese
            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r

    End Function


    <WebMethod()>
    Public Function LeggiCentri(ByVal piva As String, ByVal Data As String, ByVal objP_super_server As String, ByVal objP_server As String, ByVal objP_utenti As String) As rispostaStandard(Of List(Of AgronicaCoreModelsSTD.anagrafiche.CentroAziendale))

        Dim r As New rispostaStandard(Of List(Of AgronicaCoreModelsSTD.anagrafiche.CentroAziendale))

        Try

            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)

            Dim dataRif As Date = Today
            If Date.TryParseExact(Data, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, Nothing) Then
                dataRif = Date.ParseExact(Data, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal)
            End If

            Dim objAppHelper As New AppHelper
            Dim impostazioni = objAppHelper.Leggi_Impostazioni_APP(objParametri_Utenti)
            If impostazioni IsNot Nothing AndAlso impostazioni.SincroAnni > 0 Then
                dataRif = New Date(If(Today.Month < 11, Today.Year - 1, Today.Year) - impostazioni.SincroAnni, 11, 1)
            End If

            Dim objCentri As New AgronicaCoreAnagrafeBIZ.CentroAziendale_R
            Dim centriAziendali = objCentri.Leggi_Centri_APP(piva, dataRif, objParametri_Server, False)

            r.RispostaStringa = centriAziendali
            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r

    End Function

    <WebMethod()>
    Public Function LeggiCampi(ByVal piva As String, ByVal Data As String, ByVal objP_super_server As String, ByVal objP_server As String, ByVal objP_utenti As String) As rispostaStandard(Of List(Of AgronicaCoreModelsSTD.anagrafiche.Campo))

        Dim r As New rispostaStandard(Of List(Of AgronicaCoreModelsSTD.anagrafiche.Campo))

        Try

            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)

            Dim dataRif As Date = Today
            If Date.TryParseExact(Data, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, Nothing) Then
                dataRif = Date.ParseExact(Data, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal)
            End If

            Dim objAppHelper As New AppHelper
            Dim impostazioni = objAppHelper.Leggi_Impostazioni_APP(objParametri_Utenti)
            If impostazioni IsNot Nothing AndAlso impostazioni.SincroAnni > 0 Then
                dataRif = New Date(If(Today.Month < 11, Today.Year - 1, Today.Year) - impostazioni.SincroAnni, 11, 1)
            End If

            Dim objCampi As New AgronicaCoreAnagrafeBIZ.Campo_R
            Dim campi = objCampi.Leggi_Campi_APP(piva, dataRif, objParametri_Server, False)

            r.RispostaStringa = campi
            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r

    End Function

    <WebMethod()>
    Public Function LeggiAppezzamenti(ByVal piva As String, ByVal Data As String, ByVal objP_super_server As String, ByVal objP_server As String, ByVal objP_utenti As String) As rispostaStandard(Of List(Of AgronicaCoreModelsSTD.anagrafiche.Appezzamento))

        Dim r As New rispostaStandard(Of List(Of AgronicaCoreModelsSTD.anagrafiche.Appezzamento))

        Try

            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)

            Dim dataRif As Date = Today
            If Date.TryParseExact(Data, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, Nothing) Then
                dataRif = Date.ParseExact(Data, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal)
            End If

            Dim objAppHelper As New AppHelper
            Dim impostazioni = objAppHelper.Leggi_Impostazioni_APP(objParametri_Utenti)
            If impostazioni IsNot Nothing AndAlso impostazioni.SincroAnni > 0 Then
                dataRif = New Date(If(Today.Month < 11, Today.Year - 1, Today.Year) - impostazioni.SincroAnni, 11, 1)
            End If

            Dim objAppezzamenti As New AgronicaCoreAnagrafeBIZ.Appezzamento_R
            Dim appezzamenti = objAppezzamenti.Leggi_Appezzamenti_APP(piva, dataRif, objParametri_Server, objParametri_Utenti)

            r.RispostaStringa = appezzamenti
            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r

    End Function


    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiVisite(ByVal piva As String, ByVal objP_super_server As String, ByVal objP_server As String, ByVal objP_utenti As String) As rispostaStandard(Of List(Of AgronicaCoreModelsSTD.attivita.Attivita))

        Dim r As New rispostaStandard(Of List(Of AgronicaCoreModelsSTD.attivita.Attivita))
        Dim NomeRoutine As String = "SincroDatiApp.LeggiVisite()"
        Dim MessaggioErrore As String = ""

        Try

            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)

            Dim objDP As New DataProvider
            Dim objAgenda As New Agenda_Operazione_Helper
            Dim map As New AgronicaCoreMapper.AgendaToAttivita
            Dim objVisiteBIZ As New AgronicaCoreVisiteBIZ.Visite_R
            Dim visite = New List(Of AgronicaCoreModelsSTD.attivita.Attivita)
            Dim username = objParametri_Server.UtenteUsername

            Dim objAppHelper As New AppHelper
            Dim impostazioni = objAppHelper.Leggi_Impostazioni_APP(objParametri_Utenti)
            Dim rilievi As Boolean = impostazioni IsNot Nothing AndAlso impostazioni.VisiteRilievi
            Dim dtVisite As DataTable = objVisiteBIZ.Leggi_Visite_APP(piva, username, objParametri_Server, objParametri_Utenti, rilievi)

            For Each row In dtVisite.Rows
                Try
                    Dim agenda As OperazioneAgenda_Temp.Operazione_Agenda = objAgenda.Leggi(row("Piva"), 0, row("Id_Agenda"), 0, objParametri_Server)
                    If agenda IsNot Nothing Then
                        visite.Add(map.AgendaSuAttivita(agenda, verbose:=False, objParametri_Super_Server, objParametri_Server, objParametri_Utenti))
                    End If
                Catch ex As Exception
                    MessaggioErrore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
                    objDP.Scrivi_LOG(objParametri_Server, NomeRoutine, MessaggioErrore)
                End Try
            Next

            r.RispostaOK = True
            r.RispostaStringa = visite

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r

    End Function

    <WebMethod()>
    Public Function CreaProdotti(ByVal tipo As String, ByVal piva As String, ByVal objP_super_server As String, ByVal objP_server As String, ByVal objP_utenti As String) As RispostaStandard

        Dim r As New RispostaStandard

        Try

            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)

            Dim Log_Import As New StringBuilder
            Dim Log_Errori As New StringBuilder
            Dim Log_Riepilogo As New StringBuilder
            Dim objImportaGias As New Importa_GIAS

            Dim Flag_Crea_Trasformati As Boolean = False
            Dim Flag_Crea_Semilavorati As Boolean = False
            Dim Flag_Crea_Sementi As Boolean = False

            If tipo = "0" Then
                Flag_Crea_Trasformati = True
                Flag_Crea_Semilavorati = True
                Flag_Crea_Sementi = True
            Else
                Dim categorie = tipo.Split(",")
                Flag_Crea_Trasformati = categorie.Contains(CostantiPersonalizzate.TRASFORMATI_VEGETALI)
                Flag_Crea_Semilavorati = categorie.Contains(CostantiPersonalizzate.SEMILAVORATI_VEGETALI)
                Flag_Crea_Sementi = categorie.Contains(CostantiPersonalizzate.SEMENTI)
            End If

            objImportaGias.Crea_MateriePrimeVegetali(
                Log_Import, Log_Errori, Log_Riepilogo, objParametri_Server, objParametri_Utenti,
                piva, 1, 1, 0, 0, Flag_Crea_Trasformati, Flag_Crea_Semilavorati, Flag_Crea_Sementi)

            r.RispostaOK = True
            r.RispostaStringa = Log_Riepilogo.ToString

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r

    End Function

    <WebMethod()>
    Public Function LeggiDatiApp(ByVal tipo As String, ByVal piva As String, ByVal data As String, ByVal objP_super_server As String, ByVal objP_server As String, ByVal objP_utenti As String) As RispostaStandard

        Dim r As New RispostaStandard

        Try

            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)

            Dim objAppHelper As New AppHelper
            Dim dati = objAppHelper.Leggi_Dati_APP(tipo, piva, data, objParametri_Server)

            r.RispostaOK = True
            r.RispostaStringa = JsonConvert.SerializeObject(dati)

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r

    End Function

    <WebMethod()>
    Public Function LeggiDatiAppStorico(ByVal tipo As String, ByVal piva As String, ByVal data As String, ByVal objP_super_server As String, ByVal objP_server As String, ByVal objP_utenti As String) As RispostaStandard

        Dim r As New RispostaStandard

        Try

            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)

            Dim objAppHelper As New AppHelper
            Dim dati = objAppHelper.Leggi_Dati_APP_Storico(tipo, piva, data, objParametri_Server)

            r.RispostaOK = True
            r.RispostaStringa = JsonConvert.SerializeObject(dati)

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function ConsultaSincroDatiApp(InData As Object) As RispostaStandard

        Dim r As New RispostaStandard
        Dim NomeRoutine As String = "ConsultaSincroDatiApp"


        Try

            Dim datiRequest As String = JsonConvert.SerializeObject(InData)
            Dim objRequest = JsonConvert.DeserializeObject(Of CoreWS_Generic(Of ConsultaSincroDatiApp))(datiRequest)
            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_utenti)

            Dim leggiLingua As New Lingue_Read
            Dim dtLingua As DataTable = leggiLingua.Leggi_datoCODGIAS(objParametri_Server.Lingua_Cod, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Utenti)
            Dim linguaCodiceISO As String = dtLingua.Rows(0)("CodiceISO")
            Threading.Thread.CurrentThread.CurrentUICulture = New Globalization.CultureInfo(linguaCodiceISO)

            Dim objAppHelper As New AppHelper
            Dim dati = objAppHelper.Consulta_Sincro_Dati_App(objRequest.InData.tipiDato,
                                                             objRequest.InData.dataFiltro_inizio,
                                                             objRequest.InData.dataFiltro_fine,
                                                             objRequest.InData.filtroImportati,
                                                             objParametri_Server,
                                                             objParametri_Utenti,
                                                             objRequest.InData.datiAggiuntivi)

            r.RispostaOK = True
            r.RispostaStringa = JsonConvert.SerializeObject(dati)

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r

    End Function

    <WebMethod()>
    Public Function CaricaDatiApp(ByVal tipo As String, ByVal piva As String, ByVal objP_super_server As String, ByVal objP_server As String, ByVal objP_utenti As String) As RispostaStandard

        Dim r As New RispostaStandard
        Dim messaggio As String = ""
        Dim NomeRoutine As String = "CaricaDatiApp" & tipo

        If Application(NomeRoutine) = "1" Then
            r.RispostaOK = False
            r.Errore = "Procedura già in esecuzione"
            Return r
        End If

        Try

            Application(NomeRoutine) = "1"

            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)
            Dim objSincroHelper As New AgronicaCoreMapper.SincroAppHelper(objParametri_Super_Server, objParametri_Server, objParametri_Utenti)

            Dim objAppHelper As New AppHelper
            Dim impostazioni = objAppHelper.Leggi_Impostazioni_APP(objParametri_Utenti)
            Dim agenda As String = If(impostazioni Is Nothing, "", If(impostazioni.ImportAgenda, ""))
            Dim esito = objSincroHelper.SincronizzaDatiApp(messaggio, tipo, True, agenda, False, piva)

            ' aggiorna documenti attivita
            Dim tipi = tipo.Split(",")
            If esito AndAlso tipi.Contains(enum_Dati_App.Attivita) OrElse tipi.Contains(enum_Dati_App.Ricette) Then
                objSincroHelper.AggiornaDocumentiAttivita()
            End If

            Application(NomeRoutine) = "0"

            If Not String.IsNullOrEmpty(messaggio) Then
                messaggio = Replace(messaggio, vbCrLf, "<br>")
            End If

            r.RispostaOK = True
            r.RispostaStringa = messaggio

        Catch ex As Exception

            Application(NomeRoutine) = "0"

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r

    End Function

    <WebMethod()>
    Public Function CaricaDatiApp_NG(InData As CoreWS_Generic(Of CaricaDatiApp)) As RispostaStandard

        Dim r As New RispostaStandard
        Dim messaggio As String = ""
        Dim NomeRoutine As String = "CaricaDatiApp" & InData.InData.tipo

        If Application(NomeRoutine) = "1" Then
            r.RispostaOK = False
            r.Errore = "Procedura già in esecuzione"
            Return r
        End If

        Try

            Application(NomeRoutine) = "1"

            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)
            Dim objSincroHelper As New AgronicaCoreMapper.SincroAppHelper(objParametri_Super_Server, objParametri_Server, objParametri_Utenti)

            Dim objAppHelper As New AppHelper
            Dim impostazioni = objAppHelper.Leggi_Impostazioni_APP(objParametri_Utenti)
            Dim agenda As String = If(impostazioni Is Nothing, "", If(impostazioni.ImportAgenda, ""))
            Dim esito = objSincroHelper.SincronizzaDatiApp(messaggio, InData.InData.tipo, True, agenda, False, InData.InData.piva)

            ' aggiorna documenti attivita
            Dim tipi = InData.InData.tipo.Split(",")
            If esito AndAlso tipi.Contains(enum_Dati_App.Attivita) OrElse tipi.Contains(enum_Dati_App.Ricette) Then
                objSincroHelper.AggiornaDocumentiAttivita()
            End If

            Application(NomeRoutine) = "0"

            If Not String.IsNullOrEmpty(messaggio) Then
                messaggio = Replace(messaggio, vbCrLf, "<br>")
            End If

            r.RispostaOK = True
            r.RispostaStringa = messaggio

        Catch ex As Exception

            Application(NomeRoutine) = "0"

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r

    End Function

    <WebMethod()>
    Public Function ScriviAttivita(InData As Object) As RispostaStandard

        Dim r As New RispostaStandard
        Dim unid As String = ""
        Dim cancellato As Boolean = False
        Dim riferimentoPianificata As String = ""
        Dim aggiornamento As Boolean = False
        Dim tipoDatiApp = enum_Dati_App.Attivita
        Dim attivitaAsJson As String = ""
        Dim objParametri_Server As AgronicaCoreParametri
        Dim agronicaLogInvioChiamateWrite As New AgronicaCoreVarieBIZ.Agronica_Log_Invio_Chiamate_W
        Dim codice As String = "-1"
        Dim versione As String = ""

        Try

            Dim objAppHelper As New AppHelper
            Dim datiRequest As String = JsonConvert.SerializeObject(InData)
            Dim objRequest = JsonConvert.DeserializeObject(Of CoreWS_Generic(Of RicettePerScarico))(datiRequest)
            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_super_server)
            objParametri_Server = Utility.convertStringtoOBJparametri(objRequest.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_utenti)
            Dim objSincroHelper As New AgronicaCoreMapper.SincroAppHelper(objParametri_Super_Server, objParametri_Server, objParametri_Utenti)
            Dim attivita As RicettePerScarico = objRequest.InData
            attivitaAsJson = JsonConvert.SerializeObject(attivita)
            Dim documenti As List(Of DocumentoPerScarico) = attivita.Documenti
            ' se non devo fare restore rimuovo allegati per ridurre occupazione su db
            If Not objAppHelper.Leggi_RestoreDati_APP(objParametri_Utenti) Then
                attivita.Documenti = Nothing
            End If
            Dim dati = JsonConvert.SerializeObject(attivita)
            unid = attivita.guid
            cancellato = attivita.cancellato
            Dim posizione As String = attivita.posizione
            Dim riferimento As String = ""
            Dim piva As String = ""
            Dim sa_cod As Integer

            Dim utentiImpostazioniRead As New Utenti_Impostazioni_Read
            Dim modalitaDemetra = utentiImpostazioniRead.LeggiValoreImpostazioneScalare(enum_Impostazioni_Utenti.SUPERUSER_ModalitaDemetra, objParametri_Utenti.SuperUserUsername, objParametri_Utenti) = "1"

            ' tipo dati app
            tipoDatiApp = If(attivita.isPianificata, enum_Dati_App.Ricette, enum_Dati_App.Attivita)
            Dim ricettaOperazione = attivita.RicetteOperazioni.FirstOrDefault
            If ricettaOperazione IsNot Nothing Then
                Dim ricetta = attivita.Ricette.FirstOrDefault
                piva = ricetta.Piva
                sa_cod = ricetta.Sa_Cod
                Dim stato As Integer = ricettaOperazione.W_Anagrafica_Stati_Cod
                tipoDatiApp = objAppHelper.Leggi_TipoDati_APP(ricettaOperazione.Lav_Cod)
                ' attività pianificate
                If tipoDatiApp = enum_Dati_App.Attivita Then
                    If stato = enum_WWorflow_WAnagraficaStati.Esecuzione_ed_avanzamento_delle_ricette_Da_Eseguire Then
                        tipoDatiApp = enum_Dati_App.Ricette
                    End If
                End If
            ElseIf attivita.Attivita.Count > 0 Then
                Dim attivitaCdG = attivita.Attivita.FirstOrDefault
                piva = attivitaCdG.Piva
                tipoDatiApp = enum_Dati_App.AttivitaCdG
            End If

            If String.IsNullOrEmpty(unid) Then
                unid = Guid.NewGuid().ToString()
            Else
                aggiornamento = True
            End If

            Dim ricetta_cod_esistente = 0
            Dim ricetta_operazione_cod_esistente = 0

            If modalitaDemetra AndAlso aggiornamento AndAlso (tipoDatiApp = enum_Dati_App.Attivita OrElse tipoDatiApp = enum_Dati_App.Ricette) Then
                Dim app_Dati_R As New AgronicaCoreContabDAL.APP_Dati_R
                Dim dt = app_Dati_R.LeggiDatiMinimiDaAppDati(unid, objParametri_Server)
                If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then

                    Dim tipoEsistente = CStr(dt.Rows(0)("Tipo"))

                    Dim attivitaCancellata = CShort(dt.Rows(0)("Cancellato"))
                    If attivitaCancellata = 1 Then
                        r.RispostaOK = False
                        r.Errore = "L'attività è già stata cancellata"
                        Return r
                    End If

                    Dim rif = CStr(dt.Rows(0)("Riferimento"))
                    If Not String.IsNullOrWhiteSpace(rif) AndAlso rif.Contains("|") Then

                        Dim rifSplitted = rif.Split("|")
                        If tipoDatiApp = enum_Dati_App.Attivita Then
                            Dim idAgenda = CInt(rifSplitted(0))
                            If idAgenda <> 0 Then
                                r.RispostaOK = False
                                r.Errore = "L'attività è stata resa non più modificabile su Gias"
                                Return r
                            End If
                        End If

                        If tipoEsistente = enum_Dati_App.Attivita OrElse tipoEsistente = enum_Dati_App.Ricette Then
                            Dim ricetta_operazione_cod = CInt(rifSplitted(1))
                            ricetta_operazione_cod_esistente = ricetta_operazione_cod

                            Dim logRicette As New AgronicaCoreContabDAL.AgronicaLogRicette_R
                            Dim dtLog = logRicette.Leggi_UltimaOperazione("", enum_TipoOperazioneDB.Cancellazione, "Ricette_Operazioni", ricetta_operazione_cod,
                                                                          "", "", "", "", "", "", 0, 0, "", "", objParametri_Server)

                            If dtLog IsNot Nothing AndAlso dtLog.Rows.Count > 0 Then
                                r.RispostaOK = False
                                r.Errore = "L'attività è già stata cancellata"
                                Return r
                            End If
                        ElseIf tipoEsistente = enum_Dati_App.AttivitaDemetra OrElse tipoEsistente = enum_Dati_App.RicetteDemetra Then
                            Dim ricetta_cod = CInt(rifSplitted(1))
                            ricetta_cod_esistente = ricetta_cod

                            Dim logRicette As New AgronicaCoreContabDAL.AgronicaLogRicette_R
                            Dim dtLog = logRicette.Leggi_UltimaOperazione("", enum_TipoOperazioneDB.Cancellazione, "Ricette_Operazioni", "",
                                                                          ricetta_cod, "", "", "", "", "", 0, 0, "", "", objParametri_Server)

                            If dtLog IsNot Nothing AndAlso dtLog.Rows.Count > 0 Then
                                r.RispostaOK = False
                                r.Errore = "L'attività è già stata cancellata"
                                Return r
                            End If
                        End If

                        If cancellato Then 'a questo punto significa che l'attività non è ancora stata cancellata ma verrà cancellata con questa chiamata
                            If ricetta_operazione_cod_esistente = 0 AndAlso ricetta_cod_esistente <> 0 Then
                                Dim logRicette As New AgronicaCoreContabDAL.AgronicaLogRicette_R
                                Dim dtLog = logRicette.Leggi_UltimaOperazione("", enum_TipoOperazioneDB.Lettura, "Ricette_Operazioni", "",
                                                                          ricetta_cod_esistente, "", "", "", "", "", 0, 0, "", "", objParametri_Server)
                                If dtLog IsNot Nothing AndAlso dtLog.Rows.Count > 0 Then
                                    ricetta_operazione_cod_esistente = CInt(dtLog.Rows(0)("Chiave"))
                                End If
                            End If
                        End If
                    End If

                    codice = CStr(dt.Rows(0)("Codice"))
                End If
            End If

            ' scrive dati app
            Dim objRicette As New AgronicaCoreContabBIZ.Ricette_Operazioni_W
            If tipoDatiApp = enum_Dati_App.Ricette Then
                ' normalizza prodotti agenzie
                objRicette.Normalizza_Ricette(
                    piva, sa_cod,
                    attivita.RicetteDettagli,
                    attivita.RicetteDestinazioni,
                    objParametri_Server)
            ElseIf tipoDatiApp = enum_Dati_App.Attivita Then
                ' normalizza macchine / operatori
                objRicette.Normalizza_Attivita(
                    piva,
                    attivita.RicetteDettagli,
                    attivita.Attivita,
                    attivita.AttivitaMovimenti,
                    attivita.AttivitaOperazioni,
                    objParametri_Server,
                    objParametri_Utenti)
            End If

            objRicette.Ricetta_Operazione_ScriviPerAPP(
                unid,
                attivita.Ricette,
                attivita.RicetteOperazioni,
                attivita.RicetteDettagli,
                attivita.RicetteDettaglioTecnico,
                attivita.RicetteXNote,
                attivita.RicetteDestinazioni,
                attivita.Attivita,
                attivita.AttivitaMovimenti,
                attivita.AttivitaOperazioni,
                objParametri_Server,
                aggiornamento
            )

            ' scrive documenti allegati
            If documenti IsNot Nothing Then
                For Each documento In documenti
                    objSincroHelper.ScriviDocumento(documento, unid, aggiornamento)
                Next
            Else
                objSincroHelper.CancellaDocumento(unid)
            End If

            ' importazione dati app
            Dim importAgenda As String = ""
            If Not String.IsNullOrEmpty(attivita.versione) Then
                versione = attivita.versione
            End If
            riferimentoPianificata = If(attivita.riferimentoPianificata, "")

            Dim importazione = objAppHelper.Leggi_ImportDati_APP(tipoDatiApp, objParametri_Utenti, importAgenda) = enum_Import_App.Completo
            objSincroHelper.SincroDatiApp(unid, tipoDatiApp, dati, riferimento, aggiornamento, cancellato, importazione, piva,
                                          codice, importazione, importAgenda, posizione, userAgent:=objRequest.objP.user_Agent, versione:=versione,
                                          origine:=enum_SistemiEsterni.GiasAPP, riferimentoPianificata:=riferimentoPianificata)

            If (cancellato AndAlso ricetta_operazione_cod_esistente <> 0) Then
                'aggiorna app_dati con il riferimento corretto
                Dim rifCorretto = $"0|{ricetta_operazione_cod_esistente}"
                Dim appDatiScrivi As New AgronicaCoreContabDAL.APP_Dati_W
                appDatiScrivi.Aggiorna_Riferimento_DatiAPP(unid, rifCorretto, objParametri_Server)
            End If

            If tipoDatiApp = enum_Dati_App.Attivita Then

                Dim tipoOperazioneDb = GetTipoOperazioneDb(cancellato, aggiornamento)
                agronicaLogInvioChiamateWrite.Scrivi_Log_Invio_Chiamate(enum_Esportazioni_Sistema_Cod.App_Import_Attivita, attivitaAsJson, Date.Now,
                                                                        0, tipoOperazioneDb, "OK", "", objParametri_Server,
                                                                        unid, codice, versione)
            ElseIf tipoDatiApp = enum_Dati_App.Ricette Then
                Dim tipoOperazioneDb = GetTipoOperazioneDb(cancellato, aggiornamento)
                agronicaLogInvioChiamateWrite.Scrivi_Log_Invio_Chiamate(enum_Esportazioni_Sistema_Cod.App_Import_Ricette, attivitaAsJson, Date.Now,
                                                        0, tipoOperazioneDb, "OK", "", objParametri_Server,
                                                        unid, codice, versione)
            End If

            r.RispostaStringa = unid
            r.RispostaOK = True

        Catch ex As Exception

            If tipoDatiApp = enum_Dati_App.Attivita Then

                Dim tipoOperazioneDb = GetTipoOperazioneDb(cancellato, aggiornamento)
                agronicaLogInvioChiamateWrite.Scrivi_Log_Invio_Chiamate(enum_Esportazioni_Sistema_Cod.App_Import_Attivita, attivitaAsJson, Date.Now,
                                                                        0, tipoOperazioneDb, "KO", ex.Message, objParametri_Server,
                                                                        unid, codice, versione)
            ElseIf tipoDatiApp = enum_Dati_App.Ricette Then
                Dim tipoOperazioneDb = GetTipoOperazioneDb(cancellato, aggiornamento)
                agronicaLogInvioChiamateWrite.Scrivi_Log_Invio_Chiamate(enum_Esportazioni_Sistema_Cod.App_Import_Ricette, attivitaAsJson, Date.Now,
                                                                        0, tipoOperazioneDb, "KO", ex.Message, objParametri_Server,
                                                                        unid, codice, versione)
            End If

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r

    End Function

    Private Function GetTipoOperazioneDb(cancellato As Boolean, aggiornamento As Boolean) As enum_TipoOperazioneDB

        Dim tipoOperazioneDb As enum_TipoOperazioneDB = enum_TipoOperazioneDB.Scrittura

        If cancellato Then
            tipoOperazioneDb = enum_TipoOperazioneDB.Cancellazione
        ElseIf aggiornamento Then
            tipoOperazioneDb = enum_TipoOperazioneDB.Modifica
        End If

        Return tipoOperazioneDb

    End Function

    <WebMethod()>
    Public Function ScriviRilievo(InData As Object) As RispostaStandard

        Dim r As New RispostaStandard

        Try

            Dim objAppHelper As New AppHelper
            Dim datiRequest As String = JsonConvert.SerializeObject(InData)
            Dim objRequest = JsonConvert.DeserializeObject(Of CoreWS_Generic(Of AgronicaCoreModelsSTD.attivita.Attivita))(datiRequest)
            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_utenti)
            Dim objSincroHelper As New AgronicaCoreMapper.SincroAppHelper(objParametri_Super_Server, objParametri_Server, objParametri_Utenti)
            Dim rilievo As AgronicaCoreModelsSTD.attivita.Attivita = objRequest.InData
            Dim documenti As List(Of documenti.Documento) = rilievo.documenti
            ' se non devo fare restore rimuovo allegati per ridurre occupazione su db
            If Not objAppHelper.Leggi_RestoreDati_APP(objParametri_Utenti) Then
                rilievo.documenti = Nothing
            End If

            Dim tipoDatiApp = enum_Dati_App.Rilievi
            If rilievo.statoWorkflow = AgronicaCoreModelsSTD.attivita.Attivita.StatiWorkflowQdC.Eseguito Then
                tipoDatiApp = enum_Dati_App.Visite
            End If

            Dim idAttivita As Integer = 0
            If rilievo.job.getTipo() = TipiJob.JOB_COMPOSITE Then
                Dim jobRilievo As JobComposito = rilievo.job
                'rilievo.job = jobRilievo.lavorazione
                idAttivita = jobRilievo.attivitaCDG.getCodice()
            End If

            Dim importazioneDatiApp = objAppHelper.Leggi_ImportDati_APP(enum_Dati_App.Rilievi, objParametri_Utenti)
            Dim importazione = importazioneDatiApp = enum_Import_App.Completo

            Dim piva As String = rilievo.centroAziendale.primaryKey.partitaIva
            Dim codice As String = rilievo.codice
            Dim unid As String = rilievo.guid
            Dim cancellato As Boolean = rilievo.cancellato
            Dim aggiornamento As Boolean = False
            Dim riferimento As String = ""

            If String.IsNullOrEmpty(unid) Then
                unid = Guid.NewGuid().ToString()
            Else
                aggiornamento = True
            End If

            ' scrivo visita rilievo
            If tipoDatiApp = enum_Dati_App.Visite Then
                Dim objVisiteApp As New AgronicaCoreVisiteBIZ.Visite_APP
                Dim Posizione As String = ""
                Dim Operatore As String = objParametri_Server.UtenteUsername
                If rilievo.latitude <> 0 AndAlso rilievo.longitude <> 0 Then
                    Posizione = Replace(rilievo.latitude & "|" & rilievo.longitude, ",", ".")
                End If
                If CInt(rilievo.codice) > 0 Then
                    riferimento = rilievo.codice
                End If
                Dim Visita = New AgronicaCoreEntityFramework_POCO.APP_Visite With {
                    .Visita_Cod = If(CInt(rilievo.codice) > 0, CInt(rilievo.codice), -1),
                    .Tipo_Visita = 0,
                    .Piva = rilievo.centroAziendale.primaryKey.partitaIva,
                    .Sa_Cod = rilievo.centroAziendale.primaryKey.codice,
                    .Data_Visita = rilievo.inizio,
                    .Validita_Inizio = rilievo.inizio,
                    .Validita_Fine = rilievo.fine,
                    .Operatore = Operatore,
                    .Posizione = Posizione,
                    .Lav_Cod = CostantiPersonalizzate.LAVCOD_VISITA,
                    .Note = rilievo.descrizione
                }
                Dim VisitaDettaglio = New AgronicaCoreEntityFramework_POCO.APP_Visite_Dettagli With {
                    .Visita_Cod = Visita.Visita_Cod,
                    .Visita_Dettaglio_Cod = -1,
                    .Id_Attivita = idAttivita,
                    .Descrizione = rilievo.note
                }
                objVisiteApp.ScriviVisiteAPP(
                    unid,
                    New List(Of AgronicaCoreEntityFramework_POCO.APP_Visite) From {Visita},
                    New List(Of AgronicaCoreEntityFramework_POCO.APP_Visite_Dettagli) From {VisitaDettaglio},
                    New List(Of AgronicaCoreEntityFramework_POCO.APP_Visite_Destinazioni),
                    objParametri_Server,
                    aggiornamento
                )
            End If

            ' scrive documento allegato
            If Not cancellato AndAlso documenti IsNot Nothing AndAlso documenti.Count > 0 Then
                objSincroHelper.ScriviDocumento(piva, documenti(0), unid, aggiornamento)
            Else
                objSincroHelper.CancellaDocumento(unid)
            End If

            'valorizzo utilizzoTerreno
            If Not IsNothing(rilievo.risorse) AndAlso rilievo.risorse.Count > 0 Then

                Dim index = rilievo.risorse.FindIndex(Function(x) x.classType = AgronicaCoreModelsSTD.costanti.ClassType.RisorsaSpecie)

                If index > -1 Then
                    Dim risorsaSpecie As AgronicaCoreModelsSTD.attivita.risorse.RisorsaSpecie = rilievo.risorse(index)

                    If Not IsNothing(risorsaSpecie) AndAlso Not IsNothing(risorsaSpecie.specie) Then
                        Dim varieta As New AgronicaCoreModelsSTD.metaschema.utilizzi.Varieta(0) With {
                            .specie = risorsaSpecie.specie
                         }

                        rilievo.utilizzoTerreno = varieta

                    End If

                End If
            End If

            ' importazione dati app
            Dim dati As String = JsonConvert.SerializeObject(rilievo)
            objSincroHelper.SincroDatiApp(unid, tipoDatiApp, dati, riferimento, aggiornamento, cancellato, importazione, piva, codice, importazione, agenda:="NG", userAgent:=objRequest.objP.user_Agent, origine:=enum_SistemiEsterni.GiasAPP)

            r.RispostaStringa = unid
            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function ScriviVisite(InData As Object) As RispostaStandard

        Dim r As New RispostaStandard

        Try

            Dim objAppHelper As New AppHelper
            Dim datiRequest As String = JsonConvert.SerializeObject(InData)
            Dim objRequest = JsonConvert.DeserializeObject(Of CoreWS_Generic(Of VisitePerScarico))(datiRequest)
            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_utenti)
            Dim objSincroHelper As New AgronicaCoreMapper.SincroAppHelper(objParametri_Super_Server, objParametri_Server, objParametri_Utenti)
            Dim visite As VisitePerScarico = objRequest.InData
            Dim documenti As List(Of DocumentoPerScarico) = visite.VisiteDocumenti
            ' se non devo fare restore rimuovo allegati per ridurre occupazione su db
            If Not objAppHelper.Leggi_RestoreDati_APP(objParametri_Utenti) Then
                visite.VisiteDocumenti = Nothing
            End If
            Dim dati = JsonConvert.SerializeObject(visite)
            Dim unid As String = visite.guid
            Dim cancellato As Boolean = visite.cancellato
            Dim aggiornamento As Boolean = False
            Dim riferimento As String = ""
            Dim posizione As String = ""
            Dim piva As String = ""
            Dim codice As String = ""

            Dim visita = visite.Visite.FirstOrDefault
            If visita IsNot Nothing Then
                piva = visita.Piva
                codice = visita.Visita_Cod
                If visita.Visita_Cod > 0 Then
                    riferimento = visita.Visita_Cod
                End If
            End If

            If String.IsNullOrEmpty(unid) Then
                unid = Guid.NewGuid().ToString()
            Else
                aggiornamento = True
            End If

            ' scrive dati app
            Dim objVisiteApp As New AgronicaCoreVisiteBIZ.Visite_APP
            objVisiteApp.ScriviVisiteAPP(
                unid,
                visite.Visite,
                visite.VisiteDettagli,
                visite.VisiteDestinazioni,
                objParametri_Server,
                aggiornamento
            )

            ' scrive documenti allegati alle visite dell'app
            If documenti IsNot Nothing Then
                For Each documento In documenti
                    objSincroHelper.ScriviDocumento(documento, unid, aggiornamento)
                Next
            Else
                objSincroHelper.CancellaDocumenti(unid)
            End If

            ' scrive rilievo collegato a visita
            If visite.VisiteRilievi IsNot Nothing Then
                posizione = visite.VisiteRilievi.posizione
                Dim objRicette As New AgronicaCoreContabBIZ.Ricette_Operazioni_W
                objRicette.Ricetta_Operazione_ScriviPerAPP(
                    unid,
                    visite.VisiteRilievi.Ricette,
                    visite.VisiteRilievi.RicetteOperazioni,
                    visite.VisiteRilievi.RicetteDettagli,
                    visite.VisiteRilievi.RicetteDettaglioTecnico,
                    visite.VisiteRilievi.RicetteXNote,
                    visite.VisiteRilievi.RicetteDestinazioni,
                    visite.VisiteRilievi.Attivita,
                    visite.VisiteRilievi.AttivitaMovimenti,
                    visite.VisiteRilievi.AttivitaOperazioni,
                    objParametri_Server,
                    aggiornamento
                )
            End If

            ' importazione dati app
            Dim importazione = objAppHelper.Leggi_ImportDati_APP(enum_Dati_App.Visite, objParametri_Utenti) = enum_Import_App.Completo
            objSincroHelper.SincroDatiApp(unid, enum_Dati_App.Visite, dati, riferimento, aggiornamento, cancellato, importazione, piva, codice, importazione, "", posizione, userAgent:=objRequest.objP.user_Agent, origine:=enum_SistemiEsterni.GiasAPP)

            r.RispostaStringa = unid
            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function ScriviDocumenti(InData As Object) As RispostaStandard

        Dim r As New RispostaStandard

        Try

            Dim objAppHelper As New AppHelper
            Dim datiRequest As String = JsonConvert.SerializeObject(InData)
            Dim objRequest = JsonConvert.DeserializeObject(Of CoreWS_Generic(Of DocumentoPerScarico))(datiRequest)
            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_utenti)
            Dim objSincroHelper As New AgronicaCoreMapper.SincroAppHelper(objParametri_Super_Server, objParametri_Server, objParametri_Utenti)
            Dim documento As DocumentoPerScarico = objRequest.InData
            Dim unid As String = documento.guid
            Dim cancellato As Boolean = documento.cancellato
            Dim aggiornamento As Boolean = False
            Dim riferimento As String = ""
            Dim piva As String = ""
            Dim codice As String = ""

            If documento IsNot Nothing Then
                piva = documento.Piva
                codice = documento.Documento_Cod
            End If

            If String.IsNullOrEmpty(unid) Then
                unid = Guid.NewGuid().ToString()
            Else
                aggiornamento = True
            End If

            ' scrive dati app
            objSincroHelper.ScriviDocumento(documento, unid, aggiornamento)

            ' importazione dati app
            Dim importazioneDatiApp = objAppHelper.Leggi_ImportDati_APP(enum_Dati_App.Documenti, objParametri_Utenti)
            Dim importazione = importazioneDatiApp = enum_Import_App.Completo
            ' se non devo fare restore rimuovo allegati per ridurre occupazione su db
            If Not objAppHelper.Leggi_RestoreDati_APP(objParametri_Utenti) Then
                documento.Allegati = New List(Of DocumentoAllegato)
            End If
            Dim dati = JsonConvert.SerializeObject(documento)
            objSincroHelper.SincroDatiApp(unid, enum_Dati_App.Documenti, dati, riferimento, aggiornamento, cancellato, importazione, piva, codice, userAgent:=objRequest.objP.user_Agent, origine:=enum_SistemiEsterni.GiasAPP)

            r.RispostaStringa = unid
            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function


    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function ScriviDocumentiImport(InData As Object) As RispostaStandard

        Dim r As New RispostaStandard

        Try

            Dim objAppHelper As New AppHelper
            Dim datiRequest As String = JsonConvert.SerializeObject(InData)
            Dim objRequest = JsonConvert.DeserializeObject(Of CoreWS_Generic(Of DocumentoPerImport))(datiRequest)
            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_utenti)
            Dim objSincroHelper As New AgronicaCoreMapper.SincroAppHelper(objParametri_Super_Server, objParametri_Server, objParametri_Utenti)

            Dim datiJson As String = AgroZip.DeCompressioneBase64(1, objRequest.InData.dati.ToString)
            Dim tzh As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}
            Dim documento As DatiDocumentoAllegatoPerImport = JsonConvert.DeserializeObject(Of DatiDocumentoAllegatoPerImport)(datiJson, tzh)

            Dim aggiornamento As Boolean = False
            Dim riferimento As String = ""
            Dim codice As String = ""
            Dim unid As String = ""
            Dim result As String = ""
            Dim piva As String = ""
            Dim errorMessage As String = ""

            Dim DocumentoPerScarico = New DocumentoPerScarico
            Dim FlagConnessioneLocale, FlagTransazioneLocale As Boolean
            Dim erroriList As New List(Of String)
            Dim bEsito As Boolean = True
            Try

                ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale, FlagTransazioneLocale, objParametri_Server)

                'Ricavo la piva dal cuaa
                Dim ObjImpresa As New AgronicaCoreAnagrafeDAL.Imprese_Read
                piva = ObjImpresa.Piva_From_CUAA(objRequest.InData.CUAA, objParametri_Server)

                If Trim(piva) <> "" Then

                    For Each allegato In documento.Documenti

                        If bEsito Then

                            unid = Guid.NewGuid().ToString()

                            'scrive dati app
                            objSincroHelper.ScriviDocumentoPerImport(piva, allegato, unid, errorMessage)

                            If Trim(errorMessage) = "" Then

                                ' importazione dati app
                                Dim importazioneDatiApp = objAppHelper.Leggi_ImportDati_APP(enum_Dati_App.Documenti, objParametri_Utenti)
                                Dim importazione = importazioneDatiApp = enum_Import_App.Completo
                                ' se non devo fare restore rimuovo allegati per ridurre occupazione su db
                                'If Not objAppHelper.Leggi_RestoreDati_APP(objParametri_Utenti) Then
                                '    documento.Documenti = New List(Of DocumentoAllegatoPerImport)
                                'End If
                                Dim dati = JsonConvert.SerializeObject(allegato)

                                bEsito = objSincroHelper.SincroDatiApp(unid, enum_Dati_App.Documenti, dati, riferimento, False, False, importazione, piva, codice, userAgent:=objRequest.objP.user_Agent, origine:=enum_SistemiEsterni.GiasAPP)

                            End If

                        End If

                    Next

                    If Not bEsito Or Trim(errorMessage) <> "" Then
                        Throw New Exception("Si è verificato un errore durante la procedura di importazione.")
                    Else
                        result = "Importazione terminata correttamente." & errorMessage
                    End If

                Else
                    result = "Partita Iva non corretta."

                End If

                ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri_Server)

            Catch ex As Exception

                If objParametri_Server.objTransazione IsNot Nothing Then
                    ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)
                End If

                Dim messaggio As String = "Imprortazione documenti non riuscita!. " & ex.Message
                erroriList.Add(messaggio)


            Finally

                ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri_Server)

            End Try

            If erroriList.Count > 0 Then
                result = String.Join(" | ", erroriList)
            End If

            r.RispostaStringa = result
            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function


    <WebMethod()>
    Public Function ScriviImpresa(InData As Object) As rispostaStandard(Of Impresa)

        Dim r As New rispostaStandard(Of Impresa)

        Try

            Dim objAppHelper As New AppHelper
            Dim datiRequest As String = JsonConvert.SerializeObject(InData)
            Dim objRequest = JsonConvert.DeserializeObject(Of CoreWS_Generic(Of Impresa))(datiRequest)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_utenti)
            Dim objImpresa As New Impresa_W
            Dim impresa As Impresa = objRequest.InData
            Dim tipoOperazione = enum_TipoOperazioneDB.Lettura
            Dim cancellato As Boolean = impresa.flag_cancellazione
            Dim riferimento As String = ""
            Dim errore As String = ""

            ' controllo guid sulla tabella di frontiera per evitare doppioni (7202)
            Dim checkDatiAPP As Boolean = impresa.partitaIva = "" AndAlso Not String.IsNullOrEmpty(impresa.guid)

            If checkDatiAPP Then
                riferimento = objAppHelper.Check_Dati_APP(impresa.guid, enum_Dati_App.Imprese, objParametri_Server)
                If riferimento = "ERRORE" Then
                    r.RispostaOK = False
                    r.Errore = "Operazione in corso"
                    Return r
                ElseIf Not String.IsNullOrEmpty(riferimento) Then
                    impresa.partitaIva = riferimento
                End If
                objAppHelper.Scrivi_Dati_APP(impresa.guid, enum_Dati_App.Imprese, datiRequest, riferimento, cancellato, objParametri_Server, User_Agent:=objRequest.objP.user_Agent)
            End If

            If Not String.IsNullOrEmpty(impresa.partitaIva) Then
                If cancellato Then
                    tipoOperazione = enum_TipoOperazioneDB.Cancellazione
                Else
                    tipoOperazione = enum_TipoOperazioneDB.Modifica
                End If
            ElseIf Not cancellato Then
                impresa.partitaIva = ""
                tipoOperazione = enum_TipoOperazioneDB.Scrittura
            End If

            ' scrive impresa
            If tipoOperazione <> enum_TipoOperazioneDB.Lettura Then
                Try
                    objImpresa.Scrivi_Impresa_APP(impresa, tipoOperazione, objParametri_Server, objParametri_Utenti)
                Catch ex As Exception
                    errore = "Errore durante l'operazione"
                End Try
            End If

            ' aggiorna riferimento dati app
            If checkDatiAPP Then
                objAppHelper.Aggiorna_Dati_APP(impresa.guid, enum_Dati_App.Imprese, errore, impresa.partitaIva, cancellato, True, objParametri_Server)
            End If

            If String.IsNullOrEmpty(errore) Then
                r.RispostaStringa = impresa
                r.RispostaOK = True
            Else
                r.RispostaOK = False
                r.Errore = errore
            End If

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r

    End Function


    <WebMethod()>
    Public Function ScriviCentro(InData As Object) As rispostaStandard(Of AgronicaCoreModelsSTD.anagrafiche.CentroAziendale)

        Dim r As New rispostaStandard(Of AgronicaCoreModelsSTD.anagrafiche.CentroAziendale)

        Try

            Dim datiRequest As String = JsonConvert.SerializeObject(InData)
            Dim objRequest = JsonConvert.DeserializeObject(Of CoreWS_Generic(Of AgronicaCoreModelsSTD.anagrafiche.CentroAziendale))(datiRequest)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_utenti)
            Dim objCentro As New CentroAziendale_W
            Dim centro As AgronicaCoreModelsSTD.anagrafiche.CentroAziendale = objRequest.InData
            Dim response As New List(Of String)

            Dim tipoOperazione = enum_TipoOperazioneDB.Lettura
            Dim cancellato As Boolean = centro.flag_cancellazione

            If centro.primaryKey.codice > 0 Then
                If cancellato Then
                    tipoOperazione = enum_TipoOperazioneDB.Cancellazione
                Else
                    tipoOperazione = enum_TipoOperazioneDB.Modifica
                End If
            ElseIf Not cancellato Then
                centro.primaryKey.codice = 0
                tipoOperazione = enum_TipoOperazioneDB.Scrittura
            End If

            If tipoOperazione <> enum_TipoOperazioneDB.Lettura Then
                objCentro.Scrivi_Centro_APP(centro, tipoOperazione, objParametri_Server, objParametri_Utenti, True)
            End If

            r.RispostaStringa = centro
            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r

    End Function

    <WebMethod()>
    Public Function ScriviAppezzamento(InData As Object) As rispostaStandard(Of anagrafiche.Appezzamento)

        Dim r As New rispostaStandard(Of anagrafiche.Appezzamento)

        Try

            Dim objAppHelper As New AppHelper
            Dim datiRequest As String = JsonConvert.SerializeObject(InData)
            Dim objRequest = JsonConvert.DeserializeObject(Of CoreWS_Generic(Of anagrafiche.Appezzamento))(datiRequest)
            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_utenti)

            Dim objAppezzamento As New Appezzamento_W
            Dim appezzamento As anagrafiche.Appezzamento = objRequest.InData
            Dim piva As String = appezzamento.primaryKey.centroAziendalePK.partitaIva
            Dim codice As String = appezzamento.primaryKey.codice
            Dim unid As String = appezzamento.guid
            Dim cancellato As Boolean = appezzamento.flag_cancellazione
            Dim riferimento As String = ""
            Dim errore As String = ""

            ' controllo guid sulla tabella di frontiera per evitare doppioni (7202)
            Dim checkDatiAPP As Boolean = codice <= 0 AndAlso Not String.IsNullOrEmpty(unid)

            If checkDatiAPP Then
                riferimento = objAppHelper.Check_Dati_APP(appezzamento.guid, enum_Dati_App.Appezzamenti, objParametri_Server)
                If riferimento = "ERRORE" Then
                    r.RispostaOK = False
                    r.Errore = "Operazione in corso"
                    Return r
                ElseIf Not String.IsNullOrEmpty(riferimento) Then
                    Dim chiave = riferimento.Split("|")
                    If chiave.Length = 3 Then
                        appezzamento.primaryKey.centroAziendalePK.partitaIva = chiave(0)
                        appezzamento.primaryKey.centroAziendalePK.codice = chiave(1)
                        appezzamento.primaryKey.codice = chiave(2)
                        objRequest.InData = appezzamento
                    End If
                End If
                objAppHelper.Scrivi_Dati_APP(unid, enum_Dati_App.Appezzamenti, datiRequest, riferimento, cancellato, objParametri_Server, piva, codice, User_Agent:=objRequest.objP.user_Agent)
            End If

            Try
                ' scrive appezzamento
                r = New Appezzamento().Scrivi_Appezzamento_Anagrafica(objRequest)

                If r.RispostaOK = False AndAlso r.ErroriGias.Count > 0 Then
                    For Each ex In r.ErroriGias
                        If ex.messaggio.Contains("GUID") Then
                            'Rimandiamo indietro l'appezzamento già esistente con codici aggiornati da web, Gestione casistica doppio appezzamento (9042)
                            r = New Appezzamento().Leggi_Appezzamento_AnagraficaXGUID(objRequest)
                            Exit For
                        End If
                    Next
                End If
            Catch ex As GiasException
                'Errore non gestito
            End Try


            If r.RispostaOK Then
                appezzamento = r.RispostaStringa
                riferimento = appezzamento.primaryKey.centroAziendalePK.partitaIva & "|" & appezzamento.primaryKey.centroAziendalePK.codice & "|" & appezzamento.primaryKey.codice
            Else
                errore = r.Errore.Substring(0, Math.Min(2000, r.Errore.Length))
            End If

            ' aggiorna riferimento dati app
            If checkDatiAPP Then
                objAppHelper.Aggiorna_Dati_APP(unid, enum_Dati_App.Appezzamenti, errore, riferimento, cancellato, True, objParametri_Server)
            End If

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r

    End Function

    <WebMethod()>
    Public Function ScriviMagazzini(InData As Object) As RispostaStandard

        Dim r As New RispostaStandard

        Try

            Dim datiRequest As String = JsonConvert.SerializeObject(InData)
            Dim objRequest = JsonConvert.DeserializeObject(Of CoreWS_Generic(Of List(Of Fabbricato)))(datiRequest)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_utenti)
            Dim objFabbricati As New Fabbricato_W
            Dim magazzini As List(Of Fabbricato) = objRequest.InData
            Dim response As New List(Of String)

            For Each magazzino In magazzini

                Dim tipoOperazione = enum_TipoOperazioneDB.Lettura
                Dim cancellato As Boolean = magazzino.flag_cancellazione

                If magazzino.primaryKey.codice > 0 Then
                    If cancellato Then
                        tipoOperazione = enum_TipoOperazioneDB.Cancellazione
                    Else
                        tipoOperazione = enum_TipoOperazioneDB.Modifica
                    End If
                ElseIf Not cancellato Then
                    magazzino.primaryKey.codice = 0
                    tipoOperazione = enum_TipoOperazioneDB.Scrittura
                End If

                If tipoOperazione <> enum_TipoOperazioneDB.Lettura Then
                    objFabbricati.Scrivi_Fabbricato_APP(magazzino, tipoOperazione, objParametri_Server, objParametri_Utenti)
                End If

                response.Add(magazzino.primaryKey.codice)

            Next

            r.RispostaStringa = String.Join("|", response)
            'r.RispostaStringa = objFabbricati.ScriviMagazzini(magazzini, objParametri_Server, objParametri_Utenti)
            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r

    End Function

    <WebMethod()>
    Public Function ScriviMovimenti(InData As Object) As RispostaStandard

        Dim r As New RispostaStandard

        Try

            Dim objAppHelper As New AppHelper
            Dim datiRequest As String = JsonConvert.SerializeObject(InData)
            Dim objRequest = JsonConvert.DeserializeObject(Of CoreWS_Generic(Of List(Of MovimentoDiMagazzino)))(datiRequest)
            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_utenti)
            Dim objSincroHelper As New AgronicaCoreMapper.SincroAppHelper(objParametri_Super_Server, objParametri_Server, objParametri_Utenti)
            Dim movimenti As List(Of MovimentoDiMagazzino) = objRequest.InData
            Dim response As New List(Of String)

            Dim importazioneDatiApp = objAppHelper.Leggi_ImportDati_APP(enum_Dati_App.Movimenti, objParametri_Utenti)
            Dim importazione = importazioneDatiApp = enum_Import_App.Completo

            For Each movimento In movimenti

                Dim piva As String = movimento.Magazzino.primaryKey.centroAziendalePK.partitaIva
                Dim codice As String = movimento.Codice
                Dim unid As String = movimento.guid
                Dim cancellato As Boolean = movimento.cancellato
                Dim aggiornamento As Boolean = False
                Dim riferimento As String = ""

                If String.IsNullOrEmpty(unid) Then
                    unid = Guid.NewGuid().ToString()
                Else
                    aggiornamento = True
                End If

                If aggiornamento Then
                    Dim movimentoCancellato As Boolean
                    Dim movimentoModificatoSuGias As Boolean

                    ControllaModificabilitaEntita(unid, objParametri_Server, objParametri_Utenti, movimentoCancellato, movimentoModificatoSuGias)

                    If (movimentoCancellato) Then
                        r.RispostaOK = False
                        r.Errore = "Il movimento è già stata cancellato"
                        Return r
                    ElseIf (movimentoModificatoSuGias) Then
                        r.RispostaOK = False
                        r.Errore = "Il movimento non è modificabile perché è stato modificato su gias"
                        Return r
                    End If
                End If

                ' scrive documento allegato
                If Not cancellato AndAlso movimento.documenti IsNot Nothing AndAlso movimento.documenti.Count > 0 Then
                    objSincroHelper.ScriviDocumento(piva, movimento.documenti(0), unid, aggiornamento)
                Else
                    objSincroHelper.CancellaDocumento(unid)
                End If

                Dim versione As String = ""
                If Not String.IsNullOrEmpty(movimento.versione) Then
                    versione = movimento.versione
                End If

                ' importazione dati app
                Dim dati As String = JsonConvert.SerializeObject(movimento)
                objSincroHelper.SincroDatiApp(unid, enum_Dati_App.Movimenti, dati, riferimento, aggiornamento, cancellato, importazione, piva, codice, importazione, userAgent:=objRequest.objP.user_Agent, origine:=enum_SistemiEsterni.GiasAPP, versione:=versione)

                response.Add(unid)

            Next

            r.RispostaStringa = String.Join("|", response)
            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r

    End Function


    <WebMethod()>
    Public Function ScriviAcquisto(InData As Object) As RispostaStandard

        Dim r As New RispostaStandard

        Try

            Dim objAppHelper As New AppHelper
            Dim datiRequest As String = JsonConvert.SerializeObject(InData)
            Dim objRequest = JsonConvert.DeserializeObject(Of CoreWS_Generic(Of Acquisto))(datiRequest)
            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_utenti)
            Dim objSincroHelper As New AgronicaCoreMapper.SincroAppHelper(objParametri_Super_Server, objParametri_Server, objParametri_Utenti)
            Dim acquisto As Acquisto = objRequest.InData

            Dim importazioneDatiApp = objAppHelper.Leggi_ImportDati_APP(enum_Dati_App.Acquisti, objParametri_Utenti)
            Dim importazione = importazioneDatiApp = enum_Import_App.Completo

            Dim piva As String = acquisto.centroAziendale.primaryKey.partitaIva
            Dim codice As String = acquisto.codice
            Dim unid As String = acquisto.guid
            Dim cancellato As Boolean = acquisto.cancellato
            Dim aggiornamento As Boolean = False
            Dim riferimento As String = ""

            If String.IsNullOrEmpty(unid) Then
                unid = Guid.NewGuid().ToString()
            Else
                aggiornamento = True
            End If

            If aggiornamento Then
                Dim acquistoCancellato As Boolean
                Dim acquistoModificatoSuGias As Boolean

                ControllaModificabilitaEntita(unid, objParametri_Server, objParametri_Utenti, acquistoCancellato, acquistoModificatoSuGias)

                If (acquistoCancellato) Then
                    r.RispostaOK = False
                    r.Errore = "L'acquisto è già stata cancellato"
                    Return r
                ElseIf (acquistoModificatoSuGias) Then
                    r.RispostaOK = False
                    r.Errore = "L'acquisto non è modificabile perché è stato modificato su gias"
                    Return r
                End If
            End If

            ' scrive documento allegato
            If Not cancellato AndAlso acquisto.documenti IsNot Nothing AndAlso acquisto.documenti.Count > 0 Then
                objSincroHelper.ScriviDocumento(piva, acquisto.documenti(0), unid, aggiornamento)
            Else
                objSincroHelper.CancellaDocumento(unid)
            End If

            Dim versione As String = ""
            If Not String.IsNullOrEmpty(acquisto.versione) Then
                versione = acquisto.versione
            End If

            ' importazione dati app
            Dim dati As String = JsonConvert.SerializeObject(acquisto)
            objSincroHelper.SincroDatiApp(unid, enum_Dati_App.Acquisti, dati, riferimento, aggiornamento, cancellato, importazione, piva, codice, importazione, userAgent:=objRequest.objP.user_Agent, origine:=enum_SistemiEsterni.GiasAPP, versione:=versione)

            r.RispostaStringa = unid
            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r

    End Function

    Private Function ControllaModificabilitaEntita(ByVal unid As String, ByRef objParametri_Server As AgronicaCoreParametri, ByRef objParametri_Utenti As AgronicaCoreParametri, ByRef cancellata As Boolean, ByRef modificataSuGias As Boolean)

        Dim utentiImpostazioniRead As New Utenti_Impostazioni_Read
        Dim modalitaDemetra = utentiImpostazioniRead.LeggiValoreImpostazioneScalare(enum_Impostazioni_Utenti.SUPERUSER_ModalitaDemetra, objParametri_Utenti.SuperUserUsername, objParametri_Utenti) = "1"

        If modalitaDemetra Then
            Dim app_Dati_R As New AgronicaCoreContabDAL.APP_Dati_R
            Dim dt = app_Dati_R.LeggiDatiMinimiDaAppDati(unid, objParametri_Server)
            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then

                Dim entitaCancellata = CShort(dt.Rows(0)("Cancellato"))
                If entitaCancellata = 1 Then
                    cancellata = True
                    Exit Function
                End If

                Dim rif = CStr(dt.Rows(0)("Riferimento"))
                If Not String.IsNullOrEmpty(rif) Then

                    Dim id_agenda = CInt(rif)
                    Dim logAgenda As New AgronicaCoreContabDAL.AgronicaLogAgenda_R
                    Dim dtLog = logAgenda.Leggi_UltimaOperazione("", enum_TipoOperazioneDB.Lettura, id_agenda, "", 0, 0, 0, "", "Data_Ora_RegistrazioneLog desc", objParametri_Server)

                    If dtLog IsNot Nothing AndAlso dtLog.Rows.Count > 0 Then

                        Dim ultimaOperazione = CInt(dtLog.Rows(0)("UltimaOperazione"))
                        Dim origine = CInt(dtLog.Rows(0)("Origine"))

                        If ultimaOperazione = enum_TipoOperazioneDB.Cancellazione Then
                            cancellata = True
                            Exit Function
                        ElseIf (ultimaOperazione = enum_TipoOperazioneDB.Scrittura OrElse ultimaOperazione = enum_TipoOperazioneDB.Modifica) AndAlso origine = enum_SistemiEsterni.gias Then
                            modificataSuGias = True
                            Exit Function
                        End If
                    End If
                End If
            End If
        End If

    End Function

    <WebMethod()>
    Public Function ScriviManutenzioni(InData As Object) As RispostaStandard

        Dim r As New RispostaStandard

        Try

            Dim objAppHelper As New AppHelper
            Dim datiRequest As String = JsonConvert.SerializeObject(InData)
            Dim objRequest = JsonConvert.DeserializeObject(Of CoreWS_Generic(Of List(Of Manutenzione)))(datiRequest)
            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_utenti)
            Dim objSincroHelper As New AgronicaCoreMapper.SincroAppHelper(objParametri_Super_Server, objParametri_Server, objParametri_Utenti)
            Dim manutenzioni As List(Of Manutenzione) = objRequest.InData
            Dim response As New List(Of String)

            Dim importazioneDatiApp = objAppHelper.Leggi_ImportDati_APP(enum_Dati_App.Manutenzioni, objParametri_Utenti)
            Dim importazione = importazioneDatiApp = enum_Import_App.Completo

            For Each manutenzione In manutenzioni

                Dim piva As String = manutenzione.macchina.partitaIva
                Dim codice As String = manutenzione.codice
                Dim unid As String = manutenzione.guid
                Dim cancellato As Boolean = manutenzione.cancellato
                Dim aggiornamento As Boolean = False
                Dim riferimento As String = ""

                If String.IsNullOrEmpty(unid) Then
                    unid = Guid.NewGuid().ToString()
                Else
                    aggiornamento = True
                End If

                ' importazione dati app
                Dim dati As String = JsonConvert.SerializeObject(manutenzione)
                objSincroHelper.SincroDatiApp(unid, enum_Dati_App.Manutenzioni, dati, riferimento, aggiornamento, cancellato, importazione, piva, codice, userAgent:=objRequest.objP.user_Agent, origine:=enum_SistemiEsterni.GiasAPP)

                response.Add(unid)

            Next

            r.RispostaStringa = String.Join("|", response)
            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r

    End Function

    <WebMethod()>
    Public Function ScriviMacchine(InData As Object) As RispostaStandard

        Dim r As New RispostaStandard

        Try

            Dim datiRequest As String = JsonConvert.SerializeObject(InData)
            Dim objRequest = JsonConvert.DeserializeObject(Of CoreWS_Generic(Of List(Of ParcoMacchine)))(datiRequest)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_utenti)
            Dim objMacchinaBIZ As New AgronicaCoreContabBIZ.Parco_Macchine_W
            Dim macchine As List(Of ParcoMacchine) = objRequest.InData
            Dim response As New List(Of String)

            Dim alert_W As New AgronicaCoreScadenziario_BIZ.Alert_W

            For Each macchina In macchine

                Dim tipoOperazione = enum_TipoOperazioneDB.Lettura
                Dim cancellato As Boolean = macchina.flag_cancellazione

                If macchina.codice > 0 Then
                    If cancellato Then
                        tipoOperazione = enum_TipoOperazioneDB.Cancellazione
                    Else
                        tipoOperazione = enum_TipoOperazioneDB.Modifica
                    End If
                ElseIf Not cancellato Then
                    macchina.codice = 0
                    tipoOperazione = enum_TipoOperazioneDB.Scrittura
                End If

                If tipoOperazione <> enum_TipoOperazioneDB.Lettura Then
                    If tipoOperazione = enum_TipoOperazioneDB.Modifica Then
                        Dim objMacchine As New AgronicaCoreContabBIZ.Parco_Macchine_R
                        Dim m = objMacchine.Leggi_Macchina(macchina.partitaIva, macchina.codice, objParametri_Server)
                        ' se tipo app = tipo server mantendo il terzo livello presente su server
                        If m.dettaglio_1 IsNot Nothing AndAlso macchina.dettaglio_1 IsNot Nothing AndAlso m.dettaglio_1.codice = macchina.dettaglio_1.codice Then
                            macchina.dettaglio_2 = m.dettaglio_2
                        Else
                            m.tipo = macchina.tipo
                            m.dettaglio_1 = macchina.dettaglio_1
                            m.dettaglio_2 = macchina.dettaglio_2
                        End If
                        ' sovrascrivo campi gestitu su app
                        m.descrizione = macchina.descrizione
                        m.modello = macchina.modello
                        m.validita = macchina.validita
                        m.titolo_Possesso = macchina.titolo_Possesso
                        ' m.finalita = macchina.finalita
                        m.targa = macchina.targa
                        m.n_Immatricolazione = macchina.n_Immatricolazione
                        m.data_Immatricolazione = macchina.data_Immatricolazione
                        m.codice_stringa = macchina.codice_stringa
                        If macchina.proprietario IsNot Nothing Then
                            m.proprietario = macchina.proprietario
                        End If
                        ' nuovi campi
                        m.VIN = macchina.VIN
                        m.BTM_Serial = macchina.BTM_Serial
                        m.immagineGrande = macchina.immagineGrande
                        m.immaginePiccola = macchina.immaginePiccola

                        'aggiornare il contatto
                        If macchina.contatto IsNot Nothing Then
                            m.contatto.primaryKey.codice = macchina.contatto.primaryKey.codice
                            m.contatto.primaryKey.partitaIva = macchina.contatto.primaryKey.partitaIva
                        End If

                        '----------------------------------------------------------------
                        ' Scrittura parti Stazione Meteo
                        '----------------------------------------------------------------
                        m.Distinta_Installazione = macchina.Distinta_Installazione
                        m.Contratto_Installazione = macchina.Contratto_Installazione
                        m.Tipologia_Installazione = macchina.Tipologia_Installazione
                        m.Data_Inizio_Installazione = macchina.Data_Inizio_Installazione
                        m.Data_Fine_Installazione = macchina.Data_Fine_Installazione
                        m.Stato_Installazione = macchina.Stato_Installazione
                        m.Provincia_Istat_Installazione = macchina.Provincia_Istat_Installazione
                        m.Comune_Istat_Installazione = macchina.Comune_Istat_Installazione
                        m.Indirizzo_Installazione = macchina.Indirizzo_Installazione
                        m.Latitudine_Installazione = macchina.Latitudine_Installazione
                        m.Longitudine_Installazione = macchina.Longitudine_Installazione

                        objMacchinaBIZ.Scrivi_Macchina_Anagrafica(m, tipoOperazione, objParametri_Server, objParametri_Utenti)
                    Else
                        objMacchinaBIZ.Scrivi_Macchina_Anagrafica(macchina, tipoOperazione, objParametri_Server, objParametri_Utenti)
                    End If


                    'occorre caricare gli allegati eventualmente presenti
                    For Each testataDoc In macchina.Documenti

                        For Each doc In testataDoc.Allegati

                            Dim LeggiConfSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
                            Dim dt_Conf As DataTable = LeggiConfSiti.Leggi(6, "GestioneAllegati_Repository", "", "", objParametri_Server)
                            Dim Percorso As String = dt_Conf.Rows(0).Item("Valore")

                            'Determino se è una scadenza o un documento
                            Dim chkdocumento As Integer = 0
                            Dim elenco_des As String = ""

                            If Not IsNothing(doc.FileName) AndAlso doc.FileName <> "" Then
                                chkdocumento = 1 'Documento
                                elenco_des = Gias.Documento
                                If testataDoc.Data_Scadenza <> CostantiPersonalizzate.AGRODATAFINE Then
                                    chkdocumento = 2 'Hybrid
                                    elenco_des = Gias.DocumentoConScadenza
                                End If
                            Else
                                chkdocumento = 0 'Scadenza
                                elenco_des = Gias.Scadenza
                            End If

                            Dim objE As New AgronicaCoreScadenziario.Alert_Entita With {
                                .ChkDocumento = chkdocumento,
                                .Piva = macchina.partitaIva,
                                .PivaSuperUser = objParametri_Server.PivaSuperUser,
                                .Sa_Cod = macchina.centroPK.codice,
                                .TipoEntita_Cod = enum_TipoEntita.Macchina,
                                .Mac_Cod = macchina.codice
                            }

                            Dim strerr As String = alert_W.Scrivi(testataDoc.ID_Tipologia,
                                                                  objE,
                                                                  testataDoc.Data_Scadenza,
                                                                  testataDoc.Descrizione,
                                                                  doc.FileName,
                                                                  Percorso,
                                                                  CostantiPersonalizzate.AGRODATAINIZIO,
                                                                  CostantiPersonalizzate.AGRODATAFINE,
                                                                  objParametri_Server,
                                                                  testataDoc.Note,
                                                                  -1,
                                                                  EntitaxIndici:=Nothing,
                                                                  Ente_Des:="",
                                                                  Validazione_Flag:=0,
                                                                  Username_Upload:=objParametri_Server.UtenteUsername,
                                                                  Data_Upload:=Date.Now,
                                                                  File_Allegato:=doc.FileByte,
                                                                  Allegati_Documenti_Numero:="",
                                                                  SalvaAllegato:=0,
                                                                  ID_App:="",
                                                                  FileByteArrayQDC:=Nothing,
                                                                  ID_Area:=2,
                                                                  CompressoDaGIAS:=False,
                                                                  newAllegati_Documenti_Cod:=Nothing,
                                                                  Piva:=macchina.partitaIva)

                        Next

                    Next

                End If

                response.Add(macchina.codice)

            Next

            r.RispostaStringa = String.Join("|", response)
            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r

    End Function

    <WebMethod()>
    Public Function ScriviContatto(InData As Object) As rispostaStandard(Of AgronicaCoreModelsSTD.anagrafiche.RisorseUmane)

        Dim r As New rispostaStandard(Of AgronicaCoreModelsSTD.anagrafiche.RisorseUmane)

        Try

            Dim datiRequest As String = JsonConvert.SerializeObject(InData)
            Dim objRequest = JsonConvert.DeserializeObject(Of CoreWS_Generic(Of AgronicaCoreModelsSTD.anagrafiche.RisorseUmane))(datiRequest)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_utenti)
            Dim objContattiBIZ As New Contatti_W
            Dim risorsa As RisorseUmane = objRequest.InData
            Dim response As New List(Of String)

            Dim tipoOperazione = enum_TipoOperazioneDB.Lettura
            Dim cancellato As Boolean = risorsa.flag_cancellazione

            If risorsa.codice > 0 Then
                If cancellato Then
                    tipoOperazione = enum_TipoOperazioneDB.Cancellazione
                Else
                    tipoOperazione = enum_TipoOperazioneDB.Modifica
                End If
            ElseIf Not cancellato Then
                risorsa.codice = 0
                tipoOperazione = enum_TipoOperazioneDB.Scrittura
            End If

            If tipoOperazione <> enum_TipoOperazioneDB.Lettura Then

                objContattiBIZ.Valida_Contatto_APP(risorsa, objParametri_Server)
                objContattiBIZ.Scrivi_Contatto_APP(risorsa, tipoOperazione, objParametri_Server, objParametri_Utenti)

                ' salva patentino
                If tipoOperazione <> enum_TipoOperazioneDB.Cancellazione AndAlso Not IsNothing(risorsa.contatto.documenti) Then
                    Dim objAlertWrite As New AgronicaCoreScadenziario_BIZ.Alert_W
                    objAlertWrite.Scrivi_Patentino_APP(risorsa, objParametri_Server, objParametri_Utenti)
                End If

            End If

            r.RispostaStringa = risorsa
            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r

    End Function

    <WebMethod()>
    Public Function ScriviSquadra(InData As Object) As rispostaStandard(Of AgronicaCoreModelsSTD.anagrafiche.SquadraAttivita)

        Dim r As New rispostaStandard(Of AgronicaCoreModelsSTD.anagrafiche.SquadraAttivita)

        Try

            Dim datiRequest As String = JsonConvert.SerializeObject(InData)
            Dim objRequest = JsonConvert.DeserializeObject(Of CoreWS_Generic(Of AgronicaCoreModelsSTD.anagrafiche.SquadraAttivita))(datiRequest)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_utenti)
            Dim objCDGBIZ As New CDG_BIZ_W
            Dim squadra As AgronicaCoreModelsSTD.anagrafiche.SquadraAttivita = objRequest.InData
            Dim response As New List(Of String)

            Dim tipoOperazione = enum_TipoOperazioneDB.Lettura
            Dim cancellato As Boolean = squadra.flag_cancellazione
            If Not String.IsNullOrEmpty(squadra.codiceSquadra) AndAlso squadra.codiceSquadra <> "0" Then
                If cancellato Then
                    tipoOperazione = enum_TipoOperazioneDB.Cancellazione
                Else
                    tipoOperazione = enum_TipoOperazioneDB.Modifica
                End If
            ElseIf Not cancellato Then
                squadra.codiceSquadra = "0"
                tipoOperazione = enum_TipoOperazioneDB.Scrittura
            End If

            If tipoOperazione <> enum_TipoOperazioneDB.Lettura Then
                objCDGBIZ.Valida_Squadra_APP(squadra, objParametri_Server)
                objCDGBIZ.Scrivi_Squadra_APP(squadra, tipoOperazione, objParametri_Server, objParametri_Utenti)
            End If

            r.RispostaStringa = squadra
            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r

    End Function

End Class
