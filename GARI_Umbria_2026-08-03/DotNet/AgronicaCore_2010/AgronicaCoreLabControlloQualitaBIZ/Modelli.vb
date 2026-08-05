Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreLabControlloQualitaDAL

Public Class Modelli_R
    Public Function leggiListaModelli( _
                              ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                              Optional ByVal Ciclo_Nome As String = Nothing, _
                              Optional ByVal MatPrima_Nome As String = Nothing, _
                              Optional ByVal CatMerceologica As String = Nothing _
                              ) As List(Of String)

        Dim m As New AgronicaCoreLabControlloQualitaDAL.Modelli_R
        Dim listaModelli As New List(Of String)
        Dim dt As DataTable

        dt = m.Leggi_Modelli(Nothing, Nothing, Ciclo_Nome, MatPrima_Nome, CatMerceologica, "", "", objParametri)

        For Each row As DataRow In dt.Rows
            listaModelli.Add(row("Codice"))
        Next

        Return listaModelli

    End Function

    Public Function leggi_LCQ_Modelli( _
                              ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                              Optional ByVal codice As String = Nothing, _
                              Optional ByVal Ciclo_Nome As String = Nothing, _
                              Optional ByVal MatPrima_Nome As String = Nothing, _
                              Optional ByVal CatMerceologica As String = Nothing _
                              ) As List(Of LCQ_Modelli)

        Dim m As New AgronicaCoreLabControlloQualitaDAL.Modelli_R
        Dim listaObjLCQ As New List(Of LCQ_Modelli)

        Dim dt As DataTable = m.Leggi_Modelli(codice, Nothing, Ciclo_Nome, MatPrima_Nome, CatMerceologica, "", "", objParametri)

        For Each dRow As DataRow In dt.Rows
            Dim lcq As New LCQ_Modelli( _
                                        dRow("PivaSuperUser"), _
                                        dRow("Codice"), _
                                        dRow("Nome"), _
                                        UtilityProvider.DBNullToNothing(dRow("Descrizione")), _
                                        UtilityProvider.DBNullToNothing(dRow("Ciclo_Nome")), _
                                        UtilityProvider.DBNullToNothing(dRow("MatPrima_Nome")), _
                                        UtilityProvider.DBNullToNothing(dRow("CatMerceologica")) _
                                    )
            listaObjLCQ.Add(lcq)
        Next

        Return listaObjLCQ

    End Function

    Public Function leggi_LCQ_ModelliXproduzione( _
                              ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                              Optional ByVal codice As String = Nothing, _
                              Optional ByVal Ciclo_Nome As String = Nothing, _
                              Optional ByVal MatPrima_Nome As String = Nothing, _
                              Optional ByVal CatMerceologica As String = Nothing _
                              ) As List(Of LCQ_Modelli)

        Dim m As New AgronicaCoreLabControlloQualitaDAL.Modelli_R
        Dim listaObjLCQ As New List(Of LCQ_Modelli)

        Dim dt As DataTable = m.Leggi_ModelliXproduzione(codice, Nothing, _
                                        Ciclo_Nome, MatPrima_Nome, CatMerceologica, "", "", objParametri)

        For Each dRow As DataRow In dt.Rows
            Dim lcq As New LCQ_Modelli( _
                                        dRow("PivaSuperUser"), _
                                        dRow("Codice"), _
                                        dRow("Nome"), _
                                        UtilityProvider.DBNullToNothing(dRow("Descrizione")), _
                                        UtilityProvider.DBNullToNothing(dRow("Ciclo_Nome")), _
                                        UtilityProvider.DBNullToNothing(dRow("MatPrima_Nome")), _
                                        UtilityProvider.DBNullToNothing(dRow("CatMerceologica")) _
                                    )
            listaObjLCQ.Add(lcq)
        Next

        Return listaObjLCQ

    End Function

    Function test_ModEsiste(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                              ByVal Modello_Codice As String) As Boolean

        Dim m As New AgronicaCoreLabControlloQualitaDAL.Modelli_R
        Dim res As Boolean = True

        res = m.Verifica_ModEsiste(Modello_Codice, objParametri)

        Return res
    End Function

End Class


Public Class Modelli_W

    Public Function aggiungi(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                           ByVal modello As LCQ_Modelli
                              ) As Boolean

        Dim m As New AgronicaCoreLabControlloQualitaDAL.Modelli_W
        Dim res As Boolean = False

        res = m.Scrivi(objParametri, _
                       modello.Codice, modello.Nome, modello.Descrizione, _
                       modello.Ciclo_Nome, modello.MatPrima_Nome, modello.CatMerceologica)

        Return res

    End Function

    Public Function aggiungi(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                            ByVal Codice As String, _
                            ByVal Nome As String, _
                            ByVal Descrizione As String, _
                            ByVal Ciclo_Nome As String, _
                            ByVal MatPrima_Nome As String, _
                            ByVal CatMerceologica As String _
                              ) As Boolean

        Dim m As New AgronicaCoreLabControlloQualitaDAL.Modelli_W
        Dim res As Boolean = False

        res = m.Scrivi(objParametri, _
                       Codice, Nome, Descrizione, _
                       Ciclo_Nome, MatPrima_Nome, CatMerceologica)

        Return res

    End Function

    Public Function modifica(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                                ByVal Old_Codice As String, _
                                ByVal New_Codice As String, _
                                ByVal New_Nome As String, _
                                ByVal New_Descrizione As String, _
                                ByVal New_Ciclo_Nome As String, _
                                ByVal New_MatPrima_Nome As String, _
                                ByVal New_CatMerceologica As String _
                              ) As Boolean

        Dim m As New AgronicaCoreLabControlloQualitaDAL.Modelli_W
        Dim res As Boolean = False

        res = m.Modifica(objParametri, Old_Codice, _
                            New_Codice, New_Nome, New_Descrizione, _
                            New_Ciclo_Nome, New_MatPrima_Nome, New_CatMerceologica)

        Return res

    End Function

    Public Function modificaProduzione(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                                ByVal Old_Codice As String, _
                                ByVal New_Ciclo_Nome As String, _
                                ByVal New_MatPrima_Nome As String, _
                                ByVal New_CatMerceologica As String _
                              ) As Boolean

        Dim m As New AgronicaCoreLabControlloQualitaDAL.Modelli_W
        Dim res As Boolean = False

        res = m.ModificaProduzione(objParametri, Old_Codice, _
                            New_Ciclo_Nome, New_MatPrima_Nome, New_CatMerceologica)

        Return res

    End Function

End Class

Public Class LCQ_Modelli

    Private _PivaSuperUser As String
    Private _Codice As String
    Private _Nome As String
    Private _Descrizione As String
    Private _Ciclo_Nome As String
    Private _MatPrima_Nome As String
    Private _CatMerceologica As String

    Public Sub New()
        _PivaSuperUser = Nothing
        _Codice = Nothing
        _Nome = Nothing
        _Descrizione = Nothing
        _Ciclo_Nome = Nothing
        _MatPrima_Nome = Nothing
        _CatMerceologica = Nothing
    End Sub

    Public Sub New(PivaSuperUser As String, _
                    Codice As String, _
                    Nome As String, _
                    Descrizione As String, _
                    Ciclo_Nome As String, _
                    MatPrima_Nome As String, _
                    CatMerceologica As String)

        _PivaSuperUser = PivaSuperUser
        _Codice = Codice
        _Nome = Nome
        _Descrizione = Descrizione
        _Ciclo_Nome = Ciclo_Nome
        _MatPrima_Nome = MatPrima_Nome
        _CatMerceologica = CatMerceologica

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

    Public Property Nome() As String
        Get
            Return _Nome
        End Get
        Set(ByVal value As String)
            _Nome = value
        End Set
    End Property

    Public Property Descrizione() As String
        Get
            Return _Descrizione
        End Get
        Set(ByVal value As String)
            _Descrizione = value
        End Set
    End Property

    Public Property Ciclo_Nome() As String
        Get
            Return _Ciclo_Nome
        End Get
        Set(ByVal value As String)
            _Ciclo_Nome = value
        End Set
    End Property

    Public Property MatPrima_Nome() As String
        Get
            Return _MatPrima_Nome
        End Get
        Set(ByVal value As String)
            _MatPrima_Nome = value
        End Set
    End Property

    Public Property CatMerceologica() As String
        Get
            Return _CatMerceologica
        End Get
        Set(ByVal value As String)
            _CatMerceologica = value
        End Set
    End Property




End Class