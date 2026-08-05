Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.DataProviderExtensions
Imports AgronicaCoreDataProvider

Public Class Utenti_Visibilita_Appoggio_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Sub New()
        MyBase.New()
    End Sub
    '##############################################################################################
    Public Function Leggi(ByVal Entita_Cod As Int32,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                Optional username As String = ""
                                ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   Username_Utente
        '   Id_Servizio
        '   FiltroAggiuntivo
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.AppendLine("SELECT *")
            StrSQL.AppendLine("FROM  Utenti_Visibilita_Appoggio")
            StrSQL.AppendLine("WITH (NOLOCK) ")
            StrSQL.AppendLine("WHERE PivaSuperUser='" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'")
            StrSQL.AppendLine($"    AND Username = '{If(Not String.IsNullOrEmpty(username), username, Agro_SQL_SaveText(objParametri.UtenteUsername))}' ")

            If Entita_Cod <> 0 Then
                StrSQL.AppendLine(" AND (Entita_Cod = " & Agro_SQL_SaveNum(Entita_Cod) & ") ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, True, objParametri) & " ")
            End If

            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
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

    '##############################################################################################
    Public Function Leggi_Massiva(ByVal Entita_Cod As Int32,
                                ByVal listaPiva As List(Of String),
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R.Leggi_Massiva()"

        '====================================================================================
        'Parametri opzionali :
        '   Username_Utente
        '   Id_Servizio
        '   FiltroAggiuntivo
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            If listaPiva Is Nothing OrElse listaPiva.Count = 0 Then
                Throw New Exception("La lista di PIVA è vuota.")
            End If

            ConnessioniTransazioni.ApriConnessione(True, objParametri)

            TempChiaviMassivo.CreaTabellaTemp_FiltroPiva(listaPiva, NomeRoutine, objParametri)

            StrSQL.Length = 0
            StrSQL.AppendLine(" SELECT * ")
            StrSQL.AppendLine(" FROM  Utenti_Visibilita_Appoggio ")
            StrSQL.AppendLine(" WITH (NOLOCK) ")
            StrSQL.AppendLine(" INNER JOIN #TempPiva tmp ON tmp.Piva = Utenti_Visibilita_Appoggio.Piva COLLATE DATABASE_DEFAULT ")
            StrSQL.AppendLine(" WHERE PivaSuperUser='" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'")
            StrSQL.AppendLine(" AND Username = '" & Agro_SQL_SaveText(objParametri.UtenteUsername) & "' ")

            If Entita_Cod <> 0 Then
                StrSQL.AppendLine(" AND (Entita_Cod = " & Agro_SQL_SaveNum(Entita_Cod) & ") ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, True, objParametri) & " ")
            End If

            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            ' Eliminazione tabella temporanea
            TempChiaviMassivo.EliminaTabellaTemp_FiltroPiva(NomeRoutine, objParametri)

            'commit transazione
            ConnessioniTransazioni.ChiudiTransazione(1, objParametri)

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            'rollback transazione
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        Finally
            ConnessioniTransazioni.ChiudiConnessione(objParametri)
        End Try

        Return DT

    End Function

    '##############################################################################################
    Public Function Leggi(ByVal Username As String,
                            ByVal Entita_Cod As Int32,
                            ByVal xFiltroAggiuntivo As String,
                            ByVal xOrderBy As String,
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   Username_Utente
        '   Id_Servizio
        '   FiltroAggiuntivo
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.AppendLine(" SELECT * ")
            StrSQL.AppendLine(" FROM  Utenti_Visibilita_Appoggio ")
            StrSQL.AppendLine(" WITH (NOLOCK) ")
            StrSQL.AppendLine(" WHERE PivaSuperUser='" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'")
            StrSQL.AppendLine(" AND Username = '" & Agro_SQL_SaveText(Username) & "' ")

            If Entita_Cod <> 0 Then
                StrSQL.AppendLine(" AND (Entita_Cod = " & Agro_SQL_SaveNum(Entita_Cod) & ") ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, True, objParametri) & " ")
            End If

            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
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

    ''' <summary>
    ''' Legge la tabella Utenti_Visibilita_Appoggio in join con le imprese
    ''' </summary>
    Public Function LeggiJoinImprese(xFiltroAggiuntivo As String, xOrderBy As String, ByRef objParametri As AgronicaCoreParametri) As DataTable
        Return LeggiJoinImprese(objParametri, "", xFiltroAggiuntivo, xOrderBy)
    End Function

    ''' <summary>
    ''' Legge la tabella Utenti_Visibilita_Appoggio in join con le imprese
    ''' </summary>
    Public Function LeggiJoinImprese(
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
        Optional username As String = "",
        Optional xFiltroAggiuntivo As String = "",
        Optional xOrderBy As String = ""
    ) As DataTable
        Dim NomeRoutine As String = "AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R.LeggiJoinImprese()"
        Dim StrSQL As New Text.StringBuilder
        Try
            StrSQL.AppendLine(" SELECT va.Username, va.Entita_Cod, ")
            StrSQL.AppendLine("   Imprese_Codici.val_cod as cuaa, i.PIVA, i.rag_soc, i.partitaIvaReale, ")
            StrSQL.AppendLine("   ISNULL(ca.sa_cod, 0) as sa_cod, ISNULL(ca.sa_nome, '') as sa_nome ")
            StrSQL.AppendLine(" FROM  Utenti_Visibilita_Appoggio va (NOLOCK)")
            StrSQL.AppendLine(" LEFT JOIN Imprese i                 (NOLOCK) ON i.PIVA = va.Piva ")
            StrSQL.AppendLine(" LEFT JOIN Centri_Aziendali ca       (NOLOCK) ON ca.PIVA = va.Piva AND ca.sa_cod = va.Sa_Cod ")
            StrSQL.AppendLine(" LEFT JOIN Imprese_Codici            (NOLOCK) ON Imprese_Codici.PIVA = i.piva and id_cod = 1010 ")
            StrSQL.AppendLine(" WHERE 1=1 ")

            If username <> "" Then
                StrSQL.AppendLine(" AND va.Username = '" & Agro_SQL_SaveText(username) & "' ")
            End If
            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, True, objParametri) & " ")
            End If
            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            return EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
        Catch ex As Exception
            Scrivi_LOG(objParametri, NomeRoutine, ex.Message)
            Throw New Exception("[" & NomeRoutine & "] : " & ex.Message)
        End Try
    End Function

    Public Function LeggiVisibilitaArea(ByVal userName As String,
                                        ByVal gruppo As Integer,
                                        ByVal area As Integer,
                                        ByVal visibilitaCompleta As Integer,
                                        ByVal xFiltroAggiuntivo As String,
                                        ByVal xOrderBy As String,
                                        ByRef objParametri_U As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                        ByRef objParametri_S As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                        Optional ByVal gestisciVisibilitaCompleta As Boolean = False) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R.leggiVisibilitaArea()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Dim selectStr As String = IIf(visibilitaCompleta = 0,
                                      " gp.Gruppi_Utente_cod As Gruppo_Cod, gp.Gruppi_Utente_des As Gruppo, Area As Area_Cod, UserName, CASE WHEN Area = '" + CStr(enum_Area_Visibilita.UMA) + "'  THEN 'UMA' ELSE 'TEST' END As Area, Piva_Azienda ",
                                      " DISTINCT Piva_Azienda, gp.Gruppi_Utente_cod As Gruppo_Cod, gp.Gruppi_Utente_des As Gruppo, UserName, Area As Area_Cod, '" + enum_Area_Visibilita.UMA.ToString + "' As Area "
                                      )

        Dim leftJoinImprese = ""
        If gestisciVisibilitaCompleta Then
            leftJoinImprese = " LEFT "
        End If

        Try

            StrSQL.Length = 0
            StrSQL.AppendLine(" SELECT " + selectStr + ", case when uv.Visibilita_Completa = 1 then '[Visibilità Completa]' else i.rag_soc end rag_soc, ic.val_cod as CUAA ")
            StrSQL.AppendLine(" FROM  Utenti_Visibilita uv ")
            StrSQL.AppendLine(" WITH (NOLOCK) ")
            StrSQL.AppendLine(" LEFT JOIN Gruppi_Utente gp ON gp.Gruppi_Utente_cod = uv.Gruppo ")
            StrSQL.AppendLine(leftJoinImprese + " JOIN " & objParametri_S.Recupera_NomeDB & ".dbo.Imprese_Codici ic On ic.PIVA = uv.Piva_Azienda and ic.id_cod = 1010")
            StrSQL.AppendLine(leftJoinImprese + " JOIN " & objParametri_S.Recupera_NomeDB & ".dbo.Imprese i On i.PIVA = uv.Piva_Azienda ")

            If (visibilitaCompleta = 0) Then

                StrSQL.AppendLine(" WHERE Area = " + area.ToString + " ")
                StrSQL.AppendLine(" AND UserName like '" + Agro_SQL_SaveText(userName) + "' ")
                StrSQL.AppendLine(" AND Gruppo = " + gruppo.ToString + " ")

            End If


            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, True, objParametri_U) & " ")
            End If

            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri_U, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_U, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try


        Return DT

    End Function

    Public Function CheckPiveMultiple(ByVal Piva As String,
                                        ByVal visibilitaCompleta As Integer,
                                        ByVal xFiltroAggiuntivo As String,
                                        ByVal xOrderBy As String,
                                        ByRef objParametri_U As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                        ByRef objParametri_S As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R.CheckPiveMultiple()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Dim selectStr As String = IIf(visibilitaCompleta = 0, " gp.Gruppi_Utente_cod As Gruppo_Cod, gp.Gruppi_Utente_des As Gruppo, Area As Area_Cod, UserName, CASE WHEN Area = '" + CStr(enum_Area_Visibilita.UMA) + "'  THEN 'UMA' ELSE 'TEST' END As Area, Piva_Azienda ",
                                        " DISTINCT Piva_Azienda, gp.Gruppi_Utente_cod As Gruppo_Cod, gp.Gruppi_Utente_des As Gruppo, UserName, Area As Area_Cod, '" + enum_Area_Visibilita.UMA.ToString + "' As Area ")

        Try

            StrSQL.Length = 0
            StrSQL.AppendLine(" SELECT " + selectStr + ", i.rag_soc ")
            StrSQL.AppendLine(" FROM  Utenti_Visibilita uv ")
            StrSQL.AppendLine(" WITH (NOLOCK) ")
            StrSQL.AppendLine(" LEFT JOIN Gruppi_Utente gp ON gp.Gruppi_Utente_cod = uv.Gruppo ")
            StrSQL.AppendLine(" JOIN " & objParametri_S.Recupera_NomeDB & ".dbo.Imprese i On i.PIVA = uv.Piva_Azienda ")
            StrSQL.AppendLine(" WHERE uv.Piva_Azienda =  '" + Agro_SQL_SaveText(Piva) + "' ")


            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, True, objParametri_U) & " ")
            End If

            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri_U, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_U, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try


        Return DT

    End Function

    Public Function OttieniPIVEVisibilita(ByVal userName As String,
                                          ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                          ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim result As String = ""
        Dim objUtenti_Visibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
        Dim objGruppi_Utente As New AgronicaCoreUtentiDAL.Utenti_xGruppi_Utente_R

        Dim gruppo As Integer = 0

        Dim checkDt As DataTable = CheckVisibilitaUtenteGruppoArea(userName, gruppo, 0, "", "", objParametri_Utenti)

        Dim visibilitaCompleta As Boolean = False

        Dim dt As New DataTable

        If checkDt.Rows.Count <= 0 Then

            Dim dtGruppi = objGruppi_Utente.Leggi_IdentificativoGruppoUtenti(userName, objParametri_Utenti)

            If dtGruppi.Rows.Count > 0 Then

                gruppo = dtGruppi.Rows.Item(0).Item("Gruppi_Utente_cod")
                userName = String.Empty

                checkDt = CheckVisibilitaUtenteGruppoArea(userName, gruppo, 0, "", "", objParametri_Utenti)

            End If

        End If

        If checkDt.Rows.Count > 0 Then

            If (checkDt.Rows.Item(0).Item("Visibilita_Completa")) Then
                visibilitaCompleta = True
            End If

            dt = LeggiVisibilitaArea(userName, gruppo, 0, visibilitaCompleta, "", "", objParametri_Utenti, objParametri_Server)

        End If

        Return dt

    End Function

    Public Function LeggiAree(ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R.leggiAree()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.AppendLine(" SELECT DISTINCT Area As area_Cod, CASE WHEN Area = '" + CStr(enum_Area_Visibilita.UMA) + "'  THEN 'UMA' ELSE 'TEST' END As area_Desc ")
            StrSQL.AppendLine(" FROM  Utenti_Visibilita ")
            StrSQL.AppendLine(" WITH (NOLOCK) ")
            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" WHERE " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, True, objParametri) & " ")
            End If

            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy))
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

    Public Function CheckVisibilitaUtenteGruppoArea(ByVal userName As String,
                                                    ByVal gruppo As Integer,
                                                    ByVal area As Integer,
                                                    ByVal xFiltroAggiuntivo As String,
                                                    ByVal xOrderBy As String,
                                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R.leggiVisibilitaArea()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.AppendLine(" SELECT * ")
            StrSQL.AppendLine(" FROM  Utenti_Visibilita ")
            StrSQL.AppendLine(" WITH (NOLOCK) ")
            StrSQL.AppendLine(" WHERE Area = " + area.ToString + " ")

            If gruppo <> 0 Then
                StrSQL.AppendLine(" AND Gruppo = " + gruppo.ToString + " ")
            End If

            StrSQL.AppendLine(" AND UserName like '" + Agro_SQL_SaveText(userName) + "' ")

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, True, objParametri) & " ")
            End If

            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy))
            Else
                StrSQL.AppendLine(" ORDER BY Visibilita_Completa DESC ")
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

    Public Sub OttieniTipoFiltroVisibilita(ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                ByVal Area As Integer,
                                                ByRef FiltroUtente As Boolean,
                                                ByRef FiltroGruppo As Boolean,
                                                ByRef Gruppo As Integer,
                                                ByRef VisibilitaTotale As Boolean)

        Dim dt = CheckVisibilitaUtenteGruppoArea(objParametri_Utenti.UtenteUsername, 0, 0, "", "", objParametri_Utenti)
        Dim objGruppi_Utente As New AgronicaCoreUtentiDAL.Utenti_xGruppi_Utente_R

        If dt.Rows.Count = 0 Then
            FiltroUtente = False

            Dim dtGruppi = objGruppi_Utente.Leggi_IdentificativoGruppoUtenti(objParametri_Utenti.UtenteUsername, objParametri_Utenti)
            If dtGruppi.Rows.Count > 0 Then
                Gruppo = dtGruppi.Rows.Item(0).Item("Gruppi_Utente_cod")

                Dim checkDt = CheckVisibilitaUtenteGruppoArea("", Gruppo, 0, "", "", objParametri_Utenti)

                If checkDt.Rows.Count > 0 Then
                    FiltroGruppo = True

                    If (checkDt.Rows(0)("Visibilita_Completa") = 1) Then
                        VisibilitaTotale = True
                    Else
                        VisibilitaTotale = False
                    End If

                Else
                    FiltroGruppo = False
                End If
            Else
                FiltroGruppo = False
            End If

        Else
            FiltroUtente = True

            If (dt.Rows(0)("Visibilita_Completa") = 1) Then
                VisibilitaTotale = True
            Else
                VisibilitaTotale = False
            End If

        End If

    End Sub

    Public Function CreaDTFiltroneImprese(
                                         ByRef PivaSuperUser As String,
                                         ByRef utente_Username As String,
                                         ByRef Enita_Cod As Integer,
                                         ByRef sqlQuery As String,
                                         ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R.CreaDTFiltroneImprese()"

        '====================================================================================
        'Parametri opzionali :
        '   Username_Utente
        '   Id_Servizio
        '   FiltroAggiuntivo
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.AppendLine(" SELECT '" & Agro_SQL_SaveText(PivaSuperUser) & "' as PivaSuperUser ")
            StrSQL.AppendLine("       ,'" & Agro_SQL_SaveText(utente_Username) & "' as Username ")
            StrSQL.AppendLine("       ," & Agro_SQL_SaveNum(Enita_Cod) & " as Entita_Cod ")
            StrSQL.AppendLine("       ,q.Piva as Piva ")
            StrSQL.AppendLine("       ,0 as Sa_Cod ")
            StrSQL.AppendLine("       ,0 as Appezza ")
            StrSQL.AppendLine("       ,0 as Id_Reg ")
            StrSQL.AppendLine(" FROM  (" & sqlQuery & ") q ")

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

    Public Function CreaDTFiltroneCentri(ByRef PivaSuperUser As String,
                                         ByRef utente_Username As String,
                                         ByRef Enita_Cod As Integer,
                                         ByRef sqlQuery As String,
                                         ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable
        Dim NomeRoutine As String = "AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R.CreaDTFiltroneCentri()"

        '====================================================================================
        'Parametri opzionali :
        '   Username_Utente
        '   Id_Servizio
        '   FiltroAggiuntivo
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.AppendLine(" SELECT '" & Agro_SQL_SaveText(PivaSuperUser) & "' as PivaSuperUser ")
            StrSQL.AppendLine("       ,'" & Agro_SQL_SaveText(utente_Username) & "' as Username ")
            StrSQL.AppendLine("       ," & Agro_SQL_SaveNum(Enita_Cod) & " as Entita_Cod ")
            StrSQL.AppendLine("       ,q.Piva as Piva ")
            StrSQL.AppendLine("       ,q.Sa_Cod as Sa_Cod ")
            StrSQL.AppendLine("       ,0 as Appezza ")
            StrSQL.AppendLine("       ,0 as Id_Reg ")
            StrSQL.AppendLine(" FROM  (" & sqlQuery & ") q WHERE q.Sa_Cod IS NOT NULL ")

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

    Public Function Ottieni_Sql_Popola_Tabella_Temporanea_DT_FiltroneImprese(ByRef guid As String,
                        ByRef PivaSuperUser As String,
                        ByRef utente_Username As String,
                        ByRef Enita_Cod As Integer,
                        ByVal sqlQuery As String) As String

        Dim StrSQL As New System.Text.StringBuilder
        StrSQL.Length = 0
        StrSQL.AppendLine(" INSERT INTO #Utenti_Visibilita_Appoggio_" & guid & " ")
        StrSQL.AppendLine(" SELECT '" & Agro_SQL_SaveText(PivaSuperUser) & "' as PivaSuperUser ")
        StrSQL.AppendLine("       ,'" & Agro_SQL_SaveText(utente_Username) & "' as Username ")
        StrSQL.AppendLine("       ," & Agro_SQL_SaveNum(Enita_Cod) & " as Entita_Cod ")
        StrSQL.AppendLine("       ,q.Piva as Piva ")
        StrSQL.AppendLine("       ,0 as Sa_Cod ")
        StrSQL.AppendLine("       ,0 as Appezza ")
        StrSQL.AppendLine("       ,0 as Id_Reg ")
        StrSQL.AppendLine(" FROM  (" & sqlQuery & ") q ")

        Return StrSQL.ToOrigin

    End Function

    Public Function PopolaTabellaTemporaneaDTFiltroneImprese(ByRef guid As String,
                                                            ByRef PivaSuperUser As String,
                                                            ByRef utente_Username As String,
                                                            ByRef Enita_Cod As Integer,
                                                            ByRef sqlQuery As String,
                                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R.PopolaTabellaTemporaneaDTFiltroneImprese()"

        '====================================================================================
        'Parametri opzionali :
        '   Username_Utente
        '   Id_Servizio
        '   FiltroAggiuntivo
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.AppendLine(" INSERT INTO ##Utenti_Visibilita_Appoggio_" & guid & " ")
            StrSQL.AppendLine(" SELECT '" & Agro_SQL_SaveText(PivaSuperUser) & "' as PivaSuperUser ")
            StrSQL.AppendLine("       ,'" & Agro_SQL_SaveText(utente_Username) & "' as Username ")
            StrSQL.AppendLine("       ," & Agro_SQL_SaveNum(Enita_Cod) & " as Entita_Cod ")
            StrSQL.AppendLine("       ,q.Piva as Piva ")
            StrSQL.AppendLine("       ,0 as Sa_Cod ")
            StrSQL.AppendLine("       ,0 as Appezza ")
            StrSQL.AppendLine("       ,0 as Id_Reg ")
            StrSQL.AppendLine(" FROM  (" & sqlQuery & ") q ")

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

    Public Function Ottieni_Sql_Popola_Tabella_Temporanea_DT_FiltroneCentri(ByVal guid As String,
                        ByVal PivaSuperUser As String,
                        ByVal utente_Username As String,
                        ByVal Enita_Cod As Integer,
                        ByVal sqlQuery As String) As String

        Dim StrSQL As New System.Text.StringBuilder

        StrSQL.Length = 0
        StrSQL.AppendLine(" INSERT INTO #Utenti_Visibilita_Appoggio_" & guid & " ")
        StrSQL.AppendLine(" SELECT '" & Agro_SQL_SaveText(PivaSuperUser) & "' as PivaSuperUser ")
        StrSQL.AppendLine("       ,'" & Agro_SQL_SaveText(utente_Username) & "' as Username ")
        StrSQL.AppendLine("       ," & Agro_SQL_SaveNum(Enita_Cod) & " as Entita_Cod ")
        StrSQL.AppendLine("       ,q.Piva as Piva ")
        StrSQL.AppendLine("       ,q.Sa_Cod as Sa_Cod ")
        StrSQL.AppendLine("       ,0 as Appezza ")
        StrSQL.AppendLine("       ,0 as Id_Reg ")
        StrSQL.AppendLine(" FROM  (" & sqlQuery & ") q WHERE q.Sa_Cod IS NOT NULL ")

        Return StrSQL.ToOrigin()

    End Function


    Public Function PopolaTabellaTemporaneaDTFiltroneCentri(ByRef guid As String,
                                                            ByRef PivaSuperUser As String,
                                                            ByRef utente_Username As String,
                                                            ByRef Enita_Cod As Integer,
                                                            ByRef sqlQuery As String,
                                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable
        Dim NomeRoutine As String = "AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R.PopolaTabellaTemporaneaDTFiltroneCentri()"

        '====================================================================================
        'Parametri opzionali :
        '   Username_Utente
        '   Id_Servizio
        '   FiltroAggiuntivo
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.AppendLine(" INSERT INTO ##Utenti_Visibilita_Appoggio_" & guid & " ")
            StrSQL.AppendLine(" SELECT '" & Agro_SQL_SaveText(PivaSuperUser) & "' as PivaSuperUser ")
            StrSQL.AppendLine("       ,'" & Agro_SQL_SaveText(utente_Username) & "' as Username ")
            StrSQL.AppendLine("       ," & Agro_SQL_SaveNum(Enita_Cod) & " as Entita_Cod ")
            StrSQL.AppendLine("       ,q.Piva as Piva ")
            StrSQL.AppendLine("       ,q.Sa_Cod as Sa_Cod ")
            StrSQL.AppendLine("       ,0 as Appezza ")
            StrSQL.AppendLine("       ,0 as Id_Reg ")
            StrSQL.AppendLine(" FROM  (" & sqlQuery & ") q WHERE q.Sa_Cod IS NOT NULL ")

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


    Public Function LeggiDaTabellaTemporaneaxCancellazione(ByRef guid As String,
                                                            ByRef PivaSuperUser As String,
                                                            ByRef utente_Username As String,
                                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Integer
        Dim NomeRoutine As String = "AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_W.CancellaDaTabellaTemporanea()"

        '====================================================================================
        'Parametri opzionali :
        '   Username_Utente
        '   Id_Servizio
        '   FiltroAggiuntivo
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim conteggio As Integer = 0
        Try

            StrSQL.Length = 0
            StrSQL.AppendLine(" SELECT COUNT(*) ")
            StrSQL.AppendLine(" FROM Utenti_Visibilita_Appoggio (NOLOCK) ")
            StrSQL.AppendLine(" LEFT JOIN Utenti_Visibilita_Appoggio_" & guid & "  ON Utenti_Visibilita_Appoggio.PivaSuperUser = Utenti_Visibilita_Appoggio_" & guid & ".PivaSuperUser COLLATE Latin1_General_CI_AS ")
            StrSQL.AppendLine(" 										AND Utenti_Visibilita_Appoggio.Username = Utenti_Visibilita_Appoggio_" & guid & ".Username COLLATE Latin1_General_CI_AS ")
            StrSQL.AppendLine(" 										AND Utenti_Visibilita_Appoggio.Entita_Cod = Utenti_Visibilita_Appoggio_" & guid & ".Entita_Cod ")
            StrSQL.AppendLine(" 										AND Utenti_Visibilita_Appoggio.Piva = Utenti_Visibilita_Appoggio_" & guid & ".Piva COLLATE Latin1_General_CI_AS ")
            StrSQL.AppendLine(" 										AND Utenti_Visibilita_Appoggio.Sa_Cod = Utenti_Visibilita_Appoggio_" & guid & ".Sa_Cod ")
            StrSQL.AppendLine(" 										AND Utenti_Visibilita_Appoggio.Appezza = Utenti_Visibilita_Appoggio_" & guid & ".Appezza ")
            StrSQL.AppendLine(" 										AND Utenti_Visibilita_Appoggio.Id_Reg = Utenti_Visibilita_Appoggio_" & guid & ".Id_Reg ")
            StrSQL.AppendLine(" WHERE Utenti_Visibilita_Appoggio.Username = '" & Agro_SQL_SaveText(utente_Username) & "' AND Utenti_Visibilita_Appoggio_" & guid & ".Username IS NULL ")

            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)

            If DT IsNot Nothing AndAlso DT.Rows.Count > 0 Then
                conteggio = DT.Rows(0)(0)
            End If
            Return conteggio
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try


        Return conteggio


    End Function

End Class

Public Class Utenti_Visibilita_Appoggio_W
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function Scrivi(ByVal Entita_Cod As Int32,
                            ByVal Piva As String,
                            ByVal Sa_Cod As Int32,
                            ByVal Appezza As Int32,
                            ByVal Id_Reg As Int32,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                    ) As Boolean

        Dim NomeRoutine As String = "AnagrafeCoreUtentiDAL.Utenti_Visibilita_Appoggio_W.Scrivi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.AppendLine("INSERT INTO Utenti_Visibilita_Appoggio WITH(ROWLOCK)(       ")
            StrSQL.AppendLine("                    PivaSuperUser, Username, Entita_Cod,  ")
            StrSQL.AppendLine("                    Piva, Sa_Cod, Appezza, Id_Reg  ")
            StrSQL.AppendLine("                    ) ")
            StrSQL.AppendLine("VALUES (")
            StrSQL.AppendLine("          '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(objParametri.UtenteUsername) & "' ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Entita_Cod) & " ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(Piva) & "' ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Appezza) & " ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Id_Reg) & " ")
            StrSQL.AppendLine(")")
            '---------------------------------------------


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

    ''' <summary>
    ''' Popola la griglia Utenti_Visibilita_Appoggio con le imprese indicate e
    ''' ricorsivamente tutti i loro discendenti e i relativi centri aziendali.
    ''' </summary>
    ''' <remarks>
    ''' Per ottenere le pive dei capostipiti dalla tabella <tt>Utenti_Profili</tt>
    ''' vedi <see cref="AgronicaCoreUtentiBIZ.Utenti_Visibilita.LeggiPiveCapostipiti()"/>
    ''' </remarks>
    ''' <param name="username">Username a cui attribuire la visibilità</param>
    ''' <param name="piveList">Pive delle imprese capostipite.
    ''' Una lista vuota corrisponde a visibilità totale o nulla, non vine aggiunto alcun record in tabella.</param>
    ''' <param name="statementSql">Riferimento a una stringa in cui salvare la query da eseguire</param>
    ''' <param name="tableName">Nome della tabella in cui inserire i record.
    ''' Se non specificata, i record sono inseriti in Utenti_Visibilita_Appoggio</param>
    ''' <returns></returns>
    Public Function PopolaConGerarchia(
        username As String, piveList As List(Of String),
        objPServer As AgronicaCoreDataProvider.AgronicaCoreParametri,
        ByRef statementSql As String,
        Optional tableName As String = Nothing
    ) As Boolean

        Dim NomeRoutine As String = "AnagrafeCoreUtentiDAL.Utenti_Visibilita_Appoggio_W.PopolaConGerarchia()"
        Dim StrSQL As New Text.StringBuilder With {.Length = 0}
        Dim xRisp As Boolean
        Dim strList = ""
        If (piveList.Any AndAlso Not piveList.Contains("###########")) Then
            strList = piveList.Select(Function(piva) "'" & piva & "'").
                Aggregate(Function(acc, piva) acc & ", " & piva)
        Else
            'Per visibilita totale o nulla non inserisco records
            Return True
        End If
        Try

            statementSql = String.Empty

            '-- imprese_figlie ritorna lista delle imprese con piva come quella specificata più tutti i suoi discendenti
            '-- depth è inizializzato con il livello dei capostipiti, si assicura che non si entri in un loop infinito
            StrSQL.AppendLine(" WITH imprese_figlie AS ( ")
            StrSQL.AppendLine("  SELECT padre, figlio, livello, cast(livello as int) as depth FROM GerarchiaImprese ")
            StrSQL.AppendLine("  WHERE figlio in (" & Agro_SQL_Save_Clausola_IN(strList, True) & ") ")
            StrSQL.AppendLine("  UNION ALL ")
            StrSQL.AppendLine("  SELECT f.padre, f.figlio, f.livello, p.depth +1 as depth FROM GerarchiaImprese f ")
            StrSQL.AppendLine("  INNER JOIN imprese_figlie p ON f.Padre = p.Figlio ")
            StrSQL.AppendLine("  WHERE f.livello = p.depth + 1 ")
            StrSQL.AppendLine(" ) ")

            If IsNothing(tableName) Then
                StrSQL.AppendLine(" INSERT INTO Utenti_Visibilita_Appoggio ")
            Else
                StrSQL.AppendLine(" INSERT INTO " & tableName & " ")
            End If
            StrSQL.AppendLine(" (Piva, Sa_Cod, PivaSuperUser, username, Entita_Cod, Appezza, Id_Reg) ")

            StrSQL.AppendLine(" SELECT DISTINCT ")
            StrSQL.AppendLine("   Piva, sa_cod, ")
            StrSQL.AppendLine("   '" & Agro_SQL_SaveText(objPServer.PivaSuperUser) & "' as PivaSuperUser, ")
            StrSQL.AppendLine("   '" & Agro_SQL_SaveText(username) & "' as Username, ")
            StrSQL.AppendLine("   CASE sa_cod ")
            StrSQL.AppendLine("     WHEN cast(sa_cod as bit) THEN 1 ")
            StrSQL.AppendLine("     ELSE 2 ")
            StrSQL.AppendLine("   END as Entita_Cod, ")
            StrSQL.AppendLine("   0 as Appezza, 0 as Id_Reg ")
            StrSQL.AppendLine(" FROM ( ")
            StrSQL.AppendLine("   SELECT ")
            StrSQL.AppendLine("   imprese_figlie.Figlio as Piva, ")
            StrSQL.AppendLine("   isnull(Centri_Aziendali.sa_cod, 0) as sa_cod ")
            StrSQL.AppendLine("   FROM imprese_figlie ")
            StrSQL.AppendLine("   LEFT JOIN Centri_Aziendali ON centri_aziendali.PIVA = imprese_figlie.Figlio ")
            StrSQL.AppendLine("   UNION ALL ")
            StrSQL.AppendLine("   SELECT ")
            StrSQL.AppendLine("     a.Figlio as Piva, ")
            StrSQL.AppendLine("     0 As sa_cod ")
            StrSQL.AppendLine("   FROM imprese_figlie a ")
            StrSQL.AppendLine(" ) m ")

            If Not IsNothing(tableName) Then
                statementSql = StrSQL.ToOrigin(objPServer)
                Return True
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objPServer, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------
        Catch ex As Exception
            Scrivi_LOG(objPServer, NomeRoutine, ex.Message)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & ex.Message)
        End Try
        Return xRisp
    End Function

    Public Function ScriviVisibilitaImpresePassandoQueryFiltrone(
                                  ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                  ByVal QueryFiltrone As String) As Boolean

        Dim NomeRoutine As String = "AnagrafeCoreUtentiDAL.Utenti_Visibilita_Appoggio_W.Scrivi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Dim Entita_Cod As Int32 = enum_TipoEntita.Impresa
        Dim Sa_Cod As Int32 = 0
        Dim Appezza As Int32 = 0
        Dim Id_Reg As Int32 = 0

        Try
            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.AppendLine("INSERT INTO Utenti_Visibilita_Appoggio WITH (ROWLOCK) (       ")
            StrSQL.AppendLine("                    PivaSuperUser, Username, Entita_Cod,  ")
            StrSQL.AppendLine("                    Piva, Sa_Cod, Appezza, Id_Reg  ")
            StrSQL.AppendLine("                    ) ")
            StrSQL.AppendLine("SELECT DISTINCT ")
            StrSQL.AppendLine(" '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.AppendLine(",'" & Agro_SQL_SaveText(objParametri.UtenteUsername) & "' ")
            StrSQL.AppendLine(", " & Agro_SQL_SaveNum(Entita_Cod) & " ")
            StrSQL.AppendLine(", f.piva ")
            StrSQL.AppendLine(", " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            StrSQL.AppendLine(", " & Agro_SQL_SaveNum(Appezza) & " ")
            StrSQL.AppendLine(", " & Agro_SQL_SaveNum(Id_Reg) & " ")
            StrSQL.AppendLine("FROM ( ")
            StrSQL.AppendLine(QueryFiltrone)
            StrSQL.AppendLine(") as f ")

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

    Public Function ScriviVisibilitaCentriAzPassandoQueryFiltrone(
                              ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                              ByVal QueryFiltrone As String) As Boolean

        Dim NomeRoutine As String = "AnagrafeCoreUtentiDAL.Utenti_Visibilita_Appoggio_W.Scrivi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Dim Entita_Cod As Int32 = enum_TipoEntita.Centro
        Dim Appezza As Int32 = 0
        Dim Id_Reg As Int32 = 0

        Try
            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.AppendLine("INSERT INTO Utenti_Visibilita_Appoggio WITH (ROWLOCK) (       ")
            StrSQL.AppendLine("                    PivaSuperUser, Username, Entita_Cod,  ")
            StrSQL.AppendLine("                    Piva, Sa_Cod, Appezza, Id_Reg  ")
            StrSQL.AppendLine("                    ) ")
            StrSQL.AppendLine("SELECT DISTINCT ")
            StrSQL.AppendLine(" '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.AppendLine(",'" & Agro_SQL_SaveText(objParametri.UtenteUsername) & "' ")
            StrSQL.AppendLine(", " & Agro_SQL_SaveNum(Entita_Cod) & " ")
            StrSQL.AppendLine(", f.piva ")
            StrSQL.AppendLine(", ISNULL(f.sa_cod,0) ")
            StrSQL.AppendLine(", " & Agro_SQL_SaveNum(Appezza) & " ")
            StrSQL.AppendLine(", " & Agro_SQL_SaveNum(Id_Reg) & " ")
            StrSQL.AppendLine("FROM ( ")
            StrSQL.AppendLine(QueryFiltrone)
            StrSQL.AppendLine(") as f ")

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

    Public Function Aggiorna_Visibilita(ByVal QInsert As ArrayList, ByVal QDelete As ArrayList, ByRef objParametriUtenti As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Const nomeRoutine = "AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio.Aggiorna_Visibilita()"
        Dim messaggioErrore As String = ""
        Dim risultato As Integer = True

        Try

            For Each q In QInsert

                risultato = EseguiQuery_Scrittura(objParametriUtenti, q, nomeRoutine)

            Next

            For Each q In QDelete

                risultato = EseguiQuery_Scrittura(objParametriUtenti, q, nomeRoutine)

            Next


        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametriUtenti, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
            risultato = False
        End Try

        Return risultato
    End Function

    '##############################################################################################

    Public Sub ScriviVisibilitaNulla(username As String, objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri)

        Dim NomeRoutine As String = "AnagrafeCoreUtentiDAL.Utenti_Visibilita_Appoggio_W.ScriviVisibilitaNulla()"
        Dim StrSQL As New System.Text.StringBuilder With {.Length = 0}

        Try
            Cancella(0, "", objParametri, username)

            StrSQL.AppendLine(" INSERT INTO Utenti_Visibilita_Appoggio ")
            StrSQL.AppendLine(" (PivaSuperUser, Username, Entita_Cod, Piva,Sa_Cod, Appezza, Id_Reg) ")
            StrSQL.AppendLine(" VALUES( ")
            StrSQL.AppendLine("   '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "', ")
            StrSQL.AppendLine("   '" & Agro_SQL_SaveText(username) & "', ")
            StrSQL.AppendLine("   1, '###########', 0, 0 ,0 ")
            StrSQL.AppendLine(" ) ")

            '--------------------------------------------------------------------------
            EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            Dim MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

    End Sub

    '##############################################################################################
    Public Function Cancella(
                            ByVal Entita_Cod As Int32,
                                ByVal xFiltroAggiuntivo As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                Optional ByVal userName As String = ""
                                    ) As Boolean

        Dim NomeRoutine As String = "AnagrafeCoreUtentiDAL.Utenti_Visibilita_Appoggio_W.Cancella()"
        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            StrSQL.Length = 0
            StrSQL.AppendLine(" DELETE ")
            StrSQL.AppendLine(" FROM     Utenti_Visibilita_Appoggio ")
            StrSQL.AppendLine(" WITH(ROWLOCK) ")
            'StrSQL.AppendLine(" WITH (NOLOCK) ")
            StrSQL.AppendLine(" WHERE PivaSuperUser='" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'")

            If String.IsNullOrEmpty(userName) Then
                StrSQL.AppendLine(" AND Username = '" & Agro_SQL_SaveText(objParametri.UtenteUsername) & "' ")
            Else
                StrSQL.AppendLine(" AND Username = '" & Agro_SQL_SaveText(userName) & "' ")
            End If

            If Entita_Cod <> 0 Then
                StrSQL.AppendLine(" AND  Entita_Cod =  " & Agro_SQL_SaveNum(Entita_Cod) & " ")
            End If

            '---------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
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

    Public Function Ottieni_Sql_Cancella_Per_Piu_Utenti(
                            ByVal Entita_Cod As Int32,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal listaUtenti As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                    ) As String

        Dim NomeRoutine As String = "AnagrafeCoreUtentiDAL.Utenti_Visibilita_Appoggio_W.Cancella_Per_Piu_Utenti()"
        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            StrSQL.Length = 0
            StrSQL.AppendLine(" DELETE ")
            StrSQL.AppendLine(" FROM     Utenti_Visibilita_Appoggio ")
            StrSQL.AppendLine(" WITH(ROWLOCK) ")
            'StrSQL.AppendLine(" WITH (NOLOCK) ")
            StrSQL.AppendLine(" WHERE PivaSuperUser='" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'")
            StrSQL.AppendLine(" AND Username IN ( " & Agro_SQL_Save_Clausola_IN(listaUtenti, True) & ") ")

            If Entita_Cod <> 0 Then
                StrSQL.AppendLine(" AND  Entita_Cod =  " & Agro_SQL_SaveNum(Entita_Cod) & " ")
            End If

            '---------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            Return StrSQL.ToOrigin(objParametri)

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try
        Return xRisp

    End Function

    Public Function Ottieni_Sql_Creazione_Tabella_Temp_Utenti_Visibilita_Appoggio(ByVal guid As String) As String

        Dim StrSQL As New System.Text.StringBuilder

        StrSQL.Length = 0
        StrSQL.AppendLine(" CREATE TABLE #Utenti_Visibilita_Appoggio_" & guid & " ( ")
        StrSQL.AppendLine(" 	[PivaSuperUser] [nvarchar](25) NOT NULL, ")
        StrSQL.AppendLine(" 	[Username] [varchar](250) NOT NULL, ")
        StrSQL.AppendLine(" 	[Entita_Cod] [int] NOT NULL, ")
        StrSQL.AppendLine(" 	[Piva] [nvarchar](25) NOT NULL, ")
        StrSQL.AppendLine(" 	[Sa_Cod] [int] NOT NULL, ")
        StrSQL.AppendLine(" 	[Appezza] [int] NOT NULL, ")
        StrSQL.AppendLine(" 	[Id_Reg] [int] NOT NULL, ")
        StrSQL.AppendLine("  CONSTRAINT [PK_Utenti_Visibilita_Appoggio_" & guid & "] PRIMARY KEY CLUSTERED  ")
        StrSQL.AppendLine(" ( ")
        StrSQL.AppendLine(" 	[PivaSuperUser] ASC, ")
        StrSQL.AppendLine(" 	[Username] ASC, ")
        StrSQL.AppendLine(" 	[Entita_Cod] ASC, ")
        StrSQL.AppendLine(" 	[Piva] ASC, ")
        StrSQL.AppendLine(" 	[Sa_Cod] ASC, ")
        StrSQL.AppendLine(" 	[Appezza] ASC, ")
        StrSQL.AppendLine(" 	[Id_Reg] ASC ")
        StrSQL.AppendLine(" )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY] ")
        StrSQL.AppendLine(" ) ON [PRIMARY] ")

        Return StrSQL.ToOrigin

    End Function

    Public Function CreaTabellaTemporaneaUtenti_Visibilita_Appoggio(ByRef guid As String,
                                         ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable
        Dim NomeRoutine As String = "AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_W.CreaTabellaTemporaneaUtenti_Visibilita_Appoggio()"

        '====================================================================================
        'Parametri opzionali :
        '   Username_Utente
        '   Id_Servizio
        '   FiltroAggiuntivo
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.AppendLine(" CREATE TABLE ##Utenti_Visibilita_Appoggio_" & guid & " ( ")
            StrSQL.AppendLine(" 	[PivaSuperUser] [nvarchar](25) NOT NULL, ")
            StrSQL.AppendLine(" 	[Username] [varchar](250) NOT NULL, ")
            StrSQL.AppendLine(" 	[Entita_Cod] [int] NOT NULL, ")
            StrSQL.AppendLine(" 	[Piva] [nvarchar](25) NOT NULL, ")
            StrSQL.AppendLine(" 	[Sa_Cod] [int] NOT NULL, ")
            StrSQL.AppendLine(" 	[Appezza] [int] NOT NULL, ")
            StrSQL.AppendLine(" 	[Id_Reg] [int] NOT NULL, ")
            StrSQL.AppendLine("  CONSTRAINT [PK_Utenti_Visibilita_Appoggio_" & guid & "] PRIMARY KEY CLUSTERED  ")
            StrSQL.AppendLine(" ( ")
            StrSQL.AppendLine(" 	[PivaSuperUser] ASC, ")
            StrSQL.AppendLine(" 	[Username] ASC, ")
            StrSQL.AppendLine(" 	[Entita_Cod] ASC, ")
            StrSQL.AppendLine(" 	[Piva] ASC, ")
            StrSQL.AppendLine(" 	[Sa_Cod] ASC, ")
            StrSQL.AppendLine(" 	[Appezza] ASC, ")
            StrSQL.AppendLine(" 	[Id_Reg] ASC ")
            StrSQL.AppendLine(" )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY] ")
            StrSQL.AppendLine(" ) ON [PRIMARY] ")

            EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try


        Return DT


    End Function

    Public Function CancellaTabellaTemporaneaUtenti_Visibilita_Appoggio(ByRef guid As String,
                                         ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable
        Dim NomeRoutine As String = "AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_W.CancellaTabellaTemporaneaUtenti_Visibilita_Appoggio()"

        '====================================================================================
        'Parametri opzionali :
        '   Username_Utente
        '   Id_Servizio
        '   FiltroAggiuntivo
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.AppendLine(" DROP TABLE Utenti_Visibilita_Appoggio_" & guid & " ")

            EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try


        Return DT


    End Function

    Public Function Ottieni_Sql_Popola_Utenti_Visibilita_Appoggio_Da_Tabella_Temp(ByRef guid As String,
                        ByRef PivaSuperUser As String,
                        ByRef utente_Username As String) As String

        Dim StrSQL As New System.Text.StringBuilder

        StrSQL.Length = 0

        StrSQL.AppendLine(" INSERT INTO Utenti_Visibilita_Appoggio WITH(ROWLOCK) ")
        StrSQL.AppendLine(" SELECT DISTINCT uvaTemp.* ")
        StrSQL.AppendLine(" FROM #Utenti_Visibilita_Appoggio_" & guid & " uvaTemp ")
        StrSQL.AppendLine(" LEFT JOIN Utenti_Visibilita_Appoggio (NOLOCK) ON Utenti_Visibilita_Appoggio.PivaSuperUser = uvaTemp.PivaSuperUser COLLATE Latin1_General_CI_AS ")
        StrSQL.AppendLine(" 										AND Utenti_Visibilita_Appoggio.Username = uvaTemp.Username COLLATE Latin1_General_CI_AS ")
        StrSQL.AppendLine(" 										AND Utenti_Visibilita_Appoggio.Entita_Cod = uvaTemp.Entita_Cod ")
        StrSQL.AppendLine(" 										AND Utenti_Visibilita_Appoggio.Piva = uvaTemp.Piva COLLATE Latin1_General_CI_AS ")
        StrSQL.AppendLine(" 										AND Utenti_Visibilita_Appoggio.Sa_Cod = uvaTemp.Sa_Cod ")
        StrSQL.AppendLine(" 										AND Utenti_Visibilita_Appoggio.Appezza = uvaTemp.Appezza ")
        StrSQL.AppendLine(" 										AND Utenti_Visibilita_Appoggio.Id_Reg = uvaTemp.Id_Reg ")
        StrSQL.AppendLine(" WHERE uvaTemp.Username = '" & Agro_SQL_SaveText(utente_Username) & "' AND Utenti_Visibilita_Appoggio.Username IS NULL ")

        Return StrSQL.ToOrigin

    End Function

    Public Function PopolaDaTabellaTemporanea(ByRef guid As String,
                                                            ByRef PivaSuperUser As String,
                                                            ByRef utente_Username As String,
                                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable
        Dim NomeRoutine As String = "AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_W.PopolaDaTabellaTemporanea()"

        '====================================================================================
        'Parametri opzionali :
        '   Username_Utente
        '   Id_Servizio
        '   FiltroAggiuntivo
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try


            StrSQL.Length = 0

            StrSQL.AppendLine(" INSERT INTO Utenti_Visibilita_Appoggio WITH(ROWLOCK) ")
            StrSQL.AppendLine(" SELECT DISTINCT uvaTemp.* ")
            StrSQL.AppendLine(" FROM ##Utenti_Visibilita_Appoggio_" & guid & " uvaTemp ")
            StrSQL.AppendLine(" LEFT JOIN Utenti_Visibilita_Appoggio (NOLOCK) ON Utenti_Visibilita_Appoggio.PivaSuperUser = uvaTemp.PivaSuperUser COLLATE Latin1_General_CI_AS ")
            StrSQL.AppendLine(" 										AND Utenti_Visibilita_Appoggio.Username = uvaTemp.Username COLLATE Latin1_General_CI_AS ")
            StrSQL.AppendLine(" 										AND Utenti_Visibilita_Appoggio.Entita_Cod = uvaTemp.Entita_Cod ")
            StrSQL.AppendLine(" 										AND Utenti_Visibilita_Appoggio.Piva = uvaTemp.Piva COLLATE Latin1_General_CI_AS ")
            StrSQL.AppendLine(" 										AND Utenti_Visibilita_Appoggio.Sa_Cod = uvaTemp.Sa_Cod ")
            StrSQL.AppendLine(" 										AND Utenti_Visibilita_Appoggio.Appezza = uvaTemp.Appezza ")
            StrSQL.AppendLine(" 										AND Utenti_Visibilita_Appoggio.Id_Reg = uvaTemp.Id_Reg ")
            StrSQL.AppendLine(" WHERE uvaTemp.Username = '" & Agro_SQL_SaveText(utente_Username) & "' AND Utenti_Visibilita_Appoggio.Username IS NULL ")

            EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try


        Return DT


    End Function

    Public Function Ottieni_Sql_Cancella_Utenti_Visibilita_Appoggio_Da_Tabella_Temp(ByVal guid As String,
                                                                                    ByVal PivaSuperUser As String,
                                                                                    ByVal utente_Username As String) As String

        Dim StrSQL As New System.Text.StringBuilder

        StrSQL.Length = 0
        StrSQL.AppendLine(" DELETE Utenti_Visibilita_Appoggio ")
        StrSQL.AppendLine(" FROM Utenti_Visibilita_Appoggio uva (NOLOCK) ")
        StrSQL.AppendLine(" LEFT JOIN #Utenti_Visibilita_Appoggio_" & guid & " uvaTemp  ON uva.PivaSuperUser = uvaTemp.PivaSuperUser COLLATE Latin1_General_CI_AS ")
        StrSQL.AppendLine(" 										AND uva.Username = uvaTemp.Username COLLATE Latin1_General_CI_AS ")
        StrSQL.AppendLine(" 										AND uva.Entita_Cod = uvaTemp.Entita_Cod ")
        StrSQL.AppendLine(" 										AND uva.Piva = uvaTemp.Piva COLLATE Latin1_General_CI_AS ")
        StrSQL.AppendLine(" 										AND uva.Sa_Cod = uvaTemp.Sa_Cod ")
        StrSQL.AppendLine(" 										AND uva.Appezza = uvaTemp.Appezza ")
        StrSQL.AppendLine(" 										AND uva.Id_Reg = uvaTemp.Id_Reg ")
        StrSQL.AppendLine(" WHERE uva.Username = '" & Agro_SQL_SaveText(utente_Username) & "' AND uvaTemp.Username IS NULL ")

        Return StrSQL.ToOrigin
    End Function

    Public Function CancellaDaTabellaTemporanea(ByRef guid As String,
                                                            ByRef PivaSuperUser As String,
                                                            ByRef utente_Username As String,
                                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable
        Dim NomeRoutine As String = "AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_W.CancellaDaTabellaTemporanea()"

        '====================================================================================
        'Parametri opzionali :
        '   Username_Utente
        '   Id_Servizio
        '   FiltroAggiuntivo
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.AppendLine(" DELETE Utenti_Visibilita_Appoggio ")
            StrSQL.AppendLine(" FROM Utenti_Visibilita_Appoggio uva (NOLOCK) ")
            StrSQL.AppendLine(" LEFT JOIN ##Utenti_Visibilita_Appoggio_" & guid & " uvaTemp  ON uva.PivaSuperUser = uvaTemp.PivaSuperUser COLLATE Latin1_General_CI_AS ")
            StrSQL.AppendLine(" 										AND uva.Username = uvaTemp.Username COLLATE Latin1_General_CI_AS ")
            StrSQL.AppendLine(" 										AND uva.Entita_Cod = uvaTemp.Entita_Cod ")
            StrSQL.AppendLine(" 										AND uva.Piva = uvaTemp.Piva COLLATE Latin1_General_CI_AS ")
            StrSQL.AppendLine(" 										AND uva.Sa_Cod = uvaTemp.Sa_Cod ")
            StrSQL.AppendLine(" 										AND uva.Appezza = uvaTemp.Appezza ")
            StrSQL.AppendLine(" 										AND uva.Id_Reg = uvaTemp.Id_Reg ")
            StrSQL.AppendLine(" WHERE uva.Username = '" & Agro_SQL_SaveText(utente_Username) & "' AND uvaTemp.Username IS NULL ")

            EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
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
