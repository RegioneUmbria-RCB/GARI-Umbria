Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.AgronicaCoreParametri

Public Class Allegati_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function Leggi_Allegati( _
                                        ByVal ID As Integer?, _
                                        ByVal ID_Lista As Integer?, _
                                        ByVal NomeFile As String, _
                                        ByVal Descrizione As String, _
                                        ByVal ID_Categoria As Integer?, _
                                        ByVal xFiltroAggiuntivo As String, _
                                        ByVal xOrderBy As String, _
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                        ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCorePannelloDiControlloDAL.PnlCtrl_Allegati_R.Leggi_Allegati"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim strSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            strSQL.Length = 0

            strSQL.Append(" SELECT  ID, ID_Lista, NomeFile, Descrizione, ID_Categoria " + vbCrLf)
            strSQL.Append(" FROM PnlCtrl_Allegati ")

            strSQL.Append(" WHERE PivaSuperUser = " & Agro_SQL_SaveText_NULL(objParametri.PivaSuperUser) + vbCrLf)

            If Not IsNothing(ID) Then
                strSQL.Append(" AND ID = " & Agro_SQL_SaveNum_NULL(ID))
            End If

            If Not IsNothing(ID_Lista) Then
                strSQL.Append(" AND ID_Lista = " & Agro_SQL_SaveNum_NULL(ID_Lista))
            End If

            If Not IsNothing(NomeFile) Then
                strSQL.Append(" AND NomeFile = " & Agro_SQL_SaveText_NULL(NomeFile))
            End If

            If Not IsNothing(Descrizione) Then
                strSQL.Append(" AND Descrizione = " & Agro_SQL_SaveText_NULL(Descrizione))
            End If

            If Not IsNothing(ID_Categoria) Then
                strSQL.Append(" AND ID_Categoria = " & Agro_SQL_SaveNum_NULL(ID_Categoria))
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

Public Class Allegati_W
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function Modifica(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                            ByVal Old_ID As Integer, _
                            ByVal New_ID_Lista As Integer, _
                            ByVal New_NomeFile As String, _
                            ByVal New_Descrizione As String, _
                            ByVal New_ID_Categoria As Integer, _
                            Optional ByVal Data_modifica As Date = #2/1/1900#, _
                            Optional ByVal username_modifica As String = "" _
                            ) As Boolean


        Dim NomeRoutine As String = "AgronicaCorePannelloDiControlloDAL.PnlCtrl_Allegati_W.Modifica()"

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
            StrSQL.Append(" UPDATE PnlCtrl_Allegati SET" + vbCrLf)

            StrSQL.Append("	 ID_Lista  =		" & Agro_SQL_SaveNum_NULL(New_ID_Lista))
            StrSQL.Append(", NomeFile = " & Agro_SQL_SaveText_NULL(New_NomeFile) & vbCrLf)
            StrSQL.Append(", Descrizione = " & Agro_SQL_SaveText_NULL(New_Descrizione) & vbCrLf)
            StrSQL.Append(", ID_Categoria = " & Agro_SQL_SaveNum_NULL(New_ID_Categoria) & vbCrLf)

            StrSQL.Append(", Data_Modifica = " & Agro_SQL_SaveDateTime(Data_modifica) & vbCrLf)
            StrSQL.Append(", UserName_Modifica = '" & Agro_SQL_SaveText(username_modifica) & "' " & vbCrLf)

            StrSQL.Append("	WHERE PivaSuperUser = " & Agro_SQL_SaveText_NULL(objParametri.PivaSuperUser))
            StrSQL.Append("	AND ID  =		" & Agro_SQL_SaveNum_NULL(Old_ID))



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
    Public Function Scrivi(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                            ByVal ID As Integer, _
                            ByVal ID_Lista As Integer, _
                            ByVal NomeFile As String, _
                            ByVal Descrizione As String, _
                            ByVal ID_Categoria As Integer, _
                            Optional ByVal Data_creazione As Date = #2/1/1900#, _
                            Optional ByVal Data_modifica As Date = #2/1/1900#, _
                            Optional ByVal username_creazione As String = "", _
                            Optional ByVal username_modifica As String = "" _
                            ) As Boolean


        Dim NomeRoutine As String = "AgronicaCorePannelloDiControlloDAL.PnlCtrl_Allegati_W.Scrivi()"

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
            StrSQL.Append(" INSERT INTO  PnlCtrl_Allegati" + vbCrLf)

            StrSQL.Append("              (")
            StrSQL.Append("              PivaSuperUser, ")
            StrSQL.Append("              ID,                            ID_Lista, ")
            StrSQL.Append("              NomeFile,                          Descrizione, ")
            StrSQL.Append("              ID_Categoria, ")

            StrSQL.Append("              Inviato,            datainvio, ")
            StrSQL.Append("              Data_Creazione,     Data_Modifica, ")
            StrSQL.Append("              UserName_Creazione, UserName_Modifica, ")
            StrSQL.Append("              Validita_Inizio,    Validita_Fine " + vbCrLf)
            StrSQL.Append("              ) ")

            StrSQL.Append(" VALUES ( ")

            StrSQL.Append("			 " & Agro_SQL_SaveText_NULL(objParametri.PivaSuperUser))
            StrSQL.Append("			," & Agro_SQL_SaveNum_NULL(ID))
            StrSQL.Append("			," & Agro_SQL_SaveNum_NULL(ID_Lista))
            StrSQL.Append("			," & Agro_SQL_SaveText_NULL(NomeFile))
            StrSQL.Append("			," & Agro_SQL_SaveText_NULL(Descrizione))
            StrSQL.Append("			," & Agro_SQL_SaveNum_NULL(ID_Categoria))

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


    '#################################################################
    'Public Function Cancella(ByVal xFiltroAggiuntivo As String, _
    '                         ByVal Parametri_Cod As Integer, _
    '                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
    '                          ) As Boolean

    '    '----- Descrizione
    '    Dim NomeRoutine As String = "AgronicaCorePannelloDiControlloDAL.PnlCtrl_Elementi_W.Cancella()"

    '    Dim MessaggioErrore As String = ""
    '    Dim StrSQL As New System.Text.StringBuilder
    '    Dim xRisp As Boolean = False

    '    Try
    '        '---------------------------------------------
    '        StrSQL.Length = 0

    '        '---------------------------------------------
    '        If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then
    '            'StrSQL.Append(" UPDATE ... ")
    '            'StrSQL.Append(" SET ")
    '            'StrSQL.Append("         Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
    '            'StrSQL.Append("         ,Data_Modifica= " & Agro_SQL_SaveDate(Date.Now) & " ")
    '            'StrSQL.Append("         ,Inviato = -1 ")
    '            'StrSQL.Append(" WHERE   1=1 ")
    '            'StrSQL.Append(" AND     Inviato >= 0 ")
    '        Else
    '            StrSQL.Append(" DELETE FROM LCQ_Parametri ")
    '            StrSQL.Append(" WHERE PivaSuperUser = " & Agro_SQL_SaveText_NULL(objParametri.PivaSuperUser))
    '            StrSQL.Append("	AND Parametri_Cod =		" & Agro_SQL_SaveNum_NULL(Parametri_Cod))
    '        End If
    '        '---------------------------------------------

    '        If xFiltroAggiuntivo <> "" Then
    '            StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
    '        End If

    '        '--------------------------------------------------------------------------
    '        xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
    '        '--------------------------------------------------------------------------

    '    Catch ex As Exception
    '        MessaggioErrore = ex.Message & " QUERY: " & StrSQL.ToString.Replace(vbCrLf, " ")
    '        Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
    '        xRisp = False
    '        Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
    '    End Try

    '    Return xRisp

    'End Function

End Class
