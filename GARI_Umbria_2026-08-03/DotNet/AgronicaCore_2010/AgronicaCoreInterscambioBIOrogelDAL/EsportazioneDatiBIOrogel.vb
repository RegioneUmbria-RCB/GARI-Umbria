Imports System.Data
Imports System.Text
Imports AgronicaCoreDataProvider

Public Class EsportazioneDatiBIOrogel_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function InizializzaTabelle(objParametriInterscambio As AgronicaCoreParametri, ByRef updateQuery As String) As DataTable
        Dim NomeRoutine As String = "AgronicaCoreInterscambioBIOrogelDAL.EsportazioneDatiBIOrogel_W.InizializzaTabelle()"
        Dim MessaggioErrore As String = ""
        Dim DT As New DataTable

        Try

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametriInterscambio, updateQuery, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            ' Registra l'eccezione
            ' Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
            DT = Nothing
        End Try

        Return DT
    End Function

    Public Function InserisciImprese(objParametriInterscambio As AgronicaCoreParametri, ByRef insertQuery As String) As DataTable
        Dim NomeRoutine As String = "AgronicaCoreInterscambioBIOrogelDAL.EsportazioneDatiBIOrogel_W.InserisciImprese()"
        Dim MessaggioErrore As String = ""
        Dim DT As New DataTable

        Try

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametriInterscambio, insertQuery, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
            DT = Nothing
        End Try

        Return DT
    End Function
    Public Function InserisciCentriAziendali(objParametriInterscambio As AgronicaCoreParametri, ByRef insertQuery As String) As DataTable
        Dim NomeRoutine As String = "AgronicaCoreInterscambioBIOrogelDAL.EsportazioneDatiBIOrogel_W.InserisciCentriAziendali()"
        Dim MessaggioErrore As String = ""
        Dim DT As New DataTable

        Try

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametriInterscambio, insertQuery, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
            DT = Nothing
        End Try

        Return DT
    End Function
    Public Function InserisciAppezzamenti(objParametriInterscambio As AgronicaCoreParametri, ByRef insertQuery As String) As DataTable
        Dim NomeRoutine As String = "AgronicaCoreInterscambioBIOrogelDAL.EsportazioneDatiBIOrogel_W.InserisciAppezzamenti()"
        Dim MessaggioErrore As String = ""
        Dim DT As New DataTable

        Try

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametriInterscambio, insertQuery, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
            DT = Nothing
        End Try

        Return DT
    End Function

    Public Function InserisciParticelle(objParametriInterscambio As AgronicaCoreParametri, ByRef insertQuery As String) As DataTable
        Dim NomeRoutine As String = "AgronicaCoreInterscambioBIOrogelDAL.EsportazioneDatiBIOrogel_W.InserisciParticelle()"
        Dim MessaggioErrore As String = ""
        Dim DT As New DataTable

        Try

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametriInterscambio, insertQuery, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
            DT = Nothing
        End Try

        Return DT
    End Function
    Public Function InserisciAppezzamentoParticelle(objParametriInterscambio As AgronicaCoreParametri, ByRef insertQuery As String) As DataTable
        Dim NomeRoutine As String = "AgronicaCoreInterscambioBIOrogelDAL.EsportazioneDatiBIOrogel_W.InserisciAppezzamentoParticelle()"
        Dim MessaggioErrore As String = ""
        Dim DT As New DataTable

        Try

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametriInterscambio, insertQuery, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
            DT = Nothing
        End Try

        Return DT
    End Function

    Public Function GetBackupDatiAltriAnni(tabellaOriginale As String, annoDaEscludere As Integer, objParametriInterscambio As AgronicaCoreDataProvider.AgronicaCoreParametri)
        Dim NomeRoutine As String = "Interscambio_BIOrogle.Utility.GetBackupDatiAltriAnni()"

        Dim backupData As New DataTable

        Dim strSql As New StringBuilder()

        Try
            strSql.AppendLine($" SELECT * FROM {tabellaOriginale} ")
            strSql.AppendLine($" WHERE ANNO_CHIAVE <> {annoDaEscludere} ")

            '--------------------------------------------------------------------------
            backupData = EseguiQuery_Lettura(objParametriInterscambio, strSql.ToString(), NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            Throw New Exception("[" & NomeRoutine & "] : " & ex.Message)
            backupData = Nothing
        End Try

        Return backupData
    End Function

End Class
