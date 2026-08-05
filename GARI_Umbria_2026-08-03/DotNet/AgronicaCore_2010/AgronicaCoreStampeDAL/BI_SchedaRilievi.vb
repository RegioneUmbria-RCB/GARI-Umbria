


Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.AgronicaCoreParametri

Public Class BI_SchedaRilievi_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function CaricaDSRilAvvAus(
            ByVal DataDa As DateTime,
            ByVal DataA As DateTime,
            ByVal IncludiSoloDatiRilevati As Boolean,
            ByVal xFiltroAggiuntivo As String,
            ByVal xOrderBy As String,
            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
            ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri
        ) As DataTable



        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCore_DAL.Leggi()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Dim NomeDB_Utenti As String
            NomeDB_Utenti = objParametri_Utenti.StringaConnessione.Split(";")(2)
            NomeDB_Utenti = NomeDB_Utenti.Split("=")(1)

            Stb.Length = 0


            'query x reperire i dati

            Stb.Append("select  " & vbCrLf)
            Stb.Append("      PIVA " & vbCrLf)
            Stb.Append("    , RagioneSociale " & vbCrLf)
            Stb.Append("    , Sa_Nome " & vbCrLf)
            Stb.Append("    , DataOperazione " & vbCrLf)
            Stb.Append("    , Veg_Des " & vbCrLf)
            Stb.Append("    , Cul_Des " & vbCrLf)
            Stb.Append("    , Sup_Ha " & vbCrLf)
            Stb.Append("    , Sup_Are " & vbCrLf)
            Stb.Append("    , FF_DES " & vbCrLf)
            Stb.Append("    , UDM_DES    " & vbCrLf)
            Stb.Append("    , Latitudine " & vbCrLf)
            Stb.Append("    , Longitudine " & vbCrLf)
            Stb.Append("    , Appezzamento_Indirizzo " & vbCrLf)
            Stb.Append("    , Appezzamento_Descrizione " & vbCrLf)
            Stb.Append("    , Av_Des " & vbCrLf)
            Stb.Append("    , Utente " & vbCrLf)
            Stb.Append("    , DescrizionePunto " & vbCrLf)
            Stb.Append("    , Progetto " & vbCrLf)
            Stb.Append("    , DescrizioneOperazione " & vbCrLf)
            Stb.Append("    , ff_Des2 " & vbCrLf)
            Stb.Append("    , NoteGenerali " & vbCrLf)
            Stb.Append("    , id_agenda " & vbCrLf)
            Stb.Append("    , ordine " & vbCrLf)
            Stb.Append("    , max(qta) as qta " & vbCrLf)
            Stb.Append("    , max(qta_descrittiva) as qta_Descrittiva " & vbCrLf)

            Stb.Append(" FROM ( ")

            Stb.Append("      SELECT  " & vbCrLf)
            Stb.Append("      op.PIVA " & vbCrLf)
            Stb.Append("    , isNull(i.rag_soc, '') AS RagioneSociale " & vbCrLf)
            Stb.Append("    , isNull(sa.sa_nome, '') as Sa_Nome " & vbCrLf)
            Stb.Append("    , op.Data_Movimento AS DataOperazione " & vbCrLf)
            Stb.Append("    , isNull(SpecieVegetali.Veg_Des, '') as Veg_Des " & vbCrLf)
            Stb.Append("    , isNull(Cultivar.Cul_Des, '') as Cul_Des " & vbCrLf)
            Stb.Append("    , CAST(op.Sup_Imp AS integer) AS Sup_Ha " & vbCrLf)
            Stb.Append("    , CAST((op.Sup_Imp - CAST(op.Sup_Imp AS integer)) * 100 AS integer) AS Sup_Are " & vbCrLf)
            Stb.Append("    , isNull( ff.FF_DES , '') as FF_DES " & vbCrLf)

            If IncludiSoloDatiRilevati Then
                Stb.AppendLine(", case when mav.av_cod = 99999 then '_Ril. Avv. Si/No' else ISNULL(mav.Av_Des_Vol collate Latin1_General_CI_AS, '') + ' ' + ISNULL(mav.UDM_DES, '') end AS UDM_DES")
                Stb.AppendLine(", op.Qta")
            Else
                Stb.Append("    , case when mav1.cod = mav.cod then  " & vbCrLf)
                Stb.Append("            case when mav.av_cod = 99999 then '_Ril. Avv. Si/No' else ISNULL(mav.Av_Des_Vol collate Latin1_General_CI_AS, '') + ' ' + ISNULL(mav.UDM_DES, '') end " & vbCrLf)
                Stb.Append("        else " & vbCrLf)
                Stb.Append("            case when mav1.av_cod = 99999 then '_Ril. Avv. Si/No' else ISNULL(mav1.Av_Des_Vol collate Latin1_General_CI_AS, '') + ' ' + ISNULL(mav1.UDM_DES, '') end " & vbCrLf)
                Stb.Append("        end AS UDM_DES " & vbCrLf)
                Stb.Append("    , case when mav1.cod = mav.cod then  op.Qta else '' end as Qta " & vbCrLf)
            End If





            Stb.Append("    , isnull( G.lat , 0.0 ) AS Latitudine " & vbCrLf)
            Stb.Append("    , isNull( G.lon , 0.0) AS Longitudine " & vbCrLf)
            Stb.Append("    , COALESCE (op.Via_Stringa, '') AS Appezzamento_Indirizzo " & vbCrLf)
            Stb.Append("    , op.APP_NOME AS Appezzamento_Descrizione " & vbCrLf)

            If IncludiSoloDatiRilevati Then
                Stb.AppendLine(", case when mav.av_cod = 99999 then '_Ril. Avv. Si/No' else ISNULL(mav.Av_Des_Vol collate Latin1_General_CI_AS, '') + ' ' + ISNULL(mav.UDM_DES, '') end as AV_DES")

            Else
                Stb.Append("    , case when mav1.cod = mav.cod then  " & vbCrLf)
                Stb.Append("            case when mav.av_cod = 99999 then '_Ril. Avv. Si/No' else ISNULL(mav.Av_Des_Vol collate Latin1_General_CI_AS, '') + ' ' + ISNULL(mav.UDM_DES, '') end " & vbCrLf)
                Stb.Append("        else " & vbCrLf)
                Stb.Append("            case when mav1.av_cod = 99999 then '_Ril. Avv. Si/No' else ISNULL(mav1.Av_Des_Vol collate Latin1_General_CI_AS, '') + ' ' + ISNULL(mav1.UDM_DES, '') end " & vbCrLf)
                Stb.Append("        end AS  Av_Des " & vbCrLf)

            End If


            Stb.Append("    , ISNULL((Dettagli.Cognome + ' ' + Dettagli.Nome), 'N.D.') AS Utente  " & vbCrLf)
            Stb.Append("    , isnull( G.[TEXT], '') AS DescrizionePunto " & vbCrLf)
            Stb.Append("    , case when op.Piezo1 = 0 or op.Piezo1 = 1 then 'Cv Docg' else case when op.Piezo1 = 2 then  'Pendenze' else 'Testimone Fen.' end end as Progetto  " & vbCrLf)
            Stb.Append("    , op.Des_Lib as DescrizioneOperazione " & vbCrLf)
            Stb.Append("    , isNull(ff2.ff_Des, '' ) as ff_Des2 " & vbCrLf)
            Stb.Append("    , isNull(op.NoteGenerali , '') as NoteGenerali" & vbCrLf)
            Stb.Append("  " & vbCrLf)

            If IncludiSoloDatiRilevati Then
                Stb.AppendLine(", coalesce(anag.anag_des, cast( op.qta as varchar(1000) ) ) as qta_Descrittiva ")

            Else
                Stb.Append("    , case when mav1.cod = mav.cod then  " & vbCrLf)
                Stb.Append("        coalesce(anag.anag_des, cast( op.qta as varchar(1000) ) ) " & vbCrLf)
                Stb.Append("    else " & vbCrLf)
                Stb.Append("        '' " & vbCrLf)
                Stb.Append("    end AS qta_Descrittiva " & vbCrLf)

            End If

            Stb.Append("  " & vbCrLf)
            Stb.Append("    , op.id_agenda " & vbCrLf)

            If IncludiSoloDatiRilevati Then
                Stb.Append("    , mav.ordine " & vbCrLf)
            Else
                Stb.Append("    , mav1.ordine " & vbCrLf)
            End If


            Stb.Append(" FROM  " & vbCrLf)
            Stb.Append("  " & vbCrLf)
            Stb.Append(" ( " & vbCrLf)
            Stb.Append("    select mav.cod, Avversita.Av_Cod, Avversita.Abbreviazione as Av_Des_Vol, UnitaMisura.UDM_COD, UnitaMisura.UDM_SIM as UDM_DES, mav.ordine   " & vbCrLf)
            Stb.Append("    from  " & vbCrLf)
            Stb.Append("    MisuraxAvversita mav  " & vbCrLf)
            Stb.Append("  " & vbCrLf)
            Stb.Append("    INNER JOIN  Avversita ON mav.Av_Cod = Avversita.Av_Cod  " & vbCrLf)
            Stb.Append("  " & vbCrLf)
            Stb.Append("    INNER JOIN UnitaMisura ON mav.Udm_Cod = UnitaMisura.Udm_Cod  " & vbCrLf)
            Stb.Append(" ) mav " & vbCrLf)
            Stb.Append(" inner join  " & vbCrLf)
            Stb.Append(" ( " & vbCrLf)
            Stb.Append("  " & vbCrLf)
            Stb.Append("    select  " & vbCrLf)
            Stb.Append("        Agenda.piva " & vbCrLf)
            Stb.Append("        , Agenda.Sa_Cod " & vbCrLf)
            Stb.Append("        , Agenda.id_agenda " & vbCrLf)
            Stb.Append("        , Agenda.lav_cod " & vbCrLf)
            Stb.Append("        , Agenda.inviato " & vbCrLf)
            Stb.Append("        , Mov_Destinazioni.mov_destinazioni_graphickey " & vbCrLf)
            Stb.Append("        , Mov_Dettaglio_Tecnico.Av_Cod " & vbCrLf)
            Stb.Append("        , Movimenti_dettagli.Udm_Cod " & vbCrLf)
            Stb.Append("        , Reg_Impianti.CUL_COD " & vbCrLf)
            Stb.Append("        , Mov_Dettaglio_Tecnico.FF_Classe " & vbCrLf)
            Stb.Append("        , Movimenti.Data_Movimento " & vbCrLf)
            Stb.Append("        , Reg_Impianti.Sup_Imp " & vbCrLf)
            Stb.Append("        , Mov_Destinazioni.Qta " & vbCrLf)
            Stb.Append("        , Appezzamento.Via_Stringa " & vbCrLf)
            Stb.Append("        , Appezzamento.APP_NOME " & vbCrLf)
            Stb.Append("        , Agenda.Username_Modifica " & vbCrLf)
            Stb.Append("        , mov_dettaglio_tecnico.Piezo1 " & vbCrLf)
            Stb.Append("        , mov_dettaglio_tecnico.Piezo2 as FF_Classe2 " & vbCrLf)
            Stb.Append("        , Agenda.Des_Lib " & vbCrLf)
            Stb.Append("        , Reg_Impianti.Appezza " & vbCrLf)
            Stb.Append("        , Reg_Impianti.id_reg " & vbCrLf)
            Stb.Append("        , Reg_Impianti.Validita_Inizio " & vbCrLf)
            Stb.Append("        , Reg_Impianti.Validita_Fine " & vbCrLf)
            Stb.Append("        , Agenda.Validita_Inizio as Agenda_Validita_Inizio " & vbCrLf)
            Stb.Append("        , Agenda.Validita_Fine as Agenda_Validita_Fine " & vbCrLf)
            Stb.Append("        , Movimenti.Mov_Desc as NoteGenerali " & vbCrLf)
            Stb.Append("    FROM Agenda  " & vbCrLf)
            Stb.Append("    INNER JOIN Movimenti  " & vbCrLf)
            Stb.Append("        ON Agenda.Id_Agenda = Movimenti.Id_Agenda  " & vbCrLf)
            Stb.Append("        AND Agenda.PIVA = Movimenti.PIVA  " & vbCrLf)
            Stb.Append("        AND Agenda.Sa_Cod = Movimenti.Sa_Cod " & vbCrLf)
            Stb.Append("     INNER JOIN Movimenti_dettagli  " & vbCrLf)
            Stb.Append("        ON Movimenti.Id_Mov = Movimenti_dettagli.Id_Mov  " & vbCrLf)
            Stb.Append("        AND Movimenti.PIVA = Movimenti_dettagli.PIVA  " & vbCrLf)
            Stb.Append("        AND Movimenti.Sa_Cod = Movimenti_dettagli.Sa_Cod  " & vbCrLf)
            Stb.Append("        AND Movimenti.Id_Agenda = Movimenti_dettagli.Id_Agenda  " & vbCrLf)
            Stb.Append("    INNER JOIN    Mov_Dettaglio_Tecnico  " & vbCrLf)
            Stb.Append("        ON Movimenti_dettagli.Id_Mov = Mov_Dettaglio_Tecnico.Id_Mov  " & vbCrLf)
            Stb.Append("        AND Movimenti_dettagli.PIVA = Mov_Dettaglio_Tecnico.Piva  " & vbCrLf)
            Stb.Append("        AND Movimenti_dettagli.Sa_Cod = Mov_Dettaglio_Tecnico.Sa_Cod  " & vbCrLf)
            Stb.Append("        AND Movimenti_dettagli.Id_Agenda = Mov_Dettaglio_Tecnico.Id_Agenda  " & vbCrLf)
            Stb.Append("        AND Movimenti_dettagli.Id_Mov_Det = Mov_Dettaglio_Tecnico.Id_Mov_Det  " & vbCrLf)
            Stb.Append("    INNER JOIN Mov_Destinazioni  " & vbCrLf)
            Stb.Append("        ON Movimenti_dettagli.PIVA = Mov_Destinazioni.Piva  " & vbCrLf)
            Stb.Append("        AND Movimenti_dettagli.Sa_Cod = Mov_Destinazioni.Sa_Cod  " & vbCrLf)
            Stb.Append("        AND Movimenti_dettagli.Id_Agenda = Mov_Destinazioni.Id_Agenda  " & vbCrLf)
            Stb.Append("        AND Movimenti_dettagli.Id_Mov = Mov_Destinazioni.Id_Mov  " & vbCrLf)
            Stb.Append("        AND Movimenti_dettagli.Id_Mov_Det = Mov_Destinazioni.Id_Mov_Det  " & vbCrLf)
            Stb.Append("    INNER JOIN Appezzamento  " & vbCrLf)
            Stb.Append("        ON Mov_Destinazioni.Piva = Appezzamento.PIVA  " & vbCrLf)
            Stb.Append("        AND Mov_Destinazioni.Sa_Cod = Appezzamento.SA_COD  " & vbCrLf)
            Stb.Append("        AND Mov_Destinazioni.Appezza = Appezzamento.APPEZZA  " & vbCrLf)
            Stb.Append("    INNER JOIN Reg_Impianti  " & vbCrLf)
            Stb.Append("        ON Mov_Destinazioni.Piva = Reg_Impianti.PIVA  " & vbCrLf)
            Stb.Append("        AND Mov_Destinazioni.Sa_Cod = Reg_Impianti.SA_COD  " & vbCrLf)
            Stb.Append("        AND Mov_Destinazioni.Id_Destinazione = Reg_Impianti.ID_REG  " & vbCrLf)
            Stb.Append("        AND Mov_Destinazioni.Appezza = Reg_Impianti.APPEZZA  " & vbCrLf)

            Stb.Append("    union all " & vbCrLf)

            Stb.Append("   select   " & vbCrLf)
            Stb.Append("           isNull(R.Piva , '') as Piva  " & vbCrLf)
            Stb.Append("         , isNull(R.sa_cod, 0 ) as Sa_Cod " & vbCrLf)
            Stb.Append("         , R.Ricetta_Cod as id_agenda " & vbCrLf)
            Stb.Append("         , Op.Lav_Cod  " & vbCrLf)
            Stb.Append("         , R.inviato  " & vbCrLf)
            Stb.Append("         , tec.Ricette_Dettaglio_Tecnico_GraphicKey " & vbCrLf)
            Stb.Append("         , tec.Av_Cod  " & vbCrLf)
            Stb.Append("         , D.Udm_Cod  " & vbCrLf)
            Stb.Append("         , rcul.CUL_COD  " & vbCrLf)
            Stb.Append("         , tec.FF_Classe  " & vbCrLf)
            Stb.Append("         , Op.Validita_Inizio as Data_Movimento " & vbCrLf)
            Stb.Append("         , 0 as Sup_Imp  " & vbCrLf)
            Stb.Append("         , tec.qta_ril as qta " & vbCrLf)
            Stb.Append("         , '' as Via_Stringa  " & vbCrLf)
            Stb.Append("         , '' as APP_NOME  " & vbCrLf)
            Stb.Append("         , R.Username_Modifica  " & vbCrLf)
            Stb.Append("         , tec.Piezo1  " & vbCrLf)
            Stb.Append("         , tec.Piezo2 as FF_Classe2  " & vbCrLf)
            Stb.Append("         , R.Ricetta_Des as Des_Lib " & vbCrLf)
            Stb.Append("         , 0 as Appezza  " & vbCrLf)
            Stb.Append("         , 0 as id_reg  " & vbCrLf)
            Stb.Append("         , R.Validita_Inizio  " & vbCrLf)
            Stb.Append("         , R.Validita_Fine  " & vbCrLf)
            Stb.Append("         , R.Validita_Inizio as Agenda_Validita_Inizio  " & vbCrLf)
            Stb.Append("         , R.Validita_Fine as Agenda_Validita_Fine  " & vbCrLf)
            Stb.Append("         , Op.note as NoteGenerali  " & vbCrLf)
            Stb.Append("  " & vbCrLf)
            Stb.Append("    from Ricette R " & vbCrLf)
            Stb.Append("    inner join Ricette_Operazioni Op " & vbCrLf)
            Stb.Append("        on R.Ricetta_Cod = Op.Ricetta_Cod " & vbCrLf)
            Stb.Append("        and R.Ricetta_SuperUser = Op.Ricetta_SuperUser " & vbCrLf)
            Stb.Append("    inner join Ricette_dettagli D " & vbCrLf)
            Stb.Append("        on D.Ricetta_Cod = Op.Ricetta_Cod " & vbCrLf)
            Stb.Append("        and D.Ricetta_SuperUser = Op.Ricetta_SuperUser " & vbCrLf)
            Stb.Append("  " & vbCrLf)
            Stb.Append("    inner join Ricette_Dettaglio_Tecnico tec " & vbCrLf)
            Stb.Append("        on D.Ricetta_Cod = tec.Ricetta_Cod " & vbCrLf)
            Stb.Append("        and D.Ricetta_SuperUser = tec.Ricetta_superUser " & vbCrLf)
            Stb.Append("        and D.Ricetta_Dettaglio_Cod = tec.Ricetta_Dettaglio_Cod " & vbCrLf)
            Stb.Append("  " & vbCrLf)
            Stb.Append("    left join RicettexCultivar rcul " & vbCrLf)
            Stb.Append("        on rcul.Ricetta_Cod = Op.Ricetta_Cod " & vbCrLf)
            Stb.Append("        and rcul.Ricetta_SuperUser = Op.Ricetta_SuperUser " & vbCrLf)
            Stb.Append(" ")


            Stb.Append(" ) op " & vbCrLf)
            Stb.Append("  " & vbCrLf)
            Stb.Append("  " & vbCrLf)
            Stb.Append(" on op.av_cod = mav.av_cod " & vbCrLf)
            Stb.Append(" and op.udm_cod = mav.udm_cod " & vbCrLf)

            Stb.AppendLine("  Left Join " & NomeDB_Utenti & ".dbo.Utenti_Dettagli Dettagli ")
            Stb.AppendLine("  On Dettagli.CodFisc = op.Username_Modifica ")


            If Not IncludiSoloDatiRilevati Then
                Stb.Append("  " & vbCrLf)
                Stb.Append(" left join  " & vbCrLf)
                Stb.Append("  " & vbCrLf)
                Stb.Append(" ( " & vbCrLf)
                Stb.Append("    select mav.cod, Avversita.Av_Cod, Avversita.Abbreviazione as Av_Des_Vol, UnitaMisura.UDM_COD, UnitaMisura.UDM_SIM as UDM_DES, mav.ordine   " & vbCrLf)
                Stb.Append("    from  " & vbCrLf)
                Stb.Append("    MisuraxAvversita mav  " & vbCrLf)
                Stb.Append("  " & vbCrLf)
                Stb.Append("    INNER JOIN  Avversita ON mav.Av_Cod = Avversita.Av_Cod  " & vbCrLf)
                Stb.Append("  " & vbCrLf)
                Stb.Append("    INNER JOIN UnitaMisura ON mav.Udm_Cod = UnitaMisura.Udm_Cod  " & vbCrLf)
                Stb.Append(" ) mav1 " & vbCrLf)
                Stb.Append(" on 1=1 " & vbCrLf)
                Stb.Append("  " & vbCrLf)
            End If


            Stb.Append(" LEFT JOIN Cultivar  " & vbCrLf)
            Stb.Append("        ON Cultivar.Cul_Cod = op.CUL_COD  " & vbCrLf)
            Stb.Append(" LEFT JOIN SpecieVegetali  " & vbCrLf)
            Stb.Append("    ON SpecieVegetali.Veg_Cod = Cultivar.Veg_Cod  " & vbCrLf)
            Stb.Append(" LEFT JOIN Imprese AS i  " & vbCrLf)
            Stb.Append("    ON i.PIVA = op.PIVA  " & vbCrLf)
            Stb.Append(" LEFT JOIN Centri_Aziendali AS sa  " & vbCrLf)
            Stb.Append("    ON sa.PIVA = op.PIVA  " & vbCrLf)
            Stb.Append("    AND sa.sa_cod = op.sa_cod  " & vbCrLf)
            Stb.Append(" LEFT OUTER JOIN FasiFenologiche AS ff ON ff.FF_COD = op.FF_Classe " & vbCrLf)
            Stb.Append(" LEFT OUTER JOIN FasiFenologiche AS ff2 ON ff2.FF_COD = op.FF_Classe2 " & vbCrLf)
            Stb.Append("  " & vbCrLf)
            Stb.Append(" LEFT OUTER JOIN " & vbCrLf)
            Stb.Append("     Grafica AS G ON G.Piva = op.PIVA AND G.Sa_Cod = op.Sa_Cod AND G.Id = op.mov_destinazioni_graphickey " & vbCrLf)
            Stb.Append("  " & vbCrLf)
            Stb.Append(" left join MisuraXAvversita_Anagrafiche anag " & vbCrLf)
            Stb.Append("    on anag.MxAV_Cod = mav.COD  " & vbCrLf)
            Stb.Append("    and anag.Anag_valore = op.Qta " & vbCrLf)

            Stb.Append(" WHERE 1=1 " & vbCrLf)

            Stb.Append(" AND op.Data_Movimento <= " & Agro_SQL_SaveDate(CDate(DataA)) & " ")
            Stb.Append(" AND op.Data_Movimento >= " & Agro_SQL_SaveDate(CDate(DataDa)) & " ")


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


            Stb.Append("  ) a " & vbCrLf)

            Stb.Append(" group by " & vbCrLf)
            Stb.Append("      PIVA " & vbCrLf)
            Stb.Append("    , RagioneSociale " & vbCrLf)
            Stb.Append("    , Sa_Nome " & vbCrLf)
            Stb.Append("    , DataOperazione " & vbCrLf)
            Stb.Append("    , Veg_Des " & vbCrLf)
            Stb.Append("    , Cul_Des " & vbCrLf)
            Stb.Append("    , Sup_Ha " & vbCrLf)
            Stb.Append("    , Sup_Are " & vbCrLf)
            Stb.Append("    , FF_DES " & vbCrLf)
            Stb.Append("    , UDM_DES " & vbCrLf)
            Stb.Append("    , Latitudine " & vbCrLf)
            Stb.Append("    , Longitudine " & vbCrLf)
            Stb.Append("    , Appezzamento_Indirizzo " & vbCrLf)
            Stb.Append("    , Appezzamento_Descrizione " & vbCrLf)
            Stb.Append("    , Av_Des " & vbCrLf)
            Stb.Append("    , Utente " & vbCrLf)
            Stb.Append("    , DescrizionePunto " & vbCrLf)
            Stb.Append("    , Progetto " & vbCrLf)
            Stb.Append("    , DescrizioneOperazione " & vbCrLf)
            Stb.Append("    , ff_Des2 " & vbCrLf)
            Stb.Append("    , NoteGenerali " & vbCrLf)
            Stb.Append("    , id_agenda " & vbCrLf)
            Stb.Append("    , ordine " & vbCrLf)
            Stb.Append(" ")


            If xOrderBy <> "" Then
                Stb.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else

                Stb.Append("ORDER BY a.ordine ")

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
