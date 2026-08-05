Imports System.IO

Public Interface IFIleManager

    Function CreaFileZip(ByVal zipFileName As String, xmlFilePath As String) As String
    Function CreaFileZip(ByVal zipFileName As String, xmlFilePath As List(Of String)) As String
    Function OttieniPercorso(ByVal tipoPercorso As FatturaElettronicaPath) As String

    Function OttieniNomeFileLog() As String
    Function LeggiFilesDaSpedire() As List(Of FileInfo)

    Sub SpostaXMLInSpediti(ByVal fileName As String)

    Sub EliminaXML(ByVal fileName As String)

    Sub EliminaZip(ByVal fileName As String)
    Sub Initialize()

End Interface
