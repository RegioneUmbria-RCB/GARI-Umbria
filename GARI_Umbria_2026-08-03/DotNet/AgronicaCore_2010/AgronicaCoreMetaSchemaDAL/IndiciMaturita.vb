Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate

Public Class IndiciMaturita_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function Leggi(ByVal IND_MAT_COD As Long,
                          ByVal xSelezioneVariabile As enumSelezioneVariabile,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreParametri
                          ) As DataTable

        Const nomeRoutine = "AgronicaCoreMetaschemaDAL.IndiciMaturita_r.Leggi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim dt As DataTable

        Try
            '---------------------------------------------
            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT IND_MAT_COD, IND_MAT_DES  ")
                    StrSQL.Append(" FROM  IndiciMaturita ")
                    StrSQL.Append(" WHERE Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND   Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    If IND_MAT_COD <> 0 Then
                        StrSQL.Append(" AND IndiciMaturita.IND_MAT_COD =  " & Agro_SQL_SaveNum(IND_MAT_COD) & "  ")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND     IndiciMaturita.Inviato >= 0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND     IndiciMaturita.Inviato = -1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY IND_MAT_DES ASC ")
                    End If


                Case enumSelezioneVariabile.Selezione_TabellaCompleta

                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT * ")
                    StrSQL.Append(" FROM  IndiciMaturita ")
                    StrSQL.Append(" WHERE IndiciMaturita.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND   IndiciMaturita.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    If IND_MAT_COD <> 0 Then
                        StrSQL.Append(" AND IndiciMaturita.IND_MAT_COD =  " & Agro_SQL_SaveNum(IND_MAT_COD) & "  ")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND     IndiciMaturita.Inviato >= 0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND     IndiciMaturita.Inviato = -1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY IndiciMaturita.IND_MAT_DES ASC ")
                    End If

            End Select

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    '##############################################################################################
    Public Function IndMatDes_from_IndMatCod(ByVal IndMatCod As Integer,
                                             ByRef objParametri As AgronicaCoreParametri
                                             ) As String

        Dim dt As DataTable

        'Recupero le informazioni
        dt = Leggi(CInt(IndMatCod),
                   enumSelezioneVariabile.Selezione_TabellaCompleta,
                   "", "", objParametri)

        'Se il recordset non è chiuso allora ...
        If dt.Rows.Count > 0 Then
            Return dt.Rows(0).Item("Ind_Mat_Des")
        End If

    End Function

    '##############################################################################################
    Public Function Leggi_WS(ByVal VEG_COD As Int32,
                             ByVal IND_MAT_COD As Int32,
                             ByVal UDM_COD As Int32,
                             ByVal Tipo_Testata As Integer,
                             ByVal xFiltroAggiuntivo As String,
                             ByVal xOrderBy As String,
                             ByRef objParametri As AgronicaCoreParametri,
                             Optional ByVal Personalizzate As Boolean = False,
                             Optional ByVal Piva_SuperUser As String = "",
                             Optional ByVal LAV_COD As Int32 = 0,
                             Optional ByVal NoSpecie As Boolean = False,
                             Optional ByVal FiltraSpecie As Boolean = True,
                             Optional joinPersonalizzate As Boolean = True,
                             Optional ByVal estraiPersonalizzatePerAPP As Boolean = False
                             ) As DataTable

        Const nomeRoutine = "AgronicaCoreMetaSchemaDAL.IndiciMaturita_R.Leggi_WS()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim dt As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.AppendLine(" SELECT  ")
            StrSQL.AppendLine("    imxsp.IND_MAT_COD, imxsp.VEG_COD, imxsp.REG_COD, ISNULL(imxsp.classe,'') as classe, ISNULL(imxsp.Flag_Raccolta,0) as Flag_Raccolta, ")
            StrSQL.AppendLine("    sp.Veg_Des, ISNULL(sp.Gru_Cod,0) as Gru_Cod, ISNULL(sp.Grsp_Cod,0) as Grsp_Cod, im.IND_MAT_DES, mxim.UDM_COD, um.UDM_DES, um.UDM_SIM, ")
            StrSQL.AppendLine("    ISNULL(im.lav_cod,0) as lav_cod ")

            If joinPersonalizzate Then
                If estraiPersonalizzatePerAPP Then
                    StrSQL.AppendLine(" , sop.Piva_SuperUser ")
                End If
            End If

            StrSQL.AppendLine(" FROM  IndiciMaturitaxSpecieVegetali AS imxsp ")
            StrSQL.AppendLine(" INNER JOIN SpecieVegetali sp ")
            StrSQL.AppendLine("    ON imxsp.VEG_COD = sp.VEG_COD ")
            StrSQL.AppendLine(" INNER JOIN IndiciMaturita im ")
            StrSQL.AppendLine("    ON imxsp.IND_MAT_COD = im.IND_MAT_COD ")
            StrSQL.AppendLine(" INNER JOIN MisuraxIndiciMaturita mxim ")
            StrSQL.AppendLine("    ON mxim.IND_MAT_COD = im.IND_MAT_COD ")
            StrSQL.AppendLine(" INNER JOIN UnitaMisura um ")
            StrSQL.AppendLine("    ON mxim.UDM_COD = um.UDM_COD ")

            If joinPersonalizzate Then
                If Personalizzate Then
                    StrSQL.AppendLine(" INNER JOIN SuperUser_OperazioniPersonalizzate AS sop ON ")
                    StrSQL.AppendLine("     sop.codice = im.IND_MAT_COD ")
                Else
                    StrSQL.AppendLine(" LEFT OUTER JOIN SuperUser_OperazioniPersonalizzate AS sop ON ")
                    StrSQL.AppendLine("     sop.codice = im.IND_MAT_COD ")
                End If
            End If

            StrSQL.AppendLine(" WHERE 1 = 1")
            StrSQL.AppendLine(" AND imxsp.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine))
            StrSQL.AppendLine(" AND imxsp.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio))
            StrSQL.AppendLine(" AND sp.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine))
            StrSQL.AppendLine(" AND sp.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio))
            StrSQL.AppendLine(" AND im.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine))
            StrSQL.AppendLine(" AND im.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio))
            StrSQL.AppendLine(" AND mxim.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine))
            StrSQL.AppendLine(" AND mxim.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio))
            StrSQL.AppendLine(" AND um.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine))
            StrSQL.AppendLine(" AND um.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio))

            If LAV_COD <> 0 Then
                StrSQL.AppendLine(" AND im.lav_cod = " & Agro_SQL_SaveNum(LAV_COD))
            End If

            If FiltraSpecie Then
                If NoSpecie Then
                    StrSQL.AppendLine(" AND imxsp.VEG_COD = 0")
                Else
                    If VEG_COD <> 0 Then
                        StrSQL.AppendLine(" AND sp.VEG_COD = " & Agro_SQL_SaveNum(VEG_COD))
                    Else
                        'Escludiamo sempre il veg_cod = 0
                        StrSQL.AppendLine(" AND imxsp.VEG_COD <> 0 ")
                    End If
                End If
            End If

            If IND_MAT_COD <> 0 Then
                StrSQL.Append(" AND im.ind_mat_Cod = " & Agro_SQL_SaveNum(IND_MAT_COD) & "  ")
            End If

            If UDM_COD <> 0 Then
                StrSQL.Append(" AND mxim.udm_cod = " & Agro_SQL_SaveNum(UDM_COD) & "  ")
            End If

            If joinPersonalizzate Then
                If Personalizzate Then
                    'Per l'estrazione da APP dobbiamo estrarre TUTTE le personalzzate, indipendentemente dall'superuser
                    If estraiPersonalizzatePerAPP = False Then
                        StrSQL.Append(" AND sop.Piva_SuperUser = '" & Agro_SQL_SaveText(Piva_SuperUser) & "'")
                    End If
                    Select Case Tipo_Testata
                        Case 0
                            StrSQL.Append(" AND sop.lav_cod = " & LAVCOD_RILIEVO_INDICI_MATURITA)
                        Case 1
                            StrSQL.Append(" AND sop.lav_cod = " & LAVCOD_RILIEVO_INDICI_RESE_RACCOLTA)
                        Case Else
                            Throw New Exception("tipoTestata non può valere '" & Tipo_Testata & "'")
                    End Select
                Else
                    'Se solo NON personalizzati (da nessuna PIVA SUPERUSER), eslcudiamo quelli personalizzati
                    StrSQL.Append(" AND sop.codice IS NULL ")
                End If
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY im.ind_mat_des ")
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

End Class
