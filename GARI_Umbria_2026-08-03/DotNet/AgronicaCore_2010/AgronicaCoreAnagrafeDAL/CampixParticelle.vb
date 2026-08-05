Imports System.Data.Common
Imports System.Text
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.DataProviderExtensions
Imports System.Data.Entity.Infrastructure

'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################
Public Class CampixParticelle_R
    Inherits AgronicaCoreDataProvider.DataProvider

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Versione che utilizza AgronicaCoreParametri    ''' 
    ''' </summary>
    ''' <param name="Piva"></param>
    ''' <param name="Sa_Cod"></param>
    ''' <param name="Campo_Cod"></param>
    ''' <param name="PROV"></param>
    ''' <param name="COM"></param>
    ''' <param name="SEZIONE"></param>
    ''' <param name="FOGLIO"></param>
    ''' <param name="NUMERO"></param>
    ''' <param name="SUBALTERNO"></param>
    ''' <param name="xSelezioneVariabile"></param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="xOrderBy"></param>
    ''' <param name="objParametri"></param>
    ''' <returns>ritorna il DataTable</returns>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[garavini]	10/06/2010	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Function Leggi(
                         ByVal Piva As String,
                         ByVal Sa_Cod As Int32,
                         ByVal Campo_Cod As Int32,
                         ByVal PROV As String,
                         ByVal COM As String,
                         ByVal SEZIONE As String,
                         ByVal FOGLIO As Long,
                         ByVal NUMERO As Long,
                         ByVal SUBALTERNO As String,
                         ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile,
                         ByVal xFiltroAggiuntivo As String,
                         ByVal xOrderBy As String,
                         ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                         Optional ByVal joinImpreseParticelle As Boolean = False,
                         Optional ByVal idImpreseParticelleDaEscludere As Integer = 0,
                         Optional ByVal filtroImpreseParticelleAssenti As Boolean = False,
                         Optional ByVal isBudget As Boolean = False,
                         Optional ByVal idBudget As Integer = 0
                         ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.CampixParticelle_R.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""                   =>  si leggono tutte le imprese
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            '---------------------------------------------
            StrSQL.Length = 0

            Dim campiTableName As String = "#Campi"
            Dim campiXParticelleTableName As String = "#CampiXParticelle"
            Dim appezzamentoTableName As String = "#Appezzamento"
            Dim appezzamentiXParticelleTableName As String = "#AppezzamentiXParticelle"

            UtilityParticelle.GetQueryTablesAlias(StrSQL, isBudget, campiTableName, campiXParticelleTableName, appezzamentoTableName, appezzamentiXParticelleTableName)

            Select Case xSelezioneVariabile


                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi


                    StrSQL.AppendLine(" SELECT ParticelleCatastali.PART_COD, 
                                    ParticelleCatastali.PROV, ParticelleCatastali.COM, 
                                    ParticelleCatastali.SEZIONE, 
                                    ParticelleCatastali.FOGLIO, 
                                    ParticelleCatastali.NUMERO, 
                                    ParticelleCatastali.SUBALTERNO, 
                                    ParticelleCatastali.PARTITA_CATASTALE, 
                                    ParticelleCatastali.ETTARI, 
                                    ParticelleCatastali.ARE, 
                                    ParticelleCatastali.CENTIARE, 
                                    CP.AREA")

                    StrSQL.AppendLine(String.Format(" FROM  {0} CP", campiXParticelleTableName))
                    StrSQL.AppendLine(" INNER JOIN ParticelleCatastali ON ")
                    StrSQL.AppendLine("       CP.PROV = ParticelleCatastali.PROV ")
                    StrSQL.AppendLine(" AND   CP.COM = ParticelleCatastali.COM ")
                    StrSQL.AppendLine(" AND   CP.SEZIONE = ParticelleCatastali.SEZIONE ")
                    StrSQL.AppendLine(" AND   CP.FOGLIO = ParticelleCatastali.FOGLIO ")
                    StrSQL.AppendLine(" AND   CP.NUMERO = ParticelleCatastali.NUMERO ")
                    StrSQL.AppendLine(" AND   CP.SUBALTERNO = ParticelleCatastali.SUBALTERNO ")

                    SeJoinImpreseParticelle(joinImpreseParticelle, idImpreseParticelleDaEscludere, StrSQL)

                    StrSQL.AppendLine(" WHERE CP.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.AppendLine(" AND   CP.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.AppendLine(" AND   ParticelleCatastali.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.AppendLine(" AND   ParticelleCatastali.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    If Piva <> "" Then
                        StrSQL.AppendLine(" AND CP.PIVA = '" & Agro_SQL_SaveText(Trim(Piva)) & "' ")
                    End If

                    If Sa_Cod <> 0 Then
                        StrSQL.AppendLine(" AND CP.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
                    End If

                    If Campo_Cod <> 0 Then
                        StrSQL.AppendLine(" AND CP.Campo_Cod = " & Agro_SQL_SaveNum(Campo_Cod) & "  ")
                    End If

                    If PROV <> "" Then
                        StrSQL.AppendLine(" AND CP.PROV = '" & Agro_SQL_SaveText(Trim(PROV)) & "' ")
                    End If

                    If COM <> "" Then
                        StrSQL.AppendLine(" AND CP.COM = '" & Agro_SQL_SaveText(Trim(COM)) & "' ")
                    End If

                    If SEZIONE <> "" Then
                        StrSQL.AppendLine(" AND CP.SEZIONE = '" & Agro_SQL_SaveText(LCase(SEZIONE)) & "' ")
                    End If

                    If FOGLIO <> 0 Then
                        StrSQL.AppendLine(" AND CP.FOGLIO =  " & Agro_SQL_SaveNum(FOGLIO) & " ")
                    End If

                    If NUMERO <> 0 Then
                        StrSQL.AppendLine(" AND CP.NUMERO =  " & Agro_SQL_SaveNum(NUMERO) & " ")
                    End If

                    If SUBALTERNO <> "" Then
                        StrSQL.AppendLine(" AND CP.SUBALTERNO = '" & Agro_SQL_SaveText(Trim(LCase(SUBALTERNO))) & "' ")
                    End If

                    SeFiltroImpreseParticelleAssenti(joinImpreseParticelle, filtroImpreseParticelleAssenti, StrSQL)

                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.AppendLine(" AND   CP.Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.AppendLine(" AND   CP.Inviato =-1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))

                    End If
                    '---------------------------------------------

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta

                    StrSQL.AppendLine(" SELECT * , CP.Validita_Inizio as xValidita_Inizio, CP.Validita_Fine as xValidita_Fine ")
                    StrSQL.AppendLine(String.Format(" FROM  {0} CP", campiXParticelleTableName))
                    StrSQL.AppendLine(" INNER JOIN ParticelleCatastali ON ")
                    StrSQL.AppendLine("       CP.PROV = ParticelleCatastali.PROV ")
                    StrSQL.AppendLine(" AND   CP.COM = ParticelleCatastali.COM ")
                    StrSQL.AppendLine(" AND   CP.SEZIONE = ParticelleCatastali.SEZIONE ")
                    StrSQL.AppendLine(" AND   CP.FOGLIO = ParticelleCatastali.FOGLIO ")
                    StrSQL.AppendLine(" AND   CP.NUMERO = ParticelleCatastali.NUMERO ")
                    StrSQL.AppendLine(" AND   CP.SUBALTERNO = ParticelleCatastali.SUBALTERNO ")

                    SeJoinImpreseParticelle(joinImpreseParticelle, idImpreseParticelleDaEscludere, StrSQL)

                    StrSQL.AppendLine(" WHERE CP.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.AppendLine(" AND   CP.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.AppendLine(" AND   ParticelleCatastali.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.AppendLine(" AND   ParticelleCatastali.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    If Piva <> "" Then
                        StrSQL.AppendLine(" AND CP.PIVA = '" & Agro_SQL_SaveText(Trim(Piva)) & "' ")
                    End If

                    If Sa_Cod <> 0 Then
                        StrSQL.AppendLine(" AND CP.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
                    End If

                    If Campo_Cod <> 0 Then
                        StrSQL.AppendLine(" AND CP.Campo_Cod = " & Agro_SQL_SaveNum(Campo_Cod) & "  ")
                    End If

                    If PROV <> "" Then
                        StrSQL.AppendLine(" AND CP.PROV = '" & Agro_SQL_SaveText(Trim(PROV)) & "' ")
                    End If

                    If COM <> "" Then
                        StrSQL.AppendLine(" AND CP.COM = '" & Agro_SQL_SaveText(Trim(COM)) & "' ")
                    End If

                    If SEZIONE <> "" Then
                        StrSQL.AppendLine(" AND CP.SEZIONE = '" & Agro_SQL_SaveText(LCase(SEZIONE)) & "' ")
                    End If

                    If FOGLIO <> 0 Then
                        StrSQL.AppendLine(" AND CP.FOGLIO =  " & Agro_SQL_SaveNum(FOGLIO) & " ")
                    End If

                    If NUMERO <> 0 Then
                        StrSQL.AppendLine(" AND CP.NUMERO =  " & Agro_SQL_SaveNum(NUMERO) & " ")
                    End If

                    If SUBALTERNO <> "" Then
                        StrSQL.AppendLine(" AND CP.SUBALTERNO = '" & Agro_SQL_SaveText(Trim(LCase(SUBALTERNO))) & "' ")
                    End If

                    SeFiltroImpreseParticelleAssenti(joinImpreseParticelle, filtroImpreseParticelleAssenti, StrSQL)

                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.AppendLine(" AND   CP.Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.AppendLine(" AND   CP.Inviato =-1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))

                    End If
                    '---------------------------------------------

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni
                    '
                    '
                    '
                    '

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta

                    StrSQL.AppendLine(" SELECT  CP.*, ")
                    StrSQL.AppendLine(" CP.Validita_Inizio as xValidita_Inizio, ")
                    StrSQL.AppendLine(" CP.Validita_Fine as xValidita_Fine, ")
                    StrSQL.AppendLine(" C.Campo_Des, C.Validita_Inizio as Validita_Inizio_Campo, C.Validita_Fine as Validita_Fine_Campo,")
                    StrSQL.AppendLine(" ParticelleCatastali.ETTARI, ParticelleCatastali.[ARE], ParticelleCatastali.CENTIARE, ")
                    StrSQL.AppendLine(" ISTAT.LOCALITA, ISTAT.COMUNI_PROV, Lista_Province.PROVINCIA, ")
                    StrSQL.AppendLine(" CP.Piva, Imprese.rag_soc, CP.Sa_Cod, Centri_Aziendali.sa_nome ")

                    StrSQL.AppendLine(String.Format(" FROM  {0} CP INNER JOIN  ", campiXParticelleTableName))
                    StrSQL.AppendLine(String.Format(" {0} C ON CP.PIVA = C.PIVA AND CP.SA_COD = C.SA_COD AND  ", campiTableName))
                    StrSQL.AppendLine(" CP.Campo_Cod = C.Campo_Cod INNER JOIN ")
                    StrSQL.AppendLine(" ParticelleCatastali ON CP.PROV = ParticelleCatastali.PROV AND CP.COM = ParticelleCatastali.COM AND  ")
                    StrSQL.AppendLine(" CP.SEZIONE = ParticelleCatastali.SEZIONE AND CP.FOGLIO = ParticelleCatastali.FOGLIO AND  ")
                    StrSQL.AppendLine(" CP.NUMERO = ParticelleCatastali.NUMERO AND ")
                    StrSQL.AppendLine(" CP.SUBALTERNO = ParticelleCatastali.SUBALTERNO INNER JOIN ")
                    StrSQL.AppendLine(" ISTAT ON CP.PROV = ISTAT.PROV AND CP.COM = ISTAT.COM INNER JOIN ")
                    StrSQL.AppendLine(" Lista_Province ON ISTAT.PROV = Lista_Province.PROV INNER JOIN ")
                    StrSQL.AppendLine(" Centri_Aziendali ON CP.PIVA = Centri_Aziendali.PIVA AND CP.sa_cod = Centri_Aziendali.sa_cod INNER JOIN ")
                    StrSQL.AppendLine(" Imprese ON Centri_Aziendali.PIVA = Imprese.PIVA ")

                    SeJoinImpreseParticelle(joinImpreseParticelle, idImpreseParticelleDaEscludere, StrSQL)

                    StrSQL.AppendLine(" WHERE   (ParticelleCatastali.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & ") ")
                    StrSQL.AppendLine(" AND     (ParticelleCatastali.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & ") ")
                    StrSQL.AppendLine(" AND     (CP.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & ") ")
                    StrSQL.AppendLine(" AND     (CP.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & ") ")


                    '----- Condizioni

                    If (Piva <> "") Then
                        StrSQL.AppendLine(" AND CP.PIVA = '" & Agro_SQL_SaveText(Trim(Piva)) & "' ")
                    End If

                    If Sa_Cod <> 0 Then
                        StrSQL.AppendLine(" AND CP.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
                    End If

                    If Campo_Cod <> 0 Then
                        StrSQL.AppendLine(" AND CP.Campo_Cod = " & Agro_SQL_SaveNum(Campo_Cod) & "  ")
                    End If

                    If PROV <> "" Then
                        StrSQL.AppendLine(" AND CP.PROV = '" & Agro_SQL_SaveText(Trim(PROV)) & "' ")
                    End If

                    If COM <> "" Then
                        StrSQL.AppendLine(" AND CP.COM = '" & Agro_SQL_SaveText(Trim(COM)) & "' ")
                    End If

                    If SEZIONE <> "" Then
                        StrSQL.AppendLine(" AND CP.SEZIONE = '" & Agro_SQL_SaveText(Trim(LCase(SEZIONE))) & "' ")
                    End If

                    If FOGLIO <> 0 Then
                        StrSQL.AppendLine(" AND CP.FOGLIO =  " & Agro_SQL_SaveNum(FOGLIO) & " ")
                    End If

                    If NUMERO <> 0 Then
                        StrSQL.AppendLine(" AND CP.NUMERO =  " & Agro_SQL_SaveNum(NUMERO) & " ")
                    End If

                    If SUBALTERNO <> "" Then
                        StrSQL.AppendLine(" AND CP.SUBALTERNO = '" & Agro_SQL_SaveText(Trim(LCase(SUBALTERNO))) & "' ")
                    End If

                    SeFiltroImpreseParticelleAssenti(joinImpreseParticelle, filtroImpreseParticelleAssenti, StrSQL)

                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.AppendLine(" AND   CP.Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.AppendLine(" AND   CP.Inviato =-1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------

                    If xOrderBy <> "" Then
                        StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))

                    End If

            End Select

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

    Private Sub SeJoinImpreseParticelle(ByVal joinImpreseParticelle As Boolean,
                                        ByVal idImpreseParticelleDaEscludere As Integer,
                                        ByRef StrSQL As StringBuilder)

        If joinImpreseParticelle Then

            StrSQL.Append(" LEFT OUTER JOIN ImpreseXParticelle IxP ON")
            StrSQL.Append(" IxP.PIVA = CP.PIVA AND")
            StrSQL.Append(" IxP.SA_COD = CP.SA_COD AND")
            StrSQL.Append(" IxP.PROV = CP.PROV AND")
            StrSQL.Append(" IxP.COM = CP.COM AND")
            StrSQL.Append(" IxP.SEZIONE = CP.SEZIONE AND")
            StrSQL.Append(" IxP.FOGLIO = CP.FOGLIO AND")
            StrSQL.Append(" IxP.NUMERO = CP.NUMERO AND")
            StrSQL.Append(" IxP.SUBALTERNO = CP.SUBALTERNO AND")
            StrSQL.Append(" IxP.Validita_Inizio <= CP.Validita_Fine AND ")
            StrSQL.Append(" IxP.Validita_Fine >= CP.Validita_Inizio")

            If idImpreseParticelleDaEscludere <> 0 Then
                StrSQL.Append(" AND IxP.ID <> " & Agro_SQL_SaveNum(idImpreseParticelleDaEscludere) & "  ")
            End If

        End If

    End Sub

    Private Sub SeFiltroImpreseParticelleAssenti(ByVal joinImpreseParticelle As Boolean,
                                                 ByVal filtroImpreseParticelleAssenti As Boolean,
                                                 ByRef StrSQL As StringBuilder)

        If joinImpreseParticelle AndAlso filtroImpreseParticelleAssenti Then
            StrSQL.Append(" AND IxP.ID IS NULL ")
        End If

    End Sub

    '################################################################################
    Public Function Recupera_Somma_Superfici_Particella_intersecata_con_Campi(
                                                                             ByVal Piva As String,
                                                                             ByVal Sa_Cod As Integer,
                                                                             ByVal Prov As String,
                                                                             ByVal Com As String,
                                                                             ByVal Sezione As String,
                                                                             ByVal Foglio As Integer,
                                                                             ByVal Numero As Integer,
                                                                             ByVal Subalterno As String,
                                                                             ByVal xFiltroAggiuntivo As String,
                                                                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                                             ) As Decimal

        Dim DTParticella As DataTable
        Dim Somma As Decimal = 0

        Dim objCampixParticelle As New AgronicaCoreAnagrafeDAL.CampixParticelle_R


        'modifico la data 
        Dim app_data As New Date
        'app_data = objParametri.FinestraTemporaleFine
        'objParametri.FinestraTemporaleFine = Now.Date


        DTParticella = objCampixParticelle.Leggi(
            CStr(Piva),
            CInt(Sa_Cod),
            CInt(0),
            Prov,
            Com,
            Sezione,
            Foglio,
            Numero,
            Subalterno,
            AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta,
            "CP.validita_fine >= " & Agro_SQL_SaveDate(Now.Date),
            "",
            objParametri
            )


        'resetto la data 
        'objParametri.FinestraTemporaleFine = app_data

        Dim i As Integer
        If Not IsNothing(DTParticella) And DTParticella.Rows.Count <> 0 Then
            For i = 0 To DTParticella.Rows.Count - 1

                Somma = Somma + DTParticella.Rows(i).Item("Area")

                'Passo al prossimo record
            Next
        End If

        Return Somma

    End Function

    '################################################################################
    Public Function Campo_Definito_Come_Squadro(
                                               ByVal Piva As String,
                                               ByVal Sa_Cod As Integer,
                                               ByVal Campo_Cod As Integer,
                                               ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                               Optional ByVal isBudget As Boolean = False,
                                               Optional ByVal idBudget As Integer = 0
                                               ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.CampixParticelle_R.Campo_Definito_Come_Squadro()"

        Dim DT As DataTable

        Dim objCOM As New AgronicaCoreAnagrafeDAL.CampixParticelle_R

        DT = objCOM.Leggi(
            CStr(Piva),
            CInt(Sa_Cod),
            CInt(Campo_Cod),
            "",
            "",
            "",
            0,
            0,
            "",
            AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
            "",
            "",
            objParametri,
            isBudget:=isBudget,
            idBudget:=idBudget
            )

        'Elimino l'oggetto COM
        objCOM = Nothing

        'Verifico se e' presente almeno un record
        Return (Not IsNothing(DT) AndAlso DT.Rows.Count > 0)

    End Function

    '########################################################################################
    Public Function Recupera_Particelle_CAMPO_Squadro(
                                                     ByVal Piva As String,
                                                     ByVal Sa_Cod As Integer,
                                                     ByVal Campo_Cod As Integer,
                                                     ByVal DataInizio As Date,
                                                     ByVal DataFine As Date,
                                                     ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                     Optional ByVal isBudget As Boolean = False,
                                                     Optional ByVal idBudget As Integer = 0
                                                     ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.CampixParticelle_R.Carica_Particelle_CAMPO_Squadro()"

        Dim DtCampi As New DataTable
        Dim DtAppezzamenti As New DataTable
        Dim stbQuery As New System.Text.StringBuilder
        Dim stbQuery1 As New System.Text.StringBuilder
        Dim i, j, n As Integer
        Dim Prov, Com, Sezione, Subalterno As String
        Dim Foglio, Numero As Integer

        Dim DrsApp() As DataRow
        Dim DrsCampi() As DataRow
        Dim DrsAll() As DataRow
        Dim ArrayDate() As Date
        Dim N_Date As Integer = 0
        Dim Data As Date
        Dim Data1Presente As Boolean
        Dim Data2Presente As Boolean

        Dim SupUtilizzataMax As Decimal = 0
        Dim SupUtilizzataAppMax As Decimal = 0
        Dim SupUtilizzataCampiMax As Decimal = 0

        '--------------------------------------------------------------------
        '--------------------------------------------------------------------
        'Dalla superficie di intersezione della particella con il campo, 
        'devo togliere le seguenti quantita' per stabilire la superficie ancora disponibile...
        '- Superfici di intersezione con appezzamenti attivi appartenenti allo stesso campo
        '--------------------------------------------------------------------
        '--------------------------------------------------------------------

        '--------------------------------------------------------------------
        'Superficie di intersezione della particella con il campo 

        stbQuery.Length = 0

        Dim campiTableName As String = "#Campi"
        Dim campiXParticelleTableName As String = "#CampiXParticelle"
        Dim appezzamentoTableName As String = "#Appezzamento"
        Dim appezzamentiXParticelleTableName As String = "#AppezzamentiXParticelle"

        UtilityParticelle.GetQueryTablesAlias(stbQuery, isBudget, campiTableName, campiXParticelleTableName, appezzamentoTableName, appezzamentiXParticelleTableName)

        stbQuery.AppendLine("SELECT")
        stbQuery.AppendLine("    CP2.PIVA")
        stbQuery.AppendLine("    , CP2.SA_COD")
        stbQuery.AppendLine("    , ParticelleCatastali.PART_COD")
        stbQuery.AppendLine("    , CP2.PROV")
        stbQuery.AppendLine("    , CP2.COM")
        stbQuery.AppendLine("    , CP2.SEZIONE")
        stbQuery.AppendLine("    , CP2.FOGLIO")
        stbQuery.AppendLine("    , CP2.NUMERO")
        stbQuery.AppendLine("    , CP2.SUBALTERNO")
        stbQuery.AppendLine("    , CP2.SAU_Convenz_Ettari AS EttariCondotti")
        stbQuery.AppendLine("    , CP2.SAU_Convenz_Are AS AreCondotte")
        stbQuery.AppendLine("    , CP2.SAU_Convenz_Centiare AS CentiareCondotte")
        stbQuery.AppendLine("    , ImpreseXParticelle.TitoloPossesso")
        stbQuery.AppendLine("    , 0 AS SuperficieSquadri")
        stbQuery.AppendLine("    , ISTAT.LOCALITA")
        stbQuery.AppendLine("    , ISTAT.COMUNI_PROV")
        stbQuery.AppendLine("    , 0 AS SuperficieAppLiberi ")
        stbQuery.AppendLine("    , ParticelleCatastali.ETTARI AS particella_ettari")
        stbQuery.AppendLine("    , ParticelleCatastali.ARE AS particella_are")
        stbQuery.AppendLine("    , ParticelleCatastali.CENTIARE AS particella_centiare")
        stbQuery.AppendLine("")

        stbQuery.AppendLine(String.Format("FROM {0} CP2", campiXParticelleTableName))
        stbQuery.AppendLine("")
        stbQuery.AppendLine("    INNER JOIN ImpreseXParticelle ON CP2.PIVA = ImpreseXParticelle.PIVA")
        stbQuery.AppendLine("        AND CP2.SA_COD = ImpreseXParticelle.sa_cod")
        stbQuery.AppendLine("        AND CP2.PROV = ImpreseXParticelle.PROV")
        stbQuery.AppendLine("        AND CP2.COM = ImpreseXParticelle.COM")
        stbQuery.AppendLine("        AND CP2.SEZIONE = ImpreseXParticelle.SEZIONE")
        stbQuery.AppendLine("        AND CP2.FOGLIO = ImpreseXParticelle.FOGLIO")
        stbQuery.AppendLine("        AND CP2.NUMERO = ImpreseXParticelle.NUMERO")
        stbQuery.AppendLine("        AND CP2.SUBALTERNO = ImpreseXParticelle.SUBALTERNO")
        stbQuery.AppendLine("")
        stbQuery.AppendLine("    INNER JOIN ParticelleCatastali ON ImpreseXParticelle.PROV = ParticelleCatastali.PROV")
        stbQuery.AppendLine("        AND ImpreseXParticelle.COM = ParticelleCatastali.COM")
        stbQuery.AppendLine("        AND ImpreseXParticelle.SEZIONE = ParticelleCatastali.SEZIONE")
        stbQuery.AppendLine("        AND ImpreseXParticelle.FOGLIO = ParticelleCatastali.FOGLIO")
        stbQuery.AppendLine("        AND ImpreseXParticelle.NUMERO = ParticelleCatastali.NUMERO")
        stbQuery.AppendLine("        AND ImpreseXParticelle.SUBALTERNO = ParticelleCatastali.SUBALTERNO")
        stbQuery.AppendLine("")
        stbQuery.AppendLine("    INNER JOIN ISTAT ON ParticelleCatastali.PROV = ISTAT.PROV")
        stbQuery.AppendLine("        AND ParticelleCatastali.COM = ISTAT.COM")
        stbQuery.AppendLine("")
        stbQuery.AppendLine(String.Format("WHERE CP2.PIVA = '{0}'", Agro_SQL_SaveText(Piva)))
        stbQuery.AppendLine(String.Format("    AND CP2.SA_COD = {0}", Agro_SQL_SaveNum(Sa_Cod)))
        stbQuery.AppendLine(String.Format("    AND CP2.CAMPO_COD = {0}", Agro_SQL_SaveNum(Campo_Cod)))

        '--------------------------------------------------------------------------
        DtCampi = EseguiQuery_Lettura(objParametri, stbQuery.ToString, NomeRoutine)
        '--------------------------------------------------------------------------

        DtCampi.Columns.Add(New DataColumn("Sup_Condotta", GetType(String)))
        DtCampi.Columns.Add(New DataColumn("SuperficieAppSquadro", GetType(String)))

        '--------------------------------------------------------------------
        stbQuery1.Length = 0

        Me.GetQuerySuperficiIntersezione(stbQuery1, Piva, Sa_Cod, Campo_Cod, isBudget, idBudget)

        '--------------------------------------------------------------------------
        DtAppezzamenti = EseguiQuery_Lettura(objParametri, stbQuery1.ToString, NomeRoutine)
        '--------------------------------------------------------------------------

        For i = 0 To DtCampi.Rows.Count - 1

            Prov = DtCampi.Rows(i).Item("prov")
            Com = DtCampi.Rows(i).Item("com")
            Sezione = DtCampi.Rows(i).Item("sezione")
            Foglio = CInt(DtCampi.Rows(i).Item("foglio"))
            Numero = CInt(DtCampi.Rows(i).Item("numero"))
            Subalterno = DtCampi.Rows(i).Item("subalterno")

            DrsAll = UtilityParticelle.FilterParcelRows(DtAppezzamenti, Prov, Com, Sezione, Foglio, Numero, Subalterno, "")
            DrsApp = UtilityParticelle.FilterParcelRows(DtAppezzamenti, Prov, Com, Sezione, Foglio, Numero, Subalterno, "AND campo_cod = 0 ")
            DrsCampi = UtilityParticelle.FilterParcelRows(DtAppezzamenti, Prov, Com, Sezione, Foglio, Numero, Subalterno, "AND campo_cod <> 0 ")

            ArrayDate = UtilityParticelle.ExtractDateArray(DrsAll, DataInizio, DataFine, "validita_inizio", "validita_fine")

            '----------------------------------------------------------------
            'per ogni data calcolo la sup disponibile della particella...

            SupUtilizzataMax = UtilityParticelle.CalculateMaxUsedSurface(ArrayDate, DataInizio, DataFine, DrsAll, "area", "validita_inizio", "validita_fine")
            SupUtilizzataAppMax = UtilityParticelle.CalculateMaxUsedSurface(ArrayDate, DataInizio, DataFine, DrsApp, "area", "validita_inizio", "validita_fine")
            SupUtilizzataCampiMax = UtilityParticelle.CalculateMaxUsedSurface(ArrayDate, DataInizio, DataFine, DrsCampi, "area", "validita_inizio", "validita_fine")

            DtCampi.Rows(i).Item("SuperficieAppSquadro") = SupUtilizzataAppMax

            Dim supCondotta = Ettari_from_EttariAreCentiare(
                CDbl(DtCampi.Rows(i).Item("EttariCondotti")),
                CDbl(DtCampi.Rows(i).Item("AreCondotte")),
                CDbl(DtCampi.Rows(i).Item("CentiareCondotte"))
                )

            Dim supParticella = Ettari_from_EttariAreCentiare(
                CDbl(DtCampi.Rows(i).Item("particella_ettari")),
                CDbl(DtCampi.Rows(i).Item("particella_are")),
                CDbl(DtCampi.Rows(i).Item("particella_centiare"))
                )

            ' se non è stata impostata la superficie condotta nel campo, assumo che sia il massimo
            If supCondotta = 0 Then
                supCondotta = supParticella - SupUtilizzataCampiMax
            End If

            DtCampi.Rows(i).Item("Sup_Condotta") = supCondotta

        Next

        Return DtCampi

    End Function

    '########################################################################################
    Public Function Recupera_Particelle_CAMPO_Squadro_con_Macrousi(
                                                                  ByVal Piva As String,
                                                                  ByVal Sa_Cod As Integer,
                                                                  ByVal Campo_Cod As Integer,
                                                                  ByVal DataInizio As Date,
                                                                  ByVal DataFine As Date,
                                                                  ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                                  Optional ByVal isBudget As Boolean = False,
                                                                  Optional ByVal idBudget As Integer = 0
                                                                  ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.CampixParticelle_R.Carica_Particelle_CAMPO_Squadro_con_Macrousi()"

        Dim DtCampi As New DataTable
        Dim DtAppezzamenti As New DataTable
        Dim stbQuery As New System.Text.StringBuilder
        Dim stbQuery1 As New System.Text.StringBuilder
        Dim i, j, n As Integer
        Dim Prov, Com, Sezione, Subalterno As String
        Dim Foglio, Numero As Integer

        Dim DrsApp() As DataRow
        Dim DrsCampi() As DataRow
        Dim DrsAll() As DataRow
        Dim ArrayDate() As Date
        Dim N_Date As Integer = 0
        Dim Data As Date
        Dim Data1Presente As Boolean
        Dim Data2Presente As Boolean

        Dim SupUtilizzataMax As Decimal = 0
        Dim SupUtilizzataAppMax As Decimal = 0
        Dim SupUtilizzataCampiMax As Decimal = 0

        '--------------------------------------------------------------------
        '--------------------------------------------------------------------
        'Dalla superficie di intersezione della particella con il campo, 
        'devo togliere le seguenti quantita' per stabilire la superficie ancora disponibile...
        '- Superfici di intersezione con appezzamenti attivi appartenenti allo stesso campo
        '--------------------------------------------------------------------
        '--------------------------------------------------------------------

        '--------------------------------------------------------------------
        'Superficie di intersezione della particella con il campo 

        stbQuery.Length = 0

        Dim campiTableName As String = "#Campi"
        Dim campiXParticelleTableName As String = "#CampiXParticelle"
        Dim appezzamentoTableName As String = "#Appezzamento"
        Dim appezzamentiXParticelleTableName As String = "#AppezzamentiXParticelle"

        UtilityParticelle.GetQueryTablesAlias(stbQuery, isBudget, campiTableName, campiXParticelleTableName, appezzamentoTableName, appezzamentiXParticelleTableName)

        stbQuery.AppendLine("SELECT")
        stbQuery.AppendLine("    CP2.PIVA")
        stbQuery.AppendLine("    , CP2.SA_COD")
        stbQuery.AppendLine("    , ParticelleCatastali.PART_COD")
        stbQuery.AppendLine("    , CP2.PROV")
        stbQuery.AppendLine("    , CP2.COM")
        stbQuery.AppendLine("    , CP2.SEZIONE")
        stbQuery.AppendLine("    , CP2.FOGLIO")
        stbQuery.AppendLine("    , CP2.NUMERO")
        stbQuery.AppendLine("    , CP2.SUBALTERNO")
        stbQuery.AppendLine("    , CP2.SAU_Convenz_Ettari AS Particella_Ettari")
        stbQuery.AppendLine("    , CP2.SAU_Convenz_Are AS Particella_Are")
        stbQuery.AppendLine("    , CP2.SAU_Convenz_Centiare AS Particella_Centiare")
        stbQuery.AppendLine("    , ImpreseXParticelle.TitoloPossesso")
        stbQuery.AppendLine("    , 0 AS SuperficieSquadri")
        stbQuery.AppendLine("    , ISTAT.LOCALITA")
        stbQuery.AppendLine("    , ISTAT.COMUNI_PROV")
        stbQuery.AppendLine("    , 0 AS SuperficieAppLiberi")
        stbQuery.AppendLine("    , ISNULL(ParticelleCatastalixMacrousi.Macrouso_Cod,'') as Macrouso_Cod")
        stbQuery.AppendLine("    , ParticelleCatastalixMacrousi.Superficie AS Sup_Macrouso")
        stbQuery.AppendLine("    , ISNULL(Macrousi.Macrouso_Des,'') AS Macrouso_Des")
        stbQuery.AppendLine("")

        stbQuery.AppendLine("FROM Macrousi")
        stbQuery.AppendLine("")
        stbQuery.AppendLine("    INNER JOIN ParticelleCatastalixMacrousi ON Macrousi.Macrouso_Cod = ParticelleCatastalixMacrousi.Macrouso_Cod")
        stbQuery.AppendLine("")
        stbQuery.AppendLine(String.Format("    RIGHT OUTER JOIN {0} CP2", campiXParticelleTableName))
        stbQuery.AppendLine("")
        stbQuery.AppendLine("    INNER JOIN ImpreseXParticelle ON CP2.PIVA = ImpreseXParticelle.PIVA")
        stbQuery.AppendLine("        AND CP2.SA_COD = ImpreseXParticelle.sa_cod")
        stbQuery.AppendLine("        AND CP2.PROV = ImpreseXParticelle.PROV")
        stbQuery.AppendLine("        AND CP2.COM = ImpreseXParticelle.COM")
        stbQuery.AppendLine("        AND CP2.SEZIONE = ImpreseXParticelle.SEZIONE")
        stbQuery.AppendLine("        AND CP2.FOGLIO = ImpreseXParticelle.FOGLIO")
        stbQuery.AppendLine("        AND CP2.NUMERO = ImpreseXParticelle.NUMERO")
        stbQuery.AppendLine("        AND CP2.SUBALTERNO = ImpreseXParticelle.SUBALTERNO")
        stbQuery.AppendLine("")
        stbQuery.AppendLine("    INNER JOIN ParticelleCatastali ON ImpreseXParticelle.PROV = ParticelleCatastali.PROV")
        stbQuery.AppendLine("        AND ImpreseXParticelle.COM = ParticelleCatastali.COM")
        stbQuery.AppendLine("        AND ImpreseXParticelle.SEZIONE = ParticelleCatastali.SEZIONE")
        stbQuery.AppendLine("        AND ImpreseXParticelle.FOGLIO = ParticelleCatastali.FOGLIO")
        stbQuery.AppendLine("        AND ImpreseXParticelle.NUMERO = ParticelleCatastali.NUMERO")
        stbQuery.AppendLine("        AND ImpreseXParticelle.SUBALTERNO = ParticelleCatastali.SUBALTERNO")
        stbQuery.AppendLine("")
        stbQuery.AppendLine("    INNER JOIN ISTAT ON ParticelleCatastali.PROV = ISTAT.PROV")
        stbQuery.AppendLine("        AND ParticelleCatastali.COM = ISTAT.COM")
        stbQuery.AppendLine("        ON ParticelleCatastalixMacrousi.PROV = ParticelleCatastali.PROV")
        stbQuery.AppendLine("        AND ParticelleCatastalixMacrousi.COM = ParticelleCatastali.COM")
        stbQuery.AppendLine("        AND ParticelleCatastalixMacrousi.SEZIONE = ParticelleCatastali.SEZIONE")
        stbQuery.AppendLine("        AND ParticelleCatastalixMacrousi.FOGLIO = ParticelleCatastali.FOGLIO")
        stbQuery.AppendLine("        AND ParticelleCatastalixMacrousi.NUMERO = ParticelleCatastali.NUMERO")
        stbQuery.AppendLine("        AND ParticelleCatastalixMacrousi.SUBALTERNO = ParticelleCatastali.SUBALTERNO")
        stbQuery.AppendLine("")

        stbQuery.AppendLine(String.Format("WHERE CP2.PIVA = '{0}'", Agro_SQL_SaveText(Piva)))
        stbQuery.AppendLine(String.Format("    AND CP2.SA_COD = {0}", Agro_SQL_SaveNum(Sa_Cod)))
        stbQuery.AppendLine(String.Format("    AND CP2.CAMPO_COD = {0}", Agro_SQL_SaveNum(Campo_Cod)))
        stbQuery.AppendLine("")

        '--------------------------------------------------------------------------
        DtCampi = EseguiQuery_Lettura(objParametri, stbQuery.ToString, NomeRoutine)
        '--------------------------------------------------------------------------

        DtCampi.Columns.Add(New DataColumn("Sup_Condotta", GetType(String)))
        DtCampi.Columns.Add(New DataColumn("SuperficieAppSquadro", GetType(String)))

        '--------------------------------------------------------------------
        stbQuery1.Length = 0

        Me.GetQuerySuperficiIntersezione(stbQuery1, Piva, Sa_Cod, Campo_Cod, isBudget, idBudget)

        '--------------------------------------------------------------------------
        DtAppezzamenti = EseguiQuery_Lettura(objParametri, stbQuery1.ToString, NomeRoutine)
        '--------------------------------------------------------------------------

        For i = 0 To DtCampi.Rows.Count - 1

            Prov = DtCampi.Rows(i).Item("prov")
            Com = DtCampi.Rows(i).Item("com")
            Sezione = DtCampi.Rows(i).Item("sezione")
            Foglio = CInt(DtCampi.Rows(i).Item("foglio"))
            Numero = CInt(DtCampi.Rows(i).Item("numero"))
            Subalterno = DtCampi.Rows(i).Item("subalterno")

            DrsAll = UtilityParticelle.FilterParcelRows(DtAppezzamenti, Prov, Com, Sezione, Foglio, Numero, Subalterno, "")
            DrsApp = UtilityParticelle.FilterParcelRows(DtAppezzamenti, Prov, Com, Sezione, Foglio, Numero, Subalterno, "AND campo_cod = 0 ")
            DrsCampi = UtilityParticelle.FilterParcelRows(DtAppezzamenti, Prov, Com, Sezione, Foglio, Numero, Subalterno, "AND campo_cod <> 0 ")

            ArrayDate = UtilityParticelle.ExtractDateArray(DrsAll, DataInizio, DataFine, "validita_inizio", "validita_fine")

            '----------------------------------------------------------------
            'per ogni data calcolo la sup disponibile della particella...

            SupUtilizzataMax = UtilityParticelle.CalculateMaxUsedSurface(ArrayDate, DataInizio, DataFine, DrsAll, "area", "validita_inizio", "validita_fine")
            SupUtilizzataAppMax = UtilityParticelle.CalculateMaxUsedSurface(ArrayDate, DataInizio, DataFine, DrsApp, "area", "validita_inizio", "validita_fine")
            SupUtilizzataCampiMax = UtilityParticelle.CalculateMaxUsedSurface(ArrayDate, DataInizio, DataFine, DrsCampi, "area", "validita_inizio", "validita_fine")

            DtCampi.Rows(i).Item("SuperficieAppSquadro") = SupUtilizzataAppMax

            DtCampi.Rows(i).Item("Sup_Condotta") = Ettari_from_EttariAreCentiare(
                CDbl(DtCampi.Rows(i).Item("Particella_Ettari")),
                CDbl(DtCampi.Rows(i).Item("Particella_are")),
                CDbl(DtCampi.Rows(i).Item("Particella_centiare"))
                )

            If Not IsDBNull(DtCampi.Rows(i).Item("Sup_Macrouso")) Then
                DtCampi.Rows(i).Item("Sup_Macrouso") = Format(CDbl(DtCampi.Rows(i).Item("Sup_Macrouso")), "0.0000")
            Else
                DtCampi.Rows(i).Item("Sup_Macrouso") = 0
            End If

        Next

        Return DtCampi

    End Function

    '########################################################################################
    Public Function Recupera_Particelle_CAMPO_Squadro_con_MacrousiUtilizzo(
                                                                          ByVal Piva As String,
                                                                          ByVal Sa_Cod As Integer,
                                                                          ByVal Campo_Cod As Integer,
                                                                          ByVal DataInizio As Date,
                                                                          ByVal DataFine As Date,
                                                                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                                          Optional ByVal isBudget As Boolean = False,
                                                                          Optional ByVal idBudget As Integer = 0
                                                                          ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.CampixParticelle_R.Recupera_Particelle_CAMPO_Squadro_con_MacrousiUtilizzo()"

        Dim DtCampi As New DataTable
        Dim DtAppezzamenti As New DataTable
        Dim stbQuery As New System.Text.StringBuilder
        Dim stbQuery1 As New System.Text.StringBuilder
        Dim i, j, n As Integer
        Dim Prov, Com, Sezione, Subalterno As String
        Dim Foglio, Numero As Integer

        Dim DrsApp() As DataRow
        Dim DrsCampi() As DataRow
        Dim DrsAll() As DataRow
        Dim ArrayDate() As Date
        Dim N_Date As Integer = 0
        Dim Data As Date
        Dim Data1Presente As Boolean
        Dim Data2Presente As Boolean

        Dim SupUtilizzataMax As Decimal = 0
        Dim SupUtilizzataAppMax As Decimal = 0
        Dim SupUtilizzataCampiMax As Decimal = 0

        '--------------------------------------------------------------------
        '--------------------------------------------------------------------
        'Dalla superficie di intersezione della particella con il campo, 
        'devo togliere le seguenti quantita' per stabilire la superficie ancora disponibile...
        '- Superfici di intersezione con appezzamenti attivi appartenenti allo stesso campo
        '--------------------------------------------------------------------
        '--------------------------------------------------------------------

        '--------------------------------------------------------------------
        'Superficie di intersezione della particella con il campo 

        stbQuery.Length = 0

        Dim campiTableName As String = "#Campi"
        Dim campiXParticelleTableName As String = "#CampiXParticelle"
        Dim appezzamentoTableName As String = "#Appezzamento"
        Dim appezzamentiXParticelleTableName As String = "#AppezzamentiXParticelle"

        UtilityParticelle.GetQueryTablesAlias(stbQuery, isBudget, campiTableName, campiXParticelleTableName, appezzamentoTableName, appezzamentiXParticelleTableName)

        stbQuery.AppendLine(" SELECT  DISTINCT CP2.PIVA, CP2.SA_COD, ParticelleCatastali.PART_COD,  ")
        stbQuery.AppendLine(" CP2.PROV, CP2.COM, CP2.SEZIONE, CP2.FOGLIO, CP2.NUMERO, CP2.SUBALTERNO,  ")
        stbQuery.AppendLine(" CP2.SAU_Convenz_Ettari AS Particella_Ettari, CP2.SAU_Convenz_Are AS Particella_Are, CP2.SAU_Convenz_Centiare AS Particella_Centiare,  ")
        stbQuery.AppendLine(" ImpreseXParticelle.TitoloPossesso, 0 AS SuperficieSquadri, ")
        stbQuery.AppendLine(" ISTAT.LOCALITA, ISTAT.COMUNI_PROV, ")
        stbQuery.AppendLine(" 0 AS SuperficieAppLiberi, ")
        stbQuery.AppendLine(" ISNULL(ParticelleCatastalixMacrousi.Macrouso_Cod,'') as Macrouso_Cod, ParticelleCatastalixMacrousi.Superficie AS Sup_Macrouso, ISNULL(Macrousi.Macrouso_Des,'') AS Macrouso_Des, ")

        stbQuery.AppendLine(" ISNULL(ParticelleCatastalixMacrousixUtilizzo.Veg_Cod_Agea,'') as Veg_Cod_Agea, ISNULL(Codifica_SpecieVegetali_AGEA.Veg_Des_Agea,'') as Veg_Des_Agea, ")
        stbQuery.AppendLine(" ISNULL(ParticelleCatastalixMacrousixUtilizzo.Cul_Cod_Agea,'') as Cul_Cod_Agea, ISNULL(Codifica_SpecieVegetali_AGEA.Cul_Des_Agea,'') as Cul_Des_Agea, ")
        stbQuery.AppendLine(" ISNULL(ParticelleCatastalixMacrousixUtilizzo.Superficie,0) AS Sup_Utilizzo ")

        stbQuery.AppendLine(" FROM         ParticelleCatastalixMacrousixUtilizzo LEFT OUTER JOIN  ")
        stbQuery.AppendLine(" Codifica_SpecieVegetali_Agea ON ParticelleCatastalixMacrousixUtilizzo.Veg_Cod_Agea = Codifica_SpecieVegetali_Agea.Veg_Cod_Agea AND   ")
        stbQuery.AppendLine(" ParticelleCatastalixMacrousixUtilizzo.Cul_Cod_Agea = Codifica_SpecieVegetali_Agea.Cul_Cod_Agea RIGHT OUTER JOIN  ")
        stbQuery.AppendLine(String.Format(" {0} AS CP2 INNER JOIN  ", campiXParticelleTableName))
        stbQuery.AppendLine(" ImpreseXParticelle ON CP2.PIVA = ImpreseXParticelle.PIVA AND CP2.SA_COD = ImpreseXParticelle.sa_cod AND CP2.PROV = ImpreseXParticelle.PROV AND   ")
        stbQuery.AppendLine(" CP2.COM = ImpreseXParticelle.COM AND CP2.SEZIONE = ImpreseXParticelle.SEZIONE AND CP2.FOGLIO = ImpreseXParticelle.FOGLIO AND   ")
        stbQuery.AppendLine("  CP2.NUMERO = ImpreseXParticelle.NUMERO AND CP2.SUBALTERNO = ImpreseXParticelle.SUBALTERNO INNER JOIN  ")
        stbQuery.AppendLine(" ParticelleCatastali ON ImpreseXParticelle.PROV = ParticelleCatastali.PROV AND ImpreseXParticelle.COM = ParticelleCatastali.COM AND   ")
        stbQuery.AppendLine(" ImpreseXParticelle.SEZIONE = ParticelleCatastali.SEZIONE AND ImpreseXParticelle.FOGLIO = ParticelleCatastali.FOGLIO AND   ")
        stbQuery.AppendLine(" ImpreseXParticelle.NUMERO = ParticelleCatastali.NUMERO AND ImpreseXParticelle.SUBALTERNO = ParticelleCatastali.SUBALTERNO INNER JOIN  ")
        stbQuery.AppendLine(" ISTAT ON ParticelleCatastali.PROV = ISTAT.PROV AND ParticelleCatastali.COM = ISTAT.COM ON   ")
        stbQuery.AppendLine(" ParticelleCatastalixMacrousixUtilizzo.PROV = ParticelleCatastali.PROV AND ParticelleCatastalixMacrousixUtilizzo.COM = ParticelleCatastali.COM AND   ")
        stbQuery.AppendLine(" ParticelleCatastalixMacrousixUtilizzo.SEZIONE = ParticelleCatastali.SEZIONE AND ParticelleCatastalixMacrousixUtilizzo.FOGLIO = ParticelleCatastali.FOGLIO AND   ")
        stbQuery.AppendLine(" ParticelleCatastalixMacrousixUtilizzo.NUMERO = ParticelleCatastali.NUMERO AND   ")
        stbQuery.AppendLine(" ParticelleCatastalixMacrousixUtilizzo.SUBALTERNO = ParticelleCatastali.SUBALTERNO LEFT OUTER JOIN  ")
        stbQuery.AppendLine(" Macrousi INNER JOIN  ")
        stbQuery.AppendLine(" ParticelleCatastalixMacrousi ON Macrousi.Macrouso_Cod = ParticelleCatastalixMacrousi.Macrouso_Cod ON   ")
        stbQuery.AppendLine(" ParticelleCatastali.PROV = ParticelleCatastalixMacrousi.PROV AND ParticelleCatastali.COM = ParticelleCatastalixMacrousi.COM AND   ")
        stbQuery.AppendLine("  ParticelleCatastali.SEZIONE = ParticelleCatastalixMacrousi.SEZIONE AND ParticelleCatastali.FOGLIO = ParticelleCatastalixMacrousi.FOGLIO AND   ")
        stbQuery.AppendLine(" ParticelleCatastali.NUMERO = ParticelleCatastalixMacrousi.NUMERO And ParticelleCatastali.SUBALTERNO = ParticelleCatastalixMacrousi.SUBALTERNO  ")

        stbQuery.AppendLine(" WHERE   (CP2.PIVA = '" & Agro_SQL_SaveText(Piva) & "') ")
        stbQuery.AppendLine(" AND     (CP2.SA_COD = " & Agro_SQL_SaveNum(Sa_Cod) & ") ")
        stbQuery.AppendLine(" AND     (CP2.CAMPO_COD = " & Agro_SQL_SaveNum(Campo_Cod) & ") ")

        '--------------------------------------------------------------------------
        DtCampi = EseguiQuery_Lettura(objParametri, stbQuery.ToString, NomeRoutine)
        '--------------------------------------------------------------------------

        DtCampi.Columns.Add(New DataColumn("Sup_Condotta", GetType(String)))
        DtCampi.Columns.Add(New DataColumn("SuperficieAppSquadro", GetType(String)))

        '--------------------------------------------------------------------
        stbQuery1.Length = 0

        Me.GetQuerySuperficiIntersezione(stbQuery1, Piva, Sa_Cod, Campo_Cod, isBudget, idBudget)

        '--------------------------------------------------------------------------
        DtAppezzamenti = EseguiQuery_Lettura(objParametri, stbQuery1.ToString, NomeRoutine)
        '--------------------------------------------------------------------------

        For i = 0 To DtCampi.Rows.Count - 1

            Prov = DtCampi.Rows(i).Item("prov")
            Com = DtCampi.Rows(i).Item("com")
            Sezione = DtCampi.Rows(i).Item("sezione")
            Foglio = CInt(DtCampi.Rows(i).Item("foglio"))
            Numero = CInt(DtCampi.Rows(i).Item("numero"))
            Subalterno = DtCampi.Rows(i).Item("subalterno")

            DrsAll = UtilityParticelle.FilterParcelRows(DtAppezzamenti, Prov, Com, Sezione, Foglio, Numero, Subalterno, "")
            DrsApp = UtilityParticelle.FilterParcelRows(DtAppezzamenti, Prov, Com, Sezione, Foglio, Numero, Subalterno, "AND campo_cod = 0 ")
            DrsCampi = UtilityParticelle.FilterParcelRows(DtAppezzamenti, Prov, Com, Sezione, Foglio, Numero, Subalterno, "AND campo_cod <> 0 ")

            ArrayDate = UtilityParticelle.ExtractDateArray(DrsAll, DataInizio, DataFine, "validita_inizio", "validita_fine")

            '----------------------------------------------------------------
            'per ogni data calcolo la sup disponibile della particella...

            SupUtilizzataMax = UtilityParticelle.CalculateMaxUsedSurface(ArrayDate, DataInizio, DataFine, DrsAll, "area", "validita_inizio", "validita_fine")
            SupUtilizzataAppMax = UtilityParticelle.CalculateMaxUsedSurface(ArrayDate, DataInizio, DataFine, DrsApp, "area", "validita_inizio", "validita_fine")
            SupUtilizzataCampiMax = UtilityParticelle.CalculateMaxUsedSurface(ArrayDate, DataInizio, DataFine, DrsCampi, "area", "validita_inizio", "validita_fine")

            DtCampi.Rows(i).Item("SuperficieAppSquadro") = SupUtilizzataAppMax

            DtCampi.Rows(i).Item("Sup_Condotta") = Ettari_from_EttariAreCentiare(
                CDbl(DtCampi.Rows(i).Item("Particella_Ettari")),
                CDbl(DtCampi.Rows(i).Item("Particella_are")),
                CDbl(DtCampi.Rows(i).Item("Particella_centiare"))
                )

            If Not IsDBNull(DtCampi.Rows(i).Item("Sup_Macrouso")) Then
                DtCampi.Rows(i).Item("Sup_Macrouso") = Format(CDbl(DtCampi.Rows(i).Item("Sup_Macrouso")), "0.0000")
            Else
                DtCampi.Rows(i).Item("Sup_Macrouso") = 0
            End If

            If Not IsDBNull(DtCampi.Rows(i).Item("Sup_Utilizzo")) Then
                DtCampi.Rows(i).Item("Sup_Utilizzo") = Format(CDbl(DtCampi.Rows(i).Item("Sup_Utilizzo")), "0.0000")
            Else
                DtCampi.Rows(i).Item("Sup_Utilizzo") = 0
            End If

        Next

        Return DtCampi

    End Function

    Private Sub GetQuerySuperficiIntersezioneAppezzamentiStessoCampo(
                                                                    ByRef stb As System.Text.StringBuilder,
                                                                    ByVal piva As String,
                                                                    ByVal saCod As Integer,
                                                                    ByVal campoCod As Integer,
                                                                    ByVal isBudget As Boolean,
                                                                    ByVal idBudget As Integer,
                                                                    ByVal appezzamentoTableName As String,
                                                                    ByVal appezzamentiXParticelleTableName As String
                                                                    )
        'Superfici di intersezione con appezzamenti attivi appartenenti allo stesso campo
        stb.AppendLine("SELECT")
        stb.AppendLine("    A.Validita_Inizio")
        stb.AppendLine("    , A.Validita_Fine")
        stb.AppendLine("    , A.PIVA")
        stb.AppendLine("    , A.SA_COD")
        stb.AppendLine("    , 0 AS Campo_Cod")
        stb.AppendLine("    , A.APPEZZA")
        stb.AppendLine("    , AP.PROV")
        stb.AppendLine("    , AP.COM")
        stb.AppendLine("    , AP.SEZIONE")
        stb.AppendLine("    , AP.FOGLIO")
        stb.AppendLine("    , AP.NUMERO")
        stb.AppendLine("    , AP.SUBALTERNO")
        stb.AppendLine("    , AP.AREA")
        stb.AppendLine("    , AP.SAU_Convenz_Ettari")
        stb.AppendLine("    , AP.SAU_Convenz_Are")
        stb.AppendLine("    , AP.SAU_Convenz_Centiare")

        If isBudget Then
            stb.AppendLine("    , AP.Id_Budget")
        End If

        stb.AppendLine(String.Format("FROM {0} A", appezzamentoTableName))
        stb.AppendLine("")
        stb.AppendLine(String.Format("    INNER JOIN {0} AP ON A.PIVA = AP.PIVA", appezzamentiXParticelleTableName))
        stb.AppendLine("        AND A.SA_COD = AP.SA_COD")
        stb.AppendLine("        AND A.APPEZZA = AP.APPEZZA")

        If isBudget Then
            stb.AppendLine("        AND A.Id_Budget = AP.Id_Budget")
        End If

        stb.AppendLine("")
        stb.AppendLine(String.Format("WHERE A.PIVA = '{0}'", Agro_SQL_SaveText(piva)))
        stb.AppendLine(String.Format("    AND A.SA_COD = {0}", Agro_SQL_SaveNum(saCod)))
        stb.AppendLine(String.Format("    AND A.CAMPO_COD = {0}", Agro_SQL_SaveNum(campoCod)))

        If isBudget Then
            stb.AppendLine(String.Format("    AND A.Id_Budget = {0}", Agro_SQL_SaveNum(idBudget)))
        End If
    End Sub

    Private Sub GetQuerySuperficiIntersezioneAltriCampiAttivi(
                                                             ByRef stb As System.Text.StringBuilder,
                                                             ByVal piva As String,
                                                             ByVal saCod As Integer,
                                                             ByVal campoCod As Integer,
                                                             ByVal isBudget As Boolean,
                                                             ByVal idBudget As Integer,
                                                             ByVal campiTableName As String,
                                                             ByVal campiXParticelleTableName As String
                                                             )
        'Superfici di intersezione con altri campi attivi, diversi da quello selezionato
        stb.AppendLine("SELECT")
        stb.AppendLine("    C.Validita_Inizio")
        stb.AppendLine("    , C.Validita_Fine")
        stb.AppendLine("    , C.PIVA")
        stb.AppendLine("    , C.SA_COD")
        stb.AppendLine("    , C.Campo_Cod")
        stb.AppendLine("    , 0 AS APPEZZA")
        stb.AppendLine("    , CP.PROV")
        stb.AppendLine("    , CP.COM")
        stb.AppendLine("    , CP.SEZIONE")
        stb.AppendLine("    , CP.FOGLIO")
        stb.AppendLine("    , CP.NUMERO")
        stb.AppendLine("    , CP.SUBALTERNO")
        stb.AppendLine("    , CP.AREA")
        stb.AppendLine("    , CP.SAU_Convenz_Ettari")
        stb.AppendLine("    , CP.SAU_Convenz_Are")
        stb.AppendLine("    , CP.SAU_Convenz_Centiare")

        If isBudget Then
            stb.AppendLine("    , CP.Id_Budget")
        End If

        stb.AppendLine(String.Format("FROM {0} C", campiTableName))
        stb.AppendLine("")
        stb.AppendLine(String.Format("    INNER JOIN {0} CP ON C.PIVA = CP.PIVA", campiXParticelleTableName))
        stb.AppendLine("        AND C.SA_COD = CP.SA_COD")
        stb.AppendLine("        AND C.Campo_Cod = CP.CAMPO_COD")

        If isBudget Then
            stb.AppendLine("        AND C.Id_Budget = CP.Id_Budget")
        End If

        stb.AppendLine("")
        stb.AppendLine(String.Format("WHERE C.PIVA = '{0}'", Agro_SQL_SaveText(piva)))
        stb.AppendLine(String.Format("    AND C.SA_COD = {0}", Agro_SQL_SaveNum(saCod)))
        stb.AppendLine(String.Format("    AND C.CAMPO_COD <> {0}", Agro_SQL_SaveNum(campoCod)))

        If isBudget Then
            stb.AppendLine(String.Format("    AND C.Id_Budget = {0}", Agro_SQL_SaveNum(idBudget)))
        End If
    End Sub

    Private Sub GetQuerySuperficiIntersezione(
                                             ByRef stb As System.Text.StringBuilder,
                                             ByVal piva As String,
                                             ByVal saCod As Integer,
                                             ByVal campoCod As Integer,
                                             ByVal isBudget As Boolean,
                                             ByVal idBudget As Integer
                                             )

        Dim campiTableName As String = "#Campi"
        Dim campiXParticelleTableName As String = "#CampiXParticelle"
        Dim appezzamentoTableName As String = "#Appezzamento"
        Dim appezzamentiXParticelleTableName As String = "#AppezzamentiXParticelle"

        UtilityParticelle.GetQueryTablesAlias(stb, isBudget, campiTableName, campiXParticelleTableName, appezzamentoTableName, appezzamentiXParticelleTableName)

        Me.GetQuerySuperficiIntersezioneAppezzamentiStessoCampo(stb, piva, saCod, campoCod, isBudget, idBudget, appezzamentoTableName, appezzamentiXParticelleTableName)

        stb.AppendLine("")
        stb.AppendLine("UNION")
        stb.AppendLine("")

        Me.GetQuerySuperficiIntersezioneAltriCampiAttivi(stb, piva, saCod, campoCod, isBudget, idBudget, campiTableName, campiXParticelleTableName)

    End Sub

#Region "InvestimentoCatasto"
    Public Function LeggiCampi_Da_Particelle(
        ByVal campiConRiparto As Boolean,
        ByVal campiSenzaRiparto As Boolean,
        ByVal particelleInConduzioneSenzaRipartoSuiCampi As Boolean,
        ByVal tuttoIlCatastoInArchivio As Boolean,
        ByVal gisEntita_Cod As Integer,
        ByVal piva As String,
        ByVal sa_Cod As Integer,
        ByVal DataDa As Date,
        ByVal DataA As Date,
        ByVal idTestataTemp As Integer,
        ByVal LeggiDettagliKendoGrid As Boolean,
        sintesiCUAAEstremiCatastali As Boolean,
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
    ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.AppezzaxParticelle_R.LeggiCampi_Da_Particelle()"

        Dim MessaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim DT As New DataTable

        Try

            Dim UnionRichiesto As Boolean = False

            Dim conteggioXQueryAggrega As Integer = 0
            If campiConRiparto Then
                conteggioXQueryAggrega += 1
            End If
            If campiSenzaRiparto Then
                conteggioXQueryAggrega += 1
            End If
            If particelleInConduzioneSenzaRipartoSuiCampi Then
                conteggioXQueryAggrega += 1
            End If

            If conteggioXQueryAggrega > 1 Then
                stb.AppendLine("SELECT * ")
                stb.AppendLine("FROM (")
            End If


            If campiConRiparto Then

                LeggiCampi_Da_Particelle_GetQuery(
                    enum_ParticelleCampi_TipoQuery.CampiConRiparto,
                    gisEntita_Cod,
                    piva,
                    sa_Cod,
                    DataDa,
                    DataA,
                    idTestataTemp,
                    tuttoIlCatastoInArchivio,
                    sintesiCUAAEstremiCatastali,
                    stb)

                UnionRichiesto = True

            End If

            If campiSenzaRiparto Then

                If UnionRichiesto Then
                    stb.AppendLine(" UNION ALL ")
                End If

                LeggiCampi_Da_Particelle_GetQuery(
                    enum_ParticelleCampi_TipoQuery.CampiSenzaRiparto,
                    gisEntita_Cod,
                    piva,
                    sa_Cod,
                    DataDa,
                    DataA,
                    idTestataTemp,
                    tuttoIlCatastoInArchivio,
                    sintesiCUAAEstremiCatastali,
                    stb)

                UnionRichiesto = True

            End If

            If particelleInConduzioneSenzaRipartoSuiCampi Then

                If UnionRichiesto Then
                    stb.AppendLine(" UNION ALL ")
                End If

                LeggiCampi_Da_Particelle_GetQuery(
                    enum_ParticelleCampi_TipoQuery.ParticelleSenzaRiparto,
                    gisEntita_Cod,
                    piva,
                    sa_Cod,
                    DataDa,
                    DataA,
                    idTestataTemp,
                    tuttoIlCatastoInArchivio,
                    sintesiCUAAEstremiCatastali,
                    stb)

            End If

            If conteggioXQueryAggrega > 1 Then
                stb.AppendLine(" ) agg1")
            End If

            'If LeggiDettagliKendoGrid Then
            '    stb.AppendLine(" order by chiave  ")
            'End If

            'Se l'utente non imposta nessun check, non viene creata la query sopra, non posso eseguirla
            If stb.Length > 0 Then
                '--------------------------------------------------------------------------
                DT = EseguiQuery_Lettura(objParametri, stb.ToString, NomeRoutine)
                '--------------------------------------------------------------------------
            End If

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return DT

    End Function

    Private Sub LeggiCampi_Da_Particelle_GetQuery(
        TipoQuery As enum_ParticelleCampi_TipoQuery,
        gisEntita_Cod As Integer,
        piva As String,
        sa_Cod As Integer,
        DataDa As Date,
        DataA As Date,
        idTestataTemp As Integer,
        tuttoIlCatastoInArchivio As Boolean,
        sintesiCUAAEstremiCatastali As Boolean,
        stb As StringBuilder)

        stb.AppendLine("SELECT")

        If TipoQuery = enum_ParticelleCampi_TipoQuery.CampiConRiparto Then

            stb.AppendLine("    (CampiXParticelle.PIVA ")
            stb.AppendLine("     + '_' + CAST(CampiXParticelle.SA_COD AS varchar(100))")
            stb.AppendLine("     + '_' + CAST(CampiXParticelle.CAMPO_COD AS varchar(100))) AS chiave,")

            stb.AppendLine("    'Campi con Riparto' AS TipoDatoEstratto,")
            stb.AppendLine("    Imprese.rag_soc AS OrganismoReferente,")
            stb.AppendLine("    Imprese.PIVA,")
            stb.AppendLine("    Imprese_Codici.val_cod AS CUAA,")
            stb.AppendLine("    Imprese.rag_soc,")

            If Not sintesiCUAAEstremiCatastali Then
                stb.AppendLine("    Centri_Aziendali.sa_nome,")
                stb.AppendLine("    Campi.Campo_Des,")
                stb.AppendLine("    CampixParticelle.SA_COD,")
                stb.AppendLine("    CampixParticelle.CAMPO_COD,")
            End If

            stb.AppendLine("    ISTAT.COMUNI_PROV,")
            stb.AppendLine("    ISTAT.LOCALITA,")
            stb.AppendLine("    ISTAT.CAP,")
            stb.AppendLine("    CampiXParticelle.PROV,")
            stb.AppendLine("    CampiXParticelle.COM,")
            stb.AppendLine("    CampiXParticelle.SEZIONE,")
            stb.AppendLine("    CampiXParticelle.FOGLIO,")
            stb.AppendLine("    CampiXParticelle.NUMERO,")
            stb.AppendLine("    CampiXParticelle.SUBALTERNO,")
            stb.AppendLine("    (")
            stb.AppendLine("        CAST(ParticelleCatastali.ETTARI AS float) +")
            stb.AppendLine("        (CAST(ParticelleCatastali.ARE AS decimal) * 0.01) +")
            stb.AppendLine("        (CAST(ParticelleCatastali.CENTIARE AS decimal) * 0.0001)")
            stb.AppendLine("    ) AS ParticellaSuperficieHa,")
            stb.AppendLine("    ParticelleCatastali.ETTARI,")
            stb.AppendLine("    ParticelleCatastali.ARE,")
            stb.AppendLine("    ParticelleCatastali.CENTIARE,")


            If sintesiCUAAEstremiCatastali Then
                stb.AppendLine("    SUM(CampiXParticelle.AREA) AS SemTrap_Superficie,")
                stb.AppendLine("    0 AS SemTrap_Ha,")
                stb.AppendLine("    0 AS SemTrap_Are,")
                stb.AppendLine("    0 AS SemTrap_Centiare")
            Else
                stb.AppendLine("    CampiXParticelle.AREA AS SemTrap_Superficie,")
                stb.AppendLine("    CampiXParticelle.SAU_Convenz_Ettari AS SemTrap_Ha,")
                stb.AppendLine("    CampiXParticelle.SAU_Convenz_Are AS SemTrap_Are,")
                stb.AppendLine("    CampiXParticelle.SAU_Convenz_Centiare AS SemTrap_Centiare,")
                stb.AppendLine("    CampiXParticelle.Validita_Inizio,")
                stb.AppendLine("    CampiXParticelle.Validita_Fine")
            End If

            stb.AppendLine("")
            stb.AppendLine("FROM Campixparticelle")

            stb.AppendLine("")
            stb.AppendLine("INNER JOIN Campi")
            stb.AppendLine("    ON Campi.Piva = CampiXParticelle.PIVA")
            stb.AppendLine("    AND Campi.Sa_Cod = CampiXParticelle.SA_COD")
            stb.AppendLine("    AND Campi.Campo_Cod = CampiXParticelle.CAMPO_COD")

            stb.AppendLine("")
            stb.AppendLine("INNER JOIN ParticelleCatastali")
            stb.AppendLine("    ON ParticelleCatastali.PROV = Campixparticelle.PROV")
            stb.AppendLine("    AND ParticelleCatastali.COM = Campixparticelle.COM")
            stb.AppendLine("    AND ParticelleCatastali.SEZIONE = Campixparticelle.SEZIONE")
            stb.AppendLine("    AND ParticelleCatastali.FOGLIO = Campixparticelle.FOGLIO")
            stb.AppendLine("    AND ParticelleCatastali.NUMERO = Campixparticelle.NUMERO")
            stb.AppendLine("    AND ParticelleCatastali.SUBALTERNO = Campixparticelle.SUBALTERNO")

            stb.AppendLine("")
            stb.AppendLine("INNER JOIN Imprese")
            stb.AppendLine("    ON Imprese.PIVA = CampiXParticelle.PIVA")

            stb.AppendLine("")
            stb.AppendLine("INNER JOIN Imprese_Codici")
            stb.AppendLine("    ON Imprese_Codici.PIVA = Imprese.PIVA")
            stb.AppendLine("    AND Imprese_Codici.id_cod = " & enum_CodiciAnagrafe.CodiceCUAA)

            stb.AppendLine("")
            stb.AppendLine("INNER JOIN Centri_Aziendali")
            stb.AppendLine("    ON Centri_Aziendali.sa_cod = CampiXParticelle.SA_COD")
            stb.AppendLine("    AND Centri_Aziendali.PIVA = CampiXParticelle.PIVA")

            If idTestataTemp <> 0 Then
                stb.AppendLine("INNER JOIN ( SELECT DISTINCT idTestataTemp, piva, sa_cod, campo_cod FROM  __tmp_filtroCampi) __tmp_filtroCampi")
                stb.AppendLine("    On __tmp_filtroCampi.piva = CampiXParticelle.PIVA  ")
                stb.AppendLine("    And __tmp_filtroCampi.sa_cod = CampiXParticelle.SA_COD ")
                stb.AppendLine("    And __tmp_filtroCampi.campo_cod = CampiXParticelle.CAMPO_COD ")
                stb.AppendLine("    And __tmp_filtroCampi.idTestataTemp = " & idTestataTemp)
            End If

            stb.AppendLine("")
            stb.AppendLine("INNER JOIN ISTAT")
            stb.AppendLine("    ON ISTAT.PROV = CampiXParticelle.PROV")
            stb.AppendLine("    AND ISTAT.COM = CampiXParticelle.COM")

            stb.AppendLine("")
            stb.AppendLine("WHERE CampiXParticelle.Validita_Inizio <= " & Agro_SQL_SaveDate(DataA) & "")
            stb.AppendLine("    AND CampiXParticelle.Validita_Fine >= " & Agro_SQL_SaveDate(DataDa) & "")

            If Not tuttoIlCatastoInArchivio AndAlso piva <> "" Then
                stb.AppendLine("")
                stb.AppendLine("AND CampiXParticelle.PIVA = '" & Agro_SQL_SaveText(piva) & "'")
            End If
            If sintesiCUAAEstremiCatastali Then
                stb.AppendLine("")
                stb.AppendLine("GROUP BY Imprese.rag_soc,")
                stb.AppendLine("    Imprese.PIVA,")
                stb.AppendLine("	Imprese_Codici.val_cod,")
                stb.AppendLine("    Imprese.rag_soc,")
                stb.AppendLine("    CampixParticelle.PIVA,")
                stb.AppendLine("    CampixParticelle.SA_COD,")
                stb.AppendLine("    CampixParticelle.CAMPO_COD,")
                stb.AppendLine("    Campi.Campo_Des,")
                stb.AppendLine("    ISTAT.COMUNI_PROV,")
                stb.AppendLine("    ISTAT.LOCALITA,")
                stb.AppendLine("    ISTAT.CAP,")
                stb.AppendLine("    CampixParticelle.PROV,")
                stb.AppendLine("    CampixParticelle.COM,")
                stb.AppendLine("    CampiXParticelle.SEZIONE,")
                stb.AppendLine("    CampiXParticelle.FOGLIO,")
                stb.AppendLine("    CampiXParticelle.NUMERO,")
                stb.AppendLine("    CampiXParticelle.SUBALTERNO,")
                stb.AppendLine("    ParticelleCatastali.ETTARI,")
                stb.AppendLine("    ParticelleCatastali.ARE,")
                stb.AppendLine("    ParticelleCatastali.CENTIARE")
            End If
            stb.AppendLine("")
        End If

        If TipoQuery = enum_ParticelleCampi_TipoQuery.CampiSenzaRiparto Then

            stb.AppendLine("    (Campi.PIVA ")
            stb.AppendLine("     + '_' + CAST(Campi.SA_COD AS varchar(100))")
            stb.AppendLine("     + '_' + CAST(Campi.CAMPO_COD AS varchar(100))) AS chiave,")

            stb.AppendLine("    'Campi senza Riparto' AS TipoDatoEstratto,")
            stb.AppendLine("    Imprese.rag_soc AS OrganismoReferente,")
            stb.AppendLine("    Imprese.PIVA,")
            stb.AppendLine("    Imprese_Codici.val_cod AS CUAA,")
            stb.AppendLine("    Imprese.rag_soc,")
            stb.AppendLine("    Centri_Aziendali.sa_nome,")
            stb.AppendLine("    Campi.Campo_Des,")
            stb.AppendLine("    Campi.SA_COD,")
            stb.AppendLine("    Campi.CAMPO_COD,")
            stb.AppendLine("    '' AS COMUNI_PROV,")
            stb.AppendLine("    '' AS LOCALITA,")
            stb.AppendLine("    '' AS CAP,")
            stb.AppendLine("    '' AS PROV,")
            stb.AppendLine("    '' AS COM,")
            stb.AppendLine("    '' AS SEZIONE,")
            stb.AppendLine("    0 AS FOGLIO,")
            stb.AppendLine("    0 AS NUMERO,")
            stb.AppendLine("    '' AS SUBALTERNO,")
            stb.AppendLine("    0 AS ParticellaSuperficieHa,")
            stb.AppendLine("    0 AS ETTARI,")
            stb.AppendLine("    0 AS ARE,")
            stb.AppendLine("    0 AS CENTIARE,")
            stb.AppendLine("    0 AS SemTrap_Superficie,")
            stb.AppendLine("    0 AS SemTrap_Ha,")
            stb.AppendLine("    0 AS SemTrap_Are,")
            stb.AppendLine("    0 AS SemTrap_Centiare,")
            stb.AppendLine("    Campi.Validita_Inizio,")
            stb.AppendLine("    Campi.Validita_Fine")

            stb.AppendLine("")
            stb.AppendLine("FROM Campi")

            stb.AppendLine("")
            stb.AppendLine("INNER JOIN Imprese")
            stb.AppendLine("    ON Imprese.PIVA = Campi.PIVA")

            stb.AppendLine("")
            stb.AppendLine("INNER JOIN Imprese_Codici")
            stb.AppendLine("    ON Imprese_Codici.PIVA = Imprese.PIVA")
            stb.AppendLine("    AND Imprese_Codici.id_cod = " & enum_CodiciAnagrafe.CodiceCUAA)

            stb.AppendLine("")
            stb.AppendLine("INNER JOIN Centri_Aziendali")
            stb.AppendLine("    ON Centri_Aziendali.sa_cod = Campi.SA_COD")
            stb.AppendLine("    AND Centri_Aziendali.PIVA = Campi.PIVA")

            If idTestataTemp <> 0 Then
                stb.AppendLine("INNER JOIN ( SELECT DISTINCT idTestataTemp, piva, sa_cod, campo_cod FROM  __tmp_filtroCampi) __tmp_filtroCampi")
                stb.AppendLine("    On __tmp_filtroCampi.piva = Campi.PIVA  ")
                stb.AppendLine("    And __tmp_filtroCampi.sa_cod = Campi.SA_COD ")
                stb.AppendLine("    And __tmp_filtroCampi.campo_cod = Campi.CAMPO_COD ")
                stb.AppendLine("    And __tmp_filtroCampi.idTestataTemp = " & idTestataTemp)
            End If

            stb.AppendLine("")
            stb.AppendLine("WHERE NOT EXISTS (SELECT CampiXParticelle.SA_COD,")
            stb.AppendLine("                        CampiXParticelle.CAMPO_COD,")
            stb.AppendLine("                        CampiXParticelle.PIVA")
            stb.AppendLine("                    FROM CampiXParticelle")
            stb.AppendLine("                    WHERE CampiXParticelle.PIVA = Campi.Piva")
            stb.AppendLine("                        AND CampiXParticelle.SA_COD = Campi.Sa_Cod")
            stb.AppendLine("                        AND CampiXParticelle.CAMPO_COD = Campi.Campo_Cod)")
            stb.AppendLine("    AND Campi.Validita_Inizio <= " & Agro_SQL_SaveDate(DataA) & "")
            stb.AppendLine("    AND Campi.Validita_Fine >= " & Agro_SQL_SaveDate(DataDa) & "")

            If Not tuttoIlCatastoInArchivio AndAlso piva <> "" Then
                stb.AppendLine("    AND Campi.PIVA = '" & Agro_SQL_SaveText(piva) & "'")
            End If
            stb.AppendLine("")
        End If

        If TipoQuery = enum_ParticelleCampi_TipoQuery.ParticelleSenzaRiparto Then

            stb.AppendLine("    (ImpreseXParticelle.PIVA ")
            stb.AppendLine("     + '_' + cast(ImpreseXParticelle.SA_COD As varchar(100))")
            stb.AppendLine("     + '_' + ImpreseXParticelle.PROV")
            stb.AppendLine("     + '_' + ImpreseXParticelle.COM")
            stb.AppendLine("     + '_' + ImpreseXParticelle.SEZIONE")
            stb.AppendLine("     + '_' + cast(ImpreseXParticelle.FOGLIO As varchar(100))")
            stb.AppendLine("     + '_' + cast(ImpreseXParticelle.NUMERO As varchar(100))")
            stb.AppendLine("     + '_' + ImpreseXParticelle.SUBALTERNO) AS chiave,")

            stb.AppendLine("    'Particelle in conduzione senza riparto sui campi' AS TipoDatoEstratto,")
            stb.AppendLine("    Imprese.rag_soc AS OrganismoReferente,")
            stb.AppendLine("    Imprese.PIVA,")
            stb.AppendLine("    Imprese_Codici.val_cod AS CUAA,")
            stb.AppendLine("    Imprese.rag_soc,")
            stb.AppendLine("    '' AS sa_nome,")
            stb.AppendLine("    '' AS Campo_Des,")
            stb.AppendLine("    ImpreseXParticelle.SA_COD,")
            stb.AppendLine("    0 AS CAMPO_COD,")
            stb.AppendLine("    ISTAT.COMUNI_PROV,")
            stb.AppendLine("    ISTAT.LOCALITA,")
            stb.AppendLine("    ISTAT.CAP,")
            stb.AppendLine("    ImpreseXParticelle.PROV,")
            stb.AppendLine("    ImpreseXParticelle.COM,")
            stb.AppendLine("    ImpreseXParticelle.SEZIONE,")
            stb.AppendLine("    ImpreseXParticelle.FOGLIO,")
            stb.AppendLine("    ImpreseXParticelle.NUMERO,")
            stb.AppendLine("    ImpreseXParticelle.SUBALTERNO,")
            stb.AppendLine("    (")
            stb.AppendLine("        CAST(ParticelleCatastali.ETTARI AS float) +")
            stb.AppendLine("        (CAST(ParticelleCatastali.ARE AS decimal) * 0.01) +")
            stb.AppendLine("        (CAST(ParticelleCatastali.CENTIARE AS decimal) * 0.0001)")
            stb.AppendLine("    ) AS ParticellaSuperficieHa,")
            stb.AppendLine("    ParticelleCatastali.ETTARI,")
            stb.AppendLine("    ParticelleCatastali.ARE,")
            stb.AppendLine("    ParticelleCatastali.CENTIARE,")
            stb.AppendLine("    0 AS SemTrap_Superficie,")
            stb.AppendLine("    0 AS SemTrap_Ha,")
            stb.AppendLine("    0 AS SemTrap_Are,")
            stb.AppendLine("    0 AS SemTrap_Centiare,")
            stb.AppendLine("    ImpreseXParticelle.Validita_Inizio,")
            stb.AppendLine("    ImpreseXParticelle.Validita_Fine")

            stb.AppendLine("")
            stb.AppendLine("FROM ImpreseXParticelle")

            stb.AppendLine("")
            stb.AppendLine("INNER JOIN ParticelleCatastali")
            stb.AppendLine("    ON ParticelleCatastali.PROV = ImpreseXParticelle.PROV")
            stb.AppendLine("    AND ParticelleCatastali.COM = ImpreseXParticelle.COM")
            stb.AppendLine("    AND ParticelleCatastali.SEZIONE = ImpreseXParticelle.SEZIONE")
            stb.AppendLine("    AND ParticelleCatastali.FOGLIO = ImpreseXParticelle.FOGLIO")
            stb.AppendLine("    AND ParticelleCatastali.NUMERO = ImpreseXParticelle.NUMERO")
            stb.AppendLine("    AND ParticelleCatastali.SUBALTERNO = ImpreseXParticelle.SUBALTERNO")

            stb.AppendLine("")
            stb.AppendLine("INNER JOIN Imprese")
            stb.AppendLine("    ON Imprese.PIVA = ImpreseXParticelle.PIVA")

            stb.AppendLine("")
            stb.AppendLine("INNER JOIN Imprese_Codici")
            stb.AppendLine("    ON Imprese_Codici.PIVA = Imprese.PIVA")
            stb.AppendLine("    AND Imprese_Codici.id_cod = " & enum_CodiciAnagrafe.CodiceCUAA)

            stb.AppendLine("")
            stb.AppendLine("INNER JOIN Centri_Aziendali")
            stb.AppendLine("    ON Centri_Aziendali.sa_cod = ImpreseXParticelle.SA_COD")
            stb.AppendLine("    AND Centri_Aziendali.PIVA = ImpreseXParticelle.PIVA")

            If idTestataTemp <> 0 Then
                stb.AppendLine("INNER JOIN ( SELECT DISTINCT idTestataTemp, piva, sa_cod FROM  __tmp_filtroCampi) __tmp_filtroCampi")
                stb.AppendLine("    On __tmp_filtroCampi.piva = ImpreseXParticelle.piva  ")
                stb.AppendLine("    And __tmp_filtroCampi.sa_cod = ImpreseXParticelle.SA_COD ")
                stb.AppendLine("    And __tmp_filtroCampi.idTestataTemp = " & idTestataTemp)
            End If

            stb.AppendLine("")
            stb.AppendLine("INNER JOIN ISTAT")
            stb.AppendLine("    ON ISTAT.PROV = ImpreseXParticelle.PROV")
            stb.AppendLine("    AND ISTAT.COM = ImpreseXParticelle.COM")

            stb.AppendLine("")
            stb.AppendLine("WHERE NOT EXISTS (SELECT CampiXParticelle.SA_COD,")
            stb.AppendLine("                        CampiXParticelle.CAMPO_COD,")
            stb.AppendLine("                        CampiXParticelle.PIVA")
            stb.AppendLine("                    FROM CampiXParticelle")
            stb.AppendLine("                    WHERE CampiXParticelle.PIVA = ImpreseXParticelle.Piva")
            stb.AppendLine("                        AND CampiXParticelle.FOGLIO = ImpreseXParticelle.FOGLIO")
            stb.AppendLine("                        AND CampiXParticelle.NUMERO = ImpreseXParticelle.NUMERO")
            stb.AppendLine("                        AND CampiXParticelle.PROV = ImpreseXParticelle.PROV")
            stb.AppendLine("                        AND CampiXParticelle.COM = ImpreseXParticelle.COM)")
            stb.AppendLine("    AND ImpreseXParticelle.Validita_Inizio <= " & Agro_SQL_SaveDate(DataA) & "")
            stb.AppendLine("    AND ImpreseXParticelle.Validita_Fine >= " & Agro_SQL_SaveDate(DataDa) & "")

            If Not tuttoIlCatastoInArchivio AndAlso piva <> "" Then
                stb.AppendLine("    AND Campi.PIVA = '" & Agro_SQL_SaveText(piva) & "'")
            End If
            stb.AppendLine("")
        End If

    End Sub

    Friend Enum enum_ParticelleCampi_TipoQuery
        NonSpecificato = 0
        CampiConRiparto = 1
        CampiSenzaRiparto = 2
        ParticelleSenzaRiparto = 3
    End Enum
#End Region

End Class


Public Class CampixParticelle_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Scrivi(
                            ByVal PIVA As String,
                            ByVal Sa_Cod As Int32,
                            ByVal Campo_Cod As Int32,
                            ByVal PROV As String,
                            ByVal COM As String,
                            ByVal SEZIONE As String,
                            ByVal FOGLIO As Int32,
                            ByVal NUMERO As Int32,
                            ByVal SUBALTERNO As String,
                            ByVal SAU_Convenz_Ettari As Decimal,
                            ByVal SAU_Convenz_Are As Int32,
                            ByVal SAU_Convenz_Centiare As Int32,
                            ByVal SAU_Convers_Ettari As Decimal,
                            ByVal SAU_Convers_Are As Int32,
                            ByVal SAU_Convers_Centiare As Int32,
                            ByVal SAU_Bio_Ettari As Decimal,
                            ByVal SAU_Bio_Are As Int32,
                            ByVal SAU_Bio_Centiare As Int32,
                            ByVal Area As Decimal,
                                ByVal Validita_Inizio As Date,
                                ByVal Validita_Fine As Date,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.CampixParticelle_W.Scrivi()"

        '====================================================================================
        'Parametri opzionali :
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.AppendLine("INSERT INTO CampixParticelle(       ")
            StrSQL.AppendLine("                    PIVA, Sa_Cod, Campo_Cod, PROV, COM, SEZIONE, ")
            StrSQL.AppendLine("                    FOGLIO, NUMERO, SUBALTERNO, Area, ")
            StrSQL.AppendLine("                    SAU_Convenz_Ettari, SAU_Convenz_Are, SAU_Convenz_Centiare, ")
            StrSQL.AppendLine("                    SAU_Convers_Ettari, SAU_Convers_Are, SAU_Convers_Centiare, ")
            StrSQL.AppendLine("                    SAU_Bio_Ettari,     SAU_Bio_Are,     SAU_Bio_Centiare, ")
            StrSQL.AppendLine("                    Inviato,            DataInvio, ")
            StrSQL.AppendLine("                    Data_Creazione,     Data_Modifica, ")
            StrSQL.AppendLine("                    UserName_Creazione, UserName_Modifica, ")
            StrSQL.AppendLine("                    Validita_Inizio,    Validita_Fine ")
            StrSQL.AppendLine("                    ) ")
            StrSQL.AppendLine("VALUES (")
            StrSQL.AppendLine("          '" & Agro_SQL_SaveText(Trim(PIVA)) & "'  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Campo_Cod) & "  ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(Trim(PROV)) & "'  ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(Trim(COM)) & "'  ")
            StrSQL.AppendLine("         ,'" & IIf(Agro_SQL_SaveText(SEZIONE, False) <> "", Agro_SQL_SaveText(LCase(SEZIONE)), 0) & "'  ")
            StrSQL.AppendLine("         , " & IIf(Agro_SQL_SaveNum(FOGLIO, False) <> 0, Agro_SQL_SaveNum(FOGLIO), 0) & "  ")
            StrSQL.AppendLine("         , " & IIf(Agro_SQL_SaveNum(NUMERO, False) <> 0, Agro_SQL_SaveNum(NUMERO), 0) & "  ")
            StrSQL.AppendLine("         ,'" & IIf(Agro_SQL_SaveText(SUBALTERNO, False) <> "", Agro_SQL_SaveText(LCase(SUBALTERNO)), 0) & "' ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Area) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(SAU_Convenz_Ettari) & "   ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(SAU_Convenz_Are) & "   ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(SAU_Convenz_Centiare) & "   ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(SAU_Convers_Ettari) & "   ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(SAU_Convers_Are) & "   ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(SAU_Convers_Centiare) & "   ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(SAU_Bio_Ettari) & "   ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(SAU_Bio_Are) & "   ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(SAU_Bio_Centiare) & "   ")
            StrSQL.AppendLine("         , 0  ")
            StrSQL.AppendLine("         , Null  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDate(Date.Now) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDate(Date.Now) & "  ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")
            StrSQL.AppendLine(")")
            '---------------------------------------------

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

    Public Function Modifica(
                                ByVal PIVA As String,
                                ByVal Sa_Cod As Int32,
                                ByVal Campo_Cod As Int32,
                                ByVal PROV As String,
                                ByVal COM As String,
                                ByVal SEZIONE As String,
                                ByVal FOGLIO As Int32,
                                ByVal NUMERO As Int32,
                                ByVal SUBALTERNO As String,
                                ByVal Area As Decimal,
                                ByVal SAU_Convenz_Ettari As Decimal,
                                ByVal SAU_Convenz_Are As Int32,
                                ByVal SAU_Convenz_Centiare As Int32,
                                ByVal SAU_Convers_Ettari As Decimal,
                                ByVal SAU_Convers_Are As Int32,
                                ByVal SAU_Convers_Centiare As Int32,
                                ByVal SAU_Bio_Ettari As Decimal,
                                ByVal SAU_Bio_Are As Int32,
                                ByVal SAU_Bio_Centiare As Int32,
                                    ByVal Validita_Inizio As Date,
                                    ByVal Validita_Fine As Date,
                                    ByVal xFiltroAggiuntivo As String,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                    ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.CampixParticelle_W.Modifica()"

        '====================================================================================
        'Parametri opzionali :
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.AppendLine("UPDATE CampixParticelle SET ")
            StrSQL.AppendLine("        Area                 =  " & Agro_SQL_SaveNum(Area))
            StrSQL.AppendLine("       ,SAU_Convenz_Ettari   =  " & Agro_SQL_SaveNum(SAU_Convenz_Ettari) & "   ")
            StrSQL.AppendLine("       ,SAU_Convenz_Are      =  " & Agro_SQL_SaveNum(SAU_Convenz_Are) & "   ")
            StrSQL.AppendLine("       ,SAU_Convenz_Centiare =  " & Agro_SQL_SaveNum(SAU_Convenz_Centiare) & "   ")
            StrSQL.AppendLine("       ,SAU_Convers_Ettari   =  " & Agro_SQL_SaveNum(SAU_Convers_Ettari) & "   ")
            StrSQL.AppendLine("       ,SAU_Convers_Are      =  " & Agro_SQL_SaveNum(SAU_Convers_Are) & "   ")
            StrSQL.AppendLine("       ,SAU_Convers_Centiare =  " & Agro_SQL_SaveNum(SAU_Convers_Centiare) & "   ")
            StrSQL.AppendLine("       ,SAU_Bio_Ettari       =  " & Agro_SQL_SaveNum(SAU_Bio_Ettari) & "   ")
            StrSQL.AppendLine("       ,SAU_Bio_Are          =  " & Agro_SQL_SaveNum(SAU_Bio_Are) & "   ")
            StrSQL.AppendLine("       ,SAU_Bio_Centiare     =  " & Agro_SQL_SaveNum(SAU_Bio_Centiare) & "   ")
            StrSQL.AppendLine("       ,Inviato              =  0 ")
            StrSQL.AppendLine("       ,DataInvio            =  Null ")
            StrSQL.AppendLine("       ,Data_Modifica        =  " & Agro_SQL_SaveDate(Date.Now))
            StrSQL.AppendLine("       ,UserName_Modifica    = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.AppendLine("       ,Validita_Inizio      =  " & Agro_SQL_SaveDate(Validita_Inizio))
            StrSQL.AppendLine("       ,Validita_Fine        =  " & Agro_SQL_SaveDate(Validita_Fine))
            StrSQL.AppendLine(" WHERE    PIVA        = '" & Agro_SQL_SaveText(Trim(PIVA)) & "' ")
            StrSQL.AppendLine(" AND      Sa_Cod      =  " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            StrSQL.AppendLine(" AND      Campo_Cod   =  " & Agro_SQL_SaveNum(Campo_Cod) & "  ")
            StrSQL.AppendLine(" AND      PROV        = '" & Agro_SQL_SaveText(Trim(PROV)) & "' ")
            StrSQL.AppendLine(" AND      COM         = '" & Agro_SQL_SaveText(Trim(COM)) & "' ")
            StrSQL.AppendLine(" AND      Sezione     = '" & IIf(Agro_SQL_SaveText(SEZIONE, False) <> "", Agro_SQL_SaveText(LCase(SEZIONE)), 0) & "'  ")
            StrSQL.AppendLine(" AND      FOGLIO      = " & IIf(Agro_SQL_SaveNum(FOGLIO, False) <> 0, Agro_SQL_SaveNum(FOGLIO), 0) & "  ")
            StrSQL.AppendLine(" AND      Numero      = " & IIf(Agro_SQL_SaveNum(NUMERO, False) <> 0, Agro_SQL_SaveNum(NUMERO), 0) & "  ")
            StrSQL.AppendLine(" AND      SUBALTERNO =  '" & IIf(Agro_SQL_SaveText(SUBALTERNO, False) <> "", Agro_SQL_SaveText(LCase(SUBALTERNO)), 0) & "' ")

            '----------------------------------------------------------------------
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

    '##############################################################################################
    Public Function Cancella_Associazione(
                                ByVal PIVA As String,
                                ByVal Sa_Cod As Long,
                                ByVal Campo_Cod As Long,
                                ByVal PROV As String,
                                ByVal COM As String,
                                ByVal SEZIONE As String,
                                ByVal FOGLIO As Long,
                                ByVal NUMERO As Long,
                                ByVal SUBALTERNO As String,
                                    ByVal xFiltroAggiuntivo As String,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                    ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.CampixParticelle_W.Cancella_Associazione()"

        '====================================================================================
        'Parametri opzionali :
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = AgronicaCoreDataProvider.AgronicaCoreParametri.enumCancellazioneLogica.CancellazioneLogica Then

                StrSQL.Length = 0
                StrSQL.Append(" UPDATE   CampixParticelle ")
                StrSQL.Append(" SET ")
                StrSQL.Append("          Validita_Fine = " & Agro_SQL_SaveDate(CDate("31/12/1899")) & " ")
                StrSQL.Append("         ,Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("         ,Inviato = -1 ")
                StrSQL.Append(" WHERE  PIVA <> '0' ")

            Else

                StrSQL.Length = 0
                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM     CampixParticelle ")
                StrSQL.Append(" WHERE  PIVA <> '0' ")

            End If


            If PIVA <> "" Then
                StrSQL.Append(" AND  Piva = '" & Agro_SQL_SaveText(PIVA) & "' ")
            End If

            If Sa_Cod <> 0 Then
                StrSQL.Append(" AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod))
            End If

            If Campo_Cod <> 0 Then
                StrSQL.Append(" AND Campo_Cod = " & Agro_SQL_SaveNum(Campo_Cod))
            End If

            If PROV <> "" Then
                StrSQL.Append(" AND   Prov = '" & Agro_SQL_SaveText(PROV) & "' ")
            End If
            If COM <> "" Then
                StrSQL.Append(" AND   Com = '" & Agro_SQL_SaveText(COM) & "' ")
            End If

            If SEZIONE <> "" Then
                StrSQL.Append(" AND   Sezione = '" & Agro_SQL_SaveText(LCase(SEZIONE)) & "' ")
            End If
            If FOGLIO <> 0 Then
                StrSQL.Append(" AND   Foglio = " & Agro_SQL_SaveNum(FOGLIO) & " ")
            End If
            If NUMERO <> 0 Then
                StrSQL.Append(" AND   Numero = " & Agro_SQL_SaveNum(NUMERO) & " ")
            End If
            If SUBALTERNO <> "" Then
                StrSQL.Append(" AND   Subalterno = '" & Agro_SQL_SaveText(LCase(SUBALTERNO)) & "' ")
            End If


            '----------------------------------------------------------------------
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

    Public Function Cancella(
                            ByVal PIVA As String,
                            ByVal Sa_Cod As Int32,
                            ByVal Campo_Cod As Int32,
                            ByVal PROV As String,
                            ByVal COM As String,
                            ByVal SEZIONE As String,
                            ByVal FOGLIO As Int32,
                            ByVal NUMERO As Int32,
                            ByVal SUBALTERNO As String,
                                ByVal xFiltroAggiuntivo As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.CampixParticelle_W.Cancella()"

        '====================================================================================
        'Parametri opzionali :
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try


            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = AgronicaCoreDataProvider.AgronicaCoreParametri.enumCancellazioneLogica.CancellazioneLogica Then

                StrSQL.Length = 0
                StrSQL.Append(" UPDATE CampixParticelle ")
                StrSQL.Append(" SET ")
                StrSQL.Append("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("      ,Inviato = -1 ")
                StrSQL.Append(" WHERE  PIVA <> '0' ")
                StrSQL.Append(" AND Inviato >= 0")

                If PIVA <> "" Then
                    StrSQL.Append(" AND  Piva = '" & Agro_SQL_SaveText(PIVA) & "' ")
                End If

                If Sa_Cod <> 0 Then
                    StrSQL.Append(" AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod))
                End If

                If Campo_Cod <> 0 Then
                    StrSQL.Append(" AND Campo_Cod = " & Agro_SQL_SaveNum(Campo_Cod))
                End If

                If PROV <> "" Then
                    StrSQL.Append(" AND   Prov = '" & Agro_SQL_SaveText(PROV) & "' ")
                End If
                If COM <> "" Then
                    StrSQL.Append(" AND   Com = '" & Agro_SQL_SaveText(COM) & "' ")
                End If
                If SEZIONE <> "" Then
                    StrSQL.Append(" AND   Sezione = '" & Agro_SQL_SaveText(LCase(SEZIONE)) & "' ")
                End If
                If FOGLIO <> 0 Then
                    StrSQL.Append(" AND   Foglio = " & Agro_SQL_SaveNum(FOGLIO) & " ")
                End If
                If NUMERO <> 0 Then
                    StrSQL.Append(" AND   Numero = " & Agro_SQL_SaveNum(NUMERO) & " ")
                End If
                If SUBALTERNO <> "" Then
                    StrSQL.Append(" AND   Subalterno = '" & Agro_SQL_SaveText(LCase(SUBALTERNO)) & "' ")
                End If

            Else

                StrSQL.Length = 0
                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM     CampixParticelle ")
                StrSQL.Append(" WHERE  PIVA <> '0' ")

                If PIVA <> "" Then
                    StrSQL.Append(" AND  Piva = '" & Agro_SQL_SaveText(PIVA) & "' ")
                End If

                If Sa_Cod <> 0 Then
                    StrSQL.Append(" AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod))
                End If

                If Campo_Cod <> 0 Then
                    StrSQL.Append(" AND Campo_Cod = " & Agro_SQL_SaveNum(Campo_Cod))
                End If

                If PROV <> "" Then
                    StrSQL.Append(" AND   Prov = '" & Agro_SQL_SaveText(PROV) & "' ")
                End If
                If COM <> "" Then
                    StrSQL.Append(" AND   Com = '" & Agro_SQL_SaveText(COM) & "' ")
                End If
                If SEZIONE <> "" Then
                    StrSQL.Append(" AND   Sezione = '" & Agro_SQL_SaveText(LCase(SEZIONE)) & "' ")
                End If
                If FOGLIO <> 0 Then
                    StrSQL.Append(" AND   Foglio = " & Agro_SQL_SaveNum(FOGLIO) & " ")
                End If
                If NUMERO <> 0 Then
                    StrSQL.Append(" AND   Numero = " & Agro_SQL_SaveNum(NUMERO) & " ")
                End If
                If SUBALTERNO <> "" Then
                    StrSQL.Append(" AND   Subalterno = '" & Agro_SQL_SaveText(LCase(SUBALTERNO)) & "' ")
                End If


            End If


            '----------------------------------------------------------------------
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

    Public Function AggiornaValiditaInizio(
                           ByVal Piva As String,
                           ByVal Sa_Cod As Int32,
                           ByVal Campo_Cod As Int32,
                           ByVal PROV As String,
                           ByVal COM As String,
                           ByVal SEZIONE As String,
                           ByVal FOGLIO As Int32,
                           ByVal NUMERO As Int32,
                           ByVal SUBALTERNO As String,
                                ByVal Validita_Inizio As Date,
                                    ByVal xFiltroAggiuntivo As String,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                    ) As Boolean

        Dim NomeRoutine As String = "AnagrafeCoreAnagrafeDAL.CampixParticelle_W.AggiornaValiditaInizio()"

        '====================================================================================
        'Parametri opzionali :
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append("UPDATE CampixParticelle SET ")
            StrSQL.Append("   UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.Append("   ,Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Inizio))
            StrSQL.Append(" WHERE   Validita_Inizio < " & Agro_SQL_SaveDate(Validita_Inizio))

            If Piva <> "" Then
                StrSQL.Append(" AND Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If

            If Sa_Cod <> 0 Then
                StrSQL.Append(" AND   Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            End If

            If Campo_Cod <> 0 Then
                StrSQL.Append(" AND Campo_Cod = " & Agro_SQL_SaveNum(Campo_Cod))
            End If

            If PROV <> "" Then
                StrSQL.Append(" AND   Prov = '" & Agro_SQL_SaveText(PROV) & "' ")
            End If
            If COM <> "" Then
                StrSQL.Append(" AND   Com = '" & Agro_SQL_SaveText(COM) & "' ")
            End If
            If SEZIONE <> "" Then
                StrSQL.Append(" AND   Sezione = '" & Agro_SQL_SaveText(LCase(SEZIONE)) & "' ")
            End If
            If FOGLIO <> 0 Then
                StrSQL.Append(" AND   Foglio = " & Agro_SQL_SaveNum(FOGLIO) & " ")
            End If
            If NUMERO <> 0 Then
                StrSQL.Append(" AND   Numero = " & Agro_SQL_SaveNum(NUMERO) & " ")
            End If
            If SUBALTERNO <> "" Then
                StrSQL.Append(" AND   Subalterno = '" & Agro_SQL_SaveText(LCase(SUBALTERNO)) & "' ")
            End If
            '---------------------------------------------


            '----------------------------------------------------------------------
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

    '##############################################################################################
    Public Function AggiornaValidita(
                                ByVal PIVA As String,
                                ByVal Sa_Cod As Long,
                                ByVal Campo_Cod As Long,
                                    ByVal Validita_Inizio As Date,
                                    ByVal Validita_Fine As Date,
                                        ByVal xFiltroAggiuntivo As String,
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                        ) As Boolean

        Dim NomeRoutine As String = "AnagrafeCoreAnagrafeDAL.CampixParticelle_W.AggiornaValiditaFine()"

        '====================================================================================
        'Parametri opzionali :
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append("UPDATE CampixParticelle SET ")
            StrSQL.Append("   UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.Append("   ,Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Inizio))
            StrSQL.Append("   ,Validita_Fine     =  " & Agro_SQL_SaveDate(Validita_Fine))
            StrSQL.Append(" WHERE    PIVA        <> '0'")

            If PIVA <> "" Then
                StrSQL.Append(" AND Piva = '" & Agro_SQL_SaveText(PIVA) & "' ")
            End If

            If Sa_Cod <> 0 Then
                StrSQL.Append(" AND   Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            End If

            If Campo_Cod <> 0 Then
                StrSQL.Append(" AND Campo_Cod = " & Agro_SQL_SaveNum(Campo_Cod))
            End If


            '----------------------------------------------------------------------
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

    '##############################################################################################
    Public Function AggiornaValiditaFine(
                            ByVal Piva As String,
                            ByVal Sa_Cod As Int32,
                            ByVal Campo_Cod As Int32,
                            ByVal PROV As String,
                            ByVal COM As String,
                            ByVal SEZIONE As String,
                            ByVal FOGLIO As Int32,
                            ByVal NUMERO As Int32,
                            ByVal SUBALTERNO As String,
                                ByVal Validita_Fine As Date,
                                    ByVal xFiltroAggiuntivo As String,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                    ) As Boolean

        Dim NomeRoutine As String = "AnagrafeCoreAnagrafeDAL.CampixParticelle_W.AggiornaValiditaFine()"

        '====================================================================================
        'Parametri opzionali :
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append("UPDATE CampixParticelle SET ")
            StrSQL.Append("   UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.Append("   ,Validita_Fine   =  " & Agro_SQL_SaveDate(Validita_Fine))
            StrSQL.Append(" WHERE   Validita_Fine > " & Agro_SQL_SaveDate(Validita_Fine))

            If Piva <> "" Then
                StrSQL.Append(" AND Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If

            If Sa_Cod <> 0 Then
                StrSQL.Append(" AND   Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            End If

            If Campo_Cod <> 0 Then
                StrSQL.Append(" AND Campo_Cod = " & Agro_SQL_SaveNum(Campo_Cod))
            End If

            If PROV <> "" Then
                StrSQL.Append(" AND   Prov = '" & Agro_SQL_SaveText(PROV) & "' ")
            End If
            If COM <> "" Then
                StrSQL.Append(" AND   Com = '" & Agro_SQL_SaveText(COM) & "' ")
            End If
            If SEZIONE <> "" Then
                StrSQL.Append(" AND   Sezione = '" & Agro_SQL_SaveText(LCase(SEZIONE)) & "' ")
            End If
            If FOGLIO <> 0 Then
                StrSQL.Append(" AND   Foglio = " & Agro_SQL_SaveNum(FOGLIO) & " ")
            End If
            If NUMERO <> 0 Then
                StrSQL.Append(" AND   Numero = " & Agro_SQL_SaveNum(NUMERO) & " ")
            End If
            If SUBALTERNO <> "" Then
                StrSQL.Append(" AND   Subalterno = '" & Agro_SQL_SaveText(LCase(SUBALTERNO)) & "' ")
            End If


            '----------------------------------------------------------------------
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

    '######################################################################################################
    Public Function ModificaChiave(
                                  ByVal PROV As String,
                                  ByVal COM As String,
                                  ByVal SEZIONE As String,
                                  ByVal FOGLIO As Int32,
                                  ByVal NUMERO As Int32,
                                  ByVal SUBALTERNO As String,
                                  ByVal PROV_Origine As String,
                                  ByVal COM_Origine As String,
                                  ByVal SEZIONE_Origine As String,
                                  ByVal FOGLIO_Origine As Int32,
                                  ByVal NUMERO_Origine As Int32,
                                  ByVal SUBALTERNO_Origine As String,
                                  ByVal xFiltroAggiuntivo As String,
                                  ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                  ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.CampixParticelle_W.ModificaChiave()"

        '====================================================================================
        'Parametri opzionali :
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            StrSQL.Length = 0
            StrSQL.AppendLine(" UPDATE CampiXParticelle ")
            StrSQL.AppendLine(" SET  ")
            StrSQL.AppendLine(" PROV = '" & Agro_SQL_SaveText(PROV) & "', ")
            StrSQL.AppendLine(" COM = '" & Agro_SQL_SaveText(COM) & "', ")
            StrSQL.AppendLine(" SEZIONE = '" & Agro_SQL_SaveText(SEZIONE) & "', ")
            StrSQL.AppendLine(" FOGLIO =" & FOGLIO & ", ")
            StrSQL.AppendLine(" NUMERO =" & NUMERO & ", ")
            StrSQL.AppendLine(" Subalterno = '" & Agro_SQL_SaveText(SUBALTERNO) & "',")
            StrSQL.AppendLine(" Data_Modifica =" & Agro_SQL_SaveDate(New Date) & ",")
            StrSQL.AppendLine(" Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UtenteCodFiscale) & "'")
            StrSQL.AppendLine(" WHERE PROV = '" & Agro_SQL_SaveText(PROV_Origine) & "' ")
            StrSQL.AppendLine(" AND COM = '" & Agro_SQL_SaveText(COM_Origine) & "' ")
            StrSQL.AppendLine(" AND SEZIONE ='" & Agro_SQL_SaveText(SEZIONE_Origine) & "' ")
            StrSQL.AppendLine(" AND FOGLIO =" & FOGLIO_Origine & " ")
            StrSQL.AppendLine(" AND NUMERO =" & NUMERO_Origine & " ")
            StrSQL.AppendLine(" AND Subalterno = '" & Agro_SQL_SaveText(SUBALTERNO_Origine) & "'")


            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
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

End Class
