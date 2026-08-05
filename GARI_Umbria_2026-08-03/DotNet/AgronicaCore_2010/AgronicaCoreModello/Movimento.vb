Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider

Namespace OperazioneAgenda_Temp    

    Public Class Movimento

        Private _Movimenti_Dettagli_Tecnici As List(Of Movimento_Dettaglio_Tecnico)
        Private _Movimenti_Dettagli_Tecnici_Extra As List(Of Movimento_Dettaglio_Tecnico_Extra)
        Private _Movimenti_Dettagli As List(Of Movimento_Dettaglio)
        Private _Pagamenti As List(Of Pagamento)

        Sub New()

            Piva = ""
            Sa_Cod = 0
            Id_Agenda = 0
            Id_Mov = 0
            Data = AGRODATAINIZIO
            Lav_Cod = 0
            Cau_Mov = ""
            Cod_Risum = 0
            Mov_Desc = ""
            Num_Protocollo = 0
            Num_Protocollo_Decimal = 0
            Disciplinare_PubblicoPrivato = 0
            Mezzo = 0
            Modalita = 0
            Extra_Int = 0

            Scadenza = AGRODATAFINE
            Data_Registrazione = AGRODATAINIZIO
            Extra_Str = ""
            Extra_Date = AGRODATAINIZIO
            Doc_Numero = 0
            Doc_Numero_Des = ""
            Doc_Numero_Sin = ""
            Tipo_Sconto = 0
            Cod_IndirizzoRisUm = 0
            Cod_Destinazione = 0
            Cod_IndirizzoDestinazione = 0
            Cod_Vettore = 0
            Cod_IndirizzoVettore = 0
            Causale_Trasporto = ""
            Aspetto = ""
            Peso = 0
            Colli = 0
            Natura_Beni = ""
            Tara_Veicolo = 0
            Tara_Imballi = 0
            Tipo_Peso = 0

            Username_Note = ""
            Scadenza_Extra = AGRODATAINIZIO

            Progr_Protocollo = 0
            Progr_Registrazione = 0
            ChkLayOut_Bypass_Fatturato = 0
            ChkLayOut_Join_Prodotti = 0

            Cod_RisUm_Altro = 0
            ChkLayOut_Peso = 0
            ChkLayOut_Prezzo = 0
            ChkFiltro_Varietale = 0
            ChkLayOut_Litri = 0

            Sezionale_Cod = 0
            Causale_Trasporto_Cod = 0
            TipoDocumento = 0

            Ora = AGRODATAFINE
            OraFine = AGRODATAFINE

            Cod_RisUm_Aggiuntivo = 0
            Cod_Indirizzo_Aggiuntivo = 0
            ChkLayOut_Riscontrato = 0
            Doc_Numero_Visualizzato = ""
            Cod_Macchina_Lav = ""

            'valori che poi il DAL in fase di scrittura sostituirà con quelli reali
            Data_Creazione = #2/1/1900#
            Data_Modifica = #2/1/1900#
            Username_Creazione = ""
            Username_Modifica = ""

            BaseCode = 0
            TopCode = 200000000

            Modalita_Applicazione = 0

            _Movimenti_Dettagli_Tecnici = New List(Of Movimento_Dettaglio_Tecnico)
            _Movimenti_Dettagli_Tecnici_Extra = New List(Of Movimento_Dettaglio_Tecnico_Extra)
            _Movimenti_Dettagli = New List(Of Movimento_Dettaglio)
            _Pagamenti = New List(Of Pagamento)
        End Sub

        Sub New(ByVal pivaInput As String, ByVal lavCod As Integer, ByVal dataOperazione As Date)

            Piva = pivaInput
            Sa_Cod = 0
            Id_Agenda = 0
            Id_Mov = 0
            Data = dataOperazione
            Lav_Cod = lavCod
            Cau_Mov = ""
            Cod_Risum = 0
            Mov_Desc = ""
            Num_Protocollo = 0
            Num_Protocollo_Decimal = 0
            Disciplinare_PubblicoPrivato = 0
            Mezzo = 0
            Modalita = 0
            Extra_Int = 0

            Scadenza = AGRODATAFINE
            Data_Registrazione = AGRODATAINIZIO
            Extra_Str = ""
            Extra_Date = AGRODATAINIZIO
            Doc_Numero = 0
            Doc_Numero_Des = ""
            Doc_Numero_Sin = ""
            Tipo_Sconto = 0
            Cod_IndirizzoRisUm = 0
            Cod_Destinazione = 0
            Cod_IndirizzoDestinazione = 0
            Cod_Vettore = 0
            Cod_IndirizzoVettore = 0
            Causale_Trasporto = ""
            Aspetto = ""
            Peso = 0
            Colli = 0
            Natura_Beni = ""
            Tara_Veicolo = 0
            Tara_Imballi = 0
            Tipo_Peso = 0
            Username_Note = ""
            Scadenza_Extra = AGRODATAINIZIO

            Progr_Protocollo = 0
            Progr_Registrazione = 0
            ChkLayOut_Bypass_Fatturato = 0
            ChkLayOut_Join_Prodotti = 0

            Cod_RisUm_Altro = 0
            ChkLayOut_Peso = 0
            ChkLayOut_Prezzo = 0
            ChkFiltro_Varietale = 0
            ChkLayOut_Litri = 0

            Sezionale_Cod = 0
            Causale_Trasporto_Cod = 0

            Ora = AGRODATAFINE
            OraFine = AGRODATAFINE

            Cod_RisUm_Aggiuntivo = 0
            Cod_Indirizzo_Aggiuntivo = 0
            ChkLayOut_Riscontrato = 0

            Doc_Numero_Visualizzato = ""
            Cod_Macchina_Lav = ""

            'valori che poi il DAL in fase di scrittura sostituirà con quelli reali
            Data_Creazione = #2/1/1900#
            Data_Modifica = #2/1/1900#
            Username_Creazione = ""
            Username_Modifica = ""

            BaseCode = 0
            TopCode = 200000000

            _Movimenti_Dettagli_Tecnici = New List(Of Movimento_Dettaglio_Tecnico)
            _Movimenti_Dettagli_Tecnici_Extra = New List(Of Movimento_Dettaglio_Tecnico_Extra)
            _Movimenti_Dettagli = New List(Of Movimento_Dettaglio)
            _Pagamenti = New List(Of Pagamento)
        End Sub

        Public Property Lav_Cod As Integer

        Public Property Piva As String

        Public Property Sa_Cod As Integer

        Public Property Id_Agenda As Integer

        Public Property Id_Mov As Integer

        Public Property Cod_Risum As Integer

        Public Property Cau_Mov As String

        Public Property Mov_Desc As String

        Public Property Data As Date

        Public Property Scadenza As Date

        Public Property Doc_Numero As Decimal

        Public Property Num_Protocollo As Integer

        Public Property Num_Protocollo_Decimal As Decimal

        Public Property Cod_IndirizzoRisUm As Integer

        Public Property Cod_Destinazione As Integer

        Public Property Cod_IndirizzoDestinazione As Integer

        Public Property TipoDocumento As Integer

        Public Property Mezzo As Integer

        Public Property Cod_Vettore As Integer

        Public Property Cod_IndirizzoVettore As Integer

        Public Property Causale_Trasporto As String

        Public Property Aspetto As String

        Public Property Peso As Decimal

        Public Property Ora As DateTime

        Public Property OraFine As DateTime

        Public Property Colli As Integer

        Public Property Extra_Str As String

        Public Property Extra_Int As Integer

        Public Property Extra_Date As Date

        Public Property Tipo_Sconto As Integer

        Public Property Doc_Numero_Des As String

        Public Property Natura_Beni As String

        Public Property Tara_Veicolo As Decimal

        Public Property Tara_Imballi As Decimal

        Public Property Tipo_Peso As Integer

        Public Property Modalita As Integer

        Public Property Username_Note As String

        Public Property Scadenza_Extra As DateTime

        Public Property Doc_Numero_Sin As String

        Public Property Progr_Protocollo As Integer

        Public Property Progr_Registrazione As Integer

        Public Property Data_Registrazione As Date

        Public Property ChkLayOut_Bypass_Fatturato As Integer

        Public Property ChkLayOut_Join_Prodotti As Integer

        Public Property Cod_RisUm_Altro As Integer

        Public Property ChkLayOut_Peso As Integer

        Public Property ChkLayOut_Prezzo As Integer

        Public Property ChkFiltro_Varietale As Integer

        Public Property Disciplinare_PubblicoPrivato As Integer

        Public Property Sezionale_Cod As Integer

        Public Property Causale_Trasporto_Cod As Integer

        Public Property ChkLayOut_Litri As Integer

        Public Property Cod_RisUm_Aggiuntivo As Integer

        Public Property Cod_Indirizzo_Aggiuntivo As Integer

        Public Property ChkLayOut_Riscontrato As Integer

        Public Property Doc_Numero_Visualizzato As String

        Public Property Cod_Macchina_Lav As String

        Public Property Data_Creazione As DateTime
        
        Public Property Data_Modifica As DateTime

        Public Property Username_Creazione As String

        Public Property Username_Modifica As String

        Public Property TopCode As Integer

        Public Property BaseCode As Integer

        Public Property Modalita_Applicazione As Integer

        Public Property Movimenti_Dettagli_Tecnici() As List(Of Movimento_Dettaglio_Tecnico)
            Get
                Return _Movimenti_Dettagli_Tecnici
            End Get
            Set(ByVal value As List(Of Movimento_Dettaglio_Tecnico))
                _Movimenti_Dettagli_Tecnici = value
            End Set
        End Property

        Public Property Movimenti_Dettagli_Tecnici_Extra() As List(Of Movimento_Dettaglio_Tecnico_Extra)
            Get
                Return _Movimenti_Dettagli_Tecnici_Extra
            End Get
            Set(ByVal value As List(Of Movimento_Dettaglio_Tecnico_Extra))
                _Movimenti_Dettagli_Tecnici_Extra = value
            End Set
        End Property

        Public Property Movimenti_Dettagli() As List(Of Movimento_Dettaglio)
            Get
                Return _Movimenti_Dettagli
            End Get
            Set(ByVal value As List(Of Movimento_Dettaglio))
                _Movimenti_Dettagli = value
            End Set
        End Property

        Public Property Pagamenti() As List(Of Pagamento)
            Get
                Return _Pagamenti
            End Get
            Set(ByVal value As List(Of Pagamento))
                _Pagamenti = value
            End Set
        End Property
    End Class

    Public Class Agenda_Movimenti_Helper

        Public Function Scrivi(ByVal Movimento As Movimento,
                               ByVal objParametri As AgronicaCoreParametri,
                               Optional ByVal flagUsaOraReale As Boolean = False,
                               Optional ByVal flagScriviSempreLotto As Boolean = False,
                               Optional ByVal documentoPrevisionale As Boolean = False
                               ) As Boolean

            Dim flagConnessione As Boolean = False
            Dim flagTransazione As Boolean = False

            Dim idMov As Integer
            Dim i As Integer

            Try

                Utility.VerificaApriTransazione(objParametri, flagConnessione, flagTransazione)

                '---------------------------
                'Movimenti
                '---------------------------

                idMov = Movimento.Id_Mov

                If idMov <= 0 Then

                    Dim objSequenze As New Agro_Sequenze

                    idMov = objSequenze.NuovoId_Tabella("Movimenti",
                                                         Movimento.BaseCode,
                                                         Movimento.TopCode,
                                                         objParametri)

                    objSequenze = Nothing

                End If

                Dim objMovimenti As New AgronicaCoreContabDAL.Movimenti_W

                'Giulia, 08/08/2016 11.01.22: in fase di scrittura scriveva correttamente un decimal, ma in lettura leggeva un integer
                '                               quanto sotto è per assicurarsi che venga scritto il dato giusto, 
                '                               anche se è solo valorizzato Num_Protocollo e non Num_Protocollo_Decimal
                If Movimento.Num_Protocollo_Decimal <> CDec(Movimento.Num_Protocollo) AndAlso Movimento.Num_Protocollo <> 0 Then
                    Movimento.Num_Protocollo_Decimal = CDec(Movimento.Num_Protocollo)
                End If

                'Mantengo Data_Creazione e Username_creazione, mentre le info sulle modifiche non le passo, così vengono aggiornate
                objMovimenti.Scrivi(Movimento.Piva,
                                    Movimento.Sa_Cod,
                                    Movimento.Id_Agenda,
                                    idMov,
                                    Movimento.Cod_Risum,
                                    Movimento.Cau_Mov,
                                    Movimento.Mov_Desc,
                                    Movimento.Data,
                                    Movimento.Scadenza,
                                    Movimento.Scadenza_Extra,
                                    Movimento.Doc_Numero,
                                    Movimento.Num_Protocollo_Decimal,
                                    Movimento.Cod_IndirizzoRisUm,
                                    Movimento.Cod_Destinazione,
                                    Movimento.Cod_IndirizzoDestinazione,
                                    Movimento.Mezzo,
                                    Movimento.Cod_Vettore,
                                    Movimento.Cod_IndirizzoVettore,
                                    Movimento.Causale_Trasporto,
                                    Movimento.Aspetto,
                                    Movimento.Peso,
                                    Movimento.Ora,
                                    Movimento.Colli,
                                    Movimento.Tipo_Sconto,
                                    Movimento.Extra_Str,
                                    Movimento.Extra_Int,
                                    Movimento.Extra_Date,
                                    Movimento.Doc_Numero_Sin,
                                    Movimento.Doc_Numero_Des,
                                    Movimento.Natura_Beni,
                                    Movimento.Tara_Veicolo,
                                    Movimento.Tara_Imballi,
                                    Movimento.Tipo_Peso,
                                    Movimento.Modalita,
                                    Movimento.Username_Note,
                                    Movimento.Progr_Protocollo,
                                    Movimento.Progr_Registrazione,
                                    Movimento.Data_Registrazione,
                                    Movimento.ChkLayOut_Bypass_Fatturato,
                                    Movimento.ChkLayOut_Join_Prodotti,
                                    Movimento.Data,
                                    AGRODATAFINE,
                                    Movimento.Disciplinare_PubblicoPrivato,
                                    objParametri,
                                    Cod_RisUm_Altro:=Movimento.Cod_RisUm_Altro,
                                    ChkLayOut_Peso:=Movimento.ChkLayOut_Peso,
                                    ChkLayOut_Prezzo:=Movimento.ChkLayOut_Prezzo,
                                    ChkFiltro_Varietale:=Movimento.ChkFiltro_Varietale,
                                    Causale_Trasporto_Cod:=Movimento.Causale_Trasporto_Cod,
                                    Sezionale_Cod:=Movimento.Sezionale_Cod,
                                    ChkLayOut_Litri:=Movimento.ChkLayOut_Litri,
                                    Flag_Usa_Ora_Reale:=flagUsaOraReale,
                                    Cod_RisUm_Aggiuntivo:=Movimento.Cod_RisUm_Aggiuntivo,
                                    Cod_IndirizzoAggiuntivo:=Movimento.Cod_Indirizzo_Aggiuntivo,
                                    ChkLayOut_Riscontrato:=Movimento.ChkLayOut_Riscontrato,
                                    Doc_Numero_Visualizzato:=Movimento.Doc_Numero_Visualizzato,
                                    Cod_Macchina_Lav:=Movimento.Cod_Macchina_Lav,
                                    TipoDocumento:=Movimento.TipoDocumento,
                                    Data_creazione:=Movimento.Data_Creazione,
                                    username_creazione:=Movimento.Username_Creazione,
                                    OraFine:=Movimento.OraFine,
                                    Modalita_Applicazione:=Movimento.Modalita_Applicazione)


                objMovimenti = Nothing

                '---------------------------
                'MOVIMENTI_DETTAGLI_TECNICI 

                If Not IsNothing(Movimento.Movimenti_Dettagli_Tecnici) AndAlso Movimento.Movimenti_Dettagli_Tecnici.Count > 0 Then

                    For i = 0 To Movimento.Movimenti_Dettagli_Tecnici.Count - 1

                        Movimento.Movimenti_Dettagli_Tecnici(i).Id_Agenda = Movimento.Id_Agenda
                        Movimento.Movimenti_Dettagli_Tecnici(i).Id_Mov = idMov
                        Movimento.Movimenti_Dettagli_Tecnici(i).Id_Mov_Det = 0

                        Dim objMovDettTecn As New Agenda_Movimenti_Dettagli_Tecnici_Helper
                        objMovDettTecn.Scrivi(Movimento.Movimenti_Dettagli_Tecnici(i), objParametri)
                        objMovDettTecn = Nothing

                    Next

                End If


                '---------------------------
                'MOVIMENTI_DETTAGLI_tecnici_extra 

                If Not IsNothing(Movimento.Movimenti_Dettagli_Tecnici_Extra) AndAlso Movimento.Movimenti_Dettagli_Tecnici_Extra.Count > 0 Then

                    For i = 0 To Movimento.Movimenti_Dettagli_Tecnici_Extra.Count - 1

                        Movimento.Movimenti_Dettagli_Tecnici_Extra(i).Id_Agenda = Movimento.Id_Agenda
                        Movimento.Movimenti_Dettagli_Tecnici_Extra(i).Id_Mov = idMov
                        Movimento.Movimenti_Dettagli_Tecnici_Extra(i).Id_Mov_Det = 0

                        Dim objMovDettTecn As New Agenda_Movimenti_Dettagli_Tecnici_extra_Helper
                        objMovDettTecn.Scrivi(Movimento.Movimenti_Dettagli_Tecnici_Extra(i), objParametri)
                        objMovDettTecn = Nothing

                    Next

                End If


                '---------------------------
                'MOVIMENTI_DETTAGLI 

                If Not IsNothing(Movimento.Movimenti_Dettagli) AndAlso Movimento.Movimenti_Dettagli.Count > 0 Then

                    For i = 0 To Movimento.Movimenti_Dettagli.Count - 1

                        Movimento.Movimenti_Dettagli(i).Id_Agenda = Movimento.Id_Agenda
                        Movimento.Movimenti_Dettagli(i).Id_Mov = idMov

                        Dim objMovDet As New Agenda_Movimenti_Dettagli_Helper
                        Movimento.Movimenti_Dettagli(i).Id_Mov_Det = objMovDet.Scrivi(Movimento.Movimenti_Dettagli(i), objParametri, flagScriviSempreLotto, documentoPrevisionale)
                        objMovDet = Nothing

                    Next

                End If

                '---------------------------
                'PAGAMENTI

                If Not IsNothing(Movimento.Pagamenti) AndAlso Movimento.Pagamenti.Count > 0 Then

                    For i = 0 To Movimento.Pagamenti.Count - 1

                        Movimento.Pagamenti(i).Id_Agenda = Movimento.Id_Agenda
                        Movimento.Pagamenti(i).Id_Mov = idMov

                        Dim objPagamenti As New Agenda_Pagamenti_Helper
                        objPagamenti.Scrivi(Movimento.Pagamenti(i), objParametri)
                        objPagamenti = Nothing

                    Next

                End If

                Utility.VerificaChiudiTransazione(objParametri, flagTransazione)

            Catch ex As Exception

                Utility.VerificaAnnullaTransazione(objParametri, flagTransazione)

                Throw New Exception("[ Agenda_Movimenti_Helper.Scrivi() ] : " & ex.Message)
            Finally

                Utility.VerificaChiudiConnessione(objParametri, flagConnessione)

            End Try

            Return True

        End Function

        Public Function Cancella(ByVal piva As String,
                                 ByVal saCodAgenda As Integer,
                                 ByVal idAgenda As Integer,
                                 ByVal idMov As Integer,
                                 ByVal objParametri As AgronicaCoreParametri
                                 ) As Boolean

            Dim flagConnessione As Boolean = False
            Dim flagTransazione As Boolean = False

            Try

                Utility.VerificaApriTransazione(objParametri, flagConnessione, flagTransazione)

                '---------------------------
                'MOVIMENTI_DETTAGLI_TECNICI 

                Dim objMovDettTecn As New Agenda_Movimenti_Dettagli_Tecnici_Helper
                objMovDettTecn.Cancella(piva, 0, idAgenda, idMov, 0, 0, objParametri)
                objMovDettTecn = Nothing


                Dim objMovDettTecnExtra As New Agenda_Movimenti_Dettagli_Tecnici_extra_Helper
                objMovDettTecnExtra.Cancella(piva, 0, idAgenda, idMov, 0, 0, objParametri)
                objMovDettTecnExtra = Nothing

                If True Then

                    '---------------------------
                    'MOVIMENTI_DETTAGLI 

                    Dim objMovDet As New Agenda_Movimenti_Dettagli_Helper
                    objMovDet.Cancella(piva, 0, idAgenda, idMov, 0, objParametri)
                    objMovDet = Nothing

                    '---------------------------
                    'PAGAMENTI

                    Dim objPagamenti As New Agenda_Pagamenti_Helper
                    objPagamenti.Cancella(piva, 0, idAgenda, idMov, 0, objParametri)
                    objPagamenti = Nothing


                    If True Then

                        '---------------------------
                        'Movimenti
                        '---------------------------

                        Dim objMovimenti As New AgronicaCoreContabDAL.Movimenti_W

                        objMovimenti.Cancella(piva,
                                              0,
                                              idAgenda,
                                              idMov,
                                              "",
                                              objParametri)

                        objMovimenti = Nothing

                    End If


                End If

                Utility.VerificaChiudiTransazione(objParametri, flagTransazione)

            Catch ex As Exception

                Utility.VerificaAnnullaTransazione(objParametri, flagTransazione)
                Throw New Exception("[ Agenda_Movimenti_Helper.Cancella() ] : " & ex.Message)

            Finally

                Utility.VerificaChiudiConnessione(objParametri, flagConnessione)

            End Try

            Return True

        End Function

        Public Function ModificaPuntuale(ByVal piva As String,
                                         ByVal saCodAgenda As Integer,
                                         ByVal idAgenda As Integer,
                                         ByVal idMov As Integer,
                                         ByVal objParametri As AgronicaCoreParametri,
                                         ByVal objMovimento As Movimento
                                         ) As Boolean

            Dim flagConnessione As Boolean = False
            Dim flagTransazione As Boolean = False
            Dim xRisp As Boolean = False

            Try

                Utility.VerificaApriTransazione(objParametri, flagConnessione, flagTransazione)

                Dim objMovW As New AgronicaCoreContabDAL.Movimenti_W

                xRisp = objMovW.ModificaPuntuale(piva, saCodAgenda, idAgenda, idMov, objParametri,
                                                 Cod_RisUm:=objMovimento.Cod_Risum,
                                                 Cau_Mov:=objMovimento.Cau_Mov,
                                                 Mov_Desc:=objMovimento.Mov_Desc,
                                                 Data_Movimento:=objMovimento.Data,
                                                 Scadenza:=objMovimento.Scadenza,
                                                 Scadenza_Extra:=objMovimento.Scadenza_Extra,
                                                 Doc_Numero:=objMovimento.Doc_Numero,
                                                 Num_Protocollo:=objMovimento.Num_Protocollo,
                                                 Cod_IndirizzoRisUm:=objMovimento.Cod_IndirizzoRisUm,
                                                 Cod_Destinazione:=objMovimento.Cod_Destinazione,
                                                 Cod_IndirizzoDestinazione:=objMovimento.Cod_IndirizzoDestinazione,
                                                 Mezzo:=objMovimento.Mezzo,
                                                 Cod_Vettore:=objMovimento.Cod_Vettore,
                                                 Cod_IndirizzoVettore:=objMovimento.Cod_IndirizzoVettore,
                                                 Causale_Trasporto:=objMovimento.Causale_Trasporto,
                                                 Aspetto:=objMovimento.Aspetto,
                                                 Peso:=objMovimento.Peso,
                                                 Ora:=objMovimento.Ora,
                                                 Colli:=objMovimento.Colli,
                                                 Tipo_Sconto:=objMovimento.Tipo_Sconto,
                                                 Extra_Str:=objMovimento.Extra_Str,
                                                 Extra_Int:=objMovimento.Extra_Int,
                                                 Extra_Date:=objMovimento.Extra_Date,
                                                 Doc_Numero_Sin:=objMovimento.Doc_Numero_Sin,
                                                 Doc_Numero_Des:=objMovimento.Doc_Numero_Des,
                                                 Natura_Beni:=objMovimento.Natura_Beni,
                                                 Tara_Veicolo:=objMovimento.Tara_Veicolo,
                                                 Tara_Imballi:=objMovimento.Tara_Imballi,
                                                 Tipo_Peso:=objMovimento.Tipo_Peso,
                                                 Modalita:=objMovimento.Modalita,
                                                 Username_Note:=objMovimento.Username_Note,
                                                 Progr_Protocollo:=objMovimento.Progr_Protocollo,
                                                 Progr_Registrazione:=objMovimento.Progr_Registrazione,
                                                 Data_Registrazione:=objMovimento.Data_Registrazione,
                                                 ChkLayOut_Bypass_Fatturato:=objMovimento.ChkLayOut_Bypass_Fatturato,
                                                 ChkLayOut_Join_Prodotti:=objMovimento.ChkLayOut_Join_Prodotti,
                                                 Disciplinare_PubblicoPrivato:=objMovimento.Disciplinare_PubblicoPrivato,
                                                 Cod_RisUm_Altro:=objMovimento.Cod_RisUm_Altro,
                                                 ChkLayOut_Peso:=objMovimento.ChkLayOut_Peso,
                                                 ChkLayOut_Prezzo:=objMovimento.ChkLayOut_Prezzo,
                                                 ChkFiltro_Varietale:=objMovimento.ChkFiltro_Varietale,
                                                 Sezionale_Cod:=objMovimento.Sezionale_Cod,
                                                 Causale_Trasporto_Cod:=objMovimento.Causale_Trasporto_Cod,
                                                 ChkLayOut_Litri:=objMovimento.ChkLayOut_Litri,
                                                 Cod_RisUm_Aggiuntivo:=objMovimento.Cod_RisUm_Aggiuntivo,
                                                 Cod_Indirizzo_Aggiuntivo:=objMovimento.Cod_Indirizzo_Aggiuntivo,
                                                 ChkLayOut_Riscontrato:=objMovimento.ChkLayOut_Riscontrato,
                                                 Doc_Numero_Visualizzato:=objMovimento.Doc_Numero_Visualizzato,
                                                 Cod_Macchina_Lav:=objMovimento.Cod_Macchina_Lav,
                                                 Flag_Usa_Ora_Reale:=True,
                                                 Data_Modifica:=objMovimento.Data_Modifica,
                                                 Username_Modifica:=objMovimento.Username_Modifica)

                objMovW = Nothing

                Utility.VerificaChiudiTransazione(objParametri, flagTransazione)

            Catch ex As Exception

                Utility.VerificaAnnullaTransazione(objParametri, flagTransazione)
                xRisp = False
                Throw New Exception("[ Agenda_Movimenti_Helper.ModificaPuntuale() ] : " & ex.Message)

            Finally

                Utility.VerificaChiudiConnessione(objParametri, flagConnessione)

            End Try

            Return xRisp
            
        End Function

        Public Function ModificaPuntuale(ByVal piva As String,
                                         ByVal saCodAgenda As Integer,
                                         ByVal idAgenda As Integer,
                                         ByVal idMov As Integer,
                                         ByVal objParametri As AgronicaCoreParametri,
                                         Optional ByVal codRisUm As Integer? = Nothing,
                                         Optional ByVal cauMov As String = Nothing,
                                         Optional ByVal movDesc As String = Nothing,
                                         Optional ByVal dataMovimento As Date? = Nothing,
                                         Optional ByVal Scadenza As Date? = Nothing,
                                         Optional ByVal Scadenza_Extra As Date? = Nothing,
                                         Optional ByVal Doc_Numero As Decimal? = Nothing,
                                         Optional ByVal Num_Protocollo As Decimal? = Nothing,
                                         Optional ByVal Cod_IndirizzoRisUm As Integer? = Nothing,
                                         Optional ByVal Cod_Destinazione As Integer? = Nothing,
                                         Optional ByVal Cod_IndirizzoDestinazione As Integer? = Nothing,
                                         Optional ByVal mezzo As Integer? = Nothing,
                                         Optional ByVal codVettore As Integer? = Nothing,
                                         Optional ByVal codIndirizzoVettore As Integer? = Nothing,
                                         Optional ByVal causaleTrasporto As String = Nothing,
                                         Optional ByVal aspetto As String = Nothing,
                                         Optional ByVal peso As Decimal? = Nothing,
                                         Optional ByVal ora As DateTime? = Nothing,
                                         Optional ByVal colli As Integer? = Nothing,
                                         Optional ByVal tipoSconto As Integer? = Nothing,
                                         Optional ByVal extraStr As String = Nothing,
                                         Optional ByVal extraInt As Integer? = Nothing,
                                         Optional ByVal extraDate As Date? = Nothing,
                                         Optional ByVal docNumeroSin As String = Nothing,
                                         Optional ByVal docNumeroDes As String = Nothing,
                                         Optional ByVal naturaBeni As String = Nothing,
                                         Optional ByVal taraVeicolo As Decimal? = Nothing,
                                         Optional ByVal taraImballi As Decimal? = Nothing,
                                         Optional ByVal tipoPeso As Integer? = Nothing,
                                         Optional ByVal modalita As Integer? = Nothing,
                                         Optional ByVal usernameNote As String = Nothing,
                                         Optional ByVal progrProtocollo As Integer? = Nothing,
                                         Optional ByVal progrRegistrazione As Integer? = Nothing,
                                         Optional ByVal dataRegistrazione As DateTime? = Nothing,
                                         Optional ByVal chkLayOutBypassFatturato As Integer? = Nothing,
                                         Optional ByVal chkLayOutJoinProdotti As Integer? = Nothing,
                                         Optional ByVal validitaInizio As Date? = Nothing,
                                         Optional ByVal validitaFine As Date? = Nothing,
                                         Optional ByVal disciplinarePubblicoPrivato As Integer? = Nothing,
                                         Optional ByVal codRisUmAltro As Integer? = Nothing,
                                         Optional ByVal chkLayOutPeso As Integer? = Nothing,
                                         Optional ByVal chkLayOutPrezzo As Integer? = Nothing,
                                         Optional ByVal chkFiltroVarietale As Integer? = Nothing,
                                         Optional ByVal sezionaleCod As Integer? = Nothing,
                                         Optional ByVal causaleTrasportoCod As Integer? = Nothing,
                                         Optional ByVal chkLayOutLitri As Integer? = Nothing,
                                         Optional ByVal codRisUmAggiuntivo As Integer? = Nothing,
                                         Optional ByVal codIndirizzoAggiuntivo As Integer? = Nothing,
                                         Optional ByVal chkLayOutRiscontrato As Integer? = Nothing,
                                         Optional ByVal xFiltroAggiuntivo As String = "",
                                         Optional ByVal flagUsaOraReale As Boolean = False,
                                         Optional ByVal dataModifica As DateTime = #2/1/1900#,
                                         Optional ByVal usernameModifica As String = "",
                                         Optional ByVal docNumeroVisualizzato As String = Nothing,
                                         Optional ByVal codMacchinaLav As String = Nothing,
                                         Optional ByVal TipoDocumento As Integer? = Nothing,
                                         Optional ByVal oraFine As DateTime? = Nothing
                                         ) As Boolean

            Dim flagConnessione As Boolean = False
            Dim flagTransazione As Boolean = False
            Dim xRisp As Boolean = False

            Try

                Utility.VerificaApriTransazione(objParametri, flagConnessione, flagTransazione)

                Dim objMovW As New AgronicaCoreContabDAL.Movimenti_W

                xRisp = objMovW.ModificaPuntuale(piva, saCodAgenda, idAgenda, idMov, objParametri,
                                                 Cod_RisUm:=codRisUm,
                                                 Cau_Mov:=cauMov,
                                                 Mov_Desc:=movDesc,
                                                 Data_Movimento:=dataMovimento,
                                                 Scadenza:=Scadenza,
                                                 Scadenza_Extra:=Scadenza_Extra,
                                                 Doc_Numero:=Doc_Numero,
                                                 Num_Protocollo:=Num_Protocollo,
                                                 Cod_IndirizzoRisUm:=Cod_IndirizzoRisUm,
                                                 Cod_Destinazione:=Cod_Destinazione,
                                                 Cod_IndirizzoDestinazione:=Cod_IndirizzoDestinazione,
                                                 Mezzo:=mezzo,
                                                 Cod_Vettore:=codVettore,
                                                 Cod_IndirizzoVettore:=codIndirizzoVettore,
                                                 Causale_Trasporto:=causaleTrasporto,
                                                 Aspetto:=aspetto,
                                                 Peso:=peso,
                                                 Ora:=ora,
                                                 Colli:=colli,
                                                 Tipo_Sconto:=tipoSconto,
                                                 Extra_Str:=extraStr,
                                                 Extra_Int:=extraInt,
                                                 Extra_Date:=extraDate,
                                                 Doc_Numero_Sin:=docNumeroSin,
                                                 Doc_Numero_Des:=docNumeroDes,
                                                 Natura_Beni:=naturaBeni,
                                                 Tara_Veicolo:=taraVeicolo,
                                                 Tara_Imballi:=taraImballi,
                                                 Tipo_Peso:=tipoPeso,
                                                 Modalita:=modalita,
                                                 Username_Note:=usernameNote,
                                                 Progr_Protocollo:=progrProtocollo,
                                                 Progr_Registrazione:=progrRegistrazione,
                                                 Data_Registrazione:=dataRegistrazione,
                                                 ChkLayOut_Bypass_Fatturato:=chkLayOutBypassFatturato,
                                                 ChkLayOut_Join_Prodotti:=chkLayOutJoinProdotti,
                                                 Validita_Inizio:=validitaInizio,
                                                 Validita_Fine:=validitaFine,
                                                 Disciplinare_PubblicoPrivato:=disciplinarePubblicoPrivato,
                                                 Cod_RisUm_Altro:=codRisUmAltro,
                                                 ChkLayOut_Peso:=chkLayOutPeso,
                                                 ChkLayOut_Prezzo:=chkLayOutPrezzo,
                                                 ChkFiltro_Varietale:=chkFiltroVarietale,
                                                 Sezionale_Cod:=sezionaleCod,
                                                 Causale_Trasporto_Cod:=causaleTrasportoCod,
                                                 ChkLayOut_Litri:=chkLayOutLitri,
                                                 Cod_RisUm_Aggiuntivo:=codRisUmAggiuntivo,
                                                 Cod_Indirizzo_Aggiuntivo:=codIndirizzoAggiuntivo,
                                                 ChkLayOut_Riscontrato:=chkLayOutRiscontrato,
                                                 Doc_Numero_Visualizzato:=docNumeroVisualizzato,
                                                 Cod_Macchina_Lav:=codMacchinaLav,
                                                 TipoDocumento:=TipoDocumento,
                                                 xFiltroAggiuntivo:=xFiltroAggiuntivo,
                                                 Flag_Usa_Ora_Reale:=True,
                                                 Data_Modifica:=dataModifica,
                                                 Username_Modifica:=usernameModifica,
                                                 OraFine:=oraFine)

                objMovW = Nothing

                Utility.VerificaChiudiTransazione(objParametri, flagTransazione)

            Catch ex As Exception

                Utility.VerificaAnnullaTransazione(objParametri, flagTransazione)
                xRisp = False
                Throw New Exception("[ Agenda_Movimenti_Helper.ModificaPuntuale() ] : " & ex.Message)

            Finally

                Utility.VerificaChiudiConnessione(objParametri, flagConnessione)

            End Try

            Return xRisp

        End Function

        Public Function Leggi(ByVal piva As String,
                              ByVal saCodAgenda As Integer,
                              ByVal idAgenda As Integer,
                              ByVal objParametri As AgronicaCoreParametri,
                              Optional ByVal opzioniLetturaMovimenti As Opzioni_Lettura_Movimenti = Nothing
                              ) As List(Of Movimento)

            If IsNothing(opzioniLetturaMovimenti) Then
                opzioniLetturaMovimenti = New Opzioni_Lettura_Movimenti
            End If

            Dim flagConnessione As Boolean = False

            Dim listaMovimenti As New List(Of Movimento)
            Dim Movimento As Movimento

            Try

                Utility.VerificaApriConnessione(objParametri, flagConnessione)

                '--------------------------------------------------------
                '-------- MOVIMENTI -------------------------------------
                '--------------------------------------------------------
                Dim objMovimenti = New AgronicaCoreContabDAL.Movimenti_R
                Dim Dt_Movimenti As DataTable

                'modificato con Sa_Cod = 0 per leggere anche i magazzini esterni a quel centro aziendale
                Dt_Movimenti = objMovimenti.Leggi(CStr(piva),
                                                  CInt(0),
                                                  CInt(idAgenda),
                                                  0,
                                                  0,
                                                  "",
                                                  enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                  "",
                                                  "",
                                                  objParametri)

                objMovimenti = Nothing

                If Dt_Movimenti.Rows.Count > 0 Then

                    For j = 0 To Dt_Movimenti.Rows.Count - 1

                        Movimento = New Movimento With {
                            .Piva = Dt_Movimenti.Rows(j).Item("Piva"),
                            .Sa_Cod = Dt_Movimenti.Rows(j).Item("Sa_Cod"),
                            .Id_Agenda = Dt_Movimenti.Rows(j).Item("Id_Agenda"),
                            .Id_Mov = Dt_Movimenti.Rows(j).Item("Id_Mov"),
                            .Cod_Risum = Dt_Movimenti.Rows(j).Item("Cod_Risum"),
                            .Cau_Mov = Dt_Movimenti.Rows(j).Item("Cau_Mov"),
                            .Mov_Desc = Dt_Movimenti.Rows(j).Item("Mov_Desc"),
                            .Data = Dt_Movimenti.Rows(j).Item("Data_Movimento"),
                            .Scadenza = Dt_Movimenti.Rows(j).Item("Scadenza"),
                            .Doc_Numero = Dt_Movimenti.Rows(j).Item("Doc_Numero"),
                            .Doc_Numero_Des = Dt_Movimenti.Rows(j).Item("Doc_Numero_Des"),
                            .Doc_Numero_Sin = Dt_Movimenti.Rows(j).Item("Doc_Numero_Sin"),
                            .Num_Protocollo = Dt_Movimenti.Rows(j).Item("Num_Protocollo"),
                            .Num_Protocollo_Decimal = Dt_Movimenti.Rows(j).Item("Num_Protocollo"),
                            .Cod_IndirizzoRisUm = Dt_Movimenti.Rows(j).Item("Cod_IndirizzoRisUm"),
                            .Cod_Destinazione = Dt_Movimenti.Rows(j).Item("Cod_Destinazione"),
                            .Cod_IndirizzoDestinazione = Dt_Movimenti.Rows(j).Item("Cod_IndirizzoDestinazione"),
                            .Mezzo = Dt_Movimenti.Rows(j).Item("Mezzo"),
                            .Cod_Vettore = Dt_Movimenti.Rows(j).Item("Cod_Vettore"),
                            .Cod_IndirizzoVettore = Dt_Movimenti.Rows(j).Item("Cod_IndirizzoVettore"),
                            .Causale_Trasporto = Dt_Movimenti.Rows(j).Item("Causale_Trasporto"),
                            .Aspetto = Dt_Movimenti.Rows(j).Item("Aspetto"),
                            .Peso = Dt_Movimenti.Rows(j).Item("Peso"),
                            .Ora = Dt_Movimenti.Rows(j).Item("Ora"),
                            .Colli = Dt_Movimenti.Rows(j).Item("Colli"),
                            .Extra_Str = Dt_Movimenti.Rows(j).Item("Extra_Str"),
                            .Extra_Int = Dt_Movimenti.Rows(j).Item("Extra_Int"),
                            .Extra_Date = Dt_Movimenti.Rows(j).Item("Extra_Date"),
                            .Tipo_Sconto = Dt_Movimenti.Rows(j).Item("Tipo_Sconto"),
                            .Natura_Beni = Dt_Movimenti.Rows(j).Item("Natura_Beni"),
                            .Tara_Veicolo = Dt_Movimenti.Rows(j).Item("Tara_Veicolo"),
                            .Tara_Imballi = Dt_Movimenti.Rows(j).Item("Tara_Imballi"),
                            .Modalita = Dt_Movimenti.Rows(j).Item("Modalita"),
                            .Scadenza_Extra = Dt_Movimenti.Rows(j).Item("Scadenza_Extra"),
                            .Progr_Protocollo = Dt_Movimenti.Rows(j).Item("Progr_Protocollo"),
                            .Progr_Registrazione = Dt_Movimenti.Rows(j).Item("Progr_Registrazione"),
                            .Data_Registrazione = Dt_Movimenti.Rows(j).Item("Data_Registrazione"),
                            .Disciplinare_PubblicoPrivato = Dt_Movimenti.Rows(j).Item("Disciplinare_PubblicoPrivato"),
                            .Data_Creazione = CDate(Dt_Movimenti.Rows(j).Item("Data_Creazione")),
                            .Data_Modifica = CDate(Dt_Movimenti.Rows(j).Item("Data_Modifica")),
                            .Username_Creazione = Dt_Movimenti.Rows(j).Item("Username_Creazione"),
                            .Username_Modifica = Dt_Movimenti.Rows(j).Item("Username_Modifica")
                        }

                        If Not IsNothing(Dt_Movimenti.Rows(j).Item("Tipo_Peso")) AndAlso
                            Not IsDBNull(Dt_Movimenti.Rows(j).Item("Tipo_Peso")) Then
                            Movimento.Tipo_Peso = Dt_Movimenti.Rows(j).Item("Tipo_Peso")
                        End If

                        If Not IsNothing(Dt_Movimenti.Rows(j).Item("ChkLayOut_Bypass_Fatturato")) AndAlso
                            Not IsDBNull(Dt_Movimenti.Rows(j).Item("ChkLayOut_Bypass_Fatturato")) Then
                            Movimento.ChkLayOut_Bypass_Fatturato = Dt_Movimenti.Rows(j).Item("ChkLayOut_Bypass_Fatturato")
                        End If

                        If Not IsNothing(Dt_Movimenti.Rows(j).Item("ChkLayOut_Join_Prodotti")) AndAlso
                            Not IsDBNull(Dt_Movimenti.Rows(j).Item("ChkLayOut_Join_Prodotti")) Then
                            Movimento.ChkLayOut_Join_Prodotti = Dt_Movimenti.Rows(j).Item("ChkLayOut_Join_Prodotti")
                        End If

                        If Not IsNothing(Dt_Movimenti.Rows(j).Item("Cod_RisUm_Altro")) AndAlso
                            Not IsDBNull(Dt_Movimenti.Rows(j).Item("Cod_RisUm_Altro")) Then
                            Movimento.Cod_RisUm_Altro = Dt_Movimenti.Rows(j).Item("Cod_RisUm_Altro")
                        End If

                        If Not IsNothing(Dt_Movimenti.Rows(j).Item("ChkLayOut_Peso")) AndAlso
                            Not IsDBNull(Dt_Movimenti.Rows(j).Item("ChkLayOut_Peso")) Then
                            Movimento.ChkLayOut_Peso = Dt_Movimenti.Rows(j).Item("ChkLayOut_Peso")
                        End If

                        If Not IsNothing(Dt_Movimenti.Rows(j).Item("ChkLayOut_Prezzo")) AndAlso
                            Not IsDBNull(Dt_Movimenti.Rows(j).Item("ChkLayOut_Prezzo")) Then
                            Movimento.ChkLayOut_Prezzo = Dt_Movimenti.Rows(j).Item("ChkLayOut_Prezzo")
                        End If

                        If Not IsNothing(Dt_Movimenti.Rows(j).Item("ChkFiltro_Varietale")) AndAlso
                            Not IsDBNull(Dt_Movimenti.Rows(j).Item("ChkFiltro_Varietale")) Then
                            Movimento.ChkFiltro_Varietale = Dt_Movimenti.Rows(j).Item("ChkFiltro_Varietale")
                        End If

                        If Not IsNothing(Dt_Movimenti.Rows(j).Item("Sezionale_Cod")) AndAlso
                            Not IsDBNull(Dt_Movimenti.Rows(j).Item("Sezionale_Cod")) Then
                            Movimento.Sezionale_Cod = Dt_Movimenti.Rows(j).Item("Sezionale_Cod")
                        End If

                        If Not IsNothing(Dt_Movimenti.Rows(j).Item("Causale_Trasporto_Cod")) AndAlso
                            Not IsDBNull(Dt_Movimenti.Rows(j).Item("Causale_Trasporto_Cod")) Then
                            Movimento.Causale_Trasporto_Cod = Dt_Movimenti.Rows(j).Item("Causale_Trasporto_Cod")
                        End If

                        If Not IsNothing(Dt_Movimenti.Rows(j).Item("ChkLayOut_Litri")) AndAlso
                            Not IsDBNull(Dt_Movimenti.Rows(j).Item("ChkLayOut_Litri")) Then
                            Movimento.ChkLayOut_Litri = Dt_Movimenti.Rows(j).Item("ChkLayOut_Litri")
                        End If

                        If Not IsNothing(Dt_Movimenti.Rows(j).Item("Cod_RisUm_Aggiuntivo")) AndAlso
                           Not IsDBNull(Dt_Movimenti.Rows(j).Item("Cod_RisUm_Aggiuntivo")) Then
                            Movimento.Cod_RisUm_Aggiuntivo = Dt_Movimenti.Rows(j).Item("Cod_RisUm_Aggiuntivo")
                        End If

                        If Not IsNothing(Dt_Movimenti.Rows(j).Item("Cod_indirizzo_Aggiuntivo")) AndAlso
                           Not IsDBNull(Dt_Movimenti.Rows(j).Item("Cod_indirizzo_Aggiuntivo")) Then
                            Movimento.Cod_Indirizzo_Aggiuntivo = Dt_Movimenti.Rows(j).Item("Cod_indirizzo_Aggiuntivo")
                        End If

                        If Not IsNothing(Dt_Movimenti.Rows(j).Item("ChkLayOut_Riscontrato")) AndAlso
                           Not IsDBNull(Dt_Movimenti.Rows(j).Item("ChkLayOut_Riscontrato")) Then
                            Movimento.ChkLayOut_Riscontrato = Dt_Movimenti.Rows(j).Item("ChkLayOut_Riscontrato")
                        End If

                        If Not IsNothing(Dt_Movimenti.Rows(j).Item("Doc_Numero_Visualizzato")) AndAlso
                           Not IsDBNull(Dt_Movimenti.Rows(j).Item("Doc_Numero_Visualizzato")) Then
                            Movimento.Doc_Numero_Visualizzato = Dt_Movimenti.Rows(j).Item("Doc_Numero_Visualizzato")
                        End If

                        If Not IsNothing(Dt_Movimenti.Rows(j).Item("Cod_Macchina_Lav")) AndAlso
                           Not IsDBNull(Dt_Movimenti.Rows(j).Item("Cod_Macchina_Lav")) Then
                            Movimento.Cod_Macchina_Lav = Dt_Movimenti.Rows(j).Item("Cod_Macchina_Lav")
                        End If

                        If Not IsNothing(Dt_Movimenti.Rows(j).Item("TipoDocumento")) AndAlso
                           Not IsDBNull(Dt_Movimenti.Rows(j).Item("TipoDocumento")) Then
                            Movimento.TipoDocumento = Dt_Movimenti.Rows(j).Item("TipoDocumento")
                        End If

                        If Not IsNothing(Dt_Movimenti.Rows(j).Item("Lav_Cod")) AndAlso
                           Not IsDBNull(Dt_Movimenti.Rows(j).Item("Lav_Cod")) Then
                            Movimento.Lav_Cod = Dt_Movimenti.Rows(j).Item("Lav_Cod")
                        End If

                        If Not IsNothing(Dt_Movimenti.Rows(j).Item("OraFine")) AndAlso
                           Not IsDBNull(Dt_Movimenti.Rows(j).Item("OraFine")) Then
                            Movimento.OraFine = Dt_Movimenti.Rows(j).Item("OraFine")
                        End If

                        If Not IsNothing(Dt_Movimenti.Rows(j).Item("Modalita_Applicazione")) AndAlso
                           Not IsDBNull(Dt_Movimenti.Rows(j).Item("Modalita_Applicazione")) Then
                            Movimento.Modalita_Applicazione = Dt_Movimenti.Rows(j).Item("Modalita_Applicazione")
                        End If

                        listaMovimenti.Add(Movimento)

                        '--------------------------------------------------------
                        '-------- MOVIMENTI_DETTAGLI_TECNICI --------------------
                        '--------------------------------------------------------

                        If opzioniLetturaMovimenti.LeggiMovimentiDettagliTecnici Then

                            Dim objMovimentiDettagliTecnici = New Agenda_Movimenti_Dettagli_Tecnici_Helper
                            Dim listaMovimentiDettagliTecnici As List(Of Movimento_Dettaglio_Tecnico)

                            listaMovimentiDettagliTecnici = objMovimentiDettagliTecnici.Leggi(piva,
                                                                                              Movimento.Sa_Cod,
                                                                                              idAgenda,
                                                                                              Dt_Movimenti.Rows(j).Item("Id_Mov"),
                                                                                              0,
                                                                                              objParametri)
                            objMovimentiDettagliTecnici = Nothing

                            If Not IsNothing(listaMovimentiDettagliTecnici) Then
                                Movimento.Movimenti_Dettagli_Tecnici = listaMovimentiDettagliTecnici
                            End If

                        End If

                        '--------------------------------------------------------
                        '-------- MOVIMENTI_DETTAGLI_TECNICI_EXTRA --------------
                        '--------------------------------------------------------

                        If opzioniLetturaMovimenti.LeggiMovimentiDettagliTecniciExtra Then

                            Dim objMovimentiDettagliTecniciExtra = New Agenda_Movimenti_Dettagli_Tecnici_extra_Helper
                            Dim listaMovimentiDettagliTecniciExtra As List(Of Movimento_Dettaglio_Tecnico_Extra)

                            listaMovimentiDettagliTecniciExtra = objMovimentiDettagliTecniciExtra.Leggi(piva,
                                                                                                        0,
                                                                                                        idAgenda,
                                                                                                        Dt_Movimenti.Rows(j).Item("Id_Mov"),
                                                                                                        0,
                                                                                                        0,
                                                                                                        objParametri)
                            objMovimentiDettagliTecniciExtra = Nothing

                            If Not IsNothing(listaMovimentiDettagliTecniciExtra) Then
                                Movimento.Movimenti_Dettagli_Tecnici_Extra = listaMovimentiDettagliTecniciExtra
                            End If

                        End If

                        '--------------------------------------------------------
                        '-------- MOVIMENTI_DETTAGLI ----------------------------
                        '--------------------------------------------------------

                        If opzioniLetturaMovimenti.LeggiMovimentiDettagli Then

                            Dim objMovimentiDettagli = New Agenda_Movimenti_Dettagli_Helper
                            Dim listaMovimentiDettagli As List(Of Movimento_Dettaglio)

                            listaMovimentiDettagli = objMovimentiDettagli.Leggi(piva,
                                                                                Movimento.Sa_Cod,
                                                                                idAgenda,
                                                                                Dt_Movimenti.Rows(j).Item("Id_Mov"), 0,
                                                                                objParametri,
                                                                                filtroMovimentiDettagli:=opzioniLetturaMovimenti.FiltroMovimentiDettagli)
                            objMovimentiDettagli = Nothing

                            If Not IsNothing(listaMovimentiDettagli) Then
                                Movimento.Movimenti_Dettagli = listaMovimentiDettagli
                            End If

                        End If

                        '--------------------------------------------------------
                        '-------- PAGAMENTI -------------------------------------
                        '--------------------------------------------------------

                        If opzioniLetturaMovimenti.LeggiPagamenti Then

                            Dim objPagamenti = New Agenda_Pagamenti_Helper
                            Dim listaPagamenti As List(Of Pagamento)

                            listaPagamenti = objPagamenti.Leggi(piva,
                                                            Movimento.Sa_Cod,
                                                            idAgenda,
                                                            Dt_Movimenti.Rows(j).Item("Id_Mov"), 0,
                                                            objParametri)
                            objPagamenti = Nothing

                            If Not IsNothing(listaPagamenti) Then
                                Movimento.Pagamenti = listaPagamenti
                            End If

                        End If

                    Next

                End If

            Catch ex As Exception

                Throw New Exception("[ Agenda_Movimenti_Helper.leggi() ] : " & ex.Message)

            Finally

                Utility.VerificaChiudiConnessione(objParametri, flagConnessione)

            End Try

            Return listaMovimenti

        End Function

    End Class

    Public Class Opzioni_Lettura_Movimenti

        Public Property LeggiMovimentiDettagli As Boolean

        Public Property FiltroMovimentiDettagli As Integer

        Public Property LeggiMovimentiDettagliTecnici As Boolean

        Public Property LeggiMovimentiDettagliTecniciExtra As Boolean

        Public Property LeggiPagamenti As Boolean

        Sub New()
            LeggiMovimentiDettagli = True
            FiltroMovimentiDettagli = enum_FiltroMovimentiDettagli.Nessuno
            LeggiMovimentiDettagliTecnici = True
            LeggiMovimentiDettagliTecniciExtra = True
            LeggiPagamenti = True
        End Sub

    End Class

    Public Enum enum_FiltroMovimentiDettagli
        Nessuno = 0
        DatiTestataLavorazione = 1
    End Enum

End Namespace
