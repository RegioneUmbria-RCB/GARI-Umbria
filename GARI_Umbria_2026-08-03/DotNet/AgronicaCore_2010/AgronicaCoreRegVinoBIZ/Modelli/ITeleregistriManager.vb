Public Interface ITeleregistriManager

    Sub inizializzaReaderWriter()

    Sub cicloEsportazione()

    Function cicloEsportazioneInput() As Boolean

    Function cicloEsportazioneOutput() As Boolean

    Function inserisciAggiornaTuttoInput(tipoRichiesta As Integer) As Boolean

    Function eliminaTuttoInput() As Boolean

    Function inserisciAggiornaTuttoOutput() As Boolean

    Function eliminaTuttoOutput() As Boolean

    Sub checkResult()

End Interface
