Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.ComponentModel
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports Newtonsoft.Json.Linq
Imports Newtonsoft.Json
Imports AgronicaCoreUtility
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreDataProvider

' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
' <System.Web.Script.Services.ScriptService()> _
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<System.Web.Script.Services.ScriptService()>
Public Class Listini_Prezzi
    Inherits System.Web.Services.WebService

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Leggi(ByVal piva As String,
                            ByVal Listino_Cod As Integer,
                            ByVal Listino_Classe_Cod As Integer,
                            ByVal Tipo_Classe As Integer,
                            ByVal ChkApplicabilita As Integer,
                            ByVal TipoIva As Integer,
                            ByVal Tipo_Provvigione As Integer,
                            ByVal x_OrderBy As String,
                            ByVal objP_server As String) As RispostaStandard

        Dim r As New RispostaStandard()

        Try

            Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)

            If objP_server = "" Then
                r.Errore = "objP_server non valorizzato"
                Return r
            End If

            Dim dal As New AgronicaCoreContabDAL.Listini_Prezzi_R

            Dim Dt As DataTable = dal.Leggi(piva, Listino_Cod, Listino_Classe_Cod, Tipo_Classe, ChkApplicabilita, TipoIva, Tipo_Provvigione, "", x_OrderBy, objParametri_Server)

            Dim JArrayListaOp As New JArray()

            For Each dr As DataRow In Dt.Rows
                JArrayListaOp.Add(
                    New JObject(
                        New JProperty("Listino_Cod", dr("Listino_Cod")),
                        New JProperty("Listino_Des", dr("Listino_Des")),
                        New JProperty("Listino_Classe_Cod", dr("Listino_Classe_Cod")),
                        New JProperty("Listino_Classe_Des", dr("Listino_Classe_Des")),
                        New JProperty("Tipo_Classe", dr("Tipo_Classe"))
                        ))
            Next

            'ritorno la tabella trasformata in json (aggiustando anche le colonne)
            Dim js As New AgronicaCoreDataProvider.JSON_DataTable
            r.RispostaStringa = JsonConvert.SerializeObject(JArrayListaOp, Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.Errore = ex.Message
            Return r
        End Try

        Return r

    End Function

    <WebMethod()>
    Public Function LeggiListiniPrezziValidi(ByVal objP_server As String,
                                             ByVal objP_utenti As String,
                                             ByVal piva As String,
                                             ByVal lav_cod As Integer,
                                             ByVal tipo_classe As Integer,
                                             ByVal elem_cod As Integer,
                                             ByVal pro_cod As Integer,
                                             ByVal mat_cod As Integer,
                                             ByVal cal_cod As Integer,
                                             ByVal qualita_cod As Integer,
                                             ByVal lotto_cod1 As Integer,
                                             ByVal lotto_val1 As String,
                                             ByVal lotto_cod2 As Integer,
                                             ByVal lotto_val2 As String,
                                             ByVal chkvettore As Integer,
                                             ByVal cod_rapporto As Integer,
                                             ByVal cod_risum As Integer,
                                             ByVal chksemina As Integer,
                                             ByVal livello_prezzo As Integer,
                                             ByVal udm_cod As Integer,
                                             ByVal udm_cod_extra As Integer,
                                             ByVal data As String,
                                             ByVal chkcontatto As Boolean,
                                             ByVal listino_cod_default As Integer
                                             ) As RispostaStandard

        Dim r As New RispostaStandard
        Dim dt As New DataTable
        Dim bOk As Boolean = False

        Try

            Dim objParametriServer As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
            Dim objParametriUtenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)

            Dim objListini As New AgronicaCoreContabDAL.Listini_Prezzi_Dettagli_R
            bOk = objListini.GetListiniConSpiegazione(dt,
                                                      data, lav_cod, piva, tipo_classe, elem_cod, pro_cod, mat_cod, cal_cod, qualita_cod,
                                                      lotto_cod1, lotto_val1, lotto_cod2, lotto_val2, chkvettore, cod_rapporto, cod_risum,
                                                      chksemina, livello_prezzo, udm_cod, udm_cod_extra, objParametriServer,
                                                      listino_cod_default, chkcontatto,
                                                      rimuoviCampiInutili:=True)

            If bOk = True Then

                Dim serializerSettings As New JsonSerializerSettings With { .ReferenceLoopHandling = ReferenceLoopHandling.Ignore }
                r.RispostaStringa = JsonConvert.SerializeObject(dt, Formatting.None, serializerSettings)
                r.RispostaOK = True

            Else
                r.RispostaStringa = "Nessun Listino Valido."
                r.RispostaOK = False
            End If

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

End Class