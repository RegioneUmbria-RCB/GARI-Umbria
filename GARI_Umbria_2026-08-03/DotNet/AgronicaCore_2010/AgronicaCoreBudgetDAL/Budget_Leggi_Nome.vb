
Imports System.ComponentModel
Imports System.Web.Services
Imports System.Web.Services.Protocols

' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
' <System.Web.Script.Services.ScriptService()> _
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<ToolboxItem(False)>
Public Class BudgetLeggiNome
    Inherits AgronicaCoreDataProvider.DataProvider

    <WebMethod()>
    Public Function HelloWorld() As String
        Return "Hello World"
    End Function

    Public Function Budget_Leggi_Nome(budgetID As Integer, objParametri_server As AgronicaCoreDataProvider.AgronicaCoreParametri
                                     ) As DataTable



        Dim NomeRoutine As String = "AgronicaCoreBudgetWS.Leggi_Budget_Nome()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            StrSQL.Length = 0

            StrSQL.Append(" SELECT Nome_Budget FROM Budget_Testata")
            StrSQL.Append(" WHERE Id_Budget = " & Agro_SQL_SaveNum(budgetID) & "")

            'StrSQL.Append(" WHERE   (Budget_Appezzamento.Id_Budget = " & Id_Budget & ")  ")

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri_server, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_server, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return DT

    End Function

End Class