Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.AgronicaCoreParametri

Public Class Scadenze_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function Leggi_Scadenze( _
                                        ByVal ID As Integer?, _
                                        ByVal ID_Categoria As Integer?, _
                                        ByVal Piva As String, _
                                        ByVal Utente As String, _
                                        ByVal DataScadenza As DateTime?, _
                                        ByVal ID_ListaAllegati As Integer?, _
                                        ByVal xFiltroAggiuntivo As String, _
                                        ByVal xOrderBy As String, _
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                    ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCorePannelloDiControlloDAL.PnlCtrl_Scadenze_R.Leggi_NonConformita"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim strSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            strSQL.Length = 0

            strSQL.Append(" SELECT ID, PivaSuperUser, ID_Categoria, Piva, Utente, DataScadenza, ID_ListaAllegati, ID_Gravita, Descrizione, TabDettaglio_Nome, TabDettaglio_Chiave, Note, Ricorrenza, PreavvisoInfo, PreavvisoWarning, PreavvisoError " + vbCrLf)
            strSQL.Append(" FROM PnlCtrl_Scadenze ")

            strSQL.Append(" WHERE PivaSuperUser = " & Agro_SQL_SaveText_NULL(objParametri.PivaSuperUser) + vbCrLf)

            If Not IsNothing(ID) Then
                strSQL.Append(" AND ID = " & Agro_SQL_SaveNum_NULL(ID))
            End If

            If Not IsNothing(ID_Categoria) Then
                strSQL.Append(" AND ID_Categoria = " & Agro_SQL_SaveNum_NULL(ID_Categoria))
            End If

            If Not IsNothing(Piva) Then
                strSQL.Append(" AND Piva = " & Agro_SQL_SaveText_NULL(Piva))
            End If

            If Not IsNothing(Utente) Then
                strSQL.Append(" AND Utente = " & Agro_SQL_SaveText_NULL(Utente))
            End If

            If Not IsNothing(DataScadenza) Then
                strSQL.Append(" AND DataScadenza = " & Agro_SQL_SaveDateTime_NULL(DataScadenza))
            End If

            If Not IsNothing(ID_ListaAllegati) Then
                strSQL.Append(" AND ID_ListaAllegati = " & Agro_SQL_SaveNum_NULL(ID_ListaAllegati))
            End If

            If xFiltroAggiuntivo <> "" Then
                strSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    strSQL.Append(" AND   Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    strSQL.Append(" AND   Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                strSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
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


    '##############################################################################################
    Public Function Leggi_Scadenze_Patentino_DaAggiungereInPnlCtrl( _
                                        ByVal xFiltroAggiuntivo As String, _
                                        ByVal xOrderBy As String, _
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                        ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCorePannelloDiControlloDAL.PnlCtrl_Scadenze_R.Leggi_Scadenze_Patentino_DaAggiungereInPnlCtrl"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim strSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            strSQL.Length = 0

            strSQL.Append(" SELECT c.Piva, c.Cod_Contatto, c.Nome, c.Cognome, ru.Patentino, ru.Data_Rilascio_Patentino, ru.Data_Scadenza_Patentino, ru.Ente_di_rilascio  " & vbCrLf)

            strSQL.Append(" FROM Risorse_Umane ru   " & vbCrLf)
            strSQL.Append(" INNER JOIN Contatti c  " & vbCrLf)
            strSQL.Append(" ON c.Cod_Contatto = ru.Cod_Contatto   " & vbCrLf)
            strSQL.Append(" AND c.Piva = ru.Piva   " & vbCrLf)

            strSQL.Append(" WHERE(ru.Patentino IS NOT NULL)  " & vbCrLf)
            strSQL.Append(" AND LTRIM(RTRIM(ru.Patentino)) <> '' " & vbCrLf)
            strSQL.Append(" AND CONCAT('Piva=', c.Piva, '~Cod_Contatto=', c.Cod_Contatto) NOT IN " & vbCrLf)
            strSQL.Append(" ( " & vbCrLf)
            strSQL.Append("    SELECT s.TabDettaglio_Chiave  " & vbCrLf)

            strSQL.Append("    FROM PnlCtrl_Scadenze s " & vbCrLf)
            strSQL.Append("    INNER JOIN PnlCtrl_Categorie c " & vbCrLf)
            strSQL.Append("    ON s.PivaSuperUser = c.PivaSuperUser " & vbCrLf)
            strSQL.Append("    AND s.ID_Categoria = c.ID " & vbCrLf)

            strSQL.Append("	   WHERE s.PivaSuperUser = " & Agro_SQL_SaveText_NULL(objParametri.PivaSuperUser) & vbCrLf)
            strSQL.Append("    AND c.Area = 'Persone' " & vbCrLf)
            strSQL.Append("    AND c.Tipologia = 'Patentino' " & vbCrLf)
            strSQL.Append(" )")

            If xFiltroAggiuntivo <> "" Then
                strSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    strSQL.Append(" AND   ru.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    strSQL.Append(" AND   ru.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                strSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
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

    '##############################################################################################
    Public Function Leggi_Scadenze_Patentino_DaModficareInPnlCtrl( _
                                        ByVal xFiltroAggiuntivo As String, _
                                        ByVal xOrderBy As String, _
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                        ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCorePannelloDiControlloDAL.PnlCtrl_Scadenze_R.Leggi_Scadenze_Patentino_DaModficareInPnlCtrl"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim strSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            strSQL.Length = 0

            strSQL.Append(" SELECT e.ID, cnt.Cod_Contatto, cnt.Nome, cnt.Cognome, ru.Patentino, ru.Data_Rilascio_Patentino, ru.Data_Scadenza_Patentino, ru.Ente_di_rilascio  " & vbCrLf)

            strSQL.Append(" FROM Risorse_Umane ru   " & vbCrLf)
            strSQL.Append(" INNER JOIN Contatti cnt  " & vbCrLf)
            strSQL.Append(" ON cnt.Cod_Contatto = ru.Cod_Contatto   " & vbCrLf)
            strSQL.Append(" AND cnt.Piva = ru.Piva  " & vbCrLf)

            strSQL.Append(" INNER JOIN PnlCtrl_Scadenze s " & vbCrLf)
            strSQL.Append(" ON s.TabDettaglio_Chiave = CONCAT('Piva=', cnt.Piva, '~Cod_Contatto=', cnt.Cod_Contatto) " & vbCrLf)

            strSQL.Append(" INNER JOIN PnlCtrl_Categorie c " & vbCrLf)
            strSQL.Append(" ON s.PivaSuperUser = c.PivaSuperUser " & vbCrLf)
            strSQL.Append(" AND s.ID_Categoria = c.ID " & vbCrLf)

            strSQL.Append("	WHERE s.PivaSuperUser = " & Agro_SQL_SaveText_NULL(objParametri.PivaSuperUser) & vbCrLf)
            strSQL.Append(" AND ru.Patentino IS NOT NULL " & vbCrLf)
            strSQL.Append(" AND LTRIM(RTRIM(Patentino)) <> '' " & vbCrLf)
            strSQL.Append(" AND c.Area = 'Persone' " & vbCrLf)
            strSQL.Append(" AND c.Tipologia = 'Patentino' " & vbCrLf)
            strSQL.Append(" AND s.DataScadenza <> ru.Data_Scadenza_Patentino")


            If xFiltroAggiuntivo <> "" Then
                strSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    strSQL.Append(" AND   ru.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    strSQL.Append(" AND   ru.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                strSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
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

Public Class Scadenze_W
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function Modifica(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                            ByVal Old_ID As Integer, _
                            ByVal New_ID_Categoria As Integer, _
                            ByVal New_Piva As String, _
                            ByVal New_Utente As String, _
                            ByVal New_DataScadenza As DateTime, _
                            ByVal New_ID_ListaAllegati As Integer?, _
                            ByVal New_ID_Gravita As Integer?, _
                            ByVal New_Descrizione As String, _
                            ByVal New_TabDettaglio_Nome As String, _
                            ByVal New_TabDettaglio_Chiave As String, _
                            ByVal New_Note As String, _
                            ByVal New_Ricorrenza As String, _
                            ByVal New_PreavvisoInfo As Integer?, _
                            ByVal New_PreavvisoWarning As Integer?, _
                            ByVal New_PreavvisoError As Integer?, _
                            Optional ByVal Data_modifica As Date = #2/1/1900#, _
                            Optional ByVal username_modifica As String = "" _
                            ) As Boolean


        Dim NomeRoutine As String = "AgronicaCorePannelloDiControlloDAL.PnlCtrl_Scadenze_W.Modifica()"

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
            StrSQL.Append(" UPDATE PnlCtrl_Scadenze SET" + vbCrLf)

            StrSQL.Append(" ID_Categoria = " & Agro_SQL_SaveNum_NULL(New_ID_Categoria) & vbCrLf)
            StrSQL.Append(", Piva = " & Agro_SQL_SaveText_NULL(New_Piva) & vbCrLf)

            StrSQL.Append(", Utente = " & Agro_SQL_SaveText_NULL(New_Utente) & vbCrLf)
            StrSQL.Append(", DataScadenza = " & Agro_SQL_SaveDateTime(New_DataScadenza) & vbCrLf)

            StrSQL.Append(", ID_ListaAllegati  = " & Agro_SQL_SaveNum_NULL(NothingToDBNull(New_ID_ListaAllegati)) & vbCrLf)
            StrSQL.Append(", ID_Gravita  = " & Agro_SQL_SaveNum_NULL(NothingToDBNull(New_ID_Gravita)) & vbCrLf)
            StrSQL.Append(", Descrizione = " & Agro_SQL_SaveText_NULL(New_Descrizione) & vbCrLf)
            StrSQL.Append(", TabDettaglio_Nome = " & Agro_SQL_SaveText_NULL(NothingToDBNull(New_TabDettaglio_Nome)) & vbCrLf)
            StrSQL.Append(", TabDettaglio_Chiave = " & Agro_SQL_SaveText_NULL(NothingToDBNull(New_TabDettaglio_Chiave)) & vbCrLf)
            StrSQL.Append(", Note = " & Agro_SQL_SaveText_NULL(New_Note) & vbCrLf)
            StrSQL.Append(", Ricorrenza = " & Agro_SQL_SaveText_NULL(New_Ricorrenza) & vbCrLf)
            StrSQL.Append(", PreavvisoInfo = " & Agro_SQL_SaveNum_NULL(New_PreavvisoInfo) & vbCrLf)
            StrSQL.Append(", PreavvisoWarning = " & Agro_SQL_SaveNum_NULL(New_PreavvisoWarning) & vbCrLf)
            StrSQL.Append(", PreavvisoError = " & Agro_SQL_SaveNum_NULL(New_PreavvisoError) & vbCrLf)

            StrSQL.Append(", Data_Modifica = " & Agro_SQL_SaveDateTime(Data_modifica) & vbCrLf)
            StrSQL.Append(", UserName_Modifica = '" & Agro_SQL_SaveText(username_modifica) & "' " & vbCrLf)

            StrSQL.Append("	WHERE PivaSuperUser = " & Agro_SQL_SaveText_NULL(objParametri.PivaSuperUser))
            StrSQL.Append("	AND ID  =		" & Agro_SQL_SaveNum_NULL(Old_ID))


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
    Public Function Scrivi(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                            ByVal ID As Integer, _
                            ByVal ID_Categoria As Integer, _
                            ByVal Piva As String, _
                            ByVal Utente As String, _
                            ByVal DataScadenza As DateTime, _
                            ByVal ID_ListaAllegati As Integer?, _
                            ByVal ID_Gravita As Integer?, _
                            ByVal Descrizione As String, _
                            ByVal TabDettaglio_Nome As String, _
                            ByVal TabDettaglio_Chiave As String, _
                            ByVal Note As String, _
                            ByVal Ricorrenza As String, _
                            ByVal PreavvisoInfo As Integer?, _
                            ByVal PreavvisoWarning As Integer?, _
                            ByVal PreavvisoError As Integer?, _
                            Optional ByVal Data_creazione As Date = #2/1/1900#, _
                            Optional ByVal Data_modifica As Date = #2/1/1900#, _
                            Optional ByVal username_creazione As String = "", _
                            Optional ByVal username_modifica As String = "" _
                            ) As Boolean


        Dim NomeRoutine As String = "AgronicaCorePannelloDiControlloDAL.PnlCtrl_Scadenze_W.Scrivi()"

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
            StrSQL.Append(" INSERT INTO  PnlCtrl_Scadenze" + vbCrLf)

            StrSQL.Append("              (")
            StrSQL.Append("              PivaSuperUser,                 ")
            StrSQL.Append("              ID,                            ID_Categoria, ")
            StrSQL.Append("              Piva,                          Utente,  ")
            StrSQL.Append("              DataScadenza,                  ID_Stato, ")
            StrSQL.Append("              ID_ListaAllegati,              ID_Gravita, ")
            StrSQL.Append("              Descrizione,                   TabDettaglio_Nome, ")
            StrSQL.Append("              TabDettaglio_Chiave,           Note,  ")
            StrSQL.Append("              Ricorrenza,                    PreavvisoInfo,  ")
            StrSQL.Append("              PreavvisoWarning,              PreavvisoError,  ")


            StrSQL.Append("              Inviato,            datainvio, ")
            StrSQL.Append("              Data_Creazione,     Data_Modifica, ")
            StrSQL.Append("              UserName_Creazione, UserName_Modifica, ")
            StrSQL.Append("              Validita_Inizio,    Validita_Fine " + vbCrLf)
            StrSQL.Append("              ) ")

            StrSQL.Append(" VALUES ( ")

            StrSQL.Append("			 " & Agro_SQL_SaveText_NULL(objParametri.PivaSuperUser))
            StrSQL.Append("			," & Agro_SQL_SaveNum_NULL(ID))
            StrSQL.Append("			," & Agro_SQL_SaveNum_NULL(ID_Categoria))
            StrSQL.Append("			," & Agro_SQL_SaveText_NULL(Piva))
            StrSQL.Append("			," & Agro_SQL_SaveText_NULL(Utente))
            StrSQL.Append("			," & Agro_SQL_SaveDateTime(DataScadenza))
            StrSQL.Append("			," & Agro_SQL_SaveNum_NULL(NothingToDBNull(ID_ListaAllegati)))
            StrSQL.Append("			," & Agro_SQL_SaveNum_NULL(NothingToDBNull(ID_Gravita)))
            StrSQL.Append("			," & Agro_SQL_SaveText_NULL(NothingToDBNull(Descrizione)))
            StrSQL.Append("			," & Agro_SQL_SaveText_NULL(NothingToDBNull(TabDettaglio_Nome)))
            StrSQL.Append("			," & Agro_SQL_SaveText_NULL(NothingToDBNull(TabDettaglio_Chiave)))
            StrSQL.Append("			," & Agro_SQL_SaveText_NULL(NothingToDBNull(Note)))
            StrSQL.Append("			," & Agro_SQL_SaveText_NULL(NothingToDBNull(Ricorrenza)))
            StrSQL.Append("			," & Agro_SQL_SaveText_NULL(NothingToDBNull(PreavvisoInfo)))
            StrSQL.Append("			," & Agro_SQL_SaveText_NULL(NothingToDBNull(PreavvisoWarning)))
            StrSQL.Append("			," & Agro_SQL_SaveText_NULL(NothingToDBNull(PreavvisoError)))

            StrSQL.Append("         , 0  " + vbCrLf)
            StrSQL.Append("         , Null  " + vbCrLf)
            StrSQL.Append("			, " & Agro_SQL_SaveDateTime(Data_creazione) & "  ")
            StrSQL.Append("			, " & Agro_SQL_SaveDateTime(Data_modifica) & "  ")
            StrSQL.Append("			,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            StrSQL.Append("			,'" & Agro_SQL_SaveText(username_modifica) & "' ")
            StrSQL.Append("			, " & Agro_SQL_SaveDate(AGRODATAINIZIO) & "  ")
            StrSQL.Append("			, " & Agro_SQL_SaveDate(AGRODATAFINE) & "  ")

            StrSQL.Append(") ")

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
    Public Function Cancella(ByVal xFiltroAggiuntivo As String, _
                             ByVal ID As Integer, _
                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                             ) As Boolean

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCorePannelloDiControlloDAL.PnlCtrl_Scadenze_W.Cancella()"

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
                StrSQL.Append(" DELETE FROM PnlCtrl_Scadenze ")
                StrSQL.Append(" WHERE PivaSuperUser = " & Agro_SQL_SaveText_NULL(objParametri.PivaSuperUser))
                StrSQL.Append("	AND ID =		" & Agro_SQL_SaveNum_NULL(ID))

            End If
            '---------------------------------------------

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
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