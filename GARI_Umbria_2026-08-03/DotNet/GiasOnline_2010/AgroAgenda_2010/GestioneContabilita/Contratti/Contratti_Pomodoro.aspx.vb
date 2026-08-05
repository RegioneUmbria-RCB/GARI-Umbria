

Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreUtility
Imports AgronicaCoreGestioneRichieste
Imports AgronicaCoreXML.XML_Stampe
Imports AgronicaCoreVarieBIZ

Imports AgronicaCoreModello.OperazioneAgenda_Temp
Imports AgronicaCoreModello.ParametriAgenda_Temp
Imports System.Web.Services
Imports Newtonsoft.Json.Linq
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreAnagrafeBIZ
Imports AgronicaCoreAnagrafeDAL
Imports AgronicaCoreContabDAL
Imports Newtonsoft.Json

Public Class Contratti_Pomodoro
    Inherits System.Web.UI.Page

    Protected Overrides Sub InitializeCulture()
        MyBase.InitializeCulture()
        Lingua.Gias_InizializzaCultura_DaSession()
    End Sub

    Dim objParametri_Server, objParametri_Utenti As AgronicaCoreParametri
    Public objparametri_server_string, objparametri_utenti_string As String
    Dim objParametriAgenda As ParametriAgenda

    Private Sub inizializzoObjParametri()
        '---
        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))
        objParametri_Utenti = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))
        objparametri_server_string = AgronicaCoreDataProvider.Utility.convertOBJparametritoString(objParametri_Server)
        '---
    End Sub

    Private _qsPagRitorno As String


#Region "script services leggi Contratti Pomodoro"

    <WebMethod(EnableSession:=True)>
    Public Shared Function Leggi_Fasi(ByVal piva As String, ByVal contratto_cod As Integer) As RispostaStandard

        Dim r As New RispostaStandard
        Dim Dt As DataTable
        Dim leggi As New AgronicaCoreContabDAL.Imprese_Contratto_Fasi_R
        Dim Filtro_Aggiuntivo As String = ""

        Try

            Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
                r.Sessione = False
                Return r
            End If

            Dt = leggi.Leggi(piva, contratto_cod, 0, 0, "", 0, 0, 0, 0, 0, "", 0, enumSelezioneVariabile.Selezione_TabellaDatiMinimi, Filtro_Aggiuntivo, "", objParametri_Server)

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            r.RispostaStringa = JsonConvert.SerializeObject(Dt, Formatting.None, serializerSettings)

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
    Public Shared Function Leggi_Clausole(ByVal piva As String, ByVal cau_contratto As String) As RispostaStandard

        Dim r As New RispostaStandard
        Dim Dt As DataTable
        Dim leggi As New AgronicaCoreContabDAL.Imprese_Contratti_Clausole_R
        Dim Filtro_Aggiuntivo As String = ""

        Try

            Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
                r.Sessione = False
                Return r
            End If

            Dt = leggi.Leggi(piva, 0, cau_contratto, enumSelezioneVariabile.Selezione_TabellaDatiMinimi, Filtro_Aggiuntivo, "", objParametri_Server)

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            r.RispostaStringa = JsonConvert.SerializeObject(Dt, Formatting.None, serializerSettings)

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
    Public Shared Function Leggi_Specie(ByVal veg_cod As Integer) As RispostaStandard

        Dim r As New RispostaStandard
        Dim Dt As DataTable
        Dim leggi As New AgronicaCoreMetaSchemaDAL.SpecieVegetali_R

        Try

            Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
                r.Sessione = False
                Return r
            End If

            Dt = leggi.Leggi(veg_cod, 0, "", "", enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_Server)

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            r.RispostaStringa = JsonConvert.SerializeObject(Dt, Formatting.None, serializerSettings)

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
    Public Shared Function Leggi_Prodotti(ByVal piva As String, ByVal veg_cod As Integer) As RispostaStandard

        Dim r As New RispostaStandard
        Dim Dt As DataTable
        Dim Dt_Filtro As DataTable
        Dim leggi As New AgronicaCoreAnagrafeDAL.Materie_Prime_R
        Dim leggi_filtro As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_FiltroMono_R
        Dim Filtro_Aggiuntivo As String

        Try

            Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
                r.Sessione = False
                Return r
            End If

            Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Utenti) Then
                r.Sessione = False
                Return r
            End If

            Dt_Filtro = leggi_filtro.Leggi2(2, objParametri_Utenti.SuperUserUsername, enum_Impostazioni_Utenti.SuperUser_FiltroSQL_MateriePrime, enum_AreaGIAS.Conferimento, "", "", objParametri_Utenti)

            If Dt_Filtro.Rows.Count > 0 Then

                Filtro_Aggiuntivo = Dt_Filtro(0)("Str_0")

            End If


            Dt = leggi.LeggiMateriePrimeContratto(piva, 210, 0, veg_cod, Filtro_Aggiuntivo, "Mat_Des_Esteso", objParametri_Server)
            'Dt = leggi.LeggiGenerazioniReferenze(piva, 210, 0, 2, "", "Mat_Des_Esteso", objParametri_Server, veg_cod)

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            r.RispostaStringa = JsonConvert.SerializeObject(Dt, Formatting.None, serializerSettings)

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
    Public Shared Function Leggi_Listini(ByVal piva As String, ByVal tipo_classe As Integer, ByVal veg_cod As Integer) As RispostaStandard

        Dim r As New RispostaStandard
        Dim Dt As DataTable
        Dim leggi As New AgronicaCoreContabDAL.FF_CampionamentoConferimento_R
        Dim Filtro_Aggiuntivo As String = ""

        Try

            Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
                r.Sessione = False
                Return r
            End If

            Dt = leggi.LeggiListini_From_Veg_Cod(piva, 0, veg_cod, enumSelezioneVariabile.Selezione_TabellaDatiMinimi, Filtro_Aggiuntivo, "", objParametri_Server)


            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            r.RispostaStringa = JsonConvert.SerializeObject(Dt, Formatting.None, serializerSettings)

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
    Public Shared Function Leggi_Centri(ByVal piva As String) As RispostaStandard

        Dim r As New RispostaStandard
        Dim Dt As DataTable
        Dim leggi As New CentriAziendali_Read
        Dim Filtro_Aggiuntivo As String = ""

        Try

            Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
                r.Sessione = False
                Return r
            End If

            Dt = leggi.Leggi_x_anagrafica(piva, 0, Filtro_Aggiuntivo, "", objParametri_Server)

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            r.RispostaStringa = JsonConvert.SerializeObject(Dt, Formatting.None, serializerSettings)

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
    Public Shared Function Leggi_Fabbricati(ByVal piva As String, ByVal sa_cod As Integer) As RispostaStandard

        Dim r As New RispostaStandard
        Dim Dt As DataTable
        Dim leggi As New Fabbricati_R
        Dim Filtro_Aggiuntivo As String = ""

        Try

            Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
                r.Sessione = False
                Return r
            End If

            Dt = leggi.Leggi(piva, sa_cod, 0, enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_Server)
            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            r.RispostaStringa = JsonConvert.SerializeObject(Dt, Formatting.None, serializerSettings)

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
    Public Shared Function Leggi_ContrattoxClausole(ByVal piva As String, ByVal contratto_cod As Integer) As RispostaStandard

        Dim r As New RispostaStandard
        Dim Dt As DataTable
        Dim leggi As New AgronicaCoreContabDAL.Imprese_Contratto_Fasi_R
        Dim Filtro_Aggiuntivo As String = ""

        Try

            Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
                r.Sessione = False
                Return r
            End If

            Dt = leggi.LeggiNew(piva, contratto_cod, 0, enumSelezioneVariabile.Selezione_TabellaDatiMinimi, Filtro_Aggiuntivo, "", objParametri_Server)

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            r.RispostaStringa = JsonConvert.SerializeObject(Dt, Formatting.None, serializerSettings)

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
    Public Shared Function Leggi_Contratto(ByVal piva As String, ByVal contratto_cod As Integer) As RispostaStandard

        Dim r As New RispostaStandard
        Dim Dt As DataTable
        Dim leggi As New AgronicaCoreContabDAL.Imprese_Contratti_R
        Dim Filtro_Aggiuntivo As String = ""


        Try

            Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
                r.Sessione = False
                Return r
            End If

            Dt = leggi.Leggi(piva, contratto_cod, "", "", "", 0, 0, enumSelezioneVariabile.Selezione_TabellaDatiMinimi, Filtro_Aggiuntivo, "", objParametri_Server)


            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore

            r.RispostaStringa = JsonConvert.SerializeObject(Dt, Formatting.None, serializerSettings)


            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r


    End Function




#End Region



#Region "script services aggiorna Contratti"

    <WebMethod(EnableSession:=True)>
    Public Shared Function AggiornaContrattoPomodoro(ByVal piva As String,
                                                     ByVal contratto_cod As Integer,
                                                     ByVal data_stipulazione As String,
                                                     ByVal anno As Integer,
                                                     ByVal superficie_prevista As String,
                                                     ByVal resa_prevista As String,
                                                     ByVal cod_risum As Integer,
                                                     ByVal cau_contratto As String,
                                                     ByVal listino_cod As String,
                                                     ByVal contratto_numero As String,
                                                     ByVal contratto_nome As String,
                                                     ByVal valore1 As String,
                                                     ByVal valore2 As String,
                                                     ByVal valore3 As String,
                                                     ByVal valore4 As String,
                                                     ByVal valore5 As String,
                                                     ByVal valore6 As String,
                                                     ByVal valore7 As String,
                                                     ByVal valore8 As String,
                                                     ByVal valore9 As String,
                                                     ByVal data_inizio_prevista As String,
                                                     ByVal data_fine_prevista As String,
                                                     ByVal valore1_2 As String,
                                                     ByVal valore2_2 As String,
                                                     ByVal valore3_2 As String,
                                                     ByVal data_inizio_prevista2 As String,
                                                     ByVal data_fine_prevista2 As String,
                                                     ByVal righeGridFasi_Inserite As String,
                                                     ByVal righeGridFasi_Modificate As String,
                                                     ByVal righeGridFasi_Cancellate As String,
                                                     ByVal righeGridClausole As String,
                                                     ByVal sa_cod As Integer,
                                                     ByVal Fabbricato_Cod As Integer
                                                     ) As RispostaStandard

        Dim r As New RispostaStandard
        Dim validita_inizio As Date = "01/01/" & anno
        Dim validita_fine As Date = "31/12/" & anno

        Dim Valore2_Dec As Decimal = 0
        Dim Valore3_Dec As Decimal = 0
        Dim Valore4_Dec As Decimal = 0
        Dim Valore5_Dec As Decimal = 0
        Dim Valore6_Dec As Decimal = 0
        Dim Valore7_Dec As Decimal = 0
        Dim Valore8_Dec As Decimal = 0
        Dim Valore9_Dec As Decimal = 0
        Dim Valore2_2_Dec As Decimal = 0
        Dim Valore3_2_Dec As Decimal = 0
        Dim Superficie_Prevista_Dec As Decimal = 0
        Dim Resa_Prevista_Dec As Decimal = 0

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try
            Dim scrivi As New AgronicaCoreContabBIZ.Imprese_Contratti_BIZ_W
            Dim Dummy As Integer

            If Not IsDate(data_inizio_prevista) Then
                data_inizio_prevista = AGRODATAINIZIO
            End If

            If Not IsDate(data_fine_prevista) Then
                data_fine_prevista = AGRODATAFINE
            End If

            If Not IsDate(data_inizio_prevista2) Then
                data_inizio_prevista2 = AGRODATAINIZIO
            End If

            If Not IsDate(data_fine_prevista2) Then
                data_fine_prevista2 = AGRODATAFINE
            End If

            If IsNumeric(superficie_prevista) Then
                Superficie_Prevista_Dec = Replace(superficie_prevista, ".", ",")
            End If

            If IsNumeric(resa_prevista) Then
                Resa_Prevista_Dec = Replace(resa_prevista, ".", ",")
            End If

            valore1 = IIf(valore1 = "false", 0, 1)
            valore1_2 = IIf(valore1_2 = "false", 0, 1)

            If IsNumeric(valore2) Then
                Valore2_Dec = Replace(valore2, ".", ",")
            End If
            If IsNumeric(valore3) Then
                Valore3_Dec = Replace(valore3, ".", ",")
            End If
            If IsNumeric(valore4) Then
                Valore4_Dec = Replace(valore4, ".", ",")
            End If
            If IsNumeric(valore5) Then
                Valore5_Dec = Replace(valore5, ".", ",")
            End If
            If IsNumeric(valore6) Then
                Valore6_Dec = Replace(valore6, ".", ",")
            End If
            If IsNumeric(valore7) Then
                Valore7_Dec = Replace(valore7, ".", ",")
            End If
            If IsNumeric(valore8) Then
                Valore8_Dec = Replace(valore8, ".", ",")
            End If
            If IsNumeric(valore9) Then
                Valore9_Dec = Replace(valore9, ".", ",")
            End If
            If IsNumeric(valore2_2) Then
                Valore2_2_Dec = Replace(valore2_2, ".", ",")
            End If
            If IsNumeric(valore3_2) Then
                Valore3_2_Dec = Replace(valore3_2, ".", ",")
            End If

            If Not IsNumeric(listino_cod) Then
                listino_cod = 0
            End If

            Dummy = scrivi.AggiornaContrattoPomodoro(
                piva,
                contratto_cod, "",
                contratto_nome, "",
                contratto_numero,
                Superficie_Prevista_Dec,
                Resa_Prevista_Dec, 0,
                cau_contratto, 0, "",
                data_inizio_prevista,
                data_fine_prevista,
                "", "", cod_risum, 0, 0, 0,
                data_stipulazione,
                validita_inizio,
                validita_fine,
                righeGridFasi_Inserite,
                righeGridFasi_Modificate,
                righeGridFasi_Cancellate,
                righeGridClausole,
                listino_cod,
                valore1,
                Valore2_Dec,
                Valore3_Dec,
                Valore4_Dec,
                Valore5_Dec,
                Valore6_Dec,
                Valore7_Dec,
                Valore8_Dec,
                Valore9_Dec,
                valore1_2,
                Valore2_2_Dec,
                Valore3_2_Dec,
                data_inizio_prevista2,
                data_fine_prevista2,
                sa_cod,
                Fabbricato_Cod,
                objParametri_Server
            )

            r.RispostaStringa = CStr(Dummy)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, False, source:=False)
        End Try

        Return r

    End Function



#End Region


#Region "Caricamento"

    Private Sub caricaControlli()


        'End Select
    End Sub

#End Region


    Private Sub AnnullaTutto(ByVal sender As Object, ByVal e As System.EventArgs)


        Select Case CInt(_qsPagRitorno)

            Case 209
                Dim url As String = "~/GestioneContabilita/Contratti/Contratti_Menu.aspx" &
                                    "?p=" & Stringa_Codifica(hdPiva.Value, AgroKey_EncoderDecoder)
                Response.Redirect(url)

            Case enum_PagineAgenda_2010.Menu
                If Master.flag_MenuBS_2017 Then
                    Response.Redirect("../Menu/MenuBS_2017.aspx")
                Else
                    Response.Redirect("~/Menu/Menu.aspx")
                End If

            Case enum_PagineAgenda_2010.Menu_BS
                Response.Redirect("~/Menu/MenuBS_Agenda_Nuovo.aspx")
        End Select

    End Sub





    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Master().Lbl_Titolo.Text = "Contratto Conferimento Pomodoro"

        'pagina chiamante --> per gestire il tipo di uscita dalla pagina:
        'fare un redirect o chiuderla (perché aperta in modal dialog)
        'usa enum_PagineGiasOnline

        If Not IsNothing(Request.QueryString("orig")) Then
            _qsPagRitorno = Stringa_Decodifica(Request.QueryString("orig").ToString, AgroKey_EncoderDecoder)
        Else
            _qsPagRitorno = CStr(209)
        End If



        inizializzoObjParametri()
        inizializzoParametriPagina()

        'Controllo se l'utente ha i permessi per accedere
        Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
        Dim UtenteAbilitatoLettura As Boolean = objPermessi.Controlla_Permessi_Utente(
                                            Session("ASG_Utente_Username"),
                                            Session("ASG_IdServizio"),
                                            enum_Security_Attivita.Gestione_Contratti_Conferimento,
                                            enum_Security_Operazione.Lettura,
                                            Date.Now,
                                            "",
                                            objParametri_Utenti)

        Dim UtenteAbilitatoScrittura As Boolean = objPermessi.Controlla_Permessi_Utente(
                                           Session("ASG_Utente_Username"),
                                           Session("ASG_IdServizio"),
                                           enum_Security_Attivita.Gestione_Contratti_Conferimento,
                                           enum_Security_Operazione.Modifica,
                                           Date.Now,
                                           "",
                                           objParametri_Utenti)

        'Imposto le variabili di ponte con il client
        hf_UtenteAbilitatoLettura.Value = UtenteAbilitatoLettura
        hf_UtenteAbilitatoScrittura.Value = UtenteAbilitatoScrittura

        If UtenteAbilitatoLettura = False Then
            Response.Redirect("~/Menu/MenuBS_Agenda_Nuovo.aspx")
        End If

        hdPiva.Value = Stringa_Decodifica(CStr(Request.QueryString("p")),
                                          AgroKey_EncoderDecoder,
                                          Server)


        hdPiva_Codificata.Value = Stringa_Codifica(hdPiva.Value, AgroKey_EncoderDecoder, HttpContext.Current.Session)

        If Not Request.QueryString("contratto_cod") Is Nothing Then
            hdContratto_Cod.Value = Request.QueryString("contratto_cod")
        Else
            hdContratto_Cod.Value = 0
        End If

        If Not Request.QueryString("veg_cod") Is Nothing Then
            hdVeg_Cod.Value = Request.QueryString("veg_cod")
        Else
            hdVeg_Cod.Value = 0
        End If

        'hdVeg_Cod.Value = 52 'Forzato al momento


        Dim PaginaRedirect As String = ""

        hdPaginaRedirect.Value = ""
        If Not Request.QueryString("origine") Is Nothing Then

            hdPaginaRedirect.Value = Request.QueryString("origine")
        Else
            hdPaginaRedirect.Value = "../Menu/MenuBS_Agenda_Nuovo.aspx"
        End If

        If Not IsNothing(Session("ParametriAgenda_2010")) Then

            Dim objParametriAgenda_2010 As New ParametriAgenda_2010
            objParametriAgenda_2010.Leggi()

        End If


        Master.flag_MostraBtnIndietro = True
        AddHandler Master.ImgBtn_AnnullaTutto.Click, AddressOf Me.AnnullaTutto
        AddHandler Master.ImgBtnAnnullaTutto.Click, AddressOf Me.AnnullaTutto


    End Sub


    Private Sub inizializzoParametriPagina()


    End Sub

    Public Shadows ReadOnly Property Master() As AgroAgenda_2010.AgendaBootstrap
        Get
            Return CType(MyBase.Master, AgroAgenda_2010.AgendaBootstrap)
        End Get
    End Property

End Class