Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCorePannelloDiControlloDAL
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

Public Class NonConformita_Dettagli_R

    Public Function leggi_DettaglioNC(ByRef objParametri As AgronicaCoreParametri, _
                                        ByVal ID_Dettaglio As Integer _
                                        ) As List(Of PnlCtrl_NonConformita_Dettagli)

        Return leggi_NonConformita_Dettagli(objParametri, ID_Dettaglio, Nothing, _
                                            Nothing, Nothing, Nothing, Nothing)
    End Function

    Public Function leggi_NonConformita_Dettagli(ByRef objParametri As AgronicaCoreParametri, _
                                                    ByVal ID_NC As Integer _
                                                    ) As List(Of PnlCtrl_NonConformita_Dettagli)

        Return leggi_NonConformita_Dettagli(objParametri, Nothing, ID_NC, Nothing, _
                                            Nothing, Nothing, Nothing)
    End Function

    Public Function leggi_NonConformita_Dettagli(ByRef objParametri As AgronicaCoreParametri, _
                                                    ByVal ID_Dettaglio As Integer?, _
                                                    ByVal ID_NC As Integer?, _
                                                    ByVal Utente As String, _
                                                    ByVal Data As DateTime?, _
                                                    ByVal ID_Stato As Integer?, _
                                                    ByVal ID_ListaAllegati As Integer? _
                                                  ) As List(Of PnlCtrl_NonConformita_Dettagli)

        Dim r As New AgronicaCorePannelloDiControlloDAL.NonConformita_Dettagli_R
        Dim listaObjPnlCtrl As New List(Of PnlCtrl_NonConformita_Dettagli)

        Dim dt As DataTable = r.Leggi_NonConformita_Dettagli(ID_Dettaglio, ID_NC, Utente, Data, _
                                                ID_Stato, ID_ListaAllegati, "", "", objParametri)

        For Each dRow As DataRow In dt.Rows
            Dim det As New PnlCtrl_NonConformita_Dettagli( _
                                          UtilityProvider.DBNullToNothing(dRow("ID_Dettaglio")), _
                                          UtilityProvider.DBNullToNothing(dRow("ID_NC")), _
                                          UtilityProvider.DBNullToNothing(dRow("Utente")), _
                                          UtilityProvider.DBNullToNothing(dRow("Data")), _
                                          UtilityProvider.DBNullToNothing(dRow("ID_Stato")), _
                                          UtilityProvider.DBNullToNothing(dRow("ID_ListaAllegati")), _
                                          UtilityProvider.DBNullToNothing(dRow("Descrizione")), _
                                          UtilityProvider.DBNullToNothing(dRow("Note")) _
                                          )
            listaObjPnlCtrl.Add(det)
        Next

        Return listaObjPnlCtrl

    End Function

End Class

Public Class NonConformita_Dettagli_W

    Public Function aggiungi(ByRef objParametri As AgronicaCoreParametri, _
                                ByVal det As PnlCtrl_NonConformita_Dettagli
                                ) As Boolean

        Dim res As Boolean = aggiungi(objParametri, _
                        det.ID_Dettaglio, det.ID_NC, det.Utente, det.Data, _
                        det.ID_Stato, det.ID_ListaAllegati, det.Descrizione, det.Note)

        Return res

    End Function

    Public Function aggiungi(ByRef objParametri As AgronicaCoreParametri, _
                                ByVal ID_Dettaglio As Integer, _
                                ByVal ID_NC As Integer, _
                                ByVal Utente As String, _
                                ByVal Data As DateTime, _
                                ByVal ID_Stato As Integer?, _
                                ByVal ID_ListaAllegati As Integer?, _
                                ByVal Descrizione As String, _
                                ByVal Note As String _
                              ) As Boolean

        Dim w As New AgronicaCorePannelloDiControlloDAL.NonConformita_Dettagli_W
        Dim res As Boolean = False

        res = w.Scrivi(objParametri, ID_Dettaglio, ID_NC, Utente, Data, _
                          ID_Stato, ID_ListaAllegati, Descrizione, Note)

        Return res

    End Function

    Public Function modifica(ByRef objParametri As AgronicaCoreParametri, _
                                ByVal Old_ID_Dettaglio As Integer, _
                                ByVal New_Utente As String, _
                                ByVal New_Data As DateTime?, _
                                ByVal New_ID_Stato As Integer?, _
                                ByVal New_ID_ListaAllegati As Integer?, _
                                ByVal New_Descrizione As String, _
                                ByVal New_Note As String _
                              ) As Boolean

        Dim w As New AgronicaCorePannelloDiControlloDAL.NonConformita_Dettagli_W
        Dim res As Boolean = w.Modifica(objParametri, Old_ID_Dettaglio, New_Utente, New_Data, _
                          New_ID_Stato, New_ID_ListaAllegati, New_Descrizione, New_Note)

        Return res

    End Function

    Public Function modifica(ByRef objParametri As AgronicaCoreParametri, _
                                ByVal NC_Det As PnlCtrl_NonConformita_Dettagli _
                              ) As Boolean

        Dim res As Boolean = modifica(objParametri, NC_Det.ID_Dettaglio, NC_Det.Utente, NC_Det.Data, _
                             NC_Det.ID_Stato, NC_Det.ID_ListaAllegati, NC_Det.Descrizione, NC_Det.Note)

        Return res

    End Function

    Public Function CancellaTuttiIDettagliDiNC(ByRef objParametri As AgronicaCoreParametri, _
                                            ByVal ID_NC As Integer _
                                           ) As Boolean

        Dim elemW As New AgronicaCorePannelloDiControlloDAL.NonConformita_Dettagli_W
        Dim res = elemW.CancellaTuttiIDettagliDiNC("", ID_NC, objParametri)

        Return res
    End Function

    Public Function CancellaDettaglio(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                                            ByVal ID_Dettaglio As Integer _
                                           ) As Boolean

        Dim elemW As New AgronicaCorePannelloDiControlloDAL.NonConformita_Dettagli_W
        Dim res = elemW.CancellaDettaglio("", ID_Dettaglio, objParametri)

        Return res
    End Function

End Class

Public Class PnlCtrl_NonConformita_Dettagli
    Public Property ID_Dettaglio As Integer
    Public Property ID_NC As Integer
    Public Property Utente As String
    Public Property Data As DateTime
    Public Property ID_Stato As Integer?
    Public Property ID_ListaAllegati As Integer?
    Public Property Descrizione As String
    Public Property Note As String

    Public Sub New(ID_Dettaglio As Integer, _
                    ID_NC As Integer, _
                    Utente As String, _
                    Data As DateTime, _
                    ID_Stato As Integer, _
                    ID_ListaAllegati As Integer, _
                    Descrizione As String, _
                    Note As String)

        _ID_Dettaglio = ID_Dettaglio
        _ID_NC = ID_NC
        _Utente = Utente
        _Data = Data
        _ID_Stato = ID_Stato
        _ID_ListaAllegati = ID_ListaAllegati
        _Descrizione = Descrizione
        _Note = Note
    End Sub

    Public Sub New()
        _ID_Dettaglio = -1
        _ID_NC = -1
        _Utente = ""
        _Data = Date.Now
        _ID_Stato = Nothing
        _ID_ListaAllegati = Nothing
        _Descrizione = Nothing
        _Note = Nothing
    End Sub

    Public Sub New(obj As JObject)
        'JObject jObject = JObject.Parse(json);
        'JToken jUser = jObject["user"];
        'name = (string) jUser["name"];
        'teamname = (string) jUser["teamname"];
        'email = (string) jUser["email"];
        'players = jUser["players"].ToArray();
        _ID_Dettaglio = CInt(obj("ID_Dettaglio"))
        _ID_NC = CInt(obj("ID_NC"))
        _Utente = obj("Utente")
        _Data = CDate(obj("Data"))
        _ID_Stato = CInt(obj("ID_Stato"))
        _ID_ListaAllegati = CInt(obj("ID_ListaAllegati"))
        _Descrizione = obj("Descrizione")
        _Note = obj("Note")
    End Sub

End Class
