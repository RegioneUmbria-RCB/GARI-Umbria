Imports System.Web.Services
Imports System.ComponentModel
Imports AgronicaCoreDataProvider
Imports AgronicaCoreUtility
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports Newtonsoft.Json

' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
<System.Web.Script.Services.ScriptService()>
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<ToolboxItem(False)>
Public Class Contabilita
    Inherits System.Web.Services.WebService

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LastNumDocumento(ByVal objP_server As String,
                                     ByVal piva As String,
                                     ByVal lavCod As Integer,
                                     ByVal anno As Integer,
                                     ByVal docNumeroSin As String,
                                     ByVal docNumeroDes As String,
                                     ByVal codRisUm As Integer,
                                     ByVal cauMov As String
                                     ) As RispostaStandard

        Dim r As New RispostaStandard
        Dim lastNumDoc As Integer = 0

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        Try

            Dim objParametriServer As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)

            Dim obj As New AgronicaCoreContabDAL.Contabilita_R
            lastNumDoc = obj.LastNumDocumento(piva, lavCod, anno, docNumeroSin, docNumeroDes, codRisUm, cauMov, "", objParametriServer)

            r.RispostaStringa = lastNumDoc
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function NuovoProgressivoUpdate(ByVal objP_server As String,
                                           ByVal piva As String,
                                           ByVal anno As Integer,
                                           ByVal tipoProgressivo As Integer,
                                           ByVal docNumeroSin As String,
                                           ByVal docNumeroDes As String,
                                           ByVal sezionaleCod As Integer
                                           ) As RispostaStandard

        Dim r As New RispostaStandard
        Dim nuovoProgressivo As Integer = 0

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        Try

            Dim objParametriServer As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)

            Dim obj As New AgronicaCoreDataProvider.Sequenza_Progressivi_R
            nuovoProgressivo = obj.Nuovo_Progressivo_UpdateImmediato(piva, anno, tipoProgressivo, docNumeroSin, docNumeroDes, sezionaleCod, objParametriServer)

            r.RispostaStringa = nuovoProgressivo
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function NuovoProgressivoValBase(ByVal objP_server As String,
                                            ByVal piva As String,
                                            ByVal anno As Integer,
                                            ByVal tipoProgressivo As Integer,
                                            ByVal docNumeroSin As String,
                                            ByVal docNumeroDes As String,
                                            ByVal sezionaleCod As Integer,
                                            ByVal valoreInizialeDefault As Integer
                                            ) As RispostaStandard

        Dim r As New RispostaStandard
        Dim nuovoProgressivo As Integer = 0

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        Try

            Dim objParametriServer As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)

            Dim obj As New AgronicaCoreDataProvider.Sequenza_Progressivi_R
            nuovoProgressivo = obj.Nuovo_ProgressivoValBase(piva, anno, tipoProgressivo, docNumeroSin, docNumeroDes,
                                                            sezionaleCod, valoreInizialeDefault, objParametriServer)

            r.RispostaStringa = nuovoProgressivo
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiRapportiContabili(ByVal objP_server As String, 
                                           ByVal cliente As Boolean, 
                                           ByVal fornitore As Boolean,
                                           ByVal dipendente As Boolean,
                                           ByVal terzista As Boolean,
                                           ByVal legale As Boolean,
                                           ByVal agente As Boolean,
                                           ByVal consulente As Boolean) As RispostaStandard

        Dim r As New RispostaStandard

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        Dim objParametriServer As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)

        Try
            Dim ObjRapp As New AgronicaCoreAnagrafeDAL.Rapporti_Contabili_R
            Dim rapportiContabili = ObjRapp.Contatti_RapportiContabili_Leggi(SACOD_CONTATTO_NONDEFINITO,
                                                      0,
                                                      cliente, fornitore, dipendente, terzista, legale, agente, consulente,
                                                       AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                       "",
                                                       "",
                                                       objParametriServer)

            Dim lista = (From d In rapportiContabili.AsEnumerable() Select New With
                                                                     {
                                                                        .Cod_Rapporto = d.Item("Cod_Rapporto").ToString(),
                                                                        .Rapporto_Des = d.Item("Rapporto_Des").ToString(),
                                                                        .Sa_Cod = CInt(d.Item("Sa_Cod"))
                                                                     }).ToList()

            r.RispostaOK = True
            r.RispostaStringa = JsonConvert.SerializeObject(lista, Formatting.None)

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r

    End Function

End Class