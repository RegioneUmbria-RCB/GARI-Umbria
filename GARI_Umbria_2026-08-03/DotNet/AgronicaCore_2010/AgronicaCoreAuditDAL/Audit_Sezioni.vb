
Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.AgronicaCoreParametri

Public Class Audit_Sezioni_R

    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function Leggi(ByVal Audit_Tipo As enum_AuditTipi,
                                       ByVal Regolamento_Cod As Integer,
                                       ByVal Sezione_Cod As Long,
                                       ByVal Parte As Long,
                                       ByVal Data As Date,
                                       ByRef MessaggioErrore As String,
                                        ByVal xFiltroAggiuntivo As String,
                                        ByVal xOrderBy As String,
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                       Optional ByVal Disp_Cod As Integer = 0,
                                       Optional ByVal FlagVisibilita As Integer = -1
                                        ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCoreAuditDAL.Audit_Sezioni_R.Leggi()"

        '----- Variabili
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Stb.Length = 0


            Stb.Append(" SELECT *, ")
            Stb.Append(" ISNULL ((SELECT FunCalcoloLivello FROM Audit_DisposizioniXSezioni ")
            Stb.Append("          WHERE Audit_Sezioni.Sezione_Cod = Audit_DisposizioniXSezioni.Sezione_Cod ")
            Stb.Append("          AND   Audit_Sezioni.Audit_Tipo = Audit_DisposizioniXSezioni.Audit_Tipo ")
            Stb.Append("          AND   Audit_Sezioni.Regolamento_Cod = Audit_DisposizioniXSezioni.Regolamento_Cod ")

            ' possono esserci + funzioni di calcolo del livello per la stessa sezione
            If Data <> #1/1/1900# Then
                Stb.Append("      AND Audit_DisposizioniXSezioni.Validita_Inizio <= " & Agro_SQL_SaveDate(Data) & " ")
                Stb.Append("      AND Audit_DisposizioniXSezioni.Validita_Fine > " & Agro_SQL_SaveDate(Data) & " ")
            End If
            Stb.Append("          AND Disp_Cod = " & Agro_SQL_SaveNum(Disp_Cod) & "), NULL) AS FunCalcoloLivello ")

            Stb.Append(" FROM Audit_Sezioni ")

            Stb.Append(" WHERE Audit_Sezioni.Audit_Tipo = " & Agro_SQL_SaveNum(Audit_Tipo))
            Stb.Append(" AND   Audit_Sezioni.Regolamento_Cod = " & Agro_SQL_SaveNum(Regolamento_Cod))

            If Sezione_Cod <> 0 Then
                Stb.Append(" AND Audit_Sezioni.Sezione_Cod = " & Agro_SQL_SaveNum(Sezione_Cod))
            End If

            If Parte <> 0 Then
                Stb.Append(" AND Audit_Sezioni.Parte = " & Agro_SQL_SaveNum(Parte))
            End If

            If Data <> #1/1/1900# Then
                Stb.Append(" AND Audit_Sezioni.Validita_Inizio <= " & Agro_SQL_SaveDate(Data) & " ")
                Stb.Append(" AND Audit_Sezioni.Validita_Fine > " & Agro_SQL_SaveDate(Data) & " ")
            End If


            If xFiltroAggiuntivo <> "" Then
                Stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------

            If FlagVisibilita = -1 Then
                FlagVisibilita = objParametri.FlagVisibilita
            End If

            Select Case FlagVisibilita
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
            Else
                Stb.Append(" ORDER BY Ordine ")
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, Stb.ToString, NomeRoutine)
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

Public Class Audit_Sezioni_W
    Inherits AgronicaCoreDataProvider.DataProvider



    '##############################################################################################
    Public Function Scrivi(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                , Optional ByVal Data_creazione As Date = #2/1/1900# _
                , Optional ByVal Data_modifica As Date = #2/1/1900# _
                , Optional ByVal username_creazione As String = "" _
                , Optional ByVal username_modifica As String = "" _
                ) As Boolean


        Dim NomeRoutine As String = "AgronicaCoreAuditDAL.Audit_Sezioni_W.Scrivi()"

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
            Stb.Append(" INSERT ... " + vbCrLf)

            Stb.Append("              (")
            Stb.Append("              Inviato,            datainvio, ")
            Stb.Append("              Data_Creazione,     Data_Modifica, ")
            Stb.Append("              UserName_Creazione, UserName_Modifica, ")
            Stb.Append("              Validita_Inizio,    Validita_Fine, ")
            Stb.Append("              ) ")

            Stb.Append(" VALUES ( ")



            Stb.Append("         , 0  " + vbCrLf)
            Stb.Append("         , Null  " + vbCrLf)

            Stb.Append("			, " & Agro_SQL_SaveDate(Data_creazione) & "  ")
            Stb.Append("			, " & Agro_SQL_SaveDate(Data_modifica) & "  ")
            Stb.Append("			,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            Stb.Append("			,'" & Agro_SQL_SaveText(username_modifica) & "' ")



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
    Public Function Cancella(ByVal xFiltroAggiuntivo As String, _
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




End Class
