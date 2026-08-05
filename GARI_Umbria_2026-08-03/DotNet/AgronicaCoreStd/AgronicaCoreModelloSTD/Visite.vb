Imports AgronicaCoreDataProviderSTD.TipiEnumerativi
Imports AgronicaCoreEntityFrameworkSTD_POCO.Models

Public Class Visite

    Public Property TipoOperazioneDB As enum_TipoOperazioneDB
    Public Property Tipo_Visita As enum_TipoVisita_DB

    Public Property Visita_Cod As Integer
    Public Property Data_Visita As Date
    Public Property Operatore As String
    Public Property Posizione As String
    Public Property DescrizionePosizione As String
    Public Property Centro_Aziendale As Centri_Aziendali
    Public Property Specie As SpecieVegetaliDestinazioni
    Public Property Note As String

    Public Property Visita_Dettaglio_Cod As Integer
    Public Property Attivita As Attivita
    Public Property Descrizione As String
    Public Property DescrizioneImpianti As String
    Public Property Impianti As List(Of APP_Reg_Impianti)

    Public Property Documento As Documento

    Public ReadOnly Property DescrizioneDocumento() As String
        Get
            Return If(Documento Is Nothing, "", Documento.DescrizioneAllegati)
        End Get
    End Property

    Public ReadOnly Property DataOperatore() As String
        Get
            Return Data_Visita.ToString("ddd dd/MM/yyyy ore HH:mm") & " - " & Operatore
        End Get
    End Property

    Public ReadOnly Property TestoRicerca() As String
        Get

            Dim testo As String = Data_Visita.ToString("dd/MM/yyyy - HH:mm") & " " & Attivita.Descrizione

            If Centro_Aziendale IsNot Nothing Then
                testo &= " " & Centro_Aziendale.Centro_Des
                testo &= " " & Centro_Aziendale.Rag_Soc
            End If

            If Specie IsNot Nothing Then
                testo &= " " & Specie.veg_des
            End If

            If Descrizione IsNot Nothing Then
                testo &= " " & Descrizione
            End If

            If DescrizioneImpianti IsNot Nothing Then
                testo &= " " & DescrizioneImpianti
            End If

            Return testo
        End Get
    End Property

End Class
