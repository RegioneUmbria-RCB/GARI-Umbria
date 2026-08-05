
Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.AgronicaCoreParametri


Public Class Analisi_Testata_W

    Inherits AgronicaCoreDataProvider.DataProvider
    Public Sub CancellaDatiTempByUniqueID(ByVal UniqueID As String, _
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri)


        Dim NomeRoutine As String = "AgronicaCoreCapitolatoClienteDAL.Analisi_Testata_W.CancellaDatiTempByUniqueID()"

        Dim MessaggioErrore As String = ""
        Dim xRisp As Boolean = False
        Dim StrSql As String



        Try
            StrSql = _
                    "DELETE FROM Analisi_TestataXPrincipiAttiviRilevati WHERE " & _
                    " UniqueID = '" & UniqueID & "'"


            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSql, NomeRoutine)
            '--------------------------------------------------------------------------

            StrSql = _
                                "DELETE FROM Analisi_TestataXFamigliePrincipiAttiviRilevati WHERE " & _
                                " UniqueID = '" & UniqueID & "'"


            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSql, NomeRoutine)
            '--------------------------------------------------------------------------


            StrSql = _
                    "DELETE FROM Analisi_TestataXDPI WHERE " & _
                    " UniqueID = '" & UniqueID & "'"


            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSql, NomeRoutine)
            '--------------------------------------------------------------------------

            StrSql = _
                    "DELETE FROM Analisi_TestataXCapitolatoCliente WHERE " & _
                    " UniqueID = '" & UniqueID & "'"


            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSql, NomeRoutine)
            '--------------------------------------------------------------------------

            StrSql = _
                    "DELETE FROM Analisi_TestataXLFSCultivarRilevate WHERE " & _
                    " UniqueID = '" & UniqueID & "'"


            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSql, NomeRoutine)
            '--------------------------------------------------------------------------


            StrSql = _
                    "DELETE FROM Analisi_Testata WHERE " & _
                    " UniqueID = '" & UniqueID & "'"


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
    Public Sub CancellaByUniqueID(ByVal UniqueID As String, _
                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri)

        Dim NomeRoutine As String = "AgronicaCoreCapitolatoClienteDAL.Analisi_Testata_W.CancellaByUniqueID()"

        Dim MessaggioErrore As String = ""
        Dim xRisp As Boolean = False
        Dim StrSql As String

        Try
            StrSql = _
                    "DELETE FROM Analisi_Testata WHERE " & _
                    " UniqueID = '" & UniqueID & "'"


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
                      ByVal UniqueID As String, _
                      ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri)


        Dim NomeRoutine As String = "AgronicaCoreCapitolatoClienteDAL.Analisi_Testata_W.Cancella()"

        Dim MessaggioErrore As String = ""
        Dim xRisp As Boolean = False
        Dim StrSql As String

        Try
            StrSql = _
                    "DELETE FROM Analisi_Testata WHERE " & _
                    " PivaSuperUser = '" & PivaSuperUser & "'" & _
                    " and Progressivo = " & Progressivo & _
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
                      ByVal Descrizione As String, _
                      ByVal dataAnalisi As DateTime, _
                      ByVal veg_cod As Integer, _
                      ByVal UniqueID As String, _
                      ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri)


        Dim NomeRoutine As String = "AgronicaCoreCapitolatoClienteDAL.Analisi_Testata_W.Scrivi()"

        Dim MessaggioErrore As String = ""
        Dim xRisp As Boolean = False
        Dim StrSql As String

        Try
            StrSql =
            "INSERT INTO Analisi_Testata " &
             "           ([PivaSuperUser] " &
             "           ,[Progressivo] " &
             "           ,[Descrizione] " &
             "           ,[Data_Analisi] " &
             "           ,[Veg_Cod] " &
             "           ,[UniqueID]) " &
             "            VALUES " &
             "           ('" & PivaSuperUser & "' " &
             "           ," & Progressivo & "" &
             "           ,'" & Agro_SQL_SaveText(Descrizione) & "' " &
             "           ,'" & Agro_SQL_SaveText(dataAnalisi.ToString.Replace(".", ":")) & "' " &
             "           ," & veg_cod & "" &
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

Public Class Analisi_Testata_R

End Class
