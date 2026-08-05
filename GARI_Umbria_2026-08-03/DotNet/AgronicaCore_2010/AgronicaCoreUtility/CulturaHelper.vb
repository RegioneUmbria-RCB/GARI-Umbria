Public Class CulturaHelper

    Public Shared Function SeparatoreDecimaleVB() As String
        Return System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator
    End Function
End Class
