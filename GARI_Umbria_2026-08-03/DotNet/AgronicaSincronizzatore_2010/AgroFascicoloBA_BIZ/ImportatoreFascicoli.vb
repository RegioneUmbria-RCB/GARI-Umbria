Imports System.Security.Cryptography
Imports System.Text
Imports System.IO
Imports Sincro_Agrea2Gias.MyWsAgriRER
Imports Sincro_Agrea2Gias
Imports System.Web.Services

Imports System.Xml
Imports System.Xml.Serialization
Imports System.Runtime.Serialization
Imports System.Configuration
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgroFascicoloBA_BIZ.localhost
Imports System.Web
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO
Imports Importazione_Agrea_WS
Imports AgronicaCoreDataProvider

Public Class ImportatoreFascicoli

    Private ObjParametri_SuperServer As AgronicaCoreDataProvider.AgronicaCoreParametri
    Private ObjParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri
    Private ObjParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri


    Private objLog As AgronicaCoreDataProvider.LogProvider
    Private customLOGParams As CustomLOGParams
    Private Configurazione_Servizi_R As AgronicaCoreVarieDAL.Configurazione_Servizi_R

    Private LogFileName As String
    Private LogDirectory As String
    Private LogDescrizioneUtente As String
    Private DirectoryFileImportazioni As String
    Private DirectoryFileEsportazioni As String
    Private ParametriExtra As String

    Private username As String
    Private password As String

    Private Gias_Fascicoli As String
    Private AnniDaImportare As List(Of Integer)

    Sub New(ByVal _Configurazione_Servizio As AgronicaCoreVarieDAL.Configurazione_Servizio, ByVal _ObjParametri_SuperServer As AgronicaCoreDataProvider.AgronicaCoreParametri, ByVal _ObjParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri, ByVal _ObjParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri)
        Me.ObjParametri_SuperServer = _ObjParametri_SuperServer
        Me.ObjParametri_Server = _ObjParametri_Server
        Me.ObjParametri_Utenti = _ObjParametri_Utenti

        InizializzoOggettiCore()

        ImpostoGliAltriParametri(_Configurazione_Servizio)

        impostaParametriExtra()

    End Sub

    Public Function avviaImportazione(ByRef returnStr As String) As Boolean
        Dim returnBool = True
        'importaFascicoli()
        'importaPianiColturali()
        'importaUnitaVitate()
        importaFascicoliProg()
        importaPianiColturaliProg()
        recuperaPianiColturaliErrati()
        recuperaFascicoliErrati()
        Return returnBool
    End Function

    Private Function importaFascicoli() As Boolean
        Dim returnBool = True
        Dim reader As New Fascicolo_R
        Dim writer As New Fascicolo_W
        Dim fascicoli = reader.leggiCuaaImportazioneMassiva(ObjParametri_Server, 0, 0, 1)
        For Each fascicolo As DataRow In fascicoli.Rows
            Try
                logga("Importo fascicolo:" & fascicolo.Item("cuaa"))
                Dim ws_FascicoloA As New SincroAnagrafeBA.FascicoloSiar2Response
                Dim ws_FascicoloColdi As New SincroAnagrafeBA.FascicoloWS_FascicoloHttpService(Gias_Fascicoli)
                Dim gF As New SincroAnagrafeBA.getFascicoloNew
                gF.cuaa = fascicolo.Item("cuaa")
                gF.filtro = SincroAnagrafeBA.FiltroGetFascicolo.anagrafe
                gF.numSchedaValidaz = "1"
                gF.Username = username
                gF.Password = password
                gF.OPR = ""
                ws_FascicoloColdi.SoapVersion = Protocols.SoapProtocolVersion.Soap12
                Dim FascicoloSiar As New FascicoloSiar2Response
                Dim response = ws_FascicoloColdi.getFascicoloNew(gF)
                AgronicaCoreUtility.XMLUtility.getObjectFromResponse(response, ws_FascicoloA, , False)
                Dim returnStr As String = ""
                If ws_FascicoloA.messaggioRisposta.cod = "000" Then
                    SincroAnagrafeBA.FascicoloSiar2Response2XMLPrivato.Convert2EntityFrameworkModel(ws_FascicoloA, ObjParametri_Server, ObjParametri_Utenti, False, returnStr)
                End If
                logga(returnStr)
                writer.FascicoloImportato(fascicolo.Item("cuaa"), 1, ObjParametri_Server)
            Catch ex As Exception
                logga("ERRORE: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, "True"))
                writer.scriviErrore("__T_FascicoliDaCaricare", CStr(fascicolo.Item("CUAA")),
                                    -1,
                                    ObjParametri_Server)
                writer.FascicoloImportato(fascicolo.Item("cuaa"), -1, ObjParametri_Server)
                'returnStr &= "ERRORE:" & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, "True")
                returnBool = False
            End Try
        Next
        returnBool = True

        Return returnBool
    End Function

    Private Function importaUnitaVitate() As Boolean
        logga("IMPORTA UNITA' VITATE")
        Dim returnBool = True
        Dim reader As New Fascicolo_R
        Dim writer As New Fascicolo_W
        Dim fascicoli = reader.leggiCuaaImportazioneMassiva(ObjParametri_Server, 0, 0, 1)
        Dim returnStr As String = ""
        For Each fascicolo As DataRow In fascicoli.Rows
            Try
                logga("Importo fascicolo:" & fascicolo.Item("cuaa"))
                Dim ws_FascicoloA As New SincroAnagrafeBA.FascicoloSiar2Response
                Dim ws_FascicoloColdi As New SincroAnagrafeBA.FascicoloWS_FascicoloHttpService(Gias_Fascicoli)
                Dim gF As New SincroAnagrafeBA.getFascicoloNew
                gF.cuaa = fascicolo.Item("cuaa")
                gF.filtro = SincroAnagrafeBA.FiltroGetFascicolo.anagrafe
                gF.numSchedaValidaz = "1"
                gF.Username = username
                gF.Password = password
                gF.OPR = ""
                ws_FascicoloColdi.SoapVersion = Protocols.SoapProtocolVersion.Soap12
                Dim FascicoloSiar As New FascicoloSiar2Response
                Dim response = ws_FascicoloColdi.getFascicoloNew(gF)
                AgronicaCoreUtility.XMLUtility.getObjectFromResponse(response, ws_FascicoloA, , False)
                If ws_FascicoloA.messaggioRisposta.cod = "000" Then
                    SincroAnagrafeBA.FascicoloSiar2Response2XMLPrivato.ImportaUnitaVitate(ws_FascicoloA, ObjParametri_Server, ObjParametri_Utenti, False, returnStr)
                End If
                writer.FascicoloImportato(fascicolo.Item("cuaa"), 1, ObjParametri_Server)
            Catch ex As Exception
                logga("ERRORE: " & returnStr & vbCrLf & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, "True"))
                writer.scriviErrore("__T_FascicoliDaCaricare", CStr(fascicolo.Item("CUAA")),
                                    -1,
                                    ObjParametri_Server)
                writer.FascicoloImportato(fascicolo.Item("cuaa"), -1, ObjParametri_Server)
                'returnStr &= "ERRORE:" & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, "True")
                returnBool = False
            End Try
        Next
        returnBool = True

        Return returnBool
    End Function

    Private Sub importaPianiColturali()
        Dim OprFascicoloAGEA As New ISWSToOprResponse
        Dim returnBool = True
        Dim reader As New Fascicolo_R
        'Dim impreseReader As New AgronicaCoreAnagrafeDAL.Imprese_Codici_Read
        Dim writer As New Fascicolo_W
        Dim anno As Integer = 2015
        'Dim fascicoli = reader.leggiCuaaImportazioneMassiva(ObjParametri_Server)
        Dim listaCuaa As New List(Of String)
        Dim dt As DataTable = reader.leggiPianoColturaleMassivo2016(ObjParametri_Server)
        For Each dr As DataRow In dt.Rows
            listaCuaa.Add(dr.Item("CUAA"))
        Next
        For Each fascicolo As String In listaCuaa
            Try
                logga("Importo fascicolo:" & fascicolo)
                'If impreseReader.Piva_from_IdCodValCod(1010, fascicolo, ObjParametri_Server) <> "" Then
                Dim myISWSResponse As pc.common.webservice.sop.agrea.it.ISWSResponse
                Dim ws_FascicoloColdi As New SincroAnagrafeBA.FascicoloWS_FascicoloHttpService(Gias_Fascicoli)
                Dim gF As New SincroAnagrafeBA.getFascicoloNew
                gF.cuaa = fascicolo
                gF.filtro = SincroAnagrafeBA.FiltroGetFascicolo.PianoColturale
                gF.numSchedaValidaz = ""
                gF.OPR = ""
                gF.Username = username
                gF.Password = password
                gF.Parametri_Extra = CStr(anno)
                ws_FascicoloColdi.SoapVersion = Protocols.SoapProtocolVersion.Soap12
                Dim FascicoloSiar As New FascicoloSiar2Response
                Dim response = ws_FascicoloColdi.getFascicoloNew(gF)
                If response <> "" Then
                    AgronicaCoreUtility.XMLUtility.getObjectFromResponse(response, myISWSResponse, , False)
                    importaISWS(myISWSResponse, fascicolo, CStr(anno), False)
                    'writer.FascicoloImportato(fascicolo, 1, ObjParametri_Server)
                Else
                    Throw New Exception("Errore durante l'import di " & fascicolo & " anno " & CStr(anno) & ": Impresa non Presente nel DB")
                End If
                'End If
            Catch ex As Exception
                logga("ERRORE: cuaa:" & fascicolo & " - " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, "True"))
                writer.scriviErrore("__T_PianiColturali2016", fascicolo, -1, ObjParametri_Server)
                'returnStr &= "ERRORE:" & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, "True")
                returnBool = False
            End Try
        Next
        returnBool = True
    End Sub

    Private Function importaFascicoliProg() As Boolean
        aziendeModificate(1)
        Dim returnBool = True
        Dim reader As New Fascicolo_R
        Dim writer As New Fascicolo_W
        'Cancello record in AggiornaFascicoli
        Dim dataMax = CDate(reader.maxDataAggiornaFascicoli(ObjParametri_Server, 1).Rows(0)(0))
        writer.eliminaAggiornaFascicoli(ObjParametri_Server, 1, 1, dataMax)
        For Each Cuaa As String In reader.getFascicolidaCaricare(-1, ObjParametri_Server, 1)
            Dim FascicoliInseriti As Integer = 0
            Dim errori As Integer = 0
            Try
                Dim dt2 = reader.leggiImportaFascicoli(Now.Date, 1, ObjParametri_Server)
                'logga("Importo:" & Cuaa)
                'Scarico Fascicolo
                Try
                    Dim myFascicoloSiarResponse As New SincroAnagrafeBA.FascicoloSiar2Response

                    Dim ws_FascicoloColdi As New SincroAnagrafeBA.FascicoloWS_FascicoloHttpService(Gias_Fascicoli)
                    Dim gF As New SincroAnagrafeBA.getFascicoloNew
                    gF.cuaa = Cuaa
                    gF.filtro = SincroAnagrafeBA.FiltroGetFascicolo.anagrafe
                    gF.Username = username
                    gF.Password = password
                    ws_FascicoloColdi.SoapVersion = Protocols.SoapProtocolVersion.Soap12
                    Dim FascicoloSiar As New FascicoloSiar2Response
                    Dim response = ws_FascicoloColdi.getFascicoloNew(gF)
                    AgronicaCoreUtility.XMLUtility.getObjectFromResponse(response, myFascicoloSiarResponse, , False)
                    If myFascicoloSiarResponse.messaggioRisposta.cod = "000" Then
                        SincroAnagrafeBA.FascicoloSiar2Response2XMLPrivato.Convert2EntityFrameworkModel(myFascicoloSiarResponse, ObjParametri_Server, ObjParametri_Utenti, True)
                    End If
                    'Metto flag importato
                    writer.UpdateAggiornaFascicoli(ObjParametri_Server, 1, Cuaa, 1)
                    FascicoliInseriti += 1
                    'logga("ImportaFascicoliProg1   Importato Cuaa:" & Cuaa & " ")
                Catch ex As Exception
                    errori += 1
                    logga("ImportaFascicoliProg1   Errore nel Cuaa:" & Cuaa & " - " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, "True"))
                    writer.UpdateAggiornaFascicoli(ObjParametri_Server, 1, Cuaa, -1)
                    returnBool = False
                End Try
            Catch ex As Exception
                errori += 1
                logga("ImportaFascicoliProg2   Errore nel Cuaa:" & Cuaa & " - " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, "True"))
                writer.UpdateAggiornaFascicoli(ObjParametri_Server, 1, Cuaa, -1)
                writer.scriviErrore("__T_FascicoliDaCaricare",
                                                CStr(Cuaa),
                                                -1,
                                                ObjParametri_Server)
                returnBool = False
            End Try
            Dim dt = reader.leggiImportaFascicoli(Now.Date, 1, ObjParametri_Server)
            If dt.Rows.Count > 0 Then
                If Not (IsDBNull(dt.Rows(0).Item("numeroFascicoliCaricati"))) Then
                    FascicoliInseriti = FascicoliInseriti + CInt(dt.Rows(0).Item("numeroFascicoliCaricati"))
                End If
                If Not (IsDBNull(dt.Rows(0).Item("numeroErrori"))) Then
                    errori = errori + CInt(dt.Rows(0).Item("numeroErrori"))
                End If
            End If
            writer.ScriviImportaFascicoli(Now.Date, 1, FascicoliInseriti, errori, ObjParametri_Server)
        Next
        Return returnBool
    End Function

    Private Function importaPianiColturaliProg()
        aziendeModificate(2)
        Dim returnBool = True
        Dim reader As New Fascicolo_R
        Dim writer As New Fascicolo_W
        'Cancello record in AggiornaFascicoli
        Dim dataMax = CDate(reader.maxDataAggiornaFascicoli(ObjParametri_Server, 1).Rows(0)(0))
        writer.eliminaAggiornaFascicoli(ObjParametri_Server, 2, 1, dataMax)
        For Each Cuaa As String In reader.getFascicolidaCaricare(-1, ObjParametri_Server, 2)
            Dim FascicoliInseriti As Integer = 0
            Dim errori As Integer = 0
            Try
                'Scarico Fascicolo
                Try

                    Dim ws_FascicoloColdi As New SincroAnagrafeBA.FascicoloWS_FascicoloHttpService(Gias_Fascicoli)
                    For Each anno In AnniDaImportare
                        logga("Importo:" & Cuaa & " Anno:" & CStr(anno))
                        Dim myISWSResponse As New pc.common.webservice.sop.agrea.it.ISWSResponse
                        Dim gF As New SincroAnagrafeBA.getFascicoloNew
                        gF.cuaa = Cuaa
                        gF.Username = username
                        gF.Password = password
                        gF.filtro = SincroAnagrafeBA.FiltroGetFascicolo.PianoColturale
                        gF.Parametri_Extra = CStr(anno)
                        ws_FascicoloColdi.SoapVersion = Protocols.SoapProtocolVersion.Soap12
                        Dim response = ws_FascicoloColdi.getFascicoloNew(gF)
                        If response IsNot Nothing AndAlso response <> "" Then
                            AgronicaCoreUtility.XMLUtility.getObjectFromResponse(response, myISWSResponse, , False)
                            importaISWS(myISWSResponse, Cuaa, CStr(anno), False)
                        End If
                    Next
                    'Metto flag importato
                    writer.UpdateAggiornaFascicoli(ObjParametri_Server, 2, Cuaa, 1)
                    FascicoliInseriti += 1
                    logga("ImportaPianiColturaliProg1   Importato Cuaa:" & Cuaa & " ")
                Catch ex As Exception
                    errori += 1
                    logga("ImportaPianiColturaliProg1   Errore nel Cuaa:" & Cuaa & " - " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, "True"))
                    writer.UpdateAggiornaFascicoli(ObjParametri_Server, 2, Cuaa, -1)
                    returnBool = False
                End Try
            Catch ex As Exception
                errori += 1
                logga("ImportaPianiColturaliProg2   Errore nel Cuaa:" & Cuaa & " - " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, "True"))
                writer.scriviErrore("__T_FascicoliDaCaricare",
                                                CStr(Cuaa),
                                                -1,
                                                ObjParametri_Server)
                returnBool = False
            End Try
        Next
        Return returnBool
    End Function

    Private Function recuperaFascicoliErrati()
        Dim returnBool = True
        Dim reader As New Fascicolo_R
        Dim writer As New Fascicolo_W
        'Cancello record in AggiornaFascicoli
        logga("RECUPERA FASCICOLI ERRATI")
        Dim listaCuaa As New List(Of String)
        For Each row In reader.LeggiAggiornaFascicoli(ObjParametri_Server, 1, , , , -1, ).Rows
            listaCuaa.Add(row.item("Cuaa"))
        Next

        For Each Cuaa As String In listaCuaa
            Dim FascicoliInseriti As Integer = 0
            Dim errori As Integer = 0
            Try
                logga("Importo:" & Cuaa)
                'Scarico Fascicolo
                Try
                    Dim myFascicoloSiarResponse As New SincroAnagrafeBA.FascicoloSiar2Response

                    Dim ws_FascicoloColdi As New SincroAnagrafeBA.FascicoloWS_FascicoloHttpService(Gias_Fascicoli)
                    Dim gF As New SincroAnagrafeBA.getFascicoloNew
                    gF.cuaa = Cuaa
                    gF.filtro = SincroAnagrafeBA.FiltroGetFascicolo.anagrafe
                    gF.Username = username
                    gF.Password = password
                    ws_FascicoloColdi.SoapVersion = Protocols.SoapProtocolVersion.Soap12
                    Dim FascicoloSiar As New FascicoloSiar2Response
                    Dim response = ws_FascicoloColdi.getFascicoloNew(gF)
                    If response IsNot Nothing Then
                        AgronicaCoreUtility.XMLUtility.getObjectFromResponse(response, myFascicoloSiarResponse, , False)
                        If myFascicoloSiarResponse.messaggioRisposta.cod = "000" Then
                            SincroAnagrafeBA.FascicoloSiar2Response2XMLPrivato.Convert2EntityFrameworkModel(myFascicoloSiarResponse, ObjParametri_Server, ObjParametri_Utenti, False)
                        End If
                        'Metto flag importato
                        writer.UpdateAggiornaFascicoli(ObjParametri_Server, 1, Cuaa, 1)
                        FascicoliInseriti += 1
                        logga("RecuperaFascicoliErrati1   Importato Cuaa:" & Cuaa & " ")
                    Else
                        logga("fascicolo " & Cuaa & " non presente!")
                    End If
                Catch ex As Exception
                    errori += 1
                    logga("RecuperaFascicoliErrati1   Errore nel Cuaa:" & Cuaa & " - " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, "True"))
                    writer.UpdateAggiornaFascicoli(ObjParametri_Server, 1, Cuaa, -1)
                    returnBool = False
                End Try
            Catch ex As Exception
                errori += 1
                logga("RecuperaFascicoliErrati2   Errore nel Cuaa:" & Cuaa & " - " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, "True"))
                writer.UpdateAggiornaFascicoli(ObjParametri_Server, 1, Cuaa, -1)
                writer.scriviErrore("__T_FascicoliDaCaricare",
                                                CStr(Cuaa),
                                                -1,
                                                ObjParametri_Server)
                returnBool = False
            End Try
            Dim dt = reader.leggiImportaFascicoli(Now.Date, 1, ObjParametri_Server)
            If dt.Rows.Count > 0 Then
                If Not (IsDBNull(dt.Rows(0).Item("numeroFascicoliCaricati"))) Then
                    FascicoliInseriti = FascicoliInseriti + CInt(dt.Rows(0).Item("numeroFascicoliCaricati"))
                End If
                If Not (IsDBNull(dt.Rows(0).Item("numeroErrori"))) Then
                    errori = errori + CInt(dt.Rows(0).Item("numeroErrori"))
                End If
            End If
            writer.ScriviImportaFascicoli(Now.Date, 1, FascicoliInseriti, errori, ObjParametri_Server)
        Next
        Return returnBool
    End Function

    Private Function recuperaPianiColturaliErrati()
        Dim returnBool = True
        Dim reader As New Fascicolo_R
        Dim writer As New Fascicolo_W
        'Cancello record in AggiornaFascicoli
        logga("RECUPERA PIANI COLTURALI ERRATI")
        Dim listaCuaa As New List(Of String)
        For Each row In reader.LeggiAggiornaFascicoli(ObjParametri_Server, 2, , , , -1, ).Rows
            listaCuaa.Add(row.item("Cuaa"))
        Next
        For Each Cuaa As String In listaCuaa
            Dim FascicoliInseriti As Integer = 0
            Dim errori As Integer = 0
            Try
                'Scarico Fascicolo
                Try

                    Dim ws_FascicoloColdi As New SincroAnagrafeBA.FascicoloWS_FascicoloHttpService(Gias_Fascicoli)
                    For Each anno In AnniDaImportare
                        logga("Importo:" & Cuaa & " Anno:" & CStr(anno))
                        Dim myISWSResponse As pc.common.webservice.sop.agrea.it.ISWSResponse
                        Dim gF As New SincroAnagrafeBA.getFascicoloNew
                        gF.cuaa = Cuaa
                        gF.Username = username
                        gF.Password = password
                        gF.filtro = SincroAnagrafeBA.FiltroGetFascicolo.PianoColturale
                        gF.Parametri_Extra = CStr(anno)
                        ws_FascicoloColdi.SoapVersion = Protocols.SoapProtocolVersion.Soap12
                        Dim response = ws_FascicoloColdi.getFascicoloNew(gF)
                        If response IsNot Nothing AndAlso response <> "" Then
                            AgronicaCoreUtility.XMLUtility.getObjectFromResponse(response, myISWSResponse, , False)
                            importaISWS(myISWSResponse, Cuaa, CStr(anno), False)
                        End If
                        logga("RecuperaPianiColturaliErrati1   Anno:" & CStr(anno) & " Importato Cuaa:" & Cuaa & " ")
                    Next
                    'Metto flag importato
                    writer.UpdateAggiornaFascicoli(ObjParametri_Server, 2, Cuaa, 1)
                    FascicoliInseriti += 1
                Catch ex As Exception
                    errori += 1
                    logga("RecuperaPianiColturaliErrati1   Errore nel Cuaa:" & Cuaa & " - " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, "True"))
                    writer.UpdateAggiornaFascicoli(ObjParametri_Server, 2, Cuaa, -1)
                    returnBool = False
                End Try
            Catch ex As Exception
                errori += 1
                logga("RecuperaPianiColturaliErrati2   Errore nel Cuaa:" & Cuaa & " - " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, "True"))
                writer.scriviErrore("__T_FascicoliDaCaricare",
                                                CStr(Cuaa),
                                                -1,
                                                ObjParametri_Server)
                returnBool = False
            End Try
        Next
        Return returnBool
    End Function

    Private Sub aziendeModificate(ByRef EnteValidatore_Cod As Integer)
        Dim writer As New Fascicolo_W
        Dim result As String
        Dim dataModifica As DateTime? = getDataModifica(EnteValidatore_Cod)
        If dataModifica IsNot Nothing Then
            logga("Richiesta Aziende Modificate " & CStr(dataModifica))
            Dim ws_FascicoloA As New SincroAnagrafeBA.FascicoloSiar2Response
            Dim ws_FascicoloColdi As New SincroAnagrafeBA.FascicoloWS_FascicoloHttpService(Gias_Fascicoli)
            Dim gF As New SincroAnagrafeBA.getFascicoloNew
            gF.Data_Riferimento = dataModifica
            gF.Username = username
            gF.Password = password
            Select Case EnteValidatore_Cod
                Case 1
                    gF.filtro = SincroAnagrafeBA.FiltroGetFascicolo.anagrafe
                Case 2
                    gF.filtro = SincroAnagrafeBA.FiltroGetFascicolo.PianoColturale
            End Select
            ws_FascicoloColdi.SoapVersion = Protocols.SoapProtocolVersion.Soap12
            Dim FascicoloSiar As New FascicoloSiar2Response
            result = ws_FascicoloColdi.getFascicoloNew(gF)
            If result IsNot Nothing AndAlso result <> "" Then
                Dim listaCuaa = result.Split("|").ToList
                For Each cuaa In listaCuaa
                    If cuaa IsNot Nothing AndAlso cuaa <> "" Then
                        writer.ScriviAggiornaFascicoli(ObjParametri_Server, EnteValidatore_Cod, dataModifica, Now.Date, cuaa, 0)
                    End If
                Next
            End If
        End If
    End Sub

    Private Function getDataModifica(ByRef enteValidatore_cod As Integer) As DateTime?
        Dim reader As New Fascicolo_R
        Dim dataModifica As Date
        Dim dt As DataTable = reader.LeggiAggiornaFascicoli(ObjParametri_Server, enteValidatore_cod)
        If dt.Rows.Count > 0 Then
            Dim dt1 As DataTable = reader.LeggiAggiornaFascicoli(ObjParametri_Server, enteValidatore_cod, , , , 0)
            If dt1.Rows.Count = 0 Then
                dataModifica = CDate(reader.maxDataAggiornaFascicoli(ObjParametri_Server, enteValidatore_cod).Rows(0)(0))
                If dataModifica = Now.Date Then
                    Return Nothing
                End If
            Else
                Return Nothing
            End If
        Else
            dataModifica = New Date(2016, 12, 1)
        End If
        Return dataModifica
    End Function

    Private Sub importaISWS(ByRef myISWSResponse As pc.common.webservice.sop.agrea.it.ISWSResponse, fascicolo As String, anno As String, importaeRibalta As Boolean)
        'Dim writer As New Fascicolo_W
        If myISWSResponse.codRet = "012" Then
            Dim msgOK As String
            Dim Log_Errori As New StringBuilder
            Dim Log_Import As New StringBuilder
            If myISWSResponse IsNot Nothing AndAlso myISWSResponse.azienda IsNot Nothing Then

                Dim ObjSequenze As New AgronicaCoreDataProvider.Agro_Sequenze

                Dim Pianificazione_Des = "PC" & CStr(anno) & " Fascicolo:" & myISWSResponse.domanda.idDomanda & " Data:" & myISWSResponse.domanda.dataValidazione.ToString()
                Dim gefutils As New AgronicaCoreEntityFramework.Gias_EF_Utility

                Dim EFConnString As String = gefutils.GetEntityConnectionString(ObjParametri_Server.StringaConnessione)

                Dim dal As New AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities(EFConnString)
                dal.Database.Connection.Open()
                If (From pt As Programmazione_Testata In dal.Programmazione_Testata Where pt.Programmazione_Des = Pianificazione_Des).Count = 0 Then
                    Dim Programmazione_Cod As Integer = ObjSequenze.NuovoId_Tabella(
                                                                                   "Programmazione_Testata",
                                                                                    Nothing,
                                                                                    Nothing,
                                                                                    ObjParametri_Server)
                    IMPORTA_AGREA(myISWSResponse,
                                  False,
                                  True,
                                  Log_Errori,
                                  Log_Import,
                                  anno,
                                  Programmazione_Cod,
                                  importaeRibalta)

                    If Log_Errori.Length = 0 Then
                        If Log_Import.Length = 0 Then
                            msgOK = "L'impresa " & fascicolo & " è stata importata"
                            objLog.Scrivi_LOG(ObjParametri_Server,
                                              "",
                                              msgOK,
                                              CustomLOGParams:=customLOGParams)
                        Else
                            msgOK = "L'impresa " & fascicolo & " è stata importata! --> " & Log_Import.ToString
                            objLog.Scrivi_LOG(ObjParametri_Server,
                                              "",
                                              msgOK,
                                              CustomLOGParams:=customLOGParams)
                        End If
                        'writer.aggiornaPianoColturale2016Massivo(ObjParametri_Server, fascicolo, 1)
                    Else
                        msgOK = "L'impresa " & fascicolo & " NON è stata importata! --> " & Log_Errori.ToString
                        objLog.Scrivi_LOG(ObjParametri_Server,
                                          "",
                                          msgOK,
                                          CustomLOGParams:=customLOGParams)
                        Throw New Exception
                        'writer.aggiornaPianoColturale2016Massivo(ObjParametri_Server, fascicolo, -1)
                    End If

                Else
                    msgOK = "L'impresa " & fascicolo & " è già presente nell'archivio! --> " & myISWSResponse.msgRet
                    objLog.Scrivi_LOG(ObjParametri_Server,
                                      "",
                                      msgOK,
                                      CustomLOGParams:=customLOGParams)
                End If
            End If
        Else
            Dim msgOK = "L'impresa " & fascicolo & " Già Presente"
            objLog.Scrivi_LOG(ObjParametri_Server,
                              "",
                              msgOK,
                              CustomLOGParams:=customLOGParams)
        End If
    End Sub

    Public Sub IMPORTA_AGREA(ByRef myISWSResponse As pc.common.webservice.sop.agrea.it.ISWSResponse,
                        ByVal Flag_ImportAnagrafica As Boolean,
                        ByVal Flag_ImportPianificazione As Boolean,
                        ByRef Log_Errori As StringBuilder,
                        ByRef Log_Import As StringBuilder,
                        ByVal Anno As Integer,
                        Optional Programmazione_Cod As Integer = 0,
                        Optional importaERibalta As Boolean = False)

        Const NomeFunzione As String = "IMPORTA."

        Try

            Dim strXmlAnagrafe As String = String.Empty
            Dim strXmlPianificazione As String = String.Empty
            Dim TipoOperazione As enum_TipoOperazioneDB

            Dim Esiste_Impresa As Boolean
            Dim objImp As New AgronicaCoreAnagrafeDAL.Imprese_Read
            Dim Str_RisultatoMassivaImport As String = ""
            Dim Str_RisultatoMassivaErrori As String = ""
            Dim Str_RisultatoCUAA As String = ""
            Dim Azienda_InElaborazione As String = ""

            '-----------------------------------------
            '----  DEFINIZIONE UTENTE SCRITTURA ------
            '----------------------------------------
            Dim Utente_Username As String = ""
            Dim Utente_Password As String = ""

            Dim utentiCodici_R As New AgronicaCoreUtentiDAL.Utenti_CodiciGiasPRO_R
            Dim utenti_R As New AgronicaCoreUtentiDAL.Utenti_Read

            Dim ProgressivoGIAS As Integer = utentiCodici_R.ProgressivoGias_from_Superuser(ObjParametri_Utenti) 'confSitiReader.Leggi_Valore(16, "Sincro_Codice_Chiave_Cliente", "", "", ObjParametri_Server) 'Session("ASG_ProgressivoGIAS")


            Utente_Username = ObjParametri_Utenti.SuperUserUsername  'Session("ASG_Utente_Username")
            Utente_Password = utenti_R.Password_From_UserName(Utente_Username, ObjParametri_Utenti) 'Session("ASG_Utente_Password")

            If Not IsNothing(myISWSResponse) Then

                Azienda_InElaborazione = myISWSResponse.azienda.ragioneSociale & " (cuaa=" & myISWSResponse.azienda.cuaa & ")"
                If Flag_ImportAnagrafica Then

                    TipoOperazione = enum_TipoOperazioneDB.Modifica

                    Esiste_Impresa = objImp.VerificaEsistenza_PivaGIAS(myISWSResponse.azienda.partitaIva,
                                                                       ObjParametri_Server)
                    If Not Esiste_Impresa Then
                        TipoOperazione = enum_TipoOperazioneDB.Scrittura
                    End If

                    'TipoOperazione = enum_TipoOperazioneDB.Scrittura

                    strXmlAnagrafe = Xml_Genera_Stringone_Anagrafe(myISWSResponse,
                                                                   TipoOperazione,
                                                                   ObjParametri_Server,
                                                                   Utente_Username,
                                                                   Utente_Password,
                                                                   ProgressivoGIAS,
                                                                   enum_TipoImportazioneAnagrafe.Agrea_Excel,
                                                                   Log_Errori,
                                                                   Log_Import)
                End If

                TipoOperazione = enum_TipoOperazioneDB.Scrittura

                'da gestire il salvataggio del log
                Dim LogCodificheMancantiSpecie As String = ""
                Dim LogCodificheMancantiVarieta As String = ""
                If Flag_ImportPianificazione Then
                    strXmlPianificazione = Xml_Genera_Stringone_Pianificazione(myISWSResponse,
                                                                               TipoOperazione,
                                                                               ObjParametri_Server,
                                                                               ObjParametri_Utenti,
                                                                               Utente_Username,
                                                                               Utente_Password,
                                                                               ProgressivoGIAS,
                                                                               Anno,
                                                                               LogCodificheMancantiSpecie,
                                                                               LogCodificheMancantiVarieta,
                                                                               "",
                                                                               Programmazione_Cod)
                End If


                Dim Num_Azi_Sincronizzate As Integer = 0
                Scrivi_Dati(Log_Errori,
                            Log_Import,
                            strXmlAnagrafe,
                            strXmlPianificazione,
                            Str_RisultatoMassivaImport,
                            Str_RisultatoMassivaErrori,
                            Str_RisultatoCUAA,
                            Num_Azi_Sincronizzate,
                            Utente_Username,
                            Utente_Password,
                            ObjParametri_Server.PivaSuperUser,
                            Azienda_InElaborazione,
                            ObjParametri_Server,
                            ObjParametri_Utenti)


                If Str_RisultatoMassivaErrori <> "" Then
                    Log_Errori.Append(Str_RisultatoMassivaErrori & vbCrLf)
                Else
                    If importaERibalta AndAlso Programmazione_Cod <> 0 Then
                        Dim Ribalta_Limiti As Boolean = False
                        Try
                            Ribalta_Pianificazione(Programmazione_Cod, "", AGRODATAINIZIO, Nothing, Ribalta_Limiti, ProgressivoGIAS, Log_Errori)
                        Catch ex As Exception
                            Log_Errori.Append(ex.Message & vbCrLf)
                        End Try
                    End If
                End If

                Log_Import.Append(Str_RisultatoMassivaImport & vbCrLf)

            Else
                Log_Errori.Append(NomeFunzione & "Oggetto vuoto, impossibile importare l'azienda " & vbCrLf)
            End If


        Catch ex As Exception
            Log_Errori.Append(NomeFunzione & "Si è verificato il seguente errore: " & ex.Message & vbCrLf)
            Throw New Exception
        End Try

    End Sub

    Public Function Xml_Genera_Stringone_Anagrafe(ByRef myISWSResponse As pc.common.webservice.sop.agrea.it.ISWSResponse,
                                                   ByVal TipoOperazione As enum_TipoOperazioneDB,
                                                   ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                   ByVal Utente_Username As String,
                                                     ByVal Utente_Password As String,
                                                    ByVal ProgressivoGIAS As Integer,
                                                    ByVal TipoImport As enum_TipoImportazioneAnagrafe,
                                                      ByRef Log_Errori As StringBuilder,
                                                    ByRef Log_Import As StringBuilder, Optional Piva_Padre_Selected As String = "0", Optional Path As String = "") As String

        Dim NomeRoutine As String = "cls_ImportAgrea.Xml_Genera_Anagrafe(): "

        Dim XmlDoc As New XmlDocument
        ' Dim XmlUtente As XmlElement
        Dim XmlImpresa As XmlElement
        Dim XmlCentro As XmlElement
        Dim objXML As New AgronicaCoreXML.AnagrafeXML

        Dim Str_Xml As String = ""

        Try

            Dim CodiceChiaveCliente As Integer
            Dim confSitiReader As New AgronicaCoreVarieDAL.Configurazione_Siti_R
            CodiceChiaveCliente = confSitiReader.Leggi_Valore(16, "Sincro_Codice_Chiave_Cliente", "", "", objParametri_Server)

            ''-------------------------------
            ''----- IMPRESA
            ''-------------------------------
            Dim Piva As String = ""
            Dim Cuaa As String = ""

            Piva = myISWSResponse.azienda.partitaIva
            Cuaa = myISWSResponse.azienda.cuaa

            '06/05/2020: gestione piva estera, campo lungo ora 25
            If Piva = "" AndAlso Cuaa <> "" Then
                'se non ho la partita iva metto il CUAA tagliato a 11 cifre
                'Piva = Cuaa.Substring(0, 11)
                Piva = Cuaa
            End If

            Dim Piva_Padre As String
            If Piva_Padre_Selected = "0" Then
                Piva_Padre = Recupera_Piva_Padre(myISWSResponse.domanda.idCaa, Piva, objParametri_Server)
            Else
                Piva_Padre = Piva_Padre_Selected
            End If

            If Piva_Padre = Piva Then
                Piva_Padre = "#"
            End If

            Dim objSincroCatasto As New cls_SincronizzatoreCatasto

            '-------------------------------
            '----- IMPRESA - CENTRO - FABBRICATO
            '-------------------------------
            Dim SchedaValidazione As String = ""
            Dim DataValidazione As Date = AGRODATAINIZIO

            SchedaValidazione = CStr(myISWSResponse.domanda.idDomanda)
            If myISWSResponse.domanda.dataValidazione IsNot Nothing Then
                DataValidazione = CDate(myISWSResponse.domanda.dataValidazione)
            End If
            Dim Detentore_Fascicolo = myISWSResponse.domanda.descCaa

            objSincroCatasto.Gestione_Sincronizzazione_Anagrafica(
                                                                  objParametri_Server,
                                                                  Log_Errori,
                                                                  Log_Import,
                                                                  XmlDoc,
                                                                  XmlImpresa,
                                                                  XmlCentro,
                                                                  Utente_Username,
                                                                  Utente_Password,
                                                                  ProgressivoGIAS,
                                                                  TipoOperazione,
                                                                  CodiceChiaveCliente,
                                                                  myISWSResponse.azienda.idAzienda,
                                                                  Piva,
                                                                  Cuaa,
                                                                  Piva_Padre,
                                                                  myISWSResponse.azienda.ragioneSociale,
                                                                  myISWSResponse.azienda.indirizzo,
                                                                  myISWSResponse.azienda.cap,
                                                                  myISWSResponse.azienda.codIstatProv,
                                                                  myISWSResponse.azienda.codIstatCom,
                                                                  Path,
                                                                  SchedaValidazione,
                                                                  DataValidazione,
                                                                  Detentore_Fascicolo)


            '-------------------------------
            '----- CATASTO
            '-------------------------------
            'questi due parametri sono gestiti nell'import da anagrafe
            Dim Opt_Particelle_1Insert2Modifica As Integer = 1 'imposto di modificare le particelle esistenti e inserire quelle che mancano
            Dim HT_PartCentri As New Hashtable 'non è stato scelto da interfaccia il centro da impostare nelle nuove particelle

            Xml_Genera_Stringone_Catasto(myISWSResponse,
                                         TipoImport,
                                         TipoOperazione,
                                         objParametri_Server,
                                         objSincroCatasto,
                                         objXML,
                                         XmlDoc,
                                         XmlImpresa,
                                         XmlCentro,
                                         Opt_Particelle_1Insert2Modifica,
                                         HT_PartCentri,
                                         CodiceChiaveCliente,
                                         myISWSResponse.azienda.idAzienda,
                                         Piva,
                                         Log_Errori,
                                         Log_Import)


        Catch ex As Exception
            Throw New Exception(NomeRoutine & " " & ex.Message)
        End Try

        ' Return XmlUtente.OuterXml

        Str_Xml = XmlDoc.OuterXml

        XmlDoc = Nothing

        Return Str_Xml

    End Function

    Public Function Xml_Genera_Stringone_Pianificazione(ByRef myISWSResponse As pc.common.webservice.sop.agrea.it.ISWSResponse,
                                                        ByVal TipoOperazione As enum_TipoOperazioneDB,
                                                        ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                        ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                        ByVal Utente_Username As String,
                                                        ByVal Utente_Password As String,
                                                        ByVal ProgressivoGIAS As Integer,
                                                        ByVal ANNO As Integer,
                                                        ByRef LogCodificheMancantiSpecie As String,
                                                        ByRef LogCodificheMancantiVarieta As String, Optional NomeFile As String = "",
                                                        Optional Programmazione_Cod As Integer = 0)

        Dim XmlDoc As New System.Xml.XmlDocument

        Dim XmlUtente As System.Xml.XmlElement
        Dim XmlTestata As System.Xml.XmlElement
        Dim XmlEntita As System.Xml.XmlElement
        Dim XmlParticella As System.Xml.XmlElement
        Dim XmlFascicoloP As System.Xml.XmlElement

        Dim Piva As String = ""
        Dim Cuaa As String = ""

        Dim objCodificaZona As New AgronicaCoreAnagrafeDAL.CAC_Codifica_Zone
        Dim objCultivar As New AgronicaCoreMetaSchemaDAL.Cultivar_R
        Dim objImpresexParticelle As New AgronicaCoreAnagrafeDAL.ImpresexParticelle2_R
        Dim objXML As New AgronicaCoreXML.AnagrafeXML

        Dim SchedaValidazione As String = ""
        Dim DataValidazione As Date = AGRODATAINIZIO
        Dim DataAperturaFascicolo As Date = AGRODATAINIZIO
        Dim DataChiusuraFascicolo As Date = AGRODATAFINE
        Dim DataInizioMandato As Date = AGRODATAINIZIO
        Dim DataFineMandato As Date = AGRODATAFINE

        Dim CodiceChiaveCliente As Integer
        Dim confSitiReader As New AgronicaCoreVarieDAL.Configurazione_Siti_R
        CodiceChiaveCliente = confSitiReader.Leggi_Valore(16, "Sincro_Codice_Chiave_Cliente", "", "", objParametri_Server)


        XmlUtente = objXML.Xml_Pubblico_Utente(XmlDoc,
                                               Utente_Username,
                                               Utente_Password,
                                               ProgressivoGIAS)
        Dim strAnno As String = String.Empty
        Dim strValiditaInizio As String = String.Empty
        Dim strValiditaFine As String = String.Empty

        Dim objImpost As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
        Dim DataInizioUtente As Date = AGRODATAINIZIO
        Dim DataFineUtente As Date = AGRODATAFINE
        objImpost.AnnataAgraria(CDate("01/01/" & ANNO.ToString), DataInizioUtente, DataFineUtente, objParametri_Utenti)

        If IsNumeric(ANNO) AndAlso (CInt(ANNO) > 1900 AndAlso CInt(ANNO) < 2100) Then
            strAnno = CInt(ANNO).ToString
            strValiditaInizio = DataInizioUtente.ToShortDateString
            strValiditaFine = DataFineUtente.ToShortDateString
        Else
            strValiditaInizio = DataInizioUtente.ToShortDateString
            strValiditaFine = DataFineUtente.ToShortDateString
        End If

        Piva = myISWSResponse.azienda.partitaIva
        Cuaa = myISWSResponse.azienda.cuaa

        '06/05/2020: gestione piva estera, campo lungo ora 25
        If Piva = "" AndAlso Cuaa <> "" Then
            'se non ho la partita iva metto il CUAA tagliato a 11 cifre
            'Piva = Cuaa.Substring(0, 11)
            Piva = Cuaa
        End If

        Dim DtParticelle As New DataTable
        DtParticelle = objImpresexParticelle.Leggi(0,
                                                 Piva,
                                                 0, 0, "", "", "", 0, 0, "",
                                                 enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                 "", "", objParametri_Server)

        XmlTestata = objXML.Xml_Pubblico_ProgrammazioneTestata(TipoOperazione,
                                                               CStr(Programmazione_Cod),
                                                               Piva,
                                                               "PC" & strAnno & " Fascicolo:" & myISWSResponse.domanda.idDomanda & " Data:" & myISWSResponse.domanda.dataValidazione.ToString(),
                                                               "PC" & strAnno & " Fascicolo:" & myISWSResponse.domanda.idDomanda & " Data:" & myISWSResponse.domanda.dataValidazione.ToString(),
                                                               "Importazione da archivi AGREA " & strAnno,
                                                               enum_Planning_Fonte.Agrea,
                                                               enum_TipoPianificazione.Pianificazione_Annuale,
                                                               strValiditaInizio,
                                                               strValiditaFine,
                                                               XmlDoc)

        If myISWSResponse.domanda.idDomanda <> 0 Then
            SchedaValidazione = myISWSResponse.domanda.idDomanda
        End If
        If myISWSResponse.domanda.dataValidazione IsNot Nothing Then
            DataValidazione = CDate(myISWSResponse.domanda.dataValidazione)
        End If
        'If Not ws_fasciResponse_New.out.fascicolo.fascicolo.DataSottMandato Is Nothing Then
        '    DataInizioMandato = CDate(AgronicaCoreDataProvider.Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(ws_fasciResponse_New.out.fascicolo.fascicolo.DataSottMandato))
        'End If
        'If Not ws_fasciResponse_New.out.fascicolo.fascicolo.DataAperturaFascicolo Is Nothing Then
        '    DataAperturaFascicolo = CDate(AgronicaCoreDataProvider.Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(ws_fasciResponse_New.out.fascicolo.fascicolo.DataAperturaFascicolo))
        'End If
        'If Not ws_fasciResponse_New.out.fascicolo.fascicolo.DataChiusuraFascicolo Is Nothing Then
        '    DataChiusuraFascicolo = CDate(AgronicaCoreDataProvider.Conversioni.DataGGMMYYYY_From_DataYYYYMMGG(ws_fasciResponse_New.out.fascicolo.fascicolo.DataChiusuraFascicolo))
        'End If

        Dim Detentore_Fascicolo As String = ""
        If myISWSResponse.domanda.descCaa IsNot Nothing Then
            Detentore_Fascicolo = myISWSResponse.domanda.descCaa
        End If

        XmlFascicoloP = objXML.Xml_Pubblico_Fascicolo(NomeFile,
                                                      SchedaValidazione,
                                                      DataValidazione,
                                                      Detentore_Fascicolo,
                                                      DataInizioMandato,
                                                      DataFineMandato,
                                                      DataAperturaFascicolo,
                                                      DataChiusuraFascicolo,
                                                      XmlDoc)

        XmlTestata.AppendChild(XmlFascicoloP)



        Dim SaCod As Integer = 0
        Dim SaNome As String = ""

        SaCod = New AgronicaCoreAnagrafeDAL.Centri_Codici_Read().RecuperaSaCodImpresaByIdAziendaFascicolo(
                                    CodiceChiaveCliente,
                                    Piva,
                                    myISWSResponse.azienda.idAzienda,
                                    SaNome,
                                    objParametri_Server)


        Dim i, j, k As Integer

        Dim strSezione As String = String.Empty
        Dim strSubalterno As String = String.Empty

        Dim objUtilizzi As New AgronicaCoreMetaSchemaDAL.Codifica_SpecieVegetali_Agea_2015_2020_R

        Dim Veg_Cod As Integer
        Dim Veg_Cod_CodificaCultivar As Integer
        Dim Cul_Cod As Integer
        Dim Grfi_Cod As Integer
        Dim Id_Cod As Integer
        Dim Grva_Cod As Integer
        Dim Cul_Cod_Cliente As String
        Dim Veg_Cod_Cliente As String
        Dim Macrouso_Cod As String

        Dim Veg_Cod_Agea As String
        Dim Cul_Cod_Agea As String
        Dim Uso_Cod_Agea As String
        Dim Occupazione_Cod_Agea As String
        Dim Destinazione_Cod_Agea As String
        Dim Qualita_Cod_Agea As String

        Dim nEntita As Integer = 1

        If Not IsNothing(myISWSResponse.possessi) Then

            Dim vett_str As New List(Of String)
            Dim FiltraParticelle As Boolean = False
            'vett_str.Add("038 005  47 24 000")
            'vett_str.Add("038 005  47 36 000")
            'If Not HttpContext.Current.Session("objImpiantiTot") Is Nothing Then
            '    vett_str = HttpContext.Current.Session("objImpiantiTot")
            '    FiltraParticelle = True
            'End If

            For i = 0 To myISWSResponse.possessi.Count - 1

                If IsNothing(myISWSResponse.possessi(i).sezione) OrElse myISWSResponse.possessi(i).sezione = "" Then
                    strSezione = "0"
                Else
                    strSezione = myISWSResponse.possessi(i).sezione
                End If

                If IsNothing(myISWSResponse.possessi(i).subalterno) OrElse myISWSResponse.possessi(i).subalterno = "" _
                   OrElse myISWSResponse.possessi(i).subalterno = "000" Then
                    strSubalterno = "0"
                Else
                    strSubalterno = myISWSResponse.possessi(i).subalterno
                End If

                If Not IsNothing(myISWSResponse.possessi(i).macrousi) Then

                    For j = 0 To myISWSResponse.possessi(i).macrousi.Count - 1

                        If Not IsNothing(myISWSResponse.possessi(i).macrousi(j).utilizzi) Then

                            For k = 0 To myISWSResponse.possessi(i).macrousi(j).utilizzi.Count - 1

                                Veg_Cod = 0
                                Veg_Cod_CodificaCultivar = 0
                                Cul_Cod = 0
                                Grfi_Cod = 0
                                Id_Cod = 0
                                Grva_Cod = 0

                                Veg_Cod_Agea = ""
                                Cul_Cod_Agea = ""
                                Uso_Cod_Agea = ""
                                Occupazione_Cod_Agea = ""
                                Destinazione_Cod_Agea = ""
                                Qualita_Cod_Agea = ""

                                Uso_Cod_Agea = ""
                                Occupazione_Cod_Agea = ""
                                Destinazione_Cod_Agea = ""
                                Qualita_Cod_Agea = ""

                                Cul_Cod_Cliente = myISWSResponse.possessi(i).macrousi(j).utilizzi(k).codVarieta
                                Macrouso_Cod = myISWSResponse.possessi(i).macrousi(j).codMacrouso

                                If ANNO <= 2014 Then
                                    Veg_Cod_Cliente = Strings.Right("000" & myISWSResponse.possessi(i).macrousi(j).utilizzi(k).codColtura, 3)
                                    Dim objUtilizziAgr As New AgronicaCoreMetaSchemaDAL.Codifica_SpecieVegetali_Agea_R
                                    objUtilizziAgr.Specie_e_Varieta_Gias_Da_Agea(LogCodificheMancantiSpecie,
                                                                              LogCodificheMancantiVarieta,
                                                                              Veg_Cod_Cliente, Cul_Cod_Cliente,
                                                                              Veg_Cod, Cul_Cod, Grfi_Cod, Grva_Cod, Id_Cod,
                                                                              "", "",
                                                                              myISWSResponse.possessi(i).macrousi(j).utilizzi(k).descColtura,
                                                                              myISWSResponse.possessi(i).macrousi(j).utilizzi(k).descVarieta,
                                                                              CDate(strValiditaInizio),
                                                                              Uso_Cod_Agea,
                                                                              Occupazione_Cod_Agea,
                                                                              Destinazione_Cod_Agea,
                                                                              Qualita_Cod_Agea,
                                                                              objParametri_Server)
                                Else
                                    Dim cod_ColturaArr = myISWSResponse.possessi(i).macrousi(j).utilizzi(k).codColtura.Split("-")
                                    Veg_Cod_Agea = ""
                                    Cul_Cod_Agea = cod_ColturaArr(4)
                                    Uso_Cod_Agea = cod_ColturaArr(0)
                                    Occupazione_Cod_Agea = cod_ColturaArr(1)
                                    Destinazione_Cod_Agea = cod_ColturaArr(2)
                                    Qualita_Cod_Agea = cod_ColturaArr(3)

                                    If Cul_Cod_Agea = "000" AndAlso myISWSResponse.possessi(i).macrousi(j).utilizzi(k).codVarieta <> "000" Then
                                        Cul_Cod_Agea = myISWSResponse.possessi(i).macrousi(j).utilizzi(k).codVarieta
                                    End If

                                    objUtilizzi.Specie_e_Varieta_Gias_Da_AGEA(LogCodificheMancantiSpecie,
                                                          LogCodificheMancantiVarieta,
                                                          Veg_Cod_Agea,
                                                          Cul_Cod_Agea,
                                                          Uso_Cod_Agea,
                                                          Occupazione_Cod_Agea,
                                                          Destinazione_Cod_Agea,
                                                          Qualita_Cod_Agea,
                                                          Veg_Cod, Cul_Cod,
                                                          Grfi_Cod,
                                                          Grva_Cod,
                                                          Id_Cod,
                                                          "",
                                                          "",
                                                          objParametri_Server,
                                                          "",
                                                          "")
                                    'Veg_Cod_Cliente = myISWSResponse.possessi(i).macrousi(j).utilizzi(k).codColtura
                                    'Dim objUtilizzi As New AgronicaCoreMetaSchemaDAL.Codifica_SpecieVegetali_Agrea_R
                                    'objUtilizzi.Specie_e_Varieta_Gias_Da_Agrea(LogCodificheMancantiSpecie,
                                    '                                           LogCodificheMancantiVarieta,
                                    '                                           Veg_Cod_Cliente,
                                    '                                           Cul_Cod_Cliente,
                                    '                                           Veg_Cod_Agea,
                                    '                                           Cul_Cod_Agea,
                                    '                                           Veg_Cod,
                                    '                                           Cul_Cod,
                                    '                                           Grfi_Cod,
                                    '                                           Grva_Cod,
                                    '                                           Id_Cod,
                                    '                                           "",
                                    '                                           "",
                                    '                                           myISWSResponse.possessi(i).macrousi(j).utilizzi(k).descColtura,
                                    '                                           myISWSResponse.possessi(i).macrousi(j).utilizzi(k).descVarieta,
                                    '                                           Uso_Cod_Agea,
                                    '                                           Occupazione_Cod_Agea,
                                    '                                           Destinazione_Cod_Agea,
                                    '                                           Qualita_Cod_Agea,
                                    '                                           objParametri_Server)
                                End If

                                If Veg_Cod = 0 Then
                                    Veg_Cod = Veg_Cod_CodificaCultivar
                                End If

                                If CInt(Veg_Cod) <> 0 AndAlso CInt(Cul_Cod) = 0 Then
                                    Cul_Cod = objCultivar.VarietaAltre(Veg_Cod, objParametri_Server)
                                End If

                                Dim z As Integer
                                Dim strZona As String = "n"
                                Dim CodiceZonaGIAS As Integer

                                If Not IsNothing(myISWSResponse.possessi(i).codZone) Then

                                    For z = 0 To myISWSResponse.possessi(i).codZone.Count - 1
                                        CodiceZonaGIAS = objCodificaZona.ConvertiZona(myISWSResponse.possessi(i).codZone(z),
                                                                                      "", objParametri_Server)

                                        If CodiceZonaGIAS = "-17" Then
                                            strZona = "v"
                                            Exit For
                                        End If
                                    Next

                                End If

                                Dim flagIrrigabilita As String = "0"
                                If Not IsNothing(myISWSResponse.possessi(i).flagIrrigabilita) AndAlso LCase(myISWSResponse.possessi(i).flagIrrigabilita) = "s" Then
                                    flagIrrigabilita = "1"
                                End If

                                Dim flagSecondoRaccolto As String = "0"
                                If Not IsNothing(myISWSResponse.possessi(i).flagSecondoRaccolto) AndAlso LCase(myISWSResponse.possessi(i).flagSecondoRaccolto) = "s" Then
                                    flagSecondoRaccolto = "1"
                                End If

                                Dim Prov As String = myISWSResponse.possessi(i).codProv
                                Dim Com As String = myISWSResponse.possessi(i).codCom
                                Dim Sezione As String = strSezione
                                Dim Foglio As Integer = myISWSResponse.possessi(i).foglio
                                Dim Numero As Integer = myISWSResponse.possessi(i).particella
                                Dim Subalterno As String = strSubalterno

                                '(29/04/2015 fede) verifico in quale centro è la particella
                                If DtParticelle IsNot Nothing AndAlso DtParticelle.Rows.Count > 0 Then
                                    Dim DrParticella() As DataRow = DtParticelle.Select("PROV='" & Prov & "' AND COM='" & Com & "' AND Sezione='" & Sezione & "' AND foglio=" & Foglio.ToString & " AND Numero=" & Numero.ToString & " AND subalterno='" & Subalterno & "'")
                                    If DrParticella IsNot Nothing AndAlso DrParticella.Length > 0 Then
                                        SaCod = DrParticella(0).Item("sa_cod")
                                    End If
                                End If

                                '(07/11/2016 fede) controllo selezione utente
                                Dim lPart As String = myISWSResponse.possessi(i).codProv & " " & myISWSResponse.possessi(i).codCom & " " &
                                    myISWSResponse.possessi(i).sezione & " " & myISWSResponse.possessi(i).foglio & " " &
                                    myISWSResponse.possessi(i).particella & " " & myISWSResponse.possessi(i).subalterno & " " &
                                    myISWSResponse.possessi(i).macrousi(j).utilizzi(k).codColtura & " " & myISWSResponse.possessi(i).macrousi(j).utilizzi(k).codVarieta & " " &
                                    myISWSResponse.possessi(i).macrousi(j).utilizzi(k).supUtilizzo.ToString

                                If Not FiltraParticelle OrElse vett_str.Contains(lPart) Then


                                    XmlEntita = objXML.Xml_Pubblico_ProgrammazioneEntita(TipoOperazione,
                                                                                         "0",
                                                                                         "App. " & Right("000" & nEntita, 3),
                                                                                         SaCod,
                                                                                         "#",
                                                                                         -nEntita,
                                                                                          "0", "0", "Lotto" & strAnno,
                                                                                          Veg_Cod,
                                                                                          Grfi_Cod,
                                                                                          Cul_Cod,
                                                                                          Grva_Cod,
                                                                                          Veg_Cod_Cliente, Cul_Cod_Cliente, "#",
                                                                                          Macrouso_Cod,
                                                                                          Id_Cod, "#",
                                                                                          myISWSResponse.possessi(i).macrousi(j).utilizzi(k).supUtilizzo,
                                                                                          "#",
                                                                                          strZona,
                                                                                          "#", "#", "#", "#", "#", "#", "#", "#", "#", "#",
                                                                                          strValiditaInizio,
                                                                                          strValiditaFine,
                                                                                          strValiditaInizio,
                                                                                          XmlDoc,
                                                                                          FlagIrrigabilita:=flagIrrigabilita,
                                                                                          FlagSecondoRaccolto:=flagSecondoRaccolto,
                                                                                          Veg_Cod_Agea:=Veg_Cod_Agea,
                                                                                          Cul_Cod_Agea:=Cul_Cod_Agea,
                                                                                          Uso_Cod_Agea:=Uso_Cod_Agea,
                                                                                          Occupazione_Cod_Agea:=Occupazione_Cod_Agea,
                                                                                          Destinazione_Cod_Agea:=Destinazione_Cod_Agea,
                                                                                          Qualita_Cod_Agea:=Qualita_Cod_Agea)

                                    XmlParticella = objXML.Xml_Pubblico_ProgrammazioneParticella(TipoOperazione,
                                                                                                 myISWSResponse.possessi(i).codCom,
                                                                                                 myISWSResponse.possessi(i).codProv,
                                                                                                 strSezione,
                                                                                                 myISWSResponse.possessi(i).foglio,
                                                                                                 myISWSResponse.possessi(i).particella,
                                                                                                 strSubalterno,
                                                                                                 myISWSResponse.possessi(i).macrousi(j).utilizzi(k).supUtilizzo,
                                                                                                 "#", "#",
                                                                                                 XmlDoc)

                                    nEntita += 1

                                    XmlEntita.AppendChild(XmlParticella)
                                    XmlTestata.AppendChild(XmlEntita)

                                End If

                            Next

                        End If 'utilizzi

                    Next

                End If 'macrousi

            Next

        End If 'possessi

        XmlUtente.AppendChild(XmlTestata)

        Return XmlUtente.OuterXml

    End Function

    Private Sub Xml_Genera_Stringone_Catasto(ByRef myISWSResponse As pc.common.webservice.sop.agrea.it.ISWSResponse,
                                             ByVal TipoImport As enum_TipoImportazioneAnagrafe,
                                                ByVal TipoOperazione As enum_TipoOperazioneDB,
                                                ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                ByRef objSincroCatasto As cls_SincronizzatoreCatasto,
                                                ByRef objXML As AgronicaCoreXML.AnagrafeXML,
                                                ByRef XmlDoc As XmlDocument,
                                                ByRef XMLImpresa As XmlElement,
                                                ByRef XMLCentro As XmlElement,
                                                ByVal Opt_Particelle_1Insert2Modifica As Integer,
                                                ByVal HT_PartCentri As Hashtable,
                                                ByVal CodiceChiaveCliente As Integer,
                                                ByVal ValCod_CodiceChiaveCliente As String,
                                                ByVal Piva As String,
                                                   ByRef Log_Errori As StringBuilder,
                                                    ByRef Log_Import As StringBuilder)

        Dim NomeRoutine As String = "cls_ImportAgrea.Xml_Genera_Catasto(): "

        Dim objCodificaZona As New AgronicaCoreAnagrafeDAL.CAC_Codifica_Zone
        Dim objZonexParticelle As New AgronicaCoreAnagrafeDAL.ZonexParticelle_R
        Dim objMacrousixParticelle As New AgronicaCoreAnagrafeDAL.ParticelleCatastalixMacrousi_R
        Dim objPartxMacrousixUtilizzi As New AgronicaCoreAnagrafeDAL.ParticelleCatastalixMacrousixUtilizzo_R
        Dim objImpresexParticelle As New AgronicaCoreAnagrafeDAL.ImpresexParticelle2_R
        Dim objCentri As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read


        Dim Str_Particelle As New StringBuilder
        Dim Str_ParticelleEsistenti As New StringBuilder
        Dim Str_ParticelleMancanti As New StringBuilder
        Dim Str_ParticelleUtilizzi As New StringBuilder
        Dim Str_ParticelleZone As New StringBuilder
        Dim Str_ParticelleMacrousi As New StringBuilder

        Dim num_ParticelleEsistenti As Integer = 0
        Dim num_ParticelleMancanti As Integer = 0

        Dim PROV As String = ""
        Dim COM As String = ""
        Dim Sezione As String = ""
        Dim Foglio As String = ""
        Dim Numero As String = ""
        Dim Subalterno As String = ""
        Dim SupCatastale As Double
        Dim TitoloPossesso As Integer
        Dim supConduzione As Double
        Dim PossessoInizio As Date
        Dim PossessoFine As Date
        Dim Indirizzo_Via As String
        Dim Indirizzo_Cap As String
        Dim Indirizzo_Prov As String
        Dim Indirizzo_Com As String

        Dim HT_ZoneXPart As New Hashtable
        Dim HT_CodificaZoneMancanti As New Hashtable
        Dim HT_MacrousiXPart As New Hashtable

        Dim ListaMacrouso As New List(Of MacrousoObject)
        Dim objMacrouso As New MacrousoObject
        Dim objUtilizzo As New UtilizzoObject

        Dim i, j, z As Integer

        Try

            If Not IsNothing(myISWSResponse.possessi) Then

                Dim vett_str As New List(Of String)
                Dim FiltraParticelle As Boolean = False
                'vett_str.Add("038 005  47 24 000")
                'vett_str.Add("038 005  47 36 000")
                If HttpContext.Current.Session("objParticelleTot") IsNot Nothing Then
                    vett_str = HttpContext.Current.Session("objParticelleTot")
                    FiltraParticelle = True
                End If

                For i = 0 To myISWSResponse.possessi.Count - 1

                    Try

                        'Id_Chiave_Cliente = myISWSResponse.azienda.idAzienda

                        PROV = myISWSResponse.possessi(i).codProv
                        COM = myISWSResponse.possessi(i).codCom
                        Sezione = myISWSResponse.possessi(i).sezione
                        Foglio = myISWSResponse.possessi(i).foglio
                        Numero = myISWSResponse.possessi(i).particella
                        Subalterno = myISWSResponse.possessi(i).subalterno

                        'verifico se la particella è stata selezionata dall'utente
                        Dim lPart As String = PROV & " " & COM & " " & Sezione & " " & Foglio & " " & Numero & " " & Subalterno

                        If Not FiltraParticelle OrElse vett_str.Contains(lPart) Then

                            'myISWSResponse.azienda.descComune, _
                            'myISWSResponse.azienda.descProvincia, _
                            Indirizzo_Via = myISWSResponse.azienda.indirizzo
                            Indirizzo_Cap = myISWSResponse.azienda.cap
                            Indirizzo_Prov = myISWSResponse.azienda.codIstatProv
                            Indirizzo_Com = myISWSResponse.azienda.codIstatCom

                            SupCatastale = 0
                            If Not IsNothing(myISWSResponse.possessi(i).supCatastale) Then
                                SupCatastale = myISWSResponse.possessi(i).supCatastale
                            End If

                            TitoloPossesso = Converti_TitoliPossesso_Agrea(myISWSResponse.possessi(i).codPossesso)

                            supConduzione = myISWSResponse.possessi(i).supPossesso
                            PossessoInizio = myISWSResponse.possessi(i).dataInizioPoss
                            PossessoFine = myISWSResponse.possessi(i).dataFinePoss

                            If Not IsNothing(myISWSResponse.possessi(i).macrousi) Then
                                For j = 0 To myISWSResponse.possessi(i).macrousi.Count - 1

                                    objMacrouso = New MacrousoObject
                                    objMacrouso.CodMacrouso = Right(myISWSResponse.possessi(i).macrousi(j).codMacrouso, 3)

                                    Select Case TipoImport
                                        Case -1
                                            'gestire qui i casi che vanno divisi per 10000
                                            objMacrouso.SupMacrouso = CDbl(myISWSResponse.possessi(i).macrousi(j).supMacrouso) / 10000.0
                                        Case Else
                                            objMacrouso.SupMacrouso = CDbl(myISWSResponse.possessi(i).macrousi(j).supMacrouso)
                                    End Select


                                    If Not IsNothing(myISWSResponse.possessi(i).macrousi(j).utilizzi) Then

                                        objMacrouso.Utilizzo = New List(Of UtilizzoObject)

                                        For z = 0 To myISWSResponse.possessi(i).macrousi(j).utilizzi.Count - 1
                                            objUtilizzo = New UtilizzoObject
                                            objUtilizzo.SpecieCod = myISWSResponse.possessi(i).macrousi(j).utilizzi(z).codColtura
                                            objUtilizzo.VarietaCod = myISWSResponse.possessi(i).macrousi(j).utilizzi(z).codVarieta

                                            Select Case TipoImport
                                                Case -1
                                                    'gestire qui i casi che vanno divisi per 10000
                                                    objUtilizzo.SupUtilizzo = CDbl(myISWSResponse.possessi(i).macrousi(j).utilizzi(z).supUtilizzo) / 10000.0
                                                Case Else
                                                    objUtilizzo.SupUtilizzo = CDbl(myISWSResponse.possessi(i).macrousi(j).utilizzi(z).supUtilizzo)
                                            End Select
                                            objMacrouso.Utilizzo.Add(objUtilizzo)
                                            'ListaUtilizzo.Add(objUtilizzo)
                                        Next
                                    End If
                                    'objMacrouso.Utilizzo = ListaUtilizzo
                                    ListaMacrouso.Add(objMacrouso)
                                    'ListaUtilizzo.Clear()
                                Next
                            End If 'macrousi

                            objSincroCatasto.Gestione_Sincronizzazione_Catasto(TipoImport,
                                                                               objParametri_Server,
                                                                               XmlDoc,
                                                                               XMLImpresa,
                                                                               XMLCentro,
                                                                               Str_Particelle,
                                                                               Str_ParticelleEsistenti,
                                                                               Str_ParticelleMancanti,
                                                                               Str_ParticelleUtilizzi,
                                                                               Str_ParticelleZone,
                                                                               Str_ParticelleMacrousi,
                                                                               num_ParticelleEsistenti,
                                                                               num_ParticelleMancanti,
                                                                               HT_ZoneXPart,
                                                                               HT_MacrousiXPart,
                                                                               HT_CodificaZoneMancanti,
                                                                               TipoOperazione,
                                                                               CodiceChiaveCliente,
                                                                               ValCod_CodiceChiaveCliente,
                                                                               Opt_Particelle_1Insert2Modifica,
                                                                               HT_PartCentri,
                                                                               Piva,
                                                                               Indirizzo_Via,
                                                                               Indirizzo_Cap,
                                                                               Indirizzo_Prov,
                                                                               Indirizzo_Com,
                                                                               PROV,
                                                                               COM,
                                                                               Sezione,
                                                                               Foglio,
                                                                               Numero,
                                                                               Subalterno,
                                                                               SupCatastale,
                                                                               TitoloPossesso,
                                                                               supConduzione,
                                                                               PossessoInizio,
                                                                               PossessoFine,
                                                                               myISWSResponse.possessi(i).codZone.ToArray,
                                                                               ListaMacrouso,
                                                                               objXML,
                                                                               objImpresexParticelle,
                                                                               objCentri,
                                                                               objCodificaZona,
                                                                               objZonexParticelle,
                                                                               objMacrousixParticelle,
                                                                               objPartxMacrousixUtilizzi)

                            ListaMacrouso.Clear()

                        End If

                    Catch ex As Exception
                        Dim Str_Particella_InCorso As String
                        Str_Particella_InCorso = PROV & " " &
                                  COM & " " &
                                  Sezione & " " &
                                  Foglio & " " &
                                  Numero & " " &
                                  Subalterno

                        Throw New Exception(Str_Particella_InCorso & " - " & ex.Message)
                    End Try




                Next

            End If

            'Dim pippo1, pippo2, pippo3, pippo4, pippo5, pippo6 As String

            ''Str_ParticelleDuplicate.Append("Num. particelle ripetute: " & CStr(num_ParticelleDuplicate))
            ''Str_ParticelleMancanti.Append("Num. particelle mancanti: " & CStr(num_ParticelleMancanti))
            ''Str_ParticelleSupCatModifica.Append("Num. particelle con sup.catastale aggiornata: " & CStr(num_ParticelleSupCatModifica))

            'pippo1 = Str_Particelle.ToString
            'pippo3 = Str_ParticelleMancanti.ToString
            '' pippo2 = Str_ParticelleDuplicate.ToString
            ''pippo4 = Str_ParticelleSupCatModifica.ToString
            'pippo5 = Str_ParticelleZone.ToString
            'pippo6 = Str_ParticelleMacrousi.ToString

            Log_Import.Append(vbCrLf & "Riepilogo situazione catasto:" & vbCrLf)
            Log_Import.Append("Num. particelle mancanti: " & CStr(num_ParticelleMancanti) & vbCrLf)
            Log_Import.Append(Str_ParticelleMancanti.ToString & vbCrLf)
            Log_Import.Append("Num. particelle esistenti: " & CStr(num_ParticelleEsistenti) & vbCrLf)
            Log_Import.Append(Str_ParticelleEsistenti.ToString & vbCrLf)
            Log_Import.Append(Str_ParticelleZone.ToString)
            Log_Import.Append("Num. codifiche zone mancanti: " & CStr(HT_CodificaZoneMancanti.Keys.Count) & vbCrLf)
            If HT_CodificaZoneMancanti.Keys.Count > 0 Then
                Log_Import.Append("Elenco codici zona (origine) mancanti: " & vbCrLf)
                For Each key In HT_CodificaZoneMancanti.Keys
                    Log_Import.Append(key & vbCrLf)
                Next
            End If
            Log_Import.Append(vbCrLf)
            Log_Import.Append(Str_ParticelleMacrousi.ToString)
            Log_Import.Append(Str_ParticelleUtilizzi.ToString)

        Catch ex As Exception
            Throw New Exception(NomeRoutine & " " & ex.Message)
        End Try

    End Sub

    Private Function Converti_TitoliPossesso_Agrea(ByVal TitoloPossesso As String) As Integer

        Dim Particella_Possesso As Integer

        If TitoloPossesso.StartsWith("FP") Then
            Particella_Possesso = Converti_TitoliPossesso_Anagrafe(TitoloPossesso)
        Else
            Select Case TitoloPossesso
                'Case "FP003", "FP004"
                '    Particella_Possesso = 0 'altro
                Case "001", "008", "014"
                    Particella_Possesso = 1 'Proprietà
                Case "009", "011", "010"
                    Particella_Possesso = 2 'Comodato d'uso
                Case "002"
                    Particella_Possesso = 3 'Affitto con contratto
                Case Else
                    Particella_Possesso = 0 'altro
            End Select
        End If

        Return Particella_Possesso

    End Function

    Public Function Converti_TitoliPossesso_Anagrafe(ByVal TitoloPossesso As String) As Integer

        Dim Particella_Possesso As Integer

        Select Case TitoloPossesso

            Case "FP003", "FP004"
                Particella_Possesso = 0 'altro
            Case "FP001", "FP021"
                Particella_Possesso = 1 'Proprietà
            Case "FP011", "FP018"
                Particella_Possesso = 2 'Comodato d'uso
            Case "FP002", "FP017"
                Particella_Possesso = 3 'Affitto con contratto
            Case ""
                Particella_Possesso = 4 'Affitto senza contratto
            Case Else
                Particella_Possesso = 0 'altro
        End Select

        Return Particella_Possesso

    End Function

    '##############################################################################################
    Private Function Recupera_Piva_Padre(ByVal ID_CAA As String, _
                                         ByVal Piva As String, _
                                         ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri) As String

        Dim PivaPadre As String = ""

        Select Case objParametri_Server.PivaSuperUser

            Case "05390270014", "05644051004" 'COLDIRETTI ER, COLDIRETTI NAZIONALE

                Dim ID_CAA_Format As String
                If ID_CAA.Length = 2 Then
                    ID_CAA_Format = "0" & ID_CAA
                Else
                    ID_CAA_Format = ID_CAA
                End If

                Dim objInfoAgg As New AgronicaCoreAnagrafeDAL.CAC_Codifica_InfoAggiuntive_R
                Dim Dt As DataTable
                Dt = objInfoAgg.Leggi(enum_CAC_Codifica_InfoAggiuntive_ArgomentoCod.CAA_Agrea, _
                                      ID_CAA_Format, _
                                      0, _
                                      1, _
                                      enumSelezioneVariabile.Selezione_JoinCompleta, _
                                      "", "", objParametri_Server)

                If Not IsNothing(Dt) AndAlso Dt.Rows.Count > 0 Then
                    PivaPadre = Dt.Rows(0).Item("TestoAux_1")
                Else
                    Select Case ID_CAA
                        Case 88, 89, 90, 91, 92, 93, 94, 95, 96  'BOLOGNA
                            PivaPadre = "04138500378"   '"00000000000"
                        Case 142, 147, 148, 149, 150, 151, 152  'REGGIO EMILIA
                            PivaPadre = "01895640355"    ' "11111111111"
                        Case 137, 138, 139  'RAVENNA
                            PivaPadre = "03477530400" '"22222222222"
                        Case 120, 121, 122, 123, 124, 125, 126, 127  'PIACENZA
                            PivaPadre = "01283730339" ' "33333333333"
                        Case 129, 130, 131, 132, 133, 134, 135, 136  'PARMA
                            PivaPadre = "02102610348" '"44444444444"
                        Case 145, 153, 154  'RIMINI
                            PivaPadre = "03477530400" '"55555555555"
                        Case 102, 103, 104, 105, 106, 107, 108  'FERRARA
                            PivaPadre = "00675910384"   '"66666666666"
                        Case 97, 98, 99, 100, 101  'FORLI
                            PivaPadre = "03477530400" '"77777777777"
                        Case 109, 110, 111, 112, 113, 114, 115, 116, 117, 118, 119  'MODENA
                            PivaPadre = "02613050364" '"88888888888"
                        Case Else
                            PivaPadre = objParametri_Server.PivaSuperUser
                    End Select

                End If

                '=====================================================

            Case "02317021208" 'FEDEREMILIA

                Select Case ID_CAA
                    Case 83, 180, 181, 182, 183 'BOLOGNA OK
                        PivaPadre = "80038970374"
                    Case 86, 194, 195 'REGGIO EMILIA OK
                        PivaPadre = "80012830354"
                    Case 81, 176, 177, 178, 179  'PIACENZA OK
                        PivaPadre = "90011170338"
                    Case 84, 211, 212, 213, 214, 215  'PARMA OK
                        PivaPadre = "80004070340"
                    Case 87 'FERRARA
                        PivaPadre = "80006190385"
                    Case 80  'FORLI CESENA RIMINI OK
                        PivaPadre = "80009510407"
                    Case 82, 230, 231, 232, 233, 234, 235, 236, 237  'MODENA OK
                        PivaPadre = "80008110365"
                    Case 85, 240, 241  'RAVENNA OK
                        PivaPadre = "80008770390"
                    Case Else
                        PivaPadre = objParametri_Server.PivaSuperUser
                End Select

                '======================================

            Case Else
                '    PivaPadre = objParametri_Server.PivaSuperUser

                Dim objInfoAgg As New AgronicaCoreAnagrafeDAL.CAC_Codifica_InfoAggiuntive_R
                Dim Dt As DataTable = Nothing

                Dt = objInfoAgg.Leggi(enum_CAC_Codifica_InfoAggiuntive_ArgomentoCod.CAA_Agrea, _
                                      ID_CAA, _
                                      0, _
                                      1, _
                                      enumSelezioneVariabile.Selezione_JoinCompleta, _
                                      "", "", objParametri_Server)

                If Not IsNothing(Dt) AndAlso Dt.Rows.Count > 0 Then
                    PivaPadre = Dt.Rows(0).Item("TestoAux_1")
                Else
                    'correzione dell'08/08/2014: 
                    'per evitare di scrivere male il record del superuser in gerarchieimprese
                    If Piva = objParametri_Server.PivaSuperUser Then
                        'è il superuser
                        PivaPadre = ""
                    Else
                        'è un'azienda figlia del superuser
                        PivaPadre = objParametri_Server.PivaSuperUser
                    End If
                End If

                Dt.Dispose()
                Dt = Nothing

                objInfoAgg = Nothing

                '--------------------------------------

        End Select

        Return PivaPadre

    End Function


    '########################################################################################
    Public Sub Scrivi_Dati(ByRef Log_Errori As StringBuilder, _
                            ByRef Log_Import As StringBuilder, _
                            ByVal strDatiAnagrafe As String, _
                            ByVal strDatiPianificazione As String, _
                            ByRef Str_RisultatoMassivaImport As String, _
                            ByRef Str_RisultatoMassivaErrori As String, _
                            ByRef Str_RisultatoCUAA As String, _
                            ByRef Num_Azi_Sincronizzate As Integer, _
                            ByVal Utente_Username As String, _
                            ByVal Utente_Password As String, _
                            ByVal Piva_SuperUser As String, _
                            ByVal Azienda_InElaborazione As String, _
                            ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                            ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri)

        Const NomeFunzione As String = "Scrivi_Dati."

        Dim Documento_Finale As New System.Xml.XmlDocument
        Dim XML_Risultato As System.Xml.XmlElement
        Dim XML_Risposta As System.Xml.XmlElement
        Dim XMLs_Risposta As System.Xml.XmlNodeList

        Dim StrFinaleAnagrafe As String
        Dim StrFinalePianificazione As String

        Dim LinkWSImportaGIAS As String
        Dim CodiceChiaveCliente As Integer

        Dim x As Integer

        Dim Importa As New ImportaWS

        Dim confSitiReader As New AgronicaCoreVarieDAL.Configurazione_Siti_R

        CodiceChiaveCliente = confSitiReader.Leggi_Valore(16, "Sincro_Codice_Chiave_Cliente", "", "", objParametri_Server)
        LinkWSImportaGIAS = confSitiReader.Leggi_Valore(16, "Sincro_LinkWSImportaGIAS", "", "", objParametri_Server)


        Try

            If LinkWSImportaGIAS <> "" Then

                Importa.Url = LinkWSImportaGIAS
                Importa.Timeout = Integer.MaxValue

                Str_RisultatoMassivaImport = String.Empty
                Str_RisultatoMassivaErrori = String.Empty
                Str_RisultatoCUAA = String.Empty

                Dim Str_Credenziali_WS As String
                Dim objXmlWs As New AgronicaCoreXML.XML_WS_Importa_Gias

                Str_Credenziali_WS = objXmlWs.Genera_Stringa_Credenziali( _
                                     True, _
                                     Nothing, _
                                     Utente_Username, _
                                     Utente_Password, _
                                     objParametri_Server.PivaSuperUser, _
                                     True, _
                                     "", _
                                     "", _
                                     "", _
                                     "", _
                                     "", _
                                     "", _
                                     objParametri_Server.StringaConnessione, _
                                        objParametri_Utenti.StringaConnessione)

                If strDatiAnagrafe <> String.Empty Then

                    Str_RisultatoCUAA &= "<b>Sincronizzazione anagrafe:" & "</b><br/>"

                    Log_Import.Append(vbCrLf & Date.Now.ToString & " - Inizio scrittura catasto." & vbCrLf)

                    Try

                        StrFinaleAnagrafe = Importa.Importa_DocumentoPubblico_SuperServer(Str_Credenziali_WS, _
                                                                                        strDatiAnagrafe, _
                                                                                       CodiceChiaveCliente)

                        Num_Azi_Sincronizzate += 1


                    Catch ex As HttpException
                        Str_RisultatoCUAA &= "Chiamata al web service: " & ex.Message & vbCrLf
                        Str_RisultatoMassivaErrori &= "Chiamata al web service: " & ex.Message & vbCrLf
                        Exit Sub
                    End Try

                    Log_Import.Append(Date.Now.ToString & " - Fine scrittura catasto." & vbCrLf)

                    Documento_Finale.LoadXml(StrFinaleAnagrafe)

                    XML_Risultato = Documento_Finale.SelectSingleNode("Risultato")

                    If XML_Risultato.HasAttribute("errore") Then
                        Str_RisultatoCUAA &= "- " & XML_Risultato.GetAttribute("errore").ToString & "<br/>"
                        Str_RisultatoMassivaErrori &= "Sincro anagrafe ERRORE: " & XML_Risultato.GetAttribute("errore").ToString & vbCrLf
                    Else

                        XMLs_Risposta = XML_Risultato.GetElementsByTagName("Risposta")

                        Dim strRisp As String = String.Empty

                        For x = 0 To XMLs_Risposta.Count - 1

                            XML_Risposta = XMLs_Risposta.Item(x)

                            strRisp = XML_Risposta.GetAttribute("Ris")

                            Str_RisultatoCUAA &= "- " & strRisp & IIf(InStr(strRisp, "Errore"), "", " - Terminata correttamente") & "<br/>"

                            If InStr(strRisp, "Errore") > 0 Then
                                Str_RisultatoMassivaErrori &= "Sincro anagrafe ERRORE: " & strRisp & vbCrLf
                            Else
                                Str_RisultatoMassivaImport &= "Sincro anagrafe: " & strRisp & " OK" & vbCrLf
                            End If

                        Next

                    End If

                Else
                    ''stringa vuota
                    Str_RisultatoCUAA &= "<b>Sincronizzazione anagrafe:" & "</b><br/>"
                    Str_RisultatoCUAA &= "- " & "non eseguita" & "<br/>"
                End If

                If strDatiPianificazione <> String.Empty Then

                    Str_RisultatoCUAA &= "</br><b>Importazione pianificazione:" & "</b><br/>"

                    Log_Import.Append(vbCrLf & Date.Now.ToString & " - Inizio scrittura pianificazione." & vbCrLf)

                    'StrFinalePianificazione = Importa.Importa_Pianificazione(Utente_Username, _
                    '                                                        Utente_Password, _
                    '                                                       Piva_SuperUser, _
                    '                                                       strDatiPianificazione, _
                    '                                                       CodiceChiaveCliente)

                    StrFinalePianificazione = Importa.Importa_Pianificazione_SuperServer(Str_Credenziali_WS, _
                                                                                         strDatiPianificazione, _
                                                                                            CodiceChiaveCliente)

                    Log_Import.Append(Date.Now.ToString & " - Fine scrittura pianificazione." & vbCrLf)

                    Documento_Finale.LoadXml(StrFinalePianificazione)

                    XML_Risultato = Documento_Finale.SelectSingleNode("Risultato")

                    If XML_Risultato.HasAttribute("errore") Then
                        Str_RisultatoCUAA &= "- " & XML_Risultato.GetAttribute("errore").ToString & "<br/>"
                        Str_RisultatoMassivaErrori &= "Import Pianificazione ERRORE: " & XML_Risultato.GetAttribute("errore").ToString & vbCrLf
                    Else

                        XMLs_Risposta = XML_Risultato.GetElementsByTagName("Risposta")

                        Dim strRisp As String = String.Empty

                        For x = 0 To XMLs_Risposta.Count - 1

                            XML_Risposta = XMLs_Risposta.Item(x)

                            strRisp = XML_Risposta.GetAttribute("Ris")

                            Str_RisultatoCUAA &= "- " & strRisp & If(InStr(strRisp, "Errore"), "", " - Terminata correttamente") & "<br/>"

                            If InStr(strRisp, "Errore") > 0 Then
                                Str_RisultatoMassivaErrori &= "Import pianificazione ERRORE: " & strRisp & vbCrLf
                            Else
                                Str_RisultatoMassivaImport &= "Import pianificazione: " & strRisp & " OK" & vbCrLf
                            End If

                        Next

                    End If
                Else
                    ''stringa vuota
                    Str_RisultatoCUAA &= "</br><b>Importazione pianificazione:" & "</b><br/>"
                    Str_RisultatoCUAA &= "- " & "non eseguita" & "<br/>"
                End If

                'Me.Lbl_RisImportazione.Text = Descrizione

            Else
                'il controllo è già stato fatto a monte
                'AgroMsgBox("Inserire l'indirizzo del Web Service Gias per continuare.", Page)
                'Log_Errori.Append(NomeFunzione & "Inserire l'indirizzo del Web Service Gias per continuare." & vbCrLf)
            End If

        Catch ex As Exception
            'AgroMsgBox(ex.Message, Page)
            'Log_Errori.Append(NomeFunzione & ex.Message & vbCrLf)
            Str_RisultatoCUAA &= NomeFunzione & ex.Message & vbCrLf
            Str_RisultatoMassivaErrori &= NomeFunzione & ex.Message & vbCrLf
        End Try


    End Sub

    Private Function Ribalta_Pianificazione(ByVal Programmazione_Cod As Integer, _
                                               ByVal SchedaValidazione As String, _
                                               ByVal DataValidazione As Date, _
                                               ByVal DtPV As DataTable, _
                                               Ribalta_Limiti As Boolean, _
                                               progressivoGIAS As Integer, _
                                               logErrori As StringBuilder) As Boolean


        Dim i As Integer = 0
        Dim strErr As String = ""

        Dim Key_Piva As String
        Dim Key_SaCod As String
        Dim Key_CampoCod As Integer
        Dim Key_Appezza As Integer
        Dim Key_IdReg As Integer
        Dim Key_ProgettoCod As Integer
        Dim Key_Entita As Integer

        Dim StrCampo As String
        Dim StrAppezzamento As String
        Dim StrParticelle As String
        Dim StrRegImpianto As String
        Dim StrProgetto As String

        Dim strXMLAppezzamenti As String

        Dim OUTPUT_Piva As String
        Dim OUTPUT_Sa_Cod As Integer
        Dim OUTPUT_Campo_Cod As Integer
        Dim OUTPUT_Appezza As Integer
        Dim OUTPUT_Id_Reg As Integer
        Dim OUTPUT_Progetto_Cod As Integer

        Dim InseritoProgetto As Boolean = False
        Dim InseritoImpianto As Boolean = False
        Dim InseritoAppezzamento As Boolean = False
        Dim InseritoCampo As Boolean = False

        Dim ZeroData As String = "0"
        Dim ZeroInt As Integer = 0
        Dim ZeroString As String = "0"
        Dim ZeroDouble As Double = 0
        Dim NullString As String = ""
        Dim Null As String = "NULL"
        Dim PuntoString As String = "."

        Dim Progetto_Nome As String
        Dim Resa As Double
        Dim Stato_Impianto As Integer
        Dim Regolamento_Concimazioni_Cod As Integer = 5 'UMBRIA

        Dim Id_Cod As Integer
        Dim Veg_Cod As Integer
        'Dim Gru_Cod As Integer
        Dim Cul_Cod As Integer
        Dim Grfi_Cod As Integer
        Dim Grva_Cod As Integer

        Dim Veg_Cod_Cliente As String = ""
        Dim Cul_Cod_Cliente As String = ""

        Dim Validita_Inizio As Date
        Dim Validita_Fine As Date

        Dim Sup_App As Double
        Dim Ettari As Double
        Dim Are As Double
        Dim Centiare As Double

        Dim objAzoto As New AgronicaCoreMetaSchemaDAL.LimitiAzotoxSpecie_R
        Dim Riduzione_N As Double = 0
        Dim RiduzioneP_N As Double = 0
        Dim Limite As Integer = 0
        Dim Prov, Com As String
        Dim Foglio As Integer

        Dim BaseCode As Integer
        Dim TopCode As Integer

        Dim XmlDocAppoggio As System.Xml.XmlDocument

        Dim XmlAppezzamento As System.Xml.XmlElement
        Dim XML_DatiParticelle As System.Xml.XmlElement

        Dim objXML As New AgronicaCoreXML.AnagrafeXML

        '----- Calcolo i valori di BaseCode e TopCode
        UtilityProvider.Calcola_BaseCode_TopCode(BaseCode, TopCode, progressivoGIAS)

        Dim objP As New AgronicaCoreAnagrafeBIZ.Programmazione_R
        Dim objPP As New AgronicaCoreAnagrafeDAL.Programmazione_Particelle_R
        Dim objCampo_W As New AgronicaCoreAnagrafeBIZ.Campo_W
        Dim objAppezzamento_W As New AgronicaCoreAnagrafeBIZ.Appezzamento_W
        Dim objReg_Impianto_W As New AgronicaCoreAnagrafeBIZ.Reg_Impianto_W
        Dim objReg_Impianti_Codici_W As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Codici_W
        Dim objProgetto_W As New AgronicaCoreAnagrafeBIZ.Progetto_W

        Try

            Dim DT_Appezzamenti As New DataTable

            DT_Appezzamenti = objP.Anagrafica_AppezzamentixRibaltamento_Leggi(CInt(Programmazione_Cod), _
                                                                              ObjParametri_Server.PivaSuperUser, _
                                                                              strErr, _
                                                                              ObjParametri_Server)

            '-----------------------------------------
            '-----------------------------------------
            '-----------------------------------------

            Dim DtPE As DataTable = Nothing

            If Ribalta_Limiti Then
                'PER LIMITI AZOTO!!!!!!!!!!!!!
                'Leggo le particelle della pianificazione
                Dim objPE As New AgronicaCoreAnagrafeDAL.Programmazione_Particelle_R
                DtPE = objPE.LeggiParticelle_Da_Programmazione("", 0, Programmazione_Cod, 0, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", ObjParametri_Server)
            End If




            '-----------------------------------------
            '-----------------------------------------
            '-----------------------------------------

            For i = 0 To DT_Appezzamenti.Rows.Count - 1

                Key_Piva = DT_Appezzamenti.Rows(i).Item("Piva")
                Key_SaCod = DT_Appezzamenti.Rows(i).Item("Sa_Cod")
                Key_CampoCod = DT_Appezzamenti.Rows(i).Item("Campo_Cod")
                Key_Appezza = DT_Appezzamenti.Rows(i).Item("Appezza")
                Key_IdReg = DT_Appezzamenti.Rows(i).Item("Id_Reg")
                Key_ProgettoCod = DT_Appezzamenti.Rows(i).Item("Progetto_Cod")

                Key_Entita = DT_Appezzamenti.Rows(i).Item("Programmazione_Entita_Cod")

                Sup_App = DT_Appezzamenti.Rows(i).Item("Sup_App")

                Veg_Cod = DT_Appezzamenti.Rows(i).Item("Veg_Cod")
                Cul_Cod = DT_Appezzamenti.Rows(i).Item("Cul_Cod")
                Grva_Cod = DT_Appezzamenti.Rows(i).Item("Grva_Cod")
                Grfi_Cod = DT_Appezzamenti.Rows(i).Item("Grfi_Cod")
                Id_Cod = DT_Appezzamenti.Rows(i).Item("Id_Cod")

                Veg_Cod_Cliente = DT_Appezzamenti.Rows(i).Item("Veg_Cod_Cliente")
                Cul_Cod_Cliente = DT_Appezzamenti.Rows(i).Item("Cul_Cod_Cliente")

                Progetto_Nome = DT_Appezzamenti.Rows(i).Item("Progetto_Nome")
                Resa = DT_Appezzamenti.Rows(i).Item("Resa")

                Stato_Impianto = 102

                Validita_Inizio = CDate(DT_Appezzamenti.Rows(i).Item("Validita_Inizio"))
                Validita_Fine = CDate(DT_Appezzamenti.Rows(i).Item("Validita_Fine"))

                ''controllo che esistano le particelle
                '' inizio possesso
                'If IsDate(DT_Appezzamenti.Rows(i).Item("Inizio_Possesso")) Then
                '    If Validita_Inizio < CDate(DT_Appezzamenti.Rows(i).Item("Inizio_Possesso")) Then
                '        Validita_Inizio = CDate(DT_Appezzamenti.Rows(i).Item("Inizio_Possesso"))
                '    End If
                'End If
                '' fine possesso
                'If IsDate(DT_Appezzamenti.Rows(i).Item("Fine_Possesso")) Then
                '    If Validita_Fine > CDate(DT_Appezzamenti.Rows(i).Item("Fine_Possesso")) Then
                '        Validita_Fine = CDate(DT_Appezzamenti.Rows(i).Item("Fine_Possesso"))
                '    End If
                'End If


                '-----------------------------------------
                '-----------------------------------------
                '-----------------------------------------
                'Creo il Campo
                If i = 0 Then

                    StrCampo = ""
                    objXML.XML_Campo(enum_CodificaDecodifica.Codifica, _
                                            StrCampo, _
                                            enum_TipoOperazioneDB.Scrittura, _
                                            Key_Piva, _
                                            Key_SaCod, _
                                            ZeroInt, _
                                            ZeroInt, _
                                            "Fascicolo: " & SchedaValidazione & " (Data validazione " & DataValidazione.ToShortDateString & ")", _
                                            AGRODATAINIZIO, _
                                            AGRODATAINIZIO, _
                                            ZeroInt, _
                                            ZeroInt, _
                                            ZeroDouble, _
                                            ZeroDouble, _
                                            NullString, _
                                            ZeroInt, _
                                            ZeroInt, _
                                            Validita_Inizio, _
                                            Validita_Fine, _
                                            BaseCode, _
                                            TopCode)

                    If StrCampo <> "" Then

                        StrCampo = "<DatiCampi>" & StrCampo & "</DatiCampi>"

                        InseritoCampo = objCampo_W.Campo_Scrivi(StrCampo, _
                                                                OUTPUT_Piva, _
                                                                OUTPUT_Sa_Cod, _
                                                                OUTPUT_Campo_Cod, _
                                                                False, _
                                                                ObjParametri_Server, _
                                                                ObjParametri_Utenti)

                    End If


                End If



                If InseritoCampo Then


                    StrAppezzamento = ""
                    objXML.XML_Appezzamento(enum_CodificaDecodifica.Codifica, _
                                            StrAppezzamento, _
                                            enum_TipoOperazioneDB.Scrittura, _
                                            Key_Piva, _
                                            Key_SaCod, _
                                            ZeroInt, _
                                            Sup_App, _
                                            ZeroData, _
                                            ZeroData, _
                                            ZeroInt, _
                                            ZeroInt, _
                                            ZeroInt, _
                                            PuntoString, _
                                            ZeroInt, _
                                            PuntoString, _
                                            ZeroInt, _
                                            NullString, _
                                            ZeroDouble, _
                                            ZeroDouble, _
                                            ZeroDouble, _
                                            ZeroDouble, _
                                            ZeroDouble, _
                                            ZeroDouble, _
                                            ZeroDouble, _
                                            ZeroDouble, _
                                            ZeroDouble, _
                                            ZeroDouble, _
                                            ZeroDouble, _
                                            ZeroDouble, _
                                            ZeroString, _
                                            ZeroInt, _
                                            ZeroDouble, _
                                            ZeroData, _
                                            ZeroDouble, _
                                            ZeroData, _
                                            ZeroData, _
                                            ZeroDouble, _
                                            ZeroData, _
                                            NullString, _
                                            DT_Appezzamenti.Rows(i).Item("App_Nome"), _
                                            ZeroInt, _
                                            ZeroDouble, _
                                            NullString, _
                                            OUTPUT_Campo_Cod, _
                                            ZeroInt, _
                                            ZeroData, _
                                            ZeroData, _
                                            Validita_Inizio, _
                                            Validita_Fine, _
                                            BaseCode, _
                                            TopCode)


                    XmlDocAppoggio = New System.Xml.XmlDocument
                    XmlDocAppoggio.LoadXml(StrAppezzamento)

                    XmlAppezzamento = XmlDocAppoggio.SelectSingleNode("Appezzamento")

                    Dim Str_CodiceAppezzamento As String = ""
                    objXML.XML_Codice(enum_CodificaDecodifica.Codifica, _
                                      Str_CodiceAppezzamento, _
                                      enum_TipoOperazioneDB.Scrittura, _
                                      enum_CodiciAnagrafe.TitoloPossesso, _
                                      1, _
                                      Validita_Inizio, _
                                      Validita_Fine, _
                                      BaseCode, _
                                      TopCode, "Appezzamento")


                    Dim DT_Particelle As New DataTable

                    DT_Particelle = objPP.Programmazione_Particelle_Leggi_3(ObjParametri_Server, _
                                                                          Key_Piva, _
                                                                          strErr, _
                                                                          DT_Appezzamenti.Rows(i).Item("Programmazione_Entita_Cod"))
                    StrParticelle = String.Empty

                    For j = 0 To DT_Particelle.Rows.Count - 1

                        Conversioni.EttariAreCentiare_from_Ettari(DT_Particelle.Rows(j).Item("Superficie"), Ettari, Are, Centiare)

                        StrParticelle &= objXML.XML_AppezzamentoParticella(enum_TipoOperazioneDB.Scrittura, _
                                                                          Key_Piva, _
                                                                          Key_SaCod, _
                                                                          ZeroInt, _
                                                                          DT_Particelle.Rows(j).Item("PROV"), _
                                                                          DT_Particelle.Rows(j).Item("COM"), _
                                                                          DT_Particelle.Rows(j).Item("Sezione"), _
                                                                          DT_Particelle.Rows(j).Item("foglio"), _
                                                                          DT_Particelle.Rows(j).Item("numero"), _
                                                                          DT_Particelle.Rows(j).Item("subalterno"), _
                                                                          DT_Particelle.Rows(j).Item("Superficie"), _
                                                                          Ettari, _
                                                                          Are, _
                                                                          Centiare, _
                                                                          ZeroInt, ZeroInt, ZeroInt, ZeroInt, ZeroInt, ZeroInt, _
                                                                          Validita_Inizio, _
                                                                          Validita_Fine, _
                                                                          BaseCode, _
                                                                          TopCode)

                    Next

                    XmlAppezzamento.InnerXml = Str_CodiceAppezzamento

                    If StrParticelle <> "" Then
                        XML_DatiParticelle = XmlAppezzamento.OwnerDocument.CreateElement("DatiParticelle")
                        XmlAppezzamento.AppendChild(XML_DatiParticelle)
                        XML_DatiParticelle.InnerXml = StrParticelle
                    End If

                    strXMLAppezzamenti = "<DatiAppezzamenti>" & XmlAppezzamento.OuterXml & "</DatiAppezzamenti>"

                    InseritoAppezzamento = objAppezzamento_W.Appezzamento_Scrivi(strXMLAppezzamenti, _
                                                                                OUTPUT_Piva, _
                                                                                OUTPUT_Sa_Cod, _
                                                                                OUTPUT_Appezza, _
                                                                                ObjParametri_Server, _
                                                                                ObjParametri_Utenti)


                    If InseritoAppezzamento Then

                        StrRegImpianto = String.Empty
                        objXML.XML_Impianto(enum_CodificaDecodifica.Codifica, _
                                            StrRegImpianto, _
                                            enum_TipoOperazioneDB.Scrittura, _
                                            OUTPUT_Piva, _
                                            OUTPUT_Sa_Cod, _
                                            OUTPUT_Campo_Cod, _
                                            OUTPUT_Appezza, _
                                            ZeroInt, _
                                            Sup_App, _
                                            ZeroInt, _
                                            ZeroInt, _
                                            ZeroInt, _
                                            Validita_Inizio, _
                                            Cul_Cod, _
                                            Grva_Cod, _
                                            Null, _
                                            ZeroDouble, _
                                            ZeroDouble, _
                                            ZeroInt, _
                                            ZeroInt, ZeroString, NullString, NullString, NullString, _
                                            ZeroDouble, _
                                            ZeroDouble, _
                                            ZeroDouble, _
                                            ZeroString, _
                                            ZeroInt, _
                                            ZeroInt, ZeroInt, ZeroInt, ZeroInt, ZeroInt, ZeroInt, ZeroString, ZeroInt, ZeroInt, ZeroInt, ZeroInt, ZeroInt, _
                                            NullString, ZeroString, _
                                            Grfi_Cod, _
                                            ZeroInt, _
                                            CInt(1), _
                                            ZeroInt, _
                                            ZeroInt, _
                                            ZeroInt, _
                                            ZeroInt, _
                                            ZeroInt, ZeroInt, _
                                            Validita_Inizio, _
                                            Validita_Fine, _
                                            BaseCode, _
                                            TopCode, _
                                            ZeroInt)

                        StrRegImpianto = "<DatiReg_Impianti>" & StrRegImpianto & "</DatiReg_Impianti>"

                        'salvataggio!!!
                        InseritoImpianto = objReg_Impianto_W.Reg_Impianto_Scrivi( _
                                                        CStr(StrRegImpianto), _
                                                        OUTPUT_Piva, _
                                                        OUTPUT_Sa_Cod, _
                                                        OUTPUT_Appezza, _
                                                        OUTPUT_Id_Reg, _
                                                        "", _
                                                        ObjParametri_Server)


                        If InseritoImpianto Then
                            '---------------------------------
                            'caso destinazioni d'uso
                            If Id_Cod <> 0 Then
                                objReg_Impianti_Codici_W.ScrivixProgetto(DT_Appezzamenti.Rows(i).Item("Piva"), _
                                                                         Key_SaCod, OUTPUT_Appezza, OUTPUT_Id_Reg, Key_ProgettoCod, _
                                                                Id_Cod, _
                                                                "", _
                                                                Validita_Inizio, Validita_Fine, ObjParametri_Server)
                            End If


                            'CREO LA DISTINTA
                            StrProgetto = ""
                            objXML.Xml_ProgettoPerImpianto(enum_CodificaDecodifica.Codifica, _
                                                           StrProgetto, _
                                                           enum_TipoOperazioneDB.Scrittura, _
                                                           Key_Piva, _
                                                           Key_SaCod, _
                                                           ZeroInt, _
                                                           Progetto_Nome, _
                                                           NullString, _
                                                           enum_Agenda_Causali.Progetto_Produzione_Agricola, _
                                                            ZeroInt, _
                                                           ZeroInt, _
                                                           AGRODATAINIZIO, _
                                                           AGRODATAFINE, _
                                                           NullString, _
                                                           OUTPUT_Appezza, _
                                                           OUTPUT_Id_Reg, _
                                                           0, _
                                                           0, _
                                                           Stato_Impianto, _
                                                           1, _
                                                           ZeroInt, _
                                                           -Regolamento_Concimazioni_Cod, _
                                                           ZeroInt, _
                                                           ZeroDouble, _
                                                           Resa, _
                                                           Validita_Inizio, _
                                                           Validita_Fine, _
                                                           BaseCode, _
                                                           TopCode)

                            InseritoProgetto = objProgetto_W.Impresa_Progetto_Scrivi( _
                                                            CStr(StrProgetto), _
                                                            OUTPUT_Piva, _
                                                            OUTPUT_Progetto_Cod, _
                                                            ObjParametri_Server)

                            If InseritoProgetto Then


                                If Ribalta_Limiti Then
                                    Riduzione_N = 0
                                    RiduzioneP_N = 0

                                    If DtPV IsNot Nothing AndAlso DtPV.Rows.Count > 0 Then

                                        Dim DrPE() As DataRow
                                        If DtPE IsNot Nothing AndAlso DtPE.Rows.Count > 0 Then
                                            DrPE = DtPE.Select("Programmazione_Entita_Cod=" & Key_Entita.ToString)
                                            If DrPE IsNot Nothing AndAlso DrPE.Length > 0 Then
                                                For p = 0 To DrPE.Length - 1
                                                    RiduzioneP_N = 0
                                                    Prov = DrPE(p).Item("prov")
                                                    Com = DrPE(p).Item("com")
                                                    Foglio = DrPE(p).Item("foglio")
                                                    Dim DrPV() As DataRow
                                                    DrPV = DtPV.Select("PROV='" & Prov & "' AND COM='" & Com & "' AND foglio=" & Foglio.ToString)
                                                    If DrPV IsNot Nothing AndAlso DrPV.Length > 0 Then
                                                        If Not IsDBNull(DrPV(0).Item("Riduzione_N")) AndAlso CDbl(DrPV(0).Item("Riduzione_N")) <> 0 Then
                                                            RiduzioneP_N = CDbl(DrPV(0).Item("Riduzione_N"))
                                                        End If
                                                    End If
                                                    If Riduzione_N < RiduzioneP_N Then
                                                        Riduzione_N = RiduzioneP_N
                                                    End If
                                                Next
                                            End If
                                        End If
                                    End If

                                    Limite = 0
                                    Limite = objAzoto.RecuperaAzotoFromVegCod_Regolamento(Veg_Cod, _
                                                                       Grfi_Cod, _
                                                                      Stato_Impianto, _
                                                                      Validita_Inizio, _
                                                                      Validita_Inizio, _
                                                                      True, _
                                                                      Regolamento_Concimazioni_Cod, _
                                                                      ObjParametri_Server)

                                    'riduco in percentuale in caso ci sia da ridurre l'apporto max di azoto
                                    If Limite <> 0 AndAlso Riduzione_N <> 0 Then
                                        Limite = Limite - (Limite * Riduzione_N / 100)
                                    End If

                                    If Limite <> 0 Then
                                        objReg_Impianti_Codici_W.ScrivixProgetto(Key_Piva, Key_SaCod, OUTPUT_Appezza, OUTPUT_Id_Reg, OUTPUT_Progetto_Cod, _
                                                                                 enum_CodiciAnagrafe.Impianto_LimiteN, _
                                                                                 Limite, _
                                                                                 Validita_Inizio, Validita_Fine, ObjParametri_Server)
                                    End If

                                End If

                                '  Vanni, 07/06/2013 14:22:20: ripristinata da Ribalta_OLD() --> Genera_Stringone_XML
                                If Veg_Cod_Cliente <> "" Then
                                    objReg_Impianti_Codici_W.ScrivixProgetto(Key_Piva, Key_SaCod, OUTPUT_Appezza, OUTPUT_Id_Reg, OUTPUT_Progetto_Cod, _
                                                                     enum_CodiciAnagrafe.Codice_Specie_Agea, _
                                                                     Veg_Cod_Cliente, _
                                                                    Validita_Inizio, Validita_Fine, ObjParametri_Server)
                                End If

                                If Cul_Cod_Cliente <> "" Then
                                    objReg_Impianti_Codici_W.ScrivixProgetto(Key_Piva, Key_SaCod, OUTPUT_Appezza, OUTPUT_Id_Reg, OUTPUT_Progetto_Cod, _
                                                                     enum_CodiciAnagrafe.Codice_Cultivar_Agea, _
                                                                     Cul_Cod_Cliente, _
                                                                    Validita_Inizio, Validita_Fine, ObjParametri_Server)
                                End If

                                Dim reg_impianti_programmazione_w As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Programmazioni_W
                                reg_impianti_programmazione_w.Scrivi( _
                                    Key_Piva, Key_SaCod, OUTPUT_Appezza, OUTPUT_Id_Reg, OUTPUT_Progetto_Cod, _
                                    Programmazione_Cod, Key_Entita, Validita_Inizio, Validita_Fine, _
                                    ObjParametri_Server _
                                )



                            End If

                        End If

                    End If

                End If


            Next

        Catch ex As Exception

            logErrori.Append(ex.Message & vbCrLf)

        End Try

        Return True

    End Function

#Region "Util"

    Private Sub logga(ByVal msg As String)
        Dim _customLOGParams As New CustomLOGParams With {
            .LogDescrizioneUtente = LogDescrizioneUtente,
            .LogDirectory = LogDirectory,
            .LogFileName = LogFileName
        }

        objLog.Scrivi_LOG(ObjParametri_Server, "", msg, CustomLOGParams:=_customLOGParams)

    End Sub

    Private Sub InizializzoOggettiCore()

        customLOGParams = New CustomLOGParams With {
            .LogDescrizioneUtente = ObjParametri_Server.LogDescrizioneUtente,
            .LogDirectory = LogDirectory,
            .LogFileName = LogFileName
        }

        objLog = New AgronicaCoreDataProvider.LogProvider

    End Sub

    Private Sub ImpostoGliAltriParametri(ByVal _Configurazione_Servizio As AgronicaCoreVarieDAL.Configurazione_Servizio)

        LogFileName = _Configurazione_Servizio.Tipo_Sincro.ToString & "_log.txt"
        LogDirectory = _Configurazione_Servizio.DirectoryLOG
        LogDescrizioneUtente = _Configurazione_Servizio.Tipo_Sincro.ToString
        DirectoryFileImportazioni = _Configurazione_Servizio.DirectoryFileImportazioni
        DirectoryFileEsportazioni = _Configurazione_Servizio.DirectoryFileEsportazioni
        ParametriExtra = _Configurazione_Servizio.Parametri_Extra

        username = _Configurazione_Servizio.Username
        password = _Configurazione_Servizio.Password

    End Sub

    Private Sub impostaParametriExtra()
        Dim listaPar As List(Of String) = ParametriExtra.Split("|").ToList
        Dim ht As New Hashtable
        For Each par In listaPar
            Dim key = par.Split("=")(0)
            Dim value = par.Split("=")(1)
            ht.Add(key, value)
        Next
        'WS
        Gias_Fascicoli = ht.Item("Gias_Fascicoli")
        Dim stringaAnni As String = ht.Item("Anni_Da_Importare")
        If stringaAnni <> "" Then
            AnniDaImportare = New List(Of Integer)
            For Each annoStr In stringaAnni.Split(",")
                If annoStr <> "" Then
                    Try
                        Dim anno As Integer = CInt(annoStr)
                        AnniDaImportare.Add(anno)
                    Catch ex As Exception

                    End Try
                End If
            Next
        End If
        'If ConfigurationSettings.AppSettings("WS_Anagrafe") IsNot Nothing Then
        '    linkWS_Anagrafe = ConfigurationSettings.AppSettings("WS_Anagrafe")
        'Else
        '    linkWS_Anagrafe = ""
        'End If
        'If ConfigurationSettings.AppSettings("Ws_Agrea") IsNot Nothing Then
        '    linkWS_Agrea = ConfigurationSettings.AppSettings("Ws_Agrea")
        'Else
        '    linkWS_Agrea = ""
        'End If

        'Username
        'If ConfigurationSettings.AppSettings("usernameWSAnagrafe_Coldi") IsNot Nothing Then
        '    username_Anagrafe = ConfigurationSettings.AppSettings("usernameWSAnagrafe_Coldi")
        'Else
        '    username_Anagrafe = ""
        'End If
        'If ConfigurationSettings.AppSettings("usernameWSAgrea_Coldi") IsNot Nothing Then
        '    username_Agrea = ConfigurationSettings.AppSettings("usernameWSAgrea_Coldi")
        'Else
        '    username_Agrea = ""
        'End If

        ''PWD
        'If ConfigurationSettings.AppSettings("pwdWSAnagrafe_Coldi") IsNot Nothing Then
        '    password_Anagrafe = ConfigurationSettings.AppSettings("pwdWSAnagrafe_Coldi")
        'Else
        '    password_Anagrafe = ""
        'End If
        'If ConfigurationSettings.AppSettings("pwdWSAgrea_Coldi") IsNot Nothing Then
        '    password_Agrea = ConfigurationSettings.AppSettings("pwdWSAgrea_Coldi")
        'Else
        '    password_Agrea = ""
        'End If
        'If ConfigurationSettings.AppSettings("limite_Giornaliero") IsNot Nothing Then
        '    limiteGiornaliero = ConfigurationSettings.AppSettings("limite_Giornaliero")
        'Else
        '    limiteGiornaliero = 0
        'End If

    End Sub

#End Region

End Class