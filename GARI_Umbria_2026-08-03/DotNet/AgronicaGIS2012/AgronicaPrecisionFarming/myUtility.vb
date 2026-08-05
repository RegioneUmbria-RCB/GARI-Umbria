Imports AgronicaGIS2012.Commons

Public Class MyUtility
    Private Const _SEPARATORE_CODICE_DESCRIZIONE As String = " - "

    Public Shared Function CodiceDaElemento(ByVal pElemento As String) As String
        Dim RetVal As String
        If pElemento <> String.Empty Then
            Dim index As String
            index = pElemento.IndexOf(_SEPARATORE_CODICE_DESCRIZIONE)
            If index <> -1 Then
                RetVal = pElemento.Substring(0, index)
            End If
        Else
            RetVal = pElemento
        End If

        Return RetVal
    End Function

    Public Shared Function DescrizioneDaElemento(ByVal pElemento As String) As String
        Dim RetVal As String
        If pElemento <> String.Empty Then
            Dim index As String
            index = pElemento.IndexOf(_SEPARATORE_CODICE_DESCRIZIONE)
            If index <> -1 Then
                RetVal = pElemento.Substring(index + _SEPARATORE_CODICE_DESCRIZIONE.Length)
            End If
        Else
            RetVal = pElemento
        End If

        Return RetVal
    End Function

    Public Shared Function IntToStr(ByRef pIntValue As Object) As String
        Dim RetVal As String

        If Not pIntValue Is Nothing Then
            RetVal = CStr(pIntValue)
        End If

        Return RetVal
    End Function

    Public Shared Function SingleToStr(ByRef pSingleValue As Object) As String
        Dim RetVal As String

        If Not pSingleValue Is Nothing Then
            RetVal = CStr(pSingleValue)
        End If

        Return RetVal
    End Function

    Public Shared Function BoolToStr(ByRef pBoolValue As Object) As String
        Dim RetVal As String
        Dim BFalse As Boolean = False

        If Not pBoolValue Is Nothing Then
            If CBool(pBoolValue) Then
                RetVal = "1"
            Else
                RetVal = "0"
            End If
        Else
            RetVal = "0"
        End If

        Return RetVal
    End Function


    Public Shared Function RotatePolygon() As String


    End Function



    'Public Shared Function EccezioneToAzione(ByVal pMsg As String, ByRef pEccezione As Exception) As Istanze.Azione
    '    Dim Msg As String
    '    Dim Azione As Istanze.Azione

    '    Msg = pMsg
    '    While Not pEccezione Is Nothing
    '        Msg &= "<BR>" & pEccezione.Message
    '        pEccezione = pEccezione.InnerException
    '    End While

    '    Azione = New Istanze.Azione(Istanze.Azione.Tipi.Warning, Msg)

    '    Return Azione
    'End Function
End Class