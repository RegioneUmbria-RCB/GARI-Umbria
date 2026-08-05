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


''' <summary>
''' Per utilizzare questo controllo è necessario includere all'iterno della pagina i seguenti script 
''' jquery.cookie.js
''' jquery.hotkeys.js
''' jquery.jstree.js
''' RICORDARE DI AGGIUNGERE LA FUNZIONE JS
''' function DoPostBack_Combo($_combo,valoreOpt) {
'''        if($_combo.attr("id").endsWith("ComboMagazzini")){
'''            //salvo nel campo nascosto il nome della funzione da richiamare
'''            $("#<%=fooName_PostedBack.ClientID %>").val("comboLavorazioni_IndexChanged");
'''            //scatena il postback simulato il click del btn nascosto
'''            $("#<%=btnDummyPostback.ClientID %>").click();
'''        }
''' </summary>
''' <remarks></remarks>

<DefaultProperty("Text"), ToolboxData("<{0}:ComboDisciplinari runat=server></{0}:ComboDisciplinari>")> Public Class ComboDisciplinari
    Inherits System.Web.UI.WebControls.WebControl

    'Public Class ComboOperazioni
    '    Inherits ScriptControl
    '    Implements IScriptControl

    Public ddl_Disciplinari As DropDownList
    Private _Bootstrap As Boolean
    Private _Reg_Cod As Integer
    Private _Disciplinare_Cod As Integer
    Private _Veg_Cod As String
    Private _Id_RcDpi As Integer
    Private _Flag_Privato_Pubblico As Integer
    Private _Includi_Nessuno As Boolean
    Private _Includi_Biologico As Boolean
    Private _Consultazione As Boolean
    Private _Tipo_Testata As Integer
    Private _ricerca As Boolean
    Private _Includi_Dir_Nitrati As Boolean
    Private _Includi_NessunoNessuno As Boolean 'no dpi no etichetta

    Private _FinestraTemporaleInizio As Date
    Private _FinestraTemporaleFine As Date
    Private _WS_Disciplinari_AgroWS_Disciplinari As String

    Public TestoNessunDisciplinare As String = My.Resources.AgronicaControlli_2010.NessunDisciplinare
    Public TestoNessunDisciplinareNessunVincoloNormativo As String = My.Resources.AgronicaControlli_2010.NessunDisciplinareNessunVincoloNormativo


    Public Sub New()

        _Reg_Cod = 0
        _Disciplinare_Cod = 0
        _Veg_Cod = 0
        _Id_RcDpi = 0
        _Flag_Privato_Pubblico = 0
        _Includi_Nessuno = True
        _Includi_Biologico = True
        _Consultazione = False
        _ricerca = True
        _Tipo_Testata = -1
        _WS_Disciplinari_AgroWS_Disciplinari = ""

        _FinestraTemporaleInizio = Date.Now
        _FinestraTemporaleFine = Date.Now
        _Bootstrap = False

        _Includi_Dir_Nitrati = False
        _Includi_NessunoNessuno = False

    End Sub

    Protected Overrides Sub OnInit(ByVal e As System.EventArgs)

        ddl_Disciplinari = New DropDownList
        ddl_Disciplinari.ID = Me.ClientID & "ComboDisciplinari"
        If Bootstrap = False Then
            ddl_Disciplinari.CssClass = "myCombo ComboDisciplinari"
        Else
            ddl_Disciplinari.CssClass = "selectpicker ComboDisciplinari"
            If Ricerca = True Then
                ddl_Disciplinari.Attributes.Add("data-live-search", "true")
            End If
            ddl_Disciplinari.Attributes.Add("data-container", "body")
        End If

        'ddl_Disciplinari.ClientIDMode = UI.ClientIDMode.Static
        Me.Controls.Add(ddl_Disciplinari)
        MyBase.OnInit(e)
    End Sub


#Region "Proprietà"



    Public Property Ricerca() As Boolean
        Get
            Return _ricerca
        End Get
        Set(ByVal value As Boolean)
            _ricerca = value
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
    Public Property WS_Disciplinari_AgroWS_Disciplinari() As String
        Get
            Return _WS_Disciplinari_AgroWS_Disciplinari
        End Get
        Set(ByVal value As String)
            _WS_Disciplinari_AgroWS_Disciplinari = value
        End Set
    End Property
    Public Property Reg_Cod() As Integer
        Get
            Return _Reg_Cod
        End Get
        Set(ByVal value As Integer)
            _Reg_Cod = value
        End Set
    End Property
    Public Property Disciplinare_Cod() As Integer
        Get
            Return _Disciplinare_Cod
        End Get
        Set(ByVal value As Integer)
            _Disciplinare_Cod = value
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
    Public Property Id_RcDpi() As Integer
        Get
            Return _Id_RcDpi
        End Get
        Set(ByVal value As Integer)
            _Id_RcDpi = value
        End Set
    End Property
    Public Property Includi_Nessuno() As Boolean
        Get
            Return _Includi_Nessuno
        End Get
        Set(ByVal value As Boolean)
            _Includi_Nessuno = value
        End Set
    End Property
    Public Property Includi_Biologico() As Boolean
        Get
            Return _Includi_Biologico
        End Get
        Set(ByVal value As Boolean)
            _Includi_Biologico = value
        End Set
    End Property

    Public Property Includi_Dir_Nitrati() As Boolean
        Get
            Return _Includi_Dir_Nitrati
        End Get
        Set(ByVal value As Boolean)
            _Includi_Dir_Nitrati = value
        End Set
    End Property

    Public Property Includi_NessunoNessuno() As Boolean
        Get
            Return _Includi_NessunoNessuno
        End Get
        Set(ByVal value As Boolean)
            _Includi_NessunoNessuno = value
        End Set
    End Property

    Public Property Consultazione() As Boolean
        Get
            Return _Consultazione
        End Get
        Set(ByVal value As Boolean)
            _Consultazione = value
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

    Public Property Flag_Privato_Pubblico() As Integer
        Get
            Return _Flag_Privato_Pubblico
        End Get
        Set(ByVal value As Integer)
            _Flag_Privato_Pubblico = value
        End Set
    End Property



    Public Property FinestraTemporaleFine() As Date
        Get
            Return _FinestraTemporaleFine
        End Get
        Set(ByVal value As Date)
            _FinestraTemporaleFine = value
        End Set
    End Property
    Public Property FinestraTemporaleInizio() As Date
        Get
            Return _FinestraTemporaleInizio
        End Get
        Set(ByVal value As Date)
            _FinestraTemporaleInizio = value
        End Set
    End Property



    Public Property Valore_Combo() As String
        Get
            Return ddl_Disciplinari.SelectedValue
        End Get
        Set(ByVal value As String)
            If Not IsNothing(value) Then
                If Not IsNothing(ddl_Disciplinari.Items.FindByValue(value)) Then
                    ddl_Disciplinari.Items.FindByValue(value).Selected = True
                End If
            End If
        End Set
    End Property

    Public Property Testo_Combo() As String
        Get
            Return ddl_Disciplinari.SelectedItem.Text
        End Get
        Set(ByVal value As String)
            If Not IsNothing(value) Then
                If Not IsNothing(ddl_Disciplinari.Items.FindByText(value)) Then
                    ddl_Disciplinari.Items.FindByText(value).Selected = True
                End If
            End If
        End Set
    End Property

    Public Property Indice_Combo() As Integer
        Get
            Return ddl_Disciplinari.SelectedIndex
        End Get
        Set(ByVal value As Integer)
            If Not IsNothing(value) Then
                If Not IsNothing(ddl_Disciplinari.Items.Count >= value) Then
                    ddl_Disciplinari.SelectedIndex = value
                End If
            End If
        End Set
    End Property

    Public Property N_Dpi() As Integer
        Get
            Return ddl_Disciplinari.Items.Count - 1
        End Get
        Set(ByVal value As Integer)
        End Set
    End Property

#End Region



    Public Sub CaricaComboDisciplinari()

        ddl_Disciplinari.Items.Clear()

        Select Case _Tipo_Testata

            Case enum_Disciplinare_Tipo_Testata.Difesa, enum_Disciplinare_Tipo_Testata.Diserbo,
                         enum_Disciplinare_Tipo_Testata.Fitoregolatore

                If _Veg_Cod.Split("/")(0) > 0 Then

                    Dim ObjDownloadWs As WS_Disciplinari.AgroWS_Disciplinari
                    Dim Dati As String = ""

                    Dim DatiRcdpi As String = ""

                    Dim XmlDocumento As New System.Xml.XmlDocument
                    Dim StrErr As String = ""

                    Dim Dpi_Des As String = ""

                    Try
                        Dim objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri
                        objParametri_Utenti = New AgronicaCoreDataProvider.AgronicaCoreParametri(
                                                    HttpContext.Current.Session("ASG_objParametri_Utenti"))
                        ObjDownloadWs = New WS_Disciplinari.AgroWS_Disciplinari

                        Dim objWs As New AgronicaCoreVarieDAL.ConnessioneWS

                        objWs.NewWS(ObjDownloadWs,
                                            _WS_Disciplinari_AgroWS_Disciplinari,
                                            objParametri_Utenti)

                        Dim objCaricaCombo As New AgronicaCoreDpiBIZ.CaricaListControl

                        objCaricaCombo.Disciplinari_ElencoxTestata(ddl_Disciplinari,
                                                        False, "", "",
                                                        HttpContext.Current.Session,
                                                        HttpContext.Current.Session("ASG_objParametri_Server"),
                                                        HttpContext.Current.Session("ASG_objParametri_Utenti"),
                                                        CInt(_Reg_Cod),
                                                        CInt(_Veg_Cod),
                                                        CInt(_Id_RcDpi),
                                                        CInt(_Flag_Privato_Pubblico),
                                                        CDate(_FinestraTemporaleFine),
                                                        CDate(_FinestraTemporaleFine),
                                                        _Includi_Nessuno, TestoNessunDisciplinare,
                                                        _Includi_Biologico, "",
                                                        False,
                                                        _Tipo_Testata)

                        If _Includi_NessunoNessuno Then
                            ddl_Disciplinari.Items.Add(New ListItem(TestoNessunDisciplinareNessunVincoloNormativo, "-999"))
                        End If

                    Catch ex As Exception
                        ''Aggiungo la voce nulla 
                        If _Includi_Nessuno Then
                            ddl_Disciplinari.Items.Add(New ListItem(TestoNessunDisciplinare, "0"))
                        End If
                        If _Includi_NessunoNessuno Then
                            ddl_Disciplinari.Items.Add(New ListItem(TestoNessunDisciplinareNessunVincoloNormativo, "-999"))
                        End If
                    End Try

                Else

                    If _Includi_Nessuno Then
                        ddl_Disciplinari.Items.Add(New ListItem(TestoNessunDisciplinare, "0"))
                    End If
                    If _Includi_NessunoNessuno Then
                        ddl_Disciplinari.Items.Add(New ListItem(TestoNessunDisciplinareNessunVincoloNormativo, "-999"))
                    End If

                End If


            Case enum_Disciplinare_Tipo_Testata.Fertilizzazione


                Try

                    Dim FiltroTipo As String = ""
                    If _Veg_Cod.Split("/")(0) > 0 Then
                        FiltroTipo = " Tipo=" & enum_PUARegolamenti_Tipo.PianoComcimazione
                        If _Includi_Dir_Nitrati = True Then
                            FiltroTipo = " ( Tipo=1 OR Tipo=2 )"
                        End If
                    Else
                        If _Includi_Dir_Nitrati = True Then
                            FiltroTipo = " Tipo=" & enum_PUARegolamenti_Tipo.PUA
                        End If
                    End If

                    ListControl_PianoConcimazione_WS.PUA_Regolamento_WS_xAgenda(ddl_Disciplinari, _Includi_Nessuno, TestoNessunDisciplinare, "0",
                                                            _Includi_Biologico,
                                                            0,
                                                            CDate(_FinestraTemporaleFine),
                                                            CDate(_FinestraTemporaleFine),
                                                            FiltroTipo, " Ordine desc ")

                Catch ex As Exception
                    ''Aggiungo la voce nulla 
                    If _Includi_Nessuno Then
                        ddl_Disciplinari.Items.Add(New ListItem(TestoNessunDisciplinare, "0"))
                    End If

                End Try

        End Select


    End Sub


#Region "JS"
    Public Function GetJS()

        Dim StrSelect As New StringBuilder

        'StrSelect.AppendLine("<script type='text/javascript'>")


        If Bootstrap = False Then
            StrSelect.AppendLine("$(document).ready(function () { ")
            StrSelect.AppendLine("   $('#" & ddl_Disciplinari.ClientID & "').combobox();")
            StrSelect.AppendLine("});")
        End If



        'StrSelect.AppendLine("</script>")
        Return StrSelect.ToString
    End Function

#End Region
End Class
