Public Class Ricette_Operazioni

    'info di testata 
    Public Property DataRiferimento As DateTime

    Public Property W_Anagrafica_Stati_Cod As Integer
    Public Property Ricetta_Operazione_Cod As Integer
    Public Property Ricetta_Operazione_Des As String
    Public Property Note As String
    Public Property Operazione As Operazione
    Public Property OperazioneAttivita As OperazioneAttivita
    Public Property Epoca As Integer

    ''' <summary>
    ''' Sempre ad udm HL (se positiva H2O Totale, Se negativa H20 Per ha)
    ''' </summary>
    ''' <returns></returns>
    Public Property Acqua As Decimal

    ''' <summary>
    ''' Se = 0 allora Totale, se = 1 allora ad ettaro
    ''' </summary>
    ''' <returns></returns>
    Public Property Acqua_Totale_o_Ha As Integer

    Public Property Dettagli As List(Of Ricette_Dettagli)

    Public Property RisorseImpiegate As List(Of Ricette_Risorse)

    Public Property CentroAziendale As Centri_Aziendali

    Public Property Specie As SpecieVegetaliDestinazioni


    Public Property Ricetta_Operazione_Rif As Ricette_Operazioni

    Public Property DescrizioneImpianti As String

    Public Property Bozza As Integer

    Public ReadOnly Property TestoRicerca() As String
        Get
            Return DataRiferimento.ToString("dd/MM/yyyy") & " " & Ricetta_Operazione_Des & " " & CentroAziendale.Centro_Des & " " & Specie.veg_des & " " & DescrizioneImpianti
        End Get
    End Property

End Class
