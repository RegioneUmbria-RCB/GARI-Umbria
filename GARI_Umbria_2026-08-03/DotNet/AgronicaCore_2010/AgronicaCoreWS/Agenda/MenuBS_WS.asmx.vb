Imports System.ComponentModel
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreModello.ParametriAgenda_Temp
Imports AgronicaCoreVarieBIZ
Imports Newtonsoft.Json
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreModello
Imports AgronicaCoreGestioneRichieste
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDTOStd.InData.Agenda
Imports AgronicaCoreUtentiDAL
Imports AgronicaCoreContabBIZ

' To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line.
<System.Web.Script.Services.ScriptService()>
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
Public Class MenuBS_WS
    Inherits System.Web.Services.WebService

    Public Shared LAV_COD_COPIABILI As Integer() = {LAVCOD_CONFUSIONE_SESSUALE, LAVCOD_DISORIENTAMENTO_SESSUALE, LAVCOD_DISTRIBUZIONE_INSETTI,
                                                    LAVCOD_TRATTAMENTO_ANTIPARASSITARIO, LAVCOD_TRATTAMENTO_FITOREGOLATORE, LAVCOD_GEODISINFESTAZIONE, LAVCOD_DISERBO,
                                                    LAVCOD_DISSECCAMENTO, LAVCOD_TRATTAMENTO_ANTIBUTTERATURA, LAVCOD_CONCIMAZIONE_FOGLIARE, LAVCOD_DISTRIBUZIONE_AMMENDANTI,
                                                    LAVCOD_DISTRIBUZIONE_CONCIME, LAVCOD_FERTIRRIGAZIONE, LAVCOD_SARCHIATURA_CONCIMAZIONE, LAVCOD_ANDANAMENTO, LAVCOD_ARATURA, LAVCOD_DEFOGLIAZIONE,
                                                    LAVCOD_ASPORTAZIONE_ORGANI_INFETTI, LAVCOD_ASSOLCATURA, LAVCOD_CARICO_MANUALE_FRUTTA, LAVCOD_CIMATURA, LAVCOD_DIRADAMENTO_MANUALE,
                                                    LAVCOD_DISSODAMENTO, LAVCOD_ERPICATURA, LAVCOD_ESTIRPATURA, LAVCOD_ESPIANTO, LAVCOD_FALCIACONDIZIONATURA, LAVCOD_FALCIATURA_ERBAI,
                                                    LAVCOD_FORMAZIONE_ARGINELLI, LAVCOD_FRANGIZOLLATURA, LAVCOD_FRESATURA, LAVCOD_IMBALLO_FIENO_ROTOLI, LAVCOD_INTERRAMENTO_PAGLIE,
                                                    LAVCOD_LAVORAZIONE_TRA_FILA, LAVCOD_LAVORAZIONE_SU_FILA, LAVCOD_LEGATURA, LAVCOD_LIVELLAMENTO, LAVCOD_MANUTENZIONE_ARGINI,
                                                    LAVCOD_MESSA_DIMORA_PIANTE, LAVCOD_MIETITREBBIATURA, LAVCOD_MINIMUM_TILLAGE, LAVCOD_PACCIAMATURA, LAVCOD_POTATURA_SECCA, LAVCOD_POTATURA_VERDE,
                                                    LAVCOD_PRESSATURA, LAVCOD_RACCOLTA_LEGNA_POTATURA, LAVCOD_RACCOLTA_MANUALE, LAVCOD_RACCOLTA_MECCANICA, LAVCOD_RANGHINATURA, LAVCOD_RINCALZATURA,
                                                    LAVCOD_RIPPATURA, LAVCOD_RIPUNTATURA, LAVCOD_RIVOLTAMENTO_FORAGGIO, LAVCOD_RULLATURA, LAVCOD_SARCHIATURA, LAVCOD_SCARIFICATURA, LAVCOD_SCASSO,
                                                    LAVCOD_TRINCIATURA, LAVCOD_VANGATURA, LAVCOD_ZAPPATURA, LAVCOD_GEBIATURA, LAVCOD_ROMPICROSTA, LAVCOD_LAVORAZIONE_CONBINATA,
                                                    LAVCOD_ERPICATURA_ROTANTE, LAVCOD_INTERVENTO_ANTIBRINA, LAVCOD_ALTRE_OPERAZIONI, LAVCOD_STRIGLIATURA, LAVCOD_PIRODISERBO, LAVCOD_ABBATTIMENTOIMPIANTI, LAVCOD_CONFUSIONE_DISORIENTAMENTO_SESSUALE,
                                                    LAVCOD_PASCOLAMENTO_PROPRIO, LAVCOD_PASCOLAMENTO_TERZI}

    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function CaricaOperazioni(ByVal filtro As String, piva As String, objP_super_server As String, objP_server As String, objP_utenti As String) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_super_server)
        Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
        Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)

        Try

            HttpContext.Current.Session("ASG_objParametri_Super_Server") = objParametri_Super_Server
            HttpContext.Current.Session("ASG_objParametri_Server") = objParametri_Server
            HttpContext.Current.Session("ASG_objParametri_Utenti") = objParametri_Utenti

            Dim Mov_Biz As AgronicaCoreContabBIZ.Movimenti_R = New AgronicaCoreContabBIZ.Movimenti_R()
            r = Mov_Biz.CaricaOperazioni(filtro, piva, objParametri_Server, objParametri_Utenti, True)

            Dim cf As New AgronicaCoreVarieDAL.Configurazione_Siti_R
            Dim lavCodRicettabili As List(Of String) = cf.Ottiene_Lav_Cod_Ricettabili(objParametri_Server, isFromNG:=True)


            r.ParametroDue_stringa = JsonConvert.SerializeObject(lavCodRicettabili)
            r.ParametroDue = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try
        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function CaricaOperazioni_NG(InData As CoreWS_Generic(Of CaricaOperazioni)) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_super_server)
        Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
        Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

        Try

            HttpContext.Current.Session("ASG_objParametri_Super_Server") = objParametri_Super_Server
            HttpContext.Current.Session("ASG_objParametri_Server") = objParametri_Server
            HttpContext.Current.Session("ASG_objParametri_Utenti") = objParametri_Utenti

            Dim leggiLingua As New AgronicaCoreUtentiDAL.Lingue_Read
            Dim dtLingua As DataTable = leggiLingua.Leggi_datoCODGIAS(objParametri_Server.Lingua_Cod, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Utenti)
            Dim linguaCodiceISO As String = dtLingua.Rows(0)("CodiceISO")
            Threading.Thread.CurrentThread.CurrentUICulture = New Globalization.CultureInfo(linguaCodiceISO)

            Dim Mov_Biz As AgronicaCoreContabBIZ.Movimenti_R = New AgronicaCoreContabBIZ.Movimenti_R()
            r = Mov_Biz.CaricaOperazioni(InData.InData.filtro, InData.InData.piva, objParametri_Server, objParametri_Utenti, True)

            Dim cf As New AgronicaCoreVarieDAL.Configurazione_Siti_R
            Dim lavCodRicettabili = cf.Ottiene_Lav_Cod_Ricettabili(objParametri_Server, isFromNG:=True)

            r.ParametroDue_stringa = JsonConvert.SerializeObject(lavCodRicettabili)
            r.ParametroDue = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try
        Return r

    End Function
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function CaricaRicette(ByVal filtri As String, piva As String, objParam_server As String, objParam_utenti As String) As RispostaStandard
        Try
            Dim objP_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objParam_server)
            Dim objP_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objParam_utenti)

            Dim bus As New AgronicaCoreContabBIZ.MovimentiNG(objP_Server, objP_Utenti) ' bussiness layer
            Return ProvideRispostaStandardFrom(bus.CaricaRicette(filtri, piva))
        Catch ex As Exception
            Return MessaggioErroreFrom(ex)
        End Try
    End Function
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function CaricaRicette_NG(InData As CoreWS_Generic(Of CaricaRicette)) As RispostaStandard
        Try
            Dim objP_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objP_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            Dim leggiLingua As New Lingue_Read
            Dim dtLingua As DataTable = leggiLingua.Leggi_datoCODGIAS(objP_Server.Lingua_Cod,
                                                                  AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                  "", "", objP_Server)

            Dim linguaCodiceISO As String = dtLingua.Rows(0)("CodiceISO")
            Threading.Thread.CurrentThread.CurrentUICulture = New System.Globalization.CultureInfo(linguaCodiceISO)

            Dim bus As New AgronicaCoreContabBIZ.MovimentiNG(objP_Server, objP_Utenti) ' bussiness layer
            Return ProvideRispostaStandardFrom(bus.CaricaRicette(InData.InData.filtri, InData.InData.piva))
        Catch ex As Exception
            Return MessaggioErroreFrom(ex)
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function CaricaZoo_NG(InData As CoreWS_Generic(Of CaricaZoo)) As RispostaStandard
        Try
            Dim objP_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objP_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            Dim gestioneRedirect = New AgronicaCoreGestioneRichieste.GiasNG_Redirect

            Dim VariabiliInSessione = gestioneRedirect.GetVariabiliInSessioneNG(InData.InData.variabiliInSessione_NG)

            gestioneRedirect.ImpostaVariabiliInSessione(VariabiliInSessione, objP_Server, objP_Utenti)

            Dim bus As New AgronicaCoreContabBIZ.MovimentiNG(objP_Server, objP_Utenti) ' bussiness layer
            Return ProvideRispostaStandardFrom(bus.CaricaZoo(InData.InData.filtro, InData.InData.piva))
        Catch ex As Exception
            Return MessaggioErroreFrom(ex)
        End Try
    End Function
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function CopiaOperazioneSingola(datiDto As AgronicaCoreModello.CopiaOperazioniDto, objParam_server As String, objParam_utenti As String) As RispostaStandard
        Try
            Dim r As New RispostaStandard

            Dim objP_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objParam_server)
            Dim objP_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objParam_utenti)

            datiDto.LAV_COD_COPIABILI = LAV_COD_COPIABILI
            Dim risposta As CopiaOperazioneResult = Utility_Operazioni.CopiaOperazioneSingola(datiDto, objP_Server, objP_Utenti) ' bussiness layer

            Return ProvideRispostaStandardFrom(risposta)
        Catch ex As Exception
            Return MessaggioErroreFrom(ex)
        End Try
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function CopiaOperazioneSingola_NG(InData As CoreWS_Generic(Of AgronicaCoreModello.CopiaOperazioniDto)) As RispostaStandard
        Try
            Dim r As New RispostaStandard

            Dim objP_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objP_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            InData.InData.LAV_COD_COPIABILI = LAV_COD_COPIABILI
            Dim risposta As CopiaOperazioneResult = Utility_Operazioni.CopiaOperazioneSingola(InData.InData, objP_Server, objP_Utenti) ' bussiness layer

            Return ProvideRispostaStandardFrom(risposta)
        Catch ex As Exception
            Return MessaggioErroreFrom(ex)
        End Try
    End Function

    <Script.Services.ScriptMethod()>
    <WebMethod()>
    Public Function crea_ricetta_NG(InData As CoreWS_Generic(Of crea_ricetta)) As RispostaStandard

        Dim r As New RispostaStandard


        Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
        Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

        'Dim objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))
        'If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
        '    r.Sessione = False
        '    Return r
        'End If

        ' VAnni: 25/2/2020: Verifica Sessione..? Ok
        ' Lingua.Gias_InizializzaCultura_DaSession()

        Try
            Dim objUtenti As New AgronicaCoreUtentiDAL.Utenti_CodiciGiasPRO_R
            Dim progressivoGias As Integer = objUtenti.ProgressivoGias_from_Superuser(objParametri_Utenti)

            Dim RicetteOpW As New AgronicaCoreContabBIZ.Ricette_Operazioni_W
            r = RicetteOpW.CreaRicetta(InData.InData.data_inizio, InData.InData.data_fine,
                                       InData.InData.id_agenda_checked, InData.InData.id_agenda,
                                       InData.InData.piva, InData.InData.sa_cod, InData.InData.veg_cod,
                                       InData.InData.ricetta_des, InData.InData.ricetta_numero,
                                       InData.InData.nota_des, progressivoGias, objParametri_Server, objParametri_Utenti)

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r

    End Function
    <Script.Services.ScriptMethod()>
    <WebMethod()>
    Public Function crea_ricetta(ByVal data_inizio As String, ByVal data_fine As String,
                                        ByVal id_agenda_checked As String, ByVal id_agenda As String,
                                        ByVal piva As String, ByVal sa_cod As String, ByVal veg_cod As String,
                                        ByVal ricetta_des As String, ByVal ricetta_numero As String,
                                        ByVal nota_des As String,
                                        objP_utenti As String, objP_server As String) As RispostaStandard

        Dim r As New RispostaStandard


        Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
        Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)

        'Dim objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))
        'If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
        '    r.Sessione = False
        '    Return r
        'End If

        ' VAnni: 25/2/2020: Verifica Sessione..? Ok
        ' Lingua.Gias_InizializzaCultura_DaSession()

        Try
            Dim objUtenti As New AgronicaCoreUtentiDAL.Utenti_CodiciGiasPRO_R
            Dim progressivoGias As Integer = objUtenti.ProgressivoGias_from_Superuser(objParametri_Utenti)

            Dim RicetteOpW As New AgronicaCoreContabBIZ.Ricette_Operazioni_W
            r = RicetteOpW.CreaRicetta(data_inizio, data_fine,
                                       id_agenda_checked, id_agenda,
                                       piva, sa_cod, veg_cod,
                                       ricetta_des, ricetta_numero,
                                       nota_des, progressivoGias, objParametri_Server, objParametri_Utenti)

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r

    End Function
    '   <Script.Services.ScriptMethod()>
    '  <WebMethod(EnableSession:=True)>
    ' Public Shared Function copia_operazione_singola(ByVal data As String, ByVal id_agenda_checked As String, ByVal id_agenda As String, ByVal lav_cod_checked As String, ByVal lav_cod As String, ByVal piva As String, ByVal sa_cod As String) As RispostaStandard

    'Dim r As New RispostaStandard

    '    Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
    '    If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
    '        r.Sessione = False
    '        Return r
    '    End If


    '    Lingua.Gias_InizializzaCultura_DaSession()

    '    Dim objParametri_Utenti = New AgronicaCoreDataProvider.AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Utenti"))


    '    Dim id_agenda_array As String() = id_agenda_checked.Split(",")
    '    Dim lav_cod_array As String() = lav_cod_checked.Split(",")

    '    Dim id_agenda_copiabili_array As New List(Of String)
    '    Dim lav_cod_copiabili_array As New List(Of String)

    '    'Creo un nuovo elenco di operazioni d'agenda copiabili
    '    For i = 0 To id_agenda_array.Length - 1

    '        'Se è il -1, lo includo
    '        If id_agenda_array(i) = "-1" Then
    '            id_agenda_copiabili_array.Add(id_agenda_array(i))
    '            lav_cod_copiabili_array.Add(lav_cod_array(i))
    '            Continue For
    '        End If

    '        'Controllo che il lav_cod sia tra quelli copiabili
    '        If LAV_COD_COPIABILI.Contains(lav_cod_array(i)) = False Then
    '            Continue For
    '        End If

    '        'Controllo se l'utente ha permessi di scrittura sull'operazione in oggetto

    '        'permessi op contabili e magazzino
    '        Dim permesso As Boolean = AgronicaCoreModello.Utility_Operazioni.PermessiOpContabiliEMagazzino(lav_cod, enum_TipoOperazioneDB.Copia,
    '                                                                                                       objParametri_Server, objParametri_Utenti,
    '                                                                                                       HttpContext.Current.Session)
    '        If permesso = False Then
    '            Continue For
    '        End If

    '        'Verifico Permessi per operazioni colturali
    '        Dim op_R As New AgronicaCoreMetaSchemaDAL.Operazioni_R
    '        Dim gru_op As Integer = op_R.Gru_Op_from_LavorazioneCod(lav_cod, objParametri_Server)

    '        If Not {6, 10, 20}.Contains(gru_op) Then
    '            Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
    '            permesso = objPermessi.Controlla_Permessi_Utente(
    '                       HttpContext.Current.Session("ASG_Utente_Username"),
    '                       HttpContext.Current.Session("ASG_IdServizio"),
    '                       enum_Security_Attivita.Agenda_AccessoMenu,
    '                       enum_Security_Operazione.Modifica,
    '                       Date.Now, "", objParametri_Utenti)
    '        End If

    '        If permesso = False Then
    '            Continue For
    '        End If

    '        'Aggiungo l'elemento tra i copiabili
    '        id_agenda_copiabili_array.Add(id_agenda_array(i))
    '        lav_cod_copiabili_array.Add(lav_cod_array(i))
    '    Next

    '    'Se non ci sono operazioni d'agenda copiabili, ritorno un errore
    '    If id_agenda_copiabili_array.Count = 0 OrElse (id_agenda_copiabili_array.Count = 1 AndAlso id_agenda_copiabili_array(0) = "-1") Then
    '        r.RispostaOK = False
    '        r.Errore = My.Resources.NonÈPossibileCopiareQuestOperazionePlurale
    '        Return r
    '    End If

    '    'Se c'è almeno un operazione d'agenda copiabile, allora vado in duplicazione con i soli id copiabili
    '    id_agenda_checked = String.Join(",", id_agenda_copiabili_array)

    '    Dim objImpre As New AgronicaCoreAnagrafeDAL.Imprese_Read
    '    Dim objCentriAziendali As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read
    '    Dim objParametriAgenda As New ParametriAgenda

    '    objParametriAgenda.Data = data
    '    objParametriAgenda.Id_Agenda = id_agenda_copiabili_array(0) 'id_agenda
    '    objParametriAgenda.Lav_Cod = lav_cod_copiabili_array(0) 'lav_cod
    '    objParametriAgenda.Piva = piva
    '    objParametriAgenda.Sa_Cod = sa_cod
    '    objParametriAgenda.RagSoc = objImpre.RagSoc_from_Piva(piva, objParametri_Server)
    '    objParametriAgenda.SaNome = objCentriAziendali.SaNome_from_SaCod(piva, sa_cod, objParametri_Server)

    '    r.RispostaOK = True
    '    r.RispostaStringa = "../Operazioni/DuplicaOperazione.aspx?id=" & id_agenda_checked


    '    Return r

    'End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function ricetta_numero_default(ByVal piva As String, ByVal data_operazione As String, objP_server As String) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
        Dim data As DateTime = Convert.ToDateTime(data_operazione)

        Dim r_R As New AgronicaCoreContabBIZ.Ricette_R
        Dim ricetta_numero As String = r_R.Genera_Nuovo_Nome_Ricetta(piva, data, objParametri_Server)

        r.RispostaOK = True
        r.RispostaStringa = ricetta_numero

        Return r

    End Function
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function ricetta_numero_default_NG(InData As CoreWS_Generic(Of ricetta_numero_default)) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
        ' Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)

        ' VAnni: 25/2/2020: Verifica Sessione..? Ok
        ' Lingua.Gias_InizializzaCultura_DaSession()
        Dim data As DateTime = Date.Now
        If IsDate(InData.InData.data) AndAlso InData.InData.data <> Date.MinValue Then
            data = InData.InData.data
        End If
        Dim objR As New AgronicaCoreContabBIZ.Ricette_R
        Dim ricetta_numero As String = objR.Genera_Nuovo_Numero_Ricetta(InData.InData.piva, data, objParametri_Server)

        r.RispostaOK = True
        r.RispostaStringa = ricetta_numero

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Ricette_Copia(ricetta_cod As Integer, objParam_server As String, objParam_utenti As String) As RispostaStandard
        ' TODO Razvan. Da aggiungere Gias_InizializzaCultura_DaSession appena mettiamo in piedi un metodo
        '              di gestire le traduzioni in Angular. In questo momento non esiste tale modalità.

        Try
            Dim objP_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objParam_server)
            Dim objP_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objParam_utenti)

            Dim r_W As New AgronicaCoreContabBIZ.Ricette_W
            Dim esito As Boolean = r_W.Ricetta_Copia(ricetta_cod, 0, GetProgressivoGias(objP_Utenti), objP_Server)

            Return ProvideRispostaStandardFrom(esito)
        Catch ex As Exception
            Return MessaggioErroreFrom(ex)
        End Try
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Ricette_Copia_NG(InData As CoreWS_Generic(Of AgronicaCoreDTOStd.InData.Agenda.Ricetta_Operazione)) As RispostaStandard
        ' TODO Razvan. Da aggiungere Gias_InizializzaCultura_DaSession appena mettiamo in piedi un metodo
        '              di gestire le traduzioni in Angular. In questo momento non esiste tale modalità.

        Dim ricetta_cod As Integer = InData.InData.ricetta_cod



        Try
            Dim objP_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objP_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            Dim r_W As New AgronicaCoreContabBIZ.Ricette_W
            Dim esito As Boolean = r_W.Ricetta_Copia(ricetta_cod, 0, GetProgressivoGias(objP_Utenti), objP_Server)

            Return ProvideRispostaStandardFrom(esito)
        Catch ex As Exception
            Return MessaggioErroreFrom(ex)
        End Try
    End Function
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Ricette_Cancella(ricetta_cod As Integer, ricetta_operazione_cod As Integer, in_uso As Integer, objParam_server As String, objParam_utenti As String) As RispostaStandard


        ' TODO Razvan. Da aggiungere Gias_InizializzaCultura_DaSession appena mettiamo in piedi un metodo
        '              di gestire le traduzioni in Angular. In questo momento non esiste tale modalità.

        Try
            Dim objP_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objParam_server)
            Dim objP_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objParam_utenti)


            If ricetta_cod <= 0 Then
                Throw New Exception(My.Resources.RicettaCodNonValorizzato)
            End If

            If ricetta_operazione_cod <= 0 Then
                Throw New Exception(My.Resources.RicettaOperazioneCodNonValorizzato)
            End If

            If in_uso <> 0 Then
                Throw New Exception(My.Resources.ImpossibileCancellareRicettaInUso)
            End If

            Dim ricette As New AgronicaCoreContabBIZ.Ricette_Operazioni_W
            Dim errMsg As String = ricette.Ricetta_Operazione_Cancella_ESeUnicaAncheLaRicettaPadre(ricetta_cod, ricetta_operazione_cod, objP_Server, objParametri_Utenti:=objP_Utenti)

            Return ProvideRispostaStandardFrom(errMsg)
        Catch ex As Exception
            Return MessaggioErroreFrom(ex)
        End Try
    End Function

    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Ricette_Cancella_Multi(InData As CoreWS_Generic(Of AgronicaCoreDTOStd.InData.Agenda.Elimina_Ricetta_Brogliaccio)) As RispostaStandard
        Dim r As New RispostaStandard
        Dim errMsg As String = ""

        Dim objParametri_Super_Server As AgronicaCoreParametri
        Dim objParametri_Server As AgronicaCoreParametri
        Dim objParametri_Utenti As AgronicaCoreParametri
        Dim ricette As List(Of AgronicaCoreDTOStd.InData.Agenda.Ricetta_Operazione)

        Dim objRicette As New AgronicaCoreContabBIZ.Ricette_Operazioni_W

        Try
            objParametri_Super_Server = Utility.convertStringtoOBJparametri(InData.objP.objP_super_server)
            objParametri_Server = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            objParametri_Utenti = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            If objParametri_Server Is Nothing Then
                r.Errore = "objP_server non valorizzato"
                Return r
            End If

            If objParametri_Utenti Is Nothing Then
                r.Errore = "objP_utenti non valorizzato"
                Return r
            End If

            If objParametri_Super_Server Is Nothing Then
                r.Errore = "objP_super_server non valorizzato"
                Return r
            End If

            Dim gestioneRedirect = New AgronicaCoreGestioneRichieste.GiasNG_Redirect

            Dim VariabiliInSessione = gestioneRedirect.GetVariabiliInSessioneNG(InData.InData.variabiliInSessione_NG)

            ricette = InData.InData.ricette

            gestioneRedirect.ImpostaVariabiliInSessione(VariabiliInSessione, objParametri_Server, objParametri_Utenti)

        Catch ex As Exception
            ' Non siamp riusciti a inizializzare i dati
            Return MessaggioErroreFrom(ex)
        End Try

        Try
            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(True,
                                                                                    True,
                                                                                    objParametri_Server)


            For Each ricetta In ricette
                If Not ricetta.in_uso Then
                    errMsg = objRicette.Ricetta_Operazione_Cancella_ESeUnicaAncheLaRicettaPadre(
                    ricetta.ricetta_cod, ricetta.ricetta_operazione_cod,
                    objParametri_Server, False, objParametri_Utenti:=objParametri_Utenti)

                    If errMsg <> "" Then
                        AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)
                        Exit For
                    End If
                End If
            Next

            If Not IsNothing(objParametri_Server.objTransazione) Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(True, objParametri_Server)
            End If

        Catch ex As Exception
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)
            r = MessaggioErroreFrom(ex)
        Finally
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(True, objParametri_Server)
        End Try

        r.RispostaOK = If(errMsg <> "", False, True)
        r.RispostaStringa = errMsg
        Return r
    End Function

    Public Function GetProgressivoGias(objParametri_Utenti As AgronicaCoreParametri)
        Dim objUtenti As New AgronicaCoreUtentiDAL.Utenti_CodiciGiasPRO_R
        Dim progressivoGias As Integer = objUtenti.ProgressivoGias_from_Superuser(objParametri_Utenti)
        Return progressivoGias
    End Function



    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Function PaginaLinkGestioneMagazziniQueryStringDto(lav_cod As Integer, piva As String, sa_cod As Integer) As RispostaStandard
        Try
            Dim biz As New Utility_Operazioni
            Dim dto = biz.PaginaLinkGestioneMagazziniQueryStringDto(lav_cod, piva, sa_cod)
            Return ProvideRispostaStandardFrom(dto)
        Catch ex As Exception
            Return MessaggioErroreFrom(ex)
        End Try

    End Function
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Function PaginaLinkGestioneMagazziniQueryStringDto_NG(InData As CoreWS_Generic(Of AgronicaCoreDTOStd.InData.Agenda.PaginaLinkGestioneMagazziniQueryStringDto)) As RispostaStandard
        Try
            Dim biz As New Utility_Operazioni
            Dim dto = biz.PaginaLinkGestioneMagazziniQueryStringDto(InData.InData.lav_cod, InData.InData.piva, InData.InData.sa_cod)
            Return ProvideRispostaStandardFrom(dto)
        Catch ex As Exception
            Return MessaggioErroreFrom(ex)
        End Try

    End Function

    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Function infomodifica_operazione_singola_new(InData As CoreWS_Generic(Of AgronicaCoreDTOStd.InData.Agenda.InfomodificaOperazioneSingola)) As RispostaStandard
        Dim r As New RispostaStandard

        Try
            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            If objParametri_Server Is Nothing Then
                r.Errore = "objP_server non valorizzato"
                Return r
            End If

            If objParametri_Utenti Is Nothing Then
                r.Errore = "objP_utenti non valorizzato"
                Return r
            End If

            If objParametri_Super_Server Is Nothing Then
                r.Errore = "objP_super_server non valorizzato"
                Return r
            End If

            Dim leggiLingua As New Lingue_Read
            Dim dtLingua As DataTable = leggiLingua.Leggi_datoCODGIAS(objParametri_Server.Lingua_Cod,
                                                                  AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                  "", "", objParametri_Server)

            Dim linguaCodiceISO As String = dtLingua.Rows(0)("CodiceISO")
            Threading.Thread.CurrentThread.CurrentUICulture = New System.Globalization.CultureInfo(linguaCodiceISO)

            Dim gestioneRedirect = New AgronicaCoreGestioneRichieste.GiasNG_Redirect

            Dim VariabiliInSessione = gestioneRedirect.GetVariabiliInSessioneNG(InData.InData.variabiliInSessione_NG)

            gestioneRedirect.ImpostaVariabiliInSessione(VariabiliInSessione, objParametri_Server, objParametri_Utenti)

            Return Utility_Operazioni.InfoModificaOperazioneSingola(InData.InData.tipo, InData.InData.dataOp,
                                                                    InData.InData.id_agenda, InData.InData.lav_cod,
                                                                    InData.InData.blocco_flag, InData.InData.veg_cod,
                                                                    objParametri_Server, objParametri_Utenti,
                                                                    True, InData.InData.piva)
        Catch ex As Exception
            Return MessaggioErroreFrom(ex)
        End Try
        Return r
    End Function

    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Function Redirect_In_Base_Al_Lav_Cod(InData As CoreWS_Generic(Of LeggiLink_Operazione)) As RispostaStandard

        Dim r As New RispostaStandard

        Try

            Dim objParametri_Super_Server As AgronicaCoreParametri
            Dim objParametri_Server As AgronicaCoreParametri
            Dim objParametri_Utenti As AgronicaCoreParametri


            Dim strObjP As String = JsonConvert.SerializeObject(InData.objP)
            Dim objP As CoreWS_GenericObjP = JsonConvert.DeserializeObject(Of CoreWS_GenericObjP)(strObjP)

            objParametri_Super_Server = Utility.convertStringtoOBJparametri(objP.objP_super_server)
            objParametri_Server = Utility.convertStringtoOBJparametri(objP.objP_server)
            objParametri_Utenti = Utility.convertStringtoOBJparametri(objP.objP_utenti)

            Dim gestioneRedirect = New AgronicaCoreGestioneRichieste.GiasNG_Redirect

            Dim VariabiliInSessione = gestioneRedirect.GetVariabiliInSessioneNG(InData.InData.variabiliInSessione_NG)

            gestioneRedirect.ImpostaVariabiliInSessione(VariabiliInSessione, objParametri_Server, objParametri_Utenti)


            Dim objUtility_Operazioni As New AgronicaCoreModello.Utility_Operazioni

            r = objUtility_Operazioni.getLinkPaginaforQdC_Angular(InData.InData, objParametri_Server, objParametri_Utenti)


        Catch ex As Exception
            Return MessaggioErroreFrom(ex)
        End Try

        Return r

    End Function

    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Function elimina_operazione_multipla_NG(InData As CoreWS_Generic(Of elimina_operazione_multipla)) As RispostaStandard

        Try
            Dim objP_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objP_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)


            Dim gestioneRedirect = New AgronicaCoreGestioneRichieste.GiasNG_Redirect

            Dim VariabiliInSessione = gestioneRedirect.GetVariabiliInSessioneNG(InData.InData.variabiliInSessione_NG)

            gestioneRedirect.ImpostaVariabiliInSessione(VariabiliInSessione, objP_Server, objP_Utenti)

            Dim messaggio = Utility_Operazioni.elimina_operazione_multipla(InData.InData.strChiaviComposite, InData.InData.proseguiInCasoDiAlert, objP_Server, objP_Utenti)

            Return ProvideRispostaStandardFrom(messaggio)
        Catch ex As Exception
            Return MessaggioErroreFrom(ex)
        End Try

    End Function
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Function Ricette_VerificaSeCostiCollegatiECancella_NG(InData As CoreWS_Generic(Of Elimina_Ricetta_Brogliaccio)) As RispostaStandard

        Dim r As New RispostaStandard
        Dim errMsg As String = ""

        Dim objParametri_Server As AgronicaCoreParametri
        Dim objParametri_Utenti As AgronicaCoreParametri

        Dim objRicette As New AgronicaCoreContabBIZ.Ricette_Operazioni_W

        Try
            objParametri_Server = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            objParametri_Utenti = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            If objParametri_Server Is Nothing Then
                r.Errore = "objP_server non valorizzato"
                Return r
            End If

            If objParametri_Utenti Is Nothing Then
                r.Errore = "objP_utenti non valorizzato"
                Return r
            End If

            Dim gestioneRedirect = New AgronicaCoreGestioneRichieste.GiasNG_Redirect

            Dim VariabiliInSessione = gestioneRedirect.GetVariabiliInSessioneNG(InData.InData.variabiliInSessione_NG)

            gestioneRedirect.ImpostaVariabiliInSessione(VariabiliInSessione, objParametri_Server, objParametri_Utenti)
        Catch ex As Exception
            ' Non siamp riusciti a inizializzare i dati
            Return MessaggioErroreFrom(ex)
        End Try

        Try
            'Se la connessione è chiusa la apro e se la transazione è chiusa la inizio
            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(True,
                                                                                    True,
                                                                                    objParametri_Server)


            For Each ricetta In InData.InData.ricette
                Dim risposta As RispostaStandard = Utility_Operazioni.Ricette_VerificaSeCostiCollegatiECancella(ricetta.ricetta_cod, ricetta.ricetta_operazione_cod, ricetta.in_uso, ricetta.app_ricetta_operazione_id, usaTransazione:=False)

                If Not IsNothing(risposta) AndAlso ((risposta.RispostaOK = False AndAlso risposta.Errore <> "") OrElse (risposta.RispostaOK = True AndAlso risposta.RispostaStringa <> "")) Then

                    'Eccezione/Errore
                    If risposta.RispostaOK = False AndAlso risposta.Errore <> "" Then
                        Throw New Exception(risposta.Errore)
                    Else
                        'Ci sono dei costi collegati
                        If risposta.RispostaOK = True AndAlso risposta.RispostaStringa <> "" Then

                            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)
                            errMsg = risposta.RispostaStringa
                            Exit For
                        End If
                    End If

                End If
            Next


            If Not IsNothing(objParametri_Server.objTransazione) Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(True, objParametri_Server)
            End If

            r.RispostaOK = True
            r.RispostaStringa = errMsg

        Catch ex As Exception
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)
            r = MessaggioErroreFrom(ex)
        Finally
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(True, objParametri_Server)
        End Try

        Return r

    End Function

    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Function Ricette_CancellaRicettaCancellaCosti(InData As CoreWS_Generic(Of Elimina_Ricetta_Brogliaccio)) As RispostaStandard
        Dim r As New RispostaStandard
        Dim errMsg As String = ""

        Dim objParametri_Server As AgronicaCoreParametri
        Dim objParametri_Utenti As AgronicaCoreParametri

        Dim objRicette As New AgronicaCoreContabBIZ.Ricette_Operazioni_W

        Try
            objParametri_Server = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            objParametri_Utenti = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            If objParametri_Server Is Nothing Then
                r.Errore = "objP_server non valorizzato"
                Return r
            End If

            If objParametri_Utenti Is Nothing Then
                r.Errore = "objP_utenti non valorizzato"
                Return r
            End If

            Dim gestioneRedirect = New AgronicaCoreGestioneRichieste.GiasNG_Redirect

            Dim VariabiliInSessione = gestioneRedirect.GetVariabiliInSessioneNG(InData.InData.variabiliInSessione_NG)

            gestioneRedirect.ImpostaVariabiliInSessione(VariabiliInSessione, objParametri_Server, objParametri_Utenti)
        Catch ex As Exception
            ' Non siamp riusciti a inizializzare i dati
            Return MessaggioErroreFrom(ex)
        End Try

        Try
            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(True,
                                                                                    True,
                                                                                    objParametri_Server)


            For Each ricetta In InData.InData.ricette
                'La cancellazione dei costi in realtà prevede il fatto di ignorarli e lasciare degli zombie non collegati
                Dim risposta = Utility_Operazioni.Ricette_Cancella(ricetta.ricetta_cod, ricetta.ricetta_operazione_cod, ricetta.in_uso, True, usaTransazione:=False)

                If Not IsNothing(risposta) AndAlso risposta.RispostaOK = False AndAlso risposta.Errore <> "" Then
                    Throw New Exception(risposta.Errore)
                End If
            Next

            If Not IsNothing(objParametri_Server.objTransazione) Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(True, objParametri_Server)
            End If

            r.RispostaOK = True
            r.RispostaStringa = errMsg

        Catch ex As Exception
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)
            r = MessaggioErroreFrom(ex)
        Finally
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(True, objParametri_Server)
        End Try

        Return r

    End Function
    <WebMethod(EnableSession:=True)>
    Public Function CaricaImpostazioniApp(objParam_server As String, objParam_utenti As String) As RispostaStandard
        Try
            Dim objP_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objParam_server)
            Dim objP_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objParam_utenti)

            Dim objAppHelper As New AppHelper
            Dim objUtentiImpostazioni As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
            Dim SincroDatiApp As Boolean = False
            Dim CaricaDatiApp = objAppHelper.Leggi_CaricaDati_APP(SincroDatiApp, objP_Server)
            Dim ImportaSoloAziendaSelezionata As Boolean = True

            ' se presente configurazione per GIASAPP importa i dati di tutte le aziende
            If objUtentiImpostazioni.ImpostazioneValore1_from_ImpostazioneCod(enum_Impostazioni_Utenti.SUPERUSER_GiasAPP_NUOVO_INTERVENTO, objP_Utenti, 2) = "1" Then
                ImportaSoloAziendaSelezionata = False
            End If

            Return ProvideRispostaStandardFrom(New With {
                .ImportaSoloAziendaSelezionata = ImportaSoloAziendaSelezionata,
                .SincroDatiApp = SincroDatiApp
            })
        Catch ex As Exception
            Return MessaggioErroreFrom(ex)
        End Try

    End Function
    <WebMethod(EnableSession:=True)>
    Public Function CaricaImpostazioniApp_NG(InData As CoreWS_Generic(Of String)) As RispostaStandard
        Try
            Dim objParam_server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParam_utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            Dim objAppHelper As New AppHelper
            Dim objUtentiImpostazioni As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
            Dim SincroDatiApp As Boolean = False
            Dim CaricaDatiApp = objAppHelper.Leggi_CaricaDati_APP(SincroDatiApp, objParam_server)
            Dim ImportaSoloAziendaSelezionata As Boolean = True

            ' se presente configurazione per GIASAPP importa i dati di tutte le aziende
            If objUtentiImpostazioni.ImpostazioneValore1_from_ImpostazioneCod(enum_Impostazioni_Utenti.SUPERUSER_GiasAPP_NUOVO_INTERVENTO, objParam_utenti, 2) = "1" Then
                ImportaSoloAziendaSelezionata = False
            End If

            Return ProvideRispostaStandardFrom(New With {
                .ImportaSoloAziendaSelezionata = ImportaSoloAziendaSelezionata,
                .SincroDatiApp = SincroDatiApp
            })
        Catch ex As Exception
            Return MessaggioErroreFrom(ex)
        End Try

    End Function

    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Function Ricette_CancellaRicettaConvertiCosti(InData As CoreWS_Generic(Of Elimina_Ricetta_Brogliaccio)) As RispostaStandard

        Dim r As New RispostaStandard
        Dim errMsg As String = ""

        Dim objParametri_Server As AgronicaCoreParametri
        Dim objParametri_Utenti As AgronicaCoreParametri

        Dim objRicette As New AgronicaCoreContabBIZ.Ricette_Operazioni_W

        Try
            objParametri_Server = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            objParametri_Utenti = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            If objParametri_Server Is Nothing Then
                r.Errore = "objP_server non valorizzato"
                Return r
            End If

            If objParametri_Utenti Is Nothing Then
                r.Errore = "objP_utenti non valorizzato"
                Return r
            End If

            Dim gestioneRedirect = New AgronicaCoreGestioneRichieste.GiasNG_Redirect

            Dim VariabiliInSessione = gestioneRedirect.GetVariabiliInSessioneNG(InData.InData.variabiliInSessione_NG)

            gestioneRedirect.ImpostaVariabiliInSessione(VariabiliInSessione, objParametri_Server, objParametri_Utenti)

        Catch ex As Exception
            ' Non siamp riusciti a inizializzare i dati
            Return MessaggioErroreFrom(ex)
        End Try

        Try
            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(True,
                                                                                    True,
                                                                                    objParametri_Server)


            For Each ricetta In InData.InData.ricette
                'La cancellazione dei costi in realtà prevede il fatto di ignorarli e lasciare degli zombie non collegati
                Dim risposta As RispostaStandard = Utility_Operazioni.Ricette_CancellaRicettaConvertiCosti(ricetta.ricetta_cod, ricetta.ricetta_operazione_cod, ricetta.in_uso, ricetta.app_ricetta_operazione_id, False)

                If Not IsNothing(risposta) AndAlso risposta.RispostaOK = False AndAlso risposta.Errore <> "" Then
                    Throw New Exception(risposta.Errore)
                End If
            Next

            If Not IsNothing(objParametri_Server.objTransazione) Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(True, objParametri_Server)
            End If

            r.RispostaOK = True
            r.RispostaStringa = errMsg

        Catch ex As Exception
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)
            r = MessaggioErroreFrom(ex)
        Finally
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(True, objParametri_Server)
        End Try

        Return r

    End Function


    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Function BloccaAttivitaAgenda(attivita As List(Of Object), objParam_server As String, objParam_utenti As String) As RispostaStandard

        Dim r As New RispostaStandard

        Try
            Dim objP_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objParam_server)
            Dim objP_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objParam_utenti)

            Dim objAgenda As New AgronicaCoreContabBIZ.Agenda_W
            objAgenda.Blocca_AttivitaAgenda(attivita, objP_Server)

            r.RispostaOK = True
        Catch ex As Exception
            Return MessaggioErroreFrom(ex)
        End Try

        Return r
    End Function
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Function BloccaAttivitaAgenda_NG(InData As CoreWS_Generic(Of List(Of Object))) As RispostaStandard

        Dim r As New RispostaStandard

        Try
            Dim objP_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objP_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            Dim objAgenda As New AgronicaCoreContabBIZ.Agenda_W
            objAgenda.Blocca_AttivitaAgenda(InData.InData, objP_Server)

            r.RispostaOK = True
        Catch ex As Exception
            Return MessaggioErroreFrom(ex)
        End Try

        Return r
    End Function
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Function SbloccaAttivitaAgenda(attivita As List(Of Object), objParam_server As String, objParam_utenti As String) As RispostaStandard

        Dim r As New RispostaStandard

        Try
            Dim objP_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objParam_server)
            Dim objP_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objParam_utenti)

            Dim objAgenda As New AgronicaCoreContabBIZ.Agenda_W
            objAgenda.Sblocca_AttivitaAgenda(attivita, objP_Server)

            r.RispostaOK = True
        Catch ex As Exception
            Return MessaggioErroreFrom(ex)
        End Try

        Return r
    End Function
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Function SbloccaAttivitaAgenda_NG(InData As CoreWS_Generic(Of List(Of Object))) As RispostaStandard

        Dim r As New RispostaStandard

        Try
            Dim objP_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objP_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            Dim objAgenda As New AgronicaCoreContabBIZ.Agenda_W
            objAgenda.Sblocca_AttivitaAgenda(InData.InData, objP_Server)

            r.RispostaOK = True
        Catch ex As Exception
            Return MessaggioErroreFrom(ex)
        End Try

        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Aggiungi_Al_PUA(InData As CoreWS_Generic(Of Object)) As RispostaStandard
        Dim r As New RispostaStandard

        Try

            Dim datiRequest As String = JsonConvert.SerializeObject(InData)
            Dim objRequest = JsonConvert.DeserializeObject(Of CoreWS_Generic(Of AgronicaCoreDTOStd.InData.Agenda.AggiungiRicettaAlPUA))(datiRequest)
            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_utenti)
            Dim objParametri As AgronicaCoreDTOStd.InData.Agenda.AggiungiRicettaAlPUA = objRequest.InData

            Dim leggiLingua As New Lingue_Read
            Dim dtLingua As DataTable = leggiLingua.Leggi_datoCODGIAS(objParametri_Server.Lingua_Cod,
                                                                  AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                  "", "", objParametri_Server)

            Dim linguaCodiceISO As String = dtLingua.Rows(0)("CodiceISO")
            Threading.Thread.CurrentThread.CurrentUICulture = New System.Globalization.CultureInfo(linguaCodiceISO)

            Dim id_agenda_array As String() = objParametri.id_agenda_checked.Split(",")
            Dim lav_cod_array As String() = objParametri.lav_cod_checked.Split(",")

            Dim id_agenda_copiabili_array As New List(Of String)
            Dim lav_cod_copiabili_array As New List(Of String)

            For i = 0 To id_agenda_array.Length - 1
                If id_agenda_array(i) = "-1" Then
                    id_agenda_copiabili_array.Add(id_agenda_array(i))
                    lav_cod_copiabili_array.Add(lav_cod_array(i))
                    Continue For
                End If
                id_agenda_copiabili_array.Add(id_agenda_array(i))
                lav_cod_copiabili_array.Add(lav_cod_array(i))
            Next

            If id_agenda_copiabili_array.Count = 0 OrElse (id_agenda_copiabili_array.Count = 1 AndAlso id_agenda_copiabili_array(0) = "-1") Then
                r.RispostaOK = False
                r.Errore = AgronicaCoreContabBIZ.My.Resources.AgronicaCoreContabBIZ.NonÈPossibileCopiareQuestOperazionePlurale
                Return r
            End If


            'salvataggio ricette
            Dim objRicettaOpW As New AgronicaCoreContabBIZ.Ricette_Operazioni_W
            Dim aggiungi_operazioni As Boolean = objRicettaOpW.Aggiungi_RicettaOperazioni_Da_OperazioniAgenda(objParametri.piva, objParametri.sa_cod, objParametri.ricetta_cod, enum_TipoRicetta.PianoDistribuzionePua, objParametri.id_agenda_checked, objParametri_Server, objParametri_Utenti, "")

            r.RispostaOK = True
            r.RispostaStringa = ""

        Catch ex As Exception
            Return MessaggioErroreFrom(ex)
        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function CaricaTrappole(InData As CoreWS_Generic(Of CaricaOperazioni)) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_super_server)
        Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
        Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

        Try

            Dim leggiLingua As New AgronicaCoreUtentiDAL.Lingue_Read
            Dim dtLingua As DataTable = leggiLingua.Leggi_datoCODGIAS(objParametri_Server.Lingua_Cod, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Utenti)
            Dim linguaCodiceISO As String = dtLingua.Rows(0)("CodiceISO")
            Threading.Thread.CurrentThread.CurrentUICulture = New Globalization.CultureInfo(linguaCodiceISO)

            Dim Mov_Biz As New AgronicaCoreContabBIZ.Movimenti_R()
            r = Mov_Biz.CaricaTrappole(InData.InData.filtro, InData.InData.piva, objParametri_Server, objParametri_Utenti)

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try
        Return r

    End Function

End Class