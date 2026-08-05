Public Interface IFileManager

    Sub Initialize()
    Function OttieniPercorso(ByVal tipoPercorso As AccisePath) As String
    Function OttieniNomeFileLog() As String

    Sub Delete(ByVal percorsoFileCompleto As String)

End Interface
