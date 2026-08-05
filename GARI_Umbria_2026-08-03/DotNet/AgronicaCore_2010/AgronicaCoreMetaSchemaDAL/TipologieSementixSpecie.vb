Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.DataProviderExtensions
Imports AgronicaCoreDataProvider.TipiEnumerativi


Public Class TipologieSementixSpecie_R
    Inherits AgronicaCoreDataProvider.DataProvider


    '##############################################################################################
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Da usare con Selezione_JoinCompleta
    ''' </summary>
    ''' <param name="VEG_COD"></param>
    ''' <param name="SEM_COD"></param>
    ''' <param name="xSelezioneVariabile"></param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="xOrderBy"></param>
    ''' <param name="objParametri"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[pierantoni]	20/01/2011	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Function Leggi(ByVal VEG_COD As Integer,
                          ByVal SEM_COD As Integer,
                          ByVal xSelezioneVariabile As enumSelezioneVariabile,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreParametri
                          ) As DataTable

        Const nomeRoutine = "AgronicaCoreMetaSchemaDAL.TipologieSementixSpecie_R.Leggi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try

            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                Case enumSelezioneVariabile.Selezione_TabellaCompleta

                Case enumSelezioneVariabile.Selezione_JoinDescrizioni

                Case enumSelezioneVariabile.Selezione_JoinCompleta
                    StrSQL.Length = 0

                    StrSQL.AppendLine(" SELECT * ")
                    StrSQL.AppendLine(" FROM  TipologieSementixSpecieVegetali , SpecieVegetali , TipologieSementi ")
                    StrSQL.AppendLine(" WHERE TipologieSementixSpecieVegetali.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.AppendLine(" AND   TipologieSementixSpecieVegetali.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.AppendLine(" AND   SpecieVegetali.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.AppendLine(" AND   SpecieVegetali.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.AppendLine(" AND   TipologieSementi.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.AppendLine(" AND   TipologieSementi.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.AppendLine(" AND   TipologieSementixSpecieVegetali.VEG_COD = SpecieVegetali.VEG_COD ")
                    StrSQL.AppendLine(" AND   TipologieSementixSpecieVegetali.SEM_COD = TipologieSementi.SEM_COD ")

                    If VEG_COD <> 0 Then
                        StrSQL.AppendLine(" AND TipologieSementixSpecieVegetali.VEG_COD =  " & Agro_SQL_SaveNum(VEG_COD) & "  ")
                    End If

                    If SEM_COD <> 0 Then
                        StrSQL.AppendLine(" AND TipologieSementixSpecieVegetali.SEM_COD =  " & Agro_SQL_SaveNum(SEM_COD) & "  ")
                    End If


                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If

                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.AppendLine(" AND     dbo.TipologieSementixSpecieVegetali.Inviato >= 0 ")
                            StrSQL.AppendLine(" AND     dbo.SpecieVegetali.Inviato >= 0 ")
                            StrSQL.AppendLine(" AND     dbo.TipologieSementi.Inviato >= 0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.AppendLine(" AND     dbo.TipologieSementixSpecieVegetali.Inviato = -1 ")
                            StrSQL.AppendLine(" AND     dbo.SpecieVegetali.Inviato = -1 ")
                            StrSQL.AppendLine(" AND     dbo.TipologieSementi.Inviato = -1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------

                    If xOrderBy <> "" Then
                        StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.AppendLine(" ORDER BY SpecieVegetali.VEG_DES ASC , TipologieSementixSpecieVegetali.SEM_COD ASC ")
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
    Public Function Leggi2JoinSpecieColtivate(ByVal Piva As String,
                                              ByVal VEG_COD As Integer,
                                              ByVal SEM_COD As Integer,
                                              ByVal xFiltroAggiuntivo As String,
                                              ByVal xOrderBy As String,
                                              ByRef objParametri As AgronicaCoreParametri
                                              ) As DataTable

        Const nomeRoutine = "AgronicaCoreMetaSchemaDAL.TipologieSementixSpecie_R.Leggi2JoinSpecieColtivate()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT DISTINCT TipologieSementi.Sem_Cod, TipologieSementi.Sem_Des, SpecieVegetali.Veg_cod, SpecieVegetali.Veg_Des  ")
            StrSQL.AppendLine(" FROM  TipologieSementixSpecieVegetali ")
            StrSQL.AppendLine(" INNER JOIN SpecieVegetali ON TipologieSementixSpecieVegetali.VEG_COD = SpecieVegetali.VEG_COD ")
            StrSQL.AppendLine(" INNER JOIN TipologieSementi ON TipologieSementixSpecieVegetali.SEM_COD = TipologieSementi.SEM_COD  ")
            StrSQL.AppendLine(" INNER JOIN Cultivar ON Cultivar.Veg_Cod = SpecieVegetali.Veg_Cod ")
            StrSQL.AppendLine(" INNER JOIN Reg_Impianti ON Cultivar.Cul_Cod = Reg_Impianti.Cul_Cod ")

            StrSQL.AppendLine(" WHERE TipologieSementixSpecieVegetali.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.AppendLine(" AND   TipologieSementixSpecieVegetali.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
            StrSQL.AppendLine(" AND   SpecieVegetali.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.AppendLine(" AND   SpecieVegetali.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
            StrSQL.AppendLine(" AND   TipologieSementi.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.AppendLine(" AND   TipologieSementi.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            If Piva <> "" Then
                StrSQL.AppendLine(" AND     Reg_Impianti.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If

            If VEG_COD <> 0 Then
                StrSQL.AppendLine(" AND TipologieSementixSpecieVegetali.VEG_COD =  " & Agro_SQL_SaveNum(VEG_COD) & "  ")
            End If

            If SEM_COD <> 0 Then
                StrSQL.AppendLine(" AND TipologieSementixSpecieVegetali.SEM_COD =  " & Agro_SQL_SaveNum(SEM_COD) & "  ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine(" ORDER BY SpecieVegetali.VEG_DES, TipologieSementi.sem_des ")
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


    '##############################################################################################
    'cultivar c'è 2 volte, la prima come tramite da reg_impianti a specie
    'la seconda per ottenere tutte le varietà della specie
    Public Function Leggi3JoinSpecieColtivateVarietaTutte(ByVal Piva As String,
                                             ByVal CUL_COD As Integer,
                                             ByVal VEG_COD As Integer,
                                                ByVal SEM_COD As Integer,
                                                ByVal xFiltroAggiuntivo As String,
                                                ByVal xOrderBy As String,
                                                ByRef objParametri As AgronicaCoreParametri
                                                ) As DataTable

        Const nomeRoutine = "AgronicaCoreMetaSchemaDAL.TipologieSementixSpecie_R.Leggi3JoinSpecieColtivateVarietaTutte()"


        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT DISTINCT TipologieSementi.Sem_Cod, TipologieSementi.Sem_Des, SpecieVegetali.Veg_cod, SpecieVegetali.Veg_Des, cultivar2.cul_Cod, cultivar2.cul_Des  ")
            StrSQL.AppendLine(" FROM  TipologieSementixSpecieVegetali ")
            StrSQL.AppendLine(" INNER JOIN SpecieVegetali ON TipologieSementixSpecieVegetali.VEG_COD = SpecieVegetali.VEG_COD ")
            StrSQL.AppendLine(" INNER JOIN TipologieSementi ON TipologieSementixSpecieVegetali.SEM_COD = TipologieSementi.SEM_COD  ")
            StrSQL.AppendLine(" INNER JOIN Cultivar ON Cultivar.Veg_Cod = SpecieVegetali.Veg_Cod ")
            StrSQL.AppendLine(" INNER JOIN Reg_Impianti ON Cultivar.Cul_Cod = Reg_Impianti.Cul_Cod ")
            StrSQL.AppendLine(" INNER JOIN Cultivar cultivar2 ON cultivar2.VEG_COD = SpecieVegetali.VEG_COD ")

            StrSQL.AppendLine(" WHERE TipologieSementixSpecieVegetali.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.AppendLine(" AND   TipologieSementixSpecieVegetali.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
            StrSQL.AppendLine(" AND   SpecieVegetali.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.AppendLine(" AND   SpecieVegetali.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
            StrSQL.AppendLine(" AND   TipologieSementi.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.AppendLine(" AND   TipologieSementi.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            If Piva <> "" Then
                StrSQL.AppendLine(" AND     Reg_Impianti.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If

            If CUL_COD <> 0 Then
                StrSQL.AppendLine(" AND cultivar2.CUL_COD =  " & Agro_SQL_SaveNum(CUL_COD) & "  ")
            End If

            If VEG_COD <> 0 Then
                StrSQL.AppendLine(" AND TipologieSementixSpecieVegetali.VEG_COD =  " & Agro_SQL_SaveNum(VEG_COD) & "  ")
            End If

            If SEM_COD <> 0 Then
                StrSQL.AppendLine(" AND TipologieSementixSpecieVegetali.SEM_COD =  " & Agro_SQL_SaveNum(SEM_COD) & "  ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine(" ORDER BY SpecieVegetali.VEG_DES, Cultivar2.Cul_Des, TipologieSementi.sem_des ")
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


    '##############################################################################################
    'cultivar c'è 2 volte, la prima come tramite da reg_impianti a specie
    'la seconda per ottenere tutte le varietà della specie
    Public Function Leggi3JoinSpecieColtivateVarietaTutte_NON_su_MateriePrime(ByVal Piva As String,
                                                                              ByVal CUL_COD As Integer,
                                                                              ByVal VEG_COD As Integer,
                                                                              ByVal SEM_COD As Integer,
                                                                              ByVal regolamento_COD As Integer,
                                                                              ByVal xFiltroAggiuntivo As String,
                                                                              ByVal xOrderBy As String,
                                                                              ByRef objParametri As AgronicaCoreParametri
                                                                              ) As DataTable


        Const nomeRoutine = "AgronicaCoreMetaSchemaDAL.TipologieSementixSpecie_R.Leggi3JoinSpecieColtivateVarietaTutte()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT DISTINCT TipologieSementi.Sem_Cod, TipologieSementi.Sem_Des, SpecieVegetali.Veg_cod, SpecieVegetali.Veg_Des, cultivar2.cul_Cod, cultivar2.cul_Des  ")
            StrSQL.AppendLine(" FROM  TipologieSementixSpecieVegetali ")
            StrSQL.AppendLine(" INNER JOIN SpecieVegetali ON TipologieSementixSpecieVegetali.VEG_COD = SpecieVegetali.VEG_COD ")
            StrSQL.AppendLine(" INNER JOIN TipologieSementi ON TipologieSementixSpecieVegetali.SEM_COD = TipologieSementi.SEM_COD  ")
            StrSQL.AppendLine(" INNER JOIN Cultivar ON Cultivar.Veg_Cod = SpecieVegetali.Veg_Cod ")
            StrSQL.AppendLine(" INNER JOIN Reg_Impianti ON Cultivar.Cul_Cod = Reg_Impianti.Cul_Cod ")
            StrSQL.AppendLine(" INNER JOIN Cultivar cultivar2 ON cultivar2.VEG_COD = SpecieVegetali.VEG_COD ")

            StrSQL.AppendLine(" WHERE TipologieSementixSpecieVegetali.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.AppendLine(" AND   TipologieSementixSpecieVegetali.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
            StrSQL.AppendLine(" AND   SpecieVegetali.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.AppendLine(" AND   SpecieVegetali.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
            StrSQL.AppendLine(" AND   TipologieSementi.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.AppendLine(" AND   TipologieSementi.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            'se c'è un regolamento selezionato
            If regolamento_COD <> 0 Then
                StrSQL.AppendLine("  AND NOT EXISTS (  ")
                StrSQL.AppendLine("                  SELECT 1 ")
                StrSQL.AppendLine("                 FROM Materie_Prime ")
                StrSQL.AppendLine("                  WHERE Materie_Prime.veg_cod = Cultivar.veg_cod ")
                StrSQL.AppendLine("                  AND Materie_Prime.cul_cod = Cultivar2.cul_cod ")
                StrSQL.AppendLine("                  AND Materie_Prime.SEM_cod = TipologieSementi.SEM_cod ")
                StrSQL.AppendLine("                  AND Materie_Prime.regolamento = " & Agro_SQL_SaveNum(regolamento_COD) & " ")
                StrSQL.AppendLine("                 AND Materie_Prime.Elem_Cod = " & Agro_SQL_SaveNum(SEMENTI) & " ")
                StrSQL.AppendLine("  )  ")
            Else
                'se devo cercare su regolamento conv e bio
                StrSQL.AppendLine("  AND (  ")
                StrSQL.AppendLine("         NOT EXISTS (  ")
                StrSQL.AppendLine("                  SELECT 1 ")
                StrSQL.AppendLine("                 FROM Materie_Prime ")
                StrSQL.AppendLine("                  WHERE Materie_Prime.veg_cod = Cultivar.veg_cod ")
                StrSQL.AppendLine("                  AND Materie_Prime.cul_cod = Cultivar2.cul_cod ")
                StrSQL.AppendLine("                  AND Materie_Prime.SEM_cod = TipologieSementi.SEM_cod ")
                StrSQL.AppendLine("                  AND Materie_Prime.regolamento = " & Agro_SQL_SaveNum(enum_Cod_Regolamento.Regolamento_Nessuno) & " ")
                StrSQL.AppendLine("                 AND Materie_Prime.Elem_Cod = " & Agro_SQL_SaveNum(SEMENTI) & " ")
                StrSQL.AppendLine("                 )  ")
                StrSQL.AppendLine("         OR   ")
                StrSQL.AppendLine("         NOT EXISTS (  ")
                StrSQL.AppendLine("                  SELECT 1 ")
                StrSQL.AppendLine("                 FROM Materie_Prime ")
                StrSQL.AppendLine("                  WHERE Materie_Prime.veg_cod = Cultivar.veg_cod ")
                StrSQL.AppendLine("                  AND Materie_Prime.cul_cod = Cultivar2.cul_cod ")
                StrSQL.AppendLine("                  AND Materie_Prime.SEM_cod = TipologieSementi.SEM_cod ")
                StrSQL.AppendLine("                  AND Materie_Prime.regolamento = " & Agro_SQL_SaveNum(enum_Cod_Regolamento.Regolamento_bio) & " ")
                StrSQL.AppendLine("                 AND Materie_Prime.Elem_Cod = " & CStr(SEMENTI) & " ")
                StrSQL.AppendLine("                 )  ")
                StrSQL.AppendLine("  )  ")
            End If

            If Piva <> "" Then
                StrSQL.AppendLine(" AND     Reg_Impianti.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If

            If CUL_COD <> 0 Then
                StrSQL.AppendLine(" AND cultivar2.CUL_COD =  " & Agro_SQL_SaveNum(CUL_COD) & "  ")
            End If

            If VEG_COD <> 0 Then
                StrSQL.AppendLine(" AND TipologieSementixSpecieVegetali.VEG_COD =  " & Agro_SQL_SaveNum(VEG_COD) & "  ")
            End If

            If SEM_COD <> 0 Then
                StrSQL.AppendLine(" AND TipologieSementixSpecieVegetali.SEM_COD =  " & Agro_SQL_SaveNum(SEM_COD) & "  ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine(" ORDER BY SpecieVegetali.VEG_DES, Cultivar2.Cul_Des, TipologieSementi.sem_des ")
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


    '##############################################################################################
    Public Function Leggi3JoinVarietaColtivate(ByVal Piva As String,
                                               ByVal CUL_COD As Integer,
                                               ByVal VEG_COD As Integer,
                                               ByVal SEM_COD As Integer,
                                               ByVal xFiltroAggiuntivo As String,
                                               ByVal xOrderBy As String,
                                               ByRef objParametri As AgronicaCoreParametri
                                               ) As DataTable

        Const nomeRoutine = "AgronicaCoreMetaSchemaDAL.TipologieSementixSpecie_R.Leggi3JoinVarietaColtivate()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT DISTINCT TipologieSementi.Sem_Cod, TipologieSementi.Sem_Des, SpecieVegetali.Veg_cod, SpecieVegetali.Veg_Des, Cultivar.cul_Cod, Cultivar.cul_Des  ")
            StrSQL.AppendLine(" FROM  TipologieSementixSpecieVegetali ")
            StrSQL.AppendLine(" INNER JOIN SpecieVegetali ON TipologieSementixSpecieVegetali.VEG_COD = SpecieVegetali.VEG_COD ")
            StrSQL.AppendLine(" INNER JOIN TipologieSementi ON TipologieSementixSpecieVegetali.SEM_COD = TipologieSementi.SEM_COD  ")
            StrSQL.AppendLine(" INNER JOIN Cultivar ON Cultivar.Veg_Cod = SpecieVegetali.Veg_Cod ")
            StrSQL.AppendLine(" INNER JOIN Reg_Impianti ON Cultivar.Cul_Cod = Reg_Impianti.Cul_Cod ")

            StrSQL.AppendLine(" WHERE TipologieSementixSpecieVegetali.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.AppendLine(" AND   TipologieSementixSpecieVegetali.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
            StrSQL.AppendLine(" AND   SpecieVegetali.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.AppendLine(" AND   SpecieVegetali.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
            StrSQL.AppendLine(" AND   TipologieSementi.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.AppendLine(" AND   TipologieSementi.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            If Piva <> "" Then
                StrSQL.AppendLine(" AND     Reg_Impianti.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If

            If CUL_COD <> 0 Then
                StrSQL.AppendLine(" AND Cultivar.CUL_COD =  " & Agro_SQL_SaveNum(CUL_COD) & "  ")
            End If

            If VEG_COD <> 0 Then
                StrSQL.AppendLine(" AND TipologieSementixSpecieVegetali.VEG_COD =  " & Agro_SQL_SaveNum(VEG_COD) & "  ")
            End If

            If SEM_COD <> 0 Then
                StrSQL.AppendLine(" AND TipologieSementixSpecieVegetali.SEM_COD =  " & Agro_SQL_SaveNum(SEM_COD) & "  ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine(" ORDER BY SpecieVegetali.VEG_DES, Cultivar.Cul_Des, TipologieSementi.sem_des ")
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

    '##############################################################################################
    'Select mirata
    Public Function Leggi2(ByVal VEG_COD As Integer,
                           ByVal SEM_COD As Integer,
                           ByVal xFiltroAggiuntivo As String,
                           ByVal xOrderBy As String,
                           ByRef objParametri As AgronicaCoreParametri
                           ) As DataTable

        Const nomeRoutine = "AgronicaCoreMetaSchemaDAL.TipologieSementixSpecie_R.Leggi2()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT TipologieSementi.Sem_Cod, TipologieSementi.Sem_Des, SpecieVegetali.Veg_cod, SpecieVegetali.Veg_Des  ")
            StrSQL.AppendLine(" FROM  TipologieSementixSpecieVegetali ")
            StrSQL.AppendLine(" INNER JOIN SpecieVegetali ON TipologieSementixSpecieVegetali.VEG_COD = SpecieVegetali.VEG_COD ")
            StrSQL.AppendLine(" INNER JOIN TipologieSementi ON TipologieSementixSpecieVegetali.SEM_COD = TipologieSementi.SEM_COD  ")

            StrSQL.AppendLine(" WHERE TipologieSementixSpecieVegetali.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.AppendLine(" AND   TipologieSementixSpecieVegetali.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
            StrSQL.AppendLine(" AND   SpecieVegetali.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.AppendLine(" AND   SpecieVegetali.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
            StrSQL.AppendLine(" AND   TipologieSementi.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.AppendLine(" AND   TipologieSementi.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            If VEG_COD <> 0 Then
                StrSQL.AppendLine(" AND TipologieSementixSpecieVegetali.VEG_COD =  " & Agro_SQL_SaveNum(VEG_COD) & "  ")
            End If

            If SEM_COD <> 0 Then
                StrSQL.AppendLine(" AND TipologieSementixSpecieVegetali.SEM_COD =  " & Agro_SQL_SaveNum(SEM_COD) & "  ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine(" ORDER BY SpecieVegetali.VEG_DES, TipologieSementi.sem_des ")
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

    '##############################################################################################
    Public Function Leggi3(ByVal CUL_COD As Integer,
                           ByVal VEG_COD As Integer,
                           ByVal SEM_COD As Integer,
                           ByVal xFiltroAggiuntivo As String,
                           ByVal xOrderBy As String,
                           ByRef objParametri As AgronicaCoreParametri
                           ) As DataTable

        Const nomeRoutine = "AgronicaCoreMetaSchemaDAL.TipologieSementixSpecie_R.Leggi3()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT TipologieSementi.Sem_Cod, TipologieSementi.Sem_Des, SpecieVegetali.Veg_cod, SpecieVegetali.Veg_Des, Cultivar.cul_cod, Cultivar.cul_des  ")
            StrSQL.AppendLine(" FROM  TipologieSementixSpecieVegetali ")
            StrSQL.AppendLine(" INNER JOIN SpecieVegetali ON TipologieSementixSpecieVegetali.VEG_COD = SpecieVegetali.VEG_COD ")
            StrSQL.AppendLine(" INNER JOIN Cultivar ON Cultivar.VEG_COD = SpecieVegetali.VEG_COD ")
            StrSQL.AppendLine(" INNER JOIN TipologieSementi ON TipologieSementixSpecieVegetali.SEM_COD = TipologieSementi.SEM_COD  ")

            StrSQL.AppendLine(" WHERE TipologieSementixSpecieVegetali.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.AppendLine(" AND   TipologieSementixSpecieVegetali.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
            StrSQL.AppendLine(" AND   SpecieVegetali.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.AppendLine(" AND   SpecieVegetali.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
            StrSQL.AppendLine(" AND   TipologieSementi.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.AppendLine(" AND   TipologieSementi.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
            StrSQL.AppendLine(" AND   Cultivar.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.AppendLine(" AND   Cultivar.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            If CUL_COD <> 0 Then
                StrSQL.AppendLine(" AND Cultivar.CUL_COD =  " & Agro_SQL_SaveNum(CUL_COD) & "  ")
            End If

            If VEG_COD <> 0 Then
                StrSQL.AppendLine(" AND TipologieSementixSpecieVegetali.VEG_COD =  " & Agro_SQL_SaveNum(VEG_COD) & "  ")
            End If

            If SEM_COD <> 0 Then
                StrSQL.AppendLine(" AND TipologieSementixSpecieVegetali.SEM_COD =  " & Agro_SQL_SaveNum(SEM_COD) & "  ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine(" ORDER BY SpecieVegetali.VEG_DES, Cultivar.Cul_Des, TipologieSementi.sem_des ")
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

    '################################################################################
    Public Function SemCod_from_VegCod(ByVal Veg_Cod As Integer,
                                       ByVal xFiltroAggiuntivo As String,
                                       ByVal xOrderBy As String,
                                       ByRef objParametri As AgronicaCoreParametri
                                       ) As Integer

        Dim dt As DataTable

        dt = Leggi2(Veg_Cod, 0,
                    xFiltroAggiuntivo, xOrderBy, objParametri)

        If Not IsNothing(dt) Then
            If dt.Rows.Count <> 0 Then
                Return dt.Rows(0).Item("Sem_Cod")
            Else
                Return 0
            End If
        Else
            Return 0
        End If

    End Function

    '###########################
    'sul 2003 era NewCom_TipologieSementixSpecieVegetali_GestioneFiltroUtente_Leggi
    'se occorre fare la caricacombo, copia dal core ws: CaricaCombo_SpecieVegetale_Semente
    Public Function TipologieSementixSpecieVegetali_GestioneFiltroUtente_Leggi(
                                   ByVal SEM_COD As Integer,
                                   ByVal VEG_COD As Integer,
                                   ByVal xFiltroAggiuntivo As String,
                                   ByVal xOrderBy As String,
                                   ByRef objParametri_server As AgronicaCoreParametri,
                                   ByRef objParametri_utenti As AgronicaCoreParametri,
                                    Optional ByVal FinestraTemp_Inizio As String = "01/01/1900",
                                   Optional ByVal FinestraTemp_Fine As String = "31/12/2100"
                                   ) As DataTable

        Const nomeRoutine = "AgronicaCoreMetaSchemaDAL.TipologieSementixSpecie_R.TipologieSementixSpecieVegetali_GestioneFiltroUtente_Leggi()"

        Dim messaggioErrore As String = ""
        Dim stbQuery As New Text.StringBuilder
        Dim dt As DataTable

        Try

            Dim Flag_EseguiQueryConGruCod As Boolean = False
            Dim Flag_EseguiQueryTutti As Boolean = False
            Dim Flag_EseguiQueryUnionAll As Boolean = False

            Dim Gru_Cod As Integer = 0
            Dim i As Integer
            Dim Vet_GruCod As Integer()


            '--------------------------------------------------------------
            '-------------- GRUPPO VEGETALE NON VALORIZZATO ---------------
            '               CASO: sementi e materiali vivaisti
            '--------------------------------------------------------------

            'caso nessun gruppo selezionato: voglio vedere tutte le specie dell'archivio

            'else: 
            '       un solo gruppo selezionato -> faccio la query con il gru_cod valorizzato
            '       più di un gruppo selezionato (ma non tutti, quindi due): faccio la query per ogni gru_cod e unifico il risultato

            Dim DT_GruCod As DataTable
            Dim objUtenti As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_FiltroMono_R

            DT_GruCod = objUtenti.Leggi(enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_GRUPPI_VEGETALI,
                                        0,
                                        enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                        "", "",
                                        objParametri_utenti)


            If Not IsNothing(DT_GruCod) Then

                Select Case DT_GruCod.Rows.Count

                    Case 0
                        'non ho specificato nessun gruppo vegetale, quindi faccio la query normale
                        Flag_EseguiQueryTutti = True

                    Case 1
                        'ho specificato un gruppo, faccio la query IF ELSE
                        Gru_Cod = CInt(DT_GruCod.Rows(0).Item("ID_0"))
                        Flag_EseguiQueryConGruCod = True

                    Case Else

                        'Esempio: nel filtro ho selezionato erbacee con 4 specie vegetali
                        '                   e ho selezionato orticole senza specificare le specie (quindi le voglio tutte)

                        'visto che il gru_cod non è passato, per evitare che nel menù vengano caricate solo le specie delle erbacee,
                        'faccio la query per ogni gru_cod e unifico il risultato

                        Dim numGruppi As Integer = DT_GruCod.Rows.Count

                        ReDim Vet_GruCod(DT_GruCod.Rows.Count - 1)

                        For i = 0 To DT_GruCod.Rows.Count - 1

                            'mi salvo i gru_cod selezionati
                            Vet_GruCod(i) = DT_GruCod.Rows(i).Item("Id_0")

                        Next

                        Flag_EseguiQueryUnionAll = True

                End Select

            Else
                Flag_EseguiQueryTutti = True
            End If

            '####################################

            If Flag_EseguiQueryConGruCod Then


                'Genero la query SQL
                stbQuery.Length = 0

                stbQuery.AppendLine(" IF (  ")
                stbQuery.AppendLine(" SELECT COUNT(*)  ")
                stbQuery.AppendLine(" FROM          SpecieVegetali ")
                stbQuery.AppendLine(" INNER JOIN  TipologieSementixSpecieVegetali  ")
                stbQuery.AppendLine("               ON TipologieSementixSpecieVegetali.VEG_COD = SpecieVegetali.Veg_Cod  ")
                stbQuery.AppendLine(" INNER JOIN    Utenti_Impostazioni_FiltroMono ")
                stbQuery.AppendLine("               ON SpecieVegetali.veg_cod = Utenti_Impostazioni_FiltroMono.ID_0 ")
                stbQuery.AppendLine(" AND           Utenti_Impostazioni_FiltroMono.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri_utenti.PivaSuperUser) & "'  ")
                stbQuery.AppendLine(" AND           Utenti_Impostazioni_FiltroMono.UserName = '" & Agro_SQL_SaveText(objParametri_utenti.UtenteUsername) & "'  ")
                stbQuery.AppendLine(" AND           Utenti_Impostazioni_FiltroMono.Impostazione_Cod = " & Agro_SQL_SaveNum(enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_SPECIE_VEGETALI) & " ")
                stbQuery.AppendLine(" AND           SpecieVegetali.gru_Cod = " & Agro_SQL_SaveNum(Gru_Cod) & " ")
                'If Sem_Cod <> 0 Then
                '    stbQuery.AppendLine(" AND       TipologieSementixSpecieVegetali.SEM_COD = " & SQL_SaveNum(Sem_Cod))
                'End If
                'If Veg_Cod <> 0 Then
                '    stbQuery.AppendLine(" AND       TipologieSementixSpecieVegetali.Veg_COD = " & SQL_SaveNum(Veg_Cod))
                'End If
                stbQuery.AppendLine("   ) > 0 ")


                stbQuery.AppendLine(" SELECT DISTINCT TipologieSementixSpecieVegetali.SEM_COD, TipologieSementixSpecieVegetali.VEG_COD, ")
                stbQuery.AppendLine("       ISNULL(TipologieSementi.SEM_DES,'') AS Sem_Des, ISNULL(SpecieVegetali.Veg_Des,'') AS Veg_Des,  ")
                stbQuery.AppendLine("       ISNULL(SpecieVegetali.Grsp_Cod,0) AS Grsp_Cod, ISNULL(SpecieVegetali.Gru_Cod,-1) AS Gru_Cod, ")
                stbQuery.AppendLine("       ISNULL(TipologieSementi.Coeff_ener,0) AS Coeff_ener, ISNULL(TipologieSementi.Coeff_azoto,0) AS Coeff_azoto, ISNULL(TipologieSementi.Coeff_fosforo,0) AS Coeff_fosforo ")
                stbQuery.AppendLine(" FROM  TipologieSementixSpecieVegetali INNER JOIN ")
                stbQuery.AppendLine("       TipologieSementi ON TipologieSementixSpecieVegetali.SEM_COD = TipologieSementi.SEM_COD INNER JOIN ")
                stbQuery.AppendLine("       SpecieVegetali ON TipologieSementixSpecieVegetali.VEG_COD = SpecieVegetali.Veg_Cod ")
                stbQuery.AppendLine(" INNER JOIN Utenti_Impostazioni_FiltroMono ")
                stbQuery.AppendLine("       ON SpecieVegetali.veg_cod = Utenti_Impostazioni_FiltroMono.ID_0 ")
                stbQuery.AppendLine(" WHERE SpecieVegetali.Validita_Inizio <=" & Agro_SQL_SaveDate(CDate(FinestraTemp_Fine)) & " ")
                stbQuery.AppendLine(" AND   SpecieVegetali.Validita_Fine >=" & Agro_SQL_SaveDate(CDate(FinestraTemp_Inizio)) & " ")
                stbQuery.AppendLine(" AND   Utenti_Impostazioni_FiltroMono.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri_utenti.PivaSuperUser) & "'  ")
                stbQuery.AppendLine(" AND   Utenti_Impostazioni_FiltroMono.UserName = '" & Agro_SQL_SaveText(objParametri_utenti.UtenteUsername) & "'  ")
                stbQuery.AppendLine(" AND   Utenti_Impostazioni_FiltroMono.Impostazione_Cod = " & Agro_SQL_SaveNum(enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_SPECIE_VEGETALI) & " ")
                stbQuery.AppendLine(" AND   TipologieSementixSpecieVegetali.Validita_Inizio <=" & Agro_SQL_SaveDate(CDate(FinestraTemp_Fine)) & " ")
                stbQuery.AppendLine(" AND   TipologieSementixSpecieVegetali.Validita_Fine >=" & Agro_SQL_SaveDate(CDate(FinestraTemp_Inizio)) & " ")
                stbQuery.AppendLine(" AND   SpecieVegetali.gru_Cod = " & Agro_SQL_SaveNum(Gru_Cod) & " ")
                If SEM_COD <> 0 Then
                    stbQuery.AppendLine(" AND TipologieSementixSpecieVegetali.SEM_COD = " & Agro_SQL_SaveNum(SEM_COD))
                End If
                If VEG_COD <> 0 Then
                    stbQuery.AppendLine(" AND TipologieSementixSpecieVegetali.Veg_COD = " & Agro_SQL_SaveNum(VEG_COD))
                End If
                If xFiltroAggiuntivo <> "" Then
                    stbQuery.AppendLine(Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri_server))
                End If
                If xOrderBy <> "" Then
                    stbQuery.AppendLine(Agro_SQL_Save_xOrderBy(xOrderBy, objParametri_server))
                Else
                    stbQuery.AppendLine(" ORDER BY Sem_Des, Veg_Des ")
                End If


                stbQuery.AppendLine("ELSE ")


                stbQuery.AppendLine(" SELECT DISTINCT TipologieSementixSpecieVegetali.SEM_COD, TipologieSementixSpecieVegetali.VEG_COD, ")
                stbQuery.AppendLine("       ISNULL(TipologieSementi.SEM_DES,'') AS Sem_Des, ISNULL(SpecieVegetali.Veg_Des,'') AS Veg_Des,  ")
                stbQuery.AppendLine("       ISNULL(SpecieVegetali.Grsp_Cod,0) AS Grsp_Cod, ISNULL(SpecieVegetali.Gru_Cod,-1) AS Gru_Cod, ")
                stbQuery.AppendLine("       ISNULL(TipologieSementi.Coeff_ener,0) AS Coeff_ener, ISNULL(TipologieSementi.Coeff_azoto,0) AS Coeff_azoto, ISNULL(TipologieSementi.Coeff_fosforo,0) AS Coeff_fosforo ")
                stbQuery.AppendLine(" FROM  TipologieSementixSpecieVegetali INNER JOIN ")
                stbQuery.AppendLine("       TipologieSementi ON TipologieSementixSpecieVegetali.SEM_COD = TipologieSementi.SEM_COD INNER JOIN ")
                stbQuery.AppendLine("       SpecieVegetali ON TipologieSementixSpecieVegetali.VEG_COD = SpecieVegetali.Veg_Cod ")
                stbQuery.AppendLine(" WHERE SpecieVegetali.Validita_Inizio <=" & Agro_SQL_SaveDate(CDate(FinestraTemp_Fine)) & " ")
                stbQuery.AppendLine(" AND   SpecieVegetali.Validita_Fine >=" & Agro_SQL_SaveDate(CDate(FinestraTemp_Inizio)) & " ")
                stbQuery.AppendLine(" AND   TipologieSementixSpecieVegetali.Validita_Inizio <=" & Agro_SQL_SaveDate(CDate(FinestraTemp_Fine)) & " ")
                stbQuery.AppendLine(" AND   TipologieSementixSpecieVegetali.Validita_Fine >=" & Agro_SQL_SaveDate(CDate(FinestraTemp_Inizio)) & " ")
                stbQuery.AppendLine(" AND   SpecieVegetali.gru_Cod = " & Agro_SQL_SaveNum(Gru_Cod) & " ")
                If SEM_COD <> 0 Then
                    stbQuery.AppendLine(" AND TipologieSementixSpecieVegetali.SEM_COD = " & Agro_SQL_SaveNum(SEM_COD))
                End If
                If VEG_COD <> 0 Then
                    stbQuery.AppendLine(" AND TipologieSementixSpecieVegetali.Veg_COD = " & Agro_SQL_SaveNum(VEG_COD))
                End If
                If xFiltroAggiuntivo <> "" Then
                    stbQuery.AppendLine(Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri_server))
                End If
                If xOrderBy <> "" Then
                    stbQuery.AppendLine(Agro_SQL_Save_xOrderBy(xOrderBy, objParametri_server))
                Else
                    stbQuery.AppendLine(" ORDER BY Sem_Des, Veg_Des ")
                End If


            End If

            '####################################

            If Flag_EseguiQueryTutti Then

                'nessun filtro, faccio vedere tutte le specie

                'Genero la query SQL
                stbQuery.Length = 0

                stbQuery.AppendLine(" SELECT DISTINCT TipologieSementixSpecieVegetali.SEM_COD, TipologieSementixSpecieVegetali.VEG_COD, ")
                stbQuery.AppendLine("       ISNULL(TipologieSementi.SEM_DES,'') AS Sem_Des, ISNULL(SpecieVegetali.Veg_Des,'') AS Veg_Des,  ")
                stbQuery.AppendLine("       ISNULL(SpecieVegetali.Grsp_Cod,0) AS Grsp_Cod, ISNULL(SpecieVegetali.Gru_Cod,-1) AS Gru_Cod, ")
                stbQuery.AppendLine("       ISNULL(TipologieSementi.Coeff_ener,0) AS Coeff_ener, ISNULL(TipologieSementi.Coeff_azoto,0) AS Coeff_azoto, ISNULL(TipologieSementi.Coeff_fosforo,0) AS Coeff_fosforo ")
                stbQuery.AppendLine(" FROM  TipologieSementixSpecieVegetali INNER JOIN ")
                stbQuery.AppendLine("       TipologieSementi ON TipologieSementixSpecieVegetali.SEM_COD = TipologieSementi.SEM_COD INNER JOIN ")
                stbQuery.AppendLine("       SpecieVegetali ON TipologieSementixSpecieVegetali.VEG_COD = SpecieVegetali.Veg_Cod ")
                stbQuery.AppendLine(" WHERE SpecieVegetali.Validita_Inizio <=" & Agro_SQL_SaveDate(CDate(FinestraTemp_Fine)) & " ")
                stbQuery.AppendLine(" AND   SpecieVegetali.Validita_Fine >=" & Agro_SQL_SaveDate(CDate(FinestraTemp_Inizio)) & " ")
                stbQuery.AppendLine(" AND   TipologieSementixSpecieVegetali.Validita_Inizio <=" & Agro_SQL_SaveDate(CDate(FinestraTemp_Fine)) & " ")
                stbQuery.AppendLine(" AND   TipologieSementixSpecieVegetali.Validita_Fine >=" & Agro_SQL_SaveDate(CDate(FinestraTemp_Inizio)) & " ")
                If SEM_COD <> 0 Then
                    stbQuery.AppendLine(" AND TipologieSementixSpecieVegetali.SEM_COD = " & Agro_SQL_SaveNum(SEM_COD))
                End If
                If VEG_COD <> 0 Then
                    stbQuery.AppendLine(" AND TipologieSementixSpecieVegetali.Veg_COD = " & Agro_SQL_SaveNum(VEG_COD))
                End If
                If xFiltroAggiuntivo <> "" Then
                    stbQuery.AppendLine(Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri_server))
                End If
                If xOrderBy <> "" Then
                    stbQuery.AppendLine(Agro_SQL_Save_xOrderBy(xOrderBy, objParametri_server))
                Else
                    stbQuery.AppendLine(" ORDER BY Sem_Des, Veg_Des ")
                End If


            End If


            '####################################

            If Flag_EseguiQueryConGruCod OrElse Flag_EseguiQueryTutti Then

                'eseguo la query per uno dei due casi

                '--------------------------------------------------------------------------
                dt = EseguiQuery_Lettura(objParametri_utenti, stbQuery.ToString, nomeRoutine)
                '--------------------------------------------------------------------------

                Return dt


            End If


            '####################################

            If Flag_EseguiQueryUnionAll Then

                'caso di più gruppi selezionati

                Dim DT_Clonato As DataTable
                Dim j As Integer

                For i = 0 To Vet_GruCod.Length - 1

                    'faccio la query per ogni gru_cod
                    Gru_Cod = Vet_GruCod(i)

                    stbQuery.Length = 0

                    stbQuery.AppendLine(" IF (  ")
                    stbQuery.AppendLine(" SELECT COUNT(*)  ")
                    stbQuery.AppendLine(" FROM          SpecieVegetali ")
                    stbQuery.AppendLine(" INNER JOIN  TipologieSementixSpecieVegetali  ")
                    stbQuery.AppendLine("               ON TipologieSementixSpecieVegetali.VEG_COD = SpecieVegetali.Veg_Cod  ")
                    stbQuery.AppendLine(" INNER JOIN    Utenti_Impostazioni_FiltroMono ")
                    stbQuery.AppendLine("               ON SpecieVegetali.veg_cod = Utenti_Impostazioni_FiltroMono.ID_0 ")
                    stbQuery.AppendLine(" AND           Utenti_Impostazioni_FiltroMono.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri_utenti.PivaSuperUser) & "'  ")
                    stbQuery.AppendLine(" AND           Utenti_Impostazioni_FiltroMono.UserName = '" & Agro_SQL_SaveText(objParametri_utenti.UtenteUsername) & "'  ")
                    stbQuery.AppendLine(" AND           Utenti_Impostazioni_FiltroMono.Impostazione_Cod = " & Agro_SQL_SaveNum(enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_SPECIE_VEGETALI) & " ")
                    stbQuery.AppendLine(" AND           SpecieVegetali.gru_Cod = " & Agro_SQL_SaveNum(Gru_Cod) & " ")
                    'If Sem_Cod <> 0 Then
                    '    stbQuery.AppendLine(" AND       TipologieSementixSpecieVegetali.SEM_COD = " & SQL_SaveNum(Sem_Cod))
                    'End If
                    'If Veg_Cod <> 0 Then
                    '    stbQuery.AppendLine(" AND       TipologieSementixSpecieVegetali.Veg_COD = " & SQL_SaveNum(Veg_Cod))
                    'End If
                    stbQuery.AppendLine("   ) > 0 ")


                    stbQuery.AppendLine(" SELECT DISTINCT TipologieSementixSpecieVegetali.SEM_COD, TipologieSementixSpecieVegetali.VEG_COD, ")
                    stbQuery.AppendLine("       ISNULL(TipologieSementi.SEM_DES,'') AS Sem_Des, ISNULL(SpecieVegetali.Veg_Des,'') AS Veg_Des,  ")
                    stbQuery.AppendLine("       ISNULL(SpecieVegetali.Grsp_Cod,0) AS Grsp_Cod, ISNULL(SpecieVegetali.Gru_Cod,-1) AS Gru_Cod, ")
                    stbQuery.AppendLine("       ISNULL(TipologieSementi.Coeff_ener,0) AS Coeff_ener, ISNULL(TipologieSementi.Coeff_azoto,0) AS Coeff_azoto, ISNULL(TipologieSementi.Coeff_fosforo,0) AS Coeff_fosforo ")
                    stbQuery.AppendLine(" FROM  TipologieSementixSpecieVegetali INNER JOIN ")
                    stbQuery.AppendLine("       TipologieSementi ON TipologieSementixSpecieVegetali.SEM_COD = TipologieSementi.SEM_COD INNER JOIN ")
                    stbQuery.AppendLine("       SpecieVegetali ON TipologieSementixSpecieVegetali.VEG_COD = SpecieVegetali.Veg_Cod ")
                    stbQuery.AppendLine(" INNER JOIN Utenti_Impostazioni_FiltroMono ")
                    stbQuery.AppendLine("       ON SpecieVegetali.veg_cod = Utenti_Impostazioni_FiltroMono.ID_0 ")
                    stbQuery.AppendLine(" WHERE SpecieVegetali.Validita_Inizio <=" & Agro_SQL_SaveDate(CDate(FinestraTemp_Fine)) & " ")
                    stbQuery.AppendLine(" AND   SpecieVegetali.Validita_Fine >=" & Agro_SQL_SaveDate(CDate(FinestraTemp_Inizio)) & " ")
                    stbQuery.AppendLine(" AND   Utenti_Impostazioni_FiltroMono.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri_utenti.PivaSuperUser) & "'  ")
                    stbQuery.AppendLine(" AND   Utenti_Impostazioni_FiltroMono.UserName = '" & Agro_SQL_SaveText(objParametri_utenti.UtenteUsername) & "'  ")
                    stbQuery.AppendLine(" AND   Utenti_Impostazioni_FiltroMono.Impostazione_Cod = " & Agro_SQL_SaveNum(enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_SPECIE_VEGETALI) & " ")
                    stbQuery.AppendLine(" AND   TipologieSementixSpecieVegetali.Validita_Inizio <=" & Agro_SQL_SaveDate(CDate(FinestraTemp_Fine)) & " ")
                    stbQuery.AppendLine(" AND   TipologieSementixSpecieVegetali.Validita_Fine >=" & Agro_SQL_SaveDate(CDate(FinestraTemp_Inizio)) & " ")
                    stbQuery.AppendLine(" AND   SpecieVegetali.gru_Cod = " & Agro_SQL_SaveNum(Gru_Cod) & " ")
                    If SEM_COD <> 0 Then
                        stbQuery.AppendLine(" AND TipologieSementixSpecieVegetali.SEM_COD = " & Agro_SQL_SaveNum(SEM_COD))
                    End If
                    If VEG_COD <> 0 Then
                        stbQuery.AppendLine(" AND TipologieSementixSpecieVegetali.Veg_COD = " & Agro_SQL_SaveNum(VEG_COD))
                    End If
                    If xFiltroAggiuntivo <> "" Then
                        stbQuery.AppendLine(Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri_server))
                    End If
                    If xOrderBy <> "" Then
                        stbQuery.AppendLine(Agro_SQL_Save_xOrderBy(xOrderBy, objParametri_server))
                    Else
                        stbQuery.AppendLine(" ORDER BY Sem_Des, Veg_Des ")
                    End If


                    stbQuery.AppendLine("ELSE ")


                    stbQuery.AppendLine(" SELECT DISTINCT TipologieSementixSpecieVegetali.SEM_COD, TipologieSementixSpecieVegetali.VEG_COD, ")
                    stbQuery.AppendLine("       ISNULL(TipologieSementi.SEM_DES,'') AS Sem_Des, ISNULL(SpecieVegetali.Veg_Des,'') AS Veg_Des,  ")
                    stbQuery.AppendLine("       ISNULL(SpecieVegetali.Grsp_Cod,0) AS Grsp_Cod, ISNULL(SpecieVegetali.Gru_Cod,-1) AS Gru_Cod, ")
                    stbQuery.AppendLine("       ISNULL(TipologieSementi.Coeff_ener,0) AS Coeff_ener, ISNULL(TipologieSementi.Coeff_azoto,0) AS Coeff_azoto, ISNULL(TipologieSementi.Coeff_fosforo,0) AS Coeff_fosforo ")
                    stbQuery.AppendLine(" FROM  TipologieSementixSpecieVegetali INNER JOIN ")
                    stbQuery.AppendLine("       TipologieSementi ON TipologieSementixSpecieVegetali.SEM_COD = TipologieSementi.SEM_COD INNER JOIN ")
                    stbQuery.AppendLine("       SpecieVegetali ON TipologieSementixSpecieVegetali.VEG_COD = SpecieVegetali.Veg_Cod ")
                    stbQuery.AppendLine(" WHERE SpecieVegetali.Validita_Inizio <=" & Agro_SQL_SaveDate(CDate(FinestraTemp_Fine)) & " ")
                    stbQuery.AppendLine(" AND   SpecieVegetali.Validita_Fine >=" & Agro_SQL_SaveDate(CDate(FinestraTemp_Inizio)) & " ")
                    stbQuery.AppendLine(" AND   TipologieSementixSpecieVegetali.Validita_Inizio <=" & Agro_SQL_SaveDate(CDate(FinestraTemp_Fine)) & " ")
                    stbQuery.AppendLine(" AND   TipologieSementixSpecieVegetali.Validita_Fine >=" & Agro_SQL_SaveDate(CDate(FinestraTemp_Inizio)) & " ")
                    stbQuery.AppendLine(" AND   SpecieVegetali.gru_Cod = " & Agro_SQL_SaveNum(Gru_Cod) & " ")
                    If SEM_COD <> 0 Then
                        stbQuery.AppendLine(" AND TipologieSementixSpecieVegetali.SEM_COD = " & Agro_SQL_SaveNum(SEM_COD))
                    End If
                    If VEG_COD <> 0 Then
                        stbQuery.AppendLine(" AND TipologieSementixSpecieVegetali.Veg_COD = " & Agro_SQL_SaveNum(VEG_COD))
                    End If
                    If xFiltroAggiuntivo <> "" Then
                        stbQuery.AppendLine(Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri_server))
                    End If
                    If xOrderBy <> "" Then
                        stbQuery.AppendLine(Agro_SQL_Save_xOrderBy(xOrderBy, objParametri_server))
                    Else
                        stbQuery.AppendLine(" ORDER BY Sem_Des, Veg_Des ")
                    End If


                    '--------------------------------------------------------------------------
                    dt = EseguiQuery_Lettura(objParametri_utenti, stbQuery.ToString, nomeRoutine)
                    '--------------------------------------------------------------------------

                    If i = 0 Then
                        'il primo giro clono la struttura del datatable
                        DT_Clonato = dt.Clone
                    End If

                    For j = 0 To dt.Rows.Count - 1
                        'importo ogni riga nel nuovo datatable
                        DT_Clonato.ImportRow(dt.Rows.Item(j))

                    Next

                Next 'per i gru_cod

                Return DT_Clonato


            End If 'union all


        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri_utenti, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

End Class
