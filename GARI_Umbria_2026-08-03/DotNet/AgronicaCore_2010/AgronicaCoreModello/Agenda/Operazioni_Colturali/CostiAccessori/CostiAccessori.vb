Public Class CostiAccessori

    Private _KendoKey As String
    Private _Centro As String
    Private _Sa_Cod As Integer
    Private _Centro_Cod As Integer
    Private _Categoria_Des As String
    Private _Risorsa_Des As String
    Private _Udm_Des As String
    Private _Qta_Ril As Decimal
    Private _Udm_Cod As Integer
    Private _Elem_Cod As Integer
    Private _Riga As Integer
    Private _Tipo_Centro As String
    Private _Pro_Cod As Integer
    Private _Ditta_Cod As Integer
    Private _Mat_Cod As Integer
    Private _Costo_Unitario As String
    Private _Costo As String
    Private _Ore As Integer
    Private _Minuti As Integer
    Private _Codice As String
    Private _Lotto As String    

    Private _Udm_Selezionata As Integer 
    Private _Valore As String 
    Private _ID_Attivita As Integer 
    Private _Turno_Cod As Integer 
    Private _Qualifica_Cod As Integer 
    Private _Tariffa_Cod As Integer 
    private _Cod_Rapporto As Integer 


    Public Property KendoKey As String
        Get
            Return _KendoKey
        End Get
        Set(value As String)
            _KendoKey = value
        End Set
    End Property

    Public Property Centro As String
        Get
            Return _Centro
        End Get
        Set(value As String)
            _Centro = value
        End Set
    End Property

    Public Property Sa_Cod As Integer
        Get
            Return _Sa_Cod
        End Get
        Set(value As Integer)
            _Sa_Cod = value
        End Set
    End Property

    Public Property Centro_Cod As Integer
        Get
            Return _Centro_Cod
        End Get
        Set(value As Integer)
            _Centro_Cod = value
        End Set
    End Property

    Public Property Categoria_Des As String
        Get
            Return _Categoria_Des
        End Get
        Set(value As String)
            _Categoria_Des = value
        End Set
    End Property

    Public Property Risorsa_Des As String
        Get
            Return _Risorsa_Des
        End Get
        Set(value As String)
            _Risorsa_Des = value
        End Set
    End Property

    Public Property Udm_Des As String
        Get
            Return _Udm_Des
        End Get
        Set(value As String)
            _Udm_Des = value
        End Set
    End Property

    Public Property Qta_Ril As Decimal
        Get
            Return _Qta_Ril
        End Get
        Set(value As Decimal)
            _Qta_Ril = value
        End Set
    End Property

    Public Property Udm_Cod As Integer
        Get
            Return _Udm_Cod
        End Get
        Set(value As Integer)
            _Udm_Cod = value
        End Set
    End Property

    Public Property Elem_Cod As Integer
        Get
            Return _Elem_Cod
        End Get
        Set(value As Integer)
            _Elem_Cod = value
        End Set
    End Property

    Public Property Riga As Integer
        Get
            Return _Riga
        End Get
        Set(value As Integer)
            _Riga = value
        End Set
    End Property

    Public Property Tipo_Centro As String
        Get
            Return _Tipo_Centro
        End Get
        Set(value As String)
            _Tipo_Centro = value
        End Set
    End Property

    Public Property Pro_Cod As Integer
        Get
            Return _Pro_Cod
        End Get
        Set(value As Integer)
            _Pro_Cod = value
        End Set
    End Property

    Public Property Ditta_Cod As Integer
        Get
            Return _Ditta_Cod
        End Get
        Set(value As Integer)
            _Ditta_Cod = value
        End Set
    End Property

    Public Property Mat_Cod As Integer
        Get
            Return _Mat_Cod
        End Get
        Set(value As Integer)
            _Mat_Cod = value
        End Set
    End Property

    Public Property Costo As String
        Get
            Return _Costo
        End Get
        Set(value As String)
            _Costo = value
        End Set
    End Property

    Public Property Ore As Integer
        Get
            Return _Ore
        End Get
        Set(value As Integer)
            _Ore = value
        End Set
    End Property

    Public Property Minuti As Integer
        Get
            Return _Minuti
        End Get
        Set(value As Integer)
            _Minuti = value
        End Set
    End Property

    Public Property Codice As String
        Get
            Return _Codice
        End Get
        Set(value As String)
            _Codice = value
        End Set
    End Property

    Public Property Lotto As String
        Get
            Return _Lotto
        End Get
        Set(value As String)
            _Lotto = value
        End Set
    End Property

    Public Property Costo_Unitario As String
        Get
            Return _Costo_Unitario
        End Get
        Set(value As String)
            _Costo_Unitario = value
        End Set
    End Property

    Public Property Udm_Selezionata As Integer
        Get
            Return _Udm_Selezionata
        End Get
        Set(value As Integer)
            _Udm_Selezionata = value
        End Set
    End Property

    Public Property Valore As String
        Get
            Return _Valore
        End Get
        Set(value As String)
            _Valore = value
        End Set
    End Property

    Public Property ID_Attivita As Integer
        Get
            Return _ID_Attivita
        End Get
        Set(value As Integer)
            _ID_Attivita = value
        End Set
    End Property

    Public Property Turno_Cod As Integer
        Get
            Return _Turno_Cod
        End Get
        Set(value As Integer)
            _Turno_Cod = value
        End Set
    End Property

    Public Property Qualifica_Cod As Integer
        Get
            Return _Qualifica_Cod
        End Get
        Set(value As Integer)
            _Qualifica_Cod = value
        End Set
    End Property

    Public Property Tariffa_Cod As Integer
        Get
            Return _Tariffa_Cod
        End Get
        Set(value As Integer)
            _Tariffa_Cod = value
        End Set
    End Property

    Public Property Cod_Rapporto As Integer
        Get
            Return _Cod_Rapporto
        End Get
        Set(value As Integer)
            _Cod_Rapporto = value
        End Set
    End Property

    Public Shared Function ListaDiCostiAccessoriToDatatable(ByVal ListaDiCostiAccessori As List(Of CostiAccessori)) As DataTable


        Return AgronicaCoreDataProvider.Oggetti_DatatableUtility.CreateDataTable(Of CostiAccessori)(ListaDiCostiAccessori)

    End Function


    Public Shared Function Costruisci_DT_Scarico() As DataTable


        Dim dt As DataTable
        dt = New DataTable("dtScarico")
        dt.Columns.Add("kendoKey", System.Type.GetType("System.String"))
        dt.Columns.Add("Centro", System.Type.GetType("System.String"))
        dt.Columns.Add("Sa_Cod", System.Type.GetType("System.Int32"))
        dt.Columns.Add("Centro_Cod", System.Type.GetType("System.Int32"))
        dt.Columns.Add("Categoria_Des", System.Type.GetType("System.String"))
        dt.Columns.Add("Risorsa_Des", System.Type.GetType("System.String"))
        dt.Columns.Add("Udm_Des", System.Type.GetType("System.String"))
        dt.Columns.Add("Qta_Ril", System.Type.GetType("System.Decimal"))
        dt.Columns.Add("Udm_Cod", System.Type.GetType("System.Int32"))
        dt.Columns.Add("Elem_Cod", System.Type.GetType("System.Int32"))
        dt.Columns.Add("Riga", System.Type.GetType("System.Int32"))
        dt.Columns.Add("Tipo_Centro", System.Type.GetType("System.String"))
        dt.Columns.Add("Pro_Cod", System.Type.GetType("System.Int32"))
        dt.Columns.Add("Ditta_Cod", System.Type.GetType("System.Int32"))
        dt.Columns.Add("Mat_Cod", System.Type.GetType("System.Int32"))
        dt.Columns.Add("Costo_Unitario", System.Type.GetType("System.String"))
        dt.Columns.Add("Costo", System.Type.GetType("System.String"))
        dt.Columns.Add("Ore", System.Type.GetType("System.Int32"))
        dt.Columns.Add("Minuti", System.Type.GetType("System.Int32"))
        dt.Columns.Add("Codice", System.Type.GetType("System.String"))
        dt.Columns.Add("Lotto", System.Type.GetType("System.String"))


        'per gli input degli utenti
        dt.Columns.Add("Udm_Selezionata", System.Type.GetType("System.String"))
        dt.Columns.Add("Valore", System.Type.GetType("System.String"))
        dt.Columns.Add("ID_Attivita", System.Type.GetType("System.Int32"))
        dt.Columns.Add("Turno_Cod", System.Type.GetType("System.Int32"))

        dt.Columns.Add("Qualifica_Cod", System.Type.GetType("System.Int32"))
        dt.Columns.Add("Tariffa_Cod", System.Type.GetType("System.Int32"))
        dt.Columns.Add("Cod_Rapporto", System.Type.GetType("System.Int32"))

        Return dt

    End Function

End Class
