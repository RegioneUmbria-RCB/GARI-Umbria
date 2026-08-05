Imports System.Data.Common
Imports AgronicaCoreDataProvider.UtilityProvider

Public Class SE_LavorazionixMacrovoci_R
    Inherits AgronicaCoreDataProvider.DataProvider



    '##############################################################################################
    Public Function Leggi(
                      ByVal Macrovoce_Cod As Int32,
                      ByVal Lav_Cod As Int32,
                      ByVal FinestraTemp_Inizio As Date,
                      ByVal FinestraTemp_Fine As Date,
                      ByRef objConnessione As DbConnection,
                      ByRef objTransazione As DbTransaction,
                      ByVal StringaConnessione As String,
                      ByVal FlagVisibilita As Int32,
                      ByVal DirectoryLOG As String,
                      ByVal FileLOG As String,
                      ByVal IdentificatoreUtente As String
                      ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.SE_LavorazionixMacrovoci_R.Leggi()"

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
            StrSQL.Append(" FROM  SE_LavorazionixMacrovoci ")
            StrSQL.Append(" WHERE Validita_inizio <= " & Agro_SQL_SaveDate(FinestraTemp_Fine) & " ")
            StrSQL.Append(" AND   Validita_Fine >= " & Agro_SQL_SaveDate(FinestraTemp_Inizio) & " ")


            If Macrovoce_Cod <> 0 Then
                StrSQL.Append(" AND Macrovoce_Cod = " & Agro_SQL_SaveNum(Macrovoce_Cod) & " ")
            End If

            If Lav_Cod <> 0 Then
                StrSQL.Append(" AND Lav_Cod = " & Agro_SQL_SaveNum(Lav_Cod) & " ")
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

            StrSQL.Append(" ORDER BY Macrovoce_Cod, Lav_Cod ASC ")

            '------------------------------------------------------------------

            DT = EseguiQuery_Lettura(objConnessione, objTransazione, StringaConnessione, StrSQL.ToString, DirectoryLOG, FileLOG, IdentificatoreUtente, NomeRoutine)

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(DirectoryLOG, FileLOG, IdentificatoreUtente, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return DT

    End Function



End Class
