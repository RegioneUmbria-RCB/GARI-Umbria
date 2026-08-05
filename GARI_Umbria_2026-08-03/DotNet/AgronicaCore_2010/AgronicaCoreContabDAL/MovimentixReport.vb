Imports System.Data.Common
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi

Public Class MovimentixReport_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '################################################################################
    Public Function NumeroIdoneita_Classificazioni_from_Lotto(ByVal Piva As String, _
                                                             ByVal Lotto As String, _
                                                             ByVal Data_Riferimento As Date, _
                                                            ByVal xFiltroAggiuntivo As String, _
                                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As String

        Dim DT As DataTable
        Dim Num_Idoneita As String = ""

        DT = Leggi_NumeroIdoneita_Classificazioni(Piva, Lotto, Data_Riferimento, xFiltroAggiuntivo, objParametri)

        If Not IsNothing(DT) AndAlso DT.Rows.Count > 0 Then
            Num_Idoneita = DT.Rows(0).Item("Descrizione")
        End If

        Return Num_Idoneita

    End Function


    '#############################################################################
    'nel registro di imbottigliamento, Data_Riferimento è la data dell'imbottigliamento
    'in modo che se in un lotto di vinificazione ci sono più classificazioni e più imbottigliamenti
    'viene stampato il numero di idoneità della classificazione con data maggiore ma <= data imbottigliamento
    'nella stampa delle etichette di vasca, Data_Riferimento è la data odierna
    Public Function Leggi_NumeroIdoneita_Classificazioni(ByVal Piva As String, _
                                                        ByVal Lotto As String, _
                                                        ByVal Data_Riferimento As Date, _
                                                        ByVal xFiltroAggiuntivo As String, _
                                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                                            ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.MovimentixReport_R.Leggi_NumeroIdoneita_Classificazioni()"
        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            '------------------------------------------------------------------
            StrSQL.Length = 0

            'legge una riga x ogni record di Movimenti_dettagli per ogni record di MovimentixReport (di solito sono due record per i due id_report)
            StrSQL.Append(" SELECT DISTINCT  [Descrizione], MovimentixReport.Validita_Inizio " & vbCrLf)
            StrSQL.Append(" FROM MovimentixReport " & vbCrLf)
            StrSQL.Append(" INNER JOIN Movimenti_dettagli ON Movimenti_dettagli.PIVA=MovimentixReport.piva " & vbCrLf)
            StrSQL.Append(" -- il sa_cod è 0 in movxreport " & vbCrLf)
            StrSQL.Append(" --AND Movimenti_dettagli.sa_cod=MovimentixReport.sa_cod " & vbCrLf)
            StrSQL.Append("  AND Movimenti_dettagli.id_agenda=MovimentixReport.id_agenda  " & vbCrLf)
            StrSQL.Append(" -- l'id_mov è diverso " & vbCrLf)
            StrSQL.Append(" -- and Movimenti_dettagli.id_mov=MovimentixReport.id_mov " & vbCrLf)
            StrSQL.Append(" INNER JOIN Agenda ON Agenda.PIVA = MovimentixReport.PIVA AND Agenda.Sa_Cod = MovimentixReport.Sa_Cod AND Agenda.Id_Agenda = MovimentixReport.Id_Agenda " & vbCrLf)
            StrSQL.Append(" INNER JOIN Linee_Preparazioni ON Agenda.PIVA = Linee_Preparazioni.Piva AND  Agenda.PREPARAZIONE_COD = Linee_Preparazioni.Preparazione_Cod  " & vbCrLf)
            StrSQL.Append(" INNER JOIN Trasformazioni ON Movimenti_dettagli.Lotto= Trasformazioni.Trasformazione_Des AND Movimenti_dettagli.piva= Trasformazioni.piva  " & vbCrLf)

            StrSQL.Append(" WHERE  MovimentixReport.Descrizione <> '' " & vbCrLf)

            If Piva <> "" Then
                StrSQL.Append(" AND   Agenda.Piva = '" & Agro_SQL_SaveText(Piva) & "'   " & vbCrLf)
            End If

            If Lotto <> "" Then

                'correzione del 26/08/2015: 
                'gestione del caso in cui il lotto è un lotto collegato al lotto sul quale è stata fatta la classificazione
                'StrSQL.Append(" AND Movimenti_dettagli.Lotto = '" & Agro_SQL_SaveText(Lotto) & "' " & vbCrLf)

                StrSQL.Append(" AND (  " & vbCrLf)
                StrSQL.Append("     Trasformazioni.Trasformazione_Des = '" & Agro_SQL_SaveText(Lotto) & "' " & vbCrLf)
                StrSQL.Append("     OR " & vbCrLf)
                StrSQL.Append("     Trasformazioni.id_trasformazione IN " & vbCrLf)
                StrSQL.Append("         (" & vbCrLf)
                StrSQL.Append("         SELECT DISTINCT Id_Trasformazione_Rif " & vbCrLf)
                StrSQL.Append("         FROM Trasformazioni_Riferimenti " & vbCrLf)
                StrSQL.Append("         INNER JOIN Trasformazioni TR_Int ON Trasformazioni_Riferimenti.id_trasformazione= TR_Int.id_trasformazione " & vbCrLf)
                StrSQL.Append("                 AND Trasformazioni_Riferimenti.piva= TR_Int.piva " & vbCrLf)
                StrSQL.Append("         WHERE TR_Int.Trasformazione_Des = '" & Agro_SQL_SaveText(Lotto) & "' " & vbCrLf)
                StrSQL.Append("         " & vbCrLf)
                StrSQL.Append("         UNION ALL " & vbCrLf)
                StrSQL.Append("   " & vbCrLf)
                StrSQL.Append("         SELECT DISTINCT Trasformazioni_Riferimenti.Id_Trasformazione " & vbCrLf)
                StrSQL.Append("         FROM Trasformazioni_Riferimenti " & vbCrLf)
                StrSQL.Append("         INNER JOIN Trasformazioni TR_Int ON Trasformazioni_Riferimenti.id_trasformazione_rif= TR_Int.id_trasformazione " & vbCrLf)
                StrSQL.Append("                 AND Trasformazioni_Riferimenti.piva= TR_Int.piva " & vbCrLf)
                StrSQL.Append("         WHERE TR_Int.Trasformazione_Des = '" & Agro_SQL_SaveText(Lotto) & "' " & vbCrLf)
                StrSQL.Append("         " & vbCrLf)
                StrSQL.Append("         ) --IN" & vbCrLf)
                StrSQL.Append("  ) --AND " & vbCrLf)

            End If

            StrSQL.Append(" AND Linee_Preparazioni.Modulo_Generazione = " & Agro_SQL_SaveNum(enum_Omni_Modulo_Generazione.Cantine))
            StrSQL.Append(" AND Linee_Preparazioni.Codice_Generazione IN ( " & vbCrLf)
            StrSQL.Append(" " & Agro_SQL_SaveNum(enum_Omni_Preparazione_Cod.Classificazione) & ", " & vbCrLf)
            StrSQL.Append(" " & Agro_SQL_SaveNum(enum_Omni_Preparazione_Cod.ClassificazioneInLinee) & ", " & vbCrLf)
            StrSQL.Append(" " & Agro_SQL_SaveNum(enum_Omni_Preparazione_Cod.ClassificazioneVinoAttoFineSpumantizzazioneInAutoclave_RegCommercializzazione) & ", " & vbCrLf)
            StrSQL.Append(" " & Agro_SQL_SaveNum(enum_Omni_Preparazione_Cod.ClassificazioneVinoAttoFineSpumantizzazioneInAutoclave_RegVinificazione) & " " & vbCrLf)
            StrSQL.Append("                     ) " & vbCrLf)

            'MODIFICA DEL 09/12/2015
            'in modo che se in un lotto di vinificazione ci sono più classificazioni e più imbottigliamenti
            'viene stampato il numero di idoneità della classificazione con data maggiore ma <= data imbottigliamento
            If Data_Riferimento <> AGRODATAINIZIO Then
                StrSQL.Append("  AND MovimentixReport.Validita_Inizio <= " & Agro_SQL_SaveDate(Data_Riferimento) & " " & vbCrLf)
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri) & vbCrLf)
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   MovimentixReport.Inviato >=0 " & vbCrLf)
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   MovimentixReport.Inviato =-1 " & vbCrLf)
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)" & vbCrLf)
            End Select
            '--------------------------------------------------------------------------
            StrSQL.Append(" ORDER BY MovimentixReport.Validita_Inizio DESC " & vbCrLf)


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
    Public Function Descrizione_from_MovimentixReport(ByVal Piva As String, _
                                                      ByVal Sa_Cod As Integer, _
                                                      ByVal Id_Agenda As Integer, _
                                                      ByVal Id_Report As Integer, _
                                                       ByVal xFiltroAggiuntivo As String, _
                                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As String

        Dim DT As DataTable
        Dim strRet As String = ""

        DT = LeggiJOINLineePreparazionixReport(Piva, Sa_Cod, Id_Agenda, Id_Report, xFiltroAggiuntivo, "", objParametri)

        If Not IsNothing(DT) AndAlso DT.Rows.Count > 0 Then
            strRet = DT.Rows(0).Item("Descrizione")
        End If

        Return strRet

    End Function


    '#############################################################################
    Public Function LeggiJOINLineePreparazionixReport( _
                        ByVal Piva As String, _
                        ByVal Sa_Cod As Int32, _
                        ByVal Id_Agenda As Int32, _
                        ByVal Id_Report As Int32, _
                           ByVal xFiltroAggiuntivo As String, _
                            ByVal xOrderBy As String, _
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                            ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.MovimentixReport_R.LeggiJOINLineePreparazionixReport()"

        '====================================================================================
        'Parametri opzionali :
        '   Sa_Cod = 0 
        '   Id_Agenda = 0    
        '   Id_Mov = 0
        '   Id_Mov_Det = 0
        '   Id_Report = 0    
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            '------------------------------------------------------------------
            StrSQL.Length = 0

            StrSQL.Append(" SELECT Linee_Preparazioni.Preparazione_Cod, Linee_Preparazioni.Preparazione_Des, Linee_PreparazionixReport.Id_Report, ")
            StrSQL.Append(" Linee_PreparazionixReport.StrCampo_Registri, MovimentixReport.Descrizione, Agenda.PIVA, Agenda.Sa_Cod, Agenda.Id_Agenda, ")
            StrSQL.Append(" Movimenti.Id_Mov ")

            StrSQL.Append(" FROM Agenda  ")
            StrSQL.Append(" INNER JOIN Movimenti ON Agenda.PIVA = Movimenti.PIVA AND Agenda.Sa_Cod = Movimenti.Sa_Cod AND Agenda.Id_Agenda = Movimenti.Id_Agenda ")
            StrSQL.Append(" INNER JOIN MovimentixReport ON Movimenti.PIVA = MovimentixReport.Piva AND Movimenti.Sa_Cod = MovimentixReport.Sa_Cod AND ")
            StrSQL.Append(" Movimenti.Id_Agenda = MovimentixReport.Id_Agenda AND Movimenti.Id_Mov = MovimentixReport.Id_Mov ")
            StrSQL.Append(" INNER JOIN Linee_Preparazioni ON Agenda.PIVA = Linee_Preparazioni.Piva AND ")
            StrSQL.Append(" Agenda.PREPARAZIONE_COD = Linee_Preparazioni.Preparazione_Cod ")
            StrSQL.Append(" INNER JOIN Linee_PreparazionixReport ON Linee_Preparazioni.Piva = Linee_PreparazionixReport.Piva AND ")
            StrSQL.Append(" Linee_Preparazioni.Preparazione_Cod = Linee_PreparazionixReport.Preparazione_Cod ")

            StrSQL.Append(" WHERE Movimenti.Cau_Mov = '" & CStr(CAU_LINEA_PRODUZIONE) & "' ")

            If Piva <> "" Then
                StrSQL.Append(" AND   Agenda.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
            End If

            If Sa_Cod <> 0 Then
                StrSQL.Append(" AND Agenda.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
            End If

            If Id_Agenda <> 0 Then
                StrSQL.Append(" AND Agenda.Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & "   ")
            End If

            If Id_Report <> 0 Then
                StrSQL.Append(" AND Linee_PreparazionixReport.Id_Report = " + Agro_SQL_SaveNum(Id_Report) + "  ")
                'aggiunta clausaola where su MovimentixReport in data 13/10/2014
                StrSQL.Append(" AND MovimentixReport.Id_Report = " + Agro_SQL_SaveNum(Id_Report) + "  ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   MovimentixReport.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   MovimentixReport.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
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


    '#############################################################################
    Public Function Leggi( _
                        ByVal Piva As String, _
                        ByVal Sa_Cod As Int32, _
                        ByVal Id_Agenda As Int32, _
                        ByVal Id_Mov As Int32, _
                        ByVal Id_Mov_Det As Int32, _
                        ByVal Id_Report As Int32, _
                            ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile, _
                            ByVal xFiltroAggiuntivo As String, _
                            ByVal xOrderBy As String, _
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                            ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.MovimentixReport_R.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   Sa_Cod = 0 
        '   Id_Agenda = 0    
        '   Id_Mov = 0
        '   Id_Mov_Det = 0
        '   Id_Report = 0    
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            '------------------------------------------------------------------
            StrSQL.Length = 0

            StrSQL.Append(" SELECT * ")
            StrSQL.Append(" FROM  MovimentixReport ")
            StrSQL.Append(" WHERE Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.Append(" AND   Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
            StrSQL.Append(" AND   Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")

            If Sa_Cod <> 0 Then
                StrSQL.Append(" AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
            End If

            If Id_Agenda <> 0 Then
                StrSQL.Append(" AND Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & "   ")
            End If

            If Id_Mov <> 0 Then
                StrSQL.Append(" AND Id_Mov = " & Agro_SQL_SaveNum(Id_Mov) & "   ")
            End If

            If Id_Mov_Det <> 0 Then
                StrSQL.Append(" AND Id_Mov_Det = " & Agro_SQL_SaveNum(Id_Mov_Det) & "   ")
            End If

            If Id_Report <> 0 Then
                StrSQL.Append(" AND Id_Report = " & Agro_SQL_SaveNum(Id_Report) & "   ")
            End If



            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY Piva, Sa_Cod, Id_Agenda, Id_Mov, Id_Mov_Det, Id_Report Asc ")
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


    '#############################################################################
    Public Function Leggi_Partite_LineeProduzioni( _
                        ByVal Piva As String, _
                        ByVal Linea_Cod As Int32, _
                        ByVal Id_Report As enum_AgroReportistica, _
                        ByVal Flag_ScartaPartiteNonValorizzate As Boolean, _
                            ByVal xFiltroAggiuntivo As String, _
                            ByVal xOrderBy As String, _
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                            ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.MovimentixReport_R.Leggi_Partite_LineeProduzioni()"

        '====================================================================================
        'Parametri opzionali :
        '   Id_Report = 0  

        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            '------------------------------------------------------------------
            StrSQL.Length = 0

            ' non ho messo in join i movimenti dettagli perchè non ci sono per i frizzanti
            StrSQL.Append(" SELECT MR.* " & vbCrLf)
            StrSQL.Append(" FROM Linee_ProduzionixPreparazioni " & vbCrLf)
            StrSQL.Append(" INNER JOIN Linee_Preparazioni ON Linee_ProduzionixPreparazioni.Preparazione_Cod= Linee_Preparazioni.Preparazione_Cod " & vbCrLf)
            StrSQL.Append(" AND Linee_ProduzionixPreparazioni.Piva= Linee_Preparazioni.Piva " & vbCrLf)
            StrSQL.Append(" INNER JOIN Agenda A ON A.PREPARAZIONE_COD=Linee_Preparazioni.Preparazione_Cod AND A.Piva = Linee_Preparazioni.Piva " & vbCrLf)
            StrSQL.Append(" INNER JOIN Movimenti M ON A.Piva = M.PIVA AND A.Id_Agenda =M.Id_Agenda " & vbCrLf)
            'StrSQL.Append(" INNER JOIN Movimenti_dettagli MD ON MD.Piva = M.PIVA AND MD.Id_Agenda =M.Id_Agenda AND MD.Id_Mov=M.Id_Mov ")
            StrSQL.Append(" INNER JOIN MovimentixReport MR ON M.Piva = MR.PIVA AND M.Id_Agenda =MR.Id_Agenda AND M.Id_Mov=MR.Id_Mov " & vbCrLf)
            '--AND MD.Id_Mov_Det=MR.Id_Mov_Det 

            StrSQL.Append(" WHERE MR.Piva = '" & Agro_SQL_SaveText(Piva) & "' " & vbCrLf)

            StrSQL.Append(" AND Linee_ProduzionixPreparazioni.Linea_Cod = " & Agro_SQL_SaveNum(Linea_Cod) & "   " & vbCrLf)

            'il report lo devo filtrare, evito che mi arrivi 0
            'If Id_Report <> 0 Then
            StrSQL.Append(" AND Id_Report = " & Agro_SQL_SaveNum(Id_Report) & "   " & vbCrLf)
            ' End If

            'aggiunto il filtro 29/11/13: verifico di controllare la partita nelle operazioni di inizio frizzantatura/spumantizzazione
            Select Case Id_Report
                Case enum_AgroReportistica.Frizzanti
                    StrSQL.Append(" AND Linee_Preparazioni.Modulo_Generazione = " & Agro_SQL_SaveNum(enum_Omni_Modulo_Generazione.Cantine))
                    StrSQL.Append(" AND Linee_Preparazioni.Codice_Generazione IN ( " & vbCrLf)
                    StrSQL.Append(" " & Agro_SQL_SaveNum(enum_Omni_Preparazione_Cod.ImbottigliamentoInizioFrizzantaturaBottiglia_RegVinificazione) & ", " & vbCrLf)
                    StrSQL.Append(" " & Agro_SQL_SaveNum(enum_Omni_Preparazione_Cod.ImbottigliamentoInizioFrizzantaturaBottiglia_RegCommercializzazione) & ", " & vbCrLf)
                    StrSQL.Append(" " & Agro_SQL_SaveNum(enum_Omni_Preparazione_Cod.ImbottigliamentoInizioFrizzantaturaBottigliaViniQualita_RegVinificazione) & ", " & vbCrLf)
                    StrSQL.Append(" " & Agro_SQL_SaveNum(enum_Omni_Preparazione_Cod.ImbottigliamentoInizioFrizzantaturaBottigliaViniQualita_RegCommercializzazione) & ", " & vbCrLf)
                    StrSQL.Append(" " & Agro_SQL_SaveNum(enum_Omni_Preparazione_Cod.InizioFrizzantaturaAutoclaveMPF_RegVinificazione) & ", " & vbCrLf)
                    StrSQL.Append(" " & Agro_SQL_SaveNum(enum_Omni_Preparazione_Cod.InizioFrizzantaturaAutoclaveMPFxVinoAttoADivenire_RegVinificazione) & ", " & vbCrLf)
                    StrSQL.Append(" " & Agro_SQL_SaveNum(enum_Omni_Preparazione_Cod.InizioFrizzantaturaAutoclaveVNAF_RegVinificazione) & ", " & vbCrLf)
                    StrSQL.Append(" " & Agro_SQL_SaveNum(enum_Omni_Preparazione_Cod.InizioFrizzantaturaAutoclaveVNAFxVinoAttoADivenire_RegVinificazione) & ", " & vbCrLf)
                    StrSQL.Append(" " & Agro_SQL_SaveNum(enum_Omni_Preparazione_Cod.InizioFrizzantaturaAutoclave_RegVinificazione) & ", " & vbCrLf)
                    StrSQL.Append(" " & Agro_SQL_SaveNum(enum_Omni_Preparazione_Cod.InizioFrizzantaturaAutoclave_RegCommercializzazione) & ", " & vbCrLf)
                    StrSQL.Append(" " & Agro_SQL_SaveNum(enum_Omni_Preparazione_Cod.InizioFrizzantaturaAutoclave_VinoAttoA_RegCommercializzazione) & ", " & vbCrLf)
                    StrSQL.Append(" " & Agro_SQL_SaveNum(enum_Omni_Preparazione_Cod.InizioFrizzantaturaAutoclave_VinoAttoA_RegVinificazione) & " " & vbCrLf)
                    StrSQL.Append("                     ) " & vbCrLf)

                    'CStr(enum_Omni_Preparazione_Cod.Frizzantatura_Imbottigliamento_RegVinificazione) & "," & _
                    'CStr(enum_Omni_Preparazione_Cod.Frizzantatura_Imbottigliamento_RegCommercializzazione) & "," & _

                Case enum_AgroReportistica.Spumanti
                    StrSQL.Append(" AND Linee_Preparazioni.Modulo_Generazione = " & Agro_SQL_SaveNum(enum_Omni_Modulo_Generazione.Cantine))
                    StrSQL.Append(" AND Linee_Preparazioni.Codice_Generazione IN ( " & vbCrLf)
                    StrSQL.Append(" " & Agro_SQL_SaveNum(enum_Omni_Preparazione_Cod.ImbottigliamentoInizioSpumantizzazioneBottiglia_RegCommercializzazione) & ", " & vbCrLf)
                    StrSQL.Append(" " & Agro_SQL_SaveNum(enum_Omni_Preparazione_Cod.ImbottigliamentoInizioSpumantizzazioneBottiglia_RegVinificazione) & ", " & vbCrLf)
                    StrSQL.Append(" " & Agro_SQL_SaveNum(enum_Omni_Preparazione_Cod.ImbottigliamentoInizioSpumantizzazioneBottigliaViniQualita_RegCommercializzazione) & ", " & vbCrLf)
                    StrSQL.Append(" " & Agro_SQL_SaveNum(enum_Omni_Preparazione_Cod.ImbottigliamentoInizioSpumantizzazioneBottigliaViniQualita_RegVinificazione) & ", " & vbCrLf)
                    StrSQL.Append(" " & Agro_SQL_SaveNum(enum_Omni_Preparazione_Cod.InizioSpumantizzazioneAutoclaveMPF_RegVinificazione) & ", " & vbCrLf)
                    StrSQL.Append(" " & Agro_SQL_SaveNum(enum_Omni_Preparazione_Cod.InizioSpumantizzazioneAutoclaveVNAF_RegVinificazione) & ", " & vbCrLf)
                    StrSQL.Append(" " & Agro_SQL_SaveNum(enum_Omni_Preparazione_Cod.InizioSpumantizzazioneAutoclaveMPF_XVinoAttoADivenire_RegVinificazione) & ", " & vbCrLf)
                    StrSQL.Append(" " & Agro_SQL_SaveNum(enum_Omni_Preparazione_Cod.InizioSpumantizzazioneAutoclaveVNAF_XVinoAttoADivenire_RegVinificazione) & ", " & vbCrLf)
                    StrSQL.Append(" " & Agro_SQL_SaveNum(enum_Omni_Preparazione_Cod.InizioSpumantizzazioneAutoclave_RegCommercializzazione) & ", " & vbCrLf)
                    StrSQL.Append(" " & Agro_SQL_SaveNum(enum_Omni_Preparazione_Cod.InizioSpumantizzazioneAutoclave_RegVinificazione) & ", " & vbCrLf)
                    StrSQL.Append(" " & Agro_SQL_SaveNum(enum_Omni_Preparazione_Cod.InizioSpumantizzazioneAutoclave_VinoAttoA_RegCommercializzazione) & ", " & vbCrLf)
                    StrSQL.Append(" " & Agro_SQL_SaveNum(enum_Omni_Preparazione_Cod.InizioSpumantizzazioneAutoclave_VinoAttoA_RegVinificazione) & " " & vbCrLf)
                    StrSQL.Append("                     ) " & vbCrLf)

            End Select

            If Flag_ScartaPartiteNonValorizzate = True Then
                StrSQL.Append(" AND MR.Descrizione <> '' AND MR.Descrizione IS NOT NULL    " & vbCrLf)
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri) & vbCrLf)
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   Linee_ProduzionixPreparazioni.Inviato >=0 ")
                    StrSQL.Append(" AND   Linee_Preparazioni.Inviato >=0 ")
                    StrSQL.Append(" AND   A.Inviato >=0 ")
                    StrSQL.Append(" AND   M.Inviato >=0 ")
                    StrSQL.Append(" AND   MR.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   Linee_ProduzionixPreparazioni.Inviato =-1 ")
                    StrSQL.Append(" AND   Linee_Preparazioni.Inviato =-1 ")
                    StrSQL.Append(" AND   A.Inviato =-1 ")
                    StrSQL.Append(" AND   M.Inviato =-1 ")
                    StrSQL.Append(" AND   MR.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)" & vbCrLf)
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri) & vbCrLf)
            Else
                StrSQL.Append(" ORDER BY MR.Descrizione " & vbCrLf)
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





'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################






Public Class MovimentixReport_W
    Inherits AgronicaCoreDataProvider.DataProvider




    ''============================================================================
    'Public Function Scrivi( _
    '                        ByVal Piva As String, _
    '                        ByVal Sa_Cod As Int32, _
    '                        ByVal Id_Agenda As Int32, _
    '                        ByVal Id_Mov As Int32, _
    '                        ByVal Id_Mov_Det As Int32, _
    '                        ByVal Id_Report As Int32, _
    '                        ByVal Descrizione As String, _
    '                            ByVal UserName_Creazione As String, _
    '                            ByVal FinestraTemp_Inizio As Date, _
    '                            ByVal FinestraTemp_Fine As Date, _
    '                            ByRef objConnessione As DbConnection, _
    '                            ByRef objTransazione As DbTransaction, _
    '                            ByVal StringaConnessione As String, _
    '                            ByVal DirectoryLOG As String, _
    '                            ByVal FileLOG As String, _
    '                            ByVal IdentificatoreUtente As String _
    '                            ) As Boolean

    '    Dim NomeRoutine As String = "AgronicaCoreContabDAL.MovimentixReport_W.Scrivi()"

    '    '====================================================================================
    '    'Parametri opzionali :
    '    '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
    '    '   DirectoryLOG = ""           =>  viene usato il valore di default
    '    '   FileLOG = ""                =>  viene usato il valore di default
    '    '====================================================================================

    '    Dim MessaggioErrore As String = ""
    '    Dim StrSQL As New System.Text.StringBuilder
    '    Dim xRisp As Boolean = False

    '    Try

    '        '---------------------------------------------
    '        StrSQL.Length = 0

    '        StrSQL.Append(" INSERT INTO MovimentixReport ")
    '        StrSQL.Append("         ( ")
    '        StrSQL.Append("          Piva,       Sa_Cod,    Id_Agenda,    Id_Mov, ")
    '        StrSQL.Append("          Id_Mov_Det, Id_Report, Descrizione,          ")

    '        StrSQL.Append("          Inviato,            DataInvio, ")
    '        StrSQL.Append("          Data_Creazione,     Data_Modifica, ")
    '        StrSQL.Append("          UserName_Creazione, UserName_Modifica, ")
    '        StrSQL.Append("          Validita_Inizio,    Validita_Fine ")
    '        StrSQL.Append("         ) ")

    '        StrSQL.Append(" VALUES ( ")
    '        StrSQL.Append("          '" & Agro_SQL_SaveText(Piva) & "'  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(Id_Agenda) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(Id_Mov) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(Id_Mov_Det) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(Id_Report) & "  ")
    '        StrSQL.Append("         ,'" & Agro_SQL_SaveText(Descrizione) & "'  ")

    '        StrSQL.Append("         , 0  ")
    '        StrSQL.Append("         , Null  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveDate(Date.Now) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveDate(Date.Now) & "  ")
    '        StrSQL.Append("         ,'" & Agro_SQL_SaveText(UserName_Creazione) & "' ")
    '        StrSQL.Append("         ,'" & Agro_SQL_SaveText(UserName_Creazione) & "' ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveDate(FinestraTemp_Inizio) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveDate(FinestraTemp_Fine) & "  ")

    '        StrSQL.Append(") ")

    '        '---------------------------------------------
    '        xRisp = EseguiQuery_Scrittura(objConnessione, objTransazione, StringaConnessione, StrSQL.ToString, DirectoryLOG, FileLOG, IdentificatoreUtente, NomeRoutine)

    '    Catch ex As Exception

    '        MessaggioErrore = ex.Message
    '        Scrivi_LOG(DirectoryLOG, FileLOG, IdentificatoreUtente, NomeRoutine, MessaggioErrore)
    '        xRisp = False
    '        Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

    '    End Try

    '    Return xRisp

    'End Function




    Public Function Scrivi( _
                                ByVal Piva As String, _
                                ByVal Sa_Cod As Int32, _
                                ByVal Id_Agenda As Int32, _
                                ByVal Id_Mov As Int32, _
                                ByVal Id_Mov_Det As Int32, _
                                ByVal Id_Report As Int32, _
                                ByVal Descrizione As String, _
                                    ByVal Validita_Inizio As Date, _
                                    ByVal Validita_Fine As Date, _
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                    , Optional ByVal Data_creazione As Date = #2/1/1900# _
                    , Optional ByVal Data_modifica As Date = #2/1/1900# _
                    , Optional ByVal username_creazione As String = "" _
                    , Optional ByVal username_modifica As String = "" _
                                        ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.MovimentixReport_W.Scrivi()"

        '====================================================================================
        'Parametri opzionali :
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
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

            StrSQL.Append(" INSERT INTO MovimentixReport ")
            StrSQL.Append("         ( ")
            StrSQL.Append("          Piva,       Sa_Cod,    Id_Agenda,    Id_Mov, ")
            StrSQL.Append("          Id_Mov_Det, Id_Report, Descrizione,          ")

            StrSQL.Append("          Inviato,            DataInvio, ")
            StrSQL.Append("          Data_Creazione,     Data_Modifica, ")
            StrSQL.Append("          UserName_Creazione, UserName_Modifica, ")
            StrSQL.Append("          Validita_Inizio,    Validita_Fine ")
            StrSQL.Append("         ) ")

            StrSQL.Append(" VALUES ( ")
            StrSQL.Append("          '" & Agro_SQL_SaveText(Piva) & "'  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Id_Agenda) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Id_Mov) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Id_Mov_Det) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Id_Report) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Descrizione) & "'  ")

            StrSQL.Append("         , 0  ")
            StrSQL.Append("         , Null  ")

            StrSQL.Append("			, " & Agro_SQL_SaveDate(Data_creazione) & "  ")
            StrSQL.Append("			, " & Agro_SQL_SaveDate(Data_modifica) & "  ")
            StrSQL.Append("			,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            StrSQL.Append("			,'" & Agro_SQL_SaveText(username_modifica) & "' ")



            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")

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






    ''============================================================================
    'Public Function Cancella( _
    '                        ByVal Piva As String, _
    '                        ByVal Sa_Cod As Int32, _
    '                        ByVal Id_Agenda As Int32, _
    '                        ByVal Id_Mov As Int32, _
    '                        ByVal Id_Mov_Det As Int32, _
    '                        ByVal Id_Report As Int32, _
    '                            ByVal UserName_Modifica As String, _
    '                            ByVal FlagCancellazioneLogica As Int32, _
    '                            ByRef objConnessione As DbConnection, _
    '                            ByRef objTransazione As DbTransaction, _
    '                            ByVal StringaConnessione As String, _
    '                            ByVal DirectoryLOG As String, _
    '                            ByVal FileLOG As String, _
    '                            ByVal IdentificatoreUtente As String _
    '                            ) As Boolean

    '    Dim NomeRoutine As String = "AgronicaCoreContabDAL.MovimentixReport_W.Cancella()"

    '    '====================================================================================
    '    'Parametri opzionali :
    '    '   Sa_Cod = 0
    '    '   Id_Agenda = 0
    '    '   Id_Mov = 0
    '    '   Id_Mov_Det = 0
    '    '   Id_Report = 0

    '    '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
    '    '   DirectoryLOG = ""           =>  viene usato il valore di default
    '    '   FileLOG = ""                =>  viene usato il valore di default
    '    '====================================================================================

    '    Dim MessaggioErrore As String = ""
    '    Dim StrSQL As New System.Text.StringBuilder
    '    Dim xRisp As Boolean = False

    '    Try

    '        '---------------------------------------------
    '        If FlagCancellazioneLogica Then

    '            StrSQL.Length = 0
    '            StrSQL.Append(" UPDATE MovimentixReport ")
    '            StrSQL.Append(" SET ")
    '            StrSQL.Append("      Username_Modifica = '" & Agro_SQL_SaveText(UserName_Modifica) & "' ")
    '            StrSQL.Append("      ,Inviato = -1 ")
    '            StrSQL.Append(" WHERE  Inviato >= 0 ")

    '        Else
    '            StrSQL.Length = 0
    '            StrSQL.Append(" DELETE ")
    '            StrSQL.Append(" FROM MovimentixReport ")
    '            StrSQL.Append(" WHERE  1=1 ")
    '        End If


    '        StrSQL.Append(" AND Piva = '" & Agro_SQL_SaveText(Piva) & "' ")


    '        If Sa_Cod <> 0 Then
    '            StrSQL.Append(" AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
    '        End If

    '        If Id_Agenda <> 0 Then
    '            StrSQL.Append(" AND Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & "   ")
    '        End If

    '        If Id_Mov <> 0 Then
    '            StrSQL.Append(" AND Id_Mov = " & Agro_SQL_SaveNum(Id_Mov) & "   ")
    '        End If

    '        If Id_Mov_Det <> 0 Then
    '            StrSQL.Append(" AND Id_Mov_Det = " & Agro_SQL_SaveNum(Id_Mov_Det) & "   ")
    '        End If

    '        If Id_Report <> 0 Then
    '            StrSQL.Append(" AND Id_Report = " & Agro_SQL_SaveNum(Id_Report) & "   ")
    '        End If

    '        '---------------------------------------------
    '        xRisp = EseguiQuery_Scrittura(objConnessione, objTransazione, StringaConnessione, StrSQL.ToString, DirectoryLOG, FileLOG, IdentificatoreUtente, NomeRoutine)

    '    Catch ex As Exception

    '        MessaggioErrore = ex.Message
    '        Scrivi_LOG(DirectoryLOG, FileLOG, IdentificatoreUtente, NomeRoutine, MessaggioErrore)
    '        xRisp = False
    '        Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

    '    End Try

    '    Return xRisp

    'End Function



    Public Function Cancella( _
                            ByVal Piva As String, _
                            ByVal Sa_Cod As Int32, _
                            ByVal Id_Agenda As Int32, _
                            ByVal Id_Mov As Int32, _
                            ByVal Id_Mov_Det As Int32, _
                            ByVal Id_Report As Int32, _
                                ByVal xFiltroAggiuntivo As String, _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.MovimentixReport_W.Cancella()"

        '====================================================================================
        'Parametri opzionali :
        '   Sa_Cod = 0
        '   Id_Agenda = 0
        '   Id_Mov = 0
        '   Id_Mov_Det = 0
        '   Id_Report = 0

        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = AgronicaCoreDataProvider.AgronicaCoreParametri.enumCancellazioneLogica.CancellazioneLogica Then

                StrSQL.Length = 0
                StrSQL.Append(" UPDATE MovimentixReport ")
                StrSQL.Append(" SET ")
                StrSQL.Append("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("      ,Inviato = -1 ")
                StrSQL.Append(" WHERE  Inviato >= 0 ")

            Else
                StrSQL.Length = 0
                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM MovimentixReport ")
                StrSQL.Append(" WHERE  1=1 ")
            End If


            StrSQL.Append(" AND Piva = '" & Agro_SQL_SaveText(Piva) & "' ")


            If Sa_Cod <> 0 Then
                StrSQL.Append(" AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
            End If

            If Id_Agenda <> 0 Then
                StrSQL.Append(" AND Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & "   ")
            End If

            If Id_Mov <> 0 Then
                StrSQL.Append(" AND Id_Mov = " & Agro_SQL_SaveNum(Id_Mov) & "   ")
            End If

            If Id_Mov_Det <> 0 Then
                StrSQL.Append(" AND Id_Mov_Det = " & Agro_SQL_SaveNum(Id_Mov_Det) & "   ")
            End If

            If Id_Report <> 0 Then
                StrSQL.Append(" AND Id_Report = " & Agro_SQL_SaveNum(Id_Report) & "   ")
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





End Class
