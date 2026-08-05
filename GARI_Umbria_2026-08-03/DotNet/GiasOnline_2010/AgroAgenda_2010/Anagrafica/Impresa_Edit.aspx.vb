Imports System.Web
Imports System.Web.Services

Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreUtility
Imports AgronicaCoreUtility.CaricaListControl

Imports AgronicaCoreModello.ParametriAgenda_Temp
Imports AgronicaCoreUtentiDAL
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreXML
Imports AgroAgenda_2010.Resources

Partial Class Impresa_Edit
    Inherits System.Web.UI.Page

    Dim objParametri_Server, objParametri_Utenti As AgronicaCoreParametri
    Dim objParametriAgenda As ParametriAgenda


    Dim EseguitaOperazione As Boolean
    '----- Gestione Querystring
    Dim Qs_Key As String
    Dim Qs_Operazione As String
    Dim Qs_Piva As String
    Dim Qs_PivaPadre As String
    Dim Qs_PivaNuova As String
    Dim Qs_PaginaRitorno As String
    Dim Qs_Visibilita As Integer = 0
    Dim xPiva As String


    Public jsPadre As String
    Public jsCodici As String

    Public Operazione As Integer
    Dim Operazione_Contatti As Integer

    'gestione risorse umane
    Dim EsistenzaRisorseUmane As Boolean

    Public permessi As PermessiUtente

    Protected Overrides Sub InitializeCulture()
        MyBase.InitializeCulture()
        Lingua.Gias_InizializzaCultura_DaSession()

    End Sub


#Region "ws"

    '########################################################################################
    <Script.Services.ScriptMethod()> _
 <WebMethod(EnableSession:=True)> _
    Public Shared Function VerificaPivaPresente(ByVal testoPiva As String)
        Dim objParametri_Server As AgronicaCoreParametri
        objParametri_Server = HttpContext.Current.Session("ASG_objParametri_Server")

        Dim PivaNuova As String
        'Prelevo la partita IVA che l'utente sta inserendo
        PivaNuova = testoPiva

        Dim objImpreser As New AgronicaCoreAnagrafeDAL.Imprese_Read
        If objImpreser.VerificaEsistenza_PivaGIAS(PivaNuova, objParametri_Server) Then
            Return True
            'TxtVerificaPiva.Visible = True
            'TxtVerificaPiva.Text = "Gia' Presente"
            'TxtVerificaPiva.CssClass = "danger"

        Else
            Return False
            'TxtVerificaPiva.Visible = True
            'TxtVerificaPiva.Text = "Non Presente"
            'TxtVerificaPiva.CssClass = "success"

        End If

    End Function


    '#########################################################################################
    <Script.Services.ScriptMethod()> _
    <WebMethod(EnableSession:=True)> _
    Public Shared Function Carica_Select_Padri(ByVal parametro As String) As String

        Dim cmb_imprese2 As New DropDownList

        Lingua.Gias_InizializzaCultura_DaSession()

        AgronicaCoreUtility.CaricaListControl.ImpreseConFiltroUtente(cmb_imprese2, True, AgronicaAgenda_2010.Seleziona.ToUpper(), "",
                                                                         " Imprese.rag_soc LIKE '%" & parametro & "%'", " ORDER BY Rag_Soc asc",
                                                                         HttpContext.Current.Session("ASG_objParametri_Server"),
                                                                         HttpContext.Current.Session("ASG_objParametri_Utenti"))
        Dim rval As String = ""
        For Each itm As ListItem In cmb_imprese2.Items
            rval &= "<option value=""" & itm.Value & """>" & itm.Text & "</option>"
        Next

        'Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        'HttpContext.Current.Session("prova") = "caio"

        Return rval

    End Function


    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Carica_Province(ByVal stato As String) As Array

        Dim dll_Provincia As New DropDownList
        Dim rval(0) As String
        Dim options As String = ""


        CaricaListControl.Provincie(dll_Provincia,
                                    True, "", "",
                                    False, 1, "", "", "", "", "", "", "", HttpContext.Current.Session("ASG_objParametri_Server"),
                                    stato)


        For Each itm As ListItem In dll_Provincia.Items
            options &= "<option value=""" & itm.Value & """>" & itm.Text & "</option>"
        Next

        rval(0) = options

        Return rval

    End Function



    <Script.Services.ScriptMethod()> _
    <WebMethod(EnableSession:=True)> _
    Public Shared Function Carica_Comuni(ByVal provincia As String) As Array


        Dim cmb_comuni2 As New DropDownList
        Dim rval(1) As String
        Dim options As String = ""

        If provincia <> "" Then

            CaricaListControl.Comuni(cmb_comuni2,
                                     True, "", "",
                                     provincia, False, 1, "", "", HttpContext.Current.Session("ASG_objParametri_Server"))


            For Each itm As ListItem In cmb_comuni2.Items
                options &= "<option value=""" & itm.Value & """>" & itm.Text & "</option>"
            Next

            rval(0) = options

            Dim objI As New AgronicaCoreMetaSchemaDAL.Istat_R
            'Imposto la textbox del CODICE ISTAT
            rval(1) = objI.CodIstat_from_Provincia(provincia, HttpContext.Current.Session("ASG_objParametri_Server"))


        End If

        Return rval

    End Function






    <Script.Services.ScriptMethod()> _
   <WebMethod(EnableSession:=True)> _
    Public Shared Function EliminaGerarchiaPadre(ByVal Piva As String) As rispostaStandard
        Dim r As New RispostaStandard
        Dim Dt As DataTable
        Dt = HttpContext.Current.Session("dt_Padri")

        'Controllo se esiste già la voce che si vuole inserire
        For i = 0 To Dt.Rows.Count - 1
            If (Dt.Rows(i).Item("Piva") = Piva) Then
                Dt.Rows.RemoveAt(i)
                Exit For
            End If
        Next
        HttpContext.Current.Session("dt_Padri") = Dt
        Dim str_Risposta = DT_to_Json_GerarchiaPadre(Dt)
        r.RispostaOK = True
        r.RispostaStringa = str_Risposta
        Return r
    End Function



    <Script.Services.ScriptMethod()> _
  <WebMethod(EnableSession:=True)> _
    Public Shared Function EliminaCodice(ByVal Id_Cod As String) As rispostaStandard
        Dim r As New RispostaStandard
        Dim Dt As DataTable
        Dt = HttpContext.Current.Session("dt_Codici")

        'Controllo se esiste già la voce che si vuole inserire
        For i = 0 To Dt.Rows.Count - 1
            If (Dt.Rows(i).Item("Id_Cod") = Id_Cod) Then
                Dt.Rows.RemoveAt(i)
                Exit For
            End If
        Next
        HttpContext.Current.Session("dt_Codici") = Dt
        Dim str_Risposta = DT_to_Json_Codici(Dt)
        r.RispostaOK = True
        r.RispostaStringa = str_Risposta
        Return r
    End Function





    '#########################################################################################
    <Script.Services.ScriptMethod()> _
    <WebMethod(EnableSession:=True)> _
    Public Shared Function Aggiungi_Padre(ByVal p_iva As String, ByVal rag_soc As String) As rispostaStandard
        Dim r As New RispostaStandard
        Dim objParametriAgenda As ParametriAgenda
        objParametriAgenda = New ParametriAgenda

        Dim flag As Boolean = True
        Dim Dt As New DataTable

        Lingua.Gias_InizializzaCultura_DaSession()

        If (IsNothing(HttpContext.Current.Session("dt_Padri"))) Then
            'Dim objGerarchia As New AgronicaCoreAnagrafeDAL.GerarchiaImprese_R
            'Dim TempDt As DataTable
            'Dim ArrayPadri() As String

            'Dim objParametriAgenda As New ParametriAgenda
            ''Dim xChiave As String
            'Dim xPiva As String = objParametriAgenda.Piva
            'Dim objParametri_Server As New AgronicaCoreDataProvider.AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))

            ''Leggo le imprese padri associate al profilo selezionato			
            'Dt = objGerarchia.LeggixFiglio(xPiva, _
            '                                 AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, _
            '                                 "", "", objParametri_Server)

            ''Elimino gli oggetti COM
            'objGerarchia = Nothing
            Dt.Columns.Add(New DataColumn("Piva", GetType(String)))
            Dt.Columns.Add(New DataColumn("Rag_Soc", GetType(String)))
            Dt.Columns.Add(New DataColumn("Validita_Inizio", GetType(String)))
            Dt.Columns.Add(New DataColumn("Validita_Fine", GetType(String)))

        Else
            Dt = HttpContext.Current.Session("dt_Padri")
        End If

        'Creo una nuova riga
        Dim riga As DataRow
        riga = Dt.NewRow

        'Definisco i valori
        riga.Item("Piva") = p_iva
        riga.Item("Rag_Soc") = rag_soc

        riga.Item("Validita_Inizio") = "..."
        riga.Item("Validita_Fine") = "..."



        'Controllo se esiste già la voce che si vuole inserire
        For i = 0 To Dt.Rows.Count - 1

            If (Dt.Rows(i).Item("Piva") = p_iva) Then
                flag = False
                r.Errore = AgronicaAgenda_2010.ImpresaPadreGiàInserita
            End If
        Next


        If objParametriAgenda.Tipo_Operazione <> 1 Then
            ' Controllo se l'azienda inserita NON è SE STESSA
            If p_iva = objParametriAgenda.Piva Then
                flag = False
                r.Errore = AgronicaAgenda_2010.AziendaCorrenteNonPuòEsserePadreDiSeStessa
            End If

        End If


        If (flag) Then
            Dt.Rows.Add(riga)
            HttpContext.Current.Session("dt_Padri") = Dt
            Dim str_Risposta = DT_to_Json_GerarchiaPadre(Dt)
            r.RispostaOK = True
            r.RispostaStringa = str_Risposta
        Else
            r.RispostaOK = False

        End If

        Return r
    End Function


    '#########################################################################################
    <Script.Services.ScriptMethod()> _
    <WebMethod(EnableSession:=True)> _
    Public Shared Function DT_to_Json_GerarchiaPadre(ByVal dt As DataTable) As String

        Dim objParametriAgenda As Integer
        objParametriAgenda = HttpContext.Current.Session("operazione")

        Lingua.Gias_InizializzaCultura_DaSession()

        'creo la lista delle colonne da visualizzare
        Dim l As New List(Of ColonneNome)

        'aggiungo i pulsanti per modifica ed eliminazione
        Dim c = New ColonneNome("Piva", "Tool", "string") 'i18n

        Dim listaBtn = New List(Of btnAzioni)

        If objParametriAgenda <> enum_TipoOperazioneDB.Lettura Then
            listaBtn.Add(New btnAzioni("fa-trash-o del_elem", "EliminaGerarchiaPadre(this);"))

        End If


        Dim tool As New ToolStandard(listaBtn)
        tool.cssColonna = "colmodmovimenti"
        c._FormatoParticolare = tool.toString()
        c._Filtrabile = False
        c._ColonnaDiSelezione = True
        l.Add(c)


        Dim cn As New ColonneNome("Piva", AgronicaAgenda_2010.PartitaIVA, "string")
        l.Add(cn)

        cn = New ColonneNome("Rag_Soc", AgronicaAgenda_2010.RagioneSociale, "string")
        l.Add(cn)



        'ritorno la tabella trasformata in json (aggiustando anche le colonne)
        Dim js As New AgronicaCoreDataProvider.JSON_DataTable
        Dim risp = "{}"
        If dt IsNot Nothing Then
            risp = js.JSON_DataTable(dt, l)
        End If
        Return risp
    End Function


    '#########################################################################################
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Set_Comune(ByVal comune As String, ByVal nome_comune As String)
        Dim Arrayp() As String
        Dim Com As String = ""

        HttpContext.Current.Session("comune_settato") = comune
        HttpContext.Current.Session("nome_comune_settato") = nome_comune

        If Trim(comune) <> "" Then
            Arrayp = Split(comune, "|")
            Com = Arrayp(1)
        End If
        HttpContext.Current.Session("com") = Com

    End Function




#End Region



    '#########################################################################################
    <Script.Services.ScriptMethod()> _
    <WebMethod(EnableSession:=True)> _
    Public Shared Function Aggiungi_Codice(ByVal codice As String, ByVal valore As String, ByVal codice_id As String, ByVal DataInizio As String, ByVal DataFine As String) As RispostaStandard

        Dim r As New RispostaStandard

        Dim Dt As New DataTable
        Dim Dr As DataRow
        Dim Contatore As Integer
        Dim flag As Boolean = True

        Lingua.Gias_InizializzaCultura_DaSession()

        Dt = HttpContext.Current.Session("dt_Codici")

        If Not IsDate(DataInizio) Then
            DataInizio = AGRODATAINIZIO
        Else
            DataInizio = CDate(DataInizio)
        End If

        If Not IsDate(DataFine) Then
            DataFine = AGRODATAFINE
        Else
            DataFine = CDate(DataFine)
        End If


        '----- Cerco il valore massimo del contatore

        Contatore = 0

        If Dt IsNot Nothing Then
            For i = 0 To Dt.Rows.Count - 1
                If Dt.Rows(i).Item("Contatore") > Contatore Then
                    Contatore = Dt.Rows(i).Item("Contatore")
                End If
            Next
        End If

        Contatore += 1

        '----- Inserisco la riga nel datagrid

        'Creo una nuova riga
        Dr = Dt.NewRow

        'Definisco i valori
        Dr.Item("Contatore") = Contatore

        Dr.Item("Id_Cod") = codice_id
        Dr.Item("Descrizione") = codice
        Dr.Item("Val_Cod") = valore

        If DataInizio = #1/1/1900# Then
            Dr.Item("Validita_Inizio") = "..."
        Else
            ' Dr.Item("Validita_Inizio") = DataInizio.ToShortDateString
            Dr.Item("Validita_Inizio") = DataInizio
        End If

        If DataFine = #12/31/2100# Then
            Dr.Item("Validita_Fine") = "..."
        Else
            ' Dr.Item("Validita_Fine") = DataFine.ToShortDateString
            Dr.Item("Validita_Fine") = DataFine
        End If


        'Controllo se esiste già la voce che si vuole inserire
        Dim dtCopy As DataTable = Dt.Copy
        Dim dtId_Cod As DataRow() = dtCopy.Select(" Id_Cod = " & CStr(codice_id) & " ")
        If dtId_Cod.Count > 0 Then

            For i = 0 To dtId_Cod.Count - 1
                Dim DataInizio_Id = AGRODATAINIZIO
                Dim DataFine_Id = AGRODATAFINE
                If dtId_Cod(i)("Validita_Inizio") <> "..." Then
                    DataInizio_Id = CDate(dtId_Cod(i)("Validita_Inizio"))
                End If

                If dtId_Cod(i)("Validita_Fine") <> "..." Then
                    DataFine_Id = CDate(dtId_Cod(i)("Validita_Fine"))
                End If

                If DataInizio_Id <= DataInizio AndAlso DataFine_Id >= DataInizio Then
                    flag = False
                    Exit For
                End If

                If DataInizio_Id <= DataFine AndAlso DataFine_Id >= DataFine Then
                    flag = False
                    Exit For
                End If

            Next
        End If


        If (flag) Then
            'Associo alla tabella la nuova riga creata
            Dt.Rows.Add(Dr)
            HttpContext.Current.Session("dt_Codici") = Dt
            Dim str_Risposta = DT_to_Json_Codici(Dt)
            r.RispostaOK = True
            r.RispostaStringa = str_Risposta
        Else
            r.RispostaOK = False
            r.Errore = "Codice già inserito"
        End If

        Return r

    End Function

    <Script.Services.ScriptMethod()> _
    <WebMethod(EnableSession:=True)> _
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

        'Dim cn As New ColonneNome("Contatore", "Contatore", "string")
        'l.Add(cn)

        'Dim cn As New ColonneNome("Id_Cod", "Id_Cod", "string")
        'cn._hidden = True
        'l.Add(cn)

        Dim cn As New ColonneNome("descrizione", AgronicaAgenda_2010.Codice, "string")
        l.Add(cn)

        cn = New ColonneNome("Val_Cod", AgronicaAgenda_2010.Valore, "string")
        l.Add(cn)

        cn = New ColonneNome("Validita_Inizio", AgronicaAgenda_2010.Dal, "string")
        l.Add(cn)

        cn = New ColonneNome("Validita_Fine", AgronicaAgenda_2010.Al, "string")
        l.Add(cn)



        'ritorno la tabella trasformata in json (aggiustando anche le colonne)
        Dim js As New AgronicaCoreDataProvider.JSON_DataTable
        Dim risp As String = js.JSON_DataTable(dt, l)
        Return risp
    End Function


    <Script.Services.ScriptMethod()> _
  <WebMethod(EnableSession:=True)> _
    Public Shared Function WS_ControllaPiva(ByVal Piva As String) As rispostaStandard
        Dim r As New RispostaStandard
        'Dim Dt As DataTable
        'Dt = HttpContext.Current.Session("dt_Padri")

        ''Controllo se esiste già la voce che si vuole inserire
        'For i = 0 To Dt.Rows.Count - 1
        '    If (Dt.Rows(i).Item("Piva") = Piva) Then
        '        Dt.Rows.RemoveAt(i)
        '        Exit For
        '    End If
        'Next
        'HttpContext.Current.Session("dt_Padri") = Dt
        'Dim str_Risposta = DT_to_Json_GerarchiaPadre(Dt)

        Lingua.Gias_InizializzaCultura_DaSession()

        Dim operazione As Integer
        operazione = HttpContext.Current.Session("operazione")


        Dim objImpreseR As New AgronicaCoreAnagrafeDAL.Imprese_Read
        If objImpreseR.VerificaEsistenza_PivaGIAS(Piva, HttpContext.Current.Session("ASG_objParametri_Server")) = True AndAlso
           operazione = 1 Then
            r.RispostaOK = False
            r.Errore = AgronicaAgenda_2010.LaPartitaIvaInseritaEsisteGià
        Else
            r.RispostaOK = True
        End If

        Return r
    End Function



    Protected Overrides ReadOnly Property PageStatePersister As PageStatePersister
        Get
            Return New SessionPageStatePersister(Me)
        End Get
    End Property



    '##########################################################################################################################################
    Private Sub AnnullaTutto(ByVal sender As Object, ByVal e As System.EventArgs)

        ' Pulisco la Sessione
        HttpContext.Current.Session("dt_Padri") = Nothing
        HttpContext.Current.Session("dt_Codici") = Nothing
        HttpContext.Current.Session("comune_settato") = Nothing
        HttpContext.Current.Session("nome_comune_settato") = Nothing

        Dim TargetUrl As String
        TargetUrl = Master.TrovaRedirectCorretto(False, enum_PagineGiasOnline.MenuAnagrafica, objParametriAgenda)

        If Qs_Visibilita <> 0 Then
            TargetUrl &= "?visibilita=" & CStr(Qs_Visibilita)
        End If

        Response.Redirect(TargetUrl)

    End Sub

    Private Sub Impresa_Edit_Init(sender As Object, e As System.EventArgs) Handles Me.Init

        ' VAnni: 2/10/2017: a seguito di AgroMaseterPage
        'AddHandler CType(Page.Master.FindControl("ImgBtn_AnnullaTutto"), ImageButton).Click, AddressOf Me.AnnullaTutto
        AddHandler CType(Me.Master, AgendaBootstrap).ImgBtn_AnnullaTutto.Click, AddressOf Me.AnnullaTutto
        Master.flag_MostraBtnIndietro = True
    End Sub

    ' Ripristina combo stato per codice e in seconda battuta per descrizione (per retocompatibiltà)
    Private Sub Ripristina_Cmb_Stato(ByVal stato As String)
        Dim index = cmb_Stato.Items.IndexOf(cmb_Stato.Items.FindByValue(stato))
        If index = -1 Then
            stato = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(stato.ToLower())
            index = cmb_Stato.Items.IndexOf(cmb_Stato.Items.FindByText(stato))
        End If
        cmb_Stato.SelectedIndex = If(index = -1, cmb_Stato.Items.IndexOf(cmb_Stato.Items.FindByValue("IT")), index)
    End Sub

    '##########################################################################################################################################
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        permessi = New PermessiUtente()

        Master.flag_pag_Anagrafica = True
        'If Not Page.IsPostBack Then
        '    HttpContext.Current.Session("dt_Padri") = Nothing
        '    HttpContext.Current.Session("dt_Codici") = Nothing
        'End If

        '----- Verifico che l'utente sia autenticato
        If Session("ASG_Utente_Username") = "" Then
            Response.Redirect("~/Custom500.aspx")
        End If

        '---
        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))
        objParametri_Utenti = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))
        '---

        ValidationSettings.UnobtrusiveValidationMode = UnobtrusiveValidationMode.None


        objParametriAgenda = New ParametriAgenda
        'Dim xChiave As String

        If Request.QueryString("visibilita") IsNot Nothing AndAlso IsNumeric(Request.QueryString("visibilita")) Then
            Qs_Visibilita = CInt(Request.QueryString("visibilita"))
        End If

        'Se Qs_Visibilita = 0 è stato aperto dal Menu Anagrafe generale e quindi utilizzo la versione standard della grafica
        If Qs_Visibilita = 0 Then
            Master.Master_versione = VERSIONE_MASTER_DEFAULT
            Master.Header_versione = VERSIONE_HEADER_DEFAULT
        End If

        'Qs_Key = Stringa_Decodifica(Request.QueryString("k").ToString, _
        '                            AgroKey_EncoderDecoder, _
        '                            Server)

        'Qs_Operazione = Stringa_Decodifica(Request.QueryString("o").ToString, _
        '                            AgroKey_EncoderDecoder, _
        '                            Server)

        'Qs_Piva = Stringa_Decodifica(Request.QueryString("p").ToString, _
        '                            AgroKey_EncoderDecoder, _
        '                            Server)

        'Qs_PaginaRitorno = Stringa_Decodifica(Request.QueryString("r").ToString, _
        '                            AgroKey_EncoderDecoder, _
        '                            Server)

        'Recupero Chiave ed Operazione dalla querystring
        'xChiave = Qs_Key
        'Operazione = Qs_Operazione
        'xPiva = Qs_Piva

        'Dim xChiave As String
        'Dim xPiva As String

        Select Case objParametriAgenda.Tipo_Operazione
            Case enum_TipoOperazioneDB.Scrittura
                Master.Lbl_Titolo.Text = AgronicaAgenda_2010.CreazioneNuovaImpresa
                objParametriAgenda.Sa_Cod = 0
                xPiva = ""
                CaricaGriglia_Codici(False, xPiva)
                jsPadre = DT_to_Json_GerarchiaPadre(HttpContext.Current.Session("dt_Padri"))
            Case enum_TipoOperazioneDB.Lettura
                Master.Lbl_Titolo.Text = AgronicaAgenda_2010.LetturaImpresa
                xPiva = objParametriAgenda.Piva
            Case enum_TipoOperazioneDB.Modifica
                Master.Lbl_Titolo.Text = AgronicaAgenda_2010.ModificaImpresa
                xPiva = objParametriAgenda.Piva
        End Select

        Operazione = objParametriAgenda.Tipo_Operazione
        HttpContext.Current.Session("operazione") = Operazione


        '##############################################################
        '#####  Recupero la chiave che identifica l'oggetto  ##########
        '##############################################################

        'Qs_Key = Stringa_Decodifica(Request.QueryString("k").ToString, _
        '                            AgroKey_EncoderDecoder, _
        '                            Server)

        'Qs_Operazione = Stringa_Decodifica(Request.QueryString("o").ToString, _
        '                            AgroKey_EncoderDecoder, _
        '                            Server)

        'Qs_Piva = Stringa_Decodifica(Request.QueryString("p").ToString, _
        '                            AgroKey_EncoderDecoder, _
        '                            Server)

        'Qs_PaginaRitorno = Stringa_Decodifica(Request.QueryString("r").ToString, _
        '                            AgroKey_EncoderDecoder, _
        '                            Server)

        'Recupero Chiave ed Operazione dalla querystring
        ' xChiave = Qs_Key
        ' Operazione = Qs_Operazione

        'ChiaveAlbero_Decodifica_PartitaIVA(Qs_Key, Qs_PivaPadre)


        '==================================
        '======= VERIFICA PERMESSI ========
        '==================================

        Dim UtenteAbilitato_Lettura As Boolean = False

        Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
        'UtenteAbilitato_Lettura = objPermessi.Controlla_Permessi_Utente( _
        '                            Session("ASG_Utente_Username"), _
        '                            Session("ASG_IdServizio"), _
        '                            enum_Security_Attivita.Anagrafica_ParticellaCatastale, _
        '                            enum_Security_Operazione.Lettura, _
        '                            Date.Now, _
        '                            "", _
        '                            objParametri_Utenti)

        'Session("UtenteAbilitato_Lettura") = UtenteAbilitato_Lettura

        Dim UtenteAbilitato_Modifica As Boolean = False
        'UtenteAbilitato_Modifica = objPermessi.Controlla_Permessi_Utente( _
        '                            Session("ASG_Utente_Username"), _
        '                            Session("ASG_IdServizio"), _
        '                            enum_Security_Attivita.Anagrafica_ParticellaCatastale, _
        '                            enum_Security_Operazione.Modifica, _
        '                            Date.Now, _
        '                            "", _
        '                            objParametri_Utenti)

        'Session("UtenteAbilitato_Modifica") = UtenteAbilitato_Modifica


        UtenteAbilitato_Lettura = permessi.getPermesso(enum_Security_Attivita.Anagrafica_CentroAziendale).Lettura
        UtenteAbilitato_Modifica = permessi.getPermesso(enum_Security_Attivita.Anagrafica_CentroAziendale).Scrittura


        Session("UtenteAbilitato_Modifica") = UtenteAbilitato_Modifica
        Session("UtenteAbilitato_Lettura") = UtenteAbilitato_Lettura


        If Not UtenteAbilitato_Modifica Then
            Response.Redirect("../MenuAnagrafica/Menubs_anagrafica.aspx")
            Exit Sub
        End If






        If Not IsPostBack Then

            HttpContext.Current.Session("OTE") = Nothing

            'Pulisco le varie Textbox e ComboBox 
            TxtPiva.Text = ""
            TxtRagioneSociale.Text = ""

            TxtValiditaInizio.Text = ""
            TxtValiditaFine.Text = ""

            Txt_CodIndirizzo.Text = "0"
            Txt_Via.Text = ""
            Txt_Frazione.Text = ""
            Txt_CAP.Text = ""
            'Txt_Stato.Text = "Italia"
            Txt_Note.Text = ""

            'txtCuaaCod.Text = ""
            TxtCodiceValore.Text = ""

            CaricaGriglia_Codici(True, xPiva)

        Else

            CaricaGriglia_Codici(False, xPiva)
            jsPadre = DT_to_Json_GerarchiaPadre(HttpContext.Current.Session("dt_Padri"))
            Exit Sub
        End If



        Dim objCodici As New AgronicaCoreAnagrafeDAL.Imprese_Codici_Read
        Dim DTCodici As DataTable

        Dim StrXmlCodice As String
        Dim StrXmlCodiciAttuali As String

        Dim BaseCode As Integer
        Dim TopCode As Integer


        Dim TipoOperazioneDB As enum_TipoOperazioneDB
        Dim Id_Cod As Integer
        Dim Val_Cod As String
        Dim Validita_Inizio As Date
        Dim Validita_Fine As Date

        Dim i, j As Integer

        '---------------------------------------------------------------------------------
        '----- Tabella Imprese_Codici

        'CaricaGriglia_Codici(False, xPiva)

        Dim objImpreseCodici As New AgronicaCoreAnagrafeDAL.Imprese_Codici_Read

        'Leggo le informazioni sull'impresa selezionata			
        DTCodici = objCodici.Leggi(CStr(xPiva),
                                    0,
                                        AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                        " ((id_cod < 2000 AND id_cod <> 1107) OR (id_cod >= 3000 AND id_cod <> 1107)) AND id_cod <> 1010 ",
                                        "",
                                        objParametri_Server)


        StrXmlCodiciAttuali = ""

        'Se il recordset non e' nullo
        If DTCodici.Rows.Count > 0 Then

            'Calcolo i valori di BaseCode e TopCode 
            Calcola_BaseCode_TopCode(BaseCode, TopCode, Session("ASG_ProgressivoGIAS"))

            For i = 0 To DTCodici.Rows.Count - 1

                '----- Genero l'XML del codice

                'Recupero le informazioni
                TipoOperazioneDB = enum_TipoOperazioneDB.Cancellazione
                Id_Cod = DTCodici.Rows(i).Item("Id_Cod")
                Val_Cod = DTCodici.Rows(i).Item("Val_cod")
                Validita_Inizio = DTCodici.Rows(i).Item("xValidita_Inizio")
                Validita_Fine = DTCodici.Rows(i).Item("xValidita_Fine")

                'Genero l'XML del singolo nodo
                Call XML_Codice(enum_CodificaDecodifica.Codifica,
                                StrXmlCodice,
                                TipoOperazioneDB,
                                Id_Cod,
                                Val_Cod,
                                Validita_Inizio,
                                Validita_Fine,
                                BaseCode,
                                TopCode)

                '----- Inserisco il codice nella stringa complessiva

                StrXmlCodiciAttuali &= StrXmlCodice

                '-----

            Next

        End If



        '----- Salvo la StrXmlCodiciAttuali 

        Session("StrXmlCodiciAttuali") = StrXmlCodiciAttuali



        'Dim UtenteAbilitato As Boolean





        EseguitaOperazione = False
        'PremutoAnnulla = False

        '##############################################################
        '#####  Inizializzo i controlli  ##############################
        '##############################################################


        'Dim xTipoNodo As enum_TipoNodo

        'Dim xSa_Cod As Integer
        'Dim xCampo_Cod As Integer
        'Dim xAppezza As Integer
        'Dim xID_Imp As Integer
        'Dim xPart_Cod As Integer
        'Dim xFabbricato_Cod As Integer
        'Dim xCodFiscale As String


        Dim objIndirizzi As New AgronicaCoreAnagrafeDAL.ImpresexIndirizzi_R
        Dim DTIndirizzi As DataTable

        Dim Errore As String = ""

        Dim Dt_Contatti As DataTable


        ' Select Forme Giuridiche
        CaricaListControl.FormeGiuridiche(Cmb_FormaGiuridica, True, AgronicaAgenda_2010.Seleziona.ToUpper(), "", "", "", objParametri_Server)


        '----- Inizializzo la gestione dei pannelli

        'Imposta_Pannelli(enum_Pannello.Pannello_Riferimenti)






        'Me.Opt_TipoImpresa.Checked = True
        'Me.Opt_TipoCooperativa.Checked = False
        'Me.Opt_TipoOP.Checked = False
        'Me.Opt_TipoConsorzio.Checked = False

        '-----

        'Pulisco la listbox dei codici
        'ListCodici.Items.Clear()

        CaricaListControl.Provincie(dll_Provincia,
                                    True, "", "",
                                    True, 1, "", "", "", "", "", "", "", objParametri_Server)

        'carico la combo dei codici

        Dim StrCodiciAzienda As String

        Dim objCodiceAnagrafe As New AgronicaCoreAnagrafeDAL.Codice_Anagrafe_R

        StrCodiciAzienda = objCodiceAnagrafe.Filtro_Codici_Anagrafe(1, 3, 2, objParametri_Server)

        'Elimino codice CUAA, Titolo Possesso e tecnico perchè già presenti nella form
        StrCodiciAzienda = Replace(StrCodiciAzienda, " Or Codice = 1010", "")
        StrCodiciAzienda = Replace(StrCodiciAzienda, " Codice = 1010", "")
        StrCodiciAzienda = Replace(StrCodiciAzienda, " Or Codice = 1016", "")
        StrCodiciAzienda = Replace(StrCodiciAzienda, "Codice = 1016 Or ", "")
        StrCodiciAzienda = Replace(StrCodiciAzienda, " Or Codice = 1088", "")
        StrCodiciAzienda = Replace(StrCodiciAzienda, " Codice = 1088", "")

        CaricaListControl.Codici(CType(Me.CmbCodice, ListControl),
                                 True, "", "",
                                 0, "",
                                 StrCodiciAzienda, "", objParametri_Server)

        CaricaListControl.CaricaCombo_Tecnici(CmbTecnico, True, AgronicaAgenda_2010.Seleziona.ToUpper(), "", objParametri_Server.PivaSuperUser, "", "", objParametri_Server)

        'metto la stinga restituita dal componente nel formato "cod,cod,cod.."
        'per utilizzarla quando devo caricare i dati
        StrCodiciAzienda = Replace(StrCodiciAzienda, " Or Codice = ", ",")
        StrCodiciAzienda = Right(StrCodiciAzienda, StrCodiciAzienda.Length - 9)

        'If (IsNothing(HttpContext.Current.Session("dt_Codici"))) Then
        '    CaricaGriglia_Codici(True)
        'End If

        '================================================
        'Verifica dei rapporti contabili
        Dim objRappContabR As New AgronicaCoreAnagrafeDAL.Rapporti_Contabili_R
        objRappContabR.RapportiContabiliDiBase_Verifica(Errore, objParametri_Server)

        If Errore <> "" Then
            AgroMsgBox(Errore, Page)
        End If
        '================================================


        'carico il datagrid dei rapporti contabili
        Call CaricaGriglia_Contatti()


        ' carico il datagrid per Gridview_Padri
        Dim Dt_Padri As New DataTable

        Dim Padre_Piva As String
        Dim Padre_RagSoc As String

        '---------------------------------------------------------------
        ' Padri attuali

        '----- Definisco la struttura del DataTable

        Dt_Padri.Columns.Add(New DataColumn("Piva", GetType(String)))
        Dt_Padri.Columns.Add(New DataColumn("Rag_Soc", GetType(String)))
        Dt_Padri.Columns.Add(New DataColumn("Validita_Inizio", GetType(String)))
        Dt_Padri.Columns.Add(New DataColumn("Validita_Fine", GetType(String)))

        '----- Definisco l'insieme di colonne che costituiscono la chiave della tabella

        'Vettore di DataColumn
        Dim DtKeys(0) As DataColumn

        'Valorizzo le celle del vettore
        DtKeys(0) = Dt_Padri.Columns("Piva")

        'Assegno il vettore delle chiavi al DataTable
        Dt_Padri.PrimaryKey = DtKeys

        ' Riempio la tendina delle nazioni
        Dim DT_Nazioni As DataTable
        Dim objNazioni As New AgronicaCoreMetaSchemaDAL.ACCDAA_ANAG_T005_TabellaCodiciPaesiTerziISO3166_R
        DT_Nazioni = objNazioni.Leggi("", "", "Descrizione", objParametri_Server)

        For i = 0 To DT_Nazioni.Rows.Count - 1
            cmb_Stato.Items.Add(New ListItem(DT_Nazioni.Rows(i).Item("Descrizione"), DT_Nazioni.Rows(i).Item("Codice")))

            cmb_Stato.Items(i).Attributes.Add("Gestione_Gerarchia_Geografica", DT_Nazioni.Rows(i).Item("Gestione_Gerarchia_Geografica"))

        Next

        ' setto Italia come default
        cmb_Stato.SelectedIndex = cmb_Stato.Items.IndexOf(cmb_Stato.Items.FindByValue("IT"))

        Dim Organismi_R As New AgronicaCoreAnagrafeDAL.Contatti_R
        Dim DT_Organismi = Organismi_R.Leggi_Contatti_ByCod_Rapporto(True, "", "", enum_Rapporti_Contabili_Standard.Organismo_Di_Controllo, "", "", objParametri_Server)
        CmbOdc.Items.Add(New ListItem(AgronicaAgenda_2010.Seleziona.ToUpper(), "0"))
        For i = 0 To DT_Organismi.Rows.Count - 1
            CmbOdc.Items.Add(New ListItem(DT_Organismi.Rows(i).Item("Rag_Soc").ToString, DT_Organismi.Rows(i).Item("Cod_Risum").ToString))
        Next

        '##############################################################
        '#####  Se sono in MODIFICA carico i dati  ####################
        '##############################################################

        'Definisco il titolo della pagina
        'Master.Lbl_Titolo.Text = "Nuova : " & AgroLabel_Impresa

        If Operazione = enum_TipoOperazioneDB.Scrittura Then

            'INSERIMENTO

            ViewState("Operazione_Contatti") = enum_TipoOperazioneDB.Scrittura

            HttpContext.Current.Session("dt_Padri") = Dt_Padri
            'ViewState("Dt_Padri") = Dt_Padri

            'imposto il default sul fornitore
            For i = 0 To DataGrid_Contatti.Rows.Count - 1

                If DataGrid_Contatti.Rows(i).Cells(3).Text = COD_FORNITORE Then

                    CType(DataGrid_Contatti.Rows(i).FindControl("ChkRapporto"), CheckBox).Checked = True

                    Exit For

                End If

            Next

        ElseIf Operazione = enum_TipoOperazioneDB.Modifica OrElse Operazione = enum_TipoOperazioneDB.Lettura Then
            'If Operazione = enum_TipoOperazioneDB.Modifica Then

            'Definisco il titolo della pagina
            'Master.Lbl_Titolo.Text = "Modifica : " & AgroLabel_Impresa

            'Disattivo/rendo invisibili i checkbox x la creazione automatica Centro Az. e Magazzino
            Chk_Centro.Checked = False
            Chk_Centro.Visible = False
            'Lbl_Centro.Visible = False

            Chk_Magazzino.Checked = False
            Chk_Magazzino.Visible = False
            'Lbl_Magazzino.Visible = False

            'Rendo la TxtPiva a sola lettura
            TxtPiva.ReadOnly = True

            Dim cod_contatto As String = ""

            'Rendo invisibile il pulsante di verifica della PIVA


            'Data la chiave ricavo gli elementi che la compongono
            'ChiaveAlbero_Decodifica_ImpiantiVegetali( _
            '                                    xChiave, _
            '                                    xTipoNodo, _
            '                                    xPiva, _
            '                                    xSa_Cod, _
            '                                    xCampo_Cod, _
            '                                    xAppezza, _
            '                                    xID_Imp, _
            '                                    xCodFiscale, _
            '                                    xFabbricato_Cod)


            '----------------------------------------------------------------------

            '----- Tabella IMPRESE
            Dim DT_Impresa As DataTable

            Dim objImpreseR As New AgronicaCoreAnagrafeDAL.Imprese_Read
            DT_Impresa = objImpreseR.Leggi_3(xPiva, False, 0, 0, True, False, False, True, False, False, False, False, False, False,
                                 "", "", objParametri_Server)

            'Se il recordset non e' nullo
            If DT_Impresa.Rows.Count <> 0 Then

                TxtPiva.Text = DT_Impresa.Rows(0).Item("Piva")
                TxtRagioneSociale.Text = DT_Impresa.Rows(0).Item("Rag_Soc")

                'txtCuaaCod.Text = DT_Impresa.Rows(0).Item("CUAA")

                '''TxtSuperficie.Text = RsImprese.Fields("Sup_Totale").Value

                If DT_Impresa.Rows(0).Item("Validita_Inizio") = "01/01/1900" Then
                    TxtValiditaInizio.Text = ""
                Else
                    TxtValiditaInizio.Text = DT_Impresa.Rows(0).Item("Validita_Inizio")
                End If

                If DT_Impresa.Rows(0).Item("Validita_Fine") = "31/12/2100" Then
                    TxtValiditaFine.Text = ""
                Else
                    TxtValiditaFine.Text = DT_Impresa.Rows(0).Item("Validita_Fine")
                End If


                'Imposto la posizione nella combo 
                Cmb_FormaGiuridica.SelectedIndex =
                    Cmb_FormaGiuridica.Items.IndexOf(
                        Cmb_FormaGiuridica.Items.FindByValue(
                            DT_Impresa.Rows(0).Item("Forma_Giuridica")))


                Select Case DT_Impresa.Rows(0).Item("TipoImpresaGerarchia")

                    Case 1
                        Me.Opt_TipoImpresa.Checked = True
                        Me.Opt_TipoCooperativa.Checked = False
                        Me.Opt_TipoOP.Checked = False
                        Me.Opt_TipoConsorzio.Checked = False
                        'Me.ImgIcona.ImageUrl = "../AB_Immagini/icone24/x02_Impresa.png"
                    Case 2
                        Me.Opt_TipoImpresa.Checked = False
                        Me.Opt_TipoCooperativa.Checked = True
                        Me.Opt_TipoOP.Checked = False
                        Me.Opt_TipoConsorzio.Checked = False
                        'Me.ImgIcona.ImageUrl = "../AB_Immagini/icone24/cooperativa24c.ico"
                    Case 3
                        Me.Opt_TipoImpresa.Checked = False
                        Me.Opt_TipoCooperativa.Checked = False
                        Me.Opt_TipoOP.Checked = False
                        Me.Opt_TipoConsorzio.Checked = True
                        'Me.ImgIcona.ImageUrl = "../AB_Immagini/icone24/cooperativa24a.ico"
                    Case 4
                        Me.Opt_TipoImpresa.Checked = False
                        Me.Opt_TipoCooperativa.Checked = False
                        Me.Opt_TipoOP.Checked = True
                        Me.Opt_TipoConsorzio.Checked = False
                        'Me.ImgIcona.ImageUrl = "../AB_Immagini/icone24/cooperativa24c.ico"

                End Select


                If Not IsDBNull(DT_Impresa.Rows(0).Item("Tecnico_Riferimento")) AndAlso DT_Impresa.Rows(0).Item("Tecnico_Riferimento") <> "" Then

                    Dt_Contatti = New DataTable
                    Dim objContattiRr As New AgronicaCoreAnagrafeDAL.Contatti_R
                    'Dt_Contatti = objContattiRr.Contatti_Contatto_Leggi(CStr(Session("ASG_SuperUser_CodFiscale")), _
                    '                                                CStr(DT_Impresa.Rows(0).Item("Tecnico_Riferimento")), 0, 0, False, False, 0, 0, False, 0, 0, 0, "", False, 0, 0, 0, 0, 0, _
                    '                                                AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni, _
                    '                                                "", _
                    '                                                "", _
                    '                                                objParametri_Server)

                    Dt_Contatti = objContattiRr.Contatti_Contatto_Leggi(
                                                         objParametri_Server.PivaSuperUser, xPiva, 0, 0, False, False, 0, 0, False, 0, -99, 0, "", True, 0, 0, 0, 0, 0,
                                                         AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                                         "",
                                                         "",
                                                        objParametri_Server)

                    If Dt_Contatti.Rows.Count <> 0 Then
                        CmbTecnico.SelectedIndex =
                            CmbTecnico.Items.IndexOf(CmbTecnico.Items.FindByValue(
                                DT_Impresa.Rows(0).Item("Tecnico_Riferimento")))
                    End If
                    Dt_Contatti = Nothing

                End If

                Dim objImprese_Codici As New AgronicaCoreAnagrafeDAL.Imprese_Codici_Read

                Dim dt_Organismo = objImpreseCodici.Leggi(xPiva, 1205, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)

                If dt_Organismo.Rows.Count > 0 AndAlso dt_Organismo.Rows(0).Item("Val_Cod") <> "" Then

                    CmbOdc.SelectedIndex = CmbOdc.Items.IndexOf(CmbOdc.Items.FindByValue(dt_Organismo.Rows(0).Item("Val_Cod")))

                End If

            End If




            'ViewState("Dt_Padri") = Dt_Padri

            HttpContext.Current.Session("dt_Padri") = Dt_Padri


            Dim objGerarchia As New AgronicaCoreAnagrafeDAL.GerarchiaImprese_R
            Dim TempDt As DataTable
            'Dim ArrayPadri As String()


            'Leggo le imprese padri associate al profilo selezionato
            TempDt = objGerarchia.LeggixFiglio(xPiva,
                                               enumSelezioneVariabile.Selezione_TabellaCompleta,
                                               "", "", objParametri_Server)

            objGerarchia = Nothing


            ' Controllo se la variabile Padri non è in sessione, la aggiungo
            'If (IsNothing(HttpContext.Current.Session("dt_Padri"))) Then
            '    HttpContext.Current.Session("dt_Padri") = TempDt
            'End If


            If TempDt IsNot Nothing AndAlso TempDt.Rows.Count > 0 Then

                For k = 0 To TempDt.Rows.Count - 1

                    'ReDim Preserve ArrayPadri(i)
                    'ArrayPadri(i) = Rs.Fields("padre").Value

                    Padre_Piva = TempDt.Rows(k).Item("padre")
                    'Padre_RagSoc = RagSoc_from_Piva( _
                    '                            Server, Session, Page, _
                    '                            CStr(TempDt.Rows(k).Item("padre")))
                    If Padre_Piva <> "" Then
                        Padre_RagSoc = RagSoc_from_Piva(
                                                objParametri_Server, Session, Page,
                                                CStr(TempDt.Rows(k).Item("padre")))
                        InserisciRiga(Padre_Piva,
                                      Padre_RagSoc,
                                      "01/01/1900",
                                      "31/12/2100")
                    End If

                Next

                'viewstate("Padri") = ArrayPadri

            End If

            TempDt = Nothing

            HttpContext.Current.Session("dt_Padri_origine") = Dt_Padri.Copy

            '----- Tabella Indirizzi

            DTIndirizzi = objIndirizzi.Leggi(CStr(xPiva),
                                                0, 0,
                                                  AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta,
                                                  "",
                                                  "",
                                                  objParametri_Server)

            'Elimino l'oggetto COM+
            objIndirizzi = Nothing

            'Se il recordset non e' nullo
            If DTIndirizzi.Rows.Count > 0 Then

                Txt_CodIndirizzo.Text = DTIndirizzi.Rows(0).Item("cod_indirizzo")

                Txt_Via.Text = DTIndirizzi.Rows(0).Item("ind_des")
                Txt_Frazione.Text = DTIndirizzi.Rows(0).Item("frz_des")
                Txt_CAP.Text = DTIndirizzi.Rows(0).Item("cap")

                Dim Stato As String = "IT"
                If Trim(DTIndirizzi.Rows(0).Item("Stato")) <> "" Then
                    Stato = UCase(Trim(DTIndirizzi.Rows(0).Item("Stato")))
                End If

                Ripristina_Cmb_Stato(Stato)

                Txt_Note.Text = DTIndirizzi.Rows(0).Item("note")

                Me.Txt_ProvinciaSigla.Text = CStr(DTIndirizzi.Rows(0).Item("pro_cod"))

                Me.Txt_ProCodIstat.Text = CStr(DTIndirizzi.Rows(0).Item("pro_cod_istat"))
                Me.Txt_ComCodIstat.Text = CStr(DTIndirizzi.Rows(0).Item("com_cod_istat"))

                Province_and_Comuni(dll_Provincia,
                    ddl_comune,
                    CStr(DTIndirizzi.Rows(0).Item("pro_cod")),
                    CStr(DTIndirizzi.Rows(0).Item("com_cod_istat")),
                    False,
                    1, objParametri_Server,
                    Stato)

                If ddl_comune.Items.Count > 0 Then

                    HttpContext.Current.Session("nome_comune_settato") = ddl_comune.SelectedItem.Text
                    HttpContext.Current.Session("comune_settato") = ddl_comune.SelectedItem.Value

                End If

                'TxtCodProvincia.Text = objParametriAgenda.Particelle(0).Provincia
                'TxtCodComune.Text = objParametriAgenda.Particelle(0).Comune

                'dll_Provincia.SelectedValue = CStr(DTIndirizzi.Rows(0).Item("pro_des"))
                'ddl_comune.SelectedValue = CStr(DTIndirizzi.Rows(0).Item("com_des"))

            End If



            '@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
            '  CONTATTI
            '@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

            'disattivo tutti i controlli del datagrid
            'in fase di modifica dell'impresa non è editabile
            For j = 0 To DataGrid_Contatti.Rows.Count - 1

                CType(DataGrid_Contatti.Rows(j).FindControl("TxtProgressivo"), TextBox).ReadOnly = True
                CType(DataGrid_Contatti.Rows(j).FindControl("TxtAttivita"), TextBox).ReadOnly = True

                CType(DataGrid_Contatti.Rows(j).FindControl("ChkRapporto"), CheckBox).Enabled = False
                Me.ChkVisibilita.Enabled = False
                Me.Btn_VerificaProgressivo.Visible = False


            Next


            '-----------------------------------------------------------

            Dt_Contatti = New DataTable

            Dim objContattiR As New AgronicaCoreAnagrafeDAL.Contatti_R
            Dt_Contatti = objContattiR.Contatti_Contatto_Leggi(
                                                 objParametri_Server.PivaSuperUser, xPiva, 0, 0, False, False, 0, 0, False, 0, -99, 0, "", True, 0, 0, 0, 0, 0,
                                                 AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                                 "",
                                                 "",
                                                objParametri_Server)

            If Dt_Contatti.Rows.Count <> 0 Then

                ViewState("Operazione_Contatti") = enum_TipoOperazioneDB.Modifica

                If CInt(Dt_Contatti.Rows(0).Item("Sa_Cod")) = -1 Then
                    Me.ChkVisibilita.Checked = True
                    Me.Btn_VerificaProgressivo.Visible = True
                End If

                Me.TxtCodiceFiscale.Text = CStr(Dt_Contatti.Rows(0).Item("codice_fiscale"))

                'scorro le risorse umane, ovvero tutti i rapporti contabili attribuiti all'impresa
                For i = 0 To Dt_Contatti.Rows.Count - 1

                    'scorro il datagrid
                    For j = 0 To DataGrid_Contatti.Rows.Count - 1

                        'carico i dati nel datagrid dei contatti e setto il colore del testo a nero, in quanto in modifica
                        'i contatti non sono editabili
                        If DataGrid_Contatti.Rows(j).Cells(3).Text = Dt_Contatti.Rows(i).Item("cod_rapporto") Then

                            CType(DataGrid_Contatti.Rows(j).FindControl("TxtCodRisUm"), TextBox).Text = Dt_Contatti.Rows(i).Item("cod_risum")

                            CType(DataGrid_Contatti.Rows(j).FindControl("TxtProgressivo"), TextBox).Text = Dt_Contatti.Rows(i).Item("settore_des")
                            CType(DataGrid_Contatti.Rows(j).FindControl("TxtProgressivo"), TextBox).CssClass = "testo_08_nero"

                            CType(DataGrid_Contatti.Rows(j).FindControl("TxtAttivita"), TextBox).Text = Dt_Contatti.Rows(i).Item("attivita_des")
                            CType(DataGrid_Contatti.Rows(j).FindControl("TxtAttivita"), TextBox).CssClass = "testo_08_nero"

                            CType(DataGrid_Contatti.Rows(j).FindControl("ChkRapporto"), CheckBox).Checked = True

                        End If


                    Next

                Next

            Else

                'L'IMPRESA NON è PRESENTE NEI CONTATTI, BISOGNA INSERIRLA!!!

                ViewState("Operazione_Contatti") = enum_TipoOperazioneDB.Scrittura


            End If

            Dt_Contatti = Nothing



            ' Else

            ''INSERIMENTO

            'ViewState("Operazione_Contatti") = enum_TipoOperazioneDB.Scrittura

            ''imposto il default sul fornitore
            'For i = 0 To DataGrid_Contatti.Rows.Count - 1

            '    If DataGrid_Contatti.Rows(i).Cells(3).Text = COD_FORNITORE Then

            '        CType(DataGrid_Contatti.Rows(i).FindControl("ChkRapporto"), CheckBox).Checked = True

            '        Exit For

            '    End If

            'Next


        End If

        jsPadre = DT_to_Json_GerarchiaPadre(HttpContext.Current.Session("dt_Padri"))


        If Operazione = enum_TipoOperazioneDB.Lettura Then

            TxtRagioneSociale.Enabled = False
            Cmb_FormaGiuridica.Enabled = False
            TxtPiva.Enabled = False
            TxtCodiceFiscale.Enabled = False
            Txt_Via.Enabled = False
            Txt_Frazione.Enabled = False
            dll_Provincia.Enabled = False
            ddl_comune.Enabled = False
            Txt_CAP.Enabled = False
            'Txt_Stato.Enabled = False
            cmb_Stato.Enabled = False
            Txt_Note.Enabled = False
            ChkVisibilita.Enabled = False
            txtPivaPadre.Enabled = False
            Opt_TipoConsorzio.Enabled = False
            Opt_TipoOP.Enabled = False
            Opt_TipoCooperativa.Enabled = False
            Opt_TipoImpresa.Enabled = False

            CmbTecnico.Enabled = False
            TxtValiditaInizio.Enabled = False
            TxtValiditaFine.Enabled = False
            CmbCodice.Enabled = False
            TxtCodiceValore.Enabled = False
            TxtValiditaInizioCodice.Enabled = False
            TxtValiditaFineCodice.Enabled = False
            CmbOdc.Enabled = False

        End If


    End Sub


    '################################################################################
    Private Sub InserisciRiga(ByVal Piva As String, _
                              ByVal Rag_Soc As String, _
                              ByVal Validita_Inizio As String, _
                              ByVal Validita_Fine As String)

        Try

            Dim Dt As New DataTable
            Dim Dr As DataRow

            'If (IsNothing(HttpContext.Current.Session("dt_Padri"))) Then
            ' Dt = ViewState("Dt_Padri")
            Dt = HttpContext.Current.Session("dt_Padri")

            'Creo una nuova riga
            Dr = Dt.NewRow

            'Definisco i valori
            Dr.Item("Piva") = Piva
            Dr.Item("Rag_Soc") = Rag_Soc

            If Validita_Inizio <> "01/01/1900" Then
                Dr.Item("Validita_Inizio") = Validita_Inizio
            Else
                Dr.Item("Validita_Inizio") = "..."
            End If
            If Validita_Fine <> "31/12/2100" Then
                Dr.Item("Validita_Fine") = Validita_Fine
            Else
                Dr.Item("Validita_Fine") = "..."
            End If

            'Associo alla tabella la nuova riga creata
            Dt.Rows.Add(Dr)

            'ViewState("Dt_Padri") = Dt
            HttpContext.Current.Session("dt_Padri") = Dt
            ''Else
            ' Dt = HttpContext.Current.Session("dt_Padri")
            ' End If



            'If (IsNothing(HttpContext.Current.Session("dt_Padri"))) Then
            '    HttpContext.Current.Session("dt_Padri") = Dt
            'End If

            'jsPadre = DT_to_Json_GerarchiaPadre(Dt)


            'GridView_Padri.DataSource = Dt
            'GridView_Padri.DataBind()


        Catch ex As Exception

            Dim pippo As String = ex.Message

            AgroMsgBox(ex.Message, Page)

        End Try
    End Sub



    '######################################################################################################
    Private Sub CaricaGriglia_Contatti()


        '----- Definizione delle variabili

        Dim Dt As New DataTable
        Dim Dr As DataRow
        Dim i As Integer

        '----- Definisco la struttura del DataTable

        Dt.Columns.Add(New DataColumn("Piva", GetType(String)))
        Dt.Columns.Add(New DataColumn("Sa_Cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Cod_Rapporto", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Rapporto_Des", GetType(String)))

        '----- Definisco l'insieme di colonne che costituiscono la chiave della tabella

        'Vettore di DataColumn
        Dim DtKeys(2) As DataColumn

        'Valorizzo le celle del vettore
        DtKeys(0) = Dt.Columns("Piva")
        DtKeys(1) = Dt.Columns("Sa_Cod")
        DtKeys(2) = Dt.Columns("Cod_Rapporto")

        'Assegno il vettore delle chiavi al DataTable
        Dt.PrimaryKey = DtKeys

        Dim DT_RappCont As DataTable

        Dim objRappContabiliR As New AgronicaCoreAnagrafeDAL.Rapporti_Contabili_R

        DT_RappCont = objRappContabiliR.Contatti_RapportiContabili_Leggi(SACOD_CONTATTO_NONDEFINITO, _
                                                                            0, _
                                                                            False, False, False, False, False, False, False, _
                                                                            AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, _
                                                                            " (Sa_Cod = 0 OR Sa_Cod = 1)", _
                                                                            "", _
                                                                            objParametri_Server)


        If DT_RappCont.Rows.Count <> 0 Then

            For i = 0 To DT_RappCont.Rows.Count - 1

                'Creo una nuova riga
                Dr = Dt.NewRow

                'Definisco i valori
                Dr.Item("Piva") = DT_RappCont.Rows(i).Item("Piva")
                Dr.Item("Sa_Cod") = DT_RappCont.Rows(i).Item("Sa_Cod")
                Dr.Item("Cod_Rapporto") = DT_RappCont.Rows(i).Item("Cod_Rapporto")
                Dr.Item("Rapporto_Des") = DT_RappCont.Rows(i).Item("Rapporto_Des")

                'Associo alla tabella la nuova riga creata
                Dt.Rows.Add(Dr)

            Next


        End If

        '----- Associo il DataTable con la DataGrid

        DataGrid_Contatti.DataSource = Dt
        DataGrid_Contatti.DataBind()

    End Sub

    '###############################################################################################
    Private Sub Btn_VerificaProgressivo_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_VerificaProgressivo.Click
        Verifica_Progressivo()
    End Sub
    '###############################################################################################
    Private Sub Verifica_Progressivo()

        If Not IsNothing(Me.DataGrid_Contatti) AndAlso DataGrid_Contatti.Rows.Count > 0 Then

            Dim i As Integer
            Dim Progressivo As String
            Dim Log As String = ""
            Dim objRisUm As New AgronicaCoreAnagrafeDAL.Risorse_Umane_R
            Dim flag_esiste As Boolean
            Dim Piva As String = ""
            Dim cod_contatto As String = ""
            Dim rag_soc As String = ""

            'escludo se stesso per il caso di modifica azienda e per ilc aso di inseriemnto nuova azienda con contatto già esistente 
            '(già gestito nel salvataggio)
            Dim filtro As String = " NOT ( Contatti.piva = '" & Agro_SQL_SaveText(objParametri_Server.PivaSuperUser) & "'  AND contatti.cod_contatto = '" & Agro_SQL_SaveText(TxtPiva.Text) & "' ) "


            For i = 0 To DataGrid_Contatti.Rows.Count - 1

                flag_esiste = False

                Progressivo = CType(DataGrid_Contatti.Rows(i).FindControl("TxtProgressivo"), TextBox).Text

                If Trim(Progressivo) <> "" Then
                    flag_esiste = objRisUm.Esiste_SettoreDes_RitornaDati(Progressivo, _
                                                                            filtro, _
                                                                            Piva, _
                                                                            cod_contatto, _
                                                                            rag_soc, _
                                                                            objParametri_Server)

                    If flag_esiste Then
                        Log &= String.Format(DirectCast(GetLocalResourceObject("ProgressivoNEsisteGiàInAnagraficaContattiAssociatoA"), String), Progressivo, rag_soc)
                    End If

                End If

            Next

            If Log <> "" Then
                AgroMsgBox(Log, Page)
            Else
                AgroMsgBox(DirectCast(GetLocalResourceObject("IProgressiviSpecificatiNonSonoUtilizzatiDaAltriContatti"), String), Page)
            End If

        End If

    End Sub

    '###############################################################################################
    Private Function Genera_StringaXML_Contatti(ByVal Piva As String,
                                                ByVal Rag_Soc As String,
                                                ByVal Codice_Fiscale As String,
                                                ByVal Validita_Inizio As Date,
                                                ByVal Validita_Fine As Date,
                                                ByVal ID_CF As enum_Contatti_IdCf) As String

        Dim StrXmlContatti As String = ""

        Operazione_Contatti = ViewState("Operazione_Contatti")


        Dim XmlDoc As New System.Xml.XmlDocument
        Dim XmlDatiContatti As System.Xml.XmlElement
        Dim XmlContatto As System.Xml.XmlElement

        Dim trovato As Boolean = False
        Dim StrRapCon As String

        Dim BaseCode As Integer
        Dim TopCode As Integer

        Dim i, visibilita, Cod_Rapporto As Integer

        If Me.ChkVisibilita.Checked = True Then
            visibilita = -1
        Else
            visibilita = 0
        End If

        'calcolo il basecode e il topcode
        Calcola_BaseCode_TopCode(BaseCode, TopCode, Session("ASG_ProgressivoGIAS"))



        If Piva = CStr(Session("ASG_SuperUser_CodFiscale")) Then
            Cod_Rapporto = COD_CLIENTE
            visibilita = PUBBLICO
        Else
            Cod_Rapporto = COD_FORNITORE
        End If


        If Operazione_Contatti = enum_TipoOperazioneDB.Scrittura Then


            Dim objCont As New AgronicaCoreAnagrafeDAL.Contatti_R
            Dim flag_esiste As Boolean

            flag_esiste = objCont.Esiste_Contatto(objParametri_Server.PivaSuperUser,
                                                  Piva,
                                                  enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                                  "", "",
                                                  objParametri_Server)

            If flag_esiste Then
                'il contatto esiste già, vado in modifica

                ' MODIFICARE IL CONTATTO

                XmlDatiContatti = XmlDoc.CreateElement("DatiContatti")

                XmlDatiContatti.InnerXml = XML_Contatto(enum_TipoOperazioneDB.Modifica,
                                                        Session("ASG_SuperUser_CodFiscale"),
                                                        visibilita,
                                                        Piva,
                                                        Rag_Soc,
                                                        ID_CF,
                                                        Codice_Fiscale,
                                                        "Spett.le",
                                                        Validita_Inizio,
                                                        Validita_Fine,
                                                        BaseCode,
                                                        TopCode)

                XmlContatto = XmlDatiContatti.SelectSingleNode("Contatto")

                StrRapCon = ""

                Dim objRisum As New AgronicaCoreAnagrafeDAL.Risorse_Umane_R
                Dim esiste_risum As Boolean
                Dim cod_risum As Integer = 0

                If DataGrid_Contatti.Rows.Count > 0 Then

                    For i = 0 To DataGrid_Contatti.Rows.Count - 1

                        esiste_risum = False

                        If CType(DataGrid_Contatti.Rows(i).FindControl("ChkRapporto"), CheckBox).Checked = True Then

                            Cod_Rapporto = DataGrid_Contatti.Rows(i).Cells(3).Text

                            esiste_risum = objRisum.Esiste_RisorsaUmana2(objParametri_Server.PivaSuperUser,
                                                                        Piva,
                                                                        Cod_Rapporto,
                                                                        "",
                                                                        cod_risum,
                                                                        objParametri_Server)

                            If esiste_risum Then
                                'modifica risorsa umana
                                StrRapCon &= XML_RapportoContabileXRisorseUmane(enum_TipoOperazioneDB.Modifica,
                                                                                Session("ASG_SuperUser_CodFiscale"),
                                                                                visibilita,
                                                                                cod_risum,
                                                                                Piva,
                                                                                DataGrid_Contatti.Rows(i).Cells(3).Text,
                                                                                CType(DataGrid_Contatti.Rows(i).FindControl("TxtAttivita"), TextBox).Text,
                                                                                CType(DataGrid_Contatti.Rows(i).FindControl("TxtProgressivo"), TextBox).Text,
                                                                                ,
                                                                                ,
                                                                                ,
                                                                                ,
                                                                                ,
                                                                                ,
                                                                                ,
                                                                                Validita_Inizio,
                                                                                Validita_Fine)

                            Else
                                'non esiste, inserisco il rapporto contabile
                                StrRapCon &= XML_RapportoContabileXRisorseUmane(enum_TipoOperazioneDB.Scrittura,
                                                                                                    Session("ASG_SuperUser_CodFiscale"),
                                                                                                    visibilita,
                                                                                                    0,
                                                                                                    Piva,
                                                                                                    DataGrid_Contatti.Rows(i).Cells(3).Text,
                                                                                                    CType(DataGrid_Contatti.Rows(i).FindControl("TxtAttivita"), TextBox).Text,
                                                                                                    CType(DataGrid_Contatti.Rows(i).FindControl("TxtProgressivo"), TextBox).Text,
                                                                                                    ,
                                                                                                    ,
                                                                                                    ,
                                                                                                    ,
                                                                                                    ,
                                                                                                    ,
                                                                                                    ,
                                                                                                    Validita_Inizio,
                                                                                                    Validita_Fine)
                            End If

                        End If

                    Next

                    XmlContatto.InnerXml = StrRapCon

                End If

                XmlDoc.AppendChild(XmlDatiContatti)

                StrXmlContatti = XmlDoc.OuterXml

            Else
                'il contatto non esiste


                'CASO DI INSERIMENTO IMPRESA ---> INSERIRE IL CONTATTO CON I RELATIVI RAPPORTI CONTABILI

                XmlDatiContatti = XmlDoc.CreateElement("DatiContatti")

                XmlDatiContatti.InnerXml = XML_Contatto(enum_TipoOperazioneDB.Scrittura,
                                                        Session("ASG_SuperUser_CodFiscale"),
                                                        visibilita,
                                                        Piva,
                                                        Rag_Soc,
                                                        ID_CF,
                                                        Codice_Fiscale,
                                                        "Spett.le",
                                                        Validita_Inizio,
                                                        Validita_Fine,
                                                        BaseCode,
                                                        TopCode)


                XmlContatto = XmlDatiContatti.SelectSingleNode("Contatto")

                StrRapCon = ""

                For i = 0 To DataGrid_Contatti.Rows.Count - 1

                    If CType(DataGrid_Contatti.Rows(i).FindControl("ChkRapporto"), CheckBox).Checked = True Then

                        trovato = True

                        StrRapCon &= XML_RapportoContabileXRisorseUmane(enum_TipoOperazioneDB.Scrittura,
                                                                        Session("ASG_SuperUser_CodFiscale"),
                                                                        visibilita,
                                                                        0,
                                                                        Piva,
                                                                        DataGrid_Contatti.Rows(i).Cells(3).Text,
                                                                        CType(DataGrid_Contatti.Rows(i).FindControl("TxtAttivita"), TextBox).Text,
                                                                        CType(DataGrid_Contatti.Rows(i).FindControl("TxtProgressivo"), TextBox).Text,
                                                                        ,
                                                                        ,
                                                                        ,
                                                                        ,
                                                                        ,
                                                                        ,
                                                                        ,
                                                                        Validita_Inizio,
                                                                        Validita_Fine)


                    End If

                Next

                If Not trovato Then

                    'è il caso di modifica di un'impresa che però non è presente nei contatti

                    If Piva = CStr(Session("ASG_SuperUser_CodFiscale")) Then

                        'superuser

                        StrRapCon &= XML_RapportoContabileXRisorseUmane(enum_TipoOperazioneDB.Scrittura,
                                                                                     Session("ASG_SuperUser_CodFiscale"),
                                                                                     visibilita,
                                                                                     0,
                                                                                     Piva,
                                                                                     COD_CLIENTE,
                                                                                     ,
                                                                                     ,
                                                                                     ,
                                                                                     ,
                                                                                     ,
                                                                                     ,
                                                                                     ,
                                                                                     ,
                                                                                     ,
                                                                                     Validita_Inizio,
                                                                                     Validita_Fine)

                        StrRapCon &= XML_RapportoContabileXRisorseUmane(enum_TipoOperazioneDB.Scrittura,
                                                                               Session("ASG_SuperUser_CodFiscale"),
                                                                               visibilita,
                                                                               0,
                                                                               Piva,
                                                                               COD_FORNITORE,
                                                                               ,
                                                                               ,
                                                                               ,
                                                                               ,
                                                                               ,
                                                                               ,
                                                                               ,
                                                                               ,
                                                                               ,
                                                                               Validita_Inizio,
                                                                               Validita_Fine)

                    Else
                        'impresa normale

                        StrRapCon &= XML_RapportoContabileXRisorseUmane(enum_TipoOperazioneDB.Scrittura,
                                                                                     Session("ASG_SuperUser_CodFiscale"),
                                                                                     visibilita,
                                                                                     0,
                                                                                     Piva,
                                                                                     Cod_Rapporto,
                                                                                     ,
                                                                                     ,
                                                                                     ,
                                                                                     ,
                                                                                     ,
                                                                                     ,
                                                                                     ,
                                                                                     ,
                                                                                     ,
                                                                                     Validita_Inizio,
                                                                                     Validita_Fine)

                    End If

                End If

                XmlContatto.InnerXml = StrRapCon

                XmlDoc.AppendChild(XmlDatiContatti)

                StrXmlContatti = XmlDoc.OuterXml

            End If 'esiste contatto

        Else

            'CASO DI MODIFICA IMPRESA ---> MODIFICARE SOLO IL CONTATTO

            XmlDatiContatti = XmlDoc.CreateElement("DatiContatti")

            XmlDatiContatti.InnerXml = XML_Contatto(enum_TipoOperazioneDB.Modifica,
                                                    Session("ASG_SuperUser_CodFiscale"),
                                                    visibilita,
                                                    Piva,
                                                    Rag_Soc,
                                                    1,
                                                    Codice_Fiscale,
                                                    "Spett.le",
                                                    Validita_Inizio,
                                                    Validita_Fine,
                                                    BaseCode,
                                                    TopCode)


            XmlDoc.AppendChild(XmlDatiContatti)

            StrXmlContatti = XmlDoc.OuterXml

        End If

        Return StrXmlContatti

    End Function


    ''########################################################################################
    'Private Sub ImgBtn_Codice_Ins_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImgBtn_Codice_Ins.Click

    '    Dim Testo As String
    '    Dim Testo2 As String
    '    Dim Testo3 As String
    '    Dim Codice As Integer
    '    Dim Indice As Integer
    '    Dim Valore As String

    '    If CmbCodice.SelectedIndex < 1 Then
    '        AgroMsgBox("Selezionare un codice !!!", Page)
    '        Exit Sub
    '    End If

    '    If TxtCodiceValore.Text = "" Then
    '        AgroMsgBox("Non e' ammesso un valore nullo per i Codici Anagrafici !!!", Page)
    '        Exit Sub
    '    End If

    '    'Verifico che non esista gia' l'elemento
    '    For Indice = 0 To ListCodici.Items.Count - 1

    '        Testo = ListCodici.Items(Indice).Value

    '        If CmbCodice.SelectedItem.Value = Testo Then
    '            AgroMsgBox("Il codice è già stato risulta essere già presente !!!", Page)
    '            Exit Sub
    '        End If
    '    Next

    '    'Inserisco l'elemento nella Listbox
    '    Testo = CmbCodice.SelectedItem.Text
    '    Testo2 = Testo & " = " & TxtCodiceValore.Text

    '    Valore = CmbCodice.SelectedItem.Value

    '    ListCodici.Items.Add(New ListItem(Testo2, Valore))

    '    Me.CmbCodice.SelectedIndex = 0
    '    Me.TxtCodiceValore.Text = ""



    'End Sub




    ''########################################################################################
    'Private Sub ImgBtn_Codice_Canc_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImgBtn_Codice_Canc.Click

    '    'Elimino l'oggetto selezionato nella listbox..
    '    If Me.ListCodici.SelectedIndex <> -1 Then

    '        Me.ListCodici.Items.RemoveAt(ListCodici.SelectedIndex)
    '        TxtCodiceValore.Text = ""
    '        'LblMsgCodici2.Text = ""

    '    End If

    'End Sub




    '########################################################################################
    'Private Sub ImgBtn_Cerca_RagSoc_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImgBtn_Cerca_RagSoc.Click

    '    'Salva_Tutto()
    '    Carica_Select_Padri2()

    'End Sub


    'Private Sub Carica_Select_Padri2()

    '    'Dim cmb_imprese2 As New DropDownList

    '    AgronicaCoreUtility.CaricaListControl.ImpreseConFiltroUtente(Cmb_Imprese, True, "SELEZIONA", "", _
    '                                                                     " Imprese.rag_soc LIKE '%" & txtPivaPadre.text & "%'", " ORDER BY Rag_Soc asc", _
    '                                                                     HttpContext.Current.Session("ASG_objParametri_Server"), _
    '                                                                     HttpContext.Current.Session("ASG_objParametri_Utenti"))
    '    'Dim rval As String = ""
    '    'For Each itm As ListItem In cmb_imprese2.Items
    '    '    rval &= "<option value=""" & itm.Value & """>" & itm.Text & "</option>"
    '    'Next


    'End Sub



    '########################################################################################
    Private Sub ImgBtn_SalvaTutto_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImgBtn_SalvaTutto.Click

        Salva_Tutto()

    End Sub

    '########################################################################################
    Private Sub Salva_Tutto()

        Try

            Dim StrIndirizzo As String
            Dim StrCodice As String
            Dim StrCodici As String
            Dim StrImpresa As String
            Dim StrDatiContatti As String
            Dim StrXmlInserisci As String
            Dim StrXmlCancella As String
            Dim StrGerarchie As String
            Dim StrGerarchieI As String

            Dim Operazione As enum_TipoOperazioneDB
            Dim Indice As Integer

            Dim TipoImpresaGerarchia As Integer
            Dim i As Integer

            Dim BaseCode As Integer
            Dim TopCode As Integer

            Dim TipoOperazioneDB As enum_TipoOperazioneDB
            Dim Tipo_Indirizzo As Integer
            Dim Cod_Indirizzo As Integer
            Dim Ind_Des As String
            Dim Frz_Des As String
            Dim CAP As String
            Dim Com_Des As String
            Dim Pro_Cod As String
            Dim Stato As String
            Dim Note As String
            Dim Pro_Cod_Istat As String
            Dim Com_Cod_Istat As String
            Dim Validita_Inizio As Date
            Dim Validita_Fine As Date

            Dim Id_Cod As Integer
            Dim Val_Cod As String

            Dim Piva, Codice_Fiscale As String
            Dim Rag_Soc As String
            Dim Delega As String
            Dim AT_Prevalente As String
            Dim Forma_Giuridica As String
            Dim Forma_Conduzione As String
            Dim Sup_Totale As Double

            Dim XmlDoc As New System.Xml.XmlDocument
            Dim XmlDoc2 As New System.Xml.XmlDocument
            Dim XmlDatiImprese As System.Xml.XmlElement
            Dim XmlImpresa As System.Xml.XmlElement

            Dim objImpresa As New AgronicaCoreAnagrafeBIZ.Impresa_W
            Dim StrDummy As String

            Dim NoteLog As String = NOTELOG_ANAGRAFE_BOOTSTRAP

            '------------------------------------------------
            '----- Verifico l'operazione richiesta (Inserimento / Modifica / Cancellazione)
            '------------------------------------------------

            'Recupero l'operazione richiesta, dalla querystring
            Operazione = objParametriAgenda.Tipo_Operazione  'CInt(Qs_Operazione)


            '------------------------------------------------
            '----- Calcolo i valori di BaseCode e TopCode
            '------------------------------------------------

            Calcola_BaseCode_TopCode(BaseCode, TopCode, Session("ASG_ProgressivoGIAS"))


            '------------------------------------------------
            '----- Costruisco la stringa XML del INDIRIZZO
            '------------------------------------------------

            'Definisco il tipo di operazione da eseguire
            TipoOperazioneDB = Operazione

            'Prelevo le informazioni immediate
            Tipo_Indirizzo = 1
            Cod_Indirizzo = Txt_CodIndirizzo.Text
            Ind_Des = Txt_Via.Text


            'Lettura Gestione_Gerarchia_Geografica
            Dim DT_Nazioni As DataTable
            Dim objNazioni As New AgronicaCoreMetaSchemaDAL.ACCDAA_ANAG_T005_TabellaCodiciPaesiTerziISO3166_R
            DT_Nazioni = objNazioni.Leggi(cmb_Stato.SelectedItem.Value, "", "Descrizione", objParametri_Server)

            If CInt(DT_Nazioni(0).Item("Gestione_Gerarchia_Geografica")) = 1 Then

                Frz_Des = Txt_Frazione.Text
                CAP = Txt_CAP.Text
                '
                Pro_Cod = Me.Txt_ProvinciaSigla.Text
                Pro_Cod_Istat = Me.Txt_ProCodIstat.Text

                If HttpContext.Current.Session("com") = Nothing Then
                    Set_Comune(ddl_comune.SelectedItem.Value, ddl_comune.SelectedItem.Text)
                End If

                Dim com_codice As String
                com_codice = HttpContext.Current.Session("com")
                Com_Des = HttpContext.Current.Session("nome_comune_settato")
                Com_Cod_Istat = com_codice

                    If CAP.Length > 5 Then
                        Throw New Exception("Superato il limite di lunghezza del CAP")
                    End If

                Else
                    Frz_Des = Txt_Frazione.Text
                CAP = Txt_CAP.Text
                'Com_Des = ddl_comune.SelectedItem.Text
                Com_Des = ""
                Pro_Cod = "00"
                Pro_Cod_Istat = "000"
                Com_Cod_Istat = "000"
            End If

            'Stato = Txt_Stato.Text
            Stato = cmb_Stato.SelectedItem.Value
            Note = Txt_Note.Text

            'Com_Cod_Istat = Me.Txt_ComCodIstat.Text

            '--- Periodo Attivita'
            If Not IsDate(TxtValiditaInizio.Text) Then
                Validita_Inizio = AGRODATAINIZIO
            Else
                Validita_Inizio = CDate(TxtValiditaInizio.Text)
            End If

            If Not IsDate(TxtValiditaFine.Text) Then
                Validita_Fine = AGRODATAFINE
            Else
                Validita_Fine = CDate(TxtValiditaFine.Text)
            End If



            'Genero la stringa XML
            Call XML_Indirizzo(enum_CodificaDecodifica.Codifica,
                            StrIndirizzo,
                            TipoOperazioneDB,
                            Tipo_Indirizzo,
                            Cod_Indirizzo,
                            Ind_Des,
                            Frz_Des,
                            CAP,
                            Com_Des,
                            Pro_Cod,
                            Stato,
                            Note,
                            Pro_Cod_Istat,
                            Com_Cod_Istat,
                            AGRODATAINIZIO,
                            AGRODATAFINE,
                            BaseCode,
                            TopCode)


            '------------------------------------------------
            '----- Costruisco la stringa XML dei CODICI
            '------------------------------------------------

            'Azzero la stringa complessiva dei codici
            StrCodici = ""



            '----- CUAA codice

            'Recupero le informazioni
            'non c'è nessun controllo sulla valorizzazione del campo xchè fatti lato client...
            'vedi l'on_blur del controllo....
            TipoOperazioneDB = enum_TipoOperazioneDB.Scrittura
            Id_Cod = enum_CodiciAnagrafe.CodiceCUAA
            'Val_Cod = Me.txtCuaaCod.Text
            Val_Cod = Me.TxtCodiceFiscale.Text

            'Genero l'XML del singolo nodo
            Call XML_Codice(enum_CodificaDecodifica.Codifica,
                            StrCodice,
                            TipoOperazioneDB,
                            Id_Cod,
                            Val_Cod,
                            AGRODATAINIZIO,
                            AGRODATAFINE,
                            BaseCode,
                            TopCode)

            'Inserisco l'XML nella stringa complessiva
            StrCodici &= StrCodice

            '----- Titolo Possesso

            'If Me.CmbTecnico.SelectedItem.Value <> "0" Then

            '    Id_Cod = enum_CodiciAnagrafe.TitoloPossesso
            '    Val_Cod = Me.CmbTecnico.SelectedItem.Value

            '    'Genero l'XML del singolo nodo
            '    Call XML_Codice(enum_CodificaDecodifica.Codifica, _
            '                    StrCodice, _
            '                    TipoOperazioneDB, _
            '                    Id_Cod, _
            '                    Val_Cod, _
            '                    AGRODATAINIZIO, _
            '                    AGRODATAFINE, _
            '                    BaseCode, _
            '                    TopCode)

            '    'Inserisco l'XML nella stringa complessiva
            '    StrCodici &= StrCodice

            'End If

            '----- Tecnico

            If Me.CmbTecnico.SelectedItem.Value <> "0" Then

                Id_Cod = enum_CodiciAnagrafe.Tecnico
                Val_Cod = Me.CmbTecnico.SelectedItem.Value

                'Genero l'XML del singolo nodo
                Call XML_Codice(enum_CodificaDecodifica.Codifica,
                                StrCodice,
                                TipoOperazioneDB,
                                Id_Cod,
                                Val_Cod,
                            AGRODATAINIZIO,
                            AGRODATAFINE,
                                BaseCode,
                                TopCode)

                'Inserisco l'XML nella stringa complessiva
                StrCodici &= StrCodice

            End If

            If Me.CmbOdc.SelectedItem.Value <> "0" Then

                Id_Cod = enum_CodiciAnagrafe.Organismo_di_Controllo
                Val_Cod = Me.CmbOdc.SelectedItem.Value

                'Genero l'XML del singolo nodo
                Call XML_Codice(enum_CodificaDecodifica.Codifica,
                                StrCodice,
                                TipoOperazioneDB,
                                Id_Cod,
                                Val_Cod,
                            AGRODATAINIZIO,
                            AGRODATAFINE,
                                BaseCode,
                                TopCode)

                'Inserisco l'XML nella stringa complessiva
                StrCodici &= StrCodice

            End If


            ''----- Leggo l'elenco delle CERTIFICAZIONI

            'For Indice = 0 To ListCertificazioni.Items.Count - 1

            '    'Recupero le informazioni
            '    TipoOperazioneDB = enum_TipoOperazioneDB.Scrittura
            '    Id_Cod = ListCertificazioni.Items(Indice).Value
            '    Val_Cod = "0"

            '    'Genero l'XML del singolo nodo
            '    Call XML_Codice(enum_CodificaDecodifica.Codifica, _
            '                    StrCodice, _
            '                    TipoOperazioneDB, _
            '                    Id_Cod, _
            '                    Val_Cod, _
            '                    AGRODATAINIZIO, _
            '                    AGRODATAFINE, _
            '                    BaseCode, _
            '                    TopCode)

            '    'Inserisco l'XML nella stringa complessiva
            '    StrCodici = StrCodici & StrCodice

            'Next

            ''----- Leggo l'elenco dei CODICI ANAGRAFICI

            ' Controllo che abbia almeno un Menu padre
            Dim Dt_Codici As New DataTable
            Dim v_ini As Date
            Dim v_fine As Date

            If (Not IsNothing(HttpContext.Current.Session("dt_Codici"))) Then
                Dt_Codici = HttpContext.Current.Session("dt_Codici")

                For Indice = 0 To Dt_Codici.Rows.Count - 1
                    'Recupero le informazioni
                    Id_Cod = CInt(Dt_Codici.Rows(Indice).Item("Id_Cod"))
                    Val_Cod = Dt_Codici.Rows(Indice).Item("Val_Cod")



                    If (Dt_Codici.Rows(Indice).Item("Validita_Inizio")) = "..." Then
                        v_ini = AGRODATAINIZIO
                    Else
                        v_ini = CDate(Dt_Codici.Rows(Indice).Item("Validita_Inizio"))
                    End If

                    If (Dt_Codici.Rows(Indice).Item("Validita_Fine")) = "..." Then
                        v_fine = AGRODATAFINE
                    Else
                        v_fine = CDate(Dt_Codici.Rows(Indice).Item("Validita_Fine"))
                    End If

                    'Genero l'XML del singolo nodo
                    Call XML_Codice(enum_CodificaDecodifica.Codifica,
                                    StrCodice,
                                    TipoOperazioneDB,
                                    Id_Cod,
                                    Val_Cod,
                                v_ini,
                                v_fine,
                                    BaseCode,
                                    TopCode)

                    'Inserisco l'XML nella stringa complessiva
                    StrCodici &= StrCodice
                Next


            End If

            '------------------------------------------------
            '----- Costruisco la stringa XML dell'IMPRESA
            '------------------------------------------------

            'Definisco il tipo di operazione da eseguire
            TipoOperazioneDB = Operazione

            'Recupero le altre informazioni
            Piva = TxtPiva.Text
            Rag_Soc = TxtRagioneSociale.Text
            Codice_Fiscale = Me.TxtCodiceFiscale.Text
            Delega = ""
            AT_Prevalente = ""
            Forma_Giuridica = ""
            Forma_Conduzione = ""
            Sup_Totale = 0      'TxtSuperficie.Text

            '-----

            If Stato = "IT" AndAlso (Piva.Length <> 11 AndAlso Piva.Length <> 16) Then
                Throw New Exception("Inserire una partita IVA valida")
            End If

            If Me.Opt_TipoImpresa.Checked Then
                TipoImpresaGerarchia = 1
            End If

            If Me.Opt_TipoCooperativa.Checked Then
                TipoImpresaGerarchia = 2
            End If

            If Me.Opt_TipoConsorzio.Checked Then
                TipoImpresaGerarchia = 3
            End If

            If Me.Opt_TipoOP.Checked Then
                TipoImpresaGerarchia = 4
            End If

            '-----



            ' @Paolo: ciclare per tutti i padri inseriti in Gerarchia e inserire la stessa impresa 
            Dim Dt_Padri As DataTable
            'Dt_Padri = ViewState("Dt_Padri")
            Dt_Padri = HttpContext.Current.Session("dt_Padri")
            Dim Piva_Padre As String = ""
            If Dt_Padri.Rows.Count > 0 Then
                Piva_Padre = Dt_Padri.Rows(0).Item("Piva")
            End If

            If Piva <> objParametri_Server.PivaSuperUser AndAlso Dt_Padri.Rows.Count = 0 Then
                Piva_Padre = objParametri_Server.PivaSuperUser
            End If

            Forma_Giuridica = Cmb_FormaGiuridica.SelectedValue

            'Genero l'XML
            Call XML_Impresa(enum_CodificaDecodifica.Codifica,
                            StrImpresa,
                            TipoOperazioneDB,
                            Piva,
                            Rag_Soc,
                            Delega,
                            AT_Prevalente,
                            Forma_Giuridica,
                            Forma_Conduzione,
                            Sup_Totale,
                            Piva_Padre,
                            TipoImpresaGerarchia,
                            Validita_Inizio,
                            Validita_Fine,
                            BaseCode,
                            TopCode)

            'Call XML_Impresa(enum_CodificaDecodifica.Codifica, _
            '               StrImpresa, _
            '               TipoOperazioneDB, _
            '               Piva, _
            '               Rag_Soc, _
            '               Delega, _
            '               AT_Prevalente, _
            '               Forma_Giuridica, _
            '               Forma_Conduzione, _
            '               Sup_Totale, _
            '               "NON_IMPOSTATO", _
            '               TipoImpresaGerarchia, _
            '               Validita_Inizio, _
            '               Validita_Fine, _
            '               BaseCode, _
            '               TopCode)

            '------------------------------------------------ 
            '----- Costruisco la stringa XML complessiva di inserimento
            '------------------------------------------------

            'Creo il nodo "DatiImprese"
            XmlDatiImprese = XmlDoc.CreateElement("DatiImprese")

            'Inserisco il nodo "Impresa"
            XmlDatiImprese.InnerXml = StrImpresa

            'Rendo l'albero figlio del documento
            XmlDoc.AppendChild(XmlDatiImprese)

            'Faccio una copia del documento XML
            XmlDoc2 = XmlDoc

            'Seleziono il nodo "Impresa"
            XmlImpresa = XmlDoc.SelectSingleNode("//Impresa")



            'se sto creando una nuova impresa allora aggiungo l'xml del contatto
            If Operazione = enum_TipoOperazioneDB.Scrittura Then

                Dim id_cf = Ottieni_ID_CF_Contatto(Stato)
                StrDatiContatti = Genera_StringaXML_Contatti(Piva, Rag_Soc, Codice_Fiscale, Validita_Inizio, Validita_Fine, id_cf)

            Else

                'PRIMA IN MODIFICA NON SI SCRIVEVA IL CONTATTO
                'ORA LO FACCIAMO SCRIVERE PERCHE' BISOGNA SALVARE
                'IL CODICE FISCALE!!!
                'StrDatiContatti = ""

                Dim XmlDoc3 As New System.Xml.XmlDocument
                Dim XmlGerarchia As System.Xml.XmlElement
                Dim StrGerarchia_singola As String = ""
                XmlGerarchia = XmlDoc3.CreateElement("Gerarchia")
                Dim id_cf = Ottieni_ID_CF_Contatto(Stato)
                StrDatiContatti = Genera_StringaXML_Contatti(Piva, Rag_Soc, Codice_Fiscale, Validita_Inizio, Validita_Fine, id_cf)

                StrGerarchieI = ""

                Dim objGerarchia As New AgronicaCoreXML.XML_Anagrafe
                If Dt_Padri.Rows.Count > 0 Then
                    For i = 0 To Dt_Padri.Rows.Count - 1

                        If (Dt_Padri.Rows(i).Item("Validita_Inizio")) = "..." Then
                            Dt_Padri.Rows(i).Item("Validita_Inizio") = AGRODATAINIZIO
                        End If

                        If (Dt_Padri.Rows(i).Item("Validita_Fine")) = "..." Then
                            Dt_Padri.Rows(i).Item("Validita_Fine") = AGRODATAFINE
                        End If


                        objGerarchia.XML_GerarchiaImprese(enum_TipoOperazioneDB.Scrittura,
                                                        Dt_Padri.Rows(i).Item("Piva"),
                                                         StrGerarchia_singola,
                        CDate(Dt_Padri.Rows(i).Item("Validita_Inizio")),
                        CDate(Dt_Padri.Rows(i).Item("Validita_Fine")))

                        ''Imposto XmlIndirizzo come figlio del documento principale
                        'XmlDoc3.AppendChild(XmlDatiGerarchia)
                        StrGerarchieI &= StrGerarchia_singola
                    Next
                Else
                    If Piva <> objParametri_Server.PivaSuperUser AndAlso Dt_Padri.Rows.Count = 0 Then
                        objGerarchia.XML_GerarchiaImprese(enum_TipoOperazioneDB.Scrittura,
                                                          objParametri_Server.PivaSuperUser,
                                                          StrGerarchia_singola,
                                                          AGRODATAINIZIO,
                                                          AGRODATAFINE)
                        StrGerarchieI &= StrGerarchia_singola
                    End If
                End If


            End If




            'UtenteAbilitato_Lettura = objPermessi.Controlla_Permessi_Utente( _
            '                            Session("ASG_Utente_Username"), _
            '                            Session("ASG_IdServizio"), _
            '                            enum_Security_Attivita.Anagrafica_ParticellaCatastale, _
            '                            enum_Security_Operazione.Lettura, _
            '                            Date.Now, _
            '                            "", _
            '                            objParametri_Utenti)

            'StrDatiContatti = Genera_StringaXML_Contatti(Piva, Rag_Soc, Codice_Fiscale, Validita_Inizio, Validita_Fine)

            'Inserisco gli elementi "Indirizzo", "Codice" e "DatiContatti" come figli del nodo "Impresa"
            XmlImpresa.InnerXml = StrIndirizzo & StrCodici & StrDatiContatti

            'Estraggo la stringa XML complessiva
            StrXmlInserisci = XmlDoc.InnerXml


            '------------------------------------------------
            '----- Costruisco la stringa XML complessiva di cancellazione
            '------------------------------------------------



            If Operazione = enum_TipoOperazioneDB.Modifica Then

                If Session("StrXmlCodiciAttuali").ToString <> "" Then

                    'Seleziono il nodo "Impresa"
                    XmlImpresa = XmlDoc2.SelectSingleNode("//Impresa")

                    ''Rendo l'impresa in lettura
                    'XmlImpresa.SetAttribute("TipoOperazioneDB", CStr(enum_TipoOperazioneDB.Lettura))

                    'Inserisco gli elementi "Codice" da cancellare
                    XmlImpresa.InnerXml = Session("StrXmlCodiciAttuali")

                End If

                ' @Paolo: Cancello tutti i padri iniziali (per poi riscrivere i correnti)
                If HttpContext.Current.Session("dt_Padri_origine") IsNot Nothing Then

                    Dim Dt_Padri_ori As DataTable

                    Dt_Padri_ori = HttpContext.Current.Session("dt_Padri_origine")

                    Dim XmlDoc3 As New System.Xml.XmlDocument
                    Dim XmlGerarchia As System.Xml.XmlElement
                    Dim StrGerarchia_singola As String = ""
                    XmlGerarchia = XmlDoc3.CreateElement("Gerarchia")

                    StrGerarchie = ""

                    For i = 0 To Dt_Padri_ori.Rows.Count - 1

                        StrGerarchia_singola = ""

                        If (Dt_Padri_ori.Rows(i).Item("Validita_Inizio")) = "..." Then
                            Dt_Padri_ori.Rows(i).Item("Validita_Inizio") = AGRODATAINIZIO
                        End If

                        If (Dt_Padri_ori.Rows(i).Item("Validita_Fine")) = "..." Then
                            Dt_Padri_ori.Rows(i).Item("Validita_Fine") = AGRODATAFINE
                        End If

                        Dim objGerarchia As New AgronicaCoreXML.XML_Anagrafe
                        objGerarchia.XML_GerarchiaImprese(enum_TipoOperazioneDB.Cancellazione,
                                                        Dt_Padri_ori.Rows(i).Item("Piva"),
                                                         StrGerarchia_singola,
                        CDate(Dt_Padri_ori.Rows(i).Item("Validita_Inizio")),
                        CDate(Dt_Padri_ori.Rows(i).Item("Validita_Fine")))

                        ''Imposto XmlIndirizzo come figlio del documento principale
                        'XmlDoc3.AppendChild(XmlDatiGerarchia)
                        StrGerarchie &= StrGerarchia_singola
                    Next

                    'Seleziono il nodo "Impresa"
                    XmlImpresa = XmlDoc.SelectSingleNode("//Impresa")

                    'Inserisco il nodo "Impresa"
                    XmlImpresa.InnerXml &= StrGerarchie

                End If

                'Estraggo la stringa XML complessiva
                StrXmlCancella = XmlDoc2.InnerXml

            End If


            'Inserisco gli elementi "Indirizzo", "Codice" e "DatiContatti" come figli del nodo "Impresa"
            XmlImpresa.InnerXml = StrGerarchieI & StrDatiContatti & StrCodici & StrIndirizzo

            StrXmlInserisci = XmlDoc.InnerXml

            'Distruggo gli oggetti
            XmlDatiImprese = Nothing
            XmlImpresa = Nothing
            XmlDoc = Nothing
            XmlDoc2 = Nothing


            '=======================
            '===  Aggiornamento  ===
            '=======================
            Dim OUT_Piva As String
            Dim OUT_SaCod As String

            '------------------------------------------------
            '----- apro connessione e transazione
            '------------------------------------------------
            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessione(True, objParametri_Server)
            Dim Errore As Boolean = False


            Try
                'stefania
                'ParoleChiave_Salvatutto(Piva, objParametri_Server)


                '------------------------------------------------
                '----- Se sono in MODIFICA cancello i codici attuali
                '------------------------------------------------

                If Operazione = enum_TipoOperazioneDB.Modifica Then

                    'If Session("StrXmlCodiciAttuali") <> "" Then
                    If StrXmlCancella <> "" Then

                        'Eseguo i comandi XML

                        StrDummy = objImpresa.Impresa_Scrivi(
                                                CStr(StrXmlCancella),
                                                OUT_Piva,
                                                objParametri_Server,
                                                objParametri_Utenti,
                                                NoteLog:=NoteLog)

                        'DEBUG !!!!!
                        '''Throw New Exception("Errore generato da me medesimo !!!")

                    End If

                End If


                '------------------------------------------------
                '----- Modifico o Inserisco l'azienda
                '------------------------------------------------

                'Creo gli oggetti COM+
                '   *   CreateCANCELLATOObject("Agro_Anagrafe.Impresa_W")

                'Eseguo i comandi XML 
                StrDummy = objImpresa.Impresa_Scrivi(
                                            CStr(StrXmlInserisci),
                                                OUT_Piva,
                                                objParametri_Server,
                                                objParametri_Utenti,
                                                NoteLog:=NoteLog)

                'Elimino gli oggetti COM+
                objImpresa = Nothing


                ''§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§

                ''------------------------------------------------
                ''----- Validazione dati inseriti
                ''------------------------------------------------

                ''NOTA
                ''La routine seguenti verifica se l'utente dispone dei permessi
                ''di validazione dei dati.
                ''In caso negativo imposta a (-1) il flag di validazione
                ''dell'impresa e invia un messaggio all'Ufficio Segreteria Tecnica

                'Call Gestione_Validazione_Dati(Operazione, _
                '                                Piva, _
                '                                AgroLabel_Impresa & " : " _
                '                                    & Rag_Soc & " (" & Piva & ") ", _
                '                                1, _
                '                                "", _
                '                                Session, _
                '                                Server)

                ''§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§



                ' Solo nel caso di creazione nuova azienda
                If Operazione = enum_TipoOperazioneDB.Scrittura Then

                    '================================================
                    '----- Inserisco il centro aziendale
                    '================================================

                    Dim XmlDocCentro As New System.Xml.XmlDocument
                    Dim XmlDoc2Centro As New System.Xml.XmlDocument
                    Dim XmlDatiCentriAziendali As System.Xml.XmlElement
                    Dim XmlCentroAziendale As System.Xml.XmlElement
                    Dim objCentroAziendale As New AgronicaCoreAnagrafeBIZ.CentroAziendale_W

                    Dim Sa_Nome As String
                    Dim TitoloPossesso As Integer

                    Dim StrCentroAziendale As String
                    Dim sa_cod As Integer = 0

                    If Chk_Centro.Checked Then

                        Sa_Nome = TxtRagioneSociale.Text

                        'Genero la stringa XML del centro aziendale
                        Call XML_CentroAziendale(
                                        enum_CodificaDecodifica.Codifica,
                                        StrCentroAziendale,
                                        TipoOperazioneDB,
                                        Piva,
                                        sa_cod,
                                        "Centro " & Sa_Nome,
                                        0,
                                        0,
                                        0,
                                        0,
                                        0,
                                        0,
                                        " ",
                                        " ",
                                        " ",
                                        101,
                                        0,
                                        0,
                                        0,
                                        0,
                                        0,
                                        Validita_Inizio,
                                        Validita_Fine,
                                        BaseCode,
                                        TopCode,
                                        0,
                                        0,
                                        0,
                                        0)


                        '------------------------------------------------
                        '----- Costruisco la stringa XML complessiva di inserimento del CENTRO AZIENDALE
                        '------------------------------------------------

                        'Creo il nodo "DatiCentriAziendali"
                        XmlDatiCentriAziendali = XmlDocCentro.CreateElement("DatiCentriAziendali")

                        'Inserisco il nodo "CentroAziendale"
                        XmlDatiCentriAziendali.InnerXml = StrCentroAziendale

                        'Rendo l'albero figlio del documento
                        XmlDocCentro.AppendChild(XmlDatiCentriAziendali)

                        'Faccio una copia del documento XML
                        XmlDoc2Centro = XmlDocCentro

                        'Seleziono il nodo "CentroAziendale"
                        XmlCentroAziendale = XmlDocCentro.SelectSingleNode("//CentroAziendale")

                        'Inserisco gli elementi "Indirizzo" e "Codice"  e "Rubrica" come figli del nodo "CentroAziendale"
                        XmlCentroAziendale.InnerXml = StrIndirizzo '& StrCodici & StrRubricaGruppo

                        'Estraggo la stringa XML complessiva
                        StrXmlInserisci = XmlDocCentro.InnerXml


                        '------------------------------------------------
                        '----- Inserisco il CENTRO AZIENDALE
                        '------------------------------------------------

                        Dim Cod_Centro As Integer

                        'Eseguo i comandi XML
                        StrDummy = objCentroAziendale.CentroAziendale_Scrivi(
                                            CStr(StrXmlInserisci),
                                            OUT_Piva,
                                            OUT_SaCod,
                                            objParametri_Server,
                                            objParametri_Utenti)


                        Cod_Centro = CInt(OUT_SaCod)

                        objCentroAziendale = Nothing



                        '========================================================================
                        '----- FABBRICATO
                        '========================================================================

                        If Chk_Magazzino.Checked Then

                            Dim objSequenza As New AgronicaCoreDataProvider.Agro_Sequenze
                            Dim objIndirizzo As New AgronicaCoreAnagrafeDAL.Indirizzi_Write
                            'Dim Tipo_Indirizzo As Integer

                            '----- Creo un nuovo record indirizzo

                            'Recupero un nuovo indice
                            Cod_Indirizzo = objSequenza.NuovoId_Tabella(
                                                                    CStr("Indirizzi"),
                                                                    CInt(BaseCode),
                                                                    CInt(TopCode),
                                                                    objParametri_Server)



                            'Memorizzo il nuovo codice indirizzo
                            Txt_CodIndirizzo.Text = Cod_Indirizzo

                            'Scrivo il nuovo record
                            Indice = objIndirizzo.Scrivi(
                                                    CInt(Cod_Indirizzo),
                                                    CStr(Ind_Des),
                                                    CStr(Frz_Des),
                                                    CStr(CAP),
                                                    CStr(Com_Des),
                                                    CStr(Pro_Cod),
                                                    CStr(Stato),
                                                    CStr(Note),
                                                    CStr(Pro_Cod_Istat),
                                                    CStr(Com_Cod_Istat),
                                                     CDate("01/01/1900"),
                                                    CDate("31/12/2100"),
                                                    objParametri_Server)

                            'Distruggo gli oggetti COM+
                            objSequenza = Nothing
                            objIndirizzo = Nothing



                            '------------------------------------------------
                            '-----  FABBRICATO (MAGAZZINO)        -----------
                            '------------------------------------------------

                            Dim objSeqFabbricati As New AgronicaCoreDataProvider.Agro_Sequenze
                            Dim objFabbricati As New AgronicaCoreAnagrafeDAL.Fabbricati_W

                            Dim Fabbricato_Cod As Integer
                            Dim Fabbricato_Des As String
                            Dim Tipo_Fabbricato_Cod As Integer
                            Dim Regolamento_Cod As Integer
                            Dim mc_Convenzionale As Double
                            Dim mc_Conversione As Double
                            Dim mc_Biologico As Double
                            Dim Conversione_Inizio As Date
                            Dim Conversione_Fine As Date

                            Dim p_Prov As String
                            Dim p_Com As String
                            Dim p_Sezione As String
                            Dim p_Foglio As Integer
                            Dim p_Numero As Integer
                            Dim p_Subalterno As String


                            Fabbricato_Des = "Magazzino " & Sa_Nome
                            Tipo_Fabbricato_Cod = 20
                            TitoloPossesso = 0
                            mc_Convenzionale = 0
                            mc_Conversione = 0
                            mc_Biologico = 0
                            Regolamento_Cod = 1
                            Conversione_Inizio = #1/1/1900#
                            Conversione_Fine = #1/1/1900#

                            p_Prov = "000"
                            p_Com = "000"
                            p_Sezione = ""
                            p_Foglio = 0
                            p_Numero = 0
                            p_Subalterno = ""


                            '----- Creo un nuovo record

                            'Creo gli oggetti COM+
                            '   *   CreateCANCELLATOObject("Agro_Anagrafe2_AD.Agro_Sequenze")
                            '   *   CreateCANCELLATOObject("Agro_Anagrafe2_AD.Fabbricati_W")

                            'Recupero un nuovo indice
                            Fabbricato_Cod = objSeqFabbricati.NuovoId_xPiva_xSaCod(
                                                    CStr("SeqMagazzino"),
                                                    CStr("Mag_Cod"),
                                                    CStr(Piva),
                                                    CInt(Cod_Centro),
                                                    CInt(BaseCode),
                                                    CInt(TopCode),
                                                    objParametri_Server)

                            'Scrivo il nuovo record
                            Indice = objFabbricati.Scrivi(
                                                    CStr(Piva),
                                                    CInt(Cod_Centro),
                                                    CInt(Fabbricato_Cod),
                                                    CStr(Fabbricato_Des),
                                                    CInt(Cod_Indirizzo),
                                                    CInt(Tipo_Fabbricato_Cod),
                                                    CStr(p_Prov),
                                                    CStr(p_Com),
                                                    CStr(p_Sezione),
                                                    CInt(p_Foglio),
                                                    CInt(p_Numero),
                                                    CStr(p_Subalterno),
                                                    CDbl(mc_Convenzionale),
                                                    CDbl(mc_Conversione),
                                                    CDbl(mc_Biologico),
                                                    CInt(Regolamento_Cod),
                                                    CInt(TitoloPossesso),
                                                    CInt(0),
                                                    CInt(0),
                                                    CInt(0),
                                                    CInt(0),
                                                    CInt(0),
                                                    CInt(0),
                                                    CInt(0),
                                                    CInt(0),
                                                    CInt(0),
                                                    CInt(0),
                                                    CInt(0),
                                                    CInt(0),
                                                    CDate(Conversione_Inizio),
                                                    CDate(Conversione_Fine),
                                                    0,
                                                    0,
                                                    0,
                                                    0,
                                                    0,
                                                    0,
                                                    0,
                                                    0,
                                                    "", AgronicaCoreDataProvider.CostantiPersonalizzate.AGRODATAINIZIO, 0,
                                                    CDate(Validita_Inizio),
                                                    CDate(Validita_Fine),
                                                    objParametri_Server)

                            'Distruggo gli oggetti COM+
                            objSeqFabbricati = Nothing
                            objFabbricati = Nothing


                        End If


                    End If

                End If




                '------------------------------------------------
                '------------------------------------------------
                '------------------------------------------------
                'chiudo la transazione
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(1, objParametri_Server)
                'chiudo la connessione
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessione(objParametri_Server)


                ' Pulisco la Sessione
                HttpContext.Current.Session("dt_Padri") = Nothing
                HttpContext.Current.Session("dt_Codici") = Nothing
                HttpContext.Current.Session("comune_settato") = Nothing
                HttpContext.Current.Session("nome_comune_settato") = Nothing
                HttpContext.Current.Session("com") = Nothing



            Catch exc As Exception

                Errore = True
                '------------------------------------------------
                'Si e' verificata una eccezione !!!!!!
                '------------------------------------------------
                'chiudo la transazione con il rollback
                If objParametri_Server.objConnessione IsNot Nothing Then
                    If objParametri_Server.objTransazione IsNot Nothing Then
                        'chiudo transazione
                        ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)
                    End If
                    'chiudo la connessione
                    ConnessioniTransazioni.ChiudiConnessione(objParametri_Server)
                End If

                'Messaggio di errore
                StrDummy = exc.Message.ToString()

                'Faccio abortire la transazione
                'System.EnterpriseServices.ContextUtil.SetAbort()

                'Se la transazione ha avuto esito positivo allora ...
                'LblMessaggio.Text = " E' avvenuto un errore durante la fase di salvataggio !!! "

                'Stampo il messaggio sul client
                AgroMsgBox(AgronicaAgenda_2010.SiÈVerificatoUnErroreDuranteLaFaseDiSalvat & StrDummy, Page)

                '------------------------------------------------

            End Try

            '============================
            '===  Fine Aggiornamento  ===
            '============================
            If Not Errore Then
                'Dim Piva As String

                'Se la transazione ha avuto esito positivo allora ...
                If objParametriAgenda.Tipo_Operazione = enum_TipoOperazioneDB.Scrittura Then
                    'impresa appena inserita
                    Piva = Qs_PivaNuova
                Else
                    'Piva = Qs_Piva
                End If
                Piva = Stringa_Codifica(Piva, AgroKey_EncoderDecoder, Server)

                'Ritorno alla pagina AlberoImprese
                'Response.Redirect(Qs_PaginaRitorno & "?P=" & Piva)

                Dim Messaggio As String
                If Operazione = enum_TipoOperazioneDB.Modifica Then
                    Messaggio = DirectCast(GetLocalResourceObject("AZIENDAModificataConSuccesso"), String) & vbCrLf & vbCrLf
                Else
                    Messaggio = DirectCast(GetLocalResourceObject("NUOVAAZIENDASalvataConSuccesso"), String) & vbCrLf & vbCrLf
                End If

                If Chk_Centro.Checked Then
                    Messaggio += DirectCast(GetLocalResourceObject("CreatoNUOVOCENTROAssociato"), String) & vbCrLf
                End If

                If Chk_Magazzino.Checked Then
                    Messaggio += DirectCast(GetLocalResourceObject("CreatoNUOVOMAGAZZINOAssociato"), String) & vbCrLf
                End If


                Dim TargetUrl As String
                Select Case tipo_salva.Value

                    Case 1
                        Messaggio += AgronicaAgenda_2010.VerràRicaricataLaPaginaPerInserimento & vbCrLf
                        'Messaggi.AgroMsgBox(Messaggio, Page, , UpdatePanel_script, , True)
                        AgroMsgBox(Messaggio, Page)

                        Page_Load(Nothing, EventArgs.Empty)
                        clear_form()
                    Case Else
                        'Messaggi.AgroMsgBox(Messaggio, Page, , UpdatePanel_script, , True)

                        TargetUrl = Master.TrovaRedirectCorretto(False, enum_PagineGiasOnline.MenuAnagrafica, objParametriAgenda, , Qs_Visibilita)
                        'Response.Redirect(TargetUrl)
                        AgroMsgBox(Messaggio, Page, , , "window.location = '" & TargetUrl & "';")

                End Select

            End If

        Catch ex As Exception

            'Messaggi.AgroMsgBox(Messaggio, Page, , UpdatePanel_script, , True)
            AgroMsgBox(ex.Message, Page)

        End Try

    End Sub

    Private Sub clear_form()

        HttpContext.Current.Session("OTE") = Nothing

        'Pulisco le varie Textbox e ComboBox 
        TxtPiva.Text = ""
        TxtRagioneSociale.Text = ""

        TxtValiditaInizio.Text = ""
        TxtValiditaFine.Text = ""

        Txt_CodIndirizzo.Text = "0"
        Txt_Via.Text = ""
        Txt_Frazione.Text = ""
        Txt_CAP.Text = ""
        'Txt_Stato.Text = "Italia"
        Txt_Note.Text = ""

        'txtCuaaCod.Text = ""
        TxtCodiceValore.Text = ""

        CaricaGriglia_Codici(True, xPiva)

    End Sub



    '########################################################################################
    'Private Sub Page_CommitTransaction(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.CommitTransaction

    '    Dim Piva As String
    '    Dim Chiave As String

    '    'Se la transazione ha avuto esito positivo allora ...
    '    If EseguitaOperazione = True Then

    '        ''Avevo messo questo perchè nel caso un utente inserisca un'impresa
    '        ''ma che poi non abbia i permessi x vederla, lo dirottavo all'impresa padre
    '        'If Qs_Operazione = enum_TipoOperazioneDB.Scrittura Then
    '        '    'Recupero  la partita IVA dalla querystring
    '        '    Piva = Qs_PivaPadre
    '        'Else
    '        '    'Recupero  la partita IVA dalla querystring
    '        '    Piva = Qs_Piva
    '        'End If


    '        If Qs_Operazione = enum_TipoOperazioneDB.Scrittura Then
    '            'impresa appena inserita
    '            Piva = Qs_PivaNuova
    '        Else
    '            Piva = Qs_Piva
    '        End If

    '        'Piva = Qs_Piva

    '        'Codifico la partita IVA
    '        Piva = Stringa_Codifica(Piva, AgroKey_EncoderDecoder, Server)

    '        'Ritorno alla pagina AlberoImprese
    '        Response.Redirect(Qs_PaginaRitorno & "?P=" & Piva)

    '    End If

    'End Sub




    '########################################################################################
    'Private Sub Page_AbortTransaction(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.AbortTransaction

    '    Dim Piva As String

    '    'Verifico che l'uscita sia voluta ....
    '    If PremutoAnnulla = True Then

    '        'Recupero  la partita IVA dalla querystring
    '        Piva = Qs_Piva

    '        'Codifico la partita IVA
    '        Piva = Stringa_Codifica(Piva, AgroKey_EncoderDecoder, Server)

    '        'Ritorno alla pagina AlberoImprese
    '        Response.Redirect(Qs_PaginaRitorno & "?P=" & Piva)

    '    End If

    'End Sub



    '#########################################################################################
    '#########################################################################################
    '#########################################################################################
    '#########################################################################################



    '########################################################################################
    Private Enum enum_Pannello
        Pannello_Riferimenti = 1
        Pannello_Accessori = 2
        Pannello_Catasto = 3
        Pannello_Analisi = 4
        Pannello_Biologico = 5
        Pannello_ParoleChiave = 6
    End Enum


    ''########################################################################################
    'Private Sub Imposta_Pannelli(ByVal PannelloAttivo As enum_Pannello)

    '    Dim Dimensione As Unit
    '    Dim xHeight As Integer = 440
    '    Dim xWidth As Integer = 950
    '    Dim xTop As String = "152px"
    '    Dim xLeft As String = "16px"

    '    '----- Imposto le dimensioni

    '    With Me.Pannello_Riferimenti
    '        .Height = Dimensione.Pixel(xHeight)
    '        .Width = Dimensione.Pixel(xWidth)
    '        .Style.Item("Top") = xTop
    '        .Style.Item("Left") = xLeft
    '    End With

    '    With Me.Pannello_Accessori
    '        .Height = Dimensione.Pixel(xHeight)
    '        .Width = Dimensione.Pixel(xWidth)
    '        .Style.Item("Top") = xTop
    '        .Style.Item("Left") = xLeft
    '    End With

    '    With Me.Pannello_Catasto
    '        .Height = Dimensione.Pixel(xHeight)
    '        .Width = Dimensione.Pixel(xWidth)
    '        .Style.Item("Top") = xTop
    '        .Style.Item("Left") = xLeft
    '    End With

    '    With Me.Pannello_Analisi
    '        .Height = Dimensione.Pixel(xHeight)
    '        .Width = Dimensione.Pixel(xWidth)
    '        .Style.Item("Top") = xTop
    '        .Style.Item("Left") = xLeft
    '    End With

    '    With Me.Pannello_Biologico
    '        .Height = Dimensione.Pixel(xHeight)
    '        .Width = Dimensione.Pixel(xWidth)
    '        .Style.Item("Top") = xTop
    '        .Style.Item("Left") = xLeft
    '    End With

    '    With Me.Pannello_ParoleChiave
    '        .Height = Dimensione.Pixel(xHeight)
    '        .Width = Dimensione.Pixel(xWidth)
    '        .Style.Item("Top") = xTop
    '        .Style.Item("Left") = xLeft
    '    End With


    '    '----- Disattivo tutti i pannelli

    '    Me.Pannello_Riferimenti.Visible = False
    '    Me.Pannello_Accessori.Visible = False
    '    Me.Pannello_Catasto.Visible = False
    '    Me.Pannello_Analisi.Visible = False
    '    Me.Pannello_Biologico.Visible = False
    '    Me.Pannello_ParoleChiave.Visible = False


    '    'AgroColor_BluChiaro .... #afeeee
    '    'AgroColor_BluChiaro2 ... #c0ffff

    '    Me.Pannello_BTN_Riferimenti.BackColor = AgroColor_BluChiaro
    '    Me.Pannello_BTN_Accessori.BackColor = AgroColor_BluChiaro
    '    Me.Pannello_BTN_Catasto.BackColor = AgroColor_BluChiaro
    '    Me.Pannello_BTN_Analisi.BackColor = AgroColor_BluChiaro
    '    Me.Pannello_BTN_Biologico.BackColor = AgroColor_BluChiaro
    '    Me.Pannello_BTN_ParoleChiave.BackColor = AgroColor_BluChiaro

    '    Me.Lbl_BTN_Riferimenti.BackColor = AgroColor_BluChiaro
    '    Me.Lbl_BTN_Accessori.BackColor = AgroColor_BluChiaro
    '    Me.Lbl_BTN_Catasto.BackColor = AgroColor_BluChiaro
    '    Me.Lbl_BTN_Analisi.BackColor = AgroColor_BluChiaro
    '    Me.Lbl_BTN_Biologico.BackColor = AgroColor_BluChiaro
    '    Me.Lbl_BTN_ParoleChiave.BackColor = AgroColor_BluChiaro

    '    '----- Attivo il pannello richiesto

    '    Select Case PannelloAttivo

    '        Case enum_Pannello.Pannello_Riferimenti

    '            Me.Pannello_Riferimenti.Visible = True
    '            Me.Pannello_BTN_Riferimenti.BackColor = Color.Gold
    '            Me.Lbl_BTN_Riferimenti.BackColor = Color.Gold

    '        Case enum_Pannello.Pannello_Accessori

    '            Me.Pannello_Accessori.Visible = True
    '            Me.Pannello_BTN_Accessori.BackColor = Color.Gold
    '            Me.Lbl_BTN_Accessori.BackColor = Color.Gold

    '        Case enum_Pannello.Pannello_Catasto

    '            Me.Pannello_Catasto.Visible = True
    '            Me.Pannello_BTN_Catasto.BackColor = Color.Gold
    '            Me.Lbl_BTN_Catasto.BackColor = Color.Gold

    '        Case enum_Pannello.Pannello_Analisi

    '            Me.Pannello_Analisi.Visible = True
    '            Me.Pannello_BTN_Analisi.BackColor = Color.Gold
    '            Me.Lbl_BTN_Analisi.BackColor = Color.Gold

    '        Case enum_Pannello.Pannello_Biologico

    '            Me.Pannello_Biologico.Visible = True
    '            Me.Pannello_BTN_Biologico.BackColor = Color.Gold
    '            Me.Lbl_BTN_Biologico.BackColor = Color.Gold

    '        Case enum_Pannello.Pannello_ParoleChiave

    '            Me.Pannello_ParoleChiave.Visible = True
    '            Me.Pannello_BTN_ParoleChiave.BackColor = Color.Gold
    '            Me.Lbl_BTN_ParoleChiave.BackColor = Color.Gold

    '    End Select

    'End Sub


    '########################################################################################
    'Private Sub ImgBtn_Riferimenti_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImgBtn_Riferimenti.Click
    '    Imposta_Pannelli(enum_Pannello.Pannello_Riferimenti)
    'End Sub

    ''########################################################################################
    'Private Sub ImgBtn_Accessori_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImgBtn_Accessori.Click
    '    Imposta_Pannelli(enum_Pannello.Pannello_Accessori)
    'End Sub

    ''########################################################################################
    'Private Sub ImgBtn_Catasto_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImgBtn_Catasto.Click
    '    Imposta_Pannelli(enum_Pannello.Pannello_Catasto)
    'End Sub

    ''########################################################################################
    'Private Sub ImgBtn_Analisi_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImgBtn_Analisi.Click
    '    Imposta_Pannelli(enum_Pannello.Pannello_Analisi)
    'End Sub

    ''########################################################################################
    'Private Sub ImgBtn_Biologico_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImgBtn_Biologico.Click
    '    Imposta_Pannelli(enum_Pannello.Pannello_Biologico)
    'End Sub

    ''########################################################################################
    'Private Sub ImgBtn_ParoleChiave_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImgBtn_ParoleChiave.Click
    '    Imposta_Pannelli(enum_Pannello.Pannello_ParoleChiave)
    'End Sub

    ''########################################################################################
    Private Sub Ordina_Listbox(ByRef LST As ListBox)

        Dim i As Integer
        Dim j As Integer
        Dim appoggio As String

        For i = 0 To LST.Items.Count - 2
            For j = i + 1 To LST.Items.Count - 1
                If LST.Items(i).Text > LST.Items(j).Text Then

                    appoggio = LST.Items(i).Text
                    LST.Items(i).Text = LST.Items(j).Text
                    LST.Items(j).Text = appoggio

                    appoggio = LST.Items(i).Value
                    LST.Items(i).Value = LST.Items(j).Value
                    LST.Items(j).Value = appoggio

                End If
            Next j
        Next i

    End Sub


    '#########################################################################################
    '#########################################################################################
    '#########################################################################################








    '########################################################################################
    Private Sub CaricaGriglia_Codici(ByVal Inizializza As Boolean, _
                                       Optional ByVal Piva As String = "")

        '----- Definizione delle variabili

        Dim Dt As New DataTable
        Dim Dr As DataRow
        Dim Contatore As Integer

        Dim StrXmlCodiciAttuali As String = ""
        Dim StrXmlCodice As String = ""

        Dim BaseCode As Integer
        Dim TopCode As Integer

        '----- Definisco la struttura del DataTable

        Dt.Columns.Add(New DataColumn("Contatore", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Id_Cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Descrizione", GetType(String)))
        Dt.Columns.Add(New DataColumn("Val_Cod", GetType(String)))
        Dt.Columns.Add(New DataColumn("Validita_Inizio", GetType(String)))
        Dt.Columns.Add(New DataColumn("Validita_Fine", GetType(String)))


        '----- Recupero l'elenco delle particelle

        Dim objImpreseCodici As New AgronicaCoreAnagrafeDAL.Imprese_Codici_Read
        Dim DTCodici As DataTable

        'If Inizializza = True Then
        '    DTCodici = Nothing
        '    HttpContext.Current.Session("dt_Codici") = Nothing

        'Else

        If Piva <> "" Then
            DTCodici = objImpreseCodici.Leggi(CStr(Piva),
                                    0,
                                        enumSelezioneVariabile.Selezione_TabellaCompleta,
                                        " ((id_cod < 2000 AND id_cod <> 1107) OR (id_cod >= 3000 AND id_cod <> 1107)) ",
                                        "",
                                        objParametri_Server)
        End If

        If (Not IsNothing(DTCodici)) AndAlso (DTCodici.Rows.Count > 0) Then

            Calcola_BaseCode_TopCode(BaseCode, TopCode, Session("ASG_ProgressivoGIAS"))

            Contatore = 1
            Dim i As Integer
            Dim objCodiceAnagrafeR As New AgronicaCoreAnagrafeDAL.Codice_Anagrafe_R

            For i = 0 To DTCodici.Rows.Count - 1

                Select Case DTCodici.Rows(i).Item("Id_Cod")

                    Case enum_CodiciAnagrafe.CodiceCUAA, enum_CodiciAnagrafe.Tecnico, enum_CodiciAnagrafe.Organismo_di_Controllo

                    Case Else

                        'Creo una nuova riga
                        Dr = Dt.NewRow

                        'Definisco i valori

                        Dr.Item("Contatore") = Contatore
                        Contatore += 1

                        Dr.Item("Id_Cod") = DTCodici.Rows(i).Item("id_cod")
                        Dr.Item("Descrizione") = objCodiceAnagrafeR.CodiceAnagrafeDes_from_CodiceAnagrafeCod(CInt(DTCodici.Rows(i).Item("id_cod")), objParametri_Server)
                        Dr.Item("Val_Cod") = DTCodici.Rows(i).Item("val_cod")

                        If (CDate(DTCodici.Rows(i).Item("Validita_Inizio"))) = AGRODATAINIZIO Then
                            Dr.Item("Validita_Inizio") = "..."
                        Else
                            Dr.Item("Validita_Inizio") = CDate(DTCodici.Rows(i).Item("Validita_Inizio")).ToShortDateString
                        End If

                        If (CDate(DTCodici.Rows(i).Item("Validita_Fine"))) = AGRODATAFINE Then
                            Dr.Item("Validita_Fine") = "..."
                        Else
                            Dr.Item("Validita_Fine") = CDate(DTCodici.Rows(i).Item("Validita_Fine")).ToShortDateString
                        End If

                        Dt.Rows.Add(Dr)

                End Select

                'Genero l'XML del singolo nodo
                Call XML_Codice(enum_CodificaDecodifica.Codifica,
                                StrXmlCodice,
                                enum_TipoOperazioneDB.Cancellazione,
                                DTCodici.Rows(i).Item("Id_Cod"),
                                DTCodici.Rows(i).Item("Val_cod"),
                                DTCodici.Rows(i).Item("xValidita_Inizio"),
                                DTCodici.Rows(i).Item("xValidita_Fine"),
                                BaseCode,
                                TopCode)

                StrXmlCodiciAttuali &= StrXmlCodice


            Next

        End If

        ' Controllo se non è stato aggiunto un nuovo record di Codici Anagrafici
        If (IsNothing(HttpContext.Current.Session("dt_Codici"))) Then

            HttpContext.Current.Session("dt_Codici") = Dt
            'GridView_Codici.DataSource = HttpContext.Current.Session("dt_Codici")
            'GridView_Codici.DataBind()
        Else
            'GridView_Codici.DataSource = Dt
            'GridView_Codici.DataBind()
        End If



        'End If

        jsCodici = DT_to_Json_Codici(Dt)

        '----- Salvo la StrXmlCodiciAttuali 

        Session("StrXmlCodiciAttuali") = StrXmlCodiciAttuali


        '----- Associo il DataTable con il DataGrid
        'Aggiorna_Griglia_Codici(Dt)


    End Sub

    ''#############################################################################################################################################################
    'Private Sub GridView_Codici_RowCommand(sender As Object, e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles GridView_Codici.RowCommand

    '    Dim IndiceRigaGriglia As Integer

    '    IndiceRigaGriglia = Convert.ToInt32(e.CommandArgument)

    '    Select Case e.CommandName

    '        Case "Elimina"

    '            DataGrid_Codice_Elimina(IndiceRigaGriglia)

    '    End Select
    'End Sub

    '#############################################################################################################################################################
    Private Sub DataGrid_Codice_Elimina(ByVal IndiceRigaGriglia As Integer)

        'Dim Contatore As Integer
        Dim Dt As DataTable

        'Recupero il codice da cancellare
        'Contatore = GridView_Codici.DataKeys(IndiceRigaGriglia).Item(0)
        'Dt.Rows(i).Item("Contatore")

        'Recupero il datatable
        If (IsNothing(HttpContext.Current.Session("dt_Codici"))) Then
            Dt = ViewState("vs_dtCodici")
        Else
            Dt = HttpContext.Current.Session("dt_Codici")
        End If


        Dim Dr As DataRow = Dt.Rows(IndiceRigaGriglia)

        'For i As Integer = 0 To Dt.Rows.Count - 1
        '    If Dt.Rows(i).Item("Contatore") = Contatore Then
        '        Dr = Dt.Rows(i)
        '        Exit For
        '    End If
        'Next

        'Elimino la riga
        Dr.Delete()

        'GridView_Codici.DataSource = Dt
        'GridView_Codici.DataBind()


    End Sub


    '#############################################################################################################################################################
    'Private Sub GridView_Padri_RowCommand(sender As Object, e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles GridView_Padri.RowCommand

    '    Dim IndiceRigaGriglia As Integer

    '    IndiceRigaGriglia = Convert.ToInt32(e.CommandArgument)

    '    Select Case e.CommandName

    '        Case "Elimina"

    '            DataGrid_Padre_Elimina(IndiceRigaGriglia)

    '    End Select
    'End Sub

    '#############################################################################################################################################################
    'Private Sub DataGrid_Padre_Elimina(ByVal IndiceRigaGriglia As Integer)

    '    Dim Contatore As Integer
    '    Dim Dt As DataTable
    '    Dim Dr As DataRow
    '    Dim i As Integer

    '    'Recupero il codice da cancellare
    '    'Contatore = GridView_Codici.DataKeys(IndiceRigaGriglia).Item(0)
    '    'Dt.Rows(i).Item("Contatore")

    '    'Recupero il datatable
    '    If (IsNothing(HttpContext.Current.Session("dt_Padri"))) Then
    '        Dt = ViewState("vs_dtPadri")
    '    Else
    '        Dt = HttpContext.Current.Session("dt_Padri")
    '    End If


    '    Dr = Dt.Rows(IndiceRigaGriglia)

    '    'Dim i As Integer
    '    'For i = 0 To Dt.Rows.Count - 1
    '    '    If Dt.Rows(i).Item("Contatore") = Contatore Then
    '    '        Dr = Dt.Rows(i)
    '    '        Exit For
    '    '    End If
    '    'Next

    '    'Elimino la riga
    '    Dr.Delete()

    '    GridView_Padri.DataSource = Dt
    '    GridView_Padri.DataBind()

    '    'Aggiorna_Griglia_Codici(Dt)


    'End Sub


    '#####################################################################################################################################################
    'Private Sub ImgBtn_Aggiungi_Codice_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImgBtn_Aggiungi_Codice.Click

    '    Codice_Inserisci()

    'End Sub

    '#####################################################################################################################################################
    'Private Sub Codice_Inserisci()

    '    Dim Codice_Cod As Integer
    '    Dim Codice_Des As String
    '    Dim Codice_Valore As String
    '    Dim DataInizio As Date
    '    Dim DataFine As Date
    '    Dim i As Integer
    '    Dim Errore As Boolean = False
    '    Dim xDataInizio As Date
    '    Dim xDataFine As Date
    '    Dim Dt As DataTable
    '    Dim Dr As DataRow
    '    Dim IndiceRiga As Integer
    '    Dim Contatore As Integer
    '    Dim MessaggioErrore As String

    '    '----------------------------------------
    '    '----------------------------------------
    '    '----- Verifico le informazioni
    '    '----------------------------------------
    '    '----------------------------------------

    '    If Not IsDate(TxtValiditaInizioCodice.Text) Then
    '        DataInizio = AGRODATAINIZIO
    '    Else
    '        DataInizio = CDate(TxtValiditaInizioCodice.Text)
    '    End If

    '    If Not IsDate(TxtValiditaFineCodice.Text) Then
    '        DataFine = AGRODATAFINE
    '    Else
    '        DataFine = CDate(TxtValiditaFineCodice.Text)
    '    End If

    '    Codice_Cod = CmbCodice.SelectedValue
    '    Codice_Des = CmbCodice.SelectedItem.Text
    '    Codice_Valore = TxtCodiceValore.Text

    '    '----------------------------------------
    '    '----- Verifico che la data inizio non sia posteriore alla data fine
    '    If DataInizio > DataFine Then

    '        'Messaggio di errore
    '        Messaggi.AgroMsgBox("La data di validità fine del codice non può precedere la data di validità inizio!", Page, , UpdatePanel_script, , True)

    '        'Esco dalla subroutine
    '        Exit Sub

    '    End If


    '    '----------------------------------------
    '    '----- Verifico che non ci sia intersezione fra gli intervalli gia' impostati

    '    'NOTA
    '    'La DataInizio e la DataFine non devono coincidere con nessuna della altre date
    '    'La DataInizio e la DataFine non devono cadere in nessuno degli intervalli

    '    'Se e' gia' presente almeno un intervallo ...
    '    If GridView_Codici.Rows.Count > 0 Then

    '        For i = 0 To GridView_Codici.Rows.Count - 1

    '            'Recupero i valori della riga i-esima
    '            If (GridView_Codici.DataKeys(i).Item(3) = "...") Then
    '                xDataInizio = #1/1/1900#
    '            Else
    '                xDataInizio = CDate(GridView_Codici.DataKeys(i).Item(3))
    '            End If

    '            If (GridView_Codici.DataKeys(i).Item(4) = "...") Then
    '                xDataFine = #12/31/2100#
    '            Else
    '                xDataFine = CDate(GridView_Codici.DataKeys(i).Item(4))
    '            End If

    '            'Inizializzo
    '            Errore = False

    '            'Verifico
    '            If (DataInizio >= xDataInizio) And (DataInizio <= xDataFine) Then

    '                'Intersezione
    '                Errore = True
    '                MessaggioErrore = "L'intervallo impostato si sovrappone a quelli precedenti !!!!"

    '                'Esco dal ciclo
    '                Exit For

    '            Else

    '                If (DataFine >= xDataInizio) And (DataFine <= xDataFine) Then

    '                    'Intersezione
    '                    Errore = True
    '                    MessaggioErrore = "L'intervallo impostato si sovrappone a quelli precedenti !!!!"

    '                    'Esco dal ciclo
    '                    Exit For

    '                Else

    '                    If IsDate(TxtValiditaInizio.Text) Then
    '                        If DataInizio < CDate(TxtValiditaInizio.Text) Then
    '                            Errore = True
    '                            MessaggioErrore += "L'inizio del periodo non puo' precedere la creazione dell'Impresa. (" & InizioCentro.Value & ")"

    '                            'Esco dal ciclo
    '                            Exit For

    '                        End If
    '                    End If

    '                    If IsDate(TxtValiditaFine.Text) Then
    '                        If DataFine > CDate(TxtValiditaFine.Text) Then
    '                            Errore = True
    '                            MessaggioErrore += "La fine del periodo non puo' seguire la cessazione dell'Impresa. (" & FineCentro.Value & ")"

    '                            'Esco dal ciclo
    '                            Exit For

    '                        End If
    '                        Errore = False
    '                    End If
    '                End If


    '            End If

    '        Next


    '        'Se c'e' stato un errore ...
    '        If Errore = True Then

    '            'Messaggio di errore
    '            ' Call AgroMsgBox(MessaggioErrore, Page)

    '            Messaggi.AgroMsgBox(MessaggioErrore, Page, , UpdatePanel_script, , True)

    '            'Esco dalla subroutine
    '            Exit Sub

    '        End If

    '    End If


    '    '----- Recupero il datatable

    '    If Not IsNothing(ViewState("vs_dtCodici")) Then
    '        'Recupero il datatable dal ViewState
    '        Dt = ViewState("vs_dtCodici")
    '    Else
    '        Dt = New DataTable
    '    End If

    '    '----- Cerco il valore massimo del contatore

    '    Contatore = 0

    '    For i = 0 To Dt.Rows.Count - 1
    '        If Dt.Rows(i).Item("Contatore") > Contatore Then
    '            Contatore = Dt.Rows(i).Item("Contatore")
    '        End If
    '    Next

    '    Contatore = Contatore + 1

    '    '----- Inserisco la riga nel datagrid

    '    'Creo una nuova riga
    '    Dr = Dt.NewRow

    '    'Definisco i valori
    '    Dr.Item("Contatore") = Contatore

    '    Dr.Item("Id_Cod") = Codice_Cod
    '    Dr.Item("Descrizione") = Codice_Des
    '    Dr.Item("Val_Cod") = Codice_Valore

    '    If DataInizio = #1/1/1900# Then
    '        Dr.Item("Validita_Inizio") = "..."
    '    Else
    '        Dr.Item("Validita_Inizio") = DataInizio.ToShortDateString
    '    End If

    '    If DataFine = #12/31/2100# Then
    '        Dr.Item("Validita_Fine") = "..."
    '    Else
    '        Dr.Item("Validita_Fine") = DataFine.ToShortDateString
    '    End If

    '    'Associo alla tabella la nuova riga creata
    '    Dt.Rows.Add(Dr)

    '    '----- Associo il DataTable con il DataGrid
    '    Aggiorna_Griglia_Codici(Dt)


    '    '----- Azzero i controlli di provenienza dei dati

    '    Me.TxtValiditaInizio.Text = ""
    '    Me.TxtValiditaFine.Text = ""
    '    Me.TxtCodiceValore.Text = ""

    'End Sub


    '#####################################################################################################################################################
    'Private Sub ImgBtn_Aggiungi_Padre_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImgBtn_Aggiungi_Padre.Click

    '    'Padre_Inserisci()
    '    Dim Padre_Cod As String
    '    Dim Padre_Des As String

    '    Padre_Cod = Cmb_Imprese.SelectedItem.Value
    '    'Padre_Des = Cmb_Imprese.SelectedItem.Text
    '    Padre_Des = Cmb_Imprese.SelectedItem.Text

    '    InserisciRiga(Padre_Cod, _
    '                          Padre_Des, _
    '                          "01/01/1900", _
    '                          "31/12/2100")

    'End Sub




    '#####################################################################################################################################################
    Private Sub Aggiorna_Griglia_Codici(ByVal Dt As DataTable)

        ' Chiavi per recuperare le righe
        Dim DtKeys(4) As String
        DtKeys(0) = "Contatore"
        DtKeys(1) = "Id_Cod"
        DtKeys(2) = "Val_Cod"
        DtKeys(3) = "Validita_Inizio"
        DtKeys(4) = "Validita_Fine"

        If (Not IsNothing(HttpContext.Current.Session("dt_Codici"))) Then

            ' Aggiorno il datatable di Codici Anagrafici  con quello in sessione
            'DTCodici = HttpContext.Current.Session("dt_Codici")

            'GridView_Codici.DataSource = HttpContext.Current.Session("dt_Codici")
            ''GridView_Codici.DataKeyNames = DtKeys
            'GridView_Codici.DataBind()

        Else

            '----- Associo il DataTable con la DataGrid
            'GridView_Codici.DataSource = Dt
            'GridView_Codici.DataKeyNames = DtKeys
            'GridView_Codici.DataBind()

            '----- Salvo il DataTable dentro il viewstate
            ViewState("vs_dtCodici") = Dt

        End If




    End Sub











    ''###############################################################################
    Public Sub ChiaveAlbero_Decodifica_ImpiantiVegetali(ByVal Chiave As String, _
                                ByRef TipoNodo As enum_TipoNodo, _
                                ByRef Piva As String, _
                                ByRef Sa_Cod As Integer, _
                                ByRef Campo_Cod As Integer, _
                                ByRef Appezza As Integer, _
                                ByRef Id_Imp As Integer, _
                                ByRef Cod_Fiscale As String, _
                                ByRef Fabbricato_Cod As Integer)
        '------------------------------------------------------------------------
        Dim ArrayKey() As String

        'Spezzo la chiave
        ArrayKey = Split(Chiave, "\")

        'Recupero gli elementi
        TipoNodo = CInt(ArrayKey(0))
        Piva = CStr(ArrayKey(1))
        Sa_Cod = CInt(ArrayKey(2))
        Campo_Cod = CInt(ArrayKey(3))
        Appezza = CInt(ArrayKey(4))
        Id_Imp = CInt(ArrayKey(5))
        Cod_Fiscale = CStr(ArrayKey(13))
        Fabbricato_Cod = CInt(ArrayKey(14))

    End Sub

    ''###############################################################################
    Public Sub ChiaveAlbero_Decodifica_PartitaIVA_SaCod(ByVal Chiave As String, _
                                ByRef PartitaIVA As String, _
                                ByRef Sa_Cod As Integer)
        '------------------------------------------------------------------------
        Dim ArrayKey() As String

        'Spezzo la chiave
        ArrayKey = Split(Chiave, "\")

        'Recupero gli elementi
        PartitaIVA = CStr(ArrayKey(1))
        Sa_Cod = CInt(ArrayKey(2))


    End Sub




    'Protected Sub dll_Provincia_SelectedIndexChanged(sender As Object, e As EventArgs) Handles dll_Provincia.SelectedIndexChanged


    '    If dll_Provincia.SelectedItem.Value <> "" Then

    '        AgronicaCoreUtility.CaricaListControl.Comuni(ddl_comune, _
    '                                                         True, "", "", _
    '                                                         dll_Provincia.SelectedItem.Value, False, 1, "", "", objParametri_Server)

    '        Me.Txt_ProvinciaSigla.Text = dll_Provincia.SelectedItem.Value

    '        'chiamo il componente solo se non è selezionata stringa vuota
    '        'se passo al componente stringa vuoto mi tira su tutto
    '        Dim objI As New AgronicaCoreMetaSchemaDAL.Istat_R
    '        'Imposto la textbox del CODICE ISTAT
    '        Me.Txt_ProCodIstat.Text = objI.CodIstat_from_Provincia(dll_Provincia.SelectedItem.Value, objParametri_Server)

    '    Else

    '        Me.Txt_ProCodIstat.Text = "000"

    '        'Svuoto la select Comuni
    '        ddl_comune.Items.Clear()

    '    End If


    '    'Imposto la textbox della PROVINCIA
    '    'Me.Txt_ProvinciaSigla.Text = dll_Provincia.SelectedItem.Text

    '    'Pulisco le textbox associate al comune
    '    'Me.Txt_Comune.Value = ""
    '    Me.Txt_ComCodIstat.Text = ""
    '    Me.Txt_CAP.Text = ""
    'End Sub

    Private Sub Impresa_Edit_Unload(sender As Object, e As System.EventArgs) Handles Me.Unload

        ' Cancello la Session
        'HttpContext.Current.Session("dt_Padri") = ""
    End Sub


    '########################################################################################
    'Private Sub refresh_Grid_Padri_Click(sender As Object, e As System.EventArgs) Handles refresh_Grid_Padri.Click

    '    GridView_Padri.DataSource = HttpContext.Current.Session("dt_Padri")
    '    GridView_Padri.DataBind()

    'End Sub

    'Private Sub refresh_Grid_Codici_Click(sender As Object, e As System.EventArgs) Handles refresh_Grid_Codici.Click

    '    GridView_Codici.DataSource = HttpContext.Current.Session("dt_Codici")
    '    GridView_Codici.DataBind()

    'End Sub

    ''###############################################################################
    Public Sub ChiaveAlbero_Decodifica_PartitaIVA(ByVal Chiave As String,
                                ByRef PartitaIVA As String)
        '------------------------------------------------------------------------
        Dim ArrayKey() As String

        'Spezzo la chiave
        ArrayKey = Split(Chiave, "\")

        'Recupero l'elemento di indice 0 (TipoNodo)
        PartitaIVA = CStr(ArrayKey(1))

    End Sub

    <WebMethod(EnableSession:=True)>
    Public Shared Function OttieniCAP(ISTAT_Prov As String, ISTAT_Com As String) As RispostaStandard
        Dim r As New RispostaStandard

        Lingua.Gias_InizializzaCultura_DaSession()

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

            Dim objIstat As New AgronicaCoreMetaSchemaDAL.Istat_R

            r.RispostaOK = True
            r.RispostaStringa = objIstat.CAP_form_PROV_COM(ISTAT_Prov, ISTAT_Com, objParametri_Server)

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf & AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try
        Return r
    End Function


    <WebMethod(EnableSession:=True)>
    Public Shared Function GeneraRandom() As RispostaStandard

        Dim r As New RispostaStandard

        Lingua.Gias_InizializzaCultura_DaSession()

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

            objParametri_Server.ImpostaFinestre_con_SalvataggioTemporale(AGRODATAINIZIO, AGRODATAFINE)

            Dim ok = False


            Dim objAgroSe As New AgronicaCoreDataProvider.Agro_Sequenze
            Dim objCont As New AgronicaCoreAnagrafeDAL.Contatti_R
            Dim dt As DataTable

            Dim str_r As String = ""
            While ok = False
                str_r = objAgroSe.NuovoId_Tabella("impresa", 0, 0, objParametri_Server).ToString.Replace("-", "F")
                'controllo se è già usato 
                dt = objCont.LeggiContattoSpecifico("", str_r, 0, enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_Server)
                If dt.Rows.Count = 0 Then
                    ok = True
                End If
            End While
            objParametri_Server.ResettaFinestra()

            r.RispostaOK = True
            r.RispostaStringa = str_r

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf & AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try
        Return r
    End Function

    Private Function Ottieni_ID_CF_Contatto(ByVal Stato As String) As enum_Contatti_IdCf

        Dim id_cf As enum_Contatti_IdCf = enum_Contatti_IdCf.PersonaGiuridica

        If Stato.ToLower() <> "it" Then
            id_cf = enum_Contatti_IdCf.ContattoEstero
        End If

        Return id_cf

    End Function

End Class
