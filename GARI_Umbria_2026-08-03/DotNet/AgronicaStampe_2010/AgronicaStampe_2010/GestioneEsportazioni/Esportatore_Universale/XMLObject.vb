Public Class XMLObject

#Region "Imprese"

    Public Class ImpreseList
        Property imprese As List(Of Impresa)
    End Class

    Public Class Impresa
        Property PIVA As String
        Property Ragione_Sociale As String
        Property Tipo_Impresa As String
        Property Codice_Socio As String
        Property CUAA As String
        Property Impresa_Padre As String
        Property PIVA_Padre As String
        Property Indirizzo As Indirizzo
        Property Istat_Comune As String
        Property Istat_Provincia As String
        Property Rappresentante_Legale As Rappresentante_Legale
        Property Particelle As List(Of Particella)
        Property Data_Inizio As Date?
        Property Data_Fine As Date?
    End Class

    Public Class Indirizzo
        Property Indirizzo_Des As String
        Property Frazione_Des As String
        Property CAP As String
        Property Comune_Des As String
        Property Provincia_Cod As String
    End Class

    Public Class Particella
        Property Provincia As String
        Property Comune As String
        Property Sezione As String
        Property Foglio As String
        Property Numero As String
        Property Subalterno As String
        Property Titolo_Possesso As String
        Property Data_Inizio_Possesso As Date?
        Property Data_Fine_Possesso As Date?
        Property Superficie_HA As Integer
        Property Superficie_AA As Integer
        Property Superficie_CA As Integer
        Property Superficie As Double
    End Class

    Public Class Rappresentante_Legale
        Property Codice_Fiscale As String
        Property Cognome_Nome As String
        Property Comune_Nascita As String
        Property Provincia_Nascita As String
    End Class

#End Region

#Region "Impianti"
    Public Class ImpiantiList
        Property impianti As List(Of Impianto)
    End Class

    Public Class Impianto
        Property id_Impianto As String
        Property PIVA As String
        Property Ragione_Sociale As String
        Property Codice_Socio As String
        Property CUAA As String
        Property Impresa_Padre As String
        Property PIVA_Padre As String
        Property CentroAziendale_id As Integer
        Property CentroAziendale_Nome As String
        Property Veg_Cod As Integer
        Property Veg_Des As String
        Property Cul_Cod As Integer
        Property Cul_Des As String
        Property DataInizioImpianto As Date
        Property DataFineImpianto As Date
        Property Superficie As Double
        Property Tipo_Copertura As String
        Property Portainnesto As String
        Property Operazione As String
        Property DataSeminaTrapianto As Date
        Property MetodoProduzione As Integer
        Property Nr_Piante As Integer
        Property GuppoVarietale As String
        Property Catasto As ImpiantoCatasto
    End Class

    Public Class ImpiantoCatasto
        Property Prov As String
        Property Com As String
        Property Sezione As String
        Property Foglio As String
        Property Numero As String
        Property Subalterno As String
    End Class

#End Region

End Class

