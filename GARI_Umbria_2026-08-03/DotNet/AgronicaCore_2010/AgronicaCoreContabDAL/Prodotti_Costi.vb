Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi

Public Class Prodotti_Costi_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################
    Public Function Leggi(ByVal Piva As String,
                          ByVal Elem_Cod As Int32,
                          ByVal Riferimento As String,
                          ByVal Pro_Cod As Int32,
                          ByVal Mat_Cod As Int32,
                          ByVal Veg_Cod As Int32,
                          ByVal Cul_Cod As Int32,
                          ByVal Udm_Cod As Int32,
                          ByVal xSelezioneVariabile As enumSelezioneVariabile,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreParametri,
                          Optional ByVal Id_Budget As Int32 = 0
                          ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Prodotti_Costi_R.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   Riferimento = ""  
        '   Pro_Cod = 0 
        '   Mat_Cod = 0 
        '   Veg_Cod = 0 
        '   Cul_Cod = 0    
        '   Udm_Cod = 0
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try

            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                    StrSQL.Length = 0

                    StrSQL.Append(" SELECT Prodotti_Costi.*  ")
                    StrSQL.Append(" FROM   Prodotti_Costi ")

                    StrSQL.Append(" WHERE  Prodotti_Costi.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND    Prodotti_Costi.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio))

                    'Elem_Cod Obbligatorio (Risorse_Umane --> Elem_Cod = 0)
                    StrSQL.Append(" AND Prodotti_Costi.Elem_Cod = " & Agro_SQL_SaveNum(Elem_Cod) & "  And Prodotti_Costi.Id_Budget = " & Agro_SQL_SaveNum(Id_Budget) & " ")

                    If Riferimento <> "" Then
                        StrSQL.Append(" AND Prodotti_Costi.Riferimento = '" & Agro_SQL_SaveText(Riferimento) & "'   ")
                    End If

                    If Pro_Cod <> 0 Then
                        StrSQL.Append(" AND Prodotti_Costi.Pro_Cod = " & Agro_SQL_SaveNum(Pro_Cod) & "   ")
                    End If

                    If Mat_Cod <> 0 Then
                        StrSQL.Append(" AND Prodotti_Costi.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & "   ")
                    End If

                    If Veg_Cod <> 0 Then
                        StrSQL.Append(" AND Prodotti_Costi.Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod) & "   ")
                    End If

                    If Cul_Cod <> 0 Then
                        StrSQL.Append(" AND Prodotti_Costi.Cul_Cod = " & Agro_SQL_SaveNum(Cul_Cod) & "   ")
                    End If

                    If Udm_Cod <> 0 Then
                        StrSQL.Append(" AND Prodotti_Costi.Udm_Cod = " & Agro_SQL_SaveNum(Udm_Cod) & "   ")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   Prodotti_Costi.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Prodotti_Costi.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    End If


                Case enumSelezioneVariabile.Selezione_TabellaCompleta

                    '------------------------------------------------------------------
                    StrSQL.Length = 0

                    StrSQL.Append(" SELECT Prodotti_Costi.* , Imprese.Rag_Soc as Referente , UnitaMisura.Udm_Des, UnitaMisura.Udm_Sim ")
                    StrSQL.Append(" FROM   Prodotti_Costi ")

                    StrSQL.Append(" LEFT OUTER JOIN Imprese ON Prodotti_Costi.Piva = Imprese.Piva ")
                    StrSQL.Append(" LEFT OUTER JOIN UnitaMisura ON Prodotti_Costi.Udm_Cod = UnitaMisura.Udm_Cod ")

                    StrSQL.Append(" WHERE  Prodotti_Costi.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND    Prodotti_Costi.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio))

                    'Elem_Cod Obbligatorio (Risorse_Umane --> Elem_Cod = 0)
                    StrSQL.Append(" AND Prodotti_Costi.Elem_Cod = " & Agro_SQL_SaveNum(Elem_Cod) & " And Prodotti_Costi.Id_Budget = " & Agro_SQL_SaveNum(Id_Budget) & " ")

                    If Riferimento <> "" Then
                        StrSQL.Append(" AND Prodotti_Costi.Riferimento = '" & Agro_SQL_SaveText(Riferimento) & "'   ")
                    End If

                    If Pro_Cod <> 0 Then
                        StrSQL.Append(" AND Prodotti_Costi.Pro_Cod = " & Agro_SQL_SaveNum(Pro_Cod) & "   ")
                    End If

                    If Mat_Cod <> 0 Then
                        StrSQL.Append(" AND Prodotti_Costi.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & "   ")
                    End If

                    If Veg_Cod <> 0 Then
                        StrSQL.Append(" AND Prodotti_Costi.Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod) & "   ")
                    End If

                    If Cul_Cod <> 0 Then
                        StrSQL.Append(" AND Prodotti_Costi.Cul_Cod = " & Agro_SQL_SaveNum(Cul_Cod) & "   ")
                    End If

                    If Udm_Cod <> 0 Then
                        StrSQL.Append(" AND Prodotti_Costi.Udm_Cod = " & Agro_SQL_SaveNum(Udm_Cod) & "   ")
                    End If


                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   Prodotti_Costi.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Prodotti_Costi.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY Prodotti_Costi.Riferimento, Prodotti_Costi.Elem_Cod,  Prodotti_Costi.Pro_Cod,  Prodotti_Costi.Mat_Cod,  Prodotti_Costi.Udm_Cod,  Prodotti_Costi.Veg_Cod,  Prodotti_Costi.Cul_Cod  ASC ")
                    End If

                Case enumSelezioneVariabile.Selezione_JoinDescrizioni


                Case enumSelezioneVariabile.Selezione_JoinCompleta


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

    Public Function Leggi_Macchine(ByVal Piva As String,
                                   ByVal Mat_Cod As Int32,
                                   ByVal xSelezioneVariabile As enumSelezioneVariabile,
                                   ByVal xFiltroAggiuntivo As String,
                                   ByVal xOrderBy As String,
                                   ByRef objParametri As AgronicaCoreParametri,
                                   Optional ByVal Id_Budget As Integer = 0
                                   ) As DataTable

        Const nomeRoutine As String = "AgronicaCoreContabDAL.Prodotti_Costi_R.Leggi_Macchine()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try

            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi
                    StrSQL.Length = 0

                    StrSQL.AppendLine(" SELECT Prodotti_Costi.* , Imprese.Rag_Soc as Referente , ")

                    StrSQL.AppendLine(" CASE ")
                    StrSQL.AppendLine("     WHEN Mezzo = 1 THEN 'Ettaro' ")
                    StrSQL.AppendLine("     WHEN Mezzo = 2 THEN 'Ora' ")
                    StrSQL.AppendLine("     ELSE '' ")
                    StrSQL.AppendLine(" END AS Udm_Des,")

                    StrSQL.AppendLine(" CASE ")
                    StrSQL.AppendLine("     WHEN Mezzo = 1 THEN 'Ettaro' ")
                    StrSQL.AppendLine("     WHEN Mezzo = 2 THEN 'Ora' ")
                    StrSQL.AppendLine("     ELSE '' ")
                    StrSQL.AppendLine(" END AS Udm_Sin")

                    StrSQL.AppendLine(" FROM   Prodotti_Costi ")

                    StrSQL.AppendLine(" LEFT OUTER JOIN Imprese ON Prodotti_Costi.Piva = Imprese.Piva ")
                    StrSQL.AppendLine(" LEFT OUTER JOIN UnitaMisura ON Prodotti_Costi.Udm_Cod = UnitaMisura.Udm_Cod ")

                    StrSQL.AppendLine(" WHERE  Prodotti_Costi.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.AppendLine(" AND    Prodotti_Costi.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.AppendLine(" And Prodotti_Costi.Id_Budget = " & Agro_SQL_SaveNum(Id_Budget))
                    StrSQL.AppendLine(" AND    Prodotti_Costi.Elem_Cod = " & Agro_SQL_SaveNum(MACCHINE) & "   ")

                    If Mat_Cod <> 0 Then
                        StrSQL.AppendLine(" AND Prodotti_Costi.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & "   ")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.AppendLine(" AND   Prodotti_Costi.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.AppendLine(" AND   Prodotti_Costi.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.AppendLine(" ORDER BY Prodotti_Costi.Riferimento, Prodotti_Costi.Elem_Cod,  Prodotti_Costi.Pro_Cod,  Prodotti_Costi.Mat_Cod,  Prodotti_Costi.Udm_Cod,  Prodotti_Costi.Veg_Cod,  Prodotti_Costi.Cul_Cod  ASC ")
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

    '##############################################################
    'per non filtrare il sa_cod, passare il valore SACOD_NOFILTRO
    'perché 0 è significativo
    Public Function LeggiCostiParcoMacchine(ByVal Piva As String,
                                            ByVal Mac_Cod As Int32,
                                            ByVal Sa_Cod As Int32,
                                            ByVal xFiltroAggiuntivo As String,
                                            ByVal xOrderBy As String,
                                            ByRef objParametri As AgronicaCoreParametri,
                                            Optional ByVal Id_Budget As Int32 = 0
                                            ) As DataTable

        Const nomeRoutine As String = "AgronicaCoreContabDAL.Prodotti_Costi_R.LeggiCostiParcoMacchine()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.Append(" SELECT Prodotti_Costi.*  ")
            StrSQL.Append(" FROM   Prodotti_Costi ")
            StrSQL.Append(" INNER JOIN Parco_Macchine ")
            StrSQL.Append(" ON Prodotti_Costi.piva = Parco_Macchine.piva AND Prodotti_Costi.Mat_Cod = Parco_Macchine.Mac_Cod ")

            StrSQL.Append(" WHERE  Prodotti_Costi.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.Append(" AND    Prodotti_Costi.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio))

            StrSQL.Append(" AND Prodotti_Costi.Elem_Cod = " & Agro_SQL_SaveNum(MACCHINE) & " And Prodotti_Costi.Id_Budget = " & Agro_SQL_SaveNum(Id_Budget) & " ")

            If Mac_Cod <> 0 Then
                StrSQL.Append(" AND Prodotti_Costi.Mat_Cod = " & Agro_SQL_SaveNum(Mac_Cod) & "   ")
            End If

            If Piva <> "" Then
                StrSQL.Append(" AND Prodotti_Costi.piva = '" & Agro_SQL_SaveText(Piva) & "'  ")
            End If

            If Sa_Cod <> SACOD_NOFILTRO Then
                StrSQL.Append(" AND    Parco_Macchine.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   " & vbCrLf)
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   Prodotti_Costi.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   Prodotti_Costi.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
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


    '#######################################################################################################
    Public Sub Prezzo_from_Prodotto(ByVal Piva As String,
                                    ByVal Elem_Cod As Integer,
                                    ByVal Pro_Cod As Integer,
                                    ByVal Mat_Cod As Integer,
                                    ByRef Data As Date,
                                    ByRef Mezzo As Integer,
                                    ByRef Udm_Cod As Integer,
                                    ByRef Prezzo As Decimal,
                                    ByVal xFiltroAggiuntivo As String,
                                    ByRef objParametri As AgronicaCoreParametri)

        Dim dtProdotti As DataTable

        Dim app_inizio As Date = objParametri.FinestraTemporaleInizio
        Dim app_fine As Date = objParametri.FinestraTemporaleFine

        objParametri.FinestraTemporaleFine = Data
        objParametri.FinestraTemporaleInizio = Data

        dtProdotti = Leggi(CStr(Piva),
                           CInt(Elem_Cod),
                           "",
                           CInt(Pro_Cod),
                           CInt(Mat_Cod),
                           0,
                           0,
                           CInt(Udm_Cod),
                           enumSelezioneVariabile.Selezione_TabellaCompleta,
                           xFiltroAggiuntivo, "", objParametri)

        objParametri.FinestraTemporaleFine = app_fine
        objParametri.FinestraTemporaleInizio = app_inizio

        If dtProdotti.Rows.Count > 0 Then
            Mezzo = dtProdotti.Rows(0).Item("Mezzo")
            Prezzo = dtProdotti.Rows(0).Item("Prezzo_Unitario")
            Udm_Cod = dtProdotti.Rows(0).Item("Udm_Cod")
        Else
            Mezzo = -1
            Prezzo = 0.0
            Udm_Cod = 0
        End If

    End Sub


    '#######################################################################################################
    Public Function Prezzo_Unitario_DaListino(ByVal Piva As String,
                                              ByVal Riferimento As String,
                                              ByVal Elem_Cod As Long,
                                              ByVal Pro_Cod As Long,
                                              ByVal Mat_Cod As Long,
                                              ByVal VEG_COD As Long,
                                              ByVal Cul_Cod As Long,
                                              ByVal Udm_Cod As Long,
                                              ByRef Mezzo As Integer,
                                              ByVal Data As Date,
                                              ByVal xFiltroAggiuntivo As String,
                                              ByRef objParametri As AgronicaCoreParametri
                                              ) As String

        Dim dtProdotti As DataTable

        'modifico la data 
        Dim app_inizio As Date = objParametri.FinestraTemporaleInizio
        Dim app_fine As Date = objParametri.FinestraTemporaleFine

        objParametri.FinestraTemporaleFine = Data
        objParametri.FinestraTemporaleInizio = Data

        dtProdotti = Leggi(CStr(Piva),
                           CInt(Elem_Cod),
                           CStr(Riferimento),
                           CInt(Pro_Cod),
                           CInt(Mat_Cod),
                           CInt(VEG_COD),
                           CInt(Cul_Cod),
                           CInt(Udm_Cod),
                           enumSelezioneVariabile.Selezione_TabellaCompleta,
                           xFiltroAggiuntivo, "", objParametri)

        objParametri.FinestraTemporaleFine = app_fine
        objParametri.FinestraTemporaleInizio = app_inizio

        Dim prezzoUnitarioDaListino As String = "Indefinito" 'Inizializzazione
        Mezzo = 1 'Inizializzazione

        If dtProdotti.Rows.Count > 0 Then
            If dtProdotti.Rows(0).Item("Prezzo_Unitario") <> 0 Then
                prezzoUnitarioDaListino = Format(CDbl(dtProdotti.Rows(0).Item("Prezzo_Unitario")), "0.00")
                Mezzo = dtProdotti.Rows(0).Item("Mezzo")
            Else
                prezzoUnitarioDaListino = "0"
            End If
        Else
            prezzoUnitarioDaListino = "0"
        End If

        Return prezzoUnitarioDaListino

    End Function

    Public Function Leggi_Costi_Completa(ByVal piva As String,
                                         ByVal leggiPersone As Boolean,
                                         ByVal leggiMacchine As Boolean,
                                         ByVal leggiProdotti As Boolean,
                                         ByVal xFiltroAggiuntivo As String,
                                         ByVal xOrderBy As String,
                                         ByRef objParametri As AgronicaCoreParametri,
                                         Optional ByVal idBudget As Integer = 0
                                         ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Prodotti_Costi_R.Leggi_Costi_Completa()"

        '====================================================================================
        'Parametri opzionali :
        ' - IdBudget = 0 ==> Solo costi a consuntivo
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try

            If Not leggiPersone AndAlso Not leggiMacchine AndAlso Not leggiProdotti Then
                Throw new Exception("Parametri non validi: indicare almeno uno tra: leggiPersone, leggiMacchine, leggiProdotti")
            End If


            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT pc.ID, pc.Piva AS Piva_Prodotti_Costi, i.rag_soc AS Azienda ")
            StrSQL.AppendLine(" , ISNULL(pc.Id_Budget, 0) As Id_Budget, ISNULL(bt.Nome_Budget, '') AS Nome_Budget, bt.Revisione ")
            StrSQL.AppendLine(" , pc.Elem_Cod, CASE WHEN pc.Elem_Cod = 0 THEN 'Risorsa Umana' ELSE cm.NomeComune END AS NomeComune ")
            StrSQL.AppendLine(" , pc.Pro_Cod, pc.Mat_Cod ")

            If leggiPersone Then
                StrSQL.AppendLine(" , ISNULL(Contatti.Piva, '') AS Piva_Contatto, ISNULL(Contatti.Sa_Cod, 0) AS Sa_Cod_Contatto ")
                StrSQL.AppendLine(" , ISNULL(Contatti.Cod_Contatto, '') AS Cod_Contatto ")
                StrSQL.AppendLine(" , LTRIM(RTRIM(ISNULL(Contatti.Rag_Soc, '') + ISNULL(Contatti.Nome, '') + ' ' + ISNULL(Contatti.Cognome, ''))) AS Des_Contatto ")
                StrSQL.AppendLine(" , ISNULL(Risorse_Umane.Cod_RisUm, 0) AS Cod_RisUm ")
                StrSQL.AppendLine(" , ISNULL(Risorse_Umane.Cod_Rapporto, 0) AS Cod_Rapporto, ISNULL(Rapporti_Contabili.Rapporto_Des, '') AS Rapporto_Des ")
            Else
                StrSQL.AppendLine(" , '' AS Piva_Contatto, 0 AS Sa_Cod_Contatto ")
                StrSQL.AppendLine(" , '' AS Cod_Contatto, '' AS Des_Contatto, 0 AS Cod_RisUm ")
                StrSQL.AppendLine(" , 0 AS Cod_Rapporto, '' AS Rapporto_Des ")
            End If

            If leggiMacchine Then
                StrSQL.AppendLine(" , ISNULL(Parco_Macchine.Piva, '') AS Piva_Macchina, ISNULL(Parco_Macchine.Sa_Cod, 0) AS Sa_Cod_Macchina ")
                StrSQL.AppendLine(" , ISNULL(Parco_Macchine.Tipo, 0) AS Tipo ")
                StrSQL.AppendLine(" , CASE Parco_Macchine.Tipo WHEN 0 THEN 'Agricola/Zootecnica' WHEN 1 THEN 'Industriale' WHEN 2 THEN 'Commerciale' ELSE '' END AS Tipo_Des ")
                StrSQL.AppendLine(" , ISNULL(Parco_Macchine.Class_Code, '') AS Class_Code ")
                StrSQL.AppendLine(" , ISNULL(Macchine.Class_Code, '0') AS Class_Code_Root, ISNULL(Macchine.Class_Desc, '') AS Class_Desc ")
                StrSQL.AppendLine(" , ISNULL(Parco_Macchine.Mac_Cod, 0) AS Mac_Cod, ISNULL(Parco_Macchine.Mac_Des, '') AS Mac_Des ")
            Else
                StrSQL.AppendLine(" , '' AS Piva_Macchina, 0 AS Sa_Cod_Macchina ")
                StrSQL.AppendLine(" , 0 AS Tipo, '' AS Tipo_Des, '' AS Class_Code, '0' AS Class_Code_Root, '' AS Class_Desc ")
                StrSQL.AppendLine(" , 0 AS Mac_Cod, '' AS Mac_Des ")
            End If

            If leggiProdotti Then
                StrSQL.AppendLine(" , ISNULL(Materie_Prime.Piva, '') AS Piva_Prodotto, ISNULL(Materie_Prime.Sa_Cod, 0) AS Sa_Cod_prodotto ")
                StrSQL.AppendLine(" , CASE ")
                StrSQL.AppendLine("     WHEN pc.Elem_Cod > 1 AND pc.Pro_Cod <> 0 THEN pc.Pro_Cod ")
                StrSQL.AppendLine("     WHEN pc.Elem_Cod > 1 AND pc.Mat_Cod <> 0 THEN -1 * pc.Mat_Cod ")
                StrSQL.AppendLine("     ELSE 0 END AS Prodotto_Cod ")
                StrSQL.AppendLine(" , COALESCE(Materie_Prime.Mat_Des, Fertilizzanti.Fer_Des, Formulati.Fr_Des, InsettiUtili.Ins_Des, Trappole.Trap_Des, Avversita.Av_Des_Vol, '') AS Prodotto_Des ")
                StrSQL.AppendLine(" , ISNULL(Materie_Prime.Cod_Articolo, '') AS Cod_Articolo ")
            Else
                StrSQL.AppendLine(" , '' AS Piva_Prodotto, 0 AS Sa_Cod_prodotto ")
                StrSQL.AppendLine(" , 0 AS Prodotto_Cod, '' AS Prodotto_Des, '' AS Cod_Articolo ")
            End If
   
            StrSQL.AppendLine(" , pc.Mezzo, pc.Udm_Cod ")
            StrSQL.AppendLine(" , CASE ")
            StrSQL.AppendLine("     WHEN pc.Elem_Cod > 1 THEN um.UDM_DES ")
            StrSQL.AppendLine("     ELSE CASE ")
            StrSQL.AppendLine("             WHEN Mezzo = 1 THEN 'Ettaro' ")
            StrSQL.AppendLine("             WHEN Mezzo = 2 THEN 'Ora' ")
            StrSQL.AppendLine("             ELSE '' END ")
            StrSQL.AppendLine("     END AS Udm_Des ")
            StrSQL.AppendLine(" , CASE ")
            StrSQL.AppendLine("     WHEN pc.Elem_Cod > 1 THEN um.UDM_SIM ")
            StrSQL.AppendLine("     ELSE CASE ")
            StrSQL.AppendLine("             WHEN Mezzo = 1 THEN 'Ettaro' ")
            StrSQL.AppendLine("             WHEN Mezzo = 2 THEN 'Ora' ")
            StrSQL.AppendLine("             ELSE '' END ")
            StrSQL.AppendLine("     END AS Udm_Sim ")
            StrSQL.AppendLine(" , pc.Prezzo_Unitario, pc.Validita_Inizio, pc.Validita_Fine ")
            StrSQL.AppendLine(" , pc.Riferimento, pc.Veg_Cod, pc.Cul_Cod ")
            StrSQL.AppendLine(" , pc.Data_Creazione, pc.Data_Modifica, pc.Username_Creazione, pc.Username_Modifica ")

            StrSQL.AppendLine(" FROM Prodotti_Costi pc ")
            StrSQL.AppendLine(" INNER JOIN Imprese i ON pc.Piva = i.Piva ")
            StrSQL.AppendLine(" LEFT JOIN UnitaMisura um ON pc.Udm_Cod = um.UDM_COD ")
            StrSQL.AppendLine(" LEFT JOIN Budget_Testata bt ON pc.Id_Budget = bt.Id_Budget ")
            StrSQL.AppendLine(" LEFT JOIN CategorieMagazzino cm ON pc.Elem_Cod = cm.Elem_Cod ")

            If leggiPersone Then
                StrSQL.AppendLine(" LEFT JOIN Risorse_Umane ON pc.Mat_Cod = Risorse_Umane.Cod_RisUm AND pc.Elem_Cod = 0 ")
                StrSQL.AppendLine(" LEFT JOIN Contatti ON Risorse_Umane.Piva = Contatti.Piva AND Risorse_Umane.Cod_Contatto = Contatti.Cod_Contatto ")
                StrSQL.AppendLine(" LEFT JOIN Rapporti_Contabili ON Risorse_Umane.Cod_Rapporto = Rapporti_Contabili.Cod_Rapporto ")
            End If

            If leggiMacchine Then
                StrSQL.AppendLine(" LEFT JOIN Parco_Macchine ON pc.Mat_Cod = Parco_Macchine.Mac_Cod AND pc.Elem_Cod = 1 ")
                StrSQL.AppendLine(" LEFT OUTER JOIN Macchine ON LEFT(Parco_Macchine.Class_Code + '00', 2) = Macchine.Class_Code ")
            End If

            If leggiProdotti Then
                StrSQL.AppendLine(" LEFT JOIN Materie_Prime ON pc.Elem_Cod = Materie_Prime.Elem_Cod AND pc.Mat_Cod = Materie_Prime.Mat_Cod ")
                StrSQL.AppendLine("      AND pc.Elem_Cod NOT IN (0,1,3,191,196,197,198) ")
                StrSQL.AppendLine(" LEFT JOIN Fertilizzanti ON Fertilizzanti.Fer_Cod = pc.Pro_Cod AND pc.Elem_Cod = 3 ")
                StrSQL.AppendLine(" LEFT JOIN Formulati ON Formulati.Fr_Cod = pc.Pro_Cod AND pc.Elem_Cod = 191 ")
                StrSQL.AppendLine(" LEFT JOIN InsettiUtili ON InsettiUtili.Ins_Cod = pc.Pro_Cod AND pc.Elem_Cod = 196 ")
                StrSQL.AppendLine(" LEFT JOIN Trappole ON Trappole.Trap_Cod = pc.Pro_Cod AND pc.Elem_Cod = 197 ")
                StrSQL.AppendLine(" LEFT JOIN Avversita ON Avversita.Av_Cod = pc.Pro_Cod AND pc.Elem_Cod = 198 ")
            End If

            StrSQL.AppendLine(" WHERE 1 = 1 ")
            StrSQL.AppendLine(" AND pc.Id_Budget = " & Agro_SQL_SaveNum(idBudget))

            If Piva <> "" Then
                StrSQL.AppendLine(" AND (pc.Piva = '" & Agro_SQL_SaveText(piva) & "' ")
                If leggiPersone Then
                    StrSQL.AppendLine("      OR Contatti.Sa_Cod = -1 ")
                End If
                If leggiMacchine Then
                    StrSQL.AppendLine("      OR Parco_Macchine.Sa_Cod = -1 ")
                End If
                If leggiProdotti Then
                    StrSQL.AppendLine("      OR Materie_Prime.Sa_Cod = -1 ")
                End If
                StrSQL.AppendLine("      ) ")
            End If

            StrSQL.Append(" AND (1 = 2 ")
            If leggiPersone Then
                StrSQL.Append(" OR pc.Elem_Cod = 0 ")
            End If
            If leggiMacchine Then
                StrSQL.Append(" OR pc.Elem_Cod = 1 ")
            End If
            If leggiProdotti Then
                StrSQL.Append(" OR pc.Elem_Cod NOT IN (0,1) ")
            End If
            StrSQL.AppendLine(" ) ")


            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine(" ORDER BY pc.Elem_Cod, pc.Mat_Cod, pc.Pro_Cod, pc.Udm_Cod, pc.Validita_Inizio ")
            End If
            
            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function


    Public Function Leggi_Prodotti(ByVal Piva As String,
                                   ByVal xFiltroAggiuntivo As String,
                                   ByRef objParametri As AgronicaCoreParametri,
                                   Optional ByVal IdBudget As Integer = 0
                                   ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Prodotti_Costi_R.Leggi_Prodotti()"

        '====================================================================================
        'Parametri opzionali :
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT pc.*, i.Rag_Soc As Azienda  ")
            StrSQL.AppendLine(" FROM   Prodotti_Costi pc ")
            StrSQL.AppendLine(" INNER JOIN Imprese i on i.piva = pc.piva ")
            StrSQL.AppendLine(" Where Elem_Cod > 1 ")
            StrSQL.AppendLine(" And pc.Id_Budget = " & Agro_SQL_SaveNum(IdBudget))

            'TODO: Filtrare Piva?

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

End Class


'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################


Public Class Prodotti_Costi_W
    Inherits AgronicaCoreDataProvider.DataProvider

    '============================================================================
    Public Function Scrivi_Completa(ByVal Id As Integer,
                                    ByVal Piva As String,
                                    ByVal Riferimento As String,
                                    ByVal Elem_Cod As Int32,
                                    ByVal Pro_Cod As Int32,
                                    ByVal Mat_Cod As Int32,
                                    ByVal Udm_Cod As Int32,
                                    ByVal Mezzo As Integer,
                                    ByVal Prezzo_Unitario As Decimal,
                                    ByVal Veg_Cod As Int32,
                                    ByVal Cul_Cod As Int32,
                                    ByVal Validita_Inizio As Date,
                                    ByVal Validita_Fine As Date,
                                    ByVal TipoOperazione As enum_TipoOperazioneDB,
                                    ByRef objParametri As AgronicaCoreParametri,
                                    Optional ByVal Data_creazione As DateTime = #2/1/1900#,
                                    Optional ByVal Data_modifica As DateTime = #2/1/1900#,
                                    Optional ByVal username_creazione As String = "",
                                    Optional ByVal username_modifica As String = "",
                                    Optional ByVal Id_Budget As Int32 = 0
                                    ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.Prodotti_Costi_W.Scrivi_Completa()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False

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

        Try

            '---------------------------------------------
            StrSQL.Length = 0

            '@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
            '@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
            '@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

            If TipoOperazione = enum_TipoOperazioneDB.Scrittura Then

                StrSQL.Append(" INSERT INTO Prodotti_Costi ")
                StrSQL.Append("         ( ")
                StrSQL.Append("          Piva, Riferimento, Elem_Cod, Pro_Cod, Mat_Cod, ")
                StrSQL.Append("          Udm_Cod, Mezzo, Prezzo_Unitario, Veg_Cod, ")
                StrSQL.Append("          Cul_Cod, ")

                StrSQL.Append("          Inviato,            DataInvio, ")
                StrSQL.Append("          Data_Creazione,     Data_Modifica, ")
                StrSQL.Append("          UserName_Creazione, UserName_Modifica, ")
                StrSQL.Append("          Validita_Inizio,    Validita_Fine, Id_Budget ")
                StrSQL.Append("         ) ")

                StrSQL.Append(" VALUES ( ")
                StrSQL.Append("          '" & Agro_SQL_SaveText(Piva) & "'  ")
                StrSQL.Append("         ,'" & Agro_SQL_SaveText(Riferimento) & "'  ")
                StrSQL.Append("         , " & Agro_SQL_SaveNum(Elem_Cod) & "  ")
                StrSQL.Append("         , " & Agro_SQL_SaveNum(Pro_Cod) & "  ")
                StrSQL.Append("         , " & Agro_SQL_SaveNum(Mat_Cod) & "  ")
                StrSQL.Append("         , " & Agro_SQL_SaveNum(Udm_Cod) & "  ")
                StrSQL.Append("         , " & Agro_SQL_SaveNum(Mezzo) & "  ")
                StrSQL.Append("         , " & Agro_SQL_SaveNum(Prezzo_Unitario) & "  ")
                StrSQL.Append("         , " & Agro_SQL_SaveNum(Veg_Cod) & "  ")
                StrSQL.Append("         , " & Agro_SQL_SaveNum(Cul_Cod) & "  ")
                StrSQL.Append("         , 0  ")
                StrSQL.Append("         , Null  ")
                StrSQL.Append("         , " & Agro_SQL_SaveDateTime(Data_creazione) & "  ")
                StrSQL.Append("         , " & Agro_SQL_SaveDateTime(Data_modifica) & "  ")
                StrSQL.Append("         ,'" & Agro_SQL_SaveText(username_creazione) & "' ")
                StrSQL.Append("         ,'" & Agro_SQL_SaveText(username_modifica) & "' ")
                StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
                StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")
                StrSQL.Append("         , " & Agro_SQL_SaveNum(Id_Budget) & "  ")
                StrSQL.Append(") ")

            Else

                StrSQL.Append(" UPDATE Prodotti_Costi ")
                StrSQL.Append(" SET Piva = '" & Agro_SQL_SaveText(Piva) & "',  ")
                StrSQL.Append(" Riferimento = '" & Agro_SQL_SaveText(Riferimento) & "',  ")
                StrSQL.Append(" Elem_Cod = " & Agro_SQL_SaveNum(Elem_Cod) & ",  ")
                StrSQL.Append(" Pro_Cod = " & Agro_SQL_SaveNum(Pro_Cod) & ",  ")
                StrSQL.Append(" Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & ",  ")

                StrSQL.Append(" Udm_Cod = " & Agro_SQL_SaveNum(Udm_Cod) & ",  ")
                StrSQL.Append(" Mezzo = " & Agro_SQL_SaveNum(Mezzo) & ",  ")
                StrSQL.Append(" Prezzo_Unitario = " & Agro_SQL_SaveNum(Prezzo_Unitario) & ",  ")

                StrSQL.Append(" Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod) & ",  ")
                StrSQL.Append(" Cul_Cod = " & Agro_SQL_SaveNum(Cul_Cod) & ",  ")
                StrSQL.Append(" Inviato = 0, DataInvio = null, ")

                'StrSQL.Append(" Data_Creazione = " & Agro_SQL_SaveDateTime(Data_creazione) & ", ")
                StrSQL.Append(" Data_Modifica = " & Agro_SQL_SaveDateTime(Data_modifica) & ", ")
                'StrSQL.Append(" UserName_Creazione = '" & Agro_SQL_SaveText(username_creazione) & "', ")
                StrSQL.Append(" UserName_Modifica = '" & Agro_SQL_SaveText(username_modifica) & "', ")
                StrSQL.Append(" Validita_Inizio = " & Agro_SQL_SaveDate(Validita_Inizio) & ", ")
                StrSQL.Append(" Validita_Fine = " & Agro_SQL_SaveDate(Validita_Fine) & ", ")
                StrSQL.Append(" Id_Budget = " & Agro_SQL_SaveNum(Id_Budget) & " ")

                StrSQL.Append(" WHERE Id = " & Agro_SQL_SaveNum(Id))

            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function

    Public Function Scrivi(ByVal Piva As String,
                           ByVal Riferimento As String,
                           ByVal Elem_Cod As Int32,
                           ByVal Pro_Cod As Int32,
                           ByVal Mat_Cod As Int32,
                           ByVal Udm_Cod As Int32,
                           ByVal Mezzo As Integer,
                           ByVal Prezzo_Unitario As Decimal,
                           ByVal Veg_Cod As Int32,
                           ByVal Cul_Cod As Int32,
                           ByVal Validita_Inizio As Date,
                           ByVal Validita_Fine As Date,
                           ByRef objParametri As AgronicaCoreParametri,
                           Optional ByVal Data_creazione As DateTime = #2/1/1900#,
                           Optional ByVal Data_modifica As DateTime = #2/1/1900#,
                           Optional ByVal username_creazione As String = "",
                           Optional ByVal username_modifica As String = "",
                           Optional ByVal Id_Budget As Int32 = 0
                           ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.Prodotti_Costi_W.Scrivi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False

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

        Try

            '---------------------------------------------
            StrSQL.Length = 0


            '@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
            '@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
            '@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@


            StrSQL.Append(" INSERT INTO Prodotti_Costi ")
            StrSQL.Append("         ( ")
            StrSQL.Append("          Piva, Riferimento, Elem_Cod, Pro_Cod, Mat_Cod, ")
            StrSQL.Append("          Udm_Cod, Mezzo, Prezzo_Unitario, Veg_Cod, ")
            StrSQL.Append("          Cul_Cod, ")

            StrSQL.Append("          Inviato,            DataInvio, ")
            StrSQL.Append("          Data_Creazione,     Data_Modifica, ")
            StrSQL.Append("          UserName_Creazione, UserName_Modifica, ")
            StrSQL.Append("          Validita_Inizio,    Validita_Fine, Id_Budget ")
            StrSQL.Append("         ) ")

            StrSQL.Append(" VALUES ( ")
            StrSQL.Append("          '" & Agro_SQL_SaveText(Piva) & "'  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Riferimento) & "'  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Elem_Cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Pro_Cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Mat_Cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Udm_Cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Mezzo) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Prezzo_Unitario) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Veg_Cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Cul_Cod) & "  ")

            StrSQL.Append("         , 0  ")
            StrSQL.Append("         , Null  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDateTime(Data_creazione) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDateTime(Data_modifica) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(username_modifica) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Id_Budget) & "  ")

            StrSQL.Append(") ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function

    '============================================================================
    Public Function Cancella(ByVal Piva As String,
                             ByVal Riferimento As String,
                             ByVal Elem_Cod As Integer,
                             ByVal Pro_Cod As Integer,
                             ByVal Mat_Cod As Integer,
                             ByVal Veg_Cod As Integer,
                             ByVal Cul_Cod As Integer,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreParametri,
                             Optional Id_Budget As Integer = 0
                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.Prodotti_Costi_W.Cancella()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""
        '   Riferimento = ""
        '   Pro_Cod = 0
        '   Mat_Cod = 0
        '   Veg_Cod = 0
        '   Cul_Cod = 0
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then

                StrSQL.Length = 0
                StrSQL.Append(" UPDATE Prodotti_Costi ")
                StrSQL.Append(" SET ")
                StrSQL.Append("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("      ,Inviato = -1 ")
                StrSQL.Append(" WHERE  Inviato >= 0 ")

            Else
                StrSQL.Length = 0
                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM Prodotti_Costi ")
                StrSQL.Append(" WHERE  1=1 And Prodotti_Costi.Id_Budget = " & Agro_SQL_SaveNum(Id_Budget))
            End If



            'Elem_Cod Obbligatorio (Risorse_Umane --> Elem_Cod = 0)
            StrSQL.Append(" AND Prodotti_Costi.Elem_Cod = " & Agro_SQL_SaveNum(Elem_Cod) & "   ")


            If Piva <> "" Then
                StrSQL.Append(" AND Prodotti_Costi.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
            End If

            If Riferimento <> "" Then
                StrSQL.Append(" AND Prodotti_Costi.Riferimento = '" & Agro_SQL_SaveText(Riferimento) & "'   ")
            End If

            If Pro_Cod <> 0 Then
                StrSQL.Append(" AND Prodotti_Costi.Pro_Cod = " & Agro_SQL_SaveNum(Pro_Cod) & "   ")
            End If

            If Mat_Cod <> 0 Then
                StrSQL.Append(" AND Prodotti_Costi.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & "   ")
            End If

            If Veg_Cod <> 0 Then
                StrSQL.Append(" AND Prodotti_Costi.Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod) & "   ")
            End If

            If Cul_Cod <> 0 Then
                StrSQL.Append(" AND Prodotti_Costi.Cul_Cod = " & Agro_SQL_SaveNum(Cul_Cod) & "   ")
            End If


            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function

    Public Function CancellaDaBudget(ByVal Id_Budget As Int32,
                                     ByVal xFiltroAggiuntivo As String,
                                     ByRef objParametri As AgronicaCoreParametri
                                     ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.Prodotti_Costi_W.CancellaDaBudget()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            StrSQL.Length = 0
            StrSQL.Append(" DELETE ")
            StrSQL.Append(" FROM Prodotti_Costi ")
            StrSQL.Append(" WHERE Prodotti_Costi.Id_Budget = " & Agro_SQL_SaveNum(Id_Budget) & "   ")

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="ID"></param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="objParametri"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[pierantoni]	04/02/2011	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Function Cancella_da_ID(ByVal ID As Integer,
                                   ByVal xFiltroAggiuntivo As String,
                                   ByRef objParametri As AgronicaCoreParametri
                                   ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.Prodotti_Costi_W.Cancella_da_ID()"

        '============================================================================
        'Se tento di cancellare fisicamente un record con INVIATO=1
        'allora pongo INVIATO=-1
        'questo per consentire la risincronizzazione col server
        '============================================================================

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            StrSQL.Length = 0

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then

                StrSQL.Append(" UPDATE  Prodotti_Costi ")
                StrSQL.Append(" SET ")
                StrSQL.Append(" Username_Modifica = '" & objParametri.UsernameOperazione & "' ")
                StrSQL.Append(" ,Inviato = -1 ")
                StrSQL.Append(" WHERE   Inviato > 0 ")

            Else

                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM  Prodotti_Costi ")
                StrSQL.Append(" WHERE Inviato = 0 ")

            End If


            If ID <> 0 Then
                StrSQL.Append(" AND ID = " & Agro_SQL_SaveNum(ID) & "   ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function

End Class
