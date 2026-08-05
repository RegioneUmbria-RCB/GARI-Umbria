Imports System.Web.Services
Imports Newtonsoft.Json
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreUtility
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreModelsSTD.profilazione
Imports AgronicaCoreDTOStd.InData.AgronicaCoreUtentiBIZ
Imports System.Net.Http
Imports AgronicaCoreModelsSTD.exceptions

' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
' <System.Web.Script.Services.ScriptService()> _
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<System.Web.Script.Services.ScriptService()>
Public Class Utenti_W
    Inherits WebService

    Private Function GetObjParams(Of T)(inData As CoreWS_Generic(Of T)) As ObjParams
        Return New ObjParams With {
            .ObjParametri_SuperServer = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(inData.objP.objP_super_server),
            .ObjParametri_Server = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(inData.objP.objP_server),
            .ObjParametri_Utenti = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(inData.objP.objP_utenti)
        }
    End Function

    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Function SalvaUtenti_NG(InData As CoreWS_Generic(Of ScriviUtenti)) As RispostaStandard
        Dim res As New RispostaStandard
        Dim objUtentiBIZ = New AgronicaCoreUtentiBIZ.Utenti
        Try
            Dim params As New ObjParams With {
                .ObjParametri_Utenti = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti),
                .ObjParametri_Server = Utility.convertStringtoOBJparametri(InData.objP.objP_server),
                .ObjParametri_SuperServer = Utility.convertStringtoOBJparametri(InData.objP.objP_super_server)
            }
            Dim warning = objUtentiBIZ.ControllaUtenti(InData.InData.Utenti, InData.InData.Operazione, params)
            objUtentiBIZ.ScriviUtenti(
                InData.InData.Utenti, InData.InData.Operazione,
                params, impostazioniDaProfilo:=InData.InData.SettingsFromProfile
            )
            If InData.InData.Operazione = enum_TipoOperazioneDB.Scrittura Then
                ' Operazione legata alla presenza di un permesso, eseguita solo
                ' in fase di scrittura in quanto in modifica diretta non è possibile
                ' cambiare il profilo (c'è chiamata apposta)
                objUtentiBIZ.InizializzaLayersGis(InData.InData.Utenti, params.ObjParametri_Server, params.ObjParametri_Utenti)
            End If

            res.RispostaOK = True
            res.Errore = warning
        Catch ex As GiasException
            res.RispostaOK = False
            res.Errore = ex.Message
        Catch ex As Exception
            res.RispostaOK = False
            res.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try
        Return res
    End Function

    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Function DisattivaUtenti_NG(InData As CoreWS_Generic(Of String())) As RispostaStandard
        Dim res As New RispostaStandard
        Dim obj_Utenti As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)
        Dim obj_Server As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(InData.objP.objP_server)

        Try
            Dim objUtentiBIZ = New AgronicaCoreUtentiBIZ.Utenti
            objUtentiBIZ.DisattivaPermessiUtenti(InData.InData, obj_Server, obj_Utenti)

            Dim gestoreCache As New AgronicaCoreVarieBIZ.GestoreCache
            Dim client As New HttpClient()

            gestoreCache.PulisciCachePermessi(client, obj_Server)
            gestoreCache.PulisciCacheImpostazioni(client, obj_Server)

            res.RispostaOK = True
        Catch ex As Exception
            res.RispostaOK = False
            res.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return res
    End Function

    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Function ImpostaLingua(InData As CoreWS_Generic(Of CambioLinguaObj)) As RispostaStandard
        Dim res As New RispostaStandard
        Dim obj_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

        Try
            Dim objUtentiBIZ = New AgronicaCoreUtentiBIZ.Utenti
            objUtentiBIZ.ImpostaLinguaUtente(InData.InData.Username,
                                             InData.InData.LinguaCod,
                                             obj_Utenti)
            res.RispostaOK = True
        Catch ex As Exception
            res.RispostaOK = False
            res.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return res
    End Function

    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Function ModificaValiditaPermessi(InData As CoreWS_Generic(Of AssociaProfiloObj)) As RispostaStandard
        Dim res As New RispostaStandard
        Dim obj_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)
        Dim obj_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)

        Try
            Dim objUtentiBIZ = New AgronicaCoreUtentiBIZ.Utenti
            objUtentiBIZ.ModificaValiditaPermessi(InData.InData.Utenti, obj_Server, obj_Utenti)

            res.RispostaOK = True
        Catch ex As Exception
            res.RispostaOK = False
            res.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return res
    End Function

    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Function ModificaFinestraTemporale(InData As CoreWS_Generic(Of UtenteFinestraTemp())) As RispostaStandard
        Dim res As New RispostaStandard
        Dim obj_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
        Dim obj_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

        Try
            Dim objUtentiBIZ = New AgronicaCoreUtentiBIZ.Utenti
            Dim utenti As IEnumerable(Of UtenteFinestraTemp) = InData.InData
            objUtentiBIZ.AggiornaFinestraTemporale(utenti, obj_Server, obj_Utenti)
            res.RispostaOK = True
        Catch ex As Exception
            res.RispostaOK = False
            res.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return res
    End Function

    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Function AssociaProfilo(InData As CoreWS_Generic(Of AssociaProfiloObj)) As RispostaStandard
        Dim res As New RispostaStandard
        Dim params As New ObjParams With {
            .ObjParametri_Utenti = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti),
            .ObjParametri_Server = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
        }
        Try
            Dim objUtentiBIZ = New AgronicaCoreUtentiBIZ.Utenti
            objUtentiBIZ.AssociaProfilo(
                InData.InData.Utenti,
                InData.InData.Profilo,
                InData.InData.AssociaImpostazioni,
                params
            )
            res.RispostaOK = True
        Catch ex As Exception
            res.RispostaOK = False
            res.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return res
    End Function

End Class

