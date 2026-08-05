


Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.AgronicaCoreParametri

Public Class FF_TrackedData_Agenda_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function Leggi(ByVal xFiltroAggiuntivo As String, _
                                        ByVal xOrderBy As String, _
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                        ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCore_DAL.Leggi()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Stb.Length = 0

            Stb.Append(" SELECT * " + vbCrLf)
            Stb.Append(" From  FF_TrackedData_Agenda" + vbCrLf)

            If xFiltroAggiuntivo <> "" Then
                Stb.Append(" WHERE " & xFiltroAggiuntivo)
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    Stb.Append(" AND   Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    Stb.Append(" AND   Inviato =-1 ")
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

    Public Function LeggiXCache(
                                ByVal FF_TrackedData_Cod As Integer,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                Optional ByVal SwitchMatCodXPOC As Boolean = False
                            ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCore_DAL.Leggi()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Stb.Length = 0

            Stb.AppendLine("   Select ")
            Stb.AppendLine("      t.piva ")
            Stb.AppendLine("     ,t.sa_cod ")
            Stb.AppendLine("     ,t.id_agenda ")
            Stb.AppendLine("     ,a.lav_cod ")
            'Stb.AppendLine("     ,lp.Preparazione_Cod ")
            'Stb.AppendLine("     ,case when a.lav_cod<>5000 then lav_des else lp.preparazione_des end as lav_des ")
            Stb.AppendLine("     ,Des_lib ")
            Stb.AppendLine("     ,DATEADD(second, datepart(second, m.Ora), DATEADD(minute, datepart(MINUTE, m.Ora), DATEADD(HH, datepart(HH, m.Ora), m.data_movimento))) as Data_Movimento ")
            Stb.AppendLine("     ,t.id_mov ")
            Stb.AppendLine("     ,t.id_mov_det ")
            Stb.AppendLine("     ,d.Elem_Cod ")
            Stb.AppendLine("     ,Pro_Cod ")
            If SwitchMatCodXPOC = True Then
                Stb.AppendLine("     ,case when (a.lav_cod='5000' and lp.Preparazione_Cod=-137) then 2 else mat_cod end as mat_cod ")
            Else
                Stb.AppendLine("     ,mat_cod ")
            End If
            Stb.AppendLine("     ,d.udm_cod ")
            Stb.AppendLine("     ,u.udm_sim ")
            Stb.AppendLine("     ,t.cal_Cod ")
            Stb.AppendLine("     ,t.Lotto ")
            Stb.AppendLine("     ,1 as percentuale_contributo ")
            Stb.AppendLine("     ,t.ordine ")
            Stb.AppendLine("     ,0 as progressivo ")
            Stb.AppendLine("     ,Lotto_Padre ")
            Stb.AppendLine("     ,cal_Cod_Padre ")
            Stb.AppendLine("     ,piva_Padre ")
            Stb.AppendLine("     ,sa_Cod_Padre ")
            Stb.AppendLine("     ,id_agenda_padre ")
            Stb.AppendLine("     ,id_mov_padre ")
            Stb.AppendLine("     ,id_mov_det_padre ")
            Stb.AppendLine("     ,t.Qta_Extra_Totale ")
            Stb.AppendLine("     ,case when lp.Preparazione_Cod is null then '0' else lp.Preparazione_Cod end as codice_generazione ")
            Stb.AppendLine("     ,c.Tabella ")
            Stb.AppendLine("     ,c.Tabella_Cod ")
            Stb.AppendLine("     ,c.Tabella_Des ")
            Stb.AppendLine("     ,'' as prodotto ")
            Stb.AppendLine("     ,numero_documento ")
            Stb.AppendLine("     ,data_documento ")
            Stb.AppendLine("     ,intestatario_documento_ragione_sociale ")
            Stb.AppendLine("     ,numero_documento_fornitore ")
            Stb.AppendLine("     ,data_documento_fornitore ")
            Stb.AppendLine("     ,case when t.Visibile=1 then 'true' else 'false' end as visibile ")

            Stb.AppendLine("  ")
            Stb.AppendLine(" From FF_TrackedData_Agenda t ")
            Stb.AppendLine("     inner Join agenda a ")
            Stb.AppendLine("         On a.piva = t.piva ")
            Stb.AppendLine("         And a.id_agenda = t.id_agenda ")
            Stb.AppendLine("     inner Join movimenti m ")
            Stb.AppendLine("         On m.piva =t.piva ")
            Stb.AppendLine("         And m.id_agenda = t.id_Agenda ")
            Stb.AppendLine("         And m.id_mov = t.id_mov ")
            Stb.AppendLine("     inner Join Movimenti_dettagli d ")
            Stb.AppendLine("         On d.piva =t.piva ")
            Stb.AppendLine("         And d.id_agenda = t.id_Agenda ")
            Stb.AppendLine("         And d.id_mov = t.id_mov ")
            Stb.AppendLine("         And d.Id_Mov_Det = t.id_mov_det ")
            Stb.AppendLine("     inner Join Operazioni o ")
            Stb.AppendLine("         on a.lav_cod = o.lav_cod ")
            Stb.AppendLine("     inner Join CategorieMagazzino c ")
            Stb.AppendLine("         on c.Elem_Cod = d.Elem_Cod ")
            Stb.AppendLine(" left join Linee_Preparazioni lp ")
            Stb.AppendLine("    On a.PREPARAZIONE_COD = lp.Preparazione_Cod ")
            Stb.AppendLine("    inner join UnitaMisura u ")
            Stb.AppendLine("        on d.Udm_Cod=u.UDM_COD ")
            Stb.AppendLine(" WHERE FF_TrackedData_Cod = " & Agro_SQL_SaveNum(FF_TrackedData_Cod))


            If xFiltroAggiuntivo <> "" Then
                Stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo))
            End If
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                Stb.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy))
            Else
                Stb.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy("t.Data_Movimento Desc, t.Cal_Cod asc"))
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


    Public Function LeggiXCacheSoloEntità(
            ByVal FF_TrackedData_Cod As Integer,
            ByVal FromOutToIn As Boolean,
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

            Stb.AppendLine("   Select ")
            Stb.AppendLine("      t.piva ")
            Stb.AppendLine("     ,t.sa_cod ")
            Stb.AppendLine("     ,t.id_agenda ")
            Stb.AppendLine("     ,a.lav_cod ")
            Stb.AppendLine("     ,case when a.lav_cod<>5000 then lav_des else lp.preparazione_des end as lav_des ")
            Stb.AppendLine("     ,Des_lib ")
            Stb.AppendLine("     ,DATEADD(second, datepart(second, m.Ora), DATEADD(minute, datepart(MINUTE, m.Ora), DATEADD(HH, datepart(HH, m.Ora), m.data_movimento))) as Data_Movimento ")
            Stb.AppendLine("     ,t.id_mov ")
            Stb.AppendLine("     ,t.id_mov_det ")
            Stb.AppendLine("     ,d.Elem_Cod ")
            Stb.AppendLine("     ,Pro_Cod ")
            Stb.AppendLine("     ,mat_cod ")
            Stb.AppendLine("     ,d.udm_cod ")
            Stb.AppendLine("     ,u.UDM_SIM ")
            Stb.AppendLine("     ,t.cal_Cod ")
            Stb.AppendLine("     ,t.Lotto ")
            Stb.AppendLine("     ,100 as percentuale_contributo ")
            Stb.AppendLine("     ,0 as ordine ")
            Stb.AppendLine("     ,0 as progressivo ")
            Stb.AppendLine("     ,Lotto_Padre ")
            Stb.AppendLine("     ,t.cal_Cod_Padre ")
            Stb.AppendLine("     ,piva_padre ")
            Stb.AppendLine("     ,sa_cod_padre ")
            Stb.AppendLine("     ,id_agenda_padre ")
            Stb.AppendLine("     ,id_mov_padre ")
            Stb.AppendLine("     ,id_mov_det_padre ")
            Stb.AppendLine("     ,t.Qta_Extra_Totale ")
            Stb.AppendLine("     ,0 as codice_generazione ")
            Stb.AppendLine("     ,c.Tabella ")
            Stb.AppendLine("     ,c.Tabella_Cod ")
            Stb.AppendLine("     ,c.Tabella_Des ")
            Stb.AppendLine("     ,'' as Prodotto ")
            Stb.AppendLine("     ,Numero_Documento ")
            Stb.AppendLine("     ,Data_Documento ")
            Stb.AppendLine("     ,Numero_Documento_Fornitore ")
            Stb.AppendLine("     ,Data_Documento_Fornitore ")
            Stb.AppendLine("     ,Intestatario_Documento_Ragione_Sociale ")
            Stb.AppendLine("     ,t.Visibile ")
            Stb.AppendLine("     ,t.Ordine ")

            Stb.AppendLine("  ")
            Stb.AppendLine(" From FF_TrackedData_Agenda t ")
            Stb.AppendLine("     inner Join agenda a ")
            Stb.AppendLine("         On a.piva = t.piva ")
            Stb.AppendLine("         And a.id_agenda = t.id_agenda ")
            Stb.AppendLine("     inner Join movimenti m ")
            Stb.AppendLine("         On m.piva =t.piva ")
            Stb.AppendLine("         And m.id_agenda = t.id_Agenda ")
            Stb.AppendLine("         And m.id_mov = t.id_mov ")
            Stb.AppendLine("     inner Join Movimenti_dettagli d ")
            Stb.AppendLine("         On d.piva =t.piva ")
            Stb.AppendLine("         And d.id_agenda = t.id_Agenda ")
            Stb.AppendLine("         And d.id_mov = t.id_mov ")
            Stb.AppendLine("         And d.Id_Mov_Det = t.id_mov_det ")
            Stb.AppendLine("     inner Join CategorieMagazzino c ")
            Stb.AppendLine("         on c.Elem_Cod = d.Elem_Cod ")
            Stb.AppendLine("     inner Join Operazioni o ")
            Stb.AppendLine("         on a.lav_cod = o.lav_cod ")
            Stb.AppendLine(" left join Linee_Preparazioni lp ")
            Stb.AppendLine("    On a.PREPARAZIONE_COD = lp.Preparazione_Cod ")
            Stb.AppendLine("    inner join UnitaMisura u ")
            Stb.AppendLine("        on d.Udm_Cod=u.UDM_COD ")
            Stb.AppendLine(" WHERE FF_TrackedData_Cod = " & Agro_SQL_SaveNum(FF_TrackedData_Cod))
            If FromOutToIn = True Then
                Stb.AppendLine(" and (t.cal_Cod_Padre = 0 or t.cal_Cod_Padre not in (")
                Stb.AppendLine("  select distinct cal_Cod from FF_TrackedData_Agenda where FF_TrackedData_Cod = " & Agro_SQL_SaveNum(FF_TrackedData_Cod))
                Stb.AppendLine(" ))")
            Else
                Stb.AppendLine(" and (t.cal_Cod = 12 or t.cal_Cod not in (")
                Stb.AppendLine("  select distinct cal_Cod from FF_TrackedData_Agenda where FF_TrackedData_Cod = " & Agro_SQL_SaveNum(FF_TrackedData_Cod))
                Stb.AppendLine(" ))")
            End If


            If xFiltroAggiuntivo <> "" Then
                Stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo))
            End If
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                Stb.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy))
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

    Public Function LeggiXCacheSoloConnessioni(
            ByVal FF_TrackedData_Cod As Integer,
            ByVal FromOutToIn As Boolean,
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

            Stb.AppendLine("   Select ")
            Stb.AppendLine("      t.piva ")
            Stb.AppendLine("     ,t.sa_cod ")
            Stb.AppendLine("     ,t.id_agenda ")
            Stb.AppendLine("     ,a.lav_cod ")
            Stb.AppendLine("     ,case when a.lav_cod<>5000 then lav_des else lp.preparazione_des end as lav_des ")
            Stb.AppendLine("     ,Des_lib ")
            Stb.AppendLine("     ,DATEADD(second, datepart(second, m.Ora), DATEADD(minute, datepart(MINUTE, m.Ora), DATEADD(HH, datepart(HH, m.Ora), m.data_movimento))) as Data_Movimento ")
            Stb.AppendLine("     ,t.id_mov ")
            Stb.AppendLine("     ,t.id_mov_det ")
            Stb.AppendLine("     ,d.Elem_Cod ")
            Stb.AppendLine("     ,Pro_Cod ")
            Stb.AppendLine("     ,mat_cod ")
            Stb.AppendLine("     ,d.udm_cod ")
            Stb.AppendLine("     ,u.UDM_SIM ")
            Stb.AppendLine("     ,t.cal_Cod ")
            Stb.AppendLine("     ,t.Lotto ")
            Stb.AppendLine("     ,100 as percentuale_contributo ")
            Stb.AppendLine("     ,0 as ordine ")
            Stb.AppendLine("     ,0 as progressivo ")
            Stb.AppendLine("     ,Lotto_Padre ")
            Stb.AppendLine("     ,t.cal_Cod_Padre ")
            Stb.AppendLine("     ,piva_padre ")
            Stb.AppendLine("     ,sa_cod_padre ")
            Stb.AppendLine("     ,id_agenda_padre ")
            Stb.AppendLine("     ,id_mov_padre ")
            Stb.AppendLine("     ,id_mov_det_padre ")
            Stb.AppendLine("     ,t.Qta_Extra_Totale ")
            Stb.AppendLine("     ,0 as codice_generazione ")
            Stb.AppendLine("     ,c.Tabella ")
            Stb.AppendLine("     ,c.Tabella_Cod ")
            Stb.AppendLine("     ,c.Tabella_Des ")
            Stb.AppendLine("     ,'' as Prodotto ")
            Stb.AppendLine("     ,Numero_Documento ")
            Stb.AppendLine("     ,Data_Documento ")
            Stb.AppendLine("     ,Numero_Documento_Fornitore ")
            Stb.AppendLine("     ,Data_Documento_Fornitore ")
            Stb.AppendLine("     ,Intestatario_Documento_Ragione_Sociale ")
            Stb.AppendLine("     ,t.Visibile ")
            Stb.AppendLine("     ,t.Ordine ")

            Stb.AppendLine("  ")
            Stb.AppendLine(" From FF_TrackedData_Agenda t ")
            Stb.AppendLine("     inner Join agenda a ")
            Stb.AppendLine("         On a.piva = t.piva ")
            Stb.AppendLine("         And a.id_agenda = t.id_agenda ")
            Stb.AppendLine("     inner Join movimenti m ")
            Stb.AppendLine("         On m.piva =t.piva ")
            Stb.AppendLine("         And m.id_agenda = t.id_Agenda ")
            Stb.AppendLine("         And m.id_mov = t.id_mov ")
            Stb.AppendLine("     inner Join Movimenti_dettagli d ")
            Stb.AppendLine("         On d.piva =t.piva ")
            Stb.AppendLine("         And d.id_agenda = t.id_Agenda ")
            Stb.AppendLine("         And d.id_mov = t.id_mov ")
            Stb.AppendLine("         And d.Id_Mov_Det = t.id_mov_det ")
            Stb.AppendLine("     inner Join CategorieMagazzino c ")
            Stb.AppendLine("         on c.Elem_Cod = d.Elem_Cod ")
            Stb.AppendLine("     inner Join Operazioni o ")
            Stb.AppendLine("         on a.lav_cod = o.lav_cod ")
            Stb.AppendLine(" left join Linee_Preparazioni lp ")
            Stb.AppendLine("    On a.PREPARAZIONE_COD = lp.Preparazione_Cod ")
            Stb.AppendLine("    inner join UnitaMisura u ")
            Stb.AppendLine("        on d.Udm_Cod=u.UDM_COD ")
            Stb.AppendLine(" WHERE FF_TrackedData_Cod = " & Agro_SQL_SaveNum(FF_TrackedData_Cod))
            If FromOutToIn = True Then
                Stb.AppendLine(" and (t.cal_Cod_Padre <> 0 and t.cal_Cod_Padre in (")
                Stb.AppendLine("  select distinct cal_Cod from FF_TrackedData_Agenda where FF_TrackedData_Cod = " & Agro_SQL_SaveNum(FF_TrackedData_Cod))
                Stb.AppendLine(" ))")
            Else
                Stb.AppendLine(" and (t.cal_Cod <> 12 and t.cal_Cod in (")
                Stb.AppendLine("  select distinct cal_Cod from FF_TrackedData_Agenda where FF_TrackedData_Cod = " & Agro_SQL_SaveNum(FF_TrackedData_Cod))
                Stb.AppendLine(" ))")
            End If


            If xFiltroAggiuntivo <> "" Then
                Stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo))
            End If
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                Stb.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy))
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


'#################################################################
'#################################################################
'#################################################################

Public Class FF_TrackedData_Agenda_W
    Inherits AgronicaCoreDataProvider.DataProvider



    '##############################################################################################
    Public Function Scrivi(
                          ByVal FF_TrackedData_Cod As Integer _
                        , ByVal Piva As String _
                        , ByVal sa_cod As Integer _
                        , ByVal id_agenda As Integer _
                        , ByVal id_mov As Integer _
                        , ByVal id_mov_det As Integer _
                        , ByVal lotto As String _
                        , ByVal lotto_padre As String _
                        , ByVal cal_cod As Integer _
                        , ByVal cal_cod_padre As Integer _
                        , ByVal qta_extra_totale As Decimal _
                        , ByVal piva_padre As String _
                        , ByVal sa_cod_padre As Integer _
                        , ByVal id_agenda_padre As Integer _
                        , ByVal id_mov_padre As Integer _
                        , ByVal id_mov_det_padre As Integer _
                        , ByVal DataMovimento As DateTime _
                        , ByVal NumeroDocumento As String _
                        , ByVal DataDocumento As DateTime _
                        , ByVal IntestatarioDocumentoRagioneSociale As String _
                        , ByVal NumeroDocumentoFornitore As String _
                        , ByVal DataDocumentoFornitore As DateTime _
                        , ByVal Visibile As Integer _
                        , ByVal Ordine As Integer _
                        , ByVal Validita_Inizio As DateTime _
                        , ByVal Validita_Fine As DateTime _
                        , ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
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
        Dim Stb As New System.Text.StringBuilder
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
            Stb.Length = 0
            Stb.Append(" INSERT FF_TrackedData_Agenda " + vbCrLf)


            Stb.Append("              (")


            Stb.Append("   [FF_TrackedData_Cod] " & vbCrLf)
            Stb.Append("  ,[piva] " & vbCrLf)
            Stb.Append("  ,[sa_cod] " & vbCrLf)
            Stb.Append("  ,[id_Agenda] " & vbCrLf)
            Stb.Append("  ,[id_mov] " & vbCrLf)
            Stb.Append("  ,[id_mov_det] " & vbCrLf)
            Stb.Append("  ,[Lotto] " & vbCrLf)
            Stb.Append("  ,[Lotto_Padre] " & vbCrLf)
            Stb.Append("  ,[Cal_Cod] " & vbCrLf)
            Stb.Append("  ,[Cal_Cod_Padre] " & vbCrLf)
            Stb.Append("  ,[Qta_Extra_Totale] " & vbCrLf)
            Stb.Append("  ,[piva_padre] " & vbCrLf)
            Stb.Append("  ,[sa_cod_padre] " & vbCrLf)
            Stb.Append("  ,[id_Agenda_padre] " & vbCrLf)
            Stb.Append("  ,[id_mov_padre] " & vbCrLf)
            Stb.Append("  ,[id_mov_det_padre] " & vbCrLf)
            Stb.Append("  ,Data_Movimento " & vbCrLf)
            Stb.Append("  ,Numero_Documento " & vbCrLf)
            Stb.Append("  ,Data_Documento " & vbCrLf)
            Stb.Append("  ,Intestatario_Documento_Ragione_Sociale " & vbCrLf)
            Stb.Append("  ,Numero_Documento_Fornitore " & vbCrLf)
            Stb.Append("  ,Data_Documento_Fornitore " & vbCrLf)
            Stb.Append("  ,Visibile " & vbCrLf)
            Stb.Append("  ,Ordine " & vbCrLf)
            Stb.Append("  ,Inviato " & vbCrLf)
            Stb.Append("  ,datainvio " & vbCrLf)
            Stb.Append("  ,Data_Creazione " & vbCrLf)
            Stb.Append("  ,Data_Modifica " & vbCrLf)
            Stb.Append("  ,UserName_Creazione " & vbCrLf)
            Stb.Append("  ,UserName_Modifica " & vbCrLf)
            Stb.Append("  ,Validita_Inizio " & vbCrLf)
            Stb.Append("  ,Validita_Fine " & vbCrLf)
            Stb.Append("              ) ")

            Stb.Append(" VALUES ( ")

            Stb.Append("  " & Agro_SQL_SaveNum(FF_TrackedData_Cod) & " " & vbCrLf)
            Stb.Append(",  '" & Agro_SQL_SaveText(Piva) & "' " & vbCrLf)
            Stb.Append(", " & Agro_SQL_SaveNum(sa_cod) & " " & vbCrLf)
            Stb.Append(", " & Agro_SQL_SaveNum(id_agenda) & " " & vbCrLf)
            Stb.Append(", " & Agro_SQL_SaveNum(id_mov) & " " & vbCrLf)
            Stb.Append(", " & Agro_SQL_SaveNum(id_mov_det) & " " & vbCrLf)
            Stb.Append(", '" & Agro_SQL_SaveText(lotto) & "' " & vbCrLf)
            Stb.Append(", '" & Agro_SQL_SaveText(lotto_padre) & "' " & vbCrLf)
            Stb.Append(", " & Agro_SQL_SaveNum(cal_cod) & " " & vbCrLf)
            Stb.Append(", " & Agro_SQL_SaveNum(cal_cod_padre) & " " & vbCrLf)
            Stb.Append(", " & Agro_SQL_SaveNum(qta_extra_totale) & " " & vbCrLf)
            Stb.Append(",  '" & Agro_SQL_SaveText(piva_padre) & "' " & vbCrLf)
            Stb.Append(", " & Agro_SQL_SaveNum(sa_cod_padre) & " " & vbCrLf)
            Stb.Append(", " & Agro_SQL_SaveNum(id_agenda_padre) & " " & vbCrLf)
            Stb.Append(", " & Agro_SQL_SaveNum(id_mov_padre) & " " & vbCrLf)
            Stb.Append(", " & Agro_SQL_SaveNum(id_mov_det_padre) & " " & vbCrLf)
            Stb.Append(", " & Agro_SQL_SaveDate(DataMovimento) & " " & vbCrLf)
            Stb.Append(",  '" & Agro_SQL_SaveText(NumeroDocumento) & "' " & vbCrLf)
            Stb.Append(", " & Agro_SQL_SaveDate(DataDocumento) & " " & vbCrLf)
            Stb.Append(",  '" & Agro_SQL_SaveText(IntestatarioDocumentoRagioneSociale) & "' " & vbCrLf)
            Stb.Append(",  '" & Agro_SQL_SaveText(NumeroDocumentoFornitore) & "' " & vbCrLf)
            Stb.Append(", " & Agro_SQL_SaveDate(DataDocumentoFornitore) & " " & vbCrLf)
            Stb.Append(", " & Agro_SQL_SaveNum(Visibile) & " " & vbCrLf)
            Stb.Append(", " & Agro_SQL_SaveNum(Ordine) & " " & vbCrLf)
            Stb.Append("         , 0  " + vbCrLf)
            Stb.Append("         , Null  " + vbCrLf)

            Stb.Append("			, " & Agro_SQL_SaveDate(Data_creazione) & "  ")
            Stb.Append("			, " & Agro_SQL_SaveDate(Data_modifica) & "  ")
            Stb.Append("			,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            Stb.Append("			,'" & Agro_SQL_SaveText(username_modifica) & "' ")

            Stb.Append("			, " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            Stb.Append("			, " & Agro_SQL_SaveDate(Validita_Fine) & "  ")

            Stb.Append(") ")

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







    '#################################################################
    Public Function Cancella( _
                            ByVal FF_TrackedData_Cod As Integer, _
                            ByVal xFiltroAggiuntivo As String, _
                              ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                              ) As Boolean

        '----- Descrizione
        Dim NomeRoutine As String = "Cancella()"

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            Stb.Length = 0

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then
                Stb.Append(" UPDATE FF_TrackedData_Agenda ")
                Stb.Append(" SET ")
                Stb.Append("         Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                Stb.Append("         ,Data_Modifica= " & Agro_SQL_SaveDate(Date.Now) & " ")
                Stb.Append("         ,Inviato = -1 ")
                Stb.Append(" WHERE   1=1 ")
                Stb.Append(" AND     Inviato >= 0 ")
                Stb.Append(" AND     FF_TrackedData_Cod =  " & Agro_SQL_SaveNum(FF_TrackedData_Cod))
            Else
                Stb.Append(" DELETE FROM FF_TrackedData_Agenda ")
                Stb.Append(" WHERE FF_TrackedData_Cod =  " & Agro_SQL_SaveNum(FF_TrackedData_Cod))
            End If
            '---------------------------------------------

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





