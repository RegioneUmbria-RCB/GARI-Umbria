Imports AgronicaCoreDataProvider
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreDataProvider.CostantiPersonalizzate

Imports AgronicaCoreDataProvider.DataProviderExtensions
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi

Imports agronicacoremodello
Imports System.Data.SqlClient

Public Class Agronica_Log_Invio_Chiamate_R
    Inherits AgronicaCoreDataProvider.DataProvider
    Public Function Leggi(Tipo_Esportazione As enum_Esportazioni_Sistema_Cod,
                          Esito As String,
                          Controllata As Integer,
                          Tipo_Operazione As String,
                          ObjParametri_Server As AgronicaCoreParametri,
                              Optional Dettaglio1 As String = "",
                              Optional Dettaglio2 As String = "",
                              Optional Dettaglio3 As String = "",
                              Optional Data_InvioInizio As DateTime = AGRODATAINIZIO,
                              Optional Data_InvioFine As DateTime = AGRODATAFINE,
                              Optional Ultimo_Log As Boolean = False
                          ) As DataTable
        Dim NomeRoutine As String = "AgronicaCoreVarieDAL.Agronica_Log_Invio_Chiamate_R.Leggi"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            StrSQL.Length = 0

            StrSQL.AppendLine(If(Ultimo_Log, "SELECT TOP 1 *", " SELECT * "))
            StrSQL.AppendLine(" FROM Agronica_Log_Invio_Chiamate ")
            StrSQL.AppendLine(" WHERE 1 = 1 ")

            If Tipo_Esportazione <> 0 Then
                StrSQL.AppendLine($" AND Tipo_Esportazione = {Agro_SQL_SaveNum(Tipo_Esportazione)} ")
            End If
            If Esito <> "" Then
                StrSQL.AppendLine($" AND Esito = '{Agro_SQL_SaveText(Esito)}' ")
            End If
            If Controllata <> 0 Then
                StrSQL.AppendLine($" AND Controllata = {Agro_SQL_SaveNum(Controllata)} ")
            End If
            If Tipo_Operazione <> "" Then
                StrSQL.AppendLine($" AND Tipo_Operazione = '{Agro_SQL_SaveText(Tipo_Operazione)}' ")
            End If
            If Dettaglio1 <> "" Then
                StrSQL.AppendLine($" AND Dettaglio1 = '{Agro_SQL_SaveText(Dettaglio1)}' ")
            End If
            If Dettaglio2 <> "" Then
                StrSQL.AppendLine($" AND Dettaglio2 = '{Agro_SQL_SaveText(Dettaglio2)}' ")
            End If
            If Dettaglio3 <> "" Then
                StrSQL.AppendLine($" AND Dettaglio3 = '{Agro_SQL_SaveText(Dettaglio3)}' ")
            End If
            If Data_InvioInizio <> AGRODATAINIZIO Then
                StrSQL.AppendLine($" AND Data_Invio >= {Agro_SQL_SaveDateTime(Data_InvioInizio)} ")
            End If
            If Data_InvioFine <> AGRODATAFINE Then
                StrSQL.AppendLine($" AND Data_Invio <= {Agro_SQL_SaveDateTime(Data_InvioFine)} ")
            End If

            StrSQL.AppendLine(" ORDER BY Data_Invio DESC")

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(ObjParametri_Server, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(ObjParametri_Server, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try


        Return DT
    End Function

    Public Function Get_LoggingisModalitaDemetra(objParametri_Server As AgronicaCoreParametri) As Boolean
        Dim NomeRoutine As String = "AgronicaCoreVarieDAL.Agronica_Log_Invio_Chiamate_R.Get_isModalitaDemetra"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT TOP 1 * ")
            StrSQL.AppendLine(" FROM Agronica_Log_Invio_Chiamate ")
            StrSQL.AppendLine(" WHERE Tipo_Esportazione IN (" &
                              enum_Esportazioni_Sistema_Cod.Demetra_Import_DataPublish & "," &
                              enum_Esportazioni_Sistema_Cod.Demetra_Export_Analisi & "," & enum_Esportazioni_Sistema_Cod.Demetra_Import_Analisi & "," &
                              enum_Esportazioni_Sistema_Cod.Demetra_Export_Attivita & "," & enum_Esportazioni_Sistema_Cod.Demetra_Import_Attivita & "," &
                              enum_Esportazioni_Sistema_Cod.Demetra_Export_Fabbricati & "," & enum_Esportazioni_Sistema_Cod.Demetra_Import_Fabbricati & "," &
                              enum_Esportazioni_Sistema_Cod.Demetra_Export_Fornitori & "," & enum_Esportazioni_Sistema_Cod.Demetra_Import_Fornitori & "," &
                              enum_Esportazioni_Sistema_Cod.Demetra_Export_LavoratoriQDC & "," & enum_Esportazioni_Sistema_Cod.Demetra_Import_LavoratoriQDC & "," &
                              enum_Esportazioni_Sistema_Cod.Demetra_Export_Macchine & "," & enum_Esportazioni_Sistema_Cod.Demetra_Import_Macchine &
                              ") ")

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri_Server, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        If DT.Rows.Count > 0 Then
            Return True
        Else
            Return False
        End If

    End Function

    Public Function Consulta_Log_Interscambio(ByVal Tipo_Esportazione As List(Of String),
                                              ByVal FiltroImportati As Integer,
                                              ByVal xFiltroAggiuntivo As String,
                                              ByVal xOrderBy As String,
                                              ByRef objParametri_Server As AgronicaCoreParametri,
                                              Optional ByVal Data_Inizio As Date = AGRODATAINIZIO,
                                              Optional ByVal Data_Fine As Date = AGRODATAFINE,
                                              Optional ByVal Piva_CUAA As String = "",
                                              Optional ByVal isElasticSearch As Boolean = False
                                              ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreVarieDAL.Agronica_Log_Invio_Chiamate_R.Consulta_Log_Interscambio()"
        Dim MessaggioErrore As String = ""
        Dim listaQuery As New List(Of System.Text.StringBuilder)
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Dim caricaTutto As Boolean = True
        If Tipo_Esportazione IsNot Nothing AndAlso Tipo_Esportazione.Count > 0 Then
            caricaTutto = False
        End If

        Try

#Region "EXPORT"
            If caricaTutto OrElse Tipo_Esportazione.Contains(enum_Esportazioni_Sistema_Cod.Demetra_Export_Analisi) Then
                Dim strImportExport = New System.Text.StringBuilder

                strImportExport.AppendLine("      SELECT 'Export - Analisi' AS Tipo, alic.Tipo_Esportazione ")
                strImportExport.AppendLine("             , alic.Data_Invio AS Data_Ora_Invio, alic.Esito ")
                strImportExport.AppendLine("             , alic.Dati_Inviati, alic.Dati_Ricevuti ")
                strImportExport.AppendLine("             , alic.Tipo_Operazione ")
                strImportExport.AppendLine("             , CONVERT(VARCHAR(250), alia.analisi_testata_cod) AS Chiave_GIAS, alia.Chiave_Esterna AS Chiave_Esterna ")
                strImportExport.AppendLine("             , alic.Username_Creazione ")
                strImportExport.AppendLine("             , alia.Piva ")
                If isElasticSearch Then
                    strImportExport.AppendLine("             , CONVERT(VARCHAR(250), alic.ID) AS ID_Log_Invio, alic.inviato ")
                End If
                strImportExport.AppendLine("      FROM Agronica_Log_Invio_Chiamate alic WITH (NOLOCK) ")
                strImportExport.AppendLine("      JOIN Agronica_Log_Invio_Analisi alia WITH (NOLOCK) ON alic.id = alia.ID_Log_Invio ")
                strImportExport.AppendLine("      WHERE alic.Tipo_Esportazione = " & enum_Esportazioni_Sistema_Cod.Demetra_Export_Analisi & " ")

                listaQuery.Add(strImportExport)
            End If

            If caricaTutto OrElse Tipo_Esportazione.Contains(enum_Esportazioni_Sistema_Cod.Demetra_Export_Attivita) Then
                Dim strImportExport = New System.Text.StringBuilder

                strImportExport.AppendLine("      SELECT 'Export - Attivita' AS Tipo, alic.Tipo_Esportazione ")
                strImportExport.AppendLine("             , alic.Data_Invio AS Data_Ora_Invio, alic.Esito ")
                strImportExport.AppendLine("             , alic.Dati_Inviati, alic.Dati_Ricevuti ")
                strImportExport.AppendLine("             , alic.Tipo_Operazione ")
                strImportExport.AppendLine("             , CASE WHEN aga.Piva IS NOT NULL  ")
                strImportExport.AppendLine("               THEN ")
                strImportExport.AppendLine("                    CASE WHEN CONVERT(VARCHAR(250), alia.ID_Agenda) IS NOT NULL THEN 'Quaderno ' + ISNULL(CONVERT(VARCHAR(250), alia.ID_Agenda), '') ELSE '' END  -- EXPORT AGENDA ")
                strImportExport.AppendLine("               ELSE ")
                strImportExport.AppendLine("                    CASE WHEN CONVERT(VARCHAR(250), alir.ID_Ricetta_Operazione) IS NOT NULL THEN 'Brogliaccio ' + ISNULL(CONVERT(VARCHAR(250), alir.ID_Ricetta_Operazione), '') ELSE '' END -- EXPORT RICETTA ")
                strImportExport.AppendLine("               END AS Chiave_GIAS ")
                strImportExport.AppendLine("             , CASE WHEN aga.Piva IS NOT NULL ")
                strImportExport.AppendLine("               THEN ISNULL(CONVERT(VARCHAR(250), ad.Codice), '')  -- EXPORT AGENDA ")
                strImportExport.AppendLine("               ELSE ISNULL(CONVERT(VARCHAR(250), alir.ID_Ricetta_Operazione_Esterna), '') -- EXPORT RICETTA ")
                strImportExport.AppendLine("               END AS Chiave_Esterna ")
                strImportExport.AppendLine("             , alic.Username_Creazione ")
                strImportExport.AppendLine("             , ISNULL(aga.Piva, '') + ISNULL(agr.Param3, '') AS Piva ")
                If isElasticSearch Then
                    strImportExport.AppendLine("             , CONVERT(VARCHAR(250), alic.ID) AS ID_Log_Invio, alic.inviato ")
                End If
                strImportExport.AppendLine("      FROM Agronica_Log_Invio_Chiamate alic WITH (NOLOCK) ")

                strImportExport.AppendLine("      -- EXPORT AGENDA ")
                strImportExport.AppendLine("      LEFT JOIN Agronica_Log_Invio_Agenda alia WITH (NOLOCK) ON alic.id = alia.ID_Log_Invio ")
                strImportExport.AppendLine("      LEFT JOIN Agronica_Log_Agenda_UltimaOperazione aga WITH (NOLOCK) ON alia.ID_Agenda = aga.Id_Agenda ")
                strImportExport.AppendLine("      LEFT JOIN APP_Dati ad  WITH (NOLOCK) ON SUBSTRING(riferimento, 0, CHARINDEX('|', riferimento, 0)) = CONVERT(VARCHAR(250), alia.ID_Agenda) ")

                strImportExport.AppendLine("      -- EXPORT RICETTA ")
                strImportExport.AppendLine("      LEFT JOIN Agronica_Log_Invio_Ricette alir WITH (NOLOCK) ON alic.id = alir.ID_Log_Invio  ")
                strImportExport.AppendLine("      LEFT JOIN Agronica_Log_Ricette_UltimaOperazione agr WITH (NOLOCK) ON alir.ID_Ricetta_Operazione = agr.Chiave and agr.Tipo = 'Ricette_Operazioni' ")
                strImportExport.AppendLine("      WHERE alic.Tipo_Esportazione = " & enum_Esportazioni_Sistema_Cod.Demetra_Export_Attivita & " ")

                listaQuery.Add(strImportExport)
            End If

            If caricaTutto OrElse Tipo_Esportazione.Contains(enum_Esportazioni_Sistema_Cod.Demetra_Export_Fabbricati) Then
                Dim strImportExport = New System.Text.StringBuilder

                strImportExport.AppendLine("      SELECT 'Export - Fabbricati' AS Tipo, alic.Tipo_Esportazione ")
                strImportExport.AppendLine("             , alic.Data_Invio AS Data_Ora_Invio, alic.Esito ")
                strImportExport.AppendLine("             , alic.Dati_Inviati, alic.Dati_Ricevuti ")
                strImportExport.AppendLine("             , alic.Tipo_Operazione ")
                strImportExport.AppendLine("             , CONVERT(VARCHAR(250), alia.Chiave) AS Chiave_GIAS, alia.Chiave_Esterna AS Chiave_Esterna ")
                strImportExport.AppendLine("             , alic.Username_Creazione ")
                strImportExport.AppendLine("             , alia.Piva ")
                If isElasticSearch Then
                    strImportExport.AppendLine("             , CONVERT(VARCHAR(250), alic.ID) AS ID_Log_Invio, alic.inviato ")
                End If
                strImportExport.AppendLine("      FROM Agronica_Log_Invio_Chiamate alic WITH (NOLOCK) ")
                strImportExport.AppendLine("      JOIN Agronica_Log_Invio_Anagrafe alia WITH (NOLOCK) ON alic.id = alia.ID_Log_Invio ")
                strImportExport.AppendLine("      WHERE alic.Tipo_Esportazione = " & enum_Esportazioni_Sistema_Cod.Demetra_Export_Fabbricati & " ")

                listaQuery.Add(strImportExport)
            End If

            If caricaTutto OrElse Tipo_Esportazione.Contains(enum_Esportazioni_Sistema_Cod.Demetra_Export_LavoratoriQDC) Then
                Dim strImportExport = New System.Text.StringBuilder

                strImportExport.AppendLine("      SELECT 'Export - Lavoratori QdC' AS Tipo, alic.Tipo_Esportazione ")
                strImportExport.AppendLine("             , alic.Data_Invio AS Data_Ora_Invio, alic.Esito ")
                strImportExport.AppendLine("             , alic.Dati_Inviati, alic.Dati_Ricevuti ")
                strImportExport.AppendLine("             , alic.Tipo_Operazione  ")
                strImportExport.AppendLine("             , CONVERT(VARCHAR(250), alia.Chiave) AS Chiave_GIAS, alia.Chiave_Esterna AS Chiave_Esterna ")
                strImportExport.AppendLine("             , alic.Username_Creazione ")
                strImportExport.AppendLine("             , alia.Piva ")
                If isElasticSearch Then
                    strImportExport.AppendLine("             , CONVERT(VARCHAR(250), alic.ID) AS ID_Log_Invio, alic.inviato ")
                End If
                strImportExport.AppendLine("      FROM Agronica_Log_Invio_Chiamate alic WITH (NOLOCK) ")
                strImportExport.AppendLine("      JOIN Agronica_Log_Invio_Contatti alia WITH (NOLOCK) ON alic.id = alia.ID_Log_Invio ")
                strImportExport.AppendLine("      WHERE alic.Tipo_Esportazione = " & enum_Esportazioni_Sistema_Cod.Demetra_Export_LavoratoriQDC & " ")

                listaQuery.Add(strImportExport)
            End If

            If caricaTutto OrElse Tipo_Esportazione.Contains(enum_Esportazioni_Sistema_Cod.Demetra_Export_Macchine) Then
                Dim strImportExport = New System.Text.StringBuilder

                strImportExport.AppendLine("      SELECT 'Export - Macchine' AS Tipo, alic.Tipo_Esportazione ")
                strImportExport.AppendLine("             , alic.Data_Invio AS Data_Ora_Invio, alic.Esito ")
                strImportExport.AppendLine("             , alic.Dati_Inviati, alic.Dati_Ricevuti ")
                strImportExport.AppendLine("             , alic.Tipo_Operazione ")
                strImportExport.AppendLine("             , CONVERT(VARCHAR(250), alia.Chiave) AS Chiave_GIAS, alia.Chiave_Esterna AS Chiave_Esterna ")
                strImportExport.AppendLine("             , alic.Username_Creazione ")
                strImportExport.AppendLine("             , alia.Piva ")
                If isElasticSearch Then
                    strImportExport.AppendLine("             , CONVERT(VARCHAR(250), alic.ID) AS ID_Log_Invio, alic.inviato ")
                End If
                strImportExport.AppendLine("      FROM Agronica_Log_Invio_Chiamate alic WITH (NOLOCK) ")
                strImportExport.AppendLine("      JOIN Agronica_Log_Invio_Anagrafe alia WITH (NOLOCK) ON alic.id = alia.ID_Log_Invio ")
                strImportExport.AppendLine("      WHERE alic.Tipo_Esportazione = " & enum_Esportazioni_Sistema_Cod.Demetra_Export_Macchine & " ")

                listaQuery.Add(strImportExport)
            End If

            If caricaTutto OrElse Tipo_Esportazione.Contains(enum_Esportazioni_Sistema_Cod.Demetra_Export_Fornitori) Then
                Dim strImportExport = New System.Text.StringBuilder

                strImportExport.AppendLine("      SELECT 'Export - Fornitori' AS Tipo, alic.Tipo_Esportazione ")
                strImportExport.AppendLine("             , alic.Data_Invio AS Data_Ora_Invio, alic.Esito ")
                strImportExport.AppendLine("             , alic.Dati_Inviati, alic.Dati_Ricevuti ")
                strImportExport.AppendLine("             , alic.Tipo_Operazione ")
                strImportExport.AppendLine("             , CONVERT(VARCHAR(250), alia.Chiave) AS Chiave_GIAS, alia.Chiave_Esterna AS Chiave_Esterna ")
                strImportExport.AppendLine("             , alic.Username_Creazione ")
                strImportExport.AppendLine("             , alia.Piva ")
                If isElasticSearch Then
                    strImportExport.AppendLine("             , CONVERT(VARCHAR(250), alic.ID) AS ID_Log_Invio, alic.inviato ")
                End If
                strImportExport.AppendLine("      FROM Agronica_Log_Invio_Chiamate alic WITH (NOLOCK) ")
                strImportExport.AppendLine("      JOIN Agronica_Log_Invio_Contatti alia WITH (NOLOCK) ON alic.id = alia.ID_Log_Invio ")
                strImportExport.AppendLine("      WHERE alic.Tipo_Esportazione = " & enum_Esportazioni_Sistema_Cod.Demetra_Export_Fornitori & " ")

                listaQuery.Add(strImportExport)
            End If

            If caricaTutto OrElse Tipo_Esportazione.Contains(enum_Esportazioni_Sistema_Cod.Demetra_Export_MovimentiMag) Then
                Dim strImportExport = New System.Text.StringBuilder

                strImportExport.AppendLine("      SELECT 'Export - Movimenti Magazzino' AS Tipo, alic.Tipo_Esportazione ")
                strImportExport.AppendLine("             , alic.Data_Invio AS Data_Ora_Invio, alic.Esito ")
                strImportExport.AppendLine("             , alic.Dati_Inviati, alic.Dati_Ricevuti ")
                strImportExport.AppendLine("             , alic.Tipo_Operazione ")
                strImportExport.AppendLine("             , ISNULL(CONVERT(VARCHAR(250), alia.Chiave), '') AS Chiave_GIAS,  ISNULL(CONVERT(VARCHAR(250), alia.Chiave_Esterna), '') AS Chiave_Esterna ")
                strImportExport.AppendLine("             , alic.Username_Creazione ")
                strImportExport.AppendLine("             , ISNULL(alia.Piva, '') as Piva ")
                If isElasticSearch Then
                    strImportExport.AppendLine("             , CONVERT(VARCHAR(250), alic.ID) AS ID_Log_Invio, alic.inviato ")
                End If
                strImportExport.AppendLine("      FROM Agronica_Log_Invio_Chiamate alic WITH (NOLOCK) ")
                strImportExport.AppendLine("      JOIN Agronica_Log_Invio_Agenda alia WITH (NOLOCK) ON alic.id = alia.ID_Log_Invio ")
                strImportExport.AppendLine("      WHERE alic.Tipo_Esportazione = " & enum_Esportazioni_Sistema_Cod.Demetra_Export_MovimentiMag & " ")

                listaQuery.Add(strImportExport)
            End If
#End Region

#Region "IMPORT"
            If caricaTutto OrElse Tipo_Esportazione.Contains(enum_Esportazioni_Sistema_Cod.Demetra_Import_Analisi) Then
                Dim strImportExport = New System.Text.StringBuilder

                strImportExport.AppendLine("      SELECT 'Import - Analisi' AS Tipo, alic.Tipo_Esportazione ")
                strImportExport.AppendLine("             , alic.Data_Invio AS Data_Ora_Invio, alic.Esito ")
                strImportExport.AppendLine("             , alic.Dati_Inviati, alic.Dati_Ricevuti ")
                strImportExport.AppendLine("             , alic.Tipo_Operazione ")
                strImportExport.AppendLine("             , CONVERT(VARCHAR(250), alia.Analisi_Testata_Cod) AS Chiave_GIAS, alia.Chiave_Esterna AS Chiave_Esterna")
                strImportExport.AppendLine("             , alic.Username_Creazione ")
                strImportExport.AppendLine("             , alia.Piva ")
                If isElasticSearch Then
                    strImportExport.AppendLine("             , CONVERT(VARCHAR(250), alic.ID) AS ID_Log_Invio, alic.inviato ")
                End If
                strImportExport.AppendLine("      FROM Agronica_Log_Invio_Chiamate alic WITH (NOLOCK) ")
                strImportExport.AppendLine("      JOIN Agronica_Log_Invio_Analisi alia WITH (NOLOCK) ON alic.ID = alia.ID_Log_Invio")
                strImportExport.AppendLine("      WHERE alic.Tipo_Esportazione = " & enum_Esportazioni_Sistema_Cod.Demetra_Import_Analisi & " ")

                listaQuery.Add(strImportExport)
            End If

            If caricaTutto OrElse Tipo_Esportazione.Contains(enum_Esportazioni_Sistema_Cod.Demetra_Import_Attivita) Then
                Dim strImportExport = New System.Text.StringBuilder

                strImportExport.AppendLine("      SELECT 'Import - Attivita' AS Tipo, '" & enum_Esportazioni_Sistema_Cod.Demetra_Import_Attivita & "' AS Tipo_Esportazione ")
                strImportExport.AppendLine("             , Importato_Data AS Data_Ora_Invio ")
                strImportExport.AppendLine("             , CASE WHEN Importato_Errore = '' THEN 'OK' ELSE 'KO' END AS Esito ")
                strImportExport.AppendLine("             , Dati AS Dati_Inviati, Importato_Errore AS Dati_Ricevuti ")
                strImportExport.AppendLine("             , CASE WHEN cancellato = '1' THEN 'DEL' ELSE 'INS/UPD' END AS Tipo_Operazione ")
                strImportExport.AppendLine("             , CASE WHEN SUBSTRING(Riferimento, 0, CHARINDEX('|', Riferimento, 0)) > 0 THEN 'Quaderno ' + SUBSTRING(Riferimento, 0, CHARINDEX('|', Riferimento, 0)) ELSE '' END ")
                strImportExport.AppendLine("              + CASE WHEN SUBSTRING(Riferimento, 0, CHARINDEX('|', Riferimento, 0)) > 0 AND SUBSTRING(Riferimento, CHARINDEX('|', Riferimento, 0) + 1, LEN(Riferimento)) > 0 THEN ', ' ELSE '' END ")
                strImportExport.AppendLine("              + CASE WHEN SUBSTRING(Riferimento, CHARINDEX('|', Riferimento, 0) + 1, LEN(Riferimento)) > 0 THEN ' Brogliaccio ' + SUBSTRING(Riferimento, CHARINDEX('|', Riferimento, 0) + 1, LEN(Riferimento)) END AS Chiave_GIAS ")
                strImportExport.AppendLine("             , Codice AS Chiave_Esterna ")
                strImportExport.AppendLine("             , ad.Username_Creazione ")
                strImportExport.AppendLine("             , ad.Piva  ")
                If isElasticSearch Then
                    strImportExport.AppendLine("             , ad.ID AS ID_Log_Invio, ad.inviato ")
                End If
                strImportExport.AppendLine("      FROM APP_Dati ad ")
                strImportExport.AppendLine("      WHERE ad.Tipo = 90 ")

                listaQuery.Add(strImportExport)
            End If

            If caricaTutto OrElse Tipo_Esportazione.Contains(enum_Esportazioni_Sistema_Cod.Demetra_Import_Fabbricati) Then
                Dim strImportExport = New System.Text.StringBuilder

                strImportExport.AppendLine("      SELECT  'Import - Fabbricati' AS Tipo, alic.Tipo_Esportazione ")
                strImportExport.AppendLine("             , alic.Data_Invio AS Data_Ora_Invio, alic.Esito ")
                strImportExport.AppendLine("             , alic.Dati_Inviati, alic.Dati_Ricevuti ")
                strImportExport.AppendLine("             , alic.Tipo_Operazione ")
                strImportExport.AppendLine("             , CONVERT(VARCHAR(250), alia.Chiave) AS Chiave_GIAS, alia.Chiave_Esterna AS Chiave_Esterna ")
                strImportExport.AppendLine("             , alic.Username_Creazione ")
                strImportExport.AppendLine("             , alia.Piva ")
                If isElasticSearch Then
                    strImportExport.AppendLine("             , CONVERT(VARCHAR(250), alic.ID) AS ID_Log_Invio, alic.inviato ")
                End If
                strImportExport.AppendLine("      FROM Agronica_Log_Invio_Chiamate alic WITH (NOLOCK) ")
                strImportExport.AppendLine("      JOIN Agronica_Log_Invio_Anagrafe alia WITH (NOLOCK) ON alic.ID = alia.ID_Log_Invio ")
                strImportExport.AppendLine("      WHERE alic.Tipo_Esportazione = " & enum_Esportazioni_Sistema_Cod.Demetra_Import_Fabbricati & " ")

                listaQuery.Add(strImportExport)
            End If

            If caricaTutto OrElse Tipo_Esportazione.Contains(enum_Esportazioni_Sistema_Cod.Demetra_Import_LavoratoriQDC) Then
                Dim strImportExport = New System.Text.StringBuilder

                strImportExport.AppendLine("      SELECT 'Import - Lavoratori QdC' AS Tipo, alic.Tipo_Esportazione ")
                strImportExport.AppendLine("             , alic.Data_Invio AS Data_Ora_Invio, alic.Esito ")
                strImportExport.AppendLine("             , alic.Dati_Inviati, alic.Dati_Ricevuti ")
                strImportExport.AppendLine("             , alic.Tipo_Operazione ")
                strImportExport.AppendLine("             , alia.Chiave AS Chiave_GIAS, alia.Chiave_Esterna AS Chiave_Esterna ")
                strImportExport.AppendLine("             , alic.Username_Creazione ")
                strImportExport.AppendLine("             , alia.Piva ")
                If isElasticSearch Then
                    strImportExport.AppendLine("             , CONVERT(VARCHAR(250), alic.ID) AS ID_Log_Invio, alic.inviato ")
                End If
                strImportExport.AppendLine("      FROM Agronica_Log_Invio_Chiamate alic WITH (NOLOCK) ")
                strImportExport.AppendLine("      JOIN Agronica_Log_Invio_Contatti alia WITH (NOLOCK) ON alic.id = alia.ID_Log_Invio ")
                strImportExport.AppendLine("      WHERE alic.Tipo_Esportazione = " & enum_Esportazioni_Sistema_Cod.Demetra_Import_LavoratoriQDC & " ")

                listaQuery.Add(strImportExport)
            End If

            If caricaTutto OrElse Tipo_Esportazione.Contains(enum_Esportazioni_Sistema_Cod.Demetra_Import_Macchine) Then
                Dim strImportExport = New System.Text.StringBuilder

                strImportExport.AppendLine("      SELECT 'Import - Macchine' AS Tipo, alic.Tipo_Esportazione ")
                strImportExport.AppendLine("             , alic.Data_Invio AS Data_Ora_Invio, alic.Esito ")
                strImportExport.AppendLine("             , alic.Dati_Inviati, alic.Dati_Ricevuti ")
                strImportExport.AppendLine("             , alic.Tipo_Operazione ")
                strImportExport.AppendLine("             , CONVERT(VARCHAR(250), alia.Chiave) AS Chiave_GIAS, alia.Chiave_Esterna AS Chiave_Esterna ")
                strImportExport.AppendLine("             , alic.Username_Creazione  ")
                strImportExport.AppendLine("             , alia.Piva ")
                If isElasticSearch Then
                    strImportExport.AppendLine("             , CONVERT(VARCHAR(250), alic.ID) AS ID_Log_Invio, alic.inviato ")
                End If
                strImportExport.AppendLine("      FROM Agronica_Log_Invio_Chiamate alic WITH (NOLOCK) ")
                strImportExport.AppendLine("      JOIN Agronica_Log_Invio_Anagrafe alia WITH (NOLOCK) ON alic.id = alia.ID_Log_Invio ")
                strImportExport.AppendLine("      WHERE alic.Tipo_Esportazione = " & enum_Esportazioni_Sistema_Cod.Demetra_Import_Macchine & " ")

                listaQuery.Add(strImportExport)
            End If

            If caricaTutto OrElse Tipo_Esportazione.Contains(enum_Esportazioni_Sistema_Cod.Demetra_Import_Fornitori) Then
                Dim strImportExport = New System.Text.StringBuilder

                strImportExport.AppendLine("      SELECT 'Import - Fornitori' AS Tipo, alic.Tipo_Esportazione ")
                strImportExport.AppendLine("             , alic.Data_Invio AS Data_Ora_Invio, alic.Esito ")
                strImportExport.AppendLine("             , alic.Dati_Inviati, alic.Dati_Ricevuti ")
                strImportExport.AppendLine("             , alic.Tipo_Operazione ")
                strImportExport.AppendLine("             , CONVERT(VARCHAR(250), alia.Chiave) AS Chiave_GIAS, alia.Chiave_Esterna AS Chiave_Esterna ")
                strImportExport.AppendLine("             , alic.Username_Creazione ")

                If Piva_CUAA = "" Then
                    strImportExport.AppendLine("             , '' AS Piva -- LO ESTRIAMO DAL JSON (Piva) IN DATI_INVIATI")
                Else
                    strImportExport.AppendLine("             , JSON_VALUE(alic.Dati_Inviati, '$.piva') As Piva -- in realtà è il CUAA")
                End If

                If isElasticSearch Then
                    strImportExport.AppendLine("             , CONVERT(VARCHAR(250), alic.ID) AS ID_Log_Invio, alic.inviato")
                End If
                strImportExport.AppendLine("      FROM Agronica_Log_Invio_Chiamate alic WITH (NOLOCK) ")
                strImportExport.AppendLine("      JOIN Agronica_Log_Invio_Contatti alia WITH (NOLOCK) ON alic.id = alia.ID_Log_Invio ")
                strImportExport.AppendLine("      WHERE alic.Tipo_Esportazione = " & enum_Esportazioni_Sistema_Cod.Demetra_Import_Fornitori & " ")

                listaQuery.Add(strImportExport)
            End If

            If caricaTutto OrElse Tipo_Esportazione.Contains(enum_Esportazioni_Sistema_Cod.Demetra_Import_MovimentiMag) Then
                Dim strImportExport = New System.Text.StringBuilder

                strImportExport.AppendLine("      SELECT 'Import - Movimenti Magazzino' AS Tipo, alic.Tipo_Esportazione ")
                strImportExport.AppendLine("             , alic.Data_Invio AS Data_Ora_Invio, alic.Esito ")
                strImportExport.AppendLine("             , alic.Dati_Inviati, alic.Dati_Ricevuti ")
                strImportExport.AppendLine("             , alic.Tipo_Operazione ")
                strImportExport.AppendLine("             , ISNULL(CONVERT(VARCHAR(250), alia.Chiave), '') AS Chiave_GIAS,  ISNULL(CONVERT(VARCHAR(250), alia.Chiave_Esterna), '') AS Chiave_Esterna ")
                strImportExport.AppendLine("             , alic.Username_Creazione ")

                If Piva_CUAA = "" Then
                    strImportExport.AppendLine("             , ISNULL(alia.Piva, '') AS Piva")
                Else
                    strImportExport.AppendLine("             , JSON_VALUE(alic.Dati_Inviati, '$.piva') As Piva ")
                End If

                If isElasticSearch Then
                    strImportExport.AppendLine("             , CONVERT(VARCHAR(250), alic.ID) AS ID_Log_Invio, alic.inviato")
                End If
                strImportExport.AppendLine("      FROM Agronica_Log_Invio_Chiamate alic WITH (NOLOCK) ")
                strImportExport.AppendLine("      JOIN Agronica_Log_Invio_Agenda alia WITH (NOLOCK) ON alic.id = alia.ID_Log_Invio ")
                strImportExport.AppendLine("      WHERE alic.Tipo_Esportazione = " & enum_Esportazioni_Sistema_Cod.Demetra_Import_MovimentiMag & " ")

                listaQuery.Add(strImportExport)
            End If

            If (caricaTutto AndAlso Not isElasticSearch) OrElse Tipo_Esportazione.Contains(enum_Esportazioni_Sistema_Cod.Demetra_Import_DataPublish) Then
                Dim strImportExport = New System.Text.StringBuilder

                strImportExport.AppendLine("      SELECT 'Import - Piano Colturale' AS Tipo, alic.Tipo_Esportazione ")
                strImportExport.AppendLine("             , alic.Data_Invio AS Data_Ora_Invio, alic.Esito ")
                strImportExport.AppendLine("             , alic.Dati_Inviati, alic.Dati_Ricevuti ")
                strImportExport.AppendLine("             , alic.Tipo_Operazione ")
                strImportExport.AppendLine("             , CONVERT(VARCHAR(250), alia.Chiave) AS Chiave_GIAS, alia.Chiave_Esterna AS Chiave_Esterna ")
                strImportExport.AppendLine("             , alic.Username_Creazione  ")

                If Piva_CUAA = "" Then
                    strImportExport.AppendLine("             , '' AS Piva -- lo estraiamo dalla chiave tramite il CUAA ")
                Else
                    'mi tiro fuori già il valore della Piva
                    strImportExport.AppendLine("             , SUBSTRING(v.ChiaveStr, v.Pos + 1, LEN(v.ChiaveStr)) AS Piva ")
                End If

                If isElasticSearch Then
                    strImportExport.AppendLine("             , CONVERT(VARCHAR(250), alic.ID) AS ID_Log_Invio, alic.inviato")
                End If
                strImportExport.AppendLine("      FROM Agronica_Log_Invio_Chiamate alic WITH (NOLOCK) ")
                strImportExport.AppendLine("      JOIN Agronica_Log_Invio_Anagrafe alia WITH (NOLOCK) ON alic.id = alia.ID_Log_Invio ")

                If Piva_CUAA <> "" Then
                    strImportExport.AppendLine("      CROSS APPLY ( ")
                    strImportExport.AppendLine("                    SELECT ")
                    strImportExport.AppendLine("                           CONVERT(VARCHAR(250), alia.Chiave) AS ChiaveStr, ")
                    strImportExport.AppendLine("                           CHARINDEX('_', CONVERT(VARCHAR(250), alia.Chiave)) AS Pos ")
                    strImportExport.AppendLine("      ) v ")
                End If

                strImportExport.AppendLine("      WHERE alic.Tipo_Esportazione = " & enum_Esportazioni_Sistema_Cod.Demetra_Import_DataPublish & " ")

                listaQuery.Add(strImportExport)
            End If

#End Region

            StrSQL.Length = 0

            StrSQL.AppendLine(" Select ")
            If isElasticSearch Then
                StrSQL.AppendLine(" TOP 500 ")
            End If

            StrSQL.AppendLine("        Report.*, ISNULL(u.[USER], '') AS utente ")

            StrSQL.AppendLine(" FROM ( ")
            StrSQL.AppendLine(String.Join(" UNION " & vbCrLf, listaQuery))
            StrSQL.AppendLine(" ) AS Report")

            StrSQL.AppendLine(" LEFT OUTER JOIN utenti u WITH (NOLOCK) ON u.CODICE_FISCALE = Report.Username_Creazione ")

            StrSQL.AppendLine(" WHERE 1 = 1 ")

            Select Case FiltroImportati
                Case 0 'Importati
                    StrSQL.AppendLine(" And (Report.Esito = 'OK')")
                Case 1 'Non importati e Disattivati Manualmente
                    StrSQL.AppendLine(" AND (Report.Esito IN ('KO', 'BLK'))")
            End Select

            If Data_Inizio <> AGRODATAINIZIO Then
                StrSQL.AppendLine(" AND CONVERT(Datetime, Report.Data_Ora_Invio) >= " & Agro_SQL_SaveDateTime(Data_Inizio) & " ")
            End If

            If Data_Fine <> AGRODATAFINE Then
                StrSQL.AppendLine(" AND CONVERT(Datetime, Report.Data_Ora_Invio) <= " & Agro_SQL_SaveDateTime(Data_Fine) & " ")
            End If

            StrSQL.AppendLine(" AND Report.Dati_Inviati IS NOT NULL AND Report.Dati_Inviati <> '' ")

            'se abbiamo passato anche la stringa Piva/CUAA di filtro
            If Piva_CUAA <> "" Then

                StrSQL.AppendLine(" And EXISTS ( ")
                StrSQL.AppendLine("              Select 1 ")
                StrSQL.AppendLine("               FROM Imprese_Codici ic ")
                StrSQL.AppendLine("              WHERE ic.id_cod = 1010 ")
                StrSQL.AppendLine("                  And (ic.PIVA = Report.Piva Or ic.val_cod = Report.Piva) ")
                StrSQL.AppendLine("                  And (ic.PIVA = '" & Agro_SQL_SaveText(Piva_CUAA) & "' OR ic.val_cod = '" & Agro_SQL_SaveText(Piva_CUAA) & "') ")
                StrSQL.AppendLine("            ) ")

            End If


            If isElasticSearch Then
                StrSQL.AppendLine(" AND Report.inviato = 0 ")
            End If

            '--------------------------------------------------------------------------------------------------------------------------------'

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri_Server))
            End If

            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri_Server))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri_Server, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function

    Public Function EstraiRecordPuliziaLog(ByVal TabellePulizia As Dictionary(Of String, Integer),
                                              ByVal GiorniConservazione As Integer,
                                              ByVal xFiltroAggiuntivo As String,
                                              ByRef objParametri_Server As AgronicaCoreParametri
                                              ) As DataTable
        Dim DT As New DataTable
        Dim NomeRoutine As String = "AgronicaCoreVarieDAL.Agronica_Log_Invio_Chiamate_R.EstraiRecordPuliziaLog()"
        Dim MessaggioErrore As String = ""

        Try
            Dim StrSQL As New System.Text.StringBuilder
            StrSQL.AppendLine("SELECT *")
            StrSQL.AppendLine("FROM ( ")
            StrSQL.AppendLine(" SELECT ")
            StrSQL.AppendLine("     IIf(alic.Dati_Inviati IS NULL, 0, alic.Id) AS Agronica_Log_Invio_Chiamate")

            If TabellePulizia.ContainsKey("Agronica_Log_Anagrafe") Then
                StrSQL.AppendLine(" , ISNULL(anagrafe.Agronica_Log_Anagrafe, 0) AS Agronica_Log_Anagrafe")
                StrSQL.AppendLine(" , anagrafe.Tipo")
            End If
            If TabellePulizia.ContainsKey("Agronica_Log_Analisi") Then
                StrSQL.AppendLine(" , ISNULL(analisi.Agronica_Log_Analisi, 0) AS Agronica_Log_Analisi")
            End If
            If TabellePulizia.ContainsKey("Agronica_Log_Contatti") Then
                StrSQL.AppendLine(" , ISNULL(contatti.Agronica_Log_Contatti, 0) AS Agronica_Log_Contatti")
            End If
            If TabellePulizia.ContainsKey("Agronica_Log_Agenda") Then
                StrSQL.AppendLine(" , ISNULL(agenda.Agronica_Log_Agenda, 0) AS Agronica_Log_Agenda")
            End If

            StrSQL.AppendLine(" , IIF (")
            StrSQL.AppendLine("     IIF(alic.Dati_Inviati IS NULL, 0, alic.Id)")

            If TabellePulizia.ContainsKey("Agronica_Log_Anagrafe") Then
                StrSQL.AppendLine("     + ISNULL(anagrafe.Agronica_Log_Anagrafe, 0)")
            End If
            If TabellePulizia.ContainsKey("Agronica_Log_Analisi") Then
                StrSQL.AppendLine("     + ISNULL(analisi.Agronica_Log_Analisi, 0)")
            End If
            If TabellePulizia.ContainsKey("Agronica_Log_Contatti") Then
                StrSQL.AppendLine("     + ISNULL(contatti.Agronica_Log_Contatti, 0)")
            End If
            If TabellePulizia.ContainsKey("Agronica_Log_Agenda") Then
                StrSQL.AppendLine("     + ISNULL(agenda.Agronica_Log_Agenda, 0)")
            End If

            StrSQL.AppendLine("     = 0, 'Completo', 'Da Pulire'")
            StrSQL.AppendLine(" ) AS StatoPulizia")
            StrSQL.AppendLine(" FROM Agronica_Log_Invio_Chiamate alic")

            If TabellePulizia.ContainsKey("Agronica_Log_Anagrafe") Then
                StrSQL.AppendLine(" LEFT JOIN (")
                StrSQL.AppendLine("     SELECT alia.ID_Log_Invio, alia.Tipo_Esportazione, ")
                StrSQL.AppendLine("         IIF(ala.object_data IS NULL, 0, ala.ID) AS Agronica_Log_Anagrafe,")
                StrSQL.AppendLine("         alia.Tipo")
                StrSQL.AppendLine("     FROM Agronica_Log_Invio_Anagrafe alia")
                StrSQL.AppendLine("     LEFT JOIN Agronica_Log_Anagrafe ala")
                StrSQL.AppendLine("     ON alia.Chiave = ala.Chiave ")
                StrSQL.AppendLine($"     WHERE alia.Tipo_Esportazione IN ({CInt(enum_Esportazioni_Sistema_Cod.Demetra_Import_Fabbricati)},{CInt(enum_Esportazioni_Sistema_Cod.Demetra_Export_Fabbricati)},{CInt(enum_Esportazioni_Sistema_Cod.Demetra_Export_Macchine)},{CInt(enum_Esportazioni_Sistema_Cod.Demetra_Import_Macchine)}, {CInt(enum_Esportazioni_Sistema_Cod.Demetra_Import_DataPublish)})")
                StrSQL.AppendLine("     AND alia.Tipo = ala.Tipo ")
                StrSQL.AppendLine($"     AND alia.Tipo IN ('{enum_TipoEntita_Des.Fabbricati}', '{enum_TipoEntita_Des.ParcoMacchine}', '{enum_TipoEntita_Des.DataPublish}')")

                If TabellePulizia("Agronica_Log_Anagrafe") > 0 Then
                    StrSQL.AppendLine($"        AND ala.Data_Ora_RegistrazioneLog < DATEADD(DAY,-{TabellePulizia("Agronica_Log_Anagrafe")}, CONVERT(DATETIME, SUBSTRING( CONVERT(VARBINARY(8), GETDATE()), 1, 4) + 0x00000000)) ")
                End If

                StrSQL.AppendLine(" ) anagrafe")
                StrSQL.AppendLine(" ON alic.ID = anagrafe.ID_Log_Invio")
                StrSQL.AppendLine(" AND alic.Tipo_Esportazione = anagrafe.Tipo_Esportazione")
            End If

            If TabellePulizia.ContainsKey("Agronica_Log_Analisi") Then
                StrSQL.AppendLine(" LEFT JOIN (")
                StrSQL.AppendLine("	    SELECT alia.ID_Log_Invio, alia.Tipo_Esportazione, ")
                StrSQL.AppendLine("		    IIF(ala.object_data IS NULL, 0, ala.ID) AS Agronica_Log_Analisi ")
                StrSQL.AppendLine("     FROM Agronica_Log_Invio_Analisi alia")
                StrSQL.AppendLine("     LEFT JOIN Agronica_Log_Analisi ala")
                StrSQL.AppendLine("     ON alia.Analisi_Testata_Cod = ala.Analisi_Testata_Cod")
                StrSQL.AppendLine($"     WHERE alia.Tipo_Esportazione IN ({CInt(enum_Esportazioni_Sistema_Cod.Demetra_Export_Analisi)},{CInt(enum_Esportazioni_Sistema_Cod.Demetra_Import_Analisi)})")

                If TabellePulizia("Agronica_Log_Analisi") > 0 Then
                    StrSQL.AppendLine($"        AND ala.Data_Ora_RegistrazioneLog < DATEADD(DAY,-{TabellePulizia("Agronica_Log_Analisi")}, CONVERT(DATETIME, SUBSTRING( CONVERT(VARBINARY(8), GETDATE()), 1, 4) + 0x00000000)) ")
                End If

                StrSQL.AppendLine(" ) analisi")
                StrSQL.AppendLine(" ON alic.ID = analisi.ID_Log_Invio")
                StrSQL.AppendLine(" AND alic.Tipo_Esportazione = analisi.Tipo_Esportazione")
            End If

            If TabellePulizia.ContainsKey("Agronica_Log_Contatti") Then
                StrSQL.AppendLine(" LEFT JOIN (")
                StrSQL.AppendLine("	    SELECT alia.ID_Log_Invio, alia.Tipo_Esportazione, ")
                StrSQL.AppendLine("		    IIF(ala.object_data IS NULL, 0, ala.ID) AS Agronica_Log_Contatti ")
                StrSQL.AppendLine("	    FROM Agronica_Log_Invio_Contatti alia")
                StrSQL.AppendLine("	    LEFT JOIN Agronica_Log_Contatti ala")
                StrSQL.AppendLine("	    ON alia.Chiave = ala.Chiave")
                StrSQL.AppendLine($"	    WHERE alia.Tipo_Esportazione IN ({CInt(enum_Esportazioni_Sistema_Cod.Demetra_Export_LavoratoriQDC)},{CInt(enum_Esportazioni_Sistema_Cod.Demetra_Import_LavoratoriQDC)},{CInt(enum_Esportazioni_Sistema_Cod.Demetra_Export_Fornitori)},{CInt(enum_Esportazioni_Sistema_Cod.Demetra_Import_Fornitori)})")

                If TabellePulizia("Agronica_Log_Contatti") > 0 Then
                    StrSQL.AppendLine($"        AND ala.Data_Ora_RegistrazioneLog < DATEADD(DAY,-{TabellePulizia("Agronica_Log_Contatti")}, CONVERT(DATETIME, SUBSTRING( CONVERT(VARBINARY(8), GETDATE()), 1, 4) + 0x00000000))")
                End If

                StrSQL.AppendLine(" ) contatti")
                StrSQL.AppendLine(" ON alic.ID = contatti.ID_Log_Invio")
                StrSQL.AppendLine(" AND alic.Tipo_Esportazione = contatti.Tipo_Esportazione")
            End If

            If TabellePulizia.ContainsKey("Agronica_Log_Agenda") Then
                StrSQL.AppendLine(" LEFT JOIN (")
                StrSQL.AppendLine("	    SELECT alia.ID_Log_Invio, alia.Tipo_Esportazione,")
                StrSQL.AppendLine("		    IIF(ala.object_data IS NULL, 0, ala.ID) AS Agronica_Log_Agenda ")
                StrSQL.AppendLine("	    FROM Agronica_Log_Invio_Agenda alia")
                StrSQL.AppendLine("	    LEFT JOIN Agronica_Log_Agenda ala")
                StrSQL.AppendLine("	    ON alia.Id_Agenda = ala.Id_Agenda")
                StrSQL.AppendLine($"	    WHERE alia.Tipo_Esportazione IN ({CInt(enum_Esportazioni_Sistema_Cod.Demetra_Export_Attivita)},{CInt(enum_Esportazioni_Sistema_Cod.Demetra_Import_Attivita)},{CInt(enum_Esportazioni_Sistema_Cod.Demetra_Export_MovimentiMag)},{CInt(enum_Esportazioni_Sistema_Cod.Demetra_Import_MovimentiMag)})")

                If TabellePulizia("Agronica_Log_Agenda") > 0 Then
                    StrSQL.AppendLine($"        AND ala.Data_Ora_RegistrazioneLog < DATEADD(DAY,-{TabellePulizia("Agronica_Log_Agenda")}, CONVERT(DATETIME, SUBSTRING( CONVERT(VARBINARY(8), GETDATE()), 1, 4) + 0x00000000)) ")
                End If

                StrSQL.AppendLine(" ) agenda")
                StrSQL.AppendLine(" ON alic.ID = agenda.ID_Log_Invio")
                StrSQL.AppendLine(" AND alic.Tipo_Esportazione = agenda.Tipo_Esportazione")
            End If

            StrSQL.AppendLine($" WHERE (alic.inviato = 1 OR alic.Tipo_Esportazione = {CInt(enum_Esportazioni_Sistema_Cod.Demetra_Import_DataPublish)})")
            StrSQL.AppendLine(" --Puliamo solo i log inviati a ElasticSearch (inviato=1) oppure quelli del DataPublish (Tipo_Esportazione=36) che fanno un giro diverso")
            StrSQL.AppendLine(" AND alic.Tipo_Esportazione IN (22,23,24,25,26,27,28,29,30,31,32,33,34,35,36)")
            If Not String.IsNullOrEmpty(xFiltroAggiuntivo) Then
                StrSQL.AppendLine($"    AND {Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo)}")
            End If
            StrSQL.AppendLine($"AND alic.Data_Creazione < DATEADD(DAY,-{GiorniConservazione}, CONVERT(DATETIME, SUBSTRING( CONVERT(VARBINARY(8), GETDATE()), 1, 4) + 0x00000000))")
            StrSQL.AppendLine(") totale")
            StrSQL.AppendLine("WHERE NOT StatoPulizia = 'Completo'")
            '--------------------------------------------------------------------------

            DT = EseguiQuery_Lettura(objParametri_Server, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT
    End Function
End Class

Public Class Agronica_Log_Invio_Chiamate_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Create_Agronica_Log_Invio_Chiamate(Tipo_Esportazione As enum_Esportazioni_Sistema_Cod,
                                                        DatiInviati As String,
                                                        Data_Invio As DateTime,
                                                        Esito As String,
                                                        Dati_Ricevuti As String,
                                                        Controllata As Integer,
                                                        Tipo_Operazione As String,
                                                        ObjParametri_Server As AgronicaCoreParametri,
                                                        GiasContext As Gias_DeveloperServer_Entities,
                                                            Optional Dettaglio1 As String = "",
                                                            Optional Dettaglio2 As String = "",
                                                            Optional Dettaglio3 As String = ""
                                                       ) As Agronica_Log_Invio_Chiamate

        Dim idGen As New Agro_Sequenze
        'Dim id = idGen.NuovoId_Tabella_EF(GiasContext, "Agronica_Log_Invio_Chiamate", 0, 2000000000, ObjParametri_Server)
        Dim id = idGen.NuovoId_Tabella("Agronica_Log_Invio_Chiamate", 0, 2000000000, ObjParametri_Server)
        Dim logws As New Agronica_Log_Invio_Chiamate

        logws.PivaSuperUser = ObjParametri_Server.PivaSuperUser
        logws.ID = id

        logws.Tipo_Esportazione = Tipo_Esportazione
        logws.Dati_Inviati = DatiInviati
        logws.Data_Invio = Data_Invio
        logws.Esito = Esito
        logws.Dati_Ricevuti = Dati_Ricevuti
        logws.Controllata = Controllata
        logws.Tipo_Operazione = Tipo_Operazione

        logws.Data_Creazione = DateTime.Now
        logws.Data_Modifica = DateTime.Now
        logws.inviato = 0
        logws.Username_Creazione = ObjParametri_Server.UtenteCodFiscale
        logws.Username_Modifica = ObjParametri_Server.UtenteCodFiscale
        logws.Validita_Inizio = AGRODATAINIZIO
        logws.Validita_Fine = AGRODATAFINE

        logws.Dettaglio1 = Dettaglio1
        logws.Dettaglio2 = Dettaglio2
        logws.Dettaglio3 = Dettaglio3

        GiasContext.Agronica_Log_Invio_Chiamate.Add(logws)
        GiasContext.SaveChanges()

        Return logws

    End Function

    Public Function Scrivi(Tipo_Esportazione As enum_Esportazioni_Sistema_Cod,
                           Dati_Inviati As String,
                           Data_Invio As DateTime?,
                           Esito As String,
                           Dati_Ricevuti As String,
                           Controllata As Integer,
                           Tipo_Operazione As String,
                           objParametri As AgronicaCoreParametri,
                               Optional Dettaglio1 As String = "",
                               Optional Dettaglio2 As String = "",
                               Optional Dettaglio3 As String = "",
                               Optional UtilizzaTransazione As Boolean = True
                           ) As Integer

        Dim NomeRoutine As String = "AgronicaCoreVarieDAL.Agronica_Log_Invio_Chiamate_W.Scrivi()"

        Dim StrSQL As New System.Text.StringBuilder

        Dim objSequenze As New AgronicaCoreDataProvider.Agro_Sequenze
        Dim ID = objSequenze.NuovoId_Tabella("Agronica_Log_Invio_Chiamate", 0, 2000000000, objParametri, UtilizzaTransazione:=UtilizzaTransazione)

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine(" INSERT INTO Agronica_Log_Invio_Chiamate ( ")
            StrSQL.AppendLine("           PivaSuperUser")
            StrSQL.AppendLine("         , ID")
            StrSQL.AppendLine("         , Tipo_Esportazione")
            StrSQL.AppendLine("         , Dati_Inviati")
            StrSQL.AppendLine("         , Data_Invio")
            StrSQL.AppendLine("         , Esito")
            StrSQL.AppendLine("         , Dati_Ricevuti")
            StrSQL.AppendLine("         , Controllata")
            StrSQL.AppendLine("         , Tipo_Operazione")

            StrSQL.AppendLine("         , inviato")
            StrSQL.AppendLine("         , datainvio")

            StrSQL.AppendLine("         , Data_Creazione")
            StrSQL.AppendLine("         , Data_Modifica")
            StrSQL.AppendLine("         , Username_Creazione")
            StrSQL.AppendLine("         , Username_Modifica")
            StrSQL.AppendLine("         , Validita_Inizio")
            StrSQL.AppendLine("         , Validita_Fine")

            StrSQL.AppendLine("         , Dettaglio1")
            StrSQL.AppendLine("         , Dettaglio2")
            StrSQL.AppendLine("         , Dettaglio3")
            StrSQL.AppendLine(" ) ")

            StrSQL.AppendLine(" VALUES (")
            StrSQL.AppendLine("            '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

            StrSQL.AppendLine("          , " & Agro_SQL_SaveNum(ID) & " ")
            StrSQL.AppendLine("          , " & Agro_SQL_SaveNum(Tipo_Esportazione) & " ")
            StrSQL.AppendLine("          , '" & Agro_SQL_SaveText(Dati_Inviati) & "' ")
            StrSQL.AppendLine("          , " & Agro_SQL_SaveDateTime(If(Data_Invio IsNot Nothing AndAlso Data_Invio.HasValue, Data_Invio, Date.Now)) & " ")
            StrSQL.AppendLine("          , '" & Agro_SQL_SaveText(Esito) & "' ")
            StrSQL.AppendLine("          , '" & Agro_SQL_SaveText(Dati_Ricevuti) & "' ")
            StrSQL.AppendLine("          , " & Agro_SQL_SaveNum(Controllata) & " ")
            StrSQL.AppendLine("          , '" & Agro_SQL_SaveText(Tipo_Operazione) & "' ")

            StrSQL.AppendLine("          , 0 ")
            StrSQL.AppendLine("          , NULL ")

            StrSQL.AppendLine("          , " & Agro_SQL_SaveDateTime(If(Data_Invio IsNot Nothing AndAlso Data_Invio.HasValue, Data_Invio, Date.Now)) & " ")
            StrSQL.AppendLine("          , " & Agro_SQL_SaveDateTime(If(Data_Invio IsNot Nothing AndAlso Data_Invio.HasValue, Data_Invio, Date.Now)) & " ")
            StrSQL.AppendLine("          , '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.AppendLine("          , '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.AppendLine("          , " & Agro_SQL_SaveDate(AGRODATAINIZIO) & " ")
            StrSQL.AppendLine("          , " & Agro_SQL_SaveDate(AGRODATAFINE) & " ")

            StrSQL.AppendLine("          , '" & Agro_SQL_SaveText(Dettaglio1) & "' ")
            StrSQL.AppendLine("          , '" & Agro_SQL_SaveText(Dettaglio2) & "' ")
            StrSQL.AppendLine("          , '" & Agro_SQL_SaveText(Dettaglio3) & "' ")

            StrSQL.AppendLine(" )")

            '--------------------------------------------------------------------------
            EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            Scrivi_LOG(objParametri, NomeRoutine, ex.Message)
            Throw New Exception("[" & NomeRoutine & "] : " & ex.Message)
        End Try

        Return ID

    End Function

    Public Function Modifica(ID As Integer,
                             Tipo_Esportazione As enum_Esportazioni_Sistema_Cod,
                             Dati_Inviati As String,
                             Esito As String,
                             Dati_Ricevuti As String,
                             Controllata As String,
                             Tipo_Operazione As String,
                             objParametri As AgronicaCoreParametri,
                                Optional Dettaglio1 As String = "",
                                Optional Dettaglio2 As String = "",
                                Optional Dettaglio3 As String = "",
                                Optional Data_Invio As DateTime? = Nothing
                           ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreVarieDAL.Agronica_Log_Invio_Chiamate_W.Modifica()"
        Dim StrSQL As New System.Text.StringBuilder

        If ID = 0 Then
            Throw New Exception("PARAMETRO ID OBBLIGATORIO")
        End If

        Dim xRisp As Boolean

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine(" UPDATE Agronica_Log_Invio_Chiamate SET ")
            StrSQL.AppendLine("          Data_Modifica = " & Agro_SQL_SaveDateTime(If(Data_Invio IsNot Nothing AndAlso Data_Invio.HasValue, Data_Invio, Date.Now)) & " ")
            StrSQL.AppendLine("        , Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.AppendLine("        , Data_Invio = " & Agro_SQL_SaveDateTime(If(Data_Invio IsNot Nothing AndAlso Data_Invio.HasValue, Data_Invio, Date.Now)) & " ")
            StrSQL.AppendLine("        , inviato = 0 ")
            StrSQL.AppendLine("        , datainvio = NULL ")

            If Dati_Inviati IsNot Nothing Then
                StrSQL.AppendLine("        , Dati_Inviati = '" & Agro_SQL_SaveText(Dati_Inviati) & "' ")
            End If

            If Esito <> "" Then
                StrSQL.AppendLine("        , Esito = '" & Agro_SQL_SaveText(Esito) & "' ")
            End If

            If Dati_Ricevuti IsNot Nothing Then
                StrSQL.AppendLine("        , Dati_Ricevuti = '" & Agro_SQL_SaveText(Dati_Ricevuti) & "' ")
            End If

            If Controllata <> "" Then
                StrSQL.AppendLine("        , Controllata = '" & Agro_SQL_SaveText(Controllata) & "' ")
            End If

            If Tipo_Operazione <> "" Then
                StrSQL.AppendLine("        , Tipo_Operazione = '" & Agro_SQL_SaveText(Tipo_Operazione) & "' ")
            End If

            If Dettaglio1 <> "" Then
                StrSQL.AppendLine("        , Dettaglio1 = '" & Agro_SQL_SaveText(Dettaglio1) & "' ")
            End If

            If Dettaglio2 <> "" Then
                StrSQL.AppendLine("        , Dettaglio2 = '" & Agro_SQL_SaveText(Dettaglio2) & "' ")
            End If

            If Dettaglio3 <> "" Then
                StrSQL.AppendLine("        , Dettaglio3 = '" & Agro_SQL_SaveText(Dettaglio3) & "' ")
            End If

            StrSQL.AppendLine(" WHERE ID = " & Agro_SQL_SaveNum(ID) & " ")
            StrSQL.AppendLine(" AND Tipo_Esportazione = " & Agro_SQL_SaveNum(Tipo_Esportazione) & " ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            Scrivi_LOG(objParametri, NomeRoutine, ex.Message)
            Throw New Exception("[" & NomeRoutine & "] : " & ex.Message)
        End Try

        Return xRisp

    End Function

    Public Function Cancella(ID As Integer,
                             Tipo_Esportazione As enum_Esportazioni_Sistema_Cod,
                             objParametri As AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreVarieDAL.Agronica_Log_Invio_Chiamate_W.Modifica()"
        Dim StrSQL As New System.Text.StringBuilder

        If ID = 0 Then
            Throw New Exception("PARAMETRO ID OBBLIGATORIO")
        End If

        Dim xRisp As Boolean

        Try
            StrSQL.Length = 0

            StrSQL.AppendLine(" DELETE Agronica_Log_Invio_Chiamate ")
            StrSQL.AppendLine(" WHERE ID = " & Agro_SQL_SaveNum(ID) & " ")
            StrSQL.AppendLine(" AND Tipo_Esportazione = " & Agro_SQL_SaveNum(Tipo_Esportazione) & " ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            Scrivi_LOG(objParametri, NomeRoutine, ex.Message)
            Throw New Exception("[" & NomeRoutine & "] : " & ex.Message)
        End Try

        Return xRisp

    End Function

    Public Function UpdateMassivo_Inviato_ElasticSearch(ID_Log_Invio As List(Of Integer),
                                                        timestamp As DateTime,
                                                        objParametri As AgronicaCoreParametri
                                                        ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreVarieDAL.Agronica_Log_Invio_Chiamate_W.UpdateMassivo_Inviato_ElasticSearch()"
        Dim StrSQL As New System.Text.StringBuilder

        If ID_Log_Invio.Count = 0 Then
            Throw New Exception("PARAMETRO ID_Log_Invio OBBLIGATORIO")
        End If

        Dim xRisp As Boolean

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine(" UPDATE Agronica_Log_Invio_Chiamate SET ")
            StrSQL.AppendLine("          Data_Modifica = " & Agro_SQL_SaveDateTime(Date.Now) & " ")
            StrSQL.AppendLine("        , Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.AppendLine("        , datainvio = " & Agro_SQL_SaveDateTime(timestamp) & " ")
            StrSQL.AppendLine("        , Inviato = 1")

            StrSQL.AppendLine(" WHERE ID IN (" & Agro_SQL_Save_Clausola_IN(String.Join(",", ID_Log_Invio)) & ") ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            Scrivi_LOG(objParametri, NomeRoutine, ex.Message)
            Throw New Exception("[" & NomeRoutine & "] : " & ex.Message)
        End Try

        Return xRisp

    End Function

    Public Function EseguiPuliziaLog(pulizie As DataTable, ByVal TabellePulizia As Dictionary(Of String, Integer),
                                     ByRef objParametri As AgronicaCoreParametri) As Boolean
        Dim NomeRoutine As String = "AgronicaCoreVarieDAL.Agronica_Log_Invio_Chiamate_W.EseguiPuliziaLog()"
        Dim StrSQL As New System.Text.StringBuilder
        Dim tabelleDaPulire As New List(Of String)
        Dim campoDaPulire As String
        Dim xRisp As Boolean

        Try

            xRisp = PulisciTabellaPulizia(objParametri)
            xRisp = xRisp AndAlso CreazioneTabellaPulizia(pulizie, objParametri, tabelleDaPulire, TabellePulizia)
            xRisp = xRisp AndAlso ImportaDatiTabellaPulizia(pulizie, objParametri)

            If xRisp Then
                For Each tabella As String In tabelleDaPulire
                    Select Case tabella
                        Case "Agronica_Log_Invio_Chiamate" : campoDaPulire = "Dati_Inviati"
                        Case Else : campoDaPulire = "object_data"
                    End Select

                    StrSQL.Length = 0

                    StrSQL.AppendLine($"UPDATE {tabella} ")
                    StrSQL.AppendLine($"SET {campoDaPulire} = NULL ")
                    StrSQL.AppendLine("WHERE ID IN (")
                    StrSQL.AppendLine($"SELECT {tabella}")
                    StrSQL.AppendLine("FROM Pulizia")
                    StrSQL.AppendLine($"WHERE {tabella} > 0")
                    StrSQL.AppendLine($"GROUP BY {tabella}")
                    StrSQL.AppendLine(")")

                    '--------------------------------------------------------------------------
                    xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
                    '--------------------------------------------------------------------------

                    If Not xRisp Then
                        Throw New Exception($"Pulizia della tabella [{tabella}] non completata")
                    End If
                Next
            Else
                Throw New Exception($"Pulizia delle tabelle log non completabile")
            End If

            PulisciTabellaPulizia(objParametri)

        Catch ex As Exception
            Scrivi_LOG(objParametri, NomeRoutine, ex.Message)
            Throw New Exception("[" & NomeRoutine & "] : " & ex.Message)
        End Try

        Return xRisp
    End Function

    Private Function ImportaDatiTabellaPulizia(pulizie As DataTable, objParametri As AgronicaCoreParametri) As Boolean
        Dim NomeRoutine As String = "AgronicaCoreVarieDAL.Agronica_Log_Invio_Chiamate_W.EseguiPuliziaLog.CreazioneTabellaPulizia()"
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        ConnessioniTransazioni.ApriConnessione(True, objParametri)

        Try
            While pulizie.Rows.Count > 0
                Dim chunk As DataTable = pulizie.Clone()
                Dim rows As List(Of DataRow) = pulizie.AsEnumerable().Take(5000).ToList()
                For Each row In rows
                    chunk.ImportRow(row)
                    pulizie.Rows.Remove(row)
                Next
                chunk.AcceptChanges()
                pulizie.AcceptChanges()

                Using sqlBulkCopy As New SqlBulkCopy(objParametri.objConnessione, Nothing, objParametri.objTransazione)
                    sqlBulkCopy.DestinationTableName = "Pulizia"
                    sqlBulkCopy.BulkCopyTimeout = 60
                    sqlBulkCopy.WriteToServer(chunk)
                End Using

            End While

            'commit transazione
            ConnessioniTransazioni.ChiudiTransazione(1, objParametri)
            xRisp = True

        Catch ex As Exception
            ConnessioniTransazioni.ChiudiTransazione(2, objParametri)
            Scrivi_LOG(objParametri, NomeRoutine, ex.Message)
            Throw New Exception("[" & NomeRoutine & "] : " & ex.Message)
        Finally
            ConnessioniTransazioni.ChiudiConnessione(objParametri)
        End Try

        Return xRisp
    End Function

    Private Function CreazioneTabellaPulizia(pulizie As DataTable, objParametri As AgronicaCoreParametri, tabelleDaPulire As List(Of String),
                                             tabellePulizia As Dictionary(Of String, Integer)) As Boolean
        Dim NomeRoutine As String = "AgronicaCoreVarieDAL.Agronica_Log_Invio_Chiamate_W.EseguiPuliziaLog.CreazioneTabellaPulizia()"
        Dim StrSQL As New System.Text.StringBuilder

        Try
            StrSQL.AppendLine("CREATE TABLE Pulizia")
            StrSQL.AppendLine("(")
            For Each column As DataColumn In pulizie.Columns
                If column.Ordinal > 0 Then
                    StrSQL.Append(", ")
                End If
                If (column.DataType Is GetType(Int32)) Then
                    If tabellePulizia.ContainsKey(column.ColumnName) Then
                        tabelleDaPulire.Add(column.ColumnName)
                    End If
                    StrSQL.Append(column.ColumnName).AppendLine(" INTEGER")
                Else
                    StrSQL.Append(column.ColumnName).AppendLine(" VARCHAR(100)")
                End If
            Next
            StrSQL.AppendLine(")")

            '--------------------------------------------------------------------------
            Return EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------
        Catch ex As Exception
            Scrivi_LOG(objParametri, NomeRoutine, ex.Message)
            Throw New Exception("[" & NomeRoutine & "] : " & ex.Message)
        End Try
    End Function

    Private Function PulisciTabellaPulizia(objParametri As AgronicaCoreParametri) As Boolean
        Dim NomeRoutine As String = "AgronicaCoreVarieDAL.Agronica_Log_Invio_Chiamate_W.EseguiPuliziaLog.PulisciTabellaPulizia()"
        Dim StrSQL As New System.Text.StringBuilder

        Try
            StrSQL.AppendLine("IF OBJECT_ID (N'Pulizia', N'U') IS NOT NULL")
            StrSQL.AppendLine("BEGIN")
            StrSQL.AppendLine("TRUNCATE TABLE Pulizia")
            StrSQL.AppendLine("DROP TABLE Pulizia")
            StrSQL.AppendLine("END")

            '--------------------------------------------------------------------------
            Return EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------
        Catch ex As Exception
            Scrivi_LOG(objParametri, NomeRoutine, ex.Message)
            Throw New Exception("[" & NomeRoutine & "] : " & ex.Message)
        End Try

    End Function
End Class