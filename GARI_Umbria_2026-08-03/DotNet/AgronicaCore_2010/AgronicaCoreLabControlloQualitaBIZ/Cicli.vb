Imports AgronicaCoreDataProvider
Imports AgronicaCoreLabControlloQualitaDAL


Public Class Cicli_R

    Public Function leggi_ListaCicli( _
                              ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                              ) As List(Of String)

        Dim cf As New AgronicaCoreLabControlloQualitaDAL.Cicli_R
        Dim listaCicli As New List(Of String)

        Dim dt As DataTable = cf.Leggi_Cicli(Nothing, "", "Nome", objParametri)

        For Each dRow As DataRow In dt.Rows
            listaCicli.Add(dRow("Nome"))
        Next

        Return listaCicli

    End Function


    Public Function leggi_LCQ_Cicli( _
                              ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                              Optional ByVal Nome As String = Nothing
                              ) As List(Of LCQ_Cicli)

        Dim cf As New AgronicaCoreLabControlloQualitaDAL.Cicli_R
        Dim listaObjLCQ As New List(Of LCQ_Cicli)

        Dim dt As DataTable = cf.Leggi_Cicli(Nome, "", "", objParametri)

        For Each dRow As DataRow In dt.Rows
            Dim lcq As New LCQ_Cicli( _
                                          dRow("PivaSuperUser"), _
                                          dRow("Nome") _
                                          )
            listaObjLCQ.Add(lcq)
        Next

        Return listaObjLCQ

    End Function

End Class


Public Class Cicli_W

    Public Function modifica(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                               ByVal Old_Nome As String, _
                               ByVal New_Nome As String _
                             ) As Boolean

        Dim cf As New AgronicaCoreLabControlloQualitaDAL.Cicli_W
        Dim res As Boolean = False

        res = cf.Modifica(objParametri, Old_Nome, New_Nome)

        Return res

    End Function

End Class

Public Class LCQ_Cicli

    Private _PivaSuperUser As String
    Private _Nome As String

    Public Sub New()
        _PivaSuperUser = Nothing
        _Nome = Nothing
    End Sub

    Public Sub New(PivaSuperUser As String, _
                    Nome As String)

        _PivaSuperUser = PivaSuperUser
        _Nome = Nome
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

End Class
