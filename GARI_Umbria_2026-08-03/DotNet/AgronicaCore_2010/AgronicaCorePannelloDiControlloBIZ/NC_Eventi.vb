Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports System.Web.UI.WebControls

Public Class NC_Eventi_R

    Public Function leggi_NC_Eventi_ToListOfListItem( _
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                          ) As List(Of ListItem)

        Dim objNC As List(Of NC_Eventi) = leggi_NC_Eventi(objParametri, Nothing, Nothing)
        Dim listNC As New List(Of ListItem)

        For Each elem As NC_Eventi In objNC
            listNC.Add(New ListItem(elem.Nome, elem.ID_Evento))
        Next

        Return listNC

    End Function

    Public Function leggi_NC_Eventi( _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                              ) As List(Of NC_Eventi)

        Return leggi_NC_Eventi(objParametri, Nothing, Nothing)
    End Function

    Public Function leggi_NC_Eventi( _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                                ByVal ID_Evento As Integer?, _
                                ByVal Nome As String _
                              ) As List(Of NC_Eventi)

        Dim r As New AgronicaCorePannelloDiControlloDAL.NC_Eventi_R
        Dim listaObjPnlCtrl As New List(Of NC_Eventi)

        Dim dt As DataTable = r.Leggi(ID_Evento, Nome, "", "", objParametri)

        For Each dRow As DataRow In dt.Rows
            Dim elem As New NC_Eventi( _
                                          UtilityProvider.DBNullToNothing(dRow("ID_Evento")), _
                                          UtilityProvider.DBNullToNothing(dRow("Nome")) _
                                          )
            listaObjPnlCtrl.Add(elem)
        Next

        Return listaObjPnlCtrl

    End Function

End Class

Public Class NC_Eventi_W

End Class

Public Class NC_Eventi
    Public Property ID_Evento As Integer
    Public Property Nome As String

    Public Sub New(ID_Evento As Integer, _
                    Nome As String)
        _ID_Evento = ID_Evento
        _Nome = Nome
    End Sub

    Public Sub New()
        _ID_Evento = 0
        _Nome = ""
    End Sub

End Class