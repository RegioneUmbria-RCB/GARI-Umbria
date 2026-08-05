Imports System.Linq
Imports System.Text
Imports System.Xml
Imports System.Xml.Linq
Imports AgronicaCoreAnagrafeDAL
Imports AgronicaCoreContabDAL
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider

Public Class Copia_Contatto_Service
    Inherits AgronicaCoreDataProvider.LogProvider

    Private _objParametri_Server As AgronicaCoreParametri
    Private _objParametri_Utenti As AgronicaCoreParametri
    Private _loader As Copia_Contatto_Loader
    Private _cloner As Copia_Contatto_Cloner
    Private _preparer As Copia_Contatto_Preparer
    Private _repointer As Copia_Contatto_Repointer
    Private _privatizzatore As Copia_Contatto_Privatizzatore
    Private _gestisciDatiContabili As Boolean = False

    Public Sub New(ByVal gestisciDatiContabili As Boolean,
        ByRef objParametri_Server As AgronicaCoreParametri,
        ByRef objParametri_Utenti As AgronicaCoreParametri)

        _objParametri_Server = objParametri_Server
        _objParametri_Utenti = objParametri_Utenti
        _gestisciDatiContabili = gestisciDatiContabili
        _loader = New Copia_Contatto_Loader(_objParametri_Server, _objParametri_Utenti)
        _cloner = New Copia_Contatto_Cloner(_objParametri_Server, _objParametri_Utenti)
        _preparer = New Copia_Contatto_Preparer(_gestisciDatiContabili, _objParametri_Server, _objParametri_Utenti)
        _repointer = New Copia_Contatto_Repointer(_objParametri_Server, _objParametri_Utenti)
        _privatizzatore = New Copia_Contatto_Privatizzatore(_objParametri_Server, _objParametri_Utenti)

    End Sub

    ''' <summary>
    ''' Passare il parametro contatti NOTHING e pivaOrigine valorizzato se si desidera che vengano processati
    ''' tutti i contatti di una certa piva
    ''' </summary>
    ''' <param name="indiceProggressivoGias"></param>
    ''' <param name="contatti"></param>
    Public Function CopiaContatti(ByVal indiceProggressivoGias As Integer?,
                             ByVal pivaOrigine As String,
                             ByVal cod_Contatti As List(Of String),
                             ByVal cod_Rapporto As List(Of Integer)
                             ) As List(Of ParametroCopiaContatto)

        Dim messaggioErrore As String = String.Empty
        Dim retVal As Boolean = False
        Dim nomeProcedura As String = "Copia_Contatto_Service.CopiaContatti"

        If String.IsNullOrEmpty(pivaOrigine) AndAlso IsNothing(cod_Contatti) Then
            Throw New Exception("I parametri piva e contatti non possono essere entrambi vuoti")
        End If

        Dim listaContatti As List(Of ParametroCopiaContatto) = Nothing

        Try

            ' Se non è stato passato il progressivoGias lo recupero da db utenti
            If indiceProggressivoGias Is Nothing OrElse Not indiceProggressivoGias.HasValue Then
                Dim obj As New AgronicaCoreUtentiDAL.Utenti_CodiciGiasPRO_R
                Dim dt As DataTable
                dt = obj.Leggi("", "", _objParametri_Utenti)
                indiceProggressivoGias = CInt(dt.Rows(0).Item("progressivogias"))
            End If

            Dim objContatti As New AgronicaCoreAnagrafeBIZ.Contatti_MultiHost_R()
            Dim dtCp = objContatti.ContattiPubbliciDaPrivatizzare(pivaOrigine, cod_Contatti, 0, cod_Rapporto, "", _objParametri_Server)

            Dim contattiPubblici = From cp In dtCp.AsEnumerable
                                   Select New With
                    {
                        .PivaOrigine = cp.Item("PivaOrigine"),
                        .RagSocOrigine = cp.Item("RagSocOrigine"),
                        .PivaDest = cp.Item("PivaDest"),
                        .RagSocDest = cp.Item("RagSocDest"),
                        .Cod_Contatto = cp.Item("Cod_Contatto"),
                        .Contatto_Des = cp.Item("Contatto_Des")
                    }

            Dim contattiPubbliciRag = (From cp In contattiPubblici
                                       Order By cp.PivaOrigine, cp.Cod_Contatto, cp.PivaDest Ascending
                                       Group By pivaOrig = cp.PivaOrigine, ragSocOrig = cp.RagSocOrigine, cp.Cod_Contatto, cp.Contatto_Des
                                Into cprag = Group, Count())

            listaContatti = New List(Of ParametroCopiaContatto)
            For Each rag In contattiPubbliciRag

                Dim piveDest = rag.cprag.Select(Function(s) New KeyValuePair(Of String, String)(s.PivaDest, s.RagSocDest)).ToList

                listaContatti.Add(New ParametroCopiaContatto(rag.Cod_Contatto, rag.Contatto_Des,
                                                            rag.pivaOrig,
                                                            rag.ragSocOrig,
                                                            piveDest))

            Next

            For Each c As ParametroCopiaContatto In listaContatti
                Dim errori = New List(Of String)
                Dim possoPrivatizzare As Boolean = True

                ' Se la piva di destinazione è una sola ed è uguale a quella di origine 
                ' lo privatizzo solo
                If c.PivaDestinazione.Count = 1 AndAlso c.PivaDestinazione.FirstOrDefault().Key = c.PivaOrigine Then
                    If possoPrivatizzare Then
                        Privatizza(c, errori)
                    End If
                Else

                    'Procedo alla creazione dei vari contatti privati
                    Dim piveDaPrivatizzare As List(Of KeyValuePair(Of String, String)) = c.PivaDestinazione.Where(Function(s) s.Key <> c.PivaOrigine).ToList

                    For Each kvp As KeyValuePair(Of String, String) In piveDaPrivatizzare

                        Dim pivaDest = kvp.Key
                        Dim risultato = New RisultatoCopia With {.Piva = pivaDest, .RagSoc = kvp.Value}

                        Try
                            ConnessioniTransazioni.ApriConnessione(True, _objParametri_Server)
                            If Not (Copia(c.PivaOrigine, pivaDest, c.CodContatto, indiceProggressivoGias, errori)) Then
                                risultato.Errori = errori
                                risultato.Stato = Enum_Stato_Copia_Contatto.KO
                                possoPrivatizzare = False
                            Else
                                risultato.OperazioniEffettuate.Add("Copiato")
                                ' Ripuntamento dei cod_ris_um
                                If Repoint(c, pivaDest, errori) Then
                                    risultato.OperazioniEffettuate.Add("Ripuntato")
                                    risultato.Stato = Enum_Stato_Copia_Contatto.OK
                                Else
                                    risultato.Stato = Enum_Stato_Copia_Contatto.KO
                                    possoPrivatizzare = False
                                End If
                            End If

                            If risultato.Errori Is Nothing OrElse risultato.Errori.Count = 0 Then
                                'commit transazione
                                ConnessioniTransazioni.ChiudiTransazione(1, _objParametri_Server)
                                risultato.OperazioniEffettuate.Add("Commit")
                            Else
                                ConnessioniTransazioni.ChiudiTransazione(2, _objParametri_Server)
                                risultato.OperazioniEffettuate.Add("Rollback")
                                possoPrivatizzare = False
                            End If

                        Catch ex As Exception

                            ConnessioniTransazioni.ChiudiTransazione(2, _objParametri_Server)
                            messaggioErrore = ex.Message
                            Scrivi_LOG(_objParametri_Server, nomeProcedura, messaggioErrore)
                            errori.Add(messaggioErrore)
                            risultato.Errori = errori
                            risultato.Stato = Enum_Stato_Copia_Contatto.KO
                            possoPrivatizzare = False

                        Finally
                            ConnessioniTransazioni.ChiudiConnessione(_objParametri_Server)
                        End Try

                        c.Risultati.Add(risultato)

                    Next

                    ' Privatizzo contatto di partenza
                    If possoPrivatizzare Then
                        Privatizza(c, errori)
                    End If

                End If

            Next

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(_objParametri_Server, nomeProcedura, messaggioErrore)
        End Try

        Return listaContatti

    End Function
    Private Function Copia(ByVal pivaOrigine As String,
                          ByVal pivaDest As String,
                          ByVal codContatto As String,
                          ByVal indiceProggressivoGias As Integer,
                          ByRef errori As List(Of String)) As Boolean

        Dim messaggioErrore As String = String.Empty
        Dim retVal As Boolean = False
        Dim nomeProcedura As String = "Copia_Contatto_Service.Copia"

        Try

            ' Controllo se esiste già nella destinazione
            If _loader.ContattoEsisteGia(pivaDest, codContatto) Then
                Throw New Exception(String.Format("Il contatto esista già nella destinazione. PIVA = {0}, COD_CONTATTO = {1}", pivaDest, codContatto))
            End If

            Dim contatto = _loader.Carica(pivaOrigine, codContatto, "", errori)
            If contatto Is Nothing Then
                Throw New Exception(String.Format("Contatto non trovato. PIVA = {0}, COD_CONTATTO = {1}", pivaOrigine, codContatto))
            End If

            Dim cloneContatto = _cloner.Clona(contatto)
            If cloneContatto Is Nothing Then
                Throw New Exception(String.Format("Errore durante la clonazione del contatto. PIVA = {0}, COD_CONTATTO = {1}", pivaOrigine, codContatto))
            End If

            Dim xContatto = _preparer.Prepare(pivaOrigine, pivaDest, cloneContatto, indiceProggressivoGias, errori)
            If xContatto Is Nothing Then
                Throw New Exception(String.Format("Errore durante la preparazione XML del nuovo contatto. PIVA = {0}, COD_CONTATTO = {1}", pivaOrigine, codContatto))
            End If

            Dim output_piva As String = ""
            Dim output_cod_contatto As String = ""
            Dim objContattoW As New AgronicaCoreAnagrafeBIZ.Contatti_W

            ' CHiamata a funzione salva vera e propia
            retVal = objContattoW.Contatto_Scrivi(xContatto.OuterXml, output_piva, output_cod_contatto, _objParametri_Server, True)
            If Not retVal Then
                errori.Add(String.Format("Errore durante la copia effettiva del contatto. PIVA = {0}, COD_CONTATTO = {1}", pivaOrigine, codContatto))
            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            errori.Add(messaggioErrore)
            Scrivi_LOG(_objParametri_Server, nomeProcedura, messaggioErrore)
            Return False
        End Try

        Return retVal

    End Function

    Private Function Repoint(ByVal contatto As ParametroCopiaContatto, ByVal pivaDest As String, ByRef errori As List(Of String)) As Boolean

        Return _repointer.Repoint(contatto, pivaDest, errori)

    End Function
    Private Function Privatizza(ByVal contatto As ParametroCopiaContatto, ByRef errori As List(Of String)) As Boolean

        Dim risultato As Boolean = False
        Dim messaggioErrore As String
        Dim risultatoOperazione = New RisultatoCopia With {.Piva = contatto.PivaOrigine, .RagSoc = contatto.RagSocOrigine}

        Try

            risultatoOperazione.OperazioniEffettuate.Add("Privatizzato")
            ConnessioniTransazioni.ApriConnessione(True, _objParametri_Server)
            _privatizzatore.Privatizza(contatto, errori)
            ConnessioniTransazioni.ChiudiTransazione(1, _objParametri_Server)
            risultatoOperazione.Stato = Enum_Stato_Copia_Contatto.OK
            risultatoOperazione.OperazioniEffettuate.Add("Commit")

        Catch ex As Exception
            ConnessioniTransazioni.ChiudiTransazione(2, _objParametri_Server)
            messaggioErrore = ex.Message
            risultatoOperazione.Stato = Enum_Stato_Copia_Contatto.KO
            risultatoOperazione.OperazioniEffettuate.Add("Rollback")
            errori.Add(messaggioErrore)
        Finally
            ConnessioniTransazioni.ChiudiConnessione(_objParametri_Server)
        End Try

        contatto.Risultati.Add(risultatoOperazione)

        Return risultato

    End Function

End Class


Public Class Copia_Contatto_Loader
    Inherits AgronicaCoreDataProvider.LogProvider

    Private _objParametri_Server As AgronicaCoreParametri
    Private _objParametri_Utenti As AgronicaCoreParametri

    Public Sub New(ByRef objParametri_Server As AgronicaCoreParametri,
                   ByRef objParametri_Utenti As AgronicaCoreParametri)

        _objParametri_Server = objParametri_Server
        _objParametri_Utenti = objParametri_Utenti

    End Sub

    Public Function ContattoEsisteGia(ByVal pivaDest As String,
                                      ByVal cod_contatto As String) As Boolean

        Dim messaggioErrore As String = String.Empty
        Dim nomeProcedura = "Copia_Contatto_Service.Copia_Contatto_Loader.ContattoEsisteGia"
        Dim datiXML As String = String.Empty
        Dim objContatto_R As New AgronicaCoreAnagrafeBIZ.Contatti_MultiHost_R

        Try
            datiXML = objContatto_R.Contatto_Leggi(pivaDest, cod_contatto, "", False, _objParametri_Server)

            ''sono in info di un contatto legato ad un'altra azienda
            'If String.IsNullOrEmpty(datiXML) Then
            '    datiXML = objContatto_R.Contatto_Leggi("", pivaDest, "", False, _objParametri_Server)
            'End If

            If Not String.IsNullOrEmpty(datiXML) Then
                Return True
            End If

            Return False

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(_objParametri_Server, nomeProcedura, messaggioErrore)
            Return False
        End Try

    End Function

    Public Function CaricaSmart(ByVal piva As String,
                          ByVal cod_RsiUm As Integer,
                          ByVal piva_SuperUser_Origine As String) As ContattoModel

        Dim messaggioErrore As String = String.Empty
        Dim nomeProcedura = "Copia_Contatto_Service.Copia_Contatto_Loader.Copia"
        Dim datiXML As String = String.Empty
        Dim objContatto_R As New AgronicaCoreAnagrafeBIZ.Contatti_MultiHost_R
        Dim objRisUm As New AgronicaCoreAnagrafeDAL.Risorse_Umane_R

        Dim doc As XDocument = Nothing
        Dim contattoModel As New ContattoModel()

        Try

            Dim dtRisum = objRisUm.Leggi2(piva, "", cod_RsiUm, 0, 0, piva_SuperUser_Origine, False, "", "", _objParametri_Server)
            If dtRisum Is Nothing OrElse dtRisum.Rows.Count = 0 Then
                Return Nothing
            End If

            Dim contattoTemp = dtRisum.AsEnumerable().Select(Function(s) s.Item("Cod_Contatto")).FirstOrDefault()
            Dim codContatto  = ""
            If contattoTemp IsNot Nothing Then
                codContatto = contattoTemp.ToString()
                datiXML = objContatto_R.Contatto_Leggi(piva, codContatto, "", False, _objParametri_Server)
            End If

            'sono in info di un contatto legato ad un'altra azienda
            If String.IsNullOrEmpty(datiXML) Then
                datiXML = objContatto_R.Contatto_Leggi("", piva, "", False, _objParametri_Server)
            End If

            If String.IsNullOrEmpty(datiXML) Then
                Throw New Exception(String.Format("Nessun dato trovato per CodContatto {0} e PIVA {1} ", codContatto, piva))
            End If

            doc = XDocument.Parse(datiXML)
            Dim risultato = (From c In doc.<DatiContatti>.<Contatto>
                             Select c).FirstOrDefault()

            contattoModel.ID_CF = risultato.@id_cf
            contattoModel.Sa_Cod = risultato.@sa_cod

            Select Case contattoModel.ID_CF
                Case PERSONA_GIURIDICA

                    contattoModel.Piva = risultato.@cod_contatto
                    contattoModel.Rag_Soc = risultato.@rag_soc

                Case PERSONA_FISICA

                    contattoModel.CF = risultato.@cod_contatto
                    contattoModel.Nome = risultato.@nome
                    contattoModel.Cognome = risultato.@cognome
                    If contattoModel.Nome = "" AndAlso contattoModel.Cognome = "" Then
                        contattoModel.Nome = risultato.@rag_soc
                    End If

                    Dim dateParsed As DateTime

                    If Not String.IsNullOrEmpty(risultato.@data_nascita) AndAlso
                        DateTime.TryParse(risultato.@data_nascita, dateParsed) Then
                        contattoModel.DataNascita = dateParsed
                    End If
                    If Not String.IsNullOrEmpty(risultato.@sesso) Then
                        contattoModel.Sesso = risultato.@sesso
                    End If

                Case CONTATTO_ESTERO

                    contattoModel.Piva = risultato.@cod_contatto
                    contattoModel.Rag_Soc = risultato.@rag_soc
                    contattoModel.CF_Estero = risultato.@codice_fiscale

            End Select

            Return contattoModel

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(_objParametri_Server, nomeProcedura, messaggioErrore)
            Return Nothing
        End Try

    End Function


    Public Function Carica(ByVal piva As String,
                          ByVal codContatto As String,
                          ByVal piva_SuperUser_Origine As String,
                          ByRef errori As List(Of String)) As ContattoModel

        Dim messaggioErrore As String = String.Empty
        Dim nomeProcedura = "Copia_Contatto_Service.Copia_Contatto_Loader.Copia"
        Dim datiXML As String = String.Empty
        Dim objContatto_R As New AgronicaCoreAnagrafeBIZ.Contatti_MultiHost_R
        Dim doc As XDocument = Nothing
        Dim contattoModel As New ContattoModel()

        Try

            datiXML = objContatto_R.Contatto_Leggi(piva, codContatto, "", False, _objParametri_Server)

            'sono in info di un contatto legato ad un'altra azienda
            If String.IsNullOrEmpty(datiXML) Then
                datiXML = objContatto_R.Contatto_Leggi("", piva, "", False, _objParametri_Server)
            End If

            If String.IsNullOrEmpty(datiXML) Then
                Throw New Exception(String.Format("Nessun dato trovato per CodContatto {0} e PIVA {1} ", codContatto, piva))
            End If

            doc = XDocument.Parse(datiXML)
            Dim risultato = (From c In doc.<DatiContatti>.<Contatto>
                             Select c).FirstOrDefault()

            contattoModel.ID_CF = risultato.@id_cf
            contattoModel.Sa_Cod = risultato.@sa_cod

            Select Case contattoModel.ID_CF
                Case PERSONA_GIURIDICA

                    contattoModel.Piva = risultato.@cod_contatto
                    contattoModel.Rag_Soc = risultato.@rag_soc

                Case PERSONA_FISICA

                    contattoModel.CF = risultato.@cod_contatto
                    contattoModel.Nome = risultato.@nome
                    contattoModel.Cognome = risultato.@cognome
                    If contattoModel.Nome = "" AndAlso contattoModel.Cognome = "" Then
                        contattoModel.Nome = risultato.@rag_soc
                    End If

                    Dim dateParsed As DateTime

                    If Not String.IsNullOrEmpty(risultato.@data_nascita) AndAlso
                        DateTime.TryParse(risultato.@data_nascita, dateParsed) Then
                        contattoModel.DataNascita = dateParsed
                    End If
                    If Not String.IsNullOrEmpty(risultato.@sesso) Then
                        contattoModel.Sesso = risultato.@sesso
                    End If

                Case CONTATTO_ESTERO

                    contattoModel.Piva = risultato.@cod_contatto
                    contattoModel.Rag_Soc = risultato.@rag_soc
                    contattoModel.CF_Estero = risultato.@codice_fiscale

            End Select
            contattoModel.Convenevoli = risultato.@convenevoli
            contattoModel.Badge = risultato.@nrBadge
            contattoModel.Indirizzi = CaricaIndirizzi(doc, piva)
            contattoModel.Rubrica = CaricaRubrica(doc)
            contattoModel.Costi = CaricaCosti(doc)
            contattoModel.ContattiCodici = CaricaContattiCodici(doc)
            contattoModel.RapportiContabili = CaricaRapportiContabili(piva, codContatto)
            contattoModel.AltriDati = CaricaAltriDati(doc)
            contattoModel.Liquidita = CaricaLiquidita(doc)
            contattoModel.Conti = CaricaConti(doc)
            contattoModel.DettagliContabili = CaricaDettagliContabili(doc)

            MappaContattiCollegati(contattoModel)

            Return contattoModel

        Catch ex As Exception
            messaggioErrore = ex.Message
            errori.Add(messaggioErrore)
            Scrivi_LOG(_objParametri_Server, nomeProcedura, messaggioErrore)
            Return Nothing
        End Try

    End Function

    Private Sub MappaContattiCollegati(ByVal contatto As ContattoModel)

        Dim cc = New List(Of ContattoCollegatoModel)

        'agente
        AggiungiContattoCollegato(cc, contatto.DettagliContabili.dettCont_agente_cod)

        'vettore
        AggiungiContattoCollegato(cc, contatto.DettagliContabili.dettCont_vettore_cod)

        'capoarea
        AggiungiContattoCollegato(cc, contatto.DettagliContabili.dettCont_capoarea_cod)

        'destinatario diverso
        AggiungiContattoCollegato(cc, contatto.DettagliContabili.dettCont_destinazione_diversa)

        'rappresentante fiscale
        Dim rf = contatto.ContattiCodici.FirstOrDefault(Function(s) s.ID_Cod = 4021)
        If rf IsNot Nothing Then
            AggiungiContattoCollegato(cc, rf.Val_Cod)
        End If

        contatto.ContattiCollegati = cc

    End Sub

    Private Sub AggiungiContattoCollegato(ByVal ContattiCollegati As List(Of ContattoCollegatoModel), ByVal cod_RisUm As Integer)

        If cod_RisUm <> 0 AndAlso Not ContattiCollegati.Any(Function(s) s.Cod_Risum = cod_RisUm) Then
            ContattiCollegati.Add(New ContattoCollegatoModel With {.Cod_Risum = cod_RisUm})
        End If

    End Sub

    Private Function CaricaDettagliContabili(ByVal doc As XDocument) As DettagliContabiliModel

        If doc Is Nothing Then
            Return New DettagliContabiliModel
        End If

        Dim risultato = (From c In doc.<DatiContatti>.<Contatto>
                         Select c).FirstOrDefault

        Dim dc = New DettagliContabiliModel With {
            .dettCont_sconto_add1 = 0,
            .dettCont_sconto_add2 = 0,
            .dettCont_sconto_add3 = 0,
            .dettCont_sconto_cliente = 0,
            .dettCont_provvigione_agente = 0,
            .dettCont_provvigione_capoarea = 0,
            .dettCont_agente_cod = 0,
            .dettCont_capoarea_cod = 0,
            .dettCont_vettore_cod = 0,
            .dettCont_iva_default = -1,
            .dettCont_cod_conto_economico_default = 1,
            .dettCont_cod_conto_patrimoniale_default = 1,
            .dettCont_fatturazione_automatica = -1,
            .dettCont_documento_fatturazione = -1,
            .dettCont_destinazione_diversa = -1
        }

        If risultato.@sconto_testo IsNot Nothing AndAlso Not String.IsNullOrEmpty(risultato.@sconto_testo) Then
            Dim scontiAddizionali = risultato.@sconto_testo.Split("-")
            dc.dettCont_sconto_add1 = scontiAddizionali(0)
            If scontiAddizionali.Count() > 1 Then
                dc.dettCont_sconto_add2 = scontiAddizionali(1)
            End If
            If scontiAddizionali.Count() > 2 Then
                dc.dettCont_sconto_add3 = scontiAddizionali(2)
            End If
        End If

        If risultato.@provvigione IsNot Nothing AndAlso Not String.IsNullOrEmpty(risultato.@provvigione) Then
            dc.dettCont_provvigione_agente = risultato.@provvigione
        End If

        If risultato.@provvigione_capoarea IsNot Nothing AndAlso Not String.IsNullOrEmpty(risultato.@provvigione_capoarea) Then
            dc.dettCont_provvigione_capoarea = risultato.@provvigione_capoarea
        End If

        If risultato.@agente_cod IsNot Nothing AndAlso Not String.IsNullOrEmpty(risultato.@agente_cod) Then
            dc.dettCont_agente_cod = risultato.@agente_cod
        End If

        If risultato.@capoarea_cod IsNot Nothing AndAlso Not String.IsNullOrEmpty(risultato.@capoarea_cod) Then
            dc.dettCont_capoarea_cod = risultato.@capoarea_cod
        End If

        If risultato.@vettore_cod IsNot Nothing AndAlso Not String.IsNullOrEmpty(risultato.@vettore_cod) Then
            dc.dettCont_vettore_cod = risultato.@vettore_cod
        End If

        If risultato.@cod_iva_contatto IsNot Nothing AndAlso Not String.IsNullOrEmpty(risultato.@cod_iva_contatto) Then
            dc.dettCont_iva_default = risultato.@cod_iva_contatto
        End If

        If risultato.@cod_conto_economico IsNot Nothing AndAlso Not String.IsNullOrEmpty(risultato.@cod_conto_economico) Then
            dc.dettCont_cod_conto_economico_default = risultato.@cod_conto_economico
        End If

        If risultato.@cod_conto_patrimoniale IsNot Nothing AndAlso Not String.IsNullOrEmpty(risultato.@cod_conto_patrimoniale) Then
            dc.dettCont_cod_conto_patrimoniale_default = risultato.@cod_conto_patrimoniale
        End If

        If risultato.@modalita_fatturazione IsNot Nothing AndAlso Not String.IsNullOrEmpty(risultato.@modalita_fatturazione) Then
            dc.dettCont_fatturazione_automatica = risultato.@modalita_fatturazione
        End If

        If risultato.@documento_fatturazione IsNot Nothing AndAlso Not String.IsNullOrEmpty(risultato.@documento_fatturazione) Then
            dc.dettCont_documento_fatturazione = risultato.@documento_fatturazione
        End If

        If risultato.@cod_risum_destinazione_diversa IsNot Nothing AndAlso Not String.IsNullOrEmpty(risultato.@cod_risum_destinazione_diversa) Then
            dc.dettCont_destinazione_diversa = risultato.@cod_risum_destinazione_diversa
        End If

        If risultato.@tipo_indirizzo_default_destinazione_diversa IsNot Nothing AndAlso Not String.IsNullOrEmpty(risultato.@tipo_indirizzo_default_destinazione_diversa) Then
            dc.dettCont_tipo_indirizzo_default_destinazione_diversa = risultato.@tipo_indirizzo_default_destinazione_diversa
        End If

        Return dc

    End Function

    Private Function CaricaConti(ByVal doc As XDocument) As List(Of ContiModel)

        If doc Is Nothing Then
            Return New List(Of ContiModel)
        End If

        Dim conti = (From l In doc.<DatiContatti>.<Contatto>.<DatiConti>
                     Select New ContiModel With
                             {
                                .Cod_Conto = l.@Cod_Conto,
                                .ID_Riclassificazione = l.@ID_Riclassificazione,
                                .Conto_Descr = l.@Conto_Descr,
                                .Validita_Inizio = l.@Validita_Inizio,
                                .Validita_Fine = l.@Validita_Fine,
                                .key_conto = String.Format("{0}-{1}-{2}", l.@piva, l.@Cod_Conto, l.@cod_contatto)
                     }).ToList()

        Return conti

    End Function

    Private Function CaricaLiquidita(ByVal doc As XDocument) As List(Of LiquiditaModel)

        If doc Is Nothing Then
            Return New List(Of LiquiditaModel)
        End If

        Dim liquidita = (From l In doc.<DatiContatti>.<Contatto>.<DatiLiquidita>
                         Select New LiquiditaModel With
                             {
                                .Sa_Cod = l.@Sa_Cod,
                                .Cod_Liquidita = l.@cod_liquidita,
                                .Cod_Istituto = l.@cod_istituto,
                                .Istituto_Des = l.@istituto_des,
                                .Nazione = l.@nazione,
                                .Cifre_Controllo = l.@cifre_controllo,
                                .Cin = l.@cin,
                                .Abi = l.@abi,
                                .Numero = l.@numero,
                                .Bic = l.@Bic,
                                .ChkAbilitazione = l.@chkabilitazione,
                                .Abilitazione_Des = l.@abilitazione_des,
                                .Validita_Inizio = l.@validita_inizio,
                                .Validita_Fine = l.@validita_fine,
                                .Note = l.@note,
                                .ChkDefault = CInt(l.@chkdefault) <> 0
        }).ToList()

        Return liquidita

    End Function
    Private Function CaricaAltriDati(ByVal doc As XDocument) As AltriDatiModel

        If doc Is Nothing Then
            Return New AltriDatiModel
        End If

        Dim risultato = (From c In doc.<DatiContatti>.<Contatto>
                         Select c).FirstOrDefault()

        Dim altriDati = New AltriDatiModel
        altriDati.Note = risultato.@note
        altriDati.Note2 = risultato.@note2

        altriDati.NoteOperazioni = risultato.@note_operazioni
        altriDati.NoteOperazioni2 = risultato.@note2_operazioni

        altriDati.Tipo_destinazione = risultato.@tipo_destinazione
        altriDati.OrigineDestinazione = risultato.@tipo_speditore
        altriDati.DettCont_tipo_ind_default = risultato.@tipo_indirizzo_default
        altriDati.Memo = risultato.@memo

        If Not IsNothing(risultato.@chkfittizio) AndAlso IsNumeric(risultato.@chkfittizio) Then
            altriDati.Fittizio = Convert.ToBoolean(CInt(risultato.@chkfittizio))
        End If

        Return altriDati

    End Function
    Private Function CaricaIndirizzi(ByVal doc As XDocument,
                                ByVal piva As String) As List(Of IndirizzoModel)

        If doc Is Nothing Then
            Return New List(Of IndirizzoModel)
        End If

        Dim indiTipoDal = New IndirizzoTipo_R()
        Dim indirizziTipo = TipiIndirizzoToList(indiTipoDal.Leggi(piva, -1, -99, -1, "", "", _objParametri_Server))

        Dim risultato_Indirizzi = (From c In doc.<DatiContatti>.<Contatto>.<Indirizzo>
                                   Select c).ToList

        Dim indirizzi = (From c In risultato_Indirizzi
                         Select New IndirizzoModel With
                                 {
                                    .piva = c.@piva,
                                    .Sa_cod = CInt(c.@sa_cod),
                                    .Cod_Indirizzo = CInt(c.@cod_indirizzo),
                                    .Tipo_Indirizzo = CInt(c.@tipo_indirizzo),
                                    .Via = c.@ind_des,
                                    .Frazione = c.@frz_des,
                                    .Cap = c.@cap,
                                    .Com_des = c.@com_des,
                                    .Provincia_Cod = c.@pro_cod,
                                    .Stato = c.@stato,
                                    .Istat_Prov = c.@pro_cod_istat,
                                    .Istat_Com = c.@com_cod_istat,
                                    .CodiceLingua = c.@codice_lingua,
                                    .Note = c.@note
        }).ToList()

        Return indirizzi

    End Function

    Private Function TipiIndirizzoToList(ByVal DT As DataTable) As IEnumerable(Of Object)

        Dim lista = From ind In DT.AsEnumerable()
                    Select New With
                    {
                        .IndirizzoTipo_Cod = CInt(ind("IndirizzoTipo_Cod")),
                        .Descrizione = ind("Descrizione")
                    }

        Return lista.ToList()

    End Function

    Private Function CaricaCosti(ByVal doc As XDocument) As List(Of CostiModel)

        If doc Is Nothing Then
            Return New List(Of CostiModel)
        Else
            Dim costi = (From c In doc.<DatiContatti>.<Contatto>.<RapCon>.<DatiProdotti_Costi>.<Prodotto_Costo>
                         Select New CostiModel With
                                 {
                                    .Id = c.@id,
                                    .Udm_Cod = c.@mezzo,
                                    .InizioPrezzo = c.@validita_inizio,
                                    .FinePrezzo = c.@validita_fine,
                                    .Prezzo = c.@prezzo_unitario,
                                    .Cod_RisUm = c.@mat_cod,
                                    .Cod_RisUm_Des = "",
                                    .Udm_Des = "",
                                    .Id_Budget = 0
                }).ToList()
            For Each c As Object In costi
                c.Udm_Des = IIf(c.Udm_Cod = enum_TipoMezzo.Ettaro, "HA", "Ora")
            Next

            Return costi

        End If

    End Function

    Private Function CaricaRubrica(ByVal doc As XDocument) As List(Of RubricaModel)

        If doc Is Nothing Then
            Return New List(Of RubricaModel)
        Else
            Dim rubrica = (
                        From c In doc.<DatiContatti>.<Contatto>.<Rubrica>
                        Select New RubricaModel With
                                {
                                    .Cod_Rubrica = c.@cod_rubrica,
                                    .Numero = c.@numero,
                                    .Descrizione = c.@descr,
                                    .Key_Rubrica = Guid.NewGuid.ToString()
                                }
                        ).ToList()
            Return rubrica
        End If

    End Function

    Private Function CaricaRapportiContabili(ByVal piva As String,
                                             ByVal codContatto As String) As List(Of RapportoContabileModel)

        Dim dal = New Rapporti_Contabili_R()
        Dim dtRappCont As DataTable = dal.RapportiContabili_X_Contatto_Leggi(piva, codContatto, 0, 0, "", _objParametri_Server)

        If IsNothing(dtRappCont) OrElse dtRappCont.Rows.Count = 0 Then
            Return New List(Of RapportoContabileModel)
        End If

        Dim rapportiContabili = New List(Of RapportoContabileModel)

        For Each r As DataRow In dtRappCont.Rows

            Dim rc = New RapportoContabileModel
            rc.Cod_RisUm = If(r.Item("Cod_RisUm") Is DBNull.Value, 0, CInt(r.Item("Cod_RisUm")))
            rc.piva = r.Item("piva").ToString()
            rc.sa_cod = CInt(r.Item("sa_cod"))
            rc.Cod_Contatto = r.Item("Cod_Contatto").ToString()
            rc.Cod_Rapporto = CInt(r.Item("Cod_Rapporto"))
            rc.Settore_Des = If(r.Item("Settore_Des") Is DBNull.Value, "", r.Item("Settore_Des").ToString())
            rc.Qualifica_Cod = If(r.Item("Qualifica_Cod") Is DBNull.Value, 0, CInt(r.Item("Qualifica_Cod")))
            rc.Mansione_Cod = If(r.Item("Mansione_Cod") Is DBNull.Value, 0, CInt(r.Item("Mansione_Cod")))
            rc.Validita_Inizio = If(r.Item("Validita_Inizio") Is DBNull.Value, AGRODATAINIZIO, CDate(r.Item("Validita_Inizio")))
            rc.Validita_Fine = If(r.Item("Validita_Fine") Is DBNull.Value, AGRODATAFINE, CDate(r.Item("Validita_Fine")))
            rc.Attivita_Des = If(r.Item("Attivita_Des") Is DBNull.Value, "", r.Item("Attivita_Des").ToString())
            rc.occasionale = If(r.Item("occasionale") Is DBNull.Value, 0, CInt(r.Item("occasionale")))
            rc.Ore_Settimanali = If(r.Item("Ore_Settimanali") Is DBNull.Value, 0, CDec(r.Item("Ore_Settimanali")))
            rc.Info_Famiglia = If(r.Item("Info_Famiglia") Is DBNull.Value, "", r.Item("Info_Famiglia").ToString())
            rc.Classificazione_Cod = If(r.Item("Classificazione_Cod") Is DBNull.Value, 0, CInt(r.Item("Classificazione_Cod")))
            rc.TipoRapporto_Cod = rc.occasionale

            rapportiContabili.Add(rc)
        Next

        Return rapportiContabili

    End Function

    Private Function CaricaContattiCodici(ByVal doc As XDocument) As List(Of ContattoCodiceModel)


        If doc Is Nothing Then
            Return New List(Of ContattoCodiceModel)
        End If

        Dim contattiCodici = (From cc In doc.<DatiContatti>.<Contatto>.<Contatto_Codice>
                              Select New ContattoCodiceModel With
                                 {
                                    .ID_Cod = cc.@id_cod,
                                    .Val_Cod = cc.@val_cod
                              }).ToList()

        Return contattiCodici

    End Function

End Class

Public Class Copia_Contatto_Repointer
    Inherits AgronicaCoreDataProvider.DataProvider

    Private _objParametri_Server As AgronicaCoreParametri
    Private _objParametri_Utenti As AgronicaCoreParametri
    Private _repointerStack As List(Of RepointElement) = New List(Of RepointElement)

    Private Const TAG_PIVA As String = "#PIVA#"
    Private Const TAG_COD_RISUM_ORIGINE As String = "#COD_RISUM_ORIGINE#"
    Private Const TAG_COD_RISUM_DEST As String = "#TAG_COD_RISUM_DEST#"
    Private Const TAG_COD_INDIRIZZO_ORIGINE As String = "#COD_INDIRIZZO_ORIGINE#"
    Private Const TAG_COD_INDIRIZZO_DEST As String = "#TAG_COD_INDIRIZZO_DEST#"


    Public Sub New(ByRef objParametri_Server As AgronicaCoreParametri,
                   ByRef objParametri_Utenti As AgronicaCoreParametri)
        _objParametri_Server = objParametri_Server
        _objParametri_Utenti = objParametri_Utenti
        InizializzaStack()

    End Sub

    Public Function Repoint(ByVal contatto As ParametroCopiaContatto,
                            ByVal pivaDest As String,
                            ByRef errori As List(Of String)) As Boolean

        Dim messaggioErrore As String
        Dim nomeProcedura As String = "Copia_Contatto_Repointer.Repoint"
        Dim risultato As Boolean = False

        Try

            ' repoint risorse umane
            Dim risUm_Mapping = CaricaMappaRapportiContabili(contatto, pivaDest)
            For Each ru As RisUmMapping In risUm_Mapping
                For Each re As RepointElement In _repointerStack.Where(Function(s) s.IdGruppo.Equals("RISUM"))

                    Dim sql = re.Sql.Replace(TAG_PIVA, pivaDest)
                    sql = sql.Replace(TAG_COD_RISUM_ORIGINE, ru.Cod_RisUM_Origine)
                    sql = sql.Replace(TAG_COD_RISUM_DEST, ru.Cod_RisUM_Nuovo)
                    EseguiQuery_Scrittura(_objParametri_Server, sql, nomeProcedura)
                Next
            Next

            'repoint indirizzi
            Dim indirizzi_Mapping = CaricaMappaIndirizzi(contatto, pivaDest)
            For Each ind As IndirizzoMapping In indirizzi_Mapping
                For Each re As RepointElement In _repointerStack.Where(Function(s) s.IdGruppo.Equals("INDIRIZZO"))

                    Dim sql = re.Sql.Replace(TAG_PIVA, pivaDest)
                    sql = sql.Replace(TAG_COD_INDIRIZZO_ORIGINE, ind.Cod_Indirizzo_Origine)
                    sql = sql.Replace(TAG_COD_INDIRIZZO_DEST, ind.Cod_Indirizzo_Nuovo)
                    EseguiQuery_Scrittura(_objParametri_Server, sql, nomeProcedura)
                Next
            Next

            risultato = True

        Catch ex As Exception
            messaggioErrore = ex.Message
            errori.Add(messaggioErrore)
            Scrivi_LOG(_objParametri_Server, nomeProcedura, messaggioErrore)
        End Try

        Return risultato

    End Function

    Private Function CaricaMappaIndirizzi(
                                        ByVal contatto As ParametroCopiaContatto,
                                        ByVal pivaDest As String) As IEnumerable(Of IndirizzoMapping)

        Dim nomeProcedura As String = "Copia_Contatto_Repointer.CaricaMappaIndirizzi"
        Dim stb As New System.Text.StringBuilder
        Dim risultato As New List(Of IndirizzoMapping)
        Dim messaggioErrore As String

        stb.AppendLine(" SELECT * FROM ContattiXIndirizzi ")
        stb.AppendLine(" WHERE Piva IN ('" & Agro_SQL_SaveText(contatto.PivaOrigine) & "','" & Agro_SQL_SaveText(pivaDest) & "') ")
        stb.AppendLine(" AND Cod_Contatto = '" & Agro_SQL_SaveText(contatto.CodContatto) & "' ")

        Dim dtIndirizzi = EseguiQuery_Lettura(_objParametri_Server, stb.ToString(), nomeProcedura)

        Dim indirizzi = (From r In dtIndirizzi.AsEnumerable
                         Select New With {
                    .Piva = r.Item("Piva"),
                    .Tipo_Indirizzo = r.Item("Tipo_Indirizzo"),
                    .Validita_Inizio = CDate(r.Item("Validita_Inizio")),
                    .Validita_Fine = CDate(r.Item("Validita_Fine")),
                    .Cod_Indirizzo = CInt(r.Item("Cod_Indirizzo"))
                }).ToList()

        Dim indirizziOrigine = indirizzi.Where(Function(s) s.Piva.Equals(contatto.PivaOrigine)).ToList
        Dim indirizziDest = indirizzi.Where(Function(s) Not s.Piva.Equals(contatto.PivaOrigine)).ToList

        indirizziOrigine.ForEach(
            Function(r)
                Dim rm = New IndirizzoMapping
                rm.PivaOri = r.Piva
                rm.Cod_Indirizzo_Origine = r.Cod_Indirizzo

                Dim rmDest = indirizziDest.Where(Function(s) s.Tipo_Indirizzo = r.Tipo_Indirizzo AndAlso s.Validita_Inizio.Equals(r.Validita_Inizio) AndAlso s.Validita_Fine.Equals(r.Validita_Fine))
                If rmDest Is Nothing OrElse rmDest.Count = 0 Then
                    messaggioErrore = String.Format("Non è stato trovato il mapping per l'indirizzo Id {0}", r.Cod_Indirizzo)
                    Throw New Exception(messaggioErrore)
                Else
                    If rmDest.Count > 1 Then
                        messaggioErrore = String.Format("Trovati più mapping per l'indirizzo con Id {0}", r.Cod_Indirizzo)
                        Throw New Exception(messaggioErrore)
                    End If

                    Dim rmDestFirst = rmDest.FirstOrDefault()
                    If rmDestFirst IsNot Nothing Then
                        rm.Cod_Indirizzo_Nuovo = rmDestFirst.Cod_Indirizzo
                        rm.PivaDest = rmDestFirst.Piva
                    End If
                End If

                risultato.Add(rm)

            End Function)


        Return risultato

    End Function

    Private Function CaricaMappaRapportiContabili(
                                                 ByVal contatto As ParametroCopiaContatto,
                                                 ByVal pivaDest As String) As IEnumerable(Of RisUmMapping)

        Dim nomeProcedura As String = "Copia_Contatto_Repointer.CaricaMappaRapportiContabili"
        Dim stb As New System.Text.StringBuilder
        Dim risultato As New List(Of RisUmMapping)
        Dim messaggioErrore As String

        stb.AppendLine(" SELECT * FROM risorse_umane ")
        stb.AppendLine(" WHERE Piva IN ('" & Agro_SQL_SaveText(contatto.PivaOrigine) & "','" & Agro_SQL_SaveText(pivaDest) & "') ")
        stb.AppendLine(" AND Cod_Contatto = '" & Agro_SQL_SaveText(contatto.CodContatto) & "' ")

        Dim dtRisUm = EseguiQuery_Lettura(_objParametri_Server, stb.ToString(), nomeProcedura)

        Dim risorseUmane = (From r In dtRisUm.AsEnumerable
                            Select New With {
                    .Piva = r.Item("Piva"),
                    .Cod_Rapporto = r.Item("Cod_Rapporto"),
                    .Validita_Inizio = CDate(r.Item("Validita_Inizio")),
                    .Validita_Fine = CDate(r.Item("Validita_Fine")),
                    .Cod_RisUm = CInt(r.Item("Cod_RisUm"))
                }).ToList()

        Dim risorseUmaneOrigine = risorseUmane.Where(Function(s) s.Piva.Equals(contatto.PivaOrigine)).ToList
        Dim risorseUmaneDest = risorseUmane.Where(Function(s) Not s.Piva.Equals(contatto.PivaOrigine)).ToList

        risorseUmaneOrigine.ForEach(
            Function(r)
                Dim rm = New RisUmMapping
                rm.PivaOri = r.Piva
                rm.Cod_RisUM_Origine = r.Cod_RisUm

                Dim rmDest = risorseUmaneDest.Where(Function(s) s.Cod_Rapporto = r.Cod_Rapporto AndAlso s.Validita_Inizio.Equals(r.Validita_Inizio) AndAlso s.Validita_Fine.Equals(r.Validita_Fine))
                If rmDest Is Nothing OrElse rmDest.Count = 0 Then
                    messaggioErrore = String.Format("Non è stato trovato il mapping per la risorsa umana con Id {0}", r.Cod_RisUm)
                    Throw New Exception(messaggioErrore)
                Else
                    If rmDest.Count > 1 Then
                        messaggioErrore = String.Format("Trovati più mapping per la risorsa umana con Id {0}", r.Cod_RisUm)
                        Throw New Exception(messaggioErrore)
                    End If

                    Dim rmDestFirst = rmDest.FirstOrDefault()
                    If rmDestFirst IsNot Nothing Then
                        rm.Cod_RisUM_Nuovo = rmDestFirst.Cod_RisUm
                        rm.PivaDest = rmDestFirst.Piva
                    End If

                End If

                risultato.Add(rm)

            End Function)


        Return risultato

    End Function

    Private Sub InizializzaStack()

        Dim stb As New System.Text.StringBuilder

        ' ******** RISORSE UMANE *******
        stb.AppendLine(String.Format(" update Movimenti_dettagli set Mat_cod = {0} where piva = '{1}' and Mat_Cod = {2} and Elem_Cod = 0 ", TAG_COD_RISUM_DEST, TAG_PIVA, TAG_COD_RISUM_ORIGINE))
        _repointerStack.Add(New RepointElement With {.Tabella = "Movimenti_dettagli", .Descrizione = "Manodoporera", .Sql = stb.ToString})
        stb.Clear()

        stb.AppendLine(String.Format(" update Prodotti_Costi set Mat_Cod = {0} where piva = '{1}' and Mat_Cod = {2} and elem_cod = 0 and Id_Budget = 0", TAG_COD_RISUM_DEST, TAG_PIVA, TAG_COD_RISUM_ORIGINE))
        _repointerStack.Add(New RepointElement With {.Tabella = "Prodotti_Costi", .Descrizione = "Costo", .Sql = stb.ToString})
        stb.Clear()

        stb.AppendLine(String.Format(" update movimenti set Cod_RisUm = {0} where piva = '{1}' and Cod_RisUm = {2}", TAG_COD_RISUM_DEST, TAG_PIVA, TAG_COD_RISUM_ORIGINE))
        _repointerStack.Add(New RepointElement With {.Tabella = "Movimenti", .Descrizione = "Intestatario Doc", .Sql = stb.ToString})
        stb.Clear()

        stb.AppendLine(String.Format(" update movimenti set cod_vettore = {0} where piva = '{1}' and cod_vettore = {2}", TAG_COD_RISUM_DEST, TAG_PIVA, TAG_COD_RISUM_ORIGINE))
        _repointerStack.Add(New RepointElement With {.Tabella = "Movimenti", .Descrizione = "Vettore", .Sql = stb.ToString})
        stb.Clear()

        stb.AppendLine(String.Format(" update movimenti set Cod_Destinazione = {0} where piva = '{1}' and Cod_Destinazione = {2}", TAG_COD_RISUM_DEST, TAG_PIVA, TAG_COD_RISUM_ORIGINE))
        _repointerStack.Add(New RepointElement With {.Tabella = "Movimenti", .Descrizione = "Destinazione diversa", .Sql = stb.ToString})
        stb.Clear()

        stb.AppendLine(String.Format(" update movimenti set Cod_RisUm_Altro = {0} where piva = '{1}' and Cod_RisUm_Altro = {2}", TAG_COD_RISUM_DEST, TAG_PIVA, TAG_COD_RISUM_ORIGINE))
        _repointerStack.Add(New RepointElement With {.Tabella = "Movimenti", .Descrizione = " Conferimento", .Sql = stb.ToString})
        stb.Clear()

        stb.AppendLine(String.Format(" update movimenti set Cod_RisUm_Aggiuntivo = {0} where piva = '{1}' and Cod_RisUm_Aggiuntivo = {2}", TAG_COD_RISUM_DEST, TAG_PIVA, TAG_COD_RISUM_ORIGINE))
        _repointerStack.Add(New RepointElement With {.Tabella = "Movimenti", .Descrizione = " Terzo Cedente / Cessionario", .Sql = stb.ToString})
        stb.Clear()

        stb.AppendLine(String.Format(" update Mov_Dettaglio_Tecnico_Extra set ACCDAA_Cod_Risum_Destinatario = {0} where piva = '{1}' and ACCDAA_Cod_Risum_Destinatario = {2}", TAG_COD_RISUM_DEST, TAG_PIVA, TAG_COD_RISUM_ORIGINE))
        _repointerStack.Add(New RepointElement With {.Tabella = "Mov_Dettaglio_Tecnico_Extra", .Descrizione = "Destinatario DAA", .Sql = stb.ToString})
        stb.Clear()

        stb.AppendLine(String.Format(" update Mov_Dettaglio_Tecnico_Extra set ACCDAA_Cod_Risum_Destinazione = {0} where piva = '{1}' and ACCDAA_Cod_Risum_Destinazione = {2}", TAG_COD_RISUM_DEST, TAG_PIVA, TAG_COD_RISUM_ORIGINE))
        _repointerStack.Add(New RepointElement With {.Tabella = "Mov_Dettaglio_Tecnico_Extra", .Descrizione = "Destinazione DAA", .Sql = stb.ToString})
        stb.Clear()

        stb.AppendLine(String.Format(" update Mov_Dettaglio_Tecnico_Extra set Agente_Cod = {0} where piva = '{1}' and Agente_Cod = {2}", TAG_COD_RISUM_DEST, TAG_PIVA, TAG_COD_RISUM_ORIGINE))
        _repointerStack.Add(New RepointElement With {.Tabella = "Mov_Dettaglio_Tecnico_Extra", .Descrizione = "Agente", .Sql = stb.ToString})
        stb.Clear()

        stb.AppendLine(String.Format(" update Mov_Dettaglio_Tecnico_Extra set CapoArea_Cod = {0} where piva = '{1}' and CapoArea_Cod = {2}", TAG_COD_RISUM_DEST, TAG_PIVA, TAG_COD_RISUM_ORIGINE))
        _repointerStack.Add(New RepointElement With {.Tabella = "Mov_Dettaglio_Tecnico_Extra", .Descrizione = "Capo Area", .Sql = stb.ToString})
        stb.Clear()

        stb.AppendLine(String.Format(" update CDG_Testata set Cod_RisUm = {0} where piva = '{1}' and Cod_RisUm = {2} and Budget = 0", TAG_COD_RISUM_DEST, TAG_PIVA, TAG_COD_RISUM_ORIGINE))
        _repointerStack.Add(New RepointElement With {.Tabella = "CDG_Testata", .Descrizione = "Costo", .Sql = stb.ToString})
        stb.Clear()


        ''stb.AppendLine(String.Format(" update Analisi_Tipologia_Laboratori set Cod_Risum = {0}
        ''        where piva = '{1}' and Cod_RisUm = {2}", TAG_COD_RISUM_DEST, TAG_PIVA, TAG_COD_RISUM_ORIGINE))
        ''_repointerStack.Add(New RepointElement With {.Tabella = "Analisi_Tipologia_Laboratori", .Descrizione = "Lab. Analisi", .Sql = stb.ToString})
        ''stb.Clear()

        ''stb.AppendLine(String.Format(" update Analisi_Certificato set Analisi_Certificato_Laboratorio = {0}
        ''        where piva = '{1}' and Analisi_Certificato_Laboratorio = {2}", TAG_COD_RISUM_DEST, TAG_PIVA, TAG_COD_RISUM_ORIGINE))
        ''_repointerStack.Add(New RepointElement With {.Tabella = "Analisi_Certificato", .Descrizione = "Lab. Analisi", .Sql = stb.ToString})
        ''stb.Clear()

        stb.AppendLine(String.Format(" update Pagamenti set Tipo_Cod_Dare = {0} where piva = '{1}' and Tipo_Cod_Dare = {2} and Tipo_Dare = 1 and Tipo_Cod_Dare <> 0", TAG_COD_RISUM_DEST, TAG_PIVA, TAG_COD_RISUM_ORIGINE))
        _repointerStack.Add(New RepointElement With {.Tabella = "Pagamenti", .Descrizione = "Partita Doppia Dare", .Sql = stb.ToString})
        stb.Clear()

        stb.AppendLine(String.Format(" update Pagamenti set Tipo_Cod_Avere = {0} where piva = '{1}' and Tipo_Cod_Avere = {2} and Tipo_Avere = 1 and Tipo_Cod_Avere <> 0", TAG_COD_RISUM_DEST, TAG_PIVA, TAG_COD_RISUM_ORIGINE))
        _repointerStack.Add(New RepointElement With {.Tabella = "Pagamenti", .Descrizione = "Partita Doppia Avere", .Sql = stb.ToString})
        stb.Clear()

        For Each e As RepointElement In _repointerStack
            e.IdGruppo = "RISUM"
        Next

        ' ******** INDIRIZZI *******
        stb.AppendLine(String.Format(" update Mov_Dettaglio_Tecnico_Extra set ACCDAA_Cod_IndirizzoRisum_Destinatario = {0} where piva = '{1}' and ACCDAA_Cod_IndirizzoRisum_Destinatario = {2}", TAG_COD_INDIRIZZO_DEST, TAG_PIVA, TAG_COD_INDIRIZZO_ORIGINE))
        _repointerStack.Add(New RepointElement With {.Tabella = "Mov_Dettaglio_Tecnico_Extra", .Descrizione = "DAA Inidirzzo Destinatario", .Sql = stb.ToString})
        stb.Clear()

        stb.AppendLine(String.Format(" update Mov_Dettaglio_Tecnico_Extra set ACCDAA_Cod_IndirizzoRisum_Destinazione = {0} where piva = '{1}' and ACCDAA_Cod_IndirizzoRisum_Destinazione = {2}", TAG_COD_INDIRIZZO_DEST, TAG_PIVA, TAG_COD_INDIRIZZO_ORIGINE))
        _repointerStack.Add(New RepointElement With {.Tabella = "Mov_Dettaglio_Tecnico_Extra", .Descrizione = "DAA Inidirzzo Destinazione", .Sql = stb.ToString})
        stb.Clear()

        stb.AppendLine(String.Format(" update Movimenti set Cod_Indirizzo_Aggiuntivo = {0} where piva = '{1}' and Cod_Indirizzo_Aggiuntivo = {2}", TAG_COD_INDIRIZZO_DEST, TAG_PIVA, TAG_COD_INDIRIZZO_ORIGINE))
        _repointerStack.Add(New RepointElement With {.Tabella = "Movimenti", .Descrizione = "Indirizzo Aggiuntivo", .Sql = stb.ToString})
        stb.Clear()

        stb.AppendLine(String.Format(" update Movimenti set Cod_IndirizzoDestinazione = {0} where piva = '{1}' and Cod_IndirizzoDestinazione = {2}", TAG_COD_INDIRIZZO_DEST, TAG_PIVA, TAG_COD_INDIRIZZO_ORIGINE))
        _repointerStack.Add(New RepointElement With {.Tabella = "Movimenti", .Descrizione = "Indirizzo Destinazione", .Sql = stb.ToString})
        stb.Clear()

        stb.AppendLine(String.Format(" update Movimenti set Cod_IndirizzoRisUm = {0} where piva = '{1}' and Cod_IndirizzoRisUm = {2}", TAG_COD_INDIRIZZO_DEST, TAG_PIVA, TAG_COD_INDIRIZZO_ORIGINE))
        _repointerStack.Add(New RepointElement With {.Tabella = "Movimenti", .Descrizione = "Indirizzo", .Sql = stb.ToString})
        stb.Clear()

        stb.AppendLine(String.Format(" update Movimenti set Cod_IndirizzoVettore = {0} where piva = '{1}' and Cod_IndirizzoVettore = {2}", TAG_COD_INDIRIZZO_DEST, TAG_PIVA, TAG_COD_INDIRIZZO_ORIGINE))
        _repointerStack.Add(New RepointElement With {.Tabella = "Movimenti", .Descrizione = "Indirizzo Vettore", .Sql = stb.ToString})
        stb.Clear()

        For Each e As RepointElement In _repointerStack.Where(Function(s) String.IsNullOrEmpty(s.IdGruppo))
            e.IdGruppo = "INDIRIZZO"
        Next

    End Sub

    Private Class RepointElement : Inherits Copia_Contatto_Stack_Element
        Public Descrizione As String
        Public IdGruppo As String
    End Class

End Class

Public Class Copia_Contatto_Privatizzatore
    Inherits AgronicaCoreDataProvider.DataProvider

    Private _objParametri_Server As AgronicaCoreParametri
    Private _objParametri_Utenti As AgronicaCoreParametri
    Private _privatizzatoreStack As List(Of Copia_Contatto_Stack_Element) = New List(Of Copia_Contatto_Stack_Element)

    Private Const TAG_PIVA As String = "#PIVA#"
    Private Const TAG_COD_CONTATTO As String = "#COD_CONTATTO#"
    Private Const TAG_DATA_MODIFICA As String = "#DATA_MODIFICA#"

    Public Sub New(ByRef objParametri_Server As AgronicaCoreParametri,
                   ByRef objParametri_Utenti As AgronicaCoreParametri)
        _objParametri_Server = objParametri_Server
        _objParametri_Utenti = objParametri_Utenti
        InizializzaStack()

    End Sub

    Public Function Privatizza(ByVal contatto As ParametroCopiaContatto,
                               ByRef errori As List(Of String)) As Boolean

        Dim messaggioErrore As String
        Dim nomeProcedura As String = "Copia_Contatto_Privatizzatore.Privatizza"
        Dim risultato As Boolean = False
        Dim dataModifica As String = UtilityProvider.Agro_SQL_SaveDateTime(DateTime.Now)

        Try
            For Each re As Copia_Contatto_Stack_Element In _privatizzatoreStack

                Dim sql = re.Sql.Replace(TAG_PIVA, contatto.PivaOrigine)
                sql = sql.Replace(TAG_COD_CONTATTO, contatto.CodContatto)
                sql = sql.Replace(TAG_DATA_MODIFICA, dataModifica)
                EseguiQuery_Scrittura(_objParametri_Server, sql, nomeProcedura)
            Next

            risultato = True

        Catch ex As Exception
            messaggioErrore = ex.Message
            errori.Add(messaggioErrore)
            Scrivi_LOG(_objParametri_Server, nomeProcedura, messaggioErrore)
            Throw ex
        End Try

        Return risultato

    End Function
    Private Sub InizializzaStack()

        Dim stb As New System.Text.StringBuilder

        stb.AppendLine(String.Format(" update Contatti set sa_cod = 0, data_modifica = {2} where piva = '{0}' and Cod_Contatto = '{1}' ", TAG_PIVA, TAG_COD_CONTATTO, TAG_DATA_MODIFICA))
        _privatizzatoreStack.Add(New Copia_Contatto_Stack_Element With {.Tabella = "Contatti", .Sql = stb.ToString})
        stb.Clear()

        stb.AppendLine(String.Format(" update Risorse_Umane set sa_cod = 0, data_modifica = {2} where piva = '{0}' and Cod_Contatto = '{1}' ", TAG_PIVA, TAG_COD_CONTATTO, TAG_DATA_MODIFICA))
        _privatizzatoreStack.Add(New Copia_Contatto_Stack_Element With {.Tabella = "Risorse_Umane", .Sql = stb.ToString})
        stb.Clear()

        stb.AppendLine(String.Format(" update Contatti_Codici set sa_cod = 0, data_modifica = {2} where piva = '{0}' and Cod_Contatto = '{1}' ", TAG_PIVA, TAG_COD_CONTATTO, TAG_DATA_MODIFICA))
        _privatizzatoreStack.Add(New Copia_Contatto_Stack_Element With {.Tabella = "Contatti_Codici", .Sql = stb.ToString})
        stb.Clear()

        stb.AppendLine(String.Format(" update ContattiXIndirizzi set sa_cod = 0, data_modifica = {2} where piva = '{0}' and Cod_Contatto = '{1}' ", TAG_PIVA, TAG_COD_CONTATTO, TAG_DATA_MODIFICA))
        _privatizzatoreStack.Add(New Copia_Contatto_Stack_Element With {.Tabella = "ContattiXIndirizzi", .Sql = stb.ToString})
        stb.Clear()

        stb.AppendLine(String.Format(" update Liquidita set sa_cod = 0 where piva = '{0}' and Cod_Contatto = '{1}' ", TAG_PIVA, TAG_COD_CONTATTO))
        _privatizzatoreStack.Add(New Copia_Contatto_Stack_Element With {.Tabella = "Liquidita", .Sql = stb.ToString})
        stb.Clear()

        stb.AppendLine(String.Format(" update ContattiXRubrica set sa_cod = 0, data_modifica = {2} where piva = '{0}' and Cod_Contatto = '{1}' ", TAG_PIVA, TAG_COD_CONTATTO, TAG_DATA_MODIFICA))
        _privatizzatoreStack.Add(New Copia_Contatto_Stack_Element With {.Tabella = "ContattiXRubrica", .Sql = stb.ToString})
        stb.Clear()

        'TODO parco_Macchine (Cod_RisUm)

    End Sub

End Class

Public Class Copia_Contatto_Stack_Element
    Public Tabella As String
    Public Sql As String
End Class

Public Class Copia_Contatto_Preparer
    Inherits AgronicaCoreDataProvider.LogProvider

    Private _objParametri_Server As AgronicaCoreParametri
    Private _objParametri_Utenti As AgronicaCoreParametri
    Private _gestisciDatiContabili As Boolean = False
    Public Sub New(ByVal gestisciDatiContabili As Boolean,
                  ByRef objParametri_Server As AgronicaCoreParametri,
                  ByRef objParametri_Utenti As AgronicaCoreParametri)

        _objParametri_Server = objParametri_Server
        _objParametri_Utenti = objParametri_Utenti
        _gestisciDatiContabili = gestisciDatiContabili

    End Sub

    Public Function Prepare(ByVal pivaOrigine As String,
                            ByVal pivaDest As String,
                            ByVal contatto As ContattoModel,
                            ByVal indiceProgressivoGias As Integer,
                            ByRef errori As List(Of String)) As XmlElement

        Dim Provider = Globalization.CultureInfo.InvariantCulture
        Dim Format As String = "yyyyMMdd"
        Dim messaggioErrore As String = ""
        Dim nomeProcedura As String = "Copia_Contatto_Preparer.Prepare"

        Dim objXML_Utility As New AgronicaCoreXML.XML_Utility
        Dim DT_RisUm As DataTable = objXML_Utility.CaricaGriglia_RisUm_for_XML()
        Dim Dt_Prodotti As DataTable = objXML_Utility.CaricaGriglia_ProdottiCosti_for_XML()
        Dim Dt_Indirizzi As DataTable = objXML_Utility.CaricaGriglia_Indirizzi_for_XML()
        Dim DT_Codici As DataTable = objXML_Utility.CaricaGriglia_CodiciContatto_for_XML()
        Dim DT_Liquidita As DataTable = objXML_Utility.CaricaGriglia_Liquidita_for_XML()
        Dim DT_Conti As DataTable = objXML_Utility.CaricaGriglia_Conti_for_XML()
        Dim Dt_Rubrica As DataTable = objXML_Utility.CaricaGriglia_Rubrica_for_XML()

        Dim Nome As String = contatto.Nome
        Dim Cognome As String = contatto.Cognome
        Dim Rag_Soc As String = contatto.Rag_Soc
        Dim COnvenevoli As String = contatto.Convenevoli
        Dim nrBadge As String = contatto.Badge
        Dim sesso As String = ""
        Dim id_cf As Integer
        Dim Cod_Contatto As String = ""
        Dim CodiceFiscaleEstero As String = If(String.IsNullOrEmpty(contatto.CF_Estero), "", contatto.CF_Estero)
        Dim Fittizio As Boolean = contatto.AltriDati.Fittizio
        Dim dataNascita As Date = AGRODATAINIZIO
        Dim streerore As String = ""
        Dim tipoOperazioneDB_Contatto As Integer = enum_TipoOperazioneDB.Scrittura
        Dim noteOperazioni As String = If(String.IsNullOrEmpty(contatto.AltriDati.NoteOperazioni), "", "|" & contatto.AltriDati.NoteOperazioni & "|")
        Dim noteOperazioni2 As String = If(String.IsNullOrEmpty(contatto.AltriDati.NoteOperazioni2), "", "|" & contatto.AltriDati.NoteOperazioni2 & "|")

        Dim tipoSpeditore As Integer = If(String.IsNullOrEmpty(contatto.AltriDati.OrigineDestinazione), 0, CInt(contatto.AltriDati.OrigineDestinazione))
        Dim tipoDestinazione As Integer = If(String.IsNullOrEmpty(contatto.AltriDati.Tipo_destinazione), 0, CInt(contatto.AltriDati.Tipo_destinazione))
        Dim destinazioneDiversaCod As Integer = If(String.IsNullOrEmpty(contatto.DettagliContabili.dettCont_destinazione_diversa), 0, CInt(contatto.DettagliContabili.dettCont_destinazione_diversa))

        'TODO
        Dim tipIndDestDiversa_Cod As Integer = 0 'If(String.IsNullOrEmpty(contatto.DettagliContabili.IndirizzoDestinazioneDiversa_Cod), 0, CInt(contatto.DettagliContabili.IndirizzoDestinazioneDiversa_Cod))
        Dim indirizzoFatturazione As Integer = 0 'If(String.IsNullOrEmpty(contatto.DettagliContabili.de), 0, CInt(contatto.DettagliContabili.IndirizzoFatturazione_Cod))
        Dim Cod_Fisc_giuridica As String

        Dim xContatto As XmlElement

        Try

            id_cf = contatto.ID_CF
            ' PERSONA FISICA / GIURIDICA
            If id_cf = "0" Then
                ' fISICA
                Rag_Soc = ""
                sesso = contatto.Sesso
                Cod_Contatto = contatto.CF.Trim()
                If Not String.IsNullOrEmpty(contatto.DataNascita) AndAlso IsDate(contatto.DataNascita) Then
                    dataNascita = CDate(contatto.DataNascita)
                End If
                Cod_Fisc_giuridica = contatto.CF
            Else
                ' Giuridica
                Nome = ""
                Cognome = ""
                sesso = ""
                Cod_Contatto = contatto.Piva.Trim()
                Cod_Fisc_giuridica = Cod_Contatto
            End If

            ' ***********************************************************************
            ' INDIRIZZI
            ' ***********************************************************************
            PreparaSalvataggioIndirizzi(pivaOrigine, pivaDest, Cod_Contatto, contatto.Indirizzi, Dt_Indirizzi)

            ' *************************************************************************
            ' RUBRICA
            ' *************************************************************************
            PreparaSalvataggioRubrica(contatto.Rubrica, Dt_Rubrica)

            '**************************************************************************
            ' RAPPORTI CONTABILI
            '**************************************************************************
            PreparaSalvataggioRapportiContabili(pivaOrigine, pivaDest, Cod_Contatto,
                                                contatto.RapportiContabili, DT_RisUm)

            '**************************************************************************
            ' COSTI
            '**************************************************************************
            PreparaSalvataggioCosti(pivaOrigine, pivaDest, contatto.Costi, Dt_Prodotti)

            PreparaSalvataggioProdottiCosti(pivaOrigine, pivaDest, DT_RisUm, Dt_Prodotti)

            '**************************************************************************
            ' CODICI
            '**************************************************************************
            PreparaSalvataggioCodici(pivaOrigine, pivaDest, Cod_Contatto, contatto.Sa_Cod,
                                     contatto.ContattiCodici, DT_Codici)

            If (_gestisciDatiContabili) Then
                '**************************************************************************
                ' LIQUIDITA
                '**************************************************************************
                PreparaSalvataggioLiquidita(pivaOrigine, pivaDest, Cod_Contatto, contatto.Sa_Cod,
                                            contatto.Liquidita, DT_Liquidita)

                '**************************************************************************
                ' CONTI
                '**************************************************************************
                PreparaSalvataggioConti(pivaOrigine, pivaDest, Cod_Contatto, contatto.Conti, DT_Conti)

            End If


            '**************************************************************************
            ' DETTAGLI CONTABILI
            '**************************************************************************

            Dim arrayScontiAdd = New List(Of String)
            If Not String.IsNullOrEmpty(contatto.DettagliContabili.dettCont_sconto_add1) Then
                arrayScontiAdd.Add(contatto.DettagliContabili.dettCont_sconto_add1)
            End If
            If Not String.IsNullOrEmpty(contatto.DettagliContabili.dettCont_sconto_add2) Then
                arrayScontiAdd.Add(contatto.DettagliContabili.dettCont_sconto_add2)
            End If
            If Not String.IsNullOrEmpty(contatto.DettagliContabili.dettCont_sconto_add3) Then
                arrayScontiAdd.Add(contatto.DettagliContabili.dettCont_sconto_add3)
            End If
            Dim ScontoTesto As String = If(arrayScontiAdd.Any(), String.Join("-", arrayScontiAdd), "")
            Dim Agente_Cod As Integer = If(String.IsNullOrEmpty(contatto.DettagliContabili.dettCont_agente_cod), 0, CInt(contatto.DettagliContabili.dettCont_agente_cod))
            Dim CapoArea_Cod As Integer = If(String.IsNullOrEmpty(contatto.DettagliContabili.dettCont_capoarea_cod), 0, CInt(contatto.DettagliContabili.dettCont_capoarea_cod))
            Dim Vettore_Cod As Integer = If(String.IsNullOrEmpty(contatto.DettagliContabili.dettCont_vettore_cod), 0, CInt(contatto.DettagliContabili.dettCont_vettore_cod))
            Dim Modalita_Fatturazione As Integer = If(String.IsNullOrEmpty(contatto.DettagliContabili.dettCont_fatturazione_automatica), 0, CInt(contatto.DettagliContabili.dettCont_fatturazione_automatica))
            Dim Cod_Iva_Contatto As Integer = If(String.IsNullOrEmpty(contatto.DettagliContabili.dettCont_iva_default), 0, CInt(contatto.DettagliContabili.dettCont_iva_default))
            Dim Cod_Conto_Economico_Default As Integer = If(String.IsNullOrEmpty(contatto.DettagliContabili.dettCont_cod_conto_economico_default), 0, CInt(contatto.DettagliContabili.dettCont_cod_conto_economico_default))
            Dim Cod_Conto_Patrimoniale_Default As Integer = If(String.IsNullOrEmpty(contatto.DettagliContabili.dettCont_cod_conto_patrimoniale_default), 0, CInt(contatto.DettagliContabili.dettCont_cod_conto_patrimoniale_default))
            Dim Provvigione As Decimal = If(String.IsNullOrEmpty(contatto.DettagliContabili.dettCont_provvigione_agente), 0, CDbl(contatto.DettagliContabili.dettCont_provvigione_agente))
            Dim Provvigione_CapoArea As Decimal = If(String.IsNullOrEmpty(contatto.DettagliContabili.dettCont_provvigione_capoarea), 0, CDbl(contatto.DettagliContabili.dettCont_provvigione_capoarea))

            'TODO
            Dim TipoInidrizzoDefault As Integer = 0 'If(String.IsNullOrEmpty(contatto.DettagliContabili.dettCont_provvigione_capoarea), 0, CInt(parametri.DettagliContabilita.TipoIndirizzoDefault))

            Dim objxml As New AgronicaCoreXML.XML_Anagrafe
            Dim objContattoW As New AgronicaCoreAnagrafeBIZ.Contatti_W
            Dim xmlDoc As XmlDocument = Nothing
            Dim Base, Top As Integer

            Call Calcola_BaseCode_TopCode(Base, Top, indiceProgressivoGias)

            xContatto = objxml.XML_2_Contatti(streerore,
                            xmlDoc, Base, Top, False, tipoOperazioneDB_Contatto, pivaDest, Cod_Contatto,
                            Dt_Indirizzi, DT_RisUm, contatto.Sa_Cod, id_cf, Rag_Soc, COnvenevoli,
                            Cod_Fisc_giuridica, indirizzoFatturazione, Nome, Cognome, dataNascita, sesso, , , ,
                            Dt_Rubrica, DT_Codici,
                            nrBadge:=contatto.Badge,
                            Tipo_Speditore:=tipoSpeditore,
                            Tipo_Destinazione:=tipoDestinazione,
                            Note:=contatto.AltriDati.Note,
                            Note2:=contatto.AltriDati.Note2,
                            Note_Operazioni:=noteOperazioni,
                            Note2_Operazioni:=noteOperazioni2,
                            Memo:=contatto.AltriDati.Memo,
                            DT_Liquidita:=DT_Liquidita,
                            Sconto_Testo:=ScontoTesto,
                            Agente_Cod:=Agente_Cod,
                            CapoArea_Cod:=CapoArea_Cod,
                            Vettore_Cod:=Vettore_Cod,
                            Modalita_Fatturazione:=Modalita_Fatturazione,
                            Cod_Iva_Contatto:=Cod_Iva_Contatto,
                            Cod_Conto_Economico_Default:=Cod_Conto_Economico_Default,
                            Cod_Conto_Patrimoniale_Default:=Cod_Conto_Patrimoniale_Default,
                            Provvigione:=Provvigione,
                            Provvigione_CapoArea:=Provvigione_CapoArea,
                            DT_Conti:=DT_Conti,
                            Cod_Risum_Destinazione_Diversa:=destinazioneDiversaCod,
                            Tipo_Indirizzo_Default_Destinazione_Diversa:=tipIndDestDiversa_Cod,
                            ChkFittizio:=Fittizio
                            )

            If Not String.IsNullOrEmpty(streerore) Then
                Throw New Exception(streerore)
            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            errori.Add(messaggioErrore)
            Scrivi_LOG(_objParametri_Server, nomeProcedura, messaggioErrore)
            Return Nothing
        End Try

        Return xContatto

    End Function

    Private Sub PreparaSalvataggioConti(ByVal pivaOrigine As String,
                                        ByVal pivaDest As String,
                                        ByVal codContatto As String,
                                        ByVal conti As List(Of ContiModel),
                                        ByRef DT_Conti As DataTable)

        Dim objXML_Utility As New AgronicaCoreXML.XML_Utility

        Dim ricxc As New RicxConti_R

        For Each l As ContiModel In conti

            Dim Cod_Conto As Integer = If(l.Cod_Conto Is Nothing, 0, l.Cod_Conto)
            Dim Validita_Inizio As DateTime = If(l.Validita_Inizio Is Nothing, AGRODATAINIZIO, l.Validita_Inizio)
            Dim Validita_Fine As DateTime = If(l.Validita_Fine Is Nothing, AGRODATAFINE, l.Validita_Fine)

            Dim dtCercaConti = ricxc.Leggi(pivaDest, 2, 0, Cod_Conto, 0, "", "", "", _objParametri_Server)
            If dtCercaConti.Rows.Count > 0 Then
                objXML_Utility.Inserisci_Riga_Dt_Conti_for_XML(
                DT_Conti, enum_TipoOperazioneDB.Scrittura, pivaDest, Cod_Conto, codContatto, Validita_Inizio, Validita_Fine)
            End If

        Next

    End Sub

    Private Sub PreparaSalvataggioLiquidita(ByVal pivaOrigine As String,
                                            ByVal pivaDest As String,
                                            ByVal codContatto As String,
                                            ByVal visibilita As Integer,
                                            ByVal liquidita As List(Of LiquiditaModel),
                                            ByRef DT_Liquidita As DataTable)

        Dim objXML_Utility As New AgronicaCoreXML.XML_Utility

        Dim tipoOperazioneDB As Integer

        For Each l As LiquiditaModel In liquidita

            If l.Cod_Liquidita = 0 Then
                tipoOperazioneDB = enum_TipoOperazioneDB.Scrittura
            Else
                tipoOperazioneDB = enum_TipoOperazioneDB.Modifica
            End If

            Dim Sa_Cod = visibilita
            Dim Cod_Istituto As Integer = If(l.Cod_Istituto Is Nothing, 0, l.Cod_Istituto)
            Dim Nazione As String = If(l.Nazione Is Nothing, "", l.Nazione)
            Dim Cifre_Controllo As String = If(l.Cifre_Controllo Is Nothing, "", l.Cifre_Controllo)
            Dim Cin As String = If(l.Cin Is Nothing, "", l.Cin)
            Dim Abi As String = If(l.Abi Is Nothing, "", l.Abi)
            Dim Cab As String = If(l.Cab Is Nothing, "", l.Cab)
            Dim Numero As String = If(l.Numero Is Nothing, "", l.Numero)
            Dim Bic As String = If(l.Bic Is Nothing, "", l.Bic)
            Dim ChkAbilitazione As Integer = If(l.ChkAbilitazione Is Nothing, 0, l.ChkAbilitazione)
            Dim Validita_Inizio As DateTime = If(l.Validita_Inizio Is Nothing, AGRODATAINIZIO, l.Validita_Inizio)
            Dim Validita_Fine As DateTime = If(l.Validita_Fine Is Nothing, AGRODATAFINE, l.Validita_Fine)
            Dim Note As String = If(l.Note Is Nothing, "", l.Note)

            Dim ChkDefault As Integer = 0
            If Not IsNothing(l.ChkDefault) Then

                ChkDefault = Convert.ToInt32(l.ChkDefault)
            End If

            objXML_Utility.Inserisci_Riga_Dt_Liquidita_for_XML(
                DT_Liquidita, tipoOperazioneDB, pivaDest, Sa_Cod,
                codContatto, codContatto, CStr(enum_Liquidita_CauRisorsa.RisorsaFinanziaria), "", "", l.Cod_Liquidita, Cod_Istituto, Nazione, Cifre_Controllo, Cin,
                Abi, Cab, Numero, Bic, ChkAbilitazione, ChkDefault, Validita_Inizio, Validita_Fine, Note)

        Next


    End Sub

    Private Sub PreparaSalvataggioRapportiContabili(
            ByVal pivaOrigine As String,
            ByVal pivaDset As String,
            ByVal codContatto As String,
            ByVal rapportiContabili As List(Of RapportoContabileModel),
            ByRef DT_RisUm As DataTable
        )

        Dim objXML_Utility As New AgronicaCoreXML.XML_Utility
        Dim cod_rapp As Integer
        Dim cod_risum As Integer
        Dim tipoOperazioneDB_RapCont As Integer
        Dim validita_inizio As DateTime = AGRODATAINIZIO
        Dim validita_fine As DateTime = AGRODATAFINE
        Dim progressivo As String = ""
        Dim attivita As String = ""
        Dim numero_patentino As String = ""
        Dim Ente_di_rilascio As String = ""
        Dim data_rilascio As DateTime = AGRODATAINIZIO
        Dim data_scadenza As DateTime = AGRODATAFINE
        Dim oreSettimanali As Decimal = 0
        Dim occasionale As Integer = 0
        Dim mansioneCod As Integer = 0
        Dim classificazioneCod As Integer = 0
        Dim qualificaCod As Integer = 0
        Dim infoFamiglia As String = ""


        For Each rc As RapportoContabileModel In rapportiContabili
            cod_risum = If(rc.Cod_RisUm Is Nothing, 0, rc.Cod_RisUm)
            tipoOperazioneDB_RapCont = enum_TipoOperazioneDB.Scrittura
            cod_rapp = rc.Cod_Rapporto
            validita_inizio = rc.Validita_Inizio
            validita_fine = rc.Validita_Fine
            progressivo = rc.Settore_Des
            attivita = rc.Attivita_Des
            oreSettimanali = rc.Ore_Settimanali
            occasionale = If(rc.occasionale Is Nothing, 0, rc.occasionale)
            mansioneCod = If(rc.Mansione_Cod Is Nothing, 0, rc.Mansione_Cod)
            occasionale = If(rc.TipoRapporto_Cod Is Nothing, 0, rc.TipoRapporto_Cod)
            classificazioneCod = If(rc.Classificazione_Cod Is Nothing, 0, rc.Classificazione_Cod)
            qualificaCod = If(rc.Qualifica_Cod Is Nothing, 0, rc.Qualifica_Cod)
            infoFamiglia = rc.Info_Famiglia

            objXML_Utility.Inserisci_Riga_Dt_RisUm_for_XML(DT_RisUm,
                                                          tipoOperazioneDB_RapCont,
                                                          pivaDset,
                                                          codContatto, ,
                                                          cod_risum,
                                                          cod_rapp,
                                                          progressivo,
                                                          attivita, occasionale, oreSettimanali, , , ,
                                                          numero_patentino,
                                                          data_rilascio,
                                                          data_scadenza, , ,
                                                          validita_inizio,
                                                          validita_fine,
                                                          Nothing,
                                                          Ente_di_rilascio, , , ,
                                                          qualificaCod,
                                                          mansioneCod,
                                                          classificazioneCod,
                                                          infoFamiglia)

        Next

    End Sub

    Private Sub PreparaSalvataggioCodici(
        ByVal pivaOrigine As String,
        ByVal pivaDest As String,
        ByVal codContatto As String,
        ByVal visibilita As Integer,
        ByVal codici As List(Of ContattoCodiceModel),
        ByRef DT_Codici As DataTable)

        Dim objXML_Utility As New AgronicaCoreXML.XML_Utility

        Dim valore As ContattoCodiceModel = Nothing

        ' pec
        valore = codici.FirstOrDefault(Function(s) s.ID_Cod = enum_CodiciAnagrafe.PecContatto)
        objXML_Utility.Inserisci_Riga_Dt_CodiciContatto_for_XML(DT_Codici, enum_TipoOperazioneDB.Cancellazione,
                                                                pivaDest, codContatto, enum_CodiciAnagrafe.PecContatto,
                                                                visibilita, If(valore Is Nothing, 0, valore.Val_Cod), AGRODATAINIZIO, AGRODATAFINE)
        objXML_Utility.Inserisci_Riga_Dt_CodiciContatto_for_XML(DT_Codici, enum_TipoOperazioneDB.Scrittura,
                                                                pivaDest, codContatto, enum_CodiciAnagrafe.PecContatto,
                                                                visibilita, If(valore Is Nothing, 0, valore.Val_Cod), AGRODATAINIZIO, AGRODATAFINE)

        ' codice sdi
        valore = codici.FirstOrDefault(Function(s) s.ID_Cod = enum_CodiciAnagrafe.CodiceSDI)
        objXML_Utility.Inserisci_Riga_Dt_CodiciContatto_for_XML(DT_Codici, enum_TipoOperazioneDB.Cancellazione,
                                                                pivaDest, codContatto, enum_CodiciAnagrafe.CodiceSDI,
                                                                visibilita, If(valore Is Nothing, 0, valore.Val_Cod), AGRODATAINIZIO, AGRODATAFINE)

        objXML_Utility.Inserisci_Riga_Dt_CodiciContatto_for_XML(DT_Codici, enum_TipoOperazioneDB.Scrittura,
                                                                pivaDest, codContatto, enum_CodiciAnagrafe.CodiceSDI,
                                                                visibilita, If(valore Is Nothing, 0, valore.Val_Cod), AGRODATAINIZIO, AGRODATAFINE)

        ' tipo Contatto
        valore = codici.FirstOrDefault(Function(s) s.ID_Cod = enum_CodiciAnagrafe.TipoContattoFattura)
        objXML_Utility.Inserisci_Riga_Dt_CodiciContatto_for_XML(DT_Codici, enum_TipoOperazioneDB.Cancellazione,
                                                               pivaDest, codContatto, enum_CodiciAnagrafe.TipoContattoFattura,
                                                               visibilita, If(valore Is Nothing, 0, valore.Val_Cod), AGRODATAINIZIO, AGRODATAFINE)
        objXML_Utility.Inserisci_Riga_Dt_CodiciContatto_for_XML(DT_Codici, enum_TipoOperazioneDB.Scrittura,
                                                                pivaDest, codContatto, enum_CodiciAnagrafe.TipoContattoFattura,
                                                                visibilita, If(valore Is Nothing, 0, valore.Val_Cod), AGRODATAINIZIO, AGRODATAFINE)

        ' rap fiscale
        valore = codici.FirstOrDefault(Function(s) s.ID_Cod = enum_CodiciAnagrafe.RappresentanteFiscale)
        objXML_Utility.Inserisci_Riga_Dt_CodiciContatto_for_XML(DT_Codici, enum_TipoOperazioneDB.Cancellazione,
                                                                      pivaDest, codContatto, enum_CodiciAnagrafe.RappresentanteFiscale,
                                                                      visibilita, If(valore Is Nothing, 0, valore.Val_Cod), AGRODATAINIZIO, AGRODATAFINE)
        objXML_Utility.Inserisci_Riga_Dt_CodiciContatto_for_XML(DT_Codici, enum_TipoOperazioneDB.Scrittura,
                                                                pivaDest, codContatto, enum_CodiciAnagrafe.RappresentanteFiscale,
                                                                visibilita, If(valore Is Nothing, 0, valore.Val_Cod), AGRODATAINIZIO, AGRODATAFINE)

        ' Codice accisa
        valore = codici.FirstOrDefault(Function(s) s.ID_Cod = enum_CodiciAnagrafe.CodiceAccisa)
        objXML_Utility.Inserisci_Riga_Dt_CodiciContatto_for_XML(DT_Codici, enum_TipoOperazioneDB.Cancellazione,
                                                                      pivaDest, codContatto, enum_CodiciAnagrafe.CodiceAccisa,
                                                                      visibilita, If(valore Is Nothing, 0, valore.Val_Cod), AGRODATAINIZIO, AGRODATAFINE)
        objXML_Utility.Inserisci_Riga_Dt_CodiciContatto_for_XML(DT_Codici, enum_TipoOperazioneDB.Scrittura,
                                                                pivaDest, codContatto, enum_CodiciAnagrafe.CodiceAccisa,
                                                                visibilita, If(valore Is Nothing, 0, valore.Val_Cod), AGRODATAINIZIO, AGRODATAFINE)

        ' Codice Ufficio Dogane
        valore = codici.FirstOrDefault(Function(s) s.ID_Cod = enum_CodiciAnagrafe.Codice_Ufficio_Doganale)
        objXML_Utility.Inserisci_Riga_Dt_CodiciContatto_for_XML(DT_Codici, enum_TipoOperazioneDB.Cancellazione,
                                                                      pivaDest, codContatto, enum_CodiciAnagrafe.Codice_Ufficio_Doganale,
                                                                      visibilita, If(valore Is Nothing, 0, valore.Val_Cod), AGRODATAINIZIO, AGRODATAFINE)
        objXML_Utility.Inserisci_Riga_Dt_CodiciContatto_for_XML(DT_Codici, enum_TipoOperazioneDB.Scrittura,
                                                                pivaDest, codContatto, enum_CodiciAnagrafe.Codice_Ufficio_Doganale,
                                                                visibilita, If(valore Is Nothing, 0, valore.Val_Cod), AGRODATAINIZIO, AGRODATAFINE)




        If _gestisciDatiContabili Then

            ' Sconto cliente
            valore = codici.FirstOrDefault(Function(s) s.ID_Cod = enum_CodiciAnagrafe.ScontoContattoDefault)
            objXML_Utility.Inserisci_Riga_Dt_CodiciContatto_for_XML(DT_Codici, enum_TipoOperazioneDB.Cancellazione,
                                                                          pivaDest, codContatto, enum_CodiciAnagrafe.ScontoContattoDefault,
                                                                          visibilita,
                                                                          If(valore Is Nothing, 0, valore.Val_Cod),
                                                                          AGRODATAINIZIO, AGRODATAFINE)
            objXML_Utility.Inserisci_Riga_Dt_CodiciContatto_for_XML(DT_Codici, enum_TipoOperazioneDB.Scrittura,
                                                                        pivaDest, codContatto, enum_CodiciAnagrafe.ScontoContattoDefault,
                                                                        visibilita,
                                                                        If(valore Is Nothing, 0, valore.Val_Cod),
                                                                        AGRODATAINIZIO, AGRODATAFINE)

            ' modalita pagamento default
            valore = codici.FirstOrDefault(Function(s) s.ID_Cod = enum_CodiciAnagrafe.ModalitaPagamentoDefault)
            objXML_Utility.Inserisci_Riga_Dt_CodiciContatto_for_XML(DT_Codici, enum_TipoOperazioneDB.Cancellazione,
                                                                              pivaDest, codContatto, enum_CodiciAnagrafe.ModalitaPagamentoDefault,
                                                                              visibilita,
                                                                              If(valore Is Nothing, 0, valore.Val_Cod),
                                                                              AGRODATAINIZIO, AGRODATAFINE)
            objXML_Utility.Inserisci_Riga_Dt_CodiciContatto_for_XML(DT_Codici, enum_TipoOperazioneDB.Scrittura,
                                                                        pivaDest, codContatto, enum_CodiciAnagrafe.ModalitaPagamentoDefault,
                                                                        visibilita,
                                                                        If(valore Is Nothing, 0, valore.Val_Cod),
                                                                        AGRODATAINIZIO, AGRODATAFINE)

            ' Iban Default 
            valore = codici.FirstOrDefault(Function(s) s.ID_Cod = enum_CodiciAnagrafe.IBANDefault)
            objXML_Utility.Inserisci_Riga_Dt_CodiciContatto_for_XML(DT_Codici, enum_TipoOperazioneDB.Cancellazione,
                                                                              pivaDest, codContatto, enum_CodiciAnagrafe.IBANDefault,
                                                                              visibilita,
                                                                              If(valore Is Nothing, 0, valore.Val_Cod),
                                                                              AGRODATAINIZIO, AGRODATAFINE)
            objXML_Utility.Inserisci_Riga_Dt_CodiciContatto_for_XML(DT_Codici, enum_TipoOperazioneDB.Scrittura,
                                                                        pivaDest, codContatto, enum_CodiciAnagrafe.IBANDefault,
                                                                        visibilita,
                                                                        If(valore Is Nothing, 0, valore.Val_Cod),
                                                                        AGRODATAINIZIO, AGRODATAFINE)

            ' ListinoPrezziAcquistoDefault
            valore = codici.FirstOrDefault(Function(s) s.ID_Cod = enum_CodiciAnagrafe.ListinoPrezziAcquistoDefault)
            objXML_Utility.Inserisci_Riga_Dt_CodiciContatto_for_XML(DT_Codici, enum_TipoOperazioneDB.Cancellazione,
                                                                              pivaDest, codContatto, enum_CodiciAnagrafe.ListinoPrezziAcquistoDefault,
                                                                              visibilita,
                                                                              If(valore Is Nothing, 0, valore.Val_Cod),
                                                                              AGRODATAINIZIO, AGRODATAFINE)
            objXML_Utility.Inserisci_Riga_Dt_CodiciContatto_for_XML(DT_Codici, enum_TipoOperazioneDB.Scrittura,
                                                                        pivaDest, codContatto, enum_CodiciAnagrafe.ListinoPrezziAcquistoDefault,
                                                                        visibilita,
                                                                        If(valore Is Nothing, 0, valore.Val_Cod),
                                                                        AGRODATAINIZIO, AGRODATAFINE)

            ' ListinoPrezziVenditaDefault
            valore = codici.FirstOrDefault(Function(s) s.ID_Cod = enum_CodiciAnagrafe.ListinoPrezziVenditaDefault)
            objXML_Utility.Inserisci_Riga_Dt_CodiciContatto_for_XML(DT_Codici, enum_TipoOperazioneDB.Cancellazione,
                                                                              pivaDest, codContatto, enum_CodiciAnagrafe.ListinoPrezziVenditaDefault,
                                                                              visibilita,
                                                                              If(valore Is Nothing, 0, valore.Val_Cod),
                                                                              AGRODATAINIZIO, AGRODATAFINE)
            objXML_Utility.Inserisci_Riga_Dt_CodiciContatto_for_XML(DT_Codici, enum_TipoOperazioneDB.Scrittura,
                                                                        pivaDest, codContatto, enum_CodiciAnagrafe.ListinoPrezziVenditaDefault,
                                                                        visibilita,
                                                                        If(valore Is Nothing, 0, valore.Val_Cod),
                                                                        AGRODATAINIZIO, AGRODATAFINE)

            'Referente Conferimento
            valore = codici.FirstOrDefault(Function(s) s.ID_Cod = enum_CodiciAnagrafe.ReferenteConferimento)
            objXML_Utility.Inserisci_Riga_Dt_CodiciContatto_for_XML(DT_Codici, enum_TipoOperazioneDB.Cancellazione,
                                                                              pivaDest, codContatto, enum_CodiciAnagrafe.ReferenteConferimento,
                                                                              visibilita,
                                                                              If(valore Is Nothing, 0, valore.Val_Cod),
                                                                              AGRODATAINIZIO, AGRODATAFINE)
            objXML_Utility.Inserisci_Riga_Dt_CodiciContatto_for_XML(DT_Codici, enum_TipoOperazioneDB.Scrittura,
                                                                        pivaDest, codContatto, enum_CodiciAnagrafe.ReferenteConferimento,
                                                                        visibilita,
                                                                        If(valore Is Nothing, 0, valore.Val_Cod),
                                                                        AGRODATAINIZIO, AGRODATAFINE)

            'Gestione Vettore
            valore = codici.FirstOrDefault(Function(s) s.ID_Cod = enum_CodiciAnagrafe.Gestione_Vettore_Default)
            objXML_Utility.Inserisci_Riga_Dt_CodiciContatto_for_XML(DT_Codici, enum_TipoOperazioneDB.Cancellazione,
                                                                              pivaDest, codContatto, enum_CodiciAnagrafe.Gestione_Vettore_Default,
                                                                              visibilita,
                                                                              If(valore Is Nothing, 0, valore.Val_Cod),
                                                                              AGRODATAINIZIO, AGRODATAFINE)
            objXML_Utility.Inserisci_Riga_Dt_CodiciContatto_for_XML(DT_Codici, enum_TipoOperazioneDB.Scrittura,
                                                                    pivaDest, codContatto, enum_CodiciAnagrafe.Gestione_Vettore_Default,
                                                                    visibilita,
                                                                    If(valore Is Nothing, 0, valore.Val_Cod),
                                                                    AGRODATAINIZIO, AGRODATAFINE)

        End If


        If Trim(codContatto.ToUpper()) = pivaDest Then

            ' COdice UA
            valore = codici.FirstOrDefault(Function(s) s.ID_Cod = enum_CodiciAnagrafe.Codice_Accise_UA)
            objXML_Utility.Inserisci_Riga_Dt_CodiciContatto_for_XML(DT_Codici, enum_TipoOperazioneDB.Cancellazione,
                                                                      pivaDest, codContatto, enum_CodiciAnagrafe.Codice_Accise_UA,
                                                                      visibilita, If(valore Is Nothing, 0, valore.Val_Cod), AGRODATAINIZIO, AGRODATAFINE)
            objXML_Utility.Inserisci_Riga_Dt_CodiciContatto_for_XML(DT_Codici, enum_TipoOperazioneDB.Scrittura,
                                                                pivaDest, codContatto, enum_CodiciAnagrafe.Codice_Accise_UA,
                                                                visibilita, If(valore Is Nothing, 0, valore.Val_Cod), AGRODATAINIZIO, AGRODATAFINE)


            'Codice conto Garanzia
            valore = codici.FirstOrDefault(Function(s) s.ID_Cod = enum_CodiciAnagrafe.Codice_Accise_Conto_Garanzia)
            objXML_Utility.Inserisci_Riga_Dt_CodiciContatto_for_XML(DT_Codici, enum_TipoOperazioneDB.Cancellazione,
                                                                      pivaDest, codContatto, enum_CodiciAnagrafe.Codice_Accise_Conto_Garanzia,
                                                                      visibilita, If(valore Is Nothing, 0, valore.Val_Cod), AGRODATAINIZIO, AGRODATAFINE)
            objXML_Utility.Inserisci_Riga_Dt_CodiciContatto_for_XML(DT_Codici, enum_TipoOperazioneDB.Scrittura,
                                                                    pivaDest, codContatto, enum_CodiciAnagrafe.Codice_Accise_Conto_Garanzia,
                                                                    visibilita, If(valore Is Nothing, 0, valore.Val_Cod), AGRODATAINIZIO, AGRODATAFINE)

            ' deposito fiscale
            valore = codici.FirstOrDefault(Function(s) s.ID_Cod = enum_CodiciAnagrafe.Codice_Magazzino_Fiscale)
            objXML_Utility.Inserisci_Riga_Dt_CodiciContatto_for_XML(DT_Codici, enum_TipoOperazioneDB.Cancellazione,
                                                                     pivaDest, codContatto, enum_CodiciAnagrafe.Codice_Magazzino_Fiscale,
                                                                     visibilita, If(valore Is Nothing, 0, valore.Val_Cod), AGRODATAINIZIO, AGRODATAFINE)
            objXML_Utility.Inserisci_Riga_Dt_CodiciContatto_for_XML(DT_Codici, enum_TipoOperazioneDB.Scrittura,
                                                                    pivaDest, codContatto, enum_CodiciAnagrafe.Codice_Magazzino_Fiscale,
                                                                    visibilita, If(valore Is Nothing, 0, valore.Val_Cod), AGRODATAINIZIO, AGRODATAFINE)

        End If


    End Sub

    Private Sub PreparaSalvataggioCosti(
        ByVal pivaOrigine As String,
        ByVal pivaDest As String,
        ByVal costi As List(Of CostiModel),
        ByRef DT_Prodotti As DataTable)

        Dim objXML_Utility As New AgronicaCoreXML.XML_Utility

        Dim id As Integer
        Dim cod_risum As Integer
        Dim udm_Cod As Integer
        Dim prezzo As Decimal
        Dim inizioPrezzo As DateTime = AGRODATAINIZIO
        Dim finePrezzo As DateTime = AGRODATAFINE

        'TODO ??????? CodRisUM

        For Each c As CostiModel In costi
            'Dim tipoOperazione As Integer = If(id > 0, enum_TipoOperazioneDB.Modifica, enum_TipoOperazioneDB.Scrittura)
            Dim tipoOperazione As Integer = enum_TipoOperazioneDB.Scrittura
            id = c.Id
            cod_risum = c.Cod_RisUm
            udm_Cod = c.Udm_Cod
            prezzo = c.Prezzo
            inizioPrezzo = c.InizioPrezzo
            finePrezzo = c.FinePrezzo

            objXML_Utility.Inserisci_Riga_Dt_ProdottiCosti_for_XML(DT_Prodotti, tipoOperazione, id, pivaDest, "", 0, 0, cod_risum, 0, udm_Cod, prezzo, 0, 0, inizioPrezzo, finePrezzo)
        Next

    End Sub

    Private Shared Sub PreparaSalvataggioProdottiCosti(ByVal pivaOrigine As String,
                                                       ByVal pivaDest As String,
                                                       ByRef DT_RisUm As DataTable, ByVal Dt_Prodotti As DataTable)

        Dim objXML_Utility As New AgronicaCoreXML.XML_Utility
        Dim risorseUmane = (From row In Dt_Prodotti.AsEnumerable()
                            Select row.Field(Of Integer)("mat_cod") Distinct).ToList()

        For Each ru As Integer In risorseUmane
            Dim dtProdXRu = objXML_Utility.CaricaGriglia_ProdottiCosti_for_XML()

            Dim costi As DataRow() = Dt_Prodotti.Select("mat_cod = " & ru)
            For Each row As DataRow In costi.AsEnumerable()
                row.Item("mat_cod") = 0
                dtProdXRu.ImportRow(row)
            Next

            Dim risorsaUmana As DataRow = DT_RisUm.Select("Cod_Risum = " & ru).FirstOrDefault()

            If risorsaUmana IsNot Nothing Then
                risorsaUmana.Item("cod_risum") = "0"
                risorsaUmana("DT_ProdottiCosti") = dtProdXRu
            End If

        Next

        For Each ru As DataRow In DT_RisUm.Rows
            ru.Item("cod_risum") = "0"
        Next

    End Sub
    Private Shared Sub PreparaSalvataggioIndirizzi(ByVal pivaOrigine As String,
                                                   ByVal pivaDest As String,
                                                   ByVal codContatto As String,
                                                   ByVal indirizzi As List(Of IndirizzoModel),
                                                   ByRef Dt_Indirizzi As DataTable)

        Dim objXML_Utility As New AgronicaCoreXML.XML_Utility
        Dim tipoOperazioneDB_INDIRIZZI As enum_TipoOperazioneDB = enum_TipoOperazioneDB.Scrittura

        For Each i As IndirizzoModel In indirizzi

            Dim Tipo_Indirizzo As String = i.Tipo_Indirizzo
            Dim Cod_Indirizzo As String = i.Cod_Indirizzo
            Dim Via As String = i.Via
            Dim Frazione As String = i.Frazione
            Dim Istat_Prov As String = i.Istat_Prov
            Dim Istat_Com As String = i.Istat_Com
            Dim Cap As String = i.Cap
            Dim Stato As String = i.Stato
            Dim Note As String = i.Note
            Dim SiglaProvincia As String = i.Provincia_Cod
            Dim CodiceLingua As String = i.CodiceLingua

            objXML_Utility.Inserisci_Riga_Dt_Indirizzi_for_XML(Dt_Indirizzi, tipoOperazioneDB_INDIRIZZI, pivaDest,
                                                  codContatto,
                                                  Tipo_Indirizzo,
                                                   Istat_Prov,
                                                   Istat_Com,
                                                   Cod_Indirizzo,
                                                   Via,
                                                   Frazione,
                                                   Cap,
                                                   Stato,
                                                   Note,,,
                                                   SiglaProvincia,
                                                   CodiceLingua)

        Next

    End Sub

    Private Sub PreparaSalvataggioRubrica(
        ByVal rubrica As List(Of RubricaModel),
        ByRef Dt_Rubrica As DataTable)

        Dim objXML_Utility As New AgronicaCoreXML.XML_Utility
        Dim cod_rubrica As Long
        Dim descrizione As String = ""
        Dim numero As String = ""

        For Each r As RubricaModel In rubrica
            cod_rubrica = If(r.Cod_Rubrica Is Nothing, 0, r.Cod_Rubrica)
            descrizione = If(String.IsNullOrEmpty(r.Descrizione), "", r.Descrizione)
            numero = If(String.IsNullOrEmpty(r.Numero), "", r.Numero)
            objXML_Utility.Inserisci_Riga_Dt_Rubrica_for_XML(Dt_Rubrica, enum_TipoOperazioneDB.Scrittura, cod_rubrica,
                                                                   numero, descrizione)
        Next

    End Sub

End Class



Public Class Copia_Contatto_Cloner
    Inherits AgronicaCoreDataProvider.LogProvider

    Private _objParametri_Server As AgronicaCoreParametri
    Private _objParametri_Utenti As AgronicaCoreParametri

    Public Sub New(ByRef objParametri_Server As AgronicaCoreParametri,
                   ByRef objParametri_Utenti As AgronicaCoreParametri)

        _objParametri_Server = objParametri_Server
        _objParametri_Utenti = objParametri_Utenti

    End Sub

    Public Function Clona(ByVal contatto As ContattoModel) As ContattoModel

        Dim contattoClone As ContattoModel = contatto.ShallowCopy()

        contattoClone.Sa_Cod = 0
        For Each i As IndirizzoModel In contattoClone.Indirizzi
            i.Cod_Indirizzo = 0
        Next
        For Each r As RubricaModel In contattoClone.Rubrica
            r.Cod_Rubrica = 0
        Next

        For Each c As CostiModel In contattoClone.Costi
            c.Id = 0
        Next

        For Each l As LiquiditaModel In contattoClone.Liquidita
            l.Cod_Liquidita = 0
        Next

        Return contattoClone

    End Function


End Class

#Region "Models Classes"

Public Class ContattoModel

    Public TipoUtente As Integer
    Public ID_CF As Integer
    Public Piva As String = String.Empty
    Public Rag_Soc As String = String.Empty
    Public CF As String = String.Empty
    Public Nome As String = String.Empty
    Public Cognome As String = String.Empty
    Public DataNascita As DateTime
    Public Sesso As String = String.Empty
    Public CF_Estero As String = String.Empty
    Public Convenevoli As String = String.Empty
    Public Badge As String = String.Empty
    Public Fittizio As Boolean = False
    Public Sa_Cod As Integer

    Public Indirizzi As List(Of IndirizzoModel)
    Public Rubrica As List(Of RubricaModel)
    Public Costi As List(Of CostiModel)
    Public RapportiContabili As List(Of RapportoContabileModel)
    Public ContattiCodici As List(Of ContattoCodiceModel)
    Public AltriDati As AltriDatiModel
    Public Liquidita As List(Of LiquiditaModel)
    Public Conti As List(Of ContiModel)
    Public DettagliContabili As DettagliContabiliModel
    Public ContattiCollegati As List(Of ContattoCollegatoModel)

    Public Function ShallowCopy() As ContattoModel
        Return DirectCast(Me.MemberwiseClone(), ContattoModel)
    End Function

End Class

Public Class ContattoCodiceModel
    Public ID_Cod As Integer?
    Public Val_Cod As String
End Class

Public Class CostiModel

    Public Id As Integer?
    Public Cod_RisUm As Integer?
    Public Cod_RisUm_Des As String
    Public Udm_Des As String
    Public Prezzo As Decimal?
    Public InizioPrezzo As DateTime?
    Public FinePrezzo As DateTime?
    Public Udm_Cod As Integer?
    Public Id_Budget As Integer?

End Class

Public Class RubricaModel

    Public Key_Rubrica As String
    Public Cod_Rubrica As Long?
    Public TipoRubrica_Des As String
    Public Numero As String
    Public Descrizione As String

End Class

Public Class IndirizzoModel

    Public Tipo_Indirizzo As Integer?
    Public Cod_Indirizzo As Integer?
    Public Comune_Cod As String = String.Empty
    Public Provincia_Cod As String = String.Empty
    Public Via As String = String.Empty
    Public Frazione As String = String.Empty
    Public Istat_Prov As String = String.Empty
    Public Istat_Com As String = String.Empty
    Public Cap As String = String.Empty
    Public Stato As String = String.Empty
    Public Note As String = String.Empty
    Public SiglaProvincia As String = String.Empty
    Public CodiceLingua As String = String.Empty
    Public piva As String = String.Empty
    Public Sa_cod As Integer? = 0
    Public Com_des As String = String.Empty

End Class

Public Class RapportoContabileModel

    Public Cod_RisUm As Integer?
    Public piva As String
    Public sa_cod As Integer?
    Public Cod_Contatto As String
    Public Cod_Rapporto As Integer?
    Public Settore_Des As String
    Public Rapporto_Des As String
    Public Qualifica_Cod As Integer?
    Public Qualifica_Des As String
    Public Mansione_Cod As Integer?
    Public Mansione_Des As String
    Public Validita_Inizio As DateTime?
    Public Validita_Fine As DateTime?
    Public Attivita_Des As String
    Public occasionale As Integer?
    Public Ore_Settimanali As Decimal?
    Public Info_Famiglia As String
    Public Classificazione_Cod As Integer?
    Public Classificazione_Des As String
    Public TipoRapporto_Cod As Integer?
    Public TipoRapporto_Des As String
    Public key_rap_cont As String

End Class

Public Class AltriDatiModel
    Public Note As String = String.Empty
    Public Note2 As String = String.Empty
    Public Fittizio As Boolean = False
    Public NoteOperazioni As String = String.Empty
    Public NoteOperazioni2 As String = String.Empty
    Public Tipo_destinazione As String = String.Empty
    Public OrigineDestinazione As String = String.Empty
    Public DettCont_tipo_ind_default As Integer? = Nothing
    Public Memo As String = String.Empty
End Class

Public Class LiquiditaModel

    Public Sa_Cod As Integer?
    Public Cod_Liquidita As Integer?
    Public Cod_Istituto As Integer?
    Public Istituto_Des As String = String.Empty
    Public Nazione As String = String.Empty
    Public Cifre_Controllo As String = String.Empty
    Public Cin As String = String.Empty
    Public Abi As String = String.Empty
    Public Cab As String = String.Empty
    Public Numero As String = String.Empty
    Public Bic As String = String.Empty
    Public ChkAbilitazione As Integer?
    Public Abilitazione_Des As String = String.Empty
    Public Validita_Inizio As DateTime?
    Public Validita_Fine As DateTime?
    Public Note As String = String.Empty
    Public Selected As Boolean
    Public ChkDefault? As Boolean

End Class

Public Class ContiModel

    Public key_conto As String = String.Empty
    Public Cod_Conto As Integer?
    Public Conto_Descr As String = String.Empty
    Public ID_Riclassificazione As String = String.Empty
    Public Validita_Inizio As DateTime?
    Public Validita_Fine As DateTime?

End Class

Public Class ContattoCollegatoModel

    Public Cod_Risum As Integer
    Public Cod_Contatto As String
    Public Piva As String
    Public Sa_Cod As Integer

End Class

Public Class DettagliContabiliModel

    Public dettCont_sconto_add1 As Integer? = Nothing
    Public dettCont_sconto_add2 As Integer? = Nothing
    Public dettCont_sconto_add3 As Integer? = Nothing
    Public dettCont_sconto_cliente As Integer? = Nothing
    Public dettCont_provvigione_agente As Integer? = Nothing
    Public dettCont_provvigione_capoarea As Integer? = Nothing
    Public dettCont_agente_cod As Integer? = Nothing
    Public dettCont_capoarea_cod As Integer? = Nothing
    Public dettCont_vettore_cod As Integer? = Nothing
    Public dettCont_iva_default As Integer? = Nothing
    Public dettCont_cod_conto_economico_default As Integer? = Nothing
    Public dettCont_cod_conto_patrimoniale_default As Integer? = Nothing
    Public dettCont_fatturazione_automatica As Integer? = Nothing
    Public dettCont_documento_fatturazione As Integer? = Nothing
    Public dettCont_destinazione_diversa As Integer? = Nothing
    Public dettCont_tipo_indirizzo_default_destinazione_diversa As Integer? = Nothing

End Class

Public Class ParametroCopiaContatto
    Public CodContatto As String
    Public DesContatto As String
    Public PivaOrigine As String
    Public RagSocOrigine As String
    Public PivaDestinazione As List(Of KeyValuePair(Of String, String))
    Public Risultati As List(Of RisultatoCopia)
    Public Sub New(ByVal codContatto As String,
                   ByVal desContatto As String,
                   ByVal pivaOrigine As String,
                   ByVal ragSocOrigine As String,
                   ByVal pivaDest As List(Of KeyValuePair(Of String, String)))

        Me.CodContatto = codContatto
        Me.DesContatto = desContatto
        Me.PivaOrigine = pivaOrigine
        Me.RagSocOrigine = ragSocOrigine
        Me.PivaDestinazione = pivaDest
        Me.Risultati = New List(Of RisultatoCopia)
    End Sub

    Public Overrides Function ToString() As String

        If Risultati.Count = 0 Then
            Return String.Format("Log non disponibile per il contatto {0} ", CodContatto)
        Else

            Dim stb As New StringBuilder
            stb.AppendLine("-".PadLeft(50, "-"))
            stb.AppendLine(String.Format(" Contatto '{0}' - {1} :", CodContatto, DesContatto))
            stb.AppendLine("-".PadLeft(50, "-"))

            Dim piveDaPrivatizzare = PivaDestinazione.Where(Function(s) s.Key <> PivaOrigine).ToList()
            If piveDaPrivatizzare.Any Then
                stb.Append(Environment.NewLine)
                For Each kvp As KeyValuePair(Of String, String) In piveDaPrivatizzare
                    Dim risultato = Risultati.FirstOrDefault(Function(r) r.Piva.Equals(kvp.Key))
                    stb.AppendLine(RisultatoToString(risultato))
                    stb.Append(Environment.NewLine)
                Next
            End If

            Dim risultatoPrivatizzazione = Risultati.FirstOrDefault(Function(r) r.Piva.Equals(PivaOrigine))
            stb.AppendLine(RisultatoToString(risultatoPrivatizzazione))
            stb.Append(Environment.NewLine)

            Return stb.ToString()
        End If

    End Function

    Private Function RisultatoToString(ByVal risultato As RisultatoCopia) As String

        If risultato IsNot Nothing Then
            Dim tipoPiva As String = If(risultato.Piva = PivaOrigine, "Origine", "Destinazione")
            Dim stb As New StringBuilder
            stb.AppendLine(String.Format("  -> Piva {0}: {1} - {2}", tipoPiva, risultato.Piva, risultato.RagSoc))
            stb.AppendLine("  Operazioni: ")
            For Each op As String In risultato.OperazioniEffettuate
                stb.AppendLine(String.Format("     {0}", op))
            Next
            stb.AppendLine("  Errori: ")
            If risultato.Errori IsNot Nothing Then
                For Each er As String In risultato.Errori
                    stb.AppendLine(String.Format("     {0}", er))
                Next
            End If
            stb.AppendLine(String.Format("  Stato: {0}", risultato.Stato.ToString))
            Return stb.ToString
        Else
            Return String.Empty
        End If

    End Function

End Class

Public Class RisultatoCopia
    Public Piva As String
    Public RagSoc As String
    Public Stato As Enum_Stato_Copia_Contatto
    Public Errori As List(Of String)
    Public OperazioniEffettuate As List(Of String) = New List(Of String)
End Class

Public Class RisUmMapping : Inherits ElementoMapping
    Public Cod_RisUM_Origine As Integer
    Public Cod_RisUM_Nuovo As Integer
End Class
Public Class IndirizzoMapping : Inherits ElementoMapping
    Public Cod_Indirizzo_Origine As Integer
    Public Cod_Indirizzo_Nuovo As Integer
End Class

Public Class ElementoMapping
    Public PivaOri As String
    Public PivaDest As String
End Class

Public Enum Enum_Stato_Copia_Contatto
    Skipped = 1
    OK = 2
    KO = 3
End Enum

#End Region
