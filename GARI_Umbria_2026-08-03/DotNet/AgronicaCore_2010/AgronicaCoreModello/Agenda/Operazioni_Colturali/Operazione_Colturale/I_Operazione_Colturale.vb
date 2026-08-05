Imports AgronicaCoreDataProvider
Imports AgronicaCoreModello.OperazioneAgenda_Temp

Namespace Agenda.Operazioni_Colturali.Operazione_Colturale

    Public Interface I_Operazione_Colturale

        Property Nota As String

        Property ID_Agenda As Integer

        'Non modificabile
        ReadOnly Property Piva As String

        Property Sa_Cod As Integer

        Property ID_Reg As Integer

        Property DataOperazione As Date

        'Non modificabile, ricavata runtime
        ReadOnly Property RagioneSociale As String

        'Non modificabile, creata nel costruttore degli oggetti che ereditano
        ReadOnly Property Lav_Cod As Integer

        'Non modificabile, creata runtime nel costruttore degli oggetti che ereditano
        ReadOnly Property Lav_Des As String

        'Non modificabile, creata nel costruttore degli oggetti che ereditano
        ReadOnly Property Cau_Mov As Integer

        'Non modificabile, creata nel costruttore degli oggetti che ereditano
        ReadOnly Property Cau_Des As String

        Property Magazzino As Anagrafe.Magazzino

        'Non modificabile, creata nel costruttore degli oggetti che ereditano, influenza la pagina, forse non è corretto tenerlo qui
        Property Tipo_Operazione As TipiEnumerativi.enum_TipoOperazioneDB

        'Non modificabile, creata nel costruttore degli oggetti che ereditano, influenza la pagina, forse non è corretto tenerlo qui
        ReadOnly Property OperazioneMulticentro As Boolean

        Property Note_Codificate As List(Of Integer)

        '------------------------------
        'lista movimenti costi accessori, sarà da modificare creando modellomnapostito!
        Property Costi_Accessori_Temporaneo As List(Of Movimento)
        '------------------------------

        'Ripristina i dati dell'operazione, forse da togliere, ci pensa l'handler a creare uno nuovo
        Sub RipristinaDatiIniziali()

        'cancella l'operazione, forse da togliere, ci pensa l'handler a creare uno nuovo
        Sub CancellaDati()


    End Interface


End Namespace

