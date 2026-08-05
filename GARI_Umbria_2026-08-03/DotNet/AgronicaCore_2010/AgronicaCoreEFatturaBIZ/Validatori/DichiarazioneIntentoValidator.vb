Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreEFatturaBIZ.Entita.FatturaPa

Public Class DichiarazioneIntentoValidator : Inherits ValidatoreBase : Implements IFatturaValidator

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

        Dim procedoAControllo As Boolean = False

        Dim contattiCodici = DirectCast(oggettoDaValidare, IEnumerable(Of DatoAggiuntivoMap)).ToList
        If contattiCodici Is Nothing Then
            Return False
        End If

        Try

            If Not IsNothing(fattura) AndAlso Not IsNothing(fattura.Fattura) AndAlso Not IsNothing(fattura.Fattura.Dettagli) Then
                If Not IsNothing(fattura.Fattura.Dettagli.ProdottiServizi) Then
                    For Each ps In fattura.Fattura.Dettagli.ProdottiServizi
                        If Not IsNothing(ps.Movimento) Then
                            Dim natura = _mapper.DecodificaNaturaEsclusionePerDichiarazioneIntento(ps.Movimento.Cod_Iva)
                            If natura <> Int32.MinValue Then
                                If natura = NaturaType.N35 Then
                                    procedoAControllo = True
                                    Exit For
                                End If
                            End If
                        End If
                    Next
                End If
            End If

            If procedoAControllo Then
                Dim numeroProtocollo As String = fattura.Cessionario.OttieniDatoAggiuntivo(enum_CodiciAnagrafe.DichiarazioneIntentoNumeroProtocollo)
                Dim dataProtocollo As String = fattura.Cessionario.OttieniDatoAggiuntivo(enum_CodiciAnagrafe.DichiarazioneIntentoDataRicezione)

                ' Se uno dei due dati per la dichiarazione di intenti non è valorizzato non creo il nodo
                If String.IsNullOrEmpty(numeroProtocollo) OrElse String.IsNullOrEmpty(dataProtocollo) Then
                    listaErrori.Add("Sono presenti righe di dettaglio che prevedono le informazioni relative alla Dichiarazione di Intento. Compilare i campi N° Protocollo e Data nei dati del cliente.")
                    Return False
                End If

                ' Controllo sulla validita della data (perchè arriva da un campo stringa e potenzialmente potrebbe essere errato)
                Dim checkData As DateTime = DateTime.MinValue
                If Not DateTime.TryParse(dataProtocollo, checkData) Then
                    listaErrori.Add("Sono presenti righe di dettaglio che prevedono le informazioni relative alla Dichiarazione di Intento. La data ricezione del N° Protocollo non è una data valida.")
                    Return False
                End If

            End If

            Return True

        Catch ex As Exception
            Return True
        End Try

    End Function

End Class
