
'''<remarks/>
<System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1"),
 System.SerializableAttribute(),
 System.Diagnostics.DebuggerStepThroughAttribute(),
 System.ComponentModel.DesignerCategoryAttribute("code")>
Partial Public Class DettaglioExportLog

    Private NomeFile_Field As String
    Private Descrizione_Field As String
    Private TipoOperazione_Field As String
    Private KeyAgenda_Field As String
    Private KeyDettaglio_Field As String
    Private KeyExport_Field As String
    Private TipoRisorsa_Field As String
    Private Cod_Contatto_Field As String
    Private Cod_Risum_Field As Integer
    Private Mac_Cod_Field As Integer
    Private Elem_Cod_Field As Integer
    Private Pro_Cod_Field As Integer
    Private Mat_Cod_Field As Integer
    Private Lotto_Field As String
    Private UdmGias_Field As Integer
    Private UdmExport_Field As String
    Private Qta_Field As Decimal
    Private Data_Log_Field As DateTime
    Private Note_Field As String

    Public Sub New()
        MyBase.New
        Me.NomeFile_Field = ""
        Me.Descrizione_Field = ""
        Me.TipoOperazione_Field = ""
        Me.KeyAgenda_Field = ""
        Me.KeyDettaglio_Field = ""
        Me.KeyExport_Field = ""
        Me.TipoOperazione_Field = ""
        Me.Cod_Contatto_Field = ""
        Me.Cod_Risum_Field = 0
        Me.Mac_Cod_Field = 0
        Me.Elem_Cod_Field = 0
        Me.Pro_Cod_Field = 0
        Me.Mat_Cod_Field = 0
        Me.Lotto_Field = ""
        Me.UdmGias_Field = 0
        Me.UdmExport_Field = ""
        Me.Qta_Field = 0
        Me.Note_Field = ""

    End Sub

    '''<remarks/>
    Public Property NomeFile() As String
        Get
            Return Me.NomeFile_Field
        End Get
        Set
            Me.NomeFile_Field = Value
        End Set
    End Property

    Public Property Descrizione() As String
        Get
            Return Me.Descrizione_Field
        End Get
        Set
            Me.Descrizione_Field = Value
        End Set
    End Property

    Public Property TipoOperazione() As String
        Get
            Return Me.TipoOperazione_Field
        End Get
        Set
            Me.TipoOperazione_Field = Value
        End Set
    End Property
    Public Property KeyAgenda() As String
        Get
            Return Me.KeyAgenda_Field
        End Get
        Set
            Me.KeyAgenda_Field = Value
        End Set
    End Property
    Public Property KeyDettaglio() As String
        Get
            Return Me.KeyDettaglio_Field
        End Get
        Set
            Me.KeyDettaglio_Field = Value
        End Set
    End Property

    Public Property KeyExport() As String
        Get
            Return Me.KeyExport_Field
        End Get
        Set
            Me.KeyExport_Field = Value
        End Set
    End Property
    Public Property TipoRisorsa() As String
        Get
            Return Me.TipoRisorsa_Field
        End Get
        Set
            Me.TipoRisorsa_Field = Value
        End Set
    End Property
    Public Property Cod_Contatto() As String
        Get
            Return Me.Cod_Contatto_Field
        End Get
        Set
            Me.Cod_Contatto_Field = Value
        End Set
    End Property
    Public Property Cod_Risum() As Integer
        Get
            Return Me.Cod_Risum_Field
        End Get
        Set
            Me.Cod_Risum_Field = Value
        End Set
    End Property
    Public Property Mac_Cod() As Integer
        Get
            Return Me.Mac_Cod_Field
        End Get
        Set
            Me.Mac_Cod_Field = Value
        End Set
    End Property
    Public Property Elem_Cod() As Integer
        Get
            Return Me.Elem_Cod_Field
        End Get
        Set
            Me.Elem_Cod_Field = Value
        End Set
    End Property
    Public Property Pro_Cod() As Integer
        Get
            Return Me.Pro_Cod_Field
        End Get
        Set
            Me.Pro_Cod_Field = Value
        End Set
    End Property
    Public Property Mat_Cod() As Integer
        Get
            Return Me.Mat_Cod_Field
        End Get
        Set
            Me.Mat_Cod_Field = Value
        End Set
    End Property
    Public Property Lotto() As String
        Get
            Return Me.Lotto_Field
        End Get
        Set
            Me.Lotto_Field = Value
        End Set
    End Property
    Public Property UdmGias() As Integer
        Get
            Return Me.UdmGias_Field
        End Get
        Set
            Me.UdmGias_Field = Value
        End Set
    End Property
    Public Property UdmExport() As String
        Get
            Return Me.UdmExport_Field
        End Get
        Set
            Me.UdmExport_Field = Value
        End Set
    End Property
    Public Property Qta() As Decimal
        Get
            Return Me.Qta_Field
        End Get
        Set
            Me.Qta_Field = Value
        End Set
    End Property
    Public Property Note() As String
        Get
            Return Me.Note_Field
        End Get
        Set
            Me.Note_Field = Value
        End Set
    End Property

End Class