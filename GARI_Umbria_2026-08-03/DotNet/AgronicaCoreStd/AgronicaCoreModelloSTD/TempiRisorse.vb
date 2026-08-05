Imports AgronicaCoreEntityFrameworkSTD_POCO.Models

Public Class TempiRisorse

    Public Property Id_Cdg_Generale As Integer

    Public Property Azienda As APP_Imprese
    Public Property Attivita As Attivita
    Public Property OperazioneAttivita As OperazioneAttivita
    Public Property CentroAziendale As Centri_Aziendali
    Public Property Progetto As APP_Imputazioni_Fasi
    Public Property DataInserimento As DateTime
    Public Property Impianti As List(Of APP_Reg_Impianti)
    Public Property Movimenti As List(Of TempiRisorse_Movimenti)

    Public Property Ricetta As Ricette
    Public Property RicettaOperazione As Ricette_Operazioni
    Public Property DescrizioneImpianti As String
    Public Property DescrizionePersone As String
    Public Property Bozza As Integer
    Public Property Note As String

    Public ReadOnly Property ExtraCampagna() As Boolean
        Get
            Return Ricetta Is Nothing
        End Get
    End Property

    Public ReadOnly Property TestoRicerca() As String
        Get
            Dim testo As String = DataInserimento.ToString("dd/MM/yyyy - HH:mm") & " " & Attivita.Descrizione
            If CentroAziendale IsNot Nothing Then
                testo &= " " & CentroAziendale.Centro_Des
            Else
                testo &= " " & Azienda.rag_soc
            End If
            If Progetto IsNot Nothing Then
                testo &= " " & Progetto.Imputazione_Nome
            End If
            If DescrizioneImpianti IsNot Nothing Then
                testo &= " " & DescrizioneImpianti
            End If
            If DescrizionePersone IsNot Nothing Then
                testo &= " " & DescrizionePersone
            End If
            Return testo
        End Get
    End Property

End Class
