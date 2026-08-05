Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider

Public Class NC_Testata_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function Leggi(
                            ByVal xFiltroAggiuntivo As String,
                            ByVal xOrderBy As String,
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                            ) As DataTable

        ' Qui dovrei recuperare l'id dell'audit
        Return Leggi(Nothing, Nothing, Nothing, Nothing, Nothing,
                     Nothing, Nothing, Nothing, Nothing, Nothing,
                     xFiltroAggiuntivo, xOrderBy, objParametri)

    End Function

    '##############################################################################################
    Public Function Leggi(
                            ByVal ID_NC As Integer,
                            ByVal xFiltroAggiuntivo As String,
                            ByVal xOrderBy As String,
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                            ) As DataTable

        ' Qui dovrei recuperare l'id dell'audit
        Return Leggi(ID_NC, Nothing, Nothing, Nothing, Nothing,
                     Nothing, Nothing, Nothing, Nothing, Nothing,
                     xFiltroAggiuntivo, xOrderBy, objParametri)

    End Function


    '##############################################################################################
    Public Function Leggi(
                            ByVal ID_NC As Integer?,
                            ByVal ID_Categoria As Integer?,
                            ByVal Piva As String,
                            ByVal ID_Gravita As Integer?,
                            ByVal CodiceNC As String,
                            ByVal Data_Apertura As DateTime?,
                            ByVal Data_Chiusura As DateTime?,
                            ByVal ID_CausaChiusura As Integer?,
                            ByVal ID_Rischio As Integer?,
                            ByVal ID_LineaImpianto As Integer?,
                            ByVal xFiltroAggiuntivo As String,
                            ByVal xOrderBy As String,
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                            ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCorePannelloDiControlloDAL.NC_Testata_R.Leggi"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim strSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            strSQL.Length = 0

            strSQL.AppendLine(" SELECT ID_NC, PivaSuperUser, Piva, CodiceNC, ID_Categoria, Descrizione, Note, ID_Gravita, ")
            strSQL.AppendLine(" Data_Apertura, Data_Chiusura, ID_CausaChiusura, ID_Rischio, CodiceProdotto, ")
            strSQL.AppendLine(" DescrizioneProdotto, LineaProdotti, LottoJDE, DataArrivoProdotto, LottoFruttagel, LottoFornitore, ")
            strSQL.AppendLine(" Bloccato, QtaBloccati_UdM, QtaBloccati_Qta, QtaBloccati_Progressivi, Fornitore, ID_LineaImpianto, Utente, ")
            strSQL.AppendLine(" TipoNC_Codice, TipoNC_Chiave ")

            strSQL.AppendLine(" FROM NC_Testata ")

            strSQL.AppendLine(" WHERE PivaSuperUser = " & Agro_SQL_SaveText_NULL(objParametri.PivaSuperUser) + vbCrLf)

            If Not IsNothing(ID_NC) Then
                strSQL.AppendLine(" AND ID_NC = " & Agro_SQL_SaveNum_NULL(ID_NC))
            End If

            If Not IsNothing(ID_Categoria) Then
                strSQL.AppendLine(" AND ID_Categoria = " & Agro_SQL_SaveNum_NULL(ID_Categoria))
            End If

            If Not IsNothing(Piva) Then
                strSQL.AppendLine(" AND Piva = " & Agro_SQL_SaveText_NULL(Piva))
            End If

            If Not IsNothing(ID_Gravita) Then
                strSQL.AppendLine(" AND ID_Gravita = " & Agro_SQL_SaveNum_NULL(ID_Gravita))
            End If

            If Not IsNothing(CodiceNC) Then
                strSQL.AppendLine(" AND CodiceNC = " & Agro_SQL_SaveText_NULL(CodiceNC))
            End If

            If Not IsNothing(Data_Apertura) Then
                strSQL.AppendLine(" AND Data_Apertura = " & Agro_SQL_SaveDateTime_NULL(Data_Apertura))
            End If

            If Not IsNothing(Data_Chiusura) Then
                strSQL.AppendLine(" AND Data_Chiusura = " & Agro_SQL_SaveDateTime_NULL(Data_Chiusura))
            End If

            If Not IsNothing(ID_CausaChiusura) Then
                strSQL.AppendLine(" AND ID_CausaChiusura = " & Agro_SQL_SaveNum_NULL(ID_CausaChiusura))
            End If

            If Not IsNothing(ID_Rischio) Then
                strSQL.AppendLine(" AND ID_Rischio = " & Agro_SQL_SaveNum_NULL(ID_Rischio))
            End If

            If Not IsNothing(ID_LineaImpianto) Then
                strSQL.AppendLine(" AND ID_LineaImpianto = " & Agro_SQL_SaveNum_NULL(ID_LineaImpianto))
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

    Public Function VerificaEsisteGiaCodiceNC(codiceNC As String, ID_NC As Integer, objParametri As AgronicaCoreParametri) As Boolean

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCorePannelloDiControlloDAL.NC_Testata_R.Leggi"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim strSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim result As Boolean = True

        Try

            strSQL.Length = 0

            strSQL.AppendLine(" SELECT ID_NC, PivaSuperUser, Piva, CodiceNC ")
            strSQL.AppendLine(" FROM NC_Testata ")

            strSQL.AppendLine(" WHERE PivaSuperUser = " & Agro_SQL_SaveText_NULL(objParametri.PivaSuperUser))
            strSQL.AppendLine(" AND CodiceNC = " & Agro_SQL_SaveText_NULL(codiceNC))

            If Not IsNothing(ID_NC) Then
                strSQL.AppendLine(" AND ID_NC <> " & Agro_SQL_SaveNum_NULL(ID_NC))
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

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, strSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            result = If(Not IsNothing(DT) AndAlso DT.Rows.Count > 0, True, False)

        Catch ex As Exception
            MessaggioErrore = AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, True) & " QUERY: " & strSQL.ToString.Replace(vbCrLf, " ")
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return result

    End Function
End Class

Public Class NC_Testata_W
    Inherits AgronicaCoreDataProvider.DataProvider

    'ho tolto ID_NC perché non si può cambiare l'appartenenza ad una non conformità
    '##############################################################################################
    Public Function Modifica(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                            ByVal Old_ID_NC As Integer?,
                            ByVal New_ID_Categoria As Integer?,
                            ByVal New_Piva As String,
                            ByVal New_ID_Gravita As Integer?,
                            ByVal New_CodiceNC As String,
                            ByVal New_Descrizione As String,
                            ByVal New_Note As String,
                            ByVal New_Data_Apertura As DateTime?,
                            ByVal New_Data_Chiusura As DateTime?,
                            ByVal New_ID_CausaChiusura As Integer?,
                            ByVal New_ID_Rischio As Integer?,
                            ByVal New_CodiceProdotto As String,
                            ByVal New_DescrizioneProdotto As String,
                            ByVal New_LineaProdotti As String,
                            ByVal New_LottoJDE As String,
                            ByVal New_DataArrivoProdotto As DateTime?,
                            ByVal New_LottoFruttagel As String,
                            ByVal New_LottoFornitore As String,
                            ByVal New_Bloccato As Integer?,
                            ByVal New_QtaBloccati_UdM As String,
                            ByVal New_QtaBloccati_Qta As Decimal?,
                            ByVal New_QtaBloccati_Progressivi As String,
                            ByVal New_Fornitore As String,
                            ByVal New_ID_LineaImpianto As Integer?,
                            ByVal New_Utente As String,
                            ByVal New_TipoNC_Codice As Integer?,
                            ByVal New_TipoNC_Chiave As String,
                            Optional ByVal Data_modifica As Date = #2/1/1900#,
                            Optional ByVal username_modifica As String = ""
                            ) As Boolean


        Dim NomeRoutine As String = "AgronicaCorePannelloDiControlloDAL.NC_Testata_W.Modifica()"

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
            StrSQL.AppendLine(" UPDATE NC_Testata SET ")

            StrSQL.AppendLine(" ID_Categoria = " & Agro_SQL_SaveNum_NULL(New_ID_Categoria))
            StrSQL.AppendLine(", Piva = " & Agro_SQL_SaveText_NULL(New_Piva))

            StrSQL.AppendLine(", ID_Gravita  = " & Agro_SQL_SaveNum_NULL(NothingToDBNull(New_ID_Gravita)))

            StrSQL.AppendLine(", CodiceNC  = " & Agro_SQL_SaveText_NULL(NothingToDBNull(New_CodiceNC)))
            StrSQL.AppendLine(", Descrizione  = " & Agro_SQL_SaveText_NULL(NothingToDBNull(New_Descrizione)))
            StrSQL.AppendLine(", Note  = " & Agro_SQL_SaveText_NULL(NothingToDBNull(New_Note)))
            StrSQL.AppendLine(", Data_Apertura  = " & Agro_SQL_SaveDateTime_NULL(NothingToDBNull(New_Data_Apertura)))
            StrSQL.AppendLine(", Data_Chiusura  = " & Agro_SQL_SaveDateTime_NULL(NothingToDBNull(New_Data_Chiusura)))
            StrSQL.AppendLine(", ID_CausaChiusura  = " & Agro_SQL_SaveNum_NULL(NothingToDBNull(New_ID_CausaChiusura)))
            StrSQL.AppendLine(", ID_Rischio  = " & Agro_SQL_SaveNum_NULL(NothingToDBNull(New_ID_Rischio)))
            StrSQL.AppendLine(", CodiceProdotto  = " & Agro_SQL_SaveText_NULL(NothingToDBNull(New_CodiceProdotto)))
            StrSQL.AppendLine(", DescrizioneProdotto  = " & Agro_SQL_SaveText_NULL(NothingToDBNull(New_DescrizioneProdotto)))
            StrSQL.AppendLine(", LineaProdotti  = " & Agro_SQL_SaveText_NULL(NothingToDBNull(New_LineaProdotti)))
            StrSQL.AppendLine(", LottoJDE  = " & Agro_SQL_SaveText_NULL(NothingToDBNull(New_LottoJDE)))
            StrSQL.AppendLine(", DataArrivoProdotto  = " & Agro_SQL_SaveDateTime_NULL(NothingToDBNull(New_DataArrivoProdotto)))
            StrSQL.AppendLine(", LottoFruttagel  = " & Agro_SQL_SaveText_NULL(NothingToDBNull(New_LottoFruttagel)))
            StrSQL.AppendLine(", LottoFornitore  = " & Agro_SQL_SaveText_NULL(NothingToDBNull(New_LottoFornitore)))
            StrSQL.AppendLine(", Bloccato  = " & Agro_SQL_SaveNum_NULL(NothingToDBNull(New_Bloccato)))
            StrSQL.AppendLine(", QtaBloccati_UdM  = " & Agro_SQL_SaveText_NULL(NothingToDBNull(New_QtaBloccati_UdM)))
            StrSQL.AppendLine(", QtaBloccati_Qta  = " & Agro_SQL_SaveNum_NULL(NothingToDBNull(New_QtaBloccati_Qta)))
            StrSQL.AppendLine(", QtaBloccati_Progressivi  = " & Agro_SQL_SaveText_NULL(NothingToDBNull(New_QtaBloccati_Progressivi)))
            StrSQL.AppendLine(", Fornitore  = " & Agro_SQL_SaveText_NULL(NothingToDBNull(New_Fornitore)))
            StrSQL.AppendLine(", ID_LineaImpianto  = " & Agro_SQL_SaveNum_NULL(NothingToDBNull(New_ID_LineaImpianto)))
            StrSQL.AppendLine(", Utente  = " & Agro_SQL_SaveText_NULL(NothingToDBNull(New_Utente)))
            StrSQL.AppendLine(", TipoNC_Codice  = " & Agro_SQL_SaveNum_NULL(NothingToDBNull(New_TipoNC_Codice)))
            StrSQL.AppendLine(", TipoNC_Chiave  = " & Agro_SQL_SaveText_NULL(NothingToDBNull(New_TipoNC_Chiave)))

            StrSQL.AppendLine(", Data_Modifica = " & Agro_SQL_SaveDateTime(Data_modifica))
            StrSQL.AppendLine(", UserName_Modifica = '" & Agro_SQL_SaveText(username_modifica) & "' ")

            StrSQL.AppendLine("	WHERE PivaSuperUser = " & Agro_SQL_SaveText_NULL(objParametri.PivaSuperUser))
            StrSQL.AppendLine("	AND ID_NC  =		" & Agro_SQL_SaveNum_NULL(Old_ID_NC))


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
                            ByVal ID_NC As Integer,
                            ByVal ID_Categoria As Integer?,
                            ByVal Piva As String,
                            ByVal ID_Gravita As Integer?,
                            ByVal CodiceNC As String,
                            ByVal Descrizione As String,
                            ByVal Note As String,
                            ByVal Data_Apertura As DateTime?,
                            ByVal Data_Chiusura As DateTime?,
                            ByVal ID_CausaChiusura As Integer?,
                            ByVal ID_Rischio As Integer?,
                            ByVal CodiceProdotto As String,
                            ByVal DescrizioneProdotto As String,
                            ByVal LineaProdotti As String,
                            ByVal LottoJDE As String,
                            ByVal DataArrivoProdotto As DateTime?,
                            ByVal LottoFruttagel As String,
                            ByVal LottoFornitore As String,
                            ByVal Bloccato As Integer?,
                            ByVal QtaBloccati_UdM As String,
                            ByVal QtaBloccati_Qta As Decimal?,
                            ByVal QtaBloccati_Progressivi As String,
                            ByVal Fornitore As String,
                            ByVal ID_LineaImpianto As Integer?,
                            ByVal Utente As String,
                            ByVal TipoNC_Codice As Integer?,
                            ByVal TipoNC_Chiave As String,
                            Optional ByVal Data_creazione As Date = #2/1/1900#,
                            Optional ByVal Data_modifica As Date = #2/1/1900#,
                            Optional ByVal username_creazione As String = "",
                            Optional ByVal username_modifica As String = ""
                            ) As Boolean


        Dim NomeRoutine As String = "AgronicaCorePannelloDiControlloDAL.NC_Testata_W.Scrivi()"

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
            StrSQL.AppendLine(" INSERT INTO  NC_Testata" + vbCrLf)

            StrSQL.AppendLine("              (")
            StrSQL.AppendLine("              PivaSuperUser,                 ID_NC, ")
            StrSQL.AppendLine("              ID_Categoria,                  Piva, ")
            StrSQL.AppendLine("              ID_Gravita,                    CodiceNC, ")
            StrSQL.AppendLine("              Descrizione,                   Note, ")
            StrSQL.AppendLine("              Data_Apertura,                 Data_Chiusura, ")
            StrSQL.AppendLine("              ID_CausaChiusura,              ID_Rischio, ")
            StrSQL.AppendLine("              CodiceProdotto,                DescrizioneProdotto, ")
            StrSQL.AppendLine("              LineaProdotti,                 LottoJDE, ")
            StrSQL.AppendLine("              DataArrivoProdotto,            LottoFruttagel, ")
            StrSQL.AppendLine("              LottoFornitore,                Bloccato, ")
            StrSQL.AppendLine("              QtaBloccati_UdM,               QtaBloccati_Qta, ")
            StrSQL.AppendLine("              QtaBloccati_Progressivi,       Fornitore, ")
            StrSQL.AppendLine("              ID_LineaImpianto,              Utente, ")
            StrSQL.AppendLine("              TipoNC_Codice,                 TipoNC_Chiave, ")

            StrSQL.AppendLine("              Inviato,            datainvio, ")
            StrSQL.AppendLine("              Data_Creazione,     Data_Modifica, ")
            StrSQL.AppendLine("              UserName_Creazione, UserName_Modifica, ")
            StrSQL.AppendLine("              Validita_Inizio,    Validita_Fine " + vbCrLf)
            StrSQL.AppendLine("              ) ")

            StrSQL.AppendLine(" VALUES ( ")

            StrSQL.AppendLine("			 " & Agro_SQL_SaveText_NULL(objParametri.PivaSuperUser))
            StrSQL.AppendLine("			," & Agro_SQL_SaveNum_NULL(ID_NC))
            StrSQL.AppendLine("			," & Agro_SQL_SaveNum_NULL(NothingToDBNull(ID_Categoria)))
            StrSQL.AppendLine("			," & Agro_SQL_SaveText_NULL(NothingToDBNull(Piva)))
            StrSQL.AppendLine("			," & Agro_SQL_SaveNum_NULL(NothingToDBNull(ID_Gravita)))

            StrSQL.AppendLine("			," & Agro_SQL_SaveText_NULL(NothingToDBNull(CodiceNC)))
            StrSQL.AppendLine("			," & Agro_SQL_SaveText_NULL(NothingToDBNull(Descrizione)))
            StrSQL.AppendLine("			," & Agro_SQL_SaveText_NULL(NothingToDBNull(Note)))
            StrSQL.AppendLine("			," & Agro_SQL_SaveDateTime_NULL(NothingToDBNull(Data_Apertura)))
            StrSQL.AppendLine("			," & Agro_SQL_SaveDateTime_NULL(NothingToDBNull(Data_Chiusura)))
            StrSQL.AppendLine("			," & Agro_SQL_SaveNum_NULL(NothingToDBNull(ID_CausaChiusura)))
            StrSQL.AppendLine("			," & Agro_SQL_SaveNum_NULL(NothingToDBNull(ID_Rischio)))
            StrSQL.AppendLine("			," & Agro_SQL_SaveText_NULL(NothingToDBNull(CodiceProdotto)))
            StrSQL.AppendLine("			," & Agro_SQL_SaveText_NULL(NothingToDBNull(DescrizioneProdotto)))
            StrSQL.AppendLine("			," & Agro_SQL_SaveText_NULL(NothingToDBNull(LineaProdotti)))
            StrSQL.AppendLine("			," & Agro_SQL_SaveText_NULL(NothingToDBNull(LottoJDE)))
            StrSQL.AppendLine("			," & Agro_SQL_SaveDateTime_NULL(NothingToDBNull(DataArrivoProdotto)))
            StrSQL.AppendLine("			," & Agro_SQL_SaveText_NULL(NothingToDBNull(LottoFruttagel)))
            StrSQL.AppendLine("			," & Agro_SQL_SaveText_NULL(NothingToDBNull(LottoFornitore)))
            StrSQL.AppendLine("			," & Agro_SQL_SaveNum_NULL(NothingToDBNull(Bloccato)))
            StrSQL.AppendLine("			," & Agro_SQL_SaveText_NULL(NothingToDBNull(QtaBloccati_UdM)))
            StrSQL.AppendLine("			," & Agro_SQL_SaveNum_NULL(NothingToDBNull(QtaBloccati_Qta)))
            StrSQL.AppendLine("			," & Agro_SQL_SaveText_NULL(NothingToDBNull(QtaBloccati_Progressivi)))
            StrSQL.AppendLine("			," & Agro_SQL_SaveText_NULL(NothingToDBNull(Fornitore)))
            StrSQL.AppendLine("			," & Agro_SQL_SaveNum_NULL(NothingToDBNull(ID_LineaImpianto)))
            StrSQL.AppendLine("			," & Agro_SQL_SaveText_NULL(NothingToDBNull(Utente)))
            StrSQL.AppendLine("			," & Agro_SQL_SaveNum_NULL(NothingToDBNull(TipoNC_Codice)))
            StrSQL.AppendLine("			," & Agro_SQL_SaveText_NULL(NothingToDBNull(TipoNC_Chiave)))


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
    Public Function CancellaNonConformita(ByVal xFiltroAggiuntivo As String, _
                             ByVal ID_NC As Integer, _
                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                             ) As Boolean

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCorePannelloDiControlloDAL.NC_Testata_W.CancellaNonConformita()"

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
                StrSQL.AppendLine(" DELETE FROM NC_Testata ")
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

End Class
