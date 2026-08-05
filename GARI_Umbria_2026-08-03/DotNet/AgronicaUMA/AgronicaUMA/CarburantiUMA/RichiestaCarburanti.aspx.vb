Imports System.IO
Imports System.Text
Imports System.Transactions
Imports System.Web
Imports System.Web.Caching
Imports System.Web.Services
Imports AgronicaCoreContabDAL
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreGestioneRichieste
Imports AgronicaCoreModello.ParametriAgenda_Temp
Imports AgronicaCoreModelsSTD.exceptions
Imports AgronicaCoreUmaBiz
Imports AgronicaCoreUmaDal
Imports AgronicaCoreUtentiDAL
Imports AgronicaCoreUtility
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreXML.XML_Stampe
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

Public Class RichiestaCarburanti
    Inherits System.Web.UI.Page
    Private Const V As String = "[]"

    Public QS_Piva As String = ""
    Public QS_Richiesta As Integer = -1
    Public QS_Fp As Integer = 0
    Public QS_Type As Integer = 0
    Public QS_Avanzamento As Integer = 0
    Public QS_Anticipo As Integer = 0
    Public QS_TipoAzienda As Integer = enum_TipoAzienda_UMA.Azienda_Agricola_Privata
    Public QS_PivaCod As String = ""
    Public QS_Programmazione_Cod_Fascicolo As Integer = 0
    Public QS_TipoOp As Integer = enum_TipoOperazioneDB.Scrittura
    Private QS_Provenienza As Integer = 0
    Public QS_UsaAnalisiTerrenoNG As Integer = 0

    'COSTANTI

    Public Const DOCUMENTALE As Integer = 1
    Public Const ANAGRAFICA As Integer = 2

    Public Const CodiceFascicoloColtureNonImputabili As Integer = -1
    Public Const CodiceFascicoloAnticipi As Integer = -2
    Public Const CodiceFascicoloTrasferimenti As Integer = -3
    Public Const CodiceFascicoloPianoColturaleGrafico As Integer = -4

    Public Const DescrizioneFascicoloColtureNonImputabili As String = "Colture non imputabili al Fascicolo"
    Public Const DescrizioneFascicoloAnticipi As String = "Anticipi"
    Public Const DescrizioneFascicoloTrasferimenti As String = "Trasferimenti"
    Public Const DescrizioneFascicoloPianoColturaleGrafico As String = "Piano Colturale"

    Public Const MacrousoCod_ColtivazioniSottoSerra As String = "1034"
    Public Const MacrousoCod_TrasformazioneLatte As String = "1035"
    Public Const MacrousoCod_TrasformazioneOliveOlio As String = "1036"
    Public Const MacrousoCod_TrasformazioneCarciofi As String = "1037"
    Public Const MacrousoCod_TrasformazioneProdottiOrtofrutticoli As String = "1038"
    Public Const MacrousoCod_ConsorziBonificaIrrigazione As String = "1040"
    Public Const MacrousoCod_LavoriStraordinari As String = "9994"

    Public Const MacrousoCod_AnticipazioniColturali As String = "9997"
    Public Const MacrousoCod_EccedenzaAnticipi As String = "9996"
    Public Const MacrousoCod_TrasferimentiEffettuati As String = "9995"

    Public Const lavorazioneCod_QuotaAnticipoEccedente As String = "10236"
    Public Const lavorazioneCod_QuotaAnticipoEccedenteAccisePagate As String = "10237"
    Public Const lavorazioneCod_QuotaTrasferita As String = "10350"

    Protected Overrides Sub InitializeCulture()
        MyBase.InitializeCulture()
        Lingua.Gias_InizializzaCultura_DaSession()
    End Sub

    Dim objParametri_Server, objParametri_Utenti As AgronicaCoreParametri
    Public prepiv As String
    Dim objParametriAgenda As ParametriAgenda
    Dim objAnalisiModelloUtils As AgronicaCoreAnagrafeBIZ.Analisi_Modello_Utility

    Private Sub inizializzoObjParametri()
        '---
        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))
        objParametri_Utenti = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))
        'objparametri_server_string = AgronicaCoreDataProvider.Utility.convertOBJparametritoString(objParametri_Server)
        objAnalisiModelloUtils = New AgronicaCoreAnagrafeBIZ.Analisi_Modello_Utility

        '---
    End Sub

    Private Sub AnnullaTutto(ByVal sender As Object, ByVal e As System.EventArgs)

        'Redirect al sito agenda
        Dim objAgenda As New AgronicaCoreGestioneRichieste.ParametriAgenda_2010
        objAgenda.PaginaRichiesta = enum_PagineAgenda_2010.Menu
        objAgenda.Piva = QS_Piva
        Dim link As String = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_SitoAgenda_PassandoDirettamente_ParametriAgenda_2010(Enum_SiteRedirector.Sito_AgronicaAgenda_2010, objAgenda)


        Dim TargetUrl As String

        If Not IsNothing(Request.QueryString("fp")) Then
            QS_Fp = CInt(Request.QueryString("fp").ToString)
        End If

        If Master.flag_MenuBS_2017 Then
            Select Case QS_Fp
                Case enum_PagineAgenda_2010.Pagina_UMA_Elenco
                    TargetUrl = "./ElencoRichieste.aspx?p=" & If(QS_Provenienza = 2, "&avanzamento=" & 2, If(QS_Avanzamento <> 0, "&avanzamento=" & QS_Avanzamento, ""))
                    Response.Redirect(TargetUrl)
                Case Else
                    Response.Redirect(link)
            End Select
        Else
            Response.Redirect(link)
        End If
    End Sub

#Region "Caricamenti"

    <WebMethod(EnableSession:=True)>
    Public Shared Function LeggiNumeroIscrizioneCdC(ByVal piva As String) As RispostaStandard

        Dim imprese_codici As New AgronicaCoreAnagrafeDAL.Imprese_Codici_Read
        Dim r As New RispostaStandard
        Dim nIscrizione As Integer
        Dim Dtres As New DataTable
        Try

            Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
                r.Sessione = False
                Return r
            End If

            Lingua.Gias_InizializzaCultura_DaSession()

            nIscrizione = imprese_codici.LeggiNumeroIscrizioneCameraDiCommercio(piva, objParametri_Server).Rows.Count

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            r.RispostaStringa = JsonConvert.SerializeObject(nIscrizione, Formatting.None, serializerSettings)

            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
            AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function LeggiNoProssimaRichiesta(ByVal richiesta_Cod As Integer) As RispostaStandard

        Dim testata As New AgronicaCoreUmaDal.UMA_Richieste_Testata_R
        Dim r As New RispostaStandard
        Dim noProxRichiesta As Integer
        Try

            Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
                r.Sessione = False
                Return r
            End If

            Lingua.Gias_InizializzaCultura_DaSession()

            noProxRichiesta = testata.LeggiDaRichiestaCod(richiesta_Cod, objParametri_Server).Rows.Item(0).Item("Rinuncia_Nuova_Richiesta_Anno_Successivo")

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            r.RispostaStringa = JsonConvert.SerializeObject(noProxRichiesta, Formatting.None, serializerSettings)

            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
            AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function SalvaNoProssimaRichiesta(ByVal richiesta_Cod As Integer,
                                                    ByVal noProxRichiesta As Boolean,
                                                    ByVal isTerzista As Boolean) As RispostaStandard

        Dim testataR As New AgronicaCoreUmaDal.UMA_Richieste_Testata_R
        Dim testataW As New AgronicaCoreUmaDal.UMA_Richieste_Testata_W
        Dim r As New RispostaStandard
        Dim Dtres As New DataTable

        Try

            Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
                r.Sessione = False
                Return r
            End If

            Lingua.Gias_InizializzaCultura_DaSession()

            Dim testataRichiesta As DataTable = testataR.Leggi("",
                                                               richiesta_Cod,
                                                               0,
                                                               AGRODATAINIZIO,
                                                               AGRODATAFINE,
                                                               0,
                                                               "",
                                                               -10,
                                                               objParametri_Server,
                                                               isTerzista:=isTerzista)

            Dim aggiornaRinunciaNuovaRichiestaAnnoSuccessivo = True

            If noProxRichiesta Then

                Dim richiesteSuccessive As DataTable = testataR.Leggi(testataRichiesta.Rows.Item(0).Item("Piva"),
                                                                      0,
                                                                      0,
                                                                      AGRODATAINIZIO,
                                                                      AGRODATAFINE,
                                                                      testataRichiesta.Rows.Item(0).Item("Anno") + 1,
                                                                      "",
                                                                      -2,
                                                                      objParametri_Server,
                                                                      testataRichiesta.Rows.Item(0).Item("Tipo_Richiesta") = enum_UMA_TipoRichiesta.Conto_Terzi)

                If richiesteSuccessive.Rows.Count > 0 Then
                    aggiornaRinunciaNuovaRichiestaAnnoSuccessivo = False
                End If

            End If

            If aggiornaRinunciaNuovaRichiestaAnnoSuccessivo Then

                testataW.SalvaNoProssimaRichiesta(richiesta_Cod, noProxRichiesta, objParametri_Server)

                Dim serializerSettings As New JsonSerializerSettings()
                serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                r.RispostaStringa = JsonConvert.SerializeObject(noProxRichiesta, Formatting.None, serializerSettings)

                r.RispostaOK = True

            Else

                r.RispostaOK = False
                r.Errore = "Impossibile impostare la cessazione dell'azienda a causa della presenza di anticipi o richieste per l'anno successivo"

            End If

        Catch ex As Exception

            r.RispostaOK = False
            'Uso questa funzione per ottenere il messaggio
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
            AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function
    <WebMethod(EnableSession:=True)>
    Public Shared Function Leggi_TrasferimentiCarburanti(ByVal piva As String, ByVal Richiesta_Cod As Integer) As RispostaStandard

        Dim r As New RispostaStandard
        Dim leggiLav As New UMA_Richieste_Trasferimenti_BIZ

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Utenti) Then
            r.Sessione = False
            Return r
        End If

        Lingua.Gias_InizializzaCultura_DaSession()

        Dim serializerSettings As New JsonSerializerSettings()
        serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore

        Dim dtL As DataTable = leggiLav.Leggi(piva, Richiesta_Cod, objParametri_Server)


        r.RispostaStringa = JsonConvert.SerializeObject(dtL, Formatting.None, serializerSettings)
        r.RispostaOK = True
        Return r


    End Function
    <WebMethod(EnableSession:=True)>
    Public Shared Function Salva_Trasferimenti(ByVal righeCancellate As String,
                                                       ByVal righeInserite As String,
                                                       ByVal righeModificate As String) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If

        Try
            ''' Salva le righe nel DB '''
            Dim biz As New UMA_Richieste_Trasferimenti_BIZ

            Return biz.Salva_Trasferimenti(objParametriServer, righeInserite, righeModificate, righeCancellate)
        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "ErroreDuranteOperazione_" & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            Return r
        End Try
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Salva_Restituzioni(ByVal righeCancellate As String,
                                                       ByVal righeInserite As String,
                                                       ByVal righeModificate As String) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If

        Try
            ''' Salva le righe nel DB '''
            Dim biz As New UMA_Richieste_Restituzioni_BIZ

            Return biz.Salva_Restituzioni(objParametriServer, righeInserite, righeModificate, righeCancellate)
        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "ErroreDuranteOperazione_" & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            Return r
        End Try
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Salva_Anticipi(ByVal richiestaCod As Integer,
                                          ByVal percAnticipo As Decimal,
                                          ByVal gasolioAnticipo As Decimal,
                                          ByVal benzinaAnticipo As Decimal,
                                          ByVal gasolioSerraAnticipo As Decimal) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If

        Try
            Dim biz As New UMA_Richieste_Anticipi_BIZ

            Dim err = biz.ModificaAnticipo(richiestaCod, percAnticipo, gasolioAnticipo, benzinaAnticipo, gasolioSerraAnticipo, objParametriServer)

            If err.Equals("") Then
                r.RispostaOK = True
                r.RispostaStringa = "Anticipo aggiornato correttamente"
            Else
                r.RispostaOK = False
                r.Errore = err
            End If

            Return r

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "ErroreDuranteOperazione_" & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            Return r
        End Try
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function ElencoLavUMA(Macrouso_UMA_Cod As String,
                                        Terzista As Integer,
                                        Utilizzata_Da_Consorzio_Bonifica As Integer,
                                        Anno As String,
                                        Regolamento_Cod As Integer) As RispostaStandard

        Dim r As New RispostaStandard
        Dim Dt As New DataTable
        Dim Dtres As New DataTable
        Dim leggiLavorazioni As New AgronicaCoreUmaDal.UMA_Configurazione_MacrousixLavorazioni_R

        Try

            Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
                r.Sessione = False
                Return r
            End If

            Lingua.Gias_InizializzaCultura_DaSession()

            Dim cache As New Cache
            If HttpContext.Current.Cache IsNot Nothing Then
                cache = HttpContext.Current.Cache
            End If

            Dim key = objParametri_Server.PivaSuperUser & "UMA_ElencoLavUMA_" & Macrouso_UMA_Cod & "_" & Terzista & "_" & Utilizzata_Da_Consorzio_Bonifica & "_" & Anno & "_" & Regolamento_Cod

            If cache.Get(key) Is Nothing Then

                Dim validitaAnno = GetValiditaAnno(Anno)

                Dt = leggiLavorazioni.Leggi("",
                                            Macrouso_UMA_Cod,
                                            "",
                                            0,
                                            0,
                                            objParametri_Server,
                                            validitaInizio:=validitaAnno.Inizio,
                                            validitaFine:=validitaAnno.Fine,
                                            regolamentoCod:=Regolamento_Cod)

                If Dt.Rows.Count > 0 Then

                    If Terzista = 1 Then
                        Dt = Dt.Select(" GestioneTerzista = 1 ").CopyToDataTable()
                    End If

                    If Utilizzata_Da_Consorzio_Bonifica = 1 Then
                        If Dt.Select(" Utilizzata_Da_Consorzio_Bonifica = 1 ").Count > 0 Then
                            Dt = Dt.Select(" Utilizzata_Da_Consorzio_Bonifica = 1 ").CopyToDataTable()
                        Else
                            Dt.Clear()
                        End If
                    End If

                End If

                Dtres.Columns.Add(New DataColumn("LavUMA", GetType(String)))
                Dtres.Columns.Add(New DataColumn("Lav_UMA_Cod", GetType(String)))
                Dtres.Columns.Add(New DataColumn("LavGIAS", GetType(String)))
                Dtres.Columns.Add(New DataColumn("LAV_COD", GetType(String)))
                Dtres.Columns.Add(New DataColumn("Regolamento_Cod", GetType(Integer)))

                Dim dRow As DataRow

                For Each dr As DataRow In Dt.Rows

                    If Dtres.Select(" Lav_UMA_Cod = '" & dr.Item("Lav_UMA_Cod") & "' and  Regolamento_Cod = " & dr.Item("Regolamento_Cod") & " ").Length = 0 Then

                        dRow = Dtres.NewRow

                        Dim numMaxOp = 0

                        If Not IsDBNull(dr.Item("N_Max_Operazioni")) Then
                            numMaxOp = dr.Item("N_Max_Operazioni")
                        End If

                        If numMaxOp > 1 Then
                            dRow("LavUMA") = dr.Item("Lav_UMA_Des") & " (MAX " & numMaxOp.ToString() & " VOLTE)"
                        Else
                            dRow("LavUMA") = dr.Item("Lav_UMA_Des")
                        End If

                        dRow("Lav_UMA_Cod") = dr.Item("Lav_UMA_Cod")
                        dRow("LavGIAS") = dr.Item("LAV_DES")
                        dRow("LAV_COD") = dr.Item("LAV_COD")
                        dRow("Regolamento_Cod") = dr.Item("Regolamento_Cod")

                        If Not IsDBNull(dr.Item("Udm_Alternativa")) AndAlso CStr(dr.Item("Udm_Alternativa")) <> "" Then
                            dRow("LavUMA") = dRow("LavUMA") & " [" & CStr(dr.Item("Udm_Alternativa")) & "]"
                        End If

                        Dtres.Rows.Add(dRow)

                    End If

                Next

                cache(key) = Dtres

            Else

                Dtres = cache(key)

            End If

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            r.RispostaStringa = JsonConvert.SerializeObject(Dtres, Formatting.None, serializerSettings)

            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
            AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function ElencoLavGIAS(Macrouso_UMA_Cod As String,
                                         Regolamento_Cod As Integer,
                                         Lav_UMA_Cod As String,
                                         Anno As String) As RispostaStandard

        Dim r As New RispostaStandard
        Dim Dt As New DataTable
        Dim Dtres As New DataTable
        Dim leggiLavorazioni As New AgronicaCoreUmaDal.UMA_Configurazione_MacrousixLavorazioni_R

        Try

            Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
                r.Sessione = False
                Return r
            End If

            Lingua.Gias_InizializzaCultura_DaSession()

            Dim cache As New Cache
            If HttpContext.Current.Cache IsNot Nothing Then
                cache = HttpContext.Current.Cache
            End If

            Dim key = objParametri_Server.PivaSuperUser & "UMA_ElencoLavGIAS_" & Macrouso_UMA_Cod & "_" & Regolamento_Cod.ToString() & "_" & Lav_UMA_Cod & "_" & Anno

            If cache.Get(key) Is Nothing Then

                Dim validitaAnno = GetValiditaAnno(Anno)

                Dt = leggiLavorazioni.Leggi("",
                                            Macrouso_UMA_Cod,
                                            Lav_UMA_Cod,
                                            0,
                                            0,
                                            objParametri_Server,
                                            validitaInizio:=validitaAnno.Inizio,
                                            validitaFine:=validitaAnno.Fine,
                                            regolamentoCod:=Regolamento_Cod)

                Dtres.Columns.Add(New DataColumn("LavGIAS", GetType(String)))
                Dtres.Columns.Add(New DataColumn("LAV_COD", GetType(String)))

                Dim dRow As DataRow

                For Each dr As DataRow In Dt.Rows
                    dRow = Dtres.NewRow
                    dRow("LavGIAS") = dr.Item("LAV_DES")
                    dRow("LAV_COD") = dr.Item("LAV_COD")
                    Dtres.Rows.Add(dRow)
                Next

                cache(key) = Dtres

            Else

                Dtres = cache(key)

            End If

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            r.RispostaStringa = JsonConvert.SerializeObject(Dtres, Formatting.None, serializerSettings)

            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
            AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function ElencoAttivitaGIAS(Macrouso_UMA_Cod As String,
                                              Regolamento_Cod As Integer,
                                              Lav_UMA_Cod As String,
                                              Lav_Cod As Integer,
                                              Anno As String) As RispostaStandard

        Dim r As New RispostaStandard
        Dim Dt As New DataTable
        Dim Dtres As New DataTable
        Dim leggiLavorazioni As New AgronicaCoreUmaDal.UMA_Configurazione_MacrousixLavorazioni_R

        Try

            Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
                r.Sessione = False
                Return r
            End If

            Lingua.Gias_InizializzaCultura_DaSession()

            Dim validitaAnno = GetValiditaAnno(Anno)

            Dt = leggiLavorazioni.Leggi("",
                                        Macrouso_UMA_Cod,
                                        Lav_UMA_Cod,
                                        Lav_Cod,
                                        0,
                                        objParametri_Server,
                                        validitaInizio:=validitaAnno.Inizio,
                                        validitaFine:=validitaAnno.Fine,
                                        regolamentoCod:=Regolamento_Cod)

            Dtres.Columns.Add(New DataColumn("Attivita_Des", GetType(String)))
            Dtres.Columns.Add(New DataColumn("Attivita_Cod", GetType(String)))

            Dim dRow As DataRow

            For Each dr As DataRow In Dt.Rows
                If Not IsDBNull(dr.Item("Desc")) Then
                    dRow = Dtres.NewRow
                    dRow("Attivita_Des") = dr.Item("Desc")
                    dRow("Attivita_Cod") = dr.Item("Id_Attivita")
                    Dtres.Rows.Add(dRow)
                End If

            Next

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            r.RispostaStringa = JsonConvert.SerializeObject(Dtres, Formatting.None, serializerSettings)

            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
            AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function ElencoLavValidita(Anno As String) As RispostaStandard

        Dim r As New RispostaStandard
        Dim Dt As New DataTable
        Dim leggiLavorazioni As New AgronicaCoreUmaDal.UMA_Configurazione_MacrousixLavorazioni_R

        Try

            Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
                r.Sessione = False
                Return r
            End If

            Lingua.Gias_InizializzaCultura_DaSession()

            Dim validitaAnno = GetValiditaAnno(Anno)

            Dt = leggiLavorazioni.Leggi("",
                                        "",
                                        "",
                                        0,
                                        0,
                                        objParametri_Server,
                                        validitaInizio:=validitaAnno.Inizio,
                                        validitaFine:=validitaAnno.Fine)

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            r.RispostaStringa = JsonConvert.SerializeObject(Dt, Formatting.None, serializerSettings)

            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
            AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function RecuperaDataPassaggioDiStato(ByVal richiesta_Cod As Integer) As RispostaStandard

        Dim r As New RispostaStandard
        Dim leggiTestata As New AgronicaCoreUmaDal.UMA_Richieste_Testata_R

        Try

            If (richiesta_Cod > 0) Then
                Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
                If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
                    r.Sessione = False
                    Return r
                End If

                Lingua.Gias_InizializzaCultura_DaSession()

                Dim Data = leggiTestata.RecuperaDataPassaggioDiStato(richiesta_Cod, objParametri_Server)

                Dim serializerSettings As New JsonSerializerSettings()
                serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                r.RispostaStringa = JsonConvert.SerializeObject(Data, Formatting.None, serializerSettings)
            End If

            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
            AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function CalcoloCosti(Regione_Cod As String,
                                        Anno As String) As RispostaStandard

        Dim r As New RispostaStandard
        Dim Dt As New DataTable
        Dim leggiLavorazioni As New AgronicaCoreUmaDal.UMA_Configurazione_MacrousixLavorazioni_R

        Try

            Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
                r.Sessione = False
                Return r
            End If

            Lingua.Gias_InizializzaCultura_DaSession()

            Dim cache As New Cache
            If HttpContext.Current.Cache IsNot Nothing Then
                cache = HttpContext.Current.Cache
            End If

            Dim key = objParametri_Server.PivaSuperUser & "_UMA_CalcoloCosti_" & Regione_Cod & "_" & Anno

            If cache.Get(key) Is Nothing Then

                Dim validitaAnno = GetValiditaAnno(Anno)

                Dt = leggiLavorazioni.Leggi(Regione_Cod,
                                            "",
                                            "",
                                            0,
                                            0,
                                            objParametri_Server,
                                            validitaInizio:=validitaAnno.Inizio,
                                            validitaFine:=validitaAnno.Fine)

                cache(key) = Dt

            Else

                Dt = cache(key)

            End If

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            r.RispostaStringa = JsonConvert.SerializeObject(Dt, Formatting.None, serializerSettings)

            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
            AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function

    Private Shared Function GetValiditaAnno(ByVal anno As String) As ValiditaAnno

        Dim validitaAnno As New ValiditaAnno

        validitaAnno.Inizio = AGRODATAINIZIO
        validitaAnno.Fine = AGRODATAFINE

        If IsNumeric(anno) Then
            validitaAnno.Inizio = New DateTime(anno, 1, 1)
            validitaAnno.Fine = New DateTime(anno, 12, 31)
        End If

        Return validitaAnno

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function ElencoCarburanti() As RispostaStandard

        Dim r As New RispostaStandard
        Dim Dt As New DataTable
        Dim Dtres As New DataTable
        Dim leggiCarb As New AgronicaCoreMetaSchemaDAL.Carburanti_R

        Try

            Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
                r.Sessione = False
                Return r
            End If

            Lingua.Gias_InizializzaCultura_DaSession()

            Dt = leggiCarb.Leggi(0, #1/1/2000 12:00 PM#, DateTime.Now, "Car_Cod IN (" &
                                 enum_TipoCarburante_UMA.Gasolio & ", " &
                                 enum_TipoCarburante_UMA.Benzina & ", " &
                                 enum_TipoCarburante_UMA.Gasolio_Serra & ", " &
                                 enum_TipoCarburante_UMA.Elettricita & ", " &
                                 enum_TipoCarburante_UMA.Carburante_Non_Agricolo & ")", "", objParametri_Server)

            Dtres.Columns.Add(New DataColumn("TipoCarb", GetType(String)))
            Dtres.Columns.Add(New DataColumn("Car_Cod", GetType(String)))

            Dim dRow As DataRow

            For Each dr As DataRow In Dt.Rows
                dRow = Dtres.NewRow
                dRow("TipoCarb") = dr.Item("Car_Des")
                dRow("Car_Cod") = dr.Item("Car_Cod")
                Dtres.Rows.Add(dRow)
            Next

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            r.RispostaStringa = JsonConvert.SerializeObject(Dtres, Formatting.None, serializerSettings)

            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
            AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Terzista_Abilitato_Per_Integrazione(ByVal piva As String,
                                           ByVal anno As Integer) As RispostaStandard

        Dim r As New RispostaStandard
        Dim Dt As New DataTable
        Dim leggiTestate As New AgronicaCoreUmaDal.UMA_Richieste_Testata_R
        Dim leggiVendite As New AgronicaCoreUmaDal.UMA_Vendite_R
        Dim setup As New AgronicaCoreUmaDal.UMASetup_R
        Dim abilitato As Boolean = False
        Dim totaleRichiesto As Decimal = 0
        Dim totaleApprovato As Decimal = 0
        Dim totalePrelevato As Decimal = 0
        Dim percentualePrelievo As Decimal = 0
        Dim totaleRimanenze As Decimal = 0

        Try

            Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
                r.Sessione = False
                Return r
            End If

            Lingua.Gias_InizializzaCultura_DaSession()

            Dt = leggiTestate.Leggi(piva, 0, 0, AGRODATAINIZIO, AGRODATAFINE, anno, "", 0, objParametri_Server, isTerzista:=-1, xFiltroAggiuntivo:=" 
                                    Pratiche_Stati_Attuali.Stato_Cod NOT IN ( " & enum_WAnagraficaStati.Verifica_Intermedia_Non_Superata & ", " & enum_WAnagraficaStati.Rinuncia & " ) ")

            For Each row As DataRow In Dt.Rows
                Dim Approvazione_Gasolio As Double = 0
                Dim Approvazione_Benzina As Double = 0
                Dim Approvazione_Gasolio_Serra As Double = 0

                If Not IsDBNull(row("Approvazione_Iniziale_Gasolio")) Then
                    Approvazione_Gasolio = row("Approvazione_Iniziale_Gasolio")
                End If

                If Not IsDBNull(row("Approvazione_Iniziale_Benzina")) Then
                    Approvazione_Benzina = row("Approvazione_Iniziale_Benzina")
                End If

                If Not IsDBNull(row("Approvazione_Iniziale_Gasolio_Serra")) Then
                    Approvazione_Gasolio_Serra = row("Approvazione_Iniziale_Gasolio_Serra")
                End If

                Dim Richiesta_Iniziale_Gasolio As Double = 0
                Dim Richiesta_Iniziale_Benzina As Double = 0
                Dim Richiesta_Iniziale_Gasolio_Serra As Double = 0

                If Not IsDBNull(row("Richiesta_Iniziale_Gasolio")) Then
                    Richiesta_Iniziale_Gasolio = row("Richiesta_Iniziale_Gasolio")
                End If

                If Not IsDBNull(row("Richiesta_Iniziale_Benzina")) Then
                    Richiesta_Iniziale_Benzina = row("Richiesta_Iniziale_Benzina")
                End If

                If Not IsDBNull(row("Richiesta_Iniziale_Gasolio_Serra")) Then
                    Richiesta_Iniziale_Gasolio_Serra = row("Richiesta_Iniziale_Gasolio_Serra")
                End If

                Dim Rimanenza_Gasolio As Double = 0
                Dim Rimanenza_Benzina As Double = 0
                Dim Rimanenza_Gasolio_Serra As Double = 0

                If Not IsDBNull(row("Rimanenza_Gasolio")) Then
                    Rimanenza_Gasolio = row("Rimanenza_Gasolio")
                End If

                If Not IsDBNull(row("Rimanenza_Benzina")) Then
                    Rimanenza_Benzina = row("Rimanenza_Benzina")
                End If

                If Not IsDBNull(row("Rimanenza_Gasolio_Serra")) Then
                    Rimanenza_Gasolio_Serra = row("Rimanenza_Gasolio_Serra")
                End If

                totaleRichiesto += Richiesta_Iniziale_Gasolio + Richiesta_Iniziale_Benzina + Richiesta_Iniziale_Gasolio_Serra
                totaleApprovato += Approvazione_Gasolio + Approvazione_Benzina + Approvazione_Gasolio_Serra
                totaleRimanenze += Rimanenza_Gasolio + Rimanenza_Benzina + Rimanenza_Gasolio_Serra
            Next

            totaleApprovato -= totaleRimanenze

            Dim carburanteVendutoRows = leggiVendite.LeggiCarburanteVenduto(piva, anno, -1, "", objParametri_Server).Rows

            If (carburanteVendutoRows.Count = 0) Then
                totalePrelevato = 0
            Else
                totalePrelevato = carburanteVendutoRows.Item(0).Item("Totale_Carb")
            End If

            percentualePrelievo = setup.LeggiSetup(anno, objParametri_Server).Rows.Item(0).Item("Percentuale_Integrazione_Terzista")

            If totaleApprovato < 0 Then
                totaleApprovato = 0
            End If

            If (totaleApprovato > 0) Then
                abilitato = ((totalePrelevato * 100) / totaleApprovato) >= percentualePrelievo
            Else
                abilitato = True
            End If

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            r.RispostaStringa = JsonConvert.SerializeObject(abilitato & "|" & percentualePrelievo.ToString(), Formatting.None, serializerSettings)
            'r.RispostaStringa = JsonConvert.SerializeObject(abilitato, Formatting.None, serializerSettings)

            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
            AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function CheckPrimaRichiesta(ByVal piva As String,
                                            ByVal richiestaAvanz As Integer,
                                            ByVal richiestaCod As Integer,
                                            ByVal anno As Integer,
                                            ByVal terzista As Boolean) As RispostaStandard

        Dim r As New RispostaStandard
        Dim Dt As New DataTable
        Dim leggiTestate As New AgronicaCoreUmaDal.UMA_Richieste_Testata_R
        Dim res As Boolean

        Try

            Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
                r.Sessione = False
                Return r
            End If

            Lingua.Gias_InizializzaCultura_DaSession()

            Dt = leggiTestate.Leggi(piva, richiestaCod, 0, AGRODATAINIZIO, AGRODATAFINE, objParametri_Server, isTerzista:=IIf(terzista, -1, 0), anno:=anno, avanzamento:=richiestaAvanz, xOrderBy:=" t.Data_Creazione ASC ")

            res = True

            If (Dt.Rows.Count > 0) Then

                res = IIf(IIf(IsDBNull(Dt.Rows.Item(0).Item("Richiesta_Integrativa")), 0, Dt.Rows.Item(0).Item("Richiesta_Integrativa")) = 0, True, False)

            End If

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            r.RispostaStringa = JsonConvert.SerializeObject(res, Formatting.None, serializerSettings)

            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
            AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function CheckPerc(ByVal anno As Integer) As RispostaStandard

        Dim r As New RispostaStandard
        Dim setup As New AgronicaCoreUmaDal.UMASetup_R
        Dim Dt As DataTable
        Dim perc As Integer

        Dim serializerSettings As New JsonSerializerSettings()
        serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore

        Try

            Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
                r.Sessione = False
                Return r
            End If

            Lingua.Gias_InizializzaCultura_DaSession()

            Dt = setup.LeggiSetup(anno, objParametri_Server)

            If Dt.Rows.Count > 0 Then
                perc = Dt.Rows.Item(0).Item("Per_Riduzione")
            Else
                perc = 23
            End If

            r.RispostaStringa = JsonConvert.SerializeObject(perc, Formatting.None, serializerSettings)

            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "ErroreDuranteOperazione_" & vbCrLf &
            AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function

    ''' <summary>
    ''' Funzione ideata per restituire tutte le lavorazioni eseguite sugli stessi terreni dell'azienda chiamante
    ''' </summary>
    ''' <param name="piva"> P.IVA dell'azienda su cui viene eseguito il controllo</param>
    ''' <param name="programmazione_cod"> Il codice del fascicolo utilizzato come filtro nel controllo</param>
    ''' <param name="pivaChiamante"> P.IVA dell'azienda che sta operando al momento del lancio del controllo</param>
    ''' <param name="gruppo_colturale"> Il codice del gruppo colturale utilizzato come filtro nel controllo</param>
    ''' <param name="anno"> Anno della richiesta/rendicontazione</param>
    ''' <param name="isTerzista"> True se la richiesta/rendicontazione è un conto terzi (Impresa Agromeccanica o Cooperativa), False se è un Conto Proprio</param>
    ''' <param name="avanzamento"> 0 per le richieste, 1 per le rendicontazioni</param>
    ''' <param name="integrativa"> True se si tratta di una richiesta integrativa, False altrimenti</param>
    ''' <returns></returns>
    ''' 
    <WebMethod(EnableSession:=True)>
    Public Shared Function Leggi_Lavorazioni_Controllo_Incrociato(ByVal piva As String,
                                                                  ByVal programmazione_cod As Integer,
                                                                  ByVal pivaChiamante As String,
                                                                  ByVal gruppo_colturale As Integer,
                                                                  ByVal anno As Integer,
                                                                  ByVal isTerzista As Boolean,
                                                                  ByVal avanzamento As Integer,
                                                                  ByVal integrativa As Boolean,
                                                                  ByVal soloContoTerzi As Boolean,
                                                                  ByVal richiesta_cod As Integer) As RispostaStandard

        Dim r As New RispostaStandard
        Dim Dt As DataTable
        Dim leggi As New AgronicaCoreUmaDal.UMA_Richieste_Lavorazioni_R

        Try

            Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
                r.Sessione = False
                Return r
            End If

            Lingua.Gias_InizializzaCultura_DaSession()

            Dt = leggi.Leggi_Per_Controllo_Incrociato(piva,
                                                      programmazione_cod,
                                                      pivaChiamante,
                                                      gruppo_colturale,
                                                      anno,
                                                      "",
                                                      "",
                                                      objParametri_Server,
                                                      isTerzista,
                                                      avanzamento:=avanzamento,
                                                      integrativa:=integrativa,
                                                      soloContoTerzi:=soloContoTerzi,
                                                      richiesta_Cod:=richiesta_cod)

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            r.RispostaStringa = JsonConvert.SerializeObject(Dt, Formatting.None, serializerSettings)
            r.RispostaOK = True

            'GLORIA
            If (programmazione_cod = 0) Then
                'AGGIUNGERE LA PARTE PER LA CREAZIONE DELLA TABELLA

            End If

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
            AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Leggi_Anticipazioni_Colturali(ByVal piva As String,
                                                         ByVal anno As Integer) As RispostaStandard

        Dim r As New RispostaStandard
        Dim Dt As DataTable
        Dim leggi As New AgronicaCoreUmaDal.UMA_Richieste_Testata_R

        Try

            Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
                r.Sessione = False
                Return r
            End If

            Lingua.Gias_InizializzaCultura_DaSession()

            Dt = leggi.RecuperaAnticipazioniColturaliTerzisti(piva, anno, objParametri_Server, True)

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            r.RispostaStringa = JsonConvert.SerializeObject(Dt, Formatting.None, serializerSettings)
            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
            AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function


    'Inizio Gloria
    <WebMethod(EnableSession:=True)>
    Public Shared Function Leggi_Richieste_Lavorazioni_Testa(ByVal piva As String,
                                                                  ByVal programmazione_cod As Integer,
                                                                  ByVal pivaChiamante As String,
                                                                  ByVal gruppo_colturale As Integer,
                                                                  ByVal anno As Integer,
                                                                  isTerzista As Boolean) As RispostaStandard

        Dim r As New RispostaStandard
        Dim Dt As New DataTable
        Dim leggi As New AgronicaCoreUmaDal.UMA_Richieste_Lavorazioni_R

        Try

            Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
                r.Sessione = False
                Return r
            End If

            Lingua.Gias_InizializzaCultura_DaSession()

            Dt = leggi.Leggi_Per_Controllo_Incrociato(piva, programmazione_cod, pivaChiamante, gruppo_colturale, anno, "", "", objParametri_Server, isTerzista)

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            r.RispostaStringa = JsonConvert.SerializeObject(Dt, Formatting.None, serializerSettings)
            r.RispostaOK = True


            'GLORIA
            If (programmazione_cod = 0) Then
                'AGGIUNGERE LA PARTE PER LA CREAZIONE DELLA TABELLA

            End If
        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
            AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    'Fine Gloria

    'scelta l'azienda con una testata relativa, carico le varie richieste da planning e le confronto con quelle presenti nel db

    <WebMethod(EnableSession:=True)>
    Public Shared Function Trova_Richieste(ByVal piva As String,
                                           ByVal richiesta_cod As Integer,
                                           ByVal check As Boolean,
                                           ByVal Programmazione_Cod As Integer,
                                           ByVal avanzamento As Integer) As RispostaStandard

        Dim r As New RispostaStandard
        Dim testate As New AgronicaCoreUmaDal.UMA_Richieste_Testata_R
        Dim res As Tuple(Of DataTable, String)

        Dim leggi As New CDG_DAL_R

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")

        Dim objParametri_Super_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Super_Server")


        Try

            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
                r.Sessione = False
                Return r
            End If

            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Super_Server) Then
                r.Sessione = False
                Return r
            End If

            Lingua.Gias_InizializzaCultura_DaSession()

            If Programmazione_Cod = CodiceFascicoloPianoColturaleGrafico Then

                res = TrovaRichiesteDal2025InPoi(piva, richiesta_cod, check, avanzamento, objParametri_Server, objParametri_Super_Server)

            Else

                'La risposta è una tupla contenente la Datatable da visualizzare e un ipotetico errore generato durante la costruzione della tabella,
                'se l'errore è vuoto, allora è andato tutto bene
                res = TrovaRichiestePrimaDel2025(piva, richiesta_cod, check, Programmazione_Cod, avanzamento, objParametri_Server)

            End If

            If String.IsNullOrEmpty(res.Item2) Then

                Dim dt As DataTable
                dt = res.Item1

                Dim serializerSettings As New JsonSerializerSettings()
                serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                r.RispostaStringa = JsonConvert.SerializeObject(dt, Formatting.None, serializerSettings)
                r.RispostaOK = True

            Else

                r.Errore = res.Item2
                r.RispostaOK = False

            End If

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
            AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    Private Shared Function TrovaRichiesteDal2025InPoi(piva As String,
                                                       richiesta_cod As Integer,
                                                       check As Boolean,
                                                       avanzamento As Integer,
                                                       ByRef objParametri_Server As AgronicaCoreParametri,
                                                       ByRef objParametri_Super_Server As AgronicaCoreParametri) As Tuple(Of DataTable, String)

        Dim dtColture As DataTable
        Dim richieste As New AgronicaCoreUmaDal.UMA_Richieste_R
        Dim richiestexRegImpianti As New AgronicaCoreUmaDal.UMA_RichiesteXReg_Impianti_W
        Dim Dt As New DataTable
        Dim errore As String = ""
        Dim res As Tuple(Of DataTable, String)

        Dt.Columns.Add(New DataColumn("Gruppo_Colturale_UMA", GetType(String)))
        Dt.Columns.Add(New DataColumn("Macrouso_UMA_Cod", GetType(String)))
        Dt.Columns.Add(New DataColumn("Programmazione_Cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Richiesta_Cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Regolamento_Cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("sup_UMA", GetType(Decimal)))
        Dt.Columns.Add(New DataColumn("sup_UMA_A", GetType(Decimal)))
        Dt.Columns.Add(New DataColumn("sup_UMA_B", GetType(Decimal)))
        Dt.Columns.Add(New DataColumn("sup_UMA_Edit", GetType(Decimal)))
        Dt.Columns.Add(New DataColumn("sup_UMA_A_Edit", GetType(Decimal)))
        Dt.Columns.Add(New DataColumn("sup_UMA_B_Edit", GetType(Decimal)))
        Dt.Columns.Add(New DataColumn("UMA_Cod", GetType(String)))
        Dt.Columns.Add(New DataColumn("calcolato", GetType(Decimal)))
        Dt.Columns.Add(New DataColumn("richiesto", GetType(Decimal)))
        Dt.Columns.Add(New DataColumn("assegnato", GetType(Decimal)))
        Dt.Columns.Add(New DataColumn("TerrenoNormale", GetType(Decimal)))
        Dt.Columns.Add(New DataColumn("TerrenoMedio", GetType(Decimal)))
        Dt.Columns.Add(New DataColumn("TerrenoTenace", GetType(Decimal)))
        Dt.Columns.Add(New DataColumn("TerrenoNormale_Edit", GetType(Decimal)))
        Dt.Columns.Add(New DataColumn("TerrenoMedio_Edit", GetType(Decimal)))
        Dt.Columns.Add(New DataColumn("TerrenoTenace_Edit", GetType(Decimal)))
        Dt.Columns.Add(New DataColumn("Veg_Cod", GetType(String)))
        Dt.Columns.Add(New DataColumn("Id_Cod", GetType(String)))
        Dt.Columns.Add(New DataColumn("Programmazione_Des", GetType(String)))

        Dim EFrichieste As New AgronicaCoreUmaBiz.UMA_Richieste
        Dim dtRichieste As New DataTable
        Dim testate As New AgronicaCoreUmaDal.UMA_Richieste_Testata_R

        Dim dtgroup As New DataTable
        Dim d0 As DataRow

        dtgroup.Columns.Add(New DataColumn("macrouso_UMA_Cod", GetType(String)))
        dtgroup.Columns.Add(New DataColumn("macrouso_UMA_Des", GetType(String)))
        dtgroup.Columns.Add(New DataColumn("Programmazione_Cod", GetType(Integer)))
        dtgroup.Columns.Add(New DataColumn("Regolamento_Cod", GetType(Integer)))
        dtgroup.Columns.Add(New DataColumn("sup_UMA", GetType(Decimal)))
        dtgroup.Columns.Add(New DataColumn("sup_A", GetType(Decimal)))
        dtgroup.Columns.Add(New DataColumn("sup_B", GetType(Decimal)))
        dtgroup.Columns.Add(New DataColumn("sup_Normale", GetType(Decimal)))
        dtgroup.Columns.Add(New DataColumn("sup_Media", GetType(Decimal)))
        dtgroup.Columns.Add(New DataColumn("sup_Tenace", GetType(Decimal)))
        dtgroup.Columns.Add(New DataColumn("calcolato", GetType(Decimal)))
        dtgroup.Columns.Add(New DataColumn("richiesto", GetType(Decimal)))
        dtgroup.Columns.Add(New DataColumn("assegnato", GetType(Decimal)))
        dtgroup.Columns.Add(New DataColumn("Veg_Cod", GetType(String)))
        dtgroup.Columns.Add(New DataColumn("Id_Cod", GetType(String)))
        dtgroup.Columns.Add(New DataColumn("sup_UMA_Edit", GetType(Decimal)))
        dtgroup.Columns.Add(New DataColumn("sup_UMA_A_Edit", GetType(Decimal)))
        dtgroup.Columns.Add(New DataColumn("sup_UMA_B_Edit", GetType(Decimal)))
        dtgroup.Columns.Add(New DataColumn("TerrenoNormale_Edit", GetType(Decimal)))
        dtgroup.Columns.Add(New DataColumn("TerrenoMedio_Edit", GetType(Decimal)))
        dtgroup.Columns.Add(New DataColumn("TerrenoTenace_Edit", GetType(Decimal)))
        dtgroup.Columns.Add(New DataColumn("Programmazione_Des", GetType(String)))

        'recupero le richieste presenti attualmente nel db
        dtRichieste = richieste.Leggi(piva, richiesta_cod, "", 0, enumSelezioneVariabile.Selezione_TabellaCompleta, objParametri_Server)

        If dtRichieste.Rows.Count = 0 Then

            'Dim anno = testate.RecuperaAnnoRichiesta(richiesta_cod, objParametri_Server)

            Dim legamiRichiesteAppezzamento As DataTable = richieste.EstraiLegamiRichiestaxAppezzamento(piva, richiesta_cod, "", objParametri_Server)

            If legamiRichiesteAppezzamento.Rows.Count > 0 Then

                dtColture = richieste.TrovaColtureAppezzamentiSenzaFascicolo(piva, richiesta_cod, objParametri_Server)

                If dtColture.Rows.Count > 0 Then

                    Dim objAnalisi As New AgronicaCoreAnagrafeBIZ.Analisi_Modello_R
                    objAnalisi.CalcolaTessiturexUMA(dtColture, richiesta_cod, objParametri_Server, objParametri_Super_Server)

                    richiestexRegImpianti.InserisciAssociazioniPerNuovaPraticaContoProprio(legamiRichiesteAppezzamento, richiesta_cod, objParametri_Server)

                    Dim DtEntitaDistinct = dtColture.DefaultView.ToTable(False, "Macrouso_UMA_Cod", "Macrouso_UMA_Des", "REGOLAMENTO", "SUP_APP", "Sup_A", "Sup_B", "Sup_Normale", "Sup_Media", "Sup_Tenace")

                    Dim UMAGroups = DtEntitaDistinct.AsEnumerable().
                    GroupBy(Function(row) New With {
                        Key .Macrouso_Cod = row.Item("Macrouso_UMA_Cod"),
                        Key .Macrouso_Des = CStr(row.Item("Macrouso_UMA_Des")).Trim,
                        Key .Regolamento = row.Item("REGOLAMENTO")
                    })

                    Dim tableResult = DtEntitaDistinct.Clone()
                    For Each grp In UMAGroups
                        tableResult.Rows.Add(grp.Key.Macrouso_Cod, grp.Key.Macrouso_Des, grp.Key.Regolamento, grp.Sum(Function(row) CType(row.Item("SUP_APP"), Decimal)),
                    grp.Sum(Function(row) CType(row.Item("Sup_A"), Decimal)), grp.Sum(Function(row) CType(row.Item("Sup_B"), Decimal)), grp.Sum(Function(row) CType(row.Item("Sup_Normale"), Decimal)),
                    grp.Sum(Function(row) CType(row.Item("Sup_Media"), Decimal)), grp.Sum(Function(row) CType(row.Item("Sup_Tenace"), Decimal)))
                    Next

                    For Each row As DataRow In tableResult.Rows

                        Dim dtgroupRow = dtgroup.NewRow
                        dtgroupRow.Item("Programmazione_Cod") = CodiceFascicoloPianoColturaleGrafico
                        dtgroupRow.Item("macrouso_UMA_Cod") = row.Item("macrouso_UMA_Cod")
                        dtgroupRow.Item("macrouso_UMA_Des") = row.Item("macrouso_UMA_Des")
                        dtgroupRow.Item("Programmazione_Des") = DescrizioneFascicoloPianoColturaleGrafico
                        dtgroupRow.Item("Veg_Cod") = "000"
                        dtgroupRow.Item("Id_Cod") = "000"
                        dtgroupRow.Item("sup_A") = row.Item("sup_A")
                        dtgroupRow.Item("sup_B") = row.Item("sup_B")
                        dtgroupRow.Item("sup_UMA_A_Edit") = row.Item("sup_A")
                        dtgroupRow.Item("sup_UMA_B_Edit") = row.Item("sup_B")
                        dtgroupRow.Item("calcolato") = 0
                        dtgroupRow.Item("richiesto") = 0
                        dtgroupRow.Item("assegnato") = 0

                        If row.Item("sup_A") + row.Item("sup_B") <> row.Item("SUP_APP") Then
                            dtgroupRow.Item("sup_UMA") = row.Item("sup_A") + row.Item("sup_B")
                            dtgroupRow.Item("sup_UMA_Edit") = row.Item("sup_A") + row.Item("sup_B")
                        Else
                            dtgroupRow.Item("sup_UMA") = row.Item("SUP_APP")
                            dtgroupRow.Item("sup_UMA_Edit") = row.Item("SUP_APP")
                        End If

                        dtgroupRow.Item("sup_Normale") = row.Item("Sup_Normale")
                        dtgroupRow.Item("sup_Media") = row.Item("Sup_Media")
                        dtgroupRow.Item("sup_Tenace") = row.Item("Sup_Tenace")

                        dtgroupRow.Item("TerrenoNormale_Edit") = row.Item("Sup_Normale")
                        dtgroupRow.Item("TerrenoMedio_Edit") = row.Item("Sup_Media")
                        dtgroupRow.Item("TerrenoTenace_Edit") = row.Item("Sup_Tenace")

                        If (dtgroupRow.Item("Sup_Normale") = 0 AndAlso
                        dtgroupRow.Item("Sup_Media") = 0 AndAlso
                        dtgroupRow.Item("Sup_Tenace") = 0) Then

                            dtgroupRow.Item("Sup_Normale") = row.Item("SUP_APP")
                            dtgroupRow.Item("TerrenoNormale_Edit") = row.Item("SUP_APP")
                        End If

                        If (dtgroupRow.Item("sup_A") = 0 AndAlso
                        dtgroupRow.Item("sup_B") = 0) Then
                            dtgroupRow.Item("sup_A") = row.Item("SUP_APP")
                        End If

                        dtgroupRow.Item("Regolamento_Cod") = row.Item("Regolamento")
                        dtgroup.Rows.Add(dtgroupRow)

                    Next


                    Dim dtInsert As New DataTable
                    dtInsert.Columns.Add(New DataColumn("Macrouso_UMA_Des", GetType(String)))
                    dtInsert.Columns.Add(New DataColumn("Macrouso_UMA_Cod", GetType(String)))
                    dtInsert.Columns.Add(New DataColumn("Programmazione_Cod", GetType(Integer)))
                    dtInsert.Columns.Add(New DataColumn("Regolamento_Cod", GetType(String)))
                    dtInsert.Columns.Add(New DataColumn("Sup_UMA", GetType(Decimal)))
                    dtInsert.Columns.Add(New DataColumn("Sup_A", GetType(Decimal)))
                    dtInsert.Columns.Add(New DataColumn("Sup_B", GetType(Decimal)))
                    dtInsert.Columns.Add(New DataColumn("TerrenoNormale", GetType(Decimal)))
                    dtInsert.Columns.Add(New DataColumn("TerrenoMedio", GetType(Decimal)))
                    dtInsert.Columns.Add(New DataColumn("TerrenoTenace", GetType(Decimal)))
                    dtInsert.Columns.Add(New DataColumn("calcolato", GetType(Decimal)))
                    dtInsert.Columns.Add(New DataColumn("richiesto", GetType(Decimal)))
                    dtInsert.Columns.Add(New DataColumn("assegnato", GetType(Decimal)))
                    dtInsert.Columns.Add(New DataColumn("sup_UMA_Edit", GetType(Decimal)))
                    dtInsert.Columns.Add(New DataColumn("sup_UMA_A_Edit", GetType(Decimal)))
                    dtInsert.Columns.Add(New DataColumn("sup_UMA_B_Edit", GetType(Decimal)))
                    dtInsert.Columns.Add(New DataColumn("TerrenoNormale_Edit", GetType(Decimal)))
                    dtInsert.Columns.Add(New DataColumn("TerrenoMedio_Edit", GetType(Decimal)))
                    dtInsert.Columns.Add(New DataColumn("TerrenoTenace_Edit", GetType(Decimal)))
                    dtInsert.Columns.Add(New DataColumn("Programmazione_Des", GetType(String)))

                    For Each row In dtgroup.Rows

                        d0 = dtInsert.NewRow
                        d0("Macrouso_UMA_Des") = row.Item("Macrouso_UMA_Des")
                        d0("Macrouso_UMA_Cod") = row.Item("Macrouso_UMA_Cod")
                        d0("Programmazione_Cod") = row.Item("Programmazione_Cod")
                        d0("Regolamento_Cod") = row.Item("Regolamento_Cod")
                        d0("Sup_UMA") = row.Item("Sup_UMA")
                        d0("Sup_A") = row.Item("Sup_A")
                        d0("Sup_B") = row.Item("Sup_B")
                        d0("TerrenoNormale") = row.Item("Sup_Normale")
                        d0("TerrenoMedio") = row.Item("Sup_Media")
                        d0("TerrenoTenace") = row.Item("Sup_Tenace")
                        d0("calcolato") = row.Item("calcolato")
                        d0("richiesto") = row.Item("richiesto")
                        d0("assegnato") = row.Item("assegnato")
                        d0("sup_UMA_Edit") = row.Item("Sup_UMA")
                        d0("sup_UMA_A_Edit") = row.Item("Sup_A")
                        d0("sup_UMA_B_Edit") = row.Item("Sup_B")
                        d0("TerrenoNormale_Edit") = row.Item("Sup_Normale")
                        d0("TerrenoMedio_Edit") = row.Item("Sup_Media")
                        d0("TerrenoTenace_Edit") = row.Item("Sup_Tenace")
                        d0("Programmazione_Cod") = row.Item("Programmazione_Cod")
                        d0("Programmazione_Des") = row.Item("Programmazione_Des")
                        dtInsert.Rows.Add(d0)
                    Next


                    EFrichieste.Aggiorna_Richieste(dtInsert, New DataTable, piva, richiesta_cod, objParametri_Server)
                    'Dim regolamentiBio = EFrichieste.Check_Biologico_Integrative(richiesta_cod, objParametri_Server)

                    For Each row In dtgroup.Rows

                        d0 = Dt.NewRow
                        d0("Gruppo_Colturale_UMA") = row.Item("Macrouso_UMA_Des")
                        d0("Macrouso_UMA_Cod") = row.Item("Macrouso_UMA_Cod")
                        d0("Programmazione_Cod") = row.Item("Programmazione_Cod")
                        d0("Regolamento_Cod") = CInt(row.Item("Programmazione_Cod").ToString)
                        d0("Richiesta_Cod") = richiesta_cod
                        d0("sup_UMA") = row.Item("sup_UMA")
                        d0("sup_UMA_A") = row.Item("sup_A")
                        d0("sup_UMA_B") = row.Item("sup_B")
                        d0("sup_UMA_Edit") = row.Item("sup_UMA_Edit")
                        d0("sup_UMA_A_Edit") = row.Item("sup_UMA_A_Edit")
                        d0("sup_UMA_B_Edit") = row.Item("sup_UMA_B_Edit")
                        d0("UMA_Cod") = row.Item("Macrouso_UMA_Cod").ToString.TrimEnd(" ")
                        d0("calcolato") = row.Item("calcolato")
                        d0("richiesto") = row.Item("richiesto")
                        d0("assegnato") = row.Item("assegnato")
                        d0("TerrenoNormale") = row.Item("sup_Normale")
                        d0("TerrenoMedio") = row.Item("sup_Media")
                        d0("TerrenoTenace") = row.Item("sup_Tenace")
                        d0("TerrenoNormale_Edit") = row.Item("TerrenoNormale_Edit")
                        d0("TerrenoMedio_Edit") = row.Item("TerrenoMedio_Edit")
                        d0("TerrenoTenace_Edit") = row.Item("TerrenoTenace_Edit")
                        d0("Veg_Cod") = row.Item("Veg_Cod")
                        d0("Id_Cod") = row.Item("Id_Cod")
                        d0("Programmazione_Cod") = row.Item("Programmazione_Cod")
                        d0("Programmazione_Des") = row.Item("Programmazione_Des")
                        Dt.Rows.Add(d0)

                    Next

                    If errore <> "" Then

                        res = New Tuple(Of DataTable, String)(New DataTable, errore)

                        Return res
                    End If

                End If

            Else

                res = New Tuple(Of DataTable, String)(New DataTable, "Non sono stati trovati impianti validi per questa azienda.")

                Return res

            End If

        Else

            For Each row In dtRichieste.Rows

                d0 = Dt.NewRow
                d0("Gruppo_Colturale_UMA") = row.Item("Macrouso_UMA_Des")
                d0("Macrouso_UMA_Cod") = row.Item("Gruppo_Colturale_UMA")
                d0("Programmazione_Cod") = row.Item("Programmazione_Cod")
                d0("Regolamento_Cod") = row.Item("Regolamento_Cod")
                d0("Richiesta_Cod") = richiesta_cod
                d0("sup_UMA") = row.Item("Totale_Superficie_UMA")
                d0("sup_UMA_A") = row.Item("Zona_Pendenza_A_UMA")
                d0("sup_UMA_B") = row.Item("Zona_Pendenza_B_UMA")
                d0("sup_UMA_Edit") = row.Item("Totale_Superficie_UMA_Edit")
                d0("sup_UMA_A_Edit") = row.Item("Zona_Pendenza_A_UMA_Edit")
                d0("sup_UMA_B_Edit") = row.Item("Zona_Pendenza_B_UMA_Edit")
                d0("UMA_Cod") = row.Item("Gruppo_Colturale_UMA")
                d0("calcolato") = row.Item("Carburante_Calcolato")
                d0("richiesto") = row.Item("Carburante_Richiesto")
                d0("assegnato") = row.Item("Carburante_Approvato")
                d0("TerrenoNormale") = row.Item("Zona_Tessitura_Normale_UMA")
                d0("TerrenoMedio") = row.Item("Zona_Tessitura_Media_UMA")
                d0("TerrenoTenace") = row.Item("Zona_Tessitura_Tenace_UMA")
                d0("TerrenoNormale_Edit") = row.Item("Zona_Tessitura_Normale_UMA_Edit")
                d0("TerrenoMedio_Edit") = row.Item("Zona_Tessitura_Media_UMA_Edit")
                d0("TerrenoTenace_Edit") = row.Item("Zona_Tessitura_Tenace_UMA_Edit")
                d0("Programmazione_Cod") = row.Item("Programmazione_Cod")

                Select Case d0("Programmazione_Cod")
                    Case CodiceFascicoloColtureNonImputabili
                        d0("Programmazione_Des") = DescrizioneFascicoloColtureNonImputabili
                    Case CodiceFascicoloAnticipi
                        d0("Programmazione_Des") = DescrizioneFascicoloAnticipi
                    Case CodiceFascicoloTrasferimenti
                        d0("Programmazione_Des") = DescrizioneFascicoloTrasferimenti
                    Case Else
                        d0("Programmazione_Des") = DescrizioneFascicoloPianoColturaleGrafico
                End Select

                If (Not IsDBNull(d0("Veg_Cod")) AndAlso (d0("Veg_Cod") <> "0")) Then
                    d0("Veg_Cod") = d0("Veg_Cod").ToString.Remove(0, 3)
                End If

                If (Not IsDBNull(d0("Id_Cod")) AndAlso (d0("Id_Cod") <> "0")) Then
                    d0("Id_Cod") = d0("Id_Cod").ToString.Remove(0, 3)
                End If

                Dt.Rows.Add(d0)

            Next

        End If

        Dt.DefaultView.Sort = "Gruppo_Colturale_UMA ASC "
        Dt = Dt.DefaultView.ToTable

        res = New Tuple(Of DataTable, String)(Dt, "")

        Return res

    End Function


    Private Shared Function TrovaRichiestePrimaDel2025(piva As String, richiesta_cod As Integer, check As Boolean, ByRef Programmazione_Cod As Integer,
                                                       avanzamento As Integer, ByRef objParametri_Server As AgronicaCoreParametri) As Tuple(Of DataTable, String)

        Dim DtMacro As DataTable
        Dim DtEntita As DataTable
        Dim Dt As New DataTable
        Dim errore As String = ""
        Dim res As Tuple(Of DataTable, String)

        Dt.Columns.Add(New DataColumn("Gruppo_Colturale_UMA", GetType(String)))
        Dt.Columns.Add(New DataColumn("Macrouso_UMA_Cod", GetType(String)))
        Dt.Columns.Add(New DataColumn("Programmazione_Cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Richiesta_Cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Regolamento_Cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("sup_UMA", GetType(Decimal)))
        Dt.Columns.Add(New DataColumn("sup_UMA_A", GetType(Decimal)))
        Dt.Columns.Add(New DataColumn("sup_UMA_B", GetType(Decimal)))
        Dt.Columns.Add(New DataColumn("sup_UMA_Edit", GetType(Decimal)))
        Dt.Columns.Add(New DataColumn("sup_UMA_A_Edit", GetType(Decimal)))
        Dt.Columns.Add(New DataColumn("sup_UMA_B_Edit", GetType(Decimal)))
        Dt.Columns.Add(New DataColumn("UMA_Cod", GetType(String)))
        Dt.Columns.Add(New DataColumn("calcolato", GetType(Decimal)))
        Dt.Columns.Add(New DataColumn("richiesto", GetType(Decimal)))
        Dt.Columns.Add(New DataColumn("assegnato", GetType(Decimal)))
        Dt.Columns.Add(New DataColumn("TerrenoNormale", GetType(Decimal)))
        Dt.Columns.Add(New DataColumn("TerrenoMedio", GetType(Decimal)))
        Dt.Columns.Add(New DataColumn("TerrenoTenace", GetType(Decimal)))
        Dt.Columns.Add(New DataColumn("TerrenoNormale_Edit", GetType(Decimal)))
        Dt.Columns.Add(New DataColumn("TerrenoMedio_Edit", GetType(Decimal)))
        Dt.Columns.Add(New DataColumn("TerrenoTenace_Edit", GetType(Decimal)))
        Dt.Columns.Add(New DataColumn("Veg_Cod", GetType(String)))
        Dt.Columns.Add(New DataColumn("Id_Cod", GetType(String)))

        'Dt.Columns.Add(New DataColumn("Programmazione_Cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Programmazione_Des", GetType(String)))

        Dim testata As New AgronicaCoreAnagrafeDAL.Programmazione_Testata_R
        Dim entita As New AgronicaCoreAnagrafeDAL.Programmazione_Entita_R
        Dim dataFit As Date = #1/1/2000 12:00 PM#
        Dim UMADict As New Dictionary(Of Tuple(Of String, String, String, String, String), Tuple(Of String, String))
        Dim macroDict As New Dictionary(Of String, String)
        Dim richieste As New AgronicaCoreUmaDal.UMA_Richieste_R
        Dim EFrichieste As New AgronicaCoreUmaBiz.UMA_Richieste
        Dim dtRichieste As New DataTable

        Dim dtgroup As New DataTable
        Dim dtTable_Pendenza As DataTable
        Dim dtTable_Tessitura As DataTable
        Dim exp As String
        Dim exp2 As String
        Dim expEnt As String
        Dim dr As DataRow()
        Dim dr2 As DataRow()
        Dim drEnt As DataRow()
        Dim d0 As DataRow

        dtgroup.Columns.Add(New DataColumn("macrouso_UMA_Cod", GetType(String)))
        dtgroup.Columns.Add(New DataColumn("macrouso_UMA_Des", GetType(String)))
        dtgroup.Columns.Add(New DataColumn("Programmazione_Cod", GetType(Integer)))
        dtgroup.Columns.Add(New DataColumn("Regolamento_Cod", GetType(Integer)))
        dtgroup.Columns.Add(New DataColumn("sup_UMA", GetType(Decimal)))
        dtgroup.Columns.Add(New DataColumn("sup_A", GetType(Decimal)))
        dtgroup.Columns.Add(New DataColumn("sup_B", GetType(Decimal)))
        dtgroup.Columns.Add(New DataColumn("sup_Normale", GetType(Decimal)))
        dtgroup.Columns.Add(New DataColumn("sup_Media", GetType(Decimal)))
        dtgroup.Columns.Add(New DataColumn("sup_Tenace", GetType(Decimal)))
        dtgroup.Columns.Add(New DataColumn("calcolato", GetType(Decimal)))
        dtgroup.Columns.Add(New DataColumn("richiesto", GetType(Decimal)))
        dtgroup.Columns.Add(New DataColumn("assegnato", GetType(Decimal)))
        dtgroup.Columns.Add(New DataColumn("Veg_Cod", GetType(String)))
        dtgroup.Columns.Add(New DataColumn("Id_Cod", GetType(String)))
        dtgroup.Columns.Add(New DataColumn("sup_UMA_Edit", GetType(Decimal)))
        dtgroup.Columns.Add(New DataColumn("sup_UMA_A_Edit", GetType(Decimal)))
        dtgroup.Columns.Add(New DataColumn("sup_UMA_B_Edit", GetType(Decimal)))
        dtgroup.Columns.Add(New DataColumn("TerrenoNormale_Edit", GetType(Decimal)))
        dtgroup.Columns.Add(New DataColumn("TerrenoMedio_Edit", GetType(Decimal)))
        dtgroup.Columns.Add(New DataColumn("TerrenoTenace_Edit", GetType(Decimal)))
        'dtgroup.Columns.Add(New DataColumn("Programmazione_Cod", GetType(Integer)))
        dtgroup.Columns.Add(New DataColumn("Programmazione_Des", GetType(String)))

        'recupero le richieste presenti attualmente nel db
        dtRichieste = richieste.Leggi(piva, richiesta_cod, "", 0, enumSelezioneVariabile.Selezione_TabellaCompleta, objParametri_Server)

        If dtRichieste.Rows.Count > 0 Then
            Programmazione_Cod = 0
        End If

        'prendo le righe più recenti dalle testate UMA
        DtMacro = testata.Leggi("", Programmazione_Cod, piva, "", 0, dataFit, DateTime.Now, enum_TipoRicetta.Non_Filtrare, enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                "", "Data_creazione DESC", objParametri_Server)

        If (DtMacro.Rows.Count > 0 AndAlso (check OrElse dtRichieste.Rows.Count = 0)) Then
            Programmazione_Cod = DtMacro.Rows.Item(0).Item("Programmazione_Cod")
            'recupero i dati dal planning
            DtEntita = entita.Programmazione_Entita_Leggi(Programmazione_Cod, "", 0, "", piva, 0, 0, 0, 0, 0,
                                                      dataFit, DateTime.Now, 0, enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                                      "EXISTS (SELECT TOP 1 * FROM Programmazione_Particelle WHERE Programmazione_Particelle.Programmazione_Entita_Cod = Programmazione_entita.Programmazione_Entita_Cod AND Programmazione_Particelle.prov in ('054', '055'))",
                                                      "Macrouso_Cod, Occupazione_Cod_Agea, Cul_Cod_Agea", objParametri_Server, Lettura_Per_UMA:=True)

            dtTable_Pendenza = entita.Programmazione_Entita_Leggi_per_UMA_DT_Pendenza(DtMacro.Rows.Item(0).Item("Programmazione_Cod"), objParametri_Server, richiesta_Cod:=richiesta_cod)
            dtTable_Tessitura = entita.Programmazione_Entita_Leggi_per_UMA_DT_Tessitura(DtMacro.Rows.Item(0).Item("Programmazione_Cod"), objParametri_Server, richiesta_Cod:=richiesta_cod, avanzamento:=avanzamento)

            Dim DtEntitaDistinct = DtEntita.DefaultView.ToTable(False, "Macrouso_Cod", "Occupazione_Cod_Agea", "Cul_Cod_Agea", "Veg_Cod", "Id_Cod", "Destinazione_Cod_Agea", "Superficie")

            Dim UMAGroups = DtEntitaDistinct.AsEnumerable().
                GroupBy(Function(row) New With {
                    Key .Macrouso_Cod = row.Item("Macrouso_Cod"),
                    Key .Occupazione_Cod_Agea = row.Item("Occupazione_Cod_Agea"),
                    Key .Cul_Cod_Agea = row.Item("Cul_Cod_Agea"),
                    Key .Veg_Cod = row.Item("Veg_Cod"),
                    Key .Id_Cod = row.Item("Id_Cod"),
                    Key .Destinazione_Cod_Agea = row.Item("Destinazione_Cod_Agea")
                })

            Dim tableResult = DtEntitaDistinct.Clone()
            For Each grp In UMAGroups
                tableResult.Rows.Add(grp.Key.Macrouso_Cod, grp.Key.Occupazione_Cod_Agea, grp.Key.Cul_Cod_Agea, grp.Key.Veg_Cod,
                                     grp.Key.Id_Cod, grp.Key.Destinazione_Cod_Agea, grp.Sum(Function(row) CType(row.Item("Superficie"), Decimal)))
            Next

            If dtTable_Pendenza.Rows.Count > 0 AndAlso dtTable_Tessitura.Rows.Count > 0 Then

                dtTable_Pendenza = dtTable_Pendenza.Select(" macrouso_UMA_Cod <> '' ").CopyToDataTable

                For Each row As DataRow In dtTable_Pendenza.Rows

                    If (macroDict.ContainsKey(row.Item("macrouso_UMA_Cod"))) Then

                        exp = "Macrouso_UMA_Cod = '" & row.Item("Macrouso_UMA_Cod") & "' "
                        dr = dtgroup.Select(exp)
                        If (dr.Count > 0) Then
                            'se era già presente un record con quel macrouso, sommo le superfici
                            dr.First.Item("Sup_A") = dr.First.Item("Sup_A") + row.Item("sup_A")
                            dr.First.Item("Sup_B") = dr.First.Item("Sup_B") + row.Item("sup_B")
                            dr.First.Item("sup_UMA_A_Edit") = dr.First.Item("sup_UMA_A_Edit") + row.Item("sup_A")
                            dr.First.Item("sup_UMA_B_Edit") = dr.First.Item("sup_UMA_B_Edit") + row.Item("sup_B")

                            exp2 = "Macrouso_UMA_Cod = '" & row.Item("Macrouso_UMA_Cod") & "' AND Occupazione_Cod_Agea = '" & row.Item("Occupazione_Cod_Agea").ToString & "' AND Destinazione_Cod_Agea = '" & row.Item("Destinazione_Cod_Agea").ToString &
                          "' AND Veg_Cod = '" & row.Item("veg_Cod").ToString & "' AND Id_Cod = '" & row.Item("Id_Cod").ToString & "' AND Cul_Cod_Agea = '" & row.Item("Cul_Cod_Agea").ToString & "' AND Macrouso_Cod = '" &
                          row.Item("Macrouso_Cod").ToString & "' "
                            dr2 = dtTable_Tessitura.Select(exp2)

                            expEnt = "Occupazione_Cod_Agea = '" & row.Item("Occupazione_Cod_Agea").ToString & "' AND Destinazione_Cod_Agea = '" & row.Item("Destinazione_Cod_Agea").ToString &
                          "' AND Veg_Cod = '" & row.Item("veg_Cod").ToString & "' AND Id_Cod = '" & row.Item("Id_Cod").ToString &
                          "' AND Cul_Cod_Agea = '" & row.Item("Cul_Cod_Agea").ToString & "' AND Macrouso_Cod = '" & row.Item("Macrouso_Cod").ToString & "' "
                            drEnt = tableResult.Select(expEnt)

                            If (dr2.Count > 0) Then
                                dr.First.Item("sup_Normale") = dr.First.Item("sup_Normale") + dr2.First.Item("sup_Normale")
                                dr.First.Item("sup_Media") = dr.First.Item("sup_Media") + dr2.First.Item("sup_Media")
                                dr.First.Item("sup_Tenace") = dr.First.Item("sup_Tenace") + dr2.First.Item("sup_Tenace")

                                dr.First.Item("TerrenoNormale_Edit") = dr.First.Item("TerrenoNormale_Edit") + dr2.First.Item("sup_Normale")
                                dr.First.Item("TerrenoMedio_Edit") = dr.First.Item("TerrenoMedio_Edit") + dr2.First.Item("sup_Media")
                                dr.First.Item("TerrenoTenace_Edit") = dr.First.Item("TerrenoTenace_Edit") + dr2.First.Item("sup_Tenace")
                            End If

                            If (drEnt.Count > 0) Then
                                dr.First.Item("sup_UMA") = dr.First.Item("sup_UMA") + drEnt.First.Item("Superficie")
                                dr.First.Item("sup_UMA_Edit") = dr.First.Item("sup_UMA_Edit") + drEnt.First.Item("Superficie")
                            End If

                        End If

                    Else

                        exp2 = "Macrouso_UMA_Cod = '" & row.Item("Macrouso_UMA_Cod") & "' AND Occupazione_Cod_Agea = '" & row.Item("Occupazione_Cod_Agea").ToString &
                          "' AND Veg_Cod = '" & row.Item("veg_Cod").ToString & "' AND Id_Cod = '" & row.Item("Id_Cod").ToString & "' AND Destinazione_Cod_Agea = '" & row.Item("Destinazione_Cod_Agea").ToString &
                          "' AND Cul_Cod_Agea = '" & row.Item("Cul_Cod_Agea").ToString & "' AND Macrouso_Cod = '" & row.Item("Macrouso_Cod").ToString & "' "
                        dr2 = dtTable_Tessitura.Select(exp2)

                        expEnt = "Occupazione_Cod_Agea = '" & row.Item("Occupazione_Cod_Agea").ToString & "' AND Destinazione_Cod_Agea = '" & row.Item("Destinazione_Cod_Agea").ToString &
                          "' AND Veg_Cod = '" & row.Item("veg_Cod").ToString & "' AND Id_Cod = '" & row.Item("Id_Cod").ToString &
                          "' AND Cul_Cod_Agea = '" & row.Item("Cul_Cod_Agea").ToString & "' AND Macrouso_Cod = '" & row.Item("Macrouso_Cod").ToString & "' "
                        drEnt = tableResult.Select(expEnt)

                        Dim dtgroupRow = dtgroup.NewRow
                        dtgroupRow.Item("Programmazione_Cod") = Programmazione_Cod
                        dtgroupRow.Item("macrouso_UMA_Cod") = row.Item("macrouso_UMA_Cod")
                        dtgroupRow.Item("macrouso_UMA_Des") = row.Item("macrouso_UMA_Des")
                        dtgroupRow.Item("Programmazione_Des") = dr2.First.Item("Programmazione_Des")
                        dtgroupRow.Item("Veg_Cod") = drEnt.First.Item("Veg_Cod")
                        dtgroupRow.Item("Id_Cod") = drEnt.First.Item("Id_Cod")
                        dtgroupRow.Item("sup_A") = row.Item("sup_A")
                        dtgroupRow.Item("sup_B") = row.Item("sup_B")
                        dtgroupRow.Item("sup_UMA_A_Edit") = row.Item("sup_A")
                        dtgroupRow.Item("sup_UMA_B_Edit") = row.Item("sup_B")
                        dtgroupRow.Item("calcolato") = 0
                        dtgroupRow.Item("richiesto") = 0
                        dtgroupRow.Item("assegnato") = 0
                        If drEnt.Count = 0 OrElse row.Item("sup_A") + row.Item("sup_B") <> drEnt.First.Item("Superficie") Then
                            dtgroupRow.Item("sup_UMA") = row.Item("sup_A") + row.Item("sup_B")
                            dtgroupRow.Item("sup_UMA_Edit") = row.Item("sup_A") + row.Item("sup_B")
                        Else
                            dtgroupRow.Item("sup_UMA") = drEnt.First.Item("Superficie")
                            dtgroupRow.Item("sup_UMA_Edit") = drEnt.First.Item("Superficie")
                        End If


                        If (dr2.Count > 0) Then

                            dtgroupRow.Item("sup_Normale") = dr2.First.Item("sup_Normale")
                            dtgroupRow.Item("sup_Media") = dr2.First.Item("sup_Media")
                            dtgroupRow.Item("sup_Tenace") = dr2.First.Item("sup_Tenace")

                            dtgroupRow.Item("TerrenoNormale_Edit") = dr2.First.Item("sup_Normale")
                            dtgroupRow.Item("TerrenoMedio_Edit") = dr2.First.Item("sup_Media")
                            dtgroupRow.Item("TerrenoTenace_Edit") = dr2.First.Item("sup_Tenace")

                        Else

                            dtgroupRow.Item("sup_Normale") = 0
                            dtgroupRow.Item("sup_Media") = 0
                            dtgroupRow.Item("sup_Tenace") = 0

                            dtgroupRow.Item("TerrenoNormale_Edit") = 0
                            dtgroupRow.Item("TerrenoMedio_Edit") = 0
                            dtgroupRow.Item("TerrenoTenace_Edit") = 0

                        End If

                        If (dtgroupRow.Item("sup_Normale") = 0 AndAlso
                        dtgroupRow.Item("sup_Media") = 0 AndAlso
                        dtgroupRow.Item("sup_Tenace") = 0) Then

                            dtgroupRow.Item("sup_Normale") = drEnt.First.Item("Superficie")
                            dtgroupRow.Item("TerrenoNormale_Edit") = drEnt.First.Item("Superficie")

                        End If

                        If (dtgroupRow.Item("sup_A") = 0 AndAlso
                        dtgroupRow.Item("sup_B") = 0) Then
                            dtgroupRow.Item("sup_A") = drEnt.First.Item("Superficie")
                        End If

                        dtgroupRow.Item("Regolamento_Cod") = 1
                        dtgroup.Rows.Add(dtgroupRow)
                        macroDict.Add(row.Item("macrouso_UMA_Cod"), row.Item("macrouso_UMA_Des"))

                    End If
                Next

                For Each row In dtgroup.Rows

                    If row("sup_UMA") <> (row("sup_A") + row("sup_B")) Then
                        row("sup_A") = row("sup_UMA") - row("sup_B")
                    End If

                    If row("sup_UMA") <> (row("sup_Normale") + row("sup_Media") + row("sup_Tenace")) Then
                        row("sup_Normale") = row("sup_UMA") - row("sup_Media") - row("sup_Tenace")
                    End If

                    If row("sup_UMA_Edit") <> (row("sup_UMA_A_Edit") + row("sup_UMA_B_Edit")) Then
                        row("sup_UMA_A_Edit") = row("sup_UMA_Edit") - row("sup_UMA_B_Edit")
                    End If

                    If row("sup_UMA_Edit") <> (row("TerrenoNormale_Edit") + row("TerrenoMedio_Edit") + row("TerrenoTenace_Edit")) Then
                        row("TerrenoNormale_Edit") = row("sup_UMA_Edit") - row("TerrenoMedio_Edit") - row("TerrenoTenace_Edit")
                    End If

                Next

                Dim dtUpdate As New DataTable
                Dim dtInsert As New DataTable

                'inizializzo le dataTable di insert e update con i campi significativi
                dtUpdate.Columns.Add(New DataColumn("Macrouso_UMA_Des", GetType(String)))
                dtUpdate.Columns.Add(New DataColumn("Macrouso_UMA_Cod", GetType(String)))
                dtUpdate.Columns.Add(New DataColumn("Programmazione_Cod", GetType(Integer)))
                dtUpdate.Columns.Add(New DataColumn("Regolamento_Cod", GetType(Integer)))
                dtUpdate.Columns.Add(New DataColumn("Sup_A", GetType(Decimal)))
                dtUpdate.Columns.Add(New DataColumn("Sup_B", GetType(Decimal)))
                dtUpdate.Columns.Add(New DataColumn("TerrenoNormale", GetType(Decimal)))
                dtUpdate.Columns.Add(New DataColumn("TerrenoMedio", GetType(Decimal)))
                dtUpdate.Columns.Add(New DataColumn("TerrenoTenace", GetType(Decimal)))
                dtUpdate.Columns.Add(New DataColumn("calcolato", GetType(Decimal)))
                dtUpdate.Columns.Add(New DataColumn("richiesto", GetType(Decimal)))
                dtUpdate.Columns.Add(New DataColumn("assegnato", GetType(Decimal)))
                dtUpdate.Columns.Add(New DataColumn("sup_UMA_Edit", GetType(Decimal)))
                dtUpdate.Columns.Add(New DataColumn("sup_UMA_A_Edit", GetType(Decimal)))
                dtUpdate.Columns.Add(New DataColumn("sup_UMA_B_Edit", GetType(Decimal)))
                dtUpdate.Columns.Add(New DataColumn("TerrenoNormale_Edit", GetType(Decimal)))
                dtUpdate.Columns.Add(New DataColumn("TerrenoMedio_Edit", GetType(Decimal)))
                dtUpdate.Columns.Add(New DataColumn("TerrenoTenace_Edit", GetType(Decimal)))
                'dtUpdate.Columns.Add(New DataColumn("Programmazione_Cod", GetType(Integer)))
                dtUpdate.Columns.Add(New DataColumn("Programmazione_Des", GetType(String)))

                dtInsert.Columns.Add(New DataColumn("Macrouso_UMA_Des", GetType(String)))
                dtInsert.Columns.Add(New DataColumn("Macrouso_UMA_Cod", GetType(String)))
                dtInsert.Columns.Add(New DataColumn("Programmazione_Cod", GetType(Integer)))
                dtInsert.Columns.Add(New DataColumn("Regolamento_Cod", GetType(String)))
                dtInsert.Columns.Add(New DataColumn("Sup_A", GetType(Decimal)))
                dtInsert.Columns.Add(New DataColumn("Sup_B", GetType(Decimal)))
                dtInsert.Columns.Add(New DataColumn("TerrenoNormale", GetType(Decimal)))
                dtInsert.Columns.Add(New DataColumn("TerrenoMedio", GetType(Decimal)))
                dtInsert.Columns.Add(New DataColumn("TerrenoTenace", GetType(Decimal)))
                dtInsert.Columns.Add(New DataColumn("calcolato", GetType(Decimal)))
                dtInsert.Columns.Add(New DataColumn("richiesto", GetType(Decimal)))
                dtInsert.Columns.Add(New DataColumn("assegnato", GetType(Decimal)))
                dtInsert.Columns.Add(New DataColumn("sup_UMA_Edit", GetType(Decimal)))
                dtInsert.Columns.Add(New DataColumn("sup_UMA_A_Edit", GetType(Decimal)))
                dtInsert.Columns.Add(New DataColumn("sup_UMA_B_Edit", GetType(Decimal)))
                dtInsert.Columns.Add(New DataColumn("TerrenoNormale_Edit", GetType(Decimal)))
                dtInsert.Columns.Add(New DataColumn("TerrenoMedio_Edit", GetType(Decimal)))
                dtInsert.Columns.Add(New DataColumn("TerrenoTenace_Edit", GetType(Decimal)))
                'dtInsert.Columns.Add(New DataColumn("Programmazione_Cod", GetType(Integer)))
                dtInsert.Columns.Add(New DataColumn("Programmazione_Des", GetType(String)))
                'controllo le differenze tra i record del planning e quelli di UMA_Richieste
                'metto in insert i record non presenti in UMA_Richieste e in update i record modificati
                If (dtRichieste.Rows.Count > 0) Then
                    For Each row As DataRow In dtgroup.Rows
                        exp = "Gruppo_Colturale_UMA = '" & row.Item("Macrouso_UMA_Cod") & "' AND Programmazione_Cod = " & CStr(row.Item("Programmazione_Cod")) & " "
                        dr = dtRichieste.Select(exp)
                        'per ogni record del planning controllo se ne esiste uno con lo stesso macrouso
                        If (dr.Count > 0) Then
                            row.Item("calcolato") = dr(0).Item("Carburante_Calcolato")
                            row.Item("richiesto") = dr(0).Item("Carburante_Richiesto")
                            row.Item("assegnato") = dr(0).Item("Carburante_Approvato")
                            'in caso positivo controllo se ci sono differenze su vari campi e in caso li metto in update
                            If (Decimal.Parse(dr(0).Item("Zona_Pendenza_A_UMA")) <> Decimal.Parse(row.Item("Sup_A")) OrElse
                            Decimal.Parse(dr(0).Item("Zona_Pendenza_B_UMA")) <> Decimal.Parse(row.Item("Sup_B")) OrElse
                            Decimal.Parse(dr(0).Item("Zona_Tessitura_Normale_UMA")) <> Decimal.Parse(row.Item("Sup_Normale")) OrElse
                            Decimal.Parse(dr(0).Item("Zona_Tessitura_Media_UMA")) <> Decimal.Parse(row.Item("Sup_Media")) OrElse
                            Decimal.Parse(dr(0).Item("Zona_Tessitura_Tenace_UMA")) <> Decimal.Parse(row.Item("Sup_Tenace"))) Then

                                d0 = dtUpdate.NewRow
                                d0("Macrouso_UMA_Des") = row.Item("Macrouso_UMA_Des")
                                d0("Macrouso_UMA_Cod") = row.Item("Macrouso_UMA_Cod")
                                d0("Programmazione_Cod") = row.Item("Programmazione_Cod")
                                d0("Regolamento_Cod") = 1
                                d0("Sup_A") = row.Item("Sup_A")
                                d0("Sup_B") = row.Item("Sup_B")
                                d0("TerrenoNormale") = row.Item("Sup_Normale")
                                d0("TerrenoMedio") = row.Item("Sup_Media")
                                d0("TerrenoTenace") = row.Item("Sup_Tenace")
                                d0("calcolato") = row.Item("calcolato")
                                d0("richiesto") = row.Item("richiesto")
                                d0("assegnato") = row.Item("assegnato")

                                d0("sup_UMA_Edit") = row.Item("Sup_UMA")
                                d0("sup_UMA_A_Edit") = row.Item("Sup_A")
                                d0("sup_UMA_B_Edit") = row.Item("Sup_B")
                                d0("TerrenoNormale_Edit") = row.Item("Sup_Normale")
                                d0("TerrenoMedio_Edit") = row.Item("Sup_Media")
                                d0("TerrenoTenace_Edit") = row.Item("Sup_Tenace")
                                d0("Programmazione_Cod") = row.Item("Programmazione_Cod")
                                d0("Programmazione_Des") = row.Item("Programmazione_Des")
                                dtUpdate.Rows.Add(d0)
                            End If
                        Else

                            'in caso negativo aggiungo il record in insert
                            d0 = dtInsert.NewRow
                            d0("Macrouso_UMA_Des") = row.Item("Macrouso_UMA_Des")
                            d0("Macrouso_UMA_Cod") = row.Item("Macrouso_UMA_Cod")
                            d0("Programmazione_Cod") = row.Item("Programmazione_Cod")
                            d0("Regolamento_Cod") = 1
                            d0("Sup_A") = row.Item("Sup_A")
                            d0("Sup_B") = row.Item("Sup_B")
                            d0("TerrenoNormale") = row.Item("Sup_Normale")
                            d0("TerrenoMedio") = row.Item("Sup_Media")
                            d0("TerrenoTenace") = row.Item("Sup_Tenace")
                            d0("calcolato") = row.Item("calcolato")
                            d0("richiesto") = row.Item("richiesto")
                            d0("assegnato") = row.Item("assegnato")

                            d0("sup_UMA_Edit") = row.Item("Sup_UMA")
                            d0("sup_UMA_A_Edit") = row.Item("Sup_A")
                            d0("sup_UMA_B_Edit") = row.Item("Sup_B")
                            d0("TerrenoNormale_Edit") = row.Item("Sup_Normale")
                            d0("TerrenoMedio_Edit") = row.Item("Sup_Media")
                            d0("TerrenoTenace_Edit") = row.Item("Sup_Tenace")
                            d0("Programmazione_Cod") = row.Item("Programmazione_Cod")
                            d0("Programmazione_Des") = row.Item("Programmazione_Des")
                            dtInsert.Rows.Add(d0)
                        End If
                    Next
                Else
                    dtInsert = dtgroup
                End If

                EFrichieste.Aggiorna_Richieste(dtInsert, dtUpdate, piva, richiesta_cod, objParametri_Server)
                Dim regolamentiBio = EFrichieste.Check_Biologico_Integrative(richiesta_cod, objParametri_Server)

                For Each row In dtgroup.Rows

                    d0 = Dt.NewRow
                    d0("Gruppo_Colturale_UMA") = row.Item("Macrouso_UMA_Des")
                    d0("Macrouso_UMA_Cod") = row.Item("Macrouso_UMA_Cod")
                    d0("Programmazione_Cod") = row.Item("Programmazione_Cod")
                    d0("Regolamento_Cod") = If(regolamentiBio.ContainsKey(CInt(row.Item("Programmazione_Cod").ToString)), regolamentiBio(CInt(row.Item("Programmazione_Cod").ToString)), 1)
                    d0("Richiesta_Cod") = richiesta_cod
                    d0("sup_UMA") = row.Item("sup_UMA")
                    d0("sup_UMA_A") = row.Item("sup_A")
                    d0("sup_UMA_B") = row.Item("sup_B")
                    d0("sup_UMA_Edit") = row.Item("sup_UMA_Edit")
                    d0("sup_UMA_A_Edit") = row.Item("sup_UMA_A_Edit")
                    d0("sup_UMA_B_Edit") = row.Item("sup_UMA_B_Edit")
                    d0("UMA_Cod") = row.Item("Macrouso_UMA_Cod").ToString.TrimEnd(" ")
                    d0("calcolato") = row.Item("calcolato")
                    d0("richiesto") = row.Item("richiesto")
                    d0("assegnato") = row.Item("assegnato")
                    d0("TerrenoNormale") = row.Item("sup_Normale")
                    d0("TerrenoMedio") = row.Item("sup_Media")
                    d0("TerrenoTenace") = row.Item("sup_Tenace")
                    d0("TerrenoNormale_Edit") = row.Item("TerrenoNormale_Edit")
                    d0("TerrenoMedio_Edit") = row.Item("TerrenoMedio_Edit")
                    d0("TerrenoTenace_Edit") = row.Item("TerrenoTenace_Edit")
                    d0("Veg_Cod") = row.Item("Veg_Cod")
                    d0("Id_Cod") = row.Item("Id_Cod")
                    d0("Programmazione_Cod") = row.Item("Programmazione_Cod")
                    d0("Programmazione_Des") = row.Item("Programmazione_Des")
                    Dt.Rows.Add(d0)

                Next
            Else

                errore = "Rilevata mancanza di informazioni riguardanti le pendenze o le tessiture del fascicolo selezionato. Per continuare è necessaria la rivalidazione del fascicolo"

                res = New Tuple(Of DataTable, String)(New DataTable, errore)

                Return res
            End If
        Else

            For Each row In dtRichieste.Rows

                d0 = Dt.NewRow
                d0("Gruppo_Colturale_UMA") = row.Item("Macrouso_UMA_Des")
                d0("Macrouso_UMA_Cod") = row.Item("Gruppo_Colturale_UMA")
                d0("Programmazione_Cod") = row.Item("Programmazione_Cod")
                d0("Regolamento_Cod") = row.Item("Regolamento_Cod")
                d0("Richiesta_Cod") = richiesta_cod
                d0("sup_UMA") = row.Item("Totale_Superficie_UMA")
                d0("sup_UMA_A") = row.Item("Zona_Pendenza_A_UMA")
                d0("sup_UMA_B") = row.Item("Zona_Pendenza_B_UMA")
                d0("sup_UMA_Edit") = row.Item("Totale_Superficie_UMA_Edit")
                d0("sup_UMA_A_Edit") = row.Item("Zona_Pendenza_A_UMA_Edit")
                d0("sup_UMA_B_Edit") = row.Item("Zona_Pendenza_B_UMA_Edit")
                d0("UMA_Cod") = row.Item("Gruppo_Colturale_UMA")
                d0("calcolato") = row.Item("Carburante_Calcolato")
                d0("richiesto") = row.Item("Carburante_Richiesto")
                d0("assegnato") = row.Item("Carburante_Approvato")
                d0("TerrenoNormale") = row.Item("Zona_Tessitura_Normale_UMA")
                d0("TerrenoMedio") = row.Item("Zona_Tessitura_Media_UMA")
                d0("TerrenoTenace") = row.Item("Zona_Tessitura_Tenace_UMA")
                d0("TerrenoNormale_Edit") = row.Item("Zona_Tessitura_Normale_UMA_Edit")
                d0("TerrenoMedio_Edit") = row.Item("Zona_Tessitura_Media_UMA_Edit")
                d0("TerrenoTenace_Edit") = row.Item("Zona_Tessitura_Tenace_UMA_Edit")
                d0("Programmazione_Cod") = row.Item("Programmazione_Cod")
                If IsDBNull(row.Item("Programmazione_Des")) OrElse CStr(row.Item("Programmazione_Des")) = "" Then
                    Select Case d0("Programmazione_Cod")
                        Case CodiceFascicoloColtureNonImputabili
                            d0("Programmazione_Des") = DescrizioneFascicoloColtureNonImputabili
                        Case CodiceFascicoloAnticipi
                            d0("Programmazione_Des") = DescrizioneFascicoloAnticipi
                        Case Else
                            d0("Programmazione_Des") = DescrizioneFascicoloTrasferimenti
                    End Select
                Else
                    d0("Programmazione_Des") = row.Item("Programmazione_Des")
                End If

                If (Not IsDBNull(d0("Veg_Cod")) AndAlso (d0("Veg_Cod") <> "0")) Then
                    d0("Veg_Cod") = d0("Veg_Cod").ToString.Remove(0, 3)
                End If

                If (Not IsDBNull(d0("Id_Cod")) AndAlso (d0("Id_Cod") <> "0")) Then
                    d0("Id_Cod") = d0("Id_Cod").ToString.Remove(0, 3)
                End If

                Dt.Rows.Add(d0)

            Next

        End If

        Dt.DefaultView.Sort = " Programmazione_Des ASC, Gruppo_Colturale_UMA ASC "
        Dt = Dt.DefaultView.ToTable

        res = New Tuple(Of DataTable, String)(Dt, errore)

        Return res

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Trova_Richieste_Dettaglio(ByVal piva As String,
                                                     ByVal richiesta_cod As Integer,
                                                     ByVal check As Boolean,
                                                     ByVal macrouso_UMA_Cod As Integer,
                                                     ByVal Programmazione_Cod As Integer,
                                                     ByVal avanzamento As Integer) As RispostaStandard

        Dim r As New RispostaStandard
        Dim testate As New AgronicaCoreUmaDal.UMA_Richieste_Testata_R
        Dim res As Tuple(Of DataTable, String)

        Dim leggi As New CDG_DAL_R

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")

        Dim objParametri_Super_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Super_Server")


        Try

            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
                r.Sessione = False
                Return r
            End If

            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Super_Server) Then
                r.Sessione = False
                Return r
            End If

            Lingua.Gias_InizializzaCultura_DaSession()

            If Programmazione_Cod = CodiceFascicoloPianoColturaleGrafico Then

                res = Trova_Richieste_Dettaglio_Post2025(piva, richiesta_cod, macrouso_UMA_Cod, Programmazione_Cod, objParametri_Server, objParametri_Super_Server)

            Else

                'La risposta è una tupla contenente la Datatable da visualizzare e un ipotetico errore generato durante la costruzione della tabella,
                'se l'errore è vuoto, allora è andato tutto bene
                res = Trova_Richieste_Dettaglio_Pre2025(piva, richiesta_cod, check, macrouso_UMA_Cod, Programmazione_Cod, avanzamento, objParametri_Server)

            End If

            If String.IsNullOrEmpty(res.Item2) Then

                Dim dt As DataTable
                dt = res.Item1

                Dim serializerSettings As New JsonSerializerSettings()
                serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                r.RispostaStringa = JsonConvert.SerializeObject(dt, Formatting.None, serializerSettings)
                r.RispostaOK = True

            Else

                r.Errore = res.Item2
                r.RispostaOK = False

            End If

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
            AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Trova_Richieste_Dettaglio_Pre2025(ByVal piva As String,
                                                     ByVal richiesta_cod As Integer,
                                                     ByVal check As Boolean,
                                                     ByVal macrouso_UMA_Cod As Integer,
                                                     ByVal Programmazione_Cod As Integer,
                                                     ByVal avanzamento As Integer,
                                                     ByRef objParametri_Server As AgronicaCoreParametri) As Tuple(Of DataTable, String)

        Dim errore As String = ""
        Dim res As Tuple(Of DataTable, String)

        'Dim r As New RispostaStandard
        Dim Dt As New DataTable
        Dim DtEntita As DataTable
        Dim leggi As New CDG_DAL_R

        Try

            'Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
            'If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            '    r.Sessione = False
            '    Return r
            'End If

            Lingua.Gias_InizializzaCultura_DaSession()

            Dt.Columns.Add(New DataColumn("Macrouso_Des", GetType(String)))
            Dt.Columns.Add(New DataColumn("Macrouso_Cod", GetType(String)))
            Dt.Columns.Add(New DataColumn("Programmazione_Cod", GetType(Integer)))
            Dt.Columns.Add(New DataColumn("Richiesta_Cod", GetType(Integer)))
            Dt.Columns.Add(New DataColumn("sup_tot", GetType(Decimal)))
            Dt.Columns.Add(New DataColumn("sup_A", GetType(Decimal)))
            Dt.Columns.Add(New DataColumn("sup_B", GetType(Decimal)))
            Dt.Columns.Add(New DataColumn("Cod", GetType(String)))
            Dt.Columns.Add(New DataColumn("TerrenoNormale", GetType(Decimal)))
            Dt.Columns.Add(New DataColumn("TerrenoMedio", GetType(Decimal)))
            Dt.Columns.Add(New DataColumn("TerrenoTenace", GetType(Decimal)))
            Dt.Columns.Add(New DataColumn("Destinazione_Cod", GetType(String)))
            Dt.Columns.Add(New DataColumn("Uso_Cod", GetType(String)))
            Dt.Columns.Add(New DataColumn("Qualita_Cod", GetType(String)))
            Dt.Columns.Add(New DataColumn("Occupazione_Cod", GetType(String)))

            Dim testata As New AgronicaCoreAnagrafeDAL.Programmazione_Testata_R
            Dim entita As New AgronicaCoreAnagrafeDAL.Programmazione_Entita_R
            Dim dataFit As Date = #1/1/2000 12:00 PM#

            Dim richieste As New AgronicaCoreUmaDal.UMA_Richieste_R
            Dim EFrichieste As New AgronicaCoreUmaBiz.UMA_Richieste
            Dim dtRichieste As New DataTable

            Dim dtgroup As New DataTable
            Dim dtTable_Pendenza As DataTable
            Dim dtTable_PendenzaApp As DataTable
            Dim dtTable_Tessitura As DataTable
            Dim d0 As DataRow

            dtgroup.Columns.Add(New DataColumn("veg_Cod", GetType(String)))
            dtgroup.Columns.Add(New DataColumn("veg_Des", GetType(String)))
            dtgroup.Columns.Add(New DataColumn("sup_A", GetType(Decimal)))
            dtgroup.Columns.Add(New DataColumn("sup_B", GetType(Decimal)))
            dtgroup.Columns.Add(New DataColumn("sup_Normale", GetType(Decimal)))
            dtgroup.Columns.Add(New DataColumn("sup_Media", GetType(Decimal)))
            dtgroup.Columns.Add(New DataColumn("sup_Tenace", GetType(Decimal)))

            dtTable_Pendenza = entita.Programmazione_Entita_Leggi_per_UMA_DT_Pendenza(Programmazione_Cod, objParametri_Server,
                                                                                              "", "", macrouso_UMA_Cod, richiesta_Cod:=richiesta_cod)

            Dim Veg_Cod = ""
            Dim Id_Cod = ""

            For Each penRow As DataRow In dtTable_Pendenza.Rows
                If (penRow.Item("Veg_Cod") <> 0) Then
                    Veg_Cod = penRow.Item("Veg_Cod").ToString 'Veg_Cod.ToString + " , " + 
                Else
                    Veg_Cod = "0"
                End If

                If (penRow.Item("Id_Cod") <> 0) Then
                    Id_Cod = penRow.Item("Id_Cod").ToString 'Id_Cod.ToString + " , " + 
                Else
                    Id_Cod = "0"
                End If


                'If Not IsDBNull(Veg_Cod) AndAlso Veg_Cod.TrimStart().First = "," Then 'OrElse ((Not IsDBNull(Veg_Cod) AndAlso (Veg_Cod <> "0")) AndAlso Veg_Cod <> "") Then
                '    Veg_Cod = Veg_Cod.ToString.Remove(0, 3)
                'End If

                'If (Not IsDBNull(Id_Cod) AndAlso (Id_Cod <> "0")) AndAlso (Id_Cod <> "") Then
                '    Id_Cod = Id_Cod.ToString.Remove(0, 3)
                'End If


                'prendo le righe più recenti dalle testate UMA
                'DtMacro = testata.Leggi("", Programmazione_Cod, piva, "", 0, dataFit, DateTime.Now, enum_TipoRicetta.Non_Filtrare, enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                '"", "Data_creazione DESC", objParametri_Server)

                'recupero le richieste presenti attualmente nel db
                'dtRichieste = richieste.Leggi(piva, richiesta_cod, "", 0, enumSelezioneVariabile.Selezione_TabellaCompleta, objParametri_Server)

                'recupero i dati dal planning con filtro su veg_cod e id_cod
                If Not String.IsNullOrWhiteSpace(Veg_Cod.ToString) AndAlso Not String.IsNullOrWhiteSpace(Id_Cod.ToString) Then
                    DtEntita = entita.Programmazione_Entita_Leggi(Programmazione_Cod, "", 0, "", piva, 0, 0, 0, 0, 0,
                                                              dataFit, DateTime.Now, 0, enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "Programmazione_Entita.Veg_Cod In ( " & Veg_Cod.ToString & " ) And Programmazione_Entita.Id_Cod In ( " & Id_Cod.ToString & " ) ", "Macrouso_Cod, Occupazione_Cod_Agea, Cul_Cod_Agea", objParametri_Server, Lettura_Per_UMA:=True)
                Else
                    DtEntita = New DataTable
                End If

                For Each row In DtEntita.Rows

                    dtTable_PendenzaApp = entita.Programmazione_Entita_Leggi_per_UMA_DT_Pendenza_Dettaglio(Programmazione_Cod, row.item("Programmazione_Entita_Cod"), objParametri_Server, richiesta_Cod:=richiesta_cod)
                    dtTable_Tessitura = entita.Programmazione_Entita_Leggi_per_UMA_DT_Tessitura_Dettaglio(Programmazione_Cod, row.item("Programmazione_Entita_Cod"), objParametri_Server, richiesta_cod:=richiesta_cod, avanzamento:=avanzamento)

                    If Not (IsDBNull(dtTable_PendenzaApp.Rows.Item(0).Item("sup_A")) AndAlso IsDBNull(dtTable_PendenzaApp.Rows.Item(0).Item("sup_B"))) Then

                        'costruisco i record come dovranno essere utilizzati nella griglia
                        d0 = Dt.NewRow
                        If (row.Item("Veg_Cod") = "0") Then
                            d0("Macrouso_Des") = row.Item("DestinazioneUso_Des")
                            d0("Macrouso_Cod") = row.Item("Id_Cod")
                        Else
                            d0("Macrouso_Des") = row.Item("Veg_Des")
                            d0("Macrouso_Cod") = row.Item("Veg_Cod")
                        End If
                        d0("Richiesta_Cod") = richiesta_cod
                        d0("Programmazione_Cod") = Programmazione_Cod
                        d0("sup_tot") = IIf(row.Item("Superficie") = dtTable_PendenzaApp.Rows.Item(0).Item("sup_A") + dtTable_PendenzaApp.Rows.Item(0).Item("sup_B"), row.Item("Superficie"), dtTable_PendenzaApp.Rows.Item(0).Item("sup_A") + dtTable_PendenzaApp.Rows.Item(0).Item("sup_B"))
                        d0("sup_A") = dtTable_PendenzaApp.Rows.Item(0).Item("sup_A")
                        d0("sup_B") = dtTable_PendenzaApp.Rows.Item(0).Item("sup_B")
                        d0("Cod") = row.Item("Programmazione_Entita_Cod") '.ToString.TrimEnd(" ")
                        d0("TerrenoNormale") = dtTable_Tessitura.Rows.Item(0).Item("sup_Normale")
                        d0("TerrenoMedio") = dtTable_Tessitura.Rows.Item(0).Item("sup_Media")
                        d0("TerrenoTenace") = dtTable_Tessitura.Rows.Item(0).Item("sup_Tenace")
                        d0("Occupazione_Cod") = row.Item("Occupazione_Cod_Agea")
                        d0("Uso_Cod") = row.Item("Uso_Cod_Agea")
                        d0("Qualita_Cod") = row.Item("Qualita_Cod_Agea")
                        d0("Destinazione_Cod") = row.Item("Destinazione_Cod_Agea")

                        Dt.Rows.Add(d0)

                    End If

                Next


            Next

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            'r.RispostaStringa = JsonConvert.SerializeObject(Dt, Formatting.None, serializerSettings)
            'r.RispostaOK = True

        Catch ex As Exception

            'r.RispostaOK = False

            ''uso questa funzione per ottenere il Messaggio..:
            'r.Errore = "Errore durante l'operazione: " & vbCrLf &
            'AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

            errore = "Errore durante l'operazione: " & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

            res = New Tuple(Of DataTable, String)(New DataTable, errore)

            Return res
        End Try

        'Return r
        res = New Tuple(Of DataTable, String)(Dt, errore)

        Return res

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Trova_Richieste_Dettaglio_Post2025(ByVal piva As String,
                                                     ByVal richiesta_cod As Integer,
                                                     ByVal macrouso_UMA_Cod As Integer,
                                                     ByVal Programmazione_Cod As Integer,
                                                     ByRef objParametri_Server As AgronicaCoreParametri,
                                                     ByRef objParametri_Super_Server As AgronicaCoreParametri) As Tuple(Of DataTable, String)

        Dim errore As String = ""
        Dim res As Tuple(Of DataTable, String)

        Dim r As New RispostaStandard
        Dim Dt As New DataTable

        Try

            Lingua.Gias_InizializzaCultura_DaSession()

            Dt.Columns.Add(New DataColumn("Macrouso_Des", GetType(String)))
            Dt.Columns.Add(New DataColumn("Macrouso_Cod", GetType(String)))
            Dt.Columns.Add(New DataColumn("Programmazione_Cod", GetType(Integer)))
            Dt.Columns.Add(New DataColumn("Richiesta_Cod", GetType(Integer)))
            Dt.Columns.Add(New DataColumn("sup_tot", GetType(Decimal)))
            Dt.Columns.Add(New DataColumn("sup_A", GetType(Decimal)))
            Dt.Columns.Add(New DataColumn("sup_B", GetType(Decimal)))
            Dt.Columns.Add(New DataColumn("Cod", GetType(String)))
            Dt.Columns.Add(New DataColumn("TerrenoNormale", GetType(Decimal)))
            Dt.Columns.Add(New DataColumn("TerrenoMedio", GetType(Decimal)))
            Dt.Columns.Add(New DataColumn("TerrenoTenace", GetType(Decimal)))
            Dt.Columns.Add(New DataColumn("Destinazione_Cod", GetType(String)))
            Dt.Columns.Add(New DataColumn("Uso_Cod", GetType(String)))
            Dt.Columns.Add(New DataColumn("Qualita_Cod", GetType(String)))
            Dt.Columns.Add(New DataColumn("Occupazione_Cod", GetType(String)))
            'se macrouso_UMA_Cod = 0 allora significa che voglio tutti i dettagli di tutti i macrousi quindi aggiungo altre info
            If macrouso_UMA_Cod = 0 Then
                Dt.Columns.Add(New DataColumn("Veg_Des", GetType(String)))
                Dt.Columns.Add(New DataColumn("Destinazione", GetType(String)))
                Dt.Columns.Add(New DataColumn("APP_NOME", GetType(String)))
                Dt.Columns.Add(New DataColumn("PROV", GetType(String)))
                Dt.Columns.Add(New DataColumn("COM", GetType(String)))
                Dt.Columns.Add(New DataColumn("Centro", GetType(String)))
                Dt.Columns.Add(New DataColumn("INIZIO_GESTIONE", GetType(Date)))
                Dt.Columns.Add(New DataColumn("FINE_GESTIONE", GetType(Date)))
            End If

            Dim d0 As DataRow

            'Dim dataFit As Date = #1/1/2000 12:00 PM#

            Dim testate As New AgronicaCoreUmaDal.UMA_Richieste_Testata_R
            'Dim anno = testate.RecuperaAnnoRichiesta(richiesta_cod, objParametri_Server)

            Dim umaRichiesta As New UMA_Richieste_R
            Dim strFiltro = ""
            Dim trovaTutti = True
            If macrouso_UMA_Cod > 0 Then
                strFiltro = "macrouso_UMA_Cod = '" & macrouso_UMA_Cod & "' "
                trovaTutti = False
            End If
            Dim dtColture = umaRichiesta.TrovaColtureAppezzamentiSenzaFascicolo(piva, richiesta_cod, objParametri_Server, xFiltroAggiuntivo:=strFiltro, TrovaTutti:=trovaTutti)
            Dim objAnalisi As New AgronicaCoreAnagrafeBIZ.Analisi_Modello_R
            objAnalisi.CalcolaTessiturexUMA(dtColture, richiesta_cod, objParametri_Server, objParametri_Super_Server)

            Dim Cod = 0

            For Each row In dtColture.Rows

                Cod = Cod + 1

                d0 = Dt.NewRow
                d0("Macrouso_Des") = row.Item("macrouso_UMA_Des")
                d0("Macrouso_Cod") = row.Item("macrouso_UMA_Cod")
                d0("Richiesta_Cod") = richiesta_cod
                d0("Programmazione_Cod") = Programmazione_Cod
                d0("Cod") = Cod
                d0("sup_A") = row.Item("sup_A")
                d0("sup_B") = row.Item("sup_B")
                d0("sup_tot") = row.Item("sup_A") + row.Item("sup_B")
                d0("TerrenoNormale") = row.Item("sup_Normale")
                d0("TerrenoMedio") = row.Item("sup_Media")
                d0("TerrenoTenace") = row.Item("sup_Tenace")
                d0("Occupazione_Cod") = row.Item("Occupazione_Agea")
                d0("Uso_Cod") = row.Item("Uso_Agea")
                d0("Qualita_Cod") = row.Item("Qualita_Agea")
                d0("Destinazione_Cod") = row.Item("Destinazione_Agea")
                If macrouso_UMA_Cod = 0 Then
                    d0("Veg_Des") = row.Item("Veg_Des")
                    d0("Destinazione") = row.Item("Destinazione")
                    d0("APP_NOME") = row.Item("APP_NOME")
                    d0("PROV") = row.Item("PROV")
                    d0("COM") = row.Item("COM")
                    d0("Centro") = row.Item("Centro")
                    d0("INIZIO_GESTIONE") = row.Item("INIZIO_GESTIONE")
                    d0("FINE_GESTIONE") = row.Item("FINE_GESTIONE")
                End If
                Dt.Rows.Add(d0)

            Next

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            r.RispostaStringa = JsonConvert.SerializeObject(Dt, Formatting.None, serializerSettings)
            r.RispostaOK = True

        Catch ex As Exception

            errore = "Errore durante l'operazione: " & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

            res = New Tuple(Of DataTable, String)(New DataTable, errore)

            Return res
        End Try

        'Return r
        res = New Tuple(Of DataTable, String)(Dt, errore)

        Return res

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Verifica_Appezzamenti(ByVal piva As String,
                                                 ByVal richiesta_cod As Integer,
                                                 ByVal tipo_richiesta As Integer,
                                                 ByVal avanzamento As Integer) As RispostaStandard

        Dim r As New RispostaStandard
        Dim Dt As New DataTable

        Try

            Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
                r.Sessione = False
                Return r
            End If

            Dim objParametri_Super_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Super_Server")
            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Super_Server) Then
                r.Sessione = False
                Return r
            End If

            Dim rendicontazione_terzista As Boolean = tipo_richiesta = -1 And avanzamento = 1

            Dim umaRichiestaXRegImpiantiR As New UMA_RichiesteXReg_Impianti_R
            Dim umaRichiestaXRegImpiantiW As New UMA_RichiesteXReg_Impianti_W
            Dim dtRichiestaXImpianti As DataTable

            '******** Verifica associazione impianti nuovi o eliminati ********

            dtRichiestaXImpianti = umaRichiestaXRegImpiantiR.Leggi_Richiesta_X_Reg_Impianti(piva, richiesta_cod, "", 0, rendicontazione_terzista, enumSelezioneVariabile.Selezione_TabellaDatiMinimi, objParametri_Server)

            Dim dtAssociazioniDaInserire As New DataTable
            Dim dtAssociazioniDaEliminare As DataTable = dtRichiestaXImpianti.Clone()

            Dim dtLegamiRichiesteAppezzamento As DataTable

            Dim umaRichiesteR As New AgronicaCoreUmaDal.UMA_Richieste_R
            Dim dtRichiestePresenti As DataTable = umaRichiesteR.Leggi(piva, richiesta_cod, "", 0, enumSelezioneVariabile.Selezione_TabellaDatiMinimi, objParametri_Server)

            If tipo_richiesta = 0 Then

                ' Estraggo tutti gli impianti associabili per l'azienda in conto proprio
                dtLegamiRichiesteAppezzamento = umaRichiesteR.EstraiLegamiRichiestaxAppezzamento(piva, richiesta_cod, "", objParametri_Server)

                dtAssociazioniDaInserire = dtLegamiRichiesteAppezzamento.Clone()

                Dim richiesteInsert As New ArrayList()
                Dim newMacrousi As New List(Of String)
                Dim macrouso_UMA_Cod As String

                ' Verifico le associazioni richiesta-impianto da inserire
                For Each row In dtLegamiRichiesteAppezzamento.Rows
                    If dtRichiestaXImpianti.Select("Piva_Impianto = '" & row("Piva") & "' AND Sa_Cod = " & row("Sa_Cod") & " AND APPEZZA = " & row("APPEZZA") & " AND ID_Reg = " & row("ID_Reg")).Count() = 0 Then
                        dtAssociazioniDaInserire.ImportRow(row)

                        ' Se manca inserisco anche la richiesta per il macrouso
                        macrouso_UMA_Cod = row("macrouso_UMA_Cod")
                        If dtRichiestePresenti.Select("Gruppo_Colturale_UMA = '" & macrouso_UMA_Cod & "'").Count() = 0 AndAlso Not newMacrousi.Contains(macrouso_UMA_Cod) Then
                            Dim Richiesta_Ins As New AgronicaCoreEntityFramework_POCO.UMA_Richieste With {
                            .Piva_SuperUser = objParametri_Server.PivaSuperUser,
                            .Piva = piva,
                            .Gruppo_Colturale_UMA = macrouso_UMA_Cod,
                            .Richiesta_Cod = richiesta_cod,
                            .Totale_Superficie_UMA = 0,
                            .Zona_Pendenza_A_UMA = 0,
                            .Zona_Pendenza_B_UMA = 0,
                            .Zona_Tessitura_Normale_UMA = 0,
                            .Zona_Tessitura_Media_UMA = 0,
                            .Zona_Tessitura_Tenace_UMA = 0,
                            .Carburante_Calcolato = 0,
                            .Carburante_Richiesto = 0,
                            .Carburante_Approvato = 0,
                            .Validita_Inizio = AGRODATAINIZIO,
                            .Validita_Fine = AGRODATAFINE,
                            .Username_Creazione = objParametri_Server.UtenteUsername,
                            .Username_Modifica = objParametri_Server.UtenteUsername,
                            .Data_Creazione = DateTime.Now,
                            .Data_Modifica = DateTime.Now,
                            .Totale_Superficie_UMA_Edit = 0,
                            .Zona_Pendenza_A_UMA_Edit = 0,
                            .Zona_Pendenza_B_UMA_Edit = 0,
                            .Zona_Tessitura_Normale_UMA_Edit = 0,
                            .Zona_Tessitura_Media_UMA_Edit = 0,
                            .Zona_Tessitura_Tenace_UMA_Edit = 0,
                            .Programmazione_Cod = CodiceFascicoloPianoColturaleGrafico,
                            .Regolamento_Cod = 0
                        }
                            richiesteInsert.Add(Richiesta_Ins)
                            newMacrousi.Add(macrouso_UMA_Cod)
                        End If

                    End If
                Next

                If richiesteInsert.Count > 0 Then
                    Dim umaRichiesteW As New AgronicaCoreUmaDal.UMA_Richieste_W
                    Dim Res = umaRichiesteW.Aggiorna_Richieste(richiesteInsert, New ArrayList(), New ArrayList(), objParametri_Server)
                End If

                If dtAssociazioniDaInserire.Rows.Count > 0 Then
                    umaRichiestaXRegImpiantiW.InserisciAssociazioniPerNuovaPraticaContoProprio(dtAssociazioniDaInserire, richiesta_cod, objParametri_Server)
                End If

                ' Verifico le associazioni richiesta-impianto da eliminare (non elimino subito l'associazione per non perdere il riferimento alla superfice da aggiornare)
                For Each row In dtRichiestaXImpianti.Rows
                    If dtLegamiRichiesteAppezzamento.Select("Piva = '" & row("Piva_Impianto") & "' AND Sa_Cod = " & row("Sa_Cod") & " AND Appezza = " & row("Appezza") & " AND ID_Reg = " & row("ID_Reg")).Count() = 0 Then
                        dtAssociazioniDaEliminare.ImportRow(row)
                    End If
                Next

            ElseIf rendicontazione_terzista Then

                ' Essendo terzista il controllo va effettuato separatamente per ogni azienda fra le richieste
                Dim lsPivaRichieste = (From row1 In dtRichiestePresenti Select row1.Field(Of String)("Piva")).Distinct().ToList()
                For Each pivaRichiesta In lsPivaRichieste

                    ' Estraggo tutti gli impianti associabili per l'azienda della relativa richiesta
                    dtLegamiRichiesteAppezzamento = umaRichiesteR.EstraiLegamiRichiestaxAppezzamento(pivaRichiesta, richiesta_cod, "", objParametri_Server)

                    dtAssociazioniDaInserire = dtLegamiRichiesteAppezzamento.Clone()

                    ' Filtro per i soli macrousi esistenti
                    Dim lsMacrousi = (From row1 In dtRichiestePresenti.Select("Piva = '" & pivaRichiesta & "'")
                                      Select row1.Field(Of String)("Gruppo_Colturale_UMA")).Distinct().ToList()
                    Dim dsLegamiRichiesteAppezzamento = dtLegamiRichiesteAppezzamento.Select("macrouso_UMA_Cod in ('" & String.Join("','", lsMacrousi) & "')")

                    ' Verifico le associazioni richiesta-impianto da inserire
                    For Each row In dsLegamiRichiesteAppezzamento
                        If dtRichiestaXImpianti.Select("Piva_Impianto = '" & row("Piva") & "' AND Sa_Cod = " & row("Sa_Cod") & " AND APPEZZA = " & row("APPEZZA") & " AND ID_Reg = " & row("ID_Reg")).Count() = 0 AndAlso lsMacrousi.Contains(row("macrouso_UMA_Cod")) Then
                            dtAssociazioniDaInserire.ImportRow(row)
                        End If
                    Next

                    If dtAssociazioniDaInserire.Rows.Count > 0 Then
                        umaRichiestaXRegImpiantiW.InserisciAssociazioniPerNuovaPraticaContoTerzi(piva, dtAssociazioniDaInserire, richiesta_cod, objParametri_Server)
                    End If

                    ' Verifico le associazioni richiesta-impianto da eliminare (non elimino subito l'associazione per non perdere il riferimento alla superfice da aggiornare)
                    For Each row In dtRichiestaXImpianti.Select("Piva_Impianto = '" & pivaRichiesta & "'")
                        If dtLegamiRichiesteAppezzamento.Select("Piva = '" & pivaRichiesta & "' AND Sa_Cod = " & row("Sa_Cod") & " AND Appezza = " & row("Appezza") & " AND ID_Reg = " & row("ID_Reg")).Count() = 0 Then
                            dtAssociazioniDaEliminare.ImportRow(row)
                        End If
                    Next
                Next

            End If

            '******** Verifica sulle superfici ********

            Dt.Columns.Add(New DataColumn("Piva_Impianto", GetType(String)))
            Dt.Columns.Add(New DataColumn("Gruppo_Colturale_UMA_Cod", GetType(String)))
            Dt.Columns.Add(New DataColumn("Gruppo_Colturale_UMA_Des", GetType(String)))
            Dt.Columns.Add(New DataColumn("Programmazione_Cod", GetType(Integer)))
            Dt.Columns.Add(New DataColumn("Totale_Superficie_UMA", GetType(Decimal)))
            Dt.Columns.Add(New DataColumn("Totale_Superficie_Rilevata", GetType(Decimal)))
            Dt.Columns.Add(New DataColumn("Zona_Pendenza_A_UMA", GetType(Decimal)))
            Dt.Columns.Add(New DataColumn("Zona_Pendenza_A_Rilevata", GetType(Decimal)))
            Dt.Columns.Add(New DataColumn("Zona_Pendenza_B_UMA", GetType(Decimal)))
            Dt.Columns.Add(New DataColumn("Zona_Pendenza_B_Rilevata", GetType(Decimal)))
            Dt.Columns.Add(New DataColumn("Zona_Tessitura_Normale_UMA", GetType(Decimal)))
            Dt.Columns.Add(New DataColumn("Zona_Tessitura_Normale_Rilevata", GetType(Decimal)))
            Dt.Columns.Add(New DataColumn("Zona_Tessitura_Media_UMA", GetType(Decimal)))
            Dt.Columns.Add(New DataColumn("Zona_Tessitura_Media_Rilevata", GetType(Decimal)))
            Dt.Columns.Add(New DataColumn("Zona_Tessitura_Tenace_UMA", GetType(Decimal)))
            Dt.Columns.Add(New DataColumn("Zona_Tessitura_Tenace_Rilevata", GetType(Decimal)))
            Dt.Columns.Add(New DataColumn("Zona_Tessitura_Aggiornata", GetType(Boolean)))

            Dim d0 As DataRow

            Dim cessati = umaRichiestaXRegImpiantiR.Controlla_Presenza_Impianti_Cessati(piva, richiesta_cod, objParametri_Server)

            dtRichiestaXImpianti = umaRichiestaXRegImpiantiR.Leggi_Richiesta_X_Reg_Impianti(piva, richiesta_cod, "", 0, rendicontazione_terzista,
                                                                                            enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                                                                            objParametri_Server,
                                                                                            filtroSuLogOpAnagraficheSuccessive:=IIf((cessati.Rows.Count > 0 OrElse dtAssociazioniDaInserire.Rows.Count > 0), False, True))

            If dtRichiestaXImpianti.Rows.Count > 0 Then

                Dim Piva_Impianto As String
                Dim Gruppo_Colturale_Cod As String
                Dim Gruppo_Colturale_Des As String
                Dim Programmazione_Cod As Integer
                Dim Totale_Superficie_UMA As Decimal
                Dim Zona_Pendenza_A_UMA As Decimal
                Dim Zona_Pendenza_B_UMA As Decimal
                Dim Zona_Tessitura_Normale_UMA As Decimal
                Dim Zona_Tessitura_Media_UMA As Decimal
                Dim Zona_Tessitura_Tenace_UMA As Decimal

                Dim umaRichiesta As New UMA_Richieste_R
                Dim dtColture As DataTable
                Dim lsMacrousi = (From row1 In dtRichiestaXImpianti Select row1.Field(Of String)("Gruppo_Colturale_UMA")).Distinct().ToList()
                Dim strFiltroColtureAppezzamenti As String = "macrouso_UMA_Cod in ('" & String.Join("', '", lsMacrousi) & "') "

                ' Per terzisti la PIVA azienda non corrisponde alle PIVA degli impianti 
                If rendicontazione_terzista Then
                    dtColture = Nothing
                    Dim lsPivaImpianto = (From row1 In dtRichiestaXImpianti Select row1.Field(Of String)("Piva_Impianto")).Distinct().ToList()
                    For Each pivaImpianto In lsPivaImpianto
                        If IsNothing(dtColture) Then
                            dtColture = umaRichiesta.TrovaColtureAppezzamentiSenzaFascicolo(pivaImpianto, richiesta_cod, objParametri_Server, xFiltroAggiuntivo:=strFiltroColtureAppezzamenti)
                        Else
                            dtColture.Merge(umaRichiesta.TrovaColtureAppezzamentiSenzaFascicolo(pivaImpianto, richiesta_cod, objParametri_Server, xFiltroAggiuntivo:=strFiltroColtureAppezzamenti))
                        End If
                    Next
                Else
                    dtColture = umaRichiesta.TrovaColtureAppezzamentiSenzaFascicolo(piva, richiesta_cod, objParametri_Server, xFiltroAggiuntivo:=strFiltroColtureAppezzamenti)
                End If

                Dim objAnalisi As New AgronicaCoreAnagrafeBIZ.Analisi_Modello_R
                objAnalisi.CalcolaTessiturexUMA(dtColture, richiesta_cod, objParametri_Server, objParametri_Super_Server)

                Dim drColture As DataRow()
                Dim totSup As Decimal
                Dim totSupA As Decimal
                Dim totSupB As Decimal
                Dim totSupNormale As Decimal
                Dim totSupMedia As Decimal
                Dim totSupTenace As Decimal
                Dim tessituraAggiornata As Boolean

                Dim dtRichieste = dtRichiestaXImpianti.DefaultView.ToTable(True,
                                                                         {
                                                                            "Piva_Impianto",
                                                                            "Gruppo_Colturale_UMA",
                                                                            "Macrouso_UMA_Des",
                                                                            "Programmazione_Cod",
                                                                            "Totale_Superficie_UMA",
                                                                            "Zona_Pendenza_A_UMA",
                                                                            "Zona_Pendenza_B_UMA",
                                                                            "Zona_Tessitura_Normale_UMA",
                                                                            "Zona_Tessitura_Normale_UMA_Edit",
                                                                            "Zona_Tessitura_Media_UMA",
                                                                            "Zona_Tessitura_Media_UMA_Edit",
                                                                            "Zona_Tessitura_Tenace_UMA",
                                                                            "Zona_Tessitura_Tenace_UMA_Edit"
                                                                         })
                For Each row In dtRichieste.Rows

                    Piva_Impianto = row.Item("Piva_Impianto")
                    Gruppo_Colturale_Cod = row.Item("Gruppo_Colturale_UMA")
                    Gruppo_Colturale_Des = row.Item("Macrouso_UMA_Des")
                    Programmazione_Cod = row.Item("Programmazione_Cod")
                    Totale_Superficie_UMA = row.Item("Totale_Superficie_UMA")
                    Zona_Pendenza_A_UMA = row.Item("Zona_Pendenza_A_UMA")
                    Zona_Pendenza_B_UMA = row.Item("Zona_Pendenza_B_UMA")
                    Zona_Tessitura_Normale_UMA = row.Item("Zona_Tessitura_Normale_UMA")
                    Zona_Tessitura_Media_UMA = row.Item("Zona_Tessitura_Media_UMA")
                    Zona_Tessitura_Tenace_UMA = row.Item("Zona_Tessitura_Tenace_UMA")

                    drColture = dtColture.Select("Piva = '" & Piva_Impianto & "' AND macrouso_UMA_Cod = '" & Gruppo_Colturale_Cod & "'")

                    totSup = 0
                    totSupA = 0
                    totSupB = 0
                    totSupNormale = 0
                    totSupMedia = 0
                    totSupTenace = 0
                    For Each row1 In drColture
                        totSup += row1.Item("SUP_APP")
                        totSupA += row1.Item("Sup_A")
                        totSupB += row1.Item("Sup_B")
                        totSupNormale += row1.Item("Sup_Normale")
                        totSupMedia += row1.Item("Sup_Media")
                        totSupTenace += row1.Item("Sup_Tenace")
                    Next

                    'Casadei 16/12/2025 converto tutti i valori a 4 decimali per evitare problemi di arrotondamento strani

                    totSup = Math.Round(totSup, 4)
                    totSupA = Math.Round(totSupA, 4)
                    totSupB = Math.Round(totSupB, 4)
                    totSupNormale = Math.Round(totSupNormale, 4)
                    totSupMedia = Math.Round(totSupMedia, 4)
                    totSupTenace = Math.Round(totSupTenace, 4)

                    Totale_Superficie_UMA = Math.Round(Totale_Superficie_UMA, 4)
                    Zona_Pendenza_A_UMA = Math.Round(Zona_Pendenza_A_UMA, 4)
                    Zona_Pendenza_B_UMA = Math.Round(Zona_Pendenza_B_UMA, 4)
                    Zona_Tessitura_Normale_UMA = Math.Round(Zona_Tessitura_Normale_UMA, 4)
                    Zona_Tessitura_Media_UMA = Math.Round(Zona_Tessitura_Media_UMA, 4)
                    Zona_Tessitura_Tenace_UMA = Math.Round(Zona_Tessitura_Tenace_UMA, 4)

                    If totSup <> Totale_Superficie_UMA OrElse
                       totSupA <> Zona_Pendenza_A_UMA OrElse
                       totSupB <> Zona_Pendenza_B_UMA OrElse
                       totSupNormale <> Zona_Tessitura_Normale_UMA OrElse
                       totSupMedia <> Zona_Tessitura_Media_UMA OrElse
                       totSupTenace <> Zona_Tessitura_Tenace_UMA Then

                        tessituraAggiornata =
                            Zona_Tessitura_Normale_UMA <> row.Item("Zona_Tessitura_Normale_UMA_Edit") OrElse
                            Zona_Tessitura_Media_UMA <> row.Item("Zona_Tessitura_Media_UMA_Edit") OrElse
                            Zona_Tessitura_Tenace_UMA <> row.Item("Zona_Tessitura_Tenace_UMA_Edit")

                        d0 = Dt.NewRow
                        d0("Piva_Impianto") = Piva_Impianto
                        d0("Gruppo_Colturale_UMA_Cod") = Gruppo_Colturale_Cod
                        d0("Gruppo_Colturale_UMA_Des") = Gruppo_Colturale_Des
                        d0("Programmazione_Cod") = Programmazione_Cod
                        d0("Totale_Superficie_UMA") = Totale_Superficie_UMA
                        d0("Totale_Superficie_Rilevata") = totSup
                        d0("Zona_Pendenza_A_UMA") = Zona_Pendenza_A_UMA
                        d0("Zona_Pendenza_A_Rilevata") = totSupA
                        d0("Zona_Pendenza_B_UMA") = Zona_Pendenza_B_UMA
                        d0("Zona_Pendenza_B_Rilevata") = totSupB
                        d0("Zona_Tessitura_Normale_UMA") = Zona_Tessitura_Normale_UMA
                        d0("Zona_Tessitura_Normale_Rilevata") = totSupNormale
                        d0("Zona_Tessitura_Media_UMA") = Zona_Tessitura_Media_UMA
                        d0("Zona_Tessitura_Media_Rilevata") = totSupMedia
                        d0("Zona_Tessitura_Tenace_UMA") = Zona_Tessitura_Tenace_UMA
                        d0("Zona_Tessitura_Tenace_Rilevata") = totSupTenace
                        d0("Zona_Tessitura_Aggiornata") = tessituraAggiornata
                        Dt.Rows.Add(d0)
                    End If

                Next

            End If

            ' Per poter individuare le superfici variate, le associazioni Richieste-Impianti vanno eliminate dopo la lettura 
            If dtAssociazioniDaEliminare.Rows.Count > 0 Then
                ' Prima di eliminare le associazioni, verifico che le anomalie siano state risolte, altrimenti non è più possibile rilevare i cambiamenti di superficie
                Dim dtAssociazioniDaEliminare2 As DataTable = dtAssociazioniDaEliminare.Clone()
                For Each row In dtAssociazioniDaEliminare.Rows
                    If Dt.Select("Gruppo_Colturale_UMA_Cod = '" & row.item("Gruppo_Colturale_UMA") & "'").Count = 0 Then
                        dtAssociazioniDaEliminare2.ImportRow(row)
                    End If
                Next

                umaRichiestaXRegImpiantiW.EliminaAssociazioni(piva, richiesta_cod, dtAssociazioniDaEliminare2, objParametri_Server)
            End If

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            r.RispostaStringa = JsonConvert.SerializeObject(Dt, Formatting.None, serializerSettings)
            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
            AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Correggi_Superfici_Appezzamenti(ByVal piva As String,
                                                           ByVal richiesta_cod As Integer,
                                                           ByVal tipo_richiesta As Integer,
                                                           ByVal avanzamento As Integer,
                                                           ByVal Regione_Cod As String,
                                                           ByVal appezzamentiDataStr As String) As RispostaStandard

        Dim r As New RispostaStandard
        Dim errorMsg = ""

        Try

            Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
                r.Sessione = False
                Return r
            End If

            Dim objParametri_Super_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Super_Server")
            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Super_Server) Then
                r.Sessione = False
                Return r
            End If

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            Dim dtAppezzamentiData As DataTable = JsonConvert.DeserializeObject(Of DataTable)(appezzamentiDataStr, serializerSettings)

            If Not IsNothing(dtAppezzamentiData) AndAlso dtAppezzamentiData.Rows.Count > 0 Then

                Dim umaRichieste As New AgronicaCoreUmaBiz.UMA_Richieste
                Dim aggiornaRes As RispostaStandard

                Dim Piva_Impianto As String
                Dim Gruppo_Colturale As String
                Dim Programmazione_Cod As Integer
                Dim totSup As Decimal
                Dim totSupA As Decimal
                Dim totSupB As Decimal
                Dim totSupNormale As Decimal
                Dim totSupMedia As Decimal
                Dim totSupTenace As Decimal

                For Each row In dtAppezzamentiData.Rows

                    Piva_Impianto = row.Item("Piva_Impianto")
                    Gruppo_Colturale = row.Item("Gruppo_Colturale_UMA_Cod")
                    Programmazione_Cod = row.Item("Programmazione_Cod")
                    totSup = row.Item("Totale_Superficie_Rilevata")
                    totSupA = row.Item("Zona_Pendenza_A_Rilevata")
                    totSupB = row.Item("Zona_Pendenza_B_Rilevata")
                    totSupNormale = row.Item("Zona_Tessitura_Normale_Rilevata")
                    totSupMedia = row.Item("Zona_Tessitura_Media_Rilevata")
                    totSupTenace = row.Item("Zona_Tessitura_Tenace_Rilevata")

                    aggiornaRes = umaRichieste.AggiornaTotaliSuperfici(Piva_Impianto, richiesta_cod, Gruppo_Colturale, Programmazione_Cod, Regione_Cod, totSup, totSupA, totSupB, totSupNormale, totSupMedia, totSupTenace, objParametri_Server)

                    If Not aggiornaRes.RispostaOK Then
                        errorMsg = errorMsg & aggiornaRes.Errore & vbCrLf
                    End If

                Next

                'Ricalcolo ed aggiorno il carburante totale calcolato/richiesto per testata
                'NOTA: aggiorno comunque anche se i valori non dovessero essere cambiati, per poter aggiornare Data_Modifica in testata

                Dim objRichiesteR As New AgronicaCoreUmaDal.UMA_Richieste_R
                Dim dtRichieste = If(tipo_richiesta = -1,
                    objRichiesteR.Leggi_Richieste_Terzisti(piva, richiesta_cod, 0, objParametri_Server),
                    objRichiesteR.Leggi(piva, richiesta_cod, "", 0, enumSelezioneVariabile.Selezione_TabellaCompleta, objParametri_Server))

                Dim carbCalcolato As Double = 0
                Dim carbRichiesto As Double = 0
                For Each row In dtRichieste.Rows
                    carbCalcolato = carbCalcolato + row.Item("Carburante_Calcolato")
                    carbRichiesto = carbRichiesto + row.Item("Carburante_Richiesto")
                Next

                Dim objTestataW As New AgronicaCoreUmaDal.UMA_Richieste_Testata_W
                objTestataW.Modifica_Campo_Richiesta("Carburante_Calcolato", carbCalcolato, piva, richiesta_cod, objParametri_Server)
                objTestataW.Modifica_Campo_Richiesta("Carburante_Richiesto", carbRichiesto, piva, richiesta_cod, objParametri_Server)

            End If

            If errorMsg = "" Then
                r.RispostaOK = True
                r.RispostaStringa = "Superfici dichiarate per richiesta aggiornate correttamente."
            Else
                r.RispostaOK = False
                r.Errore = errorMsg
            End If

        Catch ex As Exception

            r.RispostaOK = False
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
            AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Leggi_Appezzamenti_Programmazione(ByVal piva As String, ByVal programmazione_cod As Integer, ByVal richiesta_Cod As Integer) As RispostaStandard

        Dim strAppezzamenti As String = ""
        Dim r As New RispostaStandard

        Try

            Dim NomeRoutine As String = "Leggi_Appezzamenti_Programmazione"
            Dim MsgOK As String = ""
            Dim objLog As New AgronicaCoreDataProvider.LogProvider

            Dim strErr As String = ""

            Dim bAggregaSpecie As Boolean = False
            Dim bDividiCentri As Boolean = False
            Dim bAggregaTare As Boolean = False

            Dim Validita_Inizio As Date = AGRODATAINIZIO
            Dim Validita_Fine As Date = AGRODATAFINE


            Dim Programmazione_Des As String = ""
            Dim Programmazione_Des_Long As String = ""
            Dim Tipo_Pianificazione As Integer
            Dim TuttiCentri As Boolean

            Dim DtParticelle As New DataTable
            DtParticelleCreaStruttura(DtParticelle)

            Dim DR As DataRow
            Dim i As Integer

            Dim chiave As Integer = 1

            Dim DT_Appezzamenti As New DataTable
            Dim DT_Intersezioni As New DataTable
            Dim objP As New AgronicaCoreAnagrafeBIZ.Programmazione_R
            Dim serializerSettings As New JsonSerializerSettings()
            objP.DT_Appezzamenti_Crea(DT_Appezzamenti)
            objP.DT_Intersezioni_Crea(DT_Intersezioni)

            Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
                'Return strAppezzamenti 'da verificare se restituire un errore
                r.Sessione = False
                Return r
            End If

            Dim provenienza_fascicolo As String = ""

            Dim objP_R As New AgronicaCoreAnagrafeBIZ.Programmazione_R
            If programmazione_cod >= 0 Then
                objP_R.Pianificazione_Leggi(programmazione_cod,
                                        Programmazione_Des, Programmazione_Des_Long, "", 0, "", Validita_Inizio, Validita_Fine, Tipo_Pianificazione,
                                        DT_Appezzamenti, DT_Intersezioni,
                                        TuttiCentri, objParametri_Server,
                                        True, True, True, True)

                If DT_Appezzamenti IsNot Nothing Then

                    For i = 0 To DT_Appezzamenti.Rows.Count - 1

                        DR = DtParticelle.NewRow

                        DR.Item("PROV") = ""
                        DR.Item("COM") = ""
                        DR.Item("Prov_Des") = ""
                        DR.Item("Com_Des") = ""
                        'DR.Item("Sezione") = "0"
                        'DR.Item("Foglio") = 0
                        'DR.Item("Numero") = 0
                        'DR.Item("Subalterno") = "0"

                        DR.Item("macrouso_cod") = DT_Appezzamenti.Rows(i).Item("macrouso_cod")

                        'DR.Item("macrouso_sup") = Macrouso_Sup
                        DR.Item("macrouso") = DT_Appezzamenti.Rows(i).Item("macrouso_des")
                        DR.Item("utilizzo") = DT_Appezzamenti.Rows(i).Item("veg_des_cliente")
                        DR.Item("varieta") = DT_Appezzamenti.Rows(i).Item("cul_des_cliente")
                        DR.Item("utilizzo_sup") = DT_Appezzamenti.Rows(i).Item("sup_app")

                        DR.Item("Veg_Cod_Agea") = DT_Appezzamenti.Rows(i).Item("Veg_Cod_Agea")
                        DR.Item("Cul_Cod_Agea") = DT_Appezzamenti.Rows(i).Item("Cul_Cod_Agea")

                        DR.Item("Veg_Cod") = DT_Appezzamenti.Rows(i).Item("Veg_Cod") & "|" & DT_Appezzamenti.Rows(i).Item("destinazioneuso")

                        DR.Item("Cul_Cod") = DT_Appezzamenti.Rows(i).Item("Cul_Cod")
                        DR.Item("Grfi_Cod") = DT_Appezzamenti.Rows(i).Item("Grfi_Cod")
                        DR.Item("Grva_Cod") = DT_Appezzamenti.Rows(i).Item("Grva_Cod")
                        DR.Item("Id_Cod") = 0 'DT_Appezzamenti.Rows(i).Item("destinazioneuso")

                        If DT_Appezzamenti.Rows(i).Item("Veg_Cod") <> 0 Then
                            DR.Item("veg_des") = DT_Appezzamenti.Rows(i).Item("veg_des")
                        Else
                            If DT_Appezzamenti.Rows(i).Item("destinazioneuso") = 0 Then
                                DR.Item("veg_des") = DT_Appezzamenti.Rows(i).Item("veg_des")
                            Else
                                DR.Item("veg_des") = DT_Appezzamenti.Rows(i).Item("destinazioneuso_des")
                            End If
                        End If

                        DR.Item("cul_des") = DT_Appezzamenti.Rows(i).Item("cul_des")
                        DR.Item("grfi_des") = DT_Appezzamenti.Rows(i).Item("grfi_des")
                        DR.Item("grva_des") = DT_Appezzamenti.Rows(i).Item("grva_des")

                        DR.Item("Scarto") = 0
                        DR.Item("validita_inizio") = DT_Appezzamenti.Rows(i).Item("validita_inizio")
                        DR.Item("validita_fine") = DT_Appezzamenti.Rows(i).Item("validita_fine")

                        DR.Item("sa_nome") = DT_Appezzamenti.Rows(i).Item("sa_nome")
                        DR.Item("App_Nome") = DT_Appezzamenti.Rows(i).Item("app_nome")

                        DR.Item("piva") = DT_Appezzamenti.Rows(i).Item("piva")
                        DR.Item("sa_cod") = DT_Appezzamenti.Rows(i).Item("sa_cod")
                        DR.Item("appezza") = DT_Appezzamenti.Rows(i).Item("appezza")

                        DR.Item("programmazione_entita_cod") = DT_Appezzamenti.Rows(i).Item("programmazione_entita_cod")

                        Dim DrTmp() As DataRow
                        DrTmp = DT_Intersezioni.Select("Piva='" & DT_Appezzamenti.Rows(i).Item("piva") & "' AND Sa_Cod=" & DT_Appezzamenti.Rows(i).Item("sa_cod") & " AND Appezza=" & DT_Appezzamenti.Rows(i).Item("appezza"))

                        'DR.Item("PROV") = ""
                        'DR.Item("COM") = ""
                        'DR.Item("SEZIONE") = ""
                        'DR.Item("FOGLIO") = ""
                        'DR.Item("NUMERO") = ""
                        'DR.Item("SUBALTERNO") = ""

                        If DrTmp IsNot Nothing Then

                            If DrTmp.Length > 0 Then
                                For p = 0 To DrTmp.Length - 1
                                    If p = 0 Then
                                        DR.Item("Catasto") = " [ "
                                    End If

                                    If p <> DrTmp.Length - 1 Then
                                        DR.Item("Catasto") &= ImpostaCatasto(Istat_Prov:=DrTmp(p).Item("PROV"),
                                                                     Prov:=DrTmp(p).Item("Prov_Des"),
                                                                     Istat_Com:=DrTmp(p).Item("COM"),
                                                                     Com:=DrTmp(p).Item("Com_Des"),
                                                                     Sezione:=DrTmp(p).Item("Sezione"),
                                                                     Foglio:=DrTmp(p).Item("Foglio").ToString,
                                                                     Numero:=DrTmp(p).Item("Numero").ToString,
                                                                     Subalterno:=DrTmp(p).Item("Subalterno"),
                                                                     Sup_Condotta:=DrTmp(p).Item("SupTotale").ToString,
                                                                     possesso:=DrTmp(p).Item("possesso"),
                                                                     datepossesso:=DrTmp(p).Item("datepossesso"),
                                                                     inizio_possesso:=DrTmp(p).Item("Validita_Inizio"),
                                                                     fine_possesso:=DrTmp(p).Item("Validita_Fine"),
                                                                     sup:=DrTmp(p).Item("SupCatastale").ToString,
                                                                     utilizzo_sup:=DrTmp(p).Item("SupIntersezione").ToString) & " , "
                                    End If

                                    If p = DrTmp.Length - 1 Then
                                        DR.Item("Catasto") &= ImpostaCatasto(Istat_Prov:=DrTmp(p).Item("PROV"),
                                                                     Prov:=DrTmp(p).Item("Prov_Des"),
                                                                     Istat_Com:=DrTmp(p).Item("COM"),
                                                                     Com:=DrTmp(p).Item("Com_Des"),
                                                                     Sezione:=DrTmp(p).Item("Sezione"),
                                                                     Foglio:=DrTmp(p).Item("Foglio").ToString,
                                                                     Numero:=DrTmp(p).Item("Numero").ToString,
                                                                     Subalterno:=DrTmp(p).Item("Subalterno"),
                                                                     Sup_Condotta:=DrTmp(p).Item("SupTotale").ToString,
                                                                     possesso:=DrTmp(p).Item("possesso"),
                                                                     datepossesso:=DrTmp(p).Item("datepossesso"),
                                                                     inizio_possesso:=DrTmp(p).Item("Validita_Inizio"),
                                                                     fine_possesso:=DrTmp(p).Item("Validita_Fine"),
                                                                     sup:=DrTmp(p).Item("SupCatastale").ToString,
                                                                     utilizzo_sup:=DrTmp(p).Item("SupIntersezione").ToString) & " ] "
                                    End If

                                    DR.Item("Catasto_Key") &= IIf(p <> 0, "<BR>", "") &
                                                            DrTmp(p).Item("PROV") & "_" &
                                                            DrTmp(p).Item("COM") & "_" &
                                                            DrTmp(p).Item("Sezione") & "_" &
                                                            DrTmp(p).Item("Foglio").ToString & "_" &
                                                            DrTmp(p).Item("Numero").ToString & "_" &
                                                            DrTmp(p).Item("Subalterno") & ":" &
                                                            DrTmp(p).Item("SupIntersezione").ToString
                                Next
                            Else
                                DR.Item("Catasto") = "[ ]"
                            End If


                        End If

                        If JArray.Parse(DR.Item("Catasto")).Count = 1 Then
                            For Each jCat As JObject In JArray.Parse(DR.Item("Catasto"))
                                DR.Item("PROV") = jCat("Istat_Prov")
                                DR.Item("prov_des") = jCat("Prov")
                                DR.Item("COM") = jCat("Istat_Com")
                                DR.Item("com_des") = jCat("Com")
                                DR.Item("sezione") = jCat("Sezione")
                                DR.Item("foglio") = jCat("Foglio")
                                DR.Item("numero") = jCat("Numero")
                                DR.Item("subalterno") = jCat("Subalterno")
                            Next
                        Else
                            DR.Item("PROV") = ""
                            DR.Item("prov_des") = ""
                            DR.Item("COM") = ""
                            DR.Item("com_des") = ""
                            DR.Item("sezione") = ""
                            DR.Item("foglio") = 0
                            DR.Item("numero") = 0
                            DR.Item("subalterno") = ""
                        End If

                        DR.Item("grva_des") = DT_Appezzamenti.Rows(i).Item("grva_des")
                        DR.Item("cop_cod") = DT_Appezzamenti.Rows(i).Item("cop_cod")
                        DR.Item("cop_des") = DT_Appezzamenti.Rows(i).Item("cop_des")
                        DR.Item("lotto") = DT_Appezzamenti.Rows(i).Item("Progetto_Nome")
                        DR.Item("resa") = DT_Appezzamenti.Rows(i).Item("resa")
                        DR.Item("num_piante") = DT_Appezzamenti.Rows(i).Item("num_piante")
                        DR.Item("TRA_fila") = DT_Appezzamenti.Rows(i).Item("TRA_fila")
                        DR.Item("SU_fila") = DT_Appezzamenti.Rows(i).Item("SU_fila")
                        DR.Item("Validita_Inizio_Impianto") = DT_Appezzamenti.Rows(i).Item("Validita_Inizio_Impianto")
                        DR.Item("TipoZona") = DT_Appezzamenti.Rows(i).Item("TipoZona")
                        'DR.Item("TipoZona_Des") = DT_Appezzamenti.Rows(i).Item("TipoZona_Des")
                        DR.Item("MetodoProduzione_Cod") = DT_Appezzamenti.Rows(i).Item("MetodoProduzione_Cod")
                        'DR.Item("MetodoProduzione_Des") = DT_Appezzamenti.Rows(i).Item("MetodoProduzione_Des")
                        DR.Item("Unita_Vitata") = DT_Appezzamenti.Rows(i).Item("Unita_Vitata")
                        DR.Item("Veg_Cod_Agea") = DT_Appezzamenti.Rows(i).Item("Veg_Cod_Agea")
                        'DR.Item("Veg_Des_Agea") = DT_Appezzamenti.Rows(i).Item("Veg_Des_Agea")
                        DR.Item("Cul_Cod_Agea") = DT_Appezzamenti.Rows(i).Item("Cul_Cod_Agea")
                        'DR.Item("Cul_Des_Agea") = DT_Appezzamenti.Rows(i).Item("Cul_Des_Agea")
                        DR.Item("Uso_Cod_Agea") = DT_Appezzamenti.Rows(i).Item("Uso_Cod_Agea")
                        'DR.Item("Uso_Des_Agea") = DT_Appezzamenti.Rows(i).Item("Uso_Des_Agea")
                        DR.Item("Occupazione_Cod_Agea") = DT_Appezzamenti.Rows(i).Item("Occupazione_Cod_Agea")
                        ' DR.Item("Occupazione_Des_Agea") = DT_Appezzamenti.Rows(i).Item("Occupazione_Des_Agea")
                        DR.Item("Destinazione_Cod_Agea") = DT_Appezzamenti.Rows(i).Item("Destinazione_Cod_Agea")
                        'DR.Item("Destinazione_Des_Agea") = DT_Appezzamenti.Rows(i).Item("Destinazione_Des_Agea")
                        DR.Item("Qualita_Cod_Agea") = DT_Appezzamenti.Rows(i).Item("Qualita_Cod_Agea")
                        'DR.Item("Qualita_Des_Agea") = DT_Appezzamenti.Rows(i).Item("Qualita_Des_Agea")
                        'DR.Item("Gru_Cod") = DT_Appezzamenti.Rows(i).Item("Gru_Cod")
                        DR.Item("Dpi_Cod") = DT_Appezzamenti.Rows(i).Item("Disciplinare_Cod")
                        DR.Item("Reg_Cod") = DT_Appezzamenti.Rows(i).Item("Regolamento_Cod")
                        DR.Item("StatoImpianto_Cod") = DT_Appezzamenti.Rows(i).Item("Stato_Cod")
                        DR.Item("N") = DT_Appezzamenti.Rows(i).Item("Limite_N")
                        DR.Item("P") = DT_Appezzamenti.Rows(i).Item("Limite_P")
                        DR.Item("K") = DT_Appezzamenti.Rows(i).Item("Limite_K")
                        DR.Item("Data_Semina") = DT_Appezzamenti.Rows(i).Item("Data_Semina")
                        DR.Item("Data_Raccolta") = DT_Appezzamenti.Rows(i).Item("Data_Raccolta")
                        DR.Item("Data_Fioritura") = DT_Appezzamenti.Rows(i).Item("Data_Fioritura_Prevista")
                        DR.Item("Coltura_Precedente") = DT_Appezzamenti.Rows(i).Item("Veg_Cod_Prec")
                        DR.Item("Coltura_Precedente2") = DT_Appezzamenti.Rows(i).Item("Veg_Cod_Prec2")
                        DR.Item("Coltura_Precedente3") = DT_Appezzamenti.Rows(i).Item("Veg_Cod_Prec3")
                        DR.Item("Coltura_Precedente4") = DT_Appezzamenti.Rows(i).Item("Veg_Cod_Prec4")
                        DR.Item("Piano_Semina") = DT_Appezzamenti.Rows(i).Item("Piano_Semina")
                        DR.Item("Codice_Contratto") = DT_Appezzamenti.Rows(i).Item("Codice_Contratto")
                        'DR.Item("unito") = DT_Appezzamenti.Rows(i).Item("unito")
                        'DR.Item("frazionato") = DT_Appezzamenti.Rows(i).Item("frazionato")


                        DR.Item("appezza") = DT_Appezzamenti.Rows(i).Item("appezza")

                        DR.Item("ribaltato") = DT_Appezzamenti.Rows(i).Item("ribaltato")
                        DR.Item("movimentato") = DT_Appezzamenti.Rows(i).Item("movimentato")

                        DR.Item("stato_ribaltamento") = 0
                        If Not IsDBNull(DT_Appezzamenti.Rows(i).Item("stato_ribaltamento")) AndAlso IsNumeric(DT_Appezzamenti.Rows(i).Item("stato_ribaltamento")) Then
                            DR.Item("stato_ribaltamento") = DT_Appezzamenti.Rows(i).Item("stato_ribaltamento")
                        End If

                        DR.Item("provenienza_fascicolo") = DT_Appezzamenti.Rows(i).Item("provenienza_fascicolo")

                        DR.Item("Codice_Fiscale_Tecnico") = DT_Appezzamenti.Rows(i).Item("Codice_Fiscale_Tecnico")

                        DR.Item("datoGis") = DT_Appezzamenti.Rows(i).Item("datoGis")


                        DR.Item("chiave") = chiave

                        DR.Item("IAF") = DT_Appezzamenti.Rows(i).Item("IAF")

                        DR.Item("Regolamento_Concimazione_Cod") = 0
                        If Not IsDBNull(DT_Appezzamenti.Rows(i).Item("Regolamento_Concimazione_Cod")) Then
                            DR.Item("Regolamento_Concimazione_Cod") = DT_Appezzamenti.Rows(i).Item("Regolamento_Concimazione_Cod")
                        End If

                        DR.Item("Flag_PubblicoPrivato") = 0
                        If Not IsDBNull(DT_Appezzamenti.Rows(i).Item("Flag_PubblicoPrivato")) Then
                            DR.Item("Flag_PubblicoPrivato") = DT_Appezzamenti.Rows(i).Item("Flag_PubblicoPrivato")
                        End If

                        DR.Item("Id_tr") = 0
                        If Not IsDBNull(DT_Appezzamenti.Rows(i).Item("Id_tr")) Then
                            DR.Item("Id_tr") = DT_Appezzamenti.Rows(i).Item("Id_tr")
                        End If


                        If DT_Appezzamenti.Rows(i).Item("Regolamento_Cod") = 4 Then
                            DR.Item("disciplinare") = "-2"
                        Else
                            If DT_Appezzamenti.Rows(i).Item("Disciplinare_Cod") <> 0 Then
                                DR.Item("disciplinare") = DT_Appezzamenti.Rows(i).Item("Disciplinare_Cod") & "/" &
                                                  DT_Appezzamenti.Rows(i).Item("Flag_PubblicoPrivato") & "/" &
                                                  DT_Appezzamenti.Rows(i).Item("Regolamento_Concimazione_Cod") & "/" &
                                                  DT_Appezzamenti.Rows(i).Item("Id_tr")
                            Else
                                DR.Item("disciplinare") = "0"
                            End If

                        End If


                        DR.Item("DistBZ_CorpiIdrici") = DT_Appezzamenti.Rows(i).Item("DistBZ_CorpiIdrici")
                        DR.Item("DistBZ_AreeResPub") = DT_Appezzamenti.Rows(i).Item("DistBZ_AreeResPub")
                        DR.Item("DistBZ_Allevamenti") = DT_Appezzamenti.Rows(i).Item("DistBZ_Allevamenti")
                        DR.Item("DistBZ_VegNatNonColt") = DT_Appezzamenti.Rows(i).Item("DistBZ_VegNatNonColt")
                        DR.Item("SupBZ_Riduzione") = DT_Appezzamenti.Rows(i).Item("SupBZ_Riduzione")

                        If (DT_Appezzamenti.Rows(i).Item("TipoZona") = "n") Then
                            DR.Item("TipoZona") = "n"
                            DR.Item("TipoZona_Des") = "Non Vulnerabile"
                        ElseIf (DT_Appezzamenti.Rows(i).Item("TipoZona") = "v") Then
                            DR.Item("TipoZona") = "v"
                            DR.Item("TipoZona_Des") = "Vulnerabile"
                        Else
                            DR.Item("TipoZona") = "n"
                            DR.Item("TipoZona_Des") = "Non Vulnerabile"
                        End If

                        DR.Item("MetodoProduzione_Cod") = DT_Appezzamenti.Rows(i).Item("MetodoProduzione_Cod")
                        DR.Item("MetodoProduzione_Des") = DT_Appezzamenti.Rows(i).Item("MetodoProduzione_Des")

                        DR.Item("Campo_Cod") = DT_Appezzamenti.Rows(i).Item("Campo_Cod")
                        DR.Item("Campo_Des") = DT_Appezzamenti.Rows(i).Item("Campo_Des")

                        DR.Item("Riferimento_Alfanumerico_Appezzamento") = DT_Appezzamenti.Rows(i).Item("Riferimento_Alfanumerico_Appezzamento")
                        DR.Item("Isola") = DT_Appezzamenti.Rows(i).Item("Isola")

                        DR.Item("CapitolatoPrivato") = DT_Appezzamenti.Rows(i).Item("CapitolatoPrivato")
                        DR.Item("CapitolatoPrivato_Des") = DT_Appezzamenti.Rows(i).Item("CapitolatoPrivato_Des")

                        DR.Item("Finalita_Concimazione_Impianto") = DT_Appezzamenti.Rows(i).Item("Finalita_Concimazione_Impianto")

                        'INDIRIZZO
                        DR.Item("Cod_Indirizzo") = DT_Appezzamenti.Rows(i).Item("Cod_Indirizzo")
                        DR.Item("ind_des") = DT_Appezzamenti.Rows(i).Item("ind_des")
                        DR.Item("frz_des") = DT_Appezzamenti.Rows(i).Item("frz_des")
                        DR.Item("CAP") = DT_Appezzamenti.Rows(i).Item("CAP")
                        DR.Item("com_des_indirizzo") = DT_Appezzamenti.Rows(i).Item("com_des_indirizzo")
                        DR.Item("pro_cod_indirizzo") = DT_Appezzamenti.Rows(i).Item("pro_cod_indirizzo")
                        DR.Item("stato_indirizzo") = DT_Appezzamenti.Rows(i).Item("stato_indirizzo")
                        DR.Item("stato_indirizzo_des") = DT_Appezzamenti.Rows(i).Item("stato_indirizzo_des")
                        DR.Item("note_indirizzo") = DT_Appezzamenti.Rows(i).Item("note_indirizzo")
                        DR.Item("pro_cod_istat_indirizzo") = DT_Appezzamenti.Rows(i).Item("pro_cod_istat_indirizzo")
                        DR.Item("com_cod_istat_indirizzo") = DT_Appezzamenti.Rows(i).Item("com_cod_istat_indirizzo")

                        Dim Pratiche_Cod As String = " "
                        Dim Pratiche_Des As String = " "

                        ImpostaPraticheProgrammazione(Pratiche_Cod, Pratiche_Des, DT_Appezzamenti.Rows(i).Item("Programmazione_Entita_Cod"), objParametri_Server)

                        DR.Item("Pratiche_Cod") = Pratiche_Cod
                        DR.Item("Pratiche_Des") = Pratiche_Des

                        DR.Item("KPIN") = DT_Appezzamenti.Rows(i).Item("KPIN")
                        DR.Item("Block_Name") = DT_Appezzamenti.Rows(i).Item("Block_Name")

                        DR.Item("Foral_Cod") = DT_Appezzamenti.Rows(i).Item("Foral_Cod")
                        DR.Item("Foral_Des") = DT_Appezzamenti.Rows(i).Item("Foral_Des")

                        DR.Item("Data_Inizio_Portinnesto") = DT_Appezzamenti.Rows(i).Item("Data_Inizio_Portinnesto")

                        DR.Item("Data_Creazione") = DT_Appezzamenti.Rows(i).Item("Data_Creazione")
                        DR.Item("Data_Modifica") = DT_Appezzamenti.Rows(i).Item("Data_Modifica")

                        DR.Item("ZespriFase_Cod") = DT_Appezzamenti.Rows(i).Item("ZespriFase_Cod")
                        DR.Item("ZespriFase_Des") = DT_Appezzamenti.Rows(i).Item("ZespriFase_Des")
                        DR.Item("ZespriTipo_Cod") = DT_Appezzamenti.Rows(i).Item("ZespriTipo_Cod")
                        DR.Item("ZespriTipo_Des") = DT_Appezzamenti.Rows(i).Item("ZespriTipo_Des")
                        DR.Item("ZespriGrower_Cod") = DT_Appezzamenti.Rows(i).Item("ZespriGrower_Cod")
                        DR.Item("ZespriGrower_Des") = DT_Appezzamenti.Rows(i).Item("ZespriGrower_Des")

                        DR.Item("Num_Piante_Femmine") = DT_Appezzamenti.Rows(i).Item("Num_Piante_Femmine")
                        DR.Item("Num_Piante_Maschi") = DT_Appezzamenti.Rows(i).Item("Num_Piante_Maschi")

                        DR.Item("Port_Cod") = DT_Appezzamenti.Rows(i).Item("Port_Cod")
                        DR.Item("Port_Des") = DT_Appezzamenti.Rows(i).Item("Port_Des")

                        DR.Item("TipologiaInnestoTrapianto_Cod") = DT_Appezzamenti.Rows(i).Item("TipologiaInnestoTrapianto_Cod")
                        DR.Item("TipologiaInnestoTrapianto_Des") = DT_Appezzamenti.Rows(i).Item("TipologiaInnestoTrapianto_Des")

                        chiave += 1

                        DtParticelle.Rows.Add(DR)

                    Next

                    r.RispostaStringa = JSON_DataTableAppezzamenti_Tabella1(DtParticelle, Validita_Inizio.ToShortDateString, Validita_Fine.ToShortDateString)

                End If

            Else

                Dim objParametri_Super_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Super_Server")
                If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Super_Server) Then
                    r.Sessione = False
                    Return r
                End If

                Dim dt = Trova_Tutti_Dettagli_Post2025(piva, richiesta_Cod, programmazione_cod, objParametri_Server, objParametri_Super_Server)

                r.RispostaStringa = JSON_DataTableAppezzamenti_TabellaPost2025(dt, Validita_Inizio.ToShortDateString, Validita_Fine.ToShortDateString)

            End If

            'r.RispostaStringa = JsonConvert.SerializeObject(DtParticelle, Formatting.None, serializerSettings)
            'strAppezzamenti = JSON_DataTableAppezzamenti_Tabella1(DtParticelle, Validita_Inizio.ToShortDateString, Validita_Fine.ToShortDateString)
            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False
            Return r

        End Try

        'Return strAppezzamenti
        Return r

    End Function

    Private Shared Function Trova_Tutti_Dettagli_Post2025(piva As String,
                                                          richiesta_Cod As Integer,
                                                          programmazione_cod As Integer,
                                                          objParametri_Server As AgronicaCoreParametri,
                                                          objParametri_Super_Server As AgronicaCoreParametri) As DataTable

        Dim dt As New DataTable
        Dim dTupla = Trova_Richieste_Dettaglio_Post2025(piva, richiesta_Cod, 0, programmazione_cod, objParametri_Server, objParametri_Super_Server)
        If dTupla.Item2 = "" Then
            dt = dTupla.Item1
        End If

        Return dt

    End Function

    Private Shared Sub DtParticelleCreaStruttura(ByRef DtParticelle As DataTable)

        DtParticelle.Columns.Add(New DataColumn("chiave", GetType(Integer)))

        DtParticelle.Columns.Add(New DataColumn("piva", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("sa_cod", GetType(Integer)))
        DtParticelle.Columns.Add(New DataColumn("appezza", GetType(Integer)))

        DtParticelle.Columns.Add(New DataColumn("programmazione_entita_cod", GetType(Integer)))

        DtParticelle.Columns.Add(New DataColumn("Sa_Nome", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("App_Nome", GetType(String)))

        DtParticelle.Columns.Add(New DataColumn("PROV", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("COM", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("Prov_Des", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("Com_Des", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("Sezione", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("Foglio", GetType(Integer)))
        DtParticelle.Columns.Add(New DataColumn("Numero", GetType(Integer)))
        DtParticelle.Columns.Add(New DataColumn("Subalterno", GetType(String)))

        DtParticelle.Columns.Add(New DataColumn("possesso", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("TitoloPossesso", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("datepossesso", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("inizio_possesso", GetType(Date)))
        DtParticelle.Columns.Add(New DataColumn("fine_possesso", GetType(Date)))
        DtParticelle.Columns.Add(New DataColumn("sup", GetType(Double)))
        DtParticelle.Columns.Add(New DataColumn("supcondotta", GetType(Double)))

        DtParticelle.Columns.Add(New DataColumn("catasto", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("catasto_key", GetType(String)))

        DtParticelle.Columns.Add(New DataColumn("macrouso", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("macrouso_cod", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("macrouso_sup", GetType(Double)))

        DtParticelle.Columns.Add(New DataColumn("utilizzo", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("varieta", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("utilizzo_sup", GetType(Double)))
        'DtParticelle.Columns.Add(New DataColumn("Veg_Cod_Agea", GetType(String)))
        'DtParticelle.Columns.Add(New DataColumn("Cul_Cod_Agea", GetType(String)))

        DtParticelle.Columns.Add(New DataColumn("Veg_Cod", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("Cul_Cod", GetType(Integer)))
        DtParticelle.Columns.Add(New DataColumn("Grfi_Cod", GetType(Integer)))
        DtParticelle.Columns.Add(New DataColumn("Grva_Cod", GetType(Integer)))
        DtParticelle.Columns.Add(New DataColumn("Id_Cod", GetType(Integer)))
        DtParticelle.Columns.Add(New DataColumn("Scarto", GetType(Integer)))

        DtParticelle.Columns.Add(New DataColumn("veg_des", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("cul_des", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("grfi_des", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("grva_des", GetType(String)))

        DtParticelle.Columns.Add(New DataColumn("validita_inizio", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("validita_fine", GetType(String)))

        DtParticelle.Columns.Add(New DataColumn("ribaltato", GetType(Integer)))
        DtParticelle.Columns.Add(New DataColumn("movimentato", GetType(Integer)))

        'DtParticelle.Columns.Add(New DataColumn("Uso_Cod_Agea", GetType(String)))
        'DtParticelle.Columns.Add(New DataColumn("Occupazione_Cod_Agea", GetType(String)))
        'DtParticelle.Columns.Add(New DataColumn("Destinazione_Cod_Agea", GetType(String)))
        'DtParticelle.Columns.Add(New DataColumn("Qualita_Cod_Agea", GetType(String)))

        'DtParticelle.Columns.Add(New DataColumn("Grva_Cod", GetType(Integer)))
        DtParticelle.Columns.Add(New DataColumn("Cop_Cod", GetType(Integer)))
        DtParticelle.Columns.Add(New DataColumn("Cop_Des", GetType(String)))

        DtParticelle.Columns.Add(New DataColumn("Lotto", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("Resa", GetType(Double)))
        DtParticelle.Columns.Add(New DataColumn("Num_Piante", GetType(Integer)))
        DtParticelle.Columns.Add(New DataColumn("TRA_Fila", GetType(Double)))
        DtParticelle.Columns.Add(New DataColumn("SU_Fila", GetType(Double)))
        DtParticelle.Columns.Add(New DataColumn("Validita_Inizio_Impianto", GetType(Date)))
        DtParticelle.Columns.Add(New DataColumn("TipoZona", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("TipoZona_Des", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("MetodoProduzione_Cod", GetType(Integer)))
        DtParticelle.Columns.Add(New DataColumn("MetodoProduzione_Des", GetType(String)))
        'DtParticelle.Columns.Add(New DataColumn("Macrouso_Cod", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("Unita_Vitata", GetType(Integer)))

        DtParticelle.Columns.Add(New DataColumn("Veg_Cod_Agea", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("Veg_Des_Agea", GetType(String)))

        DtParticelle.Columns.Add(New DataColumn("Cul_Cod_Agea", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("Cul_Des_Agea", GetType(String)))

        DtParticelle.Columns.Add(New DataColumn("Uso_Cod_Agea", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("Uso_Des_Agea", GetType(String)))

        DtParticelle.Columns.Add(New DataColumn("Occupazione_Cod_Agea", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("Occupazione_Des_Agea", GetType(String)))

        DtParticelle.Columns.Add(New DataColumn("Destinazione_Cod_Agea", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("Destinazione_Des_Agea", GetType(String)))

        DtParticelle.Columns.Add(New DataColumn("Qualita_Cod_Agea", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("Qualita_Des_Agea", GetType(String)))

        DtParticelle.Columns.Add(New DataColumn("Gru_Cod", GetType(Integer)))

        DtParticelle.Columns.Add(New DataColumn("Dpi_Cod", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("Reg_Cod", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("StatoImpianto_Cod", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("N", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("P", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("K", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("Data_semina", GetType(Date)))
        DtParticelle.Columns.Add(New DataColumn("Data_Raccolta", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("Data_Fioritura", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("Coltura_Precedente", GetType(Integer)))
        DtParticelle.Columns.Add(New DataColumn("Coltura_Precedente2", GetType(Integer)))
        DtParticelle.Columns.Add(New DataColumn("Coltura_Precedente3", GetType(Integer)))
        DtParticelle.Columns.Add(New DataColumn("Coltura_Precedente4", GetType(Integer)))
        DtParticelle.Columns.Add(New DataColumn("Piano_Semina", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("Codice_Contratto", GetType(String)))

        DtParticelle.Columns.Add(New DataColumn("unito", GetType(Integer)))
        DtParticelle.Columns.Add(New DataColumn("frazionato", GetType(Integer)))
        DtParticelle.Columns.Add(New DataColumn("Codice_Fiscale_Tecnico", GetType(String)))

        DtParticelle.Columns.Add(New DataColumn("provenienza_fascicolo", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("stato_ribaltamento", GetType(Integer)))

        DtParticelle.Columns.Add(New DataColumn("datoGis", GetType(String)))

        DtParticelle.Columns.Add(New DataColumn("IAF", GetType(String)))

        DtParticelle.Columns.Add(New DataColumn("Disciplinare", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("Regolamento_Concimazione_Cod", GetType(Integer)))
        DtParticelle.Columns.Add(New DataColumn("Flag_PubblicoPrivato", GetType(Integer)))
        DtParticelle.Columns.Add(New DataColumn("Id_tr", GetType(Integer)))

        DtParticelle.Columns.Add(New DataColumn("DistBZ_CorpiIdrici", GetType(Double)))
        DtParticelle.Columns.Add(New DataColumn("DistBZ_AreeResPub", GetType(Double)))
        DtParticelle.Columns.Add(New DataColumn("DistBZ_Allevamenti", GetType(Double)))
        DtParticelle.Columns.Add(New DataColumn("DistBZ_VegNatNonColt", GetType(Double)))
        DtParticelle.Columns.Add(New DataColumn("SupBZ_Riduzione", GetType(Double)))

        DtParticelle.Columns.Add(New DataColumn("ZoneCatasto", GetType(String)))

        DtParticelle.Columns.Add(New DataColumn("campo_cod", GetType(Integer)))
        DtParticelle.Columns.Add(New DataColumn("campo_des", GetType(String)))

        ' Vanni 08/04/2019: aggiunta
        DtParticelle.Columns.Add(New DataColumn("wkt", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("wkt_georiferimento_cod", GetType(String)))

        DtParticelle.Columns.Add(New DataColumn("Riferimento_Alfanumerico_Appezzamento", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("Isola", GetType(String)))

        DtParticelle.Columns.Add(New DataColumn("CapitolatoPrivato", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("CapitolatoPrivato_Des", GetType(String)))

        DtParticelle.Columns.Add(New DataColumn("Finalita_Concimazione_Impianto", GetType(String)))

        'INDIRIZZO
        DtParticelle.Columns.Add(New DataColumn("Cod_Indirizzo", GetType(Integer)))
        DtParticelle.Columns.Add(New DataColumn("ind_des", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("frz_des", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("CAP", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("com_des_indirizzo", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("pro_cod_indirizzo", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("stato_indirizzo", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("stato_indirizzo_des", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("note_indirizzo", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("pro_cod_istat_indirizzo", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("com_cod_istat_indirizzo", GetType(String)))

        DtParticelle.Columns.Add(New DataColumn("Pratiche_Cod", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("Pratiche_Des", GetType(String)))

        DtParticelle.Columns.Add(New DataColumn("KPIN", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("Block_Name", GetType(String)))

        DtParticelle.Columns.Add(New DataColumn("foral_cod", GetType(Integer)))
        DtParticelle.Columns.Add(New DataColumn("foral_des", GetType(String)))

        DtParticelle.Columns.Add(New DataColumn("Data_Inizio_Portinnesto", GetType(String)))

        DtParticelle.Columns.Add(New DataColumn("Data_Creazione", GetType(Date)))
        DtParticelle.Columns.Add(New DataColumn("Data_Modifica", GetType(Date)))

        'ZESPRI
        DtParticelle.Columns.Add(New DataColumn("ZespriFase_Cod", GetType(Integer)))
        DtParticelle.Columns.Add(New DataColumn("ZespriFase_Des", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("ZespriTipo_Cod", GetType(Integer)))
        DtParticelle.Columns.Add(New DataColumn("ZespriTipo_Des", GetType(String)))
        DtParticelle.Columns.Add(New DataColumn("ZespriGrower_Cod", GetType(Integer)))
        DtParticelle.Columns.Add(New DataColumn("ZespriGrower_Des", GetType(String)))

        'NUM PIANTE MF
        DtParticelle.Columns.Add(New DataColumn("Num_Piante_Femmine", GetType(Integer)))
        DtParticelle.Columns.Add(New DataColumn("Num_Piante_Maschi", GetType(Integer)))

        'PORTINNESTO
        DtParticelle.Columns.Add(New DataColumn("Port_Cod", GetType(Integer)))
        DtParticelle.Columns.Add(New DataColumn("Port_Des", GetType(String)))


        'TIPOLOGIA DI INNESTO O TRAPIANTO
        DtParticelle.Columns.Add(New DataColumn("TipologiaInnestoTrapianto_Cod", GetType(Integer)))
        DtParticelle.Columns.Add(New DataColumn("TipologiaInnestoTrapianto_Des", GetType(String)))

    End Sub

    Private Shared Sub ImpostaPraticheProgrammazione(ByRef Pratiche_Cod As String, ByRef Pratiche_Des As String, ByRef Programmazione_Entita_Cod As Integer, objParametri_Server As AgronicaCoreParametri)

        Dim objPratiche As New AgronicaCoreProfilazioneDAL.Pratiche_R

        Dim DT_Pratiche = objPratiche.Leggi_conStatoAttuale(0, "", "", "", 0, 0, 0, 0, 0, AGRODATAINIZIO, AGRODATAFINE, "", "", objParametri_Server, 0, Programmazione_Entita_Cod)


        If DT_Pratiche IsNot Nothing AndAlso DT_Pratiche.Rows.Count > 0 Then
            Pratiche_Cod = ""
            Pratiche_Des = ""

            For Each rowPratiche In DT_Pratiche.Rows

                Pratiche_Cod &= rowPratiche("Pratica_Cod") & ", "
                Pratiche_Des &= "" & rowPratiche("Servizio_Des") & " - " & rowPratiche("Stato_Des") & ", "

            Next

            Pratiche_Cod = Pratiche_Cod.Substring(0, Pratiche_Cod.Length - 2)
            Pratiche_Des = Pratiche_Des.Substring(0, Pratiche_Des.Length - 2)

            Pratiche_Cod &= ""
            Pratiche_Des &= ""
        End If

    End Sub


    Private Shared Function ImpostaCatasto(Istat_Prov As String,
                                      Prov As String,
                                      Istat_Com As String,
                                      Com As String,
                                      Sezione As String,
                                      Foglio As Integer,
                                      Numero As Integer,
                                      Subalterno As String,
                                      Sup_Condotta As Double,
                                      possesso As String,
                                      datepossesso As String,
                                      inizio_possesso As Date,
                                      fine_possesso As Date,
                                      sup As Double,
                                      utilizzo_sup As Double) As String
        Dim Catasto As New CatastoAppezzamento
        If Istat_Prov IsNot Nothing Then
            Catasto.Istat_Prov = Istat_Prov
            Catasto.Prov = Prov
            Catasto.Istat_Com = Istat_Com
            Catasto.Com = Com
            Catasto.Sezione = Sezione
            Catasto.Foglio = Foglio
            Catasto.Numero = Numero
            Catasto.Subalterno = Subalterno
            Catasto.Sup_Condotta = Sup_Condotta
            Catasto.Possesso = possesso
            Catasto.Datepossesso = datepossesso
            Catasto.Inizio_possesso = inizio_possesso
            Catasto.Fine_possesso = fine_possesso
            Catasto.Sup = sup
            Catasto.Utilizzo_sup = utilizzo_sup
            Catasto.Key = Istat_Prov & "_" & Istat_Com & "_" & Sezione & "_" & CStr(Foglio) & "_" & CStr(Numero) & "_" & Subalterno
            Dim ser = JsonSerializer.Create
            Dim sb As New StringBuilder
            Dim sw As New StringWriter(sb)
            ser.Serialize(sw, Catasto)
            Return sb.ToString
        Else
            Return V
        End If



    End Function

    Private Shared Function JSON_DataTableAppezzamenti_Tabella1(ByRef DT_Appezzamenti As DataTable,
                                                               ByVal DataInizio As String, ByVal DataFine As String,
                                                               Optional ByVal stringaKendoRow As String = "") As String

        'creo la lista delle colonne da visualizzare
        Dim l As New List(Of ColonneNome)
        Dim c As ColonneNome

        c = New ColonneNome("chiave", "chiave", "number") With {
            ._hidden = True
        }
        l.Add(c)

        Dim colonneNome As ColonneNome = New ColonneNome("piva", "piva", "string") With {
            ._hidden = True
        }
        c = colonneNome
        l.Add(c)
        c = New ColonneNome("sa_cod", "sa_cod", "number") With {
            ._hidden = True,
            ._Editabile = True
        }
        l.Add(c)

        c = New ColonneNome("appezza", "appezza", "number") With {
            ._hidden = True
        }
        l.Add(c)

        c = New ColonneNome("programmazione_entita_cod", "Programmazione_Entita_Cod", "number") With {
            ._hidden = True
        }
        l.Add(c)

        'c._RemoveHtmlEncode = True
        'c = New ColonneNome("datoGis", "Gis", "String") With {
        '    ._FormatoParticolare = "<span class='fa fa-globe fa-2x'></span>",
        '    ._Filtrabile = False,
        '    ._width = "60px"
        '}
        'l.Add(c)

        'c._Filtrabile = False
        'c = New ColonneNome("app_nome", "App.", "string") With {
        c = New ColonneNome("chiave", "App.", "string") With {
            ._Editabile = True,
            ._obbligatorio = True,
            ._width = "78px"
        }
        l.Add(c)

        c = New ColonneNome("utilizzo_sup", "Sup. Utilizzo [ha] (SAU)", "number") With {
            ._Editabile = True,
            ._obbligatorio = True,
            ._Filtrabile = False,
            ._width = "120px",
            ._formatNr = "n4"
        }
        l.Add(c)

        c = New ColonneNome("macrouso_cod", "Macrouso_Cod", "string") With {
            ._Editabile = True,
            ._hidden = True
        }
        l.Add(c)

        c = New ColonneNome("macrouso_sup", "Macrouso_Sup", "string") With {
            ._Editabile = True,
            ._hidden = True
        }
        l.Add(c)

        c = New ColonneNome("utilizzo", "Utilizzo (Specie Vegetale)", "string") With {
            ._Editabile = True,
            ._hidden = True
        }
        l.Add(c)

        c = New ColonneNome("varieta", "Varietà", "string") With {
            ._Editabile = True,
            ._hidden = True
        }
        l.Add(c)

        'c._hidden = True
        c = New ColonneNome("veg_des", "Specie Vegetale", "string") With {
            ._Editabile = True,
            ._width = "200px"
        }
        l.Add(c)

        'c._hidden = True
        c = New ColonneNome("cul_des", "Varietà", "string") With {
            ._Editabile = True,
            ._width = "200px"
        }
        l.Add(c)

        'c._hidden = True
        c = New ColonneNome("grfi_des", "Finalità Produttiva", "string") With {
            ._Editabile = True,
            ._width = "200px"
        }
        l.Add(c)

        'c._hidden = True
        'c = New ColonneNome("macrouso", "Macrouso", "string") With {
        '    ._Editabile = True,
        '    ._width = "200px"
        '}
        'l.Add(c)

        ''c._hidden = True
        'c = New ColonneNome("grva_des", "Tipologia Varietale", "string") With {
        '    ._width = "101px"
        '}
        'l.Add(c)

        '''GESTIONE CATASTO IN LINEA
        ''c._Editabile = True
        ''c._hidden = True
        'c = New ColonneNome("PROV", "PROV", "String") With {
        '    ._width = "80px"
        '}
        'l.Add(c)

        'c._Editabile = True
        'c._hidden = True
        c = New ColonneNome("prov_des", "PR.", "String") With {
            ._width = "80px"
        }
        l.Add(c)

        'c._Editabile = True
        'c._hidden = True
        'c = New ColonneNome("COM", "COM", "String") With {
        '    ._width = "80px"
        '}
        'l.Add(c)

        'c._Editabile = True
        'c._hidden = True
        c = New ColonneNome("com_des", "Comune", "String") With {
            ._width = "150px"
        }
        l.Add(c)

        'c._Editabile = True
        'c._hidden = True
        c = New ColonneNome("sezione", "Sez.", "String") With {
            ._width = "80px"
        }
        l.Add(c)

        'c._Editabile = True
        'c._hidden = True
        c = New ColonneNome("foglio", "Fgl.", "String") With {
            ._width = "80px"
        }
        l.Add(c)

        'c._Editabile = True
        'c._hidden = True
        c = New ColonneNome("numero", "Numero", "String") With {
            ._width = "100px"
        }
        l.Add(c)

        'c._Editabile = True
        'c._hidden = True
        c = New ColonneNome("subalterno", "Sub.", "String") With {
            ._width = "80px"
        }
        l.Add(c)

        c = New ColonneNome("sa_nome", "Centro", "string") With {
            ._Editabile = True,
            ._Filtrabile = False,
            ._width = "100px"
        }
        c._Filtrabile = True
        l.Add(c)

        c = New ColonneNome("validita_inizio", "Inizio Gestione Appezzamento", "date") With {
            ._Editabile = True,
            ._obbligatorio = True,
            ._width = "133px",
            ._valueDefault = DataInizio,
            ._Filtrabile = True
        }
        l.Add(c)

        c = New ColonneNome("validita_fine", "Fine Gestione Appezzamento", "date") With {
            ._Editabile = True,
            ._obbligatorio = True,
            ._width = "132px",
            ._valueDefault = DataFine,
            ._Filtrabile = True
        }
        l.Add(c)

        c = New ColonneNome("prov", "PROV", "string") With {
            ._hidden = True
        }
        l.Add(c)
        c = New ColonneNome("com", "COM", "string") With {
            ._hidden = True
        }
        l.Add(c)

        c = New ColonneNome("catasto", "Catasto (Provincia-Comune-Sezione-Foglio-Numero-Subalterno)", "string") With {
            ._FormatoParticolare = "#= GetValoreParticella(chiave, catasto)#",
            ._RemoveHtmlEncode = True,
            ._hidden = True
        }
        l.Add(c)

        c = New ColonneNome("catasto_key", "Catasto_Key", "String") With {
            ._RemoveHtmlEncode = True,
            ._hidden = True
        }
        l.Add(c)

        c = New ColonneNome("id_cod", "Id_Cod", "String") With {
            ._hidden = True
        }
        l.Add(c)
        c = New ColonneNome("veg_cod", "Veg_Cod", "String") With {
            ._hidden = True,
            ._Editabile = True
        }
        l.Add(c)
        c = New ColonneNome("cul_cod", "Cul_Cod", "String") With {
            ._hidden = True,
            ._Editabile = True
        }
        l.Add(c)

        c = New ColonneNome("grfi_cod", "Grfi_Cod", "String") With {
            ._hidden = True,
            ._Editabile = True
        }
        l.Add(c)
        c = New ColonneNome("grva_cod", "Grva_Cod", "String") With {
            ._hidden = True
        }
        l.Add(c)

        c = New ColonneNome("veg_cod_agea", "Veg_Cod_Agea", "String") With {
            ._hidden = True,
            ._Editabile = True
        }
        l.Add(c)
        c = New ColonneNome("cul_cod_agea", "Cul_Cod_Agea", "String") With {
            ._hidden = True,
            ._Editabile = True
        }
        l.Add(c)

        c = New ColonneNome("ribaltato", "ribaltato", "String") With {
            ._hidden = True
        }
        l.Add(c)

        c = New ColonneNome("movimentato", "movimentato", "String") With {
            ._hidden = True
        }
        l.Add(c)


        c = New ColonneNome("Cop_Cod", "Cop_Cod", "String") With {
            ._Editabile = True,
            ._hidden = True
        }
        l.Add(c)
        'c._hidden = True
        c = New ColonneNome("Cop_Des", "Copertura", "String") With {
            ._Editabile = True,
            ._width = "130px"
        }
        l.Add(c)
        'c._hidden = True
        c = New ColonneNome("Lotto", "Lotto", "String") With {
            ._Editabile = True,
            ._width = "110px"
        }
        l.Add(c)
        'c._hidden = True
        c = New ColonneNome("Resa", "Resa (t/ha)", "String") With {
            ._Editabile = True,
            ._width = "77px"
        }
        l.Add(c)
        'c._hidden = True
        c = New ColonneNome("Num_Piante", "Num. Piante", "String") With {
            ._Editabile = True,
            ._width = "80px"
        }
        l.Add(c)
        'c._hidden = True
        c = New ColonneNome("TRA_Fila", "TRA Fila (m)", "number") With {
            ._Editabile = True,
            ._width = "66px"
        }
        l.Add(c)
        'c._hidden = True
        c = New ColonneNome("SU_Fila", "SU Fila (m)", "number") With {
            ._Editabile = True,
            ._width = "66px"
        }
        l.Add(c)
        'c._hidden = True
        c = New ColonneNome("Validita_Inizio_Impianto", "Validita Inizio Impianto", "date") With {
            ._Editabile = True,
            ._valueDefault = DataInizio,
            ._width = "100px"
        }
        l.Add(c)
        c = New ColonneNome("TipoZona", "TipoZona", "String") With {
            ._Editabile = True,
            ._hidden = True
        }
        l.Add(c)
        'c._hidden = True
        c = New ColonneNome("TipoZona_Des", "Tipo Zona", "String") With {
            ._Editabile = True,
            ._width = "86px"
        }
        l.Add(c)
        c = New ColonneNome("MetodoProduzione_Cod", "MetodoProduzione_Cod", "String") With {
            ._Editabile = True,
            ._hidden = True
        }
        l.Add(c)
        'c._hidden = True
        c = New ColonneNome("MetodoProduzione_Des", "Metodo Produzione", "String") With {
            ._Editabile = True,
            ._width = "109px"
        }
        l.Add(c)
        'c._hidden = True
        c = New ColonneNome("Unita_Vitata", "Unità Vitata", "String") With {
            ._Editabile = True,
            ._width = "78px"
        }
        l.Add(c)
        c = New ColonneNome("Veg_Cod_Agea", "Veg_Cod_Agea", "String") With {
            ._Editabile = True,
            ._hidden = True
        }
        l.Add(c)
        c = New ColonneNome("Veg_Des_Agea", "Veg_Des_Agea", "String") With {
            ._Editabile = True,
            ._hidden = True
        }
        l.Add(c)
        c = New ColonneNome("Cul_Cod_Agea", "Cul_Cod_Agea", "String") With {
            ._Editabile = True,
            ._hidden = True
        }
        l.Add(c)
        c = New ColonneNome("Cul_Des_Agea", "Cul_Des_Agea", "String") With {
            ._Editabile = True,
            ._hidden = True
        }
        l.Add(c)
        c = New ColonneNome("Uso_Cod_Agea", "Uso_Cod_Agea", "String") With {
            ._Editabile = True,
            ._hidden = True
        }
        l.Add(c)
        c = New ColonneNome("Uso_Des_Agea", "Uso_Des_Agea", "String") With {
            ._Editabile = True,
            ._hidden = True
        }
        l.Add(c)
        c = New ColonneNome("Occupazione_Cod_Agea", "Occupazione_Cod_Agea", "String") With {
            ._Editabile = True,
            ._hidden = True
        }
        l.Add(c)
        c = New ColonneNome("Occupazione_Des_Agea", "Occupazione_Des_Agea", "String") With {
            ._Editabile = True,
            ._hidden = True
        }
        l.Add(c)
        c = New ColonneNome("Destinazione_Cod_Agea", "Destinazione_Cod_Agea", "String") With {
            ._Editabile = True,
            ._hidden = True
        }
        l.Add(c)
        c = New ColonneNome("Destinazione_Des_Agea", "Destinazione_Des_Agea", "String") With {
            ._Editabile = True,
            ._hidden = True
        }
        l.Add(c)
        c = New ColonneNome("Qualita_Cod_Agea", "Qualita_Cod_Agea", "String") With {
            ._Editabile = True,
            ._hidden = True
        }
        l.Add(c)
        c = New ColonneNome("Qualita_Des_Agea", "Qualita_Des_Agea", "String") With {
            ._Editabile = True,
            ._hidden = True
        }
        l.Add(c)

        c = New ColonneNome("Gru_Cod", "Gru_Cod", "String") With {
            ._Editabile = True,
            ._hidden = True
        }
        l.Add(c)

        c = New ColonneNome("Dpi_Cod", "Dpi_Cod", "String") With {
            ._Editabile = True,
            ._hidden = True
        }
        l.Add(c)

        c = New ColonneNome("Reg_Cod", "Reg_Cod", "String") With {
            ._Editabile = True,
            ._hidden = True
        }
        l.Add(c)

        c = New ColonneNome("Regolamento_Concimazione_Cod", "Regolamento_Concimazione_Cod", "String") With {
            ._Editabile = True,
            ._hidden = True
        }
        l.Add(c)

        c = New ColonneNome("Disciplinare", "Disciplinare", "String") With {
            ._Editabile = True,
            ._hidden = True
        }
        l.Add(c)

        c = New ColonneNome("Flag_PubblicoPrivato", "Flag_PubblicoPrivato", "String") With {
            ._Editabile = True,
            ._hidden = True
        }
        l.Add(c)

        c = New ColonneNome("id_tr", "id_tr", "String") With {
            ._Editabile = True,
            ._hidden = True
        }
        l.Add(c)

        c = New ColonneNome("StatoImpianto_Cod", "StatoImpianto_Cod", "String") With {
            ._Editabile = True,
            ._hidden = True
        }
        l.Add(c)

        'c._hidden = True
        c = New ColonneNome("N", "N (kg/ha)", "String") With {
            ._Editabile = True,
            ._width = "87px"
        }
        l.Add(c)

        'c._hidden = True
        c = New ColonneNome("P", "P (kg/ha)", "String") With {
            ._Editabile = True,
            ._width = "87px"
        }
        l.Add(c)

        'c._hidden = True
        c = New ColonneNome("K", "K (kg/ha)", "String") With {
            ._Editabile = True,
            ._width = "87px"
        }
        l.Add(c)

        'c._hidden = True
        c = New ColonneNome("Data_Semina", "Data Semina", "date") With {
            ._Editabile = True,
            ._width = "92px"
        }
        l.Add(c)

        'c._hidden = True
        c = New ColonneNome("Data_Raccolta", "Data Raccolta", "date") With {
            ._Editabile = True,
            ._valueDefault = DataInizio,
            ._width = "92px"
        }
        l.Add(c)

        'c._hidden = True
        c = New ColonneNome("Data_Fioritura", "Data Fioritura", "date") With {
            ._Editabile = True,
            ._valueDefault = DataInizio,
            ._width = "92px"
        }
        l.Add(c)

        c = New ColonneNome("Coltura_Precedente", "Coltura_Precedente", "String") With {
            ._Editabile = True,
            ._hidden = True
        }
        l.Add(c)

        c = New ColonneNome("Coltura_Precedente2", "Coltura_Precedente2", "String") With {
            ._Editabile = True,
            ._hidden = True
        }
        l.Add(c)

        c = New ColonneNome("Coltura_Precedente3", "Coltura_Precedente3", "String") With {
            ._Editabile = True,
            ._hidden = True
        }
        l.Add(c)

        c = New ColonneNome("Coltura_Precedente4", "Coltura_Precedente4", "String") With {
            ._Editabile = True,
            ._hidden = True
        }
        l.Add(c)

        c = New ColonneNome("Piano_Semina", "Piano_Semina", "String") With {
            ._Editabile = True,
            ._hidden = True
        }
        l.Add(c)

        c = New ColonneNome("Codice_Contratto", "Codice_Contratto", "String") With {
            ._Editabile = True,
            ._hidden = True
        }
        l.Add(c)

        'c._Editabile = True
        c = New ColonneNome("unito", "unito", "number") With {
            ._hidden = True
        }
        l.Add(c)

        'c._Editabile = True
        c = New ColonneNome("frazionato", "frazionato", "number") With {
            ._hidden = True
        }
        l.Add(c)

        'c._Editabile = True
        c = New ColonneNome("Codice_Fiscale_Tecnico", "Codice_Fiscale_Tecnico", "String") With {
            ._hidden = True
        }
        l.Add(c)

        'c._Editabile = True
        c = New ColonneNome("stato_ribaltamento", "stato_ribaltamento", "number") With {
            ._hidden = True
        }
        l.Add(c)

        'c._Editabile = True
        c = New ColonneNome("provenienza_fascicolo", "provenienza_fascicolo", "String") With {
            ._hidden = True
        }
        l.Add(c)

        c = New ColonneNome("IAF", "IAF", "String")
        'c._Editabile = True
        'c._hidden = True
        l.Add(c)

        'c._hidden = True
        c = New ColonneNome("DistBZ_CorpiIdrici", "Corpi idridi superficiali [m]", "number") With {
            ._Editabile = True,
            ._width = "107px"
        }
        l.Add(c)

        'c._hidden = True
        c = New ColonneNome("DistBZ_AreeResPub", "Aree residenziali/ pubbliche [m]", "number") With {
            ._Editabile = True,
            ._width = "115px"
        }
        l.Add(c)

        'c._hidden = True
        c = New ColonneNome("DistBZ_Allevamenti", "Allevamenti [m]", "number") With {
            ._Editabile = True,
            ._width = "111px"
        }
        l.Add(c)

        'c._hidden = True
        c = New ColonneNome("DistBZ_VegNatNonColt", "Vegetazione naturale/non coltivata [m]", "number") With {
            ._Editabile = True,
            ._width = "120px"
        }
        l.Add(c)

        'c._hidden = True
        c = New ColonneNome("SupBZ_Riduzione", "Offset da ultima pianta (Capezzagna) [m]", "number") With {
            ._Editabile = True,
            ._width = "120px"
        }
        l.Add(c)

        c = New ColonneNome("campo_cod", "campo_cod", "number") With {
            ._Editabile = True,
            ._hidden = True
        }
        l.Add(c)

        'c._hidden = True
        c = New ColonneNome("campo_des", "Campo", "string") With {
            ._Editabile = True,
            ._width = "200px"
        }
        l.Add(c)

        c = New ColonneNome("wkt", "wkt", "String") With {
            ._Editabile = True,
            ._hidden = True
        }
        l.Add(c)

        c = New ColonneNome("wkt_georiferimento_cod", "wkt_georiferimento_cod", "String") With {
            ._Editabile = True,
            ._hidden = True
        }
        l.Add(c)

        c = New ColonneNome("Riferimento_Alfanumerico_Appezzamento", "Riferimento_Alfanumerico_Appezzamento", "String") With {
            ._Editabile = True
        }
        l.Add(c)

        c = New ColonneNome("Isola", "Isola", "String") With {
            ._Editabile = True
        }
        l.Add(c)

        c = New ColonneNome("CapitolatoPrivato", "CapitolatoPrivato", "String") With {
            ._Editabile = True
        }
        l.Add(c)

        c = New ColonneNome("CapitolatoPrivato_Des", "CapitolatoPrivato_Des", "String") With {
            ._Editabile = True
        }
        l.Add(c)

        c = New ColonneNome("Finalita_Concimazione_Impianto", "Finalita_Concimazione_Impianto", "number") With {
            ._Editabile = True,
            ._hidden = True
        }
        l.Add(c)

        'INDIRIZZO
        c = New ColonneNome("Cod_Indirizzo", "Cod_Indirizzo", "number") With {
            ._Editabile = True,
            ._hidden = True
        }
        l.Add(c)

        c = New ColonneNome("ind_des", "ind_des", "string") With {
            ._Editabile = True,
            ._hidden = True
        }
        l.Add(c)

        c = New ColonneNome("frz_des", "frz_des", "string") With {
            ._Editabile = True,
            ._hidden = True
        }
        l.Add(c)

        c = New ColonneNome("CAP", "CAP", "string") With {
            ._Editabile = True,
            ._hidden = True
        }
        l.Add(c)

        c = New ColonneNome("com_des_indirizzo", "com_des", "string") With {
            ._Editabile = True,
            ._hidden = True
        }

        l.Add(c)
        c = New ColonneNome("pro_cod_indirizzo", "pro_cod", "string") With {
            ._Editabile = True,
            ._hidden = True
        }
        l.Add(c)

        c = New ColonneNome("stato_indirizzo", "stato", "string") With {
            ._Editabile = True,
            ._hidden = True
        }
        l.Add(c)

        c = New ColonneNome("stato_indirizzo_des", "stato_des", "string") With {
            ._Editabile = True,
            ._hidden = True
        }
        l.Add(c)

        c = New ColonneNome("note_indirizzo", "note", "string") With {
            ._Editabile = True,
            ._hidden = True
        }
        l.Add(c)

        c = New ColonneNome("pro_cod_istat_indirizzo", "pro_cod_istat", "string") With {
            ._Editabile = True,
            ._hidden = True
        }
        l.Add(c)

        c = New ColonneNome("com_cod_istat_indirizzo", "com_cod_istat", "string") With {
            ._Editabile = True,
            ._hidden = True
        }
        l.Add(c)


        c = New ColonneNome("Pratiche_Cod", "Pratiche_Cod", "string") With {
            ._Editabile = True,
            ._hidden = True
        }
        l.Add(c)

        c = New ColonneNome("Pratiche_Des", "Pratiche_Des", "string") With {
            ._Editabile = True
        }
        l.Add(c)

        c = New ColonneNome("KPIN", "KPIN", "string") With {
            ._Editabile = True
        }
        l.Add(c)

        c = New ColonneNome("Block_Name", "Block_Name", "string") With {
            ._Editabile = True
        }
        l.Add(c)

        c = New ColonneNome("Foral_Cod", "Foral_Cod", "number") With {
            ._Editabile = True
        }
        l.Add(c)

        c = New ColonneNome("Foral_Des", "Foral_Des", "string") With {
            ._Editabile = True
        }
        l.Add(c)

        c = New ColonneNome("Data_Inizio_Portinnesto", "Data_Inizio_Portinnesto", "date") With {
            ._Editabile = True
        }
        l.Add(c)

        c = New ColonneNome("Data_Creazione", "Data_Creazione", "date") With {
            ._Editabile = True
        }
        l.Add(c)

        c = New ColonneNome("Data_Modifica", "Data_Modifica", "date") With {
            ._Editabile = True
        }
        l.Add(c)

        c = New ColonneNome("ZespriFase_Cod", "ZespriFase_Cod", "string") With {
            ._Editabile = True
        }
        l.Add(c)

        c = New ColonneNome("ZespriFase_Des", "ZespriFase_Des", "string") With {
            ._Editabile = True
        }
        l.Add(c)

        c = New ColonneNome("ZespriTipo_Cod", "ZespriTipo_Cod", "string") With {
            ._Editabile = True
        }
        l.Add(c)

        c = New ColonneNome("ZespriTipo_Des", "ZespriTipo_Des", "string") With {
            ._Editabile = True
        }
        l.Add(c)

        c = New ColonneNome("ZespriGrower_Cod", "ZespriGrower_Cod", "string") With {
            ._Editabile = True
        }
        l.Add(c)

        c = New ColonneNome("ZespriGrower_Des", "ZespriGrower_Des", "string") With {
            ._Editabile = True
        }
        l.Add(c)

        c = New ColonneNome("Num_Piante_Femmine", "N. Piante Maschi", "number") With {
            ._Editabile = True
        }
        l.Add(c)

        c = New ColonneNome("Num_Piante_Maschi", "N. Piante Femmine", "number") With {
            ._Editabile = True
        }
        l.Add(c)

        c = New ColonneNome("Port_Cod", "Port_Cod", "number") With {
            ._Editabile = True
        }
        l.Add(c)

        c = New ColonneNome("Port_Des", "Port_Des", "string") With {
            ._Editabile = True
        }
        l.Add(c)

        c = New ColonneNome("TipologiaInnestoTrapianto_Cod", "TipologiaInnestoTrapianto_Cod", "number") With {
            ._Editabile = True
        }
        l.Add(c)

        c = New ColonneNome("TipologiaInnestoTrapianto_Des", "TipologiaInnestoTrapianto_Des", "string") With {
            ._Editabile = True
        }
        l.Add(c)

        'If Not String.IsNullOrEmpty(stringaKendoRow) Then

        '    Dim addedProp As Boolean = False
        '    Dim jo As JArray = JArray.Parse(stringaKendoRow)
        '    For Each rr As JObject In jo.Descendants().OfType(Of JObject)
        '        If Not rr("wkt") Is Nothing Then
        '            'esiste già...
        '            Exit For
        '        Else
        '            rr.Add(New JProperty("wkt", ""))
        '            rr.Add(New JProperty("wkt_georiferimento_cod", ""))
        '            addedProp = True
        '        End If
        '    Next

        '    If addedProp Then
        '        stringaKendoRow = jo.ToString()
        '    End If

        'End If

        'ritorno la tabella trasformata in json (aggiustando anche le colonne)
        Dim js As New AgronicaCoreDataProvider.JSON_DataTable With {
            .Editabile_Deafault = False
        }

        Dim risp As String

        'If stringaKendoRow = "" Then
        '    Dim strKendoRow As New StringBuilder
        '    AgronicaCoreDataProvider.JSON_DataTable.kendo_Rows(DT_Appezzamenti, l, strKendoRow)
        '    risp = strKendoRow.ToString
        'Else
        risp = js.JSON_DataTable_Kendo(DT_Appezzamenti, l, AssegnaAutomaticamenteTipoFiltro_daTipoDato:=True, tipoFiltroKendo:=TipoFiltroKendo_colonne.Menu,
                                           stringaKendoRow:=stringaKendoRow) 'True, True, TipoFiltroKendo_colonne.CasellaTesto) '
        'End If

        Return risp

    End Function

    Private Shared Function JSON_DataTableAppezzamenti_TabellaPost2025(ByRef DT_Appezzamenti As DataTable,
                                                               ByVal DataInizio As String, ByVal DataFine As String,
                                                               Optional ByVal stringaKendoRow As String = "") As String

        'creo la lista delle colonne da visualizzare
        Dim l As New List(Of ColonneNome)
        Dim c As ColonneNome

        c = New ColonneNome("cod", "chiave", "number") With {
            ._hidden = True
        }
        l.Add(c)


        c = New ColonneNome("Programmazione_Cod", "Programmazione_Cod", "number") With {
            ._hidden = True
        }
        l.Add(c)

        c = New ColonneNome("Macrouso_Cod", "Macrouso_Cod", "number") With {
            ._Editabile = True,
            ._hidden = True
        }
        l.Add(c)

        c = New ColonneNome("Macrouso_Des", "Macrouso UMA", "string") With {
            ._Editabile = False,
            ._obbligatorio = True,
            ._Filtrabile = True,
            ._width = "120px"
        }
        l.Add(c)

        c = New ColonneNome("Veg_Des", "Specie Vegetale", "string") With {
            ._Editabile = False,
            ._Filtrabile = True,
            ._width = "200px"
        }
        l.Add(c)
        c = New ColonneNome("Destinazione", "Destinazione", "String") With {
            ._Editabile = False,
            ._width = "200px"
        }
        l.Add(c)
        'c = New ColonneNome("varieta", "Varietà", "string") With {
        '    ._Editabile = True,
        '    ._hidden = True
        '}
        'l.Add(c)

        'c._hidden = True
        'c = New ColonneNome("cul_des", "Varietà", "string") With {
        '    ._Editabile = True,
        '    ._width = "200px"
        '}
        'l.Add(c)

        'c._hidden = True
        c = New ColonneNome("Richiesta_Cod", "Richiesta Cod", "number") With {
            ._Editabile = True,
            ._hidden = True
        }
        l.Add(c)

        c = New ColonneNome("PROV", "PR.", "String") With {
            ._width = "80px",
            ._Filtrabile = True
        }
        l.Add(c)

        c = New ColonneNome("COM", "COM", "String") With {
            ._width = "80px",
            ._Filtrabile = True
        }
        l.Add(c)

        c = New ColonneNome("APP_NOME", "APP NOME", "String") With {
            ._width = "80px",
            ._Filtrabile = True
        }
        l.Add(c)

        c = New ColonneNome("sup_tot", "Sup. Utilizzo [ha]", "number") With {
            ._Editabile = False,
            ._obbligatorio = True,
            ._Filtrabile = False,
            ._width = "120px",
            ._formatNr = "n4"
        }
        l.Add(c)

        c = New ColonneNome("sup_A", "Sup. in zona pendenza A", "number") With {
            ._Editabile = False,
            ._obbligatorio = True,
            ._Filtrabile = False,
            ._width = "120px",
            ._formatNr = "n4"
        }
        l.Add(c)

        c = New ColonneNome("sup_B", "Sup. in zona pendenza B", "number") With {
            ._Editabile = False,
            ._obbligatorio = True,
            ._Filtrabile = False,
            ._width = "120px",
            ._formatNr = "n4"
        }
        l.Add(c)

        c = New ColonneNome("TerrenoNormale", "Sup. a tessitura normale", "number") With {
            ._Editabile = False,
            ._obbligatorio = True,
            ._Filtrabile = False,
            ._width = "120px",
            ._formatNr = "n4"
        }
        l.Add(c)

        c = New ColonneNome("TerrenoMedio", "Sup. a tessitura media", "number") With {
            ._Editabile = False,
            ._obbligatorio = True,
            ._Filtrabile = False,
            ._width = "120px",
            ._formatNr = "n4"
        }
        l.Add(c)

        c = New ColonneNome("TerrenoTenace", "Sup. a tessitura tenace", "number") With {
            ._Editabile = False,
            ._obbligatorio = True,
            ._Filtrabile = False,
            ._width = "120px",
            ._formatNr = "n4"
        }
        l.Add(c)

        c = New ColonneNome("Centro", "Centro", "string") With {
            ._Editabile = False,
            ._Filtrabile = True,
            ._width = "100px"
        }
        l.Add(c)

        c = New ColonneNome("INIZIO_GESTIONE", "Inizio Gestione Appezzamento", "date") With {
            ._Editabile = False,
            ._obbligatorio = True,
            ._width = "133px",
            ._valueDefault = DataInizio,
            ._Filtrabile = True
        }
        l.Add(c)

        c = New ColonneNome("FINE_GESTIONE", "Fine Gestione Appezzamento", "date") With {
            ._Editabile = False,
            ._obbligatorio = True,
            ._width = "132px",
            ._valueDefault = DataFine,
            ._Filtrabile = True
        }
        l.Add(c)

        c = New ColonneNome("Uso_Cod", "Uso_Cod_Agea", "String") With {
            ._Editabile = False,
            ._hidden = True
        }
        l.Add(c)

        c = New ColonneNome("Occupazione_Cod", "Occupazione_Cod_Agea", "String") With {
            ._Editabile = False,
            ._hidden = True
        }
        l.Add(c)
        c = New ColonneNome("Destinazione_Cod", "Destinazione_Cod_Agea", "String") With {
            ._Editabile = False,
            ._hidden = True
        }
        l.Add(c)

        c = New ColonneNome("Qualita_Cod", "Qualita_Cod_Agea", "String") With {
            ._Editabile = True,
            ._hidden = True
        }
        l.Add(c)

        'ritorno la tabella trasformata in json (aggiustando anche le colonne)
        Dim js As New AgronicaCoreDataProvider.JSON_DataTable With {
            .Editabile_Deafault = False
        }

        Dim risp As String

        'If stringaKendoRow = "" Then
        '    Dim strKendoRow As New StringBuilder
        '    AgronicaCoreDataProvider.JSON_DataTable.kendo_Rows(DT_Appezzamenti, l, strKendoRow)
        '    risp = strKendoRow.ToString
        'Else
        risp = js.JSON_DataTable_Kendo(DT_Appezzamenti, l, AssegnaAutomaticamenteTipoFiltro_daTipoDato:=True, tipoFiltroKendo:=TipoFiltroKendo_colonne.Menu,
                                           stringaKendoRow:=stringaKendoRow) 'True, True, TipoFiltroKendo_colonne.CasellaTesto) '
        'End If

        Return risp

    End Function



    Private Shared Function JSON_DataTableAppezzamenti_Tabella(ByRef DT_Appezzamenti As DataTable,
                                                               ByVal DataInizio As String, ByVal DataFine As String,
                                                               Optional ByVal stringaKendoRow As String = "") As String

        'creo la lista delle colonne da visualizzare
        Dim l As New List(Of ColonneNome)
        Dim c As ColonneNome

        c = New ColonneNome("chiave", "chiave", "number") With {
            ._hidden = True
        }
        l.Add(c)

        Dim colonneNome As ColonneNome = New ColonneNome("piva", "piva", "string") With {
            ._hidden = True
        }
        c = colonneNome
        l.Add(c)
        c = New ColonneNome("sa_cod", "sa_cod", "number") With {
            ._hidden = True,
            ._Editabile = True
        }
        l.Add(c)

        c = New ColonneNome("appezza", "appezza", "number") With {
            ._hidden = True
        }
        l.Add(c)

        c = New ColonneNome("programmazione_entita_cod", "Programmazione_Entita_Cod", "number") With {
            ._hidden = True
        }
        l.Add(c)

        'c._RemoveHtmlEncode = True
        c = New ColonneNome("datoGis", "Gis", "String") With {
            ._FormatoParticolare = "<span class='fa fa-globe fa-2x'></span>",
            ._Filtrabile = False,
            ._width = "60px"
        }
        l.Add(c)

        'c._Filtrabile = False
        c = New ColonneNome("app_nome", "App.", "string") With {
            ._Editabile = True,
            ._obbligatorio = True,
            ._width = "78px"
        }
        l.Add(c)

        c = New ColonneNome("utilizzo_sup", "Sup. Utilizzo [ha] (SAU)", "number") With {
            ._Editabile = True,
            ._obbligatorio = True,
            ._Filtrabile = False,
            ._width = "120px",
            ._formatNr = "n4"
        }
        l.Add(c)

        c = New ColonneNome("macrouso_cod", "Macrouso_Cod", "string") With {
            ._Editabile = True,
            ._hidden = True
        }
        l.Add(c)

        c = New ColonneNome("macrouso_sup", "Macrouso_Sup", "string") With {
            ._Editabile = True,
            ._hidden = True
        }
        l.Add(c)

        c = New ColonneNome("utilizzo", "Utilizzo (Specie Vegetale)", "string") With {
            ._Editabile = True,
            ._hidden = True
        }
        l.Add(c)

        c = New ColonneNome("varieta", "Varietà", "string") With {
            ._Editabile = True,
            ._hidden = True
        }
        l.Add(c)

        'c._hidden = True
        c = New ColonneNome("veg_des", "Specie Vegetale", "string") With {
            ._Editabile = True,
            ._width = "200px"
        }
        l.Add(c)

        'c._hidden = True
        c = New ColonneNome("cul_des", "Varietà", "string") With {
            ._Editabile = True,
            ._width = "200px"
        }
        l.Add(c)

        'c._hidden = True
        c = New ColonneNome("grfi_des", "Finalità Produttiva", "string") With {
            ._Editabile = True,
            ._width = "200px"
        }
        l.Add(c)

        'c._hidden = True
        c = New ColonneNome("macrouso", "Macrouso", "string") With {
            ._Editabile = True,
            ._width = "200px"
        }
        l.Add(c)

        'c._hidden = True
        c = New ColonneNome("grva_des", "Tipologia Varietale", "string") With {
            ._width = "101px"
        }
        l.Add(c)

        ''GESTIONE CATASTO IN LINEA
        'c._Editabile = True
        'c._hidden = True
        c = New ColonneNome("PROV", "PROV", "String") With {
            ._width = "80px"
        }
        l.Add(c)

        'c._Editabile = True
        'c._hidden = True
        c = New ColonneNome("prov_des", "PR.", "String") With {
            ._width = "80px"
        }
        l.Add(c)

        'c._Editabile = True
        'c._hidden = True
        c = New ColonneNome("COM", "COM", "String") With {
            ._width = "80px"
        }
        l.Add(c)

        'c._Editabile = True
        'c._hidden = True
        c = New ColonneNome("com_des", "Comune", "String") With {
            ._width = "150px"
        }
        l.Add(c)

        'c._Editabile = True
        'c._hidden = True
        c = New ColonneNome("sezione", "Sez.", "String") With {
            ._width = "80px"
        }
        l.Add(c)

        'c._Editabile = True
        'c._hidden = True
        c = New ColonneNome("foglio", "Fgl.", "String") With {
            ._width = "80px"
        }
        l.Add(c)

        'c._Editabile = True
        'c._hidden = True
        c = New ColonneNome("numero", "Numero", "String") With {
            ._width = "100px"
        }
        l.Add(c)

        'c._Editabile = True
        'c._hidden = True
        c = New ColonneNome("subalterno", "Sub.", "String") With {
            ._width = "80px"
        }
        l.Add(c)

        c = New ColonneNome("sa_nome", "Centro", "string") With {
            ._Editabile = True,
            ._Filtrabile = False,
            ._width = "100px"
        }
        c._Filtrabile = True
        l.Add(c)

        c = New ColonneNome("validita_inizio", "Inizio Gestione Appezzamento", "date") With {
            ._Editabile = True,
            ._obbligatorio = True,
            ._width = "133px",
            ._valueDefault = DataInizio,
            ._Filtrabile = True
        }
        l.Add(c)

        c = New ColonneNome("validita_fine", "Fine Gestione Appezzamento", "date") With {
            ._Editabile = True,
            ._obbligatorio = True,
            ._width = "132px",
            ._valueDefault = DataFine,
            ._Filtrabile = True
        }
        l.Add(c)

        c = New ColonneNome("prov", "PROV", "string") With {
            ._hidden = True
        }
        l.Add(c)
        c = New ColonneNome("com", "COM", "string") With {
            ._hidden = True
        }
        l.Add(c)

        c = New ColonneNome("catasto", "Catasto (Provincia-Comune-Sezione-Foglio-Numero-Subalterno)", "string") With {
            ._FormatoParticolare = "#= GetValoreParticella(chiave, catasto)#",
            ._RemoveHtmlEncode = True,
            ._hidden = True
        }
        l.Add(c)

        c = New ColonneNome("catasto_key", "Catasto_Key", "String") With {
            ._RemoveHtmlEncode = True,
            ._hidden = True
        }
        l.Add(c)

        c = New ColonneNome("id_cod", "Id_Cod", "String") With {
            ._hidden = True
        }
        l.Add(c)
        c = New ColonneNome("veg_cod", "Veg_Cod", "String") With {
            ._hidden = True,
            ._Editabile = True
        }
        l.Add(c)
        c = New ColonneNome("cul_cod", "Cul_Cod", "String") With {
            ._hidden = True,
            ._Editabile = True
        }
        l.Add(c)

        c = New ColonneNome("grfi_cod", "Grfi_Cod", "String") With {
            ._hidden = True,
            ._Editabile = True
        }
        l.Add(c)
        c = New ColonneNome("grva_cod", "Grva_Cod", "String") With {
            ._hidden = True
        }
        l.Add(c)

        c = New ColonneNome("veg_cod_agea", "Veg_Cod_Agea", "String") With {
            ._hidden = True,
            ._Editabile = True
        }
        l.Add(c)
        c = New ColonneNome("cul_cod_agea", "Cul_Cod_Agea", "String") With {
            ._hidden = True,
            ._Editabile = True
        }
        l.Add(c)

        c = New ColonneNome("ribaltato", "ribaltato", "String") With {
            ._hidden = True
        }
        l.Add(c)

        c = New ColonneNome("movimentato", "movimentato", "String") With {
            ._hidden = True
        }
        l.Add(c)


        c = New ColonneNome("Cop_Cod", "Cop_Cod", "String") With {
            ._Editabile = True,
            ._hidden = True
        }
        l.Add(c)
        'c._hidden = True
        c = New ColonneNome("Cop_Des", "Copertura", "String") With {
            ._Editabile = True,
            ._width = "130px"
        }
        l.Add(c)
        'c._hidden = True
        c = New ColonneNome("Lotto", "Lotto", "String") With {
            ._Editabile = True,
            ._width = "110px"
        }
        l.Add(c)
        'c._hidden = True
        c = New ColonneNome("Resa", "Resa (t/ha)", "String") With {
            ._Editabile = True,
            ._width = "77px"
        }
        l.Add(c)
        'c._hidden = True
        c = New ColonneNome("Num_Piante", "Num. Piante", "String") With {
            ._Editabile = True,
            ._width = "80px"
        }
        l.Add(c)
        'c._hidden = True
        c = New ColonneNome("TRA_Fila", "TRA Fila (m)", "number") With {
            ._Editabile = True,
            ._width = "66px"
        }
        l.Add(c)
        'c._hidden = True
        c = New ColonneNome("SU_Fila", "SU Fila (m)", "number") With {
            ._Editabile = True,
            ._width = "66px"
        }
        l.Add(c)
        'c._hidden = True
        c = New ColonneNome("Validita_Inizio_Impianto", "Validita Inizio Impianto", "date") With {
            ._Editabile = True,
            ._valueDefault = DataInizio,
            ._width = "100px"
        }
        l.Add(c)
        c = New ColonneNome("TipoZona", "TipoZona", "String") With {
            ._Editabile = True,
            ._hidden = True
        }
        l.Add(c)
        'c._hidden = True
        c = New ColonneNome("TipoZona_Des", "Tipo Zona", "String") With {
            ._Editabile = True,
            ._width = "86px"
        }
        l.Add(c)
        c = New ColonneNome("MetodoProduzione_Cod", "MetodoProduzione_Cod", "String") With {
            ._Editabile = True,
            ._hidden = True
        }
        l.Add(c)
        'c._hidden = True
        c = New ColonneNome("MetodoProduzione_Des", "Metodo Produzione", "String") With {
            ._Editabile = True,
            ._width = "109px"
        }
        l.Add(c)
        'c._hidden = True
        c = New ColonneNome("Unita_Vitata", "Unità Vitata", "String") With {
            ._Editabile = True,
            ._width = "78px"
        }
        l.Add(c)
        c = New ColonneNome("Veg_Cod_Agea", "Veg_Cod_Agea", "String") With {
            ._Editabile = True,
            ._hidden = True
        }
        l.Add(c)
        c = New ColonneNome("Veg_Des_Agea", "Veg_Des_Agea", "String") With {
            ._Editabile = True,
            ._hidden = True
        }
        l.Add(c)
        c = New ColonneNome("Cul_Cod_Agea", "Cul_Cod_Agea", "String") With {
            ._Editabile = True,
            ._hidden = True
        }
        l.Add(c)
        c = New ColonneNome("Cul_Des_Agea", "Cul_Des_Agea", "String") With {
            ._Editabile = True,
            ._hidden = True
        }
        l.Add(c)
        c = New ColonneNome("Uso_Cod_Agea", "Uso_Cod_Agea", "String") With {
            ._Editabile = True,
            ._hidden = True
        }
        l.Add(c)
        c = New ColonneNome("Uso_Des_Agea", "Uso_Des_Agea", "String") With {
            ._Editabile = True,
            ._hidden = True
        }
        l.Add(c)
        c = New ColonneNome("Occupazione_Cod_Agea", "Occupazione_Cod_Agea", "String") With {
            ._Editabile = True,
            ._hidden = True
        }
        l.Add(c)
        c = New ColonneNome("Occupazione_Des_Agea", "Occupazione_Des_Agea", "String") With {
            ._Editabile = True,
            ._hidden = True
        }
        l.Add(c)
        c = New ColonneNome("Destinazione_Cod_Agea", "Destinazione_Cod_Agea", "String") With {
            ._Editabile = True,
            ._hidden = True
        }
        l.Add(c)
        c = New ColonneNome("Destinazione_Des_Agea", "Destinazione_Des_Agea", "String") With {
            ._Editabile = True,
            ._hidden = True
        }
        l.Add(c)
        c = New ColonneNome("Qualita_Cod_Agea", "Qualita_Cod_Agea", "String") With {
            ._Editabile = True,
            ._hidden = True
        }
        l.Add(c)
        c = New ColonneNome("Qualita_Des_Agea", "Qualita_Des_Agea", "String") With {
            ._Editabile = True,
            ._hidden = True
        }
        l.Add(c)

        c = New ColonneNome("Gru_Cod", "Gru_Cod", "String") With {
            ._Editabile = True,
            ._hidden = True
        }
        l.Add(c)

        c = New ColonneNome("Dpi_Cod", "Dpi_Cod", "String") With {
            ._Editabile = True,
            ._hidden = True
        }
        l.Add(c)

        c = New ColonneNome("Reg_Cod", "Reg_Cod", "String") With {
            ._Editabile = True,
            ._hidden = True
        }
        l.Add(c)

        c = New ColonneNome("Regolamento_Concimazione_Cod", "Regolamento_Concimazione_Cod", "String") With {
            ._Editabile = True,
            ._hidden = True
        }
        l.Add(c)

        c = New ColonneNome("Disciplinare", "Disciplinare", "String") With {
            ._Editabile = True,
            ._hidden = True
        }
        l.Add(c)

        c = New ColonneNome("Flag_PubblicoPrivato", "Flag_PubblicoPrivato", "String") With {
            ._Editabile = True,
            ._hidden = True
        }
        l.Add(c)

        c = New ColonneNome("id_tr", "id_tr", "String") With {
            ._Editabile = True,
            ._hidden = True
        }
        l.Add(c)

        c = New ColonneNome("StatoImpianto_Cod", "StatoImpianto_Cod", "String") With {
            ._Editabile = True,
            ._hidden = True
        }
        l.Add(c)

        'c._hidden = True
        c = New ColonneNome("N", "N (kg/ha)", "String") With {
            ._Editabile = True,
            ._width = "87px"
        }
        l.Add(c)

        'c._hidden = True
        c = New ColonneNome("P", "P (kg/ha)", "String") With {
            ._Editabile = True,
            ._width = "87px"
        }
        l.Add(c)

        'c._hidden = True
        c = New ColonneNome("K", "K (kg/ha)", "String") With {
            ._Editabile = True,
            ._width = "87px"
        }
        l.Add(c)

        'c._hidden = True
        c = New ColonneNome("Data_Semina", "Data Semina", "date") With {
            ._Editabile = True,
            ._width = "92px"
        }
        l.Add(c)

        'c._hidden = True
        c = New ColonneNome("Data_Raccolta", "Data Raccolta", "date") With {
            ._Editabile = True,
            ._valueDefault = DataInizio,
            ._width = "92px"
        }
        l.Add(c)

        'c._hidden = True
        c = New ColonneNome("Data_Fioritura", "Data Fioritura", "date") With {
            ._Editabile = True,
            ._valueDefault = DataInizio,
            ._width = "92px"
        }
        l.Add(c)

        c = New ColonneNome("Coltura_Precedente", "Coltura_Precedente", "String") With {
            ._Editabile = True,
            ._hidden = True
        }
        l.Add(c)

        c = New ColonneNome("Coltura_Precedente2", "Coltura_Precedente2", "String") With {
            ._Editabile = True,
            ._hidden = True
        }
        l.Add(c)

        c = New ColonneNome("Coltura_Precedente3", "Coltura_Precedente3", "String") With {
            ._Editabile = True,
            ._hidden = True
        }
        l.Add(c)

        c = New ColonneNome("Coltura_Precedente4", "Coltura_Precedente4", "String") With {
            ._Editabile = True,
            ._hidden = True
        }
        l.Add(c)

        c = New ColonneNome("Piano_Semina", "Piano_Semina", "String") With {
            ._Editabile = True,
            ._hidden = True
        }
        l.Add(c)

        c = New ColonneNome("Codice_Contratto", "Codice_Contratto", "String") With {
            ._Editabile = True,
            ._hidden = True
        }
        l.Add(c)

        'c._Editabile = True
        c = New ColonneNome("unito", "unito", "number") With {
            ._hidden = True
        }
        l.Add(c)

        'c._Editabile = True
        c = New ColonneNome("frazionato", "frazionato", "number") With {
            ._hidden = True
        }
        l.Add(c)

        'c._Editabile = True
        c = New ColonneNome("Codice_Fiscale_Tecnico", "Codice_Fiscale_Tecnico", "String") With {
            ._hidden = True
        }
        l.Add(c)

        'c._Editabile = True
        c = New ColonneNome("stato_ribaltamento", "stato_ribaltamento", "number") With {
            ._hidden = True
        }
        l.Add(c)

        'c._Editabile = True
        c = New ColonneNome("provenienza_fascicolo", "provenienza_fascicolo", "String") With {
            ._hidden = True
        }
        l.Add(c)

        c = New ColonneNome("IAF", "IAF", "String")
        'c._Editabile = True
        'c._hidden = True
        l.Add(c)

        'c._hidden = True
        c = New ColonneNome("DistBZ_CorpiIdrici", "Corpi idridi superficiali [m]", "number") With {
            ._Editabile = True,
            ._width = "107px"
        }
        l.Add(c)

        'c._hidden = True
        c = New ColonneNome("DistBZ_AreeResPub", "Aree residenziali/ pubbliche [m]", "number") With {
            ._Editabile = True,
            ._width = "115px"
        }
        l.Add(c)

        'c._hidden = True
        c = New ColonneNome("DistBZ_Allevamenti", "Allevamenti [m]", "number") With {
            ._Editabile = True,
            ._width = "111px"
        }
        l.Add(c)

        'c._hidden = True
        c = New ColonneNome("DistBZ_VegNatNonColt", "Vegetazione naturale/non coltivata [m]", "number") With {
            ._Editabile = True,
            ._width = "120px"
        }
        l.Add(c)

        'c._hidden = True
        c = New ColonneNome("SupBZ_Riduzione", "Offset da ultima pianta (Capezzagna) [m]", "number") With {
            ._Editabile = True,
            ._width = "120px"
        }
        l.Add(c)

        c = New ColonneNome("campo_cod", "campo_cod", "number") With {
            ._Editabile = True,
            ._hidden = True
        }
        l.Add(c)

        'c._hidden = True
        c = New ColonneNome("campo_des", "Campo", "string") With {
            ._Editabile = True,
            ._width = "200px"
        }
        l.Add(c)

        c = New ColonneNome("wkt", "wkt", "String") With {
            ._Editabile = True,
            ._hidden = True
        }
        l.Add(c)

        c = New ColonneNome("wkt_georiferimento_cod", "wkt_georiferimento_cod", "String") With {
            ._Editabile = True,
            ._hidden = True
        }
        l.Add(c)

        c = New ColonneNome("Riferimento_Alfanumerico_Appezzamento", "Riferimento_Alfanumerico_Appezzamento", "String") With {
            ._Editabile = True
        }
        l.Add(c)

        c = New ColonneNome("Isola", "Isola", "String") With {
            ._Editabile = True
        }
        l.Add(c)

        c = New ColonneNome("CapitolatoPrivato", "CapitolatoPrivato", "String") With {
            ._Editabile = True
        }
        l.Add(c)

        c = New ColonneNome("CapitolatoPrivato_Des", "CapitolatoPrivato_Des", "String") With {
            ._Editabile = True
        }
        l.Add(c)

        c = New ColonneNome("Finalita_Concimazione_Impianto", "Finalita_Concimazione_Impianto", "number") With {
            ._Editabile = True,
            ._hidden = True
        }
        l.Add(c)

        'INDIRIZZO
        c = New ColonneNome("Cod_Indirizzo", "Cod_Indirizzo", "number") With {
            ._Editabile = True,
            ._hidden = True
        }
        l.Add(c)

        c = New ColonneNome("ind_des", "ind_des", "string") With {
            ._Editabile = True,
            ._hidden = True
        }
        l.Add(c)

        c = New ColonneNome("frz_des", "frz_des", "string") With {
            ._Editabile = True,
            ._hidden = True
        }
        l.Add(c)

        c = New ColonneNome("CAP", "CAP", "string") With {
            ._Editabile = True,
            ._hidden = True
        }
        l.Add(c)

        c = New ColonneNome("com_des_indirizzo", "com_des", "string") With {
            ._Editabile = True,
            ._hidden = True
        }

        l.Add(c)
        c = New ColonneNome("pro_cod_indirizzo", "pro_cod", "string") With {
            ._Editabile = True,
            ._hidden = True
        }
        l.Add(c)

        c = New ColonneNome("stato_indirizzo", "stato", "string") With {
            ._Editabile = True,
            ._hidden = True
        }
        l.Add(c)

        c = New ColonneNome("stato_indirizzo_des", "stato_des", "string") With {
            ._Editabile = True,
            ._hidden = True
        }
        l.Add(c)

        c = New ColonneNome("note_indirizzo", "note", "string") With {
            ._Editabile = True,
            ._hidden = True
        }
        l.Add(c)

        c = New ColonneNome("pro_cod_istat_indirizzo", "pro_cod_istat", "string") With {
            ._Editabile = True,
            ._hidden = True
        }
        l.Add(c)

        c = New ColonneNome("com_cod_istat_indirizzo", "com_cod_istat", "string") With {
            ._Editabile = True,
            ._hidden = True
        }
        l.Add(c)


        c = New ColonneNome("Pratiche_Cod", "Pratiche_Cod", "string") With {
            ._Editabile = True,
            ._hidden = True
        }
        l.Add(c)

        c = New ColonneNome("Pratiche_Des", "Pratiche_Des", "string") With {
            ._Editabile = True
        }
        l.Add(c)

        c = New ColonneNome("KPIN", "KPIN", "string") With {
            ._Editabile = True
        }
        l.Add(c)

        c = New ColonneNome("Block_Name", "Block_Name", "string") With {
            ._Editabile = True
        }
        l.Add(c)

        c = New ColonneNome("Foral_Cod", "Foral_Cod", "number") With {
            ._Editabile = True
        }
        l.Add(c)

        c = New ColonneNome("Foral_Des", "Foral_Des", "string") With {
            ._Editabile = True
        }
        l.Add(c)

        c = New ColonneNome("Data_Inizio_Portinnesto", "Data_Inizio_Portinnesto", "date") With {
            ._Editabile = True
        }
        l.Add(c)

        c = New ColonneNome("Data_Creazione", "Data_Creazione", "date") With {
            ._Editabile = True
        }
        l.Add(c)

        c = New ColonneNome("Data_Modifica", "Data_Modifica", "date") With {
            ._Editabile = True
        }
        l.Add(c)

        c = New ColonneNome("ZespriFase_Cod", "ZespriFase_Cod", "string") With {
            ._Editabile = True
        }
        l.Add(c)

        c = New ColonneNome("ZespriFase_Des", "ZespriFase_Des", "string") With {
            ._Editabile = True
        }
        l.Add(c)

        c = New ColonneNome("ZespriTipo_Cod", "ZespriTipo_Cod", "string") With {
            ._Editabile = True
        }
        l.Add(c)

        c = New ColonneNome("ZespriTipo_Des", "ZespriTipo_Des", "string") With {
            ._Editabile = True
        }
        l.Add(c)

        c = New ColonneNome("ZespriGrower_Cod", "ZespriGrower_Cod", "string") With {
            ._Editabile = True
        }
        l.Add(c)

        c = New ColonneNome("ZespriGrower_Des", "ZespriGrower_Des", "string") With {
            ._Editabile = True
        }
        l.Add(c)

        c = New ColonneNome("Num_Piante_Femmine", "N. Piante Maschi", "number") With {
            ._Editabile = True
        }
        l.Add(c)

        c = New ColonneNome("Num_Piante_Maschi", "N. Piante Femmine", "number") With {
            ._Editabile = True
        }
        l.Add(c)

        c = New ColonneNome("Port_Cod", "Port_Cod", "number") With {
            ._Editabile = True
        }
        l.Add(c)

        c = New ColonneNome("Port_Des", "Port_Des", "string") With {
            ._Editabile = True
        }
        l.Add(c)

        c = New ColonneNome("TipologiaInnestoTrapianto_Cod", "TipologiaInnestoTrapianto_Cod", "number") With {
            ._Editabile = True
        }
        l.Add(c)

        c = New ColonneNome("TipologiaInnestoTrapianto_Des", "TipologiaInnestoTrapianto_Des", "string") With {
            ._Editabile = True
        }
        l.Add(c)

        'If Not String.IsNullOrEmpty(stringaKendoRow) Then

        '    Dim addedProp As Boolean = False
        '    Dim jo As JArray = JArray.Parse(stringaKendoRow)
        '    For Each rr As JObject In jo.Descendants().OfType(Of JObject)
        '        If Not rr("wkt") Is Nothing Then
        '            'esiste già...
        '            Exit For
        '        Else
        '            rr.Add(New JProperty("wkt", ""))
        '            rr.Add(New JProperty("wkt_georiferimento_cod", ""))
        '            addedProp = True
        '        End If
        '    Next

        '    If addedProp Then
        '        stringaKendoRow = jo.ToString()
        '    End If

        'End If

        'ritorno la tabella trasformata in json (aggiustando anche le colonne)
        Dim js As New AgronicaCoreDataProvider.JSON_DataTable With {
            .Editabile_Deafault = False
        }

        Dim risp As String

        If stringaKendoRow = "" Then
            Dim strKendoRow As New StringBuilder
            AgronicaCoreDataProvider.JSON_DataTable.kendo_Rows(DT_Appezzamenti, l, strKendoRow)
            risp = strKendoRow.ToString
        Else
            risp = js.JSON_DataTable_Kendo(DT_Appezzamenti, l, AssegnaAutomaticamenteTipoFiltro_daTipoDato:=True, tipoFiltroKendo:=TipoFiltroKendo_colonne.Menu,
                                           stringaKendoRow:=stringaKendoRow) 'True, True, TipoFiltroKendo_colonne.CasellaTesto) '
        End If

        Return risp

    End Function


    <WebMethod(EnableSession:=True)>
    Public Shared Function Cerca_Dettagli_Azienda(piva As String,
                                                  Richiesta_Cod As Integer,
                                                  Tipo_Richiesta As Integer,
                                                  Avanzamento_Richiesta As Integer,
                                                  annoR As Integer 'R come Richiesta, rendicontazione o richiesta anticipo
                                                  ) As RispostaStandard

        Dim r As New RispostaStandard
        Dim ind_des As String = ""
        Dim com_des As String = ""
        Dim CUAA As String
        Dim cod As Integer = 0
        Dim Dt As New DataTable
        Dim leggiIndirizzo As New AgronicaCoreAnagrafeDAL.ImpresexIndirizzi_R
        Dim leggiCUAA As New AgronicaCoreAnagrafeDAL.Imprese_Codici_Read
        Dim check As New AgronicaCoreUmaDal.UMA_Richieste_Testata_R
        Dim aggiornaRimanenze As New AgronicaCoreUmaDal.UMA_Richieste_Testata_W
        Dim vendite As New AgronicaCoreUmaDal.UMA_Vendite_R
        Dim docs As New AgronicaCoreScadenziario.RichiestaDocumenti_R

        Try
            Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")

            Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
                r.Sessione = False
                Return r
            End If

            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Utenti) Then
                r.Sessione = False
                Return r
            End If

            Lingua.Gias_InizializzaCultura_DaSession()

            leggiIndirizzo.Indirizzo_from_Piva(piva, "", ind_des, "", "", com_des, "",
                                      "", "", objParametri_Server)

            CUAA = leggiCUAA.Leggi_CUAA(piva, objParametri_Server)
            Dim testata As New DataTable
            If Richiesta_Cod > 0 Then
                testata = check.CheckTestate(piva, Richiesta_Cod, Tipo_Richiesta, objParametri_Server)
            End If

            Dim anno = ""
            Dim nDichiarazione = ""
            Dim benzina As Integer
            Dim gasolio As Integer
            Dim gasolioSerra As Integer

            Dim benzinaAppro As Integer
            Dim gasolioAppro As Integer
            Dim gasolioSerraAppro As Integer

            Dim stato_des As String = ""
            Dim stato_cod As Integer = 0
            Dim pratica_cod As Integer = 0

            Dim benzinaTot As Integer
            Dim gasolioTot As Integer
            Dim gasolioSerraTot As Integer

            Dim Rimanenza_Gasolio As Double = 0
            Dim Rimanenza_Benzina As Double = 0
            Dim Rimanenza_Gasolio_Serra As Double = 0

            Dim Rimanenza_Gasolio_prec As Double = 0
            Dim Rimanenza_Benzina_prec As Double = 0
            Dim Rimanenza_Gasolio_Serra_prec As Double = 0

            Dim Anticipazioni_Gasolio As Double = 0
            Dim Anticipazioni_Benzina As Double = 0
            Dim Anticipazioni_Gasolio_Serra As Double = 0
            Dim Anticipazioni_Altro As Double = 0

            Dim modifica_richiesta = True
            Dim modifica_assegnato = True
            Dim aliquota_anticipo As Double

            Dim dataPrimoAcquisto As New Date

            Dim tipo_azienda As Integer = 0

            Dim integrativa As Boolean = False
            Dim cessata As Boolean = False

            Dim permessoAcqua As Double = 0.0
            Dim notePermessoAcqua As String = ""
            Dim permessoAcquaGiaRich As Double = 0.0

            Dim rim_riass_gasolio As Double
            Dim rim_riass_benzina As Double
            Dim rim_riass_gasolio_serra As Double
            Dim rim_riass_conf_gasolio As Double
            Dim rim_riass_conf_benzina As Double
            Dim rim_riass_conf_gasolio_serra As Double
            Dim rec_acc_dich_gasolio As Double
            Dim rec_acc_dich_benzina As Double
            Dim rec_acc_dich_gasolio_serra As Double
            Dim rec_acc_conf_gasolio As Double
            Dim rec_acc_conf_benzina As Double
            Dim rec_acc_conf_gasolio_serra As Double
            Dim Causale_Non_Utilizzo As String = ""
            Dim Rim_Dich_Gasolio As Double
            Dim Rim_Dich_Benzina As Double
            Dim Rim_Dich_Gasolio_Serra As Double
            Dim approvatore As String = ""

            Dim Allevati_Montagna As Boolean

            If testata IsNot Nothing AndAlso testata.Rows.Count > 0 Then

                Dim riga = testata.Rows(0)

                tipo_azienda = riga("tipo_azienda")

                cod = riga("Richiesta_Cod")
                anno = riga("anno")
                nDichiarazione = riga("numero")
                aliquota_anticipo = IIf(IsDBNull(riga("Percentuale_Anticipo_Richiesta_da_Azienda")), 0, riga("Percentuale_Anticipo_Richiesta_da_Azienda"))
                integrativa = riga("Richiesta_Integrativa") = 1

                benzina = IIf(IsDBNull(riga("Richiesta_Iniziale_Benzina")), 0, riga("Richiesta_Iniziale_Benzina"))
                gasolio = IIf(IsDBNull(riga("Richiesta_Iniziale_Gasolio")), 0, riga("Richiesta_Iniziale_Gasolio"))
                gasolioSerra = IIf(IsDBNull(riga("Richiesta_Iniziale_Gasolio_Serra")), 0, riga("Richiesta_Iniziale_Gasolio_Serra"))

                benzinaAppro = IIf(IsDBNull(riga("Approvazione_Iniziale_Benzina")), 0, riga("Approvazione_Iniziale_Benzina"))
                gasolioAppro = IIf(IsDBNull(riga("Approvazione_Iniziale_Gasolio")), 0, riga("Approvazione_Iniziale_Gasolio"))
                gasolioSerraAppro = IIf(IsDBNull(riga("Approvazione_Iniziale_Gasolio_Serra")), 0, riga("Approvazione_Iniziale_Gasolio_Serra"))

                benzinaTot = IIf(IsDBNull(riga("Carburante_Richiesto_Benzina")), 0, riga("Carburante_Richiesto_Benzina"))
                gasolioTot = IIf(IsDBNull(riga("Carburante_Richiesto_Gasolio")), 0, riga("Carburante_Richiesto_Gasolio"))
                gasolioSerraTot = IIf(IsDBNull(riga("Carburante_Richiesto_Gasolio_Serra")), 0, riga("Carburante_Richiesto_Gasolio_Serra"))

                Rimanenza_Gasolio = IIf(IsDBNull(riga("Rimanenza_Gasolio")), 0, riga("Rimanenza_Gasolio"))
                Rimanenza_Benzina = IIf(IsDBNull(riga("Rimanenza_Benzina")), 0, riga("Rimanenza_Benzina"))
                Rimanenza_Gasolio_Serra = IIf(IsDBNull(riga("Rimanenza_Gasolio_Serra")), 0, riga("Rimanenza_Gasolio_Serra"))

                rim_riass_gasolio = IIf(IsDBNull(riga("rim_riass_gasolio")), 0, riga("rim_riass_gasolio"))
                rim_riass_benzina = IIf(IsDBNull(riga("rim_riass_benzina")), 0, riga("rim_riass_benzina"))
                rim_riass_gasolio_serra = IIf(IsDBNull(riga("rim_riass_gasolio_serra")), 0, riga("rim_riass_gasolio_serra"))

                rim_riass_conf_gasolio = IIf(IsDBNull(riga("rim_riass_conf_gasolio")), 0, riga("rim_riass_conf_gasolio"))
                rim_riass_conf_benzina = IIf(IsDBNull(riga("rim_riass_conf_benzina")), 0, riga("rim_riass_conf_benzina"))
                rim_riass_conf_gasolio_serra = IIf(IsDBNull(riga("rim_riass_conf_gasolio_serra")), 0, riga("rim_riass_conf_gasolio_serra"))

                rec_acc_dich_gasolio = IIf(IsDBNull(riga("rec_acc_dich_gasolio")), 0, riga("rec_acc_dich_gasolio"))
                rec_acc_dich_benzina = IIf(IsDBNull(riga("rec_acc_dich_benzina")), 0, riga("rec_acc_dich_benzina"))
                rec_acc_dich_gasolio_serra = IIf(IsDBNull(riga("rec_acc_dich_gasolio_serra")), 0, riga("rec_acc_dich_gasolio_serra"))

                rec_acc_conf_gasolio = IIf(IsDBNull(riga("rec_acc_conf_gasolio")), 0, riga("rec_acc_conf_gasolio"))
                rec_acc_conf_benzina = IIf(IsDBNull(riga("rec_acc_conf_benzina")), 0, riga("rec_acc_conf_benzina"))
                rec_acc_conf_gasolio_serra = IIf(IsDBNull(riga("rec_acc_conf_gasolio_serra")), 0, riga("rec_acc_conf_gasolio_serra"))

                Causale_Non_Utilizzo = IIf(IsDBNull(riga("Causale_Non_Utilizzo")), "", riga("Causale_Non_Utilizzo"))

                Rim_Dich_Gasolio = IIf(IsDBNull(riga("Rim_Dich_Gasolio")), 0, riga("Rim_Dich_Gasolio"))
                Rim_Dich_Benzina = IIf(IsDBNull(riga("Rim_Dich_Benzina")), 0, riga("Rim_Dich_Benzina"))
                Rim_Dich_Gasolio_Serra = IIf(IsDBNull(riga("Rim_Dich_Gasolio_Serra")), 0, riga("Rim_Dich_Gasolio_Serra"))

                Allevati_Montagna = If(IsDBNull(riga("Allevati_Montagna")), False, IIf(riga("Allevati_Montagna") = 1, True, False))

                cessata = riga("Rinuncia_Nuova_Richiesta_Anno_Successivo")

                permessoAcqua = If(IsDBNull(riga("Permesso_acqua")), 0, riga("Permesso_acqua"))
                notePermessoAcqua = If(IsDBNull(riga("Note_permesso_acqua")), "", riga("Note_permesso_acqua"))

                If integrativa AndAlso Tipo_Richiesta = enum_UMA_TipoRichiesta.Conto_Proprio Then
                    Dim strFiltroRichiestePrec = "UMA_Richieste_Testata.Richiesta_Cod < " & Richiesta_Cod &
                        " AND Pratiche_Stati_Attuali.Stato_Cod = " & enum_WAnagraficaStati.Verifica_Intermedia_Completata_Con_Successo
                    Dim dtTestataPrec = check.Leggi(piva, 0, 0,
                                                    AGRODATAINIZIO, AGRODATAFINE,
                                                    annoR, "", enum_UMA_Avanzamento.Richiesta,
                                                    objParametri_Server,
                                                    isTerzista:=False,
                                                    xFiltroAggiuntivo:=strFiltroRichiestePrec)
                    For Each testataPrecRow In dtTestataPrec.Rows
                        permessoAcquaGiaRich += testataPrecRow("Permesso_acqua")
                    Next
                End If

                Dim pratiche_stati As New AgronicaCoreProfilazioneDAL.Pratiche_R

                Dim pratica = pratiche_stati.Leggi_conStatoAttuale(riga("Pratica_Cod"), "", "", "",
                                                    0, 0, 0, 0, 0, AGRODATAINIZIO, AGRODATAFINE,
                                                    "", "", objParametri_Server, 0, 0)

                If pratica.Rows.Count > 0 Then
                    stato_des = pratica.Rows(0)("Stato_Des")
                    stato_cod = pratica.Rows(0)("Stato_Cod")
                End If

                Dim DtPrimoAcquisto = vendite.OttieniDataPrimoAcquisto(piva, riga("anno"), Tipo_Richiesta, "", objParametri_Server)

                If (DtPrimoAcquisto.Rows.Count <> 0) Then
                    dataPrimoAcquisto = DtPrimoAcquisto.Rows.Item(0).Item("Data_Documento")
                End If

                If stato_cod <= 2006 AndAlso stato_cod >= 2002 Then
                    approvatore = check.RecuperaApprovatore(riga("Pratica_Cod"), objParametri_Server, objParametri_Utenti)
                End If

                Dim DtDocs = docs.LeggiRichiestaDocumenti(objParametri_Server, piva, Richiesta_Cod, objParametri_Utenti)
                For Each doc As DataRow In DtDocs.Rows

                    If ((doc.Item("Id_Schema_Template") = 18 OrElse doc.Item("Id_Schema_Template") = 16 OrElse doc.Item("Id_Schema_Template") = 14) AndAlso doc.Item("Nr_Documenti_Presenti") > 0) OrElse doc.Item("WAnagraficaStati_Cod") <> enum_WAnagraficaStati.In_Compilazione Then

                        modifica_richiesta = False
                    End If

                    If ((doc.Item("Id_Schema_Template") = 19 OrElse doc.Item("Id_Schema_Template") = 17 OrElse doc.Item("Id_Schema_Template") = 15) AndAlso doc.Item("Nr_Documenti_Presenti") > 0) OrElse doc.Item("WAnagraficaStati_Cod") <> enum_WAnagraficaStati.Verifica_In_Corso Then

                        modifica_assegnato = False
                    End If

                Next

                'Leggo la Data_Fine_Rendicontazione
                Dim Data_Fine As String = ""
                Leggi_Date_Ins_Rendicontazione("", Data_Fine, anno, tipo_azienda, piva, Richiesta_Cod, objParametri_Server)

                'Se siamo oltre la data_fine della rendicontazione per l'anno selezionato, blocco la modifica 
                If riga.Item("Avanzamento_Richiesta") = enum_UMA_Avanzamento.Rendicontazione AndAlso Today > Data_Fine Then
                    modifica_richiesta = False
                End If
            End If

            If (Avanzamento_Richiesta = enum_UMA_Avanzamento.Richiesta_Anticipo OrElse Avanzamento_Richiesta = enum_UMA_Avanzamento.Richiesta) AndAlso Not integrativa Then
                Dim annoFix As Integer = 0
                If anno <> "" Then
                    annoFix = CInt(anno)
                End If
                'Provo a leggere la rimanenza dalla rendicontazione dell'anno precedente
                Dim rimanenze_prec = check.Leggi_rimanenza_iniziale(piva, enum_UMA_Avanzamento.Rendicontazione, Tipo_Richiesta, annoFix - 1, objParametri_Server)

                If (rimanenze_prec.Rows.Count > 0) Then
                    Rimanenza_Gasolio_prec = rimanenze_prec.Rows(0)("Rimanenza_Gasolio")
                    Rimanenza_Benzina_prec = rimanenze_prec.Rows(0)("Rimanenza_Benzina")
                    Rimanenza_Gasolio_Serra_prec = rimanenze_prec.Rows(0)("Rimanenza_Gasolio_Serra")

                    If Rimanenza_Gasolio <> Rimanenza_Gasolio_prec Then
                        aggiornaRimanenze.Modifica_Campo_Richiesta("Rimanenza_Gasolio", Rimanenza_Gasolio_prec, piva, Richiesta_Cod, objParametri_Server)
                    End If
                    Rimanenza_Gasolio = Rimanenza_Gasolio_prec

                    If Rimanenza_Benzina <> Rimanenza_Benzina_prec Then
                        aggiornaRimanenze.Modifica_Campo_Richiesta("Rimanenza_Benzina", Rimanenza_Benzina_prec, piva, Richiesta_Cod, objParametri_Server)
                    End If
                    Rimanenza_Benzina = Rimanenza_Benzina_prec

                    If Rimanenza_Gasolio_Serra <> Rimanenza_Gasolio_Serra_prec Then
                        aggiornaRimanenze.Modifica_Campo_Richiesta("Rimanenza_Gasolio_Serra", Rimanenza_Gasolio_Serra_prec, piva, Richiesta_Cod, objParametri_Server)
                    End If
                    Rimanenza_Gasolio_Serra = Rimanenza_Gasolio_Serra_prec
                Else
                    Rimanenza_Gasolio_prec = Rimanenza_Gasolio
                    Rimanenza_Benzina_prec = Rimanenza_Benzina
                    Rimanenza_Gasolio_Serra_prec = Rimanenza_Gasolio_Serra
                End If

            Else

                'Provo a cercare i valori nella prima richiesta dell'anno corrente
                Dim rimanenzeDaPrimaRich = check.Leggi_rimanenza_iniziale(piva, enum_UMA_Avanzamento.Richiesta, Tipo_Richiesta, annoR, objParametri_Server)

                If rimanenzeDaPrimaRich.Rows.Count > 0 Then
                    Rimanenza_Gasolio_prec = rimanenzeDaPrimaRich.Rows(0)("Rimanenza_Gasolio")
                    Rimanenza_Benzina_prec = rimanenzeDaPrimaRich.Rows(0)("Rimanenza_Benzina")
                    Rimanenza_Gasolio_Serra_prec = rimanenzeDaPrimaRich.Rows(0)("Rimanenza_Gasolio_Serra")
                End If

            End If

            If (Avanzamento_Richiesta = enum_UMA_Avanzamento.Rendicontazione) Then

                If Tipo_Richiesta = enum_UMA_TipoRichiesta.Conto_Proprio Then

                    If anno <> "" Then

                        Dim anticipazioniTerzisti = check.RecuperaAnticipazioniColturaliTerzisti(piva, anno, objParametri_Server, False)

                        If anticipazioniTerzisti.Rows.Count > 0 Then

                            For Each row In anticipazioniTerzisti.Rows

                                Select Case CInt(row.Item("Tipo_Carburante"))
                                    Case enum_TipoCarburante_UMA.Gasolio
                                        Anticipazioni_Gasolio = CDbl(row.Item("Fabbisogno_Assegnato"))

                                    Case enum_TipoCarburante_UMA.Benzina
                                        Anticipazioni_Benzina = CDbl(row.Item("Fabbisogno_Assegnato"))

                                    Case enum_TipoCarburante_UMA.Gasolio_Serra
                                        Anticipazioni_Gasolio_Serra = CDbl(row.Item("Fabbisogno_Assegnato"))

                                    Case Else
                                        Anticipazioni_Altro = CDbl(row.Item("Fabbisogno_Assegnato"))

                                End Select

                            Next

                        End If

                    End If

                End If

            End If

            Dt.Columns.Add(New DataColumn("indDes", GetType(String)))
            Dt.Columns.Add(New DataColumn("comDes", GetType(String)))
            Dt.Columns.Add(New DataColumn("CUAA", GetType(String)))
            Dt.Columns.Add(New DataColumn("anno", GetType(String)))
            Dt.Columns.Add(New DataColumn("nDichiarazione", GetType(String)))
            Dt.Columns.Add(New DataColumn("cod", GetType(Integer)))
            Dt.Columns.Add(New DataColumn("benzina", GetType(Integer)))
            Dt.Columns.Add(New DataColumn("gasolio", GetType(Integer)))
            Dt.Columns.Add(New DataColumn("gasolioSerra", GetType(Integer)))
            Dt.Columns.Add(New DataColumn("stato_des", GetType(String)))
            Dt.Columns.Add(New DataColumn("stato_cod", GetType(Integer)))

            Dt.Columns.Add(New DataColumn("benzinaAppro", GetType(Integer)))
            Dt.Columns.Add(New DataColumn("gasolioAppro", GetType(Integer)))
            Dt.Columns.Add(New DataColumn("gasolioSerraAppro", GetType(Integer)))

            Dt.Columns.Add(New DataColumn("pratica_cod", GetType(Integer)))

            Dt.Columns.Add(New DataColumn("Gasolio_Tot", GetType(Double)))
            Dt.Columns.Add(New DataColumn("Benzina_Tot", GetType(Double)))
            Dt.Columns.Add(New DataColumn("Gasolio_Serra_Tot", GetType(Double)))

            Dt.Columns.Add(New DataColumn("Rimanenza_Gasolio", GetType(Double)))
            Dt.Columns.Add(New DataColumn("Rimanenza_Benzina", GetType(Double)))
            Dt.Columns.Add(New DataColumn("Rimanenza_Gasolio_Serra", GetType(Double)))

            Dt.Columns.Add(New DataColumn("Rimanenza_Gasolio_prec", GetType(Double)))
            Dt.Columns.Add(New DataColumn("Rimanenza_Benzina_prec", GetType(Double)))
            Dt.Columns.Add(New DataColumn("Rimanenza_Gasolio_Serra_prec", GetType(Double)))

            Dt.Columns.Add(New DataColumn("Anticipazioni_Gasolio", GetType(Double)))
            Dt.Columns.Add(New DataColumn("Anticipazioni_Benzina", GetType(Double)))
            Dt.Columns.Add(New DataColumn("Anticipazioni_Gasolio_Serra", GetType(Double)))

            Dt.Columns.Add(New DataColumn("modifica_richiesto", GetType(Boolean)))
            Dt.Columns.Add(New DataColumn("modifica_assegnato", GetType(Boolean)))

            Dt.Columns.Add(New DataColumn("Primo_Acquisto", GetType(Date)))

            Dt.Columns.Add(New DataColumn("Percentuale_Anticipo_Richiesta_da_Azienda", GetType(Double)))

            Dt.Columns.Add(New DataColumn("integrativa", GetType(Boolean)))
            Dt.Columns.Add(New DataColumn("cessata", GetType(Boolean)))

            Dt.Columns.Add(New DataColumn("permessoAcqua", GetType(Double)))
            Dt.Columns.Add(New DataColumn("notePermessoAcqua", GetType(String)))
            Dt.Columns.Add(New DataColumn("permessoAcquaGiaRich", GetType(String)))

            Dt.Columns.Add(New DataColumn("Acquisto_Gasolio", GetType(Double)))
            Dt.Columns.Add(New DataColumn("Acquisto_Benzina", GetType(Double)))
            Dt.Columns.Add(New DataColumn("Acquisto_Gasolio_Serra", GetType(Double)))

            Dt.Columns.Add(New DataColumn("Rim_Riass_Gasolio", GetType(Double)))
            Dt.Columns.Add(New DataColumn("Rim_Riass_Benzina", GetType(Double)))
            Dt.Columns.Add(New DataColumn("Rim_Riass_Gasolio_Serra", GetType(Double)))
            Dt.Columns.Add(New DataColumn("Rim_Riass_Conf_Gasolio", GetType(Double)))
            Dt.Columns.Add(New DataColumn("Rim_Riass_Conf_Benzina", GetType(Double)))
            Dt.Columns.Add(New DataColumn("Rim_Riass_Conf_Gasolio_Serra", GetType(Double)))

            Dt.Columns.Add(New DataColumn("Rec_Acc_Dich_Gasolio", GetType(Double)))
            Dt.Columns.Add(New DataColumn("Rec_Acc_Dich_Benzina", GetType(Double)))
            Dt.Columns.Add(New DataColumn("Rec_Acc_Dich_Gasolio_Serra", GetType(Double)))
            Dt.Columns.Add(New DataColumn("Rec_Acc_Conf_Gasolio", GetType(Double)))
            Dt.Columns.Add(New DataColumn("Rec_Acc_Conf_Benzina", GetType(Double)))
            Dt.Columns.Add(New DataColumn("Rec_Acc_Conf_Gasolio_Serra", GetType(Double)))
            Dt.Columns.Add(New DataColumn("Causale_Non_Utilizzo", GetType(String)))
            Dt.Columns.Add(New DataColumn("Rim_Dich_Gasolio", GetType(Double)))
            Dt.Columns.Add(New DataColumn("Rim_Dich_Benzina", GetType(Double)))
            Dt.Columns.Add(New DataColumn("Rim_Dich_Gasolio_Serra", GetType(Double)))
            Dt.Columns.Add(New DataColumn("Approvatore", GetType(String)))
            Dt.Columns.Add(New DataColumn("Allevati_Montagna", GetType(Boolean)))

            Dim paraGas As New FiltriDettaglioLtAcquistabili()
            Dim risstagas As RispostaStandard
            paraGas.Anno = annoR
            paraGas.Cuaa = CUAA

            Dim Acquisto_Gasolio As Integer = 0
            Dim Acquisto_Benzina As Integer = 0
            Dim Acquisto_Gasolio_Serra As Integer = 0

            paraGas.Tipo_Carburante = enum_TipoCarburante_UMA.Gasolio
            risstagas = TentativoDettaglioLtAcquistabili(paraGas, objParametri_Server)
            Dim AcqGasolio As Object = JsonConvert.DeserializeObject(risstagas.RispostaStringa)
            If Tipo_Richiesta = enum_UMA_TipoRichiesta.Conto_Proprio Then
                Acquisto_Gasolio = AcqGasolio("ltAcquistabiliProprio")
            ElseIf Tipo_Richiesta = enum_UMA_TipoRichiesta.Conto_Proprio Then
                Acquisto_Gasolio = AcqGasolio("ltAcquistabiliTerzi")
            End If

            paraGas.Tipo_Carburante = enum_TipoCarburante_UMA.Gasolio_Serra
            risstagas = TentativoDettaglioLtAcquistabili(paraGas, objParametri_Server)
            If Tipo_Richiesta = enum_UMA_TipoRichiesta.Conto_Proprio Then
                Acquisto_Gasolio_Serra = AcqGasolio("ltAcquistabiliProprio")
            ElseIf Tipo_Richiesta = enum_UMA_TipoRichiesta.Conto_Proprio Then
                Acquisto_Gasolio_Serra = AcqGasolio("ltAcquistabiliTerzi")
            End If

            paraGas.Tipo_Carburante = enum_TipoCarburante_UMA.Benzina
            risstagas = TentativoDettaglioLtAcquistabili(paraGas, objParametri_Server)
            If Tipo_Richiesta = enum_UMA_TipoRichiesta.Conto_Proprio Then
                Acquisto_Benzina = AcqGasolio("ltAcquistabiliProprio")
            ElseIf Tipo_Richiesta = enum_UMA_TipoRichiesta.Conto_Proprio Then
                Acquisto_Benzina = AcqGasolio("ltAcquistabiliTerzi")
            End If

            Dim d0 As DataRow
            d0 = Dt.NewRow
            d0("indDes") = ind_des
            d0("comDes") = com_des
            d0("CUAA") = CUAA
            d0("cod") = cod
            d0("anno") = anno
            d0("nDichiarazione") = nDichiarazione
            d0("benzina") = benzina
            d0("gasolio") = gasolio
            d0("gasolioSerra") = gasolioSerra

            d0("benzinaAppro") = benzinaAppro
            d0("gasolioAppro") = gasolioAppro
            d0("gasolioSerraAppro") = gasolioSerraAppro

            d0("stato_des") = stato_des
            d0("stato_cod") = stato_cod

            d0("Gasolio_Tot") = gasolioTot
            d0("Benzina_Tot") = benzinaTot
            d0("Gasolio_Serra_Tot") = gasolioSerraTot

            d0("Rimanenza_Gasolio") = Rimanenza_Gasolio
            d0("Rimanenza_Benzina") = Rimanenza_Benzina
            d0("Rimanenza_Gasolio_Serra") = Rimanenza_Gasolio_Serra

            d0("Rimanenza_Gasolio_prec") = Rimanenza_Gasolio_prec
            d0("Rimanenza_Benzina_prec") = Rimanenza_Benzina_prec
            d0("Rimanenza_Gasolio_Serra_prec") = Rimanenza_Gasolio_Serra_prec

            d0("Anticipazioni_Gasolio") = Math.Round(Anticipazioni_Gasolio, 0)
            d0("Anticipazioni_Benzina") = Math.Round(Anticipazioni_Benzina, 0)
            d0("Anticipazioni_Gasolio_Serra") = Math.Round(Anticipazioni_Gasolio_Serra, 0)

            d0("modifica_richiesto") = modifica_richiesta
            d0("modifica_assegnato") = modifica_assegnato

            d0("Primo_Acquisto") = dataPrimoAcquisto
            d0("Percentuale_Anticipo_Richiesta_da_Azienda") = aliquota_anticipo

            d0("Integrativa") = integrativa
            d0("cessata") = cessata

            d0("permessoAcqua") = permessoAcqua
            d0("notePermessoAcqua") = notePermessoAcqua
            d0("permessoAcquaGiaRich") = permessoAcquaGiaRich

            d0("Acquisto_Gasolio") = Acquisto_Gasolio
            d0("Acquisto_Benzina") = Acquisto_Benzina
            d0("Acquisto_Gasolio_Serra") = Acquisto_Gasolio_Serra

            d0("rim_riass_gasolio") = rim_riass_gasolio
            d0("rim_riass_benzina") = rim_riass_benzina
            d0("rim_riass_gasolio_serra") = rim_riass_gasolio_serra

            d0("rim_riass_conf_gasolio") = rim_riass_conf_gasolio
            d0("rim_riass_conf_benzina") = rim_riass_conf_benzina
            d0("rim_riass_conf_gasolio_serra") = rim_riass_conf_gasolio_serra

            d0("rec_acc_dich_gasolio") = rec_acc_dich_gasolio
            d0("rec_acc_dich_benzina") = rec_acc_dich_benzina
            d0("rec_acc_dich_gasolio_serra") = rec_acc_dich_gasolio_serra

            d0("rec_acc_conf_gasolio") = rec_acc_conf_gasolio
            d0("rec_acc_conf_benzina") = rec_acc_conf_benzina
            d0("rec_acc_conf_gasolio_serra") = rec_acc_conf_gasolio_serra

            d0("Causale_Non_Utilizzo") = Causale_Non_Utilizzo

            d0("Rim_Dich_Gasolio") = Rim_Dich_Gasolio
            d0("Rim_Dich_Benzina") = Rim_Dich_Benzina
            d0("Rim_Dich_Gasolio_Serra") = Rim_Dich_Gasolio_Serra
            d0("Approvatore") = approvatore
            d0("Allevati_Montagna") = Allevati_Montagna

            d0("Allevati_Montagna") = Allevati_Montagna

            Dt.Rows.Add(d0)

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            r.RispostaStringa = JsonConvert.SerializeObject(Dt, Formatting.None, serializerSettings)

            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
            AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function
    Private Shared Function TentativoDettaglioLtAcquistabili(params As FiltriDettaglioLtAcquistabili, objParametri_Server As AgronicaCoreParametri) As RispostaStandard

        Dim servizio As New Vendita_Carburanti(objParametri_Server, HttpContext.Current.Session("ASG_objParametri_Utenti"))
        Dim r As RispostaStandard = servizio.DettaglioLtAcquistabili(params)
        Return r
        'Dim r As RispostaStandard = EseguireOperazione(AddressOf servizio.DettaglioLtAcquistabili, parametri)
    End Function
    '<WebMethod(EnableSession:=True)>
    'Public Shared Function Trova_Macrousi_Superfici(ByVal piva As String, ByVal anno As Integer) As RispostaStandard

    '    'recupero una lista con tutti i macrousi abilitati su quella determinata piva insieme a tutte le informazioni relative sulle superfici
    '    Dim testata As New AgronicaCoreAnagrafeDAL.Programmazione_Testata_R
    '    Dim entita As New AgronicaCoreAnagrafeDAL.Programmazione_Entita_R
    '    Dim dataFit As Date = #1/1/2000 12:00 PM#
    '    Dim r As New RispostaStandard
    '    Dim DtSup As New DataTable
    '    Dim DtTess As New DataTable
    '    Dim Dtres As New DataTable
    '    Dim DtMacro As New DataTable
    '    Dim macroDict As New Dictionary(Of String, String)
    '    Dim exp As String
    '    Dim exp2 As String
    '    Dim progr_cod As Integer
    '    Dim dr As DataRow()
    '    Dim dr2 As DataRow()
    '    Dim DtEntita As DataTable
    '    Lingua.Gias_InizializzaCultura_DaSession()

    '    Dim serializerSettings As New JsonSerializerSettings()
    '    serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore

    '    Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
    '    Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")

    '    If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
    '        r.Sessione = False
    '        Return r
    '    End If

    '    Dtres.Columns.Add(New DataColumn("macrouso_UMA_Cod", GetType(String)))
    '    Dtres.Columns.Add(New DataColumn("macrouso_UMA_Des", GetType(String)))
    '    Dtres.Columns.Add(New DataColumn("sup_A", GetType(Decimal)))
    '    Dtres.Columns.Add(New DataColumn("sup_B", GetType(Decimal)))
    '    Dtres.Columns.Add(New DataColumn("tessitura_Norm", GetType(Decimal)))
    '    Dtres.Columns.Add(New DataColumn("tessitura_Media", GetType(Decimal)))
    '    Dtres.Columns.Add(New DataColumn("tessitura_Tenace", GetType(Decimal)))
    '    Dtres.Columns.Add(New DataColumn("Programmazione_Cod", GetType(Decimal)))

    '    Try

    '        DtMacro = testata.Leggi("", 0, piva, "", 0, #1/1/2000 12:00 PM#, DateTime.Now, enum_TipoRicetta.Non_Filtrare, enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
    '                                "", "Data_creazione DESC", objParametri_Server)

    '        If (DtMacro.Rows.Count > 0) Then

    '            progr_cod = DtMacro.Rows.Item(0).Item("Programmazione_Cod")
    '            DtEntita = entita.Programmazione_Entita_Leggi(progr_cod, "", 0, "", piva, 0, 0, 0, 0, 0,
    '                                                      dataFit, DateTime.Now, 0, enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "Macrouso_Cod, Occupazione_Cod_Agea, Cul_Cod_Agea", objParametri_Server)
    '            DtSup = entita.Programmazione_Entita_Leggi_per_UMA_DT_Pendenza(progr_cod, objParametri_Server, xOrderBy:="Macrouso_UMA_Des", anno:=anno)
    '            DtTess = entita.Programmazione_Entita_Leggi_per_UMA_DT_Tessitura(progr_cod, objParametri_Server, anno:=anno)

    '            Dim DtEntitaDistinct = DtEntita.DefaultView.ToTable(False, "Macrouso_Cod", "Occupazione_Cod_Agea", "Cul_Cod_Agea", "Veg_Cod", "Id_Cod", "Superficie")

    '            Dim UMAGroups = DtEntitaDistinct.AsEnumerable().
    '                GroupBy(Function(row) New With {
    '                    Key .Macrouso_Cod = row.Item("Macrouso_Cod"),
    '                    Key .Occupazione_Cod_Agea = row.Item("Occupazione_Cod_Agea"),
    '                    Key .Cul_Cod_Agea = row.Item("Cul_Cod_Agea"),
    '                    Key .Veg_Cod = row.Item("Veg_Cod"),
    '                    Key .Id_Cod = row.Item("Id_Cod")
    '                })

    '            Dim tableResult = DtEntitaDistinct.Clone()
    '            For Each grp In UMAGroups
    '                tableResult.Rows.Add(grp.Key.Macrouso_Cod, grp.Key.Occupazione_Cod_Agea, grp.Key.Cul_Cod_Agea, grp.Key.Veg_Cod,
    '                                     grp.Key.Id_Cod, grp.Sum(Function(row) CType(row.Item("Superficie"), Decimal)))
    '            Next

    '            For Each row As DataRow In DtSup.Rows

    '                If (macroDict.ContainsKey(row.Item("macrouso_UMA_Cod"))) Then

    '                    exp = "Macrouso_UMA_Cod = '" + row.Item("Macrouso_UMA_Cod") + "' "
    '                    dr = Dtres.Select(exp)
    '                    If (dr.Count > 0) Then
    '                        'se era già presente un record con quel macrouso, sommo le superfici
    '                        dr.First.Item("Sup_A") = dr.First.Item("Sup_A") + row.Item("sup_A")
    '                        dr.First.Item("Sup_B") = dr.First.Item("Sup_B") + row.Item("sup_B")

    '                        exp2 = "Macrouso_UMA_Cod = '" + row.Item("Macrouso_UMA_Cod") + "' AND Occupazione_Cod_Agea = '" + row.Item("Occupazione_Cod_Agea").ToString +
    '                          "' AND Veg_Cod = '" + row.Item("veg_Cod").ToString + "' AND Cul_Cod_Agea = '" + row.Item("Cul_Cod_Agea").ToString + "' AND Macrouso_Cod = '" +
    '                          row.Item("Macrouso_Cod").ToString + "' "
    '                        dr2 = DtTess.Select(exp2)

    '                        If (dr2.Count > 0) Then

    '                            dr.First.Item("tessitura_Norm") = dr.First.Item("tessitura_Norm") + dr2.First.Item("sup_Normale")
    '                            dr.First.Item("tessitura_Media") = dr.First.Item("tessitura_Media") + dr2.First.Item("sup_Media")
    '                            dr.First.Item("tessitura_Tenace") = dr.First.Item("tessitura_Tenace") + dr2.First.Item("sup_Tenace")

    '                        End If

    '                    End If

    '                Else

    '                    exp2 = "Macrouso_UMA_Cod = '" + row.Item("Macrouso_UMA_Cod") + "' AND Occupazione_Cod_Agea = '" + row.Item("Occupazione_Cod_Agea").ToString +
    '                          "' AND Veg_Cod = '" + row.Item("veg_Cod").ToString + "' AND Cul_Cod_Agea = '" + row.Item("Cul_Cod_Agea").ToString + "' AND Macrouso_Cod = '" +
    '                          row.Item("Macrouso_Cod").ToString + "' "
    '                    dr2 = DtTess.Select(exp2)

    '                    Dim dtgroupRow = Dtres.NewRow
    '                    dtgroupRow.Item("Programmazione_Cod") = progr_cod
    '                    dtgroupRow.Item("macrouso_UMA_Cod") = row.Item("macrouso_UMA_Cod")
    '                    dtgroupRow.Item("macrouso_UMA_Des") = row.Item("macrouso_UMA_Des")
    '                    dtgroupRow.Item("sup_A") = row.Item("sup_A")
    '                    dtgroupRow.Item("sup_B") = row.Item("sup_B")

    '                    If (dr2.Count > 0) Then

    '                        dtgroupRow.Item("tessitura_Norm") = dr2.First.Item("sup_Normale")
    '                        dtgroupRow.Item("tessitura_Media") = dr2.First.Item("sup_Media")
    '                        dtgroupRow.Item("tessitura_Tenace") = dr2.First.Item("sup_Tenace")

    '                    Else

    '                        dtgroupRow.Item("tessitura_Norm") = 0
    '                        dtgroupRow.Item("tessitura_Media") = 0
    '                        dtgroupRow.Item("tessitura_Tenace") = 0

    '                    End If

    '                    Dtres.Rows.Add(dtgroupRow)
    '                    macroDict.Add(row.Item("macrouso_UMA_Cod"), row.Item("macrouso_UMA_Des"))

    '                End If
    '            Next

    '            r.RispostaStringa = JsonConvert.SerializeObject(Dtres, Formatting.None, serializerSettings)
    '            r.RispostaOK = True

    '        Else

    '            r.RispostaOK = False

    '        End If


    '    Catch ex As Exception

    '        r.RispostaOK = False
    '        r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

    '    End Try

    '    Return r

    'End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Trova_Macrousi_Superfici_Fascicolo(ByVal piva As String, Programmazione_Cod As Integer,
                                                              ByVal anno As Integer, ByVal richiesta_Cod As Integer,
                                                              ByVal avanzamento As Integer, ByVal terzista As Integer) As RispostaStandard

        'recupero una lista con tutti i macrousi abilitati su quella determinata piva insieme a tutte le informazioni relative sulle superfici
        Dim testata As New AgronicaCoreAnagrafeDAL.Programmazione_Testata_R
        Dim entita As New AgronicaCoreAnagrafeDAL.Programmazione_Entita_R
        Dim dataFit As Date = #1/1/2000 12:00 PM#
        Dim r As New RispostaStandard
        Dim DtSup As New DataTable
        Dim DtTess As New DataTable
        Dim Dtres As New DataTable
        Dim DtMacro As New DataTable
        Dim macroDict As New Dictionary(Of String, String)
        Dim exp As String
        Dim exp2 As String
        Dim expEnt As String
        Dim progr_cod As Integer
        Dim dr As DataRow()
        Dim dr2 As DataRow()
        Dim drEnt As DataRow()
        Dim DtEntita As DataTable

        Dim serializerSettings As New JsonSerializerSettings()
        serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        Dim objParametri_Super_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Super_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Super_Server) Then
            r.Sessione = False
            Return r
        End If

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Utenti) Then
            r.Sessione = False
            Return r
        End If

        Lingua.Gias_InizializzaCultura_DaSession()

        Dtres.Columns.Add(New DataColumn("Macrouso_UMA_Cod", GetType(String)))
        Dtres.Columns.Add(New DataColumn("Macrouso_UMA_Des", GetType(String)))
        Dtres.Columns.Add(New DataColumn("sup_tot", GetType(Decimal)))
        Dtres.Columns.Add(New DataColumn("sup_A", GetType(Decimal)))
        Dtres.Columns.Add(New DataColumn("sup_B", GetType(Decimal)))
        Dtres.Columns.Add(New DataColumn("tessitura_Norm", GetType(Decimal)))
        Dtres.Columns.Add(New DataColumn("tessitura_Media", GetType(Decimal)))
        Dtres.Columns.Add(New DataColumn("tessitura_Tenace", GetType(Decimal)))
        Dtres.Columns.Add(New DataColumn("Programmazione_Cod", GetType(Decimal)))

        If anno >= 2025 AndAlso Programmazione_Cod = -4 Then

            Dim res = Trova_Macrousi_Superfici_Fascicolo_2025(piva, richiesta_Cod, avanzamento, terzista, Dtres, objParametri_Server, objParametri_Super_Server)

            If res.Item2 = "" Then

                r.RispostaStringa = JsonConvert.SerializeObject(res.Item1, Formatting.None, serializerSettings)
                r.RispostaOK = True

            Else

                r.RispostaOK = False
                r.RispostaStringa = ""
                r.Errore = "Nessuna coltura situata in Umbria trovata"

            End If

        Else

            Try

                DtMacro = testata.Leggi("", Programmazione_Cod, piva, "", 0, #1/1/2000 12:00 PM#, DateTime.Now, enum_TipoRicetta.Non_Filtrare, enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                    "", "Data_creazione DESC", objParametri_Server)

                If (DtMacro.Rows.Count > 0) Then

                    progr_cod = DtMacro.Rows.Item(0).Item("Programmazione_Cod")
                    DtEntita = entita.Programmazione_Entita_Leggi(progr_cod, "", 0, "", piva, 0, 0, 0, 0, 0,
                                                          dataFit, DateTime.Now, 0, enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                                          "EXISTS (SELECT TOP 1 * FROM Programmazione_Particelle WHERE Programmazione_Particelle.Programmazione_Entita_Cod = Programmazione_entita.Programmazione_Entita_Cod AND Programmazione_Particelle.prov in ('054', '055'))",
                                                          "Macrouso_Cod, Occupazione_Cod_Agea, Cul_Cod_Agea", objParametri_Server, Lettura_Per_UMA:=True)
                    DtSup = entita.Programmazione_Entita_Leggi_per_UMA_DT_Pendenza(progr_cod, objParametri_Server, xOrderBy:="Macrouso_UMA_Des", anno:=anno)
                    DtTess = entita.Programmazione_Entita_Leggi_per_UMA_DT_Tessitura(progr_cod, objParametri_Server, anno:=anno, richiesta_Cod:=richiesta_Cod, avanzamento:=avanzamento, terzista:=terzista)

                    Dim DtEntitaDistinct = DtEntita.DefaultView.ToTable(False, "Macrouso_Cod", "Occupazione_Cod_Agea", "Cul_Cod_Agea", "Veg_Cod", "Id_Cod", "Destinazione_Cod_Agea", "Superficie")

                    Dim UMAGroups = DtEntitaDistinct.AsEnumerable().
                    GroupBy(Function(row) New With {
                        Key .Macrouso_Cod = row.Item("Macrouso_Cod"),
                        Key .Occupazione_Cod_Agea = row.Item("Occupazione_Cod_Agea"),
                        Key .Cul_Cod_Agea = row.Item("Cul_Cod_Agea"),
                        Key .Veg_Cod = row.Item("Veg_Cod"),
                        Key .Id_Cod = row.Item("Id_Cod"),
                        Key .Destinazione_Cod_Agea = row.Item("Destinazione_Cod_Agea")
                    })

                    If UMAGroups.Count > 0 Then


                        Dim tableResult = DtEntitaDistinct.Clone()
                        For Each grp In UMAGroups
                            tableResult.Rows.Add(grp.Key.Macrouso_Cod, grp.Key.Occupazione_Cod_Agea, grp.Key.Cul_Cod_Agea, grp.Key.Veg_Cod,
                                         grp.Key.Id_Cod, grp.Key.Destinazione_Cod_Agea, grp.Sum(Function(row) CType(row.Item("Superficie"), Decimal)))
                        Next

                        DtSup = DtSup.Select(" macrouso_UMA_Cod <> '' ").CopyToDataTable

                        For Each row As DataRow In DtSup.Rows

                            If (macroDict.ContainsKey(row.Item("macrouso_UMA_Cod"))) Then

                                exp = "Macrouso_UMA_Cod = '" & row.Item("Macrouso_UMA_Cod") & "' "
                                dr = Dtres.Select(exp)
                                If (dr.Count > 0) Then
                                    'se era già presente un record con quel macrouso, sommo le superfici
                                    dr.First.Item("Sup_A") = dr.First.Item("Sup_A") + row.Item("sup_A")
                                    dr.First.Item("Sup_B") = dr.First.Item("Sup_B") + row.Item("sup_B")

                                    exp2 = "Macrouso_UMA_Cod = '" & row.Item("Macrouso_UMA_Cod") & "' AND Occupazione_Cod_Agea = '" & row.Item("Occupazione_Cod_Agea").ToString &
                              "' AND Veg_Cod = '" & row.Item("veg_Cod").ToString & "' AND Id_Cod = '" & row.Item("Id_Cod").ToString & "' AND Cul_Cod_Agea = '" & row.Item("Cul_Cod_Agea").ToString & "' AND Macrouso_Cod = '" &
                              row.Item("Macrouso_Cod").ToString & "' AND Destinazione_Cod_Agea = '" & row.Item("Destinazione_Cod_Agea").ToString & "' "
                                    dr2 = DtTess.Select(exp2)

                                    expEnt = "Occupazione_Cod_Agea = '" & row.Item("Occupazione_Cod_Agea").ToString & "' AND Destinazione_Cod_Agea = '" & row.Item("Destinazione_Cod_Agea").ToString &
                              "' AND Veg_Cod = '" & row.Item("veg_Cod").ToString & "' AND Id_Cod = '" & row.Item("Id_Cod").ToString &
                              "' AND Cul_Cod_Agea = '" & row.Item("Cul_Cod_Agea").ToString & "' AND Macrouso_Cod = '" & row.Item("Macrouso_Cod").ToString & "' "
                                    drEnt = tableResult.Select(expEnt)

                                    If (dr2.Count > 0) Then

                                        dr.First.Item("tessitura_Norm") = dr.First.Item("tessitura_Norm") + dr2.First.Item("sup_Normale")
                                        dr.First.Item("tessitura_Media") = dr.First.Item("tessitura_Media") + dr2.First.Item("sup_Media")
                                        dr.First.Item("tessitura_Tenace") = dr.First.Item("tessitura_Tenace") + dr2.First.Item("sup_Tenace")

                                    End If

                                    If (drEnt.Count > 0) Then
                                        dr.First.Item("sup_tot") = dr.First.Item("sup_tot") + drEnt.First.Item("Superficie")
                                    End If

                                End If

                            Else

                                exp2 = "Macrouso_UMA_Cod = '" & row.Item("Macrouso_UMA_Cod") & "' AND Occupazione_Cod_Agea = '" & row.Item("Occupazione_Cod_Agea").ToString &
                              "' AND Veg_Cod = '" & row.Item("veg_Cod").ToString & "' AND Id_Cod = '" & row.Item("Id_Cod").ToString & "' AND Destinazione_Cod_Agea = '" & row.Item("Destinazione_Cod_Agea").ToString &
                              "' AND Cul_Cod_Agea = '" & row.Item("Cul_Cod_Agea").ToString & "' AND Macrouso_Cod = '" & row.Item("Macrouso_Cod").ToString & "' "
                                dr2 = DtTess.Select(exp2)

                                expEnt = "Occupazione_Cod_Agea = '" & row.Item("Occupazione_Cod_Agea").ToString & "' AND Destinazione_Cod_Agea = '" & row.Item("Destinazione_Cod_Agea").ToString &
                              "' AND Veg_Cod = '" & row.Item("veg_Cod").ToString & "' AND Id_Cod = '" & row.Item("Id_Cod").ToString &
                              "' AND Cul_Cod_Agea = '" & row.Item("Cul_Cod_Agea").ToString & "' AND Macrouso_Cod = '" & row.Item("Macrouso_Cod").ToString & "' "
                                drEnt = tableResult.Select(expEnt)

                                Dim dtgroupRow = Dtres.NewRow
                                dtgroupRow.Item("Programmazione_Cod") = progr_cod
                                dtgroupRow.Item("macrouso_UMA_Cod") = row.Item("macrouso_UMA_Cod")
                                dtgroupRow.Item("macrouso_UMA_Des") = row.Item("macrouso_UMA_Des")
                                dtgroupRow.Item("sup_A") = row.Item("sup_A")
                                dtgroupRow.Item("sup_B") = row.Item("sup_B")
                                If drEnt.Count = 0 Then
                                    dtgroupRow.Item("sup_tot") = row.Item("sup_A") + row.Item("sup_B")
                                Else
                                    dtgroupRow.Item("sup_tot") = drEnt.First.Item("Superficie")
                                End If


                                If (dr2.Count > 0) Then

                                    dtgroupRow.Item("tessitura_Norm") = dr2.First.Item("sup_Normale")
                                    dtgroupRow.Item("tessitura_Media") = dr2.First.Item("sup_Media")
                                    dtgroupRow.Item("tessitura_Tenace") = dr2.First.Item("sup_Tenace")

                                Else

                                    dtgroupRow.Item("tessitura_Norm") = 0
                                    dtgroupRow.Item("tessitura_Media") = 0
                                    dtgroupRow.Item("tessitura_Tenace") = 0

                                End If

                                If (dtgroupRow.Item("tessitura_Norm") = 0 AndAlso
                            dtgroupRow.Item("tessitura_Media") = 0 AndAlso
                            dtgroupRow.Item("tessitura_Tenace") = 0) Then

                                    dtgroupRow.Item("tessitura_Norm") = drEnt.First.Item("Superficie")

                                End If

                                If (dtgroupRow.Item("sup_A") = 0 AndAlso
                            dtgroupRow.Item("sup_B") = 0) Then
                                    dtgroupRow.Item("sup_A") = drEnt.First.Item("Superficie")
                                End If

                                Dtres.Rows.Add(dtgroupRow)
                                macroDict.Add(row.Item("macrouso_UMA_Cod"), row.Item("macrouso_UMA_Des"))

                            End If
                        Next

                        r.RispostaStringa = JsonConvert.SerializeObject(Dtres, Formatting.None, serializerSettings)
                        r.RispostaOK = True


                    Else

                        r.RispostaOK = False
                        r.RispostaStringa = ""
                        r.Errore = "Nessuna coltura situata in Umbria trovata"

                    End If

                Else

                    r.RispostaOK = False
                    r.RispostaStringa = ""
                    r.Errore = "Nessuna coltura situata in Umbria trovata"

                End If


            Catch ex As Exception

                r.RispostaOK = False
                r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

            End Try

        End If

        Return r
    End Function

    Public Shared Function Trova_Macrousi_Superfici_Fascicolo_2025(piva As String, richiesta_Cod As Integer, avanzamento As Integer, terzista As Integer,
                                                        Dtres As DataTable,
                                                        ByRef objParametri_Server As AgronicaCoreParametri,
                                                        ByRef objParametri_Super_Server As AgronicaCoreParametri) As Tuple(Of DataTable, String)

        Dim dtColture As DataTable
        Dim richieste As New AgronicaCoreUmaDal.UMA_Richieste_R
        Dim richiestexRegImpianti As New AgronicaCoreUmaDal.UMA_RichiesteXReg_Impianti_W
        Dim errore As String = ""
        Dim res As Tuple(Of DataTable, String)
        Dim EFrichieste As New AgronicaCoreUmaBiz.UMA_Richieste
        Dim dtRichieste As New DataTable
        Dim testate As New AgronicaCoreUmaDal.UMA_Richieste_Testata_R
        Dim dtgroup As New DataTable
        Dim d0 As DataRow
        'Dim anno = 2025

        dtgroup.Columns.Add(New DataColumn("macrouso_UMA_Cod", GetType(String)))
        dtgroup.Columns.Add(New DataColumn("macrouso_UMA_Des", GetType(String)))
        dtgroup.Columns.Add(New DataColumn("Programmazione_Cod", GetType(Integer)))
        dtgroup.Columns.Add(New DataColumn("Regolamento_Cod", GetType(Integer)))
        dtgroup.Columns.Add(New DataColumn("sup_UMA", GetType(Decimal)))
        dtgroup.Columns.Add(New DataColumn("sup_A", GetType(Decimal)))
        dtgroup.Columns.Add(New DataColumn("sup_B", GetType(Decimal)))
        dtgroup.Columns.Add(New DataColumn("sup_Normale", GetType(Decimal)))
        dtgroup.Columns.Add(New DataColumn("sup_Media", GetType(Decimal)))
        dtgroup.Columns.Add(New DataColumn("sup_Tenace", GetType(Decimal)))
        dtgroup.Columns.Add(New DataColumn("calcolato", GetType(Decimal)))
        dtgroup.Columns.Add(New DataColumn("richiesto", GetType(Decimal)))
        dtgroup.Columns.Add(New DataColumn("assegnato", GetType(Decimal)))
        dtgroup.Columns.Add(New DataColumn("Veg_Cod", GetType(String)))
        dtgroup.Columns.Add(New DataColumn("Id_Cod", GetType(String)))
        dtgroup.Columns.Add(New DataColumn("sup_UMA_Edit", GetType(Decimal)))
        dtgroup.Columns.Add(New DataColumn("sup_UMA_A_Edit", GetType(Decimal)))
        dtgroup.Columns.Add(New DataColumn("sup_UMA_B_Edit", GetType(Decimal)))
        dtgroup.Columns.Add(New DataColumn("TerrenoNormale_Edit", GetType(Decimal)))
        dtgroup.Columns.Add(New DataColumn("TerrenoMedio_Edit", GetType(Decimal)))
        dtgroup.Columns.Add(New DataColumn("TerrenoTenace_Edit", GetType(Decimal)))
        dtgroup.Columns.Add(New DataColumn("Programmazione_Des", GetType(String)))

        Dim legamiRichiesteAppezzamento As DataTable = richieste.EstraiLegamiRichiestaxAppezzamento(piva, richiesta_Cod, "", objParametri_Server)

        If legamiRichiesteAppezzamento.Rows.Count > 0 Then

            'richiestexRegImpianti.InserisciAssociazioniPerNuovaPraticaContoProprio(legamiRichiesteAppezzamento, richiesta_Cod, objParametri_Server)

            dtColture = richieste.TrovaColtureAppezzamentiSenzaFascicolo(piva, richiesta_Cod, objParametri_Server)

            If dtColture.Rows.Count > 0 Then

                Dim objAnalisi As New AgronicaCoreAnagrafeBIZ.Analisi_Modello_R
                objAnalisi.CalcolaTessiturexUMA(dtColture, richiesta_Cod, objParametri_Server, objParametri_Super_Server)

                Dim DtEntitaDistinct = dtColture.DefaultView.ToTable(False, "Macrouso_UMA_Cod", "Macrouso_UMA_Des", "REGOLAMENTO", "SUP_APP", "Sup_A", "Sup_B", "Sup_Normale", "Sup_Media", "Sup_Tenace")

                Dim UMAGroups = DtEntitaDistinct.AsEnumerable().
                GroupBy(Function(row) New With {
                    Key .Macrouso_Cod = row.Item("Macrouso_UMA_Cod"),
                    Key .Macrouso_Des = CStr(row.Item("Macrouso_UMA_Des")).Trim,
                    Key .Regolamento = row.Item("REGOLAMENTO")
                })

                Dim tableResult = DtEntitaDistinct.Clone()
                For Each grp In UMAGroups
                    tableResult.Rows.Add(grp.Key.Macrouso_Cod, grp.Key.Macrouso_Des, grp.Key.Regolamento, grp.Sum(Function(row) CType(row.Item("SUP_APP"), Decimal)),
                grp.Sum(Function(row) CType(row.Item("Sup_A"), Decimal)), grp.Sum(Function(row) CType(row.Item("Sup_B"), Decimal)), grp.Sum(Function(row) CType(row.Item("Sup_Normale"), Decimal)),
                grp.Sum(Function(row) CType(row.Item("Sup_Media"), Decimal)), grp.Sum(Function(row) CType(row.Item("Sup_Tenace"), Decimal)))
                Next

                For Each row As DataRow In tableResult.Rows

                    Dim dtgroupRow = dtgroup.NewRow
                    dtgroupRow.Item("Programmazione_Cod") = CodiceFascicoloPianoColturaleGrafico
                    dtgroupRow.Item("macrouso_UMA_Cod") = row.Item("macrouso_UMA_Cod")
                    dtgroupRow.Item("macrouso_UMA_Des") = row.Item("macrouso_UMA_Des")
                    dtgroupRow.Item("Programmazione_Des") = DescrizioneFascicoloPianoColturaleGrafico
                    dtgroupRow.Item("Veg_Cod") = "000"
                    dtgroupRow.Item("Id_Cod") = "000"
                    dtgroupRow.Item("sup_A") = row.Item("sup_A")
                    dtgroupRow.Item("sup_B") = row.Item("sup_B")
                    dtgroupRow.Item("sup_UMA_A_Edit") = row.Item("sup_A")
                    dtgroupRow.Item("sup_UMA_B_Edit") = row.Item("sup_B")
                    dtgroupRow.Item("calcolato") = 0
                    dtgroupRow.Item("richiesto") = 0
                    dtgroupRow.Item("assegnato") = 0

                    If row.Item("sup_A") + row.Item("sup_B") <> row.Item("SUP_APP") Then
                        dtgroupRow.Item("sup_UMA") = row.Item("sup_A") + row.Item("sup_B")
                        dtgroupRow.Item("sup_UMA_Edit") = row.Item("sup_A") + row.Item("sup_B")
                    Else
                        dtgroupRow.Item("sup_UMA") = row.Item("SUP_APP")
                        dtgroupRow.Item("sup_UMA_Edit") = row.Item("SUP_APP")
                    End If

                    dtgroupRow.Item("sup_Normale") = row.Item("Sup_Normale")
                    dtgroupRow.Item("sup_Media") = row.Item("Sup_Media")
                    dtgroupRow.Item("sup_Tenace") = row.Item("Sup_Tenace")

                    dtgroupRow.Item("TerrenoNormale_Edit") = row.Item("Sup_Normale")
                    dtgroupRow.Item("TerrenoMedio_Edit") = row.Item("Sup_Media")
                    dtgroupRow.Item("TerrenoTenace_Edit") = row.Item("Sup_Tenace")

                    If (dtgroupRow.Item("Sup_Normale") = 0 AndAlso
                    dtgroupRow.Item("Sup_Media") = 0 AndAlso
                    dtgroupRow.Item("Sup_Tenace") = 0) Then

                        dtgroupRow.Item("Sup_Normale") = row.Item("SUP_APP")
                        dtgroupRow.Item("TerrenoNormale_Edit") = row.Item("SUP_APP")
                    End If

                    If (dtgroupRow.Item("sup_A") = 0 AndAlso
                    dtgroupRow.Item("sup_B") = 0) Then
                        dtgroupRow.Item("sup_A") = row.Item("SUP_APP")
                    End If

                    dtgroupRow.Item("Regolamento_Cod") = row.Item("Regolamento")
                    dtgroup.Rows.Add(dtgroupRow)

                Next


                For Each row In dtgroup.Rows

                    d0 = Dtres.NewRow
                    d0("Macrouso_UMA_Des") = row.Item("Macrouso_UMA_Des")
                    d0("Macrouso_UMA_Cod") = row.Item("Macrouso_UMA_Cod")
                    d0("Programmazione_Cod") = row.Item("Programmazione_Cod")
                    d0("sup_tot") = row.Item("sup_UMA")
                    d0("sup_A") = row.Item("sup_A")
                    d0("sup_B") = row.Item("sup_B")
                    d0("tessitura_Norm") = row.Item("sup_Normale")
                    d0("tessitura_Media") = row.Item("sup_Media")
                    d0("tessitura_Tenace") = row.Item("sup_Tenace")
                    Dtres.Rows.Add(d0)

                Next

                If errore <> "" Then

                    res = New Tuple(Of DataTable, String)(New DataTable, errore)

                    Return res
                End If

            End If

        Else

            res = New Tuple(Of DataTable, String)(New DataTable, "Non sono stati trovati impianti validi per questa azienda.")

            Return res

        End If

        Dtres.DefaultView.Sort = "Macrouso_UMA_Des ASC "
        Dtres = Dtres.DefaultView.ToTable

        res = New Tuple(Of DataTable, String)(Dtres, "")

        Return res
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Nuova_Richiesta(ByVal piva As String,
                                           ByVal cuaa As String,
                                           ByVal isTerzista As Boolean,
                                           ByVal anno As Integer,
                                           ByVal avanzamento As Integer,
                                           ByVal recuperaRimanenze As Boolean,
                                           ByVal integrativa As Integer,
                                           ByVal anticipo As Integer,
                                           ByVal percentuale_anticipo_carb As Double,
                                           ByVal tipo_azienda As Integer,
                                           ByVal anticipoGasolio As Double,
                                           ByVal anticipoBenzina As Double,
                                           ByVal anticipoGasolioSerra As Double) As RispostaStandard
        Dim r As New RispostaStandard
        Dim pratica_cod As Integer
        Dim pratica_num As Integer = 1
        Dim Dt As New DataTable

        Dim serializerSettings As New JsonSerializerSettings()
        serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Utenti) Then
            r.Sessione = False
            Return r
        End If

        Lingua.Gias_InizializzaCultura_DaSession()

        Try

            Dim praticheW As New AgronicaCoreProfilazioneBIZ.Pratiche_W
            Dim progr As New Sequenza_Progressivi_R
            Dim testate As New AgronicaCoreUmaBiz.UMA_Richieste
            Dim testate_R As New AgronicaCoreUmaDal.UMA_Richieste_Testata_R
            pratica_num = progr.Nuovo_Progressivo_UpdateImmediato(0, anno, IIf(avanzamento = 0, enum_SequenzaProgressiviTipi.Pratiche_UMA_Richieste, enum_SequenzaProgressiviTipi.Pratiche_UMA_Rendicontazioni),
                                                                  "", "", 0, objParametri_Server)
            Dim Data_Inizio = New Date(Date.Now.Year, 1, 1)
            Dim Data_Fine = New Date(Date.Now.Year, 12, 31)

            pratica_cod = 0
            Dim statoFinaleRichiesto As Integer = IIf(anticipo <> 1, enum_WAnagraficaStati.In_Compilazione, enum_WAnagraficaStati.Verifica_Intermedia_Completata_Con_Successo)

            praticheW.impostaPratica(0, piva, cuaa, objParametri_Server.UtenteUsername, enum_Servizi.Gestione_UMA,
                                     statoFinaleRichiesto,
                                     objParametri_Server,
                                     objParametri_Utenti,
                                     anno,
                                     pratica_num,
                                     pratica_cod,
                                     True,
                                     "",
                                     Data_Inizio,
                                     Data_Fine, 0, 0, -1)

            Dim xFA As String = "Numero = '" & pratica_num.ToString & "' "
            Dim praticheR As New AgronicaCoreProfilazioneDAL.Pratiche_R
            Dt = praticheR.Leggi(pratica_cod, "", piva, cuaa, 0, 0, 0, 0, #1/1/2000 12:00 PM#, DateTime.Now, "", "", objParametri_Server, 0, 0)
            pratica_cod = Dt.Rows.Item(0).Item("Pratica_Cod")

            Dim Topcode As Integer
            Dim BaseCode As Integer
            AgronicaCoreDataProvider.UtilityProvider.Calcola_BaseCode_TopCode(BaseCode, Topcode, 1)

            Dim richiesta_cod As Integer
            Dim objSequenze As New AgronicaCoreDataProvider.Agro_Sequenze

            richiesta_cod = objSequenze.NuovoId_Tabella("UMA_Richieste_Testata", BaseCode, Topcode, objParametri_Server)



            Dim Rimanenza_Benzina As Double = 0
            Dim Rimanenza_Gasolio As Double = 0
            Dim Rimanenza_Gasolio_Serra As Double = 0
            Dim permesso_Acqua As Integer = 0
            Dim note_Permesso_Acqua As String = ""

            If recuperaRimanenze AndAlso integrativa = 0 Then
                Dim dtTestata = testate_R.Leggi(piva, 0, 0, AGRODATAINIZIO, AGRODATAFINE, anno - 1, "", 1,
                                                objParametri_Server, isTerzista, " Pratiche_Stati_Attuali.Stato_Cod IN (" & enum_WAnagraficaStati.Verifica_Completata &
                                                ", " & enum_WAnagraficaStati.Verifica_Intermedia_Completata_Con_Successo & ", " &
                                                enum_WAnagraficaStati.Inserimento_Completato_Per_Il_Periodo_Di_Competenza & ") ")
                If dtTestata.Rows.Count > 0 Then
                    Rimanenza_Gasolio = dtTestata.Rows(0)("Rimanenza_Gasolio")
                    Rimanenza_Benzina = dtTestata.Rows(0)("Rimanenza_Benzina")
                    Rimanenza_Gasolio_Serra = dtTestata.Rows(0)("Rimanenza_Gasolio_Serra")

                Else

                    dtTestata = testate_R.Leggi_rimanenza_iniziale(piva, 0, isTerzista, anno, objParametri_Server)

                    If dtTestata.Rows.Count > 0 Then
                        Rimanenza_Gasolio = dtTestata.Rows(0)("Rimanenza_Gasolio")
                        Rimanenza_Benzina = dtTestata.Rows(0)("Rimanenza_Benzina")
                        Rimanenza_Gasolio_Serra = dtTestata.Rows(0)("Rimanenza_Gasolio_Serra")

                    End If
                End If
            End If

            If Not isTerzista And integrativa Then
                Dim dtAcqua = testate_R.Leggi_rimanenza_iniziale(piva, 0, isTerzista, anno, objParametri_Server, True)
                If dtAcqua.Rows.Count > 0 Then
                    Dim permessoAcquaDt = dtAcqua.Rows.Item(0).Item("permesso_Acqua")
                    permesso_Acqua = CDbl(If(IsDBNull(permessoAcquaDt), 0, permessoAcquaDt))
                    Dim notePermessoAcquaDt = dtAcqua.Rows.Item(0).Item("Note_permesso_Acqua")
                    note_Permesso_Acqua = If(IsDBNull(notePermessoAcquaDt), "", notePermessoAcquaDt)
                End If
            End If

            Dim carburante_Richiesto As Double? = 0
            Dim carburante_Richiesto_Benzina As Double? = 0
            Dim carburante_Richiesto_Gasolio As Double? = 0
            Dim carburante_Richiesto_Gasolio_Serra As Double? = 0

            If anticipoGasolio > 0 OrElse anticipoBenzina > 0 OrElse anticipoGasolioSerra > 0 Then

                'carburante_Richiesto = anticipoBenzina + anticipoGasolio + anticipoGasolioSerra
                carburante_Richiesto_Benzina = anticipoBenzina
                carburante_Richiesto_Gasolio = anticipoGasolio
                carburante_Richiesto_Gasolio_Serra = anticipoGasolioSerra

            End If

            'Dim res = JsonConvert.SerializeObject(testate.Nuova_Richiesta(piva,
            '                                                                        pratica_cod,
            '                                                                        richiesta_cod,
            '                                                                        isTerzista,
            '                                                                        tipo,
            '                                                                        anno,
            '                                                                        Rimanenza_Gasolio,
            '                                                                        Rimanenza_Benzina,
            '                                                                        Rimanenza_Gasolio_Serra,
            '                                                                        integrativa,
            '                                                                        objParametri_Server, objParametri_Utenti,
            '                                                                        IIf(anticipo = 1, True, False),
            '                                                                        carburante_Richiesto_Benzina,
            '                                                                        carburante_Richiesto_Gasolio,
            '                                                                        carburante_Richiesto_Gasolio_Serra), Formatting.None, serializerSettings)

            Dim cessata = False
            If avanzamento = 1 Then
                Dim objLeggiDateInsRendicontazione = Leggi_Date_Ins_Rendicontazione(anno, tipo_azienda, piva, richiesta_cod)
                If objLeggiDateInsRendicontazione.RispostaOK Then
                    cessata = Not CBool(JsonConvert.DeserializeObject(objLeggiDateInsRendicontazione.RispostaStringa).ToString().Split("|").First)
                Else
                    Throw New Exception(objLeggiDateInsRendicontazione.RispostaStringa)
                End If
            End If

            Dim res = JsonConvert.SerializeObject(testate.Nuova_Richiesta(piva,
                                                                          pratica_cod,
                                                                          richiesta_cod,
                                                                          isTerzista,
                                                                          avanzamento,
                                                                          anno,
                                                                          Rimanenza_Gasolio,
                                                                          Rimanenza_Benzina,
                                                                          Rimanenza_Gasolio_Serra,
                                                                          integrativa,
                                                                          objParametri_Server, objParametri_Utenti,
                                                                          IIf(anticipo = 1, True, False),
                                                                          carburante_Richiesto,
                                                                          carburante_Richiesto_Benzina,
                                                                          carburante_Richiesto_Gasolio,
                                                                          carburante_Richiesto_Gasolio_Serra,
                                                                          percentuale_anticipo_carb,
                                                                          tipo_azienda,
                                                                          cessata_Attivita:=cessata), Formatting.None, serializerSettings)

            'Dim kcol As New JObject(New JProperty("richiesta_cod", richiesta_cod.ToString),
            '                        New JProperty("carburante_Richiesto", carburante_Richiesto.ToString),
            '                        New JProperty("carburante_Richiesto_Benzina", carburante_Richiesto_Benzina.ToString),
            '                        New JProperty("carburante_Richiesto_Gasolio", carburante_Richiesto_Gasolio.ToString),
            '                        New JProperty("carburante_Richiesto_Gasolio_Serra", carburante_Richiesto_Gasolio_Serra.ToString))
            'Dim kcol As New JObject(richiesta_cod.ToString, carburante_Richiesto.ToString, carburante_Richiesto_Benzina.ToString, carburante_Richiesto_Gasolio.ToString, carburante_Richiesto_Gasolio_Serra.ToString)
            'Dim array() As String = {richiesta_cod.ToString, carburante_Richiesto.ToString, carburante_Richiesto_Benzina.ToString, carburante_Richiesto_Gasolio.ToString, carburante_Richiesto_Gasolio_Serra.ToString}
            'r.RispostaStringa = array.ToString

            Dim dtRes As New DataTable

            dtRes.Columns.Add(New DataColumn("richiesta_cod", GetType(Integer)))
            dtRes.Columns.Add(New DataColumn("carburante_Richiesto", GetType(Double)))
            dtRes.Columns.Add(New DataColumn("carburante_Richiesto_Benzina", GetType(Double)))
            dtRes.Columns.Add(New DataColumn("carburante_Richiesto_Gasolio", GetType(Double)))
            dtRes.Columns.Add(New DataColumn("carburante_Richiesto_Gasolio_Serra", GetType(Double)))


            Dim d = dtRes.NewRow
            d("richiesta_cod") = richiesta_cod
            d("carburante_Richiesto") = carburante_Richiesto
            d("carburante_Richiesto_Benzina") = carburante_Richiesto_Benzina
            d("carburante_Richiesto_Gasolio") = carburante_Richiesto_Gasolio
            d("carburante_Richiesto_Gasolio_Serra") = carburante_Richiesto_Gasolio_Serra

            dtRes.Rows.Add(d)


            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            r.RispostaStringa = JsonConvert.SerializeObject(dtRes, Formatting.None, serializerSettings)


            r.RispostaOK = True

        Catch ex As GiasException

            r.RispostaOK = False
            r.Errore = ex.Message

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r

    End Function


    <WebMethod(EnableSession:=True)>
    Public Shared Function Aggiorna_Testata(ByVal piva As String,
                                           ByVal richiesta_cod As Integer,
                                           ByVal Rimanenza_Gasolio As Double,
                                           ByVal Rimanenza_Benzina As Double,
                                           ByVal Rimanenza_Gasolio_Serra As Double) As RispostaStandard
        Dim r As New RispostaStandard
        Dim pratica_num As Integer = 1
        Dim Dt As New DataTable

        Dim serializerSettings As New JsonSerializerSettings()
        serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Utenti) Then
            r.Sessione = False
            Return r
        End If

        Lingua.Gias_InizializzaCultura_DaSession()

        Try

            Dim uma_richieste As New AgronicaCoreUmaDal.UMA_Richieste_Testata_W

            uma_richieste.Modifica_Campo_Richiesta("Rimanenza_Gasolio", Rimanenza_Gasolio, piva, richiesta_cod, objParametri_Server)
            uma_richieste.Modifica_Campo_Richiesta("Rimanenza_Benzina", Rimanenza_Benzina, piva, richiesta_cod, objParametri_Server)
            uma_richieste.Modifica_Campo_Richiesta("Rimanenza_Gasolio_Serra", Rimanenza_Gasolio_Serra, piva, richiesta_cod, objParametri_Server)


            r.RispostaStringa = "Aggiornamento eseguito correttamente"
            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Aggiorna_Carburante(ByVal piva As String,
                                               ByVal richiesta_cod As Integer,
                                               ByVal rim_riass_gasolio As Double,
                                               ByVal rim_riass_benzina As Double,
                                               ByVal rim_riass_gasolio_serra As Double,
                                               ByVal rim_riass_conf_gasolio As Double,
                                               ByVal rim_riass_conf_benzina As Double,
                                               ByVal rim_riass_conf_gasolio_serra As Double,
                                               ByVal rec_acc_dich_gasolio As Double,
                                               ByVal rec_acc_dich_benzina As Double,
                                               ByVal rec_acc_dich_gasolio_serra As Double,
                                               ByVal rec_acc_conf_gasolio As Double,
                                               ByVal rec_acc_conf_benzina As Double,
                                               ByVal rec_acc_conf_gasolio_serra As Double,
                                               ByVal causale_non_utilizzo As String,
                                               ByVal stato As String,
                                               ByVal avanzamento As Integer,
                                               ByVal Rim_Dich_Gasolio As Double,
                                               ByVal Rim_Dich_Benzina As Double,
                                               ByVal Rim_Dich_Gasolio_Serra As Double) As RispostaStandard

        Dim r As New RispostaStandard

        Dim serializerSettings As New JsonSerializerSettings()
        serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Utenti) Then
            r.Sessione = False
            Return r
        End If

        Lingua.Gias_InizializzaCultura_DaSession()

        Try

            Dim uma_richieste As New AgronicaCoreUmaDal.UMA_Richieste_Testata_W
            Dim uma_testate As New AgronicaCoreUmaBiz.UMA_Richieste

            uma_richieste.Modifica_Carburante(piva,
                                              richiesta_cod,
                                              rim_riass_gasolio,
                                              rim_riass_benzina,
                                              rim_riass_gasolio_serra,
                                              rim_riass_conf_gasolio,
                                              rim_riass_conf_benzina,
                                              rim_riass_conf_gasolio_serra,
                                              rec_acc_dich_gasolio,
                                              rec_acc_dich_benzina,
                                              rec_acc_dich_gasolio_serra,
                                              rec_acc_conf_gasolio,
                                              rec_acc_conf_benzina,
                                              rec_acc_conf_gasolio_serra,
                                              causale_non_utilizzo,
                                              stato,
                                              avanzamento,
                                              Rim_Dich_Gasolio,
                                              Rim_Dich_Benzina,
                                              Rim_Dich_Gasolio_Serra,
                                              objParametri_Server)

            Dim res = uma_testate.CalcolaRecuperoAccise(piva,
                                                        richiesta_cod,
                                                        stato,
                                                        avanzamento,
                                                        objParametri_Server,
                                                        aggiornaRecuperoAccise:=True)

            If res.Item1 Then
                serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                r.RispostaStringa = JsonConvert.SerializeObject((res, "ok"), Formatting.None, serializerSettings)
                r.RispostaOK = True
            Else
                Dim recAccGasolio = res.Item2
                Dim recAccBenzina = res.Item3
                Dim recAccGasolioSerra = res.Item4

                Dim descCarb = ""
                Dim msgRimRiass = ""
                Dim diffCarb = 0

                If avanzamento = 1 Then
                    msgRimRiass = "/rim.riassegnabili"
                End If

                Dim msgFormato = "E' stato attribuito più {0} trasferito/restituito{1} ({2}L) di quanto indicato come carburante non utilizzato dichiarato. "

                Dim msg = ""
                If recAccGasolio < 0 Then
                    descCarb = "gasolio"
                    diffCarb = recAccGasolio * -1
                    msg = String.Format(msgFormato, descCarb, msgRimRiass, diffCarb)
                End If
                If recAccBenzina < 0 AndAlso msg = "" Then
                    descCarb = "benzina"
                    diffCarb = recAccBenzina * -1
                    msg = String.Format(msgFormato, descCarb, msgRimRiass, diffCarb)
                End If
                If recAccGasolioSerra < 0 AndAlso msg = "" Then
                    descCarb = "gasolio serra"
                    diffCarb = recAccGasolioSerra * -1
                    msg = String.Format(msgFormato, descCarb, msgRimRiass, diffCarb)
                End If

                r.RispostaOK = False
                r.RispostaStringa = JsonConvert.SerializeObject((res, msg), Formatting.None, serializerSettings)

            End If

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r
    End Function


    <WebMethod(EnableSession:=True)>
    Public Shared Function Aggiorna_PermessiAcqua(ByVal piva As String,
                                                  ByVal richiesta_cod As Integer,
                                                  ByVal isTerzista As Boolean,
                                                  ByVal permessoAcqua As Double,
                                                  ByVal notePermessoAcqua As String,
                                                  ByVal isIntegrativaAcqua As Boolean) As RispostaStandard
        Dim r As New RispostaStandard
        Dim pratica_num As Integer = 1
        Dim Dt As New DataTable

        Dim serializerSettings As New JsonSerializerSettings()
        serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Utenti) Then
            r.Sessione = False
            Return r
        End If

        Lingua.Gias_InizializzaCultura_DaSession()

        Try

            Dim uma_richieste As New AgronicaCoreUmaDal.UMA_Richieste_Testata_W
            Dim leggiTesta As New AgronicaCoreUmaDal.UMA_Richieste_Testata_R
            Dim leggiLav As New AgronicaCoreUmaDal.UMA_Richieste_Lavorazioni_R
            Dim controlloPermessoAcqua As New AgronicaCoreUmaBiz.UMA_Richieste
            Dim errori As String = ""
            Dim testaRow As DataRow
            Dim dtLav = New DataTable

            Dim dtTesta = leggiTesta.Leggi(piva, richiesta_cod, 0, AGRODATAINIZIO, AGRODATAFINE, 0, "", -10, objParametri_Server)
            If (dtTesta.Rows.Count > 0) Then
                testaRow = dtTesta.Rows.Item(0)
                dtLav = leggiLav.Leggi_Lavorazioni(piva,
                                                   testaRow.Item("Anno"),
                                                   testaRow.Item("Avanzamento_Richiesta"),
                                                   testaRow.Item("Tipo_Richiesta"),
                                                   "",
                                                   "",
                                                   0,
                                                   0,
                                                   objParametri_Server,
                                                   lavorazione_Gias:=LAVCOD_IRRIGAZIONE,
                                                   xFiltroAggiuntivo:="ucm.Coeff_acq_distr > 0")

            End If

            If dtLav.Rows.Count > 0 Then
                If permessoAcqua <= 0 AndAlso Not isIntegrativaAcqua Then
                    r.RispostaStringa = "Totale permesso acqua non indicato o non valido"
                    r.RispostaOK = False
                ElseIf notePermessoAcqua = "" AndAlso Not isIntegrativaAcqua Then
                    r.RispostaStringa = "Note permesso acqua non indicate"
                    r.RispostaOK = False
                ElseIf controlloPermessoAcqua.ControlloPermessoAcqua(piva, richiesta_cod, isTerzista, errori, objParametri_Server, permessoAcqua) Then
                    uma_richieste.Modifica_Campo_Richiesta("Permesso_acqua", permessoAcqua, piva, richiesta_cod, objParametri_Server)
                    uma_richieste.Modifica_Campo_Richiesta("Note_permesso_acqua", notePermessoAcqua, piva, richiesta_cod, objParametri_Server)

                    r.RispostaStringa = "Aggiornamento eseguito correttamente"
                    r.RispostaOK = True
                Else
                    r.RispostaStringa = errori
                    r.RispostaOK = False
                End If
            Else
                uma_richieste.Modifica_Campo_Richiesta("Permesso_acqua", permessoAcqua, piva, richiesta_cod, objParametri_Server)
                uma_richieste.Modifica_Campo_Richiesta("Note_permesso_acqua", notePermessoAcqua, piva, richiesta_cod, objParametri_Server)

                r.RispostaStringa = "Aggiornamento eseguito correttamente"
                r.RispostaOK = True
            End If

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
            r.RispostaStringa = r.Errore

        End Try

        Return r
    End Function


    <WebMethod(EnableSession:=True)>
    Public Shared Function Conta_Richieste(ByVal piva As String,
                                           ByVal cuaa As String,
                                           ByVal isTerzista As Boolean,
                                           ByVal anno As Integer,
                                           ByVal Avanzamento_Richiesta As Integer,
                                           ByVal Tipo_Azienda As Integer,
                                           ByVal bocciate As Boolean) As RispostaStandard
        Dim r As New RispostaStandard
        Dim pratica_num As Integer = 1
        Dim Dt As New DataTable

        Dim serializerSettings As New JsonSerializerSettings()
        serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Utenti) Then
            r.Sessione = False
            Return r
        End If

        Lingua.Gias_InizializzaCultura_DaSession()

        Try
            Dim testate As New AgronicaCoreUmaDal.UMA_Richieste_Testata_R

            Dim dt_testate = testate.Leggi(piva, 0, 0,
                                           AGRODATAINIZIO,
                                           AGRODATAFINE,
                                           anno, "",
                                           Avanzamento_Richiesta,
                                           objParametri_Server,
                                           isTerzista:=isTerzista,
                                           " Pratiche_Stati_Attuali.Stato_Cod NOT IN (" & IIf(bocciate, "", enum_WAnagraficaStati.Verifica_Intermedia_Non_Superata & ", ") & enum_WAnagraficaStati.Rinuncia & ") ",
                                           Tipo_Azienda:=Tipo_Azienda)

            r.RispostaStringa = JsonConvert.SerializeObject(dt_testate, Formatting.None, serializerSettings)
            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r

    End Function


    <WebMethod(EnableSession:=True)>
    Public Shared Function Trova_Lavorazioni(ByVal piva As String,
                                             ByVal gruppo As String,
                                             ByVal Programmazione_Cod As Integer,
                                             ByVal Richiesta_Cod As Integer) As RispostaStandard

        Dim r As New RispostaStandard
        Dim Dt As New DataTable
        Dim DtCosti As New DataTable
        Dim dtL As New DataTable
        Dim leggiLav As New AgronicaCoreUmaDal.UMA_Richieste_Lavorazioni_R

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Utenti) Then
            r.Sessione = False
            Return r
        End If

        Lingua.Gias_InizializzaCultura_DaSession()

        Dim serializerSettings As New JsonSerializerSettings()
        serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore

        dtL = leggiLav.Leggi(piva, gruppo, Programmazione_Cod, Richiesta_Cod, 0, enumSelezioneVariabile.Selezione_TabellaCompleta, objParametri_Server)

        Dt.Columns.Add(New DataColumn("richiestaDettaglioCod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Richiesta_Cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Macrouso_UMA_Cod", GetType(String)))
        Dt.Columns.Add(New DataColumn("Programmazione_Cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Piva", GetType(String)))
        Dt.Columns.Add(New DataColumn("LavUMA", GetType(String)))
        Dt.Columns.Add(New DataColumn("Lav_UMA_Cod", GetType(String)))
        Dt.Columns.Add(New DataColumn("LavGIAS", GetType(String)))
        Dt.Columns.Add(New DataColumn("LAV_COD", GetType(String)))
        Dt.Columns.Add(New DataColumn("TipoCarb", GetType(String)))
        Dt.Columns.Add(New DataColumn("Car_Cod", GetType(String)))
        Dt.Columns.Add(New DataColumn("SupMaggiorazioneTrasferimenti", GetType(Decimal)))
        Dt.Columns.Add(New DataColumn("fabbisognoCalc", GetType(Decimal)))
        Dt.Columns.Add(New DataColumn("ltrichiesto", GetType(Decimal)))
        Dt.Columns.Add(New DataColumn("ltAssegnato", GetType(Decimal)))
        Dt.Columns.Add(New DataColumn("nLavPreviste", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("nLavRichieste", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("piuLavPreviste", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("piuRaccoltiPrevisti", GetType(Integer)))

        Dt.Columns.Add(New DataColumn("Superficie_Trattata", GetType(Decimal)))
        Dt.Columns.Add(New DataColumn("Sup_A", GetType(Decimal)))
        Dt.Columns.Add(New DataColumn("Sup_B", GetType(Decimal)))
        Dt.Columns.Add(New DataColumn("TerrenoNormale", GetType(Decimal)))
        Dt.Columns.Add(New DataColumn("TerrenoMedio", GetType(Decimal)))
        Dt.Columns.Add(New DataColumn("TerrenoTenace", GetType(Decimal)))

        Dt.Columns.Add(New DataColumn("Attivita_Cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Attivita_Des", GetType(String)))
        Dt.Columns.Add(New DataColumn("Note_Compilatore", GetType(String)))
        Dt.Columns.Add(New DataColumn("Note_Approvatore", GetType(String)))
        Dt.Columns.Add(New DataColumn("Udm_Alt", GetType(String)))
        Dt.Columns.Add(New DataColumn("Qta_Manuale", GetType(Decimal)))
        Dt.Columns.Add(New DataColumn("Mesi", GetType(Integer)))

        Dt.Columns.Add(New DataColumn("Validita_Inizio", GetType(Date)))
        Dt.Columns.Add(New DataColumn("Validita_Fine", GetType(Date)))
        Dt.Columns.Add(New DataColumn("CUAA", GetType(String)))
        Dt.Columns.Add(New DataColumn("rag_soc", GetType(String)))

        Dim cache As New Cache
        If HttpContext.Current.Cache IsNot Nothing Then
            cache = HttpContext.Current.Cache
        End If

        Dim Regione_Cod = "010"
        Dim key = objParametri_Server.PivaSuperUser & "_UMA_CalcoloCosti_" & Regione_Cod

        Dim leggiLavorazioni As New AgronicaCoreUmaDal.UMA_Configurazione_MacrousixLavorazioni_R

        If cache.Get(key) Is Nothing Then
            DtCosti = leggiLavorazioni.Leggi(Regione_Cod, "", "", 0, 0, objParametri_Server)
            cache(key) = DtCosti
        Else
            DtCosti = cache(key)
        End If

        Dim udm_alt As String = ""

        For Each row As DataRow In dtL.Rows
            Dim d0 As DataRow

            udm_alt = ""

            d0 = Dt.NewRow
            d0("richiestaDettaglioCod") = row.Item("Richiesta_Dettaglio_Cod")
            d0("Richiesta_Cod") = row.Item("Richiesta_Cod")
            d0("Piva") = row.Item("Piva")
            d0("Macrouso_UMA_Cod") = row.Item("Gruppo_Colturale_UMA")
            d0("Programmazione_Cod") = row.Item("Programmazione_Cod")

            Dim drCosto = DtCosti.Select(" Macrouso_UMA_Cod = '" & row.Item("Gruppo_Colturale_UMA") & "' " &
                                         "AND Lav_UMA_Cod = '" & row.Item("Lavorazione_UMA") & "' " &
                                         "AND Regolamento_Cod IN ('0','" & row.Item("Regolamento_Cod") & "') ")

            d0("LavUMA") = row.Item("Lav_UMA_Des")

            If drCosto.Length > 0 Then

                Dim numMaxOp = 0

                If Not IsDBNull(drCosto(0)("N_Max_Operazioni")) Then
                    numMaxOp = drCosto(0)("N_Max_Operazioni")
                End If

                If numMaxOp > 1 Then
                    d0("LavUMA") = row.Item("Lav_UMA_Des") & " (MAX " & numMaxOp.ToString() & " VOLTE)"
                End If

            End If

            If drCosto.Length > 0 AndAlso Not IsDBNull(drCosto(0)("Udm_Alternativa")) AndAlso CStr(drCosto(0)("Udm_Alternativa")).Trim() <> "" Then
                d0("LavUMA") = d0("LavUMA") & " [" & CStr(drCosto(0)("Udm_Alternativa")).Trim() & "]"
            End If

            d0("Lav_UMA_Cod") = row.Item("Lavorazione_UMA")
            d0("LavGIAS") = row.Item("Lav_Des")
            d0("LAV_COD") = row.Item("Lavorazione_GIAS")
            d0("Car_Cod") = row.Item("Tipo_Carburante")
            d0("TipoCarb") = row.Item("Car_Des")
            d0("SupMaggiorazioneTrasferimenti") = row.Item("Superficie_Maggiorazione_Trasferimenti")
            d0("fabbisognoCalc") = row.Item("Fabbisogno_Calcolato")
            d0("ltrichiesto") = row.Item("Fabbisogno_Richiesto")
            d0("ltAssegnato") = row.Item("Fabbisogno_Assegnato")
            d0("nLavPreviste") = row.Item("Nr_Lavorazioni_Previste")
            d0("nLavRichieste") = row.Item("Nr_Lavorazioni_Richieste")
            d0("piuLavPreviste") = row.Item("Piu_Lavorazioni_Previste")
            d0("piuRaccoltiPrevisti") = row.Item("Piu_Raccolti_Previsti")

            d0("Superficie_Trattata") = row.Item("Totale_Superficie_UMA")
            d0("Sup_A") = row.Item("Zona_Pendenza_A_UMA")
            d0("Sup_B") = row.Item("Zona_Pendenza_B_UMA")
            d0("TerrenoNormale") = row.Item("Zona_Tessitura_Normale_UMA")
            d0("TerrenoMedio") = row.Item("Zona_Tessitura_Media_UMA")
            d0("TerrenoTenace") = row.Item("Zona_Tessitura_Tenace_UMA")

            d0("Attivita_Cod") = row.Item("Attivita_Cod")
            d0("Attivita_Des") = row.Item("Attivita_Des")
            d0("Note_Compilatore") = row.Item("Note_Compilatore")
            d0("Note_Approvatore") = row.Item("Note_Approvatore")
            d0("Udm_Alt") = row.Item("Udm_Alternativa")
            d0("Qta_Manuale") = row.Item("Qta_Manuale")
            d0("Mesi") = row.Item("Mesi")

            d0("Validita_Inizio") = row.Item("Validita_Inizio")
            d0("Validita_Fine") = row.Item("Validita_Fine")
            d0("CUAA") = row.Item("CUAA")
            If String.IsNullOrEmpty(row.Item("CUAA").ToString()) = False Then
                d0("rag_soc") = row.Item("rag_soc")
            End If

            Dt.Rows.Add(d0)
        Next

        r.RispostaStringa = JsonConvert.SerializeObject(Dt, Formatting.None, serializerSettings)
        r.RispostaOK = True
        Return r


    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Leggi_Lavorazioni_Alternative(ByVal gruppo_colturale As Integer,
                                                         ByVal anno As String) As RispostaStandard

        Dim r As New RispostaStandard
        Dim dt As DataTable
        Dim dtres As New DataTable
        Dim dtresRow As DataRow
        Dim serializerSettings As New JsonSerializerSettings()
        serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        Dim leggiLavAlt As New AgronicaCoreUmaDal.UMA_Richieste_Lavorazioni_R

        dtres.Columns.Add(New DataColumn("Lavorazione_UMA", GetType(Integer)))
        dtres.Columns.Add(New DataColumn("Lav_UMA_Des", GetType(String)))
        dtres.Columns.Add(New DataColumn("Lavorazioni_Alt", GetType(String)))
        dtres.Columns.Add(New DataColumn("Regolamento_Cod", GetType(Integer)))


        Try

            Dim validitaAnno = GetValiditaAnno(anno)

            Dim dtQuery = leggiLavAlt.Leggi_Lavorazioni_Alternative(gruppo_colturale,
                                                                    objParametri_Server,
                                                                    validitaInizio:=validitaAnno.Inizio,
                                                                    validitaFine:=validitaAnno.Fine)

            'Isolo le lavorazioni alternative valide per entrambi i regolamenti
            Dim dr_conv = dtQuery.Select("regolamento_cod = 0")

            If dr_conv.Length > 0 Then

                'Copio elenco datarow isolati su datatable
                Dim dt_conv = dr_conv.CopyToDataTable()

                'Aggiorno il regolamento = CONVENZIONALE
                dt_conv.Columns("regolamento_cod").Expression = "1"

                'Aggiungo la datatable CONVENZIONALE a quella finale
                dtQuery.Merge(dt_conv)

                'Copio la datatable CONVENZIONALE su quella del BIOLOGICO
                Dim dt_bio = dt_conv.Copy()

                'Aggiorno il regolamento = BIOLOGICO
                dt_bio.Columns("regolamento_cod").Expression = "4"

                'Aggiungo la datatable BIOLOGICO a quella finale
                dtQuery.Merge(dt_bio)

            End If

            'Elimino dalla datatable finale le lavorazioni alternative valide per entrambi i regolamenti e riordino per lavorazione / regolamento / lavorazione alternativa
            Dim dvQuery As New DataView(dtQuery)
            dvQuery.RowFilter = "regolamento_cod <> 0"
            dvQuery.Sort = "Lavorazione_UMA, Regolamento_Cod, Lavorazione_UMA_Alt"
            dt = dvQuery.ToTable

            'Per ogni lavorazione / regolamento, costruisco una stringa contente tutte le lavorazione alternative concatenate in una stringa
            If (dt.Rows.Count > 0) Then

                dtresRow = dtres.NewRow
                dtresRow.Item("Lavorazione_UMA") = dt.Rows(0).Item("Lavorazione_UMA")
                dtresRow.Item("Lav_UMA_Des") = dt.Rows(0).Item("Lav_UMA_Des")
                dtresRow.Item("Regolamento_Cod") = dt.Rows(0).Item("Regolamento_Cod")

                For Each row As DataRow In dt.Rows

                    If (dtresRow.Item("Lavorazione_UMA") <> row.Item("Lavorazione_UMA") OrElse dtresRow.Item("Regolamento_Cod") <> row.Item("Regolamento_Cod")) Then
                        dtres.Rows.Add(dtresRow)
                        dtresRow = dtres.NewRow
                        dtresRow.Item("Lavorazione_UMA") = row.Item("Lavorazione_UMA")
                        dtresRow.Item("Lav_UMA_Des") = row.Item("Lav_UMA_Des")
                        dtresRow.Item("Regolamento_Cod") = row.Item("Regolamento_Cod")
                    End If

                    dtresRow.Item("Lavorazioni_Alt") = If(IsDBNull(dtresRow.Item("Lavorazioni_Alt")), "", dtresRow.Item("Lavorazioni_Alt") & " , ") & row.Item("Lavorazione_UMA_Alt") & " "

                Next

                dtres.Rows.Add(dtresRow)

            End If

            r.RispostaStringa = JsonConvert.SerializeObject(dtres, Formatting.None, serializerSettings)
            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Trova_Azienda_Da_CUAA(ByVal CUAA As String) As RispostaStandard

        Dim r As New RispostaStandard
        Dim rag_soc As String = ""
        Dim piva As String

        Dim serializerSettings As New JsonSerializerSettings()
        serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        Dim leggiCUAA As New AgronicaCoreAnagrafeDAL.Imprese_Codici_Read
        Dim leggiRagSoc As New AgronicaCoreAnagrafeDAL.Imprese_Read

        Try

            piva = leggiCUAA.Piva_from_CUAA(CUAA, objParametri_Server)
            If (piva <> "") Then

                rag_soc = leggiRagSoc.RagSoc_from_Piva(piva, objParametri_Server)

            End If

            r.RispostaStringa = JsonConvert.SerializeObject((rag_soc, piva), Formatting.None, serializerSettings)
            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r

    End Function
    <WebMethod(EnableSession:=True)>
    Public Shared Function Leggi_Causali_UMA() As RispostaStandard

        Dim r As New RispostaStandard

        Dim serializerSettings As New JsonSerializerSettings()
        serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        Dim leggiCausali As New UMA_Causali_BIZ

        Try

            Dim Dt = leggiCausali.Leggi(objParametri_Server)

            r.RispostaStringa = JsonConvert.SerializeObject(Dt, Formatting.None, serializerSettings)
            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Trova_FormaGiuridica_Da_CUAA(ByVal CUAA As String) As RispostaStandard

        Dim r As New RispostaStandard
        Dim isPubblica As Boolean = False

        Dim piva As String

        Dim serializerSettings As New JsonSerializerSettings()
        serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        Dim leggiCUAA As New AgronicaCoreAnagrafeDAL.Imprese_Codici_Read
        Dim leggiRagSoc As New AgronicaCoreAnagrafeDAL.Imprese_Read

        Dim objForme As New AgronicaCoreMetaSchemaDAL.FormeGiuridiche_R

        Try

            piva = leggiCUAA.Piva_from_CUAA(CUAA, objParametri_Server)
            If (piva <> "") Then
                Dim forma_giuridica As String = leggiRagSoc.FormaGiuridica_from_Piva(piva, objParametri_Server)

                If forma_giuridica <> "" Then
                    Dim dt = objForme.Leggi(forma_giuridica, "", AGRODATAINIZIO, AGRODATAFINE, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)
                    If dt.Rows.Count > 0 Then
                        Select Case dt(0).Item("flagPubblica")
                            Case "1"
                                isPubblica = True
                        End Select
                    End If
                End If
            End If

            r.RispostaStringa = JsonConvert.SerializeObject((isPubblica), Formatting.None, serializerSettings)
            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function LeggiNumeroRichiesta(ByVal richiesta_cod As Integer) As RispostaStandard

        Dim r As New RispostaStandard
        Dim rag_soc As String = ""

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")

        Try

            Dim objUMA_Richieste_Testata As New AgronicaCoreUmaDal.UMA_Richieste_Testata_R
            Dim dt = New DataTable
            dt = objUMA_Richieste_Testata.Leggi("", richiesta_cod, 0, AGRODATAINIZIO, AGRODATAFINE, 0, "", -10, objParametri_Server, False)
            If dt.Rows.Count = 0 Then
                dt = objUMA_Richieste_Testata.Leggi("", richiesta_cod, 0, AGRODATAINIZIO, AGRODATAFINE, 0, "", -10, objParametri_Server, True)
            End If


            r.RispostaStringa = ""

            If dt.Rows.Count > 0 Then
                r.RispostaStringa = CStr(dt.Rows(0)("Numero"))
            End If

            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function UC_Allevamenti_CercaRichiesteAllevamenti(ByVal piva As String, ByVal richiestaCod As Integer) As RispostaStandard
        Return RichiestaAllevamenti.CercaRichiesteAllevamenti(piva, richiestaCod)
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function UC_Allevamenti_ElencoConfigurazioni(ByVal anno As String) As RispostaStandard
        Return RichiestaAllevamenti.ElencoConfigurazioni(anno)
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function UC_Allevamenti_ElencoTipi(ByVal anno As String) As RispostaStandard
        Return RichiestaAllevamenti.ElencoTipi(anno)
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function UC_Allevamenti_Aggiorna(ByVal piva As String, ByVal richiestaCod As Integer, ByVal righeInserite As String, ByVal righeModificate As String, ByVal righeCancellate As String) As RispostaStandard
        Return RichiestaAllevamenti.AggiornaRichiesteAllevamenti(piva, richiestaCod, righeInserite, righeModificate, righeCancellate)
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function UC_Allevamenti_UF_LeggiColtureUF(ByVal piva As String, ByVal richiestaCod As Integer) As RispostaStandard
        Return RichiestaAllevamenti_UF.LeggiColtureUF(piva, richiestaCod)
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function UC_Allevamenti_UF_AggiungiRigheFascicolo(ByVal piva As String, ByVal richiestaCod As Integer, ByVal programmazione_cod As Integer) As RispostaStandard
        Return RichiestaAllevamenti_UF.AggiungiRigheFascicolo_UF(piva, richiestaCod, programmazione_cod)
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function UC_Allevamenti_UF_EliminaModificaRighe(ByVal piva As String, ByVal richiestaCod As Integer, ByVal righeCancellate As String, ByVal righeModificate As String) As RispostaStandard
        Return RichiestaAllevamenti_UF.UC_Allevamenti_UF_EliminaModificaRighe(piva, richiestaCod, righeCancellate, righeModificate)
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function UC_Allevamenti_UF_ControlloCapiAllevabili(ByVal piva As String, ByVal richiestaCod As Integer) As RispostaStandard
        Return RichiestaAllevamenti_UF.UC_Allevamenti_UF_ControlloCapiAllevabili(piva, richiestaCod)
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function UC_Allevamenti_UF_SalvaAllevatiInMontagna(ByVal richiestaCod As Integer, ByVal allevatoMontagna As Boolean) As RispostaStandard
        Return RichiestaAllevamenti_UF.UC_Allevamenti_UF_SalvaAllevatiInMontagna(richiestaCod, allevatoMontagna)
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Leggi_RestituzioniCarburanti(ByVal piva As String, ByVal Richiesta_Cod As Integer) As RispostaStandard

        Dim r As New RispostaStandard
        Dim leggiLav As New UMA_Richieste_Restituzioni_BIZ

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Utenti) Then
            r.Sessione = False
            Return r
        End If

        Lingua.Gias_InizializzaCultura_DaSession()

        Dim serializerSettings As New JsonSerializerSettings()
        serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore

        Dim dtL As DataTable = leggiLav.Leggi(piva, Richiesta_Cod, objParametri_Server)


        r.RispostaStringa = JsonConvert.SerializeObject(dtL, Formatting.None, serializerSettings)
        r.RispostaOK = True
        Return r


    End Function




#End Region

#Region "Terzisti"


    <WebMethod(EnableSession:=True)>
    Public Shared Function Trova_Richieste_Terzisti(ByVal piva As String,
                                                    ByVal richiesta_cod As Integer,
                                                    ByVal anno As Integer) As RispostaStandard

        Dim r As New RispostaStandard
        Dim dt As DataTable
        Dim dtres As New DataTable
        Dim drres As DataRow
        Dim cont As Integer = 0

        Dim leggiCUAA As New AgronicaCoreAnagrafeDAL.Imprese_Codici_Read
        Dim uma_macrousi As New AgronicaCoreMetaSchemaDAL.UMA_Macrousi_R

        Dim serializerSettings As New JsonSerializerSettings()
        serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try

            dtres.Columns.Add(New DataColumn("UMA_Cod", GetType(String)))
            dtres.Columns.Add(New DataColumn("piva", GetType(String)))
            dtres.Columns.Add(New DataColumn("rag_soc", GetType(String)))
            dtres.Columns.Add(New DataColumn("CUAA", GetType(String)))
            dtres.Columns.Add(New DataColumn("Macrouso_UMA_Cod", GetType(String)))
            dtres.Columns.Add(New DataColumn("macrouso_UMA_Des", GetType(String)))
            dtres.Columns.Add(New DataColumn("Programmazione_Cod", GetType(Integer)))
            dtres.Columns.Add(New DataColumn("Programmazione_Des", GetType(String)))
            dtres.Columns.Add(New DataColumn("sup_tot", GetType(Decimal)))
            dtres.Columns.Add(New DataColumn("sup_tot_Orig", GetType(Decimal)))
            dtres.Columns.Add(New DataColumn("supA", GetType(Decimal)))
            dtres.Columns.Add(New DataColumn("supB", GetType(Decimal)))
            dtres.Columns.Add(New DataColumn("tessitura_Norm", GetType(Decimal)))
            dtres.Columns.Add(New DataColumn("tessitura_Media", GetType(Decimal)))
            dtres.Columns.Add(New DataColumn("tessitura_Tenace", GetType(Decimal)))
            dtres.Columns.Add(New DataColumn("supA_Edit", GetType(Decimal)))
            dtres.Columns.Add(New DataColumn("supB_Edit", GetType(Decimal)))
            dtres.Columns.Add(New DataColumn("tessitura_Norm_Edit", GetType(Decimal)))
            dtres.Columns.Add(New DataColumn("tessitura_Media_Edit", GetType(Decimal)))
            dtres.Columns.Add(New DataColumn("tessitura_Tenace_Edit", GetType(Decimal)))
            dtres.Columns.Add(New DataColumn("fabbisognoCalc", GetType(Decimal)))
            dtres.Columns.Add(New DataColumn("ltrichiesto", GetType(Decimal)))
            dtres.Columns.Add(New DataColumn("ltAssegnato", GetType(Decimal)))
            dtres.Columns.Add(New DataColumn("Richiesta_Cod", GetType(Integer)))
            dtres.Columns.Add(New DataColumn("Car_Cod", GetType(String)))
            dtres.Columns.Add(New DataColumn("Regolamento_Cod", GetType(Integer)))

            Dim richieste As New AgronicaCoreUmaDal.UMA_Richieste_R

            dt = richieste.Leggi_Richieste_Terzisti(piva, richiesta_cod, anno, objParametri_Server)

            For Each dr As DataRow In dt.Rows

                drres = dtres.NewRow

                'giusto per avere una chiave univoca
                drres.Item("UMA_Cod") = cont
                cont += 1
                drres.Item("piva") = dr.Item("Piva")
                drres.Item("rag_soc") = dr.Item("rag_soc")
                drres.Item("CUAA") = leggiCUAA.Leggi_CUAA(dr.Item("Piva"), objParametri_Server)
                drres.Item("Richiesta_Cod") = dr.Item("Richiesta_Cod")
                drres.Item("Programmazione_Cod") = dr.Item("Programmazione_Cod")
                drres.Item("Regolamento_Cod") = dr.Item("Regolamento_Cod")

                If IsDBNull(dr.Item("Programmazione_Des")) Then
                    Select Case dr.Item("Programmazione_Cod")
                        Case CodiceFascicoloColtureNonImputabili
                            drres.Item("Programmazione_Des") = DescrizioneFascicoloColtureNonImputabili
                        Case CodiceFascicoloAnticipi
                            drres.Item("Programmazione_Des") = DescrizioneFascicoloAnticipi
                        Case CodiceFascicoloTrasferimenti
                            drres.Item("Programmazione_Des") = DescrizioneFascicoloTrasferimenti
                        Case CodiceFascicoloPianoColturaleGrafico
                            drres.Item("Programmazione_Des") = DescrizioneFascicoloPianoColturaleGrafico
                        Case Else
                            drres.Item("Programmazione_Des") = ""
                    End Select
                Else
                    drres.Item("Programmazione_Des") = dr.Item("Programmazione_Des")
                End If

                drres.Item("Macrouso_UMA_Cod") = dr.Item("Gruppo_Colturale_UMA")

                If (IsDBNull(dr.Item("macrouso_UMA_Des"))) Then
                    Dim macrousi = uma_macrousi.Leggi("", objParametri_Server).Where(Function(el) el.Macrouso_UMA_Cod = dr.Item("Gruppo_Colturale_UMA")).ToList
                    If macrousi.Count > 0 Then
                        drres.Item("macrouso_UMA_Des") = macrousi.First.Macrouso_UMA_Des
                    Else
                        drres.Item("macrouso_UMA_Des") = ""
                    End If
                Else
                    drres.Item("macrouso_UMA_Des") = dr.Item("macrouso_UMA_Des")
                End If

                drres.Item("sup_tot") = dr.Item("Totale_Superficie_UMA_Edit")
                drres.Item("sup_tot_Orig") = dr.Item("Totale_Superficie_UMA")
                drres.Item("supA") = dr.Item("Zona_Pendenza_A_UMA")
                drres.Item("supB") = dr.Item("Zona_Pendenza_B_UMA")
                drres.Item("tessitura_Norm") = dr.Item("Zona_Tessitura_Normale_UMA")
                drres.Item("tessitura_Media") = dr.Item("Zona_Tessitura_Media_UMA")
                drres.Item("tessitura_Tenace") = dr.Item("Zona_Tessitura_Tenace_UMA")
                drres.Item("supA_Edit") = dr.Item("Zona_Pendenza_A_UMA_Edit")
                drres.Item("supB_Edit") = dr.Item("Zona_Pendenza_B_UMA_Edit")
                drres.Item("tessitura_Norm_Edit") = dr.Item("Zona_Tessitura_Normale_UMA_Edit")
                drres.Item("tessitura_Media_Edit") = dr.Item("Zona_Tessitura_Media_UMA_Edit")
                drres.Item("tessitura_Tenace_Edit") = dr.Item("Zona_Tessitura_Tenace_UMA_Edit")
                drres.Item("fabbisognoCalc") = dr.Item("Carburante_Calcolato")
                drres.Item("ltrichiesto") = dr.Item("Carburante_Richiesto")
                drres.Item("ltAssegnato") = dr.Item("Carburante_Approvato")

                dtres.Rows.Add(drres)

            Next

            r.RispostaStringa = JsonConvert.SerializeObject(dtres, Formatting.None, serializerSettings)
            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Aggiorna_Richiesta_Carburanti(ByVal piva As String,
                                                         ByVal r_cod As Integer,
                                                         ByVal anno As Integer,
                                                         ByVal benzina As Integer,
                                                         ByVal gasolio As Integer,
                                                         ByVal gasolioSerra As Integer,
                                                         ByVal Approvato_Gasolio As Integer,
                                                         ByVal Approvato_Benzina As Integer,
                                                         ByVal Approvato_Gasolio_Serra As Integer,
                                                         ByVal Percentuale_Anticipo_Carb As Double,
                                                         ByVal Tipo_Azienda As Integer) As RispostaStandard

        Dim r As New RispostaStandard
        Dim Dt As New DataTable

        Dim serializerSettings As New JsonSerializerSettings()
        serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")

        Try
            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
                r.Sessione = False
                Return r
            End If

            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Utenti) Then
                r.Sessione = False
                Return r
            End If

            Lingua.Gias_InizializzaCultura_DaSession()

            Dim richieste As New AgronicaCoreUmaBiz.UMA_Richieste
            Dim testateDAL As New AgronicaCoreUmaDal.UMA_Richieste_Testata_R

            If (r_cod = 0) Then

                If (testateDAL.CheckTestate(piva, r_cod, -2, objParametri_Server).Rows.Count > 0) Then
                    r_cod = testateDAL.CheckTestate(piva, r_cod, -2, objParametri_Server).Rows(0)("Richiesta_Cod")
                    Tipo_Azienda = testateDAL.CheckTestate(piva, r_cod, -2, objParametri_Server).Rows(0)("Tipo_Azienda")
                Else
                    r_cod = 0
                End If

                If r_cod = 0 Then
                    Nuova_Richiesta(piva, piva, 1, anno, 0, False, 0, 0, Percentuale_Anticipo_Carb, Tipo_Azienda, 0, 0, 0)
                    r_cod = testateDAL.CheckTestate(piva, r_cod, -1, objParametri_Server).Rows(0)("Richiesta_Cod")
                End If

            End If

            r.RispostaStringa = richieste.Aggiorna_Richiesta_Carburanti(piva, r_cod, benzina, gasolio, gasolioSerra, Approvato_Benzina, Approvato_Gasolio, Approvato_Gasolio_Serra, objParametri_Server)

            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Aggiorna_Richieste_Terzisti(ByVal piva As String,
                                                       ByVal r_cod As Integer,
                                                       ByVal righeInserite As String,
                                                       ByVal righeModificate As String,
                                                       ByVal righeCancellate As String,
                                                       ByVal anno As Integer) As RispostaStandard
        '                                              ByVal Percentuale_Anticipo_Carb As Double

        Dim r As New RispostaStandard
        Dim Dt As New DataTable

        Dim serializerSettings As New JsonSerializerSettings()
        serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")

        Try
            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
                r.Sessione = False
                Return r
            End If

            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Utenti) Then
                r.Sessione = False
                Return r
            End If

            Lingua.Gias_InizializzaCultura_DaSession()

            Dim richieste As New AgronicaCoreUmaBiz.UMA_Richieste
            Dim testateDAL As New AgronicaCoreUmaDal.UMA_Richieste_Testata_R

            If (r_cod <= 0) Then

                If (testateDAL.CheckTestate(piva, r_cod, -1, objParametri_Server).Rows.Count > 0) Then

                    r_cod = testateDAL.CheckTestate(piva, r_cod, -1, objParametri_Server).Rows(0)("Richiesta_Cod")

                Else
                    r_cod = 0
                End If

            End If

            If (r_cod = 0) Then
                Nuova_Richiesta(piva, piva, 1, anno, 0, False, 0, 0, 0.0, enum_TipoAzienda_UMA.Azienda_Terzista, 0, 0, 0)
                r_cod = testateDAL.CheckTestate(piva, r_cod, -1, objParametri_Server).Rows(0)("Richiesta_Cod")
            End If

            richieste.Aggiorna_Richieste_Terzisti(r_cod, righeInserite,
                                                                      righeModificate, righeCancellate,
                                                                      objParametri_Server, piva, anno)

            r.RispostaStringa = r_cod

            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r

    End Function


    <WebMethod(EnableSession:=True)>
    Public Shared Function Trova_Richieste_Terzisti_Lavorazioni_Parziali(ByVal piva As String,
                                                    ByVal richiesta_cod As Integer,
                                                    ByVal anno As Integer) As RispostaStandard

        Dim r As New RispostaStandard
        Dim dt As DataTable
        Dim dtres As New DataTable
        Dim drres As DataRow
        Dim cont As Integer = 0

        Dim leggiCUAA As New AgronicaCoreAnagrafeDAL.Imprese_Codici_Read
        'Dim uma_macrousi As New AgronicaCoreMetaSchemaDAL.UMA_Macrousi_R

        Dim serializerSettings As New JsonSerializerSettings()
        serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try


            dtres.Columns.Add(New DataColumn("Richiesta_Cod", GetType(Integer)))
            dtres.Columns.Add(New DataColumn("Piva", GetType(String)))
            dtres.Columns.Add(New DataColumn("Lavorazione_Parziale_Cod", GetType(String)))
            dtres.Columns.Add(New DataColumn("LavUMA", GetType(String)))
            dtres.Columns.Add(New DataColumn("Lav_UMA_Cod", GetType(String)))
            dtres.Columns.Add(New DataColumn("LavGIAS", GetType(String)))
            dtres.Columns.Add(New DataColumn("LAV_COD", GetType(String)))
            dtres.Columns.Add(New DataColumn("TipoCarb", GetType(String)))
            dtres.Columns.Add(New DataColumn("Car_Cod", GetType(String)))
            dtres.Columns.Add(New DataColumn("SupMaggiorazioneTrasferimenti", GetType(Decimal)))
            dtres.Columns.Add(New DataColumn("fabbisognoCalc", GetType(Decimal)))
            dtres.Columns.Add(New DataColumn("ltrichiesto", GetType(Decimal)))
            dtres.Columns.Add(New DataColumn("ltAssegnato", GetType(Decimal)))
            dtres.Columns.Add(New DataColumn("nLavPreviste", GetType(Integer)))
            dtres.Columns.Add(New DataColumn("nLavRichieste", GetType(Integer)))
            dtres.Columns.Add(New DataColumn("piuLavPreviste", GetType(Integer)))
            dtres.Columns.Add(New DataColumn("piuRaccoltiPrevisti", GetType(Integer)))

            dtres.Columns.Add(New DataColumn("Superficie_Trattata", GetType(Decimal)))
            dtres.Columns.Add(New DataColumn("Sup_A", GetType(Decimal)))
            dtres.Columns.Add(New DataColumn("Sup_B", GetType(Decimal)))
            dtres.Columns.Add(New DataColumn("TerrenoNormale", GetType(Decimal)))
            dtres.Columns.Add(New DataColumn("TerrenoMedio", GetType(Decimal)))
            dtres.Columns.Add(New DataColumn("TerrenoTenace", GetType(Decimal)))
            dtres.Columns.Add(New DataColumn("Udm_Alt", GetType(String)))
            dtres.Columns.Add(New DataColumn("Validita_Inizio", GetType(Date)))
            dtres.Columns.Add(New DataColumn("Validita_Fine", GetType(Date)))
            dtres.Columns.Add(New DataColumn("Regolamento_Cod", GetType(Integer)))
            dtres.Columns.Add(New DataColumn("Regolamento_Check", GetType(Boolean)))


            Dim lavorazioni As New AgronicaCoreUmaDal.UMA_Richieste_Lavorazioni_R

            dt = lavorazioni.Leggi_Lavorazioni_Parziali(piva, richiesta_cod, 0, enumSelezioneVariabile.Selezione_TabellaCompleta, objParametri_Server)
            For Each dr As DataRow In dt.Rows

                drres = dtres.NewRow

                'giusto per avere una chiave univoca
                'drres.Item("UMA_Cod") = cont
                'cont += 1

                drres("Richiesta_Cod") = dr.Item("Richiesta_Cod")
                drres("Regolamento_Cod") = dr.Item("Regolamento_Cod")
                drres("Regolamento_Check") = If(dr.Item("Regolamento_Cod") = 4, True, False)
                drres("Piva") = dr.Item("Piva")
                drres("Lavorazione_Parziale_Cod") = dr.Item("Lavorazione_Parziale_Cod")
                'drres("Macrouso_UMA_Cod") = dr.Item("Gruppo_Colturale_UMA")
                'drres("Programmazione_Cod") = dr.Item("Programmazione_Cod")

                'Dim drCosto = DtCosti.Select(" Macrouso_UMA_Cod = '" & row.Item("Gruppo_Colturale_UMA") & "' " &
                '           "AND Lav_UMA_Cod = '" & row.Item("Lavorazione_UMA") & "' ")


                'd0("LavUMA") = row.Item("Lav_UMA_Des")

                'If drCosto.Length > 0 AndAlso Not IsDBNull(drCosto(0)("Udm_Alternativa")) AndAlso CStr(drCosto(0)("Udm_Alternativa")).Trim() <> "" Then
                '    d0("LavUMA") = row.Item("Lav_UMA_Des") & " [" & CStr(drCosto(0)("Udm_Alternativa")).Trim() & "]"
                'End If

                drres("LavUMA") = dr.Item("Lav_UMA_Des")
                drres("Lav_UMA_Cod") = dr.Item("Lavorazione_UMA")
                drres("LavGIAS") = dr.Item("Lavorazione_GIAS")
                drres("LAV_COD") = dr.Item("Lavorazione_GIAS")
                drres("Car_Cod") = dr.Item("Tipo_Carburante")
                drres("TipoCarb") = dr.Item("Car_Des")
                drres("SupMaggiorazioneTrasferimenti") = dr.Item("Superficie_Maggiorazione_Trasferimenti")
                drres("fabbisognoCalc") = dr.Item("Fabbisogno_Calcolato")
                drres("ltrichiesto") = dr.Item("Fabbisogno_Richiesto")
                drres("ltAssegnato") = dr.Item("Fabbisogno_Assegnato")
                drres("nLavPreviste") = dr.Item("Nr_Lavorazioni_Previste")
                drres("nLavRichieste") = dr.Item("Nr_Lavorazioni_Richieste")
                drres("piuLavPreviste") = dr.Item("Piu_Lavorazioni_Previste")
                drres("piuRaccoltiPrevisti") = dr.Item("Piu_Raccolti_Previsti")

                drres("Superficie_Trattata") = dr.Item("Totale_Superficie_UMA")
                drres("Sup_A") = dr.Item("Zona_Pendenza_A_UMA")
                drres("Sup_B") = dr.Item("Zona_Pendenza_B_UMA")
                drres("TerrenoNormale") = dr.Item("Zona_Tessitura_Normale_UMA")
                drres("TerrenoMedio") = dr.Item("Zona_Tessitura_Media_UMA")
                drres("TerrenoTenace") = dr.Item("Zona_Tessitura_Tenace_UMA")

                drres("Udm_Alt") = dr.Item("Udm_Alternativa")
                drres("Validita_Inizio") = dr.Item("Validita_Inizio")
                drres("Validita_Fine") = dr.Item("Validita_Fine")


                dtres.Rows.Add(drres)

            Next

            r.RispostaStringa = JsonConvert.SerializeObject(dtres, Formatting.None, serializerSettings)
            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r

    End Function

#End Region

#Region "Aggiornamento Dati"

    <WebMethod(EnableSession:=True)>
    Public Shared Function CopiaRichiesta(richiesta_cod As Integer, nome_file As String, file_allegato As String) As RispostaStandard
        Dim r As New RispostaStandard
        Dim rag_soc As String = ""

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri_Server.StringaConnessione)
        Dim transactionOptions = New TransactionOptions()
        transactionOptions.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted

        Using scope As New TransactionScope(TransactionScopeOption.Required, transactionOptions)
            Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

                Try
                    Dim resp = ""
                    Dim objUMA_Richieste_Testata As New AgronicaCoreUmaDal.UMA_Richieste_Testata_R
                    Dim dt = New DataTable
                    dt = objUMA_Richieste_Testata.Leggi("", richiesta_cod, 0, AGRODATAINIZIO, AGRODATAFINE, 0, "", 0, objParametri_Server, False)
                    If dt.Rows.Count = 0 Then
                        dt = objUMA_Richieste_Testata.Leggi("", richiesta_cod, 0, AGRODATAINIZIO, AGRODATAFINE, 0, "", 0, objParametri_Server, True)
                    End If

                    If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                        Dim isTerz = False
                        If dt.Rows(0)("Tipo_Richiesta") = -1 Then
                            isTerz = True
                        End If
                        Dim richiesta_old = objUMA_Richieste_Testata.Leggi(dt.Rows(0)("Piva"),
                                                                           0,
                                                                           0,
                                                                           AGRODATAINIZIO,
                                                                           AGRODATAFINE,
                                                                           0,
                                                                           "",
                                                                           1,
                                                                           objParametri_Server,
                                                                           isTerz)

                        If richiesta_old IsNot Nothing AndAlso richiesta_old.Rows.Count > 0 Then
                            Dim objUMA_Richieste_BIZ As New AgronicaCoreUmaBiz.UMA_Richieste
                            objUMA_Richieste_BIZ.CopiaRendicontazioneInRichiesta(richiesta_old.Rows(richiesta_old.Rows.Count - 1)("Richiesta_Cod"),
                                                                                 richiesta_cod,
                                                                                 True,
                                                                                 "",
                                                                                 objParametri_Server)

                            If isTerz = False AndAlso Not String.IsNullOrEmpty(nome_file) AndAlso Not String.IsNullOrEmpty(file_allegato) Then
                                Dim objAlert_BIZ As New AgronicaCoreScadenziario_BIZ.Alert_W


                                resp = objAlert_BIZ.Salvataggio_Allegato_Documentale(dt.Rows(0)("Piva"),
                                                                                        enum_ID_Area_Alert.UMA_Carburanti,
                                                                                        enum_ID_Area_Tipologia.UMA_dichiarazione_pre_assegnazione,
                                                                                        nome_file,
                                                                                        file_allegato,
                                                                                        objParametri_Utenti.UtenteUsername,
                                                                                        AGRODATAFINE,
                                                                                        "",
                                                                                        New JArray(),
                                                                                        richiesta_cod,
                                                                                        0,
                                                                                        objParametri_Server,
                                                                                        objParametri_Utenti)


                                If Not String.IsNullOrEmpty(resp) Then
                                    Throw New Exception(resp)
                                End If
                            End If



                        End If

                    End If

                    GiasContext.SaveChanges()
                    scope.Complete()

                    r.RispostaStringa = resp
                    r.RispostaOK = True

                Catch ex As Exception
                    scope.Dispose()

                    r.RispostaOK = False
                    r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

                End Try
            End Using

        End Using

        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function AggiornaRichieste(ByVal piva As String,
                                             ByVal Richiesta_Cod As Integer,
                                             ByVal isTerzista As Boolean,
                                             ByVal Programmazione_Cod As Integer,
                                             ByVal righeInserite As String,
                                             ByVal righeModificate As String,
                                             ByVal righeCancellate As String,
                                             ByVal avanzamento As Integer,
                                             ByVal regolamento_Cod As Integer) As RispostaStandard

        Dim r As New RispostaStandard
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try

            Dim scrivi As New AgronicaCoreUmaBiz.UMA_Richieste
            Dim res = scrivi.Aggiorna_Lavorazioni(piva, Richiesta_Cod, isTerzista, Programmazione_Cod, righeInserite, righeModificate, righeCancellate, objParametri_Server, avanzamento, regolamento_Cod)
            r.RispostaStringa = res.Item1
            If (res.Item2.Length > 0) Then
                r.RispostaOK = False
                r.Errore = "Non tutte le lavorazioni sono state inserite perchè sono state rilevate delle sovrapposizioni " & res.Item2
            ElseIf res.Item3.Length > 0 Then
                r.RispostaOK = False
                r.Errore = res.Item3
            Else
                r.RispostaOK = True
            End If

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function CercaPivaReale(ByVal piva As String) As RispostaStandard


        Dim r As New RispostaStandard
        Dim PartitaIvaReale As String = ""
        Dim leggiPivaReale As New AgronicaCoreAnagrafeDAL.Imprese_Read

        Try

            Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
                r.Sessione = False
                Return r
            End If

            Lingua.Gias_InizializzaCultura_DaSession()

            If (Not piva.Equals(String.Empty)) Then
                PartitaIvaReale = leggiPivaReale.Leggi_PivaReale(piva, objParametri_Server)
            End If

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            r.RispostaStringa = JsonConvert.SerializeObject(PartitaIvaReale, Formatting.None, serializerSettings)

            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "ErroreDuranteOperazione_" & vbCrLf &
            AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function AggiornaRichieste_UMA(ByVal piva As String,
                                             ByVal Richiesta_Cod As Integer,
                                             ByVal righe As String) As RispostaStandard

        Dim r As New RispostaStandard
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If
        Dim testate = New AgronicaCoreUmaDal.UMA_Richieste_Testata_R
        Dim richiesteR = New AgronicaCoreUmaDal.UMA_Richieste_R
        Dim richiesteW = New AgronicaCoreUmaDal.UMA_Richieste_W
        Dim legamiRApp As New DataTable ' per contenere i nuovi record di UMARichiestexregImpianti
        Dim legamiDaRecidere As List(Of Tuple(Of String, String)) = New List(Of Tuple(Of String, String))
        Dim richiestexRegImpianti As New AgronicaCoreUmaDal.UMA_RichiesteXReg_Impianti_W
        'Dim anno = testate.RecuperaAnnoRichiesta(Richiesta_Cod, objParametri_Server)

        Try
            Dim Res As Integer
            Dim transactionOptions = New TransactionOptions()
            transactionOptions.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted
            Using scope As New TransactionScope(TransactionScopeOption.Required, transactionOptions)
                Dim richieste As New AgronicaCoreUmaDal.UMA_Richieste_W
                Dim lavorazioni As New AgronicaCoreUmaDal.UMA_Richieste_Lavorazioni_W
                Dim gefutils As New Gias_EF_Utility
                Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri_Server.StringaConnessione)
                Dim GiasContext As New Gias_DeveloperServer_Entities(EFConnString)
                Dim richiesteDelete = New ArrayList

                'Richieste Insert
                Dim richiesteInsert = New ArrayList()
                Dim arrIns = New JArray 'JArray.Parse(righe).Where(Function(el) CInt(el("Richiesta_Cod").ToString) = 0)

                'Richieste Update
                Dim richiesteUpdate = New ArrayList()
                Dim arrUpd = JArray.Parse(righe).Where(Function(el) CInt(el("Richiesta_Cod").ToString) <> 0)
                For Each elem In arrUpd
                    Dim Macrouso_UMA_Cod As String = CStr(elem("Macrouso_UMA_Cod"))
                    Dim Programmazione_Cod As Integer = CStr(elem("Programmazione_Cod"))
                    Dim uma_r = (From u In
                                     GiasContext.UMA_Richieste Where
                                                    u.Piva = piva AndAlso
                                                    u.Richiesta_Cod = Richiesta_Cod AndAlso
                                                    u.Gruppo_Colturale_UMA = Macrouso_UMA_Cod AndAlso
                                                    u.Programmazione_Cod = Programmazione_Cod).FirstOrDefault
                    If uma_r IsNot Nothing Then
                        If uma_r.Regolamento_Cod = CInt(elem("Regolamento_Cod")) Then
                            uma_r.Totale_Superficie_UMA_Edit = elem("sup_UMA_Edit")
                            uma_r.Zona_Pendenza_A_UMA_Edit = elem("sup_UMA_A_Edit")
                            uma_r.Zona_Pendenza_B_UMA_Edit = elem("sup_UMA_B_Edit")
                            uma_r.Zona_Tessitura_Normale_UMA_Edit = elem("TerrenoNormale_Edit")
                            uma_r.Zona_Tessitura_Media_UMA_Edit = elem("TerrenoMedio_Edit")
                            uma_r.Zona_Tessitura_Tenace_UMA_Edit = elem("TerrenoTenace_Edit")

                            richiesteUpdate.Add(uma_r)
                        Else

                            richiesteDelete.Add(uma_r)

                            Dim uma_r_temp = uma_r.DeepCloneObject
                            uma_r_temp.Totale_Superficie_UMA_Edit = elem("sup_UMA_Edit")
                            uma_r_temp.Zona_Pendenza_A_UMA_Edit = elem("sup_UMA_A_Edit")
                            uma_r_temp.Zona_Pendenza_B_UMA_Edit = elem("sup_UMA_B_Edit")
                            uma_r_temp.Zona_Tessitura_Normale_UMA_Edit = elem("TerrenoNormale_Edit")
                            uma_r_temp.Zona_Tessitura_Media_UMA_Edit = elem("TerrenoMedio_Edit")
                            uma_r_temp.Zona_Tessitura_Tenace_UMA_Edit = elem("TerrenoTenace_Edit")
                            uma_r_temp.Regolamento_Cod = elem("Regolamento_Cod")

                            richiesteInsert.Add(uma_r_temp)

                        End If
                    Else
                        arrIns.Add(elem)
                    End If

                Next

                For Each elem In arrIns
                    Dim Richiesta_Ins As New AgronicaCoreEntityFramework_POCO.UMA_Richieste With {
                        .Piva_SuperUser = objParametri_Server.PivaSuperUser,
                        .Piva = piva,
                        .Gruppo_Colturale_UMA = CStr(elem("Macrouso_UMA_Cod")),
                        .Richiesta_Cod = Richiesta_Cod,
                        .Totale_Superficie_UMA = CDbl(elem("sup_UMA")),
                        .Zona_Pendenza_A_UMA = CDbl(elem("sup_UMA_A")),
                        .Zona_Pendenza_B_UMA = CDbl(elem("sup_UMA_B")),
                        .Zona_Tessitura_Normale_UMA = CDbl(elem("TerrenoNormale")),
                        .Zona_Tessitura_Media_UMA = CDbl(elem("TerrenoMedio")),
                        .Zona_Tessitura_Tenace_UMA = CDbl(elem("TerrenoTenace")),
                        .Carburante_Calcolato = CDbl(elem("calcolato")),
                        .Carburante_Richiesto = CDbl(elem("richiesto")),
                        .Carburante_Approvato = CDbl(elem("assegnato")),
                        .Validita_Inizio = AGRODATAINIZIO,
                        .Validita_Fine = AGRODATAFINE,
                        .Username_Creazione = objParametri_Server.UtenteUsername,
                        .Username_Modifica = objParametri_Server.UtenteUsername,
                        .Data_Creazione = DateTime.Now,
                        .Data_Modifica = DateTime.Now,
                        .Totale_Superficie_UMA_Edit = CDbl(elem("sup_UMA_Edit")),
                        .Zona_Pendenza_A_UMA_Edit = CDbl(elem("sup_UMA_A_Edit")),
                        .Zona_Pendenza_B_UMA_Edit = CDbl(elem("sup_UMA_B_Edit")),
                        .Zona_Tessitura_Normale_UMA_Edit = CDbl(elem("TerrenoNormale_Edit")),
                        .Zona_Tessitura_Media_UMA_Edit = CDbl(elem("TerrenoMedio_Edit")),
                        .Zona_Tessitura_Tenace_UMA_Edit = CDbl(elem("TerrenoTenace_Edit")),
                        .Programmazione_Cod = CInt(elem("Programmazione_Cod").ToString()),
                        .Regolamento_Cod = CInt(elem("Regolamento_Cod").ToString())
                    }

                    If (Richiesta_Ins.Gruppo_Colturale_UMA <> "") Then
                        richiesteInsert.Add(Richiesta_Ins)
                    End If

                    If Richiesta_Ins.Programmazione_Cod = -4 Then

                        Dim tempLegami = richiesteR.EstraiLegamiRichiestaxAppezzamento(Richiesta_Ins.Piva, Richiesta_Cod,
                                                                          " (a.macrouso_UMA_Cod  = " & Richiesta_Ins.Gruppo_Colturale_UMA & " OR (a.macrouso_UMA_Cod IS NULL AND b.macrouso_UMA_Cod  = " & Richiesta_Ins.Gruppo_Colturale_UMA & ") OR (a.macrouso_UMA_Cod IS NULL AND b.macrouso_UMA_Cod IS NULL AND c.macrouso_UMA_Cod  = " & Richiesta_Ins.Gruppo_Colturale_UMA & ")" &
"OR (a.macrouso_UMA_Cod IS NULL AND b.macrouso_UMA_Cod IS NULL AND c.macrouso_UMA_Cod IS NULL AND d.macrouso_UMA_Cod  = " & Richiesta_Ins.Gruppo_Colturale_UMA & ")) ", objParametri_Server)

                        If legamiRApp.Rows.Count = 0 Then

                            legamiRApp = tempLegami

                        Else

                            legamiRApp.Merge(tempLegami)

                        End If

                    End If

                Next

                Dim RichiesteDB = (From rd In GiasContext.UMA_Richieste Where rd.Richiesta_Cod = Richiesta_Cod AndAlso rd.Piva = piva)
                For Each rd In RichiesteDB
                    Dim find = False
                    For Each upd As AgronicaCoreEntityFramework_POCO.UMA_Richieste In richiesteUpdate
                        If rd.Piva_SuperUser = upd.Piva_SuperUser AndAlso
                            rd.Piva = upd.Piva AndAlso
                            rd.Richiesta_Cod = upd.Richiesta_Cod AndAlso
                            rd.Programmazione_Cod = upd.Programmazione_Cod AndAlso
                            rd.Gruppo_Colturale_UMA = upd.Gruppo_Colturale_UMA Then
                            find = True
                            Exit For
                        End If
                    Next
                    If Not find Then
                        richiesteDelete.Add(rd)

                        If rd.Programmazione_Cod = -4 Then

                            legamiDaRecidere.Add(Tuple.Create(rd.Piva, rd.Gruppo_Colturale_UMA))

                        End If
                    End If

                Next


                Res = richieste.Aggiorna_Richieste(richiesteInsert, richiesteUpdate, richiesteDelete, objParametri_Server)

                If Res = 0 Then
                    If legamiRApp.Rows.Count > 0 Then

                        richiestexRegImpianti.InserisciAssociazioniPerNuovaPraticaContoProprio(legamiRApp, Richiesta_Cod, objParametri_Server)

                    End If

                    If legamiDaRecidere.Count > 0 Then

                        richiestexRegImpianti.EliminaAssociazioni(piva, legamiDaRecidere, Richiesta_Cod, objParametri_Server)

                    End If
                End If

                Dim deleteLavorazioni As New ArrayList

                For Each richiestaDel As AgronicaCoreEntityFramework_POCO.UMA_Richieste In richiesteDelete
                    Dim opDel = (From op In GiasContext.UMA_Richieste_Lavorazioni Where op.Piva = richiestaDel.Piva AndAlso
                                                                                      op.Piva_SuperUser = richiestaDel.Piva_SuperUser AndAlso
                                                                                      op.Richiesta_Cod = richiestaDel.Richiesta_Cod AndAlso
                                                                                      op.Gruppo_Colturale_UMA = richiestaDel.Gruppo_Colturale_UMA AndAlso
                                                                                      op.Programmazione_Cod = richiestaDel.Programmazione_Cod).ToList
                    If opDel.Count > 0 Then
                        For Each op In opDel
                            deleteLavorazioni.Add(op)
                        Next
                    End If
                Next

                lavorazioni.Aggiorna_Lavorazioni(New ArrayList,
                                                 New ArrayList,
                                                 New ArrayList,
                                                 deleteLavorazioni,
                                                 objParametri_Server)

                If Res = 0 Then

                    ' COMMIT Effettivo
                    scope.Complete()
                Else
                    ' Rollback
                    scope.Dispose()
                End If

            End Using
            r.RispostaStringa = ""
            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r
    End Function

#End Region
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Master.flag_MostraBtnIndietro = True

        inizializzoObjParametri()
        inizializzoParametriPagina()

        Dim Entrata_Diretta As Integer = 0
        Dim Split As Integer = 0

        If Not IsNothing(Request.QueryString("p")) Then
            QS_Piva = Stringa_Decodifica(Request.QueryString("p").ToString, AgroKey_EncoderDecoder, Server)
        End If

        If Not IsNothing(Request.QueryString("rc")) Then
            QS_Richiesta = CInt(Stringa_Decodifica(Request.QueryString("rc").ToString, AgroKey_EncoderDecoder, Server))
        End If

        QS_Type = 0
        If Not IsNothing(Request.QueryString("type")) Then
            QS_Type = CInt(Request.QueryString("type"))
        End If

        QS_Avanzamento = 0
        If Not IsNothing(Request.QueryString("avanzamento")) Then
            QS_Avanzamento = CInt(Request.QueryString("avanzamento"))
        End If

        QS_Anticipo = 0
        If Not IsNothing(Request.QueryString("anticipo")) Then
            QS_Anticipo = CInt(Request.QueryString("anticipo"))
        End If

        QS_TipoAzienda = enum_TipoAzienda_UMA.Azienda_Agricola_Privata
        If Not IsNothing(Request.QueryString("tipo_azienda")) Then
            QS_TipoAzienda = CInt(Request.QueryString("tipo_azienda"))
        End If

        QS_TipoOp = enum_TipoOperazioneDB.Scrittura
        If Not IsNothing(Request.QueryString("tipoOp")) Then
            QS_TipoOp = CInt(Request.QueryString("tipoOp"))
        End If

        If Not IsNothing(Request.QueryString("provenienza")) Then
            QS_Provenienza = CInt(Request.QueryString("provenienza"))
        End If

        QS_UsaAnalisiTerrenoNG = If(objAnalisiModelloUtils.usaAnalisiTerrenoNG(objParametri_Utenti), 1, 0)

        Dim enumAppro = IIf(QS_Avanzamento = 0, enum_Security_Attivita.Approvazione_Richiesta_UMA, enum_Security_Attivita.Approvazione_Rendicontazione_UMA)
        Dim enumBase = IIf(QS_Avanzamento = 0, enum_Security_Attivita.Richiesta_UMA, enum_Security_Attivita.Rendicontazione_UMA)

        'Controllo se l'utente ha i permessi per accedere
        Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
        Dim UtenteAbilitatoLettura As Boolean = objPermessi.Controlla_Permessi_Utente(
                                            Session("ASG_Utente_Username"),
                                            Session("ASG_IdServizio"),
                                            enumAppro,
                                            enum_Security_Operazione.Lettura,
                                            Date.Now,
                                            "",
                                            objParametri_Utenti)
        If Not UtenteAbilitatoLettura Then
            UtenteAbilitatoLettura = objPermessi.Controlla_Permessi_Utente(
                                            Session("ASG_Utente_Username"),
                                            Session("ASG_IdServizio"),
                                            enumBase,
                                            enum_Security_Operazione.Lettura,
                                            Date.Now,
                                            "",
                                            objParametri_Utenti)
        End If

        Dim UtenteAbilitatoScrittura As Boolean = objPermessi.Controlla_Permessi_Utente(
                                           Session("ASG_Utente_Username"),
                                           Session("ASG_IdServizio"),
                                           enumAppro,
                                           enum_Security_Operazione.Modifica,
                                           Date.Now,
                                           "",
                                           objParametri_Utenti)

        If Not UtenteAbilitatoScrittura Then
            UtenteAbilitatoScrittura = objPermessi.Controlla_Permessi_Utente(
                                            Session("ASG_Utente_Username"),
                                            Session("ASG_IdServizio"),
                                            enumBase,
                                            enum_Security_Operazione.Modifica,
                                            Date.Now,
                                            "",
                                            objParametri_Utenti)
        End If

        Dim UtenteAbilitatoMacchine As Boolean = objPermessi.Controlla_Permessi_Utente(
                                                       Session("ASG_Utente_Username"),
                                                       Session("ASG_IdServizio"),
                                                       enum_Security_Attivita.Anagrafica_ParcoMacchine,
                                                       enum_Security_Operazione.Modifica,
                                                       Date.Now,
                                                       "",
                                                       objParametri_Utenti)
        hdUtenteAbilitatoMacchine.Value = UtenteAbilitatoMacchine

        Dim UtenteAbilitatoAnalisiTerreno As Boolean = objPermessi.Controlla_Permessi_Utente(
                                                       Session("ASG_Utente_Username"),
                                                       Session("ASG_IdServizio"),
                                                       enum_Security_Attivita.Gest_Analisi_AccessoMenu,
                                                       enum_Security_Operazione.Lettura,
                                                       Date.Now,
                                                       "",
                                                       objParametri_Utenti)
        hdUtenteAbilitatoAnalisiTerreno.Value = UtenteAbilitatoAnalisiTerreno

        'Pagina di origine
        hdPaginaRedirect.Value = ""
        hdPaginaRedirect_Codificata.Value = ""

        'Imposto le variabili di ponte con il client
        hf_UtenteAbilitatoLettura.Value = UtenteAbilitatoLettura
        hf_UtenteAbilitatoScrittura.Value = UtenteAbilitatoScrittura

        If Not UtenteAbilitatoLettura Then
            If String.IsNullOrWhiteSpace(CStr(hdPaginaRedirect_Codificata.Value)) Then
                Response.Redirect("~/Menu/MenuBS_2017.aspx")
            Else
                Response.Redirect(CStr(hdPaginaRedirect.Value) & "?p=" & Request.QueryString("p"))
            End If
        End If

        Dim objGruppi_Utenti As New AgronicaCoreUtentiDAL.Gruppi_Utente_R
        Dim objUtenti As New AgronicaCoreUtentiDAL.Utenti_xGruppi_Utente_R
        Dim coordinatore As Boolean
        Dim DTGruppoUtente = objUtenti.Leggi(objParametri_Server.UtenteUsername, "0", "", "", objParametri_Utenti)
        If DTGruppoUtente.Rows.Count > 0 Then
            Dim DTConfigurazioneGruppo = objGruppi_Utenti.Leggi(DTGruppoUtente.Rows(0)("Gruppi_Utente_cod"), "", "", objParametri_Utenti)
            If DTConfigurazioneGruppo.Rows.Count > 0 AndAlso Not IsDBNull(DTConfigurazioneGruppo.Rows(0)("ConfigurazioniAggiuntive")) Then
                Dim ConfigurazioneGruppoStr = CStr(DTConfigurazioneGruppo.Rows(0)("ConfigurazioniAggiuntive"))
                If ConfigurazioneGruppoStr <> "" Then
                    Try
                        Dim ConfigurazioneGruppo = JObject.Parse(ConfigurazioneGruppoStr)
                        If ConfigurazioneGruppo("CoordinatoreAfor") IsNot Nothing Then
                            If ConfigurazioneGruppo("CoordinatoreAfor") Then
                                coordinatore = True
                            End If
                        End If
                    Catch ex As Exception

                    End Try
                End If
            End If
        End If

        hdId_Agenda.Value = 0
        hdId_Mov.Value = 0
        hdId_Mov_Det.Value = 0
        hdId_Agenda_CDG.Value = 0
        hdModalita.Value = Entrata_Diretta
        hdId_CDG.Value = 0
        hdLav_Cod.Value = 0
        hdVeg_Cod.Value = 0
        hdDes_Lib.Value = ""
        hdMov_Desc.Value = ""
        hdSuperUser.Value = objParametri_Server.SuperUserUsername
        hcoordinatoreAfor.Value = coordinatore

        hdData.Value = JToken.Parse(JsonConvert.SerializeObject(Now))

        'Lettura chiave test UMA
        Dim objCfgSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
        Dim VCS_TestUMA = objCfgSiti.Leggi_Valore_JSON(Of AgronicaCoreVarieDAL.VCS_TestUMA)(objParametri_Server)
        hdTestUMA_Abilitato.Value = VCS_TestUMA.Abilitato
        hdTestUMA_Mese.Value = VCS_TestUMA.Mese

        'Automatico (Impostazione da QDC)

        hdAutomatico.Value = 1 'Default

        If Not Page.IsPostBack Then


            If Not IsNothing(Session("ParametriAgenda_2010")) Then

                Dim objParametriAgenda_2010 As New ParametriAgenda_2010
                objParametriAgenda_2010.Leggi()

                hdId_Agenda.Value = Val(objParametriAgenda_2010.Id_Agenda)

            End If

            'If Entrata_Diretta = 0 Then

            If Val(hdId_Agenda.Value) = 0 Then

            End If

        End If

        AddHandler CType(Me.Master, AgronicaUMA.UmaBootstrap).ImgBtn_AnnullaTutto.Click, AddressOf Me.AnnullaTutto

    End Sub

    Private Sub inizializzoParametriPagina()

        hdPiva.Value = Stringa_Decodifica(CStr(Request.QueryString("pc")), AgroKey_EncoderDecoder, Nothing)
        If hdPiva.Value <> "" Then
            QS_PivaCod = Request.QueryString("pc")
        End If


    End Sub

    Public Shadows ReadOnly Property Master() As AgronicaUMA.UmaBootstrap
        Get
            Return CType(MyBase.Master, AgronicaUMA.UmaBootstrap)
        End Get
    End Property

    <WebMethod(EnableSession:=True)>
    Public Shared Function Leggi_UMA_Setup(ByVal anno As Integer) As RispostaStandard
        Dim r As New RispostaStandard
        Try

            Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
            Dim objUMA_Setup As New AgronicaCoreUmaDal.UMASetup_R
            Dim setDt = objUMA_Setup.LeggiSetup(anno, objParametri_Server)

            Dim cache As New Cache
            If HttpContext.Current.Cache IsNot Nothing Then
                cache = HttpContext.Current.Cache
            End If

            Dim key = objParametri_Server.PivaSuperUser & "UMA_Leggi_UMA_Setup_" & anno
            Dim strResult = "{}"

            If cache IsNot Nothing AndAlso cache.Item(key) Is Nothing Then

                If setDt.Rows.Count > 0 Then
                    Dim serializerSettings As New JsonSerializerSettings()
                    serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    strResult = JsonConvert.SerializeObject(setDt, Formatting.None, serializerSettings)
                End If

                cache.Item(key) = strResult
                r.RispostaStringa = strResult
                r.RispostaOK = True

            Else
                r.RispostaStringa = cache.Item(key)
                r.RispostaOK = True
            End If

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
            AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Cerca_Macchine(ByVal piva As String, richiesta_cod As Integer, anno As String) As RispostaStandard

        Dim r As New RispostaStandard
        Dim Dt As New DataTable
        Dim Dtres As New DataTable
        Dim objParcoMacchine As New AgronicaCoreContabDAL.Parco_Macchine_R

        Try

            Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
                r.Sessione = False
                Return r
            End If

            Lingua.Gias_InizializzaCultura_DaSession()

            'Dim xFiltroAggiuntivo As String = "Parco_Macchine.Validita_Inizio <= " & Agro_SQL_SaveDate("31/12/" & anno) & " AND Parco_Macchine.validita_fine >= " & Agro_SQL_SaveDate("01/01/" & anno) & ""

            Dim xFiltroAggiuntivo = GetFiltroAggiuntivoValiditaParcoMacchine("Parco_Macchine", anno)

            Dt = objParcoMacchine.leggi_x_anagrafica(piva, xFiltroAggiuntivo, "", objParametri_Server)

            Dt.Columns.Add(New DataColumn("Selected", GetType(Boolean)))
            Dt.Columns.Add(New DataColumn("Possesso", GetType(String)))
            Dt.Columns.Add(New DataColumn("isTarga_Obbligatoria", GetType(Integer)))
            Dt.Columns.Add(New DataColumn("Targa_Obbligatoria", GetType(String)))

            Dim check As New AgronicaCoreUmaDal.UMA_Richieste_Testata_R

            Dim testata = check.CheckTestate(piva, richiesta_cod, -2, objParametri_Server)

            '------------------------------------
            ' VARIABILI CONTROLLI TARGA
            '------------------------------------
            Dim targhe_list As New List(Of String)
            Dim ClassCode_Targa_Obbligatoria As New List(Of String)
            'leggo da UMA_Setup le tipologie di macchina per cui è obbligatoria la targa. 
            getFiltro_Macchine_Targhe_Obbligatorie(anno, ClassCode_Targa_Obbligatoria, objParametri_Server)

            Dim listMacchine As New List(Of String)
            If testata.Rows.Count > 0 Then
                If Not IsDBNull(testata.Rows(0)("Macchine_Impiegate")) Then
                    Dim macchine_str As String = testata.Rows(0)("Macchine_Impiegate")
                    If Not IsDBNull(macchine_str) AndAlso macchine_str.Trim <> "" Then
                        listMacchine = macchine_str.Split("|").AsQueryable.Where(Function(el As String) el IsNot Nothing AndAlso el <> "").ToList
                    End If
                End If
            End If

            For Each rows In Dt.Rows
                If listMacchine.Contains(rows("chiave")) Then
                    rows("Selected") = True
                    If rows("Targa") <> "" Then
                        targhe_list.Add(rows("Targa").ToString.Replace(" ", ""))
                    End If
                Else
                    rows("Selected") = False
                End If
            Next

            'Controllo le targhe solo dei tipi che la richiedono obbligatoria
            Dim DT_Possessi As DataTable = Nothing
            If targhe_list.Count > 0 Then
                DT_Possessi = objParcoMacchine.Leggi_Possesso_x_Targa(targhe_list,
                                                                      piva,
                                                                      GetFiltroAggiuntivoValiditaParcoMacchine("pm", anno),
                                                                      objParametri_Server)
            End If

            For Each rows In Dt.Rows

                'Mi segno se la macchina corrente è fra i tipi che devono avere la targa obbligatoria
                'Riporto eventualmente la stringa dei possessi

                If ClassCode_Targa_Obbligatoria.Contains(rows("CLASS_CODE")) Then

                    If listMacchine.Contains(rows("chiave")) AndAlso Not IsNothing(DT_Possessi) Then
                        'Se la targa risulta registrata su altre imprese, aggiungo il dato
                        Dim dr = DT_Possessi.Select("Targa = '" & Agro_SQL_SaveText(rows("Targa").ToString.Replace(" ", "")) & "'").FirstOrDefault()
                        If dr IsNot Nothing Then
                            Dim possessoStr = dr.Item("Possessi")
                            rows("Possesso") = possessoStr
                        End If
                    End If

                    rows("isTarga_Obbligatoria") = 1
                    rows("Targa_Obbligatoria") = "Sì"

                Else

                    rows("isTarga_Obbligatoria") = 0
                    rows("Targa_Obbligatoria") = "No"

                End If

            Next

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            r.RispostaStringa = JsonConvert.SerializeObject(Dt, Formatting.None, serializerSettings)

            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
            AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    Private Shared Function GetFiltroAggiuntivoValiditaParcoMacchine(aliasTabella As String, anno As String) As String
        Return String.Format("{0}.Validita_Inizio <= {1} AND {0}.Validita_Fine >= {2}",
                                                          aliasTabella,
                                                          Agro_SQL_SaveDate("31/12/" & anno),
                                                          Agro_SQL_SaveDate("01/01/" & anno))
    End Function

    Private Shared Sub getFiltro_Macchine_Targhe_Obbligatorie(anno As String, ByRef ClassCode_Targa_Obbligatoria As List(Of String), objParametri_Server As AgronicaCoreParametri)

        Dim xFiltroAggiuntivo As String = ""
        Dim umaSetup = New AgronicaCoreUmaDal.UMASetup_R
        Dim DT = umaSetup.LeggiSetup(anno, objParametri_Server)

        If DT.Rows.Count > 0 Then
            If Not IsDBNull(DT.Rows(0).Item("Macchine_Targa_Obbligatoria")) AndAlso DT.Rows(0).Item("Macchine_Targa_Obbligatoria") <> "" Then
                Dim filtro_LIKE As New List(Of String)
                Dim filtro_IN As New List(Of String)

                Dim ClassCode_list As String() = DT.Rows(0).Item("Macchine_Targa_Obbligatoria").split("|")

                If ClassCode_list IsNot Nothing AndAlso ClassCode_list.Count > 0 Then
                    For Each classCode In ClassCode_list
                        If classCode.Contains("%") Then
                            filtro_LIKE.Add(" CLASS_CODE LIKE '" & classCode & "' ")
                        Else
                            filtro_IN.Add(classCode)
                        End If
                    Next
                End If

                If filtro_IN.Count > 0 Then
                    xFiltroAggiuntivo = " CLASS_CODE IN ('" & Agro_SQL_Save_Clausola_IN(String.Join("', '", filtro_IN), True) & "')"
                End If

                If filtro_LIKE.Count > 0 Then
                    If xFiltroAggiuntivo <> "" Then
                        xFiltroAggiuntivo &= " OR "
                    End If
                    xFiltroAggiuntivo &= String.Join(" OR ", filtro_LIKE)
                End If


                If xFiltroAggiuntivo <> "" Then
                    xFiltroAggiuntivo = "( " & xFiltroAggiuntivo & " )"
                End If

                Dim objMacchine As New AgronicaCoreMetaSchemaDAL.Macchine_R
                Dim DT_ClassCode = objMacchine.Leggi("", objParametri_Server, xFiltroAggiuntivo)
                For Each row In DT_ClassCode.Rows
                    ClassCode_Targa_Obbligatoria.Add(row.item("Class_Code"))
                Next
            End If
        End If
    End Sub

    <WebMethod(EnableSession:=True)>
    Public Shared Function Aggiorna_Macchine(ByVal piva As String, richiesta_cod As Integer, strMacchine As String) As RispostaStandard

        Dim r As New RispostaStandard
        Dim Dt As New DataTable
        Dim Dtres As New DataTable
        Dim leggiLavorazioni As New AgronicaCoreContabDAL.Parco_Macchine_R

        Try

            Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
                r.Sessione = False
                Return r
            End If

            Lingua.Gias_InizializzaCultura_DaSession()

            Dim Mac_Arr = JArray.Parse(strMacchine)
            Dim Str_Mac_Arr = ""
            For Each el In Mac_Arr
                Str_Mac_Arr &= el.ToString & "|"
            Next
            If Str_Mac_Arr.Trim <> "" Then
                Str_Mac_Arr = Str_Mac_Arr.Substring(0, Str_Mac_Arr.Length - 1)
            End If



            Dim modTestata As New AgronicaCoreUmaDal.UMA_Richieste_Testata_W
            modTestata.Modifica_Campo_Richiesta("Macchine_Impiegate", Str_Mac_Arr, piva, richiesta_cod, objParametri_Server)

            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
            AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Controllo_Congruenza_Rendicontazione_Terzista(piva As String, programmazione_cod As Integer,
                                                                         macrouso_UMA As String, lav_UMA As Integer,
                                                                         anno As Integer, terzista_CUAA As String) As RispostaStandard

        Dim nomeRoutine As String = "Controllo_Congruenza_Rendicontazione_Terzista()"

        Dim res As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            res.Sessione = False
            Return res
        End If

        Dim leggiCuaa = New AgronicaCoreAnagrafeDAL.Imprese_Codici_Read
        Dim pivaTerzista = leggiCuaa.Piva_from_CUAA(terzista_CUAA, objParametri_Server)
        Dim listaLavorazioniEffettuate As List(Of Integer)
        Dim serializerSettings As New JsonSerializerSettings()
        serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore

        Try

            Dim gefutils As New Gias_EF_Utility
            Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri_Server.StringaConnessione)
            Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

                listaLavorazioniEffettuate = (From t In GiasContext.UMA_Richieste_Testata
                                              Join p In GiasContext.Pratiche
                                                    On t.Pratica_Cod Equals p.Pratica_Cod
                                              Join l In GiasContext.UMA_Richieste_Lavorazioni
                                                    On t.Richiesta_Cod Equals l.Richiesta_Cod
                                              Where t.Piva = pivaTerzista AndAlso
                                                        p.Anno = anno AndAlso
                                                        l.Piva = piva AndAlso
                                                        t.Avanzamento_Richiesta = 1 AndAlso
                                                        t.Tipo_Richiesta = -1 AndAlso
                                                        l.Programmazione_Cod = programmazione_cod AndAlso
                                                        l.Gruppo_Colturale_UMA = macrouso_UMA AndAlso
                                                        l.Lavorazione_UMA = lav_UMA
                                              Select t.Richiesta_Cod).ToList()

            End Using

            res.RispostaStringa = JsonConvert.SerializeObject(listaLavorazioniEffettuate.Count, Formatting.None, serializerSettings)
            res.RispostaOK = True

        Catch ex As Exception

            res.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            res.Errore = "Errore durante l'operazione: " & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return res

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function get_ListaCUAA_Richiesti(piva_coop As String, anno As Integer, tipo_azienda As Integer) As RispostaStandard

        Dim nomeRoutine As String = "get_ListaCUAA_Richiesti()"

        Dim res As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            res.Sessione = False
            Return res
        End If
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Utenti) Then
            res.Sessione = False
            Return res
        End If

        Dim serializerSettings As New JsonSerializerSettings()
        serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore

        Dim listaCUAARichesti As List(Of String)
        Try

            Dim gefutils As New Gias_EF_Utility
            Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri_Server.StringaConnessione)
            Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

                Dim listaRichiesteEffettuate = (From t In GiasContext.UMA_Richieste_Testata
                                                Join p In GiasContext.Pratiche
                                                 On t.Pratica_Cod Equals p.Pratica_Cod
                                                Join psa In GiasContext.Pratiche_Stati_Attuali
                                                 On p.Pratica_Cod Equals psa.Pratica_Cod
                                                Join wa In GiasContext.WAnagraficaStati
                                                 On psa.Stato_Cod Equals wa.WAnagraficaStati_Cod
                                                Where t.Tipo_Azienda = tipo_azienda AndAlso
                                                      p.Anno = anno AndAlso
                                                      t.Piva = piva_coop AndAlso
                                                 t.Avanzamento_Richiesta = 0 AndAlso
                                                    wa.WAnagraficaStati_Cod = enum_WAnagraficaStati.Verifica_Intermedia_Completata_Con_Successo
                                                Select t.Richiesta_Cod).ToList()


                listaCUAARichesti = (From r In GiasContext.UMA_Richieste
                                     Join ic In GiasContext.Imprese_Codici
                                             On r.Piva Equals ic.PIVA
                                     Where listaRichiesteEffettuate.Contains(r.Richiesta_Cod) AndAlso
                                            ic.id_cod = enum_CodiciAnagrafe.CodiceCUAA
                                     Select ic.val_cod).ToList()
            End Using


            res.RispostaStringa = JsonConvert.SerializeObject(listaCUAARichesti, Formatting.None, serializerSettings)
            res.RispostaOK = True

        Catch ex As Exception

            res.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            res.Errore = "Errore durante l'operazione: " & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return res

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Leggi_Fascicoli(ByVal piva As String, ByVal anno As Integer) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Utenti) Then
            r.Sessione = False
            Return r
        End If

        Dim data_da = AGRODATAINIZIO
        Dim data_a = AGRODATAFINE

        Try

            If piva = "" OrElse piva.Length < 8 Then
                r.RispostaOK = True
                r.RispostaStringa = "[ ]"
                Return r
            End If

            Dim lista As New List(Of String)


            Dim Dt_Schede As New DataTable
            Dim DrScheda As DataRow

            Dt_Schede.Columns.Add(New DataColumn("Scheda", GetType(String)))
            Dt_Schede.Columns.Add(New DataColumn("Validita_Inizio", GetType(Date)))
            Dt_Schede.Columns.Add(New DataColumn("Validita_Fine", GetType(Date)))
            Dt_Schede.Columns.Add(New DataColumn("Data", GetType(String)))
            Dt_Schede.Columns.Add(New DataColumn("DataFine", GetType(String)))
            Dt_Schede.Columns.Add(New DataColumn("Origine", GetType(String))) 'S=SIGPA G=Gias (solo Gias)
            Dt_Schede.Columns.Add(New DataColumn("Presente", GetType(String))) '0= nuova ; 1= già presente
            Dt_Schede.Columns.Add(New DataColumn("strProgrammazioniCod", GetType(String))) 'Programmazione_Cod|Programmazione_Cod\Programmazione_Cod se presente (potrebbe essere stata scaricata diverse volte)
            Dt_Schede.Columns.Add(New DataColumn("strProgrammazioniCod_Movimentate", GetType(String))) 'Programmazione_Cod|Programmazione_Cod\Programmazione_Cod se presente (potrebbe essere stata scaricata diverse volte)
            Dt_Schede.Columns.Add(New DataColumn("strProgrammazioniCod_NONMovimentate", GetType(String))) 'Programmazione_Cod|Programmazione_Cod\Programmazione_Cod se presente (potrebbe essere stata scaricata diverse volte)
            Dt_Schede.Columns.Add(New DataColumn("strProgrammazioniDes", GetType(String)))
            Dt_Schede.Columns.Add(New DataColumn("chiaveKendoCache", GetType(String)))

            Dt_Schede.Columns.Add(New DataColumn("idOPR", GetType(Integer)))
            Dt_Schede.Columns.Add(New DataColumn("desOPR", GetType(String)))
            Dt_Schede.Columns.Add(New DataColumn("OrigineOpr", GetType(String)))
            Dt_Schede.Columns.Add(New DataColumn("fonte_cod", GetType(Integer)))
            Dt_Schede.Columns.Add(New DataColumn("pratica_cod", GetType(String)))
            Dt_Schede.Columns.Add(New DataColumn("Tipo_Pianificazione", GetType(String)))

            Dim N_Schede As Integer = 0


            'AGGIUNGO LE SCHEDE PRESENTI SOLO SU GIAS
            Dim FiltroSchede As String = " Allegati_Documenti_CatCod = " & enum_CategorieDocumenti.DomandaFascicolo & " AND Allegati_Documenti_Piva='" & piva & "' AND Allegati_Documenti_Ente_Cod NOT IN (42, 43, 44, 45) "
            Dim FiltroPlanning As String = " Programmazione_Cod<>0 AND Programmazione_Entita_Cod=0 "
            Dim Allegati_Documenti_R As New AgronicaCoreAnagrafeDAL.Allegati_Documenti_R
            Dim Allegati_EntitaxDocumenti_R As New AgronicaCoreAnagrafeDAL.Allegati_EntitaxDocumenti_R
            Dim Reg_Impianti_Programmazioni_R As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Programmazioni_R
            Dim strPlanning As String = ""
            Dim strPlanning_Movimentati As String = ""
            Dim strPlanning_NONMovimentati As String = ""
            Dim strProgrammazioneDes As String = ""
            Dim DtMov As New DataTable
            Dim objProgrammazione As New AgronicaCoreAnagrafeDAL.Programmazione_Testata_R

            Dim Dt_Schede_Gias As New DataTable
            Dim Dt_SchedeOrd As New DataTable
            Dim DT_Programmazione As New DataTable

            Dim strFiltroAnno As String = anno.ToString

            Dim DT = objProgrammazione.Leggi("", 0, piva, "", 1, AGRODATAINIZIO, AGRODATAFINE,
                                             enum_TipoPianificazione.Pianificazione_Annuale,
                                             enumSelezioneVariabile.Selezione_TabellaCompleta,
                                             " ( year(a.Validazione_Data) IN ( " & strFiltroAnno & " ) 
                                                 OR year(tt.Validita_Inizio) IN ( " & strFiltroAnno & " ) ) AND (tt.importato_automaticamente = 1) AND year(tt.Validita_Fine) < 2025", "", objParametri_Server)

            Dim DTNonImportati = objProgrammazione.Leggi("", 0, piva, "", 1, AGRODATAINIZIO, AGRODATAFINE,
                                             enum_TipoPianificazione.Pianificazione_Annuale,
                                             enumSelezioneVariabile.Selezione_TabellaCompleta,
                                             " ( year(a.Validazione_Data) IN ( " & strFiltroAnno & " ) 
                                                 OR year(tt.Validita_Inizio) IN ( " & strFiltroAnno & " ) ) AND (tt.importato_automaticamente = 0 or tt.importato_automaticamente is null) AND year(tt.Validita_Fine) < 2025", "", objParametri_Server)

            For Each row In DTNonImportati.Rows
                Dim scheda = CStr(row("Allegati_Documenti_Numero"))
                Dim dr_schedeimportate = DT.Select(" Allegati_Documenti_Numero = '" & scheda & "' ")

                If dr_schedeimportate.Length = 0 Then
                    DT.ImportRow(row)
                End If

            Next



            For Each row As DataRow In DT.Rows
                Dim DT_Pr = Dt_Schede.Select(" strProgrammazioniCod like '%|" & CStr(row.Item("Programmazione_Cod")) & "|%' ")
                If DT_Pr.Length = 0 Then
                    DrScheda = Dt_Schede.NewRow
                    If Not IsDBNull(row.Item("Allegati_Documenti_Numero")) Then
                        DrScheda.Item("Scheda") = row.Item("Allegati_Documenti_Numero")
                    Else
                        DrScheda.Item("Scheda") = ""
                    End If
                    If Not IsDBNull(row.Item("Validazione_Data")) Then
                        DrScheda.Item("Validita_Inizio") = CDate(row.Item("Validazione_Data")).ToShortDateString
                    Else
                        DrScheda.Item("Validita_Inizio") = CDate(row.Item("Data_Creazione")).ToShortDateString
                    End If

                    strPlanning = ""
                    strPlanning_Movimentati = ""
                    strPlanning_NONMovimentati = ""
                    strProgrammazioneDes = ""

                    '---------------------------------------------------------------------------
                    'VERIFICO se della programmazione sono stati MOVIMENTATI impianti ribaltati 
                    'in tal caso non modifico nulla
                    '---------------------------------------------------------------------------
                    DtMov = Reg_Impianti_Programmazioni_R.Leggi_OperazioniImpiantiRibaltati_DaProgrammazione("", row.Item("Programmazione_Cod"), 0, "", "", objParametri_Server)

                    'MOVIMENTATO 
                    If DtMov.Rows.Count <> 0 Then
                        strPlanning_Movimentati &= "|" & row.Item("Programmazione_Cod") & "|"
                    Else
                        strPlanning_NONMovimentati &= "|" & row.Item("Programmazione_Cod") & "|"
                    End If

                    If Not IsDBNull(row.Item("Validazione_Data")) Then
                        DrScheda.Item("Data") = CDate(row.Item("Validazione_Data")).ToShortDateString
                    Else
                        DrScheda.Item("Data") = CDate(row.Item("Data_Creazione")).ToShortDateString
                    End If

                    DrScheda.Item("Origine") = "P"
                    DrScheda.Item("Presente") = "SI"
                    DrScheda.Item("strProgrammazioniCod") = CStr(row.Item("Programmazione_Cod"))
                    DrScheda.Item("strProgrammazioniCod_Movimentate") = strPlanning_Movimentati
                    DrScheda.Item("strProgrammazioniCod_NONMovimentate") = strPlanning_NONMovimentati
                    DrScheda.Item("Tipo_Pianificazione") = row("Tipo_Pianificazione")
                    If Not IsDBNull(row.Item("Pratica_Cod")) Then
                        DrScheda.Item("Pratica_Cod") = row.Item("Pratica_Cod")
                    Else
                        DrScheda.Item("Pratica_Cod") = ""
                    End If

                    DrScheda.Item("idOPR") = 0
                    DrScheda.Item("OrigineOpr") = ""
                    If Not IsDBNull(row.Item("fonte_cod")) Then
                        DrScheda.Item("fonte_cod") = CStr(row.Item("fonte_cod"))
                    Else
                        DrScheda.Item("fonte_cod") = "0"
                    End If
                    DrScheda.Item("strProgrammazioniDes") = CStr(row.Item("Programmazione_Des"))

                    DrScheda.Item("chiaveKendoCache") = "p" & CStr(row.Item("Programmazione_Cod"))
                    'If Not IsDBNull(row.Item("fonte_cod")) Then
                    '    Dim fonte_cod1 As enum_Planning_Fonte = row.Item("fonte_cod")
                    '    If fonte_cod1 = enum_Planning_Fonte.Agrea Then
                    '        DrScheda.Item("strProgrammazioniDes") = CStr(row.Item("Programmazione_Des")) & " del " & CDate(row.Item("Data_Creazione")).ToShortDateString()

                    '    End If
                    'End If

                    Dim fonte_des = ""
                    If Not IsDBNull(row.Item("fonte_cod")) Then
                        Dim fonte_cod As enum_Planning_Fonte = row.Item("fonte_cod")
                        Select Case fonte_cod
                            Case enum_Planning_Fonte.Agrea
                                fonte_des = "AGREA"
                            Case enum_Planning_Fonte.AnagrafeER
                                fonte_des = "ANAGRAFE ER"
                            Case enum_Planning_Fonte.Arpea
                                fonte_des = "ARPEA"
                            Case enum_Planning_Fonte.Artea
                                fonte_des = "ARTEA"
                            Case enum_Planning_Fonte.Avepa
                                fonte_des = "AVEPA"
                            Case enum_Planning_Fonte.AvepaB1
                                fonte_des = "AVEPA B1"
                            Case enum_Planning_Fonte.AvepaOracle
                                fonte_des = "AVEPA ORACLE"
                            Case enum_Planning_Fonte.BluArancioAgea
                                fonte_des = "AGEA"
                            Case enum_Planning_Fonte.BluArancioAvepa
                                fonte_des = "AVEPA BA"
                            Case enum_Planning_Fonte.Brogliaccio_SIAN
                                fonte_des = "BROGLIACCIO"
                            Case enum_Planning_Fonte.NotificaBioER
                                fonte_des = "BIO ER"
                            Case enum_Planning_Fonte.Sisco
                                fonte_des = "SISCO"
                            Case 20
                                fonte_des = "AGREA"
                        End Select
                    End If
                    fonte_des = ""
                    DrScheda.Item("desOPR") = fonte_des
                    DrScheda.Item("OrigineOpr") = fonte_des
                    Dt_Schede.Rows.Add(DrScheda)
                End If
            Next

            If Dt_Schede.Rows.Count > 0 Then

                Dim dataView As New DataView(Dt_Schede) With {
                    .Sort = " Validita_Inizio DESC, Scheda DESC"
                }
                Dt_SchedeOrd = dataView.ToTable

                If Dt_SchedeOrd.Rows.Count > 1 Then
                    For s = 0 To Dt_SchedeOrd.Rows.Count - 1
                        If IsDBNull(Dt_SchedeOrd.Rows(s).Item("Data")) Then
                            Dt_SchedeOrd.Rows(s).Item("Data") = CDate(Dt_SchedeOrd.Rows(s).Item("Validita_inizio")).ToShortDateString
                        End If
                    Next
                    For s = 0 To Dt_SchedeOrd.Rows.Count - 2
                        Dt_SchedeOrd.Rows(s).Item("Validita_Fine") = DateAdd(DateInterval.Day, -1, CDate(Dt_SchedeOrd.Rows(s + 1).Item("Validita_Inizio"))).ToShortDateString
                        If IsDBNull(Dt_SchedeOrd.Rows(s + 1).Item("Data")) Then
                            Dt_SchedeOrd.Rows(s).Item("Data") = Dt_SchedeOrd.Rows(s).Item("Validita_inizio")
                        End If
                        Dt_SchedeOrd.Rows(s).Item("DataFine") = DateAdd(DateInterval.Day, -1, CDate(Dt_SchedeOrd.Rows(s + 1).Item("Data"))).ToShortDateString
                    Next
                End If

            End If

            Dim DT_SchedeSplit = Dt_SchedeOrd.Clone

            For Each row As DataRow In Dt_SchedeOrd.Rows
                If IsDBNull(row("strProgrammazioniCod")) Then
                    row("strProgrammazioniCod") = ""
                End If
                If row("strProgrammazioniCod") = "" Then
                    Dim drNew = DT_SchedeSplit.NewRow()
                    drNew.ItemArray = row.ItemArray
                    DT_SchedeSplit.Rows.Add(drNew)
                    'DT_SchedeSplit.Rows.Add(row.ItemArray.Clone())
                Else
                    Dim strProgrammazioniCod_Elems = row("strProgrammazioniCod").ToString.Split("|")
                    For Each ActualProg In strProgrammazioniCod_Elems
                        If IsNumeric(ActualProg) Then

                            Dim drNew = DT_SchedeSplit.NewRow()
                            drNew.ItemArray = row.ItemArray

                            Dim strProgrammazioniCod = "|" & ActualProg & "|"
                            Dim strProgrammazioniCod_Movimentate = ""
                            Dim strProgrammazioniCod_NONMovimentate = ""
                            If row("strProgrammazioniCod_Movimentate").ToString.Contains("|" & ActualProg & "|") Then
                                strProgrammazioniCod_Movimentate = "|" & ActualProg & "|"
                                strProgrammazioniCod_NONMovimentate = ""
                            ElseIf row("strProgrammazioniCod_NONMovimentate").ToString.Contains("|" & ActualProg & "|") Then
                                strProgrammazioniCod_Movimentate = ""
                                strProgrammazioniCod_NONMovimentate = "|" & ActualProg & "|"
                            End If

                            drNew("strProgrammazioniCod") = strProgrammazioniCod
                            drNew("strProgrammazioniCod_Movimentate") = strProgrammazioniCod_Movimentate
                            drNew("strProgrammazioniCod_NONMovimentate") = strProgrammazioniCod_NONMovimentate
                            DT_SchedeSplit.Rows.Add(drNew)

                            'Dim rowCopied = row.ItemArray.Clone()
                            'DT_SchedeSplit.Rows.Add(rowCopied)
                        End If
                    Next
                End If
            Next

            If DT_SchedeSplit.Rows.Count > 0 Then

                Dim dataViewSplit As New DataView(DT_SchedeSplit) With {
                        .Sort = " Validita_Inizio DESC, Scheda DESC"
                    }
                DT_SchedeSplit = dataViewSplit.ToTable
            End If


            For s = 0 To DT_SchedeSplit.Rows.Count - 1
                If IsDBNull(DT_SchedeSplit.Rows(s).Item("strProgrammazioniDes")) Then
                    DT_SchedeSplit.Rows(s).Item("strProgrammazioniDes") = ""
                End If

                If IsDBNull(DT_SchedeSplit.Rows(s).Item("chiaveKendoCache")) Then
                    DT_SchedeSplit.Rows(s).Item("chiaveKendoCache") = ""
                End If

                If IsDBNull(DT_SchedeSplit.Rows(s).Item("Pratica_Cod")) Then
                    DT_SchedeSplit.Rows(s).Item("Pratica_Cod") = ""
                End If

                If IsDBNull(DT_SchedeSplit.Rows(s).Item("strProgrammazioniCod")) Then
                    DT_SchedeSplit.Rows(s).Item("strProgrammazioniCod") = ""
                End If

                If IsDBNull(DT_SchedeSplit.Rows(s).Item("strProgrammazioniCod_Movimentate")) Then
                    DT_SchedeSplit.Rows(s).Item("strProgrammazioniCod_Movimentate") = ""
                End If

                If IsDBNull(DT_SchedeSplit.Rows(s).Item("strProgrammazioniCod_NONMovimentate")) Then
                    DT_SchedeSplit.Rows(s).Item("strProgrammazioniCod_NONMovimentate") = ""
                End If

                If IsDBNull(DT_SchedeSplit.Rows(s).Item("Tipo_Pianificazione")) Then
                    DT_SchedeSplit.Rows(s).Item("Tipo_Pianificazione") = "0"
                End If

                If IsDBNull(DT_SchedeSplit.Rows(s).Item("Scheda")) Or DT_SchedeSplit.Rows(s).Item("Scheda") = "" Then
                    DT_SchedeSplit.Rows(s).Item("Scheda") = DT_SchedeSplit.Rows(s).Item("strProgrammazioniDes")
                End If

                lista.Add(
                    "{""text"":""" & jSon.Escape(DT_SchedeSplit.Rows(s).Item("Scheda") & " (" & DT_SchedeSplit.Rows(s).Item("Data") & ") - " & DT_SchedeSplit.Rows(s).Item("desOPR")) & """, " &
                    """value"": """ & jSon.Escape(DT_SchedeSplit.Rows(s).Item("Scheda") & "|" & DT_SchedeSplit.Rows(s).Item("origineOPR")) & """ , " &
                    """n_validazione"": """ & jSon.Escape(DT_SchedeSplit.Rows(s).Item("Scheda")) & """ , " &
                    """data_validazione"": """ & jSon.Escape(DT_SchedeSplit.Rows(s).Item("Data") & "") & """ , " &
                    """ente_validatore"": """ & jSon.Escape(DT_SchedeSplit.Rows(s).Item("origineOPR")) & """, " &
                    """fonte_cod"": """ & jSon.Escape(DT_SchedeSplit.Rows(s).Item("fonte_cod")) & """, " &
                    """strProgrammazioniCod"": """ & jSon.Escape(DT_SchedeSplit.Rows(s).Item("strProgrammazioniCod")) & """, " &
                    """strProgrammazioniCod_Movimentate"": """ & jSon.Escape(DT_SchedeSplit.Rows(s).Item("strProgrammazioniCod_Movimentate")) & """, " &
                    """strProgrammazioniCod_NONMovimentate"": """ & jSon.Escape(DT_SchedeSplit.Rows(s).Item("strProgrammazioniCod_NONMovimentate")) & """, " &
                    """strProgrammazioniDes"": """ & jSon.Escape(DT_SchedeSplit.Rows(s).Item("strProgrammazioniDes")) & """, " &
                    """Pratica_Cod"": """ & jSon.Escape(DT_SchedeSplit.Rows(s).Item("Pratica_Cod")) & """, " &
                    """Tipo_Pianificazione"": """ & jSon.Escape(DT_SchedeSplit.Rows(s).Item("Tipo_Pianificazione")) & """, " &
                    """chiaveKendoCache"": """ & jSon.Escape(DT_SchedeSplit.Rows(s).Item("chiaveKendoCache")) & """} "
                    )
            Next




            Dim strRisp As String = "[" & String.Join(",", lista.Distinct.ToList) & "]"

            r.RispostaOK = True
            r.RispostaStringa = strRisp

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function LavNoteObbligatorie() As RispostaStandard

        Dim r As New RispostaStandard
        Dim configurazioneLav As New UMA_Configurazione_MacrousixLavorazioni_R
        Dim dtConfig As New DataTable

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Utenti) Then
            r.Sessione = False
            Return r
        End If

        Dim umaSetup = New AgronicaCoreUmaDal.UMASetup_R

        Try

            dtConfig = configurazioneLav.Leggi("", "", "", 0, 0, objParametri_Server, " FlagNoteCompObbl = 1 ")

            r.RispostaOK = True
            r.RispostaStringa = JsonConvert.SerializeObject(dtConfig, Formatting.None)

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function LeggiUMA_Macrousi(ByVal Programmazione_Cod As Integer,
                                             ByVal Anno As String,
                                             ByVal CUAA As String,
                                             ByVal Piva As String,
                                             ByVal Avanzamento As Integer,
                                             ByVal Terzista As Integer) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Utenti) Then
            r.Sessione = False
            Return r
        End If

        Dim validitaAnno = GetValiditaAnno(Anno)
        Dim umaSetup = New AgronicaCoreUmaDal.UMASetup_R

        Try

            If Programmazione_Cod = CodiceFascicoloColtureNonImputabili Then
                Dim uma_macrousi As New AgronicaCoreMetaSchemaDAL.UMA_Macrousi_R
                Dim listaMacrousiNonFascicolo As New List(Of String)({
                                                                     MacrousoCod_ColtivazioniSottoSerra,
                                                                     MacrousoCod_TrasformazioneLatte,
                                                                     MacrousoCod_TrasformazioneOliveOlio,
                                                                     MacrousoCod_TrasformazioneCarciofi,
                                                                     MacrousoCod_TrasformazioneProdottiOrtofrutticoli,
                                                                     MacrousoCod_ConsorziBonificaIrrigazione,
                                                                     MacrousoCod_LavoriStraordinari
                                                                     })
                Dim macrousiList = uma_macrousi.Leggi("",
                                                      objParametri_Server,
                                                      validitaInizio:=validitaAnno.Inizio,
                                                      validitaFine:=validitaAnno.Fine).Where(Function(el) listaMacrousiNonFascicolo.Contains(el.Macrouso_UMA_Cod)).ToList
                r.RispostaOK = True
                r.RispostaStringa = JsonConvert.SerializeObject(macrousiList, Formatting.None)
            ElseIf Programmazione_Cod = CodiceFascicoloAnticipi Then
                Dim uma_macrousi As New AgronicaCoreMetaSchemaDAL.UMA_Macrousi_R
                Dim listaMacrousiNonFascicolo As New List(Of String)({MacrousoCod_EccedenzaAnticipi, MacrousoCod_AnticipazioniColturali})
                Dim macrousiList = uma_macrousi.Leggi("",
                                                      objParametri_Server,
                                                      validitaInizio:=validitaAnno.Inizio,
                                                      validitaFine:=validitaAnno.Fine).Where(Function(el) listaMacrousiNonFascicolo.Contains(el.Macrouso_UMA_Cod)).ToList
                If macrousiList.Count > 0 AndAlso Avanzamento = 1 Then
                    Dim leggiLavorazioni As New AgronicaCoreUmaDal.UMA_Richieste_Lavorazioni_R
                    Dim dtRichiesteConEccedenzeAnticipo = leggiLavorazioni.Leggi_Lavorazioni(Piva,
                                                                                             Anno,
                                                                                             enum_UMA_Avanzamento.Richiesta,
                                                                                             Terzista,
                                                                                             MacrousoCod_EccedenzaAnticipi,
                                                                                             lavorazioneCod_QuotaAnticipoEccedente,
                                                                                             CodiceFascicoloAnticipi,
                                                                                             enum_WAnagraficaStati.Verifica_Intermedia_Completata_Con_Successo,
                                                                                             objParametri_Server)
                    If Not IsNothing(dtRichiesteConEccedenzeAnticipo) AndAlso dtRichiesteConEccedenzeAnticipo.Rows.Count = 0 Then
                        Dim macrousiListFiltrato = New List(Of AgronicaCoreEntityFramework_POCO.UMA_Macrousi)
                        For Each elem In macrousiList
                            If elem.Macrouso_UMA_Cod <> MacrousoCod_EccedenzaAnticipi Then
                                macrousiListFiltrato.Add(elem)
                            End If
                        Next
                        macrousiList = macrousiListFiltrato
                    End If

                End If

                Dim setup = umaSetup.LeggiSetup(Anno, objParametri_Server)
                Dim antColt = setup.Rows.Item(0).Item("Gestione_Anticipazioni_Colturali")

                If Avanzamento = 0 OrElse (antColt <> 0 AndAlso ((Terzista = -1 AndAlso antColt <> 2) OrElse
                    (Terzista = 0 AndAlso antColt <> 1))) Then

                    macrousiList.Remove(macrousiList.Find(Function(elem As AgronicaCoreEntityFramework_POCO.UMA_Macrousi)
                                                              Return elem.Macrouso_UMA_Cod = "9997"
                                                          End Function)
                                       )

                End If

                r.RispostaOK = True
                r.RispostaStringa = JsonConvert.SerializeObject(macrousiList, Formatting.None)
            ElseIf Programmazione_Cod = CodiceFascicoloTrasferimenti Then
                Dim uma_macrousi As New AgronicaCoreMetaSchemaDAL.UMA_Macrousi_R
                Dim listaMacrousiNonFascicolo As New List(Of String)({MacrousoCod_TrasferimentiEffettuati})
                Dim macrousiList = uma_macrousi.Leggi("",
                                                      objParametri_Server,
                                                      validitaInizio:=validitaAnno.Inizio,
                                                      validitaFine:=validitaAnno.Fine).Where(Function(el) listaMacrousiNonFascicolo.Contains(el.Macrouso_UMA_Cod)).ToList

                If macrousiList.Count > 0 AndAlso Not IsNothing(CUAA) Then
                    Dim leggiVendite As New AgronicaCoreUmaDal.UMA_Vendite_R
                    Dim filtroAggiuntivoCuaa = "NOTE_RIVENDITORE = 'RICEVUTO DA " & CUAA.Trim() & "'"
                    Dim dtTrasferimento = leggiVendite.LeggiCarburante("F1999998826",
                                                                       "",
                                                                       Anno,
                                                                       -999,
                                                                       filtroAggiuntivoCuaa,
                                                                       objParametri_Server)
                    If Not IsNothing(dtTrasferimento) AndAlso dtTrasferimento.Rows.Count = 0 Then
                        macrousiList = New List(Of AgronicaCoreEntityFramework_POCO.UMA_Macrousi)
                    End If
                End If

                r.RispostaOK = True
                r.RispostaStringa = JsonConvert.SerializeObject(macrousiList, Formatting.None)
            Else
                r.RispostaOK = True
                r.RispostaStringa = "[]"
            End If

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Assegna_Automaticamente_Carburante(ByVal piva As String, richiesta_cod As Integer, Percentuale_Decurtamento As Double, lav_Parziali As Boolean) As RispostaStandard

        Dim r As New RispostaStandard
        Dim Dt As New DataTable
        Dim Dtres As New DataTable

        Try

            Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
                r.Sessione = False
                Return r
            End If

            Lingua.Gias_InizializzaCultura_DaSession()

            Dim objeUma_Richieste As New AgronicaCoreUmaBiz.UMA_Richieste
            objeUma_Richieste.AssegnaAutomaticamenteCarburante(piva, richiesta_cod, Percentuale_Decurtamento, lav_Parziali, objParametri_Server)

            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
            AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function CaricaConfigurazioniRichiestaDocumenti(ByVal piva As String, ByVal Richiesta_Cod As Integer) As RispostaStandard
        Return RichiestaDocumenti.CaricaConfigurazioniRichiestaDocumenti(piva, Richiesta_Cod)
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function StampaRichiestaRendicon(
            ByVal piva As String,
            ByVal richiestaAvanz As Integer,
            ByVal richiestaCod As Integer) As RispostaStandard

        Dim r As New RispostaStandard

        Dim serializerSettings As New JsonSerializerSettings()
        serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore

        Dim objAgronicaStampe As New AgronicaCoreGestioneRichieste.ParametriAgronicaStampe
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")

        If richiestaAvanz = 1 Then
            objAgronicaStampe.report = enum_CodificaStampe.UMA_RendicontazioneCarb
        Else
            objAgronicaStampe.report = enum_CodificaStampe.UMA_RichiestaCarbPrevisioneLav
        End If


        Try
            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
                r.Sessione = False
                Return r
            End If

            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Utenti) Then
                r.Sessione = False
                Return r
            End If

            Lingua.Gias_InizializzaCultura_DaSession()

            Dim vVarStampe(1) As ElementoStampe

            vVarStampe(0).Nome = "piva"
            vVarStampe(0).Valore = piva

            vVarStampe(1).Nome = "richiesta_cod"
            vVarStampe(1).Valore = richiestaCod


            Dim objVS As New AgronicaCoreXML.XML_Stampe
            Dim StrNodo As String = objVS.XML_VariabiliStampe(vVarStampe)
            Dim StrNodiVariabili As String = StrNodo

            objAgronicaStampe.username = CStr(HttpContext.Current.Session("ASG_Utente_Username"))
            objAgronicaStampe.user_profilo = CStr(HttpContext.Current.Session("ASG_ProgressivoGIAS"))
            objAgronicaStampe.Xml_Generico.Length = 0
            objAgronicaStampe.Xml_Generico.Append(StrNodiVariabili)

            Dim link = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_SitoStampe_PassandoDirettamente_ParametriAgronicaStampe(
                                                     Enum_SiteRedirector.Sito_AgronicaAgenda_2010, objAgronicaStampe)



            r.RispostaOK = True
            r.RispostaStringa = link

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function StampaIstruttoria(
            ByVal piva As String,
            ByVal richiestaAvanz As Integer,
            ByVal richiestaCod As Integer,
            ByVal modificaDati As Boolean,
            ByVal segnalazioniMacchine As Boolean,
            ByVal esito As Integer,
            ByVal noteEsito As String) As RispostaStandard

        Dim r As New RispostaStandard

        Dim serializerSettings As New JsonSerializerSettings()
        serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore

        Dim objAgronicaStampe As New AgronicaCoreGestioneRichieste.ParametriAgronicaStampe
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")

        If richiestaAvanz = 1 Then
            objAgronicaStampe.report = enum_CodificaStampe.UMA_IstruttoriaRendCarb
        Else
            objAgronicaStampe.report = enum_CodificaStampe.UMA_VerbaleIstruttoriaRichCarb
        End If


        Try
            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
                r.Sessione = False
                Return r
            End If

            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Utenti) Then
                r.Sessione = False
                Return r
            End If

            Lingua.Gias_InizializzaCultura_DaSession()

            Dim vVarStampe(5) As ElementoStampe

            vVarStampe(0).Nome = "piva"
            vVarStampe(0).Valore = piva

            vVarStampe(1).Nome = "richiesta_cod"
            vVarStampe(1).Valore = richiestaCod

            vVarStampe(2).Nome = "modifica_dati"
            vVarStampe(2).Valore = modificaDati

            vVarStampe(3).Nome = "segnalazioni_macchine"
            vVarStampe(3).Valore = segnalazioniMacchine

            vVarStampe(4).Nome = "esito"
            vVarStampe(4).Valore = esito

            vVarStampe(5).Nome = "note_esito"
            vVarStampe(5).Valore = noteEsito


            Dim objVS As New AgronicaCoreXML.XML_Stampe
            Dim StrNodo As String = objVS.XML_VariabiliStampe(vVarStampe)
            Dim StrNodiVariabili As String = StrNodo

            objAgronicaStampe.username = CStr(HttpContext.Current.Session("ASG_Utente_Username"))
            objAgronicaStampe.user_profilo = CStr(HttpContext.Current.Session("ASG_ProgressivoGIAS"))
            objAgronicaStampe.Xml_Generico.Length = 0
            objAgronicaStampe.Xml_Generico.Append(StrNodiVariabili)

            Dim link = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_SitoStampe_PassandoDirettamente_ParametriAgronicaStampe(
                                                     Enum_SiteRedirector.Sito_AgronicaAgenda_2010, objAgronicaStampe)



            r.RispostaOK = True
            r.RispostaStringa = link

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r

    End Function


    <WebMethod(EnableSession:=True)>
    Public Shared Function gestionePassaggioDiStato(Richiesta_Cod As Integer) As RispostaStandard
        Dim r As New RispostaStandard

        Dim serializerSettings As New JsonSerializerSettings()
        serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore

        Dim objAgronicaStampe As New AgronicaCoreGestioneRichieste.ParametriAgronicaStampe
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")

        objAgronicaStampe.report = enum_CodificaStampe.UMA_VerbaleIstruttoriaRichCarb

        Try
            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
                r.Sessione = False
                Return r
            End If

            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Utenti) Then
                r.Sessione = False
                Return r
            End If

            Lingua.Gias_InizializzaCultura_DaSession()

            Dim objUmaRichieste As New AgronicaCoreUmaDal.UMA_Richieste_Testata_R

            Dim dtR = objUmaRichieste.Leggi("", Richiesta_Cod, 0, AGRODATAINIZIO, AGRODATAFINE, objParametri_Server, 0)

            If dtR.Rows.Count = 0 Then
                dtR = objUmaRichieste.Leggi("", Richiesta_Cod, 0, AGRODATAINIZIO, AGRODATAFINE, objParametri_Server, -1)
            End If

            Dim url As String = ""
            If dtR.Rows.Count > 0 Then
                url = AgronicaCoreGestioneRichieste.RedirectGestione.GetLinkPassaggioDiStato(Enum_SiteRedirector.Sito_AgronicaUma, dtR.Rows(0)("Pratica_Cod"), 0)
            End If

            r.RispostaOK = True
            r.RispostaStringa = url

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r
    End Function


    <WebMethod(EnableSession:=True)>
    Public Shared Function LeggiRichiestaTestata(Richiesta_Cod As Integer)

        Dim r As New RispostaStandard

        Dim serializerSettings As New JsonSerializerSettings()
        serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore

        Dim objAgronicaStampe As New AgronicaCoreGestioneRichieste.ParametriAgronicaStampe
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")

        objAgronicaStampe.report = enum_CodificaStampe.UMA_VerbaleIstruttoriaRichCarb

        Try
            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
                r.Sessione = False
                Return r
            End If

            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Utenti) Then
                r.Sessione = False
                Return r
            End If

            Lingua.Gias_InizializzaCultura_DaSession()

            Dim objUmaRichieste As New AgronicaCoreUmaDal.UMA_Richieste_Testata_R

            Dim dtR = objUmaRichieste.Leggi("", Richiesta_Cod, 0, AGRODATAINIZIO, AGRODATAFINE, objParametri_Server, 0)

            If dtR.Rows.Count = 0 Then
                dtR = objUmaRichieste.Leggi("", Richiesta_Cod, 0, AGRODATAINIZIO, AGRODATAFINE, objParametri_Server, -1)
            End If


            r.RispostaStringa = JsonConvert.SerializeObject(dtR, Formatting.None, serializerSettings)

            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function LeggiPraticheSuccessive(ByVal piva As String,
                                                   ByVal richiesta_cod As Integer,
                                                   ByVal avanzamento As Integer)

        Dim r As New RispostaStandard

        Dim serializerSettings As New JsonSerializerSettings()
        serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore

        Dim objAgronicaStampe As New AgronicaCoreGestioneRichieste.ParametriAgronicaStampe
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")

        Try
            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
                r.Sessione = False
                Return r
            End If

            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Utenti) Then
                r.Sessione = False
                Return r
            End If

            Lingua.Gias_InizializzaCultura_DaSession()

            Dim objUmaRichieste As New AgronicaCoreUmaDal.UMA_Richieste_Testata_R

            If avanzamento <> enum_UMA_Avanzamento.Richiesta_Anticipo Then

                Dim dtR = objUmaRichieste.Leggi(piva,
                                richiesta_cod,
                                0,
                                AGRODATAINIZIO,
                                AGRODATAFINE,
                                objParametri_Server)

                If Not IsNothing(dtR) AndAlso dtR.Rows.Count > 0 Then

                    Dim tipoRichiesta As Integer = dtR.Rows(0).Item("tipo_richiesta")

                    Dim filtroAggiuntivo = String.Format("     t.avanzamento_richiesta <> -1 " &
                                                         " and t.tipo_richiesta = {0} " &
                                                         " and t.richiesta_cod <> {1} ",
                                                         tipoRichiesta,
                                                         richiesta_cod)

                    Dim validitaInizio As Date = dtR.Rows(0).Item("validita_inizio")
                    Dim anno As Integer = Year(validitaInizio)

                    Select Case avanzamento

                        Case enum_UMA_Avanzamento.Richiesta
                            Dim seIntegrativa As Integer = dtR.Rows(0).Item("richiesta_integrativa")
                            If seIntegrativa = 0 Then
                                'Verifico se esistono richieste integrative o rendicontazioni chiuse nell'anno corrente
                                filtroAggiuntivo += String.Format(" and year(t.validita_inizio) = {0} " &
                                                                  " and (t.richiesta_integrativa = 1 or avanzamento_richiesta = 1)",
                                                                  anno)
                            Else
                                'Verifico se esistono richieste integrative successive o rendicontazioni chiuse nell'anno corrente
                                filtroAggiuntivo += String.Format(" and year(t.validita_inizio) = {0} " &
                                                                  " and ((t.richiesta_integrativa = 1 and t.richiesta_cod > {1}) or avanzamento_richiesta = 1)",
                                                                  anno,
                                                                  richiesta_cod)
                            End If

                        Case enum_UMA_Avanzamento.Rendicontazione
                            'Verifico se esistono richieste/rendicontazioni chiuse nell'anno successivo
                            filtroAggiuntivo += String.Format(" and year(t.validita_inizio) = {0} ",
                                                              anno + 1)

                    End Select

                    Dim dtSucc = objUmaRichieste.Leggi_Elenco3(piva,
                                                               filtroAggiuntivo,
                                                               "",
                                                               objParametri_Server,
                                                               statoCod:=enum_WAnagraficaStati.Verifica_Intermedia_Completata_Con_Successo)

                    r.RispostaStringa = JsonConvert.SerializeObject(dtSucc, Formatting.None, serializerSettings)

                End If

            End If

            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r

    End Function


    <WebMethod(EnableSession:=True)>
    Public Shared Function Leggi_Termine_Ultimo_Rendicontazione(anno As String,
                                                                tipo_azienda As Integer)

        Dim nomeRoutine As String = "Leggi_Termine_Ultimo_Rendiconatione()"

        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Dim serializerSettings As New JsonSerializerSettings()
        serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore

        Dim abilitato As Boolean = False
        Dim Ultima_Data_Possibile As String = ""
        Try

            Dim gefutils As New Gias_EF_Utility
            Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri_Server.StringaConnessione)
            Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

                Leggi_Termine_Ultimo_Rendicontazione(Ultima_Data_Possibile, abilitato, anno, tipo_azienda, objParametri_Server)

                If String.IsNullOrEmpty(Ultima_Data_Possibile) Then
                    r.RispostaStringa = "Le date di termine ultimo rendicontazione non è stata valorizzata correttamente"
                    r.RispostaOK = False
                Else
                    If Today <= Ultima_Data_Possibile Then
                        abilitato = True
                    End If

                    r.RispostaStringa = JsonConvert.SerializeObject(abilitato & "|" & Ultima_Data_Possibile, Formatting.None, serializerSettings)
                    r.RispostaOK = True
                End If

            End Using

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r
    End Function

    Private Shared Sub Leggi_Termine_Ultimo_Rendicontazione(ByRef Ultima_Data_Possibile As String,
                                                            ByRef abilitato As String,
                                                            anno As String,
                                                            tipo_azienda As Integer,
                                                            objParametri_Server As AgronicaCoreParametri
                                                            )

        Dim nomeRoutine As String = "Leggi_Termine_Ultimo_Rendiconatione()"

        Dim serializerSettings As New JsonSerializerSettings()
        serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore


        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri_Server.StringaConnessione)
        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            Ultima_Data_Possibile = (From x In GiasContext.UMA_Setup_Date_Rendicontazione
                                     Where x.Tipo_Azienda = tipo_azienda AndAlso
                                                  x.Anno_Richiesta = anno
                                     Select x.Termine_Ultimo_Rendicontazione).FirstOrDefault()
        End Using

    End Sub


    <WebMethod(EnableSession:=True)>
    Public Shared Function Leggi_Data_Limite_INS_Azienda(anno As String,
                                                         tipo_azienda As Integer)

        Dim nomeRoutine As String = "Leggi_Data_Limite_INS_Azienda()"

        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Utenti) Then
            r.Sessione = False
            Return r
        End If

        Dim serializerSettings As New JsonSerializerSettings()
        serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore

        Dim abilitato As Boolean = False
        Dim Data_Limite_INS_Azienda As Date?

        Try

            Dim gefutils As New Gias_EF_Utility
            Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri_Server.StringaConnessione)
            Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

                Data_Limite_INS_Azienda = (From x In GiasContext.UMA_Setup_Date_Rendicontazione
                                           Where x.Tipo_Azienda = tipo_azienda AndAlso
                                                      x.Anno_Richiesta = anno
                                           Select x.Data_Limite_INS_Azienda_Terzista).FirstOrDefault()


                If IsNothing(Data_Limite_INS_Azienda) Then
                    abilitato = True
                Else
                    If Today <= CDate(Data_Limite_INS_Azienda) OrElse Data_Limite_INS_Azienda = AGRODATAINIZIO Then
                        abilitato = True
                    End If

                End If
            End Using

            r.RispostaStringa = JsonConvert.SerializeObject(abilitato & "|" & CDate(Data_Limite_INS_Azienda).ToShortDateString(), Formatting.None, serializerSettings)
            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r
    End Function


    <WebMethod(EnableSession:=True)>
    Public Shared Function Controlla_Permessi_Inserimento_Nuovo_Documento(ByVal piva As String)

        Dim r As New RispostaStandard

        Dim serializerSettings As New JsonSerializerSettings()
        serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")

        Dim Utente_Username As String = HttpContext.Current.Session("ASG_Utente_Username")

        Dim IdServizio As Integer = HttpContext.Current.Session("ASG_IdServizio")

        Try
            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
                r.Sessione = False
                Return r
            End If

            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Utenti) Then
                r.Sessione = False
                Return r
            End If

            Lingua.Gias_InizializzaCultura_DaSession()

            Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R

            'Controllo se l'utente ha i permessi per accedere
            Dim bPermessiOk = objPermessi.Controlla_Permessi_Utente(Utente_Username,
                                                                   IdServizio,
                                                                    enum_Security_Attivita.Documentale_Inser,
                                                                    enum_Security_Operazione.Modifica,
                                                                    Date.Now, "", objParametri_Utenti)

            If bPermessiOk Then
                Dim objtipologia As New AgronicaCoreScadenziario.Alert_Tipologia_R
                Dim dt As DataTable = objtipologia.Leggi(enum_ID_Area_Alert.UMA_Carburanti, enum_ID_Area_Tipologia.UMA_dichiarazione_pre_assegnazione,
                                                         piva, False, objParametri_Server, True, Tipo_Permesso_Documentale.Gestione_Completa)

                If IsNothing(dt) OrElse dt.Rows.Count = 0 Then
                    Throw New Exception("Utente non abilitato per l'inserimento della tipologia di documento 'UMA - Dichiarazione Preassegnazione'.")
                End If
            Else

                Throw New Exception("Utente non autorizzato all'inserimento di un nuovo documento.")

            End If

            r.RispostaStringa = ""

            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = ex.Message

        End Try

        Return r

    End Function


    <WebMethod(EnableSession:=True)>
    Public Shared Function Leggi_Date_Ins_Rendicontazione(anno As String,
                                                          tipo_azienda As Integer,
                                                          piva As String,
                                                          richiesta_cod As Integer) As RispostaStandard

        Dim nomeRoutine As String = "Leggi_Date_Ins_Rendicontazione()"

        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Utenti) Then
            r.Sessione = False
            Return r
        End If

        Dim serializerSettings As New JsonSerializerSettings()
        serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore

        Dim abilitato As Boolean = False
        Dim Data_Inizio As String = ""
        Dim Data_Fine As String = ""
        Const DataNonTrovata = "00:00:00"

        Try

            Dim gefutils As New Gias_EF_Utility
            Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri_Server.StringaConnessione)
            Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

                Leggi_Date_Ins_Rendicontazione(Data_Inizio, Data_Fine, anno, tipo_azienda, piva, richiesta_cod, objParametri_Server)

                If String.IsNullOrEmpty(Data_Fine) OrElse String.IsNullOrEmpty(Data_Inizio) OrElse Data_Fine = DataNonTrovata OrElse Data_Inizio = DataNonTrovata Then
                    r.RispostaStringa = "Le date di inizio e fine inserimento rendicontazione non sono state valorizzate correttamente"
                    r.RispostaOK = False
                Else
                    If Today <= Data_Fine AndAlso Today >= Data_Inizio Then
                        abilitato = True
                    End If

                    r.RispostaStringa = JsonConvert.SerializeObject(abilitato & "|" & Data_Inizio & "|" & Data_Fine, Formatting.None, serializerSettings)
                    r.RispostaOK = True
                End If
            End Using

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function controllaPermesso_InserimentoRendicontazione_ContoProprio(anno As String, piva As String)

        Dim nomeRoutine As String = "controllaPermesso_InserimentoRendicontazione_ContoProprio()"

        Dim ris As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            ris.Sessione = False
            Return ris
        End If
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Utenti) Then
            ris.Sessione = False
            Return ris
        End If

        Dim serializerSettings As New JsonSerializerSettings()
        serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore

        Dim abilitato As Boolean = True
        Dim notAbilitatoMess As String = ""
        'Dim TipoAzienda As Integer() = {enum_TipoAzienda_UMA.Azienda_Terzista, enum_TipoAzienda_UMA.Cooperativa_Agricola}
        Dim EscludiStati As Integer() = {
                    enum_WAnagraficaStati.Verifica_Intermedia_Completata_Con_Successo,
                    enum_WAnagraficaStati.Verifica_Intermedia_Non_Superata,
                    enum_WAnagraficaStati.Rinuncia
                }

        Try

            Dim gefutils As New Gias_EF_Utility
            Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri_Server.StringaConnessione)
            Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)



                'Leggo tutto le rendicontazioni ContoTerzi dove è stato dichiarato il CUAA dell'azienda selezionata per l'anno selezionato
                'Mi tiro su il tipo di azienda che ha effettuato la rendicontazione
                Dim listaRendicontazione_inContoTerzi = (From r In GiasContext.UMA_Richieste
                                                         Join rt In GiasContext.UMA_Richieste_Testata
                                                             On r.Richiesta_Cod Equals rt.Richiesta_Cod
                                                         Join p In GiasContext.Pratiche
                                                             On rt.Pratica_Cod Equals p.Pratica_Cod
                                                         Join psa In GiasContext.Pratiche_Stati_Attuali
                                                             On rt.Pratica_Cod Equals psa.Pratica_Cod
                                                         Join i In GiasContext.Imprese
                                                             On i.PIVA Equals rt.Piva
                                                         Where r.Piva = piva AndAlso
                                                             p.Anno = anno AndAlso
                                                             rt.Avanzamento_Richiesta = 1 AndAlso
                                                             rt.Tipo_Richiesta = enum_UMA_TipoRichiesta.Conto_Terzi AndAlso 'TipoAzienda.Contains(rt.Tipo_Azienda) 
                                                             Not EscludiStati.Contains(psa.Stato_Cod)
                                                         Select rt.Tipo_Azienda, i.rag_soc, pivaTerzista = i.PIVA).Distinct.ToList()

                'Se l'azienda risulta presente in Rendicontazioni ContoTerzi leggo le date di limite inserimento rendicontazione per le tipologie trovate
                If listaRendicontazione_inContoTerzi.Count > 0 Then
                    Dim listaTipoAziendaPresenti As New List(Of Integer)
                    Dim messaggio As String = ""
                    For Each tipo In listaRendicontazione_inContoTerzi
                        If Not (listaTipoAziendaPresenti.Contains(tipo.Tipo_Azienda)) Then
                            listaTipoAziendaPresenti.Add(tipo.Tipo_Azienda)
                        End If

                        messaggio += If(tipo.Tipo_Azienda = enum_TipoAzienda_UMA.Cooperativa_Agricola, Cooperativa_Agricola, Azienda_Terzista) & ": " & tipo.rag_soc & " (" & tipo.pivaTerzista & ") <br>"
                    Next

                    'Seleziono la data fine maggiore
                    Dim Data_Fine_Rendic_Terzista = (From s In GiasContext.UMA_Setup_Date_Rendicontazione
                                                     Where listaTipoAziendaPresenti.Contains(s.Tipo_Azienda) AndAlso s.Anno_Richiesta = anno
                                                     Select s.Data_Fine_Rendicontazione
                                                     Order By Data_Fine_Rendicontazione Descending).FirstOrDefault()

                    'Leggo la data entro cui è comunque possibile effettuare rendicontazione per Conto Proprio
                    Dim Data_Fine_Blocco_Rendic_ContoProprio = (From s In GiasContext.UMA_Setup_Date_Rendicontazione
                                                                Where s.Tipo_Azienda = enum_TipoAzienda_UMA.Azienda_Agricola_Privata AndAlso s.Anno_Richiesta = anno
                                                                Select s.Data_Fine_Blocco_Rendic_Conto_Proprio).FirstOrDefault()
                    If IsNothing(Data_Fine_Blocco_Rendic_ContoProprio) OrElse IsDBNull(Data_Fine_Blocco_Rendic_ContoProprio) Then
                        Data_Fine_Blocco_Rendic_ContoProprio = CostantiPersonalizzate.AGRODATAFINE
                    End If

                    'Se la data odierna è <= della data fine rendicontazione letta, l'azienda non è abilitata ad inserire una rendicontazione, 
                    'a meno che la data odierna non sia maggiore O UGUALE della data fine blocco di rendicontazione per Conto Proprio
                    If Today <= Data_Fine_Rendic_Terzista And Today < Data_Fine_Blocco_Rendic_ContoProprio Then
                        abilitato = False
                        Dim Min_Data As Date = If(Data_Fine_Blocco_Rendic_ContoProprio.HasValue AndAlso Data_Fine_Rendic_Terzista < Data_Fine_Blocco_Rendic_ContoProprio.Value,
                            Data_Fine_Rendic_Terzista,
                            Data_Fine_Blocco_Rendic_ContoProprio.Value.AddDays(-1))
                        notAbilitatoMess = "L'Azienda selezionata risulta presente in una o più rendicontazioni <b>" & anno.ToString() & "</b> in Conto Terzi o Cooperative: <br>" &
                            messaggio &
                            "<br> <b> Sarà possibile procedere con la rendicontazione in Conto Proprio dopo il " & CDate(Min_Data).ToShortDateString() & "<b>"
                    End If
                End If
            End Using

            ris.RispostaStringa = JsonConvert.SerializeObject(abilitato & "|" & notAbilitatoMess, Formatting.None, serializerSettings)
            ris.RispostaOK = True


        Catch ex As Exception

            ris.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            ris.Errore = "Errore durante l'operazione: " & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return ris
    End Function

    Private Shared Sub Leggi_Date_Ins_Rendicontazione(ByRef Data_Inizio As String,
                                                      ByRef Data_Fine As String,
                                                      anno As String,
                                                      tipo_azienda As Integer,
                                                      ByVal piva As String,
                                                      richiesta_Cod As Integer,
                                                      objParametri_Server As AgronicaCoreParametri
                                                      )

        Dim nomeRoutine As String = "Leggi_Date_Ins_Rendicontazione()"


        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri_Server.StringaConnessione)
        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            Data_Inizio = (From x In GiasContext.UMA_Setup_Date_Rendicontazione
                           Where x.Tipo_Azienda = tipo_azienda AndAlso
                               x.Anno_Richiesta = anno
                           Select x.Data_Inizio_Rendicontazione).FirstOrDefault()

            Data_Fine = (From x In GiasContext.UMA_Setup_Date_Rendicontazione
                         Where x.Tipo_Azienda = tipo_azienda AndAlso
                             x.Anno_Richiesta = anno
                         Select x.Data_Fine_Rendicontazione).FirstOrDefault()

            Dim riserva = (From x In GiasContext.UMA_Richieste_Testata
                           Join p In GiasContext.Pratiche On p.Pratica_Cod Equals x.Pratica_Cod
                           Join ps In GiasContext.Pratiche_Stati On ps.Pratica_Cod Equals p.Pratica_Cod
                           Where x.Tipo_Azienda = tipo_azienda AndAlso x.Avanzamento_Richiesta = enum_UMA_Avanzamento.Rendicontazione AndAlso
                                 x.Piva = piva AndAlso p.Anno = anno AndAlso ps.Stato_Cod = enum_WAnagraficaStati.Verifica_Intermedia_Completata_Con_Riserva
                           Order By p.Pratica_Cod Descending
                           Select p.Pratica_Cod).FirstOrDefault()

            Dim bocciate = (From x In GiasContext.UMA_Richieste_Testata
                            Join p In GiasContext.Pratiche On p.Pratica_Cod Equals x.Pratica_Cod
                            Join psa In GiasContext.Pratiche_Stati_Attuali On psa.Pratica_Cod Equals p.Pratica_Cod
                            Where x.Tipo_Azienda = tipo_azienda AndAlso x.Avanzamento_Richiesta = enum_UMA_Avanzamento.Rendicontazione AndAlso
                                 x.Piva = piva AndAlso p.Anno = anno AndAlso (psa.Stato_Cod = enum_WAnagraficaStati.Verifica_Intermedia_Non_Superata OrElse psa.Stato_Cod = enum_WAnagraficaStati.Rinuncia)
                            Order By p.Pratica_Cod
                            Select p.Pratica_Cod).FirstOrDefault()

            Dim cessata = (From x In GiasContext.UMA_Richieste_Testata
                           Join p In GiasContext.Pratiche On p.Pratica_Cod Equals x.Pratica_Cod
                           Where x.Tipo_Azienda = tipo_azienda AndAlso x.Avanzamento_Richiesta = enum_UMA_Avanzamento.Rendicontazione AndAlso
                                 x.Piva = piva AndAlso p.Anno = anno AndAlso x.Richiesta_Cod = richiesta_Cod AndAlso x.Rinuncia_Nuova_Richiesta_Anno_Successivo = 1
                           Select p.Pratica_Cod).FirstOrDefault()

            If riserva > 0 OrElse bocciate > 0 OrElse cessata > 0 Then
                Data_Inizio = AGRODATAINIZIO
                Data_Fine = AGRODATAFINE
            End If

        End Using

    End Sub



    <WebMethod(EnableSession:=True)>
    Public Shared Function GetUrlDocAgenda2010(ByVal piva As String,
                                               ByVal id_Tipologia As Integer,
                                               ByVal richiesta_Cod As Integer,
                                               ByVal pratica_Cod As Integer,
                                               ByVal id_Schema_Template As Integer,
                                               ByVal operazione As String,
                                               ByVal Documentale1_Anagrafica2 As Integer
                                               ) As RispostaStandard

        Dim r As New RispostaStandard
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try
            Dim link As String = ""

            If Documentale1_Anagrafica2 = DOCUMENTALE Then
                Dim ParametriScadenziario As New AgronicaCoreGestioneRichieste.ParametriScadenziario
                Dim str_Richiesta = "" '..una pagina lo vuole maiuscolo, l'altra minuscolo...

                Select Case operazione
                    Case "Add"
                        str_Richiesta = "&Richiesta_Cod="
                        ParametriScadenziario.Pagina_Richiesta = enum_PagineAgenda_2010.Pagina_Documentale_CreaModifica
                    Case "Read"
                        str_Richiesta = "&richiesta_cod="
                        ParametriScadenziario.Pagina_Richiesta = enum_PagineAgenda_2010.Pagina_Documentale_Lista
                End Select

                ParametriScadenziario.Piva = piva
                ParametriScadenziario.Id_Tipologia = id_Tipologia
                ParametriScadenziario.Id_Area = enum_ID_Area_Alert.UMA_Carburanti
                ParametriScadenziario.QueryStringFiltrino = ""

                If richiesta_Cod <> 0 Then
                    ParametriScadenziario.QueryStringFiltrino += str_Richiesta & richiesta_Cod
                End If
                If id_Schema_Template <> 0 Then
                    ParametriScadenziario.QueryStringFiltrino += "&Id_Schema_Template=" & id_Schema_Template
                End If
                link = RedirectGestione.IndirizzoCompleto_SitoAgenda_PassandoDirettamente_ParametriScadenziario(Enum_SiteRedirector.Sito_AgronicaUma, ParametriScadenziario)

            ElseIf Documentale1_Anagrafica2 = ANAGRAFICA Then
                Dim ParametriAgenda As New AgronicaCoreGestioneRichieste.ParametriAgenda_2010

                Dim objAgenda As New Parametri_ObjParametriAgenda_NG

                objAgenda.Pagina_Richiesta = enum_PagineGiasNG.Pagina_Menu_Anagrafica_Macchine
                objAgenda.Pagina_Provenienza = 0
                objAgenda.Piva = piva
                objAgenda.TipoOperazioneDB = enum_TipoOperazioneDB.Scrittura
                objAgenda.QueryStringFiltrino = String.Format("?seFrame=1&tabToShow=" & enum_AnagraficaNgTabs.macchine & "&showTree=false")

                objAgenda.Salva()

                link = RedirectGestione.IndirizzoCompleto_SitoGiasNG_PassandoDirettamente_ParametriAgenda_NG(Enum_SiteRedirector.Sito_AgronicaAgenda_2010, objAgenda)

            End If

            r.RispostaOK = True
            r.RispostaStringa = link

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r

    End Function


    'INIZIO AGGIUNTO GLORIA
    <WebMethod(EnableSession:=True)>
    Public Shared Function GetUrlSintesiRendicontazione(ByVal piva As String) As RispostaStandard
        Dim r As New RispostaStandard

        Dim link = "../CarburantiUMA/RiepilogoRichieste.aspx?p=" & Stringa_Codifica(piva, AgroKey_EncoderDecoder) & "&pa=1"

        r.RispostaOK = True
        r.RispostaStringa = link

        Return r

    End Function
    'FINE AGGIUNTO GLORIA

    <WebMethod(EnableSession:=True)>
    Public Shared Function GetUrlSintesiUMA(ByVal piva As String, ByVal anno As String, ByVal type As String) As RispostaStandard
        Dim r As New RispostaStandard

        Dim link = "../CarburantiUMA/RiepilogoDatiUMA.aspx?p=" & Stringa_Codifica(piva, AgroKey_EncoderDecoder) & "&anno=" & anno & "&type=" & type & "&pa=1"

        r.RispostaOK = True
        r.RispostaStringa = link

        Return r

    End Function


    <WebMethod(EnableSession:=True)>
    Public Shared Function AggiornaLavorazioniParziali(ByVal piva As String,
                                             ByVal Richiesta_Cod As Integer,
                                             ByVal isTerzista As Boolean,
                                             ByVal righeInserite As String,
                                             ByVal righeModificate As String,
                                             ByVal righeCancellate As String) As RispostaStandard

        Dim r As New RispostaStandard
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try

            Dim scrivi As New AgronicaCoreUmaBiz.UMA_Richieste
            Dim res = scrivi.Aggiorna_Lavorazioni_Parziali(piva, Richiesta_Cod, isTerzista, righeInserite, righeModificate, righeCancellate, objParametri_Server)
            r.RispostaStringa = res.Item1
            If (res.Item2.Length > 0) Then
                r.RispostaOK = False
                r.Errore = "Non tutte le lavorazioni sono state inserite perchè sono state rilevate delle sovrapposizioni " & res.Item2
            Else
                r.RispostaOK = True
            End If

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r
    End Function


    <WebMethod(EnableSession:=True)>
    Public Shared Function Leggi_Data_Limite_Inserimento_Richiesta_Anticipo(anno As String,
                                                                            tipo_azienda As Integer)

        Dim nomeRoutine As String = "Leggi_Data_Limite_Inserimento_Richiesta_Anticipo()"

        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Utenti) Then
            r.Sessione = False
            Return r
        End If

        Dim serializerSettings As New JsonSerializerSettings()
        serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore

        Dim valida As Boolean = False
        Dim Ultima_Data_Inserimento As Date?
        Try

            Dim gefutils As New Gias_EF_Utility
            Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri_Server.StringaConnessione)
            Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

                Ultima_Data_Inserimento = (From x In GiasContext.UMA_Setup_Date_Rendicontazione
                                           Where x.Tipo_Azienda = tipo_azienda AndAlso
                                                      x.Anno_Richiesta = anno
                                           Select x.Data_Limite_INS_Richiesta_Anticipo).FirstOrDefault()

                If IsNothing(Ultima_Data_Inserimento) Then
                    Ultima_Data_Inserimento = AGRODATAINIZIO
                Else
                    If Today <= CDate(Ultima_Data_Inserimento) Then
                        valida = True
                    End If
                End If
            End Using

            r.RispostaStringa = JsonConvert.SerializeObject(CDate(Ultima_Data_Inserimento), Formatting.None, serializerSettings)
            r.RispostaOK = True


        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r
    End Function


    <WebMethod(EnableSession:=True)>
    Public Shared Function leggiAcquistatoAnnoPrecedente(anno As String,
                                                         terzista As Integer,
                                                         piva As String)

        Dim nomeRoutine As String = "leggiAcquistatoAnnoPrecedente()"

        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Utenti) Then
            r.Sessione = False
            Return r
        End If

        Dim serializerSettings As New JsonSerializerSettings()
        serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore

        Dim objVendite As New AgronicaCoreUmaDal.UMA_Vendite_R

        Try

            Dim AcquistatoAnnoPrecedente As New AcquistatoAnnoPrecedente
            Dim dt As DataTable

            dt = objVendite.LeggiCarburanteVenduto(piva, anno - 1, terzista, enum_TipoCarburante_UMA.Gasolio, "", objParametri_Server)
            If dt.Rows.Count > 0 Then
                AcquistatoAnnoPrecedente.ltGasolio = dt(0).Item("Totale_Carb")
            End If

            dt = objVendite.LeggiCarburanteVenduto(piva, anno - 1, terzista, enum_TipoCarburante_UMA.Benzina, "", objParametri_Server)
            If dt.Rows.Count > 0 Then
                AcquistatoAnnoPrecedente.ltBenzina = dt(0).Item("Totale_Carb")
            End If

            dt = objVendite.LeggiCarburanteVenduto(piva, anno - 1, terzista, enum_TipoCarburante_UMA.Gasolio_Serra, "", objParametri_Server)
            If dt.Rows.Count > 0 Then
                AcquistatoAnnoPrecedente.ltGasolioSerra = dt(0).Item("Totale_Carb")
            End If


            r.RispostaStringa = JsonConvert.SerializeObject((AcquistatoAnnoPrecedente), Formatting.None, serializerSettings)
            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function leggiSommeCarburanti_daDB(Piva As String,
                                                     GruppoColturaleUMA As String,
                                                     ProgrammazioneCod As Integer,
                                                     RichiestaCod As Integer
                                                     ) As RispostaStandard



        Dim nomeRoutine As String = "leggiSommeCarburanti_daDB()"

        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Utenti) Then
            r.Sessione = False
            Return r
        End If

        Dim serializerSettings As New JsonSerializerSettings()
        serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore


        Dim objSommeLt As New AcquistatoAnnoPrecedente


        Try

            Dim gefutils As New Gias_EF_Utility
            Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri_Server.StringaConnessione)
            Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)


                objSommeLt.ltGasolio = (From l In GiasContext.UMA_Richieste_Lavorazioni
                                        Where l.Piva_SuperUser = objParametri_Server.PivaSuperUser AndAlso
                                            l.Piva = Piva AndAlso
                                            l.Gruppo_Colturale_UMA = GruppoColturaleUMA AndAlso
                                            l.Richiesta_Cod = RichiestaCod AndAlso
                                            l.Programmazione_Cod = ProgrammazioneCod AndAlso
                                            l.Tipo_Carburante = enum_TipoCarburante_UMA.Gasolio
                                        Select l.Fabbisogno_Assegnato).DefaultIfEmpty(0).Sum()


                objSommeLt.ltGasolioSerra = (From l In GiasContext.UMA_Richieste_Lavorazioni
                                             Where l.Piva_SuperUser = objParametri_Server.PivaSuperUser AndAlso
                                            l.Piva = Piva AndAlso
                                            l.Gruppo_Colturale_UMA = GruppoColturaleUMA AndAlso
                                            l.Richiesta_Cod = RichiestaCod AndAlso
                                            l.Programmazione_Cod = ProgrammazioneCod AndAlso
                                            l.Tipo_Carburante = enum_TipoCarburante_UMA.Gasolio_Serra
                                             Select l.Fabbisogno_Assegnato).DefaultIfEmpty(0).Sum()


                objSommeLt.ltBenzina = (From l In GiasContext.UMA_Richieste_Lavorazioni
                                        Where l.Piva_SuperUser = objParametri_Server.PivaSuperUser AndAlso
                                            l.Piva = Piva AndAlso
                                            l.Gruppo_Colturale_UMA = GruppoColturaleUMA AndAlso
                                            l.Richiesta_Cod = RichiestaCod AndAlso
                                            l.Programmazione_Cod = ProgrammazioneCod AndAlso
                                            l.Tipo_Carburante = enum_TipoCarburante_UMA.Benzina
                                        Select l.Fabbisogno_Assegnato).DefaultIfEmpty(0).Sum()

            End Using

            r.RispostaStringa = JsonConvert.SerializeObject(objSommeLt, Formatting.None, serializerSettings)
            r.RispostaOK = True


        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function GetUrlAnalisi2010(piva As String) As RispostaStandard

        Dim r As New RispostaStandard
        Dim Permessi As PermessiUtente = New PermessiUtente()
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try
            Dim link As String = ""
            Dim objAnalisiModelloUtils = New AgronicaCoreAnagrafeBIZ.Analisi_Modello_Utility
            Dim analisiTerrenoNG = objAnalisiModelloUtils.usaAnalisiTerrenoNG(objParametri_Utenti)
            If analisiTerrenoNG Then

                Dim UtenteAbilitato_Modifica As Boolean = Permessi.getPermesso(enum_Security_Attivita.Gest_Analisi_AccessoMenu).Lettura

                If UtenteAbilitato_Modifica Then

                    Dim objAnalisiNG As New AgronicaCoreGestioneRichieste.ParametriAnalisiTerrenoNG
                    objAnalisiNG.Pagina_SitoOrigine = enum_PagineAgronicaUMA.UMA_Nuova_Richiesta
                    objAnalisiNG.SitoOrigine = Enum_SiteRedirector.Sito_AgronicaUma
                    objAnalisiNG.Piva = piva
                    objAnalisiNG.Tipo_Analisi = enum_AnalisiTipo.Analisi_Terreno
                    objAnalisiNG.Tipo_Operazione = enum_TipoOperazioneDB.Lettura
                    objAnalisiNG.Pagina_Richiesta = enum_PagineGiasNG.Pagina_Analisi_Terreno

                    link = objAnalisiModelloUtils.Link_Pagina_AnalisiTerrenoNG(piva, objAnalisiNG)

                Else
                    link = "alert('Non si dispone del permesso richiesto per gestire le analisi.');"
                End If
            Else

                Dim ParametriAnalisi As New ParametriAnalisi_2010
                ParametriAnalisi.Pagina_Richiesta = enum_PagineAnalisi_2010.Pagina_Analisi
                ParametriAnalisi.Piva = piva
                ParametriAnalisi.Pagina_SitoOrigine = enum_PagineAgronicaUMA.UMA_Nuova_Richiesta
                ParametriAnalisi.SitoOrigine = Enum_SiteRedirector.Sito_AgronicaUma
                ParametriAnalisi.Tipo_Analisi = enum_AnalisiTipo.Analisi_Terreno
                ParametriAnalisi.Tipo_Operazione = enum_TipoOperazioneDB.Scrittura

                link = RedirectGestione.IndirizzoCompleto_SitoAnalisi_2010_PassandoDirettamente_ParametriAnalisi_2010(Enum_SiteRedirector.Sito_AgronicaUma, ParametriAnalisi)
            End If

            r.RispostaOK = True
            r.RispostaStringa = link

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r

    End Function
End Class

Public Class AcquistatoAnnoPrecedente
    Public Property ltGasolio As Double = 0
    Public Property ltBenzina As Double = 0
    Public Property ltGasolioSerra As Double = 0
End Class

Public Class CatastoAppezzamento
    Public Property Istat_Prov As String
    Public Property Prov As String
    Public Property Com As String
    Public Property Istat_Com As String
    Public Property Sezione As String
    Public Property Foglio As Integer
    Public Property Numero As Integer
    Public Property Subalterno As String
    Public Property Sup_Condotta As Double
    Public Property Key As String
    Public Property Possesso As String
    Public Property Datepossesso As String
    Public Property Inizio_possesso As Date
    Public Property Fine_possesso As Date
    Public Property Sup As Double
    Public Property Utilizzo_sup As Double
End Class

Public Class ValiditaAnno
    Public Property Inizio As Date
    Public Property Fine As Date
End Class

