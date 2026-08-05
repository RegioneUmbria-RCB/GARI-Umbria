Imports System.Runtime.InteropServices
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.DataProviderExtensions

Public Class Materie_Prime_Alias_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function LeggiAlias(ByVal PIVA As String,
                               ByVal Mat_Cod As Integer,
                               ByVal xSelezioneVariabile As enumSelezioneVariabile,
                               ByVal xFiltroAggiuntivo As String,
                               ByVal xOrderBy As String,
                               ByRef objParametri As AgronicaCoreParametri
                               ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Materie_Prime_Alias_R.LeggiAlias()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try

            '------------------------------------------------------------------
            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi


                Case enumSelezioneVariabile.Selezione_TabellaCompleta

                    StrSQL.Length = 0

                    StrSQL.Append(" SELECT Materie_Prime.*   ")
                    StrSQL.Append(" FROM   Materie_Prime, Materie_Prime_Alias ")
                    StrSQL.Append(" WHERE  Materie_Prime_Alias.Piva_SuperUser = '" & Trim(objParametri.PivaSuperUser) & "' ")
                    StrSQL.Append(" AND    Materie_Prime_Alias.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND    Materie_Prime_Alias.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND    Materie_Prime.Mat_Cod = Materie_Prime_Alias.Mat_Cod_Alias")
                    StrSQL.Append(" AND    Materie_Prime_Alias.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & "   ")

                    If Trim(PIVA) <> "" Then
                        StrSQL.Append("  AND    Materie_Prime.Piva = '" & Agro_SQL_SaveText(Trim(PIVA)) & "' ")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   Materie_Prime.Inviato >=0 ")
                            StrSQL.Append(" AND   Materie_Prime_Alias.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Materie_Prime.Inviato =-1 ")
                            StrSQL.Append(" AND   Materie_Prime_Alias.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY Materie_Prime.Sa_Cod, Materie_Prime.Mat_Des ASC ")
                    End If

                Case enumSelezioneVariabile.Selezione_JoinCompleta


                Case enumSelezioneVariabile.Selezione_JoinDescrizioni

                    StrSQL.Length = 0

                    StrSQL.AppendLine(" SELECT Materie_Prime_Alias.*  ,Materie_Prime.Mat_Des, Materie_Prime.Inviato ")
                    StrSQL.AppendLine(" FROM   Materie_Prime_Alias, Materie_Prime ")
                    StrSQL.AppendLine(" WHERE  Materie_Prime_Alias.Piva_SuperUser = '" & Trim(objParametri.PivaSuperUser) & "' ")
                    StrSQL.AppendLine(" AND    Materie_Prime_Alias.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.AppendLine(" AND    Materie_Prime_Alias.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.AppendLine(" AND    Materie_Prime.Mat_Cod = Materie_Prime_Alias.Mat_Cod_Alias")

                    If Mat_Cod <> 0 Then
                        StrSQL.AppendLine(" AND    Materie_Prime_Alias.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & "   ")
                    End If

                    If Trim(PIVA) <> "" Then
                        StrSQL.AppendLine("  AND    Materie_Prime.Piva = '" & Agro_SQL_SaveText(Trim(PIVA)) & "' ")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.AppendLine(" AND   Materie_Prime.Inviato >=0 ")
                            StrSQL.AppendLine(" AND   Materie_Prime_Alias.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.AppendLine(" AND   Materie_Prime.Inviato =-1 ")
                            StrSQL.AppendLine(" AND   Materie_Prime_Alias.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    End If

            End Select

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function


    Public Function Leggi(ByVal PIVA As String,
                          ByVal Mat_Cod As Integer,
                          ByVal Mat_Cod_Alias As Integer,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreParametri
                          ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Materie_Prime_Alias_R.Leggi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.Append(" SELECT *   ")
            StrSQL.Append(" FROM   Materie_Prime_Alias ")
            StrSQL.Append(" WHERE  Piva_SuperUser = '" & Trim(objParametri.PivaSuperUser) & "' ")

            If Trim(PIVA) <> "" Then
                StrSQL.Append("  AND    Piva = '" & Agro_SQL_SaveText(Trim(PIVA)) & "' ")
            End If

            If Trim(Mat_Cod) <> "" Then
                StrSQL.Append(" AND    Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & "   ")
            End If

            If Trim(Mat_Cod_Alias) <> "" Then
                StrSQL.Append(" AND    Mat_Cod_Alias = " & Agro_SQL_SaveNum(Mat_Cod_Alias) & "   ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY Sa_Cod ASC ")
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function


    Public Function IsAlias(ByVal PIVA As String,
                            ByVal Mat_Cod_Alias As Integer,
                            ByRef objParametri As AgronicaCoreParametri
                            ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.Materie_Prime_Alias_R.IsAlias()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.Append(" SELECT *   ")
            StrSQL.Append(" FROM   Materie_Prime_Alias ")
            StrSQL.Append(" WHERE  Piva_SuperUser = '" & Trim(objParametri.PivaSuperUser) & "' ")

            If Trim(PIVA) <> "" Then
                StrSQL.Append("  AND    Piva = '" & Agro_SQL_SaveText(Trim(PIVA)) & "' ")
            End If

            If Trim(Mat_Cod_Alias) <> "" Then
                StrSQL.Append(" AND  Mat_Cod_Alias = " & Agro_SQL_SaveNum(Mat_Cod_Alias) & "   ")
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

            If Not IsNothing(dt) AndAlso dt.Rows.Count <> 0 Then
                Return True
            Else
                Return False
            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

    End Function

    Public Function Leggi_ConfigurazioneReferenze(ByVal PIVA As String,
                                                  ByVal Mat_Cod_Alias As Integer,
                                                  ByVal xFiltroAggiuntivo As String,
                                                  ByVal xOrderBy As String,
                                                  ByRef objParametri As AgronicaCoreParametri,
                                                  Optional ByVal Mat_Cod As Integer = 0
                                                  ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Materie_Prime_Alias_R.Leggi_ConfigurazioneReferenze()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.Append("   SELECT Materie_Prime.* " & vbCrLf)
            StrSQL.Append("   FROM Materie_Prime " & vbCrLf)
            StrSQL.Append("   WHERE Mat_Cod = (SELECT Mat_Cod " & vbCrLf)
            StrSQL.Append("                    FROM Materie_Prime_Alias " & vbCrLf)
            StrSQL.Append("                    WHERE Mat_Cod_Alias = " & Agro_SQL_SaveNum(Mat_Cod_Alias) & " " & vbCrLf)

            ' Giulia: 30/8/2017:devo aggiungerlo per evitare che la sotto query restituisca più di un risultato
            ' di fatto sarebbe anche inutile questo giro perché avendo già il mat_cod potrei direttamente andare a pescare il contenuto di materie prime
            If Mat_Cod <> 0 Then
                StrSQL.Append("                    AND Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & " " & vbCrLf)
            End If

            StrSQL.Append("                    ) " & vbCrLf)


            If Trim(PIVA) <> "" Then
                StrSQL.Append("  AND    Piva = '" & Agro_SQL_SaveText(Trim(PIVA)) & "' " & vbCrLf)
            End If


            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri) & vbCrLf)
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   Inviato >= 0 " & vbCrLf)
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   Inviato = -1 " & vbCrLf)
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY Sa_Cod ASC ")
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    Public Function LeggiAliasxGrigliaAnagraficaProdotti(ByVal PIVA As String,
                                                         ByVal Mat_Cod As Integer,
                                                         ByVal Mat_Cod_Alias As Integer,
                                                         ByVal Sa_Cod As Integer?,
                                                         ByVal Elem_Cod As Integer,
                                                         ByVal xFiltroAggiuntivo As String,
                                                         ByVal xOrderBy As String,
                                                         ByRef objParametri As AgronicaCoreParametri,
                                                         Optional ByVal cercaPubblici As Boolean = false
                                                         ) As DataTable


        Const nomeRoutine = "AgronicaCoreContabDAL.Materie_Prime_Alias_R.LeggiAliasxGrigliaAnagraficaProdotti()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT Materie_Prime_Alias.PIVA, Materie_Prime_Alias.Sa_Cod, Materie_Prime_Alias.Mat_Cod,  ")
            StrSQL.AppendLine(" Materie_Prime_Alias.Mat_Cod_Alias, Materie_Prime.Mat_Des AS Mat_Des_Alias, ")
            StrSQL.AppendLine(" Materie_Prime_Alias.Codice_Lingua, Materie_Prime_Alias.Filtro_Contatti AS Filtro_Contatti_String ")
            StrSQL.AppendLine(" FROM   Materie_Prime_Alias ")
            StrSQL.AppendLine(" LEFT JOIN   Materie_Prime ")
            StrSQL.AppendLine(" ON   Materie_Prime.Mat_Cod = Materie_Prime_Alias.Mat_Cod_Alias ")
            StrSQL.AppendLine(" WHERE  Materie_Prime_Alias.Piva_SuperUser = '" & Trim(objParametri.PivaSuperUser) & "' ")
            StrSQL.AppendLine(" AND    Materie_Prime_Alias.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.AppendLine(" AND    Materie_Prime_Alias.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")

            If Trim(PIVA) <> "" Then
                StrSQL.Append(" AND (Materie_Prime_Alias.Piva = '" & Agro_SQL_SaveText(Trim(PIVA)) & "'")

                If cercaPubblici Then
                    StrSQL.Append(" OR Materie_Prime_Alias.Sa_Cod = -1")
                End If

                StrSQL.AppendLine(")")
            End If

            If Mat_Cod <> 0 Then
                StrSQL.AppendLine(" AND    Materie_Prime_Alias.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & "   ")
            End If

            If Mat_Cod_Alias <> 0 Then
                StrSQL.AppendLine(" AND    Materie_Prime_Alias.Mat_Cod_Alias = " & Agro_SQL_SaveNum(Mat_Cod_Alias) & "   ")
            End If

            If Sa_Cod IsNot Nothing Then
                StrSQL.AppendLine(" AND    Materie_Prime_Alias.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
            End If

            If Elem_Cod <> 0 Then
                StrSQL.AppendLine(" AND    Materie_Prime.Elem_Cod = " & Agro_SQL_SaveNum(Elem_Cod) & "   ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.AppendLine(" AND   Materie_Prime.Inviato >=0 ")
                    StrSQL.AppendLine(" AND   Materie_Prime_Alias.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.AppendLine(" AND   Materie_Prime.Inviato =-1 ")
                    StrSQL.AppendLine(" AND   Materie_Prime_Alias.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine(" ORDER BY Materie_Prime.Sa_Cod, Materie_Prime.Mat_Des ASC ")
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    Public Function Trova_Alias_Associati(ByVal PIVA As String,
                                          ByVal Mat_Cod As Integer,
                                          ByVal Cod_Risum As Integer,
                                          ByVal xFiltroAggiuntivo As String,
                                          ByVal xOrderBy As String,
                                          ByRef objParametri As AgronicaCoreParametri
                                          ) As List(Of Object)

        Const nomeRoutine = "AgronicaCoreContabDAL.Materie_Prime_Alias_R.Trova_Alias_Associati()"

        Dim messaggioErrore As String = ""
        Dim dt As DataTable

        Dim listAlias As New List(Of Object)


        Try

            'Cerco tutti gli Alias di quel Prodotto,
            'poi a vado a cercare il Cod_Risum nel campo Filtro_Contatti degli Alias, se trovo il Cod_Risum passato
            'lo aggiungo alla lista List_Alias_FiltratiXMat_Cod_E_Cod_Risum.
            'Nel frattempo valorizzo anche un'altra lista List_Alias_FiltratiXMat_Cod con tutti gli Alias che hanno il Filtro_Contatti vuoto.
            'Così se non trovo quello specifico Cod_Risum tra i Filtro_Contatti degli Alias allora prendo gli Alias col Filtro_Contatti vuoto.

            dt = LeggiAlias(PIVA, Mat_Cod,
                            enumSelezioneVariabile.Selezione_JoinDescrizioni,
                            xFiltroAggiuntivo, xOrderBy, objParametri)

            If Not IsNothing(dt) Then

                Dim List_Alias_FiltratiXMat_Cod_E_Cod_Risum As New List(Of Object)
                Dim List_Alias_FiltratiXMat_Cod As New List(Of Object)

                For Each dr As DataRow In dt.Rows

                    Dim Filtro_Contatti_String = dr("Filtro_Contatti").ToString()

                    If Not IsNothing(Filtro_Contatti_String) Then

                        If Not String.IsNullOrEmpty(Filtro_Contatti_String) Then

                            Dim Array_Filtro_Contatti = Filtro_Contatti_String.Split("|")

                            If Not IsNothing(Array_Filtro_Contatti) AndAlso Array_Filtro_Contatti.Contains(Cod_Risum) Then

                                List_Alias_FiltratiXMat_Cod_E_Cod_Risum.Add(New With {.Mat_Cod = dr("Mat_Cod_Alias"), .Mat_Des = dr("Mat_Des")})

                            End If

                        Else

                            List_Alias_FiltratiXMat_Cod.Add(New With {.Mat_Cod = dr("Mat_Cod_Alias"), .Mat_Des = dr("Mat_Des")})

                        End If

                    End If

                Next

                If List_Alias_FiltratiXMat_Cod_E_Cod_Risum.Count > 0 Then

                    listAlias = List_Alias_FiltratiXMat_Cod_E_Cod_Risum.ToList()

                Else

                    If List_Alias_FiltratiXMat_Cod.Count > 0 Then

                        listAlias = List_Alias_FiltratiXMat_Cod.ToList()

                    End If

                End If

            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return listAlias

    End Function

End Class

Public Class Materie_Prime_Alias_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Scrivi(ByVal PIVA As String,
                           ByVal Sa_Cod As Integer,
                           ByVal Mat_Cod As Integer,
                           ByVal Mat_Cod_Alias As Integer,
                           ByVal Validita_Inizio As Date,
                           ByVal Validita_Fine As Date,
                           ByVal Codice_Lingua As String,
                           ByVal Filtro_Contatti As String,
                           ByRef objParametri As AgronicaCoreParametri
                           ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Materie_Prime_Alias_W.Scrivi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            StrSQL.Length = 0
            StrSQL.AppendLine("INSERT INTO Materie_Prime_Alias( ")
            StrSQL.AppendLine("                                PIVA_SuperUser,     PIVA, ")
            StrSQL.AppendLine("                                Sa_Cod,     Mat_Cod, ")
            StrSQL.AppendLine("                                Mat_Cod_Alias, Inviato, ")
            StrSQL.AppendLine("                                DataInvio, Data_Creazione, ")
            StrSQL.AppendLine("                                Data_Modifica,     UserName_Creazione, ")
            StrSQL.AppendLine("                                UserName_Modifica, Validita_Inizio, ")
            StrSQL.AppendLine("                                Validita_Fine, Codice_Lingua, ")
            StrSQL.AppendLine("                                Filtro_Contatti ")
            StrSQL.AppendLine("                                ) ")


            StrSQL.AppendLine("VALUES ( ")

            StrSQL.AppendLine("         '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.AppendLine("         , '" & Agro_SQL_SaveText(PIVA) & "' ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Mat_Cod) & " ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Mat_Cod_Alias) & " ")

            StrSQL.AppendLine("         , 0  ")
            StrSQL.AppendLine("         , Null  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDate(Date.Now) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDate(Date.Now) & "  ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")
            StrSQL.AppendLine("         , '" & Agro_SQL_SaveText(Codice_Lingua) & "'  ")
            StrSQL.AppendLine("         , '" & Agro_SQL_SaveText(Filtro_Contatti) & "'  ")
            StrSQL.AppendLine(" )")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function

    Public Function Modifica(ByVal PIVA As String,
                             ByVal Sa_Cod As Integer,
                             ByVal Mat_Cod As Integer,
                             ByVal Mat_Cod_Alias As Integer,
                             ByVal Mat_Cod_Alias_New As Integer,
                             ByVal Validita_Inizio As Date,
                             ByVal Validita_Fine As Date,
                             ByVal Codice_Lingua As String,
                             ByVal Filtro_Contatti As String,
                             ByRef objParametri As AgronicaCoreParametri
                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Materie_Prime_Alias_W.Modifica()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine("UPDATE Materie_Prime_Alias SET ")
            StrSQL.AppendLine("       Codice_Lingua        = '" & Agro_SQL_SaveText(Codice_Lingua) & "'")
            StrSQL.AppendLine("      ,Filtro_Contatti    =  '" & Agro_SQL_SaveText(Filtro_Contatti) & "' ")

            If Mat_Cod_Alias_New <> 0 Then
                StrSQL.AppendLine("      ,Mat_Cod_Alias    =  " & Agro_SQL_SaveNum(Mat_Cod_Alias_New) & " ")
            End If

            StrSQL.AppendLine("      ,Inviato           =  0 ")
            StrSQL.AppendLine("      ,DataInvio         =  Null ")
            StrSQL.AppendLine("      ,Data_Modifica     =  " & Agro_SQL_SaveDate(Date.Now))
            StrSQL.AppendLine("      ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.AppendLine("      ,Validita_Inizio =  " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            StrSQL.AppendLine("      ,Validita_Fine =  " & Agro_SQL_SaveDate(Validita_Fine) & "  ")

            StrSQL.AppendLine(" WHERE PIVA_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.AppendLine(" AND  PIVA = '" & Agro_SQL_SaveText(PIVA) & "' ")
            StrSQL.AppendLine(" AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            StrSQL.AppendLine(" AND Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & "  ")
            StrSQL.AppendLine(" AND Mat_Cod_Alias = " & Agro_SQL_SaveNum(Mat_Cod_Alias) & "  ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function


    Public Function Cancella(ByVal PIVA As String,
                             ByVal Sa_Cod As Integer?,
                             ByVal Mat_Cod As Integer,
                             ByVal Mat_Cod_Alias As Integer,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreParametri
                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Materie_Prime_Alias_W.Cancella()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            StrSQL.Length = 0
            StrSQL.AppendLine(" DELETE ")
            StrSQL.AppendLine(" FROM  Materie_Prime_Alias ")
            StrSQL.AppendLine(" WHERE PIVA_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.AppendLine(" AND  PIVA = '" & Agro_SQL_SaveText(PIVA) & "' ")

            If Mat_Cod <> 0 Then
                StrSQL.AppendLine(" AND Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & "  ")
            End If

            If Sa_Cod IsNot Nothing Then
                StrSQL.AppendLine(" AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            End If

            If Mat_Cod_Alias <> 0 Then
                StrSQL.AppendLine(" AND Mat_Cod_Alias = " & Agro_SQL_SaveNum(Mat_Cod_Alias) & "  ")
            End If

            '----------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function

End Class
