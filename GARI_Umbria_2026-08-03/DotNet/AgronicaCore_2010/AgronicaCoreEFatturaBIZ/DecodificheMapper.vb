Imports System.Threading.Tasks
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreEFatturaBIZ
Imports AgronicaCoreEFatturaBIZ.Entita
Imports AgronicaCoreEFatturaBIZ.Entita.FatturaPa
Imports AgronicaCoreEFatturaDAL
Imports AgronicaCoreEntityFramework_POCO

Public Class DecodificheMapper : Implements IDecodificheMapper

    Private _mappedData As New Dictionary(Of String, Dictionary(Of String, String))
    Private ReadOnly _reader As IDataReader

    Private _provincie As List(Of Lista_Province)
    Private _comuni As List(Of ISTAT)
    Private _contattiCodici As List(Of Contatto_Codice)
    Private _tipiDoc As List(Of TipologiaDocumento)
    Private _regimiFiscali As List(Of RegimeFiscaleXSezionale)
    Private _unitaMisura As List(Of UnitaMisura)
    Private _aliquoteIva As List(Of IVA)
    Private _rappresentantiFiscali As List(Of RappresentanteFiscale)
    Private _stabiliOrganizzazioni As List(Of StabileOrganizzazione)
    Private _moduloGenerazione As enum_Omni_Modulo_Generazione
    Private _tipoIMpresaGerarchia As enum_TipoImpresaGerarchia
    Private _sezionali As List(Of Imprese_Sezionali)
    Public ReadOnly Property ModuloGenerazione As enum_Omni_Modulo_Generazione Implements IDecodificheMapper.ModuloGenerazione
        Get
            Return _moduloGenerazione
        End Get

    End Property

    Public ReadOnly Property TipoImpresaGerarchia As enum_TipoImpresaGerarchia Implements IDecodificheMapper.TipoImpresaGerarchia
        Get
            Return _tipoIMpresaGerarchia
        End Get
    End Property

    Public Sub New(ByVal reader As IDataReader)
        _reader = reader
    End Sub

    Public Function Inizializza(ByVal PIVA As String, ByVal agendeXLavCod As List(Of AgendaXLavCod),
                                ByRef errori As String) As Boolean Implements IDecodificheMapper.Inizializza

        Dim messaggioErrore As String = ""
        Dim nomeRoutine As String = "DecodificheMapper.Inizializza"

        Try
            Dim t1 = Task.Factory.StartNew(Function() _reader.CaricaProvince())
            Dim t2 = Task.Factory.StartNew(Function() _reader.CaricaRegimiFiscali(PIVA))
            Dim t3 = Task.Factory.StartNew(Function() _reader.CaricaContattiCodici(PIVA))
            Dim t4 = Task.Factory.StartNew(Function() _reader.CaricaUnitaMisura)
            Dim t5 = Task.Factory.StartNew(Function() _reader.CaricaAliquteIVA)
            Dim t6 = Task.Factory.StartNew(Function() _reader.CaricaRappresentantiFiscali(PIVA))

            Dim risUms = agendeXLavCod.Select(Function(a) CInt(a.Cod_RisUm)).Distinct().ToList()
            Dim t7 = Task.Factory.StartNew(Function() _reader.CaricaStabiliOrganizzazioni(PIVA, risUms))
            Dim t8 = Task.Factory.StartNew(Function() _reader.CaricaComuni())
            Dim t9 = Task.Factory.StartNew(Function() _reader.LeggiModuloGenerazione())
            Dim t10 = Task.Factory.StartNew(Function() _reader.LeggiTipoImpresaGerarchia(PIVA))
            Dim t11 = Task.Factory.StartNew(Function() _reader.CaricaSezionali(PIVA))
            Dim t12 = Task.Factory.StartNew(Function() _reader.CaricaTipologieDocumento())


            Dim tasks = New List(Of Task) From {t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12}
            Task.WaitAll(tasks.ToArray())

            _provincie = t1.Result
            _regimiFiscali = t2.Result
            _contattiCodici = t3.Result
            _unitaMisura = t4.Result
            _aliquoteIva = t5.Result
            _rappresentantiFiscali = t6.Result
            _stabiliOrganizzazioni = t7.Result
            _comuni = t8.Result
            _moduloGenerazione = t9.Result
            _tipoIMpresaGerarchia = t10.Result
            _sezionali = t11.Result
            _tipiDoc = t12.Result

            Return True

        Catch ex As AggregateException
            messaggioErrore = ex.Message.ToString & If(Not IsNothing(ex.InnerException), " (" & ex.InnerException.Message.ToString & ")", "")
            errori &= "[" & nomeRoutine & "] : " & messaggioErrore & "</br>"
            Return False
        Catch ex As Exception
            messaggioErrore = ex.Message.ToString & If(Not IsNothing(ex.InnerException), " (" & ex.InnerException.Message.ToString & ")", "")
            errori &= "[" & nomeRoutine & "] : " & messaggioErrore & "</br>"
            Return False
        End Try

    End Function

    Public Function ContattoCodiciPerPIVA(ByVal PIVA As String, ByVal cod_Contatto As String) As List(Of Contatto_Codice) Implements IDecodificheMapper.ContattoCodiciPerPIVA

        Return _contattiCodici.Where(Function(x) x.PIVA.Equals(PIVA) AndAlso x.Cod_Contatto.Equals(cod_Contatto)).ToList()

    End Function

    Public Function GetDecodificaValue(nomeTabella As String, valoreGias As String) As Integer Implements IDecodificheMapper.GetDecodificaValue

        Dim tableData As New Dictionary(Of String, String)

        If Not _mappedData.TryGetValue(nomeTabella, tableData) Then
            Return Int32.MinValue
        End If

        Dim value As String = String.Empty

        If tableData.TryGetValue(valoreGias, value) Then

            Dim e = Activator.CreateInstance(Type.GetType(nomeTabella))

            If [Enum].IsDefined(e.GetType(), value) Then
                Return [Enum].Parse(e.GetType(), value)
            Else
                Return Int32.MinValue
            End If

        Else
            Return Int32.MinValue
        End If

    End Function

    Public Function DecodificaTipoDocumento(ByVal codiceTipoDoc As Integer) As TipoDocumentoType Implements IDecodificheMapper.DecodificaTipoDocumento

        Dim rf As TipologiaDocumento = _tipiDoc.FirstOrDefault(Function(s) s.Codice.Equals(codiceTipoDoc))
        If rf Is Nothing Then
            Throw New GiasEntityToEFatturaMapException(String.Format("Tipologia Documento non trovata per codice {0}", codiceTipoDoc))
        End If

        Dim result As TipoDocumentoType
        If [Enum].TryParse(rf.SiglaAE, result) Then
            Return result
        End If

        Throw New GiasEntityToEFatturaMapException(String.Format("Tipologia Documento non trovata per codice {0}", codiceTipoDoc))

    End Function

    Public Function RegimeFiscaleXSezionale(PIVA As String, sezionale As Integer?) As Integer Implements IDecodificheMapper.RegimeFiscaleXSezionale
        Dim result = _regimiFiscali.FirstOrDefault(Function(s) s.PIVA.Equals(PIVA) AndAlso s.CodiceSezionale.Equals(sezionale))

        If result Is Nothing Then
            Throw New GiasEntityToEFatturaMapException(String.Format("Regime Fiscale non trovato per il Sezionale con codice {0}", sezionale.Value))
        End If

        Return result.CodiceRegimeFiscale

    End Function

    Public Function DecodificaRegimeFiscale(ByVal PIVA As String,
                                            ByVal codice_Sezionale As Integer,
                                            ByVal codice_regimeFiscale As Integer) As FatturaPa.RegimeFiscaleType Implements IDecodificheMapper.DecodificaRegimeFiscale

        Dim rf = _regimiFiscali.FirstOrDefault(Function(s) s.PIVA.Equals(PIVA) AndAlso s.CodiceSezionale = codice_Sezionale AndAlso s.CodiceRegimeFiscale = codice_regimeFiscale)
        If rf Is Nothing Then
            Throw New GiasEntityToEFatturaMapException(String.Format("Regime fiscale non trovato per Sezionale {0} e Regime Fiscale {1}", codice_Sezionale, codice_regimeFiscale))
        End If

        Dim result As FatturaPa.RegimeFiscaleType
        If [Enum].TryParse(rf.SiglaAE, result) Then
            Return result
        End If

        Throw New GiasEntityToEFatturaMapException(String.Format("Regime fiscale non trovato per Sezionale {0} e Regime Fiscale {1}", codice_Sezionale, codice_regimeFiscale))

    End Function

    Public Function DecodificaUnitaMisura(codice As Integer) As String Implements IDecodificheMapper.DecodificaUnitaMisura

        Dim result = _unitaMisura.FirstOrDefault(Function(um) um.UDM_COD.Equals(codice))

        If Not result Is Nothing Then
            Return result.UDM_SIM
        End If

        Return String.Empty

    End Function

    Public Function DecodificaAliquotaIva(codice As Integer) As Decimal Implements IDecodificheMapper.DecodificaAliquotaIva

        Dim result = _aliquoteIva.FirstOrDefault(Function(i) i.Codice.Equals(codice))
        If Not result Is Nothing AndAlso result.Aliquota.HasValue Then
            Return result.Aliquota.Value
        End If

        Return 0

    End Function

    Public Function DecodificaNaturaEsclusione(ByVal codiceIva As Integer) As NaturaType Implements IDecodificheMapper.DecodificaNaturaEsclusione

        Dim rf As IVA = _aliquoteIva.FirstOrDefault(Function(s) s.Codice.Equals(codiceIva))
        If rf Is Nothing Then
            Throw New GiasEntityToEFatturaMapException(String.Format("Natura Esclusione non trovata per Aliquota Iva {0}", codiceIva))
        End If

        If rf.Aliquota = 0 Then
            Dim result As FatturaPa.NaturaType
            'Giulia 19/11/2020: le nuove codifiche hanno il punto, che però nella generazione delle classi viene eliminato
            Dim natExDb As String = Replace(rf.NaturaEsclusione, ".", "")
            If [Enum].TryParse(natExDb, result) Then
                Return result
            Else
                Throw New GiasEntityToEFatturaMapException(String.Format("Natura Esclusione non trovata per Aliquota Iva {0}", codiceIva))
            End If
        End If

        Return Int32.MinValue

    End Function


    Public Function OttieniStabileOrganizzazione(cod_Contatto As String, cod_RisUM As Integer) As StabileOrganizzazione Implements IDecodificheMapper.OttieniStabileOrganizzazione
        Return _stabiliOrganizzazioni.FirstOrDefault(Function(s) s.Cod_Contatto.Equals(cod_Contatto) AndAlso s.Cod_RisUm.Equals(cod_RisUM))
    End Function

    Public Function OttieniRappresentanteFiscale(ByVal cod_RisUm As Integer) As RappresentanteFiscale Implements IDecodificheMapper.OttieniRappresentanteFiscale
        Return _rappresentantiFiscali.FirstOrDefault(Function(r) r.Cod_RisUm.Equals(cod_RisUm.ToString()))
    End Function

    Public Function DecodificaEsigibilitaIVA(codiceSezionale As Integer) As EsigibilitaIVAType Implements IDecodificheMapper.DecodificaEsigibilitaIVA

        Dim rf = _regimiFiscali.FirstOrDefault(Function(s) s.CodiceSezionale.Equals(codiceSezionale))
        If rf Is Nothing Then
            Return Int32.MinValue
        End If

        Select Case rf.EsigibilitaIva
            Case enum_EsigibilitaIva.Non_Specificata
                Return Int32.MinValue
            Case enum_EsigibilitaIva.Differita
                Return FatturaPa.EsigibilitaIVAType.D
            Case enum_EsigibilitaIva.Immediata
                Return EsigibilitaIVAType.I
            Case enum_EsigibilitaIva.Scissione_Pagamenti
                Return EsigibilitaIVAType.S
            Case Else
                Return Int32.MinValue
        End Select

    End Function

    Public Function OttieniProvincia(pro_cod_istat As String) As Lista_Province Implements IDecodificheMapper.OttieniProvincia
        Return _provincie.FirstOrDefault(Function(p) p.PROV = pro_cod_istat)
    End Function

    Public Function OttieniComune(pro_cod_istat As String, com_cod_istat As String) As ISTAT Implements IDecodificheMapper.OttieniComune
        Return _comuni.FirstOrDefault(Function(c) c.PROV = pro_cod_istat AndAlso c.COM = com_cod_istat)
    End Function

    Public Function DecodificaRiferimentoNormativo(codiceIva As Integer) As String Implements IDecodificheMapper.DecodificaRiferimentoNormativo

        Dim rn = _aliquoteIva.FirstOrDefault(Function(i) i.Codice.Equals(codiceIva))
        If rn Is Nothing Then
            Throw New GiasEntityToEFatturaMapException(String.Format("Riferimento Normativo no trovato per Codice Iva {0}", codiceIva))
        End If

        If String.IsNullOrEmpty(rn.Descrizione) Then
            Throw New GiasEntityToEFatturaMapException(String.Format("Riferimento Normativo senza descrizione per Codice Iva {0}", codiceIva))
        End If

        Return rn.Descrizione

    End Function
    Public Function DecodificaModalitaPagamento(ByVal tipo As enum_PagamentiCausali) As ModalitaPagamentoType Implements IDecodificheMapper.DecodificaModalitaPagamento

        Select Case tipo
            Case enum_PagamentiCausali.Assegno
                Return ModalitaPagamentoType.MP02
            Case enum_PagamentiCausali.Bancomat, enum_PagamentiCausali.CartaCredito
                Return ModalitaPagamentoType.MP08
            Case enum_PagamentiCausali.Bonifico
                Return ModalitaPagamentoType.MP05
            Case enum_PagamentiCausali.Contanti, enum_PagamentiCausali.RimessaDiretta, enum_PagamentiCausali.Contrassegno
                Return ModalitaPagamentoType.MP01
            Case enum_PagamentiCausali.MAV
                Return ModalitaPagamentoType.MP13
            Case enum_PagamentiCausali.RiBa
                Return ModalitaPagamentoType.MP12
            Case enum_PagamentiCausali.RID
                Return ModalitaPagamentoType.MP09
            Case Else
                Return Int32.MinValue
        End Select

    End Function

    Public Function OttieniSezionale(sezionale_Cod As Integer) As Imprese_Sezionali Implements IDecodificheMapper.OttieniSezionale
        If _sezionali Is Nothing Then
            Return Nothing
        Else
            Return _sezionali.FirstOrDefault(Function(s) s.Sezionale_Cod.Equals(sezionale_Cod))
        End If
    End Function

    Public Function DecodificaNaturaEsclusionePerDichiarazioneIntento(codiceIva As Integer) As NaturaType Implements IDecodificheMapper.DecodificaNaturaEsclusionePerDichiarazioneIntento
        Dim rf As IVA = _aliquoteIva.FirstOrDefault(Function(s) s.Codice.Equals(codiceIva))
        If rf Is Nothing Then
            Throw New GiasEntityToEFatturaMapException(String.Format("Natura Esclusione non trovata per Aliquota Iva {0}", codiceIva))
        End If

        If rf.Aliquota = 0 Then
            Dim result As FatturaPa.NaturaType
            'Giulia 19/11/2020: le nuove codifiche hanno il punto, che però nella generazione delle classi viene eliminato
            Dim natExDb As String = Replace(rf.NaturaEsclusione, ".", "")
            If [Enum].TryParse(natExDb, result) Then
                Return result
            Else
                Return Int32.MinValue
            End If
        End If

        Return Int32.MinValue

    End Function

End Class

