Imports System.IO
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreUtility
Imports AgronicaCoreXMLUniversale

Public Class InterscambioLog

    Private _objLog As LogProvider
    Private _logFileName As String
    Private _logDirectory As String
    Private _logDescrizioneUtente As String
    Private _objParametriServer As AgronicaCoreParametri
    Private _customLOGParams As CustomLOGParams

    Const MAX_DIM_FILE_LOG As Long = 5000000     '5.000.000   'in byte (= 5 MB)

    Public Enum enum_LogMessageType
        ''' <summary>
        ''' Log
        ''' </summary>
        L = 1
        ''' <summary>
        ''' Errore
        ''' </summary>
        E = 2
    End Enum


    Public Enum enum_InterfacceAboca
        ''' <summary>
        ''' Anagrafica Materiali
        ''' </summary>
        INT50 = 1
        ''' <summary>
        ''' Anagrafica Fornitori (sempre questo, generico Contatti)
        ''' </summary>
        INT53 = 2
        ''' <summary>
        ''' Anagrafica Dipendenti
        ''' </summary>
        INT54 = 3
        ''' <summary>
        ''' Movimenti di Ricevimento Merci
        ''' </summary>
        INT55 = 4
        ''' <summary>
        ''' Creazione Ordine Produzione
        ''' </summary>
        INT56 = 5
        ''' <summary>
        ''' Consumi materiali/risorse
        ''' </summary>
        INT58 = 6
        ''' <summary>
        ''' Versamento prodotto da Ordine di Produzione
        ''' </summary>
        INT59 = 7
        ''' <summary>
        ''' Versamento prodotto da Ordine di Produzione macellazione
        ''' </summary>
        INT60 = 8
        ''' <summary>
        ''' Chiusura ordine produzione/ordine interno
        ''' </summary>
        INT61 = 9
        ''' <summary>
        ''' Ore lavorate macchina/uomo su OdP
        ''' </summary>
        INT65 = 10
    End Enum

#Region "Costruttori"

    Public Sub New()

    End Sub

    Public Sub New(logFileName As String, logDirectory As String, logDescrizioneUtente As String, objParametriServer As AgronicaCoreParametri)
        _objLog = New LogProvider
        _logFileName = logFileName
        _logDirectory = logDirectory
        _logDescrizioneUtente = logDescrizioneUtente
        _objParametriServer = objParametriServer

        _customLOGParams = New CustomLOGParams With {
            .LogDescrizioneUtente = logDescrizioneUtente,
            .LogDirectory = logDirectory,
            .LogFileName = logFileName
        }


    End Sub

#End Region

    Public Sub Logga(ByVal msg As String, Optional verificaInviaElasticSearch As Boolean = True, Optional ByVal cuaaLog As String = "")

        Dim fileCompleto As String = Path.Combine(_logDirectory, _logFileName)
        Dim fileInfoLog = My.Computer.FileSystem.GetFileInfo(fileCompleto)

        If File.Exists(fileCompleto) AndAlso fileInfoLog.Length > MAX_DIM_FILE_LOG Then

            Dim oldDir As String = Path.Combine(_logDirectory, "OLD\")
            Dim newName As String = _logFileName.Substring(0, _logFileName.Length - 4) & "_" & Now.ToString("yyyyMMdd-HHmmss-fff") & ".txt"

            FileIO.FileSystem.MoveFile(fileCompleto, oldDir & newName, True)
        End If

        _objLog.Scrivi_LOG(_objParametriServer, "", msg, verificaInviaElasticSearch, CustomLOGParams:=_customLOGParams, cuaaLog)
    End Sub

    Public Sub LoggaSummaryMail(ByVal tipoMail As String, ByVal msg As String, Optional verificaInviaElasticSearch As Boolean = True, Optional ByVal cuaaLog As String = "")
        Dim customLOGParams As New CustomLOGParams With {
            .LogDescrizioneUtente = _logDescrizioneUtente,
            .LogDirectory = FileSystemHelper.AggiungiSlashSeNonEsiste(_logDirectory) & "SummaryMail",
            .LogFileName = "mail_" & tipoMail & "_" & FileSystemHelper.NomeFileUnivoco(".txt")
        }
        _objLog.Scrivi_LOG(_objParametriServer, "", msg, verificaInviaElasticSearch, CustomLOGParams:=customLOGParams, cuaaLog)
    End Sub

    Public Shared Sub ScriviLogSapAbocaImport(ByVal interfaccia As enum_InterfacceAboca, ByVal tipoLog As enum_LogMessageType, ByVal messaggio As String, ByRef objInfoLogHelperHlp As XML_Universal_Import_Log_Helper, ByRef objParametriInterscambio As AgronicaCoreParametri)

        Const nomeRoutine = "ScriviLogSapAbocaImport"

        Try
            Dim objInterscambioW As New Gias_Interscambio_W
            objInterscambioW.Scrivi_LogGiasSapItf(interfaccia.ToString, objInfoLogHelperHlp.TipoXml, objInfoLogHelperHlp.NomeFileOrig, tipoLog.ToString, messaggio, objParametriInterscambio)
        Catch ex As Exception
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
        End Try
    End Sub

    Public Shared Sub ScriviLogSapAbocaExport(ByVal interfaccia As enum_InterfacceAboca, ByVal tipoLog As enum_LogMessageType, ByVal messaggio As String, ByRef objInfoLogHlp As XML_Universal_Log_Helper, ByRef objParametriInterscambio As AgronicaCoreParametri)

        Const nomeRoutine = "ScriviLogSapAbocaExport"

        Try
            Dim objInterscambioW As New Gias_Interscambio_W
            objInterscambioW.Scrivi_LogGiasSapItf(interfaccia.ToString, If(objInfoLogHlp.TipoXml = TipiEnumerativi.enum_TipoXml.NON_SPECIFICATO, "", objInfoLogHlp.TipoXml.ToString), objInfoLogHlp.NomeFile, tipoLog.ToString, messaggio, objParametriInterscambio)
        Catch ex As Exception
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
        End Try
    End Sub

    Public Shared Sub CreaLogDettaglioExportCancellazione(ByRef ListDettaglioExport As List(Of DettaglioExportLog), ByVal chiave As String, ByVal xFiltroAggiuntivo As String, ByRef _objParametriInterscambio As AgronicaCoreParametri)

        Const nomeRoutine = "CreaLogDettaglioExportCancellazione"

        Try


            'Lettura del log con medesima chiave e tipo risorsa 
            Dim ObjGIAS_Dettaglio_Export_Log As New Gias_Interscambio_R
            Dim DtLastGIAS_Dettaglio_Export_Log As DataTable = ObjGIAS_Dettaglio_Export_Log.Leggi_LastLogDettaglioExport(chiave, "", xFiltroAggiuntivo, _objParametriInterscambio)

            If DtLastGIAS_Dettaglio_Export_Log.Rows.Count > 0 Then

                Dim DtGIAS_Dettaglio_Export_Log As DataTable = ObjGIAS_Dettaglio_Export_Log.Leggi_LogDettaglioExport(DtLastGIAS_Dettaglio_Export_Log(0)("NomeFile"), chiave, "", "", "", xFiltroAggiuntivo, "", _objParametriInterscambio)

                For Each dr In DtGIAS_Dettaglio_Export_Log.Rows

                    Dim ObjDettaglioExport As New DettaglioExportLog With {
                    .Descrizione = dr("Descrizione"),
                    .KeyAgenda = dr("KeyAgenda"),
                    .KeyDettaglio = dr("KeyDettaglio"),
                    .KeyExport = dr("KeyExport"),
                    .TipoRisorsa = dr("TipoRisorsa"),
                    .Cod_Contatto = dr("Cod_Contatto"),
                    .Cod_Risum = dr("Cod_Risum"),
                    .Mac_Cod = dr("Mac_Cod"),
                    .Elem_Cod = dr("Elem_Cod"),
                    .Pro_Cod = dr("Pro_Cod"),
                    .Mat_Cod = dr("Mat_Cod"),
                    .Lotto = dr("Lotto"),
                    .UdmGias = dr("UdmGias"),
                    .UdmExport = dr("UdmExport"),
                    .Qta = dr("Qta"),
                    .Note = ""
                     }

                    ListDettaglioExport.Add(ObjDettaglioExport)

                Next

            End If

        Catch ex As Exception
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
        End Try
    End Sub


    Public Shared Sub ScriviLogDettaglioImport(ByRef objDettaglioLogHlp As DettaglioImportLog, ByRef objParametriInterscambio As AgronicaCoreParametri)

        Const nomeRoutine = "ScriviLogDettaglioImport"

        Try
            Dim objInterscambioW As New Gias_Interscambio_W
            objInterscambioW.Scrivi_LogDettaglioImport(objDettaglioLogHlp.NomeFile,
                                                       objDettaglioLogHlp.TipoOperazione,
                                                       objDettaglioLogHlp.KeyAgenda,
                                                       objDettaglioLogHlp.KeyDettaglio,
                                                       objDettaglioLogHlp.KeyExport,
                                                       objDettaglioLogHlp.TipoRisorsa,
                                                       objDettaglioLogHlp.Descrizione,
                                                       objDettaglioLogHlp.Cod_ContattoGias,
                                                       objDettaglioLogHlp.Cod_ContattoImport,
                                                       objDettaglioLogHlp.Cod_Risum,
                                                       objDettaglioLogHlp.Rag_Soc,
                                                       objDettaglioLogHlp.Elem_Cod,
                                                       objDettaglioLogHlp.Pro_Cod,
                                                       objDettaglioLogHlp.Mat_Cod,
                                                       objDettaglioLogHlp.Lotto,
                                                       objDettaglioLogHlp.UdmGias,
                                                       objDettaglioLogHlp.UdmImport,
                                                       objDettaglioLogHlp.Qta,
                                                       objDettaglioLogHlp.Note,
                                                       objDettaglioLogHlp.Riferimento_Mov,
                                                       objDettaglioLogHlp.Tipo_Xml,
                                                       objParametriInterscambio)
        Catch ex As Exception
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
        End Try
    End Sub

    Public Shared Sub ScriviLogDettaglioExport(ByRef objDettaglioLogHlp As DettaglioExportLog, ByRef objParametriInterscambio As AgronicaCoreParametri)

        Const nomeRoutine = "ScriviLogDettaglioExport"

        Try
            Dim objInterscambioW As New Gias_Interscambio_W
            objInterscambioW.Scrivi_LogDettaglioExport(objDettaglioLogHlp.NomeFile,
                                                       objDettaglioLogHlp.TipoOperazione,
                                                       objDettaglioLogHlp.KeyAgenda,
                                                       objDettaglioLogHlp.KeyDettaglio,
                                                       objDettaglioLogHlp.KeyExport,
                                                       objDettaglioLogHlp.TipoRisorsa,
                                                       objDettaglioLogHlp.Descrizione,
                                                       objDettaglioLogHlp.Cod_Contatto,
                                                       objDettaglioLogHlp.Cod_Risum,
                                                       objDettaglioLogHlp.Mac_Cod,
                                                       objDettaglioLogHlp.Elem_Cod,
                                                       objDettaglioLogHlp.Pro_Cod,
                                                       objDettaglioLogHlp.Mat_Cod,
                                                       objDettaglioLogHlp.Lotto,
                                                       objDettaglioLogHlp.UdmGias,
                                                       objDettaglioLogHlp.UdmExport,
                                                       objDettaglioLogHlp.Qta,
                                                       objDettaglioLogHlp.Note,
                                                       objParametriInterscambio)
        Catch ex As Exception
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
        End Try
    End Sub


    Public Sub LoggaOperazioneDbXML(ByVal piva As String,
                                    ByRef objInfoLogHelper As XML_Universal_Import_Log_Helper,
                                    ByVal stato As TipiEnumerativi.enum_WFlow_Import_XML_Universale,
                                    ByVal messaggio As String,
                                    ByVal dataOperazione As DateTime,
                                    ByRef objParametriServer As AgronicaCoreParametri)

        Dim xmlImportLogW As New XML_Import_Log_W

        Try
            xmlImportLogW.Scrivi(objParametriServer, piva, objInfoLogHelper.NomeFileWork,
                                 objInfoLogHelper.IdAgenda, objInfoLogHelper.LavCod, objInfoLogHelper.TipoXml,
                                 stato, messaggio, dataOperazione)
        Catch ex As Exception
            Logga("ERRORE: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, True))
        End Try

    End Sub

    Public Sub LoggaOperazioneDbXMLSospeso(ByVal piva As String,
                                           ByRef objInfoLogHelper As XML_Universal_Import_Log_Helper,
                                           ByVal messaggio As String,
                                           ByVal dataOperazione As DateTime,
                                           ByRef objParametriServer As AgronicaCoreParametri)

        Dim xmlImportLogW As New XML_Import_Log_W

        Try
            xmlImportLogW.Scrivi(objParametriServer, piva, objInfoLogHelper.NomeFileOrig,
                                 objInfoLogHelper.IdAgenda, objInfoLogHelper.LavCod, objInfoLogHelper.TipoXml,
                                 TipiEnumerativi.enum_WFlow_Import_XML_Universale.File_Importazione_Sospesa,
                                 messaggio, dataOperazione)
        Catch ex As Exception
            Logga("ERRORE: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, True))
        End Try

    End Sub
End Class
