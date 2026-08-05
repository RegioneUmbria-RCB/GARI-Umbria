Imports System.Web.Services
Imports Newtonsoft.Json
Imports AgronicaCoreDataProvider
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreDTOStd.InData.Widgets
Imports AgronicaCoreDataProvider.DataProvider
Imports AgronicaCoreAnagrafeBIZ
Imports AgronicaCoreUtentiDAL

' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
<System.Web.Script.Services.ScriptService()>
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
Public Class WidgetIndiciProduttivitaWS
    Inherits System.Web.Services.WebService

    <WebMethod(EnableSession:=True)>
    Public Function readWidgetIndiciProduttivita(ByVal InData As Object) As rispostaStandard(Of WidgetIndiciProduttivitaGlobal)

        Dim r As New rispostaStandard(Of WidgetIndiciProduttivitaGlobal)

        Dim a As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}
        Dim params As CoreWS_Generic(Of WidgetRequestIndiciProduttivita) =
            JsonConvert.DeserializeObject(Of CoreWS_Generic(Of WidgetRequestIndiciProduttivita))(JsonConvert.SerializeObject(InData), a)

        If params.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If params.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Try
            Dim objParametri_Super_Server As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(params.objP.objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(params.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(params.objP.objP_utenti)

            ' Servono per istanziare AgroWebConfig
            HttpContext.Current.Session("ASG_objParametri_Super_Server") = objParametri_Super_Server
            HttpContext.Current.Session("ASG_objParametri_Server") = objParametri_Server
            HttpContext.Current.Session("ASG_objParametri_Utenti") = objParametri_Utenti

            Dim objWindget As New WidgetIndiciProduttivitaRead

            r.RispostaStringa = New WidgetIndiciProduttivitaGlobal
            r.RispostaStringa.indiciXImpresa = New List(Of WidgetIndiciProduttivitaXImpresa)

            For Each rImp In params.InData.requestXImpresa
                Dim indiceXImpresa = objWindget.initializeWidgetIndiciProduttivitaXImpresa(rImp.piva, objParametri_Server)
                Dim indiciXAnno As New List(Of WidgetIndiciProduttivitaXAnno)

                For Each rAnno In rImp.specieXYear
                    Dim indiceXAnno As New WidgetIndiciProduttivitaXAnno
                    indiceXAnno.indiciProduttivita = New List(Of WidgetIndiciProduttivita)
                    indiceXAnno.year = rAnno.year

                    If rAnno.specieVegetale.Count = 0 Then
                        Dim indici = objWindget.readSpecificIndiciProduttivita(rImp.piva, rAnno.year, objParametri_Server)
                        indiceXAnno.indiciProduttivita = indici
                    Else
                        For Each rSpecie In rAnno.specieVegetale
                            Dim indici = objWindget.readSpecificIndiciProduttivita(rImp.piva, rAnno.year, objParametri_Server, rSpecie.codice)
                            Dim specieCod = rSpecie.codice

                            indiceXAnno.indiciProduttivita = indici
                        Next
                    End If

                    Dim generalIndices = objWindget.readGeneralIndiciProduttivita(indiceXImpresa.regione.codice, rAnno.year, objParametri_Server)
                    indiceXAnno.indiciProduttivita = indiceXAnno.indiciProduttivita.Concat(generalIndices).ToList()
                    indiciXAnno.Add(indiceXAnno)
                Next

                indiceXImpresa.indiciXAnno = indiciXAnno
                r.RispostaStringa.indiciXImpresa.Add(indiceXImpresa)
            Next
        Catch ex As Exception

        End Try

        r.RispostaOK = True

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Function readAvailableYearsIndiciProduttivita(ByVal InData As Object) As rispostaStandard(Of List(Of Integer))

        Dim r As New rispostaStandard(Of List(Of Integer))

        Dim a As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}
        Dim params As CoreWS_Generic(Of String) =
            JsonConvert.DeserializeObject(Of CoreWS_Generic(Of String))(JsonConvert.SerializeObject(InData), a)

        If params.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If params.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Try
            Dim objParametri_Super_Server As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(params.objP.objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(params.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(params.objP.objP_utenti)

            ' Servono per istanziare AgroWebConfig
            HttpContext.Current.Session("ASG_objParametri_Super_Server") = objParametri_Super_Server
            HttpContext.Current.Session("ASG_objParametri_Server") = objParametri_Server
            HttpContext.Current.Session("ASG_objParametri_Utenti") = objParametri_Utenti

            Dim objWindget As New WidgetIndiciProduttivitaRead

            r.RispostaStringa = objWindget.readAvailableYears(params.InData, objParametri_Server)

        Catch ex As Exception

        End Try

        r.RispostaOK = True

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Function readKPI(ByVal InData As Object) As rispostaStandard(Of List(Of WidgetKPI))

        Dim r As New rispostaStandard(Of List(Of WidgetKPI))

        Dim a As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}
        Dim params As CoreWS_Generic(Of WidgetRequestKpi) =
            JsonConvert.DeserializeObject(Of CoreWS_Generic(Of WidgetRequestKpi))(JsonConvert.SerializeObject(InData), a)

        If params.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If params.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Try
            Dim objParametri_Super_Server As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(params.objP.objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(params.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(params.objP.objP_utenti)

            Dim leggiLingua As New Lingue_Read
            Dim dtLingua As DataTable = leggiLingua.Leggi_datoCODGIAS(objParametri_Server.Lingua_Cod, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Utenti)
            Dim linguaCodiceISO As String = dtLingua.Rows(0)("CodiceISO")
            Threading.Thread.CurrentThread.CurrentUICulture = New Globalization.CultureInfo(linguaCodiceISO)

            ' Servono per istanziare AgroWebConfig
            HttpContext.Current.Session("ASG_objParametri_Super_Server") = objParametri_Super_Server
            HttpContext.Current.Session("ASG_objParametri_Server") = objParametri_Server
            HttpContext.Current.Session("ASG_objParametri_Utenti") = objParametri_Utenti

            Dim objWidget As New WidgetIndiciProduttivitaRead
            r.RispostaStringa = objWidget.readKPI(params.InData.piva, params.InData.year, objParametri_Server)

            Return r

        Catch ex As Exception

        End Try

        r.RispostaOK = True

        Return r

    End Function

End Class