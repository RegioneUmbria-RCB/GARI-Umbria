Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.AgronicaCoreParametri

Public Class DerrateRMA_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function Leggi_TabellaRMA(
        ByVal xFiltroAggiuntivo As String,
        ByVal xOrderBy As String,
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
    ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCore_DAL.Leggi()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim strSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            strSQL.Length = 0

            strSQL.Append(" SELECT * " + vbCrLf)
            strSQL.Append(" FROM TabellaRMA " + vbCrLf)
            strSQL.Append(" WHERE 1=1 " + vbCrLf)

            If xFiltroAggiuntivo <> "" Then
                strSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                strSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, strSQL.ToString, NomeRoutine)
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


'#################################################################
'#################################################################
'#################################################################

Public Class DerrateRMA_W
    Inherits AgronicaCoreDataProvider.DataProvider

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="objParametri"></param>
    ''' <returns></returns>
    ''' <remarks>nelle query che seguono si utilizza il campo "TabellaRMA_COD" in RegolamentiRMA per identificare nuovi regolamenti. Questo inizialmente vale NULL e viene poi imostato alla fine</remarks>
    Public Function FinalizzaImportazioneRMA_UE(
        ByVal FornitoreDati As enum_CAC_Codifica_PrincipiAttivi_Tipo_Codifica,
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
        ByRef MessaggioDiRitorno As String) As Boolean
        Dim NomeRoutine As String = "Scrivi()"

        '====================================================================================
        'Parametri opzionali :

        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim stb As New Text.StringBuilder
        Dim xRisp As Boolean = False


        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False


        Dim rval As String = ""

        Dim contatoreRecord As Integer

        Dim bNomeRoutine As String = "FinalizzaImportazioneRMA_UE - "

        Try



            'Apro la connessione e la transazione al DB
            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale,
                                                                            FlagTransazioneLocale,
                                                                            objParametri)

            '--------------------------------------------------------------------------
            ' Operazioni di riporto RMA da tabella popolata con file.
            '--------------------------------------------------------------------------        

            FinalizzaImportazioneRMA_UE_RiportoRMA(FornitoreDati, objParametri, MessaggioDiRitorno, NomeRoutine, xRisp, contatoreRecord, bNomeRoutine)


            'Commit transazione e Chiusura della connessione al DB
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri)
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''


            '--------------------------------------------------------------------------        


            ' VAnni: 17/9/2020: al momento l'elabrazione da normativa viene eseguita solo su forniture HOMOLOGA.
            If FornitoreDati = enum_CAC_Codifica_PrincipiAttivi_Tipo_Codifica.HOMOLOGA Then

                'Apro la connessione e la transazione al DB
                AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale,
                                                                                FlagTransazioneLocale,
                                                                                objParametri)

                '--------------------------------------------------------------------------
                ' Operazioni di riporto e completamento dei dati come da normativa (Homologa ed altri fornitori)
                '--------------------------------------------------------------------------

                FinalizzaImportazioneRMA_UE_RiportoCompletamentoDatiNormativa(FornitoreDati, objParametri, MessaggioDiRitorno, NomeRoutine, xRisp, contatoreRecord, bNomeRoutine)

                'Commit transazione e Chiusura della connessione al DB
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri)
                '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

            End If


            '--------------------------------------------------------------------------
            ' Operazioni finali
            '--------------------------------------------------------------------------

            FinalizzaImportazioneRMA_UE_OperazioniFinali(objParametri, MessaggioDiRitorno, NomeRoutine, xRisp, contatoreRecord, bNomeRoutine)


            NomeRoutine = bNomeRoutine & "Pulizia dei dati temporanei. "
            stb.Length = 0
            stb.Append("delete from RMA_Appoggio_Importazione  " & vbCrLf)

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_ScritturaNum(objParametri, stb.ToString, NomeRoutine, contatoreRecord)
            '--------------------------------------------------------------------------
            MessaggioDiRitorno &= NomeRoutine & ". Numero di righe elaborate: " & contatoreRecord & vbCrLf

           

        Catch ex As Exception

            'Faccio il rollback della transazione
            If Not objParametri.objTransazione Is Nothing Then
                'objParametri.objTransazione.Rollback()
                'objParametri.objTransazione = Nothing
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri)

            End If

            'pulisco ugualmente i file temporanei.
            NomeRoutine = bNomeRoutine & "Pulizia dei dati temporanei. "
            stb.Length = 0
            stb.Length = 0
            stb.Append("delete from RMA_Appoggio_Importazione  " & vbCrLf)
            MessaggioDiRitorno &= NomeRoutine & ". Numero di righe elaborate: " & contatoreRecord & vbCrLf

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_ScritturaNum(objParametri, stb.ToString, NomeRoutine, contatoreRecord)
            '--------------------------------------------------------------------------

            MessaggioErrore = ex.Message

            Dim Messaggio As String = ""
            If MessaggioErrore <> "" Then


                Messaggio += "ERR: Sono stati rilevati i seguenti errori : " & vbCrLf
                Messaggio += "" & vbCrLf
                Messaggio += MessaggioErrore
                Messaggio += "" & vbCrLf
                Messaggio += "Ritentare il salvataggio dopo la correzione ..."

            End If

            Throw New Exception(Messaggio)

        Finally

            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri)



        End Try

        Return True

    End Function

    Private Sub FinalizzaImportazioneRMA_UE_OperazioniFinali(
       ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
       ByRef MessaggioDiRitorno As String,
       ByRef NomeRoutine As String,
       ByRef xRisp As Boolean,
       ByRef contatoreRecord As Integer,
       bNomeRoutine As String
    )

        Dim stb As New Text.StringBuilder

        NomeRoutine = bNomeRoutine & "sequenza_tabelle, RegolamentiRMA. "
        stb.Length = 0
        stb.Append(" update sequenza_tabelle  " & vbCrLf)
        stb.Append(" set Ultimo_Valore = (select MAX(rma_reg_cod) from RegolamentiRMA  ) " & vbCrLf)
        stb.Append(" where Nome_Tabella = 'regolamentiRMA' " & vbCrLf)

        '--------------------------------------------------------------------------
        xRisp = EseguiQuery_ScritturaNum(objParametri, stb.ToString, NomeRoutine, contatoreRecord)
        '--------------------------------------------------------------------------
        MessaggioDiRitorno &= NomeRoutine & ". Numero di righe elaborate: " & contatoreRecord & vbCrLf

        NomeRoutine = bNomeRoutine & "sequenza_tabelle, PrincipiAttiviRMA. "
        stb.Length = 0
        stb.Append(" update sequenza_tabelle  " & vbCrLf)
        stb.Append(" set Ultimo_Valore = (select MAX(pa_cod) from PrincipiAttiviRMA  ) " & vbCrLf)
        stb.Append(" where Nome_Tabella = 'PrincipiAttiviRMA' " & vbCrLf)
        stb.Append("  " & vbCrLf)
        '--------------------------------------------------------------------------
        xRisp = EseguiQuery_ScritturaNum(objParametri, stb.ToString, NomeRoutine, contatoreRecord)
        '--------------------------------------------------------------------------
        MessaggioDiRitorno &= NomeRoutine & ". Numero di righe elaborate: " & contatoreRecord & vbCrLf


    End Sub


    Private Sub FinalizzaImportazioneRMA_UE_UpdateUltimoValore(
       ByVal NomeTabella As String,
       ByVal NomeColonna As String,
       ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
       ByRef MessaggioDiRitorno As String,
       ByRef NomeRoutine As String,
       ByRef xRisp As Boolean,
       ByRef contatoreRecord As Integer,
       bNomeRoutine As String
    )

        Dim stb As New Text.StringBuilder


        NomeRoutine = bNomeRoutine & "sequenza_tabelle, " & NomeTabella & ". "
        stb.Length = 0
        stb.Append(" update sequenza_tabelle  " & vbCrLf)
        stb.Append(" set Ultimo_Valore = (select MAX(" & NomeColonna & ") from " & NomeTabella & "  ) " & vbCrLf)
        stb.Append(" where Nome_Tabella = '" & NomeTabella & "' " & vbCrLf)

        '--------------------------------------------------------------------------
        xRisp = EseguiQuery_ScritturaNum(objParametri, stb.ToString, NomeRoutine, contatoreRecord)
        '--------------------------------------------------------------------------
        MessaggioDiRitorno &= NomeRoutine & ". Numero di righe elaborate: " & contatoreRecord & vbCrLf


    End Sub

    Private Sub FinalizzaImportazioneRMA_UE_RiportoCompletamentoDatiNormativa(
        ByVal FornitoreDati As enum_CAC_Codifica_PrincipiAttivi_Tipo_Codifica,
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
        ByRef MessaggioDiRitorno As String,
        ByRef NomeRoutine As String,
        ByRef xRisp As Boolean,
        ByRef contatoreRecord As Integer,
        bNomeRoutine As String
    )

        Dim stb As New Text.StringBuilder

        stb.Length = 0

        MessaggioDiRitorno &= vbCrLf & "   --- Operazioni di riporto e completamento dei dati come da normativa --- " & vbCrLf

        'individuare tutti i principi attivi che non hanno una rma. 
        'saranno quelli nuovi rispetto ad import precedente (possono avere un rma solo su un regolamento ma non su un altro, in questo caso occorre aggiungerlo)
        'oppure che fanno riferimento a nuove tabelle di RMA
        'join su CAC_codifica_Principi_attivi (codice fornitore in TabellaRMA)        


        stb.AppendLine(" create table #tmpDerrate (derr_Codifica int, rma_reg_cod int, CodiceEsterno varchar(100), Desc_Principio_Cliente nvarchar(4000), pa_cod int, EsiteRMA varchar(100), Operazione int, DefaultRMA real) ")
        stb.AppendLine("  ")
        stb.AppendLine("  ")
        stb.AppendLine("  ")
        stb.AppendLine(" insert #tmpDerrate ")
        stb.AppendLine(" select  ")
        stb.AppendLine("    acti.derr_codifica ")
        stb.AppendLine("  , acti.rma_reg_cod ")
        stb.AppendLine("  , acti.CodiceEsterno ")
        stb.AppendLine("  , acti.Desc_Principio_Cliente ")
        stb.AppendLine("  , parma.pa_cod ")
        stb.AppendLine("  , esistenti.codiceEsterno as EsiteRMA ")
        stb.AppendLine("  , 0 as Operazione ")
        stb.AppendLine("  , case when acti.DefaultRMA is null then 0.01 else acti.DefaultRMA end as DefaultRMA ")
        stb.AppendLine(" from ( ")
        stb.AppendLine("  --tutti i principi attivi pivotati su tutti i regolamenti padri delle tabelle homologa ")
        stb.AppendLine("  select distinct pivot1.rma_reg_cod, Cod_Principio_Cliente as CodiceEsterno, Desc_Principio_Cliente, DefaultRMA, derr_codifica ")
        stb.AppendLine("  from cac_codifica_principiAttivi cac ")
        stb.AppendLine("      inner join ( ")
        stb.AppendLine("          select rrma.rma_reg_Cod, tarma.RMADefault as DefaultRMA, d.derr_Codifica ")
        stb.AppendLine("          from RegolamentiRMA rrma         ")
        stb.AppendLine("          inner join TabellaRMA tarma ")
        stb.AppendLine("              on tarma.TabellaRMA_COD = rrma.TabellaRMA_COD ")
        stb.AppendLine("          inner join derratecodifica d ")
        stb.AppendLine("              on d.TabellaRMA_COD = rrma.TabellaRMA_COD ")
        stb.AppendLine("          where rrma.rma_Reg_cod_padre is null ")
        stb.AppendLine("          and tarma.FornitoreDati = " & FornitoreDati)
        stb.AppendLine("      ) pivot1 ")
        stb.AppendLine("      on 1=1       ")
        stb.AppendLine("      where tipo_Codifica = " & FornitoreDati)
        stb.AppendLine(" ) acti ")
        stb.AppendLine("  ")
        stb.AppendLine(" left join  (  ")
        stb.AppendLine("  select distinct  ")
        stb.AppendLine("        parma.codiceEsterno ")
        stb.AppendLine("      , parma.pa_cod ")
        stb.AppendLine("      , parma.rma_reg_cod ")
        stb.AppendLine("  from PrincipiAttiviRMA parma                 ")
        stb.AppendLine("  inner join RegolamentiRMA rrma ")
        stb.AppendLine("      on rrma.RMA_REG_COD = parma.RMA_REG_COD      ")
        stb.AppendLine("      and rrma.rma_reg_cod_Padre is null ")
        stb.AppendLine("  inner join TabellaRMA tarma ")
        stb.AppendLine("      on tarma.TabellaRMA_COD = rrma.TabellaRMA_COD ")
        stb.AppendLine("  where tarma.FornitoreDati =   " & FornitoreDati)
        stb.AppendLine(" ) parma ")
        stb.AppendLine("  ")
        stb.AppendLine(" on parma.CodiceEsterno = acti.CodiceEsterno ")
        stb.AppendLine(" and parma.rma_reg_cod = acti.rma_reg_cod ")
        stb.AppendLine("  ")
        stb.AppendLine(" left join ( ")
        stb.AppendLine("  --tutti i principi attivi con un RMA su qualsiasi regolamento padre ")
        stb.AppendLine("  select parma.CodiceEsterno, rrma.rma_reg_cod ")
        stb.AppendLine("  from DerrateRMAxPrincipiAttiviRMA derma ")
        stb.AppendLine("  inner join PrincipiAttiviRMA parma ")
        stb.AppendLine("      on derma.PA_COD = parma.PA_COD ")
        stb.AppendLine("      and derma.RMA_REG_COD = parma.RMA_REG_COD ")
        stb.AppendLine("  inner join RegolamentiRMA rrma ")
        stb.AppendLine("      on rrma.RMA_REG_COD = parma.RMA_REG_COD      ")
        stb.AppendLine("      and rrma.rma_reg_cod_Padre is null ")
        stb.AppendLine("  inner join TabellaRMA tarma ")
        stb.AppendLine("      on tarma.TabellaRMA_COD = rrma.TabellaRMA_COD ")
        stb.AppendLine("  where tarma.FornitoreDati =  " & FornitoreDati)
        stb.AppendLine(" ) esistenti ")
        stb.AppendLine(" on esistenti.codiceEsterno = acti.CodiceEsterno ")
        stb.AppendLine(" and esistenti.rma_reg_cod = acti.rma_reg_cod ")
        stb.AppendLine("  ")
        stb.AppendLine(" order by desc_principio_Cliente, acti.rma_reg_cod")

        FinalizzaImportazioneRMA_UE_EseguiStatement(" Inserimento in tabella temporanea.",
                                                    objParametri, MessaggioDiRitorno, NomeRoutine, bNomeRoutine, stb)

        '----------------------------------------------------
        stb.Length = 0

        stb.AppendLine(" update t ")
        stb.AppendLine(" set pa_cod = sequenza.Pa_cod ")
        stb.AppendLine(" , Operazione = 1 ")
        stb.AppendLine(" --select  ")
        stb.AppendLine(" --     t.* ")
        stb.AppendLine(" --   , sequenza.Pa_cod as sequenza_Pa_cod  ")
        stb.AppendLine(" from #tmpDerrate t ")
        stb.AppendLine(" inner join ( ")
        stb.AppendLine("  select  ")
        stb.AppendLine("      est.CodiceEsterno ")
        stb.AppendLine("  ,   DENSE_RANK() over (order by est.CodiceEsterno) + (select max(pa_cod) from PrincipiAttiviRMA ) as Pa_cod ")
        stb.AppendLine("  from ( ")
        stb.AppendLine("      select distinct codiceEsterno ")
        stb.AppendLine("      from #tmpDerrate t ")
        stb.AppendLine("  ) est ")
        stb.AppendLine("  ")
        stb.AppendLine("  left join (  ")
        stb.AppendLine("      select distinct pa_cod, CodiceEsterno ")
        stb.AppendLine("      from #tmpDerrate d ")
        stb.AppendLine("      where pa_cod is not null ")
        stb.AppendLine("  ) pa ")
        stb.AppendLine("  on pa.CodiceEsterno = est.CodiceEsterno ")
        stb.AppendLine("  where pa.pa_cod Is null ")
        stb.AppendLine(" ) sequenza ")
        stb.AppendLine(" on sequenza.CodiceEsterno = t.CodiceEsterno")

        FinalizzaImportazioneRMA_UE_EseguiStatement(" update per nuovi principi attivi (richiede nuovo progressivo, sarà memorizzato in colonna pa_cod)",
                                                    objParametri, MessaggioDiRitorno, NomeRoutine, bNomeRoutine, stb)
        '----------------------------------------------------


        '----------------------------------------------------
        stb.Length = 0
        stb.AppendLine(" update t ")
        stb.AppendLine(" set pa_cod = pa.Pa_cod ")
        stb.AppendLine(" --select  ")
        stb.AppendLine(" --     t.* ")
        stb.AppendLine(" --   , pa.Pa_cod as sequenza_Pa_cod  ")
        stb.AppendLine(" from #tmpDerrate t ")
        stb.AppendLine(" inner join  (  ")
        stb.AppendLine("      select distinct pa_cod, CodiceEsterno ")
        stb.AppendLine("      from #tmpDerrate d ")
        stb.AppendLine("      where pa_cod is not null ")
        stb.AppendLine("  ) pa ")
        stb.AppendLine("  on pa.CodiceEsterno = t.CodiceEsterno    ")
        stb.AppendLine("  ")
        stb.AppendLine(" where t.pa_cod is null")

        FinalizzaImportazioneRMA_UE_EseguiStatement(" Update del pa_cod dove non esiste. ",
                                                    objParametri, MessaggioDiRitorno, NomeRoutine, bNomeRoutine, stb)
        '----------------------------------------------------



        '----------------------------------------------------
        stb.Length = 0
        stb.AppendLine(" insert principiAttiviRMA( PA_COD, RMA_REG_COD, PA_DES, CodiceEsterno) ")
        stb.AppendLine(" select pa_cod, rma_reg_cod, Desc_Principio_Cliente, CodiceEsterno ")
        stb.AppendLine(" from #tmpDerrate t ")
        stb.AppendLine(" where not exists ( ")
        stb.AppendLine("  select 1 ")
        stb.AppendLine("  from principiAttiviRMA parma ")
        stb.AppendLine("  where parma.pa_cod = t.pa_cod ")
        stb.AppendLine("  and parma.rma_reg_Cod = t.rma_reg_cod ")
        stb.AppendLine(" )")

        FinalizzaImportazioneRMA_UE_EseguiStatement(" 1. insert nuovi principi attivi e regolamenti (sia quanto non esiste per nulla sia nuovi pa in regolamenti precedenti)",
                                                    objParametri, MessaggioDiRitorno, NomeRoutine, bNomeRoutine, stb)
        '----------------------------------------------------



        '----------------------------------------------------
        stb.Length = 0
        stb.AppendLine(" insert derratermaXprincipiAttiviRma (DERR_COD, DERR_CODIFICA, PA_COD, RMA_REG_COD, RMA, Armonizzato, Minimo_misurabile, note) ")
        stb.AppendLine("  ")
        stb.AppendLine(" select  ")
        stb.AppendLine("  '162010' as derr_Cod ")
        stb.AppendLine("  , derr_Codifica ")
        stb.AppendLine("  , pa_cod ")
        stb.AppendLine("  , rma_reg_cod ")
        stb.AppendLine("  , DefaultRMA ")
        stb.AppendLine("  , 0 ")
        stb.AppendLine("  , 1 ")
        stb.AppendLine("  , 'RMA predefinito come da normativa' ")
        stb.AppendLine(" from #tmpDerrate t ")
        stb.AppendLine(" where EsiteRMA is null")

        FinalizzaImportazioneRMA_UE_EseguiStatement(" 2. nuovo rma per nuovi principi attivi, sui regolamenti dove non esiste ",
                                                    objParametri, MessaggioDiRitorno, NomeRoutine, bNomeRoutine, stb)
        '---------------------------------------------------- 


        '----------------------------------------------------
        stb.Length = 0
        stb.AppendLine(" drop table #tmpDerrate ")

        FinalizzaImportazioneRMA_UE_EseguiStatement(" eliminata tabella temporanea ",
                                                    objParametri, MessaggioDiRitorno, NomeRoutine, bNomeRoutine, stb)

        '----------------------------------------------------


    End Sub

    Private Sub FinalizzaImportazioneRMA_UE_EseguiStatement(
        ByVal MessaggioTestata As String,
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
        ByRef MessaggioDiRitorno As String,
        ByRef NomeRoutine As String,
        bNomeRoutine As String,
        ByRef stb As Text.StringBuilder
    )

        Dim contatoreRecord As Integer
        Dim xRisp As Boolean

        NomeRoutine = bNomeRoutine & MessaggioTestata
        '--------------------------------------------------------------------------
        xRisp = EseguiQuery_ScritturaNum(objParametri, stb.ToString, NomeRoutine, contatoreRecord)
        '--------------------------------------------------------------------------        

        If contatoreRecord >= 0 Then
            MessaggioDiRitorno &= NomeRoutine & ". Numero di righe elaborate: " & contatoreRecord & vbCrLf
        End If

    End Sub

    Public Function TabellaRmaNuoviRecordPerCodiceEsterno(
        ByVal CodiceEsternoDerrateRMa As String,
        ByVal ListaCodiciDescrizioni As List(Of KeyValuePair(Of String, String)),
        ByVal FornitoreDati As enum_CAC_Codifica_PrincipiAttivi_Tipo_Codifica,
        ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
        ByRef MessaggioDiRitorno As String,
        ByRef NomeRoutine As String,
        ByRef xRisp As Boolean,
        ByRef contatoreRecord As Integer,
        bNomeRoutine As String
     ) As Boolean

        If ListaCodiciDescrizioni.Count = 0 Then
            Return True
        End If

        Dim messaggioErrore As String = ""

        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False

        Dim rval As Boolean

        'TODO: Gestire la tranzazione
        Try

            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Apro la connessione al DB
            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale,
                                                                            FlagTransazioneLocale,
                                                                            objParametri_Server)


            Dim stb As New Text.StringBuilder

            Dim prefisso As String = ""
            If FornitoreDati = enum_CAC_Codifica_PrincipiAttivi_Tipo_Codifica.HOMOLOGA Then
                prefisso = "Homologa: "
            End If

            Dim lSelect1 As New List(Of String)
            For Each kd In ListaCodiciDescrizioni
                lSelect1.Add("select '" & Agro_SQL_SaveText(kd.Key) & "' as CodiceEsterno, '" & Agro_SQL_SaveText(prefisso & kd.Value) & "' as Directive")
            Next

            Dim stmList As String = String.Join(vbCrLf & "union" & vbCrLf, lSelect1)

            stb.Length = 0

            NomeRoutine = bNomeRoutine & " Nuovi Record in tabelle di RMA"

            stb.AppendLine(" insert TabellaRMA ( ")
            stb.AppendLine("    [TabellaRMA_COD], [TabellaRMA_DES], [inviato], [datainvio], [Data_Creazione], [Data_Modifica], [Username_Creazione], [Username_Modifica], [Validita_Inizio], [Validita_Fine], [CodiceEsterno], [FornitoreDati], [RMADefault], [Note] ")
            stb.AppendLine(" ) ")
            stb.AppendLine(" Select ")
            stb.AppendLine("       DENSE_RANK() over (order by CodiceEsterno) + (Select max(Ultimo_Valore) from sequenza_tabelle where Nome_Tabella = 'TabellaRMa') ")
            stb.AppendLine("     , a.TabellaRMA_DES ")
            stb.AppendLine("     , 0 as inviato    ")
            stb.AppendLine("     , null as datainvio   ")
            stb.AppendLine("     , getdate() as Data_Creazione     ")
            stb.AppendLine("     , getdate() as Data_Modifica  ")
            stb.AppendLine("     , 'sitoblu' as Username_Creazione     ")
            stb.AppendLine("     , 'sitoblu' as Username_Modifica  ")
            stb.AppendLine("     , '01/01/1900' as Validita_Inizio     ")
            stb.AppendLine("     , '31/12/2100' as Validita_Fine   ")
            stb.AppendLine("     , a.CodiceEsterno     ")
            stb.AppendLine("     , " & FornitoreDati & " as FornitoreDati  ")
            stb.AppendLine("     , null as RMADefault  ")
            stb.AppendLine("     , '' as Note ")
            stb.AppendLine(" from( ")
            stb.AppendLine("     select distinct     ")
            stb.AppendLine("           SUBSTRING(app.CodiceEsterno, 1, 9) As CodiceEsterno ")
            stb.AppendLine("         , app.directive as TabellaRMA_DES ")
            stb.AppendLine("     from( ")
            stb.AppendLine(stmList)
            stb.AppendLine("     ) app ")
            stb.AppendLine("     where not exists( ")
            stb.AppendLine("         select 1 ")
            stb.AppendLine("         From TabellaRMA tarma ")
            stb.AppendLine("         Where tarma.CodiceEsterno = SUBSTRING(app.CodiceEsterno, 1, 9) ")
            stb.AppendLine("         AND FornitoreDati = " & FornitoreDati)
            stb.AppendLine("     ) ")
            stb.AppendLine(" ) a")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_ScritturaNum(objParametri_Server, stb.ToString, NomeRoutine, contatoreRecord)
            '--------------------------------------------------------------------------
            MessaggioDiRitorno &= NomeRoutine & ". Numero di righe elaborate: " & contatoreRecord & vbCrLf


            stb.Length = 0

            NomeRoutine = bNomeRoutine & " Nuovi Record regolamenti di base "


            stb.AppendLine(" insert [dbo].[RegolamentiRMA]( ")
            stb.AppendLine("     [RMA_REG_COD], [RMA_REG_DES], [inviato], [datainvio], [Data_Creazione], [Data_Modifica], [Username_Creazione], [Username_Modifica], [Validita_Inizio], [Validita_Fine], [TabellaRMA_COD], [RMA_REG_COD_PADRE] ")
            stb.AppendLine(" )")
            stb.AppendLine(" Select ")
            stb.AppendLine("      DENSE_RANK() over (order by TabellaRMA_cod) + (Select max(rma_reg_cod) from  RegolamentiRMA )  As rma_Reg_cod ")
            stb.AppendLine("     , TabellaRMA_DES + ' Entry into force: 01/01/1900' as rma_reg_Des ")
            stb.AppendLine("     , 0 as inviato ")
            stb.AppendLine("     , null as datainvio    ")
            stb.AppendLine("     , getdate() as Data_Creazione  ")
            stb.AppendLine("     , getdate() as Data_Modifica   ")
            stb.AppendLine("     , 'sitoblu' as Username_Creazione  ")
            stb.AppendLine("     , 'sitoblu' as Username_Modifica   ")
            stb.AppendLine("     , '01/01/1900' as Validita_Inizio  ")
            stb.AppendLine("     , '31/12/2100' as Validita_Fine    ")
            stb.AppendLine("     , TabellaRMA_COD ")
            stb.AppendLine("     , null as rma_reg_cod_padre ")
            stb.AppendLine(" From tabellarma ")
            stb.AppendLine(" Where Not exists( ")
            stb.AppendLine("     select 1 ")
            stb.AppendLine("     From RegolamentiRMA ")
            stb.AppendLine("     Where RMA_REG_DES = TabellaRMA_DES + ' Entry into force: 01/01/1900' ")
            stb.AppendLine("     And RMA_REG_COD_PADRE Is null ")
            stb.AppendLine(" ) ")
            stb.AppendLine(" And fornitoreDati = " & FornitoreDati)


            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_ScritturaNum(objParametri_Server, stb.ToString, NomeRoutine, contatoreRecord)
            '--------------------------------------------------------------------------
            MessaggioDiRitorno &= NomeRoutine & ". Numero di righe elaborate: " & contatoreRecord & vbCrLf



            stb.Length = 0

            NomeRoutine = bNomeRoutine & " Nuovi Record per codifica Derrate "

            stb.AppendLine("  insert [dbo].[DerrateCodifica]( ")
            stb.AppendLine("     [DERR_CODIFICA], [Descrizione], [inviato], [datainvio], [Data_Creazione], [Data_Modifica], [Username_Creazione], [Username_Modifica], [Validita_Inizio], [Validita_Fine], [TabellaRMA_Cod] ")
            stb.AppendLine("  ) ")
            stb.AppendLine("  Select ")
            stb.AppendLine("       DENSE_RANK() over (order by TabellaRMA_cod) + (Select max(DERR_CODIFICA) from  DerrateCodifica )  As derr_codifica ")
            stb.AppendLine("     , tarma.TabellaRMA_DES as descrizione ")
            stb.AppendLine("         , 0 as inviato ")
            stb.AppendLine("     , null as datainvio    ")
            stb.AppendLine("     , getdate() as Data_Creazione  ")
            stb.AppendLine("     , getdate() as Data_Modifica   ")
            stb.AppendLine("     , 'sitoblu' as Username_Creazione  ")
            stb.AppendLine("     , 'sitoblu' as Username_Modifica   ")
            stb.AppendLine("     , '01/01/1900' as Validita_Inizio  ")
            stb.AppendLine("     , '31/12/2100' as Validita_Fine ")
            stb.AppendLine("     , tarma.TabellaRMA_COD ")
            stb.AppendLine("  From TabellaRMA  tarma ")
            stb.AppendLine("  Where Not exists( ")
            stb.AppendLine("      select 1 ")
            stb.AppendLine("      From DerrateCodifica dc ")
            stb.AppendLine("      Where dc.Descrizione = tarma.TabellaRMA_DES ")
            stb.AppendLine(" ) ")
            stb.AppendLine(" And tarma.FornitoreDati =  " & FornitoreDati)

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_ScritturaNum(objParametri_Server, stb.ToString, NomeRoutine, contatoreRecord)
            '--------------------------------------------------------------------------
            MessaggioDiRitorno &= NomeRoutine & ". Numero di righe elaborate: " & contatoreRecord & vbCrLf


            stb.Length = 0

            NomeRoutine = bNomeRoutine & " Nuovi Record per DerrateRMA (su nuove codifiche) "


            stb.AppendLine("  insert DerrateRMA( ")
            stb.AppendLine("      [DERR_COD], [DERR_CODIFICA], [DERR_DES], [DERR_DES_LAT], [DERR_LIV], [Note1], [Note2], [DATA_AGG], [Liv1], [Liv2], [Liv3], [Liv4], [inviato], [datainvio], [Data_Creazione], [Data_Modifica], [Username_Creazione], [Username_Modifica], [Validita_Inizio], [Validita_Fine], [CodiceEsterno] ")
            stb.AppendLine("  ) ")
            stb.AppendLine("  ")
            stb.AppendLine(" Select  ")
            stb.AppendLine("       '162010' as DERR_COD ")
            stb.AppendLine("     , d.DERR_CODIFICA ")
            stb.AppendLine("     , 'Kiwi' as DERR_DES  ")
            stb.AppendLine("     , ' Actinidia deliciosa syn. A. chinensis ' as DERR_DES_LAT   ")
            stb.AppendLine("     , '4' as DERR_LIV     ")
            stb.AppendLine("     , '' as Note1     ")
            stb.AppendLine("     , '' as Note2     ")
            stb.AppendLine("     , getdate() as DATA_AGG   ")
            stb.AppendLine("     , null as Liv1    ")
            stb.AppendLine("     , '' as Liv2  ")
            stb.AppendLine("     , null as Liv3    ")
            stb.AppendLine("     , 'Kiwi' as Liv4  ")
            stb.AppendLine("     , 0 as inviato    ")
            stb.AppendLine("     , null as datainvio   ")
            stb.AppendLine("     , getdate() as Data_Creazione     ")
            stb.AppendLine("     , getdate() as Data_Modifica  ")
            stb.AppendLine("     , 'sitoblu' as Username_Creazione     ")
            stb.AppendLine("     , 'sitoblu' as Username_Modifica  ")
            stb.AppendLine("     , '01/01/1900' as Validita_Inizio     ")
            stb.AppendLine("     , '31/12/2100' as Validita_Fine   ")
            stb.AppendLine("     , '" & Agro_SQL_SaveText(CodiceEsternoDerrateRMa) & "' as CodiceEsterno ")
            stb.AppendLine(" From DerrateCodifica d ")
            stb.AppendLine("     inner Join TabellaRMA tarma ")
            stb.AppendLine("             On d.TabellaRMA_Cod = tarma.TabellaRMA_COD ")
            stb.AppendLine(" where tarma.FornitoreDati = " & FornitoreDati)
            stb.AppendLine(" And Not exists ( ")
            stb.AppendLine("     Select  1 ")
            stb.AppendLine("     From DerrateRMA derma ")
            stb.AppendLine("     Where derma.DERR_CODIFICA = d.DERR_CODIFICA ")
            stb.AppendLine("     And derma.DERR_COD = '162010' ")
            stb.AppendLine(" ) ")


            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_ScritturaNum(objParametri_Server, stb.ToString, NomeRoutine, contatoreRecord)
            '--------------------------------------------------------------------------
            MessaggioDiRitorno &= NomeRoutine & ". Numero di righe elaborate: " & contatoreRecord & vbCrLf


            FinalizzaImportazioneRMA_UE_UpdateUltimoValore(
                "TabellaRMA",
                "TabellaRMA_COD",
                objParametri_Server,
                MessaggioDiRitorno,
                NomeRoutine,
                xRisp,
                contatoreRecord,
                bNomeRoutine)


            FinalizzaImportazioneRMA_UE_UpdateUltimoValore(
                "regolamentiRMA",
                "RMA_REG_COD",
                objParametri_Server,
                MessaggioDiRitorno,
                NomeRoutine,
                xRisp,
                contatoreRecord,
                bNomeRoutine)


            FinalizzaImportazioneRMA_UE_UpdateUltimoValore(
                "DerrateCodifica",
                "DERR_CODIFICA",
                objParametri_Server,
                MessaggioDiRitorno,
                NomeRoutine,
                xRisp,
                contatoreRecord,
                bNomeRoutine)


            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Chiudo la transazione e la connessione al DB
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri_Server)
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

            rval = True

        Catch ex As Exception

            'Faccio il rollback della transazione
            If Not objParametri_Server.objTransazione Is Nothing Then
                'objParametri.objTransazione.Rollback()
                'objParametri.objTransazione = Nothing
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)

            End If

            messaggioErrore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True)


            Dim Messaggio As String = ""
            If messaggioErrore <> "" Then


                Messaggio += "ERR: Sono stati rilevati i seguenti errori : " & vbCrLf
                Messaggio += "" & vbCrLf
                Messaggio += messaggioErrore
                Messaggio += "" & vbCrLf
                Messaggio += "Ritentare il salvataggio dopo la correzione ..."

            End If

            Throw New Exception(Messaggio, ex)

        Finally

            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri_Server)

        End Try


    End Function



    Private Sub FinalizzaImportazioneRMA_UE_RiportoRMA(
        byval FornitoreDati As  enum_CAC_Codifica_PrincipiAttivi_Tipo_Codifica,
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
        ByRef MessaggioDiRitorno As String,
        ByRef NomeRoutine As String,
        ByRef xRisp As Boolean,
        ByRef contatoreRecord As Integer,
        bNomeRoutine As String
     )

        Dim stb As New Text.StringBuilder

        stb.Length = 0

        NomeRoutine = bNomeRoutine & " A.  'Normalizzazione dei dati nella tabella rma_appoggio_importazione (tabellaRma_Cod<>1)"

        stb.AppendLine(" update i ")
        stb.AppendLine(" set tabellaRma_Cod = tarma.tabellaRma_Cod ")
        stb.AppendLine(" , rma_reg_cod_Padre = rrma.RMA_REG_COD ")
        stb.AppendLine(" , derr_codifica = dc.DERR_CODIFICA ")
        stb.AppendLine(" From RMA_Appoggio_Importazione i          ")
        stb.AppendLine("  inner Join  TabellaRMA tarma ")
        stb.AppendLine("         On tarma.CodiceEsterno = SUBSTRING(i.CodiceEsterno, 1, 9) ")
        stb.AppendLine("  inner Join RegolamentiRMA rrma ")
        stb.AppendLine("         On rrma.TabellaRMA_COD = tarma.TabellaRMA_COD ")
        stb.AppendLine("      And rrma.RMA_REG_COD_PADRE Is null ")
        stb.AppendLine("  inner Join DerrateCodifica dc ")
        stb.AppendLine("         On dc.TabellaRMA_Cod = tarma.TabellaRMA_COD")
        stb.AppendLine("  where tarma.FornitoreDati =  " & FornitoreDati)


        '--------------------------------------------------------------------------
        xRisp = EseguiQuery_ScritturaNum(objParametri, stb.ToString, NomeRoutine, contatoreRecord)
        '--------------------------------------------------------------------------
        MessaggioDiRitorno &= NomeRoutine & ". Numero di righe elaborate: " & contatoreRecord & vbCrLf
        stb.Length = 0

        NomeRoutine = bNomeRoutine & " B.  'Normalizzazione dei dati nella tabella rma_appoggio_importazione (tabellaRma_Cod=1)"

        stb.AppendLine(" update i ")
        stb.AppendLine(" set tabellaRma_Cod = 1 , Rma_Reg_Cod_Padre = 1, Derr_Codifica = 1 ")
        stb.AppendLine(" From RMA_Appoggio_Importazione i ")
        stb.AppendLine(" where CodiceEsterno is null ")

        '--------------------------------------------------------------------------
        xRisp = EseguiQuery_ScritturaNum(objParametri, stb.ToString, NomeRoutine, contatoreRecord)
        '--------------------------------------------------------------------------
        MessaggioDiRitorno &= NomeRoutine & ". Numero di righe elaborate: " & contatoreRecord & vbCrLf

        stb.Length = 0

        NomeRoutine = bNomeRoutine & " 1.  'importazione dei nuovi regolamenti"

        '' VAnni: 18/10/2019: Homologa, RMA_REG_COD_PADRE letto e riportato in tabella rma_appoggio_Importazione
        '' VAnni: 18/10/2019: Homologa, left join regolamentiRMA rrma on app.Directive + ' Entry into force: ' va sostituita in maniera generica
        '' VAnni: 21/10/2019: Si suppone che il campo TabellaRma_cod 
        stb.Append(" insert regolamentiRMA(rma_reg_cod, RMA_REG_DES, RMA_REG_COD_PADRE, Username_Creazione, Username_Modifica, validita_inizio) " & vbCrLf)
        stb.Append(" Select distinct " & vbCrLf)
        stb.Append("    (select MAX(RMA_REG_COD) from RegolamentiRMA ) + DENSE_RANK () over (order by app.Directive + ' Entry into force: ' + convert( varchar(1000), app.Entry_force, 103)) as rma_reg_cod " & vbCrLf)
        stb.Append("    , app.Directive + ' Entry into force: ' + convert( varchar(1000), app.Entry_force, 103) as RMA_REG_DES " & vbCrLf)
        stb.Append("    , app.RMA_REG_COD_PADRE " & vbCrLf)
        stb.Append("    , 'sitoblu123' " & vbCrLf)
        stb.Append("    , 'sitoblu123' " & vbCrLf)
        stb.Append("    , app.Entry_force  " & vbCrLf)
        stb.Append(" from rma_appoggio_Importazione app " & vbCrLf)
        stb.Append("    left join regolamentiRMA rrma on app.Directive + ' Entry into force: ' + convert( varchar(1000), app.Entry_force, 103) = rrma.RMA_REG_DES  " & vbCrLf)
        stb.Append(" where rrma.RMA_REG_DES is null " & vbCrLf)

        '--------------------------------------------------------------------------
        xRisp = EseguiQuery_ScritturaNum(objParametri, stb.ToString, NomeRoutine, contatoreRecord)
        '--------------------------------------------------------------------------
        MessaggioDiRitorno &= NomeRoutine & ". Numero di righe elaborate: " & contatoreRecord & vbCrLf


        stb.Length = 0
        'stb.Append(" " & vbCrLf)

        'l'associazione è indispensabile perchè qualsiasi principio attivo è soggetto ad un RMA di 0,01 ppm (limite strumentale)
        ' anche se non esplicitato. quindi occorre che si trovi associato al regolamento che si sta verificando.        
        'NomeRoutine = bNomeRoutine & " 2.  print('associazione di principi attivi che esistono già in altri regolamenti della medesima tabella di RMA sui nuovi regolamenti inseriti') "
        NomeRoutine = bNomeRoutine & " 2.  print('associazione di principi attivi che esistono già nel regolamento Padre della medesima tabella di RMA sui nuovi regolamenti inseriti') "

        stb.Append(" insert PrincipiAttiviRMA(PA_COD, RMA_REG_COD, PA_DES, CodiceEsterno, Username_Creazione, Username_Modifica) " & vbCrLf)
        stb.Append(" select distinct paRMa.PA_COD, rrma.RMA_REG_COD, app.PA_Des, paRMA.CodiceEsterno, 'sitoblu123', 'sitoblu123' " & vbCrLf)
        stb.Append(" from rma_appoggio_importazione app  " & vbCrLf)
        stb.Append("    inner join RegolamentiRMA rrma  " & vbCrLf)
        stb.Append("        on app.Directive + ' Entry into force: ' + convert( varchar(1000), app.Entry_force, 103) = rrma.RMA_REG_DES  " & vbCrLf)
        stb.Append("        -- per i nuovi regolamenti appena inseriti TabellaRMA_COD è NULL. altrimenti è sempre valorizzato. I regolamenti principali con RMA_REG_COD_PADRE = NULL devono già esistere " & vbCrLf)
        stb.Append("        and rrma.TabellaRMA_COD is null " & vbCrLf)
        stb.Append("  " & vbCrLf)
        'stb.Append("    inner join principiAttiviRMA paRMA " & vbCrLf)
        stb.AppendLine("  inner join ( ")
        stb.AppendLine("  select distinct  ")
        stb.AppendLine("    rrma.RMA_REG_COD ")
        stb.AppendLine("  , rrma.TabellaRMA_COD ")
        stb.AppendLine("  , parma.pa_Cod ")
        stb.AppendLine("  , parma.pa_Des ")
        stb.AppendLine("  , parma.CodiceEsterno ")
        stb.AppendLine("  from principiattivirma parma ")
        stb.AppendLine("  inner join regolamentiRma rrma ")
        stb.AppendLine("      on parma.RMA_REG_COD = rrma.RMA_REG_COD")
        stb.AppendLine("  inner join TabellaRMA tarma ")
        stb.AppendLine("      on tarma.TabellaRMA_COD = rrma.TabellaRMA_COD")
        stb.AppendLine("  where tarma.FornitoreDati = " & FornitoreDati)
        stb.AppendLine("  and rrma.rma_reg_cod_padre is null ")
        stb.AppendLine("  ) paRMA ")
        stb.Append("  On paRMA.TabellaRMA_COD = app.TabellaRMA_COD AND ( ")
        stb.Append("  ( paRMA.CodiceEsterno Is null And paRMA.PA_DES = app.PA_Des ) " & vbCrLf)
        stb.Append("    Or (paRMA.CodiceEsterno Is Not null And paRMA.CodiceEsterno = SUBSTRING(app.CodiceEsterno, 11, 5)) " & vbCrLf)
        stb.Append("  ) ")




        '--------------------------------------------------------------------------
        xRisp = EseguiQuery_ScritturaNum(objParametri, stb.ToString, NomeRoutine, contatoreRecord)
        '--------------------------------------------------------------------------
        MessaggioDiRitorno &= NomeRoutine & ". Numero di righe elaborate: " & contatoreRecord & vbCrLf



        stb.Length = 0
        NomeRoutine = bNomeRoutine & "3.  recupera la decodifica Agronica/UE per il principio attivo che esiste già copiandolo da un regolamento precedente "

        stb.Append(" insert PrincipiAttivixPrincipiAttiviRMA(PA_COD, PA_COD_RMA, RMA_REG_COD, Username_Creazione, Username_Modifica) " & vbCrLf)
        stb.Append(" select distinct papaRMA.PA_COD, papaRMA.PA_COD_RMA, manca.RMA_REG_COD, 'sitoblu123', 'sitoblu123' " & vbCrLf)
        stb.Append(" from  " & vbCrLf)
        stb.Append(" ( " & vbCrLf)
        stb.Append(" select paRMA.* " & vbCrLf)
        stb.Append(" from PrincipiAttiviRMA paRMA " & vbCrLf)
        stb.Append(" inner join RegolamentiRMA rrma  " & vbCrLf)
        stb.Append("    on paRMA.RMA_REG_COD = rrma.RMA_REG_COD  " & vbCrLf)
        stb.Append("    -- per i nuovi regolamenti appena inseriti TabellaRMA_COD è NULL. altrimenti è sempre valorizzato." & vbCrLf)
        stb.Append("    and rrma.TabellaRMA_COD is null " & vbCrLf)
        stb.Append("  " & vbCrLf)
        stb.Append(" left join PrincipiAttivixPrincipiAttiviRMA papaRMA " & vbCrLf)
        stb.Append("    on paRMA.PA_COD = papaRMA.PA_COD_RMA  " & vbCrLf)
        stb.Append("    and paRMA.RMA_REG_COD = papaRMA.RMA_REG_COD  " & vbCrLf)
        stb.Append(" where papaRMA.PA_COD is null " & vbCrLf)
        stb.Append(" ) manca " & vbCrLf)
        stb.Append(" inner join PrincipiAttivixPrincipiAttiviRMA papaRMA " & vbCrLf)
        stb.Append("    on manca.PA_COD = papaRMA.PA_COD_RMA  " & vbCrLf)
        stb.Append("  " & vbCrLf)
        stb.Append(" group by  manca.RMA_REG_COD, papaRMA.PA_COD_RMA, papaRMA.PA_COD, manca.pa_des " & vbCrLf)

        '--------------------------------------------------------------------------
        xRisp = EseguiQuery_ScritturaNum(objParametri, stb.ToString, NomeRoutine, contatoreRecord)
        '--------------------------------------------------------------------------
        MessaggioDiRitorno &= NomeRoutine & ". Numero di righe elaborate: " & contatoreRecord & vbCrLf


        NomeRoutine = bNomeRoutine & "4.  recupera la decodifica Agronica/UE per le famiglie di principi attivi che esiste già copiandolo da un regolamento precedente "

        stb.Length = 0
        'stb.Append(" " & vbCrLf  ) 
        stb.Append(" insert FamigliePrincipiAttiviXPrincipiAttiviRMA (fam_COD, PA_COD_RMA, RMA_REG_COD ,  Username_Creazione, Username_Modifica ) " & vbCrLf)
        stb.Append(" select distinct papaRMA.Fam_COD, papaRMA.PA_COD_RMA, manca.RMA_REG_COD, 'sitoblu123', 'sitoblu123' " & vbCrLf)
        stb.Append(" from  " & vbCrLf)
        stb.Append(" ( " & vbCrLf)
        stb.Append(" select paRMA.* " & vbCrLf)
        stb.Append(" from PrincipiAttiviRMA paRMA " & vbCrLf)
        stb.Append(" inner join RegolamentiRMA rrma  " & vbCrLf)
        stb.Append("    on paRMA.RMA_REG_COD = rrma.RMA_REG_COD  " & vbCrLf)
        stb.Append("    -- per i nuovi regolamenti appena inseriti TabellaRMA_COD è NULL. altrimenti è sempre valorizzato." & vbCrLf)
        stb.Append("    and rrma.TabellaRMA_COD is null  " & vbCrLf)
        stb.Append("  " & vbCrLf)
        stb.Append(" left join FamigliePrincipiAttiviXPrincipiAttiviRMA papaRMA " & vbCrLf)
        stb.Append("    on paRMA.PA_COD = papaRMA.PA_COD_RMA  " & vbCrLf)
        stb.Append("    and paRMA.RMA_REG_COD = papaRMA.RMA_REG_COD  " & vbCrLf)
        stb.Append(" where papaRMA.Fam_COD is null " & vbCrLf)
        stb.Append(" ) manca " & vbCrLf)
        stb.Append(" inner join FamigliePrincipiAttiviXPrincipiAttiviRMA papaRMA " & vbCrLf)
        stb.Append("    on manca.PA_COD = papaRMA.PA_COD_RMA     " & vbCrLf)

        '--------------------------------------------------------------------------
        xRisp = EseguiQuery_ScritturaNum(objParametri, stb.ToString, NomeRoutine, contatoreRecord)
        '--------------------------------------------------------------------------
        MessaggioDiRitorno &= NomeRoutine & ". Numero di righe elaborate: " & contatoreRecord & vbCrLf


        NomeRoutine = bNomeRoutine & "5. inserimento di nuovi principi attivi che non esistono (in PrincipiAttiviRMA)"

        stb.Length = 0
        '' VAnni: 18/10/2019: Homologa, inner join regolamentiRMA rrma on app.Directive + ' Entry into force: ' va sostituita in maniera generica
        stb.Append(" insert PrincipiAttiviRMA(PA_COD, RMA_REG_COD, PA_DES, CodiceEsterno, Username_Creazione, Username_Modifica) " & vbCrLf)
        stb.Append(" Select distinct " & vbCrLf)
        stb.Append("    ( select MAX(pa_cod) from PrincipiAttiviRMA )  + dense_rank() over (order by app.PA_Des) as pa_cod     " & vbCrLf)
        stb.Append("    , rrma.RMA_REG_COD " & vbCrLf)
        stb.Append("    , app.PA_Des  " & vbCrLf)
        stb.Append("    , SUBSTRING(app.CodiceEsterno, 11, 5) as CodiceEsterno " & vbCrLf)
        stb.Append("    , 'sitoblu123' " & vbCrLf)
        stb.Append("    , 'sitoblu123' " & vbCrLf)
        stb.Append(" from rma_appoggio_importazione app " & vbCrLf)
        stb.Append(" inner join RegolamentiRMA rrma  " & vbCrLf)
        stb.Append("        on app.Directive + ' Entry into force: ' + convert( varchar(1000), app.Entry_force, 103) = rrma.RMA_REG_DES  " & vbCrLf)
        stb.Append("        -- per i nuovi regolamenti appena inseriti TabellaRMA_COD è NULL. altrimenti è sempre valorizzato." & vbCrLf)
        stb.Append("        and rrma.TabellaRMA_COD is null " & vbCrLf)
        stb.Append(" where Not exists ( " & vbCrLf)
        stb.Append("  select 1  " & vbCrLf)

        'stb.Append("  From PrincipiAttiviRMA paRMA  " & vbCrLf)

        stb.AppendLine("  from ( ")

        stb.AppendLine("  select  ")
        stb.AppendLine("    rrma.RMA_REG_COD ")
        stb.AppendLine("  , rrma.TabellaRMA_COD ")
        stb.AppendLine("  , parma.pa_Cod ")
        stb.AppendLine("  , parma.pa_Des ")
        stb.AppendLine("  , parma.CodiceEsterno ")
        stb.AppendLine("  from principiattivirma parma ")
        stb.AppendLine("  inner join regolamentiRma rrma ")
        stb.AppendLine("      on parma.RMA_REG_COD = rrma.RMA_REG_COD")
        stb.AppendLine("  inner join TabellaRMA tarma ")
        stb.AppendLine("      on tarma.TabellaRMA_COD = rrma.TabellaRMA_COD")
        stb.AppendLine("  where tarma.FornitoreDati = " & FornitoreDati)
        stb.AppendLine("  and rrma.rma_reg_cod_padre is null ")
        stb.Append("  ) PARMA " & vbCrLf)

        stb.Append("  Where ( " & vbCrLf)
        stb.Append("     (paRMA.CodiceEsterno Is null And paRMA.PA_DES = app.PA_Des) " & vbCrLf)
        stb.Append("    Or (paRMA.CodiceEsterno Is Not null And paRMA.CodiceEsterno = SUBSTRING(app.CodiceEsterno, 11, 5)) " & vbCrLf)
        stb.Append("  ) " & vbCrLf)
        stb.Append(" )" & vbCrLf)


        '--------------------------------------------------------------------------
        xRisp = EseguiQuery_ScritturaNum(objParametri, stb.ToString, NomeRoutine, contatoreRecord)
        '--------------------------------------------------------------------------
        MessaggioDiRitorno &= NomeRoutine & ". Numero di righe elaborate: " & contatoreRecord & vbCrLf



        NomeRoutine = bNomeRoutine & " 6. inserimento degli rma"

        stb.Length = 0

        '' VAnni: 18/10/2019: Homologa, derr_codifica va letto e riportato in tabella rma_appoggio_Importazione
        stb.Append(" insert DerrateRMAxPrincipiAttiviRMA(DERR_COD, DERR_CODIFICA, PA_COD, RMA_REG_COD, RMA, Armonizzato, Minimo_misurabile, DATA_AGG, username_creazione, username_modifica, note) " & vbCrLf)
        stb.Append(" Select distinct " & vbCrLf)
        stb.Append("      app.Derr_Cod " & vbCrLf)
        stb.Append("    , app.derr_codifica " & vbCrLf)
        stb.Append("    , parma.PA_COD " & vbCrLf)
        stb.Append("    , parma.rma_reg_Cod " & vbCrLf)
        stb.Append("    , app.RMA " & vbCrLf)
        stb.Append("    , 0 as armonizzato " & vbCrLf)
        stb.Append("    , ABS( app.Normalizzato) as minimo_misurabile " & vbCrLf)
        stb.Append("    , app.Entry_force as data_agg " & vbCrLf)
        stb.Append("    , 'sitoblu123' " & vbCrLf)
        stb.Append("    , 'sitoblu123' " & vbCrLf)
        stb.Append("    , app.Annotazioni " & vbCrLf)
        stb.Append(" from rma_appoggio_importazione app " & vbCrLf)
        stb.Append(" inner join RegolamentiRMA rrma  " & vbCrLf)
        stb.Append("        on app.Directive + ' Entry into force: ' + convert( varchar(1000), app.Entry_force, 103) = rrma.RMA_REG_DES  " & vbCrLf)
        stb.Append("        -- per i nuovi regolamenti appena inseriti TabellaRMA_COD è NULL. altrimenti è sempre valorizzato." & vbCrLf)
        stb.Append("        and rrma.TabellaRMA_COD is null " & vbCrLf)
        stb.Append("  " & vbCrLf)

        'stb.AppendLine("  inner Join principiAttiviRMA paRMA  ")

        stb.AppendLine("  inner join ( ")
        stb.AppendLine("  select distinct ")
        stb.AppendLine("    rrma.RMA_REG_COD ")
        stb.AppendLine("  , rrma.TabellaRMA_COD ")
        stb.AppendLine("  , parma.pa_Cod ")
        stb.AppendLine("  , parma.pa_Des ")
        stb.AppendLine("  , parma.CodiceEsterno ")
        stb.AppendLine("  from principiattivirma parma ")
        'stb.AppendLine("  inner join regolamentiRma rrma ")

        ' VAnni: 21/1/2021: per dedurre le tabelle di rma dove al momento è ancora null il valore della colonna TabellaRMA_COD sui nuovi regolamenti introdotti leggo questo dato
        '                   dal regolamento padre

        stb.AppendLine(" inner Join ( ")
        stb.AppendLine("     select d.tabellaRMA_COD, rrmaFigli.RMA_REG_COD     ")
        stb.AppendLine("     from( ")
        stb.AppendLine("         select distinct TabellaRMA_COD, Rma_Reg_Cod_Padre ")
        stb.AppendLine("         From RMA_Appoggio_Importazione ")
        stb.AppendLine("     ) d ")
        stb.AppendLine("     inner Join RegolamentiRMA rrmaFigli   ")
        stb.AppendLine("     On rrmaFigli.Rma_Reg_Cod_Padre = d.Rma_Reg_Cod_Padre ")
        stb.AppendLine("     where rrmaFigli.TabellaRMA_COD Is null ")
        stb.AppendLine("     union ")
        stb.AppendLine("         Select TabellaRMA_Cod, rma_reg_Cod ")
        stb.AppendLine("     From RegolamentiRMA ")
        stb.AppendLine("     Where TabellaRMA_COD Is Not null ")
        stb.AppendLine(" ) rrma")


        stb.AppendLine("      on parma.RMA_REG_COD = rrma.RMA_REG_COD")
        stb.AppendLine("  inner join TabellaRMA tarma ")
        stb.AppendLine("      on tarma.TabellaRMA_COD = rrma.TabellaRMA_COD")
        stb.AppendLine("  where tarma.FornitoreDati = " & FornitoreDati)        
        stb.AppendLine("  ) paRMA  ")

        stb.AppendLine("  On ( ")
        stb.AppendLine("  ( paRMA.CodiceEsterno Is null And paRMA.PA_DES = app.PA_Des ) ")
        stb.AppendLine("    Or (paRMA.CodiceEsterno Is Not null And paRMA.CodiceEsterno = SUBSTRING(app.CodiceEsterno, 11, 5)) ")
        stb.AppendLine("  )  ")
        stb.AppendLine("     And paRMA.RMA_REG_COD = rrma.RMA_REG_COD ")

        stb.Append("    inner join DerrateRMA derRma " & vbCrLf)
        stb.Append("        on derRma.derr_cod = app.Derr_Cod  " & vbCrLf)
        stb.Append("        and derRma.DERR_CODIFICA = app.Derr_Codifica " & vbCrLf)
        stb.Append("  " & vbCrLf)

        '--------------------------------------------------------------------------
        xRisp = EseguiQuery_ScritturaNum(objParametri, stb.ToString, NomeRoutine, contatoreRecord)
        '--------------------------------------------------------------------------
        MessaggioDiRitorno &= NomeRoutine & ". Numero di righe elaborate: " & contatoreRecord & vbCrLf


        NomeRoutine = bNomeRoutine & "7.  Riporta sul regolamento padre i principi attivi che in esso non sono presenti"

        stb.Length = 0

        '' VAnni: 18/10/2019: Homologa, RMA_REG_COD va letto e riportato in tabella rma_appoggio_Importazione
        stb.Append(" insert PrincipiAttiviRMA (PA_COD, RMA_REG_COD, PA_DES, CodiceEsterno, inviato, Username_Creazione, Username_Modifica)  " & vbCrLf)
        stb.Append(" Select a.PA_COD, a.RMA_REG_COD_PADRE, a.PA_DES, a.CodiceEsterno, -max(a.RMA_REG_COD), 'sitoblu123', 'sitoblu123' ")
        stb.Append(" from( ")
        stb.Append("  select distinct parma.PA_COD, parma.pa_Des, parma.CodiceEsterno, rrma.RMA_REG_COD, rrma.RMA_REG_COD_PADRE, rrma.TabellaRMA_COD ")
        stb.Append("     From PrincipiAttiviRMA parma ")
        stb.Append("      inner Join RegolamentiRMA rrma ")
        stb.Append("             On rrma.RMA_REG_COD = parma.RMA_REG_COD       ")
        stb.Append("  where rrma.RMA_REG_COD_PADRE Is Not null     ")
        stb.Append(" ) a ")
        stb.Append("  Left Join PrincipiAttiviRMA mpaRMA  ")
        stb.Append("         On mpaRMA.PA_COD = a.PA_COD   ")
        stb.Append("      And mpaRMA.RMA_REG_COD = a.RMA_REG_COD_PADRE ")
        stb.Append(" where mparma.PA_COD Is null ")
        stb.Append(" group by  a.RMA_REG_COD_PADRE, a.PA_COD, a.PA_DES, a.CodiceEsterno ")


        '--------------------------------------------------------------------------
        xRisp = EseguiQuery_ScritturaNum(objParametri, stb.ToString, NomeRoutine, contatoreRecord)
        '--------------------------------------------------------------------------
        MessaggioDiRitorno &= NomeRoutine & ". Numero di righe elaborate: " & contatoreRecord & vbCrLf


        NomeRoutine = bNomeRoutine & "8. Riporta sul regolamento padre le decodifiche agronica per i principi attivi che in esso non sono presenti"

        stb.Length = 0


        stb.Append(" insert PrincipiAttivixPrincipiAttiviRMA(PA_COD, PA_COD_RMA, RMA_REG_COD, Username_Creazione, Username_Modifica) " & vbCrLf)
        stb.AppendLine(" Select papaRMA.PA_COD, paRMA1.PA_COD, rrma.RMA_REG_COD_PADRE As RMA_REG_COD, 'sitoblu123', 'sitoblu123'  ")
        stb.AppendLine(" From PrincipiAttiviRMA paRMA1 ")
        stb.AppendLine("    inner Join RegolamentiRMA rrma ")
        stb.AppendLine("         On -paRMA1.inviato = rrma.RMA_REG_COD         ")
        stb.AppendLine("     inner Join PrincipiAttivixPrincipiAttiviRMA papaRMA  ")
        stb.AppendLine("     On paRMA1.PA_COD = papaRMA.PA_COD_RMA   ")
        stb.AppendLine("     And paRMA1.inviato < 0   ")
        stb.AppendLine("     And -paRMA1.inviato = papaRMA.RMA_REG_COD   ")
        stb.AppendLine(" where Not exists( ")
        stb.AppendLine("     Select 1  ")
        stb.AppendLine("     From PrincipiAttivixPrincipiAttiviRMA papaRMA1  ")
        stb.AppendLine("     Where papaRMA.PA_COD = papaRMA1.PA_COD ")
        stb.AppendLine("     And papaRMA.PA_COD_RMA = papaRMA1.PA_COD_RMA  ")
        stb.AppendLine("     And papaRMA1.RMA_REG_COD = rrma.RMA_REG_COD_PADRE  ")
        stb.AppendLine(" ) ")



        '--------------------------------------------------------------------------
        xRisp = EseguiQuery_ScritturaNum(objParametri, stb.ToString, NomeRoutine, contatoreRecord)
        '--------------------------------------------------------------------------
        MessaggioDiRitorno &= NomeRoutine & ". Numero di righe elaborate: " & contatoreRecord & vbCrLf


        NomeRoutine = bNomeRoutine & "9. Riporta sul regolamento padre le decodifiche agronica per i le famiglie di principi attivi che in esso non sono presenti"

        stb.Length = 0


        stb.Append(" insert FamigliePrincipiAttiviXPrincipiAttiviRMA(fam_COD, PA_COD_RMA, RMA_REG_COD, Username_Creazione, Username_Modifica) " & vbCrLf)
        stb.Append(" select papaRMA.Fam_COD, paRMA1.PA_COD, rrma.RMA_REG_COD_PADRE, 'sitoblu123', 'sitoblu123' " & vbCrLf)
        stb.Append(" from PrincipiAttiviRMA paRMA1 " & vbCrLf)
        stb.AppendLine("    inner Join RegolamentiRMA rrma ")
        stb.AppendLine("         On -paRMA1.inviato = rrma.RMA_REG_COD         ")
        stb.Append("    inner join FamigliePrincipiAttiviXPrincipiAttiviRMA papaRMA " & vbCrLf)
        stb.Append("    on paRMA1.PA_COD = papaRMA.PA_COD_RMA  " & vbCrLf)
        stb.Append("    and paRMA1.inviato < 0  " & vbCrLf)
        stb.Append("    and -paRMA1.inviato = papaRMA.RMA_REG_COD  " & vbCrLf)
        stb.Append(" where not exists ( " & vbCrLf)
        stb.Append("  Select 1 " & vbCrLf)
        stb.Append("    from FamigliePrincipiAttiviXPrincipiAttiviRMA papaRMA1 " & vbCrLf)
        stb.Append("    where papaRMA.Fam_COD  = papaRMA1.Fam_COD  " & vbCrLf)
        stb.Append("    and   papaRMA.PA_COD_RMA = papaRMA1.PA_COD_RMA " & vbCrLf)
        stb.Append("    and   papaRMA1.RMA_REG_COD =  rrma.RMA_REG_COD_PADRE " & vbCrLf)
        stb.Append(" )   " & vbCrLf)


        '--------------------------------------------------------------------------
        xRisp = EseguiQuery_ScritturaNum(objParametri, stb.ToString, NomeRoutine, contatoreRecord)
        '--------------------------------------------------------------------------
        MessaggioDiRitorno &= NomeRoutine & ". Numero di righe elaborate: " & contatoreRecord & vbCrLf


        NomeRoutine = bNomeRoutine & "10. Riporta sul regolamento padre le RMA per i principi attivi che in esso non sono presenti"


        stb.Length = 0


        stb.Append(" insert DerrateRMAxPrincipiAttiviRMA(DERR_COD, DERR_CODIFICA, PA_COD, RMA_REG_COD, RMA, Armonizzato, Minimo_misurabile, Username_Creazione, Username_Modifica) " & vbCrLf)
        stb.Append(" select derRMA.DERR_COD, DerRMA.DERR_CODIFICA, derRMA.PA_COD, rrma.RMA_REG_COD_PADRE as rma_reg_Cod, derRMA.RMA, derRMA.Armonizzato, derRMA.Minimo_misurabile, 'sitoblu123', 'sitoblu123'  " & vbCrLf)
        stb.Append(" from PrincipiAttiviRMA paRMA1 " & vbCrLf)
        stb.AppendLine("    inner Join RegolamentiRMA rrma ")
        stb.AppendLine("         On -paRMA1.inviato = rrma.RMA_REG_COD         ")
        stb.Append("    inner join DerrateRMAxPrincipiAttiviRMA derRMA " & vbCrLf)
        stb.Append("    on paRMA1.PA_COD = derRMA.PA_COD  " & vbCrLf)
        stb.Append("    and paRMA1.inviato < 0  " & vbCrLf)
        stb.Append("    and -paRMA1.inviato = derRMA.RMA_REG_COD     " & vbCrLf)
        stb.Append(" where not exists ( " & vbCrLf)
        stb.Append("  Select 1 " & vbCrLf)
        stb.Append("    from DerrateRMAxPrincipiAttiviRMA derRMA1 " & vbCrLf)
        stb.Append("    where derRMA.PA_COD  = derRMA1.PA_COD " & vbCrLf)
        stb.Append("    and derRMA.Derr_Cod = derRMA1.Derr_cod " & vbCrLf)
        stb.Append("    and derRMA.DERR_CODIFICA = derRMA1.DERR_CODIFICA     " & vbCrLf)
        stb.Append("    and derRMA1.RMA_REG_COD =  rrma.RMA_REG_COD_PADRE  " & vbCrLf)
        stb.Append(" ) " & vbCrLf)


        '--------------------------------------------------------------------------
        xRisp = EseguiQuery_ScritturaNum(objParametri, stb.ToString, NomeRoutine, contatoreRecord)
        '--------------------------------------------------------------------------
        MessaggioDiRitorno &= NomeRoutine & ". Numero di righe elaborate: " & contatoreRecord & vbCrLf


        NomeRoutine = bNomeRoutine & "Ultime operazioni"

        NomeRoutine = bNomeRoutine & "Azzera flag per import principi Attivi"
        stb.Length = 0
        stb.Append("update PrincipiAttiviRMA  " & vbCrLf)
        stb.Append("set inviato = 0 " & vbCrLf)
        stb.Append(" where inviato < 0 " & vbCrLf)

        '--------------------------------------------------------------------------
        xRisp = EseguiQuery_ScritturaNum(objParametri, stb.ToString, NomeRoutine, contatoreRecord)
        '--------------------------------------------------------------------------
        MessaggioDiRitorno &= NomeRoutine & ". Numero di righe elaborate: " & contatoreRecord & vbCrLf



        NomeRoutine = bNomeRoutine & "aggiorna la colonna tabella TabellaRMA_COD nei nuovi record inseriti in RegolamentiRMA leggendo il dato dal regolamento Padre. "
        stb.Length = 0
        stb.AppendLine(" --aggiorna la colonna tabella TabellaRMA_COD nei nuovi record inseriti in RegolamentiRMA leggendo il dato dal regolamento Padre. ")
        stb.AppendLine(" --select * ")
        stb.AppendLine(" update rrmaFigli ")
        stb.AppendLine(" set TabellaRMA_COD = d.TabellaRMA_COD ")
        stb.AppendLine(" from ( ")
        stb.AppendLine("  select distinct TabellaRMA_COD, Rma_Reg_Cod_Padre ")
        stb.AppendLine("  from RMA_Appoggio_Importazione ")
        stb.AppendLine(" ) d ")
        stb.AppendLine(" inner join RegolamentiRMA rrmaFigli   ")
        stb.AppendLine("  on rrmaFigli.Rma_Reg_Cod_Padre = d.Rma_Reg_Cod_Padre ")
        stb.AppendLine(" where rrmaFigli.TabellaRMA_COD is null")

        '--------------------------------------------------------------------------
        xRisp = EseguiQuery_ScritturaNum(objParametri, stb.ToString, NomeRoutine, contatoreRecord)
        '--------------------------------------------------------------------------
        MessaggioDiRitorno &= NomeRoutine & ". Numero di righe elaborate: " & contatoreRecord & vbCrLf
    End Sub


    '##############################################################################################
    Public Function Scrivi(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                , Optional ByVal Data_creazione As Date = #2/1/1900# _
                , Optional ByVal Data_modifica As Date = #2/1/1900# _
                , Optional ByVal username_creazione As String = "" _
                , Optional ByVal username_modifica As String = ""
                ) As Boolean


        Dim NomeRoutine As String = "Scrivi()"

        '====================================================================================
        'Parametri opzionali :

        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

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



            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append(" INSERT ... " + vbCrLf)

            StrSQL.Append("              (")
            StrSQL.Append("              Inviato,            datainvio, ")
            StrSQL.Append("              Data_Creazione,     Data_Modifica, ")
            StrSQL.Append("              UserName_Creazione, UserName_Modifica, ")
            StrSQL.Append("              Validita_Inizio,    Validita_Fine, ")
            StrSQL.Append("              Validazione, Data_Validazione, UserName_Validazione " + vbCrLf)
            StrSQL.Append("              ) ")

            StrSQL.Append(" VALUES ( ")



            StrSQL.Append("         , 0  " + vbCrLf)
            StrSQL.Append("         , Null  " + vbCrLf)

            StrSQL.Append("			, " & Agro_SQL_SaveDate(Data_creazione) & "  ")
            StrSQL.Append("			, " & Agro_SQL_SaveDate(Data_modifica) & "  ")
            StrSQL.Append("			,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            StrSQL.Append("			,'" & Agro_SQL_SaveText(username_modifica) & "' ")



            StrSQL.Append(") ")

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







    '#################################################################
    Public Function Cancella(ByVal xFiltroAggiuntivo As String,
                              ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                              ) As Boolean

        '----- Descrizione
        Dim NomeRoutine As String = "Cancella()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            StrSQL.Length = 0

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then
                StrSQL.Append(" UPDATE ... ")
                StrSQL.Append(" SET ")
                StrSQL.Append("         Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("         ,Data_Modifica= " & Agro_SQL_SaveDate(Date.Now) & " ")
                StrSQL.Append("         ,Inviato = -1 ")
                StrSQL.Append(" WHERE   1=1 ")
                StrSQL.Append(" AND     Inviato >= 0 ")
            Else
                StrSQL.Append(" DELETE FROM ... ")
                StrSQL.Append(" WHERE 1=1 ")
            End If
            '---------------------------------------------

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




End Class


