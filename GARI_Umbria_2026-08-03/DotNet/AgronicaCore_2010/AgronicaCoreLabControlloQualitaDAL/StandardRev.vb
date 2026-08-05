Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.AgronicaCoreParametri

Public Class StandardRev_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function Leggi_StandardRev( _
                                        ByVal TipoStd As enum_LCQ_TipoModelloStandard, _
                                        ByVal DataDaVerificare As DateTime, _
                                        ByVal Nome As String, _
                                        ByVal Revisione As String, _
                                        ByVal Modello_Codice As String, _
                                        ByVal Modello_Revisione As String, _
                                        ByVal xFiltroAggiuntivo As String, _
                                        ByVal xOrderBy As String, _
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                        ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "LabControlloQualitaDAL.StandardRev_R.Leggi_StandardRev()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim strSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            strSQL.Length = 0

            strSQL.Append(" SELECT PivaSuperUser, Nome, Revisione , Modello_Codice, Modello_Revisione, DataValiditaInizio, DataValiditaFine" + vbCrLf)
            strSQL.Append(" FROM LCQ_StandardRev " + vbCrLf)

            strSQL.Append(" WHERE PivaSuperUser = " & Agro_SQL_SaveText_NULL(objParametri.PivaSuperUser) + vbCrLf)

            Select Case TipoStd
                Case enum_LCQ_TipoModelloStandard.Tutti
                Case enum_LCQ_TipoModelloStandard.Attivo
                    strSQL.Append(" AND DataValiditaInizio IS NOT NULL AND DataValiditaInizio <= " & Agro_SQL_SaveDateTime_NULL(DataDaVerificare))
                    strSQL.Append(" AND (DataValiditaFine IS NULL OR DataValiditaFine >= " & Agro_SQL_SaveDateTime_NULL(DataDaVerificare) & " ) ")
                Case enum_LCQ_TipoModelloStandard.DaAttivare
                    strSQL.Append(" AND (DataValiditaInizio IS NULL OR (DataValiditaInizio IS NOT NULL AND DataValiditaInizio > " & Agro_SQL_SaveDateTime_NULL(DataDaVerificare) & " )) ")
                Case enum_LCQ_TipoModelloStandard.AttivoODaAttivare
                    strSQL.Append(" AND (DataValiditaFine IS NULL OR (DataValiditaFine IS NOT NULL AND DataValiditaFine >= " & Agro_SQL_SaveDateTime_NULL(DataDaVerificare) & " )) ")
                Case enum_LCQ_TipoModelloStandard.NonPiuValido
                    strSQL.Append(" AND DataValiditaFine IS NOT NULL AND DataValiditaFine < " & Agro_SQL_SaveDateTime_NULL(DataDaVerificare))
            End Select

            If Not IsNothing(Nome) Then
                strSQL.Append(" AND Nome = " & Agro_SQL_SaveText_NULL(Nome))
            End If

            If Not IsNothing(Revisione) Then
                strSQL.Append(" AND Revisione = " & Agro_SQL_SaveText_NULL(Revisione))
            End If

            If Not IsNothing(Modello_Codice) Then
                strSQL.Append(" AND Modello_Codice = " & Agro_SQL_SaveText_NULL(Modello_Codice))
            End If

            If Not IsNothing(Modello_Revisione) Then
                strSQL.Append(" AND Modello_Revisione = " & Agro_SQL_SaveText_NULL(Modello_Revisione))
            End If

            If xFiltroAggiuntivo <> "" Then
                strSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
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


    Function Verifica_StdEsiste( _
                                    ByVal Nome As String, _
                                    ByVal Revisione As String, _
                                    ByVal Modello_Codice As String, _
                                    ByVal Modello_Revisione As String, _
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                    ) As Boolean

        '----- Descrizione
        Dim NomeRoutine As String = "LabControlloQualitaDAL.StandardRev_R.Verifica_StdRevEsiste()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim strSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            strSQL.Length = 0

            strSQL.Append(" SELECT 1 " + vbCrLf)
            strSQL.Append(" FROM LCQ_StandardRev " + vbCrLf)

            strSQL.Append(" WHERE PivaSuperUser = " & Agro_SQL_SaveText_NULL(objParametri.PivaSuperUser) + vbCrLf)
            strSQL.Append(" AND Nome = " & Agro_SQL_SaveText_NULL(Nome))
            strSQL.Append(" AND Revisione = " & Agro_SQL_SaveText_NULL(Revisione))
            strSQL.Append(" AND Modello_Codice = " & Agro_SQL_SaveText_NULL(Modello_Codice))
            strSQL.Append(" AND Modello_Revisione = " & Agro_SQL_SaveText_NULL(Modello_Revisione))

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


    Function Verifica_StdRevAttivoDaAttivareInData( _
                                    ByVal Nome As String, _
                                    ByVal Revisione As String, _
                                    ByVal Modello_Codice As String, _
                                    ByVal Modello_Revisione As String, _
                                    ByVal DataDaVerificare As DateTime, _
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                    ) As Boolean

        '----- Descrizione
        Dim NomeRoutine As String = "LabControlloQualitaDAL.StandardRev_R.Verifica_StdRevAttivoDaAttivareInData()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim strSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            strSQL.Length = 0

            strSQL.Append(" SELECT 1 " + vbCrLf)
            strSQL.Append(" FROM LCQ_StandardRev " + vbCrLf)

            strSQL.Append(" WHERE PivaSuperUser = " & Agro_SQL_SaveText_NULL(objParametri.PivaSuperUser) + vbCrLf)
            strSQL.Append(" AND Nome = " & Agro_SQL_SaveText_NULL(Nome))
            strSQL.Append(" AND Revisione = " & Agro_SQL_SaveText_NULL(Revisione))
            strSQL.Append(" AND Modello_Codice = " & Agro_SQL_SaveText_NULL(Modello_Codice))
            strSQL.Append(" AND Modello_Revisione = " & Agro_SQL_SaveText_NULL(Modello_Revisione))
            strSQL.Append(" AND (DataValiditaFine IS NULL OR (DataValiditaFine IS NOT NULL AND DataValiditaFine >= " & Agro_SQL_SaveDateTime_NULL(DataDaVerificare) & " )) ")

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

    Function Verifica_StdRevInUso( _
                                    ByVal Modello_Codice As String, _
                                    ByVal Modello_Revisione As String, _
                                    ByVal Nome As String, _
                                    ByVal Revisione As String, _
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                    ) As Boolean

        '----- Descrizione
        Dim NomeRoutine As String = "LabControlloQualitaDAL.Standard_R.Verifica_StdRevInUso()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim strSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            strSQL.Length = 0

            strSQL.Append(" SELECT 1 " + vbCrLf)
            strSQL.Append(" FROM LCQ_StandardRev AS s" + vbCrLf)

            strSQL.Append(" INNER JOIN LCQ_Documenti AS d" + vbCrLf)
            strSQL.Append(" ON d.PivaSuperUser = s.PivaSuperUser" + vbCrLf)
            strSQL.Append(" AND d.Modello_Codice = s.Modello_Codice" + vbCrLf)
            strSQL.Append(" AND d.Modello_Revisione = s.Modello_Revisione" + vbCrLf)

            strSQL.Append(" WHERE s.PivaSuperUser = " & Agro_SQL_SaveText_NULL(objParametri.PivaSuperUser) + vbCrLf)
            strSQL.Append(" AND s.Modello_Codice = " & Agro_SQL_SaveText_NULL(Modello_Codice))
            strSQL.Append(" AND s.Modello_Revisione = " & Agro_SQL_SaveText_NULL(Modello_Revisione))
            strSQL.Append(" AND s.Nome = " & Agro_SQL_SaveText_NULL(Nome))
            strSQL.Append(" AND s.Revisione = " & Agro_SQL_SaveText_NULL(Revisione))

            'controllo che la data del doc sia dentro la validità dello standard
            strSQL.Append(" AND ( s.DataValiditaInizio IS NOT NULL AND s.DataValiditaInizio <= d.DataOraCreazione ) ")
            strSQL.Append(" AND ( s.DataValiditaFine IS NULL OR s.DataValiditaFine >= d.DataOraCreazione ) ")

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    strSQL.Append(" AND   s.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    strSQL.Append(" AND   s.Inviato =-1 ")
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


Public Class StandardRev_W
    Inherits AgronicaCoreDataProvider.DataProvider



    '##############################################################################################
    Public Function Scrivi(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                ByVal Nome As String, _
                ByVal Revisione As String, _
                ByVal Modello_Codice As String, _
                ByVal Modello_Revisione As String, _
                ByVal DataValiditaInizio As DateTime?, _
                ByVal DataValiditaFine As DateTime?, _
                Optional ByVal Data_creazione As Date = #2/1/1900#, _
                Optional ByVal Data_modifica As Date = #2/1/1900#, _
                Optional ByVal username_creazione As String = "", _
                Optional ByVal username_modifica As String = "" _
                ) As Boolean


        Dim NomeRoutine As String = "LabControlloQualitaDAL.Standard_W.Scrivi()"

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
            StrSQL.Append(" INSERT INTO  LCQ_StandardRev" + vbCrLf)

            StrSQL.Append("              (")
            StrSQL.Append("              PivaSuperUser,             Nome, ")
            StrSQL.Append("              Revisione,                 Modello_Codice, ")
            StrSQL.Append("              Modello_Revisione,          ")
            StrSQL.Append("              DataValiditaInizio,        DataValiditaFine, ")

            StrSQL.Append("              Inviato,            datainvio, ")
            StrSQL.Append("              Data_Creazione,     Data_Modifica, ")
            StrSQL.Append("              UserName_Creazione, UserName_Modifica, ")
            StrSQL.Append("              Validita_Inizio,    Validita_Fine " + vbCrLf)
            StrSQL.Append("              ) ")

            StrSQL.Append(" VALUES ( ")

            StrSQL.Append("			 " & Agro_SQL_SaveText_NULL(objParametri.PivaSuperUser))
            StrSQL.Append("			," & Agro_SQL_SaveText_NULL(Nome))
            StrSQL.Append("			," & Agro_SQL_SaveText_NULL(Revisione))
            StrSQL.Append("			," & Agro_SQL_SaveText_NULL(Modello_Codice))
            StrSQL.Append("			," & Agro_SQL_SaveText_NULL(Modello_Revisione))
            StrSQL.Append("			," & Agro_SQL_SaveDateTime_NULL(NothingToDBNull(DataValiditaInizio)))
            StrSQL.Append("			," & Agro_SQL_SaveDateTime_NULL(NothingToDBNull(DataValiditaFine)))


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

            MessaggioErrore = ex.Message & " QUERY: " & StrSQL.ToString.Replace(vbCrLf, " ")
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return xRisp

    End Function

    '##############################################################################################
    Public Function Modifica(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                ByVal Old_Nome As String, _
                ByVal Old_Revisione As String, _
                ByVal Old_Modello_Codice As String, _
                ByVal Old_Modello_Revisione As String, _
                ByVal New_Nome As String, _
                ByVal New_Revisione As String, _
                ByVal New_DataValiditaInizio As DateTime?, _
                ByVal New_DataValiditaFine As DateTime?, _
                Optional ByVal Data_modifica As Date = #2/1/1900#, _
                Optional ByVal username_modifica As String = "" _
                ) As Boolean


        Dim NomeRoutine As String = "LabControlloQualitaDAL.StandardRev_W.Modifica()"

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
            StrSQL.Append(" UPDATE LCQ_StandardRev SET" + vbCrLf)

            StrSQL.Append("  Nome = " & Agro_SQL_SaveText_NULL(New_Nome) & vbCrLf)
            StrSQL.Append(", Revisione = " & Agro_SQL_SaveText_NULL(New_Revisione) & vbCrLf)
            StrSQL.Append(", DataValiditaInizio = " & Agro_SQL_SaveDateTime_NULL(NothingToDBNull(New_DataValiditaInizio)) & vbCrLf)
            StrSQL.Append(", DataValiditaFine = " & Agro_SQL_SaveDateTime_NULL(NothingToDBNull(New_DataValiditaFine)) & vbCrLf)

            StrSQL.Append(", Data_Modifica = " & Agro_SQL_SaveDateTime(Data_modifica) & vbCrLf)
            StrSQL.Append(", UserName_Modifica = '" & Agro_SQL_SaveText(username_modifica) & "' " & vbCrLf)

            StrSQL.Append("	WHERE PivaSuperUser = " & Agro_SQL_SaveText_NULL(objParametri.PivaSuperUser))
            StrSQL.Append("	AND Nome =		" & Agro_SQL_SaveText_NULL(Old_Nome))
            StrSQL.Append("	AND Revisione =		" & Agro_SQL_SaveText_NULL(Old_Revisione))
            StrSQL.Append("	AND Modello_Codice =		" & Agro_SQL_SaveText_NULL(Old_Modello_Codice))
            StrSQL.Append("	AND Modello_Revisione =		" & Agro_SQL_SaveText_NULL(Old_Modello_Revisione))

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message & " QUERY: " & StrSQL.ToString.Replace(vbCrLf, " ")
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return xRisp

    End Function




    '#################################################################
    Public Function Cancella(ByVal xFiltroAggiuntivo As String, _
                             ByVal Nome As String, _
                             ByVal Revisione As String, _
                             ByVal Modello_Codice As String, _
                             ByVal Modello_Revisione As String, _
                              ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                              ) As Boolean

        '----- Descrizione
        Dim NomeRoutine As String = "LabControlloQualitaDAL.StandardRev_W.Cancella()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            StrSQL.Length = 0

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then
                'StrSQL.Append(" UPDATE ... ")
                'StrSQL.Append(" SET ")
                'StrSQL.Append("         Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                'StrSQL.Append("         ,Data_Modifica= " & Agro_SQL_SaveDate(Date.Now) & " ")
                'StrSQL.Append("         ,Inviato = -1 ")
                'StrSQL.Append(" WHERE   1=1 ")
                'StrSQL.Append(" AND     Inviato >= 0 ")
            Else
                StrSQL.Append(" DELETE FROM LCQ_StandardRev ")
                StrSQL.Append(" WHERE PivaSuperUser = " & Agro_SQL_SaveText_NULL(objParametri.PivaSuperUser))
                StrSQL.Append("	AND Nome =		" & Agro_SQL_SaveText_NULL(Nome))
                StrSQL.Append("	AND Revisione =		" & Agro_SQL_SaveText_NULL(Revisione))
                StrSQL.Append("	AND Modello_Codice =		" & Agro_SQL_SaveText_NULL(Modello_Codice))
                StrSQL.Append("	AND Modello_Revisione =		" & Agro_SQL_SaveText_NULL(Modello_Revisione))
            End If
            '---------------------------------------------

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message & " QUERY: " & StrSQL.ToString.Replace(vbCrLf, " ")
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp

    End Function




End Class
