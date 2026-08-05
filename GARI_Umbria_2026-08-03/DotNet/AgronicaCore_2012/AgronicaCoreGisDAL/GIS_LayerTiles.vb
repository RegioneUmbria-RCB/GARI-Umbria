Imports AgronicaCoreDataProvider.DataProviderExtensions
Public Class GIS_LayerTiles_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi(
                ByVal PivaSuperUser As String,
                ByVal LayerElementiGrafici_Cod As Integer,
                ByVal LayerTiles_Cod As Int32,
                ByVal TipologiaLayer_cod As Int32,
                ByVal Utente As String,
                ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile,
                ByVal xFiltroAggiuntivo As String,
                ByVal xOrderBy As String,
                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
            ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_LayerTiles_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                    StrSQL.Length = 0

                    StrSQL.Append(" SELECT LayerTiles_Cod, LayerTiles_Des ")
                    StrSQL.Append(" FROM   GIS_LayerTiles ")
                    StrSQL.Append(" WHERE GIS_LayerTiles.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND   GIS_LayerTiles.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.Append(" AND   GIS_LayerTiles.PivaSuperUser = '" & Agro_SQL_SaveText(PivaSuperUser) & "' ")
                    StrSQL.Append(" AND   GIS_LayerTiles.Utente = '" & Agro_SQL_SaveText(Utente) & "' ")

                    If LayerElementiGrafici_Cod <> 0 Then
                        StrSQL.Append(" AND GIS_LayerTiles.LayerElementiGrafici_Cod = " & Agro_SQL_SaveNum(LayerElementiGrafici_Cod) & " ")
                    End If

                    If LayerTiles_Cod <> 0 Then
                        StrSQL.Append(" AND GIS_LayerTiles.LayerTiles_Cod = " & Agro_SQL_SaveNum(LayerTiles_Cod) & " ")
                    End If

                    If TipologiaLayer_cod <> 0 Then
                        StrSQL.Append(" AND GIS_LayerTiles.TipologiaLayer_cod = " & Agro_SQL_SaveNum(TipologiaLayer_cod) & " ")
                    End If

                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   GIS_LayerTiles.Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   GIS_LayerTiles.Inviato =-1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    End If

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta

                    StrSQL.Length = 0

                    StrSQL.Append(" SELECT * ")
                    StrSQL.Append(" FROM GIS_LayerTiles ")
                    StrSQL.Append(" WHERE GIS_LayerTiles.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND   GIS_LayerTiles.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.Append(" AND   GIS_LayerTiles.Utente = '" & Agro_SQL_SaveText(Utente) & "' ")
                    StrSQL.Append(" AND   GIS_LayerTiles.PivaSuperUser = '" & Agro_SQL_SaveText(PivaSuperUser) & "' ")

                    If LayerElementiGrafici_Cod <> 0 Then
                        StrSQL.Append(" AND GIS_LayerTiles.LayerElementiGrafici_Cod = " & Agro_SQL_SaveNum(LayerElementiGrafici_Cod) & " ")
                    End If

                    If LayerTiles_Cod <> 0 Then
                        StrSQL.Append(" AND GIS_LayerTiles.LayerTiles_Cod = " & Agro_SQL_SaveNum(LayerTiles_Cod) & " ")
                    End If

                    If TipologiaLayer_cod <> 0 Then
                        StrSQL.Append(" AND GIS_LayerTiles.TipologiaLayer_cod = " & Agro_SQL_SaveNum(TipologiaLayer_cod) & " ")
                    End If

                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   GIS_LayerTiles.Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   GIS_LayerTiles.Inviato =-1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    End If

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni
                    '
                    '
                    '

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta
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

Public Class GIS_LayerTiles_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Scrivi(
            ByVal PivaSuperUser As String,
            ByVal LayerElementiGrafici_Cod As Int32,
            ByVal Utente As String,
            ByVal LayerTiles_Cod As Int32,
            ByVal TipologiaLayer_cod As Int32,
            ByVal LayerTiles_Des As String,
            ByVal Colore_Primario As String,
            ByVal Colore_Secondario As String,
            ByVal Colore_Base As String,
            ByVal Varianza As Int32,
            ByVal Validita_Inizio As Date,
            ByVal Validita_Fine As Date,
            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
        ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_LayerTiles_W.Scrivi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            If PivaSuperUser = "" Then
                Throw New Exception("Parametro non corretto nella query (PivaSuperUser obbligatorio)")
            End If

            If LayerTiles_Cod = 0 Then
                Throw New Exception("Parametro non corretto nella query (LayerTiles_Cod obbligatorio)")
            End If

            StrSQL.Length = 0
            StrSQL.Append("INSERT INTO GIS_LayerTiles ")
            StrSQL.Append("                   ( ")
            StrSQL.Append("                    PivaSuperUser,   LayerElementiGrafici_Cod,    ")
            StrSQL.Append("                    Utente,   LayerTiles_Cod,    ")
            StrSQL.Append("                    TipologiaLayer_cod, ")
            StrSQL.Append("                    LayerTiles_Des,  Colore_Primario,    ")
            StrSQL.Append("                    Colore_Secondario,   Colore_Base ,Varianza,              ")
            StrSQL.Append("                    Inviato,            DataInvio, ")
            StrSQL.Append("                    Data_Creazione,     Data_Modifica, ")
            StrSQL.Append("                    UserName_Creazione, UserName_Modifica, ")
            StrSQL.Append("                    Validita_Inizio,    Validita_Fine ")
            StrSQL.Append("                   ) ")
            StrSQL.Append("VALUES (")
            StrSQL.Append("          '" & Agro_SQL_SaveText(PivaSuperUser) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(LayerElementiGrafici_Cod) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Utente) & "' ")
            StrSQL.Append("         ,  " & Agro_SQL_SaveNum(LayerTiles_Cod) & "  ")
            StrSQL.Append("         ,  " & Agro_SQL_SaveNum(TipologiaLayer_cod) & "  ")
            StrSQL.Append("         , '" & Agro_SQL_SaveText(LayerTiles_Des) & "' ")
            StrSQL.Append("         , '" & Agro_SQL_SaveText(Colore_Primario) & "'  ")
            StrSQL.Append("         , '" & Agro_SQL_SaveText(Colore_Secondario) & "'  ")
            StrSQL.Append("         , '" & Agro_SQL_SaveText(Colore_Base) & "'  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Varianza) & "  ")
            StrSQL.Append("         , 0  ")
            StrSQL.Append("         , Null  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Date.Now) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Date.Now) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")
            StrSQL.Append(" )")

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

    Public Function Modifica_colori(
            ByVal PivaSuperUser As String,
            ByVal LayerElementiGrafici_Cod As Int32,
            ByVal LayerTiles_Cod As Int32,
            ByVal TipologiaLayer_cod As Int32,
            ByVal Utente As String,
            ByVal Colore_Primario As String,
            ByVal Colore_secondario As String,
            ByVal Varianza As Int32,
            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
        ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_LayerTiles_W.Modifica_colori()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If PivaSuperUser = "" Then
                Throw New Exception("Parametro non corretto nella query (PivaSuperUser obbligatorio)")
            End If

            If LayerElementiGrafici_Cod = 0 Then
                Throw New Exception("Parametro non corretto nella query (LayerElementiGrafici_Cod obbligatorio)")
            End If

            If LayerTiles_Cod = 0 Then
                Throw New Exception("Parametro non corretto nella query (LayerTiles_Cod obbligatorio)")
            End If

            If TipologiaLayer_cod = 0 Then
                Throw New Exception("Parametro non corretto nella query (TipologiaLayer_cod obbligatorio)")
            End If

            StrSQL.Length = 0

            StrSQL.Append("UPDATE GIS_LayerTiles SET ")
            StrSQL.Append("  [Colore_Primario] = '" & Agro_SQL_SaveText(Colore_Primario) & "' ")
            StrSQL.Append(" ,[Colore_secondario] = '" & Agro_SQL_SaveText(Colore_secondario) & "' ")
            StrSQL.Append(" ,[Varianza] = " & Agro_SQL_SaveNum(Varianza) & " ")
            StrSQL.Append(" ,[Data_Modifica] = " & Agro_SQL_SaveDate(Date.Now) & " ")
            StrSQL.Append(" ,[UserName_Modifica] = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.Append(" WHERE PivaSuperUser = '" & Agro_SQL_SaveText(PivaSuperUser) & "' ")
            StrSQL.Append(" AND   LayerElementiGrafici_Cod = " & Agro_SQL_SaveNum(LayerElementiGrafici_Cod) & " ")
            StrSQL.Append(" AND   LayerTiles_Cod = " & Agro_SQL_SaveNum(LayerTiles_Cod) & " ")
            StrSQL.Append(" AND   TipologiaLayer_cod = " & Agro_SQL_SaveNum(TipologiaLayer_cod) & " ")
            StrSQL.Append(" AND   Utente = '" & Agro_SQL_SaveText(Utente) & "' ")

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
            ByVal PivaSuperUser As String,
            ByVal LayerTiles_Cod As Int32,
            ByVal xFiltroAggiuntivo As String,
            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
        ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_LayerTiles_W.Cancella()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If PivaSuperUser = "" Then
                Throw New Exception("Parametro non corretto nella query (PivaSuperUser obbligatorio)")
            End If

            If LayerTiles_Cod = 0 Then
                Throw New Exception("Parametro non corretto nella query (LayerTiles_Cod obbligatorio)")
            End If

            If objParametri.FlagCancellazioneLogica = AgronicaCoreDataProvider.AgronicaCoreParametri.enumCancellazioneLogica.CancellazioneLogica Then

                StrSQL.Length = 0
                StrSQL.Append(" UPDATE GIS_LayerTiles ")
                StrSQL.Append(" SET ")
                StrSQL.Append("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("      ,Inviato = -1 ")
                StrSQL.Append(" WHERE  PivaSuperUser    =  '" & Agro_SQL_SaveText(PivaSuperUser) & "'  ")
                StrSQL.Append(" AND LayerTiles_Cod      =   " & Agro_SQL_SaveNum(LayerTiles_Cod) & "  ")
                StrSQL.Append(" AND Inviato >= 0")

            Else

                StrSQL.Length = 0
                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM     GIS_LayerTiles ")
                StrSQL.Append(" WHERE  PivaSuperUser    =  '" & Agro_SQL_SaveText(PivaSuperUser) & "'  ")
                StrSQL.Append(" AND LayerTiles_Cod      =   " & Agro_SQL_SaveNum(LayerTiles_Cod) & "  ")

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

    Public Function Elimina(
        ByVal PivaSuperUser As String,
        ByVal LayerTiles_Cod As Int32,
        ByVal Utente As String,
        ByVal TipoLayer As Int32,
        ByVal xFiltroAggiuntivo As String,
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
    ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.GIS_LayerTiles_W.Elimina()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If PivaSuperUser = "" Then
                Throw New Exception("Parametro non corretto nella query (PivaSuperUser obbligatorio)")
            End If

            If LayerTiles_Cod = 0 Then
                Throw New Exception("Parametro non corretto nella query (LayerTiles_Cod obbligatorio)")
            End If

            StrSQL.Length = 0
            StrSQL.Append(" DELETE ")
            StrSQL.Append(" FROM     GIS_LayerTiles ")
            StrSQL.Append(" WHERE  PivaSuperUser    =  '" & Agro_SQL_SaveText(PivaSuperUser) & "'  ")
            StrSQL.Append(" AND LayerTiles_Cod      =   " & Agro_SQL_SaveNum(LayerTiles_Cod) & "  ")
            StrSQL.Append(" AND TipologiaLayer_Cod      =   " & Agro_SQL_SaveNum(TipoLayer) & "  ")
            StrSQL.Append(" AND Utente      =   '" & Agro_SQL_SaveText(Utente) & "'  ")

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

    Public Function DeleteAllTilesPerLayer(pivaSuperUser As String,
                                           layerCod As Int32,
                                           objParametriServer As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean
        Dim routineName As String = "AgronicaCoreGisDAL.GIS_LayerTiles_W.DeleteAllTilesPerLayer()"
        Dim strSQL As New Text.StringBuilder

        Try
            If String.IsNullOrWhiteSpace(pivaSuperUser) AndAlso pivaSuperUser = "" Then
                Throw New Exception("Parametro non corretto: pivaSuperUser")
            End If

            If pivaSuperUser = 0 Then
                Throw New Exception("Parametro non corretto: layerCod")
            End If

            strSQL.Length = 0
            strSQL.AppendLine("DELETE FROM GIS_LayerTiles")
            strSQL.AppendLine($"WHERE PivaSuperUser = '{Agro_SQL_SaveText(pivaSuperUser)}'")
            strSQL.AppendLine($"    AND LayerElementiGrafici_Cod = {Agro_SQL_SaveNum(layerCod)}")

            Return EseguiQuery_Scrittura(objParametriServer, strSQL.ToString, routineName)
        Catch ex As Exception
            Scrivi_LOG(objParametriServer, routineName, ex.Message)
            Throw New Exception($"[{routineName}] : {ex.Message}")
        End Try
    End Function

End Class