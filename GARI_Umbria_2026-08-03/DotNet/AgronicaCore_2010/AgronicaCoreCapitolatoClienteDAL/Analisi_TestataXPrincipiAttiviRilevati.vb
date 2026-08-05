

Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.AgronicaCoreParametri


Public Class Analisi_TestataXPrincipiAttiviRilevati_W
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

    Public Sub Cancella(ByVal PivaSuperUser As String, _
                        ByVal Progressivo As Integer, _
                        ByVal PA_COD As Integer, _
                        ByVal UniqueID As String, _
                         ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri)

        Dim NomeRoutine As String = "AgronicaCoreCapitolatoClienteDAL.Analisi_TestataXPrincipiAttiviRilevati_W.Scrivi()"

        Dim MessaggioErrore As String = ""
        Dim xRisp As Boolean = False
        Dim StrSql As String

        Try
            StrSql = "DELETE FROM Analisi_TestataXPrincipiAttiviRilevati " & _
                        " PivaSuperUser = '" & PivaSuperUser & "'" & _
                        " and Progressivo = " & Progressivo & "" & _
                        " and PA_COD = " & PA_COD & "" & _
                        " and UniqueID = '" & UniqueID & "'"


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
                        ByVal PA_COD As Integer, _
                        ByVal qtaRilevata As Decimal, _
                        ByVal qtaRilevataUdmGIAS As Integer, _
                        ByVal UniqueID As String, _
                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri)


        Dim NomeRoutine As String = "AgronicaCoreCapitolatoClienteDAL.Analisi_TestataXPrincipiAttiviRilevati_W.Scrivi()"

        Dim MessaggioErrore As String = ""
        Dim xRisp As Boolean = False
        Dim StrSql As String

        Try
            StrSql = _
            "INSERT INTO Analisi_TestataXPrincipiAttiviRilevati " & _
             "           ([PivaSuperUser] " & _
             "           ,[Progressivo] " & _
             "           ,[PA_COD] " & _
             "           ,[qtaRilevata] " & _
             "           ,[qtaRilevataUdmGIAS] " & _
             "           ,[UniqueID]) " & _
             "            VALUES " & _
             "           ('" & PivaSuperUser & "'" & _
             "           ," & Progressivo & "" & _
             "           ," & PA_COD & "" & _
             "           ," & Agro_SQL_SaveNum(CStr(qtaRilevata)) & " " & _
             "           ," & qtaRilevataUdmGIAS & "" & _
            "           ,'" & UniqueID & "')"

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
