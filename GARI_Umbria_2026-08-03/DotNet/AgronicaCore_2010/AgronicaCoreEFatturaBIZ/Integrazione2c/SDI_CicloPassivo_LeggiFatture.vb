Imports AgronicaCoreEFatturaBIZ.Persisters

Public Class SDI_CicloPassivo_LeggiFatture

    Private ReadOnly _soapController As ISOAPControllerCicloPassivo
    Private ReadOnly _fileManager As IFIleManager
    Private ReadOnly _fileSystemPersister As IPersister
    Public Sub New(
                  ByVal soapController As ISOAPControllerCicloPassivo,
                ByVal fileManager As IFIleManager,
                ByVal fileSystemPersister As IPersister
        )
        _soapController = soapController
        _fileManager = fileManager
        _fileSystemPersister = fileSystemPersister
    End Sub

    Public Function LeggiFatture(ByVal dataInizio As Date) As Integer

        Dim fatture = _soapController.GetFatture(dataInizio)

        If Not fatture Is Nothing AndAlso fatture.Any() Then

            fatture.ForEach(Sub(f)

                                Dim fattura = _soapController.GetFattura(f.IdSdi, f.CodiceUfficio)
                                If Not fattura Is Nothing Then
                                    _fileSystemPersister.Persist(fattura.DatiFattura.NomeFile, fattura.FileFattura, fattura.DatiFattura.DataSdi)
                                End If

                            End Sub)

            Return fatture.Count

        End If

        Return 0
    End Function
    Private Sub SalvaFattura()



    End Sub


End Class
