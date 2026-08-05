Imports System.Data
Imports System.Xml
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports System.Text

Public Class Statistiche_Utilizzo_R
    Inherits AgronicaCoreDataProvider.LogProvider



    Public Function getStatistiche(validita_inizio As Date?, validita_fine? As Date,
                                        piva As String, tipoFiltro As Integer,
                                        objParametri_Server As AgronicaCoreParametri,
                                        objParametri_Utenti As AgronicaCoreParametri) As Statistiche

        Dim statistiche_transazioni = New Statistiche_Transazioni
        Dim statistiche_aziende_movimentante = New Statistiche_Aziende_Movimentante

        Dim username = ""
        'imposto la finestra temporale
        Dim v_inizio As Date
        Dim v_fine As Date

        If IsDate(validita_inizio.Value) Then
            v_inizio = validita_inizio.Value
        Else
            v_inizio = AGRODATAINIZIO
        End If

        If IsDate(validita_fine.Value) Then
            v_fine = validita_fine.Value
        Else
            v_fine = AGRODATAFINE
        End If

        '--------------------------------------------------------
        '--------------------------------------------------------
        'AGENDA TOTALE + DISTINCT
        '--------------------------------------------------------
        '--------------------------------------------------------
        Dim FiltroAggiuntivo_Imprese As String = ""
        Dim FiltroAggiuntivo_Imprese_Agenda As String = ""
        Dim FiltroAggiuntivo_Imprese_Audit As String = ""
        Dim FiltroAggiuntivo_Imprese_Pua As String = ""
        Dim FiltroAggiuntivo_Imprese_Planning As String = ""
        Dim FiltroAggiuntivo_Imprese_Ricette As String = ""
        Dim FiltroAggiuntivo_Imprese_Notifica As String = ""
        Dim FiltroAggiuntivo_Imprese_Pap As String = ""
        Dim FiltroAggiuntivo_Imprese_PapZoo As String = ""


        '----------------------------------------------------------------
        '--- Filtro associato all'utente 
        '----------------------------------------------------------------
        Dim UtenteFiltro As String = ""
        Dim objUtentiVisibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
        Dim DtImpreseVisibili = objUtentiVisibilita.Leggi(enum_TipoEntita.Impresa, "", "", objParametri_Server)

        Dim Applica_VisibilitaUtente As Boolean = False

        If Not DtImpreseVisibili Is Nothing AndAlso DtImpreseVisibili.Rows.Count > 0 Then
            Applica_VisibilitaUtente = True
        End If

        'Select Case piva
        '    Case "" 'nessuna selezione
        '        Select Case UtenteFiltro
        '            Case "" 'utente con visibilità totale --> non filtro nulla
        '            Case Else 'applico la visibilità dell'utente
        '        End Select
        '    Case Else
        '        'estraggo le imprese figlie della selezionata + applico il filtro visibilita utente
        'End Select

        If piva <> "" And piva <> objParametri_Server.PivaSuperUser Then
            Dim objger As New AgronicaCoreAnagrafeDAL.GerarchiaImprese_R
            objger.LeggiFigliNodoGerarchiaImprese(piva, FiltroAggiuntivo_Imprese, objParametri_Server)
            If FiltroAggiuntivo_Imprese <> "" Then
                FiltroAggiuntivo_Imprese = FiltroAggiuntivo_Imprese & ",'" & piva & "'"
            Else
                FiltroAggiuntivo_Imprese = "'" & piva & "'"
            End If
        End If

        If FiltroAggiuntivo_Imprese <> "" Then
            FiltroAggiuntivo_Imprese_Agenda = " Agenda.PIVA in (" & FiltroAggiuntivo_Imprese & ")"
            FiltroAggiuntivo_Imprese_Audit = " Audit.PIVA in (" & Agro_SQL_Save_Clausola_IN(FiltroAggiuntivo_Imprese, True) & ")"
            FiltroAggiuntivo_Imprese_Pua = " Pua_Testata.PIVA in (" & Agro_SQL_Save_Clausola_IN(FiltroAggiuntivo_Imprese, True) & ")"
            FiltroAggiuntivo_Imprese_Planning = " Programmazione_Testata.PIVA in (" & Agro_SQL_Save_Clausola_IN(FiltroAggiuntivo_Imprese, True) & ")"
            FiltroAggiuntivo_Imprese_Ricette = " Ricette.PIVA in (" & Agro_SQL_Save_Clausola_IN(FiltroAggiuntivo_Imprese, True) & ")"
            FiltroAggiuntivo_Imprese_Notifica = " BIO_Notifica_Frontespizio.Notifica_Piva in (" & Agro_SQL_Save_Clausola_IN(FiltroAggiuntivo_Imprese, True) & ")"
            FiltroAggiuntivo_Imprese_Pap = " CDX_PAP.Piva in (" & Agro_SQL_Save_Clausola_IN(FiltroAggiuntivo_Imprese, True) & ")"
            FiltroAggiuntivo_Imprese_PapZoo = " CDX_PAPZeta.PAP_Piva in (" & Agro_SQL_Save_Clausola_IN(FiltroAggiuntivo_Imprese, True) & ")"
        End If

        Dim N_Agenda_Tot As Integer = 0
        Dim N_Agenda_Distinct As Integer = 0
        Dim DT_Agenda_Tot As DataTable
        Dim DT_Agenda_Distinct As DataTable
        Dim objAgenda As New AgronicaCoreContabDAL.Agenda_R

        DT_Agenda_Tot = objAgenda.Leggi_con_Movimenti_xStatistiche(tipoFiltro, v_inizio, v_fine,
                                                                   username,
                                                               FiltroAggiuntivo_Imprese_Agenda, "", objParametri_Server,
                                                                Applica_VisibilitaUtente)

        N_Agenda_Tot = DT_Agenda_Tot.Rows.Count

        DT_Agenda_Distinct = objAgenda.Leggi_con_Movimenti_xStatistiche_DistinctPiva(tipoFiltro, v_inizio, v_fine,
                                                                                     username,
                                                                                        FiltroAggiuntivo_Imprese_Agenda, "", objParametri_Server,
                                                                                        Applica_VisibilitaUtente)

        N_Agenda_Distinct = DT_Agenda_Distinct.Rows.Count

        'Dim Carichi_Agenda As Integer
        'Dim Scarico_Agenda As Integer
        'Dim Trattamenti_Agenda As Integer
        'Dim Concimazioni_Agenda As Integer

        'Dim DR() As DataRow

        'Dim str_user As String
        'If username <> "" Then
        '    str_user = " AND Username_Creazione = '" & username & "'"
        'End If


        'DR = DT.Select(" Cau_Mov = '7300' " & str_user)
        'Carichi_Agenda = DR.Length

        'DR = DT.Select(" Cau_Mov = '7350' " & str_user)
        'Scarico_Agenda = DR.Length

        'DR = DT.Select(" Cau_Mov = '2050' " & str_user)
        'Trattamenti_Agenda = DR.Length

        'DR = DT.Select(" Cau_Mov = '2300' " & str_user)
        'Concimazioni_Agenda = DR.Length


        Dim objSQL As New AgronicaCoreDataProvider.DataProvider
        Dim Dt As DataTable

        Dim Pua_N_Tot As Integer = 0
        Dim Planning_N_Tot As Integer = 0
        Dim Notifiche_N_Tot As Integer = 0
        Dim Pap_N_Tot As Integer = 0
        Dim PapZoo_N_Tot As Integer = 0
        Dim Ricette_N_Tot As Integer = 0
        Dim Pua_N_Distinct As Integer = 0
        Dim Planning_N_Distinct As Integer = 0
        Dim Notifiche_N_Distinct As Integer = 0
        Dim Pap_N_Distinct As Integer = 0
        Dim PapZoo_N_Distinct As Integer = 0
        Dim Ricette_N_Distinct As Integer = 0

        Dim sql_v As String

        '---------------------------
        ' FILTRI IMPOSTATI

        If IsDate(validita_inizio.Value) Then
            Select Case tipoFiltro
                Case 1
                    sql_v = " Validita_Inizio >= " & Agro_SQL_SaveDate(validita_inizio.Value)
                Case Else
                    sql_v = " data_creazione >= " & Agro_SQL_SaveDate(validita_inizio.Value)
            End Select
        End If

        If IsDate(validita_fine.Value) Then

            Select Case tipoFiltro

                Case 1

                    If sql_v = "" Then
                        sql_v = " Validita_Inizio <= " & Agro_SQL_SaveDate(validita_fine.Value)
                    Else
                        sql_v = sql_v & " AND Validita_Inizio <= " & Agro_SQL_SaveDate(validita_fine.Value)
                    End If

                Case Else

                    If sql_v = "" Then
                        sql_v = " data_creazione <= " & Agro_SQL_SaveDate(validita_fine.Value)
                    Else
                        sql_v = sql_v & " AND data_creazione <= " & Agro_SQL_SaveDate(validita_fine.Value)
                    End If

            End Select

        End If



        Dim str_Username As String
        If username <> "" Then
            str_Username = " Username_Creazione = '" & username & "'"
        End If

        Dim str_agg As String
        If sql_v = "" Then
            If str_Username <> "" Then
                str_agg = " WHERE " & str_Username
            End If
        Else
            str_agg = " WHERE " & sql_v
            If str_Username <> "" Then
                str_agg = str_agg & " AND  " & str_Username
            End If

        End If

        '--------------------------------------------------------
        '--------------------------------------------------------
        'AUDIT-SICUREZZA-ALTRE CHECk TOTALE + DISTINCT
        '--------------------------------------------------------
        '--------------------------------------------------------

        Dim Condizionalita_N_Tot As Integer
        Dim Sicurezza_N_Tot As Integer
        Dim AltreAudit_N_Tot As Integer
        Dim Condizionalita_N_Distinct As Integer
        Dim Sicurezza_N_Distinct As Integer
        Dim AltreAudit_N_Distinct As Integer


        Dim sql_Audit As String
        Dim sql_Audit1 As String
        Dim sql_Audit2 As String
        Dim sql_Audit3 As String

        sql_Audit = "SELECT  * from Audit " & vbCrLf

        If Applica_VisibilitaUtente = True Then
            sql_Audit &= " INNER JOIN Utenti_Visibilita_Appoggio On Audit.PIVA = Utenti_Visibilita_Appoggio.PIVA And Entita_Cod = 1 And username ='" & objParametri_Server.UtenteUsername & "'" & vbCrLf
        End If

        sql_Audit &= str_agg & vbCrLf


        If FiltroAggiuntivo_Imprese_Audit <> "" Then
            If sql_Audit.Contains("WHERE") = True Then
                sql_Audit = sql_Audit & " AND " & FiltroAggiuntivo_Imprese_Audit
            Else
                sql_Audit = sql_Audit & " WHERE " & FiltroAggiuntivo_Imprese_Audit
            End If
        End If

        If sql_Audit.Contains("WHERE") = True Then
            sql_Audit1 = sql_Audit & " AND Audit_Tipo=1 "
            sql_Audit2 = sql_Audit & " AND Audit_Tipo=3 "
            sql_Audit3 = sql_Audit & " AND Audit_Tipo NOT IN (1,3) "
        Else
            sql_Audit1 = sql_Audit & " WHERE Audit_Tipo=1 "
            sql_Audit2 = sql_Audit & " WHERE Audit_Tipo=3 "
            sql_Audit3 = sql_Audit & " WHERE Audit_Tipo NOT IN (1,3) "
        End If


        Dt = objSQL.EseguiQuery_Lettura(objParametri_Server, sql_Audit1, "Audit")
        Condizionalita_N_Tot = Dt.Rows.Count
        Dt = objSQL.EseguiQuery_Lettura(objParametri_Server, sql_Audit2, "Sicurezza")
        Sicurezza_N_Tot = Dt.Rows.Count
        Dt = objSQL.EseguiQuery_Lettura(objParametri_Server, sql_Audit3, "Altre")
        AltreAudit_N_Tot = Dt.Rows.Count


        'AUDIT Distinct 
        sql_Audit = "SELECT DISTINCT Audit.Piva from Audit " & vbCrLf

        If Applica_VisibilitaUtente = True Then
            sql_Audit &= " INNER JOIN Utenti_Visibilita_Appoggio On Audit.PIVA = Utenti_Visibilita_Appoggio.PIVA And Entita_Cod = 1 And username ='" & objParametri_Server.UtenteUsername & "'" & vbCrLf
        End If

        sql_Audit &= str_agg & vbCrLf


        If FiltroAggiuntivo_Imprese_Audit <> "" Then
            If sql_Audit.Contains("WHERE") = True Then
                sql_Audit = sql_Audit & " AND " & FiltroAggiuntivo_Imprese_Audit
            Else
                sql_Audit = sql_Audit & " WHERE " & FiltroAggiuntivo_Imprese_Audit
            End If
        End If

        If sql_Audit.Contains("WHERE") = True Then
            sql_Audit1 = sql_Audit & " AND Audit_Tipo=1 "
            sql_Audit2 = sql_Audit & " AND Audit_Tipo=3 "
            sql_Audit3 = sql_Audit & " AND Audit_Tipo NOT IN (1,3) "
        Else
            sql_Audit1 = sql_Audit & " WHERE Audit_Tipo=1 "
            sql_Audit2 = sql_Audit & " WHERE Audit_Tipo=3 "
            sql_Audit3 = sql_Audit & " WHERE Audit_Tipo NOT IN (1,3) "
        End If


        Dt = objSQL.EseguiQuery_Lettura(objParametri_Server, sql_Audit1, "Audit")
        Condizionalita_N_Distinct = Dt.Rows.Count
        Dt = objSQL.EseguiQuery_Lettura(objParametri_Server, sql_Audit2, "Sicurezza")
        Sicurezza_N_Distinct = Dt.Rows.Count
        Dt = objSQL.EseguiQuery_Lettura(objParametri_Server, sql_Audit3, "Altre")
        AltreAudit_N_Distinct = Dt.Rows.Count

        '--------------------------------------------------------
        '--------------------------------------------------------
        ' PUA TOTALI + DISTINCT
        '--------------------------------------------------------
        '--------------------------------------------------------

        ', DATEADD(yy, DATEDIFF(yy,0,CAST( (cast([PUA_Anno] as varchar(10)))  AS DATETIME)), 0)
        ', DATEADD(dd,-1,DATEADD(yy,0,DATEADD(yy,DATEDIFF(yy,0,CAST( (cast([PUA_Anno] as varchar(10)))  AS DATETIME))+1,0)))

        Dim sql_Pua As String
        sql_Pua = " SELECT  * from Pua_Testata " & vbCrLf

        If Applica_VisibilitaUtente = True Then
            sql_Pua &= " INNER JOIN Utenti_Visibilita_Appoggio On Pua_Testata.PIVA = Utenti_Visibilita_Appoggio.PIVA And Entita_Cod = 1 And username ='" & objParametri_Server.UtenteUsername & "'" & vbCrLf
        End If


        Select Case tipoFiltro
            Case 1
                sql_Pua &= " WHERE  DATEADD(dd,-1,DATEADD(yy,0,DATEADD(yy,DATEDIFF(yy,0,CAST( (cast([PUA_Anno] as varchar(10)))  AS DATETIME))+1,0))) >= " & Agro_SQL_SaveDate(v_inizio)
                sql_Pua &= " AND    DATEADD(yy, DATEDIFF(yy,0,CAST( (cast([PUA_Anno] as varchar(10)))  AS DATETIME)), 0) <= " & Agro_SQL_SaveDate(v_fine)
            Case Else
                sql_Pua &= " WHERE data_creazione <= " & Agro_SQL_SaveDate(v_fine)
                sql_Pua &= " AND   data_creazione >= " & Agro_SQL_SaveDate(v_inizio)
        End Select

        If str_Username <> "" Then
            sql_Pua &= " AND  " & str_Username
        End If
        If FiltroAggiuntivo_Imprese_Pua <> "" Then
            sql_Pua &= " AND " & FiltroAggiuntivo_Imprese_Pua
        End If

        Dt = objSQL.EseguiQuery_Lettura(objParametri_Server, sql_Pua, "PUA")
        Pua_N_Tot = Dt.Rows.Count

        sql_Pua = " SELECT DISTINCT Pua_Testata.Piva from Pua_Testata " & vbCrLf

        If Applica_VisibilitaUtente = True Then
            sql_Pua &= " INNER JOIN Utenti_Visibilita_Appoggio On Pua_Testata.PIVA = Utenti_Visibilita_Appoggio.PIVA And Entita_Cod = 1 And username ='" & objParametri_Server.UtenteUsername & "'" & vbCrLf
        End If


        Select Case tipoFiltro
            Case 1
                sql_Pua &= " WHERE  DATEADD(dd,-1,DATEADD(yy,0,DATEADD(yy,DATEDIFF(yy,0,CAST( (cast([PUA_Anno] as varchar(10)))  AS DATETIME))+1,0))) >= " & Agro_SQL_SaveDate(v_inizio)
                sql_Pua &= " AND    DATEADD(yy, DATEDIFF(yy,0,CAST( (cast([PUA_Anno] as varchar(10)))  AS DATETIME)), 0) <= " & Agro_SQL_SaveDate(v_fine)
            Case Else
                sql_Pua &= " WHERE data_creazione <= " & Agro_SQL_SaveDate(v_fine)
                sql_Pua &= " AND   data_creazione >= " & Agro_SQL_SaveDate(v_inizio)
        End Select
        If str_Username <> "" Then
            sql_Pua &= " AND  " & str_Username
        End If
        If FiltroAggiuntivo_Imprese_Pua <> "" Then
            sql_Pua &= " AND " & FiltroAggiuntivo_Imprese_Pua
        End If

        Dt = objSQL.EseguiQuery_Lettura(objParametri_Server, sql_Pua, "PUA")
        Pua_N_Distinct = Dt.Rows.Count

        '--------------------------------------------------------
        '--------------------------------------------------------
        ' PLANNING TOTALI + DISTINCT
        '--------------------------------------------------------
        '--------------------------------------------------------
        Dim sql_Planning As String
        sql_Planning = " SELECT  * from Programmazione_Testata " & vbCrLf

        If Applica_VisibilitaUtente = True Then
            sql_Planning &= " INNER JOIN Utenti_Visibilita_Appoggio On Programmazione_Testata.PIVA = Utenti_Visibilita_Appoggio.PIVA And Entita_Cod = 1 And username ='" & objParametri_Server.UtenteUsername & "'" & vbCrLf
        End If


        Select Case tipoFiltro
            Case 1
                sql_Planning &= " WHERE validita_inizio <= " & Agro_SQL_SaveDate(v_fine)
                sql_Planning &= " AND   validita_fine >= " & Agro_SQL_SaveDate(v_inizio)
            Case Else
                sql_Planning &= " WHERE data_creazione <= " & Agro_SQL_SaveDate(v_fine)
                sql_Planning &= " AND   data_creazione >= " & Agro_SQL_SaveDate(v_inizio)
        End Select


        If str_Username <> "" Then
            sql_Planning &= " AND  " & str_Username
        End If
        If FiltroAggiuntivo_Imprese_Planning <> "" Then
            sql_Planning &= " AND " & FiltroAggiuntivo_Imprese_Planning
        End If

        Dt = objSQL.EseguiQuery_Lettura(objParametri_Server, sql_Planning, "Planning")
        Planning_N_Tot = Dt.Rows.Count

        sql_Planning = " SELECT DISTINCT Programmazione_Testata.Piva from Programmazione_Testata " & vbCrLf

        If Applica_VisibilitaUtente = True Then
            sql_Planning &= " INNER JOIN Utenti_Visibilita_Appoggio On Programmazione_Testata.PIVA = Utenti_Visibilita_Appoggio.PIVA And Entita_Cod = 1 And username ='" & objParametri_Server.UtenteUsername & "'" & vbCrLf
        End If


        Select Case tipoFiltro
            Case 1
                sql_Planning &= " WHERE validita_inizio <= " & Agro_SQL_SaveDate(v_fine)
                sql_Planning &= " AND   validita_fine >= " & Agro_SQL_SaveDate(v_inizio)
            Case Else
                sql_Planning &= " WHERE data_creazione <= " & Agro_SQL_SaveDate(v_fine)
                sql_Planning &= " AND   data_creazione >= " & Agro_SQL_SaveDate(v_inizio)
        End Select

        If str_Username <> "" Then
            sql_Planning &= " AND  " & str_Username
        End If
        If FiltroAggiuntivo_Imprese_Planning <> "" Then
            sql_Planning &= " AND " & FiltroAggiuntivo_Imprese_Planning
        End If

        Dt = objSQL.EseguiQuery_Lettura(objParametri_Server, sql_Planning, "Planning")
        Planning_N_Distinct = Dt.Rows.Count


        '--------------------------------------------------------
        '--------------------------------------------------------
        ' RICETTE TOTALI + DISTINCT
        '--------------------------------------------------------
        '--------------------------------------------------------

        Dim sqln_ricette As String
        sqln_ricette = " SELECT  * from Ricette " & vbCrLf

        If Applica_VisibilitaUtente = True Then
            sqln_ricette &= " INNER JOIN Utenti_Visibilita_Appoggio On Ricette.PIVA = Utenti_Visibilita_Appoggio.PIVA And Entita_Cod = 1 And username ='" & objParametri_Server.UtenteUsername & "'" & vbCrLf
        End If

        sqln_ricette &= str_agg & vbCrLf

        If FiltroAggiuntivo_Imprese_Ricette <> "" Then
            If sqln_ricette.Contains("WHERE") = True Then
                sqln_ricette = sqln_ricette & " AND " & FiltroAggiuntivo_Imprese_Ricette
            Else
                sqln_ricette = sqln_ricette & " WHERE " & FiltroAggiuntivo_Imprese_Ricette
            End If
        End If

        Dt = objSQL.EseguiQuery_Lettura(objParametri_Server, sqln_ricette, "Ricette")
        Ricette_N_Tot = Dt.Rows.Count

        sqln_ricette = " SELECT  DISTINCT Ricette.Piva  from Ricette " & vbCrLf

        If Applica_VisibilitaUtente = True Then
            sqln_ricette &= " INNER JOIN Utenti_Visibilita_Appoggio On Ricette.PIVA = Utenti_Visibilita_Appoggio.PIVA And Entita_Cod = 1 And username ='" & objParametri_Server.UtenteUsername & "'" & vbCrLf
        End If

        sqln_ricette &= str_agg & vbCrLf

        If FiltroAggiuntivo_Imprese_Ricette <> "" Then
            If sqln_ricette.Contains("WHERE") = True Then
                sqln_ricette = sqln_ricette & " AND " & FiltroAggiuntivo_Imprese_Ricette
            Else
                sqln_ricette = sqln_ricette & " WHERE " & FiltroAggiuntivo_Imprese_Ricette
            End If
        End If

        Dt = objSQL.EseguiQuery_Lettura(objParametri_Server, sqln_ricette, "Ricette")
        Ricette_N_Distinct = Dt.Rows.Count


        '--------------------------------------------------------
        '--------------------------------------------------------
        ' NOTIFICHE TOTALI + DISTINCT
        '--------------------------------------------------------
        '--------------------------------------------------------

        If str_agg <> "" Then
            Select Case tipoFiltro
                Case 1
                    str_agg = Replace(str_agg, "Validita_Inizio", "Notifica_Fotografia_Data")
                Case Else
                    str_agg = Replace(str_agg, "data_creazione", "Notifica_Fotografia_Data")
            End Select

            'str_agg = Replace(str_agg, "Validita_Inizio", "Notifica_Fotografia_Data")
        End If

        Dim sqln_notifiche As String
        sqln_notifiche = " SELECT  * from BIO_Notifica_Frontespizio " & vbCrLf

        If Applica_VisibilitaUtente = True Then
            sqln_notifiche &= " INNER JOIN Utenti_Visibilita_Appoggio On BIO_Notifica_Frontespizio.Notifica_Piva = Utenti_Visibilita_Appoggio.PIVA And Entita_Cod = 1 And username ='" & objParametri_Server.UtenteUsername & "'" & vbCrLf
        End If

        sqln_notifiche &= str_agg & vbCrLf


        If FiltroAggiuntivo_Imprese_Notifica <> "" Then
            If sqln_notifiche.Contains("WHERE") = True Then
                sqln_notifiche = sqln_notifiche & " AND " & FiltroAggiuntivo_Imprese_Notifica
            Else
                sqln_notifiche = sqln_notifiche & " WHERE " & FiltroAggiuntivo_Imprese_Notifica
            End If
        End If

        Dt = objSQL.EseguiQuery_Lettura(objParametri_Server, sqln_notifiche, "Notifiche")
        Notifiche_N_Tot = Dt.Rows.Count

        sqln_notifiche = " SELECT  DISTINCT Notifica_Piva  from BIO_Notifica_Frontespizio " & vbCrLf

        If Applica_VisibilitaUtente = True Then
            sqln_notifiche &= " INNER JOIN Utenti_Visibilita_Appoggio On BIO_Notifica_Frontespizio.Notifica_Piva = Utenti_Visibilita_Appoggio.PIVA And Entita_Cod = 1 And username ='" & objParametri_Server.UtenteUsername & "'" & vbCrLf
        End If

        sqln_notifiche &= str_agg & vbCrLf

        If FiltroAggiuntivo_Imprese_Notifica <> "" Then
            If sqln_notifiche.Contains("WHERE") = True Then
                sqln_notifiche = sqln_notifiche & " AND " & FiltroAggiuntivo_Imprese_Notifica
            Else
                sqln_notifiche = sqln_notifiche & " WHERE " & FiltroAggiuntivo_Imprese_Notifica
            End If
        End If

        Dt = objSQL.EseguiQuery_Lettura(objParametri_Server, sqln_notifiche, "Notifiche")
        Notifiche_N_Distinct = Dt.Rows.Count


        '--------------------------------------------------------
        '--------------------------------------------------------
        ' PAP VEGETALI TOTALI + DISTINCT
        '--------------------------------------------------------
        '--------------------------------------------------------

        Dim sqln_pap As String
        sqln_pap = " SELECT  * from CDX_PAP " & vbCrLf

        If Applica_VisibilitaUtente = True Then
            sqln_pap &= " INNER JOIN Utenti_Visibilita_Appoggio On CDX_PAP.Piva = Utenti_Visibilita_Appoggio.PIVA And Entita_Cod = 1 And username ='" & objParametri_Server.UtenteUsername & "'" & vbCrLf
        End If


        Select Case tipoFiltro
            Case 1
                sqln_pap &= " WHERE  DATEADD(dd,-1,DATEADD(yy,0,DATEADD(yy,DATEDIFF(yy,0,CAST( (cast([Anno] as varchar(10)))  AS DATETIME))+1,0))) >= " & Agro_SQL_SaveDate(v_inizio)
                sqln_pap &= " AND    DATEADD(yy, DATEDIFF(yy,0,CAST( (cast([Anno] as varchar(10)))  AS DATETIME)), 0) <= " & Agro_SQL_SaveDate(v_fine)
            Case Else
                sqln_pap &= " WHERE data_creazione <= " & Agro_SQL_SaveDate(v_fine)
                sqln_pap &= " AND   data_creazione >= " & Agro_SQL_SaveDate(v_inizio)
        End Select
        If str_Username <> "" Then
            sqln_pap &= " AND  " & str_Username
        End If
        If FiltroAggiuntivo_Imprese_Pap <> "" Then
            sqln_pap &= " AND " & FiltroAggiuntivo_Imprese_Pap
        End If

        Dt = objSQL.EseguiQuery_Lettura(objParametri_Server, sqln_pap, "PAP Vegetale")
        Pap_N_Tot = Dt.Rows.Count

        sqln_pap = " SELECT DISTINCT CDX_PAP.Piva from CDX_PAP " & vbCrLf

        If Applica_VisibilitaUtente = True Then
            sqln_pap &= " INNER JOIN Utenti_Visibilita_Appoggio On CDX_PAP.Piva = Utenti_Visibilita_Appoggio.PIVA And Entita_Cod = 1 And username ='" & objParametri_Server.UtenteUsername & "'" & vbCrLf
        End If

        Select Case tipoFiltro
            Case 1
                sqln_pap &= " WHERE  DATEADD(dd,-1,DATEADD(yy,0,DATEADD(yy,DATEDIFF(yy,0,CAST( (cast([Anno] as varchar(10)))  AS DATETIME))+1,0))) >= " & Agro_SQL_SaveDate(v_inizio)
                sqln_pap &= " AND    DATEADD(yy, DATEDIFF(yy,0,CAST( (cast([Anno] as varchar(10)))  AS DATETIME)), 0) <= " & Agro_SQL_SaveDate(v_fine)
            Case Else
                sqln_pap &= " WHERE data_creazione <= " & Agro_SQL_SaveDate(v_fine)
                sqln_pap &= " AND   data_creazione >= " & Agro_SQL_SaveDate(v_inizio)
        End Select
        If str_Username <> "" Then
            sqln_pap &= " AND  " & str_Username
        End If
        If FiltroAggiuntivo_Imprese_Pap <> "" Then
            sqln_pap &= " AND " & FiltroAggiuntivo_Imprese_Pap
        End If

        Dt = objSQL.EseguiQuery_Lettura(objParametri_Server, sqln_pap, "PAP Vegetale")
        Pap_N_Distinct = Dt.Rows.Count


        '--------------------------------------------------------
        '--------------------------------------------------------
        ' PAP ZOOTECNICI TOTALI + DISTINCT
        '--------------------------------------------------------
        '--------------------------------------------------------

        Dim sqln_papzoo As String
        sqln_papzoo = " SELECT  * from CDX_PAPZeta " & vbCrLf

        If Applica_VisibilitaUtente = True Then
            sqln_papzoo &= " INNER JOIN Utenti_Visibilita_Appoggio On CDX_PAPZeta.PAP_Piva = Utenti_Visibilita_Appoggio.PIVA And Entita_Cod = 1 And username ='" & objParametri_Server.UtenteUsername & "'" & vbCrLf
        End If


        Select Case tipoFiltro
            Case 1
                sqln_papzoo &= " WHERE  DATEADD(dd,-1,DATEADD(yy,0,DATEADD(yy,DATEDIFF(yy,0,CAST( (cast([Anno] as varchar(10)))  AS DATETIME))+1,0))) >= " & Agro_SQL_SaveDate(v_inizio)
                sqln_papzoo &= " AND    DATEADD(yy, DATEDIFF(yy,0,CAST( (cast([Anno] as varchar(10)))  AS DATETIME)), 0) <= " & Agro_SQL_SaveDate(v_fine)
            Case Else
                sqln_papzoo &= " WHERE data_creazione <= " & Agro_SQL_SaveDate(v_fine)
                sqln_papzoo &= " AND   data_creazione >= " & Agro_SQL_SaveDate(v_inizio)
        End Select
        If str_Username <> "" Then
            sqln_papzoo &= " AND  " & str_Username
        End If
        If FiltroAggiuntivo_Imprese_PapZoo <> "" Then
            sqln_papzoo &= " AND " & FiltroAggiuntivo_Imprese_PapZoo
        End If

        Dt = objSQL.EseguiQuery_Lettura(objParametri_Server, sqln_papzoo, "PAP Zootecnico")
        PapZoo_N_Tot = Dt.Rows.Count

        sqln_papzoo = " SELECT DISTINCT PAP_Piva from CDX_PAPZeta " & vbCrLf

        If Applica_VisibilitaUtente = True Then
            sqln_papzoo &= " INNER JOIN Utenti_Visibilita_Appoggio On CDX_PAPZeta.PAP_Piva = Utenti_Visibilita_Appoggio.PIVA And Entita_Cod = 1 And username ='" & objParametri_Server.UtenteUsername & "'" & vbCrLf
        End If

        Select Case tipoFiltro
            Case 1
                sqln_papzoo &= " WHERE  DATEADD(dd,-1,DATEADD(yy,0,DATEADD(yy,DATEDIFF(yy,0,CAST( (cast([Anno] as varchar(10)))  AS DATETIME))+1,0))) >= " & Agro_SQL_SaveDate(v_inizio)
                sqln_papzoo &= " AND    DATEADD(yy, DATEDIFF(yy,0,CAST( (cast([Anno] as varchar(10)))  AS DATETIME)), 0) <= " & Agro_SQL_SaveDate(v_fine)
            Case Else
                sqln_papzoo &= " WHERE data_creazione <= " & Agro_SQL_SaveDate(v_fine)
                sqln_papzoo &= " AND   data_creazione >= " & Agro_SQL_SaveDate(v_inizio)
        End Select
        If str_Username <> "" Then
            sqln_papzoo &= " AND  " & str_Username
        End If
        If FiltroAggiuntivo_Imprese_PapZoo <> "" Then
            sqln_papzoo &= " AND " & FiltroAggiuntivo_Imprese_PapZoo
        End If

        Dt = objSQL.EseguiQuery_Lettura(objParametri_Server, sqln_papzoo, "PAP Zootecnico")
        PapZoo_N_Distinct = Dt.Rows.Count



        ''STATISTICHE TRANSAZIONI

        statistiche_transazioni.agenda_tot = New Tuple(Of String, Integer)("Quaderni di Campagna e Magazzino", N_Agenda_Tot)
        statistiche_transazioni.condi_tot = New Tuple(Of String, Integer)("Condizionalità", Condizionalita_N_Tot)
        statistiche_transazioni.sicurezza_tot = New Tuple(Of String, Integer)("Sicurezza sul Lavoro", Sicurezza_N_Tot)
        statistiche_transazioni.altrecondi_tot = New Tuple(Of String, Integer)("Check List Agronomiche", AltreAudit_N_Tot)
        statistiche_transazioni.pua_tot = New Tuple(Of String, Integer)("Pua", Pua_N_Tot)
        statistiche_transazioni.planning_tot = New Tuple(Of String, Integer)("Piani Colturali", Planning_N_Tot)
        statistiche_transazioni.ricette_tot = New Tuple(Of String, Integer)("Ricette", Ricette_N_Tot)
        statistiche_transazioni.notifiche_tot = New Tuple(Of String, Integer)("Notifiche", Notifiche_N_Tot)
        statistiche_transazioni.pap_tot = New Tuple(Of String, Integer)("Pap Vegetali", Pap_N_Tot)
        statistiche_transazioni.papzoo_tot = New Tuple(Of String, Integer)("Pap Zootecnici", PapZoo_N_Tot)


        statistiche_aziende_movimentante.agenda_distinct = New Tuple(Of String, Integer)("Quaderni di Campagna e Magazzino", N_Agenda_Distinct)
        statistiche_aziende_movimentante.condi_distinct = New Tuple(Of String, Integer)("Condizionalità", Condizionalita_N_Distinct)
        statistiche_aziende_movimentante.sicurezza_distinct = New Tuple(Of String, Integer)("Sicurezza sul Lavoro", Sicurezza_N_Distinct)
        statistiche_aziende_movimentante.altrecondi_distinct = New Tuple(Of String, Integer)("Check List Agronomiche", AltreAudit_N_Distinct)
        statistiche_aziende_movimentante.pua_distinct = New Tuple(Of String, Integer)("Pua", Pua_N_Distinct)
        statistiche_aziende_movimentante.planning_distinct = New Tuple(Of String, Integer)("Piani Colturali", Planning_N_Distinct)
        statistiche_aziende_movimentante.ricette_distinct = New Tuple(Of String, Integer)("Ricette", Ricette_N_Distinct)
        statistiche_aziende_movimentante.notifiche_distinct = New Tuple(Of String, Integer)("Notifiche", Notifiche_N_Distinct)
        statistiche_aziende_movimentante.pap_distinct = New Tuple(Of String, Integer)("Pap Vegetali", Pap_N_Distinct)
        statistiche_aziende_movimentante.papzoo_distinct = New Tuple(Of String, Integer)("Pap Zootecnici", PapZoo_N_Distinct)



        ''STATISTICHE SERVIZI

        'statistiche_aziende_movimentante.agenda_distinct = New Tuple(Of String, Integer)("", N_Agenda_Distinct)
        'statistiche_aziende_movimentante.condi_distinct = New Tuple(Of String, Integer)("", Condizionalita_N_Distinct)
        'statistiche_aziende_movimentante.sicurezza_distinct = New Tuple(Of String, Integer)("", Sicurezza_N_Distinct)
        'statistiche_aziende_movimentante.altrecondi_distinct = New Tuple(Of String, Integer)("", AltreAudit_N_Distinct)
        'statistiche_aziende_movimentante.pua_distinct = New Tuple(Of String, Integer)("", Pua_N_Distinct)
        'statistiche_aziende_movimentante.planning_distinct = New Tuple(Of String, Integer)("", Planning_N_Distinct)
        'statistiche_aziende_movimentante.ricette_distinct = New Tuple(Of String, Integer)("", Ricette_N_Distinct)
        'statistiche_aziende_movimentante.notifiche_distinct = New Tuple(Of String, Integer)("", Notifiche_N_Distinct)
        'statistiche_aziende_movimentante.pap_distinct = New Tuple(Of String, Integer)("", Pap_N_Distinct)
        'statistiche_aziende_movimentante.papzoo_distinct = New Tuple(Of String, Integer)("", PapZoo_N_Distinct)






        'resetto
        objParametri_Server.ResettaFinestra()

        Dim statistiche = New Statistiche
        statistiche.transazioni = statistiche_transazioni
        statistiche.aziende_movimentante = statistiche_aziende_movimentante

        Return statistiche
    End Function



End Class


