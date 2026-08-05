
Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.AgronicaCoreParametri

Public Class Linee_ProduzionixPreparazioni_R
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

            strSQL.Append(" SELECT * from Linee_ProduzionixPreparazioni " + vbCrLf)
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

    Public Function LeggiDaAziendaECodGenerazione(ByVal piva As String,
                                                  ByVal codGenerazione As Integer,
                                                  ByVal xFiltroAggiuntivo As String,
                                                  ByVal xOrderBy As String,
                                                  ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                  ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCore_ContabDAL.Linee_ProduzionixPreparazioni.LeggiDaAziendaECodGenerazione()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim strSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            strSQL.Length = 0

            strSQL.AppendLine(" SELECT pxp.Piva, pxp.Preparazione_Cod, pxp.Linea_Cod, prep.Preparazione_Des + ' - ' + prod.Linea_Des As descrizione ")
            strSQL.AppendLine(" from Linee_ProduzionixPreparazioni pxp ")
            strSQL.AppendLine(" inner join Linee_Preparazioni prep on prep.Piva = pxp.Piva and prep.Preparazione_Cod = pxp.Preparazione_Cod ")
            strSQL.AppendLine(" inner join Linee_Produzioni prod on prod.Piva = pxp.Piva and prod.Linea_Cod = pxp.Linea_Cod ")
            strSQL.AppendLine(" Where pxp.piva = '" + Agro_SQL_SaveText(piva) + "' ")
            strSQL.AppendLine(" and prep.Codice_Generazione = " + Agro_SQL_SaveNum(codGenerazione) + " ")

            If xFiltroAggiuntivo <> "" Then
                strSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    strSQL.AppendLine(" AND   pxp.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    strSQL.AppendLine(" AND   pxp.Inviato =-1 ")
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

Public Class Linee_ProduzionixPreparazioni_W
    Inherits AgronicaCoreDataProvider.DataProvider



    '##############################################################################################
    Public Function Scrivi( _
                          ByVal Piva As String _
                        , ByVal Linea_Cod As String _
                        , ByVal Preparazione_Cod As Integer _
                        , ByVal Livello As String _
                        , ByVal ChkLoop_Start As String _
                        , ByVal ChkLoop_End As String _
                        , ByVal Preparazione_Cod_Vincolo As Integer _
                        , ByVal Opzionale As String _
                        , ByVal Invisibile As String _
                        , ByVal Linea_Modello_Cod As Integer _
                        , ByVal Qta_Piano As String _
                        , ByVal validita_inizio As Date _
                        , ByVal validita_fine As Date _
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
            StrSQL.Append(" INSERT Linee_ProduzionixPreparazioni " + vbCrLf)

            StrSQL.Append("              (")

            StrSQL.Append("   [Piva] " & vbCrLf)
            StrSQL.Append("  ,[Linea_Cod] " & vbCrLf)
            StrSQL.Append("  ,[Preparazione_Cod] " & vbCrLf)
            StrSQL.Append("  ,[Livello] " & vbCrLf)
            StrSQL.Append("  ,[ChkLoop_Start] " & vbCrLf)
            StrSQL.Append("  ,[ChkLoop_End] " & vbCrLf)
            StrSQL.Append("  ,[Preparazione_Cod_Vincolo] " & vbCrLf)
            StrSQL.Append("  ,[Opzionale] " & vbCrLf)
            StrSQL.Append("  ,[Invisibile] " & vbCrLf)
            StrSQL.Append("  ,[Linea_Modello_Cod] " & vbCrLf)
            StrSQL.Append("  ,[Qta_Piano], " & vbCrLf)
            StrSQL.Append("              Inviato,            datainvio, ")
            StrSQL.Append("              Data_Creazione,     Data_Modifica, ")
            StrSQL.Append("              UserName_Creazione, UserName_Modifica, ")
            StrSQL.Append("              Validita_Inizio,    Validita_Fine ")

            StrSQL.Append("              ) ")

            StrSQL.Append(" VALUES ( ")

            StrSQL.Append(",'" & Agro_SQL_SaveText(Piva) & "'" & vbCrLf)
            StrSQL.Append(",'" & Agro_SQL_SaveText(Linea_Cod) & "'" & vbCrLf)
            StrSQL.Append(", " & Agro_SQL_SaveNum(Preparazione_Cod) & " " & vbCrLf)
            StrSQL.Append(",'" & Agro_SQL_SaveText(Livello) & "'" & vbCrLf)
            StrSQL.Append(",'" & Agro_SQL_SaveText(ChkLoop_Start) & "'" & vbCrLf)
            StrSQL.Append(",'" & Agro_SQL_SaveText(ChkLoop_End) & "'" & vbCrLf)
            StrSQL.Append(", " & Agro_SQL_SaveNum(Preparazione_Cod_Vincolo) & " " & vbCrLf)
            StrSQL.Append(",'" & Agro_SQL_SaveText(Opzionale) & "'" & vbCrLf)
            StrSQL.Append(",'" & Agro_SQL_SaveText(Invisibile) & "'" & vbCrLf)
            StrSQL.Append(", " & Agro_SQL_SaveNum(Linea_Modello_Cod) & " " & vbCrLf)
            StrSQL.Append(",'" & Agro_SQL_SaveText(Qta_Piano) & "'" & vbCrLf)


            StrSQL.Append("         , 0  " + vbCrLf)
            StrSQL.Append("         , Null  " + vbCrLf)

            StrSQL.Append("			, " & Agro_SQL_SaveDateTime(Data_creazione) & "  ")
            StrSQL.Append("			, " & Agro_SQL_SaveDateTime(Data_modifica) & "  ")
            StrSQL.Append("			,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            StrSQL.Append("			,'" & Agro_SQL_SaveText(username_modifica) & "' ")

            StrSQL.Append("			, " & Agro_SQL_SaveDate(Data_creazione) & "  ")
            StrSQL.Append("			, " & Agro_SQL_SaveDate(Data_modifica) & "  ")


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


