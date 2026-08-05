Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreModello.ParametriAgenda_Temp
Imports System.Web.Services
Imports System.Data
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreAnagrafeDAL
Imports AgronicaCoreContabDAL
Imports AgroAgenda_2010.Resources
Imports AgronicaCoreUtentiDAL
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports AgronicaCoreUtility
Imports AgronicaCoreMetaSchemaDAL
Imports System.Linq
Imports System.Runtime.Serialization
Imports AgronicaCoreEntityFramework



Public Class Modifica_Multipla_Zoo
    Inherits System.Web.UI.Page

    Dim Zoo_Animali As New Zoo_Animali
    Dim Leggi_Zoo_Animali_Distinte As New Zoo_Animali_Distinte
    Dim objStalla_R As New Stalla_R
    Dim objParametriAgenda As New ParametriAgenda
    Dim objParametri_Server, objParametri_Utenti As New AgronicaCoreParametri
    Public objparametri_server_string, objparametri_utenti_string As String

    Dim Qs_Visibilita As Integer = 0

    Protected Overrides Sub InitializeCulture()
        MyBase.InitializeCulture()
        Lingua.Gias_InizializzaCultura_DaSession()

    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        inizializzoObjParametri()

        Dim Leggi_impostazioni As New Utenti_Impostazioni_Read
        Dim DTfinale As DataTable
        Dim localBool = True
        Dim modificaMultipla As String
        Dim imp = New AgronicaCoreModello.ParametriAgenda_Temp.Impianto
        Dim presetColumnsToUpdate As String = ""

        'Controllo permessi utente.
        Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
        Dim UtenteAbilitatoLettura As Boolean = objPermessi.Controlla_Permessi_Utente(
                                            Session("ASG_Utente_Username"),
                                            Session("ASG_IdServizio"),
                                            enum_Security_Attivita.Anagrafica_MultiModifica_PianoColturale,
                                            enum_Security_Operazione.Lettura,
                                            Date.Now,
                                            "",
                                            objParametri_Utenti)

        Dim UtenteAbilitatoScrittura As Boolean = objPermessi.Controlla_Permessi_Utente(
                                           Session("ASG_Utente_Username"),
                                           Session("ASG_IdServizio"),
                                           enum_Security_Attivita.Anagrafica_MultiModifica_PianoColturale,
                                           enum_Security_Operazione.Modifica,
                                           Date.Now,
                                           "",
                                           objParametri_Utenti)

        If UtenteAbilitatoLettura = False Then
            Response.Redirect("~/classi/Agro_Pages/AccessoNonConsentito.aspx")
        End If

        Dim PivaSuperUser As String = objParametri_Server.PivaSuperUser

        If objParametri_Server.SuperUserUsername = objParametri_Server.UtenteUsername Then
            modificaMultipla = "2"
        Else
            modificaMultipla = Leggi_impostazioni.ImpostazioneValore1_from_ImpostazioneCod(enum_Impostazioni_Utenti.SuperUser_Ereditatore, objParametri_Utenti, 2)
        End If

        If Request.QueryString("visibilita") IsNot Nothing AndAlso IsNumeric(Request.QueryString("visibilita")) Then
            Qs_Visibilita = CInt(Request.QueryString("visibilita"))
        End If

        hidden_modificaMultipla.Value = modificaMultipla

        'se è abilitata la modifica multipla, carichiamo il preset delle voci da modificare
        If modificaMultipla >= 1 Then
            presetColumnsToUpdate = Leggi_impostazioni.ImpostazioneValore1_from_ImpostazioneCod(enum_Impostazioni_Utenti.ModificaMultiplaZoo, objParametri_Utenti)
        End If

        hidden_presetColumnsToUpdate.Value = presetColumnsToUpdate

        'For Each i In objParametriAgenda.Impianti
        '    Dim DTzoo = Zoo_Animali.Leggi_Giacenze("", 0, 0, 0, i.Progetto_Cod, Date.Now, objParametri_Server)

        '    Dim Row As DataRow = DTzoo.Select("").FirstOrDefault()

        '    Dim Lotto = Leggi_Zoo_Animali_Distinte.LeggiDaCod_Animale(Row.Item("Cod_Animale"), objParametri_Server)

        '    Row("Lotto") = Lotto

        '    If localBool = True Then
        '        DTfinale = DTzoo.Clone()
        '        localBool = False
        '    End If


        '    DTfinale.LoadDataRow(Row.ItemArray, False)
        'Next

        'hdKendoModifica_Multipla_Zoo.Value = LeggiKendoModifica_Zoo(DTfinale)
    End Sub
    Private Sub Modifica_Multipla_Zoo_Init(sender As Object, e As EventArgs) Handles Me.Init


        ' VAnni: 2/10/2017: a seguito di AgroMaseterPage
        'AddHandler CType(Page.Master.FindControl("ImgBtn_AnnullaTutto"), ImageButton).Click, AddressOf Me.AnnullaTutto
        AddHandler CType(Me.Master, AgendaBootstrap).ImgBtn_AnnullaTutto.Click, AddressOf Me.AnnullaTutto
        CType(Me.Master, AgendaBootstrap).flag_MostraBtnIndietro = True

    End Sub
    'Private Sub Modifica_Multipla_Zoo_Init(sender As Object, e As System.EventArgs) Handles Me.Init

    '    Dim xModalBS As String = Request.QueryString("modalBS")
    '    If Not String.IsNullOrEmpty(xModalBS) AndAlso xModalBS = "1" Then
    '        Me.Master.flag_pag_BootstrapModal = True
    '        Me.Master.flag_MostraHeader = False
    '        Me.Master.flag_MostraFooter = False
    '    Else

    '        ' VAnni: 2/10/2017: a seguito di AgroMaseterPage
    '        'AddHandler CType(Page.Master.FindControl("ImgBtn_AnnullaTutto"), ImageButton).Click, AddressOf Me.AnnullaTutto
    '        AddHandler CType(Me.Master, AgendaBootstrap).ImgBtn_AnnullaTutto.Click, AddressOf Me.AnnullaTutto
    '        Master.flag_MostraBtnIndietro = True
    '    End If

    'End Sub

    Private Sub AnnullaTutto(ByVal sender As Object, ByVal e As System.EventArgs)


        Dim TargetUrl As String
        TargetUrl = CType(Me.Master, AgendaBootstrap).TrovaRedirectCorretto(False, enum_PagineGiasOnline.MenuAnagrafica, objParametriAgenda, , Qs_Visibilita)

        Response.Redirect(TargetUrl)

    End Sub

    Private Sub inizializzoObjParametri()
        objParametri_Server = New AgronicaCoreParametri(Session("ASG_objParametri_Server"))
        objParametri_Utenti = New AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))
        objparametri_server_string = Utility.convertOBJparametritoString(objParametri_Server)
    End Sub

    <WebMethod(EnableSession:=True)>
    Public Shared Function leggiGiacenze() As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametri_Server As New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Utenti) Then
            r.Sessione = False
            Return r
        End If

        Try
            Lingua.Gias_InizializzaCultura_DaSession()

            Dim objParametriAgenda As New ParametriAgenda
            Dim filtroData As Date = objParametriAgenda.Data

            Dim listCod_Animali As New List(Of Integer)
            For Each i In objParametriAgenda.Impianti
                listCod_Animali.Add(i.Progetto_Cod)
            Next
            Dim Zoo_Animali As New Zoo_Animali
            Dim DTfinale = Zoo_Animali.Leggi_Giacenze(objParametriAgenda.Piva, 0, 0, 0, 0,
                                                      filtroData, objParametri_Server, , ,
                                                      listCod_Animali, filtraFornitori:=True)
            Dim strRisposta = LeggiKendoModifica_Zoo(DTfinale)


            r.RispostaStringa = strRisposta
            r.RispostaOK = True

        Catch ex As Exception

            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.RispostaOK = False
        End Try

        Return r

    End Function

    Public Shared Function LeggiKendoModifica_Zoo(ByVal DT As DataTable) As String

        'Creazione lista colonne da visualizzare.
        Dim L As New List(Of ColonneNome)
        L.Add(New ColonneNome("chiave", "seleziona", "string") With {._hidden = True})

        L.Add(New ColonneNome("PIVA", AgronicaAgenda_2010.PartitaIVA, "string") With {._width = "145px"})
        L.Add(New ColonneNome("partitaIvaReale", AgronicaAgenda_2010.PartitaIVA, "string") With {._width = "145px"})
        L.Add(New ColonneNome("Matricola", AgronicaAgenda_2010.Matricola, "string") With {._width = "145px"})
        L.Add(New ColonneNome("Sesso", AgronicaAgenda_2010.Sesso, "string") With {._width = "145px"})
        L.Add(New ColonneNome("Validato", "Validato", "string") With {._width = "145px"})
        L.Add(New ColonneNome("dat_nascita", AgronicaAgenda_2010.DataNascita, "date") With {._width = "145px"})
        L.Add(New ColonneNome("SPE_DES", AgronicaAgenda_2010.Specie, "string") With {._width = "145px"})
        L.Add(New ColonneNome("RAZ_DES", AgronicaAgenda_2010.Razza, "string") With {._width = "145px"})
        L.Add(New ColonneNome("sa_nome", AgronicaAgenda_2010.Centro, "string") With {._width = "145px"})
        L.Add(New ColonneNome("FlagBDN", "BDN", "string") With {._width = "145px"})
        L.Add(New ColonneNome("bdn_codice_azienda", AgronicaAgenda_2010.Stalla, "string") With {._width = "145px"})
        L.Add(New ColonneNome("Metodo_Produzione", AgronicaAgenda_2010.MetodoDiProduzione, "string") With {._width = "145px"})
        L.Add(New ColonneNome("Modello4_Ingresso_Numero", "N. Modello 4 Ingresso", "string") With {._width = "145px"})
        'L.Add(New ColonneNome("Modello4_Ingresso", "Modello 4 Entrata", "string") With {._width = "145px"})
        'L.Add(New ColonneNome("Modello4_Uscita", "Modello 4 Uscita", "string") With {._width = "145px"})
        L.Add(New ColonneNome("Modello4_Uscita_Numero", "N. Modello 4 Uscita", "string") With {._width = "145px"})
        L.Add(New ColonneNome("MAT_PADRE", "Matricola Padre", "string") With {._width = "145px"})
        L.Add(New ColonneNome("RazDes_Padre", "Razza Padre", "string") With {._width = "145px"})
        L.Add(New ColonneNome("MAT_MADRE", "Matricola Madre", "string") With {._width = "145px"})
        L.Add(New ColonneNome("RazDes_Madre", "Razza Madre", "string") With {._width = "145px"})
        L.Add(New ColonneNome("Id_Capo_BDN", "ID Capo BDN", "string") With {._width = "145px"})
        L.Add(New ColonneNome("CF_PROPRIETARIO", "CF Proprietario", "string") With {._width = "145px"})
        L.Add(New ColonneNome("CF_DETENTORE", "CF Detentore", "string") With {._width = "145px"})
        L.Add(New ColonneNome("RagSoc_FornFatt", "Fornitore Fatturazione", "string") With {._width = "145px"})
        L.Add(New ColonneNome("CF_FornFatt", "CF Fornitore Fatturazione", "string") With {._hidden = True})
        L.Add(New ColonneNome("RagSoc_FornProv", "Fornitore Provenienza", "string") With {._width = "145px"})
        L.Add(New ColonneNome("CF_FornProv", "CF Fornitore Provenienza", "string") With {._hidden = True})
        L.Add(New ColonneNome("AUSL_AZI_NASCITA", "Azienda Nascita", "string") With {._width = "145px"})
        L.Add(New ColonneNome("RagSoc_StallaSvezz", "Stalla Svezzamento", "string") With {._width = "145px"})
        L.Add(New ColonneNome("CF_StallaSvezz", "CF Stalla Svezzamento", "string") With {._hidden = True})
        L.Add(New ColonneNome("Certificato", AgronicaAgenda_2010.Certificazione, "string") With {._width = "145px"})
        L.Add(New ColonneNome("Validita_Inizio", AgronicaAgenda_2010.DataInizio, "date") With {
              ._width = "145px",
              ._TemplateHtmlID = "#= ((kendo.toString(Validita_Inizio, 'dd/MM/yyyy' ) == null) || Validita_Inizio <= AGRODATAINIZIO ) ? '' : kendo.toString(Validita_Inizio, 'dd/MM/yyyy' ) #"})
        L.Add(New ColonneNome("Validita_Fine", AgronicaAgenda_2010.DataFine, "date") With {
              ._width = "145px",
              ._TemplateHtmlID = "#= ((kendo.toString(Validita_Fine, 'dd/MM/yyyy' ) == null) || Validita_Fine >= AGRODATAFINE ) ? '' : kendo.toString(Validita_Fine, 'dd/MM/yyyy' ) #"
              })
        L.Add(New ColonneNome("Data_Modifica", AgronicaAgenda_2010.DataModifica, "date") With {._width = "145px"})
        L.Add(New ColonneNome("Utente_Modifica", AgronicaAgenda_2010.UtenteModifica, "string") With {._width = "145px"})
        L.Add(New ColonneNome("Data_Creazione", AgronicaAgenda_2010.DataCreazione, "date") With {._width = "145px"})
        L.Add(New ColonneNome("Utente_Creazione", AgronicaAgenda_2010.UtenteCreazione, "string") With {._width = "145px"})
        L.Add(New ColonneNome("Lotto_Fornitore", "Lotto Fornitore", "string") With {._width = "145px"})
        L.Add(New ColonneNome("Codice_Distinta", "Lotto", "string") With {._width = "145px"})
        'L.Add(New ColonneNome("Modello4_Ingresso_Numero", "Modello 4 Entrata Numero", "string") With {._width = "145px"})
        'L.Add(New ColonneNome("Modello4_Uscita_Numero", "Modello 4 Uscita Numero", "string") With {._width = "145px"})
        L.Add(New ColonneNome("Modello4_Ingresso_Prenotazione", "Codice Modello 4 Ingresso", "string") With {._width = "145px"})
        L.Add(New ColonneNome("Modello4_Uscita_Prenotazione", "Codice Modello 4 Uscita", "string") With {._width = "145px"})
        L.Add(New ColonneNome("Data_Documento_Ingresso", "Data Documento Entrata", "date") With {
              ._width = "145px",
              ._TemplateHtmlID = "#= ((kendo.toString(Data_Documento_Ingresso, 'dd/MM/yyyy' ) == null) || Data_Documento_Ingresso <= AGRODATAINIZIO ) ? '' : kendo.toString(Data_Documento_Ingresso, 'dd/MM/yyyy' ) #"})
        L.Add(New ColonneNome("Data_Documento_Uscita", "Data Documento Uscita", "date") With {
              ._width = "145px",
              ._TemplateHtmlID = "#= ((kendo.toString(Data_Documento_Uscita, 'dd/MM/yyyy' ) == null) || Data_Documento_Uscita >= AGRODATAFINE ) ? '' : kendo.toString(Data_Documento_Uscita, 'dd/MM/yyyy' ) #"})

        L.Add(New ColonneNome("N_Bolla_Fornitore", "Numero DDT Ingresso", "string") With {._width = "145px"})
        L.Add(New ColonneNome("Data_DDT_Ingresso", "Data DDT Ingresso", "date") With {
              ._width = "145px",
              ._TemplateHtmlID = "#= ((kendo.toString(Data_DDT_Ingresso, 'dd/MM/yyyy' ) == null) || Data_DDT_Ingresso <= AGRODATAINIZIO ) ? '' : kendo.toString(Data_DDT_Ingresso, 'dd/MM/yyyy' ) #"})
        L.Add(New ColonneNome("N_Bolla_Uscita", "Numero DDT Uscita", "string") With {._width = "145px"})
        L.Add(New ColonneNome("Data_DDT_Uscita", "Data DDT Uscita", "date") With {
              ._width = "145px",
              ._TemplateHtmlID = "#= ((kendo.toString(Data_DDT_Uscita, 'dd/MM/yyyy' ) == null) || Data_DDT_Uscita >= AGRODATAFINE ) ? '' : kendo.toString(Data_DDT_Uscita, 'dd/MM/yyyy' ) #"})
        L.Add(New ColonneNome("Incremento_Teorico", "Incremento Teorico", "number") With {._width = "145px"})

        L.Add(New ColonneNome("Codice_Azienda_Fornitore", "Codice Azienda Fornitore", "string") With {._width = "145px"})
        L.Add(New ColonneNome("Impresa", "Impresa", "string") With {._width = "145px"})
        L.Add(New ColonneNome("Cod_Contatto", "Cod_Contatto", "string") With {._hidden = True})
        L.Add(New ColonneNome("IPRO_COD", "IPRO_COD", "number") With {._hidden = True})
        L.Add(New ColonneNome("RAZ_COD", "RAZ_COD", "number") With {._hidden = True})
        L.Add(New ColonneNome("SPE_COD", "SPE_COD", "number") With {._hidden = True})
        L.Add(New ColonneNome("GEN_COD", "GEN_COD", "number") With {._hidden = True})
        L.Add(New ColonneNome("Sa_Cod", "Sa_Cod", "number") With {._hidden = True})
        L.Add(New ColonneNome("Cod_Progetto", "Cod_Progetto", "number") With {._hidden = True})
        L.Add(New ColonneNome("Cod_Animale", "Cod_Animale", "number") With {._hidden = True})
        L.Add(New ColonneNome("RazCod_Padre", "RazCod_Padre", "number") With {._hidden = True})
        L.Add(New ColonneNome("RazCod_Madre", "RazCod_Madre", "number") With {._hidden = True})


        Dim js As New JSON_DataTable
        Dim risp As String = js.JSON_DataTable_Kendo(DT, L)
        Return risp

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function getRazze(ByVal SPE_COD, ByVal GEN_COD) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objP_Server As New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objP_Server) Then
            r.Sessione = False
            Return r
        End If

        Dim objP_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objP_Utenti) Then
            r.Sessione = False
            Return r
        End If

        Try
            Dim listaRazzeAnimali_R As New Lista_Razze_Animali_R
            Dim dtRazze As DataTable = listaRazzeAnimali_R.Leggi(GEN_COD, SPE_COD, -1, enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objP_Server)

            r.RispostaStringa = JsonConvert.SerializeObject(dtRazze)
            r.RispostaOK = True

        Catch ex As Exception
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.RispostaOK = False
        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function getRazzeGriglia(ByVal SPE_COD, ByVal GEN_COD) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objP_Server As New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objP_Server) Then
            r.Sessione = False
            Return r
        End If

        Dim objP_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objP_Utenti) Then
            r.Sessione = False
            Return r
        End If

        Try
            Lingua.Gias_InizializzaCultura_DaSession()

            Dim listaRazzeAnimali_R As New Lista_Razze_Animali_R
            Dim dtRazze As DataTable = listaRazzeAnimali_R.Leggi(GEN_COD, SPE_COD, -1, enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objP_Server)

            r.RispostaStringa = JsonConvert.SerializeObject(New DataView(dtRazze.Select.CopyToDataTable).ToTable(False, "RAZ_DES", "RAZ_COD"))
            r.RispostaOK = True

        Catch ex As Exception
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.RispostaOK = False
        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function getRazzeGrigliaMadre(ByVal SPE_COD, ByVal GEN_COD) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objP_Server As New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objP_Server) Then
            r.Sessione = False
            Return r
        End If

        Dim objP_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objP_Utenti) Then
            r.Sessione = False
            Return r
        End If

        Try
            Lingua.Gias_InizializzaCultura_DaSession()

            Dim listaRazzeAnimali_R As New Lista_Razze_Animali_R
            Dim dtRazze As DataTable = listaRazzeAnimali_R.Leggi(GEN_COD, SPE_COD, -1, enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objP_Server)
            Dim dtResp As DataTable = New DataView(dtRazze.Select.CopyToDataTable).ToTable(False, "RAZ_DES", "RAZ_COD")

            dtResp.Columns(0).ColumnName = "RazDes_Madre"
            dtResp.Columns(1).ColumnName = "RazCod_Madre"
            dtResp.AcceptChanges()

            r.RispostaStringa = JsonConvert.SerializeObject(dtResp)
            r.RispostaOK = True

        Catch ex As Exception

            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.RispostaOK = False
        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function getRazzeGrigliaPadre(ByVal SPE_COD, ByVal GEN_COD) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objP_Server As New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objP_Server) Then
            r.Sessione = False
            Return r
        End If

        Dim objP_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objP_Utenti) Then
            r.Sessione = False
            Return r
        End If

        Try
            Lingua.Gias_InizializzaCultura_DaSession()

            Dim listaRazzeAnimali_R As New Lista_Razze_Animali_R
            Dim dtRazze As DataTable = listaRazzeAnimali_R.Leggi(GEN_COD, SPE_COD, -1, enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objP_Server)
            Dim dtResp As DataTable = New DataView(dtRazze.Select.CopyToDataTable).ToTable(False, "RAZ_DES", "RAZ_COD")

            dtResp.Columns(0).ColumnName = "RazDes_Padre"
            dtResp.Columns(1).ColumnName = "RazCod_Padre"
            dtResp.AcceptChanges()

            r.RispostaStringa = JsonConvert.SerializeObject(dtResp)
            r.RispostaOK = True

        Catch ex As Exception
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.RispostaOK = False
        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function salvataggioModificaMultipla(ByVal obj_ModificaMultipla, ByVal selected_add) As RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim r As New RispostaStandard
        Dim Zoo_Animali As New Zoo_Animali

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Utenti) Then
            r.Sessione = False
            Return r
        End If


        Try
            Lingua.Gias_InizializzaCultura_DaSession()

            Dim Zoo_Animali_Distinte As New Zoo_Animali_Distinte
            Dim gefutils As New Gias_EF_Utility
            Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri_Server.StringaConnessione)
            Dim GiasContext As New Gias_DeveloperServer_Entities(EFConnString)
            Dim modificheEF = False

            For Each i In selected_add
                For Each j In obj_ModificaMultipla.item("anagrafica")
                    If (j = "Lotto") Then


                        Dim cod_animale As Integer = Int(i.item("Cod_Animale"))
                        Dim piva As String = i.item("PIVA").ToString()

                        Dim LottoInDB = (From a In GiasContext.Zoo_Animali_Distinte Where a.PIVA = piva AndAlso a.Cod_Animale = cod_animale).FirstOrDefault

                        LottoInDB.Codice_Distinta = i.item("Lotto")

                        'The property 'Cod_Progetto' is part of the object's key information and cannot be modified
                        Zoo_Animali_Distinte.Modifica(LottoInDB, GiasContext, objParametri_Server, enum_Id_Servizio.GiasOnline, False)
                        modificheEF = True
                    ElseIf (j = "Cod_Contatto") Then
                        Zoo_Animali.Modifica_Parametrizzata(i.item("PIVA"), i.item("Sa_Cod"), i.item("Cod_Animale"), "CF_Fornitore", i.item(j), "", objParametri_Server)
                    ElseIf (j = "Validato") Then
                        Zoo_Animali.Modifica_Parametrizzata(i.item("PIVA"), i.item("Sa_Cod"), i.item("Cod_Animale"), "Validato", IIf(i.item(j).Equals("Si"), 1, 0), "", objParametri_Server)
                    ElseIf (j = "Incremento_Teorico") Then
                        Zoo_Animali.Modifica_Parametrizzata(i.item("PIVA"), i.item("Sa_Cod"), i.item("Cod_Animale"), "Incremento_Teorico", IIf(IsNothing(i.item(j)), 0, i.item(j)), "", objParametri_Server)
                    Else
                        Zoo_Animali.Modifica_Parametrizzata(i.item("PIVA"), i.item("Sa_Cod"), i.item("Cod_Animale"), j, i.item(j), "", objParametri_Server)
                    End If

                Next
            Next

            If modificheEF Then
                GiasContext.SaveChanges()
            End If

            r.RispostaStringa = JsonConvert.SerializeObject("True")
            r.RispostaOK = True

        Catch ex As Exception
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.RispostaOK = False
            r.RispostaStringa = JsonConvert.SerializeObject("False")
        End Try
        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function salvataggioPresetColumns(ByVal listColumns) As RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objImpostazioni_W As New Utenti_Impostazioni_W
        Dim objImpostazioni_R As New Utenti_Impostazioni_Read
        Dim r As New RispostaStandard

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Utenti) Then
            r.Sessione = False
            Return r
        End If


        Try

            Dim dt = objImpostazioni_R.Leggi(enum_Impostazioni_Utenti.ModificaMultiplaZoo, 0, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Utenti)
            If dt.Rows.Count > 0 Then
                objImpostazioni_W.Modifica(enum_Impostazioni_Utenti.ModificaMultiplaZoo, listColumns, "", "", "", AGRODATAINIZIO, AGRODATAFINE, objParametri_Utenti)
            Else
                objImpostazioni_W.Scrivi(enum_Impostazioni_Utenti.ModificaMultiplaZoo, listColumns, "", "", "", AGRODATAINIZIO, AGRODATAFINE, objParametri_Utenti)
            End If

            r.RispostaStringa = JsonConvert.SerializeObject("True")
            r.RispostaOK = True

        Catch ex As Exception
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.RispostaOK = False
            r.RispostaStringa = JsonConvert.SerializeObject("False")
        End Try
        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function salvataggioModificaMultiplaGriglia(ByVal updatedRecords, ByVal obj_ModificaMultipla) As RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim r As New RispostaStandard
        Dim Zoo_Animali As New Zoo_Animali

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Utenti) Then
            r.Sessione = False
            Return r
        End If

        Dim Zoo_Animali_Distinte As New Zoo_Animali_Distinte
        Dim str = JsonConvert.SerializeObject(updatedRecords)

        Try
            Lingua.Gias_InizializzaCultura_DaSession()

            For Each i In updatedRecords
                For Each j In obj_ModificaMultipla.item("anagrafica")
                    Select Case (j)
                        Case "Metodo_Produzione"
                            Select Case i.item("Metodo_Produzione")
                                Case "Integrato"
                                    Zoo_Animali.Modifica_Parametrizzata(i.item("PIVA"), i.item("Sa_Cod"), i.item("Cod_Animale"), j, 1, "", objParametri_Server)
                                Case "In Conversione"
                                    Zoo_Animali.Modifica_Parametrizzata(i.item("PIVA"), i.item("Sa_Cod"), i.item("Cod_Animale"), j, 2, "", objParametri_Server)
                                Case "Biologico"
                                    Zoo_Animali.Modifica_Parametrizzata(i.item("PIVA"), i.item("Sa_Cod"), i.item("Cod_Animale"), j, 3, "", objParametri_Server)
                            End Select

                        Case "Razza_Madre"
                            Zoo_Animali.Modifica_Parametrizzata(i.item("PIVA"), i.item("Sa_Cod"), i.item("Cod_Animale"), j, i.item("RazCod_Madre"), "", objParametri_Server)
                        Case "Razza_Padre"
                            Zoo_Animali.Modifica_Parametrizzata(i.item("PIVA"), i.item("Sa_Cod"), i.item("Cod_Animale"), j, i.item("RazCod_Padre"), "", objParametri_Server)
                        Case "Data_Documento_Ingresso"
                            If (Not (i.item(j) = Nothing Or i.item(j) = "")) Then
                                Zoo_Animali.Modifica_Parametrizzata(i.item("PIVA"), i.item("Sa_Cod"), i.item("Cod_Animale"), j, i.item(j), "", objParametri_Server)
                            End If
                        Case "Data_Documento_Uscita"
                            If (Not (i.item(j) = Nothing Or i.item(j) = "")) Then
                                Zoo_Animali.Modifica_Parametrizzata(i.item("PIVA"), i.item("Sa_Cod"), i.item("Cod_Animale"), j, i.item(j), "", objParametri_Server)
                            End If
                        Case "Codice_Distinta" 'Lotto
                            Dim gefutils As New Gias_EF_Utility
                            Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri_Server.StringaConnessione)
                            Dim GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

                            Dim cod_animale As Integer = Int(i.item("Cod_Animale"))
                            Dim piva As String = i.item("PIVA").ToString()

                            Dim LottoInDB = (From a In GiasContext.Zoo_Animali_Distinte Where a.PIVA = piva AndAlso a.Cod_Animale = cod_animale).FirstOrDefault

                            LottoInDB.Codice_Distinta = i.item("Codice_Distinta")

                            Zoo_Animali_Distinte.Modifica(LottoInDB, GiasContext, objParametri_Server)
                        Case "CF_FornFatt"
                            If IsNothing(i.item("CF_FornFatt")) OrElse i.item("CF_FornFatt").Equals("0") Then
                                Zoo_Animali.Modifica_Parametrizzata(i.item("PIVA"), i.item("Sa_Cod"), i.item("Cod_Animale"), "CF_Fornitore", "", "", objParametri_Server)
                            Else
                                Zoo_Animali.Modifica_Parametrizzata(i.item("PIVA"), i.item("Sa_Cod"), i.item("Cod_Animale"), "CF_Fornitore", i.item("CF_FornFatt"), "", objParametri_Server)
                            End If
                        Case "CF_FornProv"
                            If IsNothing(i.item("CF_FornProv")) OrElse i.item("CF_FornProv").Equals("0") Then
                                Zoo_Animali.Modifica_Parametrizzata(i.item("PIVA"), i.item("Sa_Cod"), i.item("Cod_Animale"), "Fornitore_Provenienza", "", "", objParametri_Server)
                            Else
                                Zoo_Animali.Modifica_Parametrizzata(i.item("PIVA"), i.item("Sa_Cod"), i.item("Cod_Animale"), "Fornitore_Provenienza", i.item("CF_FornProv"), "", objParametri_Server)
                            End If
                        Case "CF_StallaSvezz"
                            If IsNothing(i.item("CF_StallaSvezz")) OrElse i.item("CF_StallaSvezz").Equals("0") Then
                                Zoo_Animali.Modifica_Parametrizzata(i.item("PIVA"), i.item("Sa_Cod"), i.item("Cod_Animale"), "Stalla_Svezzamento", "", "", objParametri_Server)
                            Else
                                Zoo_Animali.Modifica_Parametrizzata(i.item("PIVA"), i.item("Sa_Cod"), i.item("Cod_Animale"), "Stalla_Svezzamento", i.item("CF_StallaSvezz"), "", objParametri_Server)
                            End If
                        'Case "Fornitore"
                        '    Zoo_Animali.Modifica_Parametrizzata(i.item("PIVA"), i.item("Sa_Cod"), i.item("Cod_Animale"), "CF_Fornitore", i.item(j), "", objParametri_Server)
                        Case "Data_DDT_Ingresso"
                            If IsNothing(i.item(j)) Then
                                'Zoo_Animali.Modifica_Parametrizzata(i.item("PIVA"), i.item("Sa_Cod"), i.item("Cod_Animale"), "Data_Documento_Ingresso", AGRODATAINIZIO, "", objParametri_Server)
                            Else
                                Zoo_Animali.Modifica_Parametrizzata(i.item("PIVA"), i.item("Sa_Cod"), i.item("Cod_Animale"), "Data_DDT_Ingresso", i.item(j), "", objParametri_Server)
                            End If
                        Case "Data_DDT_Uscita"
                            If IsNothing(i.item(j)) Then
                                'Zoo_Animali.Modifica_Parametrizzata(i.item("PIVA"), i.item("Sa_Cod"), i.item("Cod_Animale"), "Data_Documento_Uscita", AGRODATAFINE, "", objParametri_Server)
                            Else
                                Zoo_Animali.Modifica_Parametrizzata(i.item("PIVA"), i.item("Sa_Cod"), i.item("Cod_Animale"), "Data_DDT_Uscita", i.item(j), "", objParametri_Server)
                            End If
                        Case "Validato"
                            If IsNothing(i.item(j)) Then
                                'Zoo_Animali.Modifica_Parametrizzata(i.item("PIVA"), i.item("Sa_Cod"), i.item("Cod_Animale"), "Data_Documento_Uscita", AGRODATAFINE, "", objParametri_Server)
                            Else
                                Zoo_Animali.Modifica_Parametrizzata(i.item("PIVA"), i.item("Sa_Cod"), i.item("Cod_Animale"), "Validato", IIf(i.item(j).Equals("Si"), 1, 0), "", objParametri_Server)
                            End If
                        Case "Incremento_Teorico"
                            If IsNothing(i.item(j)) Then
                                Zoo_Animali.Modifica_Parametrizzata(i.item("PIVA"), i.item("Sa_Cod"), i.item("Cod_Animale"), "Incremento_Teorico", 0, "", objParametri_Server)
                            Else
                                Zoo_Animali.Modifica_Parametrizzata(i.item("PIVA"), i.item("Sa_Cod"), i.item("Cod_Animale"), "Incremento_Teorico", i.item(j), "", objParametri_Server)
                            End If
                        Case Else
                            Dim field = j
                            Zoo_Animali.Modifica_Parametrizzata(i.item("PIVA"), i.item("Sa_Cod"), i.item("Cod_Animale"), field, i.item(j), "", objParametri_Server)
                    End Select
                Next
            Next
            r.RispostaStringa = JsonConvert.SerializeObject("True")
            r.RispostaOK = True

        Catch ex As Exception
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.RispostaOK = False
            r.RispostaStringa = JsonConvert.SerializeObject("False")
        End Try
        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Lettura_Fornitori() As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametri_Server As New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Utenti) Then
            r.Sessione = False
            Return r
        End If

        Try
            Lingua.Gias_InizializzaCultura_DaSession()

            Dim Leggi_Contatti_R As New Contatti_R

            Dim DT As DataTable = Leggi_Contatti_R.Leggi_Fornitori(objParametri_Server.PivaSuperUser, objParametri_Server)

            r.RispostaStringa = Newtonsoft.Json.JsonConvert.SerializeObject(DT)
            r.RispostaOK = True

        Catch ex As Exception

            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.RispostaOK = False
        End Try

        Return r

    End Function


End Class