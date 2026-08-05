

Public Class UI_ControlsHelper


    Private Shared Sub performSostituzioni(ByVal sostituzioni As String, ByVal ctl As Control)

        Select Case ctl.GetType.ToString.ToLower
            Case "system.web.ui.webcontrols.checkbox"
                Rinominami(sostituzioni, CType(ctl, CheckBox).Text)

            Case "system.web.ui.webcontrols.label"
                Rinominami(sostituzioni, CType(ctl, Label).Text)

                'Case "system.web.ui.webcontrols.listitem"
                '    Rinominami(sostituzioni, CType(ctl, ListItem).text)

        End Select

    End Sub


    Public Shared Sub RinominaControlli(ByVal sostituzioni As String, ByRef frm As Page, ByRef ctlS As Control)

        If sostituzioni = "" Then
            Exit Sub
        End If


        'passo base
        If ctlS Is Nothing Then
            For Each ctl As Control In frm.Controls

                RinominaControlli(sostituzioni, frm, ctl)
                performSostituzioni(sostituzioni, ctl)


            Next

        Else

            For Each ctl1 As Control In ctlS.Controls

                RinominaControlli(sostituzioni, frm, ctl1)

                performSostituzioni(sostituzioni, ctl1)

            Next


        End If


    End Sub

    Public Shared Sub Rinominami(ByVal Sostituzioni As String, ByRef ctl As String)

        If Sostituzioni = "" Then
            Exit Sub
        End If

        Dim sts As String = Sostituzioni
        Dim vS As String() = sts.Split("|")

        For Each v In vS

            Dim v1 As String() = v.Split("*")
            ctl = ctl.Replace(v1(0), v1(1))

        Next
    End Sub


End Class
