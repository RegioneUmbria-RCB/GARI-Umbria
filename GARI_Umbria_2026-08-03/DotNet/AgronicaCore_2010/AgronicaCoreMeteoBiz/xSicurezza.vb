
Imports System.Xml


Public Class xSicurezza



    '####################################################################################
    Public Shared Function Decodifica( _
                                ByVal Testo As String, _
                                ByVal Chiave As String, _
                                ByRef ErrCod As Integer, _
                                ByRef ErrMsg As String) _
                                As String

        Dim strEncrypted As String = ""
        Dim i As Integer
        Dim A1 As Integer
        Dim A2 As Integer
        Dim KeyPos As Byte

        Dim Cript As Boolean = False

        Try '--------------------------------------------------------------------

            KeyPos = 1

            For i = 1 To Len(Testo)
                A1 = Asc(Mid(Testo, i, 1))
                A2 = Asc(Mid(Chiave, KeyPos, 1))
                If Cript Then
                    strEncrypted = strEncrypted & Chr(A2 + A1)
                Else
                    strEncrypted = strEncrypted & Chr(A1 - A2)
                End If
                KeyPos = KeyPos + 1
                If KeyPos > Len(Chiave) Then KeyPos = 1
            Next

            strEncrypted = strEncrypted.Replace("{", "<")
            strEncrypted = strEncrypted.Replace("}", ">")

        Catch ex As Exception '--------------------------------------------------

            ErrCod = 452644
            ErrMsg = "[Decodifica]:" & ex.Message
            strEncrypted = ""

        End Try '----------------------------------------------------------------

        Return strEncrypted

    End Function




    '####################################################################################
    Public Shared Function Codifica_P( _
                                ByVal Testo As String, _
                                ByVal Chiave As String, _
                                ByRef ErrCod As Integer, _
                                ByRef ErrMsg As String) _
                                As String

        Dim strEncrypted As String = ""
        Dim i As Integer
        Dim A1 As Integer
        Dim A2 As Integer
        Dim KeyPos As Byte

        Dim Cript As Boolean = True


        Try '--------------------------------------------------------------------

            Testo = Testo.Replace("<", "{")
            Testo = Testo.Replace(">", "}")
            KeyPos = 1

            For i = 1 To Len(Testo)
                A1 = Asc(Mid(Testo, i, 1))
                A2 = Asc(Mid(Chiave, KeyPos, 1))
                If Cript Then
                    strEncrypted = strEncrypted & Chr(A2 + A1)
                Else
                    strEncrypted = strEncrypted & Chr(A1 - A2)
                End If
                KeyPos = KeyPos + 1
                If KeyPos > Len(Chiave) Then KeyPos = 1
            Next

        Catch ex As Exception '--------------------------------------------------

            ErrCod = 452645
            ErrMsg = "[Codifica]:" & ex.Message
            strEncrypted = ""

        End Try '----------------------------------------------------------------

        Return strEncrypted

    End Function




    '####################################################################################
    Public Shared Sub VerificaCredenziali( _
                                    ByVal XmlParametri As String, _
                                    ByVal DoorKey As String, _
                                    ByRef ErrCod As Integer, _
                                    ByRef ErrMsg As String, _
                                    ByRef StrConnUtenti As String)

        Dim Drk As String = ""
        Dim Usr As String = ""
        Dim Pwd As String = ""
        Dim Tkn As String = ""
        Dim Testo As String

        ErrCod = 0
        ErrMsg = ""

        '-----------------------------------------------------------------------
        '--- Recupero i parametri
        '-----------------------------------------------------------------------

        Dim XmlDoc As New XmlDocument
        Dim XmlNodo As XmlElement

        XmlDoc.LoadXml(XmlParametri)

        XmlNodo = XmlDoc.SelectSingleNode("PARAMETRI")

        '--- Doorkey (doorkey)

        If XmlNodo.HasAttribute("doorkey") = True Then
            Testo = CStr(XmlNodo.GetAttribute("doorkey"))
            If Testo <> "" Then
                Drk = Testo
            Else
                ErrCod = 211977
                ErrMsg = "Errore nei parametri"
                Exit Sub
            End If
        Else
            ErrCod = 223977
            ErrMsg = "Errore nei parametri"
            Exit Sub
        End If

        '--- Username (usr)

        If XmlNodo.HasAttribute("usr") = True Then
            Testo = CStr(XmlNodo.GetAttribute("usr"))
            If Testo <> "" Then
                '
                '   NOTA : deve essere decodificato
                '
                Usr = Testo
                '
                '
                '
            Else
                ErrCod = 236324
                ErrMsg = "Errore nei parametri"
                Exit Sub
            End If
        Else
            ErrCod = 236574
            ErrMsg = "Errore nei parametri"
            Exit Sub
        End If

        '--- Password (pwd)

        If XmlNodo.HasAttribute("pwd") = True Then
            Testo = CStr(XmlNodo.GetAttribute("pwd"))
            If Testo <> "" Then
                '
                '   NOTA : deve essere decodificato
                '
                Pwd = Testo
                '
                '
                '
            Else
                ErrCod = 276324
                ErrMsg = "Errore nei parametri"
                Exit Sub
            End If
        Else
            ErrCod = 238894
            ErrMsg = "Errore nei parametri"
            Exit Sub
        End If

        '--- Security Token (tkn)

        If XmlNodo.HasAttribute("tkn") = True Then
            Testo = CStr(XmlNodo.GetAttribute("tkn"))
            'If Testo <> "" Then
            '    Tkn = Testo
            'Else
            '    '
            '    '
            '    'Modifica temporanea ...
            '    Tkn = Testo
            '    'ErrCod = 276324
            '    'ErrMsg = "Errore nei parametri"
            '    'Exit Sub
            '    '
            '    '
            'End If
        Else
            ErrCod = 238894
            ErrMsg = "Errore nei parametri"
            Exit Sub
        End If

        XmlDoc = Nothing

        '-----------------------------------------------------------------------
        '--- Faccio le verifiche
        '-----------------------------------------------------------------------

        '--- Doorkey

        If Strings.StrComp(Drk, DoorKey, CompareMethod.Binary) <> 0 Then
            ErrCod = 876321
            ErrMsg = "Errore nei parametri"
            Exit Sub
        End If

        '--- Username & Password




        '--- Security Token


    End Sub
















End Class
