Imports System.Data.Common
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports Newtonsoft.Json
Imports System.Data.OleDb
Imports System.Data.SqlClient
Imports System.Web
Imports System.Reflection
Imports Microsoft.VisualBasic.Logging

Public Interface IPrototype(Of T)

    Function CreateDeepCopy(ByVal source As T) As T

End Interface

Public Class AgronicaCoreParametri : Implements IPrototype(Of AgronicaCoreParametri)

    Private _PivaSuperUser As String
    Private _UsernameOperazione As String
    Private _UtenteUsername As String
    Private _UtenteCodFiscale As String
    Private _SuperUserUsername As String
    Private _FinestraTemporaleInizio As Date
    Private _FinestraTemporaleFine As Date
    Private _FlagVisibilita As enumVisibilita
    Private _FlagCancellazioneLogica As enumCancellazioneLogica
    Private _objConnessione As DbConnection
    Private _objTransazione As DbTransaction
    Private _StringaConnessione As String
    Private _LogDirectory As String
    Private _LogFileName As String
    Private _LogDescrizioneUtente As String
    Private _Lingua_Cod As Integer = 1
    Private _TimeoutQuery As Integer = 1200
    Private _Tipologia As agronicacoreparametri_tipologia

    Private _AppoggioFinestraTemporaleInizio As Date
    Private _AppoggioFinestraTemporaleFine As Date
    Private _AppoggioUsernameOperazione As String
    Private _LinkGiasBase As String = String.Empty


    '#######################################################################
    '#######################################################################
    '######  ENUMERAZIONI  #################################################
    '#######################################################################
    '#######################################################################
    ''' <summary>
    ''' Tipo Enumerativo per visibilità
    ''' </summary>
    ''' <remarks></remarks>
    Public Enum enumVisibilita
        visibilita_SoloNonInviati = 1
        Visibilita_SoloNonCancellati = 1
        Visibilita_SoloCancellati = 2
        Visibilita_Tutti = 3
    End Enum

    Public Enum enumCancellazioneLogica
        CancellazioneFisica = 0
        CancellazioneLogica = 1
    End Enum

    Public Enum enumSelezioneVariabile
        Selezione_TabellaDatiMinimi = 0     'Solo le informazioni essenziali della tabella
        Selezione_TabellaCompleta = 1       'Select Tabella.*       (tutta la tabella)
        Selezione_JoinDescrizioni = 2       'Informazioni principali con le info correlate
        Selezione_JoinCompleta = 4          'Select *               (con le tabelle correlate)
        Selezione_LogOmni = 5               'Select *               (con le tabelle correlate)

    End Enum

    ''' <summary>
    ''' Tipo Enumerativo per dati cartografici
    ''' </summary>
    ''' <remarks></remarks>
    Public Enum enumFromatoCartograficoConvertito
        Nessuna_Conversione = 0
        GML = 1
        WKT = 2
        GML_e_WKT = 3
    End Enum


    '#######################################################################
    '#######################################################################
    '######  PROPRIETA'  ###################################################
    '#######################################################################
    '#######################################################################

#Region "Proprieta"

    Public Property PivaSuperUser() As String
        Get
            Return _PivaSuperUser
        End Get
        Set(ByVal value As String)
            _PivaSuperUser = value
        End Set
    End Property

    Public Property UsernameOperazione() As String
        Get
            Return _UsernameOperazione
        End Get
        Set(ByVal value As String)
            _UsernameOperazione = value
        End Set
    End Property


    Public Property UtenteUsername() As String
        Get
            Return _UtenteUsername
        End Get
        Set(ByVal value As String)
            _UtenteUsername = value
        End Set
    End Property

    Public Property UtenteCodFiscale() As String
        Get
            Return _UtenteCodFiscale
        End Get
        Set(ByVal value As String)
            _UtenteCodFiscale = value
        End Set
    End Property

    Public Property SuperUserUsername() As String
        Get
            Return _SuperUserUsername
        End Get
        Set(ByVal value As String)
            _SuperUserUsername = value
        End Set
    End Property

    Public Property FinestraTemporaleInizio() As Date
        Get
            Return _FinestraTemporaleInizio
        End Get
        Set(ByVal value As Date)
            _FinestraTemporaleInizio = value
        End Set
    End Property


    Public Property FinestraTemporaleFine() As Date
        Get
            Return _FinestraTemporaleFine
        End Get
        Set(ByVal value As Date)
            _FinestraTemporaleFine = value
        End Set
    End Property


    Public Property FlagVisibilita() As enumVisibilita
        Get
            Return _FlagVisibilita
        End Get
        Set(ByVal value As enumVisibilita)
            _FlagVisibilita = value
        End Set
    End Property


    Public Property FlagCancellazioneLogica() As enumCancellazioneLogica
        Get
            Return _FlagCancellazioneLogica
        End Get
        Set(ByVal value As enumCancellazioneLogica)
            _FlagCancellazioneLogica = value
        End Set
    End Property

    <JsonIgnore>
    Public Property objConnessione() As DbConnection
        Get
            Return _objConnessione
        End Get
        Set(ByVal value As DbConnection)
            _objConnessione = value
        End Set
    End Property

    <JsonIgnore>
    Public Property objTransazione() As DbTransaction
        Get
            Return _objTransazione
        End Get
        Set(ByVal value As DbTransaction)
            _objTransazione = value
        End Set
    End Property

    <JsonProperty("StringaConnessione")>
    Private Property StringaConnessioneJson() As String
        Get
            Return _StringaConnessione
        End Get
        Set(ByVal value As String)
            _StringaConnessione = value
        End Set
    End Property

    <JsonIgnore>
    Public Property StringaConnessione() As String
        Get
            ' se attiva criptazione decodifica stringa di connessione
            If StringaConnessioneEncrypted() Then
                Return Sicurezza.GetStringaConnessione(_StringaConnessione)
            End If
            Return _StringaConnessione
        End Get
        Set(ByVal value As String)
            _StringaConnessione = value
        End Set
    End Property

    <JsonIgnore>
    Public ReadOnly Property StringaConnessioneEncrypted() As Boolean
        Get
            Return IsNumeric(_StringaConnessione)
        End Get
    End Property

    Public Property LogDirectory() As String
        Get
            Return _LogDirectory
        End Get
        Set(ByVal value As String)
            _LogDirectory = value
        End Set
    End Property


    Public Property LogFileName() As String
        Get
            Return _LogFileName
        End Get
        Set(ByVal value As String)
            _LogFileName = value
        End Set
    End Property


    Public Property LogDescrizioneUtente() As String
        Get
            Return _LogDescrizioneUtente
        End Get
        Set(ByVal value As String)
            _LogDescrizioneUtente = value
        End Set
    End Property

    Public Property Lingua_Cod() As Integer
        Get
            Return _Lingua_Cod
        End Get
        Set(value As Integer)
            _Lingua_Cod = value
        End Set
    End Property

    Public Property TimeoutQuery() As Integer
        Get
            Return _TimeoutQuery
        End Get
        Set(value As Integer)
            _TimeoutQuery = value
        End Set
    End Property

    Public Property Tipologia() As Integer
        Get
            Return _Tipologia
        End Get
        Set(value As Integer)
            _Tipologia = value
        End Set
    End Property

    Public Property LinkGiasBase() As String
        Get
            Return _LinkGiasBase
        End Get
        Set(value As String)
            _LinkGiasBase = value
        End Set
    End Property

#End Region


    '#######################################################################
    '#######################################################################
    '######  COSTRUTTORE  ##################################################
    '#######################################################################
    '#######################################################################
#Region "Costruttore"

    Public Sub New()

        _PivaSuperUser = ""
        _UsernameOperazione = ""
        _FinestraTemporaleInizio = #1/1/1900#
        _FinestraTemporaleFine = #12/31/2100#
        _FlagVisibilita = enumVisibilita.Visibilita_Tutti
        _FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneFisica
        _objConnessione = Nothing
        _objTransazione = Nothing
        _StringaConnessione = ""
        _LogDirectory = ""
        _LogFileName = ""
        _LogDescrizioneUtente = ""
        _Tipologia = agronicacoreparametri_tipologia.Standard

    End Sub

    Public Sub New(ByVal FinestraTemporaleInizio As Date,
                    ByVal FinestraTemporaleFine As Date,
                    ByVal FlagCancellazioneLogica As enumCancellazioneLogica,
                    ByVal FlagVisibilita As enumVisibilita,
                    ByVal LogDirectory As String,
                    ByVal LogFileName As String,
                    ByVal SuperUser_Username As String,
                    ByVal SuperUser_CodFiscale As String,
                    ByVal Utente_Username As String,
                    ByVal Utente_CodFiscale As String,
                    ByVal Stringa_Connessione As String
                    )

        _FinestraTemporaleInizio = FinestraTemporaleInizio
        _FinestraTemporaleFine = FinestraTemporaleFine
        _FlagCancellazioneLogica = FlagCancellazioneLogica
        _FlagVisibilita = FlagVisibilita
        _LogDirectory = LogDirectory
        _LogFileName = LogFileName
        _LogDescrizioneUtente = Utente_Username
        _SuperUserUsername = SuperUser_Username
        _PivaSuperUser = SuperUser_CodFiscale
        _UtenteUsername = Utente_Username
        _UtenteCodFiscale = Utente_CodFiscale
        _UsernameOperazione = Utente_CodFiscale
        _StringaConnessione = Stringa_Connessione
        _objConnessione = Nothing
        _objTransazione = Nothing

    End Sub

    ''' <summary>
    ''' Costruttore per AgronicaCoreParametri: prende in input un vecchio oggetto e ne crea uno nuovo
    ''' </summary>
    ''' <param name="Old">Vecchio oggetto AgronicaCoreParametri</param>
    ''' <remarks></remarks>
    Sub New(ByVal Old As AgronicaCoreParametri)
        _PivaSuperUser = Old.PivaSuperUser
        _UsernameOperazione = Old.UsernameOperazione
        _UtenteUsername = Old.UtenteUsername
        _UtenteCodFiscale = Old.UtenteCodFiscale
        _SuperUserUsername = Old.SuperUserUsername
        _FinestraTemporaleInizio = Old.FinestraTemporaleInizio
        _FinestraTemporaleFine = Old.FinestraTemporaleFine
        _FlagVisibilita = Old.FlagVisibilita
        _FlagCancellazioneLogica = Old.FlagCancellazioneLogica
        _objConnessione = Old.objConnessione
        _objTransazione = Old.objTransazione
        _StringaConnessione = Old.StringaConnessione
        _LogDirectory = Old.LogDirectory
        _LogFileName = Old.LogFileName
        _LogDescrizioneUtente = Old.LogDescrizioneUtente
        _Lingua_Cod = Old.Lingua_Cod
        _LinkGiasBase = Old._LinkGiasBase

    End Sub

#End Region

#Region "Funzioni"

    Public Sub ImpostaFinestre_con_SalvataggioTemporale(ByVal Inizio As Date,
                                    ByVal Fine As Date
                                    )
        '' salvo i dati nelle variabili temporali
        _AppoggioFinestraTemporaleInizio = _FinestraTemporaleInizio
        _AppoggioFinestraTemporaleFine = _FinestraTemporaleFine

        _FinestraTemporaleInizio = Inizio
        _FinestraTemporaleFine = Fine

    End Sub

    Public Sub ResettaFinestra()
        _FinestraTemporaleInizio = _AppoggioFinestraTemporaleInizio
        _FinestraTemporaleFine = _AppoggioFinestraTemporaleFine
    End Sub


    Public Sub ImpostaUsernameCreazione_con_Salvataggio(ByVal UsernameOperazione As String)
        '' salvo i dati nelle variabili di appoggio
        _AppoggioUsernameOperazione = UsernameOperazione

        _UsernameOperazione = UsernameOperazione

    End Sub

    Public Sub ResettaUsernameOperazione()
        _UsernameOperazione = _AppoggioUsernameOperazione
    End Sub

    'Questa funzione non va usata poiché viene ritornato l'oggetto della sessione e se si modificano delle proprietà, 
    'poi non c'è più modo di risalire ai valori iniziali
    'Public Shared Function objParametri_From_objSession(ByRef objSession As System.Web.SessionState.HttpSessionState, _
    '                                ByVal Flag_Server1_Utente2 As Integer _
    '                                ) As AgronicaCoreDataProvider.AgronicaCoreParametri

    '    If Flag_Server1_Utente2 = 1 Then
    '        Return objSession("ASG_objParametri_Server")
    '    Else
    '        Return objSession("ASG_objParametri_Utenti")
    '    End If
    'End Function

    Public Sub VerificaProprietaObjParametri(ByVal ObjParametri As AgronicaCoreParametri,
                                            ByVal Server0_Utenti1 As Integer)

        Dim log As String = ""

        If ObjParametri.LogDescrizioneUtente = "" Then
            log += "LogDescrizioneUtente non valorizzato." & vbCrLf
        End If

        If ObjParametri.LogDirectory = "" Then
            log += "LogDirectory non valorizzato." & vbCrLf
        End If

        If ObjParametri.LogFileName = "" Then
            log += "LogFileName non valorizzato." & vbCrLf
        End If

        If ObjParametri.PivaSuperUser = "" Then
            log += "PivaSuperUser non valorizzato." & vbCrLf
        End If

        If ObjParametri.StringaConnessione = "" Then
            log += "StringaConnessione non valorizzato." & vbCrLf
        End If

        If ObjParametri.SuperUserUsername = "" Then
            log += "SuperUserUsername non valorizzato." & vbCrLf
        End If

        If ObjParametri.UsernameOperazione = "" Then
            log += "UsernameOperazione non valorizzato." & vbCrLf
        End If

        If ObjParametri.UtenteCodFiscale = "" Then
            log += "UtenteCodFiscale non valorizzato." & vbCrLf
        End If

        If ObjParametri.UtenteUsername = "" Then
            log += "UtenteUsername non valorizzato." & vbCrLf
        End If

        If IsNothing(ObjParametri.FinestraTemporaleInizio) Then
            log += "FinestraTemporaleInizio non valorizzato." & vbCrLf
        End If

        If IsNothing(ObjParametri.FinestraTemporaleFine) Then
            log += "FinestraTemporaleFine non valorizzato." & vbCrLf
        End If

        If IsNothing(ObjParametri.FlagCancellazioneLogica) Then
            log += "FlagCancellazioneLogica non valorizzato." & vbCrLf
        End If

        If log <> "" Then
            'se alcune proprietà non sono valorizzate, 
            'ci sono dei problemi (qualcosa non viene letto/valorizzato) nella pagina GestioneRichieste... auguri!
            If Server0_Utenti1 = 0 Then
                Throw New Exception("Errori sull'objParametri_Server: " & log)
            Else
                Throw New Exception("Errori sull'objParametri_Utenti: " & log)
            End If
        End If

    End Sub

    Public Function Recupera_NomeDB() As String

        Dim NomeDB As String = ""

        Try

            Dim Hash_Parametri As New Hashtable
            For Each par In StringaConnessione.Split(New Char() {";"c}, StringSplitOptions.RemoveEmptyEntries)
                Hash_Parametri.Add(par.Split({"="c})(0), par.Split({"="c})(1))
            Next

            NomeDB = CStr(Hash_Parametri.Item("Initial Catalog"))

        Catch ex As Exception
            NomeDB = ""
        End Try

        Return NomeDB

    End Function

    Public Function CreateDeepCopy(source As AgronicaCoreParametri) As AgronicaCoreParametri Implements IPrototype(Of AgronicaCoreParametri).CreateDeepCopy

        Dim copy = JsonConvert.SerializeObject(source)
        Return JsonConvert.DeserializeObject(Of AgronicaCoreParametri)(copy)

    End Function

#End Region


End Class


Public Class AgronicaCoreParametri_Helper

    '#################################################################################################
    Public Function Crea_ObjParametri(ByVal FinestraTemporaleInizio As Date,
                                        ByVal FinestraTemporaleFine As Date,
                                        ByVal FlagCancellazioneLogica As AgronicaCoreParametri.enumCancellazioneLogica,
                                        ByVal FlagVisibilita As AgronicaCoreParametri.enumVisibilita,
                                        ByVal LogDirectory As String,
                                        ByVal LogFileName As String,
                                        ByVal SuperUser_Username As String,
                                        ByVal SuperUser_CodFiscale As String,
                                        ByVal Utente_Username As String,
                                        ByVal Utente_CodFiscale As String,
                                        ByVal Stringa_Connessione As String,
                                        Optional ByVal LinkGiasBase As String = Nothing,
                                        Optional ByVal Lingua_Cod As Integer = enum_AgroLingue.Italiano_it
                                        ) As AgronicaCoreParametri


        Dim objParametri As New AgronicaCoreParametri

        objParametri.FinestraTemporaleInizio = FinestraTemporaleInizio
        objParametri.FinestraTemporaleFine = FinestraTemporaleFine

        objParametri.FlagCancellazioneLogica = FlagCancellazioneLogica
        objParametri.FlagVisibilita = FlagVisibilita

        objParametri.LogDirectory = LogDirectory
        objParametri.LogFileName = LogFileName

        objParametri.LogDescrizioneUtente = Utente_Username

        'partita iva del superuser
        objParametri.PivaSuperUser = SuperUser_CodFiscale
        'username del superuser
        objParametri.SuperUserUsername = SuperUser_Username

        'codice fiscale dell'utente: serve per le scritture sul db
        'il codice fiscale dell'utente va salvato in username_creazione e username_modifica
        objParametri.UsernameOperazione = Utente_CodFiscale

        'username dell'utente
        objParametri.UtenteUsername = Utente_Username
        'codice fiscale dell'utente
        objParametri.UtenteCodFiscale = Utente_CodFiscale

        'al db server o utenti
        objParametri.StringaConnessione = Stringa_Connessione

        objParametri.objConnessione = Nothing
        objParametri.objTransazione = Nothing

        'If Not IsNothing(LinkGiasBase) AndAlso Not String.IsNullOrEmpty(LinkGiasBase) AndAlso Not String.IsNullOrEmpty(LinkGiasBase) Then
        '    objParametri.LinkGiasBase = LinkGiasBase
        'Else
        '    objParametri.LinkGiasBase = Leggi_Link_Gias_Base(objParametri.StringaConnessione)
        'End If
        If LinkGiasBase IsNot Nothing Then
            objParametri.LinkGiasBase = LinkGiasBase
        End If

        objParametri.Lingua_Cod = Lingua_Cod

        Return objParametri

    End Function



    '#################################################################################################
    Public Sub Modifica_OBJP_con_Configurazione_Siti(ByVal DT_Configurazione_Siti As DataTable, _
                                        ByRef objServer As AgronicaCoreParametri, _
                                        ByRef objUtenti As AgronicaCoreParametri)

        'setto le ultime cose in objParametri

        'cerco logfilename
        Dim dr() As DataRow
        dr = DT_Configurazione_Siti.Select("chiave = 'AgronicaCore_FileNameLOG'")
        If dr.Length = 1 Then
            objServer.LogFileName = CStr(dr(0).Item("Valore"))
            objUtenti.LogFileName = CStr(dr(0).Item("Valore"))
        End If


        dr = DT_Configurazione_Siti.Select("chiave = 'AgronicaCore_DirectoryLOG'")
        If dr.Length = 1 Then
            objServer.LogDirectory = CStr(dr(0).Item("Valore"))
            objUtenti.LogDirectory = CStr(dr(0).Item("Valore"))
        End If


        dr = DT_Configurazione_Siti.Select("chiave = 'AgronicaCore_Flag_CancellazioneLogica'")
        If dr.Length = 1 Then
            objServer.FlagCancellazioneLogica = dr(0).Item("Valore")
            objUtenti.FlagCancellazioneLogica = dr(0).Item("Valore")
        End If


        dr = DT_Configurazione_Siti.Select("chiave = 'AgronicaCore_Flag_Visibilita'")
        If dr.Length = 1 Then
            objServer.FlagVisibilita = dr(0).Item("Valore")
            objUtenti.FlagVisibilita = dr(0).Item("Valore")
        End If


    End Sub

End Class



Public Structure AgronicaCoreParametriFiltroIngressoSitoOnline

    Dim ID_DB As Integer
    Dim TipoDB As Integer
    Dim Server As String
    Dim DB As String
    Dim Provider As String
    Dim UserId As String
    Dim Password As String
    Dim PivaSuperUser As String
    Dim Note As String
    Dim Progressivo As Integer
    Dim Descrizione As String
    Dim Filtro_Temp_Inizio As Date
    Dim Filtro_Temp_Fine As Date




End Structure


Public Class ObjParams
    Public ObjParametri_SuperServer As AgronicaCoreParametri
    Public ObjParametri_Server As AgronicaCoreParametri
    Public ObjParametri_Utenti As AgronicaCoreParametri
End Class

