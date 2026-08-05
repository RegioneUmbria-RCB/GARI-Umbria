Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate

Public Class ParticelleCatastali_MetodoProduzione_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi(ByVal PROV As String,
                          ByVal COM As String,
                          ByVal SEZIONE As String,
                          ByVal FOGLIO As Int32,
                          ByVal NUMERO As Int32,
                          ByVal SUBALTERNO As String,
                          ByVal MetodoProduzione_Cod As Integer,
                          ByVal Validita_Inizio As Date,
                          ByVal Validita_Fine As Date,
                          ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.ParticelleCatastali_MetodoProduzione_R.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""                   =>  si leggono tutte le imprese
        '   Sa_Cod = 0
        '   Cod_Contatto = 0
        '   Id_Cod = 0
        '   Val_Cod = ""
        '
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            '---------------------------------------------

            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta

                    'TODO
                    'modificare la query di select
                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT  ParticelleCatastali_MetodoProduzione.*,  ")

                    StrSQL.Append(" CASE MetodoProduzione_Cod ")
                    StrSQL.Append(" WHEN 1 THEN 'Integrato' ")
                    StrSQL.Append(" WHEN 2 THEN 'In Conversione' ")
                    StrSQL.Append(" WHEN 3 THEN 'Biologico'  ")
                    StrSQL.Append(" END as MetodoProduzione_Des ")

                    StrSQL.Append(" FROM    ParticelleCatastali_MetodoProduzione ")
                    StrSQL.Append(" WHERE   1=1  ")

                    If Validita_Inizio <> AGRODATAINIZIO Then
                        StrSQL.Append(" AND     (ParticelleCatastali_MetodoProduzione.Validita_Inizio <= " & Agro_SQL_SaveDate(Validita_Inizio) & ")  ")
                    End If

                    If Validita_Fine <> AGRODATAFINE Then
                        StrSQL.Append(" AND     (ParticelleCatastali_MetodoProduzione.Validita_Fine >= " & Agro_SQL_SaveDate(Validita_Fine) & ") ")
                    End If

                    If PROV <> "" Then
                        StrSQL.Append(" AND ParticelleCatastali_MetodoProduzione.PROV = '" & Agro_SQL_SaveText(Trim(PROV)) & "' ")
                    End If

                    If COM <> "" Then
                        StrSQL.Append(" AND ParticelleCatastali_MetodoProduzione.COM = '" & Agro_SQL_SaveText(Trim(COM)) & "' ")
                    End If

                    If SEZIONE <> "" Then
                        StrSQL.Append(" AND ParticelleCatastali_MetodoProduzione.SEZIONE = '" & Agro_SQL_SaveText(LCase(SEZIONE)) & "' ")
                    End If

                    If FOGLIO <> 0 Then
                        StrSQL.Append(" AND ParticelleCatastali_MetodoProduzione.FOGLIO =  " & Agro_SQL_SaveNum(FOGLIO) & " ")
                    End If

                    If NUMERO <> 0 Then
                        StrSQL.Append(" AND ParticelleCatastali_MetodoProduzione.NUMERO =  " & Agro_SQL_SaveNum(NUMERO) & " ")
                    End If

                    If SUBALTERNO <> "" Then
                        StrSQL.Append(" AND ParticelleCatastali_MetodoProduzione.SUBALTERNO = '" & Agro_SQL_SaveText(Trim(LCase(SUBALTERNO))) & "' ")
                    End If

                    If MetodoProduzione_Cod <> -1 Then
                        StrSQL.Append(" AND     (ParticelleCatastali_MetodoProduzione.MetodoProduzione_Cod = " & Agro_SQL_SaveNum(MetodoProduzione_Cod) & ")  ")
                    End If


                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   ParticelleCatastali_MetodoProduzione.Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   ParticelleCatastali_MetodoProduzione.Inviato =-1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY ParticelleCatastali_MetodoProduzione.Validita_Inizio ASC")
                    End If



                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni
                    '
                    '
                    '
                    '


                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta
                    '
                    '
                    '
                    '


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

End Class


Public Class ParticelleCatastali_MetodoProduzione_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Scrivi(ByVal PROV As String,
                           ByVal COM As String,
                           ByVal SEZIONE As String,
                           ByVal FOGLIO As Int32,
                           ByVal NUMERO As Int32,
                           ByVal SUBALTERNO As String,
                           ByVal MetodoProduzione_Cod As Integer,
                           ByVal Validita_Inizio As Date,
                           ByVal Validita_Fine As Date,
                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                               ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.ParticelleCatastali_MetodoProduzione_W.Scrivi()"

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
            StrSQL.Append("INSERT INTO ParticelleCatastali_MetodoProduzione(       ")
            StrSQL.Append("             PROV,           COM,        SEZIONE, ")
            StrSQL.Append("             FOGLIO,             NUMERO,         SUBALTERNO, ")
            StrSQL.Append("             MetodoProduzione_Cod, ")

            StrSQL.Append("             Inviato,            DataInvio, ")
            StrSQL.Append("             Data_Creazione,     Data_Modifica, ")
            StrSQL.Append("             UserName_Creazione, UserName_Modifica, ")
            StrSQL.Append("             Validita_Inizio,    Validita_Fine ")
            StrSQL.Append("             ) ")

            StrSQL.Append(" VALUES (")
            StrSQL.Append("         '" & Agro_SQL_SaveText(Trim(PROV)) & "'  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Trim(COM)) & "'  ")
            StrSQL.Append("         ,'" & IIf(Agro_SQL_SaveText(SEZIONE, False) <> "", Agro_SQL_SaveText(LCase(SEZIONE)), 0) & "'  ")
            StrSQL.Append("         , " & IIf(Agro_SQL_SaveNum(FOGLIO, False) <> 0, Agro_SQL_SaveNum(FOGLIO), 0) & "  ")
            StrSQL.Append("         , " & IIf(Agro_SQL_SaveNum(NUMERO, False) <> 0, Agro_SQL_SaveNum(NUMERO), 0) & "  ")
            StrSQL.Append("         ,'" & IIf(Agro_SQL_SaveText(SUBALTERNO, False) <> "", Agro_SQL_SaveText(LCase(SUBALTERNO)), 0) & "' ")
            StrSQL.Append("         ," & Agro_SQL_SaveNum(MetodoProduzione_Cod) & " ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(objParametri.FlagVisibilita) & "")
            StrSQL.Append("         , Null  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDateTime_NULL(DateTime.Now) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDateTime_NULL(DateTime.Now) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(objParametri.LogDescrizioneUtente) & "' ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(objParametri.LogDescrizioneUtente) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")

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

    Public Function Elimina(ByVal PROV As String,
                            ByVal COM As String,
                            ByVal SEZIONE As String,
                            ByVal FOGLIO As Int32,
                            ByVal NUMERO As Int32,
                            ByVal SUBALTERNO As String,
                            ByVal MetodoProduzione_Cod As Integer,
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.ParticelleCatastali_MetodoProduzione_R.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""                   =>  si leggono tutte le imprese
        '   Sa_Cod = 0
        '   Cod_Contatto = 0
        '   Id_Cod = 0
        '   Val_Cod = ""
        '
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim xRisp As Integer = 0
        Try

            'TODO
            'modificare la query di select
            StrSQL.Length = 0
            StrSQL.Append(" DELETE FROM    ParticelleCatastali_MetodoProduzione ")
            StrSQL.Append(" WHERE   1=1  ")

            If PROV <> "" Then
                StrSQL.Append(" AND ParticelleCatastali_MetodoProduzione.PROV = '" & Agro_SQL_SaveText(Trim(PROV)) & "' ")
            End If

            If COM <> "" Then
                StrSQL.Append(" AND ParticelleCatastali_MetodoProduzione.COM = '" & Agro_SQL_SaveText(Trim(COM)) & "' ")
            End If

            If SEZIONE <> "" Then
                StrSQL.Append(" AND ParticelleCatastali_MetodoProduzione.SEZIONE = '" & Agro_SQL_SaveText(LCase(SEZIONE)) & "' ")
            End If

            If FOGLIO <> 0 Then
                StrSQL.Append(" AND ParticelleCatastali_MetodoProduzione.FOGLIO =  " & Agro_SQL_SaveNum(FOGLIO) & " ")
            End If

            If NUMERO <> 0 Then
                StrSQL.Append(" AND ParticelleCatastali_MetodoProduzione.NUMERO =  " & Agro_SQL_SaveNum(NUMERO) & " ")
            End If

            If SUBALTERNO <> "" Then
                StrSQL.Append(" AND ParticelleCatastali_MetodoProduzione.SUBALTERNO = '" & Agro_SQL_SaveText(Trim(LCase(SUBALTERNO))) & "' ")
            End If

            If MetodoProduzione_Cod <> -1 Then
                StrSQL.Append(" AND     (ParticelleCatastali_MetodoProduzione.MetodoProduzione_Cod = " & Agro_SQL_SaveNum(MetodoProduzione_Cod) & ")  ")
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp

    End Function

End Class