Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.ComponentModel

' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
' <System.Web.Script.Services.ScriptService()> _
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")> _
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
<ToolboxItem(False)> _
Public Class Utenti_Permessi_R
    Inherits System.Web.Services.WebService

    Private Utenti_Permessi_R As AgronicaCoreUtentiDAL.Utenti_Permessi_R

    <WebMethod(True)> _
    Public Function Controlla_Permessi_Utente( _
            ByVal Parametri As String _
    ) As String



        'ByVal Id_Attivita As AgronicaCoreDataProvider.TipiEnumerativi.enum_Security_Attivita, _
        'ByVal Id_Operazione As AgronicaCoreDataProvider.TipiEnumerativi.enum_Security_Operazione, _

        Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri
        Dim UserName As String
        Dim Id_Servizio As Integer
        Dim Id_Attivita As Integer
        Dim Id_Operazione As Integer
        Dim DataOraControllo As Date
        Dim xFiltroAggiuntivo As String



        Utenti_Permessi_R = New AgronicaCoreUtentiDAL.Utenti_Permessi_R()
        Return Utenti_Permessi_R.Controlla_Permessi_Utente( _
            UserName, _
            Id_Servizio, _
            Id_Attivita, _
            Id_Operazione, _
            DataOraControllo, _
            xFiltroAggiuntivo, _
            objParametri_Server _
        )



    End Function
End Class