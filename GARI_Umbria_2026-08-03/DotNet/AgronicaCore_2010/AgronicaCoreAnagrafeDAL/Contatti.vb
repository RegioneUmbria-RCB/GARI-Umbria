Imports System.Data.Entity
Imports System.Globalization
Imports System.Text
Imports System.Transactions
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.DataProviderExtensions
Imports AgronicaCoreDataProvider.My.Resources
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO
Imports Newtonsoft.Json

Public Class Contatti_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Check_SettoreDes(ByVal Piva As String,
                                                ByVal New_Cod_Contatto As String,
                                                ByVal Old_Cod_Contatto As String,
                                                ByVal Settore_des As String,
                                                ByVal CodContatto As String,
                                                ByRef objParametri As AgronicaCoreParametri
                                                ) As String


        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Contatti_R.Check_SettoreDes()"
        Dim StbSQL As New StringBuilder
        Dim MessaggioErrore As String = ""
        Dim risposta As String = String.Empty

        Try

            StbSQL.Length = 0
            StbSQL.Append(" SELECT DISTINCT top 1 Contatti.Rag_Soc + Contatti.Cognome + ' ' + Contatti.Nome AS Rag_Soc   " & vbCrLf)
            StbSQL.Append(" FROM  Contatti  INNER JOIN Risorse_Umane ON " & vbCrLf)
            StbSQL.Append(" Risorse_Umane.Cod_Contatto = Contatti.Cod_Contatto " & vbCrLf)
            StbSQL.Append(" AND Risorse_Umane.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " " & vbCrLf)
            StbSQL.Append(" AND Risorse_Umane.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " " & vbCrLf)
            StbSQL.Append(" AND   Risorse_Umane.Cod_Contatto = Contatti.Cod_Contatto  " & vbCrLf)
            StbSQL.Append(" WHERE ( Contatti.Piva = '" & Agro_SQL_SaveText(Piva) & "'  OR  Contatti.Sa_Cod = -1 )  " & vbCrLf)
            StbSQL.Append(" AND (Contatti.Cod_Contatto != '" & Agro_SQL_SaveText(CodContatto) & "'  ) " & vbCrLf)
            StbSQL.Append(" AND (Contatti.Piva = Risorse_Umane.Piva  OR  Contatti.Sa_Cod = -1 ) " & vbCrLf)
            StbSQL.Append(" AND Upper( Contatti.Cod_Contatto) <> '" & Agro_SQL_SaveText(New_Cod_Contatto) & "' " & vbCrLf)
            StbSQL.Append(" AND Upper( Contatti.Cod_Contatto) <> '" & Agro_SQL_SaveText(Old_Cod_Contatto) & "' " & vbCrLf)
            StbSQL.Append(" And Upper(Settore_Des) = '" & Agro_SQL_SaveText(Settore_des) & "' " & vbCrLf)


            Dim dt As DataTable = EseguiQuery_Lettura(objParametri, StbSQL.ToString(), NomeRoutine)
            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                risposta = dt.Rows(0).Item("Rag_Soc").ToString()
            End If

            Return risposta

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

    End Function

    Public Function ContattoRiferimenti(ByVal piva As String,
                                        ByVal Cod_Contatto As String,
                                        ByVal Cod_RisUm As Integer,
                                        ByVal Sa_Cod As Integer,
                                        ByVal xFiltroAggiuntivo As String,
                                        ByRef objParametri As AgronicaCoreParametri
                                        ) As DataTable

        Const NomeRoutine = "AgronicaCoreAnagrafeDAL.Contatti_R.ContattoRiferimenti()"

        Dim MessaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim DT As DataTable
        Dim risp As Boolean = False

        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False

        Try

            'Verifico se c'è una connessione e nel caso la utilizzo, altrimenti ne apro una "locale" insieme ad una transazione
            ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale, FlagTransazioneLocale, objParametri)

            ' Creazione tabella temporanea
            stb.AppendLine(" Create Table #temp_risUm ( ")
            stb.AppendLine(" Piva nvarchar(25) COLLATE SQL_Latin1_General_CP850_CI_AS NULL,  ")
            stb.AppendLine(" sa_cod int NULL,  ")
            stb.AppendLine(" cod_risum int NULL,  ")
            stb.AppendLine(" cod_contatto nvarchar(25) COLLATE SQL_Latin1_General_CP850_CI_AS  NULL )")
            EseguiQuery_Scrittura(objParametri, stb.ToString(), NomeRoutine)
            stb.Clear()

            ' Creo tabella temporanea invece di usare ctl per sicurezza di compatibilità
            'stb.AppendLine(" Select Risorse_Umane.Piva, Risorse_Umane.Sa_Cod, ")
            'stb.AppendLine(" Risorse_Umane.Cod_RisUm, Risorse_Umane.Cod_Contatto")
            'stb.AppendLine(" INTO #temp_risUm ")
            'stb.AppendLine("  From Risorse_Umane , Contatti")
            'stb.AppendLine(" Where Contatti.Piva = Risorse_Umane.Piva ")
            'stb.AppendLine(" And Contatti.Cod_Contatto = Risorse_Umane.Cod_Contatto")
            'stb.AppendLine(" And Risorse_Umane.Validita_inizio <= " & Agro_SQL_SaveDate(AGRODATAFINE) & " AND ")
            'stb.AppendLine(" Risorse_Umane.Validita_Fine >=  " & Agro_SQL_SaveDate(AGRODATAINIZIO) & " AND ")
            'stb.AppendLine(" Risorse_Umane.Inviato >= 0 And Contatti.Inviato >= 0 ")

            stb.AppendLine(" INSERT INTO #temp_risUm ")
            stb.AppendLine(" Select Risorse_Umane.Piva, Risorse_Umane.Sa_Cod, ")
            stb.AppendLine(" Risorse_Umane.Cod_RisUm, Risorse_Umane.Cod_Contatto")
            stb.AppendLine("  From Risorse_Umane , Contatti")
            stb.AppendLine(" Where Contatti.Piva = Risorse_Umane.Piva ")
            stb.AppendLine(" And Contatti.Cod_Contatto = Risorse_Umane.Cod_Contatto")
            stb.AppendLine(" And Risorse_Umane.Validita_inizio <= " & Agro_SQL_SaveDate(AGRODATAFINE) & " AND ")
            stb.AppendLine(" Risorse_Umane.Validita_Fine >=  " & Agro_SQL_SaveDate(AGRODATAINIZIO) & " AND ")
            stb.AppendLine(" Risorse_Umane.Inviato >= 0 And Contatti.Inviato >= 0 ")


            If Not String.IsNullOrEmpty(piva) Then
                stb.AppendLine(" AND Risorse_Umane.piva = '" & Agro_SQL_SaveText(piva) & "' ")
            End If
            If Not String.IsNullOrEmpty(Cod_Contatto) Then
                stb.AppendLine(" AND Risorse_Umane.cod_contatto = '" & Agro_SQL_SaveText(Cod_Contatto) & "' ")
            End If
            If Cod_RisUm <> 0 Then
                stb.AppendLine(" AND Risorse_Umane.cod_risUm = " & Agro_SQL_SaveNum(Cod_RisUm) & " ")
            End If

            risp = EseguiQuery_Scrittura(objParametri, stb.ToString(), NomeRoutine)

            'stb.AppendLine("With RisorseUmane(piva, Sa_Cod, Cod_RisUm, Cod_Contatto) ")
            'stb.AppendLine(" AS ( ")
            'stb.AppendLine(" Select Risorse_Umane.Piva, Risorse_Umane.Sa_Cod, ")
            'stb.AppendLine(" Risorse_Umane.Cod_RisUm, Risorse_Umane.Cod_Contatto")
            'stb.AppendLine("  From Risorse_Umane , Contatti")
            'stb.AppendLine(" Where Contatti.Piva = Risorse_Umane.Piva ")
            'stb.AppendLine(" And Contatti.Cod_Contatto = Risorse_Umane.Cod_Contatto")
            'stb.AppendLine(" And Risorse_Umane.Validita_inizio <= " & Agro_SQL_SaveDate(AGRODATAFINE) & " AND ")
            'stb.AppendLine(" Risorse_Umane.Validita_Fine >=  " & Agro_SQL_SaveDate(AGRODATAINIZIO) & " AND ")
            'stb.AppendLine(" Risorse_Umane.Inviato >= 0 And Contatti.Inviato >= 0 ")

            stb.Clear()
            stb.AppendLine(" Select distinct  a.*, Imprese.rag_soc  from ( ")

            ' Checks per Cod_RisUm
            stb.AppendLine(" -- intestazione documenti ")
            stb.AppendLine($" Select ru.Piva As PivaOrigine, mov.piva As PivaDest, ru.Cod_RisUm, ru.Cod_Contatto, '{Gias.DocContab} - {Gias.Intestatario}' as Scopo ")
            stb.AppendLine(" From #temp_risUm ru ")
            stb.AppendLine(" inner Join movimenti mov ")
            stb.AppendLine(" On ru.Cod_RisUm = mov.Cod_RisUm ")
            stb.AppendLine(" -- vettore documenti ")
            stb.AppendLine(" union ")
            stb.AppendLine($" Select ru.Piva As PivaOrigine, mov.piva As PivaDest, ru.Cod_RisUm, ru.Cod_Contatto, '{Gias.DocContab} - {Gias.Vettore}' as Scopo ")
            stb.AppendLine(" From #temp_risUm ru ")
            stb.AppendLine(" inner Join movimenti mov ")
            stb.AppendLine(" On ru.Cod_RisUm = mov.cod_vettore ")
            stb.AppendLine(" -- destinazione diversa documenti ")
            stb.AppendLine(" union ")
            stb.AppendLine($" Select ru.Piva As PivaOrigine, mov.piva As PivaDest, ru.Cod_RisUm, ru.Cod_Contatto, '{Gias.DocContab} - {Gias.DestinazioneDiversa}' as Scopo ")
            stb.AppendLine(" From #temp_risUm ru ")
            stb.AppendLine(" inner Join movimenti mov ")
            stb.AppendLine(" On ru.Cod_RisUm = mov.Cod_Destinazione ")
            stb.AppendLine(" -- cod_risum_altro documenti ")
            stb.AppendLine(" union ")
            stb.AppendLine($" Select ru.Piva As PivaOrigine, mov.piva As PivaDest, ru.Cod_RisUm, ru.Cod_Contatto, '{Gias.Conferimento} - {Gias.PrimoCedente} (CodRisUMAltro)' as Scopo ")
            stb.AppendLine(" From #temp_risUm ru ")
            stb.AppendLine(" inner Join movimenti mov ")
            stb.AppendLine(" On ru.Cod_RisUm = mov.Cod_RisUm_Altro ")
            stb.AppendLine(" -- Extra_Int per Cedente 2 documenti ")
            stb.AppendLine(" union ")
            stb.AppendLine($" Select ru.Piva As PivaOrigine, mov.piva As PivaDest, ru.Cod_RisUm, ru.Cod_Contatto, '{Gias.Conferimento} - {Gias.SecondoCedente} (Extra_Int)' as Scopo ")
            stb.AppendLine(" From #temp_risUm ru ")
            stb.AppendLine(" inner Join movimenti mov ")
            stb.AppendLine(" On ru.Cod_RisUm = mov.Extra_Int ")
            stb.AppendLine(" inner Join agenda age ")
            stb.AppendLine(" On age.Piva = mov.Piva ")
            stb.AppendLine(" And age.Id_Agenda = mov.Id_Agenda ")
            stb.AppendLine(" where age.Lav_Cod IN (" & LAVCOD_ACCETTAZIONE_DIVERSI & ", " & LAVCOD_BOLLA_RICEVUTA & ", " &
                        LAVCOD_DISTINTA_CARICO & ", " & LAVCOD_DISTINTA_CARICO_ACCETTAZIONE & ", " &
                        LAVCOD_AUTO_DDT_EMESSO & ", " & LAVCOD_AUTO_DDT_EMESSO_ACCETTAZIONE & ")")
            stb.AppendLine(" -- Cod_RisUm_Aggiuntivo documenti ")
            stb.AppendLine(" union ")
            stb.AppendLine(" Select ru.Piva As PivaOrigine, mov.piva As PivaDest, ru.Cod_RisUm, ru.Cod_Contatto, 'Cod_RisUm_Aggiuntivo' as Scopo ")
            stb.AppendLine(" From #temp_risUm ru ")
            stb.AppendLine(" inner Join movimenti mov ")
            stb.AppendLine(" On ru.Cod_RisUm = mov.Cod_RisUm_Aggiuntivo ")
            stb.AppendLine(" -- daa destinatario ")
            stb.AppendLine(" union ")
            stb.AppendLine($" Select ru.Piva As PivaOrigine, mov.piva As PivaDest, ru.Cod_RisUm, ru.Cod_Contatto, '{Gias.DestinatarioDAA}' as Scopo ")
            stb.AppendLine(" From #temp_risUm ru ")
            stb.AppendLine(" inner Join Mov_Dettaglio_Tecnico_Extra mov ")
            stb.AppendLine(" On ru.Cod_RisUm = mov.ACCDAA_Cod_Risum_Destinatario ")
            stb.AppendLine(" -- daa destinazione ")
            stb.AppendLine(" union ")
            stb.AppendLine($" Select ru.Piva As PivaOrigine, mov.piva As PivaDest, ru.Cod_RisUm, ru.Cod_Contatto, '{Gias.DestinazioneDAA}' as Scopo ")
            stb.AppendLine(" From #temp_risUm ru ")
            stb.AppendLine(" inner Join Mov_Dettaglio_Tecnico_Extra mov ")
            stb.AppendLine(" On ru.Cod_RisUm = mov.ACCDAA_Cod_Risum_Destinazione ")
            stb.AppendLine(" -- Agente ")
            stb.AppendLine(" union ")
            stb.AppendLine($" Select ru.Piva As PivaOrigine, mov.piva As PivaDest, ru.Cod_RisUm, ru.Cod_Contatto, '{Gias.Agente}' as Scopo ")
            stb.AppendLine(" From #temp_risUm ru ")
            stb.AppendLine(" inner Join Mov_Dettaglio_Tecnico_Extra mov ")
            stb.AppendLine(" On ru.Cod_RisUm = mov.Agente_Cod ")
            stb.AppendLine(" -- CapoArea ")
            stb.AppendLine(" union ")
            stb.AppendLine($" Select ru.Piva As PivaOrigine, mov.piva As PivaDest, ru.Cod_RisUm, ru.Cod_Contatto, '{Gias.CapoArea}' as Scopo ")
            stb.AppendLine(" From #temp_risUm ru ")
            stb.AppendLine(" inner Join Mov_Dettaglio_Tecnico_Extra mov ")
            stb.AppendLine(" On ru.Cod_RisUm = mov.CapoArea_Cod ")
            stb.AppendLine(" -- Costi ")
            stb.AppendLine(" union ")
            stb.AppendLine($" Select ru.Piva As PivaOrigine, cdg.piva As PivaDest, ru.Cod_RisUm, ru.Cod_Contatto, '{Gias.Costi}' as Scopo ")
            stb.AppendLine(" From #temp_risUm ru ")
            stb.AppendLine("inner Join CDG_Testata cdg ")
            stb.AppendLine(" On ru.Cod_RisUm = cdg.Cod_RisUm And cdg.Budget = 0 ")
            stb.AppendLine(" -- Analisi ")
            stb.AppendLine(" union ")
            stb.AppendLine($" Select ru.Piva As PivaOrigine, ru.Piva As PivaDest, ru.Cod_RisUm,  ru.Cod_Contatto, '{Gias.LabAnalisi}' as Scopo ")
            stb.AppendLine(" From #temp_risUm ru ")
            stb.AppendLine(" inner Join Analisi_Tipologia_Laboratori atl ")
            stb.AppendLine(" On ru.Cod_RisUm = atl.Cod_Risum ")
            stb.AppendLine(" -- Laboratorio Analisi ")
            stb.AppendLine(" union ")
            stb.AppendLine($" Select ru.Piva As PivaOrigine, ru.Piva As PivaDest, ru.Cod_RisUm, ru.Cod_Contatto, '{Gias.LabAnalisi}' as Scopo ")
            stb.AppendLine(" From #temp_risUm ru ")
            stb.AppendLine(" inner Join Analisi_Certificato ac ")
            stb.AppendLine(" On ru.Cod_RisUm = ac.Analisi_Certificato_Laboratorio ")
            stb.AppendLine(" -- partita doppia dare ")
            stb.AppendLine(" union ")
            stb.AppendLine($" Select ru.Piva As PivaOrigine, p.Piva As PivaDest, ru.Cod_RisUm, ru.Cod_Contatto, '{Gias.PartitaDoppiaDare}' as Scopo ")
            stb.AppendLine(" From #temp_risUm ru ")
            stb.AppendLine(" inner Join Pagamenti p ")
            stb.AppendLine(" On ru.Cod_RisUm = p.Tipo_Cod_Dare ")
            stb.AppendLine(" where p.Tipo_Dare = 1 ")
            stb.AppendLine(" And p.Tipo_Cod_Dare <> 0 ")
            stb.AppendLine(" -- partita doppia avere ")
            stb.AppendLine(" union ")
            stb.AppendLine($" Select ru.Piva As PivaOrigine, p.Piva As PivaDest, ru.Cod_RisUm, ru.Cod_Contatto, '{Gias.PartitaDoppiaAvere}' as Scopo ")
            stb.AppendLine(" From #temp_risUm ru ")
            stb.AppendLine(" inner Join Pagamenti p ")
            stb.AppendLine(" On ru.Cod_RisUm = p.Tipo_Cod_Avere ")
            stb.AppendLine(" where p.Tipo_Avere = 1 ")
            stb.AppendLine(" And p.Tipo_Cod_Avere <> 0 ")

            ' Checks per Cod_Contatto
            If Cod_RisUm = 0 Then

                stb.AppendLine(" -- Organismo referente ")
                stb.AppendLine(" union ")
                stb.AppendLine($" Select ru.Piva As PivaOrigine, ric.Piva As PivaDest, ru.Cod_RisUm, ru.Cod_Contatto, '{Gias.OrganismoReferente}' as Scopo ")
                stb.AppendLine(" From #temp_risUm ru ")
                stb.AppendLine(" inner Join Reg_Impianti_Codici ric ")
                stb.AppendLine(" On ru.Cod_Contatto = ric.val_cod ")
                stb.AppendLine(" where id_cod = 1074 ")
                stb.AppendLine("--Vivaio ")
                stb.AppendLine(" union ")
                stb.AppendLine($" Select ru.Piva As PivaOrigine, ru.Piva As PivaDest, ru.Cod_RisUm, ru.Cod_Contatto, '{Gias.Vivaio}' as Scopo ")
                stb.AppendLine(" From #temp_risUm ru ")
                stb.AppendLine(" inner Join Programmazione_entita pe ")
                stb.AppendLine(" On ru.Cod_Contatto = pe.Veg_Cod_Cliente ")
                stb.AppendLine(" --Vivaio ")
                stb.AppendLine(" union ")
                stb.AppendLine($" Select ru.Piva As PivaOrigine, ru.Piva As PivaDest, ru.Cod_RisUm, ru.Cod_Contatto, '{Gias.Vivaio}' as Scopo ")
                stb.AppendLine(" From #temp_risUm ru ")
                stb.AppendLine(" inner Join Programmazione_Testata pt ")
                stb.AppendLine(" On ru.Cod_Contatto = pt.piva ")
                stb.AppendLine(" -- Operatore Lab Controllo Qualità ")
                stb.AppendLine(" union ")
                stb.AppendLine($" Select ru.Piva As PivaOrigine, ru.Piva As PivaDest, ru.Cod_RisUm, ru.Cod_Contatto, '{Gias.OperatoreLabControlloQual}' as Scopo ")
                stb.AppendLine(" From #temp_risUm ru ")
                stb.AppendLine(" inner Join LCQ_ParametriValori lcq ")
                stb.AppendLine(" On ru.Cod_Contatto = lcq.Utente ")

            End If

            stb.AppendLine(" ) a ")
            stb.AppendLine(" inner join Imprese ")
            stb.AppendLine(" On a.PivaDest = Imprese.PIVA ")

            If Sa_Cod = 0 Then
                stb.AppendLine(" where PivaOrigine = PivaDest ")
            End If

            If Sa_Cod = -999 Then
                stb.AppendLine(" where PivaOrigine <> PivaDest ")
            End If

            DT = EseguiQuery_Lettura(objParametri, stb.ToString, NomeRoutine)

            EseguiQuery_Scrittura(objParametri, " DROP TABLE #temp_risUm ", NomeRoutine)

            If objParametri.objTransazione IsNot Nothing Then
                'Se esiste la transazione ed è locale ne faccio il commit
                ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri)
            End If

        Catch ex As Exception

            If FlagTransazioneLocale AndAlso objParametri.objTransazione IsNot Nothing Then
                ConnessioniTransazioni.ChiudiTransazione(2, objParametri)
            End If

            MessaggioErrore = ex.Message

            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)

            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        Finally
            'Se la connessione è locale la chiudo e imposto a Nothing gli oggetti di connessione e transazione,
            'su quest'ultimo, nel caso devono essere già stati eseguiti il commit od il rollback
            ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri)
        End Try

        Return DT

    End Function

    Public Function ContattiPubbliciDaPrivatizzare(ByVal piva As String,
                                                   ByVal Cod_Contatto As List(Of String),
                                                   ByVal Cod_RisUm As Integer,
                                                   ByVal Cod_Rapporto As List(Of Integer),
                                                   ByVal xFiltroAggiuntivo As String,
                                                   ByRef objParametri As AgronicaCoreParametri
                                                    ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Contatti_R.ContattiPubbliciDaPrivatizzare()"

        Dim MessaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim DT As DataTable

        Try

            stb.AppendLine("With RisorseUmane(piva, Sa_Cod, Cod_RisUm, Cod_Contatto) ")
            stb.AppendLine(" AS ( ")
            stb.AppendLine(" Select piva, Sa_Cod, Cod_RisUm, Cod_Contatto from Risorse_Umane ru ")
            stb.AppendLine(" where Sa_Cod = -1  and cod_contatto <> 'superuser' ")
            stb.AppendLine(" AND piva = '" & Agro_SQL_SaveText(piva) & "' ")

            If Cod_Contatto IsNot Nothing AndAlso Cod_Contatto.Count > 0 Then
                For i As Integer = 0 To Cod_Contatto.Count - 1
                    Cod_Contatto(i) = String.Format("'{0}'", Cod_Contatto(i))
                Next
                stb.AppendLine(" AND cod_contatto IN (" & Agro_SQL_Save_Clausola_IN(String.Join(",", Cod_Contatto), True) & ") ")
            End If
            If Cod_RisUm <> 0 Then
                stb.AppendLine(" AND cod_risUm = " & Agro_SQL_SaveNum(Cod_RisUm) & " ")
            End If
            If Cod_Rapporto IsNot Nothing AndAlso Cod_Rapporto.Count > 0 Then
                stb.AppendLine(" AND cod_rapporto IN (" & Agro_SQL_Save_Clausola_IN(String.Join(",", Cod_Rapporto), False) & ") ")
            End If
            stb.AppendLine(" ) ")

            stb.AppendLine(" Select distinct a.*, ")
            stb.AppendLine(" CASE c.Rag_Soc WHEN '' then  c.Cognome + ' ' +  c.Nome ELSE c.Rag_Soc END as Contatto_Des, ")
            stb.AppendLine(" ori.rag_soc as ragsocorigine, dest.rag_soc as ragsocdest from ( ")

            ' Join x Cod_RisUm
            stb.AppendLine(" -- movimenti (Manodopera) ")
            stb.AppendLine(" Select ru.Piva As PivaOrigine, mov.piva As PivaDest, ru.Cod_RisUm, ru.Cod_Contatto, 'Manodopera' as Scopo ")
            stb.AppendLine(" From RisorseUmane ru  ")
            stb.AppendLine(" inner Join Movimenti_dettagli mov  ")
            stb.AppendLine(" On ru.Cod_RisUm = mov.Mat_Cod And mov.Elem_Cod = 0 ")
            stb.AppendLine(" union ")
            stb.AppendLine(" Select ru.Piva As PivaOrigine, pc.piva As PivaDest, ru.Cod_RisUm, ru.Cod_Contatto, 'Costo' as Scopo ")
            stb.AppendLine(" From RisorseUmane ru  ")
            stb.AppendLine(" inner Join Prodotti_Costi pc ")
            stb.AppendLine(" On ru.Cod_RisUm = pc.Mat_Cod And pc.Elem_Cod = 0 And pc.Id_Budget = 0")
            stb.AppendLine(" union ")
            stb.AppendLine(" -- intestazione documenti ")
            stb.AppendLine(" Select ru.Piva As PivaOrigine, mov.piva As PivaDest, ru.Cod_RisUm, ru.Cod_Contatto, 'Intestatario Doc' as Scopo ")
            stb.AppendLine(" From RisorseUmane ru ")
            stb.AppendLine(" inner Join movimenti mov ")
            stb.AppendLine(" On ru.Cod_RisUm = mov.Cod_RisUm ")
            stb.AppendLine(" -- vettore documenti ")
            stb.AppendLine(" union ")
            stb.AppendLine(" Select ru.Piva As PivaOrigine, mov.piva As PivaDest, ru.Cod_RisUm, ru.Cod_Contatto, 'Vettore' as Scopo ")
            stb.AppendLine(" From RisorseUmane ru ")
            stb.AppendLine(" inner Join movimenti mov ")
            stb.AppendLine(" On ru.Cod_RisUm = mov.cod_vettore ")
            stb.AppendLine(" -- destinazione diversa documenti ")
            stb.AppendLine(" union ")
            stb.AppendLine(" Select ru.Piva As PivaOrigine, mov.piva As PivaDest, ru.Cod_RisUm, ru.Cod_Contatto, 'Destinazione Diversa' as Scopo ")
            stb.AppendLine(" From RisorseUmane ru ")
            stb.AppendLine(" inner Join movimenti mov ")
            stb.AppendLine(" On ru.Cod_RisUm = mov.Cod_Destinazione ")
            stb.AppendLine(" -- cod_risum_altro documenti ")
            stb.AppendLine(" union ")
            stb.AppendLine(" Select ru.Piva As PivaOrigine, mov.piva As PivaDest, ru.Cod_RisUm, ru.Cod_Contatto, 'CodRisUMAltro - Conferimento' as Scopo ")
            stb.AppendLine(" From RisorseUmane ru ")
            stb.AppendLine(" inner Join movimenti mov ")
            stb.AppendLine(" On ru.Cod_RisUm = mov.Cod_RisUm_Altro ")
            stb.AppendLine(" -- Cod_RisUm_Aggiuntivo documenti ")
            stb.AppendLine(" union ")
            stb.AppendLine(" Select ru.Piva As PivaOrigine, mov.piva As PivaDest, ru.Cod_RisUm, ru.Cod_Contatto, 'Cod_RisUm_Aggiuntivo' as Scopo ")
            stb.AppendLine(" From RisorseUmane ru ")
            stb.AppendLine(" inner Join movimenti mov ")
            stb.AppendLine(" On ru.Cod_RisUm = mov.Cod_RisUm_Aggiuntivo ")
            stb.AppendLine(" -- daa destinatario ")
            stb.AppendLine(" union ")
            stb.AppendLine(" Select ru.Piva As PivaOrigine, mov.piva As PivaDest, ru.Cod_RisUm, ru.Cod_Contatto, 'Destinatario DAA' as Scopo ")
            stb.AppendLine(" From RisorseUmane ru ")
            stb.AppendLine(" inner Join Mov_Dettaglio_Tecnico_Extra mov ")
            stb.AppendLine(" On ru.Cod_RisUm = mov.ACCDAA_Cod_Risum_Destinatario ")
            stb.AppendLine(" -- daa destinazione ")
            stb.AppendLine(" union ")
            stb.AppendLine(" Select ru.Piva As PivaOrigine, mov.piva As PivaDest, ru.Cod_RisUm, ru.Cod_Contatto, 'Destinazione DAA' as Scopo ")
            stb.AppendLine(" From RisorseUmane ru ")
            stb.AppendLine(" inner Join Mov_Dettaglio_Tecnico_Extra mov ")
            stb.AppendLine(" On ru.Cod_RisUm = mov.ACCDAA_Cod_Risum_Destinazione ")
            stb.AppendLine(" -- Agente ")
            stb.AppendLine(" union ")
            stb.AppendLine(" Select ru.Piva As PivaOrigine, mov.piva As PivaDest, ru.Cod_RisUm, ru.Cod_Contatto, 'Agente' as Scopo ")
            stb.AppendLine(" From RisorseUmane ru ")
            stb.AppendLine(" inner Join Mov_Dettaglio_Tecnico_Extra mov ")
            stb.AppendLine(" On ru.Cod_RisUm = mov.Agente_Cod ")
            stb.AppendLine(" -- CapoArea ")
            stb.AppendLine(" union ")
            stb.AppendLine(" Select ru.Piva As PivaOrigine, mov.piva As PivaDest, ru.Cod_RisUm, ru.Cod_Contatto, 'Agente' as Scopo ")
            stb.AppendLine(" From RisorseUmane ru ")
            stb.AppendLine(" inner Join Mov_Dettaglio_Tecnico_Extra mov ")
            stb.AppendLine(" On ru.Cod_RisUm = mov.CapoArea_Cod ")
            stb.AppendLine(" -- Costi ")
            stb.AppendLine(" union ")
            stb.AppendLine(" Select ru.Piva As PivaOrigine, cdg.piva As PivaDest, ru.Cod_RisUm, ru.Cod_Contatto, 'Costo' as Scopo ")
            stb.AppendLine(" From RisorseUmane ru ")
            stb.AppendLine("inner Join CDG_Testata cdg ")
            stb.AppendLine(" On ru.Cod_RisUm = cdg.Cod_RisUm And cdg.Budget = 0 ")
            stb.AppendLine(" -- Analisi ")
            stb.AppendLine(" union ")
            stb.AppendLine(" Select ru.Piva As PivaOrigine, ru.Piva As PivaDest, ru.Cod_RisUm,  ru.Cod_Contatto, 'Lab. Analisi' as Scopo ")
            stb.AppendLine(" From RisorseUmane ru ")
            stb.AppendLine(" inner Join Analisi_Tipologia_Laboratori atl ")
            stb.AppendLine(" On ru.Cod_RisUm = atl.Cod_Risum ")
            stb.AppendLine(" -- Laboratorio Analisi ")
            stb.AppendLine(" union ")
            stb.AppendLine(" Select ru.Piva As PivaOrigine, ru.Piva As PivaDest, ru.Cod_RisUm, ru.Cod_Contatto, 'Lab. Analisi' as Scopo ")
            stb.AppendLine(" From RisorseUmane ru ")
            stb.AppendLine(" inner Join Analisi_Certificato ac ")
            stb.AppendLine(" On ru.Cod_RisUm = ac.Analisi_Certificato_Laboratorio ")
            stb.AppendLine(" -- partita doppia dare ")
            stb.AppendLine(" union ")
            stb.AppendLine(" Select ru.Piva As PivaOrigine, p.Piva As PivaDest, ru.Cod_RisUm, ru.Cod_Contatto, 'Partita Doppia Dare' as Scopo ")
            stb.AppendLine(" From RisorseUmane ru ")
            stb.AppendLine(" inner Join Pagamenti p ")
            stb.AppendLine(" On ru.Cod_RisUm = p.Tipo_Cod_Dare ")
            stb.AppendLine(" where p.Tipo_Dare = 1 ")
            stb.AppendLine(" And p.Tipo_Cod_Dare <> 0 ")
            stb.AppendLine(" -- partita doppia avere ")
            stb.AppendLine(" union ")
            stb.AppendLine(" Select ru.Piva As PivaOrigine, p.Piva As PivaDest, ru.Cod_RisUm, ru.Cod_Contatto, 'Partita Doppia Avere' as Scopo ")
            stb.AppendLine(" From RisorseUmane ru ")
            stb.AppendLine(" inner Join Pagamenti p ")
            stb.AppendLine(" On ru.Cod_RisUm = p.Tipo_Cod_Avere ")
            stb.AppendLine(" where p.Tipo_Avere = 1 ")
            stb.AppendLine(" And p.Tipo_Cod_Avere <> 0 ")

            ' Join x Cod_Contatto
            stb.AppendLine(" -- Organismo referente ")
            stb.AppendLine(" union ")
            stb.AppendLine(" Select ru.Piva As PivaOrigine, ric.Piva As PivaDest, ru.Cod_RisUm, ru.Cod_Contatto, 'Organismo Referente' as Scopo ")
            stb.AppendLine(" From RisorseUmane ru ")
            stb.AppendLine(" inner Join Reg_Impianti_Codici ric ")
            stb.AppendLine(" On ru.Cod_Contatto = ric.val_cod ")
            stb.AppendLine(" where id_cod = 1074 ")
            stb.AppendLine("--Vivaio ")
            stb.AppendLine(" union ")
            stb.AppendLine(" Select ru.Piva As PivaOrigine, ru.Piva As PivaDest, ru.Cod_RisUm, ru.Cod_Contatto, 'Vivaio' as Scopo ")
            stb.AppendLine(" From RisorseUmane ru ")
            stb.AppendLine(" inner Join Programmazione_entita pe ")
            stb.AppendLine(" On ru.Cod_Contatto = pe.Veg_Cod_Cliente ")
            stb.AppendLine(" --Vivaio ")
            stb.AppendLine(" union ")
            stb.AppendLine(" Select ru.Piva As PivaOrigine, ru.Piva As PivaDest, ru.Cod_RisUm, ru.Cod_Contatto, 'Vivaio' as Scopo ")
            stb.AppendLine(" From RisorseUmane ru ")
            stb.AppendLine(" inner Join Programmazione_Testata pt ")
            stb.AppendLine(" On ru.Cod_Contatto = pt.piva ")
            stb.AppendLine(" -- Operatore Lab Controllo Qualità ")
            stb.AppendLine(" union ")
            stb.AppendLine(" Select ru.Piva As PivaOrigine, ru.Piva As PivaDest, ru.Cod_RisUm, ru.Cod_Contatto, 'Operatore Lab Controllo Qualità' as Scopo ")
            stb.AppendLine(" From RisorseUmane ru ")
            stb.AppendLine(" inner Join LCQ_ParametriValori lcq ")
            stb.AppendLine(" On ru.Cod_Contatto = lcq.Utente ")
            stb.AppendLine(" ) a ")
            stb.AppendLine(" inner Join Imprese dest ")
            stb.AppendLine(" On a.PivaDest = dest.PIVA ")
            stb.AppendLine(" inner Join Imprese ori ")
            stb.AppendLine(" On a.PivaOrigine = ori.PIVA")
            stb.AppendLine(" inner Join contatti c ")
            stb.AppendLine(" On a.Cod_Contatto = c.Cod_Contatto ")
            stb.AppendLine(" And a.PivaOrigine = c.Piva ")

            DT = EseguiQuery_Lettura(objParametri, stb.ToString, NomeRoutine)

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function

    Public Function LeggiUtentiDaAssociare(ByRef objParametri_server As AgronicaCoreParametri,
                                           ByRef objParametri_utenti As AgronicaCoreParametri) As DataTable
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Contatti_R.LeggiUtentiDaAssociare()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New StringBuilder
        Dim DT As DataTable

        Try

            Dim NomeDB_Utenti As String = ""
            If Not IsNothing(objParametri_utenti) Then
                NomeDB_Utenti = objParametri_utenti.Recupera_NomeDB()
            End If

            If Not String.IsNullOrEmpty(NomeDB_Utenti) Then
                StrSQL.AppendLine(" SELECT CodFisc, UserName, Nome, Cognome FROM " & NomeDB_Utenti & ".dbo.Utenti_Dettagli AS utente    ")
                StrSQL.AppendLine("WHERE NOT EXISTS ( SELECT UserName FROM ContattiXUtentiGias WHERE UserName = utente.UserName)   ")
            End If


            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri_server, StrSQL.ToString, NomeRoutine)
            '-------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_server, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function

    Public Function LeggiUtenteAssociato(ByVal piva As String,
                                         ByVal cod_contatto As String,
                                         ByRef objParametri_server As AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Contatti_R.LeggiUtentiDaAssociare()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New StringBuilder
        Dim DT As DataTable

        Try
            StrSQL.AppendLine(" SELECT username FROM ContattiXUtentiGias ")
            StrSQL.AppendLine(" WHERE piva = '" & Agro_SQL_SaveText(piva) & "' ")
            StrSQL.AppendLine(" AND   cod_contatto = '" & Agro_SQL_SaveText(cod_contatto) & "' ")

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri_server, StrSQL.ToString, NomeRoutine)
            '-------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_server, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function

    Public Function LeggiRubricaGPF(ByVal Piva As String,
                                    ByVal Rubrica_Cod As Integer,
                                    ByVal xFiltroAggiuntivo As String,
                                        ByRef objParametri As AgronicaCoreParametri
                                        ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Contatti_R.LeggiDatiMinimi()"

        Dim MessaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim DT As DataTable

        Try


            stb.AppendLine("   Select ")
            stb.AppendLine("   r.cod_rubrica as id ")
            stb.AppendLine("  , c.Nome + ' ' + c.Cognome as name ")
            stb.AppendLine("  , numero as plate ")
            stb.AppendLine(" From rubrica r ")
            stb.AppendLine("  inner Join contattixrubrica cr ")
            stb.AppendLine("         On r.cod_rubrica = cr.cod_rubrica ")
            stb.AppendLine("  inner Join contatti c ")
            stb.AppendLine("         On c.piva = cr.piva ")
            stb.AppendLine("      And c.Cod_Contatto = cr.Cod_Contatto ")
            stb.AppendLine("  ")
            stb.AppendLine(" where r.descr = 'Cellulare:' ")
            stb.AppendLine(" and len(isnull(r.numero, '') ) > 5 ")
            If Piva <> "" Then
                stb.AppendLine(" And c.piva = '" & Agro_SQL_SaveText(Piva) & "'")
            End If

            If Rubrica_Cod <> 0 Then
                stb.AppendLine(" and r.cod_rubrica = " & Rubrica_Cod)
            End If

            stb.AppendLine(" And exists ( ")
            stb.AppendLine("   Select 1 ")
            stb.AppendLine("      From risorse_umane ris ")
            stb.AppendLine("   Where ris.cod_rapporto = -6 ")
            stb.AppendLine("   And ris.Cod_Contatto = c.cod_contatto ")
            stb.AppendLine("   And ris.piva = c.piva  ")
            stb.AppendLine("  )")


            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, stb.ToString, NomeRoutine)
            '-------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function


    '###################################################################################
    Public Function RagSoc_From_CodContatto(ByVal Piva As String,
                                            ByVal Cod_Contatto As String,
                                            ByVal xFiltroAggiuntivo As String,
                                            ByRef objParametri As AgronicaCoreParametri
                                            ) As String

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Contatti_R.RagSoc_From_CodContatto()"

        Dim messaggioErrore As String = ""
        Dim dt As DataTable

        Dim ragSoc As String = ""

        Try

            dt = LeggiDatiMinimi(Piva, Cod_Contatto,
                                 xFiltroAggiuntivo,
                                 objParametri)

            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                ragSoc = dt.Rows(0).Item("rag_soc")
            End If

            dt = Nothing

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return ragSoc

    End Function


    '##############################################################################################
    Public Function LeggiDatiMinimi(ByVal Piva As String,
                                    ByVal Cod_Contatto As String,
                                    ByVal xFiltroAggiuntivo As String,
                                    ByRef objParametri As AgronicaCoreParametri,
                                    Optional ByVal Flag_Pubblico_Privato As Boolean = False,
                                    Optional ByVal Flag_Visibilita_Centri As Boolean = False
                                    ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Contatti_R.LeggiDatiMinimi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New StringBuilder
        Dim DT As DataTable

        Try

            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append(" SELECT Piva, Cod_Contatto, Rag_Soc + Nome + ' ' + Cognome AS Rag_Soc, Sa_Cod ")
            StrSQL.Append(" FROM Contatti  ")
            StrSQL.Append(" WHERE 1=1 ")

            If Piva <> "" Then

                Select Case Flag_Pubblico_Privato
                    Case False
                        StrSQL.Append(" AND Contatti.Sa_Cod <> -1 " & vbCrLf)
                    Case True
                        'In questo caso non filtriamo niente per ottenere tutti i contatti disponibili.
                End Select

                StrSQL.Append(" AND Contatti.Piva = '" & Agro_SQL_SaveText(Piva) & "' " & vbCrLf)

                If Flag_Visibilita_Centri Then
                    Dim objUtentiVisibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
                    Dim FiltroCentri As String = ""
                    Dim DtCentriVisibili As DataTable
                    DtCentriVisibili = objUtentiVisibilita.Leggi(enum_TipoEntita.Centro, " Piva='" & Agro_SQL_SaveText(Piva) & "'", "", objParametri)
                    If DtCentriVisibili IsNot Nothing AndAlso DtCentriVisibili.Rows.Count > 0 Then
                        FiltroCentri = "0,"
                        For i = 0 To DtCentriVisibili.Rows.Count - 1
                            FiltroCentri &= DtCentriVisibili.Rows(i).Item("sa_cod") & ","
                        Next
                        If FiltroCentri <> "" Then
                            StrSQL.AppendLine(" AND Contatti.sa_cod IN (" & Agro_SQL_Save_Clausola_IN(Left(FiltroCentri, FiltroCentri.Length - 1)) & ") ")
                        End If
                    End If
                End If

            End If

            If Cod_Contatto <> "" Then
                StrSQL.Append(" AND Contatti.Cod_Contatto = '" & Agro_SQL_SaveText(Cod_Contatto) & "' " & vbCrLf)
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   Contatti.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   Contatti.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '-------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function

    Public Function Leggi_Contatti_Utenti_Gias(ByRef objParametri As AgronicaCoreParametri,
                                               Optional ByRef username As String = "",
                                               Optional ByRef piva As String = "",
                                               Optional ByVal rapportiContabili As String = "") As DataTable
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Contatti_R.Leggi_Contatti_Utenti_Gias()"

        '====================================================================================
        'Parametri opzionali :
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.AppendLine(" SELECT a.Piva, a.Cod_Contatto, a.Sa_Cod, a.Rag_Soc + a.Cognome + ' ' + a.Nome AS Contatto_Des, b.Cod_RisUm, b.Cod_Rapporto ")
            StrSQL.AppendLine(" FROM contatti a ")
            StrSQL.AppendLine(" INNER JOIN risorse_umane b ON a.piva=b.piva AND a.cod_contatto = b.cod_contatto ")
            StrSQL.AppendLine(" INNER JOIN ContattiXUtentiGias c ON a.piva=c.piva AND a.cod_contatto = c.cod_contatto ")
            If String.IsNullOrEmpty(piva) Then
                StrSQL.AppendLine(" WHERE a.sa_cod = -1 ")
            Else
                StrSQL.AppendLine(" WHERE a.sa_cod = 0 AND a.piva = ' " & Agro_SQL_SaveText(piva) & "' ")
            End If
            If Not String.IsNullOrEmpty(username) Then
                StrSQL.AppendLine(" AND c.username = '" & Agro_SQL_SaveText(username) & "' ")
            End If
            If Not String.IsNullOrEmpty(rapportiContabili) Then
                StrSQL.AppendLine(" AND b.cod_rapporto IN (" & Agro_SQL_Save_Clausola_IN(rapportiContabili) & ") ")
            End If
            StrSQL.AppendLine(" ORDER BY a.Rag_Soc + a.Cognome + ' ' + a.Nome ")

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function

    Public Function ContattiXUtentiGiasEsisteUtente(
        ByRef objParametri As AgronicaCoreParametri
    ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Contatti_R.LeggiVivaioPrenotazionePiante()"

        Dim MessaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim DT As DataTable

        Try

            '---------------------------------------------
            stb.Length = 0

            stb.AppendLine("  select 1 from ContattiXUtentiGias cug ")
            stb.AppendLine("  where cug.username = '" & objParametri.UtenteUsername & "' ")

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, stb.ToString, NomeRoutine)
            '-------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function


    Public Function LeggiVivaioPrenotazionePiante(
        ByVal Piva As String,
        ByVal Cod_Contatto As String,
        ByVal join_ContattiXUtentiGias As Boolean,
        ByVal xFiltroAggiuntivo As String,
        ByRef objParametri As AgronicaCoreParametri,
        Optional ByVal Flag_Pubblico_Privato As Boolean = False
    ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Contatti_R.LeggiVivaioPrenotazionePiante()"

        Dim MessaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim DT As DataTable

        Try

            '---------------------------------------------
            stb.Length = 0
            stb.AppendLine("             SELECT   ")
            stb.AppendLine("    Imprese.Rag_Soc AS Impresa ")
            stb.AppendLine("  , Contatti.Piva ")
            stb.AppendLine("  , Contatti.Sa_Cod ")
            stb.AppendLine("  , Contatti.Cod_Contatto ")
            stb.AppendLine("  , Contatti.Codice_Fiscale ")
            stb.AppendLine("  , Contatti.Id_CF ")
            stb.AppendLine("  , Contatti.Rag_Soc ")
            stb.AppendLine("  , Contatti.Convenevoli ")
            stb.AppendLine("  , Contatti.Nome ")
            stb.AppendLine("  , Contatti.Cognome ")
            stb.AppendLine("  , Contatti.Data_Nascita ")
            stb.AppendLine("  , Contatti.Sesso ")
            stb.AppendLine("  , Contatti.Cod_Contatto_Referente ")
            stb.AppendLine("  , Risorse_Umane.Sa_Cod AS Sa_Cod_2 ")
            stb.AppendLine("  , Risorse_Umane.Cod_RisUm ")
            stb.AppendLine("  , Risorse_Umane.Validita_Inizio ")
            stb.AppendLine("  , Risorse_Umane.Validita_Fine ")
            stb.AppendLine("  , Risorse_Umane.Settore_Des ")
            stb.AppendLine("  , Risorse_Umane.Attivita_Des ")
            stb.AppendLine("  , Risorse_Umane.Corrispettivo_Mensile ")
            stb.AppendLine("  , Risorse_Umane.Corrispettivo_Orario ")
            stb.AppendLine("  , Risorse_Umane.Occasionale ")
            stb.AppendLine("  , Risorse_Umane.Ore_Settimanali ")
            stb.AppendLine("  , Risorse_Umane.Giorni_Ferie ")
            stb.AppendLine("  , Risorse_Umane.Ferie_Godute ")
            stb.AppendLine("  , Risorse_Umane.Giorni_Malattia ")
            stb.AppendLine("  , Risorse_Umane.Patentino ")
            stb.AppendLine("  , Risorse_Umane.Data_Rilascio_Patentino ")
            stb.AppendLine("  , Risorse_Umane.Data_Scadenza_Patentino ")
            stb.AppendLine("  , Rapporti_Contabili.Piva AS Piva_SuperUser ")
            stb.AppendLine("  , Rapporti_Contabili.Sa_Cod AS Sa_Cod_3 ")
            stb.AppendLine("  , Rapporti_Contabili.Cod_Rapporto ")
            stb.AppendLine("  , Rapporti_Contabili.Rapporto_Des ")
            stb.AppendLine("  , Rapporti_Contabili.Cliente ")
            stb.AppendLine("  , Rapporti_Contabili.Fornitore ")
            stb.AppendLine("  , Rapporti_Contabili.Dipendente ")
            stb.AppendLine("  , Rapporti_Contabili.Terzista ")
            stb.AppendLine("  , Rapporti_Contabili.Legale ")
            stb.AppendLine("  , ISNULL (( ")
            stb.AppendLine("      SELECT TOP 1   val_cod             ")
            stb.AppendLine("      FROM        Contatti_Codici            ")
            stb.AppendLine("      WHERE       Contatti.PIVA = Contatti_Codici.PIVA  ")
            stb.AppendLine("      AND Contatti.Cod_Contatto = Contatti_Codici.Cod_Contatto  ")
            stb.AppendLine("      AND Contatti_Codici.id_cod = 5001), '0') AS Modifica   ")
            stb.AppendLine("  ")
            stb.AppendLine("  ")
            stb.AppendLine("  FROM    Contatti   ")
            stb.AppendLine("  INNER JOIN Risorse_Umane  ")
            stb.AppendLine("      ON Contatti.Cod_Contatto = Risorse_Umane.Cod_Contatto  ")
            stb.AppendLine("      AND Contatti.Piva = Risorse_Umane.Piva  ")
            stb.AppendLine("  INNER JOIN  Rapporti_Contabili  ")
            stb.AppendLine("      ON Risorse_Umane.Cod_Rapporto = Rapporti_Contabili.Cod_Rapporto  ")
            stb.AppendLine("  INNER JOIN UtentiXImprese  ")
            stb.AppendLine("      ON Rapporti_Contabili.Piva = UtentiXImprese.[USER]  ")
            stb.AppendLine("      AND Risorse_Umane.Piva = UtentiXImprese.PIVA  ")
            stb.AppendLine("  INNER JOIN Imprese  ")
            stb.AppendLine("      ON Imprese.Piva = Contatti.Piva  ")

            If join_ContattiXUtentiGias Then
                stb.AppendLine("  inner join ContattiXUtentiGias cug ")
                stb.AppendLine("      on cug.piva = Contatti.piva ")
                stb.AppendLine("      and cug.cod_contatto = Contatti.Cod_contatto ")
                stb.AppendLine("      and cug.username = '" & objParametri.UtenteUsername & "' ")
            End If

            stb.AppendLine("   WHERE (Rapporti_Contabili.Piva = '" & Agro_SQL_SaveText(Piva) & "')    ")
            stb.AppendLine("  AND ( Risorse_Umane.Validita_Inizio <=  " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & "  )  ")
            stb.AppendLine("  AND ( Risorse_Umane.Validita_Fine >=  " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & "  )  ")
            stb.AppendLine("  AND    ( (Contatti.Sa_Cod = -1)           OR  (Contatti.Piva = '" & Agro_SQL_SaveText(Piva) & "')          )  AND     (Rapporti_Contabili.Cod_Rapporto = -17)    ")
            stb.AppendLine("  AND   Contatti.Inviato >=0   ")
            stb.AppendLine("  ORDER BY Contatti.Rag_Soc ASC")


            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, stb.ToString, NomeRoutine)
            '-------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function

    Public Function LeggiUtenteTecnicoOCapo(
        ByVal username As String,
        ByRef objParametri_Server As AgronicaCoreParametri,
        ByVal codRapporti As String
    ) As DataTable

        Dim MessaggioErrore As String = ""
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Contatti_R.LeggiUtenteTecnicoOCapo()"

        Dim stb As New StringBuilder
        Dim DT As DataTable

        'objParametri_Utente.UtenteUsername = "mario.rossi"

        Try

            '---------------------------------------------
            stb.Length = 0
            stb.AppendLine(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;")
            stb.AppendLine("             SELECT  * ")
            stb.AppendLine("  FROM    Contatti   ")
            stb.AppendLine("  INNER JOIN Risorse_Umane  ")
            stb.AppendLine("      ON Contatti.Cod_Contatto = Risorse_Umane.Cod_Contatto  ")
            stb.AppendLine("      AND Contatti.Piva = Risorse_Umane.Piva  ")
            stb.AppendLine("  inner join ContattiXUtentiGias cug ")
            stb.AppendLine("      on cug.piva = Contatti.piva ")
            stb.AppendLine("      and cug.cod_contatto = Contatti.Cod_contatto ")
            stb.AppendLine("      and cug.username = '" & Agro_SQL_SaveText(username) & "' ")
            stb.AppendLine("   WHERE (Risorse_Umane.Cod_Rapporto = '" & Agro_SQL_SaveText(codRapporti) & "')    ")
            stb.AppendLine("  AND   Risorse_Umane.Sa_Cod = -1   ")
            stb.AppendLine("  AND   Contatti.Sa_Cod = -1   ")
            stb.AppendLine("  AND   Contatti.Inviato >=0   ")


            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri_Server, stb.ToString, NomeRoutine)
            '-------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function

    Public Function LeggiListaTecnici(
        ByRef objParametri_Utente As AgronicaCoreParametri,
        ByRef objParametri_Server As AgronicaCoreParametri,
        ByVal typeOperatore As enum_TipoOperatoreVisita,
        ByVal Filtro_Visibilita_Utente As Boolean
    ) As DataTable

        Dim MessaggioErrore As String = ""
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Contatti_R.LeggiListaTecnici()"

        Dim isSuperUser As Boolean = (objParametri_Utente.SuperUserUsername = objParametri_Utente.UtenteUsername)

        Dim stb As New StringBuilder
        Dim DT As DataTable


        Try

            '---------------------------------------------
            stb.Length = 0
            stb.AppendLine("  SELECT Risorse_Umane.Cod_RisUm, cug.username, contatti.Cognome, Contatti.Nome, Contatti.Codice_Fiscale ")
            stb.AppendLine("  FROM    Contatti   ")
            stb.AppendLine("  INNER JOIN Risorse_Umane  ")
            stb.AppendLine("      ON Contatti.Cod_Contatto = Risorse_Umane.Cod_Contatto  ")
            stb.AppendLine("      AND Contatti.Piva = Risorse_Umane.Piva  ")
            stb.AppendLine("  INNER JOIN  Rapporti_Contabili  ")
            stb.AppendLine("  ON Risorse_Umane.Cod_Rapporto = Rapporti_Contabili.Cod_Rapporto ")
            stb.AppendLine("  inner join ContattiXUtentiGias cug ")
            stb.AppendLine("      on cug.piva = Contatti.piva ")
            stb.AppendLine("      and cug.cod_contatto = Contatti.Cod_contatto ")
            stb.AppendLine("   WHERE (Rapporti_Contabili.Cod_Rapporto = '-6')    ")
            stb.AppendLine("  AND   Risorse_Umane.Sa_Cod = -1   ")
            stb.AppendLine("  AND   Contatti.Sa_Cod = -1   ")
            stb.AppendLine("  AND   Contatti.Inviato >=0   ")
            If typeOperatore = enum_TipoOperatoreVisita.Tecnico Then
                stb.AppendLine("  AND cug.username = '" & Agro_SQL_SaveText(objParametri_Utente.UtenteUsername) & "' ")
            End If
            If Filtro_Visibilita_Utente Then
                stb.AppendLine(" AND ( cug.username IN (SELECT DISTINCT uva1.username FROM Utenti_Visibilita_Appoggio uva1 (NOLOCK) INNER JOIN Utenti_Visibilita_Appoggio uva2 (NOLOCK) on uva1.piva = uva2.piva WHERE uva1.Entita_Cod = 1 AND uva2.Entita_Cod = 1 AND uva2.Username = '" & Agro_SQL_SaveText(objParametri_Utente.UtenteUsername) & "' ) ")
                stb.AppendLine(" OR NOT EXISTS (SELECT 1 FROM Utenti_Visibilita_Appoggio uva3 WHERE uva3.username = cug.username)) ")
            End If


            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri_Server, stb.ToString, NomeRoutine)
            '-------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function

    Public Function LeggiListaAziendeEsclusoAgenzie(
        ByRef objParametri_Utente As AgronicaCoreParametri,
        ByRef objParametri_Server As AgronicaCoreParametri,
        ByVal UsernameTecnico As String,
        ByVal typeUserLogged As enum_TipoOperatoreVisita,
        ByVal typeTecnico As enum_TipoOperatoreVisita
    ) As DataTable

        Dim MessaggioErrore As String = ""
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Contatti_R.LeggiListaAziendeEsclusoAgenzie()"

        Dim strsql As New StringBuilder
        Dim DT As DataTable

        Dim checkExistsImpresaTecnico As Boolean = Check_Impresa_Associata(UsernameTecnico, objParametri_Utente, objParametri_Server, typeTecnico)
        Dim checkExistsImpresaUserLogged As Boolean = Check_Impresa_Associata(objParametri_Utente.UtenteUsername, objParametri_Utente, objParametri_Server, typeUserLogged)

        Dim isSuperUser As Boolean = (objParametri_Utente.SuperUserUsername = objParametri_Utente.UtenteUsername) AndAlso (objParametri_Utente.UtenteUsername = UsernameTecnico)

        Try

            'join tra imprese e pive per tirar fuori la ragione sociale
            strsql.Length = 0
            strsql.Append(" SELECT DISTINCT I.PIVA, I.rag_soc ")
            strsql.Append(" FROM  Imprese I ")

            If Not isSuperUser AndAlso checkExistsImpresaTecnico Then
                strsql.Append(" inner join Utenti_Visibilita_Appoggio U (NOLOCK) ")
                strsql.Append(" on I.Piva = U.Piva ")
            End If

            If checkExistsImpresaUserLogged AndAlso (typeUserLogged = enum_TipoOperatoreVisita.CapoTecnico OrElse typeUserLogged = enum_TipoOperatoreVisita.Capo) Then
                strsql.Append(" inner join Utenti_Visibilita_Appoggio U_Capo (NOLOCK) ")
                strsql.Append(" on I.Piva = U_Capo.Piva and U_Capo.username ='" & Agro_SQL_SaveText(objParametri_Utente.UtenteUsername) & "' ")
            End If

            strsql.Append(" WHERE 1=1 ")

            If Not isSuperUser AndAlso checkExistsImpresaTecnico Then
                strsql.Append(" AND U.PivaSuperUser='" & Agro_SQL_SaveText(objParametri_Utente.PivaSuperUser) & "'")
                strsql.Append(" AND U.Username = '" & Agro_SQL_SaveText(UsernameTecnico) & "' ")
                strsql.Append(" AND (U.Entita_Cod = 1) ")
            End If

            strsql.Append(" AND I.Piva NOT IN (select IC.Piva from Imprese_Codici IC where IC.id_cod = " & enum_CodiciAnagrafe.CodiceAgenzia & " ) ")
            strsql.Append(" AND I.Piva NOT IN (select CS.Valore from Configurazione_Siti CS where CS.Chiave = 'Azienda_Timesheet_Tecnici' and CS.PivaSuperUser = '" & Agro_SQL_SaveText(objParametri_Utente.PivaSuperUser) & "') ")

            strsql.Append(" ORDER BY I.rag_soc ")

            DT = EseguiQuery_Lettura(objParametri_Server, strsql.ToString, "")

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function

    '##############################################################################################
    'da usare quando si ha solo il cod_contatto e non la piva
    '(dare priorità alla piva superuser in piva)
    'si legge solo il campo rag_soc (non si concatena nome e cognome)
    Public Function RagSoc_from_CodContatto2(ByVal Cod_Contatto As String,
                                                ByVal xFiltroAggiuntivo As String,
                                                    ByRef objParametri As AgronicaCoreParametri
                                                    ) As String

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Contatti_R.Ragsoc_from_CodContatto()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New StringBuilder
        Dim DT As DataTable
        Dim Rag_Soc As String = ""

        Try

            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.Append(" SELECT Piva, Cod_Contatto, Rag_Soc ")
            StrSQL.Append(" FROM Contatti  ")
            StrSQL.Append(" WHERE Contatti.Piva = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' " & vbCrLf)

            StrSQL.Append(" AND Contatti.Cod_Contatto = '" & Agro_SQL_SaveText(Cod_Contatto) & "' " & vbCrLf)

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            StrSQL.Append("  " & vbCrLf)
            StrSQL.Append(" UNION ALL " & vbCrLf)
            StrSQL.Append("  " & vbCrLf)

            StrSQL.Append(" SELECT Piva, Cod_Contatto, Rag_Soc ")
            StrSQL.Append(" FROM Contatti  ")
            StrSQL.Append(" WHERE  Contatti.Piva <> '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' " & vbCrLf)
            StrSQL.Append(" AND Contatti.Cod_Contatto = '" & Agro_SQL_SaveText(Cod_Contatto) & "' " & vbCrLf)

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '-------------------------------------------------------------------------

            If DT IsNot Nothing AndAlso DT.Rows.Count > 0 Then
                Rag_Soc = DT.Rows(0)("Rag_Soc")
            End If

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return Rag_Soc

    End Function



    '##############################################################################################
    Public Function Numero_Contatti(ByRef Piva As String,
                                    ByVal Piva_SuperUser_Origine As String,
                                    ByVal Flag_AncheImportati As Boolean,
                                        ByVal xFiltroAggiuntivo As String,
                                        ByRef objParametri As AgronicaCoreParametri
                                        ) As Integer

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Contatti_R.Numero_Contatti()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New StringBuilder
        Dim DT As DataTable
        Dim NumContatti As Integer = 0

        Try

            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append(" SELECT COUNT(*) AS NumeroContatti ")
            StrSQL.Append(" FROM Contatti  ")
            StrSQL.Append(" WHERE 1=1 ")

            If Piva <> "" Then
                StrSQL.Append(" AND    (Contatti.Piva = '" & Agro_SQL_SaveText(Piva) & "' OR Contatti.Sa_Cod = -1 )  " & vbCrLf)
            End If

            'gias2gias
            If Piva_SuperUser_Origine <> "" Then
                StrSQL.Append(" AND  Risorse_Umane.Piva_SuperUser_Origine = '" & Agro_SQL_SaveText(Piva_SuperUser_Origine) & "'")
            End If
            If Not Flag_AncheImportati Then
                StrSQL.Append(" AND  Risorse_Umane.Cod_RisUm_Origine = " & Agro_SQL_SaveNum(0) & " ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   Contatti.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   Contatti.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '-------------------------------------------------------------------------

            If Not IsNothing(DT) AndAlso DT.Rows.Count > 0 Then
                NumContatti = DT.Rows(0).Item("NumeroContatti")
            End If

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return NumContatti

    End Function

    '##############################################################################################
    Public Function Leggi_Generico(ByVal xFiltroAggiuntivo As String,
                                   ByVal xOrderBy As String,
                                   ByRef objParametri As AgronicaCoreParametri
                                   ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Contatti_R.Leggi_Generico()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New StringBuilder
        Dim DT As DataTable

        Try

            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.AppendLine(" SELECT *")
            StrSQL.AppendLine(" FROM Contatti ")
            StrSQL.AppendLine(" WHERE 1=1 ")

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.AppendLine(" AND   Contatti.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.AppendLine(" AND   Contatti.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select

            If xOrderBy <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xOrderBy(xOrderBy))
            Else
                StrSQL.AppendLine(" ORDER BY Contatti.Rag_Soc, Contatti.Cognome, Contatti.Nome ")
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '-------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function

    '##############################################################################################


    Public Function Leggi_Tutte_Piva_Che_Hanno_Contatto(ByRef objParametri As AgronicaCoreParametri
                                        ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Contatti_R.Numero_Contatti()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New StringBuilder
        Dim DT As DataTable

        Try

            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append("  select distinct piva from Risorse_Umane ")

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '-------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function








    '#########################################################
    Public Function CodiceFiscale_from_CodContatto(
                           ByVal Piva As String,
                           ByVal Cod_Contatto As String,
                           ByVal xFiltroAggiuntivo As String,
                            ByRef objParametri As AgronicaCoreParametri
                            ) As String

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Contatti_R.CodiceFiscale_from_CodContatto()"

        Dim messaggioErrore As String = ""
        Dim dt As DataTable
        Dim Codice_Fiscale As String

        Try

            Codice_Fiscale = ""

            dt = Leggi(Piva,
                        Cod_Contatto,
                        0, 0, False,
                        False, 0, 0,
                        False, 0,
                        -99, 0, "", True,
                        0, 0, 0, 0, 0,
                        AGRODATAINIZIO,
                        AGRODATAFINE,
                        False,
                        xFiltroAggiuntivo,
                        "",
                        objParametri)

            If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then
                Codice_Fiscale = dt.Rows(0).Item("Codice_Fiscale")
            End If

            dt = Nothing

        Catch ex As Exception
            Codice_Fiscale = ""
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return Codice_Fiscale

    End Function

    '#########################################################
    Public Function LegaleRappresentante_from_PivaImpresa(
                        ByVal Piva As String,
                           ByVal xFiltroAggiuntivo As String,
                            ByRef objParametri As AgronicaCoreParametri
                            ) As String

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Contatti_R.LegaleRappresentante_from_PivaImpresa()"

        Dim messaggioErrore As String = ""
        Dim dt As DataTable
        Dim legale As String

        Try

            legale = ""

            dt = Leggi(Piva,
                        "",
                        0,
                        COD_LEGALE,
                        False,
                        False, 0, 0,
                        False, 0,
                        -99, 0, "", True,
                        0, 0, 0, 0, 1,
                        AGRODATAINIZIO,
                        AGRODATAFINE,
                        False,
                        xFiltroAggiuntivo,
                        "",
                        objParametri)

            If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then
                legale = dt.Rows(0).Item("Nome") & " " & dt.Rows(0).Item("Cognome") & " " & dt.Rows(0).Item("Rag_Soc")
            End If

            dt = Nothing

        Catch ex As Exception
            legale = ""
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return legale

    End Function

    '#########################################################
    Public Function LegaleRappresentanteCodContatto_from_PivaImpresa(
                        ByVal Piva As String,
                           ByVal xFiltroAggiuntivo As String,
                            ByRef objParametri As AgronicaCoreParametri
                            ) As String

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Contatti_R.LegaleRappresentanteCodContatto_from_PivaImpresa()"

        Dim messaggioErrore As String = ""
        Dim dt As DataTable
        Dim legale As String = ""

        Try

            legale = ""

            dt = Leggi(Piva,
                        "",
                        0,
                        COD_LEGALE,
                        False,
                        False, 0, 0,
                        False, 0,
                        -99, 0, "", True,
                        0, 0, 0, 0, 1,
                        AGRODATAINIZIO,
                        AGRODATAFINE,
                        False,
                        xFiltroAggiuntivo,
                        "",
                        objParametri)

            If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then
                legale = dt.Rows(0).Item("Cod_contatto")
            End If

            dt = Nothing

        Catch ex As Exception
            legale = ""
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return legale

    End Function


    '#########################################################
    Public Sub LegaleRappresentanteDati_from_PivaImpresa(
                        ByRef Codice_Fiscale As String,
                        ByRef Nome As String,
                        ByRef Cognome_RagSoc As String,
                        ByRef Sesso As String,
                        ByRef Data_Nascita As String,
                        ByRef IndDes_Residenza As String,
                        ByRef FrzDes_Residenza As String,
                        ByRef CAP_Residenza As String,
                        ByRef Comune_Residenza As String,
                        ByRef Provincia_Residenza As String,
                        ByRef SiglaProv_Residenza As String,
                        ByRef Com_Istat_Residenza As String,
                        ByRef Prov_Istat_Residenza As String,
                        ByRef FrzDes_Nascita As String,
                        ByRef CAP_Nascita As String,
                        ByRef Comune_Nascita As String,
                        ByRef Provincia_Nascita As String,
                        ByRef SiglaProv_Nascita As String,
                        ByRef Com_Istat_Nascita As String,
                        ByRef Prov_Istat_Nascita As String,
                        ByVal Piva As String,
                           ByVal xFiltroAggiuntivo As String,
                            ByRef objParametri As AgronicaCoreParametri
                            )

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Contatti_R.LegaleRappresentanteDati_from_PivaImpresa()"

        Dim messaggioErrore As String = ""
        Dim dt As DataTable
        Dim i As Integer

        Try

            dt = Leggi(Piva,
                        "",
                        0,
                        COD_LEGALE,
                        False,
                        True, 0, 0,
                        False, 0,
                        -99, 0, "", True,
                        0, 0, 0, 0, 1,
                        AGRODATAINIZIO,
                        AGRODATAFINE,
                        False,
                        xFiltroAggiuntivo,
                        "",
                        objParametri)

            If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then

                For i = 0 To dt.Rows.Count - 1

                    With dt.Rows(i)

                        If i = 0 Then
                            Codice_Fiscale = .Item("Cod_contatto")
                            Nome = .Item("Nome")
                            Cognome_RagSoc = .Item("Cognome") + .Item("Rag_Soc")
                            Sesso = .Item("Sesso")
                            Data_Nascita = .Item("Data_Nascita")
                        End If

                        Select Case .Item("Tipo_Indirizzo")

                            Case INDIRIZZO_RESIDENZA
                                IndDes_Residenza = .Item("Ind_Des")
                                FrzDes_Residenza = .Item("Frz_Des")
                                CAP_Residenza = .Item("CAP")
                                Comune_Residenza = .Item("com_des")
                                SiglaProv_Residenza = .Item("pro_cod")
                                Provincia_Residenza = ""
                                Com_Istat_Residenza = .Item("com_cod_istat")
                                Prov_Istat_Residenza = .Item("pro_cod_istat")

                            Case INDIRIZZO_LUOGO_NASCITA
                                FrzDes_Nascita = .Item("Frz_Des")
                                CAP_Nascita = .Item("CAP")
                                Comune_Nascita = .Item("com_des")
                                SiglaProv_Nascita = .Item("pro_cod")
                                Provincia_Nascita = ""
                                Com_Istat_Nascita = .Item("com_cod_istat")
                                Prov_Istat_Nascita = .Item("pro_cod_istat")

                        End Select

                    End With

                Next

            End If

            dt = Nothing

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

    End Sub


    Public Function Esiste_Contatto(
                            ByVal Piva As String,
                            ByVal Cod_Contatto As String,
                                ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreParametri
                                ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Contatti_R.Esiste_Contatto()"

        '====================================================================================
        'Parametri opzionali :
        '
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New StringBuilder
        Dim DT As DataTable

        Dim bRet As Boolean = False

        Try
            If Piva = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva obbligatoria)")
            End If

            If Cod_Contatto = "" Then
                Throw New Exception("Parametro non corretto nella query (Cod_Contatto obbligatorio)")
            End If
            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append(" SELECT Contatti.* ")
            StrSQL.Append(" FROM  Contatti ")
            StrSQL.Append(" WHERE Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.Append(" AND   Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            StrSQL.Append(" AND Piva = '" & Agro_SQL_SaveText(Piva) & "'  ")
            StrSQL.Append(" AND Contatti.Cod_Contatto = '" & Agro_SQL_SaveText(Cod_Contatto) & "' ")



            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   Contatti.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   Contatti.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select

            '---------------------------------------------

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            If DT IsNot Nothing Then
                If DT.Rows.Count > 0 Then
                    bRet = True
                Else
                    bRet = False
                End If
            End If


        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            bRet = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        DT = Nothing
        Return bRet

    End Function



    '###############################################
    'richiamata da FormProdotto VerificaPermessoClasseToxPatentino
    Public Function Contatti_Con_Patentino(
                        ByVal Piva As String,
                        ByRef Data_Validita As String,
                            ByRef objParametri As AgronicaCoreParametri
                            ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Contatti_R.Contatti_Con_Patentino()"

        '====================================================================================
        'Parametri opzionali :
        '
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New StringBuilder
        Dim DT As DataTable

        Dim bRet As Boolean = False

        Try
            If Piva = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva obbligatoria)")
            End If


            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT Contatti.piva, Contatti.cod_contatto, rag_soc, nome, cognome, ")
            StrSQL.AppendLine(" ISNULL( Allegati_Documenti.Allegati_Documenti_Numero, '')  AS Patentino, ISNULL(Allegati_Documenti.Validazione_Data, '01/01/1900') AS Data_Rilascio_Patentino, ISNULL(Alert_Elenco.Data_Scadenza, '31/12/2100') AS Data_Scadenza_Patentino   ")
            StrSQL.AppendLine(" FROM    Contatti  ")
            StrSQL.AppendLine(" INNER JOIN alert_entita ON alert_entita.Cod_Contatto=Contatti.Cod_Contatto AND alert_entita.piva = Contatti.piva ")
            StrSQL.AppendLine(" INNER JOIN Alert_Elenco ON Alert_Elenco.PivaSuperUser = Alert_Entita.PivaSuperUser AND Alert_Elenco.ID_Alert_Entita = Alert_Entita.ID_Alert_Entita   ")
            StrSQL.AppendLine(" INNER JOIN Allegati_Documenti ON Allegati_Documenti.Allegati_Documenti_SuperUser = Alert_Entita.PivaSuperUser and Allegati_Documenti.Allegati_Documenti_Piva = Alert_Entita.piva  and Allegati_Documenti.Allegati_Documenti_Cod = Alert_Entita.Allegati_Documenti_Cod  ")
            StrSQL.AppendLine(" WHERE tipoentita_cod=8 ")
            StrSQL.AppendLine(" AND Allegati_Documenti.Allegati_documenti_CatCod=2 ")
            StrSQL.AppendLine(" AND allegati_documenti.Allegati_Documenti_Numero <> '' ")
            StrSQL.AppendLine(" AND Allegati_Documenti.Validazione_Data <= " & Agro_SQL_SaveDate(Data_Validita) & " ")
            StrSQL.AppendLine(" AND Alert_Elenco.Data_Scadenza >= " & Agro_SQL_SaveDate(Data_Validita) & "  ")

            StrSQL.AppendLine(" AND Contatti.Piva = '" & Agro_SQL_SaveText(Piva) & "'  ")

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   Contatti.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   Contatti.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select

            '---------------------------------------------

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------


        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            bRet = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function



    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="Piva"></param>
    ''' <param name="Cod_Contatto"></param>
    ''' <param name="sa_cod">di default 99</param>
    ''' <param name="xSelezioneVariabile"></param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="xOrderBy"></param>
    ''' <param name="objParametri"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[garavini]	26/11/2010	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Function LeggiContattoSpecifico(ByVal Piva As String,
                                                   ByVal Cod_Contatto As String,
                                                   ByVal sa_cod As Integer,
                                                    ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile,
                                                    ByVal xFiltroAggiuntivo As String,
                                                    ByVal xOrderBy As String,
                                                    ByRef objParametri As AgronicaCoreParametri,
                                                    Optional ByVal TipoG2G As Integer = 0,
                                                    Optional ByVal LeggiAnchePubblici As Boolean = False
                                                       ) As DataTable

        Const NomeRoutine = "AgronicaCoreAnagrafeDAL.Contatti_R.LeggiContattoSpecifico()"

        Dim StbSQL As New StringBuilder
        Dim MessaggioErrore As String = ""
        Dim DT As New DataTable

        Try

            StbSQL.Length = 0
            '----------------------------------------------------
            '--- Preparo la Query SQL ---------------------------
            '----------------------------------------------------
            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                    StbSQL.Append(" SELECT Contatti.* ")
                    StbSQL.Append(" FROM   Contatti (NOLOCK) ")
                    StbSQL.Append(" WHERE  Contatti.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StbSQL.Append(" AND    Contatti.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    If Piva <> "" Then
                        If LeggiAnchePubblici Then
                            StbSQL.Append(" AND  (  Contatti.Piva = '" & Agro_SQL_SaveText(Piva) & "' OR Contatti.Sa_Cod = -1 )  " & vbCrLf)
                        Else
                            StbSQL.Append(" AND    Contatti.Piva = '" & Agro_SQL_SaveText(Piva) & "'  " & vbCrLf)
                        End If
                    End If

                    If Cod_Contatto <> "" Then
                        StbSQL.Append(" AND    Contatti.Cod_Contatto = '" & Agro_SQL_SaveText(Cod_Contatto) & "'   " & vbCrLf)
                    End If

                    Select Case TipoG2G
                        Case 1 'seleziona i nuovi dati.
                            StbSQL.Append(" AND not exists ( ")
                            StbSQL.Append(" select 1 from g2g_Recode_Contatti rr where rr.From_Cod_RisUm in (select Cod_RisUm from Risorse_Umane ru where ru.piva=Contatti.Piva and ru.Cod_Contatto=Contatti.Cod_Contatto) ")
                            StbSQL.Append(" ) ")

                        Case 2 'seleziona i dati modificati
                            StbSQL.Append("and exists ( " & vbCrLf)
                            StbSQL.Append(" select 1 from g2g_Recode_Contatti rr where rr.DataInvio < Contatti.Data_Modifica and rr.From_Cod_RisUm in (select Cod_RisUm from Risorse_Umane ru where ru.piva=Contatti.Piva and ru.Cod_Contatto=Contatti.Cod_Contatto) ")
                            StbSQL.Append(" ) " & vbCrLf)

                    End Select

                    If xFiltroAggiuntivo <> "" Then
                        StbSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StbSQL.Append(" AND   Contatti.Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StbSQL.Append(" AND   Contatti.Inviato =-1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StbSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StbSQL.Append(" ORDER BY Contatti.Rag_Soc ASC  " & vbCrLf)
                    End If


                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta

                    StbSQL.Append(" SELECT DISTINCT Contatti.* , Imprese.Rag_Soc as Referente ")
                    StbSQL.Append(" FROM   Contatti (NOLOCK) ")
                    StbSQL.Append(" LEFT OUTER JOIN Imprese ON (Contatti.Piva = Imprese.Piva) ")
                    StbSQL.Append(" WHERE  Contatti.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StbSQL.Append(" AND    Contatti.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")


                    If Piva <> "" Then
                        StbSQL.Append(" AND    Contatti.Piva = '" & Agro_SQL_SaveText(Piva) & "'  " & vbCrLf)
                    End If

                    If Cod_Contatto <> "" Then
                        StbSQL.Append(" AND    (Contatti.Cod_Contatto = '" & Agro_SQL_SaveText(Cod_Contatto) & "')   " & vbCrLf)
                    End If

                    If sa_cod <> 99 Then
                        StbSQL.Append(" AND Contatti.sa_cod = " & Agro_SQL_SaveNum(sa_cod) & "  ")
                    End If

                    Select Case TipoG2G
                        Case 1 'seleziona i nuovi dati.
                            StbSQL.Append(" AND not exists ( ")
                            StbSQL.Append(" select 1 from g2g_Recode_Contatti rr where rr.From_Cod_RisUm in (select Cod_RisUm from Risorse_Umane ru where ru.piva=Contatti.Piva and ru.Cod_Contatto=Contatti.Cod_Contatto) ")
                            StbSQL.Append(" ) ")

                        Case 2 'seleziona i dati modificati
                            StbSQL.Append("and exists ( " & vbCrLf)
                            StbSQL.Append(" select 1 from g2g_Recode_Contatti rr where rr.DataInvio < Contatti.Data_Modifica and rr.From_Cod_RisUm in (select Cod_RisUm from Risorse_Umane ru where ru.piva=Contatti.Piva and ru.Cod_Contatto=Contatti.Cod_Contatto) ")
                            StbSQL.Append(" ) " & vbCrLf)

                    End Select

                    If xFiltroAggiuntivo <> "" Then
                        StbSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StbSQL.Append(" AND   Contatti.Inviato >=0 ")
                            StbSQL.Append(" AND   Imprese.Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StbSQL.Append(" AND   Contatti.Inviato =-1 ")
                            StbSQL.Append(" AND   Imprese.Inviato =-1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StbSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StbSQL.Append(" ORDER BY Contatti.Rag_Soc ASC  " & vbCrLf)
                    End If

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta


            End Select

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StbSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
        End Try

        Return DT

    End Function

    '#########################################################
    'per non filtrare il sa_cod, passare il valore SACOD_NOFILTRO
    'perché 0 è significativo
    Public Function LeggiContattiNoImpreseGias(ByVal Piva As String,
                                                  ByVal Cod_Contatto As String,
                                                  ByVal Sa_Cod As Integer,
                                                      ByVal xFiltroAggiuntivo As String,
                                                      ByVal xOrderBy As String,
                                                      ByRef objParametri As AgronicaCoreParametri
                                                      ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Contatti_R.LeggiContattiNoImpreseGias()"


        Dim StbSQL As New StringBuilder
        Dim MessaggioErrore As String = ""
        Dim DT As New DataTable

        Try

            StbSQL.Length = 0

            StbSQL.Append(" SELECT Contatti.* ")
            StbSQL.Append(" FROM   Contatti ")
            StbSQL.Append(" WHERE  Contatti.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StbSQL.Append(" AND    Contatti.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            StbSQL.Append(" AND NOT EXISTS ")
            StbSQL.Append("         ( SELECT piva FROM Imprese WHERE   Contatti.Cod_Contatto=imprese.piva ) ")

            If Piva <> "" Then
                StbSQL.Append(" AND    Contatti.Piva = '" & Agro_SQL_SaveText(Piva) & "'  " & vbCrLf)
            End If

            If Cod_Contatto <> "" Then
                StbSQL.Append(" AND    Contatti.Cod_Contatto = '" & Agro_SQL_SaveText(Cod_Contatto) & "'   " & vbCrLf)
            End If

            If Sa_Cod <> SACOD_NOFILTRO Then
                StbSQL.Append(" AND    Contatti.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   " & vbCrLf)
            End If

            If xFiltroAggiuntivo <> "" Then
                StbSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StbSQL.Append(" AND   Contatti.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StbSQL.Append(" AND   Contatti.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StbSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StbSQL.Append(" ORDER BY Contatti.Rag_Soc ASC  " & vbCrLf)
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StbSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing

        End Try

        Return DT


    End Function



    '##############################################################################################
    Public Sub Recupera_DatiContatto(
                                    ByVal Mat_Cod As Integer,
                                    ByRef Rapporto_Des As String,
                                    ByRef Rag_Soc As String,
                                    ByRef Corrispettivo_Orario As Decimal,
                                    ByRef Udm_Des As String,
                                    ByVal Cod_RisUm_Origine As Integer,
                                    ByVal Piva_SuperUser_Origine As String,
                                    ByVal Flag_AncheImportati As Boolean,
                                        ByVal xFiltroAggiuntivo As String,
                                        ByRef objParametri As AgronicaCoreParametri
                                        )

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Contatti_R.Recupera_DatiContatto()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New StringBuilder
        Dim DT As DataTable

        Dim bRet As Boolean = False

        Try

            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append(" SELECT Risorse_Umane.Cod_RisUm, Rapporti_Contabili.Rapporto_Des, ")
            StrSQL.Append(" CASE WHEN (Contatti.Rag_Soc is null or Contatti.Rag_Soc = '') then Contatti.Nome  + ' ' +  Contatti.Cognome ")
            StrSQL.Append(" else Contatti.Rag_Soc                    End    AS Rag_Soc , ")
            StrSQL.Append(" Prodotti_Costi.Mezzo, Prodotti_Costi.Prezzo_Unitario as corrispettivo_orario, ISNULL(Prodotti_Costi.Elem_Cod, 0) AS Elem_Cod ")
            StrSQL.Append(" FROM Risorse_Umane INNER JOIN ")
            StrSQL.Append(" Contatti ON Risorse_Umane.Cod_Contatto = Contatti.Cod_Contatto AND Risorse_Umane.Piva = Contatti.Piva INNER JOIN ")
            StrSQL.Append(" Rapporti_Contabili ON Risorse_Umane.Cod_Rapporto = Rapporti_Contabili.Cod_Rapporto ")
            StrSQL.Append(" INNER JOIN UtentiXImprese  ON Contatti.Piva = UtentiXImprese.PIVA AND Rapporti_Contabili.Piva = UtentiXImprese.[USER] ")
            StrSQL.Append(" LEFT OUTER JOIN Prodotti_Costi ON Risorse_Umane.Cod_RisUm = Prodotti_Costi.Mat_Cod And Prodotti_Costi.Id_Budget = 0")
            StrSQL.Append(" WHERE(Risorse_Umane.Cod_RisUm = " & Agro_SQL_SaveNum(Mat_Cod) & ")")
            StrSQL.Append(" AND    UtentiXImprese.[USER] = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.Append(" AND    Elem_Cod = 0 ")

            'gias2gias
            If Cod_RisUm_Origine <> 0 Then
                StrSQL.Append(" AND  Risorse_Umane.Cod_RisUm_Origine = " & Agro_SQL_SaveNum(Cod_RisUm_Origine) & "   ")
            End If
            If Piva_SuperUser_Origine <> "" Then
                StrSQL.Append(" AND  Risorse_Umane.Piva_SuperUser_Origine = '" & Agro_SQL_SaveText(Piva_SuperUser_Origine) & "'")
            End If
            If Not Flag_AncheImportati Then
                StrSQL.Append(" AND  Risorse_Umane.Cod_RisUm_Origine = " & Agro_SQL_SaveNum(0) & " ")
            End If


            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   Risorse_Umane.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   Risorse_Umane.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select

            '---------------------------------------------

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '-------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            bRet = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try


        'recupero le descrizioni che mi servono
        If DT.Rows.Count > 0 Then
            Rapporto_Des = DT.Rows(0).Item("rapporto_des")

            Rag_Soc = DT.Rows(0).Item("rag_soc")

            If Not IsDBNull(DT.Rows(0).Item("corrispettivo_orario")) Then
                Corrispettivo_Orario = CDbl(DT.Rows(0).Item("corrispettivo_orario"))
            Else
                Corrispettivo_Orario = CDbl(0)
            End If

            If IsDBNull(DT.Rows(0).Item("mezzo")) Then
                Udm_Des = "indefinito"
            Else
                Select Case DT.Rows(0).Item("mezzo")
                    Case enum_TipoMezzo.Indefinito
                        Udm_Des = "indefinito"
                    Case enum_TipoMezzo.Ettaro
                        Udm_Des = "ha"
                    Case enum_TipoMezzo.Ora
                        Udm_Des = "ora"
                End Select
            End If



        End If

    End Sub


    ' a differenza di quella prima recupera i dati anche se non è impostato il costo in prodotti costi.
    '##############################################################################################
    Public Sub Recupera_DatiContatto2(
                                    ByVal Mat_Cod As Integer,
                                    ByRef Rapporto_Des As String,
                                    ByRef Rag_Soc As String,
                                    ByRef Corrispettivo_Orario As Decimal,
                                    ByRef Udm_Des As String,
                                    ByVal Cod_RisUm_Origine As Integer,
                                    ByVal Piva_SuperUser_Origine As String,
                                    ByVal Flag_AncheImportati As Boolean,
                                    ByVal xFiltroAggiuntivo As String,
                                    ByRef objParametri As AgronicaCoreParametri,
                                    Optional ByRef Cod_Rapporto As Integer = 0
                                    )

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Contatti_R.Recupera_DatiContatto()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New StringBuilder
        Dim DT As DataTable

        Dim bRet As Boolean = False

        Try

            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append(" SELECT Risorse_Umane.Cod_RisUm, Rapporti_Contabili.Cod_Rapporto, Rapporti_Contabili.Rapporto_Des, ")
            StrSQL.Append(" CASE WHEN (Contatti.Rag_Soc is null or Contatti.Rag_Soc = '') then Contatti.Cognome  + ' ' +  Contatti.Nome ")
            StrSQL.Append(" else Contatti.Rag_Soc END AS Rag_Soc, ")

            'correzione del 25/07/2014: va gestito l'intervallo temporale del costo
            'ed evitare l'errore in caso di record duplicati (top 1)

            ' StrSQL.Append(" ISNULL((SELECT Mezzo FROM Prodotti_Costi WHERE Risorse_Umane.Cod_RisUm = Prodotti_Costi.Mat_Cod AND Elem_Cod=0), 0) AS Mezzo, ")

            StrSQL.Append(" ISNULL((SELECT TOP 1 Mezzo  ")
            StrSQL.Append("         FROM Prodotti_Costi  ")
            StrSQL.Append("         WHERE Risorse_Umane.Cod_RisUm = Prodotti_Costi.Mat_Cod  ")
            StrSQL.Append("         AND Elem_Cod = 0 And Id_Budget = 0 ")
            StrSQL.Append("         AND Prodotti_Costi.Validita_Inizio <= " & Agro_SQL_SaveDate(Date.Today) & " " & vbCrLf)
            StrSQL.Append("         AND Prodotti_Costi.Validita_Fine >= " & Agro_SQL_SaveDate(Date.Today) & " " & vbCrLf)
            StrSQL.Append("         ), 0) AS Mezzo, ")

            'StrSQL.Append(" ISNULL((SELECT Prezzo_Unitario FROM Prodotti_Costi WHERE Risorse_Umane.Cod_RisUm = Prodotti_Costi.Mat_Cod AND Elem_Cod=0), 0) AS corrispettivo_orario, ")

            StrSQL.Append(" ISNULL((SELECT TOP 1 Prezzo_Unitario  ")
            StrSQL.Append("         FROM Prodotti_Costi  ")
            StrSQL.Append("         WHERE Risorse_Umane.Cod_RisUm = Prodotti_Costi.Mat_Cod  ")
            StrSQL.Append("         AND Elem_Cod = 0 And Id_Budget = 0 ")
            StrSQL.Append("         AND Prodotti_Costi.Validita_Inizio <= " & Agro_SQL_SaveDate(Date.Today) & " " & vbCrLf)
            StrSQL.Append("         AND Prodotti_Costi.Validita_Fine >= " & Agro_SQL_SaveDate(Date.Today) & " " & vbCrLf)
            StrSQL.Append("         ), 0) AS corrispettivo_orario, ")

            StrSQL.Append(" 0 AS Elem_Cod ")    ' è sempre a 0, perché è un contatto
            StrSQL.Append(" FROM Risorse_Umane INNER JOIN ")
            StrSQL.Append(" Contatti ON Risorse_Umane.Cod_Contatto = Contatti.Cod_Contatto AND Risorse_Umane.Piva = Contatti.Piva INNER JOIN ")
            StrSQL.Append(" Rapporti_Contabili ON Risorse_Umane.Cod_Rapporto = Rapporti_Contabili.Cod_Rapporto ")
            StrSQL.Append(" INNER JOIN UtentiXImprese  ON Contatti.Piva = UtentiXImprese.PIVA AND Rapporti_Contabili.Piva = UtentiXImprese.[USER] ")
            'StrSQL.Append(" LEFT OUTER JOIN Prodotti_Costi ON Risorse_Umane.Cod_RisUm = Prodotti_Costi.Mat_Cod ")
            StrSQL.Append(" WHERE (Risorse_Umane.Cod_RisUm = " & Agro_SQL_SaveNum(Mat_Cod) & ")")
            StrSQL.Append(" AND    UtentiXImprese.[USER] = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

            'gias2gias
            If Cod_RisUm_Origine <> 0 Then
                StrSQL.Append(" AND  Risorse_Umane.Cod_RisUm_Origine = " & Agro_SQL_SaveNum(Cod_RisUm_Origine) & "   ")
            End If
            If Piva_SuperUser_Origine <> "" Then
                StrSQL.Append(" AND  Risorse_Umane.Piva_SuperUser_Origine = '" & Agro_SQL_SaveText(Piva_SuperUser_Origine) & "'")
            End If
            If Not Flag_AncheImportati Then
                StrSQL.Append(" AND  Risorse_Umane.Cod_RisUm_Origine = " & Agro_SQL_SaveNum(0) & " ")
            End If


            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   Risorse_Umane.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   Risorse_Umane.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select

            '---------------------------------------------

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '-------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            bRet = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Rapporto_Des = ""
        Rag_Soc = ""
        'recupero le descrizioni che mi servono
        If DT.Rows.Count > 0 Then
            Rapporto_Des = DT.Rows(0).Item("rapporto_des")
            Cod_Rapporto = DT.Rows(0).Item("Cod_Rapporto")

            Rag_Soc = DT.Rows(0).Item("rag_soc")

            If Not IsDBNull(DT.Rows(0).Item("corrispettivo_orario")) Then
                Corrispettivo_Orario = CDbl(DT.Rows(0).Item("corrispettivo_orario"))
            Else
                Corrispettivo_Orario = CDbl(0)
            End If

            If IsDBNull(DT.Rows(0).Item("mezzo")) Then
                Udm_Des = "indefinito"
            Else
                Select Case DT.Rows(0).Item("mezzo")
                    Case enum_TipoMezzo.Indefinito
                        Udm_Des = "indefinito"
                    Case enum_TipoMezzo.Ettaro
                        Udm_Des = "ha"
                    Case enum_TipoMezzo.Ora
                        Udm_Des = "ora"
                End Select
            End If



        End If




    End Sub
    Public Function Leggi_Contatti_Azienda(ByVal Cod_Contatto As String, ByRef objParametri As AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Contatti_R.Leggi_Contatti_Azienda()"

        Dim StbSQL As New StringBuilder
        Dim MessaggioErrore As String = ""
        Dim DT As New DataTable

        Try

            '----------------------------------------------------
            '--- Preparo la Query SQL ---------------------------
            '----------------------------------------------------
            StbSQL.Length = 0


            StbSQL.AppendLine(" SELECT  Imprese.Rag_Soc AS Proprietario, Contatti.Piva AS PivaProprietario, Contatti.Sa_Cod, Contatti.Cod_Contatto,")
            'StbSQL.AppendLine(" Risorse_Umane.Sa_Cod AS Sa_Cod_2, Risorse_Umane.Cod_RisUm,")
            StbSQL.AppendLine(" Risorse_Umane.Settore_Des, Risorse_Umane.Attivita_Des,")
            StbSQL.AppendLine(" Rapporti_Contabili.Cod_Rapporto, ")
            StbSQL.AppendLine(" COALESCE(Rapporti_Contabili.Rapporto_Des, '') as Rapporto_Des")

            StbSQL.AppendLine(" FROM    Contatti  ")
            StbSQL.AppendLine(" INNER JOIN Risorse_Umane ON Contatti.Cod_Contatto = Risorse_Umane.Cod_Contatto AND Contatti.Piva = Risorse_Umane.Piva ")
            StbSQL.AppendLine(" INNER JOIN  Rapporti_Contabili ON Risorse_Umane.Cod_Rapporto = Rapporti_Contabili.Cod_Rapporto ")

            'JOIN IMPRESE 
            StbSQL.AppendLine(" INNER JOIN Imprese ON Imprese.Piva = Contatti.Piva ")

            StbSQL.AppendLine(" WHERE 1 = 1")

            If Cod_Contatto <> "" Then
                StbSQL.AppendLine(" AND    (Contatti.Cod_Contatto = '" & Agro_SQL_SaveText(Cod_Contatto) & "')   ")
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StbSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function

    '#######################################################################
    'default:
    'ID_CF = -99 (costante ID_CF_NOFILTRO)
    Public Function Contatti_Contatto_Leggi(ByVal Piva As String,
                                                ByVal Cod_Contatto As String,
                                                ByVal Cod_RisUm As Integer,
                                                ByVal Cod_Rapporto As Integer,
                                                ByVal FlagPubblico As Boolean,
                                                ByVal FlagIndirizzi As Boolean,
                                                ByVal TipoIndirizzo As Integer,
                                                ByVal CodIndirizzo As Integer,
                                                ByVal FlagRubrica As Boolean,
                                                ByVal CodRubrica As Integer,
                                                ByVal ID_CF As Integer,
                                                ByVal Cod_RisUm_Origine As Integer,
                                                ByVal Piva_SuperUser_Origine As String,
                                                ByVal Flag_AncheImportati As Boolean,
                                                ByVal Cliente As Integer,
                                                ByVal Fornitore As Integer,
                                                ByVal Dipendente As Integer,
                                                ByVal Terzista As Integer,
                                                ByVal Legale As Integer,
                                                ByVal xSelezioneVariabile As AgronicaCoreParametri.enumSelezioneVariabile,
                                                    ByVal xFiltroAggiuntivo As String,
                                                    ByVal xOrderBy As String,
                                                    ByRef objParametri As AgronicaCoreParametri,
                                                    Optional ByVal Progressivo As String = "",
                                            Optional ByVal leggi_NrBadge As Boolean = False,
                                            Optional ByVal Agente As Integer = 0,
                                            Optional ByVal Lista_Cod_Contatto As List(Of String) = Nothing,
                                            Optional ByVal leggi_Patentino As Boolean = False
                                                    ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Contatti_R.Contatti_Contatto_Leggi()"

        Dim StbSQL As New StringBuilder
        Dim MessaggioErrore As String = ""
        Dim DT As New DataTable

        Try

            '----------------------------------------------------
            '--- Preparo la Query SQL ---------------------------
            '----------------------------------------------------
            StbSQL.Length = 0


            StbSQL.Append(" SELECT  Imprese.Rag_Soc AS Impresa, Contatti.Piva, Contatti.Sa_Cod, Contatti.Cod_Contatto, Contatti.Codice_Fiscale, Contatti.Id_CF, Contatti.Rag_Soc, Contatti.Convenevoli, " & vbCrLf)
            StbSQL.Append(" Contatti.Nome, Contatti.Cognome, Contatti.Data_Nascita, Contatti.Sesso, Contatti.Cod_Contatto_Referente, ISNULL(contatti.ChkFittizio, 0) AS ChkFittizio, " & vbCrLf)
            StbSQL.Append(" Risorse_Umane.Sa_Cod AS Sa_Cod_2, Risorse_Umane.Cod_RisUm, Risorse_Umane.Validita_Inizio, Risorse_Umane.Validita_Fine, " & vbCrLf)
            StbSQL.Append(" Risorse_Umane.Settore_Des, Risorse_Umane.Attivita_Des, Risorse_Umane.Corrispettivo_Mensile, Risorse_Umane.Corrispettivo_Orario, " & vbCrLf)
            StbSQL.Append(" Risorse_Umane.Occasionale, Risorse_Umane.Ore_Settimanali, Risorse_Umane.Giorni_Ferie, Risorse_Umane.Ferie_Godute, " & vbCrLf)
            StbSQL.Append(" Risorse_Umane.Giorni_Malattia, Risorse_Umane.Patentino, Risorse_Umane.Data_Rilascio_Patentino, Risorse_Umane.Data_Scadenza_Patentino, " & vbCrLf)
            StbSQL.Append(" Rapporti_Contabili.Piva AS Piva_SuperUser, Rapporti_Contabili.Sa_Cod AS Sa_Cod_3, Rapporti_Contabili.Cod_Rapporto, " & vbCrLf)
            StbSQL.Append(" COALESCE(Rapporti_Contabili.Rapporto_Des, '') as Rapporto_Des, Rapporti_Contabili.Cliente,  " & vbCrLf)
            StbSQL.Append(" Rapporti_Contabili.Fornitore, Rapporti_Contabili.Dipendente, Rapporti_Contabili.Terzista, Rapporti_Contabili.Legale, Rapporti_Contabili.Agente, Rapporti_Contabili.Consulente, Contatti.Tipo_Indirizzo_Default, " & vbCrLf)

            StbSQL.Append(" cast((  ISNULL ((SELECT TOP 1   val_cod  ")
            StbSQL.Append("          FROM        Contatti_Codici WITH(NOLOCK)")
            StbSQL.Append("          WHERE       Contatti.PIVA = Contatti_Codici.PIVA And Contatti.Cod_Contatto = Contatti_Codici.Cod_Contatto And Contatti_Codici.id_cod = " & Agro_SQL_SaveNum(CStr(enum_DatiAnagrafici_CodiciAnagrafe.Rappr_Legale)) & "), '0')) as nvarchar(255))  AS Modifica ")
            StbSQL.Append(", Contatti.Nome_Breve ")

            If FlagIndirizzi Then
                StbSQL.Append(", ContattiXIndirizzi.Cod_Indirizzo, ContattiXIndirizzi.Tipo_Indirizzo, Indirizzi.ind_des, Indirizzi.frz_des, ISNULL(Indirizzi.CAP, '') AS CAP, ISNULL(ISTAT.LOCALITA, '') AS com_des, ISNULL(ISTAT.COMUNI_PROV, '') AS pro_cod, Indirizzi.stato, Indirizzi.note, Indirizzi.pro_cod_istat, Indirizzi.com_cod_istat " & vbCrLf)

                StbSQL.Append(" , cast(( ISNULL ((SELECT TOP 1   Descrizione  ")
                StbSQL.Append("          FROM        ACCDAA_ANAG_T005_TabellaCodiciPaesiTerziISO3166 WITH(NOLOCK)")
                StbSQL.Append("          WHERE      Indirizzi.stato = ACCDAA_ANAG_T005_TabellaCodiciPaesiTerziISO3166.Codice ), '')) as NVarChar(255)) AS Stato_PaesiTerzi ")

                StbSQL.Append(" , cast(( ISNULL ((SELECT TOP 1   Descrizione  ")
                StbSQL.Append("          FROM        ACCDAA_ANAG_T004_TabellaCodiciStatiMembri WITH(NOLOCK)")
                StbSQL.Append("          WHERE      Indirizzi.stato = ACCDAA_ANAG_T004_TabellaCodiciStatiMembri.Codice ) , '')) as NvarChar(255)) AS Stato_PaesiMembri ")

            End If

            If FlagRubrica Then
                StbSQL.Append(", ISNULL(ContattiXRubrica.Cod_Rubrica,0) AS Cod_Rubrica, ISNULL(Rubrica.numero,'') AS Numero, ISNULL(Rubrica.descr,'') AS descr" & vbCrLf)
            End If

            If leggi_NrBadge Then
                StbSQL.Append(", ISNULL(substring(nrbadge, patindex('%[^0]%',Contatti.nrbadge), 10), '') AS NrBadge " & vbCrLf)
            End If

            If leggi_Patentino Then
                StbSQL.Append(", (SELECT top 1 ISNULL(Allegati_Documenti.Allegati_Documenti_Numero,'N/D')+'|'+ISNULL(CONVERT(VARCHAR(10), Allegati_Documenti.Validazione_Data, 23), '1900-01-01')+'|'+ISNULL(CONVERT(VARCHAR(10), Alert_Elenco.Data_Scadenza, 23), '2100-12-31') From alert_entita  
                    INNER JOIN Alert_Elenco ON Alert_Elenco.PivaSuperUser = Alert_Entita.PivaSuperUser And Alert_Elenco.ID_Alert_Entita = Alert_Entita.ID_Alert_Entita
                    INNER JOIN Allegati_Documenti ON Allegati_Documenti.Allegati_Documenti_SuperUser = Alert_Entita.PivaSuperUser and Allegati_Documenti.Allegati_Documenti_Piva = Alert_Entita.piva  and
                    Allegati_Documenti.Allegati_Documenti_Cod = Alert_Entita.Allegati_Documenti_Cod
                    WHERE tipoentita_cod=8 And Allegati_Documenti.Allegati_documenti_CatCod=2  
                    And REPLACE(LTRIM(RTRIM(alert_entita.Cod_Contatto)), char(9), '') = REPLACE(LTRIM(RTRIM(Contatti.Cod_Contatto)), char(9), '') and alert_entita.piva = Contatti.piva
                    order by Alert_Elenco.Data_Scadenza desc) AS DatiPatentino" & vbCrLf)
            End If

            StbSQL.Append(" FROM    Contatti WITH(NOLOCK) " & vbCrLf)
            StbSQL.Append(" INNER JOIN Risorse_Umane WITH(NOLOCK)" & vbCrLf)
            StbSQL.Append(" ON Contatti.Cod_Contatto = Risorse_Umane.Cod_Contatto And Contatti.Piva = Risorse_Umane.Piva " & vbCrLf)
            StbSQL.Append(" INNER JOIN  Rapporti_Contabili WITH(NOLOCK)" & vbCrLf)
            StbSQL.Append(" On Risorse_Umane.Cod_Rapporto = Rapporti_Contabili.Cod_Rapporto " & vbCrLf)
            StbSQL.Append(" INNER JOIN UtentiXImprese ON Rapporti_Contabili.Piva = UtentiXImprese.[USER] And Risorse_Umane.Piva = UtentiXImprese.PIVA " & vbCrLf)

            'JOIN IMPRESE 
            StbSQL.Append(" INNER JOIN Imprese WITH(NOLOCK)" & vbCrLf)
            StbSQL.Append(" ON Imprese.Piva = Contatti.Piva " & vbCrLf)

            If FlagIndirizzi Then
                StbSQL.Append("  INNER JOIN  ContattiXIndirizzi WITH(NOLOCK)" & vbCrLf)
                StbSQL.Append("  ON Contatti.Piva = ContattiXIndirizzi.Piva And Contatti.Cod_Contatto = ContattiXIndirizzi.Cod_Contatto " & vbCrLf)
                StbSQL.Append("  INNER JOIN Indirizzi WITH(NOLOCK)" & vbCrLf)
                StbSQL.Append("  ON ContattiXIndirizzi.Cod_Indirizzo = Indirizzi.cod_indirizzo " & vbCrLf)
                StbSQL.Append("  LEFT OUTER JOIN ISTAT WITH(NOLOCK)" & vbCrLf)
                StbSQL.Append("  ON Indirizzi.pro_cod_istat = ISTAT.PROV And Indirizzi.com_cod_istat = ISTAT.COM " & vbCrLf)
            End If

            If FlagRubrica Then
                StbSQL.Append(" LEFT OUTER JOIN ContattiXRubrica WITH(NOLOCK) " & vbCrLf)
                StbSQL.Append(" ON ContattiXRubrica.Piva = Contatti.Piva And ContattiXRubrica.Cod_Contatto = Contatti.Cod_Contatto " & vbCrLf)
                StbSQL.Append(" LEFT OUTER JOIN Rubrica WITH(NOLOCK)" & vbCrLf)
                StbSQL.Append(" ON Rubrica.cod_rubrica = ContattiXRubrica.Cod_Rubrica" & vbCrLf)
            End If

            StbSQL.Append(" ")
            StbSQL.Append(" ")

            StbSQL.Append(" WHERE   (Rapporti_Contabili.Piva = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "')   " & vbCrLf)

            StbSQL.Append(" AND ( Risorse_Umane.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ) " & vbCrLf)
            StbSQL.Append(" AND ( Risorse_Umane.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ) " & vbCrLf)

            If Cod_Contatto <> "" Then
                StbSQL.Append(" AND    (Contatti.Cod_Contatto = '" & Agro_SQL_SaveText(Cod_Contatto) & "')   " & vbCrLf)
            End If

            If Not IsNothing(Lista_Cod_Contatto) Then
                StbSQL.Append(" AND (Contatti.Cod_Contatto In " & Agro_SQL_Save_Clausola_IN("('" & Join(Lista_Cod_Contatto.ToArray, "', '") & "')", True) & "  )" & vbCrLf)
            End If

            If Not FlagPubblico Then
                If Piva <> "" Then
                    StbSQL.Append(" AND    (Contatti.Piva = '" & Agro_SQL_SaveText(Piva) & "')   " & vbCrLf)
                End If
            Else
                If Piva <> "" Then
                    StbSQL.Append(" AND    (Contatti.Piva = '" & Agro_SQL_SaveText(Piva) & "' OR Contatti.Sa_Cod = -1 )  " & vbCrLf)
                End If
            End If

            If ID_CF <> ID_CF_NOFILTRO Then
                StbSQL.Append(" AND     (Contatti.ID_CF = " & Agro_SQL_SaveNum(ID_CF.ToString) & ")   " & vbCrLf)
            End If

            If Cod_RisUm <> 0 Then
                StbSQL.Append(" AND     (Cod_RisUm = " & Agro_SQL_SaveNum(Cod_RisUm.ToString) & ")   " & vbCrLf)
            End If

            If Cod_Rapporto <> 0 Then
                StbSQL.Append(" AND     (Rapporti_Contabili.Cod_Rapporto = " & Agro_SQL_SaveNum(Cod_Rapporto.ToString) & ")   " & vbCrLf)
            End If

            If FlagIndirizzi Then
                If TipoIndirizzo <> 0 Then
                    StbSQL.Append(" AND     (ContattiXIndirizzi.Tipo_Indirizzo = " & Agro_SQL_SaveNum(TipoIndirizzo.ToString) & ")   " & vbCrLf)
                End If
                If CodIndirizzo <> 0 Then
                    StbSQL.Append(" AND     (ContattiXIndirizzi.Cod_Indirizzo = " & Agro_SQL_SaveNum(CodIndirizzo.ToString) & ")   " & vbCrLf)
                End If
            End If

            If FlagRubrica Then
                If CodRubrica <> 0 Then
                    StbSQL.Append(" AND     (Rubrica.Cod_Rubrica = " & Agro_SQL_SaveNum(CodRubrica.ToString) & ")   " & vbCrLf)
                End If
            End If

            'gias2gias
            If Cod_RisUm_Origine <> 0 Then
                StbSQL.Append(" AND  Risorse_Umane.Cod_RisUm_Origine = " & Agro_SQL_SaveNum(Cod_RisUm_Origine.ToString) & "   " & vbCrLf)
            End If
            If Piva_SuperUser_Origine <> "" Then
                StbSQL.Append(" AND  Risorse_Umane.Piva_SuperUser_Origine = '" & Agro_SQL_SaveText(Piva_SuperUser_Origine) & "'" & vbCrLf)
            End If
            If Not Flag_AncheImportati Then
                StbSQL.Append(" AND  Risorse_Umane.Cod_RisUm_Origine = 0 " & vbCrLf)
            End If
            'fine gias2gias

            If Cliente <> 0 Then
                StbSQL.Append(" AND Rapporti_Contabili.Cliente = 1 " & vbCrLf)
            End If

            If Fornitore <> 0 Then
                StbSQL.Append(" AND Rapporti_Contabili.Fornitore = 1 " & vbCrLf)
            End If

            If Dipendente <> 0 Then
                StbSQL.Append(" AND Rapporti_Contabili.Dipendente = 1 " & vbCrLf)
            End If

            If Terzista <> 0 Then
                StbSQL.Append(" AND Rapporti_Contabili.Terzista = 1 " & vbCrLf)
            End If

            If Legale <> 0 Then
                StbSQL.Append(" AND Rapporti_Contabili.Legale = 1 " & vbCrLf)
            End If

            If Progressivo <> "" Then
                StbSQL.Append(" AND  Risorse_Umane.Settore_Des = '" & Agro_SQL_SaveText(Progressivo) & "'   " & vbCrLf)
            End If

            If Agente <> 0 Then
                StbSQL.Append(" AND Rapporti_Contabili.Agente = 1 AND Rapporti_Contabili.Cod_Rapporto <> " & enum_Rapporti_Contabili_Standard.Capo_Area & " " & vbCrLf)
            End If

            If xFiltroAggiuntivo <> "" Then
                StbSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StbSQL.Append(" AND   Contatti.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StbSQL.Append(" AND   Contatti.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StbSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StbSQL.Append(" ORDER BY Contatti.Rag_Soc ASC  " & vbCrLf)
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StbSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function


    'default:
    'ID_CF = -99 (costante ID_CF_NOFILTRO)
    Public Function Leggi_Distinct_Contatti(ByVal Piva As String,
                                                ByVal Cod_Contatto As String,
                                                ByVal Cod_RisUm As Integer,
                                                ByVal Cod_Rapporto As Integer,
                                                ByVal FlagPubblico As Boolean,
                                                ByVal ID_CF As Integer,
                                                ByVal Cod_RisUm_Origine As Integer,
                                                ByVal Piva_SuperUser_Origine As String,
                                                ByVal Flag_AncheImportati As Boolean,
                                                ByVal Progressivo As String,
                                                ByVal Cliente As Integer,
                                                ByVal Fornitore As Integer,
                                                ByVal Dipendente As Integer,
                                                ByVal Terzista As Integer,
                                                ByVal Legale As Integer,
                                                ByVal Agente As Integer,
                                                ByVal Consulente As Integer,
                                                ByVal xFiltroAggiuntivo As String,
                                                    ByVal xOrderBy As String,
                                                    ByRef objParametri As AgronicaCoreParametri
                                                    ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Contatti_R.Contatti_Contatto_Leggi()"

        Dim StbSQL As New StringBuilder
        Dim MessaggioErrore As String = ""
        Dim DT As New DataTable

        Try

            '----------------------------------------------------
            '--- Preparo la Query SQL ---------------------------
            '----------------------------------------------------
            StbSQL.Length = 0

            StbSQL.Append(" SELECT DISTINCT Imprese.Rag_Soc AS Impresa, Contatti.* " & vbCrLf)

            StbSQL.Append(" FROM    Contatti  " & vbCrLf)
            StbSQL.Append(" INNER JOIN Risorse_Umane ON Contatti.Cod_Contatto = Risorse_Umane.Cod_Contatto AND Contatti.Piva = Risorse_Umane.Piva " & vbCrLf)
            StbSQL.Append(" INNER JOIN  Rapporti_Contabili ON Risorse_Umane.Cod_Rapporto = Rapporti_Contabili.Cod_Rapporto " & vbCrLf)
            StbSQL.Append(" INNER JOIN UtentiXImprese ON Rapporti_Contabili.Piva = UtentiXImprese.[USER] AND Risorse_Umane.Piva = UtentiXImprese.PIVA " & vbCrLf)

            'JOIN IMPRESE 
            StbSQL.Append(" INNER JOIN Imprese ON Imprese.Piva = Contatti.Piva " & vbCrLf)

            StbSQL.Append(" WHERE   (Rapporti_Contabili.Piva = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "')   " & vbCrLf)

            If Cod_Contatto <> "" Then
                StbSQL.Append(" AND    (Contatti.Cod_Contatto = '" & Agro_SQL_SaveText(Cod_Contatto) & "')   " & vbCrLf)
            End If

            If Not FlagPubblico Then
                If Piva <> "" Then
                    StbSQL.Append(" AND    (Contatti.Piva = '" & Agro_SQL_SaveText(Piva) & "')   " & vbCrLf)
                End If
            Else
                If Piva <> "" Then
                    StbSQL.Append(" AND    (Contatti.Piva = '" & Agro_SQL_SaveText(Piva) & "' OR Contatti.Sa_Cod = -1 )  " & vbCrLf)
                End If
            End If

            If ID_CF <> ID_CF_NOFILTRO Then
                StbSQL.Append(" AND     (Contatti.ID_CF = " & Agro_SQL_SaveNum(ID_CF.ToString) & ")   " & vbCrLf)
            End If

            If Cod_RisUm <> 0 Then
                StbSQL.Append(" AND     (Cod_RisUm = " & Agro_SQL_SaveNum(Cod_RisUm.ToString) & ")   " & vbCrLf)
            End If

            If Cod_Rapporto <> 0 Then
                StbSQL.Append(" AND     (Rapporti_Contabili.Cod_Rapporto = " & Agro_SQL_SaveNum(Cod_Rapporto.ToString) & ")   " & vbCrLf)
            End If

            If Progressivo <> "" Then
                StbSQL.Append(" AND  Risorse_Umane.Settore_Des = '" & Agro_SQL_SaveText(Progressivo) & "'   " & vbCrLf)
            End If

            'gias2gias
            If Cod_RisUm_Origine <> 0 Then
                StbSQL.Append(" AND  Risorse_Umane.Cod_RisUm_Origine = " & Agro_SQL_SaveNum(Cod_RisUm_Origine.ToString) & "   " & vbCrLf)
            End If
            If Piva_SuperUser_Origine <> "" Then
                StbSQL.Append(" AND  Risorse_Umane.Piva_SuperUser_Origine = '" & Agro_SQL_SaveText(Piva_SuperUser_Origine) & "'" & vbCrLf)
            End If
            If Not Flag_AncheImportati Then
                StbSQL.Append(" AND  Risorse_Umane.Cod_RisUm_Origine = 0 " & vbCrLf)
            End If
            'fine gias2gias

            If Cliente <> 0 Then
                StbSQL.Append(" AND Rapporti_Contabili.Cliente = 1 " & vbCrLf)
            End If

            If Fornitore <> 0 Then
                StbSQL.Append(" AND Rapporti_Contabili.Fornitore = 1 " & vbCrLf)
            End If

            If Dipendente <> 0 Then
                StbSQL.Append(" AND Rapporti_Contabili.Dipendente = 1 " & vbCrLf)
            End If

            If Terzista <> 0 Then
                StbSQL.Append(" AND Rapporti_Contabili.Terzista = 1 " & vbCrLf)
            End If

            If Legale <> 0 Then
                StbSQL.Append(" AND Rapporti_Contabili.Legale = 1 " & vbCrLf)
            End If

            If Agente <> 0 Then
                StbSQL.Append(" AND Rapporti_Contabili.Agente = 1 " & vbCrLf)
            End If

            If Consulente <> 0 Then
                StbSQL.Append(" AND Rapporti_Contabili.Consulente = 1 " & vbCrLf)
            End If

            If xFiltroAggiuntivo <> "" Then
                StbSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StbSQL.Append(" AND   Contatti.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StbSQL.Append(" AND   Contatti.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StbSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StbSQL.Append(" ORDER BY Contatti.Rag_Soc ASC  " & vbCrLf)
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StbSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function

    'default:
    'ID_CF = -99 (costante ID_CF_NOFILTRO)
    Public Function Leggi_Distinct_Contatti2(ByVal Piva As String,
                                                ByVal Cod_Contatto As String,
                                                ByVal Cod_RisUm As Integer,
                                                ByVal FlagPubblico As Boolean,
                                                ByVal ID_CF As Integer,
                                                ByVal Cod_RisUm_Origine As Integer,
                                                ByVal Piva_SuperUser_Origine As String,
                                                ByVal Progressivo As String,
                                                ByVal xFiltroAggiuntivo As String,
                                                    ByVal xOrderBy As String,
                                                    ByRef objParametri As AgronicaCoreParametri
                                                    ) As DataTable


        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Contatti_R.Contatti_Contatto_Leggi()"


        Dim StbSQL As New StringBuilder
        Dim MessaggioErrore As String = ""
        Dim DT As New DataTable
        Dim i As Integer = 0

        Try

            '----------------------------------------------------
            '--- Preparo la Query SQL ---------------------------
            '----------------------------------------------------
            StbSQL.Length = 0

            StbSQL.Append(" SELECT DISTINCT Contatti.Cod_Contatto, Contatti.Rag_Soc, " & vbCrLf)
            StbSQL.Append(" Contatti.Nome, Contatti.Cognome, Contatti.Id_cf " & vbCrLf)
            StbSQL.Append(" FROM    Contatti  " & vbCrLf)
            StbSQL.Append(" INNER JOIN Risorse_Umane ON Contatti.Cod_Contatto = Risorse_Umane.Cod_Contatto AND Contatti.Piva = Risorse_Umane.Piva " & vbCrLf)
            StbSQL.Append(" INNER JOIN  Rapporti_Contabili ON Risorse_Umane.Cod_Rapporto = Rapporti_Contabili.Cod_Rapporto " & vbCrLf)
            StbSQL.Append(" INNER JOIN UtentiXImprese ON Rapporti_Contabili.Piva = UtentiXImprese.[USER] AND Risorse_Umane.Piva = UtentiXImprese.PIVA " & vbCrLf)

            'JOIN IMPRESE 
            StbSQL.Append(" INNER JOIN Imprese ON Imprese.Piva = Contatti.Piva " & vbCrLf)

            If Cod_Contatto <> "" Then
                StbSQL.Append(" AND    (Contatti.Cod_Contatto = '" & Agro_SQL_SaveText(Cod_Contatto) & "')   " & vbCrLf)
            End If

            ' VISIBILITA
            If Not FlagPubblico Then

                If Piva <> "" Then

                    StbSQL.Append(" AND    ( ")

                    'visibilità centro
                    'leggo se ci sono filtri sui centri
                    Dim objUtentiVisibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
                    Dim FiltroCentri As String = ""
                    Dim DtCentriVisibili As DataTable
                    DtCentriVisibili = objUtentiVisibilita.Leggi(enum_TipoEntita.Centro, " Piva='" & Agro_SQL_SaveText(Piva) & "'", "", objParametri)
                    If DtCentriVisibili IsNot Nothing AndAlso DtCentriVisibili.Rows.Count > 0 Then
                        For i = 0 To DtCentriVisibili.Rows.Count - 1
                            FiltroCentri &= " (Contatti.Piva = '" & DtCentriVisibili.Rows(i).Item("piva") & "' AND Contatti.Sa_Cod = " & DtCentriVisibili.Rows(i).Item("sa_cod") & ") OR "
                        Next
                        If FiltroCentri <> "" Then
                            StbSQL.Append(" (" & Left(FiltroCentri, FiltroCentri.Length - 3) & " OR (Contatti.Piva = '" & Agro_SQL_SaveText(Piva) & "' AND Contatti.Sa_Cod = 0) ) ")
                        End If
                    End If

                    'aziendali
                    If FiltroCentri = "" Then
                        StbSQL.Append("    (Contatti.Piva = '" & Agro_SQL_SaveText(Piva) & "' AND Contatti.Sa_Cod <> -1 )  ")
                    End If

                    StbSQL.Append("        ) ")

                End If
            Else
                'pubblici + azienda
                If Piva <> "" Then

                    'pubbliche
                    StbSQL.Append(" AND    ( (Contatti.Sa_Cod = -1) ")

                    'visibilità centro
                    'leggo se ci sono filtri sui centri
                    Dim objUtentiVisibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
                    Dim FiltroCentri As String = ""
                    Dim DtCentriVisibili As DataTable
                    DtCentriVisibili = objUtentiVisibilita.Leggi(enum_TipoEntita.Centro, " Piva='" & Agro_SQL_SaveText(Piva) & "'", "", objParametri)
                    If DtCentriVisibili IsNot Nothing AndAlso DtCentriVisibili.Rows.Count > 0 Then
                        For i = 0 To DtCentriVisibili.Rows.Count - 1
                            FiltroCentri &= " (Contatti.Piva = '" & DtCentriVisibili.Rows(i).Item("piva") & "' AND Contatti.Sa_Cod = " & DtCentriVisibili.Rows(i).Item("sa_cod") & ") OR "
                        Next
                        If FiltroCentri <> "" Then
                            StbSQL.Append(" OR (" & Left(FiltroCentri, FiltroCentri.Length - 3) & " OR (Contatti.Piva = '" & Agro_SQL_SaveText(Piva) & "' AND Contatti.Sa_Cod = 0) ) ")
                        End If
                    End If

                    'aziendali
                    If FiltroCentri = "" Then
                        StbSQL.Append("          OR  (Contatti.Piva = '" & Agro_SQL_SaveText(Piva) & "')  ")
                    End If

                    StbSQL.Append("        ) ")

                Else
                    'tutti pubblici
                    StbSQL.Append(" AND    (Contatti.Sa_Cod = -1)  " & vbCrLf)
                End If

            End If

            If ID_CF <> ID_CF_NOFILTRO Then
                StbSQL.Append(" AND     (Contatti.ID_CF = " & Agro_SQL_SaveNum(ID_CF.ToString) & ")   " & vbCrLf)
            End If

            If Cod_RisUm <> 0 Then
                StbSQL.Append(" AND     (Cod_RisUm = " & Agro_SQL_SaveNum(Cod_RisUm.ToString) & ")   " & vbCrLf)
            End If

            If Progressivo <> "" Then
                StbSQL.Append(" AND  Risorse_Umane.Settore_Des = '" & Agro_SQL_SaveText(Progressivo) & "'   " & vbCrLf)
            End If

            StbSQL.Append(" AND (Rapporti_Contabili.Cliente = 1 OR Rapporti_Contabili.Fornitore = 1 " & vbCrLf)
            StbSQL.Append(" OR Rapporti_Contabili.Terzista = 1 OR Rapporti_Contabili.Agente = 1 " & vbCrLf)
            StbSQL.Append(" OR Rapporti_Contabili.Consulente = 1) " & vbCrLf)

            If xFiltroAggiuntivo <> "" Then
                StbSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StbSQL.Append(" AND   Contatti.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StbSQL.Append(" AND   Contatti.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StbSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StbSQL.Append(" ORDER BY Contatti.Rag_Soc ASC  " & vbCrLf)
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StbSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT


    End Function

    '########################### descrizione da codice: filippo ####################################
    Public Function RagSoc_Nome_Cognome_RapportoDes_from_Cod_Risum(ByVal cod_risum As Integer,
                                                            ByVal agroParam As AgronicaCoreParametri) As DataTable

        Dim nomeRout As String = "Contatti_R.RagSoc_Nome_Cognome_from_Cod_Contatto"

        Dim q As String = "SELECT rag_soc, nome, cognome, rapporto_des  FROM Contatti (NOLOCK)" &
                        "INNER JOIN Risorse_Umane (NOLOCK) ON Contatti.Cod_Contatto = Risorse_Umane.Cod_Contatto AND Contatti.Piva = Risorse_Umane.Piva " &
                        "INNER JOIN  Rapporti_Contabili (NOLOCK) ON Risorse_Umane.Cod_Rapporto = Rapporti_Contabili.Cod_Rapporto " &
                        "WHERE Risorse_Umane.Cod_risum = " & Agro_SQL_SaveNum(cod_risum) & " " 'AND Contatti.PIVA='" & Agro_SQL_SaveText(piva) & "'"

        Select Case agroParam.FlagVisibilita
            Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                q += " AND   Contatti.Inviato >=0 "
            Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                q += " AND   Contatti.Inviato =-1 "
            Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                '...................................
            Case Else
                Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
        End Select




        Try

            Return MyBase.EseguiQuery_Lettura(agroParam, q, nomeRout)

        Catch ex As Exception


            MyBase.Scrivi_LOG(agroParam, nomeRout, ex.Message)
            Return Nothing

        End Try


    End Function

    Public Function Dati_Fatturazione_from_Cod_Risum(ByVal cod_risum As Integer,
                                                     ByVal agroParam As AgronicaCoreParametri) As DataTable

        Dim nomeRout As String = "Contatti_R.Dati_Fatturazione_from_Cod_Risum"

        Dim q As String = "SELECT rag_soc, nome, cognome, rapporto_des, modalita_fatturazione, ID_CF, Contatti.Cod_Contatto  FROM Contatti " &
                        "INNER JOIN Risorse_Umane ON Contatti.Cod_Contatto = Risorse_Umane.Cod_Contatto AND Contatti.Piva = Risorse_Umane.Piva " &
                        "INNER JOIN  Rapporti_Contabili ON Risorse_Umane.Cod_Rapporto = Rapporti_Contabili.Cod_Rapporto " &
                        "WHERE Risorse_Umane.Cod_risum = " & Agro_SQL_SaveNum(cod_risum) & " " 'AND Contatti.PIVA='" & Agro_SQL_SaveText(piva) & "'"

        Select Case agroParam.FlagVisibilita
            Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                q += " AND   Contatti.Inviato >=0 "
            Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                q += " AND   Contatti.Inviato =-1 "
            Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                '...................................
            Case Else
                Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
        End Select




        Try

            Return MyBase.EseguiQuery_Lettura(agroParam, q, nomeRout)

        Catch ex As Exception


            MyBase.Scrivi_LOG(agroParam, nomeRout, ex.Message)
            Return Nothing

        End Try


    End Function


    '########################### descrizione da codice: filippo ####################################
    Public Function RagSoc_Nome_Cognome_Cod_Contatto(ByVal cod_Contatto As String,
                                                     ByVal agroParam As AgronicaCoreParametri) As DataTable

        Dim nomeRout As String = "Contatti_R.RagSoc_Nome_Cognome_from_Cod_Contatto"

        Dim q As String = "SELECT rag_soc, nome, cognome FROM Contatti " &
                        "WHERE Cod_Contatto = '" & Agro_SQL_SaveText(cod_Contatto) & "' " 'AND Contatti.PIVA='" & Agro_SQL_SaveText(piva) & "'"

        Select Case agroParam.FlagVisibilita
            Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                q += " AND   Contatti.Inviato >=0 "
            Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                q += " AND   Contatti.Inviato =-1 "
            Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                '...................................
            Case Else
                Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
        End Select




        Try

            Return MyBase.EseguiQuery_Lettura(agroParam, q, nomeRout)

        Catch ex As Exception


            MyBase.Scrivi_LOG(agroParam, nomeRout, ex.Message)
            Return Nothing

        End Try


    End Function



    'I costi sono in LEFT OUTER JOIN E SONO FACOLTATIVI (SI DECIDE IN BASE AL FLAG)
    Public Function leggi_x_anagrafica(ByVal Tipo_1_normale_2_Full As String,
                                       ByVal Piva As String,
                                       ByVal xFiltroAggiuntivo As String,
                                       ByVal xOrderBy As String,
                                       ByRef objParametri As AgronicaCoreParametri
                                       ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Contatti_R.Contatti_Contatto_Leggi()"

        Dim StbSQL As New StringBuilder
        Dim MessaggioErrore As String = ""
        Dim DT As New DataTable
        Dim i As Integer = 0

        Try

            StbSQL.Length = 0

            StbSQL.Append(" SELECT  Contatti.Piva + '_' + cast(contatti.sa_cod as nvarchar(50)) + '_' + contatti.cod_contatto  as chiave ,     ")

            StbSQL.Append(" Imprese.Rag_Soc AS Impresa,  CASE   Contatti.Rag_Soc WHEN '' then  Contatti.Cognome + ' ' +  Contatti.Nome ELSE Contatti.Rag_Soc END as 'Nome del Contatto',  " & vbCrLf)
            StbSQL.Append(" Risorse_Umane.Validita_Inizio, Risorse_Umane.Validita_Fine,  Rapporti_Contabili.Rapporto_Des " & vbCrLf)



            If Tipo_1_normale_2_Full = 2 Then
                StbSQL.Append(", ContattiXIndirizzi.Cod_Indirizzo, ContattiXIndirizzi.Tipo_Indirizzo, Indirizzi.ind_des, Indirizzi.frz_des, ISNULL(Indirizzi.CAP, '') AS CAP, ISNULL(ISTAT.LOCALITA, '') AS com_des, ISNULL(ISTAT.COMUNI_PROV, '') AS pro_cod, Indirizzi.stato, Indirizzi.note, Indirizzi.pro_cod_istat, Indirizzi.com_cod_istat " & vbCrLf)
            End If

            If Tipo_1_normale_2_Full = 2 Then
                StbSQL.Append(", ISNULL(ContattiXRubrica.Cod_Rubrica,0) AS Cod_Rubrica, ISNULL(Rubrica.numero,'') AS Numero, ISNULL(Rubrica.descr,'') AS descr" & vbCrLf)
            End If

            If Tipo_1_normale_2_Full = 2 Then
                StbSQL.Append(" , ISNULL(Prodotti_Costi.Elem_Cod, 1) AS Elem_Cod, ISNULL(Prodotti_Costi.Mezzo, -1) AS Mezzo,  ISNULL(Prodotti_Costi.Prezzo_Unitario, 0) AS Prezzo_Unitario,  ")
                StbSQL.Append(" ISNULL(Prodotti_Costi.Validita_Inizio, '01/01/1900') AS Costo_Inizio, ISNULL(Prodotti_Costi.Validita_Fine, '31/12/2100') AS Costo_Fine  ")
            End If

            StbSQL.Append(" FROM    Contatti  " & vbCrLf)
            StbSQL.Append(" INNER JOIN Risorse_Umane ON Contatti.Cod_Contatto = Risorse_Umane.Cod_Contatto AND Contatti.Piva = Risorse_Umane.Piva " & vbCrLf)
            StbSQL.Append(" INNER JOIN  Rapporti_Contabili ON Risorse_Umane.Cod_Rapporto = Rapporti_Contabili.Cod_Rapporto " & vbCrLf)
            StbSQL.Append(" INNER JOIN UtentiXImprese ON Rapporti_Contabili.Piva = UtentiXImprese.[USER] AND Risorse_Umane.Piva = UtentiXImprese.PIVA " & vbCrLf)

            'JOIN IMPRESE 
            StbSQL.Append(" INNER JOIN Imprese ON Imprese.Piva = Contatti.Piva " & vbCrLf)

            If Tipo_1_normale_2_Full = 2 Then
                StbSQL.Append("  INNER JOIN  ContattiXIndirizzi ON Contatti.Piva = ContattiXIndirizzi.Piva AND Contatti.Cod_Contatto = ContattiXIndirizzi.Cod_Contatto " & vbCrLf)
                StbSQL.Append("  INNER JOIN Indirizzi ON ContattiXIndirizzi.Cod_Indirizzo = Indirizzi.cod_indirizzo " & vbCrLf)
                StbSQL.Append("  LEFT OUTER JOIN ISTAT ON Indirizzi.pro_cod_istat = ISTAT.PROV AND Indirizzi.com_cod_istat = ISTAT.COM " & vbCrLf)
            End If

            If Tipo_1_normale_2_Full = 2 Then
                StbSQL.Append(" LEFT OUTER JOIN ContattiXRubrica ON ContattiXRubrica.Piva = Contatti.Piva AND ContattiXRubrica.Cod_Contatto = Contatti.Cod_Contatto " & vbCrLf)
                StbSQL.Append(" LEFT OUTER JOIN Rubrica ON Rubrica.cod_rubrica = ContattiXRubrica.Cod_Rubrica" & vbCrLf)
            End If

            If Tipo_1_normale_2_Full = 2 Then
                StbSQL.Append(" LEFT OUTER JOIN Prodotti_Costi ON Risorse_Umane.Cod_RisUm= Prodotti_Costi.Mat_Cod And Prodotti_Costi.Id_Budget = 0 ")
            End If

            StbSQL.Append(" ")


            StbSQL.Append(" WHERE   (Rapporti_Contabili.Piva = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "')   " & vbCrLf)

            StbSQL.Append(" AND ( Risorse_Umane.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ) " & vbCrLf)
            StbSQL.Append(" AND ( Risorse_Umane.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ) " & vbCrLf)


            'If FlagPubblico = False Then
            '    If Piva <> "" Then
            '        StbSQL.Append(" AND    (Contatti.Piva = '" & Agro_SQL_SaveText(Piva) & "')   " & vbCrLf)
            '    End If
            '    If Cod_Contatto <> "" Then
            '        StbSQL.Append(" AND    (Contatti.Cod_Contatto = '" & Agro_SQL_SaveText(Cod_Contatto) & "')   " & vbCrLf)
            '    End If
            'Else
            '    If Piva <> "" Then
            '        StbSQL.Append(" AND    (Contatti.Piva = '" & Agro_SQL_SaveText(Piva) & "' OR Contatti.Sa_Cod = -1 )  " & vbCrLf)
            '    End If
            'End If


            'azienda
            If Piva <> "" Then

                'StbSQL.Append(" AND    (Contatti.Piva = '" & Agro_SQL_SaveText(Piva) & "' OR Contatti.Sa_Cod = -1 )  " & vbCrLf)

                'pubbliche
                'StbSQL.Append(" AND    ( (Contatti.Sa_Cod = -1) ")

                'visibilità centro
                'leggo se ci sono filtri sui centri
                Dim objUtentiVisibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
                Dim FiltroCentri As String = ""
                Dim DtCentriVisibili As DataTable
                DtCentriVisibili = objUtentiVisibilita.Leggi(enum_TipoEntita.Centro, " Piva='" & Agro_SQL_SaveText(Piva) & "'", "", objParametri)
                If DtCentriVisibili IsNot Nothing AndAlso DtCentriVisibili.Rows.Count > 0 Then
                    For i = 0 To DtCentriVisibili.Rows.Count - 1
                        FiltroCentri &= " (Contatti.Piva = '" & DtCentriVisibili.Rows(i).Item("piva") & "' AND Contatti.Sa_Cod = " & DtCentriVisibili.Rows(i).Item("sa_cod") & ") OR "
                    Next
                    If FiltroCentri <> "" Then
                        StbSQL.Append(" OR (" & Left(FiltroCentri, FiltroCentri.Length - 3) & " OR (Contatti.Piva = '" & Agro_SQL_SaveText(Piva) & "' AND Contatti.Sa_Cod = 0) ) ")
                    End If
                End If

                'aziendali
                If FiltroCentri = "" Then
                    StbSQL.Append("          OR  (Contatti.Piva = '" & Agro_SQL_SaveText(Piva) & "')  ")
                End If

                'StbSQL.Append("        ) ")

            Else
                'tutti pubblici
                StbSQL.Append(" AND    (Contatti.Sa_Cod = -1)  " & vbCrLf)
            End If










            'If ID_CF <> -99 Then
            '    StbSQL.Append(" AND     (Contatti.ID_CF = " & Agro_SQL_SaveNum(ID_CF.ToString) & ")   " & vbCrLf)
            'End If

            'If Cod_RisUm <> 0 Then
            '    StbSQL.Append(" AND     (Cod_RisUm = " & Agro_SQL_SaveNum(Cod_RisUm.ToString) & ")   " & vbCrLf)
            'End If

            'If Cod_Rapporto <> 0 Then
            '    StbSQL.Append(" AND     (Rapporti_Contabili.Cod_Rapporto = " & Agro_SQL_SaveNum(Cod_Rapporto.ToString) & ")   " & vbCrLf)
            'End If

            'If Tipo_1_normale_2_Full = 2 Then
            '    If TipoIndirizzo <> 0 Then
            '        StbSQL.Append(" AND     (ContattiXIndirizzi.Tipo_Indirizzo = " & Agro_SQL_SaveNum(TipoIndirizzo.ToString) & ")   " & vbCrLf)
            '    End If
            '    If CodIndirizzo <> 0 Then
            '        StbSQL.Append(" AND     (ContattiXIndirizzi.Cod_Indirizzo = " & Agro_SQL_SaveNum(CodIndirizzo.ToString) & ")   " & vbCrLf)
            '    End If
            'End If

            'If Tipo_1_normale_2_Full = 2 Then
            '    If CodRubrica <> 0 Then
            '        StbSQL.Append(" AND     (Rubrica.Cod_Rubrica = " & Agro_SQL_SaveNum(CodRubrica.ToString) & ")   " & vbCrLf)
            '    End If
            'End If

            'gias2gias
            'If Cod_RisUm_Origine <> 0 Then
            '    StbSQL.Append(" AND  Risorse_Umane.Cod_RisUm_Origine = " & Agro_SQL_SaveNum(Cod_RisUm_Origine.ToString) & "   " & vbCrLf)
            'End If
            'If Piva_SuperUser_Origine <> "" Then
            '    StbSQL.Append(" AND  Risorse_Umane.Piva_SuperUser_Origine = '" & Agro_SQL_SaveText(Piva_SuperUser_Origine) & "'" & vbCrLf)
            'End If
            'If Flag_AncheImportati = False Then
            '    StbSQL.Append(" AND  Risorse_Umane.Cod_RisUm_Origine = 0 " & vbCrLf)
            'End If
            'fine gias2gias

            'If Cliente <> 0 Then
            '    StbSQL.Append(" AND Rapporti_Contabili.Cliente = 1 " & vbCrLf)
            'End If

            'If Fornitore <> 0 Then
            '    StbSQL.Append(" AND Rapporti_Contabili.Fornitore = 1 " & vbCrLf)
            'End If

            'If Dipendente <> 0 Then
            '    StbSQL.Append(" AND Rapporti_Contabili.Dipendente = 1 " & vbCrLf)
            'End If

            'If Terzista <> 0 Then
            '    StbSQL.Append(" AND Rapporti_Contabili.Terzista = 1 " & vbCrLf)
            'End If

            'If Legale <> 0 Then
            '    StbSQL.Append(" AND Rapporti_Contabili.Legale = 1 " & vbCrLf)
            'End If


            If xFiltroAggiuntivo <> "" Then
                StbSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StbSQL.Append(" AND   Contatti.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StbSQL.Append(" AND   Contatti.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StbSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StbSQL.Append(" ORDER BY Contatti.Rag_Soc ASC  " & vbCrLf)
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StbSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
        End Try

        Return DT

    End Function


    '@Paolo
    Public Function leggi_x_anagrafica2(ByVal Piva As String,
                                        ByVal modalita As String,
                                        ByVal xFiltroAggiuntivo As String,
                                        ByVal xOrderBy As String,
                                        ByRef objParametri As AgronicaCoreParametri
                                        ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Contatti_R.Contatti_Contatto_Leggi()"

        Dim StbSQL As New StringBuilder
        Dim MessaggioErrore As String = ""
        Dim DT As New DataTable
        Dim i As Integer = 0

        Try

            StbSQL.Length = 0

            StbSQL.Append(" SELECT  Contatti.Piva + '_' + cast(contatti.sa_cod as nvarchar(50)) + '_' + contatti.cod_contatto  as chiave ,     ")
            StbSQL.Append(" UPPER(Contatti.Cod_Contatto) As CF, ")
            StbSQL.Append(" Imprese.Rag_Soc AS Impresa,  CASE   Contatti.Rag_Soc WHEN '' then  Contatti.Cognome + ' ' +  Contatti.Nome ELSE Contatti.Rag_Soc END as 'Contatto_Des',  " & vbCrLf)
            StbSQL.Append(" Risorse_Umane.Validita_Inizio, Risorse_Umane.Attivita_Des, Risorse_Umane.Validita_Fine, Risorse_Umane.Settore_Des, Rapporti_Contabili.Rapporto_Des, " & vbCrLf)
            StbSQL.Append(" Rapporti_Contabili.Cod_Rapporto as Cod_Rapp,  Risorse_Umane.Cod_RisUm as Cod_Risum, " & vbCrLf)
            StbSQL.Append(" Contatti.Sa_Cod, " & vbCrLf)
            'StbSQL.Append(", ContattiXIndirizzi.Cod_Indirizzo, ContattiXIndirizzi.Tipo_Indirizzo, Indirizzi.ind_des, Indirizzi.frz_des, ISNULL(Indirizzi.CAP, '') AS CAP, ISNULL(ISTAT.LOCALITA, '') AS com_des, ISNULL(ISTAT.COMUNI_PROV, '') AS pro_cod, Indirizzi.stato, Indirizzi.note, Indirizzi.pro_cod_istat, Indirizzi.com_cod_istat " & vbCrLf)
            'StbSQL.Append(", ISNULL(ContattiXRubrica.Cod_Rubrica,0) AS Cod_Rubrica, ISNULL(Rubrica.numero,'') AS Numero, ISNULL(Rubrica.descr,'') AS descr" & vbCrLf)
            'StbSQL.Append(" , ISNULL(Prodotti_Costi.Elem_Cod, 1) AS Elem_Cod, ISNULL(Prodotti_Costi.Mezzo, -1) AS Mezzo,  ISNULL(Prodotti_Costi.Prezzo_Unitario, 0) AS Prezzo_Unitario,  ")
            'StbSQL.Append(" ISNULL(Prodotti_Costi.Validita_Inizio, '01/01/1900') AS Costo_Inizio, ISNULL(Prodotti_Costi.Validita_Fine, '31/12/2100') AS Costo_Fine  ")
            StbSQL.AppendLine(" ISNULL ((SELECT TOP 1 [User] FROM Utenti WHERE CODICE_FISCALE = Contatti.Username_Creazione), ' ') AS Utente_Creazione, ")
            StbSQL.AppendLine(" ISNULL ((SELECT TOP 1 [User] FROM Utenti WHERE CODICE_FISCALE = Contatti.Username_Modifica), ' ') AS Utente_Modifica, ")

            StbSQL.AppendLine(" Contatti.Data_Creazione, ")
            StbSQL.AppendLine(" Contatti.Data_Modifica ")

            StbSQL.Append(" FROM    Contatti  " & vbCrLf)
            StbSQL.Append(" INNER JOIN Risorse_Umane ON Contatti.Cod_Contatto = Risorse_Umane.Cod_Contatto AND Contatti.Piva = Risorse_Umane.Piva " & vbCrLf)
            StbSQL.Append(" INNER JOIN  Rapporti_Contabili ON Risorse_Umane.Cod_Rapporto = Rapporti_Contabili.Cod_Rapporto " & vbCrLf)
            StbSQL.Append(" INNER JOIN UtentiXImprese ON Rapporti_Contabili.Piva = UtentiXImprese.[USER] AND Risorse_Umane.Piva = UtentiXImprese.PIVA " & vbCrLf)

            'JOIN IMPRESE 
            StbSQL.Append(" INNER JOIN Imprese ON Imprese.Piva = Contatti.Piva " & vbCrLf)


            'StbSQL.Append("  INNER JOIN  ContattiXIndirizzi ON Contatti.Piva = ContattiXIndirizzi.Piva AND Contatti.Cod_Contatto = ContattiXIndirizzi.Cod_Contatto " & vbCrLf)
            'StbSQL.Append("  INNER JOIN Indirizzi ON ContattiXIndirizzi.Cod_Indirizzo = Indirizzi.cod_indirizzo " & vbCrLf)
            'StbSQL.Append("  LEFT OUTER JOIN ISTAT ON Indirizzi.pro_cod_istat = ISTAT.PROV AND Indirizzi.com_cod_istat = ISTAT.COM " & vbCrLf)
            'StbSQL.Append(" LEFT OUTER JOIN ContattiXRubrica ON ContattiXRubrica.Piva = Contatti.Piva AND ContattiXRubrica.Cod_Contatto = Contatti.Cod_Contatto " & vbCrLf)
            'StbSQL.Append(" LEFT OUTER JOIN Rubrica ON Rubrica.cod_rubrica = ContattiXRubrica.Cod_Rubrica" & vbCrLf)

            'StbSQL.Append(" LEFT OUTER JOIN Prodotti_Costi ON Risorse_Umane.Cod_RisUm= Prodotti_Costi.Mat_Cod  ")


            StbSQL.Append(" ")


            StbSQL.Append(" WHERE 1 = 1  " & vbCrLf)
            StbSQL.Append(" AND (Rapporti_Contabili.Piva = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "')   " & vbCrLf)
            StbSQL.Append(" AND ( Risorse_Umane.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ) " & vbCrLf)
            StbSQL.Append(" AND ( Risorse_Umane.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ) " & vbCrLf)


            'If FlagPubblico = False Then
            '    If Piva <> "" Then
            '        StbSQL.Append(" AND    (Contatti.Piva = '" & Agro_SQL_SaveText(Piva) & "')   " & vbCrLf)
            '    End If
            '    If Cod_Contatto <> "" Then
            '        StbSQL.Append(" AND    (Contatti.Cod_Contatto = '" & Agro_SQL_SaveText(Cod_Contatto) & "')   " & vbCrLf)
            '    End If
            'Else
            '    If Piva <> "" Then
            '        StbSQL.Append(" AND    (Contatti.Piva = '" & Agro_SQL_SaveText(Piva) & "' OR Contatti.Sa_Cod = -1 )  " & vbCrLf)
            '    End If
            'End If


            ''azienda
            'If Piva <> "" Then

            '    'StbSQL.Append(" AND    (Contatti.Piva = '" & Agro_SQL_SaveText(Piva) & "' OR Contatti.Sa_Cod = -1 )  " & vbCrLf)

            '    'pubbliche
            '    'StbSQL.Append(" AND    ( (Contatti.Sa_Cod = -1) ")

            '    'visibilità centro
            '    'leggo se ci sono filtri sui centri
            '    Dim objUtentiVisibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
            '    Dim FiltroCentri As String = ""
            '    Dim DtCentriVisibili As DataTable
            '    DtCentriVisibili = objUtentiVisibilita.Leggi(enum_TipoEntita.Centro, " Piva='" & Piva & "'", "", objParametri)
            '    If Not DtCentriVisibili Is Nothing AndAlso DtCentriVisibili.Rows.Count > 0 Then
            '        For i = 0 To DtCentriVisibili.Rows.Count - 1
            '            FiltroCentri &= " (Contatti.Piva = '" & DtCentriVisibili.Rows(i).Item("piva") & "' AND Contatti.Sa_Cod = " & DtCentriVisibili.Rows(i).Item("sa_cod") & ") OR "
            '        Next
            '        If FiltroCentri <> "" Then
            '            StbSQL.Append(" OR (" & Left(FiltroCentri, FiltroCentri.Length - 3) & " OR (Contatti.Piva = '" & Agro_SQL_SaveText(Piva) & "' AND Contatti.Sa_Cod = 0) ) ")
            '        End If
            '    End If

            '    'aziendali
            '    If FiltroCentri = "" Then
            '        StbSQL.Append("          OR  (Contatti.Piva = '" + Agro_SQL_SaveText(Piva) + "')  ")
            '    End If

            '    'StbSQL.Append("        ) ")

            'Else
            '    'tutti pubblici
            '    StbSQL.Append(" AND    (Contatti.Sa_Cod = -1)  " & vbCrLf)
            'End If


            If Piva <> "" Then


                Select Case modalita
                    Case "-1"
                        StbSQL.Append(" AND (Contatti.Sa_Cod = " & CInt(modalita) & ") ")
                    Case "0"
                        StbSQL.Append(" AND ((Contatti.Sa_Cod <> -1)           AND  (Contatti.Piva = '" & Agro_SQL_SaveText(Piva) & "')   ) ")
                        Dim objUtentiVisibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
                        Dim FiltroCentri As String = ""
                        Dim DtCentriVisibili As DataTable
                        DtCentriVisibili = objUtentiVisibilita.Leggi(enum_TipoEntita.Centro, " Piva='" & Agro_SQL_SaveText(Piva) & "'", "", objParametri)
                        If DtCentriVisibili IsNot Nothing AndAlso DtCentriVisibili.Rows.Count > 0 Then
                            FiltroCentri = "0,"
                            For i = 0 To DtCentriVisibili.Rows.Count - 1
                                FiltroCentri &= DtCentriVisibili.Rows(i).Item("sa_cod") & ","
                            Next
                            If FiltroCentri <> "" Then
                                StbSQL.AppendLine(" AND Contatti.sa_cod IN (" & Agro_SQL_Save_Clausola_IN(Left(FiltroCentri, FiltroCentri.Length - 1)) & ") ")
                            End If
                        End If
                    Case Else
                        StbSQL.Append(" AND ((Contatti.Sa_Cod = " & CInt(modalita) & ")           AND  (Contatti.Piva = '" & Agro_SQL_SaveText(Piva) & "')   ) ")
                End Select

            Else
                'tutti pubblici
                StbSQL.Append(" AND    (Contatti.Sa_Cod = -1)  " & vbCrLf)
            End If







            'If ID_CF <> -99 Then
            '    StbSQL.Append(" AND     (Contatti.ID_CF = " & Agro_SQL_SaveNum(ID_CF.ToString) & ")   " & vbCrLf)
            'End If

            'If Cod_RisUm <> 0 Then
            '    StbSQL.Append(" AND     (Cod_RisUm = " & Agro_SQL_SaveNum(Cod_RisUm.ToString) & ")   " & vbCrLf)
            'End If

            'If Cod_Rapporto <> 0 Then
            '    StbSQL.Append(" AND     (Rapporti_Contabili.Cod_Rapporto = " & Agro_SQL_SaveNum(Cod_Rapporto.ToString) & ")   " & vbCrLf)
            'End If

            'If Tipo_1_normale_2_Full = 2 Then
            '    If TipoIndirizzo <> 0 Then
            '        StbSQL.Append(" AND     (ContattiXIndirizzi.Tipo_Indirizzo = " & Agro_SQL_SaveNum(TipoIndirizzo.ToString) & ")   " & vbCrLf)
            '    End If
            '    If CodIndirizzo <> 0 Then
            '        StbSQL.Append(" AND     (ContattiXIndirizzi.Cod_Indirizzo = " & Agro_SQL_SaveNum(CodIndirizzo.ToString) & ")   " & vbCrLf)
            '    End If
            'End If

            'If Tipo_1_normale_2_Full = 2 Then
            '    If CodRubrica <> 0 Then
            '        StbSQL.Append(" AND     (Rubrica.Cod_Rubrica = " & Agro_SQL_SaveNum(CodRubrica.ToString) & ")   " & vbCrLf)
            '    End If
            'End If

            'gias2gias
            'If Cod_RisUm_Origine <> 0 Then
            '    StbSQL.Append(" AND  Risorse_Umane.Cod_RisUm_Origine = " & Agro_SQL_SaveNum(Cod_RisUm_Origine.ToString) & "   " & vbCrLf)
            'End If
            'If Piva_SuperUser_Origine <> "" Then
            '    StbSQL.Append(" AND  Risorse_Umane.Piva_SuperUser_Origine = '" & Agro_SQL_SaveText(Piva_SuperUser_Origine) & "'" & vbCrLf)
            'End If
            'If Flag_AncheImportati = False Then
            '    StbSQL.Append(" AND  Risorse_Umane.Cod_RisUm_Origine = 0 " & vbCrLf)
            'End If
            'fine gias2gias

            'If Cliente <> 0 Then
            '    StbSQL.Append(" AND Rapporti_Contabili.Cliente = 1 " & vbCrLf)
            'End If

            'If Fornitore <> 0 Then
            '    StbSQL.Append(" AND Rapporti_Contabili.Fornitore = 1 " & vbCrLf)
            'End If

            'If Dipendente <> 0 Then
            '    StbSQL.Append(" AND Rapporti_Contabili.Dipendente = 1 " & vbCrLf)
            'End If

            'If Terzista <> 0 Then
            '    StbSQL.Append(" AND Rapporti_Contabili.Terzista = 1 " & vbCrLf)
            'End If

            'If Legale <> 0 Then
            '    StbSQL.Append(" AND Rapporti_Contabili.Legale = 1 " & vbCrLf)
            'End If


            If xFiltroAggiuntivo <> "" Then
                StbSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StbSQL.Append(" AND   Contatti.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StbSQL.Append(" AND   Contatti.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StbSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StbSQL.Append(" ORDER BY Contatti.Rag_Soc ASC  " & vbCrLf)
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StbSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
        End Try

        Return DT

    End Function



    'I costi sono in LEFT OUTER JOIN E SONO FACOLTATIVI (SI DECIDE IN BASE AL FLAG)
    'RICORDARSI POI A LIVELLO DI CODICE DI FILTRARE ELEM_COD = 0
    Public Function Leggi(ByVal Piva As String,
                               ByVal Cod_Contatto As String,
                               ByVal Cod_RisUm As Integer,
                               ByVal Cod_Rapporto As Integer,
                               ByVal FlagPubblico As Boolean,
                               ByVal FlagIndirizzi As Boolean,
                               ByVal TipoIndirizzo As Integer,
                               ByVal CodIndirizzo As Integer,
                               ByVal FlagRubrica As Boolean,
                               ByVal CodRubrica As Integer,
                               ByVal ID_CF As Integer,
                               ByVal Cod_RisUm_Origine As Integer,
                               ByVal Piva_SuperUser_Origine As String,
                               ByVal Flag_AncheImportati As Boolean,
                               ByVal Cliente As Integer,
                               ByVal Fornitore As Integer,
                               ByVal Dipendente As Integer,
                               ByVal Terzista As Integer,
                               ByVal Legale As Integer,
                                   ByVal Validita_Inizio As Date,
                                   ByVal Validita_Fine As Date,
                                   ByVal Flag_Costi As Boolean,
                                   ByVal xFiltroAggiuntivo As String,
                                   ByVal xOrderBy As String,
                                   ByRef objParametri As AgronicaCoreParametri
                                   ) As DataTable


        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Contatti_R.Contatti_Contatto_Leggi()"


        Dim StbSQL As New StringBuilder
        Dim MessaggioErrore As String = ""
        Dim DT As New DataTable
        Dim i As Integer = 0

        Try

            '----------------------------------------------------
            '--- Preparo la Query SQL ---------------------------
            '----------------------------------------------------

            StbSQL.Length = 0

            StbSQL.Append(" SELECT  Imprese.Rag_Soc AS Impresa, Contatti.Piva, Contatti.Sa_Cod, Contatti.Cod_Contatto, Contatti.Codice_Fiscale, Contatti.Id_CF, Contatti.Rag_Soc, Contatti.Convenevoli, " & vbCrLf)
            StbSQL.Append(" Contatti.Nome, Contatti.Cognome, Contatti.Data_Nascita, Contatti.Sesso, Contatti.Cod_Contatto_Referente, " & vbCrLf)
            StbSQL.Append(" Risorse_Umane.Sa_Cod AS Sa_Cod_2, Risorse_Umane.Cod_RisUm, Risorse_Umane.Validita_Inizio, Risorse_Umane.Validita_Fine, " & vbCrLf)
            StbSQL.Append(" Risorse_Umane.Settore_Des, Risorse_Umane.Attivita_Des, Risorse_Umane.Corrispettivo_Mensile, Risorse_Umane.Corrispettivo_Orario, " & vbCrLf)
            StbSQL.Append(" Risorse_Umane.Occasionale, Risorse_Umane.Ore_Settimanali, Risorse_Umane.Giorni_Ferie, Risorse_Umane.Ferie_Godute, " & vbCrLf)
            StbSQL.Append(" Risorse_Umane.Giorni_Malattia, Risorse_Umane.Patentino, Risorse_Umane.Data_Rilascio_Patentino, Risorse_Umane.Data_Scadenza_Patentino, " & vbCrLf)
            StbSQL.Append(" Rapporti_Contabili.Piva AS Piva_SuperUser, Rapporti_Contabili.Sa_Cod AS Sa_Cod_3, Rapporti_Contabili.Cod_Rapporto, Rapporti_Contabili.Rapporto_Des, Rapporti_Contabili.Cliente,  " & vbCrLf)
            StbSQL.Append(" Rapporti_Contabili.Fornitore, Rapporti_Contabili.Dipendente, Rapporti_Contabili.Terzista, Rapporti_Contabili.Legale, " & vbCrLf)

            StbSQL.Append("  ISNULL ((SELECT TOP 1   val_cod  ")
            StbSQL.Append("          FROM        Contatti_Codici ")
            StbSQL.Append("          WHERE       Contatti.PIVA = Contatti_Codici.PIVA AND Contatti.Cod_Contatto = Contatti_Codici.Cod_Contatto AND Contatti_Codici.id_cod = " & Agro_SQL_SaveNum(CStr(enum_DatiAnagrafici_CodiciAnagrafe.Rappr_Legale)) & "), '0') AS Modifica ")


            If FlagIndirizzi Then
                StbSQL.Append(", ContattiXIndirizzi.Cod_Indirizzo, ContattiXIndirizzi.Tipo_Indirizzo, Indirizzi.ind_des, Indirizzi.frz_des, ISNULL(Indirizzi.CAP, '') AS CAP, ISNULL(ISTAT.LOCALITA, '') AS com_des, ISNULL(ISTAT.COMUNI_PROV, '') AS pro_cod, Indirizzi.stato, Indirizzi.note, Indirizzi.pro_cod_istat, Indirizzi.com_cod_istat " & vbCrLf)
            End If

            If FlagRubrica Then
                StbSQL.Append(", ISNULL(ContattiXRubrica.Cod_Rubrica,0) AS Cod_Rubrica, ISNULL(Rubrica.numero,'') AS Numero, ISNULL(Rubrica.descr,'') AS descr" & vbCrLf)
            End If

            If Flag_Costi Then
                StbSQL.Append(" , ISNULL(Prodotti_Costi.Elem_Cod, 1) AS Elem_Cod, ISNULL(Prodotti_Costi.Mezzo, -1) AS Mezzo,  ISNULL(Prodotti_Costi.Prezzo_Unitario, 0) AS Prezzo_Unitario,  ")
                StbSQL.Append(" ISNULL(Prodotti_Costi.Validita_Inizio, '01/01/1900') AS Costo_Inizio, ISNULL(Prodotti_Costi.Validita_Fine, '31/12/2100') AS Costo_Fine  ")
            End If

            StbSQL.Append(" FROM    Contatti  " & vbCrLf)
            StbSQL.Append(" INNER JOIN Risorse_Umane ON Contatti.Cod_Contatto = Risorse_Umane.Cod_Contatto AND Contatti.Piva = Risorse_Umane.Piva " & vbCrLf)
            StbSQL.Append(" INNER JOIN  Rapporti_Contabili ON Risorse_Umane.Cod_Rapporto = Rapporti_Contabili.Cod_Rapporto " & vbCrLf)
            StbSQL.Append(" INNER JOIN UtentiXImprese ON Rapporti_Contabili.Piva = UtentiXImprese.[USER] AND Risorse_Umane.Piva = UtentiXImprese.PIVA " & vbCrLf)

            'JOIN IMPRESE 
            StbSQL.Append(" INNER JOIN Imprese ON Imprese.Piva = Contatti.Piva " & vbCrLf)

            If FlagIndirizzi Then
                StbSQL.Append("  INNER JOIN  ContattiXIndirizzi ON Contatti.Piva = ContattiXIndirizzi.Piva AND Contatti.Cod_Contatto = ContattiXIndirizzi.Cod_Contatto " & vbCrLf)
                StbSQL.Append("  INNER JOIN Indirizzi ON ContattiXIndirizzi.Cod_Indirizzo = Indirizzi.cod_indirizzo " & vbCrLf)
                StbSQL.Append("  LEFT OUTER JOIN ISTAT ON Indirizzi.pro_cod_istat = ISTAT.PROV AND Indirizzi.com_cod_istat = ISTAT.COM " & vbCrLf)
            End If

            If FlagRubrica Then
                StbSQL.Append(" LEFT OUTER JOIN ContattiXRubrica ON ContattiXRubrica.Piva = Contatti.Piva AND ContattiXRubrica.Cod_Contatto = Contatti.Cod_Contatto " & vbCrLf)
                StbSQL.Append(" LEFT OUTER JOIN Rubrica ON Rubrica.cod_rubrica = ContattiXRubrica.Cod_Rubrica" & vbCrLf)
            End If

            If Flag_Costi Then
                StbSQL.Append(" LEFT OUTER JOIN Prodotti_Costi ON Risorse_Umane.Cod_RisUm= Prodotti_Costi.Mat_Cod And Prodotti_CostiId_Budget = 0 ")
            End If

            StbSQL.Append(" ")


            StbSQL.Append(" WHERE   (Rapporti_Contabili.Piva = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "')   " & vbCrLf)

            StbSQL.Append(" AND ( Risorse_Umane.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ) " & vbCrLf)
            StbSQL.Append(" AND ( Risorse_Umane.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ) " & vbCrLf)


            'If FlagPubblico = False Then
            '    If Piva <> "" Then
            '        StbSQL.Append(" AND    (Contatti.Piva = '" & Agro_SQL_SaveText(Piva) & "')   " & vbCrLf)
            '    End If
            '    If Cod_Contatto <> "" Then
            '        StbSQL.Append(" AND    (Contatti.Cod_Contatto = '" & Agro_SQL_SaveText(Cod_Contatto) & "')   " & vbCrLf)
            '    End If
            'Else
            '    If Piva <> "" Then
            '        StbSQL.Append(" AND    (Contatti.Piva = '" & Agro_SQL_SaveText(Piva) & "' OR Contatti.Sa_Cod = -1 )  " & vbCrLf)
            '    End If
            'End If

            If Cod_Contatto <> "" Then
                StbSQL.Append(" AND    (Contatti.Cod_Contatto = '" & Agro_SQL_SaveText(Cod_Contatto) & "')   " & vbCrLf)
            End If


            If Not FlagPubblico Then

                If Piva <> "" Then
                    'StbSQL.Append(" AND    (Contatti.Piva = '" & Agro_SQL_SaveText(Piva) & "')   " & vbCrLf)

                    'pubbliche
                    'StrSQL.Append(" AND    ( (Contatti.Sa_Cod = -1) ")

                    StbSQL.Append(" AND    ( ")

                    'visibilità centro
                    'leggo se ci sono filtri sui centri
                    Dim objUtentiVisibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
                    Dim FiltroCentri As String = ""
                    Dim DtCentriVisibili As DataTable
                    DtCentriVisibili = objUtentiVisibilita.Leggi(enum_TipoEntita.Centro, " Piva='" & Agro_SQL_SaveText(Piva) & "'", "", objParametri)
                    If DtCentriVisibili IsNot Nothing AndAlso DtCentriVisibili.Rows.Count > 0 Then
                        For i = 0 To DtCentriVisibili.Rows.Count - 1
                            FiltroCentri &= " (Contatti.Piva = '" & DtCentriVisibili.Rows(i).Item("piva") & "' AND Contatti.Sa_Cod = " & DtCentriVisibili.Rows(i).Item("sa_cod") & ") OR "
                        Next
                        If FiltroCentri <> "" Then
                            StbSQL.Append(" (" & Left(FiltroCentri, FiltroCentri.Length - 3) & " OR (Contatti.Piva = '" & Agro_SQL_SaveText(Piva) & "' AND Contatti.Sa_Cod = 0) ) ")
                        End If
                    End If

                    'aziendali
                    If FiltroCentri = "" Then
                        StbSQL.Append("    (Contatti.Piva = '" & Agro_SQL_SaveText(Piva) & "' AND Contatti.Sa_Cod <> -1 )  ")
                    End If

                    StbSQL.Append("        ) ")

                End If
            Else
                'pubblici + azienda
                If Piva <> "" Then

                    'StbSQL.Append(" AND    (Contatti.Piva = '" & Agro_SQL_SaveText(Piva) & "' OR Contatti.Sa_Cod = -1 )  " & vbCrLf)

                    'pubbliche
                    StbSQL.Append(" AND    ( (Contatti.Sa_Cod = -1) ")

                    'visibilità centro
                    'leggo se ci sono filtri sui centri
                    Dim objUtentiVisibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
                    Dim FiltroCentri As String = ""
                    Dim DtCentriVisibili As DataTable
                    DtCentriVisibili = objUtentiVisibilita.Leggi(enum_TipoEntita.Centro, " Piva='" & Agro_SQL_SaveText(Piva) & "'", "", objParametri)
                    If DtCentriVisibili IsNot Nothing AndAlso DtCentriVisibili.Rows.Count > 0 Then
                        For i = 0 To DtCentriVisibili.Rows.Count - 1
                            FiltroCentri &= " (Contatti.Piva = '" & DtCentriVisibili.Rows(i).Item("piva") & "' AND Contatti.Sa_Cod = " & DtCentriVisibili.Rows(i).Item("sa_cod") & ") OR "
                        Next
                        If FiltroCentri <> "" Then
                            StbSQL.Append(" OR (" & Left(FiltroCentri, FiltroCentri.Length - 3) & " OR (Contatti.Piva = '" & Agro_SQL_SaveText(Piva) & "' AND Contatti.Sa_Cod = 0) ) ")
                        End If
                    End If

                    'aziendali
                    If FiltroCentri = "" Then
                        StbSQL.Append("          OR  (Contatti.Piva = '" & Agro_SQL_SaveText(Piva) & "')  ")
                    End If

                    StbSQL.Append("        ) ")

                Else
                    'tutti pubblici
                    StbSQL.Append(" AND    (Contatti.Sa_Cod = -1)  " & vbCrLf)
                End If

            End If








            If ID_CF <> -99 Then
                StbSQL.Append(" AND     (Contatti.ID_CF = " & Agro_SQL_SaveNum(ID_CF.ToString) & ")   " & vbCrLf)
            End If

            If Cod_RisUm <> 0 Then
                StbSQL.Append(" AND     (Cod_RisUm = " & Agro_SQL_SaveNum(Cod_RisUm.ToString) & ")   " & vbCrLf)
            End If

            If Cod_Rapporto <> 0 Then
                StbSQL.Append(" AND     (Rapporti_Contabili.Cod_Rapporto = " & Agro_SQL_SaveNum(Cod_Rapporto.ToString) & ")   " & vbCrLf)
            End If

            If FlagIndirizzi Then
                If TipoIndirizzo <> 0 Then
                    StbSQL.Append(" AND     (ContattiXIndirizzi.Tipo_Indirizzo = " & Agro_SQL_SaveNum(TipoIndirizzo.ToString) & ")   " & vbCrLf)
                End If
                If CodIndirizzo <> 0 Then
                    StbSQL.Append(" AND     (ContattiXIndirizzi.Cod_Indirizzo = " & Agro_SQL_SaveNum(CodIndirizzo.ToString) & ")   " & vbCrLf)
                End If
            End If

            If FlagRubrica Then
                If CodRubrica <> 0 Then
                    StbSQL.Append(" AND     (Rubrica.Cod_Rubrica = " & Agro_SQL_SaveNum(CodRubrica.ToString) & ")   " & vbCrLf)
                End If
            End If

            'gias2gias
            If Cod_RisUm_Origine <> 0 Then
                StbSQL.Append(" AND  Risorse_Umane.Cod_RisUm_Origine = " & Agro_SQL_SaveNum(Cod_RisUm_Origine.ToString) & "   " & vbCrLf)
            End If
            If Piva_SuperUser_Origine <> "" Then
                StbSQL.Append(" AND  Risorse_Umane.Piva_SuperUser_Origine = '" & Agro_SQL_SaveText(Piva_SuperUser_Origine) & "'" & vbCrLf)
            End If
            If Not Flag_AncheImportati Then
                StbSQL.Append(" AND  Risorse_Umane.Cod_RisUm_Origine = 0 " & vbCrLf)
            End If
            'fine gias2gias

            If Cliente <> 0 Then
                StbSQL.Append(" AND Rapporti_Contabili.Cliente = 1 " & vbCrLf)
            End If

            If Fornitore <> 0 Then
                StbSQL.Append(" AND Rapporti_Contabili.Fornitore = 1 " & vbCrLf)
            End If

            If Dipendente <> 0 Then
                StbSQL.Append(" AND Rapporti_Contabili.Dipendente = 1 " & vbCrLf)
            End If

            If Terzista <> 0 Then
                StbSQL.Append(" AND Rapporti_Contabili.Terzista = 1 " & vbCrLf)
            End If

            If Legale <> 0 Then
                StbSQL.Append(" AND Rapporti_Contabili.Legale = 1 " & vbCrLf)
            End If


            If xFiltroAggiuntivo <> "" Then
                StbSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StbSQL.Append(" AND   Contatti.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StbSQL.Append(" AND   Contatti.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StbSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StbSQL.Append(" ORDER BY Contatti.Rag_Soc ASC  " & vbCrLf)
            End If

            '----------------------------------------------------
            '--- Recupero il datatable --------------------------
            '----------------------------------------------------

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StbSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing

        End Try

        Return DT


    End Function

    'filtro cod_contatto non solo per 
    Public Function Leggi_2(ByVal Piva As String,
                           ByVal Cod_Contatto As String,
                           ByVal Cod_RisUm As Integer,
                           ByVal Cod_Rapporto As Integer,
                           ByVal FlagPubblico As Boolean,
                           ByVal FlagIndirizzi As Boolean,
                           ByVal TipoIndirizzo As Integer,
                           ByVal CodIndirizzo As Integer,
                           ByVal FlagRubrica As Boolean,
                           ByVal CodRubrica As Integer,
                           ByVal ID_CF As Integer,
                           ByVal Cod_RisUm_Origine As Integer,
                           ByVal Piva_SuperUser_Origine As String,
                           ByVal Flag_AncheImportati As Boolean,
                           ByVal Cliente As Integer,
                           ByVal Fornitore As Integer,
                           ByVal Dipendente As Integer,
                           ByVal Terzista As Integer,
                           ByVal Legale As Integer,
                               ByVal Validita_Inizio As Date,
                               ByVal Validita_Fine As Date,
                               ByVal Flag_Costi As Boolean,
                               ByVal xFiltroAggiuntivo As String,
                               ByVal xOrderBy As String,
                               ByRef objParametri As AgronicaCoreParametri
                               ) As DataTable


        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Contatti_R.Leggi_2()"


        Dim StbSQL As New StringBuilder
        Dim MessaggioErrore As String = ""
        Dim DT As New DataTable
        Dim i As Integer = 0

        Try

            '----------------------------------------------------
            '--- Preparo la Query SQL ---------------------------
            '----------------------------------------------------

            StbSQL.Length = 0

            StbSQL.Append(" SELECT  Imprese.Rag_Soc AS Impresa, Contatti.Piva, Contatti.Sa_Cod, Contatti.Cod_Contatto, Contatti.Codice_Fiscale, Contatti.Id_CF, Contatti.Rag_Soc, Contatti.Convenevoli, " & vbCrLf)
            StbSQL.Append(" Contatti.Nome, Contatti.Cognome, Contatti.Data_Nascita, Contatti.Sesso, Contatti.Cod_Contatto_Referente, " & vbCrLf)
            StbSQL.Append(" Risorse_Umane.Sa_Cod AS Sa_Cod_2, Risorse_Umane.Cod_RisUm, Risorse_Umane.Validita_Inizio, Risorse_Umane.Validita_Fine, " & vbCrLf)
            StbSQL.Append(" Risorse_Umane.Settore_Des, Risorse_Umane.Attivita_Des, Risorse_Umane.Corrispettivo_Mensile, Risorse_Umane.Corrispettivo_Orario, " & vbCrLf)
            StbSQL.Append(" Risorse_Umane.Occasionale, Risorse_Umane.Ore_Settimanali, Risorse_Umane.Giorni_Ferie, Risorse_Umane.Ferie_Godute, " & vbCrLf)
            StbSQL.Append(" Risorse_Umane.Giorni_Malattia, " & vbCrLf)
            StbSQL.Append(" Rapporti_Contabili.Piva AS Piva_SuperUser, Rapporti_Contabili.Sa_Cod AS Sa_Cod_3, Rapporti_Contabili.Cod_Rapporto, Rapporti_Contabili.Rapporto_Des, Rapporti_Contabili.Cliente,  " & vbCrLf)
            StbSQL.Append(" Rapporti_Contabili.Fornitore, Rapporti_Contabili.Dipendente, Rapporti_Contabili.Terzista, Rapporti_Contabili.Legale, " & vbCrLf)

            'StbSQL.Append(" Allegati_Documenti.Allegati_Documenti_Numero as Patentino, Allegati_Documenti.Validazione_Data as Data_Rilascio_Patentino, Alert_Elenco.Data_Scadenza as Data_Scadenza_Patentino, Allegati_Documenti.Allegati_Documenti_Ente_Des as Ente_di_rilascio, " & vbCrLf)

            StbSQL.Append("  ISNULL ((SELECT TOP 1   val_cod  ")
            StbSQL.Append("          FROM        Contatti_Codici ")
            StbSQL.Append("          WHERE       Contatti.PIVA = Contatti_Codici.PIVA AND Contatti.Cod_Contatto = Contatti_Codici.Cod_Contatto AND Contatti_Codici.id_cod = " & Agro_SQL_SaveNum(CStr(enum_DatiAnagrafici_CodiciAnagrafe.Rappr_Legale)) & "), '0') AS Modifica ")


            If FlagIndirizzi Then
                StbSQL.Append(", ContattiXIndirizzi.Cod_Indirizzo, ContattiXIndirizzi.Tipo_Indirizzo, Indirizzi.ind_des, Indirizzi.frz_des, ISNULL(Indirizzi.CAP, '') AS CAP, ISNULL(ISTAT.LOCALITA, '') AS com_des, ISNULL(ISTAT.COMUNI_PROV, '') AS pro_cod, Indirizzi.stato, Indirizzi.note, Indirizzi.pro_cod_istat, Indirizzi.com_cod_istat " & vbCrLf)
            End If

            If FlagRubrica Then
                StbSQL.Append(", ISNULL(ContattiXRubrica.Cod_Rubrica,0) AS Cod_Rubrica, ISNULL(Rubrica.numero,'') AS Numero, ISNULL(Rubrica.descr,'') AS descr" & vbCrLf)
            End If

            If Flag_Costi Then
                StbSQL.Append(" , ISNULL(Prodotti_Costi.Elem_Cod, 1) AS Elem_Cod, ISNULL(Prodotti_Costi.Mezzo, -1) AS Mezzo,  ISNULL(Prodotti_Costi.Prezzo_Unitario, 0) AS Prezzo_Unitario,  ")
                StbSQL.Append(" ISNULL(Prodotti_Costi.Validita_Inizio, '01/01/1900') AS Costo_Inizio, ISNULL(Prodotti_Costi.Validita_Fine, '31/12/2100') AS Costo_Fine  ")
            End If

            '(04/12/2014 aggiunto sa_nome --> da oggi i contatti possono essere associati al centro)
            StbSQL.Append(",  ISNULL(Centri_Aziendali.Sa_Nome,'') AS Sa_Nome  ")


            StbSQL.Append(" FROM    Contatti  " & vbCrLf)
            StbSQL.Append(" INNER JOIN Risorse_Umane ON Contatti.Cod_Contatto = Risorse_Umane.Cod_Contatto AND Contatti.Piva = Risorse_Umane.Piva " & vbCrLf)
            StbSQL.Append(" INNER JOIN  Rapporti_Contabili ON Risorse_Umane.Cod_Rapporto = Rapporti_Contabili.Cod_Rapporto " & vbCrLf)
            StbSQL.Append(" INNER JOIN UtentiXImprese ON Rapporti_Contabili.Piva = UtentiXImprese.[USER] AND Risorse_Umane.Piva = UtentiXImprese.PIVA " & vbCrLf)

            'JOIN IMPRESE 
            StbSQL.Append(" INNER JOIN Imprese ON Imprese.Piva = Contatti.Piva " & vbCrLf)

            'StbSQL.Append(" LEFT JOIN Alert_Entita ON Contatti.Piva = Alert_Entita.Piva AND Contatti.Cod_Contatto= Alert_Entita.Cod_Contatto " & vbCrLf)
            'StbSQL.Append(" LEFT JOIN Allegati_Documenti ON Alert_Entita.Allegati_Documenti_Cod = Allegati_Documenti.Allegati_Documenti_Cod " & vbCrLf)
            'StbSQL.Append(" LEFT JOIN Alert_Elenco ON Alert_Entita.ID_Alert_Entita = Alert_Elenco.ID_Alert_Entita " & vbCrLf)

            If FlagIndirizzi Then
                StbSQL.Append("  INNER JOIN  ContattiXIndirizzi ON Contatti.Piva = ContattiXIndirizzi.Piva AND Contatti.Cod_Contatto = ContattiXIndirizzi.Cod_Contatto " & vbCrLf)
                StbSQL.Append("  INNER JOIN Indirizzi ON ContattiXIndirizzi.Cod_Indirizzo = Indirizzi.cod_indirizzo " & vbCrLf)
                StbSQL.Append("  LEFT OUTER JOIN ISTAT ON Indirizzi.pro_cod_istat = ISTAT.PROV AND Indirizzi.com_cod_istat = ISTAT.COM " & vbCrLf)
            End If

            If FlagRubrica Then
                StbSQL.Append(" LEFT OUTER JOIN ContattiXRubrica ON ContattiXRubrica.Piva = Contatti.Piva AND ContattiXRubrica.Cod_Contatto = Contatti.Cod_Contatto " & vbCrLf)
                StbSQL.Append(" LEFT OUTER JOIN Rubrica ON Rubrica.cod_rubrica = ContattiXRubrica.Cod_Rubrica" & vbCrLf)
            End If

            If Flag_Costi Then

                'FROM         Contatti INNER JOIN
                '      Risorse_Umane ON Contatti.Cod_Contatto = Risorse_Umane.Cod_Contatto LEFT OUTER JOIN
                '      Prodotti_Costi ON Risorse_Umane.Cod_RisUm = Prodotti_Costi.Mat_Cod
                StbSQL.Append(" LEFT OUTER JOIN Prodotti_Costi ON Risorse_Umane.Cod_RisUm= Prodotti_Costi.Mat_Cod And Id_Budget = 0 ")
            End If

            '(04/12/2014 aggiunto sa_nome --> da oggi i contatti possono essere associati al centro)
            StbSQL.Append(" LEFT OUTER JOIN Centri_Aziendali ON Contatti.Piva=Centri_Aziendali.Piva AND Contatti.Sa_Cod=Centri_Aziendali.Sa_Cod ")

            StbSQL.Append(" ")


            StbSQL.Append(" WHERE   (Rapporti_Contabili.Piva = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "')   " & vbCrLf)

            'StbSQL.Append(" AND ( Risorse_Umane.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ) " & vbCrLf)
            'StbSQL.Append(" AND ( Risorse_Umane.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ) " & vbCrLf)

            If Not FlagPubblico Then
                If Piva <> "" Then
                    'StbSQL.Append(" AND    (Contatti.Piva = '" & Agro_SQL_SaveText(Piva) & "')   " & vbCrLf)

                    'pubbliche
                    'StrSQL.Append(" AND    ( (Contatti.Sa_Cod = -1) ")

                    StbSQL.Append(" AND    ( ")

                    'visibilità centro
                    'leggo se ci sono filtri sui centri
                    Dim objUtentiVisibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
                    Dim FiltroCentri As String = ""
                    Dim DtCentriVisibili As DataTable
                    DtCentriVisibili = objUtentiVisibilita.Leggi(enum_TipoEntita.Centro, " Piva='" & Agro_SQL_SaveText(Piva) & "'", "", objParametri)
                    If DtCentriVisibili IsNot Nothing AndAlso DtCentriVisibili.Rows.Count > 0 Then
                        For i = 0 To DtCentriVisibili.Rows.Count - 1
                            FiltroCentri &= " (Contatti.Piva = '" & DtCentriVisibili.Rows(i).Item("piva") & "' AND Contatti.Sa_Cod = " & DtCentriVisibili.Rows(i).Item("sa_cod") & ") OR "
                        Next
                        If FiltroCentri <> "" Then
                            StbSQL.Append(" (" & Left(FiltroCentri, FiltroCentri.Length - 3) & " OR (Contatti.Piva = '" & Agro_SQL_SaveText(Piva) & "' AND Contatti.Sa_Cod = 0) ) ")
                        End If
                    End If

                    'aziendali
                    If FiltroCentri = "" Then
                        StbSQL.Append("    (Contatti.Piva = '" & Agro_SQL_SaveText(Piva) & "' AND Contatti.Sa_Cod <> -1 )  ")
                    End If

                    StbSQL.Append("        ) ")

                End If
            Else
                'pubblici + azienda
                If Piva <> "" Then

                    'StbSQL.Append(" AND    (Contatti.Piva = '" & Agro_SQL_SaveText(Piva) & "' OR Contatti.Sa_Cod = -1 )  " & vbCrLf)

                    'pubbliche
                    StbSQL.Append(" AND    ( (Contatti.Sa_Cod = -1) ")

                    'visibilità centro
                    'leggo se ci sono filtri sui centri
                    Dim objUtentiVisibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
                    Dim FiltroCentri As String = ""
                    Dim DtCentriVisibili As DataTable
                    DtCentriVisibili = objUtentiVisibilita.Leggi(enum_TipoEntita.Centro, " Piva='" & Agro_SQL_SaveText(Piva) & "'", "", objParametri)
                    If DtCentriVisibili IsNot Nothing AndAlso DtCentriVisibili.Rows.Count > 0 Then
                        For i = 0 To DtCentriVisibili.Rows.Count - 1
                            FiltroCentri &= " (Contatti.Piva = '" & DtCentriVisibili.Rows(i).Item("piva") & "' AND Contatti.Sa_Cod = " & DtCentriVisibili.Rows(i).Item("sa_cod") & ") OR "
                        Next
                        If FiltroCentri <> "" Then
                            StbSQL.Append(" OR (" & Left(FiltroCentri, FiltroCentri.Length - 3) & " OR (Contatti.Piva = '" & Agro_SQL_SaveText(Piva) & "' AND Contatti.Sa_Cod = 0) ) ")
                        End If
                    End If

                    'aziendali
                    If FiltroCentri = "" Then
                        StbSQL.Append("          OR  (Contatti.Piva = '" & Agro_SQL_SaveText(Piva) & "')  ")
                    End If

                    StbSQL.Append("        ) ")

                Else
                    'tutti pubblici
                    StbSQL.Append(" AND    (Contatti.Sa_Cod = -1)  " & vbCrLf)
                End If

            End If

            If Cod_Contatto <> "" Then
                StbSQL.Append(" AND    (Contatti.Cod_Contatto LIKE '%" & Agro_SQL_SaveText(Cod_Contatto) & "%')   " & vbCrLf)
            End If

            If ID_CF <> -99 Then
                StbSQL.Append(" AND     (Contatti.ID_CF = " & Agro_SQL_SaveNum(ID_CF.ToString) & ")   " & vbCrLf)
            End If

            If Cod_RisUm <> 0 Then
                StbSQL.Append(" AND     (Cod_RisUm = " & Agro_SQL_SaveNum(Cod_RisUm.ToString) & ")   " & vbCrLf)
            End If

            If Cod_Rapporto <> 0 Then
                StbSQL.Append(" AND     (Rapporti_Contabili.Cod_Rapporto = " & Agro_SQL_SaveNum(Cod_Rapporto.ToString) & ")   " & vbCrLf)
            End If

            If FlagIndirizzi Then
                If TipoIndirizzo <> 0 Then
                    StbSQL.Append(" AND     (ContattiXIndirizzi.Tipo_Indirizzo = " & Agro_SQL_SaveNum(TipoIndirizzo.ToString) & ")   " & vbCrLf)
                End If
                If CodIndirizzo <> 0 Then
                    StbSQL.Append(" AND     (ContattiXIndirizzi.Cod_Indirizzo = " & Agro_SQL_SaveNum(CodIndirizzo.ToString) & ")   " & vbCrLf)
                End If
            End If

            If FlagRubrica Then
                If CodRubrica <> 0 Then
                    StbSQL.Append(" AND     (Rubrica.Cod_Rubrica = " & Agro_SQL_SaveNum(CodRubrica.ToString) & ")   " & vbCrLf)
                End If
            End If

            'gias2gias
            If Cod_RisUm_Origine <> 0 Then
                StbSQL.Append(" AND  Risorse_Umane.Cod_RisUm_Origine = " & Agro_SQL_SaveNum(Cod_RisUm_Origine.ToString) & "   " & vbCrLf)
            End If
            If Piva_SuperUser_Origine <> "" Then
                StbSQL.Append(" AND  Risorse_Umane.Piva_SuperUser_Origine = '" & Agro_SQL_SaveText(Piva_SuperUser_Origine) & "'" & vbCrLf)
            End If
            If Not Flag_AncheImportati Then
                StbSQL.Append(" AND  Risorse_Umane.Cod_RisUm_Origine = 0 " & vbCrLf)
            End If
            'fine gias2gias

            If Cliente <> 0 Then
                StbSQL.Append(" AND Rapporti_Contabili.Cliente = 1 " & vbCrLf)
            End If

            If Fornitore <> 0 Then
                StbSQL.Append(" AND Rapporti_Contabili.Fornitore = 1 " & vbCrLf)
            End If

            If Dipendente <> 0 Then
                StbSQL.Append(" AND Rapporti_Contabili.Dipendente = 1 " & vbCrLf)
            End If

            If Terzista <> 0 Then
                StbSQL.Append(" AND Rapporti_Contabili.Terzista = 1 " & vbCrLf)
            End If

            If Legale <> 0 Then
                StbSQL.Append(" AND Rapporti_Contabili.Legale = 1 " & vbCrLf)
            End If

            'StbSQL.Append(" AND (Allegati_Documenti.Validazione_Data is NULL OR ( Allegati_Documenti.Validazione_Data < GETDATE() AND Alert_Elenco.Data_Scadenza > GETDATE() )) " & vbCrLf)

            If xFiltroAggiuntivo <> "" Then
                StbSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StbSQL.Append(" AND   Contatti.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StbSQL.Append(" AND   Contatti.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StbSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StbSQL.Append(" ORDER BY Contatti.Rag_Soc ASC  " & vbCrLf)
            End If

            '----------------------------------------------------
            '--- Recupero il datatable --------------------------
            '----------------------------------------------------

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StbSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing

        End Try

        Return DT

    End Function



    '##############################################################################################
    'Attenzione!!! Bisogna filtrare da codice l'elem_cod = 0
    'altrimenti c'è il rischio di leggere il prezzo di una macchina o di una materia prima
    'che ha il codice uguale al cod_risum!
    'non si può mettere il where perché la tabella è in left join
    '
    'il xFiltroAggiuntivo richiede l'AND
    Public Function Contatti_Costi_Leggi(ByVal Piva As String,
                                                ByVal Cod_Contatto As String,
                                                ByVal Cod_RisUm As Integer,
                                                ByVal Cod_Rapporto As Integer,
                                                ByVal FlagPubblico As Boolean,
                                                ByVal ID_CF As Integer,
                                                ByVal Cod_RisUm_Origine As Integer,
                                                ByVal Piva_SuperUser_Origine As String,
                                                ByVal Flag_AncheImportati As Boolean,
                                                ByVal Cliente As Integer,
                                                ByVal Fornitore As Integer,
                                                ByVal Dipendente As Integer,
                                                ByVal Terzista As Integer,
                                                ByVal Legale As Integer,
                                                    ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile,
                                                    ByVal xFiltroAggiuntivo As String,
                                                    ByVal xOrderBy As String,
                                                    ByRef objParametri As AgronicaCoreParametri
                                                    ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Contatti_R.Contatti_Costi_Leggi()"


        Dim StbSQL As New StringBuilder
        Dim MessaggioErrore As String = ""
        Dim DT As New DataTable
        Dim i As Integer = 0

        Try

            If Piva = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
            End If

            '----------------------------------------------------
            '--- Preparo la Query SQL ---------------------------
            '----------------------------------------------------

            StbSQL.Length = 0

            StbSQL.Append(" SELECT  Imprese.Rag_Soc AS Impresa, Contatti.Piva, Contatti.Sa_Cod, Contatti.Cod_Contatto, Contatti.Codice_Fiscale, Contatti.Id_CF, Contatti.Rag_Soc, Contatti.Convenevoli, " & vbCrLf)
            StbSQL.Append(" Contatti.Nome, Contatti.Cognome, Contatti.Data_Nascita, Contatti.Sesso, Contatti.Cod_Contatto_Referente, " & vbCrLf)
            StbSQL.Append(" Risorse_Umane.Sa_Cod AS Sa_Cod_2, Risorse_Umane.Cod_RisUm, Risorse_Umane.Validita_Inizio, Risorse_Umane.Validita_Fine, " & vbCrLf)
            StbSQL.Append(" Risorse_Umane.Settore_Des, Risorse_Umane.Attivita_Des, " & vbCrLf)
            StbSQL.Append(" Risorse_Umane.Occasionale, Risorse_Umane.Ore_Settimanali, Risorse_Umane.Giorni_Ferie, Risorse_Umane.Ferie_Godute, " & vbCrLf)
            StbSQL.Append(" Risorse_Umane.Giorni_Malattia, Risorse_Umane.Patentino, Risorse_Umane.Data_Rilascio_Patentino, Risorse_Umane.Data_Scadenza_Patentino, " & vbCrLf)
            StbSQL.Append(" Rapporti_Contabili.Piva AS Piva_SuperUser, Rapporti_Contabili.Sa_Cod AS Sa_Cod_3, Rapporti_Contabili.Cod_Rapporto, Rapporti_Contabili.Rapporto_Des, Rapporti_Contabili.Cliente,  " & vbCrLf)
            StbSQL.Append(" Rapporti_Contabili.Fornitore, Rapporti_Contabili.Dipendente, Rapporti_Contabili.Terzista, Rapporti_Contabili.Legale, " & vbCrLf)
            StbSQL.Append(" ISNULL(Prodotti_Costi.Elem_Cod, 0) AS Elem_Cod,  ISNULL(Prodotti_Costi.Mezzo, -1) AS Mezzo,  ISNULL(Prodotti_Costi.Prezzo_Unitario, 0) AS Prezzo_Unitario,  ")
            StbSQL.Append(" ISNULL(Prodotti_Costi.Validita_Inizio, '01/01/1900') AS Costo_Inizio, ISNULL(Prodotti_Costi.Validita_Fine, '31/12/2100') AS Costo_Fine  ")

            StbSQL.Append(" FROM    Contatti  " & vbCrLf)
            StbSQL.Append(" INNER JOIN Risorse_Umane ON Contatti.Cod_Contatto = Risorse_Umane.Cod_Contatto AND Contatti.Piva = Risorse_Umane.Piva " & vbCrLf)
            StbSQL.Append(" INNER JOIN Rapporti_Contabili ON Risorse_Umane.Cod_Rapporto = Rapporti_Contabili.Cod_Rapporto " & vbCrLf)
            StbSQL.Append(" INNER JOIN UtentiXImprese ON Rapporti_Contabili.Piva = UtentiXImprese.[USER] AND Risorse_Umane.Piva = UtentiXImprese.PIVA " & vbCrLf)
            StbSQL.Append(" INNER JOIN Imprese ON Imprese.Piva = Contatti.Piva " & vbCrLf)

            'JOIN MOVIMENTI_DETTAGLI - PRODOTTI COSTI
            'Bisogna filtrare dopo l'elem_cod
            StbSQL.Append(" LEFT OUTER JOIN Prodotti_Costi ")
            StbSQL.Append(" ON Prodotti_Costi.Mat_Cod = Risorse_Umane.Cod_RisUm And Prodotti_Costi.Id_Budget = 0 ")

            StbSQL.Append(" WHERE   (Rapporti_Contabili.Piva = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "')   " & vbCrLf)

            StbSQL.Append(" AND ( Risorse_Umane.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ) " & vbCrLf)
            StbSQL.Append(" AND ( Risorse_Umane.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ) " & vbCrLf)

            If Cod_Contatto <> "" Then
                StbSQL.Append(" AND    (Contatti.Cod_Contatto = '" & Agro_SQL_SaveText(Cod_Contatto) & "')   " & vbCrLf)
            End If

            If Not FlagPubblico Then
                ' If Piva <> "" Then
                StbSQL.Append(" AND    (Contatti.Piva = '" & Agro_SQL_SaveText(Piva) & "')   " & vbCrLf)
                ' End If
            Else

                'If Piva <> "" Then
                '    StbSQL.Append(" AND    (Contatti.Piva = '" & Agro_SQL_SaveText(Piva) & "' OR Contatti.Sa_Cod = -1 )  " & vbCrLf)
                'End If

                'pubbliche
                StbSQL.Append(" AND    ( (Contatti.Sa_Cod = -1) ")

                'visibilità centro
                'leggo se ci sono filtri sui centri
                Dim objUtentiVisibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
                Dim FiltroCentri As String = ""
                Dim DtCentriVisibili As DataTable
                DtCentriVisibili = objUtentiVisibilita.Leggi(enum_TipoEntita.Centro, " Piva='" & Agro_SQL_SaveText(Piva) & "'", "", objParametri)
                If DtCentriVisibili IsNot Nothing AndAlso DtCentriVisibili.Rows.Count > 0 Then
                    For i = 0 To DtCentriVisibili.Rows.Count - 1
                        FiltroCentri &= " (Contatti.Piva = '" & DtCentriVisibili.Rows(i).Item("piva") & "' AND Contatti.Sa_Cod = " & DtCentriVisibili.Rows(i).Item("sa_cod") & ") OR "
                    Next
                    If FiltroCentri <> "" Then
                        StbSQL.Append(" OR (" & Left(FiltroCentri, FiltroCentri.Length - 3) & " OR (Contatti.Piva = '" & Agro_SQL_SaveText(Piva) & "' AND Contatti.Sa_Cod = 0) ) ")
                    End If
                End If

                'aziendali
                If FiltroCentri = "" Then
                    StbSQL.Append("          OR  (Contatti.Piva = '" & Agro_SQL_SaveText(Piva) & "')  ")
                End If

                StbSQL.Append("        ) ")

            End If

            If ID_CF <> -99 Then
                StbSQL.Append(" AND     (Contatti.ID_CF = " & Agro_SQL_SaveNum(ID_CF.ToString) & ")   " & vbCrLf)
            End If

            If Cod_RisUm <> 0 Then
                StbSQL.Append(" AND     (Cod_RisUm = " & Agro_SQL_SaveNum(Cod_RisUm.ToString) & ")   " & vbCrLf)
            End If

            If Cod_Rapporto <> 0 Then
                StbSQL.Append(" AND     (Rapporti_Contabili.Cod_Rapporto = " & Agro_SQL_SaveNum(Cod_Rapporto.ToString) & ")   " & vbCrLf)
            End If

            'gias2gias
            If Cod_RisUm_Origine <> 0 Then
                StbSQL.Append(" AND  Risorse_Umane.Cod_RisUm_Origine = " & Agro_SQL_SaveNum(Cod_RisUm_Origine.ToString) & "   " & vbCrLf)
            End If
            If Piva_SuperUser_Origine <> "" Then
                StbSQL.Append(" AND  Risorse_Umane.Piva_SuperUser_Origine = '" & Agro_SQL_SaveText(Piva_SuperUser_Origine) & "'" & vbCrLf)
            End If
            If Not Flag_AncheImportati Then
                StbSQL.Append(" AND  Risorse_Umane.Cod_RisUm_Origine = 0 " & vbCrLf)
            End If
            'fine gias2gias

            If Cliente <> 0 Then
                StbSQL.Append(" AND Rapporti_Contabili.Cliente = 1 " & vbCrLf)
            End If

            If Fornitore <> 0 Then
                StbSQL.Append(" AND Rapporti_Contabili.Fornitore = 1 " & vbCrLf)
            End If

            If Dipendente <> 0 Then
                StbSQL.Append(" AND Rapporti_Contabili.Dipendente = 1 " & vbCrLf)
            End If

            If Terzista <> 0 Then
                StbSQL.Append(" AND Rapporti_Contabili.Terzista = 1 " & vbCrLf)
            End If

            If Legale <> 0 Then
                StbSQL.Append(" AND Rapporti_Contabili.Legale = 1 " & vbCrLf)
            End If

            If xFiltroAggiuntivo <> "" Then
                StbSQL.Append(Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StbSQL.Append(" AND   Contatti.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StbSQL.Append(" AND   Contatti.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StbSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StbSQL.Append(" ORDER BY Contatti.Rag_Soc ASC  " & vbCrLf)
            End If

            '----------------------------------------------------
            '--- Recupero il datatable --------------------------
            '----------------------------------------------------

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StbSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
        End Try

        Return DT


    End Function


    Public Sub Ottieni_Dati_Patentino(ByVal Piva As String,
                                      ByVal Cod_Contatto As String,
                                      ByRef Patentino As String,
                                      ByRef Data_Rilascio As Date,
                                      ByRef Data_Scadenza As Date,
                                      ByRef Ente As String,
                                      ByVal DataRiferimento As Date,
                                      ByRef objParametri As AgronicaCoreParametri
                                      )

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Contatti_R.Leggi_2()"

        Dim StbSQL As New StringBuilder
        Dim MessaggioErrore As String = ""
        Dim DT As New DataTable

        Try

            '----------------------------------------------------
            '--- Preparo la Query SQL ---------------------------
            '----------------------------------------------------

            StbSQL.Length = 0

            StbSQL.AppendLine("SELECT Allegati_Documenti.Allegati_Documenti_Numero, Allegati_Documenti.Validazione_Data, Alert_Elenco.Data_Scadenza, Allegati_Documenti.Allegati_Documenti_Ente_Des  FROM Alert_Entita ")
            StbSQL.AppendLine(" JOIN Allegati_Documenti ON Alert_Entita.Allegati_Documenti_Cod = Allegati_Documenti.Allegati_Documenti_Cod ")
            StbSQL.AppendLine(" JOIN Alert_Elenco ON Alert_Elenco.ID_Alert_Entita = Alert_Entita.ID_Alert_Entita ")
            StbSQL.AppendLine(" WHERE Alert_Entita.TipoEntita_Cod = 8 ")
            StbSQL.AppendLine(" AND Alert_Entita.Piva = " & Agro_SQL_SaveText_NULL(Piva) & "  ")
            StbSQL.AppendLine(" AND Alert_Entita.Cod_Contatto= " & Agro_SQL_SaveText_NULL(Cod_Contatto) & " ")
            StbSQL.AppendLine(" ORDER BY Alert_Elenco.Data_Scadenza DESC ")

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StbSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            If DT.Rows.Count = 0 Then
                Patentino = ""
                Data_Rilascio = AGRODATAINIZIO
                Data_Scadenza = AGRODATAFINE
                Ente = ""
            Else
                Patentino = DT.Rows(0).Item("Allegati_Documenti_Numero")
                Data_Rilascio = DT.Rows(0).Item("Validazione_Data")
                Data_Scadenza = DT.Rows(0).Item("Data_Scadenza")
                Ente = DT.Rows(0).Item("Allegati_Documenti_Ente_Des")

                DataRiferimento = CDate(DataRiferimento.ToShortDateString)

                Dim filtered_String = " Validazione_Data <= #" & DataRiferimento.ToString(DateTimeFormatInfo.InvariantInfo) & "# AND Data_Scadenza >= #" & DataRiferimento.ToString(DateTimeFormatInfo.InvariantInfo) & "# "

                Dim Dt_Filtered As DataRow() = DT.Select(filtered_String)

                If Dt_Filtered.Count > 0 Then
                    Patentino = Dt_Filtered(0).Item("Allegati_Documenti_Numero")
                    Data_Rilascio = Dt_Filtered(0).Item("Validazione_Data")
                    Data_Scadenza = Dt_Filtered(0).Item("Data_Scadenza")
                    Ente = Dt_Filtered(0).Item("Allegati_Documenti_Ente_Des")
                End If

            End If

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
        End Try

    End Sub


    '###########################################################################
    Public Function SuperUser_Contatto_Leggi(ByVal Cod_Rapporto As Integer,
                                             ByVal xFiltroAggiuntivo As String,
                                             ByVal xOrderBy As String,
                                             ByRef objParametri As AgronicaCoreParametri
                                             ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Contatti_R.SuperUser_Contatto_Leggi()"

        Dim StbSQL As New StringBuilder
        Dim MessaggioErrore As String = ""
        Dim DT As New DataTable

        Try

            StbSQL.Length = 0

            StbSQL.Append(" SELECT     Contatti.Piva, Contatti.Sa_Cod, Contatti.Cod_Contatto, Contatti.Id_CF, Contatti.Codice_Fiscale, Risorse_Umane.Cod_RisUm,  " & vbCrLf)
            StbSQL.Append(" Risorse_Umane.Validita_Inizio, Risorse_Umane.Validita_Fine, Risorse_Umane.Settore_Des, Risorse_Umane.Attivita_Des, " & vbCrLf)
            StbSQL.Append(" Rapporti_Contabili.Cod_Rapporto, Rapporti_Contabili.Rapporto_Des, Rapporti_Contabili.Cliente, Rapporti_Contabili.Fornitore, " & vbCrLf)
            StbSQL.Append(" Rapporti_Contabili.Dipendente, Rapporti_Contabili.Terzista, Rapporti_Contabili.Legale, Rapporti_Contabili.Agente, Imprese.rag_soc, " & vbCrLf)
            StbSQL.Append(" ImpresexIndirizzi.cod_indirizzo, ImpresexIndirizzi.Tipo_Indirizzo, Indirizzi.ind_des, Indirizzi.frz_des, Indirizzi.CAP, Indirizzi.stato, Indirizzi.note, " & vbCrLf)
            StbSQL.Append(" Indirizzi.pro_cod_istat, Indirizzi.com_cod_istat, ISTAT.COMUNI_PROV, ISTAT.LOCALITA " & vbCrLf)

            StbSQL.Append(" FROM Contatti " & vbCrLf)
            StbSQL.Append(" INNER JOIN  Risorse_Umane ON Contatti.Piva = Risorse_Umane.Piva AND Contatti.Cod_Contatto = Risorse_Umane.Cod_Contatto " & vbCrLf)
            StbSQL.Append(" INNER JOIN  Imprese ON Risorse_Umane.Cod_Contatto = Imprese.PIVA AND Risorse_Umane.Piva = Imprese.PIVA  " & vbCrLf)
            StbSQL.Append(" INNER JOIN ImpresexIndirizzi ON Imprese.PIVA = ImpresexIndirizzi.PIVA " & vbCrLf)
            StbSQL.Append(" INNER JOIN Indirizzi ON ImpresexIndirizzi.cod_indirizzo = Indirizzi.cod_indirizzo  " & vbCrLf)
            StbSQL.Append(" INNER JOIN Rapporti_Contabili ON Imprese.PIVA = Rapporti_Contabili.Piva AND Risorse_Umane.Cod_Rapporto = Rapporti_Contabili.Cod_Rapporto  " & vbCrLf)
            StbSQL.Append(" INNER JOIN ISTAT ON Indirizzi.pro_cod_istat = ISTAT.PROV AND Indirizzi.com_cod_istat = ISTAT.COM " & vbCrLf)
            StbSQL.Append("  " & vbCrLf)

            StbSQL.Append(" WHERE   Rapporti_Contabili.Piva = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'  " & vbCrLf)
            StbSQL.Append(" AND     Contatti.Cod_contatto = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'   " & vbCrLf)

            If Cod_Rapporto <> 0 Then
                StbSQL.Append(" AND     Rapporti_Contabili.Cod_Rapporto = " & Agro_SQL_SaveNum(Cod_Rapporto.ToString) & "   " & vbCrLf)
            End If

            If xFiltroAggiuntivo <> "" Then
                StbSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StbSQL.Append(" AND   Contatti.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StbSQL.Append(" AND   Contatti.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StbSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StbSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
        End Try

        Return DT

    End Function


    '###########################################################################
    Public Function ImpresaGias_Contatto_Leggi(ByVal Piva As String,
                                               ByVal Cod_Rapporto As Integer,
                                               ByVal xFiltroAggiuntivo As String,
                                               ByVal xOrderBy As String,
                                               ByRef objParametri As AgronicaCoreParametri
                                               ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Contatti_R.ImpresaGias_Contatto_Leggi()"

        Dim StbSQL As New StringBuilder
        Dim MessaggioErrore As String = ""
        Dim DT As New DataTable

        Try

            StbSQL.Length = 0

            StbSQL.Append(" SELECT     Contatti.Piva, Contatti.Sa_Cod, Contatti.Cod_Contatto, Contatti.Id_CF, Contatti.Codice_Fiscale, Risorse_Umane.Cod_RisUm,  " & vbCrLf)
            StbSQL.Append(" Risorse_Umane.Validita_Inizio, Risorse_Umane.Validita_Fine, Risorse_Umane.Settore_Des, Risorse_Umane.Attivita_Des, " & vbCrLf)
            StbSQL.Append(" Rapporti_Contabili.Cod_Rapporto, Rapporti_Contabili.Rapporto_Des, Rapporti_Contabili.Cliente, Rapporti_Contabili.Fornitore, " & vbCrLf)
            StbSQL.Append(" Rapporti_Contabili.Dipendente, Rapporti_Contabili.Terzista, Rapporti_Contabili.Legale, Rapporti_Contabili.Agente, Imprese.rag_soc, " & vbCrLf)
            StbSQL.Append(" ImpresexIndirizzi.cod_indirizzo, ImpresexIndirizzi.Tipo_Indirizzo, Indirizzi.ind_des, Indirizzi.frz_des, Indirizzi.CAP, Indirizzi.stato, Indirizzi.note, " & vbCrLf)
            StbSQL.Append(" Indirizzi.pro_cod_istat, Indirizzi.com_cod_istat, ISTAT.COMUNI_PROV, ISTAT.LOCALITA " & vbCrLf)

            StbSQL.Append(" FROM Contatti " & vbCrLf)
            StbSQL.Append(" INNER JOIN  Risorse_Umane ON Contatti.Piva = Risorse_Umane.Piva AND Contatti.Cod_Contatto = Risorse_Umane.Cod_Contatto " & vbCrLf)

            '09/09/2020: messo un pezzo di join sotto l'IF, altrimenti recuperava i dati solo per l'azienda superuser e non per le altre
            'StbSQL.Append(" INNER JOIN  Imprese ON Risorse_Umane.Cod_Contatto = Imprese.PIVA AND Risorse_Umane.Piva = Imprese.PIVA  " & vbCrLf)
            StbSQL.Append(" INNER JOIN  Imprese ON Risorse_Umane.Cod_Contatto = Imprese.PIVA  " & vbCrLf)
            If Piva = objParametri.PivaSuperUser Then
                StbSQL.Append("  AND Risorse_Umane.Piva = Imprese.PIVA  " & vbCrLf)
            End If

            StbSQL.Append(" INNER JOIN ImpresexIndirizzi ON Imprese.PIVA = ImpresexIndirizzi.PIVA " & vbCrLf)
            StbSQL.Append(" INNER JOIN Indirizzi ON ImpresexIndirizzi.cod_indirizzo = Indirizzi.cod_indirizzo  " & vbCrLf)
            StbSQL.Append(" INNER JOIN Rapporti_Contabili ON Risorse_Umane.Cod_Rapporto = Rapporti_Contabili.Cod_Rapporto  " & vbCrLf)
            StbSQL.Append(" INNER JOIN ISTAT ON Indirizzi.pro_cod_istat = ISTAT.PROV AND Indirizzi.com_cod_istat = ISTAT.COM " & vbCrLf)
            StbSQL.Append("  " & vbCrLf)

            StbSQL.Append(" WHERE   Rapporti_Contabili.Piva = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'  " & vbCrLf)
            StbSQL.Append(" AND     Contatti.Cod_contatto = '" & Agro_SQL_SaveText(Piva) & "'   " & vbCrLf)

            If Cod_Rapporto <> 0 Then
                StbSQL.Append(" AND     Rapporti_Contabili.Cod_Rapporto = " & Agro_SQL_SaveNum(Cod_Rapporto.ToString) & "   " & vbCrLf)
            End If

            'conviene far passare dal chiamante il filtro sul sa_cod PUBBLICO= -1
            If xFiltroAggiuntivo <> "" Then
                StbSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StbSQL.Append(" AND   Contatti.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StbSQL.Append(" AND   Contatti.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StbSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                '09/09/2020: così viene letto prima il cod_risum pubblico
                StbSQL.Append(" ORDER BY sa_cod ASC ")
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StbSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
        End Try

        Return DT

    End Function



    Public Function ControllaSePresente(ByVal Piva As String,
                                        ByVal Cod_Contatto As String,
                                        ByRef objParametri As AgronicaCoreParametri
                                        ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Contatti_R.Contatti_Contatto_Leggi()"

        Dim StbSQL As New StringBuilder
        Dim MessaggioErrore As String = ""
        Dim DT As New DataTable

        Try

            StbSQL.Length = 0

            StbSQL.Append(" SELECT  * ")
            StbSQL.Append(" from Contatti  " & vbCrLf)


            StbSQL.Append(" WHERE   cod_contatto = '" & Agro_SQL_SaveText(Cod_Contatto) & "'   " & vbCrLf)
            StbSQL.Append(" AND   Piva = '" & Agro_SQL_SaveText(Piva) & "'   " & vbCrLf)

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StbSQL.Append(" AND   Contatti.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StbSQL.Append(" AND   Contatti.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StbSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
        End Try

        Return DT

    End Function


    '###############################################################################
    Public Function VerificaEsistenza_CodContatto_as_PivaGIAS(ByVal Cod_RisUm As Integer,
                                                              ByRef Piva As String,
                                                              ByRef objParametri As AgronicaCoreParametri
                                                                ) As Boolean



        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Contatti_R.VerificaEsistenza_CodContatto_AS_PivaGIAS()"

        Dim StbSQL As New StringBuilder
        Dim MessaggioErrore As String = ""
        Dim DT As New DataTable
        Dim ImpresaGias As Boolean = False

        Try


            '----------------------------------------------------
            '--- Preparo la Query SQL ---------------------------
            '----------------------------------------------------

            StbSQL.Length = 0

            StbSQL.Append(" SELECT Imprese.* ")
            StbSQL.Append(" FROM Imprese INNER JOIN Contatti ON Imprese.Piva = Contatti.Cod_Contatto  " & vbCrLf)
            StbSQL.Append(" INNER JOIN Risorse_Umane ON Contatti.Cod_Contatto = Risorse_Umane.Cod_Contatto " & vbCrLf)
            StbSQL.Append(" AND Contatti.Piva = Risorse_Umane.Piva  " & vbCrLf)
            StbSQL.Append(" INNER JOIN UtentiXImprese ON Imprese.Piva = UtentiXImprese.PIVA  " & vbCrLf)
            StbSQL.Append(" WHERE UtentiXImprese.[USER] = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' " & vbCrLf)
            StbSQL.Append(" AND   Risorse_Umane.Cod_RisUm =" & Agro_SQL_SaveNum(Cod_RisUm) & " " & vbCrLf)

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StbSQL.Append(" AND   Contatti.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StbSQL.Append(" AND   Contatti.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StbSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            If DT.Rows.Count > 0 Then
                Piva = DT.Rows(0).Item("Piva")
                ImpresaGias = True
            Else
                Piva = ""
                ImpresaGias = False
            End If

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Piva = ""
            Return False
        End Try

        Return ImpresaGias

    End Function

    Public Function Leggi_Contatti(piva As String, Sa_Cod As String, Cod_Contatto As String, objParametri_Server As AgronicaCoreParametri) As Contatti

        Dim elem As Contatti = Nothing

        Dim Piva_SuperUser = objParametri_Server.PivaSuperUser

        '----- Descrizione
        Dim NomeRoutine As String = "Contatti_R.Leggi_Contatti()"

        Dim gefutils As New Gias_EF_Utility

        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri_Server.StringaConnessione)
        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            elem =
           (From c In GiasContext.Contatti
            Where c.Piva = piva AndAlso c.Sa_Cod = Sa_Cod AndAlso c.Cod_Contatto = Cod_Contatto
            Select c).FirstOrDefault()

        End Using

        Return elem

    End Function

    '#######################################################################
    Public Function Leggi_Contatti_ByCod_Rapporto(
                                                 ByVal FlagLeggiPubblici As Boolean,
                                                 ByVal Piva As String,
                                                 ByVal Cod_Contatto As String,
                                                 ByVal Cod_Rapporto As Integer,
                                                 ByVal xFiltroAggiuntivo As String,
                                                 ByVal xOrderBy As String,
                                                 ByRef objParametri As AgronicaCoreParametri,
                                                 Optional ByVal Progressivo As String = ""
                                                 ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Contatti_R.Leggi_Contatti_ByCod_Rapporto()"

        Dim StbSQL As New StringBuilder
        Dim MessaggioErrore As String = ""
        Dim DT As New DataTable

        Try

            StbSQL.Length = 0

            StbSQL.AppendLine(" SELECT  Contatti.Piva, Contatti.Sa_Cod, Contatti.Cod_Contatto, Contatti.Codice_Fiscale, Contatti.Id_CF, Contatti.Rag_Soc, " & vbCrLf)
            StbSQL.AppendLine(" Contatti.Nome, Contatti.Cognome, Contatti.Data_Nascita, Contatti.Sesso, Contatti.Cod_Contatto_Referente, " & vbCrLf)
            StbSQL.AppendLine(" Risorse_Umane.Cod_RisUm, Risorse_Umane.Validita_Inizio, Risorse_Umane.Validita_Fine, " & vbCrLf)
            StbSQL.AppendLine(" Risorse_Umane.Settore_Des, Risorse_Umane.Attivita_Des,  " & vbCrLf)
            StbSQL.AppendLine(" Risorse_Umane.Patentino, Risorse_Umane.Data_Rilascio_Patentino, Risorse_Umane.Data_Scadenza_Patentino, " & vbCrLf)
            StbSQL.AppendLine(" Rapporti_Contabili.Piva AS Piva_SuperUser, Rapporti_Contabili.Sa_Cod AS Sa_Cod_3, Rapporti_Contabili.Cod_Rapporto, Rapporti_Contabili.Rapporto_Des  " & vbCrLf)

            StbSQL.AppendLine(" FROM    Contatti  " & vbCrLf)
            StbSQL.AppendLine(" INNER JOIN Risorse_Umane ON Contatti.Cod_Contatto = Risorse_Umane.Cod_Contatto AND Contatti.Piva = Risorse_Umane.Piva " & vbCrLf)
            StbSQL.AppendLine(" INNER JOIN  Rapporti_Contabili ON Risorse_Umane.Cod_Rapporto = Rapporti_Contabili.Cod_Rapporto " & vbCrLf)
            StbSQL.AppendLine(" INNER JOIN UtentiXImprese ON Rapporti_Contabili.Piva = UtentiXImprese.[USER] AND Risorse_Umane.Piva = UtentiXImprese.PIVA " & vbCrLf)

            StbSQL.AppendLine(" WHERE   (Rapporti_Contabili.Piva = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "')   " & vbCrLf)

            If Not FlagLeggiPubblici Then
                If Piva <> "" Then
                    StbSQL.AppendLine(" AND    (Contatti.Piva = '" & Agro_SQL_SaveText(Piva) & "')   " & vbCrLf)
                End If
            Else
                If Piva <> "" Then
                    StbSQL.AppendLine(" AND    (Contatti.Piva = '" & Agro_SQL_SaveText(Piva) & "' OR Contatti.Sa_Cod = -1 )  " & vbCrLf)
                Else
                    StbSQL.AppendLine("AND Contatti.Sa_Cod = -1")
                End If
            End If

            If Cod_Contatto <> "" Then
                StbSQL.AppendLine(" AND    (Contatti.Cod_Contatto = '" & Agro_SQL_SaveText(Cod_Contatto) & "')   " & vbCrLf)
            End If

            If Cod_Rapporto <> 0 Then
                StbSQL.AppendLine(" AND     (Rapporti_Contabili.Cod_Rapporto = " & Agro_SQL_SaveNum(Cod_Rapporto.ToString) & ")   " & vbCrLf)
            End If

            If xFiltroAggiuntivo <> "" Then
                StbSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            If xOrderBy <> "" Then
                StbSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StbSQL.AppendLine(" ORDER BY Contatti.Rag_Soc ASC  " & vbCrLf)
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StbSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function

    Public Function Leggi_Contatti_ImpreseGias_ByCod_Rapporto(ByVal FlagLeggiPubblici As Boolean,
                                                              ByVal Piva As String,
                                                              ByVal Cod_Contatto As String,
                                                              ByVal Cod_Rapporto As Integer,
                                                              ByVal xFiltroAggiuntivo As String,
                                                              ByVal xOrderBy As String,
                                                              ByRef objParametri As AgronicaCoreParametri,
                                                              Optional ByVal Progressivo As String = ""
                                                              ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Contatti_R.Leggi_Contatti_ImpreseGias_ByCod_Rapporto()"

        Dim StbSQL As New StringBuilder
        Dim MessaggioErrore As String = ""
        Dim DT As New DataTable

        Try

            StbSQL.Length = 0

            StbSQL.Append(" SELECT  Contatti.Piva, Contatti.Sa_Cod, Contatti.Cod_Contatto, Contatti.Codice_Fiscale, Contatti.Id_CF, Contatti.Rag_Soc, " & vbCrLf)
            StbSQL.Append(" Contatti.Nome, Contatti.Cognome, Contatti.Data_Nascita, Contatti.Sesso, Contatti.Cod_Contatto_Referente, " & vbCrLf)
            StbSQL.Append(" Risorse_Umane.Cod_RisUm, Risorse_Umane.Validita_Inizio, Risorse_Umane.Validita_Fine, " & vbCrLf)
            StbSQL.Append(" Risorse_Umane.Settore_Des, Risorse_Umane.Attivita_Des,  " & vbCrLf)
            StbSQL.Append(" Risorse_Umane.Patentino, Risorse_Umane.Data_Rilascio_Patentino, Risorse_Umane.Data_Scadenza_Patentino, " & vbCrLf)
            StbSQL.Append(" Rapporti_Contabili.Piva AS Piva_SuperUser, Rapporti_Contabili.Sa_Cod AS Sa_Cod_3, Rapporti_Contabili.Cod_Rapporto, Rapporti_Contabili.Rapporto_Des  " & vbCrLf)

            StbSQL.Append(" FROM    Contatti  " & vbCrLf)
            StbSQL.Append(" INNER JOIN Risorse_Umane ON Contatti.Cod_Contatto = Risorse_Umane.Cod_Contatto AND Contatti.Piva = Risorse_Umane.Piva " & vbCrLf)
            StbSQL.Append(" INNER JOIN  Rapporti_Contabili ON Risorse_Umane.Cod_Rapporto = Rapporti_Contabili.Cod_Rapporto " & vbCrLf)
            StbSQL.Append(" INNER JOIN UtentiXImprese ON Rapporti_Contabili.Piva = UtentiXImprese.[USER] AND Risorse_Umane.Piva = UtentiXImprese.PIVA " & vbCrLf)

            StbSQL.Append(" INNER JOIN Imprese ON Imprese.Piva = Contatti.Cod_Contatto " & vbCrLf)

            StbSQL.Append(" WHERE   (Rapporti_Contabili.Piva = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "')   " & vbCrLf)

            If Not FlagLeggiPubblici Then
                If Piva <> "" Then
                    StbSQL.Append(" AND    (Contatti.Piva = '" & Agro_SQL_SaveText(Piva) & "')   " & vbCrLf)
                End If
            Else
                If Piva <> "" Then
                    StbSQL.Append(" AND    (Contatti.Piva = '" & Agro_SQL_SaveText(Piva) & "' OR Contatti.Sa_Cod = -1 )  " & vbCrLf)
                End If
            End If

            If Cod_Contatto <> "" Then
                StbSQL.Append(" AND    (Contatti.Cod_Contatto = '" & Agro_SQL_SaveText(Cod_Contatto) & "')   " & vbCrLf)
            End If

            If Cod_Rapporto <> 0 Then
                StbSQL.Append(" AND     (Rapporti_Contabili.Cod_Rapporto = " & Agro_SQL_SaveNum(Cod_Rapporto.ToString) & ")   " & vbCrLf)
            End If

            If xFiltroAggiuntivo <> "" Then
                StbSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            If xOrderBy <> "" Then
                StbSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StbSQL.Append(" ORDER BY Contatti.Rag_Soc ASC  " & vbCrLf)
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StbSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function


    Public Function Leggi_Fornitori_FF(ByVal piva As String, ByRef objParametri As AgronicaCoreParametri) As String

        Dim risposta As String = ""

        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.FF_CampionamentoConferimento_R.Leggi_Fornitori()"

        Dim rapporto As Integer?() = {COD_FORNITORE_ORTOFRUTTA, COD_CONFERENTE, COD_CLIENTE_FORNITORE_BolleAccett}

        Dim gefutils As New Gias_EF_Utility

        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            Dim Fornitori =
                (From Contatti In GiasContext.Contatti
                 Join RisorseUmane In GiasContext.Risorse_Umane
                On
                    Contatti.Cod_Contatto Equals RisorseUmane.Cod_Contatto And
                    Contatti.Piva Equals RisorseUmane.Piva
                 Join RapportiContabili In GiasContext.Rapporti_Contabili
                On
                    RisorseUmane.Cod_Rapporto Equals RapportiContabili.Cod_Rapporto
                 Where
                    (Contatti.Sa_Cod = -1 OrElse Contatti.Piva = piva) AndAlso
                    (Contatti.Id_CF <> 0) AndAlso
                    rapporto.Contains(RapportiContabili.Cod_Rapporto) AndAlso
                    (RisorseUmane.Cod_RisUm_Origine = 0)
                 Select New With {
                     .Cod_Contatto = Contatti.Cod_Contatto,
                     .Cod_RisUm = RisorseUmane.Cod_RisUm,
                     .Rag_Soc = Contatti.Rag_Soc & " (" & RapportiContabili.Rapporto_Des & ")"
                     }).OrderBy(Function(f) f.Rag_Soc)

            Dim serializerSettings As New JsonSerializerSettings With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}
            risposta = JsonConvert.SerializeObject(Fornitori.ToList(), Formatting.None, serializerSettings)

        End Using

        Return risposta

    End Function

    Public Function LeggiLegaliRappresentanti(Piva As String, Filtro_Visibilita_Utente As Boolean, ByRef objParametri As AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Contatti_R.LeggiLegaliRappresentanti()"

        Dim StbSQL As New StringBuilder
        Dim MessaggioErrore As String = ""
        Dim DT As New DataTable

        Try

            StbSQL.Length = 0

            StbSQL.AppendLine(" SELECT ")
            StbSQL.AppendLine(" Contatti.Piva, ")
            StbSQL.AppendLine(" Contatti.Nome, ")
            StbSQL.AppendLine(" Contatti.Cognome, ")
            StbSQL.AppendLine(" ISNULL(Contatti.Data_Nascita, CONVERT(datetime, '1900-01-01 00:00:00.000', 120)) as Data_Nascita, ")
            StbSQL.AppendLine(" Indirizzo_Legale.ind_des As Indirizzo_Legale, ")
            StbSQL.AppendLine(" Indirizzo_Legale.frz_des as Frazione_Legale, ")
            StbSQL.AppendLine(" Indirizzo_Legale.stato As Stato_Legale, ")
            StbSQL.AppendLine(" ISTAT_Legale.COMUNI_PROV as Prov_Legale, ")
            StbSQL.AppendLine(" ISTAT_Legale.Localita As Com_Legale, ")
            StbSQL.AppendLine(" Indirizzo_Legale.CAP as CAP_Legale ")

            StbSQL.AppendLine(" FROM    Contatti  ")
            StbSQL.AppendLine(" INNER JOIN Risorse_Umane ON Contatti.Cod_Contatto = Risorse_Umane.Cod_Contatto AND Contatti.Piva = Risorse_Umane.Piva AND Risorse_Umane.Cod_Rapporto = -1 ")
            StbSQL.AppendLine("  LEFT JOIN ContattixIndirizzi ON Contatti.Piva = ContattixIndirizzi.Piva AND Contatti.Cod_Contatto = ContattixIndirizzi.Cod_Contatto And ContattixIndirizzi.Tipo_Indirizzo = 3 ")
            StbSQL.AppendLine("  LEFT JOIN Indirizzi Indirizzo_Legale ON ContattixIndirizzi.Cod_Indirizzo = Indirizzo_Legale.cod_indirizzo ")
            StbSQL.AppendLine("  LEFT JOIN ISTAT ISTAT_Legale ON Indirizzo_Legale.pro_cod_istat = ISTAT_Legale.PROV AND Indirizzo_Legale.com_cod_istat = ISTAT_Legale.COM ")
            StbSQL.AppendLine(" INNER JOIN Imprese ON Imprese.Piva = Contatti.Piva ")

            If Filtro_Visibilita_Utente Then
                StbSQL.AppendLine("   LEFT JOIN Utenti_Visibilita_Appoggio (NOLOCK) On Imprese.Piva = Utenti_Visibilita_Appoggio.Piva AND Utenti_Visibilita_Appoggio.Entita_Cod=1  ")
            End If

            StbSQL.AppendLine(" WHERE  1 = 1   " & vbCrLf)

            If Piva <> "" Then
                StbSQL.Append(" AND    Contatti.Piva = '" & Agro_SQL_SaveText(Piva) & "'   " & vbCrLf)
            End If

            If Filtro_Visibilita_Utente Then
                StbSQL.AppendLine(" AND Utenti_Visibilita_Appoggio.Username = " & Agro_SQL_SaveText_NULL(objParametri.UtenteUsername) & "  ")
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StbSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function

    Public Function LeggiFornitoriEUDR(Piva As String,
                                       Cod_Contatto As String,
                                       ByRef objParametri As AgronicaCoreParametri,
                                       Optional Cod_Fornitore As String = "",
                                       Optional Filtro_Pubblici As Boolean = True,
                                       Optional Filtro_Visibilita_Utente As Boolean = False) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Contatti_R.LeggiFornitoriEUDR()"

        Dim StbSQL As New StringBuilder
        Dim MessaggioErrore As String = ""
        Dim DT As New DataTable

        Try

            StbSQL.AppendLine(" SELECT ")
            StbSQL.AppendLine(" Contatti.Piva, ")
            StbSQL.AppendLine(" Contatti.Cod_Contatto, ")
            StbSQL.AppendLine(" Contatti.Rag_Soc, ")
            StbSQL.AppendLine(" Contatti.Nome, ")
            StbSQL.AppendLine(" Contatti.Cognome, ")
            StbSQL.AppendLine(" Risorse_Umane.Settore_Des as Cod_Fornitore, ")
            StbSQL.AppendLine(" CASE Contatti.Rag_Soc WHEN '' THEN Contatti.Cognome + ' ' + Contatti.Nome ELSE Contatti.Rag_Soc END as Fornitore, ")
            StbSQL.AppendLine(" ISNULL(Indirizzo_Contatto.ind_des, Indirizzo_Impresa.ind_des) As Indirizzo, ")
            StbSQL.AppendLine(" ISNULL(Indirizzo_Contatto.frz_des, Indirizzo_Impresa.frz_des) as Frazione, ")
            StbSQL.AppendLine(" ISNULL(Indirizzo_Contatto.CAP, Indirizzo_Impresa.CAP) as CAP, ")
            StbSQL.AppendLine(" ISNULL(ISTAT_Contatto.Localita, ISTAT_Impresa.Localita) As Comune, ")
            StbSQL.AppendLine(" ISNULL(ISTAT_Contatto.COMUNI_PROV, ISTAT_Impresa.COMUNI_PROV) as Provincia, ")
            StbSQL.AppendLine(" ISNULL(Indirizzo_Contatto.stato, Indirizzo_Impresa.stato) As Stato, ")
            StbSQL.AppendLine(" EUDR_Classificazione_Paesi.Descrizione AS Paese, EUDR_Classificazione_Paesi.PaeseEU, ")
            StbSQL.AppendLine(" EUDR_Classificazione_Paesi.Rischio, EUDR_Classificazione_Paesi.Punteggio ")

            StbSQL.AppendLine(" FROM Contatti  ")
            StbSQL.AppendLine(" INNER JOIN Risorse_Umane ON Contatti.Cod_Contatto = Risorse_Umane.Cod_Contatto AND Contatti.Piva = Risorse_Umane.Piva AND Risorse_Umane.Cod_Rapporto = -3 ")
            StbSQL.AppendLine(" LEFT JOIN ContattixIndirizzi ON Contatti.Piva = ContattixIndirizzi.Piva AND Contatti.Cod_Contatto = ContattixIndirizzi.Cod_Contatto And ContattixIndirizzi.Tipo_Indirizzo = 101 ")
            StbSQL.AppendLine(" LEFT JOIN Indirizzi Indirizzo_Contatto ON ContattixIndirizzi.Cod_Indirizzo = Indirizzo_Contatto.cod_indirizzo ")
            StbSQL.AppendLine(" LEFT JOIN ISTAT ISTAT_Contatto ON Indirizzo_Contatto.pro_cod_istat = ISTAT_Contatto.PROV AND Indirizzo_Contatto.com_cod_istat = ISTAT_Contatto.COM ")
            StbSQL.AppendLine(" INNER JOIN Imprese ON Imprese.Piva = Contatti.Piva ")
            StbSQL.AppendLine(" LEFT JOIN ImpreseXIndirizzi ON Contatti.Cod_Contatto = ImpreseXIndirizzi.Piva And ImpreseXIndirizzi.Tipo_Indirizzo = 1 ")
            StbSQL.AppendLine(" LEFT JOIN Indirizzi Indirizzo_Impresa ON ImpreseXIndirizzi.Cod_Indirizzo = Indirizzo_Impresa.cod_indirizzo ")
            StbSQL.AppendLine(" LEFT JOIN ISTAT ISTAT_Impresa ON Indirizzo_Impresa.pro_cod_istat = ISTAT_Impresa.PROV AND Indirizzo_Impresa.com_cod_istat = ISTAT_Impresa.COM ")
            StbSQL.AppendLine(" LEFT JOIN EUDR_Classificazione_Paesi ON EUDR_Classificazione_Paesi.Codice = ISNULL(Indirizzo_Contatto.stato, Indirizzo_Impresa.stato) ")

            If Filtro_Visibilita_Utente Then
                StbSQL.AppendLine(" LEFT JOIN Utenti_Visibilita_Appoggio (NOLOCK) On Imprese.Piva = Utenti_Visibilita_Appoggio.Piva AND Utenti_Visibilita_Appoggio.Entita_Cod=1  ")
            End If

            StbSQL.AppendLine(" WHERE Contatti.EUDR = 1 ")
            StbSQL.AppendLine(" AND Contatti.Id_CF <> 0 ")

            If Piva <> "" Then
                StbSQL.AppendLine(" AND Contatti.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
                If Not Filtro_Pubblici Then
                    StbSQL.AppendLine(" AND Contatti.Sa_Cod = 0 ")
                End If
            ElseIf Filtro_Pubblici Then
                StbSQL.AppendLine(" AND Contatti.Sa_Cod = -1 ")
            End If

            If Cod_Contatto <> "" Then
                StbSQL.AppendLine(" AND Contatti.Cod_Contatto = '" & Agro_SQL_SaveText(Cod_Contatto) & "' ")
            End If

            If Cod_Fornitore <> "" Then
                StbSQL.AppendLine(" AND Risorse_Umane.Settore_Des = '" & Agro_SQL_SaveText(Cod_Fornitore) & "' ")
            End If

            If Filtro_Visibilita_Utente Then
                StbSQL.AppendLine(" AND Utenti_Visibilita_Appoggio.Username = " & Agro_SQL_SaveText_NULL(objParametri.UtenteUsername) & " ")
            End If

            StbSQL.AppendLine(" ORDER BY Contatti.Rag_Soc + Contatti.Cognome + ' ' + Contatti.Nome ")

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StbSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function

    '###############################################################################
    Public Function VerificaEsistenza_CodContatto(ByVal Piva As String,
                                                  ByVal Cod_Contatto As String,
                                                  ByRef objParametri_Server As AgronicaCoreParametri) As Boolean


        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Contatti_R.VerificaEsistenza_CodContatto()"

        Dim DT_Contatti As DataTable


        DT_Contatti = Contatti_Contatto_Leggi(Piva,
                                            Cod_Contatto,
                                            0,
                                            0,
                                            False,
                                            False,
                                            0,
                                            0,
                                                False,
                                                0,
                                            ID_CF_NOFILTRO,
                                            0,
                                                        "",
                                            True,
                                                0,
                                                0,
                                                0,
                                                0,
                                                0,
                                                AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                "",
                                                "",
                                                objParametri_Server)


        'Verifico se il recordset e' aperto
        If DT_Contatti.Rows.Count <> 0 Then

            Return True

        Else

            Return False

        End If


    End Function


    Public Function Contatti_Imprese_Leggi(ByVal SuperUser_CodFiscale As String,
                                            ByVal xFiltroAggiuntivo As String,
                                            ByVal xOrderBy As String,
                                            ByRef objParametri As AgronicaCoreParametri) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Contatti_R.Contatti_Imprese_Leggi()"


        Dim StrSQL As New StringBuilder
        Dim MessaggioErrore As String = ""
        Dim DT As New DataTable

        Try

            '----------------------------------------------------
            '--- Preparo la Query SQL ---------------------------
            '----------------------------------------------------

            'SERVE IL DISTINCT, PERCHE' ALTRIMENTI UN'IMPRESA COMPARE
            'TANTE VOLTE, QUANTE SONO I RAPPORTI CONTABILI DEL CONTATTO

            StrSQL.Length = 0
            StrSQL.Append(" SELECT DISTINCT Imprese.*, UtentiXImprese.[USER] ")

            StrSQL.Append(" FROM    Contatti ")
            StrSQL.Append("         INNER JOIN Imprese ON Imprese.Piva = Contatti.Piva ")
            StrSQL.Append("         INNER JOIN Risorse_Umane ON Contatti.Cod_Contatto = Risorse_Umane.Cod_Contatto AND Contatti.Piva = Risorse_Umane.Piva ")
            StrSQL.Append("         INNER JOIN UtentiXImprese ON Imprese.PIVA = UtentiXImprese.PIVA ")

            StrSQL.Append(" WHERE   (UtentiXImprese.[USER] = '" & Agro_SQL_SaveText(SuperUser_CodFiscale) & "')   ")
            StrSQL.Append("         AND   Risorse_Umane.Cod_RisUm_Origine = 0   ")

            '----------------------------------------------------------------
            '--- Filtro associato all'utente 
            '----------------------------------------------------------------

            Dim objUtentiVisibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
            Dim UtenteProfiloImpreseSql As New List(Of String)()
            Dim DtImpreseVisibili As DataTable

            DtImpreseVisibili = objUtentiVisibilita.Leggi(enum_TipoEntita.Impresa, "", "", objParametri)
            If Not IsNothing(DtImpreseVisibili) Then
                For i As Integer = 0 To DtImpreseVisibili.Rows.Count - 1
                    UtenteProfiloImpreseSql.Add("'" & DtImpreseVisibili.Rows(i).Item("Piva").ToString() & "'")
                Next
                If Not IsNothing(UtenteProfiloImpreseSql) AndAlso UtenteProfiloImpreseSql.Count > 0 Then
                    StrSQL.AppendLine(" AND Contatti.Piva IN (" & Agro_SQL_Save_Clausola_IN(String.Join(",", UtenteProfiloImpreseSql.ToArray()), True) & ") ")
                End If
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY Imprese.rag_soc  ")
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function

    Public Function Leggi_Contatti_Pubblici(ByRef objParametri As AgronicaCoreParametri, Optional ByVal rapportiContabili As String = "") As DataTable
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Contatti_R.Leggi_Contatti_Pubblici()"

        '====================================================================================
        'Parametri opzionali :
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.AppendLine(" SELECT DISTINCT a.Cod_Contatto, a.Rag_Soc + a.Cognome + ' ' + a.Nome AS Contatto_Des ")
            StrSQL.AppendLine(" FROM contatti a ")
            StrSQL.AppendLine(" INNER JOIN risorse_umane b ON a.piva=b.piva AND a.cod_contatto = b.cod_contatto ")
            StrSQL.AppendLine(" WHERE a.sa_cod = -1 ")
            If Not String.IsNullOrEmpty(rapportiContabili) Then
                StrSQL.AppendLine(" AND b.cod_rapporto IN ( " & Agro_SQL_Save_Clausola_IN(rapportiContabili) & ") ")
            End If
            StrSQL.AppendLine(" ORDER BY a.Rag_Soc + a.Cognome + ' ' + a.Nome ")

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function

    Public Function Leggi_Contatti_Organismo_Controllo_Anagrafica(ByRef objParametri As AgronicaCoreParametri
                                            ) As DataTable
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Reg_Impianti_Read.Leggi_Contatti_Organismo_Controllo_Anagrafica()"

        '====================================================================================
        'Parametri opzionali :
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.AppendLine(" Select a.Cod_Contatto, a.Rag_Soc ")
            StrSQL.AppendLine(" From contatti a inner Join risorse_umane b on a.piva=b.piva And a.cod_contatto = b.cod_contatto ")
            StrSQL.AppendLine(" Where a.sa_cod = -1 And b.cod_rapporto = -28 ")
            StrSQL.AppendLine(" Order By a.Rag_Soc ")


            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function

    Public Function Leggi_Fornitori(ByVal Piva As String,
                                    ByRef objParametri As AgronicaCoreParametri) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Contatti_R.Leggi_Fornitori()"


        Dim StrSQL As New StringBuilder
        Dim MessaggioErrore As String = ""
        Dim DT As New DataTable

        Try

            '----------------------------------------------------
            '--- Preparo la Query SQL ---------------------------
            '----------------------------------------------------


            StrSQL.Length = 0
            StrSQL.Append(" SELECT * ")

            StrSQL.Append(" FROM    Contatti_Codici ")

            StrSQL.Append(" WHERE   (PIVA = '" & Agro_SQL_SaveText(Piva) & "')   ")


            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function

    Public Function LeggiContattiStazioniMeteo(ByVal Piva As String,
                                               ByRef objParametri As AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Contatti_R.LeggiContattiStazioniMeteo()"


        Dim StrSQL As New StringBuilder
        Dim MessaggioErrore As String = ""
        Dim DT As New DataTable

        Try

            '----------------------------------------------------
            '--- Preparo la Query SQL ---------------------------
            '----------------------------------------------------


            StrSQL.Length = 0
            StrSQL.Append(" SELECT c.*, r.Cod_Rapporto, r.Cod_RisUm ")

            StrSQL.Append(" FROM Contatti c ")
            StrSQL.Append(" INNER JOIN Risorse_Umane r ON (c.Cod_Contatto = r.Cod_Contatto AND c.Piva = r.Piva) ")
            StrSQL.Append(" INNER JOIN Rapporti_Contabili rc ON (r.Cod_Rapporto = rc.Cod_Rapporto) ")

            StrSQL.Append(" WHERE 1 = 1 ")

            StrSQL.Append(" AND rc.Cod_Rapporto = " & COD_FORNITORE & " ")
            StrSQL.Append(" AND c.Sa_Cod <> -1 ")

            StrSQL.Append(" AND ( r.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ) " & vbCrLf)
            StrSQL.Append(" AND ( r.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ) " & vbCrLf)

            If Piva <> "" Then
                StrSQL.Append(" AND (c.Piva = '" & Agro_SQL_SaveText(Piva) & "') ")
            End If

            StrSQL.Append(" ORDER BY Rag_Soc ")

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function

    Public Function Check_Impresa_Associata(
                                          ByVal username As String,
                                          ByRef objParametri_Utente As AgronicaCoreParametri,
                                          ByRef objParametri_Server As AgronicaCoreParametri,
                                          ByVal typeOperatore As enum_TipoOperatoreVisita
                                          ) As Boolean

        Dim MessaggioErrore As String = ""
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Contatti_R.Check_Impresa_Associata()"

        Dim bRet As Boolean = False
        Dim strsql As New StringBuilder
        Dim DT As DataTable

        Try

            'join tra imprese e pive per tirar fuori la ragione sociale
            strsql.Length = 0
            strsql.Append(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;")
            strsql.Append(" SELECT DISTINCT I.PIVA, I.rag_soc ")
            strsql.Append(" FROM  Imprese I ")

            strsql.Append(" inner join Utenti_Visibilita_Appoggio U (NOLOCK) ")
            strsql.Append(" on I.Piva = U.Piva ")

            strsql.Append(" WHERE 1=1 ")

            strsql.Append(" AND U.PivaSuperUser='" & Agro_SQL_SaveText(objParametri_Utente.PivaSuperUser) & "'")
            strsql.Append(" AND U.Username = '" & Agro_SQL_SaveText(username) & "' ")
            strsql.Append(" AND (U.Entita_Cod = 1) ")

            strsql.Append(" AND I.Piva NOT IN (select CS.Valore from Configurazione_Siti CS where CS.Chiave = 'Azienda_Timesheet_Tecnici' and CS.PivaSuperUser = '" & Agro_SQL_SaveText(objParametri_Utente.PivaSuperUser) & "') ")

            DT = EseguiQuery_Lettura(objParametri_Server, strsql.ToString, "")

            If DT IsNot Nothing Then
                If DT.Rows.Count > 0 Then
                    bRet = True
                Else
                    bRet = False
                End If
            End If

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return bRet

    End Function

End Class



'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################



Public Class Contatti_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Scrivi(ByVal Piva As String,
                           ByVal Sa_Cod As Integer,
                           ByVal Cod_Contatto As String,
                           ByVal Id_CF As Integer,
                           ByVal Rag_Soc As String,
                           ByVal Codice_Fiscale As String,
                           ByVal Convenevoli As String,
                           ByVal Tipo_Indirizzo_Default As Integer,
                           ByVal Nome As String,
                           ByVal Cognome As String,
                           ByVal Data_Nascita As Date,
                           ByVal Sesso As String,
                           ByVal Cod_Contatto_Referente As String,
                           ByVal Validita_Inizio As Date,
                           ByVal Validita_Fine As Date,
                           ByRef objParametri As AgronicaCoreParametri,
                           Optional ByVal Data_creazione As Date = #2/1/1900#,
                           Optional ByVal Data_modifica As Date = #2/1/1900#,
                           Optional ByVal username_creazione As String = "",
                           Optional ByVal username_modifica As String = "",
                           Optional ByVal Tipo_Speditore As Integer = 0,
                           Optional ByVal Tipo_Destinazione As Integer = 0,
                           Optional ByVal Agente_Cod As Integer = 0,
                           Optional ByVal Provvigione As Decimal = 0,
                           Optional ByVal Note As String = "",
                           Optional ByVal Id_Gestione_Note As Integer = 0,
                           Optional ByVal Note2 As String = "",
                           Optional ByVal Note_Operazioni As String = "",
                           Optional ByVal Note2_Operazioni As String = "",
                           Optional ByVal Cod_Risum_Destinazione_Diversa As Integer = 0,
                           Optional ByVal Tipo_Indirizzo_Default_Destinazione_Diversa As Integer = 0,
                           Optional ByVal Fido As Decimal = 0,
                           Optional ByVal Limite_Posizioni As Integer = 0,
                           Optional ByVal Limite_Giorni_Evasione As Decimal = 0,
                           Optional ByVal Orari_Ritiro As String = "",
                           Optional ByVal Filtro_Rimborsi As String = "",
                           Optional ByVal Vettore_Cod As Integer = 0,
                           Optional ByVal CapoArea_Cod As Integer = 0,
                           Optional ByVal Provvigione_CapoArea As Decimal = 0,
                           Optional ByVal Memo As String = "",
                           Optional ByVal Sconto_Contatto As Decimal = 0,
                           Optional ByVal Sconto_Testo As String = "",
                           Optional ByVal Modalita_Fatturazione As Integer = 0,
                           Optional ByVal Cod_Iva_Contatto As Integer = -1,
                           Optional ByVal Documento_Fatturazione As Integer = 0,
                           Optional ByVal nrBadge As String = "",
                           Optional ByVal ChkFittizio As Integer? = Nothing,
                           Optional ByVal Cod_Conto_Economico_Default As Integer = 0,
                           Optional ByVal Cod_Conto_Patrimoniale_Default As Integer = 0,
                           Optional ByVal Nome_Breve As String = Nothing,
                           Optional ByVal EUDR As Integer? = Nothing
                           ) As Boolean


        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.Contatti_W.Scrivi()"

        '====================================================================================
        'Parametri opzionali :
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        If Data_creazione = #2/1/1900# Then
            Data_creazione = Date.Now
        End If

        If Data_modifica = #2/1/1900# Then
            Data_modifica = Date.Now
        End If

        If username_creazione = "" Then
            username_creazione = objParametri.UsernameOperazione
        End If

        If username_modifica = "" Then
            username_modifica = objParametri.UsernameOperazione
        End If

        If Not AgronicaCoreDataProvider.UtilityProvider.PivaValida(Piva) Then
            Throw New Exception("Rilevato carattere non valido nella PIVA:" & Piva)
        End If

        If Not AgronicaCoreDataProvider.UtilityProvider.PivaValida(Cod_Contatto) Then
            Throw New Exception("Rilevato carattere non valido nel codice contatto:" & Cod_Contatto)
        End If

        Try


            '---------------------------------------------
            strSql.Length = 0
            strSql.AppendLine(" INSERT INTO Contatti ")
            strSql.AppendLine("                    (Piva,   Sa_Cod,  Cod_Contatto,  Id_CF,   Rag_Soc,       ")
            strSql.AppendLine("                     Codice_Fiscale,  Convenevoli,   Tipo_Indirizzo_Default, ")
            strSql.AppendLine("                     Nome,  Cognome,  Data_Nascita,  Sesso, Cod_Contatto_Referente, ")

            strSql.AppendLine("                     Tipo_Speditore, Tipo_Destinazione, Agente_Cod, Provvigione, ")
            strSql.AppendLine("                     Note, Id_Gestione_Note, Note2, Note_Operazioni, Note2_Operazioni, ")
            strSql.AppendLine("                     Cod_Risum_Destinazione_Diversa, Tipo_Indirizzo_Default_Destinazione_Diversa, ")
            strSql.AppendLine("                     Fido, Limite_Posizioni, Limite_Giorni_Evasione, Orari_Ritiro, Filtro_Rimborsi, ")
            strSql.AppendLine("                     Vettore_Cod, CapoArea_Cod, Provvigione_CapoArea, ")

            strSql.AppendLine("                     Memo, Sconto_Contatto, Sconto_Testo, ")
            strSql.AppendLine("                     Modalita_Fatturazione, Cod_Iva_Contatto, Cod_Conto_Econ, Cod_Conto_Pat, Documento_Fatturazione, ")

            strSql.AppendLine("                     Inviato,            DataInvio, ")
            strSql.AppendLine("                     Data_Creazione,     Data_Modifica, ")
            strSql.AppendLine("                     UserName_Creazione, UserName_Modifica, ")
            strSql.AppendLine("                     Validita_Inizio,    Validita_Fine, ")

            If ChkFittizio IsNot Nothing Then
                strSql.AppendLine("                 ChkFittizio, ")
            End If

            If Nome_Breve IsNot Nothing Then
                strSql.AppendLine("                 Nome_Breve, ")
            End If

            If EUDR IsNot Nothing Then
                strSql.AppendLine("                 EUDR, ")
            End If

            strSql.AppendLine("                     nrBadge ")
            strSql.AppendLine("                     ) ")

            strSql.AppendLine(" VALUES (")
            strSql.AppendLine("          '" & Agro_SQL_SaveText(Piva) & "' ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Sa_Cod))
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(Cod_Contatto) & "' ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Id_CF))
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(Rag_Soc) & "' ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(Codice_Fiscale) & "' ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(Convenevoli) & "' ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Tipo_Indirizzo_Default))
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(Nome) & "' ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(Cognome) & "' ")
            strSql.AppendLine("         , " & Agro_SQL_SaveDate(Data_Nascita))
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(Sesso) & "' ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(Cod_Contatto_Referente) & "' ")

            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Tipo_Speditore))
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Tipo_Destinazione))
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Agente_Cod))
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Provvigione))
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(Note) & "' ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Id_Gestione_Note))
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(Note2) & "' ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(Note_Operazioni) & "' ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(Note2_Operazioni) & "' ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Cod_Risum_Destinazione_Diversa))
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Tipo_Indirizzo_Default_Destinazione_Diversa))
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Fido))
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Limite_Posizioni))
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Limite_Giorni_Evasione))
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(Orari_Ritiro) & "' ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(Filtro_Rimborsi) & "' ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Vettore_Cod))
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(CapoArea_Cod))
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Provvigione_CapoArea))
            strSql.AppendLine("         , '" & Agro_SQL_SaveText(Memo) & "' ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Sconto_Contatto))
            strSql.AppendLine("         , '" & Agro_SQL_SaveText(Sconto_Testo) & "' ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Modalita_Fatturazione))
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Cod_Iva_Contatto))
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Cod_Conto_Economico_Default))
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Cod_Conto_Patrimoniale_Default))
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Documento_Fatturazione))


            strSql.AppendLine("         , 0  ")
            strSql.AppendLine("         , Null  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveDateTime(Data_creazione) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveDateTime(Data_modifica) & "  ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(username_modifica) & "' ")
            strSql.AppendLine("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")

            If ChkFittizio IsNot Nothing Then
                strSql.AppendLine("     , " & Agro_SQL_SaveNum(ChkFittizio) & "  ")
            End If

            If Nome_Breve IsNot Nothing Then
                strSql.AppendLine("         , '" & Agro_SQL_SaveText(Nome_Breve) & "' ")
            End If

            If EUDR IsNot Nothing Then
                strSql.AppendLine("     , " & Agro_SQL_SaveNum(EUDR) & "  ")
            End If

            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(nrBadge) & "' ")
            strSql.AppendLine(" )")


            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function



    '############################################################################
    '############################################################################
    '############################################################################

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="Piva"></param>
    ''' <param name="Cod_Contatto"></param>
    ''' <param name="Campo"></param>
    ''' <param name="Valore"></param>
    ''' <param name="Validita_Inizio"></param>
    ''' <param name="Validita_Fine"></param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="objParametri"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[pierantoni]	07/02/2011	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Function Modifica_Parametrizzata(
                                           ByVal Piva As String,
                                           ByVal Cod_Contatto As String,
                                           ByVal Campo As String,
                                           ByVal Valore As Object,
                                           ByVal Validita_Inizio As Date,
                                           ByVal Validita_Fine As Date,
                                                ByVal xFiltroAggiuntivo As String,
                                                ByRef objParametri As AgronicaCoreParametri
                                                ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Contatti_W.Modifica_Parametrizzata()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New StringBuilder
        Dim xRisp As Boolean = False
        Dim strAssegnamento As String = String.Empty

        ' a seconda del tipo del valore che devo aggiornare, formatto la query
        Dim Stringa As Type = GetType(System.String)
        Dim Data As Type = GetType(System.DateTime)
        Dim Intero32 As Type = GetType(System.Int32)

        Try
            If Piva = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva obbligatoria)")
            End If

            If Cod_Contatto = "" Then
                Throw New Exception("Parametro non corretto nella query (Cod_Contatto obbligatorio)")
            End If

            '---------------------------------------------

            Dim TypeVal As Type = Valore.GetType()

            If TypeVal.Equals(Stringa) Then

                strAssegnamento = Campo & "= '" & Agro_SQL_SaveText(Valore.ToString) & "' "

            ElseIf TypeVal.Equals(Data) Then

                strAssegnamento = Campo & "= " & Agro_SQL_SaveDate(Valore.ToString) & " "

            Else

                strAssegnamento = Campo & "= " & Agro_SQL_SaveNum(Valore.ToString) & " "

            End If


            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append("UPDATE Contatti SET ")
            StrSQL.Append(strAssegnamento)
            StrSQL.Append("   ,Inviato           =  0 ")
            StrSQL.Append("   ,DataInvio         =  Null ")
            StrSQL.Append("   ,Data_Modifica     =  " & Agro_SQL_SaveDateTime(Date.Now))
            StrSQL.Append("   ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.Append("   ,Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Inizio))
            StrSQL.Append("   ,Validita_Fine     =  " & Agro_SQL_SaveDate(Validita_Fine))

            StrSQL.Append(" WHERE Piva = '" & Agro_SQL_SaveText(Piva) & "'")
            StrSQL.Append(" AND   Cod_contatto = '" & Agro_SQL_SaveText(Cod_Contatto) & "' ")

            '---------------------------------------------

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp

    End Function

    Public Function Modifica(ByVal Piva As String,
                                 ByVal New_Sa_Cod As Int32,
                                 ByVal Cod_Contatto As String,
                                 ByVal Id_CF As Integer,
                                 ByVal Rag_Soc As String,
                                 ByVal Codice_Fiscale As String,
                                 ByVal Convenevoli As String,
                                 ByVal Tipo_Indirizzo_Default As Integer,
                                 ByVal Nome As String,
                                 ByVal Cognome As String,
                                 ByVal Data_Nascita As Date,
                                 ByVal Sesso As String,
                                 ByVal Cod_Contatto_Referente As String,
                                 ByVal Validita_Inizio As Date,
                                 ByVal Validita_Fine As Date,
                                 ByVal nrBadge As String,
                                 ByVal xFiltroAggiuntivo As String,
                                 ByRef objParametri As AgronicaCoreParametri,
                                   Optional ByVal username_modifica As String = Nothing,
                                   Optional ByVal Tipo_Speditore As Integer? = Nothing,
                                   Optional ByVal Tipo_Destinazione As Integer? = Nothing,
                                   Optional ByVal Agente_Cod As Integer? = Nothing,
                                   Optional ByVal Provvigione As Decimal? = Nothing,
                                   Optional ByVal Note As String = Nothing,
                                   Optional ByVal Id_Gestione_Note As Integer? = Nothing,
                                   Optional ByVal Note2 As String = Nothing,
                                   Optional ByVal Note_Operazioni As String = Nothing,
                                   Optional ByVal Note2_Operazioni As String = Nothing,
                                   Optional ByVal Cod_Risum_Destinazione_Diversa As Integer? = Nothing,
                                   Optional ByVal Tipo_Indirizzo_Default_Destinazione_Diversa As Integer? = Nothing,
                                   Optional ByVal Fido As Decimal? = Nothing,
                                   Optional ByVal Limite_Posizioni As Integer? = Nothing,
                                   Optional ByVal Limite_Giorni_Evasione As Decimal? = Nothing,
                                   Optional ByVal Orari_Ritiro As String = Nothing,
                                   Optional ByVal Filtro_Rimborsi As String = Nothing,
                                   Optional ByVal Vettore_Cod As Integer? = Nothing,
                                   Optional ByVal CapoArea_Cod As Integer? = Nothing,
                                   Optional ByVal Provvigione_CapoArea As Decimal? = Nothing,
                                   Optional ByVal Memo As String = Nothing,
                                   Optional ByVal Sconto_Contatto As Decimal? = Nothing,
                                   Optional ByVal Sconto_Testo As String = Nothing,
                                   Optional ByVal Modalita_Fatturazione As Integer? = Nothing,
                                   Optional ByVal Cod_Iva_Contatto As Integer? = Nothing,
                                   Optional ByVal Documento_Fatturazione As Integer? = Nothing,
                                   Optional ByVal ChkFittizio As Integer? = Nothing,
                                   Optional ByVal Cod_Conto_Economico_Default As Integer? = Nothing,
                                   Optional ByVal Cod_Conto_Patrimoniale_Default As Integer? = Nothing,
                                   Optional ByVal Nome_Breve As String = Nothing,
                                   Optional ByVal EUDR As Integer? = Nothing
                                ) As Boolean

        Dim NomeRoutine As String = "AnagrafeCoreAnagrafeDAL.Contatti_W.Modifica()"

        '====================================================================================
        'Parametri opzionali :
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New StringBuilder
        Dim xRisp As Boolean = False

        Try
            If Piva = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva obbligatoria)")
            End If

            If Cod_Contatto = "" Then
                Throw New Exception("Parametro non corretto nella query (Cod_Contatto obbligatorio)")
            End If

            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append("UPDATE Contatti SET ")
            StrSQL.Append("    Sa_Cod                  = " & Agro_SQL_SaveNum(New_Sa_Cod) & "")
            StrSQL.Append("   ,Rag_Soc                 = '" & Agro_SQL_SaveText(Rag_Soc) & "'  ")
            StrSQL.Append("   ,Id_CF                   = " & Agro_SQL_SaveNum(Id_CF) & "  ")
            StrSQL.Append("   ,Codice_Fiscale          = '" & Agro_SQL_SaveText(Codice_Fiscale) & "'  ")
            StrSQL.Append("   ,Convenevoli             = '" & Agro_SQL_SaveText(Convenevoli) & "'  ")
            StrSQL.Append("   ,Tipo_Indirizzo_Default  = " & Agro_SQL_SaveNum(Tipo_Indirizzo_Default) & "  ")
            StrSQL.Append("   ,Nome                    = '" & Agro_SQL_SaveText(Nome) & "'  ")
            StrSQL.Append("   ,Cognome                 = '" & Agro_SQL_SaveText(Cognome) & "'  ")
            StrSQL.Append("   ,Data_Nascita            =  " & Agro_SQL_SaveDate(Data_Nascita) & "  ")
            StrSQL.Append("   ,Sesso                   = '" & Agro_SQL_SaveText(Sesso) & "'  ")
            StrSQL.Append("   ,Cod_Contatto_Referente  = '" & Agro_SQL_SaveText(Cod_Contatto_Referente) & "'  ")

            StrSQL.Append("   ,Inviato           =  0 ")
            StrSQL.Append("   ,DataInvio         =  Null ")
            StrSQL.Append("   ,Data_Modifica     =  " & Agro_SQL_SaveDateTime(Date.Now))
            StrSQL.Append("   ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.Append("   ,Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Inizio))
            StrSQL.Append("   ,Validita_Fine     =  " & Agro_SQL_SaveDate(Validita_Fine))
            StrSQL.Append("   ,nrBadge     =  '" & Agro_SQL_SaveText(nrBadge) & "'")

            If Memo IsNot Nothing Then
                StrSQL.Append(" ,Memo = '" & Agro_SQL_SaveText(Memo) & "'")
            End If
            If Tipo_Destinazione IsNot Nothing Then
                StrSQL.Append(" ,Tipo_Destinazione = " & Agro_SQL_SaveNum(Tipo_Destinazione))
            End If
            If Note IsNot Nothing Then
                StrSQL.Append(" ,Note = '" & Agro_SQL_SaveText(Note) & "'")
            End If
            If Note2 IsNot Nothing Then
                StrSQL.Append(" ,Note2 = '" & Agro_SQL_SaveText(Note2) & "'")
            End If
            If Note_Operazioni IsNot Nothing Then
                StrSQL.Append(" ,Note_Operazioni = '" & Agro_SQL_SaveText(Note_Operazioni) & "'")
            End If
            If Note2_Operazioni IsNot Nothing Then
                StrSQL.Append(" ,Note2_Operazioni = '" & Agro_SQL_SaveText(Note2_Operazioni) & "'")
            End If
            If Tipo_Speditore IsNot Nothing Then
                StrSQL.Append(" ,Tipo_Speditore = " & Agro_SQL_SaveNum(Tipo_Speditore))
            End If

            If Sconto_Testo IsNot Nothing Then
                StrSQL.Append(" ,Sconto_Testo = '" & Agro_SQL_SaveText(Sconto_Testo) & "'")
            End If
            If Agente_Cod IsNot Nothing Then
                StrSQL.Append(" ,Agente_Cod = " & Agro_SQL_SaveNum(Agente_Cod))
            End If
            If CapoArea_Cod IsNot Nothing Then
                StrSQL.Append(" ,CapoArea_Cod = " & Agro_SQL_SaveNum(CapoArea_Cod))
            End If
            If Vettore_Cod IsNot Nothing Then
                StrSQL.Append(" ,Vettore_Cod = " & Agro_SQL_SaveNum(Vettore_Cod))
            End If
            If Modalita_Fatturazione IsNot Nothing Then
                StrSQL.Append(" ,Modalita_Fatturazione = " & Agro_SQL_SaveNum(Modalita_Fatturazione))
            End If
            If Cod_Iva_Contatto IsNot Nothing Then
                StrSQL.Append(" ,Cod_Iva_Contatto = " & Agro_SQL_SaveNum(Cod_Iva_Contatto))
            End If
            If Cod_Conto_Economico_Default IsNot Nothing Then
                StrSQL.Append(" ,Cod_Conto_Econ = " & Agro_SQL_SaveNum(Cod_Conto_Economico_Default))
            End If
            If Cod_Conto_Patrimoniale_Default IsNot Nothing Then
                StrSQL.Append(" ,Cod_Conto_Pat = " & Agro_SQL_SaveNum(Cod_Conto_Patrimoniale_Default))
            End If
            If Provvigione IsNot Nothing Then
                StrSQL.Append(" ,Provvigione = " & Agro_SQL_SaveNum(Provvigione))
            End If
            If Provvigione_CapoArea IsNot Nothing Then
                StrSQL.Append(" ,Provvigione_CapoArea = " & Agro_SQL_SaveNum(Provvigione_CapoArea))
            End If
            If Cod_Risum_Destinazione_Diversa IsNot Nothing Then
                StrSQL.Append(" ,Cod_Risum_Destinazione_Diversa = " & Agro_SQL_SaveNum(Cod_Risum_Destinazione_Diversa))
            End If
            If Tipo_Indirizzo_Default_Destinazione_Diversa IsNot Nothing Then
                StrSQL.Append(" ,Tipo_Indirizzo_Default_Destinazione_Diversa = " & Agro_SQL_SaveNum(Tipo_Indirizzo_Default_Destinazione_Diversa))
            End If
            If ChkFittizio IsNot Nothing Then
                StrSQL.Append(" ,ChkFittizio = " & Agro_SQL_SaveNum(ChkFittizio))
            End If
            If Nome_Breve IsNot Nothing Then
                StrSQL.Append(" ,Nome_Breve = '" & Agro_SQL_SaveText(Nome_Breve) & "'")
            End If
            If EUDR IsNot Nothing Then
                StrSQL.Append(" ,EUDR = " & Agro_SQL_SaveNum(EUDR))
            End If

            StrSQL.Append(" WHERE Piva = '" & Agro_SQL_SaveText(Piva) & "'")
            StrSQL.Append(" AND   Cod_contatto = '" & Agro_SQL_SaveText(Cod_Contatto) & "' ")

            '---------------------------------------------

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp

    End Function



    Public Function Cancella(ByVal Piva As String, _
                                 ByVal Cod_Contatto As String, _
                                    ByVal xFiltroAggiuntivo As String, _
                                    ByRef objParametri As AgronicaCoreParametri _
                                    ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Contatti_W.Cancella()"

        '====================================================================================
        'Parametri opzionali :
        '   Cod_Contatto
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            If Piva = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva obbligatoria)")
            End If

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = AgronicaCoreDataProvider.AgronicaCoreParametri.enumCancellazioneLogica.CancellazioneLogica Then

                StrSQL.Length = 0
                StrSQL.Append(" UPDATE Contatti ")
                StrSQL.Append(" SET ")
                StrSQL.Append("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("      ,Inviato = -1 ")
                StrSQL.Append(" WHERE  Inviato >= 0 ")

            Else

                StrSQL.Length = 0
                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM     Contatti ")
                StrSQL.Append(" WHERE Inviato = 0")

            End If

            StrSQL.Append(" AND  Piva = '" & Agro_SQL_SaveText(Trim(Piva)) & "'  ")

            If Cod_Contatto <> "" Then
                StrSQL.Append(" AND   Cod_Contatto = '" & Agro_SQL_SaveText(Trim(Cod_Contatto)) & "'  ")
            End If



            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp

    End Function

    Public Function Aggiorna_Contatti(ByVal EFArrayToInsert As ArrayList,
                                      ByVal EFArrayToUpdate As ArrayList,
                                      ByVal EFArrayToDelete As ArrayList,
                                      ByRef objParametri As AgronicaCoreParametri
                                      ) As String

        Dim retries As Integer = 3
        Dim success As Boolean = True

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Contatti.Aggiorna_Contatti()"
        Dim MessaggioErrore As String = ""

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Try

            Using scope As New TransactionScope()

                Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

                    For Each curContatto As Contatti In EFArrayToInsert

                        success = False
                        For i As Integer = 0 To retries - 1

                            Try
                                GiasContext.Contatti.Add(curContatto)
                                GiasContext.SaveChanges()
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
                        For Each listContatti As Contatti In EFArrayToUpdate
                            GiasContext.Contatti.Attach(listContatti)
                            GiasContext.Entry(listContatti).State = EntityState.Modified
                            GiasContext.SaveChanges()
                        Next

                        For Each listContatti As Parco_Macchine In EFArrayToDelete
                            GiasContext.Parco_Macchine.Attach(listContatti)
                            GiasContext.Parco_Macchine.Remove(listContatti)

                            GiasContext.SaveChanges()
                        Next

                        ' COMMIT Effettivo
                        scope.Complete()
                    End If

                End Using
            End Using


        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore, ex)
        End Try

        Return MessaggioErrore

    End Function

    ''' <summary>
    ''' Crea un codice contatto estero con prime due lettere stato e a seguire contatore numerico con padding 11 zeri
    ''' </summary>
    Public Shared Function Genera_Cod_Contatto_Estero(ByVal stato As String,
                                                      ByVal codContatto As String,
                                                      ByRef ultimoContatoreEstero As Integer
                                                      ) As String

        If (codContatto).Trim <> "" Then
            Return codContatto
        Else
            ultimoContatoreEstero += 1
            codContatto = Left(stato, 2) & Right("00000000000" & ultimoContatoreEstero, 11)
            Return codContatto
        End If

    End Function

    'default:
    'ID_CF = -99 (costante ID_CF_NOFILTRO)
    Public Function Leggi_Distinct_Contatti2(ByVal Piva As String,
                                                ByVal Cod_Contatto As String,
                                                ByVal Cod_RisUm As Integer,
                                                ByVal Cod_Rapporto As Integer,
                                                ByVal FlagPubblico As Boolean,
                                                ByVal ID_CF As Integer,
                                                ByVal Cod_RisUm_Origine As Integer,
                                                ByVal Piva_SuperUser_Origine As String,
                                                ByVal Progressivo As String,
                                                ByVal xFiltroAggiuntivo As String,
                                                    ByVal xOrderBy As String,
                                                    ByRef objParametri As AgronicaCoreParametri
                                                    ) As DataTable


        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Contatti_R.Contatti_Contatto_Leggi()"


        Dim StbSQL As New StringBuilder
        Dim MessaggioErrore As String = ""
        Dim DT As New DataTable
        Dim i As Integer = 0

        Try

            '----------------------------------------------------
            '--- Preparo la Query SQL ---------------------------
            '----------------------------------------------------
            StbSQL.Length = 0

            StbSQL.Append(" SELECT DISTINCT Imprese.Rag_Soc AS Impresa, Contatti.* " & vbCrLf)

            StbSQL.Append(" FROM    Contatti  " & vbCrLf)
            StbSQL.Append(" INNER JOIN Risorse_Umane ON Contatti.Cod_Contatto = Risorse_Umane.Cod_Contatto AND Contatti.Piva = Risorse_Umane.Piva " & vbCrLf)
            StbSQL.Append(" INNER JOIN  Rapporti_Contabili ON Risorse_Umane.Cod_Rapporto = Rapporti_Contabili.Cod_Rapporto " & vbCrLf)
            StbSQL.Append(" INNER JOIN UtentiXImprese ON Rapporti_Contabili.Piva = UtentiXImprese.[USER] AND Risorse_Umane.Piva = UtentiXImprese.PIVA " & vbCrLf)

            'JOIN IMPRESE 
            StbSQL.Append(" INNER JOIN Imprese ON Imprese.Piva = Contatti.Piva " & vbCrLf)

            If Cod_Contatto <> "" Then
                StbSQL.Append(" AND    (Contatti.Cod_Contatto = '" & Agro_SQL_SaveText(Cod_Contatto) & "')   " & vbCrLf)
            End If

            ' VISIBILITA
            If Not FlagPubblico Then

                If Piva <> "" Then

                    StbSQL.Append(" AND    ( ")

                    'visibilità centro
                    'leggo se ci sono filtri sui centri
                    Dim objUtentiVisibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
                    Dim FiltroCentri As String = ""
                    Dim DtCentriVisibili As DataTable
                    DtCentriVisibili = objUtentiVisibilita.Leggi(enum_TipoEntita.Centro, " Piva='" & Agro_SQL_SaveText(Piva) & "'", "", objParametri)
                    If DtCentriVisibili IsNot Nothing AndAlso DtCentriVisibili.Rows.Count > 0 Then
                        For i = 0 To DtCentriVisibili.Rows.Count - 1
                            FiltroCentri &= " (Contatti.Piva = '" & DtCentriVisibili.Rows(i).Item("piva") & "' AND Contatti.Sa_Cod = " & DtCentriVisibili.Rows(i).Item("sa_cod") & ") OR "
                        Next
                        If FiltroCentri <> "" Then
                            StbSQL.Append(" (" & Left(FiltroCentri, FiltroCentri.Length - 3) & " OR (Contatti.Piva = '" & Agro_SQL_SaveText(Piva) & "' AND Contatti.Sa_Cod = 0) ) ")
                        End If
                    End If

                    'aziendali
                    If FiltroCentri = "" Then
                        StbSQL.Append("    (Contatti.Piva = '" & Agro_SQL_SaveText(Piva) & "' AND Contatti.Sa_Cod <> -1 )  ")
                    End If

                    StbSQL.Append("        ) ")

                End If
            Else
                'pubblici + azienda
                If Piva <> "" Then

                    'pubbliche
                    StbSQL.Append(" AND    ( (Contatti.Sa_Cod = -1) ")

                    'visibilità centro
                    'leggo se ci sono filtri sui centri
                    Dim objUtentiVisibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
                    Dim FiltroCentri As String = ""
                    Dim DtCentriVisibili As DataTable
                    DtCentriVisibili = objUtentiVisibilita.Leggi(enum_TipoEntita.Centro, " Piva='" & Agro_SQL_SaveText(Piva) & "'", "", objParametri)
                    If DtCentriVisibili IsNot Nothing AndAlso DtCentriVisibili.Rows.Count > 0 Then
                        For i = 0 To DtCentriVisibili.Rows.Count - 1
                            FiltroCentri &= " (Contatti.Piva = '" & DtCentriVisibili.Rows(i).Item("piva") & "' AND Contatti.Sa_Cod = " & DtCentriVisibili.Rows(i).Item("sa_cod") & ") OR "
                        Next
                        If FiltroCentri <> "" Then
                            StbSQL.Append(" OR (" & Left(FiltroCentri, FiltroCentri.Length - 3) & " OR (Contatti.Piva = '" & Agro_SQL_SaveText(Piva) & "' AND Contatti.Sa_Cod = 0) ) ")
                        End If
                    End If

                    'aziendali
                    If FiltroCentri = "" Then
                        StbSQL.Append("          OR  (Contatti.Piva = '" & Agro_SQL_SaveText(Piva) & "')  ")
                    End If

                    StbSQL.Append("        ) ")

                Else
                    'tutti pubblici
                    StbSQL.Append(" AND    (Contatti.Sa_Cod = -1)  " & vbCrLf)
                End If

            End If

            If ID_CF <> ID_CF_NOFILTRO Then
                StbSQL.Append(" AND     (Contatti.ID_CF = " & Agro_SQL_SaveNum(ID_CF.ToString) & ")   " & vbCrLf)
            End If

            If Cod_RisUm <> 0 Then
                StbSQL.Append(" AND     (Cod_RisUm = " & Agro_SQL_SaveNum(Cod_RisUm.ToString) & ")   " & vbCrLf)
            End If

            If Progressivo <> "" Then
                StbSQL.Append(" AND  Risorse_Umane.Settore_Des = '" & Agro_SQL_SaveText(Progressivo) & "'   " & vbCrLf)
            End If

            If xFiltroAggiuntivo <> "" Then
                StbSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StbSQL.Append(" AND   Contatti.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StbSQL.Append(" AND   Contatti.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StbSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StbSQL.AppendLine(" ORDER BY Contatti.Rag_Soc ASC  ")
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StbSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT


    End Function

    Public Function AssociaUtente(ByVal Piva As String,
                                  ByVal Cod_Contatto As String,
                                  ByVal Username As String,
                                  ByRef objParametri As AgronicaCoreParametri
                                   ) As Boolean

        Dim NomeRoutine As String = "AnagrafeCoreAnagrafeDAL.Contatti_W.AssociaUtente()"

        '====================================================================================
        'Parametri opzionali :
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New StringBuilder
        Dim xRisp As Boolean = False

        Try
            If Cod_Contatto = "" Then
                Throw New Exception("Parametro non corretto nella query (Cod_Contatto obbligatorio)")
            End If

            If Username = "" Then
                'Cancellazione associazione contatto/utente
                StrSQL.Length = 0
                StrSQL.AppendLine(" DELETE FROM ContattiXUtentiGias ")
                StrSQL.AppendLine(" WHERE piva = '" & Agro_SQL_SaveText(Piva) & "' ")
                StrSQL.AppendLine(" AND   cod_contatto = '" & Agro_SQL_SaveText(Cod_Contatto) & "' ")
                '---------------------------------------------
            Else
                StrSQL.Length = 0
                StrSQL.AppendLine(" DECLARE @count AS INT ")
                'Controllo se esiste l'associazione per il contatto
                StrSQL.AppendLine(" SELECT  @count=COUNT(*) FROM ContattiXUtentiGias ")
                StrSQL.AppendLine(" WHERE piva = '" & Agro_SQL_SaveText(Piva) & "' ")
                StrSQL.AppendLine(" AND   cod_contatto = '" & Agro_SQL_SaveText(Cod_Contatto) & "' ")
                'Se non esiste lo inserisco
                StrSQL.AppendLine(" IF @count=0 BEGIN ")
                StrSQL.AppendLine(" INSERT INTO ContattiXUtentiGias (piva, cod_contatto, username, Data_Creazione, Data_Modifica)    " & vbCrLf)
                StrSQL.AppendLine(" VALUES ('" & Agro_SQL_SaveText(Piva) & "', '" & Agro_SQL_SaveText(Cod_Contatto) & "', '" & Agro_SQL_SaveText(Username) & "' , " & Agro_SQL_SaveDateTime(Date.Now) & ", " & Agro_SQL_SaveDateTime(Date.Now) & ")    ")
                StrSQL.AppendLine(" END ")
                'Se esiste lo aggiorno
                StrSQL.AppendLine(" IF @count!=0 BEGIN ")
                StrSQL.AppendLine(" UPDATE ContattiXUtentiGias Set username = '" & Agro_SQL_SaveText(Username) & "' ")
                StrSQL.AppendLine(" WHERE piva = '" & Agro_SQL_SaveText(Piva) & "' ")
                StrSQL.AppendLine(" AND   cod_contatto = '" & Agro_SQL_SaveText(Cod_Contatto) & "' ")
                StrSQL.AppendLine(" END ")
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp

    End Function

End Class





'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################



