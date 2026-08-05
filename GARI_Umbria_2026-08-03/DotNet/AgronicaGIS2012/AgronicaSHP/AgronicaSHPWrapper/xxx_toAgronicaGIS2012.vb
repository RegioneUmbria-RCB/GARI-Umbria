Imports AgronicaSHPWrapper.InterpretaDatiDBF

Public Interface xxx_toAgronicaGIS2012

    ''' <summary>
    ''' restituisce il file in formato agronica.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ''' <param name="lTipoEntita_Cod"></param>
    ''' <param name="trasformaSistemaRiferimento"></param>
    Function convert(configurazioneImportazione As ConfigurazioneImportazione,
                ByVal objParametriServer As AgronicaCoreDataProvider.AgronicaCoreParametri,
                ByVal objParametriUtenti As AgronicaCoreDataProvider.AgronicaCoreParametri
        ) As String
End Interface
