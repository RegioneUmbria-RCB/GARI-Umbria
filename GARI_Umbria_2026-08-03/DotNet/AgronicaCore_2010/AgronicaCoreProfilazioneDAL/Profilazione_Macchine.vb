Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider

Public Class Profilazione_Macchine_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi(ByVal IdProfiloDati_isK As Integer, _
                        ByVal Mac_Cod_isK As Integer, _
                        ByVal Mac_Car_Cod_isK As Integer, _
                            ByVal xFiltroAggiuntivo As String, _
                            ByVal xOrderBy As String, _
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                            ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreProfilazioneDAL.Profilazione_Macchine_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            '---------------------------------------------



            StrSQL.Length = 0
            StrSQL.Append(" SELECT Profilazione_Dati_MacchinexCaratteristiche.*, Macchine_Caratteristiche.Mac_Car_Des " + vbCrLf)
            StrSQL.Append(" FROM Profilazione_Dati_MacchinexCaratteristiche INNER JOIN ")
            StrSQL.Append("      Macchine_Caratteristiche ON Profilazione_Dati_MacchinexCaratteristiche.Mac_Car_Cod = Macchine_Caratteristiche.Mac_Car_Cod ")
            StrSQL.Append(" WHERE PivaSuperUser='" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'")

            If IdProfiloDati_isK <> 0 Then
                StrSQL.Append(" AND Id_Profilo_Dati= " & Agro_SQL_SaveNum(IdProfiloDati_isK) & "  ")
            End If

            If Mac_Cod_isK <> 0 Then
                StrSQL.Append(" AND Mac_Cod=" + Agro_SQL_SaveNum(Mac_Cod_isK) + " ")
            End If

            If Mac_Car_Cod_isK <> 0 Then
                StrSQL.Append(" AND Mac_Car_Cod=" + Agro_SQL_SaveNum(Mac_Car_Cod_isK) + " ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   Profilazione_Dati_MacchinexCaratteristiche.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   Profilazione_Dati_MacchinexCaratteristiche.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))

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

Public Class Profilazione_Macchine_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Scrivi(ByVal Id_Profilo_Dati As Int32,
                            ByVal Mac_Cod As Int32,
                            ByVal Mac_Car_Cod As Int32,
                            ByVal Valore As String,
                            ByVal Validita_Inizio As Date,
                            ByVal Validita_Fine As Date,
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                            ByVal Data_creazione As Date,
                            ByVal Data_modifica As Date,
                            ByVal username_creazione As String,
                            ByVal username_modifica As String
                            ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreProfilazioneDAL.Profilazione_Macchine_W.Scrivi()"

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
            StrSQL.Append("INSERT INTO Profilazione_Dati_MacchinexCaratteristiche ")
            StrSQL.Append("                   (PivaSuperUser, Id_Profilo_Dati, Mac_Cod, Mac_Car_Cod, Valore, ")
            StrSQL.Append("                    Validita_Inizio, Validita_Fine, ")
            StrSQL.Append("                    Inviato,            DataInvio, ")
            StrSQL.Append("                    Data_Creazione,     Data_Modifica, ")
            StrSQL.Append("                    UserName_Creazione, UserName_Modifica ")
            StrSQL.Append("                   ) ")

            StrSQL.Append("VALUES (")
            StrSQL.Append("          '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Id_Profilo_Dati) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Mac_Cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Mac_Car_Cod) & "  ")
            StrSQL.Append("         , '" & Agro_SQL_SaveText(Valore) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")
            StrSQL.Append("         , 0  ")
            StrSQL.Append("         , Null  ")
            StrSQL.Append("		    , " & Agro_SQL_SaveDate(Data_creazione) & "  ")
            StrSQL.Append("		    , " & Agro_SQL_SaveDate(Data_modifica) & "  ")
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
                        ByVal Id_Profilo_Dati As Int32, _
                        ByVal Mac_Cod As Int32, _
                        ByVal Mac_Car_Cod As Int32, _
                             ByVal xFiltroAggiuntivo As String, _
                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                             ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreProfilazioneDAL.Profilazione_Macchine_W.Cancella()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = AgronicaCoreDataProvider.AgronicaCoreParametri.enumCancellazioneLogica.CancellazioneLogica Then
                StrSQL.Length = 0
                StrSQL.Append(" UPDATE Profilazione_Dati_MacchinexCaratteristiche ")
                StrSQL.Append(" SET ")
                StrSQL.Append("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("      ,Inviato = -1 ")
                StrSQL.Append(" WHERE PivaSuperUser='" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'")
                StrSQL.Append(" AND  Id_Profilo_Dati = " & Agro_SQL_SaveNum(Id_Profilo_Dati) & " ")
                StrSQL.Append(" AND Inviato >= 0")
            Else
                StrSQL.Length = 0
                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM   Profilazione_Dati_MacchinexCaratteristiche ")
                StrSQL.Append(" WHERE PivaSuperUser='" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'")
                StrSQL.Append(" AND   Id_Profilo_Dati = " & Agro_SQL_SaveNum(Id_Profilo_Dati) & " ")
            End If

            If Mac_Cod <> 0 Then
                StrSQL.Append(" AND   Mac_Cod = " & Agro_SQL_SaveNum(Mac_Cod) & " ")
            End If

            If Mac_Car_Cod <> 0 Then
                StrSQL.Append(" AND   Mac_Car_Cod = " & Agro_SQL_SaveNum(Mac_Car_Cod) & " ")
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

End Class
