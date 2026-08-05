Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate

Public Class Log_AgroFascicolo_WS_R
    Inherits AgronicaCoreDataProvider.DataProvider

End Class


Public Class Log_AgroFascicolo_WS_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function ScriviLog(PivaSuperUser As String,
                              Username As String,
                              EnteValidatore_Cod As Integer,
                              CUAA As String,
                              Parametri_Extra As String,
                              objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAgroFascicoloDAL.Log_AgroFascicolo_WS_W.ScriviLog()"

        '====================================================================================
        'Parametri opzionali :
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            Stb.AppendLine("             INSERT INTO [dbo].[Log_AgroFascicolo_WS] ")
            Stb.AppendLine("            ([PivaSuperUser] ")
            Stb.AppendLine("            ,[Username] ")
            Stb.AppendLine("            ,[EnteValidatore_Cod] ")
            Stb.AppendLine("            ,[CUAA] ")
            Stb.AppendLine("            ,[Parametri_Extra] ")
            Stb.AppendLine("            ,[Data_Richiesta]) ")
            Stb.AppendLine("      VALUES ")
            Stb.AppendLine("            ( " & Agro_SQL_SaveText_NULL(PivaSuperUser) & " ")
            Stb.AppendLine("            , " & Agro_SQL_SaveText_NULL(Username) & " ")
            Stb.AppendLine("            , " & Agro_SQL_SaveNum(EnteValidatore_Cod) & " ")
            Stb.AppendLine("            , " & Agro_SQL_SaveText_NULL(CUAA) & " ")
            Stb.AppendLine("            , " & Agro_SQL_SaveText_NULL(Parametri_Extra) & " ")
            Stb.AppendLine("            , " & Agro_SQL_SaveDateTime_NULL(DateTime.Now) & " ) ")

            '---------------------------------------------


            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, Stb.ToString, NomeRoutine)
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