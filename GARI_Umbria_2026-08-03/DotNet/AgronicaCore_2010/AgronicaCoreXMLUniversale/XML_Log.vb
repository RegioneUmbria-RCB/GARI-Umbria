Imports System.Text
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider

Public Class XML_Log_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function Leggi(ByVal idLog As Integer,
                          ByVal tipo As String,
                          ByVal stato As enum_WFlow_XML_Log_Universale,
                          ByVal nomeFile As String,
                          ByVal chiave As String,
                          ByVal piva As String,
                          ByVal idAgenda As Integer,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                          Optional ByVal top1 As Boolean = False
                          ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreXMLUniversale.XML_Log_R.Leggi()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        Try

            stb.Length = 0

            stb.AppendLine(" SELECT ")
            If top1 Then
                stb.AppendLine(" TOP(1) ")
            End If
            stb.AppendLine("  * ")
            stb.AppendLine(" FROM XML_Log ")
            stb.AppendLine(" WHERE 1=1 ")

            If idLog <> 0 Then
                stb.AppendLine(" AND Id_Log = " & Agro_SQL_SaveNum(idLog) & " ")
            End If

            If tipo <> "" Then
                stb.AppendLine(" AND Tipo = '" & Agro_SQL_SaveText(tipo) & "' ")
            End If

            If stato <> enum_WFlow_XML_Log_Universale.Nessuno Then
                stb.AppendLine(" AND Stato = " & Agro_SQL_SaveNum(CInt(stato)) & " ")
            End If

            If nomeFile <> "" Then
                stb.AppendLine(" AND NomeFile = '" & Agro_SQL_SaveText(nomeFile) & "' ")
            End If

            If chiave <> "" Then
                stb.AppendLine(" AND Chiave = '" & Agro_SQL_SaveText(chiave) & "' ")
            End If

            If piva <> "" Then
                stb.AppendLine(" AND Piva = '" & Agro_SQL_SaveText(piva) & "' ")
            End If

            If idAgenda <> 0 Then
                stb.AppendLine(" AND Id_Agenda = " & Agro_SQL_SaveNum(idAgenda) & " ")
            End If


            If xFiltroAggiuntivo <> "" Then
                stb.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            If xOrderBy <> "" Then
                stb.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    Public Function LeggiLogPerCategoria(ByVal configurazioneServizio As AgronicaCoreEntityFramework_POCO.Configurazione_Servizi_Guida,
                                         ByVal stato As enum_WFlow_XML_Log_Universale,
                                         ByRef objParametri As AgronicaCoreParametri) As DataTable
        Dim dtRisp As New DataTable

        Select Case configurazioneServizio.Categoria_Log
            Case XML_Log_Categoria_Agenda
                dtRisp = LeggiLogAgenda(configurazioneServizio, stato, objParametri)

            Case XML_Log_Categoria_Agenda_Costi
                dtRisp = LeggiLogCatAgenda(configurazioneServizio, stato, objParametri)

            Case XML_Log_Categoria_Impianti
                dtRisp = LeggiLogCatImpianti(configurazioneServizio, stato, objParametri)

            Case Else

        End Select

        Return dtRisp

    End Function


    Private Function LeggiLogAgenda(ByVal configurazioneServizio As AgronicaCoreEntityFramework_POCO.Configurazione_Servizi_Guida,
                                       ByVal stato As enum_WFlow_XML_Log_Universale,
                                       ByRef objParametri As AgronicaCoreParametri) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreXMLUniversale.XML_Log_R.LeggiLogAgenda()"

        Dim dtLogCat As DataTable
        Dim stb As New StringBuilder

        Try

            stb.Length = 0

            stb.AppendLine(" SELECT XML_Log.Id_Log, XML_Log.Chiave, XML_Log.Stato, XML_Log.Data_Operazione, XML_Log.Note, XML_Log.Piva As XML_Log_Piva")
            stb.AppendLine(" ,Imprese.Rag_Soc ")
            stb.AppendLine(" ,Agenda.Id_Agenda AS Id_Agenda_Principale ")
            stb.AppendLine(" ,Agenda.Lav_Cod AS Lav_Cod_Principale ")
            stb.AppendLine(" ,Agenda.Des_Lib")
            stb.AppendLine(" ,Agenda.Validita_Inizio As Data_Agenda_Principale ")
            stb.AppendLine(" FROM XML_Log ")
            stb.AppendLine(" INNER JOIN Agenda")
            stb.AppendLine(" ON XML_Log.Piva = Agenda.Piva AND XML_Log.Id_Agenda = Agenda.Id_Agenda ")
            stb.AppendLine(" INNER JOIN Imprese ")
            stb.AppendLine(" ON XML_Log.Piva = Imprese.PIVA ")
            stb.AppendLine(" WHERE Tipo = '" & Agro_SQL_SaveText(configurazioneServizio.Tipo_Log) & "'")

            If stato <> enum_WFlow_XML_Log_Universale.Nessuno Then
                stb.AppendLine(" AND Stato = " & Agro_SQL_SaveNum(CInt(stato)) & " ")
            End If

            If configurazioneServizio.Piva <> "" Then
                stb.AppendLine(" AND XML_Log.Piva = '" & Agro_SQL_SaveText(configurazioneServizio.Piva) & "' ")
            End If

            stb.AppendLine(" ORDER BY XML_Log.Data_Operazione DESC ")

            '--------------------------------------------------------------------------
            dtLogCat = EseguiQuery_Lettura(objParametri, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            Scrivi_LOG(objParametri, nomeRoutine, ex.Message)
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
        End Try

        Return dtLogCat

    End Function

    Private Function LeggiLogCatAgenda(ByVal configurazioneServizio As AgronicaCoreEntityFramework_POCO.Configurazione_Servizi_Guida,
                                       ByVal stato As enum_WFlow_XML_Log_Universale,
                                       ByRef objParametri As AgronicaCoreParametri) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreXMLUniversale.XML_Log_R.LeggiLogCatAgenda()"

        Dim dtLogCat As DataTable
        Dim stb As New StringBuilder

        Try

            stb.Length = 0

            stb.AppendLine(" SELECT XML_Log.Id_Log, XML_Log.Chiave, XML_Log.Stato, XML_Log.Data_Operazione, XML_Log.Note, XML_Log.Piva As XML_Log_Piva")
            stb.AppendLine(" ,Imprese.Rag_Soc ")
            stb.AppendLine(" ,Agenda.Id_Agenda AS Id_Agenda_Principale ")
            stb.AppendLine(" ,Agenda.Lav_Cod AS Lav_Cod_Principale ")
            stb.AppendLine(" ,Agenda.Des_Lib")
            stb.AppendLine(" ,Agenda.Validita_Inizio As Data_Agenda_Principale ")
            stb.AppendLine(" ,Mov_Dettagli_Riferimenti.Piva AS Piva_Referente ")
            stb.AppendLine(" ,Mov_Dettagli_Riferimenti.Id_Agenda AS Id_Agenda_Referente ")
            stb.AppendLine(" ,Mov_Dettagli_Riferimenti.Lav_Cod AS Lav_Cod_Referente ")
            stb.AppendLine(" FROM XML_Log ")
            stb.AppendLine(" INNER JOIN Agenda")
            stb.AppendLine(" ON XML_Log.Piva = Agenda.Piva AND XML_Log.Id_Agenda = Agenda.Id_Agenda ")
            stb.AppendLine(" INNER JOIN Imprese ")
            stb.AppendLine(" ON XML_Log.Piva = Imprese.PIVA ")
            stb.AppendLine(" LEFT JOIN Mov_Dettagli_Riferimenti ")
            stb.AppendLine(" ON Agenda.Piva = Mov_Dettagli_Riferimenti.Piva_Rif AND Agenda.Id_Agenda = Mov_Dettagli_Riferimenti.Id_Agenda_Rif") 'l'Agenda del log è l'operazione riferita nel collegamento
            stb.AppendLine(" WHERE Tipo = '" & Agro_SQL_SaveText(configurazioneServizio.Tipo_Log) & "'")

            If stato <> enum_WFlow_XML_Log_Universale.Nessuno Then
                stb.AppendLine(" AND Stato = " & Agro_SQL_SaveNum(CInt(stato)) & " ")
            End If

            If configurazioneServizio.Piva <> "" Then
                stb.AppendLine(" AND XML_Log.Piva = '" & Agro_SQL_SaveText(configurazioneServizio.Piva) & "' ")
            End If

            stb.AppendLine(" ORDER BY XML_Log.Data_Operazione DESC ")

            '--------------------------------------------------------------------------
            dtLogCat = EseguiQuery_Lettura(objParametri, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            Scrivi_LOG(objParametri, nomeRoutine, ex.Message)
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
        End Try

        Return dtLogCat

    End Function

    Private Function LeggiLogCatImpianti(ByVal configurazioneServizio As AgronicaCoreEntityFramework_POCO.Configurazione_Servizi_Guida,
                                         ByVal stato As enum_WFlow_XML_Log_Universale,
                                         ByRef objParametri As AgronicaCoreParametri) As DataTable

        Dim dtLogCat As New DataTable

        'TODO

        Return dtLogCat

    End Function

End Class



'#################################################################
'#################################################################
'#################################################################

Public Class XML_Log_W
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function Scrivi(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                           ByVal tipo As String,
                           ByVal stato As enum_WFlow_XML_Log_Universale,
                           ByVal nomeFile As String,
                           ByVal dataOperazione As DateTime,
                           ByVal chiave As String,
                           ByVal piva As String,
                           ByVal idAgenda As Integer,
                           ByVal saCod As Integer,
                           ByVal appezza As Integer,
                           ByVal idReg As Integer,
                           ByVal progettoCod As Integer,
                           ByVal codAnimale As Integer,
                           ByVal codAnimaleDistinta As Integer,
                           ByVal note As String,
                           Optional ByVal dataCreazione As DateTime = #2/1/1900#,
                           Optional ByVal dataModifica As DateTime = #2/1/1900#
                           ) As Boolean


        Dim nomeRoutine As String = "AgronicaCoreXMLUniversale.XML_Log_W.Scrivi()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            If dataCreazione = #2/1/1900# Then
                dataCreazione = DateTime.Now
            End If

            If dataModifica = #2/1/1900# Then
                dataModifica = DateTime.Now
            End If


            '---------------------------------------------
            stb.Length = 0
            stb.AppendLine(" INSERT INTO XML_Log ")

            stb.AppendLine("       (")

            stb.AppendLine("       Tipo,        Stato,           NomeFile,         Data_Operazione,   Chiave,     Piva, ")
            stb.AppendLine("       Id_Agenda,   Sa_Cod,          Appezza,          Id_Reg,     Progetto_Cod, ")
            stb.AppendLine("       Cod_Animale, Cod_Animale_Distinta, ")

            stb.AppendLine("       Note,        Data_Creazione,  Data_Modifica ")
            stb.AppendLine("       ) ")

            stb.AppendLine(" VALUES ( ")

            stb.AppendLine("        '" & Agro_SQL_SaveText(tipo) & "'  ")
            stb.AppendLine("		, " & Agro_SQL_SaveNum(stato) & "  ")
            stb.AppendLine("	    ,'" & Agro_SQL_SaveText(nomeFile) & "'  ")
            stb.AppendLine("		, " & Agro_SQL_SaveDateTime(dataOperazione) & "  ")
            stb.AppendLine("	    ,'" & Agro_SQL_SaveText(chiave) & "'  ")
            stb.AppendLine("	    ,'" & Agro_SQL_SaveText(piva) & "'  ")
            stb.AppendLine("		, " & Agro_SQL_SaveNum(idAgenda) & "  ")
            stb.AppendLine("		, " & Agro_SQL_SaveNum(saCod) & "  ")
            stb.AppendLine("		, " & Agro_SQL_SaveNum(appezza) & "  ")
            stb.AppendLine("		, " & Agro_SQL_SaveNum(idReg) & "  ")
            stb.AppendLine("		, " & Agro_SQL_SaveNum(progettoCod) & "  ")
            stb.AppendLine("		, " & Agro_SQL_SaveNum(codAnimale) & "  ")
            stb.AppendLine("		, " & Agro_SQL_SaveNum(codAnimaleDistinta) & "  ")

            stb.AppendLine("	    ,'" & Agro_SQL_SaveText(note) & "'  ")
            stb.AppendLine("		, " & Agro_SQL_SaveDateTime(dataCreazione) & "  ")
            stb.AppendLine("		, " & Agro_SQL_SaveDateTime(dataModifica) & "  ")

            stb.AppendLine(") ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return xRisp

    End Function

End Class
