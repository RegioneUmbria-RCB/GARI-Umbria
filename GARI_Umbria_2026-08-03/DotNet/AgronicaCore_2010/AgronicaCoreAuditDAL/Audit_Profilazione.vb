Imports AgronicaCoreDataProvider.UtilityProvider

Public Class Audit_Profilazione_R

    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function AuditZona(
                                ByVal PivaSuperUser As String,
                                ByVal Piva As String,
                                ByVal CodiceZona As Integer,
                                ByVal xFiltroAggiuntivo As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.ZonexParticelle_R.AuditZona()"

        '====================================================================================
        'Parametri opzionali :
        '
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim bRet As Boolean

        Try
            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append(" SELECT COUNT(*) ")
            StrSQL.Append(" FROM  ZonexParticelle INNER JOIN ")
            StrSQL.Append(" ImpreseXParticelle ON ZonexParticelle.PROV = ImpreseXParticelle.PROV AND ZonexParticelle.COM = ImpreseXParticelle.COM AND ")
            StrSQL.Append(" ZonexParticelle.SEZIONE = ImpreseXParticelle.SEZIONE AND ZonexParticelle.FOGLIO = ImpreseXParticelle.FOGLIO AND ")
            StrSQL.Append(" ZonexParticelle.NUMERO = ImpreseXParticelle.NUMERO And ZonexParticelle.SUBALTERNO = ImpreseXParticelle.SUBALTERNO ")
            StrSQL.Append(" WHERE 1=1 ")

            If PivaSuperUser <> "" Then
                StrSQL.Append(" AND ZonexParticelle.Piva_SuperUser = '" & Replace(PivaSuperUser, "'", "''") & "' ")
            End If

            If Piva <> "" Then
                StrSQL.Append(" AND ImpreseXParticelle.PIVA = '" & Agro_SQL_SaveText(Piva) & "'")
            End If

            If CodiceZona <> 0 Then
                StrSQL.Append(" AND ZonexParticelle.Zona_Cod =  " & Agro_SQL_SaveNum(CodiceZona) & " ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            ' se esiste almeno una particella dell'azienda in Zona_Cod, restituisco 0
            bRet = False
            If DT.Rows.Count > 0 AndAlso Not IsNothing(DT.Rows(0).Item(0)) Then
                bRet = IIf(DT.Rows(0).Item(0) > 0, True, False)
            End If

            DT = Nothing

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            bRet = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return bRet

    End Function

    ' la funzione restituisce true se l'azienda ha almeno una consistenza animale del genere (se specificato)
    '################################################################################################################################
    Public Function AuditZoo(ByVal PivaSuperUser As String,
                                ByVal Piva As String,
                                ByVal GenCod As Integer,
                                ByVal xFiltroAggiuntivo As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAuditDAL.Audit_Profilazione_R.AuditZoo()"

        '====================================================================================
        'Parametri opzionali :
        '
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim bRet As Boolean

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT count(*) ")
            StrSQL.Append(" FROM Zoo_Animali ")
            StrSQL.Append(" WHERE 1=1 ")

            If PivaSuperUser <> "" Then
                StrSQL.Append(" AND ZonexParticelle.Piva_SuperUser= '" & Agro_SQL_SaveText(PivaSuperUser) & "'")
            End If

            If Piva <> "" Then
                StrSQL.Append(" AND Zoo_Animali.Piva = '" & Agro_SQL_SaveText(Piva) & "'")
            End If

            If GenCod <> 0 Then
                StrSQL.Append(" AND Gen_Cod = " & Agro_SQL_SaveNum(GenCod))
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            If DT.Rows.Count > 0 AndAlso Not IsNothing(DT.Rows(0).Item(0)) Then
                bRet = IIf(DT.Rows(0).Item(0) > 0, True, False)
            End If

            DT = Nothing

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            bRet = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return bRet

    End Function


    ' la funzione restituisce true se l'azienda ha registrato almeno un trattamento (cau_mov=2050)
    '################################################################################################################################
    Public Function AuditPFS(ByVal PivaSuperUser As String,
                             ByVal Piva As String,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAuditDAL.Audit_Profilazione_R.AuditZoo()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim bRet As Boolean

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT count(*) ")
            StrSQL.Append(" FROM Movimenti ")
            StrSQL.Append(" WHERE Cau_Mov = '2050' ")

            'If PivaSuperUser <> "" Then
            '    StrSQL.Append(" AND Movimenti.Piva_SuperUser= '" & Agro_SQL_SaveText(PivaSuperUser) & "'")
            'End If

            If Piva <> "" Then
                StrSQL.Append(" AND Movimenti.Piva = '" & Agro_SQL_SaveText(Piva) & "'")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            bRet = False
            If DT.Rows.Count > 0 AndAlso Not IsNothing(DT.Rows(0).Item(0)) Then
                bRet = IIf(DT.Rows(0).Item(0) > 0, True, False)
            End If

            DT = Nothing

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            bRet = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return bRet

    End Function

    ' la funzione restituisce true se l'azienda ha almeno un impianto di olivo (Veg_Cod=44)
    '################################################################################################################################
    Public Function AuditOliveti(ByVal PivaSuperUser As String,
                                 ByVal Piva As String,
                                 ByVal xFiltroAggiuntivo As String,
                                 ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAuditDAL.Audit_Profilazione_R.AuditOliveti()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim bRet As Boolean

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT count(*) ")
            StrSQL.Append(" FROM Reg_Impianti ")
            StrSQL.Append(" INNER JOIN Cultivar ON Reg_Impianti.CUL_COD = Cultivar.Cul_Cod ")
            StrSQL.Append(" INNER JOIN SpecieVegetali ON Cultivar.Veg_Cod = SpecieVegetali.Veg_Cod ")
            StrSQL.Append(" WHERE Cultivar.Veg_Cod = 44 ")

            'If PivaSuperUser <> "" Then
            '    StrSQL.Append(" AND ZonexParticelle.Piva_SuperUser= '" & Agro_SQL_SaveText(PivaSuperUser) & "'"
            'End If

            If Piva <> "" Then
                StrSQL.Append(" AND Reg_Impianti.Piva = '" & Agro_SQL_SaveText(Piva) & "'")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            bRet = False
            If DT.Rows.Count > 0 AndAlso Not IsNothing(DT.Rows(0).Item(0)) Then
                bRet = IIf(DT.Rows(0).Item(0) > 0, True, False)
            End If

            DT = Nothing

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            bRet = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return bRet

    End Function


    ' la funzione restituisce true se l'azienda ha almeno un impianto a seminativo
    '################################################################################################################################
    Public Function AuditSeminativi(ByVal PivaSuperUser As String,
                                    ByVal Piva As String,
                                    ByVal xFiltroAggiuntivo As String,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAuditDAL.Audit_Profilazione_R.AuditSeminativi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim bRet As Boolean

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT count(*) ")
            StrSQL.Append(" FROM Reg_Impianti ")
            StrSQL.Append(" INNER JOIN Cultivar ON Reg_Impianti.CUL_COD = Cultivar.Cul_Cod ")
            StrSQL.Append(" INNER JOIN SpecieVegetali ON Cultivar.Veg_Cod = SpecieVegetali.Veg_Cod ")
            StrSQL.Append(" WHERE (SpecieVegetali.Grsp_Cod IN (3, 20, 19, 10, 22) ")
            StrSQL.Append(" OR SpecieVegetali.Veg_Cod IN (26, 37)) ")

            If Piva <> "" Then
                StrSQL.Append(" AND Reg_Impianti.Piva = '" & Agro_SQL_SaveText(Piva) & "'")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            bRet = False
            If DT.Rows.Count > 0 AndAlso Not IsNothing(DT.Rows(0).Item(0)) Then
                bRet = IIf(DT.Rows(0).Item(0) > 0, True, False)
            End If

            DT = Nothing

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            bRet = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return bRet

    End Function

    ' la funzione restituisce true se l'azienda ha almeno una particella
    '################################################################################################################################
    Public Function AuditParticelle(ByVal PivaSuperUser As String,
                                    ByVal Piva As String,
                                    ByVal xFiltroAggiuntivo As String,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAuditDAL.Audit_Profilazione_R.AuditParticelle()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim bRet As Boolean

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT count(*) ")
            StrSQL.Append(" FROM ImpreseXParticelle ")
            StrSQL.Append(" WHERE 1=1 ")

            If Piva <> "" Then
                StrSQL.Append(" AND ImpreseXParticelle.Piva = '" & Agro_SQL_SaveText(Piva) & "'")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            bRet = False
            If DT.Rows.Count > 0 AndAlso Not IsNothing(DT.Rows(0).Item(0)) Then
                bRet = IIf(DT.Rows(0).Item(0) > 0, True, False)
            End If

            DT = Nothing

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            bRet = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return bRet

    End Function

    ' la funzione restituisce true se l'azienda ha registrato almeno un movimento di carico (cau_mov=7300) di carburante (elem_cod=2)
    '################################################################################################################################
    Public Function AuditCarburanti(ByVal PivaSuperUser As String,
                                    ByVal Piva As String,
                                    ByVal xFiltroAggiuntivo As String,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAuditDAL.Audit_Profilazione_R.AuditCarburanti()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim bRet As Boolean

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT count(*) ")
            StrSQL.Append(" FROM Movimenti INNER JOIN ")
            StrSQL.Append(" Movimenti_dettagli ON Movimenti.PIVA = Movimenti_dettagli.PIVA AND Movimenti.Sa_Cod = Movimenti_dettagli.Sa_Cod AND ")
            StrSQL.Append(" Movimenti.Id_Agenda = Movimenti_dettagli.Id_Agenda And Movimenti.Id_Mov = Movimenti_dettagli.Id_Mov ")
            StrSQL.Append(" WHERE  (Movimenti.Cau_Mov = '7300') AND (Movimenti_dettagli.Elem_Cod = 2) ")


            If Piva <> "" Then
                StrSQL.Append(" AND Movimenti.Piva = '" & Agro_SQL_SaveText(Piva) & "'")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            bRet = False
            If DT.Rows.Count > 0 AndAlso Not IsNothing(DT.Rows(0).Item(0)) Then
                bRet = IIf(DT.Rows(0).Item(0) > 0, True, False)
            End If

            DT = Nothing

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            bRet = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return bRet

    End Function

    ' la funzione restituisce true se l'azienda ha registrato almeno macchinario
    '################################################################################################################################
    Public Function AuditParcoMacchine(ByVal PivaSuperUser As String,
                                    ByVal Piva As String,
                                    ByVal xFiltroAggiuntivo As String,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAuditDAL.Audit_Profilazione_R.AuditParcoMacchine()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim bRet As Boolean

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT count(*) ")
            StrSQL.Append(" FROM Parco_Macchine WHERE 1=1 ")

            If Piva <> "" Then
                StrSQL.Append(" AND Parco_Macchine.Piva = '" & Agro_SQL_SaveText(Piva) & "'")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            bRet = False
            If DT.Rows.Count > 0 AndAlso Not IsNothing(DT.Rows(0).Item(0)) Then
                bRet = IIf(DT.Rows(0).Item(0) > 0, True, False)
            End If

            DT = Nothing

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            bRet = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return bRet

    End Function

    ' la funzione restituisce true se l'azienda ha almeno un impianto adibito a prato stabile
    '################################################################################################################################
    Public Function AuditPascoloPermanente(ByVal PivaSuperUser As String,
                                    ByVal Piva As String,
                                    ByVal xFiltroAggiuntivo As String,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAuditDAL.Audit_Profilazione_R.AuditPascoloPermanente()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim bRet As Boolean

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT count(*) ")
            StrSQL.Append(" FROM Reg_Impianti ")
            StrSQL.Append(" INNER JOIN Cultivar ON Reg_Impianti.CUL_COD = Cultivar.Cul_Cod ")
            StrSQL.Append(" INNER JOIN SpecieVegetali ON Cultivar.Veg_Cod = SpecieVegetali.Veg_Cod ")
            StrSQL.Append(" WHERE SpecieVegetali.Veg_Cod IN (53) ")

            If Piva <> "" Then
                StrSQL.Append(" AND Reg_Impianti.Piva = '" & Agro_SQL_SaveText(Piva) & "'")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            bRet = False
            If DT.Rows.Count > 0 AndAlso Not IsNothing(DT.Rows(0).Item(0)) Then
                bRet = IIf(DT.Rows(0).Item(0) > 0, True, False)
            End If

            DT = Nothing

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            bRet = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return bRet

    End Function

    ' la funzione restituisce true se l'azienda ha almeno un impianto adibito a prato stabile
    '################################################################################################################################
    Public Function AuditSetAside(ByVal PivaSuperUser As String,
                                    ByVal Piva As String,
                                    ByVal xFiltroAggiuntivo As String,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAuditDAL.Audit_Profilazione_R.AuditSetAside()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim bRet As Boolean

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT count(*) ")
            StrSQL.Append(" FROM Reg_Impianti ")
            StrSQL.Append(" INNER JOIN Reg_Impianti_Codici ON Reg_Impianti.PIVA = Reg_Impianti_Codici.PIVA ")
            StrSQL.Append(" AND Reg_Impianti.Sa_Cod = Reg_Impianti_Codici.Sa_Cod ")
            StrSQL.Append(" AND Reg_Impianti.Appezza = Reg_Impianti_Codici.Appezza ")
            StrSQL.Append(" AND Reg_Impianti.Id_Reg = Reg_Impianti_Codici.Id_Reg ")
            StrSQL.Append(" AND Reg_Impianti_Codici.id_cod = 3001 ")
            StrSQL.Append(" AND (Reg_Impianti.Validita_Inizio <= GetDate() AND Reg_Impianti.Validita_Fine >= GetDate()) ")

            If Piva <> "" Then
                StrSQL.Append(" AND Reg_Impianti.Piva = '" & Agro_SQL_SaveText(Piva) & "'")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            bRet = False
            If DT.Rows.Count > 0 AndAlso Not IsNothing(DT.Rows(0).Item(0)) Then
                bRet = IIf(DT.Rows(0).Item(0) > 0, True, False)
            End If

            DT = Nothing

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            bRet = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return bRet

    End Function

End Class
