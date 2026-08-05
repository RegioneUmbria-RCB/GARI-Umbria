
Public Class Impianto

    Public Property Dettaglio_Specie_Personalizzato As String
    Public Property Magazzino_di_conferimento As String
    Public Property Tecnico_di_riferimento As String
    Public Property Data_Inizio_Impianto As String
    Public Property Sigla As String
    Public Property Codice_Cliente_2 As String
    Public Property note2 As String
    Public Property Regolamento As String
    Public Property Tipologia_Varietale As String
    Public Property Descrizione_Disciplinare As String
    Public Property Tipo_Lotta_Acquisti_des As String
    Public Property Tipo_Lotta_Acquisti As String
    Public Property Descrizione_PuntoDiPrelievo As String
    Public Property LottoFornitore As String
    Public Property NomeArticolo As String
    Public Property Note_Impianto As String
    Public Property Certificato As String
    Public Property Capitolato_Privato As String
    Public Property Codice_Cliente As String

    Public Property Piva As String
    Public Property Sa_Cod As Integer
    Public Property Appezza As Integer
    Public Property Id_Reg As Integer

    Public Property Veg_Cod As Integer
    Public Property Cul_Cod As Integer

    Public Property Rag_Soc As String
    Public Property Sa_Nome As String
    Public Property App_Nome As String
    Public Property Veg_Des As String
    Public Property Cul_Des As String
    Public Property Sup_Imp As String

    Public Property Data_Fornitura As Date
    Public Property Data_Raccolta As DateTime
    Public Property Data_Semina As DateTime

    Public Property CapitolatiEsclusi As String

End Class

Public Class ListaImpianti

    Private _Impianti As List(Of Impianto)

    Public Property Impianti As List(Of Impianto)
        Get
            Return _Impianti
        End Get
        Set(ByVal value As List(Of Impianto))
            _Impianti = value
        End Set
    End Property

    Public Sub New()
        _Impianti = New List(Of Impianto)
    End Sub

    Public Sub AggiungiImpianto(ByVal imp As Impianto)
        _Impianti.Add(imp)
    End Sub
End Class
