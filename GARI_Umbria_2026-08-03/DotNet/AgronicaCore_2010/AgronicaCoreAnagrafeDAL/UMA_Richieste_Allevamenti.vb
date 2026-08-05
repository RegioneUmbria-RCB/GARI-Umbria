Imports System.Text
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO

Public Class UMA_Richieste_Allevamenti_R
    Inherits DataProvider

    ''' <summary>
    ''' Alias di tabella usati: UMA_Richieste_Allevamenti, UMA_Allevamenti, UMA_Configurazione_Allevamenti, Carburanti
    ''' </summary>
    ''' <param name="piva"></param>
    ''' <param name="umaAllCod"></param>
    ''' <param name="richiestaCod"></param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="xOrderBy"></param>
    ''' <param name="xSelezioneVariabile">TabellaDatiMinimi: colonne principali; TabellaCompleta: tutte le colonne; 
    ''' JoinDescrizioni: tutte le colonne e join UMA_Allevamenti per descrizione uma_all_Cod e Carburanti per descr tipo_carburante;
    ''' JoinCompleta: tutte le colonne, join precedenti e join UMA_Configurazione_Allevamenti per colonne principali</param>
    ''' <param name="objParametri">Vengono aggiunti filtri su piva_superuser e visibilità</param>
    ''' <returns></returns>
    Public Function Leggi(
            ByVal piva As String,
            ByVal umaAllCod As String,
            ByVal richiestaCod As Integer,
            ByVal xFiltroAggiuntivo As String,
            ByVal xOrderBy As String,
            ByVal xSelezioneVariabile As enumSelezioneVariabile,
            ByRef objParametri As AgronicaCoreParametri) As DataTable
        
        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.UMA_Richieste_Allevamenti_R.Leggi()"

        Dim dt As DataTable
        Dim strSql As New StringBuilder()

        Try

            Select Case xSelezioneVariabile
                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi
                    'Colonne principali senza join
                    strSql.AppendLine("SELECT Piva + '_' + UMA_All_Cod + '_' + CAST(Richiesta_Cod AS VARCHAR) Chiave,")
                    strSql.AppendLine("Piva, UMA_All_Cod, Richiesta_Cod, Totale_Capi, Carburante_Calcolato, Carburante_Richiesto, Carburante_Approvato, Tipo_Carburante")
                    strSql.AppendLine("FROM UMA_Richieste_Allevamenti")
                    strSql.AppendLine("WHERE 1=1")

                Case enumSelezioneVariabile.Selezione_TabellaCompleta
                    'Tutte le colonne senza join
                    strSql.AppendLine("SELECT Piva + '_' + UMA_All_Cod + '_' + CAST(Richiesta_Cod AS VARCHAR) Chiave, *")
                    strSql.AppendLine("FROM UMA_Richieste_Allevamenti")
                    strSql.AppendLine("WHERE 1=1")

                Case enumSelezioneVariabile.Selezione_JoinDescrizioni
                    'Tutte le colonne con join su viste uma_allevamenti e carburanti
                    strSql.AppendLine("SELECT UMA_Richieste_Allevamenti.Piva + '_' ")
                    strSql.AppendLine("   + UMA_Richieste_Allevamenti.UMA_All_Cod + '_' ")
                    strSql.AppendLine("   + CAST(UMA_Richieste_Allevamenti.Richiesta_Cod AS VARCHAR) Chiave,")
                    strSql.AppendLine(" UMA_Richieste_Allevamenti.*,")
                    strSql.AppendLine(" UMA_Allevamenti.UMA_All_Des, UMA_Allevamenti.UMA_AllGru_Cod, UMA_Allevamenti.UMA_AllGru_Des,")
                    strSql.AppendLine(" Carburanti.Car_Cod, Carburanti.Car_Des")
                    strSql.AppendLine("FROM UMA_Richieste_Allevamenti")
                    strSql.AppendLine("INNER JOIN UMA_Allevamenti")
                    strSql.AppendLine(" ON UMA_Richieste_Allevamenti.UMA_All_Cod = Uma_Allevamenti.UMA_All_Cod")
                    strSql.AppendLine("INNER JOIN Carburanti")
                    strSql.AppendLine(" ON UMA_Richieste_Allevamenti.Tipo_Carburante = Carburanti.Car_Cod")
                    strSql.AppendLine("WHERE 1=1")

                Case enumSelezioneVariabile.Selezione_JoinCompleta
                    'Tutte le colonne con joinDescrizioni e uma_configurazione_allevamenti
                    strSql.AppendLine("SELECT UMA_Richieste_Allevamenti.Piva + '_' ")
                    strSql.AppendLine("   + UMA_Richieste_Allevamenti.UMA_All_Cod + '_' ")
                    strSql.AppendLine("   + CAST(UMA_Richieste_Allevamenti.Richiesta_Cod AS VARCHAR) Chiave,")
                    strSql.AppendLine(" UMA_Richieste_Allevamenti.*,")
                    strSql.AppendLine(" UMA_Allevamenti.UMA_All_Des, UMA_Allevamenti.UMA_AllGru_Cod, UMA_Allevamenti.UMA_AllGru_Des,")
                    strSql.AppendLine(" Uma_Configurazione_Allevamenti.Tipo_Operazione, Uma_Configurazione_Allevamenti.Gasolio_Lt, Uma_Configurazione_Allevamenti.Benzina_Lt,")
                    strSql.AppendLine(" Uma_Configurazione_Allevamenti.Qta_Aggiuntiva_Carro, Uma_Configurazione_Allevamenti.N_Max_Allevamenti,")
                    strSql.AppendLine(" Carburanti.Car_Des")
                    strSql.AppendLine("FROM UMA_Richieste_Allevamenti")
                    strSql.AppendLine("INNER JOIN UMA_Allevamenti")
                    strSql.AppendLine(" ON UMA_Richieste_Allevamenti.UMA_All_Cod = Uma_Allevamenti.UMA_All_Cod")
                    strSql.AppendLine("INNER JOIN UMA_Configurazione_Allevamenti")
                    strSql.AppendLine(" ON UMA_Richieste_Allevamenti.UMA_All_Cod = Uma_Configurazione_Allevamenti.UMA_All_Cod")
                    strSql.AppendLine("INNER JOIN Carburanti")
                    strSql.AppendLine(" ON UMA_Richieste_Allevamenti.Tipo_Carburante = Carburanti.Car_Cod")
                    strSql.AppendLine("WHERE 1=1")
            End Select

            If piva <> "" Then
                strSql.AppendLine("AND UMA_Richieste_Allevamenti.Piva = '" & piva & "'")
            End If
            If umaAllCod <> "" Then
                strSql.AppendLine("AND UMA_Richieste_Allevamenti.UMA_All_Cod = '" & umaAllCod & "'")
            End If
            If richiestaCod <> 0 Then
                strSql.AppendLine("AND UMA_Richieste_Allevamenti.Richiesta_Cod = " & richiestaCod)
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append("AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            
            strSql.AppendLine("AND UMA_Richieste_Allevamenti.Piva_SuperUser = '" & objParametri.PivaSuperUser & "'")

            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.Append("AND   UMA_Richieste_Allevamenti.Inviato >= 0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.Append("AND   UMA_Richieste_Allevamenti.Inviato = -1 ")
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

Public Class UMA_Richieste_Allevamenti_W
    Inherits DataProvider

    Public Function Aggiorna_Allevamenti(
            ByVal listaInsert As List(Of UMA_Richieste_Allevamenti),
            ByVal listaUpdate As List(Of UMA_Richieste_Allevamenti),
            ByVal listaDelete As List(Of UMA_Richieste_Allevamenti),
            ByRef objParametri As AgronicaCoreParametri) As Integer

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.UMA_Richieste_Allevamenti_W.Aggiorna()"
        Dim messaggioErrore As String = ""
        Dim risultato As Integer = 0

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Try
            Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

                For Each del In listaDelete
                    GiasContext.UMA_Richieste_Allevamenti.Attach(del)
                    GiasContext.UMA_Richieste_Allevamenti.Remove(del)
                Next

                For Each up In listaUpdate
                    GiasContext.UMA_Richieste_Allevamenti.Attach(up)
                    GiasContext.Entry(up).State = Entity.EntityState.Modified
                Next

                For Each ins In listaInsert
                    GiasContext.UMA_Richieste_Allevamenti.Add(ins)
                Next

                GiasContext.SaveChanges()
            End Using

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
            risultato = -1
        End Try

        Return risultato
    End Function


End Class