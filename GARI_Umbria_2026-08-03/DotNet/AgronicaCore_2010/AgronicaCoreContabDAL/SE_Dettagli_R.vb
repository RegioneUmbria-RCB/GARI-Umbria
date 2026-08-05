Imports System.Data.Common
Imports AgronicaCoreDataProvider.UtilityProvider

Public Class SE_Dettagli_R
    Inherits AgronicaCoreDataProvider.DataProvider



    '##############################################################################################
    Public Function Leggi(
                      ByVal Piva_SuperUser As String,
                      ByVal Piva As String,
                      ByVal Macrovoce_Cod As Int32,
                      ByVal Veg_Cod As Int32,
                      ByVal Operazione_Cod As Int32,
                      ByVal Lav_Cod As Int32,
                      ByVal Dettaglio_Cod As Int32,
                      ByVal FinestraTemp_Inizio As Date,
                      ByVal FinestraTemp_Fine As Date,
                      ByRef objConnessione As DbConnection,
                      ByVal StringaConnessione As String,
                      ByVal FlagVisibilita As Int32,
                      ByVal DirectoryLOG As String,
                      ByVal FileLOG As String,
                      ByVal IdentificatoreUtente As String
                      ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.SE_Dettagli_R.Leggi()"

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
            StrSQL.Append(" FROM  SE_Dettagli ")
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

            If Operazione_Cod <> 0 Then
                StrSQL.Append(" AND Operazione_Cod = " & Agro_SQL_SaveNum(Operazione_Cod) & " ")
            End If

            If Lav_Cod <> 0 Then
                StrSQL.Append(" AND Lav_Cod = " & Agro_SQL_SaveNum(Lav_Cod) & " ")
            End If

            If Dettaglio_Cod <> 0 Then
                StrSQL.Append(" AND Dettaglio_Cod = " & Agro_SQL_SaveNum(Dettaglio_Cod) & " ")
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

            StrSQL.Append(" ORDER BY Piva_SuperUser,Piva, Macrovoce_Cod, Veg_Cod, Operazione_Cod, Lav_Cod, Validita_Inizio ASC ")

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
