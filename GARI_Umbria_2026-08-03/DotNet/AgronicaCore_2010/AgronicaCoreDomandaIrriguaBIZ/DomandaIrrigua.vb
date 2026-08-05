Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDomandaIrriguaDAL
Public Class DomandaIrrigua_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function CheckExistDomandaIrrigua(ByVal id As Integer,
                                 ByVal piva As String,
                                 ByVal anno As Integer,
                                 ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean
        Dim ret As Boolean = False
        Dim nomeRoutine As String = "AgronicaCoreDomandaIrriguaBIZ.DomandaIrrigua_R.CheckExistDomandaIrrigua()"

        Dim messaggioErrore As String = ""

        If id = 0 And (piva = "" Or anno = 0) Then
            Throw New Exception("Specificare id della domanda oppure partita iva ed anno")
        End If
        Try
            Dim dih_dal As New AgronicaCoreDomandaIrriguaDAL.DomandaIrrigua_Testata_R
            Dim dth As New DataTable
            If id <> 0 Then
                dth = dih_dal.Leggi(id, "", -1, "", "", objParametri)
            Else
                dth = dih_dal.Leggi(0, piva, -1, "", "", objParametri, New Date(anno, 1, 1), New Date(anno, 12, 31))
            End If
            If dth.Rows.Count > 0 Then
                ret = True
            Else
                ret = False
            End If
        Catch ex As Exception
            Throw New Exception(ex.Message, ex.InnerException)
        End Try
        Return ret
    End Function


    Public Function LeggiDomanda(ByVal id As Integer,
                                 ByVal piva As String,
                                 ByVal anno As Integer,
                                 ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As AgronicaCoreDTOStd.InData.DomandaIrrigua.DomandaIrrigua
        Dim ret As New AgronicaCoreDTOStd.InData.DomandaIrrigua.DomandaIrrigua
        Dim nomeRoutine As String = "AgronicaCoreDomandaIrriguaBIZ.DomandaIrrigua_R.LeggiDomanda()"

        Dim messaggioErrore As String = ""

        If id = 0 And (piva = "" Or anno = 0) Then
            Throw New Exception("Specificare id della domanda oppure partita iva ed anno")
        End If

        Try
            Dim dih_dal As New AgronicaCoreDomandaIrriguaDAL.DomandaIrrigua_Testata_R
            Dim dir_dal As New AgronicaCoreDomandaIrriguaDAL.DomandaIrrigua_Dettaglio_R
            Dim aux As New AgronicaCoreDomandaIrriguaBIZ.DatiAggiuntiviDocumenti_R
            Dim dth As New DataTable

            If id <> 0 Then
                dth = dih_dal.Leggi(id, "", -1, "", "", objParametri)
                If dth.Rows.Count <= 0 Then
                    Throw New Exception(String.Format("Nessuna domanda trovato per l'id {0]", id))
                End If
            Else
                dth = dih_dal.Leggi(0, piva, -1, "", "", objParametri, New Date(anno, 1, 1), New Date(anno, 12, 31))
                If dth.Rows.Count <= 0 Then
                    Throw New Exception(String.Format("Nessuna domanda trovato per la partita iva {0] ed anno {1}", piva, anno))
                End If
            End If

            If dth.Rows.Count > 0 Then
                ret.id = dth.Rows(0)("ID")
                ret.piva = dth.Rows(0)("piva")
                ret.n_protocollo = dth.Rows(0)("N_Protocollo")
                ret.data_protocollo = dth.Rows(0)("Data_Protocollo")
                ret.TipoContratto = dth.Rows(0)("TipoContratto")
                ret.Stato = dth.Rows(0)("Stato")
                ret.ValiditaInizio = dth.Rows(0)("Validita_Inizio")
                ret.ValiditaFine = dth.Rows(0)("Validita_Fine")
                ret.datiAzienda = aux.GetDatiAzienda(ret.piva, objParametri)

                ret.dettaglio = New List(Of AgronicaCoreDTOStd.InData.DomandaIrrigua.DomandaIrriguaRighe)

                Dim dtr = dir_dal.LeggiConCatastoEPianoColturale(ret.id, 0, "", 0, 0, 0, -1, "", "", objParametri)
                For Each row In dtr.Rows
                    ret.dettaglio.Add(New AgronicaCoreDTOStd.InData.DomandaIrrigua.DomandaIrriguaRighe() With {
                                        .riga = row("id_riga"),
                                        .piva = row("piva"),
                                        .sa_cod = row("sa_cod"),
                                        .appezza = row("appezza"),
                                        .id_reg = row("id_reg"),
                                        .Selezionato = Convert.ToBoolean(row("selezionato")),
                                        .app_nome = row("app_nome"),
                                        .Superficie = row("area"),
                                        .veg_cod = row("veg_cod"),
                                        .veg_des = row("coltura"),  'row("veg_des"),
                                        .cul_cod = row("cul_cod"),
                                        .cul_des = IIf(row("cul_des") Is DBNull.Value, "", row("cul_des")),
                                        .gruppo_consegna = 0, 'row("gruppo_consegna"),
                                        .PROV = row("PROV"),
                                        .PROV_Des = row("PROVINCIA"),
                                        .COM = row("COM"),
                                        .COM_Des = row("LOCALITA"),
                                        .Sezione = row("SEZIONE"),
                                        .Foglio = row("FOGLIO"),
                                        .Numero = row("NUMERO")
                                      })
                Next
            End If


        Catch ex As Exception
            Throw New Exception(ex.Message, ex.InnerException)
        End Try

        Return ret

    End Function

    Public Function LeggiPianoColturaleConParticellePerInizializzazioneDomandaIrrigua(ByVal piva As String,
                                                                                      ByVal anno As Integer,
                                                                                      ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As AgronicaCoreDTOStd.InData.DomandaIrrigua.DomandaIrrigua
        Dim ret As New AgronicaCoreDTOStd.InData.DomandaIrrigua.DomandaIrrigua
        Dim nomeRoutine As String = "AgronicaCoreDomandaIrriguaBIZ.DomandaIrrigua_R.LeggiPianoColturaleConParticellePerInizializzazioneDomandaIrrigua()"

        Dim messaggioErrore As String = ""

        If piva = "" Or anno = 0 Then
            Throw New Exception("Specificare partita iva ed anno")
        End If

        Try
            Dim dir_dal As New AgronicaCoreDomandaIrriguaDAL.DomandaIrrigua_Dettaglio_R
            Dim aux As New AgronicaCoreDomandaIrriguaBIZ.DatiAggiuntiviDocumenti_R
            Dim dth As New DataTable

            ret.id = -1
            ret.piva = piva
            ret.n_protocollo = ""
            ret.data_protocollo = AGRODATAINIZIO
            ret.TipoContratto = 1
            ret.Stato = 1
            ret.ValiditaInizio = New Date(anno, 1, 1)
            ret.ValiditaFine = New Date(anno, 12, 31)

            ret.datiAzienda = aux.GetDatiAzienda(piva, objParametri)

            ret.dettaglio = New List(Of AgronicaCoreDTOStd.InData.DomandaIrrigua.DomandaIrriguaRighe)

            Dim riga As Integer = 1
            Dim dtr = dir_dal.LeggiConCatastoEPianoColturaleAllaDataPerInizializzazioneDomandaIrrigua(piva, ret.ValiditaInizio, ret.ValiditaFine, "", "", objParametri)
            For Each row In dtr.Rows
                ret.dettaglio.Add(New AgronicaCoreDTOStd.InData.DomandaIrrigua.DomandaIrriguaRighe() With {
                                        .riga = riga,
                                        .piva = row("piva"),
                                        .sa_cod = row("sa_cod"),
                                        .appezza = row("appezza"),
                                        .id_reg = row("id_reg"),
                                        .Selezionato = 0,
                                        .app_nome = row("app_nome"),
                                        .Superficie = row("area"),
                                        .veg_cod = row("veg_cod"),
                                        .veg_des = row("coltura"), 'IIf(row("veg_des") Is DBNull.Value, "", row("veg_des")),
                                        .cul_cod = row("cul_cod"),
                                        .cul_des = IIf(row("cul_des") Is DBNull.Value, "", row("cul_des")),
                                        .gruppo_consegna = 0, 'row("gruppo_consegna"),
                                        .PROV = row("PROV"),
                                        .PROV_Des = row("PROVINCIA"),
                                        .COM = row("COM"),
                                        .COM_Des = row("LOCALITA"),
                                        .Sezione = row("SEZIONE"),
                                        .Foglio = row("FOGLIO"),
                                        .Numero = row("NUMERO")
                                      })
                riga += 1
            Next
        Catch ex As Exception
            Throw New Exception(ex.Message, ex.InnerException)
        End Try
        Return ret

    End Function

    Public Function LeggiElencoDomande(ByVal pars As AgronicaCoreDTOStd.InData.DomandaIrrigua.LeggiElencoDomandeIrrigue,
                                       ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As List(Of AgronicaCoreDTOStd.InData.DomandaIrrigua.RiepilogoDomandeIrrigue)
        Dim ret As New List(Of AgronicaCoreDTOStd.InData.DomandaIrrigua.RiepilogoDomandeIrrigue)

        Try

            Dim dir_dal As New AgronicaCoreDomandaIrriguaDAL.DomandaIrrigua_Dettaglio_R

            Dim ValiditaInizio = AGRODATAINIZIO
            If pars.StartYear <> 0 Then
                ValiditaInizio = New Date(pars.StartYear, 1, 1, 0, 0, 0)
            End If
            Dim ValiditaFine = AGRODATAFINE
            If pars.EndYear <> 0 Then
                ValiditaFine = New Date(pars.EndYear, 12, 31, 23, 59, 59)
            End If


            Dim dt = dir_dal.LeggiRiepilogoDomanda(pars.elencoPiva, ValiditaInizio, ValiditaFine, "", "", objParametri)
            For Each row In dt.Rows
                ret.Add(New AgronicaCoreDTOStd.InData.DomandaIrrigua.RiepilogoDomandeIrrigue() With {
                            .id = row("id"),
                            .piva = row("piva"),
                            .pivaReale = row("PivaReale"),
                            .anno = row("anno"),
                            .ragionesociale = IIf(row("rag_soc") Is DBNull.Value, "", row("rag_soc")),
                            .superficietotale = IIf(row("sup_tot") Is DBNull.Value, 0, row("sup_tot"))
                        })
            Next

        Catch ex As Exception
            Throw New Exception(ex.Message, ex.InnerException)
        End Try
        Return ret
    End Function

End Class
Public Class DomandaIrrigua_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function ScriviAggiornaDomandaIrrigua(ByRef data As AgronicaCoreDTOStd.InData.DomandaIrrigua.DomandaIrrigua,
                                                 ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean
        Dim ret As Boolean = False
        Dim nomeRoutine As String = "AgronicaCoreDomandaIrriguaBIZ.DomandaIrrigua_W.ScriviAggiornaDomandaIrrigua()"

        Dim messaggioErrore As String = ""
        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False
        Try


            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale,
                                                                        FlagTransazioneLocale,
                                                                        objParametri)
            Dim dih_dal As New AgronicaCoreDomandaIrriguaDAL.DomandaIrrigua_Testata_W
            Dim dir_dal As New AgronicaCoreDomandaIrriguaDAL.DomandaIrrigua_Dettaglio_W

            If data.id <= 0 Then
                'creazione

                Dim AgroSequenze As New AgronicaCoreDataProvider.Agro_Sequenze
                'Lavez - 12/07/2024 - normalizzazione chiamate a stack counter
                Dim NewIdDomanda = AgroSequenze.NuovoId_Tabella("DomandaIrrigua_Testata", 0, AgronicaCoreDataProvider.CostantiPersonalizzate.UpperBoundTabelle_Per_SequenzaTabelle_Topcode, objParametri)
                'Dim NewIdDomanda = AgroSequenze.Agronica_SequenzaTabelle_NuovoID(
                '    "DomandaIrrigua_Testata",
                '    objParametri
                ')

                If NewIdDomanda = -1 Then
                    Throw New Exception("Errore in stack contatore domanda irrigua")
                Else
                    data.id = NewIdDomanda
                End If


                Dim retH = dih_dal.Scrivi(data.id,
                                data.piva,
                                data.n_protocollo,
                                data.data_protocollo,
                                data.TipoContratto,
                                1,
                                data.ValiditaInizio,
                                data.ValiditaFine,
                                objParametri)
                If retH = False Then
                    Throw New Exception("Errore in scrittura testata domanda. operazione interrotta")
                End If


                Dim rowId As Integer = 1
                For Each row In data.dettaglio
                    Dim retD = dir_dal.Scrivi(data.id,
                                    rowId,
                                    row.piva,
                                    row.sa_cod,
                                    row.appezza,
                                    row.id_reg,
                                    Convert.ToInt32(row.Selezionato),
                                    AGRODATAINIZIO,
                                    AGRODATAFINE,
                                    objParametri)
                    If retD = False Then
                        Throw New Exception(String.Format("Errore in scrittura riga {0} domanda. operazione interrotta", rowId))
                    Else
                        rowId += 1
                    End If
                Next
            Else
                'aggiornamento
                Dim retH = dih_dal.Modifica(data.id,
                                data.n_protocollo,
                                data.data_protocollo,
                                data.TipoContratto,
                                1,
                                "",
                                objParametri)
                If retH = False Then
                    Throw New Exception("Errore in aggiornamento testata domanda. operazione interrotta")
                End If



                Dim rowId As Integer = data.dettaglio.Max(Function(x) x.riga) + 1
                For Each row In data.dettaglio
                    If row.riga = -1 Then
                        row.riga = rowId
                        Dim retD = dir_dal.Scrivi(data.id,
                                    row.riga,
                                    row.piva,
                                    row.sa_cod,
                                    row.appezza,
                                    row.id_reg,
                                    row.Selezionato,
                                    AGRODATAINIZIO,
                                    AGRODATAFINE,
                                    objParametri)
                        If retD = False Then
                            Throw New Exception(String.Format("Errore in scrittura riga {0} domanda. operazione interrotta", rowId))
                        Else

                            rowId += 1
                        End If
                    Else
                        Dim retD = dir_dal.Modifica(data.id,
                                                    row.riga,
                                                    row.piva,
                                                    row.sa_cod,
                                                    row.appezza,
                                                    row.id_reg,
                                                    row.Selezionato,
                                                    AGRODATAINIZIO,
                                                    AGRODATAFINE,
                                                    objParametri)
                        If retD = False Then
                            Throw New Exception(String.Format("Errore in aggiornamento riga {0} domanda. operazione interrotta", row.riga))
                        End If
                    End If
                Next

            End If

            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri)
            ret = True
        Catch ex As Exception
            ret = False

            'Faccio il rollback della transazione
            If Not objParametri.objTransazione Is Nothing Then
                'objParametri.objTransazione.Rollback()
                'objParametri.objTransazione = Nothing
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri)
            End If

            '//////////////////////////////////////////////////////////////////////
            messaggioErrore = ex.Message
            '//////////////////////////////////////////////////////////////////////

            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)

            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try
        Return ret
    End Function

End Class

