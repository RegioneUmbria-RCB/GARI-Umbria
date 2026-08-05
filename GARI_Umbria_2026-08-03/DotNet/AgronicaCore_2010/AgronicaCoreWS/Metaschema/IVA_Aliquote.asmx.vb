Imports System.ComponentModel
Imports System.Web.Services
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDTOStd.InData.Metaschema
Imports AgronicaCoreUtility
Imports AgronicaCoreVarieBIZ
Imports Newtonsoft.Json

' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
<Script.Services.ScriptService()>
<Services.WebService(Namespace:="http://tempuri.org/")>
<WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<ToolboxItem(False)>
Public Class IVA_Aliquote
    Inherits System.Web.Services.WebService

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiIVA_Aliquote_NG(InData As CoreWS_Generic(Of LeggiIVA_Aliquote)) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametriServer As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
        Dim objParametriUtenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

        Try

            Dim leggi As New AgronicaCoreMetaSchemaDAL.IVA_Aliquote_R
            Dim dtIva = leggi.Leggi(InData.InData.Codice, InData.InData.Aliquota, InData.InData.Tipologia, InData.InData.Flag_Credito_Imposta_Export, InData.InData.NaturaEsclusione, "", "Sigla", objParametriServer)

            Dim serializerSettings As New JsonSerializerSettings With {
                .ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            }
            r.RispostaStringa = JsonConvert.SerializeObject(dtIva, Formatting.None, serializerSettings)

            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiIVA_Aliquote(ByVal Codice As Integer,
                                      ByVal Aliquota As Decimal,
                                      ByVal Tipologia As Integer,
                                      ByVal Flag_Credito_Imposta_Export As Integer,
                                      ByVal NaturaEsclusione As String,
                                      ByVal objP_server As String,
                                      ByVal objP_utenti As String
                                      ) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametriServer As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
        Dim objParametriUtenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)

        Try

            Dim leggi As New AgronicaCoreMetaSchemaDAL.IVA_Aliquote_R
            Dim dtIva = leggi.Leggi(Codice, Aliquota, Tipologia, Flag_Credito_Imposta_Export, NaturaEsclusione, "", "Sigla", objParametriServer)

            Dim serializerSettings As New JsonSerializerSettings With {
                .ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            }
            r.RispostaStringa = JsonConvert.SerializeObject(dtIva, Formatting.None, serializerSettings)

            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function
End Class