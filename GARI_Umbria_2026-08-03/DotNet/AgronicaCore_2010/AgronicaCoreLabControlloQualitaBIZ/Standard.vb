Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreLabControlloQualitaDAL

Public Class Standard_R
    Public Function leggiListaStandard( _
                              ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                              Optional ByVal Modello_Codice As String = Nothing, _
                              Optional ByVal Articolo_Cod As String = Nothing _
                              ) As List(Of String)

        Dim s As New AgronicaCoreLabControlloQualitaDAL.Standard_R
        Dim listaStandard As New List(Of String)
        Dim dt As DataTable

        dt = s.Leggi_Standard(Nothing, Modello_Codice, Articolo_Cod, "", "", objParametri)

        For Each row As DataRow In dt.Rows
            listaStandard.Add(row("Nome") & " " & row("Revisione"))
        Next

        Return listaStandard

    End Function

    Public Function leggi_LCQ_Standard( _
                              ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                              Optional ByVal Standard_Nome As String = Nothing, _
                              Optional ByVal Modello_Codice As String = Nothing, _
                              Optional ByVal Articolo_Cod As String = Nothing _
                              ) As List(Of LCQ_Standard)

        Dim s As New AgronicaCoreLabControlloQualitaDAL.Standard_R
        Dim listaObjLCQ As New List(Of LCQ_Standard)

        Dim dt As DataTable = s.Leggi_Standard(Standard_Nome, Modello_Codice, Articolo_Cod, "", "", objParametri)

        For Each dRow As DataRow In dt.Rows
            Dim lcq As New LCQ_Standard( _
                                          dRow("PivaSuperUser"), _
                                          dRow("Nome"), _
                                          dRow("Modello_Codice"), _
                                          UtilityProvider.DBNullToNothing(dRow("Articolo_Cod")) _
                                    )
            listaObjLCQ.Add(lcq)
        Next

        Return listaObjLCQ

    End Function

    Function test_StdEsiste( _
                              ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                              ByVal Standard_Nome As String, _
                              ByVal Modello_Codice As String _
                              ) As Boolean

        Dim s As New AgronicaCoreLabControlloQualitaDAL.Standard_R
        Dim res As Boolean = True

        res = s.Verifica_StdEsiste(Standard_Nome, Modello_Codice, objParametri)

        Return res
    End Function

End Class

Public Class Standard_W

    Public Function aggiungi(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                           ByVal standard As LCQ_Standard
                              ) As Boolean

        Dim s As New AgronicaCoreLabControlloQualitaDAL.Standard_W
        Dim res As Boolean = False

        res = s.Scrivi(objParametri, _
                       standard.Nome,  standard.Modello_Codice, _
                       standard.Articolo_Cod)

        Return res

    End Function

    Public Function aggiungi(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                            ByVal Nome As String, _
                            ByVal Modello_Codice As String, _
                            ByVal Articolo_Cod As String _
                              ) As Boolean

        Dim s As New AgronicaCoreLabControlloQualitaDAL.Standard_W
        Dim res As Boolean = False

        res = s.Scrivi(objParametri, _
                       Nome, Modello_Codice, _
                       Articolo_Cod)

        Return res

    End Function

    Public Function modifica(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                                ByVal Old_Nome As String, _
                                ByVal Old_Modello_Codice As String, _
                                ByVal New_Nome As String, _
                                ByVal New_Articolo_Cod As String _
                              ) As Boolean

        Dim s As New AgronicaCoreLabControlloQualitaDAL.Standard_W
        Dim res As Boolean = False

        res = s.Modifica(objParametri, Old_Nome, _
                            Old_Modello_Codice, _
                            New_Nome, New_Articolo_Cod)

        Return res

    End Function

End Class

Public Class LCQ_Standard

    Private _PivaSuperUser As String
    Private _Nome As String
    Private _Modello_Codice As String
    Private _Articolo_Cod As String

    Public Sub New()
        _PivaSuperUser = Nothing
        _Nome = Nothing
        _Modello_Codice = Nothing
        _Articolo_Cod = Nothing
    End Sub

    Public Sub New(PivaSuperUser As String, _
                     Nome As String, _
                     Modello_Codice As String, _
                     Articolo_Cod As String)

        _PivaSuperUser = PivaSuperUser
        _Nome = Nome
        _Modello_Codice = Modello_Codice
        _Articolo_Cod = Articolo_Cod
    End Sub

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


    Public Property Modello_Codice() As String
        Get
            Return _Modello_Codice
        End Get
        Set(ByVal value As String)
            _Modello_Codice = value
        End Set
    End Property

    Public Property Articolo_Cod() As String
        Get
            Return _Articolo_Cod
        End Get
        Set(ByVal value As String)
            _Articolo_Cod = value
        End Set
    End Property

End Class