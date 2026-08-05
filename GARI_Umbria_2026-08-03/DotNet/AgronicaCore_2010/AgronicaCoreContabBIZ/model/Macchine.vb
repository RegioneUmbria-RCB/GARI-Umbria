Namespace AnagrafeNG

    Public Class Macchine
        Public Property Piva As String
        Public Property Sa_Cod As Integer

        Public Property Mac_Cod As Integer
        Public Property Mac_Des As String
        Public Property Marca As Integer
        Public Property Modello As String
        Public Property Finalita As Integer

        Public Property Tipo As String
        Public Property Dettaglio_1 As String
        Public Property Dettaglio_2 As String

        Public Property Codice As String

        Public Property Validita_Inizio As Date
        Public Property Validita_Fine As Date
        Public Property Data_Carico As Date
        Public Property Data_Scarico As Date

        Public Property Titolo_Possesso As Integer
        Public Property Proprietario As String
        Public Property CUAA_Proprietario As String

        '''Dati Tecnici
        Public Property Targa As String
        Public Property Tipo_Targa As Integer

        Public Property Telaio As String

        Public Property N_Immatricolazione As String
        Public Property Data_Immatricolazione As Date
        Public Property N_Immatricolazione_Rimorchio As String
        Public Property N_Autorizzazione_Trasporto As String
        Public Property Data_Rilascio_Autorizzazione As Date

        Public Property Alimentazione As Integer


        ''Taratura Ugello
        Public Property Taratura_Ugello As Decimal
        Public Property Data_Ultima_Taratura As Date
        Public Property Scadenza_Taratura As Date

        Public Property Stato_Utilizzo As String
        Public Property Potenza As String
        Public Property Unita_Misura As String
        Public Property Note As String

        Public Property Costi As List(Of Costo_Unitario)
    End Class

    Public Class Costo_Unitario

        Public ID As Integer
        Public Unita_Misura As String
        Public Prezzo As Decimal
        Public Validita_Inizio As Date
        Public Validita_Fine As Date

    End Class

End Namespace


