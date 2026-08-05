Imports System.Data.Entity
Imports System.Text
Imports System.Transactions
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO

Public Class VivaiPassaporti_Operazioni_R
    Inherits AgronicaCoreDataProvider.DataProvider


    Public Function LeggiEFSingolo(
                ByVal ws_VivaiPassaporti_Operazione_cod As Integer,
                ByRef objParametri As AgronicaCoreParametri
        ) As ws_VivaiPassaporti_Operazioni

        Dim Elem As ws_VivaiPassaporti_Operazioni = Nothing

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        '----- Descrizione
        Dim NomeRoutine As String = "_R.Leggi()"

        Dim gefutils As New Gias_EF_Utility

        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            Elem =
           (From m In GiasContext.ws_VivaiPassaporti_Operazioni
            Where m.ws_VivaiPassaporti_Operazione_cod = ws_VivaiPassaporti_Operazione_cod
            Select m).FirstOrDefault()

        End Using

        Return Elem


    End Function


    Public Function LeggiPerStampaLibera(
        ByVal TipoZona As String,
        ByVal xFiltroAggiuntivo As String,
        ByVal xOrderBy As String,
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
    ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCore_DAL.Leggi()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Stb.Length = 0
            'i18n
            Stb.AppendLine("select  ")
            Stb.AppendLine("  - 1 as ws_VivaiPassaporti_Operazione_cod ")
            Stb.AppendLine(", 1 as Qta1 ")
            Stb.AppendLine(", 'PP' as TipoZona ")
            Stb.AppendLine(", cast(getDate() as date) as DataMovimento")
            Stb.AppendLine(", '' as Lotto_1")
            Stb.AppendLine(", '' as Veg_Des_Lat")
            Stb.AppendLine(", '' as Causale")
            Stb.AppendLine(", '' as CaricoScarico")
            Stb.AppendLine(", '' as Origine")
            Stb.AppendLine(", '' as Destinazione")



            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT





    End Function


    Public Function LogCompilato(
        ByVal xFiltroAggiuntivo As String,
        ByVal xOrderBy As String,
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
    ) As Boolean

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCore_DAL.Leggi()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Stb.Length = 0


            Stb.AppendLine(" select count(*) as conteggio")
            Stb.AppendLine(" from ws_VivaiPassaporti_Operazioni_log l ")

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        If DT.Rows(0)("conteggio") = 0 Then
            Return False
        Else
            Return True
        End If





    End Function


    Public Function LeggiLog(
        ByVal xFiltroAggiuntivo As String,
        ByVal xOrderBy As String,
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
    ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCore_DAL.Leggi()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Stb.Length = 0


            Stb.AppendLine(" select l.* ")
            Stb.AppendLine(" from ws_VivaiPassaporti_Operazioni_log l ")
            Stb.AppendLine(" where ")

            Stb.Append(Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))

            'TENERE COMMENTATO: nel log il flag visiblità è usato per altri scopi 

            '--------------------------------------------------------------------------
            'Select Case objParametri.FlagVisibilita
            '    Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
            '        Stb.Append(" AND   l.Inviato >=0 ")
            '    Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
            '        Stb.Append(" AND   l.Inviato =-1 ")
            '    Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
            '        '...................................
            '    Case Else
            '        Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            'End Select
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                Stb.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT





    End Function

    Public Function LeggiLogElementiConStessaOperazioneAgendaCodDiverso(
            ByVal ws_VivaiPassaporti_Operazione_cod As Integer,
            ByVal xFiltroAggiuntivo As String,
            ByVal xOrderBy As String,
            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
        ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCore_DAL.Leggi()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Stb.Length = 0

            Stb.AppendLine(" select l2.* ")
            Stb.AppendLine(" from [ws_VivaiPassaporti_Operazioni_log] l1 ")
            Stb.AppendLine("  inner join  [ws_VivaiPassaporti_Operazioni_log] l2 ")
            Stb.AppendLine("  on  l1.Doc_in_mov_dettagli_piva = l2.Doc_in_mov_dettagli_piva ")
            Stb.AppendLine("  and l1.Doc_in_mov_dettagli_id_agenda = l2.Doc_in_mov_dettagli_id_agenda ")
            Stb.AppendLine("  and l1.Doc_in_mov_dettagli_id_mov = l2.Doc_in_mov_dettagli_id_mov ")
            Stb.AppendLine("  and l1.Doc_in_mov_dettagli_id_mov_det = l2.Doc_in_mov_dettagli_id_mov_det  ")
            Stb.AppendLine("  and l1.ws_VivaiPassaporti_Operazione_cod <> l2.ws_VivaiPassaporti_Operazione_cod ")
            Stb.AppendLine("  and l1.ws_VivaiPassaporti_Operazione_cod = " & ws_VivaiPassaporti_Operazione_cod)


            If xFiltroAggiuntivo <> "" Then
                Stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            'TENERE COMMENTATO: nel log il flag visiblità è usato per altri scopi 

            '--------------------------------------------------------------------------
            'Select Case objParametri.FlagVisibilita
            '    Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
            '        Stb.Append(" AND   l.Inviato >=0 ")
            '    Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
            '        Stb.Append(" AND   l.Inviato =-1 ")
            '    Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
            '        '...................................
            '    Case Else
            '        Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            'End Select
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                Stb.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT





    End Function
    Public Function LeggiLog(
        ByVal ws_VivaiPassaporti_Operazione_cod As Integer,
        ByVal xFiltroAggiuntivo As String,
        ByVal xOrderBy As String,
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
    ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCore_DAL.Leggi()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Stb.Length = 0


            Stb.AppendLine(" select l2.* ")
            Stb.AppendLine(" from ws_VivaiPassaporti_Operazioni_log l ")
            Stb.AppendLine("  inner join ws_VivaiPassaporti_Operazioni_log l2 ")
            Stb.AppendLine("      on l2.codiceRiga = l.codiceRiga ")
            Stb.AppendLine(" where l.ws_VivaiPassaporti_Operazione_cod =  " & ws_VivaiPassaporti_Operazione_cod)

            If xFiltroAggiuntivo <> "" Then
                Stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            'TENERE COMMENTATO: nel log il flag visiblità è usato per altri scopi 

            '--------------------------------------------------------------------------
            'Select Case objParametri.FlagVisibilita
            '    Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
            '        Stb.Append(" AND   l.Inviato >=0 ")
            '    Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
            '        Stb.Append(" AND   l.Inviato =-1 ")
            '    Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
            '        '...................................
            '    Case Else
            '        Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            'End Select
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                Stb.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT





    End Function


    Public Function Leggi(
        ByVal piva As String,
        ByVal dataDa As Date,
        ByVal dataA As Date,
        ByVal xFiltroAggiuntivo As String,
        ByVal xOrderBy As String,
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
    ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCore_DAL.Leggi()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Stb.Length = 0

            Stb.AppendLine(" Select ")
            Stb.AppendLine("      op.[ws_VivaiPassaporti_Operazione_cod] ")
            Stb.AppendLine("    , op.[GIAS_Stato] ")
            Stb.AppendLine("    , op.[Vivaista_PIVA] ")
            Stb.AppendLine("    , op.[Rif_Distinta_Piva] ")
            Stb.AppendLine("    , op.[Rif_Distinta_Progetto_Cod] ")
            Stb.AppendLine("    , op.[Codice_RUOP] ")
            Stb.AppendLine("    , op.[TipoZona] ")
            Stb.AppendLine("    , op.[DDT_in_Numero] ")
            Stb.AppendLine("    , op.[DDT_out_Numero] ")
            Stb.AppendLine("    , op.[Doc_in_mov_dettagli_piva] ")
            Stb.AppendLine("    , op.[Doc_in_mov_dettagli_sa_cod] ")
            Stb.AppendLine("    , op.[Doc_in_mov_dettagli_id_agenda] ")
            Stb.AppendLine("    , op.[Doc_in_mov_dettagli_id_mov] ")
            Stb.AppendLine("    , op.[Doc_in_mov_dettagli_id_mov_det] ")
            Stb.AppendLine("    , op.[Doc_out_mov_dettagli_piva] ")
            Stb.AppendLine("    , op.[Doc_out_mov_dettagli_sa_cod] ")
            Stb.AppendLine("    , op.[Doc_out_mov_dettagli_id_agenda] ")
            Stb.AppendLine("    , op.[Doc_out_mov_dettagli_id_mov] ")
            Stb.AppendLine("    , op.[Doc_out_mov_dettagli_id_mov_det] ")
            Stb.AppendLine("    , op.[DataMovimento] ")
            Stb.AppendLine("    , op.[PaeseDiOrigine] ")
            Stb.AppendLine("    , op.[Causale] ")
            Stb.AppendLine("    , op.[CaricoScarico] ")
            Stb.AppendLine("    , op.[Origine] ")
            Stb.AppendLine("    , op.[Destinazione] ")
            Stb.AppendLine("    , op.[FornitoreAPO] ")
            Stb.AppendLine("    , op.[Cul_COD] ")
            Stb.AppendLine("    , op.[Grfri_Cod] ")
            Stb.AppendLine("    , op.[Grva_Cod] ")
            Stb.AppendLine("    , op.[elem_cod] ")
            Stb.AppendLine("    , op.[mat_cod] ")
            Stb.AppendLine("    , op.[cal_cod] ")
            Stb.AppendLine("    , op.[Veg_Des_Lat] ")
            Stb.AppendLine("    , op.[cul_Des] ")
            Stb.AppendLine("    , op.[Prodotto_Des] ")
            Stb.AppendLine("    , op.[Qta1] ")
            Stb.AppendLine("    , op.[Qta2] ")
            Stb.AppendLine("    , op.[Qta3] ")
            Stb.AppendLine("    , op.[Calibro] ")
            ' VAnni: 3/4/2020: come sempre tutto Hardcoded... il codice BO004 sarà da gestire.
            ' VAnni: 15/6/2020: ancora più hardcoded .. il codice BO004 vale solo sulla piva di zespri (02125421202).
            Stb.AppendLine("    , case when op.[Vivaista_PIVA] = '02125421202' then case when op.[Lotto_1] like 'BO004%' then + op.[Lotto_1]   else 'BO004 ' + op.[Lotto_1] end else op.[Lotto_1] end as Lotto_1 ")
            Stb.AppendLine("    , op.[Lotto_2] ")
            Stb.AppendLine("    , op.[Coltivatore_codRisum] ")
            Stb.AppendLine("    , op.[Coltivatore_RagioneSociale] ")
            Stb.AppendLine("    , op.[Coltivatore_Indirizzo] ")
            Stb.AppendLine("    , op.[Coltivatore_Cap] ")
            Stb.AppendLine("    , op.[Coltivatore_Citta] ")
            Stb.AppendLine("    , op.[Coltivatore_Regione] ")
            Stb.AppendLine("    , op.[DestinazioneFinale_codRisum] ")
            Stb.AppendLine("    , op.[DestinazioneFinale_RagioneSociale] ")
            Stb.AppendLine("    , op.[DestinazioneFinale_Indirizzo] ")
            Stb.AppendLine("    , op.[DestinazioneFinale_Cap] ")
            Stb.AppendLine("    , op.[DestinazioneFinale_Citta] ")
            Stb.AppendLine("    , op.[DestinazioneFinale_Regione] ")
            Stb.AppendLine("    , op.[Preso_in_cosegna_Da] ")
            Stb.AppendLine("    , op.[Note] ")
            Stb.AppendLine("    , op.[inviato] ")
            Stb.AppendLine("    , op.[datainvio] ")
            Stb.AppendLine("    , op.[Data_Creazione] ")
            Stb.AppendLine("    , op.[Data_Modifica] ")
            Stb.AppendLine("    , op.[Username_Creazione] ")
            Stb.AppendLine("    , op.[Username_Modifica] ")
            Stb.AppendLine("    , op.[Validita_Inizio] ")
            Stb.AppendLine("    , op.[Validita_Fine] ")
            Stb.AppendLine("    , op.[Rag_Soc_RUOP] ")
            Stb.AppendLine("    , op.[SitoCodice] ")
            Stb.AppendLine("    , op.[SitoRagioneSociale]")

            Stb.AppendLine("    , isnull(l.azione, '') as azione ")
            Stb.AppendLine("    , isNull(l.codiceRiga, 0) as CodiceRiga ")
            Stb.AppendLine("    , wStati.WAnagraficaStati_DES ")
            Stb.AppendLine("    , wStati.Colore as statoColore ")
            Stb.AppendLine("    , case when tipoZona = 'PP' then 'Zona Non protetta (PP)' else 'Zona protetta (ZP)' end as TipoZonaDes ")
            Stb.AppendLine("    , causale as CausaleDes ")
            Stb.AppendLine("    , caricoScarico as CaricoScaricoDes ")


            Stb.AppendLine(" From ws_VivaiPassaporti_Operazioni op")
            Stb.AppendLine(" inner join  WAnagraficaStati wStati ")
            Stb.AppendLine(" on wStati.WAnagraficaStati_Cod = op.GIAS_Stato ")
            Stb.AppendLine(" ")
            Stb.AppendLine("  left join ws_VivaiPassaporti_Operazioni_log l ")
            Stb.AppendLine("  on op.[ws_VivaiPassaporti_Operazione_cod] =  l.[ws_VivaiPassaporti_Operazione_cod] ")
            Stb.AppendLine("  and op.[Doc_in_mov_dettagli_piva] = l.[Doc_in_mov_dettagli_piva] ")
            Stb.AppendLine("  and op.[Doc_in_mov_dettagli_id_agenda]  = l.[Doc_in_mov_dettagli_id_agenda] ")
            Stb.AppendLine("  and op.[Doc_in_mov_dettagli_id_mov]  = l.[Doc_in_mov_dettagli_id_mov]  ")
            Stb.AppendLine("  and op.[Doc_in_mov_dettagli_id_mov_det] = l.[Doc_in_mov_dettagli_id_mov_det]")

            Stb.AppendLine(" Where op.vivaista_piva = '" & Agro_SQL_SaveText(piva) & "' ")
            Stb.AppendLine(" And cast( op.DataMovimento as date) >= " & Agro_SQL_SaveDate(dataDa))
            Stb.AppendLine(" And cast( op.DataMovimento as date) <=  " & Agro_SQL_SaveDate(dataA))

            Stb.AppendLine(" ")



            If xFiltroAggiuntivo <> "" Then
                Stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    Stb.Append(" AND   op.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    Stb.Append(" AND   op.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                Stb.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT





    End Function


End Class




Public Class VivaiPassaporti_Operazioni_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Enum VivaiComportamentoFlginviato

        InserisciRigheConGestitaDel = -8
        RimuoviRigheAggiunteSoloRegistroReimpostaStatoDel = -7
        ReimpostaStatoBlankSuTabellaLog = -6
        ReimpostaStatoDelSuTabellaLog = -5
        ReimpostaStatoMovRegSuTabellaLog = -4
        RimuoviRigheAggiunteSoloRegistro = -2
        RimuoviRigheAggiunteSuRegistroELog = -3

    End Enum


    Public Function Aggiorna_ws_VivaiPassaporti_Operazioni_W(
                ByVal EFArrayToInsert As ArrayList,
                ByVal EFArrayToUpdate As ArrayList,
                ByVal EFArrayToDelete As ArrayList,
                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                ) As String

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        Dim ObjSequenze = New AgronicaCoreDataProvider.Agro_Sequenze

        Dim retries As Integer = 3
        Dim success As Boolean = True

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.AgronicaCoreMetaschemaDAL.Aggiorna_NomeDal()"
        Dim MessaggioErrore As String = ""

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Dim dal As AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities = Nothing

        Try

            Using scope As New TransactionScope(TransactionScopeOption.RequiresNew)

                Try

                    dal = New Gias_DeveloperServer_Entities(EFConnString)

                    'dal.ContextOptions.UseLegacyPreserveChangesBehavior = False
                    'Open the contextObject connection state explicitly
                    dal.Database.Connection.Open()

                    Dim idSeq As Integer = 0

                    For Each curNomeDal As ws_VivaiPassaporti_Operazioni In EFArrayToInsert

                        success = False
                        For i As Integer = 0 To retries - 1

                            Try

                                'Richiedo un nuovo id sequenza
                                idSeq = ObjSequenze.NuovoId_Tabella_EF(dal,
                                               "ws_VivaiPassaporti_Operazioni", 0, 2000000000, objParametri)

                                curNomeDal.ws_VivaiPassaporti_Operazione_cod = idSeq
                                dal.ws_VivaiPassaporti_Operazioni.Add(curNomeDal)
                                dal.SaveChanges()
                                success = True

                                Exit For
                            Catch ex As Exception
                                Threading.Thread.Sleep(500) ' 500 milliseconds = 0.5 seconds
                            End Try
                        Next
                        ' Al primo errore evito di continuare le modifiche
                        If Not success Then
                            MessaggioErrore = "Non sono riuscito ad aggiornare i dati dopo " & retries & " tentativi."
                            Exit For
                        End If
                    Next

                    If success Then
                        For Each listFattVar As ws_VivaiPassaporti_Operazioni In EFArrayToUpdate
                            dal.ws_VivaiPassaporti_Operazioni.Attach(listFattVar)
                            dal.Entry(listFattVar).State = EntityState.Modified
                            dal.SaveChanges()
                        Next

                        For Each listFattVar As ws_VivaiPassaporti_Operazioni In EFArrayToDelete
                            dal.ws_VivaiPassaporti_Operazioni.Attach(listFattVar)
                            dal.ws_VivaiPassaporti_Operazioni.Remove(listFattVar)
                            dal.SaveChanges()
                        Next

                        'EF6: AcceptAllChanges (Accepts the changes on all associated entries in the ObjectStateManager so their resultant state is either unchanged or detached.
                        '                       This method iterates all the ObjectStateEntry objects within the ObjectStateManager that are Added or Modified, and then sets the state of the entry to Unchanged. The Deleted items become detached.)
                        'Qui avrebbe senso fare questa operazione solo se è stato disabilitato il comportamento di default del SaveChanges(che internamente chiama AcceptAllChanges)
                        'ma visto che è stato chiamato il SaveChanges default, non ha senso farla perché SaveChanges() == SaveChanges(SaveOptions.DetectChangesBeforeSave | SaveOptions.AcceptAllChangesAfterSave)
                        'dal.AcceptAllChanges()

                        For Each listFattVar As ws_VivaiPassaporti_Operazioni In EFArrayToUpdate

                            Dim rvalDel As Boolean
                            Try

                                rvalDel = DocumentiGiasAssegnaAzione(listFattVar.ws_VivaiPassaporti_Operazione_cod, "MODREG", "", objParametri)

                            Catch ex As Exception
                                rvalDel = False
                                Throw New Exception("Fallito aggiornamento mapping registro [cod=" & listFattVar.ws_VivaiPassaporti_Operazione_cod & "] : " & ex.Message)
                            End Try

                        Next

                        For Each listFattVar As ws_VivaiPassaporti_Operazioni In EFArrayToDelete

                            Dim rvalDel As Boolean
                            Try

                                rvalDel = DocumentiGiasAssegnaAzione(listFattVar.ws_VivaiPassaporti_Operazione_cod, "DEL", "", objParametri)

                            Catch ex As Exception
                                rvalDel = False
                                Throw New Exception("Fallito aggiornamento mapping registro [cod=" & listFattVar.ws_VivaiPassaporti_Operazione_cod & "] : " & ex.Message)
                            End Try

                            If Not rvalDel Then
                                Throw New Exception("Fallita scrittura mapping registro.")
                            End If

                            Try

                                rvalDel = DocumentiGiasAssegnaAzioneInCascata(listFattVar.ws_VivaiPassaporti_Operazione_cod, "", "", "  l2.azione <> 'DEL' ", objParametri)

                            Catch ex As Exception
                                rvalDel = False
                                Throw New Exception("Fallito aggiornamento in cascata registro [cod=" & listFattVar.ws_VivaiPassaporti_Operazione_cod & "] : " & ex.Message)
                            End Try

                            If Not rvalDel Then
                                Throw New Exception("Fallita scrittura in cascata registro.")
                            End If

                        Next 'lista eliminati

                        ' COMIT Effettivo
                        scope.Complete()
                    End If

                Catch ex As Exception

                    scope.Dispose()          'Questo fa il rollback di tutto
                    Throw New Exception("", ex)

                Finally
                    'Close the opened connection
                    If dal IsNot Nothing AndAlso dal.Database.Connection.State = ConnectionState.Open Then
                        dal.Database.Connection.Close()
                    End If

                End Try

            End Using


        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore, ex)

        End Try

        Return MessaggioErrore

    End Function

    Private Enum TipoMovimentazioneRegistro_Documento

        DDT_Raccolta = 1
        DDT_Trasferimento = 2
        DDT_ConsegnaFinale = 3

        Conferimento = 4

        LavorazioneCarico = 5
        LavorazioneScarico = 6

        TrasferimentoCarico = 7
        TrasferimentoScarico = 8

    End Enum

    Private Enum TipoColonnaRegistro
        CaricoScarico = 1
        Causale = 2
        Destinazione = 3
        QtaBUDS = 4
        Dest_RagioneSociale = 5
        Dest_Indirizzo = 6
        Dest_CAP = 7
        Dest_Citta = 8
        Dest_Regione = 9
        DestFin_RagioneSociale = 10
        DestFin_Indirizzo = 11
        DestFin_CAP = 12
        DestFin_Citta = 13
        DestFin_Regione = 14
        SitoCodice = 20
        SitoRagioneSociale = 21
        Origine_Departure = 22
    End Enum

    Private Sub RiportaDocumentiGiasQryTipoMov_Doc(stb As StringBuilder, colRegistro As TipoColonnaRegistro, tipoDoc As TipoMovimentazioneRegistro_Documento)

        Dim isCisterna As String = " detProd.piva = '02125421202' and detProd.sa_cod = 132775937 "


        Select Case colRegistro

            Case TipoColonnaRegistro.CaricoScarico

                Select Case tipoDoc
                    Case TipoMovimentazioneRegistro_Documento.TrasferimentoCarico
                        stb.AppendLine("        'Loading' ")
                    Case TipoMovimentazioneRegistro_Documento.TrasferimentoScarico
                        stb.AppendLine("        'Discharge' ")
                    Case TipoMovimentazioneRegistro_Documento.DDT_Raccolta
                        stb.AppendLine("        'Transport' ")
                    Case TipoMovimentazioneRegistro_Documento.DDT_Trasferimento
                        stb.AppendLine("        'Transport' ")
                    Case TipoMovimentazioneRegistro_Documento.DDT_ConsegnaFinale
                        stb.AppendLine("        'Delivery' ")
                    Case TipoMovimentazioneRegistro_Documento.Conferimento
                        stb.AppendLine("        'Loading' ")
                    Case TipoMovimentazioneRegistro_Documento.LavorazioneCarico
                        stb.AppendLine("        'Loading' ")
                    Case TipoMovimentazioneRegistro_Documento.LavorazioneScarico
                        stb.AppendLine("        'Discharge' ")
                End Select 'ok

            Case TipoColonnaRegistro.Causale
                Select Case tipoDoc
                    Case TipoMovimentazioneRegistro_Documento.TrasferimentoCarico
                        stb.AppendLine("        'Bud Transfer' ")
                    Case TipoMovimentazioneRegistro_Documento.TrasferimentoScarico
                        stb.AppendLine("        'Bud Transfer' ")
                    Case TipoMovimentazioneRegistro_Documento.DDT_Raccolta
                        stb.AppendLine("        'Bin Transfer' ")
                    Case TipoMovimentazioneRegistro_Documento.DDT_Trasferimento
                        stb.AppendLine("        'Bud Transfer' ")
                    Case TipoMovimentazioneRegistro_Documento.DDT_ConsegnaFinale
                        stb.AppendLine("        'Bud Allocation' ")
                    Case TipoMovimentazioneRegistro_Documento.Conferimento
                        stb.AppendLine("        'WH Load' ")
                    Case TipoMovimentazioneRegistro_Documento.LavorazioneCarico
                        stb.AppendLine("        'Bud Processing' ")
                    Case TipoMovimentazioneRegistro_Documento.LavorazioneScarico
                        stb.AppendLine("        'Bud Processing' ")

                End Select

            Case TipoColonnaRegistro.Destinazione

                Select Case tipoDoc

                    Case TipoMovimentazioneRegistro_Documento.TrasferimentoCarico
                        stb.AppendLine("  sa.sa_nome  ")
                    Case TipoMovimentazioneRegistro_Documento.TrasferimentoScarico
                        stb.AppendLine("  Trasf_sa_2.sa_nome ")
                    Case TipoMovimentazioneRegistro_Documento.DDT_Raccolta
                        stb.AppendLine(" 'WH Scanzano' ")
                    Case TipoMovimentazioneRegistro_Documento.DDT_Trasferimento
                        stb.AppendLine(" 'WH Cisterna' ")
                    Case TipoMovimentazioneRegistro_Documento.DDT_ConsegnaFinale
                        stb.AppendLine(" 'Grower' ")
                    Case TipoMovimentazioneRegistro_Documento.Conferimento
                        stb.AppendLine(" sa.sa_nome ")
                    Case TipoMovimentazioneRegistro_Documento.LavorazioneCarico
                        stb.AppendLine(" sa.sa_nome ")
                    Case TipoMovimentazioneRegistro_Documento.LavorazioneScarico
                        stb.AppendLine(" sa.sa_nome ")

                End Select 'ok

            Case TipoColonnaRegistro.QtaBUDS

                Select Case tipoDoc
                    Case TipoMovimentazioneRegistro_Documento.TrasferimentoCarico
                        stb.AppendLine("    detProd.Qta ")
                    Case TipoMovimentazioneRegistro_Documento.TrasferimentoScarico
                        stb.AppendLine("  - detProd.Qta ")
                    Case TipoMovimentazioneRegistro_Documento.DDT_Raccolta
                        stb.AppendLine("  - detProd.Qta   ")
                    Case TipoMovimentazioneRegistro_Documento.DDT_Trasferimento
                        stb.AppendLine("  - detProd.Qta   ")
                    Case TipoMovimentazioneRegistro_Documento.DDT_ConsegnaFinale
                        stb.AppendLine("   - detProd.Qta   ")
                    Case TipoMovimentazioneRegistro_Documento.Conferimento
                        stb.AppendLine("     detProd.Qta    ")
                    Case TipoMovimentazioneRegistro_Documento.LavorazioneCarico
                        stb.AppendLine("     detProd.Qta   ")
                    Case TipoMovimentazioneRegistro_Documento.LavorazioneScarico
                        stb.AppendLine("  - detProd.Qta  ")
                End Select 'ok

            Case TipoColonnaRegistro.Dest_RagioneSociale

                Select Case tipoDoc
                    Case TipoMovimentazioneRegistro_Documento.TrasferimentoCarico
                        stb.AppendLine("  sa.sa_nome  ")
                    Case TipoMovimentazioneRegistro_Documento.TrasferimentoScarico
                        stb.AppendLine("  Trasf_sa_2.sa_nome ")
                    Case TipoMovimentazioneRegistro_Documento.DDT_Raccolta
                        stb.AppendLine(" isNull(conDestinazione.Rag_Soc, con.Rag_Soc) ")
                    Case TipoMovimentazioneRegistro_Documento.DDT_Trasferimento
                        stb.AppendLine(" con.Rag_Soc")
                    Case TipoMovimentazioneRegistro_Documento.DDT_ConsegnaFinale
                        stb.AppendLine(" con.Rag_Soc")
                    Case TipoMovimentazioneRegistro_Documento.Conferimento
                        stb.AppendLine(" sa.sa_Nome")
                    Case TipoMovimentazioneRegistro_Documento.LavorazioneCarico
                        stb.AppendLine("  sa.sa_Nome  ")
                    Case TipoMovimentazioneRegistro_Documento.LavorazioneScarico
                        stb.AppendLine("  sa.sa_Nome  ")
                End Select

            Case TipoColonnaRegistro.DestFin_RagioneSociale

                Select Case tipoDoc
                    Case TipoMovimentazioneRegistro_Documento.TrasferimentoCarico
                        stb.AppendLine(" ''  ")
                    Case TipoMovimentazioneRegistro_Documento.TrasferimentoScarico
                        stb.AppendLine(" ''  ")
                    Case TipoMovimentazioneRegistro_Documento.DDT_Raccolta
                        stb.AppendLine(" ''  ")
                    Case TipoMovimentazioneRegistro_Documento.DDT_Trasferimento
                        stb.AppendLine(" ''  ")
                    Case TipoMovimentazioneRegistro_Documento.DDT_ConsegnaFinale
                        stb.AppendLine(" isNull(conDestinazione.Rag_Soc, '')  ")
                    Case TipoMovimentazioneRegistro_Documento.Conferimento
                        stb.AppendLine(" ''  ")
                    Case TipoMovimentazioneRegistro_Documento.LavorazioneCarico
                        stb.AppendLine(" ''  ")
                    Case TipoMovimentazioneRegistro_Documento.LavorazioneScarico
                        stb.AppendLine(" ''  ")
                End Select

            Case TipoColonnaRegistro.Dest_Indirizzo

                'Via della Curva 6, Borgo Baensizza 04012 Cisterna di Latina (LT)

                Select Case tipoDoc
                    Case TipoMovimentazioneRegistro_Documento.TrasferimentoCarico
                        stb.AppendLine(" saii.ind_des  ")
                    Case TipoMovimentazioneRegistro_Documento.TrasferimentoScarico
                        stb.AppendLine(" saiiTrasf_sa_2.ind_des ")
                    Case TipoMovimentazioneRegistro_Documento.DDT_Raccolta
                        stb.AppendLine(" case when ii.ind_des is null then iiDestinazione.ind_des + ' ' + iiDestinazione.frz_des + ' ' + iiDestinazione.Cap + ' ' + iiDestinazione.Com_Des + ' (' + iiDestinazione.pro_Cod + ')' else ii.ind_des + ' ' + ii.frz_des + ' '  + ' ' + ii.Com_Des + ' (' + ii.pro_Cod + ')' end ")
                    Case TipoMovimentazioneRegistro_Documento.DDT_Trasferimento
                        stb.AppendLine(" ii.ind_des + ' ' + ii.frz_des + ' '  + ii.cap +  ' ' + ii.Com_Des + ' (' + ii.pro_Cod + ')' ")
                    Case TipoMovimentazioneRegistro_Documento.DDT_ConsegnaFinale
                        stb.AppendLine(" ii.ind_des")
                    Case TipoMovimentazioneRegistro_Documento.Conferimento
                        stb.AppendLine(" saii.ind_des")
                    Case TipoMovimentazioneRegistro_Documento.LavorazioneCarico
                        stb.AppendLine(" saii.ind_des")
                    Case TipoMovimentazioneRegistro_Documento.LavorazioneScarico
                        stb.AppendLine(" saii.ind_des")
                End Select


            Case TipoColonnaRegistro.DestFin_Indirizzo

                'DestinazioneFinale_ind_des: Via della Curva 6, Borgo Baensizza 04012 Cisterna di Latina (LT)

                Select Case tipoDoc
                    Case TipoMovimentazioneRegistro_Documento.TrasferimentoCarico
                        stb.AppendLine(" '' ")
                    Case TipoMovimentazioneRegistro_Documento.TrasferimentoScarico
                        stb.AppendLine(" '' ")
                    Case TipoMovimentazioneRegistro_Documento.DDT_Raccolta
                        stb.AppendLine(" '' ")
                    Case TipoMovimentazioneRegistro_Documento.DDT_Trasferimento
                        stb.AppendLine(" '' ")
                    Case TipoMovimentazioneRegistro_Documento.DDT_ConsegnaFinale
                        stb.AppendLine(" case when iiDestinazione.ind_des is null then '' else iiDestinazione.ind_des + ' ' + iiDestinazione.frz_des + ' ' + iiDestinazione.Cap + ' ' + iiDestinazione.Com_Des + ' (' + iiDestinazione.pro_Cod + ')' end ")
                    Case TipoMovimentazioneRegistro_Documento.Conferimento
                        stb.AppendLine(" '' ")
                    Case TipoMovimentazioneRegistro_Documento.LavorazioneCarico
                        stb.AppendLine(" '' ")
                    Case TipoMovimentazioneRegistro_Documento.LavorazioneScarico
                        stb.AppendLine(" '' ")
                End Select

            Case TipoColonnaRegistro.Dest_CAP

                Select Case tipoDoc
                    Case TipoMovimentazioneRegistro_Documento.TrasferimentoCarico
                        stb.AppendLine(" saii.cap  ")
                    Case TipoMovimentazioneRegistro_Documento.TrasferimentoScarico
                        stb.AppendLine(" saiiTrasf_sa_2.cap ")
                    Case TipoMovimentazioneRegistro_Documento.DDT_Raccolta
                        stb.AppendLine(" isNull(iiDestinazione.cap, ii.cap) ")
                    Case TipoMovimentazioneRegistro_Documento.DDT_Trasferimento
                        stb.AppendLine(" ii.cap")
                    Case TipoMovimentazioneRegistro_Documento.DDT_ConsegnaFinale
                        stb.AppendLine(" ii.cap")
                    Case TipoMovimentazioneRegistro_Documento.Conferimento
                        stb.AppendLine(" saii.cap")
                    Case TipoMovimentazioneRegistro_Documento.LavorazioneCarico
                        stb.AppendLine(" saii.cap")
                    Case TipoMovimentazioneRegistro_Documento.LavorazioneScarico
                        stb.AppendLine(" saii.cap")
                End Select

            Case TipoColonnaRegistro.DestFin_CAP

                Select Case tipoDoc
                    Case TipoMovimentazioneRegistro_Documento.TrasferimentoCarico
                        stb.AppendLine(" '' ")
                    Case TipoMovimentazioneRegistro_Documento.TrasferimentoScarico
                        stb.AppendLine(" '' ")
                    Case TipoMovimentazioneRegistro_Documento.DDT_Raccolta
                        stb.AppendLine(" '' ")
                    Case TipoMovimentazioneRegistro_Documento.DDT_Trasferimento
                        stb.AppendLine(" '' ")
                    Case TipoMovimentazioneRegistro_Documento.DDT_ConsegnaFinale
                        stb.AppendLine(" isNull(iiDestinazione.cap, '') ")
                    Case TipoMovimentazioneRegistro_Documento.Conferimento
                        stb.AppendLine(" '' ")
                    Case TipoMovimentazioneRegistro_Documento.LavorazioneCarico
                        stb.AppendLine(" '' ")
                    Case TipoMovimentazioneRegistro_Documento.LavorazioneScarico
                        stb.AppendLine(" '' ")
                End Select

            Case TipoColonnaRegistro.Dest_Citta

                Select Case tipoDoc
                    Case TipoMovimentazioneRegistro_Documento.TrasferimentoCarico
                        stb.AppendLine(" saii.frz_des  ")
                    Case TipoMovimentazioneRegistro_Documento.TrasferimentoScarico
                        stb.AppendLine(" saiiTrasf_sa_2.frz_des ")
                    Case TipoMovimentazioneRegistro_Documento.DDT_Raccolta
                        stb.AppendLine(" isNull(iiDestinazione.frz_Des, ii.frz_Des) ")
                    Case TipoMovimentazioneRegistro_Documento.DDT_Trasferimento
                        stb.AppendLine(" ii.frz_Des")
                    Case TipoMovimentazioneRegistro_Documento.DDT_ConsegnaFinale
                        stb.AppendLine(" ii.frz_Des")
                    Case TipoMovimentazioneRegistro_Documento.Conferimento
                        stb.AppendLine(" saii.frz_Des")
                    Case TipoMovimentazioneRegistro_Documento.LavorazioneCarico
                        stb.AppendLine(" saii.frz_Des")
                    Case TipoMovimentazioneRegistro_Documento.LavorazioneScarico
                        stb.AppendLine(" saii.frz_Des")
                End Select

            Case TipoColonnaRegistro.DestFin_Citta

                Select Case tipoDoc
                    Case TipoMovimentazioneRegistro_Documento.TrasferimentoCarico
                        stb.AppendLine(" '' ")
                    Case TipoMovimentazioneRegistro_Documento.TrasferimentoScarico
                        stb.AppendLine(" '' ")
                    Case TipoMovimentazioneRegistro_Documento.DDT_Raccolta
                        stb.AppendLine(" '' ")
                    Case TipoMovimentazioneRegistro_Documento.DDT_Trasferimento
                        stb.AppendLine(" '' ")
                    Case TipoMovimentazioneRegistro_Documento.DDT_ConsegnaFinale
                        stb.AppendLine(" isNull(iiDestinazione.frz_Des, '') ")
                    Case TipoMovimentazioneRegistro_Documento.Conferimento
                        stb.AppendLine(" '' ")
                    Case TipoMovimentazioneRegistro_Documento.LavorazioneCarico
                        stb.AppendLine(" '' ")
                    Case TipoMovimentazioneRegistro_Documento.LavorazioneScarico
                        stb.AppendLine(" '' ")
                End Select

            Case TipoColonnaRegistro.Dest_Regione

                Select Case tipoDoc
                    Case TipoMovimentazioneRegistro_Documento.TrasferimentoCarico
                        stb.AppendLine(" '' ")
                    Case TipoMovimentazioneRegistro_Documento.TrasferimentoScarico
                        stb.AppendLine(" '' ")
                    Case TipoMovimentazioneRegistro_Documento.DDT_Raccolta
                        stb.AppendLine(" isNull(lr.Regione_Des, lrDestinazione.Regione_Des) ")
                    Case TipoMovimentazioneRegistro_Documento.DDT_Trasferimento
                        stb.AppendLine(" lr.Regione_Des ")
                    Case TipoMovimentazioneRegistro_Documento.DDT_ConsegnaFinale
                        stb.AppendLine(" lr.Regione_Des ")
                    Case TipoMovimentazioneRegistro_Documento.Conferimento
                        stb.AppendLine(" '' ")
                    Case TipoMovimentazioneRegistro_Documento.LavorazioneCarico
                        stb.AppendLine(" '' ")
                    Case TipoMovimentazioneRegistro_Documento.LavorazioneScarico
                        stb.AppendLine(" '' ")
                End Select

            Case TipoColonnaRegistro.DestFin_Regione

                Select Case tipoDoc
                    Case TipoMovimentazioneRegistro_Documento.TrasferimentoCarico
                        stb.AppendLine(" '' ")
                    Case TipoMovimentazioneRegistro_Documento.TrasferimentoScarico
                        stb.AppendLine(" '' ")
                    Case TipoMovimentazioneRegistro_Documento.DDT_Raccolta
                        stb.AppendLine(" '' ")
                    Case TipoMovimentazioneRegistro_Documento.DDT_Trasferimento
                        stb.AppendLine(" '' ")
                    Case TipoMovimentazioneRegistro_Documento.DDT_ConsegnaFinale
                        stb.AppendLine(" isNull(lrDestinazione.Regione_Des, '') ")
                    Case TipoMovimentazioneRegistro_Documento.Conferimento
                        stb.AppendLine(" '' ")
                    Case TipoMovimentazioneRegistro_Documento.LavorazioneCarico
                        stb.AppendLine(" ''  ")
                    Case TipoMovimentazioneRegistro_Documento.LavorazioneScarico
                        stb.AppendLine(" ''  ")
                End Select

            Case TipoColonnaRegistro.SitoCodice
                Select Case tipoDoc
                    Case TipoMovimentazioneRegistro_Documento.TrasferimentoCarico
                        stb.AppendLine(" '' ")
                    Case TipoMovimentazioneRegistro_Documento.TrasferimentoScarico
                        stb.AppendLine(" '' ")
                    Case TipoMovimentazioneRegistro_Documento.DDT_Raccolta
                        stb.AppendLine(" sac.val_cod")
                    Case TipoMovimentazioneRegistro_Documento.DDT_Trasferimento
                        stb.AppendLine(" ''")
                    Case TipoMovimentazioneRegistro_Documento.DDT_ConsegnaFinale
                        stb.AppendLine(" ''")
                    Case TipoMovimentazioneRegistro_Documento.Conferimento
                        stb.AppendLine(" ''")
                    Case TipoMovimentazioneRegistro_Documento.LavorazioneCarico
                        stb.AppendLine(" ''")
                    Case TipoMovimentazioneRegistro_Documento.LavorazioneScarico
                        stb.AppendLine(" ''")
                End Select

            Case TipoColonnaRegistro.SitoRagioneSociale

                Select Case tipoDoc
                    Case TipoMovimentazioneRegistro_Documento.TrasferimentoCarico
                        stb.AppendLine(" '' ")
                    Case TipoMovimentazioneRegistro_Documento.TrasferimentoScarico
                        stb.AppendLine(" '' ")
                    Case TipoMovimentazioneRegistro_Documento.DDT_Raccolta
                        stb.AppendLine(" sa.sa_nome")
                    Case TipoMovimentazioneRegistro_Documento.DDT_Trasferimento
                        stb.AppendLine(" ''")
                    Case TipoMovimentazioneRegistro_Documento.DDT_ConsegnaFinale
                        stb.AppendLine(" ''")
                    Case TipoMovimentazioneRegistro_Documento.Conferimento
                        stb.AppendLine(" ''")
                    Case TipoMovimentazioneRegistro_Documento.LavorazioneCarico
                        stb.AppendLine(" ''")
                    Case TipoMovimentazioneRegistro_Documento.LavorazioneScarico
                        stb.AppendLine(" ''")

                End Select

            Case TipoColonnaRegistro.Origine_Departure
                Select Case tipoDoc
                    Case TipoMovimentazioneRegistro_Documento.TrasferimentoCarico
                        stb.AppendLine(" Trasf_sa_2.sa_nome  ")
                    Case TipoMovimentazioneRegistro_Documento.TrasferimentoScarico
                        stb.AppendLine(" sa.sa_nome ")
                    Case TipoMovimentazioneRegistro_Documento.DDT_Raccolta
                        stb.AppendLine(" sa.sa_nome ")
                    Case TipoMovimentazioneRegistro_Documento.DDT_Trasferimento
                        stb.AppendLine(" sa.sa_nome ")
                    Case TipoMovimentazioneRegistro_Documento.DDT_ConsegnaFinale
                        stb.AppendLine(" sa.sa_nome ")
                    Case TipoMovimentazioneRegistro_Documento.Conferimento
                        stb.AppendLine(" sa.sa_nome ")
                    Case TipoMovimentazioneRegistro_Documento.LavorazioneCarico
                        stb.AppendLine(" sa.sa_nome ")
                    Case TipoMovimentazioneRegistro_Documento.LavorazioneScarico
                        stb.AppendLine(" sa.sa_nome ")

                End Select
        End Select

    End Sub

    Private Sub RiportaDocumentiGiasQryTipoMov(ByRef stb As Text.StringBuilder, ByVal tipoMov As TipoColonnaRegistro)

        Dim strAlias As String = ""
        Dim isCisterna As String = " movDest.piva = '02125421202' and movDest.sa_cod = 132775937 "

        Select Case tipoMov
            Case TipoColonnaRegistro.CaricoScarico
                strAlias = "CaricoScarico"
            Case TipoColonnaRegistro.Causale
                strAlias = "Causale"
            Case TipoColonnaRegistro.Destinazione
                strAlias = "Destinazione"
            Case TipoColonnaRegistro.QtaBUDS
                strAlias = "QTA3"
            Case TipoColonnaRegistro.Dest_RagioneSociale
                strAlias = "Dest_RagSoc"
            Case TipoColonnaRegistro.Dest_Indirizzo
                strAlias = "Dest_Indirizzo"
            Case TipoColonnaRegistro.Dest_CAP
                strAlias = "Dest_CAp"
            Case TipoColonnaRegistro.Dest_Regione
                strAlias = "Dest_Regione"
            Case TipoColonnaRegistro.Dest_Citta
                strAlias = "Dest_Citta"
            Case TipoColonnaRegistro.DestFin_RagioneSociale
                strAlias = "DestinazioneFinale_RagSoc"
            Case TipoColonnaRegistro.DestFin_Indirizzo
                strAlias = "DestinazioneFinale_Indirizzo"
            Case TipoColonnaRegistro.DestFin_CAP
                strAlias = "DestinazioneFinale_Cap"
            Case TipoColonnaRegistro.DestFin_Regione
                strAlias = "DestinazioneFinale_Regione"
            Case TipoColonnaRegistro.DestFin_Citta
                strAlias = "DestinazioneFinale_Citta"
            Case TipoColonnaRegistro.SitoCodice
                strAlias = "Sito_Cod"
            Case TipoColonnaRegistro.SitoRagioneSociale
                strAlias = "Sito_RagioneSociale"
            Case TipoColonnaRegistro.Origine_Departure
                strAlias = "Origine"
        End Select

        stb.AppendLine(" , case when a.Lav_Cod = 5000 then  ")
        stb.AppendLine("    case when m.cau_mov = '7300' then ")

        'Lavorazione Carico
        stb.AppendLine("        --Lavorazione Carico ")

        Select Case tipoMov
            Case TipoColonnaRegistro.CaricoScarico
                RiportaDocumentiGiasQryTipoMov_Doc(stb, TipoColonnaRegistro.CaricoScarico, TipoMovimentazioneRegistro_Documento.LavorazioneCarico)

            Case TipoColonnaRegistro.Causale
                RiportaDocumentiGiasQryTipoMov_Doc(stb, TipoColonnaRegistro.Causale, TipoMovimentazioneRegistro_Documento.LavorazioneCarico)

            Case TipoColonnaRegistro.Destinazione
                RiportaDocumentiGiasQryTipoMov_Doc(stb, TipoColonnaRegistro.Destinazione, TipoMovimentazioneRegistro_Documento.LavorazioneCarico)

            Case TipoColonnaRegistro.QtaBUDS
                RiportaDocumentiGiasQryTipoMov_Doc(stb, TipoColonnaRegistro.QtaBUDS, TipoMovimentazioneRegistro_Documento.LavorazioneCarico)

            Case TipoColonnaRegistro.Dest_RagioneSociale
                RiportaDocumentiGiasQryTipoMov_Doc(stb, TipoColonnaRegistro.Dest_RagioneSociale, TipoMovimentazioneRegistro_Documento.LavorazioneCarico)

            Case TipoColonnaRegistro.Dest_Indirizzo
                RiportaDocumentiGiasQryTipoMov_Doc(stb, TipoColonnaRegistro.Dest_Indirizzo, TipoMovimentazioneRegistro_Documento.LavorazioneCarico)

            Case TipoColonnaRegistro.Dest_Citta
                RiportaDocumentiGiasQryTipoMov_Doc(stb, TipoColonnaRegistro.Dest_Citta, TipoMovimentazioneRegistro_Documento.LavorazioneCarico)

            Case TipoColonnaRegistro.Dest_CAP
                RiportaDocumentiGiasQryTipoMov_Doc(stb, TipoColonnaRegistro.Dest_CAP, TipoMovimentazioneRegistro_Documento.LavorazioneCarico)

            Case TipoColonnaRegistro.Dest_Regione
                RiportaDocumentiGiasQryTipoMov_Doc(stb, TipoColonnaRegistro.Dest_Regione, TipoMovimentazioneRegistro_Documento.LavorazioneCarico)

            Case TipoColonnaRegistro.DestFin_RagioneSociale
                RiportaDocumentiGiasQryTipoMov_Doc(stb, TipoColonnaRegistro.DestFin_RagioneSociale, TipoMovimentazioneRegistro_Documento.LavorazioneCarico)

            Case TipoColonnaRegistro.DestFin_Indirizzo
                RiportaDocumentiGiasQryTipoMov_Doc(stb, TipoColonnaRegistro.DestFin_Indirizzo, TipoMovimentazioneRegistro_Documento.LavorazioneCarico)

            Case TipoColonnaRegistro.DestFin_Citta
                RiportaDocumentiGiasQryTipoMov_Doc(stb, TipoColonnaRegistro.DestFin_Citta, TipoMovimentazioneRegistro_Documento.LavorazioneCarico)

            Case TipoColonnaRegistro.DestFin_CAP
                RiportaDocumentiGiasQryTipoMov_Doc(stb, TipoColonnaRegistro.DestFin_CAP, TipoMovimentazioneRegistro_Documento.LavorazioneCarico)

            Case TipoColonnaRegistro.DestFin_Regione
                RiportaDocumentiGiasQryTipoMov_Doc(stb, TipoColonnaRegistro.DestFin_Regione, TipoMovimentazioneRegistro_Documento.LavorazioneCarico)

            Case TipoColonnaRegistro.SitoCodice
                RiportaDocumentiGiasQryTipoMov_Doc(stb, TipoColonnaRegistro.SitoCodice, TipoMovimentazioneRegistro_Documento.LavorazioneCarico)

            Case TipoColonnaRegistro.SitoRagioneSociale
                RiportaDocumentiGiasQryTipoMov_Doc(stb, TipoColonnaRegistro.SitoRagioneSociale, TipoMovimentazioneRegistro_Documento.LavorazioneCarico)

            Case TipoColonnaRegistro.Origine_Departure
                RiportaDocumentiGiasQryTipoMov_Doc(stb, TipoColonnaRegistro.Origine_Departure, TipoMovimentazioneRegistro_Documento.LavorazioneCarico)


        End Select



        'Select Case tipoMov
        '    Case TipoColonnaRegistro.CaricoScarico
        '        stb.AppendLine("        'Loading' ")


        '    Case TipoColonnaRegistro.Causale
        '        stb.AppendLine("        'Bud Processing' ")


        '    Case TipoColonnaRegistro.Destinazione
        '        stb.AppendLine(" case when " & isCisterna & " then 'WH Cisterna' else 'WH Scanzano' end ")


        '    Case TipoColonnaRegistro.QtaBUDS
        '        stb.AppendLine("  detProd.Qta    ")  'buds

        '    Case TipoColonnaRegistro.Dest_RagioneSociale
        '        stb.AppendLine("  ''  ")
        'End Select

        stb.AppendLine("    else  ")

        'Lavorazione Scarico
        stb.AppendLine("        --Lavorazione Scarico ")


        Select Case tipoMov
            Case TipoColonnaRegistro.CaricoScarico
                RiportaDocumentiGiasQryTipoMov_Doc(stb, TipoColonnaRegistro.CaricoScarico, TipoMovimentazioneRegistro_Documento.LavorazioneScarico)

            Case TipoColonnaRegistro.Causale
                RiportaDocumentiGiasQryTipoMov_Doc(stb, TipoColonnaRegistro.Causale, TipoMovimentazioneRegistro_Documento.LavorazioneScarico)

            Case TipoColonnaRegistro.Destinazione
                RiportaDocumentiGiasQryTipoMov_Doc(stb, TipoColonnaRegistro.Destinazione, TipoMovimentazioneRegistro_Documento.LavorazioneScarico)

            Case TipoColonnaRegistro.QtaBUDS
                RiportaDocumentiGiasQryTipoMov_Doc(stb, TipoColonnaRegistro.QtaBUDS, TipoMovimentazioneRegistro_Documento.LavorazioneScarico)

            Case TipoColonnaRegistro.Dest_RagioneSociale
                RiportaDocumentiGiasQryTipoMov_Doc(stb, TipoColonnaRegistro.Dest_RagioneSociale, TipoMovimentazioneRegistro_Documento.LavorazioneScarico)

            Case TipoColonnaRegistro.Dest_Indirizzo
                RiportaDocumentiGiasQryTipoMov_Doc(stb, TipoColonnaRegistro.Dest_Indirizzo, TipoMovimentazioneRegistro_Documento.LavorazioneScarico)

            Case TipoColonnaRegistro.Dest_Citta
                RiportaDocumentiGiasQryTipoMov_Doc(stb, TipoColonnaRegistro.Dest_Citta, TipoMovimentazioneRegistro_Documento.LavorazioneScarico)

            Case TipoColonnaRegistro.Dest_CAP
                RiportaDocumentiGiasQryTipoMov_Doc(stb, TipoColonnaRegistro.Dest_CAP, TipoMovimentazioneRegistro_Documento.LavorazioneScarico)

            Case TipoColonnaRegistro.Dest_Regione
                RiportaDocumentiGiasQryTipoMov_Doc(stb, TipoColonnaRegistro.Dest_Regione, TipoMovimentazioneRegistro_Documento.LavorazioneScarico)

            Case TipoColonnaRegistro.DestFin_RagioneSociale
                RiportaDocumentiGiasQryTipoMov_Doc(stb, TipoColonnaRegistro.DestFin_RagioneSociale, TipoMovimentazioneRegistro_Documento.LavorazioneScarico)

            Case TipoColonnaRegistro.DestFin_Indirizzo
                RiportaDocumentiGiasQryTipoMov_Doc(stb, TipoColonnaRegistro.DestFin_Indirizzo, TipoMovimentazioneRegistro_Documento.LavorazioneScarico)

            Case TipoColonnaRegistro.DestFin_Citta
                RiportaDocumentiGiasQryTipoMov_Doc(stb, TipoColonnaRegistro.DestFin_Citta, TipoMovimentazioneRegistro_Documento.LavorazioneScarico)

            Case TipoColonnaRegistro.DestFin_CAP
                RiportaDocumentiGiasQryTipoMov_Doc(stb, TipoColonnaRegistro.DestFin_CAP, TipoMovimentazioneRegistro_Documento.LavorazioneScarico)

            Case TipoColonnaRegistro.DestFin_Regione
                RiportaDocumentiGiasQryTipoMov_Doc(stb, TipoColonnaRegistro.DestFin_Regione, TipoMovimentazioneRegistro_Documento.LavorazioneScarico)

            Case TipoColonnaRegistro.SitoCodice
                RiportaDocumentiGiasQryTipoMov_Doc(stb, TipoColonnaRegistro.SitoCodice, TipoMovimentazioneRegistro_Documento.LavorazioneScarico)

            Case TipoColonnaRegistro.SitoRagioneSociale
                RiportaDocumentiGiasQryTipoMov_Doc(stb, TipoColonnaRegistro.SitoRagioneSociale, TipoMovimentazioneRegistro_Documento.LavorazioneScarico)

            Case TipoColonnaRegistro.Origine_Departure
                RiportaDocumentiGiasQryTipoMov_Doc(stb, TipoColonnaRegistro.Origine_Departure, TipoMovimentazioneRegistro_Documento.LavorazioneScarico)

        End Select



        'Select Case tipoMov
        '    Case TipoColonnaRegistro.CaricoScarico
        '        stb.AppendLine("        'Discharge' ")

        '    Case TipoColonnaRegistro.Causale
        '        stb.AppendLine("        'Bud Processing' ")

        '    Case TipoColonnaRegistro.Destinazione
        '        stb.AppendLine(" case when " & isCisterna & " then 'WH Cisterna' else 'WH Scanzano' end ")
        '        strAlias = "Destinazione"

        '    Case TipoColonnaRegistro.QtaBUDS
        '        stb.AppendLine("   - detProd.Qta  ")  'buds

        '    Case TipoColonnaRegistro.Dest_RagioneSociale
        '        stb.AppendLine("  ''  ")
        'End Select

        stb.AppendLine("    end ")
        stb.AppendLine("   else ")
        stb.AppendLine("  case when a.lav_cod in (1031, 1033) then ")
        stb.AppendLine("      case when mtes.causale_trasporto in ('ASSEGNAZIONE MATERIALE VEGETALE DI TERZI',  'CESSIONE GRATUITA') then ")

        'DDT Consegna finale
        stb.AppendLine("         --DDT Consegna finale ")

        Select Case tipoMov
            Case TipoColonnaRegistro.CaricoScarico
                RiportaDocumentiGiasQryTipoMov_Doc(stb, TipoColonnaRegistro.CaricoScarico, TipoMovimentazioneRegistro_Documento.DDT_ConsegnaFinale)

            Case TipoColonnaRegistro.Causale
                RiportaDocumentiGiasQryTipoMov_Doc(stb, TipoColonnaRegistro.Causale, TipoMovimentazioneRegistro_Documento.DDT_ConsegnaFinale)

            Case TipoColonnaRegistro.Destinazione
                RiportaDocumentiGiasQryTipoMov_Doc(stb, TipoColonnaRegistro.Destinazione, TipoMovimentazioneRegistro_Documento.DDT_ConsegnaFinale)

            Case TipoColonnaRegistro.QtaBUDS
                RiportaDocumentiGiasQryTipoMov_Doc(stb, TipoColonnaRegistro.QtaBUDS, TipoMovimentazioneRegistro_Documento.DDT_ConsegnaFinale)

            Case TipoColonnaRegistro.Dest_RagioneSociale
                RiportaDocumentiGiasQryTipoMov_Doc(stb, TipoColonnaRegistro.Dest_RagioneSociale, TipoMovimentazioneRegistro_Documento.DDT_ConsegnaFinale)

            Case TipoColonnaRegistro.Dest_Indirizzo
                RiportaDocumentiGiasQryTipoMov_Doc(stb, TipoColonnaRegistro.Dest_Indirizzo, TipoMovimentazioneRegistro_Documento.DDT_ConsegnaFinale)

            Case TipoColonnaRegistro.Dest_Citta
                RiportaDocumentiGiasQryTipoMov_Doc(stb, TipoColonnaRegistro.Dest_Citta, TipoMovimentazioneRegistro_Documento.DDT_ConsegnaFinale)

            Case TipoColonnaRegistro.Dest_CAP
                RiportaDocumentiGiasQryTipoMov_Doc(stb, TipoColonnaRegistro.Dest_CAP, TipoMovimentazioneRegistro_Documento.DDT_ConsegnaFinale)

            Case TipoColonnaRegistro.Dest_Regione
                RiportaDocumentiGiasQryTipoMov_Doc(stb, TipoColonnaRegistro.Dest_Regione, TipoMovimentazioneRegistro_Documento.DDT_ConsegnaFinale)

            Case TipoColonnaRegistro.DestFin_RagioneSociale
                RiportaDocumentiGiasQryTipoMov_Doc(stb, TipoColonnaRegistro.DestFin_RagioneSociale, TipoMovimentazioneRegistro_Documento.DDT_ConsegnaFinale)

            Case TipoColonnaRegistro.DestFin_Indirizzo
                RiportaDocumentiGiasQryTipoMov_Doc(stb, TipoColonnaRegistro.DestFin_Indirizzo, TipoMovimentazioneRegistro_Documento.DDT_ConsegnaFinale)

            Case TipoColonnaRegistro.DestFin_Citta
                RiportaDocumentiGiasQryTipoMov_Doc(stb, TipoColonnaRegistro.DestFin_Citta, TipoMovimentazioneRegistro_Documento.DDT_ConsegnaFinale)

            Case TipoColonnaRegistro.DestFin_CAP
                RiportaDocumentiGiasQryTipoMov_Doc(stb, TipoColonnaRegistro.DestFin_CAP, TipoMovimentazioneRegistro_Documento.DDT_ConsegnaFinale)

            Case TipoColonnaRegistro.DestFin_Regione
                RiportaDocumentiGiasQryTipoMov_Doc(stb, TipoColonnaRegistro.DestFin_Regione, TipoMovimentazioneRegistro_Documento.DDT_ConsegnaFinale)

            Case TipoColonnaRegistro.SitoCodice
                RiportaDocumentiGiasQryTipoMov_Doc(stb, TipoColonnaRegistro.SitoCodice, TipoMovimentazioneRegistro_Documento.DDT_ConsegnaFinale)

            Case TipoColonnaRegistro.SitoRagioneSociale
                RiportaDocumentiGiasQryTipoMov_Doc(stb, TipoColonnaRegistro.SitoRagioneSociale, TipoMovimentazioneRegistro_Documento.DDT_ConsegnaFinale)

            Case TipoColonnaRegistro.Origine_Departure
                RiportaDocumentiGiasQryTipoMov_Doc(stb, TipoColonnaRegistro.Origine_Departure, TipoMovimentazioneRegistro_Documento.DDT_ConsegnaFinale)

        End Select


        'Select Case tipoMov
        '    Case TipoColonnaRegistro.CaricoScarico
        '        stb.AppendLine("        'Delivery' ")

        '    Case TipoColonnaRegistro.Causale
        '        stb.AppendLine("        'Bud Allocation' ")

        '    Case TipoColonnaRegistro.Destinazione
        '        stb.AppendLine(" 'Grower' ")

        '    Case TipoColonnaRegistro.QtaBUDS
        '        stb.AppendLine("   - detProd.Qta   ")  'buds

        '    Case TipoColonnaRegistro.Dest_RagioneSociale
        '        stb.AppendLine("   con.rag_soc   ")
        'End Select

        stb.AppendLine("      else ")
        stb.AppendLine("         case when mtes.causale_trasporto = 'CONTO LAVORAZIONE' then ")

        'DDT Raccolta
        stb.AppendLine("              --DDT Raccolta ")

        Select Case tipoMov
            Case TipoColonnaRegistro.CaricoScarico
                RiportaDocumentiGiasQryTipoMov_Doc(stb, TipoColonnaRegistro.CaricoScarico, TipoMovimentazioneRegistro_Documento.DDT_Raccolta)

            Case TipoColonnaRegistro.Causale
                RiportaDocumentiGiasQryTipoMov_Doc(stb, TipoColonnaRegistro.Causale, TipoMovimentazioneRegistro_Documento.DDT_Raccolta)

            Case TipoColonnaRegistro.Destinazione
                RiportaDocumentiGiasQryTipoMov_Doc(stb, TipoColonnaRegistro.Destinazione, TipoMovimentazioneRegistro_Documento.DDT_Raccolta)

            Case TipoColonnaRegistro.QtaBUDS
                RiportaDocumentiGiasQryTipoMov_Doc(stb, TipoColonnaRegistro.QtaBUDS, TipoMovimentazioneRegistro_Documento.DDT_Raccolta)

            Case TipoColonnaRegistro.Dest_RagioneSociale
                RiportaDocumentiGiasQryTipoMov_Doc(stb, TipoColonnaRegistro.Dest_RagioneSociale, TipoMovimentazioneRegistro_Documento.DDT_Raccolta)

            Case TipoColonnaRegistro.Dest_Indirizzo
                RiportaDocumentiGiasQryTipoMov_Doc(stb, TipoColonnaRegistro.Dest_Indirizzo, TipoMovimentazioneRegistro_Documento.DDT_Raccolta)

            Case TipoColonnaRegistro.Dest_Citta
                RiportaDocumentiGiasQryTipoMov_Doc(stb, TipoColonnaRegistro.Dest_Citta, TipoMovimentazioneRegistro_Documento.DDT_Raccolta)

            Case TipoColonnaRegistro.Dest_CAP
                RiportaDocumentiGiasQryTipoMov_Doc(stb, TipoColonnaRegistro.Dest_CAP, TipoMovimentazioneRegistro_Documento.DDT_Raccolta)

            Case TipoColonnaRegistro.Dest_Regione
                RiportaDocumentiGiasQryTipoMov_Doc(stb, TipoColonnaRegistro.Dest_Regione, TipoMovimentazioneRegistro_Documento.DDT_Raccolta)

            Case TipoColonnaRegistro.DestFin_RagioneSociale
                RiportaDocumentiGiasQryTipoMov_Doc(stb, TipoColonnaRegistro.DestFin_RagioneSociale, TipoMovimentazioneRegistro_Documento.DDT_Raccolta)

            Case TipoColonnaRegistro.DestFin_Indirizzo
                RiportaDocumentiGiasQryTipoMov_Doc(stb, TipoColonnaRegistro.DestFin_Indirizzo, TipoMovimentazioneRegistro_Documento.DDT_Raccolta)

            Case TipoColonnaRegistro.DestFin_Citta
                RiportaDocumentiGiasQryTipoMov_Doc(stb, TipoColonnaRegistro.DestFin_Citta, TipoMovimentazioneRegistro_Documento.DDT_Raccolta)

            Case TipoColonnaRegistro.DestFin_CAP
                RiportaDocumentiGiasQryTipoMov_Doc(stb, TipoColonnaRegistro.DestFin_CAP, TipoMovimentazioneRegistro_Documento.DDT_Raccolta)

            Case TipoColonnaRegistro.DestFin_Regione
                RiportaDocumentiGiasQryTipoMov_Doc(stb, TipoColonnaRegistro.DestFin_Regione, TipoMovimentazioneRegistro_Documento.DDT_Raccolta)

            Case TipoColonnaRegistro.SitoCodice
                RiportaDocumentiGiasQryTipoMov_Doc(stb, TipoColonnaRegistro.SitoCodice, TipoMovimentazioneRegistro_Documento.DDT_Raccolta)

            Case TipoColonnaRegistro.SitoRagioneSociale
                RiportaDocumentiGiasQryTipoMov_Doc(stb, TipoColonnaRegistro.SitoRagioneSociale, TipoMovimentazioneRegistro_Documento.DDT_Raccolta)

            Case TipoColonnaRegistro.Origine_Departure
                RiportaDocumentiGiasQryTipoMov_Doc(stb, TipoColonnaRegistro.Origine_Departure, TipoMovimentazioneRegistro_Documento.DDT_Raccolta)


        End Select



        'Select Case tipoMov
        '    Case TipoColonnaRegistro.CaricoScarico
        '        stb.AppendLine("        'Transport' ")

        '    Case TipoColonnaRegistro.Causale
        '        stb.AppendLine("        'Bin Transfer' ")

        '    Case TipoColonnaRegistro.Destinazione
        '        stb.AppendLine(" 'WH Scanzano' ")

        '    Case TipoColonnaRegistro.QtaBUDS
        '        stb.AppendLine("   - detProd.Qta   ")  'buds

        '    Case TipoColonnaRegistro.Dest_RagioneSociale
        '        stb.AppendLine("   con.rag_soc   ")   'TODO

        '    Case TipoColonnaRegistro.SitoCodice
        '        'il codice in centri_Aziendali_codici

        '    Case TipoColonnaRegistro.SitoRagioneSociale
        '        'il codice in centri_Aziendali_codici

        'End Select

        stb.AppendLine("         else ")


        stb.AppendLine("         case when a.lav_cod = 1033 then ")
        stb.AppendLine("         case when m.cau_mov = '7300' then ")

        Select Case tipoMov
            Case TipoColonnaRegistro.CaricoScarico
                RiportaDocumentiGiasQryTipoMov_Doc(stb, TipoColonnaRegistro.CaricoScarico, TipoMovimentazioneRegistro_Documento.TrasferimentoCarico)

            Case TipoColonnaRegistro.Causale
                RiportaDocumentiGiasQryTipoMov_Doc(stb, TipoColonnaRegistro.Causale, TipoMovimentazioneRegistro_Documento.TrasferimentoCarico)

            Case TipoColonnaRegistro.Destinazione
                RiportaDocumentiGiasQryTipoMov_Doc(stb, TipoColonnaRegistro.Destinazione, TipoMovimentazioneRegistro_Documento.TrasferimentoCarico)

            Case TipoColonnaRegistro.QtaBUDS
                RiportaDocumentiGiasQryTipoMov_Doc(stb, TipoColonnaRegistro.QtaBUDS, TipoMovimentazioneRegistro_Documento.TrasferimentoCarico)

            Case TipoColonnaRegistro.Dest_RagioneSociale
                RiportaDocumentiGiasQryTipoMov_Doc(stb, TipoColonnaRegistro.Dest_RagioneSociale, TipoMovimentazioneRegistro_Documento.TrasferimentoCarico)

            Case TipoColonnaRegistro.Dest_Indirizzo
                RiportaDocumentiGiasQryTipoMov_Doc(stb, TipoColonnaRegistro.Dest_Indirizzo, TipoMovimentazioneRegistro_Documento.TrasferimentoCarico)

            Case TipoColonnaRegistro.Dest_Citta
                RiportaDocumentiGiasQryTipoMov_Doc(stb, TipoColonnaRegistro.Dest_Citta, TipoMovimentazioneRegistro_Documento.TrasferimentoCarico)

            Case TipoColonnaRegistro.Dest_CAP
                RiportaDocumentiGiasQryTipoMov_Doc(stb, TipoColonnaRegistro.Dest_CAP, TipoMovimentazioneRegistro_Documento.TrasferimentoCarico)

            Case TipoColonnaRegistro.Dest_Regione
                RiportaDocumentiGiasQryTipoMov_Doc(stb, TipoColonnaRegistro.Dest_Regione, TipoMovimentazioneRegistro_Documento.TrasferimentoCarico)

            Case TipoColonnaRegistro.DestFin_RagioneSociale
                RiportaDocumentiGiasQryTipoMov_Doc(stb, TipoColonnaRegistro.DestFin_RagioneSociale, TipoMovimentazioneRegistro_Documento.TrasferimentoCarico)

            Case TipoColonnaRegistro.DestFin_Indirizzo
                RiportaDocumentiGiasQryTipoMov_Doc(stb, TipoColonnaRegistro.DestFin_Indirizzo, TipoMovimentazioneRegistro_Documento.TrasferimentoCarico)

            Case TipoColonnaRegistro.DestFin_Citta
                RiportaDocumentiGiasQryTipoMov_Doc(stb, TipoColonnaRegistro.DestFin_Citta, TipoMovimentazioneRegistro_Documento.TrasferimentoCarico)

            Case TipoColonnaRegistro.DestFin_CAP
                RiportaDocumentiGiasQryTipoMov_Doc(stb, TipoColonnaRegistro.DestFin_CAP, TipoMovimentazioneRegistro_Documento.TrasferimentoCarico)

            Case TipoColonnaRegistro.DestFin_Regione
                RiportaDocumentiGiasQryTipoMov_Doc(stb, TipoColonnaRegistro.DestFin_Regione, TipoMovimentazioneRegistro_Documento.TrasferimentoCarico)

            Case TipoColonnaRegistro.SitoCodice
                RiportaDocumentiGiasQryTipoMov_Doc(stb, TipoColonnaRegistro.SitoCodice, TipoMovimentazioneRegistro_Documento.TrasferimentoCarico)

            Case TipoColonnaRegistro.SitoRagioneSociale
                RiportaDocumentiGiasQryTipoMov_Doc(stb, TipoColonnaRegistro.SitoRagioneSociale, TipoMovimentazioneRegistro_Documento.TrasferimentoCarico)

            Case TipoColonnaRegistro.Origine_Departure
                RiportaDocumentiGiasQryTipoMov_Doc(stb, TipoColonnaRegistro.Origine_Departure, TipoMovimentazioneRegistro_Documento.TrasferimentoCarico)

        End Select


        stb.AppendLine("         else ")

        Select Case tipoMov
            Case TipoColonnaRegistro.CaricoScarico
                RiportaDocumentiGiasQryTipoMov_Doc(stb, TipoColonnaRegistro.CaricoScarico, TipoMovimentazioneRegistro_Documento.TrasferimentoScarico)

            Case TipoColonnaRegistro.Causale
                RiportaDocumentiGiasQryTipoMov_Doc(stb, TipoColonnaRegistro.Causale, TipoMovimentazioneRegistro_Documento.TrasferimentoScarico)

            Case TipoColonnaRegistro.Destinazione
                RiportaDocumentiGiasQryTipoMov_Doc(stb, TipoColonnaRegistro.Destinazione, TipoMovimentazioneRegistro_Documento.TrasferimentoScarico)

            Case TipoColonnaRegistro.QtaBUDS
                RiportaDocumentiGiasQryTipoMov_Doc(stb, TipoColonnaRegistro.QtaBUDS, TipoMovimentazioneRegistro_Documento.TrasferimentoScarico)

            Case TipoColonnaRegistro.Dest_RagioneSociale
                RiportaDocumentiGiasQryTipoMov_Doc(stb, TipoColonnaRegistro.Dest_RagioneSociale, TipoMovimentazioneRegistro_Documento.TrasferimentoScarico)

            Case TipoColonnaRegistro.Dest_Indirizzo
                RiportaDocumentiGiasQryTipoMov_Doc(stb, TipoColonnaRegistro.Dest_Indirizzo, TipoMovimentazioneRegistro_Documento.TrasferimentoScarico)

            Case TipoColonnaRegistro.Dest_Citta
                RiportaDocumentiGiasQryTipoMov_Doc(stb, TipoColonnaRegistro.Dest_Citta, TipoMovimentazioneRegistro_Documento.TrasferimentoScarico)

            Case TipoColonnaRegistro.Dest_CAP
                RiportaDocumentiGiasQryTipoMov_Doc(stb, TipoColonnaRegistro.Dest_CAP, TipoMovimentazioneRegistro_Documento.TrasferimentoScarico)

            Case TipoColonnaRegistro.Dest_Regione
                RiportaDocumentiGiasQryTipoMov_Doc(stb, TipoColonnaRegistro.Dest_Regione, TipoMovimentazioneRegistro_Documento.TrasferimentoScarico)

            Case TipoColonnaRegistro.DestFin_RagioneSociale
                RiportaDocumentiGiasQryTipoMov_Doc(stb, TipoColonnaRegistro.DestFin_RagioneSociale, TipoMovimentazioneRegistro_Documento.TrasferimentoScarico)

            Case TipoColonnaRegistro.DestFin_Indirizzo
                RiportaDocumentiGiasQryTipoMov_Doc(stb, TipoColonnaRegistro.DestFin_Indirizzo, TipoMovimentazioneRegistro_Documento.TrasferimentoScarico)

            Case TipoColonnaRegistro.DestFin_Citta
                RiportaDocumentiGiasQryTipoMov_Doc(stb, TipoColonnaRegistro.DestFin_Citta, TipoMovimentazioneRegistro_Documento.TrasferimentoScarico)

            Case TipoColonnaRegistro.DestFin_CAP
                RiportaDocumentiGiasQryTipoMov_Doc(stb, TipoColonnaRegistro.DestFin_CAP, TipoMovimentazioneRegistro_Documento.TrasferimentoScarico)

            Case TipoColonnaRegistro.DestFin_Regione
                RiportaDocumentiGiasQryTipoMov_Doc(stb, TipoColonnaRegistro.DestFin_Regione, TipoMovimentazioneRegistro_Documento.TrasferimentoScarico)

            Case TipoColonnaRegistro.SitoCodice
                RiportaDocumentiGiasQryTipoMov_Doc(stb, TipoColonnaRegistro.SitoCodice, TipoMovimentazioneRegistro_Documento.TrasferimentoScarico)

            Case TipoColonnaRegistro.SitoRagioneSociale
                RiportaDocumentiGiasQryTipoMov_Doc(stb, TipoColonnaRegistro.SitoRagioneSociale, TipoMovimentazioneRegistro_Documento.TrasferimentoScarico)

            Case TipoColonnaRegistro.Origine_Departure
                RiportaDocumentiGiasQryTipoMov_Doc(stb, TipoColonnaRegistro.Origine_Departure, TipoMovimentazioneRegistro_Documento.TrasferimentoScarico)

        End Select
        stb.AppendLine("         end ")

        stb.AppendLine("         else ")
        'DDT Trasferimento
        stb.AppendLine("              --DDT Trasferimento  ")

        Select Case tipoMov
            Case TipoColonnaRegistro.CaricoScarico
                RiportaDocumentiGiasQryTipoMov_Doc(stb, TipoColonnaRegistro.CaricoScarico, TipoMovimentazioneRegistro_Documento.DDT_Trasferimento)

            Case TipoColonnaRegistro.Causale
                RiportaDocumentiGiasQryTipoMov_Doc(stb, TipoColonnaRegistro.Causale, TipoMovimentazioneRegistro_Documento.DDT_Trasferimento)

            Case TipoColonnaRegistro.Destinazione
                RiportaDocumentiGiasQryTipoMov_Doc(stb, TipoColonnaRegistro.Destinazione, TipoMovimentazioneRegistro_Documento.DDT_Trasferimento)

            Case TipoColonnaRegistro.QtaBUDS
                RiportaDocumentiGiasQryTipoMov_Doc(stb, TipoColonnaRegistro.QtaBUDS, TipoMovimentazioneRegistro_Documento.DDT_Trasferimento)

            Case TipoColonnaRegistro.Dest_RagioneSociale
                RiportaDocumentiGiasQryTipoMov_Doc(stb, TipoColonnaRegistro.Dest_RagioneSociale, TipoMovimentazioneRegistro_Documento.DDT_Trasferimento)

            Case TipoColonnaRegistro.Dest_Indirizzo
                RiportaDocumentiGiasQryTipoMov_Doc(stb, TipoColonnaRegistro.Dest_Indirizzo, TipoMovimentazioneRegistro_Documento.DDT_Trasferimento)

            Case TipoColonnaRegistro.Dest_Citta
                RiportaDocumentiGiasQryTipoMov_Doc(stb, TipoColonnaRegistro.Dest_Citta, TipoMovimentazioneRegistro_Documento.DDT_Trasferimento)

            Case TipoColonnaRegistro.Dest_CAP
                RiportaDocumentiGiasQryTipoMov_Doc(stb, TipoColonnaRegistro.Dest_CAP, TipoMovimentazioneRegistro_Documento.DDT_Trasferimento)

            Case TipoColonnaRegistro.Dest_Regione
                RiportaDocumentiGiasQryTipoMov_Doc(stb, TipoColonnaRegistro.Dest_Regione, TipoMovimentazioneRegistro_Documento.DDT_Trasferimento)

            Case TipoColonnaRegistro.DestFin_RagioneSociale
                RiportaDocumentiGiasQryTipoMov_Doc(stb, TipoColonnaRegistro.DestFin_RagioneSociale, TipoMovimentazioneRegistro_Documento.DDT_Trasferimento)

            Case TipoColonnaRegistro.DestFin_Indirizzo
                RiportaDocumentiGiasQryTipoMov_Doc(stb, TipoColonnaRegistro.DestFin_Indirizzo, TipoMovimentazioneRegistro_Documento.DDT_Trasferimento)

            Case TipoColonnaRegistro.DestFin_Citta
                RiportaDocumentiGiasQryTipoMov_Doc(stb, TipoColonnaRegistro.DestFin_Citta, TipoMovimentazioneRegistro_Documento.DDT_Trasferimento)

            Case TipoColonnaRegistro.DestFin_CAP
                RiportaDocumentiGiasQryTipoMov_Doc(stb, TipoColonnaRegistro.DestFin_CAP, TipoMovimentazioneRegistro_Documento.DDT_Trasferimento)

            Case TipoColonnaRegistro.DestFin_Regione
                RiportaDocumentiGiasQryTipoMov_Doc(stb, TipoColonnaRegistro.DestFin_Regione, TipoMovimentazioneRegistro_Documento.DDT_Trasferimento)

            Case TipoColonnaRegistro.SitoCodice
                RiportaDocumentiGiasQryTipoMov_Doc(stb, TipoColonnaRegistro.SitoCodice, TipoMovimentazioneRegistro_Documento.DDT_Trasferimento)

            Case TipoColonnaRegistro.SitoRagioneSociale
                RiportaDocumentiGiasQryTipoMov_Doc(stb, TipoColonnaRegistro.SitoRagioneSociale, TipoMovimentazioneRegistro_Documento.DDT_Trasferimento)

            Case TipoColonnaRegistro.Origine_Departure
                RiportaDocumentiGiasQryTipoMov_Doc(stb, TipoColonnaRegistro.Origine_Departure, TipoMovimentazioneRegistro_Documento.DDT_Trasferimento)

        End Select



        'Select Case tipoMov
        '    Case TipoColonnaRegistro.CaricoScarico
        '        stb.AppendLine("        'Transport' ")

        '    Case TipoColonnaRegistro.Causale
        '        stb.AppendLine("        'Bud Transfer' ")

        '    Case TipoColonnaRegistro.Destinazione
        '        stb.AppendLine(" 'WH Latina' ")

        '    Case TipoColonnaRegistro.QtaBUDS
        '        stb.AppendLine("   - detProd.Qta    ")  'buds

        '    Case TipoColonnaRegistro.Dest_RagioneSociale
        '        stb.AppendLine("   con.rag_soc   ")   'TODO


        '    Case TipoColonnaRegistro.SitoCodice
        '        'il codice in centri_Aziendali_codici

        '    Case TipoColonnaRegistro.SitoRagioneSociale
        '        'il codice in centri_Aziendali_codici

        'End Select

        stb.AppendLine("         end ")
        stb.AppendLine("      end ")
        stb.AppendLine("  end ")
        stb.AppendLine("  else ")
        stb.AppendLine("      case when a.lav_Cod = 1054  then ")

        'Conferimento
        stb.AppendLine("          --Conferimento ")

        Select Case tipoMov
            Case TipoColonnaRegistro.CaricoScarico
                RiportaDocumentiGiasQryTipoMov_Doc(stb, TipoColonnaRegistro.CaricoScarico, TipoMovimentazioneRegistro_Documento.Conferimento)

            Case TipoColonnaRegistro.Causale
                RiportaDocumentiGiasQryTipoMov_Doc(stb, TipoColonnaRegistro.Causale, TipoMovimentazioneRegistro_Documento.Conferimento)

            Case TipoColonnaRegistro.Destinazione
                RiportaDocumentiGiasQryTipoMov_Doc(stb, TipoColonnaRegistro.Destinazione, TipoMovimentazioneRegistro_Documento.Conferimento)

            Case TipoColonnaRegistro.QtaBUDS
                RiportaDocumentiGiasQryTipoMov_Doc(stb, TipoColonnaRegistro.QtaBUDS, TipoMovimentazioneRegistro_Documento.Conferimento)

            Case TipoColonnaRegistro.Dest_RagioneSociale
                RiportaDocumentiGiasQryTipoMov_Doc(stb, TipoColonnaRegistro.Dest_RagioneSociale, TipoMovimentazioneRegistro_Documento.Conferimento)

            Case TipoColonnaRegistro.Dest_Indirizzo
                RiportaDocumentiGiasQryTipoMov_Doc(stb, TipoColonnaRegistro.Dest_Indirizzo, TipoMovimentazioneRegistro_Documento.Conferimento)

            Case TipoColonnaRegistro.Dest_Citta
                RiportaDocumentiGiasQryTipoMov_Doc(stb, TipoColonnaRegistro.Dest_Citta, TipoMovimentazioneRegistro_Documento.Conferimento)

            Case TipoColonnaRegistro.Dest_CAP
                RiportaDocumentiGiasQryTipoMov_Doc(stb, TipoColonnaRegistro.Dest_CAP, TipoMovimentazioneRegistro_Documento.Conferimento)

            Case TipoColonnaRegistro.Dest_Regione
                RiportaDocumentiGiasQryTipoMov_Doc(stb, TipoColonnaRegistro.Dest_Regione, TipoMovimentazioneRegistro_Documento.Conferimento)

            Case TipoColonnaRegistro.DestFin_RagioneSociale
                RiportaDocumentiGiasQryTipoMov_Doc(stb, TipoColonnaRegistro.DestFin_RagioneSociale, TipoMovimentazioneRegistro_Documento.Conferimento)

            Case TipoColonnaRegistro.DestFin_Indirizzo
                RiportaDocumentiGiasQryTipoMov_Doc(stb, TipoColonnaRegistro.DestFin_Indirizzo, TipoMovimentazioneRegistro_Documento.Conferimento)

            Case TipoColonnaRegistro.DestFin_Citta
                RiportaDocumentiGiasQryTipoMov_Doc(stb, TipoColonnaRegistro.DestFin_Citta, TipoMovimentazioneRegistro_Documento.Conferimento)

            Case TipoColonnaRegistro.DestFin_CAP
                RiportaDocumentiGiasQryTipoMov_Doc(stb, TipoColonnaRegistro.DestFin_CAP, TipoMovimentazioneRegistro_Documento.Conferimento)

            Case TipoColonnaRegistro.DestFin_Regione
                RiportaDocumentiGiasQryTipoMov_Doc(stb, TipoColonnaRegistro.DestFin_Regione, TipoMovimentazioneRegistro_Documento.Conferimento)

            Case TipoColonnaRegistro.SitoCodice
                RiportaDocumentiGiasQryTipoMov_Doc(stb, TipoColonnaRegistro.SitoCodice, TipoMovimentazioneRegistro_Documento.Conferimento)

            Case TipoColonnaRegistro.SitoRagioneSociale
                RiportaDocumentiGiasQryTipoMov_Doc(stb, TipoColonnaRegistro.SitoRagioneSociale, TipoMovimentazioneRegistro_Documento.Conferimento)

            Case TipoColonnaRegistro.Origine_Departure
                RiportaDocumentiGiasQryTipoMov_Doc(stb, TipoColonnaRegistro.Origine_Departure, TipoMovimentazioneRegistro_Documento.Conferimento)


        End Select




        'Select Case tipoMov
        '    Case TipoColonnaRegistro.CaricoScarico
        '        stb.AppendLine("        'Loading' ")

        '    Case TipoColonnaRegistro.Causale
        '        stb.AppendLine("        'WH Load' ")

        '    Case TipoColonnaRegistro.Destinazione
        '        stb.AppendLine(" case when " & isCisterna & " then 'WH Cisterna' else 'WH Scanzano' end ")
        '        strAlias = "Destinazione"

        '    Case TipoColonnaRegistro.QtaBUDS
        '        stb.AppendLine("    detProd.Qta    ")  'buds

        '    Case TipoColonnaRegistro.Dest_RagioneSociale
        '        stb.AppendLine("  ''  ")   'TODO
        'End Select

        stb.AppendLine("      end ")
        stb.AppendLine("  end  ")
        stb.AppendLine("  end as " & strAlias)

    End Sub

    Public Function DocumentiGiasRiportoTabellaLog(
        piva As String,
        xFiltroAggiuntivo As String,
        objParametri As AgronicaCoreParametri
        ) As Boolean

        '----- Descrizione
        Dim NomeRoutine As String = "ConfermaVoceRegistro()"

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            Stb.Length = 0


            Stb.AppendLine(" ")
            Stb.AppendLine(" insert ws_VivaiPassaporti_Operazioni_log ( ")
            Stb.AppendLine("   ")
            Stb.AppendLine("   [ws_VivaiPassaporti_Operazione_cod]   ")
            Stb.AppendLine("  ,[Doc_in_mov_dettagli_piva]  ")
            Stb.AppendLine("  ,[Doc_in_mov_dettagli_id_agenda]   ")
            Stb.AppendLine("  ,[Doc_in_mov_dettagli_id_mov]   ")
            Stb.AppendLine("  ,[Doc_in_mov_dettagli_id_mov_det]  ")
            Stb.AppendLine("  ,[DataMovimento]   ")
            Stb.AppendLine("  ,[Azione]   ")
            Stb.AppendLine("  ,[CodiceRiga]   ")
            Stb.AppendLine("  ,[Gestita]  ")
            Stb.AppendLine(" ) ")
            Stb.AppendLine("  ")
            Stb.AppendLine(" select  ")
            Stb.AppendLine("   [ws_VivaiPassaporti_Operazione_cod]   ")
            Stb.AppendLine(" , [Doc_in_mov_dettagli_piva]  ")
            Stb.AppendLine(" , [Doc_in_mov_dettagli_id_agenda]   ")
            Stb.AppendLine(" , [Doc_in_mov_dettagli_id_mov]   ")
            Stb.AppendLine(" , [Doc_in_mov_dettagli_id_mov_det]  ")
            Stb.AppendLine(" , [DataMovimento]   ")
            Stb.AppendLine(" , '' as [Azione]   ")
            Stb.AppendLine(" , null as [CodiceRiga]   ")
            Stb.AppendLine(" , 0 as [Gestita]  ")

            ''todo, capire come usare il flag inviato per riportare
            'Stb.AppendLine(" , case when op.inviato = -1 then -2 else  0  end as [Inviato]  ")
            Stb.AppendLine("  ")

            Stb.AppendLine(" from ws_VivaiPassaporti_Operazioni op ")

            Stb.AppendLine(" where Doc_in_mov_dettagli_id_agenda <> 0  ")

            If piva <> "" Then
                Stb.AppendLine(" and Doc_in_mov_dettagli_piva = '" & Agro_SQL_SaveText(piva) & "'")
            End If

            Stb.AppendLine(" and not exists (")
            Stb.AppendLine("  select 1 ")
            Stb.AppendLine("  from ws_VivaiPassaporti_Operazioni_log l ")
            Stb.AppendLine("  where   ")
            ''TODO: capire se in fase di riporto si può escludere qualche caso, ad esempio quanto marcato come "DEL" se già presente ed associato ad agenda
            Stb.AppendLine("      op.[ws_VivaiPassaporti_Operazione_cod] =  l.[ws_VivaiPassaporti_Operazione_cod] ")
            Stb.AppendLine("  and op.[Doc_in_mov_dettagli_piva] = l.[Doc_in_mov_dettagli_piva] ")
            Stb.AppendLine("  and op.[Doc_in_mov_dettagli_id_agenda]  = l.[Doc_in_mov_dettagli_id_agenda] ")
            Stb.AppendLine("  and op.[Doc_in_mov_dettagli_id_mov]  = l.[Doc_in_mov_dettagli_id_mov]  ")
            Stb.AppendLine("  and op.[Doc_in_mov_dettagli_id_mov_det] = l.[Doc_in_mov_dettagli_id_mov_det] ")
            Stb.AppendLine("   ")
            Stb.AppendLine(" )")


            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp

    End Function
    Public Function DocumentiGiasAssegnaAzione(
        ByVal ws_VivaiPassaporti_Operazione_cod As Integer,
        ByVal AzioneDaAssegnare As String,
        xFiltroAggiuntivo As String,
        objParametri As AgronicaCoreParametri
    ) As Boolean

        '----- Descrizione
        Dim NomeRoutine As String = "ConfermaVoceRegistro()"

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            Stb.Length = 0

            Stb.AppendLine("update l ")
            Stb.AppendLine(" set azione = '" & AzioneDaAssegnare & "' ")
            Stb.AppendLine(" , data_modifica = getdate() ")
            Stb.AppendLine(" , username_modifica = '" & objParametri.UtenteUsername & "' ")
            Stb.AppendLine(" from ws_VivaiPassaporti_Operazioni_log l ")
            Stb.AppendLine(" where ws_VivaiPassaporti_Operazione_cod = " & ws_VivaiPassaporti_Operazione_cod)


            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp

    End Function


    Public Function DocumentiGiasImpostaFlgInviatoDaGestita(
        valoreFlagInviato As Integer,
        xFiltro As String,
        xFiltroAggiuntivo As String,
        objParametri As AgronicaCoreParametri
    ) As Boolean

        '----- Descrizione
        Dim NomeRoutine As String = "ConfermaVoceRegistro()"

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            Stb.Length = 0

            're-imposta lo stato precedente
            Stb.AppendLine(" update l ")
            Stb.AppendLine(" set inviato = " & valoreFlagInviato)

            'Stb.AppendLine(" , data_modifica = getdate() ")
            Stb.AppendLine(" , username_modifica = '" & objParametri.UtenteUsername & "' ")
            Stb.AppendLine(" from ws_VivaiPassaporti_Operazioni_log l ")
            Stb.AppendLine(" where  ")
            Stb.AppendLine(xFiltro)

            If xFiltroAggiuntivo <> "" Then
                Stb.AppendLine(Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))

            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp

    End Function

    Public Function DocumentiGiasImpostaAzioneDaFlgInviato(
        ByVal piva As String,
        xFiltroAggiuntivo As String,
        objParametri As AgronicaCoreParametri
    ) As Boolean

        '----- Descrizione
        Dim NomeRoutine As String = "ConfermaVoceRegistro()"

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            Stb.Length = 0

            're-imposta lo stato precedente
            Stb.AppendLine(" update l ")
            Stb.AppendLine(" set azione =  ")
            Stb.AppendLine("  case when inviato = " & VivaiComportamentoFlginviato.ReimpostaStatoMovRegSuTabellaLog & " then 'MODREG' else  ")
            Stb.AppendLine("      case when inviato in ( " & VivaiComportamentoFlginviato.ReimpostaStatoDelSuTabellaLog & ", " & VivaiComportamentoFlginviato.RimuoviRigheAggiunteSoloRegistroReimpostaStatoDel & ") then 'DEL' else  ")
            Stb.AppendLine("          case when inviato = " & VivaiComportamentoFlginviato.ReimpostaStatoBlankSuTabellaLog & " then '' else  ")
            Stb.AppendLine("              ''     ")
            Stb.AppendLine("          end   ")
            Stb.AppendLine("     end")
            Stb.AppendLine("  end")

            'backup...
            'Stb.AppendLine("  case when inviato = " & VivaiComportamentoFlginviato.ReimpostaStatoMovRegSuTabellaLog & " then 'MODREG' else  ")
            'Stb.AppendLine("      case when inviato = " & VivaiComportamentoFlginviato.RimuoviRigheAggiunteSoloRegistro & " then 'DEL' else  ")
            'Stb.AppendLine("          case when inviato = " & VivaiComportamentoFlginviato.ReimpostaStatoDelSuTabellaLog & " then 'DEL' else  ")
            'Stb.AppendLine("      ''     ")
            'Stb.AppendLine("          end   ")
            'Stb.AppendLine("     end")
            'Stb.AppendLine(" end")


            'reset del flag
            Stb.AppendLine(" , inviato = 0 ")

            'Stb.AppendLine(" , data_modifica = getdate() ")
            Stb.AppendLine(" , username_modifica = '" & objParametri.UtenteUsername & "' ")
            Stb.AppendLine(" from ws_VivaiPassaporti_Operazioni_log l ")
            Stb.AppendLine(" where inviato in (" &
                           VivaiComportamentoFlginviato.RimuoviRigheAggiunteSoloRegistroReimpostaStatoDel & "," &
                           VivaiComportamentoFlginviato.ReimpostaStatoDelSuTabellaLog & ", " &
                           VivaiComportamentoFlginviato.ReimpostaStatoBlankSuTabellaLog & ", " &
                           VivaiComportamentoFlginviato.ReimpostaStatoMovRegSuTabellaLog &
                           ") ")
            Stb.AppendLine(" and l.Doc_in_mov_dettagli_piva = '" & Agro_SQL_SaveText(piva) & "' ")


            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp

    End Function

    Public Function DocumentiGiasFlgInviatoDaFlgInTabOperazioni(
        inviato As Integer,
        xFiltroAggiuntivo As String,
        objParametri As AgronicaCoreParametri
    ) As Boolean

        '----- Descrizione
        Dim NomeRoutine As String = "ConfermaVoceRegistro()"

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            Stb.Length = 0

            Stb.AppendLine("update l ")
            Stb.AppendLine(" set ")
            Stb.AppendLine(" , inviato = " & inviato)
            'Stb.AppendLine(" , data_modifica = getdate() ")
            Stb.AppendLine(" , username_modifica = '" & objParametri.UtenteUsername & "' ")
            Stb.AppendLine(" from ws_VivaiPassaporti_Operazioni l ")
            Stb.AppendLine(" where inviato <> 0 ")


            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp

    End Function

    Public Function DocumentiGiasInTabRegistroResetFlgInviato(
        piva As String,
        inviato As Integer,
        xFiltroAggiuntivo As String,
        objParametri As AgronicaCoreParametri
    ) As Boolean

        '----- Descrizione
        Dim NomeRoutine As String = "ConfermaVoceRegistro()"

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            Stb.Length = 0

            Stb.AppendLine("update l ")
            Stb.AppendLine(" set ")
            Stb.AppendLine("   inviato = 0 ")
            'Stb.AppendLine(" , data_modifica = getdate() ")
            Stb.AppendLine(" , username_modifica = '" & objParametri.UtenteUsername & "' ")
            Stb.AppendLine(" from ws_VivaiPassaporti_Operazioni l ")
            Stb.AppendLine(" where inviato <> 0 ")
            Stb.AppendLine(" and Doc_in_mov_dettagli_piva = '" & Agro_SQL_SaveText(piva) & "' ")


            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp

    End Function

    Public Function DocumentiGiasRimuoviRigheAggiunteSuRegistroPerRecupera(
        ByVal piva As String,
        xFiltroAggiuntivo As String,
        objParametri As AgronicaCoreParametri
    ) As Boolean

        '----- Descrizione
        Dim NomeRoutine As String = "ConfermaVoceRegistro()"

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            Stb.Length = 0

            Stb.AppendLine(" delete op ")
            Stb.AppendLine(" from [dbo].[ws_VivaiPassaporti_Operazioni] op ")
            Stb.AppendLine("  inner join [ws_VivaiPassaporti_Operazioni_log] l ")
            Stb.AppendLine("  on  op.[ws_VivaiPassaporti_Operazione_cod] =  l.[ws_VivaiPassaporti_Operazione_cod]  ")
            Stb.AppendLine("               and op.[Doc_in_mov_dettagli_piva] = l.[Doc_in_mov_dettagli_piva]  ")
            Stb.AppendLine("               and op.[Doc_in_mov_dettagli_id_agenda]  = l.[Doc_in_mov_dettagli_id_agenda] ")
            Stb.AppendLine("               and op.[Doc_in_mov_dettagli_id_mov]  = l.[Doc_in_mov_dettagli_id_mov]   ")
            Stb.AppendLine("               and op.[Doc_in_mov_dettagli_id_mov_det] = l.[Doc_in_mov_dettagli_id_mov_det]  ")
            Stb.AppendLine(" where l.inviato in( " &
                           VivaiComportamentoFlginviato.RimuoviRigheAggiunteSoloRegistro & " , " &
                           VivaiComportamentoFlginviato.RimuoviRigheAggiunteSuRegistroELog & " , " &
                           VivaiComportamentoFlginviato.RimuoviRigheAggiunteSoloRegistroReimpostaStatoDel &
                           " )")
            Stb.AppendLine(" and op.Doc_in_mov_dettagli_id_agenda <> 0")
            Stb.AppendLine(" and op.Doc_in_mov_dettagli_piva = '" & Agro_SQL_SaveText(piva) & "' ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp

    End Function

    Public Function DocumentiGiasRimuoviRigheAggiunteSuLogPerRecupera(
        piva As String,
        xFiltroAggiuntivo As String,
        objParametri As AgronicaCoreParametri
    ) As Boolean

        '----- Descrizione
        Dim NomeRoutine As String = "ConfermaVoceRegistro()"

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            Stb.Length = 0

            Stb.AppendLine(" delete l ")
            Stb.AppendLine(" from [dbo].[ws_VivaiPassaporti_Operazioni_log] l ")
            Stb.AppendLine(" where l.inviato in (" & VivaiComportamentoFlginviato.RimuoviRigheAggiunteSuRegistroELog & " )")
            Stb.AppendLine(" and l.Doc_in_mov_dettagli_piva = '" & Agro_SQL_SaveText(piva) & "' ")


            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp

    End Function
    Public Function DocumentiGiasInTabLogUpdateFlgInviatoDaTabRegistro(
        piva As String,
        xFiltroAggiuntivo As String,
        objParametri As AgronicaCoreParametri
    ) As Boolean

        '----- Descrizione
        Dim NomeRoutine As String = "ConfermaVoceRegistro()"

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            Stb.Length = 0

            Stb.AppendLine(" update l ")
            Stb.AppendLine(" set  ")
            'backup
            'capire le varie casistiche, al momento imposto "-2".
            'Stb.AppendLine("  inviato = case when l.azione = 'DEL' then -2 else 0 end   ")
            'Stb.AppendLine("  inviato = -2   ")
            Stb.AppendLine("  inviato = -3   ")
            Stb.AppendLine(" from [dbo].[ws_VivaiPassaporti_Operazioni] op ")
            Stb.AppendLine("  inner join [ws_VivaiPassaporti_Operazioni_log] l ")
            Stb.AppendLine("  on  op.[ws_VivaiPassaporti_Operazione_cod] =  l.[ws_VivaiPassaporti_Operazione_cod]  ")
            Stb.AppendLine("               and op.[Doc_in_mov_dettagli_piva] = l.[Doc_in_mov_dettagli_piva]  ")
            Stb.AppendLine("               and op.[Doc_in_mov_dettagli_id_agenda]  = l.[Doc_in_mov_dettagli_id_agenda] ")
            Stb.AppendLine("               and op.[Doc_in_mov_dettagli_id_mov]  = l.[Doc_in_mov_dettagli_id_mov]   ")
            Stb.AppendLine("               and op.[Doc_in_mov_dettagli_id_mov_det] = l.[Doc_in_mov_dettagli_id_mov_det]  ")
            Stb.AppendLine(" where op.inviato = -1")
            Stb.AppendLine(" and l.inviato = 0 ") 'non ancora impostato altrove
            Stb.AppendLine(" and op.Doc_in_mov_dettagli_id_agenda<>0 ")
            Stb.AppendLine(" and op.Doc_in_mov_dettagli_piva ='" & Agro_SQL_SaveText(piva) & "' ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp

    End Function

    Public Function DocumentiGiasAssegnaFlgInviato(
       Doc_in_mov_dettagli_piva As String,
       Doc_in_mov_dettagli_id_agenda As Integer,
       Doc_in_mov_dettagli_id_mov As Integer,
       Doc_in_mov_dettagli_id_mov_det As Integer,
       ByVal inviato As Integer,
       xFiltroAggiuntivo As String,
       objParametri As AgronicaCoreParametri
   ) As Boolean

        '----- Descrizione
        Dim NomeRoutine As String = "ConfermaVoceRegistro()"

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            Stb.Length = 0

            Stb.AppendLine("update l ")
            Stb.AppendLine(" set inviato = " & inviato & " ")
            Stb.AppendLine(" , data_modifica = getdate() ")
            Stb.AppendLine(" , username_modifica = '" & objParametri.UtenteUsername & "' ")
            Stb.AppendLine(" from ws_VivaiPassaporti_Operazioni_log l ")
            Stb.AppendLine(" where Doc_in_mov_dettagli_piva = '" & Agro_SQL_SaveText(Doc_in_mov_dettagli_piva) & "'")
            Stb.AppendLine(" and Doc_in_mov_dettagli_id_agenda = " & Doc_in_mov_dettagli_id_agenda & " ")
            Stb.AppendLine(" and Doc_in_mov_dettagli_id_mov = " & Doc_in_mov_dettagli_id_mov & " ")
            Stb.AppendLine(" and Doc_in_mov_dettagli_id_mov_det = " & Doc_in_mov_dettagli_id_mov_det & " ")

            If xFiltroAggiuntivo <> "" Then
                Stb.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp

    End Function

    Public Function DocumentiGiasAssegnaFlgInviato(
        ByVal ws_VivaiPassaporti_Operazione_cod As Integer,
        ByVal inviato As Integer,
        xFiltroAggiuntivo As String,
        objParametri As AgronicaCoreParametri
    ) As Boolean

        '----- Descrizione
        Dim NomeRoutine As String = "ConfermaVoceRegistro()"

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            Stb.Length = 0

            Stb.AppendLine("update l ")
            Stb.AppendLine(" set inviato = " & inviato & " ")
            Stb.AppendLine(" , data_modifica = getdate() ")
            Stb.AppendLine(" , username_modifica = '" & objParametri.UtenteUsername & "' ")
            Stb.AppendLine(" from ws_VivaiPassaporti_Operazioni_log l ")
            Stb.AppendLine(" where ws_VivaiPassaporti_Operazione_cod = " & ws_VivaiPassaporti_Operazione_cod)


            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp

    End Function

    ''' <summary>
    ''' imposta il flag "gestita"
    ''' </summary>
    ''' <param name="ws_VivaiPassaporti_Operazione_cod"></param>
    ''' <param name="Doc_in_mov_dettagli_piva"></param>
    ''' <param name="Doc_in_mov_dettagli_id_agenda"></param>
    ''' <param name="Doc_in_mov_dettagli_id_mov"></param>
    ''' <param name="Doc_in_mov_dettagli_id_mov_det"></param>
    ''' <param name="ValoreFlag">1 = esclude dalla query di lettura (solo nel caso "recupera") -- -1 richiede cancellazione record quando si preme pulsante "recupera" --0 viene incluso nella lettura (solo nel caso "recupera"</param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="objParametri"></param>
    ''' <remarks></remarks>
    ''' <returns></returns>
    Public Function UpdateFlagGestito(
        ByVal ws_VivaiPassaporti_Operazione_cod As Integer,
        ByVal Doc_in_mov_dettagli_piva As String,
        ByVal Doc_in_mov_dettagli_id_agenda As Integer,
        ByVal Doc_in_mov_dettagli_id_mov As Integer,
        ByVal Doc_in_mov_dettagli_id_mov_det As Integer,
        ByVal ValoreFlag As Integer,
        xFiltroAggiuntivo As String,
        objParametri As AgronicaCoreParametri
    ) As Boolean

        '----- Descrizione
        Dim NomeRoutine As String = "ConfermaVoceRegistro()"

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            Stb.Length = 0

            Stb.AppendLine(" update l ")
            Stb.AppendLine(" set Gestita = " & ValoreFlag)
            Stb.AppendLine(" from ws_VivaiPassaporti_Operazioni_log l ")
            Stb.AppendLine(" where ([ws_VivaiPassaporti_Operazione_cod] = " & ws_VivaiPassaporti_Operazione_cod)
            Stb.AppendLine(" and [Doc_in_mov_dettagli_piva] = '" & Agro_SQL_SaveText(Doc_in_mov_dettagli_piva) & "' ")
            Stb.AppendLine(" and [Doc_in_mov_dettagli_id_agenda] = " & Doc_in_mov_dettagli_id_agenda)
            Stb.AppendLine(" and [Doc_in_mov_dettagli_id_mov] = " & Doc_in_mov_dettagli_id_mov)
            Stb.AppendLine(" and [Doc_in_mov_dettagli_id_mov_det] = " & Doc_in_mov_dettagli_id_mov_det)
            Stb.AppendLine(" )")

            If xFiltroAggiuntivo <> "" Then
                Stb.AppendLine(Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp

    End Function


    Public Function Cancella(
        ByVal ws_VivaiPassaporti_Operazione_cod As Integer,
        xFiltroAggiuntivo As String,
        objParametri As AgronicaCoreParametri
    ) As Boolean

        '----- Descrizione
        Dim NomeRoutine As String = "ConfermaVoceRegistro()"

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            Stb.Length = 0

            Stb.AppendLine(" delete  ")
            Stb.AppendLine(" from ws_VivaiPassaporti_Operazioni ")
            Stb.AppendLine(" where ws_VivaiPassaporti_Operazione_cod = " & ws_VivaiPassaporti_Operazione_cod)

            If xFiltroAggiuntivo <> "" Then
                Stb.AppendLine(Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp

    End Function

    Public Function CancellaLog(
        ByVal ws_VivaiPassaporti_Operazione_cod As Integer,
        ByVal Doc_in_mov_dettagli_piva As String,
        ByVal Doc_in_mov_dettagli_id_agenda As Integer,
        ByVal Doc_in_mov_dettagli_id_mov As Integer,
        ByVal Doc_in_mov_dettagli_id_mov_det As Integer,
        xFiltroAggiuntivo As String,
        objParametri As AgronicaCoreParametri
    ) As Boolean

        '----- Descrizione
        Dim NomeRoutine As String = "ConfermaVoceRegistro()"

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            Stb.Length = 0

            Stb.AppendLine(" delete  ")
            Stb.AppendLine(" from ws_VivaiPassaporti_Operazioni_log ")
            Stb.AppendLine(" where ([ws_VivaiPassaporti_Operazione_cod] = " & ws_VivaiPassaporti_Operazione_cod)
            Stb.AppendLine("     and [Doc_in_mov_dettagli_piva] = '" & Agro_SQL_SaveText(Doc_in_mov_dettagli_piva) & "' ")
            Stb.AppendLine("     and [Doc_in_mov_dettagli_id_agenda] = " & Doc_in_mov_dettagli_id_agenda)
            Stb.AppendLine("     and [Doc_in_mov_dettagli_id_mov] = " & Doc_in_mov_dettagli_id_mov)
            Stb.AppendLine("     and [Doc_in_mov_dettagli_id_mov_det] = " & Doc_in_mov_dettagli_id_mov_det)
            Stb.AppendLine(" ) ")

            If xFiltroAggiuntivo <> "" Then
                Stb.AppendLine(Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp

    End Function


    Public Function MarcaDocumentiSenzaPiuAgenda(
        piva As String,
        xFiltroAggiuntivo As String,
        objParametri As AgronicaCoreParametri
    ) As Boolean

        '----- Descrizione
        Dim NomeRoutine As String = "ConfermaVoceRegistro()"

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            Stb.Length = 0

            Stb.AppendLine(" update log1 ")
            Stb.AppendLine(" set azione = 'ASS' ")
            'test
            Stb.AppendLine(" , inviato = case when azione =  'MODREG' then " & VivaiComportamentoFlginviato.ReimpostaStatoMovRegSuTabellaLog & " else 0 end ")
            'Todo: verificare qui
            'Stb.AppendLine(" from (  ")
            'Stb.AppendLine(" select *  ")
            'Stb.AppendLine(" from ws_VivaiPassaporti_Operazioni_log log1 ")
            'Stb.AppendLine(" where Doc_in_mov_dettagli_piva = '" & Agro_Sql_SaveText(piva) & "' ")
            'Stb.AppendLine(" ) log1  ")
            Stb.AppendLine(" from ws_VivaiPassaporti_Operazioni_log log1 ")
            Stb.AppendLine("  left join Movimenti_dettagli movDet ")
            Stb.AppendLine("      on log1.[Doc_in_mov_dettagli_piva] =movDet.piva ")
            Stb.AppendLine("      And log1.[Doc_in_mov_dettagli_id_agenda] = movDet.id_agenda ")
            Stb.AppendLine("      And log1.[Doc_in_mov_dettagli_id_mov] = movDet.id_mov ")
            Stb.AppendLine("      And log1.[Doc_in_mov_dettagli_id_mov_det] = movDet.id_mov_det ")
            Stb.AppendLine(" where movDet.id_agenda Is null  ")

            If xFiltroAggiuntivo <> "" Then
                Stb.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If


            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp

    End Function

    Public Function DocumentiGiasAssegnaAzioneInCascata(
        ByVal ws_VivaiPassaporti_Operazione_cod As Integer,
        ByVal AzionePerRicerca As String,
        ByVal AzioneDaAssegnare As String,
        xFiltroAggiuntivo As String,
        objParametri As AgronicaCoreParametri
    ) As Boolean

        '----- Descrizione
        Dim NomeRoutine As String = "ConfermaVoceRegistro()"

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            Stb.Length = 0

            Stb.AppendLine(" update l2 ")
            Stb.AppendLine(" set azione = '" & AzioneDaAssegnare & "' ")
            Stb.AppendLine(" , data_modifica = getdate() ")
            Stb.AppendLine(" , username_modifica = '" & objParametri.UtenteUsername & "' ")
            Stb.AppendLine(" , inviato = 0 ")
            Stb.AppendLine(" from ws_VivaiPassaporti_Operazioni_log l ")
            Stb.AppendLine("  inner join ws_VivaiPassaporti_Operazioni_log l2 ")
            Stb.AppendLine("      on l2.codiceRiga = l.codiceRiga ")
            Stb.AppendLine(" where l.ws_VivaiPassaporti_Operazione_cod =  " & ws_VivaiPassaporti_Operazione_cod)

            If xFiltroAggiuntivo <> "" Then
                Stb.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If


            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp

    End Function

    Public Function InserimentoGestitaDel(ByVal piva As String, ByVal xFiltroAggiuntivo As String, objParametri_Server As AgronicaCoreParametri) As Boolean

        '----- Descrizione
        Dim NomeRoutine As String = "ConfermaVoceRegistro()"

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            Stb.Length = 0

            Stb.AppendLine(" declare @Contatore int ")
            Stb.AppendLine(" declare @Contatore2 int ")
            Stb.AppendLine(" declare @Contatore3 int ")
            Stb.AppendLine(" set @contatore =  ISNULL((select ultimo_valore from sequenza_tabelle where nome_tabella = 'ws_vivaiPassaporti_operazioni'), 0) ")
            Stb.AppendLine(" set @contatore2 = ISNULL((select max(ws_vivaiPassaporti_operazione_cod) from ws_vivaiPassaporti_operazioni), 0) ")
            Stb.AppendLine(" set @contatore3 = ISNULL((select max(ws_vivaiPassaporti_operazione_cod) from ws_VivaiPassaporti_Operazioni_log), 0) ")
            Stb.AppendLine("                          ")
            Stb.AppendLine(" select top  1 @contatore = contatore ")
            Stb.AppendLine(" from ( ")
            Stb.AppendLine(" select @contatore as contatore ")
            Stb.AppendLine(" union all ")
            Stb.AppendLine(" select @contatore2 ")
            Stb.AppendLine(" union all ")
            Stb.AppendLine(" select @contatore3 ")
            Stb.AppendLine("              ")
            Stb.AppendLine("              ")
            Stb.AppendLine(" ) a ")
            Stb.AppendLine(" order by Contatore desc ")

            Stb.AppendLine(" insert ws_VivaiPassaporti_Operazioni_log ")
            Stb.AppendLine(" select  ")
            Stb.AppendLine("  DENSE_RANK() over (order by Doc_in_mov_dettagli_piva, Doc_in_mov_dettagli_id_agenda, Doc_in_mov_dettagli_id_mov, Doc_in_mov_dettagli_id_mov_det) + @Contatore as ws_VivaiPassaporti_Operazioni_cod   ")
            Stb.AppendLine("  , Doc_in_mov_dettagli_piva ")
            Stb.AppendLine("  , Doc_in_mov_dettagli_id_agenda ")
            Stb.AppendLine("  , Doc_in_mov_dettagli_id_mov ")
            Stb.AppendLine("  , Doc_in_mov_dettagli_id_mov_det ")
            Stb.AppendLine("  , DataMovimento ")
            Stb.AppendLine("  , 'DEL' as Azione ")
            Stb.AppendLine("  , codiceRiga ")
            Stb.AppendLine("  , -1 ")
            Stb.AppendLine("  , 0 as inviato ")
            Stb.AppendLine("  , null  as dataInvio ")
            Stb.AppendLine("  , getdate() as data_creazione ")
            Stb.AppendLine("  , GETDATE() as data_modifica ")
            Stb.AppendLine("  , username_creazione ")
            Stb.AppendLine("  , username_modifica ")
            Stb.AppendLine("  , Validita_Inizio ")
            Stb.AppendLine("  , Validita_Fine ")
            Stb.AppendLine("   ")
            Stb.AppendLine(" from [ws_VivaiPassaporti_Operazioni_log] ")
            Stb.AppendLine(" where inviato = " & VivaiComportamentoFlginviato.InserisciRigheConGestitaDel)
            Stb.AppendLine(" and Doc_in_mov_dettagli_piva = '" & Agro_SQL_SaveText(piva) & "' ")

            If xFiltroAggiuntivo <> "" Then
                Stb.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri_Server))
            End If


            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri_Server, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp
    End Function

    Public Function ReImpostaFlagInviatoSuGestitaDel(ByVal piva As String, ByVal xFiltroAggiuntivo As String, objParametri_Server As AgronicaCoreParametri) As Boolean

        '----- Descrizione
        Dim NomeRoutine As String = "ConfermaVoceRegistro()"

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            Stb.Length = 0

            Stb.AppendLine(" update a ")
            Stb.AppendLine(" set inviato = " & VivaiComportamentoFlginviato.ReimpostaStatoBlankSuTabellaLog)
            Stb.AppendLine(" from [ws_VivaiPassaporti_Operazioni_log] a ")
            Stb.AppendLine(" where inviato =  " & VivaiComportamentoFlginviato.InserisciRigheConGestitaDel)
            Stb.AppendLine(" and a.Doc_in_mov_dettagli_piva = '" & Agro_SQL_SaveText(piva) & "' ")

            'solo per utente connesso
            'Stb.AppendLine(" and username_modifica = '" & objParametri_Server.UtenteUsername & "'")


            If xFiltroAggiuntivo <> "" Then
                Stb.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri_Server))
            End If


            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri_Server, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp
    End Function

    Public Sub MarcaPerReInserimentoGestitaDel(objParametri_Server As AgronicaCoreParametri, rip As VivaiPassaporti_Operazioni_W, dtDaMarcareXcancellati As DataTable)
        For Each riga In dtDaMarcareXcancellati.Rows
            rip.DocumentiGiasAssegnaFlgInviato(
                riga("Doc_in_mov_dettagli_piva"),
                riga("Doc_in_mov_dettagli_id_agenda"),
                riga("Doc_in_mov_dettagli_id_mov"),
                riga("Doc_in_mov_dettagli_id_mov_det"),
                VivaiPassaporti_Operazioni_W.VivaiComportamentoFlginviato.InserisciRigheConGestitaDel,
                " Azione = 'MOD' ",
                objParametri_Server
                )
        Next
    End Sub

    Public Function DocumentiGiasAssegnaAzioneMOD(
            ByVal piva As String,
            RecuperaRigheRegistro As Boolean,
            xFiltroAggiuntivo As String,
            objParametri As AgronicaCoreParametri
        ) As Boolean

        '----- Descrizione
        Dim NomeRoutine As String = "ConfermaVoceRegistro()"

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            Stb.Length = 0


            Stb.AppendLine(" update l ")
            Stb.AppendLine(" set  ")

            'Stb.AppendLine("     Azione = case when l1.data_modifica <> l.data_modifica then 'MOD' else 'PEN' end ")
            'Stb.AppendLine("  , codiceRiga = null ")

            'versione post 2020-05-22
            Stb.AppendLine("  codiceRiga = null  ")
            Stb.AppendLine("  ")
            If RecuperaRigheRegistro Then
                Stb.AppendLine("   , inviato = case when l1.data_modifica <> l.data_modifica then ")
                Stb.AppendLine("                case when l.Azione = 'MODREG' then " & VivaiComportamentoFlginviato.ReimpostaStatoMovRegSuTabellaLog & " else " & VivaiComportamentoFlginviato.ReimpostaStatoBlankSuTabellaLog & " end ")
                Stb.AppendLine("            else 0 ")
                Stb.AppendLine("            end ")
                Stb.AppendLine("  ")
            End If

            Stb.AppendLine("   , Azione = case when l1.data_modifica <> l.data_modifica then 'MOD' else 'PEN' end ")

            ''Backup 2020-05-22 -- commentare tutto
            'If RecuperaRigheRegistro Then

            '    'versione precedente al 22/05
            '    '    Stb.AppendLine("  , inviato =  case when l1.data_modifica <> l.data_modifica then " &  VivaiComportamentoFlginviato.ReimpostaStatoMovRegSuTabellaLog & " else " & VivaiComportamentoFlginviato.RimuoviRigheAggiunteSoloRegistro  & " end " )

            '    '    'Stb.AppendLine("  , inviato =  ")
            '    '    'Stb.AppendLine("      case when l1.data_modifica <> l.data_modifica then  ")
            '    '    'Stb.AppendLine("          case when l.Azione = '' then " & VivaiComportamentoFlginviato.ReimpostaStatoDelSuTabellaLog & " else " & VivaiComportamentoFlginviato.ReimpostaStatoMovRegSuTabellaLog & " end ")
            '    '    'Stb.AppendLine("      else  ")
            '    '    'Stb.AppendLine("            " & VivaiComportamentoFlginviato.RimuoviRigheAggiunteSuRegistroELog)
            '    '    'Stb.AppendLine("      end ")

            'End If
            ''fine Backup 2020-05-22 -- 

            Stb.AppendLine(" from ( ")
            Stb.AppendLine("  select [Doc_in_mov_dettagli_piva]  ")
            Stb.AppendLine("                  , [Doc_in_mov_dettagli_id_agenda]   ")
            Stb.AppendLine("                  , [Doc_in_mov_dettagli_id_mov]   ")
            Stb.AppendLine("                  , [Doc_in_mov_dettagli_id_mov_det] ")
            Stb.AppendLine("                  , max(Data_modifica) as data_modifica ")
            Stb.AppendLine("  from ws_VivaiPassaporti_Operazioni_log       ")
            'Stb.AppendLine("  WHERE azione = ''")
            Stb.AppendLine("  WHERE Doc_in_mov_dettagli_piva = '" & Agro_SQL_SaveText(piva) & "' ")

            If xFiltroAggiuntivo <> "" Then
                Stb.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            Stb.AppendLine("  group by [Doc_in_mov_dettagli_piva]  ")
            Stb.AppendLine("                  , [Doc_in_mov_dettagli_id_agenda]   ")
            Stb.AppendLine("                  , [Doc_in_mov_dettagli_id_mov]   ")
            Stb.AppendLine("                  , [Doc_in_mov_dettagli_id_mov_det] ")
            Stb.AppendLine("  having count(*) > 1 ")
            Stb.AppendLine(" ) l1 ")
            Stb.AppendLine("  inner join ws_VivaiPassaporti_Operazioni_log l ")
            Stb.AppendLine("      on l1.[Doc_in_mov_dettagli_piva] = l.[Doc_in_mov_dettagli_piva] ")
            Stb.AppendLine("  and l1.[Doc_in_mov_dettagli_id_agenda]  = l.[Doc_in_mov_dettagli_id_agenda] ")
            Stb.AppendLine("  and l1.[Doc_in_mov_dettagli_id_mov]  = l.[Doc_in_mov_dettagli_id_mov]  ")
            Stb.AppendLine("  and l1.[Doc_in_mov_dettagli_id_mov_det] = l.[Doc_in_mov_dettagli_id_mov_det] ")


            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp

    End Function
    Public Function DocumentiGiasAssegnaCodiceRiga(
            piva As String,
            xFiltroAggiuntivo As String,
            objParametri As AgronicaCoreParametri
        ) As Boolean

        '----- Descrizione
        Dim NomeRoutine As String = "ConfermaVoceRegistro()"

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            Stb.Length = 0


            Stb.AppendLine("update l1 ")
            Stb.AppendLine(" set codiceRiga = l.CodiceRiga ")
            Stb.AppendLine("    ")
            Stb.AppendLine("  from ( ")
            Stb.AppendLine("      select top 100 percent ")
            Stb.AppendLine("          dense_rank() over (order by  ")
            Stb.AppendLine("                [Doc_in_mov_dettagli_piva]  ")
            Stb.AppendLine("              , [Doc_in_mov_dettagli_id_agenda]   ")
            Stb.AppendLine("              , [Doc_in_mov_dettagli_id_mov]   ")
            Stb.AppendLine("              , [Doc_in_mov_dettagli_id_mov_det] ) + ( isnull(  ")
            Stb.AppendLine("                  (select max(codiceRiga) from ws_VivaiPassaporti_Operazioni_log) ")
            Stb.AppendLine("              , 0)) as CodiceRiga ")
            Stb.AppendLine("              , [Doc_in_mov_dettagli_piva]  ")
            Stb.AppendLine("              , [Doc_in_mov_dettagli_id_agenda]   ")
            Stb.AppendLine("              , [Doc_in_mov_dettagli_id_mov]   ")
            Stb.AppendLine("              , [Doc_in_mov_dettagli_id_mov_det] ")
            Stb.AppendLine("              , ws_VivaiPassaporti_Operazione_cod ")
            Stb.AppendLine("           ")
            Stb.AppendLine("      from ws_VivaiPassaporti_Operazioni_log  ")
            Stb.AppendLine("      where CodiceRiga is null     ")
            Stb.AppendLine("      and Doc_in_mov_dettagli_piva = '" & Agro_SQL_SaveText(piva) & "' ")

            If xFiltroAggiuntivo <> "" Then
                Stb.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            Stb.AppendLine("      order by ws_VivaiPassaporti_Operazione_cod ")
            Stb.AppendLine(" ) l ")
            Stb.AppendLine("  inner join ws_VivaiPassaporti_Operazioni_log l1 ")
            Stb.AppendLine("   on l1.[ws_VivaiPassaporti_Operazione_cod] =  l.[ws_VivaiPassaporti_Operazione_cod] ")
            Stb.AppendLine("  and l1.[Doc_in_mov_dettagli_piva] = l.[Doc_in_mov_dettagli_piva] ")
            Stb.AppendLine("  and l1.[Doc_in_mov_dettagli_id_agenda]  = l.[Doc_in_mov_dettagli_id_agenda] ")
            Stb.AppendLine("  and l1.[Doc_in_mov_dettagli_id_mov]  = l.[Doc_in_mov_dettagli_id_mov]  ")
            Stb.AppendLine("  and l1.[Doc_in_mov_dettagli_id_mov_det] = l.[Doc_in_mov_dettagli_id_mov_det] ")
            Stb.AppendLine("   ")
            Stb.AppendLine("      ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp

    End Function

    ''' <summary>
    ''' Riporto dei documenti Gias nel registro di carico scarico
    ''' </summary>
    ''' <param name="piva"></param>
    ''' <param name="listaLavCod">Lista dei lav cod per la lettura</param>
    ''' <param name="cauMovTestata">CauMov della testata su documenti da considere in ingresso (carichi)</param>
    ''' <param name="cauMovTestataOut">CauMov della testata su documenti da considere in uscita (scarichi)</param>
    ''' <param name="cauMovDettagli">CauMov dei dettagli su documenti da considere in ingresso (carichi)</param>
    ''' <param name="cauMovDettagliOut">CauMov dei dettagli su documenti da considere in uscita (scarichi)</param>
    ''' <param name="FiltroElemCod"></param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="objParametri"></param>
    ''' <returns></returns>
    Public Function RiportaDocumentiGias(
        ByVal RecuperaMovimentiRecentiRegistro As Boolean,
        ByVal RecuperaRigheRegistro As Boolean,
        piva As String,
        listaLavCodOperazioni As String,
        ByVal cauMovTestataIn As String,
        ByVal cauMovTestataOut As String,
        ByVal cauMovDettagliIn As String,
        ByVal cauMovDettagliOut As String,
        ByVal ListaParametriQualitativi As String,
        ByVal FiltroElemCod As String,
        ByVal ImpostaFlagInviato As Boolean,
        xFiltroAggiuntivo As String,
        objParametri As AgronicaCoreParametri
    ) As Boolean

        '----- Descrizione
        Dim NomeRoutine As String = "ConfermaVoceRegistro()"

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Dim vListaParametriQualitativi As String() = ListaParametriQualitativi.Split(",")
        Dim vListaLavCodOperazioni As String() = listaLavCodOperazioni.Split(",")


        Try

            Stb.Length = 0
            Stb.AppendLine("  declare @Contatore int ")
            Stb.AppendLine("  declare @Contatore2 int ")
            Stb.AppendLine("  declare @Contatore3 int ")
            Stb.AppendLine("  set @contatore =  ISNULL((select ultimo_valore from sequenza_tabelle where nome_tabella = 'ws_vivaiPassaporti_operazioni'), 0) ")
            Stb.AppendLine("  set @contatore2 = ISNULL((select max(ws_vivaiPassaporti_operazione_cod) from ws_vivaiPassaporti_operazioni), 0) ")
            Stb.AppendLine("  set @contatore3 = ISNULL((select max(ws_vivaiPassaporti_operazione_cod) from ws_VivaiPassaporti_Operazioni_log), 0) ")
            Stb.AppendLine("              ")
            Stb.AppendLine(" select top  1 @contatore = contatore ")
            Stb.AppendLine(" from ( ")
            Stb.AppendLine("  select @contatore as contatore ")
            Stb.AppendLine("  union all ")
            Stb.AppendLine("  select @contatore2 ")
            Stb.AppendLine("  union all ")
            Stb.AppendLine("  select @contatore3 ")
            Stb.AppendLine("  ")
            Stb.AppendLine("  ")
            Stb.AppendLine(" ) a ")
            Stb.AppendLine(" order by Contatore desc")


            Stb.AppendLine(" insert ws_VivaiPassaporti_Operazioni ( ")
            Stb.AppendLine("   ws_VivaiPassaporti_Operazione_cod ")
            Stb.AppendLine("  ,GIAS_Stato ")
            Stb.AppendLine("  ,Vivaista_PIVA ")
            Stb.AppendLine("  ,Rif_Distinta_Piva ")
            Stb.AppendLine("  ,Rif_Distinta_Progetto_Cod ")
            Stb.AppendLine("  ,Codice_RUOP ")
            Stb.AppendLine("  ,Rag_Soc_RUOP")
            Stb.AppendLine("  ,TipoZona ")
            Stb.AppendLine("  ,DDT_in_Numero ")
            Stb.AppendLine("  ,DDT_out_Numero ")
            Stb.AppendLine("  ,Doc_in_mov_dettagli_piva ")
            Stb.AppendLine("  ,Doc_in_mov_dettagli_sa_cod ")
            Stb.AppendLine("  ,Doc_in_mov_dettagli_id_agenda ")
            Stb.AppendLine("  ,Doc_in_mov_dettagli_id_mov ")
            Stb.AppendLine("  ,Doc_in_mov_dettagli_id_mov_det ")
            Stb.AppendLine("  ,Doc_out_mov_dettagli_piva ")
            Stb.AppendLine("  ,Doc_out_mov_dettagli_sa_cod ")
            Stb.AppendLine("  ,Doc_out_mov_dettagli_id_agenda ")
            Stb.AppendLine("  ,Doc_out_mov_dettagli_id_mov ")
            Stb.AppendLine("  ,Doc_out_mov_dettagli_id_mov_det ")
            Stb.AppendLine("  ,DataMovimento ")
            Stb.AppendLine("  ,PaeseDiOrigine ")
            Stb.AppendLine("  ,Causale ")
            Stb.AppendLine("  ,CaricoScarico ")
            Stb.AppendLine("  ,Origine ")
            Stb.AppendLine("  ,Destinazione ")
            Stb.AppendLine("  ,FornitoreAPO ")
            Stb.AppendLine("  ,Cul_COD ")
            Stb.AppendLine("  ,Grfri_Cod ")
            Stb.AppendLine("  ,Grva_Cod ")
            Stb.AppendLine("  ,elem_cod ")
            Stb.AppendLine("  ,mat_cod ")
            Stb.AppendLine("  ,cal_cod ")
            Stb.AppendLine("  ,Veg_Des_Lat ")
            Stb.AppendLine("  ,cul_Des ")
            Stb.AppendLine("  ,Prodotto_Des ")
            Stb.AppendLine("  ,Qta1 ")
            Stb.AppendLine("  ,Qta2 ")
            Stb.AppendLine("  ,Qta3 ")
            Stb.AppendLine("  ,Calibro ")
            Stb.AppendLine("  ,Lotto_1 ")
            Stb.AppendLine("  ,Lotto_2 ")
            Stb.AppendLine("  ,Coltivatore_codRisum ")
            Stb.AppendLine("  ,Coltivatore_RagioneSociale ")
            Stb.AppendLine("  ,Coltivatore_Indirizzo ")
            Stb.AppendLine("  ,Coltivatore_Cap ")
            Stb.AppendLine("  ,Coltivatore_Citta ")
            Stb.AppendLine("  ,Coltivatore_Regione ")
            Stb.AppendLine("  ,DestinazioneFinale_codRisum ")
            Stb.AppendLine("  ,DestinazioneFinale_RagioneSociale ")
            Stb.AppendLine("  ,DestinazioneFinale_Indirizzo ")
            Stb.AppendLine("  ,DestinazioneFinale_Cap ")
            Stb.AppendLine("  ,DestinazioneFinale_Citta ")
            Stb.AppendLine("  ,DestinazioneFinale_Regione ")
            Stb.AppendLine("  ,Preso_in_cosegna_Da ")
            Stb.AppendLine("  ,Note ")
            Stb.AppendLine("  ,SitoCodice ")
            Stb.AppendLine("  ,SitoRagioneSociale ")

            Stb.AppendLine("  ,inviato ")
            Stb.AppendLine("  ,datainvio ")
            Stb.AppendLine("  ,Data_Creazione ")
            Stb.AppendLine("  ,Data_Modifica ")
            Stb.AppendLine("  ,Username_Creazione ")
            Stb.AppendLine("  ,Username_Modifica ")
            Stb.AppendLine("  ,Validita_Inizio ")
            Stb.AppendLine("  ,Validita_Fine ")
            Stb.AppendLine("         )")
            Stb.AppendLine(" ")
            Stb.AppendLine("             select  ")
            Stb.AppendLine("  ")
            Stb.AppendLine("   DENSE_RANK() over (order by detProd.id_mov_det) + @Contatore as ws_VivaiPassaporti_Operazioni_cod  ")
            Stb.AppendLine(" , " & TipiEnumerativi.enum_WWorflow_WAnagraficaStati.Registro_Carico_Scarico_Passaporti_Vivaisti_DaConfermare & " as GIAS_Stato -- da confermare ")
            Stb.AppendLine(" , m.piva as Vivaista_PIVA ")
            Stb.AppendLine(" , 0 as Rif_Distinta_Piva ")
            Stb.AppendLine(" , 0 as Rif_Distinta_Progetto_Cod ")

            'TODO: come suggerito da Valerio, se non viene trovato il codice indico "HARDCODED" il codice Zespri!
            Stb.AppendLine(" , ISNULL(iRuop.Val_cod, 'IT-08-2809') as Codice_RUOP  ")
            Stb.AppendLine(" , impViv.rag_soc as rag_soc_ruop")

            Stb.AppendLine(" , 'PP' as TipoZona --TODO ")
            Stb.AppendLine("  ")

            'uso solo una colonna per il DDT
            'Stb.AppendLine(" , case when m.Cau_Mov = '" & cauMovDettagliIn & "' then  ")

            ' VAnni: 28/5/2020: evita di convertire il float in notazione esponenziale
            If cauMovTestataIn <> "" Then
                Stb.AppendLine("  , mTes.Doc_Numero_Sin + case when mTes.Doc_Numero_Sin <> '' then '/' else '' end + ")
                Stb.AppendLine("      REPLACE(RTRIM(REPLACE(REPLACE(RTRIM(REPLACE(CAST(CAST(mTes.Doc_Numero AS DECIMAL(18,9)) AS VARCHAR(20)),'0',' ')),' ','0'),'.',' ')),' ','.') + ")
                Stb.AppendLine("     case when mTes.Doc_Numero_Des <> '' then '/' else '' end + mTes.Doc_Numero_Des")
                Stb.AppendLine("   as DDT_in_Numero ")
            Else
                Stb.AppendLine("  , '0' as DDT_in_Numero ")
            End If


            'Stb.AppendLine("   else '' end as DDT_in_Numero ")
            'Stb.AppendLine("  ")
            'Stb.AppendLine(" , case when m.Cau_Mov = '" & cauMovDettagliOut & "' then  ")
            'Stb.AppendLine("  mTes.Doc_Numero_Sin + case when mTes.Doc_Numero_Sin <> '' then '/' else '' end + cast( mTes.Doc_Numero as varchar(100)) + case when mTes.Doc_Numero_Des <> '' then '/' else '' end + mTes.Doc_Numero_Des ")
            'Stb.AppendLine("   else '' end as DDT_out_Numero ")

            Stb.AppendLine(" , '' as DDT_out_Numero ")

            Stb.AppendLine(" , detProd.PIVA as Doc_in_mov_dettagli_piva  ")
            Stb.AppendLine(" , detProd.Sa_Cod as Doc_in_mov_dettagli_sa_cod  ")
            Stb.AppendLine(" , detProd.id_Agenda as Doc_in_mov_dettagli_id_agenda  ")
            Stb.AppendLine(" , detProd.Id_Mov as Doc_in_mov_dettagli_id_mov ")
            Stb.AppendLine(" , detProd.Id_Mov_Det as Doc_in_mov_dettagli_id_mov_det  ")

            Stb.AppendLine(" , '' as Doc_out_mov_dettagli_piva  ")
            Stb.AppendLine(" , 0  as Doc_out_mov_dettagli_sa_cod  ")
            Stb.AppendLine(" , 0  as Doc_out_mov_dettagli_id_agenda  ")
            Stb.AppendLine(" , 0  as Doc_out_mov_dettagli_id_mov ")
            Stb.AppendLine(" , 0  as Doc_out_mov_dettagli_id_mov_det  ")


            'Stb.AppendLine(" , case when m.Cau_Mov = '" & cauMovDettagliIn & "' then detProd.PIVA else ''  end as Doc_in_mov_dettagli_piva  ")
            'Stb.AppendLine(" , case when m.Cau_Mov = '" & cauMovDettagliIn & "' then detProd.Sa_Cod else 0  end as Doc_in_mov_dettagli_sa_cod  ")
            'Stb.AppendLine(" , case when m.Cau_Mov = '" & cauMovDettagliIn & "' then detProd.id_Agenda else 0 end as Doc_in_mov_dettagli_id_agenda  ")
            'Stb.AppendLine(" , case when m.Cau_Mov = '" & cauMovDettagliIn & "' then detProd.Id_Mov else 0  end as Doc_in_mov_dettagli_id_mov ")
            'Stb.AppendLine(" , case when m.Cau_Mov = '" & cauMovDettagliIn & "' then detProd.Id_Mov_Det else 0 end as Doc_in_mov_dettagli_id_mov_det  ")
            'Stb.AppendLine("  ")
            'Stb.AppendLine(" , case when m.Cau_Mov = '" & cauMovDettagliOut & "' then detProd.PIVA else ''  end as Doc_out_mov_dettagli_piva  ")
            'Stb.AppendLine(" , case when m.Cau_Mov = '" & cauMovDettagliOut & "' then detProd.Sa_Cod else 0  end as Doc_out_mov_dettagli_sa_cod  ")
            'Stb.AppendLine(" , case when m.Cau_Mov = '" & cauMovDettagliOut & "' then detProd.id_Agenda else 0 end as Doc_out_mov_dettagli_id_agenda  ")
            'Stb.AppendLine(" , case when m.Cau_Mov = '" & cauMovDettagliOut & "' then detProd.Id_Mov else 0  end as Doc_out_mov_dettagli_id_mov ")
            'Stb.AppendLine(" , case when m.Cau_Mov = '" & cauMovDettagliOut & "' then detProd.Id_Mov_Det else 0 end as Doc_out_mov_dettagli_id_mov_det  ")
            Stb.AppendLine("  ")

            If cauMovTestataIn <> "" Then
                Stb.AppendLine(" , mTes.ora as DataMovimento ")
            Else
                Stb.AppendLine(" , a.validita_inizio as DataMovimento ")
            End If


            Stb.AppendLine("  ")

            If vListaParametriQualitativi.Contains("Provenienza") Then
                Stb.AppendLine(" , isNull(otpProvenienza.sigla, ii.stato) as PaeseDiOrigine ")
            Else
                Stb.AppendLine(" , ii.stato as PaeseDiOrigine ")
            End If

            'Stb.AppendLine("  , case when a.Lav_Cod = 5000 then 'Bud Processing' else  ")
            'Stb.AppendLine("  case when con.piva <> con.cod_Contatto and a.lav_cod = 1031 then 'Bud Allocation' else ")
            'Stb.AppendLine("      'Bud Transfer' ")
            'Stb.AppendLine("  end end as Causale ")
            RiportaDocumentiGiasQryTipoMov(Stb, TipoColonnaRegistro.Causale)
            RiportaDocumentiGiasQryTipoMov(Stb, TipoColonnaRegistro.CaricoScarico)
            'Stb.AppendLine("  ")
            'Stb.AppendLine("  , case when a.Lav_Cod = 5000 then 'Loading' else  ")
            'Stb.AppendLine("  case when con.piva <> con.cod_Contatto and a.lav_cod = 1031 then 'Delivering' else ")
            'Stb.AppendLine("      'Transport' ")
            'Stb.AppendLine("  end end as  CaricoScarico")

            'Stb.AppendLine(" , '' as Origine ")
            RiportaDocumentiGiasQryTipoMov(Stb, TipoColonnaRegistro.Origine_Departure)

            'If vListaLavCodOperazioni.Contains("5000") Then
            '    Stb.AppendLine(" , impViv.rag_soc as Origine ")
            'Else
            '    Stb.AppendLine(" , con.Rag_Soc as Origine ")
            'End If

            RiportaDocumentiGiasQryTipoMov(Stb, TipoColonnaRegistro.Destinazione)

            Stb.AppendLine(" , '' as FornitoreAPO --TODO")

            Stb.AppendLine(" ")
            Stb.AppendLine(" , c.cul_cod as Cul_COD ")
            Stb.AppendLine("  ,0 as Grfri_Cod --TODO ")
            Stb.AppendLine("  ,0 as Grva_Cod --TODO ")
            Stb.AppendLine("  ")
            Stb.AppendLine("  ,detProd.elem_cod ")
            Stb.AppendLine("  ,detProd.mat_cod ")
            Stb.AppendLine("  ,detProd.cal_cod ")
            Stb.AppendLine("  ")
            Stb.AppendLine("  ,veg.Veg_Des + ' ' + c.Cul_Des as Veg_Des_Lat ")
            Stb.AppendLine("  ,c.Cul_Des as cul_Des ")
            Stb.AppendLine("  ,m1.Mat_Des as Prodotto_Des ")
            Stb.AppendLine("  ")
            Stb.AppendLine("  ,detProd.Qta_Dettaglio2 as Qta1 --TODO ") 'bins
            Stb.AppendLine("  ,0 as  Qta2 ") 'packaging
            'Stb.AppendLine("  ,detProd.Qta as Qta3 --TODO ")  'buds
            RiportaDocumentiGiasQryTipoMov(Stb, TipoColonnaRegistro.QtaBUDS)

            Stb.AppendLine("  ")

            If Not vListaParametriQualitativi.Contains("Calibro") Then
                Stb.AppendLine("  , '' as Calibro ")
            Else
                Stb.AppendLine("  , otpCalibro.sigla as Calibro ")
            End If

            Stb.AppendLine("  , detProd.Lotto as Lotto_1 ")
            Stb.AppendLine("  , '' as Lotto_2 ")
            Stb.AppendLine("  ")
            Stb.AppendLine("  , case when con.piva <> con.cod_Contatto and a.lav_cod = 1031 then risum.cod_Risum else 0 end as Dest_codRisum  ")

            RiportaDocumentiGiasQryTipoMov(Stb, TipoColonnaRegistro.Dest_RagioneSociale)
            RiportaDocumentiGiasQryTipoMov(Stb, TipoColonnaRegistro.Dest_Indirizzo)
            RiportaDocumentiGiasQryTipoMov(Stb, TipoColonnaRegistro.Dest_CAP)
            RiportaDocumentiGiasQryTipoMov(Stb, TipoColonnaRegistro.Dest_Citta)

            'Stb.AppendLine("  , case when con.piva <> con.cod_Contatto and a.lav_cod = 1031 then con.rag_soc else '' end as Dest_RagioneSociale   ")
            'Stb.AppendLine("  , case when con.piva <> con.cod_Contatto and a.lav_cod = 1031 then ii.ind_des else '' end as Dest_Indirizzo   ")
            'Stb.AppendLine("  , case when con.piva <> con.cod_Contatto and a.lav_cod = 1031 then ii.cap else '' end  as Dest_Cap   ")
            'Stb.AppendLine("  , case when con.piva <> con.cod_Contatto and a.lav_cod = 1031 then ii.frz_Des else '' end as Dest_Citta   ")

            RiportaDocumentiGiasQryTipoMov(Stb, TipoColonnaRegistro.Dest_Regione)

            Stb.AppendLine("  , case when conDestinazione.piva <> conDestinazione.cod_Contatto and a.lav_cod = 1031 then risumDestinazione.cod_Risum else 0 end as DestinazioneFinale_codRisum  ")

            RiportaDocumentiGiasQryTipoMov(Stb, TipoColonnaRegistro.DestFin_RagioneSociale)
            RiportaDocumentiGiasQryTipoMov(Stb, TipoColonnaRegistro.DestFin_Indirizzo)
            RiportaDocumentiGiasQryTipoMov(Stb, TipoColonnaRegistro.DestFin_CAP)
            RiportaDocumentiGiasQryTipoMov(Stb, TipoColonnaRegistro.DestFin_Citta)
            RiportaDocumentiGiasQryTipoMov(Stb, TipoColonnaRegistro.DestFin_Regione)

            'Stb.AppendLine("  , '' as Coltivatore_Regione --TODO")
            Stb.AppendLine("  , '' as Preso_in_cosegna_Da --TODO ")
            Stb.AppendLine("  , '' as Note  --TODO ")

            RiportaDocumentiGiasQryTipoMov(Stb, TipoColonnaRegistro.SitoCodice)
            RiportaDocumentiGiasQryTipoMov(Stb, TipoColonnaRegistro.SitoRagioneSociale)

            'Stb.AppendLine("  , Risum.Settore_Des as SitoCodice")
            'Stb.AppendLine("  , con.Rag_Soc as SitoRagioneSociale ")


            Stb.AppendLine("  ")

            ''backup 2020-05-22 10:30
            'If RecuperaRigheRegistro Then
            If ImpostaFlagInviato Then
                Stb.AppendLine("  ,-1 as inviato ")
            Else
                Stb.AppendLine("  ,0 as inviato ")
            End If

            Stb.AppendLine("  ,null as datainvio ")
            Stb.AppendLine("  ,getdate() as Data_Creazione ")
            Stb.AppendLine("  ,getdate() as Data_Modifica ")
            Stb.AppendLine("  ,'' as Username_Creazione ")
            Stb.AppendLine("  ,'' as Username_Modifica ")
            Stb.AppendLine("  , " & Agro_SQL_SaveDate(AGRODATAINIZIO) & " as Validita_Inizio ")
            Stb.AppendLine("  ," & Agro_SQL_SaveDate(AGRODATAFINE) & " as Validita_Fine ")
            Stb.AppendLine("   ")
            Stb.AppendLine("  --, detProd.id_agenda ")
            Stb.AppendLine("  ")
            Stb.AppendLine(" from movimenti_Dettagli detProd ")
            Stb.AppendLine("  inner join movimenti m ")
            Stb.AppendLine("      on detProd.Id_Mov = m.Id_Mov ")
            Stb.AppendLine("      and detProd.piva = m.piva  ")

            ' VAnni: 19/6/2020: verificare se cauMovTestataIn è stringa vuota (caso traferimenti magazzino non ha testata ... (..left o sopprimi))
            If cauMovTestataIn <> "" Then
                Stb.AppendLine("  inner join movimenti mTes ")
            Else
                Stb.AppendLine("  left join movimenti mTes ")
            End If

            Stb.AppendLine("      on detProd.Id_agenda = mTes.Id_Agenda ")
            Stb.AppendLine("      and detProd.piva = mTes.piva  ")
            Stb.AppendLine("      and mTes.cau_mov = '" & cauMovTestataIn & "' ")
            Stb.AppendLine("  inner join Agenda a  ")
            Stb.AppendLine("      on a.id_agenda = m.id_Agenda  ")
            Stb.AppendLine("  inner join materie_prime m1 ")
            Stb.AppendLine("      on m1.elem_cod = detProd.Elem_Cod  ")
            Stb.AppendLine("      and m1.Mat_Cod = detProd.Mat_Cod ")
            Stb.AppendLine("  inner join SpecieVegetali veg ")
            Stb.AppendLine("      on veg.Veg_Cod = m1.Veg_Cod ")
            Stb.AppendLine("  inner join Cultivar c ")
            Stb.AppendLine("      on c.Cul_Cod = m1.Cul_Cod ")
            Stb.AppendLine("  inner join imprese impViv ")
            Stb.AppendLine("     on impViv.piva = a.piva ")



            Stb.AppendLine(" LEFT JOIN Mov_Dettagli_Riferimenti Trasf_Rif ")
            Stb.AppendLine("     ON detProd.Id_Mov_Det=(CASE m.Cau_Mov WHEN '7350' THEN Trasf_Rif.Id_Mov_Det WHEN '7300' THEN Trasf_Rif.Id_Mov_Det_Rif ELSE -1 END) ")
            Stb.AppendLine("     AND Trasf_Rif.Lav_Cod = 1033 AND Trasf_Rif.Lav_Cod_Rif = 1033 ")
            Stb.AppendLine("     AND Trasf_Rif.Cau_Mov = '7350' AND Trasf_Rif.Cau_Mov_Rif = '7300' ")

            Stb.AppendLine(" -- Contiene la Cella di Carico (se il mio dettaglio principale è lo Scarico) oppure la cella di Scarico (se il mio dettaglio principale è lo scarico) ")
            Stb.AppendLine(" LEFT JOIN Mov_Destinazioni Trasf_Dest_2 ON ")
            Stb.AppendLine("                   ( Trasf_Rif.Piva = Trasf_Dest_2.Piva AND ")
            Stb.AppendLine("                     Trasf_Rif.Id_Agenda = Trasf_Dest_2.Id_Agenda AND ")
            Stb.AppendLine("   				    (CASE m.Cau_Mov WHEN '7350' THEN Trasf_Rif.Id_Mov_Rif WHEN '7300' THEN Trasf_Rif.Id_Mov ELSE -1 END) = Trasf_Dest_2.Id_Mov AND ")
            Stb.AppendLine("                     (CASE m.Cau_Mov WHEN '7350' THEN Trasf_Rif.Id_Mov_Det_Rif WHEN '7300' THEN Trasf_Rif.Id_Mov_Det ELSE -1 END) = Trasf_Dest_2.Id_Mov_Det ")
            Stb.AppendLine("                     ) ")

            Stb.AppendLine(" LEFT JOIN Centri_Aziendali Trasf_sa_2 ")
            Stb.AppendLine(" ON Trasf_dest_2.Piva = Trasf_sa_2.PIVA ")
            Stb.AppendLine(" AND Trasf_Dest_2.Sa_Cod = Trasf_sa_2.SA_COD ")


            Stb.AppendLine("  left join (  ")
            Stb.AppendLine("    select piva, sa_Cod, min(cod_indirizzo) as cod_indirizzo  ")
            Stb.AppendLine("    from centrixindirizzi  ")
            Stb.AppendLine("    group by piva, sa_Cod ")
            Stb.AppendLine("  ) ciTrasf_sa_2")


            Stb.AppendLine("      on ciTrasf_sa_2.piva = Trasf_sa_2.piva  ")
            Stb.AppendLine("      and ciTrasf_sa_2.sa_cod = Trasf_sa_2.sa_cod")


            Stb.AppendLine("  left join indirizzi saiiTrasf_sa_2 ")
            Stb.AppendLine("      on saiiTrasf_sa_2.cod_indirizzo = ciTrasf_sa_2.cod_indirizzo")


            Stb.AppendLine("  left join ( ")
            Stb.AppendLine("      select piva, min(sa_cod) as sa_cod ")
            Stb.AppendLine("   from mov_Destinazioni ")
            Stb.AppendLine("   group by piva ")
            Stb.AppendLine("  ) movDest ")
            Stb.AppendLine("    on movDest.piva = a.piva ")

            Stb.AppendLine("  left join Centri_Aziendali sa ")
            Stb.AppendLine("      on sa.piva = detProd.piva  ")
            Stb.AppendLine("      and sa.sa_cod = detProd.sa_cod ")

            ' VAnni: 30/3/2020: TODO: Rivedere, a garanzia di univocità del dato prendo il primo indirizzo ... 
            Stb.AppendLine("  left join (  ")
            Stb.AppendLine("    select piva, sa_Cod, min(cod_indirizzo) as cod_indirizzo  ")
            Stb.AppendLine("    from centrixindirizzi  ")
            Stb.AppendLine("    group by piva, sa_Cod ")
            Stb.AppendLine("  ) ci")

            Stb.AppendLine("      on ci.piva = sa.piva  ")
            Stb.AppendLine("      and ci.sa_cod = sa.sa_cod")

            Stb.AppendLine("  left join indirizzi saii ")
            Stb.AppendLine("      on saii.cod_indirizzo = ci.cod_indirizzo")

            'deduco i dati di indirizzo da centro aziendale via codice sito che si trova nelle due tabelle (centri_codici e risorse umane)
            Stb.AppendLine("  left join Centri_Aziendali_Codici sac ")
            Stb.AppendLine("      on sac.piva = sa.piva  ")
            Stb.AppendLine("      and sac.sa_cod = sa.sa_cod ")
            Stb.AppendLine("      and sac.id_cod = " & enum_CodiciAnagrafe.Codice_Sito_Vivaio)

            Stb.AppendLine("  left join Risorse_Umane risumDaCentri ")
            Stb.AppendLine("      on risumDaCentri.Settore_Des = sac.val_cod ")

            Stb.AppendLine("  left join contatti cDaCentri ")
            Stb.AppendLine("      on cDaCentri.piva = risumDaCentri.piva ")
            Stb.AppendLine("      and cDaCentri.Cod_Contatto = risumDaCentri.Cod_Contatto")


            If Not String.IsNullOrEmpty(ListaParametriQualitativi) Then
                For Each paramQ In vListaParametriQualitativi
                    AgronicaCoreStampeDAL.FF_Etichette_R.leggiParametriOmniFF_FROM(paramQ, Stb)
                Next
            End If

            Stb.AppendLine(" --lettura codice roup  ")
            Stb.AppendLine("  left join imprese_codici iRuop ")
            Stb.AppendLine("      on iRuop.piva = a.piva ")
            Stb.AppendLine("      and iRuop.id_cod = " & enum_CodiciAnagrafe.codice_RUOP)
            Stb.AppendLine(" ")

            Stb.AppendLine(" --lettura codice sito  ")
            Stb.AppendLine("  left join Risorse_Umane Risum ")
            Stb.AppendLine("      on risum.Cod_RisUm = mTes.Cod_RisUm  ")
            Stb.AppendLine("   ")


            Stb.AppendLine("  left join contatti con ")
            Stb.AppendLine("             on con.Piva = risum.Piva ")
            Stb.AppendLine("                  and con.Cod_Contatto = Risum.Cod_Contatto")

            Stb.AppendLine("  left join Indirizzi ii ")
            Stb.AppendLine("      on ii.cod_indirizzo = mTes.Cod_IndirizzoRisUm ")
            Stb.AppendLine("  ")

            Stb.AppendLine(" left join istat iii ")
            Stb.AppendLine("      on iii.prov = ii.pro_cod_istat ")
            Stb.AppendLine("      and iii.com = ii.com_cod_istat ")
            Stb.AppendLine("  ")


            Stb.AppendLine("     left join lista_province lp ")
            Stb.AppendLine("      on lp.sigla = ii.pro_cod ")

            Stb.AppendLine("     left join lista_Regioni lr ")
            Stb.AppendLine("      on lr.reg = lp.reg ")

            'Destinazione diversa
            Stb.AppendLine("  left join Risorse_Umane RisumDestinazione ")
            Stb.AppendLine("      on RisumDestinazione.Cod_RisUm = mTes.Cod_Destinazione  ")
            Stb.AppendLine("   ")


            Stb.AppendLine("  left join contatti conDestinazione ")
            Stb.AppendLine("             on conDestinazione.Piva = risumDestinazione.Piva ")
            Stb.AppendLine("                  and conDestinazione.Cod_Contatto = RisumDestinazione.Cod_Contatto")

            Stb.AppendLine("  left join Indirizzi iiDestinazione ")
            Stb.AppendLine("      on iiDestinazione.cod_indirizzo = mTes.Cod_IndirizzoDestinazione ")
            Stb.AppendLine("  ")

            Stb.AppendLine("     left join istat iiiDestinazione ")
            Stb.AppendLine("      on iiiDestinazione.prov = iiDestinazione.pro_cod_istat ")
            Stb.AppendLine("      and iiiDestinazione.com = iiDestinazione.com_cod_istat")

            Stb.AppendLine("     left join lista_province lpDestinazione ")
            Stb.AppendLine("      on lpDestinazione.sigla = iiDestinazione.pro_cod ")

            Stb.AppendLine("     left join lista_Regioni lrDestinazione ")
            Stb.AppendLine("      on lrDestinazione.reg = lpDestinazione.reg ")

            ' i contatti codici 
            'Stb.AppendLine("  left join Contatti_Codici cc ")
            'Stb.AppendLine("      on cc.piva = risum.piva  ")
            'Stb.AppendLine("      and cc.Cod_Contatto = risum.Cod_Contatto ")
            'Stb.AppendLine("      and cc.Id_cod =  " & enum_CodiciAnagrafe.codice_RUOP)
            'Stb.AppendLine("  ")

            Stb.AppendLine(" where m1.elem_cod in ( " & Agro_SQL_Save_Clausola_IN(FiltroElemCod) & " )")
            Stb.AppendLine(" and m.Cau_Mov in ( '" & cauMovDettagliIn & "', '" & cauMovDettagliOut & "' ) ")
            Stb.AppendLine(" and detProd.piva = '" & Agro_SQL_SaveText(piva) & "' ")
            Stb.AppendLine(" and a.lav_cod in (" & Agro_SQL_Save_Clausola_IN(listaLavCodOperazioni) & ")  ")

            'nelle lavorazione occorre scartare le testate.
            Stb.AppendLine(" and (( a.lav_cod = 5000 and detProd.jolly_int <> 1) or a.lav_Cod <> 5000)")

            Stb.AppendLine(" --filtro sui dettagli ")
            Stb.AppendLine(" --and detProd.Id_Mov_Det in () ")

            'gestione di movimenti già marcati come "DEL" in tabella ws_VivaiPassaporti_Operazioni_LOG
            Stb.AppendLine(" and   ")
            If RecuperaRigheRegistro Then
                Stb.AppendLine("  exists ( ")
            Else
                Stb.AppendLine("  not exists ( ")
            End If

            Stb.AppendLine("  select 1 ")
            Stb.AppendLine("  from ws_VivaiPassaporti_Operazioni_log reg ")
            Stb.AppendLine("  where   ")
            Stb.AppendLine("      reg.[Doc_in_mov_dettagli_piva] = detProd.[piva] ")
            Stb.AppendLine("  and reg.[Doc_in_mov_dettagli_id_agenda]  = detProd.[id_agenda] ")
            Stb.AppendLine("  and reg.[Doc_in_mov_dettagli_id_mov]  = detProd.[id_mov]  ")
            Stb.AppendLine("  and reg.[Doc_in_mov_dettagli_id_mov_det] = detProd.[id_mov_det] ")

            If RecuperaRigheRegistro Then
                Stb.AppendLine("  and reg.Gestita = 0 ")
            End If

            Dim filtroRighe As String = ""
            If RecuperaRigheRegistro Then
                filtroRighe = "'DEL', 'MODREG'"
            Else
                filtroRighe = "'DEL', 'MOD', 'PEN', 'MODREG'"
            End If

            Stb.AppendLine("  and reg.Azione in (" & filtroRighe & ") ")
            Stb.AppendLine("  ) ") 'not exists

            If Not RecuperaRigheRegistro Then

                'movimenti nuovi
                Stb.AppendLine(" and ( ")
                Stb.AppendLine("  not exists ( ")

                Stb.AppendLine("  select 1 ")
                Stb.AppendLine("  from  ws_VivaiPassaporti_Operazioni reg ")
                Stb.AppendLine("  where  ")
                Stb.AppendLine("          reg.Doc_in_mov_dettagli_id_agenda = detProd.id_Agenda ")
                Stb.AppendLine("      and reg.Doc_in_mov_dettagli_id_mov = detProd.Id_Mov ")
                Stb.AppendLine("      and reg.Doc_in_mov_dettagli_id_mov_det = detProd.Id_Mov_Det ")
                Stb.AppendLine("      and reg.Doc_in_mov_dettagli_piva = detProd.PIVA ")
                'Stb.AppendLine("      and reg.Doc_in_mov_dettagli_sa_cod = detProd.Sa_Cod ")
                Stb.AppendLine("      and reg.data_modifica >= detProd.data_modifica")
                Stb.AppendLine("     ") 'where
                Stb.AppendLine("  ) ") 'not exists

                'movimenti già presenti ma con data modifica di registro meno recente

                If RecuperaMovimentiRecentiRegistro Then
                    Stb.AppendLine("  or exists (")
                    Stb.AppendLine("  select 1 ")
                    Stb.AppendLine("  from  ws_VivaiPassaporti_Operazioni reg ")
                    Stb.AppendLine("  where  ")
                    Stb.AppendLine("              reg.Doc_in_mov_dettagli_id_agenda = detProd.id_Agenda ")
                    Stb.AppendLine("      and reg.Doc_in_mov_dettagli_id_mov = detProd.Id_Mov ")
                    Stb.AppendLine("      and reg.Doc_in_mov_dettagli_id_mov_det = detProd.Id_Mov_Det ")
                    Stb.AppendLine("      and reg.Doc_in_mov_dettagli_piva = detProd.PIVA ")
                    Stb.AppendLine("      and reg.data_modifica < detProd.data_modifica")
                    Stb.AppendLine("     ") 'where
                    Stb.AppendLine("  ) ") 'not exists
                    'Stb.AppendLine("      and reg.Doc_in_mov_dettagli_sa_cod = detProd.Sa_Cod ")
                End If

                Stb.AppendLine(" )") ' clausola and


            End If 'filtro solo per aggiornamento e non per il recupero righe

            If xFiltroAggiuntivo <> "" Then
                Stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp
    End Function

    Public Function ConfermaVoceRegistro(piva As String, listaOperazioni As String, xFiltroAggiuntivo As String, objParametri As AgronicaCoreParametri) As Boolean

        '----- Descrizione
        Dim NomeRoutine As String = "ConfermaVoceRegistro()"

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            Stb.Length = 0

            '---------------------------------------------

            Stb.Append(" UPDATE    ws_VivaiPassaporti_Operazioni ")
            Stb.Append(" SET ")
            Stb.Append("  GIAS_Stato = " & enum_WWorflow_WAnagraficaStati.Registro_Carico_Scarico_Passaporti_Vivaisti_Confermato)
            Stb.Append(" ,Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            'Stb.Append(" ,Data_Modifica= " & Agro_SQL_SaveDateTime(Date.Now) & " ")
            Stb.Append(" WHERE   ws_VivaiPassaporti_Operazione_cod in (" & Agro_SQL_Save_Clausola_IN(listaOperazioni) & ") ")


            If xFiltroAggiuntivo <> "" Then
                Stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp

    End Function
End Class


