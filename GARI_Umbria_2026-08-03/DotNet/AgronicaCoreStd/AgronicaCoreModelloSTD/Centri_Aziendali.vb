Public Class Centri_Aziendali


    Public Property Piva As String
    Public Property Sa_Cod As Integer
    Public Property Rag_Soc As String
    Public Property Sa_Nome As String

    Public ReadOnly Property Centro_Des As String
        Get
            Return String.Format("{0} - {1}", Rag_Soc, Sa_Nome)
        End Get
    End Property

End Class
