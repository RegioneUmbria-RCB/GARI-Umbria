Public Interface IHubIotExporter

    Sub Execute()

    Sub CheckRequirements(ByRef MessageError As String)
    Sub GeneraFileZip(ByVal IdDocument As String, ByVal FileName As String, ByVal WithPrescription As Boolean, ByRef MessageError As String)
    Sub SendFile(ByVal IdDocument As String, ByRef MessageError As String)
    Sub MarkDocumentAsSended(ByVal IdDocument As String, ByRef MessageError As String)

End Interface
