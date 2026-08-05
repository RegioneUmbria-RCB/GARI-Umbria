Imports System.Web.Services

Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi

Imports AgronicaCoreModello.ParametriAgenda_Temp
Imports AgronicaCoreUtentiDAL
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreVarieBIZ
Imports Newtonsoft.Json
Imports AgroAgenda_2010.Resources

Public Class Prodotto_Edit
    Inherits System.Web.UI.Page

    Dim objParametri_Server, objParametri_Utenti As AgronicaCoreParametri
    Public permessi As PermessiUtente

    Dim xPiva As String
    Dim xSa_Cod As String
    Dim xCod_Contatto As String
    Public XTipoOperazione As Integer

    Dim BaseCode As Integer
    Dim TopCode As Integer


    Dim Qs_Operazione As String
    Dim Qs_Piva As String
    Dim Qs_Mat_Cod As String
    Dim Qs_Elem_COd As String
    Dim QS_Proprietario As String
    Dim QS_Pro_Cod As String
    Dim QS_Prodotto_Des As String
    Dim QS_Sa_cod As String
    Dim QS_Is_Alias As String
    Dim Qs_Visibilita As Integer = 0

    Public winProdotto As Boolean

    Protected Overrides Sub InitializeCulture()
        MyBase.InitializeCulture()
        Lingua.Gias_InizializzaCultura_DaSession()
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        permessi = New PermessiUtente()

        Master.flag_pag_Anagrafica = True
        '----- Verifico che l'utente sia autenticato
        If Session("ASG_Utente_Username") = "" Then
            Response.Redirect("~/Custom500.aspx")
        End If
        If Request.QueryString("visibilita") IsNot Nothing AndAlso IsNumeric(Request.QueryString("visibilita")) Then
            Qs_Visibilita = CInt(Request.QueryString("visibilita"))
        End If

        ''Se Qs_Visibilita = 0 è stato aperto dal Menu Anagrafe generale e quindi utilizzo la versione standard della grafica
        'If Qs_Visibilita = 0 Then
        '    Master.Master_versione = VERSIONE_MASTER_DEFAULT
        '    Master.Header_versione = VERSIONE_HEADER_DEFAULT
        'End If
        '---
        objParametri_Server = New AgronicaCoreParametri(Session("ASG_objParametri_Server"))
        objParametri_Utenti = New AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))
        '---

        ' parametro usato se la pagina viene aperta in una finestra
        If Not String.IsNullOrWhiteSpace(Request.QueryString("win")) Then
            Master.flag_MostraHeader = False
            Master.flag_MostraFooter = False
            winProdotto = True
        End If

        If Not String.IsNullOrWhiteSpace(Request.QueryString("o")) Then
            Qs_Operazione = Stringa_Decodifica(Request.QueryString("o").ToString,
                                    AgroKey_EncoderDecoder,
                                    Server)
        End If

        If Not String.IsNullOrWhiteSpace(Request.QueryString("piva")) Then
            Qs_Piva = Stringa_Decodifica(Request.QueryString("piva").ToString,
                                        AgroKey_EncoderDecoder,
                                        Server)
        End If

        If Not String.IsNullOrWhiteSpace(Request.QueryString("mat_cod")) Then
            Qs_Mat_Cod = Stringa_Decodifica(Request.QueryString("mat_cod").ToString,
                                        AgroKey_EncoderDecoder,
                                        Server)
        End If

        If Not String.IsNullOrWhiteSpace(Request.QueryString("elem_cod")) Then
            Qs_Elem_COd = Stringa_Decodifica(Request.QueryString("elem_cod").ToString,
                                        AgroKey_EncoderDecoder,
                                        Server)
        End If

        If Not String.IsNullOrWhiteSpace(Request.QueryString("pro_cod")) Then
            QS_Pro_Cod = Stringa_Decodifica(Request.QueryString("pro_cod").ToString,
                                        AgroKey_EncoderDecoder,
                                        Server)
        End If

        If Not String.IsNullOrWhiteSpace(Request.QueryString("prodotto_des")) Then
            QS_Prodotto_Des = Stringa_Decodifica(Request.QueryString("prodotto_des").ToString,
                                        AgroKey_EncoderDecoder,
                                        Server)
        End If

        If Not String.IsNullOrWhiteSpace(Request.QueryString("proprietario")) Then
            QS_Proprietario = Stringa_Decodifica(Request.QueryString("proprietario").ToString,
                                        AgroKey_EncoderDecoder,
                                        Server)
        End If


        If Not String.IsNullOrWhiteSpace(Request.QueryString("sa_cod")) Then
            QS_Sa_cod = Stringa_Decodifica(Request.QueryString("sa_cod").ToString,
                                        AgroKey_EncoderDecoder,
                                        Server)
        End If



        If Not String.IsNullOrWhiteSpace(Request.QueryString("isalias")) Then
            QS_Is_Alias = Stringa_Decodifica(Request.QueryString("isalias").ToString,
                                        AgroKey_EncoderDecoder,
                                        Server)
        End If

        Dim impDict As New Dictionary(Of String, Object)
        hf_Opzioni_Materie_Prime.Value = Carica_Impostazioni_Materie_Prime(impDict)

        Dim objParametriAgenda As New ParametriAgenda

        objParametriAgenda = New ParametriAgenda
        If String.IsNullOrEmpty(Qs_Piva) Then
            xPiva = objParametriAgenda.Piva
        Else
            xPiva = Qs_Piva
        End If
        hf_Piva.Value = xPiva

        hf_Piva_Corrente.Value = objParametriAgenda.Piva

        hf_Mat_Cod.Value = Qs_Mat_Cod
        hf_Elem_Cod.Value = Qs_Elem_COd
        hf_xProprietario.Value = QS_Proprietario
        hf_Sa_Cod.Value = QS_Sa_cod
        hf_Pro_Cod.Value = QS_Pro_Cod
        hf_Prodotto_Des.Value = QS_Prodotto_Des
        hf_Ragione_Sociale.Value = objParametriAgenda.RagSoc

        'Fisso tipo operazione
        If String.IsNullOrEmpty(Qs_Operazione) Then
            XTipoOperazione = objParametriAgenda.Tipo_Operazione
        Else
            XTipoOperazione = CInt(Qs_Operazione)
        End If
        hf_TipoOperazione.Value = XTipoOperazione

        hf_DatiImmagineCaricata.Value = ""

        hf_Is_Alias.Value = QS_Is_Alias

        'Leggo l'impostazione per la gerarchia attraverso obj parametri superuser e la classe DAL Utenti_Impostazioni_Read
        Dim objImpR As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
        Dim dtSuperUser As DataTable = objImpR.Leggi(enum_Impostazioni_Utenti.SUPERUSER_ACCETTAZIONE_CON_GERARCHIA, 2,
                                        AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                        "", "", objParametri_Utenti)
        Dim superUserAccettazioneConGerarchia = 0
        If dtSuperUser.Rows.Count > 0 Then
            superUserAccettazioneConGerarchia = dtSuperUser.Rows(0)("Impostazione_Valore_1")
        End If
        hdSuperUserAccGerarchia.Value = superUserAccettazioneConGerarchia

        Dim handleImpreseImp As New AgronicaCoreAnagrafeDAL.Imprese_Impostazioni_R
        hfGruppoMerceControllo.Value = handleImpreseImp.LeggiScalareMulticentroAziendaSuperUser(
            xPiva,
            Nothing,
            enum_Impostazioni_Utenti.GruppoMerce_Controllo,
            "0",
            objParametri_Utenti,
            objParametri_Server)


        ' TODO LOcalizzazione
        Select Case XTipoOperazione
            Case enum_TipoOperazioneDB.Scrittura
                'i18N Creazione Nuovo Prodotto da tradurre
                Master.Lbl_Titolo.Text = AgronicaAgenda_2010.CreazioneNuovoProdotto
            Case enum_TipoOperazioneDB.Lettura
                'i18N Lettura Prodotto da tradurre
                Master.Lbl_Titolo.Text = AgronicaAgenda_2010.LetturaProdotto
            Case enum_TipoOperazioneDB.Modifica
                'i18N Modifica da tradurre
                Master.Lbl_Titolo.Text = AgronicaAgenda_2010.ModificaProdotto
            Case enum_TipoOperazioneDB.Copia
                'i18N Duplica da tradurre
                Master.Lbl_Titolo.Text = AgronicaAgenda_2010.DuplicaProdotto
        End Select

        '==================================
        '======= VERIFICA PERMESSI ========
        '==================================

        Dim UtenteAbilitato_Lettura As Boolean = False

        Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
        Dim UtenteAbilitato_Modifica As Boolean = False
        UtenteAbilitato_Lettura = permessi.getPermesso(enum_Security_Attivita.Angrafica_Prodotti).Lettura
        UtenteAbilitato_Modifica = permessi.getPermesso(enum_Security_Attivita.Angrafica_Prodotti).Scrittura

        hf_UtenteAbilitatoLettura.Value = UtenteAbilitato_Lettura
        hf_UtenteAbilitatoScrittura.Value = UtenteAbilitato_Modifica

        Session("UtenteAbilitato_Modifica") = UtenteAbilitato_Modifica
        Session("UtenteAbilitato_Lettura") = UtenteAbilitato_Lettura

        If UtenteAbilitato_Lettura = False Then
            Dim totseg As Integer = Request.UrlReferrer.Segments.Length
            Dim PreviousPage As String = $"../{Request.UrlReferrer.Segments(totseg - 2)}{Request.UrlReferrer.Segments(totseg - 1)}"
            hfPaginaRedirect.Value = PreviousPage
            Response.Redirect(hfPaginaRedirect.Value)
            Exit Sub
        End If

        If Not Page.IsPostBack Then


            Dim paginaRedirect As String = Master.TrovaRedirectCorretto(False, enum_PagineGiasOnline.MenuAnagrafica, objParametriAgenda)
            hfPaginaRedirect.Value = paginaRedirect
            Dim totseg As Integer = Request.UrlReferrer.Segments.Length
            Dim PreviousPage As String = $"../{Request.UrlReferrer.Segments(totseg - 2)}{Request.UrlReferrer.Segments(totseg - 1)}"
            If Request.UrlReferrer.Segments(totseg - 1).ToLowerInvariant().Contains("menubs_anagraficaprodotti") Then
                hfPaginaRedirect.Value = PreviousPage
            End If

            If XTipoOperazione = enum_TipoOperazioneDB.Scrittura Then


            ElseIf XTipoOperazione = enum_TipoOperazioneDB.Modifica Or XTipoOperazione = enum_TipoOperazioneDB.Lettura Then

                '##############################################################
                '#####  Se sono in MODIFICA carico i dati  ####################
                '##############################################################


            End If
        End If


        ' Disabilito tutti i controlli se in Lettura
        If XTipoOperazione = enum_TipoOperazioneDB.Lettura Then
            UtenteAbilitato_Modifica = False
        End If

    End Sub

    Private Function Carica_Impostazioni_Materie_Prime(ByRef impDict As Dictionary(Of String, Object)) As String

        'leggo e Jsonizzo le impostazioni
        Dim obj As New AgronicaCoreUtentiBIZ.Utenti_Impostazioni_R
        impDict = obj.LeggiOpzioni_MateriePrime(objParametri_Utenti)

        Return JsonConvert.SerializeObject(impDict, Newtonsoft.Json.Formatting.None)

    End Function



    '##########################################################################################################################################
    Private Sub AnnullaTutto(ByVal sender As Object, ByVal e As System.EventArgs)

        Dim objParametriAgenda As New ParametriAgenda

        Dim TargetUrl As String
        TargetUrl = Master.TrovaRedirectCorretto(False, enum_PagineGiasOnline.MenuAnagraficaProdotti, objParametriAgenda, , Qs_Visibilita)

        Response.Redirect(TargetUrl)

    End Sub


    Private Sub Prodotto_Edit_Init(sender As Object, e As System.EventArgs) Handles Me.Init

        AddHandler CType(Me.Master, AgendaBootstrap).ImgBtn_AnnullaTutto.Click, AddressOf Me.AnnullaTutto
        CType(Me.Master, AgendaBootstrap).flag_MostraBtnIndietro = True

    End Sub


    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Sub RedirectToMenuAnagrafica(ByVal targetUrl As String)

        HttpContext.Current.Response.Redirect(targetUrl)

    End Sub

    <WebMethod(EnableSession:=True)>
    Public Shared Function EditProdotto(ByVal piva As String, ByVal elem_cod As Integer, ByVal mat_cod As Integer, ByVal duplica As Boolean, ByVal proprietario As Boolean, ByVal sa_cod As Integer) As RispostaStandard
        Return Prodotto_Edit_UC.EditProdotto(piva, mat_cod, elem_cod, duplica, proprietario, sa_cod)
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function InfoProdotto(ByVal piva As String, ByVal elem_cod As Integer, ByVal mat_cod As Integer) As RispostaStandard
        Return Prodotto_Edit_UC.InfoProdotto(piva, mat_cod, elem_cod)
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Leggi_XddlDittadiProvenienza() As RispostaStandard
        Return Prodotto_Edit_UC.Leggi_XddlDittadiProvenienza()
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Leggi_XddlSpecieAnimali() As RispostaStandard
        Return Prodotto_Edit_UC.Leggi_XddlSpecieAnimali()
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Leggi_XddlIndirizzoProduttivo(ByVal Gen_Cod As Integer, ByVal Spe_Cod As Integer) As RispostaStandard
        Return Prodotto_Edit_UC.Leggi_XddlIndirizzoProduttivo(Gen_Cod, Spe_Cod)
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Leggi_XddlTipologieSementi() As RispostaStandard
        Return Prodotto_Edit_UC.Leggi_XddlTipologieSementi()
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function CaricaGrigliaStoricoPrezzi(ByVal piva As String, ByVal elem_cod As Integer, ByVal mat_cod As Integer, ByVal pro_cod As Integer) As RispostaStandard
        Return Prodotto_Edit_UC.CaricaGrigliaStoricoPrezzi(piva, elem_cod, mat_cod, pro_cod)
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Leggi_Unita_Di_Misura(ByVal piva As String, ByVal elem_cod As Integer, ByVal mat_cod As Integer, ByVal sa_cod As Integer) As RispostaStandard
        Return Prodotto_Edit_UC.Leggi_Unita_Di_Misura(piva, elem_cod, mat_cod, sa_cod)
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Leggi_XddlGruppoVarietale(ByVal Veg_Cod As Integer) As RispostaStandard
        Return Prodotto_Edit_UC.Leggi_GruppoVarietale(Veg_Cod)
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Leggi_XddlVarietaColturale(ByVal piva As String, ByVal Veg_cod As Integer, ByVal Cul_cod_da_modificare As Integer) As RispostaStandard
        Return Prodotto_Edit_UC.Leggi_VarietaColturale(piva, Veg_cod, Cul_cod_da_modificare)
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function CaricaGrigliaCalibri(ByVal piva As String, ByVal mat_cod As Integer, ByVal sa_cod As Integer) As RispostaStandard
        Return Prodotto_Edit_UC.CaricaGrigliaCalibri(piva, mat_cod, sa_cod)
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Leggi_CalibriFrutti() As RispostaStandard
        Return Prodotto_Edit_UC.Leggi_CalibriFrutti()
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function CaricaGrigliaIndici(ByVal piva As String, ByVal mat_cod As Integer, ByVal sa_cod As Integer) As RispostaStandard
        Return Prodotto_Edit_UC.CaricaGrigliaIndici(piva, mat_cod, sa_cod)
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Leggi_IndiciMaturita(ByVal Veg_Cod As Integer) As RispostaStandard
        Return Prodotto_Edit_UC.Leggi_IndiciMaturita(Veg_Cod)
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Leggi_XddlProdottoBase(ByVal piva As String, ByVal elem_cod As Integer, ByVal veg_cod As Integer, ByVal cul_cod As Integer, ByVal regolamento As Integer) As RispostaStandard
        Return Prodotto_Edit_UC.Leggi_XddlProdottoBase(piva, elem_cod, veg_cod, cul_cod, regolamento)
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function CaricaGrigliaTraduzioni(ByVal piva As String, ByVal elem_cod As Integer, ByVal mat_cod As Integer) As RispostaStandard
        Return Prodotto_Edit_UC.CaricaGrigliaTraduzioni(piva, elem_cod, mat_cod)
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Leggi_XddlLingua() As RispostaStandard
        Return Prodotto_Edit_UC.Leggi_XddlLingua()
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Leggi_LastCod_Articolo(ByVal piva As String) As RispostaStandard
        Return Prodotto_Edit_UC.Leggi_LastCod_Articolo(piva)
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Leggi_XddlIVACompensazione() As RispostaStandard
        Return Prodotto_Edit_UC.Leggi_XddlIVACompensazione()
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Cambia_Codice_Articolo_Codice_Esterno_InBaseAFlag(ByVal piva As String) As RispostaStandard
        Return Prodotto_Edit_UC.Cambia_Codice_Articolo_Codice_Esterno_InBaseAFlag(piva)
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function ControlliPrimaDelSubmit(ByVal model As String, ByVal importato As Boolean) As RispostaStandard

        Lingua.Gias_InizializzaCultura_DaSession()

        Return Prodotto_Edit_UC.ControlliPrimaDelSubmit(model, importato)
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Submit(ByVal model As String) As RispostaStandard
        Return Prodotto_Edit_UC.Submit(model)
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Leggi_Prodotti_Extra_Privata(
                                                         ByVal piva As String,
                                                         ByVal elem_cod As Integer,
                                                         ByVal mat_cod As Integer,
                                                         ByVal pro_cod As Integer) As RispostaStandard

        Return Prodotto_Edit_UC.Leggi_Prodotti_Extra_Privata(piva, elem_cod, mat_cod, pro_cod)

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Redirect_Prodotto(ByVal tipooperazione As Integer,
                                            ByVal piva As String,
                                            ByVal mat_cod As String,
                                            ByVal elem_cod As String,
                                            ByVal prodotto_des As String,
                                            ByVal pro_cod As String,
                                            ByVal duplica As Boolean,
                                            ByVal proprietario As Boolean,
                                            ByVal sa_cod As String,
                                            ByVal isalias As Boolean) As RispostaStandard

        Return Prodotto_Edit_UC.Redirect_Prodotto(tipooperazione, piva, mat_cod, elem_cod, prodotto_des, pro_cod, duplica, proprietario, sa_cod, isalias)
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Leggi_XddlRazzaAnimale(ByVal Gen_Cod As Integer, ByVal Spe_Cod As Integer) As RispostaStandard
        Return Prodotto_Edit_UC.Leggi_XddlRazzaAnimale(Gen_Cod, Spe_Cod)
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Leggi_XddlCategoriaRisorsa(ByVal piva As String, ByVal Tipo_Classe As Integer) As RispostaStandard
        Return Prodotto_Edit_UC.Leggi_CategoriaRisorsa(piva, Tipo_Classe)
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Leggi_XddlLineaProduzione(ByVal piva As String, ByVal Linea_Classe_Cod As Integer) As RispostaStandard
        Return Prodotto_Edit_UC.Leggi_LineaProduzione(piva, Linea_Classe_Cod)
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Leggi_XddlFinalitaProduttiva(ByVal grfi_cod As Integer, ByVal veg_cod As Integer) As RispostaStandard
        Return Prodotto_Edit_UC.Leggi_FinalitaProduttiva(grfi_cod, veg_cod)
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Leggi_XddlUnitaMisuraDefault(ByVal elem_cod As Integer) As RispostaStandard
        Return Prodotto_Edit_UC.Leggi_UnitaMisuraDefault(elem_cod)
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Leggi_XddlUnitaMisuraBene() As RispostaStandard
        Return Prodotto_Edit_UC.Leggi_UnitaMisuraBene()
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Leggi_XddlConfezioni() As RispostaStandard
        Return Prodotto_Edit_UC.Leggi_CodiciImballaggio()
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Leggi_Modulo_Anagrafe(ByVal piva As String) As RispostaStandard
        Return Prodotto_Edit_UC.Leggi_Modulo_Anagrafe(piva)
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Leggi_XddlTipo_Default() As RispostaStandard
        Return Prodotto_Edit_UC.Leggi_Tipo_Default()
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Default_ParametriQualitativi_Dati_Tecnici(ByVal cal_cod As Integer) As RispostaStandard
        Return Prodotto_Edit_UC.Carica_Default_ParametriQualitativi_Dati_Tecnici(cal_cod)
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function ParametroQualGestitiXCheckBoxBeniConf(ByVal piva As String) As RispostaStandard
        Return Prodotto_Edit_UC.ParametroQualGestitiXCheckBoxBeniConf(piva)
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Controlla_Se_Prodotto_Movimentato(ByVal elem_cod As Integer, ByVal mat_cod As Integer, ByVal pro_cod As Integer, ByVal mat_cod_alias As Integer) As RispostaStandard
        Return Prodotto_Edit_UC.Controlla_Se_Prodotto_Movimentato(elem_cod, mat_cod, pro_cod, mat_cod_alias)
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function RicercaFiltriBeniConfezVeg(ByVal piva As String, ByVal mat_Cod As Integer, ByVal tabella_Cod As Integer) As RispostaStandard
        Return Prodotto_Edit_UC.RicercaFiltriBeniConfezVeg(piva, mat_Cod, tabella_Cod)
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Cancella_Prodotto(ByVal model As String, ByVal mat_cod_referenza As Integer, ByVal ParametroQual As Integer, ByVal Sa_Cod As Integer) As RispostaStandard
        Return Prodotto_Edit_UC.Cancella_Prodotto(model, mat_cod_referenza, ParametroQual, Sa_Cod)
    End Function

    'Controlli Cancellazione Prodotto
    <WebMethod(EnableSession:=True)>
    Public Shared Function Controlli_Cancella_Materia_Prima(ByVal elem_cod As Integer, ByVal mat_cod As Integer) As RispostaStandard
        Return Prodotto_Edit_UC.Controlli_Cancella_Materia_Prima(elem_cod, mat_cod)
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function CaricaCategorieMagazzinoXUtente() As RispostaStandard
        Return Prodotto_Edit_UC.CaricaCategorieMagazzinoXUtente()
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Leggi_ImpostazioneUtente() As RispostaStandard
        Return Prodotto_Edit_UC.Leggi_ImpostazioneUtente()
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Leggi_Contatti_Clienti_Fornitori(ByVal piva As String) As RispostaStandard
        Return Prodotto_Edit_UC.Leggi_Contatti_Clienti_Fornitori(piva)
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Imposta_Default_UDM_GridStoricoPrezzi(ByVal elem_cod As Integer, Cod_BancheDati As Integer) As RispostaStandard
        Return Prodotto_Edit_UC.Imposta_Default_UDM_GridStoricoPrezzi(elem_cod, Cod_BancheDati)
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function ComponiMessaggioDiAvvisoCancellazioneProdotto(ByVal piva As String, ByVal elem_cod As Integer, ByVal mat_cod As Integer, ByVal sa_cod As Integer, ByVal isalias As Boolean) As RispostaStandard
        Return Prodotto_Edit_UC.ComponiMessaggioDiAvvisoCancellazioneProdotto(piva, elem_cod, mat_cod, sa_cod, isalias)
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function CaricaGrigliaAlias(ByVal piva As String, ByVal mat_cod As Integer, ByVal elem_cod As Integer, ByVal sa_cod As Integer) As RispostaStandard
        Return Prodotto_Edit_UC.CaricaGrigliaAlias(piva, mat_cod, elem_cod, sa_cod)
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Leggi_XddlAlias(ByVal piva As String, ByVal elem_cod As Integer, ByVal sa_cod As Integer) As RispostaStandard
        Return Prodotto_Edit_UC.Leggi_XddlAlias(piva, elem_cod, sa_cod)
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Controlla_Se_Alias_Associato_a_Materia_Prima(ByVal piva As String, ByVal mat_cod_alias As Integer) As RispostaStandard
        Return Prodotto_Edit_UC.Controlla_Se_Alias_Associato_a_Materia_Prima(piva, mat_cod_alias)
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Leggi_Gruppi_Merce(ByVal piva As String) As RispostaStandard
        Return Prodotto_Edit_UC.Leggi_Gruppi_Merce(piva)
    End Function

End Class


