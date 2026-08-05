Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi

Public Class HubIoT_WorkOrderData_R
    Inherits DataProvider

    Public Function LeggiElencoWorkerOrderKeyDaRicetteOperazioniXHubIoT(ByVal PivaSuperUser As String,
                                                                        ByVal Ricetta_Cod As Integer,
                                                                        ByVal Ricetta_Operazione_Cod As Integer,
                                                                        ByVal Mac_Cod As Integer,
                                                                        ByVal xFiltroAggiuntivo As String,
                                                                        ByVal xOrderBy As String,
                                                                        ByRef ObjParametri As AgronicaCoreParametri
                                                                        ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.Ricette_R.LeggiElencoWorkerOrderKeyDaRicetteOperazioniXHubIoT()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try


            StrSQL.Length = 0

            StrSQL.AppendLine(" Select ")
            StrSQL.AppendLine(" 	1 as Entita_Origine, ")
            StrSQL.AppendLine(" 	a.Ricetta_SuperUser, ")
            StrSQL.AppendLine("     a.Ricetta_Cod,  ")
            StrSQL.AppendLine("     a.Ricetta_Operazione_Cod,  ")
            StrSQL.AppendLine("     b.Piva,  ")
            StrSQL.AppendLine("     b.Sa_Cod,  ")
            StrSQL.AppendLine("     b.Appezza,  ")
            StrSQL.AppendLine("     b.Id_Reg, ")
            StrSQL.AppendLine("     c.Mat_Cod as Mac_Cod, ")
            StrSQL.AppendLine("     h.Validita_Inizio, ")
            StrSQL.AppendLine("     e.Regola_Elaborazione ")
            StrSQL.AppendLine(" from  ")
            StrSQL.AppendLine("     Ricette_Operazioni a left join  ")
            StrSQL.AppendLine("     Ricette_Destinazioni b On (a.Ricetta_SuperUser=b.Ricetta_SuperUser And a.Ricetta_Cod=b.Ricetta_Cod And a.Ricetta_Operazione_Cod=b.Ricetta_Operazione_Cod And b.Tipo_Destinazione=0) left join  ")
            StrSQL.AppendLine("     Ricette_Dettagli c On (a.Ricetta_SuperUser=c.Ricetta_SuperUser And a.Ricetta_Cod=c.Ricetta_Cod And a.Ricetta_Operazione_Cod=c.Ricetta_Operazione_Cod And c.Cau_Mov=8100) left join  ")
            StrSQL.AppendLine("     Ricette h On (a.Ricetta_SuperUser=h.Ricetta_SuperUser And a.Ricetta_Cod=h.Ricetta_Cod) left join ")
            StrSQL.AppendLine("     HubIoT_RegoleXEntita e on (e.Entita_Origine=1 and e.Categoria_Lavorazione=a.lav_cod) ")
            StrSQL.AppendLine(" where  ")
            StrSQL.AppendLine(" 	a.Invia_HubIoT=1  ")

            If PivaSuperUser <> "" Then
                StrSQL.Append(" AND a.Ricetta_SuperUser = '" & Agro_SQL_SaveText(PivaSuperUser) & "' ")
            End If

            If Ricetta_Cod <> 0 Then
                StrSQL.Append(" AND a.Ricetta_Cod = " & Agro_SQL_SaveNum(Ricetta_Cod) & "   ")
            End If

            If Ricetta_Operazione_Cod <> 0 Then
                StrSQL.Append(" AND a.Ricetta_Operazione_Cod = " & Agro_SQL_SaveNum(Ricetta_Operazione_Cod) & "   ")
            End If

            If Mac_Cod <> 0 Then
                StrSQL.Append(" AND c.Mat_Cod = " & Agro_SQL_SaveNum(Mac_Cod) & "   ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , ObjParametri))
            End If

            '--------------------------------------------------------------------------
            Select Case ObjParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   a.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   a.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, ObjParametri))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(ObjParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(ObjParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function

    Public Function LeggiElencoWorkerOrderKeyDaOperazioniPianificateXHubIoT(ByVal PivaSuperUser As String,
                                                                            ByVal id_agenda As Integer,
                                                                            ByVal Piva As String,
                                                                            ByVal sa_cod As Integer,
                                                                            ByVal appezza As Integer,
                                                                            ByVal id_destinazione As Integer,
                                                                            ByVal xFiltroAggiuntivo As String,
                                                                            ByVal xOrderBy As String,
                                                                            ByRef ObjParametri As AgronicaCoreParametri
                                                                            ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.Ricette_R.LeggiElencoWorkerOrderKeyDaOperazioniPianificateXHubIoT()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try


            StrSQL.Length = 0

            StrSQL.AppendLine(" select ")
            StrSQL.AppendLine(" 	2 as Entita_Origine, ")
            StrSQL.AppendLine(" 	a.Id_Agenda, ")
            StrSQL.AppendLine(" 	a.Id_Mov_Det, ")
            StrSQL.AppendLine(" 	a.piva, ")
            StrSQL.AppendLine(" 	a.Sa_Cod, ")
            StrSQL.AppendLine(" 	a.Appezza, ")
            StrSQL.AppendLine(" 	a.Id_Destinazione, ")
            StrSQL.AppendLine(" 	c1.Mat_Cod as Mac_Cod, ")
            StrSQL.AppendLine(" 	h.Validita_Inizio ")
            StrSQL.AppendLine(" from Mov_Destinazioni a left join ")
            StrSQL.AppendLine(" 	Movimenti b1 on (a.Piva = b1.PIVA and a.Sa_Cod=b1.Sa_Cod and a.Id_Agenda=b1.Id_Agenda and  b1.Cau_Mov=8100) left join ")
            StrSQL.AppendLine(" 	Movimenti_dettagli c1 on (c1.Piva = b1.PIVA and c1.Sa_Cod=b1.Sa_Cod and c1.Id_Agenda=b1.Id_Agenda and c1.Id_Mov=b1.Id_Mov ) left join ")
            StrSQL.AppendLine(" 	Agenda h on (a.Piva=h.PIVA and a.Sa_Cod=h.Sa_Cod and a.Id_Agenda=h.Id_Agenda) ")
            StrSQL.AppendLine(" where ")
            StrSQL.AppendLine(" 	h.Validita_Inizio>GETDATE() ")

            If id_agenda <> 0 Then
                StrSQL.Append(" AND a.id_agenda = " & Agro_SQL_SaveNum(id_agenda) & "   ")
            End If

            If Piva <> "" Then
                StrSQL.Append(" AND a.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If

            If sa_cod <> 0 Then
                StrSQL.Append(" AND a.sa_cod = " & Agro_SQL_SaveNum(sa_cod) & "   ")
            End If

            If appezza <> 0 Then
                StrSQL.Append(" AND a.appezza = " & Agro_SQL_SaveNum(appezza) & "   ")
            End If

            If id_destinazione <> 0 Then
                StrSQL.Append(" AND a.id_destinazione = " & Agro_SQL_SaveNum(id_destinazione) & "   ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , ObjParametri))
            End If

            '--------------------------------------------------------------------------
            Select Case ObjParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   a.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   a.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, ObjParametri))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(ObjParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(ObjParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function


    Public Function GetWorkerOrderDetail_RicetteOperazioni(ByVal PivaSuperUser As String,
                                                            ByVal Piva As String,
                                                            ByVal Ricetta_Cod As Integer,
                                                            ByVal Ricetta_Operazione_Cod As Integer,
                                                            ByVal xFiltroAggiuntivo As String,
                                                            ByVal xOrderBy As String,
                                                            ByRef ObjParametri As AgronicaCoreParametri
                                                           ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreHubIoTDAL.HubIoT_WorkOrderData_R.GetWorkerOrderDetail_RicetteOperazioni()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            StrSQL.Length = 0

            StrSQL.AppendLine(" Select ")
            StrSQL.AppendLine(" 	a.Ricetta_Cod, ")
            StrSQL.AppendLine(" 	a.Ricetta_Operazione_Cod, ")
            StrSQL.AppendLine(" 	a.Ricetta_Operazione_Des, ")
            StrSQL.AppendLine(" 	a.Invia_HubIoT, ")
            StrSQL.AppendLine(" 	a.Lav_Cod, ")
            StrSQL.AppendLine(" 	b.Piva, ")
            StrSQL.AppendLine(" 	imp.rag_soc, ")
            StrSQL.AppendLine(" 	b.Sa_Cod, ")
            StrSQL.AppendLine(" 	centri.Sa_nome, ")
            StrSQL.AppendLine(" 	b.Appezza, ")
            StrSQL.AppendLine(" 	b.Id_Reg, ")
            StrSQL.AppendLine(" 	veg.Veg_Des + ' - ' + cul.Cul_Des as FieldName, ")
            StrSQL.AppendLine(" 	c.Elem_Cod as CategoriaMagazzinoMacchine, ")
            StrSQL.AppendLine(" 	c.Mat_Cod as Mac_Cod, ")
            StrSQL.AppendLine(" 	d.Elem_Cod as CategoriaMagazzinoProdotti, ")
            StrSQL.AppendLine(" 	case when d.Mat_Cod = 0 then d.Pro_Cod else d.Mat_Cod end as Mat_Cod, ")
            StrSQL.AppendLine(" 	m.Mac_Des, ")
            StrSQL.AppendLine(" 	m.VIN, ")
            StrSQL.AppendLine(" 	m.Modello, ")
            StrSQL.AppendLine(" 	m.Codice, ")
            StrSQL.AppendLine(" 	m.Class_Code, ")
            StrSQL.AppendLine(" 	m.HubIoT_PlatformDestination as Platform, ")
            StrSQL.AppendLine(" 	b.Qta as QuantitaProdottoXImpianto, ")
            StrSQL.AppendLine(" 	g1.Entita_Cod, ")
            StrSQL.AppendLine(" 	d.Udm_Cod, ")
            StrSQL.AppendLine(" 	c.Udm_Cod_Extra, ")
            StrSQL.AppendLine(" 	d.Mezzo_Det, ")
            StrSQL.AppendLine(" 	cop.Lav_Cod_Esterno, ")
            StrSQL.AppendLine(" 	cop.Lav_Des_Esterno as CodiceOperazione, ")
            StrSQL.AppendLine(" 	Veg.Veg_Cod, ")
            StrSQL.AppendLine(" 	reg.Cul_Cod, ")
            StrSQL.AppendLine(" 	h.Validita_Inizio, ")
            StrSQL.AppendLine(" 	g2.Poligono_GeoEntity.STAsText() as WKTPoligono, ")
            StrSQL.AppendLine(" 	ag1.allegatiDocumentiXML, ")
            StrSQL.AppendLine(" 	ag1.allegatiDocumentiText ")
            StrSQL.AppendLine(" from ")
            StrSQL.AppendLine(" 	Ricette_Operazioni a left join ")
            StrSQL.AppendLine(" 	Codifica_Operazioni_SistemiEsterni cop On (a.Lav_Cod=cop.Lav_Cod And cop.Sistema_Cod=3) left join ")
            StrSQL.AppendLine(" 	Ricette_Destinazioni b On (a.Ricetta_SuperUser=b.Ricetta_SuperUser And a.Ricetta_Cod=b.Ricetta_Cod And a.Ricetta_Operazione_Cod=b.Ricetta_Operazione_Cod And b.Tipo_Destinazione=0) left join ")
            StrSQL.AppendLine(" 	Ricette_Dettagli c On (a.Ricetta_SuperUser=c.Ricetta_SuperUser And a.Ricetta_Cod=c.Ricetta_Cod And a.Ricetta_Operazione_Cod=c.Ricetta_Operazione_Cod And c.Cau_Mov=8100) left join ")
            StrSQL.AppendLine(" 	Parco_Macchine m On (c.Mat_Cod=m.Mac_Cod) left join ")
            StrSQL.AppendLine(" 	Ricette_Dettagli d On (a.Ricetta_SuperUser=d.Ricetta_SuperUser And a.Ricetta_Cod=d.Ricetta_Cod And a.Ricetta_Operazione_Cod=d.Ricetta_Operazione_Cod And d.Cau_Mov In (2050,2200,2300)) left join ")
            StrSQL.AppendLine(" 	UnitaMisura e On (d.Udm_Cod=e.UDM_COD) left join ")
            StrSQL.AppendLine(" 	Ricette h On (a.Ricetta_SuperUser=h.Ricetta_SuperUser And a.Ricetta_Cod=h.Ricetta_Cod) left join ")
            StrSQL.AppendLine(" 	GIS_Entita g1 On (b.Piva=g1.Piva And b.Sa_Cod=g1.Sa_Cod And b.Appezza=g1.Appezza And b.Id_Reg=g1.Id_Imp And g1.TipoEntita_Cod In (19,20,21,22,23)) left join ")
            StrSQL.AppendLine(" 	GIS_ElementiGrafici g2 On (g1.PivaSuperUser=g2.PivaSuperUser And g1.Entita_Cod=g2.Entita_Cod) left join ")
            StrSQL.AppendLine(" 	Alert_Entita al1 On (b.Piva = al1.Piva And b.Sa_Cod = al1.Sa_Cod And b.Appezza = al1.Appezza And b.Id_Reg = al1.Id_Imp And b.Ricetta_Operazione_Cod=al1.Ricetta_Operazione_cod And al1.TipoEntita_Cod=13) left join ")
            StrSQL.AppendLine(" 	Allegati_Documenti ag1 On (al1.PivaSuperUser=ag1.Allegati_Documenti_SuperUser And al1.Piva=ag1.Allegati_Documenti_Piva And al1.Allegati_Documenti_Cod=ag1.Allegati_Documenti_Cod) left join ")
            StrSQL.AppendLine(" 	Imprese imp On (b.Piva=imp.PIVA) left join ")
            StrSQL.AppendLine(" 	Centri_Aziendali centri On (b.Piva=centri.PIVA and b.sa_cod=centri.sa_cod) left join ")
            StrSQL.AppendLine(" 	Reg_Impianti reg On (b.Piva=reg.PIVA and b.sa_cod=reg.sa_cod and b.appezza=reg.appezza and b.id_reg=reg.id_reg) left join ")
            StrSQL.AppendLine(" 	Cultivar cul On (reg.cul_cod=cul.cul_cod) left join ")
            StrSQL.AppendLine(" 	SpecieVegetali veg On (cul.veg_cod=veg.veg_cod) ")
            StrSQL.AppendLine(" where ")
            StrSQL.AppendLine("     a.Invia_HubIoT=1 ")

            If PivaSuperUser <> "" Then
                StrSQL.Append(" AND a.Ricetta_SuperUser = '" & Agro_SQL_SaveText(PivaSuperUser) & "' ")
            End If

            If Piva <> "" Then
                StrSQL.Append(" AND b.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If

            If Ricetta_Cod <> 0 Then
                StrSQL.Append(" AND a.Ricetta_Cod = " & Agro_SQL_SaveNum(Ricetta_Cod) & "   ")
            End If

            If Ricetta_Operazione_Cod <> 0 Then
                StrSQL.Append(" AND a.Ricetta_Operazione_Cod = " & Agro_SQL_SaveNum(Ricetta_Operazione_Cod) & "   ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , ObjParametri))
            End If

            '--------------------------------------------------------------------------
            Select Case ObjParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   a.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   a.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, ObjParametri))
            Else
                StrSQL.Append(" ORDER BY a.Ricetta_Cod,a.Ricetta_Operazione_Cod,c.Mat_Cod ")
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(ObjParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------


        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(ObjParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT
    End Function

    Public Function GetWorkerOrderDetail_OperazioniPianificate(ByVal PivaSuperUser As String,
                                                               ByVal Piva As String,
                                                               ByVal id_agenda As Integer,
                                                               ByVal id_mov_det As Integer,
                                                               ByVal xFiltroAggiuntivo As String,
                                                               ByVal xOrderBy As String,
                                                               ByRef ObjParametri As AgronicaCoreParametri
                                                               ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreHubIoTDAL.HubIoT_WorkOrderData_R.GetWorkerOrderDetail_OperazioniPianificate()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            StrSQL.Length = 0

            StrSQL.AppendLine(" Select ")
            StrSQL.AppendLine("     a.id_agenda, ")
            StrSQL.AppendLine("     a.id_mov_det, ")
            StrSQL.AppendLine("     h.des_lib, ")
            StrSQL.AppendLine("     1 as Invia_HubIoT, ")
            StrSQL.AppendLine("     h.Lav_Cod, ")
            StrSQL.AppendLine("     a.Piva, ")
            StrSQL.AppendLine("     imp.rag_soc, ")
            StrSQL.AppendLine("     a.Sa_Cod, ")
            StrSQL.AppendLine("     centri.Sa_nome, ")
            StrSQL.AppendLine("     a.Appezza, ")
            StrSQL.AppendLine("     a.Id_Destinazione, ")
            StrSQL.AppendLine("     veg.Veg_Des + ' - ' + cul.Cul_Des as FieldName, ")
            StrSQL.AppendLine("     c2.Elem_Cod as CategoriaMagazzinoProdotti, ")
            StrSQL.AppendLine(" 	case when c2.Mat_Cod = 0 then c2.Pro_Cod else c2.Mat_Cod end as Mat_Cod, ")
            StrSQL.AppendLine(" 	c1.Elem_Cod as CategoriaMagazzinoMacchine, ")
            StrSQL.AppendLine("     c1.Mat_Cod as Mac_Cod, ")
            StrSQL.AppendLine("     m.Mac_Des, ")
            StrSQL.AppendLine("     m.VIN, ")
            StrSQL.AppendLine("     m.Modello, ")
            StrSQL.AppendLine("     m.Codice, ")
            StrSQL.AppendLine("     m.Class_Code, ")
            StrSQL.AppendLine("     m.HubIoT_PlatformDestination as Platform, ")
            StrSQL.AppendLine("     a.Qta as QuantitaProdottoXImpianto, ")
            StrSQL.AppendLine("     g1.Entita_Cod, ")
            StrSQL.AppendLine(" 	c2.Udm_Cod, ")
            StrSQL.AppendLine(" 	'' as Udm_Cod_Extra, ")
            StrSQL.AppendLine(" 	0 as Mezzo_Det, ")
            StrSQL.AppendLine("     cop.Lav_Cod_Esterno, ")
            StrSQL.AppendLine("     cop.Lav_Des_Esterno as CodiceOperazione, ")
            StrSQL.AppendLine("     veg.Veg_Cod, ")
            StrSQL.AppendLine("     reg.Cul_Cod, ")
            StrSQL.AppendLine("     h.Validita_Inizio, ")
            StrSQL.AppendLine("     g2.Poligono_GeoEntity.STAsText() as WKTPoligono, ")
            StrSQL.AppendLine("     ag1.allegatiDocumentiXML, ")
            StrSQL.AppendLine(" 	ag1.allegatiDocumentiText ")
            StrSQL.AppendLine(" from ")
            StrSQL.AppendLine(" 	Mov_Destinazioni a left join ")
            StrSQL.AppendLine(" 	Movimenti b1 on (a.Piva = b1.PIVA and a.Sa_Cod=b1.Sa_Cod and a.Id_Agenda=b1.Id_Agenda and  b1.Cau_Mov=8100) left join ")
            StrSQL.AppendLine(" 	Movimenti_dettagli c1 on (c1.Piva = b1.PIVA and c1.Sa_Cod=b1.Sa_Cod and c1.Id_Agenda=b1.Id_Agenda and c1.Id_Mov=b1.Id_Mov ) left join ")
            StrSQL.AppendLine(" 	Agenda h on (a.Piva=h.PIVA and a.Sa_Cod=h.Sa_Cod and a.Id_Agenda=h.Id_Agenda) left join")
            StrSQL.AppendLine("     Codifica_Operazioni_SistemiEsterni cop On (h.Lav_Cod=cop.Lav_Cod And cop.Sistema_Cod=3) left join ")
            StrSQL.AppendLine(" 	Parco_Macchine m On (c1.Mat_Cod=m.Mac_Cod) left join ")
            StrSQL.AppendLine(" 	Movimenti b2 on (a.Piva = b2.PIVA and a.Sa_Cod=b2.Sa_Cod and a.Id_Agenda=b2.Id_Agenda and  b2.Cau_Mov in (2050,2200,2300)) left join ")
            StrSQL.AppendLine(" 	Movimenti_dettagli c2 on (c2.Piva = b2.PIVA and c2.Sa_Cod=b2.Sa_Cod and c2.Id_Agenda=b2.Id_Agenda and c2.Id_Mov=b2.Id_Mov ) left join ")
            StrSQL.AppendLine(" 	UnitaMisura e On (c2.Udm_Cod=e.UDM_COD) left join ")
            StrSQL.AppendLine(" 	GIS_Entita g1 On (a.Piva=g1.Piva And a.Sa_Cod=g1.Sa_Cod And a.Appezza=g1.Appezza And a.Id_Destinazione=g1.Id_Imp And g1.TipoEntita_Cod In (19,20,21,22,23)) left join ")
            StrSQL.AppendLine(" 	GIS_ElementiGrafici g2 On (g1.PivaSuperUser=g2.PivaSuperUser And g1.Entita_Cod=g2.Entita_Cod) left join ")
            StrSQL.AppendLine(" 	Alert_Entita al1 On (a.Piva = al1.Piva And a.Sa_Cod = al1.Sa_Cod And a.Appezza = al1.Appezza And a.Id_Destinazione = al1.Id_Imp And a.Id_Agenda=al1.ID_Agenda And al1.TipoEntita_Cod=51) left join ")
            StrSQL.AppendLine("     Allegati_Documenti ag1 On (al1.PivaSuperUser=ag1.Allegati_Documenti_SuperUser And al1.Piva=ag1.Allegati_Documenti_Piva And al1.Allegati_Documenti_Cod=ag1.Allegati_Documenti_Cod) left join ")
            StrSQL.AppendLine("     Imprese imp On (a.Piva=imp.PIVA) left join ")
            StrSQL.AppendLine("     Centri_Aziendali centri On (a.Piva=centri.PIVA and a.sa_cod=centri.sa_cod) left join ")
            StrSQL.AppendLine("     Reg_Impianti reg On (a.Piva=reg.PIVA and a.sa_cod=reg.sa_cod and a.appezza=reg.appezza and a.Id_Destinazione=reg.id_reg) left join ")
            StrSQL.AppendLine("     Cultivar cul On (reg.cul_cod=cul.cul_cod) left join ")
            StrSQL.AppendLine("     SpecieVegetali veg On (cul.veg_cod=veg.veg_cod) ")
            StrSQL.AppendLine(" where ")
            StrSQL.AppendLine(" 	h.Validita_Inizio>GETDATE() ")

            If PivaSuperUser <> "" Then
                StrSQL.Append(" AND a.Ricetta_SuperUser = '" & Agro_SQL_SaveText(PivaSuperUser) & "' ")
            End If

            If Piva <> "" Then
                StrSQL.Append(" AND a.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If

            If id_agenda <> 0 Then
                StrSQL.Append(" AND a.id_agenda = " & Agro_SQL_SaveNum(id_agenda) & "   ")
            End If

            If id_mov_det <> 0 Then
                StrSQL.Append(" AND a.id_mov_det = " & Agro_SQL_SaveNum(id_mov_det) & "   ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , ObjParametri))
            End If

            '--------------------------------------------------------------------------
            Select Case ObjParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   a.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   a.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, ObjParametri))
            Else
                StrSQL.Append(" ORDER BY a.Ricetta_Cod,a.Ricetta_Operazione_Cod,c.Mat_Cod ")
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(ObjParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------


        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(ObjParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT
    End Function

    Public Function LeggiElencoMacchineSuRicette_HubIoT(ByVal PivaSuperUser As String,
                                                        ByVal Ricetta_Cod As Integer,
                                                        ByVal xFiltroAggiuntivo As String,
                                                        ByVal xOrderBy As String,
                                                        ByRef ObjParametri As AgronicaCoreParametri
                                                        ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreHubIoTDAL.HubIoT_WorkOrderData_R.LeggiElencoMacchineSuRicette_HubIoT()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try


            StrSQL.Length = 0

            StrSQL.AppendLine(" select ")
            StrSQL.AppendLine(" 	a.Ricetta_SuperUser as PivaSuperUser, ")
            StrSQL.AppendLine(" 	a.Ricetta_Cod, ")
            StrSQL.AppendLine(" 	a.Elem_Cod, ")
            StrSQL.AppendLine(" 	a.Mat_Cod, ")
            StrSQL.AppendLine(" 	b.Class_Code, ")
            StrSQL.AppendLine(" 	b.Mac_Des, ")
            StrSQL.AppendLine(" 	b.Modello, ")
            StrSQL.AppendLine(" 	b.Codice, ")
            StrSQL.AppendLine(" 	b.VIN ")
            StrSQL.AppendLine(" from ricette_dettagli a ")
            StrSQL.AppendLine(" 	left join Parco_Macchine b on ")
            StrSQL.AppendLine(" 	(a.mat_cod=b.mac_cod) ")
            StrSQL.AppendLine(" where a.cau_mov=8100 ")

            If PivaSuperUser <> "" Then
                StrSQL.Append(" AND a.Ricetta_SuperUser = '" & Agro_SQL_SaveText(PivaSuperUser) & "' ")
            End If

            If Ricetta_Cod <> 0 Then
                StrSQL.Append(" AND a.Ricetta_Cod = " & Agro_SQL_SaveNum(Ricetta_Cod) & "   ")
            End If


            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , ObjParametri))
            End If

            '--------------------------------------------------------------------------
            Select Case ObjParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   a.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   a.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, ObjParametri))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(ObjParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(ObjParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function


End Class

Public Class HubIoT_WorkOrderData_W
    Inherits DataProvider

    Public Function Aggiorna_RicettaOperazione_FlagInvio_HubIot(ByVal Ricetta_Cod As Int32,
                                                                ByVal Ricetta_Operazione_Cod As Int32,
                                                                ByVal Invia_HubIoT As Integer,
                                                                ByVal xFiltroAggiuntivo As String,
                                                                ByRef objParametri As AgronicaCoreParametri
                                                                ) As Boolean
        Dim NomeRoutine As String = "AgronicaCoreContabDAL.Ricette_Operazioni_W.Aggiorna_RicettaOperazione_FlagInvio_HubIot()"

        '====================================================================================
        'Parametri opzionali :
        '   Ricetta_SuperUser = ""
        '   Ricetta_Cod = 0
        '   Ricetta_Operazione_Cod = 0
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If objParametri.PivaSuperUser = "" Then
                Throw New Exception("Parametro non corretto nella query (Ricetta_SuperUser obbligatorio)")
            End If

            '---------------------------------------------

            StrSQL.Length = 0

            StrSQL.Append(" UPDATE Ricette_Operazioni SET ")
            StrSQL.Append("    Invia_HubIoT                  =  " & Agro_SQL_SaveNum(Invia_HubIoT) & "  ")

            StrSQL.Append("   ,Inviato           =  0 ")
            StrSQL.Append("   ,DataInvio         =  Null ")
            StrSQL.Append("   ,Data_Modifica     =  " & Agro_SQL_SaveDateTime(Now))
            StrSQL.Append("   ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")


            StrSQL.Append(" WHERE  Ricetta_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

            If Ricetta_Cod <> 0 Then
                StrSQL.Append(" AND Ricette_Operazioni.Ricetta_Cod = " & Agro_SQL_SaveNum(Ricetta_Cod) & "   ")
            End If

            If Ricetta_Operazione_Cod <> 0 Then
                StrSQL.Append(" AND Ricette_Operazioni.Ricetta_operazione_Cod = " & Agro_SQL_SaveNum(Ricetta_Operazione_Cod) & "   ")
            End If

            '----------------------------------------------------------------------
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
