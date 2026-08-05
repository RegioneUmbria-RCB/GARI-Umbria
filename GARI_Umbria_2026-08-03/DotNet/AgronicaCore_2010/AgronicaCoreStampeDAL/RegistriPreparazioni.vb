Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.DataProviderExtensions
Imports AgronicaCoreDataProvider.TipiEnumerativi


Public Class RegistriPreparazioni
    Inherits AgronicaCoreDataProvider.DataProvider

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Lettura delle operazioni di agenda della linea/preparazione in oggetto
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Public Function PreparazioniBIO_Agenda(ByVal Piva As String,
                                           ByVal Linea_Cod As Integer,
                                           ByVal Preparazione_Cod As Integer,
                                           ByVal Id_Agenda As Integer,
                                           ByVal Mat_Cod As Integer,
                                           ByVal Data_Inizio As Date,
                                           ByVal Data_Fine As Date,
                                           ByVal xFiltroAggiuntivo As String,
                                           ByVal xOrderBy As String,
                                           ByRef objParametri As AgronicaCoreParametri
                                           ) As DataTable

        Const nomeRoutine = "AgronicaCoreStampeDAL.RegistriPreparazioni.PreparazioniBIO_Agenda"

        Dim messaggioErrore As String = ""
        Dim StbSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try

            StbSQL.Length = 0


            '------------------------------------------------------ 
            '------------------- SELECT ---------------------------
            '------------------------------------------------------
            'sono in sospeso le confezioni

            StbSQL.Append(" SELECT  Agenda.piva, Agenda.Sa_Cod, Agenda.Lav_Cod, Agenda.des_lib, Agenda.Id_Agenda, Agenda.PREPARAZIONE_COD, Agenda.ID_TRASFORMAZIONE, Agenda.LINEA_COD, " & vbCrLf)
            StbSQL.Append("          Mov_Carico.Id_Mov, Mov_Carico.Data_Movimento, Mov_Carico.Mov_Desc, Mov_Carico.Extra_Str, " & vbCrLf)
            StbSQL.Append("  " & vbCrLf)
            StbSQL.Append("         Mov_Dett_Carico.Qta, Mov_Dett_Carico.Udm_Cod,Mov_Dett_Carico.Mov_Det_Des, Mov_Dett_Carico.Lotto, Mov_Dett_Carico.QTA_EXTRA, Mov_Dett_Carico.UDM_COD_EXTRA, " & vbCrLf)
            StbSQL.Append("         UnitaMisura.UDM_SIM, UnitaMisura.UDM_DES," & vbCrLf)
            StbSQL.Append("         Linee_Preparazioni_Dettagli.Dettaglio_Cod, Linee_Preparazioni_Dettagli.Dettaglio_Des, " & vbCrLf)
            StbSQL.Append("         UdmConf.UDM_SIM AS Conf_Udm_sim, UdmConf.UDM_DES AS conf_Udm_des, " & vbCrLf)
            StbSQL.Append("         Materie_Prime.Qta_Contenitore " & vbCrLf)
            StbSQL.Append(" , ISNULL( ( SELECT TOP 1  MP_Ingredienti.mat_des  " & vbCrLf)
            StbSQL.Append("             + ', Lotto:' + Mov_Dett_Scarico.lotto  " & vbCrLf)
            StbSQL.Append("             + ', '   + UnitaMisura.udm_sim COLLATE SQL_Latin1_General_CP850_CI_AS + ' ' + " & vbCrLf)
            StbSQL.Append("             + convert(varchar(50),  Mov_Dett_Scarico.qta)  " & vbCrLf)
            StbSQL.Append("             FROM Movimenti Mov_Scarico " & vbCrLf)
            StbSQL.Append("             INNER JOIN Movimenti_Dettagli Mov_Dett_Scarico ON Mov_Scarico.PIVA = Mov_Dett_Scarico.PIVA AND Mov_Scarico.Id_Agenda = Mov_Dett_Scarico.Id_Agenda AND Mov_Scarico.Id_Mov = Mov_Dett_Scarico.Id_Mov  " & vbCrLf)
            StbSQL.Append("             INNER JOIN Materie_Prime MP_Ingredienti ON Mov_Dett_Scarico.Elem_Cod = MP_Ingredienti.Elem_Cod AND Mov_Dett_Scarico.Mat_Cod = MP_Ingredienti.Mat_Cod  " & vbCrLf)
            StbSQL.Append("             INNER JOIN UnitaMisura ON UnitaMisura.Udm_Cod = Mov_Dett_Scarico.Udm_Cod  " & vbCrLf)
            StbSQL.Append("             INNER JOIN Materie_PrimexReport MP_IngredientixReport ON Mov_Dett_Scarico.Mat_Cod = MP_IngredientixReport.Mat_Cod   " & vbCrLf)
            StbSQL.Append("             INNER JOIN Linee_Preparazioni_Dettagli ON MP_IngredientixReport.PIVA = Linee_Preparazioni_Dettagli.PIVA AND Agenda.Preparazione_Cod = Linee_Preparazioni_Dettagli.Preparazione_Cod AND MP_IngredientixReport.Mat_Cod = Linee_Preparazioni_Dettagli.Mat_Cod   " & vbCrLf)
            StbSQL.Append("             WHERE Agenda.PIVA = Mov_Scarico.PIVA AND Agenda.Sa_Cod = Mov_Scarico.Sa_Cod AND Agenda.Id_Agenda = Mov_Scarico.Id_Agenda    " & vbCrLf)
            StbSQL.Append("             AND Mov_Scarico.Cau_Mov = '" & Agro_SQL_SaveText(CAU_SCARICO) & "' " & vbCrLf)
            StbSQL.Append("             AND Linee_Preparazioni_Dettagli.ChkBase = 1 " & vbCrLf)
            StbSQL.Append("              ), '' ) AS Note_Ingredienti " & vbCrLf)
            StbSQL.Append("  " & vbCrLf)

            '------------------------------------------------------ 
            '-------------------- FROM ----------------------------
            '------------------------------------------------------
            StbSQL.Append(" FROM Agenda  " & vbCrLf)

            StbSQL.Append(" INNER JOIN Movimenti Mov_Carico ON Agenda.PIVA = Mov_Carico.PIVA AND Agenda.Sa_Cod = Mov_Carico.Sa_Cod AND Agenda.Id_Agenda = Mov_Carico.Id_Agenda  " & vbCrLf)

            StbSQL.Append(" INNER JOIN Movimenti_Dettagli Mov_Dett_Carico ON Mov_Carico.PIVA = Mov_Dett_Carico.PIVA AND Mov_Carico.Id_Agenda = Mov_Dett_Carico.Id_Agenda AND Mov_Carico.Id_Mov = Mov_Dett_Carico.Id_Mov  " & vbCrLf)

            StbSQL.Append(" INNER JOIN UnitaMisura ON UnitaMisura.Udm_Cod = Mov_Dett_Carico.Udm_Cod " & vbCrLf)
            StbSQL.Append(" INNER JOIN UnitaMisura UdmConf ON UdmConf.Udm_Cod = Mov_Dett_Carico.UDM_COD_EXTRA " & vbCrLf)

            ''va eliminato il commento: è stato commentato per un bug del giaslan
            'StbSQL.Append(" INNER JOIN MovimentixReport ON MovimentixReport.PIVA = Mov_Dett_Carico.PIVA  " + vbCrLf)
            'StbSQL.Append("             AND MovimentixReport.Sa_Cod = Mov_Dett_Carico.Sa_Cod   " + vbCrLf)
            'StbSQL.Append("             AND MovimentixReport.Id_Agenda = Mov_Dett_Carico.Id_Agenda  " + vbCrLf)
            'StbSQL.Append("             AND MovimentixReport.Id_Mov = Mov_Dett_Carico.Id_Mov  " + vbCrLf)
            'StbSQL.Append("             AND MovimentixReport.Id_Mov_Det = Mov_Dett_Carico.Id_Mov_Det  " + vbCrLf)

            StbSQL.Append(" INNER JOIN Materie_Prime ON Mov_Dett_Carico.Elem_Cod = Materie_Prime.Elem_Cod AND Mov_Dett_Carico.Mat_Cod = Materie_Prime.Mat_Cod  " & vbCrLf)
            StbSQL.Append(" INNER JOIN Materie_PrimexReport MP_finitexReport ON Mov_Dett_Carico.Mat_Cod = MP_finitexReport.Mat_Cod  " & vbCrLf)

            StbSQL.Append(" INNER JOIN Linee_Preparazioni_Dettagli ON MP_finitexReport.PIVA = Linee_Preparazioni_Dettagli.PIVA AND Agenda.Preparazione_Cod = Linee_Preparazioni_Dettagli.Preparazione_Cod AND MP_finitexReport.Mat_Cod = Linee_Preparazioni_Dettagli.Mat_Cod  " & vbCrLf)


            '------------------------------------------------------ 
            '-------------------- WHERE ---------------------------
            '------------------------------------------------------
            StbSQL.Append(" WHERE Agenda.Lav_Cod = " & Agro_SQL_SaveNum(LAVCOD_TRASFORMAZIONI) & " " & vbCrLf)

            StbSQL.Append(" AND Mov_Carico.Cau_Mov = '" & Agro_SQL_SaveText(CAU_CARICO) & "' " & vbCrLf)

            StbSQL.Append(" AND Mov_Carico.Data_Movimento >= " & Agro_SQL_SaveDate(Data_Inizio) & " " & vbCrLf)
            StbSQL.Append(" AND Mov_Carico.Data_Movimento <= " & Agro_SQL_SaveDate(Data_Fine) & " " & vbCrLf)

            ''va eliminato il commento: è stato commentato per un bug del giaslan
            'StbSQL.Append(" AND MovimentixReport.Id_Report = " + Agro_SQL_SaveNum(enum_AgroReportistica.PreparazioniBio) + " " + vbCrLf)

            StbSQL.Append(" AND MP_finitexReport.Id_Report = " & Agro_SQL_SaveNum(enum_AgroReportistica.PreparazioniBio) & " " & vbCrLf)

            StbSQL.Append(" AND Linee_Preparazioni_Dettagli.ChkFinale = 1 " & vbCrLf)


            If Piva <> "" Then
                StbSQL.Append(" AND Agenda.Piva = '" & Agro_SQL_SaveText(Piva) & "' " & vbCrLf)
            End If

            If Linea_Cod <> 0 Then
                StbSQL.Append(" AND Agenda.Linea_Cod = " & Agro_SQL_SaveNum(Linea_Cod) & " " & vbCrLf)
            End If

            If Preparazione_Cod <> 0 Then
                StbSQL.Append(" AND Agenda.Preparazione_Cod = " & Agro_SQL_SaveNum(Preparazione_Cod) & " " & vbCrLf)
            End If

            If Id_Agenda <> 0 Then
                StbSQL.Append(" AND Agenda.Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & " " & vbCrLf)
            End If

            If Mat_Cod <> 0 Then
                StbSQL.Append(" AND Mov_Dett_Carico.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & " " & vbCrLf)
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StbSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                StbSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StbSQL.Append(" ORDER BY Mov_Carico.Data_Movimento, Mov_Carico.Id_Agenda ")
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, StbSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function


    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' lettura degli ingredienti di una preparazione
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Public Function PreparazioniBIO_Ingredienti_LineePreparazioni(ByVal Piva As String,
                                                                  ByVal Preparazione_Cod As Integer,
                                                                  ByVal xFiltroAggiuntivo As String,
                                                                  ByVal xOrderBy As String,
                                                                  ByRef objParametri As AgronicaCoreParametri
                                                                  ) As DataTable

        Const nomeRoutine = "AgronicaCoreStampeDAL.RegistriPreparazioni.PreparazioniBIO_Ingredienti_LineePreparazioni"

        Dim messaggioErrore As String = ""
        Dim StbSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try

            StbSQL.Length = 0

            '------------------------------------------------------ 
            '------------------- SELECT ---------------------------
            '------------------------------------------------------
            StbSQL.Append(" SELECT Linee_Preparazioni.Preparazione_Cod, Linee_Preparazioni.Preparazione_Des, Linee_Preparazioni.note, ")
            StbSQL.Append("         Linee_Preparazioni_Dettagli.*, UnitaMisura.Udm_Sim, UnitaMisura.Udm_Des, ")
            StbSQL.Append("         Materie_Prime.Mat_des, Materie_Prime.Regolamento ")

            '------------------------------------------------------ 
            '-------------------- FROM ----------------------------
            '------------------------------------------------------
            StbSQL.Append(" FROM Linee_Preparazioni ")

            StbSQL.Append(" INNER JOIN Linee_Preparazioni_Dettagli ")
            StbSQL.Append("             ON Linee_Preparazioni.Piva =  Linee_Preparazioni_Dettagli.Piva ")
            StbSQL.Append("             AND Linee_Preparazioni.Preparazione_Cod =  Linee_Preparazioni_Dettagli.Preparazione_Cod ")

            StbSQL.Append(" INNER JOIN Linee_PreparazionixReport ")
            StbSQL.Append("             ON Linee_Preparazioni.Piva =  Linee_PreparazionixReport.Piva  ")
            StbSQL.Append("             AND Linee_Preparazioni.Preparazione_Cod =  Linee_PreparazionixReport.Preparazione_Cod ")

            StbSQL.Append(" INNER JOIN Materie_Prime ON Materie_Prime.Elem_Cod = Linee_Preparazioni_Dettagli.Elem_Cod AND Materie_Prime.Mat_Cod = Linee_Preparazioni_Dettagli.Mat_Cod   " & vbCrLf)

            StbSQL.Append(" INNER JOIN UnitaMisura ON UnitaMisura.Udm_Cod = Linee_Preparazioni_Dettagli.Udm_Cod " & vbCrLf)

            '------------------------------------------------------ 
            '-------------------- WHERE ---------------------------
            '------------------------------------------------------
            StbSQL.Append(" WHERE Linee_Preparazioni.Piva = '" & Agro_SQL_SaveText(Piva) & "' " & vbCrLf)

            If Preparazione_Cod <> 0 Then
                StbSQL.Append(" AND Linee_Preparazioni.Preparazione_Cod = " & Agro_SQL_SaveNum(Preparazione_Cod) & " " & vbCrLf)
            End If

            'la materia prima deve essere scaricata, quindi un ingrediente
            StbSQL.Append(" AND Linee_Preparazioni_Dettagli.Cau_Mov = '" & Agro_SQL_SaveText(CAU_SCARICO) & "' " & vbCrLf)
            'la materia prima non deve essere il risultato della preparazione
            StbSQL.Append(" AND Linee_Preparazioni_Dettagli.ChkFinale = 0 " & vbCrLf)

            ' LA PREPARAZIONE DEVE ESSERE SINGOLA, NON DEVE ESSERE CONTENUTA IN UNA LINEA
            StbSQL.Append(" AND Linee_Preparazioni.Preparazione_Cod NOT IN (SELECT DISTINCT Preparazione_Cod FROM Linee_ProduzionixPreparazioni) " & vbCrLf)

            'IL DETTAGLIO DELLA PREPARAZIONE NON DEVE COMPARIRE
            StbSQL.Append(" AND NOT EXISTS (SELECT * FROM  Linee_Preparazioni_Report_Esclusi LPRE " & vbCrLf)
            StbSQL.Append("                 WHERE LPRE.Id_Report = Linee_PreparazionixReport.Id_Report  " & vbCrLf)
            StbSQL.Append("                 AND LPRE.Piva = Linee_Preparazioni_Dettagli.Piva AND LPRE.Preparazione_Cod = Linee_Preparazioni_Dettagli.Preparazione_Cod AND LPRE.Dettaglio_Cod = Linee_Preparazioni_Dettagli.Dettaglio_Cod ) " & vbCrLf)

            StbSQL.Append(" AND Linee_PreparazionixReport.Id_Report = " & Agro_SQL_SaveNum(enum_AgroReportistica.PreparazioniBio) & " " & vbCrLf)

            'If Mat_Cod <> 0 Then
            '    StbSQL.Append(" AND Linee_Preparazioni_Dettagli.Mat_Cod = " + Agro_SQL_SaveNum(Mat_Cod) + " " + vbCrLf)
            'End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StbSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------

            '################################
            If xOrderBy <> "" Then
                StbSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                'devo ordinare per le preparazioni (sequenza cronologica) e per descrizione ingrediente
                'in realtà c'è il filtro su una singola preparazione
                StbSQL.Append(" ORDER BY Linee_Preparazioni.Preparazione_Cod, Dettaglio_Des ")
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, StbSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' lettura degli ingredienti di una linea
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Public Function PreparazioniBIO_Ingredienti_LineeProduzioni(ByVal Piva As String,
                                                                ByVal Linea_Cod As Integer,
                                                                ByVal Mat_Cod As Integer,
                                                                ByVal xFiltroAggiuntivo As String,
                                                                ByVal xOrderBy As String,
                                                                ByRef objParametri As AgronicaCoreParametri
                                                                ) As DataTable

        Const nomeRoutine = "AgronicaCoreStampeDAL.RegistriPreparazioni.PreparazioniBIO_Ingredienti_LineeProduzioni"

        Dim messaggioErrore As String = ""
        Dim StbSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try

            StbSQL.Length = 0


            '------------------------------------------------------ 
            '------------------- SELECT ---------------------------
            '------------------------------------------------------
            StbSQL.Append(" SELECT Linee_Preparazioni.Preparazione_Cod, Linee_Preparazioni.Preparazione_Des, Linee_Preparazioni.note, ")
            StbSQL.Append("         Linee_Preparazioni_dettagli.*, UnitaMisura.Udm_Sim, UnitaMisura.Udm_Des,  ")
            StbSQL.Append("          Materie_Prime.Mat_des, Materie_Prime.Regolamento  ")

            '------------------------------------------------------ 
            '-------------------- FROM ----------------------------
            '------------------------------------------------------
            StbSQL.Append(" FROM Linee_Produzioni ")

            StbSQL.Append(" INNER JOIN Linee_ProduzionixPreparazioni ")
            StbSQL.Append("             ON Linee_Produzioni.Piva =  Linee_ProduzionixPreparazioni.Piva ")
            StbSQL.Append("             AND Linee_Produzioni.Linea_Cod =  Linee_ProduzionixPreparazioni.Linea_Cod ")

            StbSQL.Append(" INNER JOIN  Linee_Preparazioni ")
            StbSQL.Append("              ON Linee_Preparazioni.Piva =  Linee_ProduzionixPreparazioni.Piva ")
            StbSQL.Append("             AND Linee_Preparazioni.Preparazione_Cod =  Linee_ProduzionixPreparazioni.Preparazione_Cod ")

            StbSQL.Append(" INNER JOIN Linee_Preparazioni_Dettagli ")
            StbSQL.Append("             ON Linee_Preparazioni.Piva =  Linee_Preparazioni_Dettagli.Piva ")
            StbSQL.Append("             AND Linee_Preparazioni.Preparazione_Cod =  Linee_Preparazioni_Dettagli.Preparazione_Cod ")

            StbSQL.Append(" INNER JOIN Linee_PreparazionixReport ")
            StbSQL.Append("             ON Linee_Preparazioni.Piva =  Linee_PreparazionixReport.Piva  ")
            StbSQL.Append("             AND Linee_Preparazioni.Preparazione_Cod =  Linee_PreparazionixReport.Preparazione_Cod ")

            StbSQL.Append(" INNER JOIN Materie_Prime ON Materie_Prime.Elem_Cod = Linee_Preparazioni_Dettagli.Elem_Cod AND Materie_Prime.Mat_Cod = Linee_Preparazioni_Dettagli.Mat_Cod   " & vbCrLf)

            StbSQL.Append(" INNER JOIN UnitaMisura ON UnitaMisura.Udm_Cod = Linee_Preparazioni_Dettagli.Udm_Cod " & vbCrLf)


            '------------------------------------------------------ 
            '-------------------- WHERE ---------------------------
            '------------------------------------------------------
            StbSQL.Append(" WHERE Linee_Produzioni.Piva = '" & Agro_SQL_SaveText(Piva) & "' " & vbCrLf)

            'la materia prima deve essere scaricata, quindi un ingrediente
            StbSQL.Append(" AND Linee_Preparazioni_Dettagli.Cau_Mov = '" & Agro_SQL_SaveText(CAU_SCARICO) & "' " & vbCrLf)
            'nel caso delle linee, la materia prima non deve MAI essere caricata (cioè il risultato di una preparazione e l'ingresso di un'altra)
            StbSQL.Append(" AND NOT EXISTS (SELECT * FROM  Linee_Preparazioni_Dettagli LPD " & vbCrLf)
            StbSQL.Append("                 WHERE LPD.Preparazione_Cod = Linee_Preparazioni_Dettagli.Preparazione_Cod  " & vbCrLf)
            StbSQL.Append("                 AND LPD.Piva = Linee_Preparazioni_Dettagli.Piva " & vbCrLf)
            StbSQL.Append("                 AND LPD.Elem_Cod = Linee_Preparazioni_Dettagli.Elem_Cod " & vbCrLf)
            StbSQL.Append("                 AND LPD.Pro_Cod = Linee_Preparazioni_Dettagli.Pro_Cod " & vbCrLf)
            StbSQL.Append("                 AND LPD.Mat_Cod = Linee_Preparazioni_Dettagli.Mat_Cod " & vbCrLf)
            StbSQL.Append("                 AND LPD.Cau_Mov = '" & Agro_SQL_SaveText(CAU_CARICO) & "' ) " & vbCrLf)

            'la materia prima non deve essere il risultato della preparazione
            StbSQL.Append(" AND Linee_Preparazioni_Dettagli.ChkFinale = 0 " & vbCrLf)

            'IL DETTAGLIO DELLA PREPARAZIONE NON DEVE COMPARIRE
            StbSQL.Append(" AND NOT EXISTS (SELECT * FROM  Linee_Preparazioni_Report_Esclusi LPRE " & vbCrLf)
            StbSQL.Append("                 WHERE LPRE.Id_Report = Linee_PreparazionixReport.Id_Report  " & vbCrLf)
            StbSQL.Append("                 AND LPRE.Piva = Linee_Preparazioni_Dettagli.Piva AND LPRE.Preparazione_Cod = Linee_Preparazioni_Dettagli.Preparazione_Cod AND LPRE.Dettaglio_Cod = Linee_Preparazioni_Dettagli.Dettaglio_Cod ) " & vbCrLf)

            StbSQL.Append(" AND Linee_PreparazionixReport.Id_Report = " & Agro_SQL_SaveNum(enum_AgroReportistica.PreparazioniBio) & " " & vbCrLf)

            If Linea_Cod <> 0 Then
                StbSQL.Append(" AND Linee_Produzioni.Linea_Cod = " & Agro_SQL_SaveNum(Linea_Cod) & " " & vbCrLf)
            End If

            If Mat_Cod <> 0 Then
                StbSQL.Append(" AND Linee_Preparazioni_Dettagli.preparazione_cod IN (SELECT preparazione_cod FROM Linee_Preparazioni_Dettagli WHERE mat_cod = " & Mat_Cod & ") " & vbCrLf)
            End If

            'If Mat_Cod <> 0 Then
            '    StbSQL.Append(" AND Linee_Preparazioni_Dettagli.Mat_Cod = " + Agro_SQL_SaveNum(Mat_Cod) + " " + vbCrLf)
            'End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StbSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------

            '################################
            If xOrderBy <> "" Then
                StbSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                'livello è l'ordine cronologico delle preparazioni
                StbSQL.Append(" ORDER BY Linee_ProduzionixPreparazioni.Livello, Dettaglio_Des ")
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, StbSQL.ToString, nomeRoutine)
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
