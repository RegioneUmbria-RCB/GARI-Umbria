Imports AgronicaCoreDataProvider
Imports AgronicaCoreLabControlloQualitaDAL
Imports AgronicaCoreDataProvider.TipiEnumerativi

Public Class Articoli_R

    '##########################################
    Public Function leggi_LCQ_Articoli(ByVal Data_Inizio_filtroTemp As Date,
                                        ByVal Data_Fine_filtroTemp As Date,
                                      ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                      ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                      Optional ByVal codice As String = Nothing
                                      ) As List(Of LCQ_Articoli)

        Dim listaObjLCQ As New List(Of LCQ_Articoli)

        '10/01/2020: aggiunta lettura del filtro da applicare alle materie prime x area gias
        Dim objImp As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_FiltroMono_R
        Dim FiltroSQL As String
        FiltroSQL = objImp.RecuperaFiltroSQLMatPrime_from_AreaGIAS(enum_AreaGIAS.Qualita, objParametri_Utenti)

        'If FiltroSQL <> "" Then
        '    FiltroSQL &= " AND "
        'End If
        'FiltroSQL &= "Materie_Prime.Validita_inizio <= " & AgronicaCoreDataProvider.UtilityProvider.Agro_SQL_SaveDate(Date.Today)
        'FiltroSQL &= " AND Materie_Prime.Validita_Fine >= " & AgronicaCoreDataProvider.UtilityProvider.Agro_SQL_SaveDate(Date.Today)

        Dim cat_cod As Integer? = Nothing

        '------------------------
        'VECCHIA GESTIONE: si filtrano gli articoli per cat_cod=-1 
        'nella nuova gestione questo filtro andrà rimosso perchè si userà direttamente quello salvato nelle impostazioni utente
        'cat_cod = -1
        '------------------------

        Dim art_R As New AgronicaCoreLabControlloQualitaDAL.Articoli_R
        Dim dt As DataTable = art_R.LeggiFruttagelLabCQ(codice, Nothing, Nothing, Nothing, Nothing, Nothing,
                                                       Nothing, Nothing, Nothing, Nothing, Nothing,
                                                       Nothing,
                                                       Data_Inizio_filtroTemp,
                                                       Data_Fine_filtroTemp,
                                                       FiltroSQL, "",
                                                       objParametri_Server)

        For Each dRow As DataRow In dt.Rows
            Dim lcq As New LCQ_Articoli(
                                          dRow("PivaSuperUser"),
                                          dRow("Codice"),
                                          UtilityProvider.DBNullToNothing(dRow("Descrizione1")),
                                          UtilityProvider.DBNullToNothing(dRow("Descrizione2")),
                                          UtilityProvider.DBNullToNothing(dRow("Marchio")),
                                          UtilityProvider.DBNullToNothing(dRow("MatPrima_Nome")),
                                          UtilityProvider.DBNullToNothing(dRow("MatPrima_VegCod")),
                                          UtilityProvider.DBNullToNothing(dRow("MatPrima_CulCod")),
                                          UtilityProvider.DBNullToNothing(dRow("Formato")),
                                          UtilityProvider.DBNullToNothing(dRow("CatMerceologica")),
                                          UtilityProvider.DBNullToNothing(dRow("TipoProduzione")),
                                          UtilityProvider.DBNullToNothing(dRow("RegolaScadenza_Tipo")),
                                          UtilityProvider.DBNullToNothing(dRow("RegolaScadenza_Qta")),
                                          UtilityProvider.DBNullToNothing(dRow("Stato")),
                                          UtilityProvider.DBNullToNothing(dRow("LineaPDZ"))
                                          )
            listaObjLCQ.Add(lcq)
        Next

        '------------------------
        'VECCHIA GESTIONE: 
        'filtro i soli articoli che interessano al LabCQ
        '1 = bevande / 250 = SL Acquistati / 3 = pomodoro / 4 = surgelati / CQ = codici fittizi per LABCQ / 61 = ?
        '(nella nuova gestione questo filtro andrà rimosso perchè avverrà direttamente nella query tramite quello salvato nelle impostazioni utente)
        listaObjLCQ = listaObjLCQ.Where(Function(x) x.Codice.StartsWith("1") OrElse x.Codice.StartsWith("250") OrElse x.Codice.StartsWith("3") _
            OrElse x.Codice.StartsWith("4") OrElse x.Codice.StartsWith("CQ") OrElse x.Codice.StartsWith("61")).ToList()
        '-----------------------------------------------------------


        Return listaObjLCQ

    End Function


    Public Function leggi_LCQ_ArticoliPerDettagli(
                              ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                              ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                              ByVal Data_Inizio_filtroTemp As Date,
                              ByVal Data_Fine_filtroTemp As Date,
                              Optional ByVal Marchio As String = Nothing,
                              Optional ByVal MatPrima_Nome As String = Nothing,
                              Optional ByVal MatPrima_VegCod As Integer? = Nothing,
                              Optional ByVal MatPrima_CulCod As Integer? = Nothing,
                              Optional ByVal Formato As String = Nothing,
                              Optional ByVal CatMerceologica As String = Nothing,
                              Optional ByVal TipoProduzione As String = Nothing,
                              Optional ByVal RegolaScadenza_Tipo As String = Nothing,
                              Optional ByVal RegolaScadenza_Qta As String = Nothing
                              ) As List(Of LCQ_Articoli)

        Dim listaObjLCQ As New List(Of LCQ_Articoli)

        '10/01/2020: aggiunta lettura del filtro da applicare alle materie prime x area gias
        Dim objImp As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_FiltroMono_R
        Dim FiltroSQL As String
        FiltroSQL = objImp.RecuperaFiltroSQLMatPrime_from_AreaGIAS(enum_AreaGIAS.Qualita, objParametri_Utenti)

        Dim cat_cod As Integer? = Nothing

        '------------------------
        'VECCHIA GESTIONE: si filtrano gli articoli per cat_cod=-1 
        'nella nuova gestione questo filtro andrà rimosso perchè si userà direttamente quello salvato nelle impostazioni utente
        'cat_cod = -1
        '------------------------

        Dim art_R As New AgronicaCoreLabControlloQualitaDAL.Articoli_R
        Dim dt As DataTable = art_R.LeggiFruttagelLabCQ(Nothing, Marchio, MatPrima_Nome, MatPrima_VegCod, MatPrima_CulCod,
                                               Formato, CatMerceologica, TipoProduzione, RegolaScadenza_Tipo, RegolaScadenza_Qta, Nothing,
                                               Nothing,
                                               Data_Inizio_filtroTemp,
                                               Data_Fine_filtroTemp,
                                                FiltroSQL, "",
                                                objParametri_Server)

        For Each dRow As DataRow In dt.Rows
            Dim lcq As New LCQ_Articoli(
                                          dRow("PivaSuperUser"),
                                          dRow("Codice"),
                                          UtilityProvider.DBNullToNothing(dRow("Descrizione1")),
                                          UtilityProvider.DBNullToNothing(dRow("Descrizione2")),
                                          UtilityProvider.DBNullToNothing(dRow("Marchio")),
                                          UtilityProvider.DBNullToNothing(dRow("MatPrima_Nome")),
                                          UtilityProvider.DBNullToNothing(dRow("MatPrima_VegCod")),
                                          UtilityProvider.DBNullToNothing(dRow("MatPrima_CulCod")),
                                          UtilityProvider.DBNullToNothing(dRow("Formato")),
                                          UtilityProvider.DBNullToNothing(dRow("CatMerceologica")),
                                          UtilityProvider.DBNullToNothing(dRow("TipoProduzione")),
                                          UtilityProvider.DBNullToNothing(dRow("RegolaScadenza_Tipo")),
                                          UtilityProvider.DBNullToNothing(dRow("RegolaScadenza_Qta")),
                                          UtilityProvider.DBNullToNothing(dRow("Stato")),
                                          UtilityProvider.DBNullToNothing(dRow("LineaPDZ"))
                                          )
            listaObjLCQ.Add(lcq)
        Next

        '------------------------
        'VECCHIA GESTIONE: 
        'filtro i soli articoli che interessano al LabCQ
        '1 = bevande / 250 = SL Acquistati / 3 = pomodoro / 4 = surgelati / CQ = codici fittizi per LABCQ / 61 = ?
        '(nella nuova gestione questo filtro andrà rimosso perchè avverrà direttamente nella query tramite quello salvato nelle impostazioni utente)
        listaObjLCQ = listaObjLCQ.Where(Function(x) x.Codice.StartsWith("1") OrElse x.Codice.StartsWith("250") OrElse x.Codice.StartsWith("3") _
            OrElse x.Codice.StartsWith("4") OrElse x.Codice.StartsWith("CQ") OrElse x.Codice.StartsWith("61")).ToList()
        '-----------------------------------------------------------

        Return listaObjLCQ

    End Function

    Function leggi_LCQ_ArticoliPFoSLAcquistati_NonUsata(ByVal Data_Inizio_filtroTemp As Date,
                                                          ByVal Data_Fine_filtroTemp As Date,
                                                          ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                          ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                          ) As List(Of LCQ_Articoli)

        Dim art_R As New AgronicaCoreLabControlloQualitaDAL.Articoli_R
        Dim listaObjLCQ As New List(Of LCQ_Articoli)

        If objParametri_Server.PivaSuperUser = "01271980391" Then ' Fruttagel

            '10/01/2020: aggiunta lettura del filtro da applicare alle materie prime x area gias
            Dim objImp As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_FiltroMono_R
            Dim FiltroSQL As String
            FiltroSQL = objImp.RecuperaFiltroSQLMatPrime_from_AreaGIAS(enum_AreaGIAS.Qualita, objParametri_Utenti)

            Dim cat_cod As Integer? = Nothing

            '------------------------
            'VECCHIA GESTIONE: si filtrano gli articoli per cat_cod=-1 
            'nella nuova gestione questo filtro andrà rimosso perchè si userà direttamente quello salvato nelle impostazioni utente
            'cat_cod = -1
            '------------------------

            Dim dt As DataTable = art_R.LeggiFruttagelLabCQ(Nothing, Nothing, Nothing, Nothing, Nothing,
                                                  Nothing, "PRODOTTI FINITI DI NS. PRODUZ.", Nothing, Nothing, Nothing, Nothing,
                                                  Nothing,
                                                  Data_Inizio_filtroTemp,
                                                  Data_Fine_filtroTemp,
                                                  FiltroSQL, "", objParametri_Server)

            Dim dt2 As DataTable = art_R.LeggiFruttagelLabCQ(Nothing, Nothing, Nothing, Nothing, Nothing,
                                                   Nothing, "PRODOTTI FINITI ACQUISTATI", Nothing, Nothing, Nothing, Nothing,
                                                   Nothing,
                                                   Data_Inizio_filtroTemp,
                                                   Data_Fine_filtroTemp,
                                                   FiltroSQL, "", objParametri_Server)

            Dim dt3 As DataTable = art_R.LeggiFruttagelLabCQ(Nothing, Nothing, Nothing, Nothing, Nothing,
                                                   Nothing, "SEMILAVORATI ACQUISTATI", Nothing, Nothing, Nothing, Nothing,
                                                   Nothing,
                                                   Data_Inizio_filtroTemp,
                                                   Data_Fine_filtroTemp,
                                                   FiltroSQL, "", objParametri_Server)

            Dim dt4 As DataTable = art_R.LeggiFruttagelLabCQ(Nothing, Nothing, Nothing, Nothing, Nothing,
                                                   Nothing, "CODICI FITTIZI PER LAB. CQ", Nothing, Nothing, Nothing, Nothing,
                                                   Nothing,
                                                   Data_Inizio_filtroTemp,
                                                   Data_Fine_filtroTemp,
                                                   FiltroSQL, "", objParametri_Server)

            dt.Merge(dt2)
            dt.Merge(dt3)
            dt.Merge(dt4)
            For Each dRow As DataRow In dt.Rows
                Dim lcq As New LCQ_Articoli(
                                              dRow("PivaSuperUser"),
                                              dRow("Codice"),
                                              UtilityProvider.DBNullToNothing(dRow("Descrizione1")),
                                              UtilityProvider.DBNullToNothing(dRow("Descrizione2")),
                                              UtilityProvider.DBNullToNothing(dRow("Marchio")),
                                              UtilityProvider.DBNullToNothing(dRow("MatPrima_Nome")),
                                              UtilityProvider.DBNullToNothing(dRow("MatPrima_VegCod")),
                                              UtilityProvider.DBNullToNothing(dRow("MatPrima_CulCod")),
                                              UtilityProvider.DBNullToNothing(dRow("Formato")),
                                              UtilityProvider.DBNullToNothing(dRow("CatMerceologica")),
                                              UtilityProvider.DBNullToNothing(dRow("TipoProduzione")),
                                              UtilityProvider.DBNullToNothing(dRow("RegolaScadenza_Tipo")),
                                              UtilityProvider.DBNullToNothing(dRow("RegolaScadenza_Qta")),
                                              UtilityProvider.DBNullToNothing(dRow("Stato")),
                                              UtilityProvider.DBNullToNothing(dRow("LineaPDZ"))
                                              )
                listaObjLCQ.Add(lcq)
            Next
        End If

        '------------------------        
        'VECCHIA GESTIONE: 
        'filtro i soli articoli che interessano al LabCQ
        '1 = bevande / 250 = SL Acquistati / 3 = pomodoro / 4 = surgelati / CQ = codici fittizi per LABCQ / 61 = ?
        '(nella nuova gestione questo filtro andrà rimosso perchè avverrà direttamente nella query tramite quello salvato nelle impostazioni utente)
        listaObjLCQ = listaObjLCQ.Where(Function(x) x.Codice.StartsWith("1") OrElse x.Codice.StartsWith("250") OrElse x.Codice.StartsWith("3") _
            OrElse x.Codice.StartsWith("4") OrElse x.Codice.StartsWith("CQ") OrElse x.Codice.StartsWith("61")).ToList()
        '-----------------------------------------------------------


        'ordino la lista
        listaObjLCQ = listaObjLCQ.OrderBy(Function(x) x.Codice).ToList()

        Return listaObjLCQ

    End Function

    Public Function leggi_NC_Articoli(
                                     ByVal Data_Inizio_filtroTemp As Date,
                                    ByVal Data_Fine_filtroTemp As Date,
                                     ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                     ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                     Optional ByVal codice As String = Nothing
                                     ) As List(Of LCQ_Articoli)

        Dim listaObjLCQ As New List(Of LCQ_Articoli)

        '10/01/2020: aggiunta lettura del filtro da applicare alle materie prime x area gias
        Dim objImp As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_FiltroMono_R
        Dim FiltroSQL As String
        FiltroSQL = objImp.RecuperaFiltroSQLMatPrime_from_AreaGIAS(enum_AreaGIAS.NonConformita, objParametri_Utenti)

        Dim cat_cod As Integer? = Nothing

        '------------------------
        'VECCHIA GESTIONE: si filtrano gli articoli per cat_cod=-1 
        'nella nuova gestione questo filtro andrà rimosso perchè si userà direttamente quello salvato nelle impostazioni utente
        'cat_cod = -1
        '------------------------

        Dim art_R As New AgronicaCoreLabControlloQualitaDAL.Articoli_R
        Dim dt As DataTable = art_R.LeggiFruttagelLabCQ(codice, Nothing, Nothing, Nothing, Nothing, Nothing,
                                                       Nothing, Nothing, Nothing, Nothing, Nothing,
                                                       Nothing,
                                                       Data_Inizio_filtroTemp,
                                                       Data_Fine_filtroTemp,
                                                        FiltroSQL, "",
                                                       objParametri_Server)

        For Each dRow As DataRow In dt.Rows
            Dim lcq As New LCQ_Articoli(
                                          dRow("PivaSuperUser"),
                                          dRow("Codice"),
                                          UtilityProvider.DBNullToNothing(dRow("Descrizione1")),
                                          UtilityProvider.DBNullToNothing(dRow("Descrizione2")),
                                          UtilityProvider.DBNullToNothing(dRow("Marchio")),
                                          UtilityProvider.DBNullToNothing(dRow("MatPrima_Nome")),
                                          UtilityProvider.DBNullToNothing(dRow("MatPrima_VegCod")),
                                          UtilityProvider.DBNullToNothing(dRow("MatPrima_CulCod")),
                                          UtilityProvider.DBNullToNothing(dRow("Formato")),
                                          UtilityProvider.DBNullToNothing(dRow("CatMerceologica")),
                                          UtilityProvider.DBNullToNothing(dRow("TipoProduzione")),
                                          UtilityProvider.DBNullToNothing(dRow("RegolaScadenza_Tipo")),
                                          UtilityProvider.DBNullToNothing(dRow("RegolaScadenza_Qta")),
                                          UtilityProvider.DBNullToNothing(dRow("Stato")),
                                          UtilityProvider.DBNullToNothing(dRow("LineaPDZ"))
                                          )
            listaObjLCQ.Add(lcq)
        Next

        '------------------------
        'VECCHIA GESTIONE: 
        'filtro i soli articoli che interessano a  NC
        '1 = bevande / 250 = SL Acquistati / 3 = pomodoro / 4 = surgelati / 5 = imballi / 600 = materia prima agricola / 610 = ?
        '(nella nuova gestione questo filtro andrà rimosso perchè avverrà direttamente nella query tramite quello salvato nelle impostazioni utente)
        listaObjLCQ = listaObjLCQ.Where(Function(x) x.Codice.StartsWith("1") OrElse x.Codice.StartsWith("250") OrElse x.Codice.StartsWith("3") _
                                         OrElse x.Codice.StartsWith("4") OrElse x.Codice.StartsWith("5") OrElse x.Codice.StartsWith("600") _
                                         OrElse x.Codice.StartsWith("610")).ToList()
        '-------------------------------------------------------------------------------------------------------

        Return listaObjLCQ

    End Function

    '10/01/2020: è usata da NC, ma sembra che qui non debba fare filtri,
    'quindi lascio commentata la lettura dell'impostazione utente
    Public Function leggi_Articoli(ByVal Data_Inizio_filtroTemp As Date,
                                  ByVal Data_Fine_filtroTemp As Date,
                                    ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                    ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                    Optional ByVal codice As String = Nothing
                                    ) As List(Of LCQ_Articoli)

        Dim listaObjLCQ As New List(Of LCQ_Articoli)


        ''10/01/2020: aggiunta lettura del filtro da applicare alle materie prime x area gias
        'Dim objImp As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_FiltroMono_R
        Dim FiltroSQL As String = ""
        'FiltroSQL = objImp.RecuperaFiltroSQLMatPrime_from_AreaGIAS(enum_AreaGIAS., objParametri_Utenti)

        'If FiltroSQL <> "" Then
        '    FiltroSQL &= " AND "
        'End If
        'FiltroSQL &= "Materie_Prime.Validita_inizio <= " & AgronicaCoreDataProvider.UtilityProvider.Agro_SQL_SaveDate(Date.Today)
        'FiltroSQL &= " AND Materie_Prime.Validita_Fine >= " & AgronicaCoreDataProvider.UtilityProvider.Agro_SQL_SaveDate(Date.Today)

        Dim cat_cod As Integer? = Nothing

        '------------------------
        'VECCHIA GESTIONE: si filtrano gli articoli per cat_cod=-1 
        'nella nuova gestione questo filtro andrà rimosso perchè si userà direttamente quello salvato nelle impostazioni utente
        'cat_cod = -1
        '------------------------

        Dim art_R As New AgronicaCoreLabControlloQualitaDAL.Articoli_R
        Dim dt As DataTable = art_R.LeggiFruttagelLabCQ(codice, Nothing, Nothing, Nothing, Nothing, Nothing,
                                                       Nothing, Nothing, Nothing, Nothing, Nothing,
                                                       Nothing,
                                                       Data_Inizio_filtroTemp,
                                                       Data_Fine_filtroTemp,
                                                        FiltroSQL, "",
                                                       objParametri_Server)

        For Each dRow As DataRow In dt.Rows
            Dim lcq As New LCQ_Articoli(
                                          dRow("PivaSuperUser"),
                                          dRow("Codice"),
                                          UtilityProvider.DBNullToNothing(dRow("Descrizione1")),
                                          UtilityProvider.DBNullToNothing(dRow("Descrizione2")),
                                          UtilityProvider.DBNullToNothing(dRow("Marchio")),
                                          UtilityProvider.DBNullToNothing(dRow("MatPrima_Nome")),
                                          UtilityProvider.DBNullToNothing(dRow("MatPrima_VegCod")),
                                          UtilityProvider.DBNullToNothing(dRow("MatPrima_CulCod")),
                                          UtilityProvider.DBNullToNothing(dRow("Formato")),
                                          UtilityProvider.DBNullToNothing(dRow("CatMerceologica")),
                                          UtilityProvider.DBNullToNothing(dRow("TipoProduzione")),
                                          UtilityProvider.DBNullToNothing(dRow("RegolaScadenza_Tipo")),
                                          UtilityProvider.DBNullToNothing(dRow("RegolaScadenza_Qta")),
                                          UtilityProvider.DBNullToNothing(dRow("Stato")),
                                          UtilityProvider.DBNullToNothing(dRow("LineaPDZ"))
                                          )
            listaObjLCQ.Add(lcq)
        Next

        Return listaObjLCQ

    End Function

End Class

Public Class LCQ_Articoli

    Private _PivaSuperUser As String
    Private _Codice As String
    Private _Descrizione1 As String
    Private _Descrizione2 As String
    Private _Marchio As String
    Private _MatPrima_Nome As String
    Private _MatPrima_VegCod As Integer?
    Private _MatPrima_CulCod As Integer?
    Private _Formato As String
    Private _CatMerceologica As String
    Private _TipoProduzione As String
    Private _RegolaScadenza_Tipo As String
    Private _RegolaScadenza_Qta As String
    Private _Stato As String
    Private _LineaPDZ As String


    Public Sub New()

        _PivaSuperUser = Nothing
        _Codice = Nothing
        _Descrizione1 = Nothing
        _Descrizione2 = Nothing
        _Marchio = Nothing
        _MatPrima_Nome = Nothing
        _MatPrima_VegCod = Nothing
        _MatPrima_CulCod = Nothing
        _Formato = Nothing
        _CatMerceologica = Nothing
        _TipoProduzione = Nothing
        _RegolaScadenza_Tipo = Nothing
        _RegolaScadenza_Qta = Nothing
        _Stato = Nothing
        _LineaPDZ = Nothing

    End Sub

    Public Sub New(PivaSuperUser As String, _
                    Codice As String, _
                    Descrizione1 As String, _
                    Descrizione2 As String, _
                    Marchio As String, _
                    MatPrima_Nome As String, _
                    MatPrima_VegCod As Integer?, _
                    MatPrima_CulCod As Integer?, _
                    Formato As String, _
                    CatMerceologica As String, _
                    TipoProduzione As String, _
                    RegolaScadenza_Tipo As String, _
                    RegolaScadenza_Qta As String, _
                    Stato As String, _
                    LineaPDZ As String _
                    )
        'Stato As String)

        _PivaSuperUser = PivaSuperUser
        _Codice = Codice
        _Descrizione1 = Descrizione1
        _Descrizione2 = Descrizione2
        _Marchio = Marchio
        _MatPrima_Nome = MatPrima_Nome
        _MatPrima_VegCod = MatPrima_VegCod
        _MatPrima_CulCod = MatPrima_CulCod
        _Formato = Formato
        _CatMerceologica = CatMerceologica
        _TipoProduzione = TipoProduzione
        _RegolaScadenza_Tipo = RegolaScadenza_Tipo
        _RegolaScadenza_Qta = RegolaScadenza_Qta
        _Stato = Stato
        _LineaPDZ = LineaPDZ
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

    Public Property Descrizione1() As String
        Get
            Return _Descrizione1
        End Get
        Set(ByVal value As String)
            _Descrizione1 = value
        End Set
    End Property

    Public Property Descrizione2() As String
        Get
            Return _Descrizione2
        End Get
        Set(ByVal value As String)
            _Descrizione2 = value
        End Set
    End Property

    Public Property Marchio() As String
        Get
            Return _Marchio
        End Get
        Set(ByVal value As String)
            _Marchio = value
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

    Public Property MatPrima_VegCod() As Integer?
        Get
            Return _MatPrima_VegCod
        End Get
        Set(ByVal value As Integer?)
            _MatPrima_VegCod = value
        End Set
    End Property

    Public Property MatPrima_CulCod() As Integer?
        Get
            Return _MatPrima_CulCod
        End Get
        Set(ByVal value As Integer?)
            _MatPrima_CulCod = value
        End Set
    End Property

    Public Property Formato() As String
        Get
            Return _Formato
        End Get
        Set(ByVal value As String)
            _Formato = value
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

    Public Property TipoProduzione() As String
        Get
            Return _TipoProduzione
        End Get
        Set(ByVal value As String)
            _TipoProduzione = value
        End Set
    End Property

    Public Property RegolaScadenza_Tipo() As String
        Get
            Return _RegolaScadenza_Tipo
        End Get
        Set(ByVal value As String)
            _RegolaScadenza_Tipo = value
        End Set
    End Property

    Public Property RegolaScadenza_Qta() As String
        Get
            Return _RegolaScadenza_Qta
        End Get
        Set(ByVal value As String)
            _RegolaScadenza_Qta = value
        End Set
    End Property

    Public Property Stato() As String
        Get
            Return _Stato
        End Get
        Set(ByVal value As String)
            _Stato = value
        End Set
    End Property

    Public Property LineaPDZ() As String
        Get
            Return _LineaPDZ
        End Get
        Set(ByVal value As String)
            _LineaPDZ = value
        End Set
    End Property

End Class