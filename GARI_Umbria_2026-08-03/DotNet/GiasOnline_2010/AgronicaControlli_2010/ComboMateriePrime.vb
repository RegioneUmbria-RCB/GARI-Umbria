
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



<DefaultProperty("Text"), ToolboxData("<{0}:ComboMateriePrime runat=server></{0}:ComboMateriePrime>")> Public Class ComboMateriePrime
    Inherits System.Web.UI.WebControls.WebControl

    Public ddl_MateriePrime As DropDownList



    Protected Overrides Sub OnInit(ByVal e As System.EventArgs)

        ddl_MateriePrime = New DropDownList
        ddl_MateriePrime.ID = Me.ClientID & "ComboMateriePrime"
        ddl_MateriePrime.CssClass = "myCombo ComboMateriePrime"

        Me.Controls.Add(ddl_MateriePrime)
        MyBase.OnInit(e)
    End Sub




    Public Sub CaricaComboMateriePrime(ByVal piva As String, ByVal elem_cod As Integer, ByVal veg_Cod As Integer, ByVal Cul_Cod As Integer, ByVal RicercaTesto As String, ByVal xFiltroAggiuntivo As String, ByVal xOrderBy As String, ByVal ValiditaInizio As String, ByVal ValiditaFine As String, Optional ByVal MostraCodiceArticolo As Boolean = True)

        ddl_MateriePrime.Items.Clear()

        ddl_MateriePrime.Items.Add(New ListItem("", ""))

        Dim objCore As New AgronicaCoreAnagrafeDAL.Materie_Prime_R

        Dim strFiltroTipo As String = ""

        Dim Sa_cod As Integer = 0
        Dim Mat_Cod As Integer = 0
        Dim cod_articolo As String = ""

        Dim Gen_cod As Integer = 0
        Dim Spe_cod As Integer = 0
        Dim Raz_cod As Integer = 0
        Dim iPro_cod As Integer = 0
        Dim cat_cod As Integer = 0

        Dim Mat_cod_origine As Integer = 0
        Dim piva_superuser_Origine As String = ""

        Dim Flag_AncheImportatati As Boolean = True




        Dim dtRep As DataTable = objCore.MateriePrime_Anagrafica( _
                 Piva:=piva,
                 Sa_Cod:=Sa_cod,
                 Elem_Cod:=elem_cod,
                 Mat_Cod:=Mat_Cod,
                 Cod_Articolo:=cod_articolo,
                 Veg_Cod:=veg_Cod,
                 Cul_Cod:=Cul_Cod,
                 Gen_Cod:=Gen_cod,
                 Spe_Cod:=Spe_cod,
                 Raz_Cod:=Raz_cod,
                 Ipro_Cod:=iPro_cod,
                 Cat_Cod:=cat_cod,
                 RicercaTesto:=RicercaTesto,
                 Mat_Cod_Origine:=Mat_cod_origine,
                 Piva_SuperUser_Origine:=piva_superuser_Origine,
                 Flag_AncheImportatati:=Flag_AncheImportatati,
                 xFiltroAggiuntivo:=xFiltroAggiuntivo,
                 xOrderBy:=xOrderBy,
                 objParametri_Server:=HttpContext.Current.Session("ASG_objParametri_Server"), _
                 objParametri_Utenti:=HttpContext.Current.Session("ASG_objParametri_Utenti") _
                      )

        Dim i As Integer

        Dim CodiceArticolo As String = ""
        For i = 0 To dtRep.Rows.Count - 1

            If MostraCodiceArticolo Then
                CodiceArticolo = "(" & dtRep.Rows(i).Item("Cod_Articolo") & ")"
            End If

            ddl_MateriePrime.Items.Add(New ListItem(dtRep.Rows(i).Item("Mat_Des") & CodiceArticolo, _
                                                   dtRep.Rows(i).Item("Mat_Cod")))
        Next

    End Sub


#Region "Proprietà"
    Public Property Valore_Combo() As String
        Get
            Return ddl_MateriePrime.SelectedValue
        End Get
        Set(ByVal value As String)
            If Not IsNothing(value) Then
                If Not IsNothing(ddl_MateriePrime.Items.FindByValue(value)) Then
                    ddl_MateriePrime.SelectedIndex = ddl_MateriePrime.Items.IndexOf(ddl_MateriePrime.Items.FindByValue(value))
                End If
            End If
        End Set
    End Property


    Public Property Testo_Combo() As String
        Get
            Return ddl_MateriePrime.SelectedItem.Text
        End Get
        Set(ByVal value As String)
            If Not IsNothing(value) Then
                If Not IsNothing(ddl_MateriePrime.Items.FindByText(value)) Then
                    ddl_MateriePrime.SelectedIndex = ddl_MateriePrime.Items.IndexOf(ddl_MateriePrime.Items.FindByText(value))
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
        StrSelect.AppendLine("   $('#" & ddl_MateriePrime.ClientID & "').combobox();")

        StrSelect.AppendLine("});")



        Return StrSelect.ToString
    End Function
#End Region


End Class
