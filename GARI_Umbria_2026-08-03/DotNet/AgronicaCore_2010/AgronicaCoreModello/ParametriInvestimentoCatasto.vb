Imports AgronicaCoreModello.OperazioneAgenda_Temp
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports Newtonsoft.Json


Namespace ParametriInvestimentoCatasto_Temp


    Public Class ParametriInvestimentoCatasto


        Private _WParametriInvestimentoCatasto As WrappedParametriInvestimentoCatasto

        Private _UsaSession As Boolean = True


        ''' <summary>
        ''' Qusto costruttore permette di istanziare l'oggetto a prescindere dalla sessione (es.: Gias 2 Gias)
        ''' </summary>
        ''' <param name="idle_NoSession"></param>
        Sub New(ByVal idle_NoSession As Boolean)
            _WParametriInvestimentoCatasto = New WrappedParametriInvestimentoCatasto
            _UsaSession = False
        End Sub



        Sub New()
            'quando creo in una pagina un oggetto ParametriAgenda recupero quello nella sessione,
            'se non è presente ne creo uno in sessione.
            'non ho bisogno di avere una copia dell'oggetto della sessione in una variabile dato che viene 
            'riletto dalla sessione ad ogni accesso
            If Not IsNothing(System.Web.HttpContext.Current) Then

                If Not IsNothing(System.Web.HttpContext.Current.Session) Then
                    If IsNothing(System.Web.HttpContext.Current.Session("ParametriInvestimentoCatasto")) Then
                        System.Web.HttpContext.Current.Session("ParametriInvestimentoCatasto") = New WrappedParametriInvestimentoCatasto
                    Else
                        'ho objagenda in sessione
                    End If
                Else

                End If

            End If

        End Sub

        'quando modifico una proprietà dell'oggetto WParametriAgenda devo chiamare salva() per salvarlo subito in sessione 
        Public Sub salva()
            If _UsaSession Then
                System.Web.HttpContext.Current.Session("ParametriAgenda") = _WParametriInvestimentoCatasto
            End If
        End Sub


        Public Sub RecuperaRientro()

            If Not System.Web.HttpContext.Current.Session("ImpostaRientro_back") Is Nothing Then
                _WParametriInvestimentoCatasto = Newtonsoft.Json.JsonConvert.DeserializeObject(Of WrappedParametriInvestimentoCatasto)(System.Web.HttpContext.Current.Session("ImpostaRientro_back"))
                salva()

                System.Web.HttpContext.Current.Session.Remove("ImpostaRientro_back")

            End If

        End Sub

        Public Sub ImpostaPerRientro()

            Dim WParametriSerializzati As String = Newtonsoft.Json.JsonConvert.SerializeObject(WParametriInvestimentoCatasto)
            System.Web.HttpContext.Current.Session("ImpostaRientro_back") = WParametriSerializzati

        End Sub

        Public Overrides Function ToString() As String
            Return JsonConvert.SerializeObject(Me)
        End Function

#Region "Proprietà"

        'questa proprietà è readonly e deve rimanere privata,
        'è stata creata solo per automatizzare la lettura di parametri agenda dalla sessione 
        'e l'associazione alla variabile _WParametriAgenda.
        'in questo modo evito di eseguire leggi() ad ogni accesso in lettura e scrittura di una proprietà
        Private ReadOnly Property WParametriInvestimentoCatasto() As WrappedParametriInvestimentoCatasto
            Get
                If _UsaSession Then
                    _WParametriInvestimentoCatasto = System.Web.HttpContext.Current.Session("ParametriInvestimentoCatasto")
                End If
                Return _WParametriInvestimentoCatasto
            End Get
        End Property


        Public Property SitoOrigine() As Integer
            Get
                Return WParametriInvestimentoCatasto.SitoOrigine
            End Get
            Set(value As Integer)
                WParametriInvestimentoCatasto.SitoOrigine = value
                salva()
            End Set
        End Property

        Public Property PaginaSitoOrigine() As Integer
            Get
                Return WParametriInvestimentoCatasto.PaginaSitoOrigine
            End Get
            Set(value As Integer)
                WParametriInvestimentoCatasto.PaginaSitoOrigine = value
                salva()
            End Set
        End Property




        Property SaNome() As String
            Get
                Return WParametriInvestimentoCatasto.SaNome
            End Get
            Set(ByVal Value As String)
                WParametriInvestimentoCatasto.SaNome = Value
                salva()
            End Set
        End Property

        Property RagSoc() As String
            Get
                Return WParametriInvestimentoCatasto.RagSoc
            End Get
            Set(ByVal Value As String)
                WParametriInvestimentoCatasto.RagSoc = Value
                salva()
            End Set
        End Property


        Public Property TornaASitoOrigine() As Boolean
            Get
                Return WParametriInvestimentoCatasto.TornaASitoOrigine
            End Get
            Set(value As Boolean)
                WParametriInvestimentoCatasto.TornaASitoOrigine = value
                salva()
            End Set
        End Property



        Public Property PaginaDestinazioneFiltrino() As Integer
            Get
                Return WParametriInvestimentoCatasto.PaginaDestinazioneFiltrino
            End Get
            Set(value As Integer)
                WParametriInvestimentoCatasto.PaginaDestinazioneFiltrino = value
                salva()
            End Set
        End Property

        Public Property SitoDestinazioneFiltrino() As Integer
            Get
                Return WParametriInvestimentoCatasto.SitoDestinazioneFiltrino
            End Get
            Set(value As Integer)
                WParametriInvestimentoCatasto.SitoDestinazioneFiltrino = value
                salva()
            End Set
        End Property

        Public Property QueryStringFiltrino() As String
            Get
                Return WParametriInvestimentoCatasto.QueryStringFiltrino
            End Get
            Set(value As String)
                WParametriInvestimentoCatasto.QueryStringFiltrino = value
                salva()
            End Set
        End Property

        Property Data() As Date
            Get
                Return WParametriInvestimentoCatasto.Data
            End Get
            Set(ByVal Value As Date)
                WParametriInvestimentoCatasto.Data = Value
                salva()
            End Set
        End Property

        Property Piva() As String
            Get
                Return WParametriInvestimentoCatasto.Piva
            End Get
            Set(ByVal Value As String)
                WParametriInvestimentoCatasto.Piva = Value
                salva()
            End Set
        End Property

        Property Sa_Cod() As String
            Get
                Dim aa As String
                If WParametriInvestimentoCatasto.Sa_Cod.Split("/").Length > 1 Then
                    aa = WParametriInvestimentoCatasto.Sa_Cod.Split("/")(0)
                Else
                    If WParametriInvestimentoCatasto.Sa_Cod.Length > 0 Then
                        aa = WParametriInvestimentoCatasto.Sa_Cod
                    Else
                        aa = "0"
                    End If
                End If
                Return aa
            End Get
            Set(ByVal Value As String)
                WParametriInvestimentoCatasto.Sa_Cod = Value
                salva()
            End Set
        End Property

        Property Campo_Cod() As String
            Get
                If IsNumeric(WParametriInvestimentoCatasto.Campo_Cod) Then
                    Return WParametriInvestimentoCatasto.Campo_Cod
                Else
                    Return 0
                End If
            End Get
            Set(ByVal Value As String)
                WParametriInvestimentoCatasto.Campo_Cod = Value
                salva()
            End Set
        End Property

        Property FiltroRicerca() As String
            Get
                Return WParametriInvestimentoCatasto.FiltroRicerca
            End Get
            Set(ByVal Value As String)
                WParametriInvestimentoCatasto.FiltroRicerca = Value
                salva()
            End Set
        End Property

        Property Campi() As List(Of Campo)
            Get
                Return WParametriInvestimentoCatasto.Campi
            End Get
            Set(ByVal Value As List(Of Campo))
                WParametriInvestimentoCatasto.Campi = Value
                salva()
            End Set
        End Property

        Property Note() As List(Of Nota)
            Get
                Return WParametriInvestimentoCatasto.Note
            End Get
            Set(ByVal Value As List(Of Nota))
                WParametriInvestimentoCatasto.Note = Value
                salva()
            End Set
        End Property

        Property Piva_Origine() As String
            Get
                Return WParametriInvestimentoCatasto.Piva_Origine
            End Get
            Set(ByVal Value As String)
                WParametriInvestimentoCatasto.Piva_Origine = Value
                salva()
            End Set
        End Property

        Property LinkAgronicaAgenda2010() As String
            Get
                Return WParametriInvestimentoCatasto.LinkAgronicaAgenda2010
            End Get
            Set(ByVal Value As String)
                WParametriInvestimentoCatasto.LinkAgronicaAgenda2010 = Value
                salva()
            End Set
        End Property

        Property StrGenericaXlinkGiasOnline() As String
            Get
                Return WParametriInvestimentoCatasto.StrGenericaxlinkGiasOnline
            End Get
            Set(ByVal Value As String)
                WParametriInvestimentoCatasto.StrGenericaxlinkGiasOnline = Value
                salva()
            End Set
        End Property

#End Region
    End Class

    Class WrappedParametriInvestimentoCatasto


        Private _TornaASitoOrigine As Boolean
        Private _SitoOrigine As Integer
        Private _PaginaSitoOrigine As Integer

        Private _Data As Date
        Private _RagSoc As String
        Private _SaNome As String
        Private _Piva As String
        Private _Sa_Cod As String
        Private _FiltroRicerca As String

        Private _Campi As List(Of Campo)
        Private _Note As IList(Of Nota)

        Private _QueryStringFiltrino As String
        Private _PaginaDestinazioneFiltrino As Integer
        Private _SitoDestinazioneFiltrino As Integer

        Private _Campo_Cod As String

        Private _Piva_Origine As String

        Private _LinkAgronicaAgenda2010 As String
        Private _StrGenericaxlinkGiasOnline As String


        Sub New()

            _Data = Date.Now
            _TornaASitoOrigine = False
            _SitoOrigine = 0
            _PaginaSitoOrigine = 0
            _Piva = ""
            _RagSoc = ""
            _Sa_Cod = "0"

            _FiltroRicerca = ""
            _SaNome = ""

            _Campi = New List(Of Campo)
            _Note = New List(Of Nota)

            _QueryStringFiltrino = ""
            _PaginaDestinazioneFiltrino = 0
            _SitoDestinazioneFiltrino = 0

            _Campo_Cod = ""

            _Piva_Origine = ""

            _LinkAgronicaAgenda2010 = ""

        End Sub

#Region "Proprietà"
        Public Property SitoOrigine() As Integer
            Get
                Return _SitoOrigine
            End Get
            Set(value As Integer)
                _SitoOrigine = value
            End Set
        End Property

        Public Property PaginaSitoOrigine() As Integer
            Get
                Return _PaginaSitoOrigine
            End Get
            Set(value As Integer)
                _PaginaSitoOrigine = value
            End Set
        End Property

        Property SaNome() As String
            Get
                Return _SaNome
            End Get
            Set(ByVal Value As String)
                _SaNome = Value
            End Set
        End Property

        Property RagSoc() As String
            Get
                Return _RagSoc
            End Get
            Set(ByVal Value As String)
                _RagSoc = Value
            End Set
        End Property

        Public Property TornaASitoOrigine() As Boolean
            Get
                Return _TornaASitoOrigine
            End Get
            Set(value As Boolean)
                _TornaASitoOrigine = value
            End Set
        End Property

        Property Data() As Date
            Get
                Return _Data
            End Get
            Set(ByVal Value As Date)
                _Data = Value
            End Set
        End Property

        Property Piva() As String
            Get
                Return _Piva
            End Get
            Set(ByVal Value As String)
                _Piva = Value
            End Set
        End Property

        Property Sa_Cod() As String
            Get
                Dim aa As String
                If _Sa_Cod.Split("/").Length > 1 Then
                    aa = _Sa_Cod.Split("/")(0)
                Else
                    If _Sa_Cod.Length > 0 Then
                        aa = _Sa_Cod
                    Else
                        aa = "0"
                    End If
                End If
                Return aa
            End Get
            Set(ByVal Value As String)
                _Sa_Cod = Value
            End Set
        End Property

        Property Campo_Cod() As String
            Get
                Return _Campo_Cod
            End Get
            Set(ByVal Value As String)
                _Campo_Cod = Value
            End Set
        End Property

        Property FiltroRicerca() As String
            Get
                Return _FiltroRicerca
            End Get
            Set(ByVal Value As String)
                _FiltroRicerca = Value
            End Set
        End Property

        Property Campi() As List(Of Campo)
            Get
                Return _Campi
            End Get
            Set(ByVal Value As List(Of Campo))
                _Campi = Value
            End Set
        End Property

        Property Note() As List(Of Nota)
            Get
                Return _Note
            End Get
            Set(ByVal Value As List(Of Nota))
                _Note = Value
            End Set
        End Property

        Public Property QueryStringFiltrino() As String
            Get
                Return _QueryStringFiltrino
            End Get
            Set(value As String)
                _QueryStringFiltrino = value
            End Set
        End Property

        Public Property SitoDestinazioneFiltrino() As Integer
            Get
                Return _SitoDestinazioneFiltrino
            End Get
            Set(value As Integer)
                _SitoDestinazioneFiltrino = value
            End Set
        End Property

        Public Property PaginaDestinazioneFiltrino() As Integer
            Get
                Return _PaginaDestinazioneFiltrino
            End Get
            Set(value As Integer)
                _PaginaDestinazioneFiltrino = value
            End Set
        End Property

        Property Piva_Origine() As String
            Get
                Return _Piva_Origine
            End Get
            Set(ByVal Value As String)
                _Piva_Origine = Value
            End Set
        End Property

        Property LinkAgronicaAgenda2010() As String
            Get
                Return _LinkAgronicaAgenda2010
            End Get
            Set(ByVal Value As String)
                _LinkAgronicaAgenda2010 = Value
            End Set
        End Property

        Property StrGenericaxlinkGiasOnline() As String
            Get
                Return _StrGenericaxlinkGiasOnline
            End Get
            Set(ByVal Value As String)
                _StrGenericaxlinkGiasOnline = Value
            End Set
        End Property

#End Region

    End Class


    Public Class Campo

        Public Piva As String
        Public Sa_Cod As Integer
        Public Campo_Cod As Integer

        Public Campo_Des As String

        Public Sup_Campo As Decimal

        Public Rag_Soc As String
        Public Sa_Nome As String

        Public Validita_Inizio As Date
        Public Validita_Fine As Date

    End Class

End Namespace