Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.DataProviderExtensions

Public Class FF_Trace_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function LeggiImmagineProdottoDatoCodiceLotto_daInterfacciamenti(
               ByVal Lotto As String,
               ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
           ) As DataTable


        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCore_DAL.Leggi()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            stb.Length = 0
            
            stb.AppendLine("select d.DirPicture ")
            stb.AppendLine(" from materie_prime m ")
            stb.AppendLine("  inner join Mov_Dettagli_Interfacciamenti d ")
            stb.AppendLine("      on m.mat_Cod = d.mat_cod ")
            stb.AppendLine("       ")
            stb.AppendLine("  where m.mat_cod In (  ")
            stb.AppendLine("   select mat_cod  ")
            stb.AppendLine("   from movimenti_dettagli   ")
            stb.AppendLine("   where lotto = '" & Agro_SQL_SaveText(Lotto) & "'  ")
            stb.AppendLine("  ) ")
            stb.AppendLine("  and DirPicture is not null  ")
            stb.AppendLine("  and DirPicture <> '' ")
            stb.AppendLine(" ")
            

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


    Public Function LeggiDescrizioneProdottoDatoCodiceLotto(
               ByVal Lotto As String, ByVal IdMovDetSelected As Integer,
               ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
           ) As DataTable


        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCore_DAL.Leggi()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            stb.Length = 0

            If IdMovDetSelected = 0 Then

                stb.AppendLine(" select top 1 Mat_Des ")
                stb.AppendLine(" from materie_Prime ")
                stb.AppendLine(" where mat_cod In ( ")
                stb.AppendLine("  select mat_cod ")
                stb.AppendLine("  from movimenti_dettagli  ")
                stb.AppendLine("  where lotto = '" & Agro_SQL_SaveText(Lotto) & "' ")
                stb.AppendLine(" )")

            Else
                stb.AppendLine(" select mat_des + ' ' + tbQua.Sigla + ' ' + tbCal.Sigla + ' ' + tbCert.Sigla As Mat_Des")
                stb.AppendLine(" from movimenti_dettagli md ")
                stb.AppendLine(" Join materie_prime mp on mp.Mat_Cod=md.Mat_Cod ")
                stb.AppendLine(" Join materie_prime_campionature mpcCert on mpcCert.progressivo=md.Cal_Cod And mpcCert.Tipo = 'ocertificazioni'  ")
                stb.AppendLine(" Join otabelle_parametri tbCert on mpcCert.progressivo=md.Cal_Cod And tbCert.Tabella_Cod = '12' And tbCert.Tabella_Par_Cod = mpcCert.tipo_Cod ")
                stb.AppendLine(" Join materie_prime_campionature mpcCal on mpcCal.progressivo=md.Cal_Cod And mpcCal.Tipo = 'ocalibro'  ")
                stb.AppendLine(" Join otabelle_parametri tbCal on mpcCal.progressivo=md.Cal_Cod And tbCal.Tabella_Cod = '1' And tbCal.Tabella_Par_Cod = mpcCal.tipo_Cod  ")
                stb.AppendLine(" Join materie_prime_campionature mpcQua on mpcQua.progressivo=md.Cal_Cod And mpcQua.Tipo = 'oqualità'   ")
                stb.AppendLine(" Join otabelle_parametri tbQua on mpcQua.progressivo=md.Cal_Cod And tbQua.Tabella_Cod = '3' And tbQua.Tabella_Par_Cod = mpcQua.tipo_Cod  ")
                stb.AppendLine("  where md.elem_cod = 210 and id_mov_det = " & IdMovDetSelected & " ")

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

    '##############################################################################################
    Public Function Leggi(
                ByVal Lotto As String,
                ByVal lav_cod As Integer,
                ByVal cau_mov_in As String,
                ByVal cau_mov_out As String,
                ByVal mat_Cod As Integer,
                ByVal cal_cod As Integer,
                ByVal Codice_Generazione_in As Integer,
                ByVal Codice_Generazione_out As Integer,
                ByVal Ricorsivo As Integer,
                ByVal xFiltroAggiuntivo As String,
                ByVal xOrderBy As String,
                ByVal cCertificazione As Integer,
                ByVal UltimaDataMov As Date,
                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
            ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCore_DAL.Leggi()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            stb.Length = 0

            stb.Append("   select distinct " & vbCrLf)
            stb.Append("      Ag.piva as piva_padre " & vbCrLf)
            stb.Append("    , Ag.sa_cod as sa_cod_padre " & vbCrLf)
            stb.Append("    , Ag.id_agenda as id_agenda_padre " & vbCrLf)
            stb.Append("    , Ag.id_mov as id_mov_padre " & vbCrLf)
            stb.Append("    , Ag.id_mov_det  as id_mov_det_padre " & vbCrLf)
            stb.Append("    , Ag.cal_cod  as cal_cod_padre " & vbCrLf)
            stb.Append("    , Ag.lav_cod " & vbCrLf)
            stb.Append("    , Ag.Des_lib " & vbCrLf)
            stb.Append("    , Ag.Cau_Mov as Cau_Mov_Out " & vbCrLf)
            stb.Append("    , M.Data_Movimento " & vbCrLf)
            stb.Append("    , M.Cau_Mov as Cau_Mov_In " & vbCrLf)
            stb.Append("    , D.piva " & vbCrLf)
            stb.Append("    , D.sa_cod " & vbCrLf)
            stb.Append("    , D.id_agenda " & vbCrLf)
            stb.Append("    , D.id_mov " & vbCrLf)
            stb.Append("    , D.id_mov_det " & vbCrLf)
            stb.Append("    , D.Elem_Cod " & vbCrLf)
            stb.Append("    , D.Pro_Cod  " & vbCrLf)
            stb.Append("    , D.mat_cod " & vbCrLf)
            stb.Append("    , D.udm_cod " & vbCrLf)
            stb.Append("    , u.UDM_SIM " & vbCrLf)
            stb.Append("    , D.cal_Cod " & vbCrLf)
            stb.Append("    , D.Lotto " & vbCrLf)
            stb.Append("    ,  Case when D.QTA_EXTRA = 0 Then D.Qta  ELSE  D.Qta * D.QTA_EXTRA END as Qta_Extra_Totale " & vbCrLf)
            stb.Append("    , 1 as percentuale_contributo " & vbCrLf)
            stb.Append("    , codice_generazione " & vbCrLf)
            stb.Append("    , Tabella " & vbCrLf)
            stb.Append("    , Tabella_Cod " & vbCrLf)
            stb.Append("    , Tabella_Des " & vbCrLf)

            'da riverificare la percentuale di contributo su qta_dettaglio1, qta_dettaglio2..:
            'stb.Append("    , case when  isNull(Ag.AG_qta, 0) = 0 Then 1 Else D.qta_extra / Ag.AG_qta_extra End As percentuale_contributo " & vbCrLf)

            stb.Append(" from ( " & vbCrLf)

            stb.Append("    Select  " & vbCrLf)
            stb.Append("           A.id_agenda " & vbCrLf)
            stb.Append("         , A.piva " & vbCrLf)
            stb.Append("         , D.sa_cod " & vbCrLf)
            stb.Append("         , A.des_lib " & vbCrLf)
            stb.Append("         , a.Lav_Cod " & vbCrLf)
            stb.Append("         , M.Cau_mov " & vbCrLf)
            stb.Append("         , D.id_mov " & vbCrLf)
            stb.Append("         , D.id_mov_det " & vbCrLf)
            stb.Append("         , D.cal_cod " & vbCrLf)
            stb.Append("         ,  Case When D.QTA_EXTRA = 0 Then D.Qta  Else  D.Qta * D.QTA_EXTRA End As AG_qta_extra_totale " & vbCrLf)
            stb.Append("    , ISNULL(lp.codice_generazione, 0) As codice_generazione " & vbCrLf)
            stb.Append("    , c.Tabella " & vbCrLf)
            stb.Append("    , c.Tabella_Cod " & vbCrLf)
            stb.Append("    , c.Tabella_Des " & vbCrLf)

            stb.Append(" from agenda A " & vbCrLf)
            stb.Append("    inner join movimenti M " & vbCrLf)
            stb.Append("        On m.id_agenda = A.id_agenda " & vbCrLf)
            stb.Append("        And m.piva = A.piva " & vbCrLf)
            stb.Append(" ")

            stb.Append("    inner join Movimenti_dettagli D " & vbCrLf)
            stb.Append("        On M.Id_Agenda = D.Id_Agenda " & vbCrLf)
            stb.Append("        And M.piva = D.piva  " & vbCrLf)
            stb.Append("        And M.id_mov = D.id_mov  " & vbCrLf)
            stb.Append("    inner join CategorieMagazzino c " & vbCrLf)
            stb.Append("        On c.Elem_Cod = D.Elem_Cod " & vbCrLf)

            If Codice_Generazione_in <> 0 Then
                stb.Append(" inner join Linee_Preparazioni lp " & vbCrLf)
                stb.Append("    On a.PREPARAZIONE_COD = lp.Preparazione_Cod " & vbCrLf)
                stb.Append("    and a.piva = lp.Piva " & vbCrLf)
            Else
                ' Leggo comunque le linee di preparazione per poter eventualmente non mostrare quelle non significative per 
                ' la tracciabilità (ad es. unificazione lotti)
                stb.Append(" left join Linee_Preparazioni lp " & vbCrLf)
                stb.Append("    On a.PREPARAZIONE_COD = lp.Preparazione_Cod " & vbCrLf)
                stb.Append("    and a.piva = lp.Piva " & vbCrLf)
            End If

            'If cCertificazione <> 0 Then
            '    'JOIN MATERIE PRIME CAMPIONATURE - CERTIFICAZIONE
            '    stb.Append(" INNER JOIN Materie_Prime_Campionature As Certificazioni On D.Cal_Cod = Certificazioni.Progressivo And Certificazioni.Tipo = 'ocertificazioni' and Tipo_Cod = " & cCertificazione & " " & vbCrLf)
            'End If

            stb.Append(" where a.lav_cod > 0 " & vbCrLf)

            If lav_cod <> 0 Then
                stb.Append("and a.Lav_Cod = " & lav_cod & vbCrLf)
            End If


            stb.Append(" and lotto = '" & Agro_SQL_SaveText(Lotto) & "' " & vbCrLf)


            If mat_Cod <> 0 Then
                stb.Append(" and Mat_Cod =  " & mat_Cod & vbCrLf)
            End If

            If cal_cod <> 0 Then
                stb.Append(" and Cal_Cod = " & cal_cod & vbCrLf)
            End If

            If Codice_Generazione_in <> 0 Then
                stb.Append(" and lp.Codice_Generazione = " & Codice_Generazione_in & " " & vbCrLf)
            End If

            If cau_mov_out <> "" Then
                stb.Append(" and M.cau_mov = '" & cau_mov_out & "'" & vbCrLf)
            End If

            ' Non posso testare la data per i movimenti di unificazione lotti perchè possono essere successivi
            stb.Append(" and (ISNULL(codice_generazione, 0) = -208 or (M.Data_Movimento <= " & Agro_SQL_SaveDate(UltimaDataMov) & " And ISNULL(codice_generazione, 0) <> -208 ))" & vbCrLf)

            If xFiltroAggiuntivo <> "" Then
                stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If



            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    stb.Append(" AND   A.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    stb.Append(" AND   A.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                stb.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            stb.Append("    ) Ag " & vbCrLf)
            stb.Append("  " & vbCrLf)
            stb.Append("  inner join  movimenti M  " & vbCrLf)
            stb.Append("    on m.id_agenda = Ag.id_agenda  " & vbCrLf)
            stb.Append("    and m.piva = Ag.piva  " & vbCrLf)
            stb.Append("  inner join Movimenti_dettagli D  " & vbCrLf)
            stb.Append("    on M.Id_Agenda = D.Id_Agenda  " & vbCrLf)
            stb.Append("    and M.piva = D.piva   " & vbCrLf)
            stb.Append("    and M.id_mov = D.id_mov   " & vbCrLf)
            If lav_cod = LAVCOD_FATTURA_EMESSA Or lav_cod = LAVCOD_BOLLA_EMESSA Then
                stb.Append("    And Ag.Cal_Cod=d.Cal_Cod   " & vbCrLf)
            End If

            stb.Append("  inner join UnitaMisura u " & vbCrLf)
            stb.Append("        on D.Udm_Cod=u.UDM_COD " & vbCrLf)


            'If cCertificazione <> 0 Then
            '    'JOIN MATERIE PRIME CAMPIONATURE - CERTIFICAZIONE
            '    stb.Append(" INNER JOIN Materie_Prime_Campionature As Certificazioni On D.Cal_Cod = Certificazioni.Progressivo And Certificazioni.Tipo = 'ocertificazioni' and Tipo_Cod = " & cCertificazione & " " & vbCrLf)
            'End If

            stb.Append("    where M.cau_mov = '" & cau_mov_in & "'   " & vbCrLf)

            If Codice_Generazione_out <> 0 Then
                stb.Append(" and Ag.Codice_Generazione = " & Codice_Generazione_out & " " & vbCrLf)
            End If

            ' Non posso testare la data per i movimenti di unificazione lotti perchè possono essere successivi
            stb.Append(" and (ISNULL(codice_generazione, 0) = -208 or (M.Data_Movimento <= " & Agro_SQL_SaveDate(UltimaDataMov) & " And ISNULL(codice_generazione, 0) <> -208 ))" & vbCrLf)

            stb.Append(" and (D.Qta<>0 or D.qta_extra_totale<>0) ")

            If Ricorsivo <> 1 Then
                stb.Append(" and lotto = '" & Agro_SQL_SaveText(Lotto) & "' " & vbCrLf)
            Else
                stb.Append(" and lotto not in ('indefinito', '') " & vbCrLf)
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

    Public Function LeggiDatiDocContabiliCollegati(
                ByVal piva As String,
                ByVal id_agenda As Integer,
                ByVal id_mov As Integer,
                ByVal xFiltroAggiuntivo As String,
                ByVal xOrderBy As String,
                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
            ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCoreContab_DAL.LeggiDatiDocContabiliCollegati()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            stb.Length = 0

            stb.AppendLine("     select ")
            stb.AppendLine("    	mv.PIVA, ")
            stb.AppendLine("    	mv.Sa_Cod, ")
            stb.AppendLine("    	mv.cau_mov, ")
            stb.AppendLine("    	mv.Id_Agenda, ")
            stb.AppendLine("    	mv.Id_Mov, ")
            stb.AppendLine("    	mv.Mov_Desc, ")
            stb.AppendLine("    	Rag_Soc, ")
            stb.AppendLine("    	Lav_Cod, ")
            stb.AppendLine("    	case ")
            stb.AppendLine("    		when Lav_Cod=1000 THEN ")
            stb.AppendLine("    			'FATTURA ACQUISTO' ")
            stb.AppendLine("    		when Lav_Cod=1001 then ")
            stb.AppendLine("    			'FATTURA VENDITA' ")
            stb.AppendLine("    		when Lav_Cod in (1054,1026,1078) then ")
            stb.AppendLine("    			'CONFERIMENTO' ")
            stb.AppendLine("    		when Lav_Cod=1025 then ")
            stb.AppendLine("    			'DDT RICEVUTO' ")
            stb.AppendLine("    		when Lav_Cod in (1069,1031) then ")
            stb.AppendLine("    			'DDT EMESSO' ")
            stb.AppendLine("    		else ")
            stb.AppendLine("    			'' ")
            stb.AppendLine("    	END, ")
            stb.AppendLine("    	case ")
            stb.AppendLine("    		when a.Lav_Cod in (1054,1026,1078) then	 ")
            stb.AppendLine("    			'' ")
            stb.AppendLine("    		when a.Lav_Cod in (1025,1000) then ")
            stb.AppendLine("    			'' ")
            stb.AppendLine("    		when a.Lav_Cod in (1069,1031,1001) then ")
            stb.AppendLine("    			(mv.Doc_Numero_Sin +'-'+CAST(cast(mv.Doc_Numero AS INT) as varchar)+'-'+mv.Doc_Numero_Des) ")
            stb.AppendLine("    		else ")
            stb.AppendLine("    			'' ")
            stb.AppendLine("    	end	 ")
            stb.AppendLine("    	as Numero_Documento, ")
            stb.AppendLine("    	case ")
            stb.AppendLine("    		when a.Lav_Cod in (1054,1026,1078) then	 ")
            stb.AppendLine("    			'19000101' ")
            stb.AppendLine("    		when a.Lav_Cod in (1025,1000) then ")
            stb.AppendLine("    			'19000101' ")
            stb.AppendLine("    		when a.Lav_Cod in (1069,1031,1001) then ")
            stb.AppendLine("    			mv.Ora ")
            stb.AppendLine("    		else ")
            stb.AppendLine("    			'19000101' ")
            stb.AppendLine("    	end ")
            stb.AppendLine("    	as Data_Documento, ")
            stb.AppendLine("    	case ")
            stb.AppendLine("    		when a.Lav_Cod in (1054,1026,1078) then	 ")
            stb.AppendLine("    			(mvf.Doc_Numero_Sin +'-'+cast(CAST(mvf.Doc_Numero AS INT) as varchar)+'-'+mvf.Doc_Numero_Des) ")
            stb.AppendLine("    		when a.Lav_Cod in (1025,1000) then ")
            stb.AppendLine("    			(mv.Doc_Numero_Sin +'-'+cast(CAST(mv.Doc_Numero AS INT) as varchar)+'-'+mv.Doc_Numero_Des) ")
            stb.AppendLine("    		when a.Lav_Cod in (1069,1031,1001) then ")
            stb.AppendLine("    			'' ")
            stb.AppendLine("    		else ")
            stb.AppendLine("    			'' ")
            stb.AppendLine("    	end	 ")
            stb.AppendLine("    	as Numero_Documento_Fornitore, ")
            stb.AppendLine("    	case ")
            stb.AppendLine("    		when a.Lav_Cod in (1054,1026,1078) then	 ")
            stb.AppendLine("    			mvf.Ora ")
            stb.AppendLine("    		when a.Lav_Cod in (1025,1000) then ")
            stb.AppendLine("    			mv.Ora ")
            stb.AppendLine("    		when a.Lav_Cod in (1069,1031,1001) then ")
            stb.AppendLine("    			'19000101' ")
            stb.AppendLine("    		else ")
            stb.AppendLine("    			'19000101' ")
            stb.AppendLine("    	end ")
            stb.AppendLine("    	as Data_Documento_Fornitore ")
            stb.AppendLine("     from Movimenti mv ")
            stb.AppendLine("     inner join Agenda a ")
            stb.AppendLine("      on (mv.Id_Agenda=a.Id_Agenda) ")
            stb.AppendLine("     left join Risorse_Umane ru ")
            stb.AppendLine("    	on (mv.Cod_RisUm=ru.Cod_RisUm) ")
            stb.AppendLine("     inner join Contatti cn ")
            stb.AppendLine("    	on (ru.Cod_Contatto=cn.Cod_Contatto and ru.Piva=cn.Piva) ")
            stb.AppendLine("     left join  ")
            stb.AppendLine("    	(select ")
            stb.AppendLine("    		PIVA, ")
            stb.AppendLine("    		Sa_Cod, ")
            stb.AppendLine("    		Id_Agenda, ")
            stb.AppendLine("    		Cod_RisUm, ")
            stb.AppendLine("    		Ora, ")
            stb.AppendLine("    		Doc_Numero_Sin, ")
            stb.AppendLine("    		Doc_Numero, ")
            stb.AppendLine("    		Doc_Numero_Des ")
            stb.AppendLine("    	 from ")
            stb.AppendLine("    		Movimenti ")
            stb.AppendLine("    	 where ")
            stb.AppendLine("    		Cau_Mov='4050' ")
            stb.AppendLine("    	) mvf ")
            stb.AppendLine("    	on (mvf.Cod_RisUm=ru.Cod_RisUm and mvf.PIVA=mv.PIVA and mvf.Sa_Cod=mv.Sa_Cod and mvf.Id_Agenda=mv.Id_Agenda) ")
            stb.AppendLine("     where mv.Cau_Mov='4000' ")

            If piva <> "" Then
                stb.AppendLine("     and mv.piva='" & Agro_SQL_SaveText(piva) & "' ")
            End If

            If id_agenda <> 0 Then
                stb.AppendLine("     and mv.id_agenda=" & Agro_SQL_SaveNum(id_agenda) & " ")
            End If

            If id_mov <> 0 Then
                stb.AppendLine("     and mv.id_mov=" & Agro_SQL_SaveNum(id_mov) & " ")
            End If

            If xFiltroAggiuntivo <> "" Then
                stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    stb.Append(" AND   mv.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    stb.Append(" AND   mv.Inviato =-1 ")
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

    Public Function LeggiMovimentiDettagliXMovimentoPadre(
                ByVal piva As String,
                ByVal sa_cod As Integer,
                ByVal id_agenda As Integer,
                ByVal id_mov As Integer,
                ByVal id_mov_det As Integer,
                ByVal xFiltroAggiuntivo As String,
                ByVal xOrderBy As String,
                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
            ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCoreContab_DAL.LeggiMovimentiDettagliXMovimentoPadre()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            stb.Length = 0

            stb.AppendLine("     select ")
            stb.AppendLine("    	* ")
            stb.AppendLine("     from Movimenti_Dettagli md ")
            stb.AppendLine("     where ")
            stb.AppendLine("     md.piva='" & Agro_SQL_SaveText(piva) & "' ")
            stb.AppendLine("     and md.sa_cod=" & Agro_SQL_SaveNum(sa_cod) & " ")

            If id_agenda <> 0 Then
                stb.AppendLine("     and md.id_agenda=" & Agro_SQL_SaveNum(id_agenda) & " ")
            End If

            If id_mov <> 0 Then
                stb.AppendLine("     and md.id_mov=" & Agro_SQL_SaveNum(id_mov) & " ")
            End If

            If id_mov_det <> 0 Then
                stb.AppendLine("     and md.id_mov_det=" & Agro_SQL_SaveNum(id_mov_det) & " ")
            End If

            If xFiltroAggiuntivo <> "" Then
                stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    stb.Append(" AND   md.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    stb.Append(" AND   md.Inviato =-1 ")
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



