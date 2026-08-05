Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCorePannelloDiControlloDAL
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

Public Class NonConformita_R

    Public Function leggi_NonConformita( _
                                ByRef objParametri As AgronicaCoreParametri, _
                                ByVal ID_NC As Integer _
                                ) As List(Of PnlCtrl_NonConformita)

        Return leggi_NonConformita(objParametri, ID_NC, Nothing, Nothing, Nothing)
    End Function

    Public Function leggi_NonConformita( _
                                ByRef objParametri As AgronicaCoreParametri, _
                                ByVal ID_NC As Integer?, _
                                ByVal ID_Categoria As Integer?, _
                                ByVal Piva As String, _
                                ByVal ID_Gravita As Integer? _
                              ) As List(Of PnlCtrl_NonConformita)

        Dim NC_r As New AgronicaCorePannelloDiControlloDAL.NonConformita_R
        Dim NC_Det_r As New AgronicaCorePannelloDiControlloBIZ.NonConformita_Dettagli_R
        Dim listaObjPnlCtrl As New List(Of PnlCtrl_NonConformita)

        'leggo le NC che rispondono ai parametri passati
        Dim dt_NC As DataTable = NC_r.Leggi_NonConformita(ID_NC, ID_Categoria, Piva, _
                                               ID_Gravita, "", "", objParametri)

        For Each dRow As DataRow In dt_NC.Rows

            'Leggo i dettagli della NC e se ci sono, li aggiungo
            Dim lista_NC_Det As List(Of PnlCtrl_NonConformita_Dettagli) = Nothing
            If Not IsDBNull(dRow("ID_NC")) Then
                lista_NC_Det = NC_Det_r.leggi_NonConformita_Dettagli(objParametri, ID_NC)
            End If

            Dim elem As New PnlCtrl_NonConformita( _
                                          UtilityProvider.DBNullToNothing(dRow("ID_NC")), _
                                          UtilityProvider.DBNullToNothing(dRow("ID_Categoria")), _
                                          UtilityProvider.DBNullToNothing(dRow("Piva")), _
                                          UtilityProvider.DBNullToNothing(dRow("TabDettaglio_Nome")), _
                                          UtilityProvider.DBNullToNothing(dRow("TabDettaglio_Chiave")), _
                                          UtilityProvider.DBNullToNothing(dRow("ID_Gravita")), _
                                          lista_NC_Det
                                          )
            listaObjPnlCtrl.Add(elem)
        Next

        Return listaObjPnlCtrl

    End Function

End Class

Public Class NonConformita_W

    Public Function aggiungi(ByRef objParametri As AgronicaCoreParametri, _
                            ByVal nc As PnlCtrl_NonConformita
                            ) As Boolean

        Dim res As Boolean = aggiungi(objParametri, _
                                    nc.ID_NC, nc.ID_Categoria, nc.Piva, nc.ID_Gravita, _
                                    nc.TabDettaglio_Nome, nc.TabDettaglio_Chiave)

        Return res

    End Function

    Public Function aggiungi(ByRef objParametri As AgronicaCoreParametri, _
                                ByVal ID_NC As Integer, _
                                ByVal ID_Categoria As Integer, _
                                ByVal Piva As String, _
                                ByVal ID_Gravita As Integer?, _
                                ByVal TabDettaglio_Nome As String, _
                                ByVal TabDettaglio_Chiave As String _
                              ) As Boolean

        Dim w As New AgronicaCorePannelloDiControlloDAL.NonConformita_W
        Dim res As Boolean = False

        res = w.Scrivi(objParametri, ID_NC, ID_Categoria, Piva, _
                         ID_Gravita, TabDettaglio_Nome, TabDettaglio_Chiave)

        Return res

    End Function

    Public Function modifica(ByRef objParametri As AgronicaCoreParametri, _
                                ByVal Old_ID_NC As Integer, _
                                ByVal New_ID_Categoria As Integer, _
                                ByVal New_Piva As String, _
                                ByVal New_TabDettaglio_Nome As String, _
                                ByVal New_TabDettaglio_Chiave As String, _
                                ByVal New_ID_Gravita As Integer? _
                              ) As Boolean

        Dim w As New AgronicaCorePannelloDiControlloDAL.NonConformita_W
        Dim res As Boolean = w.Modifica(objParametri, Old_ID_NC, New_ID_Categoria, New_Piva, _
                         New_ID_Gravita, New_TabDettaglio_Nome, New_TabDettaglio_Chiave)

        Return res

    End Function

    Public Function modifica(ByRef objParametri As AgronicaCoreParametri, _
                                ByVal e As PnlCtrl_NonConformita _
                              ) As Boolean

        Dim res As Boolean = modifica(objParametri, e.ID_NC, e.ID_Categoria, e.Piva, _
                         e.TabDettaglio_Nome, e.TabDettaglio_Chiave, e.ID_Gravita)

        Return res

    End Function

    Public Function cancellaNC(ByRef objParametri As AgronicaCoreParametri, _
                                            ByVal ID_NC As Integer _
                                           ) As Boolean

        Dim elemW As New AgronicaCorePannelloDiControlloDAL.NonConformita_W
        Dim res = elemW.CancellaNonConformita("", ID_NC, objParametri)

        Return res
    End Function

End Class

Public Class PnlCtrl_NonConformita
    Public Property ID_NC As Integer
    Public Property ID_Categoria As Integer
    Public Property Piva As String
    Public Property TabDettaglio_Nome As String
    Public Property TabDettaglio_Chiave As String
    Public Property ID_Gravita As Integer?
    Public Property Dettagli As List(Of PnlCtrl_NonConformita_Dettagli)

    Public Sub New( _
                    ID_NC As Integer, _
                    ID_Categoria As Integer, _
                    Piva As String, _
                    TabDettaglio_Nome As String, _
                    TabDettaglio_Chiave As String, _
                    ID_Gravita As Integer?, _
                    Dettagli As List(Of PnlCtrl_NonConformita_Dettagli))

        _ID_NC = ID_NC
        _ID_Categoria = ID_Categoria
        _Piva = Piva
        _TabDettaglio_Nome = TabDettaglio_Nome
        _TabDettaglio_Chiave = TabDettaglio_Chiave
        _ID_Gravita = ID_Gravita
        _Dettagli = Dettagli

    End Sub

    Public Sub New()
        _ID_NC = -1
        _ID_Categoria = -1
        _Piva = ""
        _TabDettaglio_Nome = Nothing
        _TabDettaglio_Chiave = Nothing
        _ID_Gravita = Nothing
        _Dettagli = Nothing
    End Sub

    Public Sub New(obj As JObject)
        'JObject jObject = JObject.Parse(json);
        'JToken jUser = jObject["user"];
        'name = (string) jUser["name"];
        'teamname = (string) jUser["teamname"];
        'email = (string) jUser["email"];
        'players = jUser["players"].ToArray();
        _ID_NC = CInt(obj("ID_NC"))
        _ID_Categoria = CInt(obj("ID_Categoria"))
        _Piva = obj("Piva")
        _TabDettaglio_Nome = obj("TabDettaglio_Nome")
        _TabDettaglio_Chiave = obj("TabDettaglio_Chiave")
        _ID_Gravita = CInt(obj("ID_Gravita"))

        Dim objDet As JObject() = obj("Dettagli").ToArray()
        Dim listaDet As New List(Of PnlCtrl_NonConformita_Dettagli)
        For Each det In objDet
            listaDet.Add(New PnlCtrl_NonConformita_Dettagli(det))
        Next
        _Dettagli = listaDet
    End Sub



End Class
