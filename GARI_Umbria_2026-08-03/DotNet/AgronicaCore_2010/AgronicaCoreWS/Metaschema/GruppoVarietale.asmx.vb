Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.ComponentModel
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreDataProvider
Imports Newtonsoft.Json.Linq
Imports Newtonsoft.Json
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaControlliGIS
Imports AgronicaCoreDTOStd.InData.Metaschema

' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
<System.Web.Script.Services.ScriptService()>
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<ToolboxItem(False)> _
Public Class GruppoVarietale
    Inherits System.Web.Services.WebService

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function CaricaComboGruppoVarietale_NG(ByVal InData As CoreWS_Generic(Of CaricaComboGruppoVarietale)) As RispostaStandard

        Dim r As New RispostaStandard

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If


        Try

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)

            Dim Dt As DataTable
            Dim objGrva As New AgronicaCoreMetaSchemaDAL.GruppoVarietale_R

            Dt = objGrva.Leggi(InData.InData.Veg_Cod,
                           0, "",
                           enumSelezioneVariabile.Selezione_JoinCompleta,
                           "", "",
                           objParametri_Server)

            Dim JArrayLista As New JArray()
            For Each dr In Dt.Rows
                JArrayLista.Add(New JObject(New JProperty("Grva_Cod", dr.Item("Grva_Cod")), New JProperty("Grva_Des", dr.Item("Grva_Des"))))
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
    Public Function CaricaComboGruppoVarietale(objP_server As String,
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

            Dim Dt As DataTable
            Dim objGrva As New AgronicaCoreMetaSchemaDAL.GruppoVarietale_R

            Dt = objGrva.Leggi(Veg_Cod,
                           0, "",
                           enumSelezioneVariabile.Selezione_JoinCompleta,
                           "", "",
                           objParametri_Server)

            Dim JArrayLista As New JArray()
            For Each dr In Dt.Rows
                JArrayLista.Add(New JObject(New JProperty("Grva_Cod", dr.Item("Grva_Cod")), New JProperty("Grva_Des", dr.Item("Grva_Des"))))
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
    Public Function Leggi(InData As CoreWS_Generic(Of AgronicaCoreDTOStd.InData.Metaschema.LeggiGruppoVarietale)) As rispostaStandard(Of List(Of AgronicaCoreModelsSTD.metaschema.utilizzi.GruppoVarietale))

        Dim r As New rispostaStandard(Of List(Of AgronicaCoreModelsSTD.metaschema.utilizzi.GruppoVarietale))

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

            Dim key = "GruppoVarietaleModello" & InData.InData.specie.codice
            If InData.InData.ParametriSementieri IsNot Nothing Then
                key &= InData.InData.ParametriSementieri.Sementi & InData.InData.ParametriSementieri.SementiMappaturaLibera
            End If
            Dim listGruppoVarietale As List(Of AgronicaCoreModelsSTD.metaschema.utilizzi.GruppoVarietale)

            If cache IsNot Nothing AndAlso cache.Item(key) IsNot Nothing Then
                r.RispostaStringa = cache.Item(key)
                r.RispostaOK = True
                Return r
            End If

            Dim Dt As DataTable
            Dim objGrva As New AgronicaCoreMetaSchemaDAL.GruppoVarietale_R

            Dim xFiltroAggiuntivo As String = ""
            If InData.InData.ParametriSementieri IsNot Nothing AndAlso InData.InData.ParametriSementieri.Sementi <> "" Then

                Dim gp As New GisPurpose(InData.InData.ParametriSementieri.Sementi, If(InData.InData.ParametriSementieri.SementiMappaturaLibera = "True", "1", ""))
                Dim mode = gp.Mode()

                If mode = GisPurpose.enum_GisPurpose.SementiSportello Then

                    xFiltroAggiuntivo = "GruppoVarietale.Grva_Cod IN (SELECT Grva_Cod FROM Mappatura_Specie WHERE Veg_Cod = " & InData.InData.specie.codice.ToString & ")"

                End If
            End If

            Dt = objGrva.Leggi(InData.InData.specie.codice,
                           0, "",
                           enumSelezioneVariabile.Selezione_JoinCompleta,
                           xFiltroAggiuntivo, "",
                           objParametri_Server)

            Dim listGruppoVarietalePositive = (From dr In Dt.Rows
                                               Select New AgronicaCoreModelsSTD.metaschema.utilizzi.GruppoVarietale(dr("Grva_Cod"), dr("Grva_Des"))).ToList

            listGruppoVarietale = New List(Of AgronicaCoreModelsSTD.metaschema.utilizzi.GruppoVarietale)

            For Each elem In listGruppoVarietalePositive

                listGruppoVarietale.Add(elem)

                If elem.codice > 0 Then
                    Dim newElem = New AgronicaCoreModelsSTD.metaschema.utilizzi.GruppoVarietale(-elem.codice, elem.descrizione + " - Ibrido")
                    listGruppoVarietale.Add(newElem)
                End If

            Next

            cache.Insert(key, listGruppoVarietale)

            r.RispostaStringa = listGruppoVarietale
            r.RispostaOK = True

        Catch ex As Exception
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

End Class