

<DefaultProperty("Text"), ToolboxData("<{0}:CalendarioAperto runat=server></{0}:CalendarioAperto>")> Public Class CalendarioAperto
    Inherits System.Web.UI.WebControls.WebControl

    'Public Class CalendarioAperto
    '    Inherits ScriptControl

    Private _Abilitato As Boolean = True
    Public PanCal As Panel
    Public HidData As HiddenField

    Protected Overrides Sub OnInit(ByVal e As System.EventArgs)

        PanCal = New Panel
        PanCal.CssClass = "datepicker"
        'PanCal.ClientIDMode = UI.ClientIDMode.Static

        HidData = New HiddenField
        'HidData.ClientIDMode = UI.ClientIDMode.Static

        PanCal.ID = "Data" & Me.ClientID
        HidData.ID = "Hidden" & Me.ClientID

        HidData.Value = Date.Now

        Me.Controls.Add(PanCal)
        Me.Controls.Add(HidData)

        MyBase.OnInit(e)
    End Sub

    'Protected Overrides Function GetScriptDescriptors() As IEnumerable(Of ScriptDescriptor)
    '    Dim descriptor As New ScriptControlDescriptor("AgronicaControlli_2010.CalendarioAperto", Me.ID)

    '    Return New List(Of ScriptDescriptor) From {descriptor}
    'End Function

    '' Generare il riferimento allo script
    'Protected Overrides Function GetScriptReferences() As IEnumerable(Of ScriptReference)
    '    Dim scriptRef As New ScriptReference("AgronicaControlli_2010.ClientControl1.js", Me.GetType().Assembly.FullName)

    '    'Return New List(Of ScriptReference) From {scriptRef}
    '    Return New List(Of ScriptReference)
    'End Function


    Public Property Abilitato() As Boolean
        Get
            Return _Abilitato
        End Get
        Set(ByVal value As Boolean)
            _Abilitato = value
        End Set
    End Property


    Public Property SelectedDate() As String
        Get
            Return HidData.Value
        End Get
        Set(ByVal value As String)
            HidData.Value = value
            If HidData.ClientID <> "" Then
                Me.Controls.Add(New LiteralControl("<script type='text/javascript'> " + _
                                    "$(document).ready(function () {" + _
                                    "   $('#" + HidData.ClientID + _
                                    "       ').val($('#" + PanCal.ClientID + "').val());" +
                                    "   });" + _
                                    "</script>"))
            End If
        End Set
    End Property

    'Script.Text = "<script type='text/javascript'>$('#<%=data_Istantanea.ID%>').val($('#<%=Calendario.ID%>').val());</script>"

    Protected Overrides Sub Render(ByVal writer As System.Web.UI.HtmlTextWriter)
        Dim MyRnd As New Random



        Dim js As String = "<script type='text/javascript'> " + _
                "$(document).ready(function () {" + _
                "   $('#" + PanCal.ClientID + "').datepicker({" + _
                "   dateFormat:  'dd/mm/yy', changeYear: true, " + _
                "   changeMonth: true, " + _
                "   onChangeMonthYear: function(year, month, inst) { $('#" + HidData.ClientID + "').val(inst.currentDay + '/'+month+'/'+year);  }, " + _
                "   onSelect: function(dateText, inst) {  $('#" + HidData.ClientID + "').val($('#" + PanCal.ClientID + "').val()); }, " + _
                "   disabled: " + IIf(_Abilitato = False, "true", "false") + "});" + _
                "  " + _
                "   $('#" + PanCal.ClientID + "').click(function () {" + _
                "   $('#" + HidData.ClientID + "').val($('#" + PanCal.ClientID + "').val())" + _
                "   });" + _
                "   if ($('#" + HidData.ClientID + "').val() != '') {" + _
                "       $('#" + PanCal.ClientID + "').datepicker('setDate', " + _
                "           $('#" + HidData.ClientID + "').val()); }" + _
                "   });" + _
                "</script>"



        If HidData.Value <> "" Then
            Me.Controls.Add(New LiteralControl("<script type='text/javascript'> " + _
                                "$(document).ready(function () {" + _
                                "   $('#" + HidData.ClientID + _
                                "       ').val($('#" + PanCal.ClientID + "').val());" +
                                "   });" + _
                                "</script>"))
        End If
        'aggiungo il js
        writer.Write(js)
        MyBase.Render(writer)
    End Sub


End Class