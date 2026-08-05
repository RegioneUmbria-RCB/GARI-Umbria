Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Public Class DomandaIrrigua_Dettaglio_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi(ByVal Id_Testata As Integer,
                          ByVal Id_Riga As Integer,
                          ByVal PIVA As String,
                          ByVal Sa_Cod As Integer,
                          ByVal Appezza As Integer,
                          ByVal ID_Reg As Integer,
                          ByVal Selezionato As Integer,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable
        Dim nomeRoutine As String = "AgronicaCoreDomandaIrriguaDAL.DomandaIrrigua_Dettaglio_R.Leggi()"

        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
        Dim dt As DataTable

        Try
            strSql.Length = 0
            strSql.AppendLine(" SELECT ")
            strSql.AppendLine("     * ")
            strSql.AppendLine(" FROM  [DomandaIrrigua_Dettaglio] ")
            strSql.AppendLine(" WHERE 1 = 1  ")

            If Id_Testata <> 0 Then
                strSql.AppendLine(" And Id_Testata = " & Agro_SQL_SaveNum(Id_Testata) & " ")
            End If

            If Id_Riga <> 0 Then
                strSql.AppendLine(" And Id_Riga = " & Agro_SQL_SaveNum(Id_Riga) & " ")
            End If

            If PIVA <> "" Then
                strSql.AppendLine(" AND PIVA = '" & Agro_SQL_SaveText(PIVA) & "' ")
            End If

            If Sa_Cod <> 0 Then
                strSql.AppendLine(" And Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            End If

            If Appezza <> 0 Then
                strSql.AppendLine(" And appezza = " & Agro_SQL_SaveNum(Appezza) & " ")
            End If

            If ID_Reg <> 0 Then
                strSql.AppendLine(" And id_Reg  = " & Agro_SQL_SaveNum(ID_Reg) & " ")
            End If

            If Selezionato <> -1 Then
                strSql.AppendLine(" And Selezionato = " & Agro_SQL_SaveNum(Selezionato) & " ")
            End If

            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            If xOrderBy <> "" Then
                strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------
        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt
    End Function

    Public Function LeggiConCatastoEPianoColturale(ByVal Id_Testata As Integer,
                          ByVal Id_Riga As Integer,
                          ByVal PIVA As String,
                          ByVal Sa_Cod As Integer,
                          ByVal Appezza As Integer,
                          ByVal ID_Reg As Integer,
                          ByVal Selezionato As Integer,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable
        Dim nomeRoutine As String = "AgronicaCoreDomandaIrriguaDAL.DomandaIrrigua_Dettaglio_R.LeggiConCatastoEPianoColturale()"

        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
        Dim dt As DataTable

        Try
            strSql.Length = 0
            strSql.AppendLine(" select a.*, ")
            strSql.AppendLine("   b.APP_NOME, ")
            strSql.AppendLine("   g.PROV, ")
            strSql.AppendLine("   m.PROVINCIA, ")
            strSql.AppendLine("   g.COM, ")
            strSql.AppendLine("   l.LOCALITA, ")
            strSql.AppendLine("   g.SEZIONE, ")
            strSql.AppendLine("   g.FOGLIO, ")
            strSql.AppendLine("   g.NUMERO, ")
            strSql.AppendLine("   case when c.CUL_COD=0 then ")
            strSql.AppendLine("         dest_uso_des.descrizione ")
            strSql.AppendLine("   else ")
            strSql.AppendLine("         f.Veg_Des ")
            strSql.AppendLine("   end as Coltura, ")
            strSql.AppendLine("   c.CUL_COD, ")
            strSql.AppendLine("   e.Cul_Des, ")
            strSql.AppendLine("   case when f.Veg_Cod is null then 0 else f.Veg_Cod end as Veg_Cod, ")
            strSql.AppendLine("   f.veg_des, ")
            strSql.AppendLine("   g.AREA, ")
            strSql.AppendLine("   i.Descrizione as gruppo_consegna ")
            strSql.AppendLine("   From DomandaIrrigua_Dettaglio a Left join ")
            strSql.AppendLine("     Appezzamento b on (A.PIVA=b.PIVA and a.Sa_Cod=b.SA_COD and a.Appezza=b.APPEZZA) left join ")
            strSql.AppendLine("     Reg_Impianti c on (A.PIVA= c.PIVA And a.Sa_Cod = c.SA_COD And a.Appezza = c.APPEZZA And a.Id_Reg = c.ID_REG) left join ")
            strSql.AppendLine("     reg_impianti_codici dest_uso On (A.PIVA=dest_uso.PIVA And a.Sa_Cod=dest_uso.SA_COD And a.Appezza=dest_uso.APPEZZA And a.Id_Reg=dest_uso.ID_REG And (dest_uso.id_cod between 3000 and 3014 or dest_uso.id_cod between 3016 and 3107 or dest_uso.id_cod between 3221 and 3230 or dest_uso.id_cod between 3244 and 3245 or dest_uso.id_cod between 3249 and 3259 or dest_uso.id_cod=3265)) left join ")
            strSql.AppendLine("     Codici_Anagrafe dest_uso_des On (dest_uso.id_cod=dest_uso_des.codice) left join ")
            strSql.AppendLine("     Cultivar e On (c.CUL_COD=e.Cul_Cod) left join ")
            strSql.AppendLine("     SpecieVegetali f On (e.Veg_Cod=f.Veg_Cod) left join ")
            strSql.AppendLine("     AppezzamentiXParticelle g On (A.PIVA=g.PIVA And a.Sa_Cod=g.SA_COD And a.Appezza=g.APPEZZA) left join ")
            strSql.AppendLine("     ZonexParticelle h On (g.PROV=h.PROV And g.COM=h.COM And g.SEZIONE=h.SEZIONE And g.FOGLIO =h.FOGLIO And g.NUMERO=h.NUMERO) left join ")
            strSql.AppendLine("     Zone i On (i.Zona_Cod=h.Zona_Cod) left join ")
            strSql.AppendLine("     ISTAT l On (l.PROV=g.PROV and l.COM=g.COM and l.Stato_Country='IT') left join ")
            strSql.AppendLine("     Lista_Province m On (m.PROV=g.PROV and m.Stato_Country='IT') ")
            strSql.AppendLine(" WHERE 1 = 1  and (c.CUL_COD>0 or (c.Cul_Cod=0 and dest_uso_des.descrizione is not null)) ")
            strSql.AppendLine("     and g.area is not null ")

            If Id_Testata <> 0 Then
                strSql.AppendLine(" And a.Id_Testata = " & Agro_SQL_SaveNum(Id_Testata) & " ")
            End If

            If Id_Riga <> 0 Then
                strSql.AppendLine(" And a.Id_Riga = " & Agro_SQL_SaveNum(Id_Riga) & " ")
            End If

            If PIVA <> "" Then
                strSql.AppendLine(" And a.PIVA = '" & Agro_SQL_SaveText(PIVA) & "' ")
            End If

            If Sa_Cod <> 0 Then
                strSql.AppendLine(" And a.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            End If

            If Appezza <> 0 Then
                strSql.AppendLine(" And a.appezza = " & Agro_SQL_SaveNum(Appezza) & " ")
            End If

            If ID_Reg <> 0 Then
                strSql.AppendLine(" And a.id_Reg  = " & Agro_SQL_SaveNum(ID_Reg) & " ")
            End If

            If Selezionato <> -1 Then
                strSql.AppendLine(" And a.Selezionato = " & Agro_SQL_SaveNum(Selezionato) & " ")
            End If

            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            If xOrderBy <> "" Then
                strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                strSql.AppendLine(" ORDER BY g.PROV,g.COM,g.SEZIONE,g.FOGLIO,g.NUMERO ")
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------
        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt
    End Function

    Public Function LeggiConCatastoEPianoColturaleAllaDataPerInizializzazioneDomandaIrrigua(ByVal PIVA As String,
                                                                                            ByVal Validita_Inizio As DateTime,
                                                                                            ByVal Validita_Fine As DateTime,
                                                                                            ByVal xFiltroAggiuntivo As String,
                                                                                            ByVal xOrderBy As String,
                                                                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable
        Dim nomeRoutine As String = "AgronicaCoreDomandaIrriguaDAL.DomandaIrrigua_Dettaglio_R.LeggiConCatastoEPianoColturaleAllaDataPerInizializzazioneDomandaIrrigua()"

        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
        Dim dt As DataTable

        Try
            strSql.Length = 0
            strSql.AppendLine(" select ")
            strSql.AppendLine(" 	   c.PIVA, ")
            strSql.AppendLine(" 	   c.sa_cod, ")
            strSql.AppendLine(" 	   c.appezza, ")
            strSql.AppendLine(" 	   c.id_reg, ")
            strSql.AppendLine(" 	   b.APP_NOME, ")
            strSql.AppendLine("        g.PROV, ")
            strSql.AppendLine("        m.PROVINCIA, ")
            strSql.AppendLine("        g.COM, ")
            strSql.AppendLine("        l.LOCALITA, ")
            strSql.AppendLine(" 	   g.SEZIONE, ")
            strSql.AppendLine(" 	   g.FOGLIO, ")
            strSql.AppendLine(" 	   g.NUMERO, ")
            strSql.AppendLine(" 	   case when c.CUL_COD=0 then ")
            strSql.AppendLine(" 		 dest_uso_des.descrizione ")
            strSql.AppendLine(" 	   else ")
            strSql.AppendLine(" 	     f.Veg_Des ")
            strSql.AppendLine(" 	   end as Coltura, ")
            strSql.AppendLine(" 	   c.CUL_COD, ")
            strSql.AppendLine(" 	   e.Cul_Des, ")
            strSql.AppendLine(" 	   case when f.Veg_Cod is null then 0 else f.Veg_Cod end as Veg_Cod, ")
            strSql.AppendLine(" 	   f.veg_des, ")
            strSql.AppendLine(" 	   g.AREA, ")
            strSql.AppendLine(" 	   i.Descrizione as Comizio, ")
            strSql.AppendLine(" 	   c.Validita_Inizio, ")
            strSql.AppendLine(" 	   c.Validita_Fine, ")
            strSql.AppendLine(" 	   c.Data_Creazione ")
            strSql.AppendLine(" 	   from   Appezzamento b left join ")
            strSql.AppendLine(" 			  Reg_Impianti c on (b.PIVA=c.PIVA and b.Sa_Cod=c.SA_COD and b.Appezza=c.APPEZZA ) left join ")
            strSql.AppendLine(" 			  reg_impianti_codici dest_uso on (c.PIVA=dest_uso.PIVA and c.Sa_Cod=dest_uso.SA_COD and c.Appezza=dest_uso.APPEZZA and c.Id_Reg=dest_uso.ID_REG and (dest_uso.id_cod between 3000 and 3014 or dest_uso.id_cod between 3016 and 3107 or dest_uso.id_cod between 3221 and 3230 or dest_uso.id_cod between 3244 and 3245 or dest_uso.id_cod between 3249 and 3259 or dest_uso.id_cod=3265)) left join ")
            strSql.AppendLine(" 			  Codici_Anagrafe dest_uso_des on (dest_uso.id_cod=dest_uso_des.codice) left join ")
            strSql.AppendLine(" 			  Cultivar e on (c.CUL_COD=e.Cul_Cod) left join ")
            strSql.AppendLine(" 			  SpecieVegetali f on (e.Veg_Cod=f.Veg_Cod) left join ")
            strSql.AppendLine(" 			  AppezzamentiXParticelle g on (b.PIVA=g.PIVA and b.Sa_Cod=g.SA_COD and b.Appezza=g.APPEZZA) left join ")
            strSql.AppendLine(" 			  ZonexParticelle h on (g.PROV=h.PROV and g.COM=h.COM and g.SEZIONE=h.SEZIONE and g.FOGLIO =h.FOGLIO and g.NUMERO=h.NUMERO) left join ")
            strSql.AppendLine(" 			  Zone i on (i.Zona_Cod=h.Zona_Cod) left join ")
            strSql.AppendLine("               ISTAT l On (l.PROV=g.PROV and l.COM=g.COM and l.Stato_Country='IT') left join ")
            strSql.AppendLine("               Lista_Province m On (m.PROV=g.PROV and m.Stato_Country='IT') ")
            strSql.AppendLine(" WHERE 1 = 1 and (c.CUL_COD>0 or (c.Cul_Cod=0 and dest_uso_des.descrizione is not null)) ")
            strSql.AppendLine("     and g.area is not null ")
            strSql.AppendLine("     and ((c.Validita_Inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & " and c.Validita_Fine >=  " & Agro_SQL_SaveDate(Validita_Inizio) & ")  or ")
            strSql.AppendLine("          (c.Validita_Inizio >= " & Agro_SQL_SaveDate(Validita_Inizio) & " and c.Validita_Fine >=  " & Agro_SQL_SaveDate(Validita_Inizio) & " )) ")

            If PIVA <> "" Then
                strSql.AppendLine(" And b.PIVA = '" & Agro_SQL_SaveText(PIVA) & "' ")
            End If

            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            If xOrderBy <> "" Then
                strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                strSql.AppendLine(" ORDER BY g.PROV,g.COM,g.SEZIONE,g.FOGLIO,g.NUMERO")
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------
        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt
    End Function

    Public Function LeggiRiepilogoDomanda(ByVal elencoPiva As List(Of String),
                                          ByVal Validita_Inizio As DateTime,
                                          ByVal Validita_Fine As DateTime,
                                          ByVal xFiltroAggiuntivo As String,
                                          ByVal xOrderBy As String,
                                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable
        Dim nomeRoutine As String = "AgronicaCoreDomandaIrriguaDAL.DomandaIrrigua_Dettaglio_R.LeggiConCatastoEPianoColturaleAllaDataPerInizializzazioneDomandaIrrigua()"

        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
        Dim dt As DataTable
        Try
            strSql.Length = 0
            If elencoPiva.Count > 0 Then
                strSql.AppendLine(" Declare @TEMP_PIVA TABLE (PIVA nvarchar(20) PRIMARY KEY); ")
                For Each piva In elencoPiva
                    strSql.AppendLine(" insert into @TEMP_PIVA values('" & Agro_SQL_SaveText(piva) & "'); ")
                Next
            End If
            strSql.AppendLine(" select ")
            strSql.AppendLine(" 	   h1.ID, ")
            strSql.AppendLine(" 	   h1.PIVA, ")
            strSql.AppendLine(" 	   CASE WHEN ISNULL(imp.partitaIvaReale, '') = '' THEN h1.PIVA ELSE imp.partitaIvaReale END PivaReale, ")
            strSql.AppendLine(" 	   imp.rag_soc, ")
            strSql.AppendLine(" 	   YEAR(h1.Validita_Inizio) as Anno, ")
            strSql.AppendLine(" 	   sum(g.area) as sup_tot ")
            strSql.AppendLine(" 	   from   DomandaIrrigua_Testata h1 inner join ")
            strSql.AppendLine("               Imprese imp on (h1.piva=imp.piva) inner join ")
            If elencoPiva.Count > 0 Then
                strSql.AppendLine("               @TEMP_PIVA d1 on (h1.PIVA=d1.PIVA) inner join ")
            End If
            strSql.AppendLine("               DomandaIrrigua_Dettaglio a  on (h1.id=a.Id_Testata) Left join ")
            strSql.AppendLine("               Appezzamento b on (A.PIVA=b.PIVA And a.Sa_Cod=b.SA_COD And a.Appezza=b.APPEZZA) left join ")
            strSql.AppendLine(" 			  Reg_Impianti c on (A.PIVA= c.PIVA And a.Sa_Cod = c.SA_COD And a.Appezza = c.APPEZZA And a.Id_Reg = c.ID_REG) left join ")
            strSql.AppendLine(" 			  reg_impianti_codici dest_uso On (A.PIVA=dest_uso.PIVA And a.Sa_Cod=dest_uso.SA_COD And a.Appezza=dest_uso.APPEZZA And a.Id_Reg=dest_uso.ID_REG And (dest_uso.id_cod between 3000 and 3014 or dest_uso.id_cod between 3016 and 3107 or dest_uso.id_cod between 3221 and 3230 or dest_uso.id_cod between 3244 and 3245 or dest_uso.id_cod between 3249 and 3259 or dest_uso.id_cod=3265)) left join ")
            strSql.AppendLine(" 			  Codici_Anagrafe dest_uso_des On (dest_uso.id_cod=dest_uso_des.codice) left join ")
            strSql.AppendLine(" 			  Cultivar e on (c.CUL_COD=e.Cul_Cod) left join ")
            strSql.AppendLine(" 			  SpecieVegetali f on (e.Veg_Cod=f.Veg_Cod) left join ")
            strSql.AppendLine(" 			  AppezzamentiXParticelle g on (b.PIVA=g.PIVA and b.Sa_Cod=g.SA_COD and b.Appezza=g.APPEZZA) left join ")
            strSql.AppendLine(" 			  ZonexParticelle h on (g.PROV=h.PROV and g.COM=h.COM and g.SEZIONE=h.SEZIONE and g.FOGLIO =h.FOGLIO and g.NUMERO=h.NUMERO) left join ")
            strSql.AppendLine(" 			  Zone i on (i.Zona_Cod=h.Zona_Cod) ")
            strSql.AppendLine(" WHERE 1 = 1  and (c.CUL_COD>0 or (c.Cul_Cod=0 and dest_uso_des.descrizione is not null)) ")
            strSql.AppendLine("     and g.area is not null ")
            strSql.AppendLine("     and Selezionato=1 ")

            If Validita_Inizio <> AGRODATAINIZIO Then
                strSql.AppendLine(" And h1.Validita_Inizio >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")
            End If

            If Validita_Fine <> AGRODATAFINE Then
                strSql.AppendLine(" And h1.Validita_Fine <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
            End If

            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            strSql.AppendLine(" group by h1.id, h1.piva, imp.partitaIvaReale, imp.rag_soc, h1.Validita_Inizio ")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------
        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try
        Return dt
    End Function

End Class
Public Class DomandaIrrigua_Dettaglio_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Scrivi(ByVal Id_Testata As Integer,
                           ByVal Id_Riga As Integer,
                           ByVal PIVA As String,
                           ByVal Sa_Cod As Integer,
                           ByVal Appezza As Integer,
                           ByVal ID_Reg As Integer,
                           ByVal Selezionato As Integer,
                           ByVal Validita_Inizio As Date,
                           ByVal Validita_Fine As Date,
                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                           Optional ByVal Data_creazione As DateTime = #2/1/1900#,
                           Optional ByVal Data_modifica As DateTime = #2/1/1900#,
                           Optional ByVal username_creazione As String = "",
                           Optional ByVal username_modifica As String = "") As Boolean
        Dim nomeRoutine As String = "AgronicaCoreDomandaIrriguaDAL.DomandaIrrigua_Dettaglio_W.Scrivi()"


        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        If Data_creazione = #2/1/1900# Then
            Data_creazione = Now
        End If

        If Data_modifica = #2/1/1900# Then
            Data_modifica = Now
        End If

        If username_creazione = "" Then
            username_creazione = objParametri.UsernameOperazione
        End If

        If username_modifica = "" Then
            username_modifica = objParametri.UsernameOperazione
        End If

        Try
            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.AppendLine("INSERT INTO DomandaIrrigua_Dettaglio ( Id_Testata , Id_Riga , PIVA, Sa_Cod, Appezza, Id_Reg, Selezionato, ")
            StrSQL.AppendLine("                         Inviato, DataInvio, ")
            StrSQL.AppendLine("                         Data_Creazione,     Data_Modifica, ")
            StrSQL.AppendLine("                         UserName_Creazione, UserName_Modifica, ")
            StrSQL.AppendLine("                         Validita_Inizio,    Validita_Fine ")
            StrSQL.AppendLine("                         ) ")
            StrSQL.AppendLine("VALUES (")
            StrSQL.AppendLine("          " & Agro_SQL_SaveNum(Id_Testata) & "  ")
            StrSQL.AppendLine("         ," & Agro_SQL_SaveNum(Id_Riga) & "  ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(PIVA) & "'  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Appezza) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(ID_Reg) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Selezionato) & "  ")
            StrSQL.AppendLine("         , 0  ")
            StrSQL.AppendLine("         , Null  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDateTime(Data_creazione) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDateTime(Data_modifica) & "  ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(username_modifica) & "' ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")
            StrSQL.AppendLine(")")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp

    End Function

    Public Function Modifica(ByVal Id_Testata As Integer,
                             ByVal Id_Riga As Integer,
                             ByVal PIVA As String,
                             ByVal Sa_Cod As Integer,
                             ByVal Appezza As Integer,
                             ByVal ID_Reg As Integer,
                             ByVal Selezionato As Integer,
                             ByVal Validita_Inizio As Date,
                             ByVal Validita_Fine As Date,
                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreDomandaIrriguaDAL.DomandaIrrigua_Dettaglio_W.Modifica()"


        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------

            StrSQL.Length = 0

            StrSQL.Append(" UPDATE DomandaIrrigua_Dettaglio SET ")
            StrSQL.Append("    PIVA             =  '" & Agro_SQL_SaveText(PIVA) & "'  ")
            StrSQL.Append("   ,Sa_Cod           =  " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            StrSQL.Append("   ,Appezza          =  " & Agro_SQL_SaveNum(Appezza) & "  ")
            StrSQL.Append("   ,Id_reg           =  " & Agro_SQL_SaveNum(ID_Reg) & "  ")
            StrSQL.Append("   ,Selezionato      =  " & Agro_SQL_SaveNum(Selezionato) & "  ")
            StrSQL.Append("   ,Inviato           =  0 ")
            StrSQL.Append("   ,DataInvio         =  Null ")
            StrSQL.Append("   ,Data_Modifica     =  " & Agro_SQL_SaveDateTime(DateTime.Now))
            StrSQL.Append("   ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")


            StrSQL.Append(" WHERE  1=1 ")

            If Id_Testata <> 0 Then
                StrSQL.Append(" AND Id_Testata = " & Agro_SQL_SaveNum(Id_Testata) & "   ")
            End If
            If Id_Riga <> 0 Then
                StrSQL.Append(" AND Id_Riga = " & Agro_SQL_SaveNum(Id_Riga) & "   ")
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & MessaggioErrore)
        End Try
        Return xRisp

    End Function

    Public Function Cancella(ByVal Id_Testata As Integer,
                             ByVal Id_Riga As Integer,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean
        Dim nomeRoutine As String = "AgronicaCoreDomandaIrriguaDAL.DomandaIrrigua_Dettaglio_W.Cancella()"


        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.AppendLine("DELETE from DomandaIrrigua_Dettaglio ")
            StrSQL.AppendLine(" WHERE 1 = 1  ")

            If Id_Testata <> 0 Then
                StrSQL.AppendLine(" AND Id_Testata = " & Agro_SQL_SaveNum(Id_Testata) & " ")
            End If
            If Id_Riga <> 0 Then
                StrSQL.AppendLine(" AND Id_Riga = " & Agro_SQL_SaveNum(Id_Riga) & " ")
            End If
            '----------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp

    End Function

End Class
