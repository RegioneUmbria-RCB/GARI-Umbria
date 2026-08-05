Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider

'!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
'!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
'!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
'!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
'!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
'!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
'alcune funzioni sono state messe per sbaglio in: AgronicaCoreAnagrafeDAL.Codice_Anagrafe.vb
'!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
'!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
'!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
'!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
'!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
'!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

<CachedDataProviderAttribute("Codici_Anagrafe_R")>
Public Class Codici_Anagrafe_R
    Inherits AgronicaCoreDataProvider.CachedDataProvider

    '#########################################################################
    <Cacheable(True)>
    Public Function Leggi(ByVal Codice As Integer,
                            ByVal Gruppo As String,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.Codici_Anagrafe_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT   *")
            StrSQL.Append(" FROM     Codici_Anagrafe ")
            StrSQL.Append(" WHERE   Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & "  ")
            StrSQL.Append(" AND     Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            If Codice <> 0 Then
                StrSQL.Append(" AND     Codice = " & Agro_SQL_SaveNum(Codice) & "   ")
            End If

            If Gruppo <> "" Then
                StrSQL.Append(" AND    Gruppo = '" & Agro_SQL_SaveText(Gruppo) & "'   ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND    " + Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri) + "   ")
            End If


            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   (inviato >= 0) ")

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   (inviato =-1) ")

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select


            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY Codici_Anagrafe.descrizione  ")
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function


    '###################################################################
    Public Function DestinazioniUso_Leggi(ByVal xFiltroAggiuntivo As String,
                                        ByVal xOrderBy As String,
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                            ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.Codici_Anagrafe_R.DestinazioniUso_Leggi()"

        Dim MessaggioErrore As String = ""
        Dim DT As DataTable
        Dim FiltroDestUso As String = ""

        If xFiltroAggiuntivo <> "" Then
            FiltroDestUso = xFiltroAggiuntivo
        End If

        If FiltroDestUso <> "" Then
            FiltroDestUso &= " AND "
        End If
        FiltroDestUso &= " (Codice >= 3000) AND (gruppo = 'TERRENO') "


        Try

            DT = Leggi(0, "",
                       FiltroDestUso,
                       xOrderBy,
                         objParametri)

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function

    'Lavez - 15/10/2025 - Lettura prossimo codice disponibile per la chiave sincro clienti (UTILIZZATA SOLO NEGLI AGRONICA WEB SERVICE NELLA CREAZIONE DEL SUPER USER- NON UTILIZZARE IN AGENDA)
    Public Function GetNextCodiceSincroCliente(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Integer
        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.Codici_Anagrafe_R.GetNextCodiceSincroCliente()"
        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim ret As Integer = -1
        Try
            StrSQL.Length = 0
            StrSQL.Append(" SELECT   isnull(max(codice),0)+1 as MaxCodice ")
            StrSQL.Append(" FROM     Codici_Anagrafe ")
            StrSQL.Append(" WHERE    codice between 2000 and 2999 ")
            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------
            If DT Is Nothing Then
                Throw New Exception("Errore in lettura prossimo codice sincro cliente")
            Else
                If DT.Rows.Count > 0 Then
                    If IsDBNull(DT.Rows(0).Item("MaxCodice")) Then
                        Throw New Exception("Errore in lettura prossimo codice sincro cliente")
                    End If
                    If CInt(DT.Rows(0).Item("MaxCodice")) < 2999 Then
                        ret = CInt(DT.Rows(0).Item("MaxCodice"))
                    Else
                        Throw New Exception("[CRITICAL] Superato numero massimo chiave codice sincro!!!")
                    End If
                Else
                    Throw New Exception("Errore in lettura prossimo codice sincro cliente")
                End If
            End If
        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            ret = -1
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try
        Return ret
    End Function

End Class

'Lavez - 15/10/2025 - Aggiunta sezione eventi di scrittura (UTILIZZATA SOLO NEGLI AGRONICA WEB SERVICE NELLA CREAZIONE DEL SUPER USER- NON UTILIZZARE IN AGENDA)
Public Class Codici_Anagrafe_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function CreaNuovoCodiceSincroCliente(ByVal CodiceChiave As Integer,
                                                 ByVal Descrizione As String,
                                                 ByVal Superusername As String,
                                                 ByVal Creatore As String,
                                                 ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                 ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.Codici_Anagrafe_W.CreaNuovoCodiceSincroCliente()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim ret As Boolean = False

        Try

            StrSQL.Length = 0
            StrSQL.AppendLine(" insert into Codici_Anagrafe (")
            StrSQL.AppendLine("            Codice ")
            StrSQL.AppendLine("          , Descrizione ")
            StrSQL.AppendLine("          , Lunghezza ")
            StrSQL.AppendLine("          , Gruppo ")
            StrSQL.AppendLine("          , Genitore ")
            StrSQL.AppendLine("          , Creatore ")
            StrSQL.AppendLine("          , Data_Creazione ")
            StrSQL.AppendLine("          , Data_Modifica ")
            StrSQL.AppendLine("          , Username_Creazione ")
            StrSQL.AppendLine("          , Username_Modifica ")
            StrSQL.AppendLine("          , Validita_Inizio ")
            StrSQL.AppendLine("          , Validita_Fine ")
            StrSQL.AppendLine(" ) values (")
            StrSQL.AppendLine("           " & Agro_SQL_SaveNum(CodiceChiave) & " ")
            StrSQL.AppendLine("          ,'" & Agro_SQL_SaveText(Descrizione) & "' ")
            StrSQL.AppendLine("          ," & Agro_SQL_SaveNum(250) & " ")
            StrSQL.AppendLine("          ,'" & Agro_SQL_SaveText(Superusername) & "' ")
            StrSQL.AppendLine("          ," & Agro_SQL_SaveNum(0) & " ")
            StrSQL.AppendLine("          ,'" & Agro_SQL_SaveText(Creatore) & "' ")
            StrSQL.AppendLine("          , " & Agro_SQL_SaveDateTime(DateTime.Now) & " ")
            StrSQL.AppendLine("          , " & Agro_SQL_SaveDateTime(DateTime.Now) & " ")
            StrSQL.AppendLine("          ,'" & Agro_SQL_SaveText(Creatore) & "' ")
            StrSQL.AppendLine("          ,'" & Agro_SQL_SaveText(Creatore) & "' ")
            StrSQL.AppendLine("          , " & Agro_SQL_SaveDate(AGRODATAINIZIO) & " ")
            StrSQL.AppendLine("          , " & Agro_SQL_SaveDate(AGRODATAFINE) & " ")
            StrSQL.AppendLine(" )")


            '--------------------------------------------------------------------------
            ret = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            ret = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return ret

    End Function

End Class
