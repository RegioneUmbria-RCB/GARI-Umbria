Imports AgronicaCoreModello.ParametriAgenda_Temp

Namespace Agenda.Util

    Public Enum Enum_TipoElementoDaVerificare

        AnalizzaOperazione_DaOggettoAgenda = 1
        AnalizzaOperazioni_DaImpianto = 2
        AnalizzaOperazione_DaXmlAgenda = 3
        AnalizzaOperazioni_DaListaIdAgenda = 4
        AnalizzaOperazioni_DaListaImpianti = 5
        AnalizzaOperazioni_DaImpresaSpecieDate = 6
        AnalizzaOperazioni_DaListaAttivita = 7
    End Enum
    Public Class Elemento_Verifica_Disciplinare

        Private _Operazione_Colturale As OperazioneAgenda_Temp.Operazione_Agenda

        Private _Impianto_Da_Controllare As AgronicaCoreModello.Anagrafe.Impianto_Colturale
        Private _Disciplinare_Cod As String
        Private _Disciplinare_Des As String
        Private _Disciplinare_PubblicoPrivato As Integer
        Private _TipoElementoDaVerificare As Enum_TipoElementoDaVerificare

        Private _Piva As String

        Private _Lista_Id_Agenda As List(Of Integer)
        Private _Lista_Impianti_Da_Controllare As List(Of AgronicaCoreModello.Anagrafe.Impianto_Colturale)

        'da eliminare e sostituire in futuro con operazione colturale
        Private _StringaXmlAgenda As String
        Private _objParametriAgenda As ParametriAgenda

        Private _Data_Inizio As Date
        Private _Data_Fine As Date

        Private _Lista_Attivita As List(Of AgronicaCoreModelsSTD.attivita.Attivita)

        Sub New(ByVal Disciplinare_Cod_In As String,
                ByVal Disciplinare_PubblicoPrivato_In As Integer,
                ByVal Disciplinare_Des_In As String,
                ByRef Operazione_Colturale_In As OperazioneAgenda_Temp.Operazione_Agenda) ', _

            _Impianto_Da_Controllare = Nothing
            _StringaXmlAgenda = Nothing
            _Lista_Id_Agenda = Nothing
            _Lista_Impianti_Da_Controllare = Nothing

            _Operazione_Colturale = Operazione_Colturale_In
            _Disciplinare_Cod = Disciplinare_Cod_In
            _Disciplinare_PubblicoPrivato = Disciplinare_PubblicoPrivato_In
            _Disciplinare_Des = Disciplinare_Des_In

            _TipoElementoDaVerificare = Enum_TipoElementoDaVerificare.AnalizzaOperazione_DaOggettoAgenda

        End Sub

        Sub New(ByVal Disciplinare_Cod_In As String,
                ByVal Disciplinare_PubblicoPrivato_In As Integer,
                ByVal Disciplinare_Des_In As String,
                ByRef Lista_Id_Agenda_In As List(Of Integer)) ', _

            _Impianto_Da_Controllare = Nothing
            _StringaXmlAgenda = Nothing
            _Operazione_Colturale = Nothing
            _Lista_Impianti_Da_Controllare = Nothing

            _Lista_Id_Agenda = Lista_Id_Agenda_In
            _Disciplinare_Cod = Disciplinare_Cod_In
            _Disciplinare_PubblicoPrivato = Disciplinare_PubblicoPrivato_In
            _Disciplinare_Des = Disciplinare_Des_In

            _TipoElementoDaVerificare = Enum_TipoElementoDaVerificare.AnalizzaOperazioni_DaListaIdAgenda


        End Sub

        Sub New(ByVal Disciplinare_Cod_In As Integer,
                    ByVal Disciplinare_PubblicoPrivato_In As Integer,
                    ByVal Disciplinare_Des_In As String,
                    ByRef Lista_Impianti_In As List(Of AgronicaCoreModello.Anagrafe.Impianto_Colturale)) ', _

            _Impianto_Da_Controllare = Nothing
            _StringaXmlAgenda = Nothing
            _Operazione_Colturale = Nothing
            _Lista_Id_Agenda = Nothing

            _Lista_Impianti_Da_Controllare = Lista_Impianti_In
            _Disciplinare_Cod = Disciplinare_Cod_In
            _Disciplinare_PubblicoPrivato = Disciplinare_PubblicoPrivato_In
            _Disciplinare_Des = Disciplinare_Des_In

            _Data_Inizio = Data_Inizio
            _Data_Fine = Data_Fine

            _TipoElementoDaVerificare = Enum_TipoElementoDaVerificare.AnalizzaOperazioni_DaListaImpianti


        End Sub

        Sub New(ByVal Disciplinare_Cod_In As Integer,
                ByVal Disciplinare_PubblicoPrivato_In As Integer,
                ByVal Disciplinare_Des_In As String,
                ByRef Impianto_Da_Controllare_In As AgronicaCoreModello.Anagrafe.Impianto_Colturale)

            _StringaXmlAgenda = Nothing
            _Operazione_Colturale = Nothing
            _Lista_Id_Agenda = Nothing
            _Lista_Impianti_Da_Controllare = Nothing

            _Impianto_Da_Controllare = Impianto_Da_Controllare_In
            _Disciplinare_Cod = Disciplinare_Cod_In
            _Disciplinare_PubblicoPrivato = Disciplinare_PubblicoPrivato_In
            _Disciplinare_Des = Disciplinare_Des_In

            _TipoElementoDaVerificare = Enum_TipoElementoDaVerificare.AnalizzaOperazioni_DaImpianto

        End Sub

        'da eliminare e sostituire in futuro con operazione colturale
        Sub New(ByVal StringaXmlAgenda_In As String, ByRef objParametriAgenda_In As ParametriAgenda)

            _Impianto_Da_Controllare = Nothing
            _Operazione_Colturale = Nothing
            _Lista_Id_Agenda = Nothing
            _Lista_Impianti_Da_Controllare = Nothing

            'ìper avere dati piva etc facilmente
            _objParametriAgenda = objParametriAgenda_In
            _StringaXmlAgenda = StringaXmlAgenda_In 'da eliminare e sostituire in futuro con operazione colturale
            _Disciplinare_Cod = 0
            _Disciplinare_PubblicoPrivato = 0

            _TipoElementoDaVerificare = Enum_TipoElementoDaVerificare.AnalizzaOperazione_DaXmlAgenda


        End Sub

        Sub New()

            _Impianto_Da_Controllare = Nothing
            _Operazione_Colturale = Nothing
            _Lista_Id_Agenda = Nothing
            _Lista_Impianti_Da_Controllare = Nothing

            'ìper avere dati piva etc facilmente
            _objParametriAgenda = Nothing
            _StringaXmlAgenda = "" 'da eliminare e sostituire in futuro con operazione colturale
            _Disciplinare_Cod = 0
            _Disciplinare_PubblicoPrivato = 0

            _TipoElementoDaVerificare = Enum_TipoElementoDaVerificare.AnalizzaOperazioni_DaImpresaSpecieDate


        End Sub


        Public Property Disciplinare_Cod() As String
            Get
                Return _Disciplinare_Cod
            End Get
            Set(value As String)
                _Disciplinare_Cod = value
            End Set
        End Property

        Public Property Disciplinare_PubblicoPrivato() As Integer
            Get
                Return _Disciplinare_PubblicoPrivato
            End Get
            Set(value As Integer)
                _Disciplinare_PubblicoPrivato = value
            End Set
        End Property

        Public Property Disciplinare_Des() As String
            Get
                Return _Disciplinare_Des
            End Get
            Set(value As String)
                _Disciplinare_Des = value
            End Set
        End Property

        Public Property Piva() As String
            Get
                Return _Piva
            End Get
            Set(value As String)
                _Piva = value
            End Set
        End Property

        Public ReadOnly Property Impianto_Da_Controllare() As AgronicaCoreModello.Anagrafe.Impianto_Colturale
            Get
                Return _Impianto_Da_Controllare
            End Get
            'Set(value As AgronicaCoreModello.Anagrafe.Impianto_Colturale)
            '    _Impianto_Da_Controllare = value
            '    _Operazione_Colturale = Nothing
            'End Set
        End Property

        Public ReadOnly Property Lista_Impianti_Da_Controllare() As List(Of AgronicaCoreModello.Anagrafe.Impianto_Colturale)
            Get
                Return _Lista_Impianti_Da_Controllare
            End Get
        End Property

        Public ReadOnly Property Operazione_Colturale() As AgronicaCoreModello.OperazioneAgenda_Temp.Operazione_Agenda
            Get
                Return _Operazione_Colturale
            End Get
            'Set(value As AgronicaCoreModello.Agenda.Operazioni_Colturali.Operazione_Colturale.I_Operazione_Colturale)
            '    _Impianto_Da_Controllare = Nothing
            '    _Operazione_Colturale = value
            'End Set
        End Property
        Public ReadOnly Property Lista_Id_Agenda() As List(Of Integer)
            Get
                Return _Lista_Id_Agenda
            End Get
        End Property
        Public ReadOnly Property StringaXmlAgenda() As String
            Get
                Return _StringaXmlAgenda
            End Get
        End Property
        Public ReadOnly Property ObjParametriAgenda() As ParametriAgenda
            Get
                Return _objParametriAgenda
            End Get
        End Property
        Public Property TipoElementoDaVerificare() As Integer
            Get
                Return _TipoElementoDaVerificare
            End Get

            Set(value As Integer)
                _TipoElementoDaVerificare = value
            End Set
        End Property

        Public Property Data_Inizio() As Date
            Get
                Return _Data_Inizio
            End Get
            Set(value As Date)
                _Data_Inizio = value
            End Set
        End Property
        Public Property Data_Fine() As Date
            Get
                Return _Data_Fine
            End Get
            Set(value As Date)
                _Data_Fine = value
            End Set
        End Property


        Public Property Lista_Attivita() As List(Of AgronicaCoreModelsSTD.attivita.Attivita)
            Get
                Return _Lista_Attivita
            End Get
            Set(value As List(Of AgronicaCoreModelsSTD.attivita.Attivita))
                _Lista_Attivita = value
            End Set
        End Property

    End Class

End Namespace

