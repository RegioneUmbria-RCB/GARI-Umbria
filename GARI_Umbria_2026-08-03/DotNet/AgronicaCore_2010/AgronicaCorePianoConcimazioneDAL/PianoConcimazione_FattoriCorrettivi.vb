Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider

Public Class PianoConcimazione_FattoriCorrettivi_R
    Inherits AgronicaCoreDataProvider.DataProvider


    '##############################################################################################
    Public Function Leggi(ByVal Regolamento_Cod As Integer, _
                          ByVal PC_Testata_Cod As Integer, _
                          ByVal Fattore_Cod As Integer, _
                          ByVal xFiltroAggiuntivo As String, _
                          ByVal xOrderBy As String, _
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                          ) As DataTable


        Dim NomeRoutine As String = "AgronicaCorePianoConcimazioneDAL.PianoConcimazione_FattoriCorrettivi_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0

            'StrSQL.Append(" SELECT FattoriCorrettivi.*, PianoConcimazione_FattoriCorrettivi.PC_Testata_Cod, PianoConcimazione_FattoriCorrettivi.Piva_SuperUser " + vbCrLf)
            StrSQL.Append(" SELECT PianoConcimazione_FattoriCorrettivi.Fattore_Cod, PianoConcimazione_FattoriCorrettivi.PC_Testata_Cod " & vbCrLf)
            StrSQL.Append(" FROM  PianoConcimazione_FattoriCorrettivi " & vbCrLf)
            'StrSQL.Append(" INNER JOIN  FattoriCorrettivi ON PianoConcimazione_FattoriCorrettivi.Regolamento_Cod = FattoriCorrettivi.Regolamento_Cod " + vbCrLf)
            'StrSQL.Append(" AND PianoConcimazione_FattoriCorrettivi.Fattore_Cod = FattoriCorrettivi.Fattore_Cod ")
            StrSQL.Append(" WHERE Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' " & vbCrLf)

            If Regolamento_Cod <> 0 Then
                StrSQL.Append(" AND PianoConcimazione_FattoriCorrettivi.Regolamento_Cod = " & Agro_SQL_SaveNum(Regolamento_Cod) & " " & vbCrLf)
            End If

            If PC_Testata_Cod <> 0 Then
                StrSQL.Append(" AND PianoConcimazione_FattoriCorrettivi.PC_Testata_Cod = " & Agro_SQL_SaveNum(PC_Testata_Cod) & " " & vbCrLf)
            End If

            If Fattore_Cod <> 0 Then
                StrSQL.Append(" AND PianoConcimazione_FattoriCorrettivi.Fattore_Cod = " & Agro_SQL_SaveNum(Fattore_Cod) & " " & vbCrLf)
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri) & vbCrLf)
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   PianoConcimazione_FattoriCorrettivi.Inviato >=0 " & vbCrLf)
                    'StrSQL.Append(" AND   FattoriCorrettivi.Inviato >=0 " + vbCrLf)
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   PianoConcimazione_FattoriCorrettivi.Inviato =-1 " & vbCrLf)
                    'StrSQL.Append(" AND   FattoriCorrettivi.Inviato =-1 " + vbCrLf)
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)" & vbCrLf)
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



    '##############################################################################################
    'Public Function LeggiDose(ByVal Regolamento_Cod As Integer, _
    '                          ByVal PC_Testata_Cod As Integer, _
    '                          ByVal Variazione As String, _
    '                          ByVal Tipo As String, _
    '                          ByVal xFiltroAggiuntivo As String, _
    '                          ByVal xOrderBy As String, _
    '                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
    '                          ) As DataTable


    '    Dim NomeRoutine As String = "AgronicaCorePianoConcimazioneDAL.PianoConcimazione_FattoriCorrettivi_R.LeggiDose()"

    '    Dim MessaggioErrore As String = ""
    '    Dim StrSQL As New System.Text.StringBuilder
    '    Dim DT As DataTable

    '    Try

    '        StrSQL.Length = 0

    '        StrSQL.Append(" SELECT FattoriCorrettivi.*, PianoConcimazione_FattoriCorrettivi.PC_Testata_Cod, PianoConcimazione_FattoriCorrettivi.Piva_SuperUser " + vbCrLf)
    '        StrSQL.Append(" FROM  PianoConcimazione_FattoriCorrettivi " + vbCrLf)
    '        StrSQL.Append(" INNER JOIN  FattoriCorrettivi ON PianoConcimazione_FattoriCorrettivi.Regolamento_Cod = FattoriCorrettivi.Regolamento_Cod " + vbCrLf)
    '        StrSQL.Append(" AND PianoConcimazione_FattoriCorrettivi.Fattore_Cod = FattoriCorrettivi.Fattore_Cod ")
    '        StrSQL.Append(" WHERE Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' " + vbCrLf)

    '        If Regolamento_Cod <> 0 Then
    '            StrSQL.Append(" AND PianoConcimazione_FattoriCorrettivi.Regolamento_Cod = " & Agro_SQL_SaveNum(Regolamento_Cod) & " " + vbCrLf)
    '        End If

    '        If PC_Testata_Cod <> 0 Then
    '            StrSQL.Append(" AND PianoConcimazione_FattoriCorrettivi.PC_Testata_Cod = " & Agro_SQL_SaveNum(PC_Testata_Cod) & " " + vbCrLf)
    '        End If

    '        If Variazione <> "" Then
    '            StrSQL.Append(" AND FattoriCorrettivi.Variazione = '" & Agro_SQL_SaveText(Variazione) & "' " + vbCrLf)
    '        End If

    '        If Tipo <> "" Then
    '            StrSQL.Append(" AND FattoriCorrettivi.Tipo = '" & Agro_SQL_SaveText(Tipo) & "' " + vbCrLf)
    '        End If

    '        '--------------------------------------------------------------------------
    '        If xFiltroAggiuntivo <> "" Then
    '            StrSQL.Append(" AND " & xFiltroAggiuntivo + vbCrLf)
    '        End If
    '        '--------------------------------------------------------------------------
    '        Select Case objParametri.FlagVisibilita
    '            Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
    '                StrSQL.Append(" AND   PianoConcimazione_FattoriCorrettivi.Inviato >=0 " + vbCrLf)
    '                StrSQL.Append(" AND   FattoriCorrettivi.Inviato >=0 " + vbCrLf)
    '            Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
    '                StrSQL.Append(" AND   PianoConcimazione_FattoriCorrettivi.Inviato =-1 " + vbCrLf)
    '                StrSQL.Append(" AND   FattoriCorrettivi.Inviato =-1 " + vbCrLf)
    '            Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
    '                '...................................
    '            Case Else
    '                Throw New Exception("Parametro non corretto nella query (FlagVisibilita)" + vbCrLf)
    '        End Select
    '        '--------------------------------------------------------------------------
    '        If xOrderBy <> "" Then
    '            strSql.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
    '        End If


    '        '--------------------------------------------------------------------------
    '        DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
    '        '--------------------------------------------------------------------------

    '    Catch ex As Exception

    '        MessaggioErrore = ex.Message
    '        Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
    '        DT = Nothing
    '        Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

    '    End Try

    '    Return DT

    'End Function


End Class


'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################


'##############################################################################################
Public Class PianoConcimazione_FattoriCorrettivi_W
    Inherits AgronicaCoreDataProvider.DataProvider


    '##############################################################################################
    Public Function Scrivi(ByVal Regolamento_Cod As Integer, _
                           ByVal PC_Testata_Cod As Integer, _
                           ByVal Fattore_Cod As Integer, _
                           ByVal Validita_Inizio As Date, _
                           ByVal Validita_Fine As Date, _
                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                           ) As Boolean


        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.PianoConcimazione_FattoriCorrettivi_W.Scrivi()"

        '====================================================================================
        'Parametri opzionali :

        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append(" INSERT INTO PianoConcimazione_FattoriCorrettivi ")

            StrSQL.Append("             (Piva_SuperUser, PC_Testata_Cod, Regolamento_Cod, Fattore_Cod, ")
            StrSQL.Append("              Inviato,            datainvio, ")
            StrSQL.Append("              Data_Creazione,     Data_Modifica, ")
            StrSQL.Append("              UserName_Creazione, UserName_Modifica, ")
            StrSQL.Append("              Validita_Inizio,    Validita_Fine ")
            StrSQL.Append("              ) ")

            StrSQL.Append(" VALUES (")
            StrSQL.Append("          '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(PC_Testata_Cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Regolamento_Cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Fattore_Cod) & "  ")
            StrSQL.Append("         , 0  ")
            StrSQL.Append("         , Null  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Date.Now) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Date.Now) & "  ")
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


    '##############################################################################################
    Public Function Cancella(ByVal Regolamento_Cod As Integer, _
                             ByVal PC_Testata_Cod As Integer, _
                             ByVal Fattore_Cod As Integer, _
                               ByVal xFiltroAggiuntivo As String, _
                                   ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                   ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.PianoConcimazione_FattoriCorrettivi_W.Cancella()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If PC_Testata_Cod = 0 Then
                Throw New Exception("Parametro non corretto nella query (PC_Testata_Cod obbligatorio)")
            End If

            'If Regolamento_Cod = 0 Then
            '    Throw New Exception("Parametro non corretto nella query (Regolamento_Cod obbligatorio)")
            'End If

            '---------------------------------------------
            StrSQL.Length = 0

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then

                StrSQL.Length = 0
                StrSQL.Append(" UPDATE PianoConcimazione_FattoriCorrettivi ")
                StrSQL.Append(" SET ")
                StrSQL.Append("         Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("         ,Data_Modifica= " & Agro_SQL_SaveDate(Date.Now) & " ")
                StrSQL.Append("         ,Inviato = -1 ")
                StrSQL.Append(" WHERE   Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
                StrSQL.Append(" AND     PC_Testata_Cod = " & Agro_SQL_SaveNum(PC_Testata_Cod) & " ")

                If Regolamento_Cod <> 0 Then
                    StrSQL.Append(" AND     Regolamento_Cod = " & Agro_SQL_SaveNum(Regolamento_Cod) & " ")
                End If

                StrSQL.Append(" AND     Inviato >= 0 ")

            Else

                StrSQL.Length = 0
                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM    PianoConcimazione_FattoriCorrettivi ")
                StrSQL.Append(" WHERE   Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
                StrSQL.Append(" AND     PC_Testata_Cod = " & Agro_SQL_SaveNum(PC_Testata_Cod) & " ")
                If Regolamento_Cod <> 0 Then
                    StrSQL.Append(" AND     Regolamento_Cod = " & Agro_SQL_SaveNum(Regolamento_Cod) & " ")
                End If

            End If

            If Fattore_Cod <> 0 Then
                StrSQL.Append(" AND Fattore_Cod = " & Agro_SQL_SaveNum(Fattore_Cod) & " ")
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
