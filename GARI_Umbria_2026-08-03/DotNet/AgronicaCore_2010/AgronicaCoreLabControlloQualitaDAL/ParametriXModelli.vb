Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.AgronicaCoreParametri

Public Class ParametriXModelli_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function Leggi_ParametriXModelli( _
                                        ByVal ParametriXModelli_Cod As Integer?, _
                                        ByVal Modello_Codice As String, _
                                        ByVal Modello_Revisione As String, _
                                        ByVal Parametro_Cod As Integer?, _
                                        ByVal OrdineSequenza As Integer?, _
                                        ByVal xFiltroAggiuntivo As String, _
                                        ByVal xOrderBy As String, _
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                        ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "LabControlloQualitaDAL.ParametriXModelli_R.Leggi_ParametriXModelli"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim strSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            strSQL.Length = 0

            strSQL.Append(" SELECT PivaSuperUser, ParametriXModelli_Cod, Modello_Codice, Modello_Revisione, Parametro_Cod, OrdineSequenza " + vbCrLf)
            strSQL.Append(" FROM LCQ_ParametriXModelli ")

            strSQL.Append(" WHERE PivaSuperUser = " & Agro_SQL_SaveText_NULL(objParametri.PivaSuperUser) + vbCrLf)

            If Not IsNothing(ParametriXModelli_Cod) Then
                strSQL.Append(" AND ParametriXModelli_Cod = " & Agro_SQL_SaveNum_NULL(ParametriXModelli_Cod))
            End If

            If Not IsNothing(Modello_Codice) Then
                strSQL.Append(" AND Modello_Codice = " & Agro_SQL_SaveText_NULL(Modello_Codice))
            End If

            If Not IsNothing(Modello_Revisione) Then
                strSQL.Append(" AND Modello_Revisione = " & Agro_SQL_SaveText_NULL(Modello_Revisione))
            End If

            If Not IsNothing(Parametro_Cod) Then
                strSQL.Append(" AND Parametro_Cod = " & Agro_SQL_SaveNum_NULL(Parametro_Cod))
            End If

            If Not IsNothing(OrdineSequenza) Then
                strSQL.Append(" AND OrdineSequenza = " & Agro_SQL_SaveNum_NULL(OrdineSequenza))
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

Public Class ParametriXModelli_W
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function Scrivi(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                            ByVal ParametriXModelli_Cod As Integer, _
                            ByVal Modello_Codice As String, _
                            ByVal Modello_Revisione As String, _
                            ByVal Parametro_Cod As Integer, _
                            ByVal OrdineSequenza As Integer, _
                            Optional ByVal Data_creazione As Date = #2/1/1900#, _
                            Optional ByVal Data_modifica As Date = #2/1/1900#, _
                            Optional ByVal username_creazione As String = "", _
                            Optional ByVal username_modifica As String = "" _
                            ) As Boolean


        Dim NomeRoutine As String = "LabControlloQualitaDAL.Parametri_W.Leggi_ParametriXModelli()"

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
            StrSQL.Append(" INSERT INTO  LCQ_ParametriXModelli" + vbCrLf)

            StrSQL.Append("              (")
            StrSQL.Append("              PivaSuperUser,                 ParametriXModelli_Cod , ")
            StrSQL.Append("              Modello_Codice ,               Modello_Revisione , ")
            StrSQL.Append("              Parametro_Cod ,                OrdineSequenza,  ")

            StrSQL.Append("              Inviato,            datainvio, ")
            StrSQL.Append("              Data_Creazione,     Data_Modifica, ")
            StrSQL.Append("              UserName_Creazione, UserName_Modifica, ")
            StrSQL.Append("              Validita_Inizio,    Validita_Fine " + vbCrLf)
            StrSQL.Append("              ) ")

            StrSQL.Append(" VALUES ( ")

            StrSQL.Append("			 " & Agro_SQL_SaveText_NULL(objParametri.PivaSuperUser))
            StrSQL.Append("			," & Agro_SQL_SaveNum_NULL(ParametriXModelli_Cod))
            StrSQL.Append("			," & Agro_SQL_SaveText_NULL(Modello_Codice))
            StrSQL.Append("			," & Agro_SQL_SaveText_NULL(Modello_Revisione))
            StrSQL.Append("			," & Agro_SQL_SaveNum_NULL(Parametro_Cod))
            StrSQL.Append("			," & Agro_SQL_SaveNum_NULL(OrdineSequenza))


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
                ByVal Old_ParametriXModelli_Cod As Integer, _
                ByVal New_OrdineSequenza As Integer, _
                Optional ByVal Data_modifica As Date = #2/1/1900#, _
                Optional ByVal username_modifica As String = "" _
                ) As Boolean


        Dim NomeRoutine As String = "LabControlloQualitaDAL.ParametriXModelli_W.Modifica()"

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
            StrSQL.Append(" UPDATE LCQ_ParametriXModelli SET" + vbCrLf)

            StrSQL.Append(" OrdineSequenza = " & Agro_SQL_SaveText_NULL(NothingToDBNull(New_OrdineSequenza)) & vbCrLf)

            StrSQL.Append(", Data_Modifica = " & Agro_SQL_SaveDateTime(Data_modifica) & vbCrLf)
            StrSQL.Append(", UserName_Modifica = '" & Agro_SQL_SaveText(username_modifica) & "' " & vbCrLf)

            StrSQL.Append("	WHERE PivaSuperUser = " & Agro_SQL_SaveText_NULL(objParametri.PivaSuperUser))
            StrSQL.Append("	AND ParametriXModelli_Cod  =		" & Agro_SQL_SaveNum_NULL(Old_ParametriXModelli_Cod))

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
    Public Function Cancella(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                            ByVal ParametriXModelli_Cod As Integer, _
                            ByVal xFiltroAggiuntivo As String _
                              ) As Boolean

        '----- Descrizione
        Dim NomeRoutine As String = "LabControlloQualitaDAL.ParametriXModelli_W.Cancella()"

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
                StrSQL.Append(" DELETE FROM LCQ_ParametriXModelli ")
                StrSQL.Append(" WHERE PivaSuperUser = " & Agro_SQL_SaveText_NULL(objParametri.PivaSuperUser))
                StrSQL.Append("	AND ParametriXModelli_Cod =		" & Agro_SQL_SaveNum_NULL(ParametriXModelli_Cod))
            End If
            '---------------------------------------------

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

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

End Class
