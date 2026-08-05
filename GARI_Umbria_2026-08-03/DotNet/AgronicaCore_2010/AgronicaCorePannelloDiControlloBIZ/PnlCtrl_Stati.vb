Imports AgronicaCoreDataProvider
Imports AgronicaCorePannelloDiControlloDAL

Public Class Stati_R

    Public Function leggi_PnlCtrl_Stati( _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                              ) As List(Of PnlCtrl_Stati)

        Return leggi_PnlCtrl_Stati(objParametri, Nothing, Nothing)
    End Function

    Public Function leggi_PnlCtrl_Stati( _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                                ByVal ID As Integer?, _
                                ByVal Nome As String _
                              ) As List(Of PnlCtrl_Stati)

        Dim r As New AgronicaCorePannelloDiControlloDAL.Stati_R
        Dim listaObjPnlCtrl As New List(Of PnlCtrl_Stati)

        Dim dt As DataTable = r.Leggi_Stati(ID, Nome, "", "", objParametri)

        For Each dRow As DataRow In dt.Rows
            Dim elem As New PnlCtrl_Stati( _
                                          UtilityProvider.DBNullToNothing(dRow("ID")), _
                                          UtilityProvider.DBNullToNothing(dRow("Nome")) _
                                          )
            listaObjPnlCtrl.Add(elem)
        Next

        Return listaObjPnlCtrl

    End Function

End Class

Public Class Stati_W

End Class

Public Class PnlCtrl_Stati
    Public Property ID As Integer?
    Public Property Nome As String

    Public Sub New(ID As Integer?, _
                    Nome As String)
        _ID = ID
        _Nome = Nome
    End Sub

    Public Sub New()
        _ID = Nothing
        _Nome = ""
    End Sub

End Class