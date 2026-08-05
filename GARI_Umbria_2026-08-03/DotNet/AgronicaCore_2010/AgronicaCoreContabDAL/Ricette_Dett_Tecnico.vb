Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri


Public Class Ricette_Dett_Tecnico_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function Leggi(ByVal Ricetta_Cod As Int32,
                          ByVal Ricetta_Operazione_Cod As Int32,
                          ByVal Ricetta_Dettaglio_Cod As Int32,
                          ByVal Miscela_Cod As Int32,
                          ByVal Ricetta_Tecnico_Cod As Int32,
                          ByVal Validita_Inizio As Date,
                          ByVal Validita_Fine As Date,
                          ByVal xSelezioneVariabile As enumSelezioneVariabile,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreParametri,
                          Optional ByVal joinRicette As Boolean = False
                          ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Ricette_Dett_Tecnico_R.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   Ricetta_Cod = 0
        '   Ricetta_Operazione_Cod = 0
        '   Miscela_Cod = 0
        '   Ricetta_Tecnico_Cod = 0
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try

            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                    StrSQL.Length = 0

                    StrSQL.AppendLine(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; ")

                    StrSQL.Append(" SELECT Ricette_Dettaglio_Tecnico.* ")
                    StrSQL.Append(" FROM  Ricette_Dettaglio_Tecnico ")

                    If joinRicette Then
                        StrSQL.Append(" INNER JOIN  Ricette on Ricette.Ricetta_Cod = Ricette_Dettaglio_Tecnico.Ricetta_COD ")
                        StrSQL.Append(" INNER JOIN  Ricette_Operazioni on Ricette_Operazioni.Ricetta_operazione_Cod = Ricette_Dettaglio_Tecnico.Ricetta_Operazione_COD ")
                    End If

                    StrSQL.Append(" WHERE Ricette_Dettaglio_Tecnico.Validita_inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
                    StrSQL.Append(" AND   Ricette_Dettaglio_Tecnico.Validita_Fine >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")

                    If Ricetta_Dettaglio_Cod <> 0 Then
                        StrSQL.Append(" AND   Ricette_Dettaglio_Tecnico.Ricetta_Dettaglio_Cod = " & Agro_SQL_SaveNum(Ricetta_Dettaglio_Cod) & "   ")
                    End If

                    If objParametri.PivaSuperUser <> "" Then
                        StrSQL.Append(" AND Ricette_Dettaglio_Tecnico.Ricetta_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'   ")
                    End If

                    If Ricetta_Cod <> 0 Then
                        StrSQL.Append(" AND Ricette_Dettaglio_Tecnico.Ricetta_Cod = " & Agro_SQL_SaveNum(Ricetta_Cod) & "   ")
                    End If

                    If Ricetta_Operazione_Cod <> 0 Then
                        StrSQL.Append(" AND Ricette_Dettaglio_Tecnico.Ricetta_Operazione_Cod = " & Agro_SQL_SaveNum(Ricetta_Operazione_Cod) & "   ")
                    End If

                    If Miscela_Cod <> 0 Then
                        StrSQL.Append(" AND Ricette_Dettaglio_Tecnico.Miscela_Cod = " & Agro_SQL_SaveNum(Miscela_Cod) & "   ")
                    End If

                    If Ricetta_Tecnico_Cod <> 0 Then
                        StrSQL.Append(" AND Ricette_Dettaglio_Tecnico.Ricetta_Tecnico_Cod = " & Agro_SQL_SaveNum(Ricetta_Tecnico_Cod) & " ")
                    End If


                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   Ricette_Dettaglio_Tecnico.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Ricette_Dettaglio_Tecnico.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        If joinRicette Then
                            StrSQL.Append(" ORDER BY Ricette_Dettaglio_Tecnico.Ricetta_SuperUser, Ricette_Dettaglio_Tecnico.Ricetta_Cod, Ricette_Dettaglio_Tecnico.Ricetta_Operazione_Cod, Ricette_Dettaglio_Tecnico.Ricetta_Dettaglio_Cod, Ricette_Dettaglio_Tecnico.Ricetta_Tecnico_Cod Asc ")
                        Else
                            StrSQL.Append(" ORDER BY Ricetta_SuperUser, Ricetta_Cod, Ricetta_Operazione_Cod, Ricetta_Dettaglio_Cod, Ricetta_Tecnico_Cod Asc ")
                        End If
                    End If

                Case enumSelezioneVariabile.Selezione_TabellaCompleta

                    StrSQL.Length = 0

                    StrSQL.AppendLine(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; ")

                    StrSQL.Append(" SELECT Ricette_Dettaglio_Tecnico.* ")
                    StrSQL.Append(" FROM  Ricette_Dettaglio_Tecnico ")

                    If joinRicette Then
                        StrSQL.Append(" INNER JOIN  Ricette on Ricette.Ricetta_Cod = Ricette_Dettaglio_Tecnico.Ricetta_COD ")
                        StrSQL.Append(" INNER JOIN  Ricette_Operazioni on Ricette_Operazioni.Ricetta_operazione_Cod = Ricette_Dettaglio_Tecnico.Ricetta_Operazione_COD ")
                    End If

                    StrSQL.Append(" WHERE Ricette_Dettaglio_Tecnico.Validita_inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
                    StrSQL.Append(" AND   Ricette_Dettaglio_Tecnico.Validita_Fine >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")

                    If Ricetta_Dettaglio_Cod <> 0 Then
                        StrSQL.Append(" AND   Ricette_Dettaglio_Tecnico.Ricetta_Dettaglio_Cod = " & Agro_SQL_SaveNum(Ricetta_Dettaglio_Cod) & "   ")
                    End If

                    If objParametri.PivaSuperUser <> "" Then
                        StrSQL.Append(" AND Ricette_Dettaglio_Tecnico.Ricetta_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'   ")
                    End If

                    If Ricetta_Cod <> 0 Then
                        StrSQL.Append(" AND Ricette_Dettaglio_Tecnico.Ricetta_Cod = " & Agro_SQL_SaveNum(Ricetta_Cod) & "   ")
                    End If

                    If Ricetta_Operazione_Cod <> 0 Then
                        StrSQL.Append(" AND Ricette_Dettaglio_Tecnico.Ricetta_Operazione_Cod = " & Agro_SQL_SaveNum(Ricetta_Operazione_Cod) & "   ")
                    End If

                    If Miscela_Cod <> 0 Then
                        StrSQL.Append(" AND Ricette_Dettaglio_Tecnico.Miscela_Cod = " & Agro_SQL_SaveNum(Miscela_Cod) & "   ")
                    End If

                    If Ricetta_Tecnico_Cod <> 0 Then
                        StrSQL.Append(" AND Ricette_Dettaglio_Tecnico.Ricetta_Tecnico_Cod = " & Agro_SQL_SaveNum(Ricetta_Tecnico_Cod) & " ")
                    End If


                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   Ricette_Dettaglio_Tecnico.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Ricette_Dettaglio_Tecnico.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        If joinRicette Then
                            StrSQL.Append(" ORDER BY Ricette_Dettaglio_Tecnico.Ricetta_SuperUser, Ricette_Dettaglio_Tecnico.Ricetta_Cod, Ricette_Dettaglio_Tecnico.Ricetta_Operazione_Cod, Ricette_Dettaglio_Tecnico.Ricetta_Dettaglio_Cod, Ricette_Dettaglio_Tecnico.Ricetta_Tecnico_Cod Asc ")
                        Else
                            StrSQL.Append(" ORDER BY Ricetta_SuperUser, Ricetta_Cod, Ricetta_Operazione_Cod, Ricetta_Dettaglio_Cod, Ricetta_Tecnico_Cod Asc ")
                        End If
                    End If

                Case enumSelezioneVariabile.Selezione_JoinDescrizioni

                    StrSQL.Length = 0

                    StrSQL.AppendLine(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; ")

                    StrSQL.Append(" SELECT Ricette_Dettaglio_Tecnico.*, ")
                    StrSQL.Append(" ISNULL(Avversita.Av_Des_Vol,'') AS Av_Des_Vol, ISNULL(Avversita.Av_Des_Lat,'') AS Av_Des_Lat,  ")
                    StrSQL.Append(" ISNULL(GruppoAvversita.Av_Gru_Des,'') AS Av_Gru_Des, ISNULL(GruppoAvversita.Av_Gru_Des_Lat,'') AS Av_Gru_Des_Lat ")

                    If joinRicette Then
                        StrSQL.Append(" INNER JOIN  Ricette on Ricette.Ricetta_Cod = Ricette_Dettaglio_Tecnico.Ricetta_COD ")
                        StrSQL.Append(" INNER JOIN  Ricette_Operazioni on Ricette_Operazioni.Ricetta_operazione_Cod = Ricette_Dettaglio_Tecnico.Ricetta_Operazione_COD LEFT OUTER JOIN    ")
                    Else
                        StrSQL.Append(" FROM  Ricette_Dettaglio_Tecnico LEFT OUTER JOIN   ")
                    End If

                    StrSQL.Append(" Avversita ON Ricette_Dettaglio_Tecnico.Av_Cod = Avversita.Av_Cod LEFT OUTER JOIN ")
                    StrSQL.Append(" GruppoAvversita ON Ricette_Dettaglio_Tecnico.Av_Gru = GruppoAvversita.Av_Gru ")

                    StrSQL.Append(" WHERE Ricette_Dettaglio_Tecnico.Validita_inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
                    StrSQL.Append(" AND   Ricette_Dettaglio_Tecnico.Validita_Fine >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")

                    If Ricetta_Dettaglio_Cod <> 0 Then
                        StrSQL.Append(" AND   Ricette_Dettaglio_Tecnico.Ricetta_Dettaglio_Cod = " & Agro_SQL_SaveNum(Ricetta_Dettaglio_Cod) & "   ")
                    End If

                    If objParametri.PivaSuperUser <> "" Then
                        StrSQL.Append(" AND Ricette_Dettaglio_Tecnico.Ricetta_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'   ")
                    End If

                    If Ricetta_Cod <> 0 Then
                        StrSQL.Append(" AND Ricette_Dettaglio_Tecnico.Ricetta_Cod = " & Agro_SQL_SaveNum(Ricetta_Cod) & "   ")
                    End If

                    If Ricetta_Operazione_Cod <> 0 Then
                        StrSQL.Append(" AND Ricette_Dettaglio_Tecnico.Ricetta_Operazione_Cod = " & Agro_SQL_SaveNum(Ricetta_Operazione_Cod) & "   ")
                    End If

                    If Miscela_Cod <> 0 Then
                        StrSQL.Append(" AND Ricette_Dettaglio_Tecnico.Miscela_Cod = " & Agro_SQL_SaveNum(Miscela_Cod) & "   ")
                    End If

                    If Ricetta_Tecnico_Cod <> 0 Then
                        StrSQL.Append(" AND Ricette_Dettaglio_Tecnico.Ricetta_Tecnico_Cod = " & Agro_SQL_SaveNum(Ricetta_Tecnico_Cod) & " ")
                    End If


                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   Ricette_Dettaglio_Tecnico.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Ricette_Dettaglio_Tecnico.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        If joinRicette Then
                            StrSQL.Append(" ORDER BY Ricette_Dettaglio_Tecnico.Ricetta_SuperUser, Ricette_Dettaglio_Tecnico.Ricetta_Cod, Ricette_Dettaglio_Tecnico.Ricetta_Operazione_Cod, Ricette_Dettaglio_Tecnico.Ricetta_Dettaglio_Cod, Ricette_Dettaglio_Tecnico.Ricetta_Tecnico_Cod Asc ")
                        Else
                            StrSQL.Append(" ORDER BY Ricetta_SuperUser, Ricetta_Cod, Ricetta_Operazione_Cod, Ricetta_Dettaglio_Cod, Ricetta_Tecnico_Cod Asc ")
                        End If
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
    Public Function Leggi_x_lite(ByVal Ricetta_Cod As Int32,
                                 ByVal Ricetta_Operazione_Cod As Int32,
                                 ByVal Ricetta_Dettaglio_Cod As Int32,
                                 ByRef objParametri As AgronicaCoreParametri
                                 ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Ricette_Dett_Tecnico_R.Leggi_x_lite()"

        '====================================================================================
        'Parametri opzionali :
        '   Ricetta_Cod = 0
        '   Ricetta_Operazione_Cod = 0
        '   Ricetta_Dettaglio_Cod = 0
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New Text.StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0

            strSql.Append(" SELECT     Ricette_Dettaglio_Tecnico.Av_Gru, Ricette_Dettagli.Pro_Cod,Ricette_Dettagli.Mat_Cod, Ricette_Dettaglio_Tecnico.Av_Cod, ")
            strSql.Append("     Ricette_Dettaglio_Tecnico.Soglia_Cod, Ricette_Dettaglio_Tecnico.Soglia_Des, Ricette_Dettaglio_Tecnico.Soglia_Quantita, ")

            strSql.Append("     Ricette_Dettaglio_Tecnico.Mg, Ricette_Dettaglio_Tecnico.N, Ricette_Dettaglio_Tecnico.P, Ricette_Dettaglio_Tecnico.K, Ricette_Dettaglio_Tecnico.Efficienza ")

            strSql.Append(" FROM         Ricette_Dettaglio_Tecnico INNER JOIN ")
            strSql.Append("   Ricette_Dettagli ON Ricette_Dettaglio_Tecnico.Ricetta_SuperUser = Ricette_Dettagli.Ricetta_SuperUser AND  ")
            strSql.Append("   Ricette_Dettaglio_Tecnico.Ricetta_Cod = Ricette_Dettagli.Ricetta_Cod AND  ")
            strSql.Append("   Ricette_Dettaglio_Tecnico.Ricetta_Operazione_Cod = Ricette_Dettagli.Ricetta_Operazione_Cod AND  ")
            strSql.Append("   Ricette_Dettaglio_Tecnico.Ricetta_Dettaglio_Cod = Ricette_Dettagli.Ricetta_Dettaglio_Cod ")

            strSql.Append(" WHERE Ricette_Dettaglio_Tecnico.Ricetta_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'   ")

            If Ricetta_Dettaglio_Cod <> 0 Then
                strSql.Append(" AND   Ricette_Dettaglio_Tecnico.Ricetta_Dettaglio_Cod = " & Agro_SQL_SaveNum(Ricetta_Dettaglio_Cod) & "   ")
            End If

            If Ricetta_Cod <> 0 Then
                strSql.Append(" AND Ricette_Dettaglio_Tecnico.Ricetta_Cod = " & Agro_SQL_SaveNum(Ricetta_Cod) & "   ")
            End If

            If Ricetta_Operazione_Cod <> 0 Then
                strSql.Append(" AND Ricette_Dettaglio_Tecnico.Ricetta_Operazione_Cod = " & Agro_SQL_SaveNum(Ricetta_Operazione_Cod) & "   ")
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

    Public Function Leggi_x_avversita(ByVal Piva As String,
                                      ByVal xFiltroAggiuntivo As String,
                                      ByVal xOrderBy As String,
                                      ByRef objParametri As AgronicaCoreParametri
                                      ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Ricette_Dett_Tecnico_R.Leggi_x_avversita()"

        Dim messaggioErrore As String = ""
        Dim strSql As New Text.StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0
            strSql.AppendLine(" SELECT DISTINCT rdt.Ricetta_Cod, rdt.Ricetta_Operazione_Cod, rdt.Ricetta_Dettaglio_Cod, ISNULL(avversita.av_Des_Vol, '') AS av_Des_Vol , ISNULL(GruppoAvversita.av_gru_Des, '') AS av_gru_Des ")
            strSql.AppendLine(" FROM Ricette_Dettaglio_Tecnico rdt ")
            strSql.AppendLine(" INNER JOIN Ricette r ON r.Ricetta_Cod=rdt.Ricetta_Cod ")
            strSql.AppendLine(" LEFT JOIN Avversita ON Avversita.av_cod = rdt.av_cod AND rdt.av_cod <> 0 ")
            strSql.AppendLine(" LEFT JOIN GruppoAvversita ON GruppoAvversita.av_gru = rdt.av_gru AND rdt.av_gru <> 0 ")

            strSql.AppendLine(" WHERE (rdt.av_cod <> 0 OR rdt.av_gru <> 0) ")
            strSql.AppendLine(" AND (av_Des_Vol <> '' OR av_gru_Des <> '') ")

            '(26/07/2018) fede aggiunto filtro piva='' per leggere le ricette pubbliche
            If Piva <> "" Then
                strSql.AppendLine(" AND (r.Piva = '" & Agro_SQL_SaveText(Piva) & "' or  r.Piva = '') ")
            End If

            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.AppendLine(" AND   rdt.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.AppendLine(" AND   rdt.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                strSql.AppendLine(" ORDER BY rdt.Ricetta_Operazione_Cod ")
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

End Class


'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################


Public Class Ricette_Dett_Tecnico_W
    Inherits AgronicaCoreDataProvider.DataProvider


    '##############################################################################################
    Public Function Scrivi(ByVal Ricetta_Cod As Int32,
                           ByVal Ricetta_Operazione_Cod As Int32,
                           ByVal Ricetta_Dettaglio_Cod As Int32,
                           ByVal Miscela_Cod As Int32,
                           ByVal Ricetta_Tecnico_Cod As Int32,
                           ByVal Qta_Ril As Decimal,
                           ByVal Av_Cod As Int32,
                           ByVal Av_Gru As Int32,
                           ByVal Dett_Cod As Int32,
                           ByVal Dose As Decimal,
                           ByVal Parziale As Integer,
                           ByVal Nitrati As Integer,
                           ByVal Freatimetro As Decimal,
                           ByVal Inn1_Data As Date,
                           ByVal Inn2_Data As Date,
                           ByVal Inn3_Data As Date,
                           ByVal Inn4_Data As Date,
                           ByVal Ditta_Cod As Long,
                           ByVal Sigla_AV As String,
                           ByVal Trap_Num As Int32,
                           ByVal Id_Insetto As Int32,
                           ByVal FF_Classe As Int32,
                           ByVal Validita_Inizio As Date,
                           ByVal Validita_Fine As Date,
                           ByRef objParametri As AgronicaCoreParametri,
                           Optional ByVal Data_creazione As Date = #2/1/1900#,
                           Optional ByVal Data_modifica As Date = #2/1/1900#,
                           Optional ByVal username_creazione As String = "",
                           Optional ByVal username_modifica As String = ""
                           ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.Ricette_Dett_Tecnico_W.Scrivi()"

        Dim messaggioErrore As String = ""
        Dim strSql As New Text.StringBuilder
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

            strSql.Append(" INSERT INTO Ricette_Dettaglio_Tecnico ")
            strSql.Append("         ( ")
            strSql.Append("          Ricetta_SuperUser,          Ricetta_Cod,            Ricetta_Operazione_Cod,   ")
            strSql.Append("          Ricetta_Dettaglio_Cod,      Miscela_Cod,            Ricetta_Tecnico_Cod,    ")
            strSql.Append("          Qta_Ril,                    Av_Cod,                 Av_Gru,             Dett_Cod,   ")
            strSql.Append("          Dose,                       Parziale,               Nitrati,            Freatimetro,   ")
            strSql.Append("          Inn1_Data,                  Inn2_Data,              Inn3_Data,          Inn4_Data, ")
            strSql.Append("          Ditta_Cod,                  Sigla_Av,               Trap_Num,           ID_Insetto,  FF_Classe,  DataLock , ")

            strSql.Append("          Inviato,            DataInvio, ")
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
            strSql.Append("         , " & Agro_SQL_SaveNum(Ricetta_Tecnico_Cod) & "  ")
            strSql.Append("         , " & Agro_SQL_SaveNum(Qta_Ril) & "  ")
            strSql.Append("         , " & Agro_SQL_SaveNum(Av_Cod) & "  ")
            strSql.Append("         , " & Agro_SQL_SaveNum(Av_Gru) & "  ")
            strSql.Append("         , " & Agro_SQL_SaveNum(Dett_Cod) & "  ")
            strSql.Append("         , " & Agro_SQL_SaveNum(Dose) & "  ")
            strSql.Append("         , " & Agro_SQL_SaveNum(Parziale) & "  ")
            strSql.Append("         , " & Agro_SQL_SaveNum(Nitrati) & "  ")
            strSql.Append("         , " & Agro_SQL_SaveNum(Freatimetro) & "  ")

            strSql.Append("         , " & If(Inn1_Data = New Date, "Null", Agro_SQL_SaveDate(Inn1_Data)) & "  ")
            strSql.Append("         , " & If(Inn2_Data = New Date, "Null", Agro_SQL_SaveDate(Inn2_Data)) & "  ")
            strSql.Append("         , " & If(Inn3_Data = New Date, "Null", Agro_SQL_SaveDate(Inn3_Data)) & "  ")
            strSql.Append("         , " & If(Inn4_Data = New Date, "Null", Agro_SQL_SaveDate(Inn4_Data)) & "  ")
            strSql.Append("         , " & Agro_SQL_SaveNum(Ditta_Cod) & "  ")
            strSql.Append("         ,'" & Agro_SQL_SaveText(Sigla_AV) & "'  ")
            strSql.Append("         , " & Agro_SQL_SaveNum(Trap_Num) & "  ")
            strSql.Append("         , " & Agro_SQL_SaveNum(Id_Insetto) & "  ")
            strSql.Append("         , " & Agro_SQL_SaveNum(FF_Classe) & "  ")
            strSql.Append("         , 0  ")

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

    ' Overload con 7 campi in più: Mg, N, P, K, ApportoxHa, NnettoxHa, NutilexHa
    '##############################################################################################
    Public Function Scrivi(ByVal Ricetta_Cod As Int32,
                           ByVal Ricetta_Operazione_Cod As Int32,
                           ByVal Ricetta_Dettaglio_Cod As Int32,
                           ByVal Miscela_Cod As Int32,
                           ByVal Ricetta_Tecnico_Cod As Int32,
                           ByVal Qta_Ril As Decimal,
                           ByVal Av_Cod As Int32,
                           ByVal Av_Gru As Int32,
                           ByVal Dett_Cod As Int32,
                           ByVal Dose As Decimal,
                           ByVal Parziale As Integer,
                           ByVal Nitrati As Integer,
                           ByVal Freatimetro As Decimal,
                           ByVal Inn1_Data As Date,
                           ByVal Inn2_Data As Date,
                           ByVal Inn3_Data As Date,
                           ByVal Inn4_Data As Date,
                           ByVal Ditta_Cod As Long,
                           ByVal Sigla_AV As String,
                           ByVal Trap_Num As Int32,
                           ByVal Id_Insetto As Int32,
                           ByVal FF_Classe As Int32,
                           ByVal Mg As Decimal,
                           ByVal N As Decimal,
                           ByVal P As Decimal,
                           ByVal K As Decimal,
                           ByVal ApportoxHa As Decimal,
                           ByVal NnettoxHa As Decimal,
                           ByVal NutilexHa As Decimal,
                           ByVal Validita_Inizio As Date,
                           ByVal Validita_Fine As Date,
                           ByRef objParametri As AgronicaCoreParametri
                           ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.Ricette_Dett_Tecnico_W.Scrivi()"

        Dim messaggioErrore As String = ""
        Dim strSql As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            strSql.Length = 0

            strSql.Append(" INSERT INTO Ricette_Dettaglio_Tecnico ")
            strSql.Append("         ( ")
            strSql.Append("          Ricetta_SuperUser,          Ricetta_Cod,            Ricetta_Operazione_Cod,   ")
            strSql.Append("          Ricetta_Dettaglio_Cod,      Miscela_Cod,            Ricetta_Tecnico_Cod,    ")
            strSql.Append("          Qta_Ril,                    Av_Cod,                 Av_Gru,             Dett_Cod,   ")
            strSql.Append("          Dose,                       Parziale,               Nitrati,            Freatimetro,   ")
            strSql.Append("          Inn1_Data,                  Inn2_Data,              Inn3_Data,          Inn4_Data, ")
            strSql.Append("          Ditta_Cod,                  Sigla_Av,               Trap_Num,           ID_Insetto,  FF_Classe,  DataLock , ")
            strSql.Append("          Mg,                         N,                      P,                  K, ")
            strSql.Append("          ApportoxHa,                 NnettoxHa,              NutilexHa, ")

            strSql.Append("          Inviato,            DataInvio, ")
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
            strSql.Append("         , " & Agro_SQL_SaveNum(Ricetta_Tecnico_Cod) & "  ")
            strSql.Append("         , " & Agro_SQL_SaveNum(Qta_Ril) & "  ")
            strSql.Append("         , " & Agro_SQL_SaveNum(Av_Cod) & "  ")
            strSql.Append("         , " & Agro_SQL_SaveNum(Av_Gru) & "  ")
            strSql.Append("         , " & Agro_SQL_SaveNum(Dett_Cod) & "  ")
            strSql.Append("         , " & Agro_SQL_SaveNum(Dose) & "  ")
            strSql.Append("         , " & Agro_SQL_SaveNum(Parziale) & "  ")
            strSql.Append("         , " & Agro_SQL_SaveNum(Nitrati) & "  ")
            strSql.Append("         , " & Agro_SQL_SaveNum(Freatimetro) & "  ")

            strSql.Append("         , " & If(Inn1_Data = New Date, "Null", Agro_SQL_SaveDate(Inn1_Data)) & "  ")
            strSql.Append("         , " & If(Inn2_Data = New Date, "Null", Agro_SQL_SaveDate(Inn2_Data)) & "  ")
            strSql.Append("         , " & If(Inn3_Data = New Date, "Null", Agro_SQL_SaveDate(Inn3_Data)) & "  ")
            strSql.Append("         , " & If(Inn4_Data = New Date, "Null", Agro_SQL_SaveDate(Inn4_Data)) & "  ")
            strSql.Append("         , " & Agro_SQL_SaveNum(Ditta_Cod) & "  ")
            strSql.Append("         ,'" & Agro_SQL_SaveText(Sigla_AV) & "'  ")
            strSql.Append("         , " & Agro_SQL_SaveNum(Trap_Num) & "  ")
            strSql.Append("         , " & Agro_SQL_SaveNum(Id_Insetto) & "  ")
            strSql.Append("         , " & Agro_SQL_SaveNum(FF_Classe) & "  ")
            strSql.Append("         , 0  ")
            strSql.Append("         , " & Agro_SQL_SaveNum(Mg) & "  ")
            strSql.Append("         , " & Agro_SQL_SaveNum(N) & "  ")
            strSql.Append("         , " & Agro_SQL_SaveNum(P) & "  ")
            strSql.Append("         , " & Agro_SQL_SaveNum(K) & "  ")
            strSql.Append("         , " & Agro_SQL_SaveNum(ApportoxHa) & "  ")
            strSql.Append("         , " & Agro_SQL_SaveNum(NnettoxHa) & "  ")
            strSql.Append("         , " & Agro_SQL_SaveNum(NutilexHa) & "  ")

            strSql.Append("         , 0  ")
            strSql.Append("         , Null  ")
            strSql.Append("         , " & Agro_SQL_SaveDateTime(DateTime.Now) & "  ")
            strSql.Append("         , " & Agro_SQL_SaveDateTime(DateTime.Now) & "  ")
            strSql.Append("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            strSql.Append("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
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

    ' Overload con 4 campi in più: soglia_cod, soglia_des, soglia_quantita, efficienza
    ' (21/02/2018 fede) aggiunto Cu
    '##############################################################################################
    Public Function Scrivi(ByVal Ricetta_Cod As Int32,
                           ByVal Ricetta_Operazione_Cod As Int32,
                           ByVal Ricetta_Dettaglio_Cod As Int32,
                           ByVal Miscela_Cod As Int32,
                           ByVal Ricetta_Tecnico_Cod As Int32,
                           ByVal Qta_Ril As Decimal,
                           ByVal Av_Cod As Int32,
                           ByVal Av_Gru As Int32,
                           ByVal Dett_Cod As Int32,
                           ByVal Dose As Decimal,
                           ByVal Parziale As Integer,
                           ByVal Nitrati As Integer,
                           ByVal Freatimetro As Decimal,
                           ByVal Inn1_Data As Date,
                           ByVal Inn2_Data As Date,
                           ByVal Inn3_Data As Date,
                           ByVal Inn4_Data As Date,
                           ByVal Ditta_Cod As Long,
                           ByVal Sigla_AV As String,
                           ByVal Trap_Num As Int32,
                           ByVal Id_Insetto As Int32,
                           ByVal FF_Classe As Int32,
                           ByVal Mg As Decimal,
                           ByVal N As Decimal,
                           ByVal P As Decimal,
                           ByVal K As Decimal,
                           ByVal ApportoxHa As Decimal,
                           ByVal NnettoxHa As Decimal,
                           ByVal NutilexHa As Decimal,
                           ByVal Soglia_Cod As Integer,
                           ByVal Soglia_Des As String,
                           ByVal Soglia_Quantita As Integer,
                           ByVal Efficienza As Decimal,
                           ByVal Ricette_Dettaglio_Tecnico_graphickey As String,
                           ByVal Piezo1 As Decimal,
                           ByVal Piezo2 As Decimal,
                           ByVal Piezo3 As Decimal,
                           ByVal Piezo4 As Decimal,
                           ByVal Cu As Decimal,
                           ByVal Validita_Inizio As Date,
                           ByVal Validita_Fine As Date,
                           ByRef objParametri As AgronicaCoreParametri,
                           Optional ByVal Data_creazione As Date = #2/1/1900#,
                           Optional ByVal Data_modifica As Date = #2/1/1900#,
                           Optional ByVal username_creazione As String = "",
                           Optional ByVal username_modifica As String = ""
                           ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.Ricette_Dett_Tecnico_W.Scrivi()"

        Dim messaggioErrore As String = ""
        Dim strSql As New Text.StringBuilder
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

            strSql.Append(" INSERT INTO Ricette_Dettaglio_Tecnico ")
            strSql.Append("         ( ")
            strSql.Append("          Ricetta_SuperUser,          Ricetta_Cod,            Ricetta_Operazione_Cod,   ")
            strSql.Append("          Ricetta_Dettaglio_Cod,      Miscela_Cod,            Ricetta_Tecnico_Cod,    ")
            strSql.Append("          Qta_Ril,                    Av_Cod,                 Av_Gru,             Dett_Cod,   ")
            strSql.Append("          Dose,                       Parziale,               Nitrati,            Freatimetro,   ")
            strSql.Append("          Inn1_Data,                  Inn2_Data,              Inn3_Data,          Inn4_Data, ")
            strSql.Append("          Ditta_Cod,                  Sigla_Av,               Trap_Num,           ID_Insetto,  FF_Classe,  DataLock , ")
            strSql.Append("          Mg,                         N,                      P,                  K, ")
            strSql.Append("          ApportoxHa,                 NnettoxHa,              NutilexHa, ")
            strSql.Append("          Soglia_Cod,                 Soglia_Des,             Soglia_Quantita,    Efficienza, ")
            strSql.Append("          Ricette_Dettaglio_Tecnico_graphickey,                 Piezo1,             Piezo2,    Piezo3, Piezo4, Cu, ")

            strSql.Append("          Inviato,            DataInvio, ")
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
            strSql.Append("         , " & Agro_SQL_SaveNum(Ricetta_Tecnico_Cod) & "  ")
            strSql.Append("         , " & Agro_SQL_SaveNum(Qta_Ril) & "  ")
            strSql.Append("         , " & Agro_SQL_SaveNum(Av_Cod) & "  ")
            strSql.Append("         , " & Agro_SQL_SaveNum(Av_Gru) & "  ")
            strSql.Append("         , " & Agro_SQL_SaveNum(Dett_Cod) & "  ")
            strSql.Append("         , " & Agro_SQL_SaveNum(Dose) & "  ")
            strSql.Append("         , " & Agro_SQL_SaveNum(Parziale) & "  ")
            strSql.Append("         , " & Agro_SQL_SaveNum(Nitrati) & "  ")
            strSql.Append("         , " & Agro_SQL_SaveNum(Freatimetro) & "  ")

            strSql.Append("         , " & If(Inn1_Data = New Date, "Null", Agro_SQL_SaveDate(Inn1_Data)) & "  ")
            strSql.Append("         , " & If(Inn2_Data = New Date, "Null", Agro_SQL_SaveDate(Inn2_Data)) & "  ")
            strSql.Append("         , " & If(Inn3_Data = New Date, "Null", Agro_SQL_SaveDate(Inn3_Data)) & "  ")
            strSql.Append("         , " & If(Inn4_Data = New Date, "Null", Agro_SQL_SaveDate(Inn4_Data)) & "  ")
            strSql.Append("         , " & Agro_SQL_SaveNum(Ditta_Cod) & "  ")
            strSql.Append("         ,'" & Agro_SQL_SaveText(Sigla_AV) & "'  ")
            strSql.Append("         , " & Agro_SQL_SaveNum(Trap_Num) & "  ")
            strSql.Append("         , " & Agro_SQL_SaveNum(Id_Insetto) & "  ")
            strSql.Append("         , " & Agro_SQL_SaveNum(FF_Classe) & "  ")
            strSql.Append("         , 0  ")
            strSql.Append("         , " & Agro_SQL_SaveNum(Mg) & "  ")
            strSql.Append("         , " & Agro_SQL_SaveNum(N) & "  ")
            strSql.Append("         , " & Agro_SQL_SaveNum(P) & "  ")
            strSql.Append("         , " & Agro_SQL_SaveNum(K) & "  ")
            strSql.Append("         , " & Agro_SQL_SaveNum(ApportoxHa) & "  ")
            strSql.Append("         , " & Agro_SQL_SaveNum(NnettoxHa) & "  ")
            strSql.Append("         , " & Agro_SQL_SaveNum(NutilexHa) & "  ")
            strSql.Append("         , " & Agro_SQL_SaveNum(Soglia_Cod) & "  ")
            strSql.Append("         ,'" & Agro_SQL_SaveText(Soglia_Des) & "'  ")
            strSql.Append("         , " & Agro_SQL_SaveNum(Soglia_Quantita) & "  ")
            strSql.Append("         , " & Agro_SQL_SaveNum(Efficienza) & "  ")

            strSql.Append("         , '" & Agro_SQL_SaveText(Ricette_Dettaglio_Tecnico_graphickey) & "'  ")
            strSql.Append("         , " & Agro_SQL_SaveNum(Piezo1) & "  ")
            strSql.Append("         , " & Agro_SQL_SaveNum(Piezo2) & "  ")
            strSql.Append("         , " & Agro_SQL_SaveNum(Piezo3) & "  ")
            strSql.Append("         , " & Agro_SQL_SaveNum(Piezo4) & "  ")
            strSql.Append("         , " & Agro_SQL_SaveNum(Cu) & "  ")

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
                             ByVal Ricetta_Tecnico_Cod As Int32,
                             ByVal Qta_Ril As Decimal,
                             ByVal Av_Cod As Int32,
                             ByVal Av_Gru As Int32,
                             ByVal Dett_Cod As Int32,
                             ByVal Dose As Decimal,
                             ByVal Parziale As Integer,
                             ByVal Nitrati As Integer,
                             ByVal Freatimetro As Decimal,
                             ByVal Inn1_Data As Date,
                             ByVal Inn2_Data As Date,
                             ByVal Inn3_Data As Date,
                             ByVal Inn4_Data As Date,
                             ByVal Ditta_Cod As Int32,
                             ByVal Sigla_AV As String,
                             ByVal Trap_Num As Int32,
                             ByVal Id_Insetto As Int32,
                             ByVal FF_Classe As Int32,
                             ByVal Validita_Inizio As Date,
                             ByVal Validita_Fine As Date,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreParametri
                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.Ricette_Dett_Tecnico_W.Modifica()"

        '====================================================================================
        'Parametri opzionali :
        '   Ricetta_Cod = 0
        '   Ricetta_Operazione_Cod = 0
        '   Ricetta_Dettaglio_Cod = 0
        '   Miscela_Cod = 0
        '   Ricetta_Tecnico_Cod = 0
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If objParametri.PivaSuperUser = "" Then
                Throw New Exception("Parametro non corretto nella query (Ricetta_SuperUser obbligatorio)")
            End If

            '---------------------------------------------

            strSql.Length = 0

            strSql.Append(" UPDATE Ricette_Dettaglio_Tecnico SET ")
            strSql.Append("    Qta_Ril        =  " & Agro_SQL_SaveNum(Qta_Ril) & "  ")
            strSql.Append("   ,Av_Cod         =  " & Agro_SQL_SaveNum(Av_Cod) & "  ")
            strSql.Append("   ,Av_Gru         =  " & Agro_SQL_SaveNum(Av_Gru) & "  ")
            strSql.Append("   ,Dett_Cod       =  " & Agro_SQL_SaveNum(Dett_Cod) & "  ")
            strSql.Append("   ,Dose           = " & Agro_SQL_SaveNum(Dose) & "  ")
            strSql.Append("   ,Parziale       = " & Agro_SQL_SaveNum(Parziale) & "  ")
            strSql.Append("   ,Nitrati        = " & Agro_SQL_SaveNum(Nitrati) & "  ")
            strSql.Append("   ,Freatimetro    = " & Agro_SQL_SaveNum(Freatimetro) & "  ")
            strSql.Append("   ,Inn1_Data      =  " & Agro_SQL_SaveDate(Inn1_Data) & "  ")
            strSql.Append("   ,Inn2_Data      =  " & Agro_SQL_SaveDate(Inn2_Data) & "  ")
            strSql.Append("   ,Inn3_Data      =  " & Agro_SQL_SaveDate(Inn3_Data) & "  ")
            strSql.Append("   ,Inn4_Data      =  " & Agro_SQL_SaveDate(Inn4_Data) & "  ")

            strSql.Append("   ,Ditta_Cod      = " & Agro_SQL_SaveNum(Ditta_Cod) & "  ")
            strSql.Append("   ,Sigla_Av       = '" & Agro_SQL_SaveText(Sigla_AV) & "'  ")
            strSql.Append("   ,Trap_Num       = " & Agro_SQL_SaveNum(Trap_Num) & "  ")
            strSql.Append("   ,Id_Insetto     = " & Agro_SQL_SaveNum(Id_Insetto) & "  ")
            strSql.Append("   ,FF_Classe      = " & Agro_SQL_SaveNum(FF_Classe) & "  ")

            strSql.Append("   ,Inviato           =  0 ")
            strSql.Append("   ,DataInvio         =  Null ")
            strSql.Append("   ,Data_Modifica     =  " & Agro_SQL_SaveDateTime(DateTime.Now))
            strSql.Append("   ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            strSql.Append("   ,Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Inizio))
            strSql.Append("   ,Validita_Fine     =  " & Agro_SQL_SaveDate(Validita_Fine))


            strSql.Append(" WHERE  Ricetta_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

            If Ricetta_Cod <> 0 Then
                strSql.Append("   AND Ricette_Dettaglio_Tecnico.Ricetta_Cod = " & Agro_SQL_SaveNum(Ricetta_Cod) & "   ")
            End If

            If Ricetta_Operazione_Cod <> 0 Then
                strSql.Append("   AND Ricette_Dettaglio_Tecnico.Ricetta_operazione_Cod = " & Agro_SQL_SaveNum(Ricetta_Operazione_Cod) & "   ")
            End If

            If Ricetta_Dettaglio_Cod <> 0 Then
                strSql.Append("   AND Ricette_Dettaglio_Tecnico.Ricetta_Dettaglio_Cod = " & Agro_SQL_SaveNum(Ricetta_Dettaglio_Cod) & "   ")
            End If

            If Miscela_Cod <> 0 Then
                strSql.Append("   AND Ricette_Dettaglio_Tecnico.miscela_Cod = " & Agro_SQL_SaveNum(Miscela_Cod) & "   ")
            End If

            If Ricetta_Tecnico_Cod <> 0 Then
                strSql.Append("   AND Ricette_Dettaglio_Tecnico.Ricetta_Tecnico_Cod = " & Agro_SQL_SaveNum(Ricetta_Tecnico_Cod) & "   ")
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


    ' Overload con 7 campi in più: Mg, N, P, K, ApportoxHa, NnettoxHa, NutilexHa
    '##############################################################################################
    Public Function Modifica(ByVal Ricetta_Cod As Int32,
                             ByVal Ricetta_Operazione_Cod As Int32,
                             ByVal Ricetta_Dettaglio_Cod As Int32,
                             ByVal Miscela_Cod As Int32,
                             ByVal Ricetta_Tecnico_Cod As Int32,
                             ByVal Qta_Ril As Decimal,
                             ByVal Av_Cod As Int32,
                             ByVal Av_Gru As Int32,
                             ByVal Dett_Cod As Int32,
                             ByVal Dose As Decimal,
                             ByVal Parziale As Integer,
                             ByVal Nitrati As Integer,
                             ByVal Freatimetro As Decimal,
                             ByVal Inn1_Data As Date,
                             ByVal Inn2_Data As Date,
                             ByVal Inn3_Data As Date,
                             ByVal Inn4_Data As Date,
                             ByVal Ditta_Cod As Int32,
                             ByVal Sigla_AV As String,
                             ByVal Trap_Num As Int32,
                             ByVal Id_Insetto As Int32,
                             ByVal FF_Classe As Int32,
                             ByVal Mg As Decimal,
                             ByVal N As Decimal,
                             ByVal P As Decimal,
                             ByVal K As Decimal,
                             ByVal ApportoxHa As Decimal,
                             ByVal NnettoxHa As Decimal,
                             ByVal NutilexHa As Decimal,
                             ByVal Validita_Inizio As Date,
                             ByVal Validita_Fine As Date,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreParametri
                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.Ricette_Dett_Tecnico_W.Modifica()"

        '====================================================================================
        'Parametri opzionali :
        '   Ricetta_Cod = 0
        '   Ricetta_Operazione_Cod = 0
        '   Ricetta_Dettaglio_Cod = 0
        '   Miscela_Cod = 0
        '   Ricetta_Tecnico_Cod = 0
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If objParametri.PivaSuperUser = "" Then
                Throw New Exception("Parametro non corretto nella query (Ricetta_SuperUser obbligatorio)")
            End If

            '---------------------------------------------

            strSql.Length = 0

            strSql.Append(" UPDATE Ricette_Dettaglio_Tecnico SET ")
            strSql.Append("    Qta_Ril        =  " & Agro_SQL_SaveNum(Qta_Ril) & "  ")
            strSql.Append("   ,Av_Cod         =  " & Agro_SQL_SaveNum(Av_Cod) & "  ")
            strSql.Append("   ,Av_Gru         =  " & Agro_SQL_SaveNum(Av_Gru) & "  ")
            strSql.Append("   ,Dett_Cod       =  " & Agro_SQL_SaveNum(Dett_Cod) & "  ")
            strSql.Append("   ,Dose           = " & Agro_SQL_SaveNum(Dose) & "  ")
            strSql.Append("   ,Parziale       = " & Agro_SQL_SaveNum(Parziale) & "  ")
            strSql.Append("   ,Nitrati        = " & Agro_SQL_SaveNum(Nitrati) & "  ")
            strSql.Append("   ,Freatimetro    = " & Agro_SQL_SaveNum(Freatimetro) & "  ")
            strSql.Append("   ,Inn1_Data      =  " & Agro_SQL_SaveDate(Inn1_Data) & "  ")
            strSql.Append("   ,Inn2_Data      =  " & Agro_SQL_SaveDate(Inn2_Data) & "  ")
            strSql.Append("   ,Inn3_Data      =  " & Agro_SQL_SaveDate(Inn3_Data) & "  ")
            strSql.Append("   ,Inn4_Data      =  " & Agro_SQL_SaveDate(Inn4_Data) & "  ")

            strSql.Append("   ,Ditta_Cod      = " & Agro_SQL_SaveNum(Ditta_Cod) & "  ")
            strSql.Append("   ,Sigla_Av       = '" & Agro_SQL_SaveText(Sigla_AV) & "'  ")
            strSql.Append("   ,Trap_Num       = " & Agro_SQL_SaveNum(Trap_Num) & "  ")
            strSql.Append("   ,Id_Insetto     = " & Agro_SQL_SaveNum(Id_Insetto) & "  ")
            strSql.Append("   ,FF_Classe      = " & Agro_SQL_SaveNum(FF_Classe) & "  ")

            strSql.Append("   ,Mg             = " & Agro_SQL_SaveNum(Mg) & "  ")
            strSql.Append("   ,N              = " & Agro_SQL_SaveNum(N) & "  ")
            strSql.Append("   ,P              = " & Agro_SQL_SaveNum(P) & "  ")
            strSql.Append("   ,K              = " & Agro_SQL_SaveNum(K) & "  ")
            strSql.Append("   ,ApportoxHa     = " & Agro_SQL_SaveNum(ApportoxHa) & "  ")
            strSql.Append("   ,NnettoxHa      = " & Agro_SQL_SaveNum(NnettoxHa) & "  ")
            strSql.Append("   ,NutilexHa      = " & Agro_SQL_SaveNum(NutilexHa) & "  ")

            strSql.Append("   ,Inviato           =  0 ")
            strSql.Append("   ,DataInvio         =  Null ")
            strSql.Append("   ,Data_Modifica     =  " & Agro_SQL_SaveDateTime(DateTime.Now))
            strSql.Append("   ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            strSql.Append("   ,Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Inizio))
            strSql.Append("   ,Validita_Fine     =  " & Agro_SQL_SaveDate(Validita_Fine))

            strSql.Append(" WHERE  Ricetta_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

            If Ricetta_Cod <> 0 Then
                strSql.Append("   AND Ricette_Dettaglio_Tecnico.Ricetta_Cod = " & Agro_SQL_SaveNum(Ricetta_Cod) & "   ")
            End If

            If Ricetta_Operazione_Cod <> 0 Then
                strSql.Append("   AND Ricette_Dettaglio_Tecnico.Ricetta_operazione_Cod = " & Agro_SQL_SaveNum(Ricetta_Operazione_Cod) & "   ")
            End If

            If Ricetta_Dettaglio_Cod <> 0 Then
                strSql.Append("   AND Ricette_Dettaglio_Tecnico.Ricetta_Dettaglio_Cod = " & Agro_SQL_SaveNum(Ricetta_Dettaglio_Cod) & "   ")
            End If

            If Miscela_Cod <> 0 Then
                strSql.Append("   AND Ricette_Dettaglio_Tecnico.miscela_Cod = " & Agro_SQL_SaveNum(Miscela_Cod) & "   ")
            End If

            If Ricetta_Tecnico_Cod <> 0 Then
                strSql.Append("   AND Ricette_Dettaglio_Tecnico.Ricetta_Tecnico_Cod = " & Agro_SQL_SaveNum(Ricetta_Tecnico_Cod) & "   ")
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


    ' Overload con 4 campi in più: soglia_cod, soglia_des, soglia_quantita, efficienza
    '##############################################################################################
    Public Function Modifica(ByVal Ricetta_Cod As Int32,
                             ByVal Ricetta_Operazione_Cod As Int32,
                             ByVal Ricetta_Dettaglio_Cod As Int32,
                             ByVal Miscela_Cod As Int32,
                             ByVal Ricetta_Tecnico_Cod As Int32,
                             ByVal Qta_Ril As Decimal,
                             ByVal Av_Cod As Int32,
                             ByVal Av_Gru As Int32,
                             ByVal Dett_Cod As Int32,
                             ByVal Dose As Decimal,
                             ByVal Parziale As Integer,
                             ByVal Nitrati As Integer,
                             ByVal Freatimetro As Decimal,
                             ByVal Inn1_Data As Date,
                             ByVal Inn2_Data As Date,
                             ByVal Inn3_Data As Date,
                             ByVal Inn4_Data As Date,
                             ByVal Ditta_Cod As Int32,
                             ByVal Sigla_AV As String,
                             ByVal Trap_Num As Int32,
                             ByVal Id_Insetto As Int32,
                             ByVal FF_Classe As Int32,
                             ByVal Mg As Decimal,
                             ByVal N As Decimal,
                             ByVal P As Decimal,
                             ByVal K As Decimal,
                             ByVal ApportoxHa As Decimal,
                             ByVal NnettoxHa As Decimal,
                             ByVal NutilexHa As Decimal,
                             ByVal Soglia_Cod As Integer,
                             ByVal Soglia_Des As String,
                             ByVal Soglia_Quantita As Integer,
                             ByVal Efficienza As Decimal,
                             ByVal Cu As Decimal,
                             ByVal Validita_Inizio As Date,
                             ByVal Validita_Fine As Date,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreParametri
                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.Ricette_Dett_Tecnico_W.Modifica()"

        '====================================================================================
        'Parametri opzionali :
        '   Ricetta_Cod = 0
        '   Ricetta_Operazione_Cod = 0
        '   Ricetta_Dettaglio_Cod = 0
        '   Miscela_Cod = 0
        '   Ricetta_Tecnico_Cod = 0
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If objParametri.PivaSuperUser = "" Then
                Throw New Exception("Parametro non corretto nella query (Ricetta_SuperUser obbligatorio)")
            End If

            '---------------------------------------------

            strSql.Length = 0

            strSql.Append(" UPDATE Ricette_Dettaglio_Tecnico SET ")
            strSql.Append("    Qta_Ril        =  " & Agro_SQL_SaveNum(Qta_Ril) & "  ")
            strSql.Append("   ,Av_Cod         =  " & Agro_SQL_SaveNum(Av_Cod) & "  ")
            strSql.Append("   ,Av_Gru         =  " & Agro_SQL_SaveNum(Av_Gru) & "  ")
            strSql.Append("   ,Dett_Cod       =  " & Agro_SQL_SaveNum(Dett_Cod) & "  ")
            strSql.Append("   ,Dose           = " & Agro_SQL_SaveNum(Dose) & "  ")
            strSql.Append("   ,Parziale       = " & Agro_SQL_SaveNum(Parziale) & "  ")
            strSql.Append("   ,Nitrati        = " & Agro_SQL_SaveNum(Nitrati) & "  ")
            strSql.Append("   ,Freatimetro    = " & Agro_SQL_SaveNum(Freatimetro) & "  ")
            strSql.Append("   ,Inn1_Data      =  " & Agro_SQL_SaveDate(Inn1_Data) & "  ")
            strSql.Append("   ,Inn2_Data      =  " & Agro_SQL_SaveDate(Inn2_Data) & "  ")
            strSql.Append("   ,Inn3_Data      =  " & Agro_SQL_SaveDate(Inn3_Data) & "  ")
            strSql.Append("   ,Inn4_Data      =  " & Agro_SQL_SaveDate(Inn4_Data) & "  ")

            strSql.Append("   ,Ditta_Cod      = " & Agro_SQL_SaveNum(Ditta_Cod) & "  ")
            strSql.Append("   ,Sigla_Av       = '" & Agro_SQL_SaveText(Sigla_AV) & "'  ")
            strSql.Append("   ,Trap_Num       = " & Agro_SQL_SaveNum(Trap_Num) & "  ")
            strSql.Append("   ,Id_Insetto     = " & Agro_SQL_SaveNum(Id_Insetto) & "  ")
            strSql.Append("   ,FF_Classe      = " & Agro_SQL_SaveNum(FF_Classe) & "  ")

            strSql.Append("   ,Mg             = " & Agro_SQL_SaveNum(Mg) & "  ")
            strSql.Append("   ,N              = " & Agro_SQL_SaveNum(N) & "  ")
            strSql.Append("   ,P              = " & Agro_SQL_SaveNum(P) & "  ")
            strSql.Append("   ,K              = " & Agro_SQL_SaveNum(K) & "  ")
            strSql.Append("   ,ApportoxHa     = " & Agro_SQL_SaveNum(ApportoxHa) & "  ")
            strSql.Append("   ,NnettoxHa      = " & Agro_SQL_SaveNum(NnettoxHa) & "  ")
            strSql.Append("   ,NutilexHa      = " & Agro_SQL_SaveNum(NutilexHa) & "  ")
            strSql.Append("   ,Soglia_Cod      = " & Agro_SQL_SaveNum(Soglia_Cod) & "  ")
            strSql.Append("   ,Soglia_Des      = '" & Agro_SQL_SaveText(Soglia_Des) & "'  ")
            strSql.Append("   ,Soglia_Quantita      = " & Agro_SQL_SaveNum(Soglia_Quantita) & "  ")
            strSql.Append("   ,Efficienza      = " & Agro_SQL_SaveNum(Efficienza) & "  ")
            strSql.Append("   ,Cu              = " & Agro_SQL_SaveNum(Cu) & "  ")

            strSql.Append("   ,Inviato           =  0 ")
            strSql.Append("   ,DataInvio         =  Null ")
            strSql.Append("   ,Data_Modifica     =  " & Agro_SQL_SaveDateTime(DateTime.Now))
            strSql.Append("   ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            strSql.Append("   ,Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Inizio))
            strSql.Append("   ,Validita_Fine     =  " & Agro_SQL_SaveDate(Validita_Fine))

            strSql.Append(" WHERE  Ricetta_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

            If Ricetta_Cod <> 0 Then
                strSql.Append("   AND Ricette_Dettaglio_Tecnico.Ricetta_Cod = " & Agro_SQL_SaveNum(Ricetta_Cod) & "   ")
            End If

            If Ricetta_Operazione_Cod <> 0 Then
                strSql.Append("   AND Ricette_Dettaglio_Tecnico.Ricetta_operazione_Cod = " & Agro_SQL_SaveNum(Ricetta_Operazione_Cod) & "   ")
            End If

            If Ricetta_Dettaglio_Cod <> 0 Then
                strSql.Append("   AND Ricette_Dettaglio_Tecnico.Ricetta_Dettaglio_Cod = " & Agro_SQL_SaveNum(Ricetta_Dettaglio_Cod) & "   ")
            End If

            If Miscela_Cod <> 0 Then
                strSql.Append("   AND Ricette_Dettaglio_Tecnico.miscela_Cod = " & Agro_SQL_SaveNum(Miscela_Cod) & "   ")
            End If

            If Ricetta_Tecnico_Cod <> 0 Then
                strSql.Append("   AND Ricette_Dettaglio_Tecnico.Ricetta_Tecnico_Cod = " & Agro_SQL_SaveNum(Ricetta_Tecnico_Cod) & "   ")
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


    '##############################################################################################
    'il filtro sulla miscela nel dettaglio tecnico è stato tolto, perché non viene considerata nelle ricette nuove
    Public Function Cancella(ByVal Ricetta_Cod As Int32,
                             ByVal Ricetta_Operazione_Cod As Int32,
                             ByVal Ricetta_Dettaglio_Cod As Int32,
                             ByVal Miscela_Cod_Non_Usato As Int32,
                             ByVal Ricetta_Tecnico_Cod As Int32,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreParametri
                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.Ricette_Dett_Tecnico_W.Cancella()"

        '====================================================================================
        'Parametri opzionali :
        '   Ricetta_Cod = 0
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            'If Piva = "" Then
            '    Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
            'End If

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then
                StrSQL.Length = 0
                StrSQL.Append(" UPDATE Ricette_Dettaglio_Tecnico ")
                StrSQL.Append(" SET ")
                StrSQL.Append("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("      ,Inviato = -1 ")
                StrSQL.Append(" WHERE  Inviato >= 0 ")
            Else
                StrSQL.Length = 0
                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM Ricette_Dettaglio_Tecnico ")
                StrSQL.Append(" WHERE  1=1 ")

            End If

            If objParametri.PivaSuperUser <> "" Then
                StrSQL.Append(" AND Ricette_Dettaglio_Tecnico.Ricetta_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'   ")
            End If

            If Ricetta_Cod <> 0 Then
                StrSQL.Append(" AND Ricette_Dettaglio_Tecnico.Ricetta_Cod = " & Agro_SQL_SaveNum(Ricetta_Cod) & "   ")
            End If

            If Ricetta_Operazione_Cod <> 0 Then
                StrSQL.Append(" AND Ricette_Dettaglio_Tecnico.Ricetta_operazione_Cod = " & Agro_SQL_SaveNum(Ricetta_Operazione_Cod) & "   ")
            End If

            If Ricetta_Dettaglio_Cod <> 0 Then
                StrSQL.Append(" AND Ricette_Dettaglio_Tecnico.Ricetta_Dettaglio_Cod = " & Agro_SQL_SaveNum(Ricetta_Dettaglio_Cod) & "   ")
            End If

            'il filtro sulla miscela nel dettaglio tecnico è stato tolto, perché non viene considerata nelle ricette nuove
            'If Miscela_Cod <> 0 Then
            '    StrSQL.Append(" AND Ricette_Dettaglio_Tecnico.miscela_Cod = " & Agro_SQL_SaveNum(Miscela_Cod) & "   ")
            'End If

            If Ricetta_Tecnico_Cod <> 0 Then
                StrSQL.Append(" AND Ricette_Dettaglio_Tecnico.Ricetta_Tecnico_Cod = " & Agro_SQL_SaveNum(Ricetta_Tecnico_Cod) & "   ")
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
