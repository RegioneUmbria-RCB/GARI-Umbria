
Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreEntityFramework
Imports System.Transactions
Imports System.Text
Imports System.Data.Entity
Imports AgronicaCoreDataProvider.DataProviderExtensions

Public Class APP_EntrateUsciteCoordinate_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function LeggiXInvioTimbrature(
        ByVal isDistinct As Boolean,
        ByVal xFiltroAggiuntivo As String,
        ByVal xOrderBy As String,
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
    ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCore_DAL.Leggi()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            stb.Length = 0

            If isDistinct Then
                stb.AppendLine(" select distinct nrBadge ")
            Else
                stb.AppendLine(" select * ")
            End If


            stb.AppendLine(" from [dbo].[APP_LogEventi] ")
            stb.AppendLine(" where importato_Data is null ")

            If xFiltroAggiuntivo <> "" Then
                stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    stb.Append(" AND   Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    stb.Append(" AND   Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                stb.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
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

    '##############################################################################################
    Public Function LeggiXPuntiGPS(
        ByVal joinUtenteGiasContattiRubrica As Boolean,
        ByVal DataInizio As Date,
        ByVal DataFine As Date,
        ByVal xFiltroAggiuntivo As String,
        ByVal xOrderBy As String,
        ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
        ByRef objParametri_utenti As AgronicaCoreDataProvider.AgronicaCoreParametri
    ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCore_DAL.Leggi()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try


            Dim NomeDB_Utenti As String =
                objParametri_utenti.StringaConnessione.Split(";")(2).Split("=")(1)

            stb.Length = 0

            stb.AppendLine("    select  ")
            stb.AppendLine("      ")
            LatitudineLongitudineDaKWT_QRY("g.gisTxt", "Latitudine", "Longitudine", False, stb)
            stb.AppendLine(" , IDTransazione as id ")
            stb.AppendLine(" , identif_dispositivo as N_telefono ")
            stb.AppendLine(" , isNull( g.Nome, '')   + ' ' + isNull( g.Cognome, '') as Descrizione ")
            stb.AppendLine(" , DataOraRilevata as DataOra ")
            stb.AppendLine(" , 0 as velocita ")
            stb.AppendLine(" , 0 as quota ")
            stb.AppendLine(" , 0 as direzione")

            stb.AppendLine(" from app_gis g  ")

            If joinUtenteGiasContattiRubrica Then


                stb.AppendLine("  inner join " & NomeDB_Utenti & ".dbo.utenti_Dettagli d ")
                stb.AppendLine("      on d.codfisc = g.Username_Creazione")

            End If

            stb.AppendLine(" where [GisTxt] like '%point%' ")
            stb.AppendLine(" And cast( g.DataOraRilevata as date) >= " & Agro_SQL_SaveDate(DataInizio))
            stb.AppendLine(" And cast( g.DataOraRilevata as date) <=  " & Agro_SQL_SaveDate(DataFine))

            If joinUtenteGiasContattiRubrica Then
                stb.AppendLine(" and d.username = '" & objParametri_Server.UtenteUsername & "' ")
            End If

            If xFiltroAggiuntivo <> "" Then
                stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri_Server))
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri_Server.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    stb.Append(" AND   g.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    stb.Append(" AND   g.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                stb.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri_Server))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri_Server, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function


    Public Function LeggiXPuntiGpsUltimaPosizione(
        ByVal joinUtenteGiasContattiRubrica As Boolean,
        ByVal DataInizio As Date,
        ByVal DataFine As Date,
        ByVal xFiltroAggiuntivo As String,
        ByVal xOrderBy As String,
        ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
        ByRef objParametri_utenti As AgronicaCoreDataProvider.AgronicaCoreParametri
    ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCore_DAL.Leggi()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try


            Dim NomeDB_Utenti As String =
                objParametri_utenti.StringaConnessione.Split(";")(2).Split("=")(1)

            stb.Length = 0

            stb.AppendLine("  select                      ")
            stb.AppendLine("  trim(  ")
            stb.AppendLine("  SUBSTRING (trim(replace(replace(g.gisTxt, 'POINT (', ''), ')', '') ) ,   ")
            stb.AppendLine("  charindex(' ' ,  ")
            stb.AppendLine("      trim(replace(replace(g.gisTxt, 'POINT (', ''), ')', '') )  ")
            stb.AppendLine("      , 0 ),  ")
            stb.AppendLine("      len(g.gisTxt)  ")
            stb.AppendLine("      )  ")
            stb.AppendLine("  ) as latitudine  ")
            stb.AppendLine("  , substring(  ")
            stb.AppendLine("      trim( replace(replace(g.gisTxt, 'POINT (', ''), ')', '') )  ")
            stb.AppendLine("      ,0, len(trim( replace(replace(g.gisTxt, 'POINT (', ''), ')', '') )) - charindex(' ' ,  ")
            stb.AppendLine("      trim(replace(replace(g.gisTxt, 'POINT (', ''), ')', '') )  ")
            stb.AppendLine("      , 0 )  ")
            stb.AppendLine("      )  as longitudine       ")
            stb.AppendLine("  , IDTransazione as id  ")
            stb.AppendLine("  , g.identif_dispositivo as N_telefono  ")
            stb.AppendLine("  , isNull( g.Nome, '')   + ' ' + isNull( g.Cognome, '') as Descrizione  ")
            stb.AppendLine("  , DataOraRilevata as DataOra  ")
            stb.AppendLine("  , 0 as velocita  ")
            stb.AppendLine("  , 0 as quota  ")
            stb.AppendLine("  , 0 as direzione ")
            stb.AppendLine(" from ( ")
            stb.AppendLine("  select max(DataOraRilevata) as  maxDataOraRilevata ")
            stb.AppendLine("  , identif_dispositivo ")
            stb.AppendLine("  , username_creazione ")
            stb.AppendLine("  from app_gis g ")

            ' VAnni: 6/7/2022: bug fix su Ultima posizione in "tecnici in campo", non vengono lette le posizioni dei tecnici; (Segnalazione di Lulli, Terre dell'etruria), il filtro per date va spostato nella query interna
            stb.AppendLine(" where g.[GisTxt] like '%point%' ")
            stb.AppendLine(" And cast( g.DataOraRilevata as date) >= " & Agro_SQL_SaveDate(DataInizio))
            stb.AppendLine(" And cast( g.DataOraRilevata as date) <=  " & Agro_SQL_SaveDate(DataFine))

            stb.AppendLine("  group by  ")
            stb.AppendLine("    identif_dispositivo ")
            stb.AppendLine("  , username_creazione ")
            stb.AppendLine("  ) gMax ")
            stb.AppendLine("  inner join app_gis g ")
            stb.AppendLine("      on g.DataOraRilevata = gMax.maxDataOraRilevata ")
            stb.AppendLine("      and g.Identif_Dispositivo = gMax.Identif_Dispositivo ")
            stb.AppendLine("      and g.Username_Creazione = gMax.Username_Creazione")


            If joinUtenteGiasContattiRubrica Then

                stb.AppendLine("  inner join " & NomeDB_Utenti & ".dbo.utenti_Dettagli d ")
                stb.AppendLine("      on d.codfisc = g.Username_Creazione")

            End If


            If joinUtenteGiasContattiRubrica Then
                stb.AppendLine(" and d.username = '" & objParametri_Server.UtenteUsername & "' ")
            End If

            If xFiltroAggiuntivo <> "" Then
                stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri_Server))
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri_Server.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    stb.Append(" AND   g.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    stb.Append(" AND   g.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                stb.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri_Server))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri_Server, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function

End Class



Public Class APP_EntrateUsciteCoordinate_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function APP_AllineaUltimaPosizioneIMotionDaAPP(
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
    ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.Ricette_Write.Scrivi()"

        '====================================================================================
        'Parametri opzionali :
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            stb.Length = 0

            stb.AppendLine("update v ")
            stb.AppendLine("  set   ")
            stb.AppendLine("       ultimaposizioneTimeStamp = datediff(s, '01/01/1970', g.DataOraRilevata)     ")
            stb.AppendLine("  , latitudine =  ")
            stb.AppendLine("   trim( ")
            stb.AppendLine("  SUBSTRING (trim(replace(replace(g.gisTxt, 'POINT (', ''), ')', '') ) ,  ")
            stb.AppendLine("    charindex(' ' , ")
            stb.AppendLine("        trim(replace(replace(g.gisTxt, 'POINT (', ''), ')', '') ) ")
            stb.AppendLine("        , 0 ), ")
            stb.AppendLine("      len(g.gisTxt) ")
            stb.AppendLine("      ) ")
            stb.AppendLine("  )  ")
            stb.AppendLine("  , longitudine = substring( ")
            stb.AppendLine("      trim( replace(replace(g.gisTxt, 'POINT (', ''), ')', '') ) ")
            stb.AppendLine("      ,0, len(trim( replace(replace(g.gisTxt, 'POINT (', ''), ')', '') )) - charindex(' ' , ")
            stb.AppendLine("        trim(replace(replace(g.gisTxt, 'POINT (', ''), ')', '') ) ")
            stb.AppendLine("        , 0 ) ")
            stb.AppendLine("      )        ")

            stb.AppendLine("      , data_modifica = getdate()        ")

            stb.AppendLine(" from imotion_veicoli v ")
            stb.AppendLine("  inner join (  ")
            stb.AppendLine("      select max( DataOraRilevata ) as DataOraRilevata, identif_Dispositivo ")
            stb.AppendLine("      from app_gis g  ")
            stb.AppendLine("      where [GisTxt] like '%point%' ")
            stb.AppendLine("      group by identif_Dispositivo             ")
            stb.AppendLine("  )   g1 ")
            stb.AppendLine("  on v.plate = g1.identif_Dispositivo  ")
            stb.AppendLine("  inner join app_gis g  ")
            stb.AppendLine("      on g1.identif_Dispositivo = g.identif_Dispositivo ")
            stb.AppendLine("      and g1.DataOraRilevata = g.DataOraRilevata ")
            stb.AppendLine("        ")
            stb.AppendLine(" where dateadd(s, ultimaposizioneTimestamp, '01/01/1970'  ) < g.DataOraRilevata ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp

    End Function

    Public Function APP_AggiornaTimbratura(
        ByVal id As String,
        ByVal importatoData As Date,
        ByVal importatoErrore As String,
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
    ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.Ricette_Write.Scrivi()"

        '====================================================================================
        'Parametri opzionali :
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            stb.Length = 0

            stb.AppendLine(" update [dbo].[APP_LogEventi] ")
            stb.AppendLine(" set data_modifica = getdate() ")

            If importatoData <> AGRODATAINIZIO Then
                stb.AppendLine(" , importato_data =  " & Agro_SQL_SaveDateTime(importatoData))
            End If

            If importatoErrore <> "" Then
                stb.AppendLine(" , Importato_Errore = '" & Agro_SQL_SaveText(importatoErrore) & "'  ")
            End If
            stb.AppendLine(" where id = '" & Agro_SQL_SaveText(id) & "'")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp

    End Function
    Public Function APP_Scrivi_EntrateUsciteCoordinate(
                ByVal EFArrayToInsert As ArrayList,
                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                ) As String

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.AgronicaCoreMetaschemaDAL.Aggiorna_EntrateUsciteCoordinatePerAPP()"
        Dim MessaggioErrore As String = ""

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)


        Try

            Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

                For Each curOggetto In EFArrayToInsert

                    GiasContext.Entry(curOggetto).State = EntityState.Added
                    'GiasContext.AddObject(curOggetto.GetType.ToString.Replace("AgronicaCoreEntityFramework_POCO.", ""), curOggetto)

                Next

                GiasContext.SaveChanges()

            End Using

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore, ex)

        End Try

        Return MessaggioErrore

    End Function

End Class