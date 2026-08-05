Imports AgronicaCoreDataProvider.TipiEnumerativi

Public Class ConfigurazioneLogProviderEsteso

    Public LivelloLOG As enum_Livello_Log_Applicazioni
    Public ConfigurazioneSmtp As ConfigurazioneSmtp
    Public Mittente_Mail As String
    Public Destinatari_Mail As String
    Public Ambiente As String
    Public Limite_Errori_X_Scrittura_DBLog As String
    Public ConfigurazioneElasticSearch As ConfigurazioneElasticSearch

End Class

Public Class ConfigurazioneSmtp
    Public ClientSMTP As String
    Public ClientSMTP_Porta As String
    Public Enablessl_SMTP As String
    Public Password_SMTP As String
    Public User_SMTP As String
End Class

Public Class ConfigurazioneElasticSearch

    Public ElasticSearchUrl As String = String.Empty
    Public Parametri As ParametriElasticSearch = Nothing

End Class