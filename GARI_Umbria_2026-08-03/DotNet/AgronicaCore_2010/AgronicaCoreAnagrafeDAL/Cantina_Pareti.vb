Imports System.Transactions
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreModelsSTD.exceptions

Public Class Cantina_Pareti_R

    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi(ByVal Piva As String, _
                          ByVal Sa_Cod As Int32, _
                          ByVal Piano_Cod As Int32, _
                          ByVal Parete_Cod As Int32, _
                          ByVal Validita_Inizio As Date, _
                          ByVal Validita_Fine As Date, _
                          ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile, _
                          ByVal xFiltroAggiuntivo As String, _
                          ByVal xOrderBy As String, _
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                              ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreAnagraficaDAL.Cantina_Pareti_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta

                    StrSQL.Length = 0

                    StrSQL.Append(" SELECT * " & _
                                " FROM  Cantina_Pareti " & _
                                " WHERE Validita_inizio < " & Agro_SQL_SaveDate(Validita_Fine) & " " & _
                                " AND   Validita_Fine > " & Agro_SQL_SaveDate(Validita_Inizio) & " ")

                    If Piva <> "" Then
                        StrSQL.Append(" AND PIVA = '" & Agro_SQL_SaveText(Trim(Piva)) & "' ")
                    End If

                    If Sa_Cod <> 0 Then
                        StrSQL.Append(" AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
                    End If

                    If Piano_Cod <> 0 Then
                        StrSQL.Append(" AND Piano_Cod = " & Agro_SQL_SaveNum(Piano_Cod) & "  ")
                    End If

                    If Parete_Cod <> 0 Then
                        StrSQL.Append(" AND Parete_Cod = " & Agro_SQL_SaveNum(Parete_Cod) & "  ")
                    End If

                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If

                    '--------------------------------------------------------------------------

                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY Piva, Sa_Cod, Parete_Cod ASC ")
                    End If


                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni


                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta
                    '
                    '
                    '
                    '


            End Select



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


    '#############################################################################################################
    '#############################################################################################################
    '#############################################################################################################




End Class


Public Class Cantina_Pareti_W

    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Scrivi(ByVal Piva As String, _
                            ByVal Sa_Cod As Int32, _
                            ByVal Piano_Cod As Int32, _
                            ByVal Parete_Cod As Int32, _
                            ByVal DimX As Int32, _
                            ByVal DimY As Int32, _
                            ByVal Colore_Interno As Int32, _
                            ByVal Colore_Esterno As Int32, _
                            ByVal PosX As Int32, _
                            ByVal PosY As Int32, _
                            ByVal Spessore As Int32, _
                            ByVal Riempimento As Int32, _
                            ByVal UserName_Creazione As String, _
                            ByVal Validita_Inizio As Date, _
                            ByVal Validita_Fine As Date, _
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                , Optional ByVal Data_creazione As DateTime = #2/1/1900# _
                , Optional ByVal Data_modifica As DateTime = #2/1/1900# _
                , Optional ByVal username_modifica As String = "" _
                              ) As Boolean


        Dim NomeRoutine As String = "AgronicaCoreAnagraficaDAL.Cantina_Pareti_W.Scrivi()"

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

            If UserName_Creazione = "" Then
                UserName_Creazione = objParametri.UsernameOperazione
            End If

            If username_modifica = "" Then
                username_modifica = objParametri.UsernameOperazione
            End If



            StrSQL.Length = 0

            StrSQL.Append("INSERT INTO Cantina_Pareti(PIVA, Sa_Cod, Piano_Cod, Parete_Cod, " & _
                    "                    DimX, DimY, Colore_Interno, Colore_Esterno, PosX, PosY, Spessore, Riempimento,  " & _
                    "                    Inviato, DataInvio, " & _
                    "                    Data_Creazione,     Data_Modifica, " & _
                    "                    UserName_Creazione, UserName_Modifica, " & _
                    "                    Validita_Inizio,    Validita_Fine " & _
                    "                    ) " & _
                    "VALUES (" & _
                    "          '" & Agro_SQL_SaveText(Trim(Piva)) & "' " & _
                    "         , " & Agro_SQL_SaveNum(Sa_Cod) & "  " & _
                    "         , " & Agro_SQL_SaveNum(Piano_Cod) & "  " & _
                    "         , " & Agro_SQL_SaveNum(Parete_Cod) & "  " & _
                    "         , " & Agro_SQL_SaveNum(DimX) & " " & _
                    "         , " & Agro_SQL_SaveNum(DimY) & "  " & _
                    "         , " & Agro_SQL_SaveNum(Colore_Interno) & "  " & _
                    "         , " & Agro_SQL_SaveNum(Colore_Esterno) & "  " & _
                    "         , " & Agro_SQL_SaveNum(PosX) & "  " & _
                    "         , " & Agro_SQL_SaveNum(PosY) & "  " & _
                    "         , " & Agro_SQL_SaveNum(Spessore) & "  " & _
                    "         , " & Agro_SQL_SaveNum(Riempimento) & "  " & _
                    "         , 0  " & _
                    "         , Null  " & _
                    "         , " & Agro_SQL_SaveDateTime(Data_creazione) & "  " & _
                    "         , " & Agro_SQL_SaveDateTime(Data_modifica) & "  " & _
                    "         ,'" & Agro_SQL_SaveText(UserName_Creazione) & "' " & _
                    "         ,'" & Agro_SQL_SaveText(UserName_Creazione) & "' " & _
                    "         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  " & _
                    "         , " & Agro_SQL_SaveDate(Validita_Fine) & "  " & _
                    ")")


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


    '#############################################################################################################
    '#############################################################################################################
    '#############################################################################################################

    Public Sub Modifica(ByVal Piva As String, _
                                ByVal Sa_Cod As Int32, _
                                ByVal Piano_Cod As Int32, _
                                ByVal Parete_Cod As Int32, _
                                ByVal DimX As Int32, _
                                ByVal DimY As Int32, _
                                ByVal Colore_Interno As Int32, _
                                ByVal Colore_Esterno As Int32, _
                                ByVal PosX As Int32, _
                                ByVal PosY As Int32, _
                                ByVal Spessore As Int32, _
                                ByVal Riempimento As Int32, _
                                ByVal UserName_Modifica As String, _
                                ByVal Validita_Inizio As Date, _
                                ByVal Validita_Fine As Date, _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri)


        Dim NomeRoutine As String = "AgronicaCoreAnagraficaDAL.Cantina_Pareti_W.Modifica()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            StrSQL.Length = 0

            StrSQL.Append("UPDATE Cantina_Pareti SET " & _
                        "    DimX              = " & Agro_SQL_SaveNum(DimX) & "  " & _
                        "   ,DimY              = " & Agro_SQL_SaveNum(DimY) & "  " & _
                        "   ,Colore_Interno    = " & Agro_SQL_SaveNum(Colore_Interno) & "  " & _
                        "   ,Colore_Esterno    = " & Agro_SQL_SaveNum(Colore_Esterno) & "  " & _
                        "   ,PosX              = " & Agro_SQL_SaveNum(PosX) & "  " & _
                        "   ,PosY              = " & Agro_SQL_SaveNum(PosY) & "  " & _
                        "   ,Spessore          = " & Agro_SQL_SaveNum(Spessore) & "  " & _
                        "   ,Riempimento       = " & Agro_SQL_SaveNum(Riempimento) & "  " & _
                        "   ,Inviato           =  0 " & _
                        "   ,DataInvio         =  Null " & _
                        "   ,Data_Modifica     =  " & Agro_SQL_SaveDate(Date.Now) & " " & _
                        "   ,UserName_Modifica = '" & Agro_SQL_SaveText(UserName_Modifica) & "' " & _
                        "   ,Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Inizio) & " " & _
                        "   ,Validita_Fine     =  " & Agro_SQL_SaveDate(Validita_Fine) & " " & _
                        " WHERE PIVA       = '" & Agro_SQL_SaveText(Trim(Piva)) & "' " & _
                        " AND   Sa_cod     =  " & Agro_SQL_SaveNum(Sa_Cod) & "  ")

            If Piano_Cod <> 0 Then
                StrSQL.Append(" AND Piano_Cod = " & Agro_SQL_SaveNum(Piano_Cod) & "  ")
            End If

            If Parete_Cod <> 0 Then
                StrSQL.Append(" AND Parete_cod = " & Agro_SQL_SaveNum(Parete_Cod) & "  ")
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


    End Sub

    '#############################################################################################################
    '#############################################################################################################
    '#############################################################################################################

    Public Sub Cancella(ByVal UserName_Modifica As String, _
                        ByVal Piva As String, _
                        ByVal Sa_Cod As Int32, _
                        ByVal Piano_Cod As Int32, _
                        ByVal Parete_Cod As Int32, _
                        ByVal Validita_Inizio As Date, _
                        ByVal Validita_Fine As Date, _
                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri)


        Dim NomeRoutine As String = "AgronicaCoreAnagraficaDAL.Cantina_Pareti_W.Cancella()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False
        Dim DT As DataTable

        Try

            StrSQL.Length = 0

            If (Validita_Fine <> AGRODATAINIZIO) Then

                '############################
                '### Cancellazione Logica ###
                '############################

                StrSQL.Append(" UPDATE   Cantina_Pareti " &
                                " SET " &
                                "          Validita_Fine = " & Agro_SQL_SaveDate(Validita_Fine) & " " &
                                "         ,Username_Modifica = '" & Agro_SQL_SaveText(UserName_Modifica) & "' " &
                                " WHERE PIVA      = '" & Agro_SQL_SaveText(Trim(Piva)) & "' " &
                                " AND   Sa_Cod    =  " & Agro_SQL_SaveNum(Sa_Cod) & "  ")

                If Piano_Cod <> 0 Then
                    StrSQL.Append(" AND Piano_Cod = " & Agro_SQL_SaveNum(Piano_Cod) & "  ")
                End If

                If Parete_Cod <> 0 Then
                    StrSQL.Append(" AND Parete_cod = " & Agro_SQL_SaveNum(Parete_Cod) & "  ")
                End If

            Else

                '############################
                '### Cancellazione Fisica ###
                '############################

                'Verifico se il dato e' gia' stato inviato al server

                StrSQL.Append(" SELECT   Inviato " & _
                            " FROM     Cantina_Pareti " & _
                            " WHERE PIVA      = '" & Agro_SQL_SaveText(Trim(Piva)) & "' " & _
                            " AND   Sa_Cod    =  " & Agro_SQL_SaveNum(Sa_Cod) & "  ")

                If Piano_Cod <> 0 Then
                    StrSQL.Append(" AND Piano_Cod = " & Agro_SQL_SaveNum(Piano_Cod) & "  ")
                End If

                If Parete_Cod <> 0 Then
                    StrSQL.Append(" AND Parete_cod = " & Agro_SQL_SaveNum(Parete_Cod) & "  ")
                End If


                '--------------------------------------------------------------------------
                DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
                '--------------------------------------------------------------------------

                'If Not rs.EOF Then
                If DT.Rows.Count > 0 Then

                    If DT.Rows(0).Item("Inviato") = 1 Then

                        '######################################
                        '### Cancellazione Fisica Rinviata  ###
                        '######################################
                        '
                        StrSQL.Append(" UPDATE   Cantina_Pareti " &
                                        " SET " &
                                        "          Validita_Fine = " & Agro_SQL_SaveDate(CDate("31/12/1899")) & " " &
                                        "         ,Username_Modifica = '" & Agro_SQL_SaveText(UserName_Modifica) & "' " &
                                        "         ,Inviato = -1 " &
                                        " WHERE PIVA      = '" & Agro_SQL_SaveText(Trim(Piva)) & "' " &
                                        " AND   Sa_Cod    =  " & Agro_SQL_SaveNum(Sa_Cod) & "  ")

                        If Parete_Cod <> 0 Then
                            StrSQL.Append(" AND Parete_Cod = " & Agro_SQL_SaveNum(Parete_Cod) & "  ")
                        End If

                        If Piano_Cod <> 0 Then
                            StrSQL.Append(" AND Piano_Cod = " & Agro_SQL_SaveNum(Piano_Cod) & "  ")
                        End If

                        If Parete_Cod <> 0 Then
                            StrSQL.Append(" AND Parete_cod = " & Agro_SQL_SaveNum(Parete_Cod) & "  ")
                        End If

                        '
                        '######################################

                    Else

                        '######################################
                        '### Cancellazione Fisica Immediata ###
                        '######################################
                        '
                        StrSQL.Append(" DELETE " & _
                                    " FROM     Cantina_Pareti " & _
                                    " WHERE PIVA      = '" & Agro_SQL_SaveText(Trim(Piva)) & "' " & _
                                    " AND   Sa_Cod    =  " & Agro_SQL_SaveNum(Sa_Cod) & "  ")

                        If Parete_Cod <> 0 Then
                            StrSQL.Append(" AND Parete_Cod = " & Agro_SQL_SaveNum(Parete_Cod) & "  ")
                        End If

                        If Piano_Cod <> 0 Then
                            StrSQL.Append(" AND Piano_Cod = " & Agro_SQL_SaveNum(Piano_Cod) & "  ")
                        End If

                        If Parete_Cod <> 0 Then
                            StrSQL.Append(" AND Parete_cod = " & Agro_SQL_SaveNum(Parete_Cod) & "  ")
                        End If

                        '
                        '######################################

                    End If

                End If

            End If

            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try


    End Sub


    Public Sub AggiornaValiditaInizio(ByVal Piva As String, _
                                      ByVal Sa_Cod As Int32, _
                                      ByVal Piano_Cod As Int32, _
                                      ByVal Parete_Cod As Int32, _
                                      ByVal UserName_Modifica As String, _
                                      ByVal Validita_Inizio As Date, _
                                      ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri)
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Cantina_Pareti_W.AggiornaValiditaInizio()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim xRisp As Boolean = False

        Try
            StrSQL.Append("UPDATE Cantina_Pareti SET " & _
          "   UserName_Modifica = '" & Agro_SQL_SaveText(UserName_Modifica) & "'" & _
          "   ,Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Inizio) & _
          " WHERE PIVA      = '" & Agro_SQL_SaveText(Trim(Piva)) & "' " & _
          " AND   Sa_cod    =  " & Agro_SQL_SaveNum(Sa_Cod) & "  " & _
          " AND   Validita_Inizio < " & Agro_SQL_SaveDate(Validita_Inizio))

            If Piano_Cod <> 0 Then
                StrSQL.Append(" AND Piano_Cod = " & Agro_SQL_SaveNum(Piano_Cod) & "  ")
            End If

            If Parete_Cod <> 0 Then
                StrSQL.Append(" AND Parete_cod = " & Agro_SQL_SaveNum(Parete_Cod) & "  ")
            End If

            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try


    End Sub


    Public Sub AggiornaValiditaFine(ByVal Piva As String, _
                                          ByVal Sa_Cod As Int32, _
                                          ByVal Piano_Cod As Int32, _
                                          ByVal Parete_Cod As Int32, _
                                          ByVal UserName_Modifica As String, _
                                          ByVal Validita_Fine As Date, _
                                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri)

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Cantina_Pareti_W.AggiornaValiditaFine()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            StrSQL.Append("UPDATE Cantina_Pareti SET " & _
          "   UserName_Modifica = '" & Agro_SQL_SaveText(UserName_Modifica) & "'" & _
          "  ,Validita_Fine     =  " & Agro_SQL_SaveDate(Validita_Fine) & _
          " WHERE PIVA      = '" & Agro_SQL_SaveText(Trim(Piva)) & "' " & _
          " AND   Sa_Cod    =  " & Agro_SQL_SaveNum(Sa_Cod) & "  " & _
          " AND   Validita_Fine > " & Agro_SQL_SaveDate(Validita_Fine))

            If Piano_Cod <> 0 Then
                StrSQL.Append(" AND Piano_Cod = " & Agro_SQL_SaveNum(Piano_Cod) & "  ")
            End If

            If Parete_Cod <> 0 Then
                StrSQL.Append(" AND Parete_cod = " & Agro_SQL_SaveNum(Parete_Cod) & "  ")
            End If

            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try


    End Sub

#Region "Cantina Pareti EF"
    Public Function DeleteCantinaParetiEF(
                                         ByVal Piva As String,
                                         ByVal Sa_Cod As Integer,
                                         ByVal pareteCod As Integer,
                                         ByVal objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                         ByVal objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                         Optional NoteLog As String = NOTELOG_ANAGRAFE_NG,
                                         Optional SistemaOrigine As Integer = -1
                                         ) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.Cantina_Pareti_W.DeleteCantinaParetiEF()"
        Dim messaggioErrore As String = ""

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri_Server.StringaConnessione)

        Try
            Dim scopeOption As New TransactionScopeOption
            Dim transactionOptions As New TransactionOptions
            transactionOptions.IsolationLevel = IsolationLevel.ReadUncommitted
            Using scope As New TransactionScope(scopeOption, transactionOptions)
                Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

                    Dim parete = (From v In GiasContext.Cantina_Pareti Where v.PIVA = Piva And
                                                               v.sa_cod = Sa_Cod And
                                                               v.Parete_Cod = pareteCod).FirstOrDefault

                    GiasContext.Cantina_Pareti.Remove(parete)
                    GiasContext.SaveChanges()
                    scope.Complete()
                    scope.Dispose()
                End Using
            End Using
        Catch ex As GiasException
            messaggioErrore = ex.Message
            Throw ex
        Catch ex As Exception
            messaggioErrore = ex.Message
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
            Return False
        End Try

        Return True

    End Function

#End Region

End Class
