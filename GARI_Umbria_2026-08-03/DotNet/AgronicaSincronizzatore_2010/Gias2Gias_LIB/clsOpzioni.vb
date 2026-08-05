Public Class clsOpzioni
    Dim _wsimportaGiasURl As String

    'Dim _Piva_Filtro As String

    '#######################################################################
    '#######################################################################
    '######  PROPRIETA'  ###################################################
    '#######################################################################
    '#######################################################################

#Region "Proprieta"

    Public ReadOnly Property isGias2Gias_local As Boolean
        Get
            Return (_wsimportaGiasURl = "")
        End Get
    End Property

    Public Property wsimportaGiasURl As String
        Get
            Return _wsimportaGiasURl
        End Get
        Set(value As String)
            _wsimportaGiasURl = value
        End Set
    End Property
    Public Property PercorsoConnessioni() As String


    Private _FormatoOraZero As String
    Public Property FormatoOraZero() As String
        Get
            Return _FormatoOraZero
        End Get
        Set(value As String)
            _FormatoOraZero = value
        End Set
    End Property
    Public Property Id_Servizio() As Integer

    Public Property PathDirFileLog() As String

    Public Property PathFileXMLOpzioniImport() As String

    Public Property Id_Codice_Imprese() As Integer
        Get
            Return Id_Servizio
        End Get
        Set(ByVal value As Integer)
            Id_Servizio = value
        End Set
    End Property

    '===========================================================================

    Public Property ImpostazioniTrasformazioni As String

    Public Property Connessione_Server_GIAS_Origine() As String

    Public Property Connessione_Utenti_GIAS_Origine() As String

    Public Property Stringa_Connessione_Server_GIAS_ORIGINE() As String

    Public Property Stringa_Connessione_Utenti_GIAS_ORIGINE() As String

    Public Property objParametri_Server_GIAS_ORIGINE() As AgronicaCoreDataProvider.AgronicaCoreParametri

    Public Property objParametri_Utenti_GIAS_ORIGINE() As AgronicaCoreDataProvider.AgronicaCoreParametri

    Public Property SuperUser_Username_ORIGINE() As String

    Public Property SuperUser_CodFiscale_ORIGINE() As String

    Public Property Import_Username_ORIGINE() As String

    Public Property Import_Password_ORIGINE() As String

    Public Property Import_CodFiscale_ORIGINE() As String

    Public Property ProgressivoGIAS_ORIGINE() As Integer


    Public Property BaseCode_ORIGINE() As Integer

    Public Property TopCode_ORIGINE() As Integer

    Public Property CodiceImpresa_ORIGINE() As Integer

    '================================================================


    Public Property Connessione_Server_GIAS_Destinazione() As String

    Public Property Connessione_Utenti_GIAS_Destinazione() As String

    Public Property Stringa_Connessione_Server_GIAS_DESTINAZIONE() As String

    Public Property Stringa_Connessione_Utenti_GIAS_DESTINAZIONE() As String

    Public Property objParametri_Server_GIAS_DESTINAZIONE() As AgronicaCoreDataProvider.AgronicaCoreParametri

    Public Property objParametri_Utenti_GIAS_DESTINAZIONE() As AgronicaCoreDataProvider.AgronicaCoreParametri

    Public Property SuperUser_Username_DESTINAZIONE() As String

    Public Property SuperUser_CodFiscale_DESTINAZIONE() As String

    Public Property Import_Username_DESTINAZIONE() As String

    Public Property Import_Password_DESTINAZIONE() As String

    Public Property Import_CodFiscale_DESTINAZIONE() As String

    Public Property ProgressivoGIAS_DESTINAZIONE() As Integer


    Public Property BaseCode_DESTINAZIONE() As Integer

    Public Property TopCode_DESTINAZIONE() As Integer

    Public Property CodiceImpresa_DESTINAZIONE() As Integer

    Public Property TimeOut_Chiamata_WS() As Integer
    '========================================================



    'Public Property Piva_Filtro() As String
    '    Get
    '        Return _Piva_Filtro
    '    End Get
    '    Set(ByVal value As String)
    '        _Piva_Filtro = value
    '    End Set
    'End Property






#End Region



    ''########################################################################################
    'Public Sub New(ByVal x_Host_Smtp As String, _
    '                ByVal x_Mittente As String, _
    '                ByVal x_DestinatarioLog As String, _
    '                ByVal x_DestinatarioBancheDati As String, _
    '                ByVal x_WS_Importa_Magazzino As String, _
    '                ByVal x_IstanzaSQL As String, _
    '                ByVal x_Nome_Database As String, _
    '                ByVal x_Username_Database As String, _
    '                ByVal x_Password_Database As String, _
    '                ByVal x_PathDirFileLog As String, _
    '                ByVal x_PathDirFileXmlPubblico As String, _
    '                ByVal x_Connessione_Server As String, _
    '                ByVal x_Connessione_Utenti As String, _
    '                ByVal x_PercorsoConnessioni As String, _
    '                ByVal x_Id_Servizio As Integer, _
    '                ByVal x_SuperUser_Username As String, _
    '                ByVal x_SuperUser_CodFiscale As String, _
    '                ByVal x_Progressivo_Gias As Integer, _
    '                ByVal x_Import_Username As String, _
    '                ByVal x_Import_Password As String, _
    '                ByVal x_Import_CodFiscale As String, _
    '                ByVal x_Piva_Filtro As String, _
    '                ByVal x_Data_Inizio_Filtro As String, _
    '                ByVal x_Data_Fine_Filtro As String, _
    '                ByVal x_Filtro_Aggiuntivo_ElencoDDT As String, _
    '                ByVal x_Filtro_Aggiuntivo_DatiDDT As String, _
    '                ByVal x_TabelleTemp_Mode As Integer, _
    '                ByVal x_Str_TabelleTemp_RegolaConfronto As String, _
    '                ByVal x_FlagBloccoOperazione As Boolean, _
    '                ByVal x_Codifiche_IstanzaSQL As String, _
    '                ByVal x_Codifiche_Nome_Database As String, _
    '                ByVal x_Codifiche_Username_Database As String, _
    '                ByVal x_Codifiche_Password_Database As String _
    '                )

    '    _Host_Smtp = x_Host_Smtp
    '    _Mittente = x_Mittente
    '    _DestinatarioLog = x_DestinatarioLog
    '    _DestinatarioBancheDati = x_DestinatarioBancheDati

    '    _WS_Importa_Magazzino = x_WS_Importa_Magazzino
    '    _IstanzaSQL = x_IstanzaSQL
    '    _Nome_Database = x_Nome_Database
    '    _Username_Database = x_Username_Database
    '    _Password_Database = x_Password_Database

    '    _PathDirFileLog = x_PathDirFileLog
    '    _PathDirFileXmlPubblico = x_PathDirFileXmlPubblico

    '    _Connessione_Server = x_Connessione_Server
    '    _Connessione_Utenti = x_Connessione_Utenti
    '    _PercorsoConnessioni = x_PercorsoConnessioni
    '    _Id_Servizio = x_Id_Servizio

    '    _SuperUser_Username = x_SuperUser_Username
    '    _SuperUser_CodFiscale = x_SuperUser_CodFiscale
    '    _ProgressivoGIAS = x_Progressivo_Gias

    '    _Import_Username = x_Import_Username
    '    _Import_Password = x_Import_Password
    '    _Import_CodFiscale = x_Import_CodFiscale

    '    _Piva_Filtro = x_Piva_Filtro
    '    _Data_Inizio_Filtro = x_Data_Inizio_Filtro
    '    _Data_Fine_Filtro = x_Data_Fine_Filtro
    '    _Filtro_Aggiuntivo_ElencoDDT = x_Filtro_Aggiuntivo_ElencoDDT
    '    _Filtro_Aggiuntivo_DatiDDT = x_Filtro_Aggiuntivo_DatiDDT

    '    _TabelleTemp_Mode = x_TabelleTemp_Mode
    '    _Str_TabelleTemp_RegolaConfronto = x_Str_TabelleTemp_RegolaConfronto

    '    _FlagBloccoOperazione = x_FlagBloccoOperazione
    '    _Codifiche_IstanzaSQL = x_Codifiche_IstanzaSQL
    '    _Codifiche_Nome_Database = x_Codifiche_Nome_Database
    '    _Codifiche_Username_Database = x_Codifiche_Username_Database
    '    _Codifiche_Password_Database = x_Codifiche_Password_Database

    'End Sub


End Class



