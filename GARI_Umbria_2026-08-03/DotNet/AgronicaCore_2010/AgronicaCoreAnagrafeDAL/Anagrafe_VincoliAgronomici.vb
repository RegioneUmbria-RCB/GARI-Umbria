Imports System.Text
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.UtilityProvider

Public Class Anagrafe_VincoliAgronomici_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi(ByVal Id As Integer,
                          ByVal Piva As String,
                          ByVal Sa_Cod As Integer,
                          ByVal Appezza As Integer,
                          ByVal Id_Reg As Integer,
                          ByVal Progetto_Cod As Integer,
                          ByVal Pua_Cod As Integer,
                          ByVal Regolamento_Cod As Integer,
                          ByVal Analisi_Testata_Cod As Integer,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                          ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Anagrafe_VincoliAgronomici_R.Leggi()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0
            strSql.AppendLine(" SELECT * ")
            strSql.AppendLine(" FROM  Anagrafe_VincoliAgronomici ")
            strSql.AppendLine(" WHERE Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            strSql.AppendLine(" AND   Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine))
            strSql.AppendLine(" AND   Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio))

            If Id <> 0 Then
                strSql.AppendLine(" AND ID =  " & Agro_SQL_SaveNum(Id))
            End If

            If Piva <> "" Then
                strSql.AppendLine(" AND Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If

            If Sa_Cod <> 0 Then
                strSql.AppendLine(" AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            End If

            If Appezza <> 0 Then
                strSql.AppendLine(" AND Appezza = " & Agro_SQL_SaveNum(Appezza) & " ")
            End If

            If Id_Reg <> 0 Then
                strSql.AppendLine(" AND Id_Reg =  " & Agro_SQL_SaveNum(Id_Reg) & " ")
            End If

            If Progetto_Cod <> 0 Then
                strSql.AppendLine(" AND Progetto_Cod =  " & Agro_SQL_SaveNum(Progetto_Cod) & " ")
            End If

            If Pua_Cod <> 0 Then
                strSql.AppendLine(" AND Pua_Cod = " & Agro_SQL_SaveNum(Pua_Cod) & " ")
            End If

            If Regolamento_Cod <> 0 Then
                strSql.AppendLine(" AND Regolamento_Cod = " & Agro_SQL_SaveNum(Regolamento_Cod) & " ")
            End If

            If Analisi_Testata_Cod <> 0 Then
                strSql.AppendLine(" AND Analisi_Testata_Cod = " & Agro_SQL_SaveNum(Analisi_Testata_Cod) & " ")
            End If
            
            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.AppendLine(" AND   Anagrafe_VincoliAgronomici.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.AppendLine(" AND   Anagrafe_VincoliAgronomici.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                strSql.AppendLine(" ORDER BY Pua_Cod, Regolamento_Cod, Piva, Sa_Cod, Appezza, Id_Reg, Progetto_Cod ")
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

Public Class Anagrafe_VincoliAgronomici_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Scrivi(ByVal Id As Integer,
                           ByVal Piva As String,
                           ByVal Sa_Cod As Integer,
                           ByVal Appezza As Integer,
                           ByVal Id_Reg As Integer,
                           ByVal Progetto_Cod As Integer,
                           ByVal Pua_Cod As Integer,
                           ByVal Regolamento_Cod As Integer,
                           ByVal Analisi_Testata_Cod As Integer,
                           ByVal Veg_Cod_Prec As Integer,
                           ByVal Ubicazione_Cod As Integer,
                           ByVal TipoAcqua_Cod As Integer,
                           ByVal N_FertilizzazioniPrecedenti As Decimal,
                           ByVal Validita_Inizio As Date,
                           ByVal Validita_Fine As Date,
                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                           Optional ByVal Data_creazione As DateTime = #2/1/1900#,
                           Optional ByVal Data_modifica As DateTime = #2/1/1900#,
                           Optional ByVal username_creazione As String = "",
                           Optional ByVal username_modifica As String = ""
                           ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Anagrafe_VincoliAgronomici_W.Scrivi()"


        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        If Data_creazione = #2/1/1900# Then
            Data_creazione = Now
        End If

        If Data_modifica = #2/1/1900# Then
            Data_modifica = Now
        End If

        If username_creazione = "" Then
            username_creazione = objParametri.UsernameOperazione
        End If

        If username_modifica = "" Then
            username_modifica = objParametri.UsernameOperazione
        End If

        Try

            '---------------------------------------------
            strSql.Length = 0
            strSql.AppendLine("INSERT INTO Anagrafe_VincoliAgronomici ( ")
            strSql.AppendLine("             Piva_SuperUser,       Id, ")
            strSql.AppendLine("             Piva,                 Sa_Cod,         Appezza,          Id_Reg,        Progetto_Cod, ")
            strSql.AppendLine("             Pua_Cod,              Regolamento_Cod, ")
            strSql.AppendLine("             Analisi_Testata_Cod,  Veg_Cod_Prec,   Ubicazione_Cod,   TipoAcqua_Cod, N_FertilizzazioniPrecedenti, ")

            strSql.AppendLine("             Inviato,            DataInvio, ")
            strSql.AppendLine("             Data_Creazione,     Data_Modifica, ")
            strSql.AppendLine("             UserName_Creazione, UserName_Modifica, ")
            strSql.AppendLine("             Validita_Inizio,    Validita_Fine ")
            strSql.AppendLine("             ) ")

            strSql.AppendLine(" VALUES (")
            strSql.AppendLine("          '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Id))
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(Piva) & "' ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Sa_Cod))
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Appezza))
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Id_Reg))
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Progetto_Cod))

            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Pua_Cod))
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Regolamento_Cod))

            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Analisi_Testata_Cod))
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Veg_Cod_Prec))
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Ubicazione_Cod))
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(TipoAcqua_Cod))
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(N_FertilizzazioniPrecedenti))

            strSql.AppendLine("         , 0 ")
            strSql.AppendLine("         , NULL ")
            strSql.AppendLine("         , " & Agro_SQL_SaveDateTime(Data_creazione))
            strSql.AppendLine("         , " & Agro_SQL_SaveDateTime(Data_modifica))
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(username_modifica) & "' ")
            strSql.AppendLine("         , " & Agro_SQL_SaveDate(Validita_Inizio))
            strSql.AppendLine("         , " & Agro_SQL_SaveDate(Validita_Fine))

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

    Public Function Modifica(ByVal Old_ID As Integer,
                             ByVal Old_Piva As String,
                             ByVal Old_Sa_Cod As Integer,
                             ByVal Old_Appezza As Integer,
                             ByVal Old_Id_Reg As Integer,
                             ByVal Old_Progetto_Cod As Integer,
                             ByVal Old_Pua_Cod As Integer,
                             ByVal Old_Regolamento_Cod As Integer,
                             ByVal New_Analisi_Testata_Cod As Integer,
                             ByVal New_Veg_Cod_Prec As Integer,
                             ByVal New_Ubicazione_Cod As Integer,
                             ByVal New_TipoAcqua_Cod As Integer,
                             ByVal New_N_FertilizzazioniPrecedenti As Decimal,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                             Optional ByVal Data_Modifica As DateTime = #2/1/1900#,
                             Optional ByVal Username_Modifica As String = ""
                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Anagrafe_VincoliAgronomici_W.Modifica()"

        '====================================================================================
        'Parametri opzionali :
        '   Old_ID = 0
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try
            If Data_Modifica = #2/1/1900# Then
                Data_Modifica = Now
            End If

            If Username_Modifica = "" Then
                Username_Modifica = objParametri.UsernameOperazione
            End If

            strSql.Length = 0
            strSql.AppendLine(" UPDATE Anagrafe_VincoliAgronomici SET ")

            strSql.AppendLine(" Analisi_Testata_Cod = " & Agro_SQL_SaveNum(New_Analisi_Testata_Cod) & ",  ")
            strSql.AppendLine(" Veg_Cod_Prec = " & Agro_SQL_SaveNum(New_Veg_Cod_Prec) & ",  ")
            strSql.AppendLine(" Ubicazione_Cod = " & Agro_SQL_SaveNum(New_Ubicazione_Cod) & ",  ")
            strSql.AppendLine(" TipoAcqua_Cod = " & Agro_SQL_SaveNum(New_TipoAcqua_Cod) & ",  ")
            strSql.AppendLine(" N_FertilizzazioniPrecedenti = " & Agro_SQL_SaveNum(New_N_FertilizzazioniPrecedenti) & ",  ")

            strSql.AppendLine(" Data_Modifica = " & Agro_SQL_SaveDateTime(Data_Modifica) & ", ")
            strSql.AppendLine(" Username_Modifica = '" & Agro_SQL_SaveText(Username_Modifica) & "'")

            strSql.AppendLine(" WHERE Piva = '" & Agro_SQL_SaveText(Old_Piva) & "'  ")
            strSql.AppendLine(" AND Sa_Cod = " & Agro_SQL_SaveNum(Old_Sa_Cod) & "  ")
            strSql.AppendLine(" AND Appezza = " & Agro_SQL_SaveText(Old_Appezza) & "  ")
            strSql.AppendLine(" AND Id_Reg = " & Agro_SQL_SaveNum(Old_Id_Reg))
            strSql.AppendLine(" AND Progetto_Cod = " & Agro_SQL_SaveNum(Old_Progetto_Cod))
            
            strSql.AppendLine(" AND Pua_Cod = " & Agro_SQL_SaveNum(Old_Pua_Cod) & " ")
            strSql.AppendLine(" AND Regolamento_Cod = " & Agro_SQL_SaveNum(Old_Regolamento_Cod) & " ")

            If Old_ID <> 0 Then
                strSql.AppendLine(" AND ID = " & Agro_SQL_SaveNum(Old_ID))
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

    Public Function ModificaPuntuale(ByVal Id As Integer,
                                     ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                     Optional ByVal Analisi_Testata_Cod As Integer? = Nothing,
                                     Optional ByVal Veg_Cod_Prec As Integer? = Nothing,
                                     Optional ByVal Ubicazione_Cod As Integer? = Nothing,
                                     Optional ByVal TipoAcqua_Cod As Integer? = Nothing,
                                     Optional ByVal N_FertilizzazioniPrecedenti As Decimal? = Nothing,
                                     Optional ByVal Validita_Inizio As Date? = Nothing,
                                     Optional ByVal Validita_Fine As Date? = Nothing,
                                     Optional ByVal Data_Modifica As DateTime = #2/1/1900#,
                                     Optional ByVal Username_Modifica As String = ""
                                     ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Anagrafe_VincoliAgronomici_W.ModificaPuntuale()"

        '====================================================================================
        'Parametri opzionali :
        '   Tutti i valori non chiave (se impostati a nothing o non passati 
        '   non ne verrà fatto l'aggiornamento e rimarranno i valori precedenti)
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try
            If Data_Modifica = #2/1/1900# Then
                Data_Modifica = Now
            End If

            If Username_Modifica = "" Then
                Username_Modifica = objParametri.UsernameOperazione
            End If

            If Id = 0 Then
                Throw New Exception("Parametro non corretto nella query (ID obbligatorio)")
            End If

            '---------------------------------------------
            strSql.Length = 0

            strSql.AppendLine(" UPDATE Anagrafe_VincoliAgronomici ")
            strSql.AppendLine(" SET Data_Modifica = " & Agro_SQL_SaveDateTime(Data_Modifica) & " ")
            strSql.AppendLine("   , Username_Modifica = '" & Agro_SQL_SaveText(Username_Modifica) & "' ")


            If Not IsNothing(Analisi_Testata_Cod) Then
                strSql.AppendLine("   , Analisi_Testata_Cod = " & Agro_SQL_SaveNum(Analisi_Testata_Cod) & " ")
            End If

            If Not IsNothing(Veg_Cod_Prec) Then
                strSql.AppendLine("   , Veg_Cod_Prec = " & Agro_SQL_SaveNum(Veg_Cod_Prec) & " ")
            End If

            If Not IsNothing(Ubicazione_Cod) Then
                strSql.AppendLine("   , Ubicazione_Cod = " & Agro_SQL_SaveNum(Ubicazione_Cod) & " ")
            End If
            
            If Not IsNothing(TipoAcqua_Cod) Then
                strSql.AppendLine("   , TipoAcqua_Cod = " & Agro_SQL_SaveNum(TipoAcqua_Cod) & " ")
            End If
            
            If Not IsNothing(N_FertilizzazioniPrecedenti) Then
                strSql.AppendLine("   , N_FertilizzazioniPrecedenti = " & Agro_SQL_SaveNum(N_FertilizzazioniPrecedenti) & " ")
            End If
            
            If Not IsNothing(Validita_Inizio) Then
                strSql.AppendLine("   , Validita_Inizio = " & Agro_SQL_SaveDate(CDate(Validita_Inizio)) & " ")
            End If
            
            If Not IsNothing(Validita_Fine) Then
                strSql.AppendLine("   , Validita_Fine = " & Agro_SQL_SaveDate(CDate(Validita_Fine)) & " ")
            End If
            

            strSql.AppendLine(" WHERE ID = " & Agro_SQL_SaveNum(Id) & " ")

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

    Public Function CancellaById(ByVal Id As Integer,
                                 ByVal xFiltroAggiuntivo As String,
                                 ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                 ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Anagrafe_VincoliAgronomici_W.CancellaById()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try
            
            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then
                strSql.Length = 0
                strSql.AppendLine(" UPDATE Anagrafe_VincoliAgronomici ")
                strSql.AppendLine(" SET  Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                strSql.AppendLine("    , Inviato = -1 ")
                strSql.AppendLine(" WHERE   Inviato >= 0")
            Else
                strSql.Length = 0
                strSql.AppendLine(" DELETE ")
                strSql.AppendLine(" FROM    Anagrafe_VincoliAgronomici ")
                strSql.AppendLine(" WHERE   1 = 1  ")
            End If

            strSql.AppendLine(" AND      Piva_SuperUser    = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            strSql.AppendLine(" AND      ID                = " & Agro_SQL_SaveNum(Id) & "  ")

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

    Public Function CancellaByPuaCod(ByVal Piva As String,
                                     ByVal Pua_Cod As Integer,
                                     ByVal Regolamento_Cod As Integer,
                                     ByVal xFiltroAggiuntivo As String,
                                     ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                     ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Anagrafe_VincoliAgronomici_W.CancellaByPuaCod()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try
            
            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then
                strSql.Length = 0
                strSql.AppendLine(" UPDATE Anagrafe_VincoliAgronomici ")
                strSql.AppendLine(" SET  Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                strSql.AppendLine("    , Inviato = -1 ")
                strSql.AppendLine(" WHERE   Inviato >= 0")
            Else
                strSql.Length = 0
                strSql.AppendLine(" DELETE ")
                strSql.AppendLine(" FROM    Anagrafe_VincoliAgronomici ")
                strSql.AppendLine(" WHERE   1=1")
            End If

            strSql.AppendLine(" AND      Piva_SuperUser    = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            strSql.AppendLine(" AND      Piva              = '" & Agro_SQL_SaveText(Piva) & "' ")
            strSql.AppendLine(" AND      Pua_Cod           = " & Agro_SQL_SaveNum(Pua_Cod) & " ")
            strSql.AppendLine(" AND      Regolamento_Cod   = " & Agro_SQL_SaveNum(Regolamento_Cod) & " ")

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

    Public Function CancellaByAnagrafe(ByVal Piva As String,
                                       ByVal Sa_Cod As Integer,
                                       ByVal Appezza As Integer,
                                       ByVal Id_Reg As Integer,
                                       ByVal Progetto_Cod As Integer,
                                       ByVal xFiltroAggiuntivo As String,
                                       ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                       ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Anagrafe_VincoliAgronomici_W.CancellaByAnagrafe()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try
            
            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then
                strSql.Length = 0
                strSql.AppendLine(" UPDATE Anagrafe_VincoliAgronomici ")
                strSql.AppendLine(" SET  Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                strSql.AppendLine("    , Inviato = -1 ")
                strSql.AppendLine(" WHERE   Inviato >= 0")
            Else
                strSql.Length = 0
                strSql.AppendLine(" DELETE ")
                strSql.AppendLine(" FROM    Anagrafe_VincoliAgronomici ")
                strSql.AppendLine(" WHERE   1=1")
            End If

            strSql.AppendLine(" AND      Piva_SuperUser    = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            strSql.AppendLine(" AND      Piva              = '" & Agro_SQL_SaveText(Piva) & "' ")

            If Sa_Cod <> 0 Then
                strSql.AppendLine(" AND      Sa_Cod            = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            End If

            If Appezza <> 0 Then
                strSql.AppendLine(" AND      Appezza           = " & Agro_SQL_SaveNum(Appezza) & " ")
            End If

            If Id_Reg <> 0 Then
                strSql.AppendLine(" AND      Id_Reg            = " & Agro_SQL_SaveNum(Id_Reg) & " ")
            End If

            If Progetto_Cod <> 0 Then
                strSql.AppendLine(" AND      Progetto_Cod      = " & Agro_SQL_SaveNum(Progetto_Cod) & " ")
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

End Class
