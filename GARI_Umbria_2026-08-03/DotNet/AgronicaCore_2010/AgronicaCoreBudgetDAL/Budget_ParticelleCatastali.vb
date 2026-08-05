Imports System.Data.Common
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi

Public Class Budget_ParticelleCatastali_R
    Inherits AgronicaCoreDataProvider.DataProvider
    'Public Function Recupera_Particelle_per_Centro(
    '                                              ByVal Id_Budget As Integer,
    '                                              ByVal Piva As String,
    '                                              ByVal Sa_Cod As Integer,
    '                                              ByVal DataInizio As Date,
    '                                              ByVal DataFine As Date,
    '                                              ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
    '                                              ) As DataTable

    '    '----------------------------------------------------------------
    '    '----- Dimensionamento variabili
    '    '----------------------------------------------------------------
    '    Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.ParticelleCatastali_R.Recupera_Particelle_per_Centro()"

    '    Dim MessaggioErrore As String = ""
    '    Dim objSQL As New System.Text.StringBuilder

    '    Dim RSImpresexPart As DataTable
    '    'Dim StrSQL As String

    '    Dim Dt As New DataTable
    '    Dim Dr As DataRow

    '    Dim DtParticelle As New DataTable
    '    Dim DtCampi As New DataTable
    '    Dim DtAppezzamenti As New DataTable
    '    Dim DtAppezzamentiAgg As New DataTable

    '    Dim i, j, n As Integer
    '    Dim Prov, Com, Sezione, Subalterno, ZVN As String
    '    Dim Foglio, Numero As Integer
    '    Dim Ettari As Integer
    '    Dim Are As Integer
    '    Dim Centiare As Integer
    '    Dim Superficie As Decimal
    '    Dim Sup_Condotta As Decimal

    '    Dim stbQuery As New System.Text.StringBuilder
    '    Dim stbQueryCampi As New System.Text.StringBuilder
    '    Dim stbQueryAppezzamenti As New System.Text.StringBuilder
    '    Dim stbQueryAppezzamentiAgg As New System.Text.StringBuilder

    '    Dim ParticellaPresente As Boolean

    '    Dim DrApp() As DataRow
    '    Dim ArrayDate() As Date
    '    Dim N_Date As Integer = 0
    '    Dim Data As Date
    '    Dim Data1Presente As Boolean
    '    Dim Data2Presente As Boolean

    '    'Dim Sup_Disponibile As Decimal = 0
    '    Dim Sup_Utilizzata As Decimal = 0
    '    Dim Sup_Utilizzata_Max As Decimal = 0

    '    Dim SupSuCampiSquadri As Decimal = 0
    '    Dim SupSuCampiSquadri_Max As Decimal = 0
    '    Dim SupSuAppezzamentiLiberi As Decimal = 0
    '    Dim SupSuAppezzamentiLiberi_Max As Decimal = 0
    '    Dim SupSuAppezzamentiSuCampiNonSquadri As Decimal = 0
    '    Dim SupSuAppezzamentiSuCampiNonSquadri_Max As Decimal = 0

    '    Dim Messaggio As String

    '    '----------------------------------------------------------------
    '    '----- Elenco Appezzamenti Liberi e Utilizzati
    '    '----------------------------------------------------------------
    '    'La seguente query recupera tutte le particelle del centro aziendale indicato.
    '    'Per ogni particella compaiono anche la superficie di intersezione della particella con :
    '    '1. gli SQUADRI del centro;
    '    '2. gli appezzamenti liberi;
    '    '3. gli appezzamenti in campi NON SQUADRI;
    '    '----------------------------------------------------------------
    '    '----------------------------------------------------------------

    '    '----- Definisco la struttura del DataTable

    '    Dt.Columns.Add(New DataColumn("Prov", GetType(String)))
    '    Dt.Columns.Add(New DataColumn("Com", GetType(String)))
    '    Dt.Columns.Add(New DataColumn("COMUNI_PROV", GetType(String)))
    '    Dt.Columns.Add(New DataColumn("LOCALITA", GetType(String)))
    '    Dt.Columns.Add(New DataColumn("Part_Cod", GetType(String)))
    '    Dt.Columns.Add(New DataColumn("Sezione", GetType(String)))
    '    Dt.Columns.Add(New DataColumn("Foglio", GetType(String)))
    '    Dt.Columns.Add(New DataColumn("Numero", GetType(String)))
    '    Dt.Columns.Add(New DataColumn("Subalterno", GetType(String)))
    '    Dt.Columns.Add(New DataColumn("Superficie", GetType(String)))
    '    Dt.Columns.Add(New DataColumn("Particella_ettari", GetType(String)))
    '    Dt.Columns.Add(New DataColumn("Particella_are", GetType(String)))
    '    Dt.Columns.Add(New DataColumn("Particella_centiare", GetType(String)))
    '    Dt.Columns.Add(New DataColumn("Sup_Condotta", GetType(String)))
    '    Dt.Columns.Add(New DataColumn("SuperficieDisponibile", GetType(String)))
    '    Dt.Columns.Add(New DataColumn("SuperficieUtilizzata", GetType(String)))
    '    Dt.Columns.Add(New DataColumn("AreaSuAppLiberi", GetType(String)))
    '    Dt.Columns.Add(New DataColumn("AreaSuCampiSquadri", GetType(String)))
    '    Dt.Columns.Add(New DataColumn("AreaSuAppSuCampiNonSquadri", GetType(String)))
    '    Dt.Columns.Add(New DataColumn("ZVN", GetType(String)))

    '    Dim objImprexPart As New AgronicaCoreAnagrafeDAL.ImpresexParticelle2_R

    '    'ricavo le particelle attive al momento dell'apertura dell'appezzamento
    '    Dim strFiltro As String

    '    If DataInizio > Estremo_Validita_Inizio Then
    '        strFiltro = " AND ImpreseXParticelle.Validita_Inizio <= " & Agro_SQL_SaveDate(DataFine) &
    '                     " AND ImpreseXParticelle.Validita_Fine >=" & Agro_SQL_SaveDate(DataInizio)
    '    End If

    '    If strFiltro <> "" Then
    '        'elimino il primo AND
    '        strFiltro = Right(strFiltro, strFiltro.Length - 4)
    '    End If

    '    RSImpresexPart = objImprexPart.Leggi(
    '        0,
    '        CStr(Piva),
    '        CInt(Sa_Cod),
    '        0,
    '        "",
    '        "",
    '        "",
    '        0,
    '        0,
    '        "",
    '        AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni,
    '        strFiltro,
    '        " ParticelleCatastali.prov,ParticelleCatastali.com,ParticelleCatastali.SEZIONE ,ParticelleCatastali.FOGLIO,ParticelleCatastali.numero",
    '        objParametri,
    '        0,
    '        True
    '        )


    '    If RSImpresexPart.Rows.Count > 0 Then

    '        Dim ii As Integer

    '        '----------------------------------------------------------------------
    '        stbQuery.Length = 0

    '        '------------------------------------------------------------
    '        'Ricavo la superficie di intersezione con gli SQUADRI
    '        '------------------------------------------------------------
    '        stbQuery.AppendLine("SELECT")
    '        stbQuery.AppendLine("    CP.Id_Budget")
    '        stbQuery.AppendLine("    , CP.PIVA")
    '        stbQuery.AppendLine("    , CP.SA_COD")
    '        stbQuery.AppendLine("    , CP.CAMPO_COD")
    '        stbQuery.AppendLine("    , 0 AS APPEZZA")
    '        stbQuery.AppendLine("    , CP.PROV")
    '        stbQuery.AppendLine("    , CP.COM")
    '        stbQuery.AppendLine("    , CP.SEZIONE")
    '        stbQuery.AppendLine("    , CP.FOGLIO")
    '        stbQuery.AppendLine("    , CP.NUMERO")
    '        stbQuery.AppendLine("    , CP.SUBALTERNO")
    '        stbQuery.AppendLine("    , CP.AREA AS AreaSuCampiSquadri")
    '        stbQuery.AppendLine("    , 0 AS AreaSuAppLiberi")
    '        stbQuery.AppendLine("    , 0 AS AreaSuAppSuCampiNonSquadri")
    '        stbQuery.AppendLine("    , CP.AREA")
    '        stbQuery.AppendLine("    , CP.Validita_Inizio")
    '        stbQuery.AppendLine("    , CP.Validita_Fine")
    '        stbQuery.AppendLine("")
    '        stbQuery.AppendLine("FROM Budget_Campi C")
    '        stbQuery.AppendLine("    INNER JOIN Budget_CampiXParticelle CP ON C.Id_Budget = CP.Id_Budget")
    '        stbQuery.AppendLine("        AND C.PIVA = CP.PIVA")
    '        stbQuery.AppendLine("        AND C.SA_COD = CP.SA_COD")
    '        stbQuery.AppendLine("        AND C.Campo_Cod = CP.Campo_Cod")
    '        stbQuery.AppendLine("")
    '        stbQuery.AppendLine(String.Format("WHERE C.Id_Budget = {0}", Agro_SQL_SaveNum(Id_Budget)))
    '        stbQuery.AppendLine(String.Format("    AND C.PIVA = '{0}'", Agro_SQL_SaveText(Piva)))
    '        stbQuery.AppendLine(String.Format("    AND C.SA_COD = {0}", Agro_SQL_SaveNum(Sa_Cod)))

    '        stbQuery.AppendLine("")
    '        stbQuery.AppendLine("UNION")
    '        stbQuery.AppendLine("")

    '        '------------------------------------------------------------
    '        'Ricavo la superficie di intersezione con gli app. LIBERI
    '        '------------------------------------------------------------
    '        stbQuery.AppendLine("SELECT")
    '        stbQuery.AppendLine("    AP.Id_Budget")
    '        stbQuery.AppendLine("    , AP.PIVA")
    '        stbQuery.AppendLine("    , AP.SA_COD")
    '        stbQuery.AppendLine("    , 0 AS CAMPO_COD")
    '        stbQuery.AppendLine("    , AP.APPEZZA")
    '        stbQuery.AppendLine("    , AP.PROV")
    '        stbQuery.AppendLine("    , AP.COM")
    '        stbQuery.AppendLine("    , AP.SEZIONE")
    '        stbQuery.AppendLine("    , AP.FOGLIO")
    '        stbQuery.AppendLine("    , AP.NUMERO")
    '        stbQuery.AppendLine("    , AP.SUBALTERNO")
    '        stbQuery.AppendLine("    , 0 AS AreaSuCampiSquadri")
    '        stbQuery.AppendLine("    , AP.AREA AS AreaSuAppLiberi")
    '        stbQuery.AppendLine("    , 0 AS AreaSuAppSuCampiNonSquadri")
    '        stbQuery.AppendLine("    , AP.AREA")
    '        stbQuery.AppendLine("    , AP.Validita_Inizio")
    '        stbQuery.AppendLine("    , AP.Validita_Fine")
    '        stbQuery.AppendLine("")
    '        stbQuery.AppendLine("FROM Budget_Appezzamento A")
    '        stbQuery.AppendLine("    INNER JOIN Budget_AppezzamentiXParticelle AP ON A.Id_Budget = AP.Id_Budget")
    '        stbQuery.AppendLine("        AND A.PIVA = AP.PIVA")
    '        stbQuery.AppendLine("        AND A.SA_COD = AP.SA_COD")
    '        stbQuery.AppendLine("        AND A.APPEZZA = AP.APPEZZA")
    '        stbQuery.AppendLine("")
    '        stbQuery.AppendLine(String.Format("WHERE A.Id_Budget = {0}", Agro_SQL_SaveNum(Id_Budget)))
    '        stbQuery.AppendLine(String.Format("    AND A.PIVA = '{0}'", Agro_SQL_SaveText(Piva)))
    '        stbQuery.AppendLine(String.Format("    AND A.SA_COD = {0}", Agro_SQL_SaveNum(Sa_Cod)))
    '        stbQuery.AppendLine("    AND A.CAMPO_COD = 0")

    '        stbQuery.AppendLine("")
    '        stbQuery.AppendLine("UNION")
    '        stbQuery.AppendLine("")

    '        '-----------------------------------------------------------------------
    '        'Ricavo la superficie di intersezione con gli app. in campi NON SQUADRI
    '        '-----------------------------------------------------------------------
    '        stbQuery.AppendLine("SELECT")
    '        stbQuery.AppendLine("    AP.Id_Budget")
    '        stbQuery.AppendLine("    , AP.PIVA")
    '        stbQuery.AppendLine("    , AP.SA_COD")
    '        stbQuery.AppendLine("    , 0 AS CAMPO_COD")
    '        stbQuery.AppendLine("    , AP.APPEZZA")
    '        stbQuery.AppendLine("    , AP.PROV")
    '        stbQuery.AppendLine("    , AP.COM")
    '        stbQuery.AppendLine("    , AP.SEZIONE")
    '        stbQuery.AppendLine("    , AP.FOGLIO")
    '        stbQuery.AppendLine("    , AP.NUMERO")
    '        stbQuery.AppendLine("    , AP.SUBALTERNO")
    '        stbQuery.AppendLine("    , 0 AS AreaSuCampiSquadri")
    '        stbQuery.AppendLine("    , 0 AS AreaSuAppLiberi")
    '        stbQuery.AppendLine("    , AP.AREA AS AreaSuAppSuCampiNonSquadri")
    '        stbQuery.AppendLine("    , AP.AREA")
    '        stbQuery.AppendLine("    , AP.Validita_Inizio")
    '        stbQuery.AppendLine("    , AP.Validita_Fine")
    '        stbQuery.AppendLine("")
    '        stbQuery.AppendLine("FROM Budget_Appezzamento A")
    '        stbQuery.AppendLine("    INNER JOIN Budget_AppezzamentiXParticelle AP ON A.Id_Budget = AP.Id_Budget")
    '        stbQuery.AppendLine("        AND A.PIVA = AP.PIVA")
    '        stbQuery.AppendLine("        AND A.SA_COD = AP.SA_COD")
    '        stbQuery.AppendLine("        AND A.APPEZZA = AP.APPEZZA")
    '        stbQuery.AppendLine("")
    '        stbQuery.AppendLine(String.Format("WHERE A.Id_Budget = {0}", Agro_SQL_SaveNum(Id_Budget)))
    '        stbQuery.AppendLine(String.Format("    AND A.PIVA = '{0}'", Agro_SQL_SaveText(Piva)))
    '        stbQuery.AppendLine(String.Format("    AND A.SA_COD = {0}", Agro_SQL_SaveNum(Sa_Cod)))
    '        stbQuery.AppendLine("    AND A.CAMPO_COD <> 0")
    '        stbQuery.AppendLine("    AND NOT EXISTS (")
    '        stbQuery.AppendLine("        SELECT CP.Id_Budget, CP.Piva, CP.Sa_Cod, CP.campo_cod")
    '        stbQuery.AppendLine("        FROM Budget_CampixParticelle CP")
    '        stbQuery.AppendLine("        WHERE CP.Id_Budget= A.Id_Budget")
    '        stbQuery.AppendLine("            AND CP.piva= A.piva")
    '        stbQuery.AppendLine("            AND CP.sa_cod= A.sa_cod")
    '        stbQuery.AppendLine("            AND CP.campo_cod= A.campo_cod")
    '        stbQuery.AppendLine("    )")

    '        'ORDER BY prov,com,sezione,foglio,numero,subalterno
    '        Try
    '            '--------------------------------------------------------------------------
    '            DtParticelle = EseguiQuery_Lettura(objParametri, stbQuery.ToString, NomeRoutine)
    '            '--------------------------------------------------------------------------
    '        Catch ex As Exception
    '            MessaggioErrore = ex.Message
    '            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
    '            Dt = Nothing
    '            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
    '        End Try

    '        '----------------------------------------------------------------------
    '        '----------------------------------------------------------------------
    '        '----------------------------------------------------------------------
    '        For ii = 0 To RSImpresexPart.Rows.Count - 1

    '            'Per ogni particella ricavo la superficie di intersezione 
    '            '1. con eventuali SQUADRI
    '            '2. con altri appezzamenti liberi
    '            '3. con altri appezzamenti aggregati in campi nn squadri

    '            Prov = RSImpresexPart.Rows(ii).Item("prov")
    '            Com = RSImpresexPart.Rows(ii).Item("com")
    '            Sezione = RSImpresexPart.Rows(ii).Item("sezione")
    '            Foglio = CInt(RSImpresexPart.Rows(ii).Item("foglio"))
    '            Numero = CInt(RSImpresexPart.Rows(ii).Item("numero"))
    '            Subalterno = RSImpresexPart.Rows(ii).Item("subalterno")
    '            ZVN = RSImpresexPart.Rows(ii).Item("ZVN")
    '            ParticellaPresente = False

    '            'verifico di nn aver già inserito la particella...
    '            '(il newcom carica piu record se ho piu titoli possesso nella stessa azienda)
    '            For i = 0 To Dt.Rows.Count - 1

    '                If Dt.Rows(i).Item("prov") = Prov And
    '                    Dt.Rows(i).Item("com") = Com And
    '                    Dt.Rows(i).Item("sezione") = Sezione And
    '                    Dt.Rows(i).Item("foglio") = Foglio And
    '                    Dt.Rows(i).Item("numero") = Numero And
    '                    Dt.Rows(i).Item("subalterno") = Subalterno Then
    '                    ParticellaPresente = True
    '                    Exit For
    '                End If

    '            Next

    '            If ParticellaPresente = False Then

    '                'Creo una nuova riga e ne definisco i valori
    '                Dr = Dt.NewRow

    '                Dr.Item("prov") = Prov
    '                Dr.Item("com") = Com
    '                Dr.Item("Sezione") = IIf(Sezione = "0", "", Sezione)
    '                Dr.Item("Foglio") = Foglio
    '                Dr.Item("Numero") = Numero
    '                Dr.Item("Subalterno") = IIf(Subalterno = "0", "", Subalterno)
    '                Dr.Item("COMUNI_PROV") = RSImpresexPart.Rows(ii).Item("COMUNI_PROV")
    '                Dr.Item("LOCALITA") = RSImpresexPart.Rows(ii).Item("LOCALITA")
    '                Dr.Item("Part_Cod") = RSImpresexPart.Rows(ii).Item("part_cod")
    '                Dr.Item("ZVN") = ZVN

    '                Ettari = RSImpresexPart.Rows(ii).Item("ettari")
    '                Are = RSImpresexPart.Rows(ii).Item("are")
    '                Centiare = RSImpresexPart.Rows(ii).Item("centiare")

    '                Dr.Item("Particella_ettari") = RSImpresexPart.Rows(ii).Item("ettari")
    '                Dr.Item("Particella_are") = RSImpresexPart.Rows(ii).Item("are")
    '                Dr.Item("Particella_centiare") = RSImpresexPart.Rows(ii).Item("centiare")

    '                Superficie = AgronicaCoreDataProvider.Conversioni.Ettari_from_EttariAreCentiare(Ettari, Are, Centiare)

    '                Dr.Item("Superficie") = Format(Superficie, "0.0000")

    '                Sup_Condotta = CDbl(RSImpresexPart.Rows(ii).Item("sup_condotta"))
    '                Dr.Item("Sup_Condotta") = Format(Sup_Condotta, "0.0000")


    '                DrApp = DtParticelle.Select(
    '                    "Prov='" & Prov.ToString & "' " &
    '                    "AND Com='" & Com.ToString & "' " &
    '                    "AND sezione='" & Sezione.ToString & "' " &
    '                    "AND foglio=" & Foglio & " " &
    '                    "AND numero=" & Numero & " " &
    '                    "AND subalterno='" & Subalterno.ToString & "' "
    '                    )

    '                N_Date = 0
    '                ReDim ArrayDate(N_Date)

    '                Sup_Utilizzata = 0
    '                Sup_Utilizzata_Max = 0

    '                If Not DrApp Is Nothing Then

    '                    For j = 0 To DrApp.Length - 1

    '                        Data1Presente = False
    '                        Data2Presente = False

    '                        For n = 0 To UBound(ArrayDate)
    '                            If ArrayDate(n) = DrApp(j).Item("validita_inizio") Then
    '                                Data1Presente = True
    '                                Exit For
    '                            End If
    '                        Next
    '                        If Data1Presente = False Then
    '                            ReDim Preserve ArrayDate(N_Date)
    '                            ArrayDate(N_Date) = DrApp(j).Item("validita_inizio")
    '                            N_Date += 1
    '                        End If
    '                        For n = 0 To UBound(ArrayDate)
    '                            If ArrayDate(n) = DrApp(j).Item("validita_fine") Then
    '                                Data2Presente = True
    '                                Exit For
    '                            End If
    '                        Next
    '                        If Data2Presente = False Then
    '                            ReDim Preserve ArrayDate(N_Date)
    '                            ArrayDate(N_Date) = DrApp(j).Item("validita_fine")
    '                            N_Date += 1
    '                        End If

    '                    Next

    '                    '----------------------------------------------------------------
    '                    'aggiungo al vettore la data inizio e fine 
    '                    'dell'appezzamento che sto creando
    '                    Data1Presente = False
    '                    Data2Presente = False

    '                    For n = 0 To UBound(ArrayDate)
    '                        If ArrayDate(n) = DataInizio Then
    '                            Data1Presente = True
    '                            Exit For
    '                        End If
    '                    Next
    '                    If Data1Presente = False Then
    '                        ReDim Preserve ArrayDate(N_Date)
    '                        ArrayDate(N_Date) = DataInizio
    '                        N_Date += 1
    '                    End If
    '                    For n = 0 To UBound(ArrayDate)
    '                        If ArrayDate(n) = DataFine Then
    '                            Data2Presente = True
    '                            Exit For
    '                        End If
    '                    Next
    '                    If Data2Presente = False Then
    '                        ReDim Preserve ArrayDate(N_Date)
    '                        ArrayDate(N_Date) = DataFine
    '                        N_Date += 1
    '                    End If

    '                    '----------------------------------------------------------------
    '                    'ordino le date..
    '                    If Not ArrayDate Is Nothing Then
    '                        Array.Sort(ArrayDate)
    '                    End If

    '                End If

    '                '----------------------------------------------------------------
    '                'per ogni data calcolo la sup disponibile della particella...
    '                For j = 0 To UBound(ArrayDate)

    '                    Data = ArrayDate(j)

    '                    If Data >= DataInizio And Data <= DataFine Then

    '                        Sup_Utilizzata = 0

    '                        Sup_Utilizzata = AgronicaCoreDataProvider.Conversioni.Sup_Utilizzata_Data(Data, DrApp)

    '                        If Sup_Utilizzata > Sup_Utilizzata_Max Then
    '                            Sup_Utilizzata_Max = Sup_Utilizzata
    '                        End If

    '                        ' calcolo la superficie utilizzata da appezzamenti liberi (appezzamenti non associati ad un campo)
    '                        SupSuAppezzamentiLiberi = 0
    '                        SupSuAppezzamentiLiberi = AgronicaCoreDataProvider.Conversioni.CustomDecimalFieldAtDate(
    '                            Data,
    '                            DrApp,
    '                            "AreaSuAppLiberi",
    '                            "validita_inizio",
    '                            "validita_fine"
    '                            )

    '                        If SupSuAppezzamentiLiberi > SupSuAppezzamentiLiberi_Max Then
    '                            SupSuAppezzamentiLiberi_Max = SupSuAppezzamentiLiberi
    '                        End If

    '                        ' calcolo la superficie utilizzata da campi squadri (campi con catasto)
    '                        SupSuCampiSquadri = 0
    '                        SupSuCampiSquadri = AgronicaCoreDataProvider.Conversioni.CustomDecimalFieldAtDate(
    '                            Data,
    '                            DrApp,
    '                            "AreaSuCampiSquadri",
    '                            "validita_inizio",
    '                            "validita_fine"
    '                            )

    '                        If SupSuCampiSquadri > SupSuCampiSquadri_Max Then
    '                            SupSuCampiSquadri_Max = SupSuCampiSquadri
    '                        End If

    '                        ' calcolo la superficie utilizzata da appezzamenti associati a campi non squadri (campi senza catasto)
    '                        SupSuAppezzamentiSuCampiNonSquadri = 0
    '                        SupSuAppezzamentiSuCampiNonSquadri = AgronicaCoreDataProvider.Conversioni.CustomDecimalFieldAtDate(
    '                            Data,
    '                            DrApp,
    '                            "AreaSuAppSuCampiNonSquadri",
    '                            "validita_inizio",
    '                            "validita_fine"
    '                            )

    '                        If SupSuAppezzamentiSuCampiNonSquadri > SupSuAppezzamentiSuCampiNonSquadri_Max Then
    '                            SupSuAppezzamentiSuCampiNonSquadri_Max = SupSuAppezzamentiSuCampiNonSquadri
    '                        End If

    '                    End If

    '                Next

    '                Dr.Item("SuperficieUtilizzata") = Sup_Utilizzata_Max
    '                Dr.Item("SuperficieDisponibile") = Sup_Condotta - Sup_Utilizzata_Max

    '                Dr.Item("AreaSuAppLiberi") = SupSuAppezzamentiLiberi_Max
    '                Dr.Item("AreaSuCampiSquadri") = SupSuCampiSquadri_Max
    '                Dr.Item("AreaSuAppSuCampiNonSquadri") = SupSuAppezzamentiSuCampiNonSquadri_Max

    '                'Associo alla tabella la nuova riga creata
    '                Dt.Rows.Add(Dr)

    '            End If

    '        Next
    '    End If

    '    'Verifico la presenza di errori
    '    If Not IsNothing(Messaggio) Then
    '        'ERRORE
    '        MessaggioErrore = Messaggio
    '        Return Nothing
    '    Else
    '        MessaggioErrore = ""
    '        Return Dt
    '    End If

    'End Function
End Class

Public Class Budget_ParticelleCatastali_W

End Class
