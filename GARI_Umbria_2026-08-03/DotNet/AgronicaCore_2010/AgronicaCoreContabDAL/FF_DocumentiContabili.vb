

Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.AgronicaCoreParametri

Public Class FF_DocumentiContabili_R
    Inherits AgronicaCoreDataProvider.DataProvider


    Public Function LeggiProdottoGiasDato_CODDISTU( _
                    ByVal cod_risum_S As String, _
                    ByVal CodDisTU As String, _
                    ByVal xFiltroAggiuntivo As String, _
                    ByVal xOrderBy As String, _
                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
            ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCore_DAL.Leggi()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Dim confezione As String = "confezione"
            Dim sottoconfezione As String = "sottoconfezione"
            Dim imballaggio As String = "imballaggio"

            Dim otpConfezione As String = "otpConfezione"
            Dim otpSottoconfezione As String = "otpSottoconfezione"
            Dim otpImballaggio As String = "otpImballaggio"

            Dim campionatureConfezione As String = "cConfezione"
            Dim campionatureSottoconfezione As String = "cSottoconfezione"
            Dim campionatureImballaggio As String = "cImballaggio"

            Dim GenerazioneConfezione As String = "ogenLogConfezione"
            Dim GenerazioneSottoconfezione As String = "ogenLogSottoconfezione"
            Dim GenerazioneImballaggio As String = "ogenLogImballaggio"

            Dim mpConfezione As String = "mpConfezione"


            stb.Length = 0
            stb.Append("  select  " & vbCrLf)

            stb.Append("   mDest.Mat_Cod " & vbCrLf)
            stb.Append(" , mDest.elem_cod " & vbCrLf)
            stb.Append(" , mp.mat_cod as mat_cod_alias " & vbCrLf)
            stb.Append(" , mp.elem_cod " & vbCrLf)

            stb.Append(" , case when " & campionatureConfezione & ".val_cod = '' then 1 else coalesce(" & campionatureConfezione & ".val_cod, 1) end as nConfezioniXSottoConfezione " & vbCrLf)
            stb.Append(" , case when " & campionatureSottoconfezione & ".val_cod = '' then 1 else coalesce(" & campionatureSottoconfezione & ".val_cod, 1) end as nNumeroSottoConfezioniXImballi " & vbCrLf)

            stb.Append(" , case when isnull(" & otpConfezione & ".Codice_Generazione_Link, 0) = 0 then isnull(" & otpConfezione & ".Mat_Cod_Generazione_Link, 0) else " & GenerazioneConfezione & ".mat_cod end as mat_cod_Confezione " & vbCrLf)
            stb.Append(" , isnull(" & otpConfezione & ".Descrizione, '') as Descrizione_Confezione " & vbCrLf)


            stb.Append(" , case when isnull(" & otpSottoconfezione & ".Codice_Generazione_Link, 0) = 0 then isnull(" & otpSottoconfezione & ".Mat_Cod_Generazione_Link, 0) else " & GenerazioneSottoconfezione & ".mat_cod end as mat_cod_SottoConfezione " & vbCrLf)
            stb.Append(" , isnull(" & otpSottoconfezione & ".Descrizione, '') as Descrizione_SottoConfezione " & vbCrLf)

            stb.Append(" , case when isnull(" & otpImballaggio & ".Codice_Generazione_Link, 0) = 0 then isnull(" & otpImballaggio & ".Mat_Cod_Generazione_Link, 0) else " & GenerazioneImballaggio & ".mat_cod end as mat_cod_Imballaggio " & vbCrLf)
            stb.Append(" , isnull(" & otpImballaggio & ".Descrizione, '') as Descrizione_Imballaggio " & vbCrLf)

            stb.Append(" , isnull(" & mpConfezione & ".qta_extra, 1) as qta_extra_confezione " & vbCrLf)
            stb.Append(" , isnull(" & mpConfezione & ".udm_cod_extra, 2) as udm_cod_extra_confezione " & vbCrLf)

            stb.Append("  from materie_prime_alias a  " & vbCrLf)
            stb.Append("     inner join Materie_Prime mp  " & vbCrLf)
            stb.Append("         on mp.mat_cod = a.mat_Cod_alias  " & vbCrLf)
            stb.Append("    inner join Materie_Prime mDest " & vbCrLf)
            stb.Append("        on mDest.Mat_Cod = a.Mat_Cod")


            FF_DataCore.leggiParametriOmniFF_FROM("Imballaggio", "mDest", stb)
            FF_DataCore.leggiParametriOmniFF_FROM("Confezione", "mDest", stb)
            FF_DataCore.leggiParametriOmniFF_FROM("Sottoconfezione", "mDest", stb)





            stb.Append("  " & vbCrLf)
            stb.Append("    where '" & cod_risum_S & "' in (select strname from dbo.fSplit(a.filtro_contatti, '|') )  " & vbCrLf)
            stb.Append("    and mp.Cod_Articolo = '" & Agro_SQL_SaveText(CodDisTU) & "' " & vbCrLf)
            stb.Append(" ")

            If xFiltroAggiuntivo <> "" Then
                stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If



            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    stb.Append(" AND   a.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    stb.Append(" AND   a.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                stb.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT





    End Function


    Public Function LeggiNumeroOrdine( _
                    ByVal Prefisso As String, _
                    ByVal dataRiferimento As DateTime, _
                    ByVal piva As String, _
                    ByVal EDI_Valore As String, _
                    ByVal xFiltroAggiuntivo As String, _
                    ByVal xOrderBy As String, _
                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
            ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCore_DAL.Leggi()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            stb.Length = 0
            stb.Append(" Select max(m.Doc_Numero) + 1 " & vbCrLf)
            stb.Append(" from agenda a  " & vbCrLf)
            stb.Append("    inner join movimenti m  " & vbCrLf)
            stb.Append("        on a.Id_Agenda = m.Id_Agenda " & vbCrLf)
            stb.Append("        and a.piva = m.piva  " & vbCrLf)
            stb.Append(" where year(m.Data_Movimento) = year(" & Agro_SQL_SaveDate(dataRiferimento) & ") " & vbCrLf)
            stb.Append(" and a.Lav_Cod = 2002 " & vbCrLf)
            stb.Append(" and m.Doc_Numero_Sin = '" & Agro_SQL_SaveText(Prefisso) & "' " & vbCrLf)



            If xFiltroAggiuntivo <> "" Then
                stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If



            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    stb.Append(" AND   a.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    stb.Append(" AND   a.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                stb.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT





    End Function

    Public Function LeggiXDatiEDI( _
                    ByVal enum_codiceAnagrafe As enum_CodiciAnagrafe, _
                    ByVal dataRiferimento As DateTime, _
                    ByVal piva As String, _
                    ByVal EDI_Valore As String, _
                    ByVal xFiltroAggiuntivo As String, _
                    ByVal xOrderBy As String, _
                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
            ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCore_DAL.Leggi()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            stb.Length = 0

            If enum_codiceAnagrafe = enum_CodiciAnagrafe.INDICODE_EDI_Euritmo_Codice_punto_consegna_NAD Or _
               enum_codiceAnagrafe = enum_CodiciAnagrafe.INDICODE_EDI_Euritmo_Codice_magazzino_emissione_ordine_NAB Then

                stb.Append(" select top 1 risum.*, cxi.cod_indirizzo as cod_indirizzo_risum " & vbCrLf)
                stb.Append(" from ContattiXIndirizzi cxi " & vbCrLf)
                stb.Append("    inner join Indirizzi ii " & vbCrLf)
                stb.Append("        on cxi.Cod_Indirizzo = ii.cod_indirizzo " & vbCrLf)
                stb.Append("    inner join risorse_umane risum " & vbCrLf)
                stb.Append("        on risum.Cod_Contatto = cxi.Cod_Contatto " & vbCrLf)
                stb.Append("  and risum.Piva = cxi.Piva " & vbCrLf)

                If enum_codiceAnagrafe = enum_CodiciAnagrafe.INDICODE_EDI_Euritmo_Codice_punto_consegna_NAD Then
                    stb.Append(" where  ii.INDICODE_EDI_NAD = '" & Agro_SQL_SaveText(EDI_Valore) & "' " & vbCrLf)
                Else
                    stb.Append(" where  ii.INDICODE_EDI_NAB = '" & Agro_SQL_SaveText(EDI_Valore) & "' " & vbCrLf)
                End If


            End If


            If enum_codiceAnagrafe = enum_CodiciAnagrafe.INDICODE_EDI_Euritmo_Codice_fornitore_NAS_CodForn Then

                stb.Append(" select top 1 risum.*, ci.cod_indirizzo as cod_indirizzo_risum " & vbCrLf)
                stb.Append(" from contatti_codici c " & vbCrLf)
                stb.Append("    inner join risorse_umane risum  " & vbCrLf)
                stb.Append("        on c.Cod_Contatto = risum.Cod_Contatto " & vbCrLf)
                stb.Append("        and c.PIVA = risum.piva " & vbCrLf)
                stb.Append("        and c.Id_cod = " & enum_codiceAnagrafe & vbCrLf)
                stb.Append("        and c.val_cod = '" & Agro_SQL_SaveText(EDI_Valore) & "' " & vbCrLf)
                'stb.Append("        AND c.piva = '" & Agro_SQL_SaveText(piva) & "' " & vbCrLf)
                stb.Append("    inner join rapporti_contabili cont " & vbCrLf)
                stb.Append("        on cont.Cod_Rapporto = risum.Cod_Rapporto " & vbCrLf)
                stb.Append("        and cont.Piva = '" & objParametri.PivaSuperUser & "' " & vbCrLf)
                stb.Append("    inner join ContattiXIndirizzi ci " & vbCrLf)
                stb.Append("        on ci.Cod_Contatto = c.Cod_Contatto " & vbCrLf)
                stb.Append("        and ci.Piva = c.PIVA " & vbCrLf)
                stb.Append("        and ci.Tipo_Indirizzo = " & INDIRIZZO_SEDE_LEGALE)


                stb.Append(" where cont.Cliente = 1    " & vbCrLf)
                stb.Append("  " & vbCrLf)
                stb.Append(" and risum.validita_inizio <= " & Agro_SQL_SaveDate(dataRiferimento) & "  " & vbCrLf)
                stb.Append(" and risum.Validita_Fine >=  " & Agro_SQL_SaveDate(dataRiferimento) & " ")

            End If

            If stb.Length = 0 Then
                Throw New Exception("chiamata alla funzione LeggiXDatiEDI non valida")
            End If

            If xFiltroAggiuntivo <> "" Then
                stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If




            If xOrderBy <> "" Then
                stb.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT





    End Function


    Public Function LeggiFattureTestata( _
                    ByVal Piva As String, _
                    ByVal Lav_Cod As Integer, _
                    ByVal Id_Agenda As Integer, _
                    ByVal Data_Inizio As Date, _
                    ByVal Data_Fine As Date, _
                    ByVal Cod_RisUm As Int32, _
                    ByVal Anno As Integer, _
                    ByVal Data_Movimento As Date, _
                    ByVal Scadenza As Date, _
                    ByVal Doc_Numero_Sin As String, _
                    ByVal Doc_Numero As Decimal, _
                    ByVal Doc_Numero_Des As String, _
                    ByVal Progr_Protocollo As Integer, _
                    ByVal Progr_Registrazione As Integer, _
                    ByVal Data_Registrazione As Date, _
                    ByVal flag_Solo_Prodotti_BdGias As Boolean, _
                    ByVal flag_AltriBeniStrumentali As Boolean, _
                    ByVal flag_Mov_Dettaglio_Tecnico_Extra As Boolean, _
                    ByVal blocco_flag As Integer, _
                    ByVal filtro_doc As String, _
                    ByVal xFiltroAggiuntivo As String, _
                    ByVal xOrderBy As String, _
                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                        ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCore_DAL.Leggi()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try



            stb.Length = 0
            stb.Append("SELECT " & vbCrLf)
            stb.Append("    --Testata  " & vbCrLf)

            stb.Append("    --Dati generici  " & vbCrLf)

            stb.Append("      a.id_agenda  " & vbCrLf)

            stb.Append("    --Dati del documento  " & vbCrLf)
            stb.Append("    , a.piva  " & vbCrLf)
            stb.Append("    , a.des_lib  " & vbCrLf)
            stb.Append("    , mTes.Data_Movimento " & vbCrLf)
            stb.Append("    , mTes.Data_Registrazione " & vbCrLf)
            stb.Append("    , mTes.Scadenza " & vbCrLf)
            stb.Append("    , mTes.Doc_Numero " & vbCrLf)
            stb.Append("    , mTes.Doc_Numero_Sin " & vbCrLf)
            stb.Append("    , mTes.Doc_Numero_Des " & vbCrLf)
            stb.Append("    , mTes.Mov_Desc " & vbCrLf)
            stb.Append("    , mTes.id_mov " & vbCrLf)

            stb.Append(" from Agenda A " & vbCrLf)
            stb.Append("    inner join Movimenti MTes " & vbCrLf)
            stb.Append("        on MTes.Id_Agenda= A.Id_Agenda " & vbCrLf)
            stb.Append("        AND MTes.Piva= A.Piva " & vbCrLf)
            stb.Append("        AND MTes.sa_cod= A.sa_cod " & vbCrLf)
            stb.Append("        and MTes.Cau_Mov = '4000' " & vbCrLf)

            Dim lClausolaBloccoFlag As String = " A.Blocco_Flag = " & blocco_flag & " and "

            If blocco_flag <> "0" Then
                lClausolaBloccoFlag = ""
            End If

            stb.Append("        where  " & lClausolaBloccoFlag & " A.lav_cod =  " & LAVCOD_FATTURA_EMESSA & vbCrLf)

            If Data_Inizio <> AGRODATAINIZIO Then
                stb.Append(" AND MTes.Data_Movimento >= " & Agro_SQL_SaveDate(Data_Inizio) & vbCrLf)
            End If

            If Data_Fine <> AGRODATAFINE Then
                stb.Append(" AND MTes.Data_Movimento <= " & Agro_SQL_SaveDate(Data_Fine) & vbCrLf)
            End If

            If Cod_RisUm <> 0 Then
                stb.Append(" AND MTes.cod_risum =  " & Cod_RisUm & vbCrLf)
            End If

            If Not filtro_doc.Contains("-") Then
                stb.Append(" AND MTes.Doc_Numero " & filtro_doc & " " & Agro_SQL_SaveNum(Doc_Numero) & vbCrLf)
            End If

            If xFiltroAggiuntivo <> "" Then
                stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If



            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    stb.Append(" AND   a.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    stb.Append(" AND   a.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                stb.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If


            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT





    End Function

    Public Function LeggiFatture( _
                    ByVal filtro_id_agenda As String, _
                    ByVal xFiltroAggiuntivo As String, _
                    ByVal xOrderBy As String, _
                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
            ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCore_DAL.Leggi()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try



            stb.Length = 0

            stb.Append("SELECT " & vbCrLf)
            stb.Append("    --Testata  " & vbCrLf)

            stb.Append("    --Dati generici  " & vbCrLf)

            stb.Append("      a.id_agenda  " & vbCrLf)

            stb.Append("    --Dati del documento  " & vbCrLf)
            stb.Append("    , a.piva  " & vbCrLf)
            stb.Append("    , a.des_lib  " & vbCrLf)
            stb.Append("    , mTes.Data_Movimento " & vbCrLf)
            stb.Append("    , mTes.Data_Registrazione " & vbCrLf)
            stb.Append("    , mTes.Scadenza " & vbCrLf)
            stb.Append("    , mTes.Doc_Numero " & vbCrLf)
            stb.Append("    , mTes.Doc_Numero_Sin " & vbCrLf)
            stb.Append("    , mTes.Doc_Numero_Des " & vbCrLf)
            stb.Append("    , mTes.Mov_Desc " & vbCrLf)
            stb.Append("    , mTes.id_mov " & vbCrLf)


            stb.Append("    --Decodifiche NAS  " & vbCrLf)
            stb.Append("    , ic_NAS_CodForn.Val_cod as NAS_CODFORN " & vbCrLf)
            stb.Append("    , ic_NAS_QCodForn.val_cod as  NAS_CODQFORN " & vbCrLf)
            stb.Append("    , ic_Nas_CCiAA.val_cod as ic_Nas_CCiAA " & vbCrLf)
            stb.Append("    , ic_Nas_Tribunale.val_cod as ic_Nas_Tribunale " & vbCrLf)
            stb.Append("    --fine Decodifiche NAS  " & vbCrLf)

            stb.Append("    --FTX  " & vbCrLf)
            stb.Append("    , MTes.extra_str as Note_Fattura " & vbCrLf)
            stb.Append("    --FTX  " & vbCrLf)

            stb.Append("    --Dati del fornitore  " & vbCrLf)
            stb.Append("    , c_Fornitore_CF.Codice_Fiscale as fornitore_Codice_fiscale " & vbCrLf) 'c_Fornitore_CF
            stb.Append("    , fornitore_imprese.piva as fornitore_Piva " & vbCrLf)
            stb.Append("    , fornitore_imprese.rag_soc as fornitore_Rag_soc " & vbCrLf)
            stb.Append("    , Fornitore_Indirizzi.ind_des AS Fornitore_Indirizzo " & vbCrLf)
            stb.Append("    , ISNULL(Fornitore_Indirizzi.frz_des, '') + ' ' + Fornitore_ISTAT.LOCALITA + ' ' + Fornitore_ISTAT.COMUNI_PROV AS Fornitore_Citta " & vbCrLf)
            stb.Append("    , Fornitore_Indirizzi.CAP AS Fornitore_CAP " & vbCrLf)
            stb.Append("    , Fornitore_ISTAT.Comuni_PROV AS Fornitore_Provincia " & vbCrLf)
            stb.Append("    , ISNULL(Fornitore_Indirizzi.stato, 'IT')  AS Fornitore_Nazione " & vbCrLf)

            stb.Append("    --fine Dati del fornitore  " & vbCrLf)



            stb.Append("    --Dati del cliente  " & vbCrLf)
            stb.Append("    , Cliente_Contatti.cod_contatto as Cliente_Piva " & vbCrLf)
            stb.Append("    , Cliente_Contatti.Rag_Soc as Cliente_Rag_Soc " & vbCrLf)
            stb.Append("    , Cliente_Indirizzi.ind_des as Cliente_Indirizzo " & vbCrLf)
            stb.Append("    , ISNULL(Cliente_Indirizzi.frz_des, '') + ' ' + Cliente_ISTAT.LOCALITA + ' ' + Cliente_ISTAT.COMUNI_PROV AS Cliente_Citta " & vbCrLf)
            stb.Append("    , Cliente_Indirizzi.CAP AS Cliente_CAP " & vbCrLf)
            stb.Append("    , Fornitore_ISTAT.Comuni_PROV AS Cliente_Provincia " & vbCrLf)
            stb.Append("    , ISNULL(Cliente_Indirizzi.stato, 'IT')  AS Cliente_Nazione " & vbCrLf)
            stb.Append("    --Fine Dati del cliente  " & vbCrLf)

            stb.Append("    --Dati del punto di consegna  " & vbCrLf)
            stb.Append("    , ISNULL(Consegna_Contatti.cod_contatto, Cliente_Contatti.cod_contatto) as Consegna_Piva " & vbCrLf)
            stb.Append("    , ISNULL(Consegna_Contatti.Rag_Soc, Cliente_Contatti.Rag_Soc) as Consegna_Rag_Soc " & vbCrLf)
            stb.Append("    , ISNULL(Consegna_Indirizzi.ind_des, Cliente_Indirizzi.ind_des) as Consegna_Indirizzo " & vbCrLf)

            stb.Append("            , ISNULL(  " & vbCrLf)
            stb.Append("           ISNULL(Consegna_Indirizzi.frz_des, '') + ' ' +  Consegna_ISTAT.LOCALITA + ' ' + Consegna_ISTAT.COMUNI_PROV,  " & vbCrLf)
            stb.Append("                ISNULL( Cliente_Indirizzi.frz_des, '') + ' ' + Cliente_ISTAT.LOCALITA + ' ' + Cliente_ISTAT.COMUNI_PROV " & vbCrLf)
            stb.Append("        )  AS Consegna_Citta")


            stb.Append("    , ISNULL(Consegna_Indirizzi.CAP, Cliente_indirizzi.CAP) AS Consegna_CAP " & vbCrLf)
            stb.Append("    , ISNULL(Consegna_ISTAT.Comuni_PROV , Cliente_ISTAT.Comuni_PROV) AS Consegna_Provincia " & vbCrLf)
            stb.Append("    , ISNULL(ISNULL(Consegna_Indirizzi.stato, 'IT') , ISNULL(Cliente_Indirizzi.stato, 'IT')) AS Consegna_Nazione " & vbCrLf)
            stb.Append("    , ISNULL(Consegna_Indirizzi.INDICODE_EDI_NAD, Cliente_indirizzi.INDICODE_EDI_NAD)  AS LuogoConsegna_Codice_NAD " & vbCrLf)
            stb.Append("    --fine Dati del punto di consegna  " & vbCrLf)

            stb.Append("  " & vbCrLf)

            stb.Append("    --Dettagli " & vbCrLf)
            stb.Append("    , cast(MTes.cod_risum as varchar(100))  + '-' + d.lotto as Codice " & vbCrLf)
            stb.Append("    , fornitore_imprese.Rag_soc as Fornitore_Rag_Soc " & vbCrLf)
            stb.Append("    , D.Lotto as Fornitore_Lotto " & vbCrLf)
            stb.Append("    , D.Qta " & vbCrLf)
            stb.Append("    , D.Udm_cod " & vbCrLf)
            stb.Append("    , D.prezzo_unitario " & vbCrLf)
            stb.Append("    , D.prezzo_unitario_Netto " & vbCrLf)
            stb.Append("    , D.Sconto " & vbCrLf)
            stb.Append("    , D.Sconto_Listino " & vbCrLf)
            stb.Append("    , D.Sconto_Testo " & vbCrLf)
            stb.Append("    , D.mov_det_des " & vbCrLf)
            stb.Append("    , Materie1.Cod_Articolo " & vbCrLf)
            stb.Append("    --Dettagli " & vbCrLf)


            stb.Append("    --Tax (iva) " & vbCrLf)
            stb.Append("    , tax_iva.Sigla as iva_Des_sigla " & vbCrLf)
            stb.Append("    , tax_iva.tipologia as iva_tipologia " & vbCrLf)
            stb.Append("    , tax_iva.codice as iva_codice " & vbCrLf)
            stb.Append("    , tax_iva.aliquota as iva_aliquota " & vbCrLf)
            stb.Append("    , (select top 1 codice from iva_aliquote order by aliquota desc  ) as codice_aliquota_Max " & vbCrLf)
            stb.Append("    , D.iva as iva_imposta " & vbCrLf)
            stb.Append("    --fine Tax (iva) " & vbCrLf)


            stb.Append("    --codici prodotto " & vbCrLf)
            stb.Append("     , isnull( mm.Cod_Articolo , '') as CODDISTU " & vbCrLf)
            stb.Append("     --fine codici prodotto")


            stb.Append("    --riferimenti alla bolla " & vbCrLf)
            stb.Append("    , isnull(mTesRiff.Doc_Numero, MTes.Doc_Numero) as Bolla_Doc_Numero " & vbCrLf)
            stb.Append("    , isnull(mTesriff.Doc_Numero_Sin , MTes.Doc_Numero_sin) as Bolla_Doc_Numero_Sin " & vbCrLf)
            stb.Append("    , isnull(mTesRiff.Doc_Numero_Des, MTes.Doc_Numero_des) as Bolla_Doc_Numero_Des " & vbCrLf)
            stb.Append("    , ISNULL(mTesRiff.Data_Movimento , MTes.Data_Movimento ) as Bolla_Data_Documento" & vbCrLf)

            stb.Append("    --fine riferimenti alla bolla" & vbCrLf)



            stb.Append("    --riferimenti all'ordine di vendita (riferiemnti del cliente)  " & vbCrLf)
            stb.Append("    , ISNULL(oVmTesRiff.Username_Note, oVBmTesRiff.Username_Note ) as Cliente_Ordine_Riferimenti " & vbCrLf)
            stb.Append("    , ISNULL(oVmTesRiff.data_movimento,oVBmTesRiff.data_movimento  ) as Cliente_Ordine_Data " & vbCrLf)
            stb.Append("     --fine riferimenti all'ordine di vendita (riferiemnti del cliente) " & vbCrLf)

            stb.Append(" from Agenda A " & vbCrLf)
            stb.Append("    inner join Movimenti MTes " & vbCrLf)
            stb.Append("        on MTes.Id_Agenda= A.Id_Agenda " & vbCrLf)
            stb.Append("        AND MTes.Piva= A.Piva " & vbCrLf)
            stb.Append("        AND MTes.sa_cod= A.sa_cod " & vbCrLf)
            stb.Append("        and MTes.Cau_Mov = '4000' " & vbCrLf)

            stb.Append("    inner join Movimenti MDet " & vbCrLf)
            stb.Append("        on MDet.Id_Agenda= A.Id_Agenda " & vbCrLf)
            stb.Append("        AND MDet.Piva= A.Piva " & vbCrLf)
            stb.Append("        AND MDet.sa_cod= A.sa_cod " & vbCrLf)
            stb.Append("        and MDet.Cau_Mov = '" & CAU_SCARICO & "' " & vbCrLf)

            stb.Append("    inner join Movimenti_dettagli D " & vbCrLf)
            stb.Append("        on MDet.Id_Agenda = D.id_agenda " & vbCrLf)
            stb.Append("        and MDet.Piva = D.Piva " & vbCrLf)
            stb.Append("        and MDet.id_mov = D.id_mov " & vbCrLf)

            stb.Append("    inner join Risorse_Umane risumForn " & vbCrLf)
            stb.Append("        on risumForn.cod_risum = MTes.cod_Risum  " & vbCrLf)

            stb.Append("    -- Dati Cliente " & vbCrLf)

            stb.Append("    INNER JOIN Risorse_Umane as Cliente_Risorse_Umane  " & vbCrLf)
            stb.Append("            ON MTes.cod_Risum = Cliente_Risorse_Umane.Cod_RisUm  " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append("    INNER JOIN Contatti as Cliente_Contatti  " & vbCrLf)
            stb.Append("        ON Cliente_Risorse_Umane.Piva = Cliente_Contatti.Piva  " & vbCrLf)
            stb.Append("        AND Cliente_Risorse_Umane.Cod_Contatto = Cliente_Contatti.Cod_Contatto  " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append("    INNER JOIN Indirizzi AS Cliente_Indirizzi  " & vbCrLf)
            stb.Append("        ON MTes.Cod_IndirizzoRisum = Cliente_Indirizzi.cod_indirizzo  " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append("    INNER JOIN ISTAT AS Cliente_Istat  " & vbCrLf)
            stb.Append("        ON Cliente_Indirizzi.pro_cod_istat = Cliente_Istat.PROV  " & vbCrLf)
            stb.Append("        AND Cliente_Indirizzi.com_cod_istat = Cliente_Istat.COM  " & vbCrLf)


            stb.Append("    -- fine Dati Cliente " & vbCrLf)



            stb.Append("    -- Dati Punto di consegna " & vbCrLf)

            stb.Append("    LEFT JOIN Risorse_Umane as Consegna_Risorse_Umane  " & vbCrLf)
            stb.Append("            ON MTes.cod_Destinazione = Consegna_Risorse_Umane.Cod_RisUm  " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append("    LEFT JOIN Contatti as Consegna_Contatti  " & vbCrLf)
            stb.Append("        ON Consegna_Risorse_Umane.Piva = Consegna_Contatti.Piva  " & vbCrLf)
            stb.Append("        AND Consegna_Risorse_Umane.Cod_Contatto = Consegna_Contatti.Cod_Contatto  " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append("    LEFT JOIN Indirizzi AS Consegna_Indirizzi  " & vbCrLf)
            stb.Append("        ON MTes.Cod_IndirizzoDestinazione = Consegna_Indirizzi.cod_indirizzo  " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append("    LEFT JOIN ISTAT AS Consegna_Istat  " & vbCrLf)
            stb.Append("        ON Consegna_Indirizzi.pro_cod_istat = Consegna_Istat.PROV  " & vbCrLf)
            stb.Append("        AND Consegna_Indirizzi.com_cod_istat = Consegna_Istat.COM  " & vbCrLf)

            stb.Append("    -- fine Dati Punto di consegna " & vbCrLf)

            stb.Append("    -- Dati fornitore " & vbCrLf)
            stb.Append("    INNER JOIN Imprese fornitore_imprese " & vbCrLf)
            stb.Append("        on fornitore_imprese.piva = a.PIVA  " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append("    INNER JOIN ImpresexIndirizzi fornitore_IMPIndirizzi " & vbCrLf)
            stb.Append("        on fornitore_IMPIndirizzi.piva = a.PIVA  " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append("    INNER JOIN Indirizzi AS Fornitore_Indirizzi  " & vbCrLf)
            stb.Append("        ON Fornitore_Indirizzi.cod_indirizzo = Fornitore_impIndirizzi.cod_indirizzo " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append("    INNER join ISTAT as Fornitore_ISTAT on Fornitore_Indirizzi.pro_cod_istat = Fornitore_ISTAT.PROV  " & vbCrLf)
            stb.Append("        AND Fornitore_Indirizzi.com_cod_istat = fornitore_ISTAT.COM ")


            stb.Append("    -- fine Dati fornitore " & vbCrLf)



            stb.Append("    -- Dati record di tipo NAS " & vbCrLf)
            stb.Append("    inner join contatti_Codici ic_NAS_QCodForn " & vbCrLf)
            stb.Append("    on ic_NAS_QCodForn.id_cod = " & enum_CodiciAnagrafe.INDICODE_EDI_Euritmo_Tipo_Codice_fornitore_NAS_QCodForn & vbCrLf)
            stb.Append("    and ic_NAS_QCodForn.piva = a.piva " & vbCrLf)
            stb.Append("    and ic_NAS_QCodForn.cod_contatto = Cliente_Contatti.cod_contatto ")

            stb.Append("    inner join contatti_Codici ic_NAS_CODFORN " & vbCrLf)
            stb.Append("    on ic_NAS_CODFORN.id_cod =  " & enum_CodiciAnagrafe.INDICODE_EDI_Euritmo_Codice_fornitore_NAS_CodForn & vbCrLf)
            stb.Append("    and ic_NAS_CODFORN.piva = a.piva " & vbCrLf)
            stb.Append("    and ic_NAS_CodForn.cod_contatto = Cliente_Contatti.cod_contatto ")

            stb.Append("    inner join imprese_codici ic_Nas_Tribunale " & vbCrLf)
            stb.Append("    on ic_Nas_Tribunale.id_cod =  " & enum_CodiciAnagrafe.Tribunale_di_registrazione & vbCrLf)
            stb.Append("    and ic_Nas_Tribunale.piva = a.piva " & vbCrLf)

            stb.Append("    inner join imprese_codici ic_Nas_CCiAA " & vbCrLf)
            stb.Append("    on ic_NAS_CCiAA.id_cod = " & enum_CodiciAnagrafe.NumRegImprese & vbCrLf)
            stb.Append("    and ic_NAS_CCiAA.piva = a.piva " & vbCrLf)

            stb.Append("    inner join contatti c_Fornitore_CF " & vbCrLf)
            stb.Append("    on c_Fornitore_CF.piva = a.piva " & vbCrLf)
            stb.Append("    and c_Fornitore_CF.piva = c_Fornitore_CF.cod_contatto " & vbCrLf)


            stb.Append("    -- fine Dati record di tipo NAS " & vbCrLf)


            stb.Append("   INNER JOIN iva_Aliquote tax_iva" & vbCrLf)
            stb.Append("   on tax_iva.codice = D.Cod_iva " & vbCrLf)


            stb.Append("   inner join Materie_Prime materie1 " & vbCrLf)
            stb.Append("    on materie1.mat_cod = D.mat_cod " & vbCrLf)
            stb.Append("        and materie1.piva = D.piva  " & vbCrLf)


            stb.Append("    left join Materie_Prime_Alias al " & vbCrLf)
            stb.Append("        on al.Mat_Cod_Alias = D.Mat_Cod_Alias  " & vbCrLf)
            stb.Append("        and al.piva = D.piva " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append("    left join Materie_Prime mm " & vbCrLf)
            stb.Append("        on mm.Mat_Cod = al.Mat_Cod_alias " & vbCrLf)
            stb.Append("        and mm.Piva = al.piva  " & vbCrLf)
            stb.Append(" ")


            stb.Append("--riferimenti alla bolla " & vbCrLf)
            stb.Append("    left join Mov_Dettagli_Riferimenti riff " & vbCrLf)
            stb.Append("        on riff.Piva = a.piva  " & vbCrLf)
            stb.Append("        and riff.id_agenda = a.id_agenda " & vbCrLf)
            stb.Append("        and riff.id_mov_det = D.Id_Mov_Det  " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append("    left join Movimenti_dettagli mRiff " & vbCrLf)
            stb.Append("        on mriff.piva =riff.piva_rif " & vbCrLf)
            stb.Append("        and mRiff.Id_Agenda = riff.Id_Agenda_Rif " & vbCrLf)
            stb.Append("        and mRiff.Id_Mov_Det = riff.Id_Mov_Det_Rif " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append("    left join movimenti mTesRiff " & vbCrLf)
            stb.Append("        on mRiff.piva = mTesRiff.piva  " & vbCrLf)
            stb.Append("        and mRiff.Id_Agenda = mTesRiff.Id_Agenda " & vbCrLf)
            stb.Append("        and mTesRiff.Cau_Mov = '4000' " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append("    left join agenda aRiff  " & vbCrLf)
            stb.Append("        on aRiff.PIVA = mRiff.piva  " & vbCrLf)
            stb.Append("        and aRiff.Id_Agenda = mRiff.Id_Agenda " & vbCrLf)
            stb.Append("        and aRiff.Lav_Cod = 1031 " & vbCrLf)
            stb.Append("--fine riferimenti alla bolla " & vbCrLf)


            stb.Append("--riferimetni all'ordine di vendita " & vbCrLf)
            stb.Append("      left join Mov_Dettagli_Riferimenti oVriff  " & vbCrLf)
            stb.Append("         on oVriff.Piva = a.piva   " & vbCrLf)
            stb.Append("         and oVriff.id_agenda = a.id_agenda  " & vbCrLf)
            stb.Append("         and oVriff.id_mov_det = D.Id_Mov_Det   " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append("     left join Movimenti_dettagli oVmRiff  " & vbCrLf)
            stb.Append("         on oVmriff.piva =riff.piva_rif  " & vbCrLf)
            stb.Append("         and oVmRiff.Id_Agenda = riff.Id_Agenda_Rif  " & vbCrLf)
            stb.Append("         and oVmRiff.Id_Mov_Det = riff.Id_Mov_Det_Rif  " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append("     left join movimenti oVmTesRiff  " & vbCrLf)
            stb.Append("         on oVmRiff.piva = oVmTesRiff.piva   " & vbCrLf)
            stb.Append("         and oVmRiff.Id_Agenda = oVmTesRiff.Id_Agenda  " & vbCrLf)
            stb.Append("         and oVmTesRiff.Cau_Mov = '4000'  " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append("     left join agenda oVaRiff   " & vbCrLf)
            stb.Append("         on oVaRiff.PIVA = oVmRiff.piva   " & vbCrLf)
            stb.Append("         and oVaRiff.Id_Agenda = oVmRiff.Id_Agenda  " & vbCrLf)
            stb.Append("         and oVaRiff.Lav_Cod = 2002 " & vbCrLf)
            stb.Append("    --fine riferimenti all'ordine di vendita " & vbCrLf)



            stb.Append("    --riferimetni all'ordine di vendita da bolla " & vbCrLf)
            stb.Append("      left join Mov_Dettagli_Riferimenti oVBriff  " & vbCrLf)
            stb.Append("         on oVBriff.Piva = riff.piva   " & vbCrLf)
            stb.Append("         and oVBriff.id_agenda = riff.id_agenda  " & vbCrLf)
            stb.Append("         and oVBriff.id_mov_det = riff.Id_Mov_Det   " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append("     left join Movimenti_dettagli oVBmRiff  " & vbCrLf)
            stb.Append("         on oVBmRiff.piva =oVBriff.piva_rif  " & vbCrLf)
            stb.Append("         and oVBmRiff.Id_Agenda = oVBriff.Id_Agenda_Rif  " & vbCrLf)
            stb.Append("         and oVBmRiff.Id_Mov_Det = oVBriff.Id_Mov_Det_Rif  " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append("     left join movimenti oVBmTesRiff  " & vbCrLf)
            stb.Append("         on oVBmRiff.piva = oVBmTesRiff.piva   " & vbCrLf)
            stb.Append("         and oVBmRiff.Id_Agenda = oVBmTesRiff.Id_Agenda  " & vbCrLf)
            stb.Append("         and oVBmTesRiff.Cau_Mov = '4000'  " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append("     left join agenda oVBaRiff   " & vbCrLf)
            stb.Append("         on oVBaRiff.PIVA = oVBmRiff.piva   " & vbCrLf)
            stb.Append("         and oVBaRiff.Id_Agenda = oVBmRiff.Id_Agenda  " & vbCrLf)
            stb.Append("         and oVBaRiff.Lav_Cod = 2002 " & vbCrLf)
            stb.Append("    --fine riferimenti all'ordine di vendita " & vbCrLf)



            stb.Append("   where a.id_agenda in (" & Agro_SQL_Save_Clausola_IN(filtro_id_agenda) & ")  " & vbCrLf)
            stb.Append("   and materie1.Elem_Cod = 210 " & vbCrLf)



            If xFiltroAggiuntivo <> "" Then
                stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If



            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    stb.Append(" AND   a.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    stb.Append(" AND   a.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                stb.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, stb.ToString, NomeRoutine)
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

Public Class FF_DocumentiContabili_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function ResettaCampoInviatoSuMateriaPrima( _
                     ByVal objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                   ) As Boolean

        Dim NomeRoutine As String = "Scrivi()"

        '====================================================================================
        'Parametri opzionali :

        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim xRisp As Boolean = False


        Try



            '---------------------------------------------
            Stb.Length = 0

            
            Stb.Append(" update Materie_Prime_Campionature  " & vbCrLf)
            Stb.Append(" set inviato = 0 " & vbCrLf)
            Stb.Append(" where  abs(Progressivo ) > ( " & vbCrLf)
            Stb.Append("    isnull ( " & vbCrLf)
            Stb.Append("        ( " & vbCrLf)
            Stb.Append("   Select ultimo_valore " & vbCrLf)
            Stb.Append("        from Sequenza_Tabelle " & vbCrLf)
            Stb.Append("        where Nome_Tabella = 'materie_prime_campionature' " & vbCrLf)
            Stb.Append("        ) " & vbCrLf)
            Stb.Append("    , 0) " & vbCrLf)
            Stb.Append(" ) " & vbCrLf)
            Stb.Append(" ")



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


    Public Function RiportaCalCodSuDettaglio( _
                  ByVal objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                ) As Boolean

        Dim NomeRoutine As String = "Scrivi()"

        '====================================================================================
        'Parametri opzionali :

        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        
        Try

        

            '---------------------------------------------
            Stb.Length = 0

            Stb.Append(" " & vbCrLf  ) 
            Stb.Append(" update d " & vbCrLf)
            Stb.Append("    set cal_Cod = c.progressivo " & vbCrLf)
            Stb.Append(" from movimenti_dettagli d " & vbCrLf)
            Stb.Append("    inner join Materie_Prime_Campionature c " & vbCrLf)
            Stb.Append("        on d.Id_Mov_Det = c.inviato " & vbCrLf)
            Stb.Append("  " & vbCrLf)
            Stb.Append(" where abs(c.Progressivo ) > ( " & vbCrLf)
            Stb.Append("    isnull ( " & vbCrLf)
            Stb.Append("        ( " & vbCrLf)
            Stb.Append("   Select ultimo_valore " & vbCrLf)
            Stb.Append("        from Sequenza_Tabelle " & vbCrLf)
            Stb.Append("        where Nome_Tabella = 'materie_prime_campionature' " & vbCrLf)
            Stb.Append("        ) " & vbCrLf)
            Stb.Append("    , 0) " & vbCrLf)
            Stb.Append(" ) " & vbCrLf)
            Stb.Append(" ")


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


    Public Function RiportaCampionaturaOPAgendaDatoDefault( _
                  ByVal id_agenda As Integer _
                , ByVal validita_inizio As DateTime _
                , ByVal validita_fine As DateTime _
                , ByVal Riporta_1_ImpostaAlmeno1_2 As Integer _
                , ByVal objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                , Optional ByVal Data_creazione As Date = #2/1/1900# _
                , Optional ByVal Data_modifica As Date = #2/1/1900# _
                , Optional ByVal username_creazione As String = "" _
                , Optional ByVal username_modifica As String = "" _
                ) As Boolean

        Dim NomeRoutine As String = "Scrivi()"

        '====================================================================================
        'Parametri opzionali :

        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Dim joinCond As String = "Inner"
        Dim whereCont As String = ""

        If Riporta_1_ImpostaAlmeno1_2 = 2 Then
            joinCond = "left"
            whereCont = " and c.progressivo is null "
        End If



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
            Stb.Length = 0



            Stb.Append(" insert Materie_Prime_Campionature (Progressivo, Tipo, Tipo_Cod, Udm_Cod, Val_Cod, Descrizione, Progressivo_Origine, Piva_SuperUser_Origine, Peso_Campione, ChkStima, inviato, datainvio, Data_Creazione, Data_Modifica, Username_Creazione, Username_Modifica, Validita_Inizio, Validita_Fine) " & vbCrLf)

            Stb.Append(" select  " & vbCrLf)
            Stb.Append("     0 - ( DENSE_RANK() over ( order by d.id_mov_det) - (select min(progressivo) from Materie_Prime_Campionature) ) as Progressivo " & vbCrLf)

            If Riporta_1_ImpostaAlmeno1_2 = 1 Then

                Stb.Append("    , c.Tipo " & vbCrLf)
                Stb.Append("    , c.Tipo_Cod " & vbCrLf)
                Stb.Append("    , c.Udm_Cod " & vbCrLf)
                Stb.Append("    , c.Val_Cod " & vbCrLf)
                Stb.Append("    , c.Descrizione  " & vbCrLf)
                Stb.Append("    , c.Progressivo_Origine " & vbCrLf)
                Stb.Append("    , c.Piva_SuperUser_Origine " & vbCrLf)
                Stb.Append("    , c.Peso_Campione " & vbCrLf)
                Stb.Append("    , c.ChkStima " & vbCrLf)
                Stb.Append("  " & vbCrLf)
                Stb.Append("    , d.id_mov_det as inviato " & vbCrLf)
                Stb.Append("    , null as datainvio " & vbCrLf)

            Else

                Stb.Append("    , 'ocliente' as Tipo " & vbCrLf)
                Stb.Append("    , 0 as Tipo_Cod " & vbCrLf)
                Stb.Append("    , 0 as Udm_Cod " & vbCrLf)
                Stb.Append("    , '' as Val_Cod " & vbCrLf)
                Stb.Append("    , '' as Descrizione  " & vbCrLf)
                Stb.Append("    , 0 as progressivo_Origine " & vbCrLf)
                Stb.Append("    , '' as Piva_SuperUser_Origine " & vbCrLf)
                Stb.Append("    , 0 as Peso_Campione " & vbCrLf)
                Stb.Append("    , 0 as ChkStima " & vbCrLf)
                Stb.Append("  " & vbCrLf)
                Stb.Append("    , d.id_mov_det as inviato " & vbCrLf)
                Stb.Append("    , null as datainvio " & vbCrLf)

            End If

            Stb.Append("    , " & Agro_SQL_SaveDate(Data_creazione) & "  as  Data_Creazione " & vbCrLf)
            Stb.Append("    , " & Agro_SQL_SaveDate(Data_modifica) & " as Data_Modifica " & vbCrLf)
            Stb.Append("    ,'" & Agro_SQL_SaveText(username_creazione) & "' as Username_Creazione " & vbCrLf)
            Stb.Append("    ,'" & Agro_SQL_SaveText(username_modifica) & "' as Username_modifica " & vbCrLf)
            Stb.Append("    , " & Agro_SQL_SaveDate(validita_inizio) & " as Validita_Inizio " & vbCrLf)
            Stb.Append("    , " & Agro_SQL_SaveDate(validita_fine) & " as Validita_Fine " & vbCrLf)
            Stb.Append("  " & vbCrLf)
            Stb.Append(" from movimenti_dettagli d " & vbCrLf)
            Stb.Append("    inner join  materie_prime m " & vbCrLf)
            Stb.Append("         on d.Mat_Cod = m.Mat_Cod  " & vbCrLf)
            Stb.Append("         and d.Elem_Cod = m.Elem_Cod " & vbCrLf)
            Stb.Append("  " & vbCrLf)
            Stb.Append("  " & vbCrLf)
            Stb.Append("    " & joinCond & " join Materie_Prime_Campionature c " & vbCrLf)
            Stb.Append("        on c.Progressivo = m.Cal_Cod " & vbCrLf)
            Stb.Append("  " & vbCrLf)
            Stb.Append("  " & vbCrLf)
            Stb.Append(" where d.Id_Agenda =  " & Agro_SQL_SaveNum(id_agenda) & vbCrLf)
            Stb.Append("  " & whereCont & vbCrLf)

            
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


    '##############################################################################################
    Public Function UpdateSequenzaTabelleCampionatura( _
                  ByVal objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _                
                ) As Boolean

        Dim NomeRoutine As String = "Scrivi()"

        '====================================================================================
        'Parametri opzionali :

        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

          


            '---------------------------------------------
            Stb.Length = 0

            Stb.Append(" update Sequenza_Tabelle " & vbCrLf)
            Stb.Append(" set Ultimo_Valore = abs(isnull((select min(progressivo) from materie_prime_campionature), 1))  " & vbCrLf)
            Stb.Append(" where Nome_Tabella = 'materie_prime_campionature' " & vbCrLf)


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



    '##############################################################################################
    Public Function Scrivi(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                , Optional ByVal Data_creazione As Date = #2/1/1900# _
                , Optional ByVal Data_modifica As Date = #2/1/1900# _
                , Optional ByVal username_creazione As String = "" _
                , Optional ByVal username_modifica As String = "" _
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
    Public Function Cancella(ByVal xFiltroAggiuntivo As String, _
                              ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
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





