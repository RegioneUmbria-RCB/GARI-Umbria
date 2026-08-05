Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.DataProviderExtensions

Public Class Materie_PrimexReport_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi(ByVal Piva As String,
                          ByVal Pro_Cod As Integer,
                          ByVal Mat_Cod As Integer,
                          ByVal Id_Report As Integer,
                          ByVal xSelezioneVariabile As enumSelezioneVariabile,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreParametri
                          ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Materie_PrimexReport_R.Leggi()"

        Dim messaggioErrore As String = ""
        Dim strSql As New Text.StringBuilder
        Dim dt As DataTable

        Try

            '------------------------------------------------------------------
            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi


                Case enumSelezioneVariabile.Selezione_TabellaCompleta

                    strSql.Length = 0

                    strSql.Append(" SELECT Materie_PrimexReport.*, Agro_Reportistica.*  ")
                    strSql.Append(" FROM   Materie_PrimexReport, Agro_Reportistica ")
                    strSql.Append(" WHERE  Materie_PrimexReport.Piva = '" & Agro_SQL_SaveText(Trim(Piva)) & "' ")
                    strSql.Append(" AND    Materie_PrimexReport.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    strSql.Append(" AND    Materie_PrimexReport.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    strSql.Append(" AND    Materie_PrimexReport.Id_Report = Agro_Reportistica.Id_Report   ")

                    If Pro_Cod <> 0 Then
                        strSql.Append(" AND Materie_PrimexReport.Pro_Cod = " & Agro_SQL_SaveNum(Pro_Cod) & "   ")
                    End If

                    If Mat_Cod <> 0 Then
                        strSql.Append(" AND Materie_PrimexReport.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & "   ")
                    End If

                    If Id_Report <> 0 Then
                        strSql.Append(" AND Materie_PrimexReport.Id_Report = " & Agro_SQL_SaveNum(Id_Report) & "   ")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        strSql.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            strSql.Append(" AND   Materie_PrimexReport.Inviato >=0 ")
                            strSql.Append(" AND   Agro_Reportistica.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            strSql.Append(" AND   Materie_PrimexReport.Inviato =-1 ")
                            strSql.Append(" AND   Agro_Reportistica.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        strSql.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        strSql.Append(" ORDER BY Materie_PrimexReport.Piva, Materie_PrimexReport.Id_Report ASC ")
                    End If

                Case enumSelezioneVariabile.Selezione_JoinCompleta


                Case enumSelezioneVariabile.Selezione_JoinDescrizioni


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


    '#####################################################################
    'a differenza della leggi, legge anche la tabella materie_prime e agro_reportistica_tipi
    Public Function Leggi_2(ByVal Piva As String,
                            ByVal Pro_Cod As Integer,
                            ByVal Mat_Cod As Integer,
                            ByVal Id_Report As Integer,
                            ByVal Tipo As Integer,
                            ByVal xFiltroAggiuntivo As String,
                            ByVal xOrderBy As String,
                            ByRef objParametri As AgronicaCoreParametri
                            ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Materie_PrimexReport_R.Leggi_2()"

        Dim messaggioErrore As String = ""
        Dim strSql As New Text.StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0

            strSql.Append(" SELECT [Materie_PrimexReport].[Piva], [Materie_PrimexReport].[Pro_Cod], [Materie_PrimexReport].[Mat_Cod], [Materie_PrimexReport].[Id_Report],  ")
            strSql.Append("         [Agro_Reportistica].[Caption_Label], [Agro_Reportistica].[Tipo], [Agro_Reportistica_Tipi].[Tipo_Des], ")
            strSql.Append("         [Sa_Cod],[Elem_Cod],[Cod_Articolo],[Mat_Des],[Sem_Cod],[Cul_Cod],[Veg_Cod],[Ditta_Cod],[Note],[Regolamento]")
            strSql.Append("  ")
            strSql.Append(" FROM   Materie_PrimexReport ")
            strSql.Append(" INNER JOIN Materie_Prime ON materie_PrimexReport.Piva = Materie_Prime.Piva AND materie_PrimexReport.mat_cod = Materie_Prime.mat_cod ")
            strSql.Append(" INNER JOIN Agro_Reportistica ON materie_PrimexReport.Id_Report = Agro_Reportistica.Id_Report ")
            strSql.Append(" INNER JOIN Agro_Reportistica_Tipi ON materie_PrimexReport.Id_Report = Agro_Reportistica.Id_Report ")
            strSql.Append(" WHERE  Materie_PrimexReport.Piva = '" & Agro_SQL_SaveText(Trim(Piva)) & "' ")
            strSql.Append(" AND    Materie_PrimexReport.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            strSql.Append(" AND    Materie_PrimexReport.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")

            If Pro_Cod <> 0 Then
                strSql.Append(" AND Materie_PrimexReport.Pro_Cod = " & Agro_SQL_SaveNum(Pro_Cod) & "   ")
            End If

            If Mat_Cod <> 0 Then
                strSql.Append(" AND Materie_PrimexReport.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & "   ")
            End If

            If Id_Report <> 0 Then
                strSql.Append(" AND Materie_PrimexReport.Id_Report = " & Agro_SQL_SaveNum(Id_Report) & "   ")
            End If

            If Tipo <> 0 Then
                strSql.Append(" AND Agro_Reportistica.Tipo = " & Agro_SQL_SaveNum(Tipo) & "   ")
            End If

            If xFiltroAggiuntivo <> "" Then
                strSql.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.Append(" AND   Materie_Prime.Inviato >=0 ")
                    strSql.Append(" AND   Materie_PrimexReport.Inviato >=0 ")
                    strSql.Append(" AND   Agro_Reportistica.Inviato >=0 ")
                    strSql.Append(" AND   Agro_Reportistica_Tipi.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.Append(" AND   Materie_Prime.Inviato =-1 ")
                    strSql.Append(" AND   Materie_PrimexReport.Inviato =-1 ")
                    strSql.Append(" AND   Agro_Reportistica.Inviato =-1 ")
                    strSql.Append(" AND   Agro_Reportistica_Tipi.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                strSql.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                strSql.Append(" ORDER BY Mat_Des ")
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


'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§

Public Class Materie_PrimexReport_W
    Inherits AgronicaCoreDataProvider.DataProvider

    '============================================================================
    Public Function Scrivi(ByVal Piva As String,
                           ByVal Pro_Cod As Int32,
                           ByVal Mat_Cod As Int32,
                           ByVal Id_Report As Int32,
                           ByVal Validita_Inizio As Date,
                           ByVal Validita_Fine As Date,
                           ByRef objParametri As AgronicaCoreParametri
                           ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.Materie_PrimexReport_W.Scrivi()"

        Dim messaggioErrore As String = ""
        Dim strSql As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            strSql.Length = 0

            strSql.Append(" INSERT INTO Materie_PrimexReport ")
            strSql.Append("        ( Piva,    Pro_Cod,  Mat_Cod,   Id_Report,  ")
            strSql.Append("          Inviato,            DataInvio, ")
            strSql.Append("          Data_Creazione,     Data_Modifica, ")
            strSql.Append("          UserName_Creazione, UserName_Modifica, ")
            strSql.Append("          Validita_Inizio,    Validita_Fine ")
            strSql.Append("         ) ")

            strSql.Append(" VALUES ( ")
            strSql.Append("          '" & Agro_SQL_SaveText(Piva) & "'  ")
            strSql.Append("         , " & Agro_SQL_SaveNum(Pro_Cod) & "  ")
            strSql.Append("         , " & Agro_SQL_SaveNum(Mat_Cod) & "  ")
            strSql.Append("         , " & Agro_SQL_SaveNum(Id_Report) & "  ")

            strSql.Append("         , 0  ")
            strSql.Append("         , Null  ")
            strSql.Append("         , " & Agro_SQL_SaveDate(Date.Now) & "  ")
            strSql.Append("         , " & Agro_SQL_SaveDate(Date.Now) & "  ")
            strSql.Append("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            strSql.Append("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            strSql.Append("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            strSql.Append("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")

            strSql.Append("         ) ")

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

    '============================================================================
    Public Function Modifica(ByVal Piva As String,
                             ByVal Pro_Cod As Int32,
                             ByVal Mat_Cod As Int32,
                             ByVal Id_Report As Int32,
                             ByVal FinestraTemp_Inizio As Date,
                             ByVal FinestraTemp_Fine As Date,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreParametri
                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.Materie_PrimexReport_W.Modifica()"

        Dim messaggioErrore As String = ""
        Dim strSql As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If Piva = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
            End If

            '---------------------------------------------

            strSql.Length = 0

            strSql.Append(" UPDATE Materie_PrimexReport SET ")
            strSql.Append("    Id_Report    =  " & Agro_SQL_SaveNum(Id_Report) & "  ")

            strSql.Append("   ,Inviato           =  0 ")
            strSql.Append("   ,DataInvio         =  Null ")
            strSql.Append("   ,Data_Modifica     =  " & Agro_SQL_SaveDate(Date.Now))
            strSql.Append("   ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            strSql.Append("   ,Validita_Inizio   =  " & Agro_SQL_SaveDate(FinestraTemp_Inizio))
            strSql.Append("   ,Validita_Fine     =  " & Agro_SQL_SaveDate(FinestraTemp_Fine))

            strSql.Append(" WHERE   Piva      =  '" & Agro_SQL_SaveText(Trim(Piva)) & "'   ")

            If Pro_Cod <> 0 Then
                strSql.Append(" AND Pro_Cod = " & Agro_SQL_SaveNum(Pro_Cod) & "   ")
            End If

            If Mat_Cod <> 0 Then
                strSql.Append(" AND Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & "   ")
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

    '============================================================================
    Public Function Cancella(ByVal Piva As String,
                             ByVal Pro_Cod As Int32,
                             ByVal Mat_Cod As Int32,
                             ByVal Id_Report As Int32,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreParametri
                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.Materie_PrimexReport_W.Cancella()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""
        '   Pro_Cod = 0
        '   Mat_Cod = 0
        '   Id_Report = 0
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            'If Piva = "" Then
            '    Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
            'End If

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then

                strSql.Length = 0
                strSql.Append(" UPDATE Materie_PrimexReport ")
                strSql.Append(" SET ")
                strSql.Append("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                strSql.Append("      ,Inviato = -1 ")
                strSql.Append(" WHERE  Inviato >= 0 ")

            Else
                strSql.Length = 0
                strSql.Append(" DELETE ")
                strSql.Append(" FROM Materie_PrimexReport ")
                strSql.Append(" WHERE  1=1 ")

            End If


            If Piva <> "" Then
                strSql.Append(" AND  Piva = '" & Agro_SQL_SaveText(Trim(Piva)) & "'  ")
            End If

            If Pro_Cod <> 0 Then
                strSql.Append(" AND Pro_Cod = " & Agro_SQL_SaveNum(Pro_Cod) & "   ")
            End If

            If Mat_Cod <> 0 Then
                strSql.Append(" AND Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & "   ")
            End If

            If Id_Report <> 0 Then
                strSql.Append(" AND Id_Report = " & Agro_SQL_SaveNum(Id_Report) & "   ")
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

End Class
