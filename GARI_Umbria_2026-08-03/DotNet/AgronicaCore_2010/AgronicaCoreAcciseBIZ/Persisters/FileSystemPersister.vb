Imports System.IO
Imports EU.Europa.EC.Markt.Dss.Signature

Public Class FileSystemPersister

    Private ReadOnly _logger As AcciseLogger
    Private ReadOnly _fileManager As FileManager
    Public Sub New(ByVal logger As AcciseLogger, ByVal fileManager As FileManager)
        _logger = logger
        _fileManager = fileManager
    End Sub

    Public Function SaveDAAInTempFile(ByVal data As Byte(), ByVal fileName As String) As String

        Dim percorsoCompleto = Path.Combine(_fileManager.OttieniPercorso(AccisePath.Temp), fileName)
        File.WriteAllBytes(percorsoCompleto, data)
        Return percorsoCompleto

    End Function

    Public Function SaveDAASigned(ByVal signed As Document, ByVal fileName As String) As String

        Dim percorsoCompleto = Path.Combine(_fileManager.OttieniPercorso(AccisePath.Signed), fileName)
        If (File.Exists(percorsoCompleto)) Then File.Delete(percorsoCompleto)
        Dim fout = File.OpenWrite(percorsoCompleto)

        signed.OpenStream().CopyTo(fout)
        fout.Close()
        fout.Dispose()

        Return percorsoCompleto

    End Function

    Public Sub SaveMessaggioRisposta(ByVal data As MemoryStream, ByVal fileName As String)

        Dim percorsoCompleto = Path.Combine(_fileManager.OttieniPercorso(AccisePath.Temp), fileName)

        data.Seek(0, SeekOrigin.Begin)
        Dim fs = New FileStream(percorsoCompleto, FileMode.OpenOrCreate)
        data.CopyTo(fs)
        fs.Flush()
        fs.Close()

    End Sub

End Class
