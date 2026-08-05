Imports System.Data.Entity
Imports System.Threading.Tasks
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO

Public Class Agenda_R : Inherits EFatturaBaseDAL

    Private ReadOnly _objParametriServer As AgronicaCoreParametri
    Public Sub New(ByVal objParametriServer As AgronicaCoreParametri)
        _objParametriServer = objParametriServer
    End Sub
    Public Function LeggiAgenda(ByVal agenda As AgendaXLavCod, ByVal tipoImpresa As enum_TipoImpresaGerarchia, ByVal debug As Boolean) As FatturaGiasFromDB

        Dim nomeProcedura = "Agenda.LeggiAgenda"

        Dim result As New FatturaGiasFromDB

        Dim gefutils As New Gias_EF_Utility
        Dim efConnString As String = gefutils.GetEntityConnectionString(_objParametriServer.StringaConnessione)

        Dim t1 = Task.Factory.StartNew(Function() LeggiDatiPrincipaliCedente(efConnString, agenda.PIVA))
        Dim t2 = Task.Factory.StartNew(Function() LeggiCodiciCedente(efConnString, agenda.PIVA))
        Dim t3 = Task.Factory.StartNew(Function() LeggiDatiFatturaCessionario(efConnString, agenda))
        Dim t4 = Task.Factory.StartNew(Function() LeggiDatiDocumentiCollegati(efConnString, agenda))
        Dim t5 = Task.Factory.StartNew(Function() LeggiRiferimentiOrdini(efConnString, agenda))
        Dim t6 = Task.Factory.StartNew(Function() LeggiRiferimentoNotaCreditoDebito(efConnString, agenda))
        Dim t7 = Task.Factory.StartNew(Function() LeggiPagamenti(efConnString, agenda))

        Dim tasks = New List(Of Task) From {t1, t2, t3, t4, t5, t6, t7}

        Try

            Task.WaitAll(tasks.ToArray())

            Dim dpc = t1.Result
            Dim dpa = t2.Result
            Dim df = t3.Result
            Dim dc = t4.Result
            Dim mdte = t5.Result
            Dim ncd = t6.Result
            Dim pag = t7.Result

            result = New FatturaGiasFromDB With
            {
                .PivaReale = dpc.PivaReale,
                .Cedente = New CedenteFromDb With
                {
                    .TipoImpresa = tipoImpresa,
                    .DatiPrincipali = dpc.Cedente,
                    .DatiAggiuntivi = dpa
                },
                .DatiFattura = New DatiFatturaFromDB With
                {
                    .Testata = df.DatiPrincipali,
                    .Dettagli = df.Dettagli
                },
                .Cessionario = df.Cessionario,
                .DocumentiCOllegati = dc,
                .OrdiniAcquisto = mdte,
                .RifNotaCreditoDebito = ncd,
                .DatiPagamento = pag
            }

            If (result.DatiFattura.Testata.Cod_Destinazione Is Nothing OrElse result.DatiFattura.Testata.Cod_Destinazione = 0) AndAlso
                (result.DatiFattura.Testata.Cod_IndirizzoDestinazione Is Nothing OrElse result.DatiFattura.Testata.Cod_IndirizzoDestinazione = 0) Then
                result.CessionarioDiverso = result.Cessionario
            Else
                result.CessionarioDiverso = LeggiDatiCessionarioDiverso(efConnString, agenda)
            End If


            ' Se tipo impresa 5 (Impresa Individuale) leggo dati legale rappresentante
            If tipoImpresa = enum_TipoImpresaGerarchia.DittaIndividuale Then
                result.LegaleRappresentante = LeggiLegaleRappresentante(efConnString, agenda)
            End If

            If debug Then
                result.DatiFattura.Testata.Doc_Numero_Des = GetPrefissoDoc(agenda.PIVA) & result.DatiFattura.Testata.Doc_Numero_Des
            End If


            Dim ids = result.DocumentiCOllegati.Select(Function(s) CInt(s.Id_Agenda_Rif)).ToList()
            result.RiepilogoDocumentiCollegati = LeggiRiepilogoDocumentiCollegati(efConnString, ids)

        Catch ex As AggregateException
            Throw RaiseDAlException(ex, nomeProcedura)
        End Try

        Return result

    End Function

    Private Function LeggiLegaleRappresentante(ByVal efConnString As String, ByVal agenda As AgendaXLavCod) As Object

        Dim dal = New Gias_DeveloperServer_Entities(efConnString)
        Dim result As Object

        Using dal
            'Dim qry = (From rm In dal.Risorse_Umane
            '           Join c In dal.Contatti
            '            On rm.Piva Equals c.Piva And rm.Cod_Contatto Equals c.Cod_Contatto
            '           Where rm.Cod_Rapporto.Equals(COD_LEGALE) AndAlso rm.Piva.Equals(agenda.PIVA) AndAlso
            '           rm.Validita_Inizio <= DateTime.Now AndAlso rm.Validita_Fine >= DateTime.Now
            '           Select New With
            '              {
            '                    c.Cognome, c.Nome, c.Rag_Soc,
            '                    c.Piva, c.Codice_Fiscale,
            '                    c.Cod_Contatto, c.Id_CF
            '              }
            '    ).ToList()

            Dim qry = (From rm In dal.Risorse_Umane
                       Join c In dal.Contatti
                        On rm.Cod_Contatto Equals c.Cod_Contatto
                       Where rm.Cod_Rapporto.Equals(COD_LEGALE) AndAlso
                       rm.Validita_Inizio <= DateTime.Now AndAlso rm.Validita_Fine >= DateTime.Now
                       Select New With
                          {
                                c.Cognome, c.Nome, c.Rag_Soc,
                                c.Piva, c.Codice_Fiscale,
                                c.Cod_Contatto, c.Id_CF
                          }
                ).ToList()


            Return qry.FirstOrDefault()

        End Using

        Return result


    End Function

    Private Function GetPrefissoDoc(ByVal piva As String) As String

        Dim prefix As String = ""
        Select Case piva
            Case "00935560367"
                prefix = "GAR-2"
            Case "00040710295"
                prefix = "COF"
            Case "03668120367"
                prefix = "ZAN"
            Case "00963650338"
                prefix = "ANF"
            Case "01131040493"
                prefix = "MAS"
            Case "00555441203"
                prefix = "CAB"
            Case "04287060406"
                prefix = "BOS"
        End Select

        Return prefix
    End Function

    Private Function LeggiDatiCessionarioDiverso(ByVal efConnString As String, ByVal agenda As AgendaXLavCod) As Object

        Dim dal = New Gias_DeveloperServer_Entities(efConnString)
        Dim result As Object

        Using dal
            Dim qry = (From m In dal.Movimenti
                       Group Join rm In dal.Risorse_Umane
                       On rm.Cod_RisUm Equals m.Cod_Destinazione Into rm_group = Group
                       From _rm_group In rm_group.DefaultIfEmpty()
                       Group Join c In dal.Contatti
                       On _rm_group.Cod_Contatto Equals c.Cod_Contatto Into c_group = Group
                       From _c_group In c_group.DefaultIfEmpty()
                       Group Join ind In dal.Indirizzi
                       On m.Cod_IndirizzoDestinazione Equals ind.cod_indirizzo Into ind_group = Group
                       From _ind_group In ind_group.DefaultIfEmpty()
                       Where m.Id_Agenda.Equals(agenda.IdAgenda) AndAlso m.PIVA.Equals(agenda.PIVA) _
                       AndAlso m.Cau_Mov.Trim().Equals(CAU_REGISTRAZIONI)
                       Select New With
                          {
                                .CessionarioDiverso = New With
                                {
                                        _c_group.Cognome, _c_group.Nome, _c_group.Rag_Soc,
                                        _c_group.Piva, _c_group.Codice_Fiscale,
                                        _c_group.Cod_Contatto, _c_group.Id_CF,
                                        _ind_group.ind_des,
                                        _ind_group.frz_des,
                                        _ind_group.CAP,
                                        _ind_group.com_des,
                                        _ind_group.pro_cod,
                                        _ind_group.pro_cod_istat,
                                        _ind_group.stato,
                                        _ind_group.com_cod_istat,
                                        .Tipo_Indirizzo = 0,
                                        _rm_group.Cod_RisUm
                                }
                          }
                ).ToList()

            Dim cess = (From d In qry Select d.CessionarioDiverso).Where(Function(s) Not s Is Nothing).FirstOrDefault()
            Return cess

        End Using

        Return result

    End Function
    Private Function LeggiDatiFatturaCessionario(ByVal efConnString As String, ByVal agenda As AgendaXLavCod) As Object

        Dim dal = New Gias_DeveloperServer_Entities(efConnString)
        dal.Database.CommandTimeout = 3600
        Dim result As Object

        Using dal
            Dim qry = (From m In dal.Movimenti
                       Group Join md In dal.Movimenti_dettagli
                       On m.Id_Agenda Equals md.Id_Agenda And m.Id_Mov Equals md.Id_Mov And m.PIVA Equals md.PIVA
                       Into md_group = Group
                       From _md_group In md_group.DefaultIfEmpty()
                       Group Join rm In dal.Risorse_Umane
                       On rm.Cod_RisUm Equals m.Cod_RisUm Into rm_group = Group
                       From _rm_group In rm_group.DefaultIfEmpty()
                       Group Join c In dal.Contatti
                       On _rm_group.Cod_Contatto Equals c.Cod_Contatto And c.Piva Equals _rm_group.Piva
                       Into c_group = Group
                       From _c_group In c_group.DefaultIfEmpty()
                       Group Join ind In dal.Indirizzi
                       On m.Cod_IndirizzoRisUm Equals ind.cod_indirizzo Into ind_group = Group
                       From _ind_group In ind_group.DefaultIfEmpty()
                       Where m.Id_Agenda.Equals(agenda.IdAgenda) AndAlso m.PIVA.Equals(agenda.PIVA) AndAlso _md_group.Ordine_Det <> 1000
                       Select New With
                          {
                                .DatiPrincipali = New With
                                {
                                    m.Cau_Mov, m.Mov_Desc, m.Doc_Numero, m.Cod_RisUm, m.Cod_RisUm_Aggiuntivo, m.Cod_RisUm_Altro,
                                    .TotaleFattura = m.Num_Protocollo,
                                    m.Cod_Destinazione, m.Cod_IndirizzoDestinazione, m.Causale_Trasporto,
                                    m.Doc_Numero_Des, m.Doc_Numero_Sin, m.Data_Registrazione, m.Sezionale_Cod, m.Data_Movimento,
                                    agenda.Lav_cod, agenda.Des_lib, m.Id_Agenda, m.TipoDocumento
                                },
                                .Cessionario = New With
                                {
                                        _c_group.Cognome, _c_group.Nome, _c_group.Rag_Soc,
                                        _c_group.Piva, _c_group.Codice_Fiscale,
                                        _c_group.Cod_Contatto, _c_group.Id_CF,
                                        _ind_group.ind_des,
                                        _ind_group.frz_des,
                                        _ind_group.CAP,
                                        _ind_group.com_des,
                                        _ind_group.pro_cod,
                                        _ind_group.pro_cod_istat,
                                        _ind_group.stato,
                                        _ind_group.com_cod_istat,
                                        .Tipo_Indirizzo = 0,
                                        _rm_group.Cod_RisUm
                                },
                                .Dettagli = _md_group
                          }
                ).ToList()

            Dim movReg = (From d In qry Select d).FirstOrDefault(Function(s) s.DatiPrincipali.Cau_Mov.Trim().Equals(CAU_REGISTRAZIONI))
            Dim dp = movReg.DatiPrincipali

            ' ordinamento righe dettaglio per campo ordine_det
            Dim det = (From d In qry Select d.Dettagli).Where(Function(s) Not s Is Nothing).OrderBy(Function(d) d.Ordine_Det)
            Dim cess = movReg.Cessionario

            result = New With
            {
                .DatiPrincipali = dp,
                .Dettagli = det,
                .Cessionario = cess
            }

        End Using

        Return result

    End Function

    Private Function LeggiRiepilogoDocumentiCollegati(ByVal efConnString As String, ByVal idDocRefs As List(Of Integer)) As IEnumerable(Of Object)

        Dim dal = New Gias_DeveloperServer_Entities(efConnString)
        Dim result As IEnumerable(Of Object)

        Using dal
            Dim datiRiepilogo = From md In dal.Movimenti_dettagli
                                Where md.Ordine_Det <> 1000 AndAlso
                                idDocRefs.Contains(md.Id_Agenda)
                                Group md By md.Id_Agenda
                                Into riepilogo = Group, totalCount = Count(md.Id_Agenda)
            result = datiRiepilogo.ToList()

        End Using

        Return result

    End Function
    Private Function LeggiDatiDocumentiCollegati(ByVal efConnString As String, ByVal agenda As AgendaXLavCod) As IEnumerable(Of Object)

        Dim dal = New Gias_DeveloperServer_Entities(efConnString)
        dal.Database.CommandTimeout = 3600
        Dim result As IEnumerable(Of Object)

        Dim Lav_cod As Integer() = {LAVCOD_BOLLA_EMESSA, LAVCOD_DDT_CONTABILIZZATO_EMESSO, LAVCOD_AUTO_DDT_EMESSO}


        Using dal
            Dim qry = From mdr In dal.Mov_Dettagli_Riferimenti
                      Group Join m In dal.Movimenti
                      On mdr.Id_Agenda_Rif Equals m.Id_Agenda
                      Into m_group = Group
                      From _m_group In m_group.DefaultIfEmpty
                      Group Join md In dal.Movimenti_dettagli
                      On md.Id_Mov_Det Equals mdr.Id_Mov_Det
                      Into md_group = Group
                      From _md_group In md_group.DefaultIfEmpty
                      Where mdr.Id_Agenda.Equals(agenda.IdAgenda) AndAlso
                      _m_group.Cau_Mov.Equals(CAU_REGISTRAZIONI) AndAlso
                      Lav_cod.Contains(mdr.Lav_Cod_Rif)
                      Select New With
                          {
                                mdr.Id_Agenda,
                                mdr.Id_Agenda_Rif,
                                _m_group.Doc_Numero_Sin,
                                _m_group.Doc_Numero,
                                _m_group.Doc_Numero_Des,
                                _m_group.Data_Movimento,
                                _md_group.Ordine_Det
                          }
            result = qry.ToList()
        End Using

        Return result

    End Function

    Private Function LeggiPagamenti(ByVal efConnString As String, ByVal agenda As AgendaXLavCod) As IEnumerable(Of Object)

        Try
            Dim dal = New Gias_DeveloperServer_Entities(efConnString)
            dal.Database.CommandTimeout = 3600

            Dim qry = From p In dal.Pagamenti
                      Group Join pc In dal.Pagamenti_Causali
                      On p.Cau_Pagamento Equals pc.Cau_Pagamento
                      Into pc_group = Group
                      From _pc_group In pc_group.DefaultIfEmpty
                      Group Join l In dal.Liquidita
                      On p.Cod_Liquidita_Dare Equals l.Cod_Liquidita And p.Piva Equals l.Piva
                      Into l_group = Group
                      From _l_group In l_group.DefaultIfEmpty
                      Group Join ic In dal.Ist_Credito
                      On ic.Cod_Istituto Equals _l_group.Cod_Istituto And ic.Piva Equals _l_group.Piva
                      Into ic_group = Group
                      From _ic_group In ic_group.DefaultIfEmpty
                      Where p.Id_Agenda.Equals(agenda.IdAgenda) AndAlso p.Piva.Equals(agenda.PIVA) _
                      AndAlso p.Previsto_Avvenuto = 0
                      Select New With
                          {
                                .DatiPagamento = New With {
                                    p.Importo,
                                    p.Percentuale,
                                    p.Data_Pagamento,
                                    p.DataScadenza_Manuale,
                                    p.Cau_Pagamento,
                                    p.Cod_Liquidita_Avere,
                                    p.Cod_Liquidita_Dare,
                                    p.Previsto_Avvenuto
                                },
                                .CausalePagamento = New With {
                                    _pc_group.Cau_Pagamento_Des,
                                    _pc_group.Cau_Pagamento_Sigla,
                                    _pc_group.Tipo
                                },
                                .DatiBancari = New With
                                {
                                    _l_group.Numero,
                                    _l_group.Abi,
                                    _l_group.Cab,
                                    _l_group.Cin,
                                    _l_group.Cifre_Controllo,
                                    _l_group.Nazione,
                                    _l_group.Bic,
                                    _ic_group.Istituto_Des
                                }
                          }

            Return qry.ToList()

        Catch ex As Exception
            Throw RaiseDAlException(ex, "Agenda.LeggiPagamenti")
        End Try

    End Function
    Private Function LeggiRiferimentoNotaCreditoDebito(ByVal efConnString As String, ByVal agenda As AgendaXLavCod) As Object

        Try
            Dim dal = New Gias_DeveloperServer_Entities(efConnString)
            dal.Database.CommandTimeout = 3600

            Dim qry = From mdte In dal.Mov_Dettaglio_Tecnico_Extra
                      Where mdte.Piva.Equals(agenda.PIVA) AndAlso
                            mdte.Id_Mov_Det = 0 AndAlso
                            mdte.Id_Agenda.Equals(agenda.IdAgenda) AndAlso
                            (mdte.N_Nota_DDT <> "" OrElse mdte.N_Nota_Fattura <> "")
                      Select New With {
                            mdte.Id_Agenda,
                            mdte.N_Nota_DDT,
                            mdte.Data_Nota_DDT,
                            mdte.N_Nota_Riga_DDT,
                            mdte.N_Nota_Fattura,
                            mdte.Data_Nota_Fattura
                      }

            Return qry.ToList().FirstOrDefault()
        Catch ex As Exception
            Throw RaiseDAlException(ex, "Agenda.LeggiRiferimentoNotaCreditoDebito")
        End Try

    End Function

    Private Function LeggiRiferimentiOrdini(ByVal efConnString As String, ByVal agenda As AgendaXLavCod) As IEnumerable(Of Object)

        Try
            Dim dal = New Gias_DeveloperServer_Entities(efConnString)
            dal.Database.CommandTimeout = 3600

            Dim qry = From md In dal.Movimenti_dettagli
                      Join mdte In dal.Mov_Dettaglio_Tecnico_Extra
                      On md.PIVA Equals mdte.Piva And md.Id_Mov_Det Equals mdte.Id_Mov_Det And md.Id_Agenda Equals mdte.Id_Agenda
                      Where mdte.Piva.Equals(agenda.PIVA) AndAlso mdte.Id_Mov_Det <> 0 AndAlso
                      mdte.Id_Agenda.Equals(agenda.IdAgenda) AndAlso
                      mdte.N_Doc_Cliente <> ""
                      Select New With
                          {
                            mdte.Id_Agenda,
                            mdte.N_Doc_Cliente,
                            mdte.Data_Doc_Cliente,
                            md.Ordine_Det
                          }

            Return qry.ToList()
        Catch ex As Exception
            Throw RaiseDAlException(ex, "Agenda.LeggiRiferimentiOrdini")
        End Try

    End Function


    Private Function LeggiDatiPrincipaliCedente(
        ByVal efConnString As String,
        ByVal pIVA As String) As Object

        Try
            Dim dal = New Gias_DeveloperServer_Entities(efConnString)
            dal.Database.CommandTimeout = 3600
            Dim result As Object

            Using dal
                Dim qry = From imp In dal.Imprese
                          Join impi In dal.ImpresexIndirizzi
                          On imp.PIVA Equals impi.PIVA
                          Join ind In dal.Indirizzi
                          On impi.cod_indirizzo Equals ind.cod_indirizzo
                          Group Join c In dal.Contatti
                          On imp.PIVA Equals c.Cod_Contatto
                          Into c_group = Group
                          From _c_group In c_group.DefaultIfEmpty()
                          Where imp.PIVA.Equals(pIVA) AndAlso
                          impi.Tipo_Indirizzo.Equals(enum_IndirizzoTipo.SedeOperativa)
                          Select New With
                            {
                                .PartitaIva = imp.PIVA,
                                .PivaReale = imp.partitaIvaReale,
                                .Cedente = New With
                                    {
                                        imp.PIVA,
                                        imp.rag_soc,
                                        imp.TipoImpresaGerarchia,
                                        impi.cod_indirizzo,
                                        impi.Tipo_Indirizzo,
                                        ind.ind_des,
                                        ind.frz_des,
                                        ind.CAP,
                                        ind.com_des,
                                        ind.pro_cod,
                                        ind.pro_cod_istat,
                                        ind.stato,
                                        ind.com_cod_istat,
                                        _c_group.Id_CF,
                                        _c_group.Nome,
                                        _c_group.Cognome,
                                        _c_group.Cod_Contatto,
                                       .Codice_Fiscale = _c_group.Cod_Contatto
                                    }
                            }

                result = qry.FirstOrDefault()
            End Using

            Return result
        Catch ex As Exception
            Throw RaiseDAlException(ex, "Agenda.LeggiDatiPrincipaliCedente")
        End Try

    End Function

    Private Function LeggiCodiciCedente(ByVal efConnString As String,
       ByVal pIVA As String) As Object

        Try
            Dim dal = New Gias_DeveloperServer_Entities(efConnString)
            dal.Database.CommandTimeout = 3600
            Dim result As Object

            Using dal
                Dim qry = From imp In dal.Imprese
                          Join impc In dal.Imprese_Codici
                          On imp.PIVA Equals impc.PIVA
                          Where imp.PIVA.Equals(pIVA)
                          Select New With
                          {
                               impc.id_cod,
                               impc.val_cod
                          }
                result = qry.ToList()
            End Using

            Return result

        Catch ex As Exception
            Throw RaiseDAlException(ex, "Agenda.LeggiCodiciCedente")
        End Try

    End Function

    Public Function LeggiAgendeXLavCod(ByVal filtro As AgendaFiltroLettura) As GenerazioneFatture

        If filtro.IsEmpty() Then
            Return LeggiTutteLeAgende(filtro.PIVA, filtro.DataDal)
        Else
            Return LeggiAgendeDaFiltro(filtro)
        End If

    End Function

    Private Function LeggiAgendeDaFiltro(ByVal filtro As AgendaFiltroLettura) As GenerazioneFatture

        Try
            Dim gefutils As New AgronicaCoreEntityFramework.Gias_EF_Utility
            Dim efConnString As String = gefutils.GetEntityConnectionString(_objParametriServer.StringaConnessione)
            Dim dal = New AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities(efConnString)
            dal.Database.CommandTimeout = 3600

            Dim causali As Integer() = {LAVCOD_NOTA_ACCREDITO_EMESSA, LAVCOD_FATTURA_EMESSA}
            If filtro.Ids Is Nothing Then
                filtro.Ids = New List(Of Integer)
            End If

            Dim qry = From agenda In dal.Agenda Order By agenda.Id_Agenda Descending
                      Join movimenti In dal.Movimenti
                      On agenda.Id_Agenda Equals movimenti.Id_Agenda
                      Where causali.Contains(agenda.Lav_Cod) AndAlso agenda.PIVA.Equals(filtro.PIVA) _
                      AndAlso (filtro.Ids.Count = 0 OrElse filtro.Ids.Contains(agenda.Id_Agenda)) _
                      AndAlso (filtro.DataDal.HasValue = False OrElse movimenti.Data_Movimento >= filtro.DataDal.Value) _
                      AndAlso (filtro.DataAl.HasValue = False OrElse movimenti.Data_Movimento <= filtro.DataAl.Value) AndAlso
                      movimenti.Cau_Mov.Equals("4000")
                      Select New AgendaXLavCod With
                        {
                            .PIVA = agenda.PIVA,
                            .IdAgenda = agenda.Id_Agenda,
                            .Des_lib = agenda.des_lib,
                            .Lav_cod = agenda.Lav_Cod,
                            .Cod_RisUm = movimenti.Cod_RisUm,
                            .Doc_numero = movimenti.Doc_Numero,
                            .Doc_NUmero_Sin = movimenti.Doc_Numero_Sin,
                            .Doc_NUmero_Des = movimenti.Doc_Numero_Des
                        }

            Dim agendeTutte = qry.ToList()
            Dim ids = agendeTutte.Select(Function(a) a.IdAgenda)

            'estraggo le modifiche fatte alle agende che ricadono nel filtro
            Dim agendaUltimeOperazioni = From loga In dal.Agronica_Log_Agenda_UltimaOperazione
                                         Group Join sdilog In dal.SDI_Log
                                         On loga.Id_Agenda Equals sdilog.Id_Agenda.Value
                                         Into sdilog_group = Group
                                         From _sdilog In sdilog_group.DefaultIfEmpty()
                                         Where _sdilog.Piva.Equals(filtro.PIVA) AndAlso ids.Contains(loga.Id_Agenda)
                                         Select New With
                                 {
                                    .LogAgenda = loga,
                                    .SdiLog = _sdilog
                                 }

            Dim realmenteEliminate = agendaUltimeOperazioni.Where(Function(ala) ala.LogAgenda.UltimaOperazione.Value = enum_TipoOperazioneDB.Cancellazione)
            Dim modificate = agendaUltimeOperazioni.Where(Function(ala) ala.LogAgenda.UltimaOperazione = enum_TipoOperazioneDB.Modifica _
                                                              AndAlso ala.LogAgenda.Data_Ora_RegistrazioneLog.Value > ala.SdiLog.Data_Gen_XML.Value)

            Return New GenerazioneFatture With
            {
                .DaGenerare = agendeTutte
            }

        Catch ex As Exception
            Throw RaiseDAlException(ex, "Agenda.LeggiAgendeDaFiltro")
        End Try

    End Function

    Private Function LeggiTutteLeAgende(ByVal PIVA As String, ByVal dataDal As DateTime) As GenerazioneFatture

        Try
            Dim gefutils As New Gias_EF_Utility
            Dim efConnString As String = gefutils.GetEntityConnectionString(_objParametriServer.StringaConnessione)
            Dim dal = New Gias_DeveloperServer_Entities(efConnString)
            dal.Database.CommandTimeout = 3600
            Dim logR = New SDI_Log_R(dal)
            Dim dataMax = EndOfDay(DateTime.Now)

            Dim causali As Integer() = {LAVCOD_NOTA_ACCREDITO_EMESSA, LAVCOD_FATTURA_EMESSA}

            ' Estraggo tutte le agende mai processate
            Dim notInSelect = dal.Agenda.Where(Function(a) Not dal.SDI_Log.Select(
                                            Function(l) l.Id_Agenda).Contains(a.Id_Agenda)) _
                                        .Select(Function(agenda) agenda)
            Dim qry1 = From agenda In notInSelect
                       Join movimenti In dal.Movimenti
                          On agenda.Id_Agenda Equals movimenti.Id_Agenda
                       Where causali.Contains(agenda.Lav_Cod) AndAlso agenda.PIVA.Equals(PIVA) _
                          AndAlso movimenti.Data_Movimento >= dataDal _
                          AndAlso movimenti.Data_Movimento <= dataMax _
                          AndAlso movimenti.Cau_Mov.Equals("4000")
                       Select New AgendaXLavCod With
                            {
                                .PIVA = agenda.PIVA,
                                .IdAgenda = agenda.Id_Agenda,
                                .Des_lib = agenda.des_lib,
                                .Lav_cod = agenda.Lav_Cod,
                                .Cod_RisUm = movimenti.Cod_RisUm,
                                .Blocco_Flag = agenda.Blocco_Flag,
                                .ValiditaInizio = agenda.Validita_Inizio,
                                .DataMovimento = movimenti.Data_Movimento,
                                .Doc_numero = movimenti.Doc_Numero,
                                .Doc_NUmero_Sin = movimenti.Doc_Numero_Sin,
                                .Doc_NUmero_Des = movimenti.Doc_Numero_Des
                            }
            Dim agendeMaiProcessate = qry1.ToList()

            Dim statiGiasKO As Integer() = {StatoFattura_Gias.XMLNonGenerabile, StatoFattura_Gias.XMLNonGeneratoPerErrori, StatoFattura_Gias.InviatoSDI_KO}

            ' Estratto tutte le agende già processate ma con errori 
            Dim qry = From log In dal.SDI_Log
                      Join agenda In dal.Agenda
                      On log.Id_Agenda Equals agenda.Id_Agenda
                      Join movimenti In dal.Movimenti
                      On New With {Key .a = CType(movimenti.Id_Agenda, Integer), Key .b = CType(movimenti.Cau_Mov, String)} Equals New With {Key .a = agenda.Id_Agenda, Key .b = "4000"}
                      Where log.Piva.Equals(PIVA) AndAlso log.Gias_Status <> StatoFattura_Gias.InLock _
                      AndAlso log.Gias_Status <> StatoFattura_Gias.XMLEsportato _
                    AndAlso statiGiasKO.Contains(log.Gias_Status.Value) _
                    OrElse (log.Gias_Status.Value.Equals(StatoFattura_Gias.InviatoSDI_OK) AndAlso log.SDI_Status.Value.Equals(StatoFattura_SDI.RicevutaScato))
                      Select New AgendaXLavCod With
                            {
                                .PIVA = agenda.PIVA,
                                .IdAgenda = agenda.Id_Agenda,
                                .Des_lib = agenda.des_lib,
                                .Lav_cod = agenda.Lav_Cod,
                                .Cod_RisUm = movimenti.Cod_RisUm,
                                .Blocco_Flag = agenda.Blocco_Flag,
                                .StatoGias = log.Gias_Status,
                                .StatoSDI = log.SDI_Status,
                                .ValiditaInizio = agenda.Validita_Inizio,
                                .DataMovimento = movimenti.Data_Movimento,
                                .Doc_numero = movimenti.Doc_Numero,
                                .Doc_NUmero_Sin = movimenti.Doc_Numero_Sin,
                                .Doc_NUmero_Des = movimenti.Doc_Numero_Des
                            }

            Dim agendeProcessateKO = qry.ToList()

            Dim agendeTutte = agendeMaiProcessate.Concat(agendeProcessateKO).OrderBy(Function(a) a.IdAgenda).ToList()

            'estraggo solo id di agende di cui sono già stati generati i file xml ma non ancora inviati
            Dim agendeGenerateMaNonInviate = (From log In dal.SDI_Log
                                              Where log.Gias_Status = StatoFattura_Gias.XMLGenerato AndAlso
                                                log.SDI_Status = StatoFattura_SDI.NonDefinito AndAlso log.Piva.Equals(PIVA) AndAlso
                                                  log.Id_Agenda.HasValue()
                                              Select log.Id_Agenda.Value).Distinct().ToList()

            Dim ids = agendeTutte.Select(Function(a) a.IdAgenda).Concat(agendeGenerateMaNonInviate).Distinct().ToList()

            'estraggo le modifiche fatte alle agende che ricadono nel filtro
            Dim agendaUltimeOperazioni = From loga In dal.Agronica_Log_Agenda_UltimaOperazione
                                         Group Join sdilog In dal.SDI_Log
                                         On loga.Id_Agenda Equals sdilog.Id_Agenda.Value
                                         Into sdilog_group = Group
                                         From _sdilog In sdilog_group.DefaultIfEmpty()
                                         Where _sdilog.Piva.Equals(PIVA) AndAlso ids.Contains(loga.Id_Agenda)
                                         Select New With
                                 {
                                    .LogAgenda = loga,
                                    .SdiLog = _sdilog
                                 }

            Dim realmenteEliminate = agendaUltimeOperazioni.Where(Function(ala) ala.LogAgenda.UltimaOperazione.Value = enum_TipoOperazioneDB.Cancellazione)
            Dim modificate = agendaUltimeOperazioni.Where(Function(ala) ala.LogAgenda.UltimaOperazione = enum_TipoOperazioneDB.Modifica _
                                                              AndAlso ala.LogAgenda.Data_Ora_RegistrazioneLog.Value > ala.SdiLog.Data_Gen_XML.Value)

            Dim agendeEliminateDaCancellare = New List(Of AgendaXLavCod)
            If realmenteEliminate.Any Then
                Dim qryEliminate = From elim In realmenteEliminate
                                   Select New AgendaXLavCod With
                                {
                                    .PIVA = elim.LogAgenda.Piva,
                                    .IdAgenda = elim.LogAgenda.Id_Agenda,
                                    .Des_lib = elim.LogAgenda.Des_Lib,
                                    .Lav_cod = elim.LogAgenda.Lav_Cod,
                                    .DataModificaEliminazione = elim.LogAgenda.Data_Ora_RegistrazioneLog,
                                    .DataGenerazioneXML = elim.SdiLog.Data_Gen_XML,
                                    .NomeFIleXml = If(elim.SdiLog.NomeFileXML Is Nothing, "", elim.SdiLog.NomeFileXML),
                                    .ID_LOg = elim.SdiLog.Id_Log,
                                    .Doc_NUmero_Sin = If(elim.SdiLog.Doc_Numero_Sin Is Nothing, "", elim.SdiLog.Doc_Numero_Sin),
                                    .Doc_NUmero_Des = If(elim.SdiLog.Doc_Numero_Des Is Nothing, "", elim.SdiLog.Doc_Numero_Des),
                                    .Doc_numero = If(elim.SdiLog.Doc_Numero Is Nothing, 0, elim.SdiLog.Doc_Numero)
                                }
                agendeEliminateDaCancellare.AddRange(qryEliminate.ToList())
            End If


            Dim agendeModificateDaRigenerare = New List(Of AgendaXLavCod)
            If modificate.Any Then
                Dim qryModificate = From agenda In dal.Agenda
                                    Join movimenti In dal.Movimenti
                              On agenda.Id_Agenda Equals movimenti.Id_Agenda
                                    Join modi In modificate
                               On modi.LogAgenda.Id_Agenda Equals agenda.Id_Agenda
                                    Where causali.Contains(agenda.Lav_Cod) AndAlso agenda.PIVA.Equals(PIVA) _
                              AndAlso movimenti.Data_Movimento >= dataDal _
                              AndAlso movimenti.Cau_Mov.Equals("4000")
                                    Select New AgendaXLavCod With
                                {
                                    .PIVA = agenda.PIVA,
                                    .IdAgenda = agenda.Id_Agenda,
                                    .Des_lib = agenda.des_lib,
                                    .Lav_cod = agenda.Lav_Cod,
                                    .Cod_RisUm = movimenti.Cod_RisUm,
                                    .Blocco_Flag = agenda.Blocco_Flag,
                                    .ValiditaInizio = agenda.Validita_Inizio,
                                    .DataMovimento = movimenti.Data_Movimento,
                                    .Doc_numero = movimenti.Doc_Numero,
                                    .Doc_NUmero_Sin = movimenti.Doc_Numero_Sin,
                                    .Doc_NUmero_Des = movimenti.Doc_Numero_Des,
                                    .DataModificaEliminazione = modi.LogAgenda.Data_Ora_RegistrazioneLog,
                                    .DataGenerazioneXML = modi.SdiLog.Data_Gen_XML
                                }
                agendeModificateDaRigenerare = qryModificate.ToList().Where(Function(a) Not agendeTutte.Any(Function(y) a.IdAgenda.Equals(y.IdAgenda))).ToList()
            End If

            Return New GenerazioneFatture With
            {
                .DaGenerare = agendeTutte.Concat(agendeModificateDaRigenerare).ToList(),
                .Eliminate = agendeEliminateDaCancellare,
                .Modificate = agendeModificateDaRigenerare
            }

        Catch ex As Exception
            Throw RaiseDAlException(ex, "Agenda.LeggiTutteLeAgende")
        End Try

    End Function

    Public Function EndOfDay(ByVal d As DateTime) As DateTime
        Return DateTime.Parse(d.ToShortDateString().Trim() + " 23:59:59")
    End Function

End Class


Public Class Agenda_W : Inherits EFatturaBaseDAL

    Private ReadOnly _objParametriServer As AgronicaCoreParametri
    Public Sub New(ByVal objParametriServer As AgronicaCoreParametri)
        _objParametriServer = objParametriServer
    End Sub

    Public Sub BloccoAgenda(ByVal idAgenda As Integer, ByVal blocco As Integer)

        Try
            Dim gefutils As New AgronicaCoreEntityFramework.Gias_EF_Utility
            Dim efConnString As String = gefutils.GetEntityConnectionString(_objParametriServer.StringaConnessione)
            Dim dal = New AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities(efConnString)

            Dim agenda = (From a In dal.Agenda Where a.Id_Agenda = idAgenda).FirstOrDefault
            agenda.Blocco_Flag = blocco
            agenda.Blocco_Username = _objParametriServer.UtenteUsername
            agenda.Blocco_Data = DateTime.Now
            dal.Entry(agenda).State = EntityState.Modified
            dal.SaveChanges()

        Catch ex As Exception
            Throw RaiseDAlException(ex, "Agenda.BloccaAgenda")
        End Try

    End Sub

End Class