Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCorePannelloDiControlloDAL
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

Public Class NC_Dettagli_R

    Public Function leggi_DettaglioNC(ByRef objParametri As AgronicaCoreParametri, _
                                        ByVal ID_Dettaglio As Integer _
                                        ) As List(Of NC_Dettagli)

        Return leggi_NonConformita_Dettagli(objParametri, ID_Dettaglio, Nothing, _
                                            Nothing, Nothing, Nothing, Nothing)
    End Function

    Public Function leggi_NonConformita_Dettagli(ByRef objParametri As AgronicaCoreParametri, _
                                                    ByVal ID_NC As Integer _
                                                    ) As List(Of NC_Dettagli)

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
                                                  ) As List(Of NC_Dettagli)

        Dim r As New AgronicaCorePannelloDiControlloDAL.NC_Dettagli_R
        Dim listaObjNC As New List(Of NC_Dettagli)

        Dim dt As DataTable = r.Leggi(ID_Dettaglio, ID_NC, Utente, Data, _
                                                ID_Stato, ID_ListaAllegati, "", "", objParametri)

        For Each dRow As DataRow In dt.Rows
            Dim det As New NC_Dettagli(
                                          UtilityProvider.DBNullToNothing(dRow("ID_Dettaglio")),
                                          UtilityProvider.DBNullToNothing(dRow("ID_NC")),
                                          UtilityProvider.DBNullToNothing(dRow("Utente")),
                                          UtilityProvider.DBNullToNothing(dRow("Data")),
                                          UtilityProvider.DBNullToNothing(dRow("ID_Stato")),
                                          UtilityProvider.DBNullToNothing(dRow("ID_ListaAllegati")),
                                          UtilityProvider.DBNullToNothing(dRow("Descrizione")),
                                          UtilityProvider.DBNullToNothing(dRow("Note")),
                                          UtilityProvider.DBNullToNothing(dRow("DataChiusuraPrevista")),
                                          UtilityProvider.DBNullToNothing(dRow("DataChiusuraEffettiva")),
                                          UtilityProvider.DBNullToNothing(dRow("Responsabile")),
                                          UtilityProvider.DBNullToNothing(dRow("PersoneCoinvolte")),
                                          UtilityProvider.DBNullToNothing(dRow("CodiceDettaglio"))
                                        )
            listaObjNC.Add(det)
        Next

        Return listaObjNC

    End Function

End Class

Public Class NC_Dettagli_W

    Public Function aggiungi(ByRef objParametri As AgronicaCoreParametri,
                                ByRef ListaNC_Det As List(Of NC_Dettagli)
                                ) As Boolean

        Dim res As Boolean = True

        For Each NC_det As NC_Dettagli In ListaNC_Det
            'Aggiungo il dettaglio
            res = aggiungi(objParametri, NC_det)
        Next

        Return res

    End Function

    Public Function aggiungi(ByRef objParametri As AgronicaCoreParametri,
                                ByRef det As NC_Dettagli
                                ) As Boolean

        'Creo il nuovo ID_Dettaglio
        Dim seq As New Agro_Sequenze()
        Dim id_Det As Integer = seq.NuovoId_Tabella("NC_ID_Dettaglio", 0, 2000000000, objParametri)

        'Aggiungo il dettaglio
        Dim res As Boolean = aggiungi(objParametri,
                        id_Det, det.ID_NC, det.Utente, det.Data,
                        det.ID_Stato, det.ID_ListaAllegati, det.Descrizione, det.Note,
                        det.DataChiusuraPrevista, det.DataChiusuraEffettiva, det.Responsabile, det.PersoneCoinvolte, det.CodiceDettaglio)

        det.ID_Dettaglio = id_Det

        Return res

    End Function

    Public Function aggiungi(ByRef objParametri As AgronicaCoreParametri,
                                ByVal ID_Dettaglio As Integer,
                                ByVal ID_NC As Integer,
                                ByVal Utente As String,
                                ByVal Data As DateTime?,
                                ByVal ID_Stato As Integer?,
                                ByVal ID_ListaAllegati As Integer?,
                                ByVal Descrizione As String,
                                ByVal Note As String,
                                ByVal DataChiusuraPrevista As DateTime?,
                                ByVal DataChiusuraEffettiva As DateTime?,
                                ByVal Responsabile As String,
                                ByVal PersoneCoinvolte As String,
                                ByVal CodiceDettaglio As Integer?
                              ) As Boolean

        Dim w As New AgronicaCorePannelloDiControlloDAL.NC_Dettagli_W
        Dim res As Boolean = w.Scrivi(objParametri, ID_Dettaglio, ID_NC, Utente, Data,
                                  ID_Stato, ID_ListaAllegati, Descrizione, Note,
                                  DataChiusuraPrevista, DataChiusuraEffettiva, Responsabile, PersoneCoinvolte, CodiceDettaglio)

        Return res

    End Function

    Public Function modifica(ByRef objParametri As AgronicaCoreParametri,
                                ByVal Old_ID_Dettaglio As Integer,
                                ByVal New_Utente As String,
                                ByVal New_Data As DateTime?,
                                ByVal New_ID_Stato As Integer?,
                                ByVal New_ID_ListaAllegati As Integer?,
                                ByVal New_Descrizione As String,
                                ByVal New_Note As String,
                                ByVal New_DataChiusuraPrevista As DateTime?,
                                ByVal New_DataChiusuraEffettiva As DateTime?,
                                ByVal New_Responsabile As String,
                                ByVal New_PersoneCoinvolte As String,
                                ByVal New_CodiceDettaglio As Integer?
                              ) As Boolean

        Dim w As New AgronicaCorePannelloDiControlloDAL.NC_Dettagli_W
        Dim res As Boolean = w.Modifica(objParametri, Old_ID_Dettaglio, New_Utente, New_Data,
                          New_ID_Stato, New_ID_ListaAllegati, New_Descrizione, New_Note,
                          New_DataChiusuraPrevista, New_DataChiusuraEffettiva, New_Responsabile, New_PersoneCoinvolte, New_CodiceDettaglio)

        Return res

    End Function

    Public Function modifica(ByRef objParametri As AgronicaCoreParametri,
                                ByVal NC_Det As NC_Dettagli
                              ) As Boolean

        Dim res As Boolean = modifica(objParametri, NC_Det.ID_Dettaglio, NC_Det.Utente, NC_Det.Data,
                     NC_Det.ID_Stato, NC_Det.ID_ListaAllegati, NC_Det.Descrizione, NC_Det.Note,
                     NC_Det.DataChiusuraPrevista, NC_Det.DataChiusuraEffettiva, NC_Det.Responsabile, NC_Det.PersoneCoinvolte, NC_Det.CodiceDettaglio)

        Return res

    End Function

    Public Function modifica(ByRef objParametri As AgronicaCoreParametri,
                                ByRef ListaNC_Det As List(Of NC_Dettagli)
                              ) As Boolean

        Dim res As Boolean = True

        For Each NC_det As NC_Dettagli In ListaNC_Det

            If NC_det.ID_Dettaglio > 0 Then 'Se esiste già, allora la modifico
                res = modifica(objParametri, NC_det)
            Else 'Se non esiste, lo aggiungo
                res = aggiungi(objParametri, NC_det)
            End If

        Next

        Return res

    End Function

    Public Function CancellaTuttiIDettagliDiNC(ByRef objParametri As AgronicaCoreParametri,
                                            ByVal ID_NC As Integer
                                           ) As Boolean

        Dim elemW As New AgronicaCorePannelloDiControlloDAL.NC_Dettagli_W
        Dim res = elemW.CancellaTuttiIDettagliDiNC("", ID_NC, objParametri)

        Return res
    End Function

    Public Function CancellaDettaglio(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                            ByVal ID_Dettaglio As Integer
                                           ) As Boolean

        Dim elemW As New AgronicaCorePannelloDiControlloDAL.NC_Dettagli_W
        Dim res = elemW.CancellaDettaglio("", ID_Dettaglio, objParametri)

        Return res
    End Function

End Class

<Serializable()>
Public Class NC_Dettagli
    Public Property ID_Dettaglio As Integer?
    Public Property ID_NC As Integer?
    Public Property Utente As String
    Public Property Data As DateTime?
    Public Property ID_Stato As Integer?
    Public Property ID_ListaAllegati As Integer?
    Public Property Descrizione As String
    Public Property Note As String
    Public Property DataChiusuraPrevista As DateTime?
    Public Property DataChiusuraEffettiva As DateTime?
    Public Property Responsabile As String
    Public Property PersoneCoinvolte As String
    Public Property CodiceDettaglio As Integer?

    Public Sub New(ID_Dettaglio As Integer?,
                    ID_NC As Integer?,
                    Utente As String,
                    Data As DateTime?,
                    ID_Stato As Integer?,
                    ID_ListaAllegati As Integer?,
                    Descrizione As String,
                    Note As String,
                    DataChiusuraPrevista As DateTime?,
                    DataChiusuraEffettiva As DateTime?,
                    Responsabile As String,
                    PersoneCoinvolte As String,
                    CodiceDettaglio As Integer?)

        _ID_Dettaglio = ID_Dettaglio
        _ID_NC = ID_NC
        _Utente = Utente
        _Data = Data
        _ID_Stato = ID_Stato
        _ID_ListaAllegati = ID_ListaAllegati
        _Descrizione = Descrizione
        _Note = Note
        _DataChiusuraPrevista = DataChiusuraPrevista
        _DataChiusuraEffettiva = DataChiusuraEffettiva
        _Responsabile = Responsabile
        _PersoneCoinvolte = PersoneCoinvolte
        _CodiceDettaglio = CodiceDettaglio

    End Sub

    Public Sub New()
        _ID_Dettaglio = Nothing
        _ID_NC = Nothing
        _Utente = ""
        _Data = Nothing
        _ID_Stato = Nothing
        _ID_ListaAllegati = Nothing
        _Descrizione = ""
        _Note = ""
        _DataChiusuraPrevista = Nothing
        _DataChiusuraEffettiva = Nothing
        _Responsabile = ""
        _PersoneCoinvolte = ""
        _CodiceDettaglio = Nothing

    End Sub

    Public Sub New(obj As JObject)
        'JObject jObject = JObject.Parse(json);
        'JToken jUser = jObject["user"];
        'name = (string) jUser["name"];
        'teamname = (string) jUser["teamname"];
        'email = (string) jUser["email"];
        'players = jUser["players"].ToArray();

        _ID_Dettaglio = assegnaValoreNullableDaJSON_Integer(obj("ID_Dettaglio"), _ID_Dettaglio)
        _ID_NC = assegnaValoreNullableDaJSON_Integer(obj("ID_NC"), _ID_NC)
        _Utente = assegnaValoreNullableDaJSON_String(obj("Utente"), _Utente)
        _Data = assegnaValoreNullableDaJSON_Date(obj("Data"), _Data)
        _ID_Stato = assegnaValoreNullableDaJSON_Integer(obj("ID_Stato"), _ID_Stato)
        _ID_ListaAllegati = assegnaValoreNullableDaJSON_Integer(obj("ID_ListaAllegati"), _ID_ListaAllegati)
        _Descrizione = assegnaValoreNullableDaJSON_String(obj("Descrizione"), _Descrizione)
        _Note = assegnaValoreNullableDaJSON_String(obj("Note"), _Note)
        _DataChiusuraPrevista = assegnaValoreNullableDaJSON_Date(obj("DataChiusuraPrevista"), _DataChiusuraPrevista)
        _DataChiusuraEffettiva = assegnaValoreNullableDaJSON_Date(obj("DataChiusuraEffettiva"), _DataChiusuraEffettiva)
        _Responsabile = assegnaValoreNullableDaJSON_String(obj("Responsabile"), _Responsabile)
        _PersoneCoinvolte = assegnaValoreNullableDaJSON_String(obj("PersoneCoinvolte"), _PersoneCoinvolte)
        _CodiceDettaglio = assegnaValoreNullableDaJSON_Integer(obj("CodiceDettaglio"), _CodiceDettaglio)

    End Sub

    Private Function assegnaValoreNullableDaJSON_String(objJSON As JValue, objVB As String) As String
        Return If(Not IsNothing(objJSON) AndAlso objJSON.Type <> JTokenType.Null, CStr(objJSON), objVB)
    End Function

    Private Function assegnaValoreNullableDaJSON_Integer(objJSON As JValue, objVB As Integer?) As Integer?
        Return If(Not IsNothing(objJSON) AndAlso objJSON.Type <> JTokenType.Null, CInt(objJSON), objVB)
    End Function

    Private Function assegnaValoreNullableDaJSON_Date(objJSON As JValue, objVB As DateTime?) As DateTime?
        Return If(Not IsNothing(objJSON) AndAlso objJSON.Type <> JTokenType.Null, CDate(objJSON), objVB)
    End Function

    Private Function assegnaValoreNullableDaJSON_Decimal(objJSON As JValue, objVB As Decimal?) As Decimal?
        Return If(Not IsNothing(objJSON) AndAlso objJSON.Type <> JTokenType.Null, CDec(objJSON), objVB)
    End Function

End Class
