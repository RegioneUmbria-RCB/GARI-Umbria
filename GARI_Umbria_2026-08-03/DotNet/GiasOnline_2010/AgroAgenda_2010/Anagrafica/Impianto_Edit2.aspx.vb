Imports System.Web.Services
Imports AgronicaCoreAnagrafeBIZ
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreModello.ParametriAgenda_Temp
Imports AgronicaCoreUtentiDAL
Imports AgronicaCoreUtility
Imports AgronicaCoreVarieBIZ
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports AgroAgenda_2010.Resources
Imports AgronicaCoreGestioneRichieste

Public Class Impianto_Edit2
    Inherits System.Web.UI.Page

#Region "Default"
    Private Const DEFAULT_SPECIE As String = ""
    Private Const DEFAULT_CODICITERRENO As String = ""
    Private Const DEFAULT_IMPIRRIGAZIONE As Integer = -1
    Private Const DEFAULT_PROVENIENZA_SEME As Integer = 0
    Private Const DEFAULT_SEMINA_TRAPIANTO As String = "-1"
    Private Const DEFAULT_DETT_VARIETA_PERSONALIZZATO As String = ""
    Private Const DEFAULT_DISCIPLINARE As String = ""
    Private Const DEFAULT_CAPITOLATO_PRIVATO As String = ""
    Private Const DEFAULT_LICENZA_COLTIVAZIONE As String = ""
    Private Const DEFAULT_ORG_REFERENTE As String = ""
    Private Const DEFAULT_REGOLAMENT_CONC As Integer = 0
    Private Const DEFAULT_REGOLAMENTO As Integer = 1
    Private Const DEFAULT_FINALITA As Integer = -1
    Private Const DEFAULT_CULTIVAR As Integer = 0
    Private Const DEFAULT_GRVA As Integer = 0
    Private Const DEFAULT_PORTTINNESTO As Integer = -1
    Private Const DEFAULT_FORMA_ALLEVAMENTO As Integer = -1
    Private Const DEFAULT_COPERTURA As Integer = -1

#End Region

    Dim objParametri_Server, objParametri_Utenti As AgronicaCoreParametri
    Dim objParametriAgenda As ParametriAgenda

    Public Operazione As Integer
    Dim Operazione_Contatti As Integer

    ''----- Gestione Querystring
    'Dim Qs_Key As String
    'Dim Qs_Operazione As String
    Dim Qs_Piva As String
    'Dim Qs_PivaPadre As String
    Dim Qs_PivaNuova As String
    'Dim Qs_PaginaRitorno As String
    Dim Qs_Visibilita As Integer = 0

    Public permessi As PermessiUtente
    Public objImpianto As New JObject()
    Public objAppezzamento As New JObject()
    Public objPermessi_IAF As String

    Dim xPiva As String
    Dim xSa_Cod As String
    Dim xAppezza As String
    Dim xId_Imp As String
    Dim xCampo_Cod As String

    Dim xValiditaInizio As Date
    Dim xValiditaFine As Date

    Dim BaseCode As Integer
    Dim TopCode As Integer


    Public jsImpianti As String
    Public jsCodici As String
    Public jsParticelle As String
    Public permessiStr As String
    Public cmb_Codici As String
    Public cmb_CodiciParticelle As String
    Public cmb_Particelle As String
    Public AlgoritmoCodifica As String
    Public ReplicaGIAS As String
    Public DefaultFinalita As String
    Public DefaultRegolamento As String

    Dim StrCodiciImpianto As String

    Public Sub New()

    End Sub

    Protected Overrides Sub InitializeCulture()
        MyBase.InitializeCulture()
        Lingua.Gias_InizializzaCultura_DaSession()

    End Sub

#Region "OldVersion"

    '##########################################################################################################################################
    Private Sub AnnullaTutto(ByVal sender As Object, ByVal e As System.EventArgs)

        ' Pulisco la Sessione
        'HttpContext.Current.Session("dt_Padri") = Nothing
        'HttpContext.Current.Session("dt_Codici") = Nothing

        ' Pulisco il valore per visualizzare la lista completa di impianti nel menu principale
        objParametriAgenda.Sa_Cod = 0
        objParametriAgenda.Appezza = 0

        Dim TargetUrl As String
        TargetUrl = Master.TrovaRedirectCorretto(False, enum_PagineGiasOnline.MenuAnagrafica, objParametriAgenda, , Qs_Visibilita)

        Response.Redirect(TargetUrl)

    End Sub

    Private Sub Impianto_Edit_OLD_Init(sender As Object, e As System.EventArgs) Handles Me.Init

        ' VAnni: 2/10/2017: a seguito di AgroMaseterPage
        'AddHandler CType(Page.Master.FindControl("ImgBtn_AnnullaTutto"), ImageButton).Click, AddressOf Me.AnnullaTutto
        AddHandler CType(Me.Master, AgendaBootstrap).ImgBtn_AnnullaTutto.Click, AddressOf Me.AnnullaTutto
        Master.flag_MostraBtnIndietro = True
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        permessi = New PermessiUtente()
        permessiStr = Newtonsoft.Json.JsonConvert.SerializeObject(permessi)
        Master.flag_pag_Anagrafica = True
        Master.flag_MostraBtnIndietro = True
        '----- Verifico che l'utente sia autenticato
        If Session("ASG_Utente_Username") = "" Then
            Response.Redirect("~/Custom500.aspx")
        End If
        If Request.QueryString("visibilita") IsNot Nothing AndAlso IsNumeric(Request.QueryString("visibilita")) Then
            Qs_Visibilita = CInt(Request.QueryString("visibilita"))
        End If

        'Se Qs_Visibilita = 0 è stato aperto dal Menu Anagrafe generale e quindi utilizzo la versione standard della grafica
        If Qs_Visibilita = 0 Then
            Master.Master_versione = VERSIONE_MASTER_DEFAULT
            Master.Header_versione = VERSIONE_HEADER_DEFAULT
        End If

        '---
        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))
        objParametri_Utenti = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))
        '---

        objParametriAgenda = New ParametriAgenda
        xPiva = objParametriAgenda.Piva
        xSa_Cod = objParametriAgenda.Sa_Cod
        xAppezza = objParametriAgenda.Appezza
        xId_Imp = objParametriAgenda.Id_Imp

        Select Case objParametriAgenda.Tipo_Operazione
            Case enum_TipoOperazioneDB.Scrittura
                Master.Lbl_Titolo.Text = DirectCast(GetLocalResourceObject("CreazioneNuovoImpianto"), String)
                'objParametriAgenda.Sa_Cod = 0
                'xId_Imp = 0
            Case enum_TipoOperazioneDB.Lettura
                Master.Lbl_Titolo.Text = DirectCast(GetLocalResourceObject("LetturaImpianto"), String)
                xId_Imp = objParametriAgenda.Id_Imp
            Case enum_TipoOperazioneDB.Modifica
                Master.Lbl_Titolo.Text = DirectCast(GetLocalResourceObject("ModificaImpianto"), String)
                xId_Imp = objParametriAgenda.Id_Imp
        End Select

        Operazione = objParametriAgenda.Tipo_Operazione

        HttpContext.Current.Session("operazione") = Operazione

        '==================================
        '======= VERIFICA PERMESSI ========
        '==================================

        Dim UtenteAbilitato_Lettura As Boolean = False
        Dim UtenteAbilitato_Modifica As Boolean = False
        UtenteAbilitato_Lettura = permessi.getPermesso(enum_Security_Attivita.Anagrafica_Impianto).Lettura
        UtenteAbilitato_Modifica = permessi.getPermesso(enum_Security_Attivita.Anagrafica_Impianto).Scrittura
        Session("UtenteAbilitato_Modifica") = UtenteAbilitato_Modifica
        Session("UtenteAbilitato_Lettura") = UtenteAbilitato_Lettura


        If Not UtenteAbilitato_Modifica Then
            'Non ho i permessi per modificare l'impianto
            Response.Redirect("../MenuAnagrafica/Menubs_anagrafica.aspx")
            Exit Sub
        End If

        ' impostazioni default per impianto
        Dim objImpostazioni_Utenti As New Utenti_Impostazioni_Read
        DefaultFinalita = objImpostazioni_Utenti.Impostazione_Valore_From_Impostazione_Cod_Utente_Poi_SuperUser(enum_Impostazioni_Utenti.UTENTE_COD_FINALITA, objParametri_Utenti)
        DefaultRegolamento = objImpostazioni_Utenti.Impostazione_Valore_From_Impostazione_Cod_Utente_Poi_SuperUser(enum_Impostazioni_Utenti.UTENTE_COD_REGOLAMENTO, objParametri_Utenti)

        Dim objAppCodici As New AgronicaCoreAnagrafeDAL.Appezzamento_Codici_R
        Dim DtAppCodici As DataTable

        Dim prm As Boolean = New AgronicaCoreUtentiDAL.Utenti_Permessi_R().Controlla_Permessi_Utente(
                                    Session("ASG_Utente_Username"), Session("ASG_IdServizio"),
                                    enum_Security_Attivita.SupportoDecisioni_VerificaConformitaIAF,
                                    enum_Security_Operazione.Modifica,
                                    Date.Now, "", objParametri_Utenti)


        objPermessi_IAF = prm.ToString.ToLower

        objImpianto.Add(New JProperty("piva", xPiva))
        objImpianto.Add(New JProperty("sa_cod", xSa_Cod))
        objImpianto.Add(New JProperty("appezza", xAppezza))
        If Operazione = enum_TipoOperazioneDB.Scrittura Then
            objImpianto.Add(New JProperty("id_reg", 0))
        Else
            objImpianto.Add(New JProperty("id_reg", xId_Imp))
        End If

        Dim objCentriAz As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read
        objImpianto.Add(New JProperty("sa_nome", objCentriAz.SaNome_from_SaCod(xPiva, xSa_Cod, objParametri_Server)))

        Dim objAppezza As New AgronicaCoreAnagrafeDAL.Appezzamento_Read
        objImpianto.Add(New JProperty("appezza_des", objAppezza.AppezzamentoNome_from_Appezza(xPiva, xSa_Cod, xAppezza, objParametri_Server)))

        Dim campo_des = ""
        Dim campo_cod = 0
        Dim centro_data_inizio = AGRODATAINIZIO
        Dim centro_data_fine = AGRODATAFINE
        Dim appezza_data_inizio = AGRODATAINIZIO
        Dim appezza_data_fine = AGRODATAFINE
        Dim appezza_sup_app As Double = 0

        Dim Dt_Appezzamento As New DataTable
        Dt_Appezzamento = objAppezza.Recupera_Dati_Appezzamento(xPiva, xSa_Cod, xAppezza, "", "", objParametri_Server)
        If Dt_Appezzamento IsNot Nothing AndAlso Dt_Appezzamento.Rows.Count > 0 Then
            campo_des = Dt_Appezzamento.Rows(0).Item("Campo_Des")
            If Dt_Appezzamento.Rows(0).Item("Campo_Cod") <> 0 Then
                xCampo_Cod = CStr(Dt_Appezzamento.Rows(0).Item("Campo_Cod"))
                campo_cod = xCampo_Cod
                Dim Dt_Centro As New DataTable

                Dim objImpresaR As New AgronicaCoreAnagrafeDAL.Imprese_Read
                Dt_Centro = objImpresaR.Recupera_Dati_CentroAziendale(xPiva, xSa_Cod, AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_Server)

                If Dt_Centro IsNot Nothing AndAlso Dt_Centro.Rows.Count > 0 Then
                    centro_data_inizio = Dt_Centro.Rows(0).Item("Validita_Inizio")
                    centro_data_fine = Dt_Centro.Rows(0).Item("Validita_Fine")
                End If

            Else
                xCampo_Cod = "0"
            End If

            InizioAppezzamento.Value = Dt_Appezzamento.Rows(0).Item("Validita_Inizio")
            FineAppezzamento.Value = Dt_Appezzamento.Rows(0).Item("Validita_Fine")

            appezza_data_inizio = Dt_Appezzamento.Rows(0).Item("Validita_Inizio")
            appezza_data_fine = Dt_Appezzamento.Rows(0).Item("Validita_Fine")
            appezza_sup_app = Dt_Appezzamento.Rows(0).Item("SUP_APP")

        End If

        objImpianto.Add(New JProperty("campo_des", campo_des))
        objImpianto.Add(New JProperty("campo_cod", campo_cod))

        objImpianto.Add(New JProperty("centro_data_inizio", centro_data_inizio))
        objImpianto.Add(New JProperty("centro_data_fine", centro_data_fine))

        objImpianto.Add(New JProperty("appezza_data_inizio", appezza_data_inizio))
        objImpianto.Add(New JProperty("appezza_data_fine", appezza_data_fine))


        'carico la combo dei codici
        Dim objcodAn As New AgronicaCoreAnagrafeDAL.Codice_Anagrafe_R
        StrCodiciImpianto = objcodAn.Filtro_Codici_Anagrafe(4, 3, 2, objParametri_Server)
        'Elimino il codice Titolo Possesso, Metodo Produzione, Magazzino Conferimento,
        'Organismo Referente, Capitolato Privato e Dettaglio Specie Personalizzato 
        'perché già presenti
        StrCodiciImpianto = Replace(StrCodiciImpianto, " Or Codice = " & CStr(enum_CodiciAnagrafe.TitoloPossesso), "")
        StrCodiciImpianto = Replace(StrCodiciImpianto, "Codice = " & CStr(enum_CodiciAnagrafe.TitoloPossesso) & " Or ", "")
        StrCodiciImpianto = Replace(StrCodiciImpianto, " Or Codice = " & CStr(enum_CodiciAnagrafe.MetodoDiProduzione), "")
        StrCodiciImpianto = Replace(StrCodiciImpianto, "Codice = " & CStr(enum_CodiciAnagrafe.MetodoDiProduzione) & " Or ", "")
        StrCodiciImpianto = Replace(StrCodiciImpianto, " Or Codice = " & CStr(enum_CodiciAnagrafe.Riferimento_Trasferimento_Dati), "")
        StrCodiciImpianto = Replace(StrCodiciImpianto, "Codice = " & CStr(enum_CodiciAnagrafe.Riferimento_Trasferimento_Dati) & " Or ", "")
        StrCodiciImpianto = Replace(StrCodiciImpianto, " Or Codice = " & CStr(enum_CodiciAnagrafe.Organismo_Referente), "")
        StrCodiciImpianto = Replace(StrCodiciImpianto, "Codice = " & CStr(enum_CodiciAnagrafe.Organismo_Referente) & " Or ", "")
        StrCodiciImpianto = Replace(StrCodiciImpianto, " Or Codice = " & CStr(enum_CodiciAnagrafe.Capitolato_Privato), "")
        StrCodiciImpianto = Replace(StrCodiciImpianto, "Codice = " & CStr(enum_CodiciAnagrafe.Capitolato_Privato) & " Or ", "")
        StrCodiciImpianto = Replace(StrCodiciImpianto, " Or Codice = " & CStr(enum_CodiciAnagrafe.Zespri_Fasi_Fase), "")
        StrCodiciImpianto = Replace(StrCodiciImpianto, "Codice = " & CStr(enum_CodiciAnagrafe.Zespri_Fasi_Fase) & " Or ", "")
        StrCodiciImpianto = Replace(StrCodiciImpianto, " Or Codice = " & CStr(enum_CodiciAnagrafe.Dettaglio_Specie_Personalizzato), "")
        StrCodiciImpianto = Replace(StrCodiciImpianto, "Codice = " & CStr(enum_CodiciAnagrafe.Dettaglio_Specie_Personalizzato) & " Or ", "")
        StrCodiciImpianto = Replace(StrCodiciImpianto, " Or Codice = " & CStr(enum_CodiciAnagrafe.Magazzino_Conferimento), "")
        StrCodiciImpianto = Replace(StrCodiciImpianto, "Codice = " & CStr(enum_CodiciAnagrafe.Magazzino_Conferimento) & " Or ", "")
        StrCodiciImpianto = Replace(StrCodiciImpianto, " Or Codice = " & CStr(enum_CodiciAnagrafe.Data_Inizio_Portinnesto), "")
        StrCodiciImpianto = Replace(StrCodiciImpianto, "Codice = " & CStr(enum_CodiciAnagrafe.Data_Inizio_Portinnesto) & " Or ", "")


        Dim obj_Codici As New JArray
        Dim objCodiceAnagrafe As New AgronicaCoreAnagrafeDAL.Codice_Anagrafe_R
        Dim DTCod = objCodiceAnagrafe.Leggi(
            0,
            "",
            enumSelezioneVariabile.Selezione_TabellaCompleta,
            StrCodiciImpianto,
            "",
            objParametri_Server
            )

        For Each rowCod In DTCod.Rows
            Dim jCod As New JObject
            jCod.Add(New JProperty("value", rowCod("codice")))
            jCod.Add(New JProperty("text", rowCod("descrizione")))
            obj_Codici.Add(jCod)
        Next

        Dim codice_impianto As String = ""
        Dim obj_imp_cod As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Codici_R
        AlgoritmoCodifica = Replica_GIAS.LeggiAlgoritmoCodifica(xPiva, objParametri_Server)
        Dim ReplicaPiva = Replica_GIAS.VerificaConfigurazione(xPiva, xSa_Cod, objParametri_Server)

        If ReplicaPiva <> "" Then
            Dim objanag As New AgronicaCoreAnagrafeDAL.Imprese_Read
            ReplicaGIAS = objanag.RagSoc_from_Piva(ReplicaPiva, objParametri_Server)
        End If

        cmb_Codici = obj_Codici.ToString

        CaricaCombo_ParticelleCatastali_xAppezzamento(xPiva, xSa_Cod, xAppezza, objParametri_Server)

        'metto la stinga restituita dal componente nel formato "cod,cod,cod.."
        'per utilizzarla quando devo caricare i dati
        StrCodiciImpianto = Replace(StrCodiciImpianto, " Or Codice = ", ",")
        StrCodiciImpianto = Right(StrCodiciImpianto, StrCodiciImpianto.Length - 9)


        Dim obj_CodiciParticelle As New JArray
        Dim DTCodPart = objCodiceAnagrafe.Leggi(
            0,
            "",
            enumSelezioneVariabile.Selezione_TabellaCompleta,
            " Codice = 1078 ",
            "",
            objParametri_Server
            )

        For Each rowCod In DTCodPart.Rows
            Dim jCod As New JObject
            jCod.Add(New JProperty("value", rowCod("codice")))
            jCod.Add(New JProperty("text", rowCod("descrizione")))
            obj_CodiciParticelle.Add(jCod)
        Next

        cmb_CodiciParticelle = obj_CodiciParticelle.ToString


        '##############################################################
        '#####  Se sono in MODIFICA carico i dati  ####################
        '##############################################################

        Dim sup_Imp As Double = 0
        Dim impianto_data_inizio As Date = Nothing
        Dim impianto_data_fine As Date = Nothing

        Dim CulCod As Integer = 0
        Dim Veg_Cod As Integer? = 0
        Dim Gru_Cod As Integer? = 0
        Dim Grfi_cod As Integer? = 0
        Dim grva_cod As Integer? = 0

        Dim port_cod As Integer? = Nothing
        Dim foral_cod As Integer? = Nothing
        Dim Imp_Cod2 As Integer? = Nothing
        Dim ProvenienzaSeme As Integer? = Nothing
        Dim cop_cod As Integer? = Nothing
        Dim LblSubAppezza As Double? = Nothing
        Dim MovimentiPresenti = False
        Dim TrattamentiPresenti = False
        Dim id_cod_terreno As Integer? = 0
        Dim setup_cod As String = "-1"
        Dim tecn_cod As Integer? = Nothing
        Dim su_cod As Integer? = Nothing
        Dim tra_fila_m As Double? = Nothing
        Dim su_fila_m As Double? = Nothing
        Dim interbina As Double? = Nothing
        Dim Chkinterbina As Boolean = False
        Dim germinabilita As Double? = 100
        Dim codiceZona As String = ""
        Dim cop_data_inizio As Date = AGRODATAINIZIO
        Dim cop_data_fine As Date = AGRODATAFINE
        Dim chkConsociazione_enabled = False
        Dim chkConsociazione_checked = False
        Dim dettaglio_varieta_personalizzato As String = Nothing
        Dim ChkCoverCrops = False
        Dim ChkMonitorato = False
        Dim impianto_ibrido As String = Nothing

        Dim Unita_Vitata As String = Nothing
        Dim Resa_Prevista As Double? = Nothing
        Dim Resa_Corretta As Double? = Nothing

        Dim CodBMBDBT_M As String = Nothing
        Dim CodBMBDBT_F As String = Nothing
        Dim Genetica_M As String = Nothing
        Dim Genetica_F As String = Nothing
        Dim OffType_M As String = Nothing
        Dim OffType_F As String = Nothing
        Dim DistanzaTraFila_F As String = Nothing
        Dim DistanzaSuFila_F As String = Nothing

        Dim tagliato_tuberi As String = Nothing
        Dim partiTuberi As String = Nothing

        Dim ValiditaInizio_Distinta As Date = Nothing
        Dim ValiditaFine_Distinta As Date = Nothing
        Dim id_consociazione As Integer = 0
        Dim PianteImpianto2 As Integer? = Nothing
        Dim semina_prevista As Date = Nothing
        Dim fioritura_prevista As Date = Nothing
        Dim raccolta_prevista As Date = Nothing
        Dim Resa1 As Double? = Nothing
        Dim Disciplinare As String = Nothing
        Dim Regolamento As Integer? = Nothing
        Dim RegolamentoConc As Integer? = Nothing
        Dim Stato As Integer? = Nothing
        Dim Lotto As String = Nothing
        Dim dati_Distinte As String = ""
        Dim Data_Inizio_Portinnesto = AGRODATAINIZIO

        Dim Data_Inizio_Innesto = AGRODATAINIZIO
        Dim Data_Inizio_Produzione = AGRODATAINIZIO
        Dim ChkMaschiSesto = False

        Dim CollegamentoPianoConcimazione As Boolean = False
        'Dim destinazioni_presenti As Boolean = False

        If Operazione = enum_TipoOperazioneDB.Scrittura Then

            Dim Dt As New DataTable
            Dim Dt2 As New DataTable
            Dim Dt3 As New DataTable

            jsImpianti = DT_to_Json_Progetti(Dt)
            HttpContext.Current.Session("Dt_Progetti") = Dt

            Dt2.Columns.Add(New DataColumn("Id_Cod", GetType(Integer)))
            Dt2.Columns.Add(New DataColumn("Descrizione", GetType(String)))
            Dt2.Columns.Add(New DataColumn("Val_Cod", GetType(String)))
            Dt2.Columns.Add(New DataColumn("Validita_Inizio", GetType(String)))
            Dt2.Columns.Add(New DataColumn("Validita_Fine", GetType(String)))
            jsCodici = DT_to_Json_Codici(Dt2)
            HttpContext.Current.Session("dt_Codici_Ana") = Dt2


            Dt3.Columns.Add(New DataColumn("Nome", GetType(String)))
            Dt3.Columns.Add(New DataColumn("Val_Cod", GetType(String)))
            jsParticelle = DT_to_Json_Particelle(Dt3)
            HttpContext.Current.Session("DT_Particelle") = Dt3

            If xId_Imp = "null" OrElse xId_Imp = "" Then
                xId_Imp = "0"
            End If

            dati_Distinte = carica_Kendo_Distinte(xPiva, xSa_Cod, xAppezza, xId_Imp, sup_Imp).ToString

            sup_Imp = appezza_sup_app
            LblSubAppezza = appezza_sup_app

            impianto_data_inizio = appezza_data_inizio
            impianto_data_fine = appezza_data_fine

            'Leggo il codice che mi dice se l'appezzamento relativo a questo impianto è biologico oppure no
            DtAppCodici = objAppCodici.Leggi(xPiva, xSa_Cod, xAppezza, 1018, "",
                                             enumSelezioneVariabile.Selezione_TabellaCompleta,
                                             "", "", objParametri_Server)

            'Se l'appezzamento è biologico o in conversione abilito la possibilità
            'di creare impianti consociati
            If DtAppCodici IsNot Nothing AndAlso DtAppCodici.Rows.Count > 0 Then

                If CInt(DtAppCodici.Rows(0).Item("Val_Cod")) = 2 OrElse CInt(DtAppCodici.Rows(0).Item("Val_Cod")) = 3 Then

                    chkConsociazione_enabled = True

                    'Verifico se ci sono già altri impianti attivi e consociati nell'appezzamento
                    If Verifica_Esistenza_Impianti_Consociati(xPiva, xSa_Cod, xAppezza, id_consociazione, impianto_data_inizio, objParametri_Server) Then

                        'Se ci sono metto il check
                        chkConsociazione_checked = True
                    End If

                End If

            End If

        ElseIf Operazione = enum_TipoOperazioneDB.Modifica OrElse Operazione = enum_TipoOperazioneDB.Lettura Then

            Dim dtcodice = obj_imp_cod.Leggi(
                xPiva,
                xSa_Cod,
                xAppezza,
                xId_Imp,
                "",
                enum_CodiciAnagrafe.Codice_Impianto,
                "",
                enumSelezioneVariabile.Selezione_TabellaCompleta,
                "",
                "",
                objParametri_Server
                )

            If dtcodice.Rows.Count > 0 Then
                codice_impianto = dtcodice.Rows(0).Item("val_cod")
            End If

            'Carico i Progetti/Distinte ordinati dal in anno decrescente in modo da avere l'ultima distinta in alto
            Dim objProgetto As New AgronicaCoreAnagrafeDAL.Impresa_Progetti_R
            Dim Dt As DataTable

            Dt = objProgetto.Leggi(
                CStr(xPiva),
                0,
                CInt(9100),
                0,
                CInt(xSa_Cod),
                CInt(xAppezza),
                CInt(xId_Imp),
                0,
                0,
                enumSelezioneVariabile.Selezione_TabellaCompleta,
                "",
                " Imprese_Progetti.Validita_Inizio DESC ",
                objParametri_Server
                )

            jsImpianti = DT_to_Json_Progetti(Dt)
            HttpContext.Current.Session("Dt_Progetti") = Dt


            ' @Paolo
            ' Riempio la distinta dell'ultimo impianto 

            Dim objParametriAgenda As New ParametriAgenda
            Dim Dt_Progetti As DataTable

            'Mi procuro il recordset richiesto
            Dt_Progetti = objProgetto.Leggi(
                CStr(objParametriAgenda.Piva),
                CInt(Dt.Rows(0).Item("Progetto_Cod")),
                "",
                0,
                CInt(objParametriAgenda.Sa_Cod),
                CInt(objParametriAgenda.Appezza),
                CInt(objParametriAgenda.Id_Imp),
                0,
                0,
                AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni,
                "",
                "",
                objParametri_Server
                )

            If Not IsNothing(Dt_Progetti) Then
                ValiditaInizio_Distinta = Dt.Rows(0).Item("Validita_Inizio")
                ValiditaFine_Distinta = Dt.Rows(0).Item("Validita_Fine")
                PianteImpianto2 = CInt(Dt_Progetti.Rows(0).Item("p_ha"))
                semina_prevista = CDate(Dt_Progetti.Rows(0).Item("data_inizio_prevista"))
                fioritura_prevista = CDate(Dt_Progetti.Rows(0).Item("data_fioritura_prevista"))
                raccolta_prevista = CDate(Dt_Progetti.Rows(0).Item("data_fine_prevista"))
                'ResaPrevista = CDbl(Dt_Progetti.Rows(0).Item("giudizio"))
                Resa1 = CDbl(Dt_Progetti.Rows(0).Item("produzione_prevista"))
                Disciplinare = Dt_Progetti.Rows(0).Item("disciplinare_cod") & "/1"
                Regolamento = Dt_Progetti.Rows(0).Item("regolamento_cod")
                RegolamentoConc = Dt_Progetti.Rows(0).Item("regolamento_concimazioni_cod")
                Stato = Dt_Progetti.Rows(0).Item("stato_impianto")
                Lotto = Dt_Progetti.Rows(0).Item("Progetto_Nome")
            End If

            '''''''''''''''''''''''''''''''''''''''''

            Dim Dt2 As New DataTable

            Dim objRegImpianti As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
            Dim DtRegImpianti As DataTable

            DtRegImpianti = objRegImpianti.Leggi(
                xPiva,
                xSa_Cod,
                xAppezza,
                xId_Imp,
                enumSelezioneVariabile.Selezione_JoinCompleta,
                "",
                "",
                objParametri_Server
                )

            If DtRegImpianti IsNot Nothing AndAlso DtRegImpianti.Rows.Count > 0 Then
                ''---------------------
                ''riciclo resa_prevista e resa_corretta impianto per apofruit
                'Try
                '    If Not IsDBNull(DtRegImpianti.Fields("Resa_Prevista")) Then
                '        If Not IsNothing(DtRegImpianti.Fields("Resa_Prevista").Value) Then
                '            If IsNumeric(DtRegImpianti.Fields("Resa_Prevista").Value) Then
                '                Me.Txt_Resa1.Text = DtRegImpianti.Fields("Resa_Prevista").Value
                '            End If
                '        End If
                '    End If
                '    If Not IsDBNull(DtRegImpianti.Fields("Resa_Effettiva")) Then
                '        If Not IsNothing(DtRegImpianti.Fields("Resa_Effettiva").Value) Then
                '            If IsNumeric(DtRegImpianti.Fields("Resa_Effettiva").Value) Then
                '                Me.Txt_Resa2.Text = DtRegImpianti.Fields("Resa_Effettiva").Value
                '            End If
                '        End If
                '    End If


                'Catch ex As Exception
                'End Try

                id_consociazione = CInt(DtRegImpianti.Rows(0).Item("Id_Consociazione"))


                If DtRegImpianti.Rows(0).Item("sup_imp") <> "0" Then
                    sup_Imp = DtRegImpianti.Rows(0).Item("sup_imp")
                    LblSubAppezza = appezza_sup_app 'DtRegImpianti.Rows(0).Item("sup_imp").ToString
                End If


                CulCod = DtRegImpianti.Rows(0).Item("Cul_cod")

                'Imposto le informazioni
                impianto_data_inizio = DtRegImpianti.Rows(0).Item("Validita_Inizio")
                impianto_data_fine = DtRegImpianti.Rows(0).Item("Validita_Fine")

                ' - - - - - - - - - - - - - - - - A N N A      I N I Z I O - - - - - - - - - - - - - - - -
                If Not IsDBNull(DtRegImpianti.Rows(0).Item("Data_Inizio_Portinnesto")) AndAlso IsDate(DtRegImpianti.Rows(0).Item("Data_Inizio_Portinnesto")) Then
                    Data_Inizio_Portinnesto = DtRegImpianti.Rows(0).Item("Data_Inizio_Portinnesto")
                End If

                If Not IsDBNull(DtRegImpianti.Rows(0).Item("Data_Inizio_Innesto")) AndAlso IsDate(DtRegImpianti.Rows(0).Item("Data_Inizio_Innesto")) Then
                    Data_Inizio_Innesto = DtRegImpianti.Rows(0).Item("Data_Inizio_Innesto")
                End If

                If Not IsDBNull(DtRegImpianti.Rows(0).Item("Data_Inizio_Produzione")) AndAlso IsDate(DtRegImpianti.Rows(0).Item("Data_Inizio_Produzione")) Then
                    Data_Inizio_Produzione = DtRegImpianti.Rows(0).Item("Data_Inizio_Produzione")
                End If

                If Not IsDBNull(DtRegImpianti.Rows(0).Item("Piante_Maschi_InSesto")) Then
                    If DtRegImpianti.Rows(0).Item("Piante_Maschi_InSesto") = 0 Then
                        ChkMaschiSesto = False
                    Else
                        ChkMaschiSesto = True
                    End If
                End If
                ' - - - - - - - - - - - - - - - - A N N A      F I N E - - - - - - - - - - - - - - - - - -

                ' Controllo se è un terreno nudo
                If CulCod = 0 Then

                    ''===== TERRENO NUDO =========================

                    ''Attivo i controlli opportuni
                    ''Call xx_TerrenoNudo_On()

                    ''Attivo il check del terreno nudo
                    'ChkTerrenoNudo.Checked = True

                Else


                    Veg_Cod = DtRegImpianti.Rows(0).Item("Veg_Cod")
                    Gru_Cod = DtRegImpianti.Rows(0).Item("Gru_Cod")



                    'Leggo il codice che mi dice se l'appezzamento relativo a questo impianto è biologico oppure no
                    DtAppCodici = objAppCodici.Leggi(xPiva,
                                         xSa_Cod,
                                         xAppezza,
                                         1018, "",
                                         enumSelezioneVariabile.Selezione_TabellaCompleta,
                                         "", "", objParametri_Server)

                    'Se l'appezzamento è biologico o in conversione abilito la possibilità
                    'di creare impianti consociati
                    If DtAppCodici IsNot Nothing AndAlso DtAppCodici.Rows.Count > 0 Then

                        If CInt(DtAppCodici.Rows(0).Item("Val_Cod")) = 2 OrElse CInt(DtAppCodici.Rows(0).Item("Val_Cod")) = 3 Then

                            chkConsociazione_enabled = True

                            'Verifico se ci sono già altri impianti attivi e consociati nell'appezzamento
                            If Verifica_Esistenza_Impianti_Consociati(xPiva, xSa_Cod, xAppezza, id_consociazione, impianto_data_inizio, objParametri_Server) Then

                                'Se ci sono metto il check
                                chkConsociazione_checked = True
                            End If

                        End If

                    End If

                    '
                    If DtRegImpianti.Rows(0).Item("cover") = 0 Then
                        ChkCoverCrops = False
                    Else
                        ChkCoverCrops = True
                    End If
                    '
                    If DtRegImpianti.Rows(0).Item("monitorato") = 0 Then
                        ChkMonitorato = False
                    Else
                        ChkMonitorato = True
                    End If
                    '
                    If Not IsDBNull(DtRegImpianti.Rows(0).Item("cop_cod")) Then
                        cop_cod = DtRegImpianti.Rows(0).Item("cop_cod")
                    End If

                    If Not IsDBNull(DtRegImpianti.Rows(0).Item("cop_di")) Then
                        cop_data_inizio = DtRegImpianti.Rows(0).Item("cop_di")
                    End If

                    If Not IsDBNull(DtRegImpianti.Rows(0).Item("cop_df")) Then
                        cop_data_fine = DtRegImpianti.Rows(0).Item("cop_df")
                    End If

                    If Not IsDBNull(DtRegImpianti.Rows(0).Item("setup_cod")) Then
                        setup_cod = DtRegImpianti.Rows(0).Item("setup_cod")
                    End If

                    If Not IsDBNull(DtRegImpianti.Rows(0).Item("tecn_cod")) Then
                        tecn_cod = DtRegImpianti.Rows(0).Item("tecn_cod")
                    End If

                    If Not IsDBNull(DtRegImpianti.Rows(0).Item("su_cod")) Then
                        su_cod = DtRegImpianti.Rows(0).Item("su_cod")
                    End If

                    'Se la specie vegetale è 
                    Select Case Veg_Cod

                        Case 6  'la barbabietola da zucchero occorre il sesto impianto

                            'Semina/Trapianto


                        Case 46  'Se patata attivo il pannello tuberi

                            'Pannello_Tuberi.Visible = True

                    End Select

                    If Not IsDBNull(DtRegImpianti.Rows(0).Item("Grfi_cod")) Then
                        Grfi_cod = DtRegImpianti.Rows(0).Item("Grfi_cod")
                    Else
                        Grfi_cod = 0
                    End If

                    CulCod = CInt(DtRegImpianti.Rows(0).Item("cul_cod"))
                    grva_cod = DtRegImpianti.Rows(0).Item("grva_cod_veg")

                    'Select Case DtRegImpianti.Rows(0).Item("grva_cod_veg")
                    '    Case Is < 0 'Ibrido
                    '        ChkVarietaIbrida.Checked = True
                    '    Case Is > 0 'Non Ibrido
                    '        ChkVarietaIbrida.Checked = False
                    '    Case Else
                    '        ChkVarietaIbrida.Checked = False
                    'End Select


                    'Portinnesto
                    port_cod = DtRegImpianti.Rows(0).Item("port_cod")
                    '
                    'Forma Allevamento
                    If IsDBNull(DtRegImpianti.Rows(0).Item("foral_cod")) Then
                        foral_cod = 0
                    Else
                        foral_cod = DtRegImpianti.Rows(0).Item("foral_cod")
                    End If


                    'Impianto Irrigazione
                    Imp_Cod2 = DtRegImpianti.Rows(0).Item("Imp_Cod2")
                    '
                    'Provenienza Seme
                    ProvenienzaSeme = DtRegImpianti.Rows(0).Item("ProvenienzaSeme")

                    ' Unità vitata
                    If Not IsNothing(DtRegImpianti.Rows(0).Item("Unita_Vitata")) Then
                        Unita_Vitata = DtRegImpianti.Rows(0).Item("Unita_Vitata")
                    End If
                    If Not IsNumeric(Unita_Vitata) OrElse CInt(Unita_Vitata) = 0 Then
                        Unita_Vitata = ""
                    End If

                    If Not IsDBNull(DtRegImpianti.Rows(0).Item("Resa_Effettiva")) AndAlso DtRegImpianti.Rows(0).Item("Resa_Effettiva") <> 0 Then
                        Resa_Corretta = DtRegImpianti.Rows(0).Item("Resa_Effettiva")
                    End If
                    If Not IsDBNull(DtRegImpianti.Rows(0).Item("Resa_Prevista")) AndAlso DtRegImpianti.Rows(0).Item("Resa_Prevista") <> 0 Then
                        Resa_Prevista = DtRegImpianti.Rows(0).Item("Resa_Prevista")
                    End If

                End If

                '------------------------------------------------------------------------------------------------------
                '--------- Carico i codici associati all'impianto
                '------------------------------------------------------------------------------------------------------

                Dim objCodici As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Codici_R
                Dim DtCodici As DataTable
                'Dim Dr() As DataRow
                Dim i As Integer

                Dim Id_Cod As Integer
                Dim Val_Cod As String
                Dim StrCodice As String
                Dim StrCodici2 As String = ""
                Dim CodiceAnagrafeDes As String
                Dim FiltroLetturaCodici As String = ""

                'modifica del 27/09/2012: in modifica dell'impianto si perdevano i codici
                'in cui vengono salvate le precessioni colturali

                'Non devono essere cancellati i codici specie-varietà cliente!!!
                FiltroLetturaCodici += " ( " &
                                        " Reg_Impianti_Codici.Id_Cod NOT IN ( " &
                                        CStr(enum_CodiciAnagrafe.Codice_Specie_Agea) & ", " &
                                        CStr(enum_CodiciAnagrafe.Codice_Cultivar_Agea) & ", " &
                                        CStr(enum_CodiciAnagrafe.Coltura_Precedente_1) & ", " &
                                        CStr(enum_CodiciAnagrafe.Coltura_Precedente_2) & ", " &
                                        CStr(enum_CodiciAnagrafe.Coltura_Precedente_3) & ", " &
                                        CStr(enum_CodiciAnagrafe.Coltura_Precedente_4) & " " &
                                        "   ) " &
                                        " )"

                '--------------------------------------------------
                'filtro i codici relativi all'impianto...
                'FILTRO i codici CHIAVE DEI CLIENTI!!!!!!!!!!!!!!!
                '--------------------------------------------------
                'aggiungo al filtro anche l'esclusione dei codici clienti
                '(veniva fatto dopo la select sul dt, ma così escludo già in partenza)
                FiltroLetturaCodici += " AND Reg_Impianti_Codici.progetto_cod = 0  " &
                                   " AND (Reg_Impianti_Codici.id_cod < 2000 OR Reg_Impianti_Codici.id_cod >= 3000 ) "


                DtCodici = objCodici.Leggi(CStr(xPiva),
                                       CInt(xSa_Cod),
                                       CInt(xAppezza),
                                       CInt(xId_Imp),
                                        "",
                                       CInt(Id_Cod),
                                       CStr(Val_Cod),
                                       enumSelezioneVariabile.Selezione_TabellaCompleta,
                                        FiltroLetturaCodici,
                                       "",
                                       objParametri_Server)

                StrCodici2 = ""


                If DtCodici IsNot Nothing AndAlso DtCodici.Rows.Count > 0 Then

                    Dim Dr As DataRow

                    If (IsNothing(HttpContext.Current.Session("dt_Codici_Ana"))) Then
                        Dt2.Columns.Add(New DataColumn("Id_Cod", GetType(Integer)))
                        Dt2.Columns.Add(New DataColumn("Descrizione", GetType(String)))
                        Dt2.Columns.Add(New DataColumn("Val_Cod", GetType(String)))
                        Dt2.Columns.Add(New DataColumn("Validita_Inizio", GetType(String)))
                        Dt2.Columns.Add(New DataColumn("Validita_Fine", GetType(String)))
                    Else
                        Dt2 = HttpContext.Current.Session("dt_Codici_Ana")
                    End If

                    ''--------------------------------------------------
                    ''filtro i codici relativi all'impianto...
                    ''FILTRO i codici CHIAVE DEI CLIENTI!!!!!!!!!!!!!!!
                    ''--------------------------------------------------
                    'Dr = DtCodici.Select("(progetto_cod = 0 AND id_cod < 2000) OR (progetto_cod = 0 AND id_cod >= 3000)")

                    'If Not Dr Is Nothing AndAlso Dr.Length > 0 Then

                    'For i = 0 To Dr.Length - 1
                    For i = 0 To DtCodici.Rows.Count - 1

                        'If (Not IsDBNull(Dr(i).Item("val_cod"))) Then
                        If (Not IsDBNull(DtCodici.Rows(i).Item("val_cod"))) Then

                            Id_Cod = DtCodici.Rows(i).Item("id_cod")
                            Val_Cod = DtCodici.Rows(i).Item("val_cod")
                            CodiceAnagrafeDes = DtCodici.Rows(i).Item("descrizione")

                            'Genero l'XML del singolo nodo
                            Call XML_Codice(enum_CodificaDecodifica.Codifica,
                                        StrCodice,
                                        enum_TipoOperazioneDB.Cancellazione,
                                        Id_Cod,
                                        Val_Cod,
                                        CDate("01/01/1900"),
                                        CDate("31/12/2100"),
                                        BaseCode,
                                        TopCode,
                                        "Impianto")

                            'Inserisco l'XML nella stringa complessiva
                            StrCodici2 = StrCodici2 & StrCodice

                            Select Case Id_Cod

                                Case enum_CodiciAnagrafe.Impianto_Ibrido
                                    If Val_Cod = 0 And DtRegImpianti.Rows(0).Item("grva_cod_veg") >= 0 Then
                                        impianto_ibrido = Val_Cod
                                    Else
                                        impianto_ibrido = Nothing
                                    End If



                                Case enum_CodiciAnagrafe.Impianto_CodiceB_Maschio
                                    CodBMBDBT_M = Val_Cod

                                Case enum_CodiciAnagrafe.Impianto_CodiceB_Femmina
                                    CodBMBDBT_F = Val_Cod

                                Case enum_CodiciAnagrafe.Impianto_Genetica_Maschio
                                    Genetica_M = Val_Cod

                                Case enum_CodiciAnagrafe.Impianto_Genetica_Femmina
                                    Genetica_F = Val_Cod

                                Case enum_CodiciAnagrafe.Impianto_OffType_Maschio
                                    OffType_M = Val_Cod

                                Case enum_CodiciAnagrafe.Impianto_OffType_Femmina
                                    OffType_F = Val_Cod

                                Case enum_CodiciAnagrafe.Impianto_TraFila_Maschio
                                    If Val_Cod <> "" Then
                                        tra_fila_m = Val_Cod
                                    End If

                                Case enum_CodiciAnagrafe.Impianto_TraFila_Femmina
                                    If Val_Cod <> "" Then
                                        DistanzaTraFila_F = Val_Cod
                                    End If

                                Case enum_CodiciAnagrafe.Impianto_SuFila_Maschio
                                    If Val_Cod <> "" Then
                                        su_fila_m = Val_Cod
                                    End If

                                Case enum_CodiciAnagrafe.Impianto_SuFila_Femmina
                                    If Val_Cod <> "" Then
                                        DistanzaSuFila_F = Val_Cod
                                    End If

                                Case enum_CodiciAnagrafe.Impianto_Interbina
                                    If Val_Cod <> "0" AndAlso Val_Cod <> "" Then
                                        interbina = Val_Cod
                                        Chkinterbina = True
                                    End If

                                Case enum_CodiciAnagrafe.Impianto_Germinabilita
                                    germinabilita = Val_Cod

                                Case 3000 To 3999

                                    id_cod_terreno = Id_Cod

                                Case enum_CodiciAnagrafe.Impianto_Taglio_Tuberi_Patate

                                    tagliato_tuberi = Val_Cod

                                Case enum_CodiciAnagrafe.Impianto_Parti_Tuberi_Patate

                                    partiTuberi = Val_Cod

                                Case enum_CodiciAnagrafe.Dettaglio_Specie_Personalizzato

                                    dettaglio_varieta_personalizzato = Val_Cod

                                Case enum_CodiciAnagrafe.CodiceZona

                                    codiceZona = Val_Cod


                                Case Else

                                    'se nella stringa contenente i codici restituita dal componente è presente..
                                    If InStr(StrCodiciImpianto, Id_Cod.ToString) <> 0 Then

                                        'ListCodici.Items.Add(New ListItem(CodiceAnagrafeDes & " = " & Val_Cod, _
                                        '                                  Id_Cod.ToString))

                                        'Creo una nuova riga
                                        Dr = Dt2.NewRow


                                        Dr.Item("Id_Cod") = Id_Cod
                                        'Dr.Item("Descrizione") = objCodiceAnagrafeR.CodiceAnagrafeDes_from_CodiceAnagrafeCod(codice_id, HttpContext.Current.Session("ASG_objParametri_Server"))
                                        Dr.Item("Descrizione") = CodiceAnagrafeDes
                                        Dr.Item("Val_Cod") = Val_Cod

                                        Dr.Item("Validita_Inizio") = DtCodici.Rows(i).Item("Validita_Inizio")

                                        Dr.Item("Validita_Fine") = DtCodici.Rows(i).Item("Validita_Fine")

                                        'Associo alla tabella la nuova riga creata
                                        Dt2.Rows.Add(Dr)


                                    End If

                            End Select

                        End If

                    Next


                    'End If

                    HttpContext.Current.Session("dt_Codici_Ana") = Dt2

                    DtCodici = Nothing

                End If 'DtCodici

                Session("StrXmlCodiciAttuali") = StrCodici2
                dati_Distinte = carica_Kendo_Distinte(xPiva, xSa_Cod, xAppezza, xId_Imp, sup_Imp).ToString

                'Dim DT_Destinazioni As DataTable

                ''ricavo i movimenti di produzione associati all'impianto
                'Dim objmpianto As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
                'DT_Destinazioni = objmpianto.Leggi_Operazioni_Impianti(xPiva, xSa_Cod, xAppezza, xId_Imp, "", "", objParametri_Server)
                'If Not IsNothing(DT_Destinazioni) Then

                '    If DT_Destinazioni.Rows.Count <> 0 Then

                '        destinazioni_presenti = True

                '        'Me.Cmb_GruppoVegetale.Enabled = False
                '        'Me.Cmb_Specie.Enabled = False
                '        'Me.Cmb_Finalita.Enabled = False

                '        ''se il gru_cod è =0 (Terreno Nudo) allora posso modificare la specie dell'impianto
                '        'If GruCod <> 0 Then
                '        '    ChkTerrenoNudo.Enabled = False
                '        'Else
                '        '    ViewState("ChkTerrenoNudo_OLD") = Me.ChkTerrenoNudo.Checked
                '        '    ViewState("Cmb_GruppoVegetale_OLD") = Me.Cmb_GruppoVegetale.SelectedValue
                '        '    ViewState("Cmb_Specie_OLD") = Me.Cmb_Specie.SelectedValue
                '        '    ViewState("Cmb_Finalita_OLD") = Me.Cmb_Finalita.SelectedValue
                '        'End If

                '    End If

                '    DT_Destinazioni.Dispose()
                '    DT_Destinazioni = Nothing

                'End If


            Else
                Veg_Cod = HttpContext.Current.Session("cmb_specie")
                Grfi_cod = HttpContext.Current.Session("cmb_finalita")

                id_consociazione = 0

                'Salva_Distinta()

            End If 'RegImpianti


            jsCodici = DT_to_Json_Codici(Dt2)

            '--------------------------------------------------------------------------------------------------------------------------------------------
            'Controllo, se sono già state registrate Operazioni d'Agenda
            'che le date delle operazioni nn siano esterne alle date scelte x l'impianto 
            '--------------------------------------------------------------------------------------------------------------------------------------------

            Dim Validita_Inizio_Agenda As Date
            Dim Validita_Fine_Agenda As Date

            Dim Hash_IdAgenda_ToFlag As New Hashtable
            MovimentiPresenti = False
            TrattamentiPresenti = False

            If Operazione = enum_TipoOperazioneDB.Modifica Then

                Dim ObjAgenda As New AgronicaCoreContabDAL.Mov_Destinazioni_R  'New Agro_Contab_AD.Mov_Destinazioni_R
                'Dim DTAgenda As DataTable
                'Dim Id_Agenda As Integer

                ''ricavo il recordset dei movimenti di produzione associati all'impianto
                'DTAgenda = ObjAgenda.LeggiCronologiaMovimenti(xPiva,
                '                                            xSa_Cod,
                '                                            xAppezza,
                '                                            xId_Imp,
                '                                              AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                '                                            "",
                '                                            "",
                '                                            objParametri_Server)


                'ObjAgenda = Nothing

                'If DTAgenda.Rows.Count > 0 Then

                '    MovimentiPresenti = True

                '    ' x modificare la finalità se operazioni diverse da trattamenti e diserbi
                '    TrattamentiPresenti = ciSonoTrattamentiDiserbi(xPiva, xSa_Cod, xAppezza, xId_Imp)

                '    Validita_Inizio_Agenda = DTAgenda.Rows(0).Item("Data_Movimento")

                '    Validita_Fine_Agenda = DTAgenda.Rows(DTAgenda.Rows.Count - 1).Item("Data_Movimento")

                '    '(24/07/2015) eliminato perchè spostato nel core di scrittura
                '    ''se ho modificato la superficie, mi salvo gli id_agenda delle operazioni da flaggare
                '    'If viewstate("FlagSupModificata") = True Then
                '    '    Dim i As Integer
                '    '    For i = 0 To DTAgenda.Rows.Count - 1
                '    '        Id_Agenda = DTAgenda.Rows(i).Item("Id_agenda")
                '    '        If Not Hash_IdAgenda_ToFlag.Contains(Id_Agenda) Then
                '    '            Hash_IdAgenda_ToFlag.Add(Id_Agenda, Qs_Piva)
                '    '        End If Dim P_HA As Double
                '    '    Next
                '    'End If

                'End If


                Dim DT_Destinazioni As DataTable

                'ricavo i movimenti di produzione associati all'impianto
                DT_Destinazioni = ObjAgenda.NewCom_Mov_Destinazioni_Impianti_Leggi(objParametri_Server, xPiva, xSa_Cod, ,,, xAppezza, xId_Imp,,,,,)

                If Not IsNothing(DT_Destinazioni) Then

                    If DT_Destinazioni.Rows.Count <> 0 Then

                        MovimentiPresenti = True

                        TrattamentiPresenti = ciSonoTrattamentiDiserbi(xPiva, xSa_Cod, xAppezza, xId_Imp)

                        Validita_Inizio_Agenda = DT_Destinazioni.Rows(0).Item("Validita_Inizio")

                        Validita_Fine_Agenda = DT_Destinazioni.Rows(DT_Destinazioni.Rows.Count - 1).Item("Validita_Inizio")

                    End If

                    DT_Destinazioni.Dispose()
                    DT_Destinazioni = Nothing

                End If


                If MovimentiPresenti Then

                    'If Validita_Inizio_Agenda < Validita_Inizio Then
                    '    MessaggioErrore += "   - Non è possibile inserire l'impianto nell'intervallo scelto, " & vbCrLf & _
                    '                    "     poiché sono state registrate Operazioni d'Agenda dal " & CDate(Validita_Inizio_Agenda).ToShortDateString & " al " & CDate(Validita_Fine_Agenda).ToShortDateString & vbCrLf
                    'End If

                    'If Validita_Fine_Agenda > Validita_Fine Then
                    '    MessaggioErrore += "   - Non è possibile inserire l'impianto nell'intervallo scelto, " & vbCrLf & _
                    '                    "     poiché sono state registrate Operazioni d'Agenda dal " & CDate(Validita_Inizio_Agenda).ToShortDateString & " al " & CDate(Validita_Fine_Agenda).ToShortDateString & vbCrLf
                    'End If

                    'ChkTerrenoNudo.Enabled = False
                    'Cmb_Specie.Enabled = False
                    'Cmb_Finalita.Enabled = False

                End If


                Dim objPianoConcimazione_EntitaxTestata_R As New AgronicaCorePianoConcimazioneDAL.PianoConcimazione_EntitaxTestata_R

                Dim PianoConcimazione_EntitaxTestata = objPianoConcimazione_EntitaxTestata_R.Leggi_Default(0, 0, xPiva, xSa_Cod, 0, xAppezza,
                                                                                                           xId_Imp, 0, "", "", "", 0,
                                                                                                           0, "", 0, "",
                                                                                                           enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                                                           "", "",
                                                                                                           objParametri_Server)

                If PianoConcimazione_EntitaxTestata.Rows.Count > 0 Then
                    CollegamentoPianoConcimazione = True
                End If

            End If
        End If

        ' generazione automatica codice impianto
        If AlgoritmoCodifica <> "" AndAlso codice_impianto = "" Then
            codice_impianto = Replica_GIAS.LeggiCodiceProgressivo(xPiva, AlgoritmoCodifica, enum_SequenzaProgressiviTipi.CodiciProgettoAgricoli, impianto_data_inizio.Year, objParametri_Server)
        End If

        objImpianto.Add(New JProperty("codice_impianto", codice_impianto))

        objImpianto.Add(New JProperty("impianto_data_inizio", impianto_data_inizio))
        objImpianto.Add(New JProperty("impianto_data_fine", impianto_data_fine))

        objImpianto.Add(New JProperty("cul_cod", CulCod))
        objImpianto.Add(New JProperty("veg_cod", Veg_Cod))
        objImpianto.Add(New JProperty("gru_cod", Gru_Cod))

        objImpianto.Add(New JProperty("port_cod", port_cod))
        objImpianto.Add(New JProperty("foral_cod", foral_cod))
        objImpianto.Add(New JProperty("imp_cod2", Imp_Cod2))
        objImpianto.Add(New JProperty("provenienzaseme", ProvenienzaSeme))
        objImpianto.Add(New JProperty("cop_cod", cop_cod))
        objImpianto.Add(New JProperty("movimentipresenti", MovimentiPresenti))
        objImpianto.Add(New JProperty("trattamentipresenti", TrattamentiPresenti))

        objImpianto.Add(New JProperty("sup_appezza", LblSubAppezza))
        objImpianto.Add(New JProperty("sup_imp", sup_Imp))
        objImpianto.Add(New JProperty("grfi_cod", Grfi_cod))
        objImpianto.Add(New JProperty("grva_cod", grva_cod))
        objImpianto.Add(New JProperty("id_cod_terreno", id_cod_terreno))
        objImpianto.Add(New JProperty("setup_cod", setup_cod))

        objImpianto.Add(New JProperty("tra_fila_m", tra_fila_m))
        objImpianto.Add(New JProperty("su_fila_m", su_fila_m))
        objImpianto.Add(New JProperty("Chkinterbina", Chkinterbina))
        objImpianto.Add(New JProperty("interbina", interbina))
        objImpianto.Add(New JProperty("germinabilita", germinabilita))
        objImpianto.Add(New JProperty("codiceZona", codiceZona))
        objImpianto.Add(New JProperty("cop_data_inizio", cop_data_inizio))
        objImpianto.Add(New JProperty("cop_data_fine", cop_data_fine))
        objImpianto.Add(New JProperty("dettaglio_varieta_personalizzato", dettaglio_varieta_personalizzato))
        objImpianto.Add(New JProperty("chkConsociazione_enabled", chkConsociazione_enabled))
        objImpianto.Add(New JProperty("chkConsociazione_checked", chkConsociazione_checked))
        objImpianto.Add(New JProperty("id_consociazione", id_consociazione))

        objImpianto.Add(New JProperty("tagliato_tuberi", tagliato_tuberi))
        objImpianto.Add(New JProperty("partiTuberi", partiTuberi))

        objImpianto.Add(New JProperty("ChkCoverCrops", ChkCoverCrops))
        objImpianto.Add(New JProperty("ChkMonitorato", ChkMonitorato))

        objImpianto.Add(New JProperty("impianto_ibrido", impianto_ibrido))

        objImpianto.Add(New JProperty("Resa_Prevista", Resa_Prevista))
        objImpianto.Add(New JProperty("Resa_Effettiva", Resa_Corretta))
        objImpianto.Add(New JProperty("unita_vitata", Unita_Vitata))

        objImpianto.Add(New JProperty("CodBMBDBT_M", CodBMBDBT_M))
        objImpianto.Add(New JProperty("CodBMBDBT_F", CodBMBDBT_F))
        objImpianto.Add(New JProperty("Genetica_M", Genetica_M))
        objImpianto.Add(New JProperty("Genetica_F", Genetica_F))
        objImpianto.Add(New JProperty("OffType_M", OffType_M))
        objImpianto.Add(New JProperty("OffType_F", OffType_F))
        objImpianto.Add(New JProperty("DistanzaSuFila_F", DistanzaSuFila_F))
        objImpianto.Add(New JProperty("DistanzaTraFila_F", DistanzaTraFila_F))
        objImpianto.Add(New JProperty("tecn_cod", tecn_cod))
        objImpianto.Add(New JProperty("su_cod", su_cod))
        objImpianto.Add(New JProperty("dati_Distinte", dati_Distinte))

        objImpianto.Add(New JProperty("Data_Inizio_Portinnesto", Data_Inizio_Portinnesto))

        objImpianto.Add(New JProperty("Data_Inizio_Innesto", Data_Inizio_Innesto))
        objImpianto.Add(New JProperty("Data_Inizio_Produzione", Data_Inizio_Produzione))
        objImpianto.Add(New JProperty("ChkMaschiSesto", ChkMaschiSesto))

        objImpianto.Add(New JProperty("CollegamentoPianoConcimazione", CollegamentoPianoConcimazione))
    End Sub

    ' patch Grilli del 22/05/2019 per correggere vincolo su finalità
    Public Function ciSonoTrattamentiDiserbi(piva As String, sacod As Integer, appezza As Integer, idreg As Integer) As Boolean

        Dim o As New AgronicaCoreContabDAL.Mov_Destinazioni_R()
        Dim strFiltroLavCod As String = " Lav_Cod IN (74,13,158,155,103,18) "

        Dim dt As DataTable = o.Leggi(piva, sacod, 0, 0, 0, appezza, idreg, 0, enumSelezioneVariabile.Selezione_TabellaCompleta, strFiltroLavCod, "", objParametri_Server)
        Try
            If dt.Rows.Count > 0 Then
                Return True
            End If
            Return False
        Catch ex As Exception
            Return True
        End Try

        Return False
    End Function

    Protected Function carica_Kendo_Distinte(piva As String, sa_cod As Integer, appezza As Integer, id_reg As Integer, sup_imp As Double) As String
        Dim objProgetto As New AgronicaCoreAnagrafeDAL.Impresa_Progetti_R
        Dim Validita_Inizio As Date = Nothing
        Dim Validita_Fine As Date = Nothing
        Dim strFiltroCapitolato As String = ""
        Dim DT_Distinte As New DataTable
        DT_Distinte_Inizializza(DT_Distinte)
        Dim OrganismoReferente As String = ""
        Dim Riferimento_Trasferimento_Dati As String = ""
        'imposto la finestra
        objParametri_Server.ImpostaFinestre_con_SalvataggioTemporale(AGRODATAINIZIO, AGRODATAFINE)
        Dim Dt As DataTable
        Dt = objProgetto.Leggi(CStr(piva),
                                0,
                                CInt(9100),
                                0,
                                CInt(sa_cod),
                                CInt(appezza),
                                CInt(id_reg),
                                0, 0,
                                enumSelezioneVariabile.Selezione_TabellaCompleta,
                                "",
                                " Imprese_Progetti.Validita_Inizio DESC ",
                                objParametri_Server)

        objParametri_Server.ResettaFinestra()

        objProgetto = Nothing
        If id_reg <> 0 Then
            If Dt IsNot Nothing AndAlso Dt.Rows.Count > 0 Then

                'Dt.DefaultView.Sort = "Validita_inizio desc"

                For i = 0 To Dt.Rows.Count - 1

                    Dim obj_Codici_Distinta As New JArray
                    Dim obj_Catasto_Distinta As New JArray

                    Dim rowNew = DT_Distinte.NewRow()
                    Dim Progetto_Cod = Dt.Rows(i).Item("Progetto_Cod")
                    rowNew("Progetto_Cod") = Progetto_Cod

                    If CDate(Dt.Rows(i).Item("Validita_Inizio")) <> #1/1/1900# Then
                        Validita_Inizio = CDate(Dt.Rows(i).Item("Validita_Inizio"))
                        rowNew("Validita_Inizio") = Validita_Inizio.ToShortDateString()
                    Else
                        rowNew("Validita_Inizio") = AGRODATAINIZIO.ToShortDateString
                    End If

                    If CDate(Dt.Rows(i).Item("Validita_Fine")) <> #12/31/2100# Then
                        Validita_Fine = CDate(Dt.Rows(i).Item("Validita_Fine"))
                        rowNew("Validita_Fine") = Validita_Fine.ToShortDateString()
                    Else
                        rowNew("Validita_Fine") = AGRODATAFINE.ToShortDateString
                    End If

                    rowNew("Progetto_Nome") = Dt.Rows(i).Item("Progetto_Nome")
                    rowNew("Progetto_Des") = Dt.Rows(i).Item("Progetto_Des")
                    'rowNew("Sup_Prog") = If(Dt.Rows(i).Item("Sup_Prog") <> 0, Dt.Rows(i).Item("Sup_Prog"), sup_imp)
                    rowNew("Regolamento_Cod") = Dt.Rows(i).Item("Regolamento_Cod")
                    rowNew("disciplinare_pubblicoprivato") = Dt.Rows(i).Item("disciplinare_pubblicoprivato")
                    rowNew("Disciplinare_cod") = Dt.Rows(i).Item("Disciplinare_cod")
                    If CDate(Dt.Rows(i).Item("Validita_Fine")) <> AGRODATAFINE Then
                        strFiltroCapitolato = " validita_fine >=" & Agro_SQL_SaveDate(Validita_Fine)
                    End If

                    rowNew("Regolamento_Concimazione_Cod") = Dt.Rows(i).Item("Regolamento_Concimazioni_Cod")
                    rowNew("stato_impianto") = Dt.Rows(i).Item("stato_impianto")
                    rowNew("p_ha") = Dt.Rows(i).Item("p_ha")
                    'rowNew("PianteHa") = Dt.Rows(i).Item("PianteHa")
                    rowNew("data_fioritura_prevista") = CDate(Dt.Rows(i).Item("data_fioritura_prevista")).ToShortDateString
                    rowNew("data_raccolta_prevista") = CDate(Dt.Rows(i).Item("data_fine_prevista")).ToShortDateString
                    rowNew("data_semina_prevista") = CDate(Dt.Rows(i).Item("data_inizio_prevista")).ToShortDateString
                    rowNew("id_tr") = 3
                    rowNew("produzione_prevista") = Dt.Rows(i).Item("produzione_prevista")


                    '- - - - - - - - - - - - - - - - A N N A      I N I Z I O - - - - - - - - - - - - - - - -
                    rowNew("P_HA_Femmine") = Dt.Rows(i).Item("P_HA_Femmine")

                    rowNew("P_HA_Maschi") = Dt.Rows(i).Item("P_HA_Maschi")
                    '- - - - - - - - - - - - - - - - A N N A      F I N E - - - - - - - - - - - - - - - - - -

                    rowNew("FlagSecondoRaccolto") = Dt.Rows(i).Item("FlagSecondoRaccolto")

                    ''
                    ''  CODICI PROGETTO
                    ''
                    Dim objCodici = New AgronicaCoreAnagrafeDAL.Reg_Impianti_Codici_R
                    Dim DtCodici = objCodici.LeggixProgetto(
                        piva,
                        sa_cod,
                        appezza,
                        id_reg,
                        "",
                        Progetto_Cod,
                        0,
                        "",
                        AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                        "",
                        "",
                        objParametri_Server
                        )

                    If DtCodici IsNot Nothing AndAlso DtCodici.Rows.Count > 0 Then
                        For Each rowCodice In DtCodici.Rows
                            Dim id_cod As Integer = rowCodice("ID_Cod")
                            Dim val_cod As String = ""

                            If Not IsDBNull(rowCodice("Val_Cod")) Then
                                val_cod = rowCodice("Val_Cod")
                            End If

                            Select Case id_cod

                                Case enum_CodiciAnagrafe.Finalita_Concimazione_Impianto

                                    rowNew("Finalita_Concimazione_Cod") = val_cod

                                Case enum_CodiciAnagrafe.Impianto_LimiteN
                                    If IsNumeric(val_cod) Then
                                        rowNew("TxtN") = val_cod
                                    End If

                                Case enum_CodiciAnagrafe.Impianto_LimiteP
                                    If IsNumeric(val_cod) Then
                                        rowNew("TxtP2O5") = val_cod
                                    End If


                                Case enum_CodiciAnagrafe.Impianto_LimiteK
                                    If IsNumeric(val_cod) Then
                                        rowNew("TxtK2O") = val_cod
                                    End If

                                Case enum_CodiciAnagrafe.Impianto_LimiteMg
                                    If IsNumeric(val_cod) Then
                                        rowNew("TxtMgO") = val_cod
                                    End If

                                Case enum_CodiciAnagrafe.Riferimento_Trasferimento_Dati
                                    rowNew("Riferimento_Trasferimento_Dati") = val_cod
                                    Riferimento_Trasferimento_Dati = val_cod

                                Case enum_CodiciAnagrafe.Organismo_Referente
                                    'il caricamento del magazzino conferimento non dipende più dal menù a tendina dell'organismo
                                    'Carica_MagazzinoConferimento(Val_Cod)
                                    rowNew("Organismo_Referente") = val_cod
                                    OrganismoReferente = val_cod
                                'If val_cod <> "" And Me.Cmb_OrganismoReferente.SelectedItem.Text = "" Then
                                '    AgroMsgBox("ATTENZIONE! Su Organismo Referente è stato trovato valore= " & val_cod & " ma non è stato possibile selezionarlo --> Prima di apportare modifiche all'impianto colturale, occorre modificare il contatto con tale p.iva, impostandolo pubblico e con rapporto contabile 'organismo referente', altrimenti se si procede con il salvataggio dell'impianto l'informazione sull'organismo referente verrà persa.", Page)
                                'End If

                                Case enum_CodiciAnagrafe.Magazzino_Conferimento
                                    ''la combo dei magazzini è già caricata, perchè se è salvato il magazzino di conferimento
                                    ''è salvato anche l'organismo referente
                                    '03/11/2017: non più vero, ora i due menù a tendina sono svincolati
                                    Dim ValCodModificato As Boolean
                                    val_cod = Controlla_ValCod_MagazzinoConferimento(val_cod, OrganismoReferente, ValCodModificato)
                                    rowNew("Magazzino_Conferimento") = val_cod

                                'If val_cod <> "" And Me.Cmb_MagazzinoConferimento.SelectedItem.Text = "" Then
                                '    AgroMsgBox("ATTENZIONE! Su Magazzino conferimento è stato trovato valore= " & val_cod & " ma non è stato possibile selezionarlo --> Selezionare il magazzino dal menù a tendina prima di procedere con il salvataggio.", Page)
                                'Else
                                '    If ValCodModificato = True Then
                                '        AgroMsgBox("ATTENZIONE! E' necessario aprire in modifica e risalvare la distinta al fine di non perdere l'informazione sul magazzino di conferimento.", Page)
                                '    End If
                                'End If


                                Case enum_CodiciAnagrafe.Capitolato_Privato

                                    rowNew("Capitolato_Privato") = val_cod


                                '  - - - - - - - - - - - - - - - - A N N A      I N I Z I O - - - - - - - - - - - - - - - -
                                Case enum_CodiciAnagrafe.Zespri_Fasi_Fase

                                    rowNew("Licenza_Coltivazione") = val_cod
                                ' - - - - - - - - - - - - - - - - A N N A      F I N E - - - - - - - - - - - - - - - - - -

                                Case enum_CodiciAnagrafe.Impianto_IAF_ImpegniAggiuntiviFacoltativi
                                    Dim arrIAF As New JArray

                                    Dim valoriDaSelez As String() = val_cod.Split("|")

                                    For Each value In valoriDaSelez
                                        arrIAF.Add(value)
                                    Next

                                    rowNew("Impianto_IAF_ImpegniAggiuntiviFacoltativi") = arrIAF.ToString

                                Case enum_CodiciAnagrafe.Impianto_PianoSemina

                                    '28/03/2018: correzione per conserve italia:
                                    'non veniva caricato il valore nel menù a tendina, ma gestita questa lista (probabilmente una cosa vecchia)
                                    rowNew("Impianto_PianoSemina") = val_cod

                                Case enum_CodiciAnagrafe.Distinta_Chiusa

                                    rowNew("distinta_chiusa") = val_cod

                                Case enum_CodiciAnagrafe.Codice_Impianto_Ribaltato

                                    rowNew("distinta_replica_codice") = val_cod
                                    rowNew("distinta_replica") = If(String.IsNullOrEmpty(val_cod), "0", "1")

                                Case Else
                                    'se nella stringa contenente i codici restituita dal componente è presente..
                                    If InStr(StrCodiciImpianto, id_cod) <> 0 Then

                                        Dim objCodiceAnagrafe As New AgronicaCoreAnagrafeDAL.Codice_Anagrafe_R
                                        Dim CodiceAnagrafeDes = objCodiceAnagrafe.CodiceAnagrafeDes_from_CodiceAnagrafeCod(
                                                                    CInt(id_cod), objParametri_Server)
                                        'ListCodici.Items.Add(New ListItem(CodiceAnagrafeDes & " = " & val_cod,
                                        '                                  id_cod))
                                        Dim obj_Codice_Distinta As New JObject
                                        obj_Codice_Distinta.Add(New JProperty("id_cod", id_cod))
                                        obj_Codice_Distinta.Add(New JProperty("descrizione", CodiceAnagrafeDes))
                                        obj_Codice_Distinta.Add(New JProperty("val_cod", val_cod))
                                        obj_Codici_Distinta.Add(obj_Codice_Distinta)

                                    End If

                            End Select


                        Next


                    End If

                    rowNew("obj_Codici_Distinta") = obj_Codici_Distinta.ToString


                    ''
                    ''  PARTICELLE PROGETTO
                    ''
                    Dim objParticelle As New AgronicaCoreAnagrafeDAL.ProgettixParticelle_R

                    Dim DtParticelle = objParticelle.LeggixProgetto(
                                                piva,
                                                sa_cod,
                                                appezza,
                                                id_reg,
                                                Progetto_Cod,
                                                0,
                                                "",
                                                AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                "",
                                                "",
                                                objParametri_Server)

                    Dim obj_Particelle_Progetto As New JArray()

                    For Each partRow In DtParticelle.Rows
                        Dim id_Cod = partRow("id_Cod")
                        Dim val_Cod = partRow("val_cod")

                        Dim prov = partRow("Prov")
                        Dim provincia = partRow("Provincia")
                        Dim com = partRow("Com")
                        Dim comune = partRow("comune")
                        Dim sezione = ""
                        If partRow("sezione") <> "0" Then
                            sezione = partRow("sezione")
                        End If
                        Dim foglio = partRow("Foglio")
                        Dim numero = partRow("Numero")
                        Dim subalterno = ""
                        If partRow("subalterno") <> "0" Then
                            subalterno = partRow("subalterno")
                        End If

                        Dim testo = "(" & prov & ") " & provincia & " - (" & com & ") " & comune & ":" & sezione & ":" & foglio & ":" & numero & ":" & subalterno
                        Dim valore = prov & "_" & com & "_" & sezione & "_" & foglio & "_" & numero & "_" & subalterno

                        Dim objCodiceAnagrafe As New AgronicaCoreAnagrafeDAL.Codice_Anagrafe_R
                        Dim CodiceAnagrafeDes = objCodiceAnagrafe.CodiceAnagrafeDes_from_CodiceAnagrafeCod(
                                                                    CInt(id_Cod), objParametri_Server)

                        Dim obj_Particella As New JObject
                        obj_Particella.Add(New JProperty("prov", prov))
                        obj_Particella.Add(New JProperty("com", com))
                        obj_Particella.Add(New JProperty("provincia", provincia))
                        obj_Particella.Add(New JProperty("comune", comune))
                        obj_Particella.Add(New JProperty("sezione", sezione))
                        obj_Particella.Add(New JProperty("foglio", foglio))
                        obj_Particella.Add(New JProperty("numero", numero))
                        obj_Particella.Add(New JProperty("subalterno", subalterno))
                        obj_Particella.Add(New JProperty("testoProv", testo))
                        obj_Particella.Add(New JProperty("valoreProv", valore))
                        obj_Particella.Add(New JProperty("id_cod", id_Cod))
                        obj_Particella.Add(New JProperty("id_des", CodiceAnagrafeDes))
                        obj_Particella.Add(New JProperty("val_cod", val_Cod))
                        obj_Particelle_Progetto.Add(obj_Particella)

                    Next

                    rowNew("obj_Particelle_Distinta") = obj_Particelle_Progetto.ToString

                    DT_Distinte.Rows.Add(rowNew)

                Next


            End If
        End If

        Dt = Nothing
        Return JSON_DataTableDistinte_Tabella(DT_Distinte)

    End Function

    Private Shared Function JSON_DataTableDistinte_Tabella(ByRef DT_Distinte As DataTable, Optional ByVal stringaKendoRow As String = "") As String

        'creo la lista delle colonne da visualizzare
        Dim l As New List(Of ColonneNome)
        Dim c As ColonneNome

        c = New ColonneNome("Progetto_Cod", "Progetto_Cod", "number")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Progetto_Nome", AgronicaAgenda_2010.LottoEsercizio, "string")
        'c._hidden = True
        l.Add(c)

        c = New ColonneNome("Progetto_Des", AgronicaAgenda_2010.Descrizione, "string")
        'c._hidden = True
        l.Add(c)

        'c = New ColonneNome("Sup_Prog", "Sup_Prog", "number")
        'c._hidden = True
        'l.Add(c)

        c = New ColonneNome("Validita_Inizio", AgronicaAgenda_2010.DataInizioEsercizio, "string")
        c._FormatoParticolare = "#= Validita_Inizio == '01/01/1900' ? '' : Validita_Inizio #"
        'c._hidden = True
        l.Add(c)

        c = New ColonneNome("Validita_Fine", AgronicaAgenda_2010.DataFineEsercizio, "string")
        c._FormatoParticolare = "#= Validita_Fine == '31/12/2100' ? '' : Validita_Fine #"
        'c._hidden = True
        l.Add(c)

        c = New ColonneNome("Regolamento_Cod", "Regolamento_Cod", "number")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Disciplinare_PubblicoPrivato", "Disciplinare_PubblicoPrivato", "number")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Regolamento_Concimazione_Cod", "Regolamento_Concimazione_Cod", "number")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Finalita_Concimazione_Cod", "Finalita_Concimazione_Cod", "number")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Disciplinare_Cod", "Disciplinare_Cod", "number")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("stato_impianto", "stato_impianto", "number")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("p_ha", "p_ha", "number")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("PianteHa", "PianteHa", "number")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("TxtN", "TxtN", "number")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("TxtP2O5", "TxtP2O5", "number")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("TxtK2O", "TxtK2O", "number")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("TxtMgO", "TxtMgO", "number")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Organismo_Referente", "Organismo_Referente", "string")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Riferimento_Trasferimento_Dati", "Riferimento_Trasferimento_Dati", "string")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Magazzino_Conferimento", "Magazzino_Conferimento", "string")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Capitolato_Privato", "Capitolato_Privato", "string")
        c._hidden = True
        l.Add(c)

        ' - - - - - - - - - - - - - - - - A N N A      I N I Z I O - - - - - - - - - - - - - - - -
        c = New ColonneNome("Licenza_Coltivazione", "Licenza_Coltivazione", "string")
        c._hidden = True
        l.Add(c)
        ' - - - - - - - - - - - - - - - - A N N A      F I N E - - - - - - - - - - - - - - - - - -

        c = New ColonneNome("Impianto_IAF_ImpegniAggiuntiviFacoltativi", "Impianto_IAF_ImpegniAggiuntiviFacoltativi", "string")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Impianto_PianoSemina", "Impianto_PianoSemina", "string")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("data_semina_prevista", "data_semina_prevista", "string")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("data_raccolta_prevista", "data_raccolta_prevista", "string")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("data_fioritura_prevista", "data_fioritura_prevista", "string")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("id_tr", "id_tr", "number")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("produzione_prevista", "produzione_prevista", "number")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("distinta_chiusa", "distinta_chiusa", "string")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("distinta_replica", "distinta_replica", "string")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("distinta_replica_codice", "distinta_replica_codice", "string")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("obj_Codici_Distinta", "obj_Codici_Distinta", "string")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("obj_Particelle_Distinta", "obj_Particelle_Distinta", "string")
        c._hidden = True
        l.Add(c)

        ' - - - - - - - - - - - - - - - - A N N A      I N I Z I O - - - - - - - - - - - - - - - -
        c = New ColonneNome("P_HA_Femmine", "P_HA_Femmine", "double")
        c._hidden = True
        l.Add(c)


        c = New ColonneNome("P_HA_Maschi", "P_HA_Maschi", "double")
        c._hidden = True
        l.Add(c)
        ' - - - - - - - - - - - - - - - - A N N A      F I N E - - - - - - - - - - - - - - - - - -

        c = New ColonneNome("FlagSecondoRaccolto", "FlagSecondoRaccolto", "string")
        c._hidden = True
        l.Add(c)


        'ritorno la tabella trasformata in json (aggiustando anche le colonne)
        Dim js As New AgronicaCoreDataProvider.JSON_DataTable
        js.Editabile_Deafault = False

        Dim risp As String

        If stringaKendoRow = "" Then

            risp = js.JSON_DataTable_Kendo(DT_Distinte, l, AssegnaAutomaticamenteTipoFiltro_daTipoDato:=True, tipoFiltroKendo:=TipoFiltroKendo_colonne.CasellaTesto_e_CasellaDiscesa) 'True, True, TipoFiltroKendo_colonne.CasellaTesto) '
        End If

        Return risp

    End Function

    Protected Sub DT_Distinte_Inizializza(ByRef dt As DataTable)

        dt.Columns.Add(New DataColumn("Progetto_Cod"))
        dt.Columns.Add(New DataColumn("Progetto_Nome"))
        dt.Columns.Add(New DataColumn("Progetto_Des"))
        'dt.Columns.Add(New DataColumn("Sup_Prog"))
        dt.Columns.Add(New DataColumn("Validita_Inizio"))
        dt.Columns.Add(New DataColumn("Validita_Fine"))
        dt.Columns.Add(New DataColumn("Regolamento_Cod"))
        dt.Columns.Add(New DataColumn("Disciplinare_PubblicoPrivato"))
        dt.Columns.Add(New DataColumn("Regolamento_Concimazione_Cod"))
        dt.Columns.Add(New DataColumn("Finalita_Concimazione_Cod"))
        dt.Columns.Add(New DataColumn("Disciplinare_Cod"))
        dt.Columns.Add(New DataColumn("stato_impianto"))
        dt.Columns.Add(New DataColumn("p_ha"))
        dt.Columns.Add(New DataColumn("PianteHa"))
        dt.Columns.Add(New DataColumn("TxtN"))
        dt.Columns.Add(New DataColumn("TxtP2O5"))
        dt.Columns.Add(New DataColumn("TxtK2O"))
        dt.Columns.Add(New DataColumn("TxtMgO"))
        dt.Columns.Add(New DataColumn("Organismo_Referente"))
        dt.Columns.Add(New DataColumn("Riferimento_Trasferimento_Dati"))
        dt.Columns.Add(New DataColumn("Magazzino_Conferimento"))
        dt.Columns.Add(New DataColumn("Capitolato_Privato"))
        ' - - - - - - - - - - - - - - - - A N N A      I N I Z I O - - - - - - - - - - - - - - - -
        dt.Columns.Add(New DataColumn("Licenza_Coltivazione"))
        ' - - - - - - - - - - - - - - - - A N N A      F I N E - - - - - - - - - - - - - - - - - -
        dt.Columns.Add(New DataColumn("Impianto_IAF_ImpegniAggiuntiviFacoltativi"))
        dt.Columns.Add(New DataColumn("Impianto_PianoSemina"))
        dt.Columns.Add(New DataColumn("data_semina_prevista"))
        dt.Columns.Add(New DataColumn("data_raccolta_prevista"))
        dt.Columns.Add(New DataColumn("data_fioritura_prevista"))
        dt.Columns.Add(New DataColumn("Altri_Codici"))
        dt.Columns.Add(New DataColumn("id_tr"))
        dt.Columns.Add(New DataColumn("produzione_prevista"))
        dt.Columns.Add(New DataColumn("distinta_chiusa"))
        dt.Columns.Add(New DataColumn("distinta_replica"))
        dt.Columns.Add(New DataColumn("distinta_replica_codice"))
        dt.Columns.Add(New DataColumn("obj_Codici_Distinta"))
        dt.Columns.Add(New DataColumn("obj_Particelle_Distinta"))

        '- - - - - - - - - - - - - - - - A N N A      I N I Z I O - - - - - - - - - - - - - - - -
        dt.Columns.Add(New DataColumn("P_HA_Femmine"))

        dt.Columns.Add(New DataColumn("P_HA_Maschi"))
        '- - - - - - - - - - - - - - - - A N N A      F I N E - - - - - - - - - - - - - - - - - -

        dt.Columns.Add(New DataColumn("FlagSecondoRaccolto"))

    End Sub

    Public Shared Function Verifica_Esistenza_Impianti_Consociati(piva As String, sa_cod As Integer, appezza As Integer, ByRef pId_Consociazione As Integer, ByRef inizioImpianto As Date, objParametri_Server As AgronicaCoreParametri) As Boolean

        Dim objRegImp As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
        Dim DtRegImp As DataTable

        'Verifico se ci sono già altri impianti attivi e consociati nell'appezzamento
        DtRegImp = objRegImp.Leggi(piva,
                                   sa_cod,
                                   appezza,
                                   0,
                                   enumSelezioneVariabile.Selezione_TabellaCompleta,
                                   " Validita_Fine >= " & UtilityProvider.Agro_SQL_SaveDate(inizioImpianto) &
                                   " AND Id_Consociazione <> 0",
                                   "", objParametri_Server)

        If DtRegImp IsNot Nothing AndAlso DtRegImp.Rows.Count > 0 Then

            pId_Consociazione = CInt(DtRegImp.Rows(0).Item("Id_Consociazione"))
            Return True

        Else

            pId_Consociazione = 0
            Return False

        End If

    End Function

    Public Shared Function Controlla_ValCod_MagazzinoConferimento(ByVal Val_Cod As String,
                                                                    ByVal PivaOrgReferente As String,
                                                                    ByRef ValCodModificato As Boolean) As String

        Dim ValCodReturn As String = Val_Cod

        If Val_Cod <> "" Then

            ValCodModificato = False

            Dim vet() As String = Val_Cod.Split("|")

            If vet.Length < 3 Then
                If PivaOrgReferente <> "" Then
                    'vecchia gestione, il val_cod era solo fabbricato_cod|sa_cod
                    'ora serve anche la piva
                    ValCodReturn = Val_Cod & "|" & PivaOrgReferente
                    ValCodModificato = True
                End If
            End If

        End If

        Return ValCodReturn

    End Function

    '#########################################################################################
    Public Sub CaricaCombo_ParticelleCatastali_xAppezzamento(ByVal Piva As String,
                                    ByVal Sa_Cod As Integer,
                                    ByVal Appezza As Integer,
                                    ByVal objParametri_Server As AgronicaCoreParametri)

        Dim r As New RispostaStandard

        Try

            Dim objCOM As New AgronicaCoreAnagrafeDAL.AppezzaxParticelle_R  'Agro_Anagrafe_AD.AppezzaxParticelle_R
            Dim DT As DataTable

            DT = objCOM.LeggiParticelle_Da_Appezzamento(Piva, Sa_Cod, Appezza,
                                                     AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                                     "", "", objParametri_Server)

            Dim JArrayLista As New JArray()
            For Each partRow In DT.Rows

                Dim prov = partRow("Prov")
                Dim provincia = partRow("Provincia")
                Dim com = partRow("Com")
                Dim comune = partRow("comune")
                Dim sezione = ""
                If partRow("sezione") <> "0" Then
                    sezione = partRow("sezione")
                End If
                Dim foglio = partRow("Foglio")
                Dim numero = partRow("Numero")
                Dim subalterno = ""
                If partRow("subalterno") <> "0" Then
                    subalterno = partRow("subalterno")
                End If

                Dim testo = "(" & prov & ") " & provincia & " - (" & com & ") " & comune & ":" & sezione & ":" & foglio & ":" & numero & ":" & subalterno
                Dim valore = prov & "_" & com & "_" & sezione & "_" & foglio & "_" & numero & "_" & subalterno

                JArrayLista.Add(New JObject(New JProperty("value", valore),
                                            New JProperty("text", testo)))
            Next

            cmb_Particelle = JsonConvert.SerializeObject(JArrayLista, Formatting.None)

        Catch ex As Exception
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try
    End Sub

#End Region

    <WebMethod(EnableSession:=True)>
    Public Shared Function CaricaComboCmb_OrganismoReferente(ByVal obj_Impianto_str As String, Tipo_Salva As String, Lav_Cod As Integer) As RispostaStandard

        Dim r As New RispostaStandard

        Try






            'r.RispostaStringa = "Impianto salvato con successo."
            'r.RispostaOK = True

        Catch ex As Exception

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.RispostaOK = False
        End Try

        Return r

    End Function

#Region "NewVersion"

    '#########################################################################################
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Salva_tutto(obj_Impianto_str As String,
                                       Tipo_Salva As String,
                                       Lav_Cod As Integer,
                                       visibilita As Integer) As RispostaStandard

        Dim r As New RispostaStandard

        Try
            Dim Qs_Visibilita = visibilita
            Dim objParametri_Server As AgronicaCoreParametri = New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))
            Dim objParametri_Utenti As AgronicaCoreParametri = New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Utenti"))

            Lingua.Gias_InizializzaCultura_DaSession()

            Dim obj_Impianto = JObject.Parse(obj_Impianto_str) 'Newtonsoft.Json.JsonConvert.DeserializeObject(obj_Impianto_str)
            Dim piva As String = obj_Impianto.GetValue("piva").ToString()
            Dim sa_cod As Integer = CInt(obj_Impianto.GetValue("sa_cod"))
            Dim appezza As Integer = CInt(obj_Impianto.GetValue("appezza"))
            Dim id_reg As Integer = CInt(obj_Impianto.GetValue("id_reg"))
            Dim campo_cod As Integer = CInt(obj_Impianto.GetValue("campo_cod"))

            Dim objImpostazioni_Utenti As New Utenti_Impostazioni_Read
            Dim DefaultRegolamento = objImpostazioni_Utenti.Impostazione_Valore_From_Impostazione_Cod_Utente_Poi_SuperUser(
                enum_Impostazioni_Utenti.UTENTE_COD_REGOLAMENTO,
                objParametri_Utenti
                )

            If IsNumeric(DefaultRegolamento) AndAlso CInt(DefaultRegolamento) < 1 Then
                DefaultRegolamento = 1
            End If

            Dim NoteLog As String = NOTELOG_ANAGRAFE_BOOTSTRAP
            Dim objImpianto As New AgronicaCoreAnagrafeBIZ.Reg_Impianto_W

            r = objImpianto.Impianto_ScriviModifica(
                obj_Impianto_str,
                piva,
                sa_cod,
                appezza,
                id_reg,
                objParametri_Server,
                If(DefaultRegolamento = "", 1, CInt(DefaultRegolamento)),
                NoteLog:=NoteLog
                )

            If Not IsNothing(obj_Impianto.GetValue("veg_cod")) AndAlso Not IsNothing(obj_Impianto.GetValue("cul_cod")) Then
                Dim objGias As New AgronicaCoreAnagrafeBIZ.Importa_GIAS
                Dim objCultivar As New AgronicaCoreMetaSchemaDAL.Cultivar_R
                Dim cultivar As New AgronicaCoreModelsSTD.metaschema.utilizzi.Varieta
                Dim flag_0NoBio_1SoloBio_2Entrambi As Integer = -1

                cultivar.specie = New AgronicaCoreModelsSTD.metaschema.utilizzi.Specie
                cultivar.codice = CInt(obj_Impianto.GetValue("cul_cod"))
                cultivar.specie.codice = CInt(obj_Impianto.GetValue("veg_cod"))

                objCultivar.VegDes_CulDes_from_Vegcod_CulCod(
                    cultivar.specie.codice,
                    cultivar.codice,
                    cultivar.specie.descrizione,
                    cultivar.descrizione,
                    objParametri_Server
                    )

                Dim rows = JArray.Parse(JObject.Parse(obj_Impianto.GetValue("dati_Distinte").ToString()).GetValue("kendo_rows").ToString)

                For Each row As JObject In rows
                    Select Case (CInt(row.GetValue("Regolamento_Cod")))
                        Case enum_Cod_Regolamento.Regolamento_Nessuno
                            If flag_0NoBio_1SoloBio_2Entrambi = 1 OrElse flag_0NoBio_1SoloBio_2Entrambi = 2 Then
                                flag_0NoBio_1SoloBio_2Entrambi = 2
                            Else
                                flag_0NoBio_1SoloBio_2Entrambi = 0
                            End If
                        Case enum_Cod_Regolamento.Regolamento_bio
                            If flag_0NoBio_1SoloBio_2Entrambi = 0 OrElse flag_0NoBio_1SoloBio_2Entrambi = 2 Then
                                flag_0NoBio_1SoloBio_2Entrambi = 2
                            Else
                                flag_0NoBio_1SoloBio_2Entrambi = 1
                            End If
                        Case Else
                            flag_0NoBio_1SoloBio_2Entrambi = 2
                    End Select
                Next

                objGias.Crea_MateriaPrima_Specie_Varieta_Regolamento(
                    cultivar,
                    flag_0NoBio_1SoloBio_2Entrambi,
                    objParametri_Server,
                    objParametri_Utenti,
                    True,
                    True
                    )

            End If

            r.ParametroDue = True
            r.ParametroDue_stringa = ""

            Select Case Tipo_Salva
                Case ""
                    Dim TargetUrl = "../MenuAnagrafica/MenuBS_Anagrafica.aspx"

                    If Qs_Visibilita <> 0 Then
                        TargetUrl &= "?visibilita=" & CStr(Qs_Visibilita)
                    End If

                    r.ParametroDue_stringa = TargetUrl

                Case "1"
                    Dim TargetUrl = "../MenuAnagrafica/MenuBS_Anagrafica.aspx"

                    If Qs_Visibilita <> 0 Then
                        TargetUrl &= "?visibilita=" & CStr(Qs_Visibilita)
                    End If

                    r.ParametroDue_stringa = TargetUrl

                Case "2"
                    Dim veg_cod = 0
                    Dim id_cod As Integer = 0
                    Dim PaginaLink As String = MenuBS_Agenda_Nuovo.NuovaOperazioneAgenda(Lav_Cod, veg_cod)
                    Dim objParametriAgenda = New ParametriAgenda
                    Dim impiantiList = New List(Of AgronicaCoreModello.ParametriAgenda_Temp.Impianto)

                    PaginaLink &= "?PaginaOrigine=" & CStr(enum_PagineGiasOnline.MenuAnagrafica)
                    objParametriAgenda.Piva = obj_Impianto.GetValue("piva").ToString()
                    objParametriAgenda.Sa_Cod = obj_Impianto.GetValue("sa_cod").ToString()
                    objParametriAgenda.Appezza = obj_Impianto.GetValue("appezza").ToString()
                    objParametriAgenda.Id_Imp = obj_Impianto.GetValue("id_reg").ToString()

                    Dim obj_Impianti As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read

                    Dim dt_impianto = obj_Impianti.Leggi(
                        objParametriAgenda.Piva,
                        objParametriAgenda.Sa_Cod,
                        objParametriAgenda.Appezza,
                        objParametriAgenda.Id_Imp,
                        enumSelezioneVariabile.Selezione_TabellaCompleta,
                        "",
                        "",
                        objParametri_Server
                        )

                    Dim data_Operazione = Date.Now

                    If dt_impianto.Rows.Count > 0 Then
                        Dim validita_inizio = CDate(dt_impianto.Rows(0)("Validita_Inizio"))
                        Dim validita_fine = CDate(dt_impianto.Rows(0)("Validita_Fine"))
                        If validita_inizio > Date.Now Then
                            data_Operazione = validita_inizio
                        ElseIf validita_fine < Date.Now Then
                            data_Operazione = validita_fine
                        End If
                    End If

                    objParametriAgenda.Data = data_Operazione

                    Dim cul_cod = obj_Impianti.CulCod_from_PivaSaCodAppezzaIdimp(
                        objParametriAgenda.Piva,
                        objParametriAgenda.Sa_Cod,
                        objParametriAgenda.Appezza,
                        objParametriAgenda.Id_Imp,
                        objParametri_Server
                        )

                    If cul_cod = 0 Then
                        id_cod = obj_Impianti.Leggi_DestinazioneUso_Impianto(
                            objParametriAgenda.Piva,
                            objParametriAgenda.Sa_Cod,
                            objParametriAgenda.Appezza,
                            objParametriAgenda.Id_Imp,
                            "",
                            "",
                            objParametri_Server
                            )
                    End If

                    Dim objCultivar As New AgronicaCoreMetaSchemaDAL.Cultivar_R
                    Dim veg_cod_s As String = ""

                    If cul_cod <> 0 Then
                        veg_cod_s = objCultivar.VegCod_from_CulCod(cul_cod, objParametri_Server)
                        veg_cod = veg_cod_s
                    Else
                        veg_cod_s = "0/" & id_cod
                    End If

                    objParametriAgenda.Veg_Cod = veg_cod_s

                    Dim imp = New AgronicaCoreModello.ParametriAgenda_Temp.Impianto
                    imp.Piva = objParametriAgenda.Piva
                    imp.Sa_Cod = objParametriAgenda.Sa_Cod
                    imp.Appezza = objParametriAgenda.Appezza
                    imp.ID_Reg = objParametriAgenda.Id_Imp
                    impiantiList.Add(imp)

                    objParametriAgenda.Impianti = impiantiList
                    objParametriAgenda.TipoOperazioneAgenda = "1"
                    objParametriAgenda.Lav_Cod = Lav_Cod
                    objParametriAgenda.salva()

                    r.RispostaOK = True
                    r.ParametroDue_stringa = PaginaLink

                Case Else
                    Dim TargetUrl = "../MenuAnagrafica/MenuBS_Anagrafica.aspx"

                    If Qs_Visibilita <> 0 Then
                        TargetUrl &= "?visibilita=" & CStr(Qs_Visibilita)
                    End If

                    r.ParametroDue_stringa = TargetUrl
            End Select

        Catch ex As Exception

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.RispostaOK = False
        End Try

        Return r

    End Function

    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Controlla_Dati(ByVal obj_Impianto_str As String, saltaPrimoControllo As Boolean, saltaSecondoControllo As Boolean) As RispostaStandard

        Dim r As New RispostaStandard

        Try

            Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri = New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))
            Dim objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri = New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Utenti"))
            Dim objParametri_Agenda = New ParametriAgenda

            Lingua.Gias_InizializzaCultura_DaSession()

            Dim objImpW As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Write
            Dim objImpR As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
            Dim objRisposta = New JObject()

            Dim obj_Impianto = Newtonsoft.Json.JsonConvert.DeserializeObject(obj_Impianto_str)
            Dim piva As String = obj_Impianto.GetValue("piva").ToString()
            Dim sa_cod As Integer = CInt(obj_Impianto.GetValue("sa_cod"))
            Dim appezza As Integer = CInt(obj_Impianto.GetValue("appezza"))
            Dim id_reg As Integer = CInt(obj_Impianto.GetValue("id_reg"))
            Dim campo_cod As Integer = CInt(obj_Impianto.GetValue("campo_cod"))

            Dim validita_inizio = AGRODATAINIZIO
            If IsDate(obj_Impianto.GetValue("impianto_data_inizio").ToString) Then
                validita_inizio = CDate(obj_Impianto.GetValue("impianto_data_inizio"))
            End If

            Dim validita_fine = AGRODATAFINE
            If IsDate(obj_Impianto.GetValue("impianto_data_fine").ToString) Then
                validita_fine = CDate(obj_Impianto.GetValue("impianto_data_fine"))
            End If

            ' - - - - - - - - - - - - - - - - A N N A      I N I Z I O - - - - - - - - - - - - - - - -
            Dim Data_Inizio_Portinnesto = AGRODATAINIZIO
            If IsDate(obj_Impianto.GetValue("Data_Inizio_Portinnesto").ToString) Then
                Data_Inizio_Portinnesto = CDate(obj_Impianto.GetValue("Data_Inizio_Portinnesto"))
            End If

            Dim Data_Inizio_Innesto = AGRODATAINIZIO
            If IsDate(obj_Impianto.GetValue("Data_Inizio_Innesto").ToString) Then
                Data_Inizio_Innesto = CDate(obj_Impianto.GetValue("Data_Inizio_Innesto"))
            End If

            Dim Data_Inizio_Produzione = AGRODATAINIZIO
            If IsDate(obj_Impianto.GetValue("Data_Inizio_Produzione").ToString) Then
                Data_Inizio_Produzione = CDate(obj_Impianto.GetValue("Data_Inizio_Produzione"))
            End If

            Dim Piante_Maschi_InSesto = obj_Impianto.GetValue("ChkMaschiSesto")
            If Piante_Maschi_InSesto Then
                Piante_Maschi_InSesto = 1
            Else
                Piante_Maschi_InSesto = 0
            End If
            ' - - - - - - - - - - - - - - - - A N N A      F I N E - - - - - - - - - - - - - - - - - -

            Dim cul_cod = CInt(obj_Impianto.GetValue("cul_cod"))
            Dim veg_cod = 0
            If IsNumeric(obj_Impianto.GetValue("veg_cod")) Then
                veg_cod = obj_Impianto.GetValue("veg_cod")
            End If

            Dim gru_cod = 0
            If IsNumeric(obj_Impianto.GetValue("gru_cod")) Then
                gru_cod = obj_Impianto.GetValue("gru_cod")
            End If

            Dim port_cod = 0
            If IsNumeric(obj_Impianto.GetValue("port_cod")) Then
                port_cod = obj_Impianto.GetValue("port_cod")
            End If

            Dim foral_cod = 0
            If IsNumeric(obj_Impianto.GetValue("foral_cod")) Then
                foral_cod = obj_Impianto.GetValue("foral_cod")
            End If

            Dim imp_cod2 = 0
            If IsNumeric(obj_Impianto.GetValue("imp_cod2")) Then
                imp_cod2 = obj_Impianto.GetValue("imp_cod2")
            End If

            Dim provenienza_seme As Integer = 0
            If IsNumeric(obj_Impianto.GetValue("provenienzaseme")) Then
                provenienza_seme = CInt(obj_Impianto.GetValue("provenienzaseme"))
            End If

            Dim cop_cod As Integer = 0
            If IsNumeric(obj_Impianto.GetValue("cop_cod")) Then
                cop_cod = CInt(obj_Impianto.GetValue("cop_cod"))
            End If

            Dim sup_imp = CDbl(obj_Impianto.GetValue("sup_imp"))
            Dim grfi_cod = 0
            If IsNumeric(obj_Impianto.GetValue("grfi_cod")) Then
                grfi_cod = obj_Impianto.GetValue("grfi_cod")
            End If

            Dim id_cod_terreno = CInt(obj_Impianto.GetValue("id_cod_terreno"))
            Dim grva_cod = 0
            If IsNumeric(obj_Impianto.GetValue("grva_cod")) Then
                grva_cod = obj_Impianto.GetValue("grva_cod")
            End If

            Dim setup_cod = obj_Impianto.GetValue("setup_cod")

            Dim coverCrops = obj_Impianto.GetValue("ChkCoverCrops")
            If coverCrops Then
                coverCrops = 1
            Else
                coverCrops = 0
            End If

            Dim monitorato = obj_Impianto.GetValue("ChkMonitorato")
            If monitorato Then
                monitorato = 1
            Else
                monitorato = 0
            End If

            Dim resa_prevista = obj_Impianto.GetValue("Resa_Prevista")
            If resa_prevista Is Nothing Then
                resa_prevista = 0
            End If

            Dim resa_effettiva = obj_Impianto.GetValue("Resa_Effettiva")
            If resa_effettiva Is Nothing Then
                resa_effettiva = 0
            End If

            Dim cop_data_inizio = obj_Impianto.GetValue("cop_data_inizio").ToString
            If IsDate(cop_data_inizio) Then
                Try
                    cop_data_inizio = CDate(cop_data_inizio)
                Catch ex As Exception
                    cop_data_inizio = AGRODATAINIZIO
                End Try
            Else
                cop_data_inizio = AGRODATAINIZIO
            End If

            Dim cop_data_fine = obj_Impianto.GetValue("cop_data_fine").ToString
            If IsDate(cop_data_fine) Then
                Try
                    cop_data_fine = CDate(cop_data_fine)
                Catch ex As Exception
                    cop_data_fine = AGRODATAFINE
                End Try
            Else
                cop_data_fine = AGRODATAFINE
            End If

            Dim tra_fila_m = obj_Impianto.GetValue("tra_fila_m")
            Dim su_fila_m = obj_Impianto.GetValue("su_fila_m")
            Dim tecn_cod = obj_Impianto.GetValue("tecn_cod")
            If Not IsNumeric(tecn_cod) Then
                tecn_cod = 0
            End If

            Dim su_cod = obj_Impianto.GetValue("su_cod")
            If Not IsNumeric(su_cod) Then
                su_cod = 0
            End If

            Dim interbina = obj_Impianto.GetValue("interbina")
            Dim germinabilita = obj_Impianto.GetValue("germinabilita")
            Dim codiceZona = obj_Impianto.GetValue("codiceZona")
            Dim dettaglio_varieta_personalizzato = obj_Impianto.GetValue("dettaglio_varieta_personalizzato")
            Dim impianto_ibrido = obj_Impianto.GetValue("impianto_ibrido")

            Dim unita_vitata = obj_Impianto.GetValue("Unita_Vitata")
            Dim CodBMBDBT_M = obj_Impianto.GetValue("CodBMBDBT_M")
            Dim CodBMBDBT_F = obj_Impianto.GetValue("CodBMBDBT_F")
            Dim Genetica_M = obj_Impianto.GetValue("Genetica_M")
            Dim Genetica_F = obj_Impianto.GetValue("Genetica_F")
            Dim OffType_M = obj_Impianto.GetValue("OffType_M")
            Dim OffType_F = obj_Impianto.GetValue("OffType_F")
            Dim DistanzaSuFila_F = obj_Impianto.GetValue("DistanzaSuFila_F")
            Dim DistanzaTraFila_F = obj_Impianto.GetValue("DistanzaTraFila_F")

            Dim chkConsociazione_checked = obj_Impianto.GetValue("chkConsociazione_checked")
            Dim Id_Consociazione = obj_Impianto.GetValue("id_consociazione")
            'Se l'impianto non è consociato...
            If chkConsociazione_checked = False Then

                'controllo che sull'appezzamento non ci siano impianti attivi nella data che ho scelto..
                objParametri_Server.ImpostaFinestre_con_SalvataggioTemporale(validita_inizio, validita_fine)
                If Esistono_Impianti_Su_Appezzamenti(piva, sa_cod, appezza, id_reg, objParametri_Server) = True Then
                    Dim objRisposta2 = New JObject()
                    objRisposta2.Add(New JProperty("ProseguiSalvataggio", "false"))
                    objRisposta2.Add(New JProperty("TipoMessaggioRitorno", "alert"))
                    objRisposta2.Add(New JProperty("TipoControllo", "0"))
                    objRisposta2.Add(New JProperty("Messaggio", DirectCast(System.Web.HttpContext.GetLocalResourceObject("~/Anagrafica/Impianto_Edit2.aspx", "ImpossibileInserireImpiantoNellIntervalloPoichéNeEsisteUnoAttivo"), String)))
                    r.RispostaStringa = objRisposta2.ToString
                    r.RispostaOK = True
                    Return r
                End If
                objParametri_Server.ResettaFinestra()

                objRisposta.Add(New JProperty("Id_Consociazione", 0))

                'Altrimenti...
            Else

                'Recupero l'Id_Consociazione eventualmente già esistente
                Verifica_Esistenza_Impianti_Consociati(piva, sa_cod, appezza, Id_Consociazione, validita_inizio, objParametri_Server)

                'Se è il primo impianto consociato devo calcolare il nuovo id_consociazione
                If Id_Consociazione = 0 Then

                    Dim objSeq As New AgronicaCoreDataProvider.Agro_Sequenze

                    Id_Consociazione = objSeq.NuovoId_Tabella("Reg_Impianti_Consociazioni",
                                                            0,
                                                            2000000000,
                                                            objParametri_Server)
                End If

                ' memorizzo l'id consociazione
                objRisposta.Add(New JProperty("Id_Consociazione", Id_Consociazione))

            End If
            Dim dt_impianto_old As DataTable
            Dim rowImp As DataRow
            If objParametri_Agenda.Tipo_Operazione = enum_TipoOperazioneDB.Modifica Then
                dt_impianto_old = objImpR.Leggi(piva, sa_cod, appezza, id_reg, enumSelezioneVariabile.Selezione_JoinDescrizioni, "", "", objParametri_Server)
                If dt_impianto_old IsNot Nothing AndAlso dt_impianto_old.Rows.Count > 0 Then
                    rowImp = dt_impianto_old.Rows(0)
                Else
                    objRisposta.Add(New JProperty("ProseguiSalvataggio", "false"))
                    objRisposta.Add(New JProperty("TipoMessaggioRitorno", "alert"))
                    objRisposta.Add(New JProperty("TipoControllo", "0"))
                    objRisposta.Add(New JProperty("Messaggio", DirectCast(System.Web.HttpContext.GetLocalResourceObject("~/Anagrafica/Impianto_Edit2.aspx", "ErroreInFaseDiModificaImpiantoNonTrovato"), String)))
                    r.RispostaStringa = objRisposta.ToString
                    r.RispostaOK = True
                    Return r
                End If
            End If

            If objParametri_Agenda.Tipo_Operazione = enum_TipoOperazioneDB.Modifica Then
                Dim MovimentiPresenti = False
                Dim Validita_Inizio_Agenda As Date
                Dim Validita_Fine_Agenda As Date
                Dim ObjAgenda As New AgronicaCoreContabDAL.Mov_Destinazioni_R

                Dim DTAgenda As DataTable
                'Dim Id_Agenda As Integer

                'ricavo il recordset dei movimenti di produzione associati all'impianto
                DTAgenda = ObjAgenda.LeggiCronologiaMovimenti(piva,
                                                        sa_cod,
                                                        appezza,
                                                        id_reg,
                                                          enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                        "",
                                                        "",
                                                        objParametri_Server)


                ObjAgenda = Nothing

                If DTAgenda.Rows.Count > 0 Then

                    MovimentiPresenti = True

                    Validita_Inizio_Agenda = DTAgenda.Rows(0).Item("Data_Movimento")

                    Validita_Fine_Agenda = DTAgenda.Rows(DTAgenda.Rows.Count - 1).Item("Data_Movimento")

                    '(24/07/2015) eliminato perchè spostato nel core di scrittura
                    ''se ho modificato la superficie, mi salvo gli id_agenda delle operazioni da flaggare
                    'If viewstate("FlagSupModificata") = True Then
                    '    Dim i As Integer
                    '    For i = 0 To DTAgenda.Rows.Count - 1
                    '        Id_Agenda = DTAgenda.Rows(i).Item("Id_agenda")
                    '        If Not Hash_IdAgenda_ToFlag.Contains(Id_Agenda) Then
                    '            Hash_IdAgenda_ToFlag.Add(Id_Agenda, Qs_Piva)
                    '        End If
                    '    Next
                    'End If

                End If

                If MovimentiPresenti Then

                    If Validita_Inizio_Agenda < validita_inizio Then
                        objRisposta.Add(New JProperty("ProseguiSalvataggio", "false"))
                        objRisposta.Add(New JProperty("TipoMessaggioRitorno", "alert"))
                        objRisposta.Add(New JProperty("TipoControllo", "0"))
                        objRisposta.Add(New JProperty(
                            "Messaggio",
                            String.Format(
                                DirectCast(
                                    System.Web.HttpContext.GetLocalResourceObject(
                                        "~/Anagrafica/Impianto_Edit2.aspx",
                                        "ImpossibileInserireImpiantoNellIntervalloPoichéSonoRegistrateOperazioniAgendaDalAl"
                                    ),
                                    String
                                ),
                                CDate(Validita_Inizio_Agenda).ToShortDateString,
                                CDate(Validita_Fine_Agenda).ToShortDateString
                            )
                        ))

                        r.RispostaStringa = objRisposta.ToString
                        r.RispostaOK = True
                        Return r

                    End If

                    If Validita_Fine_Agenda > validita_fine Then
                        objRisposta.Add(New JProperty("ProseguiSalvataggio", "false"))
                        objRisposta.Add(New JProperty("TipoMessaggioRitorno", "alert"))
                        objRisposta.Add(New JProperty("TipoControllo", "0"))
                        objRisposta.Add(New JProperty(
                            "Messaggio",
                            String.Format(
                                DirectCast(
                                    System.Web.HttpContext.GetLocalResourceObject(
                                        "~/Anagrafica/Impianto_Edit2.aspx",
                                        "ImpossibileInserireImpiantoNellIntervalloPoichéSonoRegistrateOperazioniAgendaDalAl"
                                    ),
                                    String
                                ),
                                CDate(Validita_Inizio_Agenda).ToShortDateString,
                                CDate(Validita_Fine_Agenda).ToShortDateString
                            )
                        ))
                        r.RispostaStringa = objRisposta.ToString
                        r.RispostaOK = True
                        Return r

                    End If


                    If sup_imp <> rowImp("sup_imp") And Not saltaPrimoControllo Then

                        Dim Testo As String

                        Testo = DirectCast(System.Web.HttpContext.GetLocalResourceObject(
                            "~/Anagrafica/Impianto_Edit2.aspx", "ConfermaVariazioneSuperficieImpiantoImplicaModificareMovimentiAgendaAssociati"), String)

                        objRisposta.Add(New JProperty("ProseguiSalvataggio", "false"))
                        objRisposta.Add(New JProperty("TipoMessaggioRitorno", "conferma"))
                        objRisposta.Add(New JProperty("TipoControllo", "1"))
                        objRisposta.Add(New JProperty("Messaggio", Testo))
                        r.RispostaStringa = objRisposta.ToString
                        r.RispostaOK = True
                        Return r


                    End If

                    If veg_cod <> rowImp("veg_cod") Then
                        Dim piuApp As Boolean = False
                        Dim objMovDes As New AgronicaCoreContabDAL.Mov_Destinazioni_R
                        For ag As Integer = 0 To DTAgenda.Rows.Count - 1
                            Dim id_ag As Integer = DTAgenda.Rows(ag).Item("Id_Agenda")
                            Dim DT_dest As DataTable = objMovDes.Leggi_Dettagli_Impianti(piva, id_ag, "", "", objParametri_Server)
                            For Each dest As DataRow In DT_dest.Rows
                                If CInt(dest.Item("appezza")) <> appezza Then
                                    piuApp = True
                                    Exit For
                                End If
                            Next
                            If piuApp Then
                                Exit For
                            End If
                        Next

                        Dim Testo As String
                        Dim testo2 As String = DirectCast(System.Web.HttpContext.GetLocalResourceObject(
                            "~/Anagrafica/Impianto_Edit2.aspx", "AttenzioneImpossibileVariareSpeciePerchéAssociatiMovimentiAgendaCoinvolgentiAltriAppezzamenti"), String)

                        Testo = DirectCast(System.Web.HttpContext.GetLocalResourceObject(
                            "~/Anagrafica/Impianto_Edit2.aspx", "ConfermaVariazioneSpecieImplicaModificaMovimentiAgendaPerRicalcoloDosi"), String)

                        'Simone -- Io ho seguito sopra e l'ho fatto con un input hidden e un submit ma mi fa schifo!!! #ASPnonGuardarmi
                        Dim salta = False
                        If Not piuApp Then
                            Dim Stringa As String = "<script language='javascript'> " &
                                                    "result = window.confirm(" & Chr(34) & Testo & Chr(34) & ");" &
                                                    "if(result){$(" & Chr(34) & "#SI_NO2" & Chr(34) & ").val('1'); $(" & Chr(34) & "#Form1" & Chr(34) & ").submit()}" &
                                                    "</script>"

                            Stringa = Testo

                        Else
                            If Not saltaSecondoControllo Then
                                Dim Stringa As String = "<script language='javascript'> " &
                                                    "alert(" & Chr(34) & testo2 & Chr(34) & ");" &
                                                    "$(" & Chr(34) & "#SI_NO2" & Chr(34) & ").val('0'); $(" & Chr(34) & "#Form1" & Chr(34) & ").submit();" &
                                                    "</script>"

                                Stringa = testo2
                            Else
                                salta = True
                            End If
                        End If

                        If Not salta Then
                            objRisposta.Add(New JProperty("ProseguiSalvataggio", "false"))
                            objRisposta.Add(New JProperty("TipoMessaggioRitorno", "conferma"))
                            objRisposta.Add(New JProperty("TipoControllo", "2"))
                            objRisposta.Add(New JProperty("Messaggio", Testo))
                            r.RispostaStringa = objRisposta.ToString
                            r.RispostaOK = True
                            Return r
                        End If
                    End If
                End If
            End If

            'CONTROLLO SALVATAGGIO CODICE IMPIANTO
            Dim objImpostazioni_Utenti As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read

            Dim DTRiferimentoImpianto = objImpostazioni_Utenti.Leggi(enum_Impostazioni_Utenti.Codice_Univoco_Impianto, 2, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Utenti)

            If DTRiferimentoImpianto.Rows.Count > 0 AndAlso DTRiferimentoImpianto.Rows(0)("Impostazione_Valore_1").trim = "1" Then
                If Not String.IsNullOrEmpty(obj_Impianto.GetValue("codice_impianto")) Then
                    Dim objImpiantoCodici As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Codici_R
                    Dim filtro As String = "(Reg_Impianti_Codici.Sa_Cod <> " & sa_cod & " OR Reg_Impianti_Codici.Appezza <> " & appezza & " OR Reg_Impianti_Codici.ID_Reg <> " & id_reg & ") AND Progetto_Cod = 0 "
                    Dim dtCodImpianto = objImpiantoCodici.Leggi(piva, 0, 0, 0, "", enum_CodiciAnagrafe.Codice_Impianto, obj_Impianto.GetValue("codice_impianto"), enumSelezioneVariabile.Selezione_TabellaCompleta, filtro, "", objParametri_Server)
                    If dtCodImpianto.Rows.Count > 0 Then
                        objRisposta.Add(New JProperty("ProseguiSalvataggio", "false"))
                        objRisposta.Add(New JProperty("TipoMessaggioRitorno", "alert"))
                        objRisposta.Add(New JProperty("TipoControllo", "0"))
                        objRisposta.Add(New JProperty("Messaggio", "Codice Impianto già utilizzato"))
                        r.RispostaStringa = objRisposta.ToString
                        r.RispostaOK = True
                        Return r
                    End If
                End If
            End If

            Dim dati_distintekendo = obj_Impianto.GetValue("dati_Distinte")
            If dati_distintekendo IsNot Nothing Then

                ' verifica se è attiva la replica dei dati
                Dim pivaReplica = Replica_GIAS.VerificaConfigurazione(piva, sa_cod, objParametri_Server)

                'Dim dati_distinte = dati_distintekendo.getValue("kendo_rows")
                Dim aa = JObject.Parse(dati_distintekendo.ToString)
                Dim kr = aa.GetValue("kendo_rows")
                Dim dati_distinte = JArray.Parse(kr.ToString)
                Dim listaProgetto_Cod As New List(Of Integer)

                Dim listaProgetto_Nome As New List(Of String)

                Dim DTRiferimentoProgetto = objImpostazioni_Utenti.Leggi(enum_Impostazioni_Utenti.Codice_Univoco_Progetto, 2, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Utenti)

                For Each distinta In dati_distinte
                    Dim distinta1 = JObject.Parse(distinta.ToString)
                    Dim progetto_cod = CInt(distinta1.GetValue("Progetto_Cod"))
                    listaProgetto_Cod.Add(progetto_cod)
                    Dim progetto_nome = ""
                    If Not IsNothing(distinta1.GetValue("Progetto_Nome")) Then
                        progetto_nome = distinta1.GetValue("Progetto_Nome").ToString
                    End If

                    If DTRiferimentoProgetto.Rows.Count > 0 AndAlso DTRiferimentoProgetto.Rows(0)("Impostazione_Valore_1").trim = "1" Then
                        If Not String.IsNullOrEmpty(progetto_nome) Then
                            Dim objImprese_Progetti As New AgronicaCoreAnagrafeDAL.Impresa_Progetti_R
                            Dim filtro As String = "(Imprese_Progetti.Sa_Cod <> " & sa_cod & " OR Imprese_Progetti.Appezza <> " & appezza & " OR Imprese_Progetti.ID_Reg <> " & id_reg & " OR Imprese_Progetti.Progetto_Cod <> " & progetto_cod & ") AND Progetto_Nome = '" & progetto_nome & "' "
                            Dim dtCodImpianto = objImprese_Progetti.Leggi(piva, 0, "", 0, 0, 0, 0, 0, 0, enumSelezioneVariabile.Selezione_TabellaCompleta, filtro, "", objParametri_Server)
                            If dtCodImpianto.Rows.Count > 0 OrElse listaProgetto_Nome.Contains(progetto_nome) Then
                                objRisposta.Add(New JProperty("ProseguiSalvataggio", "false"))
                                objRisposta.Add(New JProperty("TipoMessaggioRitorno", "alert"))
                                objRisposta.Add(New JProperty("TipoControllo", "0"))
                                objRisposta.Add(New JProperty("Messaggio", "Lotto già utilizzato"))
                                r.RispostaStringa = objRisposta.ToString
                                r.RispostaOK = True
                                Return r
                            End If

                            listaProgetto_Nome.Add(progetto_nome)

                        End If
                    End If

                    Dim progetto_des = ""
                    If Not IsNothing(distinta1.GetValue("Progetto_Des")) Then
                        progetto_des = distinta1.GetValue("Progetto_Des").ToString
                    End If

                    Dim validita_inizio_progetto = CDate(distinta1.GetValue("Validita_Inizio").ToString)
                    Dim validita_fine_progetto As Date = AGRODATAFINE
                    If IsDate(distinta1.GetValue("Validita_Fine").ToString) Then
                        validita_fine_progetto = CDate(distinta1.GetValue("Validita_Fine").ToString)
                    End If

                    If progetto_nome = "" Then
                        progetto_nome = "[dal " & validita_inizio_progetto & " al " & validita_fine_progetto & "]"
                    End If

                    Dim regolamento_cod = 0
                    If Not IsNothing(distinta1.GetValue("Regolamento_Cod")) AndAlso IsNumeric(distinta1.GetValue("Regolamento_Cod")) Then
                        regolamento_cod = distinta1.GetValue("Regolamento_Cod")
                    End If

                    Dim Disciplinare_PubblicoPrivato = 0
                    If Not IsNothing(distinta1.GetValue("Disciplinare_PubblicoPrivato")) AndAlso IsNumeric(distinta1.GetValue("Disciplinare_PubblicoPrivato")) Then
                        Disciplinare_PubblicoPrivato = distinta1.GetValue("Disciplinare_PubblicoPrivato")
                    End If

                    Dim Regolamento_Concimazione_Cod = 0
                    If Not IsNothing(distinta1.GetValue("Regolamento_Concimazione_Cod")) AndAlso IsNumeric(distinta1.GetValue("Regolamento_Concimazione_Cod")) Then
                        Regolamento_Concimazione_Cod = distinta1.GetValue("Regolamento_Concimazione_Cod")
                    End If

                    Dim Finalita_Concimazione_Cod = 0
                    If Not IsNothing(distinta1.GetValue("Finalita_Concimazione_Cod")) AndAlso IsNumeric(distinta1.GetValue("Finalita_Concimazione_Cod")) Then
                        Finalita_Concimazione_Cod = distinta1.GetValue("Finalita_Concimazione_Cod")
                    End If

                    Dim Disciplinare_Cod = 0
                    If Not IsNothing(distinta1.GetValue("Disciplinare_Cod")) AndAlso IsNumeric(distinta1.GetValue("Disciplinare_Cod")) Then
                        Disciplinare_Cod = distinta1.GetValue("Disciplinare_Cod")
                    End If

                    Dim stato_impianto = 0
                    If Not IsNothing(distinta1.GetValue("stato_impianto")) AndAlso IsNumeric(distinta1.GetValue("stato_impianto")) Then
                        stato_impianto = distinta1.GetValue("stato_impianto")
                    End If

                    Dim p_ha As Double = 0
                    If Not IsNothing(distinta1.GetValue("p_ha")) AndAlso IsNumeric(distinta1.GetValue("p_ha")) Then
                        p_ha = distinta1.GetValue("p_ha")
                    End If

                    Dim PianteHa As Double = 0
                    If Not IsNothing(distinta1.GetValue("PianteHa")) AndAlso IsNumeric(distinta1.GetValue("PianteHa")) Then
                        PianteHa = distinta1.GetValue("PianteHa")
                    End If

                    '- - - - - - - - - - - - - - - - A N N A      I N I Z I O - - - - - - - - - - - - - - - -
                    Dim P_HA_Femmine As Double = 0
                    If Not IsNothing(distinta1.GetValue("P_HA_Femmine")) AndAlso IsNumeric(distinta1.GetValue("P_HA_Femmine")) Then
                        P_HA_Femmine = distinta1.GetValue("P_HA_Femmine")
                    End If

                    Dim P_HA_Maschi As Double = 0
                    If Not IsNothing(distinta1.GetValue("P_HA_Maschi")) AndAlso IsNumeric(distinta1.GetValue("P_HA_Maschi")) Then
                        P_HA_Maschi = distinta1.GetValue("P_HA_Maschi")
                    End If
                    '- - - - - - - - - - - - - - - - A N N A      F I N E - - - - - - - - - - - - - - - - - -
                    Dim TxtN = distinta1.GetValue("TxtN")
                    Dim TxtP2O5 = distinta1.GetValue("TxtP2O5")
                    Dim TxtK2O = distinta1.GetValue("TxtK2O")
                    Dim TxtMgO = distinta1.GetValue("TxtMgO")
                    Dim Organismo_Referente = distinta1.GetValue("Organismo_Referente")
                    Dim Magazzino_Conferimento = distinta1.GetValue("Magazzino_Conferimento")
                    Dim Capitolato_Privato = distinta1.GetValue("Capitolato_Privato")
                    ' - - - - - - - - - - - - - - - - A N N A      I N I Z I O - - - - - - - - - - - - - - - -
                    Dim Licenza_Coltivazione = distinta1.GetValue("Licenza_Coltivazione")
                    ' - - - - - - - - - - - - - - - - A N N A      F I N E - - - - - - - - - - - - - - - - - -

                    Dim Impianto_IAF_ImpegniAggiuntiviFacoltativi = distinta1.GetValue("Impianto_IAF_ImpegniAggiuntiviFacoltativi")
                    Dim Impianto_PianoSemina = distinta1.GetValue("Impianto_PianosalSemina")
                    Dim data_semina_prevista = distinta1.GetValue("data_semina_prevista")
                    Dim data_raccolta_prevista = distinta1.GetValue("data_raccolta_prevista")
                    Dim data_fioritura_prevista = distinta1.GetValue("data_fioritura_prevista")
                    Dim id_tr = distinta1.GetValue("id_tr")
                    Dim produzione_prevista = distinta1.GetValue("produzione_prevista")

                    Dim obj_Codici_Distinta = distinta1.GetValue("obj_Codici_Distinta")
                    Dim Altri_Codici = distinta1.GetValue("Altri_Codici")
                    Dim obj_Particelle_Distinta = distinta1.GetValue("obj_Particelle_Distinta")

                    '--- Controllo Organismo Referente
                    If Esiste_Obbligo_SalvataggioOrganismoReferente(objParametri_Utenti) = True Then
                        If IsNothing(Organismo_Referente) OrElse Organismo_Referente = "" Then
                            objRisposta.Add(New JProperty("ProseguiSalvataggio", "false"))
                            objRisposta.Add(New JProperty("TipoMessaggioRitorno", "alert"))
                            objRisposta.Add(New JProperty("TipoControllo", "0"))
                            objRisposta.Add(New JProperty("Messaggio", DirectCast(System.Web.HttpContext.GetLocalResourceObject(
                                "~/Anagrafica/Impianto_Edit2.aspx", "ObbligatorioOrganismoReferentePerEsercizioSelezionato"), String)))
                            r.RispostaStringa = objRisposta.ToString
                            r.RispostaOK = True
                            Return r
                        End If
                    End If

                    ' messaggio di conferma per ribaltamento impianto
                    Dim distinta_chiusa = distinta1.GetValue("distinta_chiusa")
                    Dim distinta_replica = distinta1.GetValue("distinta_replica")
                    Dim distinta_replica_codice = distinta1.GetValue("distinta_replica_codice")
                    If pivaReplica <> "" AndAlso (distinta_chiusa = "1" OrElse distinta_replica = "1") AndAlso distinta_replica_codice = "" Then

                        Dim objanag As New AgronicaCoreAnagrafeDAL.Imprese_Read
                        Dim rag_soc = objanag.RagSoc_from_Piva(pivaReplica, objParametri_Server)
                        Dim Testo As String
                        Testo = String.Format(DirectCast(System.Web.HttpContext.GetLocalResourceObject("~/Anagrafica/Impianto_Edit2.aspx",
                            "ConfermaSalvataggioImpiantoSuPianoColturaleAzienda"), String), rag_soc)

                        objRisposta.Add(New JProperty("ProseguiSalvataggio", "false"))
                        objRisposta.Add(New JProperty("TipoMessaggioRitorno", "conferma"))
                        objRisposta.Add(New JProperty("TipoControllo", "2"))
                        objRisposta.Add(New JProperty("Messaggio", Testo))
                        r.RispostaStringa = objRisposta.ToString
                        r.RispostaOK = True
                        Return r

                    End If


                    If validita_fine < validita_fine_progetto OrElse
                        validita_fine < validita_inizio_progetto OrElse
                        validita_inizio > validita_inizio_progetto OrElse
                        validita_inizio > validita_fine_progetto Then

                        Dim messaggio = ""
                        Dim messaggioSpecifico As Boolean = False 'Per sapere se specificare la data del movimento solo se DTAgenda.Rows.Count = 1


                        Dim objControllo As New AgronicaCoreAnagrafeBIZ.Progetto_W
                        Dim controllo = objControllo.controllo_CdG(Nothing, piva, sa_cod, appezza, id_reg, progetto_cod, validita_inizio, validita_fine, objParametri_Server)

                        If controllo.errore Then
                            Dim MessaggioErrore As String = "Esistono Costi di Gestione collegati ad un esercizio che impediscono la modifica delle date dell'impianto"

                            objRisposta.Add(New JProperty("ProseguiSalvataggio", "false"))
                            objRisposta.Add(New JProperty("TipoMessaggioRitorno", "alert"))
                            objRisposta.Add(New JProperty("TipoControllo", "0"))
                            objRisposta.Add(New JProperty("Messaggio", MessaggioErrore))

                            r.RispostaStringa = objRisposta.ToString
                            r.RispostaOK = True
                            Return r
                        End If

                    End If

                    '' controllo movimenti collegati a distinte da cancellare
                    'Dim objCDG As New AgronicaCoreContabDAL.CDG_DAL_R
                    'If objCDG.Verifica_CDG_Impianti(piva, sa_cod, appezza, id_reg, listaProgetto_Cod, objParametri_Server) Then

                    '    objRisposta.Add(New JProperty("ProseguiSalvataggio", "false"))
                    '    objRisposta.Add(New JProperty("TipoMessaggioRitorno", "alert"))
                    '    objRisposta.Add(New JProperty("TipoControllo", "0"))
                    '    objRisposta.Add(New JProperty("Messaggio", DirectCast(System.Web.HttpContext.GetLocalResourceObject("~/Anagrafica/Impianto_Edit2.aspx",
                    '        "ImpossibileCancellareEserciziConMovimentiCollegati"), String)))
                    '    r.RispostaStringa = objRisposta.ToString
                    '    r.RispostaOK = True
                    '    Return r
                    'End If

                Next

            End If

            objRisposta.Add(New JProperty("ProseguiSalvataggio", "true"))
            objRisposta.Add(New JProperty("TipoMessaggioRitorno", "salva"))
            objRisposta.Add(New JProperty("TipoControllo", "0"))
            objRisposta.Add(New JProperty("Messaggio", DirectCast(System.Web.HttpContext.GetLocalResourceObject("~/Anagrafica/Impianto_Edit2.aspx", "ProseguiSalvataggio"), String)))
            r.RispostaStringa = objRisposta.ToString
            r.RispostaOK = True


        Catch ex As Exception

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.RispostaOK = False
        End Try

        Return r

    End Function

    '###############################################################################
    '''Usato
    '''Versione NEW_COM non standard
    Public Shared Function Esiste_Obbligo_SalvataggioOrganismoReferente(objParametri_Utenti As AgronicaCoreParametri) As Boolean

        Dim Dt As DataTable

        Dim objUt_imp As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
        'questa impostazione è legata al superuser, quindi devo passare la sua username
        'Dt = NewCom_UtentiImpostazioni_Leggi(objServer, objSession, objPage,
        '                                        objSession("ASG_SuperUser_CodFiscale").ToString,
        '                                        objSession("ASG_SuperUser_Username").ToString,
        '                                        enum_Impostazioni_Utenti.SUPERUSER_COD_ORG_REFERENTE_OBBLIGATORIO,
        '                                        "")

        Dt = objUt_imp.Leggi(enum_Impostazioni_Utenti.SUPERUSER_COD_ORG_REFERENTE_OBBLIGATORIO, 1, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Utenti)

        If Not IsNothing(Dt) Then

            If Dt.Rows.Count <> 0 Then
                If Dt.Rows(0).Item("Impostazione_Valore_1") = 1 Then
                    Return True
                Else
                    Return False
                End If
            Else
                Return False
            End If
        Else
            Return False
        End If

    End Function

    '################################################################################
    Public Shared Function Esistono_Impianti_Su_Appezzamenti(
                                            ByVal Piva As String,
                                            ByVal Sa_Cod As Integer,
                                            ByVal Appezza As Integer,
                                            ByVal Id_Reg As Integer,
                                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                ) As Boolean

        Dim DTImpianto As DataTable

        'Creo gli oggetti COM+
        Dim objImpianto As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read  'New Agro_Anagrafe_AD.Reg_Impianti_Read
        'objImpianto = objServer....CreateCANCELLATOObject("Agro_Anagrafe_AD.Reg_Impianti_Read")

        'Recupero le info dell'impianto

        DTImpianto = objImpianto.Leggi(
                                    CStr(Piva),
                                    CInt(Sa_Cod),
                                    CInt(Appezza),
                                    0,
                                     AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                     "ID_REG <> " & Id_Reg,
                                     "", objParametri)

        'Distruggo l'oggetto COM+
        objImpianto = Nothing

        If DTImpianto.Rows.Count > 0 Then
            Return True
        Else
            Return False
        End If


    End Function

    '#########################################################################################
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Esistono_Impianti_Su_Appezzamenti(ByVal data_inizio As String, ByVal data_fine As String) As RispostaStandard
        Dim r As New RispostaStandard With {
            .RispostaOK = True,
            .RispostaStringa = "ok"
        }

        Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri
        Dim objParametriAgenda As New ParametriAgenda

        Lingua.Gias_InizializzaCultura_DaSession()

        If objParametriAgenda.Tipo_Operazione <> enum_TipoOperazioneDB.Modifica Then

            objParametri_Server = HttpContext.Current.Session("ASG_objParametri_Server")
            objParametri_Server.ImpostaFinestre_con_SalvataggioTemporale(data_inizio, data_fine)

            Dim DTImpianto As DataTable

            'Creo gli oggetti COM+
            Dim objImpianto As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read  'New Agro_Anagrafe_AD.Reg_Impianti_Read
            'objImpianto = objServer....CreateCANCELLATOObject("Agro_Anagrafe_AD.Reg_Impianti_Read")

            'Recupero le info dell'impianto

            DTImpianto = objImpianto.Leggi(
                                        CStr(objParametriAgenda.Piva),
                                        CInt(objParametriAgenda.Sa_Cod),
                                        CInt(objParametriAgenda.Appezza),
                                        0,
                                         AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                         "ID_REG <> " & 0,
                                         "", objParametri_Server)

            'Distruggo l'oggetto COM+
            objImpianto = Nothing

            If DTImpianto.Rows.Count > 0 Then
                r.RispostaOK = False
                r.Errore = DirectCast(System.Web.HttpContext.GetLocalResourceObject("~/Anagrafica/Impianto_Edit2.aspx", "EsisteGiàUnImpiantoConLeDateSelezionate"), String)

            End If

            objParametri_Server.ResettaFinestra()

        End If

        Return r

    End Function



    '#########################################################################################
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Aggiungi_Codice(ByVal codice As String, ByVal valore As String, ByVal codice_id As String) As RispostaStandard

        Dim r As New RispostaStandard

        Dim Dt As New DataTable
        Dim Dr As DataRow
        'Dim Contatore As Integer
        Dim flag As Boolean = True

        Lingua.Gias_InizializzaCultura_DaSession()

        If (IsNothing(HttpContext.Current.Session("dt_Codici_Ana"))) Then
            Dt.Columns.Add(New DataColumn("Id_Cod", GetType(Integer)))
            Dt.Columns.Add(New DataColumn("Descrizione", GetType(String)))
            Dt.Columns.Add(New DataColumn("Val_Cod", GetType(String)))
            'Dt.Columns.Add(New DataColumn("Validita_Inizio", GetType(String)))
            'Dt.Columns.Add(New DataColumn("Validita_Fine", GetType(String)))
        Else
            Dt = HttpContext.Current.Session("dt_Codici_Ana")
        End If


        'Contatore = 1
        'Dim i As Integer
        Dim objCodiceAnagrafeR As New AgronicaCoreAnagrafeDAL.Codice_Anagrafe_R

        'For i = 0 To Dt.Rows.Count - 1
        '    Contatore = Contatore + 1
        'Next




        'Controllo se esiste già la voce che si vuole inserire
        For i = 0 To Dt.Rows.Count - 1

            If (Dt.Rows(i).Item("Id_Cod") = codice_id) Then
                flag = False
            End If
        Next


        If (flag) Then

            'Creo una nuova riga
            Dr = Dt.NewRow

            'Definisco i valori

            'Dr.Item("Contatore") = Contatore


            Dr.Item("Id_Cod") = CInt(codice_id)
            'Dr.Item("Descrizione") = objCodiceAnagrafeR.CodiceAnagrafeDes_from_CodiceAnagrafeCod(codice_id, HttpContext.Current.Session("ASG_objParametri_Server"))
            Dr.Item("Descrizione") = codice
            Dr.Item("Val_Cod") = valore

            'Dr.Item("Validita_Inizio") = xValiditaInizio

            'Dr.Item("Validita_Fine") = xValiditaFine

            'Associo alla tabella la nuova riga creata
            Dt.Rows.Add(Dr)
            HttpContext.Current.Session("dt_Codici_Ana") = Dt
            Dim str_Risposta = DT_to_Json_Codici(Dt)
            r.RispostaOK = True
            r.RispostaStringa = str_Risposta
        Else
            r.RispostaOK = False
            r.Errore = AgronicaAgenda_2010.CodiceGiàInserito
        End If

        Return r

    End Function


    '#########################################################################################
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Aggiungi_Particella(ByVal nome As String, ByVal valore As String, ByVal codice_id As String) As RispostaStandard

        Dim r As New RispostaStandard

        Dim Dt As New DataTable
        Dim Dr As DataRow
        'Dim Contatore As Integer
        Dim flag As Boolean = True

        Lingua.Gias_InizializzaCultura_DaSession()

        If (IsNothing(HttpContext.Current.Session("DT_Particelle"))) Then
            Dt.Columns.Add(New DataColumn("Id_Cod", GetType(Integer)))
            Dt.Columns.Add(New DataColumn("Nome", GetType(String)))
            Dt.Columns.Add(New DataColumn("Val_Cod", GetType(String)))
            'Dt.Columns.Add(New DataColumn("Validita_Inizio", GetType(String)))
            'Dt.Columns.Add(New DataColumn("Validita_Fine", GetType(String)))
        Else
            Dt = HttpContext.Current.Session("dt_Codici_Ana")
        End If


        'Contatore = 1
        'Dim i As Integer
        Dim objCodiceAnagrafeR As New AgronicaCoreAnagrafeDAL.Codice_Anagrafe_R

        'For i = 0 To Dt.Rows.Count - 1
        '    Contatore = Contatore + 1
        'Next




        'Controllo se esiste già la voce che si vuole inserire
        For i = 0 To Dt.Rows.Count - 1

            If (Dt.Rows(i).Item("Id_Cod") = codice_id) Then
                flag = False
            End If
        Next


        If (flag) Then

            'Creo una nuova riga
            Dr = Dt.NewRow

            'Definisco i valori

            'Dr.Item("Contatore") = Contatore


            Dr.Item("Id_Cod") = codice_id
            'Dr.Item("Descrizione") = objCodiceAnagrafeR.CodiceAnagrafeDes_from_CodiceAnagrafeCod(codice_id, HttpContext.Current.Session("ASG_objParametri_Server"))
            Dr.Item("Nome") = nome
            Dr.Item("Val_Cod") = valore

            'Dr.Item("Validita_Inizio") = xValiditaInizio

            'Dr.Item("Validita_Fine") = xValiditaFine

            'Associo alla tabella la nuova riga creata
            Dt.Rows.Add(Dr)
            HttpContext.Current.Session("DT_Particelle") = Dt
            Dim str_Risposta = DT_to_Json_Particelle(Dt)
            r.RispostaOK = True
            r.RispostaStringa = str_Risposta
        Else
            r.RispostaOK = False
            r.Errore = AgronicaAgenda_2010.CodiceGiàInserito
        End If

        Return r

    End Function

    '#########################################################################################
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Salva_Cmb_CapitolatoPrivato(ByVal valore As String)

        HttpContext.Current.Session("Cmb_CapitolatoPrivato") = valore

    End Function


    '#########################################################################################
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Salva_Cmb_MagazzinoConferimento(ByVal valore As String)

        HttpContext.Current.Session("Cmb_MagazzinoConferimento") = valore

    End Function


    '#########################################################################################
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Salva_Cmb_OrganismoReferente(ByVal valore As String)

        HttpContext.Current.Session("Cmb_OrganismoReferente") = valore

    End Function

    '#########################################################################################
    ' - - - - - - - - - - - - - - - - A N N A      I N I Z I O - - - - - - - - - - - - - - - -
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Salva_Cmb_LicenzaColtivazione(ByVal valore As String)

        HttpContext.Current.Session("Cmb_LicenzaColtivazione") = valore

    End Function
    ' - - - - - - - - - - - - - - - - A N N A      F I N E - - - - - - - - - - - - - - - - - -

    '#########################################################################################
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Salva_Cmb_Riferimento_Trasferimento_Dati(ByVal valore As String)

        HttpContext.Current.Session("Cmb_Riferimento_Trasferimento_Dati") = valore

    End Function

    '#########################################################################################
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Salva_Cmb_RegolamentoConc(ByVal valore As String)

        HttpContext.Current.Session("Cmb_RegolamentoConc") = valore

    End Function

    '#########################################################################################
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Salva_Cmb_FinalitaConc(ByVal valore As String)

        HttpContext.Current.Session("Cmb_FinalitaConc") = valore

    End Function

    '#########################################################################################
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Salva_Cmb_Stato(ByVal valore As String)

        HttpContext.Current.Session("Cmb_Stato") = valore

    End Function

    '#########################################################################################
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Salva_Cmb_Regolamento(ByVal valore As String)

        HttpContext.Current.Session("Cmb_Regolamento") = valore

    End Function

    '#########################################################################################
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Salva_Cmb_Specie(ByVal valore As String)

        HttpContext.Current.Session("Cmb_Specie") = valore

    End Function

    '#########################################################################################
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Salva_Cmb_Cultivar(ByVal valore As String)

        HttpContext.Current.Session("Cmb_Cultivar") = valore

    End Function


    '#########################################################################################
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Salva_Cmb_Finalita(ByVal valore As String)

        HttpContext.Current.Session("Cmb_Finalita") = valore

    End Function


    '#########################################################################################
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Salva_Cmb_TipologiaVarietale(ByVal valore As String)

        HttpContext.Current.Session("Cmb_TipologiaVarietale") = valore

    End Function


    '#########################################################################################
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Salva_Cmb_Copertura(ByVal valore As String)

        HttpContext.Current.Session("Cmb_Copertura") = valore

    End Function


    '#########################################################################################
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Salva_Cmb_FormaAllevamento(ByVal valore As String)

        HttpContext.Current.Session("Cmb_FormaAllevamento") = valore

    End Function


    '#########################################################################################
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Salva_Cmb_Portinnesto(ByVal valore As String)

        HttpContext.Current.Session("Cmb_Portinnesto") = valore

    End Function


    '#########################################################################################
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Salva_Cmb_DettaglioVarietaPersonalizzato(ByVal valore As String)

        HttpContext.Current.Session("Cmb_DettaglioVarietaPersonalizzato") = valore

    End Function
    '#########################################################################################
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Salva_Cmb_CodiciTerreno(ByVal valore As String)

        HttpContext.Current.Session("Cmb_CodiciTerreno") = valore

    End Function


    '#########################################################################################
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Salva_Cmb_ImpIrrigazione(ByVal valore As String)

        HttpContext.Current.Session("Cmb_ImpIrrigazione") = valore

    End Function


    '#########################################################################################
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Salva_Cmb_ProvenienzaSeme(ByVal valore As String)

        HttpContext.Current.Session("Cmb_ProvenienzaSeme") = valore

    End Function


    '#########################################################################################
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Salva_Cmb_SeminaTrapianto(ByVal valore As String)

        HttpContext.Current.Session("Cmb_SeminaTrapianto") = valore

    End Function

    '#########################################################################################
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function CaricaCombo_ConduzioneTra(ByVal Veg_Cod As Integer) As RispostaStandard

        Dim r As New RispostaStandard

        Try

            Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri = New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))
            Lingua.Gias_InizializzaCultura_DaSession()

            Dim objSpecie As New AgronicaCoreMetaSchemaDAL.SpecieVegetali_R
            Dim dtSpecie = objSpecie.Leggi(Veg_Cod, 0, "", "", enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)
            Dim gru_cod = 0
            If dtSpecie IsNot Nothing AndAlso dtSpecie.Rows.Count > 0 Then
                gru_cod = dtSpecie.Rows(0).Item("Gru_Cod")
            End If
            Dim objCOM As New AgronicaCoreAnagrafeDAL.ConduzioneTraFila_R  'Agro_Anagrafe_AD.ConduzioneTraFila_R
            Dim DT As DataTable


            'Creo gli oggetti COM
            'objCOM = objServer.CANCELLARE.CreateCANCELLATOObject("Agro_Anagrafe_AD.ConduzioneTraFila_R")

            'Leggo le imprese associate al profilo selezionato			
            DT = objCOM.Leggi(0, gru_cod,
                             enumSelezioneVariabile.Selezione_TabellaCompleta, "",
                            "",
                            objParametri_Server)

            Dim JArrayLista As New JArray()
            For Each dr In DT.Rows
                JArrayLista.Add(New JObject(New JProperty("Tecn_Cod", dr.Item("Tecn_Cod")),
                                            New JProperty("Tecn_Des", dr.Item("Tecn_Des"))))
            Next

            r.RispostaStringa = JsonConvert.SerializeObject(JArrayLista, Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r
    End Function

    '#########################################################################################
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function CaricaCombo_ConduzioneSu(ByVal Veg_Cod As Integer) As RispostaStandard

        Dim r As New RispostaStandard

        Try

            Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri = New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))
            Lingua.Gias_InizializzaCultura_DaSession()

            Dim objSpecie As New AgronicaCoreMetaSchemaDAL.SpecieVegetali_R
            Dim dtSpecie = objSpecie.Leggi(Veg_Cod, 0, "", "", enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)
            Dim gru_cod = 0
            If dtSpecie IsNot Nothing AndAlso dtSpecie.Rows.Count > 0 Then
                gru_cod = dtSpecie.Rows(0).Item("Gru_Cod")
            End If
            Dim objCOM As New AgronicaCoreAnagrafeDAL.ConduzioneSuFila_R  'Agro_Anagrafe_AD.ConduzioneSuFila_R
            Dim DT As DataTable


            'Creo gli oggetti COM
            'objCOM = objServer.CANCELLARE.CreateCANCELLATOObject("Agro_Anagrafe_AD.ConduzioneTraFila_R")

            'Leggo le imprese associate al profilo selezionato			
            DT = objCOM.Leggi(0, gru_cod,
                             enumSelezioneVariabile.Selezione_TabellaCompleta, "",
                            "",
                            objParametri_Server)

            Dim JArrayLista As New JArray()
            For Each dr In DT.Rows
                JArrayLista.Add(New JObject(New JProperty("Tecn_Cod", dr.Item("Tecn_Cod")),
                                            New JProperty("Tecn_Des", dr.Item("Tecn_Des"))))
            Next

            r.RispostaStringa = JsonConvert.SerializeObject(JArrayLista, Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r
    End Function


    '#########################################################################################
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Carica_Select_Cultivar(ByVal parametro As String) As String

        Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri = New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))
        Lingua.Gias_InizializzaCultura_DaSession()

        Dim cmb_cultivar As New DropDownList

        'AgronicaCoreUtility.CaricaListControl.ImpreseConFiltroUtente(cmb_cultivar, True, "SELEZIONA", "", _
        '                                                                 " Imprese.rag_soc LIKE '%" & parametro & "%'", " ORDER BY Rag_Soc asc", _
        '                                                                 HttpContext.Current.Session("ASG_objParametri_Server"), _
        '                                                                 HttpContext.Current.Session("ASG_objParametri_Utenti"))
        AgronicaCoreUtility.CaricaListControl.Cultivar(cmb_cultivar, True, AgronicaAgenda_2010.Seleziona.ToUpper(), "", parametro, 0, "", True, 0, 0, "", "",
                                                                         objParametri_Server,
                                                                         HttpContext.Current.Session("ASG_objParametri_Utenti"))

        Dim rval As String = ""
        For Each itm As ListItem In cmb_cultivar.Items
            rval &= "<option value=""" & itm.Value & """>" & itm.Text & "</option>"
        Next

        'Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        'HttpContext.Current.Session("prova") = "caio"

        Return rval

    End Function

    '#########################################################################################
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Carica_Select_Finalita(ByVal parametro As String) As String

        Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri = New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))
        Lingua.Gias_InizializzaCultura_DaSession()

        Dim cmb_finalita As New DropDownList

        AgronicaCoreUtility.CaricaListControl.Finalita(cmb_finalita, True, AgronicaAgenda_2010.Seleziona.ToUpper(), "", parametro, 0, "", "", "",
                                                                         objParametri_Server)

        Dim rval As String = ""
        For Each itm As ListItem In cmb_finalita.Items
            rval &= "<option value=""" & itm.Value & """>" & itm.Text & "</option>"
        Next


        Return rval

    End Function


    '#########################################################################################
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Carica_Select_TipologiaVarietale(ByVal parametro As String) As String
        Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri = New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))
        Lingua.Gias_InizializzaCultura_DaSession()

        Dim cmb_tv As New DropDownList

        AgronicaCoreUtility.CaricaListControl.GruppoVarietalexSpecie(cmb_tv, True, AgronicaAgenda_2010.Seleziona.ToUpper(), "", parametro, "", "",
                                                                         objParametri_Server)

        Dim rval As String = ""
        For Each itm As ListItem In cmb_tv.Items
            rval &= "<option value=""" & itm.Value & """>" & itm.Text & "</option>"
        Next


        Return rval

    End Function


    '#########################################################################################
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Carica_Select_Allevamenti(ByVal parametro As String) As String

        Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri = New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))
        Lingua.Gias_InizializzaCultura_DaSession()

        Dim cmb_all As New DropDownList

        AgronicaCoreUtility.CaricaListControl.CaricaCombo_FormaAllevamento(cmb_all, True, AgronicaAgenda_2010.Seleziona.ToUpper(), "", parametro, 0, "", "", "",
                                                                     objParametri_Server)
        'AgronicaCoreUtility.CaricaListControl.GruppoVarietalexSpecie(cmb_tv, True, "SELEZIONA", "", parametro, "", "", _
        '                                                                 HttpContext.Current.Session("ASG_objParametri_Server"))

        Dim rval As String = ""
        For Each itm As ListItem In cmb_all.Items
            rval &= "<option value=""" & itm.Value & """>" & itm.Text & "</option>"
        Next


        Return rval

    End Function



    '#########################################################################################
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Carica_Select_Portinnesto(ByVal parametro As String) As String
        Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri = New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))
        Lingua.Gias_InizializzaCultura_DaSession()

        Dim cmb_inn As New DropDownList

        AgronicaCoreUtility.CaricaListControl.CaricaCombo_Portinnesto(cmb_inn, True, AgronicaAgenda_2010.Seleziona.ToUpper(), "", parametro, 0, "", "", "",
                                                                     objParametri_Server)
        'AgronicaCoreUtility.CaricaListControl.GruppoVarietalexSpecie(cmb_tv, True, "SELEZIONA", "", parametro, "", "", _
        '                                                                 HttpContext.Current.Session("ASG_objParametri_Server"))

        Dim rval As String = ""
        For Each itm As ListItem In cmb_inn.Items
            rval &= "<option value=""" & itm.Value & """>" & itm.Text & "</option>"
        Next


        Return rval

    End Function



    '#########################################################################################
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Carica_Select_Copertura(ByVal parametro As String) As String
        Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri = New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))
        Lingua.Gias_InizializzaCultura_DaSession()

        Dim cmb_cop As New DropDownList

        'AgronicaCoreUtility.CaricaListControl.CaricaCombo_Portinnesto(cmb_inn, True, "SELEZIONA", "", parametro, 0, "", "", "", _
        '                                                             HttpContext.Current.Session("ASG_objParametri_Server"))
        AgronicaCoreUtility.CaricaListControl.Copertura(cmb_cop, True, AgronicaAgenda_2010.Seleziona.ToUpper(), "", parametro, 0, "", "", "", objParametri_Server)


        Dim rval As String = ""
        For Each itm As ListItem In cmb_cop.Items
            rval &= "<option value=""" & itm.Value & """>" & itm.Text & "</option>"
        Next


        Return rval

    End Function


    '#########################################################################################
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Carica_Select_Regolamento(ByVal parametro As String) As String

        Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri = New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))

        Dim cmb_reg As New DropDownList

        If parametro = "" Then
            AgronicaCoreUtility.CaricaListControl.Regolamento(CType(cmb_reg, ListControl),
                                                               False, "", "",
                                                               "", "", objParametri_Server)
        Else

            AgronicaCoreUtility.CaricaListControl.Regolamento(CType(cmb_reg, ListControl),
                                                             False, "", "",
                                                             " Reg_Cod = 4", "", objParametri_Server)
        End If


        Dim rval As String = ""
        For Each itm As ListItem In cmb_reg.Items
            rval &= "<option value=""" & itm.Value & """>" & itm.Text & "</option>"
        Next


        Return rval

    End Function


    '#########################################################################################
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Carica_Select_MagazzinoConferimento() As String
        'Public Shared Function Carica_Select_MagazzinoConferimento(ByVal parametro As String) As String

        Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri = New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))

        Dim cmb_cop As New DropDownList

        'AgronicaCoreUtility.CaricaListControl.MagazzinoConferimento_OrgReferente(
        '                                cmb_cop,
        '                                    True, "", "",
        '                                parametro,
        '                                objParametri_Server)

        AgronicaCoreUtility.CaricaListControl.MagazzinoConferimento_OrgReferente(
                                                     cmb_cop,
                                                    True, "", "",
                                                     "",
                                                     objParametri_Server)

        'AgronicaCoreUtility.CaricaListControl.Copertura(cmb_cop, True, "SELEZIONA", "", parametro, 0, "", "", "", HttpContext.Current.Session("ASG_objParametri_Server"))


        Dim rval As String = ""
        For Each itm As ListItem In cmb_cop.Items
            rval &= "<option value=""" & itm.Value & """>" & itm.Text & "</option>"
        Next


        Return rval

    End Function



    '#########################################################################################
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function EliminaProgetto(ByVal progetto_cod As String) As RispostaStandard
        Dim r As New RispostaStandard
        Dim Dt As DataTable
        Dt = HttpContext.Current.Session("Dt_Progetti")

        'Controllo se esiste già la voce che si vuole inserire
        For i = 0 To Dt.Rows.Count - 1
            If (Dt.Rows(i).Item("Progetto_Cod") = progetto_cod) Then
                Dt.Rows.RemoveAt(i)
                Exit For
            End If
        Next
        HttpContext.Current.Session("progetto_cod") = progetto_cod

        HttpContext.Current.Session("Dt_Progetti") = Dt
        Dim str_Risposta = DT_to_Json_Progetti(Dt)
        r.RispostaOK = True
        r.RispostaStringa = str_Risposta
        Return r
    End Function


    '#########################################################################################
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function DT_to_Json_Progetti(ByVal dt As DataTable) As String

        Dim objParametriAgenda As Integer
        objParametriAgenda = HttpContext.Current.Session("operazione")

        Lingua.Gias_InizializzaCultura_DaSession()

        'creo la lista delle colonne da visualizzare
        Dim l As New List(Of ColonneNome)

        'aggiungo i pulsanti per modifica ed eliminazione
        Dim c = New ColonneNome("Progetto_Cod", "Tool", "string")

        Dim listaBtn = New List(Of btnAzioni)

        If objParametriAgenda <> enum_TipoOperazioneDB.Lettura Then
            listaBtn.Add(New btnAzioni("fa-info info_elem", "InfoProgetti(this);"))
            listaBtn.Add(New btnAzioni("fa-trash-o del_elem", "EliminaProgetti(this);"))

        End If



        Dim tool As New ToolStandard(listaBtn)
        tool.cssColonna = "colmodmovimenti"
        c._FormatoParticolare = tool.toString()
        c._Filtrabile = False
        c._ColonnaDiSelezione = True
        l.Add(c)

        Dim cn As New ColonneNome("anno_validita", AgronicaAgenda_2010.Anno, "string")
        l.Add(cn)

        cn = New ColonneNome("Progetto_Nome", DirectCast(System.Web.HttpContext.GetLocalResourceObject("~/Anagrafica/Impianto_Edit2.aspx", "NomeProgetto"), String), "string")
        l.Add(cn)

        'cn = New ColonneNome(dt.Columns("Validita_Inizio"), "V_I")
        cn = New ColonneNome("Validita_Inizio", AgronicaAgenda_2010.DataInizio, "date")
        l.Add(cn)

        cn = New ColonneNome("Validita_Fine", AgronicaAgenda_2010.DataFine, "date")
        l.Add(cn)



        'ritorno la tabella trasformata in json (aggiustando anche le colonne)
        Dim js As New AgronicaCoreDataProvider.JSON_DataTable
        Dim risp As String = js.JSON_DataTable(dt, l)
        Return risp
    End Function


    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function DT_to_Json_Codici(ByVal dt As DataTable) As String

        Dim objParametriAgenda As Integer
        objParametriAgenda = HttpContext.Current.Session("operazione")

        Lingua.Gias_InizializzaCultura_DaSession()

        'creo la lista delle colonne da visualizzare
        Dim l As New List(Of ColonneNome)

        'aggiungo i pulsanti per modifica ed eliminazione
        Dim c = New ColonneNome("Id_Cod", "Tool", "string")

        Dim listaBtn = New List(Of btnAzioni)

        If objParametriAgenda <> enum_TipoOperazioneDB.Lettura Then
            listaBtn.Add(New btnAzioni("fa-pencil-square-o edit_elem", "ModificaCodice(this);"))
            listaBtn.Add(New btnAzioni("fa-trash-o del_elem", "EliminaCodice(this);"))
        End If

        Dim tool As New ToolStandard(listaBtn)
        tool.cssColonna = "colmodmovimenti"
        c._FormatoParticolare = tool.toString()
        c._Filtrabile = False
        c._ColonnaDiSelezione = True
        l.Add(c)


        Dim cn As New ColonneNome("descrizione", AgronicaAgenda_2010.Codice, "string")
        l.Add(cn)

        cn = New ColonneNome("Val_Cod", AgronicaAgenda_2010.Valore, "string")
        l.Add(cn)

        'cn = New ColonneNome("Validita_Inizio", "Dal", "string")
        'l.Add(cn)

        'cn = New ColonneNome("Validita_Fine", "Al", "string")
        'l.Add(cn)



        'ritorno la tabella trasformata in json (aggiustando anche le colonne)
        Dim js As New AgronicaCoreDataProvider.JSON_DataTable
        Dim risp As String = js.JSON_DataTable(dt, l)
        Return risp
    End Function


    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function DT_to_Json_Particelle(ByVal dt As DataTable) As String

        Dim objParametriAgenda As Integer
        objParametriAgenda = HttpContext.Current.Session("operazione")

        Lingua.Gias_InizializzaCultura_DaSession()

        'creo la lista delle colonne da visualizzare
        Dim l As New List(Of ColonneNome)

        'aggiungo i pulsanti per modifica ed eliminazione
        Dim c = New ColonneNome("Id_Cod", "Tool", "string")

        Dim listaBtn = New List(Of btnAzioni)

        If objParametriAgenda <> enum_TipoOperazioneDB.Lettura Then
            listaBtn.Add(New btnAzioni("fa-pencil-square-o edit_elem", "ModificaCodice(this);"))
            listaBtn.Add(New btnAzioni("fa-trash-o del_elem", "EliminaCodice(this);"))
        End If

        Dim tool As New ToolStandard(listaBtn)
        tool.cssColonna = "colmodmovimenti"
        c._FormatoParticolare = tool.toString()
        c._Filtrabile = False
        c._ColonnaDiSelezione = True
        l.Add(c)


        Dim cn As New ColonneNome("Nome", AgronicaAgenda_2010.Nome, "string")
        l.Add(cn)

        cn = New ColonneNome("Val_Cod", AgronicaAgenda_2010.Valore, "string")
        l.Add(cn)

        'cn = New ColonneNome("Validita_Inizio", "Dal", "string")
        'l.Add(cn)

        'cn = New ColonneNome("Validita_Fine", "Al", "string")
        'l.Add(cn)



        'ritorno la tabella trasformata in json (aggiustando anche le colonne)
        Dim js As New AgronicaCoreDataProvider.JSON_DataTable
        Dim risp As String = js.JSON_DataTable(dt, l)
        Return risp
    End Function


    '#########################################################################################
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Controlla_Date_Distinta_Su_Storico(ByVal data_inizio As String, ByVal data_fine As String, ByVal modalita As Integer, ByVal chiave As Integer) As String


        Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri = New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))

        Dim objProgetto As New AgronicaCoreAnagrafeDAL.Impresa_Progetti_R
        Dim Dt As DataTable

        Dim d_inizio As Date
        Dim d_fine As Date

        Dim filtro_agg As String

        If modalita = "2" Then

            filtro_agg = "Progetto_Cod !=" & chiave
        Else
            filtro_agg = ""

        End If

        d_inizio = Date.Parse(data_inizio)
        d_fine = Date.Parse(data_fine)

        Dim objParametriAgenda As New ParametriAgenda

        Dt = objProgetto.Leggi(CStr(objParametriAgenda.Piva),
                            0,
                            CInt(9100),
                            0,
                            CInt(objParametriAgenda.Sa_Cod),
                            CInt(objParametriAgenda.Appezza),
                            CInt(objParametriAgenda.Id_Imp),
                            0, 0,
                            enumSelezioneVariabile.Selezione_TabellaCompleta,
                            filtro_agg,
                            " Imprese_Progetti.Validita_Inizio DESC ",
                            objParametri_Server)


        Dim risp As String
        Dim flag As Boolean = True

        ' Ciclo su tutte le Distinte e controllo se ci sono sovrapposizioni
        For i = 0 To Dt.Rows.Count - 1

            If (d_inizio >= Dt.Rows(i).Item("Validita_Inizio") And d_inizio <= Dt.Rows(i).Item("Validita_Fine")) Then
                flag = False
                Exit For
            End If

            If (d_fine >= Dt.Rows(i).Item("Validita_Inizio") And d_fine <= Dt.Rows(i).Item("Validita_Fine")) Then
                flag = False
                Exit For
            End If

            ' Se la nuova Distinta ne contiene interamente un'altra
            If (d_inizio < Dt.Rows(i).Item("Validita_Inizio") And d_fine > Dt.Rows(i).Item("Validita_Fine")) Then
                flag = False
                Exit For
            End If

        Next

        'ritorno la tabella trasformata in json (aggiustando anche le colonne)
        'Dim js As New AgronicaCoreDataProvider.JSON_DataTable
        'Dim risp As String = AgronicaCoreDataProvider.JSON_DataTable.Create_rows(Dt, JSON_DataTable.getListaColonneFromDT(Dt))

        If flag Then
            risp = "ok"
        Else
            risp = ""
        End If

        Return risp
    End Function


    '#########################################################################################
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Esiste_Obbligo_SalvataggioOrganismoReferente() As String

        Dim Dt As DataTable

        Dim objUtente As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read

        Dt = objUtente.Leggi(enum_Impostazioni_Utenti.SUPERUSER_COD_ORG_REFERENTE_OBBLIGATORIO,
                                2,
                                enumSelezioneVariabile.Selezione_TabellaCompleta,
                                 "", "", HttpContext.Current.Session("ASG_objParametri_Utenti")
                             )


        Dim ret As String

        If Not IsNothing(Dt) Then

            If Dt.Rows.Count <> 0 Then
                If Dt.Rows(0).Item("Impostazione_Valore_1") = 1 Then
                    'Return True
                    ret = ""
                Else
                    'Return False
                    ret = "ok"
                End If
            Else
                'Return False
                ret = "ok"
            End If
        Else
            'Return False
            ret = "ok"
        End If

    End Function


    '#########################################################################################
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Trova_Impianto(ByVal progetto_cod As String) As String

        Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri = New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))


        Dim objProgetto As New AgronicaCoreAnagrafeDAL.Impresa_Progetti_R
        Dim Dt As DataTable

        Dim objParametriAgenda As New ParametriAgenda

        Dt = objProgetto.Leggi(CStr(objParametriAgenda.Piva),
                            progetto_cod,
                            CInt(9100),
                            0,
                            CInt(objParametriAgenda.Sa_Cod),
                            CInt(objParametriAgenda.Appezza),
                            CInt(objParametriAgenda.Id_Imp),
                            0, 0,
                            enumSelezioneVariabile.Selezione_TabellaCompleta,
                            "",
                            " Imprese_Progetti.Validita_Inizio DESC ",
                            objParametri_Server)

        'creo la lista delle colonne da visualizzare
        Dim l As New List(Of ColonneNome)

        'ritorno la tabella trasformata in json (aggiustando anche le colonne)
        Dim js As New AgronicaCoreDataProvider.JSON_DataTable
        Dim risp As String = AgronicaCoreDataProvider.JSON_DataTable.Create_rows(Dt, JSON_DataTable.getListaColonneFromDT(Dt))

        Riempi_Tab_Codici(progetto_cod)

        Return risp
    End Function

    '#########################################################################################
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Riempi_Tab_Codici(ByVal progetto_cod As String) As String()

        Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri = New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))


        Dim FiltroLetturaCodici As String = ""
        Dim Id_Cod As Integer
        Dim Val_Cod As String = ""
        Dim objParametriAgenda As New ParametriAgenda
        Dim jsCodici(4) As String

        FiltroLetturaCodici &= " Reg_Impianti_Codici.progetto_cod = " & progetto_cod & " "

        Dim DT_codici As New DataTable
        Dim objCodici As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Codici_R
        DT_codici = objCodici.Leggi(CStr(objParametriAgenda.Piva),
                                           CInt(objParametriAgenda.Sa_Cod),
                                           CInt(objParametriAgenda.Appezza),
                                           CInt(objParametriAgenda.Id_Imp),
                                           "",
                                           CInt(Id_Cod),
                                           CStr(Val_Cod),
                                           AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta,
                                            FiltroLetturaCodici,
                                           "",
                                           objParametri_Server)

        For i = DT_codici.Rows.Count - 1 To 0 Step -1

            Select Case DT_codici.Rows(i).Item("descrizione")

                Case "Capitolato Privato"
                    jsCodici(1) = DT_codici.Rows(i).Item("val_cod")
                    DT_codici.Rows.RemoveAt(i)

                Case "Organismo Referente"
                    jsCodici(2) = DT_codici.Rows(i).Item("val_cod")
                    DT_codici.Rows.RemoveAt(i)

                '  - - - - - - - - - - - - - - - - A N N A      I N I Z I O - - - - - - - - - - - - - - - -
                Case "Licenza Coltivazione"
                    jsCodici(3) = DT_codici.Rows(i).Item("val_cod")
                    DT_codici.Rows.RemoveAt(i)
                '  - - - - - - - - - - - - - - - - A N N A      F I N E - - - - - - - - - - - - - - - - - -

                Case "Magazzino Conferimento"
                    jsCodici(4) = DT_codici.Rows(i).Item("val_cod")
                    DT_codici.Rows.RemoveAt(i)

            End Select


        Next

        HttpContext.Current.Session("Operazione_Distinta") = "2"

        jsCodici(0) = DT_to_Json_Codici(DT_codici)

        Return jsCodici
    End Function

    '#########################################################################################
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Riempi_Tab_Particelle(ByVal progetto_cod As String) As String
        Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri = New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))

        Dim FiltroLetturaCodici As String = ""
        Dim Id_Cod As Integer
        Dim Val_Cod As String = ""
        Dim objParametriAgenda As New ParametriAgenda
        Dim jsParticelle As String

        Dim StringaXML As String
        'variabili per spacchettamento stringa xml
        Dim XmlDoc As New System.Xml.XmlDocument
        Dim XML_DatiProgetto As System.Xml.XmlElement
        Dim XML_Progetto As System.Xml.XmlElement
        'Dim XML_Fase As System.Xml.XmlElement
        'Dim XMLs_Fase As System.Xml.XmlNodeList
        'Dim XML_DatiCodici As System.Xml.XmlElement
        'Dim XML_CodiceImpianto As System.Xml.XmlElement
        'Dim XMLs_CodiceImpianto As System.Xml.XmlNodeList
        Dim XML_DatiParticelle As System.Xml.XmlElement
        Dim XML_Particella As System.Xml.XmlElement
        Dim XMLs_Particelle As System.Xml.XmlNodeList

        ' Create new DataTable instance.
        Dim tbl_Particelle As New DataTable

        tbl_Particelle.Columns.Add("Nome", GetType(String))
        tbl_Particelle.Columns.Add("Val_Cod", GetType(String))


        If progetto_cod <> 0 Then

            Dim objProgetto As New AgronicaCoreAnagrafeBIZ.Progetto_R
            StringaXML = objProgetto.Impresa_Progetti_Leggi(
                                                CStr(objParametriAgenda.Piva),
                                                CInt(progetto_cod),
                                                "",
                                                0,
                                                CInt(objParametriAgenda.Sa_Cod),
                                                CInt(objParametriAgenda.Appezza),
                                                CInt(objParametriAgenda.Id_Imp),
                                                0, 0,
                                                False,
                                                objParametri_Server)

            XmlDoc.LoadXml(StringaXML)

            '----- Tag DatiProgetto

            XML_DatiProgetto = XmlDoc.SelectSingleNode("DatiProgetto")

            '----- Tag Progetto

            XML_Progetto = XML_DatiProgetto.SelectSingleNode("Progetto")

            '--------------------------------------
            '--- Particelle x Progetto
            '--------------------------------------

            Dim StrParticella As String = ""
            Dim StrParticelle As String = ""
            Dim Prov As String
            Dim Com As String
            Dim Sezione As String
            Dim Foglio As Integer
            Dim Numero As Integer
            Dim Subalterno As String
            'Dim StrCodProvincia As String
            'Dim StrCodComune As String
            'Dim StrSezione As String
            'Dim StrFoglio As String
            'Dim StrNumero As String
            'Dim StrSubalterno As String
            'Dim Testo As String
            'Dim Valore As String

            Dim BaseCode As Integer
            Dim TopCode As Integer

            Calcola_BaseCode_TopCode(BaseCode, TopCode,
                                     HttpContext.Current.Session("ASG_ProgressivoGIAS"))

            XML_DatiParticelle = XML_Progetto.SelectSingleNode("DatiParticellexProgetto")



            If XML_DatiParticelle IsNot Nothing Then

                If XML_DatiParticelle.HasChildNodes Then

                    '----- Tag Particella  (multiplo)

                    'Recupero la collezione dei nodi
                    XMLs_Particelle = XML_DatiParticelle.GetElementsByTagName("ParticellaxProgetto")

                    If XMLs_Particelle.Count > 0 Then

                        StrParticelle = ""




                        For i = 0 To XMLs_Particelle.Count - 1

                            XML_Particella = XMLs_Particelle.Item(i)

                            Prov = XML_Particella.GetAttribute("prov")
                            Com = XML_Particella.GetAttribute("com")
                            Sezione = XML_Particella.GetAttribute("sezione")
                            Foglio = XML_Particella.GetAttribute("foglio")
                            Numero = XML_Particella.GetAttribute("numero")
                            Subalterno = XML_Particella.GetAttribute("subalterno")

                            Id_Cod = CInt(XML_Particella.GetAttribute("id_cod"))
                            Val_Cod = XML_Particella.GetAttribute("val_cod")



                            'Genero l'XML del singolo nodo
                            Call XML_ParticellaxProgetto(enum_CodificaDecodifica.Codifica,
                                StrParticella,
                                enum_TipoOperazioneDB.Cancellazione,
                                Prov,
                                Com,
                                Sezione,
                                Foglio,
                                Numero,
                                Subalterno,
                                Id_Cod,
                                Val_Cod,
                                CDate(XML_Particella.GetAttribute("validita_inizio")),
                                CDate(XML_Particella.GetAttribute("validita_fine")),
                                BaseCode,
                                TopCode)

                            tbl_Particelle.Rows.Add(StrParticella, Val_Cod)

                            ''Inserisco l'XML nella stringa complessiva
                            'StrParticelle = StrParticelle & StrParticella

                            'Select Case Id_Cod

                            '    Case enum_CodiciAnagrafe.CodiceRigaRiferimentoQuadroP

                            '        StrCodProvincia = Left("_" & Prov & "_", 10)
                            '        StrCodComune = Left("_" & Com & "_", 10)

                            '        If Sezione = "0" Then
                            '            StrSezione = Left("_", 6)
                            '        Else
                            '            StrSezione = Left("_" & Sezione & "_", 6)
                            '        End If

                            '        StrFoglio = Left("_" & Foglio & "_", 7)
                            '        StrNumero = Left("_" & Numero & "_", 7)

                            '        If Subalterno = "0" Then
                            '            StrSubalterno = Left("_", 7)
                            '        Else
                            '            StrSubalterno = Left("_" & Subalterno & "_", 7)
                            '        End If

                            '        Testo = StrCodProvincia & " : " & _
                            '          StrCodComune & " : " & _
                            '          StrSezione & " : " & _
                            '          StrFoglio & " : " & _
                            '          StrNumero & " / " & _
                            '          StrSubalterno

                            '        Valore = "" & Prov & _
                            '          "£" & Com & _
                            '          "£" & Sezione & _
                            '          "£" & Foglio & _
                            '          "£" & Numero & _
                            '          "£" & Subalterno

                            '        'Me.ListParticelle.Items.Add(New ListItem(Testo & " = " & Val_Cod, _
                            '        '     Valore))

                            'End Select

                        Next

                    End If

                End If

            End If

        End If

        HttpContext.Current.Session("DT_Particelle") = tbl_Particelle

        jsParticelle = DT_to_Json_Particelle(tbl_Particelle)

        Return jsParticelle
    End Function


    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Riempi_Info_Impianto(ByVal progetto_cod As String) As String()

        Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri = New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))

        Dim objParametriAgenda As New ParametriAgenda
        Dim Dt_Progetti As DataTable
        Dim jsInfo(10) As String

        'Creo l'oggetto COM+
        Dim objImpresa_Progetti = New AgronicaCoreAnagrafeDAL.Impresa_Progetti_R

        'Mi procuro il recordset richiesto
        Dt_Progetti = objImpresa_Progetti.Leggi(
                                        CStr(objParametriAgenda.Piva),
                                        CInt(progetto_cod),
                                        "",
                                        0,
                                        CInt(objParametriAgenda.Sa_Cod),
                                        CInt(objParametriAgenda.Appezza),
                                        CInt(objParametriAgenda.Id_Imp),
                                        0,
                                        0,
                                        AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                        "",
                                        "",
                                        objParametri_Server)

        If Not IsNothing(Dt_Progetti) Then
            ' N° piante/impianto
            jsInfo(0) = Dt_Progetti.Rows(0).Item("p_ha")
            ' Data Semina prevista ?
            jsInfo(1) = Dt_Progetti.Rows(0).Item("data_inizio_prevista")
            ' Data Fioritura prevista
            jsInfo(2) = Dt_Progetti.Rows(0).Item("data_fioritura_prevista")
            ' Data Raccolta prevista ?
            jsInfo(3) = Dt_Progetti.Rows(0).Item("data_fine_prevista")
            ' Resa [Kg/Ha] ?
            jsInfo(4) = Dt_Progetti.Rows(0).Item("giudizio")
            ' Resa prevista
            jsInfo(5) = Dt_Progetti.Rows(0).Item("produzione_prevista")
            ' Disciplinare
            jsInfo(6) = Dt_Progetti.Rows(0).Item("disciplinare_cod")
            ' Regolamento
            jsInfo(7) = Dt_Progetti.Rows(0).Item("regolamento_cod")
            ' Regolamento concimazioni
            jsInfo(8) = Dt_Progetti.Rows(0).Item("regolamento_concimazioni_cod")
            ' Stato Impianto
            jsInfo(9) = Dt_Progetti.Rows(0).Item("stato_impianto")
            ' Nome Progetto
            jsInfo(10) = Dt_Progetti.Rows(0).Item("Progetto_Nome")

        End If


        ' Salvo nella Session la Distinta CORRENTE
        HttpContext.Current.Session("Distinta_corrente") = Dt_Progetti

        Return jsInfo
    End Function

    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Svuota_Distinta()

        HttpContext.Current.Session("Distinta_corrente") = Nothing
        HttpContext.Current.Session("Operazione_Distinta") = "1"

    End Function


    '#########################################################################################
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Carica_Select_StatoImpianto(ByVal specie As String, ByVal finalita As String, ByVal regolamento As String) As String
        Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri = New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))
        Lingua.Gias_InizializzaCultura_DaSession()

        Dim cmb_finalita As New DropDownList

        CaricaListControl.FasiClicloColturale(cmb_finalita,
                                     True, AgronicaAgenda_2010.ImpiantoInProduzione, "102",
                                     specie,
                                     finalita,
                                     Math.Abs(CInt(regolamento)),
                                     "",
                                     "", objParametri_Server)

        Dim rval As String = ""
        For Each itm As ListItem In cmb_finalita.Items
            rval &= "<option value=""" & itm.Value & """>" & itm.Text & "</option>"
        Next


        Return rval

    End Function

    '#########################################################################################
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function get_gru_cod(veg_cod As Integer) As RispostaStandard
        Dim r As New RispostaStandard
        Try
            Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri = New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))
            Dim objSpecie As New AgronicaCoreMetaSchemaDAL.SpecieVegetali_R
            Dim dt = objSpecie.Leggi(veg_cod, 0, "", "", 1, "", "", objParametri_Server)

            Dim str_risposta = 0
            If dt.Rows.Count > 0 Then
                str_risposta = dt.Rows(0).Item("Gru_Cod")
            End If

            r.RispostaOK = True
            r.RispostaStringa = str_risposta
        Catch ex As Exception

            r.RispostaOK = False
            r.RispostaStringa = ex.Message

        End Try

        Return r

    End Function

    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function GeneraProgressivo(ByVal piva As String, ByVal data As String) As RispostaStandard

        Dim r As New RispostaStandard
        Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri = New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))
        Dim objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri = New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Utenti"))

        Lingua.Gias_InizializzaCultura_DaSession()

        Try
            Dim anno = CDate(data).Year
            r.RispostaOK = True
            r.RispostaStringa = Replica_GIAS.LeggiCodiceProgressivo(piva, enum_SequenzaProgressiviTipi.CodiciOPAgriZoo, anno, objParametri_Server)

        Catch ex As Exception

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.RispostaOK = False
        Finally

        End Try

        Return r

    End Function

    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function LeggiDescrizioni(ByVal piva As String) As RispostaStandard

        Dim r As New RispostaStandard
        Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri = New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))
        Dim objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri = New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Utenti"))

        Lingua.Gias_InizializzaCultura_DaSession()

        Try

            Dim dt As DataTable
            Dim objCampi As New AgronicaCoreAnagrafeDAL.Materie_Prime_R
            dt = objCampi.MateriePrime_DescrizioniOP(piva, 0, "201,204,210", "", "", "", objParametri_Server)

            Dim jsonCampi As New JArray
            Dim i As Integer

            Dim jObj As New JObject
            For i = 0 To dt.Rows.Count - 1
                jObj = New JObject
                jObj.Add(New JProperty("des", dt.Rows(i).Item("cod_articolo") & " - " & dt.Rows(i).Item("mat_des")))
                jObj.Add(New JProperty("val", dt.Rows(i).Item("cod_articolo")))
                jsonCampi.Add(jObj)
            Next

            r.RispostaOK = True
            r.RispostaStringa = jsonCampi.ToString

        Catch ex As Exception

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.RispostaOK = False
        Finally

        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function LeggiDisciplinarePrivato() As RispostaStandard

        Dim r As New RispostaStandard

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

        Lingua.Gias_InizializzaCultura_DaSession()

        Try

            Dim objConfSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R

            Dim leggiPrivato As String = "false"
            Dim ret = objConfSiti.Leggi_Valore(0, "Flag_DisciplinarePrivato", "", "", objParametri_Server)

            If ret <> "" Then
                r.RispostaStringa = ret.ToLower
            Else
                r.RispostaStringa = leggiPrivato
            End If
            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function

    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function controlla_CdG(ByVal obj_distinta As String, ByVal obj_Impianto As String) As RispostaStandard

        Dim r As New RispostaStandard

        Try

            Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri = New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))
            Dim objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri = New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Utenti"))

            Lingua.Gias_InizializzaCultura_DaSession()

            Dim objImpW As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Write
            Dim objImpR As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
            Dim objRisposta = New JObject()

            Dim distinta = Newtonsoft.Json.JsonConvert.DeserializeObject(obj_distinta)
            Dim impianto = Newtonsoft.Json.JsonConvert.DeserializeObject(obj_Impianto)
            Dim piva As String = impianto.GetValue("piva").ToString()
            Dim sa_cod As Integer = CInt(impianto.GetValue("sa_cod"))
            Dim appezza As Integer = CInt(impianto.GetValue("appezza"))
            Dim id_reg As Integer = CInt(impianto.GetValue("id_reg"))
            Dim progetto_cod As Integer = CInt(distinta.GetValue("Progetto_Cod"))

            Dim validita_inizio = AGRODATAINIZIO
            If IsDate(distinta.GetValue("Validita_Inizio").ToString) Then
                validita_inizio = CDate(distinta.GetValue("Validita_Inizio"))
            End If

            Dim validita_fine = AGRODATAFINE
            If IsDate(distinta.GetValue("Validita_Fine").ToString) Then
                validita_fine = CDate(distinta.GetValue("Validita_Fine"))
            End If

            Dim descrizione_esercizio = distinta.GetValue("Progetto_Nome").ToString()
            If descrizione_esercizio = "" Then
                descrizione_esercizio = "con validità dal " & validita_inizio & " al " & validita_fine
            End If

            Dim objControllo As New AgronicaCoreAnagrafeBIZ.Progetto_W
            Dim controllo = objControllo.controllo_CdGxEliminazione(Nothing, piva, sa_cod, appezza, id_reg, progetto_cod, objParametri_Server)

            If controllo.errore Then
                Dim MessaggioErrore As String = ""
                If Not controllo.messaggioSpecifico Then
                    MessaggioErrore = ("Non è possibile eliminare la distinta " & descrizione_esercizio & ", perché ci sono costi di gestione associati")
                Else
                    MessaggioErrore = ("Non è possibile eliminare la distinta " & descrizione_esercizio & ", perché ci sono costi di gestione associati in data " & controllo.dataCdG)
                End If

                objRisposta.Add(New JProperty("Prosegui", "false"))
                objRisposta.Add(New JProperty("TipoMessaggioRitorno", "Alert"))
                objRisposta.Add(New JProperty("TipoControllo", "0"))
                objRisposta.Add(New JProperty("Messaggio", MessaggioErrore))

                r.RispostaStringa = objRisposta.ToString
                r.RispostaOK = True
                Return r
            End If

            objRisposta.Add(New JProperty("Prosegui", "true"))
            objRisposta.Add(New JProperty("TipoMessaggioRitorno", "Prosegui"))
            objRisposta.Add(New JProperty("TipoControllo", "0"))
            objRisposta.Add(New JProperty("Messaggio", DirectCast(System.Web.HttpContext.GetLocalResourceObject("~/Anagrafica/Impianto_Edit2.aspx", "ProseguiSalvataggio"), String)))

            r.RispostaStringa = objRisposta.ToString
            r.RispostaOK = True

        Catch ex As Exception

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.RispostaOK = False
        End Try

        Return r

    End Function

    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Controlla_DatixCancellazioneDistinta(ByVal obj_Impianto As String,
                                                                ByVal obj_distinta As String
                                                                ) As RispostaStandard

        Dim r As New RispostaStandard

        Try

            Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri = New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))
            Dim objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri = New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Utenti"))

            Lingua.Gias_InizializzaCultura_DaSession()

            Dim objRisposta = New JObject()

            Dim impianto = Newtonsoft.Json.JsonConvert.DeserializeObject(obj_Impianto)
            Dim distinta = Newtonsoft.Json.JsonConvert.DeserializeObject(obj_distinta)
            Dim piva As String = impianto.GetValue("piva").ToString()
            Dim sa_cod As Integer = CInt(impianto.GetValue("sa_cod"))
            Dim appezza As Integer = CInt(impianto.GetValue("appezza"))
            Dim id_reg As Integer = CInt(impianto.GetValue("id_reg"))

            Dim progetto_cod As Integer = CInt(distinta.GetValue("Progetto_Cod"))

            Dim validita_inizio = AGRODATAINIZIO
            If IsDate(distinta.GetValue("Validita_Inizio").ToString) Then
                validita_inizio = CDate(distinta.GetValue("Validita_Inizio"))
            End If
            Dim validita_fine = AGRODATAFINE
            If IsDate(distinta.GetValue("Validita_Fine").ToString) Then
                validita_fine = CDate(distinta.GetValue("Validita_Fine"))
            End If

            Dim des As String = distinta.GetValue("Progetto_Nome").ToString
            If des = "" Then
                des = "con validità dal " & validita_inizio & " al " & validita_fine
            End If


            'CONTROLLO CdG
            Dim objControllo As New AgronicaCoreAnagrafeBIZ.Progetto_W
            Dim controllo = objControllo.controllo_CdGxEliminazione(Nothing, piva, sa_cod, appezza, id_reg, progetto_cod, objParametri_Server)
            If controllo.errore Then
                Dim MessaggioErrore As String = ""
                If Not controllo.messaggioSpecifico Then
                    MessaggioErrore = ("Non è possibile eliminare l'esercizio  " & des & ", perché esistono costi di gestione associati")
                Else
                    MessaggioErrore = ("Non è possibile eliminare l'esercizio " & des & ", perché esistono costi di gestione associati in data " & controllo.dataCdG)
                End If

                objRisposta.Add(New JProperty("ProseguiCancellazioneDistinta", "false"))
                objRisposta.Add(New JProperty("TipoMessaggioRitorno", "alert"))
                objRisposta.Add(New JProperty("TipoControllo", "0"))
                objRisposta.Add(New JProperty("Messaggio", MessaggioErrore))
                r.RispostaStringa = objRisposta.ToString
                r.RispostaOK = True

                Return r

            End If



            'TUTTO OK NO MESSAGGI
            objRisposta.Add(New JProperty("ProseguiCancellazioneDistinta", "true"))
            objRisposta.Add(New JProperty("TipoMessaggioRitorno", "procedi"))
            objRisposta.Add(New JProperty("TipoControllo", "0"))
            objRisposta.Add(New JProperty("Messaggio", DirectCast(System.Web.HttpContext.GetLocalResourceObject("~/Anagrafica/Impianto_Edit2.aspx", "ProseguiCancellazioneDistinta"), String)))
            r.RispostaStringa = objRisposta.ToString
            r.RispostaOK = True

        Catch ex As Exception

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.RispostaOK = False
        End Try

        Return r

    End Function

    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Controlla_DatixModificaDistinta(ByVal obj_Impianto As String,
                                                           ByVal obj_distinta As String
                                                           ) As RispostaStandard

        Dim r As New RispostaStandard

        Try

            Dim objParametri_Server As AgronicaCoreParametri = New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))
            Dim objParametri_Utenti As AgronicaCoreParametri = New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Utenti"))

            Lingua.Gias_InizializzaCultura_DaSession()

            Dim objRisposta = New JObject()

            Dim impianto = JsonConvert.DeserializeObject(obj_Impianto)
            Dim distinta = JsonConvert.DeserializeObject(obj_distinta)
            Dim piva As String = impianto.GetValue("piva").ToString()
            Dim sa_cod As Integer = CInt(impianto.GetValue("sa_cod"))
            Dim appezza As Integer = CInt(impianto.GetValue("appezza"))
            Dim id_reg As Integer = CInt(impianto.GetValue("id_reg"))

            Dim progetto_cod As Integer = CInt(distinta.GetValue("Progetto_Cod"))

            Dim validita_inizio = AGRODATAINIZIO
            If IsDate(distinta.GetValue("Validita_Inizio").ToString) Then
                validita_inizio = CDate(distinta.GetValue("Validita_Inizio"))
            End If
            Dim validita_fine = AGRODATAFINE
            If IsDate(distinta.GetValue("Validita_Fine").ToString) Then
                validita_fine = CDate(distinta.GetValue("Validita_Fine"))
            End If

            Dim des As String = distinta.GetValue("Progetto_Nome").ToString
            If des = "" Then
                des = "[dal " & validita_inizio & " al " & validita_fine & "]"
            End If

            'COSTI DI GESTIONE
            Dim objControllo As New AgronicaCoreAnagrafeBIZ.Progetto_W
            Dim controllo = objControllo.controllo_CdG(Nothing, piva, sa_cod, appezza, id_reg, progetto_cod, validita_inizio, validita_fine, objParametri_Server)
            If controllo.errore Then
                Dim MessaggioErroreCdG As String = ""

                If Not controllo.messaggioSpecifico Then
                    MessaggioErroreCdG &= ("Non è possibile modificare la " & controllo.inizio_fine & " dell'esercizio " & des & ", perché sono stati associati Costi di Gestione in data successiva a quella selezionata")
                Else
                    MessaggioErroreCdG &= ("Non è possibile modificare la " & controllo.inizio_fine & " dell'esercizio " & des & ", perché sono stati associati Costi di Gestione in data " & controllo.dataCdG)
                End If

                objRisposta.Add(New JProperty("ProseguiCancellazioneDistinta", "false"))
                objRisposta.Add(New JProperty("TipoMessaggioRitorno", "alert"))
                objRisposta.Add(New JProperty("TipoControllo", "0"))
                objRisposta.Add(New JProperty("Messaggio", MessaggioErroreCdG))
                r.RispostaStringa = objRisposta.ToString
                r.RispostaOK = True

                Return r
            End If

            'TUTTO OK NO MESSAGGI
            objRisposta.Add(New JProperty("Prosegui", "true"))
            objRisposta.Add(New JProperty("TipoMessaggioRitorno", "procedi"))
            objRisposta.Add(New JProperty("TipoControllo", "0"))
            objRisposta.Add(New JProperty("Messaggio", ""))
            r.RispostaStringa = objRisposta.ToString
            r.RispostaOK = True

        Catch ex As Exception
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.RispostaOK = False
        End Try

        Return r
    End Function
#End Region

End Class
