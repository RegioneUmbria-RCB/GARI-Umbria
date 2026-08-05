Imports System.Web.Services
Imports System.Text
Imports System.Data
Imports System.Web
Imports System.Web.Script.Serialization
Imports System.Web.Script.Services

Imports AgronicaCoreUtentiDAL
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreMetaSchemaDAL
Imports System.ComponentModel
Imports AgronicaCoreDataProvider.CostantiPersonalizzate


''' <summary>
''' Per utilizzare questo controllo è necessario includere all'iterno della pagina i seguenti script 
''' jquery.cookie.js
''' jquery.hotkeys.js
''' jquery.jstree.js
''' RICORDARE DI AGGIUNGERE LA FUNZIONE JS
''' function DoPostBack_Combo($_combo,valoreOpt) {
'''        if($_combo.attr("id").endsWith("ddlLavorazioni")){
'''            //salvo nel campo nascosto il nome della funzione da richiamare
'''            $("#<%=fooName_PostedBack.ClientID %>").val("comboLavorazioni_IndexChanged");
'''            //scatena il postback simulato il click del btn nascosto
'''            $("#<%=btnDummyPostback.ClientID %>").click();
'''        }
''' </summary>
''' <remarks></remarks>

<DefaultProperty("Text"), ToolboxData("<{0}:ComboOperazioni runat=server></{0}:ComboOperazioni>")> Public Class ComboOperazioni
    Inherits System.Web.UI.WebControls.WebControl

    Private _Bootstrap As Boolean
    Public ddl_Operazioni As DropDownList
    Private _Tipo_GruppoOperazioni As String = "'C'"
    Private _Ricerca As Boolean = True

    Public Sub New()
        _Bootstrap = False
    End Sub


    Protected Overrides Sub OnInit(ByVal e As System.EventArgs)

        ddl_Operazioni = New DropDownList
        ddl_Operazioni.ID = Me.ClientID & "ComboOperazioni"
        If _Bootstrap = False Then
            ddl_Operazioni.CssClass = "myCombo ComboOperazioni"
        Else
            ddl_Operazioni.CssClass = "selectpicker ComboOperazioni"
            If _Ricerca = True Then
                ddl_Operazioni.Attributes.Add("data-live-search", "true")
            End If

            ddl_Operazioni.Attributes.Add("data-container", "body")
        End If


        Me.Controls.Add(ddl_Operazioni)
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

    Public Property Valore_Combo() As String
        Get
            Return ddl_Operazioni.SelectedValue
        End Get
        Set(ByVal value As String)
            If Not IsNothing(value) Then
                If Not IsNothing(ddl_Operazioni.Items.FindByValue(value)) Then
                    ddl_Operazioni.SelectedIndex = ddl_Operazioni.Items.IndexOf(ddl_Operazioni.Items.FindByValue(value))
                End If
            End If
        End Set
    End Property


    Public Property Testo_Combo() As String
        Get
            Return ddl_Operazioni.SelectedItem.Text
        End Get
        Set(ByVal value As String)
            If Not IsNothing(value) Then
                If Not IsNothing(ddl_Operazioni.Items.FindByText(value)) Then
                    ddl_Operazioni.SelectedIndex = ddl_Operazioni.Items.IndexOf(ddl_Operazioni.Items.FindByText(value))
                End If
            End If
        End Set
    End Property

    Public Property Tipo_GruppoOperazioni() As String
        Get
            Return _Tipo_GruppoOperazioni
        End Get
        Set(ByVal value As String)
            _Tipo_GruppoOperazioni = value
        End Set
    End Property

#End Region
    'Protected Overrides Function GetScriptDescriptors() As IEnumerable(Of ScriptDescriptor)
    '    Dim descriptor As New ScriptControlDescriptor("AgronicaControlli_2010.ComboOperazioni", Me.ClientID)

    '    Return New List(Of ScriptDescriptor) From {descriptor}
    'End Function

    '' Generare il riferimento allo script
    'Protected Overrides Function GetScriptReferences() As IEnumerable(Of ScriptReference)
    '    Dim scriptRef As New ScriptReference("AgronicaControlli_2010.ClientControl1.js", Me.GetType().Assembly.FullName)

    '    Return New List(Of ScriptReference) From {scriptRef}
    'End Function






    Public Sub CaricaComboLavorazioni(ByVal FiltraImpostazioniUtente As Boolean, Optional ByVal Filtro As String = "")

        ddl_Operazioni.Items.Clear()

        ddl_Operazioni.Items.Add(New ListItem("", ""))

        Dim DTOperazioni As DataTable

        Dim Flag_FiltroOperazioniUtente As Boolean = False

        Dim objUtente As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read

        Dim filtroUtente As String = ""
        Dim dt_FiltroUtente As DataTable

        dt_FiltroUtente = objUtente.Leggi(enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_LAVORAZIONI, _
                                                       1, _
                                                       AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni, _
                                                    "", "", _
                                                    HttpContext.Current.Session("ASG_objParametri_Utenti"))
        If dt_FiltroUtente.Rows.Count > 0 Then
            filtroUtente = " AND Operazioni.Lav_Cod in ("
            Dim j As Integer = 0
            For j = 0 To dt_FiltroUtente.Rows.Count - 1

                If j <> 0 Then
                    filtroUtente = filtroUtente & " ,"
                End If
                filtroUtente = filtroUtente & dt_FiltroUtente.Rows(j).Item("ID_0")
            Next
            filtroUtente = filtroUtente & " )  "
            Flag_FiltroOperazioniUtente = True
        End If

        Dim FiltroAggiuntivo As String = ""



        'FiltroAggiuntivo = STR_OP_NON_GESTITE + "AND ( GruppoOperazioni.Tipo IN ('C','Z','P','E')) "
        FiltroAggiuntivo = STR_OP_NON_GESTITE & " " & Filtro & " AND  GruppoOperazioni.Tipo IN (" & _Tipo_GruppoOperazioni & ")"

        FiltroAggiuntivo = FiltroAggiuntivo & filtroUtente

        ' Dim Ordinamento As String = " GruppoOperazioni.Tipo, GruppoOperazioni.GRU_COD, Lav_Des "
        Dim Ordinamento As String = " Operazioni.Lav_Des "

        Dim objOperazioniLeggi As New AgronicaCoreMetaSchemaDAL.Operazioni_R

        DTOperazioni = objOperazioniLeggi.Leggi(0, 0, 0, "", 0, "", "", False, False, False, False, _
                                                    AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni, _
                                                    FiltroAggiuntivo, _
                                                    Ordinamento, _
                                                    HttpContext.Current.Session("ASG_objParametri_Server"))



        Dim i As Integer

        For i = 0 To DTOperazioni.Rows.Count - 1

            ddl_Operazioni.Items.Add(New ListItem(DTOperazioni.Rows(i).Item("LAV_DES"), _
                                                   DTOperazioni.Rows(i).Item("LAV_COD")))
        Next

    End Sub


#Region "JS"
    Public Function GetJS()
        Dim StrSelect As New StringBuilder

        'StrSelect.AppendLine("<script type='text/javascript'>")

        If Bootstrap = False Then
            StrSelect.AppendLine("$(document).ready(function () { ")
            StrSelect.AppendLine("   $('#" & ddl_Operazioni.ClientID & "').combobox();")

            StrSelect.AppendLine("});")
        End If


        'StrSelect.AppendLine("</script>")
        Return StrSelect.ToString
    End Function
#End Region

End Class
