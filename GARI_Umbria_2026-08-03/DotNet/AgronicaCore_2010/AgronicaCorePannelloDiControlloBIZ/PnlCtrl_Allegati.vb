Imports AgronicaCoreDataProvider
Imports AgronicaCorePannelloDiControlloDAL

Public Class Allegati_R

    Public Function leggi_PnlCtrl_AllegatiDaIDLista( _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                                ByVal ID_Lista As Integer _
                              ) As List(Of PnlCtrl_Allegati)

        Return leggi_PnlCtrl_Allegati(objParametri, Nothing, ID_Lista, Nothing, Nothing, Nothing)

    End Function

    Public Function leggi_PnlCtrl_Allegati( _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                                ByVal ID As Integer _
                              ) As List(Of PnlCtrl_Allegati)

        Return leggi_PnlCtrl_Allegati(objParametri, ID, Nothing, Nothing, Nothing, Nothing)

    End Function

    Public Function leggi_PnlCtrl_Allegati( _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                                ByVal ID As Integer?, _
                                ByVal ID_Lista As Integer?, _
                                ByVal NomeFile As String, _
                                ByVal Descrizione As String, _
                                ByVal ID_Categoria As Integer? _
                              ) As List(Of PnlCtrl_Allegati)

        Dim r As New AgronicaCorePannelloDiControlloDAL.Allegati_R
        Dim listaObjPnlCtrl As New List(Of PnlCtrl_Allegati)

        Dim dt As DataTable = r.Leggi_Allegati(ID, ID_Lista, NomeFile, Descrizione, ID_Categoria, "", "", objParametri)

        For Each dRow As DataRow In dt.Rows
            Dim elem As New PnlCtrl_Allegati( _
                                          UtilityProvider.DBNullToNothing(dRow("ID")), _
                                          UtilityProvider.DBNullToNothing(dRow("ID_Lista")), _
                                          UtilityProvider.DBNullToNothing(dRow("NomeFile")), _
                                          UtilityProvider.DBNullToNothing(dRow("Descrizione")), _
                                          UtilityProvider.DBNullToNothing(dRow("ID_Categoria"))
                                          )
            listaObjPnlCtrl.Add(elem)
        Next

        Return listaObjPnlCtrl

    End Function

End Class

Public Class Allegati_W

    Public Function aggiungi(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                           ByVal allegato As PnlCtrl_Allegati
                              ) As Boolean

        Dim w As New AgronicaCorePannelloDiControlloDAL.Allegati_W
        Dim res As Boolean = False

        res = w.Scrivi(objParametri, _
                       allegato.ID, allegato.ID_Lista, allegato.NomeFile, _
                       allegato.Descrizione, allegato.ID_Categoria)

        Return res

    End Function

    Public Function aggiungi(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                            ByVal ID As Integer, _
                            ByVal ID_Lista As Integer, _
                            ByVal NomeFile As String, _
                            ByVal Descrizione As String, _
                            ByVal ID_Categoria As Integer _
                              ) As Boolean

        Dim w As New AgronicaCorePannelloDiControlloDAL.Allegati_W
        Dim res As Boolean = False

        res = w.Scrivi(objParametri, _
                       ID, ID_Lista, NomeFile, Descrizione, ID_Categoria)

        Return res

    End Function

    Public Function modifica(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                                ByVal Old_ID As Integer, _
                                ByVal New_ID_Lista As Integer, _
                                ByVal New_NomeFile As String, _
                                ByVal New_Descrizione As String, _
                                ByVal New_ID_Categoria As Integer _
                              ) As Boolean

        Dim w As New AgronicaCorePannelloDiControlloDAL.Allegati_W
        Dim res As Boolean = False

        res = w.Modifica(objParametri, Old_ID, New_ID_Lista, _
                         New_NomeFile, New_Descrizione, New_ID_Categoria)

        Return res

    End Function

End Class

Public Class PnlCtrl_Allegati
    Public Property ID As Integer?
    Public Property ID_Lista As Integer?
    Public Property NomeFile As String
    Public Property Descrizione As String
    Public Property ID_Categoria As Integer?

    Public Sub New(ID As Integer?, _
                    ID_Lista As Integer?, _
                    NomeFile As String, _
                    Descrizione As String, _
                    ID_Categoria As Integer? _
                    )

        _ID = ID
        _ID_Lista = ID_Lista
        _NomeFile = NomeFile
        _Descrizione = Descrizione
        _ID_Categoria = ID_Categoria
    End Sub

    Public Sub New()
        _ID = Nothing
        _ID_Lista = Nothing
        _NomeFile = ""
        _Descrizione = ""
        _ID_Categoria = Nothing
    End Sub


End Class
