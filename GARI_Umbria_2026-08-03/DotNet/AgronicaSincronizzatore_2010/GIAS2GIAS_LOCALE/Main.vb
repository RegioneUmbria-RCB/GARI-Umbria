Imports System.Text
Imports AgronicaCoreDataProvider.CostantiPersonalizzate

Imports <xmlns="http://G2G">
Imports System.IO
Imports AgronicaCoreDataProvider
Imports AgronicaCoreFiltroneBIZ
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreModello.Anagrafe
Imports AgronicaCoreModello
Imports Newtonsoft.Json.Linq
Imports Newtonsoft.Json
Imports System.Xml.Serialization
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreVarieDAL
Imports System.Data.Entity
Imports Interscambio_Util
Imports Interscambio_Ordine

Partial Public Class GIAS_2_GIAS



    Private _objFunzioni_Global As New Gias2Gias_LIB.FunzioniGLOBAL(ObjParametri_Server, 0)

    '  Marco Grilli, 04/06/2014 11:59:59: Variabile che contiene l'id della configurazione per la sincronizzazione
    Dim ID_Cfg As Integer = Nothing
    '  Marco Grilli, 04/06/2014 12:04:26: Variabile che recupero dalla configurazione del servizio di background sul db
    Dim cartellaLog As String = Nothing

    Public Event progress(ByVal totale As Integer, ByVal i As Integer, ByVal intLeftPos As Integer, ByVal intTopPos As Integer)

    ''' <summary>
    ''' Avvia g2g
    ''' </summary>
    ''' <param name="nomeCartellaLog"></param>
    ''' <param name="Log_G2G"></param>
    ''' <param name="Log_Errori"></param>
    ''' <param name="Log_Riepilogo"></param>
    ''' <param name="lConnessione_SERVER_ORIGINE"></param>
    ''' <param name="lConnessione_SERVER_DESTINAZIONE"></param>
    ''' <param name="lConnessione_UTENTI_ORIGINE"></param>
    ''' <param name="lConnessione_UTENTI_DESTINAZIONE"></param>
    ''' <param name="lProgressivoGIAS_ORIGINE"></param>
    ''' <param name="lProgressivoGIAS_DESTINAZIONE"></param>
    ''' <param name="lPivaSuperUser_ORIGINE"></param>
    ''' <param name="lPivaSuperUser_DESTINAZIONE"></param>
    ''' <param name="lUsernameSuperUser_Origine"></param>
    ''' <param name="lUsernameSuperUser_Destinazione"></param>
    ''' <param name="lImport_CodFiscale_ORIGINE"></param>
    ''' <param name="lImport_CodFiscale_DESTINAZIONE"></param>
    ''' <param name="lImport_Username_ORIGINE"></param>
    ''' <param name="lImport_Username_DESTINAZIONE"></param>
    ''' <param name="lPercorsoConnessioni"></param>
    ''' <remarks></remarks>
    Public Function GIAS_2_GIAS_SalvaXMLxServizio(
                            ByVal nomeCartellaLog As String,
                            ByRef Log_G2G As StringBuilder,
                            ByRef Log_Errori As StringBuilder,
                            ByRef Log_Riepilogo As StringBuilder,
                            ByVal urlWsimportaGias As String,
                            ByVal lConnessione_SERVER_ORIGINE As String,
                            ByVal lConnessione_SERVER_DESTINAZIONE As String,
                            ByVal lConnessione_UTENTI_ORIGINE As String,
                            ByVal lConnessione_UTENTI_DESTINAZIONE As String,
                            ByVal lProgressivoGIAS_ORIGINE As String,
                            ByVal lProgressivoGIAS_DESTINAZIONE As String,
                            ByVal lPivaSuperUser_ORIGINE As String,
                            ByVal lPivaSuperUser_DESTINAZIONE As String,
                            ByVal lUsernameSuperUser_Origine As String,
                            ByVal lUsernameSuperUser_Destinazione As String,
                            ByVal lImport_CodFiscale_ORIGINE As String,
                            ByVal lImport_CodFiscale_DESTINAZIONE As String,
                            ByVal lImport_Username_ORIGINE As String,
                            ByVal lImport_Username_DESTINAZIONE As String,
                            ByVal lImport_CodiceImpresa_ORIGINE As String,
                            ByVal lImport_CodiceImpresa_DESTINAZIONE As String,
                            ByVal lImpostazioniTrasformazioni As String,
                            ByVal lPercorsoConnessioni As String,
                            ByVal cfgImpresa As List(Of clsDatiImpresaLista),
                            ByVal cfgGlobali As List(Of clsDatiImpresaLista),
                            ByVal filtrone As String,
                            ByVal filtronerisultato As String,
                            ByVal filtronerisultato_azienda As String,
                            ByRef getXML As String
        ) As String

        Const NomeFunzione As String = "GIAS_2_GIAS_SalvaXMLxServizio"
        Dim sXmlImprese As String = Nothing
        Dim letturaDatiImprese As New GIAS2GIAS_LOCALE.LetturaDatiImpreseXML
        Dim objAgronicaCore As New AgronicaCoreDataProvider.DataProvider

        Try

            Dim Stringa_Connessione_Server_GIAS_Origine As String
            Stringa_Connessione_Server_GIAS_Origine = objAgronicaCore.FindConnessione_Su_Ini_O_Superserver(lPercorsoConnessioni, lConnessione_SERVER_ORIGINE)

            Dim objParametri_Server_GIAS_ORIGINE As AgronicaCoreParametri = GetObjParametri(
                lPivaSuperUser_ORIGINE,
                lPivaSuperUser_ORIGINE,
                lImport_CodFiscale_ORIGINE,
                lImport_Username_ORIGINE,
                "",
                Stringa_Connessione_Server_GIAS_Origine
            )

            sXmlImprese = letturaDatiImprese.GeneraXmlDaImportare(
                    objParametri_Server_GIAS_ORIGINE,
                    urlWsimportaGias,
                    lConnessione_SERVER_ORIGINE,
                    lConnessione_SERVER_DESTINAZIONE,
                    lConnessione_UTENTI_ORIGINE,
                    lConnessione_UTENTI_DESTINAZIONE,
                    lProgressivoGIAS_ORIGINE,
                    lProgressivoGIAS_DESTINAZIONE,
                    lPivaSuperUser_ORIGINE,
                    lPivaSuperUser_DESTINAZIONE,
                    lUsernameSuperUser_Origine,
                    lUsernameSuperUser_Destinazione,
                    lImport_CodFiscale_ORIGINE,
                    lImport_CodFiscale_DESTINAZIONE,
                    lImport_Username_ORIGINE,
                    lImport_Username_DESTINAZIONE,
                    lImport_CodiceImpresa_ORIGINE,
                    lImport_CodiceImpresa_DESTINAZIONE,
                    lImpostazioniTrasformazioni,
                    lPercorsoConnessioni,
                    getXML,
                    cfgImpresa,
                    cfgGlobali,
                    filtrone,
                    filtronerisultato,
                    filtronerisultato_azienda
)



        Catch ex As Exception
            Log_G2G.Append(CStr(Date.Now) & " - " & NomeFunzione & " Si è verificato il seguente errore: " & ex.Message & vbCrLf)
            Log_Errori.Append(CStr(Date.Now) & " - " & NomeFunzione & " Si è verificato il seguente errore: " & ex.Message & vbCrLf)
        End Try

        Return sXmlImprese


    End Function

    ''' <summary>
    ''' Avvia g2g
    ''' </summary>
    ''' <param name="nomeCartellaLog"></param>
    ''' <param name="Log_G2G"></param>
    ''' <param name="Log_Errori"></param>
    ''' <param name="Log_Riepilogo"></param>
    ''' <param name="lConnessione_SERVER_ORIGINE"></param>
    ''' <param name="lConnessione_SERVER_DESTINAZIONE"></param>
    ''' <param name="lConnessione_UTENTI_ORIGINE"></param>
    ''' <param name="lConnessione_UTENTI_DESTINAZIONE"></param>
    ''' <param name="lProgressivoGIAS_ORIGINE"></param>
    ''' <param name="lProgressivoGIAS_DESTINAZIONE"></param>
    ''' <param name="lPivaSuperUser_ORIGINE"></param>
    ''' <param name="lPivaSuperUser_DESTINAZIONE"></param>
    ''' <param name="lUsernameSuperUser_Origine"></param>
    ''' <param name="lUsernameSuperUser_Destinazione"></param>
    ''' <param name="lImport_CodFiscale_ORIGINE"></param>
    ''' <param name="lImport_CodFiscale_DESTINAZIONE"></param>
    ''' <param name="lImport_Username_ORIGINE"></param>
    ''' <param name="lImport_Username_DESTINAZIONE"></param>
    ''' <param name="lPercorsoConnessioni"></param>
    ''' <remarks></remarks>
    Public Sub GIAS_2_GIAS(
                            ByVal nomeCartellaLog As String,
                            ByRef Log_G2G As StringBuilder,
                            ByRef Log_Errori As StringBuilder,
                            ByRef Log_Riepilogo As StringBuilder,
                            ByVal urlWS As String,
                            ByVal lConnessione_SERVER_ORIGINE As String,
                            ByVal lConnessione_SERVER_DESTINAZIONE As String,
                            ByVal lConnessione_UTENTI_ORIGINE As String,
                            ByVal lConnessione_UTENTI_DESTINAZIONE As String,
                            ByVal lProgressivoGIAS_ORIGINE As String,
                            ByVal lProgressivoGIAS_DESTINAZIONE As String,
                            ByVal lPivaSuperUser_ORIGINE As String,
                            ByVal lPivaSuperUser_DESTINAZIONE As String,
                            ByVal lUsernameSuperUser_Origine As String,
                            ByVal lUsernameSuperUser_Destinazione As String,
                            ByVal lImport_CodFiscale_ORIGINE As String,
                            ByVal lImport_CodFiscale_DESTINAZIONE As String,
                            ByVal lImport_Username_ORIGINE As String,
                            ByVal lImport_Username_DESTINAZIONE As String,
                            ByVal lImport_CodiceImpresa_ORIGINE As String,
                            ByVal lImport_CodiceImpresa_DESTINAZIONE As String,
                            ByVal lImpostazioniTrasformazioni As String,
                            ByVal lPercorsoConnessioni As String,
                            ByVal cfgImpresa As List(Of clsDatiImpresaLista),
                            ByVal cfgGlobali As List(Of clsDatiImpresaLista),
                            ByVal filtrone As String,
                            ByVal filtronerisultato As String,
                            ByVal filtronerisultato_azienda As String,
                            ByRef getXML As String
        )

        Const NomeFunzione As String = "GIAS_2_GIAS"

        Dim nomeFileLog As String = ".\" & CStr(DateTime.Now.Year) & DateTime.Now.Month.ToString("D2") & DateTime.Now.Day.ToString("D2") & "log_G2G.txt"
        Dim nomeFileErrori As String = ".\" & CStr(DateTime.Now.Year) & DateTime.Now.Month.ToString("D2") & DateTime.Now.Day.ToString("D2") & "log_Errori.txt"
        Dim nomeFileRiepilogo As String = ".\" & CStr(DateTime.Now.Year) & DateTime.Now.Month.ToString("D2") & DateTime.Now.Day.ToString("D2") & "log_Riepilogo.txt"

        Try
            Log_G2G.Append(CStr(Date.Now) & " - " & "Inizio Gias 2 Gias." & vbCrLf & vbCrLf)

            Dim imprese As XDocument
            Dim objAgronicaCore As New AgronicaCoreDataProvider.DataProvider

            Dim Stringa_Connessione_Server_GIAS_Origine As String
            Stringa_Connessione_Server_GIAS_Origine = objAgronicaCore.FindConnessione_Su_Ini_O_Superserver(lPercorsoConnessioni, lConnessione_SERVER_ORIGINE)

            Dim objParametri_Server_GIAS_ORIGINE As AgronicaCoreParametri = GetObjParametri(
                lPivaSuperUser_ORIGINE,
                lPivaSuperUser_ORIGINE,
                lImport_CodFiscale_ORIGINE,
                lImport_Username_ORIGINE,
                "",
                Stringa_Connessione_Server_GIAS_Origine
            )

            Dim letturaDatiImprese As New GIAS2GIAS_LOCALE.LetturaDatiImpreseXML
            Dim sXmlImprese As String = letturaDatiImprese.GeneraXmlDaImportare(
                objParametri_Server_GIAS_ORIGINE,
                urlWS,
                lConnessione_SERVER_ORIGINE,
                lConnessione_SERVER_DESTINAZIONE,
                lConnessione_UTENTI_ORIGINE,
                lConnessione_UTENTI_DESTINAZIONE,
                lProgressivoGIAS_ORIGINE,
                lProgressivoGIAS_DESTINAZIONE,
                lPivaSuperUser_ORIGINE,
                lPivaSuperUser_DESTINAZIONE,
                lUsernameSuperUser_Origine,
                lUsernameSuperUser_Destinazione,
                lImport_CodFiscale_ORIGINE,
                lImport_CodFiscale_DESTINAZIONE,
                lImport_Username_ORIGINE,
                lImport_Username_DESTINAZIONE,
                lImport_CodiceImpresa_ORIGINE,
                lImport_CodiceImpresa_DESTINAZIONE,
                lImpostazioniTrasformazioni,
                lPercorsoConnessioni,
                getXML,
                cfgImpresa,
                cfgGlobali,
                filtrone,
                filtronerisultato,
                filtronerisultato_azienda
            )

            If getXML = "noData" Then
                getXML = sXmlImprese
                Exit Sub
            End If

            If getXML = "true" Then
                getXML = sXmlImprese
            Else

                imprese = XDocument.Parse(sXmlImprese)

                Dim idleMsg As String = ""
                GIAS_2_GIASCiclaImprese(idleMsg, Log_G2G, Log_Errori, Log_Riepilogo,
                                        nomeCartellaLog, imprese, FromServizio:=False,
                                        nomeFileLog:=nomeFileLog,
                                        nomeFileErrori:=nomeFileErrori,
                                        nomeFileRiepilogo:=nomeFileRiepilogo)

                Log_G2G.AppendLine(CStr(Date.Now) & " - " & "Fine Gias 2 Gias." & vbCrLf)
                My.Computer.FileSystem.WriteAllText(nomeCartellaLog & nomeFileLog, Log_G2G.ToString, True)
                My.Computer.FileSystem.WriteAllText(nomeCartellaLog & nomeFileErrori, Log_Errori.ToString, True)
                My.Computer.FileSystem.WriteAllText(nomeCartellaLog & nomeFileRiepilogo, Log_Riepilogo.ToString, True)

            End If

        Catch ex As Exception

            Log_G2G.AppendLine(CStr(Date.Now) & " - " & NomeFunzione & " Si è verificato il seguente errore: " & ex.Message)
            Log_Errori.AppendLine(CStr(Date.Now) & " - " & NomeFunzione & " Si è verificato il seguente errore: " & ex.Message)

            My.Computer.FileSystem.WriteAllText(nomeCartellaLog & nomeFileLog, Log_G2G.ToString, True)
            My.Computer.FileSystem.WriteAllText(nomeCartellaLog & nomeFileErrori, Log_Errori.ToString, True)

        Finally

        End Try

    End Sub

    ''' <summary>
    ''' Avvia il gias to gias, se si passa un file non occorre passare i parametri server.
    ''' </summary>
    ''' <param name="Log_G2G"></param>
    ''' <param name="Log_Errori"></param>
    ''' <param name="Log_Riepilogo"></param>
    ''' <param name="PathFileXMLOpzioniImport"></param>
    ''' <param name="objParametri_Server_GIAS_ORIGINE"></param>
    ''' <remarks></remarks>
    Public Sub GIAS_2_GIAS(ByRef Log_G2G As StringBuilder,
                            ByRef Log_Errori As StringBuilder,
                            ByRef Log_Riepilogo As StringBuilder,
                            ByVal PathFileXMLOpzioniImport As String,
                            Optional ByVal objParametri_Server_GIAS_ORIGINE As AgronicaCoreDataProvider.AgronicaCoreParametri = Nothing,
                            Optional ByVal xmlConfig As String = "")

        Const NomeFunzione As String = "GIAS_2_GIAS"


        Dim fileData As FileInfo = My.Computer.FileSystem.GetFileInfo(PathFileXMLOpzioniImport)
        Dim nomeCartella As String = fileData.DirectoryName

        Dim nomeFileLog As String = ".\" & CStr(DateTime.Now.Year) & DateTime.Now.Month.ToString("D2") & DateTime.Now.Day.ToString("D2") & "log_G2G.txt"
        Dim nomeFileErrori As String = ".\" & CStr(DateTime.Now.Year) & DateTime.Now.Month.ToString("D2") & DateTime.Now.Day.ToString("D2") & "log_Errori.txt"
        Dim nomeFileRiepilogo As String = ".\" & CStr(DateTime.Now.Year) & DateTime.Now.Month.ToString("D2") & DateTime.Now.Day.ToString("D2") & "log_Riepilogo.txt"

        For Each singleFile As String In PathFileXMLOpzioniImport.Split(",")

            Try
                Log_G2G.Append(CStr(Date.Now) & " - " & "Inizio Gias 2 Gias." & vbCrLf & vbCrLf)

                Dim imprese As XDocument
                If singleFile <> "" Then
                    imprese = XDocument.Load(singleFile)
                Else

                    Dim letturaDatiImprese As New GIAS2GIAS_LOCALE.LetturaDatiImpreseXML
                    Dim sXmlImprese As String = letturaDatiImprese.GeneraXmlDaImportare(False, objParametri_Server_GIAS_ORIGINE)

                    imprese = XDocument.Parse(sXmlImprese)
                End If

                Dim idleMsg As String = ""
                GIAS_2_GIASCiclaImprese(idleMsg, Log_G2G, Log_Errori, Log_Riepilogo, nomeCartella, imprese,
                                        FromServizio:=False,
                                        nomeFileLog:=nomeFileLog,
                                        nomeFileErrori:=nomeFileErrori,
                                        nomeFileRiepilogo:=nomeFileRiepilogo)

                Log_G2G.Append(CStr(Date.Now) & " - " & "Fine Gias 2 Gias." & vbCrLf & vbCrLf)
                My.Computer.FileSystem.WriteAllText(nomeCartella & nomeFileLog, Log_G2G.ToString, True)
                My.Computer.FileSystem.WriteAllText(nomeCartella & nomeFileErrori, Log_Errori.ToString, True)
                My.Computer.FileSystem.WriteAllText(nomeCartella & nomeFileRiepilogo, Log_Riepilogo.ToString, True)


            Catch ex As Exception
                Log_G2G.Append(CStr(Date.Now) & " - " & NomeFunzione & " Si è verificato il seguente errore: " & ex.Message & vbCrLf)
                Log_Errori.Append(CStr(Date.Now) & " - " & NomeFunzione & " Si è verificato il seguente errore: " & ex.Message & vbCrLf)

                My.Computer.FileSystem.WriteAllText(nomeCartella & nomeFileLog, Log_G2G.ToString, True)
                My.Computer.FileSystem.WriteAllText(nomeCartella & nomeFileErrori, Log_Errori.ToString, True)

            Finally



            End Try

        Next
    End Sub

    ''' <summary>
    ''' Legge i dati gis centralizzati su server Agronica e li riporta in locale
    ''' </summary>
    ''' <remarks></remarks>
    Public Function GIAS_2_GIAS_PULL_GIS_SRVAgronica(Configurazione_Servizio As Configurazione_Servizio) As RispostaStandard

        Dim rval As New RispostaStandard

        Const NomeFunzione As String = "GIAS_2_GIAS_PULL_GIS_SRVAgronica"

        Dim Log_G2G As New StringBuilder("")
        Dim Log_Errori As New StringBuilder("")
        ' Dim Log_Riepilogo As New StringBuilder("")
        Dim piva As String = ""
        Dim messaggioErrori As String = ""

        cartellaLog = Configurazione_Servizio.DirectoryLOG

        Try
            Log_G2G.AppendLine(CStr(Date.Now) & " - " & "Inizio  Gias 2 Gias -->" & NomeFunzione)
            Log_G2G.AppendLine(CStr(Date.Now) & " - " & "Leggo le configurazioni")

            Dim cfg As AgronicaCoreGisBIZ.GIS_SrvAgronica_cfg =
                JsonConvert.DeserializeObject(Of AgronicaCoreGisBIZ.GIS_SrvAgronica_cfg)(Configurazione_Servizio.Parametri_Extra)

            Dim GestioneDati As New AgronicaCoreGisBIZ.GIS_SrvAgronica
            rval =
               GestioneDati.GIS_SRVAgronica_AllineaDati(cfg, ObjParametri_Server)


            Log_G2G.AppendLine("scrittura eseguita, elementi non importati: ")


        Catch ex As Exception
            messaggioErrori = CStr(Date.Now) & " - " & NomeFunzione & " Errore: " & ex.Message
            Log_Errori.AppendLine(messaggioErrori)
            rval.Errore = messaggioErrori
            rval.RispostaOK = False
        Finally

            cartellaLog = Path.Combine(cartellaLog, "G2G_ExportXml")
            If Not Directory.Exists(cartellaLog) Then
                Directory.CreateDirectory(cartellaLog)
            End If

            If Not cartellaLog.EndsWith("\") Then
                cartellaLog &= "\"
            End If

            rval.ParametroDue_stringa &= Log_G2G.ToString

            Dim Anno As String = Date.Today.Year.ToString
            Dim Mese As String = Right("00" & Date.Today.Month.ToString, 2)
            Dim Giorno As String = Right("00" & Date.Today.Day.ToString, 2)
            Dim Ora As String = Right("00" & Date.Now.Hour.ToString, 2)
            Dim Minuti As String = Right("00" & Date.Now.Minute.ToString, 2)
            Dim Secondi As String = Right("00" & Date.Now.Second.ToString, 2)
            Dim time As String = Anno & Mese & Giorno & "_" & Ora & Minuti & Secondi

            Dim NomeFileLog As String = piva & "_" & time & "_GIS_SRVAgronica_Log.txt"
            Dim NomeFileErrori As String = piva & "_" & time & "_GIS_SRVAgronica_Errori.txt"

            If Log_Errori.ToString = "" Then
                Log_Errori.Append("Nessun errore riscontrato.")
            End If

            My.Computer.FileSystem.WriteAllText(cartellaLog & NomeFileLog, Log_G2G.ToString, True)
            My.Computer.FileSystem.WriteAllText(cartellaLog & NomeFileErrori, Log_Errori.ToString, True)
            'My.Computer.FileSystem.WriteAllText(cartellaLog & ".\log_Riepilogo.txt", Log_Riepilogo.ToString, True)

        End Try


        Return rval

    End Function

    ''' <summary>
    ''' Estrae i dati di dettaglio in modalità "PULL" a partire dal nome della confiurazione XML presente sul db
    ''' </summary>
    ''' <remarks></remarks>
    Public Function GIAS_2_GIAS_PULL_DaIDConf_letturaAgenda(ByVal codiceAzienda As String,
                                                      ByVal CartellaFileXml As String,
                                                      ByVal codice_GIAS As Integer) As RispostaStandard

        Dim rval As New RispostaStandard

        Const NomeFunzione As String = "GIAS_2_GIAS_PULL_DaIDConf_letturaAgenda"

        Dim Log_G2G As New StringBuilder("")
        Dim Log_Errori As New StringBuilder("")
        ' Dim Log_Riepilogo As New StringBuilder("")
        Dim sxml As String = Nothing
        Dim l As New AgronicaCoreG2GLocalDal.G2GLocal_R
        Dim piva As String = ""
        Dim messaggioErrori As String = ""

        Try
            Log_G2G.AppendLine(CStr(Date.Now) & " - " & "Inizio  Gias 2 Gias -->" & NomeFunzione)
            Log_G2G.AppendLine(CStr(Date.Now) & " - " & "Leggo le configurazioni")

            Dim dt As DataTable = l.LeggiConfigurazioni(ID_Cfg, ObjParametri_Server)

            '  Marco Grilli, 03/06/2014 18:20:30: se non trovo nessuna configurazione non quel nome, scrivo un errore ed esco dalla funzione.
            If (dt.Rows.Count <= 0) Then
                Dim ErroreCFG As String = CStr(Date.Now) & " - " & NomeFunzione & " Si è verificato il seguente errore: Nessuna configurazione trovata con l'id " & ID_Cfg
                Log_G2G.AppendLine(ErroreCFG)
                Log_Errori.AppendLine(ErroreCFG)
                rval.RispostaOK = False
                rval.Errore = ErroreCFG
                Return rval
            Else
                sxml = dt(0)(2) '  Marco Grilli, 04/06/2014 10:51:07: prelevo il campo con l'xml
            End If

            Dim imprese As XDocument = XDocument.Parse(sxml)

            Dim xElemCfg As XElement = imprese.<dati>.First

            Log_G2G.AppendLine(CStr(Date.Now) & " - " & "Gias2Gias_LIB.Funzioni_MasterG2G.LeggiOpzioniDaXml.")
            Dim objOpzioni As Gias2Gias_LIB.clsOpzioni = Gias2Gias_LIB.Funzioni_MasterG2G.LeggiOpzioniDaXml(xElemCfg, "http://G2G")

            Try
                objOpzioni.FormatoOraZero = System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.LongTimePattern.ToLower.Replace("h", "0").Replace("m", "0").Replace("s", "0")

                Log_G2G.AppendLine(CStr(Date.Now) & " - " & "Parametri Utente: Formato data: " & System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.LongDatePattern &
                                                " - Formato ora zero: " & objOpzioni.FormatoOraZero)
            Catch ex As Exception
                objOpzioni.FormatoOraZero = "00.00.00"
                Dim err1 As String = CStr(Date.Now) & " - " & "Parametri Utente: Formato data non impostato da sistema. "
                Log_G2G.AppendLine(err1)
                rval.RispostaOK = False
                rval.Errore = err1
                Return rval
            End Try

            Log_G2G.AppendLine(CStr(Date.Now) & " - " & "GIAS_2_GIAS_Completaopzioni.")
            GIAS_2_GIAS_Completaopzioni(objOpzioni, False)

            '=============================================================
            '======================== AVVIO LETTURE =======================
            '=============================================================

            ' VAnni: 17/5/2019: introdotto "DatiImpresa", "filtrone", lista impianti su aziende

            Log_G2G.AppendLine(CStr(Date.Now) & " - " & "ListaDatiDaImportareLetturaDaXDoc.")
            Dim listOfDataImport As clsImportData = ListaDatiDaImportareLetturaDaXDoc(imprese)

            ApplicaFiltroDatiPopolaListaImprese(objOpzioni, listOfDataImport)



            'cuore della funzione PULL
            'GIAS_2_GIASCiclaImprese(rval.ParametroDue_stringa, Log_G2G, Log_Errori, Log_Riepilogo, cartellaLog, imprese, True)

            Dim filtro1 As List(Of String) = (From pp In listOfDataImport.Imprese
                                              Select "'" & pp.Piva_ORIGINE & "'").ToList()


            'riporto le cfg dell'oggetto g2g in parametri

            'LEGGIMI: questa proprietà dovrebbe essere passata dal file di config
            Dim Flag_GestioneCodifiche As Boolean = False

            'LEGGIMI: questa proprietà dovrebbe essere passata dal file di config
            'Dim Flag_Esporta_SOLO_ImpreseConImpianti As Integer = 0
            'If listOfDataImport.DatiImpresa.Flag_Esporta_SOLO_ImpreseConImpianti = True Then
            '    Flag_Esporta_SOLO_ImpreseConImpianti = 1
            'End If



            Dim letturaDettaglioImpresa As New GIAS2GIAS_LOCALE.Esporta_Gias_Pubblico(
                objParametri_Server:=ObjParametri_Server,
                objParametri_utenti:=ObjParametri_Utenti,
                CartellaFileXml:=CartellaFileXml,
                FinestraTemp_Inizio:=AGRODATAINIZIO,
                FinestraTemp_Fine:=AGRODATAFINE
            )


            Log_G2G.AppendLine(CStr(Date.Now) & " - " & "Filtro azienda.")
            'il codice è un CUAA, una piva oppure altro...
            If objOpzioni.CodiceImpresa_ORIGINE <> 0 Then

                Dim leggiPivaCuaa As New AgronicaCoreAnagrafeDAL.Imprese_Codici_Read

                letturaDettaglioImpresa.Filtro_Piva =
                leggiPivaCuaa.Piva_from_IdCodValCod(objOpzioni.CodiceImpresa_ORIGINE, codiceAzienda, ObjParametri_Server)
            Else
                letturaDettaglioImpresa.Filtro_Piva = codiceAzienda
            End If
            piva = CStr(letturaDettaglioImpresa.Filtro_Piva)


            If filtro1.Count = 0 OrElse filtro1.Contains("'" & letturaDettaglioImpresa.Filtro_Piva & "'") Then

                Log_G2G.AppendLine(CStr(Date.Now) & " - " & "Esporta_Dati.")

                Dim cfg As clsCfgImpostazioniTrasformazioni = Nothing
                If objOpzioni.ImpostazioniTrasformazioni <> "" Then
                    cfg = JsonConvert.DeserializeObject(Of clsCfgImpostazioniTrasformazioni)(objOpzioni.ImpostazioniTrasformazioni)
                End If

                Dim GEORiferimento_Cod As Integer = -1
                If listOfDataImport.DatiImpresa.Flagimporta_gis Then
                    If cfg IsNot Nothing Then
                        GEORiferimento_Cod = cfg.GEORiferimento_Cod
                    End If
                End If

                Dim strImpresa As String =
                    letturaDettaglioImpresa.Esporta_Dati_Agenda(objOpzioni, listOfDataImport.DatiImpresa, CartellaFileXml, Flag_GestioneCodifiche, codice_GIAS, Log_G2G, Log_Errori)

                If strImpresa <> "" Then

                    If cfg IsNot Nothing Then
                        Log_G2G.AppendLine(CStr(Date.Now) & " - " & "ImpostazioniTrasformazioni.")
                        If cfg.PercorsoXsltLetturaAgenda <> "" Then
                            Dim tr As New AgronicaCoreUtility.XsltUtils
                            rval.RispostaStringa = tr.trasformaDatoPercorsoFile(strImpresa, cfg.PercorsoXsltLetturaAgenda)
                            ScriviXMLsuFile(CartellaFileXml, rval.RispostaStringa, piva & "_", "_TRASF")
                        Else
                            rval.RispostaStringa = strImpresa
                        End If
                    Else
                        Log_G2G.AppendLine(CStr(Date.Now) & " - " & "Non ci sono Trasformazioni da applicare.")
                        rval.RispostaStringa = strImpresa
                    End If

                    Log_G2G.AppendLine(CStr(Date.Now) & " - " & "Fine Gias 2 Gias --> " & NomeFunzione)
                    rval.RispostaOK = True
                    rval.Errore = ""

                Else
                    Log_G2G.AppendLine(CStr(Date.Now) & " - " & "strImpresa vuota ")
                    rval.RispostaOK = False
                    rval.Errore = "Non sono stati recuperati dati sull'azienda."
                    rval.RispostaStringa = ""
                End If
            Else
                rval.RispostaOK = False
                rval.Errore = "Richiesta non autorizzata."
                rval.RispostaStringa = ""
            End If
            'azienda autorizzata

        Catch ex As Exception
            messaggioErrori = CStr(Date.Now) & " - " & NomeFunzione & " Errore: " & ex.Message
            Log_Errori.AppendLine(messaggioErrori)
            rval.Errore = messaggioErrori
            rval.RispostaOK = False
        Finally

            cartellaLog = Path.Combine(cartellaLog, "G2G_ExportXml")
            If Not Directory.Exists(cartellaLog) Then
                Directory.CreateDirectory(cartellaLog)
            End If

            If Not cartellaLog.EndsWith("\") Then
                cartellaLog &= "\"
            End If

            rval.ParametroDue_stringa &= Log_G2G.ToString

            Dim Anno As String = Date.Today.Year.ToString
            Dim Mese As String = Right("00" & Date.Today.Month.ToString, 2)
            Dim Giorno As String = Right("00" & Date.Today.Day.ToString, 2)
            Dim Ora As String = Right("00" & Date.Now.Hour.ToString, 2)
            Dim Minuti As String = Right("00" & Date.Now.Minute.ToString, 2)
            Dim Secondi As String = Right("00" & Date.Now.Second.ToString, 2)
            Dim time As String = Anno & Mese & Giorno & "_" & Ora & Minuti & Secondi

            Dim NomeFileLog As String = piva & "_" & time & "_AnagDet_Log.txt"
            Dim NomeFileErrori As String = piva & "_" & time & "_AnagDet_Errori.txt"

            If Log_Errori.ToString = "" Then
                Log_Errori.Append("Nessun errore riscontrato.")
            End If

            My.Computer.FileSystem.WriteAllText(cartellaLog & NomeFileLog, Log_G2G.ToString, True)
            My.Computer.FileSystem.WriteAllText(cartellaLog & NomeFileErrori, Log_Errori.ToString, True)
            'My.Computer.FileSystem.WriteAllText(cartellaLog & ".\log_Riepilogo.txt", Log_Riepilogo.ToString, True)

        End Try


        Return rval

    End Function
    ''' <summary>
    ''' Estrae i dati di dettaglio in modalità "PULL" a partire dal nome della confiurazione XML presente sul db
    ''' </summary>
    ''' <remarks></remarks>
    Public Function GIAS_2_GIAS_PULL_DaIDConf_AnagDet(ByVal codiceAzienda As String,
                                                      ByVal CartellaFileXml As String,
                                                      ByVal codice_GIAS As Integer) As RispostaStandard

        Dim rval As New RispostaStandard

        Const NomeFunzione As String = "GIAS_2_GIAS_PULL_DaIDConf"

        Dim Log_G2G As New StringBuilder("")
        Dim Log_Errori As New StringBuilder("")
        ' Dim Log_Riepilogo As New StringBuilder("")
        Dim sxml As String = Nothing
        Dim l As New AgronicaCoreG2GLocalDal.G2GLocal_R
        Dim piva As String = ""
        Dim messaggioErrori As String = ""

        Try
            Log_G2G.AppendLine(CStr(Date.Now) & " - " & "Inizio  Gias 2 Gias -->" & NomeFunzione)
            Log_G2G.AppendLine(CStr(Date.Now) & " - " & "Leggo le configurazioni")

            Dim dt As DataTable = l.LeggiConfigurazioni(ID_Cfg, ObjParametri_Server)

            '  Marco Grilli, 03/06/2014 18:20:30: se non trovo nessuna configurazione non quel nome, scrivo un errore ed esco dalla funzione.
            If (dt.Rows.Count <= 0) Then
                Dim ErroreCFG As String = CStr(Date.Now) & " - " & NomeFunzione & " Si è verificato il seguente errore: Nessuna configurazione trovata con l'id " & ID_Cfg
                Log_G2G.AppendLine(ErroreCFG)
                Log_Errori.AppendLine(ErroreCFG)
                rval.RispostaOK = False
                rval.Errore = ErroreCFG
                Return rval
            Else
                sxml = dt(0)(2) '  Marco Grilli, 04/06/2014 10:51:07: prelevo il campo con l'xml
            End If

            Dim imprese As XDocument = XDocument.Parse(sxml)

            Dim xElemCfg As XElement = imprese.<dati>.First

            Log_G2G.AppendLine(CStr(Date.Now) & " - " & "Gias2Gias_LIB.Funzioni_MasterG2G.LeggiOpzioniDaXml.")
            Dim objOpzioni As Gias2Gias_LIB.clsOpzioni = Gias2Gias_LIB.Funzioni_MasterG2G.LeggiOpzioniDaXml(xElemCfg, "http://G2G")

            Try
                objOpzioni.FormatoOraZero = System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.LongTimePattern.ToLower.Replace("h", "0").Replace("m", "0").Replace("s", "0")

                Log_G2G.AppendLine(CStr(Date.Now) & " - " & "Parametri Utente: Formato data: " & System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.LongDatePattern &
                                                " - Formato ora zero: " & objOpzioni.FormatoOraZero)
            Catch ex As Exception
                objOpzioni.FormatoOraZero = "00.00.00"
                Dim err1 As String = CStr(Date.Now) & " - " & "Parametri Utente: Formato data non impostato da sistema. "
                Log_G2G.AppendLine(err1)
                rval.RispostaOK = False
                rval.Errore = err1
                Return rval
            End Try

            Log_G2G.AppendLine(CStr(Date.Now) & " - " & "GIAS_2_GIAS_Completaopzioni.")
            GIAS_2_GIAS_Completaopzioni(objOpzioni, False)

            '=============================================================
            '======================== AVVIO LETTURE =======================
            '=============================================================

            ' VAnni: 17/5/2019: introdotto "DatiImpresa", "filtrone", lista impianti su aziende

            Log_G2G.AppendLine(CStr(Date.Now) & " - " & "ListaDatiDaImportareLetturaDaXDoc.")
            Dim listOfDataImport As clsImportData = ListaDatiDaImportareLetturaDaXDoc(imprese)

            ApplicaFiltroDatiPopolaListaImprese(objOpzioni, listOfDataImport)



            'cuore della funzione PULL
            'GIAS_2_GIASCiclaImprese(rval.ParametroDue_stringa, Log_G2G, Log_Errori, Log_Riepilogo, cartellaLog, imprese, True)

            Dim filtro1 As List(Of String) = (From pp In listOfDataImport.Imprese
                                              Select "'" & pp.Piva_ORIGINE & "'").ToList()


            'riporto le cfg dell'oggetto g2g in parametri

            'LEGGIMI: questa proprietà dovrebbe essere passata dal file di config
            Dim Flag_GestioneCodifiche As Boolean = False

            'LEGGIMI: questa proprietà dovrebbe essere passata dal file di config
            Dim Flag_Esporta_SOLO_ImpreseConImpianti As Integer = 0
            'If listOfDataImport.DatiImpresa.Flag_Esporta_SOLO_ImpreseConImpianti = True Then
            '    Flag_Esporta_SOLO_ImpreseConImpianti = 1
            'End If

            'anagrafica
            Dim Flag_Crea_Nodo_Impresa As Integer = 1
            Dim Flag_Crea_Nodo_Centro As Integer = 1

            'LEGGIMI: questa proprietà dovrebbe essere passata dal file di config
            'in ogni caso non è gestita lettura ed esportazione dei fabbricati
            Dim Flag_Crea_Nodo_Fabbricato As Integer = 0

            'piano colturale
            Dim Flag_Crea_Nodo_Campo As Integer = 1
            Dim Flag_Crea_Nodo_Appezzamento As Integer = 1
            Dim Flag_Crea_Nodo_Impianto As Integer = 1
            Dim Flag_Crea_Nodo_Progetto As Integer = 1

            If Not listOfDataImport.DatiImpresa.Flagimporta_pianocolturale Then
                Flag_Crea_Nodo_Campo = 0
                Flag_Crea_Nodo_Appezzamento = 0
                Flag_Crea_Nodo_Impianto = 0
                Flag_Crea_Nodo_Progetto = 0
            End If

            'catasto
            Dim Flag_Crea_Nodo_Particella As Integer = 1
            Dim Flag_Crea_Nodo_AppezzamentoParticella As Integer = 1
            Dim Flag_Crea_Nodo_CampoParticella As Integer = 1
            If Not listOfDataImport.DatiImpresa.Flagimporta_catasto Then
                Flag_Crea_Nodo_CampoParticella = 0
                Flag_Crea_Nodo_Particella = 0
                Flag_Crea_Nodo_AppezzamentoParticella = 0
            End If

            Dim letturaDettaglioImpresa As New GIAS2GIAS_LOCALE.Esporta_Gias_Pubblico(
                objParametri_Server:=ObjParametri_Server,
                objParametri_utenti:=ObjParametri_Utenti,
                CartellaFileXml:=CartellaFileXml,
                FinestraTemp_Inizio:=listOfDataImport.DatiImpresa.ValiditaInizio_pianocolturale,
                FinestraTemp_Fine:=listOfDataImport.DatiImpresa.ValiditaFine_pianocolturale,
                Flag_Esporta_SOLO_ImpreseConImpianti:=Flag_Esporta_SOLO_ImpreseConImpianti,
                 Flag_Crea_Nodo_Impresa:=Flag_Crea_Nodo_Impresa,
                Flag_Crea_Nodo_Centro:=Flag_Crea_Nodo_Centro,
                Flag_Crea_Nodo_Particella:=Flag_Crea_Nodo_Particella,
                Flag_Crea_Nodo_Fabbricato:=Flag_Crea_Nodo_Fabbricato,
                Flag_Crea_Nodo_Campo:=Flag_Crea_Nodo_Campo,
                Flag_Crea_Nodo_CampoParticella:=Flag_Crea_Nodo_CampoParticella,
                Flag_Crea_Nodo_Appezzamento:=Flag_Crea_Nodo_Appezzamento,
                Flag_Crea_Nodo_AppezzamentoParticella:=Flag_Crea_Nodo_AppezzamentoParticella,
                Flag_Crea_Nodo_Impianto:=Flag_Crea_Nodo_Impianto,
                Flag_Crea_Nodo_Progetto:=Flag_Crea_Nodo_Progetto,
                FiltroPerOrganismoReferente:=0
            )


            Log_G2G.AppendLine(CStr(Date.Now) & " - " & "Filtro azienda.")
            'il codice è un CUAA, una piva oppure altro...
            If objOpzioni.CodiceImpresa_ORIGINE <> 0 Then

                Dim leggiPivaCuaa As New AgronicaCoreAnagrafeDAL.Imprese_Codici_Read

                letturaDettaglioImpresa.Filtro_Piva =
                leggiPivaCuaa.Piva_from_IdCodValCod(objOpzioni.CodiceImpresa_ORIGINE, codiceAzienda, ObjParametri_Server)
            Else
                letturaDettaglioImpresa.Filtro_Piva = codiceAzienda
            End If
            piva = CStr(letturaDettaglioImpresa.Filtro_Piva)


            If filtro1.Count = 0 OrElse filtro1.Contains("'" & letturaDettaglioImpresa.Filtro_Piva & "'") Then

                Log_G2G.AppendLine(CStr(Date.Now) & " - " & "Esporta_Dati.")

                Dim cfg As clsCfgImpostazioniTrasformazioni = Nothing
                If objOpzioni.ImpostazioniTrasformazioni <> "" Then
                    cfg = JsonConvert.DeserializeObject(Of clsCfgImpostazioniTrasformazioni)(objOpzioni.ImpostazioniTrasformazioni)
                End If

                Dim GEORiferimento_Cod As Integer = -1
                If listOfDataImport.DatiImpresa.Flagimporta_gis Then
                    If cfg IsNot Nothing Then
                        GEORiferimento_Cod = cfg.GEORiferimento_Cod
                    End If
                End If

                Dim strImpresa As String =
                    letturaDettaglioImpresa.Esporta_Dati(objOpzioni, listOfDataImport.DatiImpresa, CartellaFileXml, Flag_GestioneCodifiche, codice_GIAS, Log_G2G, Log_Errori)

                If strImpresa <> "" Then

                    If cfg IsNot Nothing Then
                        Log_G2G.AppendLine(CStr(Date.Now) & " - " & "ImpostazioniTrasformazioni.")
                        If cfg.PercorsoXsltAnagDet <> "" Then
                            Dim tr As New AgronicaCoreUtility.XsltUtils
                            rval.RispostaStringa = tr.trasformaDatoPercorsoFile(strImpresa, cfg.PercorsoXsltAnagDet)
                            ScriviXMLsuFile(CartellaFileXml, rval.RispostaStringa, piva & "_", "_TRASF")
                        Else
                            rval.RispostaStringa = strImpresa
                        End If
                    Else
                        Log_G2G.AppendLine(CStr(Date.Now) & " - " & "Non ci sono Trasformazioni da applicare.")
                        rval.RispostaStringa = strImpresa
                    End If

                    Log_G2G.AppendLine(CStr(Date.Now) & " - " & "Fine Gias 2 Gias --> " & NomeFunzione)
                    rval.RispostaOK = True
                    rval.Errore = ""

                Else
                    Log_G2G.AppendLine(CStr(Date.Now) & " - " & "strImpresa vuota ")
                    rval.RispostaOK = False
                    rval.Errore = "Non sono stati recuperati dati sull'azienda."
                    rval.RispostaStringa = ""
                End If
            Else
                rval.RispostaOK = False
                rval.Errore = "Richiesta non autorizzata."
                rval.RispostaStringa = ""
            End If
            'azienda autorizzata

        Catch ex As Exception
            messaggioErrori = CStr(Date.Now) & " - " & NomeFunzione & " Errore: " & ex.Message
            Log_Errori.AppendLine(messaggioErrori)
            rval.Errore = messaggioErrori
            rval.RispostaOK = False
        Finally

            cartellaLog = Path.Combine(cartellaLog, "G2G_ExportXml")
            If Not Directory.Exists(cartellaLog) Then
                Directory.CreateDirectory(cartellaLog)
            End If

            If Not cartellaLog.EndsWith("\") Then
                cartellaLog &= "\"
            End If

            rval.ParametroDue_stringa &= Log_G2G.ToString

            Dim Anno As String = Date.Today.Year.ToString
            Dim Mese As String = Right("00" & Date.Today.Month.ToString, 2)
            Dim Giorno As String = Right("00" & Date.Today.Day.ToString, 2)
            Dim Ora As String = Right("00" & Date.Now.Hour.ToString, 2)
            Dim Minuti As String = Right("00" & Date.Now.Minute.ToString, 2)
            Dim Secondi As String = Right("00" & Date.Now.Second.ToString, 2)
            Dim time As String = Anno & Mese & Giorno & "_" & Ora & Minuti & Secondi

            Dim NomeFileLog As String = piva & "_" & time & "_AnagDet_Log.txt"
            Dim NomeFileErrori As String = piva & "_" & time & "_AnagDet_Errori.txt"

            If Log_Errori.ToString = "" Then
                Log_Errori.Append("Nessun errore riscontrato.")
            End If

            My.Computer.FileSystem.WriteAllText(cartellaLog & NomeFileLog, Log_G2G.ToString, True)
            My.Computer.FileSystem.WriteAllText(cartellaLog & NomeFileErrori, Log_Errori.ToString, True)
            'My.Computer.FileSystem.WriteAllText(cartellaLog & ".\log_Riepilogo.txt", Log_Riepilogo.ToString, True)

        End Try


        Return rval

    End Function

    ''' <summary>
    ''' Estrae i dati in modalità "PULL" a partire dal nome della confiurazione XML presente sul db
    ''' </summary>
    ''' <remarks></remarks>
    Public Function GIAS_2_GIAS_PULL_DaIDConf_AnagList(ByVal CartellaFileXml As String) As RispostaStandard

        Dim rval As New RispostaStandard

        Const NomeFunzione As String = "GIAS_2_GIAS_PULL_DaIDConf_AnagList"

        Dim Log_G2G As New StringBuilder("")
        Dim Log_Errori As New StringBuilder("")
        'Dim Log_Riepilogo As New StringBuilder("")
        Dim sxml As String = Nothing
        Dim l As New AgronicaCoreG2GLocalDal.G2GLocal_R

        Dim messaggioErrori As String = ""

        Try
            Log_G2G.AppendLine(CStr(Date.Now) & " - " & "Inizio Gias 2 Gias --> " & NomeFunzione)

            Log_G2G.AppendLine(CStr(Date.Now) & " - " & "LeggiConfigurazioni")
            Dim dt As DataTable = l.LeggiConfigurazioni(ID_Cfg, ObjParametri_Server)

            '  Marco Grilli, 03/06/2014 18:20:30: se non trovo nessuna configurazione non quel nome, scrivo un errore ed esco dalla funzione.
            If (dt.Rows.Count <= 0) Then
                Dim ErroreCFG As String = CStr(Date.Now) & " - " & NomeFunzione & " Si è verificato il seguente errore: Nessuna configurazione trovata con l'id " & ID_Cfg
                Log_G2G.AppendLine(ErroreCFG)
                Log_Errori.AppendLine(ErroreCFG)
                rval.RispostaOK = False
                rval.Errore = ErroreCFG
                Return rval
            Else
                sxml = dt(0)(2) '  Marco Grilli, 04/06/2014 10:51:07: prelevo il campo con l'xml
            End If

            Log_G2G.AppendLine(CStr(Date.Now) & " - " & "Eseguo il Gias 2 Gias.")


            Dim imprese As XDocument = XDocument.Parse(sxml)
            Dim xElemCfg As XElement = imprese.<dati>.First

            Log_G2G.AppendLine(CStr(Date.Now) & " - " & "Gias2Gias_LIB.Funzioni_MasterG2G.LeggiOpzioniDaXml")
            Dim objOpzioni As Gias2Gias_LIB.clsOpzioni = Gias2Gias_LIB.Funzioni_MasterG2G.LeggiOpzioniDaXml(xElemCfg, "http://G2G")

            Try
                objOpzioni.FormatoOraZero = System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.LongTimePattern.ToLower.Replace("h", "0").Replace("m", "0").Replace("s", "0")

                Log_G2G.AppendLine(CStr(Date.Now) & " - " & "Parametri Utente: Formato data: " & System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.LongDatePattern &
                                                " - Formato ora zero: " & objOpzioni.FormatoOraZero)
            Catch ex As Exception
                objOpzioni.FormatoOraZero = "00.00.00"
                Dim err1 As String = CStr(Date.Now) & " - " & "Parametri Utente: Formato data non impostato da sistema. "
                Log_G2G.AppendLine(err1)
                rval.RispostaOK = False
                rval.Errore = err1
                Return rval
            End Try

            Log_G2G.AppendLine(CStr(Date.Now) & " - " & "GIAS_2_GIAS_Completaopzioni")
            GIAS_2_GIAS_Completaopzioni(objOpzioni, False)

            '=============================================================
            '======================== AVVIO LETTURE =======================
            '=============================================================

            ' VAnni: 17/5/2019: introdotto "DatiImpresa", "filtrone", lista impianti su aziende

            Log_G2G.AppendLine(CStr(Date.Now) & " - " & "ListaDatiDaImportareLetturaDaXDoc")
            Dim listOfDataImport As clsImportData = ListaDatiDaImportareLetturaDaXDoc(imprese)

            Log_G2G.AppendLine(CStr(Date.Now) & " - " & "ApplicaFiltroDatiPopolaListaImprese")
            ApplicaFiltroDatiPopolaListaImprese(objOpzioni, listOfDataImport)


            'cuore della funzione PULL
            'GIAS_2_GIASCiclaImprese(rval.ParametroDue_stringa, Log_G2G, Log_Errori, Log_Riepilogo, cartellaLog, imprese, True)

            Dim letturaXmlImprese As New AgronicaCoreAnagrafeBIZ.Impresa_R

            Dim filtro1 As List(Of String) = (From pp In listOfDataImport.Imprese
                                              Select "'" & pp.Piva_ORIGINE & "'").ToList()

            Dim xFiltroAggiuntivo As String = ""

            If filtro1.Count > 0 Then
                xFiltroAggiuntivo =
                    " IMPRESE.PIVA in (" & String.Join(",", filtro1.ToArray) & ")"
            End If


            Log_G2G.AppendLine(CStr(Date.Now) & " - " & "Impresa_Leggi")
            Dim strImpresa As String = letturaXmlImprese.Impresa_Leggi("", False, False, ObjParametri_Server, xFiltroAggiuntivo:=xFiltroAggiuntivo)

            If strImpresa <> "" Then

                Log_G2G.AppendLine(CStr(Date.Now) & " - " & "ImpostazioniTrasformazioni")
                If objOpzioni.ImpostazioniTrasformazioni <> "" Then
                    Dim jO As JObject = JObject.Parse(objOpzioni.ImpostazioniTrasformazioni)
                    Dim fileXslt As String = jO("PercorsoXsltAnagList")
                    If fileXslt <> "" Then
                        Dim tr As New AgronicaCoreUtility.XsltUtils
                        rval.RispostaStringa = tr.trasformaDatoPercorsoFile(strImpresa, fileXslt)
                        ScriviXMLsuFile(CartellaFileXml, rval.RispostaStringa, "", "_DatiImprese_TRASF")
                    Else
                        rval.RispostaStringa = strImpresa
                        ScriviXMLsuFile(CartellaFileXml, rval.RispostaStringa, "", "_DatiImprese")
                    End If
                Else
                    rval.RispostaStringa = strImpresa
                    ScriviXMLsuFile(CartellaFileXml, rval.RispostaStringa, "", "_DatiImprese")
                End If

                Log_G2G.AppendLine(CStr(Date.Now) & " - " & "Fine Gias 2 Gias --> " & NomeFunzione)
                rval.RispostaOK = True
                rval.Errore = ""
            Else
                Log_G2G.AppendLine(CStr(Date.Now) & " - " & "strImpresa vuota ")
                rval.RispostaOK = False
                rval.Errore = "Non è stato recuperato l'elenco delle aziende."
                rval.RispostaStringa = ""
            End If

        Catch ex As Exception
            messaggioErrori = CStr(Date.Now) & " - " & NomeFunzione & " Errore: " & ex.Message
            Log_Errori.AppendLine(messaggioErrori)
            rval.Errore = messaggioErrori
            rval.RispostaOK = False
            rval.RispostaStringa = ""
        Finally

            rval.ParametroDue_stringa &= Log_G2G.ToString

            'My.Computer.FileSystem.WriteAllText(cartellaLog & ".\log_G2G.txt", messaggioErrori, True)

            'Dim messaggioFinale As String = vbCrLf & CStr(Date.Now) & " - " & "Fine Gias 2 Gias --> " & NomeFunzione & vbCrLf & vbCrLf
            'My.Computer.FileSystem.WriteAllText(cartellaLog & ".\log_G2G.txt", messaggioFinale, True)

            'My.Computer.FileSystem.WriteAllText(cartellaLog & ".\log_Errori.txt", Log_Errori.ToString, True)
            'My.Computer.FileSystem.WriteAllText(cartellaLog & ".\log_Riepilogo.txt", Log_Riepilogo.ToString, True)

            cartellaLog = Path.Combine(cartellaLog, "G2G_ExportXml")
            If Not Directory.Exists(cartellaLog) Then
                Directory.CreateDirectory(cartellaLog)
            End If

            If Not cartellaLog.EndsWith("\") Then
                cartellaLog &= "\"
            End If

            Dim Anno As String = Date.Today.Year.ToString
            Dim Mese As String = Right("00" & Date.Today.Month.ToString, 2)
            Dim Giorno As String = Right("00" & Date.Today.Day.ToString, 2)
            Dim Ora As String = Right("00" & Date.Now.Hour.ToString, 2)
            Dim Minuti As String = Right("00" & Date.Now.Minute.ToString, 2)
            Dim Secondi As String = Right("00" & Date.Now.Second.ToString, 2)
            Dim time As String = Anno & Mese & Giorno & "_" & Ora & Minuti & Secondi

            Dim NomeFileLog As String = time & "_AnagList_Log.txt"
            Dim NomeFileErrori As String = time & "_AnagList_Errori.txt"

            If Log_Errori.ToString = "" Then
                Log_Errori.Append("Nessun errore riscontrato.")
            End If

            My.Computer.FileSystem.WriteAllText(cartellaLog & NomeFileLog, Log_G2G.ToString, True)
            My.Computer.FileSystem.WriteAllText(cartellaLog & NomeFileErrori, Log_Errori.ToString, True)
            'My.Computer.FileSystem.WriteAllText(cartellaLog & ".\log_Riepilogo.txt", Log_Riepilogo.ToString, True)

        End Try

        Return rval

    End Function

    ' restituisce la lista delle imprese G2G
    Public Function GIAS_2_GIASDatiImportazione(ByVal configurazione As String, ByRef objOpzioni As Gias2Gias_LIB.clsOpzioni) As clsImportData
        Dim imprese As XDocument = XDocument.Parse(configurazione)
        Dim xElemCfg As XElement = imprese.<dati>.First
        objOpzioni = Gias2Gias_LIB.Funzioni_MasterG2G.LeggiOpzioniDaXml(xElemCfg, "http://G2G")
        'GIAS_2_GIAS_Completaopzioni(objOpzioni, False)
        Dim datiImportazione As clsImportData = ListaDatiDaImportareLetturaDaXDoc(imprese)
        ApplicaFiltroDatiPopolaListaImprese(objOpzioni, datiImportazione)
        Return datiImportazione
    End Function

    ' scrive xml movimenti ricevuti
    Public Function GIAS_2_GS1_CarichiList(ByVal configurazione As String,
                                           ByVal configurazioneServizio As Configurazione_Servizio,
                                           ByVal piva As String,
                                           ByVal xml As String,
                                           ByVal nomeFile As String
                                           ) As RispostaStandard

        Const nomeFunzione = "GIAS_2_GS1_CarichiList"

        Dim rval As New RispostaStandard
        Dim objOpzioni As New Gias2Gias_LIB.clsOpzioni

        Try

            Dim percorso As String = Util.GetPercorsoInterscambio(configurazioneServizio.DirectoryFileImportazioni, "Input")
            Dim datiImportazione = GIAS_2_GIASDatiImportazione(configurazione, objOpzioni)
            Dim listaImprese As List(Of String) = (From pp In datiImportazione.Imprese Select pp.Piva_ORIGINE).ToList()

            If listaImprese.Count > 0 AndAlso Not listaImprese.Contains(piva) Then
                rval.RispostaOK = False
                rval.Errore = "Piva non valida"
                Return rval
            End If

            ' SECURITY - Verifico che il percorso di destinazione sia all'interno della cartella specificata
            Dim canonicalDestinationPath As String = Path.GetFullPath(percorso & nomefile)
            If Not canonicalDestinationPath.StartsWith(percorso, StringComparison.Ordinal) Then
                Throw New InvalidOperationException($"SECURITY - Salvataggio bloccato: il percorso di destinazione tenta di uscire dalla cartella di destinazione [{percorso}] specificata.")
            End If

            ' scrivo il file xml dei movimenti
            File.WriteAllText(percorso & nomeFile, xml)

            rval.RispostaOK = True
            rval.Errore = ""

        Catch ex As Exception
            rval.Errore = CStr(Date.Now) & " - " & nomeFunzione & " Errore: " & ex.Message
            rval.RispostaOK = False
            rval.RispostaStringa = ""
        End Try

        Return rval

    End Function



    ' esporta xml ricette brogliaccio
    Public Function GIAS_2_GS1_RicetteList(ByVal configurazione As String, ByVal configurazioneServizio As Configurazione_Servizio, ByRef objParametri_Interscambio As AgronicaCoreParametri) As RispostaStandard

        Const NomeFunzione As String = "GIAS_2_GS1_RicetteList"

        Dim rval As New RispostaStandard
        Dim objOpzioni As New Gias2Gias_LIB.clsOpzioni
        Dim percorso = Util.GetPercorsoInterscambio(configurazioneServizio.DirectoryFileEsportazioni, "")
        Dim percorsoLog = Util.GetPercorsoInterscambio(configurazioneServizio.DirectoryLOG, "")
        Dim messaggioErrori As String = ""

        Try

            Dim datiImportazione = GIAS_2_GIASDatiImportazione(configurazione, objOpzioni)
            Dim leggiRicetteDettaglio As New AgronicaCoreContabDAL.Ricette_Dettagli_R
            Dim leggiRicette As New AgronicaCoreG2GLocalDal.G2GRicette_R
            Dim listaOrdini As New List(Of Interscambio_Ordine.OrderType)
            Dim logErrori As New StringBuilder

            Dim origine As String = objOpzioni.SuperUser_CodFiscale_ORIGINE
            Dim destinazione As String = objOpzioni.SuperUser_CodFiscale_DESTINAZIONE
            Dim filtro_magazzino As String = "Cau_Mov IN ('" & CAU_SCARICO & "','" & CAU_CARICO & "')"
            Dim filtro_causali As String = "Cau_Mov IN ('" & CAU_TRATTAMENTO & "','" & CAU_RILIEVO_CAMPO & "','" & CAU_RILIEVO_RACCOLTA & "','" & CAU_LAVORAZIONE & "')"
            Dim filtro_dettagli As String = "Ricette_Dettagli.Elem_Cod NOT IN (0,1) AND (Ricette_Dettagli.Pro_Cod<>0 OR Ricette_Dettagli.Mat_Cod<>0) AND " & filtro_causali

            For Each impresa In datiImportazione.Imprese

                Dim piva_from = impresa.Piva_ORIGINE
                Dim piva_to = destinazione
                Dim g2gRicette As G2G_Ricette = leggiRicette.LeggiPerGias2GS1(impresa, piva_from, ObjParametri_Server)
                Dim listaRicette = g2gRicette.ricette_operazioni_insert.Concat(g2gRicette.ricette_operazioni_update).ToList

                For Each ricetta In listaRicette

                    ' solo brogliaccio
                    If ricetta.W_Anagrafica_Stati_Cod = enum_WWorflow_WAnagraficaStati.Esecuzione_ed_avanzamento_delle_ricette_Eseguita Then

                        Dim dettagli_magazzino = leggiRicetteDettaglio.Leggi(ricetta.Ricetta_Cod, ricetta.Ricetta_Operazione_Cod, 0, "", 0, 0, 0, 0, AGRODATAINIZIO, AGRODATAFINE, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, filtro_magazzino, "", ObjParametri_Server)

                        ' solo ricette senza magazzino
                        If dettagli_magazzino.Rows.Count = 0 Then

                            Dim dettagli = leggiRicetteDettaglio.Leggi(ricetta.Ricetta_Cod, ricetta.Ricetta_Operazione_Cod, 0, "", 0, 0, 0, 0, AGRODATAINIZIO, AGRODATAFINE, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni, filtro_dettagli, "", ObjParametri_Server)

                            If dettagli.Rows.Count > 0 Then
                                Dim ordine = Ordine_Utility.CreaOrderType(piva_from, piva_to, ricetta, dettagli, objParametri_Interscambio, logErrori)
                                If ordine IsNot Nothing Then
                                    listaOrdini.Add(ordine)
                                End If
                            End If

                        End If

                    End If

                Next

                For Each recode In g2gRicette.G2G_Ricette_Operazioni_Recode_delete
                    Dim ordine = Ordine_Utility.CancellaOrderType(piva_from, piva_to, recode)
                    If ordine IsNot Nothing Then
                        listaOrdini.Add(ordine)
                    End If
                Next

            Next

            ' restituisce xml ordini e salvandolo anche su disco
            If listaOrdini.Count > 0 Then

                Dim sbd = Ordine_Utility.CreaStandardBusinessDocument(origine, destinazione, "order")
                Dim orderMessage = Ordine_Utility.CreaOrderMessageType(sbd, listaOrdini)

                Dim x As New XmlSerializer(orderMessage.GetType)
                Using t As New StringWriter
                    x.Serialize(t, orderMessage)
                    Dim xml = t.ToString()
                    rval.RispostaStringa = xml
                    If Not String.IsNullOrEmpty(percorso) Then
                        ScriviXMLsuFile(percorso, xml, "", "_DatiRicette")
                    End If
                End Using

            End If

            messaggioErrori = logErrori.ToString
            rval.RispostaOK = True
            rval.Errore = messaggioErrori

        Catch ex As Exception

            messaggioErrori = CStr(Date.Now) & " - " & NomeFunzione & " Errore: " & ex.Message
            rval.Errore = messaggioErrori
            rval.RispostaOK = False
            rval.RispostaStringa = ""

        End Try

        ScriviLog(percorsoLog, messaggioErrori, "", "_RicetteList")

        Return rval

    End Function

    ' conferma ricezione ricette
    Public Function GIAS_2_GS1_ConfermaOrdine(ByVal configurazione As String, ByVal configurazioneServizio As Configurazione_Servizio, ByVal xml As String) As RispostaStandard

        Const NomeFunzione As String = "GIAS_2_GS1_ConfermaOrdine"

        Dim rval As New RispostaStandard
        Dim objOpzioni As New Gias2Gias_LIB.clsOpzioni
        Dim messaggioErrori As String = ""

        Try

            Dim percorso = Util.GetPercorsoInterscambio(configurazioneServizio.DirectoryFileEsportazioni, "")
            Dim datiImportazione = GIAS_2_GIASDatiImportazione(configurazione, objOpzioni)

            'Dim objHelper = New Gias2Gias_LIB.Funzioni(ObjParametri_Server)

            If Not String.IsNullOrEmpty(percorso) Then
                ScriviXMLsuFile(percorso, xml, "", "_DatiRicette_CONF")
            End If

            Dim response As New OrderResponseMessageType
            Dim x As New XmlSerializer(response.GetType)
            Using reader As TextReader = New StringReader(xml)
                response = x.Deserialize(reader)
            End Using

            Dim gefutils As New Gias_EF_Utility
            Dim EFConnString As String = gefutils.GetEntityConnectionString(ObjParametri_Server.StringaConnessione)
            Dim GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            Dim From_PivaSuperUser = response.StandardBusinessDocumentHeader.Receiver(0).Identifier.Value
            Dim To_PivaSuperUser = response.StandardBusinessDocumentHeader.Sender(0).Identifier.Value

            Dim pivasuperuser = ObjParametri_Server.PivaSuperUser
            Dim username = ObjParametri_Server.UsernameOperazione
            Dim data = Date.Now

            For Each orderResponse In response.orderResponse

                Dim codici() = Split(orderResponse.originalOrder.entityIdentification, "|")
                Dim Piva As String = codici(0)
                Dim Ricetta_Cod As Integer = codici(1)
                Dim Ricetta_Operazione_Cod As Integer = codici(2)

                Dim ricetta = (From r In GiasContext.Ricette_Operazioni Where r.Ricetta_SuperUser = From_PivaSuperUser AndAlso r.Ricetta_Cod = Ricetta_Cod AndAlso r.Ricetta_Operazione_Cod = Ricetta_Operazione_Cod).FirstOrDefault
                Dim recodeRicetta = (From rr In GiasContext.G2G_Recode_Ricette Where rr.From_PivaSuperUser = From_PivaSuperUser AndAlso rr.To_PivaSuperUser = To_PivaSuperUser AndAlso rr.From_Piva = Piva AndAlso rr.From_Ricetta_Cod = Ricetta_Cod).FirstOrDefault
                Dim recodeOperazione = (From rr In GiasContext.G2G_Recode_Ricette_Operazioni Where rr.From_PivaSuperUser = From_PivaSuperUser AndAlso rr.To_PivaSuperUser = To_PivaSuperUser AndAlso rr.From_Piva = Piva AndAlso rr.From_Ricetta_Cod = Ricetta_Cod AndAlso rr.From_Ricetta_Operazione_Cod = Ricetta_Operazione_Cod).FirstOrDefault

                If ricetta IsNot Nothing Then

                    If recodeRicetta Is Nothing Then

                        ' inserisce recode ricetta
                        recodeRicetta = New G2G_Recode_Ricette With {
                            .From_PivaSuperUser = From_PivaSuperUser,
                            .To_PivaSuperUser = To_PivaSuperUser,
                            .From_Piva = Piva,
                            .To_Piva = Piva,
                            .From_Ricetta_Cod = Ricetta_Cod,
                            .To_Ricetta_Cod = Ricetta_Cod,
                            .Username_Creazione = username,
                            .Username_Modifica = username,
                            .Data_Creazione = Date.Now,
                            .Data_Modifica = Date.Now,
                            .Validita_Inizio = AGRODATAINIZIO,
                            .Validita_Fine = AGRODATAFINE,
                            .inviato = "0",
                            .datainvio = data
                        }
                        GiasContext.G2G_Recode_Ricette.Add(recodeRicetta)

                    Else

                        ' modifica recode ricetta
                        recodeRicetta.Username_Modifica = username
                        recodeRicetta.Data_Modifica = Date.Now
                        recodeRicetta.datainvio = data
                        GiasContext.G2G_Recode_Ricette.Attach(recodeRicetta)
                        GiasContext.Entry(recodeRicetta).State = EntityState.Modified

                    End If

                    If recodeOperazione Is Nothing Then

                        ' inserisce recode ricetta operazione
                        recodeOperazione = New G2G_Recode_Ricette_Operazioni With {
                            .From_PivaSuperUser = From_PivaSuperUser,
                            .To_PivaSuperUser = To_PivaSuperUser,
                            .From_Piva = Piva,
                            .To_Piva = Piva,
                            .From_Ricetta_Cod = Ricetta_Cod,
                            .To_Ricetta_Cod = Ricetta_Cod,
                            .From_Ricetta_Operazione_Cod = Ricetta_Operazione_Cod,
                            .To_Ricetta_Operazione_Cod = Ricetta_Operazione_Cod,
                            .Username_Creazione = username,
                            .Username_Modifica = username,
                            .Data_Creazione = Date.Now,
                            .Data_Modifica = Date.Now,
                            .Validita_Inizio = AGRODATAINIZIO,
                            .Validita_Fine = AGRODATAFINE,
                            .inviato = "0",
                            .datainvio = data
                        }
                        GiasContext.G2G_Recode_Ricette_Operazioni.Add(recodeOperazione)

                    Else

                        ' modifica recode ricetta operazione
                        recodeOperazione.Username_Modifica = username
                        recodeOperazione.Data_Modifica = Date.Now
                        recodeOperazione.datainvio = data
                        GiasContext.G2G_Recode_Ricette_Operazioni.Attach(recodeOperazione)
                        GiasContext.Entry(recodeOperazione).State = EntityState.Modified

                    End If

                ElseIf recodeOperazione IsNot Nothing Then

                    ' cancello recode ricetta operazione
                    GiasContext.G2G_Recode_Ricette_Operazioni.Attach(recodeOperazione)
                    GiasContext.G2G_Recode_Ricette_Operazioni.Remove(recodeOperazione)


                    ' cancello recode ricetta
                    If recodeRicetta IsNot Nothing Then
                        GiasContext.G2G_Recode_Ricette.Attach(recodeRicetta)
                        GiasContext.G2G_Recode_Ricette.Remove(recodeRicetta)
                    End If

                End If

            Next

            GiasContext.SaveChanges()

            rval.RispostaOK = True
            rval.Errore = ""

        Catch ex As Exception

            messaggioErrori = CStr(Date.Now) & " - " & NomeFunzione & " Errore: " & ex.Message
            rval.Errore = messaggioErrori
            rval.RispostaOK = False
            rval.RispostaStringa = ""

        End Try

        Return rval

    End Function

    Public Function GIAS_2_GIASReverse(Piva As String, LinkWSImportaGias As String, ByRef Messaggio_di_Ritorno_Opzionale As String)

        Const NomeFunzione As String = "GIAS_2_GIASReverse"

        Dim nomeFileLog As String = ".\" & CStr(DateTime.Now.Year) & DateTime.Now.Month.ToString("D2") & DateTime.Now.Day.ToString("D2") & "log_G2G.txt"
        Dim nomeFileErrori As String = ".\" & CStr(DateTime.Now.Year) & DateTime.Now.Month.ToString("D2") & DateTime.Now.Day.ToString("D2") & "log_Errori.txt"
        Dim nomeFileRiepilogo As String = ".\" & CStr(DateTime.Now.Year) & DateTime.Now.Month.ToString("D2") & DateTime.Now.Day.ToString("D2") & "log_Riepilogo.txt"

        If cartellaLog Is Nothing OrElse cartellaLog = "" Then
            cartellaLog = "C:\GIASLan\LOG\G2G"
        End If

        Dim Log_G2G As New StringBuilder("")
        Dim Log_Errori As New StringBuilder("")
        Dim Log_Riepilogo As New StringBuilder("")
        Dim sxml As String = Nothing
        Dim l As New AgronicaCoreG2GLocalDal.G2GLocal_R
        Dim ret As Boolean = False '  Marco Grilli, 04/06/2014 11:28:18: variabile di ritorno
        Dim impostazioniLog As String = ""
        Dim messaggioErrori As String = ""

        Try
            Log_G2G.AppendLine(CStr(Date.Now) & " - " & "Inizio Gias 2 Gias --> " & NomeFunzione)
            Log_G2G.AppendLine(CStr(Date.Now) & " - " & "Leggo le configurazioni")

            Dim dt As DataTable = l.LeggiConfigurazioni(ID_Cfg, ObjParametri_Server)

            '  Marco Grilli, 03/06/2014 18:20:30: se non trovo nessuna configurazione non quel nome, scrivo un errore ed esco dalla funzione.
            If (dt.Rows.Count <= 0) Then
                Log_G2G.AppendLine(CStr(Date.Now) & " - " & NomeFunzione & " Si è verificato il seguente errore: Nessuna configurazione trovata con l'id " & ID_Cfg)
                Log_Errori.AppendLine(CStr(Date.Now) & " - " & NomeFunzione & " Si è verificato il seguente errore: Nessuna configurazione trovata con l'id " & ID_Cfg)
                Return False
            Else
                sxml = dt(0)(2) '  Marco Grilli, 04/06/2014 10:51:07: prelevo il campo con l'xml
            End If

            '  Marco Grilli, 03/06/2014 18:05:01: eseguo il G2G
            Log_G2G.AppendLine(CStr(Date.Now) & " - " & "Eseguo il Gias 2 Gias.")

            Dim imprese As XDocument = XDocument.Parse(sxml)

            Dim xN As XNamespace = "http://G2G"
            Dim xElemCfg As XElement = imprese.<dati>.First
            Dim configurazione As XElement = xElemCfg.Element(xN + "configurazione")
            If configurazione.Element(xN + "impostazioni_log") IsNot Nothing Then
                impostazioniLog = configurazione.Element(xN + "impostazioni_log").Value
            End If

            If Piva <> "" Then
                Dim xImprese = xElemCfg.<imprese>.First.<filtronerisultato_azienda>.First
                xImprese.Value = "{    ""tipo"": ""azienda"",    ""chiavi"": [ """ & Piva & """ ]  }"
            End If

            GIAS_2_GIASCiclaImprese_Reverse(Messaggio_di_Ritorno_Opzionale,
                                            Log_G2G,
                                            Log_Errori,
                                            Log_Riepilogo,
                                            cartellaLog,
                                            imprese,
                                            True,
                                            nomeFileLog,
                                            nomeFileErrori,
                                            nomeFileRiepilogo)

            Log_G2G.AppendLine(vbCrLf & CStr(Date.Now) & " - " & "Fine Gias 2 Gias --> " & NomeFunzione & vbCrLf)
            ret = True

        Catch ex As Exception

            messaggioErrori = CStr(Date.Now) & " - " & NomeFunzione & " Si è verificato il seguente errore: " & ex.Message
            Log_Errori.AppendLine(messaggioErrori)

        Finally

            Messaggio_di_Ritorno_Opzionale &= Log_G2G.ToString

            If impostazioniLog.ToLower() = "elabora" Then
                Messaggio_di_Ritorno_Opzionale = ElaboraLog(Messaggio_di_Ritorno_Opzionale)
            End If

            My.Computer.FileSystem.WriteAllText(cartellaLog & ".\log_G2G.txt", messaggioErrori, True)

            Dim messaggioFinale As String = vbCrLf & CStr(Date.Now) & " - " & "Fine Gias 2 Gias --> " & NomeFunzione & vbCrLf & vbCrLf
            Log_G2G.AppendLine(messaggioFinale)

            My.Computer.FileSystem.WriteAllText(cartellaLog & ".\log_G2G.txt", Log_G2G.ToString, True)

            My.Computer.FileSystem.WriteAllText(cartellaLog & ".\log_Errori.txt", Log_Errori.ToString, True)
            My.Computer.FileSystem.WriteAllText(cartellaLog & ".\log_Riepilogo.txt", Log_Riepilogo.ToString, True)

        End Try


        Return ret
    End Function

    Public Function Chiama_GIAS_2_GIASReverse(Piva As String, Gestore As Integer, LinkWSImportaGias As String, ByRef Messaggio_di_Ritorno_Opzionale As String)

        Const NomeFunzione As String = "GIAS_2_GIASReverse"

        If cartellaLog Is Nothing OrElse cartellaLog = "" Then
            cartellaLog = "C:\GIASLan\LOG\G2G"
        End If

        Dim Log_G2G As New StringBuilder("")
        Dim Log_Errori As New StringBuilder("")
        Dim Log_Riepilogo As New StringBuilder("")
        Dim sxml As String = Nothing
        Dim l As New AgronicaCoreG2GLocalDal.G2GLocal_R
        Dim ret As Boolean = False '  Marco Grilli, 04/06/2014 11:28:18: variabile di ritorno
        Dim impostazioniLog As String = ""
        Dim messaggioErrori As String = ""

        Try
            Log_G2G.AppendLine(CStr(Date.Now) & " - " & "Inizio Gias 2 Gias --> " & NomeFunzione)
            Log_G2G.AppendLine(CStr(Date.Now) & " - " & "Leggo le configurazioni")

            Dim dt As DataTable = l.LeggiConfigurazioni(ID_Cfg, ObjParametri_Server)

            '  Marco Grilli, 03/06/2014 18:20:30: se non trovo nessuna configurazione non quel nome, scrivo un errore ed esco dalla funzione.
            If (dt.Rows.Count <= 0) Then
                Log_G2G.AppendLine(CStr(Date.Now) & " - " & NomeFunzione & " Si è verificato il seguente errore: Nessuna configurazione trovata con l'id " & ID_Cfg)
                Log_Errori.AppendLine(CStr(Date.Now) & " - " & NomeFunzione & " Si è verificato il seguente errore: Nessuna configurazione trovata con l'id " & ID_Cfg)
                Return False
            Else
                sxml = dt(0)(2) '  Marco Grilli, 04/06/2014 10:51:07: prelevo il campo con l'xml
            End If

            '  Marco Grilli, 03/06/2014 18:05:01: eseguo il G2G
            Log_G2G.AppendLine(CStr(Date.Now) & " - " & "Eseguo il Gias 2 Gias.")

            Dim imprese As XDocument = XDocument.Parse(sxml)

            Dim xN As XNamespace = "http://G2G"
            Dim xElemCfg As XElement = imprese.<dati>.First
            Dim configurazione As XElement = xElemCfg.Element(xN + "configurazione")
            If configurazione.Element(xN + "impostazioni_log") IsNot Nothing Then
                impostazioniLog = configurazione.Element(xN + "impostazioni_log").Value
            End If

            If Piva <> "" Then
                Dim xImprese = xElemCfg.<imprese>.First.<filtronerisultato_azienda>.First
                xImprese.Value = "{    ""tipo"": ""azienda"",    ""chiavi"": [ """ & Piva & """ ]  }"
            End If

            Chiama_GIAS_2_GIASCiclaImprese_Reverse(Messaggio_di_Ritorno_Opzionale, Log_G2G, Log_Errori, Log_Riepilogo, cartellaLog, imprese, Gestore, True)

            Log_G2G.AppendLine(vbCrLf & CStr(Date.Now) & " - " & "Fine Gias 2 Gias --> " & NomeFunzione & vbCrLf)
            ret = True

        Catch ex As Exception

            messaggioErrori = CStr(Date.Now) & " - " & NomeFunzione & " Si è verificato il seguente errore: " & ex.Message
            Log_Errori.AppendLine(messaggioErrori)

        Finally

            Messaggio_di_Ritorno_Opzionale &= Log_G2G.ToString

            If impostazioniLog.ToLower() = "elabora" Then
                Messaggio_di_Ritorno_Opzionale = ElaboraLog(Messaggio_di_Ritorno_Opzionale)
            End If

            My.Computer.FileSystem.WriteAllText(cartellaLog & ".\log_G2G.txt", messaggioErrori, True)

            Dim messaggioFinale As String = vbCrLf & CStr(Date.Now) & " - " & "Fine Gias 2 Gias --> " & NomeFunzione & vbCrLf & vbCrLf
            My.Computer.FileSystem.WriteAllText(cartellaLog & ".\log_G2G.txt", messaggioFinale, True)

            My.Computer.FileSystem.WriteAllText(cartellaLog & ".\log_Errori.txt", Log_Errori.ToString, True)
            My.Computer.FileSystem.WriteAllText(cartellaLog & ".\log_Riepilogo.txt", Log_Riepilogo.ToString, True)

        End Try


        Return ret
    End Function

    Private Sub ScriviXMLsuFile(ByVal CartellaFileXml As String, ByVal StrXml As String, ByVal prefisso_nome As String, ByVal suffisso_nome As String)

        Try
            If CartellaFileXml <> "" Then

                Dim NomeFileOut As String
                Dim Anno As String = Date.Today.Year.ToString
                Dim Mese As String = Right("00" & Date.Today.Month.ToString, 2)
                Dim Giorno As String = Right("00" & Date.Today.Day.ToString, 2)
                Dim Ora As String = Right("00" & Date.Now.Hour.ToString, 2)
                Dim Minuti As String = Right("00" & Date.Now.Minute.ToString, 2)
                Dim Secondi As String = Right("00" & Date.Now.Second.ToString, 2)
                Dim time As String = Anno & Mese & Giorno & "_" & Ora & Minuti & Secondi

                NomeFileOut = CartellaFileXml & prefisso_nome & time & suffisso_nome & ".xml"

                Dim xmldoc As New Xml.XmlDocument
                xmldoc.LoadXml(StrXml)
                xmldoc.Save(NomeFileOut)

            End If
        Catch ex As Exception
            'se non salva il file non è un problema
        End Try

    End Sub

    ' scrivi log errori
    Public Shared Sub ScriviLog(ByVal cartellaLog As String, ByRef log As String, ByVal prefisso As String, ByVal suffisso As String)

        If Not String.IsNullOrEmpty(log) Then

            Dim Anno As String = Date.Today.Year.ToString
            Dim Mese As String = Right("00" & Date.Today.Month.ToString, 2)
            Dim Giorno As String = Right("00" & Date.Today.Day.ToString, 2)
            Dim Ora As String = Right("00" & Date.Now.Hour.ToString, 2)
            Dim Minuti As String = Right("00" & Date.Now.Minute.ToString, 2)
            Dim Secondi As String = Right("00" & Date.Now.Second.ToString, 2)
            Dim time As String = Anno & Mese & Giorno & "_" & Ora & Minuti & Secondi

            Dim NomeFileLog As String = prefisso & time & suffisso & "_Log.txt"

            My.Computer.FileSystem.WriteAllText(cartellaLog & NomeFileLog, log, True)

        End If

    End Sub

    ''' <summary>
    ''' Esegue il G2G in background a partire dal nome della confiurazione XML presente sul db (Marco G)
    ''' </summary>
    ''' <remarks></remarks>
    Public Function GIAS_2_GIAS_DaIDConf(ByRef Messaggio_di_Ritorno_Opzionale As String) As Boolean

        Const NomeFunzione As String = "GIAS_2_GIAS_DaNomeConf"

        Dim Log_G2G As New StringBuilder("")
        Dim Log_Errori As New StringBuilder("")
        Dim Log_Riepilogo As New StringBuilder("")
        Dim sxml As String = Nothing
        Dim l As New AgronicaCoreG2GLocalDal.G2GLocal_R
        Dim ret As Boolean = False '  Marco Grilli, 04/06/2014 11:28:18: variabile di ritorno
        Dim impostazioniLog As String = ""
        Dim messaggioErrori As String = ""

        Dim nomeFileLog As String = ".\" & CStr(DateTime.Now.Year) & DateTime.Now.Month.ToString("D2") & DateTime.Now.Day.ToString("D2") & "log_G2G.txt"
        Dim nomeFileErrori As String = ".\" & CStr(DateTime.Now.Year) & DateTime.Now.Month.ToString("D2") & DateTime.Now.Day.ToString("D2") & "log_Errori.txt"
        Dim nomeFileRiepilogo As String = ".\" & CStr(DateTime.Now.Year) & DateTime.Now.Month.ToString("D2") & DateTime.Now.Day.ToString("D2") & "log_Riepilogo.txt"

        Try
            Log_G2G.AppendLine(CStr(Date.Now) & " - " & "Inizio Gias 2 Gias --> " & NomeFunzione)
            Log_G2G.AppendLine(CStr(Date.Now) & " - " & "Leggo le configurazioni")

            Dim dt As DataTable = l.LeggiConfigurazioni(ID_Cfg, ObjParametri_Server)

            '  Marco Grilli, 03/06/2014 18:20:30: se non trovo nessuna configurazione non quel nome, scrivo un errore ed esco dalla funzione.
            If (dt.Rows.Count <= 0) Then
                Log_G2G.AppendLine(CStr(Date.Now) & " - " & NomeFunzione & " Si è verificato il seguente errore: Nessuna configurazione trovata con l'id " & ID_Cfg)
                Log_Errori.AppendLine(CStr(Date.Now) & " - " & NomeFunzione & " Si è verificato il seguente errore: Nessuna configurazione trovata con l'id " & ID_Cfg)
                Return False
            Else
                sxml = dt(0)(2) '  Marco Grilli, 04/06/2014 10:51:07: prelevo il campo con l'xml
            End If

            '  Marco Grilli, 03/06/2014 18:05:01: eseguo il G2G
            Log_G2G.AppendLine(CStr(Date.Now) & " - " & "Eseguo il Gias 2 Gias.")

            Dim imprese As XDocument = XDocument.Parse(sxml)

            Dim xN As XNamespace = "http://G2G"
            Dim xElemCfg As XElement = imprese.<dati>.First
            Dim configurazione As XElement = xElemCfg.Element(xN + "configurazione")
            If configurazione.Element(xN + "impostazioni_log") IsNot Nothing Then
                impostazioniLog = configurazione.Element(xN + "impostazioni_log").Value
            End If



            GIAS_2_GIASCiclaImprese(Messaggio_di_Ritorno_Opzionale, Log_G2G, Log_Errori, Log_Riepilogo, cartellaLog, imprese,
                                    FromServizio:=True,
                                        nomeFileLog:=nomeFileLog,
                                        nomeFileErrori:=nomeFileErrori,
                                        nomeFileRiepilogo:=nomeFileRiepilogo)

            Log_G2G.AppendLine(vbCrLf & CStr(Date.Now) & " - " & "Fine Gias 2 Gias --> " & NomeFunzione & vbCrLf)
            ret = True

        Catch ex As Exception

            messaggioErrori = CStr(Date.Now) & " - " & NomeFunzione & " Si è verificato il seguente errore: " & ex.Message
            Log_Errori.AppendLine(messaggioErrori)

            My.Computer.FileSystem.WriteAllText(cartellaLog & nomeFileErrori, Log_Errori.ToString, True)

        Finally

            Messaggio_di_Ritorno_Opzionale &= Log_G2G.ToString

            If impostazioniLog.ToLower() = "elabora" Then
                Messaggio_di_Ritorno_Opzionale = ElaboraLog(Messaggio_di_Ritorno_Opzionale)
            End If

            My.Computer.FileSystem.WriteAllText(cartellaLog & nomeFileLog, messaggioErrori, True)

            Dim messaggioFinale As String = vbCrLf & CStr(Date.Now) & " - " & "Fine Gias 2 Gias --> " & NomeFunzione & vbCrLf & vbCrLf
            Log_G2G.AppendLine(messaggioFinale)
            My.Computer.FileSystem.WriteAllText(cartellaLog & nomeFileLog, Log_G2G.ToString, True)

            My.Computer.FileSystem.WriteAllText(cartellaLog & nomeFileErrori, Log_Errori.ToString, True)
            My.Computer.FileSystem.WriteAllText(cartellaLog & nomeFileRiepilogo, Log_Riepilogo.ToString, True)

        End Try


        Return ret

    End Function

    Public Shared Function ElaboraLog(ByVal log As String) As String

        Dim piva = ""
        Dim logInizio = ""
        Dim logFine = ""
        Dim logErrori = ""
        Dim pivaErrori = ""
        Dim logImprese = ""
        Dim numImprese = 0
        Dim numImpreseErrori = 0
        Dim lines = log.Split(vbCrLf)

        For line = 0 To lines.Length - 1
            Dim errore = lines(line).ToLower().IndexOf("error") <> -1
            Dim fine = lines(line).IndexOf("Fine Gias 2 Gias") <> -1

            If lines(line).IndexOf("----- Impresa") <> -1 Then

                If logImprese.ToLower().IndexOf("error") <> -1 Then
                    logErrori += logImprese & vbCrLf
                    If piva <> "" Then pivaErrori += (If(pivaErrori <> "", ",", "")) & """" & piva & """"
                    piva = ""
                    logImprese = ""
                    numImpreseErrori += 1
                End If

                Dim index = lines(line).IndexOf(" - ")
                piva = If(index > 0, Mid(lines(line), index + 4, 11), "")
                logImprese = lines(line) & vbCrLf
                numImprese += 1

            ElseIf errore Then
                logImprese += lines(line) & vbCrLf
            ElseIf numImprese = 0 Then
                logInizio += lines(line) & vbCrLf
            ElseIf fine Then
                logFine += lines(line) & vbCrLf
            End If

        Next

        If logImprese.ToLower().IndexOf("error") <> -1 Then
            logErrori += logImprese & vbCrLf
            If piva <> "" Then pivaErrori += (If(pivaErrori <> "", ", ", "")) & """" & piva & """"
            numImpreseErrori += 1
        End If

        logErrori = logInizio & logErrori & logFine

        If pivaErrori <> "" Then
            logErrori = numImpreseErrori & " imprese con errori su " & numImprese & " totali: " & pivaErrori & vbCrLf & vbCrLf & logErrori
        Else
            logErrori = "Nessun errore, imprese trasferite: " & numImprese & vbCrLf & vbCrLf & logErrori
        End If

        Return logErrori

    End Function

    Public Shared Function GetConfigurazione(x As XElement) As String
        If x Is Nothing Then
            Return ""
        End If

        Dim elem As XElement = x.<configurazione>.FirstOrDefault

        If elem IsNot Nothing Then
            Return elem.Value
        Else
            Return ""
        End If

    End Function

    Public Shared Function GetValiditaInizio(x As XElement) As Date

        If x Is Nothing Then
            Return AGRODATAINIZIO
        End If

        Dim elem As XElement = x.<validita_inizio>.FirstOrDefault

        If elem IsNot Nothing Then
            Return elem.Value
        Else
            Return AGRODATAINIZIO
        End If

    End Function


    Public Shared Function GetValiditafine(x As XElement) As Date

        If x Is Nothing Then
            Return AGRODATAFINE
        End If

        Dim elem As XElement = x.<validita_fine>.FirstOrDefault

        If elem IsNot Nothing Then
            Return elem.Value
        Else
            Return AGRODATAFINE
        End If

    End Function

    Public Sub GIAS_2_GIASCiclaImprese(ByRef messaggioDiRitornoOpzionale As String,
                                       ByRef Log_G2G As StringBuilder,
                                       ByRef Log_Errori As StringBuilder,
                                       ByRef Log_Riepilogo As StringBuilder,
                                       ByVal nomeCartella As String,
                                       ByVal imprese As XDocument,
                                       ByVal FromServizio As Boolean,
                                       nomeFileLog As String,
                                       nomeFileErrori As String,
                                       nomeFileRiepilogo As String)

        Dim xElemCfg As XElement = imprese.<dati>.First

        Dim objOpzioni As Gias2Gias_LIB.clsOpzioni = Gias2Gias_LIB.Funzioni_MasterG2G.LeggiOpzioniDaXml(xElemCfg, "http://G2G")

        Try
            objOpzioni.FormatoOraZero = System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.LongTimePattern.ToLower.Replace("h", "0").Replace("m", "0").Replace("s", "0")

            Log_G2G.AppendLine(CStr(Date.Now) & " - " & "Parametri Utente: Formato data: " & System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.LongDatePattern &
                                                " - Formato ora zero: " & objOpzioni.FormatoOraZero)
        Catch ex As Exception
            objOpzioni.FormatoOraZero = "00.00.00"
            Log_G2G.AppendLine(CStr(Date.Now) & " - " & "Parametri Utente: Formato data non impostato da sistema. ")
            Exit Sub
        End Try

        GIAS_2_GIAS_Completaopzioni(objOpzioni, FromServizio)

        '=============================================================
        '======================== AVVIO IMPORT =======================
        '=============================================================

        ' VAnni: 17/5/2019: introdotto "DatiImpresa", "filtrone", lista impianti su aziende

        Dim listOfDataImport As clsImportData = ListaDatiDaImportareLetturaDaXDoc(imprese)

        ApplicaFiltroDatiPopolaListaImprese(objOpzioni, listOfDataImport)


        '  Vanni, 20/05/2014 10:53:28: leggo solo quello che non è stato inviato.
        objOpzioni.objParametri_Server_GIAS_ORIGINE.FlagVisibilita = AgronicaCoreParametri.enumVisibilita.visibilita_SoloNonInviati

        Dim i As Integer = 0
        Dim totale As Single = listOfDataImport.Imprese.Count
        Dim errFlag As Integer
        Dim messaggioDiRitorno As New StringBuilder

        For Each impresa In listOfDataImport.Imprese

            ' VAnni: 26/6/2019: al primo giro, riporto i dati G2G_Recode sull'origine
            If i = 0 Then
                'AL PRIMO GIRO
                'allineamento generale recode ....
                If listOfDataImport.DatiGlobali.configurazione_recodes <> "0" Then
                    Dim DataRiferimento As Date = AGRODATAINIZIO
                    ' se presente la configurazione recodes imposto la data riferimento come now - num. ore impostate (Es: now - 24)
                    If listOfDataImport.DatiGlobali.configurazione_recodes <> "" Then
                        DataRiferimento = DateAdd(DateInterval.Hour, -CDbl(listOfDataImport.DatiGlobali.configurazione_recodes), Date.Now)
                    End If
                    _objFunzioni_Global.Elabora_XML_Recodes_Salva(
                        objOpzioni,
                        DataRiferimento,
                        Log_G2G,
                        Log_Errori,
                        Log_Riepilogo)
                End If


            End If


            'If impresa.Piva_ORIGINE = objOpzioni.objParametri_Server_GIAS_ORIGINE.PivaSuperUser Then
            '  Vanni, 20/05/2014 17:02:46: vanni, nascondo
            'If i = 0 Then
            '    'AL PRIMO GIRO
            '    'trasferisco anche tutti i dati che sono globali
            '    'i default globali della profilazione li trasferisco dopo, durante l'ultima impresa

            '    If listOfDataImport.DatiGlobali.Flagimporta_note Then
            '        _objFunzioni_Global.Elabora_Note_Salva(objOpzioni, _
            '                                             Log_G2G, _
            '                                            Log_Errori, _
            '                                            Log_Riepilogo)
            '    End If

            '    _objFunzioni_Global.Elabora_Rapporti_Contabili_Salva(objOpzioni, _
            '                                                         Log_G2G, _
            '                                                        Log_Errori, _
            '                                                        Log_Riepilogo)
            'End If

            'copia dell'impresa
            RaiseEvent progress(totale, i, 2, 2)

            If objOpzioni.isGias2Gias_local Then

                G2G_Gestione_Impresa(
                                listOfDataImport.DatiGlobali,
                                impresa,
                                objOpzioni,
                                errFlag,
                                Log_G2G,
                                Log_Riepilogo,
                                Log_Errori,
                                i + 1,
                                totale)

            Else

                G2G_Gestione_Impresa_WS(
                                listOfDataImport.DatiGlobali,
                                listOfDataImport.Imprese,
                                impresa,
                                objOpzioni,
                                errFlag,
                                Log_G2G,
                                Log_Riepilogo,
                                Log_Errori,
                                i + 1,
                                totale,
                                nomeCartella,
                                nomeFileLog,
                                nomeFileErrori,
                                messaggioDiRitorno)

            End If

            i += 1
            My.Computer.FileSystem.WriteAllText(nomeCartella & nomeFileLog, Log_G2G.ToString, True)
            My.Computer.FileSystem.WriteAllText(nomeCartella & nomeFileErrori, Log_Errori.ToString, True)
            My.Computer.FileSystem.WriteAllText(nomeCartella & nomeFileRiepilogo, Log_Riepilogo.ToString, True)

            messaggioDiRitorno.Append(Log_G2G.ToString)

            Log_G2G.Length = 0
            Log_Errori.Length = 0
            Log_Riepilogo.Length = 0
        Next

        messaggioDiRitornoOpzionale &= messaggioDiRitorno.ToString

    End Sub

    Public Sub GIAS_2_GIASCiclaImprese_Reverse(ByRef messaggioDiRitornoOpzionale As String,
                                               ByRef Log_G2G As StringBuilder,
                                               ByRef Log_Errori As StringBuilder,
                                               ByRef Log_Riepilogo As StringBuilder,
                                               ByVal nomeCartella As String,
                                               ByVal imprese As XDocument,
                                               ByVal FromServizio As Boolean,
                                               nomeFileLog As String,
                                               nomeFileErrori As String,
                                               nomeFileRiepilogo As String)

        Dim xElemCfg As XElement = imprese.<dati>.First

        Dim objOpzioni As Gias2Gias_LIB.clsOpzioni = Gias2Gias_LIB.Funzioni_MasterG2G.LeggiOpzioniDaXml(xElemCfg, "http://G2G")

        Try
            objOpzioni.FormatoOraZero = System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.LongTimePattern.ToLower.Replace("h", "0").Replace("m", "0").Replace("s", "0")

            Log_G2G.AppendLine(CStr(Date.Now) & " - " & "Parametri Utente: Formato data: " & System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.LongDatePattern &
                                                " - Formato ora zero: " & objOpzioni.FormatoOraZero)
        Catch ex As Exception
            objOpzioni.FormatoOraZero = "00.00.00"
            Log_G2G.AppendLine(CStr(Date.Now) & " - " & "Parametri Utente: Formato data non impostato da sistema. ")
            Exit Sub
        End Try

        GIAS_2_GIAS_Completaopzioni(objOpzioni, FromServizio, Log_G2G)
        '=============================================================
        '======================== AVVIO IMPORT =======================
        '=============================================================

        ' VAnni: 17/5/2019: introdotto "DatiImpresa", "filtrone", lista impianti su aziende

        Dim listOfDataImport As clsImportData = ListaDatiDaImportareLetturaDaXDoc(imprese)

        ApplicaFiltroDatiPopolaListaImprese(objOpzioni, listOfDataImport)

        '  Vanni, 20/05/2014 10:53:28: leggo solo quello che non è stato inviato.
        objOpzioni.objParametri_Server_GIAS_ORIGINE.FlagVisibilita = AgronicaCoreParametri.enumVisibilita.visibilita_SoloNonInviati

        Dim i As Integer = 0
        Dim totale As Single = listOfDataImport.Imprese.Count
        Dim errFlag As Integer
        Dim messaggioDiRitorno As New StringBuilder
        For Each impresa In listOfDataImport.Imprese

            If objOpzioni.isGias2Gias_local Then

                Throw New Exception("G2G Local Reverse non implementato")

            Else

                Log_G2G.AppendLine(CStr(Date.Now) & " - " & " D ")

                G2G_Gestione_Impresa_WS_Reverse(
                                listOfDataImport.DatiGlobali,
                                listOfDataImport.Imprese,
                                impresa,
                                objOpzioni,
                                errFlag,
                                Log_G2G,
                                Log_Riepilogo,
                                Log_Errori,
                                i + 1,
                                totale,
                                nomeCartella,
                                nomeFileLog,
                                nomeFileErrori,
                                messaggioDiRitorno)

            End If

            i += 1
            My.Computer.FileSystem.WriteAllText(nomeCartella & ".\log_G2G.txt", Log_G2G.ToString, True)
            My.Computer.FileSystem.WriteAllText(nomeCartella & ".\log_Errori.txt", Log_Errori.ToString, True)
            My.Computer.FileSystem.WriteAllText(nomeCartella & ".\log_Riepilogo.txt", Log_Riepilogo.ToString, True)

            messaggioDiRitorno.Append(Log_G2G.ToString)

            Log_G2G.Length = 0
            Log_Errori.Length = 0
            Log_Riepilogo.Length = 0
        Next

        messaggioDiRitornoOpzionale &= Log_G2G.ToString

    End Sub

    Public Sub Chiama_GIAS_2_GIASCiclaImprese_Reverse(ByRef messaggioDiRitornoOpzionale As String, ByRef Log_G2G As StringBuilder, ByRef Log_Errori As StringBuilder, ByRef Log_Riepilogo As StringBuilder, ByVal nomeCartella As String, ByVal imprese As XDocument, Gestore As Integer, Optional ByVal FromServizio As Boolean = False)

        Dim xElemCfg As XElement = imprese.<dati>.First

        Dim objOpzioni As Gias2Gias_LIB.clsOpzioni = Gias2Gias_LIB.Funzioni_MasterG2G.LeggiOpzioniDaXml(xElemCfg, "http://G2G")

        Try
            objOpzioni.FormatoOraZero = System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.LongTimePattern.ToLower.Replace("h", "0").Replace("m", "0").Replace("s", "0")

            Log_G2G.AppendLine(CStr(Date.Now) & " - " & "Parametri Utente: Formato data: " & System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.LongDatePattern &
                                                " - Formato ora zero: " & objOpzioni.FormatoOraZero)
        Catch ex As Exception
            objOpzioni.FormatoOraZero = "00.00.00"
            Log_G2G.AppendLine(CStr(Date.Now) & " - " & "Parametri Utente: Formato data non impostato da sistema. ")
            Exit Sub
        End Try

        GIAS_2_GIAS_Completaopzioni(objOpzioni, FromServizio)

        '=============================================================
        '======================== AVVIO IMPORT =======================
        '=============================================================

        ' VAnni: 17/5/2019: introdotto "DatiImpresa", "filtrone", lista impianti su aziende

        Dim listOfDataImport As clsImportData = ListaDatiDaImportareLetturaDaXDoc(imprese)

        ApplicaFiltroDatiPopolaListaImprese(objOpzioni, listOfDataImport)


        '  Vanni, 20/05/2014 10:53:28: leggo solo quello che non è stato inviato.
        objOpzioni.objParametri_Server_GIAS_ORIGINE.FlagVisibilita = AgronicaCoreParametri.enumVisibilita.visibilita_SoloNonInviati

        Dim i As Integer = 0
        Dim totale As Single = listOfDataImport.Imprese.Count
        Dim errFlag As Integer


        messaggioDiRitornoOpzionale = ""
        For Each impresa In listOfDataImport.Imprese

            If objOpzioni.isGias2Gias_local Then

                Throw New Exception("G2G Local Reverse non implementato")

            Else

                Chiama_G2G_Gestione_Impresa_WS_Reverse(
                                listOfDataImport.DatiGlobali,
                                listOfDataImport.Imprese,
                                impresa,
                                objOpzioni,
                                errFlag,
                                Log_G2G,
                                Log_Riepilogo,
                                Log_Errori,
                                i + 1,
                                totale,
                                Gestore,
                                messaggioDiRitornoOpzionale)

            End If

            i += 1
            My.Computer.FileSystem.WriteAllText(nomeCartella & ".\log_G2G.txt", Log_G2G.ToString, True)
            My.Computer.FileSystem.WriteAllText(nomeCartella & ".\log_Errori.txt", Log_Errori.ToString, True)
            My.Computer.FileSystem.WriteAllText(nomeCartella & ".\log_Riepilogo.txt", Log_Riepilogo.ToString, True)

            Log_G2G.Length = 0
            Log_Errori.Length = 0
            Log_Riepilogo.Length = 0
        Next

    End Sub

    Private Shared Sub ApplicaFiltroDatiPopolaListaImprese(objOpzioni As Gias2Gias_LIB.clsOpzioni, listOfDataImport As clsImportData)

        Dim OUTPUT_dt_x_export_azienda As DataTable = Nothing
        Dim OUTPUT_dt_x_export_impianto As DataTable = Nothing
        Dim OUTPUT_dt_x_export_fabbricato As DataTable = Nothing

        Dim xFiltrone As New AgronicaCoreFiltroneBIZ.Filtrone


        'leggo da filtrone attraverso il risultato della ricerca
        If listOfDataImport.TipoUtilizzoFiltroneInG2G = enum_TipoUtilizzoFiltroneInG2G.ApplicaFiltroneUsaRisultati Then


            'applicazione del filtro di ricerca se richiesto ...

            EstraiListaImpiantiEdAziendeDaFiltroRicerca(objOpzioni, listOfDataImport, OUTPUT_dt_x_export_azienda, OUTPUT_dt_x_export_impianto)

        End If

        Dim di As clsDatiImpresa = listOfDataImport.DatiImpresa


        'risultato rispetto alle aziende (si somma a quanto estratto prima)
        If Not String.IsNullOrEmpty(listOfDataImport.filtronerisultato_azienda) Then


            Dim JsonRichiesta2 As jsonProseguiSelezionati =
                Newtonsoft.Json.JsonConvert.DeserializeObject(Of jsonProseguiSelezionati)(listOfDataImport.filtronerisultato_azienda)


            If OUTPUT_dt_x_export_azienda Is Nothing Then
                OUTPUT_dt_x_export_azienda = xFiltrone.oggettoFiltroAzienda_CreaDT()
            End If

            xFiltrone.oggettoFiltroAzienda(JsonRichiesta2, OUTPUT_dt_x_export_azienda)

        End If


        'risultato rispetto agli impianti (si somma a quanto estratto prima)
        If Not String.IsNullOrEmpty(listOfDataImport.filtronerisultato) Then


            Dim JsonRichiesta2 As jsonProseguiSelezionati =
                Newtonsoft.Json.JsonConvert.DeserializeObject(Of jsonProseguiSelezionati)(listOfDataImport.filtronerisultato)

            If JsonRichiesta2.tipo = "azienda" Then

                If OUTPUT_dt_x_export_azienda Is Nothing Then
                    OUTPUT_dt_x_export_azienda = xFiltrone.oggettoFiltroAzienda_CreaDT()
                End If

                xFiltrone.oggettoFiltroAzienda(JsonRichiesta2, OUTPUT_dt_x_export_azienda)

            ElseIf JsonRichiesta2.tipo = "impianto" Then

                If OUTPUT_dt_x_export_impianto Is Nothing Then
                    OUTPUT_dt_x_export_impianto = xFiltrone.oggettoFiltroImpianti_CreaDT()
                End If

                xFiltrone.oggettoFiltroImpianti(JsonRichiesta2, OUTPUT_dt_x_export_impianto)

            ElseIf JsonRichiesta2.tipo = "fabbricato" Then

                If OUTPUT_dt_x_export_fabbricato Is Nothing Then
                    OUTPUT_dt_x_export_fabbricato = xFiltrone.oggettoFiltroFabbricati_CreaDT()
                End If

                xFiltrone.oggettoFiltroFabbricati(JsonRichiesta2, OUTPUT_dt_x_export_fabbricato)

            End If

        End If


        'accodo per ciascuna azienda un impianto "fittizio"
        If OUTPUT_dt_x_export_azienda IsNot Nothing Then

            If OUTPUT_dt_x_export_impianto Is Nothing Then
                OUTPUT_dt_x_export_impianto =
                xFiltrone.oggettoFiltroImpianti_CreaDT()
            End If

            xFiltrone.DataTableFiltroAzienda(OUTPUT_dt_x_export_azienda, OUTPUT_dt_x_export_impianto)

        End If

        'ricavo le imprese ed i centri ... 
        If OUTPUT_dt_x_export_impianto IsNot Nothing Then

            AccodaAziendeInXmlDataListaImpianti(listOfDataImport, OUTPUT_dt_x_export_impianto, di)

        Else

            ' su ciascuna impresa riporto ugualmente la configurazione (chiaramente dovrà essere pre-popolato)           
            For Each impCur1 In listOfDataImport.Imprese
                RiportaConfigurazioneGeneraleSuImpresaCorrente(di, impCur1.Piva_ORIGINE, impCur1)
            Next

        End If
        'fine impianti su datatable

        If OUTPUT_dt_x_export_fabbricato IsNot Nothing Then
            AccodaAziendeInXmlDataListaFabbricati(listOfDataImport, OUTPUT_dt_x_export_fabbricato, di)
        End If

    End Sub

    Public Shared Function ListaDatiDaImportareLetturaDaXDoc(imprese As XDocument, Optional ByVal bSoloFiltrone As Boolean = False) As clsImportData


        Select Case bSoloFiltrone

            Case True

                Return (From d In imprese.<dati>
                        Select New clsImportData With {
                            .filtrone = d.<imprese>.<filtrone>.Value,
                            .filtronerisultato = d.<imprese>.<filtronerisultato>.Value,
                            .filtronerisultato_azienda = d.<imprese>.<filtronerisultato_azienda>.Value
                            }).FirstOrDefault

            Case False


                Return (From d In imprese.<dati>
                        Select New clsImportData With {
                            .DatiGlobali = New clsDatiGlobali With {
                                .Flagallinea_recodes = d.<imprese>.<datiglobali>.<flagallinea_recodes>.ToList.Count > 0,
                                .Flagimporta_note = d.<imprese>.<datiglobali>.<flagimporta_note>.ToList.Count > 0,
                                .Flagimporta_profilazione = d.<imprese>.<datiglobali>.<flagimporta_profilazione>.ToList.Count > 0,
                                .Flagimporta_cac_codifica = d.<imprese>.<datiglobali>.<flagimporta_cac_codifica>.ToList.Count > 0,
                                .Flagimporta_pianicampionamento = d.<imprese>.<datiglobali>.<flagimporta_pianicampionamento>.ToList.Count > 0,
                                .Flagimporta_contatti = d.<imprese>.<datiglobali>.<flagimporta_contatti>.ToList.Count > 0,
                                .Flagimporta_macchine = d.<imprese>.<datiglobali>.<flagimporta_macchine>.ToList.Count > 0,
                                .Flagimporta_materieprime = d.<imprese>.<datiglobali>.<flagimporta_materieprime>.ToList.Count > 0,
                                .Flagimporta_materieprimecampionature = d.<imprese>.<datiglobali>.<flagimporta_materieprimecampionature>.ToList.Count > 0,
                                .Flagimporta_analisicondivise = d.<imprese>.<datiglobali>.<flagimporta_analisicondivise>.ToList.Count > 0,
                                .Flagimporta_attivita = d.<imprese>.<datiglobali>.<flagimporta_attivita>.ToList.Count > 0,
                                .Validita_Inizio_note = GetValiditaInizio(d.<imprese>.<datiglobali>.<flagimporta_note>.FirstOrDefault),
                                .Validita_Inizio_profilazione = GetValiditaInizio(d.<imprese>.<datiglobali>.<flagimporta_profilazione>.FirstOrDefault),
                                .Validita_Inizio_cac_codifica = GetValiditaInizio(d.<imprese>.<datiglobali>.<flagimporta_cac_codifica>.FirstOrDefault),
                                .Validita_Inizio_pianicampionamento = GetValiditaInizio(d.<imprese>.<datiglobali>.<flagimporta_pianicampionamento>.FirstOrDefault),
                                .Validita_Inizio_contatti = GetValiditaInizio(d.<imprese>.<datiglobali>.<flagimporta_contatti>.FirstOrDefault),
                                .Validita_Inizio_macchine = GetValiditaInizio(d.<imprese>.<datiglobali>.<flagimporta_macchine>.FirstOrDefault),
                                .Validita_Inizio_materieprime = GetValiditaInizio(d.<imprese>.<datiglobali>.<flagimporta_materieprime>.FirstOrDefault),
                                .Validita_Inizio_materieprimecampionature = GetValiditaInizio(d.<imprese>.<datiglobali>.<flagimporta_materieprimecampionature>.FirstOrDefault),
                                .Validita_Inizio_analisicondivise = GetValiditaInizio(d.<imprese>.<datiglobali>.<flagimporta_analisicondivise>.FirstOrDefault),
                                .Validita_Fine_note = GetValiditafine(d.<imprese>.<datiglobali>.<flagimporta_note>.FirstOrDefault),
                                .Validita_Fine_profilazione = GetValiditafine(d.<imprese>.<datiglobali>.<flagimporta_profilazione>.FirstOrDefault),
                                .Validita_Fine_cac_codifica = GetValiditafine(d.<imprese>.<datiglobali>.<flagimporta_cac_codifica>.FirstOrDefault),
                                .Validita_Fine_pianicampionamento = GetValiditafine(d.<imprese>.<datiglobali>.<flagimporta_pianicampionamento>.FirstOrDefault),
                                .Validita_Fine_contatti = GetValiditafine(d.<imprese>.<datiglobali>.<flagimporta_contatti>.FirstOrDefault),
                                .Validita_Fine_macchine = GetValiditafine(d.<imprese>.<datiglobali>.<flagimporta_macchine>.FirstOrDefault),
                                .Validita_Fine_materieprime = GetValiditafine(d.<imprese>.<datiglobali>.<flagimporta_materieprime>.FirstOrDefault),
                                .Validita_Fine_materieprimecampionature = GetValiditafine(d.<imprese>.<datiglobali>.<flagimporta_materieprimecampionature>.FirstOrDefault),
                                .Validita_Fine_analisicondivise = GetValiditafine(d.<imprese>.<datiglobali>.<flagimporta_analisicondivise>.FirstOrDefault),
                                .configurazione_note = GetConfigurazione(d.<imprese>.<datiglobali>.<flagimporta_note>.FirstOrDefault),
                                .configurazione_profilazione = GetConfigurazione(d.<imprese>.<datiglobali>.<flagimporta_profilazione>.FirstOrDefault),
                                .configurazione_cac_codifica = GetConfigurazione(d.<imprese>.<datiglobali>.<flagimporta_cac_codifica>.FirstOrDefault),
                                .configurazione_pianicampionamento = GetConfigurazione(d.<imprese>.<datiglobali>.<flagimporta_pianicampionamento>.FirstOrDefault),
                                .configurazione_contatti = GetConfigurazione(d.<imprese>.<datiglobali>.<flagimporta_contatti>.FirstOrDefault),
                                .configurazione_macchine = GetConfigurazione(d.<imprese>.<datiglobali>.<flagimporta_macchine>.FirstOrDefault),
                                .configurazione_recodes = GetConfigurazione(d.<imprese>.<datiglobali>.<flagallinea_recodes>.FirstOrDefault)
                            },
                            .filtrone = d.<imprese>.<filtrone>.Value,
                            .filtronerisultato = d.<imprese>.<filtronerisultato>.Value,
                            .filtronerisultato_azienda = d.<imprese>.<filtronerisultato_azienda>.Value,
                            .TipoUtilizzoFiltroneInG2G = d.<imprese>.<TipoUtilizzoFiltroneInG2G>.Value,
                            .DatiImpresa = New clsDatiImpresa With {
                                .FlagImporta_Audit = d.<imprese>.<datiimpresa>.<flagimporta_audit>.ToList.Count > 0,
                                .FlagImporta_Audit_Interviste = d.<imprese>.<datiimpresa>.<flagimporta_audit_interviste>.ToList.Count > 0,
                                .flagimporta_agenda = d.<imprese>.<datiimpresa>.<flagimporta_agenda>.ToList.Count > 0,
                                .nuovalogica_agenda = d.<imprese>.<datiimpresa>.<flagimporta_agenda>.<nuovalogica>.ToList.Count > 0,
                                .nuovalogica_riferimenti = d.<imprese>.<datiimpresa>.<flagimporta_agenda>.<nuovalogica_riferimenti>.ToList.Count > 0,
                                .flagimporta_pap = d.<imprese>.<datiimpresa>.<flagimporta_pap>.ToList.Count > 0,
                                .flagimporta_papz = d.<imprese>.<datiimpresa>.<flagimporta_papz>.ToList.Count > 0,
                                .flagimporta_notificabio = d.<imprese>.<datiimpresa>.<flagimporta_notificabio>.ToList.Count > 0,
                                .flagimporta_planning = d.<imprese>.<datiimpresa>.<flagimporta_planning>.ToList.Count > 0,
                                .flagimporta_distinta = d.<imprese>.<datiimpresa>.<flagimporta_distinta>.ToList.Count > 0,
                                .flagimporta_ricette = d.<imprese>.<datiimpresa>.<flagimporta_ricette>.ToList.Count > 0,
                                .Flagimporta_materieprime = d.<imprese>.<datiimpresa>.<flagimporta_materieprime>.ToList.Count > 0,
                                .Flagimporta_pianocolturale = d.<imprese>.<datiimpresa>.<flagimporta_pianocolturale>.ToList.Count > 0,
                                .nuovalogica_pianocolturale = d.<imprese>.<datiimpresa>.<flagimporta_pianocolturale>.<nuovalogica>.ToList.Count > 0,
                                .Flagimporta_gis = d.<imprese>.<datiimpresa>.<flagimporta_gis>.ToList.Count > 0,
                                .Flagimporta_profilazione = d.<imprese>.<datiimpresa>.<flagimporta_profilazione>.ToList.Count > 0,
                                .Flagimporta_catasto = d.<imprese>.<datiimpresa>.<flagimporta_catasto>.ToList.Count > 0,
                                .flagimporta_pua = d.<imprese>.<datiimpresa>.<flagimporta_pua>.ToList.Count > 0,
                                .flagimporta_pratiche = d.<imprese>.<datiimpresa>.<flagimporta_pratiche>.ToList.Count > 0,
                                .flagimporta_pratiche_pull = d.<imprese>.<datiimpresa>.<flagimporta_pratiche_pull>.ToList.Count > 0,
                                .Flagimporta_LineeProduttive = d.<imprese>.<datiimpresa>.<flagimporta_lineeproduttive>.ToList.Count > 0,
                                .Flagimporta_piani_di_campionamento = d.<imprese>.<datiimpresa>.<flagimporta_piani_di_campionamento>.ToList.Count > 0,
                                .Flagimporta_analisi = d.<imprese>.<datiimpresa>.<flagimporta_analisi>.ToList.Count > 0,
                                .flagimporta_allegati = d.<imprese>.<datiimpresa>.<flagimporta_allegati>.ToList.Count > 0,
                                .flagimporta_piano_concimazione = d.<imprese>.<datiimpresa>.<flagimporta_piano_concimazione>.ToList.Count > 0,
                                .ValiditaInizio_Audit = GetValiditaInizio(d.<imprese>.<datiimpresa>.<flagimporta_audit>.FirstOrDefault),
                                .ValiditaInizio_Audit_Interviste = GetValiditaInizio(d.<imprese>.<datiimpresa>.<flagimporta_audit_interviste>.FirstOrDefault),
                                .ValiditaInizio_agenda = GetValiditaInizio(d.<imprese>.<datiimpresa>.<flagimporta_agenda>.FirstOrDefault),
                                .ValiditaInizio_pap = GetValiditaInizio(d.<imprese>.<datiimpresa>.<flagimporta_pap>.FirstOrDefault),
                                .ValiditaInizio_papz = GetValiditaInizio(d.<imprese>.<datiimpresa>.<flagimporta_papz>.FirstOrDefault),
                                .ValiditaInizio_notificabio = GetValiditaInizio(d.<imprese>.<datiimpresa>.<flagimporta_notificabio>.FirstOrDefault),
                                .ValiditaInizio_planning = GetValiditaInizio(d.<imprese>.<datiimpresa>.<flagimporta_planning>.FirstOrDefault),
                                .ValiditaInizio_distinta = GetValiditaInizio(d.<imprese>.<datiimpresa>.<flagimporta_distinta>.FirstOrDefault),
                                .ValiditaInizio_ricette = GetValiditaInizio(d.<imprese>.<datiimpresa>.<flagimporta_ricette>.FirstOrDefault),
                                .ValiditaInizio_materieprime = GetValiditaInizio(d.<imprese>.<datiimpresa>.<flagimporta_materieprime>.FirstOrDefault),
                                .ValiditaInizio_pianocolturale = GetValiditaInizio(d.<imprese>.<datiimpresa>.<flagimporta_pianocolturale>.FirstOrDefault),
                                .ValiditaInizio_gis = GetValiditaInizio(d.<imprese>.<datiimpresa>.<flagimporta_gis>.FirstOrDefault),
                                .ValiditaInizio_profilazione = GetValiditaInizio(d.<imprese>.<datiimpresa>.<flagimporta_profilazione>.FirstOrDefault),
                                .ValiditaInizio_catasto = GetValiditaInizio(d.<imprese>.<datiimpresa>.<flagimporta_catasto>.FirstOrDefault),
                                .ValiditaInizio_pua = GetValiditaInizio(d.<imprese>.<datiimpresa>.<flagimporta_pua>.FirstOrDefault),
                                .ValiditaInizio_pratiche = GetValiditaInizio(d.<imprese>.<datiimpresa>.<flagimporta_pratiche>.FirstOrDefault),
                                .ValiditaInizio_pratiche_pull = GetValiditaInizio(d.<imprese>.<datiimpresa>.<flagimporta_pratiche_pull>.FirstOrDefault),
                                .ValiditaInizio_piano_concimazione = GetValiditaInizio(d.<imprese>.<datiimpresa>.<flagimporta_piano_concimazione>.FirstOrDefault),
                                .ValiditaInizio_LineeProduttive = GetValiditaInizio(d.<imprese>.<datiimpresa>.<flagimporta_lineeproduttive>.FirstOrDefault),
                                .ValiditaInizio_piani_di_campionamento = GetValiditaInizio(d.<imprese>.<datiimpresa>.<flagimporta_piani_di_campionamento>.FirstOrDefault),
                                .ValiditaInizio_analisi = GetValiditaInizio(d.<imprese>.<datiimpresa>.<flagimporta_analisi>.FirstOrDefault),
                                .ValiditaInizio_allegati = GetValiditaInizio(d.<imprese>.<datiimpresa>.<flagimporta_allegati>.FirstOrDefault),
                                .ValiditaFine_Audit = GetValiditafine(d.<imprese>.<datiimpresa>.<flagimporta_audit>.FirstOrDefault),
                                .ValiditaFine_Audit_Interviste = GetValiditafine(d.<imprese>.<datiimpresa>.<flagimporta_audit_interviste>.FirstOrDefault),
                                .ValiditaFine_agenda = GetValiditafine(d.<imprese>.<datiimpresa>.<flagimporta_agenda>.FirstOrDefault),
                                .ValiditaFine_pap = GetValiditafine(d.<imprese>.<datiimpresa>.<flagimporta_pap>.FirstOrDefault),
                                .ValiditaFine_papz = GetValiditafine(d.<imprese>.<datiimpresa>.<flagimporta_papz>.FirstOrDefault),
                                .ValiditaFine_notificabio = GetValiditafine(d.<imprese>.<datiimpresa>.<flagimporta_notificabio>.FirstOrDefault),
                                .ValiditaFine_planning = GetValiditafine(d.<imprese>.<datiimpresa>.<flagimporta_planning>.FirstOrDefault),
                                .ValiditaFine_distinta = GetValiditafine(d.<imprese>.<datiimpresa>.<flagimporta_distinta>.FirstOrDefault),
                                .ValiditaFine_ricette = GetValiditafine(d.<imprese>.<datiimpresa>.<flagimporta_ricette>.FirstOrDefault),
                                .ValiditaFine_materieprime = GetValiditafine(d.<imprese>.<datiimpresa>.<flagimporta_materieprime>.FirstOrDefault),
                                .ValiditaFine_pianocolturale = GetValiditafine(d.<imprese>.<datiimpresa>.<flagimporta_pianocolturale>.FirstOrDefault),
                                .ValiditaFine_gis = GetValiditafine(d.<imprese>.<datiimpresa>.<flagimporta_gis>.FirstOrDefault),
                                .ValiditaFine_profilazione = GetValiditafine(d.<imprese>.<datiimpresa>.<flagimporta_profilazione>.FirstOrDefault),
                                .ValiditaFine_catasto = GetValiditafine(d.<imprese>.<datiimpresa>.<flagimporta_catasto>.FirstOrDefault),
                                .ValiditaFine_pua = GetValiditafine(d.<imprese>.<datiimpresa>.<flagimporta_pua>.FirstOrDefault),
                                .ValiditaFine_pratiche = GetValiditafine(d.<imprese>.<datiimpresa>.<flagimporta_pratiche>.FirstOrDefault),
                                .ValiditaFine_pratiche_pull = GetValiditafine(d.<imprese>.<datiimpresa>.<flagimporta_pratiche_pull>.FirstOrDefault),
                                .ValiditaFine_piano_concimazione = GetValiditafine(d.<imprese>.<datiimpresa>.<flagimporta_piano_concimazione>.FirstOrDefault),
                                .ValiditaFine_LineeProduttive = GetValiditafine(d.<imprese>.<datiimpresa>.<flagimporta_lineeproduttive>.FirstOrDefault),
                                .ValiditaFine_piani_di_campionamento = GetValiditafine(d.<imprese>.<datiimpresa>.<flagimporta_piani_di_campionamento>.FirstOrDefault),
                                .ValiditaFine_analisi = GetValiditafine(d.<imprese>.<datiimpresa>.<flagimporta_analisi>.FirstOrDefault),
                                .ValiditaFine_Allegati = GetValiditafine(d.<imprese>.<datiimpresa>.<flagimporta_allegati>.FirstOrDefault),
                                .configurazione_Audit = GetConfigurazione(d.<imprese>.<datiimpresa>.<flagimporta_audit>.FirstOrDefault),
                                .configurazione_Audit_Interviste = GetConfigurazione(d.<imprese>.<datiimpresa>.<flagimporta_audit_interviste>.FirstOrDefault),
                                .configurazione_agenda = GetConfigurazione(d.<imprese>.<datiimpresa>.<flagimporta_agenda>.FirstOrDefault),
                                .configurazione_pap = GetConfigurazione(d.<imprese>.<datiimpresa>.<flagimporta_pap>.FirstOrDefault),
                                .configurazione_papz = GetConfigurazione(d.<imprese>.<datiimpresa>.<flagimporta_papz>.FirstOrDefault),
                                .configurazione_notificabio = GetConfigurazione(d.<imprese>.<datiimpresa>.<flagimporta_notificabio>.FirstOrDefault),
                                .configurazione_planning = GetConfigurazione(d.<imprese>.<datiimpresa>.<flagimporta_planning>.FirstOrDefault),
                                .configurazione_distinta = GetConfigurazione(d.<imprese>.<datiimpresa>.<flagimporta_distinta>.FirstOrDefault),
                                .configurazione_ricette = GetConfigurazione(d.<imprese>.<datiimpresa>.<flagimporta_ricette>.FirstOrDefault),
                                .configurazione_materieprime = GetConfigurazione(d.<imprese>.<datiimpresa>.<flagimporta_materieprime>.FirstOrDefault),
                                .configurazione_pianocolturale = GetConfigurazione(d.<imprese>.<datiimpresa>.<flagimporta_pianocolturale>.FirstOrDefault),
                                .configurazione_gis = GetConfigurazione(d.<imprese>.<datiimpresa>.<flagimporta_gis>.FirstOrDefault),
                                .configurazione_profilazione = GetConfigurazione(d.<imprese>.<datiimpresa>.<flagimporta_profilazione>.FirstOrDefault),
                                .configurazione_catasto = GetConfigurazione(d.<imprese>.<datiimpresa>.<flagimporta_catasto>.FirstOrDefault),
                                .configurazione_pua = GetConfigurazione(d.<imprese>.<datiimpresa>.<flagimporta_pua>.FirstOrDefault),
                                .configurazione_pratiche = GetConfigurazione(d.<imprese>.<datiimpresa>.<flagimporta_pratiche>.FirstOrDefault),
                                .configurazione_pratiche_pull = GetConfigurazione(d.<imprese>.<datiimpresa>.<flagimporta_pratiche_pull>.FirstOrDefault),
                                .configurazione_piano_concimazione = GetConfigurazione(d.<imprese>.<datiimpresa>.<flagimporta_piano_concimazione>.FirstOrDefault),
                                .configurazione_LineeProduttive = GetConfigurazione(d.<imprese>.<datiimpresa>.<flagimporta_lineeproduttive>.FirstOrDefault),
                                .configurazione_piani_di_campionamento = GetConfigurazione(d.<imprese>.<datiimpresa>.<flagimporta_piani_di_campionamento>.FirstOrDefault),
                                .configurazione_analisi = GetConfigurazione(d.<imprese>.<datiimpresa>.<flagimporta_analisi>.FirstOrDefault),
                                .configurazione_Allegati = GetConfigurazione(d.<imprese>.<datiimpresa>.<flagimporta_allegati>.FirstOrDefault)
                            },
                            .Imprese =
                                (From imps In d.<imprese>.<impresa>
                                 Select New clsImpresa With {
                               .Piva_ORIGINE = imps.<origine>.Value,
                               .Piva_DESTINAZIONE = imps.<destinazione>.Value,
                               .PivaPadre_DESTINAZIONE = imps.<padre>.Value,
                               .FlagImporta_Audit = imps.<flagimporta_audit>.ToList.Count > 0,
                               .FlagImporta_Audit_Interviste = imps.<flagimporta_audit_interviste>.ToList.Count > 0,
                               .flagimporta_agenda = imps.<flagimporta_agenda>.ToList.Count > 0,
                               .flagimporta_pap = imps.<flagimporta_pap>.ToList.Count > 0,
                               .flagimporta_papz = imps.<flagimporta_papz>.ToList.Count > 0,
                               .flagimporta_notificabio = imps.<flagimporta_notificabio>.ToList.Count > 0,
                               .flagimporta_planning = imps.<flagimporta_planning>.ToList.Count > 0,
                               .flagimporta_distinta = imps.<flagimporta_distinta>.ToList.Count > 0,
                               .flagimporta_ricette = imps.<flagimporta_ricette>.ToList.Count > 0,
                               .Flagimporta_materieprime = imps.<flagimporta_materieprime>.ToList.Count > 0,
                               .Flagimporta_pianocolturale = imps.<flagimporta_pianocolturale>.ToList.Count > 0,
                               .Flagimporta_gis = imps.<flagimporta_gis>.ToList.Count > 0,
                               .Flagimporta_profilazione = imps.<flagimporta_profilazione>.ToList.Count > 0,
                               .Flagimporta_catasto = imps.<flagimporta_catasto>.ToList.Count > 0,
                               .flagimporta_pua = imps.<flagimporta_pua>.ToList.Count > 0,
                               .flagimporta_pratiche = imps.<flagimporta_pratiche>.ToList.Count > 0,
                               .flagimporta_pratiche_pull = imps.<flagimporta_pratiche_pull>.ToList.Count > 0,
                               .FlagAccodaDatiSeEsistePivaDestinazione = imps.<flagAccodaDatiSeEsistePivaDestinazione>.ToList.Count > 0,
                               .Flagimporta_LineeProduttive = imps.<flagimporta_lineeproduttive>.ToList.Count > 0,
                               .Flagimporta_piani_di_campionamento = imps.<flagimporta_piani_di_campionamento>.ToList.Count > 0,
                               .Flagimporta_analisi = imps.<flagimporta_analisi>.ToList.Count > 0,
                               .flagimporta_allegati = imps.<flagimporta_allegati>.ToList.Count > 0,
                               .flagimporta_piano_concimazione = imps.<flagimporta_piano_concimazione>.ToList.Count > 0,
                               .ValiditaInizio_Audit = GetValiditaInizio(imps.<flagimporta_audit>.FirstOrDefault),
                               .ValiditaInizio_Audit_Interviste = GetValiditaInizio(imps.<flagimporta_audit_interviste>.FirstOrDefault),
                               .ValiditaInizio_agenda = GetValiditaInizio(imps.<flagimporta_agenda>.FirstOrDefault),
                               .ValiditaInizio_pap = GetValiditaInizio(imps.<flagimporta_pap>.FirstOrDefault),
                               .ValiditaInizio_papz = GetValiditaInizio(imps.<flagimporta_papz>.FirstOrDefault),
                               .ValiditaInizio_notificabio = GetValiditaInizio(imps.<flagimporta_notificabio>.FirstOrDefault),
                               .ValiditaInizio_planning = GetValiditaInizio(imps.<flagimporta_planning>.FirstOrDefault),
                               .ValiditaInizio_distinta = GetValiditaInizio(imps.<flagimporta_distinta>.FirstOrDefault),
                               .ValiditaInizio_ricette = GetValiditaInizio(imps.<flagimporta_ricette>.FirstOrDefault),
                               .ValiditaInizio_materieprime = GetValiditaInizio(imps.<flagimporta_materieprime>.FirstOrDefault),
                               .ValiditaInizio_pianocolturale = GetValiditaInizio(imps.<flagimporta_pianocolturale>.FirstOrDefault),
                               .ValiditaInizio_gis = GetValiditaInizio(imps.<flagimporta_gis>.FirstOrDefault),
                               .ValiditaInizio_profilazione = GetValiditaInizio(imps.<flagimporta_profilazione>.FirstOrDefault),
                               .ValiditaInizio_catasto = GetValiditaInizio(imps.<flagimporta_catasto>.FirstOrDefault),
                               .ValiditaInizio_pua = GetValiditaInizio(imps.<flagimporta_pua>.FirstOrDefault),
                               .ValiditaInizio_pratiche = GetValiditaInizio(imps.<flagimporta_pratiche>.FirstOrDefault),
                               .ValiditaInizio_pratiche_pull = GetValiditaInizio(imps.<flagimporta_pratiche_pull>.FirstOrDefault),
                               .ValiditaInizio_piano_concimazione = GetValiditaInizio(imps.<flagimporta_piano_concimazione>.FirstOrDefault),
                               .ValiditaInizio_LineeProduttive = GetValiditaInizio(imps.<flagimporta_lineeproduttive>.FirstOrDefault),
                               .ValiditaInizio_piani_di_campionamento = GetValiditaInizio(imps.<flagimporta_piani_di_campionamento>.FirstOrDefault),
                               .ValiditaInizio_analisi = GetValiditaInizio(imps.<flagimporta_analisi>.FirstOrDefault),
                               .ValiditaInizio_allegati = GetValiditaInizio(imps.<flagimporta_allegati>.FirstOrDefault),
                               .ValiditaFine_Audit = GetValiditafine(imps.<flagimporta_audit>.FirstOrDefault),
                               .ValiditaFine_Audit_Interviste = GetValiditafine(imps.<flagimporta_audit_interviste>.FirstOrDefault),
                               .ValiditaFine_agenda = GetValiditafine(imps.<flagimporta_agenda>.FirstOrDefault),
                               .ValiditaFine_pap = GetValiditafine(imps.<flagimporta_pap>.FirstOrDefault),
                               .ValiditaFine_papz = GetValiditafine(imps.<flagimporta_papz>.FirstOrDefault),
                               .ValiditaFine_notificabio = GetValiditafine(imps.<flagimporta_notificabio>.FirstOrDefault),
                               .ValiditaFine_planning = GetValiditafine(imps.<flagimporta_planning>.FirstOrDefault),
                               .ValiditaFine_distinta = GetValiditafine(imps.<flagimporta_distinta>.FirstOrDefault),
                               .ValiditaFine_ricette = GetValiditafine(imps.<flagimporta_ricette>.FirstOrDefault),
                               .ValiditaFine_materieprime = GetValiditafine(imps.<flagimporta_materieprime>.FirstOrDefault),
                               .ValiditaFine_pianocolturale = GetValiditafine(imps.<flagimporta_pianocolturale>.FirstOrDefault),
                               .ValiditaFine_gis = GetValiditafine(imps.<flagimporta_gis>.FirstOrDefault),
                               .ValiditaFine_profilazione = GetValiditafine(imps.<flagimporta_profilazione>.FirstOrDefault),
                               .ValiditaFine_catasto = GetValiditafine(imps.<flagimporta_catasto>.FirstOrDefault),
                               .ValiditaFine_pua = GetValiditafine(imps.<flagimporta_pua>.FirstOrDefault),
                               .ValiditaFine_pratiche = GetValiditafine(imps.<flagimporta_pratiche>.FirstOrDefault),
                               .ValiditaFine_pratiche_pull = GetValiditafine(imps.<flagimporta_pratiche_pull>.FirstOrDefault),
                               .ValiditaFine_piano_concimazione = GetValiditafine(imps.<flagimporta_piano_concimazione>.FirstOrDefault),
                               .ValiditaFine_LineeProduttive = GetValiditafine(imps.<flagimporta_lineeproduttive>.FirstOrDefault),
                               .ValiditaFine_piani_di_campionamento = GetValiditafine(imps.<flagimporta_piani_di_campionamento>.FirstOrDefault),
                               .ValiditaFine_analisi = GetValiditafine(imps.<flagimporta_analisi>.FirstOrDefault),
                               .ValiditaFine_Allegati = GetValiditafine(imps.<flagimporta_allegati>.FirstOrDefault),
                               .configurazione_Audit = GetConfigurazione(imps.<flagimporta_audit>.FirstOrDefault),
                               .configurazione_Audit_Interviste = GetConfigurazione(imps.<flagimporta_audit_interviste>.FirstOrDefault),
                               .configurazione_agenda = GetConfigurazione(imps.<flagimporta_agenda>.FirstOrDefault),
                               .configurazione_pap = GetConfigurazione(imps.<flagimporta_pap>.FirstOrDefault),
                               .configurazione_papz = GetConfigurazione(imps.<flagimporta_papz>.FirstOrDefault),
                               .configurazione_notificabio = GetConfigurazione(imps.<flagimporta_notificabio>.FirstOrDefault),
                               .configurazione_planning = GetConfigurazione(imps.<flagimporta_planning>.FirstOrDefault),
                               .configurazione_distinta = GetConfigurazione(imps.<flagimporta_distinta>.FirstOrDefault),
                               .configurazione_ricette = GetConfigurazione(imps.<flagimporta_ricette>.FirstOrDefault),
                               .configurazione_materieprime = GetConfigurazione(imps.<flagimporta_materieprime>.FirstOrDefault),
                               .configurazione_pianocolturale = GetConfigurazione(imps.<flagimporta_pianocolturale>.FirstOrDefault),
                               .configurazione_gis = GetConfigurazione(imps.<flagimporta_gis>.FirstOrDefault),
                               .configurazione_profilazione = GetConfigurazione(imps.<flagimporta_profilazione>.FirstOrDefault),
                               .configurazione_catasto = GetConfigurazione(imps.<flagimporta_catasto>.FirstOrDefault),
                               .configurazione_pua = GetConfigurazione(imps.<flagimporta_pua>.FirstOrDefault),
                               .configurazione_pratiche = GetConfigurazione(imps.<flagimporta_pratiche>.FirstOrDefault),
                               .configurazione_pratiche_pull = GetConfigurazione(imps.<flagimporta_pratiche_pull>.FirstOrDefault),
                               .configurazione_piano_concimazione = GetConfigurazione(imps.<flagimporta_piano_concimazione>.FirstOrDefault),
                               .configurazione_LineeProduttive = GetConfigurazione(imps.<flagimporta_lineeproduttive>.FirstOrDefault),
                               .configurazione_piani_di_campionamento = GetConfigurazione(imps.<flagimporta_piani_di_campionamento>.FirstOrDefault),
                               .configurazione_analisi = GetConfigurazione(imps.<flagimporta_analisi>.FirstOrDefault),
                               .configurazione_Allegati = GetConfigurazione(imps.<flagimporta_allegati>.FirstOrDefault),
                               .Impianti = (From ii In imps.<impianti>.<impianto>
                                            Select New Impianto_Colturale With {
                       .Piva = ii.@piva,
                       .Sa_Cod = ii.@sa_cod,
                       .Appezza = ii.@appezza,
                       .ID_Reg = ii.@id_reg
                }).ToList()
                        }).ToList
                        }).FirstOrDefault

        End Select

    End Function

    Private Shared Sub EstraiListaImpiantiEdAziendeDaFiltroRicerca(objOpzioni As Gias2Gias_LIB.clsOpzioni, listOfDataImport As clsImportData, ByRef OUTPUT_dt_x_export_azienda As DataTable, ByRef OUTPUT_dt_x_export_impianto As DataTable)
        Dim xFiltrone As New Filtrone

        Dim JsonRichiesta2 As filtrone_richiesta_nuovo =
            JsonConvert.DeserializeObject(Of filtrone_richiesta_nuovo)(listOfDataImport.filtrone)

        Dim Qs_Funzione = enum_TipoFiltrone.Agenda

        'imposto leggendo da session
        Dim ASG_SuperUser_CodFiscale As String = objOpzioni.SuperUser_CodFiscale_ORIGINE 'HttpContext.Current.Session("ASG_SuperUser_CodFiscale")
        Dim ASG_Utente_Username_Crypt As String = objOpzioni.SuperUser_Username_ORIGINE 'HttpContext.Current.Session("ASG_Utente_Username_Crypt")
        Dim ASG_Utente_Password_Crypt As String = "" 'HttpContext.Current.Session("ASG_Utente_Password_Crypt")

        Dim Param_Sql_Permessi As String = "" ' HttpContext.Current.Session("Sql_Permessi")
        Dim Param_Xml_Permessi As String = "" 'HttpContext.Current.Session("Xml_Permessi")
        Dim Sql_Permessi_Amministratore As String = "" ' HttpContext.Current.Session("Sql_Permessi_Amministratore")
        Dim DgrVuoti As String = "false"
        Dim objparametri_Server As AgronicaCoreParametri = objOpzioni.objParametri_Server_GIAS_ORIGINE ' HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objparametri_Utenti As AgronicaCoreParametri = objOpzioni.objParametri_Utenti_GIAS_ORIGINE ' HttpContext.Current.Session("ASG_objParametri_Utenti")

        Dim OUTPUT_strFiltro As String = ""
        Dim idleOUTPUT_dt_x_export_centro As DataTable = Nothing
        Dim idleOUTPUT_dt_x_export_appezza As DataTable = Nothing
        Dim idleOUTPUT_dt_x_export_distinta As DataTable = Nothing
        Dim idleOUTPUT_dt_x_export_movimento As DataTable = Nothing
        Dim idleOUTPUT_dt_x_export_fabbricato As DataTable = Nothing 'Anna 17/08/21:Aggiunta scheda fabbricati nel filtrone, per TTI
        Dim OUTPUT_Xml_permessi As String = ""
        Dim OUTPUT_Sql_Permessi As String = ""
        Dim OUTPUT_Xml_Filtro As String = ""
        Dim OUTPUT_Join As JoinFiltrone = Nothing


        Dim rval As String = xFiltrone.CreaFiltroDati(
             Qs_Funzione,
             JsonRichiesta2,
             ASG_SuperUser_CodFiscale,
             ASG_Utente_Username_Crypt,
             ASG_Utente_Password_Crypt,
             Param_Sql_Permessi,
             Param_Xml_Permessi,
             Sql_Permessi_Amministratore,
             DgrVuoti,
             objparametri_Server,
             objparametri_Utenti,
             OUTPUT_strFiltro,
             OUTPUT_dt_x_export_azienda,
             idleOUTPUT_dt_x_export_centro,
             idleOUTPUT_dt_x_export_appezza,
             OUTPUT_dt_x_export_impianto,
             idleOUTPUT_dt_x_export_distinta,
             idleOUTPUT_dt_x_export_movimento,
             idleOUTPUT_dt_x_export_fabbricato, 'Anna 17/08/21:Aggiunta scheda fabbricati nel filtrone, per TTI
             OUTPUT_Xml_permessi,
             OUTPUT_Sql_Permessi,
             OUTPUT_Xml_Filtro,
             OUTPUT_Join
            )
        ' fine della versione che legge rispetto ai filtri impostati...
    End Sub

    Private Shared Sub AccodaAziendeInXmlDataListaImpianti(listOfDataImport As clsImportData, OUTPUT_dt_x_export_impianto As DataTable, di As clsDatiImpresa)
        Dim impresaPrecedente As String = ""


        For Each dRowImpianti As DataRow In OUTPUT_dt_x_export_impianto.Rows

            Dim impresaCorrente As String = dRowImpianti("piva")

            Dim impresaDaImportare As clsImpresa = Nothing

            'una volta sola per impresa
            If impresaPrecedente <> impresaCorrente Then
                impresaDaImportare = (From ii In listOfDataImport.Imprese
                                      Where ii.Piva_ORIGINE = impresaCorrente
                                ).FirstOrDefault

                'se e solo se non esiste allora aggiungo alla lista (una volta sola, come definito da impresaPrecedente <> impresaCorrente)
                If impresaDaImportare Is Nothing Then

                    impresaDaImportare = New clsImpresa
                    RiportaConfigurazioneGeneraleSuImpresaCorrente(di, impresaCorrente, impresaDaImportare)

                    listOfDataImport.Imprese.Add(impresaDaImportare)

                End If




            End If
            'test impresaPrecedente <> impresaCorrente

            impresaPrecedente = impresaCorrente

            'impianto da aggiungere ... chiaramente solo se valorizzato
            Dim appezzaPerTest As Integer = dRowImpianti("appezza")

            ' VAnni: 28/6/2019: la lista degli impianti può contenere anche un record con appezza = 0 (serve per accodare le sole aziende, vedi codice sopra)
            If appezzaPerTest <> 0 Then


                Dim impiantoDaAggiungereInLista As Impianto_Colturale = (
                    From iL In impresaDaImportare.Impianti
                    Where iL.Piva = dRowImpianti("piva") _
                    And iL.Sa_Cod = dRowImpianti("sa_Cod") _
                    And iL.Appezza = appezzaPerTest _
                    And iL.ID_Reg = dRowImpianti("id_reg")
                ).FirstOrDefault

                If impiantoDaAggiungereInLista Is Nothing Then


                    impiantoDaAggiungereInLista =
                                    New Impianto_Colturale(
                                        Piva_In:=dRowImpianti("piva"),
                                        Sa_Cod_In:=dRowImpianti("sa_cod"),
                                        Appezza_In:=dRowImpianti("appezza"),
                                        ID_Reg_In:=dRowImpianti("id_reg"),
                                        Progetto_Cod_In:=0,
                                        Validita_Inizio_In:=AGRODATAINIZIO,
                                        Validita_fine_In:=AGRODATAFINE,
                                        Qta2_In:=0
                                    )

                    impresaDaImportare.Impianti.Add(impiantoDaAggiungereInLista)


                End If
            End If
            'fine test appezza

            'fine impianto da aggiungere ... 

        Next
        'impianto
    End Sub

    Private Shared Sub AccodaAziendeInXmlDataListaFabbricati(listOfDataImport As clsImportData, OUTPUT_dt_x_export_fabbricato As DataTable, di As clsDatiImpresa)
        Dim impresaPrecedente As String = ""

        For Each drRow As DataRow In OUTPUT_dt_x_export_fabbricato.Rows

            Dim impresaCorrente As String = drRow("piva")

            Dim impresaDaImportare As clsImpresa = Nothing

            impresaDaImportare = (From ii In listOfDataImport.Imprese
                                  Where ii.Piva_ORIGINE = impresaCorrente
                                ).FirstOrDefault

            'se e solo se non esiste allora aggiungo alla lista (una volta sola, come definito da impresaPrecedente <> impresaCorrente)
            If impresaDaImportare Is Nothing Then

                impresaDaImportare = New clsImpresa
                RiportaConfigurazioneGeneraleSuImpresaCorrente(di, impresaCorrente, impresaDaImportare)

                listOfDataImport.Imprese.Add(impresaDaImportare)

            End If

            Dim fabbricatoDaAggiungereInLista As AgronicaCoreModello.Anagrafe.Magazzino = (
                    From iL In impresaDaImportare.Fabbricati
                    Where iL.Piva = drRow("piva") _
                    And iL.Sa_Cod = drRow("sa_Cod") _
                    And iL.Fabbricato_Cod = drRow("fabbricato_cod")
                ).FirstOrDefault

            If fabbricatoDaAggiungereInLista Is Nothing Then

                fabbricatoDaAggiungereInLista =
                                    New AgronicaCoreModello.Anagrafe.Magazzino(
                                        Piva_In:=drRow("piva"),
                                        Sa_Cod_In:=drRow("sa_cod"),
                                        Fabbricato_Cod_In:=drRow("fabbricato_cod")
                                    )

                impresaDaImportare.Fabbricati.Add(fabbricatoDaAggiungereInLista)

            End If

        Next

    End Sub

    Private Shared Sub RiportaConfigurazioneGeneraleSuImpresaCorrente(di As clsDatiImpresa, impresaCorrente As String, ByRef i As clsImpresa)
        With i
            .configurazione_agenda = di.configurazione_agenda
            .configurazione_pratiche = di.configurazione_pratiche
            .configurazione_pratiche_pull = di.configurazione_pratiche_pull
            .configurazione_Allegati = di.configurazione_Allegati
            .configurazione_analisi = di.configurazione_analisi
            .configurazione_piano_concimazione = di.configurazione_piano_concimazione
            .configurazione_Audit_Interviste = di.configurazione_Audit_Interviste
            .configurazione_pap = di.configurazione_pap
            .configurazione_papz = di.configurazione_papz
            .configurazione_notificabio = di.configurazione_notificabio
            .configurazione_planning = di.configurazione_planning
            .configurazione_distinta = di.configurazione_distinta
            .configurazione_ricette = di.configurazione_ricette
            .configurazione_materieprime = di.configurazione_materieprime
            .configurazione_pianocolturale = di.configurazione_pianocolturale
            .configurazione_gis = di.configurazione_gis
            .configurazione_profilazione = di.configurazione_profilazione
            .configurazione_catasto = di.configurazione_catasto
            .configurazione_pua = di.configurazione_pua
            .configurazione_LineeProduttive = di.configurazione_LineeProduttive
            .configurazione_piani_di_campionamento = di.configurazione_piani_di_campionamento

            .FlagImporta_Audit_Interviste = di.FlagImporta_Audit_Interviste
            .FlagImporta_Audit = di.FlagImporta_Audit
            .Flagimporta_catasto = di.Flagimporta_catasto
            .Flagimporta_gis = di.Flagimporta_gis
            .Flagimporta_pianocolturale = di.Flagimporta_pianocolturale
            .Flagimporta_materieprime = di.Flagimporta_materieprime
            .flagimporta_agenda = di.flagimporta_agenda
            .flagimporta_pap = di.flagimporta_pap
            .flagimporta_papz = di.flagimporta_papz
            .flagimporta_notificabio = di.flagimporta_notificabio
            .Flagimporta_profilazione = di.Flagimporta_profilazione
            .flagimporta_pratiche = di.flagimporta_pratiche
            .flagimporta_pratiche_pull = di.flagimporta_pratiche_pull
            .flagimporta_planning = di.flagimporta_planning
            .flagimporta_distinta = di.flagimporta_distinta
            .flagimporta_ricette = di.flagimporta_ricette
            .flagimporta_pua = di.flagimporta_pua
            .Flagimporta_LineeProduttive = di.Flagimporta_LineeProduttive
            .Flagimporta_piani_di_campionamento = di.Flagimporta_piani_di_campionamento
            .Flagimporta_analisi = di.Flagimporta_analisi
            .flagimporta_allegati = di.flagimporta_allegati
            .flagimporta_piano_concimazione = di.flagimporta_piano_concimazione
            .nuovalogica_pianocolturale = di.nuovalogica_pianocolturale
            .nuovalogica_agenda = di.nuovalogica_agenda
            .nuovalogica_riferimenti = di.nuovalogica_riferimenti
            .ValiditaInizio_Audit_Interviste = di.ValiditaInizio_Audit_Interviste
            .ValiditaInizio_Audit = di.ValiditaInizio_Audit
            .ValiditaInizio_catasto = di.ValiditaInizio_catasto
            .ValiditaInizio_gis = di.ValiditaInizio_gis
            .ValiditaInizio_pianocolturale = di.ValiditaInizio_pianocolturale
            .ValiditaInizio_materieprime = di.ValiditaInizio_materieprime
            .ValiditaInizio_agenda = di.ValiditaInizio_agenda
            .ValiditaInizio_pap = di.ValiditaInizio_pap
            .ValiditaInizio_papz = di.ValiditaInizio_papz
            .ValiditaInizio_notificabio = di.ValiditaInizio_notificabio
            .ValiditaInizio_profilazione = di.ValiditaInizio_profilazione
            .ValiditaInizio_planning = di.ValiditaInizio_planning
            .ValiditaInizio_distinta = di.ValiditaInizio_distinta
            .ValiditaInizio_ricette = di.ValiditaInizio_ricette
            .ValiditaInizio_pua = di.ValiditaInizio_pua
            .ValiditaInizio_LineeProduttive = di.ValiditaInizio_LineeProduttive
            .ValiditaInizio_piani_di_campionamento = di.ValiditaInizio_piani_di_campionamento
            .ValiditaInizio_allegati = di.ValiditaInizio_allegati
            .ValiditaInizio_analisi = di.ValiditaInizio_analisi
            .ValiditaInizio_piano_concimazione = di.ValiditaInizio_piano_concimazione
            .ValiditaInizio_pratiche = di.ValiditaInizio_pratiche
            .ValiditaInizio_pratiche_pull = di.ValiditaInizio_pratiche_pull

            .ValiditaFine_analisi = di.ValiditaFine_analisi
            .ValiditaFine_Audit_Interviste = di.ValiditaFine_Audit_Interviste
            .ValiditaFine_Audit = di.ValiditaFine_Audit
            .ValiditaFine_catasto = di.ValiditaFine_catasto
            .ValiditaFine_gis = di.ValiditaFine_gis
            .ValiditaFine_pianocolturale = di.ValiditaFine_pianocolturale
            .ValiditaFine_materieprime = di.ValiditaFine_materieprime
            .ValiditaFine_agenda = di.ValiditaFine_agenda
            .ValiditaFine_pap = di.ValiditaFine_pap
            .ValiditaFine_papz = di.ValiditaFine_papz
            .ValiditaFine_notificabio = di.ValiditaFine_notificabio
            .ValiditaFine_profilazione = di.ValiditaFine_profilazione
            .ValiditaFine_planning = di.ValiditaFine_planning
            .ValiditaFine_distinta = di.ValiditaFine_distinta
            .ValiditaFine_ricette = di.ValiditaFine_ricette
            .ValiditaFine_pua = di.ValiditaFine_pua
            .ValiditaFine_LineeProduttive = di.ValiditaFine_LineeProduttive
            .ValiditaFine_piani_di_campionamento = di.ValiditaFine_piani_di_campionamento
            .ValiditaFine_analisi = di.ValiditaFine_analisi
            .ValiditaFine_Allegati = di.ValiditaFine_Allegati
            .ValiditaFine_pratiche = di.ValiditaFine_pratiche
            .ValiditaFine_pratiche_pull = di.ValiditaFine_pratiche_pull

            .FlagAccodaDatiSeEsistePivaDestinazione = False
            .Piva_ORIGINE = impresaCorrente
            .Piva_DESTINAZIONE = impresaCorrente
            .PivaPadre_DESTINAZIONE = ""
        End With

    End Sub

    Public Sub GIAS_2_GIAS_Completaopzioni(ByRef objOpzioni As Gias2Gias_LIB.clsOpzioni, ByVal FromServizio As Boolean, Optional Log_G2G As StringBuilder = Nothing)

        Dim Stringa_Connessione_Server_GIAS_Origine As String
        Dim Stringa_Connessione_Utenti_GIAS_Origine As String
        Dim Stringa_Connessione_Server_GIAS_Destinazione As String
        Dim Stringa_Connessione_Utenti_GIAS_Destinazione As String
        Dim objAgronicaCore As New AgronicaCoreDataProvider.DataProvider
        Dim objParametri_Server_GIAS_Origine As AgronicaCoreParametri
        Dim objParametri_Utenti_GIAS_Origine As AgronicaCoreParametri
        Dim objParametri_Server_GIAS_Destinazione As AgronicaCoreParametri
        Dim objParametri_Utenti_GIAS_Destinazione As AgronicaCoreParametri


        If objOpzioni.isGias2Gias_local Then
            Stringa_Connessione_Server_GIAS_Destinazione = objAgronicaCore.FindConnessione_Su_Ini_O_Superserver(objOpzioni.PercorsoConnessioni, objOpzioni.Connessione_Server_GIAS_Destinazione, ObjParametri_SuperServer)
            Stringa_Connessione_Utenti_GIAS_Destinazione = objAgronicaCore.FindConnessione_Su_Ini_O_Superserver(objOpzioni.PercorsoConnessioni, objOpzioni.Connessione_Utenti_GIAS_Destinazione, ObjParametri_SuperServer)

        Else
            Dim ws As New WS_Importa_GIAS_2014.ImportaWS
            ws.Url = objOpzioni.wsimportaGiasURl
            ws.Timeout = Integer.MaxValue

            Stringa_Connessione_Server_GIAS_Destinazione = ws.Get_StringaConnessioneDaSuperServer(objOpzioni.Connessione_Server_GIAS_Destinazione)
            Stringa_Connessione_Utenti_GIAS_Destinazione = ws.Get_StringaConnessioneDaSuperServer(objOpzioni.Connessione_Utenti_GIAS_Destinazione)

        End If



        If Not FromServizio Then

            Stringa_Connessione_Server_GIAS_Origine = objAgronicaCore.FindConnessione_Su_Ini_O_Superserver(objOpzioni.PercorsoConnessioni, objOpzioni.Connessione_Server_GIAS_Origine)
            Stringa_Connessione_Utenti_GIAS_Origine = objAgronicaCore.FindConnessione_Su_Ini_O_Superserver(objOpzioni.PercorsoConnessioni, objOpzioni.Connessione_Utenti_GIAS_Origine)

            objParametri_Server_GIAS_Origine = GetObjParametri(objOpzioni, Stringa_Connessione_Server_GIAS_Origine)
            objParametri_Utenti_GIAS_Origine = New AgronicaCoreDataProvider.AgronicaCoreParametri(AGRODATAINIZIO,
                                                                                                    AGRODATAFINE,
                                                                                                    AgronicaCoreDataProvider.AgronicaCoreParametri.enumCancellazioneLogica.CancellazioneFisica,
                                                                                                    AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati,
                                                                                                    objOpzioni.PathDirFileLog,
                                                                                                    "Transfert_GiacenzeDDT_daGias_aGias.txt",
                                                                                                    objOpzioni.SuperUser_Username_ORIGINE,
                                                                                                    objOpzioni.SuperUser_CodFiscale_ORIGINE,
                                                                                                    objOpzioni.Import_Username_ORIGINE,
                                                                                                    objOpzioni.Import_CodFiscale_ORIGINE,
                                                                                                    Stringa_Connessione_Utenti_GIAS_Origine)
        Else

            objParametri_Server_GIAS_Origine = ObjParametri_Server
            objParametri_Utenti_GIAS_Origine = ObjParametri_Utenti
        End If


        objParametri_Server_GIAS_Destinazione = New AgronicaCoreDataProvider.AgronicaCoreParametri(AGRODATAINIZIO,
                                                                                                AGRODATAFINE,
                                                                                                AgronicaCoreDataProvider.AgronicaCoreParametri.enumCancellazioneLogica.CancellazioneFisica,
                                                                                                AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati,
                                                                                                objOpzioni.PathDirFileLog,
                                                                                                "Transfert_GiacenzeDDT_daGias_aGias.txt",
                                                                                                objOpzioni.SuperUser_Username_DESTINAZIONE,
                                                                                                objOpzioni.SuperUser_CodFiscale_DESTINAZIONE,
                                                                                                objOpzioni.Import_Username_DESTINAZIONE,
                                                                                                objOpzioni.Import_CodFiscale_DESTINAZIONE,
                                                                                                Stringa_Connessione_Server_GIAS_Destinazione)

        objParametri_Utenti_GIAS_Destinazione = New AgronicaCoreDataProvider.AgronicaCoreParametri(AGRODATAINIZIO,
                                                                                                AGRODATAFINE,
                                                                                                AgronicaCoreDataProvider.AgronicaCoreParametri.enumCancellazioneLogica.CancellazioneFisica,
                                                                                                AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati,
                                                                                                objOpzioni.PathDirFileLog,
                                                                                                "Transfert_GiacenzeDDT_daGias_aGias.txt",
                                                                                                objOpzioni.SuperUser_Username_DESTINAZIONE,
                                                                                                objOpzioni.SuperUser_CodFiscale_DESTINAZIONE,
                                                                                                objOpzioni.Import_Username_DESTINAZIONE,
                                                                                                objOpzioni.Import_CodFiscale_DESTINAZIONE,
                                                                                                Stringa_Connessione_Utenti_GIAS_Destinazione)


        objOpzioni.objParametri_Server_GIAS_ORIGINE = objParametri_Server_GIAS_Origine
        objOpzioni.objParametri_Server_GIAS_DESTINAZIONE = objParametri_Server_GIAS_Destinazione
        objOpzioni.objParametri_Utenti_GIAS_ORIGINE = objParametri_Utenti_GIAS_Origine
        objOpzioni.objParametri_Utenti_GIAS_DESTINAZIONE = objParametri_Utenti_GIAS_Destinazione

        objOpzioni.objParametri_Server_GIAS_ORIGINE.FlagVisibilita = AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
        objOpzioni.objParametri_Utenti_GIAS_ORIGINE.FlagVisibilita = AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti

        AgronicaCoreDataProvider.UtilityProvider.Calcola_BaseCode_TopCode(objOpzioni.BaseCode_ORIGINE,
                                                                              objOpzioni.TopCode_ORIGINE,
                                                                              objOpzioni.ProgressivoGIAS_ORIGINE)

        AgronicaCoreDataProvider.UtilityProvider.Calcola_BaseCode_TopCode(objOpzioni.BaseCode_DESTINAZIONE,
                                                                            objOpzioni.TopCode_DESTINAZIONE,
                                                                            objOpzioni.ProgressivoGIAS_DESTINAZIONE)


        ''=============================================================
        ''======= VERIFICA PERMESSI SULL'UTENTE CHE IMPORTA ===========
        ''=============================================================

        'Log_G2G.Append(CStr(Date.Now) + " - " + "Verifica permessi utente." + vbCrLf + vbCrLf)

        'Dim objUtenti As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
        'Dim UtenteAbilitato As Boolean

        'UtenteAbilitato = objUtenti.Controlla_Permessi_Utente(objOpzioni.Import_Username, _
        '                                    objOpzioni.Id_Servizio, _
        '                                    enum_Security_Attivita.ManutenzioneArchivi_Import_DDTFatture_Seled, _
        '                                    enum_Security_Operazione.Modifica, _
        '                                    Date.Today, _
        '                                    "", _
        '                                    objParametri_Utenti)

        'If UtenteAbilitato = False Then
        '    Log_G2G.Append(CStr(Date.Now) + " - " + "L'utente non dispone dei permessi per avviare l'importazione da Seled. " + vbCrLf)
        '    Log_Errori.Append(CStr(Date.Now) + " - " + "L'utente non dispone dei permessi per avviare l'importazione da Seled. " + vbCrLf)
        'Else
    End Sub

    Private Shared Function GetObjParametri(
        ByVal SuperUser_Username_ORIGINE As String,
        ByVal SuperUser_CodFiscale_ORIGINE As String,
        ByVal Import_Username_ORIGINE As String,
        ByVal Import_CodFiscale_ORIGINE As String,
        ByVal PathDirFileLog As String,
        ByVal Stringa_Connessione_Server_GIAS As String
    ) As AgronicaCoreDataProvider.AgronicaCoreParametri

        Dim objParametri_Server As AgronicaCoreParametri
        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(
            AGRODATAINIZIO,
            AGRODATAFINE,
            AgronicaCoreDataProvider.AgronicaCoreParametri.enumCancellazioneLogica.CancellazioneFisica,
            AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati,
            PathDirFileLog,
            "Transfert_GiacenzeDDT_daGias_aGias.txt",
            SuperUser_Username_ORIGINE,
            SuperUser_CodFiscale_ORIGINE,
            Import_Username_ORIGINE,
            Import_CodFiscale_ORIGINE,
            Stringa_Connessione_Server_GIAS)

        Return objParametri_Server
    End Function

    Private Shared Function GetObjParametri(
        ByRef objOpzioni As Gias2Gias_LIB.clsOpzioni,
        ByVal Stringa_Connessione_Server_GIAS As String
    ) As AgronicaCoreDataProvider.AgronicaCoreParametri

        Dim objParametri_Server As AgronicaCoreParametri
        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(
            AGRODATAINIZIO,
            AGRODATAFINE,
            AgronicaCoreDataProvider.AgronicaCoreParametri.enumCancellazioneLogica.CancellazioneFisica,
            AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati,
            objOpzioni.PathDirFileLog,
            "Transfert_GiacenzeDDT_daGias_aGias.txt",
            objOpzioni.SuperUser_Username_ORIGINE,
            objOpzioni.SuperUser_CodFiscale_ORIGINE,
            objOpzioni.Import_Username_ORIGINE,
            objOpzioni.Import_CodFiscale_ORIGINE,
            Stringa_Connessione_Server_GIAS)

        Return objParametri_Server
    End Function


    Public Sub New()

    End Sub


    Private ObjParametri_SuperServer As AgronicaCoreDataProvider.AgronicaCoreParametri
    Private ObjParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri
    Private ObjParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri
    'Private Configurazione_Servizio As AgronicaCoreVarieDAL.Configurazione_Servizio

    Public Sub New(
           ByVal _Configurazione_Servizio As AgronicaCoreVarieDAL.Configurazione_Servizio,
           ByVal _ObjParametri_SuperServer As AgronicaCoreDataProvider.AgronicaCoreParametri,
           ByVal _ObjParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
           ByVal _ObjParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri
    )

        'Configurazione_Servizio = _Configurazione_Servizio 'leggo solo ciò che mi serve
        ObjParametri_SuperServer = _ObjParametri_SuperServer
        ObjParametri_Server = _ObjParametri_Server
        ObjParametri_Utenti = _ObjParametri_Utenti

        RicavaParametri(_Configurazione_Servizio)

    End Sub


    Public Sub New(
           ByVal _ObjParametri_SuperServer As AgronicaCoreDataProvider.AgronicaCoreParametri,
           ByVal _ObjParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
           ByVal _ObjParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri
    )

        'Configurazione_Servizio = _Configurazione_Servizio 'leggo solo ciò che mi serve
        ObjParametri_SuperServer = _ObjParametri_SuperServer
        ObjParametri_Server = _ObjParametri_Server
        ObjParametri_Utenti = _ObjParametri_Utenti

    End Sub


    Public Sub New(
           ByVal _ObjParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
           ByVal _ObjParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
           ByVal ID_Cfg As Integer
    )

        'Configurazione_Servizio = _Configurazione_Servizio 'leggo solo ciò che mi serve
        'ObjParametri_SuperServer = _ObjParametri_SuperServer
        ObjParametri_Server = _ObjParametri_Server
        ObjParametri_Utenti = _ObjParametri_Utenti

        Me.ID_Cfg = ID_Cfg


    End Sub

    Private Sub RicavaParametri(ByVal _Configurazione_Servizio As AgronicaCoreVarieDAL.Configurazione_Servizio)

        '--------------------------------------------PARAMETRI----------------------------------------------
        cartellaLog = _Configurazione_Servizio.DirectoryLOG
        If cartellaLog Is Nothing OrElse cartellaLog = "" Then
            cartellaLog = "C:\GIASLan\LOG\G2G"
        End If
        If Not Directory.Exists(cartellaLog) Then
            Directory.CreateDirectory(cartellaLog)
        End If

        '-----------------------------------------PARAMETRI EXTRA-------------------------------------------
        Dim Parametri_Extra As String() = _Configurazione_Servizio.Parametri_Extra.Split("|")
        Dim Parametri_Extra_HT As New Hashtable
        Dim keyVal As String()
        For Each parametro As String In Parametri_Extra
            keyVal = parametro.Split("=")
            If keyVal.Count = 2 Then
                Parametri_Extra_HT.Add(keyVal(0).Trim, keyVal(1).Trim)
            Else
                Throw New Exception(String.Format("Errore nella lettura dei parametri extra: parametro.Split({0}).Count <> 2: {1}",
                                                  "=", parametro))
            End If
        Next

        Try
            If IsNothing(Parametri_Extra_HT("ID_Cfg")) Then
                Throw New Exception("manca il parametro obbligatorio ID_Cfg")
            End If

            ID_Cfg = CInt(Parametri_Extra_HT("ID_Cfg"))

        Catch ex As Exception
            Throw New Exception("Errore nella lettura dei parametri extra in configurazione_siti.parametri_extra:  " & ex.Message)
        End Try

    End Sub


End Class
