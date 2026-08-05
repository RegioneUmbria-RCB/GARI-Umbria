Imports System.ComponentModel

Public Class RispostaFiltrone

#Region "proprietà"
    'chiavi

    Private _ID_PDC_Testata As String
    Public Property ID_PDC_Testata() As String
        Get
            Return _ID_PDC_Testata
        End Get
        Set(value As String)
            _ID_PDC_Testata = value
        End Set
    End Property
    Private _ID_PDC_Dettagli As String
    Public Property ID_PDC_Dettagli() As String
        Get
            Return _ID_PDC_Dettagli
        End Get
        Set(value As String)
            _ID_PDC_Dettagli = value
        End Set
    End Property
    Private _ID_PDC_Campione As String
    Public Property ID_PDC_Campione() As String
        Get
            Return _ID_PDC_Campione
        End Get
        Set(value As String)
            _ID_PDC_Campione = value
        End Set
    End Property
    Private _Analisi_Testata_Cod As String
    Public Property Analisi_Testata_Cod() As String
        Get
            Return _Analisi_Testata_Cod
        End Get
        Set(value As String)
            _Analisi_Testata_Cod = value
        End Set
    End Property
    Private _PDC_Stato_Analisi As Integer?
    Public Property PDC_Stato_Analisi() As Integer?
        Get
            Return _PDC_Stato_Analisi
        End Get
        Set(value As Integer?)
            _PDC_Stato_Analisi = value
        End Set
    End Property
    Private _Analisi_Tipologia_Tipo As Integer?
    Public Property Analisi_Tipologia_Tipo() As Integer?
        Get
            Return _Analisi_Tipologia_Tipo
        End Get
        Set(value As Integer?)
            _Analisi_Tipologia_Tipo = value
        End Set
    End Property

    'valori

    Private _Nome_Campionamento As String
    Public Property Nome_Campionamento() As String
        Get
            Return _Nome_Campionamento
        End Get
        Set(value As String)
            _Nome_Campionamento = value
        End Set
    End Property
    Private _Data_Approssimativa As Date?
    Public Property Data_Approssimativa() As Date?
        Get
            Return _Data_Approssimativa
        End Get
        Set(value As Date?)
            _Data_Approssimativa = value
        End Set
    End Property
    Private _Provenienza As String
    Public Property Provenienza() As String
        Get
            Return _Provenienza
        End Get
        Set(value As String)
            _Provenienza = value
        End Set
    End Property
    Private _Descrizione_Acquisto As String
    Public Property Descrizione_Acquisto() As String
        Get
            Return _Descrizione_Acquisto
        End Get
        Set(value As String)
            _Descrizione_Acquisto = value
        End Set
    End Property
    Private _Codice_Fornitore As String
    Public Property Codice_Fornitore() As String
        Get
            Return _Codice_Fornitore
        End Get
        Set(value As String)
            _Codice_Fornitore = value
        End Set
    End Property
    Private _Piva As String
    Public Property Piva() As String
        Get
            Return _Piva
        End Get
        Set(value As String)
            _Piva = value
        End Set
    End Property
    Private _PivaReale As String
    Public Property PivaReale() As String
        Get
            Return _PivaReale
        End Get
        Set(value As String)
            _PivaReale = value
        End Set
    End Property
    Private _Ragione_Sociale As String
    Public Property Ragione_Sociale() As String
        Get
            Return _Ragione_Sociale
        End Get
        Set(value As String)
            _Ragione_Sociale = value
        End Set
    End Property

    Public Property Rag_Soc_Padre() As String

    Private _Centro As String
    Public Property Centro() As String
        Get
            Return _Centro
        End Get
        Set(value As String)
            _Centro = value
        End Set
    End Property
    Private _Appezzamento As String
    Public Property Appezzamento() As String
        Get
            Return _Appezzamento
        End Get
        Set(value As String)
            _Appezzamento = value
        End Set
    End Property
    Private _Superficie As Double?
    Public Property Superficie() As Double?
        Get
            Return _Superficie
        End Get
        Set(value As Double?)
            _Superficie = value
        End Set
    End Property
    Private _Note_Impianto As String
    Public Property Note_Impianto() As String
        Get
            Return _Note_Impianto
        End Get
        Set(value As String)
            _Note_Impianto = value
        End Set
    End Property
    Private _Specie_Vegetale As String
    Public Property Specie_Vegetale() As String
        Get
            Return _Specie_Vegetale
        End Get
        Set(value As String)
            _Specie_Vegetale = value
        End Set
    End Property
    Private _Varietà As String
    Public Property Varietà() As String
        Get
            Return _Varietà
        End Get
        Set(value As String)
            _Varietà = value
        End Set
    End Property
    Private _Tipologia_Varietale As String
    Public Property Tipologia_Varietale() As String
        Get
            Return _Tipologia_Varietale
        End Get
        Set(value As String)
            _Tipologia_Varietale = value
        End Set
    End Property
    Private _Campionato As String
    Public Property Campionato() As String
        Get
            Return _Campionato
        End Get
        Set(value As String)
            _Campionato = value
        End Set
    End Property
    Private _Capitolato_Privato As String
    Public Property Capitolato_Privato() As String
        Get
            Return _Capitolato_Privato
        End Get
        Set(value As String)
            _Capitolato_Privato = value
        End Set
    End Property
    Private _Certificato As String
    Public Property Certificato() As String
        Get
            Return _Certificato
        End Get
        Set(value As String)
            _Certificato = value
        End Set
    End Property
    Private _Data_Semina As Date?
    Public Property Data_Semina() As Date?
        Get
            Return _Data_Semina
        End Get
        Set(value As Date?)
            _Data_Semina = value
        End Set
    End Property
    Private _Data_Raccolta As Date?
    Public Property Data_Raccolta() As Date?
        Get
            Return _Data_Raccolta
        End Get
        Set(value As Date?)
            _Data_Raccolta = value
        End Set
    End Property
    Private _Data_Fornitura As Date?
    Public Property Data_Fornitura() As Date?
        Get
            Return _Data_Fornitura
        End Get
        Set(value As Date?)
            _Data_Fornitura = value
        End Set
    End Property
    Private _Codice_Articolo As String
    Public Property Codice_Articolo() As String
        Get
            Return _Codice_Articolo
        End Get
        Set(value As String)
            _Codice_Articolo = value
        End Set
    End Property
    Private _Lotto_Fornitore As String
    Public Property Lotto_Fornitore() As String
        Get
            Return _Lotto_Fornitore
        End Get
        Set(value As String)
            _Lotto_Fornitore = value
        End Set
    End Property
    Private _Tipologia_Lotta As String
    Public Property Tipologia_Lotta() As String
        Get
            Return _Tipologia_Lotta
        End Get
        Set(value As String)
            _Tipologia_Lotta = value
        End Set
    End Property
    Private _Nome_Articolo As String
    Public Property Nome_Articolo() As String
        Get
            Return _Nome_Articolo
        End Get
        Set(value As String)
            _Nome_Articolo = value
        End Set
    End Property
    Private _Punto_di_Prelievo As String
    Public Property Punto_di_Prelievo() As String
        Get
            Return _Punto_di_Prelievo
        End Get
        Set(value As String)
            _Punto_di_Prelievo = value
        End Set
    End Property
    Private _Descrizione As String
    Public Property Descrizione() As String
        Get
            Return _Descrizione
        End Get
        Set(value As String)
            _Descrizione = value
        End Set
    End Property
    Private _Documentazione As String
    Public Property Documentazione() As String
        Get
            Return _Documentazione
        End Get
        Set(value As String)
            _Documentazione = value
        End Set
    End Property
    Private _Note_Documentazione As String
    Public Property Note_Documentazione() As String
        Get
            Return _Note_Documentazione
        End Get
        Set(value As String)
            _Note_Documentazione = value
        End Set
    End Property
    Private _Disciplinare As String
    Public Property Disciplinare() As String
        Get
            Return _Disciplinare
        End Get
        Set(value As String)
            _Disciplinare = value
        End Set
    End Property

    Private _Regolamento As String
    Public Property Regolamento() As String
        Get
            Return _Regolamento
        End Get
        Set(value As String)
            _Regolamento = value
        End Set
    End Property



    'campioni
    Private _Data_campionamento As Date?
    Public Property Data_campionamento() As Date?
        Get
            Return _Data_campionamento
        End Get
        Set(value As Date?)
            _Data_campionamento = value
        End Set
    End Property
    Private _Codice_Campione As String
    Public Property Codice_Campione() As String
        Get
            Return _Codice_Campione
        End Get
        Set(value As String)
            _Codice_Campione = value
        End Set
    End Property
    Private _Note_Campione As String
    Public Property Note_Campione() As String
        Get
            Return _Note_Campione
        End Get
        Set(value As String)
            _Note_Campione = value
        End Set
    End Property


    'campioni
    Private _Laboratorio As String
    Public Property Laboratorio() As String
        Get
            Return _Laboratorio
        End Get
        Set(value As String)
            _Laboratorio = value
        End Set
    End Property
    Private _CodiceGriglia As String
    Public Property CodiceGriglia() As String
        Get
            Return _CodiceGriglia
        End Get
        Set(value As String)
            _CodiceGriglia = value
        End Set
    End Property
    Private _Data_Richiesta As Date?
    Public Property Data_Richiesta() As Date?
        Get
            Return _Data_Richiesta
        End Get
        Set(value As Date?)
            _Data_Richiesta = value
        End Set
    End Property

    Private _altreMolecole As String
    Public Property altreMolecole() As String
        Get
            Return _altreMolecole
        End Get
        Set(value As String)
            _altreMolecole = value
        End Set
    End Property


    Private _Analisi_Testata_Des As String
    Public Property Analisi_Testata_Des() As String
        Get
            Return _Analisi_Testata_Des
        End Get
        Set(value As String)
            _Analisi_Testata_Des = value
        End Set
    End Property

    Private _Data_Fine_Analisi As Date?
    Public Property Data_Fine_Analisi() As Date?
        Get
            Return _Data_Fine_Analisi
        End Get
        Set(value As Date?)
            _Data_Fine_Analisi = value
        End Set
    End Property

    Private _Analisi_Descrizione_Principi_Attivi As String
    Public Property Analisi_Descrizione_Principi_Attivi() As String
        Get
            Return _Analisi_Descrizione_Principi_Attivi
        End Get
        Set(value As String)
            _Analisi_Descrizione_Principi_Attivi = value
        End Set
    End Property

    Private _Link_Analisi As String
    Public Property Link_Analisi() As String
        Get
            Return _Link_Analisi
        End Get
        Set(value As String)
            _Link_Analisi = value
        End Set
    End Property
    Private _Stampa_Rapporto_Prova As String
    Public Property Stampa_Rapporto_Prova() As String
        Get
            Return _Stampa_Rapporto_Prova
        End Get
        Set(value As String)
            _Stampa_Rapporto_Prova = value
        End Set
    End Property

    Private _ValidaEU As String
    Public Property ValidaEU() As String
        Get
            Return _ValidaEU
        End Get
        Set(value As String)
            _ValidaEU = value
        End Set
    End Property
#End Region



    Public Shared Function ToDataTable(Of T)(data As IList(Of T)) As DataTable
        Dim props As PropertyDescriptorCollection = TypeDescriptor.GetProperties(GetType(T))
        Dim table As New DataTable()
        For i As Integer = 0 To props.Count - 1
            Dim prop As PropertyDescriptor = props(i)



            Dim propertyType = prop.PropertyType
            If propertyType.IsGenericType Then
                propertyType = propertyType.GetGenericArguments()(0)
            End If
            table.Columns.Add(prop.Name, propertyType)


        Next
        Dim values As Object() = New Object(props.Count - 1) {}
        For Each item As T In data
            For i As Integer = 0 To values.Length - 1
                values(i) = props(i).GetValue(item)
            Next
            table.Rows.Add(values)
        Next
        Return table
    End Function

    'Public Shared Function ToDataTable(Of T)(data As IList(Of T)) As DataTable
    '    Dim table As New DataTable()
    '    table.Columns.Add(prop.Name, prop.PropertyType)

    '    Return table
    'End Function


End Class
