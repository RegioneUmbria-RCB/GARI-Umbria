Imports System.Web.UI

Public Class Messaggi

    '################################################################
    'Public Shared Sub AgroMsgBox(ByVal Testo As String, _
    '                      ByRef objPage As System.Web.UI.Page, _
    '                      Optional ByVal NomeForm As String = "FORM1", _
    '                      Optional ByVal UpdatePanel As UpdatePanel = Nothing, _
    '                      Optional ByVal AltroScriptDaAppendere As String = "", _
    '                      Optional ByVal usaRegisterStartupScript As Boolean = False _
    '    )

    '    '----- Formatto il testo di ingresso in modo che non crei problemi ...

    '    'Elimino il carattere \ e lo sostituisco con \\
    '    Testo = Replace(Testo, "\", "\\")

    '    'Elimino il carattere (o coppia di caratteri) VBCRLF che crea problemi ...
    '    Testo = Replace(Testo, vbCrLf, Chr(13))

    '    'Elimino i doppi apici sostituendoli con due singoli apostrofi chr(96)
    '    Testo = Replace(Testo, Chr(34), Chr(96) & Chr(96))

    '    'Elimino i singoli apici sostituendoli con un apostrofo chr(96)
    '    Testo = Replace(Testo, Chr(39), Chr(96))

    '    'Gli eventuali Chr(13) li sostituisco con l'equivalente JS
    '    'attenzione! va messo dopo la sostituzione di \ con \\ !!!
    '    Testo = Replace(Testo, Chr(13), "\r")

    '    If UpdatePanel Is Nothing Then

    '        If Not objPage.FindControl(NomeForm) Is Nothing Then

    '            objPage.FindControl(NomeForm).Controls.Add( _
    '                New LiteralControl( _
    '                    "<script language='javascript'>$( window ).load(function () {MessaggioErrore('" & Testo & "'); " & AltroScriptDaAppendere & " });</script>"))

    '        Else

    '            If Not objPage.Master.FindControl(NomeForm) Is Nothing Then

    '                objPage.Master.FindControl(NomeForm).Controls.Add( _
    '                    New LiteralControl( _
    '                        "<script language='javascript'>$( window ).load(function () {MessaggioErrore('" & Testo & "'); " & AltroScriptDaAppendere & " });</script>"))

    '            Else

    '                If Not objPage.Master.Master.FindControl(NomeForm) Is Nothing Then

    '                    objPage.Master.Master.FindControl(NomeForm).Controls.Add( _
    '                        New LiteralControl( _
    '                            "<script language='javascript'>$( window ).load(function () {MessaggioErrore('" & Testo & "');  " & AltroScriptDaAppendere & " });</script>"))
    '                End If
    '            End If
    '        End If
    '    Else
    '        If usaRegisterStartupScript Then
    '            ScriptManager.RegisterStartupScript(UpdatePanel, UpdatePanel.GetType, "alert", "$( window ).load(function () {MessaggioErrore('" & Testo & "'); " & AltroScriptDaAppendere & "  });", True)
    '        Else
    '            ScriptManager.RegisterClientScriptBlock(UpdatePanel, UpdatePanel.GetType, "alert", "$( window ).load(function () {MessaggioErrore('" & Testo & "'); " & AltroScriptDaAppendere & "  });", True)
    '        End If

    '    End If

    'End Sub
    Public Shared Sub AgroMsgBox(ByVal Testo As String, _
                         ByRef objPage As System.Web.UI.Page, _
                         Optional ByVal NomeForm As String = "FORM1", _
                         Optional ByVal UpdatePanel As UpdatePanel = Nothing, _
                         Optional ByVal AltroScriptDaAppendere As String = "", _
                         Optional ByVal usaRegisterStartupScript As Boolean = False _
       )

        '----- Formatto il testo di ingresso in modo che non crei problemi ...

        'Elimino il carattere \ e lo sostituisco con \\
        Testo = Replace(Testo, "\", "\\")

        'Elimino il carattere (o coppia di caratteri) VBCRLF che crea problemi ...
        Testo = Replace(Testo, vbCrLf, Chr(13))

        'Elimino i doppi apici sostituendoli con due singoli apostrofi chr(96)
        Testo = Replace(Testo, Chr(34), Chr(96) & Chr(96))

        'Elimino i singoli apici sostituendoli con un apostrofo chr(96)
        Testo = Replace(Testo, Chr(39), Chr(96))

        'Gli eventuali Chr(13) li sostituisco con l'equivalente JS
        'attenzione! va messo dopo la sostituzione di \ con \\ !!!
        Testo = Replace(Testo, Chr(13), "\r")

        If UpdatePanel Is Nothing Then
            Try
                If Not objPage.FindControl(NomeForm) Is Nothing Then
                    objPage.FindControl(NomeForm).Controls.Add(
                        New LiteralControl(
                            "<script language='javascript'>$(document).ready(function () {MessaggioErrore('" & Testo & "'); " & AltroScriptDaAppendere & " });</script>"))
                Else

                    If Not objPage.Master.FindControl(NomeForm) Is Nothing Then
                        objPage.Master.FindControl(NomeForm).Controls.Add(
                            New LiteralControl(
                                "<script language='javascript'>$(document).ready(function () {MessaggioErrore('" & Testo & "'); " & AltroScriptDaAppendere & " });</script>"))
                    Else

                        If Not objPage.Master.Master.FindControl(NomeForm) Is Nothing Then

                            objPage.Master.Master.FindControl(NomeForm).Controls.Add(
                                New LiteralControl(
                                    "<script language='javascript'>$(document).ready(function () {MessaggioErrore('" & Testo & "');  " & AltroScriptDaAppendere & " });</script>"))
                        Else
                            If Not Ricerca.FindControlIterative2(objPage, NomeForm) Is Nothing Then

                                Ricerca.FindControlIterative2(objPage, NomeForm).Controls.Add(
                                    New LiteralControl(
                                        "<script language='javascript'>$(document).ready(function () {MessaggioErrore('" & Testo & "'); " & AltroScriptDaAppendere & " });</script>"))
                            End If
                        End If
                    End If
                End If
            Catch ex As Exception
                ScriptManager.RegisterClientScriptBlock(objPage, objPage.GetType(), "alert", "alert('" & Testo & "');", True)
                'ScriptManager.RegisterClientScriptBlock(objPage, objPage.GetType(), "alert", "$(document).ready(function () {MessaggioErrore('" & Testo & "'); " & AltroScriptDaAppendere & "  });", True)
            End Try
        Else
            If usaRegisterStartupScript Then
                ScriptManager.RegisterStartupScript(UpdatePanel, UpdatePanel.GetType, "alert", "$(document).ready(function () {MessaggioErrore('" & Testo & "'); " & AltroScriptDaAppendere & "  });", True)
            Else
                ScriptManager.RegisterClientScriptBlock(UpdatePanel, UpdatePanel.GetType, "alert", "$(document).ready(function () {MessaggioErrore('" & Testo & "'); " & AltroScriptDaAppendere & "  });", True)
            End If

        End If

    End Sub

    Private Sub AgroMsgBoxNew(Testo, Page)
        '----- Formatto il testo di ingresso in modo che non crei problemi ...

        'Elimino il carattere \ e lo sostituisco con \\
        Testo = Replace(Testo, "\", "\\")

        'Elimino il carattere (o coppia di caratteri) VBCRLF che crea problemi ...
        Testo = Replace(Testo, vbCrLf, Chr(13))

        'Elimino i doppi apici sostituendoli con due singoli apostrofi chr(96)
        Testo = Replace(Testo, Chr(34), Chr(96) & Chr(96))

        'Elimino i singoli apici sostituendoli con un apostrofo chr(96)
        Testo = Replace(Testo, Chr(39), Chr(96))

        'Gli eventuali Chr(13) li sostituisco con l'equivalente JS
        'attenzione! va messo dopo la sostituzione di \ con \\ !!!
        Testo = Replace(Testo, Chr(13), "\r")

        '----- Faccio apparire un msgbox aggiungendo il controllo alla form
        ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), "alert", "alert('" + Testo + "');", True)
    End Sub

    Public Shared Sub AgroMsgBuonFine(ByVal Testo As String, _
                          ByRef objPage As System.Web.UI.Page, _
                          Optional ByVal NomeForm As String = "FORM1", _
                          Optional ByVal UpdatePanel As UpdatePanel = Nothing)
        If Testo = "" Then
            Testo = "Operazione Eseguita con successo!"
        End If
        '----- Formatto il testo di ingresso in modo che non crei problemi ...

        'Elimino il carattere \ e lo sostituisco con \\
        Testo = Replace(Testo, "\", "\\")

        'Elimino il carattere (o coppia di caratteri) VBCRLF che crea problemi ...
        Testo = Replace(Testo, vbCrLf, Chr(13))

        'Elimino i doppi apici sostituendoli con due singoli apostrofi chr(96)
        Testo = Replace(Testo, Chr(34), Chr(96) & Chr(96))

        'Elimino i singoli apici sostituendoli con un apostrofo chr(96)
        Testo = Replace(Testo, Chr(39), Chr(96))

        'Gli eventuali Chr(13) li sostituisco con l'equivalente JS
        'attenzione! va messo dopo la sostituzione di \ con \\ !!!
        Testo = Replace(Testo, Chr(13), "\r")

        If UpdatePanel Is Nothing Then

            If Not objPage.FindControl(NomeForm) Is Nothing Then

                objPage.FindControl(NomeForm).Controls.Add( _
                    New LiteralControl( _
                        "<script language='javascript'>$(document).ready(function () {ScritturaOK('" & Testo & "'); });</script>"))

            Else

                If Not objPage.Master.FindControl(NomeForm) Is Nothing Then

                    objPage.Master.FindControl(NomeForm).Controls.Add( _
                        New LiteralControl( _
                            "<script language='javascript'>$(document).ready(function () {ScritturaOK('" & Testo & "'); });</script>"))

                Else

                    If Not objPage.Master.Master.FindControl(NomeForm) Is Nothing Then

                        objPage.Master.Master.FindControl(NomeForm).Controls.Add( _
                            New LiteralControl( _
                                "<script language='javascript'>$(document).ready(function () {ScritturaOK('" & Testo & "'); });</script>"))
                    Else
                        If Not Ricerca.FindControlIterative2(objPage, NomeForm) Is Nothing Then

                            Ricerca.FindControlIterative2(objPage, NomeForm).Controls.Add( _
                                New LiteralControl( _
                                    "<script language='javascript'>$(document).ready(function () {ScritturaOK('" & Testo & "'); });</script>"))
                        End If
                    End If
                End If
            End If
        Else
            ScriptManager.RegisterClientScriptBlock(UpdatePanel, UpdatePanel.GetType, "alert", "$(document).ready(function () {ScritturaOK('" & Testo & "'); });", True)
        End If

    End Sub


    Public Shared Sub AgroEliminaSiNo(ByVal CHIAVE As String, _
                          ByRef objPage As System.Web.UI.Page, _
                          Optional ByVal NomeForm As String = "FORM1", _
                          Optional ByVal UpdatePanel As UpdatePanel = Nothing)

        '----- Formatto il testo di ingresso in modo che non crei problemi ...

        'Elimino il carattere \ e lo sostituisco con \\
        CHIAVE = Replace(CHIAVE, "\", "\\")

        'Elimino il carattere (o coppia di caratteri) VBCRLF che crea problemi ...
        CHIAVE = Replace(CHIAVE, vbCrLf, Chr(13))

        'Elimino i doppi apici sostituendoli con due singoli apostrofi chr(96)
        CHIAVE = Replace(CHIAVE, Chr(34), Chr(96) & Chr(96))

        'Elimino i singoli apici sostituendoli con un apostrofo chr(96)
        CHIAVE = Replace(CHIAVE, Chr(39), Chr(96))

        'Gli eventuali Chr(13) li sostituisco con l'equivalente JS
        'attenzione! va messo dopo la sostituzione di \ con \\ !!!
        CHIAVE = Replace(CHIAVE, Chr(13), "\r")

        If UpdatePanel Is Nothing Then

            If Not objPage.FindControl(NomeForm) Is Nothing Then

                objPage.FindControl(NomeForm).Controls.Add( _
                    New LiteralControl( _
                        "<script language='javascript'>$(document).ready(function () {ConfermaEliminaControlli('" & CHIAVE & "'); });</script>"))

            Else

                If Not objPage.Master.FindControl(NomeForm) Is Nothing Then

                    objPage.Master.FindControl(NomeForm).Controls.Add( _
                        New LiteralControl( _
                            "<script language='javascript'>$(document).ready(function () {ConfermaEliminaControlli('" & CHIAVE & "'); });</script>"))

                Else

                    If Not objPage.Master.Master.FindControl(NomeForm) Is Nothing Then

                        objPage.Master.Master.FindControl(NomeForm).Controls.Add( _
                            New LiteralControl( _
                                "<script language='javascript'>$(document).ready(function () {ConfermaEliminaControlli('" & CHIAVE & "'); });</script>"))
                    Else
                        If Not Ricerca.FindControlIterative2(objPage, NomeForm) Is Nothing Then

                            Ricerca.FindControlIterative2(objPage, NomeForm).Controls.Add( _
                                New LiteralControl( _
                                    "<script language='javascript'>$(document).ready(function () {ConfermaEliminaControlli('" & CHIAVE & "'); });</script>"))
                        End If
                    End If
                End If
            End If
        Else
            ScriptManager.RegisterClientScriptBlock(UpdatePanel, UpdatePanel.GetType, "alert", "$(document).ready(function () {ConfermaEliminaControlli('" & CHIAVE & "'); });", True)
        End If

    End Sub

    Public Shared Sub AgroEliminaSiNoBis(ByVal CHIAVE As String, _
                      ByRef objPage As System.Web.UI.Page, _
                      Optional ByVal NomeForm As String = "FORM1", _
                      Optional ByVal UpdatePanel As UpdatePanel = Nothing, Optional ByVal Messaggio As String = "")

        '----- Formatto il testo di ingresso in modo che non crei problemi ...

        'Elimino il carattere \ e lo sostituisco con \\
        CHIAVE = Replace(CHIAVE, "\", "\\")

        'Elimino il carattere (o coppia di caratteri) VBCRLF che crea problemi ...
        CHIAVE = Replace(CHIAVE, vbCrLf, Chr(13))

        'Elimino i doppi apici sostituendoli con due singoli apostrofi chr(96)
        CHIAVE = Replace(CHIAVE, Chr(34), Chr(96) & Chr(96))

        'Elimino i singoli apici sostituendoli con un apostrofo chr(96)
        CHIAVE = Replace(CHIAVE, Chr(39), Chr(96))

        'Gli eventuali Chr(13) li sostituisco con l'equivalente JS
        'attenzione! va messo dopo la sostituzione di \ con \\ !!!
        CHIAVE = Replace(CHIAVE, Chr(13), "\r")

        If UpdatePanel Is Nothing Then

            If Not objPage.FindControl(NomeForm) Is Nothing Then

                objPage.FindControl(NomeForm).Controls.Add( _
                    New LiteralControl( _
                        "<script language='javascript'>$(document).ready(function () {ConfermaEliminaControlliBis('" & CHIAVE & "','" & Messaggio & "'); });</script>"))

            Else

                If Not objPage.Master.FindControl(NomeForm) Is Nothing Then

                    objPage.Master.FindControl(NomeForm).Controls.Add( _
                        New LiteralControl( _
                            "<script language='javascript'>$(document).ready(function () {ConfermaEliminaControlliBis('" & CHIAVE & "','" & Messaggio & "'); });</script>"))

                Else

                    If Not objPage.Master.Master.FindControl(NomeForm) Is Nothing Then

                        objPage.Master.Master.FindControl(NomeForm).Controls.Add( _
                            New LiteralControl( _
                                "<script language='javascript'>$(document).ready(function () {ConfermaEliminaControlliBis('" & CHIAVE & "','" & Messaggio & "'); });</script>"))
                    Else
                        If Not Ricerca.FindControlIterative2(objPage, NomeForm) Is Nothing Then

                            Ricerca.FindControlIterative2(objPage, NomeForm).Controls.Add( _
                                New LiteralControl( _
                                    "<script language='javascript'>$(document).ready(function () {ConfermaEliminaControlliBis('" & CHIAVE & "','" & Messaggio & "'); });</script>"))
                        End If
                    End If
                End If
            End If
        Else
            ScriptManager.RegisterClientScriptBlock(UpdatePanel, UpdatePanel.GetType, "alert", "$(document).ready(function () {ConfermaEliminaControlliBis('" & CHIAVE & "','" & Messaggio & "'); });", True)
        End If

    End Sub



    Public Shared Sub AgroSiNo(ByVal Messaggio As String, _
                               ByVal CHIAVE As String, _
                          ByRef objPage As System.Web.UI.Page, _
                          Optional ByVal NomeForm As String = "FORM1", _
                          Optional ByVal UpdatePanel As UpdatePanel = Nothing)

        '----- Formatto il testo di ingresso in modo che non crei problemi ...

        'Elimino il carattere \ e lo sostituisco con \\
        CHIAVE = Replace(CHIAVE, "\", "\\")

        'Elimino il carattere (o coppia di caratteri) VBCRLF che crea problemi ...
        CHIAVE = Replace(CHIAVE, vbCrLf, Chr(13))

        'Elimino i doppi apici sostituendoli con due singoli apostrofi chr(96)
        CHIAVE = Replace(CHIAVE, Chr(34), Chr(96) & Chr(96))

        'Elimino i singoli apici sostituendoli con un apostrofo chr(96)
        CHIAVE = Replace(CHIAVE, Chr(39), Chr(96))

        'Gli eventuali Chr(13) li sostituisco con l'equivalente JS
        'attenzione! va messo dopo la sostituzione di \ con \\ !!!
        CHIAVE = Replace(CHIAVE, Chr(13), "\r")


        Messaggio = Replace(Messaggio, "\", "\\")
        Messaggio = Replace(Messaggio, vbCrLf, Chr(13))
        Messaggio = Replace(Messaggio, Chr(34), Chr(96) & Chr(96))
        Messaggio = Replace(Messaggio, Chr(39), Chr(96))
        Messaggio = Replace(Messaggio, Chr(13), "\r")

        If UpdatePanel Is Nothing Then

            If Not objPage.FindControl(NomeForm) Is Nothing Then

                objPage.FindControl(NomeForm).Controls.Add( _
                    New LiteralControl( _
                        "<script language='javascript'>$(document).ready(function () {ConfermaControlliSiNo('" & Messaggio & "','" & CHIAVE & "'); });</script>"))

            Else

                If Not objPage.Master.FindControl(NomeForm) Is Nothing Then

                    objPage.Master.FindControl(NomeForm).Controls.Add( _
                        New LiteralControl( _
                            "<script language='javascript'>$(document).ready(function () {ConfermaControlliSiNo('" & Messaggio & "','" & CHIAVE & "'); });</script>"))

                Else

                    If Not objPage.Master.Master.FindControl(NomeForm) Is Nothing Then

                        objPage.Master.Master.FindControl(NomeForm).Controls.Add( _
                            New LiteralControl( _
                                "<script language='javascript'>$(document).ready(function () {ConfermaControlliSiNo('" & Messaggio & "','" & CHIAVE & "'); });</script>"))
                    Else
                        If Not Ricerca.FindControlIterative2(objPage, NomeForm) Is Nothing Then

                            Ricerca.FindControlIterative2(objPage, NomeForm).Controls.Add( _
                                New LiteralControl( _
                                    "<script language='javascript'>$(document).ready(function () {ConfermaControlliSiNo('" & Messaggio & "','" & CHIAVE & "'); });</script>"))
                        End If
                    End If
                End If
            End If
        Else
            ScriptManager.RegisterClientScriptBlock(UpdatePanel, UpdatePanel.GetType, "ConfermaControlliSiNo", "$(document).ready(function () {ConfermaControlliSiNo('" & Messaggio & "','" & CHIAVE & "'); });", True)
        End If

    End Sub




    Public Shared Sub AgroSiNo_Bis(ByVal Messaggio As String, _
                               ByVal CHIAVE As String, _
                          ByRef objPage As System.Web.UI.Page, _
                          Optional ByVal NomeForm As String = "FORM1", _
                          Optional ByVal UpdatePanel As UpdatePanel = Nothing)

        '----- Formatto il testo di ingresso in modo che non crei problemi ...

        'Elimino il carattere \ e lo sostituisco con \\
        CHIAVE = Replace(CHIAVE, "\", "\\")

        'Elimino il carattere (o coppia di caratteri) VBCRLF che crea problemi ...
        CHIAVE = Replace(CHIAVE, vbCrLf, Chr(13))

        'Elimino i doppi apici sostituendoli con due singoli apostrofi chr(96)
        CHIAVE = Replace(CHIAVE, Chr(34), Chr(96) & Chr(96))

        'Elimino i singoli apici sostituendoli con un apostrofo chr(96)
        CHIAVE = Replace(CHIAVE, Chr(39), Chr(96))

        'Gli eventuali Chr(13) li sostituisco con l'equivalente JS
        'attenzione! va messo dopo la sostituzione di \ con \\ !!!
        CHIAVE = Replace(CHIAVE, Chr(13), "\r")


        Messaggio = Replace(Messaggio, "\", "\\")
        Messaggio = Replace(Messaggio, vbCrLf, Chr(13))
        Messaggio = Replace(Messaggio, Chr(34), Chr(96) & Chr(96))
        Messaggio = Replace(Messaggio, Chr(39), Chr(96))
        Messaggio = Replace(Messaggio, Chr(13), "\r")

        If UpdatePanel Is Nothing Then

            If Not objPage.FindControl(NomeForm) Is Nothing Then

                objPage.FindControl(NomeForm).Controls.Add( _
                    New LiteralControl( _
                        "<script language='javascript'>$(document).ready(function () {ConfermaControlliSiNo_Bis('" & Messaggio & "','" & CHIAVE & "'); });</script>"))

            Else

                If Not objPage.Master.FindControl(NomeForm) Is Nothing Then

                    objPage.Master.FindControl(NomeForm).Controls.Add( _
                        New LiteralControl( _
                            "<script language='javascript'>$(document).ready(function () {ConfermaControlliSiNo_Bis('" & Messaggio & "','" & CHIAVE & "'); });</script>"))

                Else

                    If Not objPage.Master.Master.FindControl(NomeForm) Is Nothing Then

                        objPage.Master.Master.FindControl(NomeForm).Controls.Add( _
                            New LiteralControl( _
                                "<script language='javascript'>$(document).ready(function () {ConfermaControlliSiNo_Bis('" & Messaggio & "','" & CHIAVE & "'); });</script>"))
                    Else
                        If Not Ricerca.FindControlIterative2(objPage, NomeForm) Is Nothing Then

                            Ricerca.FindControlIterative2(objPage, NomeForm).Controls.Add( _
                                New LiteralControl( _
                                    "<script language='javascript'>$(document).ready(function () {ConfermaControlliSiNo_Bis('" & Messaggio & "','" & CHIAVE & "'); });</script>"))
                        End If
                    End If
                End If
            End If
        Else
            ScriptManager.RegisterClientScriptBlock(UpdatePanel, UpdatePanel.GetType, "ConfermaControlliSiNo_Bis", "$(document).ready(function () {ConfermaControlliSiNo_Bis('" & Messaggio & "','" & CHIAVE & "'); });", True)
        End If

    End Sub


End Class
