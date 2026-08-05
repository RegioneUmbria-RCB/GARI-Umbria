Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreEFatturaBIZ.Entita.FatturaPa
Imports AgronicaCoreEntityFramework_POCO
Public Class FatturaGias

    Public ProgressivoFattura As Integer

    <ValidationAttribute(0, GetType(CodiciCedenteValidator), GetType(DatiCedenteMap), "Cedente", EnuValidatoreSoggetto.Cedente)>
    Public Cedente As DatiCedenteMap

    <ValidationAttribute(0, GetType(CodiciCessionarioValidator), GetType(DatiPrincipaliCessionarioMap), "Cessionario", EnuValidatoreSoggetto.Cessionario)>
    Public Cessionario As DatiCessionarioMap

    Public CessionarioDiverso As DatiCessionarioDiversoMap
    Public Fattura As FatturaMap
    Public FlagPa As Boolean
    Public FatturaType As FatturaElettronicaType
    Public DocumentiCollegati As DocumentiCollegatiMap
    Public OrdiniAcquisto As List(Of OrdineAcquistoMap)
    Public RifNotaCreditoDebito As RifNotaCreditoDebitoMap
    Public Pagamenti As List(Of PagamentoMap)

    <ValidationAttribute(0, GetType(LegaleRappresentanteValidator), GetType(LegaleRappresentanteMap), "LegaleRappresentante", EnuValidatoreSoggetto.LegaleRappresentante)>
    Public LegaleRappresentante As LegaleRappresentanteMap

    Public Sub New()

    End Sub


End Class

Public Class DocumentiCollegatiMap
    Public Documenti As List(Of DocumentoCollegatoMap)
    Public Riepilogo As List(Of DocumentoCollegatiRiepilogoMap)
End Class

Public Class OrdineAcquistoMap
    Public N_Doc_Cliente As String
    Public Data_Doc_Cliente As DateTime?
    Public Ordine_Det As Integer
End Class

Public Class PagamentoMap
    Public DatiPagamento As DatiPagamentoMap
    Public CausalePagamento As CausalePagamentoMap
    Public DatiBancari As DatiBancariMap
End Class
Public Class DatiPagamentoMap
    Public Importo As Double?
    Public Percentuale As Double?
    Public Data_Pagamento As Date?
    Public DataScadenza_Manuale As Date?
    Public Cau_Pagamento As Integer?
    Public Cod_Liquidita_Avere As Integer?
    Public Cod_Liquidita_Dare As Integer?
    Public Previsto_Avvenuto As Integer?
End Class

Public Class CausalePagamentoMap
    Public Cau_Pagamento_Des As String
    Public Cau_Pagamento_Sigla As String
    Public Tipo As Integer
End Class
Public Class DatiBancariMap
    Public Numero As String
    Public Abi As String
    Public Cab As String
    Public Cin As String
    Public Cifre_Controllo As String
    Public Nazione As String
    Public Bic As String
    Public Istituto_Des As String

    Public Overrides Function ToString() As String

        If Not String.IsNullOrEmpty(Numero) AndAlso Not String.IsNullOrEmpty(Nazione) AndAlso
               Not String.IsNullOrEmpty(Cifre_Controllo) AndAlso Not String.IsNullOrEmpty(Cin) AndAlso
               Not String.IsNullOrEmpty(Abi) AndAlso Not String.IsNullOrEmpty(Cab) Then
            Return String.Format("{0}{1}{2}{3}{4}{5}", Nazione, Cifre_Controllo, Cin, Abi, Cab, Numero)
        Else
            Return String.Empty
        End If

    End Function

End Class
Public Class RifNotaCreditoDebitoMap
    Public N_Nota_DDT As String
    Public Data_Nota_DDT As DateTime?
    Public N_Nota_Riga_DDT As String
    Public N_Nota_Fattura As String
    Public Data_Nota_Fattura As DateTime
End Class

Public Class DocumentoCollegatoMap
    Public Id_Agenda As Integer
    Public Id_Agenda_Rif As Integer
    Public Doc_Numero_Sin As String
    Public Doc_Numero As String
    Public Doc_Numero_Des As String
    Public Data_Movimento As DateTime
    Public Ordine_Det As Integer
End Class

Public Class DocumentoCollegatiRiepilogoMap
    Public Id_Agenda As Integer
    Public NumMovimenti As Integer
End Class
Public Class DatiCessionarioMap

    <ValidationAttribute(0, GetType(AnagraficaValidator), GetType(DatiPrincipaliCessionarioMap), "Cessionario", EnuValidatoreSoggetto.Cessionario)>
    Public DatiPrincipali As DatiPrincipaliCessionarioMap

    <ValidationAttribute(0, GetType(DichiarazioneIntentoValidator), GetType(DatiPrincipaliCessionarioMap), "Cessionario", EnuValidatoreSoggetto.Cessionario)>
    Public DatiAggiuntivi As IEnumerable(Of DatoAggiuntivoMap)

    Public StabileOrganizzazione As StabileOrganizzazioneMap
    Public Function OttieniDatoAggiuntivo(ByVal id_codice As Integer) As String

        If DatiAggiuntivi Is Nothing OrElse DatiAggiuntivi.Count() = 0 Then
            Return String.Empty
        End If

        Dim dato = DatiAggiuntivi.FirstOrDefault(Function(da) da.id_cod = id_codice)
        If dato Is Nothing Then
            Return String.Empty
        Else
            Return dato.val_cod
        End If

    End Function

End Class

Public Class DatiPrincipaliCessionarioMap

    Public PEC As String
    Public IDSDI As String
    Public Cod_RisUm As Integer

    <ValidationAttribute(0, GetType(PivaValidator), GetType(AnagraficaMap), "Cessionario", EnuValidatoreSoggetto.Cessionario)>
    Public Anagrafica As AnagraficaMap

    <ValidationAttribute(0, GetType(IndirizzoMapValidator), GetType(IndirizzoMap), "Cessionario", EnuValidatoreSoggetto.Cessionario)>
    Public Indirizzo As IndirizzoMap

End Class

Public Class DatiCessionarioDiversoMap

    Public Cod_RisUm As Integer

    Public Anagrafica As AnagraficaMap

    <ValidationAttribute(0, GetType(IndirizzoMapValidator), GetType(IndirizzoMap), "CessionarioDiverso", EnuValidatoreSoggetto.CessionarioDiverso)>
    Public Indirizzo As IndirizzoMap

End Class

Public Class LegaleRappresentanteMap

    Public Anagrafica As AnagraficaMap

End Class

Public Class DatiCedenteMap

    Public TipoImpresa As enum_TipoImpresaGerarchia
    <ValidationAttribute(0, GetType(AnagraficaValidator), GetType(DatiPrincipaliCedenteMap), "Cedente", EnuValidatoreSoggetto.Cedente)>
    Public DatiPrincipali As DatiPrincipaliCedenteMap
    Public DatiAggiuntivi As IEnumerable(Of DatoAggiuntivoMap)

    Public Function OttieniDatoAggiuntivo(ByVal id_codice As Integer) As String

        If DatiAggiuntivi Is Nothing OrElse DatiAggiuntivi.Count() = 0 Then
            Return String.Empty
        End If

        Dim dato = DatiAggiuntivi.FirstOrDefault(Function(da) da.id_cod = id_codice)
        If dato Is Nothing Then
            Return String.Empty
        Else
            Return dato.val_cod
        End If

    End Function

End Class

Public Class DatiPrincipaliCedenteMap

    Public TipoImpresaGerarchia As Integer
    <ValidationAttribute(0, GetType(PivaValidator), GetType(AnagraficaMap), "Cedente", EnuValidatoreSoggetto.Cedente)>
    Public Anagrafica As AnagraficaMap
    <ValidationAttribute(0, GetType(IndirizzoMapValidator), GetType(IndirizzoMap), "Cedente", EnuValidatoreSoggetto.Cedente)>
    Public Indirizzo As IndirizzoMap
    Public Sub New()

    End Sub

End Class

Public Class IndirizzoMap
    Public Tipo_Indirizzo As Integer
    Public ind_des As String
    Public frz_des As String
    Public CAP As String
    Public com_des As String
    Public pro_cod As String
    Public pro_cod_istat As String
    Public stato As String
    Public com_cod_istat As String
End Class

Public Class DatoAggiuntivoMap

    Public id_cod As Integer
    Public val_cod As String

End Class

Public Class FatturaMap

    Public NumeroFattura As String
    Public PIVA As String
    Public TipoFattura As String
    Public Codice_RisUM_Cessionario As Integer
    Public Testata As DatiTestataFatturaMap
    Public Dettagli As DatiDettaglioFatturaMap

End Class

Public Class DatiTestataFatturaMap

    Public Id_Agenda As Integer
    Public Cau_Mov As String
    Public Mov_Desc As String
    Public Doc_Numero As Double?
    Public Cod_RisUm As Integer
    Public Cod_RisUm_Aggiuntivo As Integer?
    Public Cod_RisUm_Altro As Integer
    Public TotaleFattura As Double?
    Public Cod_Destinazione As Integer?
    Public Cod_IndirizzoDestinazione As Integer?
    Public Causale_Trasporto As String
    Public Doc_Numero_Des As String
    Public Doc_Numero_Sin As String
    Public Data_Registrazione As DateTime
    Public Data_Movimento As DateTime
    Public Sezionale_Cod As Integer?
    Public Lav_cod As Integer
    Public Des_lib As String
    Public TipoDocumento As Integer?

End Class

Public Class DatiDettaglioFatturaMap

    Public Bolli As List(Of Movimenti_dettagli)
    Public ProdottiServizi As List(Of ProdottoServizioMap)

End Class

Public Class ProdottoServizioMap

    Public Movimento As Movimenti_dettagli
    Public Flag_Extra As Boolean
    Public Descrizione As String
    Public DescrizioneLibera As Boolean

End Class

Public Class StabileOrganizzazioneMap
    Public Cod_RisUm As Integer
    Public Cod_Contatto As String

    <ValidationAttribute(0, GetType(IndirizzoMapValidator), GetType(IndirizzoMap), "StabileOrganizzazione", EnuValidatoreSoggetto.Cessionario)>
    Public Indirizzo As IndirizzoMap
End Class
Public Class AnagraficaMap
    Public rag_soc As String
    Public PIVA As String
    Public Codice_Fiscale As String
    Public CodRegimeFiscale As Integer
    Public Id_CF As Short
    Public Nome As String
    Public Cognome As String
    Public Cod_Contatto As String
    Public PivaReale As String
End Class


