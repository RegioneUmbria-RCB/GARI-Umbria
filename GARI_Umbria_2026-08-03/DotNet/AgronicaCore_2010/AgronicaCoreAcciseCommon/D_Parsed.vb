Public Class D_Parsed
    Public Riga As String
    Public Campo As String
    Public Ripetizione As String
    Public CodiceErrore As String
    Public TipoErrore As String = "F"

    Public Function NonErrore() As Boolean
        Return If(Riga = "0" AndAlso Campo = "000. 0" AndAlso CodiceErrore = "0", True, False)
    End Function

End Class
