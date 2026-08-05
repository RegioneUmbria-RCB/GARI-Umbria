Imports AgronicaCoreDataProvider
Imports AgronicaCorePannelloDiControlloDAL

Public Class Categorie_R

    Public Function leggi_PnlCtrl_Categorie( _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                              ) As List(Of PnlCtrl_Categorie)

        Dim listaObjPnlCtrl As List(Of PnlCtrl_Categorie) = leggi_PnlCtrl_Categorie( _
            objParametri, Nothing, Nothing, Nothing, Nothing)

        Return listaObjPnlCtrl

    End Function

    Public Function leggi_PnlCtrl_Categorie( _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                                ByVal ID As Integer _
                              ) As List(Of PnlCtrl_Categorie)

        Dim listaObjPnlCtrl As List(Of PnlCtrl_Categorie) = leggi_PnlCtrl_Categorie( _
            objParametri, ID, Nothing, Nothing, Nothing)

        Return listaObjPnlCtrl

    End Function

    Public Function leggi_PnlCtrl_Categorie( _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                                ByVal ID As Integer?, _
                                ByVal TipoCatgoria As Integer?, _
                                ByVal Area As String, _
                                ByVal Tipologia As String _
                              ) As List(Of PnlCtrl_Categorie)

        Dim r As New AgronicaCorePannelloDiControlloDAL.Categorie_R
        Dim listaObjPnlCtrl As New List(Of PnlCtrl_Categorie)

        Dim dt As DataTable = r.Leggi_Categorie(ID, TipoCatgoria, Area, Tipologia, "", "", objParametri)

        For Each dRow As DataRow In dt.Rows
            Dim elem As New PnlCtrl_Categorie( _
                                          UtilityProvider.DBNullToNothing(dRow("ID")), _
                                          UtilityProvider.DBNullToNothing(dRow("TipoCategoria")), _
                                          UtilityProvider.DBNullToNothing(dRow("Area")), _
                                          UtilityProvider.DBNullToNothing(dRow("Tipologia"))
                                          )
            listaObjPnlCtrl.Add(elem)
        Next

        Return listaObjPnlCtrl

    End Function

End Class

Public Class Categorie_W

End Class

Public Class PnlCtrl_Categorie
    Public Property ID As Integer
    Public Property TipoCatgoria As Integer
    Public Property Area As String
    Public Property Tipologia As String

    Public Sub New(ID As Integer?, _
                    TipoCatgoria As Integer?, _
                    Area As String, _
                    Tipologia As String)

        _ID = ID
        _TipoCatgoria = TipoCatgoria
        _Area = Area
        _Tipologia = Tipologia
    End Sub

End Class