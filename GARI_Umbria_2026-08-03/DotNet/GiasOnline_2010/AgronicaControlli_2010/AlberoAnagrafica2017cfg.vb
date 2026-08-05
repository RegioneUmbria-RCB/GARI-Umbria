


'spostato in AgronicaCoreModelloInSviluppo..!


'Imports AgronicaCoreModelloInSviluppo

'Public Class AlberoAnagrafica2017cfg_old

'    Private _Flag_Esplodi_Tutto As Boolean = False
'    Private _Flag_CheckBox As Boolean = False

'    Private _Flag_Planning As Boolean = False

'    Private _Flag_Anagrafica As Boolean = False

'    Private _Flag_Contatti As Boolean = False
'    Private _Flag_Analisi As Boolean = False
'    Private _Flag_PianoConcimazione As Boolean = False
'    Private _Flag_Esercizio As Boolean = False
'    Private _Flag_ParcoMacchine As Boolean = False
'    Private _Flag_CatastoAziendale As Boolean = False
'    Private _Flag_Fabbricati As Boolean = False
'    Private _Flag_PortafoglioProdotti As Boolean = False
'    Private _Flag_Singola_Selezione As Boolean = True

'    Private _Flag_Appezzamenti_Filtra_Tecnico As Boolean = True

'    Private _Flag_Agenda As Boolean = False

'    Private _FiltroImpiantiIdTestataTemp As Integer = 0


'    Private _ordinaDataUltimoImpianto As Boolean = False
'    Private _visualizzaRiferimentoAlfanumericoImpianto As Boolean = False

'    Private _TipoOperazioneColturale As String

'    Private _Piva As String
'    Private _Sa_Cod As String
'    Private _Veg_Cod As Integer = 0
'    Private _Cul_Cod As Integer

'    Private _Flag_Carica_Primo_Giro As Boolean

'    Private _dataInizio As DateTime
'    Private _dataFine As DateTime

'    Private _CheckBoxes As New CheckBoxFlags

'    Private _DatiSportelloSementieri As String

'    Private _ParametriAgendaData As DateTime

'    Private _Elenco_Icone_SpecieVegetali As String


'#Region "Property"




'    Public Property Flag_Anagrafica() As Boolean
'        Get
'            Return _Flag_Anagrafica
'        End Get
'        Set(value As Boolean)
'            _Flag_Anagrafica = value
'        End Set
'    End Property


'    Public Property Flag_Carica_Primo_Giro() As Boolean
'        Get
'            Return _Flag_Carica_Primo_Giro
'        End Get
'        Set(value As Boolean)
'            _Flag_Carica_Primo_Giro = value
'        End Set
'    End Property

'    Public Property visualizzaRiferimentoAlfanumericoImpianto() As Boolean
'        Get
'            Return _visualizzaRiferimentoAlfanumericoImpianto
'        End Get
'        Set(value As Boolean)
'            _visualizzaRiferimentoAlfanumericoImpianto = value
'        End Set
'    End Property

'    Public Property ordinaDataUltimoImpianto() As Boolean
'        Get
'            Return _ordinaDataUltimoImpianto
'        End Get
'        Set(value As Boolean)
'            _ordinaDataUltimoImpianto = value
'        End Set
'    End Property


'    Public Property Flag_Agenda() As Boolean
'        Get
'            Return _Flag_Agenda
'        End Get
'        Set(value As Boolean)
'            _Flag_Agenda = value
'        End Set
'    End Property

'    Private _Flag_Ricette As Boolean = False
'    Public Property Flag_Ricette() As Boolean
'        Get
'            Return _Flag_Ricette
'        End Get
'        Set(value As Boolean)
'            _Flag_Ricette = value
'        End Set
'    End Property


'    Public Property Flag_Esplodi_Tutto() As Boolean
'        Get
'            Return _Flag_Esplodi_Tutto
'        End Get
'        Set(ByVal value As Boolean)
'            _Flag_Esplodi_Tutto = value
'        End Set
'    End Property

'    Public Property Flag_Planning() As Boolean
'        Get
'            Return _Flag_Planning
'        End Get
'        Set(ByVal value As Boolean)
'            _Flag_Planning = value
'        End Set
'    End Property

'    Public Property Flag_Singola_Selezione() As Boolean
'        Get
'            Return _Flag_Singola_Selezione
'        End Get
'        Set(ByVal value As Boolean)
'            _Flag_Singola_Selezione = value
'        End Set
'    End Property

'    Public Property Flag_CheckBox() As Boolean
'        Get
'            Return _Flag_CheckBox
'        End Get
'        Set(ByVal value As Boolean)
'            _Flag_CheckBox = value
'        End Set
'    End Property
'    Public Property Flag_Contatti() As Boolean
'        Get
'            Return _Flag_Contatti
'        End Get
'        Set(ByVal value As Boolean)
'            _Flag_Contatti = value
'        End Set
'    End Property

'    Public Property Flag_Analisi() As Boolean
'        Get
'            Return _Flag_Analisi
'        End Get
'        Set(ByVal value As Boolean)
'            _Flag_Analisi = value
'        End Set
'    End Property

'    Public Property Flag_PianoConcimazione() As Boolean
'        Get
'            Return _Flag_PianoConcimazione
'        End Get
'        Set(ByVal value As Boolean)
'            _Flag_PianoConcimazione = value
'        End Set
'    End Property

'    Public Property Flag_Esercizio() As Boolean
'        Get
'            Return _Flag_Esercizio
'        End Get
'        Set(ByVal value As Boolean)
'            _Flag_Esercizio = value
'        End Set
'    End Property

'    Public Property Flag_Appezzamenti_Filtra_Tecnico() As Boolean
'        Get
'            Return _Flag_Appezzamenti_Filtra_Tecnico
'        End Get
'        Set(ByVal value As Boolean)
'            _Flag_Appezzamenti_Filtra_Tecnico = value
'        End Set
'    End Property

'    Public Property Cul_Cod() As Integer
'        Get
'            Return _Cul_Cod
'        End Get
'        Set(ByVal value As Integer)
'            _Cul_Cod = value
'        End Set
'    End Property
'    Public Property Veg_Cod() As Integer
'        Get
'            Return _Veg_Cod
'        End Get
'        Set(ByVal value As Integer)
'            _Veg_Cod = value
'        End Set
'    End Property

'    Public Property Flag_PortafoglioProdotti() As Boolean
'        Get
'            Return _Flag_PortafoglioProdotti
'        End Get
'        Set(ByVal value As Boolean)
'            _Flag_PortafoglioProdotti = value
'        End Set
'    End Property

'    Private _PivaPadre As String = ""


'    Public Property PivaPadre() As String
'        Get
'            Return _PivaPadre
'        End Get
'        Set(value As String)
'            _PivaPadre = value
'        End Set
'    End Property

'    Public Property Piva() As String
'        Get
'            Return _Piva
'        End Get
'        Set(ByVal value As String)
'            _Piva = value
'        End Set
'    End Property


'    Public Property Sa_Cod() As String
'        Get
'            Return _Sa_Cod
'        End Get
'        Set(ByVal value As String)
'            If (value = "") Then
'                _Sa_Cod = 0
'            Else
'                _Sa_Cod = value
'            End If
'        End Set
'    End Property


'    Public Property Flag_ParcoMacchine() As Boolean
'        Get
'            Return _Flag_ParcoMacchine
'        End Get
'        Set(ByVal value As Boolean)
'            _Flag_ParcoMacchine = value
'        End Set
'    End Property

'    Public Property Flag_CatastoAziendale() As Boolean
'        Get
'            Return _Flag_CatastoAziendale
'        End Get
'        Set(ByVal value As Boolean)
'            _Flag_CatastoAziendale = value
'        End Set
'    End Property

'    Private _Valore_Albero As String
'    Public Property Valore_Albero() As String
'        Get
'            Return _Valore_Albero
'        End Get
'        Set(ByVal value As String)
'            _Valore_Albero = value
'        End Set
'    End Property


'    Public Property Flag_Fabbricati() As Boolean
'        Get
'            Return _Flag_Fabbricati
'        End Get
'        Set(ByVal value As Boolean)
'            _Flag_Fabbricati = value
'        End Set
'    End Property

'    Public Property CheckBoxes() As CheckBoxFlags
'        Get
'            Return _CheckBoxes
'        End Get
'        Set(ByVal value As CheckBoxFlags)
'            _CheckBoxes = value
'        End Set
'    End Property

'    Private _FlagModalitaSementieri As Boolean = False
'    Public Property FlagModalitaSementieri() As Boolean
'        Get
'            Return _FlagModalitaSementieri
'        End Get
'        Set(value As Boolean)
'            _FlagModalitaSementieri = value
'        End Set
'    End Property


'    Public Property TipoOperazioneColturale As String
'        Get
'            Return _TipoOperazioneColturale
'        End Get
'        Set(ByVal value As String)
'            _TipoOperazioneColturale = value
'        End Set
'    End Property
'    Private _GruppoOperazioneColturale As String
'    Public Property GruppoOperazioneColturale As String
'        Get
'            Return _GruppoOperazioneColturale
'        End Get
'        Set(ByVal value As String)
'            _GruppoOperazioneColturale = value
'        End Set
'    End Property

'    Public Property DataInizio As Date
'        Get
'            Return _dataInizio
'        End Get
'        Set(value As Date)
'            _dataInizio = value
'        End Set
'    End Property

'    Public Property DataFine As Date
'        Get
'            Return _dataFine
'        End Get
'        Set(value As Date)
'            _dataFine = value
'        End Set
'    End Property

'    Public Property DatiSportelloSementieri As String
'        Get
'            Return _DatiSportelloSementieri
'        End Get
'        Set(value As String)
'            _DatiSportelloSementieri = value
'        End Set
'    End Property

'    Public Property ParametriAgendaData As Date
'        Get
'            Return _ParametriAgendaData
'        End Get
'        Set(value As Date)
'            _ParametriAgendaData = value
'        End Set
'    End Property

'    Public Property Elenco_Icone_SpecieVegetali As String
'        Get
'            Return _Elenco_Icone_SpecieVegetali
'        End Get
'        Set(value As String)
'            _Elenco_Icone_SpecieVegetali = value
'        End Set
'    End Property

'    Public Property FiltroImpiantiIdTestataTemp As Integer
'        Get
'            Return _FiltroImpiantiIdTestataTemp
'        End Get
'        Set(value As Integer)
'            _FiltroImpiantiIdTestataTemp = value
'        End Set
'    End Property


'#End Region


'End Class
