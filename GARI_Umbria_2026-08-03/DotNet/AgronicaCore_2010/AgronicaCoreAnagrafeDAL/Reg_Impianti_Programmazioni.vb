Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.DataProviderExtensions


Public Class Reg_Impianti_Programmazioni_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '################################################
    Public Function Leggi(ByVal PIVA As String,
                          ByVal Sa_Cod As Int32,
                          ByVal Appezza As Int32,
                          ByVal Id_Reg As Int32,
                          ByVal Progetto_Cod As Int32,
                          ByVal Programmazione_Cod As Int32,
                          ByVal Programmazione_Entita_Cod As Int32,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreParametri
                          ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Reg_Impianti_Programmazioni_R.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = "" 
        '   Sa_Cod = 0 
        '   Appezza = 0 
        '   Id_Reg = 0
        '   Programmazione_Cod = 0 
        '   Programmazione_Entita_Cod = "" 
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT * ")
            StrSQL.Append(" FROM  Reg_Impianti_Programmazioni ")
            StrSQL.Append(" JOIN Reg_Impianti ON Reg_Impianti_Programmazioni.PIVA = Reg_Impianti.PIVA AND ")
            StrSQL.Append("                      Reg_Impianti_Programmazioni.Sa_Cod = Reg_Impianti.Sa_Cod AND ")
            StrSQL.Append("                      Reg_Impianti_Programmazioni.Appezza = Reg_Impianti.Appezza AND ")
            StrSQL.Append("                      Reg_Impianti_Programmazioni.Id_Reg = Reg_Impianti.Id_Reg ")
            StrSQL.Append(" WHERE Reg_Impianti_Programmazioni.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.Append(" AND   Reg_Impianti_Programmazioni.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.Append(" AND   Reg_Impianti_Programmazioni.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            If PIVA <> "" Then
                StrSQL.Append(" AND Reg_Impianti_Programmazioni.Piva = '" & Agro_SQL_SaveText(PIVA) & "' ")
            End If

            If Sa_Cod <> 0 Then
                StrSQL.Append(" AND Reg_Impianti_Programmazioni.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            End If

            If Appezza <> 0 Then
                StrSQL.Append(" AND Reg_Impianti_Programmazioni.Appezza = " & Agro_SQL_SaveNum(Appezza) & " ")
            End If

            If Id_Reg <> 0 Then
                StrSQL.Append(" AND Reg_Impianti_Programmazioni.Id_Reg = " & Agro_SQL_SaveNum(Id_Reg) & " ")
            End If

            If Progetto_Cod <> 0 Then
                StrSQL.Append(" AND Reg_Impianti_Programmazioni.Progetto_Cod = " & Agro_SQL_SaveNum(Progetto_Cod) & " ")
            End If

            If Programmazione_Cod <> 0 Then
                StrSQL.Append(" AND Reg_Impianti_Programmazioni.Programmazione_Cod = " & Agro_SQL_SaveNum(Programmazione_Cod) & " ")
            End If

            If Programmazione_Entita_Cod <> 0 Then
                StrSQL.Append(" AND Reg_Impianti_Programmazioni.Programmazione_Entita_Cod = " & Agro_SQL_SaveNum(Programmazione_Entita_Cod) & " ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND     Reg_Impianti_Programmazioni.Inviato >= 0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND     Reg_Impianti_Programmazioni.Inviato = -1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY Reg_Impianti_Programmazioni.Piva, Reg_Impianti_Programmazioni.Sa_Cod, Reg_Impianti_Programmazioni.Appezza, Reg_Impianti_Programmazioni.Id_reg  ")
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


    '################################################
    Public Function Leggi_N_fabbisogno(ByVal PIVA As String,
                                       ByVal Sa_Cod As Int32,
                                       ByVal Appezza As Int32,
                                       ByVal Id_Reg As Int32,
                                       ByVal Progetto_Cod As Int32,
                                       ByVal Programmazione_Cod As Int32,
                                       ByVal Programmazione_Entita_Cod As Int32,
                                       ByVal xFiltroAggiuntivo As String,
                                       ByVal xOrderBy As String,
                                       ByRef objParametri As AgronicaCoreParametri
                                       ) As Decimal

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Reg_Impianti_Programmazioni_R.Leggi_N_fabbisogno()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = "" 
        '   Sa_Cod = 0 
        '   Appezza = 0 
        '   Id_Reg = 0
        '   Programmazione_Cod = 0 
        '   Programmazione_Entita_Cod = "" 
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable
        Dim N_fabbisogno As Decimal = 0

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT N_fabbisogno ")
            StrSQL.Append(" FROM   Programmazione_Entita INNER JOIN ")
            StrSQL.Append("        Reg_Impianti_Programmazioni ON Programmazione_Entita.Programmazione_Cod = Reg_Impianti_Programmazioni.Programmazione_Cod AND  ")
            StrSQL.Append("        Programmazione_Entita.Programmazione_Entita_Cod = Reg_Impianti_Programmazioni.Programmazione_Entita_Cod ")
            StrSQL.Append(" WHERE Reg_Impianti_Programmazioni.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.Append(" AND   Reg_Impianti_Programmazioni.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.Append(" AND   Reg_Impianti_Programmazioni.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            If PIVA <> "" Then
                StrSQL.Append(" AND Reg_Impianti_Programmazioni.Piva = '" & Agro_SQL_SaveText(PIVA) & "' ")
            End If

            If Sa_Cod <> 0 Then
                StrSQL.Append(" AND Reg_Impianti_Programmazioni.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            End If

            If Appezza <> 0 Then
                StrSQL.Append(" AND Reg_Impianti_Programmazioni.Appezza = " & Agro_SQL_SaveNum(Appezza) & " ")
            End If

            If Id_Reg <> 0 Then
                StrSQL.Append(" AND Reg_Impianti_Programmazioni.Id_Reg = " & Agro_SQL_SaveNum(Id_Reg) & " ")
            End If

            If Progetto_Cod <> 0 Then
                StrSQL.Append(" AND Reg_Impianti_Programmazioni.Progetto_Cod = " & Agro_SQL_SaveNum(Progetto_Cod) & " ")
            End If

            If Programmazione_Cod <> 0 Then
                StrSQL.Append(" AND Reg_Impianti_Programmazioni.Programmazione_Cod = " & Agro_SQL_SaveNum(Programmazione_Cod) & " ")
            End If

            If Programmazione_Entita_Cod <> 0 Then
                StrSQL.Append(" AND Reg_Impianti_Programmazioni.Programmazione_Entita_Cod = " & Agro_SQL_SaveNum(Programmazione_Entita_Cod) & " ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND     Reg_Impianti_Programmazioni.Inviato >= 0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND     Reg_Impianti_Programmazioni.Inviato = -1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY Reg_Impianti_Programmazioni.Piva, Reg_Impianti_Programmazioni.Sa_Cod, Reg_Impianti_Programmazioni.Appezza, Reg_Impianti_Programmazioni.Id_reg  ")
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                N_fabbisogno = dt.Rows(0).Item("N_fabbisogno")
            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return N_fabbisogno

    End Function


    Public Function Leggi_OperazioniImpiantiRibaltati_DaProgrammazione_OLD(
                           ByVal PIVA As String,
                           ByVal Programmazione_Cod As Int32,
                           ByVal Programmazione_Entita_Cod As Int32,
                           ByVal xFiltroAggiuntivo As String,
                           ByVal xOrderBy As String,
                           ByRef objParametri As AgronicaCoreParametri
                           ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Reg_Impianti_Programmazioni_R.Leggi_OperazioniImpiantiRibaltati_DaProgrammazione_OLD()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = "" 
        '   Sa_Cod = 0 
        '   Appezza = 0 
        '   Id_Reg = 0
        '   Programmazione_Cod = 0 
        '   Programmazione_Entita_Cod = "" 
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.Append(" Select DISTINCT Mov_Destinazioni.Piva, Mov_Destinazioni.Sa_Cod, Mov_Destinazioni.Id_Agenda, Agenda.Lav_Cod, Agenda.des_lib, Movimenti.Data_Movimento, ")
            StrSQL.Append(" Reg_Impianti_Programmazioni.Programmazione_Cod, Reg_Impianti_Programmazioni.Programmazione_Entita_Cod ")
            StrSQL.Append(" FROM         Agenda INNER JOIN   Movimenti ON Agenda.PIVA = Movimenti.PIVA AND Agenda.Sa_Cod = Movimenti.Sa_Cod AND Agenda.Id_Agenda = Movimenti.Id_Agenda INNER JOIN ")
            StrSQL.Append(" Mov_Destinazioni ON Movimenti.PIVA = Mov_Destinazioni.Piva AND Movimenti.Sa_Cod = Mov_Destinazioni.Sa_Cod AND  ")
            StrSQL.Append(" Movimenti.Id_Agenda = Mov_Destinazioni.Id_Agenda AND Movimenti.Id_Mov = Mov_Destinazioni.Id_Mov INNER JOIN ")
            StrSQL.Append(" Reg_Impianti_Programmazioni ON Mov_Destinazioni.Piva = Reg_Impianti_Programmazioni.Piva AND  ")
            StrSQL.Append(" Mov_Destinazioni.Sa_Cod = Reg_Impianti_Programmazioni.Sa_Cod And Mov_Destinazioni.Appezza = Reg_Impianti_Programmazioni.Appezza And ")
            StrSQL.Append(" Mov_Destinazioni.Id_Destinazione = Reg_Impianti_Programmazioni.Id_Reg ")

            StrSQL.Append(" WHERE Reg_Impianti_Programmazioni.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

            If PIVA <> "" Then
                StrSQL.Append(" AND Reg_Impianti_Programmazioni.Piva = '" & Agro_SQL_SaveText(PIVA) & "' ")
            End If

            If Programmazione_Cod <> 0 Then
                StrSQL.Append(" AND Reg_Impianti_Programmazioni.Programmazione_Cod = " & Agro_SQL_SaveNum(Programmazione_Cod) & " ")
            End If

            If Programmazione_Entita_Cod <> 0 Then
                StrSQL.Append(" AND Reg_Impianti_Programmazioni.Programmazione_Entita_Cod = " & Agro_SQL_SaveNum(Programmazione_Entita_Cod) & " ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND     Reg_Impianti_Programmazioni.Inviato >= 0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND     Reg_Impianti_Programmazioni.Inviato = -1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
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


    Public Function Leggi_OperazioniImpiantiRibaltati_DaProgrammazione(
                           ByVal PIVA As String,
                           ByVal Programmazione_Cod As Int32,
                           ByVal Programmazione_Entita_Cod As Int32,
                           ByVal xFiltroAggiuntivo As String,
                           ByVal xOrderBy As String,
                           ByRef objParametri As AgronicaCoreParametri
                           ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Reg_Impianti_Programmazioni_R.Leggi_OperazioniImpiantiRibaltati_DaProgrammazione()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = "" 
        '   Sa_Cod = 0 
        '   Appezza = 0 
        '   Id_Reg = 0
        '   Programmazione_Cod = 0 
        '   Programmazione_Entita_Cod = "" 
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.Append(" Select DISTINCT Mov_Destinazioni.Piva, Mov_Destinazioni.Sa_Cod, Mov_Destinazioni.Id_Agenda, Agenda.Lav_Cod, Agenda.des_lib, Movimenti.Data_Movimento, ")
            StrSQL.Append(" Reg_Impianti_Programmazioni.Programmazione_Cod, Reg_Impianti_Programmazioni.Programmazione_Entita_Cod ")
            StrSQL.Append(" FROM         Agenda INNER JOIN   Movimenti ON Agenda.PIVA = Movimenti.PIVA AND Agenda.Sa_Cod = Movimenti.Sa_Cod AND Agenda.Id_Agenda = Movimenti.Id_Agenda INNER JOIN ")
            StrSQL.Append(" Mov_Destinazioni ON Movimenti.PIVA = Mov_Destinazioni.Piva AND Movimenti.Sa_Cod = Mov_Destinazioni.Sa_Cod AND  ")
            StrSQL.Append(" Movimenti.Id_Agenda = Mov_Destinazioni.Id_Agenda AND Movimenti.Id_Mov = Mov_Destinazioni.Id_Mov INNER JOIN ")
            StrSQL.Append(" Reg_Impianti_Programmazioni ON Mov_Destinazioni.Piva = Reg_Impianti_Programmazioni.Piva AND  ")
            StrSQL.Append(" Mov_Destinazioni.Sa_Cod = Reg_Impianti_Programmazioni.Sa_Cod And Mov_Destinazioni.Appezza = Reg_Impianti_Programmazioni.Appezza And ")
            StrSQL.Append(" Mov_Destinazioni.Id_Destinazione = Reg_Impianti_Programmazioni.Id_Reg ")

            StrSQL.Append(" WHERE Reg_Impianti_Programmazioni.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

            If PIVA <> "" Then
                StrSQL.Append(" AND Reg_Impianti_Programmazioni.Piva = '" & Agro_SQL_SaveText(PIVA) & "' ")
            End If

            If Programmazione_Cod <> 0 Then
                StrSQL.Append(" AND Reg_Impianti_Programmazioni.Programmazione_Cod = " & Agro_SQL_SaveNum(Programmazione_Cod) & " ")
            End If

            If Programmazione_Entita_Cod <> 0 Then
                StrSQL.Append(" AND Reg_Impianti_Programmazioni.Programmazione_Entita_Cod = " & Agro_SQL_SaveNum(Programmazione_Entita_Cod) & " ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND     Reg_Impianti_Programmazioni.Inviato >= 0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND     Reg_Impianti_Programmazioni.Inviato = -1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
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


    Public Function Leggi_RicetteImpiantiRibaltati_DaProgrammazione(
                           ByVal PIVA As String,
                           ByVal Programmazione_Cod As Int32,
                           ByVal Programmazione_Entita_Cod As Int32,
                           ByVal xFiltroAggiuntivo As String,
                           ByVal xOrderBy As String,
                           ByRef objParametri As AgronicaCoreParametri
                           ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Reg_Impianti_Programmazioni_R.Leggi_RicetteImpiantiRibaltati_DaProgrammazione()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = "" 
        '   Sa_Cod = 0 
        '   Appezza = 0 
        '   Id_Reg = 0
        '   Programmazione_Cod = 0 
        '   Programmazione_Entita_Cod = "" 
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine("Select DISTINCT Ricette_Destinazioni.Piva,  ")
            StrSQL.AppendLine("                 Ricette_Destinazioni.Sa_Cod, ")
            StrSQL.AppendLine("                 Ricette_Destinazioni.Appezza, ")
            StrSQL.AppendLine("                 Ricette_Destinazioni.Id_Reg, ")
            StrSQL.AppendLine("                 Reg_Impianti_Programmazioni.Programmazione_Cod, ")
            StrSQL.AppendLine("                 Reg_Impianti_Programmazioni.Programmazione_Entita_Cod   ")
            StrSQL.AppendLine(" From Ricette_Destinazioni  ")
            StrSQL.AppendLine(" INNER Join       Reg_Impianti_Programmazioni ON Ricette_Destinazioni.Piva = Reg_Impianti_Programmazioni.Piva  ")
            StrSQL.AppendLine("                                          And   Ricette_Destinazioni.Sa_Cod = Reg_Impianti_Programmazioni.Sa_Cod  ")
            StrSQL.AppendLine("                                          And Ricette_Destinazioni.Appezza = Reg_Impianti_Programmazioni.Appezza  ")
            StrSQL.AppendLine("                                          And  Ricette_Destinazioni.Id_Reg = Reg_Impianti_Programmazioni.Id_Reg   ")
            StrSQL.AppendLine(" WHERE Reg_Impianti_Programmazioni.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'   ")

            If PIVA <> "" Then
                StrSQL.Append(" AND Reg_Impianti_Programmazioni.Piva = '" & Agro_SQL_SaveText(PIVA) & "' ")
            End If

            If Programmazione_Cod <> 0 Then
                StrSQL.Append(" AND Reg_Impianti_Programmazioni.Programmazione_Cod = " & Agro_SQL_SaveNum(Programmazione_Cod) & " ")
            End If

            If Programmazione_Entita_Cod <> 0 Then
                StrSQL.Append(" AND Reg_Impianti_Programmazioni.Programmazione_Entita_Cod = " & Agro_SQL_SaveNum(Programmazione_Entita_Cod) & " ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND     Reg_Impianti_Programmazioni.Inviato >= 0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND     Reg_Impianti_Programmazioni.Inviato = -1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
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


    Public Function Leggi_RicetteProgrammazioniRibaltati_DaProgrammazione(
                           ByVal PIVA As String,
                           ByVal Programmazione_Cod As Int32,
                           ByVal Programmazione_Entita_Cod As Int32,
                           ByVal xFiltroAggiuntivo As String,
                           ByVal xOrderBy As String,
                           ByRef objParametri As AgronicaCoreParametri
                           ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Reg_Impianti_Programmazioni_R.Leggi_RicetteProgrammazioniRibaltati_DaProgrammazione()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = "" 
        '   Sa_Cod = 0 
        '   Appezza = 0 
        '   Id_Reg = 0
        '   Programmazione_Cod = 0 
        '   Programmazione_Entita_Cod = "" 
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine("Select DISTINCT Ricette_Destinazioni.Piva,  ")
            StrSQL.AppendLine("                 Ricette_Destinazioni.Sa_Cod, ")
            StrSQL.AppendLine("                 Programmazione_Entita.Programmazione_Entita_Cod ")
            StrSQL.AppendLine(" From Ricette_Destinazioni  ")
            StrSQL.AppendLine(" INNER Join       Programmazione_Entita ON Ricette_Destinazioni.Piva = Programmazione_Entita.Piva  ")
            StrSQL.AppendLine("                                          And Ricette_Destinazioni.Programmazione_Entita_Cod = Programmazione_Entita.Programmazione_Entita_Cod ")
            StrSQL.AppendLine(" And Programmazione_Entita.Programmazione_Cod = 46914   ")
            StrSQL.AppendLine(" And     Programmazione_Entita.Inviato >= 0")

            StrSQL.AppendLine(" WHERE Programmazione_Entita.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'   ")

            If PIVA <> "" Then
                StrSQL.Append(" AND Programmazione_Entita.Piva = '" & Agro_SQL_SaveText(PIVA) & "' ")
            End If

            If Programmazione_Cod <> 0 Then
                StrSQL.Append(" AND Programmazione_Entita.Programmazione_Cod = " & Agro_SQL_SaveNum(Programmazione_Cod) & " ")
            End If

            If Programmazione_Entita_Cod <> 0 Then
                StrSQL.Append(" AND Programmazione_Entita.Programmazione_Entita_Cod = " & Agro_SQL_SaveNum(Programmazione_Entita_Cod) & " ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND     Programmazione_Entita.Inviato >= 0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND     Programmazione_Entita.Inviato = -1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
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

    '#############################################################################
    Public Function LeggiOccupazioneDes_su_VegCodCulCodIdCod(ByVal PIVA As String,
                                                             ByVal Sa_Cod As Int32,
                                                             ByVal Appezza As Int32,
                                                             ByVal Id_Reg As Int32,
                                                             ByVal Veg_Cod As Int32,
                                                             ByVal Cul_Cod As Int32,
                                                             ByVal Id_Cod As Int32,
                                                             ByVal FiltroImpiantiConAND As String,
                                                             ByVal xFiltroAggiuntivo As String,
                                                             ByVal xOrderBy As String,
                                                             ByRef objParametri As AgronicaCoreParametri,
                                                             Optional ByVal IDTestataTemp As Integer = 0,
                                                             Optional ByVal Validita_Inizio__tmp_FiltroImpianti As Date = AGRODATAINIZIO,
                                                             Optional ByVal Validita_Fine__tmp_FiltroImpianti As Date = AGRODATAFINE
                                                             ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Reg_Impianti_Programmazioni_R.LeggiOccupazioneDes_su_VegCodCulCodIdCod()"
        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.Append(" SELECT AGEA.Occupazione_Des, PE.cul_cod_agea, PE.uso_cod_agea, PE.occupazione_cod_agea, PE.destinazione_cod_agea, pe.qualita_cod_agea, AGEA.veg_cod, AGEA.cul_cod, AGEA.id_cod, rip.* " & vbCrLf)
            StrSQL.Append(" FROM Reg_Impianti_Programmazioni RIP " & vbCrLf)
            StrSQL.Append(" INNER JOIN Programmazione_Entita PE ON RIP.piva = PE.piva AND RIP.sa_cod = PE.sa_cod  AND rip.Programmazione_Cod = PE.Programmazione_Cod AND RIP.Programmazione_Entita_Cod = PE.Programmazione_Entita_Cod " & vbCrLf)
            StrSQL.Append(" INNER JOIN Codifica_SpecieVegetali_Agea_2015_2020 AGEA ON PE.Veg_cod=AGEA.Veg_cod " & vbCrLf)
            StrSQL.Append(" and PE.CUL_cod=AGEA.cul_cod AND PE.id_cod=AGEA.id_cod" & vbCrLf)
            StrSQL.Append(" INNER JOIN Reg_Impianti ON RIP.piva = Reg_Impianti.piva AND RIP.sa_cod = Reg_Impianti.sa_cod AND RIP.appezza = Reg_Impianti.appezza AND RIP.id_reg = Reg_Impianti.id_reg  " & vbCrLf)

            If IDTestataTemp <> 0 Then
                StrSQL.Append("         inner Join __tmp_FiltroImpianti f ")
                StrSQL.Append("         On  Reg_Impianti.PIVA = f.piva  ")
                StrSQL.Append("         And Reg_Impianti.SA_COD = f.sa_cod ")
                StrSQL.Append("         And Reg_Impianti.APPEZZA = f.appezza ")
                StrSQL.Append("         And Reg_Impianti.ID_REG = f.id_reg ")
                StrSQL.Append("               And f.idTestataTemp = " & IDTestataTemp)
                StrSQL.Append("               And f.Validita_Inizio <= " & Agro_SQL_SaveDate(Validita_Fine__tmp_FiltroImpianti) & " ")
                StrSQL.Append("               And f.Validita_Fine >= " & Agro_SQL_SaveDate(Validita_Inizio__tmp_FiltroImpianti) & " ")
            End If

            StrSQL.Append(" WHERE Occupazione_Des is not null" & vbCrLf)
            StrSQL.Append(" AND Occupazione_Des <>'' " & vbCrLf)

            'aggiunto per evitare che nei casi in cui nel planning ci sono tutti 0, venga preso il primo utilizzo a caso
            StrSQL.Append(" AND (pe.veg_cod <> 0 or pe.id_cod <> 0 ) " & vbCrLf)

            If PIVA <> "" Then
                StrSQL.Append(" AND RIP.Piva = '" & Agro_SQL_SaveText(PIVA) & "' " & vbCrLf)
            End If

            If Sa_Cod <> 0 Then
                StrSQL.Append(" AND RIP.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " " & vbCrLf)
            End If

            If Appezza <> 0 Then
                StrSQL.Append(" AND RIP.Appezza = " & Agro_SQL_SaveNum(Appezza) & " " & vbCrLf)
            End If

            If Id_Reg <> 0 Then
                StrSQL.Append(" AND RIP.Id_Reg = " & Agro_SQL_SaveNum(Id_Reg) & " " & vbCrLf)
            End If

            If Veg_Cod <> 0 Then
                StrSQL.Append(" AND PE.Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod) & " " & vbCrLf)
            End If

            If Cul_Cod <> 0 Then
                StrSQL.Append(" AND PE.Cul_Cod = " & Agro_SQL_SaveNum(Cul_Cod) & " " & vbCrLf)
            End If

            If Id_Cod <> 0 Then
                StrSQL.Append(" AND PE.Id_Cod = " & Agro_SQL_SaveNum(Id_Cod) & " " & vbCrLf)
            End If

            If FiltroImpiantiConAND <> "" Then
                StrSQL.Append(FiltroImpiantiConAND & vbCrLf)
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri) & vbCrLf)
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND     RIP.Inviato >= 0 " & vbCrLf)
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND     RIP.Inviato = -1 " & vbCrLf)
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri) & vbCrLf)
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

    '#############################################################################
    Public Function LeggiOccupazioneDes_su_quintuplaAGEA(ByVal PIVA As String,
                                                         ByVal Sa_Cod As Int32,
                                                         ByVal Appezza As Int32,
                                                         ByVal Id_Reg As Int32,
                                                         ByVal Cul_Cod_Agea As String,
                                                         ByVal Uso_Cod_Agea As String,
                                                         ByVal Occupazione_Cod_Agea As String,
                                                         ByVal Destinazione_Cod_Agea As String,
                                                         ByVal Qualita_Cod_Agea As String,
                                                         ByVal FiltroImpiantiConAND As String,
                                                         ByVal xFiltroAggiuntivo As String,
                                                         ByVal xOrderBy As String,
                                                         ByRef objParametri As AgronicaCoreParametri,
                                                         Optional ByVal IDTestataTemp As Integer = 0,
                                                         Optional ByVal Validita_Inizio__tmp_FiltroImpianti As Date = AGRODATAINIZIO,
                                                         Optional ByVal Validita_Fine__tmp_FiltroImpianti As Date = AGRODATAFINE
                                                         ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Reg_Impianti_Programmazioni_R.LeggiOccupazioneDes_su_quintuplaAGEA()"
        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.Append(" SELECT AGEA.Occupazione_Des, PE.cul_cod_agea, PE.uso_cod_agea, PE.occupazione_cod_agea, pe.destinazione_cod_agea, pe.qualita_cod_agea, AGEA.veg_cod, AGEA.cul_cod, AGEA.id_cod, rip.* " & vbCrLf)
            StrSQL.Append(" FROM Reg_Impianti_Programmazioni RIP " & vbCrLf)
            StrSQL.Append(" INNER JOIN Programmazione_Entita PE ON RIP.piva = PE.piva AND RIP.sa_cod = PE.sa_cod  AND rip.Programmazione_Cod = PE.Programmazione_Cod AND RIP.Programmazione_Entita_Cod = PE.Programmazione_Entita_Cod " & vbCrLf)
            StrSQL.Append(" INNER JOIN Codifica_SpecieVegetali_Agea_2015_2020 AGEA ON PE.uso_cod_agea = AGEA.uso_cod " & vbCrLf)
            '07/12/2018: Drudi ha detto di commentare il join sul cul_cod che molto spesso non è valorizzato
            'StrSQL.Append(" PE.cul_cod_agea = AGEA.cul_cod  " & vbCrLf)
            StrSQL.Append(" AND  PE.occupazione_cod_agea = AGEA.occupazione_cod AND PE.destinazione_cod_agea = AGEA.destinazione_cod AND pe.qualita_cod_agea = AGEA.qualita_cod " & vbCrLf)
            StrSQL.Append(" INNER JOIN Reg_Impianti ON RIP.piva = Reg_Impianti.piva AND RIP.sa_cod = Reg_Impianti.sa_cod AND RIP.appezza = Reg_Impianti.appezza AND RIP.id_reg = Reg_Impianti.id_reg  " & vbCrLf)

            If IDTestataTemp <> 0 Then
                StrSQL.Append("         inner Join __tmp_FiltroImpianti f ")
                StrSQL.Append("         On  Reg_Impianti.PIVA = f.piva  ")
                StrSQL.Append("         And Reg_Impianti.SA_COD = f.sa_cod ")
                StrSQL.Append("         And Reg_Impianti.APPEZZA = f.appezza ")
                StrSQL.Append("         And Reg_Impianti.ID_REG = f.id_reg ")
                StrSQL.Append("               And f.idTestataTemp = " & IDTestataTemp)
                StrSQL.Append("               And f.Validita_Inizio <= " & Agro_SQL_SaveDate(Validita_Fine__tmp_FiltroImpianti) & " ")
                StrSQL.Append("               And f.Validita_Fine >= " & Agro_SQL_SaveDate(Validita_Inizio__tmp_FiltroImpianti) & " ")
            End If

            StrSQL.Append(" WHERE Occupazione_Des is not null" & vbCrLf)
            StrSQL.Append(" AND Occupazione_Des <>'' " & vbCrLf)

            If PIVA <> "" Then
                StrSQL.Append(" AND RIP.Piva = '" & Agro_SQL_SaveText(PIVA) & "' " & vbCrLf)
            End If

            If Sa_Cod <> 0 Then
                StrSQL.Append(" AND RIP.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " " & vbCrLf)
            End If

            If Appezza <> 0 Then
                StrSQL.Append(" AND RIP.Appezza = " & Agro_SQL_SaveNum(Appezza) & " " & vbCrLf)
            End If

            If Id_Reg <> 0 Then
                StrSQL.Append(" AND RIP.Id_Reg = " & Agro_SQL_SaveNum(Id_Reg) & " " & vbCrLf)
            End If

            If Cul_Cod_Agea <> "" Then
                StrSQL.Append(" AND PE.Cul_Cod_Agea = '" & Agro_SQL_SaveText(Cul_Cod_Agea) & "' " & vbCrLf)
            End If

            If Uso_Cod_Agea <> "" Then
                StrSQL.Append(" AND PE.Uso_Cod_Agea = '" & Agro_SQL_SaveText(Uso_Cod_Agea) & "' " & vbCrLf)
            End If

            If Occupazione_Cod_Agea <> "" Then
                StrSQL.Append(" AND PE.Occupazione_Cod_Agea = '" & Agro_SQL_SaveText(Occupazione_Cod_Agea) & "' " & vbCrLf)
            End If

            If Destinazione_Cod_Agea <> "" Then
                StrSQL.Append(" AND PE.Destinazione_Cod_Agea = '" & Agro_SQL_SaveText(Destinazione_Cod_Agea) & "' " & vbCrLf)
            End If

            If Qualita_Cod_Agea <> "" Then
                StrSQL.Append(" AND PE.Qualita_Cod_Agea = '" & Agro_SQL_SaveText(Qualita_Cod_Agea) & "' " & vbCrLf)
            End If

            If FiltroImpiantiConAND <> "" Then
                StrSQL.Append(FiltroImpiantiConAND & vbCrLf)
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri) & vbCrLf)
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND     RIP.Inviato >= 0 " & vbCrLf)
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND     RIP.Inviato = -1 " & vbCrLf)
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri) & vbCrLf)
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

End Class


'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################


Public Class Reg_Impianti_Programmazioni_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Scrivi(ByVal Piva As String,
                           ByVal Sa_Cod As Int32,
                           ByVal Appezza As Int32,
                           ByVal Id_Reg As Int32,
                           ByVal Progetto_Cod As Int32,
                           ByVal Programmazione_Cod As Int32,
                           ByVal Programmazione_Entita_Cod As Int32,
                           ByVal Validita_Inizio As Date,
                           ByVal Validita_Fine As Date,
                           ByRef objParametri As AgronicaCoreParametri
                           ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Reg_Impianti_Programmazioni_W.Scrivi()"

        Dim messaggioErrore As String = ""
        Dim strSql As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            strSql.Length = 0
            strSql.Append("INSERT INTO Reg_Impianti_Programmazioni(         ")
            strSql.Append("                    Piva_SuperUser,              ")
            strSql.Append("                    Piva,                        ")
            strSql.Append("                    Sa_Cod,                      ")
            strSql.Append("                    Appezza,                     ")
            strSql.Append("                    Id_Reg,                      ")
            strSql.Append("                    Progetto_Cod,                ")
            strSql.Append("                    Programmazione_Cod,          ")
            strSql.Append("                    Programmazione_Entita_Cod,   ")
            strSql.Append("                    Inviato, DataInvio,          ")
            strSql.Append("                    Data_Creazione,     Data_Modifica, ")
            strSql.Append("                    UserName_Creazione, UserName_Modifica, ")
            strSql.Append("                    Validita_Inizio,    Validita_Fine ")
            strSql.Append("                    ) ")
            strSql.Append("VALUES (")
            strSql.Append("          '" & Agro_SQL_SaveText(Trim(objParametri.PivaSuperUser)) & "' ")
            strSql.Append("         , '" & Agro_SQL_SaveText(Trim(Piva)) & "' ")
            strSql.Append("         , " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            strSql.Append("         , " & Agro_SQL_SaveNum(Appezza) & "  ")
            strSql.Append("         , " & Agro_SQL_SaveNum(Id_Reg) & "  ")
            strSql.Append("         , " & Agro_SQL_SaveNum(Progetto_Cod) & "  ")
            strSql.Append("         , " & Agro_SQL_SaveNum(Programmazione_Cod) & "  ")
            strSql.Append("         , " & Agro_SQL_SaveNum(Programmazione_Entita_Cod) & " ")
            strSql.Append("         , 0  ")
            strSql.Append("         , Null  ")
            strSql.Append("         , " & Agro_SQL_SaveDate(Date.Now) & "  ")
            strSql.Append("         , " & Agro_SQL_SaveDate(Date.Now) & "  ")
            strSql.Append("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            strSql.Append("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            strSql.Append("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            strSql.Append("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")
            strSql.Append(")")

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


    Public Function Modifica(ByVal Piva As String,
                             ByVal Sa_Cod As Int32,
                             ByVal Appezza As Int32,
                             ByVal Id_Reg As Int32,
                             ByVal Progetto_Cod As Int32,
                             ByVal Programmazione_Cod As Int32,
                             ByVal Programmazione_Entita_Cod As Int32,
                             ByVal Validita_Inizio As Date,
                             ByVal Validita_Fine As Date,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreParametri
                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Reg_Impianti_Programmazioni_W.Modifica()"

        Dim messaggioErrore As String = ""
        Dim strSql As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            strSql.Length = 0
            strSql.Append("UPDATE Reg_Impianti_Programmazioni SET ")
            strSql.Append("    Programmazione_Cod           = " & Agro_SQL_SaveNum(Programmazione_Cod) & " ")
            strSql.Append("   ,Programmazione_Entita_Cod    = " & Agro_SQL_SaveNum(Programmazione_Entita_Cod) & " ")
            strSql.Append("   ,Inviato           =  0 ")
            strSql.Append("   ,DataInvio         =  Null ")
            strSql.Append("   ,Data_Modifica     =  " & Agro_SQL_SaveDate(Date.Now))
            strSql.Append("   ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            strSql.Append("   ,Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Inizio))
            strSql.Append("   ,Validita_Fine     =  " & Agro_SQL_SaveDate(Validita_Fine))
            strSql.Append(" WHERE Piva_SuperUser = '" & Agro_SQL_SaveText(Trim(objParametri.PivaSuperUser)) & "'")
            strSql.Append(" AND   Piva = '" & Agro_SQL_SaveText(Trim(Piva)) & "'")
            strSql.Append(" AND   Sa_Cod = " & Sa_Cod & " ")
            strSql.Append(" AND   Appezza = " & Appezza & " ")
            strSql.Append(" AND   Id_Reg = " & Id_Reg & " ")
            strSql.Append(" AND   Progetto_Cod = " & Progetto_Cod & " ")
            '---------------------------------------------
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


    Public Function Modifica_Parametrizzata(ByVal Piva As String,
                                            ByVal Sa_Cod As Int32,
                                            ByVal Appezza As Int32,
                                            ByVal Id_Reg As Int32,
                                            ByVal Progetto_Cod As Integer,
                                            ByVal Programmazione_Cod As Integer,
                                            ByVal Programmazione_Entita_Cod As Integer,
                                            ByVal Campo As String,
                                            ByVal Valore As Object,
                                            ByVal xFiltroAggiuntivo As String,
                                            ByRef objParametri As AgronicaCoreParametri
                                            ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Reg_Impianti_Programmazioni_W.Modifica_Parametrizzata()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False
        Dim strAssegnamento As String = String.Empty

        ' a seconda del tipo del valore che devo aggiornare, formatto la query
        Dim stringa As Type = GetType(System.String)
        Dim data As Type = GetType(System.DateTime)
        Dim intero32 As Type = GetType(System.Int32)

        Try
            If Piva = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
            End If

            If Sa_Cod = 0 Then
                Throw New Exception("Parametro non corretto nella query (Sa_Cod obbligatorio)")
            End If

            If Appezza = 0 Then
                Throw New Exception("Parametro non corretto nella query (Appezza obbligatorio)")
            End If

            If Id_Reg = 0 Then
                Throw New Exception("Parametro non corretto nella query (Id_Reg obbligatorio)")
            End If

            '---------------------------------------------

            Dim typeVal As Type = Valore.GetType()

            If typeVal.Equals(stringa) Then
                strAssegnamento = Campo & "= '" & Agro_SQL_SaveText(Valore.ToString) & "' "
            ElseIf typeVal.Equals(data) Then
                strAssegnamento = Campo & "= " & Agro_SQL_SaveDate(Valore.ToString) & " "
            Else
                strAssegnamento = Campo & "= " & Agro_SQL_SaveNum(Valore.ToString) & " "
            End If

            '---------------------------------------------

            StrSQL.Length = 0
            StrSQL.Append("UPDATE Reg_Impianti_Codici SET ")

            StrSQL.Append(strAssegnamento)

            StrSQL.Append("         ,Data_Modifica        =  " & Agro_SQL_SaveDate(Date.Now))
            StrSQL.Append("         ,UserName_Modifica    = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")


            StrSQL.Append(" WHERE Piva = '" & Agro_SQL_SaveText(Trim(Piva)) & "'")
            StrSQL.Append(" AND   Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            StrSQL.Append(" AND   Appezza = " & Agro_SQL_SaveNum(Appezza) & " ")
            StrSQL.Append(" AND   Id_Reg = " & Agro_SQL_SaveNum(Id_Reg) & " ")

            If Progetto_Cod <> 0 Then
                StrSQL.Append(" AND   Progetto_Cod = " & Agro_SQL_SaveNum(Progetto_Cod) & " ")
            End If

            If Programmazione_Cod <> 0 Then
                StrSQL.Append(" AND   Programmazione_Cod = " & Agro_SQL_SaveNum(Programmazione_Cod) & " ")
            End If

            If Programmazione_Entita_Cod <> 0 Then
                StrSQL.Append(" AND   Programmazione_Entita_Cod = " & Agro_SQL_SaveNum(Programmazione_Entita_Cod) & " ")
            End If

            '----------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function


    Public Function Modifica_Chiave(ByVal Piva As String,
                                    ByVal Sa_Cod As Int32,
                                    ByVal Appezza As Int32,
                                    ByVal Id_Reg As Int32,
                                    ByVal Piva_OLD As String,
                                    ByVal Sa_Cod_OLD As Int32,
                                    ByVal Appezza_OLD As Int32,
                                    ByVal Id_Reg_OLD As Int32,
                                    ByVal xFiltroAggiuntivo As String,
                                    ByRef objParametri As AgronicaCoreParametri
                                    ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Reg_Impianti_Programmazioni_W.Modifica_Chiave()"

        Dim messaggioErrore As String = ""
        Dim strSql As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            If Piva = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
            End If

            If Sa_Cod = 0 Then
                Throw New Exception("Parametro non corretto nella query (Sa_Cod obbligatorio)")
            End If

            If Appezza = 0 Then
                Throw New Exception("Parametro non corretto nella query (Appezza obbligatorio)")
            End If

            If Id_Reg = 0 Then
                Throw New Exception("Parametro non corretto nella query (Id_Reg obbligatorio)")
            End If

            strSql.Length = 0
            strSql.Append("UPDATE Reg_Impianti_Programmazioni SET ")

            strSql.Append(" Piva = '" & Agro_SQL_SaveText(Trim(Piva)) & "'")
            strSql.Append(" ,Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            strSql.Append(" ,Appezza = " & Agro_SQL_SaveNum(Appezza) & " ")
            strSql.Append(" ,Id_Reg = " & Agro_SQL_SaveNum(Id_Reg) & " ")

            strSql.Append("         ,Data_Modifica        =  " & Agro_SQL_SaveDate(Date.Now))
            strSql.Append("         ,UserName_Modifica    = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")

            strSql.Append(" WHERE Piva = '" & Agro_SQL_SaveText(Trim(Piva_OLD)) & "'")
            strSql.Append(" AND   Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod_OLD) & " ")
            strSql.Append(" AND   Appezza = " & Agro_SQL_SaveNum(Appezza_OLD) & " ")
            strSql.Append(" AND   Id_Reg = " & Agro_SQL_SaveNum(Id_Reg_OLD) & " ")

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


    Public Function Cancella(ByVal Piva As String,
                             ByVal Sa_Cod As Int32,
                             ByVal Appezza As Int32,
                             ByVal Id_Reg As Int32,
                             ByVal Progetto_Cod As Int32,
                             ByVal Programmazione_Cod As Int32,
                             ByVal Programmazione_Entita_Cod As Int32,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreParametri
                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Reg_Impianti_Programmazioni_W.Cancella()"

        Dim messaggioErrore As String = ""
        Dim strSql As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then
                strSql.Length = 0
                strSql.Append(" UPDATE Reg_Impianti_Programmazioni ")
                strSql.Append(" SET ")
                strSql.Append("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                strSql.Append("      ,Inviato = -1 ")
                strSql.Append(" WHERE  Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
                strSql.Append(" AND  Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
                strSql.Append(" AND Inviato > 0")
            Else
                strSql.Length = 0
                strSql.Append(" DELETE ")
                strSql.Append(" FROM     Reg_Impianti_Programmazioni ")
                strSql.Append(" WHERE  Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
                strSql.Append(" AND    Piva= '" & Agro_SQL_SaveText(Piva) & "' ")
                strSql.Append(" AND    Inviato = 0 ")
            End If

            If Sa_Cod <> 0 Then
                strSql.Append(" AND   Sa_Cod = " & Sa_Cod & " ")
            End If

            If Appezza <> 0 Then
                strSql.Append(" AND   Appezza = " & Appezza & " ")
            End If

            If Id_Reg <> 0 Then
                strSql.Append(" AND   Id_Reg = " & Id_Reg & " ")
            End If

            If Progetto_Cod <> 0 Then
                strSql.Append(" AND   Progetto_Cod = " & Progetto_Cod & " ")
            End If

            If Programmazione_Cod <> 0 Then
                strSql.Append(" AND   Programmazione_Cod = " & Agro_SQL_SaveNum(Programmazione_Cod) & " ")
            End If

            If Programmazione_Entita_Cod <> 0 Then
                strSql.Append(" AND Programmazione_Entita_Cod = " & Agro_SQL_SaveNum(Programmazione_Entita_Cod) & " ")
            End If

            '---------------------------------------------
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

End Class
