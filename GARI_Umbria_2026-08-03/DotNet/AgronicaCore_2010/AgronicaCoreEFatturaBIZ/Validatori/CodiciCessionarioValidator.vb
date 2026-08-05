Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreEFatturaBIZ

Public Class CodiciCessionarioValidator : Inherits ValidatoreBase : Implements IFatturaValidator
    Public Sub New(ByVal objectType As Type,
                  ByVal riferimento As String,
                  ByVal soggetto As EnuValidatoreSoggetto,
                  ByVal mapper As IDecodificheMapper)
        MyBase.New(objectType, riferimento, soggetto, mapper)
    End Sub

    Public ReadOnly Property AllowNullObject As Boolean Implements IFatturaValidator.AllowNullObject
        Get
            Return False
        End Get
    End Property

    Public Function Validate(oggettoDaValidare As Object, fattura As FatturaGias, ByRef listaErrori As List(Of String)) As Boolean Implements IFatturaValidator.Validate

        Dim datiCessionario = DirectCast(oggettoDaValidare, DatiCessionarioMap)
        If datiCessionario Is Nothing Then
            Return False
        End If

        If datiCessionario.DatiAggiuntivi Is Nothing OrElse Not datiCessionario.DatiAggiuntivi.Any() Then
            listaErrori.Add(ComponiMessaggio("Dati minimi per generazione fattura elettronica non trovati"))
        End If

        Dim tipologiaContatto = datiCessionario.OttieniDatoAggiuntivo(enum_CodiciAnagrafe.TipoContattoFattura)
        If (String.IsNullOrEmpty(tipologiaContatto)) OrElse tipologiaContatto.Trim() = "0" Then
            listaErrori.Add(ComponiMessaggio("Tipologia contatto non impostata"))
        End If

        Return Not listaErrori.Any()

    End Function

End Class
