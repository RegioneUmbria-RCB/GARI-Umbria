
Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.AgronicaCoreParametri

Public Class Linee_Produzioni_Parametri_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function Leggi( _
                         ByVal piva As String, _
                         ByVal xFiltroAggiuntivo As String, _
                                        ByVal xOrderBy As String, _
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                        ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCore_DAL.Leggi()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim strSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            strSQL.Length = 0

            strSQL.Append(" SELECT * from Linee_Produzioni_Parametri " + vbCrLf)

            strSQL.Append(" WHERE Piva = '" + Agro_SQL_SaveText(piva) + "' " + vbCrLf)


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
            MessaggioErrore = ex.Message
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

Public Class Linee_Produzioni_Parametri_W
    Inherits AgronicaCoreDataProvider.DataProvider



    '##############################################################################################
    Public Function Scrivi( _
                  ByVal Piva_SuperUser As String _
                , ByVal Piva As String _
                , ByVal Linea_Cod As Integer _
                , ByVal Linea_Cod_Par As Integer _
                , ByVal Tipo As Integer _
                , ByVal Tipo_Cod As Integer _
                , ByVal Elem_Cod As Integer _
                , ByVal Pro_Cod As Integer _
                , ByVal Mat_Cod As Integer _
                , ByVal Udm_Cod As Integer _
                , ByVal Valore_Min As String _
                , ByVal Valore_Max As String _
                , ByVal Note As String _
                , ByVal Modulo_Generazione As Integer _
                , ByVal Tipo_Generazione As Integer _
                , ByVal Codice_Generazione As Integer _
                , ByVal Validita_inizio As Date _
                , ByVal validita_fine As Date _
                , ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                , Optional ByVal Data_creazione As DateTime = #2/1/1900# _
                , Optional ByVal Data_modifica As DateTime = #2/1/1900# _
                , Optional ByVal username_creazione As String = "" _
                , Optional ByVal username_modifica As String = "" _
                ) As Boolean


        Dim NomeRoutine As String = "Scrivi()"

        '====================================================================================
        'Parametri opzionali :

        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If Data_creazione = #2/1/1900# Then
                Data_creazione = Date.Now
            End If

            If Data_modifica = #2/1/1900# Then
                Data_modifica = Date.Now
            End If

            If username_creazione = "" Then
                username_creazione = objParametri.UsernameOperazione
            End If

            If username_modifica = "" Then
                username_modifica = objParametri.UsernameOperazione
            End If



            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append(" INSERT  Linee_Produzioni_Parametri " + vbCrLf)

            StrSQL.Append("              (")

            StrSQL.Append("   [Piva_SuperUser] " & vbCrLf)
            StrSQL.Append("  ,[Piva] " & vbCrLf)
            StrSQL.Append("  ,[Linea_Cod] " & vbCrLf)
            StrSQL.Append("  ,[Linea_Cod_Par] " & vbCrLf)
            StrSQL.Append("  ,[Tipo] " & vbCrLf)
            StrSQL.Append("  ,[Tipo_Cod] " & vbCrLf)
            StrSQL.Append("  ,[Elem_Cod] " & vbCrLf)
            StrSQL.Append("  ,[Pro_Cod] " & vbCrLf)
            StrSQL.Append("  ,[Mat_Cod] " & vbCrLf)
            StrSQL.Append("  ,[Udm_Cod] " & vbCrLf)
            StrSQL.Append("  ,[Valore_Min] " & vbCrLf)
            StrSQL.Append("  ,[Valore_Max] " & vbCrLf)
            StrSQL.Append("  ,[Note] " & vbCrLf)
            StrSQL.Append("  ,[Modulo_Generazione] " & vbCrLf)
            StrSQL.Append("  ,[Tipo_Generazione] " & vbCrLf)
            StrSQL.Append("  ,[Codice_Generazione], " & vbCrLf)


            StrSQL.Append("              Inviato,            datainvio, ")
            StrSQL.Append("              Data_Creazione,     Data_Modifica, ")
            StrSQL.Append("              UserName_Creazione, UserName_Modifica, ")
            StrSQL.Append("              Validita_Inizio,    Validita_Fine ")

            StrSQL.Append("              ) ")

            StrSQL.Append(" VALUES ( ")

            StrSQL.Append(",'" & Agro_SQL_SaveText(Piva_SuperUser) & "'" & vbCrLf)
            StrSQL.Append(",'" & Agro_SQL_SaveText(Piva) & "'" & vbCrLf)
            StrSQL.Append(", " & Agro_SQL_SaveNum(Linea_Cod) & " " & vbCrLf)
            StrSQL.Append(", " & Agro_SQL_SaveNum(Linea_Cod_Par) & " " & vbCrLf)
            StrSQL.Append(", " & Agro_SQL_SaveNum(Tipo) & " " & vbCrLf)
            StrSQL.Append(", " & Agro_SQL_SaveNum(Tipo_Cod) & " " & vbCrLf)
            StrSQL.Append(", " & Agro_SQL_SaveNum(Elem_Cod) & " " & vbCrLf)
            StrSQL.Append(", " & Agro_SQL_SaveNum(Pro_Cod) & " " & vbCrLf)
            StrSQL.Append(", " & Agro_SQL_SaveNum(Mat_Cod) & " " & vbCrLf)
            StrSQL.Append(", " & Agro_SQL_SaveNum(Udm_Cod) & " " & vbCrLf)
            StrSQL.Append(",'" & Agro_SQL_SaveText(Valore_Min) & "'" & vbCrLf)
            StrSQL.Append(",'" & Agro_SQL_SaveText(Valore_Max) & "'" & vbCrLf)
            StrSQL.Append(",'" & Agro_SQL_SaveText(Note) & "'" & vbCrLf)
            StrSQL.Append(", " & Agro_SQL_SaveNum(Modulo_Generazione) & " " & vbCrLf)
            StrSQL.Append(", " & Agro_SQL_SaveNum(Tipo_Generazione) & " " & vbCrLf)
            StrSQL.Append(", " & Agro_SQL_SaveNum(Codice_Generazione) & " " & vbCrLf)


            StrSQL.Append("         , 0  " + vbCrLf)
            StrSQL.Append("         , Null  " + vbCrLf)

            StrSQL.Append("			, " & Agro_SQL_SaveDateTime(Data_creazione) & "  ")
            StrSQL.Append("			, " & Agro_SQL_SaveDateTime(Data_modifica) & "  ")
            StrSQL.Append("			,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            StrSQL.Append("			,'" & Agro_SQL_SaveText(username_modifica) & "' ")
            StrSQL.Append("			, " & Agro_SQL_SaveDate(Validita_inizio) & "  ")
            StrSQL.Append("			, " & Agro_SQL_SaveDate(validita_fine) & "  ")






            StrSQL.Append(") ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return xRisp

    End Function







    '#################################################################
    Public Function Cancella(ByVal xFiltroAggiuntivo As String, _
                              ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                              ) As Boolean

        '----- Descrizione
        Dim NomeRoutine As String = "Cancella()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            StrSQL.Length = 0

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then
                StrSQL.Append(" UPDATE ... ")
                StrSQL.Append(" SET ")
                StrSQL.Append("         Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("         ,Data_Modifica= " & Agro_SQL_SaveDate(Date.Now) & " ")
                StrSQL.Append("         ,Inviato = -1 ")
                StrSQL.Append(" WHERE   1=1 ")
                StrSQL.Append(" AND     Inviato >= 0 ")
            Else
                StrSQL.Append(" DELETE FROM ... ")
                StrSQL.Append(" WHERE 1=1 ")
            End If
            '---------------------------------------------

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp

    End Function




End Class


