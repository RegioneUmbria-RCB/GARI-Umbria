Imports System.Web
Imports System.Xml
Imports System.Data
Imports System.Text
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports Newtonsoft.Json
Imports AgronicaCoreUtility
Imports AgronicaCoreDTOStd.InData.FiltroRicerca
'#################################################################################################
''' <summary>
''' AGENDA 2010
''' </summary>
''' <remarks></remarks>
Public Class ParametriAgenda_2010

    Private _PaginaRichiesta As Integer
    Private _PaginaProvenienza As Integer
    Private _QueryStringFiltrino As String
    Private _PaginaDestinazioneFiltrino As Integer
    Private _SitoDestinazioneFiltrino As Integer
    Private _Piva As String

    Private _Sa_Cod As Integer
    Private _Campo_Cod As Integer
    Private _DataSelezionata As Date
    Private _Veg_Cod As Integer
    Private _Cul_Cod As Integer
    Private _Id_Agenda As Integer
    Private _Operazione As Integer
    Private _Lavorazione As Integer
    Private _Appezza As Integer
    Private _Id_Reg As Integer
    Private _Validita_Inizio As Date
    Private _Validita_Fine As Date
    Private _Fabbricato As String
    Private _OperazioneMagazzino As String
    Private _Id_Cod As Integer

    Private _Chiave As String
    Private _Mode As String

    Private _strVariabiliOpAgenda As String
    Private _Cod_risum As Integer
    Private _Cod_Contatto As String
    Private _Lav_Cod As Integer
    Private _GenericObj_string As String

    Private _TargetOperazione As enum_Tipo_Operazione_Agenda_Target
    Private _Programmazione_Cod As Integer
    Private _Tipo_Operazione As enum_TipoOperazioneDB
    Private _TipoOperazioneAgenda As enum_Tipo_Operazione_Agenda
    Private _TipoRicetta As enum_TipoRicetta

    Private _Impianti As List(Of Impianti_2010)

    Private _Sessione As Boolean
    Private _RedirectUrl As String
    Private _IdSezione As Integer

#Region "Proprietà"

    Public Property Sessione() As Boolean
        Get
            Return _Sessione
        End Get
        Set(value As Boolean)
            _Sessione = value
        End Set
    End Property

    Public Property StrVariabiliOpAgenda() As String
        Get
            Return _strVariabiliOpAgenda
        End Get
        Set(value As String)
            _strVariabiliOpAgenda = value
        End Set
    End Property

    Property Piva() As String
        Get
            Return _Piva
        End Get
        Set(ByVal Value As String)
            _Piva = Value
            Salva()
        End Set
    End Property

    Property OperazioneMagazzino() As String
        Get
            Return _OperazioneMagazzino
        End Get
        Set(ByVal Value As String)
            _OperazioneMagazzino = Value
            Salva()
        End Set
    End Property

    Property Fabbricato() As String
        Get
            Return _Fabbricato
        End Get
        Set(ByVal Value As String)
            _Fabbricato = Value
            Salva()
        End Set
    End Property

    Property PaginaRichiesta() As Integer
        Get
            Return _PaginaRichiesta
        End Get
        Set(ByVal Value As Integer)
            _PaginaRichiesta = Value
            Salva()
        End Set
    End Property

    Property PaginaProvenienza() As Integer
        Get
            Return _PaginaProvenienza
        End Get
        Set(ByVal Value As Integer)
            _PaginaProvenienza = Value
            Salva()
        End Set
    End Property

    Public Property Sa_Cod() As Integer
        Get
            Return _Sa_Cod
        End Get
        Set(ByVal value As Integer)
            _Sa_Cod = value
            Salva()
        End Set
    End Property

    Public Property Campo_Cod() As Integer
        Get
            Return _Campo_Cod
        End Get
        Set(ByVal value As Integer)
            _Campo_Cod = value
            Salva()
        End Set
    End Property

    Public Property DataSelezionata() As Date
        Get
            Return _DataSelezionata
        End Get
        Set(ByVal value As Date)
            _DataSelezionata = value
            Salva()
        End Set
    End Property

    Public Property Veg_Cod() As Integer
        Get
            Return _Veg_Cod
        End Get
        Set(ByVal value As Integer)
            _Veg_Cod = value
            Salva()
        End Set
    End Property

    Public Property Cul_Cod() As Integer
        Get
            Return _Cul_Cod
        End Get
        Set(ByVal value As Integer)
            _Cul_Cod = value
            Salva()
        End Set
    End Property

    Public Property Id_Agenda() As Integer
        Get
            Return _Id_Agenda
        End Get
        Set(ByVal value As Integer)
            _Id_Agenda = value
            Salva()
        End Set
    End Property

    Public Property Operazione() As Integer
        Get
            Return _Operazione
        End Get
        Set(ByVal value As Integer)
            _Operazione = value
            Salva()
        End Set
    End Property

    Public Property Lavorazione() As Integer
        Get
            Return _Lavorazione
        End Get
        Set(ByVal value As Integer)
            _Lavorazione = value
            Salva()
        End Set
    End Property

    Public Property Appezza() As Integer
        Get
            Return _Appezza
        End Get
        Set(ByVal value As Integer)
            _Appezza = value
        End Set
    End Property

    Public Property Id_Reg() As Integer
        Get
            Return _Id_Reg
        End Get
        Set(ByVal value As Integer)
            _Id_Reg = value
        End Set
    End Property

    Public Property Validita_Inizio() As Date
        Get
            Return _Validita_Inizio
        End Get
        Set(ByVal value As Date)
            _Validita_Inizio = value
        End Set
    End Property

    Public Property Validita_Fine() As Date
        Get
            Return _Validita_Fine
        End Get
        Set(ByVal value As Date)
            _Validita_Fine = value
        End Set
    End Property

    Public Property QueryStringFiltrino() As String
        Get
            Return _QueryStringFiltrino
        End Get
        Set(ByVal value As String)
            _QueryStringFiltrino = value
            Salva()
        End Set
    End Property

    Public Property PaginaDestinazioneFiltrino() As Integer
        Get
            Return _PaginaDestinazioneFiltrino
        End Get
        Set(ByVal value As Integer)
            _PaginaDestinazioneFiltrino = value
        End Set
    End Property

    Public Property SitoDestinazioneFiltrino() As Integer
        Get
            Return _SitoDestinazioneFiltrino
        End Get
        Set(ByVal value As Integer)
            _SitoDestinazioneFiltrino = value
        End Set
    End Property

    Public Property Chiave() As String
        Get
            Return _Chiave
        End Get
        Set(ByVal value As String)
            _Chiave = value
        End Set
    End Property

    Public Property Mode() As String
        Get
            Return _Mode
        End Get
        Set(ByVal value As String)
            _Mode = value
        End Set
    End Property

    Public Property Cod_risum() As Integer
        Get
            Return _Cod_risum
        End Get
        Set(ByVal value As Integer)
            _Cod_risum = value
            Salva()
        End Set
    End Property

    Public Property Cod_Contatto() As String
        Get
            Return _Cod_Contatto
        End Get
        Set(ByVal value As String)
            _Cod_Contatto = value
            Salva()
        End Set
    End Property

    Public Property Lav_Cod() As Integer
        Get
            Return _Lav_Cod
        End Get
        Set(ByVal value As Integer)
            _Lav_Cod = value
            Salva()
        End Set
    End Property

    Public Property GenericObj_string() As String
        Get
            If _GenericObj_string Is Nothing Then
                _GenericObj_string = ""
            End If
            Return AgroZip.DeCompressioneBase64(0, _GenericObj_string)
        End Get
        Set(ByVal value As String)
            If _GenericObj_string Is Nothing Then
                _GenericObj_string = ""
            End If
            _GenericObj_string = AgroZip.CompressioneBase64(0, value)
            Salva()
        End Set
    End Property

    Public Property Impianti() As List(Of Impianti_2010)
        Get
            Return _Impianti
        End Get
        Set(ByVal value As List(Of Impianti_2010))
            _Impianti = value
            Salva()
        End Set
    End Property

    Public Property Id_Cod() As Integer
        Get
            Return _Id_Cod
        End Get
        Set(ByVal value As Integer)
            _Id_Cod = value
            Salva()
        End Set
    End Property

    Public Property Tipo_Operazione() As enum_TipoOperazioneDB
        Get
            Return _Tipo_Operazione
        End Get
        Set(ByVal value As enum_TipoOperazioneDB)
            _Tipo_Operazione = value
            Salva()
        End Set
    End Property

    Public Property Programmazione_Cod() As Integer
        Get
            Return _Programmazione_Cod
        End Get
        Set(ByVal value As Integer)
            _Programmazione_Cod = value
            Salva()
        End Set
    End Property

    Public Property TargetOperazione() As enum_Tipo_Operazione_Agenda_Target
        Get
            Return _TargetOperazione
        End Get
        Set(ByVal value As enum_Tipo_Operazione_Agenda_Target)
            _TargetOperazione = value
            Salva()
        End Set
    End Property

    Public Property TipoOperazioneAgenda() As enum_Tipo_Operazione_Agenda
        Get
            Return _TipoOperazioneAgenda
        End Get
        Set(ByVal value As enum_Tipo_Operazione_Agenda)
            _TipoOperazioneAgenda = value
            Salva()
        End Set
    End Property

    Public Property TipoRicetta() As enum_TipoRicetta
        Get
            Return _TipoRicetta
        End Get
        Set(ByVal value As enum_TipoRicetta)
            _TipoRicetta = value
            Salva()
        End Set
    End Property

    Public Property RedirectUrl() As String
        Get
            Return _RedirectUrl
        End Get
        Set(ByVal value As String)
            _RedirectUrl = value
            Salva()
        End Set
    End Property

    Public Property IdSezione() As Integer
        Get
            Return _IdSezione
        End Get
        Set(ByVal value As Integer)
            _IdSezione = value
            Salva()
        End Set
    End Property

#End Region

#Region "X Sessione"
    Public Sub Salva()
        If _Sessione Then
            HttpContext.Current.Session("ParametriAgenda_2010") = Me
        End If
    End Sub

    Public Sub Leggi()
        If _Sessione Then
            Dim obj As New ParametriAgenda_2010
            obj = HttpContext.Current.Session("ParametriAgenda_2010")

            If Not IsNothing(obj) Then

                _PaginaRichiesta = obj._PaginaRichiesta
                _Piva = obj._Piva
                _PaginaProvenienza = obj._PaginaProvenienza
                _QueryStringFiltrino = obj._QueryStringFiltrino
                _SitoDestinazioneFiltrino = obj._SitoDestinazioneFiltrino
                _PaginaDestinazioneFiltrino = obj._PaginaDestinazioneFiltrino
                _Sa_Cod = obj._Sa_Cod
                _Campo_Cod = obj._Campo_Cod
                _Fabbricato = obj._Fabbricato
                _DataSelezionata = obj._DataSelezionata
                _Fabbricato = obj._Fabbricato

                _Veg_Cod = obj._Veg_Cod
                _Cul_Cod = obj._Cul_Cod
                _Id_Agenda = obj._Id_Agenda
                _Operazione = obj._Operazione
                _Lavorazione = obj._Lavorazione
                _Validita_Inizio = obj._Validita_Inizio
                _Validita_Fine = obj._Validita_Fine
                _Id_Cod = obj.Id_Cod

                _Appezza = obj.Appezza
                _Id_Reg = obj._Id_Reg
                _OperazioneMagazzino = obj._OperazioneMagazzino

                _Chiave = obj._Chiave
                _Mode = obj._Mode
                _Cod_risum = obj._Cod_risum
                _Cod_Contatto = obj._Cod_Contatto
                _Lav_Cod = obj._Lav_Cod
                _GenericObj_string = obj._GenericObj_string
                _Impianti = obj._Impianti


                _TargetOperazione = obj.TargetOperazione
                _Programmazione_Cod = obj.Programmazione_Cod
                _Tipo_Operazione = obj.Tipo_Operazione
                _TipoOperazioneAgenda = obj.TipoOperazioneAgenda
                _TipoRicetta = obj.TipoRicetta
                _RedirectUrl = obj.RedirectUrl
                _IdSezione = obj.IdSezione
            End If

        End If
    End Sub
#End Region

#Region "Costruttori"
    Public Sub New()
        _Sessione = True
        _Validita_Inizio = AGRODATAINIZIO
        _Validita_Fine = AGRODATAFINE
    End Sub

    Sub New(ByVal XML As String)
        _Sessione = True
        LeggiXML(XML)
    End Sub

    Sub New(ByVal flagSessione As Boolean)
        _Sessione = flagSessione
    End Sub

    Private Sub LeggiXML(ByVal XML As String)
        Dim XmlDoc As New System.Xml.XmlDocument
        XmlDoc.LoadXml(XML)

        Dim XML_ParametriAggiuntivi As System.Xml.XmlElement
        XML_ParametriAggiuntivi = XmlDoc.SelectSingleNode("Parametri").SelectSingleNode("ParametriAgenda_2010")


        If XML_ParametriAggiuntivi.HasAttribute(LCase("piva")) = True Then
            Me.Piva = XML_ParametriAggiuntivi.GetAttribute(LCase("piva"))
        End If

        If XML_ParametriAggiuntivi.HasAttribute(LCase("operazionemagazzino")) = True Then
            Me.OperazioneMagazzino = XML_ParametriAggiuntivi.GetAttribute(LCase("operazionemagazzino"))
        End If

        If XML_ParametriAggiuntivi.HasAttribute(LCase("fabbricato")) = True Then
            Me.Fabbricato = XML_ParametriAggiuntivi.GetAttribute(LCase("fabbricato"))
        End If

        If XML_ParametriAggiuntivi.HasAttribute(LCase("pagina_richiesta")) = True Then
            Me.PaginaRichiesta = XML_ParametriAggiuntivi.GetAttribute(LCase("pagina_richiesta"))
        End If

        If XML_ParametriAggiuntivi.HasAttribute(LCase("pagina_provenienza")) = True Then
            Me.PaginaProvenienza = XML_ParametriAggiuntivi.GetAttribute(LCase("pagina_provenienza"))
        End If

        If XML_ParametriAggiuntivi.HasAttribute(LCase("querystringfiltrino")) = True Then
            Me.QueryStringFiltrino = XML_ParametriAggiuntivi.GetAttribute(LCase("querystringfiltrino"))
        End If

        If XML_ParametriAggiuntivi.HasAttribute(LCase("paginadestinazionefiltrino")) = True Then
            Me.PaginaDestinazioneFiltrino = XML_ParametriAggiuntivi.GetAttribute(LCase("paginadestinazionefiltrino"))
        End If

        If XML_ParametriAggiuntivi.HasAttribute(LCase("sitodestinazionefiltrino")) = True Then
            Me.SitoDestinazioneFiltrino = XML_ParametriAggiuntivi.GetAttribute(LCase("sitodestinazionefiltrino"))
        End If

        If XML_ParametriAggiuntivi.HasAttribute(LCase("sa_cod")) = True Then
            Me.Sa_Cod = XML_ParametriAggiuntivi.GetAttribute(LCase("sa_cod"))
        End If

        If XML_ParametriAggiuntivi.HasAttribute(LCase("campo_cod")) = True Then
            Me.Campo_Cod = XML_ParametriAggiuntivi.GetAttribute(LCase("campo_cod"))
        End If

        If XML_ParametriAggiuntivi.HasAttribute(LCase("dataselezionata")) = True Then
            If IsDate(XML_ParametriAggiuntivi.GetAttribute(LCase("dataselezionata"))) = True Then
                Me.DataSelezionata = XML_ParametriAggiuntivi.GetAttribute(LCase("dataselezionata"))
            End If
        End If
        If XML_ParametriAggiuntivi.HasAttribute(LCase("veg_cod")) = True Then
            Me.Veg_Cod = XML_ParametriAggiuntivi.GetAttribute(LCase("veg_cod"))
        End If

        If XML_ParametriAggiuntivi.HasAttribute(LCase("cul_cod")) = True Then
            Me.Cul_Cod = XML_ParametriAggiuntivi.GetAttribute(LCase("cul_cod"))
        End If

        If XML_ParametriAggiuntivi.HasAttribute(LCase("id_cod")) = True Then
            Me.Id_Cod = XML_ParametriAggiuntivi.GetAttribute(LCase("id_cod"))
        End If

        If XML_ParametriAggiuntivi.HasAttribute(LCase("id_agenda")) = True Then
            Me.Id_Agenda = XML_ParametriAggiuntivi.GetAttribute(LCase("id_agenda"))
        End If
        If XML_ParametriAggiuntivi.HasAttribute(LCase("operazione")) = True Then
            Me.Operazione = XML_ParametriAggiuntivi.GetAttribute(LCase("operazione"))
        End If
        If XML_ParametriAggiuntivi.HasAttribute(LCase("lavorazione")) = True Then
            Me.Lavorazione = XML_ParametriAggiuntivi.GetAttribute(LCase("lavorazione"))
        End If
        If XML_ParametriAggiuntivi.HasAttribute(LCase("appezza")) = True Then
            Me.Appezza = XML_ParametriAggiuntivi.GetAttribute(LCase("appezza"))
        End If
        If XML_ParametriAggiuntivi.HasAttribute(LCase("id_Reg")) = True Then
            Me.Id_Reg = XML_ParametriAggiuntivi.GetAttribute(LCase("id_Reg"))
        End If
        If XML_ParametriAggiuntivi.HasAttribute(LCase("chiave")) = True Then
            Me.Chiave = XML_ParametriAggiuntivi.GetAttribute(LCase("chiave"))
        End If
        If XML_ParametriAggiuntivi.HasAttribute(LCase("mode")) = True Then
            Me.Mode = XML_ParametriAggiuntivi.GetAttribute(LCase("mode"))
        End If
        If XML_ParametriAggiuntivi.HasAttribute(LCase("cod_risum")) = True Then
            Me.Cod_risum = XML_ParametriAggiuntivi.GetAttribute(LCase("cod_risum"))
        End If
        If XML_ParametriAggiuntivi.HasAttribute(LCase("cod_contatto")) = True Then
            Me.Cod_Contatto = XML_ParametriAggiuntivi.GetAttribute(LCase("cod_contatto"))
        End If
        If XML_ParametriAggiuntivi.HasAttribute(LCase("lav_cod")) = True Then
            Me.Lav_Cod = XML_ParametriAggiuntivi.GetAttribute(LCase("lav_cod"))
        End If
        If XML_ParametriAggiuntivi.HasAttribute("genericObj_string") = True Then
            Me.GenericObj_string = XML_ParametriAggiuntivi.GetAttribute("genericObj_string")
        End If


        If XML_ParametriAggiuntivi.HasAttribute(LCase("TargetOperazione")) = True Then
            Me.TargetOperazione = XML_ParametriAggiuntivi.GetAttribute(LCase("TargetOperazione"))
        End If
        If XML_ParametriAggiuntivi.HasAttribute(LCase("TipoOperazioneAgenda")) = True Then
            Me.TipoOperazioneAgenda = XML_ParametriAggiuntivi.GetAttribute(LCase("TipoOperazioneAgenda"))
        End If
        If XML_ParametriAggiuntivi.HasAttribute(LCase("Programmazione_Cod")) = True Then
            Me.Programmazione_Cod = XML_ParametriAggiuntivi.GetAttribute(LCase("Programmazione_Cod"))
        End If
        If XML_ParametriAggiuntivi.HasAttribute(LCase("Tipo_Operazione")) = True Then
            Me.Tipo_Operazione = XML_ParametriAggiuntivi.GetAttribute(LCase("Tipo_Operazione"))
        End If
        If XML_ParametriAggiuntivi.HasAttribute(LCase("TipoRicetta")) = True Then
            Me.TipoRicetta = XML_ParametriAggiuntivi.GetAttribute(LCase("TipoRicetta"))
        End If

        Dim XML_Impianti As System.Xml.XmlElement
        XML_Impianti = XML_ParametriAggiuntivi.SelectSingleNode("impianti")
        If XML_Impianti IsNot Nothing Then
            Dim Impianti As New List(Of Impianti_2010)
            Dim List_Xml_Impianti As System.Xml.XmlNodeList
            List_Xml_Impianti = XML_Impianti.GetElementsByTagName("impianto")
            If List_Xml_Impianti IsNot Nothing Then
                For i = 0 To List_Xml_Impianti.Count - 1
                    Dim XML_Impianto As System.Xml.XmlElement = List_Xml_Impianti(i)
                    Dim impianto As New Impianti_2010
                    impianto.Piva = XML_Impianto.GetAttribute("piva")
                    impianto.Sa_Cod = XML_Impianto.GetAttribute("sa_cod")
                    impianto.Appezza = XML_Impianto.GetAttribute("appezza")
                    impianto.Id_Reg = XML_Impianto.GetAttribute("id_reg")
                    impianto.Progetto_Cod = XML_Impianto.GetAttribute("progetto_cod")
                    impianto.veg_cod = XML_Impianto.GetAttribute("veg_cod")
                    impianto.id_cod = XML_Impianto.GetAttribute("id_cod")
                    Impianti.Add(impianto)
                Next
            End If
            Me.Impianti = Impianti
        End If

        If XML_ParametriAggiuntivi.HasAttribute(LCase("RedirectUrl")) = True Then
            Me.RedirectUrl = XML_ParametriAggiuntivi.GetAttribute(LCase("RedirectUrl"))
        End If

        Me.IdSezione = 0
        If XML_ParametriAggiuntivi.HasAttribute(LCase("IdSezione")) = True Then
            If IsNumeric(XML_ParametriAggiuntivi.GetAttribute(LCase("IdSezione"))) Then
                Me.IdSezione = CInt(XML_ParametriAggiuntivi.GetAttribute(LCase("IdSezione")))
            End If
        End If

    End Sub

#End Region


    ''' <summary>
    ''' Genera il blocco XML VariabiliAnalisi con all'interno 
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GeneraXML() As String
        Dim XmlDoc As New System.Xml.XmlDocument
        Dim Xml_VariabiliAgenda As System.Xml.XmlElement
        'Creo il nodo "FiltroStampa"

        Xml_VariabiliAgenda = XmlDoc.CreateElement("ParametriAgenda_2010")
        Xml_VariabiliAgenda.SetAttribute("pagina_richiesta", PaginaRichiesta)
        Xml_VariabiliAgenda.SetAttribute("pagina_provenienza", PaginaProvenienza)
        Xml_VariabiliAgenda.SetAttribute("querystringfiltrino", QueryStringFiltrino)
        Xml_VariabiliAgenda.SetAttribute("sitodestinazionefiltrino", SitoDestinazioneFiltrino)
        Xml_VariabiliAgenda.SetAttribute("paginadestinazionefiltrino", PaginaDestinazioneFiltrino)
        Xml_VariabiliAgenda.SetAttribute("piva", Piva)
        Xml_VariabiliAgenda.SetAttribute("fabbricato", Fabbricato)

        Xml_VariabiliAgenda.SetAttribute("operazionemagazzino", OperazioneMagazzino)

        Xml_VariabiliAgenda.SetAttribute("sa_cod", Sa_Cod)
        Xml_VariabiliAgenda.SetAttribute("campo_cod", Campo_Cod)
        Xml_VariabiliAgenda.SetAttribute("dataselezionata", DataSelezionata)
        Xml_VariabiliAgenda.SetAttribute("veg_cod", Veg_Cod)
        Xml_VariabiliAgenda.SetAttribute("cul_cod", Cul_Cod)
        Xml_VariabiliAgenda.SetAttribute("id_cod", Id_Cod)
        Xml_VariabiliAgenda.SetAttribute("id_agenda", Id_Agenda)

        Xml_VariabiliAgenda.SetAttribute("id_agenda", Id_Agenda)
        Xml_VariabiliAgenda.SetAttribute("operazione", Operazione)
        Xml_VariabiliAgenda.SetAttribute("lavorazione", Lavorazione)

        Xml_VariabiliAgenda.SetAttribute("appezza", Appezza)
        Xml_VariabiliAgenda.SetAttribute("id_reg", Id_Reg)

        Xml_VariabiliAgenda.SetAttribute("chiave", Chiave)
        Xml_VariabiliAgenda.SetAttribute("mode", Mode)

        Xml_VariabiliAgenda.SetAttribute("cod_risum", Cod_risum)
        Xml_VariabiliAgenda.SetAttribute("cod_contatto", Cod_Contatto)
        Xml_VariabiliAgenda.SetAttribute("lav_cod", Lav_Cod)
        Xml_VariabiliAgenda.SetAttribute("genericObj_string", GenericObj_string)

        Xml_VariabiliAgenda.SetAttribute(LCase("TipoOperazioneAgenda"), CStr(TipoOperazioneAgenda))
        Xml_VariabiliAgenda.SetAttribute(LCase("TargetOperazione"), CStr(TargetOperazione))
        Xml_VariabiliAgenda.SetAttribute(LCase("Programmazione_Cod"), CStr(Programmazione_Cod))
        Xml_VariabiliAgenda.SetAttribute(LCase("Tipo_Operazione"), CStr(Tipo_Operazione))
        Xml_VariabiliAgenda.SetAttribute(LCase("TipoRicetta"), CStr(TipoRicetta))
        Xml_VariabiliAgenda.SetAttribute(LCase("RedirectUrl"), CStr(RedirectUrl))
        Xml_VariabiliAgenda.SetAttribute(LCase("IdSezione"), IdSezione)

        Dim Xml_Impianti As System.Xml.XmlElement
        Xml_Impianti = XmlDoc.CreateElement("impianti")
        If Impianti IsNot Nothing Then
            For Each impianto In Impianti
                Dim Xml_Impianto As System.Xml.XmlElement
                Xml_Impianto = XmlDoc.CreateElement("impianto")
                Xml_Impianto.SetAttribute("piva", impianto.Piva)
                Xml_Impianto.SetAttribute("sa_cod", impianto.Sa_Cod)
                Xml_Impianto.SetAttribute("appezza", impianto.Appezza)
                Xml_Impianto.SetAttribute("id_reg", impianto.Id_Reg)
                Xml_Impianto.SetAttribute("progetto_cod", impianto.Progetto_Cod)
                Xml_Impianto.SetAttribute("veg_cod", impianto.veg_cod)
                Xml_Impianto.SetAttribute("id_cod", impianto.id_cod)
                Xml_Impianti.AppendChild(Xml_Impianto)
            Next
        End If
        Xml_VariabiliAgenda.AppendChild(Xml_Impianti)

        XmlDoc.AppendChild(Xml_VariabiliAgenda)
        Dim str As String
        str = XmlDoc.InnerXml
        str = str.Replace("'", "&#39;")
        Return str
    End Function

End Class



''' <summary>
''' ParametriPlanning 
''' </summary>
''' <remarks></remarks>
Public Class ParametriPlanning

    Private _PaginaRichiesta As Integer
    Private _PaginaProvenienza As Integer
    Private _Piva As String
    Private _Cuaa As String
    Private _RagSoc As String = ""
    Private _ParametriAggiuntivi As String = ""

#Region "Proprietà"
    Property Piva() As String
        Get
            Return _Piva
        End Get
        Set(ByVal Value As String)
            _Piva = Value
            Salva()
        End Set
    End Property
    Property Cuaa() As String
        Get
            Return _Cuaa
        End Get
        Set(ByVal Value As String)
            _Cuaa = Value
            Salva()
        End Set
    End Property
    Property PaginaRichiesta() As Integer
        Get
            Return _PaginaRichiesta
        End Get
        Set(ByVal Value As Integer)
            _PaginaRichiesta = Value
            Salva()
        End Set
    End Property

    Property PaginaProvenienza() As Integer
        Get
            Return _PaginaProvenienza
        End Get
        Set(ByVal Value As Integer)
            _PaginaProvenienza = Value
            Salva()
        End Set
    End Property

    Public Property RagSoc() As String
        Get
            Return _RagSoc
        End Get
        Set(ByVal value As String)
            _RagSoc = value
            Salva()
        End Set
    End Property

    Public Property ParametriAggiuntivi() As String
        Get
            Return _ParametriAggiuntivi
        End Get
        Set(ByVal value As String)
            _ParametriAggiuntivi = value
            Salva()
        End Set
    End Property

#End Region

#Region "X Sessione"
    Public Sub Salva()
        HttpContext.Current.Session("ParametriPlanning") = Me
    End Sub
    Public Sub Leggi()
        Dim obj As New ParametriPlanning
        obj = HttpContext.Current.Session("ParametriPlanning")
        If Not IsNothing(obj) Then
            _PaginaRichiesta = obj._PaginaRichiesta
            _PaginaProvenienza = obj._PaginaProvenienza
            _Piva = obj.Piva
            _Cuaa = obj.Cuaa
            _RagSoc = obj.RagSoc
            _ParametriAggiuntivi = obj.ParametriAggiuntivi
        End If
    End Sub
#End Region

#Region "Costruttori"
    Public Sub New()

    End Sub

    Sub New(ByVal XML As String)
        LeggiXML(XML)
    End Sub

    Private Sub LeggiXML(ByVal XML As String)
        Dim XmlDoc As New System.Xml.XmlDocument
        XmlDoc.LoadXml(XML)

        Dim XML_ParametriAggiuntivi As System.Xml.XmlElement
        XML_ParametriAggiuntivi = XmlDoc.SelectSingleNode("Parametri").SelectSingleNode("ParametriPlanning")


        If XML_ParametriAggiuntivi.HasAttribute(LCase("paginaplanningrichiesta")) = True Then
            Me.PaginaRichiesta = XML_ParametriAggiuntivi.GetAttribute(LCase("paginaplanningrichiesta"))
        End If

        If XML_ParametriAggiuntivi.HasAttribute(LCase("pagina_provenienza")) = True Then
            Me.PaginaProvenienza = XML_ParametriAggiuntivi.GetAttribute(LCase("pagina_provenienza"))
        End If

        If XML_ParametriAggiuntivi.HasAttribute(LCase("piva")) = True Then
            Me.Piva = XML_ParametriAggiuntivi.GetAttribute(LCase("piva"))
        End If
        If XML_ParametriAggiuntivi.HasAttribute(LCase("cuaa")) = True Then
            Me.Cuaa = XML_ParametriAggiuntivi.GetAttribute(LCase("cuaa"))
        End If

        If XML_ParametriAggiuntivi.HasAttribute(LCase("ragsoc")) = True Then
            Me.RagSoc = XML_ParametriAggiuntivi.GetAttribute(LCase("ragsoc"))
        End If

        If XML_ParametriAggiuntivi.HasAttribute(LCase("parametri_aggiuntivi")) = True Then
            Me.ParametriAggiuntivi = XML_ParametriAggiuntivi.GetAttribute(LCase("parametri_aggiuntivi"))
        End If

    End Sub

#End Region


    ''' <summary>
    ''' Genera il blocco XML VariabiliAnalisi con all'interno 
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GeneraXML() As String
        Dim XmlDoc As New System.Xml.XmlDocument
        Dim Xml_VariabiliAgenda As System.Xml.XmlElement
        'Creo il nodo "FiltroStampa"

        Xml_VariabiliAgenda = XmlDoc.CreateElement("ParametriPlanning")
        Xml_VariabiliAgenda.SetAttribute("paginaplanningrichiesta", PaginaRichiesta)
        Xml_VariabiliAgenda.SetAttribute("pagina_provenienza", PaginaProvenienza)
        Xml_VariabiliAgenda.SetAttribute("piva", Piva)
        Xml_VariabiliAgenda.SetAttribute("cuaa", Cuaa)
        Xml_VariabiliAgenda.SetAttribute("ragsoc", RagSoc)
        Xml_VariabiliAgenda.SetAttribute("parametri_aggiuntivi", ParametriAggiuntivi)

        XmlDoc.AppendChild(Xml_VariabiliAgenda)
        Dim str As String
        str = XmlDoc.InnerXml
        str = str.Replace("'", "&#39;")
        Return str
    End Function

End Class


''' <summary>
''' ParametriPlanning 
''' </summary>
''' <remarks></remarks>
Public Class ParametriDomandaIrrigua

    Private _PaginaRichiesta As Integer
    Private _PaginaProvenienza As Integer
    Private _Piva As String
    Private _DataSelezionata As Date

    ' Campi aggiunti per supportare il porting delle pagine operazione su AgronicaDomandaIrrigua
    ' (Trattamenti_2, Installazione_Trappole, Reinnesco_Rilievi_Trappole, RilieviBS, Raccolta,
    ' Trattamenti_PostRaccolta, Distribuzione_Insetti). Permettono di trasportare in un singolo
    ' oggetto tutti i parametri della richiesta cross-site, evitando di affiancare un blocco
    ' ParametriAgenda_2010 nello stesso XML e i conseguenti conflitti di parsing/precedenza.
    Private _Lav_Cod As Integer
    Private _Id_Agenda As Integer
    Private _Sa_Cod As Integer
    Private _Veg_Cod As String
    Private _Appezza As Integer
    Private _Id_Reg As Integer
    Private _TipoOperazioneAgenda As enum_Tipo_Operazione_Agenda
    Private _TargetOperazione As enum_Tipo_Operazione_Agenda_Target
    Private _Programmazione_Cod As Integer
    Private _TipoRicetta As enum_TipoRicetta
    Private _Tipo_Operazione As enum_TipoOperazioneDB
    Private _RedirectUrl As String
    Private _QueryStringFiltrino As String

#Region "Proprietà"
    Property Piva() As String
        Get
            Return _Piva
        End Get
        Set(ByVal Value As String)
            _Piva = Value
            Salva()
        End Set
    End Property
    Property PaginaRichiesta() As Integer
        Get
            Return _PaginaRichiesta
        End Get
        Set(ByVal Value As Integer)
            _PaginaRichiesta = Value
            Salva()
        End Set
    End Property

    Property PaginaProvenienza() As Integer
        Get
            Return _PaginaProvenienza
        End Get
        Set(ByVal Value As Integer)
            _PaginaProvenienza = Value
            Salva()
        End Set
    End Property
    Property DataSelezionata() As Date
        Get
            Return _DataSelezionata
        End Get
        Set(ByVal Value As Date)
            _DataSelezionata = Value
            Salva()
        End Set
    End Property

    Property Lav_Cod() As Integer
        Get
            Return _Lav_Cod
        End Get
        Set(ByVal Value As Integer)
            _Lav_Cod = Value
            Salva()
        End Set
    End Property

    Property Id_Agenda() As Integer
        Get
            Return _Id_Agenda
        End Get
        Set(ByVal Value As Integer)
            _Id_Agenda = Value
            Salva()
        End Set
    End Property

    Property Sa_Cod() As Integer
        Get
            Return _Sa_Cod
        End Get
        Set(ByVal Value As Integer)
            _Sa_Cod = Value
            Salva()
        End Set
    End Property

    Property Veg_Cod() As String
        Get
            Return _Veg_Cod
        End Get
        Set(ByVal Value As String)
            _Veg_Cod = Value
            Salva()
        End Set
    End Property

    Property Appezza() As Integer
        Get
            Return _Appezza
        End Get
        Set(ByVal Value As Integer)
            _Appezza = Value
            Salva()
        End Set
    End Property

    Property Id_Reg() As Integer
        Get
            Return _Id_Reg
        End Get
        Set(ByVal Value As Integer)
            _Id_Reg = Value
            Salva()
        End Set
    End Property

    Property TipoOperazioneAgenda() As enum_Tipo_Operazione_Agenda
        Get
            Return _TipoOperazioneAgenda
        End Get
        Set(ByVal Value As enum_Tipo_Operazione_Agenda)
            _TipoOperazioneAgenda = Value
            Salva()
        End Set
    End Property

    Property TargetOperazione() As enum_Tipo_Operazione_Agenda_Target
        Get
            Return _TargetOperazione
        End Get
        Set(ByVal Value As enum_Tipo_Operazione_Agenda_Target)
            _TargetOperazione = Value
            Salva()
        End Set
    End Property

    Property Programmazione_Cod() As Integer
        Get
            Return _Programmazione_Cod
        End Get
        Set(ByVal Value As Integer)
            _Programmazione_Cod = Value
            Salva()
        End Set
    End Property

    Property TipoRicetta() As enum_TipoRicetta
        Get
            Return _TipoRicetta
        End Get
        Set(ByVal Value As enum_TipoRicetta)
            _TipoRicetta = Value
            Salva()
        End Set
    End Property

    Property Tipo_Operazione() As enum_TipoOperazioneDB
        Get
            Return _Tipo_Operazione
        End Get
        Set(ByVal Value As enum_TipoOperazioneDB)
            _Tipo_Operazione = Value
            Salva()
        End Set
    End Property

    Property RedirectUrl() As String
        Get
            Return _RedirectUrl
        End Get
        Set(ByVal Value As String)
            _RedirectUrl = Value
            Salva()
        End Set
    End Property

    Property QueryStringFiltrino() As String
        Get
            Return _QueryStringFiltrino
        End Get
        Set(ByVal Value As String)
            _QueryStringFiltrino = Value
            Salva()
        End Set
    End Property

#End Region

#Region "X Sessione"
    Public Sub Salva()
        HttpContext.Current.Session("ParametriDomandaIrrigua") = Me
    End Sub
    Public Sub Leggi()
        Dim obj As New ParametriDomandaIrrigua
        obj = HttpContext.Current.Session("ParametriDomandaIrrigua")
        If Not IsNothing(obj) Then
            _PaginaRichiesta = obj._PaginaRichiesta
            _PaginaProvenienza = obj._PaginaProvenienza
            _Piva = obj.Piva
            _DataSelezionata = obj.DataSelezionata
            _Lav_Cod = obj._Lav_Cod
            _Id_Agenda = obj._Id_Agenda
            _Sa_Cod = obj._Sa_Cod
            _Veg_Cod = obj._Veg_Cod
            _Appezza = obj._Appezza
            _Id_Reg = obj._Id_Reg
            _TipoOperazioneAgenda = obj._TipoOperazioneAgenda
            _TargetOperazione = obj._TargetOperazione
            _Programmazione_Cod = obj._Programmazione_Cod
            _TipoRicetta = obj._TipoRicetta
            _Tipo_Operazione = obj._Tipo_Operazione
            _RedirectUrl = obj._RedirectUrl
            _QueryStringFiltrino = obj._QueryStringFiltrino
        End If
    End Sub
#End Region

#Region "Costruttori"
    Public Sub New()

    End Sub

    Sub New(ByVal XML As String)
        LeggiXML(XML)
    End Sub

    Private Sub LeggiXML(ByVal XML As String)
        Dim XmlDoc As New System.Xml.XmlDocument
        XmlDoc.LoadXml(XML)

        Dim XML_ParametriAggiuntivi As System.Xml.XmlElement
        XML_ParametriAggiuntivi = XmlDoc.SelectSingleNode("Parametri").SelectSingleNode("ParametriDomandaIrrigua")


        If XML_ParametriAggiuntivi.HasAttribute(LCase("pagina_richiesta")) = True Then
            Me.PaginaRichiesta = XML_ParametriAggiuntivi.GetAttribute(LCase("pagina_richiesta"))
        End If

        If XML_ParametriAggiuntivi.HasAttribute(LCase("pagina_provenienza")) = True Then
            Me.PaginaProvenienza = XML_ParametriAggiuntivi.GetAttribute(LCase("pagina_provenienza"))
        End If

        If XML_ParametriAggiuntivi.HasAttribute(LCase("piva")) = True Then
            Me.Piva = XML_ParametriAggiuntivi.GetAttribute(LCase("piva"))
        End If

        If XML_ParametriAggiuntivi.HasAttribute(LCase("dataselezionata")) = True Then
            If IsDate(XML_ParametriAggiuntivi.GetAttribute(LCase("dataselezionata"))) = True Then
                Me.DataSelezionata = XML_ParametriAggiuntivi.GetAttribute(LCase("dataselezionata"))
            End If
        End If

        ' Attributi operazione (porting Trattamenti_2, ecc.). HasAttribute mantiene la retro-compatibilità:
        ' XML vecchi senza questi attributi continuano a leggersi senza errori, i campi restano a default.
        If XML_ParametriAggiuntivi.HasAttribute(LCase("lav_cod")) AndAlso IsNumeric(XML_ParametriAggiuntivi.GetAttribute(LCase("lav_cod"))) Then
            Me.Lav_Cod = CInt(XML_ParametriAggiuntivi.GetAttribute(LCase("lav_cod")))
        End If

        If XML_ParametriAggiuntivi.HasAttribute(LCase("id_agenda")) AndAlso IsNumeric(XML_ParametriAggiuntivi.GetAttribute(LCase("id_agenda"))) Then
            Me.Id_Agenda = CInt(XML_ParametriAggiuntivi.GetAttribute(LCase("id_agenda")))
        End If

        If XML_ParametriAggiuntivi.HasAttribute(LCase("sa_cod")) AndAlso IsNumeric(XML_ParametriAggiuntivi.GetAttribute(LCase("sa_cod"))) Then
            Me.Sa_Cod = CInt(XML_ParametriAggiuntivi.GetAttribute(LCase("sa_cod")))
        End If

        If XML_ParametriAggiuntivi.HasAttribute(LCase("veg_cod")) Then
            Me.Veg_Cod = XML_ParametriAggiuntivi.GetAttribute(LCase("veg_cod"))
        End If

        If XML_ParametriAggiuntivi.HasAttribute(LCase("appezza")) AndAlso IsNumeric(XML_ParametriAggiuntivi.GetAttribute(LCase("appezza"))) Then
            Me.Appezza = CInt(XML_ParametriAggiuntivi.GetAttribute(LCase("appezza")))
        End If

        If XML_ParametriAggiuntivi.HasAttribute(LCase("id_reg")) AndAlso IsNumeric(XML_ParametriAggiuntivi.GetAttribute(LCase("id_reg"))) Then
            Me.Id_Reg = CInt(XML_ParametriAggiuntivi.GetAttribute(LCase("id_reg")))
        End If

        If XML_ParametriAggiuntivi.HasAttribute(LCase("tipooperazioneagenda")) AndAlso IsNumeric(XML_ParametriAggiuntivi.GetAttribute(LCase("tipooperazioneagenda"))) Then
            Me.TipoOperazioneAgenda = CType(CInt(XML_ParametriAggiuntivi.GetAttribute(LCase("tipooperazioneagenda"))), enum_Tipo_Operazione_Agenda)
        End If

        If XML_ParametriAggiuntivi.HasAttribute(LCase("targetoperazione")) AndAlso IsNumeric(XML_ParametriAggiuntivi.GetAttribute(LCase("targetoperazione"))) Then
            Me.TargetOperazione = CType(CInt(XML_ParametriAggiuntivi.GetAttribute(LCase("targetoperazione"))), enum_Tipo_Operazione_Agenda_Target)
        End If

        If XML_ParametriAggiuntivi.HasAttribute(LCase("programmazione_cod")) AndAlso IsNumeric(XML_ParametriAggiuntivi.GetAttribute(LCase("programmazione_cod"))) Then
            Me.Programmazione_Cod = CInt(XML_ParametriAggiuntivi.GetAttribute(LCase("programmazione_cod")))
        End If

        If XML_ParametriAggiuntivi.HasAttribute(LCase("tiporicetta")) AndAlso IsNumeric(XML_ParametriAggiuntivi.GetAttribute(LCase("tiporicetta"))) Then
            Me.TipoRicetta = CType(CInt(XML_ParametriAggiuntivi.GetAttribute(LCase("tiporicetta"))), enum_TipoRicetta)
        End If

        If XML_ParametriAggiuntivi.HasAttribute(LCase("tipo_operazione")) AndAlso IsNumeric(XML_ParametriAggiuntivi.GetAttribute(LCase("tipo_operazione"))) Then
            Me.Tipo_Operazione = CType(CInt(XML_ParametriAggiuntivi.GetAttribute(LCase("tipo_operazione"))), enum_TipoOperazioneDB)
        End If

        If XML_ParametriAggiuntivi.HasAttribute(LCase("redirecturl")) Then
            Me.RedirectUrl = XML_ParametriAggiuntivi.GetAttribute(LCase("redirecturl"))
        End If

        If XML_ParametriAggiuntivi.HasAttribute(LCase("querystringfiltrino")) Then
            Me.QueryStringFiltrino = XML_ParametriAggiuntivi.GetAttribute(LCase("querystringfiltrino"))
        End If
    End Sub

#End Region

    ''' <summary>
    ''' Genera il blocco XML VariabiliAnalisi con all'interno
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GeneraXML() As String
        Dim XmlDoc As New System.Xml.XmlDocument
        Dim Xml_VariabiliAgenda As System.Xml.XmlElement
        'Creo il nodo "FiltroStampa"

        Xml_VariabiliAgenda = XmlDoc.CreateElement("ParametriDomandaIrrigua")
        Xml_VariabiliAgenda.SetAttribute("pagina_richiesta", PaginaRichiesta)
        Xml_VariabiliAgenda.SetAttribute("pagina_provenienza", PaginaProvenienza)
        Xml_VariabiliAgenda.SetAttribute("piva", If(Piva, ""))
        Xml_VariabiliAgenda.SetAttribute("dataselezionata", DataSelezionata)

        ' Attributi operazione
        Xml_VariabiliAgenda.SetAttribute("lav_cod", Lav_Cod)
        Xml_VariabiliAgenda.SetAttribute("id_agenda", Id_Agenda)
        Xml_VariabiliAgenda.SetAttribute("sa_cod", Sa_Cod)
        Xml_VariabiliAgenda.SetAttribute("veg_cod", If(Veg_Cod, ""))
        Xml_VariabiliAgenda.SetAttribute("appezza", Appezza)
        Xml_VariabiliAgenda.SetAttribute("id_reg", Id_Reg)
        Xml_VariabiliAgenda.SetAttribute("tipooperazioneagenda", CStr(CInt(TipoOperazioneAgenda)))
        Xml_VariabiliAgenda.SetAttribute("targetoperazione", CStr(CInt(TargetOperazione)))
        Xml_VariabiliAgenda.SetAttribute("programmazione_cod", Programmazione_Cod)
        Xml_VariabiliAgenda.SetAttribute("tiporicetta", CStr(CInt(TipoRicetta)))
        Xml_VariabiliAgenda.SetAttribute("tipo_operazione", CStr(CInt(Tipo_Operazione)))
        Xml_VariabiliAgenda.SetAttribute("redirecturl", If(RedirectUrl, ""))
        Xml_VariabiliAgenda.SetAttribute("querystringfiltrino", If(QueryStringFiltrino, ""))

        XmlDoc.AppendChild(Xml_VariabiliAgenda)
        Dim str As String
        str = XmlDoc.InnerXml
        str = str.Replace("'", "&#39;")
        Return str
    End Function

End Class


''' <summary>
''' ParametriPlanning 
''' </summary>
''' <remarks></remarks>
Public Class ParametriLabCQ

    Private _PaginaRichiesta As Integer
    Private _PaginaProvenienza As Integer
    Private _Piva As String

#Region "Proprietà"
    Property Piva() As String
        Get
            Return _Piva
        End Get
        Set(ByVal Value As String)
            _Piva = Value
            Salva()
        End Set
    End Property
    Property PaginaRichiesta() As Integer
        Get
            Return _PaginaRichiesta
        End Get
        Set(ByVal Value As Integer)
            _PaginaRichiesta = Value
            Salva()
        End Set
    End Property

    Property PaginaProvenienza() As Integer
        Get
            Return _PaginaProvenienza
        End Get
        Set(ByVal Value As Integer)
            _PaginaProvenienza = Value
            Salva()
        End Set
    End Property
#End Region

#Region "X Sessione"
    Public Sub Salva()
        HttpContext.Current.Session("ParametriLabCQ") = Me
    End Sub
    Public Sub Leggi()
        Dim obj As New ParametriLabCQ
        obj = HttpContext.Current.Session("ParametriLabCQ")
        If Not IsNothing(obj) Then
            _PaginaRichiesta = obj._PaginaRichiesta
            _PaginaProvenienza = obj._PaginaProvenienza
            _Piva = obj.Piva

        End If
    End Sub
#End Region

#Region "Costruttori"
    Public Sub New()

    End Sub

    Sub New(ByVal XML As String)
        LeggiXML(XML)
    End Sub

    Private Sub LeggiXML(ByVal XML As String)
        Dim XmlDoc As New System.Xml.XmlDocument
        XmlDoc.LoadXml(XML)

        Dim XML_ParametriAggiuntivi As System.Xml.XmlElement
        XML_ParametriAggiuntivi = XmlDoc.SelectSingleNode("Parametri").SelectSingleNode("ParametriLabCQ")


        If XML_ParametriAggiuntivi.HasAttribute(LCase("pagina_richiesta")) = True Then
            Me.PaginaRichiesta = XML_ParametriAggiuntivi.GetAttribute(LCase("pagina_richiesta"))
        End If

        If XML_ParametriAggiuntivi.HasAttribute(LCase("pagina_provenienza")) = True Then
            Me.PaginaProvenienza = XML_ParametriAggiuntivi.GetAttribute(LCase("pagina_provenienza"))
        End If

        If XML_ParametriAggiuntivi.HasAttribute(LCase("piva")) = True Then
            Me.Piva = XML_ParametriAggiuntivi.GetAttribute(LCase("piva"))
        End If
    End Sub

#End Region


    ''' <summary>
    ''' Genera il blocco XML VariabiliAnalisi con all'interno 
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GeneraXML() As String
        Dim XmlDoc As New System.Xml.XmlDocument
        Dim Xml_VariabiliAgenda As System.Xml.XmlElement
        'Creo il nodo "FiltroStampa"

        Xml_VariabiliAgenda = XmlDoc.CreateElement("ParametriLabCQ")
        Xml_VariabiliAgenda.SetAttribute("pagina_richiesta", PaginaRichiesta)
        Xml_VariabiliAgenda.SetAttribute("pagina_provenienza", PaginaProvenienza)
        Xml_VariabiliAgenda.SetAttribute("piva", Piva)

        XmlDoc.AppendChild(Xml_VariabiliAgenda)
        Dim str As String
        str = XmlDoc.InnerXml
        str = str.Replace("'", "&#39;")
        Return str
    End Function

End Class

''' <summary>
''' ParametriPlanning 
''' </summary>
''' <remarks></remarks>
Public Class ParametriNonConformita

    Public PaginaRichiesta As Integer
    Public PaginaProvenienza As Integer
    Public Piva As String
    Public NC_Str As String
    Public Categoria_Area_Str As String
    Public Categoria_Tipologia_Str As String

#Region "X Sessione"
    Public Sub Salva()
        HttpContext.Current.Session("ParametriNonConformita") = Me
    End Sub
    Public Sub Leggi()
        Dim obj As New ParametriNonConformita
        obj = HttpContext.Current.Session("ParametriNonConformita")
        If Not IsNothing(obj) Then
            PaginaRichiesta = obj.PaginaRichiesta
            PaginaProvenienza = obj.PaginaProvenienza
            Piva = obj.Piva
            NC_Str = obj.NC_Str
            Categoria_Area_Str = obj.Categoria_Area_Str
            Categoria_Tipologia_Str = obj.Categoria_Tipologia_Str
        End If
    End Sub
#End Region

#Region "Costruttori"
    Public Sub New()

    End Sub

    Sub New(ByVal XML As String)
        LeggiXML(XML)
    End Sub

    Private Sub LeggiXML(ByVal XML As String)
        Dim XmlDoc As New System.Xml.XmlDocument
        XmlDoc.LoadXml(XML)

        Dim XML_ParametriAggiuntivi As System.Xml.XmlElement
        XML_ParametriAggiuntivi = XmlDoc.SelectSingleNode("Parametri").SelectSingleNode("ParametriNonConformita")


        If XML_ParametriAggiuntivi.HasAttribute(LCase("pagina_richiesta")) = True Then
            Me.PaginaRichiesta = XML_ParametriAggiuntivi.GetAttribute(LCase("pagina_richiesta"))
        End If

        If XML_ParametriAggiuntivi.HasAttribute(LCase("pagina_provenienza")) = True Then
            Me.PaginaProvenienza = XML_ParametriAggiuntivi.GetAttribute(LCase("pagina_provenienza"))
        End If

        If XML_ParametriAggiuntivi.HasAttribute(LCase("piva")) = True Then
            Me.Piva = XML_ParametriAggiuntivi.GetAttribute(LCase("piva"))
        End If

        If XML_ParametriAggiuntivi.HasAttribute(LCase("nc_str")) = True Then
            Me.NC_Str = XML_ParametriAggiuntivi.GetAttribute(LCase("nc_str"))
        End If

        If XML_ParametriAggiuntivi.HasAttribute(LCase("cat_a")) = True Then
            Me.Categoria_Area_Str = XML_ParametriAggiuntivi.GetAttribute(LCase("cat_a"))
        End If

        If XML_ParametriAggiuntivi.HasAttribute(LCase("cat_t")) = True Then
            Me.Categoria_Tipologia_Str = XML_ParametriAggiuntivi.GetAttribute(LCase("cat_t"))
        End If

    End Sub

#End Region


    ''' <summary>
    ''' Genera il blocco XML VariabiliAnalisi con all'interno 
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GeneraXML() As String
        Dim XmlDoc As New System.Xml.XmlDocument
        Dim Xml_VariabiliAgenda As System.Xml.XmlElement

        Xml_VariabiliAgenda = XmlDoc.CreateElement("ParametriNonConformita")
        Xml_VariabiliAgenda.SetAttribute("pagina_richiesta", PaginaRichiesta)
        Xml_VariabiliAgenda.SetAttribute("pagina_provenienza", PaginaProvenienza)
        Xml_VariabiliAgenda.SetAttribute("piva", Piva)
        Xml_VariabiliAgenda.SetAttribute("nc_str", NC_Str)
        Xml_VariabiliAgenda.SetAttribute("cat_a", Categoria_Area_Str)
        Xml_VariabiliAgenda.SetAttribute("cat_t", Categoria_Tipologia_Str)

        XmlDoc.AppendChild(Xml_VariabiliAgenda)
        Dim str As String = XmlDoc.InnerXml.Replace("'", "&#39;")
        Return str
    End Function

End Class



''' <summary>
''' ParametriRicetta_2010 
''' </summary>
''' <remarks></remarks>
Public Class ParametriRicette_2010
    Private _Tipo_Operazione As AgronicaCoreDataProvider.TipiEnumerativi.enum_TipoOperazioneDB
    Private _Ricetta_Cod As Integer
    Private _Piva As String
    Private _Sa_Cod As Integer
    Private _Veg_Cod As Integer
    Private _Cul_Cod As Integer
    Private _data_inizio As Date
    Private _data_fine As Date
    Private _StrVariabiliAgenda As String
    Private _Tipo_Ricetta As AgronicaCoreDataProvider.TipiEnumerativi.enum_TipoRicetta
    Private _Ricetta_Des As String
    Private _Ricetta_Des_long As String
    Private _Programmazione_Cod As Integer
    Private _SitoOrigine As Integer
    Private _PaginaRichiesta As Integer
    Private _PaginaProvenienza As Integer

#Region "Proprietà"

    Public Property Tipo_Operazione() As AgronicaCoreDataProvider.TipiEnumerativi.enum_TipoOperazioneDB
        Get
            Return _Tipo_Operazione
        End Get
        Set(ByVal value As AgronicaCoreDataProvider.TipiEnumerativi.enum_TipoOperazioneDB)
            _Tipo_Operazione = value
            Salva()
        End Set
    End Property

    Public Property Ricetta_Cod() As Integer
        Get
            Return _Ricetta_Cod
        End Get
        Set(ByVal value As Integer)
            _Ricetta_Cod = value
            Salva()
        End Set
    End Property

    Property Piva() As String
        Get
            Return _Piva
        End Get
        Set(ByVal Value As String)
            _Piva = Value
            Salva()
        End Set
    End Property

    Public Property Sa_Cod() As Integer
        Get
            Return _Sa_Cod
        End Get
        Set(ByVal value As Integer)
            _Sa_Cod = value
            Salva()
        End Set
    End Property
    Public Property data_inizio() As Date
        Get
            Return _data_inizio
        End Get
        Set(ByVal value As Date)
            _data_inizio = value
            Salva()
        End Set
    End Property
    Public Property data_fine() As Date
        Get
            Return _data_fine
        End Get
        Set(ByVal value As Date)
            _data_fine = value
            Salva()
        End Set
    End Property
    Public Property Veg_Cod() As Integer
        Get
            Return _Veg_Cod
        End Get
        Set(ByVal value As Integer)
            _Veg_Cod = value
            Salva()
        End Set
    End Property
    Public Property Cul_Cod() As Integer
        Get
            Return _Cul_Cod
        End Get
        Set(ByVal value As Integer)
            _Cul_Cod = value
            Salva()
        End Set
    End Property
    Public Property StrVariabiliAgenda() As String
        Get
            Return _StrVariabiliAgenda
        End Get
        Set(ByVal value As String)
            _StrVariabiliAgenda = value
            Salva()
        End Set
    End Property

    Public Property Tipo_Ricetta() As AgronicaCoreDataProvider.TipiEnumerativi.enum_TipoRicetta
        Get
            Return _Tipo_Ricetta
        End Get
        Set(ByVal value As AgronicaCoreDataProvider.TipiEnumerativi.enum_TipoRicetta)
            _Tipo_Ricetta = value
            Salva()
        End Set
    End Property

    Property Ricetta_Des() As String
        Get
            Return _Ricetta_Des
        End Get
        Set(ByVal Value As String)
            _Ricetta_Des = Value
            Salva()
        End Set
    End Property

    Property Ricetta_Des_Long() As String
        Get
            Return _Ricetta_Des_long
        End Get
        Set(ByVal Value As String)
            _Ricetta_Des_long = Value
            Salva()
        End Set
    End Property

    Public Property Programmazione_Cod() As Integer
        Get
            Return _Programmazione_Cod
        End Get
        Set(ByVal value As Integer)
            _Programmazione_Cod = value
            Salva()
        End Set
    End Property

    Public Property SitoOrigine() As Integer
        Get
            Return _SitoOrigine
        End Get
        Set(ByVal value As Integer)
            _SitoOrigine = value
            Salva()
        End Set
    End Property

    Public Property PaginaRichiesta() As Integer
        Get
            Return _PaginaRichiesta
        End Get
        Set(ByVal value As Integer)
            _PaginaRichiesta = value
            Salva()
        End Set
    End Property

    Public Property PaginaProvenienza() As Integer
        Get
            Return _PaginaProvenienza
        End Get
        Set(ByVal value As Integer)
            _PaginaProvenienza = value
            Salva()
        End Set
    End Property

#End Region

#Region "X Sessione"
    Public Sub Salva()
        HttpContext.Current.Session("ParametriRicette_2010") = Me
    End Sub
    Public Sub Leggi()
        Dim obj As New ParametriRicette_2010
        obj = HttpContext.Current.Session("ParametriRicette_2010")
        If Not IsNothing(obj) Then

            _Tipo_Operazione = obj._Tipo_Operazione
            _Tipo_Ricetta = obj._Tipo_Ricetta
            _Ricetta_Cod = obj._Ricetta_Cod
            _Piva = obj._Piva
            _Sa_Cod = obj._Sa_Cod
            _Veg_Cod = obj._Veg_Cod
            _Cul_Cod = obj._Cul_Cod
            _data_inizio = obj._data_inizio
            _data_fine = obj._data_fine
            _StrVariabiliAgenda = obj._StrVariabiliAgenda
            _Ricetta_Des = obj._Ricetta_Des
            _Ricetta_Des_long = obj._Ricetta_Des_long
            _Programmazione_Cod = obj._Programmazione_Cod
            _PaginaRichiesta = obj._PaginaRichiesta
            _PaginaProvenienza = obj._PaginaProvenienza
            _SitoOrigine = obj.SitoOrigine
        End If
    End Sub
#End Region

#Region "Costruttori"
    Public Sub New()
        _data_inizio = AGRODATAINIZIO
        _data_fine = AGRODATAFINE
        _Tipo_Ricetta = enum_TipoRicetta.Standard
    End Sub

    Sub New(ByVal XML As String)
        LeggiXML(XML)
    End Sub

    Private Sub LeggiXML(ByVal XML As String)

        Dim XmlDoc As New System.Xml.XmlDocument
        XmlDoc.LoadXml(XML)

        Dim XML_ParametriAggiuntivi As System.Xml.XmlElement
        XML_ParametriAggiuntivi = XmlDoc.SelectSingleNode("Parametri").SelectSingleNode("ParametriRicette_2010")

        If XML_ParametriAggiuntivi.HasAttribute(LCase("tipo_operazione")) = True Then
            Me.Tipo_Operazione = XML_ParametriAggiuntivi.GetAttribute(LCase("tipo_operazione"))
        End If

        If XML_ParametriAggiuntivi.HasAttribute(LCase("ricetta_cod")) = True Then
            Me.Ricetta_Cod = XML_ParametriAggiuntivi.GetAttribute(LCase("ricetta_cod"))
        End If

        If XML_ParametriAggiuntivi.HasAttribute(LCase("piva")) = True Then
            Me.Piva = XML_ParametriAggiuntivi.GetAttribute(LCase("piva"))
        End If

        If XML_ParametriAggiuntivi.HasAttribute(LCase("sa_cod")) = True Then
            Me.Sa_Cod = XML_ParametriAggiuntivi.GetAttribute(LCase("sa_cod"))
        End If

        If XML_ParametriAggiuntivi.HasAttribute(LCase("veg_cod")) = True Then
            Me.Veg_Cod = XML_ParametriAggiuntivi.GetAttribute(LCase("veg_cod"))
        End If

        If XML_ParametriAggiuntivi.HasAttribute(LCase("cul_cod")) = True Then
            Me.Cul_Cod = XML_ParametriAggiuntivi.GetAttribute(LCase("cul_cod"))
        End If

        If XML_ParametriAggiuntivi.HasAttribute(LCase("data_inizio")) = True Then
            Me.data_inizio = XML_ParametriAggiuntivi.GetAttribute(LCase("data_inizio"))
        End If

        If XML_ParametriAggiuntivi.HasAttribute(LCase("data_fine")) = True Then
            Me.data_fine = XML_ParametriAggiuntivi.GetAttribute(LCase("data_fine"))
        End If

        If XML_ParametriAggiuntivi.HasAttribute(LCase("strvariabiliagenda")) = True Then
            Me.StrVariabiliAgenda = XML_ParametriAggiuntivi.GetAttribute(LCase("strvariabiliagenda"))
        End If

        If XML_ParametriAggiuntivi.HasAttribute(LCase("tipo_ricetta")) = True Then
            Me.Tipo_Ricetta = XML_ParametriAggiuntivi.GetAttribute(LCase("tipo_ricetta"))
        End If

        If XML_ParametriAggiuntivi.HasAttribute(LCase("ricetta_des")) = True Then
            Me.Ricetta_Des = XML_ParametriAggiuntivi.GetAttribute(LCase("ricetta_des"))
        End If

        If XML_ParametriAggiuntivi.HasAttribute(LCase("ricetta_des_long")) = True Then
            Me.Ricetta_Des_Long = XML_ParametriAggiuntivi.GetAttribute(LCase("ricetta_des_long"))
        End If

        If XML_ParametriAggiuntivi.HasAttribute(LCase("programmazione_cod")) = True Then
            Me.Programmazione_Cod = XML_ParametriAggiuntivi.GetAttribute(LCase("programmazione_cod"))
        End If

        If XML_ParametriAggiuntivi.HasAttribute(LCase("sitoorigine")) = True Then
            Me.SitoOrigine = XML_ParametriAggiuntivi.GetAttribute(LCase("sitoorigine"))
        End If

        If XML_ParametriAggiuntivi.HasAttribute(LCase("paginaprovenienza")) = True Then
            Me.PaginaProvenienza = XML_ParametriAggiuntivi.GetAttribute(LCase("paginaprovenienza"))
        End If

        If XML_ParametriAggiuntivi.HasAttribute(LCase("paginarichiesta")) = True Then
            Me.PaginaRichiesta = XML_ParametriAggiuntivi.GetAttribute(LCase("paginarichiesta"))
        End If
    End Sub

#End Region


    ''' <summary>
    ''' Genera il blocco XML VariabiliAnalisi con all'interno 
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GeneraXML() As String
        Dim XmlDoc As New System.Xml.XmlDocument
        Dim Xml_VariabiliAgenda As System.Xml.XmlElement
        'Creo il nodo "FiltroStampa"
        Xml_VariabiliAgenda = XmlDoc.CreateElement("ParametriRicette_2010")
        Xml_VariabiliAgenda.SetAttribute("tipo_operazione", Tipo_Operazione)
        Xml_VariabiliAgenda.SetAttribute("ricetta_cod", Ricetta_Cod)
        Xml_VariabiliAgenda.SetAttribute("piva", Piva)
        Xml_VariabiliAgenda.SetAttribute("sa_cod", Sa_Cod)
        Xml_VariabiliAgenda.SetAttribute("veg_cod", Veg_Cod)
        Xml_VariabiliAgenda.SetAttribute("cul_cod", Cul_Cod)
        Xml_VariabiliAgenda.SetAttribute("data_inizio", data_inizio)
        Xml_VariabiliAgenda.SetAttribute("data_fine", data_fine)
        Xml_VariabiliAgenda.SetAttribute("tipo_ricetta", Tipo_Ricetta)
        Xml_VariabiliAgenda.SetAttribute("ricetta_des", Ricetta_Des)
        Xml_VariabiliAgenda.SetAttribute("ricetta_des_long", Ricetta_Des_Long)
        Xml_VariabiliAgenda.SetAttribute("programmazione_cod", Programmazione_Cod)
        Xml_VariabiliAgenda.SetAttribute("sitoorigine", SitoOrigine)
        Xml_VariabiliAgenda.SetAttribute("paginaprovenienza", PaginaProvenienza)
        Xml_VariabiliAgenda.SetAttribute("paginarichiesta", PaginaRichiesta)

        Dim a As String = StrVariabiliAgenda
        If Not IsNothing(a) And a <> "" Then
            a = a.Replace("'", "&#39;")
        End If
        Xml_VariabiliAgenda.SetAttribute("strvariabiliagenda", a)

        XmlDoc.AppendChild(Xml_VariabiliAgenda)
        Dim str As String
        str = XmlDoc.InnerXml
        str = str.Replace("'", "&#39;")
        Return str
    End Function

End Class



'#################################################################################################
''' <summary>
''' ParametriVerificaDisciplinare2010
''' </summary>
''' <remarks></remarks>
Public Class ParametriVerificaDisciplinare2010

    Private _Piva As String
    Private _Sa_Cod As Integer
    Private _Appezza As Integer
    Private _Id_Reg As Integer
    Private _Cod_Disciplinare As Integer
    Private _Disciplinare_PubblicoPrivato As Integer
    Private _Des_Disciplinare As String
    Private _Progetto_Cod As Integer
    Private _Validita_Inizio As Date
    Private _Validita_Fine As Date

    Private _ListaImpianti() As Impianto


#Region "Proprietà"
    Property Piva() As String
        Get
            Return _Piva
        End Get
        Set(ByVal Value As String)
            _Piva = Value
            Salva()
        End Set
    End Property
    Public Property Sa_Cod() As Integer
        Get
            Return _Sa_Cod
        End Get
        Set(ByVal value As Integer)
            _Sa_Cod = value
            Salva()
        End Set
    End Property
    Public Property Appezza() As Integer
        Get
            Return _Appezza
        End Get
        Set(ByVal value As Integer)
            _Appezza = value
        End Set
    End Property
    Public Property Id_Reg() As Integer
        Get
            Return _Id_Reg
        End Get
        Set(ByVal value As Integer)
            _Id_Reg = value
        End Set
    End Property
    Public Property Progetto_Cod() As Integer
        Get
            Return _Progetto_Cod
        End Get
        Set(ByVal value As Integer)
            _Progetto_Cod = value
        End Set
    End Property
    Public Property Cod_Disciplinare() As Integer
        Get
            Return _Cod_Disciplinare
        End Get
        Set(ByVal value As Integer)
            _Cod_Disciplinare = value
        End Set
    End Property
    Public Property Disciplinare_PubblicoPrivato() As Integer
        Get
            Return _Disciplinare_PubblicoPrivato
        End Get
        Set(ByVal value As Integer)
            _Disciplinare_PubblicoPrivato = value
        End Set
    End Property
    Public Property Des_Disciplinare() As String
        Get
            Return _Des_Disciplinare
        End Get
        Set(ByVal value As String)
            _Des_Disciplinare = value
        End Set
    End Property
    Public Property Validita_Inizio() As Date
        Get
            Return _Validita_Inizio
        End Get
        Set(ByVal value As Date)
            _Validita_Inizio = value
        End Set
    End Property
    Public Property Validita_Fine() As Date
        Get
            Return _Validita_Fine
        End Get
        Set(ByVal value As Date)
            _Validita_Fine = value
        End Set
    End Property
    Public Property ListaImpianti() As Impianto()
        Get
            Return _ListaImpianti
        End Get
        Set(ByVal value As Impianto())
            _ListaImpianti = value
            Salva()
        End Set
    End Property


#End Region

#Region "X Sessione"
    Public Sub Salva()
        HttpContext.Current.Session("ParametriVerificaDisciplinare2010") = Me
    End Sub
    Public Sub Leggi()
        Dim obj As New ParametriVerificaDisciplinare2010
        obj = HttpContext.Current.Session("ParametriVerificaDisciplinare2010")
        If Not IsNothing(obj) Then
            _Piva = obj._Piva
            _Sa_Cod = obj._Sa_Cod
            _Appezza = obj._Appezza
            _Id_Reg = obj._Id_Reg
            _Cod_Disciplinare = obj._Cod_Disciplinare
            _Des_Disciplinare = obj._Des_Disciplinare
            _Disciplinare_PubblicoPrivato = obj._Disciplinare_PubblicoPrivato
            _Progetto_Cod = obj._Progetto_Cod
            _Validita_Inizio = obj._Validita_Inizio
            _Validita_Fine = obj._Validita_Fine
            _ListaImpianti = obj._ListaImpianti
        End If
    End Sub
#End Region

#Region "Costruttori"
    Public Sub New()
        _Validita_Inizio = AGRODATAINIZIO
        _Validita_Fine = AGRODATAFINE
    End Sub

    Sub New(ByVal XML As String)
        LeggiXML(XML)
    End Sub

    Private Sub LeggiXML(ByVal XML As String)
        Dim XmlDoc As New System.Xml.XmlDocument
        XmlDoc.LoadXml(XML)

        Dim XML_ParametriAggiuntivi As System.Xml.XmlElement
        XML_ParametriAggiuntivi = XmlDoc.SelectSingleNode("Parametri").SelectSingleNode("ParametriVerificaDisciplinare2010")

        If XML_ParametriAggiuntivi.HasAttribute(LCase("piva")) = True Then
            Me.Piva = XML_ParametriAggiuntivi.GetAttribute(LCase("piva"))
        End If

        If XML_ParametriAggiuntivi.HasAttribute(LCase("sa_cod")) = True Then
            Me.Sa_Cod = XML_ParametriAggiuntivi.GetAttribute(LCase("sa_cod"))
        End If

        If XML_ParametriAggiuntivi.HasAttribute("appezza") = True Then
            Me.Appezza = XML_ParametriAggiuntivi.GetAttribute("appezza")
        End If

        If XML_ParametriAggiuntivi.HasAttribute("id_reg") = True Then
            Me.Id_Reg = XML_ParametriAggiuntivi.GetAttribute("id_reg")
        End If

        If XML_ParametriAggiuntivi.HasAttribute("Cod_Disciplinare") = True Then
            Me.Cod_Disciplinare = XML_ParametriAggiuntivi.GetAttribute("Cod_Disciplinare")
        End If

        If XML_ParametriAggiuntivi.HasAttribute("disciplinare_pubblicoprivato") = True Then
            Me.Disciplinare_PubblicoPrivato = XML_ParametriAggiuntivi.GetAttribute("disciplinare_pubblicoprivato")
        End If

        If XML_ParametriAggiuntivi.HasAttribute("Des_Disciplinare") = True Then
            Me.Des_Disciplinare = XML_ParametriAggiuntivi.GetAttribute("Des_Disciplinare")
        End If

        If XML_ParametriAggiuntivi.HasAttribute("progetto_cod") = True Then
            Me.Progetto_Cod = XML_ParametriAggiuntivi.GetAttribute("progetto_cod")
        End If

        If XML_ParametriAggiuntivi.HasAttribute("validita_inizio") = True Then
            Me.Validita_Inizio = XML_ParametriAggiuntivi.GetAttribute("validita_inizio")
        End If

        If XML_ParametriAggiuntivi.HasAttribute("validita_fine") = True Then
            Me.Validita_Fine = XML_ParametriAggiuntivi.GetAttribute("validita_fine")
        End If

        'Lista degli impianti
        Dim list As XmlNodeList = XmlDoc.SelectSingleNode("Parametri").SelectNodes("ParametriVerificaDisciplinare2010")(0).ChildNodes()
        If list.Count > 0 Then
            Dim i As Integer
            'ridimensiono l'array
            ReDim ListaImpianti(list.Count - 1)
            Dim VarImp As XmlElement
            For Each VarImp In list
                Dim impianto As New Impianto
                impianto.Piva = VarImp.Attributes("p").Value
                impianto.Sa_Cod = VarImp.Attributes("s").Value
                impianto.Appezza = VarImp.Attributes("a").Value
                impianto.Id_Reg = VarImp.Attributes("ir").Value
                impianto.Veg_Cod = VarImp.Attributes("vc").Value
                'lo aggiungo
                ListaImpianti(i) = impianto
                i = i + 1
            Next
        End If

    End Sub

    Public Sub AddList(ByVal _Impianto As Impianto)
        'ridimensiono 
        If IsNothing(ListaImpianti) Then
            ListaImpianti = New Impianto(0) {}
        Else
            ReDim Preserve ListaImpianti(ListaImpianti.Length)
        End If
        ListaImpianti(ListaImpianti.Length - 1) = _Impianto
    End Sub

#End Region


    ''' <summary>
    ''' Genera il blocco XML VariabiliAnalisi con all'interno 
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GeneraXML() As String

        Dim XmlDoc As New System.Xml.XmlDocument
        Dim Xml_VariabiliAgenda As System.Xml.XmlElement

        Xml_VariabiliAgenda = XmlDoc.CreateElement("ParametriVerificaDisciplinare2010")
        Xml_VariabiliAgenda.SetAttribute("piva", Piva)
        Xml_VariabiliAgenda.SetAttribute("sa_cod", Sa_Cod)
        Xml_VariabiliAgenda.SetAttribute("appezza", Appezza)
        Xml_VariabiliAgenda.SetAttribute("id_reg", Id_Reg)
        Xml_VariabiliAgenda.SetAttribute("Cod_Disciplinare", Cod_Disciplinare)
        Xml_VariabiliAgenda.SetAttribute("disciplinare_pubblicoprivato", Disciplinare_PubblicoPrivato)
        Xml_VariabiliAgenda.SetAttribute("Des_Disciplinare", Des_Disciplinare)
        Xml_VariabiliAgenda.SetAttribute("progetto_cod", Progetto_Cod)
        Xml_VariabiliAgenda.SetAttribute("validita_inizio", Validita_Inizio)
        Xml_VariabiliAgenda.SetAttribute("validita_fine", Validita_Fine)

        'aggiungo gli impianti 
        Dim i As Integer
        If Not IsNothing(ListaImpianti) Then
            For i = 0 To ListaImpianti.Length - 1
                Dim VarImp As XmlElement
                VarImp = XmlDoc.CreateElement("VarImp")

                VarImp.SetAttribute("p", ListaImpianti(i).Piva.Replace("'", "&#39;"))
                VarImp.SetAttribute("s", ListaImpianti(i).Sa_Cod.ToString.Replace("'", "&#39;"))
                VarImp.SetAttribute("a", ListaImpianti(i).Appezza.ToString.Replace("'", "&#39;"))
                VarImp.SetAttribute("ir", ListaImpianti(i).Id_Reg.ToString.Replace("'", "&#39;"))
                VarImp.SetAttribute("vc", ListaImpianti(i).Veg_Cod.ToString.Replace("'", "&#39;"))

                Xml_VariabiliAgenda.AppendChild(VarImp)
            Next

        End If

        XmlDoc.AppendChild(Xml_VariabiliAgenda)

        Dim str As String
        str = XmlDoc.InnerXml
        str = str.Replace("'", "&#39;")
        Return str
    End Function

End Class



'#################################################################################################
''' <summary>
''' GiasOnline
''' </summary>
''' <remarks></remarks>
Public Class ParametriGiasOnline

    Private _PaginaRichiesta As Integer

    Private _Piva As String
    Private _Cod_Contatto As String
    Private _Sa_Cod As Integer
    Private _Rag_Soc As String
    Private _Sa_Nome As String
    Private _DataSelezionata As Date
    Private _Veg_Cod As Integer
    Private _Cul_Cod As Integer
    Private _Id_Agenda As Integer
    Private _Operazione As Integer
    Private _Lavorazione As Integer
    Private _DPI_Cod As String
    Private _IdRcdpi As String
    Private _Mode As String
    Private _Causale As String
    Private _xChiave As String
    Private _ElemCod As String
    Private _Id_Reg As String
    Private _AmaxX As String
    Private _AmaxY As String
    Private _AminX As String
    Private _AminY As String
    Private _funzioneoriginedestinazionefiltrone As String

    Private _Xml_Generico As StringBuilder

    Private _LinkAgronicaAgenda2010 As String

    Private _RisorsaEditQueryString As String


#Region "Proprietà"
    Property AmaxX() As String
        Get
            Return _AmaxX
        End Get
        Set(ByVal Value As String)
            _AmaxX = Value
            Salva()
        End Set
    End Property
    Property AmaxY() As String
        Get
            Return _AmaxY
        End Get
        Set(ByVal Value As String)
            _AmaxY = Value
            Salva()
        End Set
    End Property
    Property AminX() As String
        Get
            Return _AminX
        End Get
        Set(ByVal Value As String)
            _AminX = Value
            Salva()
        End Set
    End Property
    Property AminY() As String
        Get
            Return _AminY
        End Get
        Set(ByVal Value As String)
            _AminY = Value
            Salva()
        End Set
    End Property
    Property funzioneoriginedestinazionefiltrone() As String
        Get
            Return _funzioneoriginedestinazionefiltrone
        End Get
        Set(ByVal Value As String)
            _funzioneoriginedestinazionefiltrone = Value
            Salva()
        End Set
    End Property

    Property Id_Reg() As String
        Get
            Return _Id_Reg
        End Get
        Set(ByVal Value As String)
            _Id_Reg = Value
            Salva()
        End Set
    End Property

    Property Rag_Soc() As String
        Get
            Return _Rag_Soc
        End Get
        Set(ByVal Value As String)
            _Rag_Soc = Value
            Salva()
        End Set
    End Property

    Property Sa_Nome() As String
        Get
            Return _Sa_Nome
        End Get
        Set(ByVal Value As String)
            _Sa_Nome = Value
            Salva()
        End Set
    End Property

    Property ElemCod() As String
        Get
            Return _ElemCod
        End Get
        Set(ByVal Value As String)
            _ElemCod = Value
            Salva()
        End Set
    End Property

    Property xChiave() As String
        Get
            Return _xChiave
        End Get
        Set(ByVal Value As String)
            _xChiave = Value
            Salva()
        End Set
    End Property

    Property Causale() As String
        Get
            Return _Causale
        End Get
        Set(ByVal Value As String)
            _Causale = Value
            Salva()
        End Set
    End Property

    Property DPI_Cod() As String
        Get
            Return _DPI_Cod
        End Get
        Set(ByVal Value As String)
            _DPI_Cod = Value
            Salva()
        End Set
    End Property

    Property Mode() As String
        Get
            Return _Mode
        End Get
        Set(ByVal Value As String)
            _Mode = Value
            Salva()
        End Set
    End Property

    Property IdRcdpi() As String
        Get
            Return _IdRcdpi
        End Get
        Set(ByVal Value As String)
            _IdRcdpi = Value
            Salva()
        End Set
    End Property

    Property PaginaRichiesta() As Integer
        Get
            Return _PaginaRichiesta
        End Get
        Set(ByVal Value As Integer)
            _PaginaRichiesta = Value
            Salva()
        End Set
    End Property
    Property Piva() As String
        Get
            Return _Piva
        End Get
        Set(ByVal Value As String)
            _Piva = Value
            Salva()
        End Set
    End Property
    Property Cod_Contatto() As String
        Get
            Return _Cod_Contatto
        End Get
        Set(ByVal Value As String)
            _Cod_Contatto = Value
            Salva()
        End Set
    End Property


    Public Property Sa_Cod() As Integer
        Get
            Return _Sa_Cod
        End Get
        Set(ByVal value As Integer)
            _Sa_Cod = value
            Salva()
        End Set
    End Property
    Public Property DataSelezionata() As Date
        Get
            Return _DataSelezionata
        End Get
        Set(ByVal value As Date)
            _DataSelezionata = value
            Salva()
        End Set
    End Property
    Public Property Veg_Cod() As Integer
        Get
            Return _Veg_Cod
        End Get
        Set(ByVal value As Integer)
            _Veg_Cod = value
            Salva()
        End Set
    End Property
    Public Property Cul_Cod() As Integer
        Get
            Return _Cul_Cod
        End Get
        Set(ByVal value As Integer)
            _Cul_Cod = value
            Salva()
        End Set
    End Property
    Public Property Id_Agenda() As Integer
        Get
            Return _Id_Agenda
        End Get
        Set(ByVal value As Integer)
            _Id_Agenda = value
            Salva()
        End Set
    End Property
    Public Property Operazione() As Integer
        Get
            Return _Operazione
        End Get
        Set(ByVal value As Integer)
            _Operazione = value
            Salva()
        End Set
    End Property
    Public Property Lavorazione() As Integer
        Get
            Return _Lavorazione
        End Get
        Set(ByVal value As Integer)
            _Lavorazione = value
            Salva()
        End Set
    End Property
    Property Xml_Generico() As StringBuilder
        Get
            Return _Xml_Generico
        End Get
        Set(ByVal Value As StringBuilder)
            _Xml_Generico = Value
            Salva()
        End Set
    End Property
    Property LinkAgronicaAgenda2010() As String
        Get
            Return _LinkAgronicaAgenda2010
        End Get
        Set(ByVal Value As String)
            _LinkAgronicaAgenda2010 = Value
            Salva()
        End Set
    End Property
    Property RisorsaEditQueryString() As String
        Get
            Return _RisorsaEditQueryString
        End Get
        Set(ByVal Value As String)
            _RisorsaEditQueryString = Value
            Salva()
        End Set
    End Property
#End Region

#Region "X Sessione"
    Public Sub Salva()
        HttpContext.Current.Session("ParametriGiasOnline") = Me
    End Sub
    Public Sub Leggi()
        Dim obj As New ParametriGiasOnline
        obj = HttpContext.Current.Session("ParametriGiasOnline")
        If Not IsNothing(obj) Then
            _Piva = obj._Piva
            _PaginaRichiesta = obj._PaginaRichiesta
            _Sa_Cod = obj._Sa_Cod
            _DataSelezionata = obj._DataSelezionata
            _Veg_Cod = obj._Veg_Cod
            _Cul_Cod = obj._Cul_Cod
            _Id_Agenda = obj._Id_Agenda
            _Operazione = obj._Operazione
            _Lavorazione = obj._Lavorazione
            _IdRcdpi = obj._IdRcdpi
            _DPI_Cod = obj._DPI_Cod
            _ElemCod = obj._ElemCod
            _Mode = obj._Mode
            _Causale = obj._Causale
            _xChiave = obj._xChiave

            _Rag_Soc = obj._Rag_Soc
            _Sa_Cod = obj._Sa_Cod
            _Id_Reg = obj._Id_Reg
            _AmaxX = obj._AmaxX
            _AmaxY = obj._AmaxY
            _AminX = obj._AminX
            _AminY = obj._AminY

            _funzioneoriginedestinazionefiltrone = obj._funzioneoriginedestinazionefiltrone

            _Cod_Contatto = obj._Cod_Contatto

            _Xml_Generico = obj._Xml_Generico

            _LinkAgronicaAgenda2010 = obj._LinkAgronicaAgenda2010

            _RisorsaEditQueryString = obj._RisorsaEditQueryString

        End If

    End Sub
#End Region

#Region "Costruttori"
    Public Sub New()
        _Xml_Generico = New StringBuilder
        _Xml_Generico.Length = 0
    End Sub

    Sub New(ByVal XML As String)
        LeggiXML(XML)
    End Sub

    Private Sub LeggiXML(ByVal XML As String)

        Dim XmlDoc As New System.Xml.XmlDocument
        XmlDoc.LoadXml(XML)

        Dim XML_ParametriAggiuntivi As System.Xml.XmlElement
        XML_ParametriAggiuntivi = XmlDoc.SelectSingleNode("Parametri").SelectSingleNode("ParametriGiasOnline")

        If XML_ParametriAggiuntivi.HasAttribute(LCase("paginarichiesta")) = True Then
            Me.PaginaRichiesta = XML_ParametriAggiuntivi.GetAttribute(LCase("paginarichiesta"))
        End If

        If XML_ParametriAggiuntivi.HasAttribute(LCase("piva")) = True Then
            Me.Piva = XML_ParametriAggiuntivi.GetAttribute(LCase("piva"))
        End If

        If XML_ParametriAggiuntivi.HasAttribute(LCase("sa_cod")) = True Then
            Me.Sa_Cod = XML_ParametriAggiuntivi.GetAttribute(LCase("sa_cod"))
        End If

        If XML_ParametriAggiuntivi.HasAttribute(LCase("dataselezionata")) = True Then
            Me.DataSelezionata = XML_ParametriAggiuntivi.GetAttribute(LCase("dataselezionata"))
        End If
        If XML_ParametriAggiuntivi.HasAttribute(LCase("veg_cod")) = True Then
            Me.Veg_Cod = XML_ParametriAggiuntivi.GetAttribute(LCase("veg_cod"))
        End If

        If XML_ParametriAggiuntivi.HasAttribute(LCase("cul_cod")) = True Then
            Me.Cul_Cod = XML_ParametriAggiuntivi.GetAttribute(LCase("cul_cod"))
        End If
        If XML_ParametriAggiuntivi.HasAttribute(LCase("id_agenda")) = True Then
            Me.Id_Agenda = XML_ParametriAggiuntivi.GetAttribute(LCase("id_agenda"))
        End If
        If XML_ParametriAggiuntivi.HasAttribute(LCase("operazione")) = True Then
            Me.Operazione = XML_ParametriAggiuntivi.GetAttribute(LCase("operazione"))
        End If
        If XML_ParametriAggiuntivi.HasAttribute(LCase("lavorazione")) = True Then
            Me.Lavorazione = XML_ParametriAggiuntivi.GetAttribute(LCase("lavorazione"))
        End If

        If XML_ParametriAggiuntivi.HasAttribute(LCase("idrcdpi")) = True Then
            Me.IdRcdpi = XML_ParametriAggiuntivi.GetAttribute(LCase("idrcdpi"))
        End If
        If XML_ParametriAggiuntivi.HasAttribute(LCase("dpi_cod")) = True Then
            Me.DPI_Cod = XML_ParametriAggiuntivi.GetAttribute(LCase("dpi_cod"))
        End If

        If XML_ParametriAggiuntivi.HasAttribute(LCase("elemcod")) = True Then
            Me.ElemCod = XML_ParametriAggiuntivi.GetAttribute(LCase("elemcod"))
        End If
        If XML_ParametriAggiuntivi.HasAttribute(LCase("mode")) = True Then
            Me.Mode = XML_ParametriAggiuntivi.GetAttribute(LCase("mode"))
        End If
        If XML_ParametriAggiuntivi.HasAttribute(LCase("causale")) = True Then
            Me.Causale = XML_ParametriAggiuntivi.GetAttribute(LCase("causale"))
        End If
        If XML_ParametriAggiuntivi.HasAttribute(LCase("xchiave")) = True Then
            Me.xChiave = XML_ParametriAggiuntivi.GetAttribute(LCase("xchiave"))
        End If

        If XML_ParametriAggiuntivi.HasAttribute(LCase("sa_cod")) = True Then
            Me.Sa_Cod = XML_ParametriAggiuntivi.GetAttribute(LCase("sa_cod"))
        End If
        If XML_ParametriAggiuntivi.HasAttribute(LCase("rag_soc")) = True Then
            Me.Rag_Soc = XML_ParametriAggiuntivi.GetAttribute(LCase("rag_soc"))
        End If

        If XML_ParametriAggiuntivi.HasAttribute(LCase("id_reg")) = True Then
            Me.Id_Reg = XML_ParametriAggiuntivi.GetAttribute(LCase("id_reg"))
        End If


        If XML_ParametriAggiuntivi.HasAttribute(LCase("AmaxX")) = True Then
            Me.AmaxX = XML_ParametriAggiuntivi.GetAttribute(LCase("AmaxX"))
        End If
        If XML_ParametriAggiuntivi.HasAttribute(LCase("AmaxY")) = True Then
            Me.AmaxY = XML_ParametriAggiuntivi.GetAttribute(LCase("AmaxY"))
        End If
        If XML_ParametriAggiuntivi.HasAttribute(LCase("AminX")) = True Then
            Me.AminX = XML_ParametriAggiuntivi.GetAttribute(LCase("AminX"))
        End If
        If XML_ParametriAggiuntivi.HasAttribute(LCase("AminY")) = True Then
            Me.AminY = XML_ParametriAggiuntivi.GetAttribute(LCase("AminY"))
        End If

        If XML_ParametriAggiuntivi.HasAttribute(LCase("funzioneoriginedestinazionefiltrone")) = True Then
            Me.funzioneoriginedestinazionefiltrone = XML_ParametriAggiuntivi.GetAttribute(LCase("funzioneoriginedestinazionefiltrone"))
        End If

        If XML_ParametriAggiuntivi.HasAttribute(LCase("Cod_Contatto")) = True Then
            Me.Cod_Contatto = XML_ParametriAggiuntivi.GetAttribute(LCase("Cod_Contatto"))
        End If

        If XML_ParametriAggiuntivi.HasAttribute(LCase("linkagronicaagenda2010")) = True Then
            Me.LinkAgronicaAgenda2010 = XML_ParametriAggiuntivi.GetAttribute(LCase("linkagronicaagenda2010"))
        End If

        If XML_ParametriAggiuntivi.HasAttribute(LCase("RisorsaEditQueryString")) = True Then
            Me.RisorsaEditQueryString = XML_ParametriAggiuntivi.GetAttribute(LCase("RisorsaEditQueryString"))
        End If

        If XML_ParametriAggiuntivi.HasChildNodes = True Then
            Me._Xml_Generico = New StringBuilder
            Me.Xml_Generico.Append(XML_ParametriAggiuntivi.InnerXml)
        End If


    End Sub

#End Region


    ''' <summary>
    ''' Genera il blocco XML VariabiliAnalisi con all'interno 
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GeneraXML() As String
        Dim XmlDoc As New System.Xml.XmlDocument
        Dim Xml_VariabiliGias As System.Xml.XmlElement
        'Creo il nodo "FiltroStampa"
        Xml_VariabiliGias = XmlDoc.CreateElement("ParametriGiasOnline")
        Xml_VariabiliGias.SetAttribute("paginarichiesta", PaginaRichiesta)
        Xml_VariabiliGias.SetAttribute("piva", Piva)
        Xml_VariabiliGias.SetAttribute("sa_cod", Sa_Cod)
        Xml_VariabiliGias.SetAttribute("dataselezionata", DataSelezionata)
        Xml_VariabiliGias.SetAttribute("veg_cod", Veg_Cod)
        Xml_VariabiliGias.SetAttribute("cul_cod", Cul_Cod)
        Xml_VariabiliGias.SetAttribute("id_agenda", Id_Agenda)

        Xml_VariabiliGias.SetAttribute("id_agenda", Id_Agenda)
        Xml_VariabiliGias.SetAttribute("operazione", Operazione)
        Xml_VariabiliGias.SetAttribute("lavorazione", Lavorazione)

        Xml_VariabiliGias.SetAttribute("dpi_cod", DPI_Cod)
        Xml_VariabiliGias.SetAttribute("idrcdpi", IdRcdpi)


        Xml_VariabiliGias.SetAttribute("elemcod", ElemCod)
        Xml_VariabiliGias.SetAttribute("mode", Mode)
        Xml_VariabiliGias.SetAttribute("causale", Causale)
        Xml_VariabiliGias.SetAttribute("xchiave", xChiave)


        Xml_VariabiliGias.SetAttribute("rag_soc", Rag_Soc)
        Xml_VariabiliGias.SetAttribute("sa_nome", Sa_Nome)

        Xml_VariabiliGias.SetAttribute("id_reg", Id_Reg)



        Xml_VariabiliGias.SetAttribute("amaxx", AmaxX)
        Xml_VariabiliGias.SetAttribute("amaxy", AmaxY)
        Xml_VariabiliGias.SetAttribute("aminx", AminX)
        Xml_VariabiliGias.SetAttribute("aminy", AminY)

        Xml_VariabiliGias.SetAttribute("funzioneoriginedestinazionefiltrone", funzioneoriginedestinazionefiltrone)

        Xml_VariabiliGias.SetAttribute("cod_contatto", Cod_Contatto)

        Xml_VariabiliGias.SetAttribute("linkagronicaagenda2010", LinkAgronicaAgenda2010)

        Xml_VariabiliGias.SetAttribute("RisorsaEditQueryString".ToLower, RisorsaEditQueryString)

        If Xml_Generico.Length > 0 Then
            Dim strxml As String = Xml_Generico.ToString
            strxml = strxml.Replace("'", "&#39;")
            Xml_VariabiliGias.InnerXml = strxml
        End If

        XmlDoc.AppendChild(Xml_VariabiliGias)
        Dim str As String
        str = XmlDoc.InnerXml
        str = str.Replace("'", "&#39;")
        Return str
    End Function


End Class



'#################################################################################################
''' <summary>
''' AgronicaStampe
''' </summary>
''' <remarks></remarks>
Public Class ParametriAgronicaStampe


    Private _username As String

    Private _report As Integer
    Private _user_profilo As String

    Private _UserProfilo_CodFisc As String
    Private _Sql_Filtro As String
    Private _Xml_Filtro As String

    Private _username_codfisc As String
    Private _user_profilo_codfisc As String
    Private _user_profilo_codgias As String

    Private _utente_codfiscale As String
    Private _superuser_codfiscale As String
    Private _superuser_username As String
    Private _data_stampa As String


    Private _centro_costo As String
    Private _superficie As String
    Private _ricavi As String
    Private _costi As String
    Private _differenza As String
    Private _ricavi_ha As String
    Private _costi_ha As String
    Private _differenza_ha As String

    Private _ricavo_lordo As String
    Private _ricavo_totale As String
    Private _costi_variabili As String
    Private _margine_lordo As String
    Private _costi_operativi As String
    Private _margine_operativo As String
    Private _costi_fissi As String
    Private _margine_netto As String

    Private _visualizzazione As String

    Private _id_agenda As Integer

    Private _Xml_Generico As StringBuilder
    Private _JSon_Generico As StringBuilder
    Private _idSezione As Integer

#Region "Proprietà"

    Property Id_Agenda() As Integer
        Get
            Return _id_agenda
        End Get
        Set(ByVal Value As Integer)
            _id_agenda = Value
            Salva()
        End Set
    End Property

    Property visualizzazione() As String
        Get
            Return _visualizzazione
        End Get
        Set(ByVal Value As String)
            _visualizzazione = Value
            Salva()
        End Set
    End Property
    Property ricavo_lordo() As String
        Get
            Return _ricavo_lordo
        End Get
        Set(ByVal Value As String)
            _ricavo_lordo = Value
            Salva()
        End Set
    End Property
    Property ricavo_totale() As String
        Get
            Return _ricavo_totale
        End Get
        Set(ByVal Value As String)
            _ricavo_totale = Value
            Salva()
        End Set
    End Property
    Property costi_variabili() As String
        Get
            Return _costi_variabili
        End Get
        Set(ByVal Value As String)
            _costi_variabili = Value
            Salva()
        End Set
    End Property
    Property margine_lordo() As String
        Get
            Return _margine_lordo
        End Get
        Set(ByVal Value As String)
            _margine_lordo = Value
            Salva()
        End Set
    End Property
    Property costi_operativi() As String
        Get
            Return _costi_operativi
        End Get
        Set(ByVal Value As String)
            _costi_operativi = Value
            Salva()
        End Set
    End Property
    Property margine_operativo() As String
        Get
            Return _margine_operativo
        End Get
        Set(ByVal Value As String)
            _margine_operativo = Value
            Salva()
        End Set
    End Property
    Property costi_fissi() As String
        Get
            Return _costi_fissi
        End Get
        Set(ByVal Value As String)
            _costi_fissi = Value
            Salva()
        End Set
    End Property
    Property margine_netto() As String
        Get
            Return _margine_netto
        End Get
        Set(ByVal Value As String)
            _margine_netto = Value
            Salva()
        End Set
    End Property



    Property UserProfilo_CodFisc() As String
        Get
            Return _UserProfilo_CodFisc
        End Get
        Set(ByVal Value As String)
            _UserProfilo_CodFisc = Value
            Salva()
        End Set
    End Property

    Property Sql_Filtro() As String
        'devo stare attento al carattere percentuale e ' quindi faccio in modo di sostituirmlo
        Get
            Dim value As String = _Sql_Filtro
            'If Not String.IsNullOrEmpty(value) Then
            If Not IsNothing(value) And value <> String.Empty Then
                value = value.Replace("SQL_SAFE_CARATTERE_PERCENTUALE", "%")
                value = value.Replace("SQL_SAFE_CARATTERE_APIVCE", "'")
            Else
                value = String.Empty
            End If

            Return value
        End Get
        Set(ByVal Value As String)
            Value = Value.Replace("%", "SQL_SAFE_CARATTERE_PERCENTUALE")
            Value = Value.Replace("'", "SQL_SAFE_CARATTERE_APIVCE")
            _Sql_Filtro = Value
            Salva()
        End Set
    End Property

    Property Xml_Filtro() As String
        'devo stare attento al carattere percentuale e ' quindi faccio in modo di sostituirmlo
        Get
            Dim value As String = _Xml_Filtro
            'If Not String.IsNullOrEmpty(value) Then
            If Not IsNothing(value) And value <> String.Empty Then
                value = value.Replace("SQL_SAFE_CARATTERE_PERCENTUALE", "%")
                value = value.Replace("SQL_SAFE_CARATTERE_APIVCE", "'")
            Else
                value = String.Empty
            End If

            Return value
        End Get
        Set(ByVal Value As String)
            Value = Value.Replace("%", "SQL_SAFE_CARATTERE_PERCENTUALE")
            Value = Value.Replace("'", "SQL_SAFE_CARATTERE_APIVCE")
            _Xml_Filtro = Value
            Salva()
        End Set
    End Property

    Property centro_costo() As String
        Get
            Return _centro_costo
        End Get
        Set(ByVal Value As String)
            _centro_costo = Value
            Salva()
        End Set
    End Property
    Property superficie() As String
        Get
            Return _superficie
        End Get
        Set(ByVal Value As String)
            _superficie = Value
            Salva()
        End Set
    End Property
    Property ricavi() As String
        Get
            Return _ricavi
        End Get
        Set(ByVal Value As String)
            _ricavi = Value
            Salva()
        End Set
    End Property
    Property costi() As String
        Get
            Return _costi
        End Get
        Set(ByVal Value As String)
            _costi = Value
            Salva()
        End Set
    End Property
    Property differenza() As String
        Get
            Return _differenza
        End Get
        Set(ByVal Value As String)
            _differenza = Value
            Salva()
        End Set
    End Property
    Property ricavi_ha() As String
        Get
            Return _ricavi_ha
        End Get
        Set(ByVal Value As String)
            _ricavi_ha = Value
            Salva()
        End Set
    End Property
    Property costi_ha() As String
        Get
            Return _costi_ha
        End Get
        Set(ByVal Value As String)
            _costi_ha = Value
            Salva()
        End Set
    End Property
    Property differenza_ha() As String
        Get
            Return _differenza_ha
        End Get
        Set(ByVal Value As String)
            _differenza_ha = Value
            Salva()
        End Set
    End Property

    Property username() As String
        Get
            Return _username
        End Get
        Set(ByVal Value As String)
            _username = Value
            Salva()
        End Set
    End Property

    Property username_codfisc() As String
        Get
            Return _username_codfisc
        End Get
        Set(ByVal Value As String)
            _username_codfisc = Value
            Salva()
        End Set
    End Property

    Property user_profilo_codfisc() As String
        Get
            Return _user_profilo_codfisc
        End Get
        Set(ByVal Value As String)
            _user_profilo_codfisc = Value
            Salva()
        End Set
    End Property

    Property user_profilo_codgias() As String
        Get
            Return _user_profilo_codgias
        End Get
        Set(ByVal Value As String)
            _user_profilo_codgias = Value
            Salva()
        End Set
    End Property



    Property utente_codfiscale() As String
        Get
            Return _utente_codfiscale
        End Get
        Set(ByVal Value As String)
            _utente_codfiscale = Value
            Salva()
        End Set
    End Property
    Property superuser_codfiscale() As String
        Get
            Return _superuser_codfiscale
        End Get
        Set(ByVal Value As String)
            _superuser_codfiscale = Value
            Salva()
        End Set
    End Property
    Property superuser_username() As String
        Get
            Return _superuser_username
        End Get
        Set(ByVal Value As String)
            _superuser_username = Value
            Salva()
        End Set
    End Property
    Property data_stampa() As String
        Get
            Return _data_stampa
        End Get
        Set(ByVal Value As String)
            _data_stampa = Value
            Salva()
        End Set
    End Property



    Property report() As Integer
        Get
            Return _report
        End Get
        Set(ByVal Value As Integer)
            _report = Value
            Salva()
        End Set
    End Property

    Property user_profilo() As String
        Get
            Return _user_profilo
        End Get
        Set(ByVal Value As String)
            _user_profilo = Value
            Salva()
        End Set
    End Property


    Property Xml_Generico() As StringBuilder
        Get
            Return _Xml_Generico
        End Get
        Set(ByVal Value As StringBuilder)
            _Xml_Generico = Value
            Salva()
        End Set
    End Property

    Property JSon_Generico() As StringBuilder
        Get
            Return _JSon_Generico
        End Get
        Set(ByVal Value As StringBuilder)
            _JSon_Generico = Value
            Salva()
        End Set
    End Property

    Property Id_Sezione() As Integer
        Get
            Return _idSezione
        End Get
        Set(ByVal Value As Integer)
            _idSezione = Value
            Salva()
        End Set
    End Property

#End Region

#Region "X Sessione"
    Public Sub Salva()
        HttpContext.Current.Session("ParametriAgronicaStampe") = Me
    End Sub
    Public Sub Leggi()
        Dim obj As New ParametriAgronicaStampe
        obj = HttpContext.Current.Session("ParametriAgronicaStampe")
        If Not IsNothing(obj) Then
            _username = obj._username
            _report = obj._report
            _user_profilo = obj._user_profilo

            _Xml_Generico = obj._Xml_Generico
            _JSon_Generico = obj._JSon_Generico

            _UserProfilo_CodFisc = obj._UserProfilo_CodFisc
            _Sql_Filtro = obj._Sql_Filtro
            _Xml_Filtro = obj._Xml_Filtro

            _username_codfisc = obj._username_codfisc
            _user_profilo_codfisc = obj._user_profilo_codfisc
            _user_profilo_codgias = obj._user_profilo_codgias

            _utente_codfiscale = obj._utente_codfiscale
            _superuser_codfiscale = obj._superuser_codfiscale
            _superuser_username = obj._superuser_username
            _data_stampa = obj._data_stampa


            _centro_costo = obj._centro_costo
            _superficie = obj._superficie
            _ricavi = obj._ricavi
            _costi = obj._costi
            _differenza = obj._differenza
            _ricavi_ha = obj._ricavi_ha
            _costi_ha = obj._costi_ha
            _differenza_ha = obj._differenza_ha

            _ricavo_lordo = obj._ricavo_lordo
            _ricavo_totale = obj._ricavo_totale
            _costi_variabili = obj._costi_variabili
            _margine_lordo = obj._margine_lordo
            _costi_operativi = obj._costi_operativi
            _margine_operativo = obj._margine_operativo
            _costi_fissi = obj._costi_fissi
            _margine_netto = obj._margine_netto

            _visualizzazione = obj._visualizzazione

            _id_agenda = obj._id_agenda
            _idSezione = obj.Id_Sezione
        End If

    End Sub
#End Region

#Region "Costruttori"
    Public Sub New()
        _Xml_Generico = New StringBuilder
        _Xml_Generico.Length = 0
        _JSon_Generico = New StringBuilder
        _JSon_Generico.Length = 0
        _idSezione = 0
    End Sub



    Sub New(ByVal XML As String)
        LeggiXML(XML)
    End Sub

    Private Sub LeggiXML(ByVal XML As String)

        Dim XmlDoc As New System.Xml.XmlDocument
        XmlDoc.LoadXml(XML)

        Dim XML_ParametriAggiuntivi As System.Xml.XmlElement
        XML_ParametriAggiuntivi = XmlDoc.SelectSingleNode("Parametri").SelectSingleNode("FiltroStampa")

        If XML_ParametriAggiuntivi.HasAttribute(LCase("username")) = True Then
            Me.username = XML_ParametriAggiuntivi.GetAttribute(LCase("username"))
        End If

        If XML_ParametriAggiuntivi.HasAttribute(LCase("username_codfisc")) = True Then
            Me.username_codfisc = XML_ParametriAggiuntivi.GetAttribute(LCase("username_codfisc"))
        End If

        If XML_ParametriAggiuntivi.HasAttribute(LCase("user_profilo_codfisc")) = True Then
            Me.user_profilo_codfisc = XML_ParametriAggiuntivi.GetAttribute(LCase("user_profilo_codfisc"))
        End If

        If XML_ParametriAggiuntivi.HasAttribute(LCase("user_profilo_codgias")) = True Then
            Me.user_profilo_codgias = XML_ParametriAggiuntivi.GetAttribute(LCase("user_profilo_codgias"))
        End If

        If XML_ParametriAggiuntivi.HasAttribute(LCase("utente_codfiscale")) = True Then
            Me.utente_codfiscale = XML_ParametriAggiuntivi.GetAttribute(LCase("utente_codfiscale"))
        End If
        If XML_ParametriAggiuntivi.HasAttribute(LCase("superuser_codfiscale")) = True Then
            Me.superuser_codfiscale = XML_ParametriAggiuntivi.GetAttribute(LCase("superuser_codfiscale"))
        End If
        If XML_ParametriAggiuntivi.HasAttribute(LCase("superuser_username")) = True Then
            Me.superuser_username = XML_ParametriAggiuntivi.GetAttribute(LCase("superuser_username"))
        End If
        If XML_ParametriAggiuntivi.HasAttribute(LCase("data_stampa")) = True Then
            Me.data_stampa = XML_ParametriAggiuntivi.GetAttribute(LCase("data_stampa"))
        End If

        If XML_ParametriAggiuntivi.HasAttribute(LCase("user_profilo")) = True Then
            Me.user_profilo = XML_ParametriAggiuntivi.GetAttribute(LCase("user_profilo"))
        End If

        If XML_ParametriAggiuntivi.HasAttribute(LCase("report")) = True Then
            Me.report = XML_ParametriAggiuntivi.GetAttribute(LCase("report"))
        End If


        'no lcase per compatibilità gestione richieste stampe con titi vecchi
        If XML_ParametriAggiuntivi.HasAttribute("UserProfilo_CodFisc") = True Then
            Me.UserProfilo_CodFisc = XML_ParametriAggiuntivi.GetAttribute("UserProfilo_CodFisc")
        End If

        If XML_ParametriAggiuntivi.HasAttribute("Sql_Filtro") = True Then
            Me.Sql_Filtro = XML_ParametriAggiuntivi.GetAttribute("Sql_Filtro")
        End If

        If XML_ParametriAggiuntivi.HasAttribute("Xml_Filtro") = True Then
            Me.Xml_Filtro = XML_ParametriAggiuntivi.GetAttribute("Xml_Filtro")
        End If
        'fine


        If XML_ParametriAggiuntivi.HasAttribute("centro_costo") = True Then
            Me.centro_costo = XML_ParametriAggiuntivi.GetAttribute("centro_costo")
        End If
        If XML_ParametriAggiuntivi.HasAttribute("superficie") = True Then
            Me.superficie = XML_ParametriAggiuntivi.GetAttribute("superficie")
        End If
        If XML_ParametriAggiuntivi.HasAttribute("ricavi") = True Then
            Me.ricavi = XML_ParametriAggiuntivi.GetAttribute("ricavi")
        End If
        If XML_ParametriAggiuntivi.HasAttribute("costi") = True Then
            Me.costi = XML_ParametriAggiuntivi.GetAttribute("costi")
        End If
        If XML_ParametriAggiuntivi.HasAttribute("differenza") = True Then
            Me.differenza = XML_ParametriAggiuntivi.GetAttribute("differenza")
        End If
        If XML_ParametriAggiuntivi.HasAttribute("ricavi_ha") = True Then
            Me.ricavi_ha = XML_ParametriAggiuntivi.GetAttribute("ricavi_ha")
        End If
        If XML_ParametriAggiuntivi.HasAttribute("costi_ha") = True Then
            Me.costi_ha = XML_ParametriAggiuntivi.GetAttribute("costi_ha")
        End If
        If XML_ParametriAggiuntivi.HasAttribute("differenza_ha") = True Then
            Me.differenza_ha = XML_ParametriAggiuntivi.GetAttribute("differenza_ha")
        End If


        If XML_ParametriAggiuntivi.HasAttribute("ricavo_lordo") = True Then
            Me.ricavo_lordo = XML_ParametriAggiuntivi.GetAttribute("ricavo_lordo")
        End If
        If XML_ParametriAggiuntivi.HasAttribute("ricavo_totale") = True Then
            Me.ricavo_totale = XML_ParametriAggiuntivi.GetAttribute("ricavo_totale")
        End If
        If XML_ParametriAggiuntivi.HasAttribute("costi_variabili") = True Then
            Me.costi_variabili = XML_ParametriAggiuntivi.GetAttribute("costi_variabili")
        End If
        If XML_ParametriAggiuntivi.HasAttribute("margine_lordo") = True Then
            Me.margine_lordo = XML_ParametriAggiuntivi.GetAttribute("margine_lordo")
        End If
        If XML_ParametriAggiuntivi.HasAttribute("costi_operativi") = True Then
            Me.costi_operativi = XML_ParametriAggiuntivi.GetAttribute("costi_operativi")
        End If
        If XML_ParametriAggiuntivi.HasAttribute("margine_operativo") = True Then
            Me.margine_operativo = XML_ParametriAggiuntivi.GetAttribute("margine_operativo")
        End If
        If XML_ParametriAggiuntivi.HasAttribute("costi_fissi") = True Then
            Me.costi_fissi = XML_ParametriAggiuntivi.GetAttribute("costi_fissi")
        End If
        If XML_ParametriAggiuntivi.HasAttribute("margine_netto") = True Then
            Me.margine_netto = XML_ParametriAggiuntivi.GetAttribute("margine_netto")
        End If

        If XML_ParametriAggiuntivi.HasAttribute("visualizzazione") = True Then
            Me.visualizzazione = XML_ParametriAggiuntivi.GetAttribute("visualizzazione")
        End If

        If XML_ParametriAggiuntivi.HasAttribute("id_agenda") = True Then
            Me.Id_Agenda = XML_ParametriAggiuntivi.GetAttribute("id_agenda")
        End If

        If XML_ParametriAggiuntivi.HasAttribute("id_sezione") = True Then
            Me.Id_Agenda = XML_ParametriAggiuntivi.GetAttribute("id_sezione")
        End If


        If XmlDoc.SelectSingleNode("Parametri").SelectSingleNode("FiltroStampa").ChildNodes.Count > 0 Then
            Dim StrNodiVariabili As String
            Dim i As Integer = 0
            For i = 0 To XmlDoc.SelectSingleNode("Parametri").SelectSingleNode("FiltroStampa").ChildNodes.Count - 1
                StrNodiVariabili &= XmlDoc.SelectSingleNode("Parametri").SelectSingleNode("FiltroStampa").ChildNodes.Item(i).OuterXml
            Next
            Me.Xml_Generico = New StringBuilder
            Me.Xml_Generico.Length = 0
            Me.Xml_Generico.Append(StrNodiVariabili)
        Else
            'lo inizializzo comunque altrimenbti se mando una stampa come la stampa risultato filtro che non ha 
            'nodi si incricca
            Me.Xml_Generico = New StringBuilder
            Me.Xml_Generico.Length = 0
        End If

        Me.JSon_Generico = New StringBuilder
        Me.JSon_Generico.Length = 0

    End Sub

#End Region


    ''' <summary>
    ''' Genera il blocco XML VariabiliAnalisi con all'interno 
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GeneraXML() As String
        Dim XmlDoc As New System.Xml.XmlDocument
        Dim Xml_VariabiliGias As System.Xml.XmlElement
        'Creo il nodo "FiltroStampa"
        Xml_VariabiliGias = XmlDoc.CreateElement("FiltroStampa")
        Xml_VariabiliGias.SetAttribute("username", username)
        Xml_VariabiliGias.SetAttribute("report", report)
        Xml_VariabiliGias.SetAttribute("user_profilo", user_profilo)

        'no lcase per compatibilità vecchie chiamate e gestione richieste
        Xml_VariabiliGias.SetAttribute("UserProfilo_CodFisc", UserProfilo_CodFisc)

        'uso i parametri e non le proprieta per fare in modo che non sostituisca i percentuali e apici
        'no lcase per compatibilità vecchie chiamate e gestione richieste
        Xml_VariabiliGias.SetAttribute("Sql_Filtro", _Sql_Filtro)
        Xml_VariabiliGias.SetAttribute("Xml_Filtro", _Xml_Filtro)

        Xml_VariabiliGias.SetAttribute("username_codfisc", username_codfisc)
        Xml_VariabiliGias.SetAttribute("user_profilo_codfisc", user_profilo_codfisc)
        Xml_VariabiliGias.SetAttribute("user_profilo_codgias", user_profilo_codgias)

        Xml_VariabiliGias.SetAttribute("utente_codfiscale", utente_codfiscale)
        Xml_VariabiliGias.SetAttribute("superuser_codfiscale", superuser_codfiscale)
        Xml_VariabiliGias.SetAttribute("superuser_username", superuser_username)
        Xml_VariabiliGias.SetAttribute("data_stampa", data_stampa)


        Xml_VariabiliGias.SetAttribute("centro_costo", centro_costo)
        Xml_VariabiliGias.SetAttribute("superficie", superficie)
        Xml_VariabiliGias.SetAttribute("ricavi", ricavi)
        Xml_VariabiliGias.SetAttribute("costi", costi)
        Xml_VariabiliGias.SetAttribute("differenza", differenza)
        Xml_VariabiliGias.SetAttribute("ricavi_ha", ricavi_ha)
        Xml_VariabiliGias.SetAttribute("costi_ha", costi_ha)
        Xml_VariabiliGias.SetAttribute("differenza_ha", differenza_ha)


        Xml_VariabiliGias.SetAttribute("ricavo_lordo", ricavo_lordo)
        Xml_VariabiliGias.SetAttribute("ricavo_totale", ricavo_totale)
        Xml_VariabiliGias.SetAttribute("costi_variabili", costi_variabili)
        Xml_VariabiliGias.SetAttribute("margine_lordo", margine_lordo)
        Xml_VariabiliGias.SetAttribute("costi_operativi", costi_operativi)
        Xml_VariabiliGias.SetAttribute("margine_operativo", margine_operativo)
        Xml_VariabiliGias.SetAttribute("costi_fissi", costi_fissi)
        Xml_VariabiliGias.SetAttribute("margine_netto", margine_netto)

        Xml_VariabiliGias.SetAttribute("visualizzazione", visualizzazione)

        Xml_VariabiliGias.SetAttribute("id_agenda", Id_Agenda)

        If Xml_Generico.Length > 0 Then
            Dim strxml As String = Xml_Generico.ToString
            strxml = strxml.Replace("'", "&#39;")
            Xml_VariabiliGias.InnerXml = strxml
        End If

        XmlDoc.AppendChild(Xml_VariabiliGias)
        Dim str As String
        str = XmlDoc.InnerXml
        str = str.Replace("'", "&#39;")
        Return str
    End Function


End Class





'#################################################################################################
''' <summary>
''' AgronicaStampe
''' </summary>
''' <remarks></remarks>
Public Class ParametriSementieri


    Private _parametroVuoto As String



#Region "Proprietà"

    Property parametroVuoto() As String
        Get
            Return _parametroVuoto
        End Get
        Set(ByVal Value As String)
            _parametroVuoto = Value
            Salva()
        End Set
    End Property

#End Region

#Region "X Sessione"
    Public Sub Salva()
        HttpContext.Current.Session("ParametriSementieri") = Me
    End Sub
    Public Sub Leggi()
        Dim obj As New ParametriSementieri
        obj = HttpContext.Current.Session("ParametriSementieri")
        If Not IsNothing(obj) Then
            parametroVuoto = obj.parametroVuoto
        End If

    End Sub
#End Region

#Region "Costruttori"
    Public Sub New()
        parametroVuoto = ""
    End Sub

    Sub New(ByVal XML As String)
        LeggiXML(XML)
    End Sub

    Private Sub LeggiXML(ByVal XML As String)

        Dim XmlDoc As New System.Xml.XmlDocument
        XmlDoc.LoadXml(XML)

        Dim XML_ParametriAggiuntivi As System.Xml.XmlElement
        XML_ParametriAggiuntivi = XmlDoc.SelectSingleNode("Parametri").SelectSingleNode("ParametriSementieri")

        If XML_ParametriAggiuntivi.HasAttribute(LCase("parametroVuoto")) = True Then
            Me.parametroVuoto = XML_ParametriAggiuntivi.GetAttribute(LCase("parametroVuoto"))
        End If

    End Sub

#End Region


    ''' <summary>
    ''' Genera il blocco XML VariabiliAnalisi con all'interno 
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GeneraXML() As String
        Dim XmlDoc As New System.Xml.XmlDocument
        Dim Xml_VariabiliPDC As System.Xml.XmlElement
        'Creo il nodo "FiltroStampa"
        Xml_VariabiliPDC = XmlDoc.CreateElement("ParametriSementieri")
        Xml_VariabiliPDC.SetAttribute("parametrovuoto", parametroVuoto)


        XmlDoc.AppendChild(Xml_VariabiliPDC)
        Dim str As String
        str = XmlDoc.InnerXml
        str = str.Replace("'", "&#39;")
        Return str
    End Function


End Class













'#################################################################################################
''' <summary>
''' PIANI CAMPIONAMENTO 2010
''' </summary>
''' <remarks></remarks>
Public Class ParametriPianidiCampionamento_2010

    Private _PaginaRichiesta As String
    Private _ListaImpianti() As Impianto

    Private _Id_PDC_Testata As Integer
    Private _Piva As String
    Private _RagSoc As String = ""

#Region "Property"
    Public Property PaginaRichiesta() As String
        Get
            Return _PaginaRichiesta
        End Get
        Set(ByVal value As String)
            _PaginaRichiesta = value
            Salva()
        End Set
    End Property
    Public Property ListaImpianti() As Impianto()
        Get
            Return _ListaImpianti
        End Get
        Set(ByVal value As Impianto())
            _ListaImpianti = value
            Salva()
        End Set
    End Property

    Public Property Id_PDC_Testata() As Integer
        Get
            Return _Id_PDC_Testata
        End Get
        Set(ByVal value As Integer)
            _Id_PDC_Testata = value
            Salva()
        End Set
    End Property

    Public Property Piva() As String
        Get
            Return _Piva
        End Get
        Set(ByVal value As String)
            _Piva = value
            Salva()
        End Set
    End Property

    Public Property RagSoc() As String
        Get
            Return _RagSoc
        End Get
        Set(ByVal value As String)
            _RagSoc = value
            Salva()
        End Set
    End Property

#End Region

#Region "X Sessione"
    Public Sub Salva()
        HttpContext.Current.Session("ParametriPianidiCampionamento_2010") = Me
    End Sub
    Public Sub Leggi()
        Dim obj As New ParametriPianidiCampionamento_2010
        obj = HttpContext.Current.Session("ParametriPianidiCampionamento_2010")
        If Not IsNothing(obj) Then
            PaginaRichiesta = obj.PaginaRichiesta
            ListaImpianti = obj.ListaImpianti
            Id_PDC_Testata = obj.Id_PDC_Testata
            Piva = obj.Piva
            RagSoc = obj.RagSoc
        End If
    End Sub
#End Region

#Region "Costruttori"
    Public Sub New()

    End Sub

    ''' <summary>
    ''' mi inizializza l'oggetto dall'XML passato
    ''' </summary>
    ''' <param name="XML"></param>
    ''' <remarks></remarks>
    Sub New(ByVal XML As String)
        LeggiXML(XML)
    End Sub

    Private Sub LeggiXML(ByVal XML As String)
        Dim XmlDoc As New System.Xml.XmlDocument
        XmlDoc.LoadXml(XML)

        Dim XML_ParametriAggiuntivi As System.Xml.XmlElement
        XML_ParametriAggiuntivi = XmlDoc.SelectSingleNode("Parametri").SelectSingleNode("ParametriPianidiCampionamento_2010")
        'pagina richiesta
        If XML_ParametriAggiuntivi.HasAttribute(LCase("paginarichiesta")) = True Then
            Me.PaginaRichiesta = XML_ParametriAggiuntivi.GetAttribute(LCase("paginarichiesta"))
        End If

        'pagina richiesta
        If XML_ParametriAggiuntivi.HasAttribute(LCase("Id_PDC_Testata")) = True Then
            Me.Id_PDC_Testata = XML_ParametriAggiuntivi.GetAttribute(LCase("Id_PDC_Testata"))
        End If

        'piva
        If XML_ParametriAggiuntivi.HasAttribute(LCase("Piva")) = True Then
            Me.Piva = XML_ParametriAggiuntivi.GetAttribute(LCase("Piva"))
        End If

        'VariabiliPianiCampionamento
        'Lista degli impianti
        Dim list As XmlNodeList = XmlDoc.SelectSingleNode("Parametri").SelectNodes("ParametriPianidiCampionamento_2010")(0).ChildNodes()
        If list.Count > 0 Then
            Dim i As Integer
            'ridimensiono l'array
            ReDim ListaImpianti(list.Count - 1)
            Dim VarPC As XmlElement
            For Each VarPC In list
                Dim impianto As New Impianto
                impianto.Piva = VarPC.Attributes("p").Value
                impianto.Rag_Soc = VarPC.Attributes("rs").Value
                impianto.Sa_Cod = VarPC.Attributes("s").Value
                impianto.Sa_Nome = VarPC.Attributes("sn").Value
                impianto.Appezza = VarPC.Attributes("a").Value
                impianto.App_Nome = VarPC.Attributes("an").Value
                impianto.Id_Reg = VarPC.Attributes("ir").Value
                impianto.Veg_Cod = VarPC.Attributes("vc").Value
                impianto.Veg_Des = VarPC.Attributes("vd").Value
                impianto.Cul_Cod = VarPC.Attributes("cc").Value
                impianto.Cul_Des = VarPC.Attributes("cd").Value
                impianto.Sup_Imp = VarPC.Attributes("si").Value
                impianto.Data_Raccolta = AGRODATAFINE
                'lo aggiungo
                ListaImpianti(i) = impianto
                i = i + 1
            Next
            HttpContext.Current.Session("_AggiungiImpianti") = "true"
        Else
            HttpContext.Current.Session("_AggiungiImpianti") = "false"
        End If

    End Sub
#End Region


    ''' <summary>
    ''' Genera il blocco XML VariabiliAnalisi con all'interno 
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GeneraXML() As String
        Dim XmlDoc As New System.Xml.XmlDocument
        Dim Xml_VariabiliPDC As System.Xml.XmlElement
        'Creo il nodo "FiltroStampa"
        Xml_VariabiliPDC = XmlDoc.CreateElement("ParametriPianidiCampionamento_2010")
        Xml_VariabiliPDC.SetAttribute("paginarichiesta", PaginaRichiesta)
        Xml_VariabiliPDC.SetAttribute(LCase("Id_PDC_Testata"), Id_PDC_Testata)
        Xml_VariabiliPDC.SetAttribute(LCase("Piva"), Piva)

        'aggiungo gli impianti 
        Dim i As Integer
        If Not IsNothing(ListaImpianti) Then
            For i = 0 To ListaImpianti.Length - 1
                Dim VarPc As XmlElement
                VarPc = XmlDoc.CreateElement("VarPC")

                VarPc.SetAttribute("p", ListaImpianti(i).Piva.Replace("'", "&#39;"))
                VarPc.SetAttribute("rs", ListaImpianti(i).Rag_Soc.Replace("'", "&#39;"))
                VarPc.SetAttribute("s", ListaImpianti(i).Sa_Cod.ToString.Replace("'", "&#39;"))
                VarPc.SetAttribute("sn", ListaImpianti(i).Sa_Nome.Replace("'", "&#39;"))
                VarPc.SetAttribute("a", ListaImpianti(i).Appezza.ToString.Replace("'", "&#39;"))
                VarPc.SetAttribute("an", ListaImpianti(i).App_Nome.Replace("'", "&#39;"))
                VarPc.SetAttribute("ir", ListaImpianti(i).Id_Reg.ToString.Replace("'", "&#39;"))
                VarPc.SetAttribute("vc", ListaImpianti(i).Veg_Cod.ToString.Replace("'", "&#39;"))
                VarPc.SetAttribute("vd", ListaImpianti(i).Veg_Des.Replace("'", "&#39;"))

                VarPc.SetAttribute("cc", ListaImpianti(i).Cul_Cod.ToString.Replace("'", "&#39;"))
                VarPc.SetAttribute("cd", ListaImpianti(i).Cul_Des.Replace("'", "&#39;"))
                VarPc.SetAttribute("si", ListaImpianti(i).Sup_Imp.Replace("'", "&#39;"))

                Xml_VariabiliPDC.AppendChild(VarPc)
            Next

        End If

        XmlDoc.AppendChild(Xml_VariabiliPDC)
        Dim str As String
        str = XmlDoc.InnerXml
        str = str.Replace("'", "&#39;")
        Return str
    End Function


    Public Sub AddList(ByVal _Impianto As Impianto)
        'ridimensiono 
        If IsNothing(ListaImpianti) Then
            ListaImpianti = New Impianto(0) {}
        Else
            ReDim Preserve ListaImpianti(ListaImpianti.Length)
        End If
        ListaImpianti(ListaImpianti.Length - 1) = _Impianto
    End Sub

End Class



'#################################################################################################
''' <summary>
''' ANALISI 2010
''' </summary>
''' <remarks></remarks>
Public Class ParametriAnalisi_2010
    Private _Tipo_Analisi As Integer
    Private _Piva As String
    Private _Sa_Cod As Integer
    Private _Analisi_Testata_Cod As Integer
    Private _ID_PDC_Campione As Integer
    Private _ID_PDC_Dettagli As Integer
    Private _ID_PDC_Testata As Integer
    Private _Pagina_Richiesta As Integer 'tipo enumerativo enum_PagineAnalisi_2010
    Private _Tipo_Operazione As Integer 'Inserimento 1 - Visualizzazione 2
    Private _SitoOrigine As Integer
    Private _Pagina_SitoOrigine As Integer
    Private _Veg_Cod As Integer

    Private _Sessione As Boolean
    Private _RagSoc As String = ""

#Region "Proprietà"

    Public Property Sessione() As Boolean
        Get
            Return _Sessione
        End Get
        Set(ByVal value As Boolean)
            _Sessione = value
        End Set
    End Property

    Public Property Tipo_Operazione() As Integer
        Get
            Return _Tipo_Operazione
        End Get
        Set(ByVal value As Integer)
            _Tipo_Operazione = value
            Salva()
        End Set
    End Property

    Public Property Pagina_Richiesta() As Integer
        Get
            Return _Pagina_Richiesta
        End Get
        Set(ByVal value As Integer)
            _Pagina_Richiesta = value
            Salva()
        End Set
    End Property
    Public Property Analisi_Testata_Cod() As Integer
        Get
            Return _Analisi_Testata_Cod
        End Get
        Set(ByVal value As Integer)
            _Analisi_Testata_Cod = value
            Salva()
        End Set
    End Property
    Public Property ID_PDC_Campione() As Integer
        Get
            Return _ID_PDC_Campione
        End Get
        Set(ByVal value As Integer)
            _ID_PDC_Campione = value
            Salva()
        End Set
    End Property
    Public Property ID_PDC_Dettagli() As Integer
        Get
            Return _ID_PDC_Dettagli
        End Get
        Set(ByVal value As Integer)
            _ID_PDC_Dettagli = value
            Salva()
        End Set
    End Property
    Public Property ID_PDC_Testata() As Integer
        Get
            Return _ID_PDC_Testata
        End Get
        Set(ByVal value As Integer)
            _ID_PDC_Testata = value
            Salva()
        End Set
    End Property
    Public Property Piva() As String
        Get
            Return _Piva
        End Get
        Set(ByVal value As String)
            _Piva = value
            Salva()
        End Set
    End Property
    Public Property Sa_Cod() As Integer
        Get
            Return _Sa_Cod
        End Get
        Set(ByVal value As Integer)
            _Sa_Cod = value
            Salva()
        End Set
    End Property
    Public Property Veg_Cod() As Integer
        Get
            Return _Veg_Cod
        End Get
        Set(ByVal value As Integer)
            _Veg_Cod = value
            Salva()
        End Set
    End Property
    Public Property Tipo_Analisi() As Integer
        Get
            Return _Tipo_Analisi
        End Get
        Set(ByVal value As Integer)
            _Tipo_Analisi = value
            Salva()
        End Set
    End Property
    Public Property SitoOrigine() As Integer
        Get
            Return _SitoOrigine
        End Get
        Set(ByVal value As Integer)
            _SitoOrigine = value
            Salva()
        End Set
    End Property
    Public Property Pagina_SitoOrigine() As Integer
        Get
            Return _Pagina_SitoOrigine
        End Get
        Set(ByVal value As Integer)
            _Pagina_SitoOrigine = value
            Salva()
        End Set
    End Property

    Public Property RagSoc() As String
        Get
            Return _RagSoc
        End Get
        Set(ByVal value As String)
            _RagSoc = value
            Salva()
        End Set
    End Property

#End Region

#Region "Costruttori"

    Sub New()
        _Sessione = True
    End Sub


    Sub New(ByVal XML As String)
        _Sessione = True
        LeggiXML(XML)
    End Sub

    Private Sub LeggiXML(ByVal XML As String)
        Dim XmlDoc As New System.Xml.XmlDocument
        XmlDoc.LoadXml(XML)

        Dim XML_ParametriAggiuntivi As System.Xml.XmlElement
        XML_ParametriAggiuntivi = XmlDoc.SelectSingleNode("Parametri").SelectSingleNode("ParametriAnalisi_2010")
        If XML_ParametriAggiuntivi.HasAttribute(LCase("tipo_analisi")) = True Then
            Me.Tipo_Analisi = XML_ParametriAggiuntivi.GetAttribute(LCase("tipo_analisi"))
        End If
        If XML_ParametriAggiuntivi.HasAttribute(LCase("piva")) = True Then
            Me.Piva = XML_ParametriAggiuntivi.GetAttribute(LCase("piva"))
        End If
        If XML_ParametriAggiuntivi.HasAttribute(LCase("sa_cod")) = True Then
            Me.Sa_Cod = XML_ParametriAggiuntivi.GetAttribute(LCase("sa_cod"))
        End If
        If XML_ParametriAggiuntivi.HasAttribute(LCase("veg_cod")) = True Then
            Me.Veg_Cod = XML_ParametriAggiuntivi.GetAttribute(LCase("veg_cod"))
        End If
        ''aggiunta per il passaggio dal piano di campionamento
        If XML_ParametriAggiuntivi.HasAttribute(LCase("analisi_testata_cod")) = True Then
            Me.Analisi_Testata_Cod = XML_ParametriAggiuntivi.GetAttribute(LCase("analisi_testata_cod"))
        End If
        If XML_ParametriAggiuntivi.HasAttribute(LCase("id_pdc_campione")) = True Then
            Me.ID_PDC_Campione = XML_ParametriAggiuntivi.GetAttribute(LCase("id_pdc_campione"))
        End If
        If XML_ParametriAggiuntivi.HasAttribute(LCase("id_pdc_dettagli")) = True Then
            Me.ID_PDC_Dettagli = XML_ParametriAggiuntivi.GetAttribute(LCase("id_pdc_dettagli"))
        End If
        If XML_ParametriAggiuntivi.HasAttribute(LCase("id_pdc_testata")) = True Then
            Me.ID_PDC_Testata = XML_ParametriAggiuntivi.GetAttribute(LCase("id_pdc_testata"))
        End If

        If XML_ParametriAggiuntivi.HasAttribute(LCase("pagina_richiesta")) = True Then
            Me.Pagina_Richiesta = XML_ParametriAggiuntivi.GetAttribute(LCase("pagina_richiesta"))
        End If

        If XML_ParametriAggiuntivi.HasAttribute(LCase("tipo_operazione")) = True Then
            Me.Tipo_Operazione = XML_ParametriAggiuntivi.GetAttribute(LCase("tipo_operazione"))
        End If

        If XML_ParametriAggiuntivi.HasAttribute(LCase("sito_origine")) = True Then
            Me._SitoOrigine = XML_ParametriAggiuntivi.GetAttribute(LCase("sito_origine"))
        End If

        If XML_ParametriAggiuntivi.HasAttribute(LCase("pagina_sito_origine")) = True Then
            Me._Pagina_SitoOrigine = XML_ParametriAggiuntivi.GetAttribute(LCase("pagina_sito_origine"))
        End If

    End Sub
#End Region
#Region "x Sessione"
    Public Sub Salva()
        If _Sessione Then
            HttpContext.Current.Session("ParametriAnalisi_2010") = Me
        End If
    End Sub
    Public Sub Leggi()
        If _Sessione Then
            Dim obj As New ParametriAnalisi_2010
            obj = HttpContext.Current.Session("ParametriAnalisi_2010")

            If Not IsNothing(obj) Then
                _Tipo_Analisi = obj.Tipo_Analisi
                _Piva = obj.Piva
                _Sa_Cod = obj.Sa_Cod
                _Veg_Cod = obj.Veg_Cod
                _Analisi_Testata_Cod = obj.Analisi_Testata_Cod
                _ID_PDC_Campione = obj.ID_PDC_Campione
                _ID_PDC_Dettagli = obj.ID_PDC_Dettagli
                _ID_PDC_Testata = obj.ID_PDC_Testata
                _Pagina_Richiesta = obj.Pagina_Richiesta
                _Tipo_Operazione = obj.Tipo_Operazione
                _SitoOrigine = obj.SitoOrigine
                _Pagina_SitoOrigine = obj.Pagina_SitoOrigine
                _RagSoc = obj.RagSoc

            End If
        End If
    End Sub
#End Region

    ''' <summary>
    ''' Genera il blocco XML VariabiliAnalisi con all'interno 
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GeneraXML() As String
        Dim XmlDoc As New System.Xml.XmlDocument
        Dim Xml_VariabiliAnalisi As System.Xml.XmlElement
        'Creo il nodo "FiltroStampa"
        Xml_VariabiliAnalisi = XmlDoc.CreateElement("ParametriAnalisi_2010")

        Xml_VariabiliAnalisi.SetAttribute("tipo_analisi", CStr(Tipo_Analisi))
        Xml_VariabiliAnalisi.SetAttribute("piva", CStr(Piva))
        Xml_VariabiliAnalisi.SetAttribute("analisi_testata_cod", CStr(Analisi_Testata_Cod))
        Xml_VariabiliAnalisi.SetAttribute("id_pdc_campione", CStr(ID_PDC_Campione))
        Xml_VariabiliAnalisi.SetAttribute("id_pdc_dettagli", CStr(ID_PDC_Dettagli))
        Xml_VariabiliAnalisi.SetAttribute("id_pdc_testata", CStr(ID_PDC_Testata))
        Xml_VariabiliAnalisi.SetAttribute("pagina_richiesta", CStr(Pagina_Richiesta))
        Xml_VariabiliAnalisi.SetAttribute("tipo_operazione", CStr(Tipo_Operazione))
        Xml_VariabiliAnalisi.SetAttribute("sito_origine", CStr(SitoOrigine))
        Xml_VariabiliAnalisi.SetAttribute("pagina_sito_origine", CStr(Pagina_SitoOrigine))
        Xml_VariabiliAnalisi.SetAttribute("veg_cod", CStr(Veg_Cod))
        Xml_VariabiliAnalisi.SetAttribute("ragsoc", CStr(RagSoc))

        XmlDoc.AppendChild(Xml_VariabiliAnalisi)
        Dim str As String
        str = XmlDoc.InnerXml
        str = str.Replace("'", "&#39;")
        Return str
    End Function
End Class

Public Class ParametriAnalisiTerrenoNG
    Private _Tipo_Analisi As Integer
    Private _Piva As String
    Private _RagSoc As String = ""
    Private _Sa_Cod As Integer
    Private _Analisi_Testata_Cod As Integer
    Private _ID_PDC_Campione As Integer
    Private _ID_PDC_Dettagli As Integer
    Private _ID_PDC_Testata As Integer
    Private _Pagina_Richiesta As Integer 'tipo enumerativo enum_PagineAnalisi_2010
    Private _Tipo_Operazione As Integer 'Inserimento 1 - Visualizzazione 2
    Private _SitoOrigine As Enum_SiteRedirector
    Private _paginaProvenienza As Integer
    Private _Pagina_SitoOrigine As Integer
    Private _Veg_Cod As Integer

#Region "Proprietà"

    Public Property Analisi_Testata_Cod() As Integer
        Get
            Return _Analisi_Testata_Cod
        End Get
        Set(ByVal value As Integer)
            _Analisi_Testata_Cod = value
        End Set
    End Property
    Public Property Tipo_Analisi() As Integer
        Get
            Return _Tipo_Analisi
        End Get
        Set(ByVal value As Integer)
            _Tipo_Analisi = value
        End Set
    End Property
    Public Property Tipo_Operazione() As Integer
        Get
            Return _Tipo_Operazione
        End Get
        Set(ByVal value As Integer)
            _Tipo_Operazione = value
        End Set
    End Property
    Public Property ID_PDC_Campione() As Integer
        Get
            Return _ID_PDC_Campione
        End Get
        Set(ByVal value As Integer)
            _ID_PDC_Campione = value
        End Set
    End Property
    Public Property ID_PDC_Dettagli() As Integer
        Get
            Return _ID_PDC_Dettagli
        End Get
        Set(ByVal value As Integer)
            _ID_PDC_Dettagli = value
        End Set
    End Property
    Public Property ID_PDC_Testata() As Integer
        Get
            Return _ID_PDC_Testata
        End Get
        Set(ByVal value As Integer)
            _ID_PDC_Testata = value
        End Set
    End Property
    Public Property Pagina_Richiesta() As Integer
        Get
            Return _Pagina_Richiesta
        End Get
        Set(ByVal value As Integer)
            _Pagina_Richiesta = value
        End Set
    End Property
    Public Property Piva() As String
        Get
            Return _Piva
        End Get
        Set(ByVal value As String)
            _Piva = value
        End Set
    End Property
    Public Property Sa_Cod() As Integer
        Get
            Return _Sa_Cod
        End Get
        Set(ByVal value As Integer)
            _Sa_Cod = value
        End Set
    End Property
    Public Property Veg_Cod() As Integer
        Get
            Return _Veg_Cod
        End Get
        Set(ByVal value As Integer)
            _Veg_Cod = value
        End Set
    End Property
    Public Property SitoOrigine() As Integer
        Get
            Return _SitoOrigine
        End Get
        Set(ByVal value As Integer)
            _SitoOrigine = value
        End Set
    End Property
    Public Property PaginaProvenienza() As Integer
        Get
            Return _paginaProvenienza
        End Get
        Set(ByVal value As Integer)
            _paginaProvenienza = value
        End Set
    End Property
    Public Property Pagina_SitoOrigine() As Integer
        Get
            Return _Pagina_SitoOrigine
        End Get
        Set(ByVal value As Integer)
            _Pagina_SitoOrigine = value
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

#End Region

End Class


'#################################################################################################
''' <summary>
''' CONCIMAZIONE 2017
''' </summary>
''' <remarks></remarks>
Public Class ParametriConcimazione_2017
    Private _Tipo_Concimazione As Integer
    Private _Piva As String
    Private _RagSoc As String
    Private _Sa_Cod As Integer
    Private _PianoConcimazione_Testata_Cod As Integer
    Private _Pagina_Richiesta As Integer 'tipo enumerativo enum_ParametriConcimazione_2008
    Private _Tipo_Operazione As Integer 'Inserimento 1 - Visualizzazione 2
    Private _SitoOrigine As Integer
    Private _Pagina_SitoOrigine As Integer
    Private _Veg_Cod As Integer
    Private _ListaImpianti() As Impianto

    Private _Sessione As Boolean

#Region "Proprietà"

    Public Property Sessione() As Boolean
        Get
            Return _Sessione
        End Get
        Set(value As Boolean)
            _Sessione = value
        End Set
    End Property

    Public Property Tipo_Operazione() As Integer
        Get
            Return _Tipo_Operazione
        End Get
        Set(ByVal value As Integer)
            _Tipo_Operazione = value
            Salva()
        End Set
    End Property
    Public Property Pagina_Richiesta() As Integer
        Get
            Return _Pagina_Richiesta
        End Get
        Set(ByVal value As Integer)
            _Pagina_Richiesta = value
            Salva()
        End Set
    End Property
    Public Property PianoConcimazione_Testata_Cod() As Integer
        Get
            Return _PianoConcimazione_Testata_Cod
        End Get
        Set(ByVal value As Integer)
            _PianoConcimazione_Testata_Cod = value
            Salva()
        End Set
    End Property
    Public Property Piva() As String
        Get
            Return _Piva
        End Get
        Set(ByVal value As String)
            _Piva = value
            Salva()
        End Set
    End Property
    Public Property Sa_Cod() As Integer
        Get
            Return _Sa_Cod
        End Get
        Set(ByVal value As Integer)
            _Sa_Cod = value
            Salva()
        End Set
    End Property
    Public Property Veg_Cod() As Integer
        Get
            Return _Veg_Cod
        End Get
        Set(ByVal value As Integer)
            _Veg_Cod = value
            Salva()
        End Set
    End Property
    Public Property Tipo_Concimazione() As Integer
        Get
            Return _Tipo_Concimazione
        End Get
        Set(ByVal value As Integer)
            _Tipo_Concimazione = value
            Salva()
        End Set
    End Property
    Public Property SitoOrigine() As Integer
        Get
            Return _SitoOrigine
        End Get
        Set(ByVal value As Integer)
            _SitoOrigine = value
            Salva()
        End Set
    End Property
    Public Property Pagina_SitoOrigine() As Integer
        Get
            Return _Pagina_SitoOrigine
        End Get
        Set(ByVal value As Integer)
            _Pagina_SitoOrigine = value
            Salva()
        End Set
    End Property

    Public Property ListaImpianti() As Impianto()
        Get
            Return _ListaImpianti
        End Get
        Set(ByVal value As Impianto())
            _ListaImpianti = value
            Salva()
        End Set
    End Property

    Property RagSoc() As String
        Get
            Return _RagSoc
        End Get
        Set(ByVal Value As String)
            _RagSoc = Value
            Salva()
        End Set
    End Property

#End Region

#Region "Costruttori"

    Sub New()
        _Sessione = True
    End Sub



    Sub New(ByVal XML As String)
        _Sessione = True
        LeggiXML(XML)
    End Sub

    Private Sub LeggiXML(ByVal XML As String)
        Dim XmlDoc As New System.Xml.XmlDocument
        XmlDoc.LoadXml(XML)

        Dim XML_ParametriAggiuntivi As System.Xml.XmlElement
        XML_ParametriAggiuntivi = XmlDoc.SelectSingleNode("Parametri").SelectSingleNode("ParametriConcimazione_2017")
        If XML_ParametriAggiuntivi.HasAttribute(LCase("tipo_concimazione")) = True Then
            Me.Tipo_Concimazione = XML_ParametriAggiuntivi.GetAttribute(LCase("tipo_concimazione"))
        End If
        If XML_ParametriAggiuntivi.HasAttribute(LCase("piva")) = True Then
            Me.Piva = XML_ParametriAggiuntivi.GetAttribute(LCase("piva"))
        End If
        If XML_ParametriAggiuntivi.HasAttribute(LCase("sa_cod")) = True Then
            Me.Sa_Cod = XML_ParametriAggiuntivi.GetAttribute(LCase("sa_cod"))
        End If
        If XML_ParametriAggiuntivi.HasAttribute(LCase("veg_cod")) = True Then
            Me.Veg_Cod = XML_ParametriAggiuntivi.GetAttribute(LCase("veg_cod"))
        End If
        ''aggiunta per il passaggio dal piano di campionamento
        If XML_ParametriAggiuntivi.HasAttribute(LCase("concimazione_testata_cod")) = True Then
            Me.PianoConcimazione_Testata_Cod = XML_ParametriAggiuntivi.GetAttribute(LCase("concimazione_testata_cod"))
        End If

        If XML_ParametriAggiuntivi.HasAttribute(LCase("pagina_richiesta")) = True Then
            Me.Pagina_Richiesta = XML_ParametriAggiuntivi.GetAttribute(LCase("pagina_richiesta"))
        End If

        If XML_ParametriAggiuntivi.HasAttribute(LCase("tipo_operazione")) = True Then
            Me.Tipo_Operazione = XML_ParametriAggiuntivi.GetAttribute(LCase("tipo_operazione"))
        End If

        If XML_ParametriAggiuntivi.HasAttribute(LCase("sito_origine")) = True Then
            Me._SitoOrigine = XML_ParametriAggiuntivi.GetAttribute(LCase("sito_origine"))
        End If

        If XML_ParametriAggiuntivi.HasAttribute(LCase("pagina_sito_origine")) = True Then
            Me._Pagina_SitoOrigine = XML_ParametriAggiuntivi.GetAttribute(LCase("pagina_sito_origine"))
        End If



        'VariabiliAnalisiCosti
        'Lista degli impianti
        Dim list As XmlNodeList = XmlDoc.SelectSingleNode("Parametri").SelectNodes("ParametriConcimazione_2017")(0).ChildNodes()
        If list.Count > 0 Then
            Dim i As Integer
            'ridimensiono l'array
            ReDim ListaImpianti(list.Count - 1)
            Dim VarAC As XmlElement
            For Each VarAC In list
                Dim impianto As New Impianto
                impianto.Piva = VarAC.Attributes("piva").Value
                impianto.Sa_Cod = VarAC.Attributes("sa_cod").Value
                impianto.Appezza = VarAC.Attributes("appezza").Value
                impianto.Id_Reg = VarAC.Attributes("id_reg").Value
                impianto.Veg_Cod = VarAC.Attributes("veg_cod").Value
                impianto.Sup_Imp = VarAC.Attributes("sup_imp").Value
                impianto.Veg_Des = VarAC.Attributes("veg_des").Value
                impianto.Cul_Cod = VarAC.Attributes("cul_cod").Value
                impianto.Cul_Des = VarAC.Attributes("cul_des").Value
                impianto.Rag_Soc = VarAC.Attributes("rag_soc").Value
                impianto.Validita_Inizio = VarAC.Attributes("validita_inizio").Value
                impianto.Validita_Fine = VarAC.Attributes("validita_fine").Value

                impianto.Grfi_Cod = VarAC.Attributes("grfi_cod").Value
                impianto.Campo_Cod = VarAC.Attributes("campo_cod").Value
                impianto.App_Nome = VarAC.Attributes("app_nome").Value
                impianto.Progetto_Cod = VarAC.Attributes("progetto_cod").Value

                ' mancano validita inizio e fine

                impianto.Data_Raccolta = AGRODATAFINE
                'lo aggiungo
                ListaImpianti(i) = impianto
                i = i + 1
            Next
            HttpContext.Current.Session("_AggiungiImpianti") = "true"
        Else
            HttpContext.Current.Session("_AggiungiImpianti") = "false"
        End If

    End Sub
#End Region
#Region "x Sessione"
    Public Sub Salva()
        If _Sessione Then
            HttpContext.Current.Session("ParametriConcimazione_2017") = Me
        End If
    End Sub
    Public Sub Distruggi()
        If _Sessione Then
            HttpContext.Current.Session("ParametriConcimazione_2017") = Nothing
        End If
    End Sub
    Public Sub Leggi()
        If _Sessione Then
            Dim obj As New ParametriConcimazione_2017
            obj = HttpContext.Current.Session("ParametriConcimazione_2017")

            If Not IsNothing(obj) Then
                _Tipo_Concimazione = obj.Tipo_Concimazione
                _Piva = obj.Piva
                _RagSoc = obj.RagSoc
                _Sa_Cod = obj.Sa_Cod
                _Veg_Cod = obj.Veg_Cod
                _PianoConcimazione_Testata_Cod = obj.PianoConcimazione_Testata_Cod
                _Pagina_Richiesta = obj.Pagina_Richiesta
                _Tipo_Operazione = obj.Tipo_Operazione
                _SitoOrigine = obj.SitoOrigine
                _Pagina_SitoOrigine = obj.Pagina_SitoOrigine
                _ListaImpianti = obj.ListaImpianti

            End If
        End If
    End Sub
#End Region

    ''' <summary>
    ''' Genera il blocco XML VariabiliConcimazione con all'interno 
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GeneraXML() As String
        Dim XmlDoc As New System.Xml.XmlDocument
        Dim Xml_VariabiliConcimazione As System.Xml.XmlElement
        'Creo il nodo "FiltroStampa"
        Xml_VariabiliConcimazione = XmlDoc.CreateElement("ParametriConcimazione_2017")

        Xml_VariabiliConcimazione.SetAttribute("tipo_concimazione", CStr(Tipo_Concimazione))
        Xml_VariabiliConcimazione.SetAttribute("piva", CStr(Piva))
        Xml_VariabiliConcimazione.SetAttribute("ragsoc", CStr(RagSoc))
        Xml_VariabiliConcimazione.SetAttribute("concimazione_testata_cod", CStr(PianoConcimazione_Testata_Cod))
        Xml_VariabiliConcimazione.SetAttribute("pagina_richiesta", CStr(Pagina_Richiesta))
        Xml_VariabiliConcimazione.SetAttribute("tipo_operazione", CStr(Tipo_Operazione))
        Xml_VariabiliConcimazione.SetAttribute("sito_origine", CStr(SitoOrigine))
        Xml_VariabiliConcimazione.SetAttribute("pagina_sito_origine", CStr(Pagina_SitoOrigine))
        Xml_VariabiliConcimazione.SetAttribute("veg_cod", CStr(Veg_Cod))


        'aggiungo gli impianti 
        Dim i As Integer
        If Not IsNothing(ListaImpianti) Then
            For i = 0 To ListaImpianti.Length - 1
                Dim VarAc As XmlElement
                VarAc = XmlDoc.CreateElement("VarAC")
                VarAc.SetAttribute("piva", ListaImpianti(i).Piva.Replace("'", "&#39;"))
                VarAc.SetAttribute("rag_soc", ListaImpianti(i).Rag_Soc.Replace("'", "&#39;"))
                VarAc.SetAttribute("sa_cod", ListaImpianti(i).Sa_Cod.ToString.Replace("'", "&#39;"))
                VarAc.SetAttribute("appezza", ListaImpianti(i).Appezza.ToString.Replace("'", "&#39;"))
                VarAc.SetAttribute("app_nome", ListaImpianti(i).App_Nome.ToString.Replace("'", "&#39;"))
                VarAc.SetAttribute("id_reg", ListaImpianti(i).Id_Reg.ToString.Replace("'", "&#39;"))
                VarAc.SetAttribute("veg_cod", ListaImpianti(i).Veg_Cod.ToString.Replace("'", "&#39;"))
                VarAc.SetAttribute("veg_des", ListaImpianti(i).Veg_Des.Replace("'", "&#39;"))
                VarAc.SetAttribute("cul_cod", ListaImpianti(i).Cul_Cod.ToString.Replace("'", "&#39;"))
                VarAc.SetAttribute("cul_des", ListaImpianti(i).Cul_Des.Replace("'", "&#39;"))
                VarAc.SetAttribute("validita_inizio", ListaImpianti(i).Validita_Inizio)
                VarAc.SetAttribute("validita_fine", ListaImpianti(i).Validita_Fine)
                VarAc.SetAttribute("sup_imp", ListaImpianti(i).Sup_Imp.Replace("'", "&#39;"))
                VarAc.SetAttribute("grfi_cod", ListaImpianti(i).Grfi_Cod.ToString.Replace("'", "&#39;"))
                VarAc.SetAttribute("campo_cod", ListaImpianti(i).Campo_Cod.ToString.Replace("'", "&#39;"))
                VarAc.SetAttribute("progetto_cod", ListaImpianti(i).Progetto_Cod.ToString.Replace("'", "&#39;"))

                Xml_VariabiliConcimazione.AppendChild(VarAc)
            Next

        End If

        XmlDoc.AppendChild(Xml_VariabiliConcimazione)
        Dim str As String
        str = XmlDoc.InnerXml
        str = str.Replace("'", "&#39;")
        Return str
    End Function

    Public Sub AddList(ByVal _Impianto As Impianto)
        'ridimensiono 
        If IsNothing(ListaImpianti) Then
            ListaImpianti = New Impianto(0) {}
        Else
            ReDim Preserve ListaImpianti(ListaImpianti.Length)
        End If
        ListaImpianti(ListaImpianti.Length - 1) = _Impianto
    End Sub
End Class

Public Class ParametriPUA
    Private _Tipo_Pua As Integer
    Private _Piva As String
    Private _Sa_Cod As Integer
    Private _Regolamento_Cod As Integer
    Private _PUA_Testata_Cod As Integer
    Private _Pagina_Richiesta As Integer 'tipo enumerativo enum_ParametriConcimazione_2008
    Private _Tipo_Operazione As Integer 'Inserimento 1 - Visualizzazione 2
    Private _SitoOrigine As Integer
    Private _Pagina_SitoOrigine As Integer
    Private _ListaImpianti() As Impianto

#Region "Proprietà"
    Public Property Tipo_Operazione() As Integer
        Get
            Return _Tipo_Operazione
        End Get
        Set(ByVal value As Integer)
            _Tipo_Operazione = value
            Salva()
        End Set
    End Property
    Public Property Pagina_Richiesta() As Integer
        Get
            Return _Pagina_Richiesta
        End Get
        Set(ByVal value As Integer)
            _Pagina_Richiesta = value
            Salva()
        End Set
    End Property
    Public Property PUA_Testata_Cod() As Integer
        Get
            Return _PUA_Testata_Cod
        End Get
        Set(ByVal value As Integer)
            _PUA_Testata_Cod = value
            Salva()
        End Set
    End Property
    Public Property Regolamento_Cod() As Integer
        Get
            Return _Regolamento_Cod
        End Get
        Set(ByVal value As Integer)
            _Regolamento_Cod = value
            Salva()
        End Set
    End Property
    Public Property Piva() As String
        Get
            Return _Piva
        End Get
        Set(ByVal value As String)
            _Piva = value
            Salva()
        End Set
    End Property
    Public Property Sa_Cod() As Integer
        Get
            Return _Sa_Cod
        End Get
        Set(ByVal value As Integer)
            _Sa_Cod = value
            Salva()
        End Set
    End Property

    Public Property Tipo_Pua() As Integer
        Get
            Return _Tipo_Pua
        End Get
        Set(ByVal value As Integer)
            _Tipo_Pua = value
            Salva()
        End Set
    End Property
    Public Property SitoOrigine() As Integer
        Get
            Return _SitoOrigine
        End Get
        Set(ByVal value As Integer)
            _SitoOrigine = value
            Salva()
        End Set
    End Property
    Public Property Pagina_SitoOrigine() As Integer
        Get
            Return _Pagina_SitoOrigine
        End Get
        Set(ByVal value As Integer)
            _Pagina_SitoOrigine = value
            Salva()
        End Set
    End Property

    Public Property ListaImpianti() As Impianto()
        Get
            Return _ListaImpianti
        End Get
        Set(ByVal value As Impianto())
            _ListaImpianti = value
            Salva()
        End Set
    End Property
#End Region

#Region "Costruttori"

    Sub New()

    End Sub

    Sub New(ByVal XML As String)
        LeggiXML(XML)
    End Sub

    Private Sub LeggiXML(ByVal XML As String)
        Dim XmlDoc As New System.Xml.XmlDocument
        XmlDoc.LoadXml(XML)

        Dim XML_ParametriAggiuntivi As System.Xml.XmlElement
        XML_ParametriAggiuntivi = XmlDoc.SelectSingleNode("Parametri").SelectSingleNode("ParametriPUA")
        If XML_ParametriAggiuntivi.HasAttribute(LCase("tipo_pua")) = True Then
            Me.Tipo_Pua = XML_ParametriAggiuntivi.GetAttribute(LCase("tipo_pua"))
        End If
        If XML_ParametriAggiuntivi.HasAttribute(LCase("piva")) = True Then
            Me.Piva = XML_ParametriAggiuntivi.GetAttribute(LCase("piva"))
        End If
        If XML_ParametriAggiuntivi.HasAttribute(LCase("sa_cod")) = True Then
            Me.Sa_Cod = XML_ParametriAggiuntivi.GetAttribute(LCase("sa_cod"))
        End If
        If XML_ParametriAggiuntivi.HasAttribute(LCase("pua_testata_cod")) = True Then
            Me.PUA_Testata_Cod = XML_ParametriAggiuntivi.GetAttribute(LCase("pua_testata_cod"))
        End If
        If XML_ParametriAggiuntivi.HasAttribute(LCase("regolamento_cod")) = True Then
            Me.Regolamento_Cod = XML_ParametriAggiuntivi.GetAttribute(LCase("regolamento_cod"))
        End If

        If XML_ParametriAggiuntivi.HasAttribute(LCase("pagina_richiesta")) = True Then
            Me.Pagina_Richiesta = XML_ParametriAggiuntivi.GetAttribute(LCase("pagina_richiesta"))
        End If

        If XML_ParametriAggiuntivi.HasAttribute(LCase("tipo_operazione")) = True Then
            Me.Tipo_Operazione = XML_ParametriAggiuntivi.GetAttribute(LCase("tipo_operazione"))
        End If

        If XML_ParametriAggiuntivi.HasAttribute(LCase("sito_origine")) = True Then
            Me._SitoOrigine = XML_ParametriAggiuntivi.GetAttribute(LCase("sito_origine"))
        End If

        If XML_ParametriAggiuntivi.HasAttribute(LCase("pagina_sito_origine")) = True Then
            Me._Pagina_SitoOrigine = XML_ParametriAggiuntivi.GetAttribute(LCase("pagina_sito_origine"))
        End If

        'Lista degli impianti
        Dim list As XmlNodeList = XmlDoc.SelectSingleNode("Parametri").SelectNodes("ParametriPUA")(0).ChildNodes()
        If list.Count > 0 Then
            Dim i As Integer
            'ridimensiono l'array
            ReDim ListaImpianti(list.Count - 1)
            Dim VarAC As XmlElement
            For Each VarAC In list
                Dim impianto As New Impianto
                impianto.Piva = VarAC.Attributes("piva").Value
                impianto.Sa_Cod = VarAC.Attributes("sa_cod").Value
                impianto.Appezza = VarAC.Attributes("appezza").Value
                impianto.Id_Reg = VarAC.Attributes("id_reg").Value
                impianto.Veg_Cod = VarAC.Attributes("veg_cod").Value
                impianto.Sup_Imp = VarAC.Attributes("sup_imp").Value
                impianto.Veg_Des = VarAC.Attributes("veg_des").Value
                impianto.Cul_Cod = VarAC.Attributes("cul_cod").Value
                impianto.Cul_Des = VarAC.Attributes("cul_des").Value
                impianto.Rag_Soc = VarAC.Attributes("rag_soc").Value
                impianto.Validita_Inizio = VarAC.Attributes("validita_inizio").Value
                impianto.Validita_Fine = VarAC.Attributes("validita_fine").Value

                impianto.Grfi_Cod = VarAC.Attributes("grfi_cod").Value
                impianto.Campo_Cod = VarAC.Attributes("campo_cod").Value
                impianto.App_Nome = VarAC.Attributes("app_nome").Value
                impianto.Progetto_Cod = VarAC.Attributes("progetto_cod").Value

                ' mancano validita inizio e fine

                impianto.Data_Raccolta = AGRODATAFINE
                'lo aggiungo
                ListaImpianti(i) = impianto
                i = i + 1
            Next
            HttpContext.Current.Session("_AggiungiImpianti") = "true"
        Else
            HttpContext.Current.Session("_AggiungiImpianti") = "false"
        End If

    End Sub
#End Region
#Region "x Sessione"
    Public Sub Salva()
        HttpContext.Current.Session("ParametriPUA") = Me
    End Sub
    Public Sub Distruggi()
        HttpContext.Current.Session("ParametriPUA") = Nothing
    End Sub
    Public Sub Leggi()
        Dim obj As New ParametriPUA
        obj = HttpContext.Current.Session("ParametriPUA")
        If Not IsNothing(obj) Then
            _Tipo_Pua = obj.Tipo_Pua
            _Piva = obj.Piva
            _Sa_Cod = obj.Sa_Cod
            _PUA_Testata_Cod = obj.PUA_Testata_Cod
            _Regolamento_Cod = obj.Regolamento_Cod
            _Pagina_Richiesta = obj.Pagina_Richiesta
            _Tipo_Operazione = obj.Tipo_Operazione
            _SitoOrigine = obj.SitoOrigine
            _Pagina_SitoOrigine = obj.Pagina_SitoOrigine
            _ListaImpianti = obj.ListaImpianti
        End If
    End Sub
#End Region

    ''' <summary>
    ''' Genera il blocco XML VariabiliConcimazione con all'interno 
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GeneraXML() As String
        Dim XmlDoc As New System.Xml.XmlDocument
        Dim Xml_VariabiliPUA As System.Xml.XmlElement
        'Creo il nodo "FiltroStampa"
        Xml_VariabiliPUA = XmlDoc.CreateElement("ParametriPUA")

        Xml_VariabiliPUA.SetAttribute("tipo_pua", CStr(Tipo_Pua))
        Xml_VariabiliPUA.SetAttribute("piva", CStr(Piva))
        Xml_VariabiliPUA.SetAttribute("pua_testata_cod", CStr(PUA_Testata_Cod))
        Xml_VariabiliPUA.SetAttribute("regolamento_cod", CStr(Regolamento_Cod))
        Xml_VariabiliPUA.SetAttribute("pagina_richiesta", CStr(Pagina_Richiesta))
        Xml_VariabiliPUA.SetAttribute("tipo_operazione", CStr(Tipo_Operazione))
        Xml_VariabiliPUA.SetAttribute("sito_origine", CStr(SitoOrigine))
        Xml_VariabiliPUA.SetAttribute("pagina_sito_origine", CStr(Pagina_SitoOrigine))

        'aggiungo gli impianti 
        Dim i As Integer
        If Not IsNothing(ListaImpianti) Then
            For i = 0 To ListaImpianti.Length - 1
                Dim VarAc As XmlElement
                VarAc = XmlDoc.CreateElement("VarAC")
                VarAc.SetAttribute("piva", ListaImpianti(i).Piva.Replace("'", "&#39;"))
                VarAc.SetAttribute("rag_soc", ListaImpianti(i).Rag_Soc.Replace("'", "&#39;"))
                VarAc.SetAttribute("sa_cod", ListaImpianti(i).Sa_Cod.ToString.Replace("'", "&#39;"))
                VarAc.SetAttribute("appezza", ListaImpianti(i).Appezza.ToString.Replace("'", "&#39;"))
                VarAc.SetAttribute("app_nome", ListaImpianti(i).App_Nome.ToString.Replace("'", "&#39;"))
                VarAc.SetAttribute("id_reg", ListaImpianti(i).Id_Reg.ToString.Replace("'", "&#39;"))
                VarAc.SetAttribute("veg_cod", ListaImpianti(i).Veg_Cod.ToString.Replace("'", "&#39;"))
                VarAc.SetAttribute("veg_des", ListaImpianti(i).Veg_Des.Replace("'", "&#39;"))
                VarAc.SetAttribute("cul_cod", ListaImpianti(i).Cul_Cod.ToString.Replace("'", "&#39;"))
                VarAc.SetAttribute("cul_des", ListaImpianti(i).Cul_Des.Replace("'", "&#39;"))
                VarAc.SetAttribute("validita_inizio", ListaImpianti(i).Validita_Inizio)
                VarAc.SetAttribute("validita_fine", ListaImpianti(i).Validita_Fine)
                VarAc.SetAttribute("sup_imp", ListaImpianti(i).Sup_Imp.Replace("'", "&#39;"))
                VarAc.SetAttribute("grfi_cod", ListaImpianti(i).Grfi_Cod.ToString.Replace("'", "&#39;"))
                VarAc.SetAttribute("campo_cod", ListaImpianti(i).Campo_Cod.ToString.Replace("'", "&#39;"))
                VarAc.SetAttribute("progetto_cod", ListaImpianti(i).Progetto_Cod.ToString.Replace("'", "&#39;"))

                Xml_VariabiliPUA.AppendChild(VarAc)
            Next

        End If

        XmlDoc.AppendChild(Xml_VariabiliPUA)
        Dim str As String
        str = XmlDoc.InnerXml
        str = str.Replace("'", "&#39;")
        Return str
    End Function

    Public Sub AddList(ByVal _Impianto As Impianto)
        'ridimensiono 
        If IsNothing(ListaImpianti) Then
            ListaImpianti = New Impianto(0) {}
        Else
            ReDim Preserve ListaImpianti(ListaImpianti.Length)
        End If
        ListaImpianti(ListaImpianti.Length - 1) = _Impianto
    End Sub
End Class

'#################################################################################################
''' <summary>
''' PROFILAZIONE 2010
''' </summary>
''' <remarks></remarks>
Public Class ParametriProfilazione_2010
    Private _Piva As String
    Private _Globale As Boolean
    Private _SitoRichiesto As String
    Private _Pagina_Richiesta As Integer
    Private _Xml_Permessi As String
    Private _Sql_Permessi As String
    Private _Stringa_Parametri As String
    Private _RagSoc As String = ""
    Private _Username As String

#Region "Proprietà"

    Property Pagina_Richiesta() As Integer
        Get
            Return _Pagina_Richiesta
        End Get
        Set(ByVal Value As Integer)
            _Pagina_Richiesta = Value
            Salva()
        End Set
    End Property
    Public Property Globale() As Boolean
        Get
            Return _Globale
        End Get
        Set(ByVal value As Boolean)
            _Globale = value
            Salva()
        End Set
    End Property
    Property SitoRichiesto() As String
        Get
            Return _SitoRichiesto
        End Get
        Set(ByVal Value As String)
            _SitoRichiesto = Value
            Salva()
        End Set
    End Property
    Property Piva() As String
        Get
            Return _Piva
        End Get
        Set(ByVal Value As String)
            _Piva = Value
            Salva()
        End Set
    End Property
    Property Xml_Permessi() As String
        Get
            Return _Xml_Permessi
        End Get
        Set(ByVal Value As String)
            _Xml_Permessi = Value
            Salva()
        End Set
    End Property
    Property Sql_Permessi() As String
        Get
            Return _Sql_Permessi
        End Get
        Set(ByVal Value As String)
            _Sql_Permessi = Value
            Salva()
        End Set
    End Property
    Property Stringa_Parametri() As String
        Get
            Return _Stringa_Parametri
        End Get
        Set(ByVal Value As String)
            _Stringa_Parametri = Value
            Salva()
        End Set
    End Property
    Property Username() As String
        Get
            Return _Username
        End Get
        Set(ByVal Value As String)
            _Username = Value
            Salva()
        End Set
    End Property
    Public Property RagSoc() As String
        Get
            Return _RagSoc
        End Get
        Set(ByVal value As String)
            _RagSoc = value
            Salva()
        End Set
    End Property
#End Region

#Region "X Sessione"
    Public Sub Salva()
        HttpContext.Current.Session("ParametriProfilazione_2010") = Me
    End Sub
    Public Sub Leggi()
        Dim obj As New ParametriProfilazione_2010
        obj = HttpContext.Current.Session("ParametriProfilazione_2010")
        If Not IsNothing(obj) Then
            _Piva = obj._Piva
            _SitoRichiesto = obj._SitoRichiesto
            _Globale = obj._Globale
            _Pagina_Richiesta = obj._Pagina_Richiesta
            _Xml_Permessi = obj._Xml_Permessi
            _Sql_Permessi = obj._Sql_Permessi
            _Stringa_Parametri = obj._Stringa_Parametri
            _Username = obj._Username
            _RagSoc = obj.RagSoc
        End If
    End Sub
#End Region

#Region "Costruttori"
    Public Sub New()

    End Sub

    Sub New(ByVal XML As String)
        LeggiXML(XML)
    End Sub

    Private Sub LeggiXML(ByVal XML As String)
        Dim XmlDoc As New System.Xml.XmlDocument
        XmlDoc.LoadXml(XML)

        Dim XML_ParametriAggiuntivi As System.Xml.XmlElement
        XML_ParametriAggiuntivi = XmlDoc.SelectSingleNode("Parametri").SelectSingleNode("ParametriProfilazione_2010")

        If XML_ParametriAggiuntivi.HasAttribute(LCase("piva")) = True Then
            Me.Piva = XML_ParametriAggiuntivi.GetAttribute(LCase("piva"))
        End If
        If XML_ParametriAggiuntivi.HasAttribute(LCase("ragsoc")) = True Then
            Me.RagSoc = XML_ParametriAggiuntivi.GetAttribute(LCase("ragsoc"))
        End If

        If XML_ParametriAggiuntivi.HasAttribute(LCase("sitorichiesto")) = True Then
            Me.SitoRichiesto = XML_ParametriAggiuntivi.GetAttribute(LCase("sitorichiesto"))
        End If
        If XML_ParametriAggiuntivi.HasAttribute(LCase("pagina_richiesta")) = True Then
            Me.Pagina_Richiesta = XML_ParametriAggiuntivi.GetAttribute(LCase("pagina_richiesta"))
        End If
        If XML_ParametriAggiuntivi.HasAttribute(LCase("xml_permessi")) = True Then
            Me.Xml_Permessi = XML_ParametriAggiuntivi.GetAttribute(LCase("xml_permessi"))
        End If
        If XML_ParametriAggiuntivi.HasAttribute(LCase("sql_permessi")) = True Then
            Me.Sql_Permessi = XML_ParametriAggiuntivi.GetAttribute(LCase("sql_permessi"))
        End If
        If XML_ParametriAggiuntivi.HasAttribute(LCase("stringa_parametri")) = True Then
            Me.Stringa_Parametri = XML_ParametriAggiuntivi.GetAttribute(LCase("stringa_parametri"))
        End If
        If XML_ParametriAggiuntivi.HasAttribute(LCase("username")) = True Then
            Me.Username = XML_ParametriAggiuntivi.GetAttribute(LCase("username"))
        End If

    End Sub

#End Region


    ''' <summary>
    ''' Genera il blocco XML VariabiliAnalisi con all'interno 
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GeneraXML() As String
        Dim XmlDoc As New System.Xml.XmlDocument
        Dim Xml_VariabiliAgenda As System.Xml.XmlElement
        'Creo il nodo "FiltroStampa"
        Xml_VariabiliAgenda = XmlDoc.CreateElement("ParametriProfilazione_2010")
        Xml_VariabiliAgenda.SetAttribute("piva", Piva)
        Xml_VariabiliAgenda.SetAttribute("sitorichiesto", SitoRichiesto)
        Xml_VariabiliAgenda.SetAttribute("pagina_richiesta", Pagina_Richiesta)
        Xml_VariabiliAgenda.SetAttribute("xml_permessi", Xml_Permessi)
        Xml_VariabiliAgenda.SetAttribute("sql_permessi", Sql_Permessi)
        Xml_VariabiliAgenda.SetAttribute("stringa_parametri", Stringa_Parametri)
        Xml_VariabiliAgenda.SetAttribute("username", Username)
        Xml_VariabiliAgenda.SetAttribute("ragsoc", RagSoc)
        XmlDoc.AppendChild(Xml_VariabiliAgenda)
        Dim str As String
        str = XmlDoc.InnerXml
        str = str.Replace("'", "&#39;")
        Return str
    End Function

End Class



'#################################################################################################
''' <summary>
''' SINCRONIZZATORE 2010
''' </summary>
''' <remarks></remarks>
Public Class ParametriSincronizzatore_2010

    Private _Piva As String
    Private _Pagina_Richiesta As Integer 'tipo enumerativo enum_PagineAgronicaSincro
    Private _Id_Cod_Cliente As Integer
    Private _ParametriQueryString As String
    Private _xml As String
    Private _xml_Generico As String = ""
    Private _RagSoc As String = ""
    Private _SitoOrigine As Integer
    Private _Pagina_SitoOrigine As Integer

    Public Property Xml() As String
        Get
            Return _xml
        End Get
        Set(ByVal value As String)
            _xml = value
        End Set
    End Property

    Public Property Xml_Generico() As String
        Get
            Return _xml_Generico
        End Get
        Set(ByVal value As String)
            _xml_Generico = value
        End Set
    End Property

#Region "Proprietà"
    Public Property Pagina_Richiesta() As Integer
        Get
            Return _Pagina_Richiesta
        End Get
        Set(ByVal value As Integer)
            _Pagina_Richiesta = value
            Salva()
        End Set
    End Property


    Public Property Piva() As String
        Get
            Return _Piva
        End Get
        Set(ByVal value As String)
            _Piva = value
            Salva()
        End Set
    End Property

    Public Property Id_Cod_Cliente() As String
        Get
            Return _Id_Cod_Cliente
        End Get
        Set(ByVal value As String)
            _Id_Cod_Cliente = value
            Salva()
        End Set

    End Property

    Public Property ParametriQueryString() As String
        Get
            Return _ParametriQueryString
        End Get
        Set(ByVal value As String)
            _ParametriQueryString = value
            Salva()
        End Set

    End Property

    Public Property RagSoc() As String
        Get
            Return _RagSoc
        End Get
        Set(ByVal value As String)
            _RagSoc = value
            Salva()
        End Set
    End Property

    Public Property SitoOrigine() As Integer
        Get
            Return _SitoOrigine
        End Get
        Set(ByVal value As Integer)
            _SitoOrigine = value
            Salva()
        End Set
    End Property

    Public Property Pagina_SitoOrigine() As Integer
        Get
            Return _Pagina_SitoOrigine
        End Get
        Set(ByVal value As Integer)
            _Pagina_SitoOrigine = value
            Salva()
        End Set
    End Property


#End Region

#Region "Costruttori"

    Sub New()

    End Sub


    Sub New(ByVal XML As String)
        _xml = XML
        LeggiXML(XML)
    End Sub

    Private Sub LeggiXML(ByVal XML As String)
        Dim XmlDoc As New System.Xml.XmlDocument
        XmlDoc.LoadXml(XML)

        Dim XML_ParametriAggiuntivi As System.Xml.XmlElement
        XML_ParametriAggiuntivi = XmlDoc.SelectSingleNode("Parametri").SelectSingleNode("ParametriSincronizzatore_2010")
        If XML_ParametriAggiuntivi.HasAttribute(LCase("piva")) = True Then
            Me.Piva = XML_ParametriAggiuntivi.GetAttribute(LCase("piva"))
        End If
        If XML_ParametriAggiuntivi.HasAttribute(LCase("pagina_richiesta")) = True Then
            Me.Pagina_Richiesta = XML_ParametriAggiuntivi.GetAttribute(LCase("pagina_richiesta"))
        End If
        If XML_ParametriAggiuntivi.HasAttribute(LCase("id_cod_cliente")) = True Then
            Me.Id_Cod_Cliente = XML_ParametriAggiuntivi.GetAttribute(LCase("id_cod_cliente"))
        End If

        If XML_ParametriAggiuntivi.HasAttribute(LCase("parametriquerystring")) = True Then
            Me.ParametriQueryString = XML_ParametriAggiuntivi.GetAttribute(LCase("parametriquerystring"))
        End If

        If XML_ParametriAggiuntivi.HasAttribute(LCase("sitoorigine")) = True Then
            Me.SitoOrigine = XML_ParametriAggiuntivi.GetAttribute(LCase("sitoorigine"))
        End If

        If XML_ParametriAggiuntivi.HasAttribute(LCase("pagina_sitoorigine")) = True Then
            Me.Pagina_SitoOrigine = XML_ParametriAggiuntivi.GetAttribute(LCase("pagina_sitoorigine"))
        End If



        If XmlDoc.SelectSingleNode("Parametri").SelectSingleNode("ParametriSincronizzatore_2010").ChildNodes.Count > 0 Then
            Dim StrNodiVariabili As String = ""
            Dim i As Integer = 0
            For i = 0 To XmlDoc.SelectSingleNode("Parametri").SelectSingleNode("ParametriSincronizzatore_2010").ChildNodes.Count - 1
                StrNodiVariabili &= XmlDoc.SelectSingleNode("Parametri").SelectSingleNode("ParametriSincronizzatore_2010").ChildNodes.Item(i).OuterXml
            Next

            Me.Xml_Generico &= StrNodiVariabili

        Else
            Xml_Generico = ""
        End If

    End Sub
#End Region


#Region "x Sessione"
    Public Sub Salva()
        HttpContext.Current.Session("ParametriSincronizzatore_2010") = Me
    End Sub
    Public Sub Leggi()
        Dim obj As New ParametriSincronizzatore_2010
        obj = HttpContext.Current.Session("ParametriSincronizzatore_2010")
        If Not IsNothing(obj) Then
            _Piva = obj.Piva
            _Pagina_Richiesta = obj.Pagina_Richiesta
            _Id_Cod_Cliente = obj.Id_Cod_Cliente
            _xml = obj.Xml
            _xml_Generico = obj.Xml_Generico
            _ParametriQueryString = obj.ParametriQueryString
            _RagSoc = obj.RagSoc
            _SitoOrigine = obj.SitoOrigine
            _Pagina_SitoOrigine = obj.Pagina_SitoOrigine
        End If
    End Sub
#End Region

    ''' <summary>
    ''' Genera il blocco XML VariabiliAnalisi con all'interno 
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GeneraXML() As String
        Dim XmlDoc As New System.Xml.XmlDocument
        Dim Xml_VariabiliSincro As System.Xml.XmlElement
        'Creo il nodo "FiltroStampa"
        Xml_VariabiliSincro = XmlDoc.CreateElement("ParametriSincronizzatore_2010")

        Xml_VariabiliSincro.SetAttribute("piva", CStr(Piva))
        Xml_VariabiliSincro.SetAttribute("pagina_richiesta", CStr(Pagina_Richiesta))
        Xml_VariabiliSincro.SetAttribute("id_cod_cliente", CStr(Id_Cod_Cliente))
        Xml_VariabiliSincro.SetAttribute("parametriquerystring", CStr(ParametriQueryString))

        Xml_VariabiliSincro.SetAttribute("sitoorigine", CStr(SitoOrigine))
        Xml_VariabiliSincro.SetAttribute("pagina_sitoorigine", CStr(Pagina_SitoOrigine))

        XmlDoc.AppendChild(Xml_VariabiliSincro)

        If Xml_Generico.Length > 0 Then
            Dim strxml As String = Xml_Generico.ToString
            strxml = strxml.Replace("'", "&#39;")
            Xml_VariabiliSincro.InnerXml = strxml
        End If


        Dim str As String
        str = XmlDoc.InnerXml
        str = str.Replace("'", "&#39;")
        Return str
    End Function


End Class



'#################################################################################################
''' <summary>
''' GIASONLINE 2010
''' </summary>
''' <remarks></remarks>
Public Class ParametriGiasOnline_2010

    Private _Pagina_Richiesta As Integer 'tipo enumerativo enum_PagineAgronicaSincro
    Private _piva As String
    Private _pagina_provenienza As Integer
    Private _Cod_Contatto As String
    Private _Cod_Contatto_Piva As String
    Private _Cod_Contatto_Sa_Cod As Integer
    Private _Rapporto_Contabile As String


#Region "Proprietà"
    Public Property Pagina_Richiesta() As Integer
        Get
            Return _Pagina_Richiesta
        End Get
        Set(ByVal value As Integer)
            _Pagina_Richiesta = value
            Salva()
        End Set
    End Property

    Public Property Rapporto_Contabile() As String
        Get
            Return _Rapporto_Contabile
        End Get
        Set(ByVal value As String)
            _Rapporto_Contabile = value
            Salva()
        End Set
    End Property



    Public Property piva() As String
        Get
            Return _piva
        End Get
        Set(ByVal value As String)
            _piva = value
            Salva()
        End Set
    End Property

    Public Property pagina_provenienza() As Integer
        Get
            Return _pagina_provenienza
        End Get
        Set(ByVal value As Integer)
            _pagina_provenienza = value
            Salva()
        End Set
    End Property

    Public Property Cod_Contatto() As String
        Get
            Return _Cod_Contatto
        End Get
        Set(ByVal value As String)
            _Cod_Contatto = value
            Salva()
        End Set
    End Property

    Public Property Cod_Contatto_Piva() As String
        Get
            Return _Cod_Contatto_Piva
        End Get
        Set(ByVal value As String)
            _Cod_Contatto_Piva = value
            Salva()
        End Set
    End Property

    Public Property Cod_Contatto_Sa_Cod() As Integer
        Get
            Return _Cod_Contatto_Sa_Cod
        End Get
        Set(ByVal value As Integer)
            _Cod_Contatto_Sa_Cod = value
            Salva()
        End Set
    End Property
#End Region

#Region "Costruttori"

    Sub New()

    End Sub


    Sub New(ByVal XML As String)
        LeggiXML(XML)
    End Sub

    Private Sub LeggiXML(ByVal XML As String)
        Dim XmlDoc As New System.Xml.XmlDocument
        XmlDoc.LoadXml(XML)

        Dim XML_ParametriAggiuntivi As System.Xml.XmlElement
        XML_ParametriAggiuntivi = XmlDoc.SelectSingleNode("Parametri").SelectSingleNode("ParametriGiasOnline_2010")
        If XML_ParametriAggiuntivi.HasAttribute(LCase("pagina_richiesta")) = True Then
            Me.Pagina_Richiesta = XML_ParametriAggiuntivi.GetAttribute(LCase("pagina_richiesta"))
        End If
        If XML_ParametriAggiuntivi.HasAttribute(LCase("piva")) = True Then
            Me.piva = XML_ParametriAggiuntivi.GetAttribute(LCase("piva"))
        End If
        If XML_ParametriAggiuntivi.HasAttribute(LCase("pagina_provenienza")) = True Then
            Me.pagina_provenienza = XML_ParametriAggiuntivi.GetAttribute(LCase("pagina_provenienza"))
        End If
        If XML_ParametriAggiuntivi.HasAttribute(LCase("cod_contatto")) = True Then
            Me.Cod_Contatto = XML_ParametriAggiuntivi.GetAttribute(LCase("cod_contatto"))
        End If
        If XML_ParametriAggiuntivi.HasAttribute(LCase("cod_contatto_piva")) = True Then
            Me.Cod_Contatto_Piva = XML_ParametriAggiuntivi.GetAttribute(LCase("cod_contatto_piva"))
        End If
        If XML_ParametriAggiuntivi.HasAttribute(LCase("cod_contatto_sa_cod")) = True Then
            Me.Cod_Contatto_Sa_Cod = XML_ParametriAggiuntivi.GetAttribute(LCase("cod_contatto_sa_cod"))
        End If
        If XML_ParametriAggiuntivi.HasAttribute(LCase("Rapporto_Contabile")) = True Then
            Me.Rapporto_Contabile = XML_ParametriAggiuntivi.GetAttribute(LCase("Rapporto_Contabile"))
        End If
    End Sub
#End Region


#Region "x Sessione"
    Public Sub Salva()
        HttpContext.Current.Session("ParametriGiasOnline_2010") = Me
    End Sub
    Public Sub Leggi()
        Dim obj As New ParametriGiasOnline_2010
        obj = HttpContext.Current.Session("ParametriGiasOnline_2010")
        If Not IsNothing(obj) Then
            _Pagina_Richiesta = obj.Pagina_Richiesta
            _piva = obj.piva
            _pagina_provenienza = obj._pagina_provenienza
            _Cod_Contatto = obj._Cod_Contatto
            _Cod_Contatto_Piva = obj._Cod_Contatto_Piva
            _Cod_Contatto_Sa_Cod = obj._Cod_Contatto_Sa_Cod
            _Rapporto_Contabile = obj._Rapporto_Contabile
        End If
    End Sub
#End Region

    ''' <summary>
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GeneraXML() As String
        Dim XmlDoc As New System.Xml.XmlDocument
        Dim Xml_VariabiliGiasOnline_2010 As System.Xml.XmlElement
        Xml_VariabiliGiasOnline_2010 = XmlDoc.CreateElement("ParametriGiasOnline_2010")
        Xml_VariabiliGiasOnline_2010.SetAttribute("pagina_richiesta", CStr(Pagina_Richiesta))
        Xml_VariabiliGiasOnline_2010.SetAttribute("piva", CStr(piva))
        Xml_VariabiliGiasOnline_2010.SetAttribute("pagina_provenienza", CStr(pagina_provenienza))
        Xml_VariabiliGiasOnline_2010.SetAttribute("cod_contatto", CStr(Cod_Contatto))
        Xml_VariabiliGiasOnline_2010.SetAttribute("cod_contatto_piva", CStr(Cod_Contatto_Piva))
        Xml_VariabiliGiasOnline_2010.SetAttribute("cod_contatto_sa_cod", CStr(Cod_Contatto_Sa_Cod))
        Xml_VariabiliGiasOnline_2010.SetAttribute("rapporto_contabile", CStr(Rapporto_Contabile))
        XmlDoc.AppendChild(Xml_VariabiliGiasOnline_2010)
        Dim str As String
        str = XmlDoc.InnerXml
        str = str.Replace("'", "&#39;")
        Return str
    End Function


End Class



'#################################################################################################
''' <summary>
''' x gestrire la struttura dell'IMPIANTO
''' </summary>
''' <remarks></remarks>

Public Class Impianto
    Private _Piva As String
    Private _Sa_Cod As Integer
    Private _Appezza As Integer
    Private _Id_Reg As Integer
    Private _Campo_Cod As Integer

    Private _Veg_Cod As Integer
    Private _Cul_Cod As Integer
    Private _Grfi_Cod As Integer

    Private _Rag_Soc As String
    Private _Sa_Nome As String
    Private _App_Nome As String
    Private _Veg_Des As String
    Private _Cul_Des As String
    Private _Sup_Imp As String
    Private _Validita_Inizio As DateTime
    Private _Validita_Fine As DateTime

    Private _Data_Raccolta As DateTime
    Private _Progetto_Cod As Integer

#Region "Propery"
    Public Property Piva() As String
        Get
            Return _Piva
        End Get
        Set(ByVal value As String)
            _Piva = value
        End Set
    End Property
    Public Property Sa_Cod() As Integer
        Get
            Return _Sa_Cod
        End Get
        Set(ByVal value As Integer)
            _Sa_Cod = value
        End Set
    End Property
    Public Property Appezza() As Integer
        Get
            Return _Appezza
        End Get
        Set(ByVal value As Integer)
            _Appezza = value
        End Set
    End Property
    Public Property Id_Reg() As Integer
        Get
            Return _Id_Reg
        End Get
        Set(ByVal value As Integer)
            _Id_Reg = value
        End Set
    End Property
    Public Property Campo_Cod() As Integer
        Get
            Return _Campo_Cod
        End Get
        Set(ByVal value As Integer)
            _Campo_Cod = value
        End Set
    End Property
    Public Property Veg_Cod() As Integer
        Get
            Return _Veg_Cod
        End Get
        Set(ByVal value As Integer)
            _Veg_Cod = value
        End Set
    End Property
    Public Property Cul_Cod() As Integer
        Get
            Return _Cul_Cod
        End Get
        Set(ByVal value As Integer)
            _Cul_Cod = value
        End Set
    End Property
    Public Property Grfi_Cod() As Integer
        Get
            Return _Grfi_Cod
        End Get
        Set(ByVal value As Integer)
            _Grfi_Cod = value
        End Set
    End Property
    Public Property Rag_Soc() As String
        Get
            Return _Rag_Soc
        End Get
        Set(ByVal value As String)
            _Rag_Soc = value
        End Set
    End Property
    Public Property Sa_Nome() As String
        Get
            Return _Sa_Nome
        End Get
        Set(ByVal value As String)
            _Sa_Nome = value
        End Set
    End Property
    Public Property App_Nome() As String
        Get
            Return _App_Nome
        End Get
        Set(ByVal value As String)
            _App_Nome = value
        End Set
    End Property
    Public Property Veg_Des() As String
        Get
            Return _Veg_Des
        End Get
        Set(ByVal value As String)
            _Veg_Des = value
        End Set
    End Property
    Public Property Cul_Des() As String
        Get
            Return _Cul_Des
        End Get
        Set(ByVal value As String)
            _Cul_Des = value
        End Set
    End Property
    Public Property Sup_Imp() As String
        Get
            Return _Sup_Imp
        End Get
        Set(ByVal value As String)
            _Sup_Imp = value
        End Set
    End Property
    Public Property Data_Raccolta() As DateTime
        Get
            Return _Data_Raccolta
        End Get
        Set(ByVal value As DateTime)
            _Data_Raccolta = value
        End Set
    End Property
    Public Property Validita_Inizio() As Date
        Get
            Return _Validita_Inizio
        End Get
        Set(ByVal value As DateTime)
            _Validita_Inizio = value
        End Set
    End Property
    Public Property Validita_Fine() As Date
        Get
            Return _Validita_Fine
        End Get
        Set(ByVal value As DateTime)
            _Validita_Fine = value
        End Set
    End Property
    Public Property Progetto_Cod() As Integer
        Get
            Return _Progetto_Cod
        End Get
        Set(ByVal value As Integer)
            _Progetto_Cod = value
        End Set
    End Property
#End Region

#Region "Costruttore"
    Public Sub New()
        Piva = ""
        Sa_Cod = 0
        Appezza = 0
        Id_Reg = 0
        Campo_Cod = 0
        Veg_Cod = 0
        Cul_Cod = 0
        Grfi_Cod = 0
        Rag_Soc = ""
        Sa_Nome = ""
        App_Nome = ""
        Veg_Des = ""
        Cul_Des = ""
        Sup_Imp = "0"
        Validita_Inizio = AGRODATAINIZIO
        Validita_Fine = AGRODATAFINE
        Data_Raccolta = DateTime.Now
        Progetto_Cod = 0
    End Sub
#End Region
End Class



'#################################################################################################
''' <summary>
''' AGRONICA METEO
''' </summary>
''' <remarks></remarks>
Public Class ParametriAgronicaMeteo

    Private _Piva As String
    Private _Pagina_Richiesta As Integer 'tipo enumerativo enum_PagineAgronicaMeteo

#Region "Proprietà"
    Public Property Pagina_Richiesta() As Integer
        Get
            Return _Pagina_Richiesta
        End Get
        Set(ByVal value As Integer)
            _Pagina_Richiesta = value
            Salva()
        End Set
    End Property


    Public Property Piva() As String
        Get
            Return _Piva
        End Get
        Set(ByVal value As String)
            _Piva = value
            Salva()
        End Set
    End Property

#End Region

#Region "Costruttori"

    Sub New()

    End Sub


    Sub New(ByVal XML As String)
        LeggiXML(XML)
    End Sub

    Private Sub LeggiXML(ByVal XML As String)
        Dim XmlDoc As New System.Xml.XmlDocument
        XmlDoc.LoadXml(XML)

        Dim XML_ParametriAggiuntivi As System.Xml.XmlElement
        XML_ParametriAggiuntivi = XmlDoc.SelectSingleNode("Parametri").SelectSingleNode("ParametriAgronicaMeteo")

        If XML_ParametriAggiuntivi.HasAttribute(LCase("piva")) = True Then
            Me.Piva = XML_ParametriAggiuntivi.GetAttribute(LCase("piva"))
        End If
        If XML_ParametriAggiuntivi.HasAttribute(LCase("pagina_richiesta")) = True Then
            Me.Pagina_Richiesta = XML_ParametriAggiuntivi.GetAttribute(LCase("pagina_richiesta"))
        End If
    End Sub
#End Region


#Region "x Sessione"
    Public Sub Salva()
        HttpContext.Current.Session("ParametriAgronicaMeteo") = Me
    End Sub

    Public Sub Leggi()
        Dim obj As New ParametriAgronicaMeteo
        obj = HttpContext.Current.Session("ParametriAgronicaMeteo")
        If Not IsNothing(obj) Then
            _Piva = obj.Piva
            _Pagina_Richiesta = obj.Pagina_Richiesta
        End If
    End Sub
#End Region

    ''' <summary>
    ''' Genera il blocco XML Variabili con all'interno 
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GeneraXML() As String
        Dim XmlDoc As New System.Xml.XmlDocument
        Dim Xml_Variabili As System.Xml.XmlElement
        'Creo il nodo "FiltroStampa"
        Xml_Variabili = XmlDoc.CreateElement("ParametriAgronicaMeteo")

        Xml_Variabili.SetAttribute("piva", CStr(Piva))
        Xml_Variabili.SetAttribute("pagina_richiesta", CStr(Pagina_Richiesta))

        XmlDoc.AppendChild(Xml_Variabili)
        Dim str As String
        str = XmlDoc.InnerXml
        str = str.Replace("'", "&#39;")
        Return str
    End Function


End Class


Public Class ParametriFiltroRicercaNG

    Private _sitoOrigine As Enum_SiteRedirector
    Private _paginaProvenienza As Integer
    Private _paginaProvenienzaURL As String
    Private _sitoDestinazioneDopoIlRedirect As Integer
    Private _paginaDestinazioneDopoIlRedirect As Integer
    Private _codificaStampe As enum_CodificaStampe
    Private _idSezione As Integer
    Private _piva As String
    Private _includiVisite As Boolean
    Private _filtriPianoColturale As FiltriPianoColturale
    Private _filtriTemporali As FiltriTemporali
    Private _filtriMovimenti As FiltriMovimenti
    Private _tipoComportamentoFiltroRicercaNG As Enum_TipoComportamento_FiltroRicerca = Enum_TipoComportamento_FiltroRicerca.Ricerca

    Private _tipoMostraGestitixChiamante As New List(Of Enum_TipoMostra_FiltroRicerca)
    Private _blocchiSelezionexStampa As New BlocchiSelezionexStampa
    Private _caricaDatiAggiuntivi As New CaricaDatiAggiuntivi

    Public Property SitoOrigine() As Enum_SiteRedirector
        Get
            Return _sitoOrigine
        End Get
        Set(value As Enum_SiteRedirector)
            _sitoOrigine = value
        End Set
    End Property

    Public Property IncludiVisite() As Boolean
        Get
            Return _includiVisite
        End Get
        Set(value As Boolean)
            _includiVisite = value
        End Set
    End Property

    Public Property TipoComportamentoFiltroRicercaNG() As Enum_TipoComportamento_FiltroRicerca
        Get
            Return _tipoComportamentoFiltroRicercaNG
        End Get
        Set(value As Enum_TipoComportamento_FiltroRicerca)
            _tipoComportamentoFiltroRicercaNG = value
        End Set
    End Property

    Public Property Piva() As String
        Get
            Return _piva
        End Get
        Set(value As String)
            _piva = value
        End Set
    End Property

    Public Property IDSezione() As Integer
        Get
            Return _idSezione
        End Get
        Set(value As Integer)
            _idSezione = value
        End Set
    End Property

    Public Property PaginaProvenienza() As Integer
        Get
            Return _paginaProvenienza
        End Get
        Set(ByVal value As Integer)
            _paginaProvenienza = value
        End Set
    End Property

    Public Property PaginaProvenienzaURL() As String
        Get
            Return _paginaProvenienzaURL
        End Get
        Set(value As String)
            _paginaProvenienzaURL = value
        End Set
    End Property

    Public Property SitoDestinazioneDopoIlRedirect() As Integer
        Get
            Return _sitoDestinazioneDopoIlRedirect
        End Get
        Set(ByVal value As Integer)
            _sitoDestinazioneDopoIlRedirect = value
        End Set
    End Property

    Public Property PaginaDestinazioneDopoIlRedirect() As Integer
        Get
            Return _paginaDestinazioneDopoIlRedirect
        End Get
        Set(ByVal value As Integer)
            _paginaDestinazioneDopoIlRedirect = value
        End Set
    End Property

    Public Property CodificaStampe() As enum_CodificaStampe
        Get
            Return _codificaStampe
        End Get
        Set(value As enum_CodificaStampe)
            _codificaStampe = value
        End Set
    End Property

    Public Property TipoMostraGestitiChiamante() As List(Of Enum_TipoMostra_FiltroRicerca)
        Get
            Return _tipoMostraGestitixChiamante
        End Get
        Set(ByVal value As List(Of Enum_TipoMostra_FiltroRicerca))
            _tipoMostraGestitixChiamante = value
        End Set
    End Property

    Public Property FiltriMovimenti() As FiltriMovimenti
        Get
            Return _filtriMovimenti
        End Get
        Set(value As FiltriMovimenti)
            _filtriMovimenti = value
        End Set
    End Property

    Public Property FiltriTemporali() As FiltriTemporali
        Get
            Return _filtriTemporali
        End Get
        Set(value As FiltriTemporali)
            _filtriTemporali = value
        End Set
    End Property

    Public Property FiltriPianoColturale() As FiltriPianoColturale
        Get
            Return _filtriPianoColturale
        End Get
        Set(value As FiltriPianoColturale)
            _filtriPianoColturale = value
        End Set
    End Property

    Public Property BlocchiSelezionexStampa() As BlocchiSelezionexStampa
        Get
            Return _blocchiSelezionexStampa
        End Get
        Set(value As BlocchiSelezionexStampa)
            _blocchiSelezionexStampa = value
        End Set
    End Property

    Public Property CaricaDatiAggiuntivi() As CaricaDatiAggiuntivi
        Get
            Return _caricaDatiAggiuntivi
        End Get
        Set(value As CaricaDatiAggiuntivi)
            _caricaDatiAggiuntivi = value
        End Set
    End Property

End Class

Public Class BlocchiSelezionexStampa
    Public Property SingolaAzienda As Boolean = False
    Public Property SingoloCentro As Boolean = False
    Public Property SingolaSpecie As Boolean = False
End Class

'#################################################################################################
''' <summary>
''' Filtrone 2010
''' </summary>
''' <remarks></remarks>
Public Class ParametriFILTRONE_2010

    Private _TipoFiltrone As String
    Private _CodificaStampe As String

    Private _Pagina_Origine As String
    Private _Sito_Origine As String
    Private _Sito_Destinazione As String
    Private _Pagina_Destinazione As String

    Private _Piva As String
    Private _Veg_Cod As String
    Private _Cul_Cod As String
    Private _Data_Inizio As String
    Private _Data_Fine As String

    Private _DatiDiRitorno As String

#Region "Property"

    Public Property Piva() As String
        Get
            Return _Piva
        End Get
        Set(ByVal value As String)
            _Piva = value
            Salva()
        End Set
    End Property

    Public Property Veg_Cod() As String
        Get
            Return _Veg_Cod
        End Get
        Set(ByVal value As String)
            _Veg_Cod = value
            Salva()
        End Set
    End Property

    Public Property Cul_Cod() As String
        Get
            Return _Cul_Cod
        End Get
        Set(ByVal value As String)
            _Cul_Cod = value
            Salva()
        End Set
    End Property

    Public Property Data_Inizio() As String
        Get
            Return _Data_Inizio
        End Get
        Set(ByVal value As String)
            _Data_Inizio = value
            Salva()
        End Set
    End Property

    Public Property Data_Fine() As String
        Get
            Return _Data_Fine
        End Get
        Set(ByVal value As String)
            _Data_Fine = value
            Salva()
        End Set
    End Property



    Public Property Sito_Destinazione() As String
        Get
            Return _Sito_Destinazione
        End Get
        Set(ByVal value As String)
            _Sito_Destinazione = value
            Salva()
        End Set
    End Property
    Public Property Pagina_Destinazione() As String
        Get
            Return _Pagina_Destinazione
        End Get
        Set(ByVal value As String)
            _Pagina_Destinazione = value
            Salva()
        End Set
    End Property

    Public Property Sito_Origine() As String
        Get
            Return _Sito_Origine
        End Get
        Set(ByVal value As String)
            _Sito_Origine = value
            Salva()
        End Set
    End Property
    Public Property Pagina_Origine() As String
        Get
            Return _Pagina_Origine
        End Get
        Set(ByVal value As String)
            _Pagina_Origine = value
            Salva()
        End Set
    End Property

    Public Property TipoFiltrone() As String
        Get
            Return _TipoFiltrone
        End Get
        Set(ByVal value As String)
            _TipoFiltrone = value
            Salva()
        End Set
    End Property
    Public Property CodificaStampe() As String
        Get
            Return _CodificaStampe
        End Get
        Set(ByVal value As String)
            _CodificaStampe = value
            Salva()
        End Set
    End Property

    Public Property DatiDiRitorno() As String
        Get
            Return _DatiDiRitorno
        End Get
        Set(ByVal value As String)
            _DatiDiRitorno = value
            Salva()
        End Set
    End Property

#End Region

#Region "X Sessione"
    Public Sub Salva()
        HttpContext.Current.Session("ParametriFILTRONE_2010") = Me
        'HttpContext.Current.Session("ParametriAgenda_2010") = Nothing
    End Sub
    Public Sub Leggi()
        Dim obj As New ParametriFILTRONE_2010
        obj = HttpContext.Current.Session("ParametriFILTRONE_2010")
        If Not IsNothing(obj) Then
            Sito_Origine = obj.Sito_Origine
            Pagina_Origine = obj.Pagina_Origine

            Sito_Destinazione = obj.Sito_Destinazione
            Pagina_Destinazione = obj.Pagina_Destinazione

            TipoFiltrone = obj.TipoFiltrone
            CodificaStampe = obj.CodificaStampe


            Veg_Cod = obj.Veg_Cod
            Cul_Cod = obj.Cul_Cod
            Data_Inizio = obj.Data_Inizio
            Data_Fine = obj.Data_Fine
            Piva = obj.Piva

            DatiDiRitorno = obj.DatiDiRitorno
        End If
    End Sub
#End Region

#Region "Costruttori"
    Public Sub New()

    End Sub

    Public Sub init()
        Sito_Origine = ""
        Pagina_Origine = ""

        Sito_Destinazione = ""
        Pagina_Destinazione = ""

        TipoFiltrone = ""
        CodificaStampe = ""

        Piva = ""
        Veg_Cod = ""
        Cul_Cod = ""
        Data_Inizio = ""
        Data_Fine = ""

        DatiDiRitorno = ""
    End Sub

    ''' <summary>
    ''' mi inizializza l'oggetto dall'XML passato
    ''' </summary>
    ''' <param name="XML"></param>
    ''' <remarks></remarks>
    Sub New(ByVal XML As String)
        LeggiXML(XML)
    End Sub

    Private Sub LeggiXML(ByVal XML As String)
        Dim XmlDoc As New System.Xml.XmlDocument
        XmlDoc.LoadXml(XML)

        Dim XML_ParametriAggiuntivi As System.Xml.XmlElement
        XML_ParametriAggiuntivi = XmlDoc.SelectSingleNode("Parametri").SelectSingleNode("ParametriFILTRONE_2010")
        'pagina richiesta
        If XML_ParametriAggiuntivi.HasAttribute(LCase("Sito_Origine")) = True Then
            Me.Sito_Origine = XML_ParametriAggiuntivi.GetAttribute(LCase("Sito_Origine"))
        End If
        If XML_ParametriAggiuntivi.HasAttribute(LCase("Pagina_Origine")) = True Then
            Me.Pagina_Origine = XML_ParametriAggiuntivi.GetAttribute(LCase("Pagina_Origine"))
        End If

        If XML_ParametriAggiuntivi.HasAttribute(LCase("Sito_Destinazione")) = True Then
            Me.Sito_Destinazione = XML_ParametriAggiuntivi.GetAttribute(LCase("Sito_Destinazione"))
        End If
        If XML_ParametriAggiuntivi.HasAttribute(LCase("Pagina_Destinazione")) = True Then
            Me.Pagina_Destinazione = XML_ParametriAggiuntivi.GetAttribute(LCase("Pagina_Destinazione"))
        End If

        If XML_ParametriAggiuntivi.HasAttribute(LCase("TipoFiltrone")) = True Then
            Me.TipoFiltrone = XML_ParametriAggiuntivi.GetAttribute(LCase("TipoFiltrone"))
        End If
        If XML_ParametriAggiuntivi.HasAttribute(LCase("CodificaStampe")) = True Then
            Me.CodificaStampe = XML_ParametriAggiuntivi.GetAttribute(LCase("CodificaStampe"))
        End If

        If XML_ParametriAggiuntivi.HasAttribute(LCase("piva")) = True Then
            Me.Piva = XML_ParametriAggiuntivi.GetAttribute(LCase("piva"))
        End If
        If XML_ParametriAggiuntivi.HasAttribute(LCase("Veg_Cod")) = True Then
            Me.Veg_Cod = XML_ParametriAggiuntivi.GetAttribute(LCase("Veg_Cod"))
        End If
        If XML_ParametriAggiuntivi.HasAttribute(LCase("Cul_Cod")) = True Then
            Me.Cul_Cod = XML_ParametriAggiuntivi.GetAttribute(LCase("Cul_Cod"))
        End If
        If XML_ParametriAggiuntivi.HasAttribute(LCase("Data_Inizio")) = True Then
            Me.Data_Inizio = XML_ParametriAggiuntivi.GetAttribute(LCase("Data_Inizio"))
        End If
        If XML_ParametriAggiuntivi.HasAttribute(LCase("Data_Fine")) = True Then
            Me.Data_Fine = XML_ParametriAggiuntivi.GetAttribute(LCase("Data_Fine"))
        End If

        If XML_ParametriAggiuntivi.HasAttribute(LCase("DatiDiRitorno")) = True Then
            Me.DatiDiRitorno = XML_ParametriAggiuntivi.GetAttribute(LCase("DatiDiRitorno"))
        End If
    End Sub
#End Region


    ''' <summary>
    ''' Genera il blocco XML VariabiliAnalisi con all'interno 
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GeneraXML() As String

        Dim XmlDoc As New System.Xml.XmlDocument
        Dim Xml_VariabiliPDC As System.Xml.XmlElement
        'Creo il nodo "FiltroStampa"
        Xml_VariabiliPDC = XmlDoc.CreateElement("ParametriFILTRONE_2010")
        Xml_VariabiliPDC.SetAttribute(LCase("Sito_Origine"), Sito_Origine)
        Xml_VariabiliPDC.SetAttribute(LCase("Pagina_Origine"), Pagina_Origine)
        Xml_VariabiliPDC.SetAttribute(LCase("Sito_Destinazione"), Sito_Destinazione)
        Xml_VariabiliPDC.SetAttribute(LCase("Pagina_Destinazione"), Pagina_Destinazione)
        Xml_VariabiliPDC.SetAttribute(LCase("TipoFiltrone"), TipoFiltrone)
        Xml_VariabiliPDC.SetAttribute(LCase("CodificaStampe"), CodificaStampe)

        Xml_VariabiliPDC.SetAttribute(LCase("piva"), Piva)
        Xml_VariabiliPDC.SetAttribute(LCase("Veg_Cod"), Veg_Cod)
        Xml_VariabiliPDC.SetAttribute(LCase("Cul_Cod"), Cul_Cod)
        Xml_VariabiliPDC.SetAttribute(LCase("Data_Inizio"), Data_Inizio)
        Xml_VariabiliPDC.SetAttribute(LCase("Data_Fine"), Data_Fine)

        Xml_VariabiliPDC.SetAttribute(LCase("DatiDiRitorno"), DatiDiRitorno)

        XmlDoc.AppendChild(Xml_VariabiliPDC)
        Dim str As String
        str = XmlDoc.InnerXml
        str = str.Replace("'", "&#39;")
        Return str

    End Function

End Class



'#################################################################################################
''' <summary>
''' Analisi Costi  2010
''' </summary>
''' <remarks></remarks>
Public Class ParametriAnalisiCosti_2010

    Private _PaginaRichiesta As String
    Private _ListaImpianti() As Impianto
    Private _Piva As String
    Private _Sa_Cod As Integer


#Region "Property"
    Public Property PaginaRichiesta() As String
        Get
            Return _PaginaRichiesta
        End Get
        Set(ByVal value As String)
            _PaginaRichiesta = value
            Salva()
        End Set
    End Property
    Public Property ListaImpianti() As Impianto()
        Get
            Return _ListaImpianti
        End Get
        Set(ByVal value As Impianto())
            _ListaImpianti = value
            Salva()
        End Set
    End Property
    Public Property Piva() As String
        Get
            Return _Piva
        End Get
        Set(ByVal value As String)
            _Piva = value
        End Set
    End Property
    Public Property Sa_Cod() As Integer
        Get
            Return _Sa_Cod
        End Get
        Set(ByVal value As Integer)
            _Sa_Cod = value
        End Set
    End Property
#End Region

#Region "X Sessione"
    Public Sub Salva()
        HttpContext.Current.Session("ParametriAnalisiCosti_2010") = Me
        HttpContext.Current.Session("ParametriAgenda_2010") = Nothing
    End Sub
    Public Sub Leggi()
        Dim obj As New ParametriAnalisiCosti_2010
        obj = HttpContext.Current.Session("ParametriAnalisiCosti_2010")
        If Not IsNothing(obj) Then
            PaginaRichiesta = obj.PaginaRichiesta
            ListaImpianti = obj.ListaImpianti
            Piva = obj.Piva
            Sa_Cod = obj.Sa_Cod
        End If
    End Sub
#End Region

#Region "Costruttori"
    Public Sub New()

    End Sub

    ''' <summary>
    ''' mi inizializza l'oggetto dall'XML passato
    ''' </summary>
    ''' <param name="XML"></param>
    ''' <remarks></remarks>
    Sub New(ByVal XML As String)
        LeggiXML(XML)
    End Sub

    Private Sub LeggiXML(ByVal XML As String)
        Dim XmlDoc As New System.Xml.XmlDocument
        XmlDoc.LoadXml(XML)

        Dim XML_ParametriAggiuntivi As System.Xml.XmlElement
        XML_ParametriAggiuntivi = XmlDoc.SelectSingleNode("Parametri").SelectSingleNode("ParametriAnalisiCosti_2010")
        'pagina richiesta
        If XML_ParametriAggiuntivi.HasAttribute(LCase("paginarichiesta")) = True Then
            Me.PaginaRichiesta = XML_ParametriAggiuntivi.GetAttribute(LCase("paginarichiesta"))
        End If

        If XML_ParametriAggiuntivi.HasAttribute(LCase("piva")) = True Then
            Me.Piva = XML_ParametriAggiuntivi.GetAttribute(LCase("piva"))
        End If

        If XML_ParametriAggiuntivi.HasAttribute(LCase("sa_cod")) = True Then
            Me.Sa_Cod = XML_ParametriAggiuntivi.GetAttribute(LCase("sa_cod"))
        End If

        'VariabiliAnalisiCosti
        'Lista degli impianti
        Dim list As XmlNodeList = XmlDoc.SelectSingleNode("Parametri").SelectNodes("ParametriAnalisiCosti_2010")(0).ChildNodes()
        If list.Count > 0 Then
            Dim i As Integer
            'ridimensiono l'array
            ReDim ListaImpianti(list.Count - 1)
            Dim VarAC As XmlElement
            For Each VarAC In list
                Dim impianto As New Impianto
                impianto.Piva = VarAC.Attributes("piva").Value
                impianto.Sa_Cod = VarAC.Attributes("sa_cod").Value
                impianto.Appezza = VarAC.Attributes("appezza").Value
                impianto.Id_Reg = VarAC.Attributes("id_reg").Value
                impianto.Veg_Cod = VarAC.Attributes("veg_cod").Value
                impianto.Sup_Imp = VarAC.Attributes("sup_imp").Value
                impianto.Veg_Des = VarAC.Attributes("veg_des").Value
                impianto.Cul_Cod = VarAC.Attributes("cul_cod").Value
                impianto.Cul_Des = VarAC.Attributes("cul_des").Value
                impianto.Rag_Soc = VarAC.Attributes("rag_soc").Value
                impianto.Validita_Inizio = VarAC.Attributes("validita_inizio").Value
                impianto.Validita_Fine = VarAC.Attributes("validita_fine").Value

                impianto.Grfi_Cod = VarAC.Attributes("grfi_cod").Value
                impianto.Campo_Cod = VarAC.Attributes("campo_cod").Value
                impianto.App_Nome = VarAC.Attributes("app_nome").Value
                impianto.Progetto_Cod = VarAC.Attributes("progetto_cod").Value

                ' mancano validita inizio e fine

                impianto.Data_Raccolta = AGRODATAFINE
                'lo aggiungo
                ListaImpianti(i) = impianto
                i = i + 1
            Next
            HttpContext.Current.Session("_AggiungiImpianti") = "true"
        Else
            HttpContext.Current.Session("_AggiungiImpianti") = "false"
        End If

    End Sub
#End Region


    ''' <summary>
    ''' Genera il blocco XML VariabiliAnalisi con all'interno 
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GeneraXML() As String
        Dim XmlDoc As New System.Xml.XmlDocument
        Dim Xml_VariabiliPDC As System.Xml.XmlElement
        'Creo il nodo "FiltroStampa"
        Xml_VariabiliPDC = XmlDoc.CreateElement("ParametriAnalisiCosti_2010")
        Xml_VariabiliPDC.SetAttribute("paginarichiesta", PaginaRichiesta)
        Xml_VariabiliPDC.SetAttribute("piva", Piva)
        Xml_VariabiliPDC.SetAttribute("sa_cod", Sa_Cod)

        'aggiungo gli impianti 
        Dim i As Integer
        If Not IsNothing(ListaImpianti) Then
            For i = 0 To ListaImpianti.Length - 1
                Dim VarAc As XmlElement
                VarAc = XmlDoc.CreateElement("VarAC")
                VarAc.SetAttribute("piva", ListaImpianti(i).Piva.Replace("'", "&#39;"))
                VarAc.SetAttribute("rag_soc", ListaImpianti(i).Rag_Soc.Replace("'", "&#39;"))
                VarAc.SetAttribute("sa_cod", ListaImpianti(i).Sa_Cod.ToString.Replace("'", "&#39;"))
                VarAc.SetAttribute("appezza", ListaImpianti(i).Appezza.ToString.Replace("'", "&#39;"))
                VarAc.SetAttribute("app_nome", ListaImpianti(i).App_Nome.ToString.Replace("'", "&#39;"))
                VarAc.SetAttribute("id_reg", ListaImpianti(i).Id_Reg.ToString.Replace("'", "&#39;"))
                VarAc.SetAttribute("veg_cod", ListaImpianti(i).Veg_Cod.ToString.Replace("'", "&#39;"))
                VarAc.SetAttribute("veg_des", ListaImpianti(i).Veg_Des.Replace("'", "&#39;"))
                VarAc.SetAttribute("cul_cod", ListaImpianti(i).Cul_Cod.ToString.Replace("'", "&#39;"))
                VarAc.SetAttribute("cul_des", ListaImpianti(i).Cul_Des.Replace("'", "&#39;"))
                VarAc.SetAttribute("validita_inizio", ListaImpianti(i).Validita_Inizio)
                VarAc.SetAttribute("validita_fine", ListaImpianti(i).Validita_Fine)
                VarAc.SetAttribute("sup_imp", ListaImpianti(i).Sup_Imp.Replace("'", "&#39;"))
                VarAc.SetAttribute("grfi_cod", ListaImpianti(i).Grfi_Cod.ToString.Replace("'", "&#39;"))
                VarAc.SetAttribute("campo_cod", ListaImpianti(i).Campo_Cod.ToString.Replace("'", "&#39;"))
                VarAc.SetAttribute("progetto_cod", ListaImpianti(i).Progetto_Cod.ToString.Replace("'", "&#39;"))

                Xml_VariabiliPDC.AppendChild(VarAc)
            Next

        End If

        XmlDoc.AppendChild(Xml_VariabiliPDC)
        Dim str As String
        str = XmlDoc.InnerXml
        str = str.Replace("'", "&#39;")
        Return str
    End Function


    Public Sub AddList(ByVal _Impianto As Impianto)
        'ridimensiono 
        If IsNothing(ListaImpianti) Then
            ListaImpianti = New Impianto(0) {}
        Else
            ReDim Preserve ListaImpianti(ListaImpianti.Length)
        End If
        ListaImpianti(ListaImpianti.Length - 1) = _Impianto
    End Sub

End Class



'#################################################################################################
''' <summary>
''' GIASONLINE 2010
''' </summary>
''' <remarks></remarks>
Public Class ParametriScadenziario

    Private _Pagina_Richiesta As Integer 'tipo enumerativo enum_PagineAgronicaSincro
    Private _Piva As String
    Private _Id_Area As Integer
    Private _Id_Tipologia As Integer
    Private _Analisi_Testata_Cod As Integer
    Private _Cod_Contatto As String

    Private _PC_Testata_Cod As Integer
    Private _PUA_Cod As Integer

    Private _Data_Scadenza As Date
    Private _PathFile As String

    Private _QueryStringFiltrino As String

#Region "Proprietà"
    Public Property Pagina_Richiesta() As Integer
        Get
            Return _Pagina_Richiesta
        End Get
        Set(ByVal value As Integer)
            _Pagina_Richiesta = value
            Salva()
        End Set
    End Property

    Public Property Piva() As String
        Get
            Return _Piva
        End Get
        Set(ByVal value As String)
            _Piva = value
            Salva()
        End Set
    End Property
    Public Property Cod_Contatto() As String
        Get
            Return _Cod_Contatto
        End Get
        Set(ByVal value As String)
            _Cod_Contatto = value
            Salva()
        End Set
    End Property

    Public Property Id_Area() As Integer
        Get
            Return _Id_Area
        End Get
        Set(ByVal value As Integer)
            _Id_Area = value
            Salva()
        End Set
    End Property

    Public Property Id_Tipologia() As Integer
        Get
            Return _Id_Tipologia
        End Get
        Set(ByVal value As Integer)
            _Id_Tipologia = value
            Salva()
        End Set
    End Property

    Public Property Analisi_Testata_Cod() As Integer
        Get
            Return _Analisi_Testata_Cod
        End Get
        Set(ByVal value As Integer)
            _Analisi_Testata_Cod = value
            Salva()
        End Set
    End Property

    Public Property PC_Testata_Cod() As Integer
        Get
            Return _PC_Testata_Cod
        End Get
        Set(ByVal value As Integer)
            _PC_Testata_Cod = value
            Salva()
        End Set
    End Property

    Public Property PUA_Cod() As Integer
        Get
            Return _PUA_Cod
        End Get
        Set(ByVal value As Integer)
            _PUA_Cod = value
            Salva()
        End Set
    End Property

    Public Property Data_Scadenza() As Date
        Get
            Return _Data_Scadenza
        End Get
        Set(ByVal value As Date)
            _Data_Scadenza = value
            Salva()
        End Set
    End Property

    Public Property PathFile() As String
        Get
            Return _PathFile
        End Get
        Set(ByVal value As String)
            _PathFile = value
            Salva()
        End Set
    End Property

    Public Property QueryStringFiltrino() As String
        Get
            Return _QueryStringFiltrino
        End Get
        Set(ByVal value As String)
            _QueryStringFiltrino = value
            Salva()
        End Set
    End Property

#End Region

#Region "Costruttori"

    Sub New()

    End Sub


    Sub New(ByVal XML As String)
        LeggiXML(XML)
    End Sub

    Private Sub LeggiXML(ByVal XML As String)
        Dim XmlDoc As New System.Xml.XmlDocument
        XmlDoc.LoadXml(XML)

        Dim XML_ParametriAggiuntivi As System.Xml.XmlElement
        XML_ParametriAggiuntivi = XmlDoc.SelectSingleNode("Parametri").SelectSingleNode("ParametriScadenziario")
        If XML_ParametriAggiuntivi.HasAttribute(LCase("pagina_richiesta")) = True Then
            Me.Pagina_Richiesta = XML_ParametriAggiuntivi.GetAttribute(LCase("pagina_richiesta"))
        End If
        If XML_ParametriAggiuntivi.HasAttribute(LCase("piva")) = True Then
            Me.Piva = XML_ParametriAggiuntivi.GetAttribute(LCase("piva"))
        End If
        If XML_ParametriAggiuntivi.HasAttribute(LCase("id_area")) = True Then
            Me.Id_Area = XML_ParametriAggiuntivi.GetAttribute(LCase("id_area"))
        End If
        If XML_ParametriAggiuntivi.HasAttribute(LCase("id_tipologia")) = True Then
            Me.Id_Tipologia = XML_ParametriAggiuntivi.GetAttribute(LCase("id_tipologia"))
        End If
        If XML_ParametriAggiuntivi.HasAttribute(LCase("analisi_testata_cod")) = True Then
            Me.Analisi_Testata_Cod = XML_ParametriAggiuntivi.GetAttribute(LCase("analisi_testata_cod"))
        End If
        If XML_ParametriAggiuntivi.HasAttribute(LCase("cod_contatto")) = True Then
            Me.Cod_Contatto = XML_ParametriAggiuntivi.GetAttribute(LCase("cod_contatto"))
        End If
        If XML_ParametriAggiuntivi.HasAttribute(LCase("pc_testata_cod")) = True Then
            Me.PC_Testata_Cod = XML_ParametriAggiuntivi.GetAttribute(LCase("pc_testata_cod"))
        End If
        If XML_ParametriAggiuntivi.HasAttribute(LCase("pua_cod")) = True Then
            Me.PUA_Cod = XML_ParametriAggiuntivi.GetAttribute(LCase("pua_cod"))
        End If
        If XML_ParametriAggiuntivi.HasAttribute(LCase("data_scadenza")) = True Then
            Me.Data_Scadenza = XML_ParametriAggiuntivi.GetAttribute(LCase("data_scadenza"))
        End If
        If XML_ParametriAggiuntivi.HasAttribute(LCase("path_file")) = True Then
            Me.PathFile = XML_ParametriAggiuntivi.GetAttribute(LCase("path_file"))
        End If
        If XML_ParametriAggiuntivi.HasAttribute(LCase("querystringfiltrino")) = True Then
            Me.QueryStringFiltrino = XML_ParametriAggiuntivi.GetAttribute(LCase("querystringfiltrino"))
        End If

    End Sub
#End Region


#Region "x Sessione"
    Public Sub Salva()
        HttpContext.Current.Session("ParametriScadenziario") = Me
    End Sub
    Public Sub Leggi()
        Dim obj As New ParametriScadenziario
        obj = HttpContext.Current.Session("ParametriScadenziario")
        If Not IsNothing(obj) Then
            _Pagina_Richiesta = obj.Pagina_Richiesta
            _Piva = obj.Piva
            _Id_Area = obj.Id_Area
            _Id_Tipologia = obj.Id_Tipologia
            _Analisi_Testata_Cod = obj.Analisi_Testata_Cod
            _PC_Testata_Cod = obj.PC_Testata_Cod
            _Data_Scadenza = obj.Data_Scadenza
            _PathFile = obj.PathFile
            _Cod_Contatto = obj.Cod_Contatto
            _PUA_Cod = obj.PUA_Cod
            _QueryStringFiltrino = obj.QueryStringFiltrino
        End If
    End Sub
#End Region

    ''' <summary>
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GeneraXML() As String
        Dim XmlDoc As New System.Xml.XmlDocument
        Dim Xml_VariabiliScadenziario As System.Xml.XmlElement
        Xml_VariabiliScadenziario = XmlDoc.CreateElement("ParametriScadenziario")
        Xml_VariabiliScadenziario.SetAttribute("pagina_richiesta", CStr(Pagina_Richiesta))
        Xml_VariabiliScadenziario.SetAttribute("piva", CStr(Piva))
        Xml_VariabiliScadenziario.SetAttribute("id_area", CStr(Id_Area))
        Xml_VariabiliScadenziario.SetAttribute("id_tipologia", CStr(Id_Tipologia))
        Xml_VariabiliScadenziario.SetAttribute("analisi_testata_cod", CStr(Analisi_Testata_Cod))
        Xml_VariabiliScadenziario.SetAttribute("pc_testata_cod", CStr(PC_Testata_Cod))
        Xml_VariabiliScadenziario.SetAttribute("cod_contatto", CStr(Cod_Contatto))
        Xml_VariabiliScadenziario.SetAttribute("pua_cod", CStr(PUA_Cod))
        Xml_VariabiliScadenziario.SetAttribute("data_scadenza", CStr(Data_Scadenza))
        Xml_VariabiliScadenziario.SetAttribute("path_file", CStr(PathFile))
        Xml_VariabiliScadenziario.SetAttribute("querystringfiltrino", CStr(QueryStringFiltrino))
        XmlDoc.AppendChild(Xml_VariabiliScadenziario)
        Dim str As String
        str = XmlDoc.InnerXml
        str = str.Replace("'", "&#39;")
        Return str
    End Function


End Class




'#################################################################################################
''' <summary>
''' AgronicaStampe
''' </summary>
''' <remarks></remarks>
Public Class ParametriAgronicaStampe_2010


    Private _username As String

    Private _report As Integer
    Private _user_profilo As String

    Private _UserProfilo_CodFisc As String
    Private _Sql_Filtro As String
    Private _Xml_Filtro As String

    Private _username_codfisc As String
    Private _user_profilo_codfisc As String
    Private _user_profilo_codgias As String

    Private _centro_costo As String
    Private _superficie As String
    Private _ricavi As String
    Private _costi As String
    Private _differenza As String
    Private _ricavi_ha As String
    Private _costi_ha As String
    Private _differenza_ha As String

    Private _ricavo_lordo As String
    Private _ricavo_totale As String
    Private _costi_variabili As String
    Private _margine_lordo As String
    Private _costi_operativi As String
    Private _margine_operativo As String
    Private _costi_fissi As String
    Private _margine_netto As String

    Private _data_stampa As String

    Private _visualizzazione As String

    Private _id_agenda As Integer

    Private _Xml_Generico As StringBuilder
    Private _JSon_Generico As StringBuilder
    Private _RagSoc As String
    Private _piva As String



#Region "Proprietà"

    Property id_agenda() As Integer
        Get
            Return _id_agenda
        End Get
        Set(ByVal Value As Integer)
            _id_agenda = Value
            Salva()
        End Set
    End Property

    Property visualizzazione() As String
        Get
            Return _visualizzazione
        End Get
        Set(ByVal Value As String)
            _visualizzazione = Value
            Salva()
        End Set
    End Property
    Property ricavo_lordo() As String
        Get
            Return _ricavo_lordo
        End Get
        Set(ByVal Value As String)
            _ricavo_lordo = Value
            Salva()
        End Set
    End Property
    Property ricavo_totale() As String
        Get
            Return _ricavo_totale
        End Get
        Set(ByVal Value As String)
            _ricavo_totale = Value
            Salva()
        End Set
    End Property
    Property costi_variabili() As String
        Get
            Return _costi_variabili
        End Get
        Set(ByVal Value As String)
            _costi_variabili = Value
            Salva()
        End Set
    End Property
    Property margine_lordo() As String
        Get
            Return _margine_lordo
        End Get
        Set(ByVal Value As String)
            _margine_lordo = Value
            Salva()
        End Set
    End Property
    Property costi_operativi() As String
        Get
            Return _costi_operativi
        End Get
        Set(ByVal Value As String)
            _costi_operativi = Value
            Salva()
        End Set
    End Property
    Property margine_operativo() As String
        Get
            Return _margine_operativo
        End Get
        Set(ByVal Value As String)
            _margine_operativo = Value
            Salva()
        End Set
    End Property
    Property costi_fissi() As String
        Get
            Return _costi_fissi
        End Get
        Set(ByVal Value As String)
            _costi_fissi = Value
            Salva()
        End Set
    End Property
    Property margine_netto() As String
        Get
            Return _margine_netto
        End Get
        Set(ByVal Value As String)
            _margine_netto = Value
            Salva()
        End Set
    End Property
    Property data_stampa() As String
        Get
            Return _data_stampa
        End Get
        Set(ByVal Value As String)
            _data_stampa = Value
            Salva()
        End Set
    End Property


    Property UserProfilo_CodFisc() As String
        Get
            Return _UserProfilo_CodFisc
        End Get
        Set(ByVal Value As String)
            _UserProfilo_CodFisc = Value
            Salva()
        End Set
    End Property

    Property Sql_Filtro() As String
        'devo stare attento al carattere percentuale e ' quindi faccio in modo di sostituirmlo
        Get
            Dim value As String = _Sql_Filtro

            If value Is Nothing Then
                Return ""
            Else
                value = value.Replace("SQL_SAFE_CARATTERE_PERCENTUALE", "%")
                value = value.Replace("SQL_SAFE_CARATTERE_APIVCE", "'")
                Return value
            End If
        End Get
        Set(ByVal Value As String)
            Value = Value.Replace("%", "SQL_SAFE_CARATTERE_PERCENTUALE")
            Value = Value.Replace("'", "SQL_SAFE_CARATTERE_APIVCE")
            _Sql_Filtro = Value
            Salva()
        End Set
    End Property

    Property Xml_Filtro() As String
        'devo stare attento al carattere percentuale e ' quindi faccio in modo di sostituirmlo
        Get
            Dim value As String = _Xml_Filtro

            If value Is Nothing Then
                Return ""
            Else
                value = value.Replace("SQL_SAFE_CARATTERE_PERCENTUALE", "%")
                value = value.Replace("SQL_SAFE_CARATTERE_APIVCE", "'")
                Return value
            End If
        End Get
        Set(ByVal Value As String)
            Value = Value.Replace("%", "SQL_SAFE_CARATTERE_PERCENTUALE")
            Value = Value.Replace("'", "SQL_SAFE_CARATTERE_APIVCE")
            _Xml_Filtro = Value
            Salva()
        End Set
    End Property

    Property centro_costo() As String
        Get
            Return _centro_costo
        End Get
        Set(ByVal Value As String)
            _centro_costo = Value
            Salva()
        End Set
    End Property
    Property superficie() As String
        Get
            Return _superficie
        End Get
        Set(ByVal Value As String)
            _superficie = Value
            Salva()
        End Set
    End Property
    Property ricavi() As String
        Get
            Return _ricavi
        End Get
        Set(ByVal Value As String)
            _ricavi = Value
            Salva()
        End Set
    End Property
    Property costi() As String
        Get
            Return _costi
        End Get
        Set(ByVal Value As String)
            _costi = Value
            Salva()
        End Set
    End Property
    Property differenza() As String
        Get
            Return _differenza
        End Get
        Set(ByVal Value As String)
            _differenza = Value
            Salva()
        End Set
    End Property
    Property ricavi_ha() As String
        Get
            Return _ricavi_ha
        End Get
        Set(ByVal Value As String)
            _ricavi_ha = Value
            Salva()
        End Set
    End Property
    Property costi_ha() As String
        Get
            Return _costi_ha
        End Get
        Set(ByVal Value As String)
            _costi_ha = Value
            Salva()
        End Set
    End Property
    Property differenza_ha() As String
        Get
            Return _differenza_ha
        End Get
        Set(ByVal Value As String)
            _differenza_ha = Value
            Salva()
        End Set
    End Property

    Property username() As String
        Get
            Return _username
        End Get
        Set(ByVal Value As String)
            _username = Value
            Salva()
        End Set
    End Property

    Property username_codfisc() As String
        Get
            Return _username_codfisc
        End Get
        Set(ByVal Value As String)
            _username_codfisc = Value
            Salva()
        End Set
    End Property

    Property user_profilo_codfisc() As String
        Get
            Return _user_profilo_codfisc
        End Get
        Set(ByVal Value As String)
            _user_profilo_codfisc = Value
            Salva()
        End Set
    End Property

    Property user_profilo_codgias() As String
        Get
            Return _user_profilo_codgias
        End Get
        Set(ByVal Value As String)
            _user_profilo_codgias = Value
            Salva()
        End Set
    End Property

    Property report() As Integer
        Get
            Return _report
        End Get
        Set(ByVal Value As Integer)
            _report = Value
            Salva()
        End Set
    End Property

    Property user_profilo() As String
        Get
            Return _user_profilo
        End Get
        Set(ByVal Value As String)
            _user_profilo = Value
            Salva()
        End Set
    End Property


    Property Xml_Generico() As StringBuilder
        Get
            Return _Xml_Generico
        End Get
        Set(ByVal Value As StringBuilder)
            _Xml_Generico = Value
            Salva()
        End Set
    End Property

    Property JSon_Generico() As StringBuilder
        Get
            Return _JSon_Generico
        End Get
        Set(ByVal Value As StringBuilder)
            _JSon_Generico = Value
            Salva()
        End Set
    End Property

    Property RagSoc() As String
        Get
            Return _RagSoc
        End Get
        Set(ByVal Value As String)
            _RagSoc = Value
            Salva()
        End Set
    End Property

    Property Piva() As String
        Get
            Return _piva
        End Get
        Set(ByVal Value As String)
            _piva = Value
            Salva()
        End Set
    End Property


#End Region

#Region "X Sessione"
    Public Sub Salva()
        HttpContext.Current.Session("ParametriAgronicaStampe_2010") = Me
    End Sub
    Public Sub Leggi()
        Dim obj As New ParametriAgronicaStampe_2010
        obj = HttpContext.Current.Session("ParametriAgronicaStampe_2010")
        If Not IsNothing(obj) Then
            _username = obj._username
            _report = obj._report
            _user_profilo = obj._user_profilo

            _Xml_Generico = obj._Xml_Generico
            _JSon_Generico = obj._JSon_Generico

            _UserProfilo_CodFisc = obj._UserProfilo_CodFisc
            _Sql_Filtro = obj._Sql_Filtro
            _Xml_Filtro = obj._Xml_Filtro

            _username_codfisc = obj._username_codfisc
            _user_profilo_codfisc = obj._user_profilo_codfisc
            _user_profilo_codgias = obj._user_profilo_codgias

            _centro_costo = obj._centro_costo
            _superficie = obj._superficie
            _ricavi = obj._ricavi
            _costi = obj._costi
            _differenza = obj._differenza
            _ricavi_ha = obj._ricavi_ha
            _costi_ha = obj._costi_ha
            _differenza_ha = obj._differenza_ha

            _ricavo_lordo = obj._ricavo_lordo
            _ricavo_totale = obj._ricavo_totale
            _costi_variabili = obj._costi_variabili
            _margine_lordo = obj._margine_lordo
            _costi_operativi = obj._costi_operativi
            _margine_operativo = obj._margine_operativo
            _costi_fissi = obj._costi_fissi
            _margine_netto = obj._margine_netto
            _data_stampa = obj._data_stampa

            _visualizzazione = obj._visualizzazione

            _id_agenda = obj.id_agenda
            _piva = If(Not String.IsNullOrEmpty(obj.Piva), obj.Piva, LeggiPiva())
            _RagSoc = obj.RagSoc

        End If

    End Sub
#End Region

#Region "Costruttori"
    Public Sub New()
        _Xml_Generico = New StringBuilder
        _Xml_Generico.Length = 0
        _JSon_Generico = New StringBuilder
        _JSon_Generico.Length = 0
        _RagSoc = String.Empty
        _piva = String.Empty
    End Sub

    Sub New(ByVal XML As String)
        LeggiXML(XML)
    End Sub

    Private Sub LeggiXML(ByVal XML As String)

        Dim XmlDoc As New System.Xml.XmlDocument
        XmlDoc.LoadXml(XML)

        Dim XML_ParametriAggiuntivi As System.Xml.XmlElement
        XML_ParametriAggiuntivi = XmlDoc.SelectSingleNode("Parametri").SelectSingleNode("ParametriAgronicaStampe_2010")

        If XML_ParametriAggiuntivi.HasAttribute(LCase("username")) = True Then
            Me.username = XML_ParametriAggiuntivi.GetAttribute(LCase("username"))
        End If

        If XML_ParametriAggiuntivi.HasAttribute(LCase("username_codfisc")) = True Then
            Me.username_codfisc = XML_ParametriAggiuntivi.GetAttribute(LCase("username_codfisc"))
        End If

        If XML_ParametriAggiuntivi.HasAttribute(LCase("user_profilo_codfisc")) = True Then
            Me.user_profilo_codfisc = XML_ParametriAggiuntivi.GetAttribute(LCase("user_profilo_codfisc"))
        End If

        If XML_ParametriAggiuntivi.HasAttribute(LCase("user_profilo_codgias")) = True Then
            Me.user_profilo_codgias = XML_ParametriAggiuntivi.GetAttribute(LCase("user_profilo_codgias"))
        End If

        If XML_ParametriAggiuntivi.HasAttribute(LCase("user_profilo")) = True Then
            Me.user_profilo = XML_ParametriAggiuntivi.GetAttribute(LCase("user_profilo"))
        End If

        If XML_ParametriAggiuntivi.HasAttribute(LCase("report")) = True Then
            Me.report = XML_ParametriAggiuntivi.GetAttribute(LCase("report"))
        End If


        'no lcase per compatibilità gestione richieste stampe con titi vecchi
        If XML_ParametriAggiuntivi.HasAttribute("UserProfilo_CodFisc") = True Then
            Me.UserProfilo_CodFisc = XML_ParametriAggiuntivi.GetAttribute("UserProfilo_CodFisc")
        End If

        If XML_ParametriAggiuntivi.HasAttribute("Sql_Filtro") = True Then
            Me.Sql_Filtro = XML_ParametriAggiuntivi.GetAttribute("Sql_Filtro")
        End If

        If XML_ParametriAggiuntivi.HasAttribute("Xml_Filtro") = True Then
            Me.Xml_Filtro = XML_ParametriAggiuntivi.GetAttribute("Xml_Filtro")
        End If
        'fine


        If XML_ParametriAggiuntivi.HasAttribute("centro_costo") = True Then
            Me.centro_costo = XML_ParametriAggiuntivi.GetAttribute("centro_costo")
        End If
        If XML_ParametriAggiuntivi.HasAttribute("superficie") = True Then
            Me.superficie = XML_ParametriAggiuntivi.GetAttribute("superficie")
        End If
        If XML_ParametriAggiuntivi.HasAttribute("ricavi") = True Then
            Me.ricavi = XML_ParametriAggiuntivi.GetAttribute("ricavi")
        End If
        If XML_ParametriAggiuntivi.HasAttribute("costi") = True Then
            Me.costi = XML_ParametriAggiuntivi.GetAttribute("costi")
        End If
        If XML_ParametriAggiuntivi.HasAttribute("differenza") = True Then
            Me.differenza = XML_ParametriAggiuntivi.GetAttribute("differenza")
        End If
        If XML_ParametriAggiuntivi.HasAttribute("ricavi_ha") = True Then
            Me.ricavi_ha = XML_ParametriAggiuntivi.GetAttribute("ricavi_ha")
        End If
        If XML_ParametriAggiuntivi.HasAttribute("costi_ha") = True Then
            Me.costi_ha = XML_ParametriAggiuntivi.GetAttribute("costi_ha")
        End If
        If XML_ParametriAggiuntivi.HasAttribute("differenza_ha") = True Then
            Me.differenza_ha = XML_ParametriAggiuntivi.GetAttribute("differenza_ha")
        End If


        If XML_ParametriAggiuntivi.HasAttribute("ricavo_lordo") = True Then
            Me.ricavo_lordo = XML_ParametriAggiuntivi.GetAttribute("ricavo_lordo")
        End If
        If XML_ParametriAggiuntivi.HasAttribute("ricavo_totale") = True Then
            Me.ricavo_totale = XML_ParametriAggiuntivi.GetAttribute("ricavo_totale")
        End If
        If XML_ParametriAggiuntivi.HasAttribute("costi_variabili") = True Then
            Me.costi_variabili = XML_ParametriAggiuntivi.GetAttribute("costi_variabili")
        End If
        If XML_ParametriAggiuntivi.HasAttribute("margine_lordo") = True Then
            Me.margine_lordo = XML_ParametriAggiuntivi.GetAttribute("margine_lordo")
        End If
        If XML_ParametriAggiuntivi.HasAttribute("costi_operativi") = True Then
            Me.costi_operativi = XML_ParametriAggiuntivi.GetAttribute("costi_operativi")
        End If
        If XML_ParametriAggiuntivi.HasAttribute("margine_operativo") = True Then
            Me.margine_operativo = XML_ParametriAggiuntivi.GetAttribute("margine_operativo")
        End If
        If XML_ParametriAggiuntivi.HasAttribute("costi_fissi") = True Then
            Me.costi_fissi = XML_ParametriAggiuntivi.GetAttribute("costi_fissi")
        End If
        If XML_ParametriAggiuntivi.HasAttribute("margine_netto") = True Then
            Me.margine_netto = XML_ParametriAggiuntivi.GetAttribute("margine_netto")
        End If
        If XML_ParametriAggiuntivi.HasAttribute("data_stampa") = True Then
            Me.data_stampa = XML_ParametriAggiuntivi.GetAttribute("data_stampa")
        End If

        If XML_ParametriAggiuntivi.HasAttribute("visualizzazione") = True Then
            Me.visualizzazione = XML_ParametriAggiuntivi.GetAttribute("visualizzazione")
        End If

        If XML_ParametriAggiuntivi.HasAttribute("id_agenda") = True Then
            Me.id_agenda = XML_ParametriAggiuntivi.GetAttribute("id_agenda")
        End If

        If XML_ParametriAggiuntivi.HasAttribute("rag_soc") = True Then
            Me.RagSoc = XML_ParametriAggiuntivi.GetAttribute("rag_soc")
        End If

        If XML_ParametriAggiuntivi.HasAttribute("piva") = True Then
            Me.Piva = XML_ParametriAggiuntivi.GetAttribute("piva")
        End If

        If XmlDoc.SelectSingleNode("Parametri").SelectSingleNode("ParametriAgronicaStampe_2010").ChildNodes.Count > 0 Then
            Dim StrNodiVariabili As String
            Dim i As Integer = 0
            For i = 0 To XmlDoc.SelectSingleNode("Parametri").SelectSingleNode("ParametriAgronicaStampe_2010").ChildNodes.Count - 1
                StrNodiVariabili &= XmlDoc.SelectSingleNode("Parametri").SelectSingleNode("ParametriAgronicaStampe_2010").ChildNodes.Item(i).OuterXml
            Next
            Me.Xml_Generico = New StringBuilder
            Me.Xml_Generico.Length = 0
            Me.Xml_Generico.Append(StrNodiVariabili)
        Else
            'lo inizializzo comunque altrimenbti se mando una stampa come la stampa risultato filtro che non ha 
            'nodi si incricca
            Me.Xml_Generico = New StringBuilder
            Me.Xml_Generico.Length = 0
        End If

        Me.JSon_Generico = New StringBuilder
        Me.JSon_Generico.Length = 0

    End Sub

#End Region


    ''' <summary>
    ''' Genera il blocco XML VariabiliAnalisi con all'interno 
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GeneraXML() As String
        Dim XmlDoc As New System.Xml.XmlDocument
        Dim Xml_VariabiliGias As System.Xml.XmlElement
        'Creo il nodo "FiltroStampa"
        Xml_VariabiliGias = XmlDoc.CreateElement("ParametriAgronicaStampe_2010")
        Xml_VariabiliGias.SetAttribute("username", username)
        Xml_VariabiliGias.SetAttribute("report", report)
        Xml_VariabiliGias.SetAttribute("user_profilo", user_profilo)

        'no lcase per compatibilità vecchie chiamate e gestione richieste
        Xml_VariabiliGias.SetAttribute("UserProfilo_CodFisc", UserProfilo_CodFisc)

        'uso i parametri e non le proprieta per fare in modo che non sostituisca i percentuali e apici
        'no lcase per compatibilità vecchie chiamate e gestione richieste
        Xml_VariabiliGias.SetAttribute("Sql_Filtro", _Sql_Filtro)
        Xml_VariabiliGias.SetAttribute("Xml_Filtro", _Xml_Filtro)

        Xml_VariabiliGias.SetAttribute("username_codfisc", username_codfisc)
        Xml_VariabiliGias.SetAttribute("user_profilo_codfisc", user_profilo_codfisc)
        Xml_VariabiliGias.SetAttribute("user_profilo_codgias", user_profilo_codgias)


        Xml_VariabiliGias.SetAttribute("centro_costo", centro_costo)
        Xml_VariabiliGias.SetAttribute("superficie", superficie)
        Xml_VariabiliGias.SetAttribute("ricavi", ricavi)
        Xml_VariabiliGias.SetAttribute("costi", costi)
        Xml_VariabiliGias.SetAttribute("differenza", differenza)
        Xml_VariabiliGias.SetAttribute("ricavi_ha", ricavi_ha)
        Xml_VariabiliGias.SetAttribute("costi_ha", costi_ha)
        Xml_VariabiliGias.SetAttribute("differenza_ha", differenza_ha)


        Xml_VariabiliGias.SetAttribute("ricavo_lordo", ricavo_lordo)
        Xml_VariabiliGias.SetAttribute("ricavo_totale", ricavo_totale)
        Xml_VariabiliGias.SetAttribute("costi_variabili", costi_variabili)
        Xml_VariabiliGias.SetAttribute("margine_lordo", margine_lordo)
        Xml_VariabiliGias.SetAttribute("costi_operativi", costi_operativi)
        Xml_VariabiliGias.SetAttribute("margine_operativo", margine_operativo)
        Xml_VariabiliGias.SetAttribute("costi_fissi", costi_fissi)
        Xml_VariabiliGias.SetAttribute("margine_netto", margine_netto)
        Xml_VariabiliGias.SetAttribute("data_stampa", data_stampa)

        Xml_VariabiliGias.SetAttribute("visualizzazione", visualizzazione)

        Xml_VariabiliGias.SetAttribute("id_agenda", id_agenda)

        If Xml_Generico.Length > 0 Then
            Dim strxml As String = Xml_Generico.ToString
            strxml = strxml.Replace("'", "&#39;")
            Xml_VariabiliGias.InnerXml = strxml
        End If

        XmlDoc.AppendChild(Xml_VariabiliGias)
        Dim str As String
        str = XmlDoc.InnerXml
        str = str.Replace("'", "&#39;")
        Return str
    End Function

    Private Function LeggiPiva() As String

        Dim piva As String = String.Empty
        If IsNothing(Xml_Generico) OrElse Xml_Generico.Length = 0 Then
            Return ""
        End If

        Dim xmlDoc As New XmlDocument()
        xmlDoc.LoadXml(Xml_Generico.ToString)
        Dim vs = xmlDoc.SelectSingleNode("VariabiliStampe")
        If Not IsNothing(vs) Then
            If Not IsNothing(vs.Attributes("piva")) Then
                piva = vs.Attributes("piva").Value
            End If
        End If

        Return piva

    End Function

End Class



'##################################################################################################
'##################################################################################################
'##################################################################################################
'#############################  X STAMPE  GESTIONE XML IN STINGNODI CHIAMATE SITI #################
'##################################################################################################
'##################################################################################################

' Giulia: 3/12/2018: si potrà anche eliminare. Utilizzare AgronicaCoreXML.XML_Stampe

'Public Class ElementiStampe

'    Public Structure ElementoStampe
'        Public Nome As String
'        Public Valore As String
'    End Structure

'End Class


'Public Module XML_Stampe



'    '####################################################################################
'    'Funzione da utilizzare per il nuovo modo di chiamata all'AgronicaStampe
'    'Crea il primo elemento con gli attributi necessari
'    'a questo elemento si aggiunge poi tutto il contenuto necessario per ogni report
'    Public Function Crea_Nodo_XML_VariabiliStampe(ByRef XmlDoc As XmlDocument, _
'                                                    ByVal Codice_Report As enum_CodificaStampe, _
'                                                    ByVal Piva As String) As XmlElement

'        Dim XML_VariabiliStampe As XmlElement

'        If IsNothing(XmlDoc) Then
'            XmlDoc = New XmlDocument
'        End If

'        XML_VariabiliStampe = XmlDoc.CreateElement("VariabiliStampe")
'        XmlDoc.AppendChild(XML_VariabiliStampe)

'        XML_VariabiliStampe.SetAttribute(CStr("Codice_Report").ToLower, Codice_Report)
'        XML_VariabiliStampe.SetAttribute(CStr("Report").ToLower, Codice_Report)
'        XML_VariabiliStampe.SetAttribute(CStr("Piva").ToLower, Piva)

'        Return XML_VariabiliStampe


'    End Function

'    '##########################################################################################
'    'nuovo metodo: sostituisce XML_VariabiliStampe
'    Public Sub CreaInserisci_SottoNodo_XML_VarStampa(ByRef XmlDoc As XmlDocument, _
'                                                    ByVal VetVariabili() As ElementiStampe.ElementoStampe) 'As String

'        If IsNothing(XmlDoc) Then
'            XmlDoc = New XmlDocument
'        End If

'        Dim XML_VariabiliStampe As XmlElement
'        Dim XML_VarStampa As XmlElement
'        Dim i As Integer

'        'Try

'        If VetVariabili.Length > 0 Then

'            'Creo il nodo 
'            XML_VarStampa = XmlDoc.CreateElement("VarStampa")

'            For i = 0 To VetVariabili.GetUpperBound(0)

'                'Imposto gli attributi come coppie nome-valore
'                XML_VarStampa.SetAttribute(LCase(VetVariabili(i).Nome), VetVariabili(i).Valore)

'            Next

'            XML_VariabiliStampe = XmlDoc.SelectSingleNode("VariabiliStampe")
'            XML_VariabiliStampe.AppendChild(XML_VarStampa)

'        End If

'    End Sub



'    '####################################################################################
'    'test x chiamata stampe excel agrisfera
'    Public Function StrXmlParametri_Report_89() As String

'        Dim str_XML As String = ""
'        Return str_XML

'    End Function

'    '################################################################################
'    'Questa funzione prepara l'xml da inviare alla stampe/e relativa/e
'    'ovvero il nodo varabiliStampe e il relativo contenuto (che varia in base al report)
'    'il nodo radice Parametri e il nodo VariabiliSessione vengono creati dopo
'    Public Function StrXmlParametri_Schede_Magazzino(ByVal Codice_Report As enum_CodificaStampe, _
'                                                    ByVal Piva As String, _
'                                                    ByVal Sa_Cod As Integer, _
'                                                    ByVal Fabbricato_Cod As Integer, _
'                                                    ByVal Elem_Cod As Integer, _
'                                                    ByVal Pro_Cod As Integer, _
'                                                    ByVal Mat_Cod As Integer, _
'                                                    ByVal Data_Stampa As Date, _
'                                                    ByVal Data_Inizio As Date, _
'                                                    ByVal Data_Fine As Date) As String

'        Dim XmlDoc As New XmlDocument
'        Dim XML_VariabiliStampe As XmlElement

'        XML_VariabiliStampe = Crea_Nodo_XML_VariabiliStampe(XmlDoc, Codice_Report, Piva)

'        Dim vVarStampe(5) As ElementiStampe.ElementoStampe

'        'If Data_Inizio <> AGRODATAINIZIO And Data_Fine <> AGRODATAFINE Then
'        '    Dim vVarStampe(7) As ElementiStampe.ElementoStampe
'        'Else
'        '    Dim vVarStampe(5) As ElementiStampe.ElementoStampe
'        'End If

'        vVarStampe(0).Nome = "sa_cod"
'        vVarStampe(0).Valore = Sa_Cod

'        vVarStampe(1).Nome = "fabbricato_cod"
'        vVarStampe(1).Valore = Fabbricato_Cod

'        vVarStampe(2).Nome = "elem_cod"
'        vVarStampe(2).Valore = Elem_Cod

'        vVarStampe(3).Nome = "pro_cod"
'        vVarStampe(3).Valore = Pro_Cod

'        vVarStampe(4).Nome = "mat_cod"
'        vVarStampe(4).Valore = Mat_Cod

'        If Data_Stampa <> AGRODATAINIZIO Then
'            vVarStampe(5).Nome = "data_stampa"
'            vVarStampe(5).Valore = Data_Stampa
'        Else
'            vVarStampe(5).Nome = "data_stampa"
'            vVarStampe(5).Valore = Date.Today
'        End If

'        CreaInserisci_SottoNodo_XML_VarStampa(XmlDoc, vVarStampe)


'        Return XML_VariabiliStampe.OuterXml

'    End Function

'    '##########################################################################################
'    Public Function XML_VariabiliSessione( _
'                                        ByVal Progressivo_Gias As Integer, _
'                                        ByVal Id_Servizio As Integer, _
'                                        ByVal PathFileINI As String, _
'                                        ByVal Cn_Server As String, _
'                                        ByVal Cn_Utenti As String, _
'                                        ByVal Stringa_Cn_Server As String, _
'                                        ByVal Stringa_Cn_Utenti As String, _
'                                        ByVal Cn_LogAccessi As String, _
'                                        ByVal FinestraTemporale_Inizio As Date, _
'                                        ByVal FinestraTemporale_Fine As Date, _
'                                        ByVal Utente_Usr As String, _
'                                        ByVal Utente_Pwd As String, _
'                                        ByVal Utente_Usr_Crypt As String, _
'                                        ByVal Utente_Pwd_Crypt As String, _
'                                        ByVal Utente_CodFiscale As String, _
'                                        ByVal SuperUser_Usr As String, _
'                                        ByVal SuperUser_Pwd As String, _
'                                        ByVal SuperUser_Usr_Crypt As String, _
'                                        ByVal SuperUser_Pwd_Crypt As String, _
'                                        ByVal SuperUser_Piva As String, _
'                                        ByVal AgronicaCore_Flag_CancellazioneLogica As Integer, _
'                                        ByVal AgronicaCore_Flag_Visibilita As Integer, _
'                                        ByVal AgronicaCore_FileNameLOG As String, _
'                                        ByVal PathDirectoryLOG As String) _
'                                        As String

'        Dim XmlDoc As New System.Xml.XmlDocument
'        Dim XmlTxt As System.Xml.XmlElement

'        '----- Genero la stringa XML a partire dai valori dei parametri

'        'Creo il nodo 
'        XmlTxt = XmlDoc.CreateElement("VariabiliSessione")

'        'Imposto gli attributi
'        XmlTxt.SetAttribute(LCase("Progressivo_Gias"), CStr(Progressivo_Gias))
'        XmlTxt.SetAttribute(LCase("Id_Servizio"), CStr(Id_Servizio))
'        XmlTxt.SetAttribute(LCase("PathFileINI"), CStr(PathFileINI))
'        XmlTxt.SetAttribute(LCase("Cn_Server"), CStr(Cn_Server))
'        XmlTxt.SetAttribute(LCase("Cn_Utenti"), CStr(Cn_Utenti))
'        XmlTxt.SetAttribute(LCase("Stringa_Cn_Server"), CStr(Stringa_Cn_Server))
'        XmlTxt.SetAttribute(LCase("Stringa_Cn_Utenti"), CStr(Stringa_Cn_Utenti))
'        XmlTxt.SetAttribute(LCase("Cn_LogAccessi"), CStr(Cn_LogAccessi))
'        XmlTxt.SetAttribute(LCase("FinestraTemporale_Inizio"), CStr(FinestraTemporale_Inizio))
'        XmlTxt.SetAttribute(LCase("FinestraTemporale_Fine"), CStr(FinestraTemporale_Fine))
'        XmlTxt.SetAttribute(LCase("Utente_Usr"), CStr(Utente_Usr))
'        XmlTxt.SetAttribute(LCase("Utente_Pwd"), CStr(Utente_Pwd))
'        XmlTxt.SetAttribute(LCase("Utente_Usr_Crypt"), CStr(Utente_Usr_Crypt))
'        XmlTxt.SetAttribute(LCase("Utente_Pwd_Crypt"), CStr(Utente_Pwd_Crypt))
'        XmlTxt.SetAttribute(LCase("Utente_CodFiscale"), CStr(Utente_CodFiscale))
'        XmlTxt.SetAttribute(LCase("SuperUser_Usr"), CStr(SuperUser_Usr))
'        XmlTxt.SetAttribute(LCase("SuperUser_Pwd"), CStr(SuperUser_Pwd))
'        XmlTxt.SetAttribute(LCase("SuperUser_Usr_Crypt"), CStr(SuperUser_Usr_Crypt))
'        XmlTxt.SetAttribute(LCase("SuperUser_Pwd_Crypt"), CStr(SuperUser_Pwd_Crypt))
'        XmlTxt.SetAttribute(LCase("SuperUser_Piva"), CStr(SuperUser_Piva))
'        XmlTxt.SetAttribute(LCase("AgronicaCore_Flag_CancellazioneLogica"), CStr(AgronicaCore_Flag_CancellazioneLogica))
'        XmlTxt.SetAttribute(LCase("AgronicaCore_Flag_Visibilita"), CStr(AgronicaCore_Flag_Visibilita))
'        XmlTxt.SetAttribute(LCase("AgronicaCore_FileNameLOG"), CStr(AgronicaCore_FileNameLOG))
'        XmlTxt.SetAttribute(LCase("PathDirectoryLOG"), CStr(PathDirectoryLOG))

'        'Imposto XmlTxt come figlio del documento principale
'        XmlDoc.AppendChild(XmlTxt)

'        'Restituisco in uscita la stringa creata
'        Return XmlDoc.InnerXml

'        'Distruggo gli oggetti
'        XmlTxt = Nothing
'        XmlDoc = Nothing

'    End Function



'    '##########################################################################################
'    'vecchio metodo
'    Public Function XML_VariabiliStampe(ByVal VetVariabili() As ElementiStampe.ElementoStampe) As String


'        Dim XmlDoc As System.Xml.XmlDocument
'        Dim XmlTxt As System.Xml.XmlElement
'        Dim xmlErrore As System.Xml.XmlElement
'        Dim i As Integer

'        Try

'            XmlDoc = New System.Xml.XmlDocument

'            If VetVariabili.Length > 0 Then

'                'Creo il nodo 
'                XmlTxt = XmlDoc.CreateElement("VariabiliStampe")

'                For i = 0 To VetVariabili.GetUpperBound(0)

'                    'Imposto gli attributi

'                    XmlTxt.SetAttribute(LCase(VetVariabili(i).Nome), VetVariabili(i).Valore)

'                Next

'                'Imposto XmlTxt come figlio del documento principale
'                XmlDoc.AppendChild(XmlTxt)

'            End If

'        Catch ex As Exception
'            XmlDoc = New System.Xml.XmlDocument
'            xmlErrore = XmlDoc.CreateElement("VariabiliStampe")
'            xmlErrore.SetAttribute("Errore", ex.Message)
'            XmlDoc.AppendChild(xmlErrore)
'        End Try

'        'Restituisco in uscita la stringa creata
'        Return XmlDoc.InnerXml

'        'Distruggo gli oggetti
'        XmlTxt = Nothing
'        XmlDoc = Nothing

'    End Function

'    '###############################################################################################
'    'vecchio metodo
'    Public Sub XML_EstraiVariabiliStampe(ByVal strXml As String, ByRef htVariabiliStampe As System.Collections.Hashtable, ByRef strErr As String)

'        Dim XmlDoc As New System.Xml.XmlDocument

'        Dim XML_VariabiliStampe As System.Xml.XmlElement
'        Dim xmlAttributo As System.Xml.XmlAttribute
'        Dim i As Integer
'        Dim risp As String

'        Try

'            'Carico la stringa xml in un nuovo documento
'            XmlDoc = New System.Xml.XmlDocument
'            XmlDoc.LoadXml(strXml)

'            If XmlDoc.HasChildNodes Then

'                i = 0
'                htVariabiliStampe = New System.Collections.Hashtable

'                'Recupero l'insieme dei nodi Movimento
'                XML_VariabiliStampe = XmlDoc.FirstChild
'                If XML_VariabiliStampe.HasAttributes Then
'                    For Each xmlAttributo In XML_VariabiliStampe.Attributes()

'                        If Not htVariabiliStampe.ContainsKey(xmlAttributo.Name) Then

'                            htVariabiliStampe.Add(xmlAttributo.Name, xmlAttributo.Value)

'                        End If

'                    Next

'                End If

'            End If

'        Catch ex As Exception
'            strErr = ex.Message
'            htVariabiliStampe = Nothing
'        End Try

'    End Sub

'    '################################################################################
'    'Questa funzione prepara l'xml da inviare alla stampe/e relativa/e
'    'ovvero il nodo varabiliStampe e il relativo contenuto (che varia in base al report)
'    'il nodo radice Parametri e il nodo VariabiliSessione vengono creati dopo
'    Public Function StrXmlParametri_GestioneEtichette_Trasformati(ByVal Codice_Report As enum_CodificaStampe, _
'                                                                ByVal Piva As String, _
'                                                                ByVal Mat_Cod As Integer) As String

'        Dim XmlDoc As New XmlDocument
'        Dim XML_VariabiliStampe As XmlElement

'        XML_VariabiliStampe = Crea_Nodo_XML_VariabiliStampe(XmlDoc, Codice_Report, Piva)

'        Dim vVarStampe(0) As ElementiStampe.ElementoStampe

'        vVarStampe(0).Nome = "mat_cod"
'        vVarStampe(0).Valore = Mat_Cod

'        CreaInserisci_SottoNodo_XML_VarStampa(XmlDoc, vVarStampe)

'        Return XML_VariabiliStampe.OuterXml

'    End Function

'    '################################################################################
'    'Questa funzione prepara l'xml da inviare alla stampe/e relativa/e
'    'ovvero il nodo varabiliStampe e il relativo contenuto (che varia in base al report)
'    'il nodo radice Parametri e il nodo VariabiliSessione vengono creati dopo
'    Public Function StrXmlParametri_GestioneSchedeBio(ByVal Codice_Report As enum_CodificaStampe, _
'                                                    ByVal Piva As String) As String

'        Dim XmlDoc As New XmlDocument
'        Dim XML_VariabiliStampe As XmlElement

'        XML_VariabiliStampe = Crea_Nodo_XML_VariabiliStampe(XmlDoc, Codice_Report, Piva)

'        Return XML_VariabiliStampe.OuterXml

'    End Function



'    '################################################################################
'    'Questa funzione prepara l'xml da inviare alla stampe/e relativa/e
'    'ovvero il nodo varabiliStampe e il relativo contenuto (che varia in base al report)
'    'il nodo radice Parametri e il nodo VariabiliSessione vengono creati dopo
'    Public Function StrXmlParametri_GestioneReportIncongruenze(ByVal Codice_Report As enum_CodificaStampe, _
'                                                    ByVal Piva As String) As String

'        Dim XmlDoc As New XmlDocument
'        Dim XML_VariabiliStampe As XmlElement

'        XML_VariabiliStampe = Crea_Nodo_XML_VariabiliStampe(XmlDoc, Codice_Report, Piva)

'        Return XML_VariabiliStampe.OuterXml

'    End Function


'Public Function StrXmlParametri_FiltroElaboratiContabili(ByVal Codice_Report As enum_CodificaStampe, _
'                                                            ByVal Piva As String, _
'                                                                ByVal Rag_Soc As String, _
'                                                                ByVal Anno As Integer, _
'                                                                ByVal Data_Inizio As String, _
'                                                                ByVal Data_Fine As String) As String

'    Dim XmlDoc As New XmlDocument
'    Dim XML_VariabiliStampe As XmlElement

'    XML_VariabiliStampe = Crea_Nodo_XML_VariabiliStampe(XmlDoc, Codice_Report, Piva)

'    XML_VariabiliStampe.SetAttribute("piva", Piva)
'    XML_VariabiliStampe.SetAttribute("anno", Anno)
'    XML_VariabiliStampe.SetAttribute("data_inizio", Data_Inizio)
'    XML_VariabiliStampe.SetAttribute("data_fine", Data_Fine)
'    XML_VariabiliStampe.SetAttribute("rag_soc", Rag_Soc)

'    Return XML_VariabiliStampe.OuterXml


'End Function


'    '##################################################################################################
'    '##################################################################################################
'    '##################################################################################################
'    '#############################  X STAMPE  FINE ####################################################
'    '##################################################################################################
'    '##################################################################################################





'#################################################################################################
''' <summary>
''' AGRONICA bio
''' </summary>
''' <remarks></remarks>
Public Class ParametriAgronicaBio

    Private _Piva As String
    Private _Pagina_Richiesta As Integer 'tipo enumerativo enum_PagineAgronicaMeteo
    Private _username_codfisc As String

#Region "Proprietà"
    Public Property Pagina_Richiesta() As Integer
        Get
            Return _Pagina_Richiesta
        End Get
        Set(ByVal value As Integer)
            _Pagina_Richiesta = value
            Salva()
        End Set
    End Property


    Public Property Piva() As String
        Get
            Return _Piva
        End Get
        Set(ByVal value As String)
            _Piva = value
            Salva()
        End Set
    End Property

    Public Property username_codfisc() As String
        Get
            Return _username_codfisc
        End Get
        Set(ByVal value As String)
            _username_codfisc = value
            Salva()
        End Set
    End Property

#End Region

#Region "Costruttori"

    Sub New()

    End Sub


    Sub New(ByVal XML As String)
        LeggiXML(XML)
    End Sub

    Private Sub LeggiXML(ByVal XML As String)
        Dim XmlDoc As New System.Xml.XmlDocument
        XmlDoc.LoadXml(XML)

        Dim XML_ParametriAggiuntivi As System.Xml.XmlElement
        XML_ParametriAggiuntivi = XmlDoc.SelectSingleNode("Parametri").SelectSingleNode("VariabiliBio")

        If XML_ParametriAggiuntivi.HasAttribute(LCase("piva")) = True Then
            Me.Piva = XML_ParametriAggiuntivi.GetAttribute(LCase("piva"))
        End If
        If XML_ParametriAggiuntivi.HasAttribute(LCase("paginabiorichiesta")) = True Then
            Me.Pagina_Richiesta = XML_ParametriAggiuntivi.GetAttribute(LCase("paginabiorichiesta"))
        End If
        If XML_ParametriAggiuntivi.HasAttribute(LCase("username_codfisc")) = True Then
            Me.username_codfisc = XML_ParametriAggiuntivi.GetAttribute(LCase("username_codfisc"))
        End If
    End Sub
#End Region


#Region "x Sessione"
    Public Sub Salva()
        HttpContext.Current.Session("VariabiliBio") = Me
    End Sub

    Public Sub Leggi()
        Dim obj As New ParametriAgronicaBio
        obj = HttpContext.Current.Session("VariabiliBio")
        If Not IsNothing(obj) Then
            _Piva = obj.Piva
            _Pagina_Richiesta = obj.Pagina_Richiesta
            _username_codfisc = obj.username_codfisc
        End If
    End Sub
#End Region

    ''' <summary>
    ''' Genera il blocco XML Variabili con all'interno 
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GeneraXML() As String
        Dim XmlDoc As New System.Xml.XmlDocument
        Dim Xml_Variabili As System.Xml.XmlElement
        'Creo il nodo "FiltroStampa"
        Xml_Variabili = XmlDoc.CreateElement("VariabiliBio")

        Xml_Variabili.SetAttribute("piva", CStr(Piva))
        Xml_Variabili.SetAttribute("paginabiorichiesta", CStr(Pagina_Richiesta))
        Xml_Variabili.SetAttribute("username_codfisc", CStr(username_codfisc))

        XmlDoc.AppendChild(Xml_Variabili)
        Dim str As String
        str = XmlDoc.InnerXml
        str = str.Replace("'", "&#39;")
        Return str
    End Function


End Class



'#################################################################################################
''' <summary>
''' AGRONICA PUA
''' </summary>
''' <remarks></remarks>
Public Class ParametriAgronicaAuditPUA

    Private _Piva As String
    Private _RagSoc As String = ""
    Private _Tipo_Audit As Integer 'tipo enumerativo enum_PagineAgronicaMeteo
    Private _username_codfisc As String
    Private _programmazione_cod As Integer
    Private _tipo_operazione As Integer
    Private _sito_origine As Integer
    Private _Regolamento_Cod As Integer
    Private _Audit_Cod As Integer

    Private _LinkAgronicaAgenda2010 As String

    Private _Sessione As Boolean

#Region "Proprietà"

    Public Property Sessione() As Boolean
        Get
            Return _Sessione
        End Get
        Set(value As Boolean)
            _Sessione = value
        End Set
    End Property


    Public Property Tipo_Audit() As Integer
        Get
            Return _Tipo_Audit
        End Get
        Set(ByVal value As Integer)
            _Tipo_Audit = value
            Salva()
        End Set
    End Property


    Public Property Piva() As String
        Get
            Return _Piva
        End Get
        Set(ByVal value As String)
            _Piva = value
            Salva()
        End Set
    End Property

    Public Property username_codfisc() As String
        Get
            Return _username_codfisc
        End Get
        Set(ByVal value As String)
            _username_codfisc = value
            Salva()
        End Set
    End Property

    Public Property programmazione_cod() As Integer
        Get
            Return _programmazione_cod
        End Get
        Set(ByVal value As Integer)
            _programmazione_cod = value
            Salva()
        End Set
    End Property

    Public Property Tipo_Operazione() As Integer
        Get
            Return _tipo_operazione
        End Get
        Set(ByVal value As Integer)
            _tipo_operazione = value
            Salva()
        End Set
    End Property

    Public Property sito_origine() As Integer
        Get
            Return _sito_origine
        End Get
        Set(ByVal value As Integer)
            _sito_origine = value
            Salva()
        End Set
    End Property

    Public Property Regolamento_Cod() As Integer
        Get
            Return _Regolamento_Cod
        End Get
        Set(ByVal value As Integer)
            _Regolamento_Cod = value
            Salva()
        End Set
    End Property

    Public Property Audit_Cod() As Integer
        Get
            Return _Audit_Cod
        End Get
        Set(ByVal value As Integer)
            _Audit_Cod = value
            Salva()
        End Set
    End Property

    Property LinkAgronicaAgenda2010() As String
        Get
            Return _LinkAgronicaAgenda2010
        End Get
        Set(ByVal Value As String)
            _LinkAgronicaAgenda2010 = Value
            Salva()
        End Set
    End Property

    Public Property RagSoc() As String
        Get
            Return _RagSoc
        End Get
        Set(ByVal value As String)
            _RagSoc = value
            Salva()
        End Set
    End Property

#End Region

#Region "Costruttori"

    Sub New()
        _Sessione = True
    End Sub


    Sub New(ByVal XML As String)
        _Sessione = True
        LeggiXML(XML)
    End Sub

    Private Sub LeggiXML(ByVal XML As String)
        Dim XmlDoc As New System.Xml.XmlDocument
        XmlDoc.LoadXml(XML)

        Dim XML_ParametriAggiuntivi As System.Xml.XmlElement
        XML_ParametriAggiuntivi = XmlDoc.SelectSingleNode("Parametri").SelectSingleNode("VariabiliAudit")

        If XML_ParametriAggiuntivi.HasAttribute(LCase("piva")) = True Then
            Me.Piva = XML_ParametriAggiuntivi.GetAttribute(LCase("piva"))
        End If
        If XML_ParametriAggiuntivi.HasAttribute(LCase("ragsoc")) = True Then
            Me.RagSoc = XML_ParametriAggiuntivi.GetAttribute(LCase("ragsoc"))
        End If
        If XML_ParametriAggiuntivi.HasAttribute(LCase("tipo_audit")) = True Then
            Me.Tipo_Audit = XML_ParametriAggiuntivi.GetAttribute(LCase("tipo_audit"))
        End If
        If XML_ParametriAggiuntivi.HasAttribute(LCase("username_codfisc")) = True Then
            Me.username_codfisc = XML_ParametriAggiuntivi.GetAttribute(LCase("username_codfisc"))
        End If
        If XML_ParametriAggiuntivi.HasAttribute(LCase("programmazione_cod")) = True Then
            Me.programmazione_cod = XML_ParametriAggiuntivi.GetAttribute(LCase("programmazione_cod"))
        End If
        If XML_ParametriAggiuntivi.HasAttribute(LCase("tipo_operazione")) = True Then
            Me.Tipo_Operazione = XML_ParametriAggiuntivi.GetAttribute(LCase("tipo_operazione"))
        End If
        If XML_ParametriAggiuntivi.HasAttribute(LCase("sito_origine")) = True Then
            Me.sito_origine = XML_ParametriAggiuntivi.GetAttribute(LCase("sito_origine"))
        End If
        If XML_ParametriAggiuntivi.HasAttribute(LCase("regolamento_Cod")) = True Then
            Me.Regolamento_Cod = XML_ParametriAggiuntivi.GetAttribute(LCase("regolamento_Cod"))
        End If
        If XML_ParametriAggiuntivi.HasAttribute(LCase("audit_cod")) = True Then
            Me.Audit_Cod = XML_ParametriAggiuntivi.GetAttribute(LCase("audit_cod"))
        End If
        If XML_ParametriAggiuntivi.HasAttribute(LCase("linkagronicaagenda2010")) = True Then
            Me.LinkAgronicaAgenda2010 = XML_ParametriAggiuntivi.GetAttribute(LCase("linkagronicaagenda2010"))
        End If
    End Sub
#End Region


#Region "x Sessione"
    Public Sub Salva()
        If _Sessione Then
            HttpContext.Current.Session("VariabiliAudit") = Me
        End If
    End Sub

    Public Sub Leggi()
        If _Sessione Then
            Dim obj As New ParametriAgronicaAuditPUA
            obj = HttpContext.Current.Session("VariabiliAudit")

            If Not IsNothing(obj) Then
                _Piva = obj.Piva
                _Tipo_Audit = obj.Tipo_Audit
                _username_codfisc = obj.username_codfisc
                _programmazione_cod = obj.programmazione_cod
                _tipo_operazione = obj.Tipo_Operazione
                _sito_origine = obj.sito_origine
                _Regolamento_Cod = obj.Regolamento_Cod
                _Audit_Cod = obj.Audit_Cod
                _LinkAgronicaAgenda2010 = obj.LinkAgronicaAgenda2010

            End If
        End If

    End Sub
#End Region

    ''' <summary>
    ''' Genera il blocco XML Variabili con all'interno 
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GeneraXML() As String
        Dim XmlDoc As New System.Xml.XmlDocument
        Dim Xml_Variabili As System.Xml.XmlElement
        'Creo il nodo "FiltroStampa"
        Xml_Variabili = XmlDoc.CreateElement("VariabiliAudit")

        Xml_Variabili.SetAttribute("piva", CStr(Piva))
        Xml_Variabili.SetAttribute("ragsoc", CStr(RagSoc))
        Xml_Variabili.SetAttribute("tipo_audit", CStr(Tipo_Audit))
        Xml_Variabili.SetAttribute("username_codfisc", CStr(username_codfisc))
        Xml_Variabili.SetAttribute("programmazione_cod", CStr(programmazione_cod))
        Xml_Variabili.SetAttribute("tipo_operazione", CStr(Tipo_Operazione))
        Xml_Variabili.SetAttribute("sito_origine", CStr(sito_origine))
        Xml_Variabili.SetAttribute("regolamento_cod", CStr(Regolamento_Cod))
        Xml_Variabili.SetAttribute("audit_cod", CStr(Audit_Cod))
        Xml_Variabili.SetAttribute("linkagronicaagenda2010", CStr(LinkAgronicaAgenda2010))

        XmlDoc.AppendChild(Xml_Variabili)
        Dim str As String
        str = XmlDoc.InnerXml
        str = str.Replace("'", "&#39;")
        Return str
    End Function


End Class



''' <summary>
''' Tenere aggiornato anche AgronicaCoreDTOStd\InData\Parametri_ObjParametriAgenda_NG.cs
''' </summary>
Public Class Parametri_ObjParametriAgenda_NG
    Public TipoOperazioneDB As Integer?
    Public Piva As String
    Public Sa_Cod As Long?
    Public Fabbricato As Long?
    Public Campo_Cod As Long?
    Public Appezza As Long?
    Public Id_Reg As Long?
    Public Progetto_Cod As Long?
    Public Validita_Inizio As Date
    Public Validita_Fine As Date
    Public Veg_Cod As Integer?
    Public Veg_Des As String
    Public Id_Cod As Integer?
    Public Id_Des As String
    Public Cau_Mov As Integer?
    Public Mac_Cod As Integer?
    Public Lav_Des As String
    Public Lav_Cod As Integer?
    Public SaNome As String
    Public RagSoc As String
    Public Pagina_Provenienza As Integer?
    Public Pagina_Provenienza_AltroSito As Integer?
    Public Pagina_Richiesta As Integer?
    Public Data As Date
    Public Cod_Contatto As String
    Public QueryStringFiltrino As String
    Public Impianti As List(Of ImpiantiAgendaNG)
    Public Id_Agenda As Integer
    Public TipoOperazioneAgenda As AgronicaCoreModelsSTD.attivita.Attivita.Tipo_Attivita
    Public TipoRicetta As AgronicaCoreModelsSTD.attivita.Attivita.Tipo_Ricetta
    Public Stato As AgronicaCoreModelsSTD.attivita.Attivita.Stati
    Public Ricetta_Operazione_Cod As Integer
    Public Ricetta_Cod As Integer
    Public GenericObj_string As String
    Public TargetOperazione As enum_Tipo_Operazione_Agenda_Target
    Public Programmazione_Cod As Integer
    Public RedirectUrl As String
    Public IdSezione As Integer
    Public Chiave As String
    Public Sito_Provenienza As Integer
    Public Regolamento_Cod As Integer
    Public Tipo_Regolamento As Integer

#Region "Costruttori"

    Sub New()
        TipoOperazioneDB = 0
        Piva = ""
        Sa_Cod = 0
        Fabbricato = 0
        Campo_Cod = 0
        Appezza = 0
        Id_Reg = 0
        Progetto_Cod = 0
        Validita_Inizio = AGRODATAINIZIO
        Validita_Fine = AGRODATAFINE
        Veg_Cod = 0
        Veg_Des = ""
        Id_Cod = 0
        Id_Des = ""
        Cau_Mov = 0
        Mac_Cod = 0
        Lav_Des = ""
        Lav_Cod = 0
        SaNome = ""
        RagSoc = ""
        Pagina_Provenienza = 0
        Pagina_Provenienza_AltroSito = 0
        Pagina_Richiesta = 0
        Data = AGRODATAINIZIO
        Cod_Contatto = ""
        QueryStringFiltrino = ""
        Impianti = New List(Of ImpiantiAgendaNG)
        Id_Agenda = 0
        TipoOperazioneAgenda = AgronicaCoreModelsSTD.attivita.Attivita.Tipo_Attivita.QuadernoDiCampagna
        TipoRicetta = 0
        Stato = 0
        Ricetta_Operazione_Cod = 0
        Ricetta_Cod = 0
        GenericObj_string = ""
        TargetOperazione = enum_Tipo_Operazione_Agenda_Target.Reale
        Programmazione_Cod = 0
        RedirectUrl = String.Empty
        IdSezione = 0
        Sito_Provenienza = 0
        Regolamento_Cod = 0
        Tipo_Regolamento = 0
    End Sub


    Sub New(ByVal XML As String)
        LeggiXML(XML)
    End Sub

    Private Sub LeggiXML(ByVal XML As String)
        Dim XmlDoc As New System.Xml.XmlDocument
        XmlDoc.LoadXml(XML)

        Dim XML_ParametriAggiuntivi As System.Xml.XmlElement
        XML_ParametriAggiuntivi = XmlDoc.SelectSingleNode("Parametri").SelectSingleNode("Parametri_ObjParametriAgenda_NG")
        If XML_ParametriAggiuntivi.HasAttribute(LCase("pagina_richiesta")) = True Then
            Me.Pagina_Richiesta = XML_ParametriAggiuntivi.GetAttribute(LCase("pagina_richiesta"))
        End If
        If XML_ParametriAggiuntivi.HasAttribute(LCase("pagina_provenienza")) = True Then
            Me.Pagina_Provenienza = XML_ParametriAggiuntivi.GetAttribute(LCase("pagina_provenienza"))
        End If
        If XML_ParametriAggiuntivi.HasAttribute(LCase("pagina_provenienza_altrosito")) = True Then
            Me.Pagina_Provenienza_AltroSito = XML_ParametriAggiuntivi.GetAttribute(LCase("pagina_provenienza_altrosito"))
        End If
        If XML_ParametriAggiuntivi.HasAttribute(LCase("TipoOperazioneDB")) = True Then
            Me.TipoOperazioneDB = XML_ParametriAggiuntivi.GetAttribute(LCase("TipoOperazioneDB"))
        End If
        If XML_ParametriAggiuntivi.HasAttribute(LCase("piva")) = True Then
            Me.Piva = XML_ParametriAggiuntivi.GetAttribute(LCase("piva"))
        End If
        If XML_ParametriAggiuntivi.HasAttribute(LCase("Sa_Cod")) = True Then
            Me.Sa_Cod = XML_ParametriAggiuntivi.GetAttribute(LCase("Sa_Cod"))
        End If
        If XML_ParametriAggiuntivi.HasAttribute(LCase("Campo_Cod")) = True Then
            Me.Campo_Cod = XML_ParametriAggiuntivi.GetAttribute(LCase("Campo_Cod"))
        End If
        If XML_ParametriAggiuntivi.HasAttribute(LCase("Appezza")) = True Then
            Me.Appezza = XML_ParametriAggiuntivi.GetAttribute(LCase("Appezza"))
        End If
        If XML_ParametriAggiuntivi.HasAttribute(LCase("Id_Reg")) = True Then
            Me.Id_Reg = XML_ParametriAggiuntivi.GetAttribute(LCase("Id_Reg"))
        End If
        If XML_ParametriAggiuntivi.HasAttribute(LCase("Validita_Inizio")) = True Then
            Me.Validita_Inizio = XML_ParametriAggiuntivi.GetAttribute(LCase("Validita_Inizio"))
        End If
        If XML_ParametriAggiuntivi.HasAttribute(LCase("Validita_Fine")) = True Then
            Me.Validita_Fine = XML_ParametriAggiuntivi.GetAttribute(LCase("Validita_Fine"))
        End If
        If XML_ParametriAggiuntivi.HasAttribute(LCase("Veg_Cod")) = True Then
            Me.Veg_Cod = XML_ParametriAggiuntivi.GetAttribute(LCase("Veg_Cod"))
        End If
        If XML_ParametriAggiuntivi.HasAttribute(LCase("Veg_Des")) = True Then
            Me.Veg_Des = XML_ParametriAggiuntivi.GetAttribute(LCase("Veg_Des"))
        End If
        If XML_ParametriAggiuntivi.HasAttribute(LCase("Id_Cod")) = True Then
            Me.Id_Cod = XML_ParametriAggiuntivi.GetAttribute(LCase("Id_Cod"))
        End If
        If XML_ParametriAggiuntivi.HasAttribute(LCase("Id_Des")) = True Then
            Me.Id_Des = XML_ParametriAggiuntivi.GetAttribute(LCase("Id_Des"))
        End If
        If XML_ParametriAggiuntivi.HasAttribute(LCase("Cau_Mov")) = True Then
            Me.Cau_Mov = XML_ParametriAggiuntivi.GetAttribute(LCase("Cau_Mov"))
        End If
        If XML_ParametriAggiuntivi.HasAttribute(LCase("Mac_Cod")) = True Then
            Me.Mac_Cod = XML_ParametriAggiuntivi.GetAttribute(LCase("Mac_Cod"))
        End If
        If XML_ParametriAggiuntivi.HasAttribute(LCase("Lav_Des")) = True Then
            Me.Lav_Des = XML_ParametriAggiuntivi.GetAttribute(LCase("Lav_Des"))
        End If
        If XML_ParametriAggiuntivi.HasAttribute(LCase("Lav_Cod")) = True Then
            Me.Lav_Cod = XML_ParametriAggiuntivi.GetAttribute(LCase("Lav_Cod"))
        End If
        If XML_ParametriAggiuntivi.HasAttribute(LCase("SaNome")) = True Then
            Me.SaNome = XML_ParametriAggiuntivi.GetAttribute(LCase("SaNome"))
        End If
        If XML_ParametriAggiuntivi.HasAttribute(LCase("RagSoc")) = True Then
            Me.RagSoc = XML_ParametriAggiuntivi.GetAttribute(LCase("RagSoc"))
        End If
        If XML_ParametriAggiuntivi.HasAttribute(LCase("GenericObj_string")) = True Then
            Me.GenericObj_string = XML_ParametriAggiuntivi.GetAttribute(LCase("GenericObj_string"))
        End If
        If XML_ParametriAggiuntivi.HasAttribute(LCase("TipoOperazioneAgenda")) = True Then
            Me.TipoOperazioneAgenda = XML_ParametriAggiuntivi.GetAttribute(LCase("TipoOperazioneAgenda"))
        End If
        If XML_ParametriAggiuntivi.HasAttribute(LCase("TargetOperazione")) = True Then
            Me.TargetOperazione = XML_ParametriAggiuntivi.GetAttribute(LCase("TargetOperazione"))
        End If
        If XML_ParametriAggiuntivi.HasAttribute(LCase("Programmazione_Cod")) = True Then
            Me.Programmazione_Cod = XML_ParametriAggiuntivi.GetAttribute(LCase("Programmazione_Cod"))
        End If
        If XML_ParametriAggiuntivi.HasAttribute(LCase("querystringfiltrino")) = True Then
            Me.QueryStringFiltrino = XML_ParametriAggiuntivi.GetAttribute(LCase("querystringfiltrino"))
        End If
        If XML_ParametriAggiuntivi.HasAttribute(LCase("redirecturl")) = True Then
            Me.RedirectUrl = XML_ParametriAggiuntivi.GetAttribute(LCase("redirecturl"))
        End If
        If XML_ParametriAggiuntivi.HasAttribute(LCase("Fabbricato")) = True Then
            Me.Fabbricato = XML_ParametriAggiuntivi.GetAttribute(LCase("Fabbricato"))
        End If
        If XML_ParametriAggiuntivi.HasAttribute(LCase("Progetto_Cod")) = True Then
            Me.Progetto_Cod = XML_ParametriAggiuntivi.GetAttribute(LCase("Progetto_Cod"))
        End If
        If XML_ParametriAggiuntivi.HasAttribute(LCase("Data")) = True Then
            Me.Data = XML_ParametriAggiuntivi.GetAttribute(LCase("Data"))
        End If
        If XML_ParametriAggiuntivi.HasAttribute(LCase("Cod_Contatto")) = True Then
            Me.Cod_Contatto = XML_ParametriAggiuntivi.GetAttribute(LCase("Cod_Contatto"))
        End If
        If XML_ParametriAggiuntivi.HasAttribute(LCase("Impianti")) = True Then
            Me.Impianti = JsonConvert.DeserializeObject(Of List(Of ImpiantiAgendaNG))(XML_ParametriAggiuntivi.GetAttribute(LCase("Impianti")))
        End If
        If XML_ParametriAggiuntivi.HasAttribute(LCase("Id_Agenda")) = True Then
            Me.Id_Agenda = XML_ParametriAggiuntivi.GetAttribute(LCase("Id_Agenda"))
        End If
        If XML_ParametriAggiuntivi.HasAttribute(LCase("TipoRicetta")) = True Then
            Me.TipoRicetta = XML_ParametriAggiuntivi.GetAttribute(LCase("TipoRicetta"))
        End If
        If XML_ParametriAggiuntivi.HasAttribute(LCase("Stato")) = True Then
            Me.Stato = XML_ParametriAggiuntivi.GetAttribute(LCase("Stato"))
        End If
        If XML_ParametriAggiuntivi.HasAttribute(LCase("Ricetta_Operazione_Cod")) = True Then
            Me.Ricetta_Operazione_Cod = XML_ParametriAggiuntivi.GetAttribute(LCase("Ricetta_Operazione_Cod"))
        End If
        If XML_ParametriAggiuntivi.HasAttribute(LCase("IdSezione")) = True Then
            If IsNumeric(XML_ParametriAggiuntivi.GetAttribute(LCase("IdSezione"))) Then
                Me.IdSezione = CInt(XML_ParametriAggiuntivi.GetAttribute(LCase("IdSezione")))
            End If
        End If
        If XML_ParametriAggiuntivi.HasAttribute(LCase("Ricetta_Cod")) = True Then
            Me.Ricetta_Cod = XML_ParametriAggiuntivi.GetAttribute(LCase("Ricetta_Cod"))
        End If
        If XML_ParametriAggiuntivi.HasAttribute(LCase("sito_provenienza")) = True Then
            Me.Sito_Provenienza = XML_ParametriAggiuntivi.GetAttribute(LCase("sito_provenienza"))
        End If
        If XML_ParametriAggiuntivi.HasAttribute(LCase("regolamento_cod")) = True Then
            Me.Regolamento_Cod = XML_ParametriAggiuntivi.GetAttribute(LCase("regolamento_cod"))
        End If
        If XML_ParametriAggiuntivi.HasAttribute(LCase("tipo_regolamento")) = True Then
            Me.Tipo_Regolamento = XML_ParametriAggiuntivi.GetAttribute(LCase("tipo_regolamento"))
        End If

        Salva()
    End Sub
#End Region


#Region "x Sessione"
    Public Sub Salva()
        HttpContext.Current.Session("Parametri_ObjParametriAgenda_NG") = Me
    End Sub
    Public Sub Leggi()
        Dim obj As New Parametri_ObjParametriAgenda_NG
        obj = HttpContext.Current.Session("Parametri_ObjParametriAgenda_NG")
        If Not IsNothing(obj) Then
            TipoOperazioneDB = obj.TipoOperazioneDB
            Piva = obj.Piva
            Sa_Cod = obj.Sa_Cod
            Fabbricato = obj.Fabbricato
            Campo_Cod = obj.Campo_Cod
            Appezza = obj.Appezza
            Id_Reg = obj.Id_Reg
            Progetto_Cod = obj.Progetto_Cod
            Validita_Inizio = obj.Validita_Inizio
            Validita_Fine = obj.Validita_Fine
            Veg_Cod = obj.Veg_Cod
            Veg_Des = obj.Veg_Des
            Id_Cod = obj.Id_Cod
            Id_Des = obj.Id_Des
            Cau_Mov = obj.Cau_Mov
            Mac_Cod = obj.Mac_Cod
            Lav_Cod = obj.Lav_Cod
            Lav_Des = obj.Lav_Des
            SaNome = obj.SaNome
            RagSoc = obj.RagSoc
            Pagina_Provenienza = obj.Pagina_Provenienza
            Pagina_Provenienza_AltroSito = obj.Pagina_Provenienza_AltroSito
            Pagina_Richiesta = obj.Pagina_Richiesta
            TipoOperazioneAgenda = obj.TipoOperazioneAgenda
            TargetOperazione = obj.TargetOperazione
            Programmazione_Cod = obj.Programmazione_Cod
            QueryStringFiltrino = obj.QueryStringFiltrino
            RedirectUrl = obj.RedirectUrl
            Data = obj.Data
            Cod_Contatto = obj.Cod_Contatto
            Impianti = obj.Impianti
            Id_Agenda = obj.Id_Agenda
            TipoRicetta = obj.TipoRicetta
            Stato = obj.Stato
            Ricetta_Operazione_Cod = obj.Ricetta_Operazione_Cod
            Ricetta_Cod = obj.Ricetta_Cod
            GenericObj_string = obj.GenericObj_string
            IdSezione = obj.IdSezione
            Sito_Provenienza = obj.Sito_Provenienza
            Regolamento_Cod = obj.Regolamento_Cod
            Tipo_Regolamento = obj.Tipo_Regolamento
        End If
    End Sub
#End Region

    ''' <summary>
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GeneraXML() As String
        Dim XmlDoc As New System.Xml.XmlDocument
        Dim Xml_VariabiliGiasOnline_2010 As System.Xml.XmlElement

        Xml_VariabiliGiasOnline_2010 = XmlDoc.CreateElement("Parametri_ObjParametriAgenda_NG")
        Xml_VariabiliGiasOnline_2010.SetAttribute(LCase("pagina_richiesta"), CStr(Pagina_Richiesta))
        Xml_VariabiliGiasOnline_2010.SetAttribute(LCase("pagina_provenienza"), CStr(Pagina_Provenienza))
        Xml_VariabiliGiasOnline_2010.SetAttribute(LCase("pagina_provenienza_altrosito"), CStr(Pagina_Provenienza_AltroSito))
        Xml_VariabiliGiasOnline_2010.SetAttribute(LCase("TipoOperazioneDB"), CStr(TipoOperazioneDB))
        Xml_VariabiliGiasOnline_2010.SetAttribute(LCase("piva"), CStr(Piva))
        Xml_VariabiliGiasOnline_2010.SetAttribute(LCase("Sa_Cod"), CStr(Sa_Cod))
        Xml_VariabiliGiasOnline_2010.SetAttribute(LCase("Fabbricato"), CStr(Fabbricato))
        Xml_VariabiliGiasOnline_2010.SetAttribute(LCase("Campo_Cod"), CStr(Campo_Cod))
        Xml_VariabiliGiasOnline_2010.SetAttribute(LCase("Appezza"), CStr(Appezza))
        Xml_VariabiliGiasOnline_2010.SetAttribute(LCase("Id_Reg"), CStr(Id_Reg))
        Xml_VariabiliGiasOnline_2010.SetAttribute(LCase("Progetto_Cod"), CStr(Progetto_Cod))
        Xml_VariabiliGiasOnline_2010.SetAttribute(LCase("Validita_Inizio"), CStr(Validita_Inizio))
        Xml_VariabiliGiasOnline_2010.SetAttribute(LCase("Validita_Fine"), CStr(Validita_Fine))
        Xml_VariabiliGiasOnline_2010.SetAttribute(LCase("Veg_Cod"), CStr(Veg_Cod))
        Xml_VariabiliGiasOnline_2010.SetAttribute(LCase("Veg_Des"), CStr(Veg_Des))
        Xml_VariabiliGiasOnline_2010.SetAttribute(LCase("Id_Cod"), CStr(Id_Cod))
        Xml_VariabiliGiasOnline_2010.SetAttribute(LCase("Id_Des"), CStr(Id_Des))
        Xml_VariabiliGiasOnline_2010.SetAttribute(LCase("Cau_Mov"), CStr(Cau_Mov))
        Xml_VariabiliGiasOnline_2010.SetAttribute(LCase("Mac_Cod"), CStr(Mac_Cod))
        Xml_VariabiliGiasOnline_2010.SetAttribute(LCase("Lav_Cod"), CStr(Lav_Cod))
        Xml_VariabiliGiasOnline_2010.SetAttribute(LCase("Lav_Des"), CStr(Lav_Des))
        Xml_VariabiliGiasOnline_2010.SetAttribute(LCase("SaNome"), CStr(SaNome))
        Xml_VariabiliGiasOnline_2010.SetAttribute(LCase("RagSoc"), CStr(RagSoc))
        Xml_VariabiliGiasOnline_2010.SetAttribute(LCase("TipoOperazioneAgenda"), CStr(TipoOperazioneAgenda))
        Xml_VariabiliGiasOnline_2010.SetAttribute(LCase("TargetOperazione"), CStr(TargetOperazione))
        Xml_VariabiliGiasOnline_2010.SetAttribute(LCase("Programmazione_Cod"), CStr(Programmazione_Cod))
        Xml_VariabiliGiasOnline_2010.SetAttribute(LCase("querystringfiltrino"), CStr(QueryStringFiltrino))
        Xml_VariabiliGiasOnline_2010.SetAttribute(LCase("Data"), CStr(Data))
        Xml_VariabiliGiasOnline_2010.SetAttribute(LCase("Cod_Contatto"), CStr(Cod_Contatto))
        Xml_VariabiliGiasOnline_2010.SetAttribute(LCase("Impianti"), JsonConvert.SerializeObject(Impianti))
        Xml_VariabiliGiasOnline_2010.SetAttribute(LCase("Id_Agenda"), CStr(Id_Agenda))
        Xml_VariabiliGiasOnline_2010.SetAttribute(LCase("TipoRicetta"), CStr(TipoRicetta))
        Xml_VariabiliGiasOnline_2010.SetAttribute(LCase("Stato"), CStr(Stato))
        Xml_VariabiliGiasOnline_2010.SetAttribute(LCase("Ricetta_Operazione_Cod"), CStr(Ricetta_Operazione_Cod))
        Xml_VariabiliGiasOnline_2010.SetAttribute(LCase("Ricetta_Cod"), CStr(Ricetta_Cod))
        Xml_VariabiliGiasOnline_2010.SetAttribute(LCase("GenericObj_string"), CStr(GenericObj_string))
        Xml_VariabiliGiasOnline_2010.SetAttribute(LCase("RedirectUrl"), CStr(RedirectUrl))
        Xml_VariabiliGiasOnline_2010.SetAttribute(LCase("IdSezione"), CStr(IdSezione))
        Xml_VariabiliGiasOnline_2010.SetAttribute(LCase("sito_provenienza"), CStr(Sito_Provenienza))
        Xml_VariabiliGiasOnline_2010.SetAttribute(LCase("regolamento_cod"), CStr(Regolamento_Cod))
        Xml_VariabiliGiasOnline_2010.SetAttribute(LCase("tipo_regolamento"), CStr(Tipo_Regolamento))

        XmlDoc.AppendChild(Xml_VariabiliGiasOnline_2010)
        Dim str As String
        str = XmlDoc.InnerXml
        str = str.Replace("'", "&#39;")
        Return str
    End Function



End Class

'End Module