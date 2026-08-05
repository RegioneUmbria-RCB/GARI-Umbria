
Imports System.Web
Imports System
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi

Friend Class ScriviXml


#Region "Variabili"
    'AgroWebConfig
    Private _AgroWebConfig As AgroWebConfig
    Private _VariabiliSessione As VariabiliSessione
    'specifici
    Private _ParametriAnalisi_2010 As ParametriAnalisi_2010
    Private _ParametriAgenda_2010 As ParametriAgenda_2010
    Private _ParametriRicette_2010 As ParametriRicette_2010
    Private _ParametriVerificaDisciplinare2010 As ParametriVerificaDisciplinare2010
    Private _ParametriPianidiCampionamento_2010 As ParametriPianidiCampionamento_2010
    Private _ParametriProfilazione_2010 As ParametriProfilazione_2010

    Private _ParametriGiasOnline As ParametriGiasOnline
    Private _ParametriSincronizzazione_2010 As ParametriSincronizzatore_2010
    Private _ParametriAgronicaStampe As ParametriAgronicaStampe
    Private _ParametriAgronicaStampe_2010 As ParametriAgronicaStampe_2010
    Private _ParametriGiasOnline_2010 As ParametriGiasOnline_2010
    Private _ParametriPlanning As ParametriPlanning
    Private _ParametriLabCQ As ParametriLabCQ
    Private _ParametriAnalisiCosti_2010 As ParametriAnalisiCosti_2010
    Private _ParametriConcimazione_2017 As ParametriConcimazione_2017
    Private _ParametriScadenziario As ParametriScadenziario
    Private _ParametriSementieri As ParametriSementieri
    Private _ParametriAgronicaBio As ParametriAgronicaBio
    Private _ParametriAgronicaAuditPUA As ParametriAgronicaAuditPUA

    Private _ParametriDomandaIrrigua As ParametriDomandaIrrigua

    Private _ParametriFILTRONE_2010 As ParametriFILTRONE_2010

    Private _ParametriNonConformita As ParametriNonConformita
    Private _Parametri_ObjParametriAgenda_NG As Parametri_ObjParametriAgenda_NG

    Public Property ParametriFILTRONE_2010() As ParametriFILTRONE_2010
        Get
            Return _ParametriFILTRONE_2010
        End Get
        Set(ByVal value As ParametriFILTRONE_2010)
            _ParametriFILTRONE_2010 = value
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

    Public Property ParametriVerificaDisciplinare2010() As ParametriVerificaDisciplinare2010
        Get
            Return _ParametriVerificaDisciplinare2010
        End Get
        Set(ByVal value As ParametriVerificaDisciplinare2010)
            _ParametriVerificaDisciplinare2010 = value
        End Set
    End Property

    Public Property ParametriSincronizzatore_2010() As ParametriSincronizzatore_2010
        Get
            Return _ParametriSincronizzazione_2010
        End Get
        Set(ByVal value As ParametriSincronizzatore_2010)
            _ParametriSincronizzazione_2010 = value
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

    Public Property ParametriAgronicaStampe() As ParametriAgronicaStampe
        Get
            Return _ParametriAgronicaStampe
        End Get
        Set(ByVal value As ParametriAgronicaStampe)
            _ParametriAgronicaStampe = value
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

    Public Property AgroWebConfig() As AgroWebConfig
        Get
            Return _AgroWebConfig
        End Get
        Set(ByVal value As AgroWebConfig)
            _AgroWebConfig = value
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

    Public Property ParametriConcimazione_2017() As ParametriConcimazione_2017
        Get
            Return _ParametriConcimazione_2017
        End Get
        Set(ByVal value As ParametriConcimazione_2017)
            _ParametriConcimazione_2017 = value
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

    Public Property Parametri_ObjParametriAgenda_NG() As Parametri_ObjParametriAgenda_NG
        Get
            Return _Parametri_ObjParametriAgenda_NG
        End Get
        Set(ByVal value As Parametri_ObjParametriAgenda_NG)
            _Parametri_ObjParametriAgenda_NG = value
        End Set
    End Property


    Public Property VariabiliSessione() As VariabiliSessione
        Get
            Return _VariabiliSessione
        End Get
        Set(ByVal value As VariabiliSessione)
            _VariabiliSessione = value
        End Set
    End Property

#End Region


    ''' <summary>
    ''' Da Utilizzare solamente nell'ONLINE dato che prende come parametro ConfigurationSettings.AppSettings
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New(ByVal WebConfig As System.Collections.Specialized.NameValueCollection)
        VariabiliSessione = New VariabiliSessione
        AgroWebConfig = New AgroWebConfig(WebConfig)
    End Sub


    ''' <summary>
    ''' non sono dentro al Giasonline e quindi ho l'oggetto in sessione
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
        VariabiliSessione = New VariabiliSessione
        AgroWebConfig = New AgroWebConfig
    End Sub

    Public Sub New(ByVal flagSessione As Boolean,
                   ByVal objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                   ByVal objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                   ByVal objParametri_SuperServer As AgronicaCoreDataProvider.AgronicaCoreParametri)

        VariabiliSessione = New VariabiliSessione(flagSessione,
                                                  objParametri_Server,
                                                  objParametri_Utenti,
                                                  objParametri_SuperServer)
        AgroWebConfig = New AgroWebConfig(flagSessione,
                                          objParametri_Server,
                                          objParametri_SuperServer)

    End Sub





    ''' <summary>
    ''' Funzione che genera l XML e lo scrive su DB. Ritorna Unid da girare alla pagina 
    ''' </summary>
    ''' <param name="SitoOrigine"></param>
    ''' <param name="SitoDestinazione"></param>
    ''' <returns>Id da girare alla pagina</returns>
    ''' <remarks></remarks>
    Public Function ScriviXml(ByVal SitoOrigine As Int32, ByVal SitoDestinazione As Int32, Optional ByVal JSON_Generico As String = "") As String
        Dim xml As String
        xml = GeneraXML(SitoOrigine, SitoDestinazione)
        Dim objVarie As New AgronicaCoreVarieDAL.ComunicazioneGiasOnLine_Satelliti
        Dim UID As String
        UID = objVarie.ScriviParametriGias(xml, HttpContext.Current.Session("ASG_objParametri_Server"), JSON_Generico)
        Return UID
    End Function

    ''' <summary>
    ''' ATTENZIONE: Altra versione di ScriviXml, esegue le stesse operazioni, ma non utilizza la Session;
    ''' Funzione che genera l XML e lo scrive su DB. Ritorna Unid da girare alla pagina 
    ''' </summary>
    ''' <param name="SitoOrigine"></param>
    ''' <param name="SitoDestinazione"></param>
    ''' <param name="objParametri_Server"></param>
    ''' <param name="JSON_Generico"></param>
    ''' <returns></returns>
    Public Function ScriviXml_NoSession(ByVal SitoOrigine As Int32,
                                        ByVal SitoDestinazione As Int32,
                                        ByVal objParametri_Server As AgronicaCoreParametri,
                                        Optional ByVal JSON_Generico As String = "") As String
        Dim xml As String
        xml = GeneraXML(SitoOrigine,
                        SitoDestinazione)

        Dim objVarie As New AgronicaCoreVarieDAL.ComunicazioneGiasOnLine_Satelliti
        Dim UID As String
        UID = objVarie.ScriviParametriGias(xml,
                                           objParametri_Server,
                                           JSON_Generico)

        Return UID

    End Function


    ''' <summary>
    ''' ritorna l'XML da scrivere sul DB
    ''' </summary>
    ''' <param name="SitoOrigine"></param>
    ''' <param name="SitoDestinazione"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GeneraXML(ByVal SitoOrigine As Int32, ByVal SitoDestinazione As Int32) As String
        Dim XmlDoc As New System.Xml.XmlDocument
        Dim XML_Parametri As System.Xml.XmlElement
        XML_Parametri = XmlDoc.CreateElement("Parametri")

        XML_Parametri.SetAttribute("sito_origine", SitoOrigine)
        XML_Parametri.SetAttribute("sito_destinazione", SitoDestinazione)


        'WebConfig
        XML_Parametri.InnerXml = XML_Parametri.InnerXml & AgroWebConfig.GeneraXML()
        'Variabili sessione
        XML_Parametri.InnerXml = XML_Parametri.InnerXml & VariabiliSessione.GeneraXML()


        'procedo con la generazione dei nodi specifici del sito
        'genero solamente se l'oggetto non è nothing infatti in questo caso l'avrò inizioalizzato dalla pagina

        'STAMPE (SOLO PER versione nuova che usa il formato che usano tutti, da usare poi per  SUPERSERVER)
        If Not IsNothing(ParametriAgronicaStampe) Then
            XML_Parametri.InnerXml = XML_Parametri.InnerXml & ParametriAgronicaStampe.GeneraXML()
        End If
        'STAMPE 2010
        If Not IsNothing(ParametriAgronicaStampe_2010) Then
            XML_Parametri.InnerXml = XML_Parametri.InnerXml & ParametriAgronicaStampe_2010.GeneraXML()
        End If


        'ParametriFILTRONE_2010
        If Not IsNothing(ParametriFILTRONE_2010) Then
            XML_Parametri.InnerXml = XML_Parametri.InnerXml & ParametriFILTRONE_2010.GeneraXML()
        End If



        'SEMENTIERI
        If Not IsNothing(ParametriSementieri) Then
            XML_Parametri.InnerXml = XML_Parametri.InnerXml & ParametriSementieri.GeneraXML()
        End If



        'AGENDA
        If Not IsNothing(ParametriAgenda_2010) Then
            XML_Parametri.InnerXml = XML_Parametri.InnerXml & ParametriAgenda_2010.GeneraXML()
        End If

        'ParametriRicette_2010
        If Not IsNothing(ParametriRicette_2010) Then
            XML_Parametri.InnerXml = XML_Parametri.InnerXml & ParametriRicette_2010.GeneraXML()
        End If

        'verifica disciplinare agenda 2010
        If Not IsNothing(ParametriVerificaDisciplinare2010) Then
            XML_Parametri.InnerXml = XML_Parametri.InnerXml & ParametriVerificaDisciplinare2010.GeneraXML()
        End If

        'ANALISI
        If Not IsNothing(ParametriAnalisi_2010) Then
            XML_Parametri.InnerXml = XML_Parametri.InnerXml & ParametriAnalisi_2010.GeneraXML()
        End If

        'PIANI DI CAMPIONAMENTO
        If Not IsNothing(ParametriPianidiCampionamento_2010) Then
            XML_Parametri.InnerXml = XML_Parametri.InnerXml & ParametriPianidiCampionamento_2010.GeneraXML()
        End If

        'PRPFILAZIONE
        If Not IsNothing(ParametriProfilazione_2010) Then
            XML_Parametri.InnerXml = XML_Parametri.InnerXml & ParametriProfilazione_2010.GeneraXML()
        End If

        'GiasOnline
        If Not IsNothing(ParametriGiasOnline) Then
            XML_Parametri.InnerXml = XML_Parametri.InnerXml & ParametriGiasOnline.GeneraXML()
        End If

        'GiasOnline 2010
        If Not IsNothing(ParametriGiasOnline_2010) Then
            XML_Parametri.InnerXml = XML_Parametri.InnerXml & ParametriGiasOnline_2010.GeneraXML()
        End If

        'ParametriSincronizzatore_2010
        If Not IsNothing(ParametriSincronizzatore_2010) Then
            XML_Parametri.InnerXml = XML_Parametri.InnerXml & ParametriSincronizzatore_2010.GeneraXML()
        End If

        'ParametriPlanning
        If Not IsNothing(ParametriPlanning) Then
            XML_Parametri.InnerXml = XML_Parametri.InnerXml & ParametriPlanning.GeneraXML()
        End If

        'ParametriLabCQ
        If Not IsNothing(ParametriLabCQ) Then
            XML_Parametri.InnerXml = XML_Parametri.InnerXml & ParametriLabCQ.GeneraXML()
        End If

        'Parametri AnalisiCosti
        If Not IsNothing(ParametriAnalisiCosti_2010) Then
            XML_Parametri.InnerXml = XML_Parametri.InnerXml & ParametriAnalisiCosti_2010.GeneraXML()
        End If

        'Parametri Concimazione2017
        If Not IsNothing(ParametriConcimazione_2017) Then
            XML_Parametri.InnerXml = XML_Parametri.InnerXml & ParametriConcimazione_2017.GeneraXML()
        End If

        'ParametriScadenziario
        If Not IsNothing(ParametriScadenziario) Then
            XML_Parametri.InnerXml = XML_Parametri.InnerXml & ParametriScadenziario.GeneraXML()
        End If


        'ParametriAgronicaBio
        If Not IsNothing(ParametriAgronicaBio) Then
            XML_Parametri.InnerXml = XML_Parametri.InnerXml & ParametriAgronicaBio.GeneraXML()
        End If


        'ParametriAgronicaAuditPUA
        If Not IsNothing(ParametriAgronicaAuditPUA) Then
            XML_Parametri.InnerXml = XML_Parametri.InnerXml & ParametriAgronicaAuditPUA.GeneraXML()
        End If

        'ParametriNonConformita
        If Not IsNothing(ParametriNonConformita) Then
            XML_Parametri.InnerXml = XML_Parametri.InnerXml & ParametriNonConformita.GeneraXML()
        End If

        'ParametriDomandaIrrigua
        If Not IsNothing(ParametriDomandaIrrigua) Then
            XML_Parametri.InnerXml = XML_Parametri.InnerXml & ParametriDomandaIrrigua.GeneraXML()
        End If

        If Not IsNothing(Parametri_ObjParametriAgenda_NG) Then
            XML_Parametri.InnerXml = XML_Parametri.InnerXml & Parametri_ObjParametriAgenda_NG.GeneraXML()
        End If

        Dim str As String
        'str = XML_Parametri.OuterXml.Replace("'", Chr(34))
        str = XML_Parametri.OuterXml
        Return str

    End Function





#Region "Vecchie Chiamate Pagine - spostate in Redirect GEstione"





    'per le redirezioni usare 
    'AgronicaCoreGestioneRichieste.RedirectGestione.Apri



    Private Function DammiIndirizzo(ByVal hrefSito As String, ByVal SitoOrigine As Int32, ByVal SitoDestinazione As Int32)
        Dim UnID As String = ScriviXml(SitoOrigine, SitoDestinazione)
        Dim cn As String = HttpContext.Current.Session("ASG_Connessione_Server")

        Dim objCodifica As New AgronicaCoreDataProvider.Sicurezza

        UnID = Stringa_Codifica(UnID, AgroKey_EncoderDecoder, CType(HttpContext.Current.Session("ASG_objParametri_Server"), AgronicaCoreParametri))
        cn = Stringa_Codifica(cn, AgroKey_EncoderDecoder, CType(HttpContext.Current.Session("ASG_objParametri_Server"), AgronicaCoreParametri))

        'modifica per superserver, occorre passare la stringa connessione
        'non essendoci piu il file ini e covendo leggere
        'la connessione al server dalla tabella del superserver usanbdo l'id numerico cn

        If Not IsNothing(HttpContext.Current.Session("ASG_objParametri_Super_Server")) Then
            Dim StrConSup As String = ""
            Dim ASG_objParametri_Super_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Super_Server")
            StrConSup = ASG_objParametri_Super_Server.StringaConnessione
            StrConSup = Stringa_Codifica(StrConSup, AgroKey_EncoderDecoder, CType(HttpContext.Current.Session("ASG_objParametri_Server"), AgronicaCoreParametri))

            Return hrefSito & "?unid=" & UnID & "&cn=" & cn & "&StrConSup=" & StrConSup

        Else

            Return hrefSito & "?unid=" & UnID & "&cn=" & cn


        End If




    End Function



#End Region






End Class
