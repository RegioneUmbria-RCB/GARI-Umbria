Imports AgronicaCoreDataProvider
Imports AgronicaCoreLabControlloQualitaDAL

Public Class Parametri_R

    Public Function leggi_LCQ_Parametri( _
                              ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                              ByVal Parametri_Cod As Integer _
                              ) As LCQ_Parametri

        Dim p As New AgronicaCoreLabControlloQualitaDAL.Parametri_R
        Dim listaObjLCQ As New List(Of LCQ_Parametri)

        Dim dt As DataTable = p.Leggi_Parametri(Parametri_Cod, Nothing, Nothing, Nothing, "", "", objParametri)

        For Each dRow As DataRow In dt.Rows
            Dim lcq As New LCQ_Parametri( _
                                          dRow("PivaSuperUser"), _
                                          dRow("Parametri_Cod"), _
                                          dRow("Nome"), _
                                          UtilityProvider.DBNullToNothing(dRow("Descrizione")), _
                                          dRow("DaValutare"), _
                                          dRow("CampoCalcolato"), _
                                          dRow("PrmTipi_CodTipo"), _
                                          UtilityProvider.DBNullToNothing(dRow("UnitaMisura")), _
                                          UtilityProvider.DBNullToNothing(dRow("Param1")), _
                                          UtilityProvider.DBNullToNothing(dRow("Param2")) _
                                          )
            listaObjLCQ.Add(lcq)
        Next

        Return listaObjLCQ(0)

    End Function

    Public Function leggi_LCQ_Parametri( _
                              ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                              Optional ByVal DaValutare As Boolean? = Nothing, _
                              Optional ByVal CampoCalcolato As Boolean? = Nothing _
                              ) As List(Of LCQ_Parametri)

        Dim p As New AgronicaCoreLabControlloQualitaDAL.Parametri_R
        Dim listaObjLCQ As New List(Of LCQ_Parametri)

        Dim dt As DataTable = p.Leggi_Parametri(Nothing, Nothing, DaValutare, CampoCalcolato, "", "Nome", objParametri)

        For Each dRow As DataRow In dt.Rows
            Dim lcq As New LCQ_Parametri( _
                                          dRow("PivaSuperUser"), _
                                          dRow("Parametri_Cod"), _
                                          dRow("Nome"), _
                                          UtilityProvider.DBNullToNothing(dRow("Descrizione")), _
                                          dRow("DaValutare"), _
                                          dRow("CampoCalcolato"), _
                                          dRow("PrmTipi_CodTipo"), _
                                          UtilityProvider.DBNullToNothing(dRow("UnitaMisura")), _
                                          UtilityProvider.DBNullToNothing(dRow("Param1")), _
                                          UtilityProvider.DBNullToNothing(dRow("Param2")) _
                                          )
            listaObjLCQ.Add(lcq)
        Next

        Return listaObjLCQ

    End Function

    Public Function leggi_LCQ_Parametri( _
                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                             ByVal Modello_Codice As String, _
                             ByVal Modello_Revisione As String, _
                             Optional ByVal DaValutare As Boolean? = Nothing, _
                             Optional ByVal CampoCalcolato As Boolean? = Nothing _
                             ) As List(Of LCQ_Parametri)

        Dim p As New AgronicaCoreLabControlloQualitaDAL.Parametri_R
        Dim listaObjLCQ As New List(Of LCQ_Parametri)

        Dim dt As DataTable = p.Leggi_Parametri(Nothing, Nothing, DaValutare, CampoCalcolato, Modello_Codice, Modello_Revisione, "", "OrdineSequenza", objParametri)

        For Each dRow As DataRow In dt.Rows
            Dim lcq As New LCQ_Parametri( _
                                          dRow("PivaSuperUser"), _
                                          dRow("Parametri_Cod"), _
                                          dRow("Nome"), _
                                          UtilityProvider.DBNullToNothing(dRow("Descrizione")), _
                                          dRow("DaValutare"), _
                                          dRow("CampoCalcolato"), _
                                          dRow("PrmTipi_CodTipo"), _
                                          UtilityProvider.DBNullToNothing(dRow("UnitaMisura")), _
                                          UtilityProvider.DBNullToNothing(dRow("Param1")), _
                                          UtilityProvider.DBNullToNothing(dRow("Param2")) _
                                          )
            listaObjLCQ.Add(lcq)
        Next

        Return listaObjLCQ

    End Function

    Function test_PrmInUso( _
                              ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                              ByVal Parametro_Cod As Integer _
                              ) As Boolean

        Dim p As New AgronicaCoreLabControlloQualitaDAL.Parametri_R
        Dim res As Boolean = True

        res = p.Verifica_PrmInUso(Parametro_Cod, objParametri)

        Return res
    End Function

    Function test_PrmEsiste(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                            ByVal Nome As String, _
                            ByVal DaValutare As Boolean? _
                            ) As Boolean

        Dim p As New AgronicaCoreLabControlloQualitaDAL.Parametri_R
        Dim res As Boolean = True

        res = p.Verifica_PrmEsiste(Nome, DaValutare, objParametri)

        Return res
    End Function

End Class

Public Class Parametri_W

    ' il campoCalcolato può essere inserito solo da DB
    Public Function aggiungi(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                            ByVal Nome As String, _
                            ByVal Descrizione As String, _
                            ByVal DaValutare As Boolean, _
                            ByVal PrmTipi_CodTipo As Integer, _
                            ByVal UnitaMisura As String, _
                            ByVal Param1 As String, _
                            ByVal Param2 As String) As Integer?

        Dim p As New AgronicaCoreLabControlloQualitaDAL.Parametri_W
        Dim res As Boolean = False

        '  Prendo l'indice dalle agrosequenze
        Dim seq As New Agro_Sequenze()
        Dim Parametri_Cod As Integer = seq.NuovoId_Tabella("LCQ_Parametri", 200, 2000000000, objParametri)

        res = p.Scrivi(objParametri, _
                       Parametri_Cod, Nome, Descrizione, _
                       DaValutare, False, PrmTipi_CodTipo, _
                       UnitaMisura, Param1, Param2)

        If res Then ' se è andato tutto bene...
            Return Parametri_Cod '... ritorno il nuovo indice
        Else
            Return Nothing
        End If

    End Function

    ' non metto il campoCalcolato in quanto viene gestito solo da DB
    Public Function modifica(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                            ByVal Old_Parametri_Cod As Integer, _
                            ByVal New_Parametri_Cod As Integer, _
                            ByVal New_Nome As String, _
                            ByVal New_Descrizione As String, _
                            ByVal New_DaValutare As Boolean, _
                            ByVal New_PrmTipi_CodTipo As Integer, _
                            ByVal New_UnitaMisura As String, _
                            ByVal New_Param1 As String, _
                            ByVal New_Param2 As String) As Boolean

        Dim p As New AgronicaCoreLabControlloQualitaDAL.Parametri_W
        Dim res As Boolean = False

        res = p.Modifica(objParametri, Old_Parametri_Cod, _
                            New_Parametri_Cod, New_Nome, New_Descrizione, _
                            New_DaValutare, _
                            New_PrmTipi_CodTipo, New_UnitaMisura, _
                            New_Param1, New_Param2)

        Return res

    End Function

End Class

Public Class LCQ_Parametri

    Private _PivaSuperUser As String
    Private _Parametri_Cod As Integer
    Private _Nome As String
    Private _Descrizione As String
    Private _DaValutare As Boolean
    Private _CampoCalcolato As Boolean
    Private _PrmTipi_CodTipo As Integer
    Private _UnitaMisura As String
    Private _Param1 As String
    Private _Param2 As String

    Public Sub New()
        _PivaSuperUser = Nothing
        _Parametri_Cod = Nothing
        _Nome = Nothing
        _Descrizione = Nothing
        _DaValutare = Nothing
        _CampoCalcolato = Nothing
        _PrmTipi_CodTipo = Nothing
        _Unitamisura = Nothing
        _Param1 = Nothing
        _Param2 = Nothing
    End Sub

    Public Sub New(PivaSuperUser As String, _
                    Parametri_Cod As Integer, _
                    Nome As String, _
                    Descrizione As String, _
                    DaValutare As Boolean, _
                    CampoCalcolato As Boolean, _
                    PrmTipi_CodTipo As Integer, _
                    UnitaMisura As String, _
                    Param1 As String, _
                    Param2 As String)

        _PivaSuperUser = PivaSuperUser
        _Parametri_Cod = Parametri_Cod
        _Nome = Nome
        _Descrizione = Descrizione
        _DaValutare = DaValutare
        _CampoCalcolato = CampoCalcolato
        _PrmTipi_CodTipo = PrmTipi_CodTipo
        _UnitaMisura = UnitaMisura
        _Param1 = Param1
        _Param2 = Param2
    End Sub


    Public Property PivaSuperUser() As String
        Get
            Return _PivaSuperUser
        End Get
        Set(ByVal value As String)
            _PivaSuperUser = value
        End Set
    End Property

    Public Property Parametri_Cod() As Integer
        Get
            Return _Parametri_Cod
        End Get
        Set(ByVal value As Integer)
            _Parametri_Cod = value
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

    Public Property DaValutare() As Boolean
        Get
            Return _DaValutare
        End Get
        Set(ByVal value As Boolean)
            _DaValutare = value
        End Set
    End Property

    Public Property CampoCalcolato() As Boolean
        Get
            Return _CampoCalcolato
        End Get
        Set(ByVal value As Boolean)
            _CampoCalcolato = value
        End Set
    End Property

    Public Property PrmTipi_CodTipo() As Integer
        Get
            Return _PrmTipi_CodTipo
        End Get
        Set(ByVal value As Integer)
            _PrmTipi_CodTipo = value
        End Set
    End Property

    Public Property UnitaMisura() As String
        Get
            Return _UnitaMisura
        End Get
        Set(ByVal value As String)
            _UnitaMisura = value
        End Set
    End Property

    Public Property Param1() As String
        Get
            Return _Param1
        End Get
        Set(ByVal value As String)
            _Param1 = value
        End Set
    End Property

    Public Property Param2() As String
        Get
            Return _Param2
        End Get
        Set(ByVal value As String)
            _Param2 = value
        End Set
    End Property

End Class