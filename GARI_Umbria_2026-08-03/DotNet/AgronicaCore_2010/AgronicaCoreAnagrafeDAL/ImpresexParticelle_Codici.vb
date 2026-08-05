Imports System.Data.Common
Imports AgronicaCoreDataProvider.UtilityProvider


Public Class ImpresexParticelle_Codici_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi(ByVal ID As Integer,
                          ByVal PIVA As String,
                          ByVal Sa_Cod As Int32,
                          ByVal PROV As String,
                          ByVal COM As String,
                          ByVal SEZIONE As String,
                          ByVal FOGLIO As Int32,
                          ByVal NUMERO As Int32,
                          ByVal SUBALTERNO As String,
                          ByVal Id_Cod As Int32,
                          ByVal Val_Cod As String,
                                ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                Optional ByVal LeggiValoreEsatto_Val_Cod_SenzaPercentuale As Boolean = False
                                ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.ImpresexParticelle_Codici_R.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""                   =>  si leggono tutte le imprese
        '   Sa_Cod = 0
        '   Cod_Contatto = 0
        '   Id_Cod = 0
        '   Val_Cod = ""
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            '---------------------------------------------
            'Select Case xSelezioneVariabile

            '    Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi

            'TODO
            'modificare la query di select
            stb.Length = 0

            stb.Append(" SELECT  IPC.*, IP.* ")
            stb.Append(" FROM    ImpresexParticelle_Codici IPC ")

            stb.Append(" INNER JOIN ImpreseXParticelle IP ON IP.ID = IPC.ID ")


            stb.Append(" WHERE   (IPC.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & ")  ")
            stb.Append(" AND     (IPC.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & ") ")

            If ID <> 0 Then
                stb.Append(" AND IPC.ID = " & Agro_SQL_SaveNum(ID) & " ")
            End If

            If PIVA <> "" Then
                stb.Append(" AND IP.Piva = '" & Agro_SQL_SaveText(Trim(PIVA)) & "' ")
            End If

            If Sa_Cod <> 0 Then
                stb.Append(" AND IP.Sa_Cod = " & Agro_SQL_SaveNum(Trim(Sa_Cod)))
            End If

            If PROV <> "" Then
                stb.Append(" AND IP.PROV = '" & Agro_SQL_SaveText(Trim(PROV)) & "' ")
            End If

            If COM <> "" Then
                stb.Append(" AND IP.COM = '" & Agro_SQL_SaveText(Trim(COM)) & "' ")
            End If

            If SEZIONE <> "" Then
                stb.Append(" AND IP.SEZIONE = '" & Agro_SQL_SaveText(LCase(SEZIONE)) & "' ")
            End If

            If FOGLIO <> 0 Then
                stb.Append(" AND IP.FOGLIO =  " & Agro_SQL_SaveNum(FOGLIO) & " ")
            End If

            If NUMERO <> 0 Then
                stb.Append(" AND IP.NUMERO =  " & Agro_SQL_SaveNum(NUMERO) & " ")
            End If

            If SUBALTERNO <> "" Then
                stb.Append(" AND IP.SUBALTERNO = '" & Agro_SQL_SaveText(Trim(LCase(SUBALTERNO))) & "' ")
            End If

            If Id_Cod <> 0 Then
                stb.Append(" AND (IPC.id_cod = " & Agro_SQL_SaveNum(Id_Cod) & ")  ")
            End If

            If LeggiValoreEsatto_Val_Cod_SenzaPercentuale Then
                'inserito per proplemi in sincro apofruit dato che mi trovava non un solo
                'valore ma anche i simili es 002174|1|1|2 , 002174|1|1|2XX

                stb.Append(" AND (IPC.val_cod = '" & Agro_SQL_SaveText(Val_Cod) & "')  ")

            Else
                If Trim(Val_Cod) <> "" Then
                    stb.Append(" AND (IPC.val_cod like '%" & Agro_SQL_SaveText(Val_Cod) & "%')  ")
                End If

            End If

            If xFiltroAggiuntivo <> "" Then
                stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    stb.Append(" AND   IP.Inviato >=0 ")
                    stb.Append(" AND   IPC.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    stb.Append(" AND   IP.Inviato =-1 ")
                    stb.Append(" AND   IPC.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                stb.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                stb.Append(" ORDER BY IPC.Validita_Fine ASC")
            End If




            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function

    Public Function LeggiSenzaJoin(ByVal ID As Integer,
                              ByVal Id_Cod As Int32,
                              ByVal Val_Cod As String,
                             ByVal xFiltroAggiuntivo As String,
                            ByVal xOrderBy As String,
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                            ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.ImpresexParticelle_Codici_R.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""                   =>  si leggono tutte le imprese
        '   Sa_Cod = 0
        '   Cod_Contatto = 0
        '   Id_Cod = 0
        '   Val_Cod = ""
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            '---------------------------------------------
            'Select Case xSelezioneVariabile

            '    Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi

            'TODO
            'modificare la query di select
            StrSQL.Length = 0

            StrSQL.Append(" SELECT  IPC.* ")
            StrSQL.Append(" FROM    ImpresexParticelle_Codici IPC ")

            StrSQL.Append(" WHERE   (IPC.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & ")  ")
            StrSQL.Append(" AND     (IPC.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & ") ")

            If ID <> 0 Then
                StrSQL.Append(" AND IPC.ID = " & Agro_SQL_SaveText(ID) & " ")
            End If

            If Id_Cod <> 0 Then
                StrSQL.Append(" AND (IPC.id_cod = " & Agro_SQL_SaveNum(Id_Cod) & ")  ")
            End If

            If Trim(Val_Cod) <> "" Then
                StrSQL.Append(" AND (IPC.val_cod like '%" & Agro_SQL_SaveText(Val_Cod) & "%')  ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   IPC.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   IPC.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY IPC.Validita_Fine ASC")
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

    Public Function LeggiConDescrizioni(
        ByVal Gis_Entita_Cod As Integer,
        ByVal ID As Integer,
        ByVal piva As String,
        ByVal sa_Cod As Integer,
        ByVal PROV As String,
        ByVal COM As String,
        ByVal SEZIONE As String,
        ByVal FOGLIO As Int32,
        ByVal NUMERO As Int32,
        ByVal SUBALTERNO As String,
        ByVal Id_Cod As Int32,
        ByVal Val_Cod As String,
        ByVal xFiltroAggiuntivo As String,
        ByVal xOrderBy As String,
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
        Optional ByVal LeggiValoreEsatto_Val_Cod_SenzaPercentuale As Boolean = False
    ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.ImpresexParticelle_Codici_R.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""                   =>  si leggono tutte le imprese
        '   Sa_Cod = 0
        '   Cod_Contatto = 0
        '   Id_Cod = 0
        '   Val_Cod = ""
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            '---------------------------------------------
            'Select Case xSelezioneVariabile

            '    Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi

            'TODO
            'modificare la query di select
            stb.Length = 0

            stb.AppendLine("   Select ")
            stb.AppendLine("    i.rag_soc ")
            stb.AppendLine("  , sa.sa_nome ")
            stb.AppendLine("  , sa.piva ")
            stb.AppendLine("  , sa.sa_cod ")
            stb.AppendLine("  , ipart.PROV ")
            stb.AppendLine("  , ipart.com ")
            stb.AppendLine("  , ipart.SEZIONE ")
            stb.AppendLine("  , ipart.FOGLIO ")
            stb.AppendLine("  , ipart.NUMERO ")
            stb.AppendLine("  , ipart.SUBALTERNO ")
            stb.AppendLine("  , ipart.TitoloPossesso ")
            stb.AppendLine("  , ipart.validita_inizio ")
            stb.AppendLine("  , ipart.validita_fine ")
            stb.AppendLine("  , ipart.sup_condotta ")
            stb.AppendLine("  , isnull(IPC.val_cod, '') as CodiceParticella ")


            stb.AppendLine("   , '{' +  ")
            stb.AppendLine("         ISTAT_Comuni.Provincia  + ' : ' + ")
            stb.AppendLine("         ISTAT_Comuni.Cod_Belfiore + ' : ' + ")
            stb.AppendLine("         ISTAT_Comuni.Descrizione  + ' : ' +  ")
            stb.AppendLine("         right('___' + cast(ipart.Sezione as Varchar(3)), 3) + ' : '   + ")
            stb.AppendLine("         right('______' + cast(ipart.Foglio as Varchar(6)), 6) + ' : ' + ")
            stb.AppendLine("         right('______' + cast(ipart.Numero as Varchar(6)), 6) + ' : ' + ")
            stb.AppendLine("         right('__' + cast(ipart.Subalterno as Varchar(2)), 2) + ")
            stb.AppendLine("   '}' as DescrizioneParticella ")

            stb.AppendLine("  ")
            stb.AppendLine("  From ImpreseXParticelle ipart   ")
            stb.AppendLine("  inner Join Centri_Aziendali sa ")
            stb.AppendLine("         On sa.PIVA = ipart.piva ")
            stb.AppendLine("      And sa.sa_cod = ipart.sa_cod ")
            stb.AppendLine("  inner Join imprese i ")
            stb.AppendLine("         On i.piva = sa.piva  ")

            stb.AppendLine("  INNER Join ISTAT_Comuni  ")
            stb.AppendLine("     On ISTAT_Comuni.Pro_Cod_Istat = ipart.PROV  ")
            stb.AppendLine("  And ISTAT_Comuni.Com_Cod_Istat = ipart.COM ")
            stb.AppendLine(" ")

            If Gis_Entita_Cod <> 0 Then

                stb.AppendLine("  inner Join GIS_Entita e ")
                stb.AppendLine("         On e.PROV = ipart.PROV ")
                stb.AppendLine("      And e.com = ipart.COM ")
                stb.AppendLine("      And e.SEZIONE =ipart.SEZIONE ")
                stb.AppendLine("      And e.FOGLIO = ipart.FOGLIO      ")
                stb.AppendLine("      And e.NUMERO = ipart.NUMERO ")
                stb.AppendLine("      And e.SUBALTERNO = ipart.SUBALTERNO ")
                stb.AppendLine("  ")

            End If
            stb.AppendLine("  left Join ( select * from ImpresexParticelle_Codici IPC ")


            If Id_Cod <> 0 Then
                stb.AppendLine(" where (IPC.id_cod = " & Agro_SQL_SaveNum(Id_Cod) & ")  ")
            End If

            If LeggiValoreEsatto_Val_Cod_SenzaPercentuale Then
                'inserito per proplemi in sincro apofruit dato che mi trovava non un solo
                'valore ma anche i simili es 002174|1|1|2 , 002174|1|1|2XX

                stb.Append(" and (IPC.val_cod = '" & Agro_SQL_SaveText(Val_Cod) & "')  ")

            Else
                If Trim(Val_Cod) <> "" Then
                    stb.Append(" and (IPC.val_cod like '%" & Agro_SQL_SaveText(Val_Cod) & "%')  ")
                End If

            End If
            stb.AppendLine("  ) IPC ")
            stb.AppendLine("         On ipart.ID = IPC.ID ")


            stb.AppendLine(" WHERE   (ipart.Validita_Inizio <= " & Agro_SQL_SaveDate(Now.Date) & ")  ")
            stb.AppendLine(" AND     (ipart.Validita_Fine >= " & Agro_SQL_SaveDate(Now.Date) & ") ")


            If Gis_Entita_Cod <> 0 Then
                stb.AppendLine(" AND e.Entita_Cod = " & Agro_SQL_SaveNum(Gis_Entita_Cod) & " ")
            End If

            If ID <> 0 Then
                stb.AppendLine(" AND IPC.ID = " & Agro_SQL_SaveText(ID) & " ")
            End If

            If piva <> "" Then
                stb.Append(" AND ipart.Piva = '" & Agro_SQL_SaveText(Trim(piva)) & "' ")
            End If

            If sa_Cod <> 0 Then
                stb.Append(" AND ipart.Sa_Cod = " & Agro_SQL_SaveNum(Trim(sa_Cod)) & " ")
            End If

            If PROV <> "" Then
                stb.AppendLine(" AND ipart.PROV = '" & Agro_SQL_SaveText(Trim(PROV)) & "' ")
            End If

            If COM <> "" Then
                stb.AppendLine(" AND ipart.COM = '" & Agro_SQL_SaveText(Trim(COM)) & "' ")
            End If

            If SEZIONE <> "" Then
                stb.AppendLine(" AND ipart.SEZIONE = '" & Agro_SQL_SaveText(LCase(SEZIONE)) & "' ")
            End If

            If FOGLIO <> 0 Then
                stb.AppendLine(" AND ipart.FOGLIO =  " & Agro_SQL_SaveNum(FOGLIO) & " ")
            End If

            If NUMERO <> 0 Then
                stb.AppendLine(" AND ipart.NUMERO =  " & Agro_SQL_SaveNum(NUMERO) & " ")
            End If

            If SUBALTERNO <> "" Then
                stb.AppendLine(" AND ipart.SUBALTERNO = '" & Agro_SQL_SaveText(Trim(LCase(SUBALTERNO))) & "' ")
            End If



            If xFiltroAggiuntivo <> "" Then
                stb.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    stb.AppendLine(" AND   ipart.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    stb.AppendLine(" AND   ipart.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                stb.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                stb.AppendLine(" ORDER BY IPC.Validita_Fine ASC")
            End If


            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function


    '##############################################################
    Public Function RecuperaValCod_from_DTeCod(ByVal DtConJoin As DataTable,
                                                 ByVal PIVA As String,
                                                  ByVal Sa_Cod As Int32,
                                                  ByVal PROV As String,
                                                  ByVal COM As String,
                                                  ByVal SEZIONE As String,
                                                  ByVal FOGLIO As Int32,
                                                  ByVal NUMERO As Int32,
                                                  ByVal SUBALTERNO As String,
                                                  ByVal Id_Cod As Int32
                                                 ) As String

        Dim des As String = ""

        If Not IsNothing(DtConJoin) AndAlso DtConJoin.Rows.Count > 0 Then

            Dim dr() As DataRow

            dr = DtConJoin.Select(" piva = '" & Agro_SQL_SaveText(PIVA, False) & "' " &
                                " AND sa_cod = " & Agro_SQL_SaveNum(Sa_Cod, False) &
                                " AND prov = '" & Agro_SQL_SaveText(PROV, False) & "' " &
                                " AND sezione = '" & Agro_SQL_SaveText(SEZIONE, False) & "' " &
                                " AND foglio = " & Agro_SQL_SaveNum(FOGLIO, False) &
                                " AND numero = " & Agro_SQL_SaveNum(NUMERO, False) &
                                " AND subalterno = '" & Agro_SQL_SaveText(SUBALTERNO, False) & "' " &
                                " AND id_cod = " & Agro_SQL_SaveNum(Id_Cod, False))

            If Not IsNothing(dr) AndAlso dr.Length > 0 Then
                des = dr(0).Item("val_cod")
            End If

        End If

        Return des

    End Function


End Class



'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################



Public Class ImpresexParticelle_Codici_W
    Inherits AgronicaCoreDataProvider.DataProvider



    '============================================================================
    'Public Function Scrivi(ByVal ID As Int32, _
    '                       ByVal Id_Cod As Int32, _
    '                       ByVal Val_Cod As String, _
    '                       ByVal FinestraTemp_Inizio As Date, _
    '                       ByVal FinestraTemp_Fine As Date, _
    '                       ByRef objConnessione As DbConnection, _
    '                       ByRef objTransazione As DbTransaction, _
    '                       ByVal StringaConnessione As String, _
    '                       ByVal DirectoryLOG As String, _
    '                       ByVal FileLOG As String, _
    '                       ByVal IdentificatoreUtente As String _
    '                       ) As Boolean

    '    Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.ImpresexParticelle_Codici_W.Scrivi()"

    '    '====================================================================================
    '    'Parametri opzionali :
    '    '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
    '    '   DirectoryLOG = ""           =>  viene usato il valore di default
    '    '   FileLOG = ""                =>  viene usato il valore di default
    '    '====================================================================================

    '    Dim MessaggioErrore As String = ""
    '    Dim StrSQL As New System.Text.StringBuilder
    '    Dim xRisp As Boolean = False

    '    Try
    '        '---------------------------------------------
    '        StrSQL.Length = 0
    '        StrSQL.Append("INSERT INTO ImpresexParticelle_Codici(       ")
    '        StrSQL.Append("                    ID, Id_Cod, Val_Cod, ")
    '        StrSQL.Append("                    Inviato,            DataInvio, ")
    '        StrSQL.Append("                    Data_Creazione,     Data_Modifica, ")
    '        StrSQL.Append("                    UserName_Creazione, UserName_Modifica, ")
    '        StrSQL.Append("                    Validita_Inizio,    Validita_Fine ")
    '        StrSQL.Append("                    ) ")
    '        StrSQL.Append("VALUES (")
    '        StrSQL.Append("           " & Agro_SQL_SaveNum(ID) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(Id_Cod) & "  ")
    '        StrSQL.Append("         ,'" & Agro_SQL_SaveText(Val_Cod) & "' ")
    '        StrSQL.Append("         , 0  ")
    '        StrSQL.Append("         , Null  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveDate(Date.Now) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveDate(Date.Now) & "  ")
    '        StrSQL.Append("         ,'" & Agro_SQL_SaveText(IdentificatoreUtente) & "' ")
    '        StrSQL.Append("         ,'" & Agro_SQL_SaveText(IdentificatoreUtente) & "' ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveDate(FinestraTemp_Inizio) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveDate(FinestraTemp_Fine) & "  ")
    '        StrSQL.Append(")")

    '        '---------------------------------------------

    '        xRisp = EseguiQuery_Scrittura(objConnessione, objTransazione, StringaConnessione, StrSQL.ToString, DirectoryLOG, FileLOG, IdentificatoreUtente, NomeRoutine)

    '    Catch ex As Exception

    '        MessaggioErrore = ex.Message
    '        Scrivi_LOG(DirectoryLOG, FileLOG, IdentificatoreUtente, NomeRoutine, MessaggioErrore)
    '        xRisp = False
    '        Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

    '    End Try

    '    Return xRisp


    'End Function



    Public Function Scrivi(ByVal ID As Int32,
                           ByVal Id_Cod As Int32,
                           ByVal Val_Cod As String,
                            ByVal Validita_Inizio As Date,
                            ByVal Validita_Fine As Date,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.ImpresexParticelle_Codici_W.Scrivi()"

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
            StrSQL.Append("INSERT INTO ImpresexParticelle_Codici(       ")
            StrSQL.Append("                    ID, Id_Cod, Val_Cod, ")
            StrSQL.Append("                    Inviato,            DataInvio, ")
            StrSQL.Append("                    Data_Creazione,     Data_Modifica, ")
            StrSQL.Append("                    UserName_Creazione, UserName_Modifica, ")
            StrSQL.Append("                    Validita_Inizio,    Validita_Fine ")
            StrSQL.Append("                    ) ")
            StrSQL.Append("VALUES (")
            StrSQL.Append("           " & Agro_SQL_SaveNum(ID) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Id_Cod) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Val_Cod) & "' ")
            StrSQL.Append("         , 0  ")
            StrSQL.Append("         , Null  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Date.Now) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Date.Now) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(objParametri.LogDescrizioneUtente) & "' ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(objParametri.LogDescrizioneUtente) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")
            StrSQL.Append(")")

            '--------------------------------------------

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
    'Public Function Cancella( _
    '                        ByVal ID As Int32, _
    '                        ByVal Id_Cod As Int32, _
    '                        ByVal FlagCancellazioneLogica As Int32, _
    '                        ByRef objConnessione As DbConnection, _
    '                        ByRef objTransazione As DbTransaction, _
    '                        ByVal StringaConnessione As String, _
    '                        ByVal DirectoryLOG As String, _
    '                        ByVal FileLOG As String, _
    '                        ByVal IdentificatoreUtente As String _
    '                        ) As Boolean

    '    Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.ImpresexParticelle_Codici_W.Cancella()"

    '    '====================================================================================
    '    'Parametri opzionali :
    '    '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
    '    '   DirectoryLOG = ""           =>  viene usato il valore di default
    '    '   FileLOG = ""                =>  viene usato il valore di default
    '    '====================================================================================

    '    Dim MessaggioErrore As String = ""
    '    Dim StrSQL As New System.Text.StringBuilder
    '    Dim xRisp As Boolean = False

    '    Try



    '        '---------------------------------------------
    '        If FlagCancellazioneLogica Then

    '            StrSQL.Length = 0
    '            StrSQL.Append(" UPDATE ImpresexParticelle_Codici ")
    '            StrSQL.Append(" SET ")
    '            StrSQL.Append("      Username_Modifica = '" & Agro_SQL_SaveText(IdentificatoreUtente) & "' ")
    '            StrSQL.Append("      ,Inviato = -1 ")
    '            StrSQL.Append(" WHERE Inviato >= 0")
    '            StrSQL.Append(" AND ID = " & Agro_SQL_SaveNum(ID))
    '            StrSQL.Append(" AND Id_Cod = " & Agro_SQL_SaveNum(Id_Cod) & " ")


    '        Else

    '            StrSQL.Length = 0
    '            StrSQL.Append(" DELETE ")
    '            StrSQL.Append(" FROM     ImpresexParticelle_Codici ")
    '            StrSQL.Append(" WHERE ID = " & Agro_SQL_SaveNum(ID))
    '            StrSQL.Append(" AND Id_Cod = " & Agro_SQL_SaveNum(Id_Cod) & " ")


    '        End If
    '        '---------------------------------------------

    '        xRisp = EseguiQuery_Scrittura(objConnessione, objTransazione, StringaConnessione, StrSQL.ToString, DirectoryLOG, FileLOG, IdentificatoreUtente, NomeRoutine)

    '    Catch ex As Exception

    '        MessaggioErrore = ex.Message
    '        Scrivi_LOG(DirectoryLOG, FileLOG, IdentificatoreUtente, NomeRoutine, MessaggioErrore)
    '        xRisp = False
    '        Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

    '    End Try

    '    Return xRisp

    'End Function

    Public Function CancellaXValCod(
                            ByVal ID As Int32,
                            ByVal Id_Cod As Int32,
                                ByVal Val_Cod As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.ImpresexParticelle_Codici_W.Cancella()"

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

            If Val_Cod = "" Then
                Throw New Exception("  Val_Cod ='' ")
            End If

            '---------------------------------------------


            StrSQL.Length = 0
            StrSQL.Append(" DELETE ")
            StrSQL.Append(" FROM     ImpresexParticelle_Codici ")
            StrSQL.Append(" WHERE 1=1 ")
            StrSQL.Append(" and val_cod = '" & Agro_SQL_SaveText(Val_Cod) & "'  ")

            If ID <> 0 Then
                StrSQL.Append(" and ID = " & Agro_SQL_SaveNum(ID))
            End If

            If Id_Cod <> 0 Then
                StrSQL.Append(" AND Id_Cod = " & Agro_SQL_SaveNum(Id_Cod) & " ")
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
                            ByVal ID As Int32,
                            ByVal Id_Cod As Int32,
                                ByVal xFiltroAggiuntivo As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.ImpresexParticelle_Codici_W.Cancella()"

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
                StrSQL.Append(" UPDATE ImpresexParticelle_Codici ")
                StrSQL.Append(" SET ")
                StrSQL.Append("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.LogDescrizioneUtente) & "' ")
                StrSQL.Append("      ,Inviato = -1 ")
                StrSQL.Append(" WHERE Inviato >= 0")
                StrSQL.Append(" AND ID = " & Agro_SQL_SaveNum(ID))
                StrSQL.Append(" AND Id_Cod = " & Agro_SQL_SaveNum(Id_Cod) & " ")


            Else

                StrSQL.Length = 0
                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM     ImpresexParticelle_Codici ")
                StrSQL.Append(" WHERE ID = " & Agro_SQL_SaveNum(ID))
                StrSQL.Append(" AND Id_Cod = " & Agro_SQL_SaveNum(Id_Cod) & " ")


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


    Public Function Aggiungi_Aggiorna(ByVal ID As Int32,
                           ByVal Id_Cod As Int32,
                           ByVal Val_Cod As String,
                            ByVal Validita_Inizio As Date,
                            ByVal Validita_Fine As Date,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.ImpresexParticelle_Codici_W.Aggiungi_Aggiorna()"



        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            Dim ImpresexParticelle_Codici_R As New AgronicaCoreAnagrafeDAL.ImpresexParticelle_Codici_R
            Dim dtPC As DataTable = ImpresexParticelle_Codici_R.Leggi(ID, "", 0, "", "", "", 0, 0, "",
                                                                      Id_Cod, "", AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri)
            If dtPC.Rows.Count = 1 Then

                Cancella(ID, Id_Cod, "", objParametri)
                Scrivi(ID, Id_Cod, Val_Cod, Validita_Inizio, Validita_Fine, objParametri)

            ElseIf dtPC.Rows.Count = 0 Then

                Scrivi(ID, Id_Cod, Val_Cod, Validita_Inizio, Validita_Fine, objParametri)

            Else
                Throw New Exception("La query deve selezionare al massimo un solo record")
            End If

            xRisp = True

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp


    End Function


End Class
