Imports AgronicaCoreDataProvider
Imports AgronicaCoreLabControlloQualitaDAL

Public Class ParametriXModelli_R

    Public Function leggi_LCQ_ParametriXModelli( _
                              ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                              Optional ByVal Modello_Codice As String = Nothing, _
                              Optional ByVal Modello_Revisione As String = Nothing _
                              ) As List(Of LCQ_ParametriXModelli)

        Dim pm As New AgronicaCoreLabControlloQualitaDAL.ParametriXModelli_R
        Dim listaObjLCQ As New List(Of LCQ_ParametriXModelli)

        Dim dt As DataTable = pm.Leggi_ParametriXModelli(Nothing, Modello_Codice, Modello_Revisione, Nothing, Nothing, "", "OrdineSequenza", objParametri)

        For Each dRow As DataRow In dt.Rows
            Dim lcq As New LCQ_ParametriXModelli( _
                                            dRow("PivaSuperUser"), _
                                            dRow("ParametriXModelli_Cod"), _
                                            dRow("Modello_Codice"), _
                                            dRow("Modello_Revisione"), _
                                            dRow("Parametro_Cod"), _
                                            dRow("OrdineSequenza") _
                                          )
            listaObjLCQ.Add(lcq)
        Next

        Return listaObjLCQ

    End Function

    Public Function leggi_LCQ_ParametriXModelliDaPxm( _
                              ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                              ByVal ParametriXModelli_Cod As Integer _
                              ) As List(Of LCQ_ParametriXModelli)

        Dim pm As New AgronicaCoreLabControlloQualitaDAL.ParametriXModelli_R
        Dim listaObjLCQ As New List(Of LCQ_ParametriXModelli)

        Dim dt As DataTable = pm.Leggi_ParametriXModelli(ParametriXModelli_Cod, Nothing, Nothing, Nothing, Nothing, "", "OrdineSequenza", objParametri)

        For Each dRow As DataRow In dt.Rows
            Dim lcq As New LCQ_ParametriXModelli( _
                                            dRow("PivaSuperUser"), _
                                            dRow("ParametriXModelli_Cod"), _
                                            dRow("Modello_Codice"), _
                                            dRow("Modello_Revisione"), _
                                            dRow("Parametro_Cod"), _
                                            dRow("OrdineSequenza") _
                                          )
            listaObjLCQ.Add(lcq)
        Next

        Return listaObjLCQ

    End Function

    Public Function leggi_LCQ_ParametriXModelliDaPrm( _
                              ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                              ByVal Parametro_Cod As Integer _
                              ) As List(Of LCQ_ParametriXModelli)

        Dim pm As New AgronicaCoreLabControlloQualitaDAL.ParametriXModelli_R
        Dim listaObjLCQ As New List(Of LCQ_ParametriXModelli)

        Dim dt As DataTable = pm.Leggi_ParametriXModelli(Nothing, Nothing, Nothing, Parametro_Cod, Nothing, "", "OrdineSequenza", objParametri)

        For Each dRow As DataRow In dt.Rows
            Dim lcq As New LCQ_ParametriXModelli( _
                                            dRow("PivaSuperUser"), _
                                            dRow("ParametriXModelli_Cod"), _
                                            dRow("Modello_Codice"), _
                                            dRow("Modello_Revisione"), _
                                            dRow("Parametro_Cod"), _
                                            dRow("OrdineSequenza") _
                                          )
            listaObjLCQ.Add(lcq)
        Next

        Return listaObjLCQ

    End Function
End Class

Public Class ParametriXModelli_W

    Public Function aggiungi(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                            Modello_Codice As String, _
                            Modello_Revisione As String, _
                            Parametro_Cod As Integer, _
                            OrdineSequenza As Integer) As Integer?

        Dim pXm_W As New AgronicaCoreLabControlloQualitaDAL.ParametriXModelli_W
        Dim res As Boolean = False

        '  Prendo l'indice dalle agrosequenze
        Dim seq As New Agro_Sequenze()
        Dim ParametriXModelli_Cod As Integer = seq.NuovoId_Tabella("LCQ_ParametriXModelli", 100, 2000000000, objParametri)

        res = pXm_W.Scrivi(objParametri, ParametriXModelli_Cod, _
                       Modello_Codice, Modello_Revisione, _
                       Parametro_Cod, OrdineSequenza)

        If res Then ' se è andato tutto bene...
            Return ParametriXModelli_Cod '... ritorno il nuovo indice
        Else
            Return Nothing
        End If

    End Function

    Public Function modificaOrdine(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                                ByVal Old_ParametriXModelli_Cod As Integer, _
                                ByVal New_OrdineSequenza As Integer _
                              ) As Boolean

        Dim pXm_W As New AgronicaCoreLabControlloQualitaDAL.ParametriXModelli_W
        Dim res As Boolean = False

        res = pXm_W.Modifica(objParametri, Old_ParametriXModelli_Cod, New_OrdineSequenza)

        Return res

    End Function

    Public Function cancella(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                                                ByVal ParametriXModelli_Cod As Integer _
                                            ) As Boolean

        Dim p As New AgronicaCoreLabControlloQualitaDAL.ParametriXModelli_W
        Dim res As Boolean = False

        'salvo il precendente stato
        Dim canc As AgronicaCoreParametri.enumCancellazioneLogica = objParametri.FlagCancellazioneLogica
        'assegno la cancellazione fisica
        objParametri.FlagCancellazioneLogica = AgronicaCoreParametri.enumCancellazioneLogica.CancellazioneFisica

        res = p.Cancella(objParametri, ParametriXModelli_Cod, "")

        'ripristino il precedente stato
        objParametri.FlagCancellazioneLogica = canc

        Return res
    End Function

End Class


Public Class LCQ_ParametriXModelli
    Private _PivaSuperUser As String
    Private _ParametriXModelli_Cod As Integer
    Private _Modello_Codice As String
    Private _Modello_Revisione As String
    Private _Parametro_Cod As Integer
    Private _OrdineSequenza As Integer

    Public Sub New()
        _PivaSuperUser = Nothing
        _ParametriXModelli_Cod = Nothing
        _Modello_Codice = Nothing
        _Modello_Revisione = Nothing
        _Parametro_Cod = Nothing
        _OrdineSequenza = Nothing
    End Sub

    Public Sub New(PivaSuperUser As String, _
                    ParametriXModelli_Cod As Integer, _
                    Modello_Codice As String, _
                    Modello_Revisione As String, _
                    Parametro_Cod As Integer, _
                    OrdineSequenza As Integer?)

        _PivaSuperUser = PivaSuperUser
        _ParametriXModelli_Cod = ParametriXModelli_Cod
        _Modello_Codice = Modello_Codice
        _Modello_Revisione = Modello_Revisione
        _Parametro_Cod = Parametro_Cod
        _OrdineSequenza = OrdineSequenza
    End Sub

    Public Property PivaSuperUser() As String
        Get
            Return _PivaSuperUser
        End Get
        Set(ByVal value As String)
            _PivaSuperUser = value
        End Set
    End Property

    Public Property ParametriXModelli_Cod() As Integer
        Get
            Return _ParametriXModelli_Cod
        End Get
        Set(ByVal value As Integer)
            _ParametriXModelli_Cod = value
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

    Public Property Parametro_Cod() As Integer
        Get
            Return _Parametro_Cod
        End Get
        Set(ByVal value As Integer)
            _Parametro_Cod = value
        End Set
    End Property

    Public Property OrdineSequenza() As Integer
        Get
            Return _OrdineSequenza
        End Get
        Set(ByVal value As Integer)
            _OrdineSequenza = value
        End Set
    End Property



End Class

