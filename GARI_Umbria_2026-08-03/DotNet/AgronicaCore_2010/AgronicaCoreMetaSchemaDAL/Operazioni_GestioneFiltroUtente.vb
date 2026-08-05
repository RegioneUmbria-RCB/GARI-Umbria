
Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi


Public Class Operazioni_GestioneFiltroUtente_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    'valori di default 
    'ByVal Lav_Cod As Integer = 0, _
    'ByVal P As Integer = 0, _
    'ByVal Gru_Cod As Integer = 0, _
    'ByVal Tipo As String = "", _
    'ByVal Att_Cod As Integer = 0, _
    'ByVal Cerca_LavDes As String = "", _
    'ByVal Cerca_GruDes As String = "", _
    'ByVal Flag_OpColturali As Boolean = False, _
    'ByVal Flag_OpZoo As Boolean = False, _
    'ByVal Flag_OpMacchine As Boolean = False, _
    'ByVal Flag_OpContabili As Boolean = False
    Public Function Leggi( _
                            ByVal Lav_Cod As Integer , _
                            ByVal P As Integer , _
                            ByVal Gru_Cod As Integer , _
                            ByVal Tipo As String , _
                            ByVal Att_Cod As Integer , _
                            ByVal Cerca_LavDes As String , _
                            ByVal Cerca_GruDes As String , _
                            ByVal Flag_OpColturali As Boolean , _
                            ByVal Flag_OpZoo As Boolean , _
                            ByVal Flag_OpMacchine As Boolean , _
                            ByVal Flag_OpContabili As Boolean, _
                                ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile, _
                                ByVal xFiltroAggiuntivo As String, _
                                ByVal xOrderBy As String, _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.Operazioni_GestioneFiltroUtente_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi


                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta


                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni
                    StrSQL.Append(" IF (  ")
                    StrSQL.Append(" SELECT COUNT(*)  ")
                    StrSQL.Append(" FROM          Operazioni ")
                    StrSQL.Append(" INNER JOIN    Utenti_Impostazioni_FiltroMono ")
                    StrSQL.Append("               ON Operazioni.Lav_cod = Utenti_Impostazioni_FiltroMono.ID_0 ")
                    StrSQL.Append(" INNER JOIN    GruppoOperazioni ")
                    StrSQL.Append("               ON Operazioni.Gru_Op = GruppoOperazioni.Gru_Cod ")
                    StrSQL.Append(" WHERE         Utenti_Impostazioni_FiltroMono.Piva_SuperUser = '" + Agro_SQL_SaveText(objParametri.PivaSuperUser) + "'  ")
                    StrSQL.Append(" AND           Utenti_Impostazioni_FiltroMono.UserName = '" + Agro_SQL_SaveText(objParametri.UsernameOperazione) + "'  ")
                    StrSQL.Append(" AND           Utenti_Impostazioni_FiltroMono.Impostazione_Cod = " + Agro_SQL_SaveNum(enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_LAVORAZIONI) + " ")
                    StrSQL.Append(" AND           GruppoOperazioni.Gru_Cod = " & Agro_SQL_SaveNum(4)) 'per ora considero solo le lavorazioni
                    StrSQL.Append("   ) > 0 ")


                    StrSQL.Append(" SELECT  Operazioni.LAV_COD, Operazioni.LAV_DES, Operazioni.P, Operazioni.Validita_Inizio, Operazioni.Validita_Fine, GruppoOperazioni.GRU_COD,  ")
                    StrSQL.Append("         GruppoOperazioni.GRU_DES, GruppoOperazioni.Tipo, GruppoOperazioni.ATT_COD  ")
                    StrSQL.Append(" FROM    Operazioni INNER JOIN GruppoOperazioni ON Operazioni.GRU_OP = GruppoOperazioni.GRU_COD ")
                    StrSQL.Append(" INNER JOIN    Utenti_Impostazioni_FiltroMono ")
                    StrSQL.Append("               ON Operazioni.Lav_cod = Utenti_Impostazioni_FiltroMono.ID_0 ")

                    StrSQL.Append(" WHERE Operazioni.Validita_Inizio <=" & Agro_SQL_SaveDate(CDate(objParametri.FinestraTemporaleFine)))
                    StrSQL.Append(" AND   Operazioni.Validita_Fine >=" & Agro_SQL_SaveDate(CDate(objParametri.FinestraTemporaleInizio)))
                    StrSQL.Append(" AND   Utenti_Impostazioni_FiltroMono.Piva_SuperUser = '" + Agro_SQL_SaveText(objParametri.PivaSuperUser) + "'  ")
                    StrSQL.Append(" AND   Utenti_Impostazioni_FiltroMono.UserName = '" + Agro_SQL_SaveText(objParametri.UsernameOperazione) + "'  ")
                    StrSQL.Append(" AND   Utenti_Impostazioni_FiltroMono.Impostazione_Cod = " + Agro_SQL_SaveNum(enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_LAVORAZIONI) + " ")
                    StrSQL.Append(" AND   GruppoOperazioni.Gru_Cod = " & Agro_SQL_SaveNum(4)) 'per ora considero solo le lavorazioni
                    If Lav_Cod <> 0 Then
                        StrSQL.Append(" AND Operazioni.Lav_Cod = " & Agro_SQL_SaveNum(Lav_Cod))
                    End If
                    If P <> 0 Then
                        StrSQL.Append(" AND Operazioni.P = " & Agro_SQL_SaveNum(P))
                    End If
                    
                    If Cerca_LavDes <> "" Then
                        StrSQL.Append(" AND Operazioni.Lav_Des LIKE '%" & Agro_SQL_SaveText(Cerca_LavDes) & "%' ")
                    End If
                    

                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY GruppoOperazioni.GRU_DES, Operazioni.LAV_DES ")
                    End If

                    StrSQL.Append(" ELSE ")


                    StrSQL.Append(" SELECT  Operazioni.LAV_COD, Operazioni.LAV_DES, Operazioni.P, Operazioni.Validita_Inizio, Operazioni.Validita_Fine, GruppoOperazioni.GRU_COD,  ")
                    StrSQL.Append("         GruppoOperazioni.GRU_DES, GruppoOperazioni.Tipo, GruppoOperazioni.ATT_COD  ")

                    StrSQL.Append(" FROM    Operazioni INNER JOIN GruppoOperazioni ON Operazioni.GRU_OP = GruppoOperazioni.GRU_COD ")

                    StrSQL.Append(" WHERE   Operazioni.Validita_Inizio <=" & Agro_SQL_SaveDate(CDate(objParametri.FinestraTemporaleFine)))
                    StrSQL.Append(" AND     Operazioni.Validita_Fine >=" & Agro_SQL_SaveDate(CDate(objParametri.FinestraTemporaleInizio)))

                    If Lav_Cod <> 0 Then
                        StrSQL.Append(" AND Operazioni.Lav_Cod = " & Agro_SQL_SaveNum(Lav_Cod))
                    End If
                    If P <> 0 Then
                        StrSQL.Append(" AND Operazioni.P = " & Agro_SQL_SaveNum(P))
                    End If

                    If Cerca_LavDes <> "" Then
                        StrSQL.Append(" AND Operazioni.Lav_Des LIKE '%" & Agro_SQL_SaveText(Cerca_LavDes) & "%' ")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY GruppoOperazioni.GRU_DES, Operazioni.LAV_DES ")
                    End If

                    '########################################################################

                    StrSQL.Append(" UNION ALL ")

                    '########################################################################
                    'Query x altri gruppi op.

                    StrSQL.Append(" SELECT  Operazioni.LAV_COD, Operazioni.LAV_DES, Operazioni.P, Operazioni.Validita_Inizio, Operazioni.Validita_Fine, GruppoOperazioni.GRU_COD,  ")
                    StrSQL.Append("         GruppoOperazioni.GRU_DES, GruppoOperazioni.Tipo, GruppoOperazioni.ATT_COD  ")

                    StrSQL.Append(" FROM    Operazioni INNER JOIN GruppoOperazioni ON Operazioni.GRU_OP = GruppoOperazioni.GRU_COD ")

                    StrSQL.Append(" WHERE   Operazioni.Validita_Inizio <=" & Agro_SQL_SaveDate(CDate(objParametri.FinestraTemporaleFine)))
                    StrSQL.Append(" AND     Operazioni.Validita_Fine >=" & Agro_SQL_SaveDate(CDate(objParametri.FinestraTemporaleInizio)))

                    If Lav_Cod <> 0 Then
                        StrSQL.Append(" AND Operazioni.Lav_Cod = " & Agro_SQL_SaveNum(Lav_Cod))
                    End If
                    If P <> 0 Then
                        StrSQL.Append(" AND Operazioni.P = " & Agro_SQL_SaveNum(P))
                    End If

                    If Cerca_LavDes <> "" Then
                        StrSQL.Append(" AND Operazioni.Lav_Des LIKE '%" & Agro_SQL_SaveText(Cerca_LavDes) & "%' ")
                    End If

                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY GruppoOperazioni.GRU_DES, Operazioni.LAV_DES ")
                    End If


                    StrSQL.Append("")
                    StrSQL.Append("")
                    StrSQL.Append("")
                    StrSQL.Append("")
                    StrSQL.Append("")

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta
                    '
                    '
                    '
                    '
            End Select



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



End Class
