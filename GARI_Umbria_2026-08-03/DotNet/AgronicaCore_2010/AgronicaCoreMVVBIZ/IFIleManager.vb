Imports System.IO

Public Interface IFIleManager

    Function OttieniPercorso(ByVal tipoPercorso As MVV_E_Path) As String

    Function OttieniNomeFileLog() As String
    Function LeggiFilesDaSpedire() As List(Of FileInfo)

    Sub SpostaXMLInSpediti(ByVal fileName As String)

    Sub EliminaXML(ByVal fileName As String)

    Sub Initialize()

End Interface
