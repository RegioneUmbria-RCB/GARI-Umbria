Namespace AnagrafeNG
    Public Class Campi
        Public Property Impresa As Imprese
        Public Property Centro As Centri

        Public Property Piva As String
        Public Property Sa_Cod As Integer

        Public Property Campo_Cod As Integer
        Public Property Campo_Des As String

        Public Property Serra As Boolean

        Public Property Campo_Codice As String

        Public Property Orientamento_Colturale As Integer
        Public Property Veg_Cod As Integer

        Public Property Validita_Inizio As Date
        Public Property Validita_Fine As Date
        Public Property Codici As List(Of Codici)
        Public Property Catasto As List(Of Catasto_Campo)
    End Class


    Public Class Catasto_Campo

        Public Property Prov As String
        Public Property Com As String
        Public Property Sezione As String
        Public Property Foglio As Integer
        Public Property Numero As Integer
        Public Property Subalterno As String
        Public Property Area As Double
        Public Property Prov_Des As String
        Public Property Com_Des As String
        Public Property Superficie_Condotta As Double
        Public Property Superficie_Catastale As Double

    End Class

End Namespace


