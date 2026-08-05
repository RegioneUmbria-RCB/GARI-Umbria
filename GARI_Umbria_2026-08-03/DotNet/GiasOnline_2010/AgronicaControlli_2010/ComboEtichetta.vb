Imports System.Web.Services
Imports System.Text
Imports System.Data
Imports System.Web
Imports System.Web.Script.Serialization
Imports System.Web.Script.Services

Imports AgronicaCoreUtentiDAL
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreMetaSchemaDAL
Imports System.ComponentModel

<DefaultProperty("Text"), ToolboxData("<{0}:ComboEtichetta runat=server></{0}:ComboEtichetta>")> Public Class ComboEtichetta
    Inherits System.Web.UI.WebControls.WebControl

    Public ddl_Dosi As DropDownList

    Private _Lav_Cod As Integer
    Private _Fr_Cod As Integer
    Private _Veg_Cod As String
    Private _Grfi_Cod As Integer
    Private _FormulatiXAllegatiNormative_IDRiga As Integer
    Private _Copertura As String

    Private _Av_Cod As Integer
    Private _Av_Gru As Integer

    Private _Data As Date

    Private _Bootstrap As Boolean

    Private _Ricerca As Boolean = False
    'Private _Id_RcDpi As Integer
    'Private _Flag_Privato_Pubblico As Integer
    'Private _Tipo_Testata As Integer

    Private _TipoRichiesto As Integer

    Private _WS_Fitofarmaci_AgroWS_Fitofarmaci As String


    Public Sub New()
        _Bootstrap = False
        _Lav_Cod = 0
        _Fr_Cod = 0
        _Veg_Cod = 0
        _Grfi_Cod = 0
        _FormulatiXAllegatiNormative_IDRiga = 0
        _Copertura = ""

        _Av_Cod = 0
        _Av_Gru = 0

        '_Disciplinare_Cod = 0
        '_Id_RcDpi = 0
        '_Flag_Privato_Pubblico = 0
        '_Tipo_Testata = -1
        _WS_Fitofarmaci_AgroWS_Fitofarmaci = ""

        _Data = Date.Now
        '_FinestraTemporaleInizio = Date.Now
        '_FinestraTemporaleFine = Date.Now

        _TipoRichiesto = 0

    End Sub

    Protected Overrides Sub OnInit(ByVal e As System.EventArgs)

        ddl_Dosi = New DropDownList
        ddl_Dosi.ID = Me.ClientID & "ComboEtichetta"
        If _Bootstrap = False Then
            ddl_Dosi.CssClass = "myCombo ComboEtichetta"
        Else
            ddl_Dosi.CssClass = "selectpicker ComboEtichetta"
            If _Ricerca = True Then
                ddl_Dosi.Attributes.Add("data-live-search", "true")
            End If
            ddl_Dosi.Attributes.Add("data-container", "body")
        End If


        Me.Controls.Add(ddl_Dosi)
        MyBase.OnInit(e)

    End Sub



#Region "Proprietà"
    Public Property Ricerca As Boolean
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

    Public Property WS_Fitofarmaci_AgroWS_Fitofarmaci() As String
        Get
            Return _WS_Fitofarmaci_AgroWS_Fitofarmaci
        End Get
        Set(ByVal value As String)
            _WS_Fitofarmaci_AgroWS_Fitofarmaci = value
        End Set
    End Property
    Public Property Lav_Cod() As Integer
        Get
            Return _Lav_Cod
        End Get
        Set(ByVal value As Integer)
            _Lav_Cod = value
        End Set
    End Property
    Public Property Fr_Cod() As Integer
        Get
            Return _Fr_Cod
        End Get
        Set(ByVal value As Integer)
            _Fr_Cod = value
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
    Public Property Grfi_Cod() As String
        Get
            Return _Grfi_Cod
        End Get
        Set(ByVal value As String)
            _Grfi_Cod = value
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

    Public Property Av_Cod() As Integer
        Get
            Return _Av_Cod
        End Get
        Set(ByVal value As Integer)
            _Av_Cod = value
        End Set
    End Property
    Public Property Av_Gru() As Integer
        Get
            Return _Av_Gru
        End Get
        Set(ByVal value As Integer)
            _Av_Gru = value
        End Set
    End Property
    Public Property Data() As Date
        Get
            Return _Data
        End Get
        Set(ByVal value As Date)
            _Data = value
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

    ' Public Property Disciplinare_Cod() As Integer
    '     Get
    '         Return _Disciplinare_Cod
    '     End Get
    '     Set(ByVal value As Integer)
    '         _Disciplinare_Cod = value
    '     End Set
    ' End Property

    ' Public Property Id_RcDpi() As Integer
    '     Get
    '         Return _Id_RcDpi
    '     End Get
    '     Set(ByVal value As Integer)
    '         _Id_RcDpi = value
    '     End Set
    ' End Property

    'Public Property Tipo_Testata() As Integer
    '     Get
    '         Return _Tipo_Testata
    '     End Get
    '     Set(ByVal value As Integer)
    '         _Tipo_Testata = value
    '     End Set
    ' End Property

    ' Public Property Flag_Privato_Pubblico() As Integer
    '     Get
    '         Return _Flag_Privato_Pubblico
    '     End Get
    '     Set(ByVal value As Integer)
    '         _Flag_Privato_Pubblico = value
    '     End Set
    ' End Property

    Public Property FormulatiXAllegatiNormative_IDRiga() As Integer
        Get
            Return _FormulatiXAllegatiNormative_IDRiga
        End Get
        Set(ByVal value As Integer)
            _FormulatiXAllegatiNormative_IDRiga = value
        End Set
    End Property



    Public Property Valore_Combo() As String
        Get
            Return ddl_Dosi.SelectedValue
        End Get
        Set(ByVal value As String)
            If Not IsNothing(value) Then
                If Not IsNothing(ddl_Dosi.Items.FindByValue(value)) Then
                    ddl_Dosi.Items.FindByValue(value).Selected = True
                End If
            End If
        End Set
    End Property

    Public Property Testo_Combo() As String
        Get
            Return ddl_Dosi.SelectedItem.Text
        End Get
        Set(ByVal value As String)
            If Not IsNothing(value) Then
                If Not IsNothing(ddl_Dosi.Items.FindByText(value)) Then
                    ddl_Dosi.Items.FindByText(value).Selected = True
                End If
            End If
        End Set
    End Property

    Public Property Indice_Combo() As Integer
        Get
            Return ddl_Dosi.SelectedIndex
        End Get
        Set(ByVal value As Integer)
            If Not IsNothing(value) Then
                If Not IsNothing(ddl_Dosi.Items.Count >= value) Then
                    ddl_Dosi.SelectedIndex = value
                End If
            End If
        End Set
    End Property



#End Region



    Public Sub CaricaComboEtichetta_OLD()

        Dim DtDosi As DataTable

        Dim DoseText As String
        Dim DoseValue As String

        Dim Udm_Cod As Integer = 0
        Dim DoseMin, DoseMax As Decimal
        Dim Intervallo_min, Intervallo_Max As Integer
        Dim Udm_Sim As String
        Dim AcquaMin, AcquaMax As Decimal
        Dim AcquaUdm_Cod As Integer = 0
        Dim AcquaUdm_Sim As String
        Dim A_Epoca As Integer = 0
        Dim Da_Epoca As Integer = 0
        Dim Limite As Integer = 0
        Dim Limite_Udm_Cod As Integer = 0
        Dim LimiteUdm_Sim As String

        Dim Flag_Fioritura As Integer = 0

        Dim Mdi_Cod As Integer = 0
        Dim Flag_Protetto As Integer = 0

        Dim strCLTOSS_Grado As String = ""

        ddl_Dosi.Items.Clear()

        If _Veg_Cod.Split("/")(0) > 0 Then

            Try

                Dim XmlDoc As New System.Xml.XmlDocument
                Dim XML_Credenziali As System.Xml.XmlElement
                Dim StrCredenziali As String = ""
                Dim StrParametri As String = ""
                Dim strErr As String = ""
                Dim Parametri As String = ""
                'Dim DtRisultati As DataTable

                Dim objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri
                objParametri_Utenti = New AgronicaCoreDataProvider.AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Utenti"))

                Dim ObjDownloadWs As New WS_Fitofarmaci.AgroWS_Fitofarmaci
                Dim objWs As New AgronicaCoreVarieDAL.ConnessioneWS

                objWs.NewWS(ObjDownloadWs,
                            _WS_Fitofarmaci_AgroWS_Fitofarmaci,
                                objParametri_Utenti)

                Dim objCoreAgroWs As New AgronicaCoreWebService.AgroWs

                Select Case _Lav_Cod

                    Case LAVCOD_TRATTAMENTO_ANTIPARASSITARIO, LAVCOD_TRATTAMENTO_FITOREGOLATORE, LAVCOD_CONCIA_SEME, LAVCOD_GEODISINFESTAZIONE

                        objCoreAgroWs.AgroWS_XML__Credenziali(AgronicaCoreWebService.AgroWs.enum_AWS_Xml.Xml_CODIFICA,
                                   StrCredenziali,
                                   AgronicaCoreWebService.AgroWs.enum_AWS_Schede.AWS_Fitofarmaci_Formulati_SpecieVegetali_Avversita_Dosi,
                                    objCoreAgroWs.AgroWS_DoorKey(AgronicaCoreWebService.AgroWs.enum_AWS_Schede.AWS_Fitofarmaci_Formulati_SpecieVegetali_Avversita_Dosi),
                                   HttpContext.Current.Session("ASG_ProgressivoGIAS"),
                                   HttpContext.Current.Session("ASG_SuperUser_Username").ToString,
                                   HttpContext.Current.Session("ASG_SuperUser_Password").ToString)

                        XmlDoc.LoadXml(StrCredenziali)

                        XML_Credenziali = XmlDoc.SelectSingleNode("CREDENZIALI")

                        objCoreAgroWs.AgroWS_XML_Parametri_Formulati_SpecieVegetali_Avversita_Dosi(AgronicaCoreWebService.AgroWs.enum_AWS_Xml.Xml_CODIFICA,
                                                                                                  StrParametri,
                                                                                                  _Fr_Cod,
                                                                                                  _Veg_Cod,
                                                                                                  _Av_Cod,
                                                                                                  _Av_Gru,
                                                                                                  _Grfi_Cod,
                                                                                                  "0",
                                                                                                   _FormulatiXAllegatiNormative_IDRiga,
                                                                                                   "",
                                                                                                   _Data,
                                                                                                   _TipoRichiesto,
                                                                                                   strErr
                                                                                                   )

                        XML_Credenziali.InnerXml = StrParametri

                        Parametri = XmlDoc.OuterXml

                        Parametri = objCoreAgroWs.AWS_Codifica_P(Parametri)

                        DtDosi = ObjDownloadWs.Formulati_SpecieVegetali_Avversita_Dosi_DT(Parametri, strErr)


                    Case LAVCOD_DISERBO, LAVCOD_DISSECCAMENTO

                        objCoreAgroWs.AgroWS_XML__Credenziali(AgronicaCoreWebService.AgroWs.enum_AWS_Xml.Xml_CODIFICA,
                                   StrCredenziali,
                                   AgronicaCoreWebService.AgroWs.enum_AWS_Schede.AWS_Fitofarmaci_Formulati_SpecieVegetali_Infestanti_Dosi,
                                    objCoreAgroWs.AgroWS_DoorKey(AgronicaCoreWebService.AgroWs.enum_AWS_Schede.AWS_Fitofarmaci_Formulati_SpecieVegetali_Infestanti_Dosi),
                                   HttpContext.Current.Session("ASG_ProgressivoGIAS"),
                                   HttpContext.Current.Session("ASG_SuperUser_Username").ToString,
                                   HttpContext.Current.Session("ASG_SuperUser_Password").ToString)

                        XmlDoc.LoadXml(StrCredenziali)

                        XML_Credenziali = XmlDoc.SelectSingleNode("CREDENZIALI")

                        objCoreAgroWs.AgroWS_XML_Parametri_Formulati_SpecieVegetali_Infestanti_Dosi(AgronicaCoreWebService.AgroWs.enum_AWS_Xml.Xml_CODIFICA,
                                                                                                  StrParametri,
                                                                                                  _Fr_Cod,
                                                                                                  _Veg_Cod,
                                                                                                  _Av_Cod,
                                                                                                  _Av_Gru,
                                                                                                  _Grfi_Cod,
                                                                                                  "0",
                                                                                                  _FormulatiXAllegatiNormative_IDRiga,
                                                                                                  "",
                                                                                                    _Data,
                                                                                                  _TipoRichiesto,
                                                                                                    strErr
                                                                                                  )

                        XML_Credenziali.InnerXml = StrParametri

                        Parametri = XmlDoc.OuterXml

                        Parametri = objCoreAgroWs.AWS_Codifica_P(Parametri)

                        DtDosi = ObjDownloadWs.Formulati_SpecieVegetali_Infestanti_Dosi_DT(Parametri, strErr)

                End Select

            Catch ex As Exception

            End Try


            If Not DtDosi Is Nothing AndAlso DtDosi.Rows.Count > 0 Then

                Dim objUDMMetaschema As New AgronicaCoreMetaSchemaDAL.UnitaMisura_R

                For i = 0 To DtDosi.Rows.Count - 1

                    DoseMin = Math.Round(CDbl(DtDosi.Rows(i).Item("Dose_Min")), 4)
                    DoseMax = Math.Round(CDbl(DtDosi.Rows(i).Item("Dose_Max")), 4)
                    Udm_Cod = CInt(DtDosi.Rows(i).Item("Udm_Cod"))
                    Udm_Sim = DtDosi.Rows(i).Item("Udm_Sim")

                    AcquaMin = CDbl(DtDosi.Rows(i).Item("Acqua_Min"))
                    AcquaMax = CDbl(DtDosi.Rows(i).Item("Acqua_Max"))
                    AcquaUdm_Cod = CDbl(DtDosi.Rows(i).Item("Acqua_Udm_Cod"))
                    AcquaUdm_Sim = ""
                    If AcquaUdm_Cod <> 0 Then
                        objUDMMetaschema.UdmDes_from_UdmCod(AcquaUdm_Cod, AcquaUdm_Sim, HttpContext.Current.Session("ASG_objParametri_Server"))
                    End If

                    Intervallo_min = CInt(DtDosi.Rows(i).Item("IntervalloTrattamenti_Min"))
                    Intervallo_Max = CInt(DtDosi.Rows(i).Item("IntervalloTrattamenti_Max"))

                    Da_Epoca = DtDosi.Rows(i).Item("Da_Epoca_1")
                    A_Epoca = DtDosi.Rows(i).Item("a_Epoca_1")

                    Limite = DtDosi.Rows(i).Item("Limiteinterventi")
                    Limite_Udm_Cod = DtDosi.Rows(i).Item("udm_cod_limite")
                    LimiteUdm_Sim = ""
                    If Limite_Udm_Cod <> 0 Then
                        LimiteUdm_Sim = objUDMMetaschema.UdmDes_from_UdmCod(DtDosi.Rows(i).Item("udm_cod_limite"), "", HttpContext.Current.Session("ASG_objParametri_Server"))
                    End If

                    CreaStringa_DoseEtichetta_OLD(DtDosi.Rows(i).Item("For_Veg_Av_Dos_Cod"),
                                              DoseMin, DoseMax, Udm_Sim, Udm_Cod,
                                              AcquaMin, AcquaMax, AcquaUdm_Sim, AcquaUdm_Cod,
                                              Limite, LimiteUdm_Sim, Limite_Udm_Cod,
                                              Da_Epoca, A_Epoca, strCLTOSS_Grado, Flag_Fioritura,
                                              Intervallo_min, Intervallo_Max,
                                              DoseText, DoseValue)

                    ddl_Dosi.Items.Add(New ListItem(DoseText, DoseValue))

                Next

            Else

                ddl_Dosi.Items.Add(New ListItem("Dose NON Disponibile", "0"))

            End If

        End If


    End Sub

    Public Sub CaricaComboEtichetta()

        Dim DtDosi As DataTable

        Dim DoseText As String
        Dim DoseValue As String

        Dim Udm_Cod As Integer = 0
        Dim DoseMin, DoseMax As Decimal
        Dim Intervallo_min, Intervallo_Max As Integer
        Dim Udm_Sim As String
        Dim AcquaMin, AcquaMax As Decimal
        Dim AcquaUdm_Cod As Integer = 0
        Dim AcquaUdm_Sim As String
        Dim A_Epoca As Integer = 0
        Dim Da_Epoca As Integer = 0
        Dim Limite As Integer = 0
        Dim Limite_Udm_Cod As Integer = 0
        Dim LimiteUdm_Sim As String

        Dim Flag_Fioritura As Integer = 0

        Dim Mdi_Cod As Integer = 0
        Dim Flag_Protetto As Integer = 0

        Dim DataSmaltimentoScorte As String

        Dim Gruppo_Dosaggi As Integer = 0
        Dim Num_Max_Interventi_Globali As Integer = 0
        Dim FormulatiXAllegatiNormative_IDRiga As Integer = 0

        Dim strCLTOSS_Grado As String = ""

        ddl_Dosi.Items.Clear()

        If _Veg_Cod.Split("/")(0) > 0 Then

            Try

                Dim XmlDoc As New System.Xml.XmlDocument
                Dim XML_Credenziali As System.Xml.XmlElement
                Dim StrCredenziali As String = ""
                Dim StrParametri As String = ""
                Dim strErr As String = ""
                Dim Parametri As String = ""
                'Dim DtRisultati As DataTable

                Dim objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri
                objParametri_Utenti = New AgronicaCoreDataProvider.AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Utenti"))

                Dim ObjDownloadWs As New WS_Fitofarmaci.AgroWS_Fitofarmaci
                Dim objWs As New AgronicaCoreVarieDAL.ConnessioneWS

                objWs.NewWS(ObjDownloadWs,
                            _WS_Fitofarmaci_AgroWS_Fitofarmaci,
                                objParametri_Utenti)

                Dim objCoreAgroWs As New AgronicaCoreWebService.AgroWs

                Select Case _TipoRichiesto

                    Case enum_TipoFormulato.Disseccanti, enum_TipoFormulato.Diserbanti

                        objCoreAgroWs.AgroWS_XML__Credenziali(AgronicaCoreWebService.AgroWs.enum_AWS_Xml.Xml_CODIFICA,
                                  StrCredenziali,
                                  AgronicaCoreWebService.AgroWs.enum_AWS_Schede.AWS_Fitofarmaci_Formulati_SpecieVegetali_Infestanti_Dosi,
                                   objCoreAgroWs.AgroWS_DoorKey(AgronicaCoreWebService.AgroWs.enum_AWS_Schede.AWS_Fitofarmaci_Formulati_SpecieVegetali_Infestanti_Dosi),
                                  HttpContext.Current.Session("ASG_ProgressivoGIAS"),
                                  HttpContext.Current.Session("ASG_SuperUser_Username").ToString,
                                  HttpContext.Current.Session("ASG_SuperUser_Password").ToString)

                        XmlDoc.LoadXml(StrCredenziali)

                        XML_Credenziali = XmlDoc.SelectSingleNode("CREDENZIALI")

                        objCoreAgroWs.AgroWS_XML_Parametri_Formulati_SpecieVegetali_Infestanti_Dosi(AgronicaCoreWebService.AgroWs.enum_AWS_Xml.Xml_CODIFICA,
                                                                                                  StrParametri,
                                                                                                  _Fr_Cod,
                                                                                                  _Veg_Cod,
                                                                                                  _Av_Cod,
                                                                                                  _Av_Gru,
                                                                                                  _Grfi_Cod,
                                                                                                  "0",
                                                                                                  _FormulatiXAllegatiNormative_IDRiga,
                                                                                                  _Copertura,
                                                                                                    _Data,
                                                                                                  _TipoRichiesto,
                                                                                                    strErr
                                                                                                  )

                        XML_Credenziali.InnerXml = StrParametri

                        Parametri = XmlDoc.OuterXml

                        Parametri = objCoreAgroWs.AWS_Codifica_P(Parametri)

                        DtDosi = ObjDownloadWs.Formulati_SpecieVegetali_Infestanti_Dosi_DT(Parametri, strErr)

                    Case Else


                        objCoreAgroWs.AgroWS_XML__Credenziali(AgronicaCoreWebService.AgroWs.enum_AWS_Xml.Xml_CODIFICA,
                                   StrCredenziali,
                                   AgronicaCoreWebService.AgroWs.enum_AWS_Schede.AWS_Fitofarmaci_Formulati_SpecieVegetali_Avversita_Dosi,
                                    objCoreAgroWs.AgroWS_DoorKey(AgronicaCoreWebService.AgroWs.enum_AWS_Schede.AWS_Fitofarmaci_Formulati_SpecieVegetali_Avversita_Dosi),
                                   HttpContext.Current.Session("ASG_ProgressivoGIAS"),
                                   HttpContext.Current.Session("ASG_SuperUser_Username").ToString,
                                   HttpContext.Current.Session("ASG_SuperUser_Password").ToString)

                        XmlDoc.LoadXml(StrCredenziali)

                        XML_Credenziali = XmlDoc.SelectSingleNode("CREDENZIALI")

                        objCoreAgroWs.AgroWS_XML_Parametri_Formulati_SpecieVegetali_Avversita_Dosi(AgronicaCoreWebService.AgroWs.enum_AWS_Xml.Xml_CODIFICA,
                                                                                                  StrParametri,
                                                                                                  _Fr_Cod,
                                                                                                  _Veg_Cod,
                                                                                                  _Av_Cod,
                                                                                                  _Av_Gru,
                                                                                                  _Grfi_Cod,
                                                                                                  "0",
                                                                                                   _FormulatiXAllegatiNormative_IDRiga,
                                                                                                   _Copertura,
                                                                                                   _Data,
                                                                                                   _TipoRichiesto,
                                                                                                   strErr
                                                                                                   )

                        XML_Credenziali.InnerXml = StrParametri

                        Parametri = XmlDoc.OuterXml

                        Parametri = objCoreAgroWs.AWS_Codifica_P(Parametri)

                        DtDosi = ObjDownloadWs.Formulati_SpecieVegetali_Avversita_Dosi_DT(Parametri, strErr)

                End Select
                'Select Case _Lav_Cod

                '    Case LAVCOD_TRATTAMENTO_ANTIPARASSITARIO, LAVCOD_TRATTAMENTO_FITOREGOLATORE, LAVCOD_CONCIA_SEME, LAVCOD_GEODISINFESTAZIONE



                '    Case LAVCOD_DISERBO, LAVCOD_DISSECCAMENTO



                'End Select

            Catch ex As Exception

            End Try


            If Not DtDosi Is Nothing AndAlso DtDosi.Rows.Count > 0 Then

                Dim objUDMMetaschema As New AgronicaCoreMetaSchemaDAL.UnitaMisura_R

                Dim HashDosi As New Hashtable

                For i = 0 To DtDosi.Rows.Count - 1

                    DoseMin = Math.Round(CDbl(DtDosi.Rows(i).Item("Dose_Min")), 4)
                    DoseMax = Math.Round(CDbl(DtDosi.Rows(i).Item("Dose_Max")), 4)
                    Udm_Cod = CInt(DtDosi.Rows(i).Item("Udm_Cod"))
                    Udm_Sim = DtDosi.Rows(i).Item("Udm_Sim")

                    AcquaMin = CDbl(DtDosi.Rows(i).Item("Acqua_Min"))
                    AcquaMax = CDbl(DtDosi.Rows(i).Item("Acqua_Max"))
                    AcquaUdm_Cod = CDbl(DtDosi.Rows(i).Item("Acqua_Udm_Cod"))
                    AcquaUdm_Sim = ""
                    If AcquaUdm_Cod <> 0 Then
                        objUDMMetaschema.UdmDes_from_UdmCod(AcquaUdm_Cod, AcquaUdm_Sim, HttpContext.Current.Session("ASG_objParametri_Server"))
                    End If

                    Intervallo_min = CInt(DtDosi.Rows(i).Item("IntervalloTrattamenti_Min"))
                    Intervallo_Max = CInt(DtDosi.Rows(i).Item("IntervalloTrattamenti_Max"))

                    Da_Epoca = DtDosi.Rows(i).Item("Da_Epoca_1")
                    A_Epoca = DtDosi.Rows(i).Item("a_Epoca_1")

                    Limite = DtDosi.Rows(i).Item("Limiteinterventi")
                    Limite_Udm_Cod = DtDosi.Rows(i).Item("udm_cod_limite")
                    LimiteUdm_Sim = ""
                    If Limite_Udm_Cod <> 0 Then
                        LimiteUdm_Sim = objUDMMetaschema.UdmDes_from_UdmCod(DtDosi.Rows(i).Item("udm_cod_limite"), "", HttpContext.Current.Session("ASG_objParametri_Server"))
                    End If

                    Flag_Fioritura = DtDosi.Rows(i).Item("Epoca_Cod")

                    Mdi_Cod = DtDosi.Rows(i).Item("Mdi_Cod")
                    Flag_Protetto = DtDosi.Rows(i).Item("Flag_Protetto")

                    DataSmaltimentoScorte = ""
                    If Not IsDBNull(DtDosi.Rows(i).Item("DataSmaltimentoScorte")) AndAlso IsDate(DtDosi.Rows(i).Item("DataSmaltimentoScorte")) Then
                        DataSmaltimentoScorte = CDate(DtDosi.Rows(i).Item("DataSmaltimentoScorte")).ToShortDateString
                    End If

                    Gruppo_Dosaggi = 0
                    If Not IsDBNull(DtDosi.Rows(i).Item("Gruppo_Dosaggi")) AndAlso IsNumeric(DtDosi.Rows(i).Item("Gruppo_Dosaggi")) Then
                        Gruppo_Dosaggi = CInt(DtDosi.Rows(i).Item("Gruppo_Dosaggi"))
                    End If
                    Num_Max_Interventi_Globali = 0
                    If Not IsDBNull(DtDosi.Rows(i).Item("Num_Max_Interventi_Globali")) AndAlso IsNumeric(DtDosi.Rows(i).Item("Num_Max_Interventi_Globali")) Then
                        Num_Max_Interventi_Globali = CInt(DtDosi.Rows(i).Item("Num_Max_Interventi_Globali"))
                    End If

                    FormulatiXAllegatiNormative_IDRiga = 0
                    If Not IsDBNull(DtDosi.Rows(i).Item("FormulatiXAllegatiNormative_IDRiga")) AndAlso IsNumeric(DtDosi.Rows(i).Item("FormulatiXAllegatiNormative_IDRiga")) Then
                        FormulatiXAllegatiNormative_IDRiga = CInt(DtDosi.Rows(i).Item("FormulatiXAllegatiNormative_IDRiga"))
                    End If


                    If Not HashDosi.ContainsKey(DtDosi.Rows(i).Item("For_Veg_Av_Dos_Cod") & "|" &
                                                                     DataSmaltimentoScorte) Then

                            CreaStringa_DoseEtichetta(DtDosi.Rows(i).Item("For_Veg_Av_Dos_Cod"),
                                                  DoseMin, DoseMax, Udm_Sim, Udm_Cod,
                                                  AcquaMin, AcquaMax, AcquaUdm_Sim, AcquaUdm_Cod,
                                                  Limite, LimiteUdm_Sim, Limite_Udm_Cod,
                                                  Da_Epoca, A_Epoca, strCLTOSS_Grado, Flag_Fioritura,
                                                  Intervallo_min, Intervallo_Max,
                                                  Mdi_Cod, Flag_Protetto, FormulatiXAllegatiNormative_IDRiga, DataSmaltimentoScorte,
                                                  Gruppo_Dosaggi, Num_Max_Interventi_Globali,
                                                  DoseText, DoseValue)

                            ddl_Dosi.Items.Add(New ListItem(DoseText, DoseValue))
                            HashDosi.Add(DtDosi.Rows(i).Item("For_Veg_Av_Dos_Cod") & "|" &
                                                             DataSmaltimentoScorte, "")

                        End If


                    'CreaStringa_DoseEtichetta(DtDosi.Rows(i).Item("For_Veg_Av_Dos_Cod"),
                    '                          DoseMin, DoseMax, Udm_Sim, Udm_Cod,
                    '                          AcquaMin, AcquaMax, AcquaUdm_Sim, AcquaUdm_Cod,
                    '                          Limite, LimiteUdm_Sim, Limite_Udm_Cod,
                    '                          Da_Epoca, A_Epoca, strCLTOSS_Grado, Flag_Fioritura,
                    '                          Intervallo_min, Intervallo_Max,
                    '                          Mdi_Cod, Flag_Protetto, _FormulatiXAllegatiNormative_IDRiga, DataSmaltimentoScorte,
                    '                          Gruppo_Dosaggi, Num_Max_Interventi_Globali,
                    '                          DoseText, DoseValue)

                    'ddl_Dosi.Items.Add(New ListItem(DoseText, DoseValue))

                Next

            Else

                ddl_Dosi.Items.Add(New ListItem("Dose NON Disponibile", "0"))

            End If

        End If


    End Sub

    Public Function CreaStringa_DoseEtichetta_OLD(ByVal For_Veg_Av_Dos_Cod As Integer,
                                  ByVal DoseMin As String,
                                  ByVal DoseMax As String,
                                  ByVal Udm_Sim As String,
                                  ByVal Udm_Cod As Integer,
                                  ByVal AcquaMin As String,
                                  ByVal AcquaMax As String,
                                  ByVal AcquaUdm_Sim As String,
                                  ByVal AcquaUdm_Cod As Integer,
                                  ByVal Limite As String,
                                  ByVal Limite_Sim As String,
                                  ByVal Limite_Udm_Cod As Integer,
                                  ByVal Da_Epoca As String,
                                  ByVal A_Epoca As String,
                                  ByVal strCLTOSS_Grado As String,
                                  ByVal Flag_Fioritura As Integer,
                                  ByVal IntervalloTrattamenti_Min As Integer,
                                  ByVal IntervalloTrattamenti_Max As Integer,
                                  ByRef DoseText As String,
                                  ByRef DoseValue As String)

        Dim DoseEtichetta As String = ""
        DoseEtichetta &= DoseMin.ToString & "-" & DoseMax.ToString & " " & Udm_Sim.ToString

        If AcquaMin <> 0 Or AcquaMax <> 0 Then
            DoseEtichetta &= String.Format(" (Vol.Acqua {0}-{1} {2})", AcquaMin, AcquaMax, AcquaUdm_Sim)
        End If

        If Limite <> 0 Then
            DoseEtichetta &= String.Format(" (Max {0} interventi {1}) Then", Limite, Limite_Sim)
        End If

        If Flag_Fioritura <> 0 Then
            DoseEtichetta &= " Sospendere i trattamenti a fine fioritura "
        End If

        If IntervalloTrattamenti_Min <> 0 Or IntervalloTrattamenti_Max <> 0 Then
            DoseEtichetta &= " da effettuare da " & IntervalloTrattamenti_Min & " - " & IntervalloTrattamenti_Max & " gg. dal precedente trattamento"
        End If

        Dim strEpoca As String = ""
        If Da_Epoca <> 0 And A_Epoca <> 0 Then
            If Da_Epoca <> A_Epoca Then
                If Da_Epoca <> 0 Then
                    Dim objEpoca As New AgronicaCoreMetaSchemaDAL.Epoche_R
                    strEpoca = " DA " & objEpoca.EpocaDes_from_EpocaCod(Da_Epoca, HttpContext.Current.Session("ASG_objParametri_Server"))
                End If
                If A_Epoca <> 0 Then
                    Dim objEpoca As New AgronicaCoreMetaSchemaDAL.Epoche_R
                    strEpoca &= " A " & objEpoca.EpocaDes_from_EpocaCod(A_Epoca, HttpContext.Current.Session("ASG_objParametri_Server"))
                End If
            Else
                Dim objEpoca As New AgronicaCoreMetaSchemaDAL.Epoche_R
                strEpoca &= " In " & objEpoca.EpocaDes_from_EpocaCod(A_Epoca, HttpContext.Current.Session("ASG_objParametri_Server")) 'todo, in come si indica nelle risorse??
            End If
        Else
            If Da_Epoca <> 0 Then
                Dim objEpoca As New AgronicaCoreMetaSchemaDAL.Epoche_R
                strEpoca = " DA " & objEpoca.EpocaDes_from_EpocaCod(Da_Epoca, HttpContext.Current.Session("ASG_objParametri_Server"))
            End If
            If A_Epoca <> 0 Then
                Dim objEpoca As New AgronicaCoreMetaSchemaDAL.Epoche_R
                strEpoca &= " A " & objEpoca.EpocaDes_from_EpocaCod(A_Epoca, HttpContext.Current.Session("ASG_objParametri_Server"))
            End If
        End If

        If strEpoca <> "" Then
            DoseEtichetta &= strEpoca
        End If

        DoseText = DoseEtichetta

        'verifico se è dose Ettato o dose HL
        Dim Udm_Radice, per_ha_hl As Integer
        Dim objUdm As New AgronicaCoreMetaSchemaDAL.UnitaMisura_R
        objUdm.ScomponiUdm(Udm_Radice, per_ha_hl, Udm_Cod)

        If Udm_Radice <> -1 Then
            DoseValue = For_Veg_Av_Dos_Cod & "$" &
                        DoseMin & "$" & DoseMax & "$" & Udm_Cod & "$" & Udm_Sim & "$" &
                        AcquaMin & "$" & AcquaMax & "$" & AcquaUdm_Cod & "$" & AcquaUdm_Sim & "$" &
                        Da_Epoca & "$" & A_Epoca & "$" &
                        Limite & "$" & Limite_Udm_Cod & "$" & Limite_Sim & "$" &
                        strCLTOSS_Grado & "$" &
                        Flag_Fioritura & "$" &
                        IntervalloTrattamenti_Min & "$" & IntervalloTrattamenti_Max
        End If

                    Return DoseEtichetta

    End Function

    Public Function CreaStringa_DoseEtichetta(ByVal For_Veg_Av_Dos_Cod As Integer,
                                  ByVal DoseMin As String,
                                  ByVal DoseMax As String,
                                  ByVal Udm_Sim As String,
                                  ByVal Udm_Cod As Integer,
                                  ByVal AcquaMin As String,
                                  ByVal AcquaMax As String,
                                  ByVal AcquaUdm_Sim As String,
                                  ByVal AcquaUdm_Cod As Integer,
                                  ByVal Limite As String,
                                  ByVal Limite_Sim As String,
                                  ByVal Limite_Udm_Cod As Integer,
                                  ByVal Da_Epoca As String,
                                  ByVal A_Epoca As String,
                                  ByVal strCLTOSS_Grado As String,
                                  ByVal Flag_Fioritura As Integer,
                                  ByVal IntervalloTrattamenti_Min As Integer,
                                  ByVal IntervalloTrattamenti_Max As Integer,
                                              ByVal Mdi_Cod As Integer,
                                              ByVal Flag_Protetto As Integer,
                                                ByVal FormulatiXAllegatiNormative_IDRiga As Integer,
                                                ByVal DataSmaltimentoScorte As String,
                                                    ByVal Gruppo_Dosaggi As Integer,
                                                    ByVal Num_Max_Interventi_Globali As Integer,
                                  ByRef DoseText As String,
                                  ByRef DoseValue As String)

        Dim DoseEtichetta As String = ""
        DoseEtichetta &= DoseMin.ToString & "-" & DoseMax.ToString & " " & Udm_Sim.ToString

        If AcquaMin <> 0 Or AcquaMax <> 0 Then
            DoseEtichetta &= String.Format(" (Vol.Acqua {0}-{1} {2})", AcquaMin, AcquaMax, AcquaUdm_Sim)
        End If

        If Limite <> 0 Then
            DoseEtichetta &= String.Format(" (Max {0} interventi {1})", Limite, Limite_Sim)
        End If

        If Flag_Fioritura <> 0 Then
            DoseEtichetta &= " Sospendere i trattamenti a fine fioritura "
        End If

        If IntervalloTrattamenti_Min <> 0 Or IntervalloTrattamenti_Max <> 0 Then
            DoseEtichetta &= " da effettuare da " & IntervalloTrattamenti_Min & " - " & IntervalloTrattamenti_Max & " gg. dal precedente trattamento"
        End If

        Dim strEpoca As String = ""
        If Da_Epoca <> 0 And A_Epoca <> 0 Then
            If Da_Epoca <> A_Epoca Then
                If Da_Epoca <> 0 Then
                    Dim objEpoca As New AgronicaCoreMetaSchemaDAL.Epoche_R
                    strEpoca = " DA " & objEpoca.EpocaDes_from_EpocaCod(Da_Epoca, HttpContext.Current.Session("ASG_objParametri_Server"))
                End If
                If A_Epoca <> 0 Then
                    Dim objEpoca As New AgronicaCoreMetaSchemaDAL.Epoche_R
                    strEpoca &= " A " & objEpoca.EpocaDes_from_EpocaCod(A_Epoca, HttpContext.Current.Session("ASG_objParametri_Server"))
                End If
            Else
                Dim objEpoca As New AgronicaCoreMetaSchemaDAL.Epoche_R
                strEpoca &= " in " & objEpoca.EpocaDes_from_EpocaCod(A_Epoca, HttpContext.Current.Session("ASG_objParametri_Server")) 'todo, in come si indica nelle risorse??
            End If
        Else
            If Da_Epoca <> 0 Then
                Dim objEpoca As New AgronicaCoreMetaSchemaDAL.Epoche_R
                strEpoca = " DA " & objEpoca.EpocaDes_from_EpocaCod(Da_Epoca, HttpContext.Current.Session("ASG_objParametri_Server"))
            End If
            If A_Epoca <> 0 Then
                Dim objEpoca As New AgronicaCoreMetaSchemaDAL.Epoche_R
                strEpoca &= " A " & objEpoca.EpocaDes_from_EpocaCod(A_Epoca, HttpContext.Current.Session("ASG_objParametri_Server"))
            End If
        End If

        If strEpoca <> "" Then
            DoseEtichetta &= strEpoca
        End If

        If Mdi_Cod <> 0 Then
            Dim objModalita As New AgronicaCoreMetaSchemaDAL.ModalitaImpiego_R
            DoseEtichetta &= " - " & objModalita.MdiDes_from_MdiCod(Mdi_Cod, HttpContext.Current.Session("ASG_objParametri_Server"))
        End If


        Select Case Flag_Protetto
            Case 1
                DoseEtichetta &= " - Serra "
            Case 2
                DoseEtichetta &= " - Pieno Campo "
        End Select

        If DataSmaltimentoScorte <> "" Then
            DoseEtichetta &= " --- [Fine Scorta al " & DataSmaltimentoScorte & "]"
        End If

        DoseText = DoseEtichetta

        'verifico se è dose Ettato o dose HL
        Dim Udm_Radice, per_ha_hl As Integer
        Dim objUdm As New AgronicaCoreMetaSchemaDAL.UnitaMisura_R
        objUdm.ScomponiUdm(Udm_Radice, per_ha_hl, Udm_Cod)

        If Udm_Radice <> -1 Then
            DoseValue = For_Veg_Av_Dos_Cod & "$" &
                        DoseMin & "$" & DoseMax & "$" & Udm_Cod & "$" & Udm_Sim & "$" &
                        AcquaMin & "$" & AcquaMax & "$" & AcquaUdm_Cod & "$" & AcquaUdm_Sim & "$" &
                        Da_Epoca & "$" & A_Epoca & "$" &
                        Limite & "$" & Limite_Udm_Cod & "$" & Limite_Sim & "$" &
                        strCLTOSS_Grado & "$" &
                        Flag_Fioritura & "$" &
                        IntervalloTrattamenti_Min & "$" & IntervalloTrattamenti_Max & "$" &
                            Mdi_Cod & "$" & Flag_Protetto & "$" & FormulatiXAllegatiNormative_IDRiga & "$" & DataSmaltimentoScorte &
                                "$" & Gruppo_Dosaggi & "$" & Num_Max_Interventi_Globali

        End If

        Return DoseEtichetta

    End Function

    Public Function ValoriDose_OLD(ByVal strDose As String,
                            ByRef For_Veg_Av_Dos_Cod As Integer,
                       ByRef DoseMin As Decimal,
                       ByRef DoseMax As Decimal,
                       ByRef Udm_Cod As Integer,
                       ByRef Udm_Sim As String,
                       ByRef AcquaMin As Decimal,
                       ByRef AcquaMax As Decimal,
                       ByRef AcquaUdm_Cod As Integer,
                       ByRef AcquaUdm_Sim As String,
                       ByRef Da_Epoca As Integer,
                       ByRef A_Epoca As Integer,
                       ByRef Limite As Integer,
                       ByRef Limite_Udm_Cod As Integer,
                       ByRef Limite_Sim As String,
                       ByRef strCLTOSS_Grado As String,
                       ByRef Flag_Fioritura As Integer,
                       ByRef IntervalloTrattamenti_Min As Integer,
                       ByRef IntervalloTrattamenti_Max As Integer) As Boolean

        Dim DosePresente As Boolean = False

        Dim ArrayDose() As String

        If strDose <> "" And strDose <> "0" Then

            DosePresente = True

            ArrayDose = Split(strDose, "$")

            If Not ArrayDose Is Nothing AndAlso ArrayDose.Length > 0 Then

                For_Veg_Av_Dos_Cod = ArrayDose(0)

                DoseMin = ArrayDose(1)
                DoseMax = ArrayDose(2)
                Udm_Cod = ArrayDose(3)
                Udm_Sim = ArrayDose(4)

                AcquaMin = ArrayDose(5)
                AcquaMax = ArrayDose(6)
                AcquaUdm_Cod = ArrayDose(7)
                AcquaUdm_Sim = ArrayDose(8)

                Da_Epoca = ArrayDose(9)
                A_Epoca = ArrayDose(10)

                Limite = ArrayDose(11)
                Limite_Udm_Cod = ArrayDose(12)
                Limite_Sim = ArrayDose(13)

                Select Case ArrayDose.Length

                    Case 20

                        strCLTOSS_Grado = ""

                        Flag_Fioritura = ArrayDose(14)

                        IntervalloTrattamenti_Min = ArrayDose(15)
                        IntervalloTrattamenti_Max = ArrayDose(16)

                    Case Else

                        strCLTOSS_Grado = ArrayDose(14)

                        Flag_Fioritura = ArrayDose(15)

                        IntervalloTrattamenti_Min = ArrayDose(16)
                        IntervalloTrattamenti_Max = ArrayDose(17)

                End Select



            End If

        End If

        Return DosePresente

    End Function

    Public Function ValoriDose(ByVal strDose As String,
                            ByRef For_Veg_Av_Dos_Cod As Integer,
                       ByRef DoseMin As Decimal,
                       ByRef DoseMax As Decimal,
                       ByRef Udm_Cod As Integer,
                       ByRef Udm_Sim As String,
                       ByRef AcquaMin As Decimal,
                       ByRef AcquaMax As Decimal,
                       ByRef AcquaUdm_Cod As Integer,
                       ByRef AcquaUdm_Sim As String,
                       ByRef Da_Epoca As Integer,
                       ByRef A_Epoca As Integer,
                       ByRef Limite As Integer,
                       ByRef Limite_Udm_Cod As Integer,
                       ByRef Limite_Sim As String,
                       ByRef strCLTOSS_Grado As String,
                       ByRef Flag_Fioritura As Integer,
                       ByRef IntervalloTrattamenti_Min As Integer,
                       ByRef IntervalloTrattamenti_Max As Integer,
                               ByRef Mdi_Cod As Integer,
                               ByRef Flag_Protetto As Integer,
                               ByRef FormulatiXAllegatiNormative_IDRiga As Integer,
                               ByRef DataSmaltimentoScorte As String,
                                                    ByRef Gruppo_Dosaggi As Integer,
                                                    ByRef Num_Max_Interventi_Globali As Integer) As Boolean

        Dim DosePresente As Boolean = False

        Dim ArrayDose() As String

        If strDose <> "" And strDose <> "0" Then

            DosePresente = True

            ArrayDose = Split(strDose, "$")

            If Not ArrayDose Is Nothing AndAlso ArrayDose.Length > 0 Then

                For_Veg_Av_Dos_Cod = ArrayDose(0)

                DoseMin = ArrayDose(1)
                DoseMax = ArrayDose(2)
                Udm_Cod = ArrayDose(3)
                Udm_Sim = ArrayDose(4)

                AcquaMin = ArrayDose(5)
                AcquaMax = ArrayDose(6)
                AcquaUdm_Cod = ArrayDose(7)
                AcquaUdm_Sim = ArrayDose(8)

                Da_Epoca = ArrayDose(9)
                A_Epoca = ArrayDose(10)

                Limite = ArrayDose(11)
                Limite_Udm_Cod = ArrayDose(12)
                Limite_Sim = ArrayDose(13)

                Select Case ArrayDose.Length

                    Case 20

                        strCLTOSS_Grado = ""

                        Flag_Fioritura = ArrayDose(14)

                        IntervalloTrattamenti_Min = ArrayDose(15)
                        IntervalloTrattamenti_Max = ArrayDose(16)

                        Mdi_Cod = ArrayDose(17)
                        Flag_Protetto = ArrayDose(18)
                        FormulatiXAllegatiNormative_IDRiga = ArrayDose(19)
                        DataSmaltimentoScorte = ArrayDose(20)
                        Gruppo_Dosaggi = ArrayDose(21)
                        Num_Max_Interventi_Globali = ArrayDose(22)

                    Case Else

                        strCLTOSS_Grado = ArrayDose(14)

                        Flag_Fioritura = ArrayDose(15)

                        IntervalloTrattamenti_Min = ArrayDose(16)
                        IntervalloTrattamenti_Max = ArrayDose(17)

                        Mdi_Cod = ArrayDose(18)
                        Flag_Protetto = ArrayDose(19)
                        FormulatiXAllegatiNormative_IDRiga = ArrayDose(20)
                        DataSmaltimentoScorte = ArrayDose(21)
                        Gruppo_Dosaggi = ArrayDose(22)
                        Num_Max_Interventi_Globali = ArrayDose(23)

                End Select



            End If

        End If

        Return DosePresente

    End Function

#Region "JS"
    Public Function GetJS()

        Dim StrSelect As New StringBuilder

        If Bootstrap = False Then
            StrSelect.AppendLine("$(document).ready(function () { ")
            StrSelect.AppendLine("   $('#" & ddl_Dosi.ClientID & "').combobox();")
            StrSelect.AppendLine("   $('#" & ddl_Dosi.ClientID & "').combobox().parent().find('input.ui-autocomplete-input').css('width', '85%');")
            StrSelect.AppendLine("});")
        Else
            If ddl_Dosi.Items.Count > 1 Then
                StrSelect.AppendLine("$(document).ready(function () { ")
                StrSelect.AppendLine("   $('#" & ddl_Dosi.ClientID & "').parent().find('button').click();")
                StrSelect.AppendLine("});")
            End If

        End If

        Return StrSelect.ToString

    End Function

#End Region
End Class



