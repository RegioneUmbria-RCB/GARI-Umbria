Imports System.Data.Common
Imports AgronicaCoreDataProvider.UtilityProvider

Public Class SE_Macrovoci_R
    Inherits AgronicaCoreDataProvider.DataProvider



    '##############################################################################################
    Public Function Leggi(
                      ByVal Piva_SuperUser As String,
                      ByVal Piva As String,
                      ByVal Macrovoce_Cod As Int32,
                      ByVal Veg_Cod As Int32,
                      ByVal FinestraTemp_Inizio As Date,
                      ByVal FinestraTemp_Fine As Date,
                      ByRef objConnessione As DbConnection,
                      ByVal StringaConnessione As String,
                      ByVal FlagVisibilita As Int32,
                      ByVal DirectoryLOG As String,
                      ByVal FileLOG As String,
                      ByVal IdentificatoreUtente As String
                      ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.SE_Macrovoci_R.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   Macrovoce_Cod = 0
        '
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            '------------------------------------------------------------------
            StrSQL.Length = 0

            StrSQL.Append(" SELECT * ")
            StrSQL.Append(" FROM  SE_Macrovoci ")
            StrSQL.Append(" WHERE Validita_Inizio = " & Agro_SQL_SaveDate(FinestraTemp_Inizio) & "  ")


            If Piva_SuperUser <> "" Then
                StrSQL.Append(" AND Piva_SuperUser = '" & Agro_SQL_SaveText(Piva_SuperUser) & "' ")
            End If

            If Piva <> "" Then
                StrSQL.Append(" AND Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If

            If Macrovoce_Cod <> 0 Then
                StrSQL.Append(" AND Macrovoce_Cod = " & Agro_SQL_SaveNum(Macrovoce_Cod) & " ")
            End If

            If Veg_Cod <> 0 Then
                StrSQL.Append(" AND Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod) & " ")
            End If


            Select Case FlagVisibilita
                Case 1  'Solo i NON CANCELLATI
                    StrSQL.Append(" AND   Inviato >=0 ")
                Case 2  'Solo i CANCELLATI
                    StrSQL.Append(" AND   Inviato =-1 ")
                Case 3  'TUTTI
                    '
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select

            StrSQL.Append(" ORDER BY Piva_SuperUser,Piva,Macrovoce_Cod, Veg_Cod ASC ")

            '------------------------------------------------------------------

            DT = EseguiQuery_Lettura(objConnessione, StringaConnessione, StrSQL.ToString, DirectoryLOG, FileLOG, IdentificatoreUtente, NomeRoutine)

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(DirectoryLOG, FileLOG, IdentificatoreUtente, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return DT

    End Function




    '##############################################################################################
    Public Function Esiste_Macrovoce(
                                        ByVal Piva_SuperUser As String,
                                        ByVal Piva As String,
                                        ByVal Macrovoce_Cod As Int32,
                                        ByVal Veg_Cod As Int32,
                                        ByVal FinestraTemp_Inizio As Date,
                                        ByVal FinestraTemp_Fine As Date,
                                        ByRef objConnessione As DbConnection,
                                        ByRef objTransazione As DbTransaction,
                                        ByVal StringaConnessione As String,
                                        ByVal FlagVisibilita As Int32,
                                        ByVal DirectoryLOG As String,
                                        ByVal FileLOG As String,
                                        ByVal IdentificatoreUtente As String
                                        ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.SE_Macrovoci_R.Esiste_Macrovoce()"


        '====================================================================================
        'Parametri opzionali :
        '
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Dim bRet As Boolean = False

        Try
            If Piva_SuperUser = "" Then
                Throw New Exception("Parametro non corretto nella query (PivaSuperUser obbligatoria)")
            End If

            If Piva = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva obbligatoria)")
            End If

            If Macrovoce_Cod = 0 Then
                Throw New Exception("Parametro non corretto nella query (Macrovoce_Cod obbligatorio)")
            End If

            If Veg_Cod = 0 Then
                Throw New Exception("Parametro non corretto nella query (Veg_Cod obbligatoria)")
            End If

            '------------------------------------------------------------------
            StrSQL.Length = 0

            StrSQL.Append(" SELECT * ")
            StrSQL.Append(" FROM  SE_Macrovoci ")
            StrSQL.Append(" WHERE Validita_Inizio = " & Agro_SQL_SaveDate(FinestraTemp_Inizio) & "  ")


            If Piva_SuperUser <> "" Then
                StrSQL.Append(" AND Piva_SuperUser = '" & Agro_SQL_SaveText(Piva_SuperUser) & "' ")
            End If

            If Piva <> "" Then
                StrSQL.Append(" AND Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If

            If Macrovoce_Cod <> 0 Then
                StrSQL.Append(" AND Macrovoce_Cod = " & Agro_SQL_SaveNum(Macrovoce_Cod) & " ")
            End If

            If Veg_Cod <> 0 Then
                StrSQL.Append(" AND Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod) & " ")
            End If


            Select Case FlagVisibilita
                Case 1  'Solo i NON CANCELLATI
                    StrSQL.Append(" AND   Inviato >=0 ")
                Case 2  'Solo i CANCELLATI
                    StrSQL.Append(" AND   Inviato =-1 ")
                Case 3  'TUTTI
                    '
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '---------------------------------------------

            DT = EseguiQuery_Lettura(objConnessione, objTransazione, StringaConnessione, StrSQL.ToString, DirectoryLOG, FileLOG, IdentificatoreUtente, NomeRoutine)

            If Not DT Is Nothing Then
                If DT.Rows.Count > 0 Then
                    bRet = True
                Else
                    bRet = False
                End If
            End If

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(DirectoryLOG, FileLOG, IdentificatoreUtente, NomeRoutine, MessaggioErrore)
            DT = Nothing
            bRet = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        DT = Nothing
        Return bRet

    End Function

End Class
