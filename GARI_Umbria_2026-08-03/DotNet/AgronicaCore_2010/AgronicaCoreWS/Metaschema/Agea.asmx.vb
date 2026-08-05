Imports System.ComponentModel
Imports System.IO
Imports System.Net
Imports System.Web.Services
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDTOStd.InData.Agea
Imports AgronicaCoreModelsSTD.baseClass
Imports AgronicaCoreModelsSTD.exceptions
Imports AgronicaCoreVarieBIZ
Imports InData.Agea
Imports InData.Metaschema
Imports Newtonsoft.Json

' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
<System.Web.Script.Services.ScriptService()>
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<ToolboxItem(False)>
Public Class Agea
    Inherits System.Web.Services.WebService

    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function ReadAgeaMachineCodes(ByVal InData As CoreWS_Generic(Of CodificaMacchineAgeaRequest)) As rispostaStandard(Of List(Of BaseCodeDescrStr))
        Dim r As New rispostaStandard(Of List(Of BaseCodeDescrStr))

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If InData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Try
            Dim objMacchine As New AgronicaCoreMetaSchemaBIZ.AgeaCodifiche

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            Dim dt As DataTable
            Dim risp As New List(Of BaseCodeDescrStr)

            dt = objMacchine.ReadAgeaMachineCodes(objParametri_Server, InData.InData.ageaCod, InData.InData.ageaDes, InData.InData.classCod)

            For Each row As DataRow In dt.Rows
                Dim newCode = New BaseCodeDescrStr(row.Item("AGEA_Cod"), row.Item("AGEA_Des"))
                risp.Add(newCode)
            Next row

            r.RispostaOK = True
            r.RispostaStringa = risp

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try
        Return r
    End Function

#Region "Esportazione QdC"

    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function GetAllBundleState(ByVal InData As CoreWS_Generic(Of ExportQdCtoAgeaPaginated)) As rispostaStandard(Of String)
        Dim r As New rispostaStandard(Of String)

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If InData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        If InData.objP.objP_super_server = "" Then
            r.Errore = "objP_super_server non valorizzato"
            Return r
        End If

        Dim AllBundleState As String = ""

        Try
            Dim objBundle As New AgronicaCoreAgeaBIZ.BundleState

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)
            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_super_server)

            AllBundleState = objBundle.GetBundleState(InData.InData.impresa.partitaIva, InData.InData.impresa.CUAA, InData.InData.anno, InData.InData.Skip, InData.InData.Top, False, objParametri_Server, objParametri_Super_Server)

            r.RispostaOK = True
            r.RispostaStringa = AllBundleState

        Catch ex As Exception
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.RispostaOK = False
        End Try
        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function CountBundleState(ByVal InData As CoreWS_Generic(Of ExportQdCtoAgea)) As rispostaStandard(Of String)
        Dim r As New rispostaStandard(Of String)

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If InData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        If InData.objP.objP_super_server = "" Then
            r.Errore = "objP_super_server non valorizzato"
            Return r
        End If

        Dim LastBundleState As String = ""

        Try
            Dim objBundle As New AgronicaCoreAgeaBIZ.BundleState

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)
            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_super_server)

            LastBundleState = objBundle.CountBundleState(InData.InData.impresa.partitaIva, InData.InData.impresa.CUAA, InData.InData.anno, objParametri_Server, objParametri_Super_Server)

            r.RispostaOK = True
            r.RispostaStringa = LastBundleState

        Catch ex As Exception
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.RispostaOK = False
        End Try
        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function GetLastBundleState(ByVal InData As CoreWS_Generic(Of ExportQdCtoAgea)) As rispostaStandard(Of String)
        Dim r As New rispostaStandard(Of String)

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If InData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        If InData.objP.objP_super_server = "" Then
            r.Errore = "objP_super_server non valorizzato"
            Return r
        End If

        Dim LastBundleState As String = ""

        Try
            Dim objBundle As New AgronicaCoreAgeaBIZ.BundleState

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)
            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_super_server)

            LastBundleState = objBundle.GetBundleState(InData.InData.impresa.partitaIva, InData.InData.impresa.CUAA, InData.InData.anno, 0, 0, True, objParametri_Server, objParametri_Super_Server)

            r.RispostaOK = True
            r.RispostaStringa = LastBundleState

        Catch ex As Exception
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.RispostaOK = False
        End Try
        Return r
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function MapAttivitaToAgea(InData As CoreWS_Generic(Of ExportQdCtoAgea)) As rispostaStandard(Of String)
        Dim r As New rispostaStandard(Of String)

        Dim List_Errori_Gias As New List(Of ErroreGias)

        r.RispostaOK = False
        r.RispostaStringa = ""

        Try

            'Ottengo AgronicaCoreParametri
            Dim strObjP As String = JsonConvert.SerializeObject(InData.objP)
            Dim objP As CoreWS_GenericObjP = JsonConvert.DeserializeObject(Of CoreWS_GenericObjP)(strObjP)

            Dim obj As New AgronicaCoreAgeaBIZ.AttivitaToAgea

            Dim bundle_Id = obj.MapAttivitatoAgea(InData.InData.impresa.partitaIva, InData.InData.impresa.CUAA,
                                                  InData.InData.anno, InData.InData.impresa.ragioneSociale,
                                                  objP, InData.InData.erroriDaBypassare, List_Errori_Gias)

            If IsNothing(List_Errori_Gias) OrElse List_Errori_Gias.Count = 0 Then
                r.RispostaOK = True
                r.RispostaStringa = bundle_Id
            End If

        Catch ex As Exception

            If IsNothing(List_Errori_Gias) OrElse List_Errori_Gias.Count = 0 Then
                'uso questa funzione per ottenere il Messaggio..:
                r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            End If

        Finally
            r.ErroriGias = List_Errori_Gias
        End Try

        Return r
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function SendAttivitaToAgea(InData As CoreWS_Generic(Of ExportQdCtoAgea)) As rispostaStandard(Of String)
        Dim r As New rispostaStandard(Of String)

        Dim List_Errori_Gias As New List(Of ErroreGias)

        r.RispostaOK = False
        r.RispostaStringa = ""

        Try
            Dim obj As New AgronicaCoreAgeaBIZ.AttivitaToAgea
            Dim bundle_Id = obj.SendAttivitaToAgea(InData.InData.impresa.partitaIva, InData.InData.impresa.CUAA, InData.InData.anno,
                                                   InData.InData.impresa.ragioneSociale, InData.objP, List_Errori_Gias)

            If IsNothing(List_Errori_Gias) OrElse List_Errori_Gias.Count = 0 Then
                r.RispostaOK = True
                r.RispostaStringa = bundle_Id
            End If

        Catch ex As Exception

            If IsNothing(List_Errori_Gias) OrElse List_Errori_Gias.Count = 0 Then
                'uso questa funzione per ottenere il Messaggio..:
                r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            End If

        Finally
            r.ErroriGias = List_Errori_Gias
        End Try

        Return r
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function ExtractQdCToAgea(InData As CoreWS_Generic(Of ExportQdCtoAgea)) As rispostaStandard(Of String)
        Dim r As New rispostaStandard(Of String)

        Dim List_Errori_Gias As New List(Of ErroreGias)

        r.RispostaOK = False
        r.RispostaStringa = ""

        Try

            'Ottengo AgronicaCoreParametri
            Dim strObjP As String = JsonConvert.SerializeObject(InData.objP)
            Dim objP As CoreWS_GenericObjP = JsonConvert.DeserializeObject(Of CoreWS_GenericObjP)(strObjP)

            Dim obj As New AgronicaCoreAgeaBIZ.AttivitaToAgea

            Dim bundle = obj.ExtractQdCToAgea(InData.InData.impresa.partitaIva, InData.InData.impresa.CUAA,
                                              InData.InData.anno, InData.InData.impresa.ragioneSociale,
                                              objP, InData.InData.erroriDaBypassare, List_Errori_Gias)

            Dim segnalazioniBloccanti As ErroreGias_Severity() = {ErroreGias_Severity.Bloccante, ErroreGias_Severity.WarningBloccante}
            If IsNothing(List_Errori_Gias) OrElse
                (Not List_Errori_Gias.Any(Function(e) segnalazioniBloccanti.Contains(e.severity))) Then
                r.RispostaOK = True
                r.RispostaStringa = JsonConvert.SerializeObject(bundle)
            End If

        Catch ex As Exception

            If IsNothing(List_Errori_Gias) OrElse List_Errori_Gias.Count = 0 Then
                'uso questa funzione per ottenere il Messaggio..:
                r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
                If IsNothing(List_Errori_Gias) Then
                    List_Errori_Gias = New List(Of ErroreGias)
                End If

                List_Errori_Gias.Add(New ErroreGias() With
                                     {
                                     .severity = ErroreGias_Severity.Bloccante,
                                     .messaggio = r.Errore,
                                     .tipo = ErroreGias_Tipo.NonGestito
                                     })
            End If

        Finally
            r.ErroriGias = List_Errori_Gias
        End Try

        Return r
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function GetBundleData(InData As CoreWS_Generic(Of GetBundleData_In)) As rispostaStandard(Of String)
        Dim r As New rispostaStandard(Of String)

        Try
            Dim objBundle As New AgronicaCoreAgeaBIZ.BundleState

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)
            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_super_server)

            Dim BundleData As String = objBundle.GetBundleData(InData.InData.BundleId, objParametri_Super_Server)

            r.RispostaOK = True
            r.RispostaStringa = BundleData

        Catch ex As Exception
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.RispostaOK = False
        End Try
        Return r
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function ReadBundleLog(InData As CoreWS_Generic(Of ReadBundleLog_In)) As rispostaStandard(Of String)
        Dim r As New rispostaStandard(Of String)

        Try
            Dim objBundle As New AgronicaCoreAgeaBIZ.BundleState

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)
            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_super_server)

            Dim BundleData As String = objBundle.ReadBundleLog(InData.InData.InstanceId, objParametri_Super_Server)

            r.RispostaOK = True
            r.RispostaStringa = BundleData

        Catch ex As Exception
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.RispostaOK = False
        End Try
        Return r
    End Function
#End Region

End Class