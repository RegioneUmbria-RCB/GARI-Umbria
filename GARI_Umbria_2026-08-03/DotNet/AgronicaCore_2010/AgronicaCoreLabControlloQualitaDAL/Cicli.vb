Imports System.Data.OleDb
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.AgronicaCoreParametri

Public Class Cicli_R
    Inherits AgronicaCoreDataProvider.DataProvider



    '##############################################################################################
    Public Function Leggi_Cicli( _
                               ByVal Nome As String, _
                               ByVal xFiltroAggiuntivo As String, _
                               ByVal xOrderBy As String, _
                               ByRef objParametri As AgronicaCoreParametri _
                               ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "LabControlloQualitaDAL.Cicli_R.Leggi_Cicli()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim strSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            strSQL.Length = 0

            strSQL.Append(" SELECT PivaSuperUser, Nome" + vbCrLf)
            strSQL.Append(" FROM LCQ_Cicli" + vbCrLf)
            strSQL.Append(" WHERE PivaSuperUser = " & Agro_SQL_SaveText_NULL(objParametri.PivaSuperUser) + vbCrLf)

            If Not IsNothing(Nome) Then
                strSQL.Append(" AND Nome = " & Agro_SQL_SaveText_NULL(Nome))
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

End Class


'#################################################################
'#################################################################
'#################################################################

Public Class Cicli_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Modifica(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                            ByVal Old_Nome As String, _
                            ByVal New_Nome As String, _
                            Optional ByVal Data_modifica As Date = #2/1/1900#, _
                            Optional ByVal username_modifica As String = "" _
                            ) As Boolean


        Dim NomeRoutine As String = "LabControlloQualitaDAL.Cicli_W.ModificaCiclo()"

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
            StrSQL.Append(" UPDATE LCQ_Cicli SET" + vbCrLf)

            StrSQL.Append("  Nome = " & Agro_SQL_SaveText_NULL(New_Nome) & vbCrLf)

            StrSQL.Append(", Data_Modifica = " & Agro_SQL_SaveDateTime(Data_modifica) & vbCrLf)
            StrSQL.Append(", UserName_Modifica = '" & Agro_SQL_SaveText(username_modifica) & "' " & vbCrLf)

            StrSQL.Append("	WHERE PivaSuperUser = " & Agro_SQL_SaveText_NULL(objParametri.PivaSuperUser))
            StrSQL.Append("	AND Nome =		" & Agro_SQL_SaveText_NULL(Old_Nome))

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
    'Public Function Scrivi(ByRef objParametri As AgronicaCoreParametri, _
    '                                    Optional ByVal Data_creazione As Date = #2/1/1900#, _
    '                                    Optional ByVal Data_modifica As Date = #2/1/1900#, _
    '                                    Optional ByVal username_creazione As String = "", _
    '                                    Optional ByVal username_modifica As String = "" _
    '                                    ) As Boolean


    '    Dim NomeRoutine As String = "Scrivi()"

    '    '====================================================================================
    '    'Parametri opzionali :

    '    '====================================================================================

    '    Dim MessaggioErrore As String = ""
    '    Dim StrSQL As New System.Text.StringBuilder
    '    Dim xRisp As Boolean = False

    '    Try

    '        If Data_creazione = #2/1/1900# Then
    '            Data_creazione = DateTime.Now
    '        End If

    '        If Data_modifica = #2/1/1900# Then
    '            Data_modifica = DateTime.Now
    '        End If

    '        If username_creazione = "" Then
    '            username_creazione = objParametri.UsernameOperazione
    '        End If

    '        If username_modifica = "" Then
    '            username_modifica = objParametri.UsernameOperazione
    '        End If



    '        '---------------------------------------------
    '        StrSQL.Length = 0
    '        StrSQL.Append(" INSERT ... " + vbCrLf)

    '        StrSQL.Append("              (")
    '        StrSQL.Append("              Inviato,            datainvio, ")
    '        StrSQL.Append("              Data_Creazione,     Data_Modifica, ")
    '        StrSQL.Append("              UserName_Creazione, UserName_Modifica, ")
    '        StrSQL.Append("              Validita_Inizio,    Validita_Fine, ")
    '        StrSQL.Append("              Validazione, Data_Validazione, UserName_Validazione " + vbCrLf)
    '        StrSQL.Append("              ) ")

    '        StrSQL.Append(" VALUES ( ")



    '        StrSQL.Append("         , 0  " + vbCrLf)
    '        StrSQL.Append("         , Null  " + vbCrLf)

    '        StrSQL.Append("			, " & Agro_SQL_SaveDate(Data_creazione) & "  ")
    '        StrSQL.Append("			, " & Agro_SQL_SaveDate(Data_modifica) & "  ")
    '        StrSQL.Append("			,'" & Agro_SQL_SaveText(username_creazione) & "' ")
    '        StrSQL.Append("			,'" & Agro_SQL_SaveText(username_modifica) & "' ")



    '        StrSQL.Append(") ")

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


