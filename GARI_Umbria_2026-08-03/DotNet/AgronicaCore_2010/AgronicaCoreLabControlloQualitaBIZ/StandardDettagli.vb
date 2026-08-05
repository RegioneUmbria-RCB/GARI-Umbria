Imports AgronicaCoreDataProvider
Imports AgronicaCoreLabControlloQualitaDAL

Public Class StandardDettagli_R

    Public Function leggi_LCQ_StandardDettagli( _
                              ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                              Optional ByVal Standard_Nome As String = Nothing, _
                              Optional ByVal Standard_Revisione As String = Nothing, _
                              Optional ByVal Modello_Codice As String = Nothing, _
                              Optional ByVal Modello_Revisione As String = Nothing, _
                              Optional ByVal PrmXMod_Cod As Integer? = Nothing _
                              ) As List(Of LCQ_StandardDettagli)

        Dim s As New AgronicaCoreLabControlloQualitaDAL.StandardDettagli_R
        Dim listaObjLCQ As New List(Of LCQ_StandardDettagli)

        Dim dt As DataTable = s.Leggi_StandardDettagli(Standard_Nome, Standard_Revisione, Modello_Codice, Modello_Revisione, PrmXMod_Cod, "", "", objParametri)

        For Each dRow As DataRow In dt.Rows
            Dim lcq As New LCQ_StandardDettagli( _
                                            dRow("PivaSuperUser"), _
                                            dRow("Standard_Nome"), _
                                            dRow("Standard_Revisione"), _
                                            dRow("Modello_Codice"), _
                                            dRow("Modello_Revisione"), _
                                            dRow("PrmXMod_Cod"), _
                                            UtilityProvider.DBNullToNothing(dRow("String_LimiteInf")), _
                                            UtilityProvider.DBNullToNothing(dRow("String_LimiteSup")), _
                                            UtilityProvider.DBNullToNothing(dRow("DateTime_LimiteInf")), _
                                            UtilityProvider.DBNullToNothing(dRow("DateTime_LimiteSup")), _
                                            UtilityProvider.DBNullToNothing(dRow("Int_LimiteInf")), _
                                            UtilityProvider.DBNullToNothing(dRow("Int_LimiteSup")), _
                                            UtilityProvider.DBNullToNothing(dRow("Float_LimiteInf")), _
                                            UtilityProvider.DBNullToNothing(dRow("Float_LimiteSup")) _
                                            )
            listaObjLCQ.Add(lcq)
        Next

        Return listaObjLCQ

    End Function

    Function test_StdUsaPrm( _
                              ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                              ByVal Modello_Codice As String, _
                              ByVal Modello_Revisione As String, _
                              ByVal PrmXMod_Cod As Integer _
                              ) As Boolean

        Dim s As New AgronicaCoreLabControlloQualitaDAL.StandardDettagli_R
        Dim res As Boolean = True

        res = s.Verifica_StdUsaPrm(Modello_Codice, Modello_Revisione, PrmXMod_Cod, objParametri)

        Return res
    End Function

End Class

Public Class StandardDettagli_W

    Public Function aggiungi(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                           ByVal standardDett As LCQ_StandardDettagli
                              ) As Boolean

        Dim s As New AgronicaCoreLabControlloQualitaDAL.StandardDettagli_W
        Dim res As Boolean = False

        res = s.Scrivi(objParametri, _
                       standardDett.Standard_Nome, standardDett.Standard_Revisione, _
                       standardDett.Modello_Codice, standardDett.Modello_Revisione, _
                       standardDett.PrmXMod_Cod, _
                       standardDett.String_LimiteInf, standardDett.String_LimiteSup, _
                       standardDett.DateTime_LimiteInf, standardDett.DateTime_LimiteSup, _
                       standardDett.Int_LimiteInf, standardDett.Int_LimiteSup, _
                       standardDett.Float_LimiteInf, standardDett.Float_LimiteSup)

        Return res

    End Function

    Public Function aggiungi(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                            ByVal Standard_Nome As String, _
                            ByVal Standard_Revisione As String, _
                            ByVal Modello_Codice As String, _
                            ByVal Modello_Revisione As String, _
                            ByVal PrmXMod_Cod As Integer, _
                            ByVal String_LimiteInf As String, _
                            ByVal String_LimiteSup As String, _
                            ByVal DateTime_LimiteInf As DateTime?, _
                            ByVal DateTime_LimiteSup As DateTime?, _
                            ByVal Int_LimiteInf As Integer?, _
                            ByVal Int_LimiteSup As Integer?, _
                            ByVal Float_LimiteInf As Double?, _
                            ByVal Float_LimiteSup As Double? _
                              ) As Boolean

        Dim s As New AgronicaCoreLabControlloQualitaDAL.StandardDettagli_W
        Dim res As Boolean = False

        res = s.Scrivi(objParametri, _
                       Standard_Nome, Standard_Revisione, _
                       Modello_Codice, Modello_Revisione, _
                       PrmXMod_Cod, _
                       String_LimiteInf, String_LimiteSup, _
                       DateTime_LimiteInf, DateTime_LimiteSup, _
                       Int_LimiteInf, Int_LimiteSup, _
                       Float_LimiteInf, Float_LimiteSup)

        Return res

    End Function

    Public Function modifica(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                                ByVal Old_Standard_Nome As String, _
                                ByVal Old_Standard_Revisione As String, _
                                ByVal Old_Modello_Codice As String, _
                                ByVal Old_Modello_Revisione As String, _
                                ByVal Old_PrmXMod_Cod As Integer, _
                                ByVal New_String_LimiteInf As String, _
                                ByVal New_String_LimiteSup As String, _
                                ByVal New_DateTime_LimiteInf As DateTime?, _
                                ByVal New_DateTime_LimiteSup As DateTime?, _
                                ByVal New_Int_LimiteInf As Integer?, _
                                ByVal New_Int_LimiteSup As Integer?, _
                                ByVal New_Float_LimiteInf As Double?, _
                                ByVal New_Float_LimiteSup As Double? _
                              ) As Boolean

        Dim s As New AgronicaCoreLabControlloQualitaDAL.StandardDettagli_W
        Dim res As Boolean = False

        res = s.Modifica(objParametri, Old_Standard_Nome, Old_Standard_Revisione, _
                            Old_Modello_Codice, Old_Modello_Revisione, Old_PrmXMod_Cod, _
                            New_String_LimiteInf, New_String_LimiteSup, _
                            New_DateTime_LimiteInf, New_DateTime_LimiteSup, _
                            New_Int_LimiteInf, New_Int_LimiteSup, _
                            New_Float_LimiteInf, New_Float_LimiteSup, )

        Return res

    End Function

    Public Function cancellaTuttiDettagliStd(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                                                ByVal Standard_Nome As String, _
                                                ByVal Standard_Revisione As String, _
                                                ByVal Modello_Codice As String, _
                                                ByVal Modello_Revisione As String _
                                            ) As Boolean

        Dim s As New AgronicaCoreLabControlloQualitaDAL.StandardDettagli_W
        Dim res As Boolean = False

        'salvo il precendente stato
        Dim canc As AgronicaCoreParametri.enumCancellazioneLogica = objParametri.FlagCancellazioneLogica
        'assegno la cancellazione fisica
        objParametri.FlagCancellazioneLogica = AgronicaCoreParametri.enumCancellazioneLogica.CancellazioneFisica

        res = s.Cancella(objParametri, Standard_Nome, Standard_Revisione, Modello_Codice, Modello_Revisione, "")

        'ripristino il precedente stato
        objParametri.FlagCancellazioneLogica = canc

        Return res
    End Function

    Public Function cancellaDettaglioStd(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                                                ByVal Standard_Nome As String, _
                                                ByVal Standard_Revisione As String, _
                                                ByVal Modello_Codice As String, _
                                                ByVal Modello_Revisione As String, _
                                                ByVal PrmXMod_Cod As Integer _
                                            ) As Boolean

        Dim s As New AgronicaCoreLabControlloQualitaDAL.StandardDettagli_W
        Dim res As Boolean = False

        'salvo il precendente stato
        Dim canc As AgronicaCoreParametri.enumCancellazioneLogica = objParametri.FlagCancellazioneLogica
        'assegno la cancellazione fisica
        objParametri.FlagCancellazioneLogica = AgronicaCoreParametri.enumCancellazioneLogica.CancellazioneFisica

        res = s.Cancella(objParametri, Standard_Nome, Standard_Revisione, Modello_Codice, Modello_Revisione, PrmXMod_Cod, "")

        'ripristino il precedente stato
        objParametri.FlagCancellazioneLogica = canc

        Return res
    End Function

End Class



Public Class LCQ_StandardDettagli
    Private _PivaSuperUser As String
    Private _Standard_Nome As String
    Private _Standard_Revisione As String
    Private _Modello_Codice As String
    Private _Modello_Revisione As String
    Private _PrmXMod_Cod As Integer
    Private _String_LimiteInf As String
    Private _String_LimiteSup As String
    Private _DateTime_LimiteInf As DateTime?
    Private _DateTime_LimiteSup As DateTime?
    Private _Int_LimiteInf As Integer?
    Private _Int_LimiteSup As Integer?
    Private _Float_LimiteInf As Double?
    Private _Float_LimiteSup As Double?

    Public Sub New()
        _PivaSuperUser = Nothing
        _Standard_Nome = Nothing
        _Standard_Revisione = Nothing
        _Modello_Codice = Nothing
        _Modello_Revisione = Nothing
        _PrmXMod_Cod = Nothing
        _String_LimiteInf = Nothing
        _String_LimiteSup = Nothing
        _DateTime_LimiteInf = Nothing
        _DateTime_LimiteSup = Nothing
        _Int_LimiteInf = Nothing
        _Int_LimiteSup = Nothing
        _Float_LimiteInf = Nothing
        _Float_LimiteSup = Nothing
    End Sub

    Public Sub New(PivaSuperUser As String, _
                    Standard_Nome As String, _
                    Standard_Revisione As String, _
                    Modello_Codice As String, _
                    Modello_Revisione As String, _
                    PrmXMod_Cod As Integer, _
                    String_LimiteInf As String, _
                    String_LimiteSup As String, _
                    DateTime_LimiteInf As DateTime?, _
                    DateTime_LimiteSup As DateTime?, _
                    Int_LimiteInf As Integer?, _
                    Int_LimiteSup As Integer?, _
                    Float_LimiteInf As Double?, _
                    Float_LimiteSup As Double?)

        _PivaSuperUser = PivaSuperUser
        _Standard_Nome = Standard_Nome
        _Standard_Revisione = Standard_Revisione
        _Modello_Codice = Modello_Codice
        _Modello_Revisione = Modello_Revisione
        _PrmXMod_Cod = PrmXMod_Cod
        _String_LimiteInf = String_LimiteInf
        _String_LimiteSup = String_LimiteSup
        _DateTime_LimiteInf = DateTime_LimiteInf
        _DateTime_LimiteSup = DateTime_LimiteSup
        _Int_LimiteInf = Int_LimiteInf
        _Int_LimiteSup = Int_LimiteSup
        _Float_LimiteInf = Float_LimiteInf
        _Float_LimiteSup = Float_LimiteSup

    End Sub

    Public Property PivaSuperUser() As String
        Get
            Return _PivaSuperUser
        End Get
        Set(ByVal value As String)
            _PivaSuperUser = value
        End Set
    End Property

    Public Property Standard_Nome() As String
        Get
            Return _Standard_Nome
        End Get
        Set(ByVal value As String)
            _Standard_Nome = value
        End Set
    End Property

    Public Property Standard_Revisione() As String
        Get
            Return _Standard_Revisione
        End Get
        Set(ByVal value As String)
            _Standard_Revisione = value
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

    Public Property PrmXMod_Cod() As Integer
        Get
            Return _PrmXMod_Cod
        End Get
        Set(ByVal value As Integer)
            _PrmXMod_Cod = value
        End Set
    End Property

    Public Property String_LimiteInf() As String
        Get
            Return _String_LimiteInf
        End Get
        Set(ByVal value As String)
            _String_LimiteInf = value
        End Set
    End Property

    Public Property String_LimiteSup() As String
        Get
            Return _String_LimiteSup
        End Get
        Set(ByVal value As String)
            _String_LimiteSup = value
        End Set
    End Property

    Public Property DateTime_LimiteInf() As DateTime?
        Get
            Return _DateTime_LimiteInf
        End Get
        Set(ByVal value As DateTime?)
            _DateTime_LimiteInf = value
        End Set
    End Property

    Public Property DateTime_LimiteSup() As DateTime?
        Get
            Return _DateTime_LimiteSup
        End Get
        Set(ByVal value As DateTime?)
            _DateTime_LimiteSup = value
        End Set
    End Property

    Public Property Int_LimiteInf() As Integer?
        Get
            Return _Int_LimiteInf
        End Get
        Set(ByVal value As Integer?)
            _Int_LimiteInf = value
        End Set
    End Property

    Public Property Int_LimiteSup() As Integer?
        Get
            Return _Int_LimiteSup
        End Get
        Set(ByVal value As Integer?)
            _Int_LimiteSup = value
        End Set
    End Property

    Public Property Float_LimiteInf() As Double?
        Get
            Return _Float_LimiteInf
        End Get
        Set(ByVal value As Double?)
            _Float_LimiteInf = value
        End Set
    End Property

    Public Property Float_LimiteSup() As Double?
        Get
            Return _Float_LimiteSup
        End Get
        Set(ByVal value As Double?)
            _Float_LimiteSup = value
        End Set
    End Property

End Class
