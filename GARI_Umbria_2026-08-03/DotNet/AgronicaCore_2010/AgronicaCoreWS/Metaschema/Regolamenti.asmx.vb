Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.ComponentModel
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreDataProvider
Imports Newtonsoft.Json.Linq
Imports Newtonsoft.Json
Imports AgronicaCoreDTOStd.InData.Metaschema

' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
' <System.Web.Script.Services.ScriptService()> _
<System.Web.Script.Services.ScriptService()>
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<ToolboxItem(False)>
Public Class Regolamenti
    Inherits System.Web.Services.WebService

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function CaricaComboRegolamenti(objP_server As String) As RispostaStandard

        Dim r As New RispostaStandard

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If


        Try

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)

            Dim objReg As New AgronicaCoreMetaSchemaDAL.PUA_Regolamenti_R

            'Recupero il recordset
            Dim Dt = objReg.Leggi(0,
                          AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                          "", "",
                          objParametri_Server)

            Dim JArrayLista As New JArray()
            For Each dr In Dt.Rows
                JArrayLista.Add(New JObject(New JProperty("Reg_Cod", dr.Item("Regolamento_Cod")), New JProperty("Reg_Des", dr.Item("Regolamento_DES"))))
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
    Public Function CaricaComboRegolamenti_NG(ByVal InData As CoreWS_Generic(Of CaricaComboRegolamenti)) As RispostaStandard

        Dim r As New RispostaStandard

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If


        Try

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)

            Dim objReg As New AgronicaCoreMetaSchemaDAL.Regolamenti_R

            'Recupero il recordset
            Dim Dt = objReg.Leggi(0,
                          AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                  InData.InData.xFiltroAggiuntivo,
                                  InData.InData.xOrderBy,
                                  objParametri_Server)

            Dim JArrayLista As New JArray()
            For Each dr In Dt.Rows
                JArrayLista.Add(New JObject(New JProperty("Reg_Cod", dr.Item("Reg_cod")), New JProperty("Reg_Des", dr.Item("Reg_Des"))))
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
    Public Function CaricaComboRegolamenti(xFiltroAggiuntivo As String, xOrderBy As String, objP_server As String) As RispostaStandard

        Dim r As New RispostaStandard

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If


        Try

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)

            Dim objReg As New AgronicaCoreMetaSchemaDAL.Regolamenti_R

            'Recupero il recordset
            Dim Dt = objReg.Leggi(0,
                          AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                  xFiltroAggiuntivo,
                                  xOrderBy,
                                  objParametri_Server)

            Dim JArrayLista As New JArray()
            For Each dr In Dt.Rows
                JArrayLista.Add(New JObject(New JProperty("Reg_Cod", dr.Item("Reg_cod")), New JProperty("Reg_Des", dr.Item("Reg_Des"))))
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
    Public Function CaricaComboRegolamenti_Modello(InData As CoreWS_Generic(Of Object)) As rispostaStandard(Of List(Of AgronicaCoreModelsSTD.metaschema.Regolamenti))

        Dim r As New rispostaStandard(Of List(Of AgronicaCoreModelsSTD.metaschema.Regolamenti))

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

            Dim key = "CaricaComboRegolamenti_Modello"
            If cache IsNot Nothing And cache.Item(key) IsNot Nothing Then
                r.RispostaStringa = cache.Item(key)
                r.RispostaOK = True
                Return r
            End If

            Dim objReg As New AgronicaCoreMetaSchemaDAL.Regolamenti_R

            'Recupero il recordset
            Dim Dt = objReg.Leggi(0,
                          AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                  "",
                                  "",
                                  objParametri_Server)

            Dim JArrayLista As New JArray()
            For Each dr In Dt.Rows
                JArrayLista.Add(New JObject(New JProperty("Reg_Cod", dr.Item("Reg_cod")), New JProperty("Reg_Des", dr.Item("Reg_Des"))))
            Next

            Dim listItems = (From dr In Dt.Rows
                             Select New AgronicaCoreModelsSTD.metaschema.Regolamenti(dr.Item("Reg_Cod")) With {
                                .descrizione = dr.Item("Reg_Des")
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

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function CaricaComboRegolamenti_NuovoImpianto(InData As CoreWS_Generic(Of String)) As RispostaStandard
        Dim r As New RispostaStandard

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        Try
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim ddl As New DropDownList
            AgronicaCoreUtility.CaricaListControl.Regolamento(
                ddl,
                False, "", "",
                "", "", objParametri_Server
            )
            Dim list = (From item In ddl.Items
                        Select New AgronicaCoreModelsSTD.baseClass.BaseCodeDescr(item.Value, item.Text)
                       ).ToList()

            r.RispostaStringa = JsonConvert.SerializeObject(list)
            r.RispostaOK = True
        Catch ex As Exception
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r
    End Function

End Class