
Imports AgronicaCoreModello.Anagrafe

Public Class clsImpresa
    Inherits clsDatiImpresa

    Private _impianti As New List(Of Impianto_Colturale)
    Public Property Impianti As List(Of Impianto_Colturale)
        Get
            Return _impianti
        End Get
        Set(value As List(Of Impianto_Colturale))
            _impianti = value
        End Set
    End Property


    Private _RagioneSociale As String
    Public Property RagioneSociale() As String
        Get
            Return _RagioneSociale
        End Get
        Set(value As String)
            _RagioneSociale = value
        End Set
    End Property

    Private _Opzionale_Sa_Cod_Origine As Integer
    Public Property Opzionale_Sa_Cod_Origine() As Integer
        Get
            Return _Opzionale_Sa_Cod_Origine
        End Get
        Set(value As Integer)
            _Opzionale_Sa_Cod_Origine = value
        End Set
    End Property

    Private _Piva_ORIGINE As String
    Private _Piva_DESTINAZIONE As String
    Private _PivaPadre_DESTINAZIONE As String

    Private _PivaSuperUser_ORIGINE As String
    Private _PivaSuperUser_DESTINAZIONE As String


    Private _flagAccodaDatiSeEsistePivaDestinazione As Boolean
    Public Property FlagAccodaDatiSeEsistePivaDestinazione() As Boolean
        Get
            Return _flagAccodaDatiSeEsistePivaDestinazione
        End Get
        Set(value As Boolean)
            _flagAccodaDatiSeEsistePivaDestinazione = value
        End Set
    End Property


    Private _obj_Anagrafica As cls_Anagrafica
    Public Property Obj_Anagrafica() As cls_Anagrafica
        Get
            Return _obj_Anagrafica
        End Get
        Set(value As cls_Anagrafica)
            _obj_Anagrafica = value
        End Set
    End Property
    Private _obj_Magazzino As cls_Magazzino
    Public Property Obj_Magazzino() As cls_Magazzino
        Get
            Return _obj_Magazzino
        End Get
        Set(value As cls_Magazzino)
            _obj_Magazzino = value
        End Set
    End Property

    Private _Profilazione As cls_Magazzino
    Private _AnalisiTerreno As cls_Magazzino
    Private _PianoConcimazione As cls_Magazzino
    Private _fabbricati As New List(Of Magazzino)()


#Region "Costruttori"

    Public Sub New()
        _Piva_ORIGINE = ""
        _Piva_DESTINAZIONE = ""
        _PivaPadre_DESTINAZIONE = ""

        _PivaSuperUser_ORIGINE = ""
        _PivaSuperUser_DESTINAZIONE = ""

    End Sub

#End Region

#Region "Proprieta"

    Public Property Piva_ORIGINE() As String
        Get
            Return _Piva_ORIGINE
        End Get
        Set(ByVal value As String)
            _Piva_ORIGINE = value
        End Set
    End Property

    Public Property Piva_DESTINAZIONE() As String
        Get
            Return _Piva_DESTINAZIONE
        End Get
        Set(ByVal value As String)
            _Piva_DESTINAZIONE = value
        End Set
    End Property

    Public Property PivaPadre_DESTINAZIONE() As String
        Get
            Return _PivaPadre_DESTINAZIONE
        End Get
        Set(ByVal value As String)
            _PivaPadre_DESTINAZIONE = value
        End Set
    End Property

    Public Property PivaSuperUser_ORIGINE() As String
        Get
            Return _PivaSuperUser_ORIGINE
        End Get
        Set(ByVal value As String)
            _PivaSuperUser_ORIGINE = value
        End Set
    End Property

    Public Property PivaSuperUser_DESTINAZIONE() As String
        Get
            Return _PivaSuperUser_DESTINAZIONE
        End Get
        Set(ByVal value As String)
            _PivaSuperUser_DESTINAZIONE = value
        End Set
    End Property

    Public Property Fabbricati As List(Of Magazzino)
        Get
            Return _fabbricati
        End Get
        Set(value As List(Of Magazzino))
            _fabbricati = value
        End Set
    End Property

#End Region

End Class
