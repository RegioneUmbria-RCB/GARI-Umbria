Imports AgronicaCoreDataProvider
Imports AgronicaCoreLabControlloQualitaDAL

Public Class ParametriTipi_R

    Public Function leggi_LCQ_ParametriTipi( _
                              ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                              Optional ByVal CodTipo As Integer? = Nothing _
                              ) As List(Of LCQ_ParametriTipi)

        Dim pt As New AgronicaCoreLabControlloQualitaDAL.ParametriTipi_R
        Dim listaObjLCQ As New List(Of LCQ_ParametriTipi)

        Dim dt As DataTable = pt.Leggi(CodTipo, Nothing, Nothing, "", "", objParametri)

        For Each dRow As DataRow In dt.Rows
            Dim lcq As New LCQ_ParametriTipi( _
                                          dRow("CodTipo"), _
                                          UtilityProvider.DBNullToNothing(dRow("PivaSuperUser")), _
                                          dRow("Nome"), _
                                          UtilityProvider.DBNullToNothing(dRow("Descrizione")), _
                                          dRow("TipoVB") _
                                          )
            listaObjLCQ.Add(lcq)
        Next

        Return listaObjLCQ

    End Function

End Class

Public Class LCQ_ParametriTipi

    Private _CodTipo As Integer?
    Private _PivaSuperUser As String
    Private _Nome As String
    Private _Descrizione As String
    Private _TipoVB As Char

    Public Sub New()
        _CodTipo = Nothing
        _PivaSuperUser = Nothing
        _Nome = Nothing
        _Descrizione = Nothing
        _TipoVB = Nothing
    End Sub

    Public Sub New(CodTipo As Integer?, _
                    PivaSuperUser As String, _
                    Nome As String, _
                    Descrizione As String, _
                    TipoVB As Char)

        _CodTipo = CodTipo
        _PivaSuperUser = PivaSuperUser
        _Nome = Nome
        _Descrizione = Descrizione
        _TipoVB = TipoVB
    End Sub

    Public Property CodTipo() As Integer?
        Get
            Return _CodTipo
        End Get
        Set(ByVal value As Integer?)
            _CodTipo = value
        End Set
    End Property

    Public Property PivaSuperUser() As String
        Get
            Return _PivaSuperUser
        End Get
        Set(ByVal value As String)
            _PivaSuperUser = value
        End Set
    End Property

    Public Property Nome() As String
        Get
            Return _Nome
        End Get
        Set(ByVal value As String)
            _Nome = value
        End Set
    End Property

    Public Property Descrizione() As String
        Get
            Return _Descrizione
        End Get
        Set(ByVal value As String)
            _Descrizione = value
        End Set
    End Property

    Public Property TipoVB() As String
        Get
            Return _TipoVB
        End Get
        Set(ByVal value As String)
            _TipoVB = value
        End Set
    End Property

End Class
