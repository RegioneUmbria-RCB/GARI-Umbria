Imports System.Text
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri

Public Class UMA_Configurazione_Allevamenti_R
    Inherits DataProvider

    
    ''' <summary>
    ''' Alias di tabella usati: UMA_Configurazione_Allevamenti
    ''' </summary>
    ''' <param name="regioneCod"></param>
    ''' <param name="umaAllCod"></param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="xOrderBy"></param>
    ''' <param name="xSelezioneVariabile">I valori corrispondono</param>
    ''' <param name="objParametri">Viene aggiunto filtro per visibilità</param>
    ''' <returns></returns>
    Public Function Leggi(
            ByVal regioneCod As String,
            ByVal umaAllCod As String,
            ByVal xFiltroAggiuntivo As String,
            ByVal xOrderBy As String,
            ByVal xSelezioneVariabile As enumSelezioneVariabile,
            ByRef objParametri As AgronicaCoreParametri) As DataTable
        
        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.UMA_Configurazione_Allevamenti_R.Leggi()"

        Dim dt As DataTable
        Dim strSql As New StringBuilder()

        Try

            Select Case xSelezioneVariabile
                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                     enumSelezioneVariabile.Selezione_TabellaCompleta,
                     enumSelezioneVariabile.Selezione_JoinDescrizioni,
                     enumSelezioneVariabile.Selezione_JoinCompleta

                    strSql.AppendLine("SELECT UMA_Configurazione_Allevamenti.Regione_Cod + '_' ")
                    strSql.AppendLine("   + UMA_Configurazione_Allevamenti.UMA_All_Cod Chiave,")
                    strSql.AppendLine(" UMA_Configurazione_Allevamenti.*")
                    strSql.AppendLine("FROM UMA_Configurazione_Allevamenti")
                    strSql.AppendLine("WHERE 1=1")
            End Select

            If regioneCod <> "" Then
                strSql.AppendLine("AND UMA_Configurazione_Allevamenti.Regione_Cod = '" & regioneCod & "'")
            End If
            If umaAllCod <> "" Then
                strSql.AppendLine("AND UMA_Configurazione_Allevamenti.Uma_All_Cod = '" & umaAllCod & "'")
            End If
            

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append("AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.Append("AND   UMA_Configurazione_Allevamenti.Inviato >= 0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.Append("AND   UMA_Configurazione_Allevamenti.Inviato = -1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select

            If xOrderBy <> "" Then
                strSql.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If
            

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------
        Catch ex As Exception
            Scrivi_LOG(objParametri, nomeRoutine, ex.Message)
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
        End Try

        Return dt
    End Function
End Class
