

Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi

Public Class OperazioniLavorazioni_GestioneFiltroUtente_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
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
    'ByVal Flag_OpContabili As Boolean = False, _

    Public Function Leggi( _
                            ByVal Lav_Cod As Integer, _
                            ByVal P As Integer, _
                            ByVal Gru_Cod As Integer, _
                            ByVal Tipo As String, _
                            ByVal Att_Cod As Integer, _
                            ByVal Cerca_LavDes As String, _
                            ByVal Cerca_GruDes As String, _
                            ByVal Flag_OpColturali As Boolean, _
                            ByVal Flag_OpZoo As Boolean, _
                            ByVal Flag_OpMacchine As Boolean, _
                            ByVal Flag_OpContabili As Boolean, _
                                ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile, _
                                ByVal xFiltroAggiuntivo As String, _
                                ByVal xOrderBy As String, _
                                ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.OperazioniLavorazioni_GestioneFiltroUtente_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim stbQuery As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi


                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta


                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni
                    stbQuery.Append(" IF (  ")
                    stbQuery.Append(" SELECT COUNT(*)  ")
                    stbQuery.Append(" FROM          Operazioni ")
                    stbQuery.Append(" INNER JOIN    Utenti_Impostazioni_FiltroMono ")
                    stbQuery.Append("               ON Operazioni.Lav_cod = Utenti_Impostazioni_FiltroMono.ID_0 ")
                    stbQuery.Append(" INNER JOIN    GruppoOperazioni ")
                    stbQuery.Append("               ON Operazioni.Gru_Op = GruppoOperazioni.Gru_Cod ")
                    stbQuery.Append(" WHERE         Utenti_Impostazioni_FiltroMono.Piva_SuperUser = '" + Agro_SQL_SaveText(objParametri_Utenti.PivaSuperUser) + "'  ")
                    stbQuery.Append(" AND           Utenti_Impostazioni_FiltroMono.UserName = '" + Agro_SQL_SaveText(objParametri_Utenti.UtenteUsername) + "'  ")
                    stbQuery.Append(" AND           Utenti_Impostazioni_FiltroMono.Impostazione_Cod = " + Agro_SQL_SaveNum(enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_LAVORAZIONI) + " ")
                    stbQuery.Append(" AND           GruppoOperazioni.Gru_Cod = " & Agro_SQL_SaveNum(4)) 'per ora considero solo le lavorazioni
                    stbQuery.Append("   ) > 0 ")


                    stbQuery.Append(" SELECT  Operazioni.LAV_COD, Operazioni.LAV_DES, Operazioni.P, Operazioni.Validita_Inizio, Operazioni.Validita_Fine, GruppoOperazioni.GRU_COD,  ")
                    stbQuery.Append("         GruppoOperazioni.GRU_DES, GruppoOperazioni.Tipo, GruppoOperazioni.ATT_COD  ")
                    stbQuery.Append(" FROM    Operazioni INNER JOIN GruppoOperazioni ON Operazioni.GRU_OP = GruppoOperazioni.GRU_COD ")
                    stbQuery.Append(" INNER JOIN    Utenti_Impostazioni_FiltroMono ")
                    stbQuery.Append("               ON Operazioni.Lav_cod = Utenti_Impostazioni_FiltroMono.ID_0 ")

                    stbQuery.Append(" WHERE Operazioni.Validita_Inizio <=" & Agro_SQL_SaveDate(CDate(objParametri_Utenti.FinestraTemporaleFine)))
                    stbQuery.Append(" AND   Operazioni.Validita_Fine >=" & Agro_SQL_SaveDate(CDate(objParametri_Utenti.FinestraTemporaleFine)))
                    stbQuery.Append(" AND   Utenti_Impostazioni_FiltroMono.Piva_SuperUser = '" + Agro_SQL_SaveText(objParametri_Utenti.PivaSuperUser) + "'  ")
                    stbQuery.Append(" AND   Utenti_Impostazioni_FiltroMono.UserName = '" + Agro_SQL_SaveText(objParametri_Utenti.UtenteUsername) + "'  ")
                    stbQuery.Append(" AND   Utenti_Impostazioni_FiltroMono.Impostazione_Cod = " + Agro_SQL_SaveNum(enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_LAVORAZIONI) + " ")
                    stbQuery.Append(" AND   GruppoOperazioni.Gru_Cod = " & Agro_SQL_SaveNum(4)) 'per ora considero solo le lavorazioni
                    If Lav_Cod <> 0 Then
                        stbQuery.Append(" AND Operazioni.Lav_Cod = " & Agro_SQL_SaveNum(Lav_Cod))
                    End If
                    If P <> 0 Then
                        stbQuery.Append(" AND Operazioni.P = " & Agro_SQL_SaveNum(P))
                    End If
                    'If Gru_Cod <> 0 Then
                    '    stbQuery.Append(" AND GruppoOperazioni.Gru_Cod = " & agro_SQL_SaveNum(Gru_Cod))
                    'End If
                    'If Tipo <> "" Then
                    '    stbQuery.Append(" AND GruppoOperazioni.Tipo = '" & agro_SQL_SaveText(Tipo) & "' ")
                    'End If
                    'If Att_Cod <> 0 Then
                    '    stbQuery.Append(" AND GruppoOperazioni.Att_Cod = " & agro_SQL_SaveNum(Att_Cod))
                    'End If
                    If Cerca_LavDes <> "" Then
                        stbQuery.Append(" AND Operazioni.Lav_Des LIKE '%" & Agro_SQL_SaveText(Cerca_LavDes) & "%' ")
                    End If
                    'If Cerca_GruDes <> "" Then
                    '    stbQuery.Append(" AND GruppoOperazioni.Gru_Des LIKE '%" & agro_SQL_SaveText(Cerca_GruDes) & "%' ")
                    'End If
                    'If Flag_OpColturali = True Then
                    '    stbQuery.Append(" AND GruppoOperazioni.Tipo = 'C' ")
                    'End If
                    'If Flag_OpZoo = True Then
                    '    stbQuery.Append(" AND GruppoOperazioni.Tipo = 'Z' ")
                    'End If
                    'If Flag_OpMacchine = True Then
                    '    stbQuery.Append(" AND GruppoOperazioni.Tipo = 'P' ")
                    'End If
                    'If Flag_OpContabili = True Then
                    '    stbQuery.Append(" AND GruppoOperazioni.Tipo = 'E' ")
                    'End If

                    If xFiltroAggiuntivo <> "" Then
                        stbQuery.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri_Utenti))
                    End If
                    If xOrderBy <> "" Then
                        stbQuery.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri_Utenti))
                    Else
                        stbQuery.Append(" ORDER BY GruppoOperazioni.GRU_DES, Operazioni.LAV_DES ")
                    End If


                    stbQuery.Append(" ELSE ")


                    stbQuery.Append(" SELECT  Operazioni.LAV_COD, Operazioni.LAV_DES, Operazioni.P, Operazioni.Validita_Inizio, Operazioni.Validita_Fine, GruppoOperazioni.GRU_COD,  ")
                    stbQuery.Append("         GruppoOperazioni.GRU_DES, GruppoOperazioni.Tipo, GruppoOperazioni.ATT_COD  ")

                    stbQuery.Append(" FROM    Operazioni INNER JOIN GruppoOperazioni ON Operazioni.GRU_OP = GruppoOperazioni.GRU_COD ")

                    stbQuery.Append(" WHERE   Operazioni.Validita_Inizio <=" & Agro_SQL_SaveDate(CDate(objParametri_Utenti.FinestraTemporaleFine)))
                    stbQuery.Append(" AND     Operazioni.Validita_Fine >=" & Agro_SQL_SaveDate(CDate(objParametri_Utenti.FinestraTemporaleInizio)))

                    If Lav_Cod <> 0 Then
                        stbQuery.Append(" AND Operazioni.Lav_Cod = " & Agro_SQL_SaveNum(Lav_Cod))
                    End If
                    If P <> 0 Then
                        stbQuery.Append(" AND Operazioni.P = " & Agro_SQL_SaveNum(P))
                    End If
                    'If Gru_Cod <> 0 Then
                    '    stbQuery.Append(" AND GruppoOperazioni.Gru_Cod = " & agro_SQL_SaveNum(Gru_Cod))
                    'End If
                    'If Tipo <> "" Then
                    '    stbQuery.Append(" AND GruppoOperazioni.Tipo = '" & agro_SQL_SaveText(Tipo) & "' ")
                    'End If
                    'If Att_Cod <> 0 Then
                    '    stbQuery.Append(" AND GruppoOperazioni.Att_Cod = " & agro_SQL_SaveNum(Att_Cod))
                    'End If
                    If Cerca_LavDes <> "" Then
                        stbQuery.Append(" AND Operazioni.Lav_Des LIKE '%" & Agro_SQL_SaveText(Cerca_LavDes) & "%' ")
                    End If


                    If xFiltroAggiuntivo <> "" Then
                        stbQuery.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri_Utenti))
                    End If
                    If xOrderBy <> "" Then
                        stbQuery.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri_Utenti))
                    Else
                        stbQuery.Append(" ORDER BY GruppoOperazioni.GRU_DES, Operazioni.LAV_DES ")
                    End If

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta
                    '
                    '
                    '
                    '
            End Select



            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri_Utenti, stbQuery.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Utenti, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function



End Class
