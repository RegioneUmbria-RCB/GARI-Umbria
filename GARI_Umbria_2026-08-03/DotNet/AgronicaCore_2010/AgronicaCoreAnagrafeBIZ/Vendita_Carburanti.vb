Imports System.Linq
Imports System.Transactions
Imports AgronicaCoreAnagrafeDAL
Imports AgronicaCoreAnagrafeDAL.UMA_Configurazione_MacrousixLavorazioni_R
Imports AgronicaCoreDataProvider
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreVarieBIZ
Imports Newtonsoft.Json

Public Class Vendita_Carburanti
    Inherits LogProvider
    Private _coreDBContext As AgronicaCoreParametri
    Private _coreDBContext_user As AgronicaCoreParametri
    Private _efContext As Gias_DeveloperServer_Entities
    Private _carburantiDAL As Vendita_Carburanti_DAL

    Public Sub New(coreParametri_server As AgronicaCoreParametri, coreParametri_user As AgronicaCoreParametri)
        _coreDBContext = coreParametri_server
        _coreDBContext_user = coreParametri_user
        Dim gefutils As New Gias_EF_Utility
        Dim efConnString As String = gefutils.GetEntityConnectionString(_coreDBContext.StringaConnessione)
        _efContext = New Gias_DeveloperServer_Entities(efConnString)
        _carburantiDAL = New Vendita_Carburanti_DAL(_efContext, _coreDBContext, _coreDBContext_user)
    End Sub


    Public Function CaricaCarburantiVendita(filtri As FiltriCaricaRigheVenditaCarburanti) As DataTable
        Try
            Dim carburanti = TentativaDiCaricamentoCarburantiVendita(filtri)
            Return carburanti
        Catch ex As Exception
            Dim nomeRoutine As String = ManageException(ex)
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
        End Try
    End Function

    Private Function TentativaDiCaricamentoCarburantiVendita(filtri As FiltriCaricaRigheVenditaCarburanti) As DataTable
        Dim PiveVisibili As String = ""

        Dim FiltroUtente As Boolean = False
        Dim FiltroGruppo As Boolean = False
        Dim Gruppo As Integer = 0
        Dim VisibilitaTotale As Boolean = False

        If (filtri.NuovaVisibilita) Then
            'Dim objImprese_Dal As New AgronicaCoreAnagrafeDAL.Imprese_Read
            Dim objUtenti_Visibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
            'Dim objGruppi_Utente As New AgronicaCoreUtentiDAL.Utenti_xGruppi_Utente_R

            'Dim userName As String = _coreDBContext.UtenteUsername

            'Dim dtt As DataTable = objUtenti_Visibilita.OttieniPIVEVisibilita(userName, _coreDBContext, _coreDBContext_user)

            'If (dtt.Rows.Count > 0 AndAlso _coreDBContext.SuperUserUsername <> _coreDBContext.UtenteUsername) Then
            '    PiveVisibili = dtt.AsEnumerable().
            '    [Select](Function(x) x("Piva_Azienda").ToString()).Aggregate(Function(a, b) String.Concat(a & "'" & "," & "'" & b))
            'Else
            '    PiveVisibili = "-1"
            'End If

            objUtenti_Visibilita.OttieniTipoFiltroVisibilita(_coreDBContext_user, 0, FiltroUtente, FiltroGruppo, Gruppo, VisibilitaTotale)

        End If

        Dim dt As DataTable = _carburantiDAL.CaricaCarburantiVendita(filtri, FiltroUtente, FiltroGruppo, Gruppo, VisibilitaTotale)

        Return dt

    End Function


    Public Function VerificaValiditaCuaa(Cuaa As String) As RispostaStandard
        Try
            Dim risposta = TentativoVerificaValiditaCuaa(Cuaa)
            Return risposta
        Catch ex As Exception
            Dim nomeRoutine As String = ManageException(ex)
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
        End Try
    End Function
    Private Function TentativoVerificaValiditaCuaa(cuaa As String) As RispostaStandard
        Dim cuaaValido As Boolean = _carburantiDAL.VerificaValiditaCuaa(cuaa)
        Dim risposta As New RispostaStandard()
        risposta.RispostaOK = cuaaValido
        risposta.RispostaStringa = If(cuaaValido, "Il CUAA inserito è valido", "Il CUAA inserito è invalido")
        Return risposta
    End Function


    Public Function VerificarePraticaDiRichiestaCarburanteSiaAperta(filtri As FiltriPraticaRichiestaCarburante) As RispostaStandard
        Try
            Dim risposta As RispostaStandard = TentativoVerificarePraticaDiRichiestaCarburanteSiaAperta(filtri)
            Return risposta
        Catch ex As Exception
            Dim nomeRoutine As String = ManageException(ex)
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
        End Try
    End Function
    Private Function TentativoVerificarePraticaDiRichiestaCarburanteSiaAperta(filtri As FiltriPraticaRichiestaCarburante) As RispostaStandard
        Dim praticaAperta As String = _carburantiDAL.VerificarePraticaDiRichiestaCarburanteSiaAperta(filtri)

        Dim risposta As New RispostaStandard()
        risposta.RispostaOK = praticaAperta
        risposta.RispostaStringa = If(praticaAperta,
            "E' stata trovata almeno una richiesta carburante aperta",
            "Nessuna richiesta presente per questo CUAA, quindi il PIVA Cliente non esiste ed il campo Lt Assegnati non viene aggiornato.")
        Return risposta
    End Function

    Public Function OttienePivaClienteERagioneSociale(filtri As OttienePivaCliente) As RispostaStandard
        Try
            Dim risposta As RispostaStandard = TentareAOttenerePivaClienteERagioneSociale(filtri)
            Return risposta
        Catch ex As Exception
            Dim nomeRoutine As String = ManageException(ex)
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
        End Try
    End Function
    Public Function TentareAOttenerePivaClienteERagioneSociale(filtri As OttienePivaCliente) As RispostaStandard
        Dim cuaaValido As Boolean = _carburantiDAL.VerificaValiditaCuaa(filtri.Cuaa)

        Dim risposta As New RispostaStandard()
        If Not cuaaValido Then
            risposta.RispostaOK = False
            risposta.RispostaStringa = "Il CUAA inserito è invalido"
            Return risposta
        End If

        Dim praticaAperta As String = _carburantiDAL.VerificarePraticaDiRichiestaCarburanteSiaAperta(New FiltriPraticaRichiestaCarburante With {.Anno = filtri.Anno, .Cuaa = filtri.Cuaa})

        If Not praticaAperta = "True" Then
            risposta.RispostaOK = False
            risposta.RispostaStringa = "Nessuna richiesta aperta presente per questo CUAA"
            Return risposta
        End If

        Dim pivaCliente = _carburantiDAL.OttienePIVADallePratiche(filtri.Anno, filtri.Cuaa)

        If pivaCliente Is Nothing OrElse pivaCliente.Length = 0 Then
            risposta.RispostaOK = False
            risposta.RispostaStringa = "Nessuna PIVA Cliente è stata trovata per questo anno e CUAA"
            Return risposta
        End If

        Dim ragioneSociale As String = _carburantiDAL.CaricareRagioneSociale(filtri.Cuaa)

        Dim result As New PivaClienteERagioneSocialeResult With {.Piva_Cliente = pivaCliente, .Ragione_Sociale = ragioneSociale}
        result.MessaggioSuccess = "La Ragione Sociale è stata aggiornata correttamente"

        risposta.RispostaOK = True
        risposta.RispostaStringa = JsonConvert.SerializeObject(result)
        Return risposta
    End Function

    Public Function CaricaRagioneSociale(cuaa As String) As RispostaStandard
        Try
            Dim risposta = TentativoCaricareRagioneSociale(cuaa)
            Return risposta
        Catch ex As Exception
            Dim nomeRoutine As String = ManageException(ex)
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
        End Try
    End Function
    Private Function TentativoCaricareRagioneSociale(cuaa As String) As RispostaStandard
        Dim ragioneSociale As String = _carburantiDAL.CaricareRagioneSociale(cuaa)

        Dim risposta As New RispostaStandard()
        risposta.RispostaOK = True
        risposta.RispostaStringa = ragioneSociale
        Return risposta
    End Function
    Public Function AggiornaLtAssegnati(parametri As FiltriAggiornaLtAssegnati) As RispostaStandard
        Try
            Dim risposta = TentativoAggiornaLtAssegnati(parametri)
            Return risposta
        Catch ex As Exception
            Dim nomeRoutine As String = ManageException(ex)
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
        End Try
    End Function
    Private Function TentativoAggiornaLtAssegnati(parametri As FiltriAggiornaLtAssegnati) As RispostaStandard
        Dim ltAssegnati As Integer = _carburantiDAL.OttieneLtAssegnati(parametri)

        Dim risposta As New RispostaStandard()
        risposta.RispostaOK = True
        risposta.RispostaStringa = ltAssegnati
        Return risposta
    End Function
    Public Function AggiornaTotaleLtAcquistati(parametri As FiltriAggiornaTotaleLtAcquistati) As RispostaStandard
        Try
            Dim risposta = TentativoAggiornareTotaleLtAcquistati(parametri)
            Return risposta
        Catch ex As Exception
            Dim nomeRoutine As String = ManageException(ex)
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
        End Try
    End Function
    Public Function DettaglioLtAcquistabili(parametri As FiltriDettaglioLtAcquistabili) As RispostaStandard
        Try
            Dim risposta = TentativoDettaglioLtAcquistabili(parametri)
            Return risposta
        Catch ex As Exception
            Dim nomeRoutine As String = ManageException(ex)
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
        End Try
    End Function
    Private Function TentativoDettaglioLtAcquistabili(params As FiltriDettaglioLtAcquistabili) As RispostaStandard
        Dim totaleAcuiqstatiProprio = _carburantiDAL.OttieneTotaleLtAcquistati(New FiltriAggiornaTotaleLtAcquistati(params.Anno, params.Cuaa, params.Tipo_Carburante, ContoProprioTerzi.ContoProprio))
        Dim ltAssegnatiProprio = _carburantiDAL.OttieneLtAssegnati(New FiltriAggiornaLtAssegnati(params.Anno, params.PivaCliente, params.Tipo_Carburante, ContoProprioTerzi.ContoProprio))
        Dim ltAcquistabiliProprio = ltAssegnatiProprio - totaleAcuiqstatiProprio


        Dim totaleAcuiqstatiTerzi = _carburantiDAL.OttieneTotaleLtAcquistati(New FiltriAggiornaTotaleLtAcquistati(params.Anno, params.Cuaa, params.Tipo_Carburante, ContoProprioTerzi.ContoTerzi))
        Dim ltAssegnatiTerzi = _carburantiDAL.OttieneLtAssegnati(New FiltriAggiornaLtAssegnati(params.Anno, params.PivaCliente, params.Tipo_Carburante, ContoProprioTerzi.ContoTerzi))
        Dim ltAcquistabiliTerzi = ltAssegnatiTerzi - totaleAcuiqstatiTerzi


        Dim risposta As New RispostaStandard()
        risposta.RispostaOK = True
        risposta.RispostaStringa = JsonConvert.SerializeObject(New With {Key ltAcquistabiliProprio, Key ltAcquistabiliTerzi})
        Return risposta
    End Function

    Private Function TentativoAggiornareTotaleLtAcquistati(parametri As FiltriAggiornaTotaleLtAcquistati) As RispostaStandard
        Dim ragioneSociale As Integer = _carburantiDAL.OttieneTotaleLtAcquistati(parametri)

        Dim risposta As New RispostaStandard()
        risposta.RispostaOK = True
        risposta.RispostaStringa = ragioneSociale
        Return risposta
    End Function

    Public Function AggiornaGrigliaVenditaCarburanti(RigheDaAggiornare As RigheVenditaCarburantiDaAggiornare) As RispostaStandard
        Dim risposta As New RispostaStandard()
        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False

        Try
            ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale, FlagTransazioneLocale, _coreDBContext)
            risposta = TentativoAggiornareVenditaCarburanti(RigheDaAggiornare)
            ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, _coreDBContext)
        Catch ex As Exception
            If Not _coreDBContext.objTransazione Is Nothing Then
                ConnessioniTransazioni.ChiudiTransazione(2, _coreDBContext)
            End If
            Throw ex
        Finally
            ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, _coreDBContext)
        End Try
        Return risposta
    End Function

    Private Function TentativoAggiornareVenditaCarburanti(RigheDaAggiornare As RigheVenditaCarburantiDaAggiornare) As RispostaStandard
        Dim deserializerSettings As JsonSerializerSettings = New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}
        Dim righeInserite As List(Of VenditaCarburantiDto) = JsonConvert.DeserializeObject(Of List(Of VenditaCarburantiDto))(RigheDaAggiornare.RigheInserite, deserializerSettings)
        Dim righeModificate As List(Of VenditaCarburantiDto) = JsonConvert.DeserializeObject(Of List(Of VenditaCarburantiDto))(RigheDaAggiornare.RigheModificate, deserializerSettings)
        Dim righeCancellate As List(Of VenditaCarburantiDto) = JsonConvert.DeserializeObject(Of List(Of VenditaCarburantiDto))(RigheDaAggiornare.RigheCancellate, deserializerSettings)

        ImpostareLtCampiObbl(righeInserite.Concat(righeModificate).ToList())


        ControllaValiditaRigheInserite(righeInserite)
        ControllaValiditaRighe(righeModificate)

        VerificaRigheAggiornateComplessivamente(righeInserite, righeModificate)

        If Not righeInserite Is Nothing AndAlso righeInserite.Count > 0 Then
            _carburantiDAL.AggiungiNouvi(righeInserite)
        End If

        If Not righeModificate Is Nothing AndAlso righeModificate.Count > 0 Then
            _carburantiDAL.Aggiorna(righeModificate)
        End If

        If Not righeCancellate Is Nothing AndAlso righeCancellate.Count > 0 Then
            For Each riga As VenditaCarburantiDto In righeCancellate
                _carburantiDAL.Rimuovi(riga)
            Next
        End If

        Dim risposta As New RispostaStandard()
        risposta.RispostaOK = True
        risposta.RispostaStringa = "L'operazione è stata completata con successo."
        Return risposta
    End Function

    Private Sub ControllaValiditaLtAssegnatiInPrecedenza(carburanti As List(Of VenditaCarburantiDto))


        'Dim carburantiGruppati = carburanti.GroupBy(Function(s) New With {Key s.Anno_Cod, Key s.Tipo_Carburante_Cod, Key s.PIVA_Cliente})
        'For Each gruppo In carburantiGruppati

        '    Dim valorePrecedenteValido = carburanti.FirstOrDefault(Function(s) TestValorePrecedenteValido(s, lt_assegnati))

        '    Dim sommaGruppo = gruppo.Sum(Function(s) s.Lt)

        '    If totalLtTuttiElem <= e.model.Lt_Assegnati Then
        '        e.model.LtInPrecedenza = e.model.Lt;
        '    End If

        'Next


        'Dim elemStessaCUAA = e.sender.dataSource.data().filter(elem >= elem.Cuaa === e.model.Cuaa)
        'Dim totalLtTuttiElem = elemStessaCUAA.reduce((a, b) >= a + b.Lt, 0)


    End Sub

    Private Sub ImpostareLtCampiObbl(righe As List(Of VenditaCarburantiDto))
        Dim gruppi = righe.GroupBy(Function(s) New With {Key s.Anno_Cod, Key s.Tipo_Carburante_Cod, Key s.Conto_Proprio_Terzi_Cod, Key s.Cuaa, Key s.PIVA_Cliente})
        For Each gruppo In gruppi
            Dim filtriLtAcquistati = New FiltriAggiornaTotaleLtAcquistati(gruppo.Key.Anno_Cod, gruppo.Key.Cuaa, gruppo.Key.Tipo_Carburante_Cod, gruppo.Key.Conto_Proprio_Terzi_Cod)
            Dim ltAcquistatiGruppo = _carburantiDAL.OttieneTotaleLtAcquistati(filtriLtAcquistati)
            For Each s In gruppo
                s.Totale_Lt_Acquistati = ltAcquistatiGruppo
            Next

            Dim filtriLtAsseg = New FiltriAggiornaLtAssegnati(gruppo.Key.Anno_Cod, gruppo.Key.PIVA_Cliente, gruppo.Key.Tipo_Carburante_Cod, gruppo.Key.Conto_Proprio_Terzi_Cod)
            Dim ltAssegnati = _carburantiDAL.OttieneLtAssegnati(filtriLtAsseg)
            For Each s In gruppo
                s.Lt_Assegnati = ltAssegnati
                s.Lt_Acquistabili = s.Lt_Assegnati - s.Totale_Lt_Acquistati
            Next
        Next
    End Sub

    Private Sub VerificaRigheAggiornateComplessivamente(righeInserite As List(Of VenditaCarburantiDto), righeModificate As List(Of VenditaCarburantiDto))
        Dim errori = CarburantiHelper.VerificaValiditaLtRichiestiTuttiCarburanti(righeInserite, righeModificate, _carburantiDAL)
        If Not String.IsNullOrEmpty(errori) Then
            Throw New Exception(errori)
        End If
    End Sub

    Private Sub ControllaValiditaRigheInserite(carburanti As List(Of VenditaCarburantiDto))
        ControllaValiditaRighe(carburanti)
        ControllaRigheNonEsistonoInDatabase(carburanti)
    End Sub

    Private Sub ControllaRigheNonEsistonoInDatabase(righeDaInserire As List(Of VenditaCarburantiDto))
        If righeDaInserire IsNot Nothing AndAlso righeDaInserire.Count > 0 Then
            For Each riga In righeDaInserire
                Dim exists = _carburantiDAL.VerificaElementoNonEsisteInDB(riga)

                If exists Then
                    Throw New Exception("Il carburante con l'ID Vendita " & riga.Id_Vendite & " esiste già in database")
                End If
            Next
        End If
    End Sub

    Private Sub ControllaValiditaRighe(carburanti As List(Of VenditaCarburantiDto))

        Dim errori As String = ""
        For Each carburante In carburanti
            errori += CarburantiHelper.ControllaPresenzaCampiObbligatori(carburante)
            errori += CarburantiHelper.ControllaDataDocumentoValida(carburante.Data_Documento)
            errori += CarburantiHelper.ControllaValiditaLtRichiesti(carburante, _carburantiDAL)
        Next
        If Not String.IsNullOrEmpty(errori) Then
            Throw New Exception(errori)
        End If

    End Sub


    Public Function CaricaTipoDocumentiDDL() As List(Of Tipo_Documento_DDL)
        Dim tipiDocumento As New List(Of Tipo_Documento_DDL)
        tipiDocumento.Add(New Tipo_Documento_DDL(0, ""))
        tipiDocumento.Add(New Tipo_Documento_DDL(1, "DAS"))
        tipiDocumento.Add(New Tipo_Documento_DDL(2, "Fattura accompagnatoria"))

        Return tipiDocumento
    End Function
    Public Function CaricaTipoCarburantiDDL() As List(Of Tipo_Carburante_DDL)
        Dim tipiCarburante As New List(Of Tipo_Carburante_DDL)
        tipiCarburante.Add(New Tipo_Carburante_DDL(2, "Gasolio"))
        tipiCarburante.Add(New Tipo_Carburante_DDL(3, "Benzina"))
        tipiCarburante.Add(New Tipo_Carburante_DDL(8, "Gasolio Serra"))

        Return tipiCarburante
    End Function

    Public Function CaricaContiAcquistoDDL() As List(Of Conto_Proprio_Terzi_DDL)
        Dim contiDellaVendita As New List(Of Conto_Proprio_Terzi_DDL)
        contiDellaVendita.Add(New Conto_Proprio_Terzi_DDL(-2, ""))
        contiDellaVendita.Add(New Conto_Proprio_Terzi_DDL(0, "Conto Proprio"))
        contiDellaVendita.Add(New Conto_Proprio_Terzi_DDL(-1, "Conto Terzi"))

        Return contiDellaVendita
    End Function

    Public Function CaricaAnniValidiDDL() As List(Of Anno_DDL)
        Dim anniValidi As New List(Of Anno_DDL)
        Dim currentYear As Integer = Date.Today.Year
        anniValidi.Add(New Anno_DDL(0, ""))
        anniValidi.Add(New Anno_DDL(currentYear, currentYear.ToString()))
        anniValidi.Add(New Anno_DDL(currentYear - 1, (currentYear - 1).ToString()))

        Return anniValidi
    End Function


    Private Function ManageException(ex As Exception) As String
        Dim method = New StackFrame(1).GetMethod()
        Dim nomeRoutine = method.ReflectedType.Name & "." & method.Name
        Scrivi_LOG(_coreDBContext, nomeRoutine, ex.Message)
        Return nomeRoutine
    End Function
End Class

Public Class CarburantiHelper
    Public Shared Function VerificaValiditaLtRichiestiTuttiCarburanti(righeNuove As List(Of VenditaCarburantiDto), righeModificate As List(Of VenditaCarburantiDto), svc As Vendita_Carburanti_DAL) As String
        Dim tutteLeRighe = righeNuove.Concat(righeModificate).ToList()
        Dim carburantiGruppati = tutteLeRighe.GroupBy(Function(s) New With {Key s.Anno_Cod, Key s.Tipo_Carburante_Cod, Key s.PIVA_Cliente, Key s.Conto_Proprio_Terzi_Cod})
        Dim errori As String = ""
        For Each group In carburantiGruppati
            Dim elemGrupopo = group.First()
            Dim ltAssegnatiAlGruppo = elemGrupopo.Lt_Assegnati
            Dim elementiGruppo = group.ToList()
            Dim sumGruppo = elementiGruppo.Sum(Function(s) s.Lt)

            Dim elementiDB = svc.LeggiCarburanteVendutoAlCliente(group.Key.PIVA_Cliente, group.Key.Anno_Cod, group.Key.Tipo_Carburante_Cod, group.Key.Conto_Proprio_Terzi_Cod)

            Dim elementiDBNonModificati As New List(Of UMA_Vendite)
            For Each elemDB In elementiDB
                If Not elementiGruppo.Any(Function(s) s.Id_Vendite = elemDB.Id_Vendite) Then
                    sumGruppo += elemDB.Lt
                    elementiDBNonModificati.Add(elemDB)
                End If
            Next

            If sumGruppo > ltAssegnatiAlGruppo Then
                ' Dim ltElementiNonModificati = String.Join(",", elementiDBNonModificati.Select(Function(s) s.Lt))
                ' Dim ltTentativaDiVendita = String.Join(",", elementiGruppo.Select(Function(s) s.Lt).ToArray())
                errori += "Non è possibile vendere " & sumGruppo & " lt. a " & elemGrupopo.Ragione_Sociale & " (Cuaa " & elemGrupopo.Cuaa & ").         I Lt acquistabili sono " & elemGrupopo.Lt_Acquistabili & " <br/>"
                'Return "Non è possibile la vendita di Lt " & ltElementiNonModificati & If(ltElementiNonModificati.Any(), "-", "") & ltTentativaDiVendita & " (totale " & sumGruppo & "). Lt.Assegnati " & ltAssegnatiAlGruppo & ", Lt già acquistati " & elemGrupopo.Totale_Lt_Acquistati & ", Lt acquistabili " & elemGrupopo.Lt_Acquistabili & " <br/>"
            End If
        Next
        Return errori
    End Function

    Private Shared Function ConvertDataTableInVenditeDto(dt As DataTable) As List(Of VenditaCarburantiDto)
        Dim result As New List(Of VenditaCarburantiDto)
        For Each row In dt.Rows
            Dim elem As New VenditaCarburantiDto()

            elem.Id_Vendite = row.Item("Id_Vendite")
            elem.PIVA_Venditore = row.item("PIVA_Venditore")
            elem.PIVA_Cliente = row.item("PIVA_Cliente")
            elem.Anno_Cod = row.item("Anno")
            elem.Conto_Proprio_Terzi_Cod = row.item("Conto_Proprio_Terzi")
            elem.Tipo_Carburante_Cod = row.item("Tipo_Carburante")
            elem.Lt = row.item("Lt")
            result.Add(elem)
        Next
        Return result
    End Function

    Private Shared Sub ControllaStringEmpty(ByRef obj As Object, ByVal defaultValue As String, ByRef stringaErrore As String, ByVal prop As String, Optional nomeCol As String = Nothing)
        Dim stringa = obj.GetType.GetProperty(prop).GetValue(obj, Nothing)
        If IsNothing(stringa) OrElse String.IsNullOrEmpty(stringa) Then
            If Not IsNothing(defaultValue) Then
                obj.GetType.GetProperty(prop).SetValue(obj, defaultValue, Nothing)
            Else
                stringaErrore &= String.Format("La proprietà {0} della riga inserita è obbligatoria e non può essere vuota. <br/>", If(nomeCol IsNot Nothing, nomeCol, prop))
            End If
        End If
    End Sub

    Private Shared Sub ControllaNumZero(ByRef obj As Object, ByVal defaultValue As Decimal?, ByRef stringaErrore As String, ByVal prop As String, Optional nomeCol As String = Nothing, Optional equals As Integer = 0)
        Dim num = obj.GetType.GetProperty(prop).GetValue(obj, Nothing)
        If IsNothing(num) OrElse num = equals Then
            If Not IsNothing(defaultValue) Then
                obj.GetType.GetProperty(prop).SetValue(obj, defaultValue, Nothing)
            Else
                Dim objStr As String = obj.GetType.Name
                stringaErrore &= String.Format("La proprietà {0} della riga inserita è obbligatoria e non può essere vuota. <br/>", If(nomeCol IsNot Nothing, nomeCol, prop), objStr)
            End If
        End If
    End Sub
    Private Shared Sub ControllaPresenza(ByRef obj As Object, ByVal defaultValue As Object, ByRef stringaErrore As String, ByVal prop As String, Optional nomeCol As String = Nothing)
        Dim val = obj.GetType.GetProperty(prop).GetValue(obj, Nothing)
        If IsNothing(val) Then
            If Not IsNothing(defaultValue) Then
                obj.GetType.GetProperty(prop).SetValue(obj, defaultValue, Nothing)
            Else
                Dim objStr As String = obj.GetType.Name
                stringaErrore &= String.Format("La proprietà {0} della riga inserita è obbligatoria e non può essere vuota. <br/>", If(nomeCol IsNot Nothing, nomeCol, prop), objStr)
            End If
        End If
    End Sub

    Public Shared Function ControllaPresenzaCampiObbligatori(carburante As VenditaCarburantiDto)
        Dim stringaErrore As String = ""
        ControllaStringEmpty(carburante, Nothing, stringaErrore, "Azienda_Venditore_Cod", "Azienda Venditore")
        ControllaNumZero(carburante, Nothing, stringaErrore, "Anno_Cod", "Anno")
        ControllaStringEmpty(carburante, Nothing, stringaErrore, "Cuaa")
        ControllaStringEmpty(carburante, Nothing, stringaErrore, "Ragione_Sociale", "Ragione Sociale")
        ControllaNumZero(carburante, Nothing, stringaErrore, "Conto_Proprio_Terzi_Cod", "Conto Proprio o Terzi", -2)
        ControllaStringEmpty(carburante, Nothing, stringaErrore, "Tipo_Carburante_Cod", "Tipo Carburante")
        ControllaNumZero(carburante, Nothing, stringaErrore, "Lt")
        ControllaNumZero(carburante, Nothing, stringaErrore, "Tipo_Documento_Cod", "Tipo Documento")
        ControllaPresenza(carburante, Nothing, stringaErrore, "Data_Documento", "Data Documento")
        ControllaStringEmpty(carburante, Nothing, stringaErrore, "Nr_Documento", "Nr Documento")




        Return stringaErrore
    End Function

    Friend Shared Function ControllaDataDocumentoValida(annoImpostato As Date?) As String
        Dim annoCorrente = Date.Now.Year
        If Not (annoImpostato?.Year = annoCorrente OrElse annoImpostato?.Year = (annoCorrente - 1)) Then
            Return "Data Documento deve essere l'anno corrente o l'anno precedente <br/>"
        End If
        Return ""
    End Function

    Friend Shared Function ControllaValiditaLtRichiesti(carb As VenditaCarburantiDto, svc As Vendita_Carburanti_DAL) As String
        'Dim filtriLtAcquistati = New FiltriAggiornaTotaleLtAcquistati(carburante.Anno_Cod, carburante.Cuaa, carburante.Tipo_Carburante_Cod, carburante.Conto_Proprio_Terzi_Cod)
        'Dim totaleAcuiqstati = svc.OttieneTotaleLtAcquistati(filtriLtAcquistati)

        'Dim filtriLtAsseg = New FiltriAggiornaLtAssegnati(carburante.Anno_Cod, carburante.PIVA_Cliente, carburante.Tipo_Carburante_Cod, carburante.Conto_Proprio_Terzi_Cod)
        'Dim assegnati = svc.OttieneLtAssegnati(filtriLtAsseg)

        Dim massimoLtAcquistabili = carb.LtInPrecedenza + carb.Lt_Acquistabili
        If carb.Lt > massimoLtAcquistabili Then
            Return "I litri richiesti (" & carb.Lt & ") per " & carb.Ragione_Sociale & ", (CUAA: " & carb.Cuaa & ") sono maggiori dei litri acquistabili  (" & massimoLtAcquistabili & ")  <br/>"
        End If

        Return ""
    End Function

    Public Function OttieneTipoCarburante(cod As Integer)
        Select Case cod
            Case 2
                Return "Gasolio"
            Case 3
                Return "Benzina"
            Case 8
                Return "Gasolio Serra"
            Case Else
                Throw New Exception("OttieneTipoCarburante: Codice invalido")
        End Select

    End Function

    Public Function OttieneContoProprioTerzi(cod As Integer)
        Select Case cod
            Case 0
                Return "Conto Proprio"
            Case -1
                Return "Conto Terzi"
            Case Else
                Throw New Exception("OttieneContoProprioTerzi: Codice invalido")
        End Select
    End Function

    Public Function OttieneTipoDocuemnto(cod As Integer)
        Select Case cod
            Case 1
                Return "DAS"
            Case 2
                Return "Fattura accompagnatoria"
            Case Else
                Throw New Exception("OttieneTipoDocuemnto: Codice invalido")
        End Select
    End Function

    Friend Function OttieneTipoDocumento(cod As Integer) As String
        Select Case cod
            Case 1
                Return "DAS"
            Case 2
                Return "Fattura accompagnatoria"
            Case Else
                Throw New Exception("Metodo OttieneTipoDocumento: Codice invalido")
        End Select
    End Function
End Class

