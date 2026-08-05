Imports System.Web.Services
Imports System.ComponentModel
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreDataProvider
Imports Newtonsoft.Json
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDTOStd.InData.Metaschema

' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
<System.Web.Script.Services.ScriptService()>
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<ToolboxItem(False)>
Public Class Avversita
    Inherits System.Web.Services.WebService

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Leggi_Avversita_APP(ByVal objP_super_server As String,
                                          ByVal objP_server As String, ByVal objP_utenti As String) As RispostaStandard

        Dim r As New RispostaStandard

        Try

            Dim objParametri_SuperServer As AgronicaCoreDataProvider.AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(objP_utenti)


            Dim objAvv As New AgronicaCoreMetaSchemaDAL.Avversita_R

            Dim Dt = objAvv.Leggi(0, "",
                          enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                          "", "",
                          objParametri_Server)

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            r.RispostaStringa = JsonConvert.SerializeObject(Dt, Formatting.None, serializerSettings)

            r.RispostaOK = True

        Catch ex As Exception
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Leggi_Avversita_QdC(InData As Object) As rispostaStandard(Of List(Of AgronicaCoreModelsSTD.metaschema.avversita.AvversitaGruppo))

        Dim r As New rispostaStandard(Of List(Of AgronicaCoreModelsSTD.metaschema.avversita.AvversitaGruppo))

        Try
            Dim datiRequest As String = JsonConvert.SerializeObject(InData)

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            serializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Local
            serializerSettings.NullValueHandling = NullValueHandling.Ignore
            serializerSettings.MissingMemberHandling = MissingMemberHandling.Ignore
            Dim objRequest = JsonConvert.DeserializeObject(Of CoreWS_Generic(Of LeggiAvversita))(datiRequest, serializerSettings)

            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_utenti)
            Dim objParametri_Avversita As LeggiAvversita = objRequest.InData

            Dim AvversitaBiz As New AgronicaControlli_2010.STD_Avversita
            Dim avversitaList = AvversitaBiz.LeggiAvversita(objParametri_Avversita.tipoAttivita,
                                                                objParametri_Avversita.lavorazione,
                                                                objParametri_Avversita.impianti,
                                                                objParametri_Avversita.specie,
                                                                objParametri_Avversita.disciplinare,
                                                                objParametri_Avversita.epocaDPI,
                                                                objParametri_Avversita.dettaglioTrattamento,
                                                                objParametri_Avversita.data,
                                                                objParametri_Super_Server,
                                                                objParametri_Server,
                                                                objParametri_Utenti)

            r.RispostaStringa = avversitaList
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
    Public Function Leggi_SoglieAvversita_QdC(InData As Object) As rispostaStandard(Of List(Of AgronicaCoreModelsSTD.metaschema.Soglia))

        Dim r As New rispostaStandard(Of List(Of AgronicaCoreModelsSTD.metaschema.Soglia))

        Try
            Dim datiRequest As String = JsonConvert.SerializeObject(InData)
            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            serializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Local
            serializerSettings.NullValueHandling = NullValueHandling.Ignore
            serializerSettings.MissingMemberHandling = MissingMemberHandling.Ignore
            Dim objRequest = JsonConvert.DeserializeObject(Of CoreWS_Generic(Of LeggiAvversita))(datiRequest, serializerSettings)

            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_utenti)
            Dim objParametri_Avversita As LeggiAvversita = objRequest.InData

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

            Dim AvversitaBiz As New AgronicaControlli_2010.STD_Avversita
            Dim sogliaList = AvversitaBiz.LeggiSoglieAvversita(objParametri_Avversita.disciplinare,
                                                                objParametri_Avversita.avversitaGruppo,
                                                                objParametri_Super_Server,
                                                                objParametri_Server,
                                                                objParametri_Utenti)

            r.RispostaStringa = sogliaList
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
    Public Function Controlla_Soglia_Avversita_Soddisfatta_QdC(InData As Object) As RispostaStandard

        Dim r As New RispostaStandard

        Try
            Dim datiRequest As String = JsonConvert.SerializeObject(InData)

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            serializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Local
            serializerSettings.NullValueHandling = NullValueHandling.Ignore
            serializerSettings.MissingMemberHandling = MissingMemberHandling.Ignore
            Dim objRequest = JsonConvert.DeserializeObject(Of CoreWS_Generic(Of LeggiAvversita))(datiRequest, serializerSettings)

            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_utenti)
            Dim objParametri_Avversita As LeggiAvversita = objRequest.InData

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

            Dim AvversitaBiz As New AgronicaControlli_2010.STD_Avversita
            Dim strVerificaSoglia = AvversitaBiz.ControllaSogliaSoddisfatta(objParametri_Avversita.soglia,
                                                                objParametri_Avversita.impianti,
                                                                objParametri_Avversita.appezzamenti,
                                                                objParametri_Avversita.data,
                                                                objParametri_Avversita.specie,
                                                                objParametri_Server)

            r.RispostaStringa = strVerificaSoglia
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
    Public Function Controlla_Soglia_Avversita_Soddisfatta_QdC_NG(InData As CoreWS_Generic(Of LeggiAvversita)) As RispostaStandard

        Dim r As New RispostaStandard

        Try
            Dim datiRequest As String = JsonConvert.SerializeObject(InData)

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            serializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Local
            serializerSettings.NullValueHandling = NullValueHandling.Ignore
            serializerSettings.MissingMemberHandling = MissingMemberHandling.Ignore
            Dim objRequest = JsonConvert.DeserializeObject(Of CoreWS_Generic(Of LeggiAvversita))(datiRequest, serializerSettings)

            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_utenti)
            Dim objParametri_Avversita As LeggiAvversita = objRequest.InData

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

            Dim AvversitaBiz As New AgronicaControlli_2010.STD_Avversita
            Dim strVerificaSoglia = AvversitaBiz.ControllaSogliaSoddisfatta(objParametri_Avversita.soglia,
                                                                objParametri_Avversita.impianti,
                                                                objParametri_Avversita.appezzamenti,
                                                                objParametri_Avversita.data,
                                                                objParametri_Avversita.specie,
                                                                objParametri_Server)

            r.RispostaStringa = strVerificaSoglia
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
    Public Function Leggi_AvversitaInsettiUtili_QdC(InData As Object) As rispostaStandard(Of List(Of AgronicaCoreModelsSTD.metaschema.avversita.AvversitaGruppo))

        Dim r As New rispostaStandard(Of List(Of AgronicaCoreModelsSTD.metaschema.avversita.AvversitaGruppo))

        Try
            Dim datiRequest As String = JsonConvert.SerializeObject(InData)

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            serializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Local
            serializerSettings.NullValueHandling = NullValueHandling.Ignore
            serializerSettings.MissingMemberHandling = MissingMemberHandling.Ignore
            Dim objRequest = JsonConvert.DeserializeObject(Of CoreWS_Generic(Of LeggiAvversita))(datiRequest, serializerSettings)

            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_utenti)
            Dim objParametri_Avversita As LeggiAvversita = objRequest.InData

            Dim AvversitaBiz As New AgronicaControlli_2010.STD_Avversita
            Dim avversitaList = AvversitaBiz.LeggiAvversitaInsettiUtili(objParametri_Avversita.tipoAttivita,
                                                                        objParametri_Avversita.lavorazione,
                                                                        objParametri_Avversita.impianti,
                                                                        objParametri_Avversita.specie,
                                                                        objParametri_Avversita.dettaglioTrattamento,
                                                                        objParametri_Avversita.data,
                                                                        objParametri_Super_Server,
                                                                        objParametri_Server,
                                                                        objParametri_Utenti)

            r.RispostaStringa = avversitaList
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
    Public Function Leggi_InfestantiAttive_APP(ByVal objP_super_server As String,
                                          ByVal objP_server As String, ByVal objP_utenti As String) As RispostaStandard

        Dim r As New RispostaStandard

        Try

            Dim objParametri_SuperServer As AgronicaCoreDataProvider.AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(objP_utenti)

            Dim objInfestAttive As New AgronicaCoreMetaSchemaDAL.InfestantiAttive_R
            Dim Dt = objInfestAttive.Leggi(0, 0, enumSelezioneVariabile.Selezione_JoinDescrizioni,
                            "", "", objParametri_Server)

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            r.RispostaStringa = JsonConvert.SerializeObject(Dt, Formatting.None, serializerSettings)

            r.RispostaOK = True

        Catch ex As Exception
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function



    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Leggi_AvversitaInnesco_QdC(InData As Object) As rispostaStandard(Of List(Of AgronicaCoreModelsSTD.metaschema.avversita.AvversitaGruppo))

        Dim r As New rispostaStandard(Of List(Of AgronicaCoreModelsSTD.metaschema.avversita.AvversitaGruppo))

        Try
            Dim datiRequest As String = JsonConvert.SerializeObject(InData)

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            serializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Local
            serializerSettings.NullValueHandling = NullValueHandling.Ignore
            serializerSettings.MissingMemberHandling = MissingMemberHandling.Ignore
            Dim objRequest = JsonConvert.DeserializeObject(Of CoreWS_Generic(Of LeggiAvversita))(datiRequest, serializerSettings)

            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_utenti)
            Dim objParametri_Avversita As LeggiAvversita = objRequest.InData

            Dim AvversitaBiz As New AgronicaControlli_2010.STD_Avversita
            Dim avversitaList = AvversitaBiz.LeggiAvversitaInnesco(objParametri_Avversita.tipoAttivita,
                                                                        objParametri_Avversita.statoAttivita,
                                                                        objParametri_Avversita.lavorazione,
                                                                        objParametri_Avversita.dettaglioTrattamento,
                                                                        objParametri_Avversita.impianti,
                                                                        objParametri_Avversita.specie,
                                                                        objParametri_Avversita.disciplinare,
                                                                        objParametri_Avversita.epocaDPI,
                                                                        objParametri_Avversita.data,
                                                                        objParametri_Avversita.escludiGiacenzeZero,
                                                                        objParametri_Avversita.magazziniAgenzie,
                                                                        magazziniEsterni:=False,
                                                                        codiceAvversita:=0,
                                                                        soloLetturaAnagrafica:=Not objParametri_Avversita.visualizzaMovimentiMagazzino,
                                                                        objParametri_Super_Server,
                                                                        objParametri_Server,
                                                                        objParametri_Utenti)

            r.RispostaStringa = avversitaList
            r.RispostaOK = True

        Catch ex As Exception
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.RispostaOK = False
        End Try

        Return r

    End Function
End Class