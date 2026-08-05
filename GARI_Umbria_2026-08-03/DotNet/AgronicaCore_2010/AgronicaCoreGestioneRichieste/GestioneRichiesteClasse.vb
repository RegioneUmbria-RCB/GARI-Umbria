Imports System.Web
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreVarieDAL.ComunicazioneGiasOnLine_Satelliti
Imports AgronicaCoreUtentiDAL
Imports AgronicaCoreDataProvider.TipiEnumerativi



Public Class GestioneRichiesteClasse

    Private TargetRedirectNonTantoCorretto As String

    Public UtenteAbilitato As Boolean

    Private Value_Enum_SiteRedirector As Enum_SiteRedirector
    Private PercorsoIconeVegetali As String

    'Public ParametriVari As ParametriAggiuntivi
    Private _VariabiliSessione As VariabiliSessione
    Private _AgroWebConfig As AgroWebConfig

    'dedicate
    Private _ParametriAgenda_2010 As ParametriAgenda_2010
    Private _ParametriVerificaDisciplinare2010 As ParametriVerificaDisciplinare2010
    Private _ParametriRicette_2010 As ParametriRicette_2010
    Private _ParametriGiasOnline As ParametriGiasOnline
    Private _ParametriAnalisi_2010 As ParametriAnalisi_2010
    Private _ParametriConcimazione_2017 As ParametriConcimazione_2017
    Private _ParametriPianidiCampionamento_2010 As ParametriPianidiCampionamento_2010
    Private _ParametriProfilazione_2010 As ParametriProfilazione_2010
    Private _ParametriSincronizzatore_2010 As ParametriSincronizzatore_2010
    Private _ParametriAgronicaMeteo As ParametriAgronicaMeteo
    Private _ParametriGiasOnline_2010 As ParametriGiasOnline_2010
    Private _ParametriAnalisiCosti_2010 As ParametriAnalisiCosti_2010
    Private _ParametriAgronicaStampe As ParametriAgronicaStampe
    Private _ParametriAgronicaStampe_2010 As ParametriAgronicaStampe_2010
    Private _ParametriScadenziario As ParametriScadenziario
    Private _ParametriPlanning As ParametriPlanning
    Private _ParametriLabCQ As ParametriLabCQ
    Private _ParametriSementieri As ParametriSementieri
    Private _ParametriAgronicaBio As ParametriAgronicaBio
    Private _ParametriAgronicaAuditPUA As ParametriAgronicaAuditPUA
    Private _ParametriFILTRONE_2010 As ParametriFILTRONE_2010
    Private _ParametriNonConformita As ParametriNonConformita
    Private _ParametriDomandaIrrigua As ParametriDomandaIrrigua


#Region "Property"



    Public Property ParametriFILTRONE_2010() As ParametriFILTRONE_2010
        Get
            Return _ParametriFILTRONE_2010
        End Get
        Set(ByVal value As ParametriFILTRONE_2010)
            _ParametriFILTRONE_2010 = value
        End Set
    End Property


    Public Property ParametriAgronicaBio() As ParametriAgronicaBio
        Get
            Return _ParametriAgronicaBio
        End Get
        Set(ByVal value As ParametriAgronicaBio)
            _ParametriAgronicaBio = value
        End Set
    End Property

    Public Property ParametriAgronicaAuditPUA() As ParametriAgronicaAuditPUA
        Get
            Return _ParametriAgronicaAuditPUA
        End Get
        Set(ByVal value As ParametriAgronicaAuditPUA)
            _ParametriAgronicaAuditPUA = value
        End Set
    End Property

    Public Property ParametriAgronicaStampe() As ParametriAgronicaStampe
        Get
            Return _ParametriAgronicaStampe
        End Get
        Set(ByVal value As ParametriAgronicaStampe)
            _ParametriAgronicaStampe = value
        End Set
    End Property

    Public Property ParametriSementieri() As ParametriSementieri
        Get
            Return _ParametriSementieri
        End Get
        Set(ByVal value As ParametriSementieri)
            _ParametriSementieri = value
        End Set
    End Property

    Public Property ParametriAgronicaStampe_2010() As ParametriAgronicaStampe_2010
        Get
            Return _ParametriAgronicaStampe_2010
        End Get
        Set(ByVal value As ParametriAgronicaStampe_2010)
            _ParametriAgronicaStampe_2010 = value
        End Set
    End Property

    Public Property ParametriAgronicaMeteo() As ParametriAgronicaMeteo
        Get
            Return _ParametriAgronicaMeteo
        End Get
        Set(ByVal value As ParametriAgronicaMeteo)
            _ParametriAgronicaMeteo = value
        End Set
    End Property

    Public Property ParametriVerificaDisciplinare2010() As ParametriVerificaDisciplinare2010
        Get
            Return _ParametriVerificaDisciplinare2010
        End Get
        Set(ByVal value As ParametriVerificaDisciplinare2010)
            _ParametriVerificaDisciplinare2010 = value
        End Set
    End Property

    Public Property ParametriRicette_2010() As ParametriRicette_2010
        Get
            Return _ParametriRicette_2010
        End Get
        Set(ByVal value As ParametriRicette_2010)
            _ParametriRicette_2010 = value
        End Set
    End Property

    Public Property ParametriSincronizzatore_2010() As ParametriSincronizzatore_2010
        Get
            Return _ParametriSincronizzatore_2010
        End Get
        Set(ByVal value As ParametriSincronizzatore_2010)
            _ParametriSincronizzatore_2010 = value
        End Set
    End Property

    Public Property ParametriProfilazione_2010() As ParametriProfilazione_2010
        Get
            Return _ParametriProfilazione_2010
        End Get
        Set(ByVal value As ParametriProfilazione_2010)
            _ParametriProfilazione_2010 = value
        End Set
    End Property

    Public Property AgroWebConfig() As AgroWebConfig
        Get
            Return _AgroWebConfig
        End Get
        Set(ByVal value As AgroWebConfig)
            _AgroWebConfig = value
        End Set
    End Property
    Public Property ParametriGiasOnline() As ParametriGiasOnline
        Get
            Return _ParametriGiasOnline
        End Get
        Set(ByVal value As ParametriGiasOnline)
            _ParametriGiasOnline = value
        End Set
    End Property
    Public Property ParametriGiasOnline_2010() As ParametriGiasOnline_2010
        Get
            Return _ParametriGiasOnline_2010
        End Get
        Set(ByVal value As ParametriGiasOnline_2010)
            _ParametriGiasOnline_2010 = value
        End Set
    End Property

    Public Property ParametriAgenda_2010() As ParametriAgenda_2010
        Get
            Return _ParametriAgenda_2010
        End Get
        Set(ByVal value As ParametriAgenda_2010)
            _ParametriAgenda_2010 = value
        End Set
    End Property

    Public Property ParametriAnalisi_2010() As ParametriAnalisi_2010
        Get
            Return _ParametriAnalisi_2010
        End Get
        Set(ByVal value As ParametriAnalisi_2010)
            _ParametriAnalisi_2010 = value
        End Set
    End Property

    Public Property ParametriConcimazione_2017() As ParametriConcimazione_2017
        Get
            Return _ParametriConcimazione_2017
        End Get
        Set(ByVal value As ParametriConcimazione_2017)
            _ParametriConcimazione_2017 = value
        End Set
    End Property

    Public Property ParametriPianidiCampionamento_2010() As ParametriPianidiCampionamento_2010
        Get
            Return _ParametriPianidiCampionamento_2010
        End Get
        Set(ByVal value As ParametriPianidiCampionamento_2010)
            _ParametriPianidiCampionamento_2010 = value
        End Set
    End Property
    Public Property ParametriAnalisiCosti_2010() As ParametriAnalisiCosti_2010
        Get
            Return _ParametriAnalisiCosti_2010
        End Get
        Set(ByVal value As ParametriAnalisiCosti_2010)
            _ParametriAnalisiCosti_2010 = value
        End Set
    End Property
    Public Property ParametriScadenziario() As ParametriScadenziario
        Get
            Return _ParametriScadenziario
        End Get
        Set(ByVal value As ParametriScadenziario)
            _ParametriScadenziario = value
        End Set
    End Property
    Public Property ParametriPlanning() As ParametriPlanning
        Get
            Return _ParametriPlanning
        End Get
        Set(ByVal value As ParametriPlanning)
            _ParametriPlanning = value
        End Set
    End Property
    Public Property ParametriLabCQ() As ParametriLabCQ
        Get
            Return _ParametriLabCQ
        End Get
        Set(ByVal value As ParametriLabCQ)
            _ParametriLabCQ = value
        End Set
    End Property
    Public Property ParametriNonConformita() As ParametriNonConformita
        Get
            Return _ParametriNonConformita
        End Get
        Set(ByVal value As ParametriNonConformita)
            _ParametriNonConformita = value
        End Set
    End Property

    Public Property ParametriDomandaIrrigua() As ParametriDomandaIrrigua
        Get
            Return _ParametriDomandaIrrigua
        End Get
        Set(ByVal value As ParametriDomandaIrrigua)
            _ParametriDomandaIrrigua = value
        End Set
    End Property
    Public Property VariabiliSessione() As VariabiliSessione
        Get
            Return (_VariabiliSessione)
        End Get
        Set(ByVal value As VariabiliSessione)
            _VariabiliSessione = value
        End Set
    End Property
#End Region

#Region "Costruttori"

    ''' <summary>
    ''' Inizializza la sessione con i vari oggetti tra cui anche objParametri server e utenti
    ''' nel caso in cui l utente non abbia i diritti di lettura viene impostata la variabile 
    ''' TargetRedirect = Messaggi/AccessoNegato.htm
    ''' </summary>
    ''' <param name="Unid_Request"></param>
    ''' <param name="Cn_Server_Request"></param>
    ''' <param name="PercorsoIconeVegetali_serverMapPath"> Server.MapPath("AB_Immagini/IconeVegetali").ToString</param>
    ''' <remarks></remarks>
    Sub New(ByVal Unid_Request As String, ByVal Id_DB_Server_Prima_Era_Cn_Server_Request As String, ByVal StringaConnessione_Super_Server As String, _
            ByVal PercorsoIconeVegetali_serverMapPath As String)

        Dim strParametri As String = ""
        Dim Stringa_JSon As String = ""

        'modifica per multiserver
        '        se Cn_Server_Request è intero allora sono con superserver ,
        ' quindi devo ricavare la connessione leggendo da gias superserver
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")

        If Not Unid_Request Is Nothing And Not Id_DB_Server_Prima_Era_Cn_Server_Request Is Nothing Then

            'Se sono nella modalità con superserver passo la stringa connessione supoerserver
            'e non il percorso pathfilei
            Dim objParametriGias As AgronicaCoreVarieDAL.ComunicazioneGiasOnLine_Satelliti
            If StringaConnessione_Super_Server <> "" Then
                objParametriGias = New AgronicaCoreVarieDAL.ComunicazioneGiasOnLine_Satelliti("", StringaConnessione_Super_Server, Id_DB_Server_Prima_Era_Cn_Server_Request)
            Else
                objParametriGias = New AgronicaCoreVarieDAL.ComunicazioneGiasOnLine_Satelliti(CostantiPersonalizzate.PathFileINI, Id_DB_Server_Prima_Era_Cn_Server_Request)
            End If

            strParametri = objParametriGias.LeggiParametriGias(Unid_Request, True, Stringa_JSon)
            'commento, visto che la cancellazione è già stata fatta sopra, passando true
            'objParametriGias.CancellaParametriGias(Unid_Request)
        Else
            TargetRedirectNonTantoCorretto = "Messaggi/AccessoNegato.htm"
        End If

        PercorsoIconeVegetali = PercorsoIconeVegetali_serverMapPath

        'Carico la stringa xml in un nuovo documento
        Dim XmlDoc = New System.Xml.XmlDocument

        If strParametri <> "" Then

            XmlDoc.LoadXml(strParametri)

            If XmlDoc.HasChildNodes Then

                Dim XML_Parametri As System.Xml.XmlElement
                'identifico il nodo di destinazione
                XML_Parametri = XmlDoc.SelectSingleNode("Parametri")
                If Not XML_Parametri Is Nothing Then

                    Dim Sito_Origine As String
                    Dim Sito_Destinazione As String
                    Sito_Origine = XML_Parametri.GetAttribute("sito_origine")
                    Sito_Destinazione = XML_Parametri.GetAttribute("sito_destinazione")

                    Dim inizializza As New AgronicaCoreGestioneRichieste.Inizializzatore
                        inizializza.Inizializza_Sito_Specifico_Da_Stringa_Passaggio(StringaConnessione_Super_Server,
                                                                                Id_DB_Server_Prima_Era_Cn_Server_Request,
                                                                                Sito_Origine,
                                                                                Sito_Destinazione,
                                                                                strParametri,
                                                                                HttpContext.Current.Session)

                        'carico il tipo di nodo idoneo
                        CaricaNodoAlbero(XmlDoc, Stringa_JSon)


                    End If



                    '------------------------------------------------------------------------

                    '############################################################################################
                    '######################       INIZIALIZZAZIONE VARIABILI ICONE ALBERO  ######################
                    '############################################################################################

                    '--------------------------------------------------------
                    '----- Carico l'elenco delle Icone Specie Vegetali ------
                    '--------------------------------------------------------

                    Dim Elenco_Icone_SpecieVegetali As String = ""
                Call Recupera_Elenco_Icone_SpecieVegetali(Elenco_Icone_SpecieVegetali)
                HttpContext.Current.Session("Elenco_Icone_SpecieVegetali") = Elenco_Icone_SpecieVegetali
                TargetRedirectNonTantoCorretto = ControllaPermessi()

                'QUESTO NON DEVE ESSERCI, le chiavi le legge tutte l'0inizializzatore, al massimo per debug
                '  Dim objWeb As New AgronicaCoreGestioneRichieste.AgroWebConfig(strParametri, HttpContext.Current.Session("ASG_objParametri_Server"))


            End If 'xml con figli


        End If 'strParametri <> "" 

        Dim progressivo As Integer
        HttpContext.Current.Session("TopCode") = 0
        HttpContext.Current.Session("BaseCode") = 0

        CodiciProgressivi.Trova_Base_e_Top(progressivo,
                                           HttpContext.Current.Session("BaseCode"), HttpContext.Current.Session("TopCode"),
                                           HttpContext.Current.Session("ASG_objParametri_Utenti"),
                                           HttpContext.Current.Session("ASG_objParametri_Server"))


    End Sub



    ''' <summary>
    ''' per evitare in fase di debug il login passo automaticamente la string XML
    ''' TargetRedirectNonTantoCorretto = Messaggi/AccessoNegato.htm
    ''' </summary>
    ''' <param name="strParametri"></param>
    ''' <param name="PercorsoIconeVegetali_serverMapPath"> Server.MapPath("AB_Immagini/IconeVegetali").ToString</param>
    ''' <remarks></remarks>
    ''' SOLO PER DEBUG MI RACCOMANDO!!
    Sub New(ByVal strParametri As String, _
             ByVal Id_DB_Server_Prima_Era_Cn_Server_Request As String, _
            ByVal StringaConnessione_Super_Server As String, _
            ByVal PercorsoIconeVegetali_serverMapPath As String, _
            ByVal solo_x_test_bea_ciccia As Boolean)

        PercorsoIconeVegetali = PercorsoIconeVegetali_serverMapPath



        'Carico la stringa xml in un nuovo documento
        Dim XmlDoc = New System.Xml.XmlDocument

        If strParametri <> "" Then
            XmlDoc.LoadXml(strParametri)
            If XmlDoc.HasChildNodes Then
                Dim XML_Parametri As System.Xml.XmlElement
                'identifico il nodo di destinazione
                XML_Parametri = XmlDoc.SelectSingleNode("Parametri")
                If Not XML_Parametri Is Nothing Then

                    Dim Sito_Origine As String
                    Dim Sito_Destinazione As String
                    Sito_Origine = XML_Parametri.GetAttribute("sito_origine")
                    Sito_Destinazione = XML_Parametri.GetAttribute("sito_destinazione")

                    HttpContext.Current.Session("Sito_Origine") = Sito_Origine

                    '------------------------------------------------------------------
                    'NODO ---> VariabiliSessione
                    '------------------------------------------------------------------
                    VariabiliSessione = New VariabiliSessione(strParametri)
                    VariabiliSessione.Salva()
                    Dim inizializza As New AgronicaCoreGestioneRichieste.Inizializzatore
                    Dim StringaConnessione_Server As String = HttpContext.Current.Session("ASG_StringaConnessione_Server")
                    Dim StringaConnessione_Utenti As String = HttpContext.Current.Session("ASG_StringaConnessione_Utenti")
                    inizializza.Crea_ObjParametri_Server_E_ObjParametri_Utenti_E_Salva_In_Sessione(HttpContext.Current.Session, StringaConnessione_Server, StringaConnessione_Utenti)

                    '------------------------------------------------------------------
                    'NODO ---> AgroWebConfig
                    '------------------------------------------------------------------
                    AgroWebConfig = New AgroWebConfig(strParametri, HttpContext.Current.Session("ASG_objParametri_Server"))


                    'carico il tipo di nodo idoneo
                    CaricaNodoAlbero(XmlDoc)
                End If



                '------------------------------------------------------------------------

                '############################################################################################
                '######################       INIZIALIZZAZIONE VARIABILI ICONE ALBERO  ######################
                '############################################################################################

                '--------------------------------------------------------
                '----- Carico l'elenco delle Icone Specie Vegetali ------
                '--------------------------------------------------------

                Dim Elenco_Icone_SpecieVegetali As String = ""
                Call Recupera_Elenco_Icone_SpecieVegetali(Elenco_Icone_SpecieVegetali)
                HttpContext.Current.Session("Elenco_Icone_SpecieVegetali") = Elenco_Icone_SpecieVegetali
                TargetRedirectNonTantoCorretto = ControllaPermessi()
            End If 'xml con figli

        End If



        Dim objParametriGias As AgronicaCoreVarieDAL.ComunicazioneGiasOnLine_Satelliti
        If StringaConnessione_Super_Server <> "" Then
            objParametriGias = New AgronicaCoreVarieDAL.ComunicazioneGiasOnLine_Satelliti("", StringaConnessione_Super_Server, Id_DB_Server_Prima_Era_Cn_Server_Request)
        Else
            objParametriGias = New AgronicaCoreVarieDAL.ComunicazioneGiasOnLine_Satelliti(CostantiPersonalizzate.PathFileINI, Id_DB_Server_Prima_Era_Cn_Server_Request)
        End If

        Dim progressivo As Integer
        HttpContext.Current.Session("TopCode") = 0
        HttpContext.Current.Session("BaseCode") = 0

        CodiciProgressivi.Trova_Base_e_Top(progressivo,
                                           HttpContext.Current.Session("BaseCode"), HttpContext.Current.Session("TopCode"),
                                           HttpContext.Current.Session("ASG_objParametri_Utenti"),
                                           HttpContext.Current.Session("ASG_objParametri_Server"))


    End Sub

#End Region

#Region "Funzioni"


    Private Sub CaricaNodoAlbero(ByVal XmlDoc As System.Xml.XmlDocument, Optional ByRef Stringa_JSon As String = "")


        Dim XML_ParametriAggiuntivi As System.Xml.XmlElement


        '##################################
        'STAMPE VERSIONE NUOVE, usate da apripoputsitogenerico, sempre con SUPERSERVER (HA TUTTI I PARAMETRI COME GLI ALTRI)
        XML_ParametriAggiuntivi = XmlDoc.SelectSingleNode("Parametri").SelectSingleNode("FiltroStampa")
        If Not XML_ParametriAggiuntivi Is Nothing Then
            'inizializzo l'oggetto
            ParametriAgronicaStampe = New ParametriAgronicaStampe(XmlDoc.OuterXml)
            Value_Enum_SiteRedirector = Enum_SiteRedirector.Sito_AgronicaStampe
            ParametriAgronicaStampe.Salva()
            Exit Sub
        End If


        '##################################
        'GIASONLINE
        XML_ParametriAggiuntivi = XmlDoc.SelectSingleNode("Parametri").SelectSingleNode("ParametriGiasOnline")
        If Not XML_ParametriAggiuntivi Is Nothing Then
            'inizializzo l'oggetto
            ParametriGiasOnline = New ParametriGiasOnline(XmlDoc.OuterXml)
            Value_Enum_SiteRedirector = Enum_SiteRedirector.Sito_GiasOnline
            ParametriGiasOnline.Salva()
            Exit Sub
        End If

        '##################################
        'SEMENTIERI
        XML_ParametriAggiuntivi = XmlDoc.SelectSingleNode("Parametri").SelectSingleNode("ParametriSementieri")
        If Not XML_ParametriAggiuntivi Is Nothing Then
            'inizializzo l'oggetto
            ParametriSementieri = New ParametriSementieri(XmlDoc.OuterXml)
            Value_Enum_SiteRedirector = Enum_SiteRedirector.Sito_AgronicaSementi
            ParametriSementieri.Salva()
            Exit Sub
        End If

        '##################################
        'RICETTE
        XML_ParametriAggiuntivi = XmlDoc.SelectSingleNode("Parametri").SelectSingleNode("ParametriRicette_2010")
        If Not XML_ParametriAggiuntivi Is Nothing Then
            'inizializzo l'oggetto
            ParametriRicette_2010 = New ParametriRicette_2010(XmlDoc.OuterXml)
            Value_Enum_SiteRedirector = Enum_SiteRedirector.Sito_AgronicaAgenda_2010
            ParametriRicette_2010.Salva()
            Exit Sub
        End If


        '##################################
        'VerificaDisciplinare2010
        XML_ParametriAggiuntivi = XmlDoc.SelectSingleNode("Parametri").SelectSingleNode("ParametriVerificaDisciplinare2010")
        If Not XML_ParametriAggiuntivi Is Nothing Then
            'inizializzo l'oggetto
            ParametriVerificaDisciplinare2010 = New ParametriVerificaDisciplinare2010(XmlDoc.OuterXml)
            Value_Enum_SiteRedirector = Enum_SiteRedirector.Sito_AgronicaAgenda_2010
            ParametriVerificaDisciplinare2010.Salva()
            Exit Sub
        End If

        '##################################
        'CONCIMAZIONE_2017
        XML_ParametriAggiuntivi = XmlDoc.SelectSingleNode("Parametri").SelectSingleNode("ParametriConcimazione_2017")
        If Not XML_ParametriAggiuntivi Is Nothing Then

            ParametriConcimazione_2017 = New ParametriConcimazione_2017(XmlDoc.OuterXml)
            Value_Enum_SiteRedirector = Enum_SiteRedirector.Sito_PianoConcimazione_2017
            ParametriConcimazione_2017.Salva()
            'COMMENTATO PERCHè voglio che legga anche le altre.
            'Se fosse per me li commenterei tutti tanto se poi non vado a leggere la sessione al prossimo sito li perdo.
            '  Galassi, 13/03/2017 15.06.08: 
            'Exit Sub
        End If

        '##################################
        'AGENDA
        XML_ParametriAggiuntivi = XmlDoc.SelectSingleNode("Parametri").SelectSingleNode("ParametriAgenda_2010")
        If Not XML_ParametriAggiuntivi Is Nothing Then
            'inizializzo l'oggetto
            ParametriAgenda_2010 = New ParametriAgenda_2010(XmlDoc.OuterXml)
            Value_Enum_SiteRedirector = Enum_SiteRedirector.Sito_AgronicaAgenda_2010
            ParametriAgenda_2010.Salva()

            If Not String.IsNullOrEmpty(Stringa_JSon) Then
                HttpContext.Current.Session("ParametriAgenda_2010_Stringa_JSon") = Stringa_JSon
            End If

            Exit Sub
        End If


        '##################################
        'GIASONLINE 2010
        XML_ParametriAggiuntivi = XmlDoc.SelectSingleNode("Parametri").SelectSingleNode("ParametriGiasOnline_2010")
        If Not XML_ParametriAggiuntivi Is Nothing Then
            'inizializzo l'oggetto
            ParametriGiasOnline_2010 = New ParametriGiasOnline_2010(XmlDoc.OuterXml)
            Value_Enum_SiteRedirector = Enum_SiteRedirector.Sito_GiasOnline_2010
            ParametriGiasOnline_2010.Salva()
            Exit Sub
        End If

        '##################################
        'PIANI DI CAMPIONAMENTO
        XML_ParametriAggiuntivi = XmlDoc.SelectSingleNode("Parametri").SelectSingleNode("ParametriPianidiCampionamento_2010")
        If Not XML_ParametriAggiuntivi Is Nothing Then
            'Dim XML_ParametriAggiuntivi2 As System.Xml.XmlElement = XmlDoc.SelectSingleNode("Parametri").SelectSingleNode("ParametriPianidiCampionamento_2010")
            ParametriPianidiCampionamento_2010 = New ParametriPianidiCampionamento_2010(XmlDoc.OuterXml)
            Value_Enum_SiteRedirector = Enum_SiteRedirector.Sito_AgronicaPianiCampionamento
            ParametriPianidiCampionamento_2010.Salva()
            Exit Sub
        End If


        '##################################
        'ANALISI
        XML_ParametriAggiuntivi = XmlDoc.SelectSingleNode("Parametri").SelectSingleNode("ParametriAnalisi_2010")
        If Not XML_ParametriAggiuntivi Is Nothing Then

            ParametriAnalisi_2010 = New ParametriAnalisi_2010(XmlDoc.OuterXml)
            Value_Enum_SiteRedirector = Enum_SiteRedirector.Sito_AgronicaAnalisi
            ParametriAnalisi_2010.Salva()
            Exit Sub
        End If

        '##################################
        'PROOFILAZIONE
        XML_ParametriAggiuntivi = XmlDoc.SelectSingleNode("Parametri").SelectSingleNode("ParametriProfilazione_2010")
        If Not XML_ParametriAggiuntivi Is Nothing Then
            ParametriProfilazione_2010 = New ParametriProfilazione_2010(XmlDoc.OuterXml)
            Value_Enum_SiteRedirector = Enum_SiteRedirector.Sito_AgronicaProfilazione
            ParametriProfilazione_2010.Salva()
            Exit Sub
        End If


        '##################################
        'SINCRONIZZATORE
        XML_ParametriAggiuntivi = XmlDoc.SelectSingleNode("Parametri").SelectSingleNode("ParametriSincronizzatore_2010")
        If Not XML_ParametriAggiuntivi Is Nothing Then
            ParametriSincronizzatore_2010 = New ParametriSincronizzatore_2010(XmlDoc.OuterXml)
            Value_Enum_SiteRedirector = Enum_SiteRedirector.Sito_AgronicaSincronizzatore
            ParametriSincronizzatore_2010.Salva()
            Exit Sub
        End If


        '##################################
        'AGRONCIAMETEO
        XML_ParametriAggiuntivi = XmlDoc.SelectSingleNode("Parametri").SelectSingleNode("ParametriAgronicaMeteo")
        If Not XML_ParametriAggiuntivi Is Nothing Then
            ParametriAgronicaMeteo = New ParametriAgronicaMeteo(XmlDoc.OuterXml)
            Value_Enum_SiteRedirector = Enum_SiteRedirector.Sito_AgronicaMeteo
            ParametriAgronicaMeteo.Salva()
            Exit Sub
        End If


        '##################################
        'Analisi costi 
        'ATTENZIONE CHE anche LinkPianoConcimazione usa ParametriAnalisiCosti_2010
        'qiondi se controllo i permessi con il link non è corretto
        XML_ParametriAggiuntivi = XmlDoc.SelectSingleNode("Parametri").SelectSingleNode("ParametriAnalisiCosti_2010")
        If Not XML_ParametriAggiuntivi Is Nothing Then
            'inizializzo l'oggetto
            ParametriAnalisiCosti_2010 = New ParametriAnalisiCosti_2010(XmlDoc.OuterXml)
            'non è corretto perchè potrebbe essere anche il LinkPianoConcimazione
            Value_Enum_SiteRedirector = Enum_SiteRedirector.Sito_AgronicaAgenda_2010
            ParametriAnalisiCosti_2010.Salva()
            Exit Sub
        End If

        '##################################
        'Scadenziario
        XML_ParametriAggiuntivi = XmlDoc.SelectSingleNode("Parametri").SelectSingleNode("ParametriScadenziario")
        If Not XML_ParametriAggiuntivi Is Nothing Then
            'inizializzo l'oggetto
            ParametriScadenziario = New ParametriScadenziario(XmlDoc.OuterXml)
            Value_Enum_SiteRedirector = Enum_SiteRedirector.Sito_GiasOnline_2010
            ParametriScadenziario.Salva()
            Exit Sub
        End If

        '##################################
        'Planning
        XML_ParametriAggiuntivi = XmlDoc.SelectSingleNode("Parametri").SelectSingleNode("ParametriPlanning")
        If Not XML_ParametriAggiuntivi Is Nothing Then
            'inizializzo l'oggetto
            ParametriPlanning = New ParametriPlanning(XmlDoc.OuterXml)
            Value_Enum_SiteRedirector = Enum_SiteRedirector.Sito_AgronicaPlanning
            ParametriPlanning.Salva()
            Exit Sub
        End If

        '##################################
        'LabCQ
        XML_ParametriAggiuntivi = XmlDoc.SelectSingleNode("Parametri").SelectSingleNode("ParametriLabCQ")
        If Not XML_ParametriAggiuntivi Is Nothing Then
            'inizializzo l'oggetto
            ParametriLabCQ = New ParametriLabCQ(XmlDoc.OuterXml)
            Value_Enum_SiteRedirector = Enum_SiteRedirector.Sito_AgronicaLabQualita
            ParametriLabCQ.Salva()
            Exit Sub
        End If

        '##################################
        'Non Conformita
        XML_ParametriAggiuntivi = XmlDoc.SelectSingleNode("Parametri").SelectSingleNode("ParametriNonConformita")
        If Not XML_ParametriAggiuntivi Is Nothing Then
            'inizializzo l'oggetto
            ParametriNonConformita = New ParametriNonConformita(XmlDoc.OuterXml)
            Value_Enum_SiteRedirector = Enum_SiteRedirector.Sito_AgronicaAgenda_2010
            ParametriNonConformita.Salva()
            Exit Sub
        End If

        '##################################
        'Agronica Stampe 2010
        XML_ParametriAggiuntivi = XmlDoc.SelectSingleNode("Parametri").SelectSingleNode("ParametriAgronicaStampe_2010")
        If Not XML_ParametriAggiuntivi Is Nothing Then
            'inizializzo l'oggetto
            ParametriAgronicaStampe_2010 = New ParametriAgronicaStampe_2010(XmlDoc.OuterXml)
            Value_Enum_SiteRedirector = Enum_SiteRedirector.Sito_AgronicaStampe_2010
            ParametriAgronicaStampe_2010.Salva()

            If Not String.IsNullOrEmpty(Stringa_JSon) Then
                HttpContext.Current.Session("ParametriAgronicaStampe_2010_Stringa_JSon") = Stringa_JSon
            End If

            Exit Sub
        End If


        '##################################
        'AgronicaBio
        XML_ParametriAggiuntivi = XmlDoc.SelectSingleNode("Parametri").SelectSingleNode("VariabiliBio")
        If Not XML_ParametriAggiuntivi Is Nothing Then
            'inizializzo l'oggetto
            ParametriAgronicaBio = New ParametriAgronicaBio(XmlDoc.OuterXml)
            Value_Enum_SiteRedirector = Enum_SiteRedirector.Sito_AgronicaBio
            ParametriAgronicaBio.Salva()
            Exit Sub
        End If

        '##################################
        'AgronicaPUA
        'ATTENZIONE CHE anche gli altri link audit usano VariabiliAudit
        'qiondi se controllo i permessi con il link non è corretto
        XML_ParametriAggiuntivi = XmlDoc.SelectSingleNode("Parametri").SelectSingleNode("VariabiliAudit")
        If Not XML_ParametriAggiuntivi Is Nothing Then
            'inizializzo l'oggetto
            ParametriAgronicaAuditPUA = New ParametriAgronicaAuditPUA(XmlDoc.OuterXml)
            'non è corretto perchè potrebbe essere anche il LinkGlobal etc
            'Value_Enum_SiteRedirector = Enum_SiteRedirector.Sito_AgronicaPUA
            ParametriAgronicaAuditPUA.Salva()
            Exit Sub
        End If

        '##################################
        'ParametriFILTRONE_2010
        XML_ParametriAggiuntivi = XmlDoc.SelectSingleNode("Parametri").SelectSingleNode("ParametriFILTRONE_2010")
        If Not XML_ParametriAggiuntivi Is Nothing Then
            'inizializzo l'oggetto
            ParametriFILTRONE_2010 = New ParametriFILTRONE_2010(XmlDoc.OuterXml)
            'non è corretto perchè potrebbe essere anche il LinkGlobal etc
            'Value_Enum_SiteRedirector = Enum_SiteRedirector.Sito_AgronicaPUA
            ParametriFILTRONE_2010.Salva()
            Exit Sub
        End If

        '##################################
        'ParametriDomandaIrrigua
        XML_ParametriAggiuntivi = XmlDoc.SelectSingleNode("Parametri").SelectSingleNode("ParametriDomandaIrrigua")
        If Not XML_ParametriAggiuntivi Is Nothing Then
            'inizializzo l'oggetto
            ParametriDomandaIrrigua = New ParametriDomandaIrrigua(XmlDoc.OuterXml)
            'non è corretto perchè potrebbe essere anche il LinkGlobal etc
            'Value_Enum_SiteRedirector = Enum_SiteRedirector.Sito_AgronicaPUA
            ParametriDomandaIrrigua.Salva()
            Exit Sub
        End If


        '##################################
        'AgronicaAudit come AgronicaPUA

        '##################################
        'AgronicaCheckCOOP come AgronicaPUA

        '##################################
        'AgronicaGlobalGAP come AgronicaPUA

        '##################################
        'AgronicaSicurezzaLavoro come AgronicaPUA

        '##################################
        'AgronicaManutenzione

        '##################################
        'AgronicaPianiSemina
        'NON HA PARAMETRI SPECIFICI PER ORA

        '##################################
        'Sito_AgronicaAnalisi
        'NON GESTITO

        '##################################
        'Sito_AgronicaView

    End Sub



    ''' <summary>
    ''' controllo se l'utente ha il permesso di visualizzare la pagina richiesta
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ''' 
    Private Function ControllaPermessi() As String

        '----- Verifico se l'utente dispone dei permessi per la visualizzazione della pagina



        Dim objUtentiPermessi As New Utenti_Permessi_R

        Select Case Value_Enum_SiteRedirector
            'ANALISI
            Case Enum_SiteRedirector.Sito_AgronicaAnalisi
                'ABILITAZIONE ALLE ANALISI
                If objUtentiPermessi.Controlla_Permessi_Utente( _
                                      HttpContext.Current.Session("ASG_Utente_Username"), _
                                     HttpContext.Current.Session("ASG_IdServizio"), _
                                     TipiEnumerativi.enum_Security_Attivita.Gest_Analisi_AccessoMenu, _
                                     TipiEnumerativi.enum_Security_Operazione.Lettura, _
                                     Date.Now, _
                                     "", _
                                     HttpContext.Current.Session("ASG_objParametri_Utenti")) = False Then
                    TargetRedirectNonTantoCorretto = "Messaggi/AccessoNegato.htm"
                Else
                    UtenteAbilitato = True
                End If

            Case Enum_SiteRedirector.Sito_AgronicaSementi
                If objUtentiPermessi.Controlla_Permessi_Utente( _
                                      HttpContext.Current.Session("ASG_Utente_Username"), _
                                     HttpContext.Current.Session("ASG_IdServizio"), _
                                     TipiEnumerativi.enum_Security_Attivita.Link_AgronicaSementi, _
                                     TipiEnumerativi.enum_Security_Operazione.Lettura, _
                                     Date.Now, _
                                     "", _
                                     HttpContext.Current.Session("ASG_objParametri_Utenti")) = False Then
                    TargetRedirectNonTantoCorretto = "Messaggi/AccessoNegato.htm"
                Else
                    UtenteAbilitato = True
                End If




                'PROFILAZIONE
            Case Enum_SiteRedirector.Sito_AgronicaProfilazione
                'ABILITAZIONE ALLA PROFILAZIONE DELLA AGENDA
                If objUtentiPermessi.Controlla_Permessi_Utente( _
                                      HttpContext.Current.Session("ASG_Utente_Username"), _
                                     HttpContext.Current.Session("ASG_IdServizio"), _
                                     TipiEnumerativi.enum_Security_Attivita.ProfilazioneImpresa, _
                                     TipiEnumerativi.enum_Security_Operazione.Lettura, _
                                     Date.Now, _
                                     "", _
                                     HttpContext.Current.Session("ASG_objParametri_Utenti")) = False Then
                    TargetRedirectNonTantoCorretto = "Messaggi/AccessoNegato.htm"
                Else
                    UtenteAbilitato = True
                End If
                'AGENDA
            Case Enum_SiteRedirector.Sito_AgronicaAgenda_2010
                'ABILITAZIONE AGENDA
                If objUtentiPermessi.Controlla_Permessi_Utente( _
                                     HttpContext.Current.Session("ASG_Utente_Username"), _
                                     HttpContext.Current.Session("ASG_IdServizio"), _
                                     TipiEnumerativi.enum_Security_Attivita.Agenda_AccessoMenu, _
                                     TipiEnumerativi.enum_Security_Operazione.Lettura, _
                                     Date.Now, _
                                     "", _
                                     HttpContext.Current.Session("ASG_objParametri_Utenti")) = False Then
                    TargetRedirectNonTantoCorretto = "Messaggi/AccessoNegato.htm"
                Else
                    UtenteAbilitato = True
                End If

                'ATTENZIONE CHE anche LinkPianoConcimazione usa ParametriAnalisiCosti_2010
                'qiondi se controllo i permessi con il link non è corretto



                'Piani di Campionamento
            Case Enum_SiteRedirector.Sito_AgronicaPianiCampionamento
                'ABILITAZIONE PAINI DI CAMPIONAMENTO
                If objUtentiPermessi.Controlla_Permessi_Utente( _
                                     HttpContext.Current.Session("ASG_Utente_Username"), _
                                     HttpContext.Current.Session("ASG_IdServizio"), _
                                     TipiEnumerativi.enum_Security_Attivita.PianiCampionamento_GestioneLotti, _
                                     TipiEnumerativi.enum_Security_Operazione.Lettura, _
                                     Date.Now, _
                                     "", _
                                     HttpContext.Current.Session("ASG_objParametri_Utenti")) = False Then
                    TargetRedirectNonTantoCorretto = "Messaggi/AccessoNegato.htm"
                Else
                    UtenteAbilitato = True
                End If


                'Sincronizzatore
            Case Enum_SiteRedirector.Sito_AgronicaSincronizzatore
                Dim permesso As Integer
                Select Case ParametriSincronizzatore_2010.Pagina_Richiesta
                    Case enum_PagineAgronicaSincro.Sincro_Harvard
                        permesso = TipiEnumerativi.enum_Security_Attivita.ManutenzioneArchivi_SincronizzazioneHarvard
                    Case enum_PagineAgronicaSincro.Sincro_Agrea_OP
                        permesso = TipiEnumerativi.enum_Security_Attivita.ManutenzioneArchivi_Importa_AGREA_OP
                    Case enum_PagineAgronicaSincro.ImportazioneDaAnagrafeEmiliaRomagna
                        permesso = TipiEnumerativi.enum_Security_Attivita.ManutenzioneArchivi_Importa_Anagrafe_EmiliaRomagna
                    Case enum_PagineAgronicaSincro.ImportazioneDaAgrea
                        permesso = TipiEnumerativi.enum_Security_Attivita.ManutenzioneArchivi_Importa_AGREA
                End Select
                If objUtentiPermessi.Controlla_Permessi_Utente( _
                                     HttpContext.Current.Session("ASG_Utente_Username"), _
                                     HttpContext.Current.Session("ASG_IdServizio"), _
                                     permesso, _
                                     TipiEnumerativi.enum_Security_Operazione.Modifica, _
                                     Date.Now, _
                                     "", _
                                     HttpContext.Current.Session("ASG_objParametri_Utenti")) = False Then
                    TargetRedirectNonTantoCorretto = "Messaggi/AccessoNegato.htm"
                Else
                    UtenteAbilitato = True
                End If

            Case Enum_SiteRedirector.Sito_AgronicaPUA, Enum_SiteRedirector.Sito_AgronicaAudit, _
             Enum_SiteRedirector.Sito_AgronicaCheckCOOP, Enum_SiteRedirector.Sito_AgronicaGlobalGAP, _
              Enum_SiteRedirector.Sito_AgronicaSicurezzaLavoro
                'AgronicaPUA
                'ATTENZIONE CHE anche gli altri link audit usano VariabiliAudit
                'qiondi se controllo i permessi con il link non è corretto
                UtenteAbilitato = True
                TargetRedirectNonTantoCorretto = ""

        End Select


        Return TargetRedirectNonTantoCorretto

    End Function



    ''' <summary>
    ''' Recupera le icone per le specie vegetali
    ''' </summary>
    ''' <param name="Elenco_Icone_SpecieVegetali"></param>
    ''' <remarks></remarks>
    Private Sub Recupera_Elenco_Icone_SpecieVegetali(ByRef Elenco_Icone_SpecieVegetali As String)

        Dim NomeFile As String
        Dim Str_Veg_Cod As String

        'Inizializzo
        Elenco_Icone_SpecieVegetali = ""


        'Recupero 
        NomeFile = FileSystem.Dir(PercorsoIconeVegetali & "\*.ico")

        Do While NomeFile <> ""

            'Elimino l'estensione
            Str_Veg_Cod = Microsoft.VisualBasic.Strings.Left(NomeFile, 7)

            'Tolgo gli zeri di troppo
            Str_Veg_Cod = CInt(Str_Veg_Cod).ToString

            'Aggiungo il nuovo Veg_Cod all'elenco
            Elenco_Icone_SpecieVegetali += ":" & Str_Veg_Cod

            'Avanti un altro ...
            NomeFile = FileSystem.Dir()

        Loop

        'Tolgo il separatore iniziale
        Elenco_Icone_SpecieVegetali = Mid(Elenco_Icone_SpecieVegetali, 2)

    End Sub

#End Region

End Class








