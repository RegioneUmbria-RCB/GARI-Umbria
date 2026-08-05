

Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.WebControls

Public Class web_AspNet

    Public Shared Sub BindDataTableInDropdown(ByVal ddl As DropDownList, ByVal dt As DataTable, ByVal dataTextField As String, ByVal dataValueField As String)
        With ddl
            .Items.Clear()
            .DataSource = dt
            .DataTextField = dataTextField
            .DataValueField = dataValueField
            .DataBind()
        End With
    End Sub

    Public Shared Function GetPostBackControl(ByVal page As Page) As Control
        Dim postbackControlInstance As Control = Nothing
        Dim postbackControlName As String = page.Request.Params.[Get]("__EVENTTARGET")

        If postbackControlName IsNot Nothing AndAlso postbackControlName <> String.Empty Then
            postbackControlInstance = page.FindControl(postbackControlName)
        Else

            For i As Integer = 0 To page.Request.Form.Keys.Count - 1
                postbackControlInstance = page.FindControl(page.Request.Form.Keys(i))

                If TypeOf postbackControlInstance Is System.Web.UI.WebControls.Button Then
                    Return postbackControlInstance
                End If
            Next
        End If

        If postbackControlInstance Is Nothing Then

            For i As Integer = 0 To page.Request.Form.Count - 1

                If (page.Request.Form.Keys(i).EndsWith(".x")) OrElse (page.Request.Form.Keys(i).EndsWith(".y")) Then
                    postbackControlInstance = page.FindControl(page.Request.Form.Keys(i).Substring(0, page.Request.Form.Keys(i).Length - 2))
                    Return postbackControlInstance
                End If
            Next
        End If

        Return postbackControlInstance
    End Function

End Class


Public Class web_AspNet_GridView

    Public Shared Function GridView_IdxColonnaDaHeaderText(ByVal NomeColonna As String, ByVal g As UI.WebControls.GridView) As Integer

        For Each col As DataControlField In g.Columns
            If col.HeaderText.ToLower().Trim() = NomeColonna.ToLower().Trim() Then
                Return g.Columns.IndexOf(col)
            End If
        Next

        Return -1

    End Function

End Class
