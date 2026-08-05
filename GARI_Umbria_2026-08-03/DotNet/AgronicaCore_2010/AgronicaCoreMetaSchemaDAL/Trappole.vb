Imports System.Text
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri


Public Class Trappole_R
    Inherits AgronicaCoreDataProvider.DataProvider


    '##############################################################################################
    Public Function Leggi(ByVal Trap_Cod As Integer,
                          ByVal Uso As Integer,
                          ByVal Veg_Cod As Integer,
                          ByVal Av_Cod As Integer,
                          ByVal xSelezioneVariabile As enumSelezioneVariabile,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreParametri
                          ) As DataTable

        Const nomeRoutine = "AgronicaCoreMetaschemaDAL.Trappole_R.Leggi()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try
            '---------------------------------------------

            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                    strSql.AppendLine(" SELECT TRAP_COD, TRAP_DES ")
                    strSql.AppendLine(" from Trappole ")
                    strSql.AppendLine(" where 1=1")

                    If Uso <> 0 Then
                        strSql.AppendLine(" AND Trappole.Uso = " & Uso.ToString & " ")
                    End If

                    If Trap_Cod <> 0 Then
                        strSql.AppendLine(" AND Trappole.Trap_Cod=" & Trap_Cod.ToString & " ")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        strSql.AppendLine(" ORDER BY Trap_Des ")
                    End If



                Case enumSelezioneVariabile.Selezione_TabellaCompleta

                    strSql.AppendLine(" SELECT Trappole.* ")
                    strSql.AppendLine(" from Trappole ")
                    strSql.AppendLine(" where 1=1")

                    If Uso <> 0 Then
                        strSql.AppendLine(" AND Trappole.Uso = " & Uso.ToString & " ")
                    End If

                    If Trap_Cod <> 0 Then
                        strSql.AppendLine(" AND Trappole.Trap_Cod=" & Trap_Cod.ToString & " ")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        strSql.AppendLine(" ORDER BY Trap_Des ")
                    End If


                Case enumSelezioneVariabile.Selezione_JoinDescrizioni
                    strSql.Length = 0

                    strSql.AppendLine(" SELECT DISTINCT Trappole.* ")
                    strSql.AppendLine(" FROM TrappoleXAvversita, Trappole, SpecieVegetaliXAvversita ")
                    strSql.AppendLine(" WHERE TrappoleXAvversita.Trap_Cod = Trappole.Trap_Cod ")
                    strSql.AppendLine(" AND SpecieVegetaliXAvversita.Av_Cod = TrappoleXAvversita.Av_Cod ")

                    If Uso <> 0 Then
                        strSql.AppendLine(" AND Trappole.Uso = " & Uso.ToString & " ")
                    End If

                    If Veg_Cod <> 0 Then
                        strSql.AppendLine(" AND SpecieVegetaliXAvversita.Veg_Cod=" & Veg_Cod.ToString & " ")
                    End If

                    If Av_Cod <> 0 Then
                        strSql.AppendLine(" AND TrappoleXAvversita.Av_Cod=" & Av_Cod.ToString & " ")
                    End If

                    If Trap_Cod <> 0 Then
                        strSql.AppendLine(" AND Trappole.Trap_Cod=" & Trap_Cod.ToString & " ")
                    End If


                    If xFiltroAggiuntivo <> "" Then
                        strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        strSql.AppendLine(" ORDER BY Trap_Des ")
                    End If

                Case enumSelezioneVariabile.Selezione_JoinCompleta


            End Select

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    Public Function Leggi_conCosti(ByVal Trap_Cod As Integer,
                                   ByVal xFiltroAggiuntivo As String,
                                   ByVal xOrderBy As String,
                                   ByRef objParametri As AgronicaCoreParametri
                                   ) As DataTable

        Const nomeRoutine = "AgronicaCoreMetaSchemaDAL.Trappole_R.Leggi_conCosti()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try


            '---------------------------------------------
            strSql.Length = 0

            'Formulati SENZA COSTI

            strSql.AppendLine(" (SELECT Trappole.*, 0 AS Prezzo_Unitario, '01/01/1900' as Validita_Inizio_Prezzo, '31/12/2100' as Validita_Fine_Prezzo,   " & _
                          "  0 as Udm_Cod_Prezzo, '' as Udm_Sim_Prezzo " & _
                          "  FROM  Trappole " & _
                          " WHERE Trappole.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " " & _
                          " AND   Trappole.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            strSql.AppendLine(" AND NOT EXISTS ( ")
            strSql.AppendLine("                 SELECT * ")
            strSql.AppendLine("                 FROM Prodotti_Costi ")
            strSql.AppendLine("                 WHERE Elem_Cod=197 ")
            strSql.AppendLine("                 AND Trappole.Trap_Cod = Prodotti_Costi.Pro_Cod And Prodotti_Costi.Id_Budget = 0 )")

            If Trap_Cod <> 0 Then
                strSql.AppendLine(" AND Trappole.TRAP_COD =  " & Agro_SQL_SaveNum(Trap_Cod) & "  ")
            End If

            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.AppendLine(" AND   Trappole.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.AppendLine(" AND   Trappole.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select

            strSql.AppendLine(") UNION ALL (")


            'Formulati CON COSTI

            strSql.AppendLine(" SELECT Trappole.*, Prodotti_Costi.Prezzo_Unitario, Prodotti_Costi.validita_inizio as Validita_Inizio_Prezzo, Prodotti_Costi.validita_fine as Validita_Fine_Prezzo,   " &
                          "  Prodotti_Costi.Udm_Cod as Udm_Cod_Prezzo, UnitaMisura.UDM_SIM as Udm_Sim_Prezzo " &
                          "  FROM Trappole INNER JOIN Prodotti_Costi ON Trappole.Trap_Cod = Prodotti_Costi.Pro_Cod And Prodotti_Costi.Id_Budget = 0 INNER JOIN UnitaMisura ON Prodotti_Costi.Udm_Cod = UnitaMisura.UDM_COD " &
                          "  WHERE Trappole.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " " &
                          "  AND   Trappole.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " " &
                          "  AND   Elem_Cod=197 ")

            If Trap_Cod <> 0 Then
                strSql.AppendLine(" AND Trappole.TRAP_COD =  " & Agro_SQL_SaveNum(Trap_Cod) & "  ")
            End If

            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.AppendLine(" AND   Trappole.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.AppendLine(" AND   Trappole.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select

            strSql.AppendLine(" ) ")

            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                strSql.AppendLine(" ORDER BY Trappole.Trap_DES ASC ")
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

        Return dt

    End Function

    Public Function LeggiInneschi_conCosti(ByVal Av_Cod As Integer,
                                           ByVal xFiltroAggiuntivo As String,
                                           ByVal xOrderBy As String,
                                           ByRef objParametri As AgronicaCoreParametri
                                           ) As DataTable

        Const nomeRoutine = "AgronicaCoreMetaSchemaDAL.Trappole_R.Leggi_conCosti()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            '---------------------------------------------
            strSql.Length = 0

            'Formulati SENZA COSTI

            strSql.AppendLine(" (SELECT Avversita.*, 0 AS Prezzo_Unitario, '01/01/1900' as Validita_Inizio_Prezzo, '31/12/2100' as Validita_Fine_Prezzo,   " & _
                          "  0 as Udm_Cod_Prezzo, '' as Udm_Sim_Prezzo " & _
                          "  FROM  Avversita " & _
                          " WHERE Avversita.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " " & _
                          " AND   Avversita.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            strSql.AppendLine(" AND NOT EXISTS ( ")
            strSql.AppendLine("                 SELECT * ")
            strSql.AppendLine("                 FROM Prodotti_Costi ")
            strSql.AppendLine("                 WHERE Elem_Cod=198 ")
            strSql.AppendLine("                 AND Avversita.Av_Cod = Prodotti_Costi.Pro_Cod And Prodotti_Costi.Id_Budget = 0 )")

            If Av_Cod <> 0 Then
                strSql.AppendLine(" AND Avversita.AV_COD =  " & Agro_SQL_SaveNum(Av_Cod) & "  ")
            End If

            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.AppendLine(" AND   Avversita.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.AppendLine(" AND   Avversita.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select

            strSql.AppendLine(") UNION ALL (")


            'Formulati CON COSTI

            strSql.AppendLine(" SELECT Avversita.*, Prodotti_Costi.Prezzo_Unitario, Prodotti_Costi.validita_inizio as Validita_Inizio_Prezzo, Prodotti_Costi.validita_fine as Validita_Fine_Prezzo,   " &
                          "  Prodotti_Costi.Udm_Cod as Udm_Cod_Prezzo, UnitaMisura.UDM_SIM as Udm_Sim_Prezzo " &
                          "  FROM Avversita INNER JOIN Prodotti_Costi ON Avversita.Av_Cod = Prodotti_Costi.Pro_Cod And Prodotti_Costi.Id_Budget = 0 INNER JOIN UnitaMisura ON Prodotti_Costi.Udm_Cod = UnitaMisura.UDM_COD " &
                          "  WHERE Avversita.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " " &
                          "  AND   Avversita.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " " &
                          "  AND   Elem_Cod=198 ")

            If Av_Cod <> 0 Then
                strSql.AppendLine(" AND Avversita.AV_COD =  " & Agro_SQL_SaveNum(Av_Cod) & "  ")
            End If

            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.AppendLine(" AND   Avversita.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.AppendLine(" AND   Avversita.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select

            strSql.AppendLine(" ) ")

            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                strSql.AppendLine(" ORDER BY Avversita.Av_Des_Vol ASC ")
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

        Return dt

    End Function


    '################################################################################
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="TrapCod"></param>
    ''' <param name="objParametri"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[magnani]	27/04/2011	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Function TrapDes_from_TrapCod(ByVal TrapCod As Integer, ByRef objParametri As AgronicaCoreParametri) As String

        Dim dt As DataTable

        'Recupero le informazioni
        dt = Leggi(CInt(TrapCod), 0, 0, 0, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri)

        If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
            Return dt.Rows(0).Item("Trap_Des")
        End If

        Return ""

    End Function


    '################################################################################
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="TrapCod"></param>
    ''' <param name="objParametri"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[magnani]	27/04/2011	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Function TrapDur_from_TrapCod(ByVal TrapCod As Integer, ByRef objParametri As AgronicaCoreParametri) As Integer

        Dim dt As New DataTable
        'Recupero le informazioni
        dt = Leggi(CInt(TrapCod), 0, 0, 0, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri)


        If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
            Return dt.Rows(0).Item("Trap_Dur")
        End If

        Return 0

    End Function


    Public Function Uso_from_TrapCod(ByVal TrapCod As Integer, ByRef objParametri As AgronicaCoreParametri) As Integer

        Dim dt As New DataTable
        'Recupero le informazioni
        dt = Leggi(CInt(TrapCod), 0, 0, 0, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri)


        If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
            Return dt.Rows(0).Item("Uso")
        End If

        Return 0

    End Function

    Function usoDesc_FromUso(uso As Integer) As String
        Select Case uso
            Case 1
                Return "Installazione Trappole"
            Case 2
                Return "Cattura di Massa"
            Case 3
                Return "Confusione Sessuale"
            Case 4
                Return "Disorientamento Sessuale"
            Case Else
                Return ""
        End Select

    End Function

    Function Trap_Dur_from_TrapCod(TrapCod As Integer, objParametri As AgronicaCoreParametri) As Integer

        Dim dt As New DataTable
        'Recupero le informazioni
        dt = Leggi(CInt(TrapCod), 0, 0, 0, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri)


        If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
            Return dt.Rows(0).Item("Trap_Dur")
        End If

        Return 0

    End Function

    'recupera la prima ditta
    Public Function DittaCod_from_TrapCod(Trap_Cod As Integer, ByRef objParametri As AgronicaCoreParametri) As Integer

        Dim dt As DataTable
        Dim dittaCod As Integer = 0

        If Trap_Cod = 0 Then
            Return 0
        End If

        dt = Leggi_TrappolexDitte(Trap_Cod, "", "", objParametri)

        If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
            dittaCod = CInt(dt.Rows(0).Item("DITTA_COD"))
        End If

        Return dittaCod

    End Function

    'recupera un dt che contiene per ogni trap_cod, un campo Ditta che contiene tutte le relative ditte concatenate
    'StrElencoTrapCod: separati da virgola
    Public Function DitteConcatenatexTrapCod(ByVal Trap_Cod As Integer,
                                             ByVal StrElencoTrapCod As String,
                                             ByRef objParametri As AgronicaCoreParametri
                                             ) As DataTable

        Const nomeRoutine = "AgronicaCoreMetaschemaDAL.Trappole_R.DitteConcatenatexTrapCod()"
        Dim messaggioErrore As String = ""
        Dim dtTrap As DataTable
        Dim FiltroAgg As String = ""

        Dim dt As New DataTable
        Dim drFine As DataRow

        dt.Columns.Add(New DataColumn("Trap_cod", GetType(Integer)))
        dt.Columns.Add(New DataColumn("Ditta", GetType(String)))

        Try
            'preparo il filtro
            If StrElencoTrapCod <> "" Then
                FiltroAgg = " TrappoleXDitta.Trap_Cod IN ( " & StrElencoTrapCod & ") "
            End If

            dtTrap = Leggi_TrappolexDitte(Trap_Cod, FiltroAgg, " Trappole.trap_cod, DITTA_des ", objParametri)

            If dtTrap IsNot Nothing AndAlso dtTrap.Rows.Count > 0 Then

                'elenco trap cod
                Dim VetTrapCod As String()
                VetTrapCod = StrElencoTrapCod.Split(",")

                For i = 0 To VetTrapCod.Length - 1

                    'trapxditte, filtro trap_cod per concatenare elenco ditte
                    Dim drTrap As DataRow() = dtTrap.Select(" Trap_Cod = " & Trim(VetTrapCod(i)).ToString)

                    Dim ditte As String = " "

                    For j = 0 To drTrap.Length - 1
                        If j <> drTrap.Length - 1 Then
                            ditte &= drTrap(j).Item("Ditta_Des") & ", "
                        Else
                            ditte &= drTrap(j).Item("Ditta_Des")
                        End If
                    Next

                    drFine = dt.NewRow
                    drFine("Trap_cod") = VetTrapCod(i)
                    drFine("Ditta") = ditte
                    dt.Rows.Add(drFine)

                Next

            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function


    Public Function Leggi_TrappolexDitte(ByVal Trap_Cod As Integer,
                                         ByVal xFiltroAggiuntivo As String,
                                         ByVal xOrderBy As String,
                                         ByRef objParametri As AgronicaCoreParametri
                                         ) As DataTable

        Const nomeRoutine = "AgronicaCoreMetaschemaDAL.Trappole_R.Leggi_TrappoleConDitte()"
        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0

            strSql.AppendLine(" SELECT Trappole.Trap_Cod, Trappole.Trap_des,  Ditte.Ditta_Cod, Ditte.Ditta_Des ")
            strSql.AppendLine(" FROM TrappoleXDitta ")
            strSql.AppendLine(" INNER JOIN Ditte ON TrappoleXDitta.Ditta_Cod = Ditte.Ditta_Cod ")
            strSql.AppendLine(" INNER JOIN Trappole ON TrappoleXDitta.Trap_Cod = Trappole.Trap_Cod ")
            strSql.AppendLine(" WHERE 1= 1 ")

            If Trap_Cod <> 0 Then
                strSql.AppendLine(" AND TrappoleXDitta.Trap_Cod=" & Trap_Cod.ToString & " ")
            End If

            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                strSql.AppendLine(" ORDER BY Trap_Des, Ditta_Des ")
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

        Return dt

    End Function


    Public Function Leggi_AvversitaxTrappole(ByVal Veg_Cod As Integer,
                                             ByVal Trap_Cod As Integer,
                                             ByVal Uso As Integer,
                                             ByVal xFiltroAggiuntivo As String,
                                             ByVal xOrderBy As String,
                                             ByRef objParametri As AgronicaCoreParametri
                                             ) As DataTable

        Const nomeRoutine = "AgronicaCoreMetaschemaDAL.Trappole_R.Leggi_AvversitaxTrappole()"

        Dim StrSQL As New StringBuilder
        Dim dt As DataTable

        StrSQL.Length = 0

        StrSQL.AppendLine(" SELECT DISTINCT Avversita.* ")
        StrSQL.AppendLine(" FROM TrappoleXAvversita, Avversita, SpecieVegetaliXAvversita,Trappole ")
        StrSQL.AppendLine(" WHERE SpecieVegetaliXAvversita.Av_Cod = Avversita.Av_Cod ")
        StrSQL.AppendLine(" AND SpecieVegetaliXAvversita.Av_Cod = TrappoleXAvversita.Av_Cod ")
        StrSQL.AppendLine(" AND Trappole.Trap_Cod = TrappoleXAvversita.Trap_Cod ")
        StrSQL.AppendLine(" AND (Av_Des_Vol NOT LIKE '%non usare%') AND (Av_Des_Vol NOT LIKE '%(#)%') AND (Av_Des_Lat NOT LIKE '%non usare%') AND (Av_Des_Lat NOT LIKE '%(#)%') ")

        If Uso <> 0 Then
            StrSQL.AppendLine(" AND Trappole.Uso = " & Uso.ToString & " ")
        End If

        If Veg_Cod <> 0 Then
            StrSQL.AppendLine(" AND SpecieVegetaliXAvversita.Veg_Cod=" & Veg_Cod.ToString & " ")
        End If

        If Trap_Cod <> 0 Then
            StrSQL.AppendLine(" AND Trappole.Trap_Cod = " & Trap_Cod.ToString & " ")
        End If

        If xFiltroAggiuntivo <> "" Then
            StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri) & " ")
        End If

        If xOrderBy <> "" Then
            StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri) & " ")
        End If

        Dim objDP As New AgronicaCoreDataProvider.DataProvider

        dt = objDP.EseguiQuery_Lettura(objParametri, StrSQL.ToString, nomeRoutine)

        objDP = Nothing

        Return dt

    End Function

End Class
