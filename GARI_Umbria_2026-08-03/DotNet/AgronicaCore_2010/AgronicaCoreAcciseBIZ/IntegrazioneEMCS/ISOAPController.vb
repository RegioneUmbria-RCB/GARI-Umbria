Public Interface ISOAPController

    Function PUT(ByVal fileContent As Byte(), ByVal fileName As String, ByRef errore As String) As Boolean
    Function DIR(ByVal patternRicerca As String, ByRef errore As String) As List(Of FileRisposta)
    Function [GET](ByVal nomeFileRisposta As String, ByRef errore As String) As Byte()

    Function GETLOG() As Boolean

End Interface
