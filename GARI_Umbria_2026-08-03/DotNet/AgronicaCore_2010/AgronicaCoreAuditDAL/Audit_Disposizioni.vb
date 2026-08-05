Imports AgronicaCoreDataProvider.UtilityProvider
Public Class Audit_Disposizioni_R

    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function Leggi(
                            ByVal Audit_Tipo As Integer,
                            ByVal Regolamento_Cod As Integer,
                            ByVal Campo_Cod As Integer,
                            ByVal Disp_Cod As Integer,
                            ByVal Disp_Nome As String,
                            ByVal xFiltroAggiuntivo As String,
                            ByVal xOrderBy As String,
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                            Optional ByVal Descrizione As String = "",
                            Optional ByVal Validita_Inizio As Date = #1/1/1900#,
                            Optional ByVal Validita_Fine As Date = #12/31/2100#
                            ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAuditDAL.Audit_Disposizioni_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            '------------------------------------------------------------------
            StrSQL.Length = 0

            StrSQL.Append(" SELECT Audit_Disposizioni.* ")
            StrSQL.Append(" FROM  Audit_Disposizioni ")
            StrSQL.Append(" WHERE Audit_Disposizioni.Audit_Tipo = " & Agro_SQL_SaveNum(Audit_Tipo))
            StrSQL.Append(" AND  Audit_Disposizioni.Validita_inizio <= " & Agro_SQL_SaveDate(Validita_Fine))
            StrSQL.Append(" AND  Audit_Disposizioni.Validita_Fine >= " & Agro_SQL_SaveDate(Validita_Inizio))

            If Regolamento_Cod <> 0 Then
                StrSQL.Append(" AND   Regolamento_Cod = " & Agro_SQL_SaveNum(Regolamento_Cod) & " ")
            End If

            If Campo_Cod <> 0 Then
                StrSQL.Append(" AND Audit_Disposizioni.Campo = " & Agro_SQL_SaveNum(Campo_Cod))
            End If

            If Disp_Cod <> 0 Then
                StrSQL.Append(" AND Audit_Disposizioni.Disp_Cod = " & Agro_SQL_SaveNum(Disp_Cod))
            End If

            If Disp_Nome <> "" Then
                StrSQL.Append(" AND Audit_Disposizioni.Disp_Nome = '" & Agro_SQL_SaveText(Disp_Nome) & "'")
            End If

            If Descrizione <> "" Then
                StrSQL.Append(" AND Audit_Disposizioni.Descrizione = '" & Agro_SQL_SaveText(Descrizione) & "'")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY Ordine ")
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

    Public Function LeggiDomande(ByVal Audit_Tipo As Integer,
                                ByVal Regolamento_Cod As Integer,
                                ByVal Disp_Cod As Integer,
                                ByVal Domanda_Cod As Integer,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAuditDAL.Audit_Disposizioni_R.LeggiDomande()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim bRet As Boolean = False
        Dim bDominanteAttivo As Boolean = False

        Try

            '------------------------------------------------------------------
            StrSQL.Length = 0

            StrSQL.Append(" SELECT * ")
            StrSQL.Append(" FROM Audit_Domande_IntervisteXDisposizioni ")
            StrSQL.Append(" WHERE 1=1 ")

            If Disp_Cod <> 0 Then
                StrSQL.Append(" AND Audit_Domande_IntervisteXDisposizioni.Disp_Cod = " & Agro_SQL_SaveNum(Disp_Cod))
            End If

            If Domanda_Cod <> 0 Then
                StrSQL.Append(" AND Audit_Domande_IntervisteXDisposizioni.Domanda_Cod = " & Agro_SQL_SaveNum(Domanda_Cod))
            End If

            If Audit_Tipo <> 0 Then
                StrSQL.Append(" AND Audit_Domande_IntervisteXDisposizioni.Audit_Tipo = " & Agro_SQL_SaveNum(Audit_Tipo))
            End If

            If Regolamento_Cod <> 0 Then
                StrSQL.Append(" AND Audit_Domande_IntervisteXDisposizioni.Regolamento_Cod = " & Agro_SQL_SaveNum(Regolamento_Cod))
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY Audit_Domande_IntervisteXDisposizioni.Domanda_Cod ")
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

    Public Function LeggiRisposte(ByVal Audit_Tipo As Integer,
                                ByVal Regolamento_Cod As Integer,
                                ByVal Piva As String,
                                ByVal Disp_Cod As Integer,
                                ByVal Data As Date,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                Optional Intervista_Cod As Integer = 0) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAuditDAL.Audit_Disposizioni_R.LeggiRisposte()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim bRet As Boolean = False
        Dim bDominanteAttivo As Boolean = False

        Try

            '------------------------------------------------------------------
            StrSQL.Length = 0

            StrSQL.Append(" SELECT Audit_Risposte_Interviste.Domanda_Cod,Audit_Risposte_Interviste.Valore")
            StrSQL.Append(" FROM Audit_Interviste ")
            StrSQL.Append(" INNER JOIN Audit_Risposte_Interviste ON Audit_Interviste.Intervista_Cod = Audit_Risposte_Interviste.Intervista_Cod And ")
            StrSQL.Append(" Audit_Interviste.Intervista_SuperUser = Audit_Risposte_Interviste.Intervista_SuperUser ")
            StrSQL.Append(" And Audit_Interviste.Audit_Tipo = Audit_Risposte_Interviste.Audit_Tipo And Audit_Interviste.Regolamento_Cod = Audit_Risposte_Interviste.Regolamento_Cod ")
            StrSQL.Append(" WHERE 1=1 ")

            If objParametri.PivaSuperUser <> "" Then
                StrSQL.Append(" And Audit_Interviste.Intervista_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            End If

            If Piva <> "" Then
                StrSQL.Append(" AND Audit_Interviste.Piva = '" & Agro_SQL_SaveText(Piva) & "'")
            End If

            If Audit_Tipo <> 0 Then
                StrSQL.Append(" AND Audit_Interviste.Audit_Tipo = " & Agro_SQL_SaveNum(Audit_Tipo))
            End If

            If Regolamento_Cod <> 0 Then
                StrSQL.Append(" AND Audit_Interviste.Regolamento_Cod = " & Agro_SQL_SaveNum(Regolamento_Cod))
            End If

            If Not IsNothing(Data) Then
                StrSQL.Append(" AND " & Agro_SQL_SaveDate(Data) & " >= Audit_Interviste.Validita_Inizio " &
                      " AND " & Agro_SQL_SaveDate(Data) & " <= Audit_Interviste.Validita_Fine ")
            End If

            If Intervista_Cod <> 0 Then
                StrSQL.Append(" AND Audit_Interviste.Intervista_Cod = " & Agro_SQL_SaveNum(Intervista_Cod))
            End If


            StrSQL.Append(" ORDER BY   Audit_Risposte_Interviste.Valore DESC ")

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
    Public Function LeggiAttivazione(ByVal Audit_Tipo As Integer,
                                ByVal Regolamento_Cod As Integer,
                                ByVal Piva As String,
                                ByVal Disp_Cod As Integer,
                                ByVal Data As Date,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                Optional Intervista_Cod As Integer = 0) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAuditDAL.Audit_Disposizioni_R.LeggiAttivazione()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim bRet As Boolean = False
        Dim bDominanteAttivo As Boolean = False

        Try

            '------------------------------------------------------------------
            StrSQL.Length = 0

            StrSQL.Append(" SELECT Audit_Risposte_Interviste.Valore, Audit_Domande_IntervisteXDisposizioni.Disp_Cod_Dominante ")
            StrSQL.Append(" FROM Audit_Interviste ")
            StrSQL.Append(" INNER JOIN Audit_Risposte_Interviste ON Audit_Interviste.Intervista_Cod = Audit_Risposte_Interviste.Intervista_Cod And ")
            StrSQL.Append(" Audit_Interviste.Intervista_SuperUser = Audit_Risposte_Interviste.Intervista_SuperUser ")
            StrSQL.Append(" And Audit_Interviste.Audit_Tipo = Audit_Risposte_Interviste.Audit_Tipo And Audit_Interviste.Regolamento_Cod = Audit_Risposte_Interviste.Regolamento_Cod ")
            StrSQL.Append(" INNER JOIN Audit_Domande_IntervisteXDisposizioni ON Audit_Risposte_Interviste.Domanda_Cod = Audit_Domande_IntervisteXDisposizioni.Domanda_Cod ")
            StrSQL.Append(" And Audit_Risposte_Interviste.Audit_Tipo = Audit_Domande_IntervisteXDisposizioni.Audit_Tipo ")
            StrSQL.Append(" And Audit_Risposte_Interviste.Regolamento_Cod = Audit_Domande_IntervisteXDisposizioni.Regolamento_Cod ")
            StrSQL.Append(" WHERE 1=1 ")

            If objParametri.PivaSuperUser <> "" Then
                StrSQL.Append(" And Audit_Interviste.Intervista_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            End If

            If Piva <> "" Then
                StrSQL.Append(" AND Audit_Interviste.Piva = '" & Agro_SQL_SaveText(Piva) & "'")
            End If

            If Disp_Cod <> 0 Then
                StrSQL.Append(" AND Audit_Domande_IntervisteXDisposizioni.Disp_Cod = " & Agro_SQL_SaveNum(Disp_Cod))
            End If

            If Audit_Tipo <> 0 Then
                StrSQL.Append(" AND Audit_Interviste.Audit_Tipo = " & Agro_SQL_SaveNum(Audit_Tipo))
            End If

            If Regolamento_Cod <> 0 Then
                StrSQL.Append(" AND Audit_Interviste.Regolamento_Cod = " & Agro_SQL_SaveNum(Regolamento_Cod))
            End If

            If Not IsNothing(Data) Then
                StrSQL.Append(" AND " & Agro_SQL_SaveDate(Data) & " >= Audit_Interviste.Validita_Inizio " &
                      " AND " & Agro_SQL_SaveDate(Data) & " <= Audit_Interviste.Validita_Fine ")
            End If

            If Intervista_Cod <> 0 Then
                StrSQL.Append(" AND Audit_Interviste.Intervista_Cod = " & Agro_SQL_SaveNum(Intervista_Cod))
            End If


            StrSQL.Append(" ORDER BY   Audit_Risposte_Interviste.Valore DESC ")

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
            Return False
        End Try

        If DT.Rows.Count = 0 Then

            ' non c'è il record nella tabella Audit_Domande_IntervisteXDisposizioni quindi l'atto/norma deve essere sempre attivo
            bRet = True

        ElseIf Not IsDBNull(DT.Rows(0).Item("Valore")) Then

            bRet = IIf(DT.Rows(0).Item("Valore").ToString = "1", True, False)

            If bRet Then

                If Not IsDBNull(DT.Rows(0).Item("Disp_Cod_Dominante")) Then

                    Dim DT_Dom As New DataTable
                    Dim StrSQL_New As String

                    Try

                        StrSQL_New = StrSQL.ToString.Replace(" AND Audit_Domande_IntervisteXDisposizioni.Disp_Cod = " & Agro_SQL_SaveNum(Disp_Cod),
                                                             " AND Audit_Domande_IntervisteXDisposizioni.Disp_Cod = " & Agro_SQL_SaveNum(DT.Rows(0).Item("Disp_Cod_Dominante")))

                        DT_Dom = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)

                    Catch ex As Exception
                        MessaggioErrore = ex.Message
                        Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
                        DT_Dom = Nothing
                        Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
                        Return False
                    End Try

                    If DT_Dom.Rows.Count = 0 Then
                        ' non c'è il record nella tabella Audit_Domande_IntervisteXDisposizioni quindi l'atto dominante è attivo e quello sotto esame NON deve esserlo
                        bDominanteAttivo = True

                    ElseIf Not IsNothing(DT_Dom.Rows(0).Item("Valore")) Then

                        bDominanteAttivo = IIf(DT_Dom.Rows(0).Item("Valore").ToString = "1", True, False)

                    End If

                End If

                bRet = Not bDominanteAttivo

            End If

        End If

        Return bRet

    End Function

    '##############################################################################################
    Public Function LeggiSezioni(ByVal Audit_Tipo As Integer,
                                ByVal Regolamento_Cod As Integer,
                                ByVal Disp_Cod As Integer,
                                ByVal Sezione_Cod As Long,
                                ByVal Parte As Long,
                                ByVal Data As Date,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCoreAuditDAL.Audit_Disposizioni_R.LeggiSezioni()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Stb.Length = 0
            Stb.Append(" SELECT *, ")
            Stb.Append(" ISNULL ((SELECT FunCalcoloLivello FROM Audit_DisposizioniXSezioni ")
            Stb.Append("          WHERE Audit_Sezioni.Sezione_Cod = Audit_DisposizioniXSezioni.Sezione_Cod ")
            Stb.Append("          AND   Audit_Sezioni.Audit_Tipo = Audit_DisposizioniXSezioni.Audit_Tipo ")
            Stb.Append("          AND   Audit_Sezioni.Regolamento_Cod = Audit_DisposizioniXSezioni.Regolamento_Cod ")

            ' possono esserci + funzioni di calcolo del livello per la stessa sezione
            If Data <> #1/1/1900# Then
                Stb.Append("      AND Audit_DisposizioniXSezioni.Validita_Inizio <= " & Agro_SQL_SaveDate(Data) & " ")
                Stb.Append("      AND Audit_DisposizioniXSezioni.Validita_Fine > " & Agro_SQL_SaveDate(Data) & " ")
            End If
            Stb.Append("          AND Disp_Cod = " & Agro_SQL_SaveNum(Disp_Cod) & "), NULL) AS FunCalcoloLivello ")

            Stb.Append(" FROM Audit_Sezioni ")

            Stb.Append(" WHERE Audit_Sezioni.Audit_Tipo = " & Agro_SQL_SaveNum(Audit_Tipo))
            Stb.Append(" AND   Audit_Sezioni.Regolamento_Cod = " & Agro_SQL_SaveNum(Regolamento_Cod))

            If Sezione_Cod <> 0 Then
                Stb.Append(" AND Audit_Sezioni.Sezione_Cod = " & Agro_SQL_SaveNum(Sezione_Cod))
            End If

            If Parte <> 0 Then
                Stb.Append(" AND Audit_Sezioni.Parte = " & Agro_SQL_SaveNum(Parte))
            End If

            If Data <> #1/1/1900# Then
                Stb.Append(" AND Audit_Sezioni.Validita_Inizio <= " & Agro_SQL_SaveDate(Data) & " ")
                Stb.Append(" AND Audit_Sezioni.Validita_Fine > " & Agro_SQL_SaveDate(Data) & " ")
            End If

            If xFiltroAggiuntivo <> "" Then
                Stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            If xOrderBy <> "" Then
                Stb.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                Stb.Append(" ORDER BY Ordine ")
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

    '##############################################################################################
    Public Function LeggiCodici(ByVal Audit_Tipo As Integer,
                                ByVal Regolamento_Cod As Integer,
                                ByVal Disposizione_Cod As Integer,
                                ByVal Sezione_Cod As Integer,
                                ByVal Parte_Cod As Integer,
                                ByVal Default_Cod As Integer,
                                ByVal Validita_Inizio As Date,
                                ByVal Validita_Fine As Date,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                Optional ByVal Tipo_Audit As String = "",
                                Optional ByVal Punto_Numero As String = "",
                                Optional ByVal Descrizione As String = "",
                                Optional ByVal Punteggio As Integer = 0,
                                Optional ByVal Nota As Integer = -1
                                ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCoreAuditDAL.Audit_Disposizioni_R.LeggiCodici()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT Audit_Codici.*, ")
            StrSQL.Append(" Audit_Disposizioni.Disp_Nome, Audit_Disposizioni.Descrizione AS [Disp_Des], ")
            StrSQL.Append(" Audit_Sezioni.Sezione_Des, Audit_Sezioni.Parte, ")
            StrSQL.Append(" NULL AS Valore, NULL AS Valore_2, NULL AS ValPropostaCorrettiva, ")
            If Default_Cod = 1 Then
                StrSQL.Append(" Audit_CodiciXCodici.Punto_Numero_DA AS Punto_Numero_Default, NULL AS Punto_Numero_Deroga ")
            ElseIf Default_Cod = 2 Then
                StrSQL.Append(" NULL AS Punto_Numero_Default, Audit_CodiciXDeroghe.Punto_Numero AS Punto_Numero_Deroga ")
            Else
                StrSQL.Append(" NULL AS Punto_Numero_Default, NULL AS Punto_Numero_Deroga ")
            End If
            StrSQL.Append(" FROM   Audit_Codici ")
            StrSQL.Append(" INNER JOIN Audit_Disposizioni ON Audit_Codici.Audit_Tipo = Audit_Disposizioni.Audit_Tipo AND Audit_Codici.Regolamento_Cod = Audit_Disposizioni.Regolamento_Cod AND Audit_Codici.Disp_Cod = Audit_Disposizioni.Disp_Cod ")
            StrSQL.Append(" INNER JOIN Audit_Sezioni ON Audit_Codici.Audit_Tipo = Audit_Sezioni.Audit_Tipo AND Audit_Codici.Regolamento_Cod = Audit_Sezioni.Regolamento_Cod AND Audit_Codici.Sezione_Cod = Audit_Sezioni.Sezione_Cod ")
            If Default_Cod = 1 Then
                StrSQL.Append(" INNER JOIN Audit_CodiciXCodici ON Audit_Codici.Audit_Tipo = Audit_CodiciXCodici.Audit_Tipo AND Audit_Codici.Regolamento_Cod = Audit_CodiciXCodici.Regolamento_Cod AND Audit_Codici.Disp_Cod = Audit_CodiciXCodici.Disp_Cod AND Audit_Codici.Punto_Numero = Audit_CodiciXCodici.Punto_Numero_A ")
            ElseIf Default_Cod = 2 Then
                StrSQL.Append(" INNER JOIN Audit_CodiciXDeroghe ON Audit_Codici.Audit_Tipo = Audit_CodiciXDeroghe.Audit_Tipo AND Audit_Codici.Regolamento_Cod = Audit_CodiciXDeroghe.Regolamento_Cod AND Audit_Codici.Disp_Cod = Audit_CodiciXDeroghe.Disp_Cod AND Audit_Codici.Punto_Numero = Audit_CodiciXDeroghe.Deroga ")
            End If

            StrSQL.Append(" WHERE 1 = 1")

            If Audit_Tipo <> 0 Then
                StrSQL.Append(" And Audit_Codici.Audit_Tipo = " & Agro_SQL_SaveNum(Audit_Tipo))
            End If

            If Regolamento_Cod <> 0 Then
                StrSQL.Append(" AND Audit_Codici.Regolamento_Cod = " & Agro_SQL_SaveNum(Regolamento_Cod))
            End If

            If Validita_Fine <> #12/31/2100# Then
                StrSQL.Append(" AND Audit_Codici.Validita_inizio <= " & Agro_SQL_SaveDate(Validita_Fine))
            End If

            If Validita_Inizio <> #1/1/1900# Then
                StrSQL.Append(" AND Audit_Codici.Validita_Fine >= " & Agro_SQL_SaveDate(Validita_Inizio))
            End If

            If Nota <> -1 Then
                StrSQL.Append(" AND Audit_Codici.Nota = " & Agro_SQL_SaveNum(Nota))
            End If

            If Disposizione_Cod <> 0 Then
                StrSQL.Append(" AND Audit_Codici.Disp_Cod = " & Agro_SQL_SaveNum(Disposizione_Cod))
            End If

            If Sezione_Cod <> 0 Then
                StrSQL.Append(" AND Audit_Codici.Sezione_Cod = " & Agro_SQL_SaveNum(Sezione_Cod))
            End If

            If Parte_Cod <> 0 Then
                StrSQL.Append(" AND Audit_Sezioni.Parte = " & Agro_SQL_SaveNum(Parte_Cod))
            End If

            If Punto_Numero <> "" Then
                StrSQL.Append(" AND Audit_Codici.Punto_Numero = '" & Agro_SQL_SaveText(Punto_Numero) & "'")
            End If

            If Descrizione <> "" Then
                StrSQL.Append(" AND Audit_Codici.Descrizione LIKE '%" & Agro_SQL_SaveText(Descrizione) & "%'")
            End If

            If Punteggio <> 0 Then
                StrSQL.Append(" AND Audit_Codici.Punteggio = " & Agro_SQL_SaveNum(Punteggio))
            End If

            If Tipo_Audit <> "" Then    'A=atti, N=norme
                StrSQL.Append(" AND Audit_Disposizioni.Disp_Nome LIKE '" & Agro_SQL_SaveText(Tipo_Audit) & "%'")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY Audit_Codici.Disp_Cod, Audit_Codici.Punto_Numero ")
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
    Public Function LeggiCodiciDisposizioni(ByVal Audit_Tipo As Integer,
                                ByVal Regolamento_Cod As Integer,
                                ByVal Disposizione_Cod As Integer,
                                ByVal Sezione_Cod As Integer,
                                ByVal Parte_Cod As Integer,
                                ByVal Validita_Inizio As Date,
                                ByVal Validita_Fine As Date,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCoreAuditDAL.Audit_Disposizioni_R.LeggiCodiciDisposizioni()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT Audit_Codici.*, ")
            StrSQL.Append(" Audit_Disposizioni.Disp_Nome, Audit_Disposizioni.Descrizione AS [Disp_Des], ")
            StrSQL.Append(" Audit_Sezioni.Sezione_Des, Audit_Sezioni.Parte, ")
            StrSQL.Append(" NULL AS Valore, NULL AS Valore_2, NULL AS ValPropostaCorrettiva, ")
            StrSQL.Append(" NULL AS Punto_Numero_Default, NULL AS Punto_Numero_Deroga, ")

            StrSQL.Append(" ISNULL((SELECT FunCalcoloLivello FROM Audit_DisposizioniXSezioni ")
            StrSQL.Append(" WHERE Audit_Sezioni.Sezione_Cod = Audit_DisposizioniXSezioni.Sezione_Cod ")
            StrSQL.Append(" AND Audit_Sezioni.Audit_Tipo = Audit_DisposizioniXSezioni.Audit_Tipo ")
            StrSQL.Append(" AND Audit_Sezioni.Regolamento_Cod = Audit_DisposizioniXSezioni.Regolamento_Cod ")
            If Validita_Fine <> #12/31/2100# Then
                StrSQL.Append(" AND Audit_DisposizioniXSezioni.Validita_Inizio <= " & Agro_SQL_SaveDate(Validita_Fine))
            End If
            If Validita_Inizio <> #1/1/1900# Then
                StrSQL.Append(" AND Audit_DisposizioniXSezioni.Validita_Fine >= " & Agro_SQL_SaveDate(Validita_Inizio))
            End If
            If Disposizione_Cod <> 0 Then
                StrSQL.Append(" AND Disp_Cod = " & Agro_SQL_SaveNum(Disposizione_Cod))
            End If
            StrSQL.Append(" ), NULL) AS FunCalcoloLivello ")

            StrSQL.Append(" FROM   Audit_Codici ")
            StrSQL.Append(" INNER JOIN Audit_Disposizioni ON Audit_Codici.Audit_Tipo = Audit_Disposizioni.Audit_Tipo AND Audit_Codici.Regolamento_Cod = Audit_Disposizioni.Regolamento_Cod AND Audit_Codici.Disp_Cod = Audit_Disposizioni.Disp_Cod ")
            StrSQL.Append(" INNER JOIN Audit_Sezioni ON Audit_Codici.Audit_Tipo = Audit_Sezioni.Audit_Tipo AND Audit_Codici.Regolamento_Cod = Audit_Sezioni.Regolamento_Cod AND Audit_Codici.Sezione_Cod = Audit_Sezioni.Sezione_Cod ")

            StrSQL.Append(" WHERE Audit_Codici.Audit_Tipo = " & Agro_SQL_SaveNum(Audit_Tipo))

            If Regolamento_Cod <> 0 Then
                StrSQL.Append(" AND Audit_Codici.Regolamento_Cod = " & Agro_SQL_SaveNum(Regolamento_Cod))
            End If

            If Validita_Fine <> #12/31/2100# Then
                StrSQL.Append(" AND Audit_Codici.Validita_inizio <= " & Agro_SQL_SaveDate(Validita_Fine))
            End If

            If Validita_Inizio <> #1/1/1900# Then
                StrSQL.Append(" AND Audit_Codici.Validita_Fine >= " & Agro_SQL_SaveDate(Validita_Inizio))
            End If

            If Disposizione_Cod <> 0 Then
                StrSQL.Append(" AND Audit_Codici.Disp_Cod = " & Agro_SQL_SaveNum(Disposizione_Cod))
            End If

            If Sezione_Cod <> 0 Then
                StrSQL.Append(" AND Audit_Codici.Sezione_Cod = " & Agro_SQL_SaveNum(Sezione_Cod))
            End If

            If Parte_Cod <> 0 Then
                StrSQL.Append(" AND Audit_Sezioni.Parte = " & Agro_SQL_SaveNum(Parte_Cod))
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY Audit_Codici.Disp_Cod, Audit_Sezioni.Parte, Audit_Sezioni.Ordine, CASE   ")
                StrSQL.Append("     WHEN RIGHT(punto_Numero, 1) LIKE '[^0-9]'   ")
                StrSQL.Append("         THEN CAST(LEFT(punto_Numero, LEN(punto_Numero) - 1) AS INT)  ")
                StrSQL.Append("     ELSE CAST(punto_Numero AS INT)  ")
                StrSQL.Append(" END;  ")
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

    Public Function LeggiPunteggi(ByVal Audit_Tipo As Integer,
                                    ByVal Regolamento_Cod As Integer,
                                    ByVal Audit_Cod As Long,
                                    ByVal Disp_Cod As Integer,
                                    ByVal Sezione_Cod As Integer,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAuditDAL.Audit_Disposizioni_R.LeggiPunteggi()"
        Dim MessaggioErrore As String = ""

        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As New DataTable

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT Audit_Disposizioni.Disp_Cod, Audit_Disposizioni.Disp_Nome, Audit_Disposizioni.Ordine, ")
            StrSQL.Append(" Audit_Codici.Punto_Numero, Audit_Codici.Sezione_Cod, Audit_Codici.Descrizione, Audit_Codici.Punteggio, Valore ")
            StrSQL.Append(" FROM Audit_Disposizioni ")
            StrSQL.Append(" INNER JOIN Audit_Codici ON Audit_Disposizioni.Disp_Cod = Audit_Codici.Disp_Cod And Audit_Disposizioni.Audit_Tipo = Audit_Codici.Audit_Tipo And Audit_Disposizioni.Regolamento_Cod = Audit_Codici.Regolamento_Cod ")
            StrSQL.Append(" INNER JOIN Audit_Risposte ON Audit_Codici.Disp_Cod = Audit_Risposte.Disp_Cod And Audit_Codici.Punto_Numero = Audit_Risposte.Punto_Numero ")
            StrSQL.Append(" And Audit_Codici.Audit_Tipo = Audit_Risposte.Audit_Tipo And Audit_Codici.Regolamento_Cod = Audit_Risposte.Regolamento_Cod ")
            StrSQL.Append(" WHERE (Audit_Risposte.Audit_Cod = " & Agro_SQL_SaveNum(Audit_Cod) & ") ")
            StrSQL.Append(" And (Audit_Codici.Disp_Cod = " & Agro_SQL_SaveNum(Disp_Cod) & ") ")
            StrSQL.Append(" And (Audit_Codici.Audit_Tipo = " & Agro_SQL_SaveNum(Audit_Tipo) & ") ")
            StrSQL.Append(" And (Audit_Codici.Regolamento_Cod = " & Agro_SQL_SaveNum(Regolamento_Cod) & ") ")
            If Sezione_Cod <> 0 Then
                StrSQL.Append(" And (Audit_Codici.Sezione_Cod = " & Agro_SQL_SaveNum(Sezione_Cod) & ") ")
            Else
                StrSQL.Append(" And (Audit_Codici.Punteggio Is Not NULL)")
            End If
            StrSQL.Append(" And (Valore ='1')")
            StrSQL.Append(" ORDER BY Sezione_Cod, Punteggio")

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
    Public Function DispNome_From_DispCod(ByVal Audit_Tipo As Integer,
                                          ByVal Regolamento_Cod As Integer,
                                          ByVal Disp_Cod As Integer,
                                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As String

        '----- Descrizione        
        Dim MessaggioErrore As String = ""
        Dim NomeRoutine As String = "AgronicaCoreAuditDAL.Audit_Disposizioni_R.DispNome_From_DispCod()"
        Dim StrSQL As String
        Dim DT As New DataTable
        Dim strRet As String = String.Empty

        Try


            StrSQL = " SELECT Disp_Nome " &
                 " FROM   Audit_Disposizioni " &
                 " WHERE  1=1 "

            If Audit_Tipo <> 0 Then
                StrSQL &= " AND Audit_Tipo = " & Agro_SQL_SaveNum(Audit_Tipo)
            End If

            If Regolamento_Cod <> 0 Then
                StrSQL &= " AND Regolamento_Cod = " & Agro_SQL_SaveNum(Regolamento_Cod)
            End If

            If Disp_Cod <> 0 Then
                StrSQL &= " AND Disp_Cod = " & Agro_SQL_SaveNum(Disp_Cod)
            End If

            '----------------------------------------------------
            '--- Recupero il datatable --------------------------
            '----------------------------------------------------

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            If DT.Rows.Count <> 0 Then
                strRet = DT.Rows(0).Item(0)
            End If


        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return strRet

    End Function


End Class
