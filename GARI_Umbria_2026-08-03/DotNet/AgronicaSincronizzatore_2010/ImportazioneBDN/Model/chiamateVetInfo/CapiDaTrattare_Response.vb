Public Class CapiDaTrattare_Response
    Inherits VetInfoResponseModel.VetInfoData_Response

    Public prescapoCodificaCodice As String
    Public prescapoCodificaDescrizione As String
    Public prescapoDiagnosiCodice As String
    Public prescapoDiagnosiDescrizione As String
    Public prescapoDtNascita As Date
    Public prescapoFlStatoAnomalia As String
    Public prescapoIdentificativo As String
    Public prescapoNumero As String
    Public prescapoNumeroAnimali As Integer
    Public prescapoSesso As String
    Public prescapoSomministrazioneCodice As String
    Public prescapoSomministrazioneDescrizione As String
    Public prescapoSottocategoriaCodice As String
    Public prescapoSottocategoriaDescrizione As String
    Public prescapoSpecieCodice As String
    Public prescapoSpecieDescrizione As String
    Public prescapoTempiSospensione As List(Of TempiSospensioneCapo)
End Class

