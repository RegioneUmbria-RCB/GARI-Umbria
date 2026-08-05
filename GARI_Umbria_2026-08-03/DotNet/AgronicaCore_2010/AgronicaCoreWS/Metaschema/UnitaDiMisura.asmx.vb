Imports System.Web.Services
Imports System.ComponentModel
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreDataProvider
Imports Newtonsoft.Json
Imports AgronicaCoreDTOStd.InData.Metaschema
Imports AgronicaCoreUtentiDAL

' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
<System.Web.Script.Services.ScriptService()>
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<ToolboxItem(False)>
Public Class UnitaDiMisura
    Inherits System.Web.Services.WebService

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Leggi_UnitaDiMisura_QdC(InData As Object) As rispostaStandard(Of List(Of AgronicaCoreModelsSTD.metaschema.UnitaDiMisura))

        Dim r As New rispostaStandard(Of List(Of AgronicaCoreModelsSTD.metaschema.UnitaDiMisura))

        Try

            Dim datiRequest As String = JsonConvert.SerializeObject(InData)
            Dim objRequest = JsonConvert.DeserializeObject(Of CoreWS_Generic(Of LeggiUnitaDiMisura))(datiRequest,
                                                                                                     New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local, .NullValueHandling = NullValueHandling.Ignore, .MissingMemberHandling = MissingMemberHandling.Ignore})

            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_utenti)
            Dim objParametri_UnitaDiMisura As LeggiUnitaDiMisura = objRequest.InData

            If objParametri_Server Is Nothing Then
                r.Errore = "objP_server non valorizzato"
                Return r
            End If

            If objParametri_Utenti Is Nothing Then
                r.Errore = "objP_utenti non valorizzato"
                Return r
            End If

            If objParametri_Super_Server Is Nothing Then
                r.Errore = "objP_super_server non valorizzato"
                Return r
            End If

            Dim leggiLingua As New Lingue_Read
            Dim dtLingua As DataTable = leggiLingua.Leggi_datoCODGIAS(objParametri_Server.Lingua_Cod,
                                                                      AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                      "",
                                                                      "",
                                                                      objParametri_Utenti)

            Dim linguaCodiceISO As String = dtLingua.Rows(0)("CodiceISO")
            Threading.Thread.CurrentThread.CurrentUICulture = New Globalization.CultureInfo(linguaCodiceISO)

            Dim UnitaDiMisuraBIZ As New AgronicaControlli_2010.STD_UnitaDiMisura

            Dim UdmList = UnitaDiMisuraBIZ.LeggiUnitaDiMisura(objParametri_UnitaDiMisura.unitaDiMisura,
                                                              objParametri_UnitaDiMisura.elem_cod,
                                                              objParametri_UnitaDiMisura.avversita,
                                                              objParametri_UnitaDiMisura.lavorazione,
                                                              objParametri_UnitaDiMisura.tipo_Attivita,
                                                              objParametri_UnitaDiMisura.tipo_Ricetta,
                                                              objParametri_UnitaDiMisura.doseEtichetta,
                                                              objParametri_UnitaDiMisura.dettaglioTrattamento,
                                                              objParametri_UnitaDiMisura.dettaglioSemina,
                                                              objParametri_UnitaDiMisura.dettaglioFertilizzazione,
                                                              objParametri_Server)

            r.RispostaStringa = UdmList

            r.RispostaOK = True

        Catch ex As Exception
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function


    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Leggi_UnitaMisura(InData As Object) As rispostaStandard(Of List(Of AgronicaCoreModelsSTD.metaschema.UnitaDiMisura))
        Dim r As New rispostaStandard(Of List(Of AgronicaCoreModelsSTD.metaschema.UnitaDiMisura))
        Try
            Dim datiRequest As String = JsonConvert.SerializeObject(InData)
            Dim objRequest = JsonConvert.DeserializeObject(Of CoreWS_Generic(Of LeggiUnitaDiMisura))(
                                            datiRequest,
                                            New JsonSerializerSettings With {
                                                .DateTimeZoneHandling = DateTimeZoneHandling.Local,
                                                .NullValueHandling = NullValueHandling.Ignore,
                                                .MissingMemberHandling = MissingMemberHandling.Ignore
                                            }
                                        )
            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_utenti)
            Dim objParametri_UnitaDiMisura As LeggiUnitaDiMisura = objRequest.InData

            If objParametri_Server Is Nothing Then
                r.Errore = "objP_server non valorizzato"
                Return r
            End If
            If objParametri_Utenti Is Nothing Then
                r.Errore = "objP_utenti non valorizzato"
                Return r
            End If
            If objParametri_Super_Server Is Nothing Then
                r.Errore = "objP_super_server non valorizzato"
                Return r
            End If

            Dim leggiLingua As New Lingue_Read
            Dim dtLingua As DataTable = leggiLingua.Leggi_datoCODGIAS(objParametri_Server.Lingua_Cod,
                                                                      AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                      "",
                                                                      "",
                                                                      objParametri_Utenti)

            Dim linguaCodiceISO As String = dtLingua.Rows(0)("CodiceISO")
            Threading.Thread.CurrentThread.CurrentUICulture = New Globalization.CultureInfo(linguaCodiceISO)

            Dim UnitaDiMisuraBIZ As New AgronicaControlli_2010.STD_UnitaDiMisura

            Dim UdmList = UnitaDiMisuraBIZ.LeggiUnitaDiMisuraConTipoControllo(objParametri_Server)

            r.RispostaStringa = UdmList

            r.RispostaOK = True

        Catch ex As Exception
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Leggi_UnitaDiMisura_QdC_NG(InData As CoreWS_Generic(Of Object)) As rispostaStandard(Of List(Of AgronicaCoreModelsSTD.metaschema.UnitaDiMisura))

        Dim r As New rispostaStandard(Of List(Of AgronicaCoreModelsSTD.metaschema.UnitaDiMisura))

        Try

            Dim datiRequest As String = JsonConvert.SerializeObject(InData)
            Dim objRequest = JsonConvert.DeserializeObject(Of CoreWS_Generic(Of LeggiUnitaDiMisura))(datiRequest,
                                                                                                     New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local, .NullValueHandling = NullValueHandling.Ignore, .MissingMemberHandling = MissingMemberHandling.Ignore})

            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_utenti)
            Dim objParametri_UnitaDiMisura As LeggiUnitaDiMisura = objRequest.InData

            If objParametri_Server Is Nothing Then
                r.Errore = "objP_server non valorizzato"
                Return r
            End If

            If objParametri_Utenti Is Nothing Then
                r.Errore = "objP_utenti non valorizzato"
                Return r
            End If

            If objParametri_Super_Server Is Nothing Then
                r.Errore = "objP_super_server non valorizzato"
                Return r
            End If

            Dim leggiLingua As New Lingue_Read
            Dim dtLingua As DataTable = leggiLingua.Leggi_datoCODGIAS(objParametri_Server.Lingua_Cod,
                                                                      AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                      "",
                                                                      "",
                                                                      objParametri_Utenti)

            Dim linguaCodiceISO As String = dtLingua.Rows(0)("CodiceISO")
            Threading.Thread.CurrentThread.CurrentUICulture = New Globalization.CultureInfo(linguaCodiceISO)

            Dim UnitaDiMisuraBIZ As New AgronicaControlli_2010.STD_UnitaDiMisura

            Dim UdmList = UnitaDiMisuraBIZ.LeggiUnitaDiMisura(objParametri_UnitaDiMisura.unitaDiMisura,
                                                              objParametri_UnitaDiMisura.elem_cod,
                                                              objParametri_UnitaDiMisura.avversita,
                                                              objParametri_UnitaDiMisura.lavorazione,
                                                              objParametri_UnitaDiMisura.tipo_Attivita,
                                                              objParametri_UnitaDiMisura.tipo_Ricetta,
                                                              objParametri_UnitaDiMisura.doseEtichetta,
                                                              objParametri_UnitaDiMisura.dettaglioTrattamento,
                                                              objParametri_UnitaDiMisura.dettaglioSemina,
                                                              objParametri_UnitaDiMisura.dettaglioFertilizzazione,
                                                              objParametri_Server)

            r.RispostaStringa = UdmList

            r.RispostaOK = True

        Catch ex As Exception
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Recupera_UdM_da_FrCod(InData As Object) As rispostaStandard(Of AgronicaCoreModelsSTD.metaschema.UnitaDiMisura)

        Dim r As New rispostaStandard(Of AgronicaCoreModelsSTD.metaschema.UnitaDiMisura)

        Try

            Dim datiRequest As String = JsonConvert.SerializeObject(InData)
            Dim objRequest = JsonConvert.DeserializeObject(Of CoreWS_Generic(Of LeggiUnitaDiMisura))(datiRequest)
            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_utenti)
            Dim objParametri_UnitaDiMisura As LeggiUnitaDiMisura = objRequest.InData

            If objParametri_Server Is Nothing Then
                r.Errore = "objP_server non valorizzato"
                Return r
            End If

            If objParametri_Utenti Is Nothing Then
                r.Errore = "objP_utenti non valorizzato"
                Return r
            End If

            If objParametri_Super_Server Is Nothing Then
                r.Errore = "objP_super_server non valorizzato"
                Return r
            End If

            Dim Udm_Cod As Integer = 0

            Dim Udm_Sim As String = ""

            Dim Udm_Des As String = ""

            Dim objWebConfig As New AgronicaCoreGestioneRichieste.AgroWebConfig(objParametri_Super_Server, objParametri_Server, False)

            Dim objDPILeggi As New AgronicaCoreDpiBIZ.Fitofarmaci_Leggi

            objDPILeggi.Recupera_UdM_da_FrCod(Nothing,
                                              objParametri_UnitaDiMisura.dettaglioTrattamento.prodotto.codice,
                                              Udm_Cod,
                                              Udm_Sim,
                                              Udm_Des,
                                              Nothing,
                                              objParametri_Utenti,
                                              objParametri_Server,
                                              objParametri_Super_Server)

            If Udm_Cod > 0 Then
                Dim obj_STD_UnitaMisura As New AgronicaControlli_2010.STD_UnitaDiMisura

                Dim List_Udm = obj_STD_UnitaMisura.LeggiUnitaDiMisuraConTipoControllo(objParametri_Server)

                r.RispostaStringa = List_Udm.Where(Function(udm) udm.codice = Udm_Cod)(0)
            Else
                Dim newUnitaDiMisura As New AgronicaCoreModelsSTD.metaschema.UnitaDiMisura

                newUnitaDiMisura.codice = Udm_Cod

                newUnitaDiMisura.descrizione = Udm_Des

                newUnitaDiMisura.simbolo = Udm_Sim

                r.RispostaStringa = newUnitaDiMisura
            End If

            r.RispostaOK = True

        Catch ex As Exception
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r
    End Function

End Class