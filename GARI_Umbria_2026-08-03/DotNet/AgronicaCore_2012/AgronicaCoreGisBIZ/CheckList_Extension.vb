Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate

Public Class CheckList_Extension_R
    Inherits LogProvider

    Public Function LeggiElencoCheckListAttivePerCodiceAlgoritmo(ByVal Algoritmo_Cod As Integer,
                                                                 ByVal DataRiferimento As DateTime,
                                                                 ByVal xFiltroAggiuntivo As String,
                                                                 ByRef ObjParametri_Server As AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreGisBIZ.CheckList_Extension_R.LeggiElencoCheckListAttivePerCodiceAlgoritmo()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Dim xRead As New AgronicaCoreGisDAL.CheckList_Extension_R

        Try
            DT = xRead.LeggiChecklistEstentionPerCodiceAlgoritmo(Algoritmo_Cod, DataRiferimento, xFiltroAggiuntivo, "", ObjParametri_Server)
        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(ObjParametri_Server, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try
        Return DT
    End Function

End Class
Public Class CheckList_Extension_W
    Inherits LogProvider

End Class
