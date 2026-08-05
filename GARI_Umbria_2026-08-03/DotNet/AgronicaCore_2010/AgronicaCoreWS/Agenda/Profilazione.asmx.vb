Imports System.Web.Services
Imports Newtonsoft.Json
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreDataProvider
Imports AgronicaCoreModelsSTD.attivita.risorse
Imports AgronicaCoreDTOStd.InData.Agenda
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreModelsSTD.utente


' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
' <System.Web.Script.Services.ScriptService()> _
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<System.Web.Script.Services.ScriptService()>
Public Class Profilazione
    Inherits System.Web.Services.WebService

    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function CaricaDefaultMacchine(InData As CoreWS_Generic(Of LeggiProfilazione)) As RispostaStandard
        Dim r As New RispostaStandard

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

        Try
            Dim objProfilazione As New AgronicaCoreProfilazioneBIZ.Profilazione_R

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)

            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_super_server)

            Dim Piva As String = InData.InData.impresa.partitaIva

            Dim Lav_Cod_List As New List(Of Integer)

            If Not IsNothing(InData.InData.operazioni) AndAlso InData.InData.operazioni.Count > 0 Then
                Lav_Cod_List = InData.InData.operazioni.Select(Function(o) CInt(o.primaryKey.codice)).ToList()
            End If

            Dim Veg_Cod As Integer = InData.InData.specie.codice

            Dim Data As Date = InData.InData.data

            Dim DT As DataTable = objProfilazione.CaricaDefaultMacchine(Piva,
                                                                        Lav_Cod_List,
                                                                        Veg_Cod,
                                                                        Data,
                                                                        objParametri_Server)

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore

            r.RispostaOK = True
            r.RispostaStringa = JsonConvert.SerializeObject(DT, serializerSettings)

        Catch ex As Exception
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.RispostaOK = False
        End Try

        Return r
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function CaricaDefaultOperatori(InData As CoreWS_Generic(Of LeggiProfilazione)) As RispostaStandard


        Dim r As New RispostaStandard

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

        Try

            Dim objProfilazione As New AgronicaCoreProfilazioneBIZ.Profilazione_R

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)

            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_super_server)

            Dim Piva As String = InData.InData.impresa.partitaIva

            Dim Lav_Cod_List As New List(Of Integer)

            If Not IsNothing(InData.InData.operazioni) AndAlso InData.InData.operazioni.Count > 0 Then
                Lav_Cod_List = InData.InData.operazioni.Select(Function(o) CInt(o.primaryKey.codice)).ToList()
            End If

            Dim Veg_Cod As Integer = InData.InData.specie.codice

            Dim Data As Date = InData.InData.data

            Dim DT As DataTable = objProfilazione.CaricaDefaultOperatori(Piva,
                                                                        Lav_Cod_List,
                                                                        Veg_Cod,
                                                                        Data,
                                                                        objParametri_Server)

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore

            r.RispostaOK = True
            r.RispostaStringa = JsonConvert.SerializeObject(DT, Formatting.None, serializerSettings)

        Catch ex As Exception
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.RispostaOK = False
        End Try

        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function CaricaRisorseMacchine(InData As CoreWS_Generic(Of LeggiProfilazione)) As RispostaStandard
        Dim r As New RispostaStandard

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

        Try
            Dim objProfilazione As New AgronicaCoreProfilazioneBIZ.Profilazione_R

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)

            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_super_server)

            Dim Piva As String = InData.InData.impresa.partitaIva

            Dim Data As Date = InData.InData.data

            Dim DT As DataTable = objProfilazione.CaricaRisorseMacchine_QdC(Piva,
                                                                            Data,
                                                                            objParametri_Server,
                                                                            objParametri_Utenti)

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore

            r.RispostaOK = True
            r.RispostaStringa = JsonConvert.SerializeObject(DT, serializerSettings)

        Catch ex As Exception
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.RispostaOK = False
        End Try

        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function CaricaRisorseOperatori(InData As CoreWS_Generic(Of LeggiProfilazione)) As RispostaStandard
        Dim r As New RispostaStandard

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

        Try
            Dim objProfilazione As New AgronicaCoreProfilazioneBIZ.Profilazione_R

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)

            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_super_server)

            Dim Piva As String = InData.InData.impresa.partitaIva

            Dim Data As Date = InData.InData.data

            Dim DT As DataTable = objProfilazione.CaricaRisorseOperatori_QdC(Piva,
                                                                            Data,
                                                                            objParametri_Server)

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore

            r.RispostaOK = True
            r.RispostaStringa = JsonConvert.SerializeObject(DT, serializerSettings)

        Catch ex As Exception
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.RispostaOK = False
        End Try

        Return r
    End Function



    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Scrivi_Profilazione_Dati(InData As CoreWS_Generic(Of LeggiProfilazione)) As RispostaStandard

        Dim risp As Boolean = False

        Dim r As New RispostaStandard

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

        Try
            Dim objProfilazione As New AgronicaCoreProfilazioneBIZ.Profilazione_R

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)

            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_super_server)

            Dim Piva As String = InData.InData.impresa.partitaIva

            Dim Data As Date = InData.InData.data

            Dim Lav_cod_List As List(Of Integer) = InData.InData.operazioni.Select(Function(o) CInt(o.primaryKey.codice)).ToList()

            Dim List_Mac_Cod As List(Of Integer) = InData.InData.macchine.Select(Function(m) m.codice).ToList()

            Dim List_Cod_Risum As List(Of Integer) = InData.InData.contatti.Select(Function(c) CInt(c.risorseUmane(0).codice)).ToList()

            Dim objProf_W As New AgronicaCoreProfilazioneBIZ.Profilazione_W

            risp = objProf_W.Inserisci_Default_CostiAccessori_Profilazione_Dati(Piva,
                                                                            Lav_cod_List,
                                                                            List_Mac_Cod,
                                                                            List_Cod_Risum,
                                                                            objParametri_Server)


            r.RispostaOK = risp

        Catch ex As Exception
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.RispostaOK = False
        End Try

        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function LeggiAziendexUtenti(InData As CoreWS_Generic(Of LeggiAziendexUtenti)) As RispostaStandard

        Dim risp As Boolean = False

        Dim r As New RispostaStandard

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

        Try
            Dim objProfilazione As New AgronicaCoreProfilazioneBIZ.Profilazione_R

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)

            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_super_server)


            Dim objProf_W As New AgronicaCoreProfilazioneBIZ.Profilazione_W

            Dim DT As DataTable = objProfilazione.LeggiAziendexUtenti(objParametri_Server)

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore

            r.RispostaOK = True
            r.RispostaStringa = JsonConvert.SerializeObject(DT, serializerSettings)

            r.RispostaOK = True

        Catch ex As Exception
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.RispostaOK = False
        End Try

        Return r
    End Function

End Class