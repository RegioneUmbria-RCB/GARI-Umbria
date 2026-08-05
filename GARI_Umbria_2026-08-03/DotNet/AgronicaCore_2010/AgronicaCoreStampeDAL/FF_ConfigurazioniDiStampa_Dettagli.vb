Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.AgronicaCoreParametri

Public Class FF_ConfigurazioniDiStampa_Dettagli_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function Leggi( _
            ByVal FF_Stampa_Dettagli_cod As Integer, _
            ByVal FF_Stampanti_cod As Integer, _
            ByVal xFiltroAggiuntivo As String, _
            ByVal xOrderBy As String, _
            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
    ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "FF_ConfigurazioniDiStampa_Dettagli_R.Leggi()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Stb.Length = 0

            Stb.Append(" SELECT D.*, P.Nome_Per_Stampa, L.FF_TipologiaEtichette_Cod, coalesce(ddd.id_mov_det, 0 ) as id_mov_det " & vbCrLf)
            Stb.Append(" FROM  FF_ConfigurazioniDiStampa_Dettagli D " & vbCrLf)
            Stb.Append(" INNER JOIN FF_stampanti P " & vbCrLf)
            Stb.Append(" on P.FF_Stampanti_Cod = D.FF_Stampanti_Cod " & vbCrLf)
            Stb.Append(" INNER JOIN FF_LayoutEtichette L " & vbCrLf)
            Stb.Append(" on L.FF_LayoutEtichette_cod = D.FF_LayoutEtichette " & vbCrLf)
            Stb.Append(" LEFT JOIN Movimenti_dettagli ddd " & vbCrLf)
            Stb.Append(" on ddd.id_Attivita = D.FF_Stampa_Dettagli_Cod " & vbCrLf)
            Stb.Append(" WHERE  FF_Stampa_Dettagli_Cod =  " & FF_Stampa_Dettagli_cod & vbCrLf)
            Stb.Append(" AND  D.FF_Stampanti_Cod =  " & FF_Stampanti_cod & vbCrLf)
            If xFiltroAggiuntivo <> "" Then
                Stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    Stb.Append(" AND   D.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    Stb.Append(" AND   D.Inviato =-1 ")
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
            MessaggioErrore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

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

Public Class FF_ConfigurazioniDiStampa_Dettagli_W
    Inherits AgronicaCoreDataProvider.DataProvider


    '##############################################################################################
    Public Function AggiornaOperazioneDiAgenda( _
                              ByVal id_Agenda As Integer _
                            , ByVal id_mov_det As Integer _
                            , ByVal FF_Stampa_Dettagli_Cod As Integer _
                            , ByVal NomeTabella_AgendaxUpdate As String _
                            , ByVal NomeColonna_AgendaxUpdate As String _
                            , ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                , Optional ByVal Data_creazione As Date = #2/1/1900# _
                , Optional ByVal Data_modifica As Date = #2/1/1900# _
                , Optional ByVal username_creazione As String = "" _
                , Optional ByVal username_modifica As String = "" _
                ) As Boolean


        Dim NomeRoutine As String = "AggiornaOperazioneDiAgenda()"

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


            Stb.Append("UPDATE " & NomeTabella_AgendaxUpdate & vbCrLf)
            Stb.Append(" SET " & NomeColonna_AgendaxUpdate & " = " & FF_Stampa_Dettagli_Cod & vbCrLf)
            Stb.Append(" WHERE id_agenda = " & id_Agenda & vbCrLf)

            If id_mov_det <> 0 Then
                Stb.Append(" AND id_mov_det = " & id_mov_det & vbCrLf)
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


    Public Function AggiornaOperazioneDiAgenda_PulisciOrfani( _
                              ByVal NomeTabella_AgendaxUpdate As String _
                            , ByVal NomeColonna_AgendaxUpdate As String _
                            , ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _               
                ) As Boolean


        Dim NomeRoutine As String = "AggiornaOperazioneDiAgenda()"

        '====================================================================================
        'Parametri opzionali :

        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

           

            '---------------------------------------------
            Stb.Length = 0



            Stb.Append(" delete " & vbCrLf)
            Stb.Append(" from FF_ConfigurazioniDiStampa_Dettagli " & vbCrLf)
            Stb.Append(" where FF_Stampa_Dettagli_Cod  " & vbCrLf)
            Stb.Append(" not in ( " & vbCrLf)
            Stb.Append("     Select id_attivita " & vbCrLf)
            Stb.Append("     from Movimenti_dettagli " & vbCrLf)
            Stb.Append(" )         " & vbCrLf)


                '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, Stb.ToString, NomeRoutine)
                '--------------------------------------------------------------------------

            Stb.Length = 0


            Stb.Append(" delete " & vbCrLf)
            Stb.Append(" from FF_Stampa_Dettagli " & vbCrLf)
            Stb.Append(" where FF_Stampa_Dettagli_Cod  " & vbCrLf)
            Stb.Append(" not in ( " & vbCrLf)
            Stb.Append("     Select id_attivita " & vbCrLf)
            Stb.Append("     from Movimenti_dettagli " & vbCrLf)
            Stb.Append(" )         " & vbCrLf)


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



    '##############################################################################################
    Public Function Scrivi( _
                              ByVal FF_ConfigurazioneDiStampa_Dettagli_Descrizione As String _
                            , ByVal FF_ConfigurazioniDiStampa_Cod As Integer _
                            , ByVal FF_Stampanti_Cod As Integer _
                            , ByVal FF_Lingua_Cod As Integer _
                            , ByVal FF_LayoutEtichette As Integer _
                            , ByVal FF_Stampa_Dettagli_Cod As Integer _
                            , ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                , Optional ByVal Data_creazione As Date = #2/1/1900# _
                , Optional ByVal Data_modifica As Date = #2/1/1900# _
                , Optional ByVal username_creazione As String = "" _
                , Optional ByVal username_modifica As String = "" _
                ) As Boolean


        Dim NomeRoutine As String = "Scrivi()"

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
            Stb.Append(" INSERT FF_ConfigurazioniDiStampa_Dettagli " + vbCrLf)

            Stb.Append("              (")

            Stb.Append("   [FF_ConfigurazioneDiStampa_Dettagli_Descrizione] " & vbCrLf)
            Stb.Append("  ,[FF_ConfigurazioniDiStampa_Cod] " & vbCrLf)
            Stb.Append("  ,[FF_Stampanti_Cod] " & vbCrLf)
            Stb.Append("  ,[FF_Lingua_Cod] " & vbCrLf)
            Stb.Append("  ,[FF_LayoutEtichette] " & vbCrLf)
            Stb.Append("  ,[FF_Stampa_Dettagli_Cod] " & vbCrLf)


            Stb.Append("              , Inviato,            datainvio, ")
            Stb.Append("              Data_Creazione,     Data_Modifica, ")
            Stb.Append("              UserName_Creazione, UserName_Modifica, ")
            Stb.Append("              Validita_Inizio,    Validita_Fine ")
            Stb.Append("              ) ")

            Stb.Append(" VALUES ( ")


            Stb.Append(" '" & Agro_SQL_SaveText(FF_ConfigurazioneDiStampa_Dettagli_Descrizione) & "'" & vbCrLf)
            Stb.Append(", " & Agro_SQL_SaveNum(FF_ConfigurazioniDiStampa_Cod) & " " & vbCrLf)
            Stb.Append(", " & Agro_SQL_SaveNum(FF_Stampanti_Cod) & " " & vbCrLf)
            Stb.Append(", " & Agro_SQL_SaveNum(FF_Lingua_Cod) & " " & vbCrLf)
            Stb.Append(", " & Agro_SQL_SaveNum(FF_LayoutEtichette) & " " & vbCrLf)
            Stb.Append(", " & Agro_SQL_SaveNum(FF_Stampa_Dettagli_Cod) & " " & vbCrLf)

            Stb.Append("         , 0  " + vbCrLf)
            Stb.Append("         , Null  " + vbCrLf)

            Stb.Append("			, " & Agro_SQL_SaveDate(Data_creazione) & "  ")
            Stb.Append("			, " & Agro_SQL_SaveDate(Data_modifica) & "  ")
            Stb.Append("			,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            Stb.Append("			,'" & Agro_SQL_SaveText(username_modifica) & "' ")


            Stb.Append("			, " & Agro_SQL_SaveDate(AGRODATAINIZIO) & "  ")
            Stb.Append("			, " & Agro_SQL_SaveDate(AGRODATAFINE) & "  ")


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





    '#################################################################
    Public Function Modifica( _
              ByVal FF_ConfigurazioneDiStampa_Dettagli_Descrizione As String _
            , ByVal FF_ConfigurazioniDiStampa_Cod As Integer _
            , ByVal FF_Stampanti_Cod As Integer _
            , ByVal FF_Lingua_Cod As Integer _
            , ByVal FF_LayoutEtichette As Integer _
            , ByVal FF_Stampa_Dettagli_Cod As Integer _
            , ByVal xFiltroAggiuntivo As String _
            , ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
        ) As Boolean

        '----- Descrizione
        Dim NomeRoutine As String = "Cancella()"

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            Stb.Length = 0

            Stb.Append(" UPDATE FF_ConfigurazioniDiStampa_Dettagli ")
            Stb.Append(" SET ")
            Stb.Append("  FF_ConfigurazioneDiStampa_Dettagli_Descrizione = '" & Agro_SQL_SaveText(FF_ConfigurazioneDiStampa_Dettagli_Descrizione) & "'" & vbCrLf)
            Stb.Append(", FF_ConfigurazioniDiStampa_Cod =  " & Agro_SQL_SaveNum(FF_ConfigurazioniDiStampa_Cod) & " " & vbCrLf)
            Stb.Append(", FF_Stampanti_Cod =  " & Agro_SQL_SaveNum(FF_Stampanti_Cod) & " " & vbCrLf)
            Stb.Append(", FF_Lingua_Cod =  " & Agro_SQL_SaveNum(FF_Lingua_Cod) & " " & vbCrLf)
            Stb.Append(", FF_LayoutEtichette =  " & Agro_SQL_SaveNum(FF_LayoutEtichette) & " " & vbCrLf)


            Stb.Append(", Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            Stb.Append(",Data_Modifica= " & Agro_SQL_SaveDate(Date.Now) & " ")

            Stb.Append(" WHERE   1=1 ")
            Stb.Append(" AND FF_Stampa_Dettagli_Cod = " & FF_Stampa_Dettagli_Cod & vbCrLf)

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



    '#################################################################
    Public Function Cancella( _
        ByVal FF_Stampa_Dettagli_Cod As Integer, _
        ByVal FF_Stampanti_cod As Integer, _
        ByVal xFiltroAggiuntivo As String, _
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
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

            Stb.Append(" DELETE FROM FF_ConfigurazioniDiStampa_Dettagli ")
            Stb.Append(" WHERE FF_Stampa_Dettagli_Cod =  " & FF_Stampa_Dettagli_Cod)
            Stb.Append(" AND FF_Stampanti_cod =  " & FF_Stampanti_cod)
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




End Class



