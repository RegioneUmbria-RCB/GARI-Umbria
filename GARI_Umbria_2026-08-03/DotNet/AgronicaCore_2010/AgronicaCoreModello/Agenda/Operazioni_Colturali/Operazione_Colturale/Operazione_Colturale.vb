Imports AgronicaCoreDataProvider
Imports AgronicaCoreModello.OperazioneAgenda_Temp

Namespace Agenda.Operazioni_Colturali.Operazione_Colturale

    '--------------------
    '------NB----------
    '--------------------
    'per ora non è gestito il recupero dei centri e il multicentro nell'oggetto operazione,
    'da fare, è importante per usare l'oggetto per le operazioni multicentro,
    'Occorre gestire l'utilizzo di Sa-Cod dell'oggetto in maniera ottimizzata
    '--------------------
    '--------------------
    '--------------------
    Public Class Operazione_Colturale
        Implements I_Operazione_Colturale

        Protected _DataOperazione As Date

        Protected _ID_Agenda As Integer

        Protected _Tipo_Operazione As TipiEnumerativi.enum_TipoOperazioneDB

        Protected _Piva As String
        Protected _RagioneSociale As String 'autoasssegnata

        Protected _Sa_Cod As Integer
        Protected _ID_Reg As Integer

        Protected _Lav_Cod As Integer
        Protected _Lav_Des As String 'autoasssegnata

        Protected _Cau_Mov As Integer
        Protected _Cau_Des As String 'autoasssegnata

        Protected _Magazzino As Anagrafe.Magazzino

        Protected _OperazioneMulticentro As Boolean

        Protected _Nota As String

        Protected _Note_Codificate As List(Of Integer)

        'da modellare
        Protected _Costi_Accessori_Temporaneo As List(Of Movimento)

        Public Sub New(ByVal Partita_Iva_Azienda As String, ByVal Data_Operazione As Date, Tipo_OperazioneDb As TipiEnumerativi.enum_TipoOperazioneDB, ByVal Id_Agenda_In As Integer, ByRef objParametri_Server As AgronicaCoreParametri)
            Inizializza()

            _DataOperazione = Data_Operazione
            _ID_Agenda = Id_Agenda_In
            _Tipo_Operazione = Tipo_OperazioneDb
            _Piva = Partita_Iva_Azienda
            If IsNothing(objParametri_Server) Then
                _RagioneSociale = ""
            Else
                Dim objImprese As New AgronicaCoreAnagrafeDAL.Imprese_Read
                _RagioneSociale = objImprese.RagSoc_from_Piva(Partita_Iva_Azienda, objParametri_Server)
            End If

        End Sub

        Public Sub New(ByRef OperazioneColturaleGenerica As I_Operazione_Colturale, ByRef objParametri_Server As AgronicaCoreParametri)
            _DataOperazione = OperazioneColturaleGenerica.DataOperazione
            _ID_Agenda = OperazioneColturaleGenerica.ID_Agenda
            _Tipo_Operazione = OperazioneColturaleGenerica.Tipo_Operazione
            _Piva = OperazioneColturaleGenerica.Piva
            _RagioneSociale = OperazioneColturaleGenerica.RagioneSociale
            _Sa_Cod = OperazioneColturaleGenerica.Sa_Cod
            _ID_Reg = OperazioneColturaleGenerica.ID_Reg
            _Lav_Cod = OperazioneColturaleGenerica.Lav_Cod
            _Lav_Des = OperazioneColturaleGenerica.Lav_Des
            _Cau_Mov = OperazioneColturaleGenerica.Cau_Mov
            _Cau_Des = OperazioneColturaleGenerica.Cau_Des
            _Magazzino = OperazioneColturaleGenerica.Magazzino
            _OperazioneMulticentro = OperazioneColturaleGenerica.OperazioneMulticentro
            _Nota = OperazioneColturaleGenerica.Nota
            _Note_Codificate = OperazioneColturaleGenerica.Note_Codificate
            _Costi_Accessori_Temporaneo = OperazioneColturaleGenerica.Costi_Accessori_Temporaneo
        End Sub

        Private Sub Inizializza()
            _DataOperazione = System.DateTime.Now
            _ID_Agenda = 0
            _Tipo_Operazione = TipiEnumerativi.enum_TipoOperazioneDB.Scrittura
            _Piva = ""
            _RagioneSociale = ""
            _Sa_Cod = 0
            _ID_Reg = 0
            _Magazzino = Nothing
            _Lav_Cod = 0
            _Lav_Des = ""
            _Cau_Mov = 0
            _Cau_Des = ""
            _OperazioneMulticentro = False
            _Nota = ""
            _Note_Codificate = New List(Of Integer)
            _Costi_Accessori_Temporaneo = New List(Of Movimento)
        End Sub

        Protected Sub ImpostaCauMov(_Cau_Mov_In As TipiEnumerativi.enum_Agenda_Causali)

            _Cau_Mov = _Cau_Mov_In

            Select Case _Cau_Mov_In
                Case TipiEnumerativi.enum_Agenda_Causali.TRATTAMENTO
                    _Cau_Des = "Trattamenti"
                Case TipiEnumerativi.enum_Agenda_Causali.RILIEVO_CAMPO
                    _Cau_Des = "Rilievi in Campo"
                Case TipiEnumerativi.enum_Agenda_Causali.RILIEVO_RACCOLTA
                    _Cau_Des = "Rilievi alla Raccolta"
                Case TipiEnumerativi.enum_Agenda_Causali.LAVORAZIONE
                    _Cau_Des = "Lavorazioni"
                Case TipiEnumerativi.enum_Agenda_Causali.COSTI_ACCESSORI
                    _Cau_Des = "Costi Accessori"
                Case Else
                    _Cau_Des = "Non Implementato 'class operazione colturale'"
            End Select

            'creare tabella!
            'If Not IsNothing(ObjParametri_Server) Then
            '    Dim objOperazioni As New AgronicaCoreAnagrafeDAL.Operazioni_R
            '    _Cau_Des = objOperazioni.Lav_Des_From_Lav_Cod(_Lav_Cod, ObjParametri_Server)
            'End If
        End Sub

#Region "Metodi Implementati"

        Public Sub CancellaDati() Implements I_Operazione_Colturale.CancellaDati

        End Sub

        Public Sub RipristinaDatiIniziali() Implements I_Operazione_Colturale.RipristinaDatiIniziali

        End Sub

#End Region


#Region "Proprietà Implementate"

        Public ReadOnly Property Cau_Des As String Implements I_Operazione_Colturale.Cau_Des
            Get
                Return _Cau_Des
            End Get
        End Property

        Public ReadOnly Property Cau_Mov As Integer Implements I_Operazione_Colturale.Cau_Mov
            Get
                Return _Cau_Mov
            End Get
        End Property

        Public Property DataOperazione As Date Implements I_Operazione_Colturale.DataOperazione
            Get
                Return _DataOperazione
            End Get
            Set(value As Date)
                _DataOperazione = value
            End Set
        End Property

        Public Property ID_Reg As Integer Implements I_Operazione_Colturale.ID_Reg
            Get
                Return _ID_Reg
            End Get
            Set(value As Integer)
                _ID_Reg = value
            End Set
        End Property

        Public ReadOnly Property Lav_Cod As Integer Implements I_Operazione_Colturale.Lav_Cod
            Get
                Return _Lav_Cod
            End Get
        End Property

        Public ReadOnly Property Lav_Des As String Implements I_Operazione_Colturale.Lav_Des
            Get
                Return _Lav_Des
            End Get
        End Property

        Public Property Magazzino As Anagrafe.Magazzino Implements I_Operazione_Colturale.Magazzino
            Get
                Return _Magazzino
            End Get
            Set(value As Anagrafe.Magazzino)
                _Magazzino = value
            End Set
        End Property

        Public ReadOnly Property OperazioneMulticentro As Boolean Implements I_Operazione_Colturale.OperazioneMulticentro
            Get
                Return _OperazioneMulticentro
            End Get
            'Set(value As Boolean)
            '    _OperazioneMulticentro = value
            'End Set
        End Property

        Public Property Tipo_Operazione As TipiEnumerativi.enum_TipoOperazioneDB Implements I_Operazione_Colturale.Tipo_Operazione
            Get
                Return _Tipo_Operazione
            End Get
            Set(value As TipiEnumerativi.enum_TipoOperazioneDB)
                _Tipo_Operazione = value
            End Set
        End Property

        Public ReadOnly Property Piva As String Implements I_Operazione_Colturale.Piva
            Get
                Return _Piva
            End Get
        End Property

        Public ReadOnly Property RagioneSociale As String Implements I_Operazione_Colturale.RagioneSociale
            Get
                Return _RagioneSociale
            End Get
        End Property

        Public Property Sa_Cod As Integer Implements I_Operazione_Colturale.Sa_Cod
            Get
                Return _Sa_Cod
            End Get
            Set(value As Integer)
                _Sa_Cod = value
            End Set
        End Property

        Public Property ID_Agenda As Integer Implements I_Operazione_Colturale.ID_Agenda
            Get
                Return _ID_Agenda
            End Get
            Set(value As Integer)
                _ID_Agenda = value
            End Set
        End Property


        Public Property Nota As String Implements I_Operazione_Colturale.Nota
            Get
                Return _Nota
            End Get
            Set(value As String)
                _Nota = value
            End Set
        End Property

        Public Property Note_Codificate() As List(Of Integer) Implements I_Operazione_Colturale.Note_Codificate
            Get
                Return _Note_Codificate
            End Get
            Set(value As List(Of Integer))
                _Note_Codificate = value
            End Set
        End Property

        Public Property Costi_Accessori_Temporaneo() As List(Of Movimento) Implements I_Operazione_Colturale.Costi_Accessori_Temporaneo
            Get
                Return _Costi_Accessori_Temporaneo
            End Get
            Set(value As List(Of Movimento))
                _Costi_Accessori_Temporaneo = value
            End Set
        End Property

#End Region

    End Class





End Namespace

