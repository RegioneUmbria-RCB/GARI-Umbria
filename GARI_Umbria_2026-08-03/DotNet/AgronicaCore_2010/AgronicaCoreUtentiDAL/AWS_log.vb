Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider

Public Class AWS_log_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Scrivi(ByVal PivaSuperUser As String,
                         ByVal SuperUser_Username As String,
                         ByVal SuperUser_Password As String,
                         ByVal Utente_Username As String,
                         ByVal Utente_Password As String,
                         ByVal IP_Richiedente As String,
                         ByVal Applicazione_Richiedente As Int32,
                         ByVal Funzione_Richiesta As Int32,
                         ByVal Url_Richiesto As String,
                         ByVal Risposta_Richiesta As Int32,
                         ByVal Risposta_Errore As String,
                         ByVal Data_Richiesta As Date,
                         ByVal Veg_Cod As Int32,
                         ByVal Dpi_Cod As Int32,
                         ByVal Flag_DPI_Privato_Pubblico As Int32,
                         ByVal ID_RCDPI As Int32,
                         ByVal Grfi_Cod As Int32,
                         ByVal Flag_Protetto As Int32,
                         ByVal Tipo_Testata As Int32,
                         ByVal Tipo_Richiesto As Int32,
                         ByVal Testo_Ricerca As String,
                         ByVal StrPa As String,
                         ByVal Av_Cod As Int32,
                         ByVal Av_Gru As Int32,
                         ByVal StrSort As String,
                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                Optional ByVal Regione_Cod As String = "",
                                Optional ByVal Veg_Cod_Agea As String = "",
                                Optional ByVal Cul_Cod_Agea As String = "",
                                Optional ByVal Fr_Cod As Int32 = 0,
                                Optional ByVal For_Veg_Av_Cod As Int32 = 0,
                                Optional ByVal str_Fr_Cod As String = "",
                                Optional ByVal str_Tipi_Richiesti As String = ""
                                    ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreUtentiDAL.AWS_log_W.Scrivi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.AppendLine("INSERT INTO AWS_log (PivaSuperUser, ")
            StrSQL.AppendLine("                    SuperUser_Username, ")
            StrSQL.AppendLine("                    SuperUser_Password, ")
            StrSQL.AppendLine("                    Utente_Username, ")
            StrSQL.AppendLine("                    Utente_Password, ")
            StrSQL.AppendLine("                    IP_Richiedente, ")
            StrSQL.AppendLine("                    Applicazione_Richiedente, ")
            StrSQL.AppendLine("                    Funzione_Richiesta, ")
            StrSQL.AppendLine("                    Url_Richiesto, ")
            StrSQL.AppendLine("                    Risposta_Richiesta, ")
            StrSQL.AppendLine("                    Risposta_Errore, ")
            StrSQL.AppendLine("                    Data_Richiesta, ")
            StrSQL.AppendLine("                    Veg_Cod, ")
            StrSQL.AppendLine("                    DPI_Cod, ")
            StrSQL.AppendLine("                    Flag_DPI_Privato_Pubblico, ")
            StrSQL.AppendLine("                    ID_RCDPI, ")
            StrSQL.AppendLine("                    Grfi_Cod, ")
            StrSQL.AppendLine("                    Flag_Protetto, ")
            StrSQL.AppendLine("                    Tipo_Testata, ")
            StrSQL.AppendLine("                    Tipo_Richiesto, ")
            StrSQL.AppendLine("                    Testo_Ricerca, ")
            StrSQL.AppendLine("                    StrPA, ")
            StrSQL.AppendLine("                    Av_Cod, ")
            StrSQL.AppendLine("                    Av_Gru, ")
            StrSQL.AppendLine("                    StrSort, ")

            StrSQL.AppendLine("                    Regione_Cod, ")
            StrSQL.AppendLine("                    Veg_Cod_Agea, ")
            StrSQL.AppendLine("                    Cul_Cod_Agea, ")
            StrSQL.AppendLine("                    Fr_Cod, ")
            StrSQL.AppendLine("                    For_Veg_Av_Cod, ")
            StrSQL.AppendLine("                    str_Fr_Cod, ")
            StrSQL.AppendLine("                    str_Tipi_Richiesti, ")

            StrSQL.AppendLine("                    inviato,  ")
            StrSQL.AppendLine("                    datainvio ")
            StrSQL.AppendLine("                    ) ")

            '(12/01/2021 fede) eliminato salvataggio per nuovo utilizzo password hash
            StrSQL.AppendLine("VALUES (")
            StrSQL.AppendLine("          '" & Agro_SQL_SaveText(PivaSuperUser) & "' ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(SuperUser_Username) & "' ")
            'StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(SuperUser_Password) & "' ")
            StrSQL.AppendLine("         ,'' ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(Utente_Username) & "' ")
            'StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(Utente_Password) & "' ")
            StrSQL.AppendLine("         ,'' ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(IP_Richiedente) & "' ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Applicazione_Richiedente) & " ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Funzione_Richiesta) & " ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(Url_Richiesto) & "' ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Risposta_Richiesta) & " ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(Risposta_Errore) & "' ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDateTime(Data_Richiesta) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Veg_Cod) & " ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Dpi_Cod) & " ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Flag_DPI_Privato_Pubblico) & " ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(ID_RCDPI) & " ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Grfi_Cod) & " ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Flag_Protetto) & " ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Tipo_Testata) & " ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Tipo_Richiesto) & " ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(Testo_Ricerca) & "' ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(StrPa) & "' ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Av_Cod) & " ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Av_Gru) & " ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(StrSort) & "' ")

            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(Regione_Cod) & "' ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(Veg_Cod_Agea) & "' ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(Cul_Cod_Agea) & "' ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Fr_Cod) & " ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(For_Veg_Av_Cod) & " ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(str_Fr_Cod) & "' ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(str_Tipi_Richiesti) & "' ")

            StrSQL.AppendLine("         , 0 ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDate(Date.Now) & "  ")
            StrSQL.AppendLine(")")
            '---------------------------------------------

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

    Public Function Aggiorna(ByVal PivaSuperUser As String,
                         ByVal SuperUser_Username As String,
                         ByVal SuperUser_Password As String,
                         ByVal Utente_Username As String,
                         ByVal Utente_Password As String,
                         ByVal IP_Richiedente As String,
                         ByVal Applicazione_Richiedente As Int32,
                         ByVal Funzione_Richiesta As Int32,
                         ByVal Url_Richiesto As String,
                         ByVal Risposta_Richiesta As Int32,
                         ByVal Risposta_Errore As String,
                         ByVal Data_Richiesta As Date,
                         ByVal Veg_Cod As Int32,
                         ByVal Dpi_Cod As Int32,
                         ByVal Flag_DPI_Privato_Pubblico As Int32,
                         ByVal ID_RCDPI As Int32,
                         ByVal Grfi_Cod As Int32,
                         ByVal Flag_Protetto As Int32,
                         ByVal Tipo_Testata As Int32,
                         ByVal Tipo_Richiesto As Int32,
                         ByVal Testo_Ricerca As String,
                         ByVal StrPa As String,
                         ByVal Av_Cod As Int32,
                         ByVal Av_Gru As Int32,
                         ByVal StrSort As String,
                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                Optional ByVal Regione_Cod As String = "",
                                Optional ByVal Veg_Cod_Agea As String = "",
                                Optional ByVal Cul_Cod_Agea As String = "",
                                Optional ByVal Fr_Cod As Int32 = 0,
                                Optional ByVal For_Veg_Av_Cod As Int32 = 0,
                                Optional ByVal str_Fr_Cod As String = "",
                                Optional ByVal str_Tipi_Richiesti As String = ""
                                    ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreUtentiDAL.AWS_log_W.Aggiorna()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.AppendLine(" UPDATE AWS_log  set   ")
            StrSQL.AppendLine("                    ")
            StrSQL.AppendLine("                    Utente_Password=  '" & Agro_SQL_SaveText(Utente_Password) & "' ")
            StrSQL.AppendLine("                    ,IP_Richiedente=   '" & Agro_SQL_SaveText(IP_Richiedente) & "' ")
            StrSQL.AppendLine("                    ,Applicazione_Richiedente=  " & Agro_SQL_SaveNum(Applicazione_Richiedente) & " ")
            StrSQL.AppendLine("                    ,Funzione_Richiesta=   " & Agro_SQL_SaveNum(Funzione_Richiesta) & " ")
            StrSQL.AppendLine("                    ,Url_Richiesto=     '" & Agro_SQL_SaveText(Url_Richiesto) & "' ")
            StrSQL.AppendLine("                    ,Risposta_Richiesta=     " & Agro_SQL_SaveNum(Risposta_Richiesta) & " ")
            StrSQL.AppendLine("                    ,Risposta_Errore=     '" & Agro_SQL_SaveText(Risposta_Errore) & "' ")

            If Data_Richiesta <> AgronicaCoreDataProvider.CostantiPersonalizzate.AGRODATAINIZIO Then
                StrSQL.AppendLine("                    ,Data_Richiesta=       " & Agro_SQL_SaveDateTime(Data_Richiesta) & "  ")
            End If


            StrSQL.AppendLine("                    ,Veg_Cod=    " & Agro_SQL_SaveNum(Veg_Cod) & " ")
            StrSQL.AppendLine("                    ,DPI_Cod=          " & Agro_SQL_SaveNum(Dpi_Cod) & " ")
            StrSQL.AppendLine("                    ,Flag_DPI_Privato_Pubblico=    " & Agro_SQL_SaveNum(Flag_DPI_Privato_Pubblico) & " ")
            StrSQL.AppendLine("                    ,ID_RCDPI=    " & Agro_SQL_SaveNum(ID_RCDPI) & " ")
            StrSQL.AppendLine("                    ,Grfi_Cod=   " & Agro_SQL_SaveNum(Grfi_Cod) & " ")
            StrSQL.AppendLine("                    ,Flag_Protetto=   " & Agro_SQL_SaveNum(Flag_Protetto) & " ")
            StrSQL.AppendLine("                    ,Tipo_Testata=   " & Agro_SQL_SaveNum(Tipo_Testata) & " ")
            StrSQL.AppendLine("                    ,Tipo_Richiesto=    " & Agro_SQL_SaveNum(Tipo_Richiesto) & " ")
            StrSQL.AppendLine("                    ,Testo_Ricerca=   '" & Agro_SQL_SaveText(Testo_Ricerca) & "' ")
            StrSQL.AppendLine("                    ,StrPA=    '" & Agro_SQL_SaveText(StrPa) & "' ")
            StrSQL.AppendLine("                    ,Av_Cod=    " & Agro_SQL_SaveNum(Av_Cod) & " ")
            StrSQL.AppendLine("                    ,Av_Gru=     " & Agro_SQL_SaveNum(Av_Gru) & " ")
            StrSQL.AppendLine("                    ,StrSort=  '" & Agro_SQL_SaveText(StrSort) & "' ")

            StrSQL.AppendLine("                    ,Regione_Cod=       '" & Agro_SQL_SaveText(Regione_Cod) & "' ")
            StrSQL.AppendLine("                    ,Veg_Cod_Agea=   '" & Agro_SQL_SaveText(Veg_Cod_Agea) & "' ")
            StrSQL.AppendLine("                    ,Cul_Cod_Agea=  '" & Agro_SQL_SaveText(Cul_Cod_Agea) & "' ")
            StrSQL.AppendLine("                    ,Fr_Cod=   " & Agro_SQL_SaveNum(Fr_Cod) & " ")
            StrSQL.AppendLine("                    ,For_Veg_Av_Cod=        " & Agro_SQL_SaveNum(For_Veg_Av_Cod) & " ")
            StrSQL.AppendLine("                    ,str_Fr_Cod=  '" & Agro_SQL_SaveText(str_Fr_Cod) & "' ")
            StrSQL.AppendLine("                    ,str_Tipi_Richiesti=  '" & Agro_SQL_SaveText(str_Tipi_Richiesti) & "' ")

            StrSQL.AppendLine("                    ,inviato=        0 ")
            StrSQL.AppendLine("                    ,datainvio =    " & Agro_SQL_SaveDate(Date.Now) & "  ")
            StrSQL.AppendLine("                    ,data_modifica =    " & Agro_SQL_SaveDateTime(Date.Now) & "  ")

            StrSQL.AppendLine(" where Utente_Username=  '" & Agro_SQL_SaveText(Utente_Username) & "' ")



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


Public Class AWS_log_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi(ByVal xFiltroAggiuntivo As String,
                                        ByVal xOrderBy As String,
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                        ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreUtentiDAL.AWS_log_R.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   Username_Utente
        '   FiltroAggiuntivo
        '
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0

            ' VAnni: 27/5/2021: seleziono i soli campi richiesti e non * per tenere sotto controllo le colonne include nell'indice creato in web_utenti
            StrSQL.AppendLine(" SELECT data_richiesta, data_modifica ")
            StrSQL.AppendLine(" FROM    AWS_log (NOLOCK)")
            StrSQL.AppendLine(" WHERE 1=1 ")

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND  " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
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

    Public Function Num_Accessi_SuperUser(ByVal xFiltroAggiuntivo As String,
                                            ByVal xOrderBy As String,
                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                            ) As Integer

        Dim NomeRoutine As String = "AgronicaCoreUtentiDAL.AWS_log_R.Num_Accessi_SuperUser()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim N_Acc As Integer = 0

        Try

            StrSQL.Length = 0
            StrSQL.AppendLine(" SELECT count(*) as ConteggioAccessi ")
            StrSQL.AppendLine(" FROM    AWS_log ")
            StrSQL.AppendLine(" WHERE 1=1 ")

            StrSQL.AppendLine(" AND  SuperUser_Username ='" & objParametri.SuperUserUsername & "'")

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND  " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            If Not DT Is Nothing Then
                N_Acc = DT.Rows(0)("ConteggioAccessi")
            End If

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return N_Acc

    End Function

    Public Function Num_Accessi_Utente(ByVal xFiltroAggiuntivo As String,
                                         ByVal xOrderBy As String,
                                         ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                         ) As Integer

        Dim NomeRoutine As String = "AgronicaCoreUtentiDAL.AWS_log_R.Num_Accessi_Utente()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim N_Acc As Integer = 0

        Try

            StrSQL.Length = 0
            StrSQL.AppendLine(" SELECT count(*) as ConteggioAccessi ")
            StrSQL.AppendLine(" FROM    AWS_log ")
            StrSQL.AppendLine(" WHERE 1=1 ")

            StrSQL.AppendLine(" AND  SuperUser_Username ='" & objParametri.SuperUserUsername & "'")
            StrSQL.AppendLine(" AND  Utente_Username ='" & objParametri.UtenteUsername & "'")


            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND  " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            If Not DT Is Nothing Then
                N_Acc = DT.Rows(0)("ConteggioAccessi")
            End If

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return N_Acc

    End Function

    Public Function UltimoLoggato(ByVal xFiltroAggiuntivo As String,
                                             ByVal xOrderBy As String,
                                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                             ) As DateTime

        Dim NomeRoutine As String = "AgronicaCoreUtentiDAL.AWS_log_R.UltimoLoggato()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim N_Acc As DateTime = #1/1/1900#

        Try

            StrSQL.Length = 0
            StrSQL.AppendLine(" SELECT top 1 Data_Creazione ")
            StrSQL.AppendLine(" FROM    AWS_log ")
            StrSQL.AppendLine(" WHERE 1=1 ")

            StrSQL.AppendLine(" AND  SuperUser_Username ='" & objParametri.SuperUserUsername & "'")
            StrSQL.AppendLine(" AND  Utente_Username ='" & objParametri.UtenteUsername & "'")


            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND  " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            StrSQL.AppendLine(" ORDER BY Data_Creazione DESC " & vbCrLf)

            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            If Not DT Is Nothing AndAlso DT.Rows.Count > 0 Then
                N_Acc = DT.Rows(0)("Data_Creazione")
            End If

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return N_Acc

    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="xOrderBy"></param>
    ''' <param name="objParametri"></param>
    ''' <returns>Una DataTable contentente le informazioni relative </returns>
    Public Function LeggiTutti(ByVal xFiltroAggiuntivo As String, ByVal xOrderBy As String,
                               ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreUtentiDAL.AWS_log_R.LeggiTutti()"

        '====================================================================================
        'Parametri opzionali :
        '   xFiltroAggiuntivo
        '   xOrderBy
        '
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0

            ' VAnni: 27/5/2021: seleziono i soli campi richiesti e non * per tenere sotto controllo le colonne include nell'indice creato in web_utenti
            StrSQL.AppendLine(" SELECT ")
            StrSQL.AppendLine("         Utente_Username, Cognome, Nome, ")
            StrSQL.AppendLine("         MAX(Data_Richiesta) as Data_UltimoAccesso, count(*) as Numero_Accessi ")
            StrSQL.AppendLine(" FROM    AWS_log ")
            StrSQL.AppendLine(" JOIN Utenti_dettagli ON Utente_Username = username ")

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND  " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            StrSQL.AppendLine(" GROUP BY Utente_Username, Cognome, Nome ")

            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
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


    Public Function LeggiMinMaxTutti(ByVal xFiltroAggiuntivo As String, ByVal xOrderBy As String,
                               ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreUtentiDAL.AWS_log_R.LeggiMinMaxTutti()"

        '====================================================================================
        'Parametri opzionali :
        '   xFiltroAggiuntivo
        '   xOrderBy
        '
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT ")
            StrSQL.AppendLine("         Utente_Username, Cognome, Nome, CodFisc, MIN(AWS_log.Data_Creazione) as Data_PrimoAccesso, MAX(AWS_log.Data_Modifica) as Data_UltimoAccesso ")
            StrSQL.AppendLine(" FROM    AWS_log ")
            StrSQL.AppendLine(" JOIN Utenti_dettagli ON Utente_Username = username ")
            StrSQL.AppendLine(" WHERE 1=1 ")

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND  " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            StrSQL.AppendLine(" GROUP BY Utente_Username, Cognome, Nome, CodFisc ")

            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
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



End Class