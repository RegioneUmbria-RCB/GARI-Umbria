Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.AgronicaCoreParametri

Public Class AgeaAnagrafe_R
    Inherits AgronicaCoreDataProvider.DataProvider2010

    ''' <summary>
    ''' Legge un elenco di schede validazione da cache, restituisce un datatable
    ''' </summary>
    ''' <param name="CUAA"></param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="xOrderBy"></param>
    ''' <param name="objParametri"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function SchedeFascicoloCache( _
        ByVal EnteValidatore_COD As Integer, _
        ByVal DataDa As Date, _
        ByVal DataA As Date, _
        ByVal CUAA As String, _        
        ByVal xFiltroAggiuntivo As String, _
        ByVal xOrderBy As String, _
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
    ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgeaAnagrafe_R.Leggi()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Stb.Length = 0

            Stb.Append(" SELECT * " & vbCrLf)
            Stb.Append(" FROM FascicoliCache " & vbCrLf)
            Stb.Append(" Where cuaa =  '" & Agro_SQL_SaveText(CUAA) & "'" & vbCrLf)
            Stb.Append(" and  EnteValidatore_COD = " & EnteValidatore_COD & vbCrLf)

            If DataDa <> AGRODATAINIZIO Then
                Stb.Append(" and  Validazione_Data >= " & Agro_SQL_SaveDate(DataDa) & vbCrLf)
            End If

            If DataA <> AGRODATAFINE Then
                Stb.Append(" and  Validazione_Data <= " & Agro_SQL_SaveDate(DataA) & vbCrLf)
            End If



            If xFiltroAggiuntivo <> "" Then
                Stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    Stb.Append(" AND   Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    Stb.Append(" AND   Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                Stb.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception


            'uso questa funzione per ottenere il Messaggio..:
            MessaggioErrore = AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, True)

            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing

            'passo l'eccezione corrente al throw come inner exception ..:
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore, ex)


        End Try

        Return DT


    End Function



    ''' <summary>
    ''' Legge un xml da cache, restituisce un XDocument
    ''' </summary>
    ''' <param name="CUAA"></param>
    ''' <param name="Validazione_Numero"></param>    
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="xOrderBy"></param>
    ''' <param name="objParametri"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function LeggiCache(
        ByVal EnteValidatore_COD As Integer,
        ByVal CUAA As String,
        ByVal Validazione_Numero As String,
        ByVal xFiltroAggiuntivo As String,
        ByVal xOrderBy As String,
        ByVal parametriExtra As String,
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
        Optional Data_Validazione As Date = AGRODATAINIZIO
    ) As String

        '----- Descrizione
        Dim NomeRoutine As String = "AgeaAnagrafe_R.Leggi()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As String

        Try

            Stb.Length = 0

            Stb.Append(" SELECT Fascicolo " & vbCrLf)
            Stb.Append(" FROM FascicoliCache " & vbCrLf)
            Stb.Append(" Where cuaa =  '" & Agro_SQL_SaveText(CUAA) & "'" & vbCrLf)
            Stb.Append(" and  EnteValidatore_COD = " & EnteValidatore_COD & vbCrLf)


            If Validazione_Numero <> "" Then
                Stb.Append(" AND Validazione_Numero =  '" & Agro_SQL_SaveText(Validazione_Numero) & "'" & vbCrLf)
            End If

            If parametriExtra <> "" Then
                Stb.Append(" AND Parametri_Extra =  " + Agro_SQL_SaveText_NULL(parametriExtra) + "" & vbCrLf)
            End If

            If Data_Validazione <> AGRODATAINIZIO Then
                Stb.Append(" AND Validazione_Data =  " + Agro_SQL_SaveDate(Data_Validazione) + "" & vbCrLf)
            End If

            If xFiltroAggiuntivo <> "" Then
                Stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    Stb.Append(" AND   Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    Stb.Append(" AND   Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                Stb.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura_XML(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception


            'uso questa funzione per ottenere il Messaggio..:
            MessaggioErrore = AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, True)

            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing

            'passo l'eccezione corrente al throw come inner exception ..:
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore, ex)


        End Try

        Return DT


    End Function

    Public Function LeggiCacheDT(
        ByVal EnteValidatore_COD As Integer,
        ByVal CUAA As String,
        ByVal Validazione_Numero As String,
        ByVal xFiltroAggiuntivo As String,
        ByVal xOrderBy As String,
        ByVal parametriExtra As String,
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
        Optional Data_Validazione As Date = AGRODATAINIZIO
    ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgeaAnagrafe_R.Leggi()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Stb.Length = 0

            Stb.Append(" SELECT * " & vbCrLf)
            Stb.Append(" FROM FascicoliCache " & vbCrLf)
            Stb.Append(" Where cuaa =  '" & Agro_SQL_SaveText(CUAA) & "'" & vbCrLf)
            Stb.Append(" and  EnteValidatore_COD = " & EnteValidatore_COD & vbCrLf)


            If Validazione_Numero <> "" Then
                Stb.Append(" AND Validazione_Numero =  '" & Agro_SQL_SaveText(Validazione_Numero) & "'" & vbCrLf)
            End If

            If parametriExtra <> "" Then
                Stb.Append(" AND Parametri_Extra =  " + Agro_SQL_SaveText_NULL(parametriExtra) + "" & vbCrLf)
            End If

            If Data_Validazione <> AGRODATAINIZIO Then
                Stb.Append(" AND Data_Validazione =  " + Agro_SQL_SaveDate(Data_Validazione) + "" & vbCrLf)
            End If

            If xFiltroAggiuntivo <> "" Then
                Stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    Stb.Append(" AND   Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    Stb.Append(" AND   Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                Stb.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception


            'uso questa funzione per ottenere il Messaggio..:
            MessaggioErrore = AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, True)

            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing

            'passo l'eccezione corrente al throw come inner exception ..:
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore, ex)


        End Try

        Return DT


    End Function

    Public Function FascicoliDaCaricare( _
       ByVal top As Integer, _
       ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
       Optional EnteValidatore As Integer = 0, _
       Optional ValidazioneNumero As String = "" _
   ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgeaAnagrafe_R.Leggi()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Stb.Length = 0
            If top <> -1 Then
                Stb.Append(" SELECT Top " + CStr(top) + " Cuaa " & vbCrLf)
            Else
                Stb.Append(" SELECT Cuaa " & vbCrLf)
            End If
            Stb.Append(" FROM AggiornaFascicoli " & vbCrLf)
            Stb.Append(" WHERE 1=1 " & vbCrLf)
            Stb.Append(" AND importato = 0 " & vbCrLf)
            If EnteValidatore <> 0 Then
                Stb.Append(" AND EnteValidatore_COD = " + Agro_SQL_SaveNum(EnteValidatore) + " " & vbCrLf)
            End If
            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception


            'uso questa funzione per ottenere il Messaggio..:
            MessaggioErrore = AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, True)

            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing

            'passo l'eccezione corrente al throw come inner exception ..:
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore, ex)


        End Try

        Return DT


    End Function

    Public Function FascicoliDaCaricare_1(
       ByVal top As Integer,
       ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
   ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgeaAnagrafe_R.Leggi()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Stb.Length = 0
            If top <> -1 Then
                Stb.Append(" SELECT Top " + CStr(top) + " Cuaa " & vbCrLf)
            Else
                Stb.Append(" SELECT Cuaa " & vbCrLf)
            End If
            Stb.Append(" FROM __T_FascicoliDaCaricare " & vbCrLf)
            Stb.Append(" WHERE 1=1 " & vbCrLf)
            Stb.Append(" AND Importato = 0 " & vbCrLf)

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception


            'uso questa funzione per ottenere il Messaggio..:
            MessaggioErrore = AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, True)

            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing

            'passo l'eccezione corrente al throw come inner exception ..:
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore, ex)


        End Try

        Return DT


    End Function

    ''' <summary>
    ''' Legge un xml da cache, restituisce un XDocument
    ''' </summary>
    ''' <param name="EnteValidatore_COD"></param>
    ''' <param name="DataVerifica"></param>    
    ''' <param name="objParametri"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetCuaaModificato(
        ByVal EnteValidatore_COD As Integer,
        ByVal DataVerifica As Date,
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
    ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgeaAnagrafe_R.GetCuaaModificato()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Stb.Length = 0

            Stb.Append(" SELECT CUAA, Istat_Provincia, Istat_Comune " & vbCrLf)
            Stb.Append(" FROM FascicoliCache " & vbCrLf)
            Stb.Append(" Where EnteValidatore_COD = " & EnteValidatore_COD & vbCrLf)
            Stb.Append(" AND  Validazione_Data >= '" & Agro_SQL_SaveDate(DataVerifica) & "'" & vbCrLf)

            'Scrivi_LOG(objParametri, NomeRoutine, "SQL " + Stb.ToString)

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception


            'uso questa funzione per ottenere il Messaggio..:
            MessaggioErrore = AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, True)

            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing

            'passo l'eccezione corrente al throw come inner exception ..:
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore, ex)


        End Try

        Return DT


    End Function

End Class


'#################################################################
'#################################################################
'#################################################################

Public Class AgeaAnagrafe_W
    Inherits AgronicaCoreDataProvider.DataProvider



    ''' <summary>
    ''' Salva in Cache un documento XML
    ''' </summary>
    ''' <param name="CUAA"></param>
    ''' <param name="Validazione_Numero"></param>
    ''' <param name="Validazione_Data"></param>
    ''' <param name="Fascicolo"></param>
    ''' <param name="objParametri"></param>
    ''' <param name="Data_creazione"></param>
    ''' <param name="Data_modifica"></param>
    ''' <param name="username_creazione"></param>
    ''' <param name="username_modifica"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ScriviCache(
                    ByVal EnteValidatore_COD As Integer,
                    ByVal CUAA As String,
                    ByVal Validazione_Numero As String,
                    ByVal Validazione_Data As DateTime,
                    ByVal Fascicolo As String,
                    ByVal Validita_Inizio As DateTime,
                    ByVal Validita_Fine As DateTime,
                  ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                , Optional ByVal Data_creazione As Date = #2/1/1900# _
                , Optional ByVal Data_modifica As Date = #2/1/1900# _
                , Optional ByVal username_creazione As String = "" _
                , Optional ByVal username_modifica As String = ""
                ) As Boolean


        Dim NomeRoutine As String = "Agea_AnagrafeDAL.Scrivi()"

        '====================================================================================
        'Parametri opzionali :

        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
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



            '---------------------------------------------
            Stb.Length = 0
            Stb.Append(" INSERT FascicoliCache " + vbCrLf)

            Stb.Append("              (")
            Stb.Append("              EnteValidatore_COD, ")
            Stb.Append("              CUAA, ")
            Stb.Append("              Validazione_Numero, ")
            Stb.Append("              Validazione_Data, ")
            Stb.Append("              Fascicolo, ")
            Stb.Append("              Inviato,            datainvio, ")
            Stb.Append("              Data_Creazione,     Data_Modifica, ")
            Stb.Append("              UserName_Creazione, UserName_Modifica, ")
            Stb.Append("              Validita_Inizio,    Validita_Fine ")
            Stb.Append("              ) ")

            Stb.Append(" VALUES ( ")

            Stb.Append(" " & Agro_SQL_SaveNum(EnteValidatore_COD) & ", ")
            Stb.Append(" '" & Agro_SQL_SaveText(CUAA) & "', ")
            Stb.Append(" '" & Agro_SQL_SaveText(Validazione_Numero) & "', ")
            Stb.Append(" " & Agro_SQL_SaveDateTime(Validazione_Data) & ", ")
            Stb.Append(" " & Agro_SQL_SaveStringToXML(Fascicolo) & " ")

            Stb.Append("         , 0  " + vbCrLf)
            Stb.Append("         , Null  " + vbCrLf)

            Stb.Append("			, " & Agro_SQL_SaveDate(Data_creazione) & "  ")
            Stb.Append("			, " & Agro_SQL_SaveDate(Data_modifica) & "  ")
            Stb.Append("			,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            Stb.Append("			,'" & Agro_SQL_SaveText(username_modifica) & "' ")
            Stb.Append("			, " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            Stb.Append("			, " & Agro_SQL_SaveDate(Validita_Fine) & "  ")

            Stb.Append(") ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return xRisp

    End Function

    Public Function AggiornaCache(
                    ByVal EnteValidatore_COD As Integer,
                    ByVal CUAA As String,
                    ByVal Validazione_Numero As String,
                    ByVal Validazione_Data As DateTime,
                    ByVal Fascicolo As String,
                    ByVal Validita_Inizio As DateTime,
                    ByVal Validita_Fine As DateTime,
                  ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                , Optional ByVal Data_creazione As Date = #2/1/1900# _
                , Optional ByVal Data_modifica As Date = #2/1/1900# _
                , Optional ByVal username_creazione As String = "" _
                , Optional ByVal username_modifica As String = "" _
                , Optional ByVal Parametri_Extra As String = "" _
                , Optional ByVal Istat_provincia As String = "" _
                , Optional ByVal Istat_comune As String = "" _
                , Optional ByVal Stato As String = ""
                ) As Boolean


        Dim NomeRoutine As String = "Agea_AnagrafeDAL.Scrivi()"

        '====================================================================================
        'Parametri opzionali :

        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim Stb1 As New System.Text.StringBuilder
        Dim xRisp As Boolean = False
        Dim dt As DataTable

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

            Stb1.Append("Select COUNT(CUAA) " & vbCrLf)
            Stb1.Append(" FROM FascicoliCache  " & vbCrLf)
            Stb1.Append(" WHERE CUAA=" + Agro_SQL_SaveText_NULL(CUAA) + " " & vbCrLf)
            Stb1.Append(" AND EnteValidatore_COD = " + Agro_SQL_SaveNum_NULL(EnteValidatore_COD) + " " & vbCrLf)
            Stb1.Append(" AND Validazione_Numero = " + Agro_SQL_SaveText_NULL(Validazione_Numero) + " " & vbCrLf)
            Stb1.Append(" AND Validazione_Data = " + Agro_SQL_SaveDateTime_NULL(Validazione_Data) + " ")

            dt = EseguiQuery_Lettura(objParametri, Stb1.ToString, NomeRoutine)

            If CInt(dt.Rows(0)(0)) > 0 Then
                'Stb.Append("UPDATE FascicoliCache " & vbCrLf)
                'Stb.Append("SET Fascicolo =  " + Agro_SQL_SaveStringToXML(Fascicolo) + " , " & vbCrLf)
                'Stb.Append(" inviato =  " + Agro_SQL_SaveNum_NULL(0) + " , " & vbCrLf)
                'Stb.Append(" dataInvio =  NULL , " & vbCrLf)
                'Stb.Append(" Data_Creazione =  " + Agro_SQL_SaveDateTime_NULL(Data_creazione) + " , " & vbCrLf)
                'Stb.Append(" Data_Modifica =  " + Agro_SQL_SaveDateTime_NULL(Data_modifica) + " , " & vbCrLf)
                'Stb.Append(" Username_Creazione =  " + Agro_SQL_SaveText_NULL(username_creazione) + " , " & vbCrLf)
                'Stb.Append(" Username_Modifica =  " + Agro_SQL_SaveText_NULL(username_modifica) + " , " & vbCrLf)
                'Stb.Append(" Validita_Inizio =  " + Agro_SQL_SaveDateTime_NULL(Validita_Inizio) + " , " & vbCrLf)
                'Stb.Append(" Validita_Fine =  " + Agro_SQL_SaveDateTime_NULL(Validita_Fine) + " , " & vbCrLf)
                'Stb.Append(" Parametri_Extra =  " + Agro_SQL_SaveText_NULL(Parametri_Extra) + " " & vbCrLf)
                'Stb.Append(" WHERE CUAA = " + Agro_SQL_SaveText_NULL(CUAA) + " ")
                'Stb.Append(" AND EnteValidatore_COD = " + Agro_SQL_SaveNum_NULL(EnteValidatore_COD) + " " & vbCrLf)
                'Stb.Append(" AND Validazione_Numero = " + Agro_SQL_SaveNum_NULL(Validazione_Numero) + " " & vbCrLf)
                'Stb.Append(" AND Validazione_Data = " + Agro_SQL_SaveDateTime_NULL(Validazione_Data) + " " & vbCrLf)
            Else
                Stb.Append("INSERT INTO [dbo].[FascicoliCache] " & vbCrLf)
                Stb.Append("            ([EnteValidatore_COD],[CUAA],[Validazione_Numero],[Validazione_Data],[Fascicolo],[inviato],[datainvio],[Data_Creazione], " & vbCrLf)
                Stb.Append("           [Data_Modifica],[Username_Creazione],[Username_Modifica],[Validita_Inizio],[Validita_Fine],[FascicoloComune],[Parametri_Extra], " & vbCrLf)
                Stb.Append("           [Istat_Provincia],[Istat_Comune],[Stato]) " & vbCrLf)
                Stb.Append("                 VALUES " & vbCrLf)
                Stb.Append("            (" + Agro_SQL_SaveNum_NULL(EnteValidatore_COD) + " " & vbCrLf)
                Stb.Append("            ," + Agro_SQL_SaveText_NULL(CUAA) + " " & vbCrLf)
                Stb.Append("            ," + Agro_SQL_SaveText_NULL(Validazione_Numero) + " " & vbCrLf)
                Stb.Append("            ," + Agro_SQL_SaveDateTime_NULL(Validazione_Data) + " " & vbCrLf)
                Stb.Append("            ," + Agro_SQL_SaveStringToXML(Fascicolo) + " " & vbCrLf)
                Stb.Append("            ," + Agro_SQL_SaveNum_NULL(0) + " " & vbCrLf)
                Stb.Append("            ,NULL " & vbCrLf)
                Stb.Append("            ," + Agro_SQL_SaveDateTime_NULL(Data_creazione) + " " & vbCrLf)
                Stb.Append("            ," + Agro_SQL_SaveDateTime_NULL(Data_modifica) + " " & vbCrLf)
                Stb.Append("            ," + Agro_SQL_SaveText_NULL(username_creazione) + " " & vbCrLf)
                Stb.Append("            ," + Agro_SQL_SaveText_NULL(username_modifica) + " " & vbCrLf)
                Stb.Append("            ," + Agro_SQL_SaveDateTime_NULL(Validita_Inizio) + " " & vbCrLf)
                Stb.Append("            ," + Agro_SQL_SaveDateTime_NULL(Validita_Fine) + " " & vbCrLf)
                Stb.Append("            ,NULL " & vbCrLf)
                Stb.Append("            ," + Agro_SQL_SaveText_NULL(Parametri_Extra) + " " & vbCrLf)
                Stb.Append("            ," + Agro_SQL_SaveText_NULL(Istat_provincia) + " " & vbCrLf)
                Stb.Append("            ," + Agro_SQL_SaveText_NULL(Istat_comune) + " " & vbCrLf)
                Stb.Append("            ," + Agro_SQL_SaveText_NULL(Stato) + " )" & vbCrLf)
            End If

            If Stb.ToString <> "" Then

                '--------------------------------------------------------------------------
                xRisp = EseguiQuery_Scrittura(objParametri, Stb.ToString, NomeRoutine)
                '--------------------------------------------------------------------------

            End If

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return xRisp

    End Function

    Public Function scriviImportaFascicoli(ByVal data As Date,
                                           ByVal EnteValidatore As Integer,
                                           ByVal numeroFascicoliCaricati As Integer,
                                           ByVal numeroErrori As Integer,
                                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
    ) As Boolean
        Dim NomeRoutine As String = "Agea_AnagrafeDAL.scriviImportaFascicoli()"

        '====================================================================================
        'Parametri opzionali :

        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim Stb1 As New System.Text.StringBuilder
        Dim dt As New DataTable
        Dim xRisp As Boolean = False
        Dim xRisp1 As Boolean = False

        Try
            '---------------------------------------------
            Stb.Length = 0
            Stb1.Length = 0
            Stb.Append(" SELECT * FROM ImportaFascicoli " + vbCrLf)
            Stb.Append(" WHERE Data=" + Agro_SQL_SaveDate(data) + " " + vbCrLf)
            Stb.Append(" AND EnteValidatore=" + Agro_SQL_SaveNum(EnteValidatore) + " " + vbCrLf)

            dt = EseguiQuery_Lettura(objParametri, Stb.ToString, NomeRoutine)

            If dt.Rows.Count > 0 Then
                'Aggiorno riga
                Stb1.Append("UPDATE ImportaFascicoli " & vbCrLf)
                Stb1.Append(" SET numeroFascicoliCaricati=" + Agro_SQL_SaveNum(CStr(numeroFascicoliCaricati)) + ", numeroErrori=" + Agro_SQL_SaveNum(CStr(numeroErrori)) + " " & vbCrLf)
                Stb1.Append(" WHERE Data = " + Agro_SQL_SaveDate(data) + " ")
                Stb1.Append(" AND EnteValidatore=" + Agro_SQL_SaveNum(EnteValidatore) + " " + vbCrLf)

            Else
                'Inserisco nuova riga
                Stb1.Append("INSERT INTO ImportaFascicoli " & vbCrLf)
                Stb1.Append(" (Data, numeroFascicoliCaricati, numeroErrori, EnteValidatore) " & vbCrLf)
                Stb1.Append(" VALUES (" + Agro_SQL_SaveDate(data) + ", " + Agro_SQL_SaveNum(CStr(numeroFascicoliCaricati)) + ", " + Agro_SQL_SaveNum(CStr(numeroErrori)) + " , " + Agro_SQL_SaveNum(CStr(EnteValidatore)) + ")")
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, Stb1.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return xRisp
    End Function

    Public Function scriviFascicoloComune(ByVal EnteValidatore As Integer,
                                          ByVal Cuaa As String,
                                          ByVal validazioneNumero As Integer,
                                          ByVal validazioneData As DateTime,
                                          ByVal xml As String,
                                          ByVal inviato As Integer,
                                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                            ) As Boolean
        Dim NomeRoutine As String = "Agea_AnagrafeDAL.scriviFascicoloComune()"

        '====================================================================================
        'Parametri opzionali :

        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim Stb1 As New System.Text.StringBuilder
        Dim dt As New DataTable
        Dim xRisp As Boolean = False
        Dim xRisp1 As Boolean = False

        Try
            '---------------------------------------------
            Stb.Length = 0
            Stb1.Length = 0
            Stb.Append(" UPDATE FascicoliCache " + vbCrLf)
            Stb.Append(" SET FascicoloComune=" + Agro_SQL_SaveStringToXML(xml) + " " + vbCrLf)
            Stb.Append(" ,inviato=" + Agro_SQL_SaveNum_NULL(inviato) + " " + vbCrLf)
            Stb.Append(" WHERE EnteValidatore_COD=" + Agro_SQL_SaveNum_NULL(EnteValidatore) + " " + vbCrLf)
            Stb.Append(" AND CUAA=" + Agro_SQL_SaveText_NULL(Cuaa) + " " + vbCrLf)
            Stb.Append(" AND Validazione_Numero=" + Agro_SQL_SaveText_NULL(CStr(validazioneNumero)) + " " + vbCrLf)

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return xRisp
    End Function



    '#################################################################
    Public Function Cancella(ByVal xFiltroAggiuntivo As String,
                              ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                              ) As Boolean

        '----- Descrizione
        Dim NomeRoutine As String = "Cancella()"

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            Stb.Length = 0

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then
                Stb.Append(" UPDATE ... ")
                Stb.Append(" SET ")
                Stb.Append("         Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                Stb.Append("         ,Data_Modifica= " & Agro_SQL_SaveDate(Date.Now) & " ")
                Stb.Append("         ,Inviato = -1 ")
                Stb.Append(" WHERE   1=1 ")
                Stb.Append(" AND     Inviato >= 0 ")
            Else
                Stb.Append(" DELETE FROM ... ")
                Stb.Append(" WHERE 1=1 ")
            End If
            '---------------------------------------------

            If xFiltroAggiuntivo <> "" Then
                Stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp

    End Function

    Function scriviErrore(Tabella As String, Cuaa As String, importato As Integer,
                          ObjParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                          ) As Boolean
        Dim NomeRoutine As String = "Agea_AnagrafeDAL.scriviErrore()"

        '====================================================================================
        'Parametri opzionali :

        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim Stb1 As New System.Text.StringBuilder
        Dim dt As New DataTable
        Dim xRisp As Boolean = False
        Dim xRisp1 As Boolean = False

        Try
            '---------------------------------------------
            Stb.Length = 0
            Stb.Append("UPDATE " + Tabella + " " & vbCrLf)
            Stb.Append(" SET importato = " + Agro_SQL_SaveNum_NULL(importato) + " " & vbCrLf)
            Stb.Append(" WHERE CUAA=" + Agro_SQL_SaveText_NULL(Cuaa) + " ")


            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(ObjParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(ObjParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return xRisp
    End Function

    Sub AggiornaDataValidazione(EnteValidatore_Cod As Integer, cuaa As String, Validazione_Numero As String, Validazione_Data As Date, objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri)
        Dim NomeRoutine As String = "Agea_AnagrafeDAL.Scrivi()"

        '====================================================================================
        'Parametri opzionali :

        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim Stb1 As New System.Text.StringBuilder
        Dim xRisp As Boolean = False
        Dim dt As DataTable

        Try


            Stb1.Append("Select COUNT(CUAA) " & vbCrLf)
            Stb1.Append(" FROM FascicoliCache  " & vbCrLf)
            Stb1.Append(" WHERE CUAA=" + Agro_SQL_SaveText_NULL(cuaa) + " " & vbCrLf)
            Stb1.Append(" AND EnteValidatore_COD = " + Agro_SQL_SaveNum_NULL(EnteValidatore_Cod) + " " & vbCrLf)
            Stb1.Append(" AND Validazione_Numero = " + Agro_SQL_SaveNum_NULL(Validazione_Numero) + " " & vbCrLf)

            dt = EseguiQuery_Lettura(objParametri, Stb1.ToString, NomeRoutine)

            If CInt(dt.Rows(0)(0)) > 0 Then
                Stb.Append("UPDATE FascicoliCache " & vbCrLf)
                Stb.Append("SET Validazione_Data =  " + Agro_SQL_SaveDateTime_NULL(Validazione_Data) + " " & vbCrLf)
                Stb.Append(" WHERE CUAA = " + Agro_SQL_SaveText_NULL(cuaa) + " ")
                Stb.Append(" AND EnteValidatore_COD = " + Agro_SQL_SaveNum_NULL(EnteValidatore_Cod) + " " & vbCrLf)
                Stb.Append(" AND Validazione_Numero = " + Agro_SQL_SaveNum_NULL(Validazione_Numero) + " " & vbCrLf)
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try
    End Sub


    'Inizio Funzione per modifica dati sui fascicoli già importati
    Public Function ModificaPerAllineamentoFascicoliCache(
                    ByVal EnteValidatore_COD As Integer,
                    ByVal CUAA As String,
                    ByVal Validazione_Numero As String,
                    ByVal Validazione_Data As DateTime,
                    ByVal Istat_Provincia As String,
                    ByVal Istat_comune As String,
                    ByVal Stato As String,
                  ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                ) As Boolean


        Dim NomeRoutine As String = "Agea_AnagrafeDAL.Scrivi()"

        '====================================================================================
        'Parametri opzionali :

        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim Stb1 As New System.Text.StringBuilder
        Dim xRisp As Boolean = False
        Dim dt As DataTable

        Try



            'If Data_modifica = #2/1/1900# Then
            '    Data_modifica = Date.Now
            'End If

            'If username_modifica = "" Then
            '    username_modifica = objParametri.UsernameOperazione
            'End If

            Stb1.Append("Select COUNT(CUAA) " & vbCrLf)
            Stb1.Append(" FROM FascicoliCache  " & vbCrLf)
            Stb1.Append(" WHERE CUAA=" + Agro_SQL_SaveText_NULL(CUAA) + " " & vbCrLf)
            Stb1.Append(" AND EnteValidatore_COD = " + Agro_SQL_SaveNum_NULL(EnteValidatore_COD) + " " & vbCrLf)
            Stb1.Append(" AND Validazione_Numero = " + Agro_SQL_SaveText_NULL(Validazione_Numero) + " " & vbCrLf)
            Stb1.Append(" AND Validazione_Data = " + Agro_SQL_SaveDateTime_NULL(Validazione_Data) + " ")

            dt = EseguiQuery_Lettura(objParametri, Stb1.ToString, NomeRoutine)

            If CInt(dt.Rows(0)(0)) > 0 Then
                Stb.Append("UPDATE FascicoliCache " & vbCrLf)
                'Stb.Append(" Data_Modifica =  " + Agro_SQL_SaveDateTime_NULL(Data_modifica) + " , " & vbCrLf)
                'Stb.Append(" Username_Modifica =  " + Agro_SQL_SaveText_NULL(username_modifica) + " , " & vbCrLf)
                Stb.Append(" SET Istat_Provincia = " + Agro_SQL_SaveText_NULL(Istat_Provincia) + " " & vbCrLf)
                Stb.Append(" ,Istat_Comune = " + Agro_SQL_SaveText_NULL(Istat_comune) + " " & vbCrLf)
                Stb.Append(" ,Stato = " + Agro_SQL_SaveText_NULL(Stato) + " " & vbCrLf)
                Stb.Append(" WHERE CUAA = " + Agro_SQL_SaveText_NULL(CUAA) + " " & vbCrLf)
                Stb.Append(" AND EnteValidatore_COD = " + Agro_SQL_SaveNum_NULL(EnteValidatore_COD) + " " & vbCrLf)
                Stb.Append(" AND Validazione_Numero = " + Agro_SQL_SaveText_NULL(Validazione_Numero) + " " & vbCrLf)
                Stb.Append(" AND Validazione_Data = " + Agro_SQL_SaveDateTime_NULL(Validazione_Data) + " " & vbCrLf)
            End If

            If Stb.ToString <> "" Then

                '--------------------------------------------------------------------------
                xRisp = EseguiQuery_Scrittura(objParametri, Stb.ToString, NomeRoutine)
                '--------------------------------------------------------------------------

            End If

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return xRisp

    End Function


    'Fine Funzione
End Class




