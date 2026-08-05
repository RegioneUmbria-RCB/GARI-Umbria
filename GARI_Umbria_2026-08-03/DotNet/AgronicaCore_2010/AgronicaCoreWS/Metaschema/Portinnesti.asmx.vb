Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.ComponentModel
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreDataProvider
Imports Newtonsoft.Json.Linq
Imports Newtonsoft.Json
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDTOStd.InData.Metaschema

' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
<System.Web.Script.Services.ScriptService()>
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")> _
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
<ToolboxItem(False)> _
Public Class Portinnesti
    Inherits System.Web.Services.WebService

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function CaricaComboPortinnesti_NG(ByVal InData As CoreWS_Generic(Of CaricaComboPortinnesti)) As RispostaStandard

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

            Dim Dt As DataTable
            'Recupero il datatable
            Dim objport As New AgronicaCoreMetaSchemaDAL.PortinnestixSpecieVegetali_R

            Dt = objport.LeggiPortinnesti(InData.InData.Veg_Cod, objParametri_Server)

            Dim JArrayLista As New JArray()
            For Each dr In Dt.Rows
                JArrayLista.Add(New JObject(New JProperty("Port_Cod", dr.Item("Port_Cod")), New JProperty("Port_Des", dr.Item("Port_Des"))))
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
    Public Function CaricaComboPortinnesti(objP_server As String,
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

            Dim Dt As DataTable
            'Recupero il datatable
            Dim objport As New AgronicaCoreMetaSchemaDAL.PortinnestixSpecieVegetali_R

            Dt = objport.LeggiPortinnesti(Veg_Cod, objParametri_Server)

            Dim JArrayLista As New JArray()
            For Each dr In Dt.Rows
                JArrayLista.Add(New JObject(New JProperty("Port_Cod", dr.Item("Port_Cod")), New JProperty("Port_Des", dr.Item("Port_Des"))))
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
    Public Function CaricaComboPortinnesti_Modello(InData As CoreWS_Generic(Of AgronicaCoreDTOStd.InData.Metaschema.LeggiPortinnesto)) As rispostaStandard(Of List(Of AgronicaCoreModelsSTD.metaschema.DensitaImpianto.Portinnesto))

        Dim r As New rispostaStandard(Of List(Of AgronicaCoreModelsSTD.metaschema.DensitaImpianto.Portinnesto))

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

            Dim key = "CaricaComboPortinnesti_Modello_" & InData.InData.specie.codice
            If cache IsNot Nothing And cache.Item(key) IsNot Nothing Then
                r.RispostaStringa = cache.Item(key)
                r.RispostaOK = True
                Return r
            End If

            Dim objSpecie As New AgronicaCoreMetaSchemaDAL.SpecieVegetali_R
            Dim DTVeg As DataTable = objSpecie.Leggi(InData.InData.specie.codice, 0, "", "", 1, "", "", objParametri_Server)
            Dim Gru_Cod As Integer = 0
            If DTVeg IsNot Nothing AndAlso DTVeg.Rows.Count > 0 Then
                Gru_Cod = CInt(DTVeg.Rows(0).Item("Gru_Cod"))
            End If

            Dim Dt As DataTable
            'Recupero il datatable
            Dim objport As New AgronicaCoreMetaSchemaDAL.PortinnestixSpecieVegetali_R

            Dt = objport.LeggiPortinnesti(InData.InData.specie.codice, objParametri_Server)

            Dim listItems = (From dr In Dt.Rows
                             Select New AgronicaCoreModelsSTD.metaschema.DensitaImpianto.Portinnesto(dr.Item("Port_Cod")) With {
                                .descrizione = dr.Item("Port_Des")
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