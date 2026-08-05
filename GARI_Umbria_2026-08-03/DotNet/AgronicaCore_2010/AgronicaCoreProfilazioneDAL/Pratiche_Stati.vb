Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate

Public Class Pratiche_Stati_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi(ByVal Pratica_Cod As Int32,
                          ByVal Stato_Cod As Int32,
                          ByVal Validita_Inizio As Date,
                          ByVal Validita_Fine As Date,
                            ByVal xFiltroAggiuntivo As String,
                            ByVal xOrderBy As String,
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                            ByVal PassaggioDiStato_Cod As Integer
                            ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreProfilazioneDAL.Pratiche_Stati_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.AppendLine(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; ")
            StrSQL.AppendLine(" SELECT Pratiche_Stati.*, Servizi.Servizio_Des ")
            StrSQL.AppendLine(" FROM  Pratiche_Stati ")
            StrSQL.AppendLine(" INNER JOIN Servizi ON Pratiche_Stati.Servizio_Cod = Servizi.Servizio_Cod ")
            StrSQL.AppendLine(" WHERE Pratiche_Stati.Validita_inizio <= " & Agro_SQL_SaveDate(Validita_Fine))
            StrSQL.AppendLine(" AND   Pratiche_Stati.Validita_Fine >= " & Agro_SQL_SaveDate(Validita_Inizio))

            If Pratica_Cod <> 0 Then
                StrSQL.AppendLine(" AND Pratiche_Stati.Pratica_Cod = " & Agro_SQL_SaveNum(Pratica_Cod))
            End If

            If Stato_Cod <> 0 Then
                StrSQL.AppendLine(" AND Pratiche_Stati.Stato_Cod = " & Agro_SQL_SaveNum(Stato_Cod))
            End If

            If PassaggioDiStato_Cod <> 0 Then
                StrSQL.AppendLine(" AND Pratiche_Stati.PassaggioDiStato_Cod = " & Agro_SQL_SaveNum(PassaggioDiStato_Cod))
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.AppendLine(" AND   Pratiche_Stati.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.AppendLine(" AND   Pratiche_Stati.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
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

Public Class Pratiche_Stati_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Scrivi(ByVal Pratica_Cod As Int32,
                            ByVal Stato_Cod As Int32,
                            ByVal Servizio_Cod As Int32,
                            ByVal Validita_Inizio As Date,
                            ByVal Validita_Fine As Date,
                            ByVal Note As String,
                            ByVal stato_origine_cod As Integer,
                            ByVal PassaggioDiStato_cod As Integer,
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                            ByVal Data_creazione As Date,
                            ByVal Data_modifica As Date,
                            ByVal username_creazione As String,
                            ByVal username_modifica As String
                            ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreProfilazioneDAL.Pratiche_Stati_W.Scrivi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

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


        Try

            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append("INSERT INTO Pratiche_Stati ")
            StrSQL.Append("                   (Piva_SuperUser, Pratica_Cod, Stato_Cod, Servizio_Cod, note, stato_origine_cod, passaggiodistato_cod, ")
            StrSQL.Append("                    Validita_Inizio, Validita_Fine, ")
            StrSQL.Append("                    Inviato,            DataInvio, ")
            StrSQL.Append("                    Data_Creazione,     Data_Modifica, ")
            StrSQL.Append("                    UserName_Creazione, UserName_Modifica ")
            StrSQL.Append("                   ) ")

            StrSQL.Append("VALUES (")
            StrSQL.Append("          '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Pratica_Cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Stato_Cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Servizio_Cod) & "  ")
            StrSQL.Append("         , '" & Agro_SQL_SaveText(Note) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(stato_origine_cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(PassaggioDiStato_cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDateTime(Validita_Inizio) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDateTime(Validita_Fine) & "  ")
            StrSQL.Append("         , 0  ")
            StrSQL.Append("         , Null  ")
            StrSQL.Append("		    , " & Agro_SQL_SaveDateTime(Data_creazione) & "  ")
            StrSQL.Append("		    , " & Agro_SQL_SaveDateTime(Data_modifica) & "  ")
            StrSQL.Append("		    ,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            StrSQL.Append("		    ,'" & Agro_SQL_SaveText(username_modifica) & "' ")
            StrSQL.Append(" )")
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


    Public Function Cancella( _
                            ByVal Pratica_Cod As Int32, _
                            ByVal Stato_Cod As Int32, _
                                 ByVal xFiltroAggiuntivo As String, _
                                 ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                 ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreProfilazioneDAL.Pratiche_Stati_W.Cancella()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = AgronicaCoreDataProvider.AgronicaCoreParametri.enumCancellazioneLogica.CancellazioneLogica Then
                StrSQL.Length = 0
                StrSQL.Append(" UPDATE Pratiche_Stati ")
                StrSQL.Append(" SET ")
                StrSQL.Append("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("      ,Inviato = -1 ")
                StrSQL.Append(" WHERE  Pratica_Cod = " & Agro_SQL_SaveNum(Pratica_Cod) & " ")
                StrSQL.Append(" AND Inviato >= 0")
            Else
                StrSQL.Length = 0
                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM   Pratiche_Stati ")
                StrSQL.Append(" WHERE  Pratica_Cod = " & Agro_SQL_SaveNum(Pratica_Cod) & " ")
            End If

            If Stato_Cod <> 0 Then
                StrSQL.Append(" AND   Stato_Cod = " & Agro_SQL_SaveNum(Stato_Cod) & " ")
            End If

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

    Public Function AggiornaDate(ByVal Pratica_Cod As Int32, _
                                 ByVal Stato_Cod As Int32, _
                                 ByVal Data As Date, _
                                ByVal xFiltroAggiuntivo As String, _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreProfilazioneDAL.Pratiche_Stati_W.AggiornaDate()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append("UPDATE Pratiche_Stati SET ")
            StrSQL.Append("              UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.Append("             ,Validita_Inizio   =  " & Agro_SQL_SaveDate(Data))
            StrSQL.Append("             ,Data_Modifica     =  " & Agro_SQL_SaveDateTime(Date.Now))
            StrSQL.Append(" WHERE Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.Append(" AND   Pratica_Cod = " & Agro_SQL_SaveNum(Pratica_Cod))
            StrSQL.Append(" AND   Stato_Cod = " & Agro_SQL_SaveNum(Stato_Cod))

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

        Finally

            StrSQL = Nothing

        End Try

        Return xRisp

    End Function



    Public Function Modifica(ByVal Pratica_Cod As Int32, _
                                 ByVal Stato_Cod As Int32, _
                                 ByVal Stato_origine_Cod As Integer, _
                                 ByVal Passaggio_di_stato_cod As Integer, _
                                 ByVal Data As Date, _
                                 ByVal note As String, _
                                ByVal xFiltroAggiuntivo As String, _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreProfilazioneDAL.Pratiche_Stati_W.AggiornaDate()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append("UPDATE Pratiche_Stati SET ")
            StrSQL.Append("              UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.Append("             ,Validita_Inizio   =  " & Agro_SQL_SaveDate(Data))
            StrSQL.Append("             ,Data_Modifica     =  " & Agro_SQL_SaveDateTime(Date.Now))
            StrSQL.Append(" , note = '" & Agro_SQL_SaveText(note) & "' ")
            StrSQL.Append(" WHERE Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.Append(" AND   Pratica_Cod = " & Agro_SQL_SaveNum(Pratica_Cod))
            StrSQL.Append(" AND   Stato_Cod = " & Agro_SQL_SaveNum(Stato_Cod))
            If Stato_origine_Cod <> 0 Then
                StrSQL.Append(" AND   Stato_Origine_cod = " & Agro_SQL_SaveNum(Stato_origine_Cod))
            End If
            StrSQL.Append(" AND   PassaggioDiStato_cod = " & Agro_SQL_SaveNum(Passaggio_di_stato_cod))



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

        Finally

            StrSQL = Nothing

        End Try

        Return xRisp

    End Function


End Class
