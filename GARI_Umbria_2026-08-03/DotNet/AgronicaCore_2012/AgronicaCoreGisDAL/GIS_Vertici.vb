Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.DataProvider
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi





'   !!!!!!!!!!!!!!!!!!!!!!!!!!!!
'   !!!   WORK IN PROGRESS   !!!
'   !!!!!!!!!!!!!!!!!!!!!!!!!!!!





'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################

Public Class GIS_Vertici_R
    Inherits AgronicaCoreDataProvider.DataProvider


    ''//////////////////////////////////////////////////////////////////////////////////////////
    ''//////////////////////////////////////////////////////////////////////////////////////////
    'Public Function Leggi( _
    '                        ByVal PivaSuperUser As String, _
    '                        ByVal ElementoGrafico_Cod As Int32, _
    '                        ByVal Entita_Cod As Int32, _
    '                        ByVal LayerElementiGrafici_Cod As Int32, _
    '                                ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile, _
    '                                ByVal xFiltroAggiuntivo As String, _
    '                                ByVal xOrderBy As String, _
    '                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
    '                                ) As DataTable

    '    Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_ElementiGrafici_R.Leggi()"

    '    '====================================================================================
    '    'Parametri opzionali :
    '    '   ElementoGrafico_Cod = 0
    '    '   Entita_Cod = 0
    '    '   LayerElementiGrafici_Cod = 0
    '    '
    '    '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
    '    '   DirectoryLOG = ""           =>  viene usato il valore di default
    '    '   FileLOG = ""                =>  viene usato il valore di default
    '    '====================================================================================

    '    Dim MessaggioErrore As String = ""
    '    Dim StrSQL As New System.Text.StringBuilder
    '    Dim DT As DataTable

    '    Try
    '        Select Case xSelezioneVariabile

    '            Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi

    '                '
    '                '
    '                '

    '            Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta

    '                StrSQL.Length = 0
    '                StrSQL.Append(" SELECT  * ")
    '                StrSQL.Append(" FROM    GIS_ElementiGrafici ")
    '                StrSQL.Append(" WHERE   GIS_ElementiGrafici.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
    '                StrSQL.Append(" AND     GIS_ElementiGrafici.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

    '                StrSQL.Append(" AND GIS_ElementiGrafici.PivaSuperUser = '" & Agro_SQL_SaveText(PivaSuperUser) & "' ")

    '                If ElementoGrafico_Cod <> 0 Then
    '                    StrSQL.Append(" AND GIS_ElementiGrafici.ElementoGrafico_Cod = " & Agro_SQL_SaveNum(ElementoGrafico_Cod) & " ")
    '                End If

    '                If Entita_Cod <> 0 Then
    '                    StrSQL.Append(" AND GIS_ElementiGrafici.Entita_Cod = " & Agro_SQL_SaveNum(Entita_Cod) & " ")
    '                End If

    '                If LayerElementiGrafici_Cod <> 0 Then
    '                    StrSQL.Append(" AND GIS_ElementiGrafici.LayerElementiGrafici_Cod = " & Agro_SQL_SaveNum(LayerElementiGrafici_Cod) & " ")
    '                End If

    '                '--------------------------------------------------------------------------
    '                If xFiltroAggiuntivo <> "" Then
    '                    StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
    '                End If
    '                '--------------------------------------------------------------------------
    '                Select Case objParametri.FlagVisibilita
    '                    Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
    '                        StrSQL.Append(" AND   GIS_ElementiGrafici.Inviato >=0 ")
    '                    Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
    '                        StrSQL.Append(" AND   GIS_ElementiGrafici.Inviato =-1 ")
    '                    Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
    '                        '...................................
    '                    Case Else
    '                        Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
    '                End Select
    '                '--------------------------------------------------------------------------
    '                If xOrderBy <> "" Then
    '                    strSql.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
    '                End If


    '            Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni
    '                '
    '                '
    '                '

    '            Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta
    '                '
    '                '
    '                '

    '        End Select

    '        '--------------------------------------------------------------------------
    '        DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
    '        '--------------------------------------------------------------------------

    '    Catch ex As Exception

    '        MessaggioErrore = ex.Message
    '        Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
    '        DT = Nothing
    '        Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

    '    End Try

    '    Return DT

    'End Function



End Class




'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################

Public Class GIS_Vertici_W
    Inherits AgronicaCoreDataProvider.DataProvider



    ''//////////////////////////////////////////////////////////////////////////////////////////
    ''//////////////////////////////////////////////////////////////////////////////////////////
    'Public Function Scrivi( _
    '                        ByVal PivaSuperUser As String, _
    '                        ByVal ElementoGrafico_Cod As Int32, _
    '                        ByVal ElementoGrafico_Des As String, _
    '                        ByVal Entita_Cod As Int32, _
    '                        ByVal LayerElementiGrafici_Cod As Int32, _
    '                        ByVal Poligono_GeoEntity As String, _
    '                        ByVal Flag_GPS As Int32, _
    '                            ByVal Validita_Inizio As Date, _
    '                            ByVal Validita_Fine As Date, _
    '                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
    '                            ) As Boolean

    '    Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_ElementiGrafici_W.Scrivi()"

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
    '        StrSQL.Append("INSERT INTO GIS_ElementiGrafici ")
    '        StrSQL.Append("                   ( ")
    '        StrSQL.Append("                    PivaSuperUser,               ElementoGrafico_Cod,    ")
    '        StrSQL.Append("                    ElementoGrafico_Des,         Entita_Cod,    ")
    '        StrSQL.Append("                    LayerElementiGrafici_Cod,    Poligono_GeoEntity,    ")
    '        StrSQL.Append("                    Flag_GPS,    ")

    '        StrSQL.Append("                    Inviato,            DataInvio, ")
    '        StrSQL.Append("                    Data_Creazione,     Data_Modifica, ")
    '        StrSQL.Append("                    UserName_Creazione, UserName_Modifica, ")
    '        StrSQL.Append("                    Validita_Inizio,    Validita_Fine ")
    '        StrSQL.Append("                   ) ")

    '        StrSQL.Append("VALUES (")

    '        StrSQL.Append("          '" & Agro_SQL_SaveText(PivaSuperUser) & "' ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(ElementoGrafico_Cod) & "  ")
    '        StrSQL.Append("         ,'" & Agro_SQL_SaveText(ElementoGrafico_Des) & "' ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(Entita_Cod) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(LayerElementiGrafici_Cod) & "  ")

    '        StrSQL.Append("         ,'" & Agro_SQL_SaveText(Poligono_GeoEntity) & "' ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(Flag_GPS) & "  ")

    '        StrSQL.Append("         , 0  ")
    '        StrSQL.Append("         , Null  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveDate(Date.Now) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveDate(Date.Now) & "  ")
    '        StrSQL.Append("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
    '        StrSQL.Append("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")
    '        StrSQL.Append(" )")
    '        '---------------------------------------------

    '        '--------------------------------------------------------------------------
    '        xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
    '        '--------------------------------------------------------------------------

    '    Catch ex As Exception

    '        MessaggioErrore = ex.Message
    '        Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
    '        xRisp = False
    '        Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

    '    End Try

    '    Return xRisp

    'End Function



    ''//////////////////////////////////////////////////////////////////////////////////////////
    ''//////////////////////////////////////////////////////////////////////////////////////////
    'Public Function Modifica( _
    '                        ByVal PivaSuperUser As String, _
    '                        ByVal ElementoGrafico_Cod As Int32, _
    '                        ByVal ElementoGrafico_Des As String, _
    '                        ByVal Entita_Cod As Int32, _
    '                        ByVal LayerElementiGrafici_Cod As Int32, _
    '                        ByVal Poligono_GeoEntity As String, _
    '                        ByVal Flag_GPS As Int32, _
    '                                ByVal Validita_Inizio As Date, _
    '                                ByVal Validita_Fine As Date, _
    '                                ByVal xFiltroAggiuntivo As String, _
    '                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
    '                                ) As Boolean

    '    Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_ElementiGrafici_W.Modifica()"

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

    '        If PivaSuperUser = "" Then
    '            Throw New Exception("Parametro non corretto nella query (PivaSuperUser obbligatorio)")
    '        End If

    '        If ElementoGrafico_Cod = 0 Then
    '            Throw New Exception("Parametro non corretto nella query (ElementoGrafico_Cod obbligatorio)")
    '        End If

    '        '---------------------------------------------
    '        StrSQL.Length = 0

    '        StrSQL.Append("UPDATE GIS_Entita SET ")

    '        StrSQL.Append("    [[ElementoGrafico_Des]]      = '" & Agro_SQL_SaveText(ElementoGrafico_Des) & "'")
    '        StrSQL.Append("   ,[Entita_Cod]                 =  " & Agro_SQL_SaveNum(Entita_Cod) & " ")
    '        StrSQL.Append("   ,[LayerElementiGrafici_Cod]   =  " & Agro_SQL_SaveNum(LayerElementiGrafici_Cod) & " ")
    '        StrSQL.Append("   ,[Poligono_GeoEntity]         = '" & Agro_SQL_SaveText(Poligono_GeoEntity) & "'")
    '        StrSQL.Append("   ,[Flag_GPS]                   =  " & Agro_SQL_SaveNum(Flag_GPS) & " ")

    '        StrSQL.Append("   ,Inviato           =  0 ")
    '        StrSQL.Append("   ,DataInvio         =  Null ")
    '        StrSQL.Append("   ,Data_Modifica     =  " & Agro_SQL_SaveDate(Date.Now))
    '        StrSQL.Append("   ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
    '        StrSQL.Append("   ,Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Inizio) & " ")
    '        StrSQL.Append("   ,Validita_Fine     =  " & Agro_SQL_SaveDate(Validita_Fine) & " ")

    '        StrSQL.Append(" WHERE PivaSuperUser         = '" & Agro_SQL_SaveText(PivaSuperUser) & "' ")
    '        StrSQL.Append(" AND   ElementoGrafico_Cod   =  " & Agro_SQL_SaveNum(ElementoGrafico_Cod) & " ")
    '        '---------------------------------------------

    '        '----------------------------------------------------------------------
    '        If xFiltroAggiuntivo <> "" Then
    '            StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
    '        End If
    '        '--------------------------------------------------------------------------
    '        xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
    '        '--------------------------------------------------------------------------

    '    Catch ex As Exception

    '        MessaggioErrore = ex.Message
    '        Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
    '        xRisp = False
    '        Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

    '    End Try

    '    Return xRisp

    'End Function



    ''//////////////////////////////////////////////////////////////////////////////////////////
    ''//////////////////////////////////////////////////////////////////////////////////////////
    'Public Function Cancella( _
    '                        ByVal PivaSuperUser As String, _
    '                        ByVal ElementoGrafico_Cod As Int32, _
    '                                ByVal xFiltroAggiuntivo As String, _
    '                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
    '                                ) As Boolean

    '    Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_ElementiGrafici_W.Cancella()"

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

    '        If PivaSuperUser = "" Then
    '            Throw New Exception("Parametro non corretto nella query (PivaSuperUser obbligatorio)")
    '        End If

    '        If ElementoGrafico_Cod = 0 Then
    '            Throw New Exception("Parametro non corretto nella query (ElementoGrafico_Cod obbligatorio)")
    '        End If

    '        '---------------------------------------------
    '        If objParametri.FlagCancellazioneLogica = AgronicaCoreDataProvider.AgronicaCoreParametri.enumCancellazioneLogica.CancellazioneLogica Then

    '            StrSQL.Length = 0
    '            StrSQL.Append(" UPDATE GIS_ElementiGrafici ")
    '            StrSQL.Append(" SET ")
    '            StrSQL.Append("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
    '            StrSQL.Append("      ,Inviato = -1 ")
    '            StrSQL.Append(" WHERE   PivaSuperUser    =  '" & Agro_SQL_SaveText(PivaSuperUser) & "'  ")
    '            StrSQL.Append(" AND     ElementoGrafico_Cod      =   " & Agro_SQL_SaveNum(ElementoGrafico_Cod) & "  ")
    '            StrSQL.Append(" AND Inviato >= 0")

    '        Else

    '            StrSQL.Length = 0
    '            StrSQL.Append(" DELETE ")
    '            StrSQL.Append(" FROM    GIS_ElementiGrafici ")
    '            StrSQL.Append(" WHERE   PivaSuperUser    =  '" & Agro_SQL_SaveText(PivaSuperUser) & "'  ")
    '            StrSQL.Append(" AND     ElementoGrafico_Cod      =   " & Agro_SQL_SaveNum(ElementoGrafico_Cod) & "  ")

    '        End If

    '        If xFiltroAggiuntivo <> "" Then
    '            StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
    '        End If

    '        '--------------------------------------------------------------------------
    '        xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
    '        '--------------------------------------------------------------------------

    '    Catch ex As Exception
    '        MessaggioErrore = ex.Message
    '        Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
    '        xRisp = False
    '        Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
    '    End Try

    '    Return xRisp

    'End Function



End Class




'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################
