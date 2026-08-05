Imports System.Web.Services
Imports System.Text
Imports System.Data
Imports System.Web
Imports System.Web.Script.Serialization
Imports System.Web.Script.Services
Imports AgronicaCoreDataProvider.CostantiPersonalizzate


<DefaultProperty("Text"), ToolboxData("<{0}:ComboRapportiContabili runat=server></{0}:ComboRapportiContabili>")> Public Class ComboRapportiContabiliMulti
    Inherits System.Web.UI.WebControls.WebControl

    Public ddl_RapCon As ListBox

    Protected Overrides Sub OnInit(ByVal e As System.EventArgs)
        ddl_RapCon = New ListBox
        ddl_RapCon.SelectionMode = ListSelectionMode.Multiple
        ddl_RapCon.ID = Me.ClientID & "ComboRapportiContabili"
        ddl_RapCon.CssClass = "myCombo ComboRapportiContabili"
        ddl_RapCon.Width = "250"

        Me.Controls.Add(ddl_RapCon)
        MyBase.OnInit(e)
    End Sub

#Region "Proprietà"

    Public ReadOnly Property Valori_Combo As List(Of String)
        Get
            Dim valori As New List(Of String)
            For i = 0 To ddl_RapCon.Items.Count - 1
                If ddl_RapCon.Items(i).Selected Then
                    valori.Add(ddl_RapCon.Items(i).Value)
                End If
            Next
            Return valori
        End Get
    End Property


#End Region


    Public Sub CaricaComboRapportiContabili()

        AgronicaCoreUtility.CaricaListControl.RapportiContabili(ddl_RapCon,
                                                 False, _
                                                 "Tutti i Rapporti Contabili", "0", _
                                                 SACOD_CONTATTO_NONDEFINITO, False,
                                                  0, _
                                                  False, False, False, False, False, False, False, _
                                                 "", "", _
                                                 HttpContext.Current.Session("ASG_objParametri_Server"))


    End Sub



#Region "JS"


    Public Function GetJS2()

        Dim StrSelect As New StringBuilder
        'http://wenzhixin.net.cn/p/multiple-select/#the-placeholder
        StrSelect.AppendLine("$(document).ready(function () { ")

        'width: 100,filter: true, 
        Dim Parametro_Filtro As String = "false"
        Dim Parametro_Testo As String = "'Tutti i Rapporti Contabili'" 'placeholder: "Here is the placeholder"
        StrSelect.AppendLine("   $('#" & ddl_RapCon.ClientID & "').multipleSelect({ " & _
                             "filter: " & Parametro_Filtro & ", " & _
                             "placeholder: " & Parametro_Testo & ", " & _
                             "" & _
                             "" & _
                             "" & _
                             "onClose: function(){comboRapConChiusa();}" & _
                             "});")

        StrSelect.AppendLine("});")

        Return StrSelect.ToString
    End Function

#End Region

End Class

