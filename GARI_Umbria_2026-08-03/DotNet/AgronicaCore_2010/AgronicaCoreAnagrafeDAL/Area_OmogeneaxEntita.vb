Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate

Public Class Area_OmogeneaxEntita_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '################################################################################
    Public Function Leggi( _
                            ByVal Piva As String, _
                            ByVal Sa_Cod As Integer, _
                            ByVal Area_Cod As Integer, _
                            ByVal Entita_Cod As Integer, _
                                ByVal xFiltroAggiuntivo As String, _
                                ByVal xOrderBy As String, _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                ) As DataTable
        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Area_OmogeneaxEntita_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim DT As DataTable

        Dim StrSQL As New System.Text.StringBuilder

        Try

            StrSQL.Length = 0

            StrSQL.Length = 0

            StrSQL.Append(" SELECT  * ")
            StrSQL.Append(" FROM    Area_OmogeneaxEntita ")

            StrSQL.Append(" WHERE   Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.Append(" AND     Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.Append(" AND     Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            If Piva <> "" Then
                StrSQL.Append(" AND Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If

            If Sa_Cod <> 0 Then
                StrSQL.Append(" AND Sa_Cod = '" & Agro_SQL_SaveText(Sa_Cod.ToString) & "' ")
            End If

            If Area_Cod <> 0 Then
                StrSQL.Append(" AND Area_Cod = " & Agro_SQL_SaveNum(Area_Cod.ToString))
            End If

            If Entita_Cod <> 0 Then
                StrSQL.Append(" AND Entita_Cod = " & Agro_SQL_SaveNum(Entita_Cod.ToString))
            End If

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

            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY Piva, Sa_Cod, Entita_Cod, Area_Cod ")
            End If

            '----------------------------------------------------
            '--- Recupero il datatable --------------------------
            '----------------------------------------------------

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

Public Class Area_OmogeneaxEntita_W
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function Scrivi(ByVal Piva As String, _
                               ByVal Area_Cod As Integer, _
                               ByVal Entita_Cod As Integer, _
                                ByVal Validita_Inizio As Date, _
                                ByVal Validita_Fine As Date, _
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                    , Optional ByVal Data_creazione As Date = #2/1/1900# _
                    , Optional ByVal Data_modifica As Date = #2/1/1900# _
                    , Optional ByVal username_creazione As String = "" _
                    , Optional ByVal username_modifica As String = "" _
                                    ) As Boolean


        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Area_OmogeneaxEntita_W.Scrivi()"

        Dim ObjSequenze As AgronicaCoreDataProvider.Agro_Sequenze

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



            '----- Genero la query SQL 
            StrSQL.Append(" INSERT INTO Area_OmogeneaxEntita ( ")
            StrSQL.Append("             Piva_SuperUser,         Piva,   Sa_Cod, ")
            StrSQL.Append("             Entita_Cod,             Area_Cod, ")
            StrSQL.Append("             Inviato,                ")
            StrSQL.Append("             Data_Creazione,         Data_Modifica, ")
            StrSQL.Append("             UserName_Creazione,     UserName_Modifica, ")
            StrSQL.Append("             Validita_Inizio,        Validita_Fine ")
            StrSQL.Append(" ) ")

            StrSQL.Append(" VALUES ( ")
            StrSQL.Append("          '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.Append("        ,'" & Agro_SQL_SaveText(Piva.ToString) & "' ")
            StrSQL.Append("         ,0")
            StrSQL.Append("         ," & Agro_SQL_SaveNum(Entita_Cod.ToString) & " ")
            StrSQL.Append("         ," & Agro_SQL_SaveNum(Area_Cod.ToString) & " ")
            StrSQL.Append("         , 0  ")

            StrSQL.Append("			, " & Agro_SQL_SaveDate(Data_creazione) & "  ")
            StrSQL.Append("			, " & Agro_SQL_SaveDate(Data_modifica) & "  ")
            StrSQL.Append("			,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            StrSQL.Append("			,'" & Agro_SQL_SaveText(username_modifica) & "' ")

            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")

            StrSQL.Append(" ) ")


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
    Public Function Modifica_Area( _
                               ByVal Area_Cod As Integer, _
                               ByVal Entita_Cod As Integer, _
                                    ByVal Validita_Inizio As Date, _
                                    ByVal Validita_Fine As Date, _
                                        ByVal xFiltroAggiuntivo As String, _
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                            ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Area_OmogeneaxEntita_W.Modifica_Area()"

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

            If objParametri.PivaSuperUser = "" Then
                Throw New Exception("Parametro non corretto nella query (PivaSuperUser obbligatorio)")
            End If

            If Entita_Cod = 0 Then
                Throw New Exception("Parametro non corretto nella query (Entita_Cod obbligatorio)")
            End If


            '---------------------------------------------
            StrSQL.Append(" UPDATE Area_OmogeneaxEntita SET  ")

            If Area_Cod <> 0 Then
                StrSQL.Append("    Area_Cod     = " & Agro_SQL_SaveNum(Area_Cod) & "  ,")
            End If

            If Validita_Inizio <> AGRODATAINIZIO Then
                StrSQL.Append("    Validita_Inizio     = " & Agro_SQL_SaveDate(Validita_Inizio) & "  ,")
            End If
            If Validita_Fine <> AGRODATAFINE Then
                StrSQL.Append("    Validita_Fine     = " & Agro_SQL_SaveDate(Validita_Fine) & "  ,")
            End If

            StrSQL.Append("Data_Modifica = " & Agro_SQL_SaveDate(Date.Now) & ",")
            StrSQL.Append("Username_Modifica= '" & Agro_SQL_SaveText(objParametri.UtenteCodFiscale) & "'")

            StrSQL.Append(" WHERE Entita_Cod         = " & Agro_SQL_SaveNum(Entita_Cod) & "  ")
            StrSQL.Append(" AND   Piva_SuperUser =  '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'   ")
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
    Public Function Modifica_Entita( _
                               ByVal Entita_Cod_Old As Integer, _
                               ByVal Entita_Cod_New As Integer, _
                                    ByVal Validita_Inizio As Date, _
                                    ByVal Validita_Fine As Date, _
                                        ByVal xFiltroAggiuntivo As String, _
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                            ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Area_OmogeneaxEntita_W.Modifica_Entita()"

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

            If objParametri.PivaSuperUser = "" Then
                Throw New Exception("Parametro non corretto nella query (PivaSuperUser obbligatorio)")
            End If

            If Entita_Cod_Old = 0 Then
                Throw New Exception("Parametro non corretto nella query (Entita_Cod_Old obbligatorio)")
            End If

            If Entita_Cod_New = 0 Then
                Throw New Exception("Parametro non corretto nella query (Entita_Cod_New obbligatorio)")
            End If

            '---------------------------------------------
            StrSQL.Append(" UPDATE Area_OmogeneaxEntita SET  ")

            StrSQL.Append("    Entita_Cod     = " & Agro_SQL_SaveNum(Entita_Cod_New) & "  ,")

            If Validita_Inizio <> AGRODATAINIZIO Then
                StrSQL.Append("    Validita_Inizio     = " & Agro_SQL_SaveDate(Validita_Inizio) & "  ,")
            End If
            If Validita_Fine <> AGRODATAFINE Then
                StrSQL.Append("    Validita_Fine     = " & Agro_SQL_SaveDate(Validita_Fine) & "  ,")
            End If

            StrSQL.Append("Data_Modifica = " & Agro_SQL_SaveDate(Date.Now) & ",")
            StrSQL.Append("Username_Modifica= '" & Agro_SQL_SaveText(objParametri.UtenteCodFiscale) & "'")

            StrSQL.Append(" WHERE Entita_Cod         = " & Agro_SQL_SaveNum(Entita_Cod_Old) & "  ")
            StrSQL.Append(" AND   Piva_SuperUser =  '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'   ")
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
    Public Function Cancella( _
                              ByVal Piva As String, _
                              ByVal Area_Cod As Integer, _
                              ByVal Entita_Cod As Integer, _
                                ByVal Cancella_Singola_Entita As Boolean, _
                                ByVal Cancella_Area As Boolean, _
                                ByVal Cancella_Azienda As Boolean, _
                                    ByVal xFiltroAggiuntivo As String, _
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Area_OmogeneaxEntita_W.Cancella()"

        '====================================================================================
        'Parametri opzionali :
        '   Programmazione_Cod=0        => Vengono cancellate tutte le entità del Piva_SuperUser
        '   Programmazione_Entita_Cod=0 => Vengono cancellate tutte le entità di una programmazione
        '
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = AgronicaCoreDataProvider.AgronicaCoreParametri.enumCancellazioneLogica.CancellazioneLogica Then

                StrSQL.Length = 0
                StrSQL.Append(" UPDATE Area_OmogeneaxEntita ")
                StrSQL.Append(" SET ")
                StrSQL.Append("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UtenteCodFiscale) & "' ")
                StrSQL.Append("      ,Inviato = -1 ")
                StrSQL.Append(" WHERE  Inviato >= 0")

            Else

                StrSQL.Length = 0
                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM     Area_OmogeneaxEntita ")
                StrSQL.Append(" WHERE   1=1 ")

            End If

            If objParametri.PivaSuperUser <> "" Then
                StrSQL.Append("  AND   Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            End If

            If Cancella_Singola_Entita = True Then
                StrSQL.Append("  AND   Entita_Cod = " & Agro_SQL_SaveNum(Entita_Cod) & " ")
            End If

            If Cancella_Area = True Then
                StrSQL.Append(" AND Area_Cod = " & Agro_SQL_SaveNum(Area_Cod.ToString) & " ")
            End If

            If Cancella_Azienda = True Then
                StrSQL.Append(" AND Piva = '" & Agro_SQL_SaveText(Piva.ToString) & "' ")
            End If

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




End Class
