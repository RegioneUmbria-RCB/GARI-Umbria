
Imports AgronicaCoreDataProvider.UtilityProvider


Public Class __tmp_Agenda_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi(ByVal IDTestataTemp As Integer, ByVal piva As String, ByVal id_agenda As Integer, ByVal lav_cod As Integer, ByVal objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "__tmp_Agenda_R.Leggi()"
        Dim MessaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            stb.Length = 0

            stb.AppendLine(" Select *  ")
            stb.AppendLine(" From __tmp_Agenda ")
            stb.AppendLine(" Where 1 = 1")

            If IDTestataTemp <> 0 Then
                stb.AppendLine(" and IDTestataTemp = " & Agro_SQL_SaveNum(IDTestataTemp))
            End If

            If piva <> "" Then
                stb.AppendLine(" and piva = '" & Agro_SQL_SaveText(piva) & "'")
            End If

            If id_agenda <> 0 Then
                stb.AppendLine(" and id_agenda = " & Agro_SQL_SaveNum(id_agenda))
            End If

            If lav_cod <> 0 Then
                stb.AppendLine(" and lav_cod = " & Agro_SQL_SaveNum(lav_cod))
            End If

            '-----------------------------------------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri_Server, stb.ToString, NomeRoutine)
            '-----------------------------------------------------------------------------------------------------------

        Catch ex As Exception

            DT = Nothing
            MessaggioErrore = "[" & NomeRoutine & "]:" & ex.Message & " "
            Throw New Exception(MessaggioErrore)

        End Try

        Return DT

    End Function

End Class


'#################################################################
'#################################################################
'#################################################################

Public Class __tmp_Agenda_W
    Inherits AgronicaCoreDataProvider.DataProvider



    '##############################################################################################
    Public Function ScriviDaDistinctUtenti_Visibilita(ByRef IDTestataTemp As Integer,
                                                      ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                      ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim nomeRoutine As String = "__tmp_Agenda_W.ScriviDaDistinctUtenti_Visibilita()"

        Dim messaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            Dim objUtentiVisibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
            Dim UtenteProfiloImpreseSql As String = ""

            Dim DtImpreseVisibili As DataTable = objUtentiVisibilita.OttieniPIVEVisibilita(objParametri_Server.UtenteUsername, objParametri_Server, objParametri_Utenti)

            If Not IsNothing(DtImpreseVisibili) AndAlso DtImpreseVisibili.Rows.Count > 0 Then

                Dim distinctDtImpreseVisibili As DataTable = DtImpreseVisibili.DefaultView.ToTable(True, "Piva_Azienda")

                If Not IsNothing(distinctDtImpreseVisibili) AndAlso distinctDtImpreseVisibili.Rows.Count > 0 Then

                    'Se la connessione è chiusa la apro e se la transazione è chiusa la inizio
                    AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessione(True, objParametri_Server)

                    For Each dr As DataRow In distinctDtImpreseVisibili.Rows
                        Scrivi(IDTestataTemp, dr("Piva_Azienda").ToString(), 0, 0, objParametri_Server)
                    Next

                    AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(1, objParametri_Server)

                    xRisp = True

                End If

            End If


        Catch ex As Exception
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        Finally
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessione(objParametri_Server)
        End Try

        Return xRisp

    End Function

    Public Function Scrivi(ByRef IDTestataTemp As Integer,
                            ByVal piva As String,
                            ByVal id_agenda As Integer,
                            ByVal lav_cod As Integer,
                            ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri
                            ) As Boolean
        Dim nomeRoutine As String = "__tmp_Agenda_W.Scrivi()"

        Dim messaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If IDTestataTemp = 0 Then
                Dim xAgrosequenze As New AgronicaCoreDataProvider.Agro_Sequenze
                'IDTestataTemp = xAgrosequenze.Agronica_SequenzaTabelle_NuovoID("IDTestataTemp", objParametri_Server)
                'Lavez - 12/07/2024 - normalizzazione chiamate a stack counter
                IDTestataTemp = xAgrosequenze.NuovoId_Tabella("IDTestataTemp", 0, AgronicaCoreDataProvider.CostantiPersonalizzate.UpperBoundTabelle_Per_SequenzaTabelle_Topcode, objParametri_Server)
            End If



            '---------------------------------------------
            Stb.Length = 0

            Stb.AppendLine(" INSERT INTO __tmp_Agenda ")
            Stb.AppendLine("         ( ")
            Stb.AppendLine("          IDTestataTemp, piva,  id_agenda, lav_cod")
            Stb.AppendLine("         ) ")

            Stb.AppendLine(" VALUES ( ")
            Stb.AppendLine("          " & Agro_SQL_SaveNum(IDTestataTemp))
            Stb.AppendLine("         ,'" & Agro_SQL_SaveText(piva) & "'   ")
            Stb.AppendLine("         , " & Agro_SQL_SaveNum(id_agenda) & "  ")
            Stb.AppendLine("         , " & Agro_SQL_SaveNum(lav_cod) & "  ")
            Stb.AppendLine("        ) ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri_Server, Stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function

    Public Function ScriviMassivo2(ByRef IDTestataTemp As Integer,
                                  ByRef objList As List(Of __Tmp_Agenda_Model2),
                                  ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri
                                  ) As Boolean

        Dim nomeRoutine As String = "__tmp_Agenda_W.ScriviMassivo2()"

        Dim messaggioErrore As String = ""
        Dim xRisp As Boolean = False

        Try

            If IDTestataTemp = 0 Then
                Dim xAgrosequenze As New AgronicaCoreDataProvider.Agro_Sequenze
                IDTestataTemp = xAgrosequenze.NuovoId_Tabella("IDTestataTemp", 0, AgronicaCoreDataProvider.CostantiPersonalizzate.UpperBoundTabelle_Per_SequenzaTabelle_Topcode, objParametri_Server)
            End If

            If Not IsNothing(objList) AndAlso objList.Any Then
                Dim chunks = ChunkBy(Of __Tmp_Agenda_Model2)(objList, 1000)
                For Each chunk In chunks
                    Dim Stb As New System.Text.StringBuilder
                    Stb.AppendLine(" INSERT INTO __tmp_Agenda ")
                    Stb.AppendLine("         ( ")
                    Stb.AppendLine("          IDTestataTemp, piva,  id_agenda, lav_cod")
                    Stb.AppendLine("         ) VALUES ")
                    For Each p As __Tmp_Agenda_Model2 In chunk
                        Stb.AppendLine(String.Format("({0}, '{1}', {2}, {3}),", IDTestataTemp, p.piva, p.id_agenda, p.lav_cod))
                    Next
                    Dim strSqlInsert As String = Stb.ToString
                    strSqlInsert = strSqlInsert.Remove(strSqlInsert.LastIndexOf(","))
                    Stb.Clear()
                    EseguiQuery_Scrittura(objParametri_Server, strSqlInsert, nomeRoutine)
                Next
            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp


    End Function
    Public Function ScriviMassivo(ByRef IDTestataTemp As Integer,
                                  ByRef objList As List(Of __Tmp_Agenda_Model),
                                  ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri
                                  ) As Boolean
        Dim nomeRoutine As String = "__tmp_Agenda_W.ScriviMassivo()"

        Dim messaggioErrore As String = ""
        Dim xRisp As Boolean = False

        Try

            If IDTestataTemp = 0 Then
                Dim xAgrosequenze As New AgronicaCoreDataProvider.Agro_Sequenze
                'IDTestataTemp = xAgrosequenze.Agronica_SequenzaTabelle_NuovoID("IDTestataTemp", objParametri_Server)
                'Lavez - 12/07/2024 - normalizzazione chiamate a stack counter
                IDTestataTemp = xAgrosequenze.NuovoId_Tabella("IDTestataTemp", 0, AgronicaCoreDataProvider.CostantiPersonalizzate.UpperBoundTabelle_Per_SequenzaTabelle_Topcode, objParametri_Server)
            End If

            If Not IsNothing(objList) AndAlso objList.Any Then
                Dim chunks = ChunkBy(Of __Tmp_Agenda_Model)(objList, 1000)
                For Each chunk In chunks
                    Dim Stb As New System.Text.StringBuilder
                    Stb.AppendLine(" INSERT INTO __tmp_Agenda ")
                    Stb.AppendLine("         ( ")
                    Stb.AppendLine("          IDTestataTemp, piva,  id_agenda, lav_cod, raccoglitore_cod")
                    Stb.AppendLine("         ) VALUES ")
                    For Each p As __Tmp_Agenda_Model In chunk
                        Stb.AppendLine(String.Format("({0}, '{1}', {2}, {3}, {4}),", IDTestataTemp, p.piva, p.id_agenda, p.lav_cod, p.raccoglitore_cod))
                    Next
                    Dim strSqlInsert As String = Stb.ToString
                    strSqlInsert = strSqlInsert.Remove(strSqlInsert.LastIndexOf(","))
                    Stb.Clear()
                    EseguiQuery_Scrittura(objParametri_Server, strSqlInsert, nomeRoutine)
                Next
            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function

    Public Function CancellaRecordDaIDTestataTemp(ByVal IDTestataTemp As Integer, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "__tmp_Agenda_W.CancellaRecordDaIDTestataTemp()"

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If IDTestataTemp <> 0 Then
                '---------------------------------------------
                Stb.Length = 0
                Stb.AppendLine(" Delete  ")
                Stb.AppendLine(" From __tmp_Agenda ")
                Stb.AppendLine(" Where IDTestataTemp = " & Agro_SQL_SaveNum(IDTestataTemp))

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

    Public Function CancellaRecordDaListIDTestataTemp(ByVal ListIDTestataTempIDTestataTemp As List(Of Integer), ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "__tmp_Agenda_W.CancellaRecordDaListIDTestataTemp()"

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If Not IsNothing(ListIDTestataTempIDTestataTemp) AndAlso ListIDTestataTempIDTestataTemp.Count > 0 Then
                '---------------------------------------------
                Stb.Length = 0
                Stb.AppendLine(" Delete  ")
                Stb.AppendLine(" From __tmp_Agenda ")
                Stb.AppendLine(" Where IDTestataTemp IN (" & Agro_SQL_Save_Clausola_IN(String.Join(",", ListIDTestataTempIDTestataTemp.ToArray())) & ")")

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

End Class

Public Class __Tmp_Agenda_Model2
    Public piva As String
    Public id_agenda As Integer
    Public lav_cod As Integer
End Class

Public Class __Tmp_Agenda_Model
    Public piva As String
    Public id_agenda As Integer
    Public lav_cod As Integer
    Public raccoglitore_cod As Integer
End Class