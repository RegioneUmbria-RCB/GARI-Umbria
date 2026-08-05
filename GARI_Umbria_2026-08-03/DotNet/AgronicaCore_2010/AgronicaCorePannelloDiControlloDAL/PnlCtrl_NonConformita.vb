Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.AgronicaCoreParametri

Public Class NonConformita_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function Leggi_NonConformita( _
                                    ByVal xFiltroAggiuntivo As String, _
                                    ByVal xOrderBy As String, _
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                    ) As DataTable

        ' Qui dovrei recuperare l'id dell'audit
        Return Leggi_NonConformita(Nothing, Nothing, Nothing, Nothing, _
                                   xFiltroAggiuntivo, xOrderBy, objParametri)

    End Function

    '##############################################################################################
    Public Function Leggi_NonConformita( _
                                    ByVal ID_NC As Integer?, _
                                    ByVal xFiltroAggiuntivo As String, _
                                    ByVal xOrderBy As String, _
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                    ) As DataTable

        ' Qui dovrei recuperare l'id dell'audit
        Return Leggi_NonConformita(ID_NC, Nothing, Nothing, Nothing, _
                                   xFiltroAggiuntivo, xOrderBy, objParametri)

    End Function


    '##############################################################################################
    Public Function Leggi_NonConformita( _
                                        ByVal ID_NC As Integer?, _
                                        ByVal ID_Categoria As Integer?, _
                                        ByVal Piva As String, _
                                        ByVal ID_Gravita As Integer?, _
                                        ByVal xFiltroAggiuntivo As String, _
                                        ByVal xOrderBy As String, _
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                        ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCorePannelloDiControlloDAL.PnlCtrl_NonConformita_R.Leggi_NonConformita"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim strSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            strSQL.Length = 0

            strSQL.Append(" SELECT ID_Dettaglio, PivaSuperUser, ID_NC, ID_Categoria, Piva, Utente, Data, ID_Stato, ID_ListaAllegati, ID_Gravita, Descrizione, TabDettaglio_Nome, TabDettaglio_Chiave, Note " + vbCrLf)
            strSQL.Append(" FROM PnlCtrl_NonConformita ")

            strSQL.Append(" WHERE PivaSuperUser = " & Agro_SQL_SaveText_NULL(objParametri.PivaSuperUser) + vbCrLf)

            If Not IsNothing(ID_NC) Then
                strSQL.Append(" AND ID_NC = " & Agro_SQL_SaveNum_NULL(ID_NC))
            End If

            If Not IsNothing(ID_Categoria) Then
                strSQL.Append(" AND ID_Categoria = " & Agro_SQL_SaveNum_NULL(ID_Categoria))
            End If

            If Not IsNothing(Piva) Then
                strSQL.Append(" AND Piva = " & Agro_SQL_SaveText_NULL(Piva))
            End If

            If Not IsNothing(ID_Gravita) Then
                strSQL.Append(" AND ID_Gravita = " & Agro_SQL_SaveNum_NULL(ID_Gravita))
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


End Class

Public Class NonConformita_W
    Inherits AgronicaCoreDataProvider.DataProvider

    'ho tolto ID_NC perché non si può cambiare l'appartenenza ad una non conformità
    '##############################################################################################
    Public Function Modifica(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                            ByVal Old_ID_NC As Integer, _
                            ByVal New_ID_Categoria As Integer, _
                            ByVal New_Piva As String, _
                            ByVal New_ID_Gravita As Integer?, _
                            ByVal New_TabDettaglio_Nome As String, _
                            ByVal New_TabDettaglio_Chiave As String, _
                            Optional ByVal Data_modifica As Date = #2/1/1900#, _
                            Optional ByVal username_modifica As String = "" _
                            ) As Boolean


        Dim NomeRoutine As String = "AgronicaCorePannelloDiControlloDAL.PnlCtrl_NonConformita_W.Modifica()"

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
            StrSQL.Append(" UPDATE PnlCtrl_NonConformita SET" + vbCrLf)

            StrSQL.Append(" ID_Categoria = " & Agro_SQL_SaveNum_NULL(New_ID_Categoria) & vbCrLf)
            StrSQL.Append(", Piva = " & Agro_SQL_SaveText_NULL(New_Piva) & vbCrLf)

            StrSQL.Append(", ID_Gravita  = " & Agro_SQL_SaveNum_NULL(NothingToDBNull(New_ID_Gravita)) & vbCrLf)
            StrSQL.Append(", TabDettaglio_Nome = " & Agro_SQL_SaveText_NULL(NothingToDBNull(New_TabDettaglio_Nome)) & vbCrLf)
            StrSQL.Append(", TabDettaglio_Chiave = " & Agro_SQL_SaveText_NULL(NothingToDBNull(New_TabDettaglio_Chiave)) & vbCrLf)

            StrSQL.Append(", Data_Modifica = " & Agro_SQL_SaveDateTime(Data_modifica) & vbCrLf)
            StrSQL.Append(", UserName_Modifica = '" & Agro_SQL_SaveText(username_modifica) & "' " & vbCrLf)

            StrSQL.Append("	WHERE PivaSuperUser = " & Agro_SQL_SaveText_NULL(objParametri.PivaSuperUser))
            StrSQL.Append("	AND ID_NC  =		" & Agro_SQL_SaveNum_NULL(Old_ID_NC))


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
                            ByVal ID_NC As Integer, _
                            ByVal ID_Categoria As Integer, _
                            ByVal Piva As String, _
                            ByVal ID_Gravita As Integer?, _
                            ByVal TabDettaglio_Nome As String, _
                            ByVal TabDettaglio_Chiave As String, _
                            Optional ByVal Data_creazione As Date = #2/1/1900#, _
                            Optional ByVal Data_modifica As Date = #2/1/1900#, _
                            Optional ByVal username_creazione As String = "", _
                            Optional ByVal username_modifica As String = "" _
                            ) As Boolean


        Dim NomeRoutine As String = "AgronicaCorePannelloDiControlloDAL.PnlCtrl_NonConformita_W.Scrivi()"

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
            StrSQL.Append(" INSERT INTO  PnlCtrl_NonConformita" + vbCrLf)

            StrSQL.Append("              (")
            StrSQL.Append("              PivaSuperUser,                 ID_NC, ")
            StrSQL.Append("              ID_Categoria,                  Piva, ")
            StrSQL.Append("              TabDettaglio_Nome,             TabDettaglio_Chiave, ")
            StrSQL.Append("              ID_Gravita,                    ")

            StrSQL.Append("              Inviato,            datainvio, ")
            StrSQL.Append("              Data_Creazione,     Data_Modifica, ")
            StrSQL.Append("              UserName_Creazione, UserName_Modifica, ")
            StrSQL.Append("              Validita_Inizio,    Validita_Fine " + vbCrLf)
            StrSQL.Append("              ) ")

            StrSQL.Append(" VALUES ( ")

            StrSQL.Append("			 " & Agro_SQL_SaveText_NULL(objParametri.PivaSuperUser))
            StrSQL.Append("			," & Agro_SQL_SaveNum_NULL(ID_NC))
            StrSQL.Append("			," & Agro_SQL_SaveNum_NULL(ID_Categoria))
            StrSQL.Append("			," & Agro_SQL_SaveText_NULL(Piva))
            StrSQL.Append("			," & Agro_SQL_SaveText_NULL(NothingToDBNull(TabDettaglio_Nome)))
            StrSQL.Append("			," & Agro_SQL_SaveText_NULL(NothingToDBNull(TabDettaglio_Chiave)))
            StrSQL.Append("			," & Agro_SQL_SaveNum_NULL(NothingToDBNull(ID_Gravita)))

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
    Public Function CancellaNonConformita(ByVal xFiltroAggiuntivo As String, _
                             ByVal ID_NC As Integer, _
                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                             ) As Boolean

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCorePannelloDiControlloDAL.PnlCtrl_NonConformita_W.CancellaNonConformita()"

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
                StrSQL.Append(" DELETE FROM PnlCtrl_NonConformita ")
                StrSQL.Append(" WHERE PivaSuperUser = " & Agro_SQL_SaveText_NULL(objParametri.PivaSuperUser))
                StrSQL.Append("	AND ID_NC =		" & Agro_SQL_SaveNum_NULL(ID_NC))

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
