Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.ComponentModel
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreDataProvider
Imports Newtonsoft.Json.Linq
Imports Newtonsoft.Json
Imports AgronicaCoreDTOStd.InData.Metaschema

' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
<System.Web.Script.Services.ScriptService()>
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<ToolboxItem(False)>
Public Class Copertura
    Inherits System.Web.Services.WebService

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function CaricaComboCopertura_NG(ByVal InData As CoreWS_Generic(Of CaricaComboCopertura)) As RispostaStandard

        Dim r As New RispostaStandard

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If


        Try

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)

            Dim objSpecie As New AgronicaCoreMetaSchemaDAL.SpecieVegetali_R
            Dim DTVeg As DataTable = objSpecie.Leggi(InData.InData.Veg_Cod, 0, "", "", 1, "", "", objParametri_Server)
            Dim Gru_Cod As Integer = 0
            If DTVeg IsNot Nothing AndAlso DTVeg.Rows.Count > 0 Then
                Gru_Cod = CInt(DTVeg.Rows(0).Item("Gru_Cod"))
            End If


            Dim objCom As New AgronicaCoreMetaSchemaDAL.Copertura_R
            Dim Dt As DataTable = objCom.Leggi(Gru_Cod, 0, "", AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_Server)

            Dim JArrayLista As New JArray()
            For Each dr In Dt.Rows
                JArrayLista.Add(New JObject(New JProperty("Cop_Cod", dr.Item("Cop_Cod")), New JProperty("Cop_Des", dr.Item("Cop_Des"))))
            Next

            r.RispostaStringa = JsonConvert.SerializeObject(JArrayLista, Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function CaricaComboCopertura(objP_server As String,
                                        Veg_Cod As Integer,
                                        StringaCerca As String,
                                        FiltroAggiuntivo As String, Ordinamento As String) As RispostaStandard

        Dim r As New RispostaStandard

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If


        Try

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)

            Dim objSpecie As New AgronicaCoreMetaSchemaDAL.SpecieVegetali_R
            Dim DTVeg As DataTable = objSpecie.Leggi(Veg_Cod, 0, "", "", 1, "", "", objParametri_Server)
            Dim Gru_Cod As Integer = 0
            If DTVeg IsNot Nothing AndAlso DTVeg.Rows.Count > 0 Then
                Gru_Cod = CInt(DTVeg.Rows(0).Item("Gru_Cod"))
            End If


            Dim objCom As New AgronicaCoreMetaSchemaDAL.Copertura_R
            Dim Dt As DataTable = objCom.Leggi(Gru_Cod, 0, "", AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_Server)

            Dim JArrayLista As New JArray()
            For Each dr In Dt.Rows
                JArrayLista.Add(New JObject(New JProperty("Cop_Cod", dr.Item("Cop_Cod")), New JProperty("Cop_Des", dr.Item("Cop_Des"))))
            Next

            r.RispostaStringa = JsonConvert.SerializeObject(JArrayLista, Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function CaricaComboCopertura_Modello(InData As CoreWS_Generic(Of AgronicaCoreDTOStd.InData.Metaschema.LeggiFormaAllevamento)) As rispostaStandard(Of List(Of AgronicaCoreModelsSTD.metaschema.Copertura))

        Dim r As New rispostaStandard(Of List(Of AgronicaCoreModelsSTD.metaschema.Copertura))

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If


        Try

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)

            Dim cache As Cache
            If HttpContext.Current.Cache IsNot Nothing Then
                cache = HttpContext.Current.Cache
            End If



            Dim objSpecie As New AgronicaCoreMetaSchemaDAL.SpecieVegetali_R
            Dim DTVeg As DataTable = objSpecie.Leggi(InData.InData.specie.codice, 0, "", "", 1, "", "", objParametri_Server)
            Dim Gru_Cod As Integer = 0
            If DTVeg IsNot Nothing AndAlso DTVeg.Rows.Count > 0 Then
                Gru_Cod = CInt(DTVeg.Rows(0).Item("Gru_Cod"))
            End If

            Dim key = "CaricaComboCopertura_Modello_" & Gru_Cod
            If cache IsNot Nothing And cache.Item(key) IsNot Nothing Then
                r.RispostaStringa = cache.Item(key)
                r.RispostaOK = True
                Return r
            End If

            Dim objCom As New AgronicaCoreMetaSchemaDAL.Copertura_R
            Dim Dt As DataTable = objCom.Leggi(Gru_Cod, 0, "", AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_Server)

            Dim listItems = (From dr In Dt.Rows
                             Select New AgronicaCoreModelsSTD.metaschema.Copertura(dr.Item("Cop_Cod")) With {
                                .descrizione = dr.Item("Cop_Des")
                             }).ToList

            cache.Insert(key, listItems)

            r.RispostaStringa = listItems
            r.RispostaOK = True

        Catch ex As Exception
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function


End Class