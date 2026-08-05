Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider
Imports System.Transactions
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO
Imports Newtonsoft.Json
Imports AgronicaCoreModelsSTD.exceptions

Public Class Cantina_Vasche_R
    Inherits AgronicaCoreDataProvider.DataProvider


    '##############################################################################################
    Public Function Leggi(ByVal Piva As String,
                            ByVal Sa_Cod As Integer,
                            ByVal Vas_Cod As Integer,
                            ByVal xFiltroAggiuntivo As String,
                            ByVal xOrderBy As String,
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                            ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Cantina_Vasche_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.AppendLine(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;")
            StrSQL.Append(" SELECT * ")
            StrSQL.Append(" FROM  Cantina_Vasche ")
            StrSQL.Append(" WHERE 1=1")

            If Piva <> "" Then
                StrSQL.Append(" AND Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If

            If Sa_Cod <> 0 Then
                StrSQL.Append(" AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            Else
                'leggo se ci sono filtri sui centri
                Dim objUtentiVisibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
                Dim FiltroCentri As String = ""
                Dim DtCentriVisibili As DataTable
                DtCentriVisibili = objUtentiVisibilita.Leggi(enum_TipoEntita.Centro, " Piva='" & Agro_SQL_SaveText(Piva) & "'", "", objParametri)
                If Not DtCentriVisibili Is Nothing AndAlso DtCentriVisibili.Rows.Count > 0 Then
                    For i = 0 To DtCentriVisibili.Rows.Count - 1
                        FiltroCentri &= DtCentriVisibili.Rows(i).Item("sa_cod") & ","
                    Next
                    If FiltroCentri <> "" Then
                        StrSQL.Append(" AND (sa_cod IN (" & Agro_SQL_Save_Clausola_IN(Left(FiltroCentri, FiltroCentri.Length - 1), False) & ") ) ")
                    End If
                End If
            End If

            If Vas_Cod <> 0 Then
                StrSQL.Append(" AND Vas_Cod = " & Agro_SQL_SaveNum(Vas_Cod) & " ")
            End If
            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY Identificativo ")
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
    Public Function LeggiJoinUdm(ByVal Piva As String,
                                ByVal Sa_Cod As Integer,
                                ByVal Vas_Cod As Integer,
                                ByVal Piano_Cod As Integer,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Cantina_Vasche_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT Cantina_Vasche.*, UnitaMisura.udm_sim, UnitaMisura.udm_des ")
            StrSQL.Append(" FROM  Cantina_Vasche ")
            StrSQL.Append(" INNER JOIN UnitaMisura ON Cantina_Vasche.Udm_Cod_Capacita = UnitaMisura.Udm_Cod ")
            StrSQL.Append(" WHERE 1=1")

            If Piva <> "" Then
                StrSQL.Append(" AND Cantina_Vasche.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If

            If Sa_Cod <> 0 Then
                StrSQL.Append(" AND Cantina_Vasche.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            End If

            If Vas_Cod <> 0 Then
                StrSQL.Append(" AND Cantina_Vasche.Vas_Cod = " & Agro_SQL_SaveNum(Vas_Cod) & " ")
            End If

            If Piano_Cod <> 0 Then
                StrSQL.Append(" AND Cantina_Vasche.Piano_Cod = " & Agro_SQL_SaveNum(Piano_Cod) & " ")
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   Cantina_Vasche.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   Cantina_Vasche.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY Cantina_Vasche.Identificativo ")
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
    Public Function LeggiVascaVinoSfusoMovimentato_byIdAgenda(ByVal Piva As String,
                                                            ByVal Id_Agenda As Integer,
                                                            ByVal Cau_Mov As String,
                                                            ByVal Data_Inizio As Date,
                                                            ByVal Data_Fine As Date,
                                                            ByVal xFiltroAggiuntivo As String,
                                                            ByVal xOrderBy As String,
                                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                            ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Cantina_Vasche_R.LeggiVascaVinoSfusoMovimentato_byIdAgenda()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT agenda.id_agenda, Mov_Destinazioni.Tipo_Destinazione, Cantina_Vasche.*, UnitaMisura.udm_sim, UnitaMisura.udm_des ")
            StrSQL.Append(" ")

            StrSQL.Append(" FROM Agenda  ")
            StrSQL.Append(" INNER JOIN Linee_Preparazioni ON Agenda.PIVA = Linee_Preparazioni.Piva AND Agenda.PREPARAZIONE_COD = Linee_Preparazioni.Preparazione_Cod ")
            StrSQL.Append(" INNER JOIN Movimenti ON Agenda.Sa_Cod = Movimenti.Sa_Cod AND Agenda.Id_Agenda = Movimenti.Id_Agenda AND Agenda.PIVA = Movimenti.PIVA  ")
            StrSQL.Append(" INNER JOIN Movimenti_dettagli ON Movimenti.PIVA = Movimenti_dettagli.PIVA AND Movimenti.Id_Agenda = Movimenti_dettagli.Id_Agenda AND Movimenti.Id_Mov = Movimenti_dettagli.Id_Mov  ")
            StrSQL.Append(" INNER JOIN Mov_Destinazioni ON Movimenti_dettagli.PIVA = Mov_Destinazioni.Piva AND Movimenti_dettagli.Id_Agenda = Mov_Destinazioni.Id_Agenda AND  ")
            StrSQL.Append(" Movimenti_dettagli.Id_Mov = Mov_Destinazioni.Id_Mov AND Movimenti_dettagli.Id_Mov_Det = Mov_Destinazioni.Id_Mov_Det  ")
            StrSQL.Append(" INNER JOIN Cantina_Vasche ON Cantina_Vasche.PIVA = Mov_Destinazioni.Piva  AND Cantina_Vasche.sa_cod = Mov_Destinazioni.sa_cod AND Cantina_Vasche.vas_cod = Mov_Destinazioni.Id_destinazione  ")
            StrSQL.Append(" INNER JOIN UnitaMisura ON Cantina_Vasche.Udm_Cod_Capacita = UnitaMisura.Udm_Cod ")

            StrSQL.Append(" WHERE Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(Data_Inizio) & " " + vbCrLf)
            StrSQL.Append(" AND Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(Data_Fine) & " " + vbCrLf)

            If Piva <> "" Then
                StrSQL.Append(" AND Agenda.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If

            If Id_Agenda <> 0 Then
                StrSQL.Append(" AND Agenda.Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & " ")
            End If

            If Cau_Mov <> "" Then
                StrSQL.Append(" AND Movimenti.Cau_Mov = '" & Agro_SQL_SaveText(Cau_Mov) & "' ")
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   Cantina_Vasche.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   Cantina_Vasche.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY Cantina_Vasche.Identificativo ")
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

    '###################################################################################
    Public Function Identificativo_from_VasCod(ByVal Piva As String,
                                                 ByVal Sa_Cod As Integer,
                                                ByVal Vas_Cod As Integer,
                                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                ) As String

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Cantina_Vasche_R.Identificativo_from_VasCod()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim Identificativo As String = ""

        Try

            DT = Leggi(Piva,
                        Sa_Cod,
                        Vas_Cod,
                        "", "",
                        objParametri)

            If Not DT Is Nothing AndAlso DT.Rows.Count > 0 Then
                Identificativo = DT.Rows(0).Item("Identificativo")
            End If

            DT = Nothing

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return Identificativo

    End Function

    '##############################################################################################
    Public Function Leggi_Celle(ByVal Piva As String,
                                ByVal Sa_Cod As Integer,
                            ByVal xFiltroAggiuntivo As String,
                            ByVal xOrderBy As String,
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                            ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Cantina_Vasche_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.AppendLine("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;")
            StrSQL.AppendLine("SELECT cc.Piva,")
            StrSQL.AppendLine(" cc.Sa_Cod,")
            StrSQL.AppendLine(" ca.sa_nome Sa_Des,")
            StrSQL.AppendLine(" cv.Piano_Cod,")
            StrSQL.AppendLine(" ci.Modulo_Generazione,")
            StrSQL.AppendLine(" cv.Vas_Cod,")
            StrSQL.AppendLine(" cc.Piano_Des,")
            StrSQL.AppendLine(" ci.Insieme_Cod,")
            StrSQL.AppendLine(" ci.Insieme_Des,")
            StrSQL.AppendLine(" cv.Identificativo,")
            StrSQL.AppendLine(" cv.inviato,")
            StrSQL.AppendLine(" cv.Validita_Inizio,")
            StrSQL.AppendLine(" cv.Validita_Fine,")
            StrSQL.AppendLine(" cv.Data_Creazione,")
            StrSQL.AppendLine(" cv.Username_Creazione")
            StrSQL.AppendLine("FROM Cantina_Caratteristiche cc WITH(NOLOCK)")
            StrSQL.AppendLine("JOIN Centri_Aziendali ca WITH(NOLOCK)")
            StrSQL.AppendLine("ON cc.Piva = ca.Piva")
            StrSQL.AppendLine("AND cc.Sa_Cod = ca.Sa_Cod ")
            StrSQL.AppendLine("JOIN Cantina_Insiemi ci WITH(NOLOCK)")
            StrSQL.AppendLine("ON cc.Piva = ci.Piva")
            StrSQL.AppendLine("AND cc.Sa_Cod = ci.Sa_Cod ")
            StrSQL.AppendLine("AND cc.Piano_Cod = ci.Piano_Cod ")
            StrSQL.AppendLine("JOIN Cantina_Vasche cv WITH(NOLOCK)")
            StrSQL.AppendLine("ON cc.Piva = cv.Piva")
            StrSQL.AppendLine("AND cc.Sa_Cod = cv.Sa_Cod ")
            StrSQL.AppendLine("AND cc.Piano_Cod = cv.Piano_Cod")
            StrSQL.AppendLine("AND ci.Insieme_Cod = cv.Insieme_Cod ")
            StrSQL.AppendLine("AND ci.Modulo_Generazione = cv.Modulo_Generazione")
            StrSQL.AppendLine(" WHERE 1=1")

            If Piva <> "" Then
                StrSQL.Append(" AND cv.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If

            If Sa_Cod <> 0 Then
                StrSQL.Append(" AND cv.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            Else
                'leggo se ci sono filtri sui centri
                Dim objUtentiVisibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
                Dim FiltroCentri As String = ""
                Dim DtCentriVisibili As DataTable
                DtCentriVisibili = objUtentiVisibilita.Leggi(enum_TipoEntita.Centro, " Piva='" & Agro_SQL_SaveText(Piva) & "'", "", objParametri)
                If Not DtCentriVisibili Is Nothing AndAlso DtCentriVisibili.Rows.Count > 0 Then
                    For i = 0 To DtCentriVisibili.Rows.Count - 1
                        FiltroCentri &= DtCentriVisibili.Rows(i).Item("sa_cod") & ","
                    Next
                    If FiltroCentri <> "" Then
                        StrSQL.AppendLine("AND cv.sa_cod IN (" & Agro_SQL_Save_Clausola_IN(Left(FiltroCentri, FiltroCentri.Length - 1), False) & ") ")
                    End If
                End If
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine("AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.AppendLine("AND   cv.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.AppendLine("AND   cv.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.AppendLine("ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine("ORDER BY ci.Insieme_Des")
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





'#################################################################
'#################################################################
'#################################################################

Public Class Cantina_Vasche_W
    Inherits AgronicaCoreDataProvider.DataProvider



    '##############################################################################################
    Public Function Scrivi(
                  ByVal Piva As String _
                , ByVal Sa_Cod As Integer _
                , ByVal Vas_Cod As Integer _
                , ByVal Piano_Cod As Integer _
                , ByVal Identificativo As String _
                , ByVal Numero_Serie As String _
                , ByVal Materiale_Cod As Integer _
                , ByVal Appoggio_Cod As Integer _
                , ByVal Inclinato As String _
                , ByVal Tipo_Tasca As Integer _
                , ByVal Coibentata As String _
                , ByVal Udm_Cod_Capacita As Integer _
                , ByVal Capacita_Nominale As String _
                , ByVal Capacita_Effettiva As String _
                , ByVal Udm_Cod_Altezza As Integer _
                , ByVal Altezza_Cilindro As String _
                , ByVal Altezza_Totale As String _
                , ByVal Udm_Cod_Peso As Integer _
                , ByVal Peso As String _
                , ByVal Note As String _
                , ByVal DimX As Integer _
                , ByVal DimY As Integer _
                , ByVal Rotazione As String _
                , ByVal Costo_Acquisto As String _
                , ByVal Modello As String _
                , ByVal Ammortamento As String _
                , ByVal Ultima_Revisione As DateTime _
                , ByVal Tipo As String _
                , ByVal PosX As Integer _
                , ByVal PosY As Integer _
                , ByVal Spessore As Integer _
                , ByVal Refrigerata As String _
                , ByVal Colore_Esterno As Integer _
                , ByVal Tipo_Serbatoio As Integer _
                , ByVal Validita_Inizio As DateTime _
                , ByVal Validita_Fine As DateTime _
                , ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                , Optional ByVal Data_creazione As DateTime = #2/1/1900# _
                , Optional ByVal Data_modifica As DateTime = #2/1/1900# _
                , Optional ByVal username_creazione As String = "" _
                , Optional ByVal username_modifica As String = ""
                ) As Boolean


        Dim NomeRoutine As String = "Scrivi()"

        '====================================================================================
        'Parametri opzionali :

        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If Data_creazione = #2/1/1900# Then
                Data_creazione = Date.Now
            End If

            If Data_modifica = #2/1/1900# Then
                Data_modifica = Date.Now
            End If

            If username_creazione = "" Then
                username_creazione = objParametri.UsernameOperazione
            End If

            If username_modifica = "" Then
                username_modifica = objParametri.UsernameOperazione
            End If



            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append(" INSERT Cantina_Vasche " + vbCrLf)

            StrSQL.Append("              (")

            StrSQL.Append("   [Piva] " & vbCrLf)
            StrSQL.Append("  ,[Sa_Cod] " & vbCrLf)
            StrSQL.Append("  ,[Vas_Cod] " & vbCrLf)
            StrSQL.Append("  ,[Piano_Cod] " & vbCrLf)
            StrSQL.Append("  ,[Identificativo] " & vbCrLf)
            StrSQL.Append("  ,[Numero_Serie] " & vbCrLf)
            StrSQL.Append("  ,[Materiale_Cod] " & vbCrLf)
            StrSQL.Append("  ,[Appoggio_Cod] " & vbCrLf)
            StrSQL.Append("  ,[Inclinato] " & vbCrLf)
            StrSQL.Append("  ,[Tipo_Tasca] " & vbCrLf)
            StrSQL.Append("  ,[Coibentata] " & vbCrLf)
            StrSQL.Append("  ,[Udm_Cod_Capacita] " & vbCrLf)
            StrSQL.Append("  ,[Capacita_Nominale] " & vbCrLf)
            StrSQL.Append("  ,[Capacita_Effettiva] " & vbCrLf)
            StrSQL.Append("  ,[Udm_Cod_Altezza] " & vbCrLf)
            StrSQL.Append("  ,[Altezza_Cilindro] " & vbCrLf)
            StrSQL.Append("  ,[Altezza_Totale] " & vbCrLf)
            StrSQL.Append("  ,[Udm_Cod_Peso] " & vbCrLf)
            StrSQL.Append("  ,[Peso] " & vbCrLf)
            StrSQL.Append("  ,[Note] " & vbCrLf)
            StrSQL.Append("  ,[DimX] " & vbCrLf)
            StrSQL.Append("  ,[DimY] " & vbCrLf)
            StrSQL.Append("  ,[Rotazione] " & vbCrLf)
            StrSQL.Append("  ,[Costo_Acquisto] " & vbCrLf)
            StrSQL.Append("  ,[Modello] " & vbCrLf)
            StrSQL.Append("  ,[Ammortamento] " & vbCrLf)
            StrSQL.Append("  ,[Ultima_Revisione] " & vbCrLf)
            StrSQL.Append("  ,[Tipo] " & vbCrLf)
            StrSQL.Append("  ,[PosX] " & vbCrLf)
            StrSQL.Append("  ,[PosY] " & vbCrLf)
            StrSQL.Append("  ,[Spessore] " & vbCrLf)
            StrSQL.Append("  ,[Validita_Inizio] " & vbCrLf)
            StrSQL.Append("  ,[Validita_Fine] " & vbCrLf)
            StrSQL.Append("  ,[Refrigerata] " & vbCrLf)
            StrSQL.Append("  ,[Colore_Esterno] " & vbCrLf)
            StrSQL.Append("  ,[Tipo_Serbatoio], " & vbCrLf)


            StrSQL.Append("              Inviato,            datainvio, ")
            StrSQL.Append("              Data_Creazione,     Data_Modifica, ")
            StrSQL.Append("              UserName_Creazione, UserName_Modifica, ")
            StrSQL.Append("              Validita_Inizio,    Validita_Fine ")

            StrSQL.Append("              ) ")

            StrSQL.Append(" VALUES ( ")

            StrSQL.Append(" '" & Agro_SQL_SaveText(Piva) & "'" & vbCrLf)
            StrSQL.Append(", " & Agro_SQL_SaveNum(Sa_Cod) & " " & vbCrLf)
            StrSQL.Append(", " & Agro_SQL_SaveNum(Vas_Cod) & " " & vbCrLf)
            StrSQL.Append(", " & Agro_SQL_SaveNum(Piano_Cod) & " " & vbCrLf)
            StrSQL.Append(",'" & Agro_SQL_SaveText(Identificativo) & "'" & vbCrLf)
            StrSQL.Append(",'" & Agro_SQL_SaveText(Numero_Serie) & "'" & vbCrLf)
            StrSQL.Append(", " & Agro_SQL_SaveNum(Materiale_Cod) & " " & vbCrLf)
            StrSQL.Append(", " & Agro_SQL_SaveNum(Appoggio_Cod) & " " & vbCrLf)
            StrSQL.Append(",'" & Agro_SQL_SaveText(Inclinato) & "'" & vbCrLf)
            StrSQL.Append(", " & Agro_SQL_SaveNum(Tipo_Tasca) & " " & vbCrLf)
            StrSQL.Append(",'" & Agro_SQL_SaveText(Coibentata) & "'" & vbCrLf)
            StrSQL.Append(", " & Agro_SQL_SaveNum(Udm_Cod_Capacita) & " " & vbCrLf)
            StrSQL.Append(",'" & Agro_SQL_SaveText(Capacita_Nominale) & "'" & vbCrLf)
            StrSQL.Append(",'" & Agro_SQL_SaveText(Capacita_Effettiva) & "'" & vbCrLf)
            StrSQL.Append(", " & Agro_SQL_SaveNum(Udm_Cod_Altezza) & " " & vbCrLf)
            StrSQL.Append(",'" & Agro_SQL_SaveText(Altezza_Cilindro) & "'" & vbCrLf)
            StrSQL.Append(",'" & Agro_SQL_SaveText(Altezza_Totale) & "'" & vbCrLf)
            StrSQL.Append(", " & Agro_SQL_SaveNum(Udm_Cod_Peso) & " " & vbCrLf)
            StrSQL.Append(",'" & Agro_SQL_SaveText(Peso) & "'" & vbCrLf)
            StrSQL.Append(",'" & Agro_SQL_SaveText(Note) & "'" & vbCrLf)
            StrSQL.Append(", " & Agro_SQL_SaveNum(DimX) & " " & vbCrLf)
            StrSQL.Append(", " & Agro_SQL_SaveNum(DimY) & " " & vbCrLf)
            StrSQL.Append(",'" & Agro_SQL_SaveText(Rotazione) & "'" & vbCrLf)
            StrSQL.Append(",'" & Agro_SQL_SaveText(Costo_Acquisto) & "'" & vbCrLf)
            StrSQL.Append(",'" & Agro_SQL_SaveText(Modello) & "'" & vbCrLf)
            StrSQL.Append(",'" & Agro_SQL_SaveText(Ammortamento) & "'" & vbCrLf)
            StrSQL.Append(",'" & Agro_SQL_SaveText(Ultima_Revisione) & "'" & vbCrLf)
            StrSQL.Append(",'" & Agro_SQL_SaveText(Tipo) & "'" & vbCrLf)
            StrSQL.Append(", " & Agro_SQL_SaveNum(PosX) & " " & vbCrLf)
            StrSQL.Append(", " & Agro_SQL_SaveNum(PosY) & " " & vbCrLf)
            StrSQL.Append(", " & Agro_SQL_SaveNum(Spessore) & " " & vbCrLf)
            StrSQL.Append(",'" & Agro_SQL_SaveText(Refrigerata) & "'" & vbCrLf)
            StrSQL.Append(", " & Agro_SQL_SaveNum(Colore_Esterno) & " " & vbCrLf)
            StrSQL.Append(", " & Agro_SQL_SaveNum(Tipo_Serbatoio) & " " & vbCrLf)


            StrSQL.Append("         , 0  " + vbCrLf)
            StrSQL.Append("         , Null  " + vbCrLf)

            StrSQL.Append("			, " & Agro_SQL_SaveDateTime(Data_creazione) & "  ")
            StrSQL.Append("			, " & Agro_SQL_SaveDateTime(Data_modifica) & "  ")
            StrSQL.Append("			,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            StrSQL.Append("			,'" & Agro_SQL_SaveText(username_modifica) & "' ")
            StrSQL.Append("			, " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            StrSQL.Append("			, " & Agro_SQL_SaveDate(Validita_Fine) & "  ")



            StrSQL.Append(") ")

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


    '##############################################################################################
    Public Function ScriviCella(
            ByVal Piva As String,
            ByVal Sa_Cod As Integer,
            ByVal Vas_Cod As Integer,
            ByVal Piano_Cod As Integer,
            ByVal Identificativo As String,
            ByVal Insieme_Cod As Integer,
            ByVal Pos_Relativa As String,
            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
            Optional ByVal Modulo_Generazione As Integer = enum_Omni_Modulo_Generazione.FreshFood,
            Optional ByVal Tipo_Destinazione As Integer = CELLA_FRIGORIFERA,
            Optional ByVal Validita_Inizio As Date = AGRODATAINIZIO,
            Optional ByVal Validita_Fine As Date = AGRODATAFINE,
            Optional ByVal Numero_Serie As String = "",
            Optional ByVal Materiale_Cod As String = "",
            Optional ByVal Appoggio_Cod As Integer = 0,
            Optional ByVal Inclinato As Integer = 0,
            Optional ByVal Tipo_Tasca As Integer = 0,
            Optional ByVal Coibentata As Integer = 0,
            Optional ByVal Udm_Cod_Capacita As Integer = 0,
            Optional ByVal Capacita_Nominale As Decimal = 0,
            Optional ByVal Capacita_Effettiva As Decimal = 0,
            Optional ByVal Udm_Cod_Altezza As Integer = 0,
            Optional ByVal Altezza_Cilindro As Decimal = 0,
            Optional ByVal Altezza_Totale As Decimal = 0,
            Optional ByVal Udm_Cod_Peso As Integer = 0,
            Optional ByVal Peso As Decimal = 0,
            Optional ByVal Note As String = "",
            Optional ByVal DimX As Integer = 0,
            Optional ByVal DimY As Integer = 0,
            Optional ByVal Rotazione As Decimal = 0,
            Optional ByVal Costo_Acquisto As Decimal = 0,
            Optional ByVal Modello As String = "",
            Optional ByVal Ammortamento As Decimal = 0,
            Optional ByVal Ultima_Revisione As Date = AGRODATAINIZIO,
            Optional ByVal Tipo As String = "",
            Optional ByVal PosX As Integer = 0,
            Optional ByVal PosY As Integer = 0,
            Optional ByVal Spessore As Integer = 0,
            Optional ByVal Refrigerata As Integer = 0,
            Optional ByVal Colore_Esterno As Integer = 0,
            Optional ByVal Tipo_Serbatoio As Integer = 0,
            Optional ByVal Dimh As Integer = 0
        ) As Boolean

        Dim NomeRoutine As String = "ScriviCella()"

        '====================================================================================
        'Parametri opzionali :

        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            StrSQL.AppendLine(" INSERT Cantina_Vasche ( ")
            StrSQL.AppendLine("   [Piva] ")
            StrSQL.AppendLine("  ,[Sa_Cod] ")
            StrSQL.AppendLine("  ,[Vas_Cod] ")
            StrSQL.AppendLine("  ,[Piano_Cod] ")
            StrSQL.AppendLine("  ,[Identificativo] ")

            StrSQL.AppendLine("  ,[Numero_Serie] ")
            StrSQL.AppendLine("  ,[Materiale_Cod] ")
            StrSQL.AppendLine("  ,[Appoggio_Cod] ")
            StrSQL.AppendLine("  ,[Inclinato] ")
            StrSQL.AppendLine("  ,[Tipo_Tasca] ")
            StrSQL.AppendLine("  ,[Coibentata] ")
            StrSQL.AppendLine("  ,[Udm_Cod_Capacita] ")
            StrSQL.AppendLine("  ,[Capacita_Nominale] ")
            StrSQL.AppendLine("  ,[Capacita_Effettiva] ")
            StrSQL.AppendLine("  ,[Udm_Cod_Altezza] ")
            StrSQL.AppendLine("  ,[Altezza_Cilindro] ")
            StrSQL.AppendLine("  ,[Altezza_Totale] ")
            StrSQL.AppendLine("  ,[Udm_Cod_Peso] ")
            StrSQL.AppendLine("  ,[Peso] ")
            StrSQL.AppendLine("  ,[Note] ")
            StrSQL.AppendLine("  ,[DimX] ")
            StrSQL.AppendLine("  ,[DimY] ")
            StrSQL.AppendLine("  ,[Rotazione] ")
            StrSQL.AppendLine("  ,[Costo_Acquisto] ")
            StrSQL.AppendLine("  ,[Modello] ")
            StrSQL.AppendLine("  ,[Ammortamento] ")
            StrSQL.AppendLine("  ,[Ultima_Revisione] ")
            StrSQL.AppendLine("  ,[Tipo] ")
            StrSQL.AppendLine("  ,[PosX] ")
            StrSQL.AppendLine("  ,[PosY] ")
            StrSQL.AppendLine("  ,[Spessore] ")
            StrSQL.AppendLine("  ,[Refrigerata] ")
            StrSQL.AppendLine("  ,[Colore_Esterno] ")
            StrSQL.AppendLine("  ,[Tipo_Serbatoio] ")
            StrSQL.AppendLine("  ,[Dimh] ")

            StrSQL.AppendLine("  ,[Insieme_Cod] ")
            StrSQL.AppendLine("  ,[Pos_Relativa] ")
            StrSQL.AppendLine("  ,[Modulo_Generazione] ")
            StrSQL.AppendLine("  ,[Tipo_Destinazione] ")

            StrSQL.AppendLine("  ,[Validita_Inizio] ")
            StrSQL.AppendLine("  ,[Validita_Fine] ")

            StrSQL.AppendLine(" ) ")

            StrSQL.AppendLine(" VALUES ( ")
            StrSQL.AppendLine(" '" & Agro_SQL_SaveText(Piva) & "'")
            StrSQL.AppendLine(" , " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            StrSQL.AppendLine(" , " & Agro_SQL_SaveNum(Vas_Cod) & " ")
            StrSQL.AppendLine(" ," & Agro_SQL_SaveNum(Piano_Cod) & " ")
            StrSQL.AppendLine(" ,'" & Agro_SQL_SaveText(Identificativo) & "'")

            StrSQL.AppendLine(" ,'" & Agro_SQL_SaveText(Numero_Serie) & "'")
            StrSQL.AppendLine(" , " & Agro_SQL_SaveNum(Materiale_Cod) & " ")
            StrSQL.AppendLine(" , " & Agro_SQL_SaveNum(Appoggio_Cod) & " ")
            StrSQL.AppendLine(" ,'" & Agro_SQL_SaveText(Inclinato) & "'")
            StrSQL.AppendLine(" , " & Agro_SQL_SaveNum(Tipo_Tasca) & " ")
            StrSQL.AppendLine(" ,'" & Agro_SQL_SaveText(Coibentata) & "'")
            StrSQL.AppendLine(" , " & Agro_SQL_SaveNum(Udm_Cod_Capacita) & " ")
            StrSQL.AppendLine(" ,'" & Agro_SQL_SaveText(Capacita_Nominale) & "'")
            StrSQL.AppendLine(" ,'" & Agro_SQL_SaveText(Capacita_Effettiva) & "'")
            StrSQL.AppendLine(" , " & Agro_SQL_SaveNum(Udm_Cod_Altezza) & " ")
            StrSQL.AppendLine(" ,'" & Agro_SQL_SaveText(Altezza_Cilindro) & "'")
            StrSQL.AppendLine(" ,'" & Agro_SQL_SaveText(Altezza_Totale) & "'")
            StrSQL.AppendLine(" , " & Agro_SQL_SaveNum(Udm_Cod_Peso) & " ")
            StrSQL.AppendLine(" ,'" & Agro_SQL_SaveText(Peso) & "'")
            StrSQL.AppendLine(" ,'" & Agro_SQL_SaveText(Note) & "'")
            StrSQL.AppendLine(" , " & Agro_SQL_SaveNum(DimX) & " ")
            StrSQL.AppendLine(" , " & Agro_SQL_SaveNum(DimY) & " ")
            StrSQL.AppendLine(" ,'" & Agro_SQL_SaveText(Rotazione) & "'")
            StrSQL.AppendLine(" ,'" & Agro_SQL_SaveText(Costo_Acquisto) & "'")
            StrSQL.AppendLine(" ,'" & Agro_SQL_SaveText(Modello) & "'")
            StrSQL.AppendLine(" ,'" & Agro_SQL_SaveText(Ammortamento) & "'")
            StrSQL.AppendLine(" ,'" & Agro_SQL_SaveText(Ultima_Revisione) & "'")
            StrSQL.AppendLine(" ,'" & Agro_SQL_SaveText(Tipo) & "'")
            StrSQL.AppendLine(" , " & Agro_SQL_SaveNum(PosX) & " ")
            StrSQL.AppendLine(" , " & Agro_SQL_SaveNum(PosY) & " ")
            StrSQL.AppendLine(" , " & Agro_SQL_SaveNum(Spessore) & " ")
            StrSQL.AppendLine(" , " & Agro_SQL_SaveNum(Refrigerata) & " ")
            StrSQL.AppendLine(" , " & Agro_SQL_SaveNum(Colore_Esterno) & " ")
            StrSQL.AppendLine(" , " & Agro_SQL_SaveNum(Tipo_Serbatoio) & " ")
            StrSQL.AppendLine(" , " & Agro_SQL_SaveNum(Dimh) & " ")

            StrSQL.AppendLine(" , " & Agro_SQL_SaveNum(Insieme_Cod) & " ")
            StrSQL.AppendLine(" ,'" & Agro_SQL_SaveText(Pos_Relativa) & "'")
            StrSQL.AppendLine(" , " & Agro_SQL_SaveNum(Modulo_Generazione) & " ")
            StrSQL.AppendLine(" , " & Agro_SQL_SaveNum(Tipo_Destinazione) & " ")

            StrSQL.AppendLine(" , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            StrSQL.AppendLine(" , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")
            StrSQL.AppendLine(" ) ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return xRisp

    End Function

    '#################################################################
    Public Function Cancella(ByVal xFiltroAggiuntivo As String,
                              ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                              ) As Boolean

        '----- Descrizione
        Dim NomeRoutine As String = "Cancella()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            StrSQL.Length = 0

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then
                StrSQL.Append(" UPDATE ... ")
                StrSQL.Append(" SET ")
                StrSQL.Append("         Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("         ,Data_Modifica= " & Agro_SQL_SaveDate(Date.Now) & " ")
                StrSQL.Append("         ,Inviato = -1 ")
                StrSQL.Append(" WHERE   1=1 ")
                StrSQL.Append(" AND     Inviato >= 0 ")
            Else
                StrSQL.Append(" DELETE FROM ... ")
                StrSQL.Append(" WHERE 1=1 ")
            End If
            '---------------------------------------------

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
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

#Region "Cantina Vasche EF"
    Public Function CreateCantinaVascheEF(
                                         ByVal Piva As String,
                                         ByVal Sa_Cod As Integer,
                                         ByVal Vas_Cod As Integer,
                                         ByVal Piano_Cod As Integer,
                                         ByVal Identificativo As String,
                                         ByVal Insieme_Cod As Integer,
                                         ByVal Pos_Relativa As String,
                                         ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                         Optional ByVal Modulo_Generazione As Integer = enum_Omni_Modulo_Generazione.FreshFood,
                                         Optional ByVal Tipo_Destinazione As Integer = CELLA_FRIGORIFERA,
                                         Optional ByVal Validita_Inizio As Date = AGRODATAINIZIO,
                                         Optional ByVal Validita_Fine As Date = AGRODATAFINE,
                                         Optional ByVal Numero_Serie As String = "",
                                         Optional ByVal Materiale_Cod As String = "",
                                         Optional ByVal Appoggio_Cod As Integer = 0,
                                         Optional ByVal Inclinato As Integer = 0,
                                         Optional ByVal Tipo_Tasca As Integer = 0,
                                         Optional ByVal Coibentata As Integer = 0,
                                         Optional ByVal Udm_Cod_Capacita As Integer = 0,
                                         Optional ByVal Capacita_Nominale As Decimal = 0,
                                         Optional ByVal Capacita_Effettiva As Decimal = 0,
                                         Optional ByVal Udm_Cod_Altezza As Integer = 0,
                                         Optional ByVal Altezza_Cilindro As Decimal = 0,
                                         Optional ByVal Altezza_Totale As Decimal = 0,
                                         Optional ByVal Udm_Cod_Peso As Integer = 0,
                                         Optional ByVal Peso As Decimal = 0,
                                         Optional ByVal Note As String = "",
                                         Optional ByVal DimX As Integer = 0,
                                         Optional ByVal DimY As Integer = 0,
                                         Optional ByVal Rotazione As Decimal = 0,
                                         Optional ByVal Costo_Acquisto As Decimal = 0,
                                         Optional ByVal Modello As String = "",
                                         Optional ByVal Ammortamento As Decimal = 0,
                                         Optional ByVal Ultima_Revisione As Date = AGRODATAINIZIO,
                                         Optional ByVal Tipo As String = "",
                                         Optional ByVal PosX As Integer = 0,
                                         Optional ByVal PosY As Integer = 0,
                                         Optional ByVal Spessore As Integer = 0,
                                         Optional ByVal Refrigerata As Integer = 0,
                                         Optional ByVal Colore_Esterno As Integer = 0,
                                         Optional ByVal Tipo_Serbatoio As Integer = 0,
                                         Optional ByVal Dimh As Integer = 0
                                         ) As Boolean

        Return False

    End Function

    Public Function EditCantinaVascheEF(
                                       ByVal Piva As String,
                                       ByVal Sa_Cod As Integer,
                                       ByVal Vas_Cod As Integer,
                                       ByVal Piano_Cod As Integer,
                                       ByVal Identificativo As String,
                                       ByVal Insieme_Cod As Integer,
                                       ByVal Pos_Relativa As String,
                                       ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                       Optional ByVal Modulo_Generazione As Integer = enum_Omni_Modulo_Generazione.FreshFood,
                                       Optional ByVal Tipo_Destinazione As Integer = CELLA_FRIGORIFERA,
                                       Optional ByVal Validita_Inizio As Date = AGRODATAINIZIO,
                                       Optional ByVal Validita_Fine As Date = AGRODATAFINE,
                                       Optional ByVal Numero_Serie As String = "",
                                       Optional ByVal Materiale_Cod As String = "",
                                       Optional ByVal Appoggio_Cod As Integer = 0,
                                       Optional ByVal Inclinato As Integer = 0,
                                       Optional ByVal Tipo_Tasca As Integer = 0,
                                       Optional ByVal Coibentata As Integer = 0,
                                       Optional ByVal Udm_Cod_Capacita As Integer = 0,
                                       Optional ByVal Capacita_Nominale As Decimal = 0,
                                       Optional ByVal Capacita_Effettiva As Decimal = 0,
                                       Optional ByVal Udm_Cod_Altezza As Integer = 0,
                                       Optional ByVal Altezza_Cilindro As Decimal = 0,
                                       Optional ByVal Altezza_Totale As Decimal = 0,
                                       Optional ByVal Udm_Cod_Peso As Integer = 0,
                                       Optional ByVal Peso As Decimal = 0,
                                       Optional ByVal Note As String = "",
                                       Optional ByVal DimX As Integer = 0,
                                       Optional ByVal DimY As Integer = 0,
                                       Optional ByVal Rotazione As Decimal = 0,
                                       Optional ByVal Costo_Acquisto As Decimal = 0,
                                       Optional ByVal Modello As String = "",
                                       Optional ByVal Ammortamento As Decimal = 0,
                                       Optional ByVal Ultima_Revisione As Date = AGRODATAINIZIO,
                                       Optional ByVal Tipo As String = "",
                                       Optional ByVal PosX As Integer = 0,
                                       Optional ByVal PosY As Integer = 0,
                                       Optional ByVal Spessore As Integer = 0,
                                       Optional ByVal Refrigerata As Integer = 0,
                                       Optional ByVal Colore_Esterno As Integer = 0,
                                       Optional ByVal Tipo_Serbatoio As Integer = 0,
                                       Optional ByVal Dimh As Integer = 0
                                       ) As Boolean

        Return False

    End Function

    Public Function DeleteCantinaVascheEF(
                                         ByVal Piva As String,
                                         ByVal Sa_Cod As Integer,
                                         ByVal Vas_Cod As Integer,
                                         ByVal objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                         ByVal objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                         Optional NoteLog As String = NOTELOG_ANAGRAFE_NG,
                                         Optional SistemaOrigine As Integer = -1
                                         ) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.Cantina_Vasche_W.DeleteCantinaVascheEF()"
        Dim messaggioErrore As String = ""

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri_Server.StringaConnessione)

        Try
            Dim scopeOption As New TransactionScopeOption
            Dim transactionOptions As New TransactionOptions
            transactionOptions.IsolationLevel = IsolationLevel.ReadUncommitted
            Using scope As New TransactionScope(scopeOption, transactionOptions)
                Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

                    Dim vasca = (From v In GiasContext.Cantina_Vasche Where v.Piva = Piva And
                                                               v.Sa_Cod = Sa_Cod And
                                                               v.Vas_Cod = Vas_Cod).FirstOrDefault

                    Dim vinoXVasche = (From vxv In GiasContext.ws_Reg_Vino_Vasi_xCantina_Vasche Where vxv.Piva = Piva And
                                                                                                    vxv.sa_cod = Sa_Cod And
                                                                                                    vxv.Vas_Cod = Vas_Cod).ToList

                    GiasContext.Cantina_Vasche.Remove(vasca)
                    GiasContext.ws_Reg_Vino_Vasi_xCantina_Vasche.RemoveRange(vinoXVasche)
                    GiasContext.SaveChanges()

                    Dim datiVascaStr = ""
                    If vasca IsNot Nothing Then
                        Dim a As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}
                        datiVascaStr = JsonConvert.SerializeObject(vasca, a)
                    End If

                    'Scrittura tabella Agronica_Log_Anagrafe
                    Dim objLogAnagrafeW As New AgronicaLogAnagrafe_W
                    Dim log As Agronica_Log_Anagrafe = objLogAnagrafeW.CreaLogAnagrafeEF(
                        enum_TipoEntita_Des.CantinaVasche,
                        CStr(Piva),
                        CStr(Sa_Cod),
                        CStr(Vas_Cod),
                        "",
                        Nothing,
                        Nothing,
                        enum_TipoOperazioneDB.Cancellazione,
                        objParametri_Server,
                        enum_Id_Servizio.GiasOnline,
                        NoteLog,
                        datiVascaStr,
                        Origine:=SistemaOrigine
                        )

                    GiasContext.Agronica_Log_Anagrafe.Add(log)
                    GiasContext.SaveChanges()
                    scope.Complete()
                    scope.Dispose()
                End Using
            End Using
        Catch ex As GiasException
            messaggioErrore = ex.Message
            Throw ex
        Catch ex As Exception
            messaggioErrore = ex.Message
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
            Return 0
        End Try

        Return True

    End Function

#End Region


End Class



