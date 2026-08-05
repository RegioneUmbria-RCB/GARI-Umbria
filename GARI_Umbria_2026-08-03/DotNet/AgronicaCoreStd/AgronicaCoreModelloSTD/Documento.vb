Imports AgronicaCoreDataProviderSTD.TipiEnumerativi
Imports AgronicaCoreEntityFrameworkSTD_POCO.Models

Public Class Documento

    Public Property TipoOperazioneDB As enum_TipoOperazioneDB
    Public Property Piva_Superuser As String
    Public Property Piva As String
    Public Property Azienda As APP_Imprese
    Public Property Documento_Cod As Integer
    Public Property ID_Tipologia As Integer
    Public Property Tipologia As APP_Tipologie
    Public Property Descrizione As String
    Public Property Data_Scadenza As Date
    Public Property Allegati As List(Of Allegato)
    Public Property Note As String
    Public Property DescrizioneAllegati As String
    Public Property Data_Upload As Date

    Public ReadOnly Property Categoria() As String
        Get
            Return Tipologia.ToString()
        End Get
    End Property

    Public ReadOnly Property DataScadenza() As String
        Get
            Return Data_Scadenza.ToString("ddd dd/MM/yyyy")
        End Get
    End Property

    Public ReadOnly Property TestoRicerca() As String
        Get

            Dim testo As String = Descrizione

            If Azienda IsNot Nothing Then
                testo &= " " & Azienda.rag_soc
            End If

            If Tipologia IsNot Nothing Then
                testo &= " " & Tipologia.Nome_Tipologia
            End If

            If Note IsNot Nothing Then
                testo &= " " & Note
            End If

            Return testo
        End Get
    End Property

End Class
