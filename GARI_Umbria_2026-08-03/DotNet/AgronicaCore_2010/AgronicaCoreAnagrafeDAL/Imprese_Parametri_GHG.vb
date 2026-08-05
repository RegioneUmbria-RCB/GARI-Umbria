Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate



Public Class Imprese_Parametri_GHG_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi(
                         ByVal piva As String,
                         ByVal vegCod As Integer,
                         ByVal culCod As Integer,
                         ByVal regolamentoCod As Integer,
                         ByVal dataRaccolta As Date,
                         ByVal xFiltroAggiuntivo As String,
                         ByVal xOrderBy As String,
                         ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
    ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Imprese_Parametri_GHG_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            StrSQL.Length = 0
            StrSQL.Append(" SELECT * ")
            StrSQL.Append(" FROM Imprese_Parametri_GHG ")
            StrSQL.Append(" WHERE 1 = 1 ")

            If CDate(dataRaccolta) <> AGRODATAINIZIO Then
                StrSQL.Append($" AND Validita_inizio <= {Agro_SQL_SaveDate(dataRaccolta)} ")
                StrSQL.Append($" AND Validita_Fine >= {Agro_SQL_SaveDate(dataRaccolta)} ")
            End If

            If piva <> "" Then
                StrSQL.Append($" AND PIVA = '{Agro_SQL_SaveText(piva)}' ")
            End If
            If vegCod <> 0 Then
                StrSQL.Append($" AND (Veg_Cod =  {Agro_SQL_SaveNum(vegCod)} OR Veg_Cod = 0) ")
            End If
            If culCod <> 0 Then
                StrSQL.Append($" AND (Cul_Cod = {Agro_SQL_SaveNum(culCod)} OR Cul_Cod = 0) ")
            End If
            If regolamentoCod <> 0 Then
                StrSQL.Append($" AND (Regolamento_Cod = {Agro_SQL_SaveNum(regolamentoCod)} OR Regolamento_Cod = 0) ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append($" AND {xFiltroAggiuntivo}")
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND     Inviato >= 0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND     Inviato = -1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append($" ORDER BY {xOrderBy}")
            Else
                ' Ordine discedente per avere i campi con 0 in fondo
                StrSQL.Append(" ORDER BY Regolamento_Cod DESC, Cul_Cod DESC, Veg_Cod DESC ")
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception($"[{NomeRoutine}] : {MessaggioErrore}")
        End Try

        Return DT

    End Function

    Public Function LeggiParametri(
                         ByVal piva As String,
                         ByVal specie As List(Of Integer),
                         ByVal culCod As Integer,
                         ByVal regolamentoCod As Integer,
                         ByVal dataValiditaInizio As Date,
                         ByVal dataValiditaFine As Date,
                         ByVal xFiltroAggiuntivo As String,
                         ByVal xOrderBy As String,
                         ByVal filtraxValidita As Boolean,
                         ByRef objParametriUtente As AgronicaCoreDataProvider.AgronicaCoreParametri,
                         ByRef objParametriServer As AgronicaCoreDataProvider.AgronicaCoreParametri
    ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Imprese_Parametri_GHG_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            StrSQL.Length = 0
            StrSQL.AppendLine(" SELECT ip.*, i.Rag_Soc, s.Veg_Des, c.Cul_Des, r.Reg_Des")
            StrSQL.AppendLine(" FROM Imprese_Parametri_GHG ip")
            StrSQL.AppendLine(" LEFT JOIN Imprese i ON i.PIVA = ip.PIVA")
            StrSQL.AppendLine(" LEFT JOIN SpecieVegetali s ON s.Veg_Cod = ip.Veg_Cod")
            StrSQL.AppendLine(" LEFT JOIN Cultivar c ON c.Cul_Cod = ip.Cul_Cod")
            StrSQL.AppendLine(" LEFT JOIN Regolamenti r ON r.Reg_Cod = ip.Regolamento_Cod")

            Dim filtraVisibilita = filtraxValidita And LeggiSeFiltroVisibilitaUtentePresente(objParametriUtente)

            If (filtraVisibilita) Then
                StrSQL.AppendLine(" LEFT JOIN Utenti_Visibilita_Appoggio uva (NOLOCK) On ip.Piva = uva.Piva AND uva.Entita_Cod=1 AND uva.Username = " & Agro_SQL_SaveText_NULL(objParametriServer.UtenteUsername) & " ")
            End If


            StrSQL.AppendLine($" WHERE ip.Validita_inizio <= {Agro_SQL_SaveDate(dataValiditaFine)} ")
            StrSQL.AppendLine($" AND   ip.Validita_Fine >= {Agro_SQL_SaveDate(dataValiditaInizio)} ")


            If piva <> "" Then
                StrSQL.AppendLine($" AND ip.PIVA = '{Agro_SQL_SaveText(piva)}' ")
            End If
            If specie.Count <> 0 Then
                StrSQL.AppendLine("   AND  ip.Veg_Cod IN ( ")
                Dim listaStringa As String = Join(specie.Select(Function(x) x.ToString()).ToArray(), ", ").ToString()
                StrSQL.Append(Agro_SQL_Save_Clausola_IN(listaStringa))
                StrSQL.Append(" )")
            End If
            If culCod <> 0 Then
                StrSQL.AppendLine($" AND (ip.Cul_Cod = {Agro_SQL_SaveNum(culCod)} ) ")
            End If
            If regolamentoCod <> 0 Then
                StrSQL.AppendLine($" AND (ip.Regolamento_Cod = {Agro_SQL_SaveNum(regolamentoCod)} ) ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine($" AND {xFiltroAggiuntivo}")
            End If

            If (filtraVisibilita) Then
                StrSQL.AppendLine($" AND uva.Username = {Agro_SQL_SaveText_NULL(objParametriServer.UtenteUsername)} ")
            End If

            '--------------------------------------------------------------------------
            Select Case objParametriServer.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.AppendLine(" AND     ip.Inviato >= 0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.AppendLine(" AND     ip.Inviato = -1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.AppendLine($" ORDER BY {xOrderBy}")
            Else
                ' Ordine discedente per avere i campi con 0 in fondo
                StrSQL.AppendLine(" ORDER BY ip.Regolamento_Cod DESC, ip.Cul_Cod DESC, ip.Veg_Cod DESC ")
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametriServer, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametriServer, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception($"[{NomeRoutine}] : {MessaggioErrore}")
        End Try

        Return DT

    End Function

    Private Function LeggiSeFiltroVisibilitaUtentePresente(objParametri_Utenti As AgronicaCoreParametri) As Boolean
        Dim objProfilo As New AgronicaCoreUtentiDAL.Utenti_Profili_Read
        Return Not objProfilo.HasFullVisibility(objParametri_Utenti.UtenteUsername, objParametri_Utenti)
    End Function

End Class

Public Class Imprese_Parametri_GHG_W
    Inherits AgronicaCoreDataProvider.DataProvider
    Public Function Scrivi(
                         ByVal Piva As String,
                         ByVal Veg_Cod As Integer,
                         ByVal Cul_Cod As Integer,
                         ByVal Regolamento_Cod As Integer,
                         ByVal EEC As Decimal,
                         ByVal Validita_Inizio As Date,
                         ByVal Validita_Fine As Date,
                         ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                         Optional ByVal Data_creazione As DateTime = Nothing,
                         Optional ByVal Data_modifica As DateTime = Nothing,
                         Optional ByVal username_creazione As String = "",
                         Optional ByVal username_modifica As String = ""
    ) As Boolean


        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Imprese_Parametri_GHG_W.Scrivi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean

        If (Data_creazione = Nothing) Then
            Data_creazione = Now
        End If
        If (Data_modifica = Nothing) Then
            Data_modifica = Now
        End If
        If (username_creazione = "") Then
            username_creazione = objParametri.UtenteUsername
        End If
        If (username_modifica = "") Then
            username_modifica = objParametri.UtenteUsername
        End If

        If (Veg_Cod = Nothing) Then
            Veg_Cod = 0
        End If

        If (Cul_Cod = Nothing) Then
            Cul_Cod = 0
        End If

        If (Regolamento_Cod = Nothing) Then
            Regolamento_Cod = 0
        End If

        Try
            StrSQL.Length = 0
            StrSQL.AppendLine("INSERT INTO Imprese_Parametri_GHG(      ")
            StrSQL.AppendLine("                    Piva,               ")
            StrSQL.AppendLine("                    Veg_Cod,            ")
            StrSQL.AppendLine("                    Cul_Cod,            ")
            StrSQL.AppendLine("                    Regolamento_Cod,    ")
            StrSQL.AppendLine("                    EEC,                ")
            StrSQL.AppendLine("                    Validita_Inizio,    ")
            StrSQL.AppendLine("                    Validita_Fine,      ")
            StrSQL.AppendLine("                    inviato,     datainvio,     Data_Creazione,     Data_Modifica,")
            StrSQL.AppendLine("                    Username_Creazione,      Username_Modifica ")
            StrSQL.AppendLine("                    ) ")
            StrSQL.AppendLine("VALUES (")
            StrSQL.AppendLine("          '" & Agro_SQL_SaveText(Piva) & "' ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Veg_Cod) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Cul_Cod) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Regolamento_Cod) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(EEC) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")
            StrSQL.AppendLine("         , 0  ")
            StrSQL.AppendLine("         , NULL ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDate(Data_creazione) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDate(Data_modifica) & "  ")
            StrSQL.AppendLine("         , '" & Agro_SQL_SaveText(username_creazione) & "'  ")
            StrSQL.AppendLine("         , '" & Agro_SQL_SaveText(username_modifica) & "'  ")
            StrSQL.AppendLine(")")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception($"[{NomeRoutine}] : {MessaggioErrore}")
        End Try

        Return xRisp

    End Function

    Public Function Modifica(
                         ByVal ID As Integer,
                         ByVal Piva As String,
                         ByVal Veg_Cod As Integer,
                         ByVal Cul_Cod As Integer,
                         ByVal Regolamento_Cod As Integer,
                         ByVal EEC As Decimal,
                         ByVal Validita_Inizio As Date,
                         ByVal Validita_Fine As Date,
                         ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                         Optional ByVal Data_modifica As DateTime = Nothing,
                         Optional ByVal username_modifica As String = ""
    ) As Boolean


        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Imprese_Parametri_GHG_W.Modifica()"

        If (Data_modifica = Nothing) Then
            Data_modifica = Now
        End If
        If (username_modifica = "") Then
            username_modifica = objParametri.UtenteUsername
        End If

        If (Veg_Cod = Nothing) Then
            Veg_Cod = 0
        End If

        If (Cul_Cod = Nothing) Then
            Cul_Cod = 0
        End If

        If (Regolamento_Cod = Nothing) Then
            Regolamento_Cod = 0
        End If

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean

        Try
            StrSQL.Length = 0
            StrSQL.AppendLine("UPDATE Imprese_Parametri_GHG SET        ")
            StrSQL.AppendLine("                    Piva = '" & Agro_SQL_SaveText(Piva) & "',                             ")
            StrSQL.AppendLine("                    Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod) & ",                        ")
            StrSQL.AppendLine("                    Cul_Cod = " & Agro_SQL_SaveNum(Cul_Cod) & ",                        ")
            StrSQL.AppendLine("                    Regolamento_Cod = " & Agro_SQL_SaveNum(Regolamento_Cod) & ",        ")
            StrSQL.AppendLine("                    EEC = " & Agro_SQL_SaveNum(EEC) & ",                                ")
            StrSQL.AppendLine("                    Validita_Inizio = " & Agro_SQL_SaveDate(Validita_Inizio) & ",       ")
            StrSQL.AppendLine("                    Validita_Fine = " & Agro_SQL_SaveDate(Validita_Fine) & ",           ")
            StrSQL.AppendLine("                    inviato = 0,                                                        ")
            StrSQL.AppendLine("                    datainvio = NULL,                                                   ")
            StrSQL.AppendLine("                    Data_modifica = " & Agro_SQL_SaveDate(Data_modifica) & ",           ")
            StrSQL.AppendLine("                    username_modifica = '" & Agro_SQL_SaveText(username_modifica) & "'    ")

            StrSQL.AppendLine(" WHERE  ID = " & Agro_SQL_SaveNum(ID) & " ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception($"[{NomeRoutine}] : {MessaggioErrore}")
        End Try

        Return xRisp

    End Function

    Public Function Cancella(
                         ByVal ID As Integer,
                         ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
    ) As Boolean


        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Imprese_Parametri_GHG_W.Cancella()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean

        Try
            StrSQL.Length = 0
            StrSQL.AppendLine(" DELETE ")
            StrSQL.AppendLine(" FROM     Imprese_Parametri_GHG ")
            StrSQL.AppendLine(" WHERE  ID = " & Agro_SQL_SaveNum(ID) & " ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception($"[{NomeRoutine}] : {MessaggioErrore}")
        End Try

        Return xRisp

    End Function
End Class