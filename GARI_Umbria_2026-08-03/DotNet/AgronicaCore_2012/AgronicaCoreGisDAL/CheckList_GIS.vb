Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreModelsSTD.anagrafiche

Public Class CheckList_GIS_R
    Inherits DataProvider

    Public Function LeggiElencoAbilitazioniAziende(ByVal checklist_type As Integer,
                                                   ByVal piva As String,
                                                   ByVal stato_cod As Integer,
                                                   ByVal xFiltroAggiuntivo As String,
                                                   ByVal xOrderBy As String,
                                                   ByRef ObjParametri_Server As AgronicaCoreParametri) As DataTable
        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCoreGisDAL.CheckList_GIS_R.LeggiElencoAbilitazioniAziende()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            Stb.Length = 0

            Stb.AppendLine("Select ")
            Stb.AppendLine("    GIS_LayerAnalysisConfig_Checklist_Type_Cod,")
            Stb.AppendLine("    PIVA,")
            Stb.AppendLine("    Stato_Cod")
            Stb.AppendLine("from ")
            Stb.AppendLine("    GIS_LayerAnalysisConfig_Checklist_Elab_Level_Detail")
            Stb.AppendLine("Where ")
            Stb.AppendLine("    1=1 ")

            If checklist_type <> 0 Then
                Stb.AppendLine(String.Format(" and GIS_LayerAnalysisConfig_Checklist_Type_Cod={0}", Agro_SQL_SaveNum(checklist_type)))
            End If

            If piva <> "" Then
                Stb.AppendLine(String.Format(" and piva='{0}'", Agro_SQL_SaveText(piva)))
            End If

            If stato_cod <> -1 Then
                Stb.AppendLine(String.Format(" and stato_cod={0}", Agro_SQL_SaveNum(stato_cod)))
            End If

            If xFiltroAggiuntivo <> "" Then
                Stb.AppendLine(String.Format(" and {0} ", Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , ObjParametri_Server)))
            End If

            If xOrderBy <> "" Then
                Stb.AppendLine(String.Format(" ORDER BY {0} ", Agro_SQL_Save_xOrderBy(xOrderBy, ObjParametri_Server)))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(ObjParametri_Server, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------
        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(ObjParametri_Server, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try
        Return DT
    End Function

    Public Function LeggiElencoAbilitazioniAziendeInVisibilità(ByVal checklist_type As Integer,
                                                               ByVal username As String,
                                                               ByVal dataRiferimento As DateTime,
                                                               ByVal xFiltroAggiuntivo As String,
                                                               ByVal xOrderBy As String,
                                                               ByRef ObjParametri_Server As AgronicaCoreParametri) As DataTable
        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCoreGisDAL.CheckList_GIS_R.LeggiElencoAbilitazioniAziendeInVisibilità()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            Stb.Length = 0

            Stb.AppendLine(" If exists(select top 1 1 from Utenti_Visibilita_Appoggio a with (NOLOCK) where Username='" & Agro_SQL_SaveText(username) & "') ")
            Stb.AppendLine("    Select ")
            Stb.AppendLine("       PivaSuperUser, ")
            Stb.AppendLine("       Username, ")
            Stb.AppendLine("       a.Piva, ")
            Stb.AppendLine("       case when i.Compliance_ISCC_DataRiferimento<=" & Agro_SQL_SaveDate(dataRiferimento) & " or i.Compliance_ISCC_DataRiferimento is null or b.piva Is null then -1 else b.Stato_Cod End As Piva_Abilitata, ")
            Stb.AppendLine("       i.Compliance_ISCC_DataRiferimento ")
            Stb.AppendLine("    from ")
            Stb.AppendLine("       Imprese i with (NOLOCK) inner join ")
            Stb.AppendLine("       Utenti_Visibilita_Appoggio a With (NOLOCK) ")
            Stb.AppendLine("       on (i.piva=a.piva) left join ")
            Stb.AppendLine("       GIS_LayerAnalysisConfig_Checklist_Elab_Level_Detail b with (NOLOCK) ")
            Stb.AppendLine("       on (a.Piva=b.Piva) ")
            Stb.AppendLine("    where ")
            Stb.AppendLine("           a.PivaSuperUser ='" & Agro_SQL_SaveText(ObjParametri_Server.PivaSuperUser) & "' ")
            Stb.AppendLine("           And a.Username='" & Agro_SQL_SaveText(username) & "' ")
            Stb.AppendLine("           And a.Entita_Cod=1 ")
            Stb.AppendLine(" Else ")
            Stb.AppendLine("    Select ")
            Stb.AppendLine("        '" & ObjParametri_Server.PivaSuperUser & "' as PivaSuperUser, ")
            Stb.AppendLine("        '" & Agro_SQL_SaveText(username) & "' as Username, ")
            Stb.AppendLine("        a.Piva, ")
            Stb.AppendLine("        case when a.Compliance_ISCC_DataRiferimento<=" & Agro_SQL_SaveDate(dataRiferimento) & " or a.Compliance_ISCC_DataRiferimento is null or b.piva Is null then -1 else b.Stato_Cod End As Piva_Abilitata, ")
            Stb.AppendLine("        a.Compliance_ISCC_DataRiferimento ")
            Stb.AppendLine("    from ")
            Stb.AppendLine("        Imprese a with (NOLOCK) left join ")
            Stb.AppendLine("        GIS_LayerAnalysisConfig_Checklist_Elab_Level_Detail b with (NOLOCK) ")
            Stb.AppendLine("        on (a.Piva=b.Piva) ")

            If xFiltroAggiuntivo <> "" Then
                Stb.AppendLine(String.Format(" and {0} ", Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , ObjParametri_Server)))
            End If

            If xOrderBy <> "" Then
                Stb.AppendLine(String.Format(" ORDER BY {0} ", Agro_SQL_Save_xOrderBy(xOrderBy, ObjParametri_Server)))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(ObjParametri_Server, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------
        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(ObjParametri_Server, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try
        Return DT
    End Function

    Public Function LeggiChecklist(ByVal id As Integer,
                                  ByVal tipo As Integer,
                                  ByVal data_validita As DateTime,
                                  ByVal xFiltroAggiuntivo As String,
                                  ByVal xOrderBy As String,
                                  ByRef ObjParametri_Server As AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.CheckList_GIS_R.LeggiChecklist()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT ")
            StrSQL.AppendLine(" 	GIS_LayerAnalysisConfig_CheckList_Type_Cod ")
            StrSQL.AppendLine(" 	, a.GIS_LayerAnalysisConfig_CheckList_Cod ")
            StrSQL.AppendLine(" 	, Descrizione ")
            StrSQL.AppendLine(" 	, LayerAnalysisConfig_Cod ")
            StrSQL.AppendLine(" 	, b.Applica_Risultato ")
            StrSQL.AppendLine(" FROM GIS_LayerAnalysisConfig_CheckList a ")
            StrSQL.AppendLine(" INNER Join GIS_LayerAnalysisConfigXCheckList b ")
            StrSQL.AppendLine("     ON a.GIS_LayerAnalysisConfig_CheckList_Cod=b.GIS_LayerAnalysisConfig_CheckList_Cod ")

            StrSQL.AppendLine(" WHERE ")
            StrSQL.AppendLine(String.Format(" a.Validita_Inizio<= {0} and a.Validita_Fine>= {1} ", Agro_SQL_SaveDateTime(data_validita), Agro_SQL_SaveDateTime(data_validita)))

            If id <> 0 Then
                StrSQL.AppendLine(String.Format(" and a.GIS_LayerAnalysisConfig_CheckList_Cod = {0} ", Agro_SQL_SaveNum(id)))
            End If

            If tipo <> 0 Then
                StrSQL.AppendLine(String.Format(" and GIS_LayerAnalysisConfig_CheckList_Type_Cod = {0} ", Agro_SQL_SaveNum(tipo)))
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(String.Format(" and {0} ", Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo)))
            End If

            If xOrderBy <> "" Then
                StrSQL.AppendLine(String.Format(" order by {0} ", Agro_SQL_Save_xOrderBy(xOrderBy)))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(ObjParametri_Server, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(ObjParametri_Server, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT
    End Function

    Public Function LeggiCheckListDaRichiestaElaborazioneGISAnalysis(ByVal LayerAnalysisConfig_Cod As Integer,
                                                                     ByVal data_riferimento As Date,
                                                                     ByVal xFiltroAggiuntivo As String,
                                                                     ByVal xOrderBy As String,
                                                                     ByRef ObjParametri_Server As AgronicaCoreParametri) As DataTable
        Dim NomeRoutine As String = "AgronicaCoreGisDAL.CheckList_GIS_R.LeggiElencoEntitaDaPianoColturale()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.AppendLine(" select ")
            StrSQL.AppendLine(" 	c.GIS_LayerAnalysisConfig_Checklist_Type_Cod, ")
            StrSQL.AppendLine(" 	c.GIS_LayerAnalysisConfig_Checklist_Type_Des, ")
            StrSQL.AppendLine(" 	b.GIS_LayerAnalysisConfig_CheckList_Cod, ")
            StrSQL.AppendLine(" 	b.Descrizione, ")
            StrSQL.AppendLine(" 	a.Applica_Risultato ")
            StrSQL.AppendLine(" from ")
            StrSQL.AppendLine(" 	GIS_LayerAnalysisConfigXCheckList a inner join ")
            StrSQL.AppendLine(" 	GIS_LayerAnalysisConfig_CheckList b ")
            StrSQL.AppendLine(" 		on (a.GIS_LayerAnalysisConfig_CheckList_Cod=b.GIS_LayerAnalysisConfig_CheckList_Cod) ")
            StrSQL.AppendLine(" 	inner join GIS_LayerAnalysisConfig_Checklist_Type c  ")
            StrSQL.AppendLine(" 	on (b.GIS_LayerAnalysisConfig_CheckList_Type_Cod=c.GIS_LayerAnalysisConfig_Checklist_Type_Cod) ")
            StrSQL.AppendLine(" where ")
            StrSQL.AppendLine("     1=1 ")

            If LayerAnalysisConfig_Cod <> -1 Then
                StrSQL.AppendLine(String.Format(" 	a.LayerAnalysisConfig_Cod ={0} ", Agro_SQL_SaveNum(LayerAnalysisConfig_Cod)))
            End If

            If data_riferimento <> AGRODATAINIZIO Then
                StrSQL.AppendLine(String.Format(" 	and (b.Validita_Inizio<={0} and b.Validita_Fine>={0}) ", Agro_SQL_SaveDate(data_riferimento)))
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(String.Format(" and {0} ", Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo)))
            End If

            If xOrderBy <> "" Then
                StrSQL.AppendLine(String.Format(" order by {0} ", Agro_SQL_Save_xOrderBy(xOrderBy)))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(ObjParametri_Server, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(ObjParametri_Server, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT
    End Function

    Public Function LeggiElencoElaborazioniMassiveConDettaglio(ByVal CheckListType As Integer,
                                                           ByVal dataRefStart As DateTime,
                                                           ByVal dataRefEnd As DateTime,
                                                           ByVal piva As String,
                                                           ByVal xFiltroAggiuntivo As String,
                                                           ByVal xOrderBy As String,
                                                           ByRef ObjParametri_Server As AgronicaCoreParametri) As DataTable
        Dim NomeRoutine As String = "AgronicaCoreGisDAL.CheckList_GIS_R.LeggiElencoElaborazioniMassiveConDettaglio()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.AppendLine(" select ")
            StrSQL.AppendLine("     a1.GIS_LayerAnalysisConfig_Checklist_Type_Cod, ")
            StrSQL.AppendLine("     a1.GIS_LayerAnalysisConfig_Checklist_Type_Des, ")
            StrSQL.AppendLine("     a.GIS_LayerAnalysisConfig_CheckList_Cod, ")
            StrSQL.AppendLine("     a.Descrizione, ")
            StrSQL.AppendLine("     f.ID_Elaborazione, ")
            StrSQL.AppendLine("     f.Data_Controllo, ")
            StrSQL.AppendLine("     case when h.Esito_Richiesta =-1 then 'IN ELABORAZIONE' ")
            StrSQL.AppendLine("         when h.Esito_Richiesta =0 then 'KO' ")
            StrSQL.AppendLine("     Else 'OK' end as Esito_Richiesta, ")
            StrSQL.AppendLine("     b.LayerAnalysisConfig_Cod, ")
            StrSQL.AppendLine("     c.LayerAnalysisConfig_Des, ")
            StrSQL.AppendLine("     Case when g.Esito_Algoritmo =-1 then 'IN ELABORAZIONE'  ")
            StrSQL.AppendLine("         when g.Esito_Algoritmo =0 then 'KO' ")
            StrSQL.AppendLine("     Else 'OK' end as Esito_Algoritmo, ")
            StrSQL.AppendLine("     case when ap.APP_NOME Is null then '' else ap.APP_NOME end as Descrizione_Elaborazione, ")
            StrSQL.AppendLine("     Case when e.Esito_Elaborazione =-1 then 'IN ELABORAZIONE' ")
            StrSQL.AppendLine("         when e.Esito_Elaborazione =0 then 'KO' ")
            StrSQL.AppendLine("     Else 'OK' end as Esito_Elaborazione ")
            StrSQL.AppendLine(" from ")
            StrSQL.AppendLine("     GIS_LayerAnalysisConfig_Checklist_Type a1 ")
            StrSQL.AppendLine("     inner Join GIS_LayerAnalysisConfig_CheckList a  on (a1.GIS_LayerAnalysisConfig_Checklist_Type_Cod=a.GIS_LayerAnalysisConfig_CheckList_Type_Cod) ")
            StrSQL.AppendLine("     inner Join GIS_LayerAnalysisConfigXCheckList b on (a.GIS_LayerAnalysisConfig_CheckList_Cod=b.GIS_LayerAnalysisConfig_CheckList_Cod) ")
            StrSQL.AppendLine("     inner Join GIS_LayerAnalysisConfig c on (b.LayerAnalysisConfig_Cod=c.LayerAnalysisConfig_Cod) ")
            StrSQL.AppendLine("     inner Join GIS_LayerAnalysisConfig_Exec_Log d on (c.LayerAnalysisConfig_Cod=d.LayerAnalysisConfig_Cod) ")
            StrSQL.AppendLine("     inner Join Imprese_ISCCXElaborazioniXRichieste e on (d.GIS_LayerAnalysisConfig_Exec_Log_Cod=e.GIS_LayerAnalysisConfig_Exec_Log_Cod) ")
            StrSQL.AppendLine("     inner Join Imprese_ISCCXElaborazioni f on (e.ID_Elaborazione=f.ID_Elaborazione And e.Piva=f.Piva) ")
            StrSQL.AppendLine("                 inner Join(select b11.ID_Elaborazione, MIN(Esito_Elaborazione) as Esito_Richiesta from  ")
            StrSQL.AppendLine("                 GIS_LayerAnalysisConfig_Exec_Log a11  ")
            StrSQL.AppendLine("                 inner Join Imprese_ISCCXElaborazioniXRichieste b11 on (a11.GIS_LayerAnalysisConfig_Exec_Log_Cod=b11.GIS_LayerAnalysisConfig_Exec_Log_Cod)  ")
            StrSQL.AppendLine("                 inner Join Imprese_ISCCXElaborazioni c11 on (b11.ID_Elaborazione=c11.ID_Elaborazione And b11.Piva=c11.Piva) ")
            StrSQL.AppendLine("                 where 1=1 ")
            If piva <> "" Then
                StrSQL.AppendLine(String.Format(" 	And c11.Piva='{0}' ", Agro_SQL_SaveText(piva)))
            End If

            If dataRefStart <> AGRODATAINIZIO Then
                StrSQL.AppendLine(String.Format(" 	and c11.Data_Controllo>={0} ", Agro_SQL_SaveDate(dataRefStart)))
            End If

            If dataRefEnd <> AGRODATAINIZIO Then
                StrSQL.AppendLine(String.Format(" 	and c11.Data_Controllo<={0} ", Agro_SQL_SaveDate(dataRefEnd)))
            End If

            StrSQL.AppendLine("             group by b11.ID_Elaborazione) h on (f.ID_Elaborazione= h.ID_Elaborazione)  ")
            StrSQL.AppendLine("         inner Join(select b11.id_elaborazione, LayerAnalysisConfig_Cod, MIN(Esito_Elaborazione) as Esito_Algoritmo from  ")
            StrSQL.AppendLine("                 GIS_LayerAnalysisConfig_Exec_Log a11  ")
            StrSQL.AppendLine("                 inner Join Imprese_ISCCXElaborazioniXRichieste b11 on (a11.GIS_LayerAnalysisConfig_Exec_Log_Cod=b11.GIS_LayerAnalysisConfig_Exec_Log_Cod)  ")
            StrSQL.AppendLine("                 inner Join Imprese_ISCCXElaborazioni c11 on (b11.ID_Elaborazione=c11.ID_Elaborazione And b11.Piva=c11.Piva) ")
            StrSQL.AppendLine("                 where 1=1 ")
            If piva <> "" Then
                StrSQL.AppendLine(String.Format(" 	And c11.Piva='{0}' ", Agro_SQL_SaveText(piva)))
            End If

            If dataRefStart <> AGRODATAINIZIO Then
                StrSQL.AppendLine(String.Format(" 	and c11.Data_Controllo>={0} ", Agro_SQL_SaveDate(dataRefStart)))
            End If

            If dataRefEnd <> AGRODATAINIZIO Then
                StrSQL.AppendLine(String.Format(" 	and c11.Data_Controllo<={0} ", Agro_SQL_SaveDate(dataRefEnd)))
            End If
            StrSQL.AppendLine("             group by b11.id_elaborazione, LayerAnalysisConfig_Cod) g on (c.LayerAnalysisConfig_Cod= g.LayerAnalysisConfig_Cod And f.ID_Elaborazione=g.ID_Elaborazione)  ")
            StrSQL.AppendLine("     Left Join Reg_Impianti imp On (imp.PIVA=e.Piva And imp.SA_COD=e.Sa_Cod And imp.APPEZZA=e.Appezza And imp.ID_REG=e.Id_Reg) ")
            StrSQL.AppendLine("     inner Join Appezzamento ap On (ap.PIVA=imp.Piva And ap.SA_COD=imp.Sa_Cod And ap.APPEZZA=imp.APPEZZA) ")
            StrSQL.AppendLine(" where 1 = 1 ")

            If CheckListType <> 0 Then
                StrSQL.AppendLine(String.Format(" 	And a1.GIS_LayerAnalysisConfig_Checklist_Type_Cod ={0} ", Agro_SQL_SaveNum(CheckListType)))
            End If

            If piva <> "" Then
                StrSQL.AppendLine(String.Format(" 	And f.Piva='{0}' ", Agro_SQL_SaveText(piva)))
            End If

            If dataRefStart <> AGRODATAINIZIO Then
                StrSQL.AppendLine(String.Format(" 	and f.Data_Controllo>={0} ", Agro_SQL_SaveDate(dataRefStart)))
            End If

            If dataRefEnd <> AGRODATAINIZIO Then
                StrSQL.AppendLine(String.Format(" 	and f.Data_Controllo<={0} ", Agro_SQL_SaveDate(dataRefEnd)))
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(String.Format(" and {0} ", Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo)))
            End If

            If xOrderBy <> "" Then
                StrSQL.AppendLine(String.Format(" order by {0} ", Agro_SQL_Save_xOrderBy(xOrderBy)))
            Else
                StrSQL.AppendLine(String.Format(" order by a1.GIS_LayerAnalysisConfig_Checklist_Type_Cod, a.GIS_LayerAnalysisConfig_CheckList_Cod, f.ID_Elaborazione, f.Data_Controllo, b.LayerAnalysisConfig_Cod, e.GIS_LayerAnalysisConfig_Exec_Log_Cod "))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(ObjParametri_Server, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(ObjParametri_Server, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function

    Public Function LeggiElencoRichiesteElaborazioniMassiveAsincrone(ByVal TipoCheckList As Integer,
                                                                     ByVal Stato_Cod As Integer,
                                                                     ByVal xFiltroAggiuntivo As String,
                                                                     ByVal xOrderBy As String,
                                                                     ByRef ObjParametri_Server As AgronicaCoreParametri) As DataTable
        Dim NomeRoutine As String = "AgronicaCoreGisDAL.CheckList_GIS_R.LeggiElencoRichiesteElaborazioniMassiveAsincrone()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.AppendLine(" select ")
            StrSQL.AppendLine("     GIS_LayerAnalysisConfig_Checklist_Elab_Async_Cod, ")
            StrSQL.AppendLine("     GIS_LayerAnalysisConfig_Checklist_Type_Cod, ")
            StrSQL.AppendLine("     Request_Payload, ")
            StrSQL.AppendLine("     UserRequest, ")
            StrSQL.AppendLine("     Data_Creazione ")
            StrSQL.AppendLine(" from ")
            StrSQL.AppendLine("     GIS_LayerAnalysisConfig_Checklist_Elab_Async ")
            StrSQL.AppendLine(" where 1 = 1 ")
            StrSQL.AppendLine(String.Format(" 	And Stato_Cod ={0} ", Agro_SQL_SaveNum(Stato_Cod)))

            If TipoCheckList <> 0 Then
                StrSQL.AppendLine(String.Format(" 	And GIS_LayerAnalysisConfig_Checklist_Type_Cod ={0} ", Agro_SQL_SaveNum(TipoCheckList)))
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(String.Format(" and {0} ", Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo)))
            End If

            If xOrderBy <> "" Then
                StrSQL.AppendLine(String.Format(" order by {0} ", Agro_SQL_Save_xOrderBy(xOrderBy)))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(ObjParametri_Server, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(ObjParametri_Server, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function

    Public Function VerificaSeEsisteteRichiestaElaborazioneMassivaAsincronaXUtente(ByVal TipoCheckList As Integer,
                                                                                   ByVal Stato_Cod As Integer,
                                                                                   ByVal UserRequest As String,
                                                                                   ByVal xFiltroAggiuntivo As String,
                                                                                   ByVal xOrderBy As String,
                                                                                   ByRef ObjParametri_Server As AgronicaCoreParametri) As Boolean
        Dim NomeRoutine As String = "AgronicaCoreGisDAL.CheckList_GIS_R.VerificaSeEsisteteRichiestaElaborazioneMassivaAsincronaXUtente()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim check As Boolean = False

        Dim DT As DataTable = Nothing

        Try
            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.AppendLine(" select ")
            StrSQL.AppendLine("     count(*) as req_num ")
            StrSQL.AppendLine(" from ")
            StrSQL.AppendLine("     GIS_LayerAnalysisConfig_Checklist_Elab_Async ")
            StrSQL.AppendLine(" where 1 = 1 ")
            StrSQL.AppendLine(String.Format(" 	And Stato_Cod ={0} ", Agro_SQL_SaveNum(Stato_Cod)))

            If TipoCheckList <> 0 Then
                StrSQL.AppendLine(String.Format(" 	And GIS_LayerAnalysisConfig_Checklist_Type_Cod ={0} ", Agro_SQL_SaveNum(TipoCheckList)))
            End If

            If UserRequest <> "" Then
                StrSQL.AppendLine(String.Format(" 	And UserRequest = '{0}' ", Agro_SQL_SaveText(UserRequest)))
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(String.Format(" and {0} ", Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo)))
            End If

            If xOrderBy <> "" Then
                StrSQL.AppendLine(String.Format(" order by {0} ", Agro_SQL_Save_xOrderBy(xOrderBy)))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(ObjParametri_Server, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            If DT.Rows.Count > 0 Then
                If DT.Rows(0)("req_num") IsNot DBNull.Value AndAlso DT.Rows(0)("req_num") > 0 Then
                    check = True
                End If
            End If

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(ObjParametri_Server, NomeRoutine, MessaggioErrore)
            check = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return check
    End Function

End Class

Public Class CheckList_GIS_W
    Inherits DataProvider

    Public Function ScriviAbilitazioneAziendaCheckListType(ByVal checklist_type As Integer,
                                                ByVal piva As String,
                                                ByVal stato_cod As Integer,
                                                ByRef ObjParametri_Server As AgronicaCoreParametri,
                                                Optional ByVal Validita_Inizio As DateTime = AGRODATAINIZIO,
                                                Optional ByVal Validita_Fine As DateTime = AGRODATAFINE
                                                ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.CheckList_GIS_W.ScriviAbilitazioneAziendaCheckListType()"

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim resp As Boolean = False

        Try
            Stb.Length = 0

            Stb.AppendLine("insert into GIS_LayerAnalysisConfig_Checklist_Elab_Level_Detail (")
            Stb.AppendLine("    GIS_LayerAnalysisConfig_Checklist_Type_Cod")
            Stb.AppendLine("    , PIVA")
            Stb.AppendLine("    , Stato_Cod")
            Stb.AppendLine("    , Validita_Inizio")
            Stb.AppendLine("    , Validita_Fine")
            Stb.AppendLine("    , Username_Creazione")
            Stb.AppendLine("    , Data_Creazione")
            Stb.AppendLine("    , Username_Modifica")
            Stb.AppendLine("    , Data_Modifica")
            Stb.AppendLine(" ) VALUES ( ")
            Stb.AppendLine(String.Format(" {0} ", Agro_SQL_SaveNum(checklist_type)))
            Stb.AppendLine(String.Format(" , '{0}' ", Agro_SQL_SaveText(piva)))
            Stb.AppendLine(String.Format(" , {0} ", Agro_SQL_SaveNum(stato_cod)))
            Stb.AppendLine(String.Format(" , {0} ", Agro_SQL_SaveDate(Validita_Inizio)))
            Stb.AppendLine(String.Format(" , {0} ", Agro_SQL_SaveDate(Validita_Fine)))
            Stb.AppendLine(String.Format(" , '{0}' ", Agro_SQL_SaveText(ObjParametri_Server.UsernameOperazione)))
            Stb.AppendLine(String.Format(" , {0} ", Agro_SQL_SaveDateTime(DateTime.Now)))
            Stb.AppendLine(String.Format(" , '{0}' ", Agro_SQL_SaveText(ObjParametri_Server.UsernameOperazione)))
            Stb.AppendLine(String.Format(" , {0} ", Agro_SQL_SaveDateTime(DateTime.Now)))
            Stb.AppendLine(" ) ")

            '--------------------------------------------------------------------------
            resp = EseguiQuery_Scrittura(ObjParametri_Server, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(ObjParametri_Server, NomeRoutine, MessaggioErrore)
            resp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return resp
    End Function

    Public Function ModificaAbilitazioneAziendaCheckListType(ByVal checklist_type As Integer,
                                                            ByVal piva As String,
                                                          ByVal stato_cod As Integer,
                                                          ByRef ObjParametri_Server As AgronicaCoreParametri,
                                                          Optional ByVal Validita_Inizio As DateTime = AGRODATAINIZIO,
                                                          Optional ByVal Validita_Fine As DateTime = AGRODATAFINE
                                                          ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.CheckList_GIS_W.ModificaAbilitazioneAziendaCheckListType()"

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim resp As Boolean = False

        Try
            Stb.Length = 0

            Stb.AppendLine("update GIS_LayerAnalysisConfig_Checklist_Elab_Level_Detail set")
            Stb.AppendLine(String.Format("      Stato_Cod={0} ", Agro_SQL_SaveNum(stato_cod)))
            Stb.AppendLine(String.Format("    , Data_Modifica={0} ", Agro_SQL_SaveDate(DateTime.Now)))
            Stb.AppendLine(String.Format("    , Username_Modifica='{0}' ", Agro_SQL_SaveText(ObjParametri_Server.UsernameOperazione)))
            Stb.AppendLine("Where ")
            Stb.AppendLine("    1=1 ")

            If checklist_type <> 0 Then
                Stb.AppendLine(String.Format(" and GIS_LayerAnalysisConfig_Checklist_Type_Cod={0}", Agro_SQL_SaveNum(checklist_type)))
            End If

            If piva <> "" Then
                Stb.AppendLine(String.Format(" and piva='{0}'", Agro_SQL_SaveText(piva)))
            End If

            '--------------------------------------------------------------------------
            resp = EseguiQuery_Scrittura(ObjParametri_Server, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(ObjParametri_Server, NomeRoutine, MessaggioErrore)
            resp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return resp
    End Function

    Public Function InserisciNuovaRichiestaElaborazioneMassivaAsincrona(ByVal id_request As Integer,
                                                                ByVal checklist_type As Integer,
                                                                ByVal payload As String,
                                                                ByRef ObjParametri_Server As AgronicaCoreParametri
                                               ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.CheckList_GIS_W.InserisciNuovaRichiestaElaborazioneMassivaAsincrona()"

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim resp As Boolean = False

        Try
            Stb.Length = 0

            Stb.AppendLine("insert into GIS_LayerAnalysisConfig_Checklist_Elab_Async (")
            Stb.AppendLine("    GIS_LayerAnalysisConfig_Checklist_Elab_Async_Cod")
            Stb.AppendLine("    , GIS_LayerAnalysisConfig_Checklist_Type_Cod")
            Stb.AppendLine("    , Request_Payload")
            Stb.AppendLine("    , UserRequest")
            Stb.AppendLine("    , Stato_Cod")
            Stb.AppendLine("    , Validita_Inizio")
            Stb.AppendLine("    , Validita_Fine")
            Stb.AppendLine("    , Username_Creazione")
            Stb.AppendLine("    , Data_Creazione")
            Stb.AppendLine("    , Username_Modifica")
            Stb.AppendLine("    , Data_Modifica")
            Stb.AppendLine(" ) VALUES ( ")
            Stb.AppendLine(String.Format(" {0} ", Agro_SQL_SaveNum(id_request)))
            Stb.AppendLine(String.Format(" , {0} ", Agro_SQL_SaveNum(checklist_type)))
            Stb.AppendLine(String.Format(" , '{0}' ", Agro_SQL_SaveText(payload)))
            Stb.AppendLine(String.Format(" , '{0}' ", Agro_SQL_SaveText(ObjParametri_Server.UtenteUsername)))
            Stb.AppendLine(String.Format(" , {0} ", Agro_SQL_SaveNum(-1)))
            Stb.AppendLine(String.Format(" , {0} ", Agro_SQL_SaveDate(AGRODATAINIZIO)))
            Stb.AppendLine(String.Format(" , {0} ", Agro_SQL_SaveDate(AGRODATAFINE)))
            Stb.AppendLine(String.Format(" , '{0}' ", Agro_SQL_SaveText(ObjParametri_Server.UsernameOperazione)))
            Stb.AppendLine(String.Format(" , {0} ", Agro_SQL_SaveDateTime(DateTime.Now)))
            Stb.AppendLine(String.Format(" , '{0}' ", Agro_SQL_SaveText(ObjParametri_Server.UsernameOperazione)))
            Stb.AppendLine(String.Format(" , {0} ", Agro_SQL_SaveDateTime(DateTime.Now)))
            Stb.AppendLine(" ) ")

            '--------------------------------------------------------------------------
            resp = EseguiQuery_Scrittura(ObjParametri_Server, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(ObjParametri_Server, NomeRoutine, MessaggioErrore)
            resp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return resp
    End Function

    Public Function AggiornaRichiestaElaborazioneMassivaAsincrona(ByVal id_request As Integer,
                                                                  ByVal stato_cod As Integer,
                                                                  ByRef ObjParametri_Server As AgronicaCoreParametri
                                                                  ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.CheckList_GIS_W.AggiornaRichiestaElaborazioneMassivaAsincrona()"

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim resp As Boolean = False

        Try
            Stb.Length = 0

            Stb.AppendLine("update GIS_LayerAnalysisConfig_Checklist_Elab_Async set")
            Stb.AppendLine(String.Format("      Stato_Cod={0} ", Agro_SQL_SaveNum(stato_cod)))
            Stb.AppendLine(String.Format("    , Data_Modifica={0} ", Agro_SQL_SaveDate(DateTime.Now)))
            Stb.AppendLine(String.Format("    , Username_Modifica='{0}' ", Agro_SQL_SaveText(ObjParametri_Server.UsernameOperazione)))
            Stb.AppendLine("Where ")
            Stb.AppendLine("    1=1 ")

            If id_request <> 0 Then
                Stb.AppendLine(String.Format(" and GIS_LayerAnalysisConfig_Checklist_Elab_Async_Cod={0}", Agro_SQL_SaveNum(id_request)))
            End If

            '--------------------------------------------------------------------------
            resp = EseguiQuery_Scrittura(ObjParametri_Server, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(ObjParametri_Server, NomeRoutine, MessaggioErrore)
            resp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return resp
    End Function

End Class
