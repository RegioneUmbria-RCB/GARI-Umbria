Imports AgronicaCoreModello.OperazioneAgenda_Temp
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports Newtonsoft.Json

Namespace ParametriAgenda_Temp


    Public Class ParametriAgenda

        Private _WParametriAgenda As WrappedParametriAgenda

        Private _UsaSession As Boolean = True


        ''' <summary>
        ''' Qusto costruttore permette di istanziare l'oggetto a prescindere dalla sessione (es.: Gias 2 Gias)
        ''' </summary>
        ''' <param name="idle_NoSession"></param>
        Sub New(ByVal idle_NoSession As Boolean)
            _WParametriAgenda = New WrappedParametriAgenda
            _UsaSession = False
        End Sub



        Sub New()
            'quando creo in una pagina un oggetto ParametriAgenda recupero quello nella sessione,
            'se non è presente ne creo uno in sessione.
            'non ho bisogno di avere una copia dell'oggetto della sessione in una variabile dato che viene 
            'riletto dalla sessione ad ogni accesso
            If Not IsNothing(System.Web.HttpContext.Current) Then

                If Not IsNothing(System.Web.HttpContext.Current.Session) Then
                    If IsNothing(System.Web.HttpContext.Current.Session("ParametriAgenda")) Then
                        System.Web.HttpContext.Current.Session("ParametriAgenda") = New WrappedParametriAgenda
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
                System.Web.HttpContext.Current.Session("ParametriAgenda") = _WParametriAgenda
            End If
        End Sub


        Public Sub RecuperaRientro()

            If Not System.Web.HttpContext.Current.Session("ImpostaRientro_back") Is Nothing Then
                _WParametriAgenda = Newtonsoft.Json.JsonConvert.DeserializeObject(Of WrappedParametriAgenda)(System.Web.HttpContext.Current.Session("ImpostaRientro_back"))
                salva()

                System.Web.HttpContext.Current.Session.Remove("ImpostaRientro_back")

            End If

        End Sub

        Public Sub ImpostaPerRientro()

            Dim WParametriSerializzati As String = Newtonsoft.Json.JsonConvert.SerializeObject(WParametriAgenda)
            System.Web.HttpContext.Current.Session("ImpostaRientro_back") = WParametriSerializzati

        End Sub



        Public Sub Svuota_DatiOperazione()
            WParametriAgenda.Svuota_DatiOperazione()
            salva()
        End Sub

        Public Overrides Function ToString() As String
            Return JsonConvert.SerializeObject(Me)
        End Function

#Region "Proprietà"

        'questa proprietà è readonly e deve rimanere privata,
        'è stata creata solo per automatizzare la lettura di parametri agenda dalla sessione 
        'e l'associazione alla variabile _WParametriAgenda.
        'in questo modo evito di eseguire leggi() ad ogni accesso in lettura e scrittura di una proprietà
        Private ReadOnly Property WParametriAgenda() As WrappedParametriAgenda
            Get
                If _UsaSession Then
                    _WParametriAgenda = System.Web.HttpContext.Current.Session("ParametriAgenda")
                End If
                Return _WParametriAgenda
            End Get
        End Property


        Public Property SitoOrigine() As Integer
            Get
                Return WParametriAgenda.SitoOrigine
            End Get
            Set(value As Integer)
                WParametriAgenda.SitoOrigine = value
                salva()
            End Set
        End Property

        Public Property PaginaSitoOrigine() As Integer
            Get
                Return WParametriAgenda.PaginaSitoOrigine
            End Get
            Set(value As Integer)
                WParametriAgenda.PaginaSitoOrigine = value
                salva()
            End Set
        End Property

        Public Property Cau_Mov() As String
            Get
                Return WParametriAgenda.Cau_Mov
            End Get
            Set(value As String)

                WParametriAgenda.Cau_Mov = value
                salva()
            End Set
        End Property

        Public Property Mac_Cod() As String
            Get
                Return WParametriAgenda.Mac_Cod
            End Get
            Set(value As String)

                WParametriAgenda.Mac_Cod = value
                salva()
            End Set
        End Property

        Public Property Lav_Des() As String
            Get
                Return WParametriAgenda.Lav_Des
            End Get
            Set(value As String)
                WParametriAgenda.Lav_Des = value
                salva()
            End Set
        End Property

        Property SaNome() As String
            Get
                Return WParametriAgenda.SaNome
            End Get
            Set(ByVal Value As String)
                WParametriAgenda.SaNome = Value
                salva()
            End Set
        End Property

        Property RagSoc() As String
            Get
                Return WParametriAgenda.RagSoc
            End Get
            Set(ByVal Value As String)
                WParametriAgenda.RagSoc = Value
                salva()
            End Set
        End Property

        Property Movimenti() As List(Of Movimento)
            Get
                Return WParametriAgenda.Movimenti
            End Get
            Set(ByVal Value As List(Of Movimento))
                WParametriAgenda.Movimenti = Value
                salva()
            End Set
        End Property

        Public Property TornaASitoOrigine() As Boolean
            Get
                Return WParametriAgenda.TornaASitoOrigine
            End Get
            Set(value As Boolean)
                WParametriAgenda.TornaASitoOrigine = value
                salva()
            End Set
        End Property

        Public Property TrappolaUso() As String
            Get
                Return WParametriAgenda.TrappolaUso
            End Get
            Set(value As String)
                WParametriAgenda.TrappolaUso = value
                salva()
            End Set
        End Property

        Public Property PaginaDestinazioneFiltrino() As Integer
            Get
                Return WParametriAgenda.PaginaDestinazioneFiltrino
            End Get
            Set(value As Integer)
                WParametriAgenda.PaginaDestinazioneFiltrino = value
                salva()
            End Set
        End Property

        Public Property SitoDestinazioneFiltrino() As Integer
            Get
                Return WParametriAgenda.SitoDestinazioneFiltrino
            End Get
            Set(value As Integer)
                WParametriAgenda.SitoDestinazioneFiltrino = value
                salva()
            End Set
        End Property

        Public Property QueryStringFiltrino() As String
            Get
                Return WParametriAgenda.QueryStringFiltrino
            End Get
            Set(value As String)
                WParametriAgenda.QueryStringFiltrino = value
                salva()
            End Set
        End Property

        Property WS_Fitofarmaci_AgroWS_Fitofarmaci() As String
            Get
                Return WParametriAgenda.WS_Fitofarmaci_AgroWS_Fitofarmaci
            End Get
            Set(ByVal Value As String)
                WParametriAgenda.WS_Fitofarmaci_AgroWS_Fitofarmaci = Value
                salva()
            End Set
        End Property

        Property WS_Disciplinari_AgroWS_Disciplinari() As String
            Get
                Return WParametriAgenda.WS_Disciplinari_AgroWS_Disciplinari
            End Get
            Set(ByVal Value As String)
                WParametriAgenda.WS_Disciplinari_AgroWS_Disciplinari = Value
                salva()
            End Set
        End Property

        Property Data() As Date
            Get
                Return WParametriAgenda.Data
            End Get
            Set(ByVal Value As Date)
                WParametriAgenda.Data = Value
                salva()
            End Set
        End Property

        Property Piva() As String
            Get
                Return WParametriAgenda.Piva
            End Get
            Set(ByVal Value As String)
                WParametriAgenda.Piva = Value
                salva()
            End Set
        End Property

        Property Sa_Cod() As String
            Get
                Dim aa As String
                If WParametriAgenda.Sa_Cod.Split("/").Length > 1 Then
                    aa = WParametriAgenda.Sa_Cod.Split("/")(0)
                Else
                    If WParametriAgenda.Sa_Cod.Length > 0 Then
                        aa = WParametriAgenda.Sa_Cod
                    Else
                        aa = "0"
                    End If
                End If
                Return aa
            End Get
            Set(ByVal Value As String)
                WParametriAgenda.Sa_Cod = Value
                salva()
            End Set
        End Property

        Property Appezza() As String
            Get
                Return WParametriAgenda.Appezza
            End Get
            Set(ByVal Value As String)
                WParametriAgenda.Appezza = Value
                salva()
            End Set
        End Property

        Property Id_Imp() As String
            Get
                Return WParametriAgenda.Id_Imp
            End Get
            Set(ByVal Value As String)
                WParametriAgenda.Id_Imp = Value
                salva()
            End Set
        End Property

        Property Campo_Cod() As String
            Get
                If IsNumeric(WParametriAgenda.Campo_Cod) Then
                    Return WParametriAgenda.Campo_Cod
                Else
                    Return 0
                End If
            End Get
            Set(ByVal Value As String)
                WParametriAgenda.Campo_Cod = Value
                salva()
            End Set
        End Property

        Property TipoOperazioneAgenda() As String
            Get
                Return WParametriAgenda.TipoOperazioneAgenda
            End Get
            Set(ByVal Value As String)
                WParametriAgenda.TipoOperazioneAgenda = Value
                salva()
            End Set
        End Property

        Property TargetOperazione() As enum_Tipo_Operazione_Agenda_Target
            Get
                Return WParametriAgenda.TargetOperazione
            End Get
            Set(ByVal Value As enum_Tipo_Operazione_Agenda_Target)
                WParametriAgenda.TargetOperazione = Value
                salva()
            End Set
        End Property


        Property TipoRicetta() As enum_TipoRicetta
            Get
                Return WParametriAgenda.TipoRicetta
            End Get
            Set(ByVal Value As enum_TipoRicetta)
                WParametriAgenda.TipoRicetta = Value
                salva()
            End Set
        End Property

        Property Programmazione_Cod() As Integer
            Get
                Return WParametriAgenda.Programmazione_Cod
            End Get
            Set(ByVal Value As Integer)
                WParametriAgenda.Programmazione_Cod = Value
                salva()
            End Set
        End Property

        Property TipoOperazioneColturale() As String
            Get
                Return WParametriAgenda.TipoOperazioneColturale
            End Get
            Set(ByVal Value As String)
                WParametriAgenda.TipoOperazioneColturale = Value
                salva()
            End Set
        End Property

        Property GruppoOperazioneColturale() As String
            Get
                Return WParametriAgenda.GruppoOperazioneColturale
            End Get
            Set(ByVal Value As String)
                WParametriAgenda.GruppoOperazioneColturale = Value
                salva()
            End Set
        End Property

        Property Des_lib() As String
            Get
                Return WParametriAgenda.Des_lib
            End Get
            Set(value As String)
                WParametriAgenda.Des_lib = value
            End Set
        End Property

        Property Lav_Cod() As String
            Get
                Return WParametriAgenda.Lav_Cod
            End Get
            Set(ByVal Value As String)
                WParametriAgenda.Lav_Cod = Value
                salva()
            End Set
        End Property

        Property Elem_Cod() As String
            Get
                Return WParametriAgenda.Elem_Cod
            End Get
            Set(ByVal Value As String)
                WParametriAgenda.Elem_Cod = Value
                salva()
            End Set
        End Property

        Property Id_Agenda() As String
            Get
                Return WParametriAgenda.Id_Agenda
            End Get
            Set(ByVal Value As String)
                WParametriAgenda.Id_Agenda = Value
                salva()
            End Set
        End Property

        Property Raccoglitore_Cod() As String
            Get
                Return WParametriAgenda.Raccoglitore_Cod
            End Get
            Set(ByVal Value As String)
                WParametriAgenda.Raccoglitore_Cod = Value
                salva()
            End Set
        End Property

        Property Tipo_Operazione() As String
            Get
                Return WParametriAgenda.Tipo_Operazione
            End Get
            Set(ByVal Value As String)
                WParametriAgenda.Tipo_Operazione = Value
                salva()
            End Set
        End Property

        Property Veg_Cod() As String
            Get
                Return WParametriAgenda.Veg_Cod
            End Get
            Set(ByVal Value As String)
                WParametriAgenda.Veg_Cod = Value
                salva()
            End Set
        End Property

        Property Cul_Cod() As String
            Get
                Return WParametriAgenda.Cul_Cod
            End Get
            Set(ByVal Value As String)
                WParametriAgenda.Cul_Cod = Value
                salva()
            End Set
        End Property

        Property Fabbricato() As String
            Get
                Return WParametriAgenda.Fabbricato
            End Get
            Set(ByVal Value As String)
                WParametriAgenda.Fabbricato = Value
                salva()
            End Set
        End Property

        Property Disciplinare() As String
            Get
                If WParametriAgenda.Disciplinare = "" Then
                    Return "0"
                End If
                Return WParametriAgenda.Disciplinare
            End Get
            Set(ByVal Value As String)
                WParametriAgenda.Disciplinare = Value
                salva()
            End Set
        End Property

        Property FiltroRicerca() As String
            Get
                Return WParametriAgenda.FiltroRicerca
            End Get
            Set(ByVal Value As String)
                WParametriAgenda.FiltroRicerca = Value
                salva()
            End Set
        End Property

        Property Impianti() As List(Of Impianto)
            Get
                Return WParametriAgenda.Impianti
            End Get
            Set(ByVal Value As List(Of Impianto))
                WParametriAgenda.Impianti = Value
                salva()
            End Set
        End Property

        Property Note() As List(Of Nota)
            Get
                Return WParametriAgenda.Note
            End Get
            Set(ByVal Value As List(Of Nota))
                WParametriAgenda.Note = Value
                salva()
            End Set
        End Property

        Public Property OperazioneMulticentro() As Boolean
            Get
                Return WParametriAgenda.OperazioneMulticentro
            End Get
            Set(value As Boolean)
                WParametriAgenda.OperazioneMulticentro = value
                salva()
            End Set
        End Property

        Public Property InstallazioneTrappola() As ParametriInstallazioneTrappole
            Get
                Return WParametriAgenda.InstallazioneTrappola
            End Get
            Set(value As ParametriInstallazioneTrappole)
                WParametriAgenda.InstallazioneTrappola = value
                salva()
            End Set
        End Property

        Property Rilievi As List(Of rilievoAvv)
            Get
                Return WParametriAgenda.Rilievi
            End Get
            Set(value As List(Of rilievoAvv))
                WParametriAgenda.Rilievi = value
                salva()
            End Set
        End Property


        Property Particelle() As List(Of Particella)
            Get
                Return WParametriAgenda.Particelle
            End Get
            Set(ByVal Value As List(Of Particella))
                WParametriAgenda.Particelle = Value
                salva()
            End Set
        End Property

        Property ClassiTessitura() As List(Of Integer)
            Get
                Return WParametriAgenda.ClassiTessitura
            End Get
            Set(ByVal Value As List(Of Integer))
                WParametriAgenda.ClassiTessitura = Value
                salva()
            End Set
        End Property

        Property Lotto() As String
            Get
                Return WParametriAgenda.Lotto
            End Get
            Set(ByVal Value As String)
                WParametriAgenda.Lotto = Value
                salva()
            End Set
        End Property

        Property Cod_Contatto() As String
            Get
                Return WParametriAgenda.Cod_Contatto
            End Get
            Set(ByVal Value As String)
                WParametriAgenda.Cod_Contatto = Value
                salva()
            End Set
        End Property

        Property Piva_Origine() As String
            Get
                Return WParametriAgenda.Piva_Origine
            End Get
            Set(ByVal Value As String)
                WParametriAgenda.Piva_Origine = Value
                salva()
            End Set
        End Property

        Public Property Qualifica_Cod() As Integer
            Get
                Return WParametriAgenda.Qualifica_Cod
            End Get
            Set(value As Integer)
                WParametriAgenda.Qualifica_Cod = value
                salva()
            End Set
        End Property

        Public Property Tariffa_Cod() As Integer
            Get
                Return WParametriAgenda.Tariffa_Cod
            End Get
            Set(value As Integer)
                WParametriAgenda.Tariffa_Cod = value
                salva()
            End Set
        End Property

        Property LinkAgronicaAgenda2010() As String
            Get
                Return WParametriAgenda.LinkAgronicaAgenda2010
            End Get
            Set(ByVal Value As String)
                WParametriAgenda.LinkAgronicaAgenda2010 = Value
                salva()
            End Set
        End Property

        Property Raggruppamento_Cod() As String
            Get
                Return WParametriAgenda.Raggruppamento_Cod
            End Get
            Set(ByVal Value As String)
                WParametriAgenda.Raggruppamento_Cod = Value
                salva()
            End Set
        End Property

        Property Cod_Progetto() As String
            Get
                Return WParametriAgenda.Cod_Progetto
            End Get
            Set(ByVal Value As String)
                WParametriAgenda.Cod_Progetto = Value
                salva()
            End Set
        End Property

        Property StrGenericaXlinkGiasOnline() As String
            Get
                Return WParametriAgenda.StrGenericaxlinkGiasOnline
            End Get
            Set(ByVal Value As String)
                WParametriAgenda.StrGenericaxlinkGiasOnline = Value
                salva()
            End Set
        End Property

        Public Property RedirectUrl() As String
            Get
                Return WParametriAgenda.RedirectUrl
            End Get
            Set(value As String)
                WParametriAgenda.RedirectUrl = value

            End Set
        End Property

        Public Property PaginaDestinazione() As Integer
            Get
                Return WParametriAgenda.PaginaDestinazione
            End Get
            Set(value As Integer)
                WParametriAgenda.PaginaDestinazione = value
                salva()
            End Set
        End Property

        Public Property SitoDestinazione() As Integer
            Get
                Return WParametriAgenda.SitoDestinazioneFiltrino
            End Get
            Set(value As Integer)
                WParametriAgenda.SitoDestinazioneFiltrino = value
                salva()
            End Set
        End Property

        'Public Property CapiAnimali() As List(Of CapoAnimale)
        '    Get
        '        Return WParametriAgenda.CapiAnimali
        '    End Get
        '    Set(value As List(Of CapoAnimale))
        '        WParametriAgenda.CapiAnimali = value

        '    End Set
        'End Property

#End Region

    End Class

    Class WrappedParametriAgenda


        Private _TipoOperazioneAgenda As enum_Tipo_Operazione_Agenda
        Private _TargetOperazione As enum_Tipo_Operazione_Agenda_Target
        Private _TipoRicetta As enum_TipoRicetta

        Private _programmazione_Cod As Integer

        Private _TornaASitoOrigine As Boolean
        Private _SitoOrigine As Integer
        Private _PaginaSitoOrigine As Integer

        Private _Mac_Cod As Integer
        Private _Data As Date
        Private _RagSoc As String
        Private _SaNome As String
        Private _Piva As String
        Private _Sa_Cod As String
        Private _Lav_Cod As String
        Private _Lav_Des As String
        Private _Cau_Mov As String
        Private _Elem_Cod As String
        Private _Id_Agenda As String
        Private _Raccoglitore_Cod As String
        Private _Tipo_Operazione As String
        Private _Veg_Cod As String
        Private _Cul_Cod As String
        Private _Fabbricato As String
        Private _Disciplinare As String
        Private _FiltroRicerca As String
        Private _OperazioneMulticentro As Boolean
        Private _WS_Disciplinari_AgroWS_Disciplinari As String
        Private _WS_Fitofarmaci_AgroWS_Fitofarmaci As String

        Private _TipoOperazioneColturale As String
        Private _GruppoOperazioneColturale As String

        Private _Impianti As List(Of Impianto)
        Private _CapiAnimali As List(Of CapoAnimale)
        Private _Note As IList(Of Nota)
        Private _Movimenti As List(Of Movimento)

        Private _TrappolaUso As String
        Private _InstallazioneTrappola As ParametriInstallazioneTrappole
        Private _Rilievi As List(Of rilievoAvv)

        Private _QueryStringFiltrino As String
        Private _PaginaDestinazioneFiltrino As Integer
        Private _SitoDestinazioneFiltrino As Integer

        Private _Des_Lib As String

        Private _Lotto As String

        Private _Particelle As List(Of Particella)
        Private _ClassiTessitura As List(Of Integer)

        Private _Appezza As String
        Private _Id_Imp As String

        Private _Campo_Cod As String
        Private _Cod_Contatto As String

        Private _Piva_Origine As String

        Private _Qualifica_Cod As Integer
        Private _Tariffa_Cod As Integer

        Private _LinkAgronicaAgenda2010 As String
        Private _Raggruppamento_Cod As Integer
        Private _Cod_Progetto As Integer
        Private _StrGenericaxlinkGiasOnline As String

        Private _redirectUrl As String

        Private _PaginaDestinazione As Integer

        Private _SitoDestinazione As Integer

        Sub New()

            TipoOperazioneAgenda = enum_Tipo_Operazione_Agenda.QuadernoDiCampagna
            TargetOperazione = enum_Tipo_Operazione_Agenda_Target.Reale
            TipoRicetta = enum_TipoRicetta.Non_Filtrare

            _programmazione_Cod = 0

            _Data = Date.Now
            _TornaASitoOrigine = False
            _SitoOrigine = 0
            _PaginaSitoOrigine = 0
            _Piva = ""
            _RagSoc = ""
            _Sa_Cod = "0"
            _Lav_Cod = "0"
            _Lav_Des = ""
            _Cau_Mov = "0"
            _Elem_Cod = "0"
            _Id_Agenda = "0"
            _Raccoglitore_Cod = "0"
            _Veg_Cod = "-1"
            _Cul_Cod = "0"
            _Fabbricato = "0"
            _Disciplinare = "0"
            _Mac_Cod = 0
            _FiltroRicerca = ""
            _SaNome = ""
            _Lotto = "Indefinito"
            _OperazioneMulticentro = True
            _Impianti = New List(Of Impianto)
            _CapiAnimali = New List(Of CapoAnimale)
            _Note = New List(Of Nota)
            _Movimenti = New List(Of Movimento)
            _Rilievi = New List(Of rilievoAvv)

            _QueryStringFiltrino = ""
            _PaginaDestinazioneFiltrino = 0
            _SitoDestinazioneFiltrino = 0

            'separerò in oggetto che eredita, uno per ciascuna operazione
            _InstallazioneTrappola = Nothing
            _TrappolaUso = ""

            _TipoOperazioneColturale = ""
            _GruppoOperazioneColturale = "0"

            _Des_Lib = ""

            _Particelle = New List(Of Particella)
            _ClassiTessitura = New List(Of Integer)

            _Appezza = ""
            _Id_Imp = ""
            _Campo_Cod = ""
            _Cod_Contatto = ""

            _Piva_Origine = ""

            _Qualifica_Cod = 0
            _Tariffa_Cod = 0

            _Cod_Progetto = 0

            _LinkAgronicaAgenda2010 = ""

            _redirectUrl = ""

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

        Public Property TipoOperazioneAgenda() As enum_Tipo_Operazione_Agenda
            Get
                Return _TipoOperazioneAgenda
            End Get
            Set(value As enum_Tipo_Operazione_Agenda)
                _TipoOperazioneAgenda = value
            End Set
        End Property

        Property TargetOperazione() As enum_Tipo_Operazione_Agenda_Target
            Get
                Return _TargetOperazione
            End Get
            Set(value As enum_Tipo_Operazione_Agenda_Target)
                _TargetOperazione = value
            End Set
        End Property

        Property TipoRicetta() As enum_TipoRicetta
            Get
                Return _TipoRicetta
            End Get
            Set(value As enum_TipoRicetta)
                _TipoRicetta = value
            End Set
        End Property
        Public Property Programmazione_Cod As Integer
            Get
                Return _programmazione_Cod
            End Get
            Set(value As Integer)
                _programmazione_Cod = value
            End Set
        End Property

        Public Property Cau_Mov() As String
            Get
                Return _Cau_Mov
            End Get
            Set(value As String)
                _Cau_Mov = value
            End Set
        End Property

        Public Property Mac_Cod() As Integer
            Get
                Return _Mac_Cod
            End Get
            Set(value As Integer)
                _Mac_Cod = value
            End Set
        End Property




        Public Property Lav_Des() As String
            Get
                Return _Lav_Des
            End Get
            Set(value As String)
                _Lav_Des = value
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

        Property Movimenti() As List(Of Movimento)
            Get
                Return _Movimenti
            End Get
            Set(ByVal Value As List(Of Movimento))
                _Movimenti = Value
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
        Public Property TrappolaUso() As String
            Get
                Return _TrappolaUso
            End Get
            Set(value As String)
                _TrappolaUso = value
            End Set
        End Property
        Property WS_Fitofarmaci_AgroWS_Fitofarmaci() As String
            Get
                Return _WS_Fitofarmaci_AgroWS_Fitofarmaci
            End Get
            Set(ByVal Value As String)
                _WS_Fitofarmaci_AgroWS_Fitofarmaci = Value
            End Set
        End Property
        Property WS_Disciplinari_AgroWS_Disciplinari() As String
            Get
                Return _WS_Disciplinari_AgroWS_Disciplinari
            End Get
            Set(ByVal Value As String)
                _WS_Disciplinari_AgroWS_Disciplinari = Value
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
        Property Appezza() As String
            Get
                Return _Appezza
            End Get
            Set(ByVal Value As String)
                _Appezza = Value
            End Set
        End Property
        Property Id_Imp() As String
            Get
                Return _Id_Imp
            End Get
            Set(ByVal Value As String)
                _Id_Imp = Value
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
        Property Lav_Cod() As String
            Get
                Return _Lav_Cod
            End Get
            Set(ByVal Value As String)
                _Lav_Cod = Value
            End Set
        End Property
        Property Elem_Cod() As String
            Get
                Return _Elem_Cod
            End Get
            Set(ByVal Value As String)
                _Elem_Cod = Value
            End Set
        End Property
        Property Id_Agenda() As String
            Get
                Return _Id_Agenda
            End Get
            Set(ByVal Value As String)
                _Id_Agenda = Value
            End Set
        End Property

        Property Raccoglitore_Cod() As String
            Get
                Return _Raccoglitore_Cod
            End Get
            Set(ByVal Value As String)
                _Raccoglitore_Cod = Value
            End Set
        End Property

        Property Tipo_Operazione() As String
            Get
                Return _Tipo_Operazione
            End Get
            Set(ByVal Value As String)
                _Tipo_Operazione = Value
            End Set
        End Property
        Property Veg_Cod() As String
            Get
                Return _Veg_Cod
            End Get
            Set(ByVal Value As String)
                _Veg_Cod = Value
            End Set
        End Property
        Property Cul_Cod() As String
            Get
                Return _Cul_Cod
            End Get
            Set(ByVal Value As String)
                _Cul_Cod = Value
            End Set
        End Property
        Property Fabbricato() As String
            Get
                Return _Fabbricato
            End Get
            Set(ByVal Value As String)
                _Fabbricato = Value
            End Set
        End Property
        Property Disciplinare() As String
            Get
                If _Disciplinare = "" Then
                    Return "0"
                End If
                Return _Disciplinare
            End Get
            Set(ByVal Value As String)
                _Disciplinare = Value
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

        Property Impianti() As List(Of Impianto)
            Get
                Return _Impianti
            End Get
            Set(ByVal Value As List(Of Impianto))
                _Impianti = Value
            End Set
        End Property

        Property CapiAnimale() As List(Of CapoAnimale)
            Get
                Return _CapiAnimali
            End Get
            Set(ByVal Value As List(Of CapoAnimale))
                _CapiAnimali = Value
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


        Public Property OperazioneMulticentro() As Boolean
            Get
                Return _OperazioneMulticentro
            End Get
            Set(value As Boolean)
                _OperazioneMulticentro = value
            End Set
        End Property

        Public Property InstallazioneTrappola() As ParametriInstallazioneTrappole
            Get
                Return _InstallazioneTrappola
            End Get
            Set(value As ParametriInstallazioneTrappole)
                _InstallazioneTrappola = value
            End Set
        End Property


        Property Rilievi As List(Of rilievoAvv)
            Get
                Return _Rilievi
            End Get
            Set(value As List(Of rilievoAvv))
                _Rilievi = value
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

        Property TipoOperazioneColturale() As String
            Get
                Return _TipoOperazioneColturale
            End Get
            Set(ByVal Value As String)
                _TipoOperazioneColturale = Value
            End Set
        End Property
        Property GruppoOperazioneColturale() As String
            Get
                Return _GruppoOperazioneColturale
            End Get
            Set(ByVal Value As String)
                _GruppoOperazioneColturale = Value
            End Set
        End Property


        Public Property Des_lib As String
            Get
                Return _Des_Lib
            End Get
            Set(ByVal Value As String)
                _Des_Lib = Value
            End Set
        End Property

        Property Particelle As List(Of Particella)
            Get
                Return _Particelle
            End Get
            Set(value As List(Of Particella))
                _Particelle = value
            End Set
        End Property

        Property ClassiTessitura As List(Of Integer)
            Get
                Return _ClassiTessitura
            End Get
            Set(value As List(Of Integer))
                _ClassiTessitura = value
            End Set
        End Property

        Property Lotto() As String
            Get
                Return _Lotto
            End Get
            Set(ByVal Value As String)
                _Lotto = Value
            End Set
        End Property

        Public Property Cod_Contatto() As String
            Get
                Return _Cod_Contatto
            End Get
            Set(value As String)
                _Cod_Contatto = value
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

        Public Property Qualifica_Cod() As Integer
            Get
                Return _Qualifica_Cod
            End Get
            Set(value As Integer)
                _Qualifica_Cod = value
            End Set
        End Property

        Public Property Tariffa_Cod() As Integer
            Get
                Return _Tariffa_Cod
            End Get
            Set(value As Integer)
                _Tariffa_Cod = value
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

        Property Raggruppamento_Cod() As String
            Get
                Return _Raggruppamento_Cod
            End Get
            Set(ByVal Value As String)
                _Raggruppamento_Cod = Value
            End Set
        End Property

        Property Cod_Progetto() As String
            Get
                Return _Cod_Progetto
            End Get
            Set(ByVal Value As String)
                _Cod_Progetto = Value
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

        Public Property RedirectUrl() As String
            Get
                Return _redirectUrl
            End Get
            Set(value As String)
                _redirectUrl = value
            End Set
        End Property

        Public Property PaginaDestinazione() As Integer
            Get
                Return _PaginaDestinazione
            End Get
            Set(value As Integer)
                _PaginaDestinazione = value
            End Set
        End Property

        Public Property SitoDestinazione() As Integer
            Get
                Return _SitoDestinazione
            End Get
            Set(value As Integer)
                _SitoDestinazione = value
            End Set
        End Property

        'Public Property CapiAnimali() As List(Of CapoAnimale)
        '    Get
        '        Return _capiAnimale
        '    End Get
        '    Set(value As List(Of CapoAnimale))
        '        _capiAnimale = value
        '    End Set
        'End Property

#End Region



        Public Sub Svuota_DatiOperazione()

            ' Me._TipoOperazioneAgenda = enum_Tipo_Operazione_Agenda.QuadernoDiCampagna
            'NON RESETTARE PAGINA SITO ORIGINE!!
            'Me._SitoOrigine = "0"
            Me._Lav_Cod = "0"
            Me._Lav_Des = ""
            Me._Cau_Mov = "0"
            Me._Elem_Cod = "0"
            Me._Id_Agenda = "0"
            Me._Raccoglitore_Cod = "0"
            'Me._Veg_Cod = "-1"
            Me._Cul_Cod = "0"
            Me._Fabbricato = "0"
            Me._Disciplinare = "0"
            Me._Mac_Cod = 0
            Me._FiltroRicerca = ""
            Me._SaNome = ""
            Me._OperazioneMulticentro = True
            Me._Impianti = New List(Of Impianto)
            Me._CapiAnimali = New List(Of CapoAnimale)
            Me._Note = New List(Of Nota)
            Me._Movimenti = New List(Of Movimento)
            Me._InstallazioneTrappola = Nothing
            Me._TrappolaUso = ""
            Me._Rilievi = New List(Of rilievoAvv)
            Me._Elem_Cod = 0
            Me._Lotto = "Indefinito"
            'non svuotare questi perche sono usati per filtro pagina come specie e centro
            'Me.TipoOperazioneColturale = ""
            'Me.GruppoOperazioneColturale = "0"

            Me._Particelle = New List(Of Particella)
            Me._ClassiTessitura = New List(Of Integer)

            Me._Qualifica_Cod = 0
            Me._Tariffa_Cod = 0

        End Sub

    End Class

    Public Class Impianto

        Public Reale_Or_Planning As enum_Tipo_Operazione_Agenda_Target

        Public Programmazione_Entita_cod As Integer

        Public Piva As String
        Public Sa_Cod As Integer
        Public Appezza As Integer
        Public Campo_Cod As Integer
        Public ID_Reg As Integer

        Public App_Nome As String
        Public Campo_Des As String

        Public Progetto_Cod As Integer
        Public Sup_Imp As Decimal
        Public Validita_Inizio_Distinta As Date
        Public Validita_Fine_Distinta As Date
        Public Cul_Des As String
        Public Data_Raccolta As Date
        Public Data_Raccolta_Prevista As Date
        Public Data_Fioritura As Date
        Public Data_Fioritura_Prevista As Date

        Public Veg_Cod As Integer
        Public Cul_Cod As Integer
        Public Rag_Soc As String
        Public Sa_Nome As String
        Public Veg_Des As String

        Public Cop_Cod As Integer
        Public Grfi_Cod As Integer
        Public Validita_Inizio_Appezzamento As Date
        Public Validita_Fine_Appezzamento As Date

        Public Validita_Inizio As Date
        Public Validita_Fine As Date

        Private _Udm_Cod As Integer
        Private _Mat_cod As Integer

        Private _Qta As Decimal
        Private _Qta2 As Decimal

        Public _QuotaDistribuzione As Decimal

        Public Codici_Anagrafe_Des As String

        Public GisWkt As String = ""
        Public GisWktGps As String = ""
        Public GisWktSistemaRiferimento As String = ""
        Public GisTipoEntita_cod As Integer = 0
        Public GisLayerCod As Integer = 0

        Public DistBZ_CorpiIdrici As Decimal
        Public DistBZ_AreeResPub As Decimal
        Public DistBZ_Allevamenti As Decimal
        Public DistBZ_VegNatNonColt As Decimal
        Public SupBZ_Riduzione As Decimal 'capezzagna
        Private _Sup_Riduzione_BufferZone As Decimal
        Private _Perc_Riduzione_Deriva As Decimal

        Public ListaParticelle As List(Of Particella)
        Public ListaClassiTessitura As List(Of Integer)

        Public Stato_Impianto As Integer

        Property Mat_cod As Integer
            Get
                Return _Mat_cod
            End Get
            Set(value As Integer)
                _Mat_cod = value
            End Set
        End Property

        Property Udm_cod As Integer
            Get
                Return _Udm_Cod
            End Get
            Set(value As Integer)
                _Udm_Cod = value
            End Set
        End Property

        Property Sup_Riduzione_BufferZone() As String
            Get
                Return _Sup_Riduzione_BufferZone
            End Get
            Set(ByVal Value As String)
                _Sup_Riduzione_BufferZone = Value.Replace(".", ",")
            End Set
        End Property
        Property Perc_Riduzione_Deriva() As String
            Get
                Return _Perc_Riduzione_Deriva
            End Get
            Set(ByVal Value As String)
                _Perc_Riduzione_Deriva = Value.Replace(".", ",")
            End Set
        End Property

        Property Qta() As String
            Get
                Return _Qta
            End Get
            Set(ByVal Value As String)
                _Qta = Value.Replace(".", ",")
            End Set
        End Property
        Property Qta2() As String
            Get
                Return _Qta2
            End Get
            Set(ByVal Value As String)
                _Qta2 = Value.Replace(".", ",")
            End Set
        End Property

    End Class

    Public Class CapoAnimale
        Public Cod_Animale As Integer
    End Class
    Public Class ParametriInstallazioneTrappole

        'parametri per installazione trappole
        Private _NumeroTrappole As Integer
        Private _CodiceProdotto As Integer
        Private _CodiceDitta As Integer
        Private _Freatimetro As Decimal
        Private _SiglaAv As String
        Private _AvCod As Integer
        Private _NumeroInneschi As Integer
        'lista degli appezzamenti coinvolti nell'operazione agenda
        Private _Appezzamenti_X_NumTrappole As Hashtable
        'elenco trappele (numero-nome personalizzato)
        Private _Trappole_X_Nome As Hashtable
        'Coppia trappola - appezzamento che la contiene
        Private _Trappole_X_Appezzamento As Hashtable
        'Coppia innesco - trappola che la contiene che la contiene
        Private _Trappole_X_NumInneschi As Hashtable

        Sub New()
            'parametri per installazione trappole
            _NumeroTrappole = 0
            _CodiceProdotto = 0
            _CodiceDitta = 0
            _Freatimetro = 0
            _SiglaAv = 0
            _AvCod = 0
            _NumeroInneschi = 0
            'lista degli appezzamenti coinvolti nell'operazione agenda
            _Appezzamenti_X_NumTrappole = New Hashtable()
            'elenco trappele (numero-nome personalizzato)
            _Trappole_X_Nome = New Hashtable()
            'Coppia trappola - appezzamento che la contiene
            _Trappole_X_Appezzamento = New Hashtable()
            'Coppia innesco - trappola che la contiene che la contiene
            _Trappole_X_NumInneschi = New Hashtable()
        End Sub
        Public Property Appezzamenti_X_NumTrappole() As Hashtable
            Get
                Return _Appezzamenti_X_NumTrappole
            End Get
            Set(value As Hashtable)
                _Appezzamenti_X_NumTrappole = value
            End Set
        End Property
        Public Property AvCod() As Integer
            Get
                Return _AvCod
            End Get
            Set(value As Integer)
                _AvCod = value
            End Set
        End Property
        Public Property CodiceDitta() As Integer
            Get
                Return _CodiceDitta
            End Get
            Set(value As Integer)
                _CodiceDitta = value
            End Set
        End Property
        Public Property CodiceProdotto() As Integer
            Get
                Return _CodiceProdotto
            End Get
            Set(value As Integer)
                _CodiceProdotto = value
            End Set
        End Property
        Public Property Freatimetro() As Decimal
            Get
                Return _Freatimetro
            End Get
            Set(value As Decimal)
                _Freatimetro = value
            End Set
        End Property
        Public Property Trappole_X_NumInneschi() As Hashtable
            Get
                Return _Trappole_X_NumInneschi
            End Get
            Set(value As Hashtable)
                _Trappole_X_NumInneschi = value
            End Set
        End Property
        Public Property NumeroInneschi() As Integer
            Get
                Return _NumeroInneschi
            End Get
            Set(value As Integer)
                _NumeroInneschi = value
            End Set
        End Property
        Public Property NumeroTrappole() As Integer
            Get
                Return _NumeroTrappole
            End Get
            Set(value As Integer)
                _NumeroTrappole = value
            End Set
        End Property
        Public Property SiglaAv() As String
            Get
                Return _SiglaAv
            End Get
            Set(value As String)
                _SiglaAv = value
            End Set
        End Property
        Public Property Trappole_X_Appezzamento() As Hashtable
            Get
                Return _Trappole_X_Appezzamento
            End Get
            Set(value As Hashtable)
                _Trappole_X_Appezzamento = value
            End Set
        End Property
        Public Property Trappole_X_Nome() As Hashtable
            Get
                Return _Trappole_X_Nome
            End Get
            Set(value As Hashtable)
                _Trappole_X_Nome = value
            End Set
        End Property


    End Class

    Public Class rilievoAvv
        Private _mov_destinazioni_graphickey As String
        Private _av_cod As Integer
        Private _udm_cod As Integer
        Private _valore As String
        Private _lav_cod As String
        Private _impianto As New AgronicaCoreModello.ParametriAgenda_Temp.Impianto

        Public Property Av_cod() As Integer
            Get
                Return _av_cod
            End Get
            Set(value As Integer)
                _av_cod = value
            End Set
        End Property
        Public Property Impianto() As AgronicaCoreModello.ParametriAgenda_Temp.Impianto
            Get
                Return _impianto
            End Get
            Set(value As AgronicaCoreModello.ParametriAgenda_Temp.Impianto)
                _impianto = value
            End Set
        End Property
        Public Property Udm_cod() As Integer
            Get
                Return _udm_cod
            End Get
            Set(value As Integer)
                _udm_cod = value
            End Set
        End Property
        Public Property Valore() As String
            Get
                Return _valore
            End Get
            Set(value As String)
                _valore = value
            End Set
        End Property

        Public Property Lav_Cod() As Integer
            Get
                Return _lav_cod
            End Get
            Set(value As Integer)
                _lav_cod = value
            End Set
        End Property

        Public Property mov_destinazioni_graphickey As String
            Get
                Return _mov_destinazioni_graphickey
            End Get
            Set(ByVal Value As String)
                _mov_destinazioni_graphickey = Value
            End Set
        End Property


    End Class


    Public Class Particella

        Private _Part_Cod As Integer
        Private _Provincia As String
        Private _Comune As String
        Private _Sezione As String
        Private _Foglio As Integer
        Private _Numero As Integer
        Private _Subalterno As String

        Sub New()

            _Part_Cod = 0
            _Provincia = ""
            _Comune = ""
            _Sezione = ""
            _Foglio = 0
            _Numero = 0
            _Subalterno = ""

        End Sub

        Public Property Part_Cod() As Integer
            Get
                Return _Part_Cod
            End Get
            Set(ByVal value As Integer)
                _Part_Cod = value
            End Set
        End Property

        Public Property Foglio() As Integer
            Get
                Return _Foglio
            End Get
            Set(ByVal value As Integer)
                _Foglio = value
            End Set
        End Property

        Public Property Numero() As Integer
            Get
                Return _Numero
            End Get
            Set(ByVal value As Integer)
                _Numero = value
            End Set
        End Property

        Public Property Provincia() As String
            Get
                Return _Provincia
            End Get
            Set(ByVal value As String)
                _Provincia = value
            End Set
        End Property

        Public Property Comune() As String
            Get
                Return _Comune
            End Get
            Set(ByVal value As String)
                _Comune = value
            End Set
        End Property

        Public Property Sezione() As String
            Get
                Return _Sezione
            End Get
            Set(ByVal value As String)
                _Sezione = value
            End Set
        End Property

        Public Property Subalterno() As String
            Get
                Return _Subalterno
            End Get
            Set(ByVal value As String)
                _Subalterno = value
            End Set
        End Property

    End Class



End Namespace