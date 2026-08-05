Imports System.Web.Services
Imports AgronicaCoreAnagrafeBIZ
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDTOStd.InData.Anagrafica
Imports AgronicaCoreModelsSTD.anagrafiche
Imports AgronicaCoreModelsSTD.metaschema
Imports AgronicaCoreUtentiDAL
Imports AgronicaCoreUtility
Imports AgronicaCoreVarieBIZ
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
' <System.Web.Script.Services.ScriptService()> _
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<System.Web.Script.Services.ScriptService()>
Public Class Indirizzi
    Inherits System.Web.Services.WebService

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiIndirizziAziendaSuperUser(InData As CoreWS_Generic(Of String)) As RispostaStandard

        Dim r As New RispostaStandard()
        Dim response As String
        Dim objIndirizzi As New Indirizzi_R
        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        Try
            Dim objParametriServer As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objImprese As New AgronicaCoreAnagrafeDAL.Imprese_Read
            Dim objIndirizzo As New AgronicaCoreModelsSTD.anagrafiche.Indirizzo
            Dim objInd As New AgronicaCoreAnagrafeDAL.Indirizzi_Read

            Dim dtIndirizzi As DataTable = objInd.IndirizziImpresa(
                objParametriServer.PivaSuperUser,
                AgronicaCoreDataProvider.TipiEnumerativi.enum_TipiIndirizzi.TipoIndirizzoDefaultAnagrafica,
                Cod_Indirizzo:=0, "", "", objParametriServer
                )

            'Dim jArrayListaOp As New JArray()

            Dim objCountry As Indirizzo
            Dim codindirizzo As Integer

            For Each dr As DataRow In dtIndirizzi.Rows

                codindirizzo = dr.Item("Cod_Indirizzo")

                objCountry = objIndirizzi.Leggi_Indirizzo(codindirizzo, objParametriServer)

            Next

            'ritorno la tabella trasformata in json (aggiustando anche le colonne)
            r.RispostaStringa = JsonConvert.SerializeObject(objCountry, Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiIndirizziContatto_NG(ByVal InData As CoreWS_Generic(Of LeggiIndirizziContatto)) As RispostaStandard

        Dim r As New RispostaStandard()

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        Try
            Dim objParametriServer As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objImprese As New AgronicaCoreAnagrafeDAL.Imprese_Read

            Dim objInd As New AgronicaCoreAnagrafeDAL.Indirizzi_Read
            Dim dtIndirizzi As DataTable = objInd.LeggiIndContattiImpreseCentri(InData.InData.piva, InData.InData.codContatto, InData.InData.codIndirizzo, InData.InData.tipoIndirizzo,
                                                                                "", "", objParametriServer)

            Dim jArrayListaOp As New JArray()

            For Each dr As DataRow In dtIndirizzi.Rows
                Dim indirizzo As String = CStr(dr.Item("Ind_Des")) & vbCrLf &
                                          CStr(dr.Item("Frz_Des")) & vbCrLf &
                                          CStr(dr.Item("Com_Des_Istat")) & " " & CStr(dr.Item("Pro_Sigla_Istat")) & "-" & CStr(dr.Item("Pro_Des_Istat")) & " " & dr.Item("Cap")

                jArrayListaOp.Add(New JObject(New JProperty("Cod_Indirizzo", dr.Item("Cod_Indirizzo")),
                                              New JProperty("Tipo_Indirizzo", dr.Item("Tipo_Indirizzo")),
                                              New JProperty("Tipo_Indirizzo_Des", dr.Item("IndirizzoTipo_Des")),
                                              New JProperty("Indirizzo", dr.Item("Ind_Des")),
                                              New JProperty("Localita", dr.Item("Frz_Des")),
                                              New JProperty("Comune", dr.Item("Com_Des_Istat")),
                                              New JProperty("Provincia_Sigla", dr.Item("Pro_Sigla_Istat")),
                                              New JProperty("Provincia", dr.Item("Pro_Des_Istat")),
                                              New JProperty("CAP", dr.Item("CAP")),
                                              New JProperty("Stato", dr.Item("Stato")),
                                              New JProperty("IndirizzoCompleto", indirizzo)))
            Next

            'ritorno la tabella trasformata in json (aggiustando anche le colonne)
            r.RispostaStringa = JsonConvert.SerializeObject(jArrayListaOp, Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r

    End Function
    <WebMethod()>
                <Script.Services.ScriptMethod()>
    Public Function LeggiIndirizziContatto(ByVal objP_server As String,
                                           ByVal piva As String,
                                           ByVal codContatto As String,
                                           ByVal codIndirizzo As Integer,
                                           ByVal tipoIndirizzo As Integer
                                           ) As RispostaStandard

        Dim r As New RispostaStandard()

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        Try
            Dim objParametriServer As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)

            Dim leggiLingua As New Lingue_Read
            Dim dtLingua As DataTable = leggiLingua.Leggi_datoCODGIAS(objParametriServer.Lingua_Cod, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametriServer)
            Dim linguaCodiceISO As String = dtLingua.Rows(0)("CodiceISO")
            Threading.Thread.CurrentThread.CurrentUICulture = New Globalization.CultureInfo(linguaCodiceISO)


            Dim objImprese As New AgronicaCoreAnagrafeDAL.Imprese_Read

            Dim objInd As New AgronicaCoreAnagrafeDAL.Indirizzi_Read
            Dim dtIndirizzi As DataTable = objInd.LeggiIndContattiImpreseCentri(piva, codContatto, codIndirizzo, tipoIndirizzo,
                                                                                "", "", objParametriServer)

            Dim jArrayListaOp As New JArray()

            For Each dr As DataRow In dtIndirizzi.Rows
                Dim indirizzo As String = CStr(dr.Item("Ind_Des")) & vbCrLf &
                                          CStr(dr.Item("Frz_Des")) & vbCrLf &
                                          CStr(dr.Item("Com_Des_Istat")) & " " & CStr(dr.Item("Pro_Sigla_Istat")) & "-" & CStr(dr.Item("Pro_Des_Istat")) & " " & dr.Item("Cap")

                jArrayListaOp.Add(New JObject(New JProperty("Cod_Indirizzo", dr.Item("Cod_Indirizzo")),
                                              New JProperty("Tipo_Indirizzo", dr.Item("Tipo_Indirizzo")),
                                              New JProperty("Tipo_Indirizzo_Des", dr.Item("IndirizzoTipo_Des")),
                                              New JProperty("Indirizzo", dr.Item("Ind_Des")),
                                              New JProperty("Localita", dr.Item("Frz_Des")),
                                              New JProperty("Comune", dr.Item("Com_Des_Istat")),
                                              New JProperty("Provincia_Sigla", dr.Item("Pro_Sigla_Istat")),
                                              New JProperty("Provincia", dr.Item("Pro_Des_Istat")),
                                              New JProperty("CAP", dr.Item("CAP")),
                                              New JProperty("Stato", dr.Item("Stato")),
                                              New JProperty("IndirizzoCompleto", indirizzo)))
            Next

            'ritorno la tabella trasformata in json (aggiustando anche le colonne)
            r.RispostaStringa = JsonConvert.SerializeObject(jArrayListaOp, Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r

    End Function

    <WebMethod()>
                        <Script.Services.ScriptMethod()>
    Public Function LeggiIndirizziContatti_X_RappContab_X_TipiInd(
        ByVal objP_server As String,
        ByVal piva As String, 
        ByVal codContatto As String,
        ByVal cliente As Boolean, 
        ByVal fornitore As Boolean,
        ByVal dipendente As Boolean,
        ByVal terzista As Boolean,
        ByVal legale As Boolean,
        ByVal agente As Boolean,
        ByVal consulente As Boolean,
        ByVal conferente As Boolean,
        ByVal tipiIndirizzi As Integer()) As RispostaStandard


        Dim r As New RispostaStandard()

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        Try
            Dim objParametriServer As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
            Dim objImprese as New AgronicaCoreAnagrafeDAL.Imprese_Read

            Dim objInd As New AgronicaCoreAnagrafeDAL.Indirizzi_Read
            Dim dtIndirizzi As DataTable = objInd.LeggiContatti_X_RapportiContab_X_TipiInd(
                objParametriServer,
                piva,
                codContatto,
                cliente,
                fornitore,
                dipendente,
                terzista,
                legale,
                agente,
                consulente,
                conferente,
                tipiIndirizzi)
            
            'ritorno la tabella trasformata in json (aggiustando anche le colonne)
            r.RispostaStringa = JsonConvert.SerializeObject(dtIndirizzi, Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r

    End Function

End Class