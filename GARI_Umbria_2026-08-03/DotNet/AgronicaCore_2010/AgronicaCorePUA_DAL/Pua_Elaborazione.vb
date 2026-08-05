Imports System.Text
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate

Public Class Pua_Elaborazione_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Scrivi(ByRef PUA_Cod As Integer, ByVal Regolamento_cod As Integer, ByVal Pua_Tipo As Integer,
                           ByVal Id_Anagrafe_Vincoli As Integer,
                           ByVal Cuaa As String, ByVal Piva As String,
                              ByVal Sa_Cod As Integer, ByVal Campo_Cod As Integer, ByVal Appezza As Integer, ByVal Id_Reg As Integer, ByVal Progetto_Cod As Integer,
                                ByVal Veg_Cod As Integer, ByVal Cul_Cod As Integer, ByVal Id_Cod As Integer, ByVal Grfi_Cod As Integer, ByVal Grfi_Cod_Concimazione As Integer,
                                    ByVal Stato_Impianto As Integer, ByVal Ciclo_Cod As Integer,
                           ByVal Resa As Decimal, ByVal Resa_Rif As Decimal, ByVal FattoreCorrettivo_N As Decimal,
                                            ByVal Sup As Decimal, ByVal Zvn As Integer,
                                        ByVal N_Fabbisogno As Decimal, ByVal N_Fabbisogno_Organico As Decimal, ByVal N_Mas As Decimal,
                                           ByVal N_Utile_Soddisfatto As Decimal, ByVal N_Totale_Soddisfatto As Decimal,
                                            ByVal N_Zootecnico As Decimal, ByVal N_Zootecnico_Letame As Decimal, ByVal N_Zootecnico_Liquame As Decimal,
                                                 ByVal Bilancio_N_Utile As Decimal, ByVal Bilancio_N_Totale As Decimal, ByVal Indice_Efficienza_Azotata As Decimal,
                                                ByVal Valutazione_N_Utile As Integer, ByVal Valutazione_N_Totale As Integer, ByVal Valutazione_Efficienza_Azotata As Integer, ByVal conforme As String,
                                                        ByVal Validita_Inizio As DateTime, ByVal Validita_Fine As DateTime,
                                                ByRef objParametri As AgronicaCoreParametri,
                                                       Optional ByVal Data_creazione As DateTime = #2/1/1900#,
                                                       Optional ByVal Data_modifica As DateTime = #2/1/1900#,
                                                       Optional ByVal username_creazione As String = "",
                                                       Optional ByVal username_modifica As String = ""
                                           ) As Boolean




        Const nomeRoutine = "Pua_Elaborazione_W.Scrivi()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try

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
            strSql.AppendLine(" INSERT INTO PUA_Elaborazione ")
            strSql.AppendLine("             (")
            strSql.AppendLine("  [Piva_SuperUser] ")
            strSql.AppendLine("  ,[PUA_Cod] ")
            strSql.AppendLine("  ,[Regolamento_cod] ")
            strSql.AppendLine("  ,[PUA_Tipo] ")
            strSql.AppendLine("  ,[Id_Anagrafe_Vincoli] ")
            strSql.AppendLine("  ,[cuaa] ")
            strSql.AppendLine("  ,[Piva] ")
            strSql.AppendLine("  ,[Sa_Cod] ")
            strSql.AppendLine("  ,[Campo_Cod] ")
            strSql.AppendLine("  ,[Appezza] ")
            strSql.AppendLine("  ,[Id_Reg] ")
            strSql.AppendLine("  ,[Progetto_Cod] ")
            strSql.AppendLine("  ,[Veg_Cod] ")
            strSql.AppendLine("  ,[Cul_Cod] ")
            strSql.AppendLine("  ,[Id_Cod] ")
            strSql.AppendLine("  ,[Grfi_Cod] ")
            strSql.AppendLine("  ,[Grfi_Cod_Concimazione] ")
            strSql.AppendLine("  ,[Stato_Impianto] ")
            strSql.AppendLine("  ,[Ciclo_Cod] ")
            strSql.AppendLine("  ,[Resa] ")
            strSql.AppendLine("  ,[Resa_Rif] ")
            strSql.AppendLine("  ,[FattoreCorrettivo_N] ")
            strSql.AppendLine("  ,[Sup] ")
            strSql.AppendLine("  ,[Zvn] ")
            strSql.AppendLine("  ,[N_Fabbisogno]  ")
            strSql.AppendLine("  ,[N_Fabbisogno_Organico]  ")
            strSql.AppendLine("  ,[N_Mas] ")
            strSql.AppendLine("  ,[N_Utile_Soddisfatto]  ")
            strSql.AppendLine("  ,[N_Totale_Soddisfatto]  ")
            strSql.AppendLine("  ,[N_Zootecnico]  ")
            strSql.AppendLine("  ,[N_Zootecnico_Letame]  ")
            strSql.AppendLine("  ,[N_Zootecnico_Liquame]  ")
            strSql.AppendLine("  ,[Bilancio_N_Utile]  ")
            strSql.AppendLine("  ,[Bilancio_N_Totale]  ")
            strSql.AppendLine("  ,[Indice_Efficienza_Azotata]  ")
            strSql.AppendLine("  ,[Valutazione_N_Utile]  ")
            strSql.AppendLine("  ,[Valutazione_N_Totale]  ")
            strSql.AppendLine("  ,[Valutazione_Efficienza_Azotata]  ")
            strSql.AppendLine("  ,[Conforme]  ")
            strSql.AppendLine("  ,[Validita_Inizio]  ")
            strSql.AppendLine("  ,[Validita_Fine]  ")
            strSql.AppendLine("  ,[inviato] ")
            strSql.AppendLine("  ,[Data_creazione] ")
            strSql.AppendLine("  ,[Data_Modifica] ")
            strSql.AppendLine("  ,[UserName_Creazione] ")
            strSql.AppendLine("  ,[UserName_Modifica] ")
            strSql.AppendLine("                ) ")

            strSql.AppendLine(" VALUES ( ")
            strSql.AppendLine("          '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(PUA_Cod) & " ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Regolamento_cod) & " ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Pua_Tipo) & " ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Id_Anagrafe_Vincoli) & " ")
            strSql.AppendLine("         , '" & Agro_SQL_SaveText(Cuaa) & "'")
            strSql.AppendLine("         , '" & Agro_SQL_SaveText(Piva) & "' ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Campo_Cod) & " ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Appezza) & " ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Id_Reg) & " ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Progetto_Cod) & " ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Veg_Cod) & " ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Cul_Cod) & " ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Id_Cod) & " ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Grfi_Cod) & " ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Grfi_Cod_Concimazione) & " ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Stato_Impianto) & " ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Ciclo_Cod) & " ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Resa) & " ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Resa_Rif) & " ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(FattoreCorrettivo_N) & " ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Sup) & " ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Zvn) & " ")

            strSql.AppendLine("         , " & Agro_SQL_SaveNum(N_Fabbisogno) & " ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(N_Fabbisogno_Organico) & " ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(N_Mas) & " ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(N_Utile_Soddisfatto) & " ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(N_Totale_Soddisfatto) & " ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(N_Zootecnico) & " ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(N_Zootecnico_Letame) & " ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(N_Zootecnico_Liquame) & " ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Bilancio_N_Utile) & " ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Bilancio_N_Totale) & " ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Indice_Efficienza_Azotata) & " ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Valutazione_N_Utile) & " ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Valutazione_N_Totale) & " ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Valutazione_Efficienza_Azotata) & " ")
            strSql.AppendLine("			,'" & Agro_SQL_SaveText(conforme) & "' ")
            strSql.AppendLine("			, " & Agro_SQL_SaveDateTime(Validita_Inizio) & "  ")
            strSql.AppendLine("			, " & Agro_SQL_SaveDateTime(Validita_Fine) & "  ")

            strSql.AppendLine("         , 0  ")
            strSql.AppendLine("			, " & Agro_SQL_SaveDateTime(Data_creazione) & "  ")
            strSql.AppendLine("			, " & Agro_SQL_SaveDateTime(Data_modifica) & "  ")
            strSql.AppendLine("			,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            strSql.AppendLine("			,'" & Agro_SQL_SaveText(username_modifica) & "' ")

            strSql.AppendLine(") ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            'Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function

    Public Function Scrivi_old(ByRef PUA_Cod As Integer, ByVal Regolamento_cod As Integer,
                           ByVal Piva As String, ByVal Cuaa As String,
                              ByVal rag_soc As String,
                                 ByVal app As String, ByVal sup_appezzamento As Decimal, ByVal specie As String,
                                    ByVal provincia As String, ByVal comune As String,
                                        ByVal istat_provincia As String, ByVal istat_comune As String,
                                           ByVal sezione As String, ByVal foglio As Integer, ByVal numero As Integer, ByVal subalterno As String,
                                              ByVal sup_particella As Decimal,
                                                ByVal zvn_si As String, ByVal zvn_no As String,
                                                    ByVal conforme_si As String, ByVal conforme_no As String,
                                                        ByVal compilatore As String,
                                                ByRef objParametri As AgronicaCoreParametri,
                                                       Optional ByVal Data_creazione As DateTime = #2/1/1900#,
                                                       Optional ByVal Data_modifica As DateTime = #2/1/1900#,
                                                       Optional ByVal username_creazione As String = "",
                                                       Optional ByVal username_modifica As String = ""
                                           ) As Boolean

        Const nomeRoutine = "Pua_Elaborazione_W.Scrivi()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try

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
            strSql.AppendLine(" INSERT INTO PUA_Elaborazione ")
            strSql.AppendLine("             (")

            strSql.AppendLine("  [cuaa] ")
            strSql.AppendLine("  ,[Piva] ")
            strSql.AppendLine("  ,[PUA_Cod] ")
            strSql.AppendLine("  ,[Regolamento_cod] ")
            strSql.AppendLine("  ,[ragione_sociale] ")
            strSql.AppendLine("  ,[app] ")
            strSql.AppendLine("  ,[sup_appezzamento] ")
            strSql.AppendLine("  ,[specie] ")
            strSql.AppendLine("  ,[provincia] ")
            strSql.AppendLine("  ,[comune] ")
            strSql.AppendLine("  ,[istat_provincia] ")
            strSql.AppendLine("  ,[istat_comune] ")
            strSql.AppendLine("  ,[sezione] ")
            strSql.AppendLine("  ,[foglio] ")
            strSql.AppendLine("  ,[numero] ")
            strSql.AppendLine("  ,[subalterno] ")
            strSql.AppendLine("  ,[sup_particella] ")
            strSql.AppendLine("  ,[zvn_si] ")
            strSql.AppendLine("  ,[zvn_no] ")
            strSql.AppendLine("  ,[conforme_si] ")
            strSql.AppendLine("  ,[conforme_no] ")
            strSql.AppendLine("  ,[compilatore], ")

            strSql.AppendLine("              Inviato,  ")
            strSql.AppendLine("              Data_Creazione,     Data_Modifica, ")
            strSql.AppendLine("              UserName_Creazione, UserName_Modifica ")

            strSql.AppendLine("                ) ")

            strSql.AppendLine(" VALUES ( ")

            strSql.AppendLine("          '" & Agro_SQL_SaveText(Cuaa) & "'")
            strSql.AppendLine("         , '" & Agro_SQL_SaveText(Piva) & "' ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(PUA_Cod) & " ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Regolamento_cod) & " ")
            strSql.AppendLine("         , '" & Agro_SQL_SaveText(rag_soc) & "' ")
            strSql.AppendLine("         , '" & Agro_SQL_SaveText(app) & "' ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(sup_appezzamento) & " ")
            strSql.AppendLine("         , '" & Agro_SQL_SaveText(specie) & "' ")

            strSql.AppendLine("         , '" & Agro_SQL_SaveText(provincia) & "' ")
            strSql.AppendLine("         , '" & Agro_SQL_SaveText(comune) & "' ")
            strSql.AppendLine("         , '" & Agro_SQL_SaveText(istat_provincia) & "' ")
            strSql.AppendLine("         , '" & Agro_SQL_SaveText(istat_comune) & "' ")
            strSql.AppendLine("         , '" & Agro_SQL_SaveText(sezione) & "' ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(foglio) & " ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(numero) & " ")
            strSql.AppendLine("         , '" & Agro_SQL_SaveText(subalterno) & "' ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(sup_particella) & " ")

            strSql.AppendLine("			,'" & Agro_SQL_SaveText(zvn_si) & "' ")
            strSql.AppendLine("			,'" & Agro_SQL_SaveText(zvn_no) & "' ")
            strSql.AppendLine("			,'" & Agro_SQL_SaveText(conforme_si) & "' ")
            strSql.AppendLine("			,'" & Agro_SQL_SaveText(conforme_no) & "' ")
            strSql.AppendLine("			,'" & Agro_SQL_SaveText(compilatore) & "' ")

            strSql.AppendLine("         , 0  ")

            strSql.AppendLine("			, " & Agro_SQL_SaveDateTime(Data_creazione) & "  ")
            strSql.AppendLine("			, " & Agro_SQL_SaveDateTime(Data_modifica) & "  ")
            strSql.AppendLine("			,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            strSql.AppendLine("			,'" & Agro_SQL_SaveText(username_modifica) & "' ")

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

    Public Function Cancella(ByVal PUA_Cod As Integer,
                             ByVal Regolamento_Cod As Integer,
                             ByVal Piva As String, ByVal Sa_Cod As Integer,
                             ByRef objParametri As AgronicaCoreParametri
                             ) As Boolean

        Const nomeRoutine = "Pua_Elaborazione_W.Cancella()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then
                strSql.Length = 0
                strSql.AppendLine(" UPDATE PUA_Elaborazione ")
                strSql.AppendLine(" SET ")
                strSql.AppendLine("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                strSql.AppendLine("      ,Inviato = -1 ")
                strSql.AppendLine(" WHERE  Inviato >= 0")
                strSql.AppendLine(" AND   Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            Else

                strSql.Length = 0
                strSql.AppendLine(" DELETE ")
                strSql.AppendLine(" FROM     PUA_Elaborazione ")
                strSql.AppendLine(" WHERE   Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            End If

            If PUA_Cod <> 0 Then
                strSql.AppendLine("  AND   PUA_Cod = " & Agro_SQL_SaveNum(PUA_Cod) & " ")
            End If
            If Regolamento_Cod <> 0 Then
                strSql.AppendLine("  AND   Regolamento_Cod = " & Agro_SQL_SaveNum(Regolamento_Cod) & " ")
            End If

            If Piva <> "" Then
                strSql.AppendLine("  AND  Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If
            If Sa_Cod <> 0 Then
                strSql.AppendLine("  AND   Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
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


Public Class Pua_Elaborazione_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi(ByVal PUA_Cod As Integer, ByVal Regolamento_Cod As Integer,
                            ByVal piva As String, ByVal Id_Anagrafe_Vincoli As Integer,
                                ByVal xFiltroAggiuntivo As String, ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreParametri
                                ) As DataTable

        Const nomeRoutine = "Pua_Elaborazione_R.Leggi()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0

            strSql.AppendLine(" SELECT * ")
            strSql.AppendLine(" FROM Pua_Elaborazione ")
            strSql.AppendLine(" WHERE  Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

            If Regolamento_Cod <> 0 Then
                strSql.AppendLine(" AND Regolamento_Cod=" & Agro_SQL_SaveNum(Regolamento_Cod) & " ")
            End If

            If PUA_Cod <> 0 Then
                strSql.AppendLine(" AND PUA_Cod=" & Agro_SQL_SaveNum(PUA_Cod) & " ")
            End If

            If piva <> "" Then
                strSql.AppendLine(" AND  Piva = '" & Agro_SQL_SaveText(piva) & "' ")
            End If

            If Id_Anagrafe_Vincoli <> 0 Then
                strSql.AppendLine(" AND Id_Anagrafe_Vincoli=" & Agro_SQL_SaveNum(Id_Anagrafe_Vincoli) & " ")
            End If

            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.Append(" AND   Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.Append(" AND   Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                strSql.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
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

    Public Function Leggi_Da_GerarchiaImpresa(ByVal ImpresePadri As List(Of String),
                                                          ByVal Piva As String,
                                                          ByVal validita_Inizio As DateTime,
                                                          ByVal validita_fine As DateTime,
                                                                ByVal xFiltroAggiuntivo As String,
                                                             ByVal xOrderBy As String,
                                                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                             ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                             ByRef filtroUltima As Boolean) As DataTable

        Dim NomeRoutine As String = "AgronicaPUADAL.Pua_Elaborazione_R.Leggi_Da_GerarchiaImpresa()"

        Dim NomeDB_Utenti As String = objParametri_Utenti.StringaConnessione.Split(";")(2).Split("=")(1)


        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            ''modificare la query di select
            StrSQL.Length = 0
            StrSQL.AppendLine("SELECT val_cod as cuaa, i.rag_soc,i.Piva, pt.Pua_Cod,pt.regolamento_cod, pt.PUA_Tipo, pt.Validita_inizio, pt.Validita_Fine, pt.Data_Creazione, " & vbCrLf)
            StrSQL.AppendLine(" ind.stato, ind.pro_cod, ind.com_Des, ind.ind_Des " & vbCrLf)
            StrSQL.AppendLine(" , ISNULL((Utenti_Dettagli.Cognome + ' ' + Utenti_Dettagli.Nome), '') AS Compilatore ")
            StrSQL.AppendLine(" , pe.* ")
            StrSQL.AppendLine("  ,a.app_nome, r.sup_imp, isnull(s.veg_des,'') as veg_des ")
            StrSQL.AppendLine("  ,isnull(ap.prov,'') as prov, isnull(ap.com,'') as com ")
            StrSQL.AppendLine("  ,isnull(istat.comuni_prov,'') as provincia, isnull(istat.localita,'') as comune ")
            StrSQL.AppendLine("  ,isnull(ap.sezione,'') as sezione, isnull(ap.foglio,0) as foglio, isnull(ap.numero,0) as numero, isnull(ap.subalterno,'') as subalterno ")

            'If Leggi_SupCondotta = True Then
            StrSQL.AppendLine(" , ISNULL((SELECT TOP 1 Sup_Condotta ")
            StrSQL.AppendLine("         FROM   ImpreseXParticelle ")
                StrSQL.AppendLine("         where ap.PIVA = ImpreseXParticelle.PIVA AND ap.SA_COD = ImpreseXParticelle.SA_COD ")
                StrSQL.AppendLine("         AND ImpreseXParticelle.PROV=ap.prov And ImpreseXParticelle.com=ap.com ")
                StrSQL.AppendLine("         AND ImpreseXParticelle.sezione=ap.sezione And ImpreseXParticelle.foglio=ap.foglio  ")
                StrSQL.AppendLine("         AND ImpreseXParticelle.numero=ap.numero And ImpreseXParticelle.subalterno=ap.subalterno  ")
                StrSQL.AppendLine("         AND sup_condotta <> 0  ")
                StrSQL.AppendLine("         order by validita_fine desc ")
            StrSQL.AppendLine("         ), 0) AS Sup_Condotta ")
            'End If

            StrSQL.AppendLine(" FROM PUA_Elaborazione pe  " & vbCrLf)
            StrSQL.AppendLine(" inner join Imprese I on pe.piva=I.PIVA " & vbCrLf)
            StrSQL.AppendLine(" inner join Pua_Testata pt on pe.pua_cod = pt.PUA_Cod and pe.Regolamento_Cod = pt.Regolamento_Cod and pe.piva=pt.piva " & vbCrLf)
            StrSQL.AppendLine(" inner join appezzamento a on a.piva=pe.piva and a.sa_cod=pe.Sa_Cod and a.APPEZZA=pe.appezza " & vbCrLf)
            StrSQL.AppendLine(" inner join reg_impianti r on r.piva=pe.piva and r.sa_cod=pe.Sa_Cod and r.APPEZZA=pe.appezza and r.id_reg=pe.id_reg " & vbCrLf)

            StrSQL.AppendLine(" LEFT JOIN SpecieVegetali s ON pe.veg_cod = s.veg_cod " & vbCrLf)

            StrSQL.AppendLine(" LEFT JOIN GerarchiaImprese gi ON I.PIVA = gi.Figlio  " & vbCrLf)
            If filtroUltima Then
                StrSQL.AppendLine(" AND Pt.pua_Cod IN ( " & vbCrLf)
                StrSQL.AppendLine("    SELECT Max(pua_Cod) FROM Pua_Testata " & vbCrLf)
                StrSQL.AppendLine("    WHERE Piva = pt.piva " & vbCrLf)
                StrSQL.AppendLine("    AND Validita_Inizio >=  " + Agro_SQL_SaveDateTime_NULL(validita_Inizio) + "   " & vbCrLf)
                StrSQL.AppendLine("    AND Validita_Fine <=  " + Agro_SQL_SaveDateTime_NULL(validita_fine) + " " & vbCrLf)
                StrSQL.AppendLine(" )")
            End If
            StrSQL.AppendLine(" LEFT JOIN Imprese_Codici ic ON i.piva = ic.piva AND ic.id_Cod = 1010  " & vbCrLf)
            StrSQL.AppendLine(" LEFT JOIN ImpreseXIndirizzi ii ON i.piva = ii.piva AND Tipo_Indirizzo = 1" & vbCrLf)
            StrSQL.AppendLine(" LEFT JOIN Indirizzi ind ON ind.Cod_Indirizzo = ii.cod_indirizzo " & vbCrLf)
            StrSQL.AppendLine(" LEFT JOIN " & NomeDB_Utenti & ".dbo.Utenti_Dettagli ON pt.username_creazione = Utenti_Dettagli.codfisc ")

            StrSQL.AppendLine(" LEFT JOIN AppezzamentiXParticelle ap ON pe.piva = ap.piva and pe.sa_cod = ap.sa_cod and pe.appezza = ap.appezza " & vbCrLf)
            StrSQL.AppendLine(" LEFT JOIN ISTAT ON ap.prov = istat.prov and ap.com = istat.com " & vbCrLf)

            StrSQL.AppendLine(" WHERE pt.pua_Cod is not null " & vbCrLf)

            If ImpresePadri.Count <> 0 Then
                If ImpresePadri.Count = 1 Then
                    Dim impresa = ImpresePadri(0)
                    If impresa <> "-1" Then
                        StrSQL.AppendLine(" AND gi.Padre = " + Agro_SQL_SaveText_NULL(impresa) + " " & vbCrLf)
                    Else
                        StrSQL.AppendLine(" AND gi.Padre <> '' " & vbCrLf)
                    End If
                Else
                    StrSQL.AppendLine(" AND gi.Padre IN ( ")
                    Dim first = True
                    For Each pPadre In ImpresePadri
                        If pPadre <> "" Then
                            If Not first Then
                                StrSQL.AppendLine(", ")
                            Else
                                first = False
                            End If
                            StrSQL.AppendLine(" " + Agro_SQL_SaveText_NULL(pPadre) + " ")
                        End If
                    Next
                    StrSQL.AppendLine(" ) ")
                End If
            End If

            If Piva <> "" Then
                StrSQL.AppendLine(" AND I.PIVA = " + Agro_SQL_SaveText_NULL(Piva) + " " & vbCrLf)
            End If
            If validita_Inizio <> AGRODATAINIZIO Then
                StrSQL.AppendLine(" AND pt.Validita_Inizio >= " + Agro_SQL_SaveDateTime(validita_Inizio) + " " & vbCrLf)
            End If
            If validita_fine <> AGRODATAFINE Then
                StrSQL.AppendLine(" AND pt.Validita_Fine <= " + Agro_SQL_SaveDateTime(validita_fine) + " ")
            End If

            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
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

End Class