Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi

Public Class Analisi_Appoggio_Import_W
    Inherits AgronicaCoreDataProvider.DataProvider



    '#########################################################################
    Public Function Scrivi( _
                                ByVal Analisi_Testata_Cod As Integer, _
                                ByVal Data_Import As DateTime, _
                                ByVal pivaLab As String, _
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                    ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Analisi_Appoggio_Import_W.Scrivi()"

        '====================================================================================
        'Parametri opzionali :
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            StrSQL.Length = 0


            StrSQL.Append("INSERT INTO Analisi_Appoggio_Import( ")
            StrSQL.Append("            PivaSuperUser, Analisi_Testata_Cod,  ")
            StrSQL.Append("            Data_Import, PivaLaboratorioAnalisi ")
            StrSQL.Append("            ) ")


            StrSQL.Append("VALUES (")
            StrSQL.Append("          '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Analisi_Testata_Cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDateTime(Data_Import) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(pivaLab) & "' ")
            StrSQL.Append(")")

            '---------------------------------------------


            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return xRisp

    End Function
 

    '#########################################################################
    Public Function Cancella( _
                            ByVal Analisi_Testata_Cod As Integer, _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Analisi_Appoggio_Import_W.Cancella()"
 
        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        If Analisi_Testata_Cod = 0 Then
            Throw New Exception("Analisi_Testata_Cod = 0 ")
        End If


        Try

            StrSQL.Length = 0
            StrSQL.Append(" DELETE ")
            StrSQL.Append(" FROM     Analisi_Appoggio_Import ")
            StrSQL.Append(" WHERE    PivaSuperUser = '" & Agro_SQL_SaveText(Trim(objParametri.PivaSuperUser)) & "' ")
            StrSQL.Append(" AND Analisi_Testata_Cod = " & Agro_SQL_SaveNum(Analisi_Testata_Cod) & " ")
            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
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

'#######################################################################
'#######################################################################
'#######################################################################

Public Class Analisi_Appoggio_Import_R
    Inherits AgronicaCoreDataProvider.DataProvider


    '##############################################################################################
    Public Function LeggiTutto(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                               ByVal pivaLab As String
                               ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Analisi_Appoggio_Import_R.LeggiTutto()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            StrSQL.Length = 0
            StrSQL.Append(" SELECT * ")
            StrSQL.Append(" FROM   Analisi_Appoggio_Import ")
            StrSQL.Append(" WHERE  PivaSuperUser = '" & Trim(objParametri.PivaSuperUser) & "'")

            If Not IsNothing(pivaLab) Then
                StrSQL.Append(" AND  PivaLaboratorioAnalisi = '" & Agro_SQL_SaveText(Trim(pivaLab)) & "'")
            End If
            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function

End Class