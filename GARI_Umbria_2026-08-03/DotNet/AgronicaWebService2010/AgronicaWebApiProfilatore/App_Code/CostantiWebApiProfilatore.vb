Public Module CostantiWebApiProfilatore
    '#################################################################################

    'Definizione del servizio corrente
    Public AgroConst_SERVIZIO As Integer = 5

    'Chiave di codifica e decodifica
    Public AWS_AgroKey_EncoderDecoder As String = "cJy97ei45vrNIsvbe86Wn42rffCHvc8e3it63koLa12"

    Public AgroKey_EncoderDecoder As String = "cobaltoleccioplutone"

    'Generazione di BaseCode e TopCode 
    Public AgroCode_BitPerCodice As Integer = 17




    Public StringaConnessione_SuperServer As String = ConfigurationManager.AppSettings("StringaConnessione_SuperServer").ToString()


    Public AgronicaCore_Flag_CancellazioneLogica As String = ConfigurationManager.AppSettings("AgronicaCore_Flag_CancellazioneLogica").ToString()
    Public AgronicaCore_Flag_Visibilita As String = ConfigurationManager.AppSettings("AgronicaCore_Flag_Visibilita").ToString()

    Public AgronicaCore_DirectoryLOG As String = ConfigurationManager.AppSettings("AgronicaCore_DirectoryLOG").ToString()
    Public AgronicaCore_FileNameLOG As String = ConfigurationManager.AppSettings("AgronicaCore_FileNameLOG").ToString()


    'verificare lettura da super-server
    Public SuperUserUsername As String = ConfigurationManager.AppSettings("SuperUserUsername").ToString()
    Public PivaSuperUser As String = ConfigurationManager.AppSettings("PivaSuperUser").ToString()
    Public cfgTokenParametri As String = ConfigurationManager.AppSettings("tokenParametri").ToString()

End Module
