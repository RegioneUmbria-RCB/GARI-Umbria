Imports AgronicaCoreDataProvider.UtilityProvider
Public Class Note_Intervento_UtilizzoXGruppi_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi( _
                                ByVal NotaGruppo_Cod As Integer, _
                                ByVal NotaUtilizzo_Cod As Integer, _
                                ByVal Validita_Inizio As Date, _
                                ByVal Validita_Fine As Date, _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.Note_Intervento_UtilizzoXGruppi_R.Leggi"
        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Append(" SELECT  *  ")
            StrSQL.Append(" FROM  Note_Intervento_UtilizzoXGruppi ")
            StrSQL.Append(" WHERE PivaSuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.Append(" AND   Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & "  ")
            StrSQL.Append(" AND   Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & "  ")

            If NotaGruppo_Cod <> 0 Then
                StrSQL.Append(" AND NotaGruppo_Cod = " & Agro_SQL_SaveNum(NotaGruppo_Cod) & " ")
            End If

            If NotaUtilizzo_Cod <> 0 Then
                StrSQL.Append(" AND NotaUtilizzo_Cod = " & Agro_SQL_SaveNum(NotaUtilizzo_Cod) & " ")
            End If

            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function


End Class


Public Class Note_Intervento_UtilizzoXGruppi_W
    Inherits AgronicaCoreDataProvider.DataProvider



    Public Function Note_Intervento_UtilizzoxGruppi_MarcaComeInviato( _
            ByVal NotaUtilizzo_Cod As Int32, _
            ByVal NotaGruppo_Cod As Int32, _
            ByVal Data_invio As DateTime, _
            ByVal xFiltroAggiuntivo As String, _
            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
        ) _
        As Boolean

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.NoteItnervento_W.Note_Intervento_Grupp_MarcaComeInviato()"

        '====================================================================================
        'Parametri opzionali :
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        '------------------------------

        Try




            If NotaUtilizzo_Cod = 0 Then
                Throw New Exception("Parametro non corretto nella query (NotaGruppo_Cod obbligatorio)")
            End If

            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.Append(" UPDATE Note_Intervento_UtilizzoxGruppi SET ")
            StrSQL.Append("     inviato         =  -2 ")
            StrSQL.Append("    ,data_invio         =  " & Agro_SQL_SaveDateTime(Data_invio))

            StrSQL.Append(" WHERE   PivaSuperUser        = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'  ")
            StrSQL.Append(" AND     NotaUtilizzo_Cod   = " & Agro_SQL_SaveNum(NotaUtilizzo_Cod) & "  ")
            StrSQL.Append(" AND     NotaGruppo_Cod   = " & Agro_SQL_SaveNum(NotaGruppo_Cod) & "  ")


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



    Public Function Scrivi( _
                            ByVal NotaGruppo_Cod As Integer, _
                            ByVal NotaUtilizzo_Cod As Integer, _
                            ByVal Validita_Inizio As Date, _
                            ByVal Validita_Fine As Date, _
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                            ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.Note_Intervento_UtilizzoXGruppi_W.Scrivi"

        '====================================================================================
        'Parametri opzionali :
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If objParametri.PivaSuperUser = "" Then
                Throw New Exception("Parametro non corretto nella query (PivaSuperUser obbligatorio)")
            End If

            If NotaGruppo_Cod = 0 Then
                Throw New Exception("Parametro non corretto nella query (NotaGruppo_Cod obbligatorio)")
            End If

            If NotaUtilizzo_Cod = 0 Then
                Throw New Exception("Parametro non corretto nella query (NotaUtilizzo_Cod obbligatorio)")
            End If


            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.Append(" INSERT INTO Note_Intervento_UtilizzoxGruppi (")
            StrSQL.Append("               [PivaSuperUser],[NotaGruppo_Cod],[NotaUtilizzo_Cod],")
            StrSQL.Append("               Inviato,            DataInvio, ")
            StrSQL.Append("               Data_Creazione,     Data_Modifica, ")
            StrSQL.Append("               UserName_Creazione, UserName_Modifica, ")
            StrSQL.Append("               Validita_Inizio,    Validita_Fine ")
            StrSQL.Append("               ) ")

            StrSQL.Append(" VALUES (")
            StrSQL.Append("         '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(NotaGruppo_Cod) & "  ")
            StrSQL.Append("         ," & Agro_SQL_SaveNum(NotaUtilizzo_Cod) & "  ")

            StrSQL.Append("         , 0  ")
            StrSQL.Append("         , Null  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDateTime(Date.Now) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDateTime(Date.Now) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")
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


    Public Function Cancella( _
                            ByVal NotaGruppo_Cod As Integer, _
                            ByVal NotaUtilizzo_Cod As Integer, _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.Note_Intervento_UtilizzoXGruppi_W.Cancella"

        '====================================================================================
        'Parametri opzionali :
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False


        Try

            If objParametri.PivaSuperUser = "" Then
                Throw New Exception("Parametro non corretto nella query (PivaSuperUser obbligatorio)")
            End If

            If NotaGruppo_Cod = 0 Then
                Throw New Exception("Parametro non corretto nella query (NotaGruppo_Cod obbligatorio)")
            End If



            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = AgronicaCoreDataProvider.AgronicaCoreParametri.enumCancellazioneLogica.CancellazioneLogica Then

                StrSQL.Length = 0
                StrSQL.Append(" UPDATE Note_Intervento_UtilizzoxGruppi ")
                StrSQL.Append(" SET ")
                StrSQL.Append("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("      ,Inviato = -1 ")
                StrSQL.Append(" WHERE  Inviato >= 0 ")
            Else
                StrSQL.Length = 0
                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM Note_Intervento_UtilizzoxGruppi ")
                StrSQL.Append(" WHERE  1=1 ")
            End If

            StrSQL.Append(" AND PivaSuperUser    = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.Append(" AND NotaGruppo_Cod   =  " & Agro_SQL_SaveNum(NotaGruppo_Cod) & "   ")
            If (NotaUtilizzo_Cod > 0) Then
                StrSQL.Append(" AND NotaUtilizzo_Cod   =  " & Agro_SQL_SaveNum(NotaUtilizzo_Cod) & "   ")
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
