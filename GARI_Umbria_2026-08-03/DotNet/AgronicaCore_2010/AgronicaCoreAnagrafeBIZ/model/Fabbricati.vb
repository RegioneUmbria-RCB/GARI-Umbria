Namespace AnagrafeNG

    Public Class Fabbricati
        Public Property Impresa As Imprese
        Public Property Centro As Centri

        Public Property Piva As String
        Public Property Sa_Cod As Integer
        Public Property Fabbricato_Cod As Integer
        Public Property Tipologia As Integer

        Public Property Validita_Inizio As Date
        Public Property Validita_Fine As Date

        Public Property Indirizzo As Indirizzi

        Public Property Idoneita As String()

        Public Property Titolo_Possesso As Integer
        'public property Catasto As Particella_Catastale
        Public Property Regolamento As String

        Public Property Volume_Convenzionale As Decimal
        Public Property Volume_Biologico As Decimal

        Public Property Visibile_Da_App As Boolean
        Public Property Default_Magazzino_Imballi_Ortofrutta As Boolean

        Public Property Codici As List(Of Codici)

    End Class

End Namespace
