Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.AgronicaCoreParametri

Public Class Modelli_R
    Inherits AgronicaCoreDataProvider.DataProvider


    '##############################################################################################
    Public Function Leggi_Modelli( _
                                        ByVal Codice As String, _
                                        ByVal Nome As String, _
                                        ByVal Ciclo_Nome As String, _
                                        ByVal MatPrima_Nome As String, _
                                        ByVal CatMerceologica As String, _
                                        ByVal xFiltroAggiuntivo As String, _
                                        ByVal xOrderBy As String, _
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                        ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "LabControlloQualitaDAL.Modelli_R.Leggi_Modelli()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim strSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            strSQL.Length = 0

            strSQL.Append(" SELECT DISTINCT m.PivaSuperUser, m.Codice, m.Nome, m.Descrizione, m.Ciclo_Nome, m.MatPrima_Nome, m.CatMerceologica " + vbCrLf)
            strSQL.Append(" FROM LCQ_Modelli AS m" + vbCrLf)

            strSQL.Append(" WHERE m.PivaSuperUser = " & Agro_SQL_SaveText_NULL(objParametri.PivaSuperUser) + vbCrLf)


            If Not IsNothing(Codice) Then
                strSQL.Append(" AND m.Codice = " & Agro_SQL_SaveText_NULL(Codice))
            End If

            If Not IsNothing(Nome) Then
                strSQL.Append(" AND m.Nome = " & Agro_SQL_SaveText_NULL(Nome))
            End If

            If Not IsNothing(Ciclo_Nome) Then
                strSQL.Append(" AND m.Ciclo_Nome = " & Agro_SQL_SaveText_NULL(Ciclo_Nome))
            End If

            If Not IsNothing(MatPrima_Nome) Then
                strSQL.Append(" AND m.MatPrima_Nome = " & Agro_SQL_SaveText_NULL(MatPrima_Nome))
            End If

            If Not IsNothing(CatMerceologica) Then
                strSQL.Append(" AND m.CatMerceologica = " & Agro_SQL_SaveText_NULL(CatMerceologica))
            End If

            If xFiltroAggiuntivo <> "" Then
                strSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    strSQL.Append(" AND   m.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    strSQL.Append(" AND   m.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                strSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, strSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message & " QUERY: " & strSQL.ToString.Replace(vbCrLf, " ")
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function

    '##############################################################################################
    Public Function Leggi_ModelliXproduzione( _
                                        ByVal Codice As String, _
                                        ByVal Nome As String, _
                                        ByVal Ciclo_Nome As String, _
                                        ByVal MatPrima_Nome As String, _
                                        ByVal CatMerceologica As String, _
                                        ByVal xFiltroAggiuntivo As String, _
                                        ByVal xOrderBy As String, _
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                        ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "LabControlloQualitaDAL.Modelli_R.Leggi_Modelli()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim strSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            strSQL.Length = 0

            strSQL.Append(" SELECT DISTINCT m.PivaSuperUser, m.Codice, m.Nome, m.Descrizione, m.Ciclo_Nome, m.MatPrima_Nome, m.CatMerceologica " + vbCrLf)
            strSQL.Append(" FROM LCQ_Modelli AS m" + vbCrLf)

            strSQL.Append(" WHERE m.PivaSuperUser = " & Agro_SQL_SaveText_NULL(objParametri.PivaSuperUser) + vbCrLf)


            If Not IsNothing(Codice) Then
                strSQL.Append(" AND m.Codice = " & Agro_SQL_SaveText_NULL(Codice))
            End If

            If Not IsNothing(Nome) Then
                strSQL.Append(" AND m.Nome = " & Agro_SQL_SaveText_NULL(Nome))
            End If

            If Not IsNothing(Ciclo_Nome) Then
                strSQL.Append(" AND (m.Ciclo_Nome IS NULL OR m.Ciclo_Nome = " & Agro_SQL_SaveText_NULL(Ciclo_Nome) & ")")
            End If

            If Not IsNothing(MatPrima_Nome) Then
                strSQL.Append(" AND (m.MatPrima_Nome IS NULL OR m.MatPrima_Nome = " & Agro_SQL_SaveText_NULL(MatPrima_Nome) & ")")
            End If

            If Not IsNothing(CatMerceologica) Then
                strSQL.Append(" AND (m.CatMerceologica IS NULL OR m.CatMerceologica = " & Agro_SQL_SaveText_NULL(CatMerceologica) & ")")
            End If

            If xFiltroAggiuntivo <> "" Then
                strSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    strSQL.Append(" AND   m.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    strSQL.Append(" AND   m.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                strSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, strSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message & " QUERY: " & strSQL.ToString.Replace(vbCrLf, " ")
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function

    Function Verifica_ModEsiste( _
                                    ByVal Codice As String, _
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                    ) As Boolean

        '----- Descrizione
        Dim NomeRoutine As String = "LabControlloQualitaDAL.Modelli_R.Verifica_ModEsiste()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim strSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            strSQL.Length = 0

            strSQL.Append(" SELECT 1 " + vbCrLf)
            strSQL.Append(" FROM LCQ_Modelli " + vbCrLf)

            strSQL.Append(" WHERE PivaSuperUser = " & Agro_SQL_SaveText_NULL(objParametri.PivaSuperUser) + vbCrLf)
            strSQL.Append(" AND Codice = " & Agro_SQL_SaveText_NULL(Codice))


            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    strSQL.Append(" AND   Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    strSQL.Append(" AND   Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, strSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message & " QUERY: " & strSQL.ToString.Replace(vbCrLf, " ")
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        If DT.Rows.Count = 0 Then
            Return False
        Else
            Return True
        End If

    End Function

End Class




'#################################################################
'#################################################################
'#################################################################

Public Class Modelli_W
    Inherits AgronicaCoreDataProvider.DataProvider



    '##############################################################################################
    Public Function Scrivi(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                ByVal Codice As String, _
                ByVal Nome As String, _
                ByVal Descrizione As String, _
                ByVal Ciclo_Nome As String, _
                ByVal MatPrima_Nome As String, _
                ByVal CatMerceologica As String, _
                Optional ByVal Data_creazione As Date = #2/1/1900#, _
                Optional ByVal Data_modifica As Date = #2/1/1900#, _
                Optional ByVal username_creazione As String = "", _
                Optional ByVal username_modifica As String = "" _
                ) As Boolean


        Dim NomeRoutine As String = "LabControlloQualitaDAL.Modelli_W.Scrivi()"

        '====================================================================================
        'Parametri opzionali :

        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
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
            StrSQL.Length = 0
            StrSQL.Append(" INSERT INTO  LCQ_Modelli" + vbCrLf)

            StrSQL.Append("              (")
            StrSQL.Append("              PivaSuperUser,             Codice, ")
            StrSQL.Append("              Nome,                      Descrizione, ")
            StrSQL.Append("              Ciclo_Nome,                 MatPrima_Nome, ")
            StrSQL.Append("              CatMerceologica, ")

            StrSQL.Append("              Inviato,            datainvio, ")
            StrSQL.Append("              Data_Creazione,     Data_Modifica, ")
            StrSQL.Append("              UserName_Creazione, UserName_Modifica, ")
            StrSQL.Append("              Validita_Inizio,    Validita_Fine " + vbCrLf)
            StrSQL.Append("              ) ")

            StrSQL.Append(" VALUES ( ")

            StrSQL.Append("			 " & Agro_SQL_SaveText_NULL(objParametri.PivaSuperUser))
            StrSQL.Append("			," & Agro_SQL_SaveText_NULL(Codice))
            StrSQL.Append("			," & Agro_SQL_SaveText_NULL(Nome))
            StrSQL.Append("			," & Agro_SQL_SaveText_NULL(NothingToDBNull(Descrizione)))
            StrSQL.Append("			," & Agro_SQL_SaveText_NULL(NothingToDBNull(Ciclo_Nome)))
            StrSQL.Append("			," & Agro_SQL_SaveText_NULL(NothingToDBNull(MatPrima_Nome)))
            StrSQL.Append("			," & Agro_SQL_SaveText_NULL(NothingToDBNull(CatMerceologica)))


            StrSQL.Append("         , 0  " + vbCrLf)
            StrSQL.Append("         , Null  " + vbCrLf)
            StrSQL.Append("			, " & Agro_SQL_SaveDateTime(Data_creazione) & "  ")
            StrSQL.Append("			, " & Agro_SQL_SaveDateTime(Data_modifica) & "  ")
            StrSQL.Append("			,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            StrSQL.Append("			,'" & Agro_SQL_SaveText(username_modifica) & "' ")
            StrSQL.Append("			, " & Agro_SQL_SaveDate(AGRODATAINIZIO) & "  ")
            StrSQL.Append("			, " & Agro_SQL_SaveDate(AGRODATAFINE) & "  ")



            StrSQL.Append(") ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message & " QUERY: " & strSQL.ToString.Replace(vbCrLf, " ")
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return xRisp

    End Function


    '##############################################################################################
    Public Function Modifica(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                ByVal Old_Codice As String, _
                ByVal New_Codice As String, _
                ByVal New_Nome As String, _
                ByVal New_Descrizione As String, _
                ByVal New_Ciclo_Nome As String, _
                ByVal New_MatPrima_Nome As String, _
                ByVal New_CatMerceologica As String, _
                Optional ByVal Data_modifica As Date = #2/1/1900#, _
                Optional ByVal username_modifica As String = "" _
                ) As Boolean


        Dim NomeRoutine As String = "LabControlloQualitaDAL.Modelli_W.Modifica()"

        '====================================================================================
        'Parametri opzionali :

        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If Data_modifica = #2/1/1900# Then
                Data_modifica = DateTime.Now
            End If

            If username_modifica = "" Then
                username_modifica = objParametri.UsernameOperazione
            End If


            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append(" UPDATE LCQ_Modelli SET" + vbCrLf)

            StrSQL.Append("  Codice = " & Agro_SQL_SaveText_NULL(New_Codice) & vbCrLf)
            StrSQL.Append(", Nome = " & Agro_SQL_SaveText_NULL(New_Nome) & vbCrLf)
            StrSQL.Append(", Descrizione = " & Agro_SQL_SaveText_NULL(NothingToDBNull(New_Descrizione)) & vbCrLf)
            StrSQL.Append(", Ciclo_Nome = " & Agro_SQL_SaveText_NULL(NothingToDBNull(New_Ciclo_Nome)) & vbCrLf)
            StrSQL.Append(", MatPrima_Nome = " & Agro_SQL_SaveText_NULL(NothingToDBNull(New_MatPrima_Nome)) & vbCrLf)
            StrSQL.Append(", CatMerceologica = " & Agro_SQL_SaveText_NULL(NothingToDBNull(New_CatMerceologica)) & vbCrLf)


            StrSQL.Append(", Data_Modifica = " & Agro_SQL_SaveDateTime(Data_modifica) & vbCrLf)
            StrSQL.Append(", UserName_Modifica = '" & Agro_SQL_SaveText(username_modifica) & "' " & vbCrLf)

            StrSQL.Append("	WHERE PivaSuperUser = " & Agro_SQL_SaveText_NULL(objParametri.PivaSuperUser))
            StrSQL.Append("	AND Codice =		" & Agro_SQL_SaveText_NULL(Old_Codice))

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message & " QUERY: " & strSQL.ToString.Replace(vbCrLf, " ")
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return xRisp

    End Function

    '##############################################################################################
    Public Function ModificaProduzione(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                ByVal Old_Codice As String, _
                ByVal New_Ciclo_Nome As String, _
                ByVal New_MatPrima_Nome As String, _
                ByVal New_CatMerceologica As String, _
                Optional ByVal Data_modifica As Date = #2/1/1900#, _
                Optional ByVal username_modifica As String = "" _
                ) As Boolean


        Dim NomeRoutine As String = "LabControlloQualitaDAL.Modelli_W.Modifica()"

        '====================================================================================
        'Parametri opzionali :

        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If Data_modifica = #2/1/1900# Then
                Data_modifica = DateTime.Now
            End If

            If username_modifica = "" Then
                username_modifica = objParametri.UsernameOperazione
            End If


            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append(" UPDATE LCQ_Modelli SET" + vbCrLf)

            StrSQL.Append(" Ciclo_Nome = " & Agro_SQL_SaveText_NULL(NothingToDBNull(New_Ciclo_Nome)) & vbCrLf)
            StrSQL.Append(", MatPrima_Nome = " & Agro_SQL_SaveText_NULL(NothingToDBNull(New_MatPrima_Nome)) & vbCrLf)
            StrSQL.Append(", CatMerceologica = " & Agro_SQL_SaveText_NULL(NothingToDBNull(New_CatMerceologica)) & vbCrLf)


            StrSQL.Append(", Data_Modifica = " & Agro_SQL_SaveDateTime(Data_modifica) & vbCrLf)
            StrSQL.Append(", UserName_Modifica = '" & Agro_SQL_SaveText(username_modifica) & "' " & vbCrLf)

            StrSQL.Append("	WHERE PivaSuperUser = " & Agro_SQL_SaveText_NULL(objParametri.PivaSuperUser))
            StrSQL.Append("	AND Codice =		" & Agro_SQL_SaveText_NULL(Old_Codice))

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message & " QUERY: " & strSQL.ToString.Replace(vbCrLf, " ")
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return xRisp

    End Function


    '#################################################################
    'Public Function Cancella(ByVal xFiltroAggiuntivo As String, _
    '                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
    '                          ) As Boolean

    '    '----- Descrizione
    '    Dim NomeRoutine As String = "Cancella()"

    '    Dim MessaggioErrore As String = ""
    '    Dim StrSQL As New System.Text.StringBuilder
    '    Dim xRisp As Boolean = False

    '    Try
    '        '---------------------------------------------
    '        StrSQL.Length = 0

    '        '---------------------------------------------
    '        If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then
    '            StrSQL.Append(" UPDATE ... ")
    '            StrSQL.Append(" SET ")
    '            StrSQL.Append("         Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
    '            StrSQL.Append("         ,Data_Modifica= " & Agro_SQL_SaveDate(Date.Now) & " ")
    '            StrSQL.Append("         ,Inviato = -1 ")
    '            StrSQL.Append(" WHERE   1=1 ")
    '            StrSQL.Append(" AND     Inviato >= 0 ")
    '        Else
    '            StrSQL.Append(" DELETE FROM ... ")
    '            StrSQL.Append(" WHERE 1=1 ")
    '        End If
    '        '---------------------------------------------

    '        If xFiltroAggiuntivo <> "" Then
    '            StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
    '        End If

    '        '--------------------------------------------------------------------------
    '        xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
    '        '--------------------------------------------------------------------------

    '    Catch ex As Exception
    '        MessaggioErrore = ex.Message & " QUERY: " & strSQL.ToString.Replace(vbCrLf, " ")
    '        Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
    '        xRisp = False
    '        Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
    '    End Try

    '    Return xRisp

    'End Function




End Class
