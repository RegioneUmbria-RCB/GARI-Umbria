Imports System.Text
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate

Public Class PUA_Testata_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function Leggi(ByVal Regolamento_Cod As Integer,
                          ByVal PUA_Cod As Integer,
                          ByVal Piva As String,
                          ByVal Validita_Inizio As Date,
                          ByVal Validita_Fine As Date,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreParametri
                          ) As DataTable

        Const nomeRoutine = "AgronicaCorePUA_DAL.PUA_Testata_R.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = "" 
        '   PUA_Cod = 0    
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            '------------------------------------------------------------------

            strSql.Length = 0

            strSql.AppendLine(" SELECT * ")
            strSql.AppendLine(" FROM  PUA_Testata ")
            strSql.AppendLine(" WHERE Validita_inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
            strSql.AppendLine(" AND   Validita_Fine >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")

            If objParametri.PivaSuperUser <> "" Then
                strSql.AppendLine(" AND Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'   ")
            End If

            If Regolamento_Cod <> 0 Then
                strSql.AppendLine(" AND Regolamento_Cod=" & Regolamento_Cod & " ")
            End If

            If Piva <> "" Then
                strSql.AppendLine(" AND Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
            End If

            If PUA_Cod <> 0 Then
                strSql.AppendLine(" AND PUA_Cod = " & Agro_SQL_SaveNum(PUA_Cod) & "   ")
            End If

            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.AppendLine(" AND   Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.AppendLine(" AND   Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                strSql.AppendLine(" ORDER BY Piva_SuperUser, Piva, PUA_Cod Asc ")
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

    Public Function Leggi_xGriglia(ByVal Piva As String,
                                   ByVal PUA_Cod As Integer,
                                   ByVal Regolamento_Cod As Integer,
                                   ByVal Data_inizio As Date,
                                   ByVal Data_fine As Date,
                                   ByVal xFiltroAggiuntivo As String,
                                   ByVal xOrderBy As String,
                                   ByRef objParametri As AgronicaCoreParametri
                                   ) As DataTable


        Const nomeRoutine = "AgronicaCorePUA_DAL.PUA_Testata_R.Leggi_xGriglia()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New StringBuilder
        Dim dt As DataTable

        Try

            StrSQL.AppendLine("SELECT PT.PUA_Cod AS PC_Testata_Cod,'PUA ' + CONVERT(varchar(4),PT.PUA_Anno) AS PC_Testata_Des, PT.PUA_Tipo AS PC_Tipo, PT.validita_inizio,PT.Validita_Fine, PT.Regolamento_Cod,0 AS PC_Elaborazione_Cod, ")
            StrSQL.AppendLine(" PT.piva AS PC_Dettagli_PIVA, CASE WHEN ISNULL(IMP.partitaIvaReale, '') = '' THEN PT.piva ELSE IMP.partitaIvaReale END PivaReale, IMP.rag_soc, ")
            StrSQL.AppendLine(" PUA_Anno AS PC_Dettagli_Anno, 0 AS PC_Dettagli_ColturaPrincipale_Veg_Cod, '' AS Veg_Des, REG.Regolamento_Des, 0 as Allegati_Documenti_Cod,")

            StrSQL.AppendLine("  '' as Veg_Des,")

            StrSQL.AppendLine(" ISNULL((select COUNT (*) FROM  Ricette R WHERE R.Programmazione_Cod = PT.PUA_Cod ")
            StrSQL.AppendLine(" AND R.Ricetta_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.AppendLine(" AND R.Piva = PT.Piva ")
            StrSQL.AppendLine(" and R.Tipo_Ricetta = " & Agro_SQL_SaveNum(enum_TipoRicetta.PianoDistribuzionePua) & "),0) as nPianiDistr, ")

            StrSQL.AppendLine(enum_PUARegolamenti_Tipo.PUA & " AS Regolamento_Tipo ")

            StrSQL.Append(" , ISNULL(pt.blocco_flag,0) AS blocco_flag " & vbCrLf)

            StrSQL.Append(" , ISNULL(CA.sa_nome,'') AS sa_nome " & vbCrLf)

            StrSQL.Append(" , ISNULL(pt.note,0) AS note " & vbCrLf)
            StrSQL.Append(" , ISNULL(pt.Flag_NonUtilizzo_Fertilizzanti,0) AS Flag_NonUtilizzo_Fertilizzanti " & vbCrLf)

            StrSQL.AppendLine("  FROM  PUA_Testata PT  ")
            StrSQL.AppendLine("  inner join Imprese IMP on PT.PIVA = IMP.PIVA ")
            StrSQL.AppendLine("  left join PUA_Regolamenti REG on PT.Regolamento_Cod = REG.Regolamento_Cod  ")
            StrSQL.AppendLine("  left join Centri_Aziendali CA on PT.PIVA = CA.PIVA AND PT.Sa_Cod = CA.Sa_Cod  ")

            StrSQL.AppendLine(" WHERE PT.Validita_inizio <= " & Agro_SQL_SaveDate(Data_fine) & " ")
            StrSQL.AppendLine(" AND   PT.Validita_Fine >= " & Agro_SQL_SaveDate(Data_inizio) & " ")
            StrSQL.AppendLine(" AND PT.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

            If PUA_Cod <> 0 Then
                StrSQL.AppendLine(" and PUA_Cod=" & PUA_Cod & "")
            End If

            If Piva <> "" Then
                StrSQL.AppendLine(" AND PT.PIVA = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If

            If Regolamento_Cod <> 0 Then
                StrSQL.AppendLine(" AND PT.Regolamento_Cod = " & Agro_SQL_SaveNum(Regolamento_Cod) & " ")
            End If

            '(25/02/2020 fede) aggiunto filtro visibilita
            Dim objUtentiVisibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
            Dim FiltroCentri As String = ""
            Dim DtCentriVisibili As DataTable
            DtCentriVisibili = objUtentiVisibilita.Leggi(enum_TipoEntita.Centro, " Piva='" & Agro_SQL_SaveText(Piva) & "'", "", objParametri)
            If Not DtCentriVisibili Is Nothing AndAlso DtCentriVisibili.Rows.Count > 0 Then
                For i = 0 To DtCentriVisibili.Rows.Count - 1
                    FiltroCentri &= " (PT.Piva = '" & DtCentriVisibili.Rows(i).Item("piva") & "' AND PT.Sa_Cod = " & DtCentriVisibili.Rows(i).Item("sa_cod") & ") OR "
                Next
                If FiltroCentri <> "" Then
                    StrSQL.Append(" AND (" & Left(FiltroCentri, FiltroCentri.Length - 3) & " OR (PT.Piva = '" & Agro_SQL_SaveText(Piva) & "' AND PT.Sa_Cod = 0) ) ")
                End If
            End If


            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.AppendLine(" AND   PT.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.AppendLine(" AND   PT.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    Public Function Leggi_Tutte_Piva_Che_Hanno_PUA(ByRef objParametri As AgronicaCoreParametri) As DataTable

        Const nomeRoutine = "AgronicaCorePUA_DAL.PUA_Testata_R.Leggi_Tutte_Piva_Che_Hanno_PUA()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            '---------------------------------------------
            strSql.Length = 0
            strSql.AppendLine(" select distinct piva from PUA_Testata ")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '-------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    Public Function LeggiTestata_Da_GerarchiaImpresa(ByVal ImpresePadri As List(Of String),
                                                          ByVal Piva As String,
                                                          ByVal validita_Inizio As DateTime,
                                                          ByVal validita_fine As DateTime,
                                                                ByVal xFiltroAggiuntivo As String,
                                                             ByVal xOrderBy As String,
                                                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                             ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                             ByRef filtroUltima As Boolean) As DataTable

        Dim NomeRoutine As String = "AgronicaPUADAL.PUA_Testata_R.LeggiTestata_Da_GerarchiaImpresa()"

        Dim NomeDB_Utenti As String = objParametri_Utenti.StringaConnessione.Split(";")(2).Split("=")(1)


        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            'modificare la query di select
            StrSQL.Length = 0

            StrSQL.Append("SELECT * FROM " & vbCrLf)
            StrSQL.Append("( ")
            StrSQL.Append(creaQueryTestata(ImpresePadri, Piva, validita_Inizio, validita_fine, filtroUltima, xFiltroAggiuntivo, objParametri_Utenti))
            StrSQL.Append(") t " & vbCrLf)
            StrSQL.Append(" Group by t.cuaa, t.rag_soc, t.Piva, t.Pua_Cod,t.regolamento_cod, t.PUA_Tipo, t.stato, t.pro_cod, t.com_Des, t.ind_Des, t.Validita_inizio, t.Validita_Fine, t.Data_Creazione, t.compilatore " & vbCrLf)

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

    Private Function creaQueryTestata(ByVal ImpresePadri As List(Of String),
                                                          ByVal Piva As String,
                                                          ByVal validita_Inizio As DateTime,
                                                          ByVal validita_fine As DateTime,
                                                          ByVal filtroUltima As Boolean,
                                                          ByVal xFiltroAggiuntivo As String,
                                        ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri) As String

        Dim NomeDB_Utenti As String = objParametri_Utenti.StringaConnessione.Split(";")(2).Split("=")(1)

        Dim StrSQL As New System.Text.StringBuilder
        StrSQL.Length = 0
        StrSQL.Append("SELECT val_cod as cuaa, i.rag_soc,i.Piva, pt.Pua_Cod,pt.regolamento_cod, pt.PUA_Tipo, ind.stato, ind.pro_cod, ind.com_Des, ind.ind_Des,pt.Validita_inizio, pt.Validita_Fine, pt.Data_Creazione " & vbCrLf)

        StrSQL.AppendLine(" , ISNULL((Utenti_Dettagli.Cognome + ' ' + Utenti_Dettagli.Nome), '') AS Compilatore ")

        StrSQL.Append(" FROM Imprese I  " & vbCrLf)
        StrSQL.Append(" LEFT JOIN GerarchiaImprese gi ON I.PIVA = gi.Figlio  " & vbCrLf)
        StrSQL.Append(" LEFT JOIN Pua_Testata pt ON I.PIVA = pt.Piva  " & vbCrLf)
        If filtroUltima Then
            StrSQL.Append(" AND Pt.pua_Cod IN ( " & vbCrLf)
            StrSQL.Append("    SELECT Max(pua_Cod) FROM Pua_Testata " & vbCrLf)
            StrSQL.Append("    WHERE Piva = pt.piva " & vbCrLf)
            StrSQL.Append("    AND Validita_Inizio <=  " + Agro_SQL_SaveDateTime_NULL(validita_fine) + "   " & vbCrLf)
            StrSQL.Append("    AND Validita_Fine >=  " + Agro_SQL_SaveDateTime_NULL(validita_Inizio) + " " & vbCrLf)
            StrSQL.Append(" )")
        End If
        StrSQL.Append(" LEFT JOIN Imprese_Codici ic ON i.piva = ic.piva AND ic.id_Cod = 1010  " & vbCrLf)
        StrSQL.Append(" LEFT JOIN ImpreseXIndirizzi ii ON i.piva = ii.piva AND Tipo_Indirizzo = 1" & vbCrLf)
        StrSQL.Append(" LEFT JOIN Indirizzi ind ON ind.Cod_Indirizzo = ii.cod_indirizzo " & vbCrLf)
        StrSQL.Append(" LEFT JOIN " & NomeDB_Utenti & ".dbo.Utenti_Dettagli ON pt.username_creazione = Utenti_Dettagli.codfisc ")

        'StrSQL.Append(" inner JOIN PUA_Non_Importati ON PUA_Non_Importati.pua_cod = pt.pua_cod " & vbCrLf)

        StrSQL.Append(" WHERE pt.pua_Cod is not null " & vbCrLf)
        'StrSQL.Append(" And pua_cod Not in ( select distinct pua_cod from PUA_Elaborazione ) " & vbCrLf)

        If ImpresePadri.Count <> 0 Then
            If ImpresePadri.Count = 1 Then
                Dim impresa = ImpresePadri(0)
                If impresa <> "-1" Then
                    StrSQL.Append(" AND gi.Padre = " + Agro_SQL_SaveText_NULL(impresa) + " " & vbCrLf)
                Else
                    StrSQL.Append(" AND gi.Padre <> '' " & vbCrLf)
                End If
            Else
                StrSQL.Append(" AND gi.Padre IN ( ")
                Dim first = True
                For Each pPadre In ImpresePadri
                    If pPadre <> "" Then
                        If Not first Then
                            StrSQL.Append(", ")
                        Else
                            first = False
                        End If
                        StrSQL.Append(" " + Agro_SQL_SaveText_NULL(pPadre) + " ")
                    End If
                Next
                StrSQL.Append(" ) ")
            End If
        End If

        If Piva <> "" Then
            StrSQL.Append(" AND I.PIVA = " + Agro_SQL_SaveText_NULL(Piva) + " " & vbCrLf)
        End If
        If validita_Inizio <> AGRODATAINIZIO Then
            StrSQL.Append(" AND pt.Validita_Inizio >= " + Agro_SQL_SaveDateTime(validita_Inizio) + " " & vbCrLf)
        End If
        If validita_fine <> AGRODATAFINE Then
            StrSQL.Append(" AND pt.Validita_Fine <= " + Agro_SQL_SaveDateTime(validita_fine) + " ")
        End If

        If xFiltroAggiuntivo <> "" Then
            StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri_Utenti))
        End If

        Return StrSQL.ToString

    End Function



    Public Function Leggi_xStatistiche_PUA_Utenti(ByVal FiltroData_Operazione1_Registrazione2 As Integer,
                                                                        ByVal DataInizio As Date,
                                                                        ByVal DataFine As Date,
                                                                        ByVal Username As String,
                                                                        ByVal xFiltroAggiuntivo As String,
                                                                        ByVal xOrderBy As String,
                                                                        ByRef objParametri As AgronicaCoreParametri,
                                                                           ByRef objParametri_Utenti As AgronicaCoreParametri,
                                                                  Optional ByVal Applica_VisibilitaUtente As Boolean = False
                                                                        ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Agenda_R.Leggi_xStatistiche_PUA_Utenti()"

        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
        Dim dt As DataTable

        Dim NomeDB_Utenti As String = objParametri_Utenti.StringaConnessione.Split(";")(2).Split("=")(1)


        Try

            strSql.Length = 0

            strSql.Append(" select  distinct " & vbCrLf)

            strSql.Append(" p.piva, p.Username_Creazione, ud.nome + ' ' + ud.Cognome as utente " & vbCrLf)

            strSql.Append(" From PUA_Testata p " & vbCrLf)
            strSql.Append("     inner join " & NomeDB_Utenti & ".dbo.Utenti_Dettagli ud on ud.CodFisc=p.Username_Creazione ")

            If Applica_VisibilitaUtente = True Then
                strSql.Append("     INNER JOIN Utenti_Visibilita_Appoggio (NOLOCK) ON p.PIVA = Utenti_Visibilita_Appoggio.PIVA AND Entita_Cod = 1 and Utenti_Visibilita_Appoggio.username ='" & objParametri.UtenteUsername & "'")
            End If

            Select Case FiltroData_Operazione1_Registrazione2
                Case 1
                    strSql.Append("         where p.validita_inizio <= " & Agro_SQL_SaveDate(DataFine) & "   " & vbCrLf)
                    strSql.Append("         AND   p.validita_fine >= " & Agro_SQL_SaveDate(DataInizio) & "   " & vbCrLf)
                Case Else
                    strSql.Append("         where p.data_creazione <= " & Agro_SQL_SaveDate(DataFine) & "   " & vbCrLf)
                    strSql.Append("         AND   p.data_creazione >= " & Agro_SQL_SaveDate(DataInizio) & "   " & vbCrLf)
            End Select
            strSql.Append("         AND   p.username_creazione <> '" & objParametri.PivaSuperUser & "'")

            If Username <> "" Then
                strSql.Append("     AND   p.Username_Creazione='" & Agro_SQL_SaveText(Username) & "'")
            End If

            If xFiltroAggiuntivo <> "" Then
                strSql.Append("     AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                strSql.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                strSql.Append(" ORDER BY piva  ")
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



End Class


'#################################################################
'#################################################################
'#################################################################

Public Class PUA_Testata_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Scrivi(ByRef PUA_Cod As Integer,
                           ByVal Piva As String,
                           ByVal Sa_Cod As Integer,
                           ByVal PUA_Anno As Integer,
                           ByVal PUA_Tipo As Integer,
                           ByVal Sup_Lagoni As Decimal,
                           ByVal Coeff_Efficienza As Decimal,
                           ByVal Colture_Vernine As Integer,
                           ByVal Legge59 As Integer,
                           ByVal Note As String,
                           ByVal Regolamento_cod As Integer,
                           ByVal Tipo_Allevamento As Integer,
                           ByVal Perc_Zootecnico As Decimal,
                           ByVal Matrice_Prevalente As Integer,
                           ByVal Validita_Fine As DateTime,
                           ByVal Validita_Inizio As DateTime,
                           ByVal Flag_NonUtilizzo_Fertilizzanti As Integer,
                           ByRef objParametri As AgronicaCoreParametri,
                           Optional ByVal Data_creazione As DateTime = #2/1/1900#,
                           Optional ByVal Data_modifica As DateTime = #2/1/1900#,
                           Optional ByVal username_creazione As String = "",
                           Optional ByVal username_modifica As String = ""
                           ) As Boolean

        Const nomeRoutine = "PUA_Testata_W.Scrivi()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try



            If PUA_Cod = 0 Then
                Dim ObjSequenze As New AgronicaCoreDataProvider.Agro_Sequenze
                'PUA_Cod = ObjSequenze.Agronica_SequenzaTabelle_NuovoID("pua_testata", objParametri)
                'Lavez - 12/07/2024 - normalizzazione chiamate a stack counter
                PUA_Cod = ObjSequenze.NuovoId_Tabella("pua_testata", 0, AgronicaCoreDataProvider.CostantiPersonalizzate.UpperBoundTabelle_Per_SequenzaTabelle_Topcode, objParametri)
            End If


            If Data_creazione = #2/1/1900# Then
                Data_creazione = DateTime.Now
            End If

            If Data_modifica = #2/1/1900# Then
                Data_modifica = DateTime.Now
            End If

            If username_creazione = "" Then
                username_creazione = objParametri.UsernameOperazione
            End If

            If username_modifica = "" Then
                username_modifica = objParametri.UsernameOperazione
            End If



            '---------------------------------------------
            strSql.Length = 0
            strSql.AppendLine(" INSERT INTO PUA_Testata ")
            strSql.AppendLine("             (")

            strSql.AppendLine("  [Piva_SuperUser] ")
            strSql.AppendLine("  ,[Piva] ")
            strSql.AppendLine("  ,[Sa_Cod] ")
            strSql.AppendLine("  ,[PUA_Anno] ")
            strSql.AppendLine("  ,[PUA_Cod] ")
            strSql.AppendLine("  ,[PUA_Tipo] ")
            strSql.AppendLine("  ,[Sup_Lagoni] ")
            strSql.AppendLine("  ,[Coeff_Efficienza] ")
            strSql.AppendLine("  ,[Colture_Vernine] ")
            strSql.AppendLine("  ,[Legge59] ")
            strSql.AppendLine("  ,[Note] ")
            strSql.AppendLine("  ,[Regolamento_COD] ")
            strSql.AppendLine("  ,[Tipo_allevamento] ")
            strSql.AppendLine("  ,[Perc_Zootecnico] ")
            strSql.AppendLine("  ,[Matrice_Prevalente], ")
            strSql.AppendLine("              Inviato,  ")
            strSql.AppendLine("              Data_Creazione,     Data_Modifica, ")
            strSql.AppendLine("              UserName_Creazione, UserName_Modifica, ")
            strSql.AppendLine("              Validita_Inizio,    Validita_Fine ")
            strSql.AppendLine("  , Flag_NonUtilizzo_Fertilizzanti ")

            strSql.AppendLine("                ) ")

            strSql.AppendLine(" VALUES ( ")

            strSql.AppendLine("          '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'")

            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(Piva) & "'")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(PUA_Anno) & " ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(PUA_Cod) & " ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(PUA_Tipo) & " ")

            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Sup_Lagoni) & " ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Coeff_Efficienza) & " ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Colture_Vernine) & " ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Legge59) & " ")
            strSql.AppendLine("         , '" & Agro_SQL_SaveText(Note) & "' ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Regolamento_cod) & " ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Tipo_Allevamento) & " ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Perc_Zootecnico) & " ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Matrice_Prevalente) & " ")

            strSql.AppendLine("         , 0  ")

            strSql.AppendLine("			, " & Agro_SQL_SaveDateTime(Data_creazione) & "  ")
            strSql.AppendLine("			, " & Agro_SQL_SaveDateTime(Data_modifica) & "  ")
            strSql.AppendLine("			,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            strSql.AppendLine("			,'" & Agro_SQL_SaveText(username_modifica) & "' ")

            strSql.AppendLine("			, " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            strSql.AppendLine("			, " & Agro_SQL_SaveDate(Validita_Fine) & "  ")

            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Flag_NonUtilizzo_Fertilizzanti) & " ")

            strSql.AppendLine(") ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function

    Public Function Cancella(ByVal Piva As String,
                             ByVal PUA_Cod As Integer,
                             ByVal Regolamento_Cod As Integer,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreParametri
                             ) As Boolean

        Const nomeRoutine = "PUA_Testata_W.Cancella()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then

                strSql.Length = 0
                strSql.AppendLine(" UPDATE PUA_Testata ")
                strSql.AppendLine(" SET ")
                strSql.AppendLine("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                strSql.AppendLine("      ,Inviato = -1 ")
                strSql.AppendLine(" WHERE  Inviato >= 0")
                strSql.AppendLine(" AND   Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
                strSql.AppendLine("  AND   PUA_Cod = " & Agro_SQL_SaveNum(PUA_Cod) & " ")

            Else

                strSql.Length = 0
                strSql.AppendLine(" DELETE ")
                strSql.AppendLine(" FROM     PUA_Testata ")
                strSql.AppendLine(" WHERE   Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
                strSql.AppendLine("  AND   PUA_Cod = " & Agro_SQL_SaveNum(PUA_Cod) & " ")

            End If

            If Regolamento_Cod <> 0 Then
                strSql.AppendLine("  AND   Regolamento_Cod = " & Agro_SQL_SaveNum(Regolamento_Cod) & " ")
            End If

            If Piva <> "" Then
                strSql.AppendLine("  AND   Piva = " & Agro_SQL_SaveText_NULL(Piva) & " ")
            End If


            '---------------------------------------------

            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function

    Public Function Pua_Blocca(ByVal Piva As String,
                               ByVal data_inizio As DateTime,
                               ByVal data_fine As DateTime,
                               ByVal bloccaSoloSeNonGiaBloccati As Boolean,
                               ByVal xFiltroAggiuntivo As String,
                               ByRef objParametri As AgronicaCoreParametri
                               ) As Boolean

        Dim nomeRoutine As String = "AgronicaCorePuaDAL.Pua_Testata_W.Pua_Blocca()"

        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        '------------------------------

        Try

            If Piva = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
            End If

            '---------------------------------------------
            strSql.Length = 0

            strSql.AppendLine(" UPDATE Pua_Testata SET ")
            strSql.AppendLine("     Blocco_Flag         =  1 ")
            strSql.AppendLine("    ,Blocco_Username     = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            strSql.AppendLine("    ,Blocco_Data         =  " & Agro_SQL_SaveDate(Now))

            strSql.AppendLine(" WHERE PIVA = '" & Agro_SQL_SaveText(Trim(Piva)) & "' ")

            strSql.AppendLine(" AND validita_inizio >= " & Agro_SQL_SaveDate(data_inizio))
            strSql.AppendLine(" AND validita_inizio <= " & Agro_SQL_SaveDate(data_fine))

            If bloccaSoloSeNonGiaBloccati = True Then
                strSql.AppendLine(" AND Blocco_Flag = 0 ")
            End If

            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function


    Public Function Pua_Sblocca(ByVal Piva As String,
                                   ByVal data_inizio As DateTime,
                                   ByVal data_fine As DateTime,
                                   ByVal xFiltroAggiuntivo As String,
                                   ByRef objParametri As AgronicaCoreParametri
                                   ) As Boolean

        Dim nomeRoutine As String = "AgronicaCorePuaDAL.Pua_Testata_W.Pua_Sblocca()"

        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If Piva = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
            End If

            '---------------------------------------------
            strSql.Length = 0

            strSql.AppendLine(" UPDATE Pua_Testata SET ")
            strSql.AppendLine("     Blocco_Flag         =  0 ")
            strSql.AppendLine("    ,Blocco_Username     = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            strSql.AppendLine("    ,Blocco_Data         =  " & Agro_SQL_SaveDate(Now))

            strSql.AppendLine(" WHERE PIVA = '" & Agro_SQL_SaveText(Trim(Piva)) & "' ")

            strSql.AppendLine(" AND validita_inizio >= " & Agro_SQL_SaveDate(data_inizio))
            strSql.AppendLine(" AND validita_inizio <= " & Agro_SQL_SaveDate(data_fine))

            strSql.AppendLine(" AND Blocco_Flag = 1 ")

            '----------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                strSql.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function

    Public Function BloccaSblocca(ByVal PUA_Cod As Integer, ByVal Regolamento_Cod As Integer, Blocco_Flag As Integer, Blocco_Data As Date, Blocco_Username As String, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri)

        Dim nomeRoutine As String = "AgronicaCorePuaDAL.Pua_Testata_W.Pua_Blocca()"

        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        '------------------------------

        Try

            If PUA_Cod = 0 Then
                Throw New Exception("PUA_COD = 0")
            End If

            If Regolamento_Cod = 0 Then
                Throw New Exception("Regolamento_Cod = 0")
            End If

            '---------------------------------------------
            strSql.Length = 0

            strSql.AppendLine(" UPDATE Pua_Testata SET ")
            strSql.AppendLine("     Blocco_Flag         =  " & Agro_SQL_SaveNum(Blocco_Flag) & " ")
            strSql.AppendLine("    ,Blocco_Username     = '" & Agro_SQL_SaveText(Blocco_Username) & "' ")
            strSql.AppendLine("    ,Blocco_Data         =  " & Agro_SQL_SaveDate(Blocco_Data))
            strSql.AppendLine("    ,Data_Modifica     =  " & Agro_SQL_SaveDateTime(Date.Now))
            strSql.AppendLine("    ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            strSql.AppendLine(" WHERE PUA_COD = " & Agro_SQL_SaveNum(PUA_Cod) & " ")
            strSql.AppendLine(" AND Regolamento_Cod = " & Agro_SQL_SaveNum(Regolamento_Cod) & " ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function

    Public Function Aggiorna_Note_e_Flag_NonUtilizzo_Fertilizzanti(ByVal Piva As String,
                              ByVal PUA_Cod As Integer, ByVal Regolamento_Cod As Integer,
                              ByVal Note As String, ByVal Flag_NonUtilizzo_Fertilizzanti As Integer,
                               ByVal xFiltroAggiuntivo As String,
                               ByRef objParametri As AgronicaCoreParametri
                               ) As Boolean

        Dim nomeRoutine As String = "AgronicaCorePuaDAL.Pua_Testata_W.Aggiorna_Note_e_Flag_NonUtilizzo_Fertilizzanti()"

        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        '------------------------------

        Try

            If Piva = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
            End If

            '---------------------------------------------
            strSql.Length = 0

            strSql.AppendLine(" UPDATE Pua_Testata SET ")
            strSql.AppendLine("    Note        = '" & Agro_SQL_SaveText(Note) & "' ")
            strSql.AppendLine("   ,Flag_NonUtilizzo_Fertilizzanti        = " & Agro_SQL_SaveNum(Flag_NonUtilizzo_Fertilizzanti) & " ")
            strSql.AppendLine("   ,Data_Modifica     =  " & Agro_SQL_SaveDate(Date.Now))
            strSql.AppendLine("   ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")

            strSql.AppendLine(" WHERE PIVA = '" & Agro_SQL_SaveText(Trim(Piva)) & "' ")
            strSql.AppendLine(" AND   Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            strSql.AppendLine(" AND   PUA_Cod = " & Agro_SQL_SaveNum(PUA_Cod) & " ")
            strSql.AppendLine(" AND   Regolamento_Cod = " & Agro_SQL_SaveNum(Regolamento_Cod) & " ")


            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function


End Class
