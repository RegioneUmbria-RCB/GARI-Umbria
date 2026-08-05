Imports System.Web.Services
Imports System.ComponentModel
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreDataProvider
Imports Newtonsoft.Json
Imports AgronicaCoreDTOStd.InData.Metaschema

' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
<System.Web.Script.Services.ScriptService()>
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<ToolboxItem(False)>
Public Class Fertilizzazione
    Inherits System.Web.Services.WebService

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiEfficienza_PUA_2007(InData As Object) As rispostaStandard(Of Decimal)

        Dim r As New rispostaStandard(Of Decimal)

        Try
            Dim datiRequest As String = JsonConvert.SerializeObject(InData)
            Dim objRequest = JsonConvert.DeserializeObject(Of CoreWS_Generic(Of LeggiEfficienza))(datiRequest)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_server)
            Dim objParametri_Fertilizzanti As LeggiEfficienza = objRequest.InData

            Dim efficienza As Decimal = 0

            If objParametri_Fertilizzanti IsNot Nothing Then
                Dim tipoFertilizzante As Integer = If(objParametri_Fertilizzanti.tipoFertilizzante IsNot Nothing, objParametri_Fertilizzanti.tipoFertilizzante.codice, 0)
                Dim epoca As Integer = If(objParametri_Fertilizzanti.tipoFertilizzante IsNot Nothing, objParametri_Fertilizzanti.epoca.codice, 0)

                Dim objEff As New AgronicaCoreMetaSchemaDAL.EfficienzaxTipoFertilizzante_R
                efficienza = objEff.Efficienza_From_Id_Tp_Fer_Em_Cod(tipoFertilizzante,
                                                                     epoca,
                                                                     objParametri_Server)
            End If

            r.RispostaStringa = efficienza
            r.RispostaOK = True

        Catch ex As Exception
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.RispostaOK = False
        End Try

        Return r

    End Function


    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiEfficienza(InData As Object) As rispostaStandard(Of Decimal)

        Dim r As New rispostaStandard(Of Decimal)

        Try
            Dim datiRequest As String = JsonConvert.SerializeObject(InData)
            Dim objRequest = JsonConvert.DeserializeObject(Of CoreWS_Generic(Of LeggiEfficienza))(datiRequest)
            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_server)
            Dim objParametri_Fertilizzanti As LeggiEfficienza = objRequest.InData

            Dim efficienza As Decimal = 0

            If objParametri_Fertilizzanti IsNot Nothing Then

                Dim FertilizzantiBiz As New AgronicaControlli_2010.STD_Fertilizzanti
                efficienza = FertilizzantiBiz.LeggiEfficienza(objParametri_Fertilizzanti.effluente,
                                                              objParametri_Fertilizzanti.epoca,
                                                              objParametri_Fertilizzanti.tipoAllevamento,
                                                              objParametri_Fertilizzanti.valoreDose,
                                                              objParametri_Fertilizzanti.disciplinare,
                                                              objParametri_Fertilizzanti.classiTessitura,
                                                              objParametri_Fertilizzanti.specie,
                                                              objParametri_Super_Server,
                                                              objParametri_Server)

            End If

            r.RispostaStringa = efficienza
            r.RispostaOK = True

        Catch ex As Exception
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.RispostaOK = False
        End Try

        Return r

    End Function


End Class