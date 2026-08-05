Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreLabControlloQualitaDAL

Public Class ModelliRev_R
    Public Function leggiListaModelli( _
                              ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                              ByVal TipoMod As enum_LCQ_TipoModelloStandard, _
                              ByVal DataDaVerificare As DateTime _
                              ) As List(Of String)

        Dim m As New AgronicaCoreLabControlloQualitaDAL.ModelliRev_R
        Dim listaModelli As New List(Of String)
        Dim dt As DataTable

        dt = m.Leggi_ModelliRev(TipoMod, DataDaVerificare, Nothing, Nothing, "", "", objParametri)

        For Each row As DataRow In dt.Rows
            listaModelli.Add(row("Codice") & " " & row("Revisione"))
        Next

        Return listaModelli

    End Function

    Public Function leggi_LCQ_ModelliRev( _
                              ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                              ByVal TipoMod As enum_LCQ_TipoModelloStandard, _
                              ByVal DataDaVerificare As DateTime, _
                              Optional ByVal codice As String = Nothing, _
                              Optional ByVal revisione As String = Nothing _
                              ) As List(Of LCQ_ModelliRev)

        Dim m As New AgronicaCoreLabControlloQualitaDAL.ModelliRev_R
        Dim listaObjLCQ As New List(Of LCQ_ModelliRev)

        Dim dt As DataTable = m.Leggi_ModelliRev(TipoMod, DataDaVerificare, codice, revisione, "", "", objParametri)

        For Each dRow As DataRow In dt.Rows
            Dim lcq As New LCQ_ModelliRev( _
                                        dRow("PivaSuperUser"), _
                                        dRow("Codice"), _
                                        dRow("Revisione"), _
                                        UtilityProvider.DBNullToNothing(dRow("DataValiditaInizio")), _
                                        UtilityProvider.DBNullToNothing(dRow("DataValiditaFine")) _
                                    )
            listaObjLCQ.Add(lcq)
        Next

        Return listaObjLCQ

    End Function

    Public Function leggi_LCQ_ModelliRevTutti( _
                              ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                              Optional ByVal codice As String = Nothing, _
                              Optional ByVal revisione As String = Nothing _
                              ) As List(Of LCQ_ModelliRev)

        Dim listaObjLCQ As List(Of LCQ_ModelliRev) = leggi_LCQ_ModelliRev(objParametri, enum_LCQ_TipoModelloStandard.Tutti, DateTime.Now, codice, revisione)

        Return listaObjLCQ

    End Function

    Function test_ModRevEsiste( _
                              ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                              ByVal Modello_Codice As String, _
                              ByVal Modello_Revisione As String _
                              ) As Boolean

        Dim m As New AgronicaCoreLabControlloQualitaDAL.ModelliRev_R
        Dim res As Boolean = True

        res = m.Verifica_ModRevEsiste(Modello_Codice, Modello_Revisione, objParametri)

        Return res
    End Function

    Function test_ModRevInUsoDoc( _
                              ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                              ByVal Modello_Codice As String, _
                              ByVal Modello_Revisione As String _
                              ) As Boolean

        Dim m As New AgronicaCoreLabControlloQualitaDAL.ModelliRev_R
        Dim res As Boolean = True

        res = m.Verifica_ModRevInUsoDoc(Modello_Codice, Modello_Revisione, objParametri)

        Return res
    End Function

    Function test_ModRevInUsoPrm( _
                              ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                              ByVal Modello_Codice As String, _
                              ByVal Modello_Revisione As String _
                              ) As Boolean

        Dim m As New AgronicaCoreLabControlloQualitaDAL.ModelliRev_R
        Dim res As Boolean = True

        res = m.Verifica_ModRevInUsoPrm(Modello_Codice, Modello_Revisione, objParametri)

        Return res
    End Function

    Function test_ModRevInUsoStd( _
                              ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                              ByVal Modello_Codice As String, _
                              ByVal Modello_Revisione As String _
                              ) As Boolean

        Dim m As New AgronicaCoreLabControlloQualitaDAL.ModelliRev_R
        Dim res As Boolean = True

        res = m.Verifica_ModRevInUsoStd(Modello_Codice, Modello_Revisione, objParametri)

        Return res
    End Function

    Function test_ModRevAttivoDaAttivareInData( _
                              ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                              ByVal Modello_Codice As String, _
                              ByVal Modello_Revisione As String, _
                              ByVal DataDaVerificare As DateTime _
                              ) As Boolean

        Dim m As New AgronicaCoreLabControlloQualitaDAL.ModelliRev_R
        Dim res As Boolean = True

        res = m.Verifica_ModRevAttivoDaAttivareInData(Modello_Codice, Modello_Revisione, DataDaVerificare, objParametri)

        Return res
    End Function

End Class


Public Class ModelliRev_W

    Public Function aggiungi(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                           ByVal modello As LCQ_ModelliRev
                              ) As Boolean

        Dim m As New AgronicaCoreLabControlloQualitaDAL.ModelliRev_W
        Dim res As Boolean = False

        res = m.Scrivi(objParametri, _
                       modello.Codice, modello.Revisione, _
                       modello.DataValiditaInizio, modello.DataValiditaFine)

        Return res

    End Function

    Public Function aggiungi(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                            ByVal Codice As String, _
                            ByVal Revisione As String, _
                            ByVal DataValiditaInizio As DateTime?, _
                            ByVal DataValiditaFine As DateTime? _
                              ) As Boolean

        Dim m As New AgronicaCoreLabControlloQualitaDAL.ModelliRev_W
        Dim res As Boolean = False

        res = m.Scrivi(objParametri, _
                       Codice, Revisione, _
                       DataValiditaInizio, DataValiditaFine)

        Return res

    End Function

    Public Function modifica(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                                ByVal Old_Codice As String, _
                                ByVal Old_Revisione As String, _
                                ByVal New_Codice As String, _
                                ByVal New_Revisione As String, _
                                ByVal New_DataValiditaInizio As DateTime?, _
                                ByVal New_DataValiditaFine As DateTime? _
                              ) As Boolean

        Dim m As New AgronicaCoreLabControlloQualitaDAL.ModelliRev_W
        Dim res As Boolean = False

        res = m.Modifica(objParametri, Old_Codice, Old_Revisione, _
                            New_Codice, New_Revisione, _
                            New_DataValiditaInizio, New_DataValiditaFine)

        Return res

    End Function

End Class

Public Class LCQ_ModelliRev

    Private _PivaSuperUser As String
    Private _Codice As String
    Private _Revisione As String
    Private _DataValiditaInizio As DateTime?
    Private _DataValiditaFine As DateTime?

    Public Sub New()
        _PivaSuperUser = Nothing
        _Codice = Nothing
        _Revisione = Nothing
        _DataValiditaInizio = Nothing
        _DataValiditaFine = Nothing
    End Sub

    Public Sub New(PivaSuperUser As String, _
                    Codice As String, _
                    Revisione As String, _
                    DataValiditaInizio As DateTime?, _
                    DataValiditaFine As DateTime?)

        _PivaSuperUser = PivaSuperUser
        _Codice = Codice
        _Revisione = Revisione
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

    Public Property Codice() As String
        Get
            Return _Codice
        End Get
        Set(ByVal value As String)
            _Codice = value
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