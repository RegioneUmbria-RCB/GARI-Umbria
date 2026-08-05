Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi


Public Class Linee_Produzioni_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function LeggiLineeProduzione(ByVal Piva As String,
                                         ByVal xFiltroAggiuntivo As String,
                                         ByVal xOrderBy As String,
                                         ByRef objParametri As AgronicaCoreParametri
                                         ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Linee_Produzioni_R.LeggiLineeProduzione"

        Dim messaggioErrore As String = ""
        Dim StbSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try

            StbSQL.Length = 0

            StbSQL.Append(" SELECT * from Linee_Produzioni ")
            StbSQL.Append(" WHERE Piva = '" & Agro_SQL_SaveText(Piva) & "' " & vbCrLf)

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StbSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                StbSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
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
    ''' Union tra le linee e le preparazioni
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Public Function LineeProduzioni_UNION_LineePreparazioni_By_MatCod_IdReport(ByVal Piva As String, _
                                                            ByVal Id_Report As Integer, _
                                                            ByVal Mat_Cod As Integer, _
                                                            ByVal xFiltroAggiuntivo As String, _
                                                            ByVal xOrderBy As String, _
                                                            ByRef objParametri As AgronicaCoreParametri _
                                                                ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Linee_Produzioni_R.LineeProduzioni_UNION_LineePreparazioni_By_MatCod_IdReport"

        Dim messaggioErrore As String = ""
        Dim StbSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try

            StbSQL.Length = 0

            StbSQL.Append(" ( " & vbCrLf)
            StbSQL.Append(" SELECT Linee_Preparazioni.Piva, 0 AS Linea_Cod, Linee_Preparazioni.preparazione_cod, Linee_Preparazioni.Preparazione_Des AS Descrizione, Linee_Preparazioni.note " & vbCrLf)


            StbSQL.Append(" FROM Linee_Preparazioni " & vbCrLf)

            StbSQL.Append(" INNER JOIN Linee_Preparazioni_Dettagli " & vbCrLf)
            StbSQL.Append("             ON Linee_Preparazioni.Piva =  Linee_Preparazioni_Dettagli.Piva " & vbCrLf)
            StbSQL.Append("             AND Linee_Preparazioni.Preparazione_Cod =  Linee_Preparazioni_Dettagli.Preparazione_Cod " & vbCrLf)
            StbSQL.Append(" INNER JOIN Linee_PreparazionixReport " & vbCrLf)
            StbSQL.Append("             ON Linee_Preparazioni.Piva =  Linee_PreparazionixReport.Piva  " & vbCrLf)
            StbSQL.Append("             AND Linee_Preparazioni.Preparazione_Cod =  Linee_PreparazionixReport.Preparazione_Cod " & vbCrLf)


            StbSQL.Append(" WHERE Linee_Preparazioni.Piva = '" & Agro_SQL_SaveText(Piva) & "' " & vbCrLf)

            'la materia prima deve essere caricata, quindi un prodotto risultante, non un ingrediente
            StbSQL.Append(" AND Linee_Preparazioni_Dettagli.Cau_Mov = '" & Agro_SQL_SaveText(CAU_CARICO) & "' " & vbCrLf)
            'la materia prima deve essere il risultato della preparazione
            StbSQL.Append(" AND Linee_Preparazioni_Dettagli.ChkFinale =1 " & vbCrLf)

            ' LA PREPARAZIONE DEVE ESSERE SINGOLA, NON DEVE ESSERE CONTENUTA IN UNA LINEA
            StbSQL.Append(" AND Linee_Preparazioni.Preparazione_Cod not in (select Preparazione_Cod from [Linee_ProduzionixPreparazioni]) " & vbCrLf)

            'IL DETTAGLIO DELLA PREPARAZIONE NON DEVE COMPARIRE
            StbSQL.Append(" AND NOT EXISTS (SELECT * FROM  Linee_Preparazioni_Report_Esclusi LPRE " & vbCrLf)
            StbSQL.Append("                 WHERE LPRE.Id_Report = Linee_PreparazionixReport.Id_Report  " & vbCrLf)
            StbSQL.Append("                 AND LPRE.Piva = Linee_Preparazioni_Dettagli.Piva AND LPRE.Preparazione_Cod = Linee_Preparazioni_Dettagli.Preparazione_Cod AND LPRE.Dettaglio_Cod = Linee_Preparazioni_Dettagli.Dettaglio_Cod ) " & vbCrLf)

            If Id_Report <> 0 Then
                StbSQL.Append(" AND Linee_PreparazionixReport.Id_Report = " & Agro_SQL_SaveNum(Id_Report) & " " & vbCrLf)
            End If

            If Mat_Cod <> 0 Then
                StbSQL.Append(" AND Linee_Preparazioni_Dettagli.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & " " & vbCrLf)
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StbSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri) & vbCrLf)
            End If
            '--------------------------------------------------------------------------


            '################################
            StbSQL.Append(" ) " & vbCrLf)
            StbSQL.Append(" UNION ALL " & vbCrLf)
            StbSQL.Append(" ( " & vbCrLf)
            '################################

            StbSQL.Append(" SELECT Linee_Produzioni.Piva, Linee_Produzioni.Linea_Cod, 0 AS preparazione_cod, Linee_Produzioni.Linea_Des AS Descrizione, Linee_Preparazioni.note " & vbCrLf)

            '------------------------------------------------------ 
            '-------------------- FROM ----------------------------
            '------------------------------------------------------
            StbSQL.Append(" FROM Linee_Produzioni " & vbCrLf)

            StbSQL.Append(" INNER JOIN Linee_ProduzionixPreparazioni " & vbCrLf)
            StbSQL.Append("             ON Linee_Produzioni.Piva =  Linee_ProduzionixPreparazioni.Piva " & vbCrLf)
            StbSQL.Append("             AND Linee_Produzioni.Linea_Cod =  Linee_ProduzionixPreparazioni.Linea_Cod " & vbCrLf)

            StbSQL.Append(" INNER JOIN  Linee_Preparazioni " & vbCrLf)
            StbSQL.Append("              ON Linee_Preparazioni.Piva =  Linee_ProduzionixPreparazioni.Piva " & vbCrLf)
            StbSQL.Append("             AND Linee_Preparazioni.Preparazione_Cod =  Linee_ProduzionixPreparazioni.Preparazione_Cod " & vbCrLf)

            StbSQL.Append(" INNER JOIN Linee_Preparazioni_Dettagli " & vbCrLf)
            StbSQL.Append("             ON Linee_Preparazioni.Piva =  Linee_Preparazioni_Dettagli.Piva " & vbCrLf)
            StbSQL.Append("             AND Linee_Preparazioni.Preparazione_Cod =  Linee_Preparazioni_Dettagli.Preparazione_Cod " & vbCrLf)

            StbSQL.Append(" INNER JOIN Linee_PreparazionixReport " & vbCrLf)
            StbSQL.Append("             ON Linee_Preparazioni.Piva =  Linee_PreparazionixReport.Piva  " & vbCrLf)
            StbSQL.Append("             AND Linee_Preparazioni.Preparazione_Cod =  Linee_PreparazionixReport.Preparazione_Cod " & vbCrLf)

            '------------------------------------------------------ 
            '-------------------- WHERE ---------------------------
            '------------------------------------------------------
            StbSQL.Append(" WHERE Linee_Produzioni.Piva = '" & Agro_SQL_SaveText(Piva) & "' " & vbCrLf)

            'la materia prima deve essere caricata, quindi un prodotto risultante, non un ingrediente
            StbSQL.Append(" AND Linee_Preparazioni_Dettagli.Cau_Mov = '" & Agro_SQL_SaveText(CAU_CARICO) & "' " & vbCrLf)
            'la materia prima deve essere il risultato della preparazione
            StbSQL.Append(" AND Linee_Preparazioni_Dettagli.ChkFinale =1 " & vbCrLf)

            'IL DETTAGLIO DELLA PREPARAZIONE NON DEVE COMPARIRE
            StbSQL.Append(" AND NOT EXISTS (SELECT * FROM  Linee_Preparazioni_Report_Esclusi LPRE " & vbCrLf)
            StbSQL.Append("                 WHERE LPRE.Id_Report = Linee_PreparazionixReport.Id_Report  " & vbCrLf)
            StbSQL.Append("                 AND LPRE.Piva = Linee_Preparazioni_Dettagli.Piva AND LPRE.Preparazione_Cod = Linee_Preparazioni_Dettagli.Preparazione_Cod AND LPRE.Dettaglio_Cod = Linee_Preparazioni_Dettagli.Dettaglio_Cod ) " & vbCrLf)

            'DEVO ESCLUDERE LE ANAGRAFICHE COLLEGATE ALLA LINEA MA DISATTIVATE
            StbSQL.Append("  AND NOT EXISTS ( " & vbCrLf)
            StbSQL.Append("                 SELECT  1 " & vbCrLf)
            StbSQL.Append("                 FROM OGenerazioni_Anagrafe_Log " & vbCrLf)
            StbSQL.Append("                 WHERE OGenerazioni_Anagrafe_Log.mat_cod=Linee_Preparazioni_Dettagli.Mat_Cod " & vbCrLf)
            StbSQL.Append("                 and OGenerazioni_Anagrafe_Log.linea_cod=Linee_ProduzionixPreparazioni.linea_cod " & vbCrLf)
            StbSQL.Append("                 and ChkScollegamento=1 " & vbCrLf)
            StbSQL.Append("                 ) " & vbCrLf)

            If Id_Report <> 0 Then
                StbSQL.Append(" AND Linee_PreparazionixReport.Id_Report = " & Agro_SQL_SaveNum(Id_Report) & " " & vbCrLf)
            End If

            If Mat_Cod <> 0 Then
                StbSQL.Append(" AND Linee_Preparazioni_Dettagli.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & " " & vbCrLf)
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StbSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri) & vbCrLf)
            End If
            '--------------------------------------------------------------------------
            StbSQL.Append(" ) " & vbCrLf)
            StbSQL.Append("  " & vbCrLf)


            '################################
            If xOrderBy <> "" Then
                StbSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StbSQL.Append(" ORDER BY Descrizione ")
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



    '######################################################################################
    Public Function LineeProduzioni(ByVal Piva As String,
                                    ByVal Cau_Mov As String,
                                    ByVal Id_Report As Integer,
                                    ByVal Mat_Cod As Integer,
                                    ByVal xFiltroAggiuntivo As String,
                                    ByVal xOrderBy As String,
                                    ByRef objParametri As AgronicaCoreParametri
                                    ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Linee_Produzioni_R.LineeProduzioni"

        Dim messaggioErrore As String = ""
        Dim StbSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try

            StbSQL.Length = 0

            StbSQL.Append(" SELECT DISTINCT Linee_Produzioni.*, Linea_Classe_Des ")


            StbSQL.Append(" FROM Linee_Produzioni ")

            StbSQL.Append(" INNER JOIN Linee_Classi_Produzioni ")
            StbSQL.Append("             ON Linee_Produzioni.Piva =  Linee_Classi_Produzioni.Piva ")
            StbSQL.Append("             AND Linee_Produzioni.Linea_Classe_Cod =  Linee_Classi_Produzioni.Linea_Classe_Cod ")

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


            StbSQL.Append(" WHERE Linee_Produzioni.Piva = '" & Agro_SQL_SaveText(Piva) & "' " & vbCrLf)

            If Cau_Mov <> "" Then
                StbSQL.Append(" AND Linee_Preparazioni_Dettagli.Cau_Mov = '" & Agro_SQL_SaveText(Cau_Mov) & "' " & vbCrLf)
            End If

            'IL DETTAGLIO DELLA PREPARAZIONE NON DEVE COMPARIRE
            StbSQL.Append(" AND NOT EXISTS (SELECT 1 FROM  Linee_Preparazioni_Report_Esclusi LPRE " & vbCrLf)
            StbSQL.Append("                 WHERE LPRE.Id_Report = Linee_PreparazionixReport.Id_Report  " & vbCrLf)
            StbSQL.Append("                 AND LPRE.Piva = Linee_Preparazioni_Dettagli.Piva AND LPRE.Preparazione_Cod = Linee_Preparazioni_Dettagli.Preparazione_Cod AND LPRE.Dettaglio_Cod = Linee_Preparazioni_Dettagli.Dettaglio_Cod ) " & vbCrLf)

            If Id_Report <> 0 Then
                StbSQL.Append(" AND Linee_PreparazionixReport.Id_Report = " & Agro_SQL_SaveNum(Id_Report) & " " & vbCrLf)
            End If

            If Mat_Cod <> 0 Then
                StbSQL.Append(" AND Linee_Preparazioni_Dettagli.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & " " & vbCrLf)
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StbSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                StbSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StbSQL.Append(" ORDER BY Linea_Classe_Des, Linea_Des, Denominazione ")
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


    '######################################################################################
    Public Function MateriePrime_byLineeProduzioni(ByVal Piva As String,
                                                   ByVal Cau_Mov As String,
                                                   ByVal Id_Report As Integer,
                                                   ByVal Linea_Cod As Integer,
                                                   ByVal xFiltroAggiuntivo As String,
                                                   ByVal xOrderBy As String,
                                                   ByRef objParametri As AgronicaCoreParametri
                                                   ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Linee_Produzioni_R.MateriePrime_byLineeProduzioni"

        Dim messaggioErrore As String = ""
        Dim StbSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try

            StbSQL.Length = 0

            StbSQL.Append(" SELECT Linee_Produzioni.*, Materie_Prime.mat_cod, Mat_des, cod_articolo ")


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

            StbSQL.Append(" INNER JOIN Materie_Prime ")
            StbSQL.Append("             ON Materie_Prime.mat_Cod =  Linee_Preparazioni_Dettagli.mat_Cod ")


            StbSQL.Append(" WHERE Linee_Produzioni.Piva = '" & Agro_SQL_SaveText(Piva) & "' " & vbCrLf)

            If Cau_Mov <> "" Then
                StbSQL.Append(" AND Linee_Preparazioni_Dettagli.Cau_Mov = '" & Agro_SQL_SaveText(Cau_Mov) & "' " & vbCrLf)
            End If

            'IL DETTAGLIO DELLA PREPARAZIONE NON DEVE COMPARIRE
            StbSQL.Append(" AND NOT EXISTS (SELECT 1 FROM  Linee_Preparazioni_Report_Esclusi LPRE " & vbCrLf)
            StbSQL.Append("                 WHERE LPRE.Id_Report = Linee_PreparazionixReport.Id_Report  " & vbCrLf)
            StbSQL.Append("                 AND LPRE.Piva = Linee_Preparazioni_Dettagli.Piva AND LPRE.Preparazione_Cod = Linee_Preparazioni_Dettagli.Preparazione_Cod AND LPRE.Dettaglio_Cod = Linee_Preparazioni_Dettagli.Dettaglio_Cod ) " & vbCrLf)

            If Id_Report <> 0 Then
                StbSQL.Append(" AND Linee_PreparazionixReport.Id_Report = " & Agro_SQL_SaveNum(Id_Report) & " " & vbCrLf)
            End If

            If Linea_Cod <> 0 Then
                StbSQL.Append(" AND Linee_Produzioni.Linea_Cod = " & Agro_SQL_SaveNum(Linea_Cod) & " " & vbCrLf)
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StbSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                StbSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StbSQL.Append(" ORDER BY Linea_Des ")
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
    ''' Join tra Linee_Produzioni e la tabella contatti tramite Cod_contatto_Terzi
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Public Function LineeProduzioni_ContattiTerzi(ByVal Piva As String,
                                                  ByVal Piva_Contatto As String,
                                                  ByVal Cod_Contatto As String,
                                                  ByVal Linea_Cod As Integer,
                                                  ByVal xFiltroAggiuntivo As String,
                                                  ByVal xOrderBy As String,
                                                  ByRef objParametri As AgronicaCoreParametri
                                                  ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Linee_Produzioni_R.LineeProduzioni_ContattiTerzi"

        Dim messaggioErrore As String = ""
        Dim StbSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try

            StbSQL.Length = 0

            StbSQL.Append(" SELECT Linee_Produzioni.Piva,Linea_Cod,Linea_Cod_Des,Linea_Des,Linea_Classe_Cod,Linee_Produzioni.Validita_Inizio,Linee_Produzioni.Validita_Fine,Cod_Contatto_Terzi,   ")
            StbSQL.Append(" Contatti.Piva AS Piva_contatto, Cod_contatto, Rag_Soc, Nome, Cognome ")

            StbSQL.Append(" FROM Linee_Produzioni ")
            StbSQL.Append(" INNER JOIN Contatti ")
            StbSQL.Append("         ON Linee_Produzioni.Cod_contatto_Terzi =  Contatti.Cod_contatto ")

            StbSQL.Append(" WHERE Linee_Produzioni.Piva = '" & Agro_SQL_SaveText(Piva) & "' " & vbCrLf)

            If Linea_Cod <> 0 Then
                StbSQL.Append(" AND Linee_Produzioni.Linea_Cod = " & Agro_SQL_SaveNum(Linea_Cod) & " " & vbCrLf)
            End If

            If Piva_Contatto <> "" Then
                StbSQL.Append(" AND Contatti.Piva = '" & Agro_SQL_SaveText(Piva_Contatto) & "' " & vbCrLf)
            End If

            If Cod_Contatto <> "" Then
                StbSQL.Append(" AND Contatti.Cod_Contatto = '" & Agro_SQL_SaveText(Cod_Contatto) & "' " & vbCrLf)
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StbSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                StbSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StbSQL.Append(" ORDER BY Rag_Soc, Nome, Cognome ")
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
    ''' Join tra Linee_Produzioni e risorse umane tramite Cod_contatto_Terzi
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Public Function DistinctRisUmLineeProd(ByVal Piva As String,
                                           ByVal Cod_Contatto_terzi As String,
                                           ByVal Linea_Cod As Integer,
                                           ByVal xFiltroAggiuntivo As String,
                                           ByVal xOrderBy As String,
                                           ByRef objParametri As AgronicaCoreParametri
                                           ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Linee_Produzioni_R.DistinctRisUmLineeProd"

        Dim messaggioErrore As String = ""
        Dim StbSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try

            StbSQL.Length = 0

            StbSQL.Append(" SELECT DISTINCT Contatti.Piva AS Piva_contatto, Contatti.Cod_contatto, Rag_Soc, Nome, Cognome, Cod_Risum ")


            StbSQL.Append(" FROM Linee_Produzioni ")

            StbSQL.Append(" INNER JOIN Contatti ")
            StbSQL.Append("         ON Linee_Produzioni.Cod_contatto_Terzi =  Contatti.Cod_contatto ")

            StbSQL.Append(" INNER JOIN Risorse_Umane ")
            StbSQL.Append("         ON Risorse_Umane.Cod_contatto =  Contatti.Cod_contatto AND Risorse_Umane.piva =  Contatti.piva  ")


            StbSQL.Append(" WHERE Linee_Produzioni.Piva = '" & Agro_SQL_SaveText(Piva) & "' " & vbCrLf)

            If Linea_Cod <> 0 Then
                StbSQL.Append(" AND Linee_Produzioni.Linea_Cod = " & Agro_SQL_SaveNum(Linea_Cod) & " " & vbCrLf)
            End If

            If Cod_Contatto_terzi <> "" Then
                StbSQL.Append(" AND Linee_Produzioni.Cod_Contatto_terzi = '" & Agro_SQL_SaveText(Cod_Contatto_terzi) & "' " & vbCrLf)
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StbSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                StbSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StbSQL.Append(" ORDER BY Rag_Soc, Nome, Cognome ")
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

    '##############################################################################################
    Public Function Lista_CodRisUm_ByCodContattoTerzi(ByVal Piva_Linea As String,
                                                      ByVal Cod_Contatto_terzi As String,
                                                      ByVal xFiltroAggiuntivo As String,
                                                      ByRef objParametri As AgronicaCoreParametri
                                                      ) As String

        Const nomeRoutine = "AgronicaCoreContabDAL.Linee_Produzioni_R.Lista_CodRisUm_ByCodContattoTerzi()"

        Dim messaggioErrore As String = ""
        Dim dt As DataTable
        Dim filtro As String = ""
        Dim i As Integer

        Try

            dt = DistinctRisUmLineeProd(Piva_Linea, Cod_Contatto_terzi, 0,
                                        xFiltroAggiuntivo, "", objParametri)

            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                For i = 0 To dt.Rows.Count - 1
                    filtro &= " " & CStr(dt.Rows(i).Item("Cod_RisUm")) & ","
                Next
                'tolgo l'ultima la virgola
                filtro = Left(filtro, filtro.Length - 1)
            End If

            dt = Nothing

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return filtro

    End Function


    Public Function LineeProduzioni_TipoDefault(ByVal Piva As String,
                                                ByVal Tipo_Default As Integer,
                                                ByVal xFiltroAggiuntivo As String,
                                                ByVal xOrderBy As String,
                                                ByRef objParametri As AgronicaCoreParametri
                                                ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Linee_Produzioni_R.LineeProduzioni_TipoDefault"

        Dim messaggioErrore As String = ""
        Dim StbSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try

            StbSQL.Length = 0

            StbSQL.Append(" SELECT DISTINCT Linee_Produzioni.* ")

            StbSQL.Append(" FROM Linee_Preparazioni ")
            StbSQL.Append(" INNER JOIN Linee_ProduzionixPreparazioni ON Linee_Preparazioni.Preparazione_Cod = Linee_ProduzionixPreparazioni.Preparazione_Cod ")
            StbSQL.Append(" AND Linee_Preparazioni.Piva = Linee_ProduzionixPreparazioni.Piva ")
            StbSQL.Append(" INNER JOIN Linee_Produzioni ON Linee_ProduzionixPreparazioni.Linea_Cod= Linee_Produzioni.Linea_Cod ")
            StbSQL.Append(" AND Linee_ProduzionixPreparazioni.Piva= Linee_Produzioni.Piva ")


            StbSQL.Append(" WHERE Linee_Preparazioni.Piva = '" & Agro_SQL_SaveText(Piva) & "' " & vbCrLf)


            If Tipo_Default <> 0 Then
                StbSQL.Append(" AND Linee_Preparazioni.Tipo_Default = " & Agro_SQL_SaveNum(Tipo_Default) & vbCrLf)
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StbSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StbSQL.Append(" AND   Linee_Preparazioni.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StbSQL.Append(" AND   Linee_Preparazioni.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StbSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StbSQL.Append(" ORDER BY Linea_Des ")
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

    Public Function Leggi_LineeProduzioni_Con_LineeProduzioni_Classi(ByVal Piva As String,
                                                                     ByVal Linea_Classe_Cod As Integer,
                                                                     ByVal xFiltroAggiuntivo As String,
                                                                     ByRef objParametri As AgronicaCoreParametri
                                                                     ) As DataTable


        Const nomeRoutine = "AgronicaCoreContabDAL.Linee_Produzioni_R.Leggi_LineeProduzioni_Con_LineeProduzioni_Classi"

        Dim messaggioErrore As String = ""
        Dim StbSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try

            StbSQL.Length = 0
            StbSQL.AppendLine(" SELECT DISTINCT Linee_Classi_Produzioni.Linea_Classe_Cod, Linee_Classi_Produzioni.Linea_Classe_Des, Linee_Produzioni.Reg_Cod,")
            StbSQL.AppendLine(" Linee_Produzioni.Linea_Cod, Linee_Produzioni.Piva,Linee_Produzioni.Linea_Cod_Des,Linee_Produzioni.Linea_Des, Linee_Produzioni_Mix.Veg_cod,Linee_Produzioni_Mix.Cul_Cod")
            StbSQL.AppendLine(" FROM   Linee_Classi_Produzioni,Linee_Produzioni_Mix, Linee_Produzioni ")
            StbSQL.AppendLine(" LEFT OUTER JOIN Contatti ON (Linee_Produzioni.Cod_Contatto_Terzi = Contatti.Cod_Contatto)  ")
            StbSQL.AppendLine(" WHERE  Linee_Produzioni.Validita_inizio <= " & Agro_SQL_SaveDateTime(AGRODATAFINE) & vbCrLf)
            StbSQL.AppendLine(" AND    Linee_Produzioni.Validita_Fine >= " & Agro_SQL_SaveDateTime(AGRODATAINIZIO) & vbCrLf)
            StbSQL.AppendLine(" AND    Linee_Produzioni.Piva = Linee_Classi_Produzioni.Piva")
            StbSQL.AppendLine(" AND    Linee_Produzioni.Linea_Classe_Cod = Linee_Classi_Produzioni.Linea_Classe_Cod")
            StbSQL.AppendLine(" AND    Linee_Produzioni.Linea_Cod = Linee_Produzioni_Mix.Linea_Cod")

            If Trim(Piva) <> "" Then
                StbSQL.AppendLine("  AND Linee_Produzioni.Piva  = '" & Agro_SQL_SaveText(Piva) & "' " & vbCrLf)
            End If

            If Linea_Classe_Cod <> 0 Then
                'Cecalupo: linea_classe_cod => identifica una classe generica
                StbSQL.AppendLine(" AND Linee_Produzioni.Linea_Classe_Cod IN (0, " & Agro_SQL_SaveNum(Linea_Classe_Cod) & ")" & vbCrLf)
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StbSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            StbSQL.AppendLine(" ORDER BY Linee_Produzioni.Piva, Linee_Produzioni.Linea_Des ASC ")
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


'##############################################################################
'##############################################################################
'##############################################################################
'##############################################################################


Public Class Linee_Produzioni_W
    Inherits AgronicaCoreDataProvider.DataProvider


    Public Function Scrivi(ByVal Piva As String _
                        , ByVal Linea_Cod As Integer _
                        , ByVal Linea_Cod_Des As String _
                        , ByVal Linea_Des As String _
                        , ByVal Linea_Classe_Cod As Integer _
                        , ByVal Colore As Integer _
                        , ByVal ChkVisualizzazione_Risorse As String _
                        , ByVal Resa As String _
                        , ByVal Linea_Classe_Cod_Preparazione As Integer _
                        , ByVal Colore_Default As Integer _
                        , ByVal Veg_Cod As Integer _
                        , ByVal Cul_Cod As Integer _
                        , ByVal ChkFine_Automatica As String _
                        , ByVal Modulo_Generazione As Integer _
                        , ByVal Denominazione As String _
                        , ByVal Tipo_Denominazione As String _
                        , ByVal Tipo_Lotto_Identificativo As String _
                        , ByVal Categoria_Gias_Cod As Integer _
                        , ByVal Classificazione_Gias_Cod As Integer _
                        , ByVal Linea_DPI_Cod As Integer _
                        , ByVal Linea_Modello_Cod As Integer _
                        , ByVal ChkParametri_Automatici As String _
                        , ByVal Reg_Cod As Integer _
                        , ByVal Grfi_Cod As Integer _
                        , ByVal Filtro_Invisibili As String _
                        , ByVal Cod_Contatto_Terzi As String _
                        , ByVal Dicitura_Gias_Cod As Integer _
                        , ByVal ChkAggiornamento As String _
                        , ByVal Deno_Gias_Cod As Integer _
                        , ByVal OFiltro_Denominazione As String _
                        , ByVal OFiltro_Colore As String _
                        , ByVal OFiltro_Categoria As String _
                        , ByVal OFiltro_Classificazione As String _
                        , ByVal OFiltro_Regolamento As String _
                        , ByVal OFiltro_Finalita As String _
                        , ByVal OFiltro_Dicitura As String _
                        , ByVal ChkControllo_Manuale As String _
                        , ByVal Validita_Inizio As Date _
                        , ByVal Validita_Fine As Date _
                        , ByRef objParametri As AgronicaCoreParametri _
                            , Optional ByVal Data_creazione As DateTime = #2/1/1900# _
                            , Optional ByVal Data_modifica As DateTime = #2/1/1900# _
                            , Optional ByVal username_creazione As String = "" _
                            , Optional ByVal username_modifica As String = "" _
                           ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.Linee_Produzioni_W.Scrivi"

        Dim messaggioErrore As String = ""
        Dim strSql As New Text.StringBuilder
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



            strSql.Append(" INSERT INTO Linee_Produzioni ")
            strSql.Append(" ( ")
            strSql.Append("  ,[Piva] " & vbCrLf)
            strSql.Append("  ,[Linea_Cod] " & vbCrLf)
            strSql.Append("  ,[Linea_Cod_Des] " & vbCrLf)
            strSql.Append("  ,[Linea_Des] " & vbCrLf)
            strSql.Append("  ,[Linea_Classe_Cod] " & vbCrLf)
            strSql.Append("  ,[Colore] " & vbCrLf)
            strSql.Append("  ,[ChkVisualizzazione_Risorse] " & vbCrLf)
            strSql.Append("  ,[Resa] " & vbCrLf)
            strSql.Append("  ,[Linea_Classe_Cod_Preparazione] " & vbCrLf)
            strSql.Append("  ,[Colore_Default] " & vbCrLf)
            strSql.Append("  ,[Veg_Cod] " & vbCrLf)
            strSql.Append("  ,[Cul_Cod] " & vbCrLf)
            strSql.Append("  ,[ChkFine_Automatica] " & vbCrLf)
            strSql.Append("  ,[Modulo_Generazione] " & vbCrLf)
            strSql.Append("  ,[Denominazione] " & vbCrLf)
            strSql.Append("  ,[Tipo_Denominazione] " & vbCrLf)
            strSql.Append("  ,[Tipo_Lotto_Identificativo] " & vbCrLf)
            strSql.Append("  ,[Categoria_Gias_Cod] " & vbCrLf)
            strSql.Append("  ,[Classificazione_Gias_Cod] " & vbCrLf)
            strSql.Append("  ,[Linea_DPI_Cod] " & vbCrLf)
            strSql.Append("  ,[Linea_Modello_Cod] " & vbCrLf)
            strSql.Append("  ,[ChkParametri_Automatici] " & vbCrLf)
            strSql.Append("  ,[Reg_Cod] " & vbCrLf)
            strSql.Append("  ,[Grfi_Cod] " & vbCrLf)
            strSql.Append("  ,[Filtro_Invisibili] " & vbCrLf)
            strSql.Append("  ,[Cod_Contatto_Terzi] " & vbCrLf)
            strSql.Append("  ,[Dicitura_Gias_Cod] " & vbCrLf)
            strSql.Append("  ,[ChkAggiornamento] " & vbCrLf)
            strSql.Append("  ,[Deno_Gias_Cod] " & vbCrLf)
            strSql.Append("  ,[OFiltro_Denominazione] " & vbCrLf)
            strSql.Append("  ,[OFiltro_Colore] " & vbCrLf)
            strSql.Append("  ,[OFiltro_Categoria] " & vbCrLf)
            strSql.Append("  ,[OFiltro_Classificazione] " & vbCrLf)
            strSql.Append("  ,[OFiltro_Regolamento] " & vbCrLf)
            strSql.Append("  ,[OFiltro_Finalita] " & vbCrLf)
            strSql.Append("  ,[OFiltro_Dicitura] " & vbCrLf)
            strSql.Append("  ,[ChkControllo_Manuale], " & vbCrLf)


            strSql.Append("                    Inviato,            DataInvio, " & vbCrLf)
            strSql.Append("                    Data_Creazione,     Data_Modifica, " & vbCrLf)
            strSql.Append("                    UserName_Creazione, UserName_Modifica, " & vbCrLf)
            strSql.Append("                    Validita_Inizio,    Validita_Fine " & vbCrLf)
            strSql.Append(" ) VALUES (" & vbCrLf)

            strSql.Append(" '" & Agro_SQL_SaveText(Piva) & "'" & vbCrLf)
            strSql.Append(", " & Agro_SQL_SaveNum(Linea_Cod) & " " & vbCrLf)
            strSql.Append(",'" & Agro_SQL_SaveText(Linea_Cod_Des) & "'" & vbCrLf)
            strSql.Append(",'" & Agro_SQL_SaveText(Linea_Des) & "'" & vbCrLf)
            strSql.Append(", " & Agro_SQL_SaveNum(Linea_Classe_Cod) & " " & vbCrLf)
            strSql.Append(", " & Agro_SQL_SaveNum(Colore) & " " & vbCrLf)
            strSql.Append(",'" & Agro_SQL_SaveText(ChkVisualizzazione_Risorse) & "'" & vbCrLf)
            strSql.Append(",'" & Agro_SQL_SaveText(Resa) & "'" & vbCrLf)
            strSql.Append(", " & Agro_SQL_SaveNum(Linea_Classe_Cod_Preparazione) & " " & vbCrLf)
            strSql.Append(", " & Agro_SQL_SaveNum(Colore_Default) & " " & vbCrLf)
            strSql.Append(", " & Agro_SQL_SaveNum(Veg_Cod) & " " & vbCrLf)
            strSql.Append(", " & Agro_SQL_SaveNum(Cul_Cod) & " " & vbCrLf)
            strSql.Append(",'" & Agro_SQL_SaveText(ChkFine_Automatica) & "'" & vbCrLf)
            strSql.Append(", " & Agro_SQL_SaveNum(Modulo_Generazione) & " " & vbCrLf)
            strSql.Append(",'" & Agro_SQL_SaveText(Denominazione) & "'" & vbCrLf)
            strSql.Append(",'" & Agro_SQL_SaveText(Tipo_Denominazione) & "'" & vbCrLf)
            strSql.Append(",'" & Agro_SQL_SaveText(Tipo_Lotto_Identificativo) & "'" & vbCrLf)
            strSql.Append(", " & Agro_SQL_SaveNum(Categoria_Gias_Cod) & " " & vbCrLf)
            strSql.Append(", " & Agro_SQL_SaveNum(Classificazione_Gias_Cod) & " " & vbCrLf)
            strSql.Append(", " & Agro_SQL_SaveNum(Linea_DPI_Cod) & " " & vbCrLf)
            strSql.Append(", " & Agro_SQL_SaveNum(Linea_Modello_Cod) & " " & vbCrLf)
            strSql.Append(",'" & Agro_SQL_SaveText(ChkParametri_Automatici) & "'" & vbCrLf)
            strSql.Append(", " & Agro_SQL_SaveNum(Reg_Cod) & " " & vbCrLf)
            strSql.Append(", " & Agro_SQL_SaveNum(Grfi_Cod) & " " & vbCrLf)
            strSql.Append(",'" & Agro_SQL_SaveText(Filtro_Invisibili) & "'" & vbCrLf)
            strSql.Append(",'" & Agro_SQL_SaveText(Cod_Contatto_Terzi) & "'" & vbCrLf)
            strSql.Append(", " & Agro_SQL_SaveNum(Dicitura_Gias_Cod) & " " & vbCrLf)
            strSql.Append(",'" & Agro_SQL_SaveText(ChkAggiornamento) & "'" & vbCrLf)
            strSql.Append(", " & Agro_SQL_SaveNum(Deno_Gias_Cod) & " " & vbCrLf)
            strSql.Append(",'" & Agro_SQL_SaveText(OFiltro_Denominazione) & "'" & vbCrLf)
            strSql.Append(",'" & Agro_SQL_SaveText(OFiltro_Colore) & "'" & vbCrLf)
            strSql.Append(",'" & Agro_SQL_SaveText(OFiltro_Categoria) & "'" & vbCrLf)
            strSql.Append(",'" & Agro_SQL_SaveText(OFiltro_Classificazione) & "'" & vbCrLf)
            strSql.Append(",'" & Agro_SQL_SaveText(OFiltro_Regolamento) & "'" & vbCrLf)
            strSql.Append(",'" & Agro_SQL_SaveText(OFiltro_Finalita) & "'" & vbCrLf)
            strSql.Append(",'" & Agro_SQL_SaveText(OFiltro_Dicitura) & "'" & vbCrLf)
            strSql.Append(",'" & Agro_SQL_SaveText(ChkControllo_Manuale) & "'" & vbCrLf)


            strSql.Append(" , 0 " & vbCrLf)
            strSql.Append("		, NULL ")
            strSql.Append("			, " & Agro_SQL_SaveDateTime(Data_creazione) & "  " & vbCrLf)
            strSql.Append("			, " & Agro_SQL_SaveDateTime(Data_modifica) & "  " & vbCrLf)
            strSql.Append("			,'" & Agro_SQL_SaveText(username_creazione) & "' " & vbCrLf)
            strSql.Append("			,'" & Agro_SQL_SaveText(username_modifica) & "' " & vbCrLf)
            strSql.Append("			, " & Agro_SQL_SaveDate(Validita_Inizio) & " " & vbCrLf)
            strSql.Append("			, " & Agro_SQL_SaveDate(Validita_Fine) & " " & vbCrLf)
            strSql.Append(" ) ")

            xRisp = EseguiQuery_Scrittura(objParametri, strSql.ToString, nomeRoutine)


        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function


    Public Function AggiornaCodContattoTerzi_DefaultxRegistri(ByVal Piva As String,
                                                              ByVal xFiltroAggiuntivo As String,
                                                              ByRef objParametri As AgronicaCoreParametri
                                                              ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.Linee_Produzioni_W.AggiornaCodContattoTerzi_DefaultxRegistri"

        Dim messaggioErrore As String = ""
        Dim StbSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            StbSQL.Length = 0

            StbSQL.Append(" UPDATE Linee_Produzioni  ")
            StbSQL.Append(" SET ")
            StbSQL.Append("    Cod_Contatto_Terzi = '" & Agro_SQL_SaveText(Piva) & "'  ")

            StbSQL.Append("     ,Inviato           = 0 ")
            StbSQL.Append("     ,DataInvio         = Null ")
            StbSQL.Append("     ,Data_Modifica     =  " & Agro_SQL_SaveDate(Date.Now) & "  ")
            StbSQL.Append("     ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'  ")

            StbSQL.Append(" WHERE Linee_Produzioni.Piva = '" & Agro_SQL_SaveText(Piva) & "' " & vbCrLf)

            StbSQL.Append(" AND ( Linee_Produzioni.Cod_Contatto_Terzi = '' OR Linee_Produzioni.Cod_Contatto_Terzi = '0' ) " & vbCrLf)


            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StbSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StbSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function

End Class
