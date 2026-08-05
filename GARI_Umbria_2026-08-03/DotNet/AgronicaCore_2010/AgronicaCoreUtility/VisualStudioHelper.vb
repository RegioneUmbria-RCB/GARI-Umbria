Public Class VisualStudioHelper


    Public Shared sub Debug_Write_Double(byval d As double, Optional  ByVal hyp As String = "-", Optional ByVal aCapo as String = "")
        Debug.Write(d.ToString().Replace(".", ",") & hyp & aCapo )
    End sub


End Class
