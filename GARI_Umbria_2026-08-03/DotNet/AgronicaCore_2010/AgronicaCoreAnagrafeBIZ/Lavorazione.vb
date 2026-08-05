Imports AgronicaCoreDataProvider

Public Class Lavorazione_R
    Public Function GetLavDes(lav_cod As String, objParametri_Server As AgronicaCoreParametri) As String
        Dim rOperazioni = New AgronicaCoreAnagrafeDAL.Operazioni_R
        Return rOperazioni.Lav_Des_From_Lav_Cod(lav_cod, objParametri_Server)
    End Function

End Class
