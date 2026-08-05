Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi

Public Class Formulati_R
    Inherits AgronicaCoreDataProvider.DataProvider



    Public Function LeggixDescrizione(
                                ByVal Fr_Des As String,
                                ByVal xSelezioneVariabile As enumSelezioneVariabile,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.Formulati_R.LeggixDescrizione()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi
                    '---------------------------------------------
                    StrSQL.Length = 0

                    StrSQL.Append("SELECT * FROM Formulati ")
                    StrSQL.Append(" WHERE 1=1 ")

                    If Fr_Des <> "" Then
                        StrSQL.Append(" AND FR_DES LIKE '%" & Agro_SQL_SaveText(Fr_Des) & "%' ")
                    End If


                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        'Nota: Questo ordinamento è importante per la gestione del campo.
                        'Viene letto l'impianto più RECENTE dell'appezzamento associato al campo
                        StrSQL.Append(" ORDER BY Fr_Des ASC")
                    End If




                Case enumSelezioneVariabile.Selezione_TabellaCompleta
                    '---------------------------------------------
                    StrSQL.Length = 0

                    StrSQL.Append("SELECT Fr_Cod , Fr_Des FROM Formulati ")
                    StrSQL.Append(" WHERE 1=1 ")

                    If Fr_Des <> "" Then
                        StrSQL.Append(" AND FR_DES LIKE '%" & Agro_SQL_SaveText(Fr_Des) & "%' ")
                    End If


                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If

                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY Fr_Des ASC")
                    End If

                Case enumSelezioneVariabile.Selezione_JoinDescrizioni
                    '
                    '
                    '
                    '


                Case enumSelezioneVariabile.Selezione_JoinCompleta
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



    Public Function Leggi(ByVal FR_COD As Integer,
                            ByVal FinestraTemp_Inizio As Date,
                            ByVal FinestraTemp_Fine As Date,
                                ByVal xSelezioneVariabile As enumSelezioneVariabile,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                  ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                         ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.Formulati_R.Leggi()"


        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                    '---------------------------------------------
                    StrSQL.Length = 0
                    StrSQL.AppendLine(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; ")
                    StrSQL.Append(" SELECT Fr_cod, Fr_Des FROM  Formulati " &
                                  " WHERE Formulati.Validita_inizio <= " & Agro_SQL_SaveDate(FinestraTemp_Fine) & " " &
                                  " AND   Formulati.Validita_Fine >= " & Agro_SQL_SaveDate(FinestraTemp_Inizio) & " ")

                    If FR_COD <> 0 Then
                        StrSQL.Append(" AND Formulati.FR_COD =  " & Agro_SQL_SaveNum(FR_COD) & "  ")
                    End If


                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        'Nota: Questo ordinamento è importante per la gestione del campo.
                        'Viene letto l'impianto più RECENTE dell'appezzamento associato al campo
                        StrSQL.Append(" ORDER BY Formulati.FR_DES ASC ")
                    End If


                Case enumSelezioneVariabile.Selezione_TabellaCompleta

                    '---------------------------------------------
                    StrSQL.Length = 0
                    StrSQL.AppendLine(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; ")
                    StrSQL.Append(" SELECT * FROM  Formulati " &
                                  " WHERE Formulati.Validita_inizio <= " & Agro_SQL_SaveDate(FinestraTemp_Fine) & " " &
                                  " AND   Formulati.Validita_Fine >= " & Agro_SQL_SaveDate(FinestraTemp_Inizio) & " ")

                    If FR_COD <> 0 Then
                        StrSQL.Append(" AND Formulati.FR_COD =  " & Agro_SQL_SaveNum(FR_COD) & "  ")
                    End If


                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        'Nota: Questo ordinamento è importante per la gestione del campo.
                        'Viene letto l'impianto più RECENTE dell'appezzamento associato al campo
                        StrSQL.Append(" ORDER BY Formulati.FR_DES ASC ")
                    End If


                Case enumSelezioneVariabile.Selezione_JoinDescrizioni
                    '
                Case enumSelezioneVariabile.Selezione_JoinCompleta
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


    Public Function Leggi_conCosti( _
                        ByVal FR_COD As Integer, _
                            ByVal xFiltroAggiuntivo As String, _
                            ByVal xOrderBy As String, _
                              ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                     ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.Formulati_R.Leggi_conCosti()"


        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try


            '---------------------------------------------
            StrSQL.Length = 0

            'Formulati SENZA COSTI

            StrSQL.Append(" (SELECT Formulati.*, 0 AS Prezzo_Unitario, '01/01/1900' as Validita_Inizio_Prezzo, '31/12/2100' as Validita_Fine_Prezzo,   " & _
                          "  0 as Udm_Cod_Prezzo, '' as Udm_Sim_Prezzo " & _
                          "  FROM  Formulati " & _
                          " WHERE Formulati.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " " & _
                          " AND   Formulati.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            StrSQL.Append(" AND NOT EXISTS ( ")
            StrSQL.Append("                 SELECT * ")
            StrSQL.Append("                 FROM Prodotti_Costi ")
            StrSQL.Append("                 WHERE Elem_Cod=191 ")
            StrSQL.Append("                 AND Formulati.Fr_Cod = Prodotti_Costi.Pro_Cod And Prodotti_Costi.Id_Budget = 0 )")

            If FR_COD <> 0 Then
                StrSQL.Append(" AND Formulati.FR_COD =  " & Agro_SQL_SaveNum(FR_COD) & "  ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   Formulati.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   Formulati.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select

            StrSQL.Append(") UNION ALL (")


            'Formulati CON COSTI

            StrSQL.Append(" SELECT Formulati.*, Prodotti_Costi.Prezzo_Unitario, Prodotti_Costi.validita_inizio as Validita_Inizio_Prezzo, Prodotti_Costi.validita_fine as Validita_Fine_Prezzo,   " &
                          "  Prodotti_Costi.Udm_Cod as Udm_Cod_Prezzo, UnitaMisura.UDM_SIM as Udm_Sim_Prezzo " &
                          "  FROM Formulati INNER JOIN Prodotti_Costi ON Formulati.Fr_Cod = Prodotti_Costi.Pro_Cod And Prodotti_Costi.Id_Budget = 0 INNER JOIN UnitaMisura ON Prodotti_Costi.Udm_Cod = UnitaMisura.UDM_COD " &
                          "  WHERE Formulati.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " " &
                          "  AND   Formulati.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " " &
                          "  AND   Elem_Cod=191 ")

            If FR_COD <> 0 Then
                StrSQL.Append(" AND Formulati.FR_COD =  " & Agro_SQL_SaveNum(FR_COD) & "  ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   Formulati.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   Formulati.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select

            StrSQL.Append(" ) ")

            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY Formulati.FR_DES ASC ")
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


    '###############################################################################
    'anzichè il left join che rallenta,
    'fa la union all
    Public Function LeggiConPeriodoSospensione( _
                            ByVal FR_COD As String, _
                            ByVal FinestraTemp_Inizio As Date, _
                            ByVal FinestraTemp_Fine As Date, _
                                ByVal xFiltroAggiuntivo As String, _
                                ByVal xOrderBy As String, _
                                  ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                         ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.Formulati_R.LeggiConPeriodoSospensione()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.Append(" SELECT * " & vbCrLf)
            StrSQL.Append(" FROM  " & vbCrLf)
            StrSQL.Append(" (  " & vbCrLf)
            '----------------------------------------------------------------
            '----PRIMA PARTE: FORMULATI CON RECORD IN PERIODO SOSPENSIONE
            '----------------------------------------------------------------
            StrSQL.Append(" SELECT    Progressivo_sospensione,DataSospensioneDA,DataSospensioneA, Formulati.* " & vbCrLf)
            StrSQL.Append(" FROM Formulati " & vbCrLf)
            StrSQL.Append("  INNER JOIN FormulatiXPeriodoSospensione " & vbCrLf)
            StrSQL.Append(" ON FormulatiXPeriodoSospensione.Fr_cod = Formulati.Fr_cod " & vbCrLf)
            StrSQL.Append(" WHERE Formulati.Validita_inizio <= " & Agro_SQL_SaveDate(FinestraTemp_Fine) & " " & vbCrLf)
            StrSQL.Append(" AND   Formulati.Validita_Fine >= " & Agro_SQL_SaveDate(FinestraTemp_Inizio) & " " & vbCrLf)
            If FR_COD <> 0 Then
                StrSQL.Append(" AND Formulati.FR_COD =  " & Agro_SQL_SaveNum(FR_COD) & "  ")
            End If
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   Formulati.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   Formulati.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            StrSQL.Append(" " & vbCrLf)
            StrSQL.Append(" UNION ALL" & vbCrLf)
            StrSQL.Append(" " & vbCrLf)
            '----------------------------------------------------------------
            '----SECONDA PARTE: FORMULATI SENZA RECORD IN PERIODO SOSPENSIONE
            '----------------------------------------------------------------
            StrSQL.Append(" SELECT 0 as Progressivo_sospensione, null as DataSospensioneDA, null as DataSospensioneA, Formulati.* " & vbCrLf)
            StrSQL.Append(" FROM Formulati " & vbCrLf)
            StrSQL.Append(" WHERE NOT EXISTS " & vbCrLf)
            StrSQL.Append("                 (SELECT * " & vbCrLf)
            StrSQL.Append("                 FROM FormulatiXPeriodoSospensione  " & vbCrLf)
            StrSQL.Append("                 WHERE FormulatiXPeriodoSospensione.Fr_cod = Formulati.Fr_cod " & vbCrLf)
            StrSQL.Append("                 )" & vbCrLf)
            StrSQL.Append(" AND Formulati.Validita_inizio <= " & Agro_SQL_SaveDate(FinestraTemp_Fine) & " " & vbCrLf)
            StrSQL.Append(" AND Formulati.Validita_Fine >= " & Agro_SQL_SaveDate(FinestraTemp_Inizio) & " " & vbCrLf)
            If FR_COD <> 0 Then
                StrSQL.Append(" AND Formulati.FR_COD =  " & Agro_SQL_SaveNum(FR_COD) & "  ")
            End If
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   Formulati.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   Formulati.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            StrSQL.Append("  ) AS FITO" & vbCrLf)
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY FR_DES ")
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


    '###############################################################################
    Public Function LeggiSospesi( _
                            ByVal FR_COD As String, _
                            ByVal FinestraTemp_Inizio As Date, _
                            ByVal FinestraTemp_Fine As Date, _
                                ByVal xFiltroAggiuntivo As String, _
                                ByVal xOrderBy As String, _
                                  ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                         ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.Formulati_R.LeggiSospesi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.Append(" SELECT    Progressivo_sospensione, DataSospensioneDA, DataSospensioneA, Formulati.* " & vbCrLf)
            StrSQL.Append(" FROM Formulati " & vbCrLf)
            StrSQL.Append(" INNER JOIN FormulatiXPeriodoSospensione " & vbCrLf)
            StrSQL.Append(" ON FormulatiXPeriodoSospensione.Fr_cod = Formulati.Fr_cod " & vbCrLf)
            StrSQL.Append(" WHERE Formulati.Validita_inizio <= " & Agro_SQL_SaveDate(FinestraTemp_Fine) & " " & vbCrLf)
            StrSQL.Append(" AND   Formulati.Validita_Fine >= " & Agro_SQL_SaveDate(FinestraTemp_Inizio) & " " & vbCrLf)
            If FR_COD <> 0 Then
                StrSQL.Append(" AND Formulati.FR_COD =  " & Agro_SQL_SaveNum(FR_COD) & "  ")
            End If
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   Formulati.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   Formulati.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY FR_DES ")
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


    '###############################################################################
    Public Function Filtra_Formulati(ByVal Dt As DataTable, _
                                     ByVal Data As Date, _
                                     ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                    ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.Formulati_R.Filtra_Formulati()"
        Dim MessaggioErrore As String = ""
        Dim Dt_New As New DataTable

        Try

            '1° Data fine uso scorte
            '2° Se la data fine uso scorte non è compilata: guardare Data fine commercializzazione
            '3° Se la data fine commercializzazione non è compilata: guardare Data di revoca – Data di sospensione: se è compilato più di un campo, guardare prima quella con la data più avanti nel tempo.

            Dim Messaggio As String
            Dim i As Integer

            Dim FormulatoValido As Boolean

            Dim Revocato As Integer = 0
            Dim Sospeso As Integer = 0
            Dim Terminato As Integer = 0

            Dt_New = Dt.Clone

            If Messaggio = "" Then

                For i = 0 To Dt.Rows.Count - 1

                    FormulatoValido = True

                    '1° Data fine uso scorte
                    If Not IsDBNull(Dt.Rows(i).Item("data_fine_usoscorte")) AndAlso IsDate(Dt.Rows(i).Item("data_fine_usoscorte")) Then

                        If CDate(Dt.Rows(i).Item("data_fine_usoscorte")) < Data Then
                            FormulatoValido = False
                        End If

                    Else

                        '2° Se la data fine uso scorte non è compilata: guardare Data fine commercializzazione
                        If Not IsDBNull(Dt.Rows(i).Item("Data_Fine_Comm")) AndAlso IsDate(Dt.Rows(i).Item("Data_Fine_Comm")) Then

                            If CDate(Dt.Rows(i).Item("Data_Fine_Comm")) < Data Then
                                FormulatoValido = False
                            End If

                        Else
                            '3° Se la data fine commercializzazione non è compilata: 
                            'guardare Data di revoca – Data di sospensione – Data di Termine: 
                            'se è compilato più di un campo, guardare prima quella con la data più avanti nel tempo.

                            'REVOCATO
                            If Not IsDBNull(Dt.Rows(i).Item("revocato")) AndAlso Dt.Rows(i).Item("revocato") <> 0 Then

                                If Not IsDBNull(Dt.Rows(i).Item("data_revo")) AndAlso IsDate(Dt.Rows(i).Item("data_revo")) Then
                                    If CDate(Dt.Rows(i).Item("data_revo")) < Data Then
                                        FormulatoValido = False
                                    End If
                                Else
                                    FormulatoValido = False
                                End If

                            Else

                                ''TERMINATO
                                'If Not IsDBNull(Dt.Rows(i).Item("termine")) AndAlso Dt.Rows(i).Item("termine") <> 0 Then

                                '    If Not IsDBNull(Dt.Rows(i).Item("data_term")) AndAlso IsDate(Dt.Rows(i).Item("data_term")) Then

                                '        If CDate(Dt.Rows(i).Item("data_term")) < Data Then
                                '            FormulatoValido = False
                                '        End If

                                '    Else

                                '        FormulatoValido = False

                                '    End If

                                'Else

                                'SOSPESO
                                If Not IsDBNull(Dt.Rows(i).Item("sospeso")) AndAlso Dt.Rows(i).Item("sospeso") <> 0 Then

                                    If Not IsDBNull(Dt.Rows(i).Item("data_sosp")) AndAlso IsDate(Dt.Rows(i).Item("data_sosp")) Then

                                        If CDate(Dt.Rows(i).Item("data_sosp")) < Data Then
                                            FormulatoValido = False
                                        End If

                                        'Else

                                        '    FormulatoValido = False

                                    End If

                                Else

                                    '

                                End If


                                'End If

                            End If


                        End If

                    End If

                    If FormulatoValido = True Then
                        Dt_New.ImportRow(Dt.Rows(i))
                    End If

                Next

            End If

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Dt_New = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return Dt_New

    End Function

    '###############################################################################
    'A differenza della precedente 
    'filtra le date di sospensione nella nuova tabella FormulatiXPeriodoSospensione
    Public Function Filtra_Formulati_2_old(ByVal Dt As DataTable, _
                                       ByVal Data As Date, _
                                       ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                        ) As DataTable

        '1. Data fine uso scorte
        '2. Se la data fine uso scorte non è compilata: guardare Data fine commercializzazione
        '3. Se la data fine commercializzazione non è compilata: guardare Data di revoca è Data di sospensione: se è compilato più di un campo, guardare prima quella con la data più avanti nel tempo.

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.Formulati_R.Filtra_Formulati_2()"
        Dim MessaggioErrore As String = ""

        Dim Messaggio As String = ""
        Dim i, j As Integer

        Dim FormulatoValido As Boolean

        Dim Revocato As Integer = 0
        Dim Sospeso As Integer = 0
        Dim Terminato As Integer = 0

        Try

            'estraggo i sospesi
            Dim DtSosp As DataTable
            Dim DrSosp() As DataRow
            DtSosp = LeggiSospesi(0, AGRODATAINIZIO, AGRODATAFINE, "", "", objParametri)


            Dim Dt_New As New DataTable

            Dt_New = Dt.Clone

            If Messaggio = "" Then

                For i = 0 To Dt.Rows.Count - 1

                    FormulatoValido = True




                    '1. Data Registrazione formulato
                    If Not IsDBNull(Dt.Rows(i).Item("data_reg")) AndAlso IsDate(Dt.Rows(i).Item("data_reg")) Then

                        If CDate(Dt.Rows(i).Item("data_reg")) > Data Then
                            FormulatoValido = False
                        End If

                    Else

                        '2. Data fine uso scorte
                        If Not IsDBNull(Dt.Rows(i).Item("data_fine_usoscorte")) AndAlso IsDate(Dt.Rows(i).Item("data_fine_usoscorte")) Then

                            If CDate(Dt.Rows(i).Item("data_fine_usoscorte")) < Data Then
                                FormulatoValido = False
                            End If

                        Else

                            '3. Se la data fine uso scorte non è compilata: guardare Data fine commercializzazione
                            If Not IsDBNull(Dt.Rows(i).Item("Data_Fine_Comm")) AndAlso IsDate(Dt.Rows(i).Item("Data_Fine_Comm")) Then

                                If CDate(Dt.Rows(i).Item("Data_Fine_Comm")) < Data Then
                                    FormulatoValido = False
                                End If

                            Else
                                '4. Se la data fine commercializzazione non è compilata: 
                                'guardare Data di revoca e Data di sospensione : 
                                'se è compilato più di un campo, guardare prima quella con la data più avanti nel tempo.

                                'REVOCATO
                                If Not IsDBNull(Dt.Rows(i).Item("revocato")) AndAlso Dt.Rows(i).Item("revocato") <> 0 Then

                                    If Not IsDBNull(Dt.Rows(i).Item("data_revo")) AndAlso IsDate(Dt.Rows(i).Item("data_revo")) Then
                                        If CDate(Dt.Rows(i).Item("data_revo")) < Data Then
                                            FormulatoValido = False
                                        End If
                                    Else
                                        FormulatoValido = False
                                    End If

                                Else

                                    ''TERMINATO
                                    'If Not IsDBNull(Dt.Rows(i).Item("termine")) AndAlso Dt.Rows(i).Item("termine") <> 0 Then

                                    '    If Not IsDBNull(Dt.Rows(i).Item("data_term")) AndAlso IsDate(Dt.Rows(i).Item("data_term")) Then

                                    '        If CDate(Dt.Rows(i).Item("data_term")) < Data Then
                                    '            FormulatoValido = False
                                    '        End If

                                    '    Else

                                    '        FormulatoValido = False

                                    '    End If

                                    'Else

                                    '5. SOSPESO
                                    DrSosp = DtSosp.Select("Fr_Cod=" & Dt.Rows(i).Item("Fr_Cod"))

                                    If Not DrSosp Is Nothing AndAlso DrSosp.Length > 0 Then
                                        For j = 0 To DrSosp.Length - 1
                                            If Not IsDBNull(DrSosp(j).Item("DataSospensioneDA")) And Not IsDBNull(DrSosp(j).Item("DataSospensioneA")) Then
                                                If CDate(DrSosp(j).Item("DataSospensioneDA")) < Data And _
                                                    CDate(DrSosp(j).Item("DataSospensioneA")) > Data Then
                                                    FormulatoValido = False
                                                    Exit For
                                                End If
                                            ElseIf Not IsDBNull(DrSosp(j).Item("DataSospensioneDA")) And IsDBNull(DrSosp(j).Item("DataSospensioneA")) Then
                                                If CDate(DrSosp(j).Item("DataSospensioneDA")) < Data Then
                                                    FormulatoValido = False
                                                    Exit For
                                                End If
                                            ElseIf IsDBNull(DrSosp(j).Item("DataSospensioneDA")) And Not IsDBNull(DrSosp(j).Item("DataSospensioneA")) Then
                                                If CDate(DrSosp(j).Item("DataSospensioneA")) > Data Then
                                                    FormulatoValido = False
                                                    Exit For
                                                End If
                                            End If
                                        Next
                                    End If

                                    ' ''SOSPESO
                                    ''If Not IsDBNull(Dt.Rows(i).Item("DataSospensioneDA")) And Not IsDBNull(Dt.Rows(i).Item("DataSospensioneA")) Then
                                    ''    If CDate(Dt.Rows(i).Item("DataSospensioneDA")) < Data And _
                                    ''        CDate(Dt.Rows(i).Item("DataSospensioneA")) > Data Then
                                    ''        FormulatoValido = False
                                    ''    End If
                                    ''ElseIf Not IsDBNull(Dt.Rows(i).Item("DataSospensioneDA")) And IsDBNull(Dt.Rows(i).Item("DataSospensioneA")) Then
                                    ''    If CDate(Dt.Rows(i).Item("DataSospensioneDA")) < Data Then
                                    ''        FormulatoValido = False
                                    ''    End If
                                    ''ElseIf IsDBNull(Dt.Rows(i).Item("DataSospensioneDA")) And Not IsDBNull(Dt.Rows(i).Item("DataSospensioneA")) Then
                                    ''    If CDate(Dt.Rows(i).Item("DataSospensioneA")) > Data Then
                                    ''        FormulatoValido = False
                                    ''    End If
                                    ''End If
                                    'End If

                                End If


                            End If

                        End If

                    End If

                    If FormulatoValido = True Then
                        Dt_New.ImportRow(Dt.Rows(i))
                    End If

                Next

            End If

            Return Dt_New

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Return Nothing
            'Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

    End Function

    '###############################################################################
    'A differenza della precedente 
    'filtra le date di sospensione nella nuova tabella FormulatiXPeriodoSospensione
    Public Function Filtra_Formulati_2(ByVal Dt As DataTable, _
                                       ByVal Data As Date, _
                                       ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                        ) As DataTable

        '1. Data fine uso scorte
        '2. Se la data fine uso scorte non è compilata: guardare Data fine commercializzazione
        '3. Se la data fine commercializzazione non è compilata: guardare Data di revoca è Data di sospensione: se è compilato più di un campo, guardare prima quella con la data più avanti nel tempo.

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.Formulati_R.Filtra_Formulati_2()"
        Dim MessaggioErrore As String = ""

        Dim Messaggio As String = ""
        Dim i, j As Integer

        Dim FormulatoValido As Boolean

        Dim Revocato As Integer = 0
        Dim Sospeso As Integer = 0
        Dim Terminato As Integer = 0

        Try
            '''
            ''' DRUDI 20/06/2024 Se non c'è neanche una riga nel DT non leggo i sospesi
            '''
            Dim Dt_New As New DataTable

            Dt_New = Dt.Clone

            If Messaggio = "" AndAlso Dt.Rows.Count > 0 Then

                'estraggo i sospesi
                Dim DtSosp As DataTable
                Dim DrSosp() As DataRow
                DtSosp = LeggiSospesi(0, AGRODATAINIZIO, AGRODATAFINE, "", "", objParametri)

                For i = 0 To Dt.Rows.Count - 1

                    FormulatoValido = True

                    '1. Data Registrazione formulato
                    If Not IsDBNull(Dt.Rows(i).Item("data_reg")) AndAlso IsDate(Dt.Rows(i).Item("data_reg")) Then

                        If CDate(Dt.Rows(i).Item("data_reg")) > Data Then
                            FormulatoValido = False
                        Else

                            '2. Data fine uso scorte
                            If Not IsDBNull(Dt.Rows(i).Item("data_fine_usoscorte")) AndAlso IsDate(Dt.Rows(i).Item("data_fine_usoscorte")) Then

                                If CDate(Dt.Rows(i).Item("data_fine_usoscorte")) < Data Then
                                    FormulatoValido = False
                                End If

                            Else

                                '3. Se la data fine uso scorte non è compilata: guardare Data fine commercializzazione
                                If Not IsDBNull(Dt.Rows(i).Item("Data_Fine_Comm")) AndAlso IsDate(Dt.Rows(i).Item("Data_Fine_Comm")) Then

                                    If CDate(Dt.Rows(i).Item("Data_Fine_Comm")) < Data Then
                                        FormulatoValido = False
                                    End If

                                Else
                                    '4. Se la data fine commercializzazione non è compilata: 
                                    'guardare Data di revoca e Data di sospensione : 
                                    'se è compilato più di un campo, guardare prima quella con la data più avanti nel tempo.

                                    'REVOCATO
                                    If Not IsDBNull(Dt.Rows(i).Item("revocato")) AndAlso Dt.Rows(i).Item("revocato") <> 0 Then

                                        If Not IsDBNull(Dt.Rows(i).Item("data_revo")) AndAlso IsDate(Dt.Rows(i).Item("data_revo")) Then
                                            If CDate(Dt.Rows(i).Item("data_revo")) < Data Then
                                                FormulatoValido = False
                                            End If
                                        Else
                                            FormulatoValido = False
                                        End If

                                    Else

                                        '5. SOSPESO
                                        DrSosp = DtSosp.Select("Fr_Cod=" & Dt.Rows(i).Item("Fr_Cod"))

                                        If Not DrSosp Is Nothing AndAlso DrSosp.Length > 0 Then
                                            For j = 0 To DrSosp.Length - 1
                                                If Not IsDBNull(DrSosp(j).Item("DataSospensioneDA")) And Not IsDBNull(DrSosp(j).Item("DataSospensioneA")) Then
                                                    If CDate(DrSosp(j).Item("DataSospensioneDA")) < Data And
                                                        CDate(DrSosp(j).Item("DataSospensioneA")) > Data Then
                                                        FormulatoValido = False
                                                        Exit For
                                                    End If
                                                ElseIf Not IsDBNull(DrSosp(j).Item("DataSospensioneDA")) And IsDBNull(DrSosp(j).Item("DataSospensioneA")) Then
                                                    If CDate(DrSosp(j).Item("DataSospensioneDA")) < Data Then
                                                        FormulatoValido = False
                                                        Exit For
                                                    End If
                                                ElseIf IsDBNull(DrSosp(j).Item("DataSospensioneDA")) And Not IsDBNull(DrSosp(j).Item("DataSospensioneA")) Then
                                                    If CDate(DrSosp(j).Item("DataSospensioneA")) > Data Then
                                                        FormulatoValido = False
                                                        Exit For
                                                    End If
                                                End If
                                            Next
                                        End If

                                    End If

                                End If

                            End If


                        End If

                    End If

                    If FormulatoValido = True Then
                        Dt_New.ImportRow(Dt.Rows(i))
                    End If

                Next

            End If

            Return Dt_New

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Return Nothing
            'Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

    End Function


    '###############################################################################
    'SI PUO' USARE IN LOCALE:
    'A differenza della Filtra_Formulati_2 
    'non legge la sospensione (perchè è stata controllata prima di questa chiamata)
    '----
    'utilizzata da WS_Importa_Magazzino_2010
    Public Function Filtra_Formulati_3(ByVal Dt As DataTable, _
                                       ByVal Data As Date, _
                                       ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                        ) As DataTable

        '1. Data fine uso scorte
        '2. Se la data fine uso scorte non è compilata: guardare Data fine commercializzazione
        '3. Se la data fine commercializzazione non è compilata: guardare Data di revoca è Data di sospensione: se è compilato più di un campo, guardare prima quella con la data più avanti nel tempo.

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.Formulati_R.Filtra_Formulati_3()"
        Dim MessaggioErrore As String = ""

        Dim Messaggio As String = ""
        Dim i, j As Integer

        Dim FormulatoValido As Boolean

        Dim Revocato As Integer = 0
        Dim Sospeso As Integer = 0
        Dim Terminato As Integer = 0

        Try

            Dim Dt_New As New DataTable

            Dt_New = Dt.Clone

            If Messaggio = "" Then

                For i = 0 To Dt.Rows.Count - 1

                    FormulatoValido = True

                    '1. Data Registrazione formulato
                    If Not IsDBNull(Dt.Rows(i).Item("data_reg")) AndAlso IsDate(Dt.Rows(i).Item("data_reg")) Then

                        If CDate(Dt.Rows(i).Item("data_reg")) > Data Then
                            FormulatoValido = False
                        Else

                            '2. Data fine uso scorte
                            If Not IsDBNull(Dt.Rows(i).Item("data_fine_usoscorte")) AndAlso IsDate(Dt.Rows(i).Item("data_fine_usoscorte")) Then

                                If CDate(Dt.Rows(i).Item("data_fine_usoscorte")) < Data Then
                                    FormulatoValido = False
                                End If

                            Else

                                '3. Se la data fine uso scorte non è compilata: guardare Data fine commercializzazione
                                If Not IsDBNull(Dt.Rows(i).Item("Data_Fine_Comm")) AndAlso IsDate(Dt.Rows(i).Item("Data_Fine_Comm")) Then

                                    If CDate(Dt.Rows(i).Item("Data_Fine_Comm")) < Data Then
                                        FormulatoValido = False
                                    End If

                                Else
                                    '4. Se la data fine commercializzazione non è compilata: 
                                    'guardare Data di revoca e Data di sospensione : 
                                    'se è compilato più di un campo, guardare prima quella con la data più avanti nel tempo.

                                    'REVOCATO
                                    If Not IsDBNull(Dt.Rows(i).Item("revocato")) AndAlso Dt.Rows(i).Item("revocato") <> 0 Then

                                        If Not IsDBNull(Dt.Rows(i).Item("data_revo")) AndAlso IsDate(Dt.Rows(i).Item("data_revo")) Then
                                            If CDate(Dt.Rows(i).Item("data_revo")) < Data Then
                                                FormulatoValido = False
                                            End If
                                        Else
                                            FormulatoValido = False
                                        End If

                                    Else
                                        'PEZZO DELLA SOSPENSIONE: CANCELLATO
                                        Dim DEBUG As Boolean = True
                                    End If

                                End If

                            End If


                        End If

                    End If

                    If FormulatoValido = True Then
                        Dt_New.ImportRow(Dt.Rows(i))
                    End If

                Next

            End If

            Return Dt_New

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Return Nothing
            'Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

    End Function



    ''###############################################################################
    ''A differenza della precedente 
    ''filtra le date di sospensione nella nuova tabella FormulatiXPeriodoSospensione
    'Public Function Filtra_Formulati_Bio(ByVal Dt As DataTable, _
    '                                     ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
    '                                    ) As DataTable

    '    '1. Data fine uso scorte
    '    '2. Se la data fine uso scorte non è compilata: guardare Data fine commercializzazione
    '    '3. Se la data fine commercializzazione non è compilata: guardare Data di revoca è Data di sospensione: se è compilato più di un campo, guardare prima quella con la data più avanti nel tempo.

    '    Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.Formulati_R.Filtra_Formulati_Bio()"
    '    Dim MessaggioErrore As String = ""

    '    Dim Messaggio As String = ""
    '    Dim i As Integer

    '    Dim FormulatoValido As Boolean

    '    Dim Revocato As Integer = 0
    '    Dim Sospeso As Integer = 0
    '    Dim Terminato As Integer = 0

    '    Try


    '        Dim Dt_New As New DataTable

    '        Dt_New = Dt.Clone

    '        If Messaggio = "" Then

    '            For i = 0 To Dt.Rows.Count - 1

    '                FormulatoValido = True

    '                '1. Data fine uso scorte
    '                If Not IsDBNull(Dt.Rows(i).Item("data_fine_usoscorte")) AndAlso IsDate(Dt.Rows(i).Item("data_fine_usoscorte")) Then

    '                    If CDate(Dt.Rows(i).Item("data_fine_usoscorte")) < Data Then
    '                        FormulatoValido = False
    '                    End If

    '                Else

    '                    '2. Se la data fine uso scorte non è compilata: guardare Data fine commercializzazione
    '                    If Not IsDBNull(Dt.Rows(i).Item("Data_Fine_Comm")) AndAlso IsDate(Dt.Rows(i).Item("Data_Fine_Comm")) Then

    '                        If CDate(Dt.Rows(i).Item("Data_Fine_Comm")) < Data Then
    '                            FormulatoValido = False
    '                        End If

    '                    Else
    '                        '3. Se la data fine commercializzazione non è compilata: 
    '                        'guardare Data di revoca è Data di sospensione è Data di Termine: 
    '                        'se è compilato più di un campo, guardare prima quella con la data più avanti nel tempo.

    '                        'REVOCATO
    '                        If Not IsDBNull(Dt.Rows(i).Item("revocato")) AndAlso Dt.Rows(i).Item("revocato") <> 0 Then

    '                            If Not IsDBNull(Dt.Rows(i).Item("data_revo")) AndAlso IsDate(Dt.Rows(i).Item("data_revo")) Then
    '                                If CDate(Dt.Rows(i).Item("data_revo")) < Data Then
    '                                    FormulatoValido = False
    '                                End If
    '                            Else
    '                                FormulatoValido = False
    '                            End If

    '                        Else

    '                            ''TERMINATO
    '                            'If Not IsDBNull(Dt.Rows(i).Item("termine")) AndAlso Dt.Rows(i).Item("termine") <> 0 Then

    '                            '    If Not IsDBNull(Dt.Rows(i).Item("data_term")) AndAlso IsDate(Dt.Rows(i).Item("data_term")) Then

    '                            '        If CDate(Dt.Rows(i).Item("data_term")) < Data Then
    '                            '            FormulatoValido = False
    '                            '        End If

    '                            '    Else

    '                            '        FormulatoValido = False

    '                            '    End If

    '                            'Else

    '                            'SOSPESO
    '                            If Not IsDBNull(Dt.Rows(i).Item("DataSospensioneDA")) And Not IsDBNull(Dt.Rows(i).Item("DataSospensioneA")) Then
    '                                If CDate(Dt.Rows(i).Item("DataSospensioneDA")) < Data And _
    '                                    CDate(Dt.Rows(i).Item("DataSospensioneA")) > Data Then
    '                                    FormulatoValido = False
    '                                End If
    '                            ElseIf Not IsDBNull(Dt.Rows(i).Item("DataSospensioneDA")) And IsDBNull(Dt.Rows(i).Item("DataSospensioneA")) Then
    '                                If CDate(Dt.Rows(i).Item("DataSospensioneDA")) < Data Then
    '                                    FormulatoValido = False
    '                                End If
    '                            ElseIf IsDBNull(Dt.Rows(i).Item("DataSospensioneDA")) And Not IsDBNull(Dt.Rows(i).Item("DataSospensioneA")) Then
    '                                If CDate(Dt.Rows(i).Item("DataSospensioneA")) > Data Then
    '                                    FormulatoValido = False
    '                                End If
    '                            End If


    '                            'End If

    '                        End If


    '                    End If

    '                End If

    '                If FormulatoValido = True Then
    '                    Dt_New.ImportRow(Dt.Rows(i))
    '                End If

    '            Next

    '        End If

    '        Return Dt_New

    '    Catch ex As Exception
    '        MessaggioErrore = ex.Message
    '        Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
    '        Return Nothing
    '        'Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
    '    End Try

    'End Function

    '################################################################################
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Nuova versione senza COM+ e con objParametri
    ''' </summary>
    ''' <param name="FrCod"></param>
    ''' <param name="objParametri"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[pierantoni]	14/01/2011	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Function FrDes_from_FrCod( _
                                    ByVal FrCod As Integer, _
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As String

        Dim Dt As DataTable

        Dim objFormulati As New AgronicaCoreMetaSchemaDAL.Formulati_R

        'Recupero le informazioni		
        Dt = objFormulati.Leggi(CInt(FrCod), _
                                objParametri.FinestraTemporaleInizio, _
                                objParametri.FinestraTemporaleFine, _
                                enumSelezioneVariabile.Selezione_TabellaCompleta, _
                                "", "", objParametri)

        'Elimino gli oggetti COM
        objFormulati = Nothing

        'Se il recordset non è chiuso allora ...	
        If Not Dt Is Nothing AndAlso Dt.Rows.Count > 0 Then

            Return Dt.Rows(0).Item("Fr_Des")

        End If

        'Elimino il recordset
        Dt = Nothing

    End Function


    '################################################################################
    Public Function NewCLTossCod_from_FrCod(ByVal FrCod As Integer, _
                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As String

        Dim Dt As DataTable

        Dt = Leggi(CInt(FrCod), _
                    objParametri.FinestraTemporaleInizio, _
                    objParametri.FinestraTemporaleFine, _
                    enumSelezioneVariabile.Selezione_TabellaCompleta, _
                    "", "", _
                    objParametri)

        If Not Dt Is Nothing AndAlso Dt.Rows.Count > 0 Then
            Return Dt.Rows(0).Item("NewCLTOSS_COD")
        End If

        Dt = Nothing

    End Function




    '################################################################################
    'usata dall'importatore di magazzino di agrisol e da quello di terremerse
    Public Function Formulati_NoNumRegistrazione(ByVal Descr_Formulato As String, _
                                                ByVal xFiltroAggiuntivo As String, _
                                                ByVal xOrderBy As String, _
                                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                                    ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.Formulati_R.Formulati_NoNumRegistrazione()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.Append(" SELECT      Formulati.Fr_Cod, Formulati.Fr_Des  " & vbCrLf)
            StrSQL.Append(" FROM        Formulati " & vbCrLf)
            StrSQL.Append(" WHERE       (Formulati.Fr_Des LIKE '%" + Agro_SQL_SaveText(Descr_Formulato) + "%')       " & vbCrLf)
            StrSQL.Append(" AND         (Formulati.Fr_Cod > 1000000 ) " & vbCrLf)
            StrSQL.Append(" AND         Formulati.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " " & vbCrLf)
            StrSQL.Append(" AND         Formulati.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " " & vbCrLf)

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY Formulati.FR_DES ASC ")
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


    '###############################################################################################
    'usata dall'importatore di magazzino di agrisol e da quello di terremerse
    Public Sub Recupera_FrCod_NoNumRegistrazione(ByVal Descr_Formulato As String, _
                                                ByRef Fr_Cod As Integer, _
                                                ByRef Fr_Des As String, _
                                                ByRef Num_FormulatiTrovati As Integer, _
                                                ByVal xFiltroAggiuntivo As String, _
                                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri)

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.Formulati_R.Recupera_FrCod_NoNumRegistrazione()"

        Dim MessaggioErrore As String
        Dim DT As DataTable

        Fr_Cod = 0
        Fr_Des = ""

        Try

            DT = Formulati_NoNumRegistrazione(Descr_Formulato, _
                                               xFiltroAggiuntivo, _
                                               "", _
                                              objParametri)

            If Not IsNothing(DT) Then

                Num_FormulatiTrovati = DT.Rows.Count

                If Num_FormulatiTrovati <> 0 Then
                    Fr_Cod = DT.Rows(0).Item("Fr_Cod")
                    Fr_Des = DT.Rows(0).Item("Fr_Des")
                End If
            End If

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

    End Sub


    Public Function Esiste_Formulato(ByVal FR_COD As Integer, _
                                   ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                          ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.Formulati_R.Esiste_Formulato()"


        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Dim Esiste As Boolean = False

        Try

            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.Append(" SELECT Fr_cod, Fr_Des FROM  Formulati " & _
                          " WHERE Formulati.FR_COD =  " & Agro_SQL_SaveNum(FR_COD) & "  ")

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            If Not DT Is Nothing AndAlso DT.Rows.Count > 0 Then
                Esiste = True
            End If

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return Esiste

    End Function

    Public Function Verifica_Revoca_Formulato(ByVal fr_cod As Integer,
                                              ByVal veg_cod As Integer,
                                              ByVal data As Date,
                                              ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                              ) As String

        '1. Data fine uso scorte
        '2. Se la data fine uso scorte non è compilata: guardare Data fine commercializzazione
        '3. Se la data fine commercializzazione non è compilata: guardare Data di revoca è Data di sospensione: se è compilato più di un campo, guardare prima quella con la data più avanti nel tempo.

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.Formulati_R.Verifica_Revoca_Formulato()"
        Dim MessaggioErrore As String = ""

        Dim j As Integer

        Dim formulato As DataRow = Nothing
        Dim fr_des As String = ""

        Dim messaggioRevoca As String = ""

        Try
            Dim Dt As DataTable = Leggi(fr_cod,
                                        objParametri.FinestraTemporaleInizio,
                                        objParametri.FinestraTemporaleFine,
                                        enumSelezioneVariabile.Selezione_TabellaCompleta,
                                        "", "",
                                        objParametri)

            If Dt.Rows.Count = 1 Then
                formulato = Dt.Rows(0)
                fr_des = formulato.Item("fr_des")
            End If

            If formulato IsNot Nothing Then
                'estraggo i sospesi
                Dim DtSosp As DataTable
                Dim DrSosp() As DataRow
                DtSosp = LeggiSospesi(fr_cod, AGRODATAINIZIO, AGRODATAFINE, "", "", objParametri)

                '1. Data Registrazione formulato
                If Not IsDBNull(formulato.Item("data_reg")) AndAlso IsDate(formulato.Item("data_reg")) Then

                    If CDate(formulato.Item("data_reg")) > data Then

                        messaggioRevoca = String.Format("Il prodotto {0} non può essere utilizzato prima del {1} (data registrazione)", fr_des, CDate(formulato.Item("data_reg")).ToShortDateString())

                    Else

                        '2. Data fine uso scorte
                        If Not IsDBNull(formulato.Item("data_fine_usoscorte")) AndAlso IsDate(formulato.Item("data_fine_usoscorte")) Then

                            If CDate(formulato.Item("data_fine_usoscorte")) < data Then
                                messaggioRevoca = String.Format("L'utilizzo delle scorte del prodotto {0} è terminato in data {1}", fr_des, CDate(formulato.Item("data_fine_usoscorte")).ToShortDateString())
                            End If

                        Else

                            '3. Se la data fine uso scorte non è compilata: guardare Data fine commercializzazione
                            If Not IsDBNull(formulato.Item("Data_Fine_Comm")) AndAlso IsDate(formulato.Item("Data_Fine_Comm")) Then

                                If CDate(formulato.Item("Data_Fine_Comm")) < data Then
                                    messaggioRevoca = String.Format("La commercializzazione del prodotto {0} è terminata in data {1}", fr_des, CDate(formulato.Item("Data_Fine_Comm")).ToShortDateString())
                                End If

                            Else

                                '4. Se la data fine commercializzazione non è compilata: 
                                'guardare Data di revoca e Data di sospensione : 
                                'se è compilato più di un campo, guardare prima quella con la data più avanti nel tempo.

                                'REVOCATO
                                If Not IsDBNull(formulato.Item("revocato")) AndAlso formulato.Item("revocato") <> 0 Then

                                    If Not IsDBNull(formulato.Item("data_revo")) AndAlso IsDate(formulato.Item("data_revo")) Then

                                        If CDate(formulato.Item("data_revo")) < data Then
                                            messaggioRevoca = String.Format("L'utilizzo del prodotto {0} è stato revocato in data {1}", fr_des, CDate(formulato.Item("data_revo")).ToShortDateString())
                                        End If

                                    Else
                                        messaggioRevoca = String.Format("L'utilizzo del prodotto {0} è stato revocato", fr_des)
                                    End If

                                Else

                                    '5. SOSPESO
                                    DrSosp = DtSosp.Select("Fr_Cod=" & formulato.Item("Fr_Cod"))

                                    If Not DrSosp Is Nothing AndAlso DrSosp.Length > 0 Then
                                        For j = 0 To DrSosp.Length - 1
                                            If Not IsDBNull(DrSosp(j).Item("DataSospensioneDA")) And Not IsDBNull(DrSosp(j).Item("DataSospensioneA")) Then

                                                If CDate(DrSosp(j).Item("DataSospensioneDA")) < data And
                                                        CDate(DrSosp(j).Item("DataSospensioneA")) > data Then
                                                    messaggioRevoca = String.Format("L'utilizzo del prodotto {0} è sospeso dal {1} al {2}", fr_des, CDate(DrSosp(j).Item("DataSospensioneDA")).ToShortDateString(), CDate(DrSosp(j).Item("DataSospensioneA")).ToShortDateString())
                                                    Exit For
                                                End If

                                            ElseIf Not IsDBNull(DrSosp(j).Item("DataSospensioneDA")) And IsDBNull(DrSosp(j).Item("DataSospensioneA")) Then

                                                If CDate(DrSosp(j).Item("DataSospensioneDA")) < data Then
                                                    messaggioRevoca = String.Format("L'utilizzo del prodotto {0} è sospeso dal {1}", fr_des, CDate(DrSosp(j).Item("DataSospensioneDA")).ToShortDateString())
                                                    Exit For
                                                End If

                                            ElseIf IsDBNull(DrSosp(j).Item("DataSospensioneDA")) And Not IsDBNull(DrSosp(j).Item("DataSospensioneA")) Then

                                                If CDate(DrSosp(j).Item("DataSospensioneA")) > data Then
                                                    messaggioRevoca = String.Format("L'utilizzo del prodotto {0} è sospeso fino al {1}", fr_des, CDate(DrSosp(j).Item("DataSospensioneA")).ToShortDateString())
                                                    Exit For
                                                End If

                                            End If
                                        Next
                                    End If
                                End If
                            End If
                        End If
                    End If
                End If
            End If

            Return messaggioRevoca

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Return Nothing
            'Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

    End Function


    ''' <summary>
    ''' In base al tipo formulato (enum_TipoFormulato.InstallazioneTrappoleCattureMassa) viene applicato il filtro per class_cod.
    ''' Se viene passato negativo carico tutti i formulati escludendo il tipo formulato che è stato passato.
    ''' </summary>
    ''' <param name="Tipo"></param>
    ''' <param name="StrSQL"></param>
    Public Sub Filtro_Formulato_Classificazione(ByVal TipiFormulati As String, ByRef StrSQL As Text.StringBuilder, Optional ByVal TabellaClassificazioni As String = "FormulatixClassificazioni")

        If Not String.IsNullOrEmpty(TipiFormulati) Then

            If IsNumeric(TipiFormulati) = True Then

                Filtro_Formulato_Internal(TabellaClassificazioni, CInt(TipiFormulati), "And", StrSQL)

            Else

                Dim TipiFormulatiList As List(Of String) = TipiFormulati.Split(",").ToList()

                If Not IsNothing(TipiFormulatiList) AndAlso TipiFormulatiList.Count > 0 Then

                    StrSQL.Append(" And ( ")

                    For x As Integer = 0 To TipiFormulatiList.Count - 1

                        If IsNumeric(TipiFormulatiList(x)) Then

                            Dim TipoFormulato As Integer = CInt(TipiFormulatiList(x))

                            Dim OperatoreCondizionale As String = ""

                            If x > 0 Then
                                OperatoreCondizionale = "Or"
                            End If

                            StrSQL.Append(OperatoreCondizionale & " (")

                            Filtro_Formulato_Internal(TabellaClassificazioni, TipoFormulato, "", StrSQL)

                            StrSQL.Append(" ) ")
                        End If
                    Next

                    StrSQL.Append(" )")
                End If
            End If

        End If

    End Sub

    Private Sub Filtro_Formulato_Internal(ByVal TabellaClassificazioni As String, ByVal TipoFormulato As Integer, ByVal OperatoreCondizionale As String, ByRef StrSQL As Text.StringBuilder)

        If TipoFormulato > 0 Then
            StrSQL.Append(" " & OperatoreCondizionale & "  " & TabellaClassificazioni & ".CLASS_COD IN (" & String.Join(",", DictionaryTipoFormulatoClassificazione(TipoFormulato)) & ")")
        Else
            Select Case TipoFormulato
                Case -enum_TipoFormulato.InstallazioneTrappoleCattureMassa
                    'Carico tutti i formulati tranne le trappole
                    Dim Formulati_Senza_Trappole = Classificazioni_Tutti_Formulati.FindAll(Function(c) Not Classificazioni_Installazione_Trappole_Catture_Massa.Contains(c))
                    StrSQL.Append(" " & OperatoreCondizionale & "  " & TabellaClassificazioni & ".CLASS_COD IN (" & String.Join(",", Formulati_Senza_Trappole) & ")")
            End Select
        End If

    End Sub

End Class
