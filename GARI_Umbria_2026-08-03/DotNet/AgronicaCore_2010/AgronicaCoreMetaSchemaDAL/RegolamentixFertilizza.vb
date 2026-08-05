Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi

Public Class RegolamentixFertilizza_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '#############################################################################################
    Public Function Leggi( _
                          ByVal REG_COD As Integer, _
                          ByVal FER_COD As Integer, _
                                ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile, _
                                ByVal xFiltroAggiuntivo As String, _
                                ByVal xOrderBy As String, _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.RegolamentixFertilizza_R.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                    StrSQL.Length = 0

                    StrSQL.Append(" SELECT DISTINCT FER_COD, REG_COD ")
                    StrSQL.Append(" FROM  RegolamentixFertilizzanti ")
                    StrSQL.Append(" WHERE RegolamentixFertilizzanti.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND   RegolamentixFertilizzanti.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    If REG_COD <> 0 Then
                        StrSQL.Append(" AND RegolamentixFertilizzanti.REG_COD =  " & Agro_SQL_SaveNum(REG_COD) & "  ")
                    End If

                    If FER_COD <> 0 Then
                        StrSQL.Append(" AND RegolamentixFertilizzanti.FER_COD =  " & Agro_SQL_SaveNum(FER_COD) & "  ")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   RegolamentixFertilizzanti.Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   RegolamentixFertilizzanti.Inviato =-1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    End If


                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta
                    '---------------------------------------------
                    StrSQL.Length = 0

                    StrSQL.Append(" SELECT * ")
                    StrSQL.Append(" FROM  RegolamentixFertilizzanti  ")
                    StrSQL.Append(" WHERE RegolamentixFertilizzanti.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND   RegolamentixFertilizzanti.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    If REG_COD <> 0 Then
                        StrSQL.Append(" AND RegolamentixFertilizzanti.REG_COD =  " & Agro_SQL_SaveNum(REG_COD) & "  ")
                    End If

                    If FER_COD <> 0 Then
                        StrSQL.Append(" AND RegolamentixFertilizzanti.FER_COD =  " & Agro_SQL_SaveNum(FER_COD) & "  ")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   RegolamentixFertilizzanti.Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   RegolamentixFertilizzanti.Inviato =-1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    End If

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni
                    '
                    '
                    '
                    '


                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta
                    StrSQL.Length = 0

                    StrSQL.Append(" SELECT * ")
                    StrSQL.Append(" FROM  RegolamentixFertilizzanti , Fertilizzanti , Regolamenti ")
                    StrSQL.Append(" WHERE RegolamentixFertilizzanti.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND   RegolamentixFertilizzanti.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.Append(" AND   Fertilizzanti.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND   Fertilizzanti.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.Append(" AND   Regolamenti.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND   Regolamenti.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.Append(" AND   RegolamentixFertilizzanti.REG_COD = Regolamenti.REG_COD ")
                    StrSQL.Append(" AND   RegolamentixFertilizzanti.FER_COD = Fertilizzanti.FER_COD ")

                    If REG_COD <> 0 Then
                        StrSQL.Append(" AND RegolamentixFertilizzanti.REG_COD =  " & Agro_SQL_SaveNum(REG_COD) & "  ")
                    End If

                    If FER_COD <> 0 Then
                        StrSQL.Append(" AND RegolamentixFertilizzanti.FER_COD =  " & Agro_SQL_SaveNum(FER_COD) & "  ")
                    End If


                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   RegolamentixFertilizzanti.Inviato >=0 ")
                            StrSQL.Append(" AND   Fertilizzanti.Inviato >=0 ")
                            StrSQL.Append(" AND   Regolamenti.Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   RegolamentixFertilizzanti.Inviato =-1 ")
                            StrSQL.Append(" AND   Fertilizzanti.Inviato =-1 ")
                            StrSQL.Append(" AND   Regolamenti.Inviato =-1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY Fertilizzanti.FER_DES ASC, Regolamenti.REG_DES ASC")
                    End If

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


    '#############################################################################################
    'Reg_cod opzionale (0 oppure valore)
    'Flag_AggiungiFerCod se = true aggiunge all'hashtable tutti i fer_cod della lista che non sono stati trovati dalla query
    '(dipende se ho bisogno di sapere l'info per ognuno o se voglio solo quelli bio in risposta)
    Public Function BIO_1_0_from_ElencoFertilizzanti( _
                                ByVal Lista_Fertilizzanti As String, _
                                ByVal Reg_Cod As enum_Cod_Regolamento, _
                                ByVal Flag_AggiungiFerCod As Boolean, _
                                ByVal xFiltroAggiuntivo As String, _
                                ByVal xOrderBy As String, _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                ) As Hashtable


        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.RegolamentixFertilizza_R.BIO_1_0_from_ElencoFertilizzanti()"

        '====================================================================================
        'Parametri opzionali :
        '
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim DT As DataTable
        Dim i As Integer
        Dim filtro As String
        Dim HT_Fert As New Hashtable
        Dim Fer_Cod As Integer

        Try

            filtro = " RegolamentixFertilizzanti.Fer_Cod IN (" + Agro_SQL_Save_Clausola_IN(Lista_Fertilizzanti) + ")"

            'reg_cod = 0 voglio leggerli tutti
            'reg_cod = 4 voglio in risultato solo quelli bio
            DT = Leggi(Reg_Cod, 0, _
                        AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, _
                        filtro, _
                        "", _
                        objParametri)

            If Not IsNothing(DT) Then

                If DT.Rows.Count > 0 Then

                    For i = 0 To DT.Rows.Count - 1

                        Fer_Cod = DT.Rows(i).Item("Fer_cod")
                        Reg_Cod = DT.Rows(i).Item("Reg_cod")

                        Select Case Reg_Cod

                            Case enum_Cod_Regolamento.Regolamento_bio
                                If Not HT_Fert.Contains(Fer_Cod) Then
                                    HT_Fert.Add(Fer_Cod, 1)
                                End If

                            Case Else
                                If Not HT_Fert.Contains(Fer_Cod) Then
                                    HT_Fert.Add(Fer_Cod, 0)
                                End If

                        End Select

                    Next

                End If

            End If


            'voglio avere la info 0 opp 1 per tutti i fer_cod della lista 
            '(anche per quelli che non sono presenti nel risultato della query)
            If Flag_AggiungiFerCod = True Then

                'aggiungo i fer_cod che non sono stati trovati nella tabella e li metto NON bio
                Dim vet_Fert() As String
                vet_Fert = Lista_Fertilizzanti.Split(",")

                For i = 0 To vet_Fert.Length - 1

                    Fer_Cod = CInt(vet_Fert(i))

                    If Not HT_Fert.Contains(Fer_Cod) Then
                        HT_Fert.Add(Fer_Cod, 0)
                    End If

                Next

            End If

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return HT_Fert

    End Function




End Class