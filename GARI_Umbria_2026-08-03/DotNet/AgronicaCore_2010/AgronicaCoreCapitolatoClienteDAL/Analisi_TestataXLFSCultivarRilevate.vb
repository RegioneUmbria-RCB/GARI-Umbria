

Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.AgronicaCoreParametri



Public Class Analisi_TestataXLFSCultivarRilevate
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Sub CancellaByUniqueID(ByVal UniqueID As String, _
                         ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri)

        Dim NomeRoutine As String = "AgronicaCoreCapitolatoClienteDAL.Analisi_TestataXPrincipiAttiviRilevati_W.Scrivi()"

        Dim MessaggioErrore As String = ""
        Dim xRisp As Boolean = False
        Dim StrSql As String

        Try
            StrSql = "DELETE FROM Analisi_TestataXPrincipiAttiviRilevati " & _
                        "WHERE  UniqueID = '" & UniqueID & "'"


            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSql, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

    End Sub



    Public Sub Scrivi(ByVal PivaSuperUser As String, _
                      ByVal Progressivo As Integer, _
                      ByVal veg_cod As Integer, _
                      ByVal cul_cod As Integer, _
                      ByVal UniqueID As String, _
                      ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri)


        Dim NomeRoutine As String = "AgronicaCoreCapitolatoClienteDAL.Analisi_Testata_W.Scrivi()"

        Dim MessaggioErrore As String = ""
        Dim xRisp As Boolean = False
        Dim StrSql As String

        Try
            StrSql = _
            "INSERT INTO Analisi_TestataXLFSCultivarRilevate " & _
             "           ([PivaSuperUser] " & _
             "           ,[Progressivo] " & _                          
             "           ,[Veg_Cod] " & _
             "           ,[Cul_cod] " & _
             "           ,[UniqueID]) " & _
             "            VALUES " & _
             "           ('" & PivaSuperUser & "' " & _
             "           ," & Progressivo & "" & _
             "           ," & veg_cod & "" & _
             "           ," & cul_cod & "" & _
             "           ,'" & UniqueID & "') "

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSql, NomeRoutine)
            '--------------------------------------------------------------------------
        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

    End Sub


End Class
