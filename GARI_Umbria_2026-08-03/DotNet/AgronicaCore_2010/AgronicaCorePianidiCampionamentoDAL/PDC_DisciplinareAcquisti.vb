Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi

'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
Public Class PDC_DisciplinareAcquisti_R
    Inherits AgronicaCoreDataProvider.DataProvider


    Public Function Leggi(ByVal ID_DisciplinareAcquisti As Int32, _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                ) As DataTable
        Dim NomeRoutine As String = "AgronicaCoreAnalisiDAL.PDC_DisciplinareAcquisti_R.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   Padre_Piva 
        '   Padre_Sa_Cod 
        '   Padre_Appezza
        '   Figlio_Piva 
        '   Figlio_Sa_Cod
        '   Figlio_Appezza 
        '
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            StrSQL.Length = 0
            '---------------------------------------------
            StrSQL.AppendLine(" SELECT     *   ")
            StrSQL.AppendLine(" FROM         PDC_DisciplinareAcquisti  ")
            StrSQL.AppendLine(" WHERE 1 = 1 ")

            If ID_DisciplinareAcquisti <> 0 Then
                StrSQL.AppendLine(" AND PDC_DisciplinareAcquisti.ID_DisciplinareAcquisti = " & Agro_SQL_SaveNum(ID_DisciplinareAcquisti))
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

