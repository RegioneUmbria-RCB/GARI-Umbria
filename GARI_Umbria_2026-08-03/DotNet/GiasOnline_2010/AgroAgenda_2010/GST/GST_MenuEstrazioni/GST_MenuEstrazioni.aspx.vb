
Imports System.Drawing
Imports System.Data
Imports System.Web.Services
Imports AgronicaCoreDataProvider
Imports System.Configuration.ConfigurationManager
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreSementieriBIZ
Imports Newtonsoft.Json.Linq
Imports AgronicaCoreSementieriDAL
Imports AgronicaCoreVarieBIZ

Partial Class GST_MenuEstrazioni
    Inherits System.Web.UI.Page

    Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri
    Dim objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri

    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function LeggiListaSportelli() As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametri_Server = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti = HttpContext.Current.Session("ASG_objParametri_Utenti")

        Try
            Dim xRead As New Sportello_R
            Dim DT = xRead.Leggi_Sementieri_Sportello_Configurazione(0,
                                                                 CostantiPersonalizzate.AGRODATAINIZIO,
                                                                 "",
                                                                 objParametri_Server)

            DT.DefaultView.Sort = "Sementieri_Sportello_Configurazione_cod DESC"
            DT = DT.DefaultView.ToTable

            Dim placeholder As DataRow = DT.NewRow
            placeholder(0) = 0
            placeholder(1) = "Scegli uno sportello"

            DT.Rows.InsertAt(placeholder, 0)

            r.RispostaOK = True
            r.RispostaStringa = Newtonsoft.Json.JsonConvert.SerializeObject(DT)

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = ex.Message
        End Try

        Return r
    End Function

    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function LeggiColtivazioni(ByVal Sementieri_Sportello_Configurazione_cod As Integer,
                                             ByVal Tipologia_Report As String,
                                             ByVal Regione As String) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametri_Server = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti = HttpContext.Current.Session("ASG_objParametri_Utenti")

        Try
            Dim xRead As New Sementieri_EstrazioneColtivazioni_R

            Dim DT = xRead.Leggi(0,
                             Sementieri_Sportello_Configurazione_cod,
                             Regione,
                             Tipologia_Report,
                             AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                             "",
                             "",
                             objParametri_Server)

            Dim listaColonne As New List(Of ColonneNome)

            For Each col In DT.Columns
                If Not col.ToString().Equals("Sementieri_Sportello_Configurazione_cod") Then
                    listaColonne.Add(New ColonneNome(col, col.ToString))
                End If
            Next

            Dim js As New AgronicaCoreDataProvider.JSON_DataTable

            r.RispostaOK = True
            r.RispostaStringa = js.JSON_DataTable_Kendo(DT, listaColonne)

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = ex.Message
        End Try

        Return r
    End Function

    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function LeggiInterferenze(ByVal Sementieri_Sportello_Configurazione_cod As Integer,
                                             ByVal Regione As String) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametri_Server = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti = HttpContext.Current.Session("ASG_objParametri_Utenti")

        Try
            Dim xRead As New Sementieri_EstrazioneInterferenze_R

            Dim DT = xRead.Leggi(0,
                             Sementieri_Sportello_Configurazione_cod,
                             Regione,
                             AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                             "",
                             "",
                             objParametri_Server)

            Dim listaColonne As New List(Of ColonneNome)

            For Each col In DT.Columns
                If Not col.ToString().Equals("Sementieri_Sportello_Configurazione_cod") Then
                    listaColonne.Add(New ColonneNome(col, col.ToString))
                End If
            Next

            Dim js As New AgronicaCoreDataProvider.JSON_DataTable

            r.RispostaOK = True
            r.RispostaStringa = js.JSON_DataTable_Kendo(DT, listaColonne)

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = ex.Message
        End Try

        Return r
    End Function

    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function LeggiVariazioni(ByVal Sementieri_Sportello_Configurazione_cod As Integer,
                                           ByVal Regione As String) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametri_Server = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti = HttpContext.Current.Session("ASG_objParametri_Utenti")

        Try
            Dim xRead As New Sementieri_EstrazioneVariazioni_R

            Dim DT = xRead.Leggi(0,
                             Sementieri_Sportello_Configurazione_cod,
                             Regione,
                             AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                             "",
                             "",
                             objParametri_Server)

            Dim listaColonne As New List(Of ColonneNome)

            For Each col In DT.Columns
                If Not col.ToString().Equals("Sementieri_Sportello_Configurazione_cod") Then
                    listaColonne.Add(New ColonneNome(col, col.ToString))
                End If
            Next

            Dim js As New AgronicaCoreDataProvider.JSON_DataTable

            r.RispostaOK = True
            r.RispostaStringa = js.JSON_DataTable_Kendo(DT, listaColonne)

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = ex.Message
        End Try

        Return r
    End Function

    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function VerificaPermessiEstrazione() As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri_Utenti = HttpContext.Current.Session("ASG_objParametri_Utenti")

        Dim ObjUtenti As New AgronicaCoreUtentiDAL.Utenti_Permessi_R

        Try

            Dim permessiWMS As Boolean = ObjUtenti.Controlla_Permessi_Utente(
            objParametri_Utenti.UtenteUsername,
            enum_Id_Servizio.GiasOnline,
            enum_Security_Attivita.Interferenze_ScaricoDati_Report_Completo,
            enum_Security_Operazione.Lettura,
            Date.Now,
            "",
            objParametri_Utenti)

            r.RispostaOK = True
            r.RispostaStringa = permessiWMS.ToString

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = ex.Message
        End Try

        Return r

    End Function

    Private Sub Page_PreInit(sender As Object, e As System.EventArgs) Handles Me.PreInit

        If Session.IsNewSession Then
            Response.Redirect("../../index.aspx")
        End If

        '----- Dimensiono le variabili
        objParametri_Server = HttpContext.Current.Session("ASG_objParametri_Server")
        objParametri_Utenti = HttpContext.Current.Session("ASG_objParametri_Utenti")

    End Sub


    '####################################################################################
    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        '##############################################################
        '#####  Verifico Credenziali di Accesso  ######################
        '##############################################################

        '----- Verifico che l'utente sia autenticato

        If IsNothing(objParametri_Server) Then
            Response.Redirect("../../index.aspx")
        End If

        '----- Verifico se l'utente dispone dei permessi per la visualizzazione della pagina

        Dim UtenteAbilitato As Boolean = False

        Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
        UtenteAbilitato = objPermessi.Controlla_Permessi_Utente(
                                    objParametri_Utenti.UtenteUsername,
                                    5,
                                    TipiEnumerativi.enum_Security_Attivita.Interferenze_ScaricoDati,
                                    enum_Security_Operazione.Lettura,
                                    Date.Now,
                                    "",
                                    objParametri_Utenti)



        '----- Se l'utente non ha il permesso per visualizzare la pagina ... lo invio alla autenticazione.

        If (UtenteAbilitato = False) Then
            Response.Redirect("../../index.aspx")
        End If

        'If objParametri_Utenti.UtenteUsername <> objParametri_Server.SuperUserUsername Then
        'btnGestioneSportello.Visible = False
        'btnStampaElenco.Visible = False
        'End If

        If Session("Codice_Fiscale_Tecnico") Is Nothing Then

            Dim Codice_Fiscale_Tecnico As String = ""
            Dim xUtenteCorrente As String = CType(objParametri_Server, AgronicaCoreDataProvider.AgronicaCoreParametri).UtenteUsername
            Dim objUtentiDAL As New AgronicaCoreUtentiDAL.Utenti_xGruppi_Utente_R
            Codice_Fiscale_Tecnico = objUtentiDAL.Leggi_IdentificativoGruppoUtenti_Singolo(xUtenteCorrente, objParametri_Utenti)
            Session("Codice_Fiscale_Tecnico") = Codice_Fiscale_Tecnico
            Session("Codice_Fiscale_Tecnico_OK") = Codice_Fiscale_Tecnico

        End If

        '##############################################################
        '#####  Preparazione della pagina  ############################
        '##############################################################

        '----- Attivo i pulsanti a seconda dei permessi

        Dim DT As New DataTable
        Dim i As Integer

        DT = objPermessi.Leggi(Session("ASG_Utente_Username"),
                               11,
                               0, 0,
                               0, "", "", objParametri_Utenti)


        If (Not IsNothing(DT)) AndAlso (DT.Rows.Count >= 1) Then

            Dim Attivita As String

            For i = 0 To DT.Rows.Count - 1

                Attivita = Right("0000000" & DT.Rows(i).Item("ID_Attivita"), 3)

                Select Case Attivita

                    Case "002"      'UTENTI
                        'imgBtnGestioneSportello.ImageUrl = MemoIcona_GestioneSportello
                        'imgBtnGestioneSportello.Enabled = True
                        'lblGestioneSportello.ForeColor = MemoColore_GestioneSportello

                    Case "003"      'DISTANZE
                        'ImgBtnGestioneConfigurazione.ImageUrl = MemoIcona_GestioneConfigurazione
                        'ImgBtnGestioneConfigurazione.Enabled = True
                        'LblGestioneConfigurazione.ForeColor = MemoColore_GestioneConfigurazione

                    Case "004"     'COLORI
                        'ImgBtnGestioneColori.ImageUrl = MemoIcona_GestioneColori
                        'ImgBtnGestioneColori.Enabled = True
                        'LblGestioneColori.ForeColor = MemoColore_GestioneColori

                    Case "005"

                        'Utente di tipo COOPERATIVA
                        Session("TipoUtente") = 1

                        'ImgBtnVerificaInterferenze.ImageUrl = MemoIcona_VerificaInterferenze
                        'ImgBtnVerificaInterferenze.Enabled = True
                        'LblVerificaInterferenze.ForeColor = MemoColore_VerificaInterferenze

                    Case "006"

                        'Utente di tipo SUPERUSER
                        Session("TipoUtente") = 2

                        'ImgBtnVerificaInterferenze.ImageUrl = MemoIcona_VerificaInterferenze
                        'ImgBtnVerificaInterferenze.Enabled = True
                        'LblVerificaInterferenze.ForeColor = MemoColore_VerificaInterferenze

                End Select

            Next

        End If

        'Pulizia Risorse
        DT.Dispose()

    End Sub


End Class
