
Imports System.Web
Imports AgronicaCoreDataProvider
Imports Agronica.Helpers.GiasBase

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
''' ''' **** Aggiungere il parametro nella GeneraXML
''' **** Modificare il costruttore che legge il Web config
''' </summary>
''' <remarks>
''' </remarks>
''' -----------------------------------------------------------------------------
Public Class AgroWebConfig

#Region "Variabili"
    'contiene tutte le chiavi dei vari web config
    Private array_key As String()

    Public _Hash_Chiavi As Hashtable

    Private _Sessione As Boolean

#Region "proprietà"



    Public Property GoogleMaps() As String
        Get
            Return _Hash_Chiavi(LCase("googlemaps"))
        End Get
        Set(ByVal value As String)

            If _Hash_Chiavi.ContainsKey(LCase("googlemaps")) Then
                _Hash_Chiavi(LCase("googlemaps")) = value
            Else
                _Hash_Chiavi.Add(LCase("googlemaps"), value)
            End If
        End Set

    End Property

    Public Property googlemaps_jsapi() As String
        Get
            Return _Hash_Chiavi(LCase("googlemaps_jsapi"))
        End Get
        Set(ByVal value As String)

            If _Hash_Chiavi.ContainsKey(LCase("googlemaps_jsapi")) Then
                _Hash_Chiavi(LCase("googlemaps_jsapi")) = value
            Else
                _Hash_Chiavi.Add(LCase("googlemaps_jsapi"), value)
            End If
        End Set

    End Property

    Public Property LinkWS_Importa_GIAS() As String
        Get
            Return _Hash_Chiavi(LCase("LinkWS_Importa_GIAS"))
        End Get
        Set(ByVal value As String)

            If _Hash_Chiavi.ContainsKey(LCase("LinkWS_Importa_GIAS")) Then
                _Hash_Chiavi(LCase("LinkWS_Importa_GIAS")) = value
            Else
                _Hash_Chiavi.Add(LCase("LinkWS_Importa_GIAS"), value)
            End If
        End Set

    End Property


    Public Property user_smtp() As String
        Get
            Return _Hash_Chiavi(LCase("user_smtp"))
        End Get
        Set(ByVal value As String)

            If _Hash_Chiavi.ContainsKey(LCase("user_smtp")) Then
                _Hash_Chiavi(LCase("user_smtp")) = value
            Else
                _Hash_Chiavi.Add(LCase("user_smtp"), value)
            End If
        End Set

    End Property
    Public Property password_smtp() As String
        Get
            Return _Hash_Chiavi(LCase("password_smtp"))
        End Get
        Set(ByVal value As String)

            If _Hash_Chiavi.ContainsKey(LCase("password_smtp")) Then
                _Hash_Chiavi(LCase("password_smtp")) = value
            Else
                _Hash_Chiavi.Add(LCase("password_smtp"), value)
            End If
        End Set

    End Property



    Public Property Path_Directory_Loghi_Cliente() As String
        Get
            Return _Hash_Chiavi(LCase("Path_Directory_Loghi_Cliente"))
        End Get
        Set(ByVal value As String)

            If _Hash_Chiavi.ContainsKey(LCase("Path_Directory_Loghi_Cliente")) Then
                _Hash_Chiavi(LCase("Path_Directory_Loghi_Cliente")) = value
            Else
                _Hash_Chiavi.Add(LCase("Path_Directory_Loghi_Cliente"), value)
            End If
        End Set

    End Property

    Public Property Codici_Analisi_PDC_x_Fruttagel() As String
        Get
            Return _Hash_Chiavi(LCase("Codici_Analisi_PDC_x_Fruttagel"))
        End Get
        Set(ByVal value As String)

            If _Hash_Chiavi.ContainsKey(LCase("Codici_Analisi_PDC_x_Fruttagel")) Then
                _Hash_Chiavi(LCase("Codici_Analisi_PDC_x_Fruttagel")) = value
            Else
                _Hash_Chiavi.Add(LCase("Codici_Analisi_PDC_x_Fruttagel"), value)
            End If
        End Set

    End Property


    Public Property LinkGiasOnline_2010() As String
        Get
            Return _Hash_Chiavi(LCase("LinkGiasOnline_2010"))
        End Get
        Set(ByVal value As String)

            If _Hash_Chiavi.ContainsKey(LCase("LinkGiasOnline_2010")) Then
                _Hash_Chiavi(LCase("LinkGiasOnline_2010")) = value
            Else
                _Hash_Chiavi.Add(LCase("LinkGiasOnline_2010"), value)
            End If
        End Set

    End Property

    Public Property WS_Timeout As String
        Get
            Return _Hash_Chiavi(LCase("WS_Timeout"))
        End Get
        Set(ByVal value As String)
            If _Hash_Chiavi.ContainsKey(LCase("WS_Timeout")) Then
                _Hash_Chiavi(LCase("WS_Timeout")) = value
            Else
                _Hash_Chiavi.Add(LCase("WS_Timeout"), value)
            End If
        End Set
    End Property

    'Private _Analisi2010 As String
    Public Property Analisi2010() As String
        Get
            Return _Hash_Chiavi(LCase("Analisi2010"))
        End Get
        Set(ByVal value As String)
            If _Hash_Chiavi.ContainsKey(LCase("Analisi2010")) Then
                _Hash_Chiavi(LCase("Analisi2010")) = value
            Else
                _Hash_Chiavi.Add(LCase("Analisi2010"), value)
            End If

        End Set
    End Property

    'Private _Agenda2010 As String
    Public Property Agenda2010() As String
        Get
            Return _Hash_Chiavi(LCase("Agenda2010"))
        End Get
        Set(ByVal value As String)

            If _Hash_Chiavi.ContainsKey(LCase("Agenda2010")) Then
                _Hash_Chiavi(LCase("Agenda2010")) = value
            Else
                _Hash_Chiavi.Add(LCase("Agenda2010"), value)
            End If
        End Set
    End Property

    'Private _Stampe2010 As String
    Public Property Stampe2010() As String
        Get
            Return _Hash_Chiavi(LCase("Stampe2010"))
        End Get
        Set(ByVal value As String)
            If _Hash_Chiavi.ContainsKey(LCase("Stampe2010")) Then
                _Hash_Chiavi(LCase("Stampe2010")) = value
            Else
                _Hash_Chiavi.Add(LCase("Stampe2010"), value)
            End If

        End Set
    End Property

    'Private _Stampe2010 As String
    Public Property enumStampe2010() As String
        Get
            Return _Hash_Chiavi(LCase("enumStampe2010"))
        End Get
        Set(ByVal value As String)
            If _Hash_Chiavi.ContainsKey(LCase("enumStampe2010")) Then
                _Hash_Chiavi(LCase("enumStampe2010")) = value
            Else
                _Hash_Chiavi.Add(LCase("enumStampe2010"), value)
            End If

        End Set
    End Property

    'Private _LinkGiasOnline As String
    Public Property LinkGiasOnline() As String
        Get
            Return _Hash_Chiavi(LCase("LinkGiasOnline"))
        End Get
        Set(ByVal value As String)

            If _Hash_Chiavi.ContainsKey(LCase("LinkGiasOnline")) Then
                _Hash_Chiavi(LCase("LinkGiasOnline")) = value
            Else
                _Hash_Chiavi.Add(LCase("LinkGiasOnline"), value)
            End If
        End Set
    End Property

    'Private _StarGate_Versione As String
    Public Property StarGate_Versione() As String
        Get
            Return _Hash_Chiavi(LCase("StarGate_Versione"))
        End Get
        Set(ByVal value As String)

            If _Hash_Chiavi.ContainsKey(LCase("StarGate_Versione")) Then
                _Hash_Chiavi(LCase("StarGate_Versione")) = value
            Else
                _Hash_Chiavi.Add(LCase("StarGate_Versione"), value)
            End If
        End Set
    End Property

    'Private _StarGate_Connessione As String
    Public Property StarGate_Connessione() As String
        Get
            Return _Hash_Chiavi(LCase("StarGate_Connessione"))
        End Get
        Set(ByVal value As String)
            If _Hash_Chiavi.ContainsKey(LCase("StarGate_Connessione")) Then
                _Hash_Chiavi(LCase("StarGate_Connessione")) = value
            Else
                _Hash_Chiavi.Add(LCase("StarGate_Connessione"), value)
            End If

        End Set
    End Property

    'Private _StarGate_ConsentiGateBypass As String
    Public Property StarGate_ConsentiGateBypass() As String
        Get
            Return _Hash_Chiavi(LCase("StarGate_ConsentiGateBypass"))
        End Get
        Set(ByVal value As String)

            If _Hash_Chiavi.ContainsKey(LCase("StarGate_ConsentiGateBypass")) Then
                _Hash_Chiavi(LCase("StarGate_ConsentiGateBypass")) = value
            Else
                _Hash_Chiavi.Add(LCase("StarGate_ConsentiGateBypass"), value)
            End If
        End Set
    End Property
    'Private _StarGate_LinkBypassBloccato As String
    Public Property StarGate_LinkBypassBloccato() As String
        Get
            Return _Hash_Chiavi(LCase("StarGate_LinkBypassBloccato"))
        End Get
        Set(ByVal value As String)

            If _Hash_Chiavi.ContainsKey(LCase("StarGate_LinkBypassBloccato")) Then
                _Hash_Chiavi(LCase("StarGate_LinkBypassBloccato")) = value
            Else
                _Hash_Chiavi.Add(LCase("StarGate_LinkBypassBloccato"), value)
            End If
        End Set
    End Property

    'Private _StarGate_CodicePredefinito As String
    Public Property StarGate_CodicePredefinito() As String
        Get
            Return _Hash_Chiavi(LCase("StarGate_CodicePredefinito"))
        End Get
        Set(ByVal value As String)

            If _Hash_Chiavi.ContainsKey(LCase("StarGate_CodicePredefinito")) Then
                _Hash_Chiavi(LCase("StarGate_CodicePredefinito")) = value
            Else
                _Hash_Chiavi.Add(LCase("StarGate_CodicePredefinito"), value)
            End If
        End Set
    End Property

    'Private _StarGate_PathF... As String
    Public Property StarGate_PathFileINI() As String
        Get
            Return _Hash_Chiavi(LCase("StarGate_PathFileINI"))
        End Get
        Set(ByVal value As String)

            If _Hash_Chiavi.ContainsKey(LCase("StarGate_PathFileINI")) Then
                _Hash_Chiavi(LCase("StarGate_PathFileINI")) = value
            Else
                _Hash_Chiavi.Add(LCase("StarGate_PathFileINI"), value)
            End If
        End Set
    End Property

    'Private _StarGate_Whynot As String
    Public Property StarGate_Whynot() As String
        Get
            Return _Hash_Chiavi(LCase("StarGate_Whynot"))
        End Get
        Set(ByVal value As String)

            If _Hash_Chiavi.ContainsKey(LCase("StarGate_Whynot")) Then
                _Hash_Chiavi(LCase("StarGate_Whynot")) = value
            Else
                _Hash_Chiavi.Add(LCase("StarGate_Whynot"), value)
            End If
        End Set
    End Property

    'Private _GestioneAllegati_Repository As String
    Public Property GestioneAllegati_Repository() As String
        Get
            Return _Hash_Chiavi(LCase("GestioneAllegati_Repository"))
        End Get
        Set(ByVal value As String)

            If _Hash_Chiavi.ContainsKey(LCase("GestioneAllegati_Repository")) Then
                _Hash_Chiavi(LCase("GestioneAllegati_Repository")) = value
            Else
                _Hash_Chiavi.Add(LCase("GestioneAllegati_Repository"), value)
            End If
        End Set
    End Property

    'Private _GestioneImportazioni_Repository As String
    Public Property GestioneImportazioni_Repository() As String
        Get
            Return _Hash_Chiavi(LCase("GestioneImportazioni_Repository"))
        End Get
        Set(ByVal value As String)

            If _Hash_Chiavi.ContainsKey(LCase("GestioneImportazioni_Repository")) Then
                _Hash_Chiavi(LCase("GestioneImportazioni_Repository")) = value
            Else
                _Hash_Chiavi.Add(LCase("GestioneImportazioni_Repository"), value)
            End If
        End Set
    End Property

    'Private _GestioneEsportazioni_Repository As String
    Public Property GestioneEsportazioni_Repository() As String
        Get
            Return _Hash_Chiavi(LCase("GestioneEsportazioni_Repository"))
        End Get
        Set(ByVal value As String)

            If _Hash_Chiavi.ContainsKey(LCase("GestioneEsportazioni_Repository")) Then
                _Hash_Chiavi(LCase("GestioneEsportazioni_Repository")) = value
            Else
                _Hash_Chiavi.Add(LCase("GestioneEsportazioni_Repository"), value)
            End If
        End Set
    End Property
    'Private _GestioneCartografia_Repository As String
    Public Property GestioneCartografia_Repository() As String
        Get
            Return _Hash_Chiavi(LCase("GestioneCartografia_Repository"))
        End Get
        Set(ByVal value As String)

            If _Hash_Chiavi.ContainsKey(LCase("GestioneCartografia_Repository")) Then
                _Hash_Chiavi(LCase("GestioneCartografia_Repository")) = value
            Else
                _Hash_Chiavi.Add(LCase("GestioneCartografia_Repository"), value)
            End If
        End Set
    End Property

    'Private _LinkManualeGiasOnline As String
    Public Property LinkManualeGiasOnline() As String
        Get
            Return _Hash_Chiavi(LCase("LinkManualeGiasOnline"))
        End Get
        Set(ByVal value As String)

            If _Hash_Chiavi.ContainsKey(LCase("LinkManualeGiasOnline")) Then
                _Hash_Chiavi(LCase("LinkManualeGiasOnline")) = value
            Else
                _Hash_Chiavi.Add(LCase("LinkManualeGiasOnline"), value)
            End If
        End Set
    End Property
    ' Private _LinkAgronicaStampe As String
    Public Property LinkAgronicaStampe() As String
        Get
            Return _Hash_Chiavi(LCase("LinkAgronicaStampe"))
        End Get
        Set(ByVal value As String)

            If _Hash_Chiavi.ContainsKey(LCase("LinkAgronicaStampe")) Then
                _Hash_Chiavi(LCase("LinkAgronicaStampe")) = value
            Else
                _Hash_Chiavi.Add(LCase("LinkAgronicaStampe"), value)
            End If
        End Set
    End Property

    ' Private _LinkAgronicaStampe As String
    Public Property LinkAgronicaStampe_2010() As String
        Get
            Return _Hash_Chiavi(LCase("LinkAgronicaStampe_2010"))
        End Get
        Set(ByVal value As String)

            If _Hash_Chiavi.ContainsKey(LCase("LinkAgronicaStampe_2010")) Then
                _Hash_Chiavi(LCase("LinkAgronicaStampe_2010")) = value
            Else
                _Hash_Chiavi.Add(LCase("LinkAgronicaStampe_2010"), value)
            End If
        End Set
    End Property

    Public Property LinkAgronicaLabControlloQualita() As String
        Get
            Return _Hash_Chiavi(LCase("LinkAgronicaLabControlloQualita"))
        End Get
        Set(ByVal value As String)

            If _Hash_Chiavi.ContainsKey(LCase("LinkAgronicaLabControlloQualita")) Then
                _Hash_Chiavi(LCase("LinkAgronicaLabControlloQualita")) = value
            Else
                _Hash_Chiavi.Add(LCase("LinkAgronicaLabControlloQualita"), value)
            End If
        End Set
    End Property

    'Private _LinkPianoConcimazione As String
    Public Property LinkPianoConcimazione() As String
        Get
            Return _Hash_Chiavi(LCase("LinkPianoConcimazione"))
        End Get
        Set(ByVal value As String)

            If _Hash_Chiavi.ContainsKey(LCase("LinkPianoConcimazione")) Then
                _Hash_Chiavi(LCase("LinkPianoConcimazione")) = value
            Else
                _Hash_Chiavi.Add(LCase("LinkPianoConcimazione"), value)
            End If
        End Set
    End Property


    'Private _LinkPianoConcimazione_2017 As String
    Public Property LinkPianoConcimazione_2017() As String
        Get
            Return _Hash_Chiavi(LCase("LinkPianoConcimazione_2017"))
        End Get
        Set(ByVal value As String)

            If _Hash_Chiavi.ContainsKey(LCase("LinkPianoConcimazione_2017")) Then
                _Hash_Chiavi(LCase("LinkPianoConcimazione_2017")) = value
            Else
                _Hash_Chiavi.Add(LCase("LinkPianoConcimazione_2017"), value)
            End If
        End Set
    End Property

    'Private _LinkAgronicaBio As String
    Public Property LinkAgronicaBio() As String
        Get
            Return _Hash_Chiavi(LCase("LinkAgronicaBio"))
        End Get
        Set(ByVal value As String)

            If _Hash_Chiavi.ContainsKey(LCase("LinkAgronicaBio")) Then
                _Hash_Chiavi(LCase("LinkAgronicaBio")) = value
            Else
                _Hash_Chiavi.Add(LCase("LinkAgronicaBio"), value)
            End If
        End Set

    End Property
    ' Private _LinkAgronicaView As String
    Public Property LinkAgronicaView() As String
        Get
            Return _Hash_Chiavi(LCase("LinkAgronicaView"))
        End Get
        Set(ByVal value As String)

            If _Hash_Chiavi.ContainsKey(LCase("LinkAgronicaView")) Then
                _Hash_Chiavi(LCase("LinkAgronicaView")) = value
            Else
                _Hash_Chiavi.Add(LCase("LinkAgronicaView"), value)
            End If
        End Set
    End Property
    ' Private _LinkAgronicaAnalisi As String
    Public Property LinkAgronicaAnalisi() As String
        Get
            Return _Hash_Chiavi(LCase("LinkAgronicaAnalisi"))
        End Get
        Set(ByVal value As String)

            If _Hash_Chiavi.ContainsKey(LCase("LinkAgronicaAnalisi")) Then
                _Hash_Chiavi(LCase("LinkAgronicaAnalisi")) = value
            Else
                _Hash_Chiavi.Add(LCase("LinkAgronicaAnalisi"), value)
            End If
        End Set
    End Property
    ' Private _LinkAgronicaAnalisi_2010 As String
    Public Property LinkAgronicaAnalisi_2010() As String
        Get
            Return _Hash_Chiavi(LCase("LinkAgronicaAnalisi_2010"))
        End Get
        Set(ByVal value As String)

            If _Hash_Chiavi.ContainsKey(LCase("LinkAgronicaAnalisi_2010")) Then
                _Hash_Chiavi(LCase("LinkAgronicaAnalisi_2010")) = value
            Else
                _Hash_Chiavi.Add(LCase("LinkAgronicaAnalisi_2010"), value)
            End If
        End Set
    End Property
    'Private _LinkAgronicaAudit As String
    Public Property LinkAgronicaAudit() As String
        Get
            Return _Hash_Chiavi(LCase("LinkAgronicaAudit"))
        End Get
        Set(ByVal value As String)

            If _Hash_Chiavi.ContainsKey(LCase("LinkAgronicaAudit")) Then
                _Hash_Chiavi(LCase("LinkAgronicaAudit")) = value
            Else
                _Hash_Chiavi.Add(LCase("LinkAgronicaAudit"), value)
            End If
        End Set
    End Property
    ' Private _LinkAgronicaSicurezzaLavoro As String
    Public Property LinkAgronicaSicurezzaLavoro() As String
        Get
            Return _Hash_Chiavi(LCase("LinkAgronicaSicurezzaLavoro"))
        End Get
        Set(ByVal value As String)

            If _Hash_Chiavi.ContainsKey(LCase("LinkAgronicaSicurezzaLavoro")) Then
                _Hash_Chiavi(LCase("LinkAgronicaSicurezzaLavoro")) = value
            Else
                _Hash_Chiavi.Add(LCase("LinkAgronicaSicurezzaLavoro"), value)
            End If
        End Set
    End Property
    ' Private _LinkAgronicaPua As String
    Public Property LinkAgronicaPua() As String
        Get
            Return _Hash_Chiavi(LCase("LinkAgronicaPua"))
        End Get
        Set(ByVal value As String)

            If _Hash_Chiavi.ContainsKey(LCase("LinkAgronicaPua")) Then
                _Hash_Chiavi(LCase("LinkAgronicaPua")) = value
            Else
                _Hash_Chiavi.Add(LCase("LinkAgronicaPua"), value)
            End If
        End Set
    End Property
    'Private _LinkAgronicaPlanning As String
    Public Property LinkAgronicaPlanning() As String
        Get
            Return _Hash_Chiavi(LCase("LinkAgronicaPlanning"))
        End Get
        Set(ByVal value As String)

            If _Hash_Chiavi.ContainsKey(LCase("LinkAgronicaPlanning")) Then
                _Hash_Chiavi(LCase("LinkAgronicaPlanning")) = value
            Else
                _Hash_Chiavi.Add(LCase("LinkAgronicaPlanning"), value)
            End If
        End Set
    End Property
    ' Private _LinkAgronicaProfilazione As String
    Public Property LinkAgronicaProfilazione() As String
        Get
            Return _Hash_Chiavi(LCase("LinkAgronicaProfilazione"))
        End Get
        Set(ByVal value As String)

            If _Hash_Chiavi.ContainsKey(LCase("LinkAgronicaProfilazione")) Then
                _Hash_Chiavi(LCase("LinkAgronicaProfilazione")) = value
            Else
                _Hash_Chiavi.Add(LCase("LinkAgronicaProfilazione"), value)
            End If
        End Set
    End Property
    ' Private _LinkAgronicaManutenzione As String
    Public Property LinkAgronicaManutenzione() As String
        Get
            Return _Hash_Chiavi(LCase("LinkAgronicaManutenzione"))
        End Get
        Set(ByVal value As String)

            If _Hash_Chiavi.ContainsKey(LCase("LinkAgronicaManutenzione")) Then
                _Hash_Chiavi(LCase("LinkAgronicaManutenzione")) = value
            Else
                _Hash_Chiavi.Add(LCase("LinkAgronicaManutenzione"), value)
            End If
        End Set
    End Property
    'Private _LinkAgronicaPianiCampionamento As String
    Public Property LinkAgronicaPianiCampionamento() As String
        Get
            Return _Hash_Chiavi(LCase("LinkAgronicaPianiCampionamento"))
        End Get
        Set(ByVal value As String)

            If _Hash_Chiavi.ContainsKey(LCase("LinkAgronicaPianiCampionamento")) Then
                _Hash_Chiavi(LCase("LinkAgronicaPianiCampionamento")) = value
            Else
                _Hash_Chiavi.Add(LCase("LinkAgronicaPianiCampionamento"), value)
            End If
        End Set
    End Property

    Public Property LinkAgronicaPianiSemina() As String
        Get
            Return _Hash_Chiavi(LCase("LinkAgronicaPianiSemina"))
        End Get
        Set(ByVal value As String)

            If _Hash_Chiavi.ContainsKey(LCase("LinkAgronicaPianiSemina")) Then
                _Hash_Chiavi(LCase("LinkAgronicaPianiSemina")) = value
            Else
                _Hash_Chiavi.Add(LCase("LinkAgronicaPianiSemina"), value)
            End If
        End Set
    End Property
    Public Property LinkAgronicaMeteo() As String
        Get
            Return _Hash_Chiavi(LCase("LinkAgronicaMeteo"))
        End Get
        Set(ByVal value As String)

            If _Hash_Chiavi.ContainsKey(LCase("LinkAgronicaMeteo")) Then
                _Hash_Chiavi(LCase("LinkAgronicaMeteo")) = value
            Else
                _Hash_Chiavi.Add(LCase("LinkAgronicaMeteo"), value)
            End If
        End Set
    End Property

    Public Property LinkSementieri() As String
        Get
            Return _Hash_Chiavi(LCase("LinkSementieri"))
        End Get
        Set(ByVal value As String)

            If _Hash_Chiavi.ContainsKey(LCase("LinkSementieri")) Then
                _Hash_Chiavi(LCase("LinkSementieri")) = value
            Else
                _Hash_Chiavi.Add(LCase("LinkSementieri"), value)
            End If
        End Set
    End Property
    'Private _LinkAgronicaAgenda2010 As String
    Public Property LinkAgronicaAgenda2010() As String
        Get
            Return _Hash_Chiavi(LCase("LinkAgronicaAgenda2010"))
        End Get
        Set(ByVal value As String)

            If _Hash_Chiavi.ContainsKey(LCase("LinkAgronicaAgenda2010")) Then
                _Hash_Chiavi(LCase("LinkAgronicaAgenda2010")) = value
            Else
                _Hash_Chiavi.Add(LCase("LinkAgronicaAgenda2010"), value)
            End If
        End Set
    End Property
    'Private _LinkAgronicaSincronizzatore As String
    Public Property LinkAgronicaSincronizzatore() As String
        Get
            Return _Hash_Chiavi(LCase("LinkAgronicaSincronizzatore"))
        End Get
        Set(ByVal value As String)

            If _Hash_Chiavi.ContainsKey(LCase("LinkAgronicaSincronizzatore")) Then
                _Hash_Chiavi(LCase("LinkAgronicaSincronizzatore")) = value
            Else
                _Hash_Chiavi.Add(LCase("LinkAgronicaSincronizzatore"), value)
            End If
        End Set
    End Property
    'Private _LinkCartografiaMappe As String
    Public Property LinkCartografiaMappe() As String
        Get
            Return _Hash_Chiavi(LCase("LinkCartografiaMappe"))
        End Get
        Set(ByVal value As String)

            If _Hash_Chiavi.ContainsKey(LCase("LinkCartografiaMappe")) Then
                _Hash_Chiavi(LCase("LinkCartografiaMappe")) = value
            Else
                _Hash_Chiavi.Add(LCase("LinkCartografiaMappe"), value)
            End If
        End Set
    End Property
    'Private _LinkProfitosan As String
    Public Property LinkProfitosan() As String
        Get
            Return _Hash_Chiavi(LCase("LinkProfitosan"))
        End Get
        Set(ByVal value As String)

            If _Hash_Chiavi.ContainsKey(LCase("LinkProfitosan")) Then
                _Hash_Chiavi(LCase("LinkProfitosan")) = value
            Else
                _Hash_Chiavi.Add(LCase("LinkProfitosan"), value)
            End If
        End Set
    End Property

    'Private _LinkProfitosan As String
    Public Property LinkProfitosan_WS() As String
        Get
            Return _Hash_Chiavi(LCase("LinkProfitosan_WS"))
        End Get
        Set(ByVal value As String)

            If _Hash_Chiavi.ContainsKey(LCase("LinkProfitosan_WS")) Then
                _Hash_Chiavi(LCase("LinkProfitosan_WS")) = value
            Else
                _Hash_Chiavi.Add(LCase("LinkProfitosan_WS"), value)
            End If
        End Set
    End Property

    'Private _LinkProfitosan_2023 As String
    Public Property LinkProfitosan_2023() As String
        Get
            Return _Hash_Chiavi(LCase("LinkProfitosan_2023"))
        End Get
        Set(ByVal value As String)

            If _Hash_Chiavi.ContainsKey(LCase("LinkProfitosan_2023")) Then
                _Hash_Chiavi(LCase("LinkProfitosan_2023")) = value
            Else
                _Hash_Chiavi.Add(LCase("LinkProfitosan_2023"), value)
            End If
        End Set
    End Property


    'Private _DescrizioneServer As String
    Public Property DescrizioneServer() As String
        Get
            Return _Hash_Chiavi(LCase("DescrizioneServer"))
        End Get
        Set(ByVal value As String)

            If _Hash_Chiavi.ContainsKey(LCase("DescrizioneServer")) Then
                _Hash_Chiavi(LCase("DescrizioneServer")) = value
            Else
                _Hash_Chiavi.Add(LCase("DescrizioneServer"), value)
            End If
        End Set
    End Property
    'Private _Connessione_ONLINE_Server As String
    Public Property Connessione_ONLINE_Server() As String
        Get
            Return _Hash_Chiavi(LCase("Connessione_ONLINE_Server"))
        End Get
        Set(ByVal value As String)

            If _Hash_Chiavi.ContainsKey(LCase("Connessione_ONLINE_Server")) Then
                _Hash_Chiavi(LCase("Connessione_ONLINE_Server")) = value
            Else
                _Hash_Chiavi.Add(LCase("Connessione_ONLINE_Server"), value)
            End If
        End Set
    End Property
    'Private _Connessione_ONLINE_Utenti As String
    Public Property Connessione_ONLINE_Utenti() As String
        Get
            Return _Hash_Chiavi(LCase("Connessione_ONLINE_Utenti"))
        End Get
        Set(ByVal value As String)

            If _Hash_Chiavi.ContainsKey(LCase("Connessione_ONLINE_Utenti")) Then
                _Hash_Chiavi(LCase("Connessione_ONLINE_Utenti")) = value
            Else
                _Hash_Chiavi.Add(LCase("Connessione_ONLINE_Utenti"), value)
            End If
        End Set
    End Property
    'Private _Connessione_ONLINE_DPI As String
    Public Property Connessione_ONLINE_DPI() As String
        Get
            Return _Hash_Chiavi(LCase("Connessione_ONLINE_DPI"))
        End Get
        Set(ByVal value As String)

            If _Hash_Chiavi.ContainsKey(LCase("Connessione_ONLINE_DPI")) Then
                _Hash_Chiavi(LCase("Connessione_ONLINE_DPI")) = value
            Else
                _Hash_Chiavi.Add(LCase("Connessione_ONLINE_DPI"), value)
            End If
        End Set
    End Property
    'Private _LinkVisualizzatoreDPI As String
    Public Property LinkVisualizzatoreDPI() As String
        Get
            Return _Hash_Chiavi(LCase("LinkVisualizzatoreDPI"))
        End Get
        Set(ByVal value As String)
            If _Hash_Chiavi.ContainsKey(LCase("LinkVisualizzatoreDPI")) Then
                _Hash_Chiavi(LCase("LinkVisualizzatoreDPI")) = value
            Else
                _Hash_Chiavi.Add(LCase("LinkVisualizzatoreDPI"), value)
            End If
        End Set
    End Property

    'Private _GiasOnline_WS_Disciplinari_AgroWS_Disciplinari As String
    Public Property GiasOnline_WS_Disciplinari_AgroWS_Disciplinari() As String
        Get
            Return _Hash_Chiavi(LCase("GiasOnline_WS_Disciplinari_AgroWS_Disciplinari"))
        End Get
        Set(ByVal value As String)

            If _Hash_Chiavi.ContainsKey(LCase("GiasOnline_WS_Disciplinari_AgroWS_Disciplinari")) Then
                _Hash_Chiavi(LCase("GiasOnline_WS_Disciplinari_AgroWS_Disciplinari")) = value
            Else
                _Hash_Chiavi.Add(LCase("GiasOnline_WS_Disciplinari_AgroWS_Disciplinari"), value)
            End If
        End Set
    End Property
    'Private _GiasOnline_WS_Fitofarmaci_AgroWS_Fitofarmaci As String
    Public Property GiasOnline_WS_Fitofarmaci_AgroWS_Fitofarmaci() As String
        Get
            Return _Hash_Chiavi(LCase("GiasOnline_WS_Fitofarmaci_AgroWS_Fitofarmaci"))
        End Get
        Set(ByVal value As String)

            If _Hash_Chiavi.ContainsKey(LCase("GiasOnline_WS_Fitofarmaci_AgroWS_Fitofarmaci")) Then
                _Hash_Chiavi(LCase("GiasOnline_WS_Fitofarmaci_AgroWS_Fitofarmaci")) = value
            Else
                _Hash_Chiavi.Add(LCase("GiasOnline_WS_Fitofarmaci_AgroWS_Fitofarmaci"), value)
            End If
        End Set
    End Property

    'Private _GiasOnline_WS_CapitolatoCliente_AgroWS_CapitolatoCliente As String
    Public Property GiasOnline_WS_CapitolatoCliente_AgroWS_CapitolatoCliente() As String
        Get
            Return _Hash_Chiavi(LCase("GiasOnline_WS_CapitolatoCliente_AgroWS_CapitolatoCliente"))
        End Get
        Set(ByVal value As String)

            If _Hash_Chiavi.ContainsKey(LCase("GiasOnline_WS_CapitolatoCliente_AgroWS_CapitolatoCliente")) Then
                _Hash_Chiavi(LCase("GiasOnline_WS_CapitolatoCliente_AgroWS_CapitolatoCliente")) = value
            Else
                _Hash_Chiavi.Add(LCase("GiasOnline_WS_CapitolatoCliente_AgroWS_CapitolatoCliente"), value)
            End If
        End Set
    End Property

    'Private GiasOnline_WS_Prodotti_Demetra As String
    Public Property GiasOnline_WS_Prodotti_Demetra() As String
        Get
            Return _Hash_Chiavi(LCase("GiasOnline_WS_Prodotti_Demetra"))
        End Get
        Set(ByVal value As String)

            If _Hash_Chiavi.ContainsKey(LCase("GiasOnline_WS_Prodotti_Demetra")) Then
                _Hash_Chiavi(LCase("GiasOnline_WS_Prodotti_Demetra")) = value
            Else
                _Hash_Chiavi.Add(LCase("GiasOnline_WS_Prodotti_Demetra"), value)
            End If
        End Set
    End Property

    'Private _GiasOnline_WS_Meteo_Meteo As String
    Public Property GiasOnline_WS_Meteo_Meteo() As String
        Get
            Return _Hash_Chiavi(LCase("GiasOnline_WS_Meteo_Meteo"))
        End Get
        Set(ByVal value As String)

            If _Hash_Chiavi.ContainsKey(LCase("GiasOnline_WS_Meteo_Meteo")) Then
                _Hash_Chiavi(LCase("GiasOnline_WS_Meteo_Meteo")) = value
            Else
                _Hash_Chiavi.Add(LCase("GiasOnline_WS_Meteo_Meteo"), value)
            End If
        End Set
    End Property


    'Private _GiasOnline_WS_Disciplinari_AgroWS_Disciplinari As String
    Public Property GiasOnline_WS_Disciplinari_AgroWS_Disciplinari_2() As String
        Get
            Return _Hash_Chiavi(LCase("GiasOnline_WS_Disciplinari_AgroWS_Disciplinari_2"))
        End Get
        Set(ByVal value As String)

            If _Hash_Chiavi.ContainsKey(LCase("GiasOnline_WS_Disciplinari_AgroWS_Disciplinari_2")) Then
                _Hash_Chiavi(LCase("GiasOnline_WS_Disciplinari_AgroWS_Disciplinari_2")) = value
            Else
                _Hash_Chiavi.Add(LCase("GiasOnline_WS_Disciplinari_AgroWS_Disciplinari_2"), value)
            End If
        End Set
    End Property
    'Private _GiasOnline_WS_Fitofarmaci_AgroWS_Fitofarmaci As String
    Public Property GiasOnline_WS_Fitofarmaci_AgroWS_Fitofarmaci_2() As String
        Get
            Return _Hash_Chiavi(LCase("GiasOnline_WS_Fitofarmaci_AgroWS_Fitofarmaci_2"))
        End Get
        Set(ByVal value As String)

            If _Hash_Chiavi.ContainsKey(LCase("GiasOnline_WS_Fitofarmaci_AgroWS_Fitofarmaci_2")) Then
                _Hash_Chiavi(LCase("GiasOnline_WS_Fitofarmaci_AgroWS_Fitofarmaci_2")) = value
            Else
                _Hash_Chiavi.Add(LCase("GiasOnline_WS_Fitofarmaci_AgroWS_Fitofarmaci_2"), value)
            End If
        End Set
    End Property

    'Private _GiasOnline_WS_CapitolatoCliente_AgroWS_CapitolatoCliente As String
    Public Property GiasOnline_WS_CapitolatoCliente_AgroWS_CapitolatoCliente_2() As String
        Get
            Return _Hash_Chiavi(LCase("GiasOnline_WS_CapitolatoCliente_AgroWS_CapitolatoCliente_2"))
        End Get
        Set(ByVal value As String)

            If _Hash_Chiavi.ContainsKey(LCase("GiasOnline_WS_CapitolatoCliente_AgroWS_CapitolatoCliente_2")) Then
                _Hash_Chiavi(LCase("GiasOnline_WS_CapitolatoCliente_AgroWS_CapitolatoCliente_2")) = value
            Else
                _Hash_Chiavi.Add(LCase("GiasOnline_WS_CapitolatoCliente_AgroWS_CapitolatoCliente_2"), value)
            End If
        End Set
    End Property

    'Private _GiasOnline_WS_Meteo_Meteo As String
    Public Property GiasOnline_WS_Meteo_Meteo_2() As String
        Get
            Return _Hash_Chiavi(LCase("GiasOnline_WS_Meteo_Meteo_2"))
        End Get
        Set(ByVal value As String)

            If _Hash_Chiavi.ContainsKey(LCase("GiasOnline_WS_Meteo_Meteo_2")) Then
                _Hash_Chiavi(LCase("GiasOnline_WS_Meteo_Meteo_2")) = value
            Else
                _Hash_Chiavi.Add(LCase("GiasOnline_WS_Meteo_Meteo_2"), value)
            End If
        End Set
    End Property


    Public Property GiasOnline_WS_Fertilizzanti_IAgroAPI_Fertilizzanti() As String
        Get
            Return _Hash_Chiavi(LCase("GiasOnline_WS_Fertilizzanti_IAgroAPI_Fertilizzanti"))
        End Get
        Set(ByVal value As String)

            If _Hash_Chiavi.ContainsKey(LCase("GiasOnline_WS_Fertilizzanti_IAgroAPI_Fertilizzanti")) Then
                _Hash_Chiavi(LCase("GiasOnline_WS_Fertilizzanti_IAgroAPI_Fertilizzanti")) = value
            Else
                _Hash_Chiavi.Add(LCase("GiasOnline_WS_Fertilizzanti_IAgroAPI_Fertilizzanti"), value)
            End If
        End Set
    End Property

    Public Property GiasOnline_WS_Fertilizzanti_IAgroAPI_Fertilizzanti_2() As String
        Get
            Return _Hash_Chiavi(LCase("GiasOnline_WS_Fertilizzanti_IAgroAPI_Fertilizzanti_2"))
        End Get
        Set(ByVal value As String)

            If _Hash_Chiavi.ContainsKey(LCase("GiasOnline_WS_Fertilizzanti_IAgroAPI_Fertilizzanti_2")) Then
                _Hash_Chiavi(LCase("GiasOnline_WS_Fertilizzanti_IAgroAPI_Fertilizzanti_2")) = value
            Else
                _Hash_Chiavi.Add(LCase("GiasOnline_WS_Fertilizzanti_IAgroAPI_Fertilizzanti_2"), value)
            End If
        End Set
    End Property

    Public Property GiasOnline_WS_SpecieVegetali_IAgroAPI_SpecieVegetali() As String
        Get
            Return _Hash_Chiavi(LCase("GiasOnline_WS_SpecieVegetali_IAgroAPI_SpecieVegetali"))
        End Get
        Set(ByVal value As String)

            If _Hash_Chiavi.ContainsKey(LCase("GiasOnline_WS_SpecieVegetali_IAgroAPI_SpecieVegetali")) Then
                _Hash_Chiavi(LCase("GiasOnline_WS_SpecieVegetali_IAgroAPI_SpecieVegetali")) = value
            Else
                _Hash_Chiavi.Add(LCase("GiasOnline_WS_SpecieVegetali_IAgroAPI_SpecieVegetali"), value)
            End If
        End Set
    End Property

    Public Property GiasOnline_WS_SpecieVegetali_IAgroAPI_SpecieVegetali_2() As String
        Get
            Return _Hash_Chiavi(LCase("GiasOnline_WS_SpecieVegetali_IAgroAPI_SpecieVegetali_2"))
        End Get
        Set(ByVal value As String)

            If _Hash_Chiavi.ContainsKey(LCase("GiasOnline_WS_SpecieVegetali_IAgroAPI_SpecieVegetali_2")) Then
                _Hash_Chiavi(LCase("GiasOnline_WS_SpecieVegetali_IAgroAPI_SpecieVegetali_2")) = value
            Else
                _Hash_Chiavi.Add(LCase("GiasOnline_WS_SpecieVegetali_IAgroAPI_SpecieVegetali_2"), value)
            End If
        End Set
    End Property


    Public Property GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione() As String
        Get
            Return _Hash_Chiavi(LCase("GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione"))
        End Get
        Set(ByVal value As String)

            If _Hash_Chiavi.ContainsKey(LCase("GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione")) Then
                _Hash_Chiavi(LCase("GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione")) = value
            Else
                _Hash_Chiavi.Add(LCase("GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione"), value)
            End If
        End Set
    End Property

    Public Property GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione_2() As String
        Get
            Return _Hash_Chiavi(LCase("GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione_2"))
        End Get
        Set(ByVal value As String)

            If _Hash_Chiavi.ContainsKey(LCase("GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione_2")) Then
                _Hash_Chiavi(LCase("GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione_2")) = value
            Else
                _Hash_Chiavi.Add(LCase("GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione_2"), value)
            End If
        End Set
    End Property


    'Private _Flag_WS_Fitofarmaci_Remoto As String
    Public Property Flag_WS_Fitofarmaci_Remoto() As String
        Get
            Return _Hash_Chiavi(LCase("Flag_WS_Fitofarmaci_Remoto"))
        End Get
        Set(ByVal value As String)

            If _Hash_Chiavi.ContainsKey(LCase("Flag_WS_Fitofarmaci_Remoto")) Then
                _Hash_Chiavi(LCase("Flag_WS_Fitofarmaci_Remoto")) = value
            Else
                _Hash_Chiavi.Add(LCase("Flag_WS_Fitofarmaci_Remoto"), value)
            End If
        End Set
    End Property
    'Private _Flag_DisciplinareAttivo As String
    Public Property Flag_DisciplinareAttivo() As String
        Get
            Return _Hash_Chiavi(LCase("Flag_DisciplinareAttivo"))
        End Get
        Set(ByVal value As String)

            If _Hash_Chiavi.ContainsKey(LCase("Flag_DisciplinareAttivo")) Then
                _Hash_Chiavi(LCase("Flag_DisciplinareAttivo")) = value
            Else
                _Hash_Chiavi.Add(LCase("Flag_DisciplinareAttivo"), value)
            End If
        End Set
    End Property
    'Private _Flag_DisciplinarePrivato As String
    Public Property Flag_DisciplinarePrivato() As String
        Get
            Return _Hash_Chiavi(LCase("Flag_DisciplinarePrivato"))
        End Get
        Set(ByVal value As String)

            If _Hash_Chiavi.ContainsKey(LCase("Flag_DisciplinarePrivato")) Then
                _Hash_Chiavi(LCase("Flag_DisciplinarePrivato")) = value
            Else
                _Hash_Chiavi.Add(LCase("Flag_DisciplinarePrivato"), value)
            End If
        End Set
    End Property
    'Private _Flag_SoglieAttive As String
    Public Property Flag_SoglieAttive() As String
        Get
            Return _Hash_Chiavi(LCase("Flag_SoglieAttive"))
        End Get
        Set(ByVal value As String)

            If _Hash_Chiavi.ContainsKey(LCase("Flag_SoglieAttive")) Then
                _Hash_Chiavi(LCase("Flag_SoglieAttive")) = value
            Else
                _Hash_Chiavi.Add(LCase("Flag_SoglieAttive"), value)
            End If
        End Set
    End Property
    ' Private _Flag_GestioneAnalisiAttivo As String
    Public Property Flag_GestioneAnalisiAttivo() As String
        Get
            Return _Hash_Chiavi(LCase("Flag_GestioneAnalisiAttivo"))
        End Get
        Set(ByVal value As String)

            If _Hash_Chiavi.ContainsKey(LCase("Flag_GestioneAnalisiAttivo")) Then
                _Hash_Chiavi(LCase("Flag_GestioneAnalisiAttivo")) = value
            Else
                _Hash_Chiavi.Add(LCase("Flag_GestioneAnalisiAttivo"), value)
            End If
        End Set
    End Property
    ' Private _Flag_Mirror As String
    Public Property Flag_Mirror() As String
        Get
            Return _Hash_Chiavi(LCase("Flag_Mirror"))
        End Get
        Set(ByVal value As String)

            If _Hash_Chiavi.ContainsKey(LCase("Flag_Mirror")) Then
                _Hash_Chiavi(LCase("Flag_Mirror")) = value
            Else
                _Hash_Chiavi.Add(LCase("Flag_Mirror"), value)
            End If
        End Set
    End Property
    'Private _LivelloEspansoAlberoImprese As String
    Public Property LivelloEspansoAlberoImprese() As String
        Get
            Return _Hash_Chiavi(LCase("LivelloEspansoAlberoImprese"))
        End Get
        Set(ByVal value As String)

            If _Hash_Chiavi.ContainsKey(LCase("LivelloEspansoAlberoImprese")) Then
                _Hash_Chiavi(LCase("LivelloEspansoAlberoImprese")) = value
            Else
                _Hash_Chiavi.Add(LCase("LivelloEspansoAlberoImprese"), value)
            End If
        End Set
    End Property
    'Private _EspandiNodoCatasto As String
    Public Property EspandiNodoCatasto() As String
        Get
            Return _Hash_Chiavi(LCase("EspandiNodoCatasto"))
        End Get
        Set(ByVal value As String)

            If _Hash_Chiavi.ContainsKey(LCase("EspandiNodoCatasto")) Then
                _Hash_Chiavi(LCase("EspandiNodoCatasto")) = value
            Else
                _Hash_Chiavi.Add(LCase("EspandiNodoCatasto"), value)
            End If
        End Set
    End Property
    ' Private _ColoreNodoScadutoAlberoImprese As String
    Public Property ColoreNodoScadutoAlberoImprese() As String
        Get
            Return _Hash_Chiavi(LCase("ColoreNodoScadutoAlberoImprese"))
        End Get
        Set(ByVal value As String)

            If _Hash_Chiavi.ContainsKey(LCase("ColoreNodoScadutoAlberoImprese")) Then
                _Hash_Chiavi(LCase("ColoreNodoScadutoAlberoImprese")) = value
            Else
                _Hash_Chiavi.Add(LCase("ColoreNodoScadutoAlberoImprese"), value)
            End If
        End Set
    End Property
    'Private _ColoreNodoBloccatoAlberoImprese As String
    Public Property ColoreNodoBloccatoAlberoImprese() As String
        Get
            Return _Hash_Chiavi(LCase("ColoreNodoBloccatoAlberoImprese"))
        End Get
        Set(ByVal value As String)

            If _Hash_Chiavi.ContainsKey(LCase("ColoreNodoBloccatoAlberoImprese")) Then
                _Hash_Chiavi(LCase("ColoreNodoBloccatoAlberoImprese")) = value
            Else
                _Hash_Chiavi.Add(LCase("ColoreNodoBloccatoAlberoImprese"), value)
            End If
        End Set
    End Property
    'Private _DettagliAppezzamentiAlberoImprese As String
    Public Property DettagliAppezzamentiAlberoImprese() As String
        Get
            Return _Hash_Chiavi(LCase("DettagliAppezzamentiAlberoImprese"))
        End Get
        Set(ByVal value As String)

            If _Hash_Chiavi.ContainsKey(LCase("DettagliAppezzamentiAlberoImprese")) Then
                _Hash_Chiavi(LCase("DettagliAppezzamentiAlberoImprese")) = value
            Else
                _Hash_Chiavi.Add(LCase("DettagliAppezzamentiAlberoImprese"), value)
            End If
        End Set
    End Property
    'Private _DettagliImpiantiAlberoImprese As String
    Public Property DettagliImpiantiAlberoImprese() As String
        Get
            Return _Hash_Chiavi(LCase("DettagliImpiantiAlberoImprese"))
        End Get
        Set(ByVal value As String)

            If _Hash_Chiavi.ContainsKey(LCase("DettagliImpiantiAlberoImprese")) Then
                _Hash_Chiavi(LCase("DettagliImpiantiAlberoImprese")) = value
            Else
                _Hash_Chiavi.Add(LCase("DettagliImpiantiAlberoImprese"), value)
            End If
        End Set
    End Property
    'Private _IconaCentroConCartografiaAlberoImprese As String
    Public Property IconaCentroConCartografiaAlberoImprese() As String
        Get
            Return _Hash_Chiavi(LCase("IconaCentroConCartografiaAlberoImprese"))
        End Get
        Set(ByVal value As String)

            If _Hash_Chiavi.ContainsKey(LCase("IconaCentroConCartografiaAlberoImprese")) Then
                _Hash_Chiavi(LCase("IconaCentroConCartografiaAlberoImprese")) = value
            Else
                _Hash_Chiavi.Add(LCase("IconaCentroConCartografiaAlberoImprese"), value)
            End If
        End Set
    End Property
    'Private _AgendaVisualizzazioneDefault As String
    Public Property AgendaVisualizzazioneDefault() As String
        Get
            Return _Hash_Chiavi(LCase("AgendaVisualizzazioneDefault"))
        End Get
        Set(ByVal value As String)

            If _Hash_Chiavi.ContainsKey(LCase("AgendaVisualizzazioneDefault")) Then
                _Hash_Chiavi(LCase("AgendaVisualizzazioneDefault")) = value
            Else
                _Hash_Chiavi.Add(LCase("AgendaVisualizzazioneDefault"), value)
            End If
        End Set
    End Property
    'Private _AttivaStampePersonalizzate As String
    Public Property AttivaStampePersonalizzate() As String
        Get
            Return _Hash_Chiavi(LCase("AttivaStampePersonalizzate"))
        End Get
        Set(ByVal value As String)

            If _Hash_Chiavi.ContainsKey(LCase("AttivaStampePersonalizzate")) Then
                _Hash_Chiavi(LCase("AttivaStampePersonalizzate")) = value
            Else
                _Hash_Chiavi.Add(LCase("AttivaStampePersonalizzate"), value)
            End If
        End Set
    End Property
    'Private _Prefix4SP As String
    Public Property Prefix4SP() As String
        Get
            Return _Hash_Chiavi(LCase("Prefix4SP"))
        End Get
        Set(ByVal value As String)

            If _Hash_Chiavi.ContainsKey(LCase("Prefix4SP")) Then
                _Hash_Chiavi(LCase("Prefix4SP")) = value
            Else
                _Hash_Chiavi.Add(LCase("Prefix4SP"), value)
            End If
        End Set
    End Property
    'Private _SitoRichiesto As String
    Public Property SitoRichiesto() As String
        Get
            Return _Hash_Chiavi(LCase("SitoRichiesto"))
        End Get
        Set(ByVal value As String)

            If _Hash_Chiavi.ContainsKey(LCase("SitoRichiesto")) Then
                _Hash_Chiavi(LCase("SitoRichiesto")) = value
            Else
                _Hash_Chiavi.Add(LCase("SitoRichiesto"), value)
            End If
        End Set
    End Property
    'Private _Cartografia_NomeMappaBase As String
    Public Property Cartografia_NomeMappaBase() As String
        Get
            Return _Hash_Chiavi(LCase("Cartografia_NomeMappaBase"))
        End Get
        Set(ByVal value As String)

            If _Hash_Chiavi.ContainsKey(LCase("Cartografia_NomeMappaBase")) Then
                _Hash_Chiavi(LCase("Cartografia_NomeMappaBase")) = value
            Else
                _Hash_Chiavi.Add(LCase("Cartografia_NomeMappaBase"), value)
            End If
        End Set
    End Property
    'Private _Cartografia_SoloMappeUtente As String
    Public Property Cartografia_SoloMappeUtente() As String
        Get
            Return _Hash_Chiavi(LCase("Cartografia_SoloMappeUtente"))
        End Get
        Set(ByVal value As String)

            If _Hash_Chiavi.ContainsKey(LCase("Cartografia_SoloMappeUtente")) Then
                _Hash_Chiavi(LCase("Cartografia_SoloMappeUtente")) = value
            Else
                _Hash_Chiavi.Add(LCase("Cartografia_SoloMappeUtente"), value)
            End If
        End Set
    End Property
    ' Private _Cartografia_LarghezzaMappaMillimetri As String
    Public Property Cartografia_LarghezzaMappaMillimetri() As String
        Get
            Return _Hash_Chiavi(LCase("Cartografia_LarghezzaMappaMillimetri"))
        End Get
        Set(ByVal value As String)

            If _Hash_Chiavi.ContainsKey(LCase("Cartografia_LarghezzaMappaMillimetri")) Then
                _Hash_Chiavi(LCase("Cartografia_LarghezzaMappaMillimetri")) = value
            Else
                _Hash_Chiavi.Add(LCase("Cartografia_LarghezzaMappaMillimetri"), value)
            End If
        End Set
    End Property
    'Private _GiasOnline_WS_Mappe_Gias_Service As String
    Public Property GiasOnline_WS_Mappe_Gias_Service() As String
        Get
            Return _Hash_Chiavi(LCase("GiasOnline_WS_Mappe_Gias_Service"))
        End Get
        Set(ByVal value As String)

            If _Hash_Chiavi.ContainsKey(LCase("GiasOnline_WS_Mappe_Gias_Service")) Then
                _Hash_Chiavi(LCase("GiasOnline_WS_Mappe_Gias_Service")) = value
            Else
                _Hash_Chiavi.Add(LCase("GiasOnline_WS_Mappe_Gias_Service"), value)
            End If
        End Set
    End Property
    'Private _WebServiceMappe_RepositoryGIS As String
    Public Property WebServiceMappe_RepositoryGIS() As String
        Get
            Return _Hash_Chiavi(LCase("WebServiceMappe_RepositoryGIS"))
        End Get
        Set(ByVal value As String)

            If _Hash_Chiavi.ContainsKey(LCase("WebServiceMappe_RepositoryGIS")) Then
                _Hash_Chiavi(LCase("WebServiceMappe_RepositoryGIS")) = value
            Else
                _Hash_Chiavi.Add(LCase("WebServiceMappe_RepositoryGIS"), value)
            End If
        End Set
    End Property
    'Private _WebService_GiasAlarm As String
    Public Property WebService_GiasAlarm() As String
        Get
            Return _Hash_Chiavi(LCase("WebService_GiasAlarm"))
        End Get
        Set(ByVal value As String)

            If _Hash_Chiavi.ContainsKey(LCase("WebService_GiasAlarm")) Then
                _Hash_Chiavi(LCase("WebService_GiasAlarm")) = value
            Else
                _Hash_Chiavi.Add(LCase("WebService_GiasAlarm"), value)
            End If
        End Set
    End Property

    'Private _Fuso As String
    Public Property Fuso() As String
        Get
            Return _Hash_Chiavi(LCase("Fuso"))
        End Get
        Set(ByVal value As String)

            If _Hash_Chiavi.ContainsKey(LCase("Fuso")) Then
                _Hash_Chiavi(LCase("Fuso")) = value
            Else
                _Hash_Chiavi.Add(LCase("Fuso"), value)
            End If
        End Set
    End Property
    'Private _Ritaglio_Servizio As String
    Public Property Ritaglio_Servizio() As String
        Get
            Return _Hash_Chiavi(LCase("Ritaglio_Servizio"))
        End Get
        Set(ByVal value As String)

            If _Hash_Chiavi.ContainsKey(LCase("Ritaglio_Servizio")) Then
                _Hash_Chiavi(LCase("Ritaglio_Servizio")) = value
            Else
                _Hash_Chiavi.Add(LCase("Ritaglio_Servizio"), value)
            End If
        End Set
    End Property
    'Private _Ritaglio_Sito As String
    Public Property Ritaglio_Sito() As String
        Get
            Return _Hash_Chiavi(LCase("Ritaglio_Sito"))
        End Get
        Set(ByVal value As String)

            If _Hash_Chiavi.ContainsKey(LCase("Ritaglio_Sito")) Then
                _Hash_Chiavi(LCase("Ritaglio_Sito")) = value
            Else
                _Hash_Chiavi.Add(LCase("Ritaglio_Sito"), value)
            End If
        End Set
    End Property
    'Private _VersioneCodifica As String
    Public Property VersioneCodifica() As String
        Get
            Return _Hash_Chiavi(LCase("VersioneCodifica"))
        End Get
        Set(ByVal value As String)

            If _Hash_Chiavi.ContainsKey(LCase("VersioneCodifica")) Then
                _Hash_Chiavi(LCase("VersioneCodifica")) = value
            Else
                _Hash_Chiavi.Add(LCase("VersioneCodifica"), value)
            End If
        End Set
    End Property
    'Private _Versione_Pwd As String
    Public Property Versione_Pwd() As String
        Get
            Return _Hash_Chiavi(LCase("Versione_Pwd"))
        End Get
        Set(ByVal value As String)

            If _Hash_Chiavi.ContainsKey(LCase("Versione_Pwd")) Then
                _Hash_Chiavi(LCase("Versione_Pwd")) = value
            Else
                _Hash_Chiavi.Add(LCase("Versione_Pwd"), value)
            End If
        End Set
    End Property
    'Private _StarterKit_Pwd As String
    Public Property StarterKit_Pwd() As String
        Get
            Return _Hash_Chiavi(LCase("StarterKit_Pwd"))
        End Get
        Set(ByVal value As String)

            If _Hash_Chiavi.ContainsKey(LCase("StarterKit_Pwd")) Then
                _Hash_Chiavi(LCase("StarterKit_Pwd")) = value
            Else
                _Hash_Chiavi.Add(LCase("StarterKit_Pwd"), value)
            End If
        End Set
    End Property
    'Private _Flag_Messaggio As String
    Public Property Flag_Messaggio() As String
        Get
            Return _Hash_Chiavi(LCase("Flag_Messaggio"))
        End Get
        Set(ByVal value As String)

            If _Hash_Chiavi.ContainsKey(LCase("Flag_Messaggio")) Then
                _Hash_Chiavi(LCase("Flag_Messaggio")) = value
            Else
                _Hash_Chiavi.Add(LCase("Flag_Messaggio"), value)
            End If
        End Set
    End Property
    ' Private _Tunnel_Pwd As String
    Public Property Tunnel_Pwd() As String
        Get
            Return _Hash_Chiavi(LCase("Tunnel_Pwd"))
        End Get
        Set(ByVal value As String)

            If _Hash_Chiavi.ContainsKey(LCase("Tunnel_Pwd")) Then
                _Hash_Chiavi(LCase("Tunnel_Pwd")) = value
            Else
                _Hash_Chiavi.Add(LCase("Tunnel_Pwd"), value)
            End If
        End Set
    End Property
    ' Private _AgronicaCore_Flag_CancellazioneLogica As String
    Public Property AgronicaCore_Flag_CancellazioneLogica() As String
        Get
            Return _Hash_Chiavi(LCase("AgronicaCore_Flag_CancellazioneLogica"))
        End Get
        Set(ByVal value As String)

            If _Hash_Chiavi.ContainsKey(LCase("AgronicaCore_Flag_CancellazioneLogica")) Then
                _Hash_Chiavi(LCase("AgronicaCore_Flag_CancellazioneLogica")) = value
            Else
                _Hash_Chiavi.Add(LCase("AgronicaCore_Flag_CancellazioneLogica"), value)
            End If
        End Set
    End Property
    ' Private _AgronicaCore_Flag_Visibilita As String
    Public Property AgronicaCore_Flag_Visibilita() As String
        Get
            Return _Hash_Chiavi(LCase("AgronicaCore_Flag_Visibilita"))
        End Get
        Set(ByVal value As String)

            If _Hash_Chiavi.ContainsKey(LCase("AgronicaCore_Flag_Visibilita")) Then
                _Hash_Chiavi(LCase("AgronicaCore_Flag_Visibilita")) = value
            Else
                _Hash_Chiavi.Add(LCase("AgronicaCore_Flag_Visibilita"), value)
            End If
        End Set
    End Property
    ' Private _AgronicaCore_DirectoryLOG As String
    Public Property AgronicaCore_DirectoryLOG() As String
        Get
            Return _Hash_Chiavi(LCase("AgronicaCore_DirectoryLOG"))
        End Get
        Set(ByVal value As String)

            If _Hash_Chiavi.ContainsKey(LCase("AgronicaCore_DirectoryLOG")) Then
                _Hash_Chiavi(LCase("AgronicaCore_DirectoryLOG")) = value
            Else
                _Hash_Chiavi.Add(LCase("AgronicaCore_DirectoryLOG"), value)
            End If
        End Set
    End Property

    'Private _AgronicaCore_FileNameLOG As String
    Public Property AgronicaCore_FileNameLOG() As String
        Get
            Return _Hash_Chiavi(LCase("AgronicaCore_FileNameLOG"))
        End Get
        Set(ByVal value As String)

            If _Hash_Chiavi.ContainsKey(LCase("AgronicaCore_FileNameLOG")) Then
                _Hash_Chiavi(LCase("AgronicaCore_FileNameLOG")) = value
            Else
                _Hash_Chiavi.Add(LCase("AgronicaCore_FileNameLOG"), value)
            End If
        End Set
    End Property

    'Private _PathDirectoryLOG As String
    Public Property PathDirectoryLOG() As String
        Get
            Return _Hash_Chiavi(LCase("PathDirectoryLOG"))
        End Get
        Set(ByVal value As String)

            If _Hash_Chiavi.ContainsKey(LCase("PathDirectoryLOG")) Then
                _Hash_Chiavi(LCase("PathDirectoryLOG")) = value
            Else
                _Hash_Chiavi.Add(LCase("PathDirectoryLOG"), value)
            End If
        End Set
    End Property



    'LinkAgronicaCheckCOOP
    Public Property LinkAgronicaCheckCOOP() As String
        Get
            Return _Hash_Chiavi(LCase("LinkAgronicaCheckCOOP"))
        End Get
        Set(ByVal value As String)

            If _Hash_Chiavi.ContainsKey(LCase("LinkAgronicaCheckCOOP")) Then
                _Hash_Chiavi(LCase("LinkAgronicaCheckCOOP")) = value
            Else
                _Hash_Chiavi.Add(LCase("LinkAgronicaCheckCOOP"), value)
            End If
        End Set
    End Property

    'Private _LinkAgronicaGlobalGap As String
    Public Property LinkAgronicaGlobalGap() As String
        Get
            Return _Hash_Chiavi(LCase("LinkAgronicaGlobalGap"))
        End Get
        Set(ByVal value As String)

            If _Hash_Chiavi.ContainsKey(LCase("LinkAgronicaGlobalGap")) Then
                _Hash_Chiavi(LCase("LinkAgronicaGlobalGap")) = value
            Else
                _Hash_Chiavi.Add(LCase("LinkAgronicaGlobalGap"), value)
            End If
        End Set
    End Property

    'Private _TabelleTemp_Mode As String
    Public Property TabelleTemp_Mode() As String
        Get
            Return _Hash_Chiavi(LCase("TabelleTemp_Mode"))
        End Get
        Set(ByVal value As String)

            If _Hash_Chiavi.ContainsKey(LCase("TabelleTemp_Mode")) Then
                _Hash_Chiavi(LCase("TabelleTemp_Mode")) = value
            Else
                _Hash_Chiavi.Add(LCase("TabelleTemp_Mode"), value)
            End If
        End Set
    End Property

    'Private _Str_TabelleTemp_RegolaConfronto As String
    Public Property Str_TabelleTemp_RegolaConfronto() As String
        Get
            Return _Hash_Chiavi(LCase("Str_TabelleTemp_RegolaConfronto"))
        End Get
        Set(ByVal value As String)

            If _Hash_Chiavi.ContainsKey(LCase("Str_TabelleTemp_RegolaConfronto")) Then
                _Hash_Chiavi(LCase("Str_TabelleTemp_RegolaConfronto")) = value
            Else
                _Hash_Chiavi.Add(LCase("Str_TabelleTemp_RegolaConfronto"), value)
            End If
        End Set
    End Property

    'Private _MailAssistenza As String
    Public Property MailAssistenza() As String
        Get
            Return _Hash_Chiavi(LCase("MailAssistenza"))
        End Get
        Set(ByVal value As String)

            If _Hash_Chiavi.ContainsKey(LCase("MailAssistenza")) Then
                _Hash_Chiavi(LCase("MailAssistenza")) = value
            Else
                _Hash_Chiavi.Add(LCase("MailAssistenza"), value)
            End If
        End Set
    End Property

    'Private _MailBancheDati As String
    Public Property MailBancheDati() As String
        Get
            Return _Hash_Chiavi(LCase("MailBancheDati"))
        End Get
        Set(ByVal value As String)

            If _Hash_Chiavi.ContainsKey(LCase("MailBancheDati")) Then
                _Hash_Chiavi(LCase("MailBancheDati")) = value
            Else
                _Hash_Chiavi.Add(LCase("MailBancheDati"), value)
            End If
        End Set
    End Property


    Public Property LinkGiasOnline_Root() As String
        Get
            Return _Hash_Chiavi(LCase("LinkGiasOnline_Root"))
        End Get
        Set(ByVal value As String)

            If _Hash_Chiavi.ContainsKey(LCase("LinkGiasOnline_Root")) Then
                _Hash_Chiavi(LCase("LinkGiasOnline_Root")) = value
            Else
                _Hash_Chiavi.Add(LCase("LinkGiasOnline_Root"), value)
            End If
        End Set
    End Property


    Public Property LinkAgronicaGiasNG() As String
        Get
            Return _Hash_Chiavi(LCase("LinkAgronicaGiasNG"))
        End Get
        Set(ByVal value As String)

            If _Hash_Chiavi.ContainsKey(LCase("LinkAgronicaGiasNG")) Then
                _Hash_Chiavi(LCase("LinkAgronicaGiasNG")) = value
            Else
                _Hash_Chiavi.Add(LCase("LinkAgronicaGiasNG"), value)
            End If
        End Set
    End Property

    Public Property LinkAgronicaUMA() As String
        Get
            Return _Hash_Chiavi(LCase("LinkAgronicaUMA"))
        End Get
        Set(ByVal value As String)

            If _Hash_Chiavi.ContainsKey(LCase("LinkAgronicaUMA")) Then
                _Hash_Chiavi(LCase("LinkAgronicaUMA")) = value
            Else
                _Hash_Chiavi.Add(LCase("LinkAgronicaUMA"), value)
            End If
        End Set
    End Property

    Public Property LinkAgronicaDomandaIrrigua() As String
        Get
            Return _Hash_Chiavi(LCase("LinkAgronicaDomandaIrrigua"))
        End Get
        Set(ByVal value As String)

            If _Hash_Chiavi.ContainsKey(LCase("LinkAgronicaDomandaIrrigua")) Then
                _Hash_Chiavi(LCase("LinkAgronicaDomandaIrrigua")) = value
            Else
                _Hash_Chiavi.Add(LCase("LinkAgronicaDomandaIrrigua"), value)
            End If
        End Set
    End Property


    ''''''''''''''x ANALISI 2010''''''''''''''
    'Private _MailBancheDati As String
    Public Property Nome_Codice_Griglia() As String
        Get
            Return _Hash_Chiavi(LCase("Nome_Codice_Griglia"))
        End Get
        Set(ByVal value As String)
            If _Hash_Chiavi.ContainsKey(LCase("Nome_Codice_Griglia")) Then
                _Hash_Chiavi(LCase("Nome_Codice_Griglia")) = value
            Else
                _Hash_Chiavi.Add(LCase("Nome_Codice_Griglia"), value)
            End If
        End Set
    End Property

    'ClientSMTP
    Public Property ClientSMTP() As String
        Get
            Return _Hash_Chiavi(LCase("ClientSMTP"))
        End Get
        Set(ByVal value As String)
            If _Hash_Chiavi.ContainsKey(LCase("ClientSMTP")) Then
                _Hash_Chiavi(LCase("ClientSMTP")) = value
            Else
                _Hash_Chiavi.Add(LCase("ClientSMTP"), value)
            End If
        End Set
    End Property

    ''''''''''''''x Stampe ''''''''''''''
    Public Property PathFileTemporanei() As String
        Get
            Return _Hash_Chiavi(LCase("PathFileTemporanei"))
        End Get
        Set(ByVal value As String)
            If _Hash_Chiavi.ContainsKey(LCase("PathFileTemporanei")) Then
                _Hash_Chiavi(LCase("PathFileTemporanei")) = value
            Else
                _Hash_Chiavi.Add(LCase("PathFileTemporanei"), value)
            End If
        End Set
    End Property

    Public Property NomeStampante() As String
        Get
            Return _Hash_Chiavi(LCase("NomeStampante"))
        End Get
        Set(ByVal value As String)
            If _Hash_Chiavi.ContainsKey(LCase("NomeStampante")) Then
                _Hash_Chiavi(LCase("NomeStampante")) = value
            Else
                _Hash_Chiavi.Add(LCase("NomeStampante"), value)
            End If
        End Set
    End Property

    ' web.config AgronicaStampe
    Public Property Mode_AgroWinSrvc_PrintUtility_PtP() As String
        Get
            Return _Hash_Chiavi(LCase("Mode_AgroWinSrvc_PrintUtility_PtP"))
        End Get
        Set(ByVal value As String)
            If _Hash_Chiavi.ContainsKey(LCase("Mode_AgroWinSrvc_PrintUtility_PtP")) Then
                _Hash_Chiavi(LCase("Mode_AgroWinSrvc_PrintUtility_PtP")) = value
            Else
                _Hash_Chiavi.Add(LCase("Mode_AgroWinSrvc_PrintUtility_PtP"), value)
            End If
        End Set
    End Property

    ' web.config AgronicaStampe
    Public Property PathDati_AgroWinSrvc_PrintUtility() As String
        Get
            Return _Hash_Chiavi(LCase("PathDati_AgroWinSrvc_PrintUtility"))
        End Get
        Set(ByVal value As String)
            If _Hash_Chiavi.ContainsKey(LCase("PathDati_AgroWinSrvc_PrintUtility")) Then
                _Hash_Chiavi(LCase("PathDati_AgroWinSrvc_PrintUtility")) = value
            Else
                _Hash_Chiavi.Add(LCase("PathDati_AgroWinSrvc_PrintUtility"), value)
            End If
        End Set
    End Property

    ' web.config AgronicaStampe
    Public Property PathComandi_AgroWinSrvc_PrintUtility() As String
        Get
            Return _Hash_Chiavi(LCase("PathComandi_AgroWinSrvc_PrintUtility"))
        End Get
        Set(ByVal value As String)
            If _Hash_Chiavi.ContainsKey(LCase("PathComandi_AgroWinSrvc_PrintUtility")) Then
                _Hash_Chiavi(LCase("PathComandi_AgroWinSrvc_PrintUtility")) = value
            Else
                _Hash_Chiavi.Add(LCase("PathComandi_AgroWinSrvc_PrintUtility"), value)
            End If
        End Set
    End Property

    ' web.config AgronicaStampe
    Public Property Lista_Stampanti() As String
        Get
            Return _Hash_Chiavi(LCase("Lista_Stampanti"))
        End Get
        Set(ByVal value As String)
            If _Hash_Chiavi.ContainsKey(LCase("Lista_Stampanti")) Then
                _Hash_Chiavi(LCase("Lista_Stampanti")) = value
            Else
                _Hash_Chiavi.Add(LCase("Lista_Stampanti"), value)
            End If
        End Set
    End Property

    ' web.config FiltroneBootstrap
    Public Property FiltroneBootstrap() As String
        Get
            Return _Hash_Chiavi(LCase("FiltroneBootstrap"))
        End Get
        Set(ByVal value As String)
            If _Hash_Chiavi.ContainsKey(LCase("FiltroneBootstrap")) Then
                _Hash_Chiavi(LCase("FiltroneBootstrap")) = value
            Else
                _Hash_Chiavi.Add(LCase("FiltroneBootstrap"), value)
            End If
        End Set
    End Property

    ' web.config FiltroGruppiUtente
    Public Property FiltroGruppiUtente() As String
        Get
            Return _Hash_Chiavi(LCase("FiltroGruppiUtente"))
        End Get
        Set(ByVal value As String)
            If _Hash_Chiavi.ContainsKey(LCase("FiltroGruppiUtente")) Then
                _Hash_Chiavi(LCase("FiltroGruppiUtente")) = value
            Else
                _Hash_Chiavi.Add(LCase("FiltroGruppiUtente"), value)
            End If
        End Set
    End Property


    '  Galassi, 23/06/2016 11:58:47: Aggiunta chiave per mappe piogge
    Public Property ConnessioneMaps_Server() As String
        Get
            Return _Hash_Chiavi(LCase("ConnessioneMaps_Server"))
        End Get
        Set(ByVal value As String)
            If _Hash_Chiavi.ContainsKey(LCase("ConnessioneMaps_Server")) Then
                _Hash_Chiavi(LCase("ConnessioneMaps_Server")) = value
            Else
                _Hash_Chiavi.Add(LCase("ConnessioneMaps_Server"), value)
            End If
        End Set
    End Property


    '  Galassi, 23/06/2016 11:59:19: Aggiunta chiave per mappe piogge
    Public Property deltaE() As String
        Get
            Return _Hash_Chiavi(LCase("deltaE"))
        End Get
        Set(ByVal value As String)
            If _Hash_Chiavi.ContainsKey(LCase("deltaE")) Then
                _Hash_Chiavi(LCase("deltaE")) = value
            Else
                _Hash_Chiavi.Add(LCase("deltaE"), value)
            End If
        End Set
    End Property


    '  Galassi, 23/06/2016 11:59:19: Aggiunta chiave per mappe piogge
    Public Property deltaN() As String
        Get
            Return _Hash_Chiavi(LCase("deltaN"))
        End Get
        Set(ByVal value As String)
            If _Hash_Chiavi.ContainsKey(LCase("deltaN")) Then
                _Hash_Chiavi(LCase("deltaN")) = value
            Else
                _Hash_Chiavi.Add(LCase("deltaN"), value)
            End If
        End Set
    End Property

    Public Property LinkGiasBase() As String
        Get
            Return _Hash_Chiavi(LCase("LinkGiasBase"))
        End Get
        Set(ByVal value As String)
            If _Hash_Chiavi.ContainsKey(LCase("LinkGiasBase")) Then
                _Hash_Chiavi(LCase("LinkGiasBase")) = value
            Else
                _Hash_Chiavi.Add(LCase("LinkGiasBase"), value)
            End If
        End Set
    End Property


#End Region


#End Region

    'lo faccio Friend perchè solo questo progetto deve gestirlo
    Friend Sub New(ByVal WebConfig As System.Collections.Specialized.NameValueCollection)
        _Sessione = True

        Inizializza_Array_Key()
        _Hash_Chiavi = New Hashtable

        'leggo tutte le impostazioni

        Dim i As Integer
        For i = 0 To array_key.Length - 1
            If WebConfig(array_key(i)) <> "" Then
                _Hash_Chiavi.Add(array_key(i), WebConfig(array_key(i)))
            End If
        Next

        '=======================================
        '=======================================
        Salva()
        '=======================================
        '=======================================


    End Sub


    'lo faccio Friend perchè solo l'inizializzatore deve gestirlo
    'SAREBBE DA ELIMINARE
    Friend Sub New(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, ByVal no As Boolean)
        _Sessione = True

        Dim DT As DataTable
        Dim objConf As New AgronicaCoreVarieDAL.Configurazione_Siti_R
        DT = objConf.Leggi(0, "", "", "", objParametri)
        Inizializza_Array_Key()
        Me._Hash_Chiavi = New Hashtable

        Dim i As Integer
        For i = 0 To DT.Rows.Count - 1
            Me._Hash_Chiavi.Add(LCase(DT.Rows(i).Item("Chiave")), DT.Rows(i).Item("Valore"))
        Next

        impostaConnessioneOK(Nothing)
        '=======================================
        '=======================================
        Salva()
        '=======================================
        '=======================================


    End Sub


    'QUESTO é QUELLO DA UTILIZZARE NELL'INIZIALIZZATORE
    Public Sub New(ByRef objParametri_SuperServer As AgronicaCoreParametri,
                    ByRef objParametri_Server As AgronicaCoreParametri,
                    ByVal no As Boolean)
        _Sessione = True
        impostaDaDB(objParametri_SuperServer, objParametri_Server)
    End Sub

    Public Sub impostaDaDB(ByRef objParametri_SuperServer As AgronicaCoreParametri,
                           ByRef objParametri_Server As AgronicaCoreParametri)

        Dim Configurazione_Siti_SUPERSERVER As DataTable
        Dim Configurazione_Siti_GIAS_SERVER As DataTable
        Dim objConf As New AgronicaCoreVarieDAL.Configurazione_Siti_R
        Configurazione_Siti_SUPERSERVER = objConf.Leggi(0, "", "", "", objParametri_SuperServer)
        Configurazione_Siti_GIAS_SERVER = objConf.Leggi(0, "", "", "", objParametri_Server)
        Inizializza_Array_Key()
        Me._Hash_Chiavi = New Hashtable

        Dim i As Integer
        For i = 0 To Configurazione_Siti_SUPERSERVER.Rows.Count - 1
            Me._Hash_Chiavi.Add(LCase(Configurazione_Siti_SUPERSERVER.Rows(i).Item("Chiave")), Configurazione_Siti_SUPERSERVER.Rows(i).Item("Valore"))
        Next

        For i = 0 To Configurazione_Siti_GIAS_SERVER.Rows.Count - 1
            ' Chiave LinkGiasBase tiene sempre quella del superserver
            Dim chiave As String = LCase(Configurazione_Siti_GIAS_SERVER.Rows(i).Item("Chiave"))

            If Not chiave.Equals("linkgiasbase") Then
                If Me._Hash_Chiavi.Contains(chiave) Then
                    Me._Hash_Chiavi.Remove(chiave)
                End If
                Me._Hash_Chiavi.Add(chiave, Configurazione_Siti_GIAS_SERVER.Rows(i).Item("Valore"))
            End If

        Next



        impostaConnessioneOK(objParametri_Server)
        '=======================================
        '=======================================
        Salva()
        '=======================================
        '=======================================
    End Sub


    'QUESTO RILEGGE DA SESSIONE
    'lo faccio PUBBLICO perchè è usato nei vari siti
    Public Sub New()
        _Sessione = True
        _Hash_Chiavi = New Hashtable
        'Inizializza_Array_Key()
        Carica()
        impostaConnessioneOK(Nothing)
    End Sub

    Public Sub New(ByVal flagSessione As Boolean,
                   ByVal objParametri_Server As AgronicaCoreParametri,
                   ByVal objParametri_SuperServer As AgronicaCoreParametri)
        _Sessione = flagSessione
        _Hash_Chiavi = New Hashtable
        'Inizializza_Array_Key()
        Carica(flagSessione,
               objParametri_Server,
               objParametri_SuperServer)
    End Sub


    Private Sub impostaConnessioneOK(objParametri_Server)
        If HttpContext.Current.Session IsNot Nothing Then
            If Not IsNothing(HttpContext.Current.Session("WS_DPI")) Then
                'imposto le connessioni al webservice
                GiasOnline_WS_Disciplinari_AgroWS_Disciplinari = HttpContext.Current.Session("WS_DPI")
                GiasOnline_WS_Fitofarmaci_AgroWS_Fitofarmaci = HttpContext.Current.Session("WS_FITO")
                GiasOnline_WS_Meteo_Meteo = HttpContext.Current.Session("WS_METEO")
                GiasOnline_WS_CapitolatoCliente_AgroWS_CapitolatoCliente = HttpContext.Current.Session("WS_CAPITOLATO")
            End If
        Else
            'Scatto - 17/5/2018 - Se ci arrivo senza session rileggo da db
            If objParametri_Server IsNot Nothing Then
                Dim xLeggiConfigurazioneSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
                GiasOnline_WS_Disciplinari_AgroWS_Disciplinari = xLeggiConfigurazioneSiti.Leggi_Valore(0, "GiasOnline_WS_Disciplinari_AgroWS_Disciplinari", "", "", objParametri_Server)
                GiasOnline_WS_Fitofarmaci_AgroWS_Fitofarmaci = xLeggiConfigurazioneSiti.Leggi_Valore(0, "GiasOnline_WS_Fitofarmaci_AgroWS_Fitofarmaci", "", "", objParametri_Server)
                'TODO GiasOnline_WS_Meteo_Meteo =  
                'TODO GiasOnline_WS_CapitolatoCliente_AgroWS_CapitolatoCliente =  
            End If
        End If
    End Sub


    'QUESTO NON SI DEVE UTILIZZARE; SOLO PER IL DEBUG
    Friend Sub New(ByVal XML As String, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri)
        _Sessione = True
        _Hash_Chiavi = New Hashtable
        Inizializza_Array_Key()
        LeggiXML(XML, objParametri)
        Salva()
    End Sub


    Private Sub Salva()
        If HttpContext.Current.Session IsNot Nothing Then

            If Not IsNothing(Me._Hash_Chiavi) AndAlso Me._Hash_Chiavi.ContainsKey(LCase("LinkGiasBase")) Then
                Dim linkGiasBase = Me._Hash_Chiavi(LCase("LinkGiasBase"))
                GiasBaseHelper.Formatta_Link_GiasBase(linkGiasBase)
                Me._Hash_Chiavi(LCase("LinkGiasBase")) = linkGiasBase
            End If

            HttpContext.Current.Session("AgroWebConfig") = Me

        End If
    End Sub



    Private Sub Inizializza_Array_Key()
        array_key = New String() {
        LCase("StarGate_CodicePredefinito"),
        LCase("StarGate_Connessione"),
        LCase("StarGate_ConsentiGateBypass"),
        LCase("StarGate_LinkBypassBloccato"),
        LCase("StarGate_Versione"),
        LCase("StarGate_PathFileINI"),
        LCase("StarGate_Whynot"),
        LCase("GestioneAllegati_Repository"),
        LCase("GestioneCartografia_Repository"),
        LCase("GestioneImportazioni_Repository"),
        LCase("LinkGiasOnline"),
        LCase("LinkManualeGiasOnline"),
        LCase("LinkAgronicaStampe"),
        LCase("LinkAgronicaStampe_2010"),
        LCase("LinkPianoConcimazione"),
        LCase("LinkPianoConcimazione_2017"),
        LCase("LinkAgronicaBio"),
        LCase("LinkAgronicaView"),
        LCase("LinkAgronicaAnalisi"),
        LCase("LinkAgronicaAnalisi_2010"),
        LCase("LinkAgronicaAudit"),
        LCase("LinkAgronicaGiasNG"),
        LCase("LinkAgronicaUMA"),
        LCase("LinkAgronicaDomandaIrrigua"),
        LCase("LinkAgronicaSicurezzaLavoro"),
        LCase("LinkAgronicaPua"),
        LCase("LinkAgronicaPlanning"),
        LCase("LinkAgronicaProfilazione"),
        LCase("LinkAgronicaManutenzione"),
        LCase("LinkAgronicaPianiCampionamento"),
        LCase("LinkAgronicaAgenda2010"),
        LCase("LinkAgronicaGlobalGap"),
        LCase("LinkAgronicaCheckCOOP"),
        LCase("LinkAgronicaSincronizzatore"),
        LCase("Agenda2010"),
        LCase("Analisi2010"),
        LCase("Stampe2010"),
        LCase("enumStampe2010"),
        LCase("LinkProfitosan"),
        LCase("LinkCartografiaMappe"),
        LCase("DescrizioneServer"),
        LCase("Connessione_ONLINE_DPI"),
        LCase("Connessione_ONLINE_Server"),
        LCase("Connessione_ONLINE_Utenti"),
        LCase("LinkVisualizzatoreDPI"),
        LCase("GiasOnline_WS_Disciplinari_AgroWS_Disciplinari"),
        LCase("GiasOnline_WS_Disciplinari_AgroWS_Disciplinari_2"),
        LCase("GiasOnline_WS_Fitofarmaci_AgroWS_Fitofarmaci"),
        LCase("GiasOnline_WS_Fitofarmaci_AgroWS_Fitofarmaci_2"),
        LCase("GiasOnline_WS_CapitolatoCliente_AgroWS_CapitolatoCliente"),
        LCase("GiasOnline_WS_CapitolatoCliente_AgroWS_CapitolatoCliente_2"),
        LCase("GiasOnline_WS_Meteo_Meteo"),
        LCase("GiasOnline_WS_Meteo_Meteo_2"),
        LCase("Flag_WS_Fitofarmaci_Remoto"),
        LCase("Flag_DisciplinareAttivo"),
        LCase("Flag_DisciplinarePrivato"),
        LCase("Flag_SoglieAttive"),
        LCase("Flag_GestioneAnalisiAttivo"),
        LCase("Flag_Mirror"),
        LCase("LivelloEspansoAlberoImprese"),
        LCase("EspandiNodoCatasto"),
        LCase("ColoreNodoBloccatoAlberoImprese"),
        LCase("ColoreNodoScadutoAlberoImprese"),
        LCase("DettagliAppezzamentiAlberoImprese"),
        LCase("DettagliImpiantiAlberoImprese"),
        LCase("IconaCentroConCartografiaAlberoImprese"),
        LCase("AgendaVisualizzazioneDefault"),
        LCase("AttivaStampePersonalizzate"),
        LCase("Prefix4SP"),
        LCase("SitoRichiesto"),
        LCase("Cartografia_NomeMappaBase"),
        LCase("Cartografia_SoloMappeUtente"),
        LCase("Cartografia_LarghezzaMappaMillimetri"),
        LCase("GiasOnline_WS_Mappe_Gias_Service"),
        LCase("WebServiceMappe_RepositoryGIS"),
        LCase("Ritaglio_Servizio"),
        LCase("Fuso"),
        LCase("Ritaglio_Sito"),
        LCase("VersioneCodifica"),
        LCase("Versione_Pwd"),
        LCase("StarterKit_Pwd"),
        LCase("Flag_Messaggio"),
        LCase("Tunnel_Pwd"),
        LCase("AgronicaCore_Flag_CancellazioneLogica"),
        LCase("AgronicaCore_Flag_Visibilita"),
        LCase("AgronicaCore_DirectoryLOG"),
        LCase("AgronicaCore_FileNameLOG"),
        LCase("PathDirectoryLOG"),
        LCase("TabelleTemp_Mode"),
        LCase("Str_TabelleTemp_RegolaConfronto"),
        LCase("MailAssistenza"),
        LCase("MailBancheDati"),
        LCase("Nome_Codice_Griglia"),
        LCase("LinkAgronicaPianiSemina"),
        LCase("LinkAgronicaMeteo"),
        LCase("ClientSMTP"),
        LCase("PathFileTemporanei"),
        LCase("NomeStampante"),
        LCase("LinkGiasOnline_2010"),
        LCase("Codici_Analisi_PDC_x_Fruttagel"),
        LCase("Path_Directory_Loghi_Cliente"),
        LCase("WebService_GiasAlarm"),
        LCase("Mode_AgroWinSrvc_PrintUtility_PtP"),
        LCase("PathDati_AgroWinSrvc_PrintUtility"),
        LCase("PathComandi_AgroWinSrvc_PrintUtility"),
        LCase("Lista_Stampanti"),
        LCase("GestioneEsportazioni_Repository"),
        LCase("LinkSementieri"),
        LCase("user_smtp"),
        LCase("password_smtp"),
        LCase("LinkWS_Importa_GIAS"),
        LCase("googlemaps"),
        LCase("googlemaps_jsapi"),
        LCase("LinkGiasOnline_Root"),
        LCase("LinkProfitosan_WS"),
        LCase("LinkProfitosan_2023"),
        LCase("FiltroneBootstrap"),
        LCase("FiltroGruppiUtente"),
        LCase("ConnessioneMaps_Server"),
        LCase("deltaE"),
        LCase("deltaN"),
        LCase("GiasOnline_WS_Fertilizzanti_IAgroAPI_Fertilizzanti"),
        LCase("GiasOnline_WS_Fertilizzanti_IAgroAPI_Fertilizzanti_2"),
        LCase("GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione"),
        LCase("GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione_2"),
        LCase("GiasOnline_WS_SpecieVegetali_IAgroAPI_SpecieVegetali"),
        LCase("GiasOnline_WS_SpecieVegetali_IAgroAPI_SpecieVegetali_2"),
        LCase("LinkGiasBase")
       }



    End Sub

    ''' <summary>
    ''' Utilizzato quando l'oggetto è già in sessione
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub Carica()
        If _Sessione Then
            Dim obj As AgroWebConfig
            obj = CType(HttpContext.Current.Session?("AgroWebConfig"), AgroWebConfig)

            If Not IsNothing(obj) Then

                Dim key As Object
                For Each key In obj._Hash_Chiavi.Keys
                    _Hash_Chiavi.Add(key, obj._Hash_Chiavi(key))
                Next

            Else

                Dim objParametri_SuperServer = CType(HttpContext.Current.Session?("ASG_objParametri_Super_Server"), AgronicaCoreParametri)
                Dim objParametri_Server = CType(HttpContext.Current.Session?("ASG_objParametri_Server"), AgronicaCoreParametri)

                If objParametri_SuperServer IsNot Nothing AndAlso objParametri_Server IsNot Nothing Then
                    impostaDaDB(objParametri_SuperServer, objParametri_Server)
                End If

            End If
        End If
    End Sub

    Private Sub Carica(ByVal flagSessione As Boolean,
                       ByVal objParametri_Server As AgronicaCoreParametri,
                       ByVal objParametri_SuperServer As AgronicaCoreParametri)
        If Not flagSessione Then
            If objParametri_SuperServer IsNot Nothing AndAlso objParametri_Server IsNot Nothing Then
                impostaDaDB(objParametri_SuperServer, objParametri_Server)

            End If
        Else
            Carica()
        End If
    End Sub

    Private Sub LeggiXML(ByVal STR As String, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri)

        Dim XmlDoc As New System.Xml.XmlDocument
        XmlDoc.LoadXml(STR)

        Dim XmlTxt As System.Xml.XmlElement
        XmlTxt = XmlDoc.SelectSingleNode("Parametri").SelectSingleNode("AgroWebConfig")


        'leggo da DB tutte le impostazioni
        Dim objConf As New AgronicaCoreVarieDAL.Configurazione_Siti_R
        Dim Dt As DataTable

        Dt = objConf.Leggi(0, "", "", "", objParametri)

        Dim dr() As DataRow

        Dim i As Integer
        For i = 0 To array_key.Length - 1
            'per ogni nodo verifico se è nell'xml altrimenti lo leggo dal db
            If XmlTxt IsNot Nothing AndAlso XmlTxt.HasAttribute(array_key(i)) Then
                _Hash_Chiavi.Add(array_key(i), XmlTxt.Attributes(array_key(i)).Value)
            Else
                dr = Dt.Select("Chiave ='" & array_key(i) & "'")
                If dr.Length = 0 Then
                    'Throw New Exception("Manca la chiave: " & array_key(i) & " nel db Tabella Configurazione_Siti")
                Else
                    _Hash_Chiavi.Add(array_key(i), dr(0).Item("valore"))
                End If

            End If
        Next

        Salva()

    End Sub


    ''' <summary>
    ''' Genera il Nodo AgroWebConfig con dentro tutte gli attributi del webconfig
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GeneraXML() As String

        Inizializza_Array_Key()

        Dim XmlDoc As New System.Xml.XmlDocument
        Dim XmlTxt As System.Xml.XmlElement

        XmlTxt = XmlDoc.CreateElement("AgroWebConfig")

        Dim i As Integer
        For i = 0 To array_key.Length - 1
            If _Hash_Chiavi.ContainsKey(array_key(i)) Then
                XmlTxt.SetAttribute(LCase(array_key(i)), CStr(_Hash_Chiavi(array_key(i))))
            End If
        Next

        XmlDoc.AppendChild(XmlTxt)

        Dim str As String
        str = XmlDoc.InnerXml
        Return str

    End Function

End Class


