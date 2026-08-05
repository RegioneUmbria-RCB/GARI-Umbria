Imports System.Data.Common
Imports AgronicaCoreDataProvider.UtilityProvider


Public Class SE_MacrovociDefault_R
    Inherits AgronicaCoreDataProvider.DataProvider



    '##############################################################################################
    Public Function Leggi(
                        ByVal Piva_SuperUser As String,
                        ByVal Elemento_Cod As Int32,
                        ByVal FinestraTemp_Inizio As Date,
                        ByVal FinestraTemp_Fine As Date,
                        ByRef objConnessione As DbConnection,
                        ByVal StringaConnessione As String,
                        ByVal FlagVisibilita As Int32,
                        ByVal DirectoryLOG As String,
                        ByVal FileLOG As String,
                        ByVal IdentificatoreUtente As String
                        ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.SE_MacrovociDefault_R.Leggi()"

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
            StrSQL.Append(" FROM  SE_MacrovociDefault ")

            StrSQL.Append(" WHERE Validita_Inizio <= " & Agro_SQL_SaveDate(FinestraTemp_Fine) & "  ")
            StrSQL.Append(" AND   Validita_Fine >= " & Agro_SQL_SaveDate(FinestraTemp_Inizio) & "  ")


            If Piva_SuperUser <> "" Then
                StrSQL.Append(" AND Piva_SuperUser = '" & Agro_SQL_SaveText(Piva_SuperUser) & "' ")
            End If

            If Elemento_Cod <> 0 Then
                StrSQL.Append(" AND Macrovoce_Cod = " & Agro_SQL_SaveNum(Elemento_Cod) & " ")
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

            StrSQL.Append(" ORDER BY Piva_SuperUser, Macrovoce_Cod ASC ")

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


End Class
