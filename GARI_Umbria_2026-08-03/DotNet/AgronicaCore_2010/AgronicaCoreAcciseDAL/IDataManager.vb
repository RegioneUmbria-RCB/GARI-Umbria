Imports AgronicaCoreAcciseCommon
Imports AgronicaCoreDataProvider
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreVarieDAL

Public Interface IDataManager

    Function ServizioAttivo(ByVal servizio As Configurazione_Servizio) As Boolean

    Function ServizioAttivo() As Boolean

    Sub DisabilitaServizio(ByVal servizio As Configurazione_Servizio)
    Sub DisabilitaServizio()

    Function ACCDAA_LeggiDaaDaSpedire() As List(Of ACCDAA_W_StoricoMessaggiDogane)

    Function ACCDAA_LeggiMessaggiRisposta() As List(Of ACCDAA_W_StoricoMessaggiDogane)
    Function ACCDAA_LeggiEsiti() As List(Of ACCDAA_W_StoricoMessaggiDogane)

    Sub ACCDAA_AggiornaStatoManager(Of T)(ByVal testata As ACCDAA_DAA_Testata, ByVal messaggio As ACCDAA_W_StoricoMessaggiDogane)
    Function Leggi_ACCDAA_Testata(ByVal DAA_ID As Integer) As ACCDAA_DAA_Testata

    Function Leggi_ACCDAA_ACC_RiepilogoInvioDati(ByVal rieplilogoInvioDati_ID As Integer) As ACCDAA_ACC_RiepilogoInvioDati

    Sub ACCDAA_StoricoMessaggiDogane_Errori_Logga(ByVal idLog As Integer, ByVal descrizione As String, ByVal tipo As String, ByVal numero As String, ByVal campo As String)
    Sub ACCDAA_StoricoMessaggiDogane_Errori_Logga(ByVal idLog As Integer, ByVal errori As List(Of D_Parsed))

    Sub ACCDAA_StoricoMessaggiDogane_Errori_Logga(ByVal idLog As Integer, ByVal errori As List(Of J_Parsed))
    Sub ACCDAA_StoricoMessaggiDogane_Errori_Elimina(ByVal idLog As Integer)

    Sub ACCDAA_StoricoMessaggiDogane_Aggiorna(ByVal entita As ACCDAA_W_StoricoMessaggiDogane)
    Sub ACCDAA_DAA_Testata_Aggiorna(ByVal entita As ACCDAA_DAA_Testata)

    Sub ACCDAA_ACC_RiepilogoInvioDati_Aggiorna(ByVal entita As ACCDAA_ACC_RiepilogoInvioDati)
    Sub Mov_Dettaglio_Tecnico_Extra_Aggiorna(ByVal entita As Mov_Dettaglio_Tecnico_Extra)
    Function Mov_Dettaglio_Tecnico_Leggi(ByVal DAA_Id As Integer) As Mov_Dettaglio_Tecnico_Extra

    Function Stampe_LeggiGaranzieCircolazione(ByVal Piva As String, ByVal dataDa As DateTime, ByVal dataA As DateTime) As List(Of GaranziaCircolazione)

    Function Stampe_Intestazione(ByVal Piva As String) As IntestazioneReport


    Function Stampe_LeggiPartiteSospensione(ByVal Piva As String, ByVal dataDa As DateTime, ByVal dataA As DateTime) As List(Of PartitaSospensione)

    Function LeggiConnessioneAgroGSB(ByRef objParametri_Server As AgronicaCoreParametri, ByRef objParametri_Super_Server As AgronicaCoreParametri) As AgronicaCoreParametri

    Function LeggiConfigurazioneServizio(ByVal Piva As String, ByVal Servizio As Integer, ByRef objParametri_Server As AgronicaCoreParametri, ByRef objParametri_Super_Server As AgronicaCoreParametri, Optional ByVal LeggiConnessione As Boolean = True) As Configurazione_Servizio

End Interface
