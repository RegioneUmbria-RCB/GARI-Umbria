Imports System.Web.Services


Public Class xSessione
    Inherits System.Web.UI.Page

     


    <Script.Services.ScriptMethod()> _
  <WebMethod(EnableSession:=True)> _
    Public Shared Function Rinfresca() As Boolean
        HttpContext.Current.Session("RinfrescaSessione") = DateTime.Now
        Return True
    End Function
End Class