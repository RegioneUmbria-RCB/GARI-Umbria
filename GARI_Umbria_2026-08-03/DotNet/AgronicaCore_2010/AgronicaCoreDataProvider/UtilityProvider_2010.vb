Imports System.Web.UI

Public Class UtilityProvider_2010

    Public Shared Sub Page_NewWindow_2010(ByRef objPage As System.Web.UI.Page, _
                                ByVal NomePaginaAspx As String, _
                                ByVal QueryString As String, _
                                Optional ByVal Pagina_Titolo As String = "GiasOnline", _
                                Optional ByVal Pagina_Height As Integer = 700, _
                                Optional ByVal Pagina_Width As Integer = 1000, _
                                Optional ByVal Pagina_Top As Integer = 0, _
                                Optional ByVal Pagina_Left As Integer = 0, _
                                Optional ByVal Menubar As String = "yes", _
                                Optional ByVal Resizable As String = "yes", _
                                Optional ByVal Scrollbars As String = "yes", _
                                Optional ByVal NomeForm As String = "FORM1", _
                                Optional ByVal EsisteMaster As Boolean = False)


        Dim StrWindowOpen As String

        '----- Preparo la stringa di apertura di una nuova pagina

        StrWindowOpen = "<script language='javascript'>" & _
                        vbNewLine & _
                        "window.open('" & _
                            NomePaginaAspx & _
                            "" & QueryString & "'," & _
                            "'" & Pagina_Titolo & "'," & _
                            "'height=" & CStr(Pagina_Height) & "," & _
                            "width=" & CStr(Pagina_Width) & "," & _
                            "menubar=" + Menubar + "," & _
                            "resizable=" + Resizable + "," & _
                            "scrollbars=" + Scrollbars + "," & _
                            "top=" + CStr(Pagina_Top) + ",left=" + CStr(Pagina_Left) + "');" & _
                        vbNewLine & _
                        "</script>"

        'Apro la finestra...

        If EsisteMaster Then
            'questa riga non fa compilare l'agronicastampe 2003
            objPage.Master.FindControl(NomeForm).Controls.Add(New LiteralControl(StrWindowOpen))

        Else

            objPage.FindControl(NomeForm).Controls.Add(New LiteralControl(StrWindowOpen))
        End If



    End Sub

    '################################################################
    Public Shared Sub AgroMsgBox_2010(ByVal Testo As String, _
                                ByRef objPage As System.Web.UI.Page, _
                                Optional ByVal NomeForm As String = "FORM1", _
                                Optional ByVal EsisteMaster As Boolean = False)

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
        If EsisteMaster Then

            'questa riga non fa compilare l'agronicastampe 2003
            objPage.Master.FindControl(NomeForm).Controls.Add( _
                New LiteralControl( _
                    "<script language='javascript'>alert('" & Testo & "');</script>"))

        Else
            objPage.FindControl(NomeForm).Controls.Add( _
                New LiteralControl( _
                    "<script language='javascript'>alert('" & Testo & "');</script>"))
        End If

    End Sub


End Class

