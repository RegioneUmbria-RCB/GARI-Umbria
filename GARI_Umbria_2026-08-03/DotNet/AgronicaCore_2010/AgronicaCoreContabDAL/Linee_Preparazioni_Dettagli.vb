
Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.AgronicaCoreParametri

Public Class Linee_Preparazioni_Dettagli_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function Leggi( _
                         ByVal piva As String, _
                         ByVal xFiltroAggiuntivo As String, _
                                        ByVal xOrderBy As String, _
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                        ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCore_DAL.Leggi()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim strSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            strSQL.Length = 0

            strSQL.Append(" SELECT * from Linee_Preparazioni_Dettagli " + vbCrLf)
            strSQL.Append(" WHERE Piva = '" + Agro_SQL_SaveText(piva) + "' " + vbCrLf)


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
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT





    End Function


End Class


'#################################################################
'#################################################################
'#################################################################

Public Class Linee_Preparazioni_Dettagli_W
    Inherits AgronicaCoreDataProvider.DataProvider



    '##############################################################################################
    Public Function Scrivi( _
                          ByVal Piva As String _
                        , ByVal Preparazione_Cod As Integer _
                        , ByVal Dettaglio_Cod As Integer _
                        , ByVal Dettaglio_Des As String _
                        , ByVal Cau_Mov As String _
                        , ByVal Elem_Cod As Integer _
                        , ByVal Pro_Cod As Integer _
                        , ByVal Mat_Cod As Integer _
                        , ByVal Veg_Cod As Integer _
                        , ByVal Cul_Cod As Integer _
                        , ByVal Udm_Cod As Integer _
                        , ByVal Mezzo As Integer _
                        , ByVal Tipo_Destinazione As String _
                        , ByVal Sa_Cod_Destinazione As Integer _
                        , ByVal Id_Destinazione As Integer _
                        , ByVal Qta As String _
                        , ByVal Qta_Extra As String _
                        , ByVal Qta_Min As String _
                        , ByVal Qta_Max As String _
                        , ByVal ChkBase As String _
                        , ByVal ChkScarto As String _
                        , ByVal ChkSomma As String _
                        , ByVal ChkRipartizione As String _
                        , ByVal ChkMassa_Volumica As String _
                        , ByVal ChkIniziale As String _
                        , ByVal ChkFinale As String _
                        , ByVal ChkCampionatura As String _
                        , ByVal ChkLotto_Edit As String _
                        , ByVal ChkDisabilitazione As String _
                        , ByVal Modulo_Generazione As Integer _
                        , ByVal Codice_Generazione As Integer _
                        , ByVal Filtro_Colori As String _
                        , ByVal Filtro_Categorie As String _
                        , ByVal Filtro_Classificazioni As String _
                        , ByVal Filtro_Cultivar As String _
                        , ByVal ChkIntegrazione As String _
                        , ByVal Lotto_Default As String _
                        , ByVal Livello_Dettaglio As String _
                        , ByVal Filtro_Diciture As String _
                        , ByVal validita_inizio As Date _
                        , ByVal validita_fine As Date _
                        , ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                , Optional ByVal Data_creazione As DateTime = #2/1/1900# _
                , Optional ByVal Data_modifica As DateTime = #2/1/1900# _
                , Optional ByVal username_creazione As String = "" _
                , Optional ByVal username_modifica As String = "" _
                ) As Boolean


        Dim NomeRoutine As String = "Scrivi()"

        '====================================================================================
        'Parametri opzionali :

        '====================================================================================

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



            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append(" INSERT Linee_Preparazioni_Dettagli " + vbCrLf)

            StrSQL.Append("              (")

            StrSQL.Append("   [Piva] " & vbCrLf)
            StrSQL.Append("  ,[Preparazione_Cod] " & vbCrLf)
            StrSQL.Append("  ,[Dettaglio_Cod] " & vbCrLf)
            StrSQL.Append("  ,[Dettaglio_Des] " & vbCrLf)
            StrSQL.Append("  ,[Cau_Mov] " & vbCrLf)
            StrSQL.Append("  ,[Elem_Cod] " & vbCrLf)
            StrSQL.Append("  ,[Pro_Cod] " & vbCrLf)
            StrSQL.Append("  ,[Mat_Cod] " & vbCrLf)
            StrSQL.Append("  ,[Veg_Cod] " & vbCrLf)
            StrSQL.Append("  ,[Cul_Cod] " & vbCrLf)
            StrSQL.Append("  ,[Udm_Cod] " & vbCrLf)
            StrSQL.Append("  ,[Mezzo] " & vbCrLf)
            StrSQL.Append("  ,[Tipo_Destinazione] " & vbCrLf)
            StrSQL.Append("  ,[Sa_Cod_Destinazione] " & vbCrLf)
            StrSQL.Append("  ,[Id_Destinazione] " & vbCrLf)
            StrSQL.Append("  ,[Qta] " & vbCrLf)
            StrSQL.Append("  ,[Qta_Extra] " & vbCrLf)
            StrSQL.Append("  ,[Qta_Min] " & vbCrLf)
            StrSQL.Append("  ,[Qta_Max] " & vbCrLf)
            StrSQL.Append("  ,[ChkBase] " & vbCrLf)
            StrSQL.Append("  ,[ChkScarto] " & vbCrLf)
            StrSQL.Append("  ,[ChkSomma] " & vbCrLf)
            StrSQL.Append("  ,[ChkRipartizione] " & vbCrLf)
            StrSQL.Append("  ,[ChkMassa_Volumica] " & vbCrLf)
            StrSQL.Append("  ,[ChkIniziale] " & vbCrLf)
            StrSQL.Append("  ,[ChkFinale] " & vbCrLf)
            StrSQL.Append("  ,[ChkCampionatura] " & vbCrLf)
            StrSQL.Append("  ,[ChkLotto_Edit] " & vbCrLf)
            StrSQL.Append("  ,[ChkDisabilitazione] " & vbCrLf)
            StrSQL.Append("  ,[Modulo_Generazione] " & vbCrLf)
            StrSQL.Append("  ,[Codice_Generazione] " & vbCrLf)
            StrSQL.Append("  ,[Filtro_Colori] " & vbCrLf)
            StrSQL.Append("  ,[Filtro_Categorie] " & vbCrLf)
            StrSQL.Append("  ,[Filtro_Classificazioni] " & vbCrLf)
            StrSQL.Append("  ,[Filtro_Cultivar] " & vbCrLf)
            StrSQL.Append("  ,[ChkIntegrazione] " & vbCrLf)
            StrSQL.Append("  ,[Lotto_Default] " & vbCrLf)
            StrSQL.Append("  ,[Livello_Dettaglio] " & vbCrLf)
            StrSQL.Append("  ,[Filtro_Diciture], " & vbCrLf)

            StrSQL.Append("              Inviato,            datainvio, ")
            StrSQL.Append("              Data_Creazione,     Data_Modifica, ")
            StrSQL.Append("              UserName_Creazione, UserName_Modifica, ")
            StrSQL.Append("              Validita_Inizio,    Validita_Fine ")

            StrSQL.Append("              ) ")

            StrSQL.Append(" VALUES ( ")

            StrSQL.Append(",'" & Agro_SQL_SaveText(Piva) & "'" & vbCrLf)
            StrSQL.Append(", " & Agro_SQL_SaveNum(Preparazione_Cod) & " " & vbCrLf)
            StrSQL.Append(", " & Agro_SQL_SaveNum(Dettaglio_Cod) & " " & vbCrLf)
            StrSQL.Append(",'" & Agro_SQL_SaveText(Dettaglio_Des) & "'" & vbCrLf)
            StrSQL.Append(",'" & Agro_SQL_SaveText(Cau_Mov) & "'" & vbCrLf)
            StrSQL.Append(", " & Agro_SQL_SaveNum(Elem_Cod) & " " & vbCrLf)
            StrSQL.Append(", " & Agro_SQL_SaveNum(Pro_Cod) & " " & vbCrLf)
            StrSQL.Append(", " & Agro_SQL_SaveNum(Mat_Cod) & " " & vbCrLf)
            StrSQL.Append(", " & Agro_SQL_SaveNum(Veg_Cod) & " " & vbCrLf)
            StrSQL.Append(", " & Agro_SQL_SaveNum(Cul_Cod) & " " & vbCrLf)
            StrSQL.Append(", " & Agro_SQL_SaveNum(Udm_Cod) & " " & vbCrLf)
            StrSQL.Append(", " & Agro_SQL_SaveNum(Mezzo) & " " & vbCrLf)
            StrSQL.Append(",'" & Agro_SQL_SaveText(Tipo_Destinazione) & "'" & vbCrLf)
            StrSQL.Append(", " & Agro_SQL_SaveNum(Sa_Cod_Destinazione) & " " & vbCrLf)
            StrSQL.Append(", " & Agro_SQL_SaveNum(Id_Destinazione) & " " & vbCrLf)
            StrSQL.Append(",'" & Agro_SQL_SaveText(Qta) & "'" & vbCrLf)
            StrSQL.Append(",'" & Agro_SQL_SaveText(Qta_Extra) & "'" & vbCrLf)
            StrSQL.Append(",'" & Agro_SQL_SaveText(Qta_Min) & "'" & vbCrLf)
            StrSQL.Append(",'" & Agro_SQL_SaveText(Qta_Max) & "'" & vbCrLf)
            StrSQL.Append(",'" & Agro_SQL_SaveText(ChkBase) & "'" & vbCrLf)
            StrSQL.Append(",'" & Agro_SQL_SaveText(ChkScarto) & "'" & vbCrLf)
            StrSQL.Append(",'" & Agro_SQL_SaveText(ChkSomma) & "'" & vbCrLf)
            StrSQL.Append(",'" & Agro_SQL_SaveText(ChkRipartizione) & "'" & vbCrLf)
            StrSQL.Append(",'" & Agro_SQL_SaveText(ChkMassa_Volumica) & "'" & vbCrLf)
            StrSQL.Append(",'" & Agro_SQL_SaveText(ChkIniziale) & "'" & vbCrLf)
            StrSQL.Append(",'" & Agro_SQL_SaveText(ChkFinale) & "'" & vbCrLf)
            StrSQL.Append(",'" & Agro_SQL_SaveText(ChkCampionatura) & "'" & vbCrLf)
            StrSQL.Append(",'" & Agro_SQL_SaveText(ChkLotto_Edit) & "'" & vbCrLf)
            StrSQL.Append(",'" & Agro_SQL_SaveText(ChkDisabilitazione) & "'" & vbCrLf)
            StrSQL.Append(", " & Agro_SQL_SaveNum(Modulo_Generazione) & " " & vbCrLf)
            StrSQL.Append(", " & Agro_SQL_SaveNum(Codice_Generazione) & " " & vbCrLf)
            StrSQL.Append(",'" & Agro_SQL_SaveText(Filtro_Colori) & "'" & vbCrLf)
            StrSQL.Append(",'" & Agro_SQL_SaveText(Filtro_Categorie) & "'" & vbCrLf)
            StrSQL.Append(",'" & Agro_SQL_SaveText(Filtro_Classificazioni) & "'" & vbCrLf)
            StrSQL.Append(",'" & Agro_SQL_SaveText(Filtro_Cultivar) & "'" & vbCrLf)
            StrSQL.Append(",'" & Agro_SQL_SaveText(ChkIntegrazione) & "'" & vbCrLf)
            StrSQL.Append(",'" & Agro_SQL_SaveText(Lotto_Default) & "'" & vbCrLf)
            StrSQL.Append(",'" & Agro_SQL_SaveText(Livello_Dettaglio) & "'" & vbCrLf)
            StrSQL.Append(",'" & Agro_SQL_SaveText(Filtro_Diciture) & "'" & vbCrLf)


            StrSQL.Append("         , 0  " + vbCrLf)
            StrSQL.Append("         , Null  " + vbCrLf)

            StrSQL.Append("			, " & Agro_SQL_SaveDateTime(Data_creazione) & "  ")
            StrSQL.Append("			, " & Agro_SQL_SaveDateTime(Data_modifica) & "  ")
            StrSQL.Append("			,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            StrSQL.Append("			,'" & Agro_SQL_SaveText(username_modifica) & "' ")
            StrSQL.Append("			, " & Agro_SQL_SaveDate(validita_inizio) & "  ")
            StrSQL.Append("			, " & Agro_SQL_SaveDate(validita_fine) & "  ")



            StrSQL.Append(") ")

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







    '#################################################################
    Public Function Cancella(ByVal xFiltroAggiuntivo As String, _
                              ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                              ) As Boolean

        '----- Descrizione
        Dim NomeRoutine As String = "Cancella()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            StrSQL.Length = 0

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then
                StrSQL.Append(" UPDATE ... ")
                StrSQL.Append(" SET ")
                StrSQL.Append("         Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("         ,Data_Modifica= " & Agro_SQL_SaveDate(Date.Now) & " ")
                StrSQL.Append("         ,Inviato = -1 ")
                StrSQL.Append(" WHERE   1=1 ")
                StrSQL.Append(" AND     Inviato >= 0 ")
            Else
                StrSQL.Append(" DELETE FROM ... ")
                StrSQL.Append(" WHERE 1=1 ")
            End If
            '---------------------------------------------

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


