Imports AgronicaCoreDataProvider
Imports AgronicaCoreLabControlloQualitaDAL

Public Class Documenti_R

    Public Function leggiDocumento( _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                                Optional ByVal Documento_Cod As Integer? = Nothing _
                              ) As List(Of LCQ_Documenti)

        Return leggiListaDocumenti(objParametri, Documento_Cod)

    End Function

    Public Function leggiListaDocumenti( _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                                Optional ByVal Documento_Cod As Integer? = Nothing, _
                                Optional ByVal Modello_Codice As String = Nothing, _
                                Optional ByVal Modello_Revisione As String = Nothing, _
                                Optional ByVal Articolo_Cod As String = Nothing, _
                                Optional ByVal DataOraCreazione As DateTime? = Nothing, _
                                Optional ByVal Stato As Integer? = Nothing, _
                                Optional ByVal Conforme_StdNome As String = Nothing, _
                                Optional ByVal Conforme_StdRev As String = Nothing _
                              ) As List(Of LCQ_Documenti)

        Dim d As New AgronicaCoreLabControlloQualitaDAL.Documenti_R
        Dim listaObjLCQ As New List(Of LCQ_Documenti)
        Dim dt As DataTable

        dt = d.Leggi_Documenti(Documento_Cod, Modello_Codice, Modello_Revisione, Articolo_Cod, _
                               DataOraCreazione, Stato, Conforme_StdNome, Conforme_StdRev, _
                               "", "DataOraCreazione", objParametri)

        For Each dRow As DataRow In dt.Rows
            Dim lcq As New LCQ_Documenti( _
                                          dRow("PivaSuperUser"), _
                                          dRow("Documento_Cod"), _
                                          dRow("Modello_Codice"), _
                                          dRow("Modello_Revisione"), _
                                          dRow("Articolo_Cod"), _
                                          dRow("DataOraCreazione"), _
                                          dRow("Stato"), _
                                          UtilityProvider.DBNullToNothing(dRow("Conforme_StdNome")), _
                                          UtilityProvider.DBNullToNothing(dRow("Conforme_StdRev")) _
                                    )
            listaObjLCQ.Add(lcq)
        Next

        Return listaObjLCQ

    End Function

    Public Function leggiListaDocumentiTraDate( _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                                Optional ByVal Documento_Cod As Integer? = Nothing, _
                                Optional ByVal Modello_Codice As String = Nothing, _
                                Optional ByVal Modello_Revisione As String = Nothing, _
                                Optional ByVal Articolo_Cod As String = Nothing, _
                                Optional ByVal DataOraCreazioneFrom As DateTime? = Nothing, _
                                Optional ByVal DataOraCreazioneTo As DateTime? = Nothing, _
                                Optional ByVal Stato As Integer? = Nothing, _
                                Optional ByVal Conforme_StdNome As String = Nothing, _
                                Optional ByVal Conforme_StdRev As String = Nothing _
                              ) As List(Of LCQ_Documenti)

        Dim d As New AgronicaCoreLabControlloQualitaDAL.Documenti_R
        Dim listaObjLCQ As New List(Of LCQ_Documenti)
        Dim dt As DataTable

        dt = d.Leggi_Documenti(Documento_Cod, Modello_Codice, Modello_Revisione, Articolo_Cod, _
                               DataOraCreazioneFrom, DataOraCreazioneTo, Stato, _
                               Conforme_StdNome, Conforme_StdRev, _
                               "", "DataOraCreazione", objParametri)

        For Each dRow As DataRow In dt.Rows
            Dim lcq As New LCQ_Documenti( _
                                          dRow("PivaSuperUser"), _
                                          dRow("Documento_Cod"), _
                                          dRow("Modello_Codice"), _
                                          dRow("Modello_Revisione"), _
                                          dRow("Articolo_Cod"), _
                                          dRow("DataOraCreazione"), _
                                          dRow("Stato"), _
                                          UtilityProvider.DBNullToNothing(dRow("Conforme_StdNome")), _
                                          UtilityProvider.DBNullToNothing(dRow("Conforme_StdRev")) _
                                    )
            listaObjLCQ.Add(lcq)
        Next

        Return listaObjLCQ

    End Function

    Public Function leggiListaDocumentiTraDate( _
                                ByRef objParametri As AgronicaCoreParametri, _
                                ByVal htParametri As Hashtable, _
                                Optional ByVal Documento_Cod As Integer? = Nothing, _
                                Optional ByVal Modello_Codice As String = Nothing, _
                                Optional ByVal Modello_Revisione As String = Nothing, _
                                Optional ByVal Articolo_Cod As String = Nothing, _
                                Optional ByVal DataOraCreazioneFrom As DateTime? = Nothing, _
                                Optional ByVal DataOraCreazioneTo As DateTime? = Nothing, _
                                Optional ByVal Stato As Integer? = Nothing, _
                                Optional ByVal Conforme_StdNome As String = Nothing, _
                                Optional ByVal Conforme_StdRev As String = Nothing _
                              ) As List(Of LCQ_Documenti)

        Dim d As New AgronicaCoreLabControlloQualitaDAL.Documenti_R
        Dim listaObjLCQ As New List(Of LCQ_Documenti)
        Dim dt As DataTable

        dt = d.Leggi_Documenti(htParametri, Documento_Cod, Modello_Codice, Modello_Revisione, _
                               Articolo_Cod, DataOraCreazioneFrom, DataOraCreazioneTo, Stato, _
                               Conforme_StdNome, Conforme_StdRev, _
                               "", "DataOraCreazione", objParametri)

        For Each dRow As DataRow In dt.Rows
            Dim lcq As New LCQ_Documenti( _
                                          dRow("PivaSuperUser"), _
                                          dRow("Documento_Cod"), _
                                          dRow("Modello_Codice"), _
                                          dRow("Modello_Revisione"), _
                                          dRow("Articolo_Cod"), _
                                          dRow("DataOraCreazione"), _
                                          dRow("Stato"), _
                                          UtilityProvider.DBNullToNothing(dRow("Conforme_StdNome")), _
                                          UtilityProvider.DBNullToNothing(dRow("Conforme_StdRev")) _
                                    )
            listaObjLCQ.Add(lcq)
        Next

        Return listaObjLCQ

    End Function

    Public Function leggiListaDocumentiConRilevazioniTraDate( _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                                Optional ByVal Documento_Cod As Integer? = Nothing, _
                                Optional ByVal Modello_Codice As String = Nothing, _
                                Optional ByVal Modello_Revisione As String = Nothing, _
                                Optional ByVal Articolo_Cod As String = Nothing, _
                                Optional ByVal DataOraRilevazioneFrom As DateTime? = Nothing, _
                                Optional ByVal DataOraRilevazioneTo As DateTime? = Nothing, _
                                Optional ByVal Stato As Integer? = Nothing, _
                                Optional ByVal Conforme_StdNome As String = Nothing, _
                                Optional ByVal Conforme_StdRev As String = Nothing _
                              ) As List(Of LCQ_Documenti)

        Dim d As New AgronicaCoreLabControlloQualitaDAL.Documenti_R
        Dim listaObjLCQ As New List(Of LCQ_Documenti)
        Dim dt As DataTable

        'sistemo la data di fine mettendo 23.59.59.999
        If Not IsNothing(DataOraRilevazioneTo) Then
            Dim dataora As New DateTime(DataOraRilevazioneTo.Value.Year, _
                                        DataOraRilevazioneTo.Value.Month, _
                                        DataOraRilevazioneTo.Value.Day, _
                                        23, 59, 59, 999)
            DataOraRilevazioneTo = dataora
        End If

        dt = d.Leggi_DocumentiConRilevazioniTraDate(Documento_Cod, Modello_Codice, Modello_Revisione, Articolo_Cod, _
                               DataOraRilevazioneFrom, DataOraRilevazioneTo, Stato, _
                               Conforme_StdNome, Conforme_StdRev, _
                               "", "DataOraCreazione", objParametri)

        For Each dRow As DataRow In dt.Rows
            Dim lcq As New LCQ_Documenti( _
                                          dRow("PivaSuperUser"), _
                                          dRow("Documento_Cod"), _
                                          dRow("Modello_Codice"), _
                                          dRow("Modello_Revisione"), _
                                          dRow("Articolo_Cod"), _
                                          dRow("DataOraCreazione"), _
                                          dRow("Stato"), _
                                          UtilityProvider.DBNullToNothing(dRow("Conforme_StdNome")), _
                                          UtilityProvider.DBNullToNothing(dRow("Conforme_StdRev")) _
                                    )
            listaObjLCQ.Add(lcq)
        Next

        Return listaObjLCQ

    End Function

    Public Function Leggi_DocumentiConDataProduzioneTraDate( _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                                ByVal DataOraInizio As DateTime?, _
                                ByVal DataOraFine As DateTime?, _
                                Optional ByVal Documento_Cod As Integer? = Nothing, _
                                Optional ByVal Modello_Codice As String = Nothing, _
                                Optional ByVal Modello_Revisione As String = Nothing, _
                                Optional ByVal Articolo_Cod As String = Nothing, _
                                Optional ByVal Stato As Integer? = Nothing, _
                                Optional ByVal Conforme_StdNome As String = Nothing, _
                                Optional ByVal Conforme_StdRev As String = Nothing _
                              ) As List(Of LCQ_Documenti)

        Return Leggi_DocumentiConParametroTestataTraDate(objParametri, _
                                        "Data Produzione", DataOraInizio, DataOraFine, _
                                        Documento_Cod, Modello_Codice, Modello_Revisione, Articolo_Cod, _
                                        Stato, Conforme_StdNome, Conforme_StdRev)

    End Function

    Public Function Leggi_DocumentiConDataConfezionamentoTraDate( _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                                ByVal DataOraInizio As DateTime?, _
                                ByVal DataOraFine As DateTime?, _
                                Optional ByVal Documento_Cod As Integer? = Nothing, _
                                Optional ByVal Modello_Codice As String = Nothing, _
                                Optional ByVal Modello_Revisione As String = Nothing, _
                                Optional ByVal Articolo_Cod As String = Nothing, _
                                Optional ByVal Stato As Integer? = Nothing, _
                                Optional ByVal Conforme_StdNome As String = Nothing, _
                                Optional ByVal Conforme_StdRev As String = Nothing _
                              ) As List(Of LCQ_Documenti)

        Return Leggi_DocumentiConParametroTestataTraDate(objParametri, _
                                        "Data Confezionamento", DataOraInizio, DataOraFine, _
                                        Documento_Cod, Modello_Codice, Modello_Revisione, Articolo_Cod, _
                                        Stato, Conforme_StdNome, Conforme_StdRev)

    End Function

    Public Function Leggi_DocumentiConDataFornituraTraDate( _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                                ByVal DataOraInizio As DateTime?, _
                                ByVal DataOraFine As DateTime?, _
                                Optional ByVal Documento_Cod As Integer? = Nothing, _
                                Optional ByVal Modello_Codice As String = Nothing, _
                                Optional ByVal Modello_Revisione As String = Nothing, _
                                Optional ByVal Articolo_Cod As String = Nothing, _
                                Optional ByVal Stato As Integer? = Nothing, _
                                Optional ByVal Conforme_StdNome As String = Nothing, _
                                Optional ByVal Conforme_StdRev As String = Nothing _
                              ) As List(Of LCQ_Documenti)

        Return Leggi_DocumentiConParametroTestataTraDate(objParametri, _
                                        "Data Fornitura", DataOraInizio, DataOraFine, _
                                        Documento_Cod, Modello_Codice, Modello_Revisione, Articolo_Cod, _
                                        Stato, Conforme_StdNome, Conforme_StdRev)

    End Function

    Private Function Leggi_DocumentiConParametroTestataTraDate( _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                                ByVal NomeParametro As String, _
                                ByVal DataOraInizio As DateTime?, _
                                ByVal DataOraFine As DateTime?, _
                                Optional ByVal Documento_Cod As Integer? = Nothing, _
                                Optional ByVal Modello_Codice As String = Nothing, _
                                Optional ByVal Modello_Revisione As String = Nothing, _
                                Optional ByVal Articolo_Cod As String = Nothing, _
                                Optional ByVal Stato As Integer? = Nothing, _
                                Optional ByVal Conforme_StdNome As String = Nothing, _
                                Optional ByVal Conforme_StdRev As String = Nothing _
                              ) As List(Of LCQ_Documenti)

        Dim d As New AgronicaCoreLabControlloQualitaDAL.Documenti_R
        Dim listaObjLCQ As New List(Of LCQ_Documenti)
        Dim dt As DataTable

        dt = d.Leggi_DocumentiConParametroTestataTraDate(Documento_Cod, Modello_Codice, Modello_Revisione, Articolo_Cod, _
                               Stato, Conforme_StdNome, Conforme_StdRev, _
                               NomeParametro, DataOraInizio, DataOraFine, _
                               "", "", objParametri)

        For Each dRow As DataRow In dt.Rows
            Dim lcq As New LCQ_Documenti( _
                                          dRow("PivaSuperUser"), _
                                          dRow("Documento_Cod"), _
                                          dRow("Modello_Codice"), _
                                          dRow("Modello_Revisione"), _
                                          dRow("Articolo_Cod"), _
                                          dRow("DataOraCreazione"), _
                                          dRow("Stato"), _
                                          UtilityProvider.DBNullToNothing(dRow("Conforme_StdNome")), _
                                          UtilityProvider.DBNullToNothing(dRow("Conforme_StdRev")) _
                                    )
            listaObjLCQ.Add(lcq)
        Next

        Return listaObjLCQ

    End Function

    Public Function Leggi_DocumentiSenzaDataDInteresseConCreazioneTraDate( _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                                ByVal DataOraCreazioneDa As DateTime?, _
                                ByVal DataOraCreazioneA As DateTime?, _
                                Optional ByVal Documento_Cod As Integer? = Nothing, _
                                Optional ByVal Modello_Codice As String = Nothing, _
                                Optional ByVal Modello_Revisione As String = Nothing, _
                                Optional ByVal Articolo_Cod As String = Nothing, _
                                Optional ByVal Stato As Integer? = Nothing, _
                                Optional ByVal Conforme_StdNome As String = Nothing, _
                                Optional ByVal Conforme_StdRev As String = Nothing _
                              ) As List(Of LCQ_Documenti)

        Dim d As New AgronicaCoreLabControlloQualitaDAL.Documenti_R
        Dim listaObjLCQ As New List(Of LCQ_Documenti)
        Dim dt As DataTable

        dt = d.Leggi_DocumentiSenzaDataDInteresseConCreazioneTraDate(Documento_Cod, _
                               Modello_Codice, Modello_Revisione, Articolo_Cod, _
                               Stato, Conforme_StdNome, Conforme_StdRev, _
                               DataOraCreazioneDa, DataOraCreazioneA, _
                               "", "", objParametri)

        For Each dRow As DataRow In dt.Rows
            Dim lcq As New LCQ_Documenti( _
                                          dRow("PivaSuperUser"), _
                                          dRow("Documento_Cod"), _
                                          dRow("Modello_Codice"), _
                                          dRow("Modello_Revisione"), _
                                          dRow("Articolo_Cod"), _
                                          dRow("DataOraCreazione"), _
                                          dRow("Stato"), _
                                          UtilityProvider.DBNullToNothing(dRow("Conforme_StdNome")), _
                                          UtilityProvider.DBNullToNothing(dRow("Conforme_StdRev")) _
                                    )
            listaObjLCQ.Add(lcq)
        Next

        Return listaObjLCQ

    End Function

    Public Function Leggi_DocumentiConDataDInteresseTraDate( _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                                ByVal DataOraInizio As DateTime?, _
                                ByVal DataOraFine As DateTime?, _
                                Optional ByVal Documento_Cod As Integer? = Nothing, _
                                Optional ByVal Modello_Codice As String = Nothing, _
                                Optional ByVal Modello_Revisione As String = Nothing, _
                                Optional ByVal Articolo_Cod As String = Nothing, _
                                Optional ByVal Stato As Integer? = Nothing, _
                                Optional ByVal Conforme_StdNome As String = Nothing, _
                                Optional ByVal Conforme_StdRev As String = Nothing _
                              ) As List(Of LCQ_Documenti)

        Dim tabDocDataProd As List(Of LCQ_Documenti) = Me.Leggi_DocumentiConDataProduzioneTraDate(objParametri, DataOraInizio, DataOraFine, Documento_Cod, Modello_Codice, Modello_Revisione, Articolo_Cod, Stato, Conforme_StdNome, Conforme_StdRev)
        Dim tabDocDataConf As List(Of LCQ_Documenti) = Me.Leggi_DocumentiConDataConfezionamentoTraDate(objParametri, DataOraInizio, DataOraFine, Documento_Cod, Modello_Codice, Modello_Revisione, Articolo_Cod, Stato, Conforme_StdNome, Conforme_StdRev)
        Dim tabDocDataForn As List(Of LCQ_Documenti) = Me.Leggi_DocumentiConDataFornituraTraDate(objParametri, DataOraInizio, DataOraFine, Documento_Cod, Modello_Codice, Modello_Revisione, Articolo_Cod, Stato, Conforme_StdNome, Conforme_StdRev)
        Dim tabDocSenzaDataDInteresse As List(Of LCQ_Documenti) = Me.Leggi_DocumentiSenzaDataDInteresseConCreazioneTraDate(objParametri, DataOraInizio, DataOraFine, Documento_Cod, Modello_Codice, Modello_Revisione, Articolo_Cod, Stato, Conforme_StdNome, Conforme_StdRev)

        'faccio un unica lista di doc
        Dim tabDocumenti As New List(Of LCQ_Documenti)
        tabDocumenti.AddRange(tabDocDataProd)
        tabDocumenti.AddRange(tabDocDataConf)
        tabDocumenti.AddRange(tabDocDataForn)
        tabDocumenti.AddRange(tabDocSenzaDataDInteresse)

        'ordino in base alla data
        tabDocumenti = tabDocumenti.OrderBy(Function(x) x.DataOraCreazione).ThenBy(Function(x) x.Documento_Cod).ToList()

        Return tabDocumenti

    End Function

End Class

Public Class Documenti_W

    Public Function aggiungi(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                                ByVal Modello_Codice As String, _
                                ByVal Modello_Revisione As String, _
                                ByVal Articolo_Cod As String, _
                                ByVal DataOraCreazione As DateTime, _
                                ByVal Stato As Integer, _
                                ByVal Conforme_StdNome As String, _
                                ByVal Conforme_StdRev As String _
                                 ) As Integer?

        Dim d As New AgronicaCoreLabControlloQualitaDAL.Documenti_W
        Dim res As Boolean = False

        '  Prendo l'indice dalle agrosequenze
        Dim seq As New Agro_Sequenze()
        Dim Documento_Cod As Integer = seq.NuovoId_Tabella("LCQ_Documenti", 100, 2000000000, objParametri)

        res = d.Scrivi(objParametri, _
                       Documento_Cod, _
                       Modello_Codice, Modello_Revisione, _
                       Articolo_Cod, _
                       DataOraCreazione, Stato, _
                       Conforme_StdNome, Conforme_StdRev)

        If res Then ' se è andato tutto bene...
            Return Documento_Cod '... ritorno il nuovo indice
        Else
            Return Nothing
        End If

    End Function

    Public Function modifica(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                                ByVal Old_Documento_Cod As String, _
                                ByVal New_Stato As Integer, _
                                ByVal New_Conforme_StdNome As String, _
                                ByVal New_Conforme_StdRev As String _
                              ) As Boolean

        Dim d As New AgronicaCoreLabControlloQualitaDAL.Documenti_W
        Dim res As Boolean = False

        res = d.Modifica(objParametri, Old_Documento_Cod, _
                         New_Stato, New_Conforme_StdNome, New_Conforme_StdRev)

        Return res

    End Function

    Public Function modificaStato(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                                ByVal Old_Documento_Cod As String, _
                                ByVal New_Stato As Integer _
                              ) As Boolean

        Dim d As New AgronicaCoreLabControlloQualitaDAL.Documenti_W
        Dim res As Boolean = False

        res = d.ModificaStato(objParametri, Old_Documento_Cod, New_Stato)

        Return res

    End Function

    Public Function modificaConformita(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                                ByVal Old_Documento_Cod As String, _
                                ByVal New_Conforme_StdNome As String, _
                                ByVal New_Conforme_StdRev As String _
                              ) As Boolean

        Dim d As New AgronicaCoreLabControlloQualitaDAL.Documenti_W
        Dim res As Boolean = False

        res = d.ModificaConformita(objParametri, Old_Documento_Cod, _
                         New_Conforme_StdNome, New_Conforme_StdRev)

        Return res

    End Function

    Public Function cancella(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                                               ByVal Documento_Cod As Integer _
                                           ) As Boolean

        Dim d As New AgronicaCoreLabControlloQualitaDAL.Documenti_W
        Dim res As Boolean = False

        'salvo il precendente stato
        Dim canc As AgronicaCoreParametri.enumCancellazioneLogica = objParametri.FlagCancellazioneLogica
        'assegno la cancellazione fisica
        objParametri.FlagCancellazioneLogica = AgronicaCoreParametri.enumCancellazioneLogica.CancellazioneFisica

        res = d.Cancella(objParametri, Documento_Cod, "")

        'ripristino il precedente stato
        objParametri.FlagCancellazioneLogica = canc

        Return res
    End Function

End Class

Public Enum StatoDoc
    aperto = 0
    chiuso = 1
End Enum

Public Class LCQ_Documenti

    Private _PivaSuperUser As String
    Private _Documento_Cod As Integer
    Private _Modello_Codice As String
    Private _Modello_Revisione As String
    Private _Articolo_Cod As String
    Private _DataOraCreazione As DateTime?
    Private _Stato As Integer?
    Private _Conforme_StdNome As String
    Private _Conforme_StdRev As String

    Public Sub New()
        _PivaSuperUser = Nothing
        _Documento_Cod = Nothing
        _Modello_Codice = Nothing
        _Modello_Revisione = Nothing
        _Articolo_Cod = Nothing
        _DataOraCreazione = Nothing
        _Stato = Nothing
        _Conforme_StdNome = Nothing
        _Conforme_StdRev = Nothing
    End Sub

    Public Sub New(PivaSuperUser As String, _
                     Documento_Cod As Integer, _
                     Modello_Codice As String, _
                     Modello_Revisione As String, _
                     Articolo_Cod As String, _
                     DataOraCreazione As DateTime?, _
                     Stato As StatoDoc, _
                     Conforme_StdNome As String, _
                     Conforme_StdRev As String)

        _PivaSuperUser = PivaSuperUser
        _Documento_Cod = Documento_Cod
        _Modello_Codice = Modello_Codice
        _Modello_Revisione = Modello_Revisione
        _Articolo_Cod = Articolo_Cod
        _DataOraCreazione = DataOraCreazione
        _Stato = Stato
        _Conforme_StdNome = Conforme_StdNome
        _Conforme_StdRev = Conforme_StdRev
    End Sub

    Public Property PivaSuperUser() As String
        Get
            Return _PivaSuperUser
        End Get
        Set(ByVal value As String)
            _PivaSuperUser = value
        End Set
    End Property

    Public Property Documento_Cod() As Integer
        Get
            Return _Documento_Cod
        End Get
        Set(ByVal value As Integer)
            _Documento_Cod = value
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

    Public Property Modello_Revisione() As String
        Get
            Return _Modello_Revisione
        End Get
        Set(ByVal value As String)
            _Modello_Revisione = value
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

    Public Property DataOraCreazione() As DateTime?
        Get
            Return _DataOraCreazione
        End Get
        Set(ByVal value As DateTime?)
            _DataOraCreazione = value
        End Set
    End Property

    Public Property Stato() As StatoDoc
        Get
            Return _Stato
        End Get
        Set(ByVal value As StatoDoc)
            _Stato = value
        End Set
    End Property

    Public Property Conforme_StdNome() As String
        Get
            Return _Conforme_StdNome
        End Get
        Set(ByVal value As String)
            _Conforme_StdNome = value
        End Set
    End Property

    Public Property Conforme_StdRev() As String
        Get
            Return _Conforme_StdRev
        End Get
        Set(ByVal value As String)
            _Conforme_StdRev = value
        End Set
    End Property

End Class