

Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.AgronicaCoreParametri


Public Class Analisi_TestataXFamigliePrincipiAttiviRilevati_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Sub CancellaByUniqueID(ByVal UniqueID As String, _
                         ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri)

        Dim NomeRoutine As String = "AgronicaCoreCapitolatoClienteDAL.Analisi_TestataXFamigliePrincipiAttiviRilevati_W.Scrivi()"

        Dim MessaggioErrore As String = ""
        Dim xRisp As Boolean = False
        Dim StrSql As String

        Try
            StrSql = "DELETE FROM Analisi_TestataXFamigliePrincipiAttiviRilevati_W " & _
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

        Dim NomeRoutine As String = "AgronicaCoreCapitolatoClienteDAL.Analisi_TestataXFamigliePrincipiAttiviRilevati_W.Scrivi()"

        Dim MessaggioErrore As String = ""
        Dim xRisp As Boolean = False
        Dim StrSql As String

        Try
            StrSql = "DELETE FROM Analisi_TestataXFamigliePrincipiAttiviRilevati_W " & _
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
                        ByVal Fam_COD As String, _
                        ByVal qtaRilevata As Decimal, _
                        ByVal qtaRilevataUdmGIAS As Integer, _
                        ByVal UniqueID As String, _
                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri)


        Dim NomeRoutine As String = "AgronicaCoreCapitolatoClienteDAL.Analisi_TestataXFamigliePrincipiAttiviRilevati_W.Scrivi()"

        Dim MessaggioErrore As String = ""
        Dim xRisp As Boolean = False
        Dim StrSql As String

        Try
            StrSql =
            "INSERT INTO Analisi_TestataXFamigliePrincipiAttiviRilevati " &
             "           ([PivaSuperUser] " &
             "           ,[Progressivo] " &
             "           ,[Fam_COD] " &
             "           ,[qtaRilevata] " &
             "           ,[qtaRilevataUdmGIAS] " &
             "           ,[UniqueID]) " &
             "            VALUES " &
             "           ('" & PivaSuperUser & "'" &
             "           ," & Progressivo & "" &
             "           ,'" & Agro_SQL_SaveText(Fam_COD) & "' " &
             "           ," & Agro_SQL_SaveNum(CStr(qtaRilevata)) & " " &
             "           ," & qtaRilevataUdmGIAS & "" &
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
