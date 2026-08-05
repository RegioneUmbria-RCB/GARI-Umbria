Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.AgronicaCoreParametri

Public Class NC_Dettagli_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function Leggi( _
                            ByVal xFiltroAggiuntivo As String, _
                            ByVal xOrderBy As String, _
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                            ) As DataTable

        ' Qui dovrei recuperare l'id dell'audit

        Return Leggi(Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, xFiltroAggiuntivo, xOrderBy, objParametri)

    End Function


    '##############################################################################################
    Public Function Leggi( _
                            ByVal ID_Dettaglio As Integer?, _
                            ByVal ID_NC As Integer?, _
                            ByVal Utente As String, _
                            ByVal Data As DateTime?, _
                            ByVal ID_Stato As Integer?, _
                            ByVal ID_ListaAllegati As Integer?, _
                            ByVal xFiltroAggiuntivo As String, _
                            ByVal xOrderBy As String, _
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                            ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCorePannelloDiControlloDAL.NC_Dettagli_R.Leggi"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim strSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            strSQL.Length = 0

            strSQL.AppendLine(" SELECT ID_Dettaglio, PivaSuperUser, ID_NC, Utente, Data, ID_Stato, ID_ListaAllegati, Descrizione, Note, ")
            strSQL.AppendLine(" DataChiusuraPrevista, DataChiusuraEffettiva, Responsabile, PersoneCoinvolte, CodiceDettaglio ")
            strSQL.AppendLine(" FROM NC_Dettagli ")

            strSQL.AppendLine(" WHERE PivaSuperUser = " & Agro_SQL_SaveText_NULL(objParametri.PivaSuperUser))

            If Not IsNothing(ID_Dettaglio) Then
                strSQL.AppendLine(" AND ID_Dettaglio = " & Agro_SQL_SaveNum_NULL(ID_Dettaglio))
            End If

            If Not IsNothing(ID_NC) Then
                strSQL.AppendLine(" AND ID_NC = " & Agro_SQL_SaveNum_NULL(ID_NC))
            End If

            If Not IsNothing(Utente) Then
                strSQL.AppendLine(" AND Utente = " & Agro_SQL_SaveText_NULL(Utente))
            End If

            If Not IsNothing(Data) Then
                strSQL.AppendLine(" AND Data = " & Agro_SQL_SaveDateTime_NULL(Data))
            End If

            If Not IsNothing(ID_Stato) Then
                strSQL.AppendLine(" AND ID_Stato = " & Agro_SQL_SaveNum_NULL(ID_Stato))
            End If

            If Not IsNothing(ID_ListaAllegati) Then
                strSQL.AppendLine(" AND ID_ListaAllegati = " & Agro_SQL_SaveNum_NULL(ID_ListaAllegati))
            End If

            If xFiltroAggiuntivo <> "" Then
                strSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    strSQL.AppendLine(" AND   Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    strSQL.AppendLine(" AND   Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                strSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                strSQL.AppendLine(" ORDER BY ID_NC, CodiceDettaglio, Data_Creazione")
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, strSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, True) & " QUERY: " & strSQL.ToString.Replace(vbCrLf, " ")
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function


End Class

Public Class NC_Dettagli_W
    Inherits AgronicaCoreDataProvider.DataProvider

    'ho tolto ID_NC perché non si può cambiare l'appartenenza ad una non conformità
    '##############################################################################################
    Public Function Modifica(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                            ByVal Old_ID_Dettaglio As Integer,
                            ByVal New_Utente As String,
                            ByVal New_Data As DateTime?,
                            ByVal New_ID_Stato As Integer?,
                            ByVal New_ID_ListaAllegati As Integer?,
                            ByVal New_Descrizione As String,
                            ByVal New_Note As String,
                            ByVal New_DataChiusuraPrevista As DateTime?,
                            ByVal New_DataChiusuraEffettiva As DateTime?,
                            ByVal New_Responsabile As String,
                            ByVal New_PersoneCoinvolte As String,
                            ByVal New_CodiceDettaglio As Integer?,
                            Optional ByVal Data_modifica As Date = #2/1/1900#,
                            Optional ByVal username_modifica As String = ""
                            ) As Boolean


        Dim NomeRoutine As String = "AgronicaCorePannelloDiControlloDAL.NC_Dettagli_W.Modifica()"

        '====================================================================================
        'Parametri opzionali :

        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If Data_modifica = #2/1/1900# Then
                Data_modifica = DateTime.Now
            End If

            If username_modifica = "" Then
                username_modifica = objParametri.UsernameOperazione
            End If

            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.AppendLine(" UPDATE NC_Dettagli SET")

            StrSQL.AppendLine("  Utente = " & Agro_SQL_SaveText_NULL(NothingToDBNull(New_Utente)))
            StrSQL.AppendLine(", Data = " & Agro_SQL_SaveDateTime_NULL(NothingToDBNull(New_Data)))
            StrSQL.AppendLine(", ID_Stato = " & Agro_SQL_SaveNum_NULL(NothingToDBNull(New_ID_Stato)))
            StrSQL.AppendLine(", ID_ListaAllegati  = " & Agro_SQL_SaveNum_NULL(NothingToDBNull(New_ID_ListaAllegati)))
            StrSQL.AppendLine(", Descrizione = " & Agro_SQL_SaveText_NULL(NothingToDBNull(New_Descrizione)))
            StrSQL.AppendLine(", Note = " & Agro_SQL_SaveText_NULL(NothingToDBNull(New_Note)))
            StrSQL.AppendLine(", DataChiusuraPrevista = " & Agro_SQL_SaveDateTime_NULL(NothingToDBNull(New_DataChiusuraPrevista)))
            StrSQL.AppendLine(", DataChiusuraEffettiva = " & Agro_SQL_SaveDateTime_NULL(NothingToDBNull(New_DataChiusuraEffettiva)))
            StrSQL.AppendLine(", Responsabile = " & Agro_SQL_SaveText_NULL(NothingToDBNull(New_Responsabile)))
            StrSQL.AppendLine(", PersoneCoinvolte = " & Agro_SQL_SaveText_NULL(NothingToDBNull(New_PersoneCoinvolte)))
            StrSQL.AppendLine(", CodiceDettaglio  = " & Agro_SQL_SaveNum_NULL(NothingToDBNull(New_CodiceDettaglio)))

            StrSQL.AppendLine(", Data_Modifica = " & Agro_SQL_SaveDateTime(Data_modifica))
            StrSQL.AppendLine(", UserName_Modifica = '" & Agro_SQL_SaveText(username_modifica) & "' ")

            StrSQL.AppendLine("	WHERE PivaSuperUser = " & Agro_SQL_SaveText_NULL(objParametri.PivaSuperUser))
            StrSQL.AppendLine("	AND ID_Dettaglio  =		" & Agro_SQL_SaveNum_NULL(Old_ID_Dettaglio))


            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, True) & " QUERY: " & StrSQL.ToString.Replace(vbCrLf, " ")
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return xRisp

    End Function

    '##############################################################################################
    Public Function Scrivi(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                            ByVal ID_Dettaglio As Integer,
                            ByVal ID_NC As Integer,
                            ByVal Utente As String,
                            ByVal Data As DateTime?,
                            ByVal ID_Stato As Integer?,
                            ByVal ID_ListaAllegati As Integer?,
                            ByVal Descrizione As String,
                            ByVal Note As String,
                            ByVal DataChiusuraPrevista As DateTime?,
                            ByVal DataChiusuraEffettiva As DateTime?,
                            ByVal Responsabile As String,
                            ByVal PersoneCoinvolte As String,
                            ByVal CodiceDettaglio As Integer?,
                            Optional ByVal Data_creazione As Date = #2/1/1900#,
                            Optional ByVal Data_modifica As Date = #2/1/1900#,
                            Optional ByVal username_creazione As String = "",
                            Optional ByVal username_modifica As String = ""
                            ) As Boolean


        Dim NomeRoutine As String = "AgronicaCorePannelloDiControlloDAL.NC_Dettagli_W.Scrivi()"

        '====================================================================================
        'Parametri opzionali :

        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If Data_creazione = #2/1/1900# Then
                Data_creazione = DateTime.Now
            End If

            If Data_modifica = #2/1/1900# Then
                Data_modifica = DateTime.Now
            End If

            If username_creazione = "" Then
                username_creazione = objParametri.UsernameOperazione
            End If

            If username_modifica = "" Then
                username_modifica = objParametri.UsernameOperazione
            End If



            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.AppendLine(" INSERT INTO  NC_Dettagli ")

            StrSQL.AppendLine("              (")
            StrSQL.AppendLine("              PivaSuperUser,                 ")
            StrSQL.AppendLine("              ID_Dettaglio,                  ID_NC, ")
            StrSQL.AppendLine("              Utente,                        Data,  ")
            StrSQL.AppendLine("              ID_Stato,                      ID_ListaAllegati, ")
            StrSQL.AppendLine("              Descrizione,                   Note,  ")
            StrSQL.AppendLine("              DataChiusuraPrevista,          DataChiusuraEffettiva,  ")
            StrSQL.AppendLine("              Responsabile,                  PersoneCoinvolte, ")
            StrSQL.AppendLine("              CodiceDettaglio,               ")

            StrSQL.AppendLine("              Inviato,            datainvio, ")
            StrSQL.AppendLine("              Data_Creazione,     Data_Modifica, ")
            StrSQL.AppendLine("              UserName_Creazione, UserName_Modifica, ")
            StrSQL.AppendLine("              Validita_Inizio,    Validita_Fine ")
            StrSQL.AppendLine("              ) ")

            StrSQL.AppendLine(" VALUES ( ")

            StrSQL.AppendLine("			 " & Agro_SQL_SaveText_NULL(objParametri.PivaSuperUser))
            StrSQL.AppendLine("			," & Agro_SQL_SaveNum_NULL(ID_Dettaglio))
            StrSQL.AppendLine("			," & Agro_SQL_SaveNum_NULL(ID_NC))
            StrSQL.AppendLine("			," & Agro_SQL_SaveText_NULL(NothingToDBNull(Utente)))
            StrSQL.AppendLine("			," & Agro_SQL_SaveDateTime_NULL(NothingToDBNull(Data)))
            StrSQL.AppendLine("			," & Agro_SQL_SaveNum_NULL(NothingToDBNull(ID_Stato)))
            StrSQL.AppendLine("			," & Agro_SQL_SaveNum_NULL(NothingToDBNull(ID_ListaAllegati)))
            StrSQL.AppendLine("			," & Agro_SQL_SaveText_NULL(NothingToDBNull(Descrizione)))
            StrSQL.AppendLine("			," & Agro_SQL_SaveText_NULL(NothingToDBNull(Note)))
            StrSQL.AppendLine("			," & Agro_SQL_SaveDateTime_NULL(NothingToDBNull(DataChiusuraPrevista)))
            StrSQL.AppendLine("			," & Agro_SQL_SaveDateTime_NULL(NothingToDBNull(DataChiusuraEffettiva)))
            StrSQL.AppendLine("			," & Agro_SQL_SaveText_NULL(NothingToDBNull(Responsabile)))
            StrSQL.AppendLine("			," & Agro_SQL_SaveText_NULL(NothingToDBNull(PersoneCoinvolte)))
            StrSQL.AppendLine("			," & Agro_SQL_SaveNum_NULL(NothingToDBNull(CodiceDettaglio)))

            StrSQL.AppendLine("         , 0  ")
            StrSQL.AppendLine("         , Null  ")
            StrSQL.AppendLine("			, " & Agro_SQL_SaveDateTime(Data_creazione) & "  ")
            StrSQL.AppendLine("			, " & Agro_SQL_SaveDateTime(Data_modifica) & "  ")
            StrSQL.AppendLine("			,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            StrSQL.AppendLine("			,'" & Agro_SQL_SaveText(username_modifica) & "' ")
            StrSQL.AppendLine("			, " & Agro_SQL_SaveDate(AGRODATAINIZIO) & "  ")
            StrSQL.AppendLine("			, " & Agro_SQL_SaveDate(AGRODATAFINE) & "  ")

            StrSQL.AppendLine(") ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, True) & " QUERY: " & StrSQL.ToString.Replace(vbCrLf, " ")
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return xRisp

    End Function


    '#################################################################
    Public Function CancellaTuttiIDettagliDiNC(ByVal xFiltroAggiuntivo As String, _
                             ByVal ID_NC As Integer, _
                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                             ) As Boolean

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCorePannelloDiControlloDAL.NC_Dettagli_W.CancellaTuttiIDettagliDiNC()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            StrSQL.Length = 0

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then
                'StrSQL.Append(" UPDATE ... ")
                'StrSQL.Append(" SET ")
                'StrSQL.Append("         Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                'StrSQL.Append("         ,Data_Modifica= " & Agro_SQL_SaveDate(Date.Now) & " ")
                'StrSQL.Append("         ,Inviato = -1 ")
                'StrSQL.Append(" WHERE   1=1 ")
                'StrSQL.Append(" AND     Inviato >= 0 ")
            Else
                StrSQL.AppendLine(" DELETE FROM NC_Dettagli ")
                StrSQL.AppendLine(" WHERE PivaSuperUser = " & Agro_SQL_SaveText_NULL(objParametri.PivaSuperUser))
                StrSQL.AppendLine("	AND ID_NC =		" & Agro_SQL_SaveNum_NULL(ID_NC))

            End If
            '---------------------------------------------

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, True) & " QUERY: " & StrSQL.ToString.Replace(vbCrLf, " ")
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp

    End Function

    '#################################################################
    Public Function CancellaDettaglio(ByVal xFiltroAggiuntivo As String, _
                             ByVal ID_Dettaglio As Integer, _
                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                             ) As Boolean

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCorePannelloDiControlloDAL.NC_Dettagli_W.CancellaDettaglio()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            StrSQL.Length = 0

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then
                'StrSQL.Append(" UPDATE ... ")
                'StrSQL.Append(" SET ")
                'StrSQL.Append("         Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                'StrSQL.Append("         ,Data_Modifica= " & Agro_SQL_SaveDate(Date.Now) & " ")
                'StrSQL.Append("         ,Inviato = -1 ")
                'StrSQL.Append(" WHERE   1=1 ")
                'StrSQL.Append(" AND     Inviato >= 0 ")
            Else
                StrSQL.AppendLine(" DELETE FROM NC_Dettagli ")
                StrSQL.AppendLine(" WHERE PivaSuperUser = " & Agro_SQL_SaveText_NULL(objParametri.PivaSuperUser))
                StrSQL.AppendLine("	AND ID_Dettaglio =		" & Agro_SQL_SaveNum_NULL(ID_Dettaglio))

            End If
            '---------------------------------------------

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, True) & " QUERY: " & StrSQL.ToString.Replace(vbCrLf, " ")
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp

    End Function

End Class
