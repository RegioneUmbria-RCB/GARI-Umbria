Imports System.Data.Common
Imports AgronicaCoreDataProvider.UtilityProvider

Public Class SE_SpecieVegetalixCosti_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function Leggi(
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
                            ) As DataTable




        Dim NomeRoutine As String = "AnagrafeDAL.SE_SpecieVegetalixCosti_R.Leggi()"

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

        Try

            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append(" SELECT SpecieVegetali.* ")
            StrSQL.Append(" FROM  SE_SpecieVegetalixCosti, SpecieVegetali ")

            StrSQL.Append(" WHERE   SE_SpecieVegetalixCosti.Validita_inizio <= " & Agro_SQL_SaveDate(FinestraTemp_Fine) & " ")
            StrSQL.Append(" AND     SE_SpecieVegetalixCosti.Validita_Fine >= " & Agro_SQL_SaveDate(FinestraTemp_Inizio) & " ")
            StrSQL.Append(" AND     SE_SpecieVegetalixCosti.Veg_Cod = SpecieVegetali.Veg_Cod ")
            If Veg_Cod <> 0 Then
                StrSQL.Append(" AND Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod) & " ")
            End If

            Select Case FlagVisibilita
                Case 1  'Solo i NON CANCELLATI
                    StrSQL.Append(" AND   SE_SpecieVegetalixCosti.Inviato >=0 ")
                Case 2  'Solo i CANCELLATI
                    StrSQL.Append(" AND   SE_SpecieVegetalixCosti.Inviato =-1 ")
                Case 3  'TUTTI
                    '
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '---------------------------------------------

            StrSQL.Append(" ORDER BY SpecieVegetali.Veg_Des ASC ")

            '------------------------------

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
