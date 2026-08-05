Imports System.Web.Services
Imports AgronicaCoreDataProvider
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreDTOStd.InData.Metaschema
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreModelsSTD.exceptions

' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
' <System.Web.Script.Services.ScriptService()> _
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<System.Web.Script.Services.ScriptService()>
Public Class Utenti_Permessi_W
    Inherits System.Web.Services.WebService

    Private uneditable As IEnumerable(Of enum_Security_Attivita) = {
        enum_Security_Attivita.Gest_Menu,
        enum_Security_Attivita.Profilazione_NG
    }

    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function AggiornaCliente_Permessi(InData As CoreWS_Generic(Of Cliente_PermessiScrivi)) As RispostaStandard
        Dim r As New RispostaStandard

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If
        If InData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If
        If InData.objP.objP_super_server = "" Then
            r.Errore = "objP_super_server non valorizzato"
            Return r
        End If

        Try
            Dim objUtenti As New AgronicaCoreUtentiBIZ.Utenti
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            Dim username As String = InData.InData.UserName
            Dim permessi As List(Of Cliente_Permesso) = InData.InData.permessi

            ' Remove duplicate and uneditable permissions
            Dim normalizedPermissions = permessi.Select(Function(p) p.Permesso_ID).
                Distinct().
                Except(uneditable).
                Select(Function(cod) permessi.Find(Function(p) p.Permesso_ID = cod)).
                ToList

            objUtenti.AggiornaCliente_Permessi(username, normalizedPermissions, objParametri_Utenti)

            r.RispostaOK = True
            r.RispostaStringa = ""
        Catch ex As GiasException
            r.RispostaStringa = "Errore durante l'operazione: " & ex.Message
            r.Errore = "Errore durante l'operazione: " & ex.Message
            r.RispostaOK = False
        Catch ex As Exception
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.RispostaOK = False
        End Try

        Return r
    End Function
End Class

