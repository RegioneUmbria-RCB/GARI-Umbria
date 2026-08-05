Imports System.Text
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.UtilityProvider

Public Class ParticelleCatastalixVincoliAgronomici_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi(ByVal id As Integer,
                          ByVal PROV As String,
                          ByVal COM As String,
                          ByVal SEZIONE As String,
                          ByVal FOGLIO As Integer,
                          ByVal NUMERO As Integer,
                          ByVal SUBALTERNO As String,
                          ByVal Cul_Cod_Agea As String,
                          ByVal Uso_Cod As String,
                          ByVal Macrouso_Cod As String,
                          ByVal Occupazione_Cod As String,
                          ByVal Destinazione_Cod As String,
                          ByVal Qualita_Cod As String,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                          ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.ParticelleCatastalixVincoliAgronomici_R.Leggi()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0
            strSql.AppendLine(" SELECT * ")
            strSql.AppendLine(" FROM  ParticelleCatastalixVincoliAgronomici ")
            strSql.AppendLine(" WHERE Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine))
            strSql.AppendLine(" AND   Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio))

            If id <> 0 Then
                strSql.AppendLine(" AND ParticelleCatastalixVincoliAgronomici.id =  " & Agro_SQL_SaveNum(id))
            End If

            If PROV <> "" Then
                strSql.AppendLine(" AND ParticelleCatastalixVincoliAgronomici.PROV = '" & Agro_SQL_SaveText(Trim(PROV)) & "' ")
            End If

            If COM <> "" Then
                strSql.AppendLine(" AND ParticelleCatastalixVincoliAgronomici.COM = '" & Agro_SQL_SaveText(Trim(COM)) & "' ")
            End If

            If SEZIONE <> "" Then
                strSql.AppendLine(" AND ParticelleCatastalixVincoliAgronomici.SEZIONE = '" & Agro_SQL_SaveText(LCase(SEZIONE)) & "' ")
            End If

            If FOGLIO <> 0 Then
                strSql.AppendLine(" AND ParticelleCatastalixVincoliAgronomici.FOGLIO =  " & Agro_SQL_SaveNum(FOGLIO))
            End If

            If NUMERO <> 0 Then
                strSql.AppendLine(" AND ParticelleCatastalixVincoliAgronomici.NUMERO =  " & Agro_SQL_SaveNum(NUMERO))
            End If

            If SUBALTERNO <> "" Then
                strSql.AppendLine(" AND ParticelleCatastalixVincoliAgronomici.SUBALTERNO = '" & Agro_SQL_SaveText(Trim(LCase(SUBALTERNO))) & "' ")
            End If

            If Cul_Cod_Agea <> "" Then
                strSql.AppendLine(" AND ParticelleCatastalixVincoliAgronomici.Cul_Cod_Agea = '" & Agro_SQL_SaveText(Cul_Cod_Agea) & "' ")
            End If

            If Uso_Cod <> "" Then
                strSql.AppendLine(" AND ParticelleCatastalixVincoliAgronomici.Uso_Cod = '" & Agro_SQL_SaveText(Uso_Cod) & "' ")
            End If

            If Macrouso_Cod <> "" Then
                strSql.AppendLine(" AND ParticelleCatastalixVincoliAgronomici.Macrouso_Cod = '" & Agro_SQL_SaveText(Macrouso_Cod) & "' ")
            End If

            If Occupazione_Cod <> "" Then
                strSql.AppendLine(" AND ParticelleCatastalixVincoliAgronomici.Occupazione_Cod = '" & Agro_SQL_SaveText(Occupazione_Cod) & "' ")
            End If

            If Destinazione_Cod <> "" Then
                strSql.AppendLine(" AND ParticelleCatastalixVincoliAgronomici.Destinazione_Cod = '" & Agro_SQL_SaveText(Destinazione_Cod) & "' ")
            End If

            If Qualita_Cod <> "" Then
                strSql.AppendLine(" AND ParticelleCatastalixVincoliAgronomici.Qualita_Cod = '" & Agro_SQL_SaveText(Qualita_Cod) & "' ")
            End If


            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.AppendLine(" AND   ParticelleCatastalixVincoliAgronomici.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.AppendLine(" AND   ParticelleCatastalixVincoliAgronomici.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                strSql.AppendLine(" ORDER BY ParticelleCatastalixVincoliAgronomici.Validita_inizio ASC")
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

Public Class ParticelleCatastalixVincoliAgronomici_W
    Inherits AgronicaCoreDataProvider.DataProvider


    Public Function Scrivi(ByVal ID As Integer,
                           ByVal PROV As String,
                           ByVal COM As String,
                           ByVal SEZIONE As String,
                           ByVal FOGLIO As Integer,
                           ByVal NUMERO As Integer,
                           ByVal SUBALTERNO As String,
                           ByVal Id_Mat_O As Integer,
                           ByVal Id_Fre As Integer,
                           ByVal Veg_Cod_Prec As Integer,
                           ByVal Analisi_Testata_Cod As Integer,
                           ByVal Ubicazione_Cod As Integer,
                           ByVal N_Distribuito As Decimal,
                           ByVal TipoAcqua_Cod As Integer,
                           ByVal Cul_Cod_Agea As String,
                           ByVal Uso_Cod As String,
                           ByVal Macrouso_Cod As String,
                           ByVal Occupazione_Cod As String,
                           ByVal Destinazione_Cod As String,
                           ByVal Qualita_Cod As String,
                           ByVal Validita_Inizio As Date,
                           ByVal Validita_Fine As Date,
                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                           Optional ByVal Data_creazione As Date = #2/1/1900#,
                           Optional ByVal Data_modifica As Date = #2/1/1900#,
                           Optional ByVal username_creazione As String = "",
                           Optional ByVal username_modifica As String = ""
                           ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.ParticelleCatastalixVincoliAgronomici_W.Scrivi()"


        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

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

        Try

            '---------------------------------------------
            strSql.Length = 0
            strSql.AppendLine("INSERT INTO ParticelleCatastalixVincoliAgronomici ( ")
            strSql.AppendLine("             ID,                 PROV,           COM,              SEZIONE, ")
            strSql.AppendLine("             FOGLIO,             NUMERO,         SUBALTERNO,  ")
            strSql.AppendLine("             Id_Mat_O,           Id_Fre,         Veg_Cod_Prec,     Analisi_Testata_Cod, ")
            strSql.AppendLine("             Ubicazione_Cod,     N_Distribuito,  TipoAcqua_Cod, ")
            strSql.AppendLine("             Cul_Cod_Agea,       Uso_Cod,           Macrouso_Cod, ")
            strSql.AppendLine("             Occupazione_Cod,    Destinazione_Cod,  Qualita_Cod, ")

            strSql.AppendLine("             Inviato,            DataInvio, ")
            strSql.AppendLine("             Data_Creazione,     Data_Modifica, ")
            strSql.AppendLine("             UserName_Creazione, UserName_Modifica, ")
            strSql.AppendLine("             Validita_Inizio,    Validita_Fine ")
            strSql.AppendLine("             ) ")

            strSql.AppendLine(" VALUES (")
            strSql.AppendLine("           " & Agro_SQL_SaveNum(ID))
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(Trim(PROV)) & "' ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(Trim(COM)) & "' ")
            strSql.AppendLine("         ,'" & IIf(Agro_SQL_SaveText(SEZIONE, False) <> "", Agro_SQL_SaveText(LCase(SEZIONE)), 0) & "' ")
            strSql.AppendLine("         , " & IIf(Agro_SQL_SaveNum(FOGLIO, False) <> 0, Agro_SQL_SaveNum(FOGLIO), 0))
            strSql.AppendLine("         , " & IIf(Agro_SQL_SaveNum(NUMERO, False) <> 0, Agro_SQL_SaveNum(NUMERO), 0))
            strSql.AppendLine("         ,'" & IIf(Agro_SQL_SaveText(SUBALTERNO, False) <> "", Agro_SQL_SaveText(LCase(SUBALTERNO)), 0) & "' ")

            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Id_Mat_O))
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Id_Fre))
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Veg_Cod_Prec))
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Analisi_Testata_Cod))
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Ubicazione_Cod))
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(N_Distribuito))
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(TipoAcqua_Cod))

            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(Cul_Cod_Agea) & "' ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(Uso_Cod) & "' ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(Macrouso_Cod) & "' ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(Occupazione_Cod) & "' ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(Destinazione_Cod) & "' ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(Qualita_Cod) & "' ")

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
                             ByVal Old_PROV As String,
                             ByVal Old_COM As String,
                             ByVal Old_SEZIONE As String,
                             ByVal Old_FOGLIO As Integer,
                             ByVal Old_NUMERO As Integer,
                             ByVal Old_SUBALTERNO As String,
                             ByVal Old_Cul_Cod_Agea As String,
                             ByVal Old_Uso_Cod As String,
                             ByVal Old_Macrouso_Cod As String,
                             ByVal Old_Occupazione_Cod As String,
                             ByVal Old_Destinazione_Cod As String,
                             ByVal Old_Qualita_Cod As String,
                             ByVal New_Id_Mat_O As Integer,
                             ByVal New_Id_Fre As Integer,
                             ByVal New_Veg_Cod_Prec As Integer,
                             ByVal New_Analisi_Testata_Cod As Integer,
                             ByVal New_Ubicazione_Cod As Integer,
                             ByVal New_N_Distribuito As Decimal,
                             ByVal New_TipoAcqua_Cod As Integer,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.ParticelleCatastalixVincoliAgronomici_W.Modifica()"

        '====================================================================================
        'Parametri opzionali :
        '   Old_ID = 0
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            strSql.Length = 0
            strSql.AppendLine(" UPDATE ParticelleCatastalixVincoliAgronomici SET ")

            strSql.AppendLine(" Id_Mat_O =" & Agro_SQL_SaveNum(New_Id_Mat_O) & ",  ")
            strSql.AppendLine(" Id_Fre =" & Agro_SQL_SaveNum(New_Id_Fre) & ",  ")
            strSql.AppendLine(" Veg_Cod_Prec =" & Agro_SQL_SaveNum(New_Veg_Cod_Prec) & ",  ")
            strSql.AppendLine(" Analisi_Testata_Cod =" & Agro_SQL_SaveNum(New_Analisi_Testata_Cod) & ",  ")
            strSql.AppendLine(" Ubicazione_Cod =" & Agro_SQL_SaveNum(New_Ubicazione_Cod) & ",  ")
            strSql.AppendLine(" N_Distribuito =" & Agro_SQL_SaveNum(New_N_Distribuito) & ",  ")
            strSql.AppendLine(" TipoAcqua_Cod =" & Agro_SQL_SaveNum(New_TipoAcqua_Cod) & ",  ")

            strSql.AppendLine(" Data_Modifica =" & Agro_SQL_SaveDateTime(DateTime.Now) & ", ")
            strSql.AppendLine(" Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")

            strSql.AppendLine(" WHERE PROV = '" & Agro_SQL_SaveText(Old_PROV) & "'  ")
            strSql.AppendLine(" AND COM = '" & Agro_SQL_SaveText(Old_COM) & "'  ")
            strSql.AppendLine(" AND SEZIONE ='" & Agro_SQL_SaveText(Old_SEZIONE) & "'  ")
            strSql.AppendLine(" AND FOGLIO =" & Agro_SQL_SaveNum(Old_FOGLIO))
            strSql.AppendLine(" AND NUMERO =" & Agro_SQL_SaveNum(Old_NUMERO))
            strSql.AppendLine(" AND Subalterno = '" & Agro_SQL_SaveText(Old_SUBALTERNO) & "' ")

            strSql.AppendLine(" AND Cul_Cod_Agea = '" & Agro_SQL_SaveText(Old_Cul_Cod_Agea) & "' ")
            strSql.AppendLine(" AND Uso_Cod = '" & Agro_SQL_SaveText(Old_Uso_Cod) & "' ")
            strSql.AppendLine(" AND Macrouso_Cod = '" & Agro_SQL_SaveText(Old_Macrouso_Cod) & "' ")
            strSql.AppendLine(" AND Occupazione_Cod = '" & Agro_SQL_SaveText(Old_Occupazione_Cod) & "' ")
            strSql.AppendLine(" AND Destinazione_Cod = '" & Agro_SQL_SaveText(Old_Destinazione_Cod) & "' ")
            strSql.AppendLine(" AND Qualita_Cod = '" & Agro_SQL_SaveText(Old_Qualita_Cod) & "' ")

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

    Public Function ModificaPuntuale(ByVal ID As Integer,
                                     ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                     Optional ByVal Id_Mat_O As Integer? = Nothing,
                                     Optional ByVal Id_Fre As Integer? = Nothing,
                                     Optional ByVal Veg_Cod_Prec As Integer? = Nothing,
                                     Optional ByVal Analisi_Testata_Cod As Integer? = Nothing,
                                     Optional ByVal Ubicazione_Cod As Integer? = Nothing,
                                     Optional ByVal N_Distribuito As Decimal? = Nothing,
                                     Optional ByVal TipoAcqua_Cod As Integer? = Nothing,
                                     Optional ByVal Data_Modifica As DateTime = #2/1/1900#,
                                     Optional ByVal Username_Modifica As String = ""
                                     ) As Boolean

        'ByVal Old_PROV As String,
        'ByVal Old_COM As String,
        'ByVal Old_SEZIONE As String,
        'ByVal Old_FOGLIO As Integer,
        'ByVal Old_NUMERO As Integer,
        'ByVal Old_SUBALTERNO As String,
        'ByVal Old_Cul_Cod_Agea As String,
        'ByVal Old_Uso_Cod As String,
        'ByVal Old_Macrouso_Cod As String,
        'ByVal Old_Occupazione_Cod As String,
        'ByVal Old_Destinazione_Cod As String,
        'ByVal Old_Qualita_Cod As String,

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.ParticelleCatastalixVincoliAgronomici_W.ModificaPuntuale()"

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
                Data_Modifica = Date.Now
            End If

            If Username_Modifica = "" Then
                Username_Modifica = objParametri.UsernameOperazione
            End If

            If ID = 0 Then
                Throw New Exception("Parametro non corretto nella query (ID obbligatorio)")
            End If

            '---------------------------------------------
            strSql.Length = 0

            strSql.AppendLine(" UPDATE ParticelleCatastalixVincoliAgronomici ")
            strSql.AppendLine(" SET Data_Modifica = " & Agro_SQL_SaveDateTime(Data_Modifica) & " ")
            strSql.AppendLine("   , Username_Modifica = '" & Agro_SQL_SaveText(Username_Modifica) & "' ")


            If Not IsNothing(Id_Mat_O) Then
                strSql.AppendLine("   , Id_Mat_O = " & Agro_SQL_SaveNum(Id_Mat_O) & " ")
            End If

            If Not IsNothing(Id_Fre) Then
                strSql.AppendLine("   , Id_Fre = " & Agro_SQL_SaveNum(Id_Fre) & " ")
            End If

            If Not IsNothing(Veg_Cod_Prec) Then
                strSql.AppendLine("   , Veg_Cod_Prec = " & Agro_SQL_SaveNum(Veg_Cod_Prec) & " ")
            End If

            If Not IsNothing(Analisi_Testata_Cod) Then
                strSql.AppendLine("   , Analisi_Testata_Cod = " & Agro_SQL_SaveNum(Analisi_Testata_Cod) & " ")
            End If

            If Not IsNothing(Ubicazione_Cod) Then
                strSql.AppendLine("   , Ubicazione_Cod = " & Agro_SQL_SaveNum(Ubicazione_Cod) & " ")
            End If

            If Not IsNothing(N_Distribuito) Then
                strSql.AppendLine("   , N_Distribuito = " & Agro_SQL_SaveNum(N_Distribuito) & " ")
            End If

            If Not IsNothing(TipoAcqua_Cod) Then
                strSql.AppendLine("   , TipoAcqua_Cod = " & Agro_SQL_SaveNum(TipoAcqua_Cod) & " ")
            End If


            strSql.AppendLine(" WHERE ID = " & Agro_SQL_SaveNum(ID) & " ")

            'strSql.AppendLine(" AND PROV = '" & Agro_SQL_SaveText(Old_PROV) & "'  ")
            'strSql.AppendLine(" AND COM = '" & Agro_SQL_SaveText(Old_COM) & "'  ")
            'strSql.AppendLine(" AND SEZIONE ='" & Agro_SQL_SaveText(Old_SEZIONE) & "'  ")
            'strSql.AppendLine(" AND FOGLIO =" & Agro_SQL_SaveNum(Old_FOGLIO))
            'strSql.AppendLine(" AND NUMERO =" & Agro_SQL_SaveNum(Old_NUMERO))
            'strSql.AppendLine(" AND Subalterno = '" & Agro_SQL_SaveText(Old_SUBALTERNO) & "' ")

            'strSql.AppendLine(" AND Cul_Cod_Agea = '" & Agro_SQL_SaveText(Old_Cul_Cod_Agea) & "' ")
            'strSql.AppendLine(" AND Uso_Cod = '" & Agro_SQL_SaveText(Old_Uso_Cod) & "' ")
            'strSql.AppendLine(" AND Macrouso_Cod = '" & Agro_SQL_SaveText(Old_Macrouso_Cod) & "' ")
            'strSql.AppendLine(" AND Occupazione_Cod = '" & Agro_SQL_SaveText(Old_Occupazione_Cod) & "' ")
            'strSql.AppendLine(" AND Destinazione_Cod = '" & Agro_SQL_SaveText(Old_Destinazione_Cod) & "' ")
            'strSql.AppendLine(" AND Qualita_Cod = '" & Agro_SQL_SaveText(Old_Qualita_Cod) & "' ")


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

    Public Function Cancella(ByVal id As Integer,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.ParticelleCatastalixVincoliAgronomici_W.Cancella()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try


            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then

                strSql.Length = 0
                strSql.AppendLine(" UPDATE ParticelleCatastalixVincoliAgronomici ")
                strSql.AppendLine(" SET ")
                'strSql.AppendLine("          Validita_Fine = " & Agro_SQL_SaveDate(CDate("01/01/1900")) & " ")
                strSql.AppendLine("         ,Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                strSql.AppendLine("         ,Inviato = -1 ")
                strSql.AppendLine(" WHERE   id =  " & Agro_SQL_SaveNum(id) & "  ")
                strSql.AppendLine(" AND     Inviato >= 0")

            Else

                strSql.Length = 0
                strSql.AppendLine(" DELETE ")
                strSql.AppendLine(" FROM    ParticelleCatastalixVincoliAgronomici ")
                strSql.AppendLine(" WHERE   id       =  " & Agro_SQL_SaveNum(id) & "  ")

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

    Public Function Cancella(ByVal PROV As String,
                             ByVal COM As String,
                             ByVal SEZIONE As String,
                             ByVal FOGLIO As Integer,
                             ByVal NUMERO As Integer,
                             ByVal SUBALTERNO As String,
                             ByVal Cul_Cod_Agea As String,
                             ByVal Uso_Cod As String,
                             ByVal Macrouso_Cod As String,
                             ByVal Occupazione_Cod As String,
                             ByVal Destinazione_Cod As String,
                             ByVal Qualita_Cod As String,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.ParticelleCatastalixVincoliAgronomici_W.Cancella()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try


            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then

                strSql.Length = 0
                strSql.AppendLine(" UPDATE ParticelleCatastalixVincoliAgronomici ")
                strSql.AppendLine(" SET ")
                'strSql.AppendLine("          Validita_Fine = " & Agro_SQL_SaveDate(CDate("01/01/1900")) & " ")
                strSql.AppendLine("         ,Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                strSql.AppendLine("         ,Inviato = -1 ")
                strSql.AppendLine(" WHERE   Inviato >= 0")

            Else

                strSql.Length = 0
                strSql.AppendLine(" DELETE ")
                strSql.AppendLine(" FROM    ParticelleCatastalixVincoliAgronomici ")
                strSql.AppendLine(" WHERE   1=1")

            End If

            ' La clausola è la stessa per entrambe le query
            strSql.AppendLine(" AND      PROV             = '" & Agro_SQL_SaveText(Trim(PROV)) & "' ")
            strSql.AppendLine(" AND      COM              = '" & Agro_SQL_SaveText(Trim(COM)) & "' ")
            strSql.AppendLine(" AND      Sezione          = '" & IIf(Agro_SQL_SaveText(SEZIONE, False) <> "", Agro_SQL_SaveText(LCase(SEZIONE)), 0) & "'  ")
            strSql.AppendLine(" AND      FOGLIO           =  " & IIf(Agro_SQL_SaveNum(FOGLIO, False) <> 0, Agro_SQL_SaveNum(FOGLIO), 0) & "  ")
            strSql.AppendLine(" AND      Numero           =  " & IIf(Agro_SQL_SaveNum(NUMERO, False) <> 0, Agro_SQL_SaveNum(NUMERO), 0) & "  ")
            strSql.AppendLine(" AND      SUBALTERNO       = '" & IIf(Agro_SQL_SaveText(SUBALTERNO, False) <> "", Agro_SQL_SaveText(LCase(SUBALTERNO)), 0) & "' ")

            strSql.AppendLine(" AND      Cul_Cod_Agea     = '" & Agro_SQL_SaveText(Cul_Cod_Agea) & "'  ")
            strSql.AppendLine(" AND      Uso_Cod          = '" & Agro_SQL_SaveText(Uso_Cod) & "'  ")
            strSql.AppendLine(" AND      Macrouso_Cod     = '" & Agro_SQL_SaveText(Macrouso_Cod) & "'  ")
            strSql.AppendLine(" AND      Occupazione_Cod  = '" & Agro_SQL_SaveText(Occupazione_Cod) & "'  ")
            strSql.AppendLine(" AND      Destinazione_Cod = '" & Agro_SQL_SaveText(Destinazione_Cod) & "'  ")
            strSql.AppendLine(" AND      Qualita_Cod      = '" & Agro_SQL_SaveText(Qualita_Cod) & "'  ")

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
