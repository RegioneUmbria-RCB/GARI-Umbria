
Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.DataProviderExtensions
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri

Public Class PrincipiAttivi_R
    Inherits AgronicaCoreDataProvider.DataProvider


    Dim hashTable As Hashtable = New Hashtable()

    Public Function LeggiConFiltroContesto(
        ByVal PA_COD As Int32,
        ByVal ListaContestiInclusi As List(Of Integer),
        ByVal ListaContestiEsclusi As List(Of Integer),
        ByVal Validita_Inizio As Date,
        ByVal Validita_Fine As Date,
        ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile,
        ByVal xFiltroAggiuntivo As String,
        ByVal xOrderBy As String,
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
    ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.PrincipiAttivi_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable



        Try


            StrSQL.Length = 0

            StrSQL.AppendLine("Select distinct")
            StrSQL.AppendLine("  pa.pa_cod")
            StrSQL.AppendLine("  , pa.pa_des")
            StrSQL.AppendLine("from PrincipiAttiviXPrincipiAttivi_Contesto ppc")
            StrSQL.AppendLine("inner join PrincipiAttivi pa")
            StrSQL.AppendLine("  on pa.pa_cod = ppc.PA_COD")


            If ListaContestiEsclusi.Count > 0 Then
                StrSQL.AppendLine("where ppc.principiAttivi_Contesto_COD Not in (" & Agro_SQL_Save_Clausola_IN(String.Join(",", ListaContestiEsclusi)) & ")")
            Else
                StrSQL.AppendLine("where ppc.principiAttivi_Contesto_COD in (" & Agro_SQL_Save_Clausola_IN(String.Join(",", ListaContestiInclusi)) & ")")
            End If


            StrSQL.AppendLine(" AND pa.Validita_inizio < " & Agro_SQL_SaveDate(Validita_Fine) & " ")
            StrSQL.AppendLine(" AND pa.Validita_Fine > " & Agro_SQL_SaveDate(Validita_Inizio) & " ")


            If PA_COD <> 0 Then
                StrSQL.AppendLine(" AND pa.PA_COD =  " & Agro_SQL_SaveNum(PA_COD) & "  ")
            End If


            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo,, objParametri))
            End If

            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy))
            Else
                StrSQL.AppendLine(" ORDER BY pa.PA_DES ASC ")
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


    Public Function Leggi(ByVal PA_COD As Int32,
                            ByVal Validita_Inizio As Date,
                            ByVal Validita_Fine As Date,
                            ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile,
                            ByVal xFiltroAggiuntivo As String,
                            ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                          ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.PrincipiAttivi_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi
                    StrSQL.Length = 0

                    StrSQL.AppendLine(" SELECT PA_Des, PA_Cod FROM  PrincipiAttivi" &
                                    " WHERE PrincipiAttivi.Validita_inizio < " & Agro_SQL_SaveDate(Validita_Fine) & " " &
                                    " AND   PrincipiAttivi.Validita_Fine > " & Agro_SQL_SaveDate(Validita_Inizio) & " ")


                    If PA_COD <> 0 Then
                        StrSQL.AppendLine(" AND PrincipiAttivi.PA_COD =  " & Agro_SQL_SaveNum(PA_COD) & "  ")
                    End If


                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo,, objParametri))
                    End If

                    '--------------------------------------------------------------------------

                    If xOrderBy <> "" Then
                        StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy))
                    Else
                        StrSQL.AppendLine(" ORDER BY PrincipiAttivi.PA_DES ASC ")
                    End If

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta

                    StrSQL.Length = 0

                    StrSQL.AppendLine(" SELECT * FROM  PrincipiAttivi" &
                                    " WHERE PrincipiAttivi.Validita_inizio < " & Agro_SQL_SaveDate(Validita_Fine) & " " &
                                    " AND   PrincipiAttivi.Validita_Fine > " & Agro_SQL_SaveDate(Validita_Inizio) & " ")


                    If PA_COD <> 0 Then
                        StrSQL.AppendLine(" AND PrincipiAttivi.PA_COD =  " & Agro_SQL_SaveNum(PA_COD) & "  ")
                    End If


                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo))
                    End If

                    '--------------------------------------------------------------------------

                    If xOrderBy <> "" Then
                        StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy))
                    Else
                        StrSQL.AppendLine(" ORDER BY PrincipiAttivi.PA_DES ASC ")
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

    Public Function Leggi_Bio(ByVal PA_COD As Int32,
                              ByVal Validita_Inizio As Date,
                              ByVal Validita_Fine As Date,
                              ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile,
                              ByVal xFiltroAggiuntivo As String,
                              ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                          ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.PrincipiAttivi_R.Leggi_Bio()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi
                    StrSQL.Length = 0

                    StrSQL.AppendLine(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;")
                    StrSQL.AppendLine(" SELECT PrincipiAttivi.PA_Des, PrincipiAttivi.PA_Cod " &
                                  " FROM  PrincipiAttivi_BIO INNER JOIN  PrincipiAttivi ON PrincipiAttivi_BIO.PA_cod = PrincipiAttivi.Pa_Cod " &
                                    " WHERE PrincipiAttivi.Validita_inizio < " & Agro_SQL_SaveDate(Validita_Fine) & " " &
                                    " AND   PrincipiAttivi.Validita_Fine > " & Agro_SQL_SaveDate(Validita_Inizio) & " ")


                    If PA_COD <> 0 Then
                        StrSQL.AppendLine(" AND PrincipiAttivi.PA_COD =  " & Agro_SQL_SaveNum(PA_COD) & "  ")
                    End If


                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo))
                    End If

                    '--------------------------------------------------------------------------

                    If xOrderBy <> "" Then
                        StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy))
                    Else
                        StrSQL.AppendLine(" ORDER BY PrincipiAttivi.PA_DES ASC ")
                    End If

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta

                    StrSQL.Length = 0

                    StrSQL.AppendLine(" SELECT * " &
                                  " FROM  PrincipiAttivi_BIO INNER JOIN  PrincipiAttivi ON PrincipiAttivi_BIO.PA_cod = PrincipiAttivi.Pa_Cod " &
                                    " WHERE PrincipiAttivi.Validita_inizio < " & Agro_SQL_SaveDate(Validita_Fine) & " " &
                                    " AND   PrincipiAttivi.Validita_Fine > " & Agro_SQL_SaveDate(Validita_Inizio) & " ")


                    If PA_COD <> 0 Then
                        StrSQL.AppendLine(" AND PrincipiAttivi.PA_COD =  " & Agro_SQL_SaveNum(PA_COD) & "  ")
                    End If


                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo))
                    End If

                    '--------------------------------------------------------------------------

                    If xOrderBy <> "" Then
                        StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy))
                    Else
                        StrSQL.AppendLine(" ORDER BY PrincipiAttivi.PA_DES ASC ")
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

    Public Function Leggi_Bio_Cached(ByVal PA_COD As Integer,
                                     ByVal Validita_Inizio As Date,
                                     ByVal Validita_Fine As Date,
                                     ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile,
                                     ByVal xFiltroAggiuntivo As String,
                                     ByVal xOrderBy As String,
                                     ByRef objParametri As AgronicaCoreParametri
                                     ) As String

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.PrincipiAttivi_R.Leggi_Bio_Cached()"

        Dim MessaggioErrore As String = ""
        Dim strPABio As String = ""

        Try
            Dim key As String = "key_" & PA_COD
            If hashTable.Contains(key) Then
                Return hashTable(key)
            End If

            Dim LeggiPABio As New AgronicaCoreMetaSchemaDAL.PrincipiAttivi_R
            Dim dtPaBio As DataTable =
                LeggiPABio.Leggi_Bio(PA_COD,
                                     Validita_Inizio,
                                     Validita_Fine,
                                     enumSelezioneVariabile.Selezione_TabellaCompleta,
                                     "",
                                     "",
                                     objParametri)

            For Each rdtPaBio As DataRow In dtPaBio.Rows
                If Not IsDBNull(rdtPaBio("Pa_Cod")) AndAlso rdtPaBio("Pa_Cod") <> 0 Then
                    'Considero il Pa_cod
                    strPABio = strPABio & If(Trim(strPABio) = "", "", ",") & rdtPaBio("Pa_Cod")
                End If
            Next

            hashTable.Add(key, strPABio)

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            strPABio = ""
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return strPABio

    End Function

    '#############################################################################################################
    '#############################################################################################################
    '#############################################################################################################


    Public Function Leggi_Da_StrPa_Cod(ByVal strPA_COD As String,
                                           ByVal Validita_Inizio As Date,
                                           ByVal Validita_Fine As Date,
                                           ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile,
                                           ByVal xFiltroAggiuntivo As String,
                                           ByVal xOrderBy As String,
                                               ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                           ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.PrincipiAttivi_R.Leggi_Da_StrPa_Cod()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi
                    StrSQL.AppendLine(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;")
                    StrSQL.AppendLine(" SELECT PA_Des, PA_Cod  FROM  PrincipiAttivi " &
                                 " WHERE PrincipiAttivi.Validita_inizio < " & Agro_SQL_SaveDate(Validita_Fine) & " " &
                                 " AND   PrincipiAttivi.Validita_Fine > " & Agro_SQL_SaveDate(Validita_Inizio) & " ")


                    If Trim(strPA_COD) <> "" Then
                        StrSQL.AppendLine(" AND PrincipiAttivi.PA_COD IN (" & Agro_SQL_Save_Clausola_IN(strPA_COD) & ") ")
                    End If


                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo))
                    End If

                    '--------------------------------------------------------------------------

                    If xOrderBy <> "" Then
                        StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy))
                    Else
                        StrSQL.AppendLine(" ORDER BY PrincipiAttivi.PA_DES ASC ")
                    End If

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta

                    StrSQL.Length = 0

                    StrSQL.AppendLine(" SELECT * FROM  PrincipiAttivi " &
                                    " WHERE PrincipiAttivi.Validita_inizio < " & Agro_SQL_SaveDate(Validita_Fine) & " " &
                                    " AND   PrincipiAttivi.Validita_Fine > " & Agro_SQL_SaveDate(Validita_Inizio) & " ")


                    If Trim(strPA_COD) <> "" Then
                        StrSQL.AppendLine(" AND PrincipiAttivi.PA_COD IN " & Agro_SQL_Save_Clausola_IN(strPA_COD) & "  ")
                    End If


                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo))
                    End If

                    '--------------------------------------------------------------------------

                    If xOrderBy <> "" Then
                        StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy))
                    Else
                        StrSQL.AppendLine(" ORDER BY PrincipiAttivi.PA_DES ASC ")
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


    Public Function Leggi_SA_FAM_utilizzatiINanalisi(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                          ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.PrincipiAttivi_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine(" select (0-Fam_Cod) as parametro, Descrizione  from FamigliePrincipiAttivi ")
            StrSQL.AppendLine(" where Fam_Cod in ( ")
            StrSQL.AppendLine("     SELECT    distinct convert (varchar, ABS( Analisi_Dettagli.Analisi_Parametro_Cod)) ")
            StrSQL.AppendLine("     FROM         Analisi_Dettagli INNER JOIN ")
            StrSQL.AppendLine("     Analisi_Testata ON Analisi_Dettagli.Analisi_SuperUser = Analisi_Testata.Analisi_SuperUser AND ")
            StrSQL.AppendLine("     Analisi_Dettagli.Analisi_Testata_Cod = Analisi_Testata.Analisi_Testata_Cod ")
            StrSQL.AppendLine(" WHERE(Analisi_Dettagli.Analisi_Parametro_Cod < 0) ")
            StrSQL.AppendLine("     and   Analisi_Testata_Tipo = 8) ")
            StrSQL.AppendLine(" union ")
            StrSQL.AppendLine(" select pa_cod as parametro, pa_des as Descrizione  from PrincipiAttivi ")
            StrSQL.AppendLine(" where pa_cod in ( ")
            StrSQL.AppendLine(" SELECT    distinct convert (varchar, ABS( Analisi_Dettagli.Analisi_Parametro_Cod)) ")
            StrSQL.AppendLine(" FROM         Analisi_Dettagli INNER JOIN ")
            StrSQL.AppendLine("       Analisi_Testata ON Analisi_Dettagli.Analisi_SuperUser = Analisi_Testata.Analisi_SuperUser AND ")
            StrSQL.AppendLine("         Analisi_Dettagli.Analisi_Testata_Cod = Analisi_Testata.Analisi_Testata_Cod ")
            StrSQL.AppendLine("         WHERE(Analisi_Dettagli.Analisi_Parametro_Cod > 0) ")
            StrSQL.AppendLine(" and   Analisi_Testata_Tipo = 8) ")

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


End Class
