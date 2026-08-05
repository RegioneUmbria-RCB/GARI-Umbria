Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri


Public Class FormulatixPrincipiAttivi_R
    Inherits AgronicaCoreDataProvider.DataProvider



    Public Function Leggi(ByVal FR_COD As Int32, _
                             ByVal PA_COD As Int32, _
                             ByVal Validita_Inizio As Date, _
                             ByVal Validita_Fine As Date, _
                                   ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile, _
                                   ByVal xFiltroAggiuntivo As String, _
                                   ByVal xOrderBy As String, _
                                   ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                             ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.FormulatixPrincipiAttivi_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0

            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta

                    StrSQL.Append(" SELECT *, FormulatixPrincipiAttivi.Stato AS StatoFP " & _
                                " FROM  FormulatixPrincipiAttivi , Formulati , PrincipiAttivi " & _
                                " WHERE FormulatixPrincipiAttivi.Validita_inizio < " & Agro_SQL_SaveDate(Validita_Fine) & " " & _
                                " AND   FormulatixPrincipiAttivi.Validita_Fine > " & Agro_SQL_SaveDate(Validita_Inizio) & " " & _
                                " AND   Formulati.Validita_inizio < " & Agro_SQL_SaveDate(Validita_Fine) & " " & _
                                " AND   Formulati.Validita_Fine > " & Agro_SQL_SaveDate(Validita_Inizio) & " " & _
                                " AND   PrincipiAttivi.Validita_inizio < " & Agro_SQL_SaveDate(Validita_Fine) & " " & _
                                " AND   PrincipiAttivi.Validita_Fine > " & Agro_SQL_SaveDate(Validita_Inizio) & " " & _
                                " AND   FormulatixPrincipiAttivi.FR_COD = Formulati.FR_COD " & _
                                " AND   FormulatixPrincipiAttivi.PA_COD = PrincipiAttivi.PA_COD ")


                    If FR_COD <> 0 Then
                        StrSQL.Append(" AND FormulatixPrincipiAttivi.FR_COD =  " & Agro_SQL_SaveNum(FR_COD) & "  ")
                    End If

                    If PA_COD <> 0 Then
                        StrSQL.Append(" AND FormulatixPrincipiAttivi.PA_COD =  " & Agro_SQL_SaveNum(PA_COD) & "  ")
                    End If


                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If

                    '--------------------------------------------------------------------------

                    If xOrderBy <> "" Then
                        strSql.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY Formulati.FR_DES ASC, PrincipiAttivi.PA_DES ASC ")
                    End If

                    '##############################################################

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni

                    StrSQL.Append(" SELECT FormulatixPrincipiAttivi.FR_COD, FormulatixPrincipiAttivi.PA_COD, FormulatixPrincipiAttivi.Titolo, FormulatixPrincipiAttivi.Stato AS StatoFP, " + vbCrLf)
                    StrSQL.Append(" Fr_Des, FormulatixPrincipiAttivi.Pa_Cod, PrincipiAttivi.Pa_Des " + vbCrLf)
                    StrSQL.Append(" FROM   FormulatixPrincipiAttivi  ")
                    StrSQL.Append(" INNER JOIN  PrincipiAttivi ON FormulatixPrincipiAttivi.Pa_Cod = PrincipiAttivi.Pa_Cod " + vbCrLf)
                    StrSQL.Append(" INNER JOIN  Formulati ON FormulatixPrincipiAttivi.FR_COD = Formulati.FR_COD  " + vbCrLf)
                    StrSQL.Append(" WHERE FormulatixPrincipiAttivi.Validita_inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & " " + vbCrLf)
                    StrSQL.Append(" AND   FormulatixPrincipiAttivi.Validita_Fine >= " & Agro_SQL_SaveDate(Validita_Inizio) & " " + vbCrLf)
                    StrSQL.Append(" AND   Formulati.Validita_inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & " " + vbCrLf)
                    StrSQL.Append(" AND   Formulati.Validita_Fine >= " & Agro_SQL_SaveDate(Validita_Inizio) & " " + vbCrLf)
                    StrSQL.Append(" AND   PrincipiAttivi.Validita_inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & " " + vbCrLf)
                    StrSQL.Append(" AND   PrincipiAttivi.Validita_Fine >= " & Agro_SQL_SaveDate(Validita_Inizio) & " " + vbCrLf)

                    If FR_COD <> 0 Then
                        StrSQL.Append(" AND FormulatixPrincipiAttivi.FR_COD =  " & Agro_SQL_SaveNum(FR_COD) & "  ")
                    End If

                    If PA_COD <> 0 Then
                        StrSQL.Append(" AND FormulatixPrincipiAttivi.PA_COD =  " & Agro_SQL_SaveNum(PA_COD) & "  ")
                    End If

                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If

                    '--------------------------------------------------------------------------

                    If xOrderBy <> "" Then
                        strSql.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY Formulati.FR_DES ASC, PrincipiAttivi.PA_DES ASC ")
                    End If

                    '###################################################


                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta
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



    '#############################################################################################################
    'valore opzionale: Visualizza_Titolo con default = false
    Public Function PrincipiAttivi_from_FrCod(ByVal Fr_Cod As Integer, _
                                                ByVal Visualizza_Titolo As Boolean, _
                                                ByVal xFiltroAggiuntivo As String, _
                                                ByVal xOrderBy As String, _
                                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As String

        Dim PrincipiAttivi As String = ""
        Dim DT As DataTable
        Dim i As Integer

        DT = Leggi(Fr_Cod, _
                    0, _
                    objParametri.FinestraTemporaleInizio, _
                    objParametri.FinestraTemporaleFine, _
                     enumSelezioneVariabile.Selezione_JoinDescrizioni, _
                    xFiltroAggiuntivo, _
                    xOrderBy, _
                    objParametri)

        If Not DT Is Nothing AndAlso DT.Rows.Count > 0 Then

            For i = 0 To DT.Rows.Count - 1

                If Visualizza_Titolo = True Then
                    PrincipiAttivi &= DT.Rows(i).Item("pa_des") & _
                                      " ( " & DT.Rows(i).Item("Titolo") & "% ), " & vbCrLf
                Else
                    PrincipiAttivi &= DT.Rows(i).Item("pa_des") + ", "
                End If

            Next

            PrincipiAttivi = Left(PrincipiAttivi, PrincipiAttivi.Length - 2)

        End If


        Return PrincipiAttivi


    End Function

    '#############################################################################################################
    'Restituisce un DataTable contenente Fr_Cod e Bio (1=Bio,0=non Bio)
    Public Function Leggi_Biologici(ByVal FR_COD As String,
                                      ByVal Stato_Cod As String,
                                      ByVal Validita_Inizio As Date,
                                      ByVal Validita_Fine As Date,
                                        ByVal xFiltroAggiuntivo As String,
                                        ByVal xOrderBy As String,
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                  ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.FormulatixPrincipiAttivi_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As New DataTable
        Dim Dr As DataRow
        Dim DT_App As DataTable
        Dim i, j As Integer

        DT.Columns.Add(New DataColumn("Fr_Cod", GetType(Integer)))
        DT.Columns.Add(New DataColumn("Fr_Des", GetType(String)))
        DT.Columns.Add(New DataColumn("Bio", GetType(Integer)))

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;")
            StrSQL.Append(" SELECT Formulati.Fr_Des, Formulati.Data_Reg, FormulatixPrincipiAttivi.Fr_Cod,  ")
            StrSQL.Append("   FormulatixPrincipiAttivi.Pa_Cod, FormulatixPrincipiAttivi.Titolo, ")
            StrSQL.Append("   PrincipiAttivi.Pa_Des, ISNULL(PrincipiAttivi_BIO.PA_cod,0) AS PA_cod_BIO ")

            StrSQL.Append("  FROM  FormulatixPrincipiAttivi INNER JOIN ")
            StrSQL.Append("  Formulati ON FormulatixPrincipiAttivi.Fr_Cod = Formulati.Fr_Cod INNER JOIN ")
            StrSQL.Append("  PrincipiAttivi ON FormulatixPrincipiAttivi.Pa_Cod = PrincipiAttivi.Pa_Cod LEFT OUTER JOIN ")
            StrSQL.Append("  PrincipiAttivi_BIO ON PrincipiAttivi.Pa_Cod = PrincipiAttivi_BIO.PA_cod ")

            If Stato_Cod <> "" Then
                StrSQL.Append("  INNER JOIN  FormulatixAmbitoEstero ON Formulati.Fr_Cod = FormulatixAmbitoEstero.Fr_Cod AND FormulatixAmbitoEstero.Stato_Cod ='" & Agro_SQL_SaveText(Stato_Cod) & "' " & vbCrLf)
            End If

            StrSQL.Append("  WHERE FormulatixPrincipiAttivi.Validita_inizio < " & Agro_SQL_SaveDate(Validita_Fine))
            StrSQL.Append("  AND   FormulatixPrincipiAttivi.Validita_Fine > " & Agro_SQL_SaveDate(Validita_Inizio))
            StrSQL.Append("  AND   Formulati.Validita_inizio < " & Agro_SQL_SaveDate(Validita_Fine))
            StrSQL.Append("  AND   Formulati.Validita_Fine > " & Agro_SQL_SaveDate(Validita_Inizio))
            StrSQL.Append("  AND   PrincipiAttivi.Validita_inizio < " & Agro_SQL_SaveDate(Validita_Fine))
            StrSQL.Append("  AND   PrincipiAttivi.Validita_Fine > " & Agro_SQL_SaveDate(Validita_Inizio))

            If FR_COD <> "0" Then
                StrSQL.Append(" AND FormulatixPrincipiAttivi.FR_COD IN (" & Agro_SQL_Save_Clausola_IN(FR_COD) & ")  ")
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                strSql.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY Formulati.FR_COD ASC, PrincipiAttivi.PA_COD ASC ")
            End If

            '--------------------------------------------------------------------------
            DT_App = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            If Not DT_App Is Nothing AndAlso DT_App.Rows.Count > 0 Then

                Dim FormulatoBio As Integer = 1
                Dim LastFr_Cod As Integer = -1
                Dim LastFr_Des As String = ""

                Dim objCore As New AgronicaCoreDataProvider.DatatableUtility

                Dim FrCod() As String = objCore.SelectDistinct(DT_App, "Fr_Cod")

                If Not FrCod Is Nothing Then

                    For i = 0 To FrCod.Length - 1

                        Dim DrFrCod() As DataRow
                        FormulatoBio = 1

                        DrFrCod = DT_App.Select("Fr_Cod=" & FrCod(i).ToString)

                        If Not DrFrCod Is Nothing AndAlso DrFrCod.Length > 0 Then

                            For j = 0 To DrFrCod.Length - 1
                                If DrFrCod(j).Item("PA_cod_BIO") = 0 Then
                                    FormulatoBio = 0
                                    Exit For
                                End If
                            Next

                            'Aggiungo al dt
                            Dr = DT.NewRow
                            Dr.Item("Fr_Des") = DrFrCod(0).Item("Fr_Des")
                            Dr.Item("Fr_Cod") = DrFrCod(0).Item("Fr_Cod")
                            Dr.Item("Bio") = FormulatoBio
                            DT.Rows.Add(Dr)

                        End If

                    Next

                End If

            End If



        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT


    End Function



End Class
