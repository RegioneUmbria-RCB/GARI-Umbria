Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.AgronicaCoreParametri

Public Class Gias_Service_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function Leggi_WS_operazioni( _
            ByVal id_pacchetto As Integer, _
            ByVal NomeUtente As String, _
            ByVal xFiltroAggiuntivo As String, _
            ByVal xOrderBy As String, _
            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
            ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "Gias_service_R.Leggi()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            stb.Length = 0


            stb.Append(" " & vbCrLf)
            stb.Append(" select poxu.utente, op.* " & vbCrLf)
            stb.Append(" from WS_Operazioni op " & vbCrLf)

            stb.Append(" inner join WS_Operazioni_x_Pacchetto_Operazioni po " & vbCrLf)
            stb.Append("    on po.ID_Operazione = op.ID_OPERAZIONE  " & vbCrLf)
            stb.Append(" inner join WS_Pacchetto_Operazioni_X_Utenti poXu " & vbCrLf)
            stb.Append("    on poXu.id_Pacchetto = po.ID_Pacchetto  " & vbCrLf)
            stb.Append(" where poXu.utente = '" & Agro_SQL_SaveText(NomeUtente) & "' " & vbCrLf)
            'stb.Append(" and poxu.id_pacchetto = " & id_pacchetto & " " & vbCrLf)

            'stb.Append(" for xml path('operazione'), root('operazioni') " & vbCrLf)


            If xFiltroAggiuntivo <> "" Then
                stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If


            stb.Append(" Order by op.ID_OPERAZIONE ")

            If xOrderBy <> "" Then
                stb.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT





    End Function



    '##############################################################################################
    Public Function Leggi_WS_Descr_operazioni( _
            ByVal id_pacchetto As Integer, _
            ByVal NomeUtente As String, _
            ByVal xFiltroAggiuntivo As String, _
            ByVal xOrderBy As String, _
            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
            ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "Gias_service_R.Leggi()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            stb.Length = 0


            stb.Append(" " & vbCrLf)
            stb.Append(" select poxu.utente " & vbCrLf)            
            stb.Append("    , d.ID_Descr_Operazioni " & vbCrLf)
            stb.Append("    , d.ID_WS_OPERAZIONI " & vbCrLf)
            stb.Append("    , d.ORDINE " & vbCrLf)
            stb.Append("    , coalesce(dbU.Utente_DB, d.DB) as DB " & vbCrLf)
            stb.Append("    , d.TABELLA " & vbCrLf)
            stb.Append("    , d.QUERY_SQL " & vbCrLf)
            stb.Append("    , d.OPERA_ON_SRV_CLI " & vbCrLf)
            stb.Append("    , d.DESCRIZIONE " & vbCrLf)
            stb.Append("    , d.CHIAVI " & vbCrLf)
            stb.Append("    , d.CAMPI " & vbCrLf)
            stb.Append("    , d.TIPI_CHIAVE " & vbCrLf)
            stb.Append("    , d.TIPI_CAMPO " & vbCrLf)
            stb.Append("    , d.TIPI_CAMPO1")

            stb.Append(" from WS_Operazioni op " & vbCrLf)
            stb.Append(" inner join WS_Descr_Operazioni d " & vbCrLf)
            stb.Append("    on op.ID_OPERAZIONE = d.ID_WS_OPERAZIONI  " & vbCrLf)
            stb.Append(" inner join WS_Operazioni_x_Pacchetto_Operazioni po " & vbCrLf)
            stb.Append("    on po.ID_Operazione = op.ID_OPERAZIONE  " & vbCrLf)
            stb.Append(" inner join WS_Pacchetto_Operazioni_X_Utenti poXu " & vbCrLf)
            stb.Append("    on poXu.id_Pacchetto = po.ID_Pacchetto  " & vbCrLf)
            stb.Append("left join WS_Utenti_Database dbU " & vbCrLf  ) 
            stb.Append("    on dbU.Descr_Operazioni_DB = d.DB ")
            stb.Append("    and dbU.Utente =  '" & Agro_SQL_SaveText(NomeUtente) & "' " & vbCrLf)

            stb.Append(" where poXu.utente = '" & Agro_SQL_SaveText(NomeUtente) & "' " & vbCrLf)
            'stb.Append(" and poxu.id_pacchetto = " & id_pacchetto & " " & vbCrLf)
            stb.Append("  Order By d.ID_WS_OPERAZIONI,d.ORDINE ")
                'stb.Append(" for xml path('operazione'), root('operazioni') " & vbCrLf)


            If xFiltroAggiuntivo <> "" Then
                stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If


            If xOrderBy <> "" Then
                stb.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

                '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, stb.ToString, NomeRoutine)
                '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT





    End Function


End Class


'#################################################################
'#################################################################
'#################################################################

Public Class Gias_Service_W
    Inherits AgronicaCoreDataProvider.DataProvider



    '##############################################################################################
    Public Function Scrivi(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                , Optional ByVal Data_creazione As Date = #2/1/1900# _
                , Optional ByVal Data_modifica As Date = #2/1/1900# _
                , Optional ByVal username_creazione As String = "" _
                , Optional ByVal username_modifica As String = "" _
                ) As Boolean


        Dim NomeRoutine As String = "Scrivi()"

        '====================================================================================
        'Parametri opzionali :

        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If Data_creazione = #2/1/1900# Then
                Data_creazione = Date.Now
            End If

            If Data_modifica = #2/1/1900# Then
                Data_modifica = Date.Now
            End If

            If username_creazione = "" Then
                username_creazione = objParametri.UsernameOperazione
            End If

            If username_modifica = "" Then
                username_modifica = objParametri.UsernameOperazione
            End If



            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append(" INSERT ... " + vbCrLf)

            StrSQL.Append("              (")
            StrSQL.Append("              Inviato,            datainvio, ")
            StrSQL.Append("              Data_Creazione,     Data_Modifica, ")
            StrSQL.Append("              UserName_Creazione, UserName_Modifica, ")
            StrSQL.Append("              Validita_Inizio,    Validita_Fine, ")
            StrSQL.Append("              Validazione, Data_Validazione, UserName_Validazione " + vbCrLf)
            StrSQL.Append("              ) ")

            StrSQL.Append(" VALUES ( ")



            StrSQL.Append("         , 0  " + vbCrLf)
            StrSQL.Append("         , Null  " + vbCrLf)

            StrSQL.Append("			, " & Agro_SQL_SaveDate(Data_creazione) & "  ")
            StrSQL.Append("			, " & Agro_SQL_SaveDate(Data_modifica) & "  ")
            StrSQL.Append("			,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            StrSQL.Append("			,'" & Agro_SQL_SaveText(username_modifica) & "' ")



            StrSQL.Append(") ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return xRisp

    End Function







    '#################################################################
    Public Function Cancella(ByVal xFiltroAggiuntivo As String, _
                              ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                              ) As Boolean

        '----- Descrizione
        Dim NomeRoutine As String = "Cancella()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            StrSQL.Length = 0

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then
                StrSQL.Append(" UPDATE ... ")
                StrSQL.Append(" SET ")
                StrSQL.Append("         Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("         ,Data_Modifica= " & Agro_SQL_SaveDate(Date.Now) & " ")
                StrSQL.Append("         ,Inviato = -1 ")
                StrSQL.Append(" WHERE   1=1 ")
                StrSQL.Append(" AND     Inviato >= 0 ")
            Else
                StrSQL.Append(" DELETE FROM ... ")
                StrSQL.Append(" WHERE 1=1 ")
            End If
            '---------------------------------------------

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp

    End Function




End Class



