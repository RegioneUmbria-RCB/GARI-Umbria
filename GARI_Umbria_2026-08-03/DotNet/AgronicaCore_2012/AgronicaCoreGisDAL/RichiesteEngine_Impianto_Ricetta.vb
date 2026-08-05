Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider

Public Class RichiesteEngine_Impianto_Ricetta_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function LeggiById(ByVal Id As Integer,
                              ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.RichiesteEngine_Impianto_Ricetta_R.LeggiById()"

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            Stb.Length = 0
            Stb.AppendLine("SELECT ")
            Stb.AppendLine("ID, request_id, piva, sa_cod, appezza, id_imp, ricetta_operazione_cod, ")
            Stb.AppendLine("inviato, datainvio, Data_Creazione, Data_Modifica, ")
            Stb.AppendLine("Username_Creazione, Username_Modifica, Validita_Inizio, Validita_Fine ")
            Stb.AppendLine("FROM RichiesteEngine_Impianto_Ricetta ")
            Stb.AppendLine("WHERE ID = " & Agro_SQL_SaveNum(Id) & " ")

            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    Stb.AppendLine("AND RichiesteEngine_Impianto_Ricetta.Inviato >= 0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    Stb.AppendLine("AND RichiesteEngine_Impianto_Ricetta.Inviato = -1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select

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

    Public Function LeggiByRequestId(ByVal Request_Id As String,
                                     ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.RichiesteEngine_Impianto_Ricetta_R.LeggiByRequestId()"

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            Stb.Length = 0
            Stb.AppendLine("SELECT ")
            Stb.AppendLine("ID, request_id, piva, sa_cod, appezza, id_imp, ricetta_operazione_cod, ")
            Stb.AppendLine("inviato, datainvio, Data_Creazione, Data_Modifica, ")
            Stb.AppendLine("Username_Creazione, Username_Modifica, Validita_Inizio, Validita_Fine ")
            Stb.AppendLine("FROM RichiesteEngine_Impianto_Ricetta ")
            Stb.AppendLine("WHERE request_id = '" & Agro_SQL_SaveText(Request_Id) & "' ")

            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    Stb.AppendLine("AND RichiesteEngine_Impianto_Ricetta.Inviato >= 0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    Stb.AppendLine("AND RichiesteEngine_Impianto_Ricetta.Inviato = -1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select

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

    Public Function LeggiByImpianto(ByVal Piva As String,
                                    ByVal Sa_Cod As Integer,
                                    ByVal Appezza As Integer,
                                    ByVal Id_Imp As Integer,
                                    ByVal Ricetta_Operazione_Cod As Integer,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.RichiesteEngine_Impianto_Ricetta_R.LeggiByImpianto()"

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            Stb.Length = 0
            Stb.AppendLine("SELECT ")
            Stb.AppendLine("  ID, request_id, piva, sa_cod, appezza, id_imp, ricetta_operazione_cod, ")
            Stb.AppendLine("  inviato, datainvio, Data_Creazione, Data_Modifica, ")
            Stb.AppendLine("  Username_Creazione, Username_Modifica, Validita_Inizio, Validita_Fine ")
            Stb.AppendLine("FROM RichiesteEngine_Impianto_Ricetta ")
            Stb.AppendLine("WHERE piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            Stb.AppendLine("  AND sa_cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            Stb.AppendLine("  AND appezza = " & Agro_SQL_SaveNum(Appezza) & " ")
            Stb.AppendLine("  AND id_imp = " & Agro_SQL_SaveNum(Id_Imp) & " ")
            Stb.AppendLine("  AND ricetta_operazione_cod = " & Agro_SQL_SaveNum(Ricetta_Operazione_Cod) & " ")

            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    Stb.AppendLine("AND RichiesteEngine_Impianto_Ricetta.Inviato >= 0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    Stb.AppendLine("AND RichiesteEngine_Impianto_Ricetta.Inviato = -1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select

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


Public Class RichiesteEngine_Impianto_Ricetta_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Scrivi(ByVal Request_Id As String,
                           ByVal Piva As String,
                           ByVal Sa_Cod As Integer,
                           ByVal Appezza As Integer,
                           ByVal Id_Imp As Integer,
                           ByVal RicettaOperazioneCod As Integer,
                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.RichiesteEngine_Impianto_Ricetta_W.Scrivi()"

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            Dim objSeq As New AgronicaCoreDataProvider.Agro_Sequenze
            Dim nuovoId As Integer = objSeq.NuovoId_Tabella(
                "RichiesteEngine_Impianto_Ricetta",
                0,
                AgronicaCoreDataProvider.CostantiPersonalizzate.UpperBoundTabelle_Per_SequenzaTabelle_Topcode,
                objParametri)

            Stb.Length = 0
            Stb.AppendLine("INSERT INTO RichiesteEngine_Impianto_Ricetta")
            Stb.AppendLine("(")
            Stb.AppendLine("  ID,")
            Stb.AppendLine("  request_id,")
            Stb.AppendLine("  piva,")
            Stb.AppendLine("  sa_cod,")
            Stb.AppendLine("  appezza,")
            Stb.AppendLine("  id_imp,")
            Stb.AppendLine("  ricetta_operazione_cod,")
            Stb.AppendLine("  Inviato,")
            Stb.AppendLine("  DataInvio,")
            Stb.AppendLine("  Data_Creazione,")
            Stb.AppendLine("  Data_Modifica,")
            Stb.AppendLine("  UserName_Creazione,")
            Stb.AppendLine("  UserName_Modifica,")
            Stb.AppendLine("  Validita_Inizio,")
            Stb.AppendLine("  Validita_Fine")
            Stb.AppendLine(")")
            Stb.AppendLine("VALUES")

            Stb.AppendLine("(")
            Stb.AppendLine(Agro_SQL_SaveNum(nuovoId) & ",")
            Stb.AppendLine("'" & Agro_SQL_SaveText(Request_Id) & "',")
            Stb.AppendLine("'" & Agro_SQL_SaveText(Piva) & "',")
            Stb.AppendLine(Agro_SQL_SaveNum(Sa_Cod) & ",")
            Stb.AppendLine(Agro_SQL_SaveNum(Appezza) & ",")
            Stb.AppendLine(Agro_SQL_SaveNum(Id_Imp) & ",")
            Stb.AppendLine(Agro_SQL_SaveNum(RicettaOperazioneCod) & ",")
            Stb.AppendLine("0,")
            Stb.AppendLine("NULL,")
            Stb.AppendLine(Agro_SQL_SaveDate(Date.Now) & ",")
            Stb.AppendLine(Agro_SQL_SaveDate(Date.Now) & ",")
            Stb.AppendLine("'" & Agro_SQL_SaveText(objParametri.SuperUserUsername) & "',")
            Stb.AppendLine("'" & Agro_SQL_SaveText(objParametri.SuperUserUsername) & "',")
            Stb.AppendLine(Agro_SQL_SaveDate(AGRODATAINIZIO) & ",")
            Stb.AppendLine(Agro_SQL_SaveDate(AGRODATAFINE))
            Stb.AppendLine(")")

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