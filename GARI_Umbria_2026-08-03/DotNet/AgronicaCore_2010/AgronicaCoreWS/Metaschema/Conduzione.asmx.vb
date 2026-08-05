Imports System.ComponentModel
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreVarieBIZ

' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
' <System.Web.Script.Services.ScriptService()> _
<System.Web.Script.Services.ScriptService()>
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<ToolboxItem(False)>
Public Class Conduzione
    Inherits System.Web.Services.WebService

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function CaricaTecnicaConduzioneSuFila_Modello(InData As CoreWS_Generic(Of AgronicaCoreDTOStd.InData.Metaschema.LeggiConduzione)) As rispostaStandard(Of List(Of AgronicaCoreModelsSTD.metaschema.DensitaImpianto.TecnicaConduzioneSuFila))

        Dim r As New rispostaStandard(Of List(Of AgronicaCoreModelsSTD.metaschema.DensitaImpianto.TecnicaConduzioneSuFila))

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

            Dim key = "ConduzioneSuFila_Modello" & InData.InData.specie.codice
            Dim listItems As List(Of AgronicaCoreModelsSTD.metaschema.DensitaImpianto.TecnicaConduzioneSuFila)

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
            Dim objCOM As New AgronicaCoreAnagrafeDAL.ConduzioneSuFila_R

            Dt = objCOM.Leggi(0, Gru_Cod,
                             enumSelezioneVariabile.Selezione_TabellaCompleta, "",
                            "",
                            objParametri_Server)

            listItems = (From dr In Dt.Rows
                         Select New AgronicaCoreModelsSTD.metaschema.DensitaImpianto.TecnicaConduzioneSuFila(dr("Tecn_Cod")) With {
                                           .descrizione = dr("Tecn_Des")
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
    Public Function CaricaTecnicaConduzioneTraFila_Modello(InData As CoreWS_Generic(Of AgronicaCoreDTOStd.InData.Metaschema.LeggiConduzione)) As rispostaStandard(Of List(Of AgronicaCoreModelsSTD.metaschema.DensitaImpianto.TecnicaConduzioneTraFila))

        Dim r As New rispostaStandard(Of List(Of AgronicaCoreModelsSTD.metaschema.DensitaImpianto.TecnicaConduzioneTraFila))

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

            Dim key = "ConduzioneTraFila_Modello" & InData.InData.specie.codice
            Dim listItems As List(Of AgronicaCoreModelsSTD.metaschema.DensitaImpianto.TecnicaConduzioneTraFila)

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
            Dim objCOM As New AgronicaCoreAnagrafeDAL.ConduzioneTraFila_R

            Dt = objCOM.Leggi(0, Gru_Cod,
                             enumSelezioneVariabile.Selezione_TabellaCompleta, "",
                            "",
                            objParametri_Server)

            listItems = (From dr In Dt.Rows
                         Select New AgronicaCoreModelsSTD.metaschema.DensitaImpianto.TecnicaConduzioneTraFila(dr("Tecn_Cod")) With {
                                           .descrizione = dr("Tecn_Des")
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