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

<DefaultProperty("Text"), ToolboxData("<{0}:ComboMagazzini runat=server></{0}:ComboMagazzini>")> Public Class ComboMagazzini
    Inherits System.Web.UI.WebControls.WebControl

    'Public Class ComboOperazioni
    '    Inherits ScriptControl
    '    Implements IScriptControl

    Public ddl_Magazzini As DropDownList
    Private _TipoMagazzino As Integer
    Private _Piva As String
    Private _Sa_Cod As Integer
    Private _Fabbricato_Cod As Integer
    Private _Flag_CodCentroFabbricato As Boolean
    Private _Flag_GestioneMagazziniImpresaPadre As Boolean
    Private _Flag_GestioneMagazziniTerzisti As Boolean
    Private _Pive_Terzisti As String
    Private _PrimaRiga_Flag As Boolean
    Public TestoRiga0 As String = My.Resources.AgronicaControlli_2010.NessunaGestioneDelMagazzinoAziendale

    Public _Bootstrap As Boolean
    Public _AppendTo As String
    Public _Ricerca As Boolean = True

    Public Sub New()
        _Bootstrap = False
        _TipoMagazzino = MAGAZZINO
        _Sa_Cod = 0
        _Fabbricato_Cod = 0
        _Flag_CodCentroFabbricato = True
        _Flag_GestioneMagazziniImpresaPadre = False
        _Flag_GestioneMagazziniTerzisti = False
        _Pive_Terzisti = ""
        _PrimaRiga_Flag = True
        TestoRiga0 = My.Resources.AgronicaControlli_2010.NessunaGestioneDelMagazzinoAziendale
    End Sub


    Protected Overrides Sub OnInit(ByVal e As System.EventArgs)

        ddl_Magazzini = New DropDownList
        ddl_Magazzini.ID = Me.ClientID & "ComboMagazzini"
        If (_Bootstrap = False) Then
            ddl_Magazzini.CssClass = "myCombo ComboMagazzini"
        Else
            ddl_Magazzini.CssClass = "form-control selectpicker ComboMagazzini"
            If _Ricerca = True Then
                ddl_Magazzini.Attributes.Add("data-live-search", "true")
            End If

            If _AppendTo = "" Then
                ddl_Magazzini.Attributes.Add("data-container", "body")
            Else
                ddl_Magazzini.Attributes.Add("data-container", _AppendTo)
            End If


        End If

        Me.Controls.Add(ddl_Magazzini)
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

    Public Property AppendTo() As String
        Get
            Return _AppendTo
        End Get
        Set(ByVal value As String)
            _AppendTo = value
        End Set
    End Property

    Public Property Flag_GestioneMagazziniImpresaPadre() As Boolean
        Get
            Return _Flag_GestioneMagazziniImpresaPadre
        End Get
        Set(ByVal value As Boolean)
            _Flag_GestioneMagazziniImpresaPadre = value
        End Set
    End Property

    Public Property Flag_GestioneMagazziniTerzisti() As Boolean
        Get
            Return _Flag_GestioneMagazziniTerzisti
        End Get
        Set(ByVal value As Boolean)
            _Flag_GestioneMagazziniTerzisti = value
        End Set
    End Property

    Public Property Pive_Terzisti() As String
        Get
            Return _Pive_Terzisti
        End Get
        Set(ByVal value As String)
            _Pive_Terzisti = value
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
    Public Property Flag_CodCentroFabbricato() As Boolean
        Get
            Return _Flag_CodCentroFabbricato
        End Get
        Set(ByVal value As Boolean)
            _Flag_CodCentroFabbricato = value
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


    Public Property Sa_Cod() As Integer
        Get
            Return _Sa_Cod
        End Get
        Set(ByVal value As Integer)
            _Sa_Cod = value
        End Set
    End Property


    Public Property Fabbricato_Cod() As Integer
        Get
            Return _Fabbricato_Cod
        End Get
        Set(ByVal value As Integer)
            _Fabbricato_Cod = value
        End Set
    End Property

    Public Property TipoMagazzino() As Integer
        Get
            Return _TipoMagazzino
        End Get
        Set(ByVal value As Integer)
            _TipoMagazzino = value
        End Set
    End Property

    Public Property Valore_Combo() As String
        Get
            Return ddl_Magazzini.SelectedValue
        End Get
        Set(ByVal value As String)
            If Not IsNothing(value) Then
                If Not IsNothing(ddl_Magazzini.Items.FindByValue(value)) Then
                    ddl_Magazzini.Items.FindByValue(value).Selected = True
                End If
            End If 
        End Set
    End Property

    Public Property Testo_Combo() As String
        Get
            Return ddl_Magazzini.SelectedItem.Text
        End Get
        Set(ByVal value As String)
            If Not IsNothing(value) Then
                If Not IsNothing(ddl_Magazzini.Items.FindByText(value)) Then
                    ddl_Magazzini.Items.FindByText(value).Selected = True
                End If
            End If
        End Set
    End Property

    Public Property Indice_Combo() As Integer
        Get
            Return ddl_Magazzini.SelectedIndex
        End Get
        Set(ByVal value As Integer)
            If Not IsNothing(value) Then
                If Not IsNothing(ddl_Magazzini.Items.Count >= value) Then
                    ddl_Magazzini.SelectedIndex = value
                End If
            End If
        End Set
    End Property

    Public Property N_Magazzini() As Integer
        Get
            Return ddl_Magazzini.Items.Count
        End Get
        Set(ByVal value As Integer)
        End Set
    End Property
#End Region

 
    Public Sub CaricaComboMagazzini()

        ddl_Magazzini.Items.Clear()
        'il primo item indica l assenza di magazino
        If _PrimaRiga_Flag = True Then
            ddl_Magazzini.Items.Add(New ListItem(TestoRiga0, "0"))
        End If

        Dim xFiltroAggiuntivo As String = ""
        Select Case _TipoMagazzino
            Case MAGAZZINO
                xFiltroAggiuntivo += " (Fabbricati.Tipo_Fabbricato_Cod = 20 OR Fabbricati.Tipo_Fabbricato_Cod = 50 OR Fabbricati.Tipo_Fabbricato_Cod = 120 OR Fabbricati.Tipo_Fabbricato_Cod = 121 OR Fabbricati.Tipo_Fabbricato_Cod = 122 OR Fabbricati.Tipo_Fabbricato_Cod = 123) "
            Case STALLA
                xFiltroAggiuntivo += " ( (Fabbricati.Tipo_Fabbricato_Cod >= 170 AND Fabbricati.Tipo_Fabbricato_Cod < 180) OR Fabbricati.Tipo_Fabbricato_Cod = 70 ) "
            Case FABBRICATI_NO_STALLE
                xFiltroAggiuntivo += " (Fabbricati.Tipo_Fabbricato_Cod < 170 OR Fabbricati.Tipo_Fabbricato_Cod >= 180) "
        End Select


        Dim DT As DataTable
        Dim DTTerzisti As DataTable
        Dim objFabbricati As New AgronicaCoreAnagrafeDAL.Fabbricati_R

        Dim objUtenti As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
        Dim Impostazione_Mag As String

        Impostazione_Mag = objUtenti.ImpostazioneValore1_from_ImpostazioneCod(enum_Impostazioni_Utenti.UTENTE_GESTIONE_MAGAZZINO_2, _
                                                                                HttpContext.Current.Session("ASG_objParametri_Utenti"), _
                                                                                1)




        If Flag_GestioneMagazziniImpresaPadre = False Then
                '=================================================================
                '============= GESTIONE SOLO MAGAZZINI IMPRESA ======================
                '=================================================================

                DT = objFabbricati.Leggi_2(Piva, _
                          Sa_Cod, _
                          Fabbricato_Cod, _
                          0, _
                          xFiltroAggiuntivo, _
                          " Fabbricati.Fabbricato_Des ", _
                          HttpContext.Current.Session("ASG_objParametri_Server"))



            Else
                '=================================================================
                '============= GESTIONE MAGAZZINI IMPRESA PADRE ======================
                '=================================================================

                Select Case Impostazione_Mag

                    Case "", "0"

                        DT = objFabbricati.Leggi_2(Piva, _
                            Sa_Cod, _
                            Fabbricato_Cod, _
                            0, _
                            xFiltroAggiuntivo, _
                            " Fabbricati.Fabbricato_Des ", _
                            HttpContext.Current.Session("ASG_objParametri_Server"))

                    Case Else

                        Dim Piva_Padre As String
                        Dim xFiltroAggiuntivo2 As String

                        xFiltroAggiuntivo2 = xFiltroAggiuntivo

                        If Impostazione_Mag = 1 Then
                            'magazzini del superuser
                            Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri
                            objParametri_Server = HttpContext.Current.Session("ASG_objParametri_Server")
                            Piva_Padre = objParametri_Server.PivaSuperUser
                        Else
                            'magazzini dell'impresa padre
                            Dim objGerarchia As New AgronicaCoreAnagrafeDAL.GerarchiaImprese_R
                            Dim Str_Pive As String
                            Str_Pive = objGerarchia.Ricava_Stringa_PivePadre(Piva, _
                                                                            "", _
                                                                           HttpContext.Current.Session("ASG_objParametri_Server"))

                            xFiltroAggiuntivo2 += " AND ( Fabbricati.Piva IN " + Str_Pive + ")"

                            Piva_Padre = ""
                        End If

                        DT = objFabbricati.LeggiUNIONFabbricatiImpresaPadre(Piva, _
                                                                             Sa_Cod, _
                                                                             Fabbricato_Cod, _
                                                                             Piva_Padre, _
                                                                             0, _
                                                                             0, _
                                                                             False, _
                                                                             xFiltroAggiuntivo, _
                                                                             xFiltroAggiuntivo2, _
                                                                             " Fabbricati.Fabbricato_Des ", _
                                                                             HttpContext.Current.Session("ASG_objParametri_Server"))

                End Select

            End If '_Flag_GestioneMagazziniImpresaPadre

        If (Flag_GestioneMagazziniTerzisti = True And Pive_Terzisti <> "") Then

            Dim xFiltroAggiuntivo2 As String
            DTTerzisti = objFabbricati.LeggiUNIONFabbricatiImpreseTerzisti(Piva,
                                                                     Sa_Cod,
                                                                     Fabbricato_Cod,
                                                                     Pive_Terzisti,
                                                                     False,
                                                                     xFiltroAggiuntivo,
                                                                     xFiltroAggiuntivo2,
                                                                     " Fabbricati.Fabbricato_Des ",
                                                                     HttpContext.Current.Session("ASG_objParametri_Server"))

            DT.Merge(DTTerzisti)

            DT = DT.DefaultView.ToTable(True, "Piva", "Sa_Cod", "Sa_Nome", "Rag_Soc", "Fabbricato_Des", "Fabbricato_Cod")

        End If

        Dim x_Des As String = ""
        Dim x_Cod As String = ""
        Dim Piva_Value As String

        If Not IsNothing(DT) Then

            For i = 0 To DT.Rows.Count - 1

                Piva_Value = CStr(DT.Rows(i).Item("Piva"))

                If Not (Flag_GestioneMagazziniTerzisti = True And Pive_Terzisti <> "") Then

                    If Flag_GestioneMagazziniImpresaPadre = False Then

                        x_Cod = CStr(DT.Rows(i).Item("Fabbricato_Cod")) & "|" & CStr(DT.Rows(i).Item("Sa_Cod")) + "|" + Piva_Value
                        x_Des = CStr(DT.Rows(i).Item("Fabbricato_Des")) & " (" & CStr(DT.Rows(i).Item("Sa_Nome")) & ")"

                    Else

                        Select Case Flag_CodCentroFabbricato
                            Case True
                                x_Cod = CStr(DT.Rows(i).Item("Fabbricato_Cod")) & "|" & CStr(DT.Rows(i).Item("Sa_Cod")) + "|" + Piva_Value

                                Select Case Impostazione_Mag
                                    Case "", "0"
                                        x_Des = CStr(DT.Rows(i).Item("Fabbricato_Des")) & " (" & CStr(DT.Rows(i).Item("Sa_Nome")) & ")"
                                    Case Else
                                        x_Des = CStr(DT.Rows(i).Item("Fabbricato_Des")) & _
                                                " (" & CStr(DT.Rows(i).Item("Sa_Nome")) & ")" + _
                                                " --- " & CStr(DT.Rows(i).Item("Rag_Soc"))

                                End Select


                            Case False
                                x_Cod = DT.Rows(i).Item("Fabbricato_Cod")
                                x_Des = CStr(DT.Rows(i).Item("Fabbricato_Des"))
                        End Select

                    End If

                Else

                    x_Cod = CStr(DT.Rows(i).Item("Fabbricato_Cod")) & "|" & CStr(DT.Rows(i).Item("Sa_Cod")) + "|" + Piva_Value
                    x_Des = CStr(DT.Rows(i).Item("Fabbricato_Des")) & _
                                            " (" & CStr(DT.Rows(i).Item("Sa_Nome")) & ")" + _
                                            " --- " & CStr(DT.Rows(i).Item("Rag_Soc"))
                End If

                ddl_Magazzini.Items.Add(New ListItem(x_Des, x_Cod))

            Next
        End If

    End Sub


#Region "JS"
    Public Function GetJS()
        Dim StrSelect As New StringBuilder

        'StrSelect.AppendLine("<script type='text/javascript'>")

        If _Bootstrap = False Then
            StrSelect.AppendLine("$(document).ready(function () { ")
            StrSelect.AppendLine("   $('#" & ddl_Magazzini.ClientID & "').combobox();")
            StrSelect.AppendLine("});")
        End If




        'StrSelect.AppendLine("</script>")
        Return StrSelect.ToString
    End Function

#End Region
End Class
