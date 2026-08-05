Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.ComponentModel

Imports AgronicaCoreContabBIZ

' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
' <System.Web.Script.Services.ScriptService()> _
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")> _
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
<ToolboxItem(False)> _
Public Class Track
    Inherits System.Web.Services.WebService

    <WebMethod()> _
    Public Function HelloWorld() As String
        Return "Hello World"
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="Credenziali"></param>
    ''' <param name="xmlTrack"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <WebMethod()> _
    Public Function TrackMe( _
                ByVal Credenziali As String, _
                ByVal xmlTrack As String) As String


        Dim rval As String


        Dim strErr As String = ""

        Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri = Nothing
        Dim objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri = Nothing

        Try

            If Credenziali = "" Then
                Throw New Exception("Credenziali non inviate!")
            End If
            If xmlTrack = "" Then
                Throw New Exception("XML Privato non inviato!")
            End If

            Dim gs As New AgronicaCoreGestioneRichieste.GestioneCredenziali

            gs.GestioneCredenziali(Credenziali, _
                                objParametri_Server, _
                                objParametri_Utenti)



            Dim trk As New AgronicaCoreContabBIZ.FF_Track
            rval = trk.TrackMe(xmlTrack, False, False, objParametri_Server)




        Catch ex As Exception

            rval = AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False, "<br />")

        End Try

        Return rval



    End Function


End Class