Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreLabControlloQualitaDAL

Public Class StandardRev_R
    Public Function leggiListaStandardRev( _
                              ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                              ByVal TipoStd As enum_LCQ_TipoModelloStandard, _
                              ByVal DataDaVerificare As DateTime, _
                              Optional ByVal Modello_Codice As String = Nothing, _
                              Optional ByVal Modello_Revisione As String = Nothing _
                              ) As List(Of String)

        Dim s As New AgronicaCoreLabControlloQualitaDAL.StandardRev_R
        Dim listaStandard As New List(Of String)
        Dim dt As DataTable

        dt = s.Leggi_StandardRev(TipoStd, DataDaVerificare, Nothing, Nothing, Modello_Codice, Modello_Revisione, "", "", objParametri)

        For Each row As DataRow In dt.Rows
            listaStandard.Add(row("Nome") & " " & row("Revisione"))
        Next

        Return listaStandard

    End Function

    Public Function leggi_LCQ_StandardRev( _
                              ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                              ByVal TipoStd As enum_LCQ_TipoModelloStandard, _
                              ByVal DataDaVerificare As DateTime, _
                              Optional ByVal Standard_Nome As String = Nothing, _
                              Optional ByVal Standard_Revisione As String = Nothing, _
                              Optional ByVal Modello_Codice As String = Nothing, _
                              Optional ByVal Modello_Revisione As String = Nothing _
                              ) As List(Of LCQ_StandardRev)

        Dim s As New AgronicaCoreLabControlloQualitaDAL.StandardRev_R
        Dim listaObjLCQ As New List(Of LCQ_StandardRev)

        Dim dt As DataTable = s.Leggi_StandardRev(TipoStd, DataDaVerificare, Standard_Nome, Standard_Revisione, Modello_Codice, Modello_Revisione, "", "", objParametri)

        For Each dRow As DataRow In dt.Rows
            Dim lcq As New LCQ_StandardRev( _
                                          dRow("PivaSuperUser"), _
                                          dRow("Nome"), _
                                          dRow("Revisione"), _
                                          dRow("Modello_Codice"), _
                                          dRow("Modello_Revisione"), _
                                          UtilityProvider.DBNullToNothing(dRow("DataValiditaInizio")), _
                                          UtilityProvider.DBNullToNothing(dRow("DataValiditaFine")) _
                                    )
            listaObjLCQ.Add(lcq)
        Next

        Return listaObjLCQ

    End Function

    Public Function leggi_LCQ_StandardRevPerDocumento( _
                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                             ByVal doc As LCQ_Documenti
                             ) As List(Of LCQ_StandardRev)

        Dim listaObjLCQ As New List(Of LCQ_StandardRev)

        ' leggo gli standard che hanno come articolo collegato, quello inserito dall'utente
        Dim std_R As New AgronicaCoreLabControlloQualitaBIZ.Standard_R
        Dim tabStd As List(Of LCQ_Standard) = std_R.leggi_LCQ_Standard(objParametri, _
                                                Nothing, doc.Modello_Codice, doc.Articolo_Cod)

        'se ho trovato qualcosa, prendo tutte le rev attive degli std trovati
        If tabStd.Count > 0 Then

            For Each std As LCQ_Standard In tabStd
                listaObjLCQ.AddRange(leggi_LCQ_StandardRev(objParametri, _
                                enum_LCQ_TipoModelloStandard.Attivo, doc.DataOraCreazione, _
                                std.Nome, Nothing, doc.Modello_Codice, doc.Modello_Revisione))
            Next
        Else 'se non esiste uno standard per l'articolo in questione, mostro tutti gli standard attivi

            'leggo tutti gli standard
            tabStd = std_R.leggi_LCQ_Standard(objParametri, _
                                                Nothing, doc.Modello_Codice, Nothing)

            'elimino gli standard che hanno un articolo collegato
            tabStd = tabStd.Where(Function(x) IsNothing(x.Articolo_Cod)).ToList()

            'prendo tutte le rev attive degli std senza articolo
            For Each std As LCQ_Standard In tabStd
                listaObjLCQ.AddRange(leggi_LCQ_StandardRev(objParametri, _
                                enum_LCQ_TipoModelloStandard.Attivo, doc.DataOraCreazione, _
                                std.Nome, Nothing, doc.Modello_Codice, doc.Modello_Revisione))
            Next
        End If

        Return listaObjLCQ

    End Function

    Public Function leggi_LCQ_StandardRevTutti(
                              ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                              Optional ByVal Standard_Nome As String = Nothing,
                              Optional ByVal Standard_Revisione As String = Nothing,
                              Optional ByVal codice As String = Nothing,
                              Optional ByVal revisione As String = Nothing
                              ) As List(Of LCQ_StandardRev)

        Dim listaObjLCQ As List(Of LCQ_StandardRev) = leggi_LCQ_StandardRev(objParametri, enum_LCQ_TipoModelloStandard.Tutti, DateTime.Now, Standard_Nome, Standard_Revisione, codice, revisione)

        Return listaObjLCQ

    End Function

    Function test_StdRevEsiste( _
                              ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                              ByVal Standard_Nome As String, _
                              ByVal Standard_Revisione As String, _
                              ByVal Modello_Codice As String, _
                              ByVal Modello_Revisione As String _
                              ) As Boolean

        Dim s As New AgronicaCoreLabControlloQualitaDAL.StandardRev_R
        Dim res As Boolean = True

        res = s.Verifica_StdEsiste(Standard_Nome, Standard_Revisione, Modello_Codice, Modello_Revisione, objParametri)

        Return res
    End Function

    Function test_StdRevAttivoDaAttivareInData( _
                              ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                              ByVal Standard_Nome As String, _
                              ByVal Standard_Revisione As String, _
                              ByVal Modello_Codice As String, _
                              ByVal Modello_Revisione As String, _
                              ByVal DataDaVerificare As DateTime _
                              ) As Boolean

        Dim s As New AgronicaCoreLabControlloQualitaDAL.StandardRev_R
        Dim res As Boolean = True

        res = s.Verifica_StdRevAttivoDaAttivareInData(Standard_Nome, Standard_Revisione, Modello_Codice, Modello_Revisione, DataDaVerificare, objParametri)

        Return res
    End Function

    Function test_StdRevInUso(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                               ByVal Modello_Codice As String, _
                               ByVal Modello_Revisione As String, _
                               ByVal Standard_Nome As String, _
                               ByVal Standard_Revisione As String _
                               ) As Boolean

        Dim s As New AgronicaCoreLabControlloQualitaDAL.StandardRev_R
        Dim inUso As Boolean = True

        inUso = s.Verifica_StdRevInUso(Modello_Codice, Modello_Revisione, Standard_Nome, Standard_Revisione, objParametri)

        Return inUso
    End Function

End Class

Public Class StandardRev_W

    Public Function aggiungi(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                           ByVal standard As LCQ_StandardRev
                              ) As Boolean

        Dim s As New AgronicaCoreLabControlloQualitaDAL.StandardRev_W
        Dim res As Boolean = False

        res = s.Scrivi(objParametri, _
                       standard.Nome, standard.Revisione, _
                       standard.Modello_Codice, standard.Modello_Revisione, _
                       standard.DataValiditaInizio, standard.DataValiditaFine)

        Return res

    End Function

    Public Function aggiungi(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                            ByVal Nome As String, _
                            ByVal Revisione As String, _
                            ByVal Modello_Codice As String, _
                            ByVal Modello_Revisione As String, _
                            ByVal New_DataValiditaInizio As DateTime?, _
                            ByVal New_DataValiditaFine As DateTime? _
                              ) As Boolean

        Dim s As New AgronicaCoreLabControlloQualitaDAL.StandardRev_W
        Dim res As Boolean = False

        res = s.Scrivi(objParametri, _
                       Nome, Revisione, _
                       Modello_Codice, Modello_Revisione, _
                       New_DataValiditaInizio, New_DataValiditaFine)

        Return res

    End Function

    Public Function modifica(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                                ByVal Old_Nome As String, _
                                ByVal Old_Revisione As String, _
                                ByVal Old_Modello_Codice As String, _
                                ByVal Old_Modello_Revisione As String, _
                                ByVal New_Nome As String, _
                                ByVal New_Revisione As String, _
                                ByVal New_DataValiditaInizio As DateTime?, _
                                ByVal New_DataValiditaFine As DateTime? _
                              ) As Boolean

        Dim s As New AgronicaCoreLabControlloQualitaDAL.StandardRev_W
        Dim res As Boolean = False

        res = s.Modifica(objParametri, Old_Nome, Old_Revisione, _
                            Old_Modello_Codice, Old_Modello_Revisione, _
                            New_Nome, New_Revisione, _
                            New_DataValiditaInizio, New_DataValiditaFine)

        Return res

    End Function

End Class

Public Class LCQ_StandardRev

    Private _PivaSuperUser As String
    Private _Nome As String
    Private _Revisione As String
    Private _Modello_Codice As String
    Private _Modello_Revisione As String
    Private _DataValiditaInizio As DateTime?
    Private _DataValiditaFine As DateTime?

    Public Sub New()
        _PivaSuperUser = Nothing
        _Nome = Nothing
        _Revisione = Nothing
        _Modello_Codice = Nothing
        _Modello_Revisione = Nothing
        _DataValiditaInizio = Nothing
        _DataValiditaFine = Nothing
    End Sub

    Public Sub New(PivaSuperUser As String, _
                     Nome As String, _
                     Revisione As String, _
                     Modello_Codice As String, _
                     Modello_Revisione As String, _
                     DataValiditaInizio As DateTime?, _
                     DataValiditaFine As DateTime?)

        _PivaSuperUser = PivaSuperUser
        _Nome = Nome
        _Revisione = Revisione
        _Modello_Codice = Modello_Codice
        _Modello_Revisione = Modello_Revisione
        _DataValiditaInizio = DataValiditaInizio
        _DataValiditaFine = DataValiditaFine
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

    Public Property Revisione() As String
        Get
            Return _Revisione
        End Get
        Set(ByVal value As String)
            _Revisione = value
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

    Public Property DataValiditaInizio() As DateTime?
        Get
            Return _DataValiditaInizio
        End Get
        Set(ByVal value As DateTime?)
            _DataValiditaInizio = value
        End Set
    End Property

    Public Property DataValiditaFine() As DateTime?
        Get
            Return _DataValiditaFine
        End Get
        Set(ByVal value As DateTime?)
            _DataValiditaFine = value
        End Set
    End Property

End Class