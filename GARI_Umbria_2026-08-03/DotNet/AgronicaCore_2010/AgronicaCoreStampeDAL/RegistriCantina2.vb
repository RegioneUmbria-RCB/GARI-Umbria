Imports System.Text
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi


'###################################################################################
'###################################################################################
'###################################################################################
'###################################################################################
'###################################################################################
'###################################################################################
'#################         REGISTRO DEI FRIZZANTI       ############################
'#################         REGISTRO DEGLI SPUMANTI       ###########################
'###################################################################################
'###################################################################################
'###################################################################################
'###################################################################################
'###################################################################################
'###################################################################################
'###################################################################################
'###################################################################################
'###################################################################################



Public Class RegistriCantina2
    Inherits AgronicaCoreDataProvider.DataProvider


    '#####################################################################
    Public Function RegistroFrizzanti_AgendaFromTrasformazione(ByVal Piva As String,
                                                               ByVal Id_Trasformazione As Integer,
                                                               ByVal Tipo_Default As Integer,
                                                               ByVal xFiltroAggiuntivo As String,
                                                               ByVal xOrderBy As String,
                                                               ByRef objParametri As AgronicaCoreParametri
                                                               ) As DataTable

        Const nomeRoutine = "AgronicaCoreStampeDAL.RegistriCantina2.RegistroFrizzanti_AgendaFromTrasformazione"

        Dim messaggioErrore As String = ""
        Dim stbQ As New StringBuilder
        Dim dt As DataTable

        Try

            stbQ.Length = 0

            stbQ.Append(" SELECT Id_Agenda " & vbCrLf)

            stbQ.Append(" FROM Agenda " & vbCrLf)
            stbQ.Append(" INNER JOIN Linee_Preparazioni ON Agenda.Preparazione_Cod = Linee_Preparazioni.Preparazione_Cod " & vbCrLf)
            stbQ.Append(" AND Agenda.Piva = Linee_Preparazioni.Piva " & vbCrLf)

            stbQ.Append(" WHERE Agenda.Piva = '" & Agro_SQL_SaveText(Piva) & "' " & vbCrLf)

            stbQ.Append(" AND (Agenda.id_trasformazione = " & Id_Trasformazione.ToString & vbCrLf)
            'stbQ.Append(" OR Agenda.id_trasformazione = ")
            'stbQ.Append("(SELECT Id_Trasformazione_Rif FROM Trasformazioni_Riferimenti WHERE Id_Trasformazione = " & Id_Trasformazione.ToString & "))" & vbCrLf)

            stbQ.Append(" OR Agenda.id_trasformazione IN ")
            stbQ.Append("(SELECT DISTINCT Id_Trasformazione_Rif FROM Trasformazioni_Riferimenti WHERE Id_Trasformazione = " & Id_Trasformazione.ToString & "))" & vbCrLf)


            If Tipo_Default <> 0 Then
                stbQ.Append(" AND Linee_Preparazioni.Tipo_Default = " & Tipo_Default.ToString & vbCrLf)
            End If

            If xFiltroAggiuntivo <> "" Then
                stbQ.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            If xOrderBy <> "" Then
                stbQ.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stbQ.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    '#####################################################################
    'xFiltroAggiuntivo varia in base alla sezione (acidificazioni, dolcificazioni, ecc) chiamante
    Public Function RegistroFrizzanti_ElencoIdAgenda(ByVal Piva As String,
                                                     ByVal Id_Trasformazione As Integer,
                                                     ByVal Tipo_Default As Integer,
                                                     ByVal xFiltroAggiuntivo As String,
                                                     ByVal xOrderBy As String,
                                                     ByRef objParametri As AgronicaCoreParametri
                                                     ) As String

        Const nomeRoutine = "AgronicaCoreStampeDAL.RegistriCantina2.RegistroFrizzanti_ElencoIdAgenda"

        Dim messaggioErrore As String = ""
        Dim dtAgenda As DataTable
        ' preparo la stringa che userò nella query successiva nella clausula del where
        Dim elencoAgenda As String = String.Empty

        Try

            dtAgenda = RegistroFrizzanti_AgendaFromTrasformazione(Piva,
                                                                  Id_Trasformazione,
                                                                  Tipo_Default,
                                                                  xFiltroAggiuntivo,
                                                                  xOrderBy,
                                                                  objParametri)

            Dim i As Integer
            If Not IsNothing(dtAgenda) AndAlso dtAgenda.Rows.Count > 0 Then
                For i = 0 To dtAgenda.Rows.Count - 1
                    elencoAgenda &= If(elencoAgenda <> String.Empty, ", ", "")
                    elencoAgenda &= dtAgenda.Rows(i).Item("Id_Agenda")
                Next
            End If

            dtAgenda.Dispose()
            dtAgenda = Nothing

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            elencoAgenda = ""
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return elencoAgenda

    End Function


    '#####################################################################
    Public Function RegistroFrizzanti_CostituzionePartita(ByVal Piva As String,
                                                          ByVal Id_Agenda As Integer,
                                                          ByRef objParametri As AgronicaCoreParametri
                                                          ) As DataTable

        Const nomeRoutine = "AgronicaCoreStampeDAL.RegistriCantina2.RegistroFrizzanti_CostituzionePartita"

        Dim messaggioErrore As String = ""
        Dim stbQ As New StringBuilder
        Dim dt As DataTable

        Try

            stbQ.Length = 0

            stbQ.Append(" SELECT agenda.linea_cod,Linea_Des, Movimenti.Id_Mov, Movimenti_dettagli.Id_Mov_Det, Movimenti.Cau_Mov, Movimenti.Mov_Desc, Movimenti.Data_Movimento, " & vbCrLf)
            stbQ.Append(" Movimenti_dettagli.Elem_Cod, Movimenti_dettagli.Pro_Cod, Movimenti_dettagli.Mat_Cod, Movimenti_dettagli.Mov_Det_Des, " & vbCrLf)
            stbQ.Append(" Movimenti_dettagli.Udm_Cod, Movimenti_dettagli.Qta, Movimenti_dettagli.Lotto, Materie_Prime.Mat_Des, Mov_Destinazioni.Id_Destinazione, " & vbCrLf)
            stbQ.Append(" Mov_Destinazioni.Tipo_Destinazione, Mov_Destinazioni.Qta AS Qta_Dest, Mov_Destinazioni.Qta2,  " & vbCrLf)

            stbQ.Append(" ISNULL( (SELECT 'Vasca ' + Cantina_Vasche.Identificativo " & vbCrLf)
            stbQ.Append(" FROM Cantina_Vasche  " & vbCrLf)
            stbQ.Append(" where Mov_Destinazioni.Piva = Cantina_Vasche.Piva " & vbCrLf)
            stbQ.Append(" AND Mov_Destinazioni.Sa_Cod = Cantina_Vasche.Sa_Cod " & vbCrLf)
            stbQ.Append(" AND Mov_Destinazioni.Id_Destinazione = Cantina_Vasche.Vas_Cod " & vbCrLf)
            stbQ.Append(" AND Mov_Destinazioni.Tipo_Destinazione= " & Agro_SQL_SaveNum(TIPO_DESTINAZIONE_VASCA) & " " & vbCrLf)
            stbQ.Append("  ) , 'Magazzino') as Identificativo, Trasformazione_Des, " & vbCrLf)
            stbQ.Append(" Linee_Produzioni.Cod_Contatto_Terzi, " & vbCrLf)
            stbQ.Append(" ISNULL ( " & vbCrLf)
            stbQ.Append(" (SELECT rag_soc + nome + ' ' + cognome " & vbCrLf)
            stbQ.Append(" FROM contatti " & vbCrLf)
            stbQ.Append(" WHERE Linee_Produzioni.Cod_Contatto_Terzi= Contatti.Cod_Contatto " & vbCrLf)
            stbQ.Append(" AND Contatti.Piva=Linee_Produzioni.piva " & vbCrLf)
            stbQ.Append(" -- escludo il caso che la piva c/terzi sia l'impresa stessa " & vbCrLf)
            stbQ.Append(" AND Linee_Produzioni.piva <> Contatti.Cod_Contatto " & vbCrLf)
            stbQ.Append("  ), '') AS ContoTerzi " & vbCrLf)
            stbQ.Append(" -- serve il TOP 1 perchè un'anagrafica è loggata su più linee " & vbCrLf)
            stbQ.Append(" , isnull((SELECT TOP 1 Tipo_Generazione " & vbCrLf)
            stbQ.Append(" FROM OGenerazioni_Anagrafe_Log " & vbCrLf)
            stbQ.Append(" WHERE Modulo_Generazione= " & Agro_SQL_SaveNum(enum_Omni_Modulo_Generazione.Cantine) & " " & vbCrLf)
            stbQ.Append(" AND Piva_SuperUser= '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' " & vbCrLf)
            stbQ.Append(" AND OGenerazioni_Anagrafe_Log.Elem_Cod=Materie_Prime.Elem_Cod " & vbCrLf)
            stbQ.Append(" AND OGenerazioni_Anagrafe_Log.Mat_Cod=Materie_Prime.Mat_Cod " & vbCrLf)
            stbQ.Append(" -- non faccio il filtro sulla linea (altrimenti se l'anagrafica è loggata in un'altra linea, invece che in questa, non viene su) " & vbCrLf)
            stbQ.Append(" -- AND OGenerazioni_Anagrafe_Log.Linea_Cod=agenda.linea_cod " & vbCrLf)
            stbQ.Append(" AND OGenerazioni_Anagrafe_Log.piva=agenda.piva " & vbCrLf)
            stbQ.Append(" ),0  ) AS Tipo_Generazione " & vbCrLf)
            stbQ.Append(" , isnull((SELECT TOP 1 Codice_Generazione " & vbCrLf)
            stbQ.Append(" FROM OGenerazioni_Anagrafe_Log " & vbCrLf)
            stbQ.Append(" WHERE Modulo_Generazione= " & Agro_SQL_SaveNum(enum_Omni_Modulo_Generazione.Cantine) & " " & vbCrLf)
            stbQ.Append(" AND Piva_SuperUser= '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' " & vbCrLf)
            stbQ.Append(" AND OGenerazioni_Anagrafe_Log.Elem_Cod=Materie_Prime.Elem_Cod " & vbCrLf)
            stbQ.Append(" AND OGenerazioni_Anagrafe_Log.Mat_Cod=Materie_Prime.Mat_Cod " & vbCrLf)
            stbQ.Append(" -- AND OGenerazioni_Anagrafe_Log.Linea_Cod=agenda.linea_cod " & vbCrLf)
            stbQ.Append(" AND OGenerazioni_Anagrafe_Log.piva=agenda.piva " & vbCrLf)
            stbQ.Append(" ),0  ) AS Codice_Generazione " & vbCrLf)
            stbQ.Append(" , isnull((SELECT TOP 1 Linea_cod_Des as Modello " & vbCrLf)
            stbQ.Append(" FROM Linee_Produzioni Modelli " & vbCrLf)
            stbQ.Append(" WHERE Modelli.Linea_Cod=Linee_Produzioni.Linea_Modello_Cod " & vbCrLf)
            stbQ.Append(" AND Modelli.piva = 'AAAAAAAAAAA' " & vbCrLf)
            stbQ.Append("  ),''  ) AS Modello, " & vbCrLf)
            stbQ.Append(" Materie_Prime.Qta_Extra " & vbCrLf)
            'stbQ.Append("  " + vbCrLf)
            'stbQ.Append("  " + vbCrLf)
            'stbQ.Append("  " + vbCrLf)

            stbQ.Append(" FROM Movimenti " & vbCrLf)
            stbQ.Append(" INNER JOIN Movimenti_dettagli ON Movimenti.PIVA = Movimenti_dettagli.PIVA AND Movimenti.Id_Agenda = Movimenti_dettagli.Id_Agenda AND " & vbCrLf)
            stbQ.Append(" Movimenti.Id_Mov = Movimenti_dettagli.Id_Mov " & vbCrLf)
            stbQ.Append(" INNER JOIN Materie_Prime ON Movimenti_dettagli.Elem_Cod = Materie_Prime.Elem_Cod AND Movimenti_dettagli.Mat_Cod = Materie_Prime.Mat_Cod " & vbCrLf)
            stbQ.Append(" INNER JOIN Mov_Destinazioni ON Movimenti_dettagli.PIVA = Mov_Destinazioni.Piva AND Movimenti_dettagli.Id_Agenda = Mov_Destinazioni.Id_Agenda AND " & vbCrLf)
            stbQ.Append(" Movimenti_dettagli.Id_Mov = Mov_Destinazioni.Id_Mov AND Movimenti_dettagli.Id_Mov_Det = Mov_Destinazioni.Id_Mov_Det " & vbCrLf)

            ''le vasche sono in left join per gestire la frizzantatura in bottiglia -> spostato sopra
            'stbQ.Append(" LEFT OUTER JOIN Cantina_Vasche ON Mov_Destinazioni.Piva = Cantina_Vasche.Piva AND Mov_Destinazioni.Sa_Cod = Cantina_Vasche.Sa_Cod AND " + vbCrLf)
            'stbQ.Append(" Mov_Destinazioni.Id_Destinazione = Cantina_Vasche.Vas_Cod" + vbCrLf)
            stbQ.Append(" INNER JOIN Agenda ON Movimenti.PIVA = Agenda.PIVA AND Movimenti.Id_Agenda = Agenda.Id_Agenda  " & vbCrLf)
            stbQ.Append(" INNER JOIN Linee_Produzioni ON Linee_Produzioni.PIVA = Agenda.PIVA AND Linee_Produzioni.linea_cod = Agenda.linea_cod  " & vbCrLf)

            stbQ.Append(" INNER JOIN Trasformazioni ON Agenda.PIVA=Trasformazioni.piva " & vbCrLf)
            stbQ.Append(" AND Agenda.ID_TRASFORMAZIONE=Trasformazioni.Id_Trasformazione " & vbCrLf)

            '---- NOTA:
            '---- Materie_PrimexReport: non posso mettere il join con filtro id_report
            '   perché il vino non ha il check sul registro (sull'omni si possono impostare fino a 3 registri e al momento c'è vinif/comm/imbott)
            '   stbQ.Append(" INNER JOIN Materie_PrimexReport MPR ON MPR.Mat_Cod = MP.Mat_Cod " + vbCrLf)

            stbQ.Append(" WHERE Movimenti.Id_Agenda = " & Id_Agenda.ToString & " " & vbCrLf)
            stbQ.Append(" AND Movimenti.Piva = '" & Agro_SQL_SaveText(Piva) & "' " & vbCrLf)
            stbQ.Append(" AND (Movimenti.Cau_Mov = '" & CStr(CAU_CARICO) & "' OR Movimenti.Cau_Mov = '" & CStr(CAU_SCARICO) & "')" & vbCrLf)

            'stbQ.Append(" AND MPR.Id_Report IN ( " & vbCrLf)
            'stbQ.Append(" " & CStr(enum_AgroReportistica.Frizzanti) & ", " & vbCrLf)
            'stbQ.Append(" " & CStr(enum_AgroReportistica.Spumanti) & " " & vbCrLf)
            'stbQ.Append(" ) " & vbCrLf)

            stbQ.Append(" ORDER BY Movimenti.CAU_MOV DESC" & vbCrLf)

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stbQ.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function


    '#####################################################################
    Public Function RegistroFrizzanti_Acidificazioni(ByVal Piva As String,
                                                     ByVal Elenco_IdAgenda As String,
                                                     ByRef objParametri As AgronicaCoreParametri
                                                     ) As DataTable

        Const nomeRoutine = "AgronicaCoreStampeDAL.RegistriCantina2.RegistroFrizzanti_Acidificazioni"

        Dim messaggioErrore As String = ""
        Dim stbQ As New StringBuilder
        Dim dt As DataTable

        Try

            stbQ.Length = 0

            stbQ.Append(" SELECT M.Data_Movimento, MD.Elem_Cod, MD.Mat_Cod, MP.Mat_Des, MD.Qta, MD.Udm_Cod, UM.UDM_SIM " & vbCrLf)
            stbQ.Append(" FROM  Agenda A " & vbCrLf)
            stbQ.Append(" INNER JOIN Movimenti M ON A.PIVA = M.PIVA AND A.Id_Agenda = M.Id_Agenda " & vbCrLf)
            stbQ.Append(" INNER JOIN Movimenti_dettagli MD ON M.PIVA = MD.PIVA AND M.Id_Agenda = MD.Id_Agenda AND M.Id_Mov = MD.Id_Mov " & vbCrLf)
            stbQ.Append(" INNER JOIN Materie_Prime MP ON MP.Elem_Cod=MD.Elem_Cod AND MP.Mat_Cod=MD.Mat_Cod " & vbCrLf)
            stbQ.Append(" INNER JOIN UnitaMisura UM ON UM.UDM_COD=MD.Udm_Cod " & vbCrLf)
            stbQ.Append(" WHERE EXISTS (SELECT * FROM Materie_PrimexReport MPR WHERE MPR.Mat_Cod=MD.Elem_Cod AND MPR.Pro_Cod=MD.Pro_Cod) " & vbCrLf)
            stbQ.Append(" AND MD.Elem_Cod=" & CStr(ALTRE_MATERIE) & vbCrLf) ' altre materie prime aziendali 200
            stbQ.Append(" AND M.Cau_Mov='" & CStr(CAU_SCARICO) & "'" & vbCrLf)
            stbQ.Append(" AND A.PIVA='" & Agro_SQL_SaveText(Piva) & "' " & vbCrLf)
            If Elenco_IdAgenda <> "" Then
                stbQ.Append(" AND A.Id_Agenda IN (" & Agro_SQL_Save_Clausola_IN(Elenco_IdAgenda) & ")" & vbCrLf)
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stbQ.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    '#####################################################################
    Public Function RegistroFrizzanti_Dolcificazioni(ByVal Piva As String,
                                                     ByVal Elenco_IdAgenda As String,
                                                     ByVal Elem_Cod As Int32,
                                                     ByVal Mat_Cod As Int32,
                                                     ByRef objParametri As AgronicaCoreParametri
                                                     ) As DataTable

        Const nomeRoutine = "AgronicaCoreStampeDAL.RegistriCantina2.RegistroFrizzanti_Dolcificazioni"

        Dim messaggioErrore As String = ""
        Dim stbQ As New StringBuilder
        Dim dt As DataTable

        Try

            stbQ.Length = 0

            stbQ.Append(" SELECT A.Id_Agenda, M.Data_Movimento, MD.Elem_Cod, MD.Mat_Cod, MP.Mat_Des, MD.Qta, MD.Udm_Cod, UM.UDM_SIM " & vbCrLf)
            stbQ.Append(" FROM  Agenda A " & vbCrLf)
            stbQ.Append(" INNER JOIN Movimenti M ON A.PIVA = M.PIVA AND A.Id_Agenda = M.Id_Agenda " & vbCrLf)
            stbQ.Append(" INNER JOIN Movimenti_dettagli MD ON M.PIVA = MD.PIVA AND M.Id_Agenda = MD.Id_Agenda AND M.Id_Mov = MD.Id_Mov " & vbCrLf)
            stbQ.Append(" INNER JOIN Materie_Prime MP ON MP.Elem_Cod=MD.Elem_Cod AND MP.Mat_Cod=MD.Mat_Cod " & vbCrLf)
            stbQ.Append(" INNER JOIN UnitaMisura UM ON UM.UDM_COD=MD.Udm_Cod " & vbCrLf)

            stbQ.Append(" WHERE ((EXISTS (SELECT 1 FROM Materie_PrimexReport MPR WHERE MPR.Mat_Cod=MD.Mat_Cod AND MPR.Pro_Cod=MD.Pro_Cod) " & vbCrLf)

            stbQ.Append(" AND MP.Mat_Des LIKE '%" & CStr("Mosto") & "%')" & vbCrLf)

            ' aggiunto il 14/05/2012 per gestione della tabella OGenerazioni_Anagrafe_Log
            If Elem_Cod <> 0 AndAlso Mat_Cod <> 0 Then
                stbQ.Append("OR (MD.Elem_Cod =" & Elem_Cod & " and MD.mat_cod=" & Mat_Cod & ") ")
            End If
            stbQ.Append(" )")

            stbQ.Append(" AND M.Cau_Mov='" & CStr(CAU_SCARICO) & "'" & vbCrLf)
            ' stbQ.Append(" OR M.Cau_Mov='" + CStr(CAU_CARICO) + "'" + vbCrLf)
            stbQ.Append(" AND A.PIVA='" & Agro_SQL_SaveText(Piva) & "' " & vbCrLf)
            If Elenco_IdAgenda <> "" Then
                stbQ.Append(" AND A.Id_Agenda IN (" & Agro_SQL_Save_Clausola_IN(Elenco_IdAgenda) & ")" & vbCrLf)
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stbQ.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    '#####################################################################
    Public Function Acidificazioni(ByVal Piva As String,
                                   ByVal Id_Trasformazione As Int32,
                                   ByVal xFiltroAggiuntivo As String,
                                   ByRef objParametri As AgronicaCoreParametri
                                   ) As DataTable

        '            ByVal Tipo_Generazione As Int32,

        Const nomeRoutine = "AgronicaCoreStampeDAL.RegistriCantina2.Acidificazioni"

        Dim messaggioErrore As String = ""
        Dim stbQ As New StringBuilder
        Dim dt As DataTable

        Try

            stbQ.Length = 0

            'metto il distinct x il join con OGenerazioni_Anagrafe_Log che produce + record
            stbQ.Append(" SELECT DISTINCT Agenda.Id_Agenda, des_lib, preparazione_des, M.Data_Movimento, MD.Elem_Cod, MD.Mat_Cod, MD.Lotto, MP.Mat_Des, MD.Qta, MD.Udm_Cod, UM.UDM_SIM " & vbCrLf)
            stbQ.Append(" FROM  Agenda " & vbCrLf)
            stbQ.Append(" INNER JOIN Movimenti M ON Agenda.PIVA = M.PIVA AND Agenda.Id_Agenda = M.Id_Agenda " & vbCrLf)
            stbQ.Append(" INNER JOIN Movimenti_dettagli MD ON M.PIVA = MD.PIVA AND M.Id_Agenda = MD.Id_Agenda AND M.Id_Mov = MD.Id_Mov " & vbCrLf)
            stbQ.Append(" INNER JOIN Materie_Prime MP ON MP.Elem_Cod=MD.Elem_Cod AND MP.Mat_Cod=MD.Mat_Cod " & vbCrLf)
            stbQ.Append(" INNER JOIN UnitaMisura UM ON UM.UDM_COD=MD.Udm_Cod " & vbCrLf)

            stbQ.Append(" INNER JOIN Materie_PrimexReport MPR ON MPR.Mat_Cod = MP.Mat_Cod " & vbCrLf)

            stbQ.Append(" INNER JOIN Linee_Preparazioni ON Agenda.Preparazione_Cod = Linee_Preparazioni.Preparazione_Cod " & vbCrLf)
            stbQ.Append(" AND Agenda.Piva = Linee_Preparazioni.Piva " & vbCrLf)

            stbQ.Append(" INNER JOIN OGenerazioni_Anagrafe_Log " & vbCrLf)
            stbQ.Append(" ON OGenerazioni_Anagrafe_Log.Modulo_Generazione= " & Agro_SQL_SaveNum(enum_Omni_Modulo_Generazione.Cantine) & " " & vbCrLf)
            stbQ.Append(" AND OGenerazioni_Anagrafe_Log.Piva_SuperUser= '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' " & vbCrLf)
            stbQ.Append(" AND OGenerazioni_Anagrafe_Log.Elem_Cod= MP.Elem_Cod " & vbCrLf)
            stbQ.Append(" AND OGenerazioni_Anagrafe_Log.Mat_Cod= MP.Mat_Cod " & vbCrLf)
            stbQ.Append(" -- non faccio il filtro sulla linea (altrimenti se l'anagrafica è loggata in un'altra linea, invece che in questa, non viene su) " & vbCrLf)
            stbQ.Append(" -- AND OGenerazioni_Anagrafe_Log.Linea_Cod= Agenda.linea_cod " & vbCrLf)
            stbQ.Append(" AND OGenerazioni_Anagrafe_Log.piva=Agenda.piva " & vbCrLf)
            stbQ.Append(" AND Tipo_Generazione = " & Agro_SQL_SaveNum(enum_Omni_Tipo_Generazione.AltreMateriePrime) & vbCrLf)

            stbQ.Append(" WHERE Agenda.Piva = '" & Agro_SQL_SaveText(Piva) & "' " & vbCrLf)

            stbQ.Append(" AND M.Cau_Mov='" & CStr(CAU_SCARICO) & "'" & vbCrLf)

            stbQ.Append(" AND Linee_Preparazioni.Modulo_Generazione = " & Agro_SQL_SaveNum(enum_Omni_Modulo_Generazione.Cantine) & vbCrLf)

            stbQ.Append(" AND MPR.Id_Report IN ( " & vbCrLf)
            stbQ.Append(" " & CStr(enum_AgroReportistica.Frizzanti) & ", " & vbCrLf)
            stbQ.Append(" " & CStr(enum_AgroReportistica.Spumanti) & " " & vbCrLf)
            stbQ.Append(" ) " & vbCrLf)

            stbQ.Append(" AND ( Agenda.id_trasformazione = " & Id_Trasformazione.ToString & vbCrLf)
            'stbQ.Append(" OR Agenda.id_trasformazione = ")
            'stbQ.Append("(SELECT Id_Trasformazione_Rif FROM Trasformazioni_Riferimenti WHERE Id_Trasformazione = " & Id_Trasformazione.ToString & "))" & vbCrLf)
            stbQ.Append("       OR Agenda.id_trasformazione IN ")
            stbQ.Append("                               (SELECT DISTINCT Id_Trasformazione_Rif FROM Trasformazioni_Riferimenti WHERE Id_Trasformazione = " & Id_Trasformazione.ToString & " " & vbCrLf)
            stbQ.Append("                               ) " & vbCrLf)
            stbQ.Append("       ) " & vbCrLf)

            'nel filtro aggiuntivo passo il filtro sui codici delle preparazioni
            If xFiltroAggiuntivo <> "" Then
                stbQ.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stbQ.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    '#####################################################################
    Public Function Dolcificazioni_Arricchimenti(ByVal Piva As String,
                                                 ByVal Id_Trasformazione As Int32,
                                                 ByVal xFiltroAggiuntivo As String,
                                                 ByRef objParametri As AgronicaCoreParametri
                                                 ) As DataTable

        '            ByVal Tipo_Generazione As Int32,

        Const nomeRoutine = "AgronicaCoreStampeDAL.RegistriCantina2.Dolcificazioni_Arricchimenti"

        Dim messaggioErrore As String = ""
        Dim stbQ As New StringBuilder
        Dim dt As DataTable

        Try

            stbQ.Length = 0

            'metto il distinct x il join con OGenerazioni_Anagrafe_Log che produce + record
            stbQ.Append(" SELECT DISTINCT Agenda.Id_Agenda, des_lib, preparazione_des, M.Data_Movimento, MD.Elem_Cod, MD.Mat_Cod, MD.Lotto, MP.Mat_Des, MD.Qta, MD.Udm_Cod, UM.UDM_SIM, " & vbCrLf)
            stbQ.Append(" Mov_Destinazioni.Tipo_Destinazione, Mov_Destinazioni.Qta AS Qta_Dest, Mov_Destinazioni.Qta2,  " & vbCrLf)

            stbQ.Append(" ISNULL( (SELECT 'Vasca ' + Cantina_Vasche.Identificativo " & vbCrLf)
            stbQ.Append(" FROM Cantina_Vasche  " & vbCrLf)
            stbQ.Append(" where Mov_Destinazioni.Piva = Cantina_Vasche.Piva " & vbCrLf)
            stbQ.Append(" AND Mov_Destinazioni.Sa_Cod = Cantina_Vasche.Sa_Cod " & vbCrLf)
            stbQ.Append(" AND Mov_Destinazioni.Id_Destinazione = Cantina_Vasche.Vas_Cod " & vbCrLf)
            stbQ.Append(" AND Mov_Destinazioni.Tipo_Destinazione= " & Agro_SQL_SaveNum(TIPO_DESTINAZIONE_VASCA) & " " & vbCrLf)
            stbQ.Append("  ) , 'Magazzino') as Identificativo " & vbCrLf)

            stbQ.Append(" FROM  Agenda " & vbCrLf)
            stbQ.Append(" INNER JOIN Movimenti M ON Agenda.PIVA = M.PIVA AND Agenda.Id_Agenda = M.Id_Agenda " & vbCrLf)
            stbQ.Append(" INNER JOIN Movimenti_dettagli MD ON M.PIVA = MD.PIVA AND M.Id_Agenda = MD.Id_Agenda AND M.Id_Mov = MD.Id_Mov " & vbCrLf)
            stbQ.Append(" INNER JOIN Materie_Prime MP ON MP.Elem_Cod=MD.Elem_Cod AND MP.Mat_Cod=MD.Mat_Cod " & vbCrLf)
            stbQ.Append(" INNER JOIN UnitaMisura UM ON UM.UDM_COD=MD.Udm_Cod " & vbCrLf)

            stbQ.Append(" INNER JOIN Materie_PrimexReport MPR ON MPR.Mat_Cod = MP.Mat_Cod " & vbCrLf)

            stbQ.Append(" INNER JOIN Linee_Preparazioni ON Agenda.Preparazione_Cod = Linee_Preparazioni.Preparazione_Cod " & vbCrLf)
            stbQ.Append(" AND Agenda.Piva = Linee_Preparazioni.Piva " & vbCrLf)

            stbQ.Append(" INNER JOIN Mov_Destinazioni ON MD.PIVA = Mov_Destinazioni.Piva AND MD.Id_Agenda = Mov_Destinazioni.Id_Agenda AND " & vbCrLf)
            stbQ.Append(" MD.Id_Mov = Mov_Destinazioni.Id_Mov AND MD.Id_Mov_Det = Mov_Destinazioni.Id_Mov_Det " & vbCrLf)

            stbQ.Append(" INNER JOIN OGenerazioni_Anagrafe_Log " & vbCrLf)
            stbQ.Append(" ON OGenerazioni_Anagrafe_Log.Modulo_Generazione= " & Agro_SQL_SaveNum(enum_Omni_Modulo_Generazione.Cantine) & " " & vbCrLf)
            stbQ.Append(" AND OGenerazioni_Anagrafe_Log.Piva_SuperUser= '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' " & vbCrLf)
            stbQ.Append(" AND OGenerazioni_Anagrafe_Log.Elem_Cod= MP.Elem_Cod " & vbCrLf)
            stbQ.Append(" AND OGenerazioni_Anagrafe_Log.Mat_Cod= MP.Mat_Cod " & vbCrLf)
            stbQ.Append(" -- non faccio il filtro sulla linea (altrimenti se l'anagrafica è loggata in un'altra linea, invece che in questa, non viene su) " & vbCrLf)
            stbQ.Append(" -- AND OGenerazioni_Anagrafe_Log.Linea_Cod= Agenda.linea_cod " & vbCrLf)
            stbQ.Append(" AND OGenerazioni_Anagrafe_Log.piva=Agenda.piva " & vbCrLf)
            stbQ.Append(" AND Tipo_Generazione = " & Agro_SQL_SaveNum(enum_Omni_Tipo_Generazione.AltreMateriePrime) & vbCrLf)

            stbQ.Append(" WHERE Agenda.Piva = '" & Agro_SQL_SaveText(Piva) & "' " & vbCrLf)

            stbQ.Append(" AND M.Cau_Mov='" & CStr(CAU_SCARICO) & "'" & vbCrLf)

            stbQ.Append(" AND Linee_Preparazioni.Modulo_Generazione = " & Agro_SQL_SaveNum(enum_Omni_Modulo_Generazione.Cantine) & vbCrLf)

            stbQ.Append(" AND MPR.Id_Report IN ( " & vbCrLf)
            stbQ.Append(" " & CStr(enum_AgroReportistica.Frizzanti) & ", " & vbCrLf)
            stbQ.Append(" " & CStr(enum_AgroReportistica.Spumanti) & " " & vbCrLf)
            stbQ.Append(" ) " & vbCrLf)

            stbQ.Append(" AND ( Agenda.id_trasformazione = " & Id_Trasformazione.ToString & vbCrLf)
            'stbQ.Append(" OR Agenda.id_trasformazione = ")
            'stbQ.Append("(SELECT Id_Trasformazione_Rif FROM Trasformazioni_Riferimenti WHERE Id_Trasformazione = " & Id_Trasformazione.ToString & "))" & vbCrLf)
            stbQ.Append("       OR Agenda.id_trasformazione IN ")
            stbQ.Append("                               (SELECT DISTINCT Id_Trasformazione_Rif FROM Trasformazioni_Riferimenti WHERE Id_Trasformazione = " & Id_Trasformazione.ToString & " " & vbCrLf)
            stbQ.Append("                               ) " & vbCrLf)
            stbQ.Append("       ) " & vbCrLf)

            'nel filtro aggiuntivo passo il filtro sui codici delle preparazioni
            If xFiltroAggiuntivo <> "" Then
                stbQ.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stbQ.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    '#####################################################################
    Public Function Travasi(ByVal Piva As String,
                            ByVal Id_Trasformazione As Int32,
                            ByVal xFiltroAggiuntivo As String,
                            ByRef objParametri As AgronicaCoreParametri
                            ) As DataTable

        '            ByVal Tipo_Generazione As Int32,

        Const nomeRoutine = "AgronicaCoreStampeDAL.RegistriCantina2.Travasi"

        Dim messaggioErrore As String = ""
        Dim stbQ As New StringBuilder
        Dim dt As DataTable

        Try

            stbQ.Length = 0

            'metto il distinct x il join con OGenerazioni_Anagrafe_Log che produce + record
            stbQ.Append(" SELECT DISTINCT Agenda.Id_Agenda, des_lib, preparazione_des, M.Data_Movimento, MD.Elem_Cod, MD.Mat_Cod, MP.Mat_Des, MD.Qta, MD.Udm_Cod, UM.UDM_SIM " & vbCrLf)
            stbQ.Append(" FROM  Agenda " & vbCrLf)
            stbQ.Append(" INNER JOIN Movimenti M ON Agenda.PIVA = M.PIVA AND Agenda.Id_Agenda = M.Id_Agenda " & vbCrLf)
            stbQ.Append(" INNER JOIN Movimenti_dettagli MD ON M.PIVA = MD.PIVA AND M.Id_Agenda = MD.Id_Agenda AND M.Id_Mov = MD.Id_Mov " & vbCrLf)
            stbQ.Append(" INNER JOIN Materie_Prime MP ON MP.Elem_Cod=MD.Elem_Cod AND MP.Mat_Cod=MD.Mat_Cod " & vbCrLf)
            stbQ.Append(" INNER JOIN UnitaMisura UM ON UM.UDM_COD=MD.Udm_Cod " & vbCrLf)

            stbQ.Append(" INNER JOIN Materie_PrimexReport MPR ON MPR.Mat_Cod = MP.Mat_Cod " & vbCrLf)

            stbQ.Append(" INNER JOIN Linee_Preparazioni ON Agenda.Preparazione_Cod = Linee_Preparazioni.Preparazione_Cod " & vbCrLf)
            stbQ.Append(" AND Agenda.Piva = Linee_Preparazioni.Piva " & vbCrLf)

            stbQ.Append(" INNER JOIN OGenerazioni_Anagrafe_Log " & vbCrLf)
            stbQ.Append(" ON OGenerazioni_Anagrafe_Log.Modulo_Generazione= " & Agro_SQL_SaveNum(enum_Omni_Modulo_Generazione.Cantine) & " " & vbCrLf)
            stbQ.Append(" AND OGenerazioni_Anagrafe_Log.Piva_SuperUser= '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' " & vbCrLf)
            stbQ.Append(" AND OGenerazioni_Anagrafe_Log.Elem_Cod= MP.Elem_Cod " & vbCrLf)
            stbQ.Append(" AND OGenerazioni_Anagrafe_Log.Mat_Cod= MP.Mat_Cod " & vbCrLf)
            stbQ.Append(" -- non faccio il filtro sulla linea (altrimenti se l'anagrafica è loggata in un'altra linea, invece che in questa, non viene su) " & vbCrLf)
            stbQ.Append(" -- AND OGenerazioni_Anagrafe_Log.Linea_Cod= Agenda.linea_cod " & vbCrLf)
            stbQ.Append(" AND OGenerazioni_Anagrafe_Log.piva=Agenda.piva " & vbCrLf)
            stbQ.Append(" AND Tipo_Generazione = " & Agro_SQL_SaveNum(enum_Omni_Tipo_Generazione.CaliLavorazione) & vbCrLf)

            stbQ.Append(" WHERE Agenda.Piva = '" & Agro_SQL_SaveText(Piva) & "' " & vbCrLf)

            stbQ.Append(" AND M.Cau_Mov='" & CStr(CAU_CARICO) & "'" & vbCrLf)

            stbQ.Append(" AND Linee_Preparazioni.Modulo_Generazione = " & Agro_SQL_SaveNum(enum_Omni_Modulo_Generazione.Cantine) & vbCrLf)

            stbQ.Append(" AND MPR.Id_Report IN ( " & vbCrLf)
            stbQ.Append(" " & CStr(enum_AgroReportistica.Frizzanti) & ", " & vbCrLf)
            stbQ.Append(" " & CStr(enum_AgroReportistica.Spumanti) & " " & vbCrLf)
            stbQ.Append(" ) " & vbCrLf)

            stbQ.Append(" AND ( Agenda.id_trasformazione = " & Id_Trasformazione.ToString & vbCrLf)
            'stbQ.Append(" OR Agenda.id_trasformazione = ")
            'stbQ.Append("(SELECT Id_Trasformazione_Rif FROM Trasformazioni_Riferimenti WHERE Id_Trasformazione = " & Id_Trasformazione.ToString & "))" & vbCrLf)
            stbQ.Append("       OR Agenda.id_trasformazione IN " & vbCrLf)
            stbQ.Append("                               (SELECT DISTINCT Id_Trasformazione_Rif FROM Trasformazioni_Riferimenti WHERE Id_Trasformazione = " & Id_Trasformazione.ToString & " " & vbCrLf)
            stbQ.Append("                               ) " & vbCrLf)
            stbQ.Append("       ) " & vbCrLf)

            'nel filtro aggiuntivo passo il filtro sui codici delle preparazioni
            If xFiltroAggiuntivo <> "" Then
                stbQ.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stbQ.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function


    '#####################################################################
    Public Function RegistroFrizzanti_Imbottigliamento(ByVal Piva As String,
                                                       ByVal Elenco_IdAgenda As String,
                                                       ByRef objParametri As AgronicaCoreParametri
                                                       ) As DataTable

        Const nomeRoutine = "AgronicaCoreStampeDAL.RegistriCantina2.RegistroFrizzanti_Imbottigliamento"

        Dim messaggioErrore As String = ""
        Dim stbQ As New StringBuilder
        Dim dt As DataTable

        Try

            stbQ.Length = 0

            stbQ.Append(" SELECT Agenda.Id_Agenda, Movimenti.Data_Movimento, Cau_Mov, Mat_Des, Materie_Prime.Elem_Cod, Materie_Prime.Mat_Cod, " & vbCrLf)
            stbQ.Append(" Materie_Prime.Cod_Articolo, Movimenti_dettagli.Lotto, Movimenti_dettagli.Qta, Movimenti_dettagli.Udm_Cod, " & vbCrLf)
            stbQ.Append("           (SELECT SUM(Qta) FROM Agenda A INNER JOIN " & vbCrLf)
            stbQ.Append("            Movimenti ON A.PIVA = Movimenti.PIVA AND A.Id_Agenda = Movimenti.Id_Agenda INNER JOIN " & vbCrLf)
            stbQ.Append("            Movimenti_dettagli ON Movimenti.PIVA = Movimenti_dettagli.PIVA AND Movimenti.Id_Agenda = Movimenti_dettagli.Id_Agenda AND " & vbCrLf)
            stbQ.Append("            Movimenti.Id_Mov = Movimenti_dettagli.Id_Mov" & vbCrLf)
            stbQ.Append("            WHERE(Agenda.Id_Agenda = A.Id_Agenda)" & vbCrLf)
            stbQ.Append("            AND Cau_Mov='" & CStr(CAU_SCARICO) & "' AND Udm_Cod=29) AS Qta_Origine, " & vbCrLf)

            stbQ.Append("  CASE WHEN Materie_Prime.Peso_Set>0 THEN Movimenti_dettagli.QTA_EXTRA*Movimenti_dettagli.QTA " & vbCrLf)
            stbQ.Append("  WHEN Materie_Prime.Peso_Set=0 THEN Materie_Prime.QTA_EXTRA*Movimenti_dettagli.QTA " & vbCrLf)
            stbQ.Append("  ELSE 0 END AS Qta_Confezionato, " & vbCrLf)

            stbQ.Append("  Movimenti_dettagli.QTA_EXTRA, Movimenti_dettagli.udm_cod_extra, UnitaMisura.UDM_SIM AS udm_sim_extra" & vbCrLf)

            stbQ.Append(" -- serve il TOP 1 perchè un'anagrafica è loggata su più linee " & vbCrLf)
            stbQ.Append(" , isnull((SELECT TOP 1 Tipo_Generazione " & vbCrLf)
            stbQ.Append(" FROM OGenerazioni_Anagrafe_Log " & vbCrLf)
            stbQ.Append(" WHERE Modulo_Generazione= " & Agro_SQL_SaveNum(enum_Omni_Modulo_Generazione.Cantine) & " " & vbCrLf)
            stbQ.Append(" AND Piva_SuperUser= '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' " & vbCrLf)
            stbQ.Append(" AND OGenerazioni_Anagrafe_Log.Elem_Cod=Materie_Prime.Elem_Cod " & vbCrLf)
            stbQ.Append(" AND OGenerazioni_Anagrafe_Log.Mat_Cod=Materie_Prime.Mat_Cod " & vbCrLf)
            stbQ.Append(" -- non faccio il filtro sulla linea (altrimenti se l'anagrafica è loggata in un'altra linea, invece che in questa, non viene su) " & vbCrLf)
            stbQ.Append(" -- AND OGenerazioni_Anagrafe_Log.Linea_Cod=agenda.linea_cod " & vbCrLf)
            stbQ.Append(" AND OGenerazioni_Anagrafe_Log.piva=agenda.piva " & vbCrLf)
            stbQ.Append(" ),0  ) AS Tipo_Generazione " & vbCrLf)

            stbQ.Append(" -- serve il TOP 1 perchè un'anagrafica è loggata su più linee " & vbCrLf)
            stbQ.Append(" , isnull((SELECT TOP 1 Codice_Generazione " & vbCrLf)
            stbQ.Append(" FROM OGenerazioni_Anagrafe_Log " & vbCrLf)
            stbQ.Append(" WHERE Modulo_Generazione= " & Agro_SQL_SaveNum(enum_Omni_Modulo_Generazione.Cantine) & " " & vbCrLf)
            stbQ.Append(" AND Piva_SuperUser= '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' " & vbCrLf)
            stbQ.Append(" AND OGenerazioni_Anagrafe_Log.Elem_Cod=Materie_Prime.Elem_Cod " & vbCrLf)
            stbQ.Append(" AND OGenerazioni_Anagrafe_Log.Mat_Cod=Materie_Prime.Mat_Cod " & vbCrLf)
            stbQ.Append(" -- non faccio il filtro sulla linea (altrimenti se l'anagrafica è loggata in un'altra linea, invece che in questa, non viene su) " & vbCrLf)
            stbQ.Append(" -- AND OGenerazioni_Anagrafe_Log.Linea_Cod=agenda.linea_cod " & vbCrLf)
            stbQ.Append(" AND OGenerazioni_Anagrafe_Log.piva=agenda.piva " & vbCrLf)
            stbQ.Append(" ),0  ) AS Codice_Generazione " & vbCrLf)


            stbQ.Append("  FROM Agenda " & vbCrLf)
            stbQ.Append("  INNER JOIN Movimenti ON Agenda.PIVA = Movimenti.PIVA AND Agenda.Id_Agenda = Movimenti.Id_Agenda AND Agenda.Sa_Cod = Movimenti.Sa_Cod " & vbCrLf)
            stbQ.Append("  INNER JOIN Movimenti_dettagli ON Movimenti.PIVA = Movimenti_dettagli.PIVA AND Movimenti.Id_Agenda = Movimenti_dettagli.Id_Agenda AND Movimenti.Id_Mov = Movimenti_dettagli.Id_Mov " & vbCrLf)
            stbQ.Append("  INNER JOIN Mov_Destinazioni ON Movimenti_dettagli.PIVA = Mov_Destinazioni.Piva AND Movimenti_dettagli.Id_Agenda = Mov_Destinazioni.Id_Agenda AND " & vbCrLf)
            stbQ.Append("             Movimenti_dettagli.Id_Mov = Mov_Destinazioni.Id_Mov And Movimenti_dettagli.Id_Mov_Det = Mov_Destinazioni.Id_Mov_Det" & vbCrLf)
            stbQ.Append("  INNER JOIN Materie_Prime ON Movimenti_dettagli.PIVA = Materie_Prime.Piva AND Movimenti_dettagli.Elem_Cod = Materie_Prime.Elem_Cod AND " & vbCrLf)
            stbQ.Append("             Movimenti_dettagli.Mat_Cod = Materie_Prime.Mat_Cod " & vbCrLf)
            stbQ.Append(" INNER JOIN UnitaMisura ON Movimenti_dettagli.udm_cod_extra = UnitaMisura.UDM_COD " & vbCrLf)

            stbQ.Append("  WHERE  Cau_Mov='" & CStr(CAU_CARICO) & "' " & vbCrLf)
            stbQ.Append("  AND Agenda.PIVA='" & Agro_SQL_SaveText(Piva) & "' " & vbCrLf)
            If Elenco_IdAgenda <> "" Then
                stbQ.Append("  AND Agenda.Id_Agenda in (" & Agro_SQL_Save_Clausola_IN(Elenco_IdAgenda) & ")" & vbCrLf)
            End If
            'stbQ.Append("  AND Materie_Prime.Elem_Cod <> " + CStr(Elem_Cod_Calo) + vbCrLf)
            'stbQ.Append("  AND Materie_Prime.Mat_Cod <> " + CStr(Mat_Cod_Calo) + vbCrLf)

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stbQ.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    '#####################################################################
    Public Function Imbottigliamento(ByVal Piva As String,
                                     ByVal Id_Trasformazione As Int32,
                                     ByVal xFiltroAggiuntivo As String,
                                     ByRef objParametri As AgronicaCoreParametri
                                     ) As DataTable

        Const nomeRoutine = "AgronicaCoreStampeDAL.RegistriCantina2.Imbottigliamento"

        Dim messaggioErrore As String = ""
        Dim stbQ As New StringBuilder
        Dim dt As DataTable

        Try

            stbQ.Length = 0

            stbQ.Append(" SELECT Agenda.Id_Agenda, des_lib, preparazione_des, Movimenti.Data_Movimento, Mat_Des, Materie_Prime.Elem_Cod, Materie_Prime.Mat_Cod, " & vbCrLf)
            stbQ.Append(" Materie_Prime.Cod_Articolo, Movimenti_dettagli.Lotto, Movimenti_dettagli.Qta, Movimenti_dettagli.Udm_Cod, " & vbCrLf)
            stbQ.Append("           (SELECT SUM(Qta) FROM Agenda A INNER JOIN " & vbCrLf)
            stbQ.Append("            Movimenti ON A.PIVA = Movimenti.PIVA AND A.Id_Agenda = Movimenti.Id_Agenda INNER JOIN " & vbCrLf)
            stbQ.Append("            Movimenti_dettagli ON Movimenti.PIVA = Movimenti_dettagli.PIVA AND Movimenti.Id_Agenda = Movimenti_dettagli.Id_Agenda AND " & vbCrLf)
            stbQ.Append("            Movimenti.Id_Mov = Movimenti_dettagli.Id_Mov" & vbCrLf)
            stbQ.Append("            WHERE(Agenda.Id_Agenda = A.Id_Agenda)" & vbCrLf)
            stbQ.Append("            AND Cau_Mov='" & CStr(CAU_SCARICO) & "' AND Udm_Cod=29) AS Qta_Origine, " & vbCrLf)

            stbQ.Append("  CASE WHEN Materie_Prime.Peso_Set>0 THEN Movimenti_dettagli.QTA_EXTRA*Movimenti_dettagli.QTA " & vbCrLf)
            stbQ.Append("  WHEN Materie_Prime.Peso_Set=0 THEN Materie_Prime.QTA_EXTRA*Movimenti_dettagli.QTA " & vbCrLf)
            stbQ.Append("  ELSE 0 END AS Qta_Confezionato, " & vbCrLf)

            stbQ.Append("  Movimenti_dettagli.QTA_EXTRA, Movimenti_dettagli.udm_cod_extra, UnitaMisura.UDM_SIM AS udm_sim_extra" & vbCrLf)

            stbQ.Append(" -- serve il TOP 1 perchè un'anagrafica è loggata su più linee " & vbCrLf)
            stbQ.Append(" , isnull((SELECT TOP 1 Tipo_Generazione " & vbCrLf)
            stbQ.Append(" FROM OGenerazioni_Anagrafe_Log " & vbCrLf)
            stbQ.Append(" WHERE Modulo_Generazione= " & Agro_SQL_SaveNum(enum_Omni_Modulo_Generazione.Cantine) & " " & vbCrLf)
            stbQ.Append(" AND Piva_SuperUser= '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' " & vbCrLf)
            stbQ.Append(" AND OGenerazioni_Anagrafe_Log.Elem_Cod=Materie_Prime.Elem_Cod " & vbCrLf)
            stbQ.Append(" AND OGenerazioni_Anagrafe_Log.Mat_Cod=Materie_Prime.Mat_Cod " & vbCrLf)
            stbQ.Append(" -- non faccio il filtro sulla linea (altrimenti se l'anagrafica è loggata in un'altra linea, invece che in questa, non viene su) " & vbCrLf)
            stbQ.Append(" -- AND OGenerazioni_Anagrafe_Log.Linea_Cod=agenda.linea_cod " & vbCrLf)
            stbQ.Append(" AND OGenerazioni_Anagrafe_Log.piva=agenda.piva " & vbCrLf)
            stbQ.Append(" ),0  ) AS Tipo_Generazione " & vbCrLf)

            stbQ.Append(" -- serve il TOP 1 perchè un'anagrafica è loggata su più linee " & vbCrLf)
            stbQ.Append(" , isnull((SELECT TOP 1 Codice_Generazione " & vbCrLf)
            stbQ.Append(" FROM OGenerazioni_Anagrafe_Log " & vbCrLf)
            stbQ.Append(" WHERE Modulo_Generazione= " & Agro_SQL_SaveNum(enum_Omni_Modulo_Generazione.Cantine) & " " & vbCrLf)
            stbQ.Append(" AND Piva_SuperUser= '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' " & vbCrLf)
            stbQ.Append(" AND OGenerazioni_Anagrafe_Log.Elem_Cod=Materie_Prime.Elem_Cod " & vbCrLf)
            stbQ.Append(" AND OGenerazioni_Anagrafe_Log.Mat_Cod=Materie_Prime.Mat_Cod " & vbCrLf)
            stbQ.Append(" -- non faccio il filtro sulla linea (altrimenti se l'anagrafica è loggata in un'altra linea, invece che in questa, non viene su) " & vbCrLf)
            stbQ.Append(" -- AND OGenerazioni_Anagrafe_Log.Linea_Cod=agenda.linea_cod " & vbCrLf)
            stbQ.Append(" AND OGenerazioni_Anagrafe_Log.piva=agenda.piva " & vbCrLf)
            stbQ.Append(" ),0  ) AS Codice_Generazione " & vbCrLf)

            stbQ.Append("  FROM Agenda " & vbCrLf)

            stbQ.Append(" INNER JOIN Linee_Preparazioni ON Agenda.Preparazione_Cod = Linee_Preparazioni.Preparazione_Cod " & vbCrLf)
            stbQ.Append("               AND Agenda.Piva = Linee_Preparazioni.Piva " & vbCrLf)

            stbQ.Append("  INNER JOIN Movimenti ON Agenda.PIVA = Movimenti.PIVA AND Agenda.Id_Agenda = Movimenti.Id_Agenda AND Agenda.Sa_Cod = Movimenti.Sa_Cod " & vbCrLf)

            stbQ.Append("  INNER JOIN Movimenti_dettagli ON Movimenti.PIVA = Movimenti_dettagli.PIVA AND Movimenti.Id_Agenda = Movimenti_dettagli.Id_Agenda AND Movimenti.Id_Mov = Movimenti_dettagli.Id_Mov " & vbCrLf)

            stbQ.Append("  INNER JOIN Mov_Destinazioni ON Movimenti_dettagli.PIVA = Mov_Destinazioni.Piva AND Movimenti_dettagli.Id_Agenda = Mov_Destinazioni.Id_Agenda AND " & vbCrLf)
            stbQ.Append("             Movimenti_dettagli.Id_Mov = Mov_Destinazioni.Id_Mov And Movimenti_dettagli.Id_Mov_Det = Mov_Destinazioni.Id_Mov_Det" & vbCrLf)

            stbQ.Append("  INNER JOIN Materie_Prime ON Movimenti_dettagli.PIVA = Materie_Prime.Piva AND Movimenti_dettagli.Elem_Cod = Materie_Prime.Elem_Cod AND " & vbCrLf)
            stbQ.Append("             Movimenti_dettagli.Mat_Cod = Materie_Prime.Mat_Cod " & vbCrLf)

            stbQ.Append(" INNER JOIN UnitaMisura ON Movimenti_dettagli.udm_cod_extra = UnitaMisura.UDM_COD " & vbCrLf)

            stbQ.Append("  WHERE  Cau_Mov='" & CStr(CAU_CARICO) & "' " & vbCrLf)

            stbQ.Append("  AND Agenda.PIVA='" & Agro_SQL_SaveText(Piva) & "' " & vbCrLf)

            stbQ.Append(" AND Linee_Preparazioni.Modulo_Generazione = " & Agro_SQL_SaveNum(enum_Omni_Modulo_Generazione.Cantine) & vbCrLf)

            'stbQ.Append(" AND MPR.Id_Report IN ( " & vbCrLf)
            'stbQ.Append(" " & CStr(enum_AgroReportistica.Frizzanti) & ", " & vbCrLf)
            'stbQ.Append(" " & CStr(enum_AgroReportistica.Spumanti) & " " & vbCrLf)
            'stbQ.Append(" ) " & vbCrLf)

            stbQ.Append(" AND ( Agenda.id_trasformazione = " & Id_Trasformazione.ToString & vbCrLf)
            'stbQ.Append(" OR Agenda.id_trasformazione = ")
            'stbQ.Append("(SELECT Id_Trasformazione_Rif FROM Trasformazioni_Riferimenti WHERE Id_Trasformazione = " & Id_Trasformazione.ToString & "))" & vbCrLf)
            stbQ.Append("       OR Agenda.id_trasformazione IN ")
            stbQ.Append("                               (SELECT DISTINCT Id_Trasformazione_Rif FROM Trasformazioni_Riferimenti WHERE Id_Trasformazione = " & Id_Trasformazione.ToString & " " & vbCrLf)
            stbQ.Append("                               ) " & vbCrLf)
            stbQ.Append("       ) " & vbCrLf)


            'nel filtro aggiuntivo passo il filtro sui codici delle preparazioni
            If xFiltroAggiuntivo <> "" Then
                stbQ.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stbQ.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function


    '#####################################################################
    Public Function RegistroFrizzanti_LeggiPerdite_FineFrizzantatura(ByVal Piva As String,
                                                                     ByVal Elenco_IdAgenda As String,
                                                                     ByVal xFiltroAggiuntivo As String,
                                                                     ByVal xOrderBy As String,
                                                                     ByRef objParametri As AgronicaCoreParametri
                                                                     ) As DataTable

        Const nomeRoutine = "AgronicaCoreStampeDAL.RegistriCantina2.RegistroFrizzanti_LeggiPerdite_FineFrizzantatura"

        Dim messaggioErrore As String = ""
        Dim stbQ As New StringBuilder
        Dim dt As DataTable

        Try

            stbQ.Length = 0

            stbQ.Append(" SELECT agenda.piva, agenda.id_agenda, des_lib,agenda.linea_cod,Linee_Preparazioni.preparazione_cod, " & vbCrLf)
            stbQ.Append(" Linee_Preparazioni.preparazione_des, Linee_Preparazioni.preparazione_sigla,  Linee_Preparazioni.tipo_default, " & vbCrLf)
            stbQ.Append(" Movimenti.cau_mov, Movimenti.Data_Movimento,Movimenti_dettagli.Elem_Cod, Movimenti_dettagli.Pro_Cod, Movimenti_dettagli.Mat_Cod, " & vbCrLf)
            stbQ.Append(" Movimenti_dettagli.Mov_Det_Des,Movimenti_dettagli.Udm_Cod, Movimenti_dettagli.qta, UDM_SIM, udm_des " & vbCrLf)
            stbQ.Append("  " & vbCrLf)
            stbQ.Append("  " & vbCrLf)
            stbQ.Append("  " & vbCrLf)

            stbQ.Append(" FROM agenda " & vbCrLf)
            stbQ.Append(" INNER JOIN Linee_Preparazioni on Linee_Preparazioni.Piva= Agenda.PIVA  " & vbCrLf)
            stbQ.Append(" AND Linee_Preparazioni.Preparazione_Cod= agenda.Preparazione_Cod " & vbCrLf)
            stbQ.Append(" INNER JOIN Movimenti ON Movimenti.PIVA = Agenda.PIVA AND Movimenti.Id_Agenda = Agenda.Id_Agenda " & vbCrLf)
            stbQ.Append(" INNER JOIN Movimenti_dettagli ON Movimenti.PIVA = Movimenti_dettagli.PIVA  " & vbCrLf)
            stbQ.Append(" AND Movimenti.Id_Agenda = Movimenti_dettagli.Id_Agenda AND  Movimenti.Id_Mov = Movimenti_dettagli.Id_Mov " & vbCrLf)
            stbQ.Append(" INNER JOIN UnitaMisura ON Movimenti_dettagli.udm_cod=UnitaMisura.udm_cod " & vbCrLf)

            stbQ.Append(" WHERE Movimenti.Piva = '" & Agro_SQL_SaveText(Piva) & "' " & vbCrLf)

            stbQ.Append(" AND Movimenti.Cau_Mov = '" & CStr(CAU_CARICO) & "' " & vbCrLf)

            'modifica del 19/11/2015
            'oltre ai cali voglio leggere il prodotto feccioso
            'stbQ.Append(" AND Movimenti_dettagli.elem_cod = " + Agro_SQL_SaveNum(CALI_LAVORAZIONE) & " " + vbCrLf)
            stbQ.Append(" AND  EXISTS ( ")
            stbQ.Append("             SELECT 1  ")
            stbQ.Append("             FROM OGenerazioni_Anagrafe_Log  ")
            stbQ.Append("             WHERE Movimenti_Dettagli.Mat_Cod = OGenerazioni_Anagrafe_Log.Mat_Cod AND Movimenti_Dettagli.elem_Cod = OGenerazioni_Anagrafe_Log.elem_Cod  " & vbCrLf)
            stbQ.Append("             AND OGenerazioni_Anagrafe_Log.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' " & vbCrLf)
            stbQ.Append("             AND OGenerazioni_Anagrafe_Log.Tipo_Generazione IN ( " & Agro_SQL_SaveNum(enum_Omni_Tipo_Generazione.CaliLavorazione) & "," & Agro_SQL_SaveNum(enum_Omni_Tipo_Generazione.Scarti) & " ) " & vbCrLf)
            stbQ.Append("            )")
            stbQ.Append(" AND Linee_Preparazioni.Modulo_Generazione = " & Agro_SQL_SaveNum(enum_Omni_Modulo_Generazione.Cantine) & " " & vbCrLf)

            If Elenco_IdAgenda <> "" Then
                stbQ.Append(" AND agenda.Id_Agenda IN (" & Agro_SQL_Save_Clausola_IN(Elenco_IdAgenda) & ")" & vbCrLf)
            End If

            If xFiltroAggiuntivo <> "" Then
                stbQ.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                stbQ.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                stbQ.Append(" ORDER BY Movimenti.Data_Movimento ")
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stbQ.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function


    '#####################################################################
    Public Function PerditeElaborazione(ByVal Piva As String,
                                        ByVal Id_Trasformazione As Integer,
                                        ByVal xFiltroAggiuntivo As String,
                                        ByVal xOrderBy As String,
                                        ByRef objParametri As AgronicaCoreParametri
                                        ) As DataTable

        Const nomeRoutine = "AgronicaCoreStampeDAL.RegistriCantina2.PerditeElaborazione"

        Dim messaggioErrore As String = ""
        Dim stbQ As New StringBuilder
        Dim dt As DataTable

        Try

            stbQ.Length = 0

            'IL DISTINCT CI VUOLE X IL JOIN CON OGenerazioni_Anagrafe_Log
            stbQ.Append(" SELECT DISTINCT agenda.piva, agenda.id_agenda, des_lib,agenda.linea_cod,Linee_Preparazioni.preparazione_cod, " & vbCrLf)
            stbQ.Append(" Linee_Preparazioni.preparazione_des, Linee_Preparazioni.preparazione_sigla,  Linee_Preparazioni.tipo_default, " & vbCrLf)
            stbQ.Append(" Movimenti.cau_mov, Movimenti.Data_Movimento,Movimenti_dettagli.Elem_Cod, Movimenti_dettagli.Pro_Cod, Movimenti_dettagli.Mat_Cod, " & vbCrLf)
            stbQ.Append(" Movimenti_dettagli.Mov_Det_Des,Movimenti_dettagli.Udm_Cod, Movimenti_dettagli.qta, UDM_SIM, udm_des " & vbCrLf)
            stbQ.Append("  " & vbCrLf)
            stbQ.Append("  " & vbCrLf)
            stbQ.Append("  " & vbCrLf)

            stbQ.Append(" FROM agenda " & vbCrLf)
            stbQ.Append(" INNER JOIN Linee_Preparazioni on Linee_Preparazioni.Piva= Agenda.PIVA  " & vbCrLf)
            stbQ.Append(" AND Linee_Preparazioni.Preparazione_Cod= agenda.Preparazione_Cod " & vbCrLf)
            stbQ.Append(" INNER JOIN Movimenti ON Movimenti.PIVA = Agenda.PIVA AND Movimenti.Id_Agenda = Agenda.Id_Agenda " & vbCrLf)
            stbQ.Append(" INNER JOIN Movimenti_dettagli ON Movimenti.PIVA = Movimenti_dettagli.PIVA  " & vbCrLf)
            stbQ.Append(" AND Movimenti.Id_Agenda = Movimenti_dettagli.Id_Agenda AND  Movimenti.Id_Mov = Movimenti_dettagli.Id_Mov " & vbCrLf)
            stbQ.Append(" INNER JOIN UnitaMisura ON Movimenti_dettagli.udm_cod=UnitaMisura.udm_cod " & vbCrLf)

            stbQ.Append(" INNER JOIN OGenerazioni_Anagrafe_Log " & vbCrLf)
            stbQ.Append(" ON OGenerazioni_Anagrafe_Log.Modulo_Generazione= " & Agro_SQL_SaveNum(enum_Omni_Modulo_Generazione.Cantine) & " " & vbCrLf)
            stbQ.Append(" AND OGenerazioni_Anagrafe_Log.Piva_SuperUser= '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' " & vbCrLf)
            stbQ.Append(" AND OGenerazioni_Anagrafe_Log.Elem_Cod= Movimenti_dettagli.Elem_Cod " & vbCrLf)
            stbQ.Append(" AND OGenerazioni_Anagrafe_Log.Mat_Cod= Movimenti_dettagli.Mat_Cod " & vbCrLf)
            stbQ.Append(" -- non faccio il filtro sulla linea (altrimenti se l'anagrafica è loggata in un'altra linea, invece che in questa, non viene su) " & vbCrLf)
            stbQ.Append(" -- AND OGenerazioni_Anagrafe_Log.Linea_Cod= Agenda.linea_cod " & vbCrLf)
            stbQ.Append(" AND OGenerazioni_Anagrafe_Log.piva=Agenda.piva " & vbCrLf)
            stbQ.Append(" AND Tipo_Generazione = " & Agro_SQL_SaveNum(enum_Omni_Tipo_Generazione.CaliLavorazione) & vbCrLf)

            stbQ.Append(" WHERE Movimenti.Piva = '" & Agro_SQL_SaveText(Piva) & "' " & vbCrLf)
            stbQ.Append(" AND Movimenti.Cau_Mov = '" & CStr(CAU_CARICO) & "' " & vbCrLf)

            'modifica del 19/11/2015
            'oltre ai cali voglio leggere il prodotto feccioso
            'stbQ.Append(" AND Movimenti_dettagli.elem_cod = " + Agro_SQL_SaveNum(CALI_LAVORAZIONE) & " " + vbCrLf)
            stbQ.Append(" AND  EXISTS ( ")
            stbQ.Append("             SELECT 1  ")
            stbQ.Append("             FROM OGenerazioni_Anagrafe_Log  ")
            stbQ.Append("             WHERE Movimenti_Dettagli.Mat_Cod = OGenerazioni_Anagrafe_Log.Mat_Cod AND Movimenti_Dettagli.elem_Cod = OGenerazioni_Anagrafe_Log.elem_Cod  " & vbCrLf)
            stbQ.Append("             AND OGenerazioni_Anagrafe_Log.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' " & vbCrLf)
            stbQ.Append("             AND OGenerazioni_Anagrafe_Log.Tipo_Generazione IN ( " & Agro_SQL_SaveNum(enum_Omni_Tipo_Generazione.CaliLavorazione) & "," & Agro_SQL_SaveNum(enum_Omni_Tipo_Generazione.Scarti) & " ) " & vbCrLf)
            stbQ.Append("            )")
          
            stbQ.Append(" AND Linee_Preparazioni.Modulo_Generazione = " & Agro_SQL_SaveNum(enum_Omni_Modulo_Generazione.Cantine) & " " & vbCrLf)

            'stbQ.Append(" AND MPR.Id_Report IN ( " & vbCrLf)
            'stbQ.Append(" " & CStr(enum_AgroReportistica.Frizzanti) & ", " & vbCrLf)
            'stbQ.Append(" " & CStr(enum_AgroReportistica.Spumanti) & " " & vbCrLf)
            'stbQ.Append(" ) " & vbCrLf)

            stbQ.Append(" AND ( Agenda.id_trasformazione = " & Id_Trasformazione.ToString & vbCrLf)
            'stbQ.Append(" OR Agenda.id_trasformazione = ")
            'stbQ.Append("(SELECT Id_Trasformazione_Rif FROM Trasformazioni_Riferimenti WHERE Id_Trasformazione = " & Id_Trasformazione.ToString & "))" & vbCrLf)
            stbQ.Append("       OR Agenda.id_trasformazione IN ")
            stbQ.Append("                               (SELECT DISTINCT Id_Trasformazione_Rif FROM Trasformazioni_Riferimenti WHERE Id_Trasformazione = " & Id_Trasformazione.ToString & " " & vbCrLf)
            stbQ.Append("                               ) " & vbCrLf)
            stbQ.Append("       ) " & vbCrLf)

            If xFiltroAggiuntivo <> "" Then
                stbQ.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                stbQ.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                stbQ.Append(" ORDER BY Movimenti.Data_Movimento ")
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stbQ.ToString, nomeRoutine)
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
