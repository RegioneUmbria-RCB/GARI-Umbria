Public Class ParametriImportAnagrafiche
    Public Property environment As String = "Dev"
    Public Property ExtSysRef As String
    Public Property DemetraBaseUrl As String
    Public Property apikey As String
    Public Property ColdirettiProvisioningBaseUrl As String
    Public Property ColdirettiAuthenticatorBaseUrl As String
    Public Property client_id As String
    Public Property client_secret As String
    Public Property grant_type As String
    Public Property username As String
    Public Property password As String
    Public Property realm As String
    Public Property cacheLog As Integer
    'Public Property DefPwd As String
    Public Property DefaultProfile As String
    Public Property ScriviLogAnagrafePianoColturale As Boolean
    Public Property BatchSize As Integer
    Public Property TagName As String
    Public Property EnableNotifyToProvisioning As Boolean = False
    Public Property ElasticSearch_Notify As Boolean = False
    Public Property ElasticSearch_InviaSoloErrori As Boolean = False
    Public Property TipoAnagrafe As TipoAnagrafiche
    Public Property ParametriContatti As AgronicaCoreDTOStd.InData.Demetra.ParametriInterscambioContatti
    Public Property ReaplceInvalidPolygon As Boolean = True
    Public Property EscludiPCGProvvisorio As Boolean = True
    Public Property TipoConfigurazione As AgronicaCoreDataProvider.TipiEnumerativi.enum_DataPublish_Configurazione = AgronicaCoreDataProvider.TipiEnumerativi.enum_DataPublish_Configurazione.Demetra
    Public Property SetAppezzaAddress As Boolean = False
    Public Property EnableVerboseLogPCG As Boolean = False
    Public Property CancellazioneLogica As Boolean = False
    Public Property EnableIdentityProvider As Boolean = True
    Public Property InviaAckEsito As Boolean = True
End Class

Public Class TipoAnagrafiche
    Public Property Anagrafica As Boolean = True
    Public Property Terreni As Boolean = True
    Public Property PCG As Boolean = True
    Public Property PCG_Terreni As Boolean = True
    Public Property Equipaggiamenti As Boolean = True
    Public Property Lavoratori As Boolean = True
    Public Property Gruppi_Appezzamenti As Boolean = True
End Class