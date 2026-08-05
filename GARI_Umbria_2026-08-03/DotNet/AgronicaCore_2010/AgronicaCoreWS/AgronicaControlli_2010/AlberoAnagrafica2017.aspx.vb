

Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.ComponentModel
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreUtility
Imports AgronicaCoreWinsortDAL
Imports AgronicaCoreWinsortBIZ
Imports AgronicaCoreContabDAL
Imports AgronicaCoreContabBIZ
Imports AgronicaCoreAnagrafeDAL
Imports Newtonsoft.Json
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaControlli_2010
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreModelloInSviluppo
Imports AgronicaCoreModelsSTD.Gis
Imports Agronica.Helpers.GiasBase
Imports AgronicaCoreUtentiDAL


Public Class AlberoAnagrafica2017
    Inherits System.Web.UI.Page


    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Shared Function CaricaAlberoCentriDropdown(piva As String, objP_server As String, objP_utenti As String)

        Dim r As New rispostaStandard(Of CentroDropdownItem())
        Try
            Dim utentiDBContext = Utility.convertStringtoOBJparametri(objP_utenti)
            Dim serverDBContext = Utility.convertStringtoOBJparametri(objP_server)

            Dim albero As New AgronicaControlli_2010.AlberoAnagrafica2017

            Dim dtAnagrafica = albero.CaricaAlberoCentriDropdown(piva, serverDBContext, insertDefault:=True)
            Dim items = dtAnagrafica.Rows.OfType(Of DataRow).Select(Function(s) New CentroDropdownItem With {.sa_nome = s("sa_nome"), .sa_cod = s("sa_cod")}).ToArray()

            r.RispostaStringa = items
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r
    End Function


    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Shared Function CaricaAlberoCentriDropdown_NG(InData As CoreWS_Generic(Of String))

        Dim r As New RispostaStandard
        Try
            Dim utentiDBContext = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)
            Dim serverDBContext = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            
            Dim leggiLingua As New Lingue_Read
            Dim dtLingua As DataTable = leggiLingua.Leggi_datoCODGIAS(serverDbContext.Lingua_Cod,
                                                                      AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                      "", "", utentiDbContext)
            Dim linguaCodiceISO As String = dtLingua.Rows(0)("CodiceISO")
            Threading.Thread.CurrentThread.CurrentUICulture = New Globalization.CultureInfo(linguaCodiceISO)


            Dim albero As New AgronicaControlli_2010.AlberoAnagrafica2017

            Dim dtAnagrafica = albero.CaricaAlberoCentriDropdown(InData.InData, serverDBContext, insertDefault:=True)
            Dim items = dtAnagrafica.Rows.OfType(Of DataRow).Select(Function(s) New CentroDropdownItem With {.sa_nome = s("sa_nome"), .sa_cod = s("sa_cod")}).ToArray()

            r.RispostaStringa = JsonConvert.SerializeObject(items)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r
    End Function


    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Shared Function GetNodesAlberoAnagrafe(
        ByVal cfgSerialized As String,
        ByVal id As String, ByVal PathRoot As String,
        ByVal objP_server As String, ByVal objP_utenti As String) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
        Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)

        Dim settingLoc As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}

        Dim cfg As ConfigurazioneAlbero = JsonConvert.DeserializeObject(Of ConfigurazioneAlbero)(cfgSerialized, settingLoc)

        Dim linkGiasBase As String = ""
        If PathRoot = "" Then
            GiasBaseHelper.Setta_Link_GiasBase(objParametri_Server, linkGiasBase)
            PathRoot = linkGiasBase + "agronica"
        End If

        Try

            Dim LetturaAlbero As New AgronicaControlli_2010.AlberoAnagrafica2017

            'SOSTITUISCO SOLO SE LE DATE SONO PIU' STRINGENTI DI QUELLE DELLA VISIBILITA' UTENTE (finestraTemporale)
            If objParametri_Server.FinestraTemporaleInizio <> AGRODATAINIZIO AndAlso
                cfg.dataInizio.ToLocalTime() > objParametri_Server.FinestraTemporaleInizio Then
                objParametri_Server.FinestraTemporaleInizio = cfg.dataInizio.ToLocalTime()
                objParametri_Utenti.FinestraTemporaleInizio = cfg.dataInizio.ToLocalTime()
            End If

            If objParametri_Server.FinestraTemporaleFine <> AGRODATAFINE AndAlso
                cfg.dataFine.ToLocalTime() < objParametri_Server.FinestraTemporaleFine Then
                objParametri_Server.FinestraTemporaleFine = cfg.dataFine.ToLocalTime()
                objParametri_Utenti.FinestraTemporaleFine = cfg.dataFine.ToLocalTime()
            End If

            r = LetturaAlbero.GetNodesAlberoAnagrafeGetJsonData(cfg, id, PathRoot, objParametri_Server, objParametri_Utenti)

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Shared Function GetNodesAlberoAnagrafeNG(InData As Object) As RispostaStandard

        Dim r As New RispostaStandard

        Dim a As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local, .NullValueHandling = NullValueHandling.Ignore}

        Dim iData As CoreWS_Generic(Of ParamsAlberoAnagrafe) = JsonConvert.DeserializeObject(Of CoreWS_Generic(Of ParamsAlberoAnagrafe))(JsonConvert.SerializeObject(InData), a)

        If iData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If iData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        If iData.objP.objP_super_server = "" Then
            r.Errore = "objP_super_server non valorizzato"
            Return r
        End If

        Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(iData.objP.objP_super_server)
        Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(iData.objP.objP_server)
        Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(iData.objP.objP_utenti)

        Dim settingLoc As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}

        Dim cfg As ConfigurazioneAlbero = JsonConvert.DeserializeObject(Of ConfigurazioneAlbero)(iData.InData.cfgSerialized, settingLoc)

        Dim linkGiasBase As String = ""
        If iData.InData.PathRoot = "" Then
            GiasBaseHelper.Setta_Link_GiasBase(objParametri_Server, linkGiasBase)
            iData.InData.PathRoot = linkGiasBase + "agronica"
        End If

        Try
            Dim leggiLingua As New Lingue_Read
            Dim dtLingua As DataTable = leggiLingua.Leggi_datoCODGIAS(objParametri_Server.Lingua_Cod,
                                                                      AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                      "", "", objParametri_Utenti)
            Dim linguaCodiceISO As String = dtLingua.Rows(0)("CodiceISO")
            Threading.Thread.CurrentThread.CurrentUICulture = New Globalization.CultureInfo(linguaCodiceISO)

            Dim LetturaAlbero As New AgronicaControlli_2010.AlberoAnagrafica2017

            'Sostituisco le date solo se sono più stringenti di quelle della visibilità utente (finestraTemporale)

            If cfg.dataInizio <> AGRODATAINIZIO AndAlso cfg.dataInizio.ToLocalTime() > objParametri_Server.FinestraTemporaleInizio Then
                objParametri_Server.FinestraTemporaleInizio = cfg.dataInizio.ToLocalTime()
                objParametri_Utenti.FinestraTemporaleInizio = cfg.dataInizio.ToLocalTime()
            End If

            If cfg.dataFine <> AGRODATAFINE AndAlso cfg.dataFine.ToLocalTime() < objParametri_Server.FinestraTemporaleFine Then
                objParametri_Server.FinestraTemporaleFine = cfg.dataFine.ToLocalTime()
                objParametri_Utenti.FinestraTemporaleFine = cfg.dataFine.ToLocalTime()
            End If

            'Lettura albero

            r = LetturaAlbero.GetNodesAlberoAnagrafeGetJsonData(cfg, iData.InData.id, iData.InData.PathRoot, objParametri_Server, objParametri_Utenti)

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r
    End Function

    Protected Class ParamsAlberoAnagrafe
        Public cfgSerialized As String
        Public id As String
        Public PathRoot As String
    End Class

End Class