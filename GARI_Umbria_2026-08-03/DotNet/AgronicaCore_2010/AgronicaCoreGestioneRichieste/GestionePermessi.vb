Imports System.Web
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreUtentiDAL

''' -----------------------------------------------------------------------------
''' Project	 : AgronicaCoreGestioneRichieste
''' Class	 : AgroWebConfig
''' 
''' -----------------------------------------------------------------------------
''' <summary>
''' RICORDA: quando si aggiunge una nuova chiave al webconfig ricordarsi di aggiungere all'interno della classe: 
''' **** La variabile privata
''' **** La corrispondente proprietà Pubblica
''' **** Aggiungere il parametro nella CARICA
''' **** Aggiungere il parametro nella LeggiXML 
''' **** Aggiungere il parametro nella GeneraXML
''' **** Modificare il costruttore che legge il Web config
''' </summary>
''' <remarks>
''' </remarks>
''' -----------------------------------------------------------------------------
Public Class Permessi_PDC

    Public ControlloValiditaCapitolati_R As Boolean
    Public ControlloValiditaCapitolati_W As Boolean

    Public ControlloValiditaQDC_R As Boolean
    Public ControlloValiditaQDC_W As Boolean

    Public GestioneLaboratori_R As Boolean
    Public GestioneLaboratori_W As Boolean

    Public Analisi_R As Boolean
    Public Analisi_W As Boolean

    Public AnalisixLaboratori_R As Boolean
    Public AnalisixLaboratori_W As Boolean

    Public AnalisiPubblicazione_R As Boolean
    Public AnalisiPubblicazione_W As Boolean

    Public GestioneBloccoSblocco_R As Boolean
    Public GestioneBloccoSblocco_W As Boolean

    Public GestioneImpostazioni_R As Boolean
    Public GestioneImpostazioni_W As Boolean

    Public GestioneLotti_R As Boolean
    Public GestioneLotti_W As Boolean

    Public GestionePianoCampioni_R As Boolean
    Public GestionePianoCampioni_W As Boolean

    Public GestionePianoProduttivo_R As Boolean
    Public GestionePianoProduttivo_W As Boolean

    Public GestioneWorkFlow_R As Boolean
    Public GestioneWorkFlow_W As Boolean

    Public InterrogazioneBloccoSblocco_R As Boolean
    Public InterrogazioneBloccoSblocco_W As Boolean

    Public Menu_R As Boolean
    Public Menu_W As Boolean

    Public Filtrone_R As Boolean
    Public Filtrone_W As Boolean

    Public CreaNonConformitaTestata_W As Boolean

    Public MarketAccess_R As Boolean
    Public MarketAccess_W As Boolean

    Public MarketAccess_Globale_R As Boolean

    Public RichiestaAnalisiMultiple_R As Boolean
    Public RichiestaAnalisiMultiple_W As Boolean

    Public PDCZootecnia_R As Boolean
    Public PDCZootecnia_W As Boolean

    ''' <summary>
    ''' Crea l'oggetto a partire dalla sessione
    ''' </summary>
    Public Sub New()

    End Sub

    Public Sub Inizializza()

        Dim objUtentiPermessi As New Utenti_Permessi_R
        Dim dt As DataTable = objUtentiPermessi.Leggi(HttpContext.Current.Session("ASG_Utente_Username"),
                                                      HttpContext.Current.Session("ASG_IdServizio"),
                                                      0,
                                                      9999,
                                                      0,
                                                      "",
                                                      "",
                                                      HttpContext.Current.Session("ASG_objParametri_Utenti"))


        GestioneLaboratori_R = Controlla(dt, enum_Security_Attivita.PianiCampionamento_GestioneLaboratori, enum_Security_Operazione.Lettura)
        ControlloValiditaCapitolati_R = Controlla(dt, enum_Security_Attivita.PianiCampionamento_ControlloValiditaCapitolati, enum_Security_Operazione.Lettura)
        ControlloValiditaQDC_R = Controlla(dt, enum_Security_Attivita.PianiCampionamento_ControlloValiditaQDC, enum_Security_Operazione.Lettura)
        Analisi_R = Controlla(dt, enum_Security_Attivita.PianiCampionamento_GestioneAnalisi, enum_Security_Operazione.Lettura)
        AnalisixLaboratori_R = Controlla(dt, enum_Security_Attivita.PianiCampionamento_GestioneAnalisiXLaboratori, enum_Security_Operazione.Lettura)
        AnalisiPubblicazione_R = Controlla(dt, enum_Security_Attivita.PianiCampionamento_PubblicazioneAnalisi, enum_Security_Operazione.Lettura)
        GestioneBloccoSblocco_R = Controlla(dt, enum_Security_Attivita.PianiCampionamento_GestioneBloccoSblocco, enum_Security_Operazione.Lettura)
        GestioneImpostazioni_R = Controlla(dt, enum_Security_Attivita.PianiCampionamento_GestioneImpostazioni, enum_Security_Operazione.Lettura)
        GestioneLotti_R = Controlla(dt, enum_Security_Attivita.PianiCampionamento_GestioneLotti, enum_Security_Operazione.Lettura)
        GestionePianoCampioni_R = Controlla(dt, enum_Security_Attivita.PianiCampionamento_GestionePianoCampioni, enum_Security_Operazione.Lettura)
        GestionePianoProduttivo_R = Controlla(dt, enum_Security_Attivita.PianiCampionamento_GestionePianoProduttivo, enum_Security_Operazione.Lettura)
        GestioneWorkFlow_R = Controlla(dt, enum_Security_Attivita.PianiCampionamento_GestioneWorkFlow, enum_Security_Operazione.Lettura)
        InterrogazioneBloccoSblocco_R = Controlla(dt, enum_Security_Attivita.PianiCampionamento_InterrogazioneBloccoSblocco, enum_Security_Operazione.Lettura)
        Menu_R = Controlla(dt, enum_Security_Attivita.PianiCampionamento_Menu, enum_Security_Operazione.Lettura)
        Filtrone_R = Controlla(dt, enum_Security_Attivita.PianiCampionamento_Filtrone, enum_Security_Operazione.Lettura)
        MarketAccess_R = Controlla(dt, enum_Security_Attivita.PianiCampionamento_MarketAccess, enum_Security_Operazione.Lettura)
        MarketAccess_Globale_R = Controlla(dt, enum_Security_Attivita.PianiCampionamento_MarketAccess_Globale, enum_Security_Operazione.Lettura)
        RichiestaAnalisiMultiple_R = Controlla(dt, enum_Security_Attivita.PianiCampionamento_RichiestaAnalisiMultiple, enum_Security_Operazione.Lettura)
        PDCZootecnia_R = Controlla(dt, enum_Security_Attivita.PianiCampionamento_Zootecnia, enum_Security_Operazione.Lettura)

        GestioneLaboratori_W = Controlla(dt, enum_Security_Attivita.PianiCampionamento_GestioneLaboratori, enum_Security_Operazione.Modifica)
        ControlloValiditaCapitolati_W = Controlla(dt, enum_Security_Attivita.PianiCampionamento_ControlloValiditaCapitolati, enum_Security_Operazione.Modifica)
        ControlloValiditaQDC_W = Controlla(dt, enum_Security_Attivita.PianiCampionamento_ControlloValiditaQDC, enum_Security_Operazione.Modifica)
        Analisi_W = Controlla(dt, enum_Security_Attivita.PianiCampionamento_GestioneAnalisi, enum_Security_Operazione.Modifica)
        AnalisixLaboratori_W = Controlla(dt, enum_Security_Attivita.PianiCampionamento_GestioneAnalisiXLaboratori, enum_Security_Operazione.Modifica)
        AnalisiPubblicazione_W = Controlla(dt, enum_Security_Attivita.PianiCampionamento_PubblicazioneAnalisi, enum_Security_Operazione.Modifica)
        GestioneBloccoSblocco_W = Controlla(dt, enum_Security_Attivita.PianiCampionamento_GestioneBloccoSblocco, enum_Security_Operazione.Modifica)
        GestioneImpostazioni_W = Controlla(dt, enum_Security_Attivita.PianiCampionamento_GestioneImpostazioni, enum_Security_Operazione.Modifica)
        GestioneLotti_W = Controlla(dt, enum_Security_Attivita.PianiCampionamento_GestioneLotti, enum_Security_Operazione.Modifica)
        GestionePianoCampioni_W = Controlla(dt, enum_Security_Attivita.PianiCampionamento_GestionePianoCampioni, enum_Security_Operazione.Modifica)
        GestionePianoProduttivo_W = Controlla(dt, enum_Security_Attivita.PianiCampionamento_GestionePianoProduttivo, enum_Security_Operazione.Modifica)
        GestioneWorkFlow_W = Controlla(dt, enum_Security_Attivita.PianiCampionamento_GestioneWorkFlow, enum_Security_Operazione.Modifica)
        InterrogazioneBloccoSblocco_W = Controlla(dt, enum_Security_Attivita.PianiCampionamento_InterrogazioneBloccoSblocco, enum_Security_Operazione.Modifica)
        Menu_W = Controlla(dt, enum_Security_Attivita.PianiCampionamento_Menu, enum_Security_Operazione.Modifica)
        Filtrone_W = Controlla(dt, enum_Security_Attivita.PianiCampionamento_Filtrone, enum_Security_Operazione.Modifica)
        CreaNonConformitaTestata_W = Controlla(dt, enum_Security_Attivita.NonConformita_Testata_Crea, enum_Security_Operazione.Modifica)
        MarketAccess_W = Controlla(dt, enum_Security_Attivita.PianiCampionamento_MarketAccess, enum_Security_Operazione.Modifica)
        RichiestaAnalisiMultiple_W = Controlla(dt, enum_Security_Attivita.PianiCampionamento_RichiestaAnalisiMultiple, enum_Security_Operazione.Modifica)
        PDCZootecnia_W = Controlla(dt, enum_Security_Attivita.PianiCampionamento_Zootecnia, enum_Security_Operazione.Modifica)

        HttpContext.Current.Session("Permessi_PDC") = Me

    End Sub

    Private Function Controlla(ByVal DT As DataTable, ByVal enum_permesso As Integer, ByVal enum_lettura_modifica As Integer)
        Dim dr() As DataRow = DT.Select("Id_Attivita = '" & enum_permesso & "' AND Id_Operazione ='" & enum_lettura_modifica & "'")
        If dr.Length > 0 Then
            Return True
        End If
        Return False
    End Function

End Class
