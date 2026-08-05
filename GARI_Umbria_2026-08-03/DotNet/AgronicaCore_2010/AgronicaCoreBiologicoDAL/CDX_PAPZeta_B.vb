Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi

Public Class CDX_PAPZeta_B_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function Leggi(ByVal PAP_Piva As String,
                          ByVal ID_PAPzoo As Integer,
                          ByVal ID_PAPzoo_B As Integer,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreParametri
                          ) As DataTable

        Const nomeRoutine = "AgronicaCoreBiologicoDAL.CDX_PAPZeta_B_R.Leggi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT * ")
            StrSQL.Append(" FROM  CDX_PAPZeta_B ")

            StrSQL.Append(" WHERE Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.Append(" AND   Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            StrSQL.Append(" AND PAP_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

            If PAP_Piva <> "" Then
                StrSQL.Append(" AND PAP_Piva = '" & Agro_SQL_SaveText(PAP_Piva) & "' ")
            End If

            If ID_PAPzoo <> 0 Then
                StrSQL.Append(" AND ID_PAPzoo = " & Agro_SQL_SaveNum(ID_PAPzoo) & "  ")
            End If

            If ID_PAPzoo_B <> 0 Then
                StrSQL.Append(" AND ID_PAPzoo_B = " & Agro_SQL_SaveNum(ID_PAPzoo_B) & "  ")
            End If

            '--------------------------------------------------------------------------
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
    Public Function LeggiMaxID_PAPzoo_B(ByVal xFiltroAggiuntivo As String,
                                        ByVal xOrderBy As String,
                                        ByRef objParametri As AgronicaCoreParametri
                                        ) As DataTable

        Const nomeRoutine = "AgronicaCoreBiologicoDAL.CDX_PAPZeta_B_R.LeggiMaxID_PAPzoo_B()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT ISNULL(MAX(ID_PAPzoo_B), 0) AS ULTIMO_ID_PAPzoo_B ")
            StrSQL.Append(" FROM  CDX_PAPZeta_B ")

            StrSQL.Append(" WHERE Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.Append(" AND   Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            StrSQL.Append(" AND PAP_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

            '--------------------------------------------------------------------------
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

    '###################################################################################
    Public Function Nuovo_ID_PAPzoo_B(ByRef objParametri As AgronicaCoreParametri) As Integer

        Const nomeRoutine = "AgronicaCoreBiologicoDAL.CDX_PAPZeta_B_R.Nuovo_ID_PAPzoo_B()"

        Dim ID_PAPzoo_B As Integer = 0
        Dim dt As DataTable

        Try

            dt = LeggiMaxID_PAPzoo_B("", "", objParametri)

            If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then
                ID_PAPzoo_B = dt.Rows(0).Item("ULTIMO_ID_PAPzoo_B")
            End If

            ID_PAPzoo_B += 1

        Catch ex As Exception
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
        End Try

        Return ID_PAPzoo_B

    End Function

End Class

Public Class CDX_PAPZeta_B_W
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function Scrivi(ByVal PAP_Piva As String,
                           ByVal ID_PAPzoo As Integer,
                           ByVal ID_PAPzoo_B As Integer,
                           ByVal UnitaProduttiva_Numero As String,
                           ByVal Codice_Specie As String,
                           ByVal Codice_Razza As String,
                           ByVal Codice_Categoria As String,
                           ByVal GEN_COD As Integer,
                           ByVal SPE_COD As Integer,
                           ByVal RAZ_COD As Integer,
                           ByVal IPRO_COD As Integer,
                           ByVal CAT_COD As Integer,
                           ByVal SpecieAnimali_Des As String,
                           ByVal CategoriaAnimali_Des As String,
                           ByVal RazzaAnimali_Des As String,
                           ByVal NUM_CAPI As Integer,
                           ByVal Num_Cicli As Decimal,
                           ByVal FLAG_BIOLOGICO As Integer,
                           ByVal REG_COD As Integer,
                           ByVal UDM_NUMERO As Integer,
                           ByVal PROD_DESCR As String,
                           ByVal PROD_UDM As Integer,
                           ByVal PROD_QTA As Decimal,
                           ByVal PROD_LOTTO As String,
                           ByVal PROD_CAT As Integer,
                           ByVal note As String,
                           ByVal Validita_Inizio As Date,
                           ByVal Validita_Fine As Date,
                           ByVal Validazione As Integer,
                           ByVal Data_Validazione As Date,
                           ByVal UserName_Validazione As String,
                           ByRef objParametri As AgronicaCoreParametri,
                           Optional ByVal Data_creazione As Date = #2/1/1900#,
                           Optional ByVal Data_modifica As Date = #2/1/1900#,
                           Optional ByVal username_creazione As String = "",
                           Optional ByVal username_modifica As String = ""
                           ) As Boolean

        Const nomeRoutine = "AgronicaCoreBiologicoDAL.CDX_PAPZeta_B_W.Scrivi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

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



            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append(" INSERT INTO CDX_PAPZeta_B " & vbCrLf)

            StrSQL.Append("             (PAP_SuperUser, PAP_Piva, ID_PAPzoo, ID_PAPzoo_B,  " & vbCrLf)
            StrSQL.Append("               UnitaProduttiva_Numero, Codice_Specie, Codice_Razza, Codice_Categoria,  " & vbCrLf)
            StrSQL.Append("               GEN_COD, SPE_COD, RAZ_COD, IPRO_COD, CAT_COD,  " & vbCrLf)
            StrSQL.Append("               SpecieAnimali_Des, CategoriaAnimali_Des, RazzaAnimali_Des, NUM_CAPI, Num_Cicli,  " & vbCrLf)
            StrSQL.Append("               FLAG_BIOLOGICO, REG_COD, UDM_NUMERO, " & vbCrLf)
            StrSQL.Append("              PROD_DESCR, PROD_UDM, PROD_QTA, PROD_LOTTO, PROD_CAT, note, " & vbCrLf)

            StrSQL.Append("              Inviato,            datainvio, ")
            StrSQL.Append("              Data_Creazione,     Data_Modifica, ")
            StrSQL.Append("              UserName_Creazione, UserName_Modifica, ")
            StrSQL.Append("              Validita_Inizio,    Validita_Fine, ")
            StrSQL.Append("              Validazione, Data_Validazione, UserName_Validazione, DataLock" & vbCrLf)
            StrSQL.Append("              ) ")

            StrSQL.Append(" VALUES ( ")
            StrSQL.Append(" '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'  ")
            StrSQL.Append(" ,'" & Agro_SQL_SaveText(Trim(PAP_Piva)) & "' " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(ID_PAPzoo)) & " " & vbCrLf)

            StrSQL.Append(" , " & Agro_SQL_SaveNum(Trim(ID_PAPzoo_B)) & " " & vbCrLf)
            StrSQL.Append(" ,'" & Agro_SQL_SaveText(Trim(UnitaProduttiva_Numero)) & "' " & vbCrLf)
            StrSQL.Append(" ,'" & Agro_SQL_SaveText(Trim(Codice_Specie)) & "' " & vbCrLf)
            StrSQL.Append(" ,'" & Agro_SQL_SaveText(Trim(Codice_Razza)) & "' " & vbCrLf)
            StrSQL.Append(" ,'" & Agro_SQL_SaveText(Trim(Codice_Categoria)) & "' " & vbCrLf)
            StrSQL.Append(" , " & Agro_SQL_SaveNum(Trim(GEN_COD)) & "  " & vbCrLf)
            StrSQL.Append(" , " & Agro_SQL_SaveNum(Trim(SPE_COD)) & "  " & vbCrLf)
            StrSQL.Append(" , " & Agro_SQL_SaveNum(Trim(RAZ_COD)) & "  " & vbCrLf)
            StrSQL.Append(" , " & Agro_SQL_SaveNum(Trim(IPRO_COD)) & "  " & vbCrLf)
            StrSQL.Append(" , " & Agro_SQL_SaveNum(Trim(CAT_COD)) & "  " & vbCrLf)
            StrSQL.Append(" ,'" & Agro_SQL_SaveText(Trim(SpecieAnimali_Des)) & "' " & vbCrLf)
            StrSQL.Append(" ,'" & Agro_SQL_SaveText(Trim(CategoriaAnimali_Des)) & "' " & vbCrLf)
            StrSQL.Append(" ,'" & Agro_SQL_SaveText(Trim(RazzaAnimali_Des)) & "' " & vbCrLf)
            StrSQL.Append(" , " & Agro_SQL_SaveNum(Trim(NUM_CAPI)) & "  " & vbCrLf)
            StrSQL.Append(" , " & Agro_SQL_SaveNum(Trim(Num_Cicli)) & "  " & vbCrLf)
            StrSQL.Append(" , " & Agro_SQL_SaveNum(Trim(FLAG_BIOLOGICO)) & "  " & vbCrLf)
            StrSQL.Append(" , " & Agro_SQL_SaveNum(Trim(REG_COD)) & "  " & vbCrLf)
            StrSQL.Append(" , " & Agro_SQL_SaveNum(Trim(UDM_NUMERO)) & "  " & vbCrLf)
            StrSQL.Append(" ,'" & Agro_SQL_SaveText(Trim(PROD_DESCR)) & "' " & vbCrLf)
            StrSQL.Append(" , " & Agro_SQL_SaveNum(Trim(PROD_UDM)) & "  " & vbCrLf)
            StrSQL.Append(" , " & Agro_SQL_SaveNum(Trim(PROD_QTA)) & "  " & vbCrLf)
            StrSQL.Append(" ,'" & Agro_SQL_SaveText(Trim(PROD_LOTTO)) & "' " & vbCrLf)
            StrSQL.Append(" ," & Agro_SQL_SaveNum(Trim(PROD_CAT)) & " " & vbCrLf)
            StrSQL.Append(" ,'" & Agro_SQL_SaveText(Trim(note)) & "' " & vbCrLf)

            StrSQL.Append("         , 0  " & vbCrLf)
            StrSQL.Append("         , Null  " & vbCrLf)

            StrSQL.Append("			, " & Agro_SQL_SaveDate(Data_creazione) & "  ")
            StrSQL.Append("			, " & Agro_SQL_SaveDate(Data_modifica) & "  ")
            StrSQL.Append("			,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            StrSQL.Append("			,'" & Agro_SQL_SaveText(username_modifica) & "' ")

            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  " & vbCrLf)
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  " & vbCrLf)
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Trim(Validazione)) & "  " & vbCrLf)
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Trim(Data_Validazione)) & " " & vbCrLf)
            StrSQL.Append("         , '" & Agro_SQL_SaveText(Trim(UserName_Validazione)) & "' " & vbCrLf)
            StrSQL.Append("         , 0  ")
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

End Class
