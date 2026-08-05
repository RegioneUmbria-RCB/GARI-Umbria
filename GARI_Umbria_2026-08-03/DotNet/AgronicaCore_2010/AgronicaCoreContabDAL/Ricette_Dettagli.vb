Imports System.Text
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi


Public Class Ricette_Dettagli_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function Leggi(ByVal Ricetta_Cod As Int32,
                          ByVal Ricetta_Operazione_Cod As Int32,
                          ByVal Ricetta_Dettaglio_Cod As Int32,
                          ByVal Cau_Mov As String,
                          ByVal Miscela_Cod As Int32,
                          ByVal Elem_Cod As Int32,
                          ByVal Pro_Cod As Int32,
                          ByVal Mat_Cod As Int32,
                          ByVal Validita_Inizio As Date,
                          ByVal Validita_Fine As Date,
                          ByVal xSelezioneVariabile As enumSelezioneVariabile,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreParametri,
                          Optional ByVal joinRicette As Boolean = False
                          ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Ricette_Dettagli_R.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   Ricetta_Cod = 0
        '   Ricetta_Operazione_Cod = 0
        '   Ricetta_Dettaglio_Cod = 0
        '   Miscela_Cod = 0
        '   Elem_Cod = 0
        '   Pro_Cod = 0
        '   Mat_Cod = 0
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim StrSQL As New StringBuilder
        Dim dt As DataTable

        Try
            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                    StrSQL.Length = 0

                    StrSQL.AppendLine(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; ")

                    StrSQL.AppendLine(" SELECT Ricette_Dettagli.Ricetta_Cod, Ricette_Dettagli.Ricetta_Operazione_Cod, Ricette_Dettagli.Ricetta_Dettaglio_Cod,  ")
                    StrSQL.AppendLine(" Ricette_Dettagli.Cau_Mov, Ricette_Dettagli.Miscela_Cod, Ricette_Dettagli.Elem_Cod, Ricette_Dettagli.Pro_Cod, Ricette_Dettagli.Mat_Cod, Ricette_Dettagli.Udm_Cod, Ricette_Dettagli.Qta, Ricette_Dettagli.Extra_Int, Ricette_Dettagli.Prezzo_Unitario, Ricette_Dettagli.Turno_Cod, Ricette_Dettagli.ID_Attivita, Ricette_Dettagli.Lotto, Ricette_Dettagli.Validita_Inizio, Ricette_Dettagli.Validita_Fine ")

                    If joinRicette Then
                        StrSQL.AppendLine(" ,Ricette_Operazioni.Validita_Inizio AS DataOperazione ")
                    End If

                    StrSQL.AppendLine(" FROM  Ricette_Dettagli ")
                    If joinRicette Then
                        StrSQL.AppendLine(" INNER JOIN  Ricette ON Ricette.Ricetta_Cod = Ricette_Dettagli.Ricetta_COD ")
                        StrSQL.AppendLine(" INNER JOIN  Ricette_Operazioni ON Ricette_Operazioni.Ricetta_Operazione_Cod = Ricette_Dettagli.Ricetta_Operazione_COD ")

                    End If
                    StrSQL.AppendLine(" WHERE Ricette_Dettagli.Validita_inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
                    StrSQL.AppendLine(" AND   Ricette_Dettagli.Validita_Fine >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")

                    If objParametri.PivaSuperUser <> "" Then
                        StrSQL.AppendLine(" AND Ricette_Dettagli.Ricetta_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'   ")
                    End If

                    If Ricetta_Cod <> 0 Then
                        StrSQL.AppendLine(" AND Ricette_Dettagli.Ricetta_Cod = " & Agro_SQL_SaveNum(Ricetta_Cod) & "   ")
                    End If

                    If Ricetta_Operazione_Cod <> 0 Then
                        StrSQL.AppendLine(" AND Ricette_Dettagli.Ricetta_Operazione_Cod = " & Agro_SQL_SaveNum(Ricetta_Operazione_Cod) & "   ")
                    End If

                    If Ricetta_Dettaglio_Cod <> 0 Then
                        StrSQL.AppendLine(" AND Ricette_Dettagli.Ricetta_Dettaglio_Cod = " & Agro_SQL_SaveNum(Ricetta_Dettaglio_Cod) & "   ")
                    End If

                    If Cau_Mov <> "" Then
                        StrSQL.AppendLine(" AND Ricette_Dettagli.Cau_Mov = '" & Agro_SQL_SaveText(Cau_Mov) & "'   ")
                    End If

                    If Miscela_Cod <> 0 Then
                        StrSQL.AppendLine(" AND Ricette_Dettagli.Miscela_Cod = " & Agro_SQL_SaveNum(Miscela_Cod) & "   ")
                    End If

                    If Elem_Cod <> 0 Then
                        StrSQL.AppendLine(" AND Ricette_Dettagli.Elem_Cod = " & Agro_SQL_SaveNum(Elem_Cod) & " ")
                    End If

                    If Pro_Cod <> 0 Then
                        StrSQL.AppendLine(" AND Ricette_Dettagli.Pro_Cod = " & Agro_SQL_SaveNum(Pro_Cod) & " ")
                    End If

                    If Mat_Cod <> 0 Then
                        StrSQL.AppendLine(" AND Ricette_Dettagli.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & " ")
                    End If


                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.AppendLine(" AND   Ricette_Dettagli.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.AppendLine(" AND   Ricette_Dettagli.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.AppendLine(" ORDER BY Ricetta_SuperUser, Ricetta_Cod, Ricetta_Dettaglio_Cod Asc ")
                    End If

                Case enumSelezioneVariabile.Selezione_TabellaCompleta

                    StrSQL.Length = 0

                    StrSQL.AppendLine(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; ")

                    StrSQL.AppendLine(" SELECT Ricette_Dettagli.* ")

                    If joinRicette Then
                        StrSQL.AppendLine(" ,Ricette_Operazioni.Validita_Inizio AS DataOperazione ")
                    End If

                    StrSQL.AppendLine(" FROM  Ricette_Dettagli ")

                    If joinRicette Then
                        StrSQL.AppendLine(" INNER JOIN  Ricette ON Ricette.Ricetta_Cod = Ricette_Dettagli.Ricetta_COD ")
                        StrSQL.AppendLine(" INNER JOIN  Ricette_Operazioni ON Ricette_Operazioni.Ricetta_Operazione_Cod = Ricette_Dettagli.Ricetta_Operazione_COD ")
                    End If

                    StrSQL.AppendLine(" WHERE Ricette_Dettagli.Validita_inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
                    StrSQL.AppendLine(" AND   Ricette_Dettagli.Validita_Fine >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")

                    If objParametri.PivaSuperUser <> "" Then
                        StrSQL.AppendLine(" AND Ricette_Dettagli.Ricetta_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'   ")
                    End If

                    If Ricetta_Cod <> 0 Then
                        StrSQL.AppendLine(" AND Ricette_Dettagli.Ricetta_Cod = " & Agro_SQL_SaveNum(Ricetta_Cod) & "   ")
                    End If

                    If Ricetta_Operazione_Cod <> 0 Then
                        StrSQL.AppendLine(" AND Ricette_Dettagli.Ricetta_Operazione_Cod = " & Agro_SQL_SaveNum(Ricetta_Operazione_Cod) & "   ")
                    End If

                    If Ricetta_Dettaglio_Cod <> 0 Then
                        StrSQL.AppendLine(" AND Ricette_Dettagli.Ricetta_Dettaglio_Cod = " & Agro_SQL_SaveNum(Ricetta_Dettaglio_Cod) & "   ")
                    End If

                    If Cau_Mov <> "" Then
                        StrSQL.AppendLine(" AND Ricette_Dettagli.Cau_Mov = '" & Agro_SQL_SaveText(Cau_Mov) & "'   ")
                    End If

                    If Miscela_Cod <> 0 Then
                        StrSQL.AppendLine(" AND Ricette_Dettagli.Miscela_Cod = " & Agro_SQL_SaveNum(Miscela_Cod) & "   ")
                    End If

                    If Elem_Cod <> 0 Then
                        StrSQL.AppendLine(" AND Ricette_Dettagli.Elem_Cod = " & Agro_SQL_SaveNum(Elem_Cod) & " ")
                    End If

                    If Pro_Cod <> 0 Then
                        StrSQL.AppendLine(" AND Ricette_Dettagli.Pro_Cod = " & Agro_SQL_SaveNum(Pro_Cod) & " ")
                    End If

                    If Mat_Cod <> 0 Then
                        StrSQL.AppendLine(" AND Ricette_Dettagli.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & " ")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.AppendLine(" AND   Ricette_Dettagli.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.AppendLine(" AND   Ricette_Dettagli.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.AppendLine(" ORDER BY Ricetta_SuperUser, Ricetta_Cod, Ricetta_Dettaglio_Cod Asc ")
                    End If

                Case enumSelezioneVariabile.Selezione_JoinDescrizioni

                    StrSQL.Length = 0

                    StrSQL.AppendLine(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; ")

                    StrSQL.AppendLine(" SELECT Ricette_Dettagli.*, ")

                    StrSQL.AppendLine(" ISNULL(Materie_Prime.Mat_Des, '') AS Mat_Des, ")
                    StrSQL.AppendLine(" ISNULL(Fertilizzanti.Fer_Des, '') AS Fer_Des, ")
                    StrSQL.AppendLine(" ISNULL(Formulati.Fr_Des, '') AS Fr_Des, ")
                    StrSQL.AppendLine(" ISNULL(Trappole.Trap_Des, '') AS Trap_Des, ")
                    StrSQL.AppendLine(" ISNULL(InsettiUtili.Ins_Des, '') AS Ins_Des, ")
                    StrSQL.AppendLine(" ISNULL(UnitaMisura.UDM_DES, '') AS UDM_DES, ")
                    StrSQL.AppendLine(" ISNULL(UnitaMisura.UDM_SIM, '') AS UDM_SIM, ")
                    StrSQL.AppendLine(" ISNULL(Parco_Macchine.Mac_Des, '') AS Mac_Des, ")
                    StrSQL.AppendLine(" ISNULL(Contatti.Rag_Soc, '') AS Rag_Soc, ")
                    StrSQL.AppendLine(" ISNULL(Contatti.Nome, '') AS Nome, ")
                    StrSQL.AppendLine(" ISNULL(Contatti.Cognome, '') AS Cognome ")

                    If joinRicette Then
                        StrSQL.AppendLine(" ,Ricette_Operazioni.Validita_Inizio AS DataOperazione ")
                    End If

                    If joinRicette Then
                        StrSQL.AppendLine(" FROM Ricette_Dettagli ")
                        StrSQL.AppendLine(" INNER JOIN Ricette ON Ricette.Ricetta_Cod = Ricette_Dettagli.Ricetta_COD ")
                        StrSQL.AppendLine(" INNER JOIN Ricette_Operazioni ON Ricette_Operazioni.Ricetta_Operazione_Cod = Ricette_Dettagli.Ricetta_Operazione_COD")
                    Else
                        StrSQL.AppendLine(" FROM Ricette_Dettagli ")
                    End If

                    StrSQL.AppendLine(" LEFT OUTER JOIN Parco_Macchine ON Ricette_Dettagli.Mat_Cod = Parco_Macchine.Mac_Cod  ")
                    StrSQL.AppendLine(" LEFT OUTER JOIN Formulati ON Ricette_Dettagli.Pro_Cod = Formulati.Fr_Cod ")
                    StrSQL.AppendLine(" LEFT OUTER JOIN Fertilizzanti ON Ricette_Dettagli.Pro_Cod = Fertilizzanti.Fer_Cod  ")
                    StrSQL.AppendLine(" LEFT OUTER JOIN Trappole ON Ricette_Dettagli.Pro_Cod = Trappole.Trap_Cod")
                    StrSQL.AppendLine(" LEFT OUTER JOIN InsettiUtili ON Ricette_Dettagli.Pro_Cod = InsettiUtili.Ins_Cod  ")
                    StrSQL.AppendLine(" LEFT OUTER JOIN UnitaMisura ON Ricette_Dettagli.Udm_Cod = UnitaMisura.Udm_Cod ")
                    StrSQL.AppendLine(" LEFT OUTER JOIN Materie_Prime ON Ricette_Dettagli.Elem_Cod = Materie_Prime.Elem_Cod AND Ricette_Dettagli.Mat_Cod = Materie_Prime.Mat_Cod ")
                    StrSQL.AppendLine(" LEFT OUTER JOIN Risorse_Umane ON Ricette_Dettagli.Mat_Cod = Risorse_Umane.Cod_RisUm ")
                    StrSQL.AppendLine(" LEFT OUTER JOIN Contatti ON Risorse_Umane.Cod_Contatto = Contatti.Cod_Contatto AND Risorse_Umane.Piva = Contatti.Piva ")

                    StrSQL.AppendLine(" WHERE Ricette_Dettagli.Validita_inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
                    StrSQL.AppendLine(" AND Ricette_Dettagli.Validita_Fine >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")

                    If objParametri.PivaSuperUser <> "" Then
                        StrSQL.AppendLine(" AND Ricette_Dettagli.Ricetta_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'   ")
                    End If

                    If Ricetta_Cod <> 0 Then
                        StrSQL.AppendLine(" AND Ricette_Dettagli.Ricetta_Cod = " & Agro_SQL_SaveNum(Ricetta_Cod) & "   ")
                    End If

                    If Ricetta_Operazione_Cod <> 0 Then
                        StrSQL.AppendLine(" AND Ricette_Dettagli.Ricetta_Operazione_Cod = " & Agro_SQL_SaveNum(Ricetta_Operazione_Cod) & "   ")
                    End If

                    If Ricetta_Dettaglio_Cod <> 0 Then
                        StrSQL.AppendLine(" AND Ricette_Dettagli.Ricetta_Dettaglio_Cod = " & Agro_SQL_SaveNum(Ricetta_Dettaglio_Cod) & "   ")
                    End If

                    If Cau_Mov <> "" Then
                        StrSQL.AppendLine(" AND Ricette_Dettagli.Cau_Mov = '" & Agro_SQL_SaveText(Cau_Mov) & "'   ")
                    End If

                    If Miscela_Cod <> 0 Then
                        StrSQL.AppendLine(" AND Ricette_Dettagli.Miscela_Cod = " & Agro_SQL_SaveNum(Miscela_Cod) & "   ")
                    End If

                    If Elem_Cod <> 0 Then
                        StrSQL.AppendLine(" AND Ricette_Dettagli.Elem_Cod = " & Agro_SQL_SaveNum(Elem_Cod) & " ")
                    End If

                    If Pro_Cod <> 0 Then
                        StrSQL.AppendLine(" AND Ricette_Dettagli.Pro_Cod = " & Agro_SQL_SaveNum(Pro_Cod) & " ")
                    End If

                    If Mat_Cod <> 0 Then
                        StrSQL.AppendLine(" AND Ricette_Dettagli.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & " ")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.AppendLine(" AND Ricette_Dettagli.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.AppendLine(" AND Ricette_Dettagli.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.AppendLine(" ORDER BY Ricetta_SuperUser, Ricetta_Cod, Ricetta_Dettaglio_Cod Asc ")
                    End If

                Case enumSelezioneVariabile.Selezione_JoinCompleta


            End Select

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore, ex)
        End Try

        Return dt

    End Function

    '##############################################################################################
    Public Function MaxValoreMiscelaCod(ByVal Ricetta_Cod As Int32,
                                        ByVal Ricetta_Operazione_Cod As Int32,
                                        ByRef objParametri As AgronicaCoreParametri
                                        ) As Integer

        Const nomeRoutine = "AgronicaCoreContabDAL.Ricette_Dettagli_R.MaxValoreMiscelaCod()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New StringBuilder
        Dim dt As DataTable
        Dim intRet As Integer

        Try

            StrSQL.Length = 0

            StrSQL.Append(" SELECT ISNULL(MAX(Miscela_Cod),0) AS MaxId  ")
            StrSQL.Append(" FROM  Ricette_Dettagli ")
            StrSQL.Append(" WHERE Ricetta_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.Append(" AND   Ricetta_Cod = " & Agro_SQL_SaveNum(Ricetta_Cod) & " ")
            StrSQL.Append(" AND   Ricetta_Operazione_Cod = " & Agro_SQL_SaveNum(Ricetta_Operazione_Cod) & " ")

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   Ricette_Dettagli.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   Ricette_Dettagli.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------         

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

            intRet = 0
            If dt.Rows.Count <> 0 Then
                intRet = dt.Rows(0).Item("MaxId")
            End If

            ' distruggo gli oggetti
            dt.Dispose()
            dt = Nothing

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return intRet

    End Function

    Public Function Leggi_Macroelementi_Distribuiti(ByRef N_Distribuito_Ha As Decimal,
                                                    ByRef P_Distribuito_Ha As Decimal,
                                                    ByRef K_Distribuito_Ha As Decimal,
                                                    ByRef Mg_Distribuito_Ha As Decimal,
                                                    ByRef Cu_Distribuito_Ha As Decimal,
                                                    ByRef N_Distribuito_Imp As Decimal,
                                                    ByRef P_Distribuito_Imp As Decimal,
                                                    ByRef K_Distribuito_Imp As Decimal,
                                                    ByRef Mg_Distribuito_Imp As Decimal,
                                                    ByRef Cu_Distribuito_Imp As Decimal,
                                                    ByVal Piva As String,
                                                    ByVal Sa_Cod As Integer,
                                                    ByVal Appezza As Integer,
                                                    ByVal Id_Reg As Integer,
                                                    ByVal Progetto_Cod As Integer,
                                                    ByVal Validita_Inizio As Date,
                                                    ByVal Validita_Fine As Date,
                                                    ByVal ricetta_operazione_cod_Escluso As Integer,
                                                    ByRef objParametri As AgronicaCoreParametri,
                                                    Optional Raccoglitore_Cod_Escluso As Integer = 0
                                                    ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Movimenti_Dettagli_R.Leggi_Macroelementi_Distribuiti()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""                   =>  si leggono tutte le imprese
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable
        Dim supTratt As Decimal
        Dim supImp As Decimal

        Try

            strSql.Length = 0

            strSql.AppendLine(" SELECT DISTINCT ")
            strSql.AppendLine(" fertilizzanti.fer_cod, fertilizzanti.FER_DES, ")
            strSql.AppendLine(" ISNULL(fertilizzanti.N,0) AS N, ISNULL(fertilizzanti.P2O5,0) AS P2O5, ISNULL(fertilizzanti.K2O,0) AS K2O, ISNULL(fertilizzanti.MgO,0) AS MgO, ISNULL(fertilizzanti.Cu,0) AS Cu,")
            strSql.AppendLine(" ricette_operazioni.ricetta_operazione_des, Imprese_Progetti.Progetto_Nome as Lotto, ")
            strSql.AppendLine(" Ricette_operazioni.validita_inizio as Data_Creazione, '' as Mov_Desc, Ricette_operazioni.Mezzo, ")
            strSql.AppendLine(" Ricette_dettaglio_tecnico.Qta_Ril, unitamisura.UDM_DES, ")
            strSql.AppendLine(" Cultivar.CUL_DES, Reg_Impianti.sup_imp,  ")
            strSql.AppendLine(" Ricette_destinazioni.Qta, Ricette_destinazioni.Qta2, ")
            strSql.AppendLine(" Operazioni.lav_cod, Operazioni.lav_des, Ricette_dettagli.UDM_COD, Ricette_dettagli.Extra_Int, ")
            strSql.AppendLine(" Reg_Impianti.ID_REG, ricette_operazioni.ricetta_operazione_cod,  ")
            strSql.AppendLine(" ISNULL(Ricette_dettaglio_tecnico.N,0) AS N_Tecnico, ")
            strSql.AppendLine(" ISNULL(Ricette_dettaglio_tecnico.K,0) AS K2O_Tecnico, ")
            strSql.AppendLine(" ISNULL(Ricette_dettaglio_tecnico.P,0) AS P2O5_Tecnico, ")
            strSql.AppendLine(" ISNULL(Ricette_dettaglio_tecnico.Mg,0) AS MgO_Tecnico, ")
            strSql.AppendLine(" ISNULL(Ricette_dettaglio_tecnico.Efficienza,1) AS Efficienza, ")
            strSql.AppendLine(" ISNULL(Ricette_dettaglio_tecnico.Cu,0) AS Cu_Tecnico, ")
            strSql.AppendLine(" ISNULL(Ricette_Operazioni.Raccoglitore_Cod,0) AS Raccoglitore_Cod ")

            strSql.AppendLine(" FROM Ricette_dettaglio_tecnico RIGHT OUTER JOIN ")
            strSql.AppendLine(" Reg_Impianti INNER JOIN ")
            strSql.AppendLine(" Ricette_destinazioni On Reg_Impianti.PIVA = Ricette_destinazioni.Piva And Reg_Impianti.SA_COD = Ricette_destinazioni.Sa_Cod And ")
            strSql.AppendLine(" Reg_Impianti.ID_REG = Ricette_destinazioni.Id_reg And Reg_Impianti.APPEZZA = Ricette_destinazioni.Appezza ")
            strSql.AppendLine(" LEFT OUTER JOIN Imprese_Progetti On Imprese_Progetti.Piva = Reg_Impianti.Piva AND Imprese_Progetti.Sa_Cod = Reg_Impianti.Sa_Cod AND Imprese_Progetti.Appezza = Reg_Impianti.Appezza AND Imprese_Progetti.Id_Reg = Reg_Impianti.Id_Reg ")
            strSql.AppendLine(" INNER Join Operazioni  ")
            strSql.AppendLine(" INNER Join  Ricette_Operazioni   ")
            strSql.AppendLine(" INNER Join Ricette_Dettagli   ")
            strSql.AppendLine(" On Ricette_Operazioni.Ricetta_SuperUser = Ricette_Dettagli.Ricetta_SuperUser And Ricette_Operazioni.Ricetta_Cod = Ricette_Dettagli.Ricetta_Cod And Ricette_Operazioni.Ricetta_Operazione_Cod = Ricette_Dettagli.Ricetta_Operazione_Cod ")
            strSql.AppendLine(" INNER JOIN Fertilizzanti fertilizzanti ON fertilizzanti.Fer_Cod = Ricette_Dettagli.Pro_Cod INNER JOIN ")
            strSql.AppendLine(" UnitaMisura unitamisura ON Ricette_Dettagli.Udm_Cod = unitamisura.UDM_COD ON Operazioni.LAV_COD = Ricette_Operazioni.Lav_Cod ON ")
            strSql.AppendLine(" Ricette_Dettagli.Ricetta_SuperUser = Ricette_Destinazioni.Ricetta_SuperUser And  ")
            strSql.AppendLine(" Ricette_Dettagli.Ricetta_Cod = Ricette_Destinazioni.Ricetta_Cod And Ricette_Dettagli.Ricetta_Operazione_Cod = Ricette_Destinazioni.Ricetta_Operazione_Cod And Ricette_Dettagli.Ricetta_Dettaglio_Cod = Ricette_Destinazioni.Ricetta_Dettaglio_Cod ")
            strSql.AppendLine(" On Ricette_Dettagli.Ricetta_SuperUser = Ricette_Dettaglio_Tecnico.Ricetta_SuperUser And  ")
            strSql.AppendLine(" Ricette_Dettagli.Ricetta_Cod = Ricette_Dettaglio_Tecnico.Ricetta_Cod And Ricette_Dettagli.Ricetta_Operazione_Cod = Ricette_Dettaglio_Tecnico.Ricetta_Operazione_Cod And Ricette_Dettagli.Ricetta_Dettaglio_Cod = Ricette_Dettaglio_Tecnico.Ricetta_Dettaglio_Cod ")
            strSql.AppendLine(" LEFT OUTER JOIN SpecieVegetali INNER JOIN Cultivar ON SpecieVegetali.Veg_Cod = Cultivar.Veg_Cod  ")
            strSql.AppendLine(" ON Reg_Impianti.CUL_COD = Cultivar.Cul_Cod ")

            '14 = concimazione in pieno campo
            '26 = fertirrigazione
            '106 = trattamento antibutteratura 
            '123 = concimazione fogliare
            '124 = distribuzione ammendanti organici o liquami
            '156 = sarchiatura con concimazione
            strSql.AppendLine("WHERE Ricette_Operazioni.Lav_Cod IN (14, 26, 106, 123, 124, 156) ")

            If Piva <> "" Then
                strSql.AppendLine(" AND Ricette_destinazioni.PIVA = '" & Agro_SQL_SaveText(Trim(Piva)) & "' ")
            End If

            If Sa_Cod <> 0 Then
                strSql.AppendLine(" AND Ricette_destinazioni.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            End If

            If Appezza <> 0 Then
                strSql.AppendLine(" AND Ricette_destinazioni.Appezza = " & Agro_SQL_SaveNum(Appezza) & "  ")
            End If

            If Id_Reg <> 0 Then
                strSql.AppendLine(" AND Ricette_destinazioni.Id_reg =  " & Agro_SQL_SaveNum(Id_Reg) & " ")
            End If

            If Progetto_Cod <> 0 Then
                strSql.AppendLine(" AND Imprese_Progetti.Progetto_Cod =" & Agro_SQL_SaveNum(Progetto_Cod))
            End If

            strSql.AppendLine(" AND Ricette_operazioni.validita_inizio >=" & Agro_SQL_SaveDate(Validita_Inizio))
            strSql.AppendLine(" AND Ricette_operazioni.validita_inizio <=" & Agro_SQL_SaveDate(Validita_Fine))
            strSql.AppendLine(" AND Imprese_Progetti.validita_fine >=" & Agro_SQL_SaveDate(Validita_Inizio))
            strSql.AppendLine(" AND Imprese_Progetti.validita_inizio <=" & Agro_SQL_SaveDate(Validita_Fine))

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.AppendLine(" AND   Ricette_destinazioni.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.AppendLine(" AND   Ricette_destinazioni.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            strSql.AppendLine(" ORDER BY Ricette_operazioni.validita_inizio DESC ")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

            If Not IsNothing(dt) Then

                Dim N_Prodotto As Decimal
                Dim P_Prodotto As Decimal
                Dim K_Prodotto As Decimal
                Dim Mg_Prodotto As Decimal
                Dim Cu_Prodotto As Decimal
                Dim i As Integer

                For i = 0 To dt.Rows.Count - 1

                    'Controllo che l'Id_Agenda non sia già stato verificato
                    If CInt(dt.Rows(i).Item("Ricetta_Operazione_Cod")) <> ricetta_operazione_cod_Escluso AndAlso
                       CInt(dt.Rows(i).Item("Raccoglitore_Cod")) <> If(Raccoglitore_Cod_Escluso = 0, -999, Raccoglitore_Cod_Escluso) Then

                        'Se ho specificato gli apporti nel dettaglio utilizzo quelli
                        'altrimenti prende quelli del prodotto
                        If CDec(dt.Rows(i).Item("N_Tecnico")) <> 0 Then
                            N_Prodotto = CDec(dt.Rows(i).Item("N_Tecnico"))
                        Else
                            N_Prodotto = CDec(dt.Rows(i).Item("N"))
                        End If

                        If CDec(dt.Rows(i).Item("P2O5_Tecnico")) <> 0 Then
                            P_Prodotto = CDec(dt.Rows(i).Item("P2O5_Tecnico"))
                        Else
                            P_Prodotto = CDec(dt.Rows(i).Item("P2O5"))
                        End If

                        If CDec(dt.Rows(i).Item("K2O_Tecnico")) <> 0 Then
                            K_Prodotto = CDec(dt.Rows(i).Item("K2O_Tecnico"))
                        Else
                            K_Prodotto = CDec(dt.Rows(i).Item("K2O"))
                        End If

                        If CDec(dt.Rows(i).Item("MgO_Tecnico")) <> 0 Then
                            Mg_Prodotto = CDec(dt.Rows(i).Item("MgO_Tecnico"))
                        Else
                            Mg_Prodotto = CDec(dt.Rows(i).Item("MgO"))
                        End If

                        If CDec(dt.Rows(i).Item("Cu_Tecnico")) <> 0 Then
                            Cu_Prodotto = CDec(dt.Rows(i).Item("Cu_Tecnico"))
                        Else
                            Cu_Prodotto = CDec(dt.Rows(i).Item("Cu"))
                        End If

                        'Aggiornamenti Macroelementi Distribuiti

                        supImp = CDec(dt.Rows(i).Item("Sup_Imp"))

                        supTratt = 0
                        supTratt = CDec(dt.Rows(i).Item("Qta2"))
                        If supTratt = 0 Then
                            supTratt = supImp
                        End If

                        If CDec(dt.Rows(i).Item("efficienza")) < 1 Then
                            Select Case dt.Rows(i).Item("extra_int")
                                Case 19
                                    N_Distribuito_Ha += CDec(dt.Rows(i).Item("Qta")) / supTratt * N_Prodotto * CDec(dt.Rows(i).Item("efficienza"))
                                Case Else
                                    N_Distribuito_Ha += CDec(dt.Rows(i).Item("Qta")) / supTratt * N_Prodotto / 100 * CDec(dt.Rows(i).Item("efficienza"))
                            End Select
                            N_Distribuito_Imp += CDec(dt.Rows(i).Item("Qta")) * N_Prodotto / 100 * CDec(dt.Rows(i).Item("efficienza"))
                        Else
                            N_Distribuito_Ha += CDec(dt.Rows(i).Item("Qta")) * N_Prodotto / 100 / supTratt
                            N_Distribuito_Imp += CDec(dt.Rows(i).Item("Qta")) * N_Prodotto / 100
                        End If

                        P_Distribuito_Ha += CDec(dt.Rows(i).Item("Qta")) * P_Prodotto / 100 / supTratt
                        K_Distribuito_Ha += CDec(dt.Rows(i).Item("Qta")) * K_Prodotto / 100 / supTratt
                        Mg_Distribuito_Ha += CDec(dt.Rows(i).Item("Qta")) * Mg_Prodotto / 100 / supTratt
                        Cu_Distribuito_Ha += CDec(dt.Rows(i).Item("Qta")) * Cu_Prodotto / 100 / supTratt

                        P_Distribuito_Imp += CDec(dt.Rows(i).Item("Qta")) * P_Prodotto / 100
                        K_Distribuito_Imp += CDec(dt.Rows(i).Item("Qta")) * K_Prodotto / 100
                        Mg_Distribuito_Imp += CDec(dt.Rows(i).Item("Qta")) * Mg_Prodotto / 100
                        Cu_Distribuito_Imp += CDec(dt.Rows(i).Item("Qta")) * Cu_Prodotto / 100

                    End If

                Next

            End If

            Return dt

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Return Nothing
        End Try

    End Function

    ''' <summary>
    ''' Stessa funzione di Leggi_Macroelementi_Distribuiti_List_Ricetta_operazione_Cod_Esclusi ma in input prende una Lista di ricetta_operazione_cod_Escluso e non uno singolo
    ''' </summary>
    ''' <returns></returns>
    Public Function Leggi_Macroelementi_Distribuiti_List_Ricetta_operazione_Cod_Esclusi(ByRef N_Distribuito_Ha As Decimal,
                                                                                        ByRef P_Distribuito_Ha As Decimal,
                                                                                        ByRef K_Distribuito_Ha As Decimal,
                                                                                        ByRef Mg_Distribuito_Ha As Decimal,
                                                                                        ByRef Cu_Distribuito_Ha As Decimal,
                                                                                        ByRef N_Distribuito_Imp As Decimal,
                                                                                        ByRef P_Distribuito_Imp As Decimal,
                                                                                        ByRef K_Distribuito_Imp As Decimal,
                                                                                        ByRef Mg_Distribuito_Imp As Decimal,
                                                                                        ByRef Cu_Distribuito_Imp As Decimal,
                                                                                        ByVal Piva As String,
                                                                                        ByVal Sa_Cod As Integer,
                                                                                        ByVal Appezza As Integer,
                                                                                        ByVal Id_Reg As Integer,
                                                                                        ByVal Progetto_Cod As Integer,
                                                                                        ByVal Validita_Inizio As Date,
                                                                                        ByVal Validita_Fine As Date,
                                                                                        ByVal ricetta_operazione_cod_Esclusi As List(Of Integer),
                                                                                        ByRef objParametri As AgronicaCoreParametri
                                                                                        ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Movimenti_Dettagli_R.Leggi_Macroelementi_Distribuiti_List_Ricetta_operazione_Cod_Esclusi()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""                   =>  si leggono tutte le imprese
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable
        Dim supTratt As Decimal
        Dim supImp As Decimal

        Try

            strSql.Length = 0
            strSql.AppendLine(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;")
            strSql.AppendLine(" SELECT DISTINCT ")
            strSql.AppendLine(" fertilizzanti.fer_cod, fertilizzanti.FER_DES, ")
            strSql.AppendLine(" ISNULL(fertilizzanti.N,0) AS N, ISNULL(fertilizzanti.P2O5,0) AS P2O5, ISNULL(fertilizzanti.K2O,0) AS K2O, ISNULL(fertilizzanti.MgO,0) AS MgO, ISNULL(fertilizzanti.Cu,0) AS Cu,")
            strSql.AppendLine(" ricette_operazioni.ricetta_operazione_des, Imprese_Progetti.Progetto_Nome as Lotto, ")
            strSql.AppendLine(" Ricette_operazioni.validita_inizio as Data_Creazione, '' as Mov_Desc, Ricette_operazioni.Mezzo, ")
            strSql.AppendLine(" Ricette_dettaglio_tecnico.Qta_Ril, unitamisura.UDM_DES, ")
            strSql.AppendLine(" Cultivar.CUL_DES, Reg_Impianti.sup_imp,  ")
            strSql.AppendLine(" Ricette_destinazioni.Qta, Ricette_destinazioni.Qta2, ")
            strSql.AppendLine(" Operazioni.lav_cod, Operazioni.lav_des, Ricette_dettagli.UDM_COD, Ricette_dettagli.Extra_Int, ")
            strSql.AppendLine(" Reg_Impianti.ID_REG, ricette_operazioni.ricetta_operazione_cod,  ")
            strSql.AppendLine(" ISNULL(Ricette_dettaglio_tecnico.N,0) AS N_Tecnico, ")
            strSql.AppendLine(" ISNULL(Ricette_dettaglio_tecnico.K,0) AS K2O_Tecnico, ")
            strSql.AppendLine(" ISNULL(Ricette_dettaglio_tecnico.P,0) AS P2O5_Tecnico, ")
            strSql.AppendLine(" ISNULL(Ricette_dettaglio_tecnico.Mg,0) AS MgO_Tecnico, ")
            strSql.AppendLine(" ISNULL(Ricette_dettaglio_tecnico.Efficienza,1) AS Efficienza, ")
            strSql.AppendLine(" ISNULL(Ricette_dettaglio_tecnico.Cu,0) AS Cu_Tecnico ")

            strSql.AppendLine(" FROM Ricette_dettaglio_tecnico RIGHT OUTER JOIN ")
            strSql.AppendLine(" Reg_Impianti INNER JOIN ")
            strSql.AppendLine(" Ricette_destinazioni On Reg_Impianti.PIVA = Ricette_destinazioni.Piva And Reg_Impianti.SA_COD = Ricette_destinazioni.Sa_Cod And ")
            strSql.AppendLine(" Reg_Impianti.ID_REG = Ricette_destinazioni.Id_reg And Reg_Impianti.APPEZZA = Ricette_destinazioni.Appezza ")
            strSql.AppendLine(" LEFT OUTER JOIN Imprese_Progetti On Imprese_Progetti.Piva = Reg_Impianti.Piva AND Imprese_Progetti.Sa_Cod = Reg_Impianti.Sa_Cod AND Imprese_Progetti.Appezza = Reg_Impianti.Appezza AND Imprese_Progetti.Id_Reg = Reg_Impianti.Id_Reg ")
            strSql.AppendLine(" INNER Join Operazioni  ")
            strSql.AppendLine(" INNER Join  Ricette_Operazioni   ")
            strSql.AppendLine(" INNER Join Ricette_Dettagli   ")
            strSql.AppendLine(" On Ricette_Operazioni.Ricetta_SuperUser = Ricette_Dettagli.Ricetta_SuperUser And Ricette_Operazioni.Ricetta_Cod = Ricette_Dettagli.Ricetta_Cod And Ricette_Operazioni.Ricetta_Operazione_Cod = Ricette_Dettagli.Ricetta_Operazione_Cod ")
            strSql.AppendLine(" INNER JOIN Fertilizzanti fertilizzanti ON fertilizzanti.Fer_Cod = Ricette_Dettagli.Pro_Cod INNER JOIN ")
            strSql.AppendLine(" UnitaMisura unitamisura ON Ricette_Dettagli.Udm_Cod = unitamisura.UDM_COD ON Operazioni.LAV_COD = Ricette_Operazioni.Lav_Cod ON ")
            strSql.AppendLine(" Ricette_Dettagli.Ricetta_SuperUser = Ricette_Destinazioni.Ricetta_SuperUser And  ")
            strSql.AppendLine(" Ricette_Dettagli.Ricetta_Cod = Ricette_Destinazioni.Ricetta_Cod And Ricette_Dettagli.Ricetta_Operazione_Cod = Ricette_Destinazioni.Ricetta_Operazione_Cod And Ricette_Dettagli.Ricetta_Dettaglio_Cod = Ricette_Destinazioni.Ricetta_Dettaglio_Cod ")
            strSql.AppendLine(" On Ricette_Dettagli.Ricetta_SuperUser = Ricette_Dettaglio_Tecnico.Ricetta_SuperUser And  ")
            strSql.AppendLine(" Ricette_Dettagli.Ricetta_Cod = Ricette_Dettaglio_Tecnico.Ricetta_Cod And Ricette_Dettagli.Ricetta_Operazione_Cod = Ricette_Dettaglio_Tecnico.Ricetta_Operazione_Cod And Ricette_Dettagli.Ricetta_Dettaglio_Cod = Ricette_Dettaglio_Tecnico.Ricetta_Dettaglio_Cod ")
            strSql.AppendLine(" LEFT OUTER JOIN SpecieVegetali INNER JOIN Cultivar ON SpecieVegetali.Veg_Cod = Cultivar.Veg_Cod  ")
            strSql.AppendLine(" ON Reg_Impianti.CUL_COD = Cultivar.Cul_Cod ")

            '14 = concimazione in pieno campo
            '26 = fertirrigazione
            '106 = trattamento antibutteratura 
            '123 = concimazione fogliare
            '124 = distribuzione ammendanti organici o liquami
            '156 = sarchiatura con concimazione
            strSql.AppendLine("WHERE Ricette_Operazioni.Lav_Cod IN (14, 26, 106, 123, 124, 156) ")

            If Piva <> "" Then
                strSql.AppendLine(" AND Ricette_destinazioni.PIVA = '" & Agro_SQL_SaveText(Trim(Piva)) & "' ")
            End If

            If Sa_Cod <> 0 Then
                strSql.AppendLine(" AND Ricette_destinazioni.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            End If

            If Appezza <> 0 Then
                strSql.AppendLine(" AND Ricette_destinazioni.Appezza = " & Agro_SQL_SaveNum(Appezza) & "  ")
            End If

            If Id_Reg <> 0 Then
                strSql.AppendLine(" AND Ricette_destinazioni.Id_reg =  " & Agro_SQL_SaveNum(Id_Reg) & " ")
            End If

            If Progetto_Cod <> 0 Then
                strSql.AppendLine(" AND Imprese_Progetti.Progetto_Cod =" & Agro_SQL_SaveNum(Progetto_Cod))
            End If

            strSql.AppendLine(" AND Ricette_operazioni.validita_inizio >=" & Agro_SQL_SaveDate(Validita_Inizio))
            strSql.AppendLine(" AND Ricette_operazioni.validita_inizio <=" & Agro_SQL_SaveDate(Validita_Fine))
            strSql.AppendLine(" AND Imprese_Progetti.validita_fine >=" & Agro_SQL_SaveDate(Validita_Inizio))
            strSql.AppendLine(" AND Imprese_Progetti.validita_inizio <=" & Agro_SQL_SaveDate(Validita_Fine))

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.AppendLine(" AND   Ricette_destinazioni.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.AppendLine(" AND   Ricette_destinazioni.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            strSql.AppendLine(" ORDER BY Ricette_operazioni.validita_inizio DESC ")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

            If Not IsNothing(dt) Then

                Dim N_Prodotto As Decimal
                Dim P_Prodotto As Decimal
                Dim K_Prodotto As Decimal
                Dim Mg_Prodotto As Decimal
                Dim Cu_Prodotto As Decimal
                Dim i As Integer

                For i = 0 To dt.Rows.Count - 1

                    'Controllo che l'Id_Agenda non sia già stato verificato
                    If Not ricetta_operazione_cod_Esclusi.Contains(CInt(dt.Rows(i).Item("Ricetta_Operazione_Cod"))) Then

                        'Se ho specificato gli apporti nel dettaglio utilizzo quelli
                        'altrimenti prende quelli del prodotto
                        If CDec(dt.Rows(i).Item("N_Tecnico")) <> 0 Then
                            N_Prodotto = CDec(dt.Rows(i).Item("N_Tecnico"))
                        Else
                            N_Prodotto = CDec(dt.Rows(i).Item("N"))
                        End If

                        If CDec(dt.Rows(i).Item("P2O5_Tecnico")) <> 0 Then
                            P_Prodotto = CDec(dt.Rows(i).Item("P2O5_Tecnico"))
                        Else
                            P_Prodotto = CDec(dt.Rows(i).Item("P2O5"))
                        End If

                        If CDec(dt.Rows(i).Item("K2O_Tecnico")) <> 0 Then
                            K_Prodotto = CDec(dt.Rows(i).Item("K2O_Tecnico"))
                        Else
                            K_Prodotto = CDec(dt.Rows(i).Item("K2O"))
                        End If

                        If CDec(dt.Rows(i).Item("MgO_Tecnico")) <> 0 Then
                            Mg_Prodotto = CDec(dt.Rows(i).Item("MgO_Tecnico"))
                        Else
                            Mg_Prodotto = CDec(dt.Rows(i).Item("MgO"))
                        End If

                        If CDec(dt.Rows(i).Item("Cu_Tecnico")) <> 0 Then
                            Cu_Prodotto = CDec(dt.Rows(i).Item("Cu_Tecnico"))
                        Else
                            Cu_Prodotto = CDec(dt.Rows(i).Item("Cu"))
                        End If

                        'Aggiornamenti Macroelementi Distribuiti

                        supImp = CDec(dt.Rows(i).Item("Sup_Imp"))

                        supTratt = 0
                        supTratt = CDec(dt.Rows(i).Item("Qta2"))
                        If supTratt = 0 Then
                            supTratt = supImp
                        End If

                        If CDec(dt.Rows(i).Item("efficienza")) < 1 Then
                            N_Distribuito_Ha += CDec(dt.Rows(i).Item("Qta")) / supTratt * N_Prodotto / 100 * CDec(dt.Rows(i).Item("efficienza"))
                            N_Distribuito_Imp += CDec(dt.Rows(i).Item("Qta")) * N_Prodotto / 100 * CDec(dt.Rows(i).Item("efficienza"))
                        Else
                            N_Distribuito_Ha += CDec(dt.Rows(i).Item("Qta")) * N_Prodotto / 100 / supTratt
                            N_Distribuito_Imp += CDec(dt.Rows(i).Item("Qta")) * N_Prodotto / 100
                        End If

                        P_Distribuito_Ha += CDec(dt.Rows(i).Item("Qta")) * P_Prodotto / 100 / supTratt
                        K_Distribuito_Ha += CDec(dt.Rows(i).Item("Qta")) * K_Prodotto / 100 / supTratt
                        Mg_Distribuito_Ha += CDec(dt.Rows(i).Item("Qta")) * Mg_Prodotto / 100 / supTratt
                        Cu_Distribuito_Ha += CDec(dt.Rows(i).Item("Qta")) * Cu_Prodotto / 100 / supTratt

                        P_Distribuito_Imp += CDec(dt.Rows(i).Item("Qta")) * P_Prodotto / 100
                        K_Distribuito_Imp += CDec(dt.Rows(i).Item("Qta")) * K_Prodotto / 100
                        Mg_Distribuito_Imp += CDec(dt.Rows(i).Item("Qta")) * Mg_Prodotto / 100
                        Cu_Distribuito_Imp += CDec(dt.Rows(i).Item("Qta")) * Cu_Prodotto / 100

                    End If

                Next

            End If

            Return dt

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Return Nothing
        End Try

    End Function

    Public Function Verifica_Giacenze_Con_Pua(ByVal Piva As String,
                                              ByVal Ricetta_Cod As Integer,
                                              ByVal Elem_Cod As Integer,
                                              ByVal Pro_Cod As Integer,
                                              ByVal Mat_Cod As Integer,
                                              ByVal Udm_Cod As Integer,
                                              ByVal Eff_Cod As Integer,
                                              ByVal Dal_Data_Verifica As Date,
                                              ByVal Al_Data_Verifica As Date,
                                              ByVal Ricetta_Operazione_Cod_daNon_considerare As Integer,
                                              ByRef objParametri As AgronicaCoreParametri
                                              ) As Decimal

        Const nomeRoutine = "AgronicaCoreContabDAL.Ricette_Dettagli_R.Verifica_Giacenze_Con_Pua()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0

            strSql.AppendLine(" SELECT ( ")

            strSql.AppendLine("           SELECT ISNULL(SUM( round( Carico , +5)),0) as Qta_Totale_Plus  ")
            strSql.AppendLine("           FROM    Ricette ")
            strSql.AppendLine("           INNER JOIN PUA_Effluente ON Ricette.Programmazione_Cod=PUA_Effluente.PUA_Cod ")
            strSql.AppendLine("           WHERE   Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

            If Piva <> "" Then
                strSql.AppendLine(" AND Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If

            If Ricetta_Cod <> 0 Then
                strSql.AppendLine(" AND ricetta_cod = " & Agro_SQL_SaveNum(Ricetta_Cod) & "   ")
            End If

            If Eff_Cod <> 0 Then
                strSql.AppendLine(" AND Eff_Cod = " & Agro_SQL_SaveNum(Eff_Cod) & "   ")
            End If

            strSql.AppendLine(" )")
            strSql.AppendLine(" + ")
            strSql.AppendLine(" ( ")

            'strSql.AppendLine(" SELECT ISNULL(-SUM(round( rdest.Qta , +4)),0) as Qta_Totale_Minus  ")

            'strSql.AppendLine("           FROM    Ricette_Operazioni ro ")
            'strSql.AppendLine("           INNER JOIN Ricette_Dettagli rdet ON ro.Ricetta_SuperUser=rdet.Ricetta_SuperUser AND ro.Ricetta_Cod = rdet.Ricetta_Cod AND ro.Ricetta_Operazione_Cod = rdet.Ricetta_Operazione_Cod  ")
            'strSql.AppendLine("           INNER JOIN Ricette_Destinazioni rdest ON rdet.Ricetta_SuperUser=rdest.Ricetta_SuperUser AND rdet.Ricetta_Cod = rdest.Ricetta_Cod AND rdet.Ricetta_Operazione_Cod = rdest.Ricetta_Operazione_Cod AND rdet.Ricetta_Dettaglio_Cod = rdest.Ricetta_Dettaglio_Cod  ")

            'strSql.AppendLine("           WHERE   ro.Ricetta_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            'strSql.AppendLine("           AND     rdest.Tipo_Destinazione = " & AgronicaCoreDataProvider.CostantiPersonalizzate.MAGAZZINO)
            'strSql.AppendLine("           AND     rdet.Cau_Mov IN ( '" & AgronicaCoreDataProvider.CostantiPersonalizzate.CAU_SCARICO & "', '" & AgronicaCoreDataProvider.CostantiPersonalizzate.CAU_CONFERIMENTO_DIVERSI & "' )  ")
            'strSql.AppendLine("           AND     ro.Validita_Inizio >= " & Agro_SQL_SaveDate(Dal_Data_Verifica))
            'strSql.AppendLine("           AND     ro.Validita_Inizio <= " & Agro_SQL_SaveDate(Al_Data_Verifica))

            'If Piva <> "" Then
            '    strSql.AppendLine(" AND rdest.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            'End If


            strSql.AppendLine(" SELECT ISNULL(-SUM(round( rdet.Qta_extra_totale , +5)),0) as Qta_Totale_Minus  ")

            strSql.AppendLine("           FROM    Ricette r ")
            strSql.AppendLine("           INNER JOIN Ricette_Operazioni ro ON r.Ricetta_SuperUser = ro.Ricetta_SuperUser AND r.Ricetta_Cod = ro.Ricetta_Cod ")
            strSql.AppendLine("           INNER JOIN Ricette_Dettagli rdet ON ro.Ricetta_SuperUser=rdet.Ricetta_SuperUser AND ro.Ricetta_Cod = rdet.Ricetta_Cod AND ro.Ricetta_Operazione_Cod = rdet.Ricetta_Operazione_Cod  ")

            strSql.AppendLine("           WHERE   ro.Ricetta_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            strSql.AppendLine("           AND     rdet.Cau_Mov IN ( '" & CAU_LAVORAZIONE & "')  ")
            strSql.AppendLine("           AND     ro.Validita_Inizio >= " & Agro_SQL_SaveDate(Dal_Data_Verifica))
            strSql.AppendLine("           AND     ro.Validita_Inizio <= " & Agro_SQL_SaveDate(Al_Data_Verifica))

            If Piva <> "" Then
                strSql.AppendLine(" AND r.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If

            If Elem_Cod <> 0 Then
                strSql.AppendLine(" AND rdet.Elem_Cod = " & Agro_SQL_SaveNum(Elem_Cod) & "   ")
            End If

            If Pro_Cod <> 0 Then
                strSql.AppendLine(" AND rdet.Pro_Cod = " & Agro_SQL_SaveNum(Pro_Cod) & "   ")
            End If

            If Mat_Cod <> 0 Then
                strSql.AppendLine(" AND rdet.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & "   ")
            End If

            If Udm_Cod <> 0 Then
                strSql.AppendLine(" AND rdet.Udm_Cod = " & Agro_SQL_SaveNum(Udm_Cod) & "   ")
            End If

            If Ricetta_Cod <> 0 Then
                strSql.AppendLine(" AND ro.Ricetta_Cod = " & Agro_SQL_SaveNum(Ricetta_Cod) & "   ")
            End If

            If Ricetta_Operazione_Cod_daNon_considerare <> 0 Then
                strSql.AppendLine(" AND ro.Ricetta_Operazione_Cod = " & Agro_SQL_SaveNum(Ricetta_Operazione_Cod_daNon_considerare) & "   ")
            End If

            strSql.AppendLine(" ) AS Verifica_Giacenze")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then
            Return dt.Rows(0).Item("Verifica_Giacenze")
        Else
            Return 0
        End If

    End Function

    Public Function Verifica_Giacenze_KG_L_Con_Pua_NEW(ByVal Qta_Kg_L_PUA As Decimal,
                                                       ByVal Piva As String,
                                                       ByVal Ricetta_Cod As Integer,
                                                       ByVal Elem_Cod As Integer,
                                                       ByVal Pro_Cod As Integer,
                                                       ByVal Mat_Cod As Integer,
                                                       ByVal Dal_Data_Verifica As Date,
                                                       ByVal Al_Data_Verifica As Date,
                                                       ByVal Ricetta_Operazione_Cod_daNon_considerare As Integer,
                                                       ByRef objParametri As AgronicaCoreParametri
                                                       ) As Decimal

        Const nomeRoutine = "AgronicaCoreContabDAL.Ricette_Dettagli_R.Verifica_Giacenze_KG_L_Con_Pua_NEW()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0
            strSql.AppendLine(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; ")
            strSql.AppendLine(" SELECT ISNULL(-SUM(ROUND((  ")
            strSql.AppendLine("     CASE  ")
            strSql.AppendLine("     WHEN rdet.extra_int = " & enum_UnitaMisura.KG & " THEN rdet.Qta_extra_totale   ")
            strSql.AppendLine("     WHEN rdet.extra_int = " & enum_UnitaMisura.Litri & " THEN rdet.Qta_extra_totale  ")
            strSql.AppendLine("     WHEN rdet.extra_int = " & enum_UnitaMisura.Metri_Cubi & " THEN rdet.Qta_extra_totale * 1000 ")
            strSql.AppendLine("     WHEN rdet.extra_int = " & enum_UnitaMisura.Tonnellate & " THEN rdet.Qta_extra_totale * 1000 ")
            strSql.AppendLine("     WHEN rdet.extra_int = " & enum_UnitaMisura.Quintali & " THEN rdet.Qta_extra_totale * 100 ")

            strSql.AppendLine("     WHEN rdet.extra_int = " & enum_UnitaMisura.CentimetriCubi & " THEN rdet.Qta_extra_totale / 1000 ")
            strSql.AppendLine("     WHEN rdet.extra_int = " & enum_UnitaMisura.Millilitri & " THEN rdet.Qta_extra_totale / 1000 ")

            strSql.AppendLine("     WHEN rdet.extra_int = " & enum_UnitaMisura.Grammi & " THEN rdet.Qta_extra_totale / 1000 ")
            strSql.AppendLine("     ELSE 0 END), +5)), 0) as Qta_Totale_Minus ")

            strSql.AppendLine(" FROM Ricette r ")
            strSql.AppendLine(" INNER JOIN Ricette_Operazioni ro ON r.Ricetta_SuperUser = ro.Ricetta_SuperUser AND r.Ricetta_Cod = ro.Ricetta_Cod ")
            strSql.AppendLine(" INNER JOIN Ricette_Dettagli rdet ON ro.Ricetta_SuperUser=rdet.Ricetta_SuperUser AND ro.Ricetta_Cod = rdet.Ricetta_Cod AND ro.Ricetta_Operazione_Cod = rdet.Ricetta_Operazione_Cod  ")

            strSql.AppendLine(" WHERE ro.Ricetta_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            strSql.AppendLine(" AND rdet.Cau_Mov IN ( '" & CAU_LAVORAZIONE & "')  ")
            strSql.AppendLine(" AND ro.Validita_Inizio >= " & Agro_SQL_SaveDate(Dal_Data_Verifica))
            strSql.AppendLine(" AND ro.Validita_Inizio <= " & Agro_SQL_SaveDate(Al_Data_Verifica))

            If Piva <> "" Then
                strSql.AppendLine(" AND r.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If

            If Elem_Cod <> 0 Then
                strSql.AppendLine(" AND rdet.Elem_Cod = " & Agro_SQL_SaveNum(Elem_Cod) & "   ")
            End If

            If Pro_Cod <> 0 Then
                strSql.AppendLine(" AND rdet.Pro_Cod = " & Agro_SQL_SaveNum(Pro_Cod) & "   ")
            End If

            If Mat_Cod <> 0 Then
                strSql.AppendLine(" AND rdet.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & "   ")
            End If

            If Ricetta_Cod <> 0 Then
                strSql.AppendLine(" AND ro.Ricetta_Cod = " & Agro_SQL_SaveNum(Ricetta_Cod) & "   ")
            End If

            If Ricetta_Operazione_Cod_daNon_considerare <> 0 Then
                strSql.AppendLine(" AND ro.Ricetta_Operazione_Cod <> " & Agro_SQL_SaveNum(Ricetta_Operazione_Cod_daNon_considerare) & "   ")
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then
            Return Qta_Kg_L_PUA + dt.Rows(0).Item("Qta_Totale_Minus")
        Else
            Return 0
        End If

    End Function

    Public Function LeggiOreOperatore(ByVal Ricetta_Operazione_Cod As Integer, ByVal Mat_Cod As Integer, ByRef objParametri As AgronicaCoreParametri) As DataTable
        Const nomeRoutine = "AgronicaCoreContabDAL.Ricette_Dettagli_R.LeggiOreOperatorePerApp()"

        Dim messaggioErrore As String = ""
        Dim sb As New StringBuilder
        Dim dt As DataTable

        Try
            sb.AppendLine(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; ")

            sb.AppendLine(" SELECT ")
            sb.AppendLine("     Qta ")
            sb.AppendLine(" FROM ")
            sb.AppendLine("     Ricette_Dettagli ")
            sb.AppendLine(" WHERE ")
            sb.AppendLine("     Ricetta_Operazione_Cod = " & Agro_SQL_SaveNum(Ricetta_Operazione_Cod) & " ")
            sb.AppendLine("     AND Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & " ")
            sb.AppendLine("     AND Elem_Cod = " & Agro_SQL_SaveNum(ELEMCOD_MANODOPERA) & " ")
            sb.AppendLine($"     AND Cau_Mov IN ('{CAU_IMPUTAZIONE_MANODOPERA}', '{CAU_IMPUTAZIONE_TECNICO_RESPONSABILE}', '{CAU_IMPUTAZIONE_TERZISTI}')")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, sb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore, ex)
        End Try

        Return dt

    End Function

    Public Function LeggiOreMacchina(ByVal Ricetta_Operazione_Cod As Integer, ByVal Mat_Cod As Integer, ByRef objParametri As AgronicaCoreParametri) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Ricette_Dettagli_R.LeggiOreMacchinaPerApp()"

        Dim messaggioErrore As String = ""
        Dim sb As New StringBuilder
        Dim dt As DataTable

        Try
            sb.AppendLine(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; ")

            sb.AppendLine(" SELECT ")
            sb.AppendLine("     Qta ")
            sb.AppendLine(" FROM ")
            sb.AppendLine("     Ricette_Dettagli ")
            sb.AppendLine(" WHERE ")
            sb.AppendLine("     Ricetta_Operazione_Cod = " & Agro_SQL_SaveNum(Ricetta_Operazione_Cod) & " ")
            sb.AppendLine("     AND Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & " ")
            sb.AppendLine("     AND Elem_Cod = " & Agro_SQL_SaveNum(MACCHINE) & " ")
            sb.AppendLine($"     AND Cau_Mov = '{CAU_IMPUTAZIONE_PARCOMACCHINE}'")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, sb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore, ex)
        End Try

        Return dt

    End Function

End Class


'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################


Public Class Ricette_Dettagli_W
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function Scrivi(ByVal Ricetta_Cod As Int32,
                           ByVal Ricetta_Operazione_Cod As Int32,
                           ByVal Ricetta_Dettaglio_Cod As Int32,
                           ByVal Miscela_Cod As Int32,
                           ByVal Elem_Cod As Int32,
                           ByVal Pro_Cod As Int32,
                           ByVal Mat_Cod As Int32,
                           ByVal Udm_Cod As Int32,
                           ByVal Qta As Decimal,
                           ByVal Extra_Int As Int32,
                           ByVal Prezzo_Unitario As Decimal,
                           ByVal Cau_Mov As String,
                           ByVal Qualifica_Cod As Integer,
                           ByVal Tariffa_Cod As Integer,
                           ByVal Validita_Inizio As Date,
                           ByVal Validita_Fine As Date,
                           ByRef objParametri As AgronicaCoreParametri,
                           Optional ByVal Data_creazione As Date = #2/1/1900#,
                           Optional ByVal Data_modifica As Date = #2/1/1900#,
                           Optional ByVal username_creazione As String = "",
                           Optional ByVal username_modifica As String = "",
                           Optional ByVal TempoCarenza As Integer = 0,
                           Optional ByVal DoseEtichetta As String = "",
                           Optional ByVal PrincipiAttivi As String = "",
                           Optional ByVal ClassiTossicologiche As String = "",
                           Optional ByVal DoseEtichetta_Value As String = "",
                           Optional ByVal Turno_Cod As Integer = 0,
                           Optional ByVal ID_Attivita As Integer = 0,
                           Optional ByVal Lotto As String = "",
                           Optional ByVal Qta_Extra As Decimal = 0,
                           Optional ByVal Qta_Extra_Totale As Decimal = 0,
                           Optional ByVal Udm_Cod_Extra As Integer = 0,
                           Optional ByVal Mezzo_Det As Integer = 0,
                           Optional ByVal PrincipiAttiviPesi As String = "",
                           Optional ByVal Buffer As String = "",
                           Optional ByVal Extra_Str As String = "",
                           Optional ByVal PrincipiAttiviPercAbb As String = "",
                           Optional ByVal Polverulento As Integer = 0
                           ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.Ricette_Dettagli_W.Scrivi()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            If Data_creazione = #2/1/1900# Then
                Data_creazione = DateTime.Now
            End If

            If Data_modifica = #2/1/1900# Then
                Data_modifica = DateTime.Now
            End If

            If username_creazione = "" Then
                username_creazione = objParametri.UsernameOperazione
            End If

            If username_modifica = "" Then
                username_modifica = objParametri.UsernameOperazione
            End If

            '---------------------------------------------
            strSql.Length = 0

            strSql.Append(" INSERT INTO Ricette_Dettagli ")
            strSql.Append("         ( ")
            strSql.Append("          Ricetta_SuperUser,      Ricetta_Cod,       Ricetta_Operazione_Cod, ")
            strSql.Append("          Ricetta_Dettaglio_Cod,  Miscela_Cod,       ")
            strSql.Append("          Elem_Cod,               Pro_Cod,           Mat_Cod,      ")
            strSql.Append("          Udm_Cod,                Qta,               Extra_Int,    Prezzo_Unitario,  Cau_Mov, TempoCarenza, DoseEtichetta, PrincipiAttivi, PrincipiAttiviPesi, Buffer, ClassiTossicologiche, DoseEtichetta_Value, Turno_Cod, ID_Attivita, Lotto, Qualifica_Cod, Tariffa_Cod, ")
            strSql.Append("          Qta_Extra,              Qta_Extra_Totale,  Udm_Cod_Extra,    Mezzo_Det,  Extra_Str,  PrincipiAttiviPercAbb, Polverulento, ")

            strSql.Append("          DataLock, Inviato,            DataInvio, ")
            strSql.Append("          Data_Creazione,     Data_Modifica, ")
            strSql.Append("          UserName_Creazione, UserName_Modifica, ")
            strSql.Append("          Validita_Inizio,    Validita_Fine ")
            strSql.Append("         ) ")

            strSql.Append(" VALUES ( ")
            strSql.Append("          '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            strSql.Append("         , " & Agro_SQL_SaveNum(Ricetta_Cod))
            strSql.Append("         , " & Agro_SQL_SaveNum(Ricetta_Operazione_Cod) & " ")
            strSql.Append("         , " & Agro_SQL_SaveNum(Ricetta_Dettaglio_Cod) & " ")
            strSql.Append("         , " & Agro_SQL_SaveNum(Miscela_Cod) & " ")
            strSql.Append("         , " & Agro_SQL_SaveNum(Elem_Cod) & "  ")
            strSql.Append("         , " & Agro_SQL_SaveNum(Pro_Cod) & "  ")
            strSql.Append("         , " & Agro_SQL_SaveNum(Mat_Cod) & "  ")
            strSql.Append("         , " & Agro_SQL_SaveNum(Udm_Cod) & "  ")
            strSql.Append("         , " & Agro_SQL_SaveNum(Qta) & "  ")
            strSql.Append("         , " & Agro_SQL_SaveNum(Extra_Int) & "  ")
            strSql.Append("         , " & Agro_SQL_SaveNum(Prezzo_Unitario) & "  ")
            strSql.Append("         ,'" & Agro_SQL_SaveText(Cau_Mov) & "'  ")
            strSql.Append("         , " & Agro_SQL_SaveNum(TempoCarenza) & "  ")
            strSql.Append("         ,'" & Agro_SQL_SaveText(DoseEtichetta) & "'  ")
            strSql.Append("         ,'" & Agro_SQL_SaveText(PrincipiAttivi) & "'  ")
            strSql.Append("         ,'" & Agro_SQL_SaveText(PrincipiAttiviPesi) & "'  ")
            strSql.Append("         ,'" & Agro_SQL_SaveText(Buffer) & "'  ")
            strSql.Append("         ,'" & Agro_SQL_SaveText(ClassiTossicologiche) & "'  ")
            strSql.Append("         ,'" & Agro_SQL_SaveText(DoseEtichetta_Value) & "'  ")
            strSql.Append("         , " & Agro_SQL_SaveNum(Turno_Cod) & "  ")
            strSql.Append("         , " & Agro_SQL_SaveNum(ID_Attivita) & "  ")
            strSql.Append("         ,'" & Agro_SQL_SaveText(Lotto) & "'  ")

            strSql.Append("         , " & Agro_SQL_SaveNum(Qualifica_Cod) & "  ")
            strSql.Append("         , " & Agro_SQL_SaveNum(Tariffa_Cod) & "  ")

            strSql.Append("         , " & Agro_SQL_SaveNum(Qta_Extra) & "  ")
            strSql.Append("         , " & Agro_SQL_SaveNum(Qta_Extra_Totale) & "  ")
            strSql.Append("         , " & Agro_SQL_SaveNum(Udm_Cod_Extra) & "  ")
            strSql.Append("         , " & Agro_SQL_SaveNum(Mezzo_Det) & "  ")
            strSql.Append("         ,'" & Agro_SQL_SaveText(Extra_Str) & "'  ")
            strSql.Append("         ,'" & Agro_SQL_SaveText(PrincipiAttiviPercAbb) & "'  ")
            strSql.Append("         , " & Agro_SQL_SaveNum(Polverulento) & "  ")
            strSql.Append("         , 0 ")
            strSql.Append("         , 0  ")
            strSql.Append("         , Null  ")

            strSql.Append("			, " & Agro_SQL_SaveDateTime(Data_creazione) & "  ")
            strSql.Append("			, " & Agro_SQL_SaveDateTime(Data_modifica) & "  ")
            strSql.Append("			,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            strSql.Append("			,'" & Agro_SQL_SaveText(username_modifica) & "' ")

            strSql.Append("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            strSql.Append("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")

            strSql.Append(") ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function

    '##############################################################################################
    Public Function Modifica(ByVal Ricetta_Cod As Int32,
                             ByVal Ricetta_Operazione_Cod As Int32,
                             ByVal Ricetta_Dettaglio_Cod As Int32,
                             ByVal Miscela_Cod As Int32,
                             ByVal Elem_Cod As Int32,
                             ByVal Pro_Cod As Int32,
                             ByVal Mat_Cod As Int32,
                             ByVal Udm_Cod As Int32,
                             ByVal Qta As Decimal,
                             ByVal Extra_Int As Int32,
                             ByVal Prezzo_Unitario As Decimal,
                             ByVal Cau_Mov As String,
                             ByVal Qualifica_Cod As Integer,
                             ByVal Tariffa_Cod As Integer,
                             ByVal Validita_Inizio As Date,
                             ByVal Validita_Fine As Date,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreParametri
                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.Ricette_Dettagli_W.Modifica()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            If objParametri.PivaSuperUser = "" Then
                Throw New Exception("Parametro non corretto nella query (Ricetta_SuperUser obbligatorio)")
            End If

            '---------------------------------------------

            strSql.Length = 0

            strSql.Append(" UPDATE Ricette_Dettagli SET ")
            strSql.Append("    Elem_Cod        =  " & Agro_SQL_SaveNum(Elem_Cod) & "  ")
            strSql.Append("   ,Pro_Cod         =  " & Agro_SQL_SaveNum(Pro_Cod) & "  ")
            strSql.Append("   ,Mat_Cod         =  " & Agro_SQL_SaveNum(Mat_Cod) & "  ")
            strSql.Append("   ,Udm_Cod         =  " & Agro_SQL_SaveNum(Udm_Cod) & "  ")
            strSql.Append("   ,Qta             =  " & Agro_SQL_SaveNum(Qta) & "   ")
            strSql.Append("   ,Extra_Int       =  " & Agro_SQL_SaveNum(Extra_Int) & "  ")
            strSql.Append("   ,Prezzo_Unitario =  " & Agro_SQL_SaveNum(Prezzo_Unitario) & "  ")
            strSql.Append("   ,Cau_Mov         =  '" & Agro_SQL_SaveText(Cau_Mov) & "'  ")
            strSql.Append("   ,Qualifica_Cod       =  " & Agro_SQL_SaveNum(Qualifica_Cod) & "  ")
            strSql.Append("   ,Tariffa_Cod       =  " & Agro_SQL_SaveNum(Tariffa_Cod) & "  ")

            strSql.Append("   ,Inviato           =  0 ")
            strSql.Append("   ,DataInvio         =  Null ")
            strSql.Append("   ,Data_Modifica     =  " & Agro_SQL_SaveDateTime(DateTime.Now))
            strSql.Append("   ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            strSql.Append("   ,Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Inizio))
            strSql.Append("   ,Validita_Fine     =  " & Agro_SQL_SaveDate(Validita_Fine))

            strSql.Append(" WHERE  Ricetta_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

            If Ricetta_Cod <> 0 Then
                strSql.Append(" AND Ricette_Dettagli.Ricetta_Cod = " & Agro_SQL_SaveNum(Ricetta_Cod) & "   ")
            End If

            If Ricetta_Operazione_Cod <> 0 Then
                strSql.Append(" AND Ricette_Dettagli.Ricetta_operazione_Cod = " & Agro_SQL_SaveNum(Ricetta_Operazione_Cod) & "   ")
            End If

            If Ricetta_Dettaglio_Cod <> 0 Then
                strSql.Append(" AND Ricette_Dettagli.Ricetta_Dettaglio_Cod = " & Agro_SQL_SaveNum(Ricetta_Dettaglio_Cod) & "   ")
            End If

            If Cau_Mov <> "" Then
                strSql.Append(" AND Ricette_Dettagli.Cau_Mov = '" & Agro_SQL_SaveText(Cau_Mov) & "'   ")
            End If

            If Miscela_Cod <> 0 Then
                strSql.Append(" AND Ricette_Dettagli.miscela_Cod = " & Agro_SQL_SaveNum(Miscela_Cod) & "   ")
            End If

            '----------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                strSql.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function

    '##############################################################################################
    Public Function Cancella(ByVal Ricetta_Cod As Int32,
                             ByVal Ricetta_Operazione_Cod As Int32,
                             ByVal Ricetta_Dettaglio_Cod As Int32,
                             ByVal Miscela_Cod As Int32,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreParametri
                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.Ricette_Dettagli_W.Cancella()"

        '====================================================================================
        'Parametri opzionali :
        '   Ricetta_Cod = 0
        '   Ricetta_Operazione_Cod = 0
        '   Ricetta_Dettaglio_Cod = 0
        '   Miscela_Cod = 0
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            'If Piva = "" Then
            '    Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
            'End If

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then

                strSql.Length = 0
                strSql.Append(" UPDATE Ricette_Dettagli ")
                strSql.Append(" SET ")
                strSql.Append("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                strSql.Append("      ,Inviato = -1 ")
                strSql.Append(" WHERE  Inviato >= 0 ")

            Else
                strSql.Length = 0
                strSql.Append(" DELETE ")
                strSql.Append(" FROM Ricette_Dettagli ")
                strSql.Append(" WHERE  1=1 ")

            End If

            If objParametri.PivaSuperUser <> "" Then
                strSql.Append(" AND Ricette_Dettagli.Ricetta_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'   ")
            End If

            If Ricetta_Cod <> 0 Then
                strSql.Append(" AND Ricette_Dettagli.Ricetta_Cod = " & Agro_SQL_SaveNum(Ricetta_Cod) & "   ")
            End If

            If Ricetta_Operazione_Cod <> 0 Then
                strSql.Append(" AND Ricette_Dettagli.Ricetta_Operazione_Cod = " & Agro_SQL_SaveNum(Ricetta_Operazione_Cod) & "   ")
            End If

            If Ricetta_Dettaglio_Cod <> 0 Then
                strSql.Append(" AND Ricette_Dettagli.Ricetta_Dettaglio_Cod = " & Agro_SQL_SaveNum(Ricetta_Dettaglio_Cod) & "   ")
            End If

            If Miscela_Cod <> 0 Then
                strSql.Append(" AND Ricette_Dettagli.Miscela_Cod = " & Agro_SQL_SaveNum(Miscela_Cod) & "   ")
            End If

            If xFiltroAggiuntivo <> "" Then
                strSql.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function

    Public Function AggiornaOreOperatore(ByVal Ricetta_Operazione_Cod As Integer, ByVal Mat_Cod As Integer, ByVal oreLavorate As Decimal, ByRef objParametri As AgronicaCoreParametri)

        Const nomeRoutine = "AgronicaCoreContabDAL.Ricette_Dettagli_R.AggiornaOreOperatore()"

        Dim messaggioErrore As String = ""
        Dim sb As New StringBuilder
        Dim risposta As Boolean = False

        Try
            sb.AppendLine(" UPDATE ")
            sb.AppendLine("     Ricette_Dettagli ")
            sb.AppendLine(" SET ")
            sb.AppendLine("     Qta = " & Agro_SQL_SaveNum(oreLavorate))
            sb.AppendLine(" WHERE ")
            sb.AppendLine("     Ricetta_Operazione_Cod = " & Agro_SQL_SaveNum(Ricetta_Operazione_Cod))
            sb.AppendLine("     AND Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & " ")
            sb.AppendLine("     AND Elem_Cod = " & Agro_SQL_SaveNum(ELEMCOD_MANODOPERA) & " ")
            sb.AppendLine($"     AND Cau_Mov IN ('{CAU_IMPUTAZIONE_MANODOPERA}', '{CAU_IMPUTAZIONE_TECNICO_RESPONSABILE}', '{CAU_IMPUTAZIONE_TERZISTI}')")

            '--------------------------------------------------------------------------
            risposta = EseguiQuery_Scrittura(objParametri, sb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            risposta = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return risposta

    End Function


    Public Function AggiornaOreMacchina(ByVal Ricetta_Operazione_Cod As Integer, ByVal Mat_Cod As Integer, ByVal oreLavorate As Decimal, ByRef objParametri As AgronicaCoreParametri)

        Const nomeRoutine = "AgronicaCoreContabDAL.Ricette_Dettagli_R.AggiornaOreMacchina()"

        Dim messaggioErrore As String = ""
        Dim sb As New StringBuilder
        Dim risposta As Boolean = False

        Try
            sb.AppendLine(" UPDATE ")
            sb.AppendLine("     Ricette_Dettagli ")
            sb.AppendLine(" SET ")
            sb.AppendLine("     Qta = " & Agro_SQL_SaveNum(oreLavorate))
            sb.AppendLine(" WHERE ")
            sb.AppendLine("     Ricetta_Operazione_Cod = " & Agro_SQL_SaveNum(Ricetta_Operazione_Cod))
            sb.AppendLine("     AND Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & " ")
            sb.AppendLine("     AND Elem_Cod = " & Agro_SQL_SaveNum(MACCHINE) & " ")
            sb.AppendLine($"     AND Cau_Mov = '{CAU_IMPUTAZIONE_PARCOMACCHINE}'")

            '--------------------------------------------------------------------------
            risposta = EseguiQuery_Scrittura(objParametri, sb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            risposta = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return risposta

    End Function

End Class
