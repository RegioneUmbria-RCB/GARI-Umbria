Imports System.ComponentModel
Imports System.Data
Imports System.Text
Imports System.Web
Imports System.Web.Services
Imports System.Web.Script.Serialization
Imports System.Web.Script.Services

Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreMetaSchemaDAL
Imports AgronicaCoreUtentiDAL


''' <summary>
''' Per utilizzare questo controllo è necessario includere all'iterno della pagina i seguenti script 
''' jquery.cookie.js
''' jquery.hotkeys.js
''' jquery.jstree.js
''' RICORDARE DI AGGIUNGERE LA FUNZIONE JS
''' function DoPostBack_Combo($_combo,valoreOpt) {
'''        if($_combo.attr("id").endsWith("ComboMagazzini")){
'''            //salvo nel campo nascosto il nome della funzione da richiamare
'''            $("#<%= fooName_PostedBack.ClientID %>").val("comboLavorazioni_IndexChanged");
'''            //scatena il postback simulato il click del btn nascosto
'''            $("#<%=btnDummyPostback.ClientID %>").click();
'''        }
''' </summary>
''' <remarks></remarks>

<DefaultProperty("Text"), ToolboxData("<{0}:ComboFormulati runat=server></{0}:ComboFormulati>")> Public Class ComboFormulati
    Inherits System.Web.UI.WebControls.WebControl


    Public ddl_Formulati As DropDownList


    Private _PrimaRiga_Flag As Boolean
    Private _PrimaRiga_Text As String
    Private _PrimaRiga_Value As String
    Private _TipoRichiesto As Integer
    Private _TipiRichiesti As List(Of Integer)
    Private _TestoRicerca As String
    Private _FrCod As Integer
    Private _Veg_Cod As String
    Private _Flag_PrincipiAttivi As Boolean
    Private _Flag_ClasseTossicologica As Boolean
    Private _Flag_Classificazione As Boolean

    Private _Piva As String
    Private _Fabbricato_Cod As String
    Private _Cau_Mov As String
    Private _Validita_Inizio As Date
    Private _Validita_Fine As Date
    Private _FiltroAggiuntivo As String
    Private _OrderBy As String
    Private _FiltroRicerca As Integer

    Private _Disciplinare_Cod As String
    Private _Epoca_Cod As Integer
    Private _Modulo As Integer
    Private _Tipo_Testata As Integer


    Private _Opt_Avversita_Infestanti As Integer
    Private _Opt_Singola_Gruppo As Integer
    Private _Av_Gru(0) As Integer
    Private _Av_Cod(0) As Integer
    Private _StrAvversita As String
    Private _strPA As String
    Private _grfi_Cod As Integer
    Private _Copertura As String
    Private _Stato_Cod As String
    Private _Stato_Impianto As Integer
    Private _GestioneLotto As enum_Gestione_Lotti
    Private _GestioneGiacenze As enum_Gestione_Giacenze
    Private _EscludiGiacenzeZero As Boolean

    Private _FormulatiXAllegatiNormative_IDRiga As Integer

    Private _Bootstrap As Boolean
    Private _Ricerca As Boolean = True

    Private _WS_Disciplinari_AgroWS_Disciplinari As String
    Private _WS_Fitofarmaci_AgroWS_Fitofarmaci As String

    Private _ListaComuni As List(Of Comune)

    Public Sub New()

        _FiltroRicerca = 0
        _PrimaRiga_Flag = True
        _PrimaRiga_Text = ""
        _PrimaRiga_Value = ""
        _TipoRichiesto = 0
        _TestoRicerca = ""

        _Veg_Cod = 0
        _FrCod = 0
        _Flag_PrincipiAttivi = False
        _Flag_ClasseTossicologica = False
        _Flag_Classificazione = False

        _Piva = ""
        _Fabbricato_Cod = "0"
        _Cau_Mov = ""
        _Validita_Inizio = AGRODATAINIZIO
        _Validita_Fine = AGRODATAFINE
        _FiltroAggiuntivo = ""
        _OrderBy = ""

        _Disciplinare_Cod = "0"
        _Tipo_Testata = 0
        _Epoca_Cod = 0
        _Modulo = 0

        _Opt_Avversita_Infestanti = 0
        _Opt_Singola_Gruppo = -1
        _Av_Cod(0) = 0
        _Av_Gru(0) = 0
        _StrAvversita = ""
        _strPA = ""
        _grfi_Cod = 0
        _Copertura = ""
        _Stato_Cod = ""
        _Stato_Impianto = 0
        _FormulatiXAllegatiNormative_IDRiga = 0
        _WS_Disciplinari_AgroWS_Disciplinari = ""
        _WS_Fitofarmaci_AgroWS_Fitofarmaci = ""
        _Bootstrap = False

        _GestioneLotto = enum_Gestione_Lotti.Nessuna
        _GestioneGiacenze = enum_Gestione_Giacenze.SoloMovimentati
        _EscludiGiacenzeZero = True

        _TipiRichiesti = New List(Of Integer)

        _ListaComuni = New List(Of Comune)

    End Sub

    Protected Overrides Sub OnInit(ByVal e As System.EventArgs)

        ddl_Formulati = New DropDownList
        ddl_Formulati.ID = Me.ClientID & "ComboFormulati"

        If Not Bootstrap Then
            ddl_Formulati.CssClass = "ComboFormulati txtUI"
            ddl_Formulati.Attributes.Add("onchange", " CambiaFormulati(); ")
        Else
            ddl_Formulati.CssClass = "selectpicker ComboFormulati"
            If Ricerca Then
                ddl_Formulati.Attributes.Add("data-live-search", "true")
            End If

            ddl_Formulati.Attributes.Add("data-container", "body")
            ddl_Formulati.Attributes.Add("onchange", " CambiaFormulati(); ")
        End If

        Me.Controls.Add(ddl_Formulati)
        MyBase.OnInit(e)
    End Sub



#Region "Proprietà"
    Public Property Ricerca() As Boolean
        Get
            Return _Ricerca
        End Get
        Set(ByVal value As Boolean)
            _Ricerca = value
        End Set
    End Property

    Public Property Bootstrap() As Boolean
        Get
            Return _Bootstrap
        End Get
        Set(ByVal value As Boolean)
            _Bootstrap = value
        End Set
    End Property
    Public Property grfi_Cod() As Integer
        Get
            Return _grfi_Cod
        End Get
        Set(ByVal value As Integer)
            _grfi_Cod = value
        End Set
    End Property
    Public Property Copertura() As String
        Get
            Return _Copertura
        End Get
        Set(ByVal value As String)
            _Copertura = value
        End Set
    End Property
    Public Property Stato_Impianto() As Integer
        Get
            Return _Stato_Impianto
        End Get
        Set(ByVal value As Integer)
            _Stato_Impianto = value
        End Set
    End Property
    Public Property Stato_Cod() As String
        Get
            Return _Stato_Cod
        End Get
        Set(ByVal value As String)
            _Stato_Cod = value
        End Set
    End Property
    Public Property FormulatiXAllegatiNormative_IDRiga() As Integer
        Get
            Return _FormulatiXAllegatiNormative_IDRiga
        End Get
        Set(ByVal value As Integer)
            _FormulatiXAllegatiNormative_IDRiga = value
        End Set
    End Property

    Public Property FiltroRicerca() As Integer
        Get
            Return _FiltroRicerca
        End Get
        Set(ByVal value As Integer)
            _FiltroRicerca = value
        End Set
    End Property
    Public Property PrimaRiga_Flag() As Boolean
        Get
            Return _PrimaRiga_Flag
        End Get
        Set(ByVal value As Boolean)
            _PrimaRiga_Flag = value
        End Set
    End Property

    Public Property PrimaRiga_Text() As String
        Get
            Return _PrimaRiga_Text
        End Get
        Set(ByVal value As String)
            _PrimaRiga_Text = value
        End Set
    End Property

    Public Property PrimaRiga_Value() As String
        Get
            Return _PrimaRiga_Value
        End Get
        Set(ByVal value As String)
            _PrimaRiga_Value = value
        End Set
    End Property

    Public Property TipoRichiesto() As Integer
        Get
            Return _TipoRichiesto
        End Get
        Set(ByVal value As Integer)
            _TipoRichiesto = value
        End Set
    End Property

    Public Property TipiRichiesti() As List(Of Integer)
        Get
            Return _TipiRichiesti
        End Get
        Set(ByVal value As List(Of Integer))
            _TipiRichiesti = value
        End Set
    End Property

    Public Property ListaComuni() As List(Of Comune)
        Get
            Return _ListaComuni
        End Get
        Set(ByVal value As List(Of Comune))
            _ListaComuni = value
        End Set
    End Property

    Public Property TestoRicerca() As String
        Get
            Return _TestoRicerca
        End Get
        Set(ByVal value As String)
            _TestoRicerca = value
        End Set
    End Property

    Public Property FrCod() As Integer
        Get
            Return _FrCod
        End Get
        Set(ByVal value As Integer)
            _FrCod = value
        End Set
    End Property

    Public Property Veg_Cod() As String
        Get
            Return _Veg_Cod
        End Get
        Set(ByVal value As String)
            _Veg_Cod = value
        End Set
    End Property

    Public Property Flag_PrincipiAttivi() As Boolean
        Get
            Return _Flag_PrincipiAttivi
        End Get
        Set(ByVal value As Boolean)
            _Flag_PrincipiAttivi = value
        End Set
    End Property

    Public Property Flag_ClasseTossicologica() As Boolean
        Get
            Return _Flag_ClasseTossicologica
        End Get
        Set(ByVal value As Boolean)
            _Flag_ClasseTossicologica = value
        End Set
    End Property

    Public Property Flag_Classificazione() As Boolean
        Get
            Return _Flag_Classificazione
        End Get
        Set(ByVal value As Boolean)
            _Flag_Classificazione = value
        End Set
    End Property

    Public Property Piva() As String
        Get
            Return _Piva
        End Get
        Set(ByVal value As String)
            _Piva = value
        End Set
    End Property

    Public Property Fabbricato_Cod() As String
        Get
            Return _Fabbricato_Cod
        End Get
        Set(ByVal value As String)
            _Fabbricato_Cod = value
        End Set
    End Property

    Public Property Cau_Mov() As String
        Get
            Return _Cau_Mov
        End Get
        Set(ByVal value As String)
            _Cau_Mov = value
        End Set
    End Property
    Public Property Validita_Inizio() As Date
        Get
            Return _Validita_Inizio
        End Get
        Set(ByVal value As Date)
            _Validita_Inizio = value
        End Set
    End Property

    Public Property Validita_Fine() As Date
        Get
            Return _Validita_Fine
        End Get
        Set(ByVal value As Date)
            _Validita_Fine = value
        End Set
    End Property

    Public Property FiltroAggiuntivo() As String
        Get
            Return _FiltroAggiuntivo
        End Get
        Set(ByVal value As String)
            _FiltroAggiuntivo = value
        End Set
    End Property

    Public Property OrderBy() As String
        Get
            Return _OrderBy
        End Get
        Set(ByVal value As String)
            _OrderBy = value
        End Set
    End Property



    Public Property Disciplinare_Cod() As String
        Get
            Return _Disciplinare_Cod
        End Get
        Set(ByVal value As String)
            _Disciplinare_Cod = value
        End Set
    End Property

    Public Property Tipo_Testata() As Integer
        Get
            Return _Tipo_Testata
        End Get
        Set(ByVal value As Integer)
            _Tipo_Testata = value
        End Set
    End Property

    Public Property Epoca_Cod() As Integer
        Get
            Return _Epoca_Cod
        End Get
        Set(ByVal value As Integer)
            _Epoca_Cod = value
        End Set
    End Property

    Public Property Modulo() As Integer
        Get
            Return _Modulo
        End Get
        Set(ByVal value As Integer)
            _Modulo = value
        End Set
    End Property


    Public Property Opt_Avversita_Infestanti() As String
        Get
            Return _Opt_Avversita_Infestanti
        End Get
        Set(ByVal value As String)
            _Opt_Avversita_Infestanti = value
        End Set
    End Property

    Public Property Opt_Singola_Gruppo() As String
        Get
            Return _Opt_Singola_Gruppo
        End Get
        Set(ByVal value As String)
            _Opt_Singola_Gruppo = value
        End Set
    End Property

    Public Property StrAvversita() As String
        Get
            Return _StrAvversita
        End Get
        Set(ByVal value As String)
            _StrAvversita = value
        End Set
    End Property

    Public Property strPA() As String
        Get
            Return _strPA
        End Get
        Set(ByVal value As String)
            _strPA = value
        End Set
    End Property

    Public Property Av_Gru() As Integer()
        Get
            Return _Av_Gru
        End Get
        Set(ByVal value As Integer())
            _Av_Gru = value
        End Set
    End Property

    Public Property Av_Cod() As Integer()
        Get
            Return _Av_Cod
        End Get
        Set(ByVal value As Integer())
            _Av_Cod = value
        End Set
    End Property

    Public Property GestioneLotto() As enum_Gestione_Lotti
        Get
            Return _GestioneLotto
        End Get
        Set(ByVal value As enum_Gestione_Lotti)
            _GestioneLotto = value
        End Set
    End Property
    Public Property GestioneGiacenze() As enum_Gestione_Giacenze
        Get
            Return _GestioneGiacenze
        End Get
        Set(ByVal value As enum_Gestione_Giacenze)
            _GestioneGiacenze = value
        End Set
    End Property

    Public Property EscludiGiacenzeZero() As Boolean
        Get
            Return _EscludiGiacenzeZero
        End Get
        Set(ByVal value As Boolean)
            _EscludiGiacenzeZero = value
        End Set
    End Property

    Public Property WS_Disciplinari_AgroWS_Disciplinari() As String
        Get
            Return _WS_Disciplinari_AgroWS_Disciplinari
        End Get
        Set(ByVal value As String)
            _WS_Disciplinari_AgroWS_Disciplinari = value
        End Set
    End Property

    Public Property WS_Fitofarmaci_AgroWS_Fitofarmaci() As String
        Get
            Return _WS_Fitofarmaci_AgroWS_Fitofarmaci
        End Get
        Set(ByVal value As String)
            _WS_Fitofarmaci_AgroWS_Fitofarmaci = value
        End Set
    End Property


    Public Property Valore_Combo() As String
        Get
            Return ddl_Formulati.SelectedValue
        End Get
        Set(ByVal value As String)
            If Not IsNothing(value) Then
                If Not IsNothing(ddl_Formulati.Items.FindByValue(value)) Then
                    ddl_Formulati.Items.FindByValue(value).Selected = True
                End If
            End If
        End Set
    End Property

    Public Property Testo_Combo() As String
        Get
            If IsNothing(ddl_Formulati.SelectedItem) Then
                Return ""
            End If
            Return ddl_Formulati.SelectedItem.Text
        End Get
        Set(ByVal value As String)
            If Not IsNothing(value) Then
                If Not IsNothing(ddl_Formulati.Items.FindByText(value)) Then
                    ddl_Formulati.Items.FindByText(value).Selected = True
                End If
            End If
        End Set
    End Property

    Public Property N_Formulati() As Integer
        Get
            Return ddl_Formulati.Items.Count - 1
        End Get
        Set(ByVal value As Integer)
        End Set
    End Property
#End Region


    'NOTA
    'Il TipoRichiesto consente di selezionare solo i formulati specifici
    'per la particolare applicazione:
    '
    '   0 = Tutti i formulati
    '   1 = Trattamenti Antiparassitari
    '   2 = Diserbo
    '   3 = Trattamenti Fitoregolatori
    '   4 = Coadiuvanti, Bagnanti, Antischiuma
    '   5 = Concianti
    '   6 = Disseccanti
    '   7 = Geodisinfestanti
    '   12 = post-raccolta
    '   ecc...
    Public Sub CaricaComboFormulati()

        Dim strErr As String = ""

        Dim XmlDoc As New System.Xml.XmlDocument

        Dim Fr_Des As String = ""
        Dim Fr_Cod As String = ""
        Dim Carenza As String = ""
        Dim strCarenza As String = ""
        Dim strPA_Des As String = ""
        Dim InRevisione As String = ""
        Dim DataAttoNormativo As String = ""

        Dim strPA_DesTot As String = ""
        Dim strPA_COD As String = ""
        Dim strTITOLI As String = ""
        Dim strPrincipi As String = ""
        Dim Principi As String()
        Dim Titoli As String()
        Dim p As Integer

        Dim strClassToss As String = ""
        Dim strCLTOSS_COD As String = ""
        Dim strValue As String = ""

        Dim strGiacenza As String = ""


        If TipoRichiesto = enum_TipoFormulato.Coadiuvanti OrElse
           TipoRichiesto = enum_TipoFormulato.Corroboranti_Fisiofarmaci Then
            _Veg_Cod = 0
        End If

        If TipoRichiesto = enum_TipoFormulato.Coadiuvanti OrElse
           TipoRichiesto = enum_TipoFormulato.Fitoregolatori OrElse
           TipoRichiesto = enum_TipoFormulato.Corroboranti_Fisiofarmaci Then
            _Opt_Avversita_Infestanti = -1
            _Opt_Singola_Gruppo = 0
            Av_Cod = New Integer() {}
            Av_Gru = New Integer() {}
        End If



        ddl_Formulati.Items.Clear()

        If _PrimaRiga_Flag Then
            ddl_Formulati.Items.Add(New ListItem(_PrimaRiga_Text, _PrimaRiga_Value))
        End If


        Dim objParametri_Utenti As New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Utenti"))
        Dim objParametri_Server As New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))


        '-------------------------------------------------------------
        '--- Se utilizzo il magazzino
        '--- leggo prima i prodotti in giacenza
        '-------------------------------------------------------------
        Dim appoggio_prodotto_des As String
        Dim Dt_Giacenze As DataTable = Nothing
        Dim Dt_Giacenze_Tot As DataTable = Nothing
        Dim DtRisultati As DataTable
        Dim DtFormulatiOrdinati As New DataTable
        Dim Dr As DataRow
        Dim DrGiacenze As DataRow()
        Dim DrGiacenze_Tot As DataRow()

        DtFormulatiOrdinati.Columns.Add(New DataColumn("Testo", GetType(String)))
        DtFormulatiOrdinati.Columns.Add(New DataColumn("Valore", GetType(String)))


        If _Fabbricato_Cod <> "0" Then

            Select Case _Cau_Mov

                Case CAU_SCARICO

                    Dim objG As New AgronicaCoreStampeDAL.Magazzino
                    Dim strFiltroFrCod As String = ""

                    Dt_Giacenze = objG.SchedaGiacenzeMagazzino(_Validita_Fine,
                                         Split(_Fabbricato_Cod, "|")(2),
                                         Split(_Fabbricato_Cod, "|")(1),
                                         Split(_Fabbricato_Cod, "|")(0),
                                         191,
                                         _FrCod,
                                         0, 0, 0, 0, 0,
                                         LOTTO_NONDEFINITO,
                                         False,
                                         " and Formulati.Fr_Des like '%" & _TestoRicerca & "%'", "", "", "", "", "", "", "", "", "", "",
                                         "",
                                         "",
                                         objParametri_Server, objParametri_Utenti)

                    Dt_Giacenze_Tot = objG.SchedaGiacenzeMagazzino(AGRODATAFINE,
                                       Split(_Fabbricato_Cod, "|")(2),
                                       Split(_Fabbricato_Cod, "|")(1),
                                       Split(_Fabbricato_Cod, "|")(0),
                                       191,
                                       _FrCod,
                                       0, 0, 0, 0, 0,
                                       LOTTO_NONDEFINITO,
                                       False,
                                       " and Formulati.Fr_Des like '%" & _TestoRicerca & "%'", "", "", "", "", "", "", "", "", "", "",
                                       "",
                                       "",
                                       objParametri_Server, objParametri_Utenti)

                    objG = Nothing

                    If FrCod <> 0 Then
                        If Dt_Giacenze.Rows.Count > 0 Then
                            appoggio_prodotto_des = Dt_Giacenze.Rows(0).Item("Descrizione_Prodotto")
                        End If
                    End If

                    For i = 0 To Dt_Giacenze.Rows.Count - 1

                            If Not (TipoRichiesto = 3 OrElse TipoRichiesto = 4 OrElse TipoRichiesto = 6 OrElse TipoRichiesto = 11) Then
                                If FiltroRicerca = 0 Then
                                    strFiltroFrCod &= Dt_Giacenze.Rows(i).Item("Pro_Cod").ToString & " ,"
                                Else
                                    strFiltroFrCod &= " Formulati.FR_COD=" & Dt_Giacenze.Rows(i).Item("Pro_Cod").ToString & " OR "
                                End If
                            Else
                                strFiltroFrCod &= " Formulati.FR_COD=" & Dt_Giacenze.Rows(i).Item("Pro_Cod").ToString & " OR "
                            End If
                    Next

                    If strFiltroFrCod <> "" Then
                        If Not (TipoRichiesto = 3 OrElse TipoRichiesto = 4 OrElse TipoRichiesto = 6 OrElse TipoRichiesto = 11) Then
                            If FiltroRicerca = 0 Then
                                strFiltroFrCod = Left(strFiltroFrCod, strFiltroFrCod.Length - 2)
                                strFiltroFrCod = " (" & strFiltroFrCod & ")"
                            Else
                                strFiltroFrCod = Left(strFiltroFrCod, strFiltroFrCod.Length - 3)
                                strFiltroFrCod = " AND (" & strFiltroFrCod & ")"
                            End If
                        Else
                            strFiltroFrCod = Left(strFiltroFrCod, strFiltroFrCod.Length - 3)
                            strFiltroFrCod = " AND (" & strFiltroFrCod & ")"
                        End If
                    Else
                        'se non ho alcun formulato in magazzino
                        Exit Sub
                    End If

                    _FiltroAggiuntivo &= strFiltroFrCod

                    'Dt_Giacenze = Nothing

            End Select

        End If



        Dim XML_Credenziali As System.Xml.XmlElement
        Dim StrCredenziali As String = ""
        Dim StrParametri As String = ""
        Dim Parametri As String = ""


        Select Case TipoRichiesto

            Case 4, 3, 11, 6    'Coadiuvanti,Fitoregolatori,corroboranti/fisiofarmaci, Dissecamento


                Try

                    Dim ObjDownloadWs As New WS_Fitofarmaci.AgroWS_Fitofarmaci
                    Dim objWs As New AgronicaCoreVarieDAL.ConnessioneWS

                    objWs.NewWS(ObjDownloadWs,
                                    _WS_Fitofarmaci_AgroWS_Fitofarmaci,
                                    objParametri_Utenti)

                    XmlDoc = New System.Xml.XmlDocument

                    Dim objCoreAgroWs As New AgronicaCoreWebService.AgroWs

                    objCoreAgroWs.AgroWS_XML__Credenziali(AgronicaCoreWebService.AgroWs.enum_AWS_Xml.Xml_CODIFICA,
                                                    StrCredenziali,
                                                    AgronicaCoreWebService.AgroWs.enum_AWS_Schede.AWS_Fitofarmaci_Formulato_Completo,
                                                    objCoreAgroWs.AgroWS_DoorKey(AgronicaCoreWebService.AgroWs.enum_AWS_Schede.AWS_Fitofarmaci_Formulato_Completo),
                                                    HttpContext.Current.Session("ASG_ProgressivoGIAS"),
                                                    HttpContext.Current.Session("ASG_SuperUser_CodFiscale").ToString,
                                                    HttpContext.Current.Session("ASG_SuperUser_Username").ToString,
                                                    HttpContext.Current.Session("ASG_SuperUser_Password").ToString,
                                                    HttpContext.Current.Session("ASG_Utente_Username").ToString,
                                                    HttpContext.Current.Session("ASG_Utente_Password").ToString)


                    XmlDoc.LoadXml(StrCredenziali)

                    XML_Credenziali = XmlDoc.SelectSingleNode("CREDENZIALI")

                    objCoreAgroWs.AgroWS_XML_Parametri_Formulati_ConDosi_Profitosan(AgronicaCoreWebService.AgroWs.enum_AWS_Xml.Xml_CODIFICA,
                                                                                     StrParametri,
                                                                                     enum_AWS_ApplicazioneRichiedente.AgronicaAgenda,
                                                                                     _Veg_Cod,
                                                                                     0, 0,
                                                                                     0,
                                                                                     0,
                                                                                     _Opt_Avversita_Infestanti,
                                                                                     _Opt_Singola_Gruppo,
                                                                                     _Av_Gru,
                                                                                     _Av_Cod,
                                                                                     "",
                                                                                     0,
                                                                                     0,
                                                                                     _strPA,
                                                                                      _TipoRichiesto,
                                                                                      _TestoRicerca,
                                                                                      _Validita_Fine,
                                                                                      _FiltroAggiuntivo,
                                                                                      "",
                                                                                      strErr,
                                                                                      grfi_Cod,
                                                                                      0,
                                                                                      "",
                                                                                      0,
                                                                                      "",
                                                                                      _Stato_Cod,
                                                                                      objParametri_Server.Lingua_Cod,
                                                                                      Stato_Impianto)

                    XML_Credenziali.InnerXml = StrParametri

                    Parametri = XmlDoc.OuterXml

                    Parametri = objCoreAgroWs.AWS_Codifica_P(Parametri)

                    Select Case _Disciplinare_Cod
                        Case "-2"
                            DtRisultati = ObjDownloadWs.Leggi_FormulatiBio_ConDosi_DT_New(Parametri, strErr)
                        Case Else
                            DtRisultati = ObjDownloadWs.Leggi_Formulati_ConDosi_DT_New(Parametri, strErr)
                    End Select

                    If strErr = "" Then

                        Dim objDistinct As New AgronicaCoreUtility.DatatableUtility
                        Dim DrFrCod As DataRow()
                        Dim DrFrDosi As DataRow()

                        '--------------------------------
                        'Seleziono i Formulati Distinti
                        '--------------------------------
                        Dim strFrCod As String() = objDistinct.SelectDistinct(DtRisultati, "Fr_Cod")

                        If strFrCod IsNot Nothing AndAlso strFrCod.Length > 0 Then

                            For i = 0 To strFrCod.Length - 1

                                Dim DtFormulato As New DataTable
                                Dim DtDosi As New DataTable

                                DtFormulato = DtRisultati.Clone
                                DtDosi = DtRisultati.Clone

                                DrFrCod = DtRisultati.Select("Fr_Cod=" & strFrCod(i))

                                If DrFrCod IsNot Nothing AndAlso DrFrCod.Length > 0 Then

                                    For j = 0 To DrFrCod.Length - 1
                                        DtFormulato.ImportRow(DrFrCod(j))
                                    Next

                                    If DtFormulato IsNot Nothing AndAlso DtFormulato.Rows.Count > 0 Then

                                        Fr_Des = DtFormulato.Rows(0).Item("fr_des")
                                        Fr_Cod = DtFormulato.Rows(0).Item("fr_cod")

                                        Carenza = ""
                                        strCarenza = ""
                                        If Not IsDBNull(DtFormulato.Rows(0).Item("tempocarenza")) AndAlso DtFormulato.Rows(0).Item("tempocarenza") <> 0 Then
                                            Carenza = DtFormulato.Rows(0).Item("tempocarenza")
                                            strCarenza = " --- [Carenza " & Carenza & " gg]"
                                        End If

                                        DataAttoNormativo = ""
                                        InRevisione = ""
                                        If Not IsDBNull(DtFormulato.Rows(0).Item("Verificato_Flag")) AndAlso DtFormulato.Rows(0).Item("Verificato_Flag") = 15 Then
                                            InRevisione = "1"
                                            If Not IsDBNull(DtFormulato.Rows(0).Item("DataAttoNormativo")) AndAlso IsDate(DtFormulato.Rows(0).Item("DataAttoNormativo")) Then
                                                DataAttoNormativo = CDate(DtFormulato.Rows(0).Item("DataAttoNormativo")).ToShortDateString
                                            End If
                                        End If

                                        'se è richiesto aggiungo il principio attivo principale
                                        If _Flag_PrincipiAttivi Then
                                            strPA_Des = ""
                                            strPA_DesTot = ""
                                            strPA_COD = ""
                                            strTITOLI = ""
                                            strPrincipi = ""
                                            If Not IsDBNull(DtFormulato.Rows(0).Item("pa_des")) AndAlso CStr(DtFormulato.Rows(0).Item("pa_des")) <> "" Then
                                                strPA_Des = " --- [" & (CStr(DtFormulato.Rows(0).Item("pa_des"))) & "]"
                                            End If
                                            If Not IsDBNull(DtFormulato.Rows(0).Item("strpa_des")) AndAlso CStr(DtFormulato.Rows(0).Item("strpa_des")) <> "" Then
                                                strPA_DesTot = " --- [" & (CStr(DtFormulato.Rows(0).Item("strpa_des"))) & "]"
                                            End If
                                            If Not IsDBNull(DtFormulato.Rows(0).Item("strPA_COD")) AndAlso CStr(DtFormulato.Rows(0).Item("strPA_COD")) <> "" Then
                                                strPA_COD = CStr(DtFormulato.Rows(0).Item("strPA_COD"))
                                                Principi = Split(strPA_COD, "|")
                                                If Not IsDBNull(DtFormulato.Rows(0).Item("strTITOLI")) AndAlso CStr(DtFormulato.Rows(0).Item("strTITOLI")) <> "" Then
                                                    strTITOLI = CStr(DtFormulato.Rows(0).Item("strTITOLI"))
                                                    Titoli = Split(strTITOLI, "|")
                                                    If Principi IsNot Nothing AndAlso Principi.Length > 0 Then
                                                        For p = 0 To Principi.Length - 1
                                                            strPrincipi &= Principi(p) & "§" & Titoli(p) & "|"
                                                        Next
                                                    End If
                                                End If
                                            End If
                                            If strPrincipi <> "" Then
                                                strPrincipi = Left(strPrincipi, strPrincipi.Length - 1)
                                            End If
                                        End If
                                        'se è richiesto aggiungo le classi tossicologiche
                                        If _Flag_ClasseTossicologica Then
                                            strClassToss = ""
                                            strCLTOSS_COD = ""
                                            If Not IsDBNull(DtFormulato.Rows(0).Item("strCLTOSS_COD")) AndAlso CStr(DtFormulato.Rows(0).Item("strCLTOSS_COD")) <> "" Then
                                                strClassToss = " --- [" & (CStr(DtFormulato.Rows(0).Item("strCLTOSS_COD"))) & "]"
                                                strCLTOSS_COD = CStr(DtFormulato.Rows(0).Item("strCLTOSS_COD"))
                                            End If
                                        End If

                                        Dim strDosi As String() = objDistinct.SelectDistinct(DtFormulato, "For_Veg_Av_Dos_Cod")
                                        Dim strdose As String = ""
                                        If strDosi IsNot Nothing AndAlso strDosi.Length > 0 Then
                                            For d = 0 To strDosi.Length - 1
                                                DrFrDosi = DtFormulato.Select("For_Veg_Av_Dos_Cod=" & strDosi(d))
                                                If DrFrDosi IsNot Nothing AndAlso DrFrDosi.Length > 0 Then
                                                    'ho delle dosi doppie xkè alcuni coadiuvanti hanno inserite le specie
                                                    'x cui visualizzo solo le dosi diverse
                                                    If InStr(strdose, DrFrDosi(0).Item("dose_min").ToString & "$" &
                                                               DrFrDosi(0).Item("dose_max").ToString & "$" &
                                                               DrFrDosi(0).Item("udm_cod").ToString & "$" &
                                                               DrFrDosi(0).Item("udm_sim").ToString & "$" &
                                                               DrFrDosi(0).Item("acqua_min").ToString & "$" &
                                                               DrFrDosi(0).Item("acqua_max").ToString & "$" &
                                                               DrFrDosi(0).Item("acqua_udm_cod").ToString & "$" &
                                                               DrFrDosi(0).Item("acqua_udm_sim").ToString & "$" &
                                                               DrFrDosi(0).Item("da_epoca_1").ToString & "$" &
                                                               DrFrDosi(0).Item("a_epoca_1").ToString & "$" &
                                                               DrFrDosi(0).Item("LimiteInterventi").ToString & "$" &
                                                               DrFrDosi(0).Item("UDM_COD_Limite").ToString & "$" &
                                                               DrFrDosi(0).Item("udm_sim_Limite").ToString & "$" &
                                                                DrFrDosi(0).Item("strCLTOSS_Grado").ToString & "$" &
                                                                DrFrDosi(0).Item("epoca_cod").ToString & "$" &
                                                                           DrFrDosi(0).Item("IntervalloTrattamenti_Min").ToString & "$" &
                                                                           DrFrDosi(0).Item("IntervalloTrattamenti_Max").ToString & "$" &
                                                                           DrFrDosi(0).Item("BufferZone_Min").ToString & "$" &
                                                                           DrFrDosi(0).Item("BufferZone_Max").ToString & "£") = 0 Then

                                                        strdose &= strDosi(d) & "$" &
                                                                   DrFrDosi(0).Item("dose_min").ToString & "$" &
                                                                   DrFrDosi(0).Item("dose_max").ToString & "$" &
                                                                   DrFrDosi(0).Item("udm_cod").ToString & "$" &
                                                                   DrFrDosi(0).Item("udm_sim").ToString & "$" &
                                                                   DrFrDosi(0).Item("acqua_min").ToString & "$" &
                                                                   DrFrDosi(0).Item("acqua_max").ToString & "$" &
                                                                   DrFrDosi(0).Item("acqua_udm_cod").ToString & "$" &
                                                                   DrFrDosi(0).Item("acqua_udm_sim").ToString & "$" &
                                                                   DrFrDosi(0).Item("da_epoca_1").ToString & "$" &
                                                                   DrFrDosi(0).Item("a_epoca_1").ToString & "$" &
                                                                   DrFrDosi(0).Item("LimiteInterventi").ToString & "$" &
                                                                   DrFrDosi(0).Item("UDM_COD_Limite").ToString & "$" &
                                                                   DrFrDosi(0).Item("udm_sim_Limite").ToString & "$" &
                                                                    DrFrDosi(0).Item("strCLTOSS_Grado").ToString & "$" &
                                                                    DrFrDosi(0).Item("epoca_cod").ToString & "$" &
                                                                           DrFrDosi(0).Item("IntervalloTrattamenti_Min").ToString & "$" &
                                                                           DrFrDosi(0).Item("IntervalloTrattamenti_Max").ToString & "$" &
                                                                           DrFrDosi(0).Item("BufferZone_Min").ToString & "$" &
                                                                           DrFrDosi(0).Item("BufferZone_Max").ToString & "£"

                                                    End If
                                                End If
                                            Next
                                        End If
                                        If strdose <> "" Then
                                            strdose = Left(strdose, strdose.Length - 1)
                                        End If

                                        strValue = Fr_Cod & "£" &
                                                   InRevisione & "£" &
                                                   DataAttoNormativo & "£" &
                                                   Carenza & "£" &
                                                   strPrincipi & "£" &
                                                   strCLTOSS_COD & "£" &
                                                   strdose

                                        strGiacenza = ""
                                        If Dt_Giacenze IsNot Nothing AndAlso Dt_Giacenze.Rows.Count > 0 Then
                                            DrGiacenze = Dt_Giacenze.Select("pro_cod=" & Fr_Cod)
                                            If DrGiacenze IsNot Nothing AndAlso DrGiacenze.Length > 0 Then
                                                strGiacenza = " --- GIACENZA: alla data " & Math.Round(DrGiacenze(0).Item("Giacenza"), 4).ToString & " " & DrGiacenze(0).Item("Udm_Sim")
                                            End If
                                        End If
                                        If Dt_Giacenze_Tot IsNot Nothing AndAlso Dt_Giacenze_Tot.Rows.Count > 0 Then
                                            DrGiacenze_Tot = Dt_Giacenze_Tot.Select("pro_cod=" & Fr_Cod)
                                            If DrGiacenze_Tot IsNot Nothing AndAlso DrGiacenze_Tot.Length > 0 Then
                                                strGiacenza &= " totale " & Math.Round(DrGiacenze_Tot(0).Item("Giacenza"), 4).ToString & " " & DrGiacenze_Tot(0).Item("Udm_Sim")
                                            End If
                                        End If
                                        Dr = DtFormulatiOrdinati.NewRow
                                        Dr.Item("Testo") = Fr_Des & " (" & Fr_Cod & ")" & strPA_Des & strCarenza & strClassToss & strGiacenza
                                        Dr.Item("Valore") = strValue
                                        DtFormulatiOrdinati.Rows.Add(Dr)

                                    End If

                                End If

                            Next

                        End If

                    End If


                    If DtFormulatiOrdinati IsNot Nothing Then

                        'uso il dataview per ordinare 
                        Dim Dv As New DataView

                        DtFormulatiOrdinati.TableName = "Prodotti"
                        Dv.Table = DtFormulatiOrdinati
                        Dv.Sort = "Testo ASC"

                        For i = 0 To Dv.Count - 1
                            ddl_Formulati.Items.Add(New ListItem(Dv(i).Item("Testo"),
                                                    Dv(i).Item("Valore")))
                        Next

                    End If


                Catch ex As Exception

                    Dim pippo As String = ex.Message

                End Try

            Case Else


                '########################################################################################################################
                'se ho filtro ricerca = 0 (nessuno nessuno)
                'I formulati vengono filtrati x classificazione e date revoca, sosp etc.
                '########################################################################################################################

                If FiltroRicerca = 0 Then

                    If FrCod <> 0 Then
                        _TestoRicerca = appoggio_prodotto_des
                    End If

                    If _Fabbricato_Cod <> "0" Then

                        If _FiltroAggiuntivo <> "" Then
                            _FiltroAggiuntivo = _FiltroAggiuntivo.Replace("AND", "")
                            _FiltroAggiuntivo = _FiltroAggiuntivo.Replace("(", "")
                            _FiltroAggiuntivo = _FiltroAggiuntivo.Replace(")", "")
                        End If

                        Dim clc = New AgronicaCoreVarieBIZ.CaricaListControl_2010
                        clc.Prodotti(ddl_Formulati,
                                                      True,
                                                      "",
                                                      "",
                                                      CAU_SCARICO,
                                                      _Piva,
                                                      Split(_Fabbricato_Cod, "|")(1),
                                                      Split(_Fabbricato_Cod, "|")(0),
                                                      FORMULATI,
                                                      False,
                                                      _TestoRicerca,
                                                      True,
                                                      False,
                                                      0, 0,
                                                      _Validita_Fine,
                                                      0,
                                                      _TipoRichiesto,
                                                      True,
                                                      True,
                                                      _FiltroAggiuntivo,
                                                      "",
                                                      objParametri_Server,
                                                      objParametri_Utenti)

                        If FrCod <> 0 Then
                            Dim j As Integer
                            For j = ddl_Formulati.Items.Count - 1 To 0 Step -1
                                If ddl_Formulati.Items(j).Value.Trim <> FrCod.ToString.Trim Then
                                    ddl_Formulati.Items.RemoveAt(j)
                                End If
                            Next
                        End If

                    Else

                        If FrCod <> 0 Then
                            _FiltroAggiuntivo = FrCod.ToString
                        End If

                        Dim clc = New AgronicaCoreVarieBIZ.CaricaListControl_2010
                        clc.Prodotti(ddl_Formulati,
                                                                              True,
                                                                              "",
                                                                              "",
                                                                              CAU_SCARICO,
                                                                              "",
                                                                              0,
                                                                              0,
                                                                              FORMULATI,
                                                                              False,
                                                                              _TestoRicerca,
                                                                              True,
                                                                              False,
                                                                              0, 0,
                                                                              _Validita_Fine,
                                                                              0,
                                                                              _TipoRichiesto,
                                                                              False,
                                                                              True,
                                                                              _FiltroAggiuntivo,
                                                                              "",
                                                                              objParametri_Server,
                                                                              objParametri_Utenti)


                        If FrCod <> 0 Then
                            Dim j As Integer
                            For j = ddl_Formulati.Items.Count - 1 To 0 Step -1
                                If ddl_Formulati.Items(j).Value.Trim <> FrCod.ToString.Trim Then
                                    ddl_Formulati.Items.RemoveAt(j)
                                End If
                            Next
                        End If

                    End If

                Else

                    '########################################################################################################################
                    ' Leggo i Formulati dal web service
                    '########################################################################################################################


                    Select Case _Disciplinare_Cod

                        Case "0", "-2"


                            Dim LastFr_Des As String = ""
                            Dim i As Integer

                            Try

                                Dim ObjDownloadWs As New WS_Fitofarmaci.AgroWS_Fitofarmaci
                                Dim objWs As New AgronicaCoreVarieDAL.ConnessioneWS

                                objWs.NewWS(ObjDownloadWs,
                                                _WS_Fitofarmaci_AgroWS_Fitofarmaci,
                                                objParametri_Utenti)

                                XmlDoc = New System.Xml.XmlDocument

                                Dim objCoreAgroWs As New AgronicaCoreWebService.AgroWs

                                objCoreAgroWs.AgroWS_XML__Credenziali(AgronicaCoreWebService.AgroWs.enum_AWS_Xml.Xml_CODIFICA,
                                                                StrCredenziali,
                                                                AgronicaCoreWebService.AgroWs.enum_AWS_Schede.AWS_Fitofarmaci_Formulato_Completo,
                                                                objCoreAgroWs.AgroWS_DoorKey(AgronicaCoreWebService.AgroWs.enum_AWS_Schede.AWS_Fitofarmaci_Formulato_Completo),
                                                                HttpContext.Current.Session("ASG_ProgressivoGIAS"),
                                                                HttpContext.Current.Session("ASG_SuperUser_CodFiscale").ToString,
                                                                HttpContext.Current.Session("ASG_SuperUser_Username").ToString,
                                                                HttpContext.Current.Session("ASG_SuperUser_Password").ToString,
                                                                HttpContext.Current.Session("ASG_Utente_Username").ToString,
                                                                HttpContext.Current.Session("ASG_Utente_Password").ToString)

                                XmlDoc.LoadXml(StrCredenziali)

                                XML_Credenziali = XmlDoc.SelectSingleNode("CREDENZIALI")

                                'Dim grfi_Cod As Integer = 0
                                objCoreAgroWs.AgroWS_XML_Parametri_Formulati_ConDosi_Profitosan(AgronicaCoreWebService.AgroWs.enum_AWS_Xml.Xml_CODIFICA,
                                                                         StrParametri,
                                                                         enum_AWS_ApplicazioneRichiedente.AgronicaAgenda,
                                                                         _Veg_Cod,
                                                                         0, 0,
                                                                         0,
                                                                         0,
                                                                         _Opt_Avversita_Infestanti,
                                                                         _Opt_Singola_Gruppo,
                                                                         _Av_Gru,
                                                                         _Av_Cod,
                                                                         "",
                                                                         0,
                                                                         0,
                                                                         _strPA,
                                                                          _TipoRichiesto,
                                                                          _TestoRicerca,
                                                                          _Validita_Fine,
                                                                          _FiltroAggiuntivo,
                                                                          "",
                                                                          strErr,
                                                                          grfi_Cod,
                                                                          0, "", 0, "",
                                                                          _Stato_Cod,
                                                                          objParametri_Server.Lingua_Cod)

                                XML_Credenziali.InnerXml = StrParametri

                                Parametri = XmlDoc.OuterXml

                                Parametri = objCoreAgroWs.AWS_Codifica_P(Parametri)

                                If _Disciplinare_Cod = "0" Then
                                    DtRisultati = ObjDownloadWs.Leggi_Formulati_ConDosi_DT_New(Parametri, strErr)
                                Else
                                    DtRisultati = ObjDownloadWs.Leggi_FormulatiBio_ConDosi_DT_New(Parametri, strErr)
                                End If

                                ObjDownloadWs.Dispose()

                                If strErr = "" Then

                                    Dim objDistinct As New AgronicaCoreUtility.DatatableUtility
                                    Dim DrFrCod As DataRow()
                                    Dim DrFrDosi As DataRow()

                                    '--------------------------------
                                    'Seleziono i Formulati Distinti
                                    '--------------------------------
                                    Dim strFrCod As String() = objDistinct.SelectDistinct(DtRisultati, "Fr_Cod")

                                    If strFrCod IsNot Nothing AndAlso strFrCod.Length > 0 Then

                                        For i = 0 To strFrCod.Length - 1

                                            Dim DtFormulato As New DataTable
                                            Dim DtDosi As New DataTable

                                            DtFormulato = DtRisultati.Clone
                                            DtDosi = DtRisultati.Clone

                                            DrFrCod = DtRisultati.Select("Fr_Cod=" & strFrCod(i))

                                            If DrFrCod IsNot Nothing AndAlso DrFrCod.Length > 0 Then

                                                For j = 0 To DrFrCod.Length - 1
                                                    DtFormulato.ImportRow(DrFrCod(j))
                                                Next

                                                If DtFormulato IsNot Nothing AndAlso DtFormulato.Rows.Count > 0 Then

                                                    Fr_Des = DtFormulato.Rows(0).Item("fr_des")
                                                    Fr_Cod = DtFormulato.Rows(0).Item("fr_cod")

                                                    Carenza = ""
                                                    strCarenza = ""
                                                    If Not IsDBNull(DtFormulato.Rows(0).Item("tempocarenza")) AndAlso DtFormulato.Rows(0).Item("tempocarenza") <> 0 Then
                                                        Carenza = DtFormulato.Rows(0).Item("tempocarenza")
                                                        strCarenza = " --- [Carenza " & Carenza & " gg]"
                                                    End If

                                                    DataAttoNormativo = ""
                                                    InRevisione = ""
                                                    If Not IsDBNull(DtFormulato.Rows(0).Item("Verificato_Flag")) AndAlso DtFormulato.Rows(0).Item("Verificato_Flag") = 15 Then
                                                        InRevisione = "1"
                                                        If Not IsDBNull(DtFormulato.Rows(0).Item("DataAttoNormativo")) AndAlso IsDate(DtFormulato.Rows(0).Item("DataAttoNormativo")) Then
                                                            DataAttoNormativo = CDate(DtFormulato.Rows(0).Item("DataAttoNormativo")).ToShortDateString
                                                        End If
                                                    End If

                                                    'se è richiesto aggiungo il principio attivo principale
                                                    If _Flag_PrincipiAttivi Then
                                                        strPA_Des = ""
                                                        strPA_DesTot = ""
                                                        strPA_COD = ""
                                                        strTITOLI = ""
                                                        strPrincipi = ""
                                                        If Not IsDBNull(DtFormulato.Rows(0).Item("pa_des")) AndAlso CStr(DtFormulato.Rows(0).Item("pa_des")) <> "" Then
                                                            strPA_Des = " --- [" & (CStr(DtFormulato.Rows(0).Item("pa_des"))) & "]"
                                                        End If
                                                        If Not IsDBNull(DtFormulato.Rows(0).Item("strpa_des")) AndAlso CStr(DtFormulato.Rows(0).Item("strpa_des")) <> "" Then
                                                            strPA_DesTot = " --- [" & (CStr(DtFormulato.Rows(0).Item("strpa_des"))) & "]"
                                                        End If
                                                        If Not IsDBNull(DtFormulato.Rows(0).Item("strPA_COD")) AndAlso CStr(DtFormulato.Rows(0).Item("strPA_COD")) <> "" Then
                                                            strPA_COD = CStr(DtFormulato.Rows(0).Item("strPA_COD"))
                                                            Principi = Split(strPA_COD, "|")
                                                            If Not IsDBNull(DtFormulato.Rows(0).Item("strTITOLI")) AndAlso CStr(DtFormulato.Rows(0).Item("strTITOLI")) <> "" Then
                                                                strTITOLI = CStr(DtFormulato.Rows(0).Item("strTITOLI"))
                                                                Titoli = Split(strTITOLI, "|")
                                                                If Principi IsNot Nothing AndAlso Principi.Length > 0 Then
                                                                    For p = 0 To Principi.Length - 1
                                                                        strPrincipi &= Principi(p) & "§" & Titoli(p) & "|"
                                                                    Next
                                                                End If
                                                            End If
                                                        End If
                                                        If strPrincipi <> "" Then
                                                            strPrincipi = Left(strPrincipi, strPrincipi.Length - 1)
                                                        End If
                                                    End If

                                                    'se è richiesto aggiungo le classi tossicologiche
                                                    If _Flag_ClasseTossicologica Then
                                                        strClassToss = ""
                                                        strCLTOSS_COD = ""
                                                        If Not IsDBNull(DtFormulato.Rows(0).Item("strCLTOSS_COD")) AndAlso CStr(DtFormulato.Rows(0).Item("strCLTOSS_COD")) <> "" Then
                                                            strClassToss = " --- [" & (CStr(DtFormulato.Rows(0).Item("strCLTOSS_COD"))) & "]"
                                                            strCLTOSS_COD = CStr(DtFormulato.Rows(0).Item("strCLTOSS_COD"))
                                                        End If
                                                    End If

                                                    Dim strDosi As String() = objDistinct.SelectDistinct(DtFormulato, "For_Veg_Av_Dos_Cod")
                                                    Dim strdose As String = ""
                                                    If strDosi IsNot Nothing AndAlso strDosi.Length > 0 Then
                                                        For d = 0 To strDosi.Length - 1
                                                            DrFrDosi = DtFormulato.Select("For_Veg_Av_Dos_Cod=" & strDosi(d))
                                                            If DrFrDosi IsNot Nothing AndAlso DrFrDosi.Length > 0 Then
                                                                strdose &= strDosi(d) & "$" &
                                                                           DrFrDosi(0).Item("dose_min").ToString & "$" &
                                                                           DrFrDosi(0).Item("dose_max").ToString & "$" &
                                                                           DrFrDosi(0).Item("udm_cod").ToString & "$" &
                                                                           DrFrDosi(0).Item("udm_sim").ToString & "$" &
                                                                           DrFrDosi(0).Item("acqua_min").ToString & "$" &
                                                                           DrFrDosi(0).Item("acqua_max").ToString & "$" &
                                                                           DrFrDosi(0).Item("acqua_udm_cod").ToString & "$" &
                                                                           DrFrDosi(0).Item("acqua_udm_sim").ToString & "$" &
                                                                           DrFrDosi(0).Item("da_epoca_1").ToString & "$" &
                                                                           DrFrDosi(0).Item("a_epoca_1").ToString & "$" &
                                                                           DrFrDosi(0).Item("LimiteInterventi").ToString & "$" &
                                                                           DrFrDosi(0).Item("UDM_COD_Limite").ToString & "$" &
                                                                           DrFrDosi(0).Item("udm_sim_Limite").ToString & "$" &
                                                                           DrFrDosi(0).Item("strCLTOSS_Grado").ToString & "$" &
                                                                           DrFrDosi(0).Item("epoca_cod").ToString & "$" &
                                                                            DrFrDosi(0).Item("IntervalloTrattamenti_Min").ToString & "$" &
                                                                           DrFrDosi(0).Item("IntervalloTrattamenti_Max").ToString & "$" &
                                                                           DrFrDosi(0).Item("BufferZone_Min").ToString & "$" &
                                                                           DrFrDosi(0).Item("BufferZone_Max").ToString & "£"

                                                            End If
                                                        Next
                                                    End If
                                                    If strdose <> "" Then
                                                        strdose = Left(strdose, strdose.Length - 1)
                                                    End If

                                                    strValue = Fr_Cod & "£" &
                                                               InRevisione & "£" &
                                                               DataAttoNormativo & "£" &
                                                               Carenza & "£" &
                                                               strPrincipi & "£" &
                                                               strCLTOSS_COD & "£" &
                                                               strdose

                                                    strGiacenza = ""
                                                    If Dt_Giacenze IsNot Nothing AndAlso Dt_Giacenze.Rows.Count > 0 Then
                                                        DrGiacenze = Dt_Giacenze.Select("pro_cod=" & Fr_Cod)
                                                        If DrGiacenze IsNot Nothing AndAlso DrGiacenze.Length > 0 Then
                                                            strGiacenza = " --- GIACENZA: alla data " & Math.Round(DrGiacenze(0).Item("Giacenza"), 4).ToString & " " & DrGiacenze(0).Item("Udm_Sim")
                                                        End If
                                                    End If
                                                    If Dt_Giacenze_Tot IsNot Nothing AndAlso Dt_Giacenze_Tot.Rows.Count > 0 Then
                                                        DrGiacenze_Tot = Dt_Giacenze_Tot.Select("pro_cod=" & Fr_Cod)
                                                        If DrGiacenze_Tot IsNot Nothing AndAlso DrGiacenze_Tot.Length > 0 Then
                                                            strGiacenza &= " totale " & Math.Round(DrGiacenze_Tot(0).Item("Giacenza"), 4).ToString & " " & DrGiacenze_Tot(0).Item("Udm_Sim")
                                                        End If
                                                    End If
                                                    Dr = DtFormulatiOrdinati.NewRow
                                                    Dr.Item("Testo") = Fr_Des & " (" & Fr_Cod & ")" & strPA_Des & strCarenza & strClassToss & strGiacenza
                                                    Dr.Item("Valore") = strValue
                                                    DtFormulatiOrdinati.Rows.Add(Dr)

                                                End If

                                            End If

                                        Next

                                    End If

                                End If

                            Catch ex As Exception

                                Dim err As String
                                err = ex.Message

                            End Try



                        Case Else

                            Dim Array As String()
                            Dim Dpi_Cod As Integer = 0
                            Dim IdRcdpi As Integer = 0
                            'Dim Grfi_Cod As Integer = 0
                            Dim Flag_Protetto As Integer = 0
                            Dim Flag_PubblicoPrivato As Integer = 0

                            Array = Split(_Disciplinare_Cod, "/")
                            Dpi_Cod = Array(0)
                            IdRcdpi = Array(1)
                            'Grfi_Cod = Array(2)
                            Flag_Protetto = Array(3)
                            Flag_PubblicoPrivato = Array(4)

                            Dim ObjDownloadWs As New WS_Disciplinari.AgroWS_Disciplinari
                            Dim objWs As New AgronicaCoreVarieDAL.ConnessioneWS

                            objWs.NewWS(ObjDownloadWs,
                                            _WS_Disciplinari_AgroWS_Disciplinari,
                                            objParametri_Utenti)

                            If Flag_PubblicoPrivato = 2 Then
                                Dpi_Cod = -Dpi_Cod
                            End If

                            XmlDoc = New System.Xml.XmlDocument

                            Dim objCoreAgroWs As New AgronicaCoreWebService.AgroWs

                            objCoreAgroWs.AgroWS_XML__Credenziali(AgronicaCoreWebService.AgroWs.enum_AWS_Xml.Xml_CODIFICA,
                                                                   StrCredenziali,
                                                                   AgronicaCoreWebService.AgroWs.enum_AWS_Schede.AWS_Disciplinari_Formulato_Completo,
                                                                   objCoreAgroWs.AgroWS_DoorKey(AgronicaCoreWebService.AgroWs.enum_AWS_Schede.AWS_Disciplinari_Formulato_Completo),
                                                                   HttpContext.Current.Session("ASG_ProgressivoGIAS"),
                                                                   HttpContext.Current.Session("ASG_SuperUser_CodFiscale").ToString,
                                                                   HttpContext.Current.Session("ASG_SuperUser_Username").ToString,
                                                                   HttpContext.Current.Session("ASG_SuperUser_Password").ToString,
                                                                   HttpContext.Current.Session("ASG_Utente_Username").ToString,
                                                                   HttpContext.Current.Session("ASG_Utente_Password").ToString)

                            XmlDoc.LoadXml(StrCredenziali)

                            XML_Credenziali = XmlDoc.SelectSingleNode("CREDENZIALI")

                            objCoreAgroWs.AgroWS_XML_Parametri_Formulati_ConDosi_Profitosan(AgronicaCoreWebService.AgroWs.enum_AWS_Xml.Xml_CODIFICA,
                                                            StrParametri,
                                                            enum_AWS_ApplicazioneRichiedente.AgronicaAgenda,
                                                            _Veg_Cod,
                                                            Dpi_Cod, 0,
                                                            IdRcdpi,
                                                            _Tipo_Testata,
                                                            Opt_Avversita_Infestanti,
                                                            _Opt_Singola_Gruppo,
                                                            _Av_Gru,
                                                            _Av_Cod,
                                                            _StrAvversita,
                                                            _Modulo,
                                                            _Epoca_Cod,
                                                            "",
                                                             _TipoRichiesto,
                                                             _TestoRicerca,
                                                             _Validita_Fine,
                                                             _FiltroAggiuntivo,
                                                             "",
                                                             strErr,
                                                             grfi_Cod,
                                                             0, "", 0, "",
                                                             _Stato_Cod,
                                                             objParametri_Server.Lingua_Cod)

                            XML_Credenziali.InnerXml = StrParametri

                            Parametri = XmlDoc.OuterXml

                            Parametri = objCoreAgroWs.AWS_Codifica_P(Parametri)

                            DtRisultati = ObjDownloadWs.Leggi_Formulati_DPI_DT_New(Parametri, strErr)


                            ObjDownloadWs.Dispose()

                            '------------------------------------

                            Dpi_Cod = Math.Abs(Dpi_Cod)

                            ObjDownloadWs.Dispose()

                            If strErr = "" Then

                                Dim objDistinct As New AgronicaCoreUtility.DatatableUtility
                                Dim DrFrCod As DataRow()
                                Dim DrFrDosi As DataRow()

                                '--------------------------------
                                'Seleziono i Formulati Distinti
                                '--------------------------------

                                If Not IsNothing(DtRisultati) AndAlso DtRisultati.Rows.Count > 0 Then



                                    Dim strFrCod As String() = objDistinct.SelectDistinct(DtRisultati, "Fr_Cod")

                                    If strFrCod IsNot Nothing AndAlso strFrCod.Length > 0 Then

                                        For i = 0 To strFrCod.Length - 1

                                            Dim DtFormulato As New DataTable
                                            Dim DtDosi As New DataTable

                                            DtFormulato = DtRisultati.Clone
                                            DtDosi = DtRisultati.Clone

                                            DrFrCod = DtRisultati.Select("Fr_Cod=" & strFrCod(i))

                                            If DrFrCod IsNot Nothing AndAlso DrFrCod.Length > 0 Then

                                                For j = 0 To DrFrCod.Length - 1
                                                    DtFormulato.ImportRow(DrFrCod(j))
                                                Next

                                                If DtFormulato IsNot Nothing AndAlso DtFormulato.Rows.Count > 0 Then

                                                    Fr_Des = DtFormulato.Rows(0).Item("fr_des")
                                                    Fr_Cod = DtFormulato.Rows(0).Item("fr_cod")

                                                    Carenza = ""
                                                    strCarenza = ""
                                                    If Not IsDBNull(DtFormulato.Rows(0).Item("tempocarenza")) AndAlso DtFormulato.Rows(0).Item("tempocarenza") <> 0 Then
                                                        Carenza = DtFormulato.Rows(0).Item("tempocarenza")
                                                        strCarenza = " --- [Carenza " & Carenza & " gg]"
                                                    End If


                                                    DataAttoNormativo = ""
                                                    InRevisione = ""
                                                    If Not IsDBNull(DtFormulato.Rows(0).Item("Verificato_Flag")) AndAlso DtFormulato.Rows(0).Item("Verificato_Flag") = 15 Then
                                                        InRevisione = "1"
                                                        If Not IsDBNull(DtFormulato.Rows(0).Item("DataAttoNormativo")) AndAlso IsDate(DtFormulato.Rows(0).Item("DataAttoNormativo")) Then
                                                            DataAttoNormativo = CDate(DtFormulato.Rows(0).Item("DataAttoNormativo")).ToShortDateString
                                                        End If
                                                    End If

                                                    'se è richiesto aggiungo il principio attivo principale
                                                    If _Flag_PrincipiAttivi Then
                                                        strPA_Des = ""
                                                        strPA_DesTot = ""
                                                        strPA_COD = ""
                                                        strTITOLI = ""
                                                        strPrincipi = ""
                                                        If Not IsDBNull(DtFormulato.Rows(0).Item("pa_des")) AndAlso CStr(DtFormulato.Rows(0).Item("pa_des")) <> "" Then
                                                            strPA_Des = " --- [" & (CStr(DtFormulato.Rows(0).Item("pa_des"))) & "]"
                                                        End If
                                                        If Not IsDBNull(DtFormulato.Rows(0).Item("strpa_des")) AndAlso CStr(DtFormulato.Rows(0).Item("strpa_des")) <> "" Then
                                                            strPA_DesTot = " --- [" & (CStr(DtFormulato.Rows(0).Item("strpa_des"))) & "]"
                                                        End If
                                                        If Not IsDBNull(DtFormulato.Rows(0).Item("strPA_COD")) AndAlso CStr(DtFormulato.Rows(0).Item("strPA_COD")) <> "" Then
                                                            strPA_COD = CStr(DtFormulato.Rows(0).Item("strPA_COD"))
                                                            Principi = Split(strPA_COD, "|")
                                                            If Not IsDBNull(DtFormulato.Rows(0).Item("strTITOLI")) AndAlso CStr(DtFormulato.Rows(0).Item("strTITOLI")) <> "" Then
                                                                strTITOLI = CStr(DtFormulato.Rows(0).Item("strTITOLI"))
                                                                Titoli = Split(strTITOLI, "|")
                                                                If Principi IsNot Nothing AndAlso Principi.Length > 0 Then
                                                                    For p = 0 To Principi.Length - 1
                                                                        strPrincipi &= Principi(p) & "§" & Titoli(p) & "|"
                                                                    Next
                                                                End If
                                                            End If
                                                        End If
                                                        If strPrincipi <> "" Then
                                                            strPrincipi = Left(strPrincipi, strPrincipi.Length - 1)
                                                        End If
                                                    End If
                                                    'se è richiesto aggiungo la classe tossicologica principale
                                                    If _Flag_ClasseTossicologica Then
                                                        strClassToss = ""
                                                        strCLTOSS_COD = ""
                                                        If Not IsDBNull(DtFormulato.Rows(0).Item("strCLTOSS_COD")) AndAlso CStr(DtFormulato.Rows(0).Item("strCLTOSS_COD")) <> "" Then
                                                            strClassToss = " --- [" & (CStr(DtFormulato.Rows(0).Item("strCLTOSS_COD"))) & "]"
                                                            strCLTOSS_COD = CStr(DtFormulato.Rows(0).Item("strCLTOSS_COD"))
                                                        End If
                                                    End If

                                                    Dim strDosi As String() = objDistinct.SelectDistinct(DtFormulato, "For_Veg_Av_Dos_Cod")
                                                    Dim strdose As String = ""
                                                    If strDosi IsNot Nothing AndAlso strDosi.Length > 0 Then
                                                        For d = 0 To strDosi.Length - 1
                                                            DrFrDosi = DtFormulato.Select("For_Veg_Av_Dos_Cod=" & strDosi(d))
                                                            If DrFrDosi IsNot Nothing AndAlso DrFrDosi.Length > 0 Then
                                                                strdose &= strDosi(d) & "$" &
                                                                           DrFrDosi(0).Item("dose_min").ToString & "$" &
                                                                           DrFrDosi(0).Item("dose_max").ToString & "$" &
                                                                           DrFrDosi(0).Item("udm_cod").ToString & "$" &
                                                                           DrFrDosi(0).Item("udm_sim").ToString & "$" &
                                                                           DrFrDosi(0).Item("acqua_min").ToString & "$" &
                                                                           DrFrDosi(0).Item("acqua_max").ToString & "$" &
                                                                           DrFrDosi(0).Item("acqua_udm_cod").ToString & "$" &
                                                                           DrFrDosi(0).Item("acqua_udm_sim").ToString & "$" &
                                                                           DrFrDosi(0).Item("da_epoca_1").ToString & "$" &
                                                                           DrFrDosi(0).Item("a_epoca_1").ToString & "$" &
                                                                           DrFrDosi(0).Item("LimiteInterventi").ToString & "$" &
                                                                           DrFrDosi(0).Item("UDM_COD_Limite").ToString & "$" &
                                                                           DrFrDosi(0).Item("udm_sim_Limite").ToString & "$" &
                                                                           DrFrDosi(0).Item("strCLTOSS_Grado").ToString & "$" &
                                                                           DrFrDosi(0).Item("epoca_cod").ToString & "$" &
                                                                           DrFrDosi(0).Item("IntervalloTrattamenti_Min").ToString & "$" &
                                                                           DrFrDosi(0).Item("IntervalloTrattamenti_Max").ToString &
                                                                           DrFrDosi(0).Item("BufferZone_Min").ToString & "$" &
                                                                           DrFrDosi(0).Item("BufferZone_Max").ToString & "£"

                                                            End If
                                                        Next
                                                    End If
                                                    If strdose <> "" Then
                                                        strdose = Left(strdose, strdose.Length - 1)
                                                    End If

                                                    strValue = Fr_Cod & "£" &
                                                               InRevisione & "£" &
                                                               DataAttoNormativo & "£" &
                                                               Carenza & "£" &
                                                               strPrincipi & "£" &
                                                               strCLTOSS_COD & "£" &
                                                               strdose


                                                    strGiacenza = ""
                                                    If Dt_Giacenze IsNot Nothing AndAlso Dt_Giacenze.Rows.Count > 0 Then
                                                        DrGiacenze = Dt_Giacenze.Select("pro_cod=" & Fr_Cod)
                                                        If DrGiacenze IsNot Nothing AndAlso DrGiacenze.Length > 0 Then
                                                            strGiacenza = " --- GIACENZA: alla data " & Math.Round(DrGiacenze(0).Item("Giacenza"), 4).ToString & " " & DrGiacenze(0).Item("Udm_Sim")
                                                        End If
                                                    End If
                                                    If Dt_Giacenze_Tot IsNot Nothing AndAlso Dt_Giacenze_Tot.Rows.Count > 0 Then
                                                        DrGiacenze_Tot = Dt_Giacenze_Tot.Select("pro_cod=" & Fr_Cod)
                                                        If DrGiacenze_Tot IsNot Nothing AndAlso DrGiacenze_Tot.Length > 0 Then
                                                            strGiacenza &= " totale " & Math.Round(DrGiacenze_Tot(0).Item("Giacenza"), 4).ToString & " " & DrGiacenze_Tot(0).Item("Udm_Sim")
                                                        End If
                                                    End If
                                                    Dr = DtFormulatiOrdinati.NewRow
                                                    Dr.Item("Testo") = Fr_Des & " (" & Fr_Cod & ")" & strPA_Des & strCarenza & strClassToss & strGiacenza
                                                    Dr.Item("Valore") = strValue
                                                    DtFormulatiOrdinati.Rows.Add(Dr)


                                                End If

                                            End If

                                        Next

                                    End If

                                End If

                            End If

                    End Select

                    If DtFormulatiOrdinati IsNot Nothing Then

                        'uso il dataview per ordinare 
                        Dim Dv As New DataView

                        DtFormulatiOrdinati.TableName = "Prodotti"
                        Dv.Table = DtFormulatiOrdinati
                        Dv.Sort = "Testo ASC"

                        For i = 0 To Dv.Count - 1
                            ddl_Formulati.Items.Add(New ListItem(Dv(i).Item("Testo"),
                                                    Dv(i).Item("Valore")))
                        Next

                    End If

                End If

        End Select

    End Sub

    'NOTA
    'Il TipoRichiesto consente di selezionare solo i formulati specifici
    'per la particolare applicazione:
    '
    '   0 = Tutti i formulati
    '   1 = Trattamenti Antiparassitari
    '   2 = Diserbo
    '   3 = Trattamenti Fitoregolatori
    '   4 = Coadiuvanti, Bagnanti, Antischiuma
    '   5 = Concianti
    '   6 = Disseccanti
    '   7 = Geodisinfestanti
    '   12 = post-raccolta
    '   ecc...

    Public Sub CaricaComboFormulatiClassificazioni_Storico()

        Dim strErr As String = ""

        Dim XmlDoc As New System.Xml.XmlDocument

        Dim Fr_Des As String = ""
        Dim Fr_Des_Prec As String = ""
        Dim Fr_Cod As String = ""
        Dim Carenza As String = ""
        Dim strCarenza As String = ""
        Dim strPA_Des As String = ""
        Dim InRevisione As String = ""
        Dim DataAttoNormativo As String = ""
        Dim DataSmaltimentoScorte As String = ""
        Dim strSmaltimento As String = ""
        Dim strBufferZone As String = ""
        Dim BufferMin As Decimal
        Dim BufferMax As Decimal
        Dim FormulatiXAllegatiNormative_IDRiga As Integer = 0
        Dim Mdi_Cod As Integer = 0
        Dim Flag_Protetto_A As Integer = 0
        Dim strProtezione As String
        Dim strModalitaImpiego As String
        Dim strEpocheBlocchi As String
        Dim For_Veg_Cod As Integer = 0


        Dim strPA_DesTot As String = ""
        Dim strPA_COD As String = ""
        Dim strTITOLI As String = ""
        Dim strPESI As String = ""
        Dim strPrincipi As String = ""
        Dim strPrincipiPesi As String = ""
        Dim Principi As String()
        Dim Titoli As String()
        Dim Pesi As String()
        Dim p As Integer

        Dim strClassToss As String = ""
        Dim strCLTOSS_COD As String = ""
        Dim strClassificazione As String = ""
        Dim strClassificazioni As String = ""

        Dim strValue As String = ""

        Dim strGiacenzaTesto As String = ""
        Dim strGiacenzaVal As String = ""
        Dim strGiacenzaTotaleVal As String = ""
        Dim strUdmCod As String = ""

        Dim t As Integer

        Dim XML_Credenziali As System.Xml.XmlElement
        Dim StrCredenziali As String = ""
        Dim StrParametri As String = ""
        Dim Parametri As String = ""
        Dim flagQtaMaggioreZero As Boolean = False
        Dim utilizzaLotto = False

        ddl_Formulati.Items.Clear()

        If _PrimaRiga_Flag Then
            ddl_Formulati.Items.Add(New ListItem(_PrimaRiga_Text, _PrimaRiga_Value))
        End If


        Dim objParametri_Utenti As New AgronicaCoreDataProvider.AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Utenti"))
        Dim objParametri_Server As New AgronicaCoreDataProvider.AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))


        '-------------------------------------------------------------
        '--- Se utilizzo il magazzino
        '--- leggo prima i prodotti in giacenza
        '-------------------------------------------------------------
        Dim appoggio_prodotto_des As String
        Dim Dt_Giacenze As DataTable = Nothing
        Dim Dt_Giacenze_Tot As DataTable = Nothing
        Dim DtRisultati As DataTable
        Dim DtFormulatiOrdinati As New DataTable
        Dim Dr As DataRow
        Dim DrGiacenze As DataRow() = Nothing
        Dim DrGiacenze_Tot As DataRow() = Nothing

        DtFormulatiOrdinati.Columns.Add(New DataColumn("Testo", GetType(String)))
        DtFormulatiOrdinati.Columns.Add(New DataColumn("Valore", GetType(String)))

        '(15/03/2019 fede)
        Select Case _GestioneLotto
            Case enum_Gestione_Lotti.Nessuna
                utilizzaLotto = False
            Case Else
                utilizzaLotto = True
        End Select

        Select Case _GestioneGiacenze
            Case enum_Gestione_Giacenze.SoloPresenti
                flagQtaMaggioreZero = True
            Case enum_Gestione_Giacenze.TuttiProdotti
                _Fabbricato_Cod = 0
                utilizzaLotto = False
        End Select



        If _Fabbricato_Cod <> "0" Then

            Select Case _Cau_Mov

                Case CAU_SCARICO

                    Dim objG As New AgronicaCoreStampeDAL.Magazzino
                    Dim strFiltroFrCod As String = ""

                    Dt_Giacenze = objG.SchedaGiacenzeMagazzino(_Validita_Fine,
                                         Split(_Fabbricato_Cod, "|")(2),
                                         Split(_Fabbricato_Cod, "|")(1),
                                         Split(_Fabbricato_Cod, "|")(0),
                                         191,
                                         _FrCod,
                                         0, 0, 0, 0, 0,
                                         LOTTO_NONDEFINITO,
                                         _EscludiGiacenzeZero,
                                         " and Formulati.Fr_Des like '%" & _TestoRicerca & "%'", "", "", "", "", "", "", "", "", "", "",
                                         "",
                                         "",
                                         objParametri_Server, objParametri_Utenti,
                                         Flag_QtaMaggioreZero:=flagQtaMaggioreZero)

                    Dt_Giacenze_Tot = objG.SchedaGiacenzeMagazzino(AGRODATAFINE,
                                       Split(_Fabbricato_Cod, "|")(2),
                                       Split(_Fabbricato_Cod, "|")(1),
                                       Split(_Fabbricato_Cod, "|")(0),
                                       191,
                                       _FrCod,
                                       0, 0, 0, 0, 0,
                                       LOTTO_NONDEFINITO,
                                       _EscludiGiacenzeZero,
                                       " and Formulati.Fr_Des like '%" & _TestoRicerca & "%'", "", "", "", "", "", "", "", "", "", "",
                                       "",
                                       "",
                                       objParametri_Server, objParametri_Utenti,
                                         Flag_QtaMaggioreZero:=flagQtaMaggioreZero)

                    objG = Nothing

                    If FrCod <> 0 Then
                        If Dt_Giacenze.Rows.Count > 0 Then
                            appoggio_prodotto_des = Dt_Giacenze.Rows(0).Item("Descrizione_Prodotto")
                        End If
                    End If


                    For i = 0 To Dt_Giacenze.Rows.Count - 1
                        'Anna 10/05/2022: nascoste migrogiacenze SE
                        '                                   impostazione utente = Solo Presenti (> 0 e QTA_GiancenzeVisualizzate)
                        '                                        O SE
                        '                                   se visualizza giacenze zero = non checkato e impostazione utente = tutti i movimenti
                        '[rif chiamata 17160]
                        Dim filtroGiacenze = (Dt_Giacenze(i).Item("Giacenza") > QTA_GiancenzeVisualizzate Or Dt_Giacenze(i).Item("Giacenza") < -QTA_GiancenzeVisualizzate)
                        If Not _EscludiGiacenzeZero Then
                            If Not flagQtaMaggioreZero Then
                                strFiltroFrCod &= " Formulati.FR_COD=" & Dt_Giacenze.Rows(i).Item("Pro_Cod").ToString & " OR "
                            ElseIf flagQtaMaggioreZero And filtroGiacenze = True Then
                                strFiltroFrCod &= " Formulati.FR_COD=" & Dt_Giacenze.Rows(i).Item("Pro_Cod").ToString & " OR "
                            End If
                        Else
                            If (filtroGiacenze) Then
                                strFiltroFrCod &= " Formulati.FR_COD=" & Dt_Giacenze.Rows(i).Item("Pro_Cod").ToString & " OR "
                            End If
                        End If
                    Next

                    If strFiltroFrCod <> "" Then
                        strFiltroFrCod = Left(strFiltroFrCod, strFiltroFrCod.Length - 3)
                        strFiltroFrCod = " AND (" & strFiltroFrCod & ")"
                    Else
                        'se non ho alcun formulato in magazzino
                        Exit Sub
                    End If

                    _FiltroAggiuntivo &= strFiltroFrCod

            End Select

        End If


        Dim strTipiRichiesti As String = ""
        For t = 0 To TipiRichiesti.Count - 1
            strTipiRichiesti &= TipiRichiesti(t) & ","
        Next
        strTipiRichiesti = strTipiRichiesti.Substring(0, strTipiRichiesti.Length - 1)


        Dim Array As String()
        Dim Dpi_Cod As Integer = 0
        Dim IdRcdpi As Integer = 0
        Dim Flag_Protetto As Integer = 0
        Dim Flag_PubblicoPrivato As Integer = 0

        Array = Split(_Disciplinare_Cod, "/")

        Select Case _Disciplinare_Cod
            Case "0", enum_Disciplinare_Operazione.Biologico, enum_Disciplinare_Operazione.NessunDpiNessunaEtichetta
                Dpi_Cod = CInt(_Disciplinare_Cod)
            Case Else
                Dpi_Cod = Array(0)
                IdRcdpi = Array(1)
                Flag_Protetto = Array(3)
                Flag_PubblicoPrivato = Array(4)
        End Select

        Dim ObjDownloadWs As New WS_Disciplinari.AgroWS_Disciplinari
        Dim objWs As New AgronicaCoreVarieDAL.ConnessioneWS

        objWs.NewWS(ObjDownloadWs, _WS_Disciplinari_AgroWS_Disciplinari, objParametri_Utenti)

        'If Flag_PubblicoPrivato = 2 Then
        '    Dpi_Cod = -Dpi_Cod
        'End If

        XmlDoc = New System.Xml.XmlDocument

        Dim objCoreAgroWs As New AgronicaCoreWebService.AgroWs

        objCoreAgroWs.AgroWS_XML__Credenziali(AgronicaCoreWebService.AgroWs.enum_AWS_Xml.Xml_CODIFICA,
                                                StrCredenziali,
                                                AgronicaCoreWebService.AgroWs.enum_AWS_Schede.AWS_Disciplinari_Formulato_Completo,
                                                objCoreAgroWs.AgroWS_DoorKey(AgronicaCoreWebService.AgroWs.enum_AWS_Schede.AWS_Disciplinari_Formulato_Completo),
                                                HttpContext.Current.Session("ASG_ProgressivoGIAS"),
                                                HttpContext.Current.Session("ASG_SuperUser_CodFiscale").ToString,
                                                HttpContext.Current.Session("ASG_SuperUser_Username").ToString,
                                                HttpContext.Current.Session("ASG_SuperUser_Password").ToString,
                                                HttpContext.Current.Session("ASG_Utente_Username").ToString,
                                                HttpContext.Current.Session("ASG_Utente_Password").ToString)

        XmlDoc.LoadXml(StrCredenziali)

        XML_Credenziali = XmlDoc.SelectSingleNode("CREDENZIALI")

        Dim strListaComuni As String = ""
        For c = 0 To ListaComuni.Count - 1
            strListaComuni &= ListaComuni(c).Prov & ListaComuni(c).Com & ","
        Next
        If strListaComuni <> "" Then
            strListaComuni = Left(strListaComuni, strListaComuni.Length - 1)
        End If

        objCoreAgroWs.AgroWS_XML_Parametri_Formulati_ConDosi_Profitosan(AgronicaCoreWebService.AgroWs.enum_AWS_Xml.Xml_CODIFICA,
                                                                        StrParametri,
                                                                        enum_AWS_ApplicazioneRichiedente.AgronicaAgenda,
                                                                        _Veg_Cod,
                                                                        Dpi_Cod, Flag_PubblicoPrivato,
                                                                        IdRcdpi,
                                                                        _Tipo_Testata,
                                                                        Opt_Avversita_Infestanti,
                                                                        _Opt_Singola_Gruppo,
                                                                        _Av_Gru,
                                                                        _Av_Cod,
                                                                        _StrAvversita,
                                                                        _Modulo,
                                                                        _Epoca_Cod,
                                                                        "",
                                                                         strTipiRichiesti,
                                                                         _TestoRicerca,
                                                                         _Validita_Fine,
                                                                         _FiltroAggiuntivo,
                                                                         "",
                                                                         strErr,
                                                                         grfi_Cod,
                                                                         0, strListaComuni,
                                                                        _FormulatiXAllegatiNormative_IDRiga,
                                                                        _Copertura,
                                                                        _Stato_Cod,
                                                                        objParametri_Server.Lingua_Cod,
                                                                        Stato_Impianto)

        XML_Credenziali.InnerXml = StrParametri

        Parametri = XmlDoc.OuterXml

        Parametri = objCoreAgroWs.AWS_Codifica_P(Parametri)

        DtRisultati = ObjDownloadWs.Leggi_Formulati_DT(Parametri, strErr)

        ObjDownloadWs.Dispose()

        '------------------------------------

        Dpi_Cod = Math.Abs(Dpi_Cod)

        ObjDownloadWs.Dispose()

        If strErr = "" Then


            Select Case _Disciplinare_Cod

                Case enum_Disciplinare_Operazione.NessunDpiNessunaEtichetta

                    If Not IsNothing(DtRisultati) AndAlso DtRisultati.Rows.Count > 0 Then

                        For i = 0 To DtRisultati.Rows.Count - 1

                            Fr_Cod = DtRisultati.Rows(i).Item("Fr_Cod")
                            Fr_Des = DtRisultati.Rows(i).Item("Fr_Des")
                            Fr_Des_Prec = DtRisultati.Rows(i).Item("Fr_Des_Prec")

                            DataSmaltimentoScorte = ""
                            If Not IsDBNull(DtRisultati.Rows(i).Item("DataSmaltimentoScorte")) AndAlso IsDate(DtRisultati.Rows(i).Item("DataSmaltimentoScorte")) Then
                                DataSmaltimentoScorte = CDate(DtRisultati.Rows(i).Item("DataSmaltimentoScorte")).ToShortDateString
                            End If



                            'se è richiesto aggiungo la classificazione
                            If _Flag_Classificazione Then
                                If Not IsDBNull(DtRisultati.Rows(i).Item("Classificazioni")) AndAlso CStr(DtRisultati.Rows(i).Item("Classificazioni")) <> "" Then
                                    strClassificazioni = " --- [" & (CStr(DtRisultati.Rows(i).Item("Classificazioni"))) & "]"
                                End If
                            End If

                            strSmaltimento = ""
                            If DataSmaltimentoScorte <> "" Then
                                strSmaltimento = " --- [" & My.Resources.AgronicaControlli_2010.FineScortaAl & " " & DataSmaltimentoScorte & "]"
                            End If

                            'se valorizzato fr_des_prec mostro quello (nome vecchio)
                            'altrimenti se nel testo ho ex lo elimino dalla visualizzazione (nuovo nome)    
                            If Fr_Des_Prec <> "" Then
                                Fr_Des = Fr_Des_Prec
                            End If
                            If InStr(LCase(Fr_Des), "(ex") <> 0 Then
                                Fr_Des = Left(Fr_Des, InStr(LCase(Fr_Des), "(ex") - 1)
                            End If

                            If _Flag_PrincipiAttivi Then
                                strPA_COD = ""
                                strTITOLI = ""
                                strPESI = ""
                                strPrincipi = ""
                                strPrincipiPesi = ""
                                If Not IsDBNull(DtRisultati.Rows(i).Item("strpa_des")) AndAlso CStr(DtRisultati.Rows(i).Item("strpa_des")) <> "" Then
                                    strPA_DesTot = " --- [" & (CStr(DtRisultati.Rows(i).Item("strpa_des"))) & "]"
                                End If
                                If Not IsDBNull(DtRisultati.Rows(i).Item("strPA_COD")) AndAlso CStr(DtRisultati.Rows(i).Item("strPA_COD")) <> "" Then
                                    strPA_COD = CStr(DtRisultati.Rows(i).Item("strPA_COD"))
                                    Principi = Split(strPA_COD, "|")
                                    If Not IsDBNull(DtRisultati.Rows(i).Item("strTITOLI")) AndAlso CStr(DtRisultati.Rows(i).Item("strTITOLI")) <> "" Then
                                        strTITOLI = CStr(DtRisultati.Rows(i).Item("strTITOLI"))
                                        strPESI = CStr(DtRisultati.Rows(i).Item("strPESI"))
                                        Titoli = Split(strTITOLI, "|")
                                        Pesi = Split(strPESI, "|")
                                        If Principi IsNot Nothing AndAlso Principi.Length > 0 Then
                                            For p = 0 To Principi.Length - 1
                                                strPrincipi &= Principi(p) & "§" & Titoli(p) & "|"
                                                strPrincipiPesi &= Principi(p) & "§" & Pesi(p) & "|"
                                            Next
                                        End If
                                    End If
                                End If
                                If strPrincipi <> "" Then
                                    strPrincipi = Left(strPrincipi, strPrincipi.Length - 1)
                                End If
                                If strPrincipiPesi <> "" Then
                                    strPrincipiPesi = Left(strPrincipiPesi, strPrincipiPesi.Length - 1)
                                End If
                            End If



                            '(15/03/2019 fede) se utilizzo il lotto aggiungo le altre giacenze
                            strGiacenzaTesto = ""
                            strGiacenzaVal = ""
                            strGiacenzaTotaleVal = ""
                            strUdmCod = ""
                            If Dt_Giacenze IsNot Nothing AndAlso Dt_Giacenze.Rows.Count > 0 Then
                                DrGiacenze = Dt_Giacenze.Select("pro_cod=" & Fr_Cod)
                            End If
                            If Dt_Giacenze_Tot IsNot Nothing AndAlso Dt_Giacenze_Tot.Rows.Count > 0 Then
                                DrGiacenze_Tot = Dt_Giacenze_Tot.Select("pro_cod=" & Fr_Cod)
                            End If

                            Dim strLotto As String = ""

                            If utilizzaLotto Then

                                If DrGiacenze IsNot Nothing AndAlso DrGiacenze.Length > 0 Then

                                    For g = 0 To DrGiacenze.Length - 1

                                        strGiacenzaTesto = " --- " & My.Resources.AgronicaControlli_2010.Giacenza & ": " & My.Resources.AgronicaControlli_2010.AllaData & " " & Math.Round(DrGiacenze(g).Item("Giacenza"), 4).ToString & " " & DrGiacenze(g).Item("Udm_Sim")
                                        strGiacenzaVal = Math.Round(DrGiacenze(g).Item("Giacenza"), 4).ToString
                                        strUdmCod = DrGiacenze(g).Item("Udm_Cod")

                                        If DrGiacenze_Tot IsNot Nothing AndAlso DrGiacenze_Tot.Length > 0 Then
                                            For gt = 0 To DrGiacenze_Tot.Length - 1
                                                If DrGiacenze_Tot(gt).Item("lotto") = DrGiacenze(g).Item("lotto") Then
                                                    strGiacenzaTesto &= " " & My.Resources.AgronicaControlli_2010.Totale & " " & Math.Round(DrGiacenze_Tot(gt).Item("Giacenza"), 4).ToString & " " & DrGiacenze_Tot(gt).Item("Udm_Sim")
                                                    strGiacenzaTotaleVal = Math.Round(DrGiacenze_Tot(gt).Item("Giacenza"), 4).ToString
                                                    Exit For
                                                End If
                                            Next
                                        End If

                                        If strGiacenzaTesto <> "" Then
                                            strGiacenzaTesto &= " --- " & My.Resources.AgronicaControlli_2010.Lotto & " " & DrGiacenze(g).Item("lotto")
                                        End If

                                        strLotto = DrGiacenze(g).Item("lotto")


                                        Dr = DtFormulatiOrdinati.NewRow
                                        Dr.Item("Testo") = Fr_Des & " (" & Fr_Cod & ")" & strClassificazioni & strSmaltimento & strGiacenzaTesto

                                        'Dr.Item("Valore") = Fr_Cod & "£££££" &
                                        '    DtRisultati.Rows(i).Item("TipoRichiesto") & "£££££££" &
                                        '    strLotto & "£" &
                                        '    strGiacenzaVal & "£" &
                                        '    strGiacenzaTotaleVal & "£" &
                                        '    strUdmCod & "£" &
                                        '    strPrincipi & "£" &
                                        '    strPrincipiPesi

                                        TipoRichiesto = DtRisultati.Rows(i).Item("TipoRichiesto")
                                        Dim strdose = ""
                                        strEpocheBlocchi = ""
                                        strValue = Fr_Cod & "£" &
                                                            InRevisione & "£" &
                                                            DataAttoNormativo & "£" &
                                                            Carenza & "£" &
                                                            FormulatiXAllegatiNormative_IDRiga & "£" &
                                                            For_Veg_Cod & "£" &
                                                            TipoRichiesto & "£" &
                                                            strPrincipi & "£" &
                                                            strPrincipiPesi & "£" &
                                                            strCLTOSS_COD & "£" &
                                                            BufferMin & "£" & BufferMax & "£" &
                                                            strEpocheBlocchi & "£" &
                                                            strLotto & "£" &
                                                            strGiacenzaVal & "£" &
                                                            strGiacenzaTotaleVal & "£" &
                                                            strUdmCod & "£" &
                                                            strdose

                                        Dr.Item("Valore") = strValue

                                        DtFormulatiOrdinati.Rows.Add(Dr)

                                    Next

                                End If

                            Else

                                If DrGiacenze IsNot Nothing AndAlso DrGiacenze.Length > 0 Then
                                    strGiacenzaTesto = " --- " & My.Resources.AgronicaControlli_2010.Giacenza & ": " & My.Resources.AgronicaControlli_2010.AllaData & " " & Math.Round(DrGiacenze(0).Item("Giacenza"), 4).ToString & " " & DrGiacenze(0).Item("Udm_Sim")
                                    strGiacenzaVal = Math.Round(DrGiacenze(0).Item("Giacenza"), 4).ToString
                                    strUdmCod = DrGiacenze(0).Item("Udm_Cod")
                                End If

                                If DrGiacenze_Tot IsNot Nothing AndAlso DrGiacenze_Tot.Length > 0 Then
                                    strGiacenzaTesto &= " " & My.Resources.AgronicaControlli_2010.Totale & " " & Math.Round(DrGiacenze_Tot(0).Item("Giacenza"), 4).ToString & " " & DrGiacenze_Tot(0).Item("Udm_Sim")
                                    strGiacenzaTotaleVal = Math.Round(DrGiacenze_Tot(0).Item("Giacenza"), 4).ToString
                                End If

                                Dr = DtFormulatiOrdinati.NewRow
                                Dr.Item("Testo") = Fr_Des & " (" & Fr_Cod & ")" & strClassificazioni & strSmaltimento & strGiacenzaTesto



                                'Dim strValore = Fr_Cod & "£" &
                                '                    "£" &
                                '                    "£" &
                                '                    "£" &
                                '                    "£" &
                                '                    DtRisultati.Rows(i).Item("TipoRichiesto") & "£" &
                                '                    "£" &
                                '                    "£" &
                                '                    "£" &
                                '                    "£" &
                                '                    "£" &
                                '                    "£" &
                                '                    strLotto & "£" &
                                '                    strGiacenzaVal & "£" &
                                '                    strGiacenzaTotaleVal & "£" &
                                '                    strUdmCod & "£" &
                                '                    strPrincipi & "£" &
                                '                    strPrincipiPesi

                                TipoRichiesto = DtRisultati.Rows(i).Item("TipoRichiesto")
                                Dim strdose = ""
                                strEpocheBlocchi = ""
                                strValue = Fr_Cod & "£" &
                                                            InRevisione & "£" &
                                                            DataAttoNormativo & "£" &
                                                            Carenza & "£" &
                                                            FormulatiXAllegatiNormative_IDRiga & "£" &
                                                            For_Veg_Cod & "£" &
                                                            TipoRichiesto & "£" &
                                                            strPrincipi & "£" &
                                                            strPrincipiPesi & "£" &
                                                            strCLTOSS_COD & "£" &
                                                            BufferMin & "£" & BufferMax & "£" &
                                                            strEpocheBlocchi & "£" &
                                                            strLotto & "£" &
                                                            strGiacenzaVal & "£" &
                                                            strGiacenzaTotaleVal & "£" &
                                                            strUdmCod & "£" &
                                                            strdose

                                Dr.Item("Valore") = strValue
                                DtFormulatiOrdinati.Rows.Add(Dr)

                            End If

                        Next

                    End If

                Case Else

                    Dim objDTU As New AgronicaCoreDataProvider.DatatableUtility
                    Dim DrFrCod As DataRow()
                    Dim DrFrDosi As DataRow()

                    '--------------------------------
                    'Seleziono i Formulati Distinti
                    '--------------------------------
                    Dim HashFormulati As New Hashtable
                    Dim TipoRichiesto As Integer
                    Dim InserisciFormulato As Boolean = False

                    If Not IsNothing(DtRisultati) AndAlso DtRisultati.Rows.Count > 0 Then

                        Dim DrF As DataTable = DatatableUtility.SelectDistinct_To_DT(DtRisultati, True, New String() {"Fr_Cod", "TipoRichiesto"})

                        For i = 0 To DrF.Rows.Count - 1

                            Dim DtFormulato As New DataTable
                            Dim DtDosi As New DataTable

                            DtFormulato = DtRisultati.Clone
                            DtDosi = DtRisultati.Clone

                            Fr_Cod = DrF.Rows(i).Item("fr_cod")
                            TipoRichiesto = DrF.Rows(i).Item("TipoRichiesto")

                            DrFrCod = DtRisultati.Select("Fr_Cod=" & Fr_Cod & " and TipoRichiesto=" & TipoRichiesto)

                            strClassificazioni = ""

                            If DrFrCod IsNot Nothing AndAlso DrFrCod.Length > 0 Then

                                For j = 0 To DrFrCod.Length - 1
                                    DtFormulato.ImportRow(DrFrCod(j))
                                Next

                                For f = 0 To DtFormulato.Rows.Count - 1

                                    DataSmaltimentoScorte = ""
                                    If Not IsDBNull(DtFormulato.Rows(f).Item("DataSmaltimentoScorte")) AndAlso IsDate(DtFormulato.Rows(f).Item("DataSmaltimentoScorte")) Then
                                        DataSmaltimentoScorte = CDate(DtFormulato.Rows(f).Item("DataSmaltimentoScorte")).ToShortDateString
                                    End If

                                    Fr_Des = DtFormulato.Rows(f).Item("fr_des")
                                    Fr_Des_Prec = DtFormulato.Rows(f).Item("Fr_Des_Prec")

                                    For_Veg_Cod = 0
                                    If Not IsDBNull(DtFormulato.Rows(f).Item("For_Veg_Cod")) AndAlso IsNumeric(DtFormulato.Rows(f).Item("For_Veg_Cod")) Then
                                        For_Veg_Cod = DtFormulato.Rows(f).Item("For_Veg_Cod")
                                    End If

                                    Carenza = ""
                                    strCarenza = ""
                                    If Not IsDBNull(DtFormulato.Rows(f).Item("tempocarenza")) AndAlso DtFormulato.Rows(f).Item("tempocarenza") <> 0 Then
                                        Carenza = DtFormulato.Rows(f).Item("tempocarenza")
                                        strCarenza = " --- [" & My.Resources.AgronicaControlli_2010.Carenza & " " & Carenza & " " & My.Resources.AgronicaControlli_2010.gg & "]"
                                    End If

                                    strSmaltimento = ""
                                    If DataSmaltimentoScorte <> "" Then
                                        strSmaltimento = " --- [" & My.Resources.AgronicaControlli_2010.FineScortaAl & " " & DataSmaltimentoScorte & "]"
                                    End If

                                    strBufferZone = ""
                                    BufferMin = 0
                                    BufferMax = 0

                                    If Not IsDBNull(DtFormulato.Rows(f).Item("BufferZone_Min")) AndAlso IsNumeric(DtFormulato.Rows(f).Item("BufferZone_Min")) Then
                                        BufferMin = CDec(DtFormulato.Rows(f).Item("BufferZone_Min"))
                                    End If
                                    If Not IsDBNull(DtFormulato.Rows(f).Item("BufferZone_Max")) AndAlso IsNumeric(DtFormulato.Rows(f).Item("BufferZone_Max")) Then
                                        BufferMax = CDec(DtFormulato.Rows(f).Item("BufferZone_Max"))
                                    End If
                                    If Not (BufferMin = 0 AndAlso BufferMax = 0) Then
                                        If BufferMin <> 0 AndAlso BufferMax = 0 Then
                                            strBufferZone &= " --- [Buffer Zone " & BufferMin & " m]" & vbCrLf
                                        Else
                                            strBufferZone &= " --- [Buffer Zone " & BufferMin & " - " & BufferMax & " m]" & vbCrLf
                                        End If
                                    End If
                                    Flag_Protetto_A = 0
                                    strProtezione = ""
                                    If Not IsDBNull(DtFormulato.Rows(f).Item("Flag_Protetto")) AndAlso IsNumeric(DtFormulato.Rows(f).Item("Flag_Protetto")) Then
                                        Flag_Protetto_A = CInt(DtFormulato.Rows(f).Item("Flag_Protetto"))
                                        Select Case Flag_Protetto_A
                                            Case 1
                                                strProtezione = " --- [" & My.Resources.AgronicaControlli_2010.Serra & "]" & vbCrLf
                                            Case 2
                                                strProtezione = " --- [" & My.Resources.AgronicaControlli_2010.PienoCampo & "]" & vbCrLf
                                        End Select
                                    End If

                                    Mdi_Cod = 0
                                    strModalitaImpiego = ""
                                    If Not IsDBNull(DtFormulato.Rows(f).Item("Mdi_Cod")) AndAlso IsNumeric(DtFormulato.Rows(f).Item("Mdi_Cod")) Then
                                        Mdi_Cod = CDec(DtFormulato.Rows(f).Item("Mdi_Cod"))
                                        If Mdi_Cod <> 0 Then
                                            Dim objModalita As New AgronicaCoreMetaSchemaDAL.ModalitaImpiego_R
                                            strModalitaImpiego &= " --- [" & objModalita.MdiDes_from_MdiCod(Mdi_Cod, HttpContext.Current.Session("ASG_objParametri_Server")) & "]"
                                        End If
                                    End If

                                    DataAttoNormativo = ""
                                    InRevisione = ""
                                    If Not IsDBNull(DtFormulato.Rows(f).Item("Verificato_Flag")) AndAlso DtFormulato.Rows(f).Item("Verificato_Flag") = 15 Then
                                        InRevisione = "1"
                                        If Not IsDBNull(DtFormulato.Rows(f).Item("DataAttoNormativo")) AndAlso IsDate(DtFormulato.Rows(f).Item("DataAttoNormativo")) Then
                                            DataAttoNormativo = CDate(DtFormulato.Rows(f).Item("DataAttoNormativo")).ToShortDateString
                                        End If
                                    End If

                                    FormulatiXAllegatiNormative_IDRiga = 0
                                    If Not IsDBNull(DtFormulato.Rows(f).Item("FormulatiXAllegatiNormative_IDRiga")) AndAlso IsNumeric(DtFormulato.Rows(f).Item("FormulatiXAllegatiNormative_IDRiga")) Then
                                        FormulatiXAllegatiNormative_IDRiga = CInt(DtFormulato.Rows(f).Item("FormulatiXAllegatiNormative_IDRiga"))
                                    End If


                                    'se è richiesto aggiungo il principio attivo principale
                                    If _Flag_PrincipiAttivi Then
                                        strPA_Des = ""
                                        strPA_DesTot = ""
                                        strPA_COD = ""
                                        strTITOLI = ""
                                        strPESI = ""
                                        strPrincipi = ""
                                        strPrincipiPesi = ""
                                        If Not IsDBNull(DtFormulato.Rows(f).Item("pa_des")) AndAlso CStr(DtFormulato.Rows(f).Item("pa_des")) <> "" Then
                                            strPA_Des = " --- [" & (CStr(DtFormulato.Rows(f).Item("pa_des"))) & "]"
                                        End If
                                        If Not IsDBNull(DtFormulato.Rows(f).Item("strpa_des")) AndAlso CStr(DtFormulato.Rows(f).Item("strpa_des")) <> "" Then
                                            strPA_DesTot = " --- [" & (CStr(DtFormulato.Rows(f).Item("strpa_des"))) & "]"
                                        End If
                                        If Not IsDBNull(DtFormulato.Rows(f).Item("strPA_COD")) AndAlso CStr(DtFormulato.Rows(f).Item("strPA_COD")) <> "" Then
                                            strPA_COD = CStr(DtFormulato.Rows(f).Item("strPA_COD"))
                                            Principi = Split(strPA_COD, "|")
                                            If Not IsDBNull(DtFormulato.Rows(f).Item("strTITOLI")) AndAlso CStr(DtFormulato.Rows(f).Item("strTITOLI")) <> "" Then
                                                strTITOLI = CStr(DtFormulato.Rows(f).Item("strTITOLI"))
                                                strPESI = CStr(DtFormulato.Rows(f).Item("strPESI"))
                                                Titoli = Split(strTITOLI, "|")
                                                Pesi = Split(strPESI, "|")
                                                If Principi IsNot Nothing AndAlso Principi.Length > 0 Then
                                                    For p = 0 To Principi.Length - 1
                                                        strPrincipi &= Principi(p) & "§" & Titoli(p) & "|"
                                                        strPrincipiPesi &= Principi(p) & "§" & Pesi(p) & "|"
                                                    Next
                                                End If
                                            End If
                                        End If
                                        If strPrincipi <> "" Then
                                            strPrincipi = Left(strPrincipi, strPrincipi.Length - 1)
                                        End If
                                        If strPrincipiPesi <> "" Then
                                            strPrincipiPesi = Left(strPrincipiPesi, strPrincipiPesi.Length - 1)
                                        End If
                                    End If

                                    'se è richiesto aggiungo la classificazione
                                    If _Flag_Classificazione Then
                                        If Not IsDBNull(DtFormulato.Rows(f).Item("Classificazioni")) AndAlso CStr(DtFormulato.Rows(f).Item("Classificazioni")) <> "" Then
                                            strClassificazioni = " --- [" & (CStr(DtFormulato.Rows(f).Item("Classificazioni"))) & "]"
                                        End If
                                    End If

                                    'se è richiesto aggiungo la classe tossicologica principale
                                    If _Flag_ClasseTossicologica Then
                                        strClassToss = ""
                                        strCLTOSS_COD = ""
                                        If Not IsDBNull(DtFormulato.Rows(f).Item("strCLTOSS_COD")) AndAlso CStr(DtFormulato.Rows(f).Item("strCLTOSS_COD")) <> "" Then
                                            strClassToss = " --- [" & (CStr(DtFormulato.Rows(f).Item("strCLTOSS_COD"))) & "]"
                                            strCLTOSS_COD = CStr(DtFormulato.Rows(f).Item("strCLTOSS_COD"))
                                        End If
                                    End If

                                    '(17/01/2017) epoche blocco 
                                    strEpocheBlocchi = ""
                                    If Not IsDBNull(DtFormulato.Rows(f).Item("EpocheBlocco")) Then
                                        strEpocheBlocchi = DtFormulato.Rows(f).Item("EpocheBlocco")
                                    End If

                                    Dim strDosi As String() = objDTU.SelectDistinct(DtFormulato, "For_Veg_Av_Dos_Cod")
                                    Dim strdose As String = ""
                                    If strDosi IsNot Nothing AndAlso strDosi.Length > 0 Then
                                        For d = 0 To strDosi.Length - 1
                                            DrFrDosi = DtFormulato.Select("For_Veg_Av_Dos_Cod=" & strDosi(d) & " And TipoRichiesto=" & TipoRichiesto.ToString)
                                            If DrFrDosi IsNot Nothing AndAlso DrFrDosi.Length > 0 Then
                                                strdose &= strDosi(d) & "$" &
                                                                DrFrDosi(0).Item("dose_min").ToString & "$" &
                                                                DrFrDosi(0).Item("dose_max").ToString & "$" &
                                                                DrFrDosi(0).Item("udm_cod").ToString & "$" &
                                                                DrFrDosi(0).Item("udm_sim").ToString & "$" &
                                                                DrFrDosi(0).Item("acqua_min").ToString & "$" &
                                                                DrFrDosi(0).Item("acqua_max").ToString & "$" &
                                                                DrFrDosi(0).Item("acqua_udm_cod").ToString & "$" &
                                                                DrFrDosi(0).Item("acqua_udm_sim").ToString & "$" &
                                                                DrFrDosi(0).Item("da_epoca_1").ToString & "$" &
                                                                DrFrDosi(0).Item("a_epoca_1").ToString & "$" &
                                                                DrFrDosi(0).Item("LimiteInterventi").ToString & "$" &
                                                                DrFrDosi(0).Item("UDM_COD_Limite").ToString & "$" &
                                                                DrFrDosi(0).Item("udm_sim_Limite").ToString & "$" &
                                                                DrFrDosi(0).Item("strCLTOSS_Grado").ToString & "$" &
                                                                DrFrDosi(0).Item("epoca_cod").ToString & "$" &
                                                                DrFrDosi(0).Item("IntervalloTrattamenti_Min").ToString & "$" &
                                                                DrFrDosi(0).Item("IntervalloTrattamenti_Max").ToString & "$" &
                                                                DrFrDosi(0).Item("Mdi_Cod").ToString & "$" &
                                                                DrFrDosi(0).Item("Flag_Protetto").ToString & "$" &
                                                                DrFrDosi(0).Item("FormulatiXAllegatiNormative_IDRiga").ToString & "$" &
                                                                DrFrDosi(0).Item("DataSmaltimentoScorte").ToString & "£"
                                            End If
                                        Next
                                    End If
                                    If strdose <> "" Then
                                        strdose = Left(strdose, strdose.Length - 1)
                                    End If




                                    'se valorizzato fr_des_prec mostro quello (nome vecchio)
                                    'altrimenti se nel testo ho ex lo elimino dalla visualizzazione (nuovo nome)    
                                    If Fr_Des_Prec <> "" Then
                                        Fr_Des = Fr_Des_Prec
                                    End If
                                    If InStr(LCase(Fr_Des), "(ex") <> 0 Then
                                        Fr_Des = Left(Fr_Des, InStr(LCase(Fr_Des), "(ex") - 1)
                                    End If

                                    Dim strLotto As String = ""

                                    '(15/03/2019 fede) se utilizzo il lotto aggiungo le altre giacenze
                                    strGiacenzaTesto = ""
                                    strGiacenzaVal = ""
                                    strGiacenzaTotaleVal = ""
                                    strUdmCod = ""
                                    If Dt_Giacenze IsNot Nothing AndAlso Dt_Giacenze.Rows.Count > 0 Then
                                        DrGiacenze = Dt_Giacenze.Select("pro_cod=" & Fr_Cod)
                                    End If
                                    If Dt_Giacenze_Tot IsNot Nothing AndAlso Dt_Giacenze_Tot.Rows.Count > 0 Then
                                        DrGiacenze_Tot = Dt_Giacenze_Tot.Select("pro_cod=" & Fr_Cod)
                                    End If

                                    If utilizzaLotto AndAlso Fabbricato_Cod <> "0" Then

                                        If DrGiacenze IsNot Nothing AndAlso DrGiacenze.Length > 0 Then

                                            For g = 0 To DrGiacenze.Length - 1

                                                strGiacenzaTesto = " --- " & My.Resources.AgronicaControlli_2010.Giacenza & ": " & My.Resources.AgronicaControlli_2010.AllaData & " " & Math.Round(DrGiacenze(g).Item("Giacenza"), 4).ToString & " " & DrGiacenze(g).Item("Udm_Sim")
                                                strGiacenzaVal = Math.Round(DrGiacenze(g).Item("Giacenza"), 4).ToString
                                                strUdmCod = DrGiacenze(g).Item("Udm_Cod")

                                                If DrGiacenze_Tot IsNot Nothing AndAlso DrGiacenze_Tot.Length > 0 Then
                                                    For gt = 0 To DrGiacenze_Tot.Length - 1
                                                        If DrGiacenze_Tot(gt).Item("lotto") = DrGiacenze(g).Item("lotto") Then
                                                            strGiacenzaTesto &= " " & My.Resources.AgronicaControlli_2010.Totale & " " & Math.Round(DrGiacenze_Tot(gt).Item("Giacenza"), 4).ToString & " " & DrGiacenze_Tot(gt).Item("Udm_Sim")
                                                            strGiacenzaTotaleVal = Math.Round(DrGiacenze_Tot(gt).Item("Giacenza"), 4).ToString
                                                            Exit For
                                                        End If
                                                    Next
                                                End If

                                                If strGiacenzaTesto <> "" Then
                                                    strGiacenzaTesto &= " --- " & My.Resources.AgronicaControlli_2010.Lotto & " " & DrGiacenze(g).Item("lotto")
                                                End If

                                                strLotto = DrGiacenze(g).Item("lotto")

                                                strValue = Fr_Cod & "£" &
                                                            InRevisione & "£" &
                                                            DataAttoNormativo & "£" &
                                                            Carenza & "£" &
                                                            FormulatiXAllegatiNormative_IDRiga & "£" &
                                                            For_Veg_Cod & "£" &
                                                            TipoRichiesto & "£" &
                                                            strPrincipi & "£" &
                                                            strPrincipiPesi & "£" &
                                                            strCLTOSS_COD & "£" &
                                                            BufferMin & "£" & BufferMax & "£" &
                                                            strEpocheBlocchi & "£" &
                                                            strLotto & "£" &
                                                            strGiacenzaVal & "£" &
                                                            strGiacenzaTotaleVal & "£" &
                                                            strUdmCod & "£" &
                                                            strdose


                                                Dr = DtFormulatiOrdinati.NewRow
                                                Dr.Item("Testo") = Fr_Des & " (" & Fr_Cod & ")" & strCarenza & strPA_DesTot & strClassificazioni & strClassToss & strBufferZone & strProtezione & strModalitaImpiego & strSmaltimento & strGiacenzaTesto
                                                Dr.Item("Valore") = strValue
                                                DtFormulatiOrdinati.Rows.Add(Dr)

                                            Next

                                        End If

                                    Else

                                        If DrGiacenze IsNot Nothing AndAlso DrGiacenze.Length > 0 Then
                                            strGiacenzaTesto = " --- " & My.Resources.AgronicaControlli_2010.Giacenza & ": " & My.Resources.AgronicaControlli_2010.AllaData & " " & Math.Round(DrGiacenze(0).Item("Giacenza"), 4).ToString & " " & DrGiacenze(0).Item("Udm_Sim")
                                            strGiacenzaVal = Math.Round(DrGiacenze(0).Item("Giacenza"), 4).ToString
                                            strUdmCod = DrGiacenze(0).Item("Udm_Cod")
                                        End If

                                        If DrGiacenze_Tot IsNot Nothing AndAlso DrGiacenze_Tot.Length > 0 Then
                                            strGiacenzaTesto &= " " & My.Resources.AgronicaControlli_2010.Totale & " " & Math.Round(DrGiacenze_Tot(0).Item("Giacenza"), 4).ToString & " " & DrGiacenze_Tot(0).Item("Udm_Sim")
                                            strGiacenzaTotaleVal = Math.Round(DrGiacenze_Tot(0).Item("Giacenza"), 4).ToString
                                        End If

                                        strValue = Fr_Cod & "£" &
                                                    InRevisione & "£" &
                                                    DataAttoNormativo & "£" &
                                                    Carenza & "£" &
                                                    FormulatiXAllegatiNormative_IDRiga & "£" &
                                                    For_Veg_Cod & "£" &
                                                    TipoRichiesto & "£" &
                                                    strPrincipi & "£" &
                                                    strPrincipiPesi & "£" &
                                                    strCLTOSS_COD & "£" &
                                                    BufferMin & "£" & BufferMax & "£" &
                                                    strEpocheBlocchi & "£" &
                                                    strLotto & "£" &
                                                    strGiacenzaVal & "£" &
                                                    strGiacenzaTotaleVal & "£" &
                                                    strUdmCod & "£" &
                                                    strdose

                                        Dr = DtFormulatiOrdinati.NewRow
                                        Dr.Item("Testo") = Fr_Des & " (" & Fr_Cod & ")" & strCarenza & strPA_DesTot & strClassificazioni & strClassToss & strBufferZone & strProtezione & strModalitaImpiego & strSmaltimento & strGiacenzaTesto
                                        Dr.Item("Valore") = strValue
                                        DtFormulatiOrdinati.Rows.Add(Dr)

                                    End If

                                Next

                            End If

                        Next

                    End If

            End Select

        End If

        If DtFormulatiOrdinati IsNot Nothing Then

            'uso il dataview per ordinare 
            Dim Dv As New DataView()
            DtFormulatiOrdinati.TableName = "Prodotti"
            Dv.Table = DtFormulatiOrdinati
            Dv.Sort = "Testo ASC"

            For i = 0 To Dv.Count - 1
                ddl_Formulati.Items.Add(New ListItem(Dv(i).Item("Testo"), Dv(i).Item("Valore")))
            Next

        End If


    End Sub

#Region "JS"
    Public Function GetJS()
        Dim StrSelect As New StringBuilder

        If Not Bootstrap Then
            StrSelect.AppendLine("$(document).ready(function () { ")
            StrSelect.AppendLine("   $('#" & ddl_Formulati.ClientID & "').combobox();")
            StrSelect.AppendLine("});")
        Else
            StrSelect.AppendLine("$(document).ready(function () { ")
            StrSelect.AppendLine("   $('#" & ddl_Formulati.ClientID & "').parent().find('button').click();")
            StrSelect.AppendLine("});")
        End If

        Return StrSelect.ToString
    End Function
#End Region

End Class



Class Formulato
    Dim fr_cod As Integer
    Dim fr_des As String
    Dim pa_cod As Integer
    Dim tempocarenza As Integer
    Dim gradotossicita As String
    Dim Lista_Dosi As List(Of Dose)


    Public Sub CaricaFormulato(ByVal XmlFormulato As System.Xml.XmlElement)
        fr_cod = XmlFormulato.GetAttribute("fr_cod")
        fr_des = XmlFormulato.GetAttribute("fr_des")
        pa_cod = XmlFormulato.GetAttribute("pa_cod")
        tempocarenza = XmlFormulato.GetAttribute("tempocarenza")
        gradotossicita = XmlFormulato.GetAttribute("gradotossicita")
        'per ogni nodo <DOSE> lo carico
        Lista_Dosi = New List(Of Dose)
        Dim XmlDosi As System.Xml.XmlNodeList = XmlFormulato.GetElementsByTagName("DOSE")
        If XmlDosi IsNot Nothing Then
            For i = 0 To XmlDosi.Count - 1
                Dim dose As New Dose
                Dim XmlDose = XmlDosi.Item(i)

                dose.CaricaDose(XmlDose)

                'lo aggiungo all lista
                Lista_Dosi.Add(dose)
            Next
        End If
    End Sub



    Public Function GeneraValue_STR()
        Dim STR As New StringBuilder
        STR.Append(fr_cod & "£" & fr_des & "£" & tempocarenza & "£" & pa_cod & "£" & gradotossicita & "£")
        'poi aggiungo tutte le info sulle dosi
        Dim i As Integer
        For i = 0 To Lista_Dosi.Count - 1
            STR.Append(Lista_Dosi(i).GeneraValue_STR())
            If i <> Lista_Dosi.Count - 1 Then
                STR.Append("&")
            End If
        Next
        Return STR.ToString
    End Function

    Public Sub CreaOggetto_da_stringa(ByVal StringaValori As String)

        'carico i dati dell'oggetto Formulato
        fr_cod = StringaValori.Split("£")(0)
        fr_des = StringaValori.Split("£")(1)
        tempocarenza = StringaValori.Split("£")(2)
        pa_cod = StringaValori.Split("£")(3)
        gradotossicita = StringaValori.Split("£")(4)

        'il quinto contiene tutte le dosi
        Dim StringaDosi As String
        StringaDosi = StringaValori.Split("£")(5)

    End Sub


End Class


Class Dose
    Dim For_Veg_Av_Dos_Cod As Integer
    Dim dose_min As Decimal
    Dim dose_max As Decimal
    Dim udm_cod As Integer
    Dim udm_sim As String
    Dim da_epoca_1 As Integer
    Dim a_epoca_1 As Integer
    Dim acqua_min As Decimal
    Dim acqua_max As Decimal
    Dim acqua_udm_cod As Integer
    Dim acqua_udm_sim As String

    Public Function GeneraValue_STR() As String
        Dim STR As New StringBuilder
        STR.Append(For_Veg_Av_Dos_Cod & "$" & dose_min & "$" & dose_max & "$" & udm_cod & "$" & udm_sim & "$" & da_epoca_1 & "$" &
                   a_epoca_1 & "$" & acqua_min & "$" & acqua_max & "$" & acqua_udm_cod & "$" & acqua_udm_sim)
        Return STR.ToString
    End Function



    Public Sub CaricaDose(ByVal XmlDose As System.Xml.XmlElement)
        For_Veg_Av_Dos_Cod = XmlDose.GetAttribute("for_veg_av_dos_cod")
        dose_min = XmlDose.GetAttribute("dose_min")
        dose_max = XmlDose.GetAttribute("dose_max")
        udm_cod = XmlDose.GetAttribute("udm_cod")
        udm_sim = XmlDose.GetAttribute("udm_sim")
        da_epoca_1 = XmlDose.GetAttribute("da_epoca_1")
        a_epoca_1 = XmlDose.GetAttribute("a_epoca_1")
        acqua_min = XmlDose.GetAttribute("acqua_min")
        acqua_max = XmlDose.GetAttribute("acqua_max")
        udm_sim = XmlDose.GetAttribute("udm_sim")
        acqua_udm_cod = XmlDose.GetAttribute("acqua_udm_cod")
        acqua_udm_sim = XmlDose.GetAttribute("acqua_udm_sim")
    End Sub




End Class

Public Class Comune

    Public Prov As String
    Public Com As String

    Sub New()
        Prov = "000"
        Com = "000"
    End Sub

End Class