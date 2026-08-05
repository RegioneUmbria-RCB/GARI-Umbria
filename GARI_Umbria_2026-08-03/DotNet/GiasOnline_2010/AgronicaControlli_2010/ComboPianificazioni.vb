

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


<DefaultProperty("Text"), ToolboxData("<{0}:ComboPianificazioni runat=server></{0}:ComboPianificazioni>")> Public Class ComboPianificazioni
    Inherits System.Web.UI.WebControls.WebControl

    Public ddl_Pianificazioni As DropDownList



    Protected Overrides Sub OnInit(ByVal e As System.EventArgs)

        ddl_Pianificazioni = New DropDownList
        ddl_Pianificazioni.ID = Me.ClientID & "ComboPianificazioni"
        ddl_Pianificazioni.CssClass = "myCombo ComboPianificazioni"

        Me.Controls.Add(ddl_Pianificazioni)
        MyBase.OnInit(e)
    End Sub




    Public Sub CaricaComboPianificazioni(ByVal piva As String, ByVal veg_cod As Integer, ByVal ValiditaInizio As String, ByVal ValiditaFine As String, Optional byval sa_cod As Integer = 0 )

        ddl_Pianificazioni.Items.Clear()

        ddl_Pianificazioni.Items.Add(New ListItem("", ""))

        Dim objCore As New AgronicaCoreAnagrafeDAL.Programmazione_Testata_R
        'visualizzo le pianificazioni annuali e quindicinali
        Dim strFiltroTipo As String = " Tipo_Pianificazione IN (0, 2) "

        Dim strErr As String
        Dim dtRep As DataTable = objCore.Leggi_xCombo( _
                       strErr, _
                       0, _
                       piva, _
                       sa_cod, _ 
                       veg_cod, _
                       "", _
                       0, _
                       ValiditaInizio, _
                       ValiditaFine, _
                       enum_TipoRicetta.Non_Filtrare, _
                       AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, _
                       strFiltroTipo, " t.Validita_Inizio DESC ", _
                       HttpContext.Current.Session("ASG_objParametri_Server"))

        Dim i As Integer

        For i = 0 To dtRep.Rows.Count - 1

            ddl_Pianificazioni.Items.Add(New ListItem(dtRep.Rows(i).Item("Programmazione_Des"), _
                                                   dtRep.Rows(i).Item("Programmazione_Cod")))
        Next

    End Sub


#Region "Proprietà"
    Public Property Valore_Combo() As String
        Get
            Return ddl_Pianificazioni.SelectedValue
        End Get
        Set(ByVal value As String)
            If Not IsNothing(value) Then
                If Not IsNothing(ddl_Pianificazioni.Items.FindByValue(value)) Then
                    ddl_Pianificazioni.SelectedIndex = ddl_Pianificazioni.Items.IndexOf(ddl_Pianificazioni.Items.FindByValue(value))
                End If
            End If
        End Set
    End Property


    Public Property Testo_Combo() As String
        Get
            Return ddl_Pianificazioni.SelectedItem.Text
        End Get
        Set(ByVal value As String)
            If Not IsNothing(value) Then
                If Not IsNothing(ddl_Pianificazioni.Items.FindByText(value)) Then
                    ddl_Pianificazioni.SelectedIndex = ddl_Pianificazioni.Items.IndexOf(ddl_Pianificazioni.Items.FindByText(value))
                End If
            End If
        End Set
    End Property
#End Region

#Region "JS"
    Public Function GetJS()
        Dim StrSelect As New StringBuilder

        'StrSelect.AppendLine("<script type='text/javascript'>")


        StrSelect.AppendLine("$(document).ready(function () { ")
        StrSelect.AppendLine("   $('#" & ddl_Pianificazioni.ClientID & "').combobox();")

        StrSelect.AppendLine("});")



        Return StrSelect.ToString
    End Function
#End Region


End Class
