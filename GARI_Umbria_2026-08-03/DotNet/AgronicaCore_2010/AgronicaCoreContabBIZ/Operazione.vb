
Public Class Operazione_Causale_R
    Inherits AgronicaCoreDataProvider.LogProvider

    Public Function Leggi_CausaleDes_From_CausaleId(Id As Integer, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As String

        Dim dt As DataTable

        Dim objCausali_Dal As New AgronicaCoreContabDAL.Operazione_Causale_R
        dt = objCausali_Dal.Leggi_CausaleDes_From_CausaleId(Id, objParametri)

        If dt.Rows.Count = 0 Then
            Throw New Exception("Causale non trovata")
        End If

        Return dt.Rows(0)("Causale")

    End Function

End Class
