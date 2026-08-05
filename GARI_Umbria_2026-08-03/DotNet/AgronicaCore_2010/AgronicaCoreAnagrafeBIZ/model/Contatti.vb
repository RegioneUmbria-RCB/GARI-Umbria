Namespace AnagrafeNG

    Public Class Contatti
        Public Property Impresa As Imprese
        Public Property Centro As Centri

        Public Property Piva As String
        Public Property Sa_Cod As Integer

        Public Property Cod_Contatto As String

        Public Property Tipo As String
        Public Property Fittizio As Boolean

        Public Property Fisico_Giuridico As Integer
        Public Property Visibilita As String

        Public Property Ragione_Sociale As String

        Public Property Cognome As String
        Public Property Nome As String

        Public Property Nome_Breve As String
        Public Property Data_Nascita As Date
        Public Property Sesso As String
        Public Property Convenevoli As String

        Public Property Badge As String

        Public Property Rubrica As Rubrica()
        Public Property Indirizzi As Indirizzi()

        Public Property Rapporti_Contabili

        '''Fattura Elettronica
        Public Property FE_Tipologia_Contatto As String
        Public Property FE_Rappresentante_Fiscale As String
        Public Property FE_Pec As String
        Public Property FE_SDI As String


    End Class

End Namespace
