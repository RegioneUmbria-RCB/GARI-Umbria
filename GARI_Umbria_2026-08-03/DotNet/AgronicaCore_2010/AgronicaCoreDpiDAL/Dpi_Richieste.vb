Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider


Public Class Dpi_Richieste_Write
    Inherits AgronicaCoreDataProvider.DataProvider


    '============================================================================
    Public Function Scrivi( _
                            ByVal Descrizione As String, _
                            ByVal Richiesta_Xml As String, _
                            ByVal Richiesta_Sql As String, _
                            ByVal Disciplinare_Cod As Long, _
                            ByVal Priorita As Integer, _
                            ByVal Progresso As Integer, _
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                    ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreDpiDAL.Dpi_Richieste_Write.Scrivi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.Append("INSERT INTO DPI_Richieste_Conformita " & _
                    "     (Piva_SuperUser,     Descrizione,   Richiesta_Xml,     Richiesta_Sql, " & _
                    "      Disciplinare_Cod,   Priorita,      " & _
                    "      Username_Creazione, Username_Modifica, Progresso,     " & _
                    "      Progresso_Inizio,   Progresso_Fine,                   " & _
                    "      Data_Creazione,     Data_Modifica,                    " & _
                    "      Inviato,            DataInvio    )")

            StrSQL.Append(" VALUES (" & _
                   "          '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'   " & _
                   "         ,'" & Agro_SQL_SaveText(Descrizione) & "'   " & _
                   "         ,'" & Agro_SQL_SaveText(Richiesta_Xml) & "'   " & _
                   "         ,'" & Agro_SQL_SaveText(Richiesta_Sql) & "'   " & _
                   "         , " & Agro_SQL_SaveNum(Disciplinare_Cod) & "  " & _
                   "         , " & Agro_SQL_SaveNum(Priorita) & "  " & _
                   "         ,'" & Agro_SQL_SaveText(objParametri.UtenteCodFiscale) & "' " & _
                   "         ,'" & Agro_SQL_SaveText(objParametri.UtenteCodFiscale) & "' " & _
                   "         , " & Agro_SQL_SaveNum(Progresso) & "  " & _
                   "         , Null " & _
                   "         , Null " & _
                   "         , " & Agro_SQL_SaveDate(Now.Today) & "  " & _
                   "         , " & Agro_SQL_SaveDate(Now.Today) & "  " & _
                   "         , 0 " & _
                   "         , Null " & _
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

    '============================================================================
    Public Function Modifica_Progresso( _
                            ByVal Id_Richiesta As Integer, _
                            ByVal Progresso As Integer, _
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                    ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreDpiDAL.Dpi_Richieste_Write.Modifica_Progresso()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            StrSQL.Length = 0
            StrSQL.Append(" UPDATE DPI_Richieste_Conformita ")
            StrSQL.Append(" SET ")
            StrSQL.Append("  Progresso             = " & Agro_SQL_SaveNum(Progresso) & " ")
            StrSQL.Append(" ,Progresso_Inizio     = " & Agro_SQL_SaveDate(Now) & " ")
            StrSQL.Append(" ,Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UtenteCodFiscale) & "' ")
            StrSQL.Append(" WHERE  Id_Richiesta = " & Agro_SQL_SaveNum(Id_Richiesta) & " ")

            If Trim(objParametri.PivaSuperUser) <> "" Then
                StrSQL.Append(" AND DPI_Richieste_Conformita.Piva_SuperUser =  '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'  ")
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


 
    '============================================================================
    Public Function Cancella( _
                            ByVal Id_Richiesta As Integer, _
                            ByVal xFiltroAggiuntivo As String, _
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                    ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreDpiDAL.Dpi_Richieste_Write.Cancella()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = AgronicaCoreDataProvider.AgronicaCoreParametri.enumCancellazioneLogica.CancellazioneLogica Then

                StrSQL.Length = 0
                StrSQL.Append(" UPDATE DPI_Richieste_Conformita ")
                StrSQL.Append(" SET ")
                StrSQL.Append("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("      ,Inviato = -1 ")
                StrSQL.Append(" WHERE  Inviato > 0  ")

                If Id_Richiesta <> 0 Then
                    StrSQL.Append(" AND Id_Richiesta = " & Agro_SQL_SaveNum(Id_Richiesta) & " ")
                End If

                If Trim(objParametri.PivaSuperUser) <> "" Then
                    StrSQL.Append(" AND DPI_Richieste_Conformita.Piva_SuperUser =  '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'  ")
                End If

            Else

                StrSQL.Length = 0
                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM     DPI_Richieste_Conformita ")
                StrSQL.Append(" WHERE    Inviato = 0  ")


                If Id_Richiesta <> 0 Then
                    StrSQL.Append(" AND Id_Richiesta = " & Agro_SQL_SaveNum(Id_Richiesta) & " ")
                End If

                If Trim(objParametri.PivaSuperUser) <> "" Then
                    StrSQL.Append(" AND DPI_Richieste_Conformita.Piva_SuperUser =  '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'  ")
                End If

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

Public Class Dpi_Richieste_Read
    Inherits AgronicaCoreDataProvider.DataProvider

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="Piva_SuperUser"></param>
    ''' <param name="Progresso"></param>
    ''' <param name="Username_Creazione"></param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="xOrderBy"></param>
    ''' <param name="objParametri"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[magnani]	03/05/2011	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Function Leggi( _
                          ByVal Progresso As Integer, _
                                    ByVal xFiltroAggiuntivo As String, _
                                    ByVal xOrderBy As String, _
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                    ) As DataTable



        Dim NomeRoutine As String = "AgronicaCoreDpiDAL.Dpi_Richieste_Read.Leggi()"


        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Try


            StrSQL.Length = 0

            StrSQL.Append(" SELECT *")

            StrSQL.Append(" FROM    DPI_Richieste_Conformita")

            StrSQL.Append(" WHERE DPI_Richieste_Conformita.Data_Creazione < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.Append("AND   DPI_Richieste_Conformita.Data_Creazione > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            'If Id_Richiesta <> 0 Then
            '    StrSQL.Append(" AND Id_Richiesta.Progresso =  " & Agro_SQL_SaveNum(Id_Richiesta) & "  ")
            'End If

            If Trim(objParametri.PivaSuperUser) <> "" Then
                StrSQL.Append(" AND DPI_Richieste_Conformita.Piva_SuperUser =  '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'  ")
            End If

            If Progresso <> -1 Then
                StrSQL.Append(" AND DPI_Richieste_Conformita.Progresso =  " & Agro_SQL_SaveNum(Progresso) & "  ")
            End If

            If Trim(objParametri.UtenteCodFiscale) <> "" Then
                StrSQL.Append(" AND DPI_Richieste_Conformita.Username_Creazione =  '" & Agro_SQL_SaveText(objParametri.UtenteCodFiscale) & "'  ")
            End If


            '---------------------------------------------

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita

            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY Priorita Desc, Data_Creazione Asc ")
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