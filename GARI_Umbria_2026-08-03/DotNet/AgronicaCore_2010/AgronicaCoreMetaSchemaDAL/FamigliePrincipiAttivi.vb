
Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider

Public Class FamigliePrincipiAttivi_R


    Inherits AgronicaCoreDataProvider.DataProvider



    Public Function Leggi(ByVal Fam_COD As Int32, _
                            ByVal Validita_Inizio As Date, _
                            ByVal Validita_Fine As Date, _
                            ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile, _
                            ByVal xFiltroAggiuntivo As String, _
                            ByVal xOrderBy As String, _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                          ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.FamigliePrincipiAttivi_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi
                    StrSQL.Length = 0

                    StrSQL.Append(" SELECT Fam_Cod , Descrizione FROM  FamigliePrincipiAttivi" & _
                                    " WHERE FamigliePrincipiAttivi.Validita_inizio < " & Agro_SQL_SaveDate(Validita_Fine) & " " & _
                                    " AND   FamigliePrincipiAttivi.Validita_Fine > " & Agro_SQL_SaveDate(Validita_Inizio) & " ")


                    If Fam_COD <> 0 Then
                        StrSQL.Append(" AND FamigliePrincipiAttivi.Fam_COD =  " & Agro_SQL_SaveNum(Fam_COD) & "  ")
                    End If


                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If

                    '--------------------------------------------------------------------------

                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY FamigliePrincipiAttivi.descrizione ASC ")
                    End If

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta

                    StrSQL.Append(" SELECT * FROM  FamigliePrincipiAttivi" & _
                                    " WHERE FamigliePrincipiAttivi.Validita_inizio < " & Agro_SQL_SaveDate(Validita_Fine) & " " & _
                                    " AND   FamigliePrincipiAttivi.Validita_Fine > " & Agro_SQL_SaveDate(Validita_Inizio) & " ")


                    If Fam_COD <> 0 Then
                        StrSQL.Append(" AND FamigliePrincipiAttivi.Fam_COD =  " & Agro_SQL_SaveNum(Fam_COD) & "  ")
                    End If


                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If

                    '--------------------------------------------------------------------------

                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY FamigliePrincipiAttivi.descrizione ASC ")
                    End If

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni


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







    Public Function Leggi_FamigliePrincipiAttivi_Utilizzati(ByVal Validita_Inizio As Date, _
                            ByVal Validita_Fine As Date, _
                            ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile, _
                            ByVal xFiltroAggiuntivo As String, _
                            ByVal xOrderBy As String, _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                          ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.FamigliePrincipiAttivi_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi
                    StrSQL.Length = 0

                    StrSQL.Append(" SELECT DISTINCT FamigliePrincipiAttivi.Fam_Cod, FamigliePrincipiAttivi.Descrizione" & _
                                  " From FamigliePrincipiAttivi INNER JOIN " & _
                                  "  PrincipiAttivi ON FamigliePrincipiAttivi.Fam_Cod = PrincipiAttivi.Fam_Cod " & _
                                    " WHERE FamigliePrincipiAttivi.Validita_inizio < " & Agro_SQL_SaveDate(Validita_Fine) & " " & _
                                    " AND   FamigliePrincipiAttivi.Validita_Fine > " & Agro_SQL_SaveDate(Validita_Inizio) & " ")
                     
                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If

                    '--------------------------------------------------------------------------

                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY FamigliePrincipiAttivi.descrizione ASC ")
                    End If

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta

                    StrSQL.Length = 0

                    StrSQL.Append(" SELECT DISTINCT FamigliePrincipiAttivi.* " & _
                                  " From FamigliePrincipiAttivi INNER JOIN " & _
                                  "  PrincipiAttivi ON FamigliePrincipiAttivi.Fam_Cod = PrincipiAttivi.Fam_Cod " & _
                                    " WHERE FamigliePrincipiAttivi.Validita_inizio < " & Agro_SQL_SaveDate(Validita_Fine) & " " & _
                                    " AND   FamigliePrincipiAttivi.Validita_Fine > " & Agro_SQL_SaveDate(Validita_Inizio) & " ")

                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If

                    '--------------------------------------------------------------------------

                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY FamigliePrincipiAttivi.descrizione ASC ")
                    End If

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni


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




    Public Function Leggi_FamigliePrincipiAttivi_con_Contesto(ByVal Contesto As Integer, _
                            ByVal Validita_Inizio As Date, _
                            ByVal Validita_Fine As Date, _
                            ByVal xFiltroAggiuntivo As String, _
                            ByVal xOrderBy As String, _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                          ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.FamigliePrincipiAttivi_R.Leggi_FamigliePrincipiAttivi_con_Contesto()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.Append(" SELECT     FamigliePrincipiAttivi.Fam_Cod, PrincipiAttiviXFamigliePrincipiAttivi.pa_cod,  FamigliePrincipiAttivi.Descrizione " & _
                          " FROM         (select * from FamigliePrincipiAttivi where isnumeric(fam_cod)=1 ) FamigliePrincipiAttivi  INNER JOIN " & _
                          "  PrincipiAttiviXFamigliePrincipiAttivi ON FamigliePrincipiAttivi.Fam_Cod = PrincipiAttiviXFamigliePrincipiAttivi.fam_cod " & _
                            " WHERE FamigliePrincipiAttivi.Validita_inizio < " & Agro_SQL_SaveDate(Validita_Fine) & " " & _
                            " AND   FamigliePrincipiAttivi.Validita_Fine > " & Agro_SQL_SaveDate(Validita_Inizio) & " ")
            If Contesto <> 0 Then
                StrSQL.Append(" AND FamigliePrincipiAttivi_Contesto_Cod =" & Contesto)
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY FamigliePrincipiAttivi.descrizione ASC ")
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





    '--per verificare quali classi sono doppie
    'select * from FamigliePrincipiAttivi where Descrizione in (  
    'SELECT   Descrizione 
    'FROM         FamigliePrincipiAttivi
    'group by descrizione 
    'having count(descrizione) > 1)

End Class
