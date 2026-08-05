Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.DataProviderExtensions
Imports System.Text
Imports AgronicaCoreDataProvider
Imports Newtonsoft.Json
Imports System.Data.SqlClient

Public Class CostiAccessori_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi(
                                    ByVal Piva As String,
                                    ByVal Sa_Cod As Integer,
                                    ByVal Id_Agenda As Integer,
                                    ByVal Id_Mov As Integer,
                                    ByVal Id_Mov_Det As Integer,
                                    ByVal Elem_Cod As Integer,
                                    ByVal Cau_Mov As String,
                                    ByVal Mat_Cod As Integer,
                                    ByVal Cod_Contatto As String,
                                    ByVal Piva_Contatto As String,
                                    ByVal Piva_Macchina As String,
                                    ByVal Mac_Cod_Origine As Long,
                                    ByVal Cod_RisUm_Origine As Integer,
                                    ByVal Piva_SuperUser_Origine As String,
                                    ByVal FiltroAggiuntivo As String,
                                    ByVal Ordinamento As String,
                                    ByVal Flag_Manodopera As Boolean,
                                    ByVal Flag_Macchine As Boolean,
                                    ByVal Flag_AncheImportati As Boolean,
                                    ByVal agroParam As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable




        'Dim objSQL As New Codex_Utility.Sql
        Dim StrSQL As String = ""
        Dim NomeRoutine As String = "AgronicaCoreContabDAL.CostiAccessori_R.Leggi"
        'Dim MessaggioErrore As String = ""
        'Dim DT As New DataTable
        'Dim i As Integer = 0

        '----------------------------------------------------
        '--- Preparo la Query SQL ---------------------------
        '----------------------------------------------------
        Try
            StrSQL = ""
            StrSQL += " SELECT     Imprese.rag_soc AS Impresa, Agenda.PIVA, Agenda.Sa_Cod, Agenda.Id_Agenda, Agenda.Lav_Cod, Agenda.des_lib, Movimenti.Id_Mov,    "
            StrSQL += "  Movimenti.Cau_Mov, Movimenti.Mov_Desc, Movimenti.Data_Movimento, Movimenti_dettagli.Id_Mov_Det, Movimenti_dettagli.Elem_Cod,    "
            StrSQL += "  Movimenti_dettagli.Pro_Cod, Movimenti_dettagli.Mat_Cod, Movimenti_dettagli.Udm_Cod, Movimenti_dettagli.Qta, Movimenti_dettagli.Mov_Det_Des,   "
            StrSQL += "   Movimenti_dettagli.Prezzo_Unitario AS Prezzo_Unitario_Dettagli, Movimenti_dettagli.Cod_Progetto, Movimenti_dettagli.Cal_Cod,   "
            StrSQL += "  Movimenti_dettagli.Prezzo_Unitario_Netto,     "
            StrSQL += " Operazioni.LAV_DES, GruppoOperazioni.GRU_COD, GruppoOperazioni.GRU_DES, GruppoOperazioni.Tipo, GruppoOperazioni.ATT_COD, "
            StrSQL += "  ISNULL(Prodotti_Costi.Mezzo, -1) AS Mezzo,  ISNULL(Prodotti_Costi.Prezzo_Unitario, 0) AS Prezzo_Unitario,  "
            StrSQL += "  ISNULL(Prodotti_Costi.Validita_Inizio, '01/01/1900') AS Costo_Inizio, ISNULL(Prodotti_Costi.Validita_Fine, '31/12/2100') AS Costo_Fine  "

            If Flag_Manodopera = True Then

                StrSQL += " , Risorse_Umane.Cod_RisUm, Risorse_Umane.Settore_Des, Risorse_Umane.Attivita_Des,   "
                StrSQL += " Risorse_Umane.Patentino, Risorse_Umane.Data_Rilascio_Patentino, Risorse_Umane.Data_Scadenza_Patentino, Contatti.Piva AS Piva_Contatto,    "
                StrSQL += " Contatti.Sa_Cod AS SaCod_Contatto, Contatti.Cod_Contatto, Contatti.Id_CF, Contatti.Rag_Soc, Contatti.Codice_Fiscale,     "
                StrSQL += " Risorse_Umane.Cod_Rapporto, Rapporti_Contabili.Rapporto_Des  "

            End If

            If Flag_Macchine = True Then

                StrSQL += " , Macchine.CLASS_CODE, Macchine.CLASS_DESC, Parco_Macchine.Mac_Des, Parco_Macchine.Costo_Acquisto, Parco_Macchine.Targa,  "
                StrSQL += "   Parco_Macchine.Telaio, Parco_Macchine.Ditta_Cod, Parco_Macchine.Modello, Parco_Macchine.Potenza, Parco_Macchine.Ammortamento,   "
                StrSQL += "   Parco_Macchine.Ammortizzato, Parco_Macchine.Data_Immatricolazione, Parco_Macchine.Ultima_Manutenzione, Parco_Macchine.Ultima_Revisione,  "
                StrSQL += "   Parco_Macchine.Stato_Utilizzo, Parco_Macchine.Note, Parco_Macchine.Tipo AS Tipo_Macchina, Parco_Macchine.Piva AS Piva_Macchina,   "
                StrSQL += "   Parco_Macchine.Sa_Cod AS SaCod_Macchina  "
                StrSQL += "    "
                StrSQL += "    "

            End If

            '-------------------------------------------------

            'JOIN AGENDA - MOVIMENTI
            StrSQL += " FROM    Agenda INNER JOIN Movimenti "
            StrSQL += " ON Agenda.PIVA = Movimenti.PIVA AND Agenda.Sa_Cod = Movimenti.Sa_Cod AND Agenda.Id_Agenda = Movimenti.Id_Agenda "

            'JOIN IMPRESE - AGENDA
            StrSQL += " INNER JOIN Imprese ON Imprese.Piva = Agenda.Piva "

            'JOIN AGENDA - OPERAZIONI
            StrSQL += " INNER JOIN Operazioni ON Agenda.Lav_Cod = Operazioni.LAV_COD "

            'JOIN OPERAZIONI - GRUPPO OPERAZIONI
            StrSQL += " INNER JOIN GruppoOperazioni ON Operazioni.GRU_OP = GruppoOperazioni.GRU_COD "

            'JOIN MOVIMENTI - MOVIMENTI_DETTAGLI
            StrSQL += " INNER JOIN Movimenti_dettagli "
            StrSQL += " ON Movimenti_dettagli.PIVA = Movimenti.PIVA AND Movimenti_dettagli.Id_Agenda = Movimenti.Id_Agenda AND Movimenti_dettagli.Id_Mov = Movimenti.Id_Mov "

            'JOIN MOVIMENTI_DETTAGLI - PRODOTTI COSTI
            StrSQL += " LEFT OUTER JOIN Prodotti_Costi "
            StrSQL += " ON Movimenti_dettagli.Elem_Cod = Prodotti_Costi.Elem_Cod AND Prodotti_Costi.Mat_Cod = Movimenti_dettagli.Mat_Cod And Prodotti_Costi.Id_Budget = 0 "

            If Flag_Manodopera = True Then

                'JOIN MOVIMENTI DETTAGLI - RISORSE UMANE
                'non mettere in join la piva, mi raccomando!!!!!!
                StrSQL += " INNER JOIN Risorse_Umane ON Risorse_Umane.Cod_RisUm = Movimenti_dettagli.Mat_Cod "

                'JOIN RISORSE UMANE - CONTATTI
                StrSQL += " INNER JOIN Contatti ON Risorse_Umane.Piva = Contatti.Piva AND Risorse_Umane.Cod_Contatto = Contatti.Cod_Contatto "

                'JOIN RISORSE UMANE - RAPPORTI CONTABILI
                StrSQL += " INNER JOIN Rapporti_Contabili ON Risorse_Umane.Cod_Rapporto = Rapporti_Contabili.Cod_Rapporto "

            End If

            If Flag_Macchine = True Then

                'JOIN MOVIMENTI DETTAGLI - PARCO MACCHINE
                'non mettere in join la piva, mi raccomando!!!!!!
                StrSQL += " INNER JOIN Parco_Macchine  "
                StrSQL += "  ON Movimenti_dettagli.Mat_Cod = Parco_Macchine.Mac_Cod  "

                'JOIN PARCO MACCHINE - MACCHINE
                StrSQL += " INNER JOIN Macchine  "
                StrSQL += "  ON Macchine.CLASS_CODE = Parco_Macchine.Class_Code   "

            End If


            '-----------------------------------------

            'CONDIZIONI
            StrSQL += " WHERE Movimenti.Validita_Inizio <= " & Agro_SQL_SaveDate(agroParam.FinestraTemporaleFine) & " "
            StrSQL += " AND   Movimenti.Validita_Fine >= " & Agro_SQL_SaveDate(agroParam.FinestraTemporaleInizio) & " "

            StrSQL += " AND     Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(agroParam.FinestraTemporaleFine) & " "
            StrSQL += " AND     Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(agroParam.FinestraTemporaleInizio) & " "

            'StrSQL += " AND     Prodotti_Costi.Validita_Inizio <= " & SQL_SaveDate(FinestraTemp_Fine) & " "
            'StrSQL += " AND     Prodotti_Costi.Validita_Fine >= " & SQL_SaveDate(FinestraTemp_Inizio) & " "

            If Flag_Manodopera = True Then

                StrSQL += "  AND   Rapporti_Contabili.Piva = '" & Agro_SQL_SaveText(agroParam.PivaSuperUser) & "'   "

                If Cod_Contatto <> "" Then
                    StrSQL += " AND Contatti.Cod_Contatto = '" & Agro_SQL_SaveText(Cod_Contatto) & "'   "
                End If

                If Piva_Contatto <> "" Then
                    StrSQL += " AND Contatti.Piva = '" & Agro_SQL_SaveText(Piva_Contatto) & "'   "
                End If

                'gias2gias
                If Cod_RisUm_Origine <> 0 Then
                    StrSQL += " AND  Risorse_Umane.Cod_RisUm_Origine = " & Agro_SQL_SaveNum(Cod_RisUm_Origine) & "   "
                End If
                If Piva_SuperUser_Origine <> "" Then
                    StrSQL += " AND  Risorse_Umane.Piva_SuperUser_Origine = '" & Agro_SQL_SaveText(Piva_SuperUser_Origine) & "'"
                End If
                If Flag_AncheImportati = False Then
                    StrSQL += " AND  Risorse_Umane.Cod_RisUm_Origine = 0 "
                End If
                'fine gias2gias

            End If

            If Flag_Macchine = True Then

                If Piva_Macchina <> "" Then
                    StrSQL += " AND Parco_Macchine.Piva = '" & Agro_SQL_SaveText(Piva_Macchina) & "'   "
                End If

                'gias2gias
                If Mac_Cod_Origine <> 0 Then
                    StrSQL += " AND  Parco_Macchine.Mac_Cod_Origine = " & Agro_SQL_SaveNum(Mac_Cod_Origine) & "   "
                End If
                If Piva_SuperUser_Origine <> "" Then
                    StrSQL += " AND  Parco_Macchine.Piva_SuperUser_Origine = '" & Agro_SQL_SaveText(Piva_SuperUser_Origine) & "'"
                End If
                If Flag_AncheImportati = False Then
                    StrSQL += " AND  Parco_Macchine.Mac_Cod_Origine = 0 "
                End If
                'fine gias2gias

            End If

            If Piva <> "" Then
                StrSQL += " AND Movimenti_Dettagli.Piva = '" & Agro_SQL_SaveText(Piva) & "'   "
            End If

            If Sa_Cod <> 0 Then
                StrSQL += " AND Movimenti.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   "
            End If

            If Id_Agenda <> 0 Then
                StrSQL += " AND Movimenti_Dettagli.Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & "   "
            End If

            If Id_Mov <> 0 Then
                StrSQL += " AND Movimenti_Dettagli.Id_Mov = " & Agro_SQL_SaveNum(Id_Mov) & "   "
            End If

            If Id_Mov_Det <> 0 Then
                StrSQL += " AND Movimenti_Dettagli.Id_Mov_Det = " & Agro_SQL_SaveNum(Id_Mov_Det) & "   "
            End If

            'se mi interessa la manodopera
            'devo filtrare elem_cod = 0
            StrSQL += " AND Movimenti_Dettagli.Elem_Cod = " & Agro_SQL_SaveNum(Elem_Cod) & "   "

            If Mat_Cod <> 0 Then
                StrSQL += " AND Movimenti_Dettagli.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & "   "
            End If

            If Cau_Mov <> "" Then
                StrSQL += " AND Movimenti.Cau_Mov = '" & Agro_SQL_SaveText(Cau_Mov) & "'   "
            End If

            If FiltroAggiuntivo <> "" Then
                StrSQL += FiltroAggiuntivo
            End If

            If Ordinamento <> "" Then
                StrSQL += Ordinamento
            Else
                StrSQL += " ORDER BY Agenda.PIVA, Movimenti.Data_Movimento DESC "
            End If


            Return MyBase.EseguiQuery_Lettura(agroParam, StrSQL, NomeRoutine)

        Catch ex As Exception


            MyBase.Scrivi_LOG(agroParam, NomeRoutine, ex.Message)
            Throw New Exception("[" & NomeRoutine & "] : " & ex.Message)
            Return Nothing

        End Try



    End Function

    '################################################################################
    '16/09/2019: nuova gestione patentino
    'data inizio e fine servono per filtrare le validità del patentino
    'a differenza dell'altra gli viene passato un solo id_agenda
    Public Function CostiAccessori_from_IdAgenda(ByVal Piva As String,
                                                 ByVal IdAgenda As Integer,
                                                 ByVal Data_Inizio As Date,
                                                 ByVal Data_Fine As Date,
                                                    ByVal xFiltroAggiuntivo As String,
                                                    ByVal xOrderBy As String,
                                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                    ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.CostiAccessori.CostiAccessori_from_IdAgenda()"
        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT Elem_Cod, Movimenti.Cau_Mov, Movimenti_dettagli.Id_Agenda, Movimenti_dettagli.Id_Mov_Det, ")
            StrSQL.AppendLine(" Risorse_Umane.Cod_RisUm, Contatti.Cod_Contatto, Contatti.Rag_Soc, Contatti.Nome, Contatti.Cognome, ")
            '16/09/2019: nuova gestione patentino
            'StrSQL.AppendLine(" Risorse_Umane.Patentino, Risorse_Umane.Data_Rilascio_Patentino, Risorse_Umane.Data_Scadenza_Patentino, ")
            StrSQL.AppendLine(" ISNULL( PATENTINO.Allegati_Documenti_Numero, '')  AS Patentino, ISNULL(PATENTINO.Validazione_Data, '01/01/1900') AS Data_Rilascio_Patentino, ISNULL(PATENTINO.Data_Scadenza, '31/12/2100') AS Data_Scadenza_Patentino,   ")
            StrSQL.AppendLine(" '' AS Mac_Des, 0 AS Mac_Cod, '' AS Class_Code, '' AS CLASS_DESC, '' AS Modello, '' AS Ultima_Manutenzione, 0 AS Ditta_cod, '' AS Ditta_Des ")
            StrSQL.AppendLine(" FROM Movimenti  ")
            StrSQL.AppendLine(" INNER JOIN Movimenti_dettagli ON Movimenti.Id_Agenda = Movimenti_dettagli.Id_Agenda AND Movimenti.Id_Mov = Movimenti_dettagli.Id_Mov AND Movimenti.Piva = Movimenti_dettagli.Piva ")
            StrSQL.AppendLine(" INNER JOIN Risorse_Umane ON Movimenti_dettagli.Mat_Cod = Risorse_Umane.Cod_RisUm ")
            StrSQL.AppendLine(" INNER JOIN Contatti ON Contatti.Cod_Contatto = Risorse_Umane.Cod_Contatto AND Contatti.Piva = Risorse_Umane.Piva")
            StrSQL.AppendLine("  ")
            StrSQL.AppendLine(" LEFT OUTER JOIN ")
            StrSQL.AppendLine("             (SELECT alert_entita.Cod_Contatto, alert_entita.piva, Allegati_Documenti.validazione_data, allegati_documenti.Allegati_Documenti_Numero, Alert_Elenco.Data_Scadenza  ")
            StrSQL.AppendLine("             FROM alert_entita ")
            StrSQL.AppendLine("             INNER JOIN Alert_Elenco ON Alert_Elenco.PivaSuperUser = Alert_Entita.PivaSuperUser AND Alert_Elenco.ID_Alert_Entita = Alert_Entita.ID_Alert_Entita   ")
            StrSQL.AppendLine("             INNER JOIN Allegati_Documenti ON Allegati_Documenti.Allegati_Documenti_SuperUser = Alert_Entita.PivaSuperUser and Allegati_Documenti.Allegati_Documenti_Piva = Alert_Entita.piva  and Allegati_Documenti.Allegati_Documenti_Cod = Alert_Entita.Allegati_Documenti_Cod  ")
            StrSQL.AppendLine("             WHERE tipoentita_cod=8 ")
            StrSQL.AppendLine("             AND Allegati_Documenti.Allegati_documenti_CatCod=2 ")
            StrSQL.AppendLine("             AND Allegati_Documenti.Validazione_Data <= " & Agro_SQL_SaveDate(Data_Fine) & " ")
            StrSQL.AppendLine("             AND Alert_Elenco.Data_Scadenza >= " & Agro_SQL_SaveDate(Data_Inizio) & "  ")
            StrSQL.AppendLine("             ) PATENTINO on PATENTINO.Cod_Contatto=Contatti.Cod_Contatto and PATENTINO.piva = Contatti.piva ")
            StrSQL.AppendLine("  ")
            'serve rifare il filtro cau_mov per escludere qui quelli del parco macchine
            StrSQL.AppendLine(" WHERE Movimenti.Cau_Mov IN ('" & CAU_IMPUTAZIONE_MANODOPERA & "','" & CAU_IMPUTAZIONE_TERZISTI & "','" & CAU_IMPUTAZIONE_TECNICO_RESPONSABILE & "') ")
            StrSQL.AppendLine(" AND Movimenti.Id_Agenda = " + Agro_SQL_SaveNum(IdAgenda) + " ")
            StrSQL.AppendLine(" AND Movimenti.Piva = '" + Agro_SQL_SaveText(Piva) + "' ")
            StrSQL.AppendLine("  ")
            StrSQL.AppendLine(" UNION ALL ")
            StrSQL.AppendLine("  ")
            StrSQL.AppendLine(" SELECT Elem_Cod, Movimenti.Cau_Mov, Movimenti_dettagli.Id_Agenda, Movimenti_dettagli.Id_Mov_Det, ")
            StrSQL.AppendLine("  0 as Cod_RisUm, '' as Cod_Contatto, '' as Rag_Soc, '' as Nome, '' as Cognome, '' as Patentino, '' as Data_Rilascio_Patentino, '' as Data_Scadenza_Patentino,  ")
            StrSQL.AppendLine(" Parco_Macchine.Mac_Des, Parco_Macchine.Mac_Cod, ")
            StrSQL.AppendLine(" Parco_Macchine.Class_Code, Macchine.CLASS_DESC, Parco_Macchine.Modello, Parco_Macchine.Ultima_Manutenzione ")
            StrSQL.AppendLine(" , isnull(Parco_Macchine.Ditta_cod,0) AS Ditta_cod, ISNULL(Ditte.Ditta_des, '') as Ditta_des ")
            StrSQL.AppendLine(" FROM Parco_Macchine INNER JOIN ")
            StrSQL.AppendLine(" Movimenti_dettagli ON Parco_Macchine.Mac_Cod = Movimenti_dettagli.Mat_Cod INNER JOIN ")
            StrSQL.AppendLine(" Movimenti ON Movimenti_dettagli.Id_Agenda = Movimenti.Id_Agenda AND ")
            StrSQL.AppendLine(" Movimenti_dettagli.Id_Mov = Movimenti.Id_Mov INNER JOIN ")
            StrSQL.AppendLine(" Macchine ON Parco_Macchine.Class_Code = Macchine.CLASS_CODE ")
            StrSQL.AppendLine(" LEFT JOIN Ditte ON Parco_Macchine.Ditta_Cod= Ditte.Ditta_Cod ")
            'serve rifare il filtro cau_mov per escludere qui quelli del parco macchine
            StrSQL.AppendLine(" WHERE Movimenti.Cau_Mov = '" & CAU_IMPUTAZIONE_PARCOMACCHINE & "' ")
            StrSQL.AppendLine(" AND Movimenti_dettagli.Elem_Cod = 1 ")
            StrSQL.AppendLine(" AND Movimenti.Id_Agenda = " + Agro_SQL_SaveNum(IdAgenda) + " ")
            StrSQL.AppendLine(" AND Movimenti.Piva = '" + Agro_SQL_SaveText(Piva) + "' ")

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                '16/09/2019: aggiunto order by per avere la Data_Rilascio_Patentino in ordine decrescente
                StrSQL.AppendLine("  ORDER BY Elem_Cod, Rag_Soc, Cognome,  Nome, Data_Rilascio_Patentino desc, mac_des ")
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

    '################################################################################
    '16/09/2019: nuova gestione patentini
    'data inizio e fine servono per filtrare le validità del patentino
    Public Function CostiAccessori_from_IdAgenda2(ByVal Piva As String,
                                                 ByVal strIdAgenda As String,
                                                 ByVal Data_Inizio As Date,
                                                 ByVal Data_Fine As Date,
                                                    ByVal xFiltroAggiuntivo As String,
                                                    ByVal xOrderBy As String,
                                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                    ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.CostiAccessori.CostiAccessori_from_IdAgenda2()"
        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            StrSQL.Length = 0

            StrSQL.AppendLine(" SET NOCOUNT ON ")
            StrSQL.AppendLine(" DECLARE @Tab_mm TABLE(Cau_Mov varchar(10), Id_Agenda int, Id_Mov_Det int, Mat_Cod int, Elem_Cod int, Data_Movimento date) ")
            StrSQL.AppendLine("  ")
            StrSQL.AppendLine(" INSERT INTO @Tab_mm ")
            StrSQL.AppendLine("     SELECT Movimenti.Cau_Mov, Movimenti_dettagli.Id_Agenda, Movimenti_dettagli.Id_Mov_Det, Movimenti_dettagli.Mat_Cod, Movimenti_dettagli.Elem_Cod, Movimenti.Data_Movimento ")
            StrSQL.AppendLine("     FROM Movimenti ")
            StrSQL.AppendLine("     INNER JOIN Movimenti_dettagli ON Movimenti.Piva = Movimenti_dettagli.Piva AND Movimenti.Id_Agenda = Movimenti_dettagli.Id_Agenda AND Movimenti.Id_Mov = Movimenti_dettagli.Id_Mov ")
            StrSQL.AppendLine("     WHERE Movimenti.Piva = '" + Agro_SQL_SaveText(Piva) + "' ")
            StrSQL.AppendLine("     AND Movimenti.Id_Agenda IN (" + Agro_SQL_Save_Clausola_IN(strIdAgenda, False) + ") ")
            StrSQL.AppendLine("     AND (Movimenti.Cau_Mov = '" & CAU_IMPUTAZIONE_MANODOPERA & "' OR Movimenti.Cau_Mov = '" & CAU_IMPUTAZIONE_TERZISTI & "' OR Movimenti.Cau_Mov = '" & CAU_IMPUTAZIONE_TECNICO_RESPONSABILE & "' OR Movimenti.Cau_Mov = '" & CAU_IMPUTAZIONE_PARCOMACCHINE & "') ")
            StrSQL.AppendLine("  ")
            StrSQL.AppendLine(" SELECT * FROM ( ")
            StrSQL.AppendLine("     SELECT mm.Elem_Cod, mm.Cau_Mov, mm.Id_Agenda, mm.Id_Mov_Det, ")
            StrSQL.AppendLine("     Contatti.Sa_Cod, Risorse_Umane.Cod_RisUm, Contatti.Cod_Contatto, Contatti.Rag_Soc, Contatti.Nome, Contatti.Cognome, ")
            StrSQL.AppendLine("     ISNULL( PATENTINO.Allegati_Documenti_Numero, '')  AS Patentino, ")
            StrSQL.AppendLine("     ISNULL(PATENTINO.Allegati_Documenti_Cod, '') AS CodiceUnivocoDocumentoPatentino, ")
            StrSQL.AppendLine("     ISNULL(PATENTINO.Validazione_Data, '01/01/1900') AS Data_Rilascio_Patentino,  ")
            StrSQL.AppendLine("     ISNULL(PATENTINO.Data_Scadenza, '31/12/2100') AS Data_Scadenza_Patentino,   ")
            StrSQL.AppendLine("     '' AS Mac_Des, 0 AS Mac_Cod, '' AS Class_Code, '' AS CLASS_DESC, '' AS Modello, '' AS Ultima_Manutenzione  , 0 AS Ditta_cod, '' AS Ditta_Des ")
            StrSQL.AppendLine("     FROM @Tab_mm mm ")
            StrSQL.AppendLine("     INNER JOIN Risorse_Umane ON mm.Mat_Cod = Risorse_Umane.Cod_RisUm ")
            StrSQL.AppendLine("     INNER JOIN Contatti ON Contatti.Cod_Contatto = Risorse_Umane.Cod_Contatto AND Contatti.Piva = Risorse_Umane.Piva ")
            StrSQL.AppendLine("  ")
            StrSQL.AppendLine("     LEFT OUTER JOIN (")
            StrSQL.AppendLine("             SELECT alert_entita.Cod_Contatto, alert_entita.piva, Allegati_Documenti.validazione_data, allegati_documenti.Allegati_Documenti_Numero, Alert_Elenco.Data_Scadenza, Allegati_Documenti.Allegati_Documenti_Cod   ")
            StrSQL.AppendLine("             FROM alert_entita ")
            StrSQL.AppendLine("             INNER JOIN Alert_Elenco ON Alert_Elenco.PivaSuperUser = Alert_Entita.PivaSuperUser AND Alert_Elenco.ID_Alert_Entita = Alert_Entita.ID_Alert_Entita   ")
            StrSQL.AppendLine("             INNER JOIN Allegati_Documenti ON Allegati_Documenti.Allegati_Documenti_SuperUser = Alert_Entita.PivaSuperUser and Allegati_Documenti.Allegati_Documenti_Piva = Alert_Entita.piva  and Allegati_Documenti.Allegati_Documenti_Cod = Alert_Entita.Allegati_Documenti_Cod  ")
            StrSQL.AppendLine("             WHERE tipoentita_cod=8 ")
            StrSQL.AppendLine("             AND Allegati_Documenti.Allegati_documenti_CatCod=2 ")
            StrSQL.AppendLine("             AND Allegati_Documenti.Validazione_Data <= " & Agro_SQL_SaveDate(Data_Fine) & "")
            StrSQL.AppendLine("             AND Alert_Elenco.Data_Scadenza >= " & Agro_SQL_SaveDate(Data_Inizio) & "  ")
            StrSQL.AppendLine("     ) PATENTINO on REPLACE(LTRIM(RTRIM(PATENTINO.Cod_Contatto)), char(9), '') = REPLACE(LTRIM(RTRIM(Contatti.Cod_Contatto)), char(9), '') and PATENTINO.piva = Contatti.piva ")
            StrSQL.AppendLine("     AND PATENTINO.Validazione_Data <=  mm.Data_Movimento AND PATENTINO.Data_Scadenza >= mm.Data_Movimento ")
            StrSQL.AppendLine("  ")
            'serve rifare il filtro cau_mov per escludere qui quelli del parco macchine
            StrSQL.AppendLine("      WHERE (mm.Cau_Mov = '" & CAU_IMPUTAZIONE_MANODOPERA & "' OR mm.Cau_Mov = '" & CAU_IMPUTAZIONE_TERZISTI & "' OR mm.Cau_Mov = '" & CAU_IMPUTAZIONE_TECNICO_RESPONSABILE & "') ")

            '16/09/2019: modificata query x nuova gestione patentini
            'StrSQL.AppendLine(" SELECT mm.Cau_Mov, mm.Id_Agenda, mm.Id_Mov_Det, ")
            'StrSQL.AppendLine(" Risorse_Umane.Cod_RisUm, Contatti.Cod_Contatto, Contatti.Rag_Soc, Contatti.Nome, Contatti.Cognome, ")
            'StrSQL.AppendLine(" Risorse_Umane.Patentino, Risorse_Umane.Data_Rilascio_Patentino, Risorse_Umane.Data_Scadenza_Patentino, '' AS Mac_Des, 0 AS Mac_Cod, '' AS Class_Code, '' AS CLASS_DESC, '' AS Modello, ")
            'StrSQL.AppendLine(" '' AS Ultima_Manutenzione  , 0 AS Ditta_cod, '' AS Ditta_Des ")
            'StrSQL.AppendLine(" FROM @Tab_mm mm ")
            'StrSQL.AppendLine(" INNER JOIN Risorse_Umane ON mm.Mat_Cod = Risorse_Umane.Cod_RisUm ")
            'StrSQL.AppendLine(" INNER JOIN Contatti ON Contatti.Cod_Contatto = Risorse_Umane.Cod_Contatto AND Contatti.Piva = Risorse_Umane.Piva ")
            'StrSQL.AppendLine(" WHERE (mm.Cau_Mov = '" & CAU_IMPUTAZIONE_MANODOPERA & "' OR mm.Cau_Mov = '" & CAU_IMPUTAZIONE_TERZISTI & "' OR mm.Cau_Mov = '" & CAU_IMPUTAZIONE_TECNICO_RESPONSABILE & "') ")

            StrSQL.AppendLine("  ")
            StrSQL.AppendLine("     UNION ALL ")
            StrSQL.AppendLine("  ")
            StrSQL.AppendLine("     SELECT mm.Elem_Cod, mm.Cau_Mov, mm.Id_Agenda, mm.Id_Mov_Det, ")
            StrSQL.AppendLine("     Parco_Macchine.Sa_Cod, 0 as Cod_RisUm, '' as Cod_Contatto, '' as Rag_Soc, '' as Nome, '' as Cognome, '' as Patentino, '' as CodiceUnivocoDocumentoPatentino, '' as Data_Rilascio_Patentino, '' as Data_Scadenza_Patentino,  ")
            StrSQL.AppendLine("     Parco_Macchine.Mac_Des, Parco_Macchine.Mac_Cod,  Parco_Macchine.Class_Code, ")
            StrSQL.AppendLine("     Macchine.CLASS_DESC, Parco_Macchine.Modello, Parco_Macchine.Ultima_Manutenzione  , isnull(Parco_Macchine.Ditta_cod,0) AS Ditta_cod, ISNULL(Ditte.Ditta_des, '') as Ditta_des ")
            StrSQL.AppendLine("     FROM @Tab_mm mm ")
            StrSQL.AppendLine("     INNER JOIN Parco_Macchine ON Parco_Macchine.Mac_Cod = mm.Mat_Cod ")
            StrSQL.AppendLine("     INNER JOIN Macchine ON Parco_Macchine.Class_Code = Macchine.CLASS_CODE ")
            StrSQL.AppendLine("     LEFT JOIN Ditte ON Parco_Macchine.Ditta_Cod= Ditte.Ditta_Cod ")

            StrSQL.AppendLine("  ")

            'serve rifare il filtro cau_mov per escludere qui quelli della manodopera
            StrSQL.AppendLine("     WHERE mm.Cau_Mov = '" & CAU_IMPUTAZIONE_PARCOMACCHINE & "' ")
            StrSQL.AppendLine("     AND mm.Elem_Cod = 1 ")
            StrSQL.AppendLine(" ) T ")
            StrSQL.AppendLine("  ")
            StrSQL.AppendLine(" WHERE 1=1 ")

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                '16/09/2019: aggiunto order by per avere la Data_Rilascio_Patentino in ordine decrescente
                StrSQL.AppendLine("  ORDER BY Elem_Cod, Rag_Soc, Cognome,  Nome, Data_Rilascio_Patentino desc, mac_des ")
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

    '################################################################################
    'a differenza della CostiAccessori_from_IdAgenda2 non legge i dati del patentino
    '(usata dal menù agenda)
    Public Function CostiAccessori_from_IdAgenda3(ByVal Piva As String,
                                                 ByVal strIdAgenda As String,
                                                    ByVal xFiltroAggiuntivo As String,
                                                    ByVal xOrderBy As String,
                                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                    ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.CostiAccessori.CostiAccessori_from_IdAgenda3()"
        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            StrSQL.Length = 0

            StrSQL.AppendLine(" SET NOCOUNT ON ")
            StrSQL.AppendLine(" DECLARE @Tab_mm TABLE(Cau_Mov varchar(10), Id_Agenda int, Id_Mov_Det int, Mat_Cod int, Elem_Cod int) ")
            StrSQL.AppendLine("  ")
            StrSQL.AppendLine(" INSERT INTO @Tab_mm ")
            StrSQL.AppendLine(" SELECT Movimenti.Cau_Mov, Movimenti_dettagli.Id_Agenda, Movimenti_dettagli.Id_Mov_Det, Movimenti_dettagli.Mat_Cod, Movimenti_dettagli.Elem_Cod ")
            StrSQL.AppendLine(" FROM Movimenti ")
            StrSQL.AppendLine(" INNER JOIN Movimenti_dettagli ON Movimenti.Piva = Movimenti_dettagli.Piva AND Movimenti.Id_Agenda = Movimenti_dettagli.Id_Agenda AND Movimenti.Id_Mov = Movimenti_dettagli.Id_Mov ")
            StrSQL.AppendLine(" WHERE Movimenti.Piva = '" + Agro_SQL_SaveText(Piva) + "' ")
            'StrSQL.AppendLine(" AND Movimenti.Id_Agenda IN (" + Agro_SQL_Save_Clausola_IN(strIdAgenda, False) + ") ")
            StrSQL.AppendLine(" AND Movimenti.Id_Agenda IN (" + Agro_SQL_Save_Clausola_IN(strIdAgenda) + ") ")
            StrSQL.AppendLine(" AND (Movimenti.Cau_Mov = '" & CAU_IMPUTAZIONE_MANODOPERA & "' OR Movimenti.Cau_Mov = '" & CAU_IMPUTAZIONE_TERZISTI & "' OR Movimenti.Cau_Mov = '" & CAU_IMPUTAZIONE_TECNICO_RESPONSABILE & "' OR Movimenti.Cau_Mov = '" & CAU_IMPUTAZIONE_PARCOMACCHINE & "') ")
            StrSQL.AppendLine("  ")
            StrSQL.AppendLine("  ")
            StrSQL.AppendLine(" SELECT mm.Elem_Cod, mm.Cau_Mov, mm.Id_Agenda, mm.Id_Mov_Det, ")
            StrSQL.AppendLine(" Risorse_Umane.Cod_RisUm, Contatti.Cod_Contatto, Contatti.Rag_Soc, Contatti.Nome, Contatti.Cognome, ")
            StrSQL.AppendLine(" '' AS Mac_Des, 0 AS Mac_Cod, '' AS Class_Code, '' AS CLASS_DESC, '' AS Modello, '' AS Ultima_Manutenzione  , 0 AS Ditta_cod, '' AS Ditta_Des ")
            StrSQL.AppendLine(" FROM @Tab_mm mm ")
            StrSQL.AppendLine(" INNER JOIN Risorse_Umane ON mm.Mat_Cod = Risorse_Umane.Cod_RisUm ")
            StrSQL.AppendLine(" INNER JOIN Contatti ON Contatti.Cod_Contatto = Risorse_Umane.Cod_Contatto AND Contatti.Piva = Risorse_Umane.Piva ")
            'serve rifare il filtro cau_mov per escludere qui quelli del parco macchine
            StrSQL.AppendLine(" WHERE (mm.Cau_Mov = '" & CAU_IMPUTAZIONE_MANODOPERA & "' OR mm.Cau_Mov = '" & CAU_IMPUTAZIONE_TERZISTI & "' OR mm.Cau_Mov = '" & CAU_IMPUTAZIONE_TECNICO_RESPONSABILE & "') ")

            StrSQL.AppendLine(" UNION ALL ")
            StrSQL.AppendLine("  ")
            StrSQL.AppendLine(" SELECT mm.Elem_Cod, mm.Cau_Mov, mm.Id_Agenda, mm.Id_Mov_Det, ")
            StrSQL.AppendLine("  0 as Cod_RisUm, '' as Cod_Contatto, '' as Rag_Soc, '' as Nome, '' as Cognome,  ")
            StrSQL.AppendLine(" Parco_Macchine.Mac_Des, Parco_Macchine.Mac_Cod,  Parco_Macchine.Class_Code, ")
            StrSQL.AppendLine(" Macchine.CLASS_DESC, Parco_Macchine.Modello, Parco_Macchine.Ultima_Manutenzione  , isnull(Parco_Macchine.Ditta_cod,0) AS Ditta_cod, ISNULL(Ditte.Ditta_des, '') as Ditta_des ")
            StrSQL.AppendLine(" FROM @Tab_mm mm ")
            StrSQL.AppendLine(" INNER JOIN Parco_Macchine ON Parco_Macchine.Mac_Cod = mm.Mat_Cod ")
            StrSQL.AppendLine(" INNER JOIN Macchine ON Parco_Macchine.Class_Code = Macchine.CLASS_CODE ")
            StrSQL.AppendLine(" LEFT JOIN Ditte ON Parco_Macchine.Ditta_Cod= Ditte.Ditta_Cod ")
            'serve rifare il filtro cau_mov per escludere qui quelli della manodopera
            StrSQL.AppendLine(" WHERE mm.Cau_Mov = '" & CAU_IMPUTAZIONE_PARCOMACCHINE & "' ")
            StrSQL.AppendLine(" AND mm.Elem_Cod = 1 ")

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine("  ORDER BY Elem_Cod, Rag_Soc, Cognome,  Nome, mac_des ")
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


    Public Function CostiAccessori_from_IdAgenda3_Ottimizzata(ByVal Piva As String,
                         ByVal agende As List(Of Integer),
                         ByVal xFiltroAggiuntivo As String,
                         ByVal xOrderBy As String,
                         ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreContabDAL.CostiAccessori.CostiAccessori_from_IdAgenda3_Ottimizzata()"
        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        ConnessioniTransazioni.ApriConnessione(True, objParametri)

        Try

            EseguiQuery_Scrittura(objParametri, CreaTabellaTemp_FiltroAgende(), NomeRoutine)


            If DataProviderFactory.Instance.TipoProvider = TipiEnumerativi.enum_DataProvidersType.SqlDataProvider Then

                'riempo tabella temporanea con BulkCopy

                Dim bulkCopyList As New List(Of Object)
                agende.ForEach(Function(a)
                                   bulkCopyList.Add(New With {.Id_Agenda = a})
                               End Function)
                Dim jsonData = JsonConvert.SerializeObject(New With {Key .Table = bulkCopyList})
                Dim ds As DataSet = JsonConvert.DeserializeObject(Of DataSet)(jsonData)

                Using sqlBulkCopy As New SqlBulkCopy(objParametri.objConnessione, Nothing, objParametri.objTransazione)
                    sqlBulkCopy.DestinationTableName = "#TempAgende"
                    sqlBulkCopy.ColumnMappings.Add("Id_Agenda", "Id_Agenda")
                    sqlBulkCopy.WriteToServer(ds.Tables(0))
                End Using

            Else

                If Not IsNothing(agende) AndAlso agende.Any Then

                    Dim chunks = ChunkBy(Of Integer)(agende, 1000)
                    For Each chunk In chunks
                        StrSQL.Append("insert into #TempAgende (Id_Agenda) values ")
                        For Each id As Integer In chunk
                            StrSQL.AppendLine(String.Format("({0}),", id))
                        Next
                        Dim strSqlInsert As String = StrSQL.ToString
                        strSqlInsert = strSqlInsert.Remove(strSqlInsert.LastIndexOf(","))
                        StrSQL.Clear()
                        EseguiQuery_Scrittura(objParametri, strSqlInsert, NomeRoutine)
                    Next
                End If

            End If


            StrSQL.Length = 0

            StrSQL.AppendLine(" SET NOCOUNT ON ")
            StrSQL.AppendLine(" DECLARE @Tab_mm TABLE(Cau_Mov varchar(10), Id_Agenda int, Id_Mov_Det int, Mat_Cod int, Elem_Cod int) ")
            StrSQL.AppendLine("  ")
            StrSQL.AppendLine(" INSERT INTO @Tab_mm ")
            StrSQL.AppendLine(" SELECT Movimenti.Cau_Mov, Movimenti_dettagli.Id_Agenda, Movimenti_dettagli.Id_Mov_Det, Movimenti_dettagli.Mat_Cod, Movimenti_dettagli.Elem_Cod ")
            StrSQL.AppendLine(" FROM Movimenti WITH (nolock) ")
            StrSQL.AppendLine(" INNER JOIN #TempAgende WITH (nolock) on Movimenti.Id_Agenda = #TempAgende.Id_agenda ")
            StrSQL.AppendLine(" INNER JOIN Movimenti_dettagli WITH (nolock) ON Movimenti.Piva = Movimenti_dettagli.Piva AND Movimenti.Id_Agenda = Movimenti_dettagli.Id_Agenda AND Movimenti.Id_Mov = Movimenti_dettagli.Id_Mov ")
            StrSQL.AppendLine(" WHERE Movimenti.Piva = '" + Agro_SQL_SaveText(Piva) + "' ")
            StrSQL.AppendLine(" AND (Movimenti.Cau_Mov = '" & CAU_IMPUTAZIONE_MANODOPERA & "' OR Movimenti.Cau_Mov = '" & CAU_IMPUTAZIONE_TERZISTI & "' OR Movimenti.Cau_Mov = '" & CAU_IMPUTAZIONE_TECNICO_RESPONSABILE & "' OR Movimenti.Cau_Mov = '" & CAU_IMPUTAZIONE_PARCOMACCHINE & "') ")
            StrSQL.AppendLine("  ")
            StrSQL.AppendLine("  ")
            StrSQL.AppendLine(" SELECT mm.Elem_Cod, mm.Cau_Mov, mm.Id_Agenda, mm.Id_Mov_Det, ")
            StrSQL.AppendLine(" Risorse_Umane.Cod_RisUm, Contatti.Cod_Contatto, Contatti.Rag_Soc, Contatti.Nome, Contatti.Cognome, ")
            StrSQL.AppendLine(" '' AS Mac_Des, 0 AS Mac_Cod, '' AS Class_Code, '' AS CLASS_DESC, '' AS Modello, '' AS Ultima_Manutenzione  , 0 AS Ditta_cod, '' AS Ditta_Des ")
            StrSQL.AppendLine(" FROM @Tab_mm mm ")
            StrSQL.AppendLine(" INNER JOIN Risorse_Umane WITH (nolock) ON mm.Mat_Cod = Risorse_Umane.Cod_RisUm ")
            StrSQL.AppendLine(" INNER JOIN Contatti WITH (nolock) ON Contatti.Cod_Contatto = Risorse_Umane.Cod_Contatto AND Contatti.Piva = Risorse_Umane.Piva ")
            'serve rifare il filtro cau_mov per escludere qui quelli del parco macchine
            StrSQL.AppendLine(" WHERE (mm.Cau_Mov = '" & CAU_IMPUTAZIONE_MANODOPERA & "' OR mm.Cau_Mov = '" & CAU_IMPUTAZIONE_TERZISTI & "' OR mm.Cau_Mov = '" & CAU_IMPUTAZIONE_TECNICO_RESPONSABILE & "') ")

            StrSQL.AppendLine(" UNION ALL ")
            StrSQL.AppendLine("  ")
            StrSQL.AppendLine(" SELECT mm.Elem_Cod, mm.Cau_Mov, mm.Id_Agenda, mm.Id_Mov_Det, ")
            StrSQL.AppendLine("  0 as Cod_RisUm, '' as Cod_Contatto, '' as Rag_Soc, '' as Nome, '' as Cognome,  ")
            StrSQL.AppendLine(" Parco_Macchine.Mac_Des, Parco_Macchine.Mac_Cod,  Parco_Macchine.Class_Code, ")
            StrSQL.AppendLine(" Macchine.CLASS_DESC, Parco_Macchine.Modello, Parco_Macchine.Ultima_Manutenzione  , isnull(Parco_Macchine.Ditta_cod,0) AS Ditta_cod, ISNULL(Ditte.Ditta_des, '') as Ditta_des ")
            StrSQL.AppendLine(" FROM @Tab_mm mm ")
            StrSQL.AppendLine(" INNER JOIN Parco_Macchine WITH (nolock) ON Parco_Macchine.Mac_Cod = mm.Mat_Cod ")
            StrSQL.AppendLine(" INNER JOIN Macchine WITH (nolock) ON Parco_Macchine.Class_Code = Macchine.CLASS_CODE ")
            StrSQL.AppendLine(" LEFT JOIN Ditte WITH (nolock) ON Parco_Macchine.Ditta_Cod= Ditte.Ditta_Cod ")
            'serve rifare il filtro cau_mov per escludere qui quelli della manodopera
            StrSQL.AppendLine(" WHERE mm.Cau_Mov = '" & CAU_IMPUTAZIONE_PARCOMACCHINE & "' ")
            StrSQL.AppendLine(" AND mm.Elem_Cod = 1 ")

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine("  ORDER BY Elem_Cod, Rag_Soc, Cognome,  Nome, mac_des ")
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------


            ' Eliminazione tabella temporanea
            EseguiQuery_Scrittura(objParametri, EliminaTabellaTemp_FiltroAgende, NomeRoutine)

            'commit transazione
            ConnessioniTransazioni.ChiudiTransazione(1, objParametri)

        Catch ex As Exception
            ConnessioniTransazioni.ChiudiTransazione(2, objParametri)
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        Finally
            ConnessioniTransazioni.ChiudiConnessione(objParametri)
        End Try

        Return DT

    End Function

    Private Function CreaTabellaTemp_FiltroAgende() As String
        Dim stb As New StringBuilder

        stb.AppendLine(" IF OBJECT_ID('tempdb.dbo.#TempAgende') IS NULL BEGIN ")
        stb.AppendLine("    CREATE TABLE #TempAgende ( ")
        stb.AppendLine("        Id_Agenda int NULL")
        stb.AppendLine("    )")
        stb.AppendLine()
        stb.AppendLine(" END ")

        Return stb.ToString()
    End Function

    Private Function EliminaTabellaTemp_FiltroAgende() As String
        Dim stb As New StringBuilder

        stb.AppendLine(" IF NOT OBJECT_ID('tempdb.dbo.#TempAgende') IS NULL BEGIN ")
        stb.AppendLine("    DROP TABLE #TempAgende ")
        stb.AppendLine(" END ")

        Return stb.ToString()
    End Function

    Private Iterator Function ChunkBy(Of TSource)(ByVal source As IEnumerable(Of TSource), ByVal chunkSize As Integer) As IEnumerable(Of IEnumerable(Of TSource))
        While source.Any()
            Yield source.Take(chunkSize)
            source = source.Skip(chunkSize)
        End While
    End Function

    '################################################################################
    'a differenza della CostiAccessori_from_IdAgenda non legge i dati del patentino
    '(usata dal menù agenda)
    Public Function CostiAccessori_from_IdAgenda4(ByVal Piva As String, _
                                                 ByVal IdAgenda As Integer, _
                                                    ByVal xFiltroAggiuntivo As String, _
                                                    ByVal xOrderBy As String, _
                                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                                    ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.CostiAccessori.CostiAccessori_from_IdAgenda4()"
        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT Elem_Cod, Movimenti.Cau_Mov, Movimenti_dettagli.Id_Agenda, Movimenti_dettagli.Id_Mov_Det, ")
            StrSQL.AppendLine(" Risorse_Umane.Cod_RisUm, Contatti.Cod_Contatto, Contatti.Rag_Soc, Contatti.Nome, Contatti.Cognome, ")
            StrSQL.AppendLine(" '' AS Mac_Des, 0 AS Mac_Cod, '' AS Class_Code, '' AS CLASS_DESC, '' AS Modello, '' AS Ultima_Manutenzione, 0 AS Ditta_cod, '' AS Ditta_Des ")
            StrSQL.AppendLine(" FROM Movimenti  ")
            StrSQL.AppendLine(" INNER JOIN Movimenti_dettagli ON Movimenti.Id_Agenda = Movimenti_dettagli.Id_Agenda AND Movimenti.Id_Mov = Movimenti_dettagli.Id_Mov AND Movimenti.Piva = Movimenti_dettagli.Piva ")
            StrSQL.AppendLine(" INNER JOIN Risorse_Umane ON Movimenti_dettagli.Mat_Cod = Risorse_Umane.Cod_RisUm ")
            StrSQL.AppendLine(" INNER JOIN Contatti ON Contatti.Cod_Contatto = Risorse_Umane.Cod_Contatto AND Contatti.Piva = Risorse_Umane.Piva")
            StrSQL.AppendLine("  ")
            'serve rifare il filtro cau_mov per escludere qui quelli del parco macchine
            StrSQL.AppendLine(" WHERE Movimenti.Cau_Mov IN ('" & CAU_IMPUTAZIONE_MANODOPERA & "','" & CAU_IMPUTAZIONE_TERZISTI & "','" & CAU_IMPUTAZIONE_TECNICO_RESPONSABILE & "') ")
            StrSQL.AppendLine(" AND Movimenti.Id_Agenda = " + Agro_SQL_SaveNum(IdAgenda) + " ")
            StrSQL.AppendLine(" AND Movimenti.Piva = '" + Agro_SQL_SaveText(Piva) + "' ")
            StrSQL.AppendLine("  ")
            StrSQL.AppendLine(" UNION ALL ")
            StrSQL.AppendLine("  ")
            StrSQL.AppendLine(" SELECT Elem_Cod, Movimenti.Cau_Mov, Movimenti_dettagli.Id_Agenda, Movimenti_dettagli.Id_Mov_Det, ")
            StrSQL.AppendLine("  0 as Cod_RisUm, '' as Cod_Contatto, '' as Rag_Soc, '' as Nome, '' as Cognome,  ")
            StrSQL.AppendLine(" Parco_Macchine.Mac_Des, Parco_Macchine.Mac_Cod, ")
            StrSQL.AppendLine(" Parco_Macchine.Class_Code, Macchine.CLASS_DESC, Parco_Macchine.Modello, Parco_Macchine.Ultima_Manutenzione ")
            StrSQL.AppendLine(" , isnull(Parco_Macchine.Ditta_cod,0) AS Ditta_cod, ISNULL(Ditte.Ditta_des, '') as Ditta_des ")
            StrSQL.AppendLine(" FROM Parco_Macchine INNER JOIN ")
            StrSQL.AppendLine(" Movimenti_dettagli ON Parco_Macchine.Mac_Cod = Movimenti_dettagli.Mat_Cod INNER JOIN ")
            StrSQL.AppendLine(" Movimenti ON Movimenti_dettagli.Id_Agenda = Movimenti.Id_Agenda AND ")
            StrSQL.AppendLine(" Movimenti_dettagli.Id_Mov = Movimenti.Id_Mov INNER JOIN ")
            StrSQL.AppendLine(" Macchine ON Parco_Macchine.Class_Code = Macchine.CLASS_CODE ")
            StrSQL.AppendLine(" LEFT JOIN Ditte ON Parco_Macchine.Ditta_Cod= Ditte.Ditta_Cod ")
            'serve rifare il filtro cau_mov per escludere qui quelli del parco macchine
            StrSQL.AppendLine(" WHERE Movimenti.Cau_Mov = '" & CAU_IMPUTAZIONE_PARCOMACCHINE & "' ")
            StrSQL.AppendLine(" AND Movimenti_dettagli.Elem_Cod = 1 ")
            StrSQL.AppendLine(" AND Movimenti.Id_Agenda = " + Agro_SQL_SaveNum(IdAgenda) + " ")
            StrSQL.AppendLine(" AND Movimenti.Piva = '" + Agro_SQL_SaveText(Piva) + "' ")

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine("  ORDER BY Elem_Cod, Rag_Soc, Cognome,  Nome, mac_des ")
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

    '################################################################################
    'a differenza della CostiAccessori_from_IdAgenda non legge i dati del patentino (legge le ore imputate e unità di misura)
    '(usata dal menù agenda)
    Public Function CostiAccessori_from_IdAgenda5(
            ByVal Piva As String,
            ByVal strIdAgenda As String,
            ByVal xFiltroAggiuntivo As String,
            ByVal xOrderBy As String,
            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.CostiAccessori.CostiAccessori_from_IdAgenda4()"
        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT Elem_Cod, Movimenti.Cau_Mov, Movimenti_dettagli.Id_Agenda, Movimenti_dettagli.Id_Mov_Det, ")
            StrSQL.AppendLine(" Risorse_Umane.Cod_RisUm, Contatti.Cod_Contatto, Contatti.Rag_Soc, Contatti.Nome, Contatti.Cognome, ")
            StrSQL.AppendLine(" '' AS Mac_Des, 0 AS Mac_Cod, '' AS Class_Code, '' AS CLASS_DESC, '' AS Modello, '' AS Ultima_Manutenzione, 0 AS Ditta_cod, '' AS Ditta_Des, qta, udm_cod, ''  as macchina_codice ")
            StrSQL.AppendLine(" FROM Movimenti  ")
            StrSQL.AppendLine(" INNER JOIN Movimenti_dettagli ON Movimenti.Id_Agenda = Movimenti_dettagli.Id_Agenda AND Movimenti.Id_Mov = Movimenti_dettagli.Id_Mov AND Movimenti.Piva = Movimenti_dettagli.Piva ")
            StrSQL.AppendLine(" INNER JOIN Risorse_Umane ON Movimenti_dettagli.Mat_Cod = Risorse_Umane.Cod_RisUm ")
            StrSQL.AppendLine(" INNER JOIN Contatti ON Contatti.Cod_Contatto = Risorse_Umane.Cod_Contatto AND Contatti.Piva = Risorse_Umane.Piva")


            StrSQL.AppendLine("  ")
            'serve rifare il filtro cau_mov per escludere qui quelli del parco macchine
            StrSQL.AppendLine(" WHERE Movimenti.Cau_Mov IN ('" & CAU_IMPUTAZIONE_MANODOPERA & "','" & CAU_IMPUTAZIONE_TERZISTI & "','" & CAU_IMPUTAZIONE_TECNICO_RESPONSABILE & "') ")
            StrSQL.AppendLine(" AND Movimenti.Id_Agenda in ( " & Agro_SQL_Save_Clausola_IN(strIdAgenda) & ") ")
            StrSQL.AppendLine(" AND Movimenti.Piva = '" + Agro_SQL_SaveText(Piva) + "' ")
            StrSQL.AppendLine("  ")
            StrSQL.AppendLine(" UNION ALL ")
            StrSQL.AppendLine("  ")
            StrSQL.AppendLine(" SELECT Elem_Cod, Movimenti.Cau_Mov, Movimenti_dettagli.Id_Agenda, Movimenti_dettagli.Id_Mov_Det, ")
            StrSQL.AppendLine("  0 as Cod_RisUm, '' as Cod_Contatto, '' as Rag_Soc, '' as Nome, '' as Cognome,  ")
            StrSQL.AppendLine(" Parco_Macchine.Mac_Des, Parco_Macchine.Mac_Cod, ")
            StrSQL.AppendLine(" Parco_Macchine.Class_Code, Macchine.CLASS_DESC, Parco_Macchine.Modello, Parco_Macchine.Ultima_Manutenzione ")
            StrSQL.AppendLine(" , isnull(Parco_Macchine.Ditta_cod,0) AS Ditta_cod, ISNULL(Ditte.Ditta_des, '') as Ditta_des, qta, udm_cod , Parco_Macchine.Codice as macchina_codice ")
            StrSQL.AppendLine(" FROM Parco_Macchine INNER JOIN ")
            StrSQL.AppendLine(" Movimenti_dettagli ON Parco_Macchine.Mac_Cod = Movimenti_dettagli.Mat_Cod INNER JOIN ")
            StrSQL.AppendLine(" Movimenti ON Movimenti_dettagli.Id_Agenda = Movimenti.Id_Agenda AND ")
            StrSQL.AppendLine(" Movimenti_dettagli.Id_Mov = Movimenti.Id_Mov INNER JOIN ")
            StrSQL.AppendLine(" Macchine ON Parco_Macchine.Class_Code = Macchine.CLASS_CODE ")
            StrSQL.AppendLine(" LEFT JOIN Ditte ON Parco_Macchine.Ditta_Cod= Ditte.Ditta_Cod ")


            'serve rifare il filtro cau_mov per escludere qui quelli del parco macchine
            StrSQL.AppendLine(" WHERE Movimenti.Cau_Mov = '" & CAU_IMPUTAZIONE_PARCOMACCHINE & "' ")
            StrSQL.AppendLine(" AND Movimenti_dettagli.Elem_Cod = 1 ")
            StrSQL.AppendLine(" AND Movimenti.Id_Agenda in (" & Agro_SQL_Save_Clausola_IN(strIdAgenda) & ") ")
            StrSQL.AppendLine(" AND Movimenti.Piva = '" + Agro_SQL_SaveText(Piva) + "' ")

            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            If xOrderBy <> "" Then
                strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine("  ORDER BY Elem_Cod, Rag_Soc, Cognome,  Nome, mac_des ")
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

    '################################################################################
    Public Function CostiAccessori_from_Ricetta_Operazione_Cod(
                                                 ByVal Str_Ricetta_Operazione_Cod As String,
                                                 ByVal xFiltroAggiuntivo As String,
                                                 ByVal xOrderBy As String,
                                                 ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                 ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.CostiAccessori_R.CostiAccessori_from_Ricetta_Operazione_Cod()"

        '====================================================================================
        'Parametri opzionali :
        '   Sa_Cod = 0           =>  si leggono tutti gli impianti dell'impresa
        '   Appezza = 0          =>  si leggono tutti gli impianti del centro aziendale
        '   Id_reg = 0           =>  si leggono tutti gli impianti dell'appezzamento
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT Ricette_dettagli.Cau_Mov, Ricette_Operazioni.Ricetta_Operazione_Cod, Ricette_dettagli.Ricetta_Dettaglio_Cod, ")
            StrSQL.AppendLine(" Risorse_Umane.Cod_RisUm, Contatti.Cod_Contatto, Contatti.Rag_Soc, Contatti.Nome, Contatti.Cognome, ")
            StrSQL.AppendLine(" Risorse_Umane.Patentino, Risorse_Umane.Data_Rilascio_Patentino, Risorse_Umane.Data_Scadenza_Patentino, ")
            StrSQL.AppendLine(" '' AS Mac_Des, 0 AS Mac_Cod, '' AS Class_Code, '' AS CLASS_DESC, '' AS Modello, '' AS Ultima_Manutenzione ")
            StrSQL.AppendLine(" , 0 AS Ditta_cod, '' AS Ditta_Des ")
            StrSQL.AppendLine(" FROM Ricette_Operazioni ")
            StrSQL.AppendLine(" INNER JOIN Ricette_dettagli ")
            StrSQL.AppendLine(" ON Ricette_Operazioni.Ricetta_SuperUser = Ricette_dettagli.Ricetta_SuperUser AND Ricette_Operazioni.Ricetta_Operazione_Cod = Ricette_dettagli.Ricetta_Operazione_Cod ")
            StrSQL.AppendLine(" INNER JOIN Risorse_Umane ")
            StrSQL.AppendLine(" ON Ricette_dettagli.Mat_Cod = Risorse_Umane.Cod_RisUm ")
            StrSQL.AppendLine(" INNER JOIN Contatti  ")
            StrSQL.AppendLine(" ON Contatti.Cod_Contatto = Risorse_Umane.Cod_Contatto AND Contatti.Piva = Risorse_Umane.Piva")
            StrSQL.AppendLine(" WHERE Ricette_dettagli.Cau_Mov IN ('" & CAU_IMPUTAZIONE_MANODOPERA & "','" & CAU_IMPUTAZIONE_TERZISTI & "','" & CAU_IMPUTAZIONE_TECNICO_RESPONSABILE & "') ")
            StrSQL.AppendLine(" AND  Ricette_Operazioni.Ricetta_Operazione_Cod IN (" & Agro_SQL_Save_Clausola_IN(Str_Ricetta_Operazione_Cod, False) & ") ")

            StrSQL.AppendLine(" UNION ")

            StrSQL.AppendLine(" SELECT Ricette_dettagli.Cau_Mov, Ricette_Operazioni.Ricetta_Operazione_Cod, Ricette_dettagli.Ricetta_Dettaglio_Cod, ")
            StrSQL.AppendLine(" 0, '', '', '', '', '', '', '',")
            StrSQL.AppendLine(" Parco_Macchine.Mac_Des, Parco_Macchine.Mac_Cod, ")
            StrSQL.AppendLine(" Parco_Macchine.Class_Code, Macchine.CLASS_DESC, Parco_Macchine.Modello, Parco_Macchine.Ultima_Manutenzione ")
            StrSQL.AppendLine(" , isnull(Parco_Macchine.Ditta_cod,0) AS Ditta_cod, ISNULL(Ditte.Ditta_des, '') as Ditta_des ")
            StrSQL.AppendLine(" FROM Parco_Macchine ")
            StrSQL.AppendLine(" INNER JOIN Ricette_dettagli ")
            StrSQL.AppendLine(" ON Parco_Macchine.Mac_Cod = Ricette_dettagli.Mat_Cod ")
            StrSQL.AppendLine(" INNER JOIN Ricette_Operazioni ")
            StrSQL.AppendLine(" ON Ricette_Operazioni.Ricetta_SuperUser = Ricette_dettagli.Ricetta_SuperUser AND Ricette_Operazioni.Ricetta_Operazione_Cod = Ricette_dettagli.Ricetta_Operazione_Cod ")
            StrSQL.AppendLine(" INNER JOIN Macchine ")
            StrSQL.AppendLine(" ON Parco_Macchine.Class_Code = Macchine.CLASS_CODE ")
            StrSQL.AppendLine(" LEFT JOIN Ditte ")
            StrSQL.AppendLine(" ON Parco_Macchine.Ditta_Cod= Ditte.Ditta_Cod ")
            StrSQL.AppendLine(" WHERE Ricette_dettagli.Cau_Mov = '" & CAU_IMPUTAZIONE_PARCOMACCHINE & "' ")
            StrSQL.AppendLine(" AND Ricette_dettagli.Elem_Cod = 1 ")
            StrSQL.AppendLine(" AND  Ricette_Operazioni.Ricetta_Operazione_Cod IN (" & Agro_SQL_Save_Clausola_IN(Str_Ricetta_Operazione_Cod, False) & ") ")

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            'Select Case objParametri.FlagVisibilita
            '    Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
            '        StrSQL.Append(" AND   Reg_Impianti_Codici.Inviato >=0 ")
            '    Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
            '        StrSQL.Append(" AND   Reg_Impianti_Codici.Inviato =-1 ")
            '    Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
            '        '...................................
            '    Case Else
            '        Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            'End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
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

End Class
