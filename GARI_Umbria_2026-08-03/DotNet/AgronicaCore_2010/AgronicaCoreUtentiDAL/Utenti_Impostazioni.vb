Imports System.Text
Imports System.Text.RegularExpressions
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.DataProviderExtensions
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreModelsSTD.utente

<CachedDataProviderAttribute("Utenti_Impostazioni_Read")>
Public Class Utenti_Impostazioni_Read
    Inherits AgronicaCoreDataProvider.CachedDataProvider

    ''' <summary>
    ''' Elenco delle impostazioni non cacheable
    ''' </summary>
    Private ReadOnly listImpostazioniToRemove As List(Of Integer) = {
        CInt(enum_Impostazioni_Utenti.UTENTE_STAMPE_PREFERITE),
        CInt(enum_Impostazioni_Utenti.SUPERUSER_GiasAPP_Permessi)
    }.ToList

    Public Shared Sub AnnataAgrariaCalcola(ByVal dataRiferimento As Date, ByRef datainizio As Date, ByRef datafine As Date, ByVal sXdate As String)
        Dim xRif As Integer = 0
        Dim dataRiferimentoXDateDay As String = dataRiferimento.Day.ToString.PadLeft(2, "0")
        Dim dataRiferimentoXDateMonth As String = dataRiferimento.Month.ToString.PadLeft(2, "0")

        Dim datainizioXDateDay As String = sXdate.Substring(0, 2).PadLeft(2, "0")
        Dim datainizioXDateMonth As String = sXdate.Substring(2, 2).PadLeft(2, "0")


        'Dim xRif As Integer = 0
        'If CInt(sXdate.Substring(2, 2) & sXdate.Substring(0, 2)) > "0801" Then
        '    xRif = -1
        'End If

        If CInt(dataRiferimentoXDateMonth & dataRiferimentoXDateDay) < CInt(datainizioXDateMonth & datainizioXDateDay) Then
            xRif = -1
        End If

        datainizio = New Date(Year(dataRiferimento) + xRif, sXdate.Substring(2, 2), sXdate.Substring(0, 2))
        datafine = datainizio.AddDays(364)

        datafine = New Date(Year(datafine), sXdate.Substring(6, 2), sXdate.Substring(4, 2))

    End Sub


    ''' <summary>
    ''' Restituisce l'annata agraria partendo dalla data impostata nel parametro data riferimento
    ''' </summary>
    ''' <param name="dataRiferimento">Data di partenza per ottenere l'Annata Agraria</param>
    ''' <param name="datainizio">restituisce la data inizio annata (rispetto al param. DataRiferimento)</param>
    ''' <param name="datafine">restituisce la data fine annata (rispetto al param. DataRiferimento)</param>
    ''' <param name="objparametri_utenti"></param>
    ''' <remarks></remarks>
    Public Sub AnnataAgraria(ByVal dataRiferimento As Date, ByRef datainizio As Date,
                             ByRef datafine As Date,
                             ByVal objparametri_utenti As AgronicaCoreDataProvider.AgronicaCoreParametri)

        Dim sXdate As String = "01013112"

        Dim dt1 As DataTable = Leggi_Utente_Poi_SuperUser(enum_Impostazioni_Utenti.UTENTE_InizioFineAnnataAgraria, 1,
                                                          enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objparametri_utenti)

        If dt1.Rows.Count > 0 Then
            sXdate = dt1.Rows(0)("impostazione_valore_1")
        End If

        AnnataAgrariaCalcola(dataRiferimento, datainizio, datafine, sXdate)

    End Sub

    'Public Function Leggi_Utente_Poi_Gruppo_Poi_SuperUser(ByVal Impostazione_Cod As Integer, _
    '                              ByVal Username_1Utente_o_2SuperUser As Integer, _
    '                                    ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile, _
    '                                    ByVal xFiltroAggiuntivo As String, _
    '                                    ByVal xOrderBy As String, _
    '                                    ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri _
    '                                    ) As DataTable
    '    Dim DT As DataTable
    '    DT = Leggi(Impostazione_Cod, 1, AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, xFiltroAggiuntivo, xOrderBy, objParametri_Utenti)

    '    If DT.Rows.Count > 0 Then
    '        Return DT
    '    Else

    '        DT = LeggiDaGruppiAppartenenza(Impostazione_Cod, 1, xFiltroAggiuntivo, xOrderBy, objParametri_Utenti)
    '        Return DT

    '        If DT.Rows.Count > 0 Then
    '            Return DT
    '        End If

    '        DT = Leggi(Impostazione_Cod, 2, AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, xFiltroAggiuntivo, xOrderBy, objParametri_Utenti)
    '        Return DT
    '    End If

    'End Function

    Public Function Leggi_Utente_Poi_SuperUser(ByVal Impostazione_Cod As Integer,
                                               ByVal NonUsato As Integer,
                                               ByVal xSelezioneVariabile As enumSelezioneVariabile,
                                               ByVal xFiltroAggiuntivo As String,
                                               ByVal xOrderBy As String,
                                               ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri
                                               ) As DataTable
        Dim dt As DataTable
        dt = Leggi_Cacheable(Impostazione_Cod, 1, enumSelezioneVariabile.Selezione_TabellaCompleta, xFiltroAggiuntivo, xOrderBy, objParametri_Utenti)

        If dt.Rows.Count > 0 Then
            Return dt
        Else
            dt = Leggi_Cacheable(Impostazione_Cod, 2, enumSelezioneVariabile.Selezione_TabellaCompleta, xFiltroAggiuntivo, xOrderBy, objParametri_Utenti)
            Return dt
        End If

    End Function

    Public Function Leggi_Utente_Poi_SuperUserNotCacheable(ByVal Impostazione_Cod As Integer,
                                               ByVal NonUsato As Integer,
                                               ByVal xSelezioneVariabile As enumSelezioneVariabile,
                                               ByVal xFiltroAggiuntivo As String,
                                               ByVal xOrderBy As String,
                                               ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri
                                               ) As DataTable
        Dim dt As DataTable
        dt = Leggi(Impostazione_Cod, 1, enumSelezioneVariabile.Selezione_TabellaCompleta, xFiltroAggiuntivo, xOrderBy, objParametri_Utenti)

        If dt.Rows.Count > 0 Then
            Return dt
        Else
            dt = Leggi(Impostazione_Cod, 2, enumSelezioneVariabile.Selezione_TabellaCompleta, xFiltroAggiuntivo, xOrderBy, objParametri_Utenti)
            Return dt
        End If

    End Function

    Public Function Leggi_Impostazione_APP(ByVal Impostazione_Cod As Integer, ByRef impostazioni As DataTable, ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean
        Dim dt As DataTable
        dt = Leggi_Cacheable(Impostazione_Cod, 1, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Utenti)
        If dt.Rows.Count = 0 Then
            dt = Leggi_Cacheable(Impostazione_Cod, 2, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Utenti)
        End If

        If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
            Dim dr = dt.Rows(0)
            Dim impostazione = impostazioni.NewRow
            impostazione.Item("Impostazione_Cod") = dr.Item("Impostazione_Cod")
            impostazione.Item("Impostazione_Valore_1") = dr.Item("Impostazione_Valore_1")
            impostazioni.Rows.Add(impostazione)
            Return True
        End If

        Return False
    End Function

    Public Function Impostazione_Valore_From_Impostazione_Cod_Utente_Poi_SuperUser(ByVal Impostazione_Cod As Integer,
                                                                                   ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                                                   ) As String
        Dim dtImpostazioni As DataTable

        dtImpostazioni = Leggi_Utente_Poi_SuperUser(Impostazione_Cod,
                                                    1,
                                                    enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                    "",
                                                    "",
                                                    objParametri_Utenti)

        If dtImpostazioni.Rows.Count > 1 Then
            Throw New Exception("Troppe righe")
        ElseIf dtImpostazioni.Rows.Count = 1 Then
            Return dtImpostazioni.Rows(0).Item("Impostazione_Valore_1")
        Else
            Return ""
        End If

    End Function

    '##############################################################################################
    'l'impostazione può essere associata all'utente o al superuser
    'quindi non si può fare il filtro fisso sulla username dell'utente
    Public Function LeggiDaGruppiAppartenenza(ByVal Impostazione_Cod As Integer,
                                              ByVal Username_1Utente_o_2SuperUser As Integer,
                                              ByVal xFiltroAggiuntivo As String,
                                              ByVal xOrderBy As String,
                                              ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri
                                              ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva_SuperUser
        '   Username
        '   Impostazione_Cod
        '   FiltroAggiuntivo
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        Try

            stb.Length = 0
            stb.AppendLine(" select i.* ")
            stb.AppendLine(" from Utenti u ")
            stb.AppendLine("    inner join Utenti_xGruppi_Utente ug ")
            stb.AppendLine("        on ug.username = u.username  ")
            stb.AppendLine("    inner join Gruppi_Utente_Impostazioni i ")
            stb.AppendLine("        on i.Gruppi_Utente_cod = ug.Gruppi_Utente_cod ")

            stb.AppendLine(" WHERE 1=1 ")

            stb.AppendLine(" AND (Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri_Utenti.PivaSuperUser) & "') ")

            If Username_1Utente_o_2SuperUser = 2 Then
                stb.AppendLine(" AND u.Username = '" & Agro_SQL_SaveText(objParametri_Utenti.SuperUserUsername) & "' ")
            Else
                stb.AppendLine(" AND u.Username = '" & Agro_SQL_SaveText(objParametri_Utenti.UtenteUsername) & "' ")
            End If

            If Impostazione_Cod <> 0 Then
                stb.AppendLine(" AND (i.Impostazione_Cod = " & Agro_SQL_SaveNum(Impostazione_Cod) & ")   ")
            End If

            If xFiltroAggiuntivo <> "" Then
                stb.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri_Utenti))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri_Utenti.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    stb.AppendLine(" AND   ug.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    stb.AppendLine(" AND   ug.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                stb.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri_Utenti))
            End If
            '---------------------------------------------

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri_Utenti, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Utenti, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try


        Return dt

    End Function

    Public Function LeggiConDefault(impostazioneCod As enum_Impostazioni_Utenti, Username_1Utente_o_2SuperUser As Integer, valoreDefault As String, objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri) As String

        Dim Impostazione_Valore_1 = ImpostazioneValore1_from_ImpostazioneCod(impostazioneCod, objParametri_Utenti, Username_1Utente_o_2SuperUser)

        If String.IsNullOrEmpty(Impostazione_Valore_1) Then
            Return valoreDefault
        Else
            Return Impostazione_Valore_1
        End If

    End Function

    '##############################################################################################
    'l'impostazione può essere associata all'utente o al superuser
    'quindi non si può fare il filtro fisso sulla username dell'utente
    Public Function Leggi(ByVal Impostazione_Cod As Integer,
                          ByVal Username_1Utente_o_2SuperUser As Integer,
                          ByVal xSelezioneVariabile As enumSelezioneVariabile,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri
                          ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva_SuperUser
        '   Username
        '   Impostazione_Cod
        '   FiltroAggiuntivo
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try
            If Impostazione_Cod > 0 AndAlso Not listImpostazioniToRemove.Contains(Impostazione_Cod) Then
                Dim listaImpostazioni = LeggiGuidaImpostazioniNonVisibili(objParametri_Utenti)
                If Not listaImpostazioni.Contains(Impostazione_Cod) Then
                    Return Leggi_Cacheable(Impostazione_Cod, Username_1Utente_o_2SuperUser, xSelezioneVariabile, xFiltroAggiuntivo, xOrderBy, objParametri_Utenti)
                End If
            End If


            '---------------------------------------------
            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                Case enumSelezioneVariabile.Selezione_TabellaCompleta
                    strSql.Length = 0
                    strSql.AppendLine(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; ")
                    strSql.AppendLine(" SELECT * ")
                    strSql.AppendLine(" FROM Utenti_Impostazioni WITH(NOLOCK)")
                    strSql.AppendLine(" WHERE 1=1 ")

                    strSql.AppendLine(" AND (Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri_Utenti.PivaSuperUser) & "') ")

                    If Username_1Utente_o_2SuperUser = 2 Then
                        strSql.AppendLine(" AND Username = '" & Agro_SQL_SaveText(objParametri_Utenti.SuperUserUsername) & "' ")
                    Else
                        strSql.AppendLine(" AND Username = '" & Agro_SQL_SaveText(objParametri_Utenti.UtenteUsername) & "' ")
                    End If

                    If Impostazione_Cod <> 0 Then
                        strSql.AppendLine(" AND (Impostazione_Cod = " & Agro_SQL_SaveNum(Impostazione_Cod) & ")   ")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri_Utenti))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri_Utenti.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            strSql.AppendLine(" AND   Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            strSql.AppendLine(" AND   Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri_Utenti))
                    End If
                    '---------------------------------------------


                Case enumSelezioneVariabile.Selezione_JoinDescrizioni
                    strSql.Length = 0
                    strSql.AppendLine(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; ")
                    strSql.AppendLine(" SELECT     Utenti_Impostazioni.Impostazione_Cod,Utenti_Impostazioni_FiltroMono.ID_0 ")
                    strSql.AppendLine(" FROM         Utenti_Impostazioni INNER JOIN ")
                    strSql.AppendLine("       Utenti_Impostazioni_FiltroMono ON Utenti_Impostazioni.Piva_SuperUser = Utenti_Impostazioni_FiltroMono.Piva_SuperUser AND ")
                    strSql.AppendLine("       Utenti_Impostazioni.UserName = Utenti_Impostazioni_FiltroMono.UserName AND ")
                    strSql.AppendLine("       Utenti_Impostazioni.Impostazione_Cod = Utenti_Impostazioni_FiltroMono.Impostazione_Cod ")

                    strSql.AppendLine(" WHERE 1=1 ")

                    strSql.AppendLine(" AND (Utenti_Impostazioni.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri_Utenti.PivaSuperUser) & "') ")

                    If Username_1Utente_o_2SuperUser = 2 Then
                        strSql.AppendLine(" AND Utenti_Impostazioni.Username = '" & Agro_SQL_SaveText(objParametri_Utenti.SuperUserUsername) & "' ")
                    Else
                        strSql.AppendLine(" AND Utenti_Impostazioni.Username = '" & Agro_SQL_SaveText(objParametri_Utenti.UtenteUsername) & "' ")
                    End If

                    If Impostazione_Cod <> 0 Then
                        strSql.AppendLine(" AND (Utenti_Impostazioni.Impostazione_Cod = " & Agro_SQL_SaveNum(Impostazione_Cod) & ")   ")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri_Utenti))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri_Utenti.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            strSql.AppendLine(" AND   Utenti_Impostazioni.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            strSql.AppendLine(" AND   Utenti_Impostazioni.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri_Utenti))
                    End If
                    '---------

                Case enumSelezioneVariabile.Selezione_JoinCompleta



            End Select

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri_Utenti, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Utenti, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function


    <Cacheable(True)>
    Public Function LeggiGuidaImpostazioniNonVisibili(
                                                     ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                     ) As List(Of Integer)

        Dim nomeRoutine As String = "AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read.LeggiGuidaImpostazioniNonVisibili()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva_SuperUser
        '   Username
        '   Impostazione_Cod
        '   FiltroAggiuntivo
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable
        Dim listaImpostazioni As New List(Of Integer)
        Try

            strSql.Length = 0
            strSql.AppendLine(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; ")
            strSql.AppendLine(" SELECT Impostazione_Cod, [Label] ")
            strSql.AppendLine(" FROM Guida_Impostazioni ")
            strSql.AppendLine(" WHERE 1=1 ")
            strSql.AppendLine(" AND Sezione = 0 ")
            strSql.AppendLine(" AND SottoSezione = 0 ")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri_Utenti, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                For Each row As DataRow In dt.Rows
                    listaImpostazioni.Add(CInt(row("Impostazione_Cod")))
                Next
            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Utenti, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return listaImpostazioni

    End Function


    <Cacheable(True)>
    Public Function Leggi_Cacheable(ByVal Impostazione_Cod As Integer,
                          ByVal Username_1Utente_o_2SuperUser As Integer,
                          ByVal xSelezioneVariabile As enumSelezioneVariabile,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri
                          ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva_SuperUser
        '   Username
        '   Impostazione_Cod
        '   FiltroAggiuntivo
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try
            '---------------------------------------------
            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                Case enumSelezioneVariabile.Selezione_TabellaCompleta
                    strSql.Length = 0
                    strSql.AppendLine(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; ")
                    strSql.AppendLine(" SELECT * ")
                    strSql.AppendLine(" FROM Utenti_Impostazioni WITH(NOLOCK)")
                    strSql.AppendLine(" WHERE 1=1 ")

                    strSql.AppendLine(" AND (Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri_Utenti.PivaSuperUser) & "') ")

                    If Username_1Utente_o_2SuperUser = 2 Then
                        strSql.AppendLine(" AND Username = '" & Agro_SQL_SaveText(objParametri_Utenti.SuperUserUsername) & "' ")
                    Else
                        strSql.AppendLine(" AND Username = '" & Agro_SQL_SaveText(objParametri_Utenti.UtenteUsername) & "' ")
                    End If

                    If Impostazione_Cod <> 0 Then
                        strSql.AppendLine(" AND (Impostazione_Cod = " & Agro_SQL_SaveNum(Impostazione_Cod) & ")   ")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri_Utenti))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri_Utenti.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            strSql.AppendLine(" AND   Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            strSql.AppendLine(" AND   Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri_Utenti))
                    End If
                    '---------------------------------------------


                Case enumSelezioneVariabile.Selezione_JoinDescrizioni
                    strSql.Length = 0
                    strSql.AppendLine(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; ")
                    strSql.AppendLine(" SELECT     Utenti_Impostazioni.Impostazione_Cod,Utenti_Impostazioni_FiltroMono.ID_0 ")
                    strSql.AppendLine(" FROM         Utenti_Impostazioni INNER JOIN ")
                    strSql.AppendLine("       Utenti_Impostazioni_FiltroMono ON Utenti_Impostazioni.Piva_SuperUser = Utenti_Impostazioni_FiltroMono.Piva_SuperUser AND ")
                    strSql.AppendLine("       Utenti_Impostazioni.UserName = Utenti_Impostazioni_FiltroMono.UserName AND ")
                    strSql.AppendLine("       Utenti_Impostazioni.Impostazione_Cod = Utenti_Impostazioni_FiltroMono.Impostazione_Cod ")

                    strSql.AppendLine(" WHERE 1=1 ")

                    strSql.AppendLine(" AND (Utenti_Impostazioni.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri_Utenti.PivaSuperUser) & "') ")

                    If Username_1Utente_o_2SuperUser = 2 Then
                        strSql.AppendLine(" AND Utenti_Impostazioni.Username = '" & Agro_SQL_SaveText(objParametri_Utenti.SuperUserUsername) & "' ")
                    Else
                        strSql.AppendLine(" AND Utenti_Impostazioni.Username = '" & Agro_SQL_SaveText(objParametri_Utenti.UtenteUsername) & "' ")
                    End If

                    If Impostazione_Cod <> 0 Then
                        strSql.AppendLine(" AND (Utenti_Impostazioni.Impostazione_Cod = " & Agro_SQL_SaveNum(Impostazione_Cod) & ")   ")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri_Utenti))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri_Utenti.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            strSql.AppendLine(" AND   Utenti_Impostazioni.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            strSql.AppendLine(" AND   Utenti_Impostazioni.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri_Utenti))
                    End If
                    '---------

                Case enumSelezioneVariabile.Selezione_JoinCompleta


            End Select

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri_Utenti, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Utenti, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    '##############################################################################################
    'l'impostazione può essere associata all'utente o al superuser
    'quindi non si può fare il filtro fisso sulla username dell'utente
    Public Function LeggiQryParametrica(ByVal Impostazione_Cod As Integer,
                                        ByVal Username_1Utente_o_2SuperUser As Integer,
                                        ByVal xSelezioneVariabile As enumSelezioneVariabile,
                                        ByVal xFiltroAggiuntivoParameters As String,
                                        ByVal xFiltroAggiuntivo As String,
                                        ByVal xOrderBy As String,
                                        ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri
                                        ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva_SuperUser
        '   Username
        '   Impostazione_Cod
        '   FiltroAggiuntivo
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            If Impostazione_Cod <> 0 Then
                Dim listaImpostazioni = LeggiGuidaImpostazioniNonVisibili(objParametri_Utenti)
                If Not listaImpostazioni.Contains(Impostazione_Cod) Then
                    Return LeggiQryParametrica_Cacheable(Impostazione_Cod, Username_1Utente_o_2SuperUser, xSelezioneVariabile, xFiltroAggiuntivoParameters, xFiltroAggiuntivo, xOrderBy, objParametri_Utenti)
                End If
            End If

            strSql.Length = 0
            strSql.Append(xFiltroAggiuntivoParameters)
            '---------------------------------------------
            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                Case enumSelezioneVariabile.Selezione_TabellaCompleta

                    strSql.AppendLine(" SELECT * ")
                    strSql.AppendLine(" FROM Utenti_Impostazioni ")
                    strSql.AppendLine(" WHERE 1=1 ")

                    strSql.AppendLine(" AND (Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri_Utenti.PivaSuperUser) & "') ")

                    If Username_1Utente_o_2SuperUser = 2 Then
                        strSql.AppendLine(" AND Username = '" & Agro_SQL_SaveText(objParametri_Utenti.SuperUserUsername) & "' ")
                    Else
                        strSql.AppendLine(" AND Username = '" & Agro_SQL_SaveText(objParametri_Utenti.UtenteUsername) & "' ")
                    End If

                    If Impostazione_Cod <> 0 Then
                        strSql.AppendLine(" AND (Impostazione_Cod = " & Agro_SQL_SaveNum(Impostazione_Cod) & ")   ")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri_Utenti))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri_Utenti.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            strSql.AppendLine(" AND   Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            strSql.AppendLine(" AND   Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        strSql.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri_Utenti))
                    End If
                    '---------------------------------------------


                Case enumSelezioneVariabile.Selezione_JoinDescrizioni
                    strSql.AppendLine(" SELECT     Utenti_Impostazioni.Impostazione_Cod,Utenti_Impostazioni_FiltroMono.ID_0 ")
                    strSql.AppendLine(" FROM         Utenti_Impostazioni INNER JOIN ")
                    strSql.AppendLine("       Utenti_Impostazioni_FiltroMono ON Utenti_Impostazioni.Piva_SuperUser = Utenti_Impostazioni_FiltroMono.Piva_SuperUser AND ")
                    strSql.AppendLine("       Utenti_Impostazioni.UserName = Utenti_Impostazioni_FiltroMono.UserName AND ")
                    strSql.AppendLine("       Utenti_Impostazioni.Impostazione_Cod = Utenti_Impostazioni_FiltroMono.Impostazione_Cod ")

                    strSql.AppendLine(" WHERE 1=1 ")

                    strSql.AppendLine(" AND (Utenti_Impostazioni.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri_Utenti.PivaSuperUser) & "') ")

                    If Username_1Utente_o_2SuperUser = 2 Then
                        strSql.AppendLine(" AND Utenti_Impostazioni.Username = '" & Agro_SQL_SaveText(objParametri_Utenti.SuperUserUsername) & "' ")
                    Else
                        strSql.AppendLine(" AND Utenti_Impostazioni.Username = '" & Agro_SQL_SaveText(objParametri_Utenti.UtenteUsername) & "' ")
                    End If

                    If Impostazione_Cod <> 0 Then
                        strSql.AppendLine(" AND (Utenti_Impostazioni.Impostazione_Cod = " & Agro_SQL_SaveNum(Impostazione_Cod) & ")   ")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri_Utenti))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri_Utenti.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            strSql.AppendLine(" AND   Utenti_Impostazioni.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            strSql.AppendLine(" AND   Utenti_Impostazioni.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri_Utenti))
                    End If
                    '---------

                Case enumSelezioneVariabile.Selezione_JoinCompleta


            End Select

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri_Utenti, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Utenti, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try


        Return dt

    End Function


    Public Function LeggiQryParametrica_Cacheable(ByVal Impostazione_Cod As Integer,
                                        ByVal Username_1Utente_o_2SuperUser As Integer,
                                        ByVal xSelezioneVariabile As enumSelezioneVariabile,
                                        ByVal xFiltroAggiuntivoParameters As String,
                                        ByVal xFiltroAggiuntivo As String,
                                        ByVal xOrderBy As String,
                                        ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri
                                        ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva_SuperUser
        '   Username
        '   Impostazione_Cod
        '   FiltroAggiuntivo
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0
            strSql.Append(xFiltroAggiuntivoParameters)
            '---------------------------------------------
            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                Case enumSelezioneVariabile.Selezione_TabellaCompleta

                    strSql.AppendLine(" SELECT * ")
                    strSql.AppendLine(" FROM Utenti_Impostazioni ")
                    strSql.AppendLine(" WHERE 1=1 ")

                    strSql.AppendLine(" AND (Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri_Utenti.PivaSuperUser) & "') ")

                    If Username_1Utente_o_2SuperUser = 2 Then
                        strSql.AppendLine(" AND Username = '" & Agro_SQL_SaveText(objParametri_Utenti.SuperUserUsername) & "' ")
                    Else
                        strSql.AppendLine(" AND Username = '" & Agro_SQL_SaveText(objParametri_Utenti.UtenteUsername) & "' ")
                    End If

                    If Impostazione_Cod <> 0 Then
                        strSql.AppendLine(" AND (Impostazione_Cod = " & Agro_SQL_SaveNum(Impostazione_Cod) & ")   ")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri_Utenti))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri_Utenti.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            strSql.AppendLine(" AND   Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            strSql.AppendLine(" AND   Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        strSql.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri_Utenti))
                    End If
                    '---------------------------------------------


                Case enumSelezioneVariabile.Selezione_JoinDescrizioni
                    strSql.AppendLine(" SELECT     Utenti_Impostazioni.Impostazione_Cod,Utenti_Impostazioni_FiltroMono.ID_0 ")
                    strSql.AppendLine(" FROM         Utenti_Impostazioni INNER JOIN ")
                    strSql.AppendLine("       Utenti_Impostazioni_FiltroMono ON Utenti_Impostazioni.Piva_SuperUser = Utenti_Impostazioni_FiltroMono.Piva_SuperUser AND ")
                    strSql.AppendLine("       Utenti_Impostazioni.UserName = Utenti_Impostazioni_FiltroMono.UserName AND ")
                    strSql.AppendLine("       Utenti_Impostazioni.Impostazione_Cod = Utenti_Impostazioni_FiltroMono.Impostazione_Cod ")

                    strSql.AppendLine(" WHERE 1=1 ")

                    strSql.AppendLine(" AND (Utenti_Impostazioni.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri_Utenti.PivaSuperUser) & "') ")

                    If Username_1Utente_o_2SuperUser = 2 Then
                        strSql.AppendLine(" AND Utenti_Impostazioni.Username = '" & Agro_SQL_SaveText(objParametri_Utenti.SuperUserUsername) & "' ")
                    Else
                        strSql.AppendLine(" AND Utenti_Impostazioni.Username = '" & Agro_SQL_SaveText(objParametri_Utenti.UtenteUsername) & "' ")
                    End If

                    If Impostazione_Cod <> 0 Then
                        strSql.AppendLine(" AND (Utenti_Impostazioni.Impostazione_Cod = " & Agro_SQL_SaveNum(Impostazione_Cod) & ")   ")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri_Utenti))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri_Utenti.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            strSql.AppendLine(" AND   Utenti_Impostazioni.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            strSql.AppendLine(" AND   Utenti_Impostazioni.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri_Utenti))
                    End If
                    '---------

                Case enumSelezioneVariabile.Selezione_JoinCompleta


            End Select

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri_Utenti, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Utenti, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try


        Return dt

    End Function


    '##############################################################################################
    'a differenza della 1, può filtrare su una username passata e non su quella dell'objparametri
    Public Function Leggi2(ByVal Username_1Utente_o_2SuperUser As Integer,
                           ByVal UsernameUtente As String,
                           ByVal Impostazione_Cod As Integer,
                           ByVal xFiltroAggiuntivo As String,
                           ByVal xOrderBy As String,
                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                           ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read.Leggi2()"
        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0
            strSql.AppendLine(" SELECT  * ")
            strSql.AppendLine(" FROM    Utenti_Impostazioni  ")
            strSql.AppendLine(" WHERE   Utenti_Impostazioni.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'   ")

            If Username_1Utente_o_2SuperUser = 2 Then
                strSql.AppendLine(" AND Utenti_Impostazioni.Username = '" & Agro_SQL_SaveText(objParametri.SuperUserUsername) & "' ")
            Else
                If UsernameUtente = "" Then
                    strSql.AppendLine(" AND     Utenti_Impostazioni.Username = '" & Agro_SQL_SaveText(objParametri.UtenteUsername) & "'   ")
                Else
                    strSql.AppendLine(" AND     Utenti_Impostazioni.Username = '" & Agro_SQL_SaveText(UsernameUtente) & "'   ")
                End If
            End If

            If Impostazione_Cod <> 0 Then
                strSql.AppendLine(" AND     Utenti_Impostazioni.Impostazione_Cod = " & Agro_SQL_SaveNum(Impostazione_Cod) & "   ")
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                strSql.AppendLine(" ORDER BY Utenti_Impostazioni.Piva_SuperUser, Utenti_Impostazioni.Username, Utenti_Impostazioni.Impostazione_Cod ")
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    Private Function GetTipologia(username As String, objUtenti As AgronicaCoreDataProvider.AgronicaCoreParametri) As String
        Dim objUtentiDal As New Utenti_Read
        Return objUtentiDal.Leggi2(username, String.Empty, String.Empty, objUtenti).Select.
            Select(Of String)(Function(row) If(IsDBNull(row("Tipologia_Cod")), String.Empty, row("Tipologia_Cod"))).
            DefaultIfEmpty(String.Empty).
            First
    End Function

    ''' <summary>
    ''' Legge il valore dell'impostazione a scalare prima su utente, poi tipologia dell'utente e poi superuser.
    ''' </summary>
    <Obsolete("Use LeggiImpostazioneScalare instead.")>
    Public Function LeggiUtentePoiSuperUser(
        ByVal codiceImpostazione As Integer,
        ByVal username As String,
        ByVal xFiltroAggiuntivo As String,
        ByVal xOrderBy As String,
        ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri
    ) As DataTable
        Dim isEmpty = Function(table As DataTable) IsNothing(table) OrElse table.Rows.Count = 0
        Dim dt As DataTable = Leggi2(
            1, username, codiceImpostazione,
            xFiltroAggiuntivo, xOrderBy, objParametri_Utenti
        )
        If dt.Rows.Count > 0 Then
            Return dt
        End If

        If isEmpty(dt) Then
            Dim tipologia = GetTipologia(username, objParametri_Utenti)
            If Not String.IsNullOrWhiteSpace(tipologia) Then
                dt = Leggi2(
                    1, tipologia, codiceImpostazione,
                    xFiltroAggiuntivo, xOrderBy, objParametri_Utenti
                )
            End If
        End If

        If isEmpty(dt) Then
            dt = Leggi2(
                2, username, codiceImpostazione,
                xFiltroAggiuntivo, xOrderBy, objParametri_Utenti
            )
        End If

        Return dt
    End Function

    Public Function LeggiValoreImpostazioneScalare(impostazione As Integer, username As String, objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri) As String

        Dim impostazioneRow = LeggiImpostazioneScalare(impostazione, username, objParametri_Utenti)

        Return impostazioneRow.Where(Function(item) Not IsDBNull(item("Impostazione_Valore_1"))) _
                    .Select(Function(item) item("Impostazione_Valore_1").ToString()) _
                    .FirstOrDefault()
    End Function

    ''' <summary>
    ''' Legge l'impostazione specificata restituendo tutti il primo valore disponibile della sua scalarità.
    ''' Esegue una sola lettura comprendente utente, profilo, superuser e default.
    ''' Vedi <seealso cref="LeggiValoreImpostazioneScalare"/>
    ''' </summary>
    Public Function LeggiImpostazioneScalare(impostazione As Integer, username As String, objParametri_Utenti As AgronicaCoreParametri) As IEnumerable(Of DataRow)
        Const nomeRoutine = "AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read.LeggiImpostazioneScalare()"

        Try

            Dim dtGuida = LeggiFlagUtenteEFlagSuperUserDaGuidaImpostazione(impostazione, objParametri_Utenti)

            Dim flagUtente As Boolean = False
            Dim flagSuperUser As Boolean = False

            If dtGuida IsNot Nothing AndAlso dtGuida.Rows.Count > 0 Then
                Dim rowGuida = dtGuida.Rows(0)
                flagUtente = rowGuida("Impostazione_Utente") = 1
                flagSuperUser = rowGuida("Impostazione_SuperUser") = 1
            Else
                Throw New Exception($"Impostazione {impostazione} non trovata nella tabella Guida_Impostazioni")
            End If

            Dim listOfFilters As New List(Of String)

            Dim profile = ""
            If flagUtente Then
                listOfFilters.Add(username)

                profile = GetTipologia(username, objParametri_Utenti)
                listOfFilters.Add(profile)
            End If

            If flagSuperUser Then
                listOfFilters.Add(objParametri_Utenti.SuperUserUsername)
            End If

            Dim userFilter = listOfFilters.
                Where(Function(str) Not String.IsNullOrWhiteSpace(str)).
                Select(Function(str) "'" & str & "'").
                Aggregate(Function(acc, s) acc & ", " & s)

            Dim strSql As New StringBuilder With {.Length = 0}
            Dim dt As DataTable

            strSql.AppendLine(" SELECT  Utenti_Impostazioni.username, Utenti_Impostazioni.Impostazione_Cod, Utenti_Impostazioni.Impostazione_Valore_1 ")
            strSql.AppendLine(" FROM    Utenti_Impostazioni  ")
            strSql.AppendLine(" WHERE   Utenti_Impostazioni.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri_Utenti.PivaSuperUser) & "'   ")
            If impostazione <> 0 Then
                strSql.AppendLine("   AND     Utenti_Impostazioni.Impostazione_Cod = " & Agro_SQL_SaveNum(impostazione) & "   ")
            End If
            strSql.AppendLine("   AND     Utenti_Impostazioni.UserName in (" & Agro_SQL_Save_Clausola_IN(userFilter, True) & ")   ")
            strSql.AppendLine(" UNION ")
            strSql.AppendLine(" SELECT  'DEFAULT' as username, Guida_Impostazioni.Impostazione_Cod, Guida_Impostazioni.Valore_Default as Impostazione_Valore_1 ")
            strSql.AppendLine(" FROM    Guida_Impostazioni  ")
            strSql.AppendLine(" WHERE   Guida_Impostazioni.Validita_Fine >= GETDATE() AND Guida_Impostazioni.Validita_Inizio <= GETDATE() ")
            If impostazione <> 0 Then
                strSql.AppendLine("   AND     Guida_Impostazioni.Impostazione_Cod = " & Agro_SQL_SaveNum(impostazione) & "   ")
            End If

            dt = EseguiQuery_Lettura(objParametri_Utenti, strSql.ToString, nomeRoutine)

            If flagUtente AndAlso dt.Select("UserName = '" & username & "'").Count > 0 Then
                Return dt.Select("UserName = '" & username & "'")
            ElseIf flagUtente AndAlso dt.Select("UserName = '" & profile & "'").Count > 0 Then
                Return dt.Select("UserName = '" & profile & "'")
            ElseIf dt.Select("UserName = '" & objParametri_Utenti.SuperUserUsername & "'").Count > 0 Then
                Return dt.Select("UserName = '" & objParametri_Utenti.SuperUserUsername & "'")
            Else
                Return dt.Select("UserName = 'DEFAULT'")
            End If
        Catch ex As Exception
            Scrivi_LOG(objParametri_Utenti, nomeRoutine, ex.Message)
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
        End Try
    End Function

    ''' <summary>
    ''' Lettura tabella Guida_Impostazione
    ''' </summary>
    ''' <param name="impostazione_cod"></param>
    ''' <param name="objParametri">Può essere obj parametri server o utenti, la tabella Guida_Impostazioni si trova nel metaschema ed è accessibile tramite vista sia dal db utenti sia dal db server</param>
    ''' <returns></returns>
    Private Function LeggiFlagUtenteEFlagSuperUserDaGuidaImpostazione(ByRef impostazione_cod As Integer, ByRef objParametri As AgronicaCoreParametri) As DataTable

        Const nomeRoutine = "AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read.LeggiFlagUtenteEFlagSuperUserDaGuidaImpostazione()"

        Dim stb As New System.Text.StringBuilder
        Dim dtGuida As New DataTable

        If impostazione_cod = 0 Then
            Throw New Exception("Parametro impostazione_cod obbligatorio")
        End If

        Try
            stb.Length = 0

            stb.AppendLine(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; ")

            stb.AppendLine(" SELECT Impostazione_Utente, Impostazione_SuperUser FROM Guida_Impostazioni ")
            stb.AppendLine(" WHERE Impostazione_cod = " & Agro_SQL_SaveNum(impostazione_cod) & " ")

            '--------------------------------------------------------------------------
            dtGuida = EseguiQuery_Lettura(objParametri, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            Scrivi_LOG(objParametri, nomeRoutine, ex.Message)
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
        End Try

        Return dtGuida

    End Function

    Private Function LeggiFlagUtenteEFlagSuperUserDaGuidaImpostazione(impostazioni As IEnumerable(Of Integer), objParametri As AgronicaCoreParametri) As DataTable
        Const nomeRoutine = "AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read.LeggiFlagUtenteEFlagSuperUserDaGuidaImpostazione()"

        Dim stb As New System.Text.StringBuilder
        Dim dtGuida As New DataTable

        Dim filtroCodici As String = Nothing

        If impostazioni IsNot Nothing AndAlso impostazioni.Any() Then
            filtroCodici = "( " & impostazioni.Select(Function(x) x.ToString).Aggregate(Function(acc, str) acc & ", " & str) & " )"
        End If

        Try
            stb.Length = 0

            stb.AppendLine(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; ")

            stb.AppendLine(" SELECT Impostazione_Cod, Impostazione_Utente, Impostazione_SuperUser FROM Guida_Impostazioni ")
            If filtroCodici IsNot Nothing Then
                stb.AppendLine(" WHERE Impostazione_Cod in " & Agro_SQL_Save_Clausola_IN(filtroCodici) & " ")
            End If

            '--------------------------------------------------------------------------
            dtGuida = EseguiQuery_Lettura(objParametri, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            Scrivi_LOG(objParametri, nomeRoutine, ex.Message)
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
        End Try

        Return dtGuida

    End Function


    '###############################################################################################
    Public Function ImpostazioneValore1_from_ImpostazioneCod(ByVal Impostazione_Cod As Integer,
                                                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                             Optional ByVal Username_1Utente_o_2SuperUser As Integer = 1
                                                             ) As String

        Dim dt As DataTable

        dt = Leggi(Impostazione_Cod,
                   Username_1Utente_o_2SuperUser,
                   enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri)

        If Not IsNothing(dt) Then

            If dt.Rows.Count <> 0 Then
                Return dt.Rows(0).Item("Impostazione_Valore_1")
            Else
                Return ""
            End If

        Else
            Return ""
        End If

    End Function

    '###############################################################################################
    Public Sub LeggiOpzioni_RegistriCantina(ByVal Username_1Utente_o_2SuperUser As Integer,
                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                            ByRef Flag_StampaNumeroVasca As Boolean,
                                            ByRef Flag_StampaCapacitaVasca As Boolean,
                                            ByRef OptGestVisualNumVascaRegImbott As Integer,
                                            ByRef Flag_GestioneRegistroVinificazione As Integer,
                                            ByRef Flag_StampaLottoTrasformazione As Boolean)

        Flag_StampaNumeroVasca = False
        Flag_StampaCapacitaVasca = False
        Flag_GestioneRegistroVinificazione = 0
        OptGestVisualNumVascaRegImbott = 0
        Flag_StampaLottoTrasformazione = False

        Dim dt As DataTable
        Dim filtro As String
        Dim i As Integer

        filtro = " ( Impostazione_Cod IN ( " &
                        CStr(enum_Impostazioni_Utenti.SuperUser_StampaCapacitaEffettivaVascaRegCantina) & ", " &
                        CStr(enum_Impostazioni_Utenti.SuperUser_StampaIndentificativoVascaRegCantina) & ", " &
                        CStr(enum_Impostazioni_Utenti.SuperUser_StampaLottoTrasformazioneRegCantina) & ", " &
                        CStr(enum_Impostazioni_Utenti.SuperUser_GestioneVisualNumVascaRegImbott) & ", " &
                        CStr(enum_Impostazioni_Utenti.SUPERUSER_COD_GESTIONE_REG_VINIFICAZIONE) & " " &
                        " ) )"

        dt = Leggi(0,
                   Username_1Utente_o_2SuperUser,
                   enumSelezioneVariabile.Selezione_TabellaCompleta,
                   filtro,
                   "", objParametri)

        If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then
            For i = 0 To dt.Rows.Count - 1

                Select Case dt.Rows(i).Item("Impostazione_Cod")

                    Case enum_Impostazioni_Utenti.SuperUser_StampaCapacitaEffettivaVascaRegCantina
                        If dt.Rows(i).Item("Impostazione_Valore_1") = 1 Then
                            Flag_StampaCapacitaVasca = True
                        End If

                    Case enum_Impostazioni_Utenti.SuperUser_StampaIndentificativoVascaRegCantina
                        If dt.Rows(i).Item("Impostazione_Valore_1") = 1 Then
                            Flag_StampaNumeroVasca = True
                        End If

                    Case enum_Impostazioni_Utenti.SUPERUSER_COD_GESTIONE_REG_VINIFICAZIONE
                        Flag_GestioneRegistroVinificazione = dt.Rows(i).Item("Impostazione_Valore_1")

                    Case enum_Impostazioni_Utenti.SuperUser_GestioneVisualNumVascaRegImbott
                        OptGestVisualNumVascaRegImbott = dt.Rows(i).Item("Impostazione_Valore_1")

                    Case enum_Impostazioni_Utenti.SuperUser_StampaLottoTrasformazioneRegCantina
                        If dt.Rows(i).Item("Impostazione_Valore_1") = 1 Then
                            Flag_StampaLottoTrasformazione = True
                        End If

                End Select

            Next

        End If

    End Sub

    '##############################################################################################
    Public Function FiltroMateriePrime_from_ImpostazioneCod(ByVal Impostazione_Cod As Integer,
                                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                            ) As Integer

        Dim dt As DataTable
        Dim xCod As Integer

        dt = Leggi(Impostazione_Cod,
                   2,
                   enumSelezioneVariabile.Selezione_TabellaCompleta,
                   "",
                   "",
                   objParametri)

        If Not IsNothing(dt) Then

            If dt.Rows.Count <> 0 Then
                xCod = dt.Rows(0).Item("Impostazione_Valore_1")
            Else
                xCod = 0
            End If

        Else
            xCod = 0
        End If

        Return xCod

    End Function

    '##############################################################################################

    Public Function LeggiSezioniImpostazioni(xFiltroAggiuntivo As String, xOrderBy As String, ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri)

        Dim NomeRoutine As String = "AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read.LeggiSezioniImpostazioni()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            StrSQL.Length = 0
            StrSQL.AppendLine(" SELECT ")

            StrSQL.AppendLine(" Guida_Impostazioni.Impostazione_Cod, ")
            StrSQL.AppendLine(" Guida_Impostazioni.Label, ")

            StrSQL.AppendLine(" sez.Label As SezioneDes, ")
            StrSQL.AppendLine(" sez.Sezione_Cod As SezioneCod, ")
            StrSQL.AppendLine(" sez.Espandibile As SezioneEspandibile,  ")
            StrSQL.AppendLine(" sez.Flag_Livello As SezioneFlagLv, ")

            StrSQL.AppendLine(" ISNULL(sottosez.Label,'') as SottoSezioneDes, ")
            StrSQL.AppendLine(" ISNULL(sottosez.Sezione_Cod, 0) As SottoSezioneCod,  ")
            StrSQL.AppendLine(" ISNULL(sottosez.Espandibile, 0) As SottoSezioneEspandibile,  ")
            StrSQL.AppendLine(" ISNULL(sottosez.Flag_Livello, 1) as SottoSezioneFlagLv, ")

            StrSQL.AppendLine(" ISNULL(livello.Label,'') as LivelloDes, ")
            StrSQL.AppendLine(" ISNULL(livello.Sezione_Cod, 0) As LivelloCod, ")
            StrSQL.AppendLine(" ISNULL(livello.Espandibile, 0) As LivelloEspandibile, ")
            StrSQL.AppendLine(" ISNULL(livello.Flag_Livello, 1) as LivelloFlagLv, ")
            StrSQL.AppendLine(" Guida_Impostazioni.Ordine, ")

            StrSQL.AppendLine(" Guida_Impostazioni.Tipo_Campo, ")
            StrSQL.AppendLine(" Guida_Impostazioni.Valore_Default, ")
            StrSQL.AppendLine(" Guida_Impostazioni.Note, ")

            StrSQL.AppendLine(" Guida_Impostazioni.Impostazione_SuperUser, ")
            StrSQL.AppendLine(" Guida_Impostazioni.Impostazione_Utente, ")
            StrSQL.AppendLine(" Guida_Impostazioni.Impostazione_Azienda, ")
            StrSQL.AppendLine(" Guida_Impostazioni.Impostazione_Azienda_Centro, ")
            StrSQL.AppendLine(" Guida_Impostazioni.Impostazione_Azienda_Centro_Specie, ")
            StrSQL.AppendLine(" Guida_Impostazioni.Flag_InApp ")

            StrSQL.AppendLine(" FROM Guida_Impostazioni ")
            StrSQL.AppendLine(" JOIN Guida_Impostazioni_Sezioni sez ON Guida_Impostazioni.Sezione = sez.Sezione_Cod ")
            StrSQL.AppendLine(" LEFT JOIN Guida_Impostazioni_Sezioni sottosez ON Guida_Impostazioni.SottoSezione = sottosez.Sezione_Cod ")
            StrSQL.AppendLine(" LEFT JOIN Guida_Impostazioni_Sezioni livello ON Guida_Impostazioni.Livello = livello.Sezione_Cod ")

            StrSQL.AppendLine(" WHERE 1=1 ")
            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" and " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo))
            End If
            StrSQL.AppendLine(" and Guida_Impostazioni.Validita_Fine >= GETDATE() ")

            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy))
            End If
            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri_Utenti, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Utenti, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT
    End Function

    Public Function LeggiImpostazioniUtentePerControlliBloccanti(ByRef objParametri_Utenti As AgronicaCoreParametri) As ImpostazioniUtentePerControlliBloccanti

        Dim risultato As New ImpostazioniUtentePerControlliBloccanti

        Dim listaImpostazioni As New List(Of Integer) From {
                        CInt(enum_Impostazioni_Utenti.UTENTE_COD_DEFAULT_BLOCCO_SALVA_NOCONFORME),
                        CInt(enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_DOSEMASSIMA_ETICHETTA_SALVATAGGIO),
                        CInt(enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_DOSEMINIMA_ETICHETTA_SALVATAGGIO),
                        CInt(enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_CARENZA_NON_RISPETTATA_SALVATAGGIO),
                        CInt(enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_ACQUAMASSIMA_ETICHETTA_SALVATAGGIO),
                        CInt(enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_ACQUAMINIMA_ETICHETTA_SALVATAGGIO),
                        CInt(enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_NUMERO_MASSIMO_TRATTAMENTI_ETICHETTA_SALVATAGGIO),
                        CInt(enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_INTERVALLOTRATTAMENTI_ETICHETTA_SALVATAGGIO),
                        CInt(enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_AVVERSITANODPI_SALVATAGGIO),
                        CInt(enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_PRODOTTONOAVVERSITA_SALVATAGGIO),
                        CInt(enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_DOSENONDISPONIBILE_SALVATAGGIO),
                        CInt(enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_DOSEUDMNONCONFORME_SALVATAGGIO),
                        CInt(enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_IMPIANTINONCOERENTIDPI_SALVATAGGIO),
                        CInt(enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_DATADPIMAX_SALVATAGGIO),
                        CInt(enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_DATADPIMIN_SALVATAGGIO),
                        CInt(enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_EPOCADPINONCONFORME_SALVATAGGIO),
                        CInt(enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_DOSEDPIDISERBOMAX_SALVATAGGIO),
                        CInt(enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_DOSEDPIDISERBOANNOMAX_SALVATAGGIO),
                        CInt(enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_NUMERO_MASSIMO_TRATTAMENTI_DPI_SALVATAGGIO),
                        CInt(enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_DOSERAMEANNOMAX_SALVATAGGIO),
                        CInt(enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_DOSERAME5ANNIMAX_SALVATAGGIO),
                        CInt(enum_Impostazioni_Utenti.UTENTE_COD_LIVELLO_CHK_DPI),
                        CInt(enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_ACQUAMASSIMA_DPI_SALVATAGGIO),
                        CInt(enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_FINO_FIORITURA_ETICHETTA_SALVATAGGIO),
                        CInt(enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_BUFFERZONE_ETICHETTA_SALVATAGGIO),
                        CInt(enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_DOSEMASSIMA_ANNO_ETICHETTA_SALVATAGGIO),
                        CInt(enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_RACCOLTA_CARENZA_NON_RISPETTATA),
                        CInt(enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_PRODOTTO_RELAZIONE_FORMULATO_SALVATAGGIO),
                        CInt(enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_PRODOTTO_ETA_IMPIANTO_SALVATAGGIO),
                        CInt(enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_MIX_POLVERULENTI_NONPOLVERULENTI_SALVATAGGIO),
                        CInt(enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_SE_SUPERA_LIMITIMAS),
                        CInt(enum_Impostazioni_Utenti.UTENTE_COD_DEFAULT_BLOCCO_SALVA_FERTILIZZAZIONI),
                        CInt(enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_DOSERAMEANNOMAX_FERTI),
                        CInt(enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_DOSERAME7ANNIMAX_FERTI),
                        CInt(enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_PRODOTTO_NON_BIO_REGOLAMENTO_BIO_FERTI),
                        CInt(enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_MASSIMALE_DISTRIBUZIONE_N_DPI_FERTI),
                        CInt(enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_INTERVENTO_NON_CONSENTITO),
                        CInt(enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_PRODOTTO_NON_UTILIZZABILE_X_DATA),
                        CInt(enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_PRODOTTO_NON_REGISTRATO_SU_COLTURA),
                        CInt(enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_AVVERSITA_NON_TRATTABILE_DPI),
                        CInt(enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_PRODOTTO_NON_REGISTRATO_SU_AVVERSITA_DPI),
                        CInt(enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_PRODOTTO_NON_UTILIZZABILE_DPI),
                        CInt(enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_PRODOTTO_NON_BIO_REGOLAMENTO_BIO_FITO)
        }


        Dim impostazioniEValori = LeggiScalare(objParametri_Utenti.UtenteUsername, listaImpostazioni, objParametri_Utenti)

        If impostazioniEValori IsNot Nothing AndAlso impostazioniEValori.Count > 0 Then
            For i = 0 To impostazioniEValori.Count - 1
                Select Case impostazioniEValori(i).Impostazione_Cod
                    Case enum_Impostazioni_Utenti.UTENTE_COD_DEFAULT_BLOCCO_SALVA_NOCONFORME
                        risultato.UTENTE_COD_DEFAULT_BLOCCO_SALVA_NOCONFORME = impostazioniEValori(i).Valore
                    Case enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_DOSEMASSIMA_ETICHETTA_SALVATAGGIO
                        risultato.UTENTE_COD_BLOCCA_DOSEMASSIMA_ETICHETTA_SALVATAGGIO = impostazioniEValori(i).Valore
                    Case enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_DOSEMINIMA_ETICHETTA_SALVATAGGIO
                        risultato.UTENTE_COD_BLOCCA_DOSEMINIMA_ETICHETTA_SALVATAGGIO = impostazioniEValori(i).Valore
                    Case enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_CARENZA_NON_RISPETTATA_SALVATAGGIO
                        risultato.UTENTE_COD_BLOCCA_CARENZA_NON_RISPETTATA_SALVATAGGIO = impostazioniEValori(i).Valore
                    Case enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_ACQUAMASSIMA_ETICHETTA_SALVATAGGIO
                        risultato.UTENTE_COD_BLOCCA_ACQUAMASSIMA_ETICHETTA_SALVATAGGIO = impostazioniEValori(i).Valore
                    Case enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_ACQUAMINIMA_ETICHETTA_SALVATAGGIO
                        risultato.UTENTE_COD_BLOCCA_ACQUAMINIMA_ETICHETTA_SALVATAGGIO = impostazioniEValori(i).Valore
                    Case enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_NUMERO_MASSIMO_TRATTAMENTI_ETICHETTA_SALVATAGGIO
                        risultato.UTENTE_COD_BLOCCA_NUMERO_MASSIMO_TRATTAMENTI_ETICHETTA_SALVATAGGIO = impostazioniEValori(i).Valore
                    Case enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_INTERVALLOTRATTAMENTI_ETICHETTA_SALVATAGGIO
                        risultato.UTENTE_COD_BLOCCA_INTERVALLOTRATTAMENTI_ETICHETTA_SALVATAGGIO = impostazioniEValori(i).Valore
                    Case enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_AVVERSITANODPI_SALVATAGGIO
                        risultato.UTENTE_COD_BLOCCA_AVVERSITANODPI_SALVATAGGIO = impostazioniEValori(i).Valore
                    Case enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_PRODOTTONOAVVERSITA_SALVATAGGIO
                        risultato.UTENTE_COD_BLOCCA_PRODOTTONOAVVERSITA_SALVATAGGIO = impostazioniEValori(i).Valore
                    Case enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_DOSENONDISPONIBILE_SALVATAGGIO
                        risultato.UTENTE_COD_BLOCCA_DOSENONDISPONIBILE_SALVATAGGIO = impostazioniEValori(i).Valore
                    Case enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_DOSEUDMNONCONFORME_SALVATAGGIO
                        risultato.UTENTE_COD_BLOCCA_DOSEUDMNONCONFORME_SALVATAGGIO = impostazioniEValori(i).Valore
                    Case enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_IMPIANTINONCOERENTIDPI_SALVATAGGIO
                        risultato.UTENTE_COD_BLOCCA_IMPIANTINONCOERENTIDPI_SALVATAGGIO = impostazioniEValori(i).Valore
                    Case enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_DATADPIMAX_SALVATAGGIO
                        risultato.UTENTE_COD_BLOCCA_DATADPIMAX_SALVATAGGIO = impostazioniEValori(i).Valore
                    Case enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_DATADPIMIN_SALVATAGGIO
                        risultato.UTENTE_COD_BLOCCA_DATADPIMIN_SALVATAGGIO = impostazioniEValori(i).Valore
                    Case enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_EPOCADPINONCONFORME_SALVATAGGIO
                        risultato.UTENTE_COD_BLOCCA_EPOCADPINONCONFORME_SALVATAGGIO = impostazioniEValori(i).Valore
                    Case enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_DOSEDPIDISERBOMAX_SALVATAGGIO
                        risultato.UTENTE_COD_BLOCCA_DOSEDPIDISERBOMAX_SALVATAGGIO = impostazioniEValori(i).Valore
                    Case enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_DOSEDPIDISERBOANNOMAX_SALVATAGGIO
                        risultato.UTENTE_COD_BLOCCA_DOSEDPIDISERBOANNOMAX_SALVATAGGIO = impostazioniEValori(i).Valore
                    Case enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_NUMERO_MASSIMO_TRATTAMENTI_DPI_SALVATAGGIO
                        risultato.UTENTE_COD_BLOCCA_NUMERO_MASSIMO_TRATTAMENTI_DPI_SALVATAGGIO = impostazioniEValori(i).Valore
                    Case enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_DOSERAMEANNOMAX_SALVATAGGIO
                        risultato.UTENTE_COD_BLOCCA_DOSERAMEANNOMAX_SALVATAGGIO = impostazioniEValori(i).Valore
                    Case enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_DOSERAME5ANNIMAX_SALVATAGGIO
                        risultato.UTENTE_COD_BLOCCA_DOSERAME5ANNIMAX_SALVATAGGIO = impostazioniEValori(i).Valore
                    Case enum_Impostazioni_Utenti.UTENTE_COD_LIVELLO_CHK_DPI
                        risultato.UTENTE_COD_LIVELLO_CHK_DPI = impostazioniEValori(i).Valore
                    Case enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_ACQUAMASSIMA_DPI_SALVATAGGIO
                        risultato.UTENTE_COD_BLOCCA_ACQUAMASSIMA_DPI_SALVATAGGIO = impostazioniEValori(i).Valore
                    Case enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_FINO_FIORITURA_ETICHETTA_SALVATAGGIO
                        risultato.UTENTE_COD_BLOCCA_FINO_FIORITURA_ETICHETTA_SALVATAGGIO = impostazioniEValori(i).Valore
                    Case enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_BUFFERZONE_ETICHETTA_SALVATAGGIO
                        risultato.UTENTE_COD_BLOCCA_BUFFERZONE_ETICHETTA_SALVATAGGIO = impostazioniEValori(i).Valore
                    Case enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_DOSEMASSIMA_ANNO_ETICHETTA_SALVATAGGIO
                        risultato.UTENTE_COD_BLOCCA_DOSEMASSIMA_ANNO_ETICHETTA_SALVATAGGIO = impostazioniEValori(i).Valore
                    Case enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_RACCOLTA_CARENZA_NON_RISPETTATA
                        risultato.UTENTE_COD_BLOCCA_RACCOLTA_CARENZA_NON_RISPETTATA = impostazioniEValori(i).Valore
                    Case enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_PRODOTTO_RELAZIONE_FORMULATO_SALVATAGGIO
                        risultato.UTENTE_COD_BLOCCA_PRODOTTO_RELAZIONE_FORMULATO_SALVATAGGIO = impostazioniEValori(i).Valore
                    Case enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_PRODOTTO_ETA_IMPIANTO_SALVATAGGIO
                        risultato.UTENTE_COD_BLOCCA_PRODOTTO_ETA_IMPIANTO_SALVATAGGIO = impostazioniEValori(i).Valore
                    Case enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_MIX_POLVERULENTI_NONPOLVERULENTI_SALVATAGGIO
                        risultato.UTENTE_COD_BLOCCA_MIX_POLVERULENTI_NONPOLVERULENTI_SALVATAGGIO = impostazioniEValori(i).Valore
                    Case enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_SE_SUPERA_LIMITIMAS
                        risultato.UTENTE_COD_BLOCCA_SE_SUPERA_LIMITIMAS = impostazioniEValori(i).Valore
                    Case enum_Impostazioni_Utenti.UTENTE_COD_DEFAULT_BLOCCO_SALVA_FERTILIZZAZIONI
                        risultato.UTENTE_COD_DEFAULT_BLOCCO_SALVA_FERTILIZZAZIONI = impostazioniEValori(i).Valore
                    Case enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_DOSERAMEANNOMAX_FERTI
                        risultato.UTENTE_COD_BLOCCA_DOSERAMEANNOMAX_FERTI = impostazioniEValori(i).Valore
                    Case enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_DOSERAME7ANNIMAX_FERTI
                        risultato.UTENTE_COD_BLOCCA_DOSERAME7ANNIMAX_FERTI = impostazioniEValori(i).Valore
                    Case enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_PRODOTTO_NON_BIO_REGOLAMENTO_BIO_FERTI
                        risultato.UTENTE_COD_BLOCCA_PRODOTTO_NON_BIO_REGOLAMENTO_BIO_FERTI = impostazioniEValori(i).Valore
                    Case enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_MASSIMALE_DISTRIBUZIONE_N_DPI_FERTI
                        risultato.UTENTE_COD_BLOCCA_MASSIMALE_DISTRIBUZIONE_N_DPI_FERTI = impostazioniEValori(i).Valore
                    Case enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_INTERVENTO_NON_CONSENTITO
                        risultato.UTENTE_COD_BLOCCA_INTERVENTO_NON_CONSENTITO = impostazioniEValori(i).Valore
                    Case enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_PRODOTTO_NON_UTILIZZABILE_X_DATA
                        risultato.UTENTE_COD_BLOCCA_PRODOTTO_NON_UTILIZZABILE_X_DATA = impostazioniEValori(i).Valore
                    Case enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_PRODOTTO_NON_REGISTRATO_SU_COLTURA
                        risultato.UTENTE_COD_BLOCCA_PRODOTTO_NON_REGISTRATO_SU_COLTURA = impostazioniEValori(i).Valore
                    Case enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_AVVERSITA_NON_TRATTABILE_DPI
                        risultato.UTENTE_COD_BLOCCA_AVVERSITA_NON_TRATTABILE_DPI = impostazioniEValori(i).Valore
                    Case enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_PRODOTTO_NON_REGISTRATO_SU_AVVERSITA_DPI
                        risultato.UTENTE_COD_BLOCCA_PRODOTTO_NON_REGISTRATO_SU_AVVERSITA_DPI = impostazioniEValori(i).Valore
                    Case enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_PRODOTTO_NON_UTILIZZABILE_DPI
                        risultato.UTENTE_COD_BLOCCA_PRODOTTO_NON_UTILIZZABILE_DPI = impostazioniEValori(i).Valore
                    Case enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_PRODOTTO_NON_BIO_REGOLAMENTO_BIO_FITO
                        risultato.UTENTE_COD_BLOCCA_PRODOTTO_NON_BIO_REGOLAMENTO_BIO_FITO = impostazioniEValori(i).Valore
                End Select
            Next
        End If

        Return risultato
    End Function

    ''' <summary>
    ''' Legge la tabella guida_impostazioni, eventualmente mettendola in join con guida_impostazioni_valori.
    ''' </summary>
    Public Function LeggiDatiImpostazione(ByVal Impostazione_Cod As Integer,
                                                ByVal xFiltroAggiuntivo As String,
                                                ByVal tipoSelezione As enumSelezioneVariabile,
                                                ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read.LeggiDatiImpostazione()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            StrSQL.Length = 0

            StrSQL.Append("SELECT ")
            Select Case tipoSelezione
                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi
                    StrSQL.Append(" V.Impostazione_Cod, V.Codice, V.Descrizione, I.Tipo_Campo, I.Note, I.Valore_Default ")
                Case enumSelezioneVariabile.Selezione_TabellaCompleta
                    StrSQL.Append(" * ")
            End Select

            StrSQL.AppendLine("FROM Guida_Impostazioni_Valori V ")
            StrSQL.AppendLine("JOIN Guida_Impostazioni I ON V.Impostazione_Cod = I.Impostazione_Cod ")
            StrSQL.AppendLine("WHERE 1=1 ")

            If Impostazione_Cod <> 0 Then
                StrSQL.AppendLine("AND V.Impostazione_Cod = " & Agro_SQL_SaveNum(Impostazione_Cod) & "   ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri_Utenti))
            End If

            StrSQL.AppendLine(" AND I.Validita_Fine >= GETDATE() AND V.Validita_Fine >= GETDATE() ")

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri_Utenti, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Utenti, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT
    End Function

    ''' <summary>
    ''' Legge la tabella guida_impostazioni, eventualmente mettendola in join con guida_impostazioni_valori.
    ''' </summary>
    Public Function LeggiDatiImpostazione(ByVal Impostazione_Cod As IEnumerable(Of Integer),
                                                ByVal xFiltroAggiuntivo As String,
                                                ByVal tipoSelezione As enumSelezioneVariabile,
                                                ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read.LeggiDatiImpostazione()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New StringBuilder With {.Length = 0}
        Dim DT As DataTable

        Try
            StrSQL.AppendLine("SELECT ")
            Select Case tipoSelezione
                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi
                    StrSQL.AppendLine(" V.Impostazione_Cod, V.Codice, V.Descrizione, I.Tipo_Campo, I.Note, I.Valore_Default ")
                Case enumSelezioneVariabile.Selezione_TabellaCompleta
                    StrSQL.AppendLine(" * ")
            End Select

            StrSQL.AppendLine("FROM Guida_Impostazioni_Valori V ")
            StrSQL.AppendLine("JOIN Guida_Impostazioni I ON V.Impostazione_Cod = I.Impostazione_Cod ")
            StrSQL.AppendLine("WHERE 1=1 ")

            If Impostazione_Cod.Any Then
                Dim list = Impostazione_Cod.Select(Function(n) n.ToString).
                    Aggregate(Function(acc, n) acc & ", " & n)
                StrSQL.AppendLine("AND V.Impostazione_Cod in ( " & Agro_SQL_Save_Clausola_IN(list) & " )  ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri_Utenti))
            End If
            StrSQL.AppendLine(" AND I.Validita_Fine >= GETDATE() AND V.Validita_Fine >= GETDATE() ")

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri_Utenti, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------
        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Utenti, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT
    End Function

    '##############################################################################################

    Public Function CaricaAreeGIAS(obj_Server As AgronicaCoreDataProvider.AgronicaCoreParametri)

        Dim areeGIAS As New List(Of Object)

        areeGIAS.Add(New With {.descrizione = "Banchedati_DPI", .codice = enum_AreaGIAS.Banchedati_DPI})
        areeGIAS.Add(New With {.descrizione = "Biologico", .codice = enum_AreaGIAS.Biologico})
        areeGIAS.Add(New With {.descrizione = "Cantine", .codice = enum_AreaGIAS.Cantine})
        areeGIAS.Add(New With {.descrizione = "CheckList", .codice = enum_AreaGIAS.CheckList})
        areeGIAS.Add(New With {.descrizione = "Conferimento", .codice = enum_AreaGIAS.Conferimento})
        areeGIAS.Add(New With {.descrizione = "Contabilità", .codice = enum_AreaGIAS.Contabilita})
        areeGIAS.Add(New With {.descrizione = "ControlloDiGestione", .codice = enum_AreaGIAS.ControlloDiGestione})
        areeGIAS.Add(New With {.descrizione = "Documentale", .codice = enum_AreaGIAS.Documentale})
        areeGIAS.Add(New With {.descrizione = "DSS", .codice = enum_AreaGIAS.DSS})
        areeGIAS.Add(New With {.descrizione = "FatturazioneElettronica", .codice = enum_AreaGIAS.FatturazioneElettronica})
        areeGIAS.Add(New With {.descrizione = "FF", .codice = enum_AreaGIAS.FF})
        areeGIAS.Add(New With {.descrizione = "ImportazioneFascicoli", .codice = enum_AreaGIAS.ImportazioneFascicoli})
        areeGIAS.Add(New With {.descrizione = "GiasAPP", .codice = enum_AreaGIAS.GiasAPP})
        areeGIAS.Add(New With {.descrizione = "GiasToGias", .codice = enum_AreaGIAS.GiasToGias})
        areeGIAS.Add(New With {.descrizione = "GIS", .codice = enum_AreaGIAS.GIS})
        areeGIAS.Add(New With {.descrizione = "IntegrazioneconSistemiEsterni", .codice = enum_AreaGIAS.IntegrazioneconSistemiEsterni})
        areeGIAS.Add(New With {.descrizione = "IrriFrame", .codice = enum_AreaGIAS.IrriFrame})
        areeGIAS.Add(New With {.descrizione = "Magazzino", .codice = enum_AreaGIAS.Magazzino})
        areeGIAS.Add(New With {.descrizione = "ManagerFramework", .codice = enum_AreaGIAS.ManagerFramework})
        areeGIAS.Add(New With {.descrizione = "ManagerCliente", .codice = enum_AreaGIAS.ManagerCliente})
        areeGIAS.Add(New With {.descrizione = "NonConformità", .codice = enum_AreaGIAS.NonConformita})
        areeGIAS.Add(New With {.descrizione = "PathFinder", .codice = enum_AreaGIAS.PathFinder})
        areeGIAS.Add(New With {.descrizione = "PianiDiCampionamento", .codice = enum_AreaGIAS.PianiDiCampionamento})
        areeGIAS.Add(New With {.descrizione = "PianiDiConcimazione", .codice = enum_AreaGIAS.PianiDiConcimazione})
        areeGIAS.Add(New With {.descrizione = "PianiSemina", .codice = enum_AreaGIAS.PianiSemina})
        areeGIAS.Add(New With {.descrizione = "Planning", .codice = enum_AreaGIAS.Planning})
        areeGIAS.Add(New With {.descrizione = "Pratiche", .codice = enum_AreaGIAS.Pratiche})
        areeGIAS.Add(New With {.descrizione = "PUA", .codice = enum_AreaGIAS.PUA})
        areeGIAS.Add(New With {.descrizione = "QdC", .codice = enum_AreaGIAS.QdC})
        areeGIAS.Add(New With {.descrizione = "Qualita", .codice = enum_AreaGIAS.Qualita})
        areeGIAS.Add(New With {.descrizione = "Sian", .codice = enum_AreaGIAS.Sian})
        areeGIAS.Add(New With {.descrizione = "Statistiche", .codice = enum_AreaGIAS.Statistiche})
        areeGIAS.Add(New With {.descrizione = "Tracciabilita", .codice = enum_AreaGIAS.Tracciabilita})
        areeGIAS.Add(New With {.descrizione = "Utenti", .codice = enum_AreaGIAS.Utenti})
        areeGIAS.Add(New With {.descrizione = "Vinificazione", .codice = enum_AreaGIAS.Vinificazione})
        areeGIAS.Add(New With {.descrizione = "Visite", .codice = enum_AreaGIAS.Visite})
        areeGIAS.Add(New With {.descrizione = "Zoo", .codice = enum_AreaGIAS.Zoo})
        areeGIAS.Add(New With {.descrizione = "ALTRE", .codice = enum_AreaGIAS.ALTRE})

        Return areeGIAS
    End Function

    Public Function LeggiTuttoScalare(objParametriUtenti As AgronicaCoreDataProvider.AgronicaCoreParametri) As IEnumerable(Of Utente_Impostazioni)
        Return LeggiScalareInternal(objParametriUtenti.UtenteUsername, Nothing, objParametriUtenti)
    End Function

    Public Function LeggiScalare(username As String, impostazioni As IEnumerable(Of Integer), objParametriUtenti As AgronicaCoreDataProvider.AgronicaCoreParametri) As IEnumerable(Of Utente_Impostazioni)
        If impostazioni Is Nothing OrElse (Not impostazioni.Any()) Then
            Return New List(Of Utente_Impostazioni)
        End If

        Return LeggiScalareInternal(username, impostazioni, objParametriUtenti)

    End Function

    Private Function ReadGuideFlags(impostazioni As IEnumerable(Of Integer), objParametriUtenti As AgronicaCoreParametri) As Dictionary(Of Integer, (isUser As Boolean, isSuperUser As Boolean))
        Dim dtGuida = LeggiFlagUtenteEFlagSuperUserDaGuidaImpostazione(impostazioni, objParametriUtenti)
        Dim dtGuidaValori = New Dictionary(Of Integer, (isUser As Boolean, isSuperUser As Boolean))

        For Each rowDtGuida In dtGuida.Rows
            Dim impostazione_cod = CInt(rowDtGuida("Impostazione_Cod"))
            Dim flagUser = rowDtGuida("Impostazione_Utente") = 1
            Dim flagSuperuser = rowDtGuida("Impostazione_SuperUser") = 1

            dtGuidaValori.Add(impostazione_cod, (isUser:=flagUser, isSuperUser:=flagSuperuser))
        Next

        Return dtGuidaValori
    End Function

    Private function GetSimplifiedOrigin(row As DataRow, profileCode As string, username As string) As String
        Dim isSearchingProfile = Regex.IsMatch(username, "^[0-9]+$", RegexOptions.None, TimeSpan.FromSeconds(3)) AndAlso (profileCode = "" orelse profileCode = "0")
        Dim matchUsername = row("username").ToString.ToLower = username.ToLower

        If (isSearchingProfile AndAlso matchUsername) OrElse (Not isSearchingProfile AndAlso row("username") = profileCode) Then
            Return "PROFILO"
        Elseif Not isSearchingProfile AndAlso matchUsername Then
            Return "UTENTE"
        Else 
            Return row("username")
        End If
    End Function

    ''' <summary>
    ''' Legge le impostazioni utente indicate tenendo conto della scalarità delle impostazioni in base al tipo di utente.
    ''' </summary>
    ''' <remarks>
    ''' Per gli utenti normali segue la scalarità: <tt>utente > profilo > default</tt>.
    ''' Per il superuser segue la scalarità: <tt>superuser > default</tt>.
    ''' </remarks>
    ''' <history>
    ''' (01/12/2025) - Rimuove il filtro sui valori per consentire il caricamento dei defaulte, anche con valore
    ''' nullo (""). Questa modifica corregge il caricamento dei valori nella pagina angular dedicata alle impostazioni
    ''' e consente una lettura scalare completa (comprensiva di default) per le ipostazioni.
    ''' Se in futuro fosse necessario riaggiungere un filtro per evitare i default, si consiglia di aggiungere un nuovo
    ''' parametro alla funzione o creare una funzione separata.
    ''' </history>
    Private Function LeggiScalareInternal(
        username As String, impostazioni As IEnumerable(Of Integer), objParametriUtenti As AgronicaCoreParametri
    ) As IEnumerable(Of Utente_Impostazioni)
        Dim nomeRoutine As String = "AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read.LeggiScalareInternal()"
        Dim strSql As New StringBuilder With {.Length = 0}

        Dim dtGuidaValori = ReadGuideFlags(impostazioni, objParametriUtenti)
        ' Checks per vedere se l'impostazione è utente o superuser
        Dim hasUserSettingGuide = Function(row) dtGuidaValori.ContainsKey(row("Impostazione_Cod")) AndAlso dtGuidaValori(row("Impostazione_Cod")).isUser
        Dim hasSuperuserSettingGuide = Function(row) dtGuidaValori.ContainsKey(row("Impostazione_Cod")) AndAlso (dtGuidaValori(row("Impostazione_Cod")).isSuperUser)

        Dim tipologia = GetTipologiaCod(username, objParametriUtenti).ToString
        Dim filtroUtenti = "( " & {username, tipologia, objParametriUtenti.SuperUserUsername}.
            Select(Function(str) "'" & str & "'").
            Aggregate(Function(acc, str) acc & ", " & str) & " )"

        Dim filtroCodici As String = Nothing
        If impostazioni IsNot Nothing AndAlso impostazioni.Any() Then
            filtroCodici = "( " & impostazioni.Select(Function(x) x.ToString).Aggregate(Function(acc, str) acc & ", " & str) & " )"
        End If

        Try
            strSql.AppendLine(" SELECT Utenti_Impostazioni.UserName, Utenti_Impostazioni.Impostazione_Cod, Utenti_Impostazioni.Impostazione_Valore_1 ")
            strSql.AppendLine(" FROM Utenti_Impostazioni ")
            strSql.AppendLine(" WHERE Utenti_Impostazioni.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametriUtenti.PivaSuperUser) & "' ")
            strSql.AppendLine("       AND Utenti_Impostazioni.Username in " & Agro_SQL_Save_Clausola_IN(filtroUtenti, True) & " ")

            If filtroCodici IsNot Nothing Then
                strSql.AppendLine("       AND Utenti_Impostazioni.Impostazione_Cod in " & Agro_SQL_Save_Clausola_IN(filtroCodici) & " ")
            End If

            strSql.AppendLine(" UNION ")
            strSql.AppendLine(" SELECT 'DEFAULT' AS UserName, Guida_Impostazioni.Impostazione_Cod, Guida_Impostazioni.Valore_Default as Impostazione_Valore_1 ")
            strSql.AppendLine(" FROM Guida_Impostazioni ")
            strSql.AppendLine(" WHERE Guida_Impostazioni.Validita_Inizio <= getdate() AND Guida_Impostazioni.Validita_Fine >= getdate() ")

            If filtroCodici IsNot Nothing Then
                strSql.AppendLine("       AND Guida_Impostazioni.Impostazione_Cod in " & Agro_SQL_Save_Clausola_IN(filtroCodici) & " ")
            End If

            Dim dt = EseguiQuery_Lettura(objParametriUtenti, strSql.ToString, nomeRoutine)
            Dim allValues = dt.Select.
                GroupBy(Function(r) r("Impostazione_Cod")).
                Select(Function(gr)
                           If gr.FirstOrDefault(Function(row) CompareStringInvariant(row("username"), username) AndAlso hasUserSettingGuide(row)) IsNot Nothing Then
                               Return gr.FirstOrDefault(Function(row) CompareStringInvariant(row("username"), username) AndAlso dtGuidaValori(row("Impostazione_Cod")).isUser)

                           ElseIf gr.FirstOrDefault(Function(row) CompareStringInvariant(row("username"), tipologia) AndAlso hasUserSettingGuide(row)) IsNot Nothing Then
                               Return gr.FirstOrDefault(Function(row) CompareStringInvariant(row("username"), tipologia) AndAlso dtGuidaValori(row("Impostazione_Cod")).isUser)

                           ElseIf gr.FirstOrDefault(Function(row) CompareStringInvariant(row("username"), objParametriUtenti.SuperUserUsername) AndAlso hasSuperuserSettingGuide(row)) IsNot Nothing Then
                               Return gr.FirstOrDefault(Function(row) CompareStringInvariant(row("username"), objParametriUtenti.SuperUserUsername) AndAlso dtGuidaValori(row("Impostazione_Cod")).isSuperUser)

                           Else
                               Return gr.FirstOrDefault(Function(row) row("username") = "DEFAULT")
                           End If
                       End Function).
                Where(Function(found) found IsNot Nothing).ToList()
            'Where(Function(rw) rw("username") <> "DEFAULT" OrElse (rw("username") = "DEFAULT" AndAlso rw("Impostazione_Valore_1") <> "")).
            Dim values = allValues.
                Select(Function(row) New Utente_Impostazioni With {
                    .Impostazione_Cod = row("Impostazione_Cod"),
                    .Valore = row("Impostazione_Valore_1"),
                    .Username = getSimplifiedOrigin(row, tipologia, username)
                }).ToList()
            Return values
        Catch ex As Exception
            Scrivi_LOG(objParametriUtenti, nomeRoutine, ex.Message)
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
        End Try
    End Function

    ''' <summary>
    ''' Compara due stringhe senza contare come differenza il fatto che una lettera sia maiuscola o meno.
    ''' </summary>
    Private Function CompareStringInvariant(str1 As String, str2 As String) As Boolean
        Return str1.ToLower.Equals(str2.ToLower)
    End Function

    Private Function GetTipologiaCod(username As String, objPUtenti As AgronicaCoreDataProvider.AgronicaCoreParametri) As Integer
        Dim objUtentiDal As New Utenti_Read
        Return objUtentiDal.Leggi2(username, String.Empty, String.Empty, objPUtenti).
            Select.Select(Function(row) If(IsDBNull(row("Tipologia_Cod")), 0, CInt(row("Tipologia_Cod")))).
            DefaultIfEmpty(0).
            First
    End Function

End Class


'################################################
'################################################
Public Class Utenti_Impostazioni_W
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function Cancella(ByVal Impostazione_Cod As Integer,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                             ) As Boolean

        Dim nomeRoutine As String = "AnagrafeCoreUtentiDAL.Utenti_Impostazioni_W.Cancella()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then

                strSql.Length = 0
                strSql.AppendLine(" UPDATE Utenti_Impostazioni ")
                strSql.AppendLine(" SET ")
                strSql.AppendLine("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                strSql.AppendLine("      ,Inviato = -1 ")
                strSql.AppendLine(" WHERE    (Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "')   ")
                If objParametri.UsernameOperazione <> "" Then
                    strSql.AppendLine(" AND     (Username = '" & Agro_SQL_SaveText(objParametri.UtenteUsername) & "')   ")
                End If

                If Impostazione_Cod <> 0 Then
                    strSql.AppendLine(" AND     (Impostazione_Cod = " & Agro_SQL_SaveNum(Impostazione_Cod) & ")   ")
                End If

            Else

                strSql.Length = 0

                strSql.AppendLine(" DELETE  ")
                strSql.AppendLine(" FROM Utenti_Impostazioni ")
                strSql.AppendLine(" WHERE    (Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "')   ")
                If objParametri.UsernameOperazione <> "" Then
                    strSql.AppendLine(" AND     (Username = '" & Agro_SQL_SaveText(objParametri.UtenteUsername) & "')   ")
                End If

                If Impostazione_Cod <> 0 Then
                    strSql.AppendLine(" AND     (Impostazione_Cod = " & Agro_SQL_SaveNum(Impostazione_Cod) & ")   ")
                End If


            End If
            '---------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try


        Return xRisp

    End Function


    '##############################################################################################
    'a differenza della 1, passa la username
    Public Function Cancella2(ByVal UsernameUtente As String,
                              ByVal Impostazione_Cod As Integer,
                              ByVal xFiltroAggiuntivo As String,
                              ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                              ) As Boolean

        Dim nomeRoutine As String = "AnagrafeCoreUtentiDAL.Utenti_Impostazioni_W.Cancella2()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then
                strSql.Length = 0
                strSql.AppendLine(" UPDATE Utenti_Impostazioni ")
                strSql.AppendLine(" SET ")
                strSql.AppendLine("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                strSql.AppendLine("      ,Inviato = -1 ")
            Else
                strSql.Length = 0
                strSql.AppendLine(" DELETE  ")
                strSql.AppendLine(" FROM Utenti_Impostazioni ")
            End If

            strSql.AppendLine(" WHERE    (Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "')   ")

            strSql.AppendLine(" AND     (Username = '" & Agro_SQL_SaveText(UsernameUtente) & "')   ")

            If Impostazione_Cod <> 0 Then
                strSql.AppendLine(" AND     (Impostazione_Cod = " & Agro_SQL_SaveNum(Impostazione_Cod) & ")   ")
            End If

            '---------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function

    ''' <summary>
    ''' Deletes all the settings for the specified users.
    ''' </summary>
    ''' <param name="utenti">Uers whom settings will be deleted.</param>
    ''' <remarks>
    ''' Currently used during the deletion of a profile.
    ''' </remarks>
    Public function CancellaPerUtente(
        utenti As IEnumerable(Of String),
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
        optional xFiltroAggiuntivo As String = "",
        optional formatUsernameString As Boolean = true
    ) 
        Dim nomeRoutine As String = "AnagrafeCoreUtentiDAL.Utenti_Impostazioni_W.CancellaPerUtente()"
        Dim strSql As New StringBuilder With {.Length = 0}

        If (utenti Is Nothing OrElse Not utenti.Any)
            Throw New ArgumentNullException("The users' list is null or empty!")
        Elseif formatUsernameString
            utenti = utenti.select(Function(username) if(username.StartsWith("'"), username, "'" & username)).
                select(Function(username) if(username.EndsWith("'"), username, username &"'"))
        End If

        Try
            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then

                strSql.AppendLine(" UPDATE Utenti_Impostazioni ")
                strSql.AppendLine(" SET ")
                strSql.AppendLine("   Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "', ")
                strSql.AppendLine("   Inviato = -1 ")
            Else
                strSql.AppendLine(" DELETE FROM Utenti_Impostazioni ")
            End If

            strSql.AppendLine(" WHERE username IN (" & Agro_SQL_Save_Clausola_IN(String.Join(",", utenti), true, false) & ") ")
            strSql.AppendLine("   AND (Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' OR Piva_SuperUser = '')   ")
            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine("   AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            Return EseguiQuery_Scrittura(objParametri, strSql.ToString, nomeRoutine)
        Catch ex As Exception
            Scrivi_LOG(objParametri, nomeRoutine, ex.Message)
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
        End Try
    End function

    Public function CancellaMassivo(
        impostazioni As IEnumerable(Of Integer), 
        utenti As IEnumerable(Of String),
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
        optional xFiltroAggiuntivo As String = ""
    ) 
        Dim nomeRoutine As String = "AnagrafeCoreUtentiDAL.Utenti_Impostazioni_W.CancellaMassivo()"
        Dim strSql As New StringBuilder With {.Length = 0}

        If (impostazioni Is Nothing OrElse Not impostazioni.Any)
            Throw New ArgumentNullException("The settings' list is null or empty!")
        End If
        If (utenti Is Nothing OrElse Not utenti.Any)
            Throw New ArgumentNullException("The users' list is null or empty!")
        End If

        Try
            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then

                strSql.AppendLine(" UPDATE Utenti_Impostazioni ")
                strSql.AppendLine(" SET ")
                strSql.AppendLine("   Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "', ")
                strSql.AppendLine("   Inviato = -1 ")
            Else
                strSql.AppendLine(" DELETE FROM Utenti_Impostazioni ")
            End If

            strSql.AppendLine(" WHERE  (Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "')   ")
            strSql.AppendLine("   AND  Impostazione_Cod IN (" & Agro_SQL_Save_Clausola_IN(String.Join(",", impostazioni.Select(Function(x) x.ToString)), False, false) & ") ")
            strSql.AppendLine("   AND  username IN (" & Agro_SQL_Save_Clausola_IN(String.Join(",", utenti), true, false) & ") ")
            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine("   AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            Return EseguiQuery_Scrittura(objParametri, strSql.ToString, nomeRoutine)
        Catch ex As Exception
            Scrivi_LOG(objParametri, nomeRoutine, ex.Message)
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
        End Try
    End function

    '##############################################################################################
    Public Function Scrivi(ByVal Impostazione_Cod As Integer,
                           ByVal Impostazione_Valore_1 As String,
                           ByVal Impostazione_Valore_2 As String,
                           ByVal Impostazione_Valore_3 As String,
                           ByVal Impostazione_Valore_4 As String,
                           ByVal Validita_Inizio As Date,
                           ByVal Validita_Fine As Date,
                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                           Optional ByVal salvaDefault As Boolean = False
                           ) As Boolean

        Dim nomeRoutine As String = "AnagrafeCoreUtentiDAL.Utenti_Impostazioni_W.Scrivi()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            strSql.Length = 0

            strSql.AppendLine(" INSERT INTO Utenti_Impostazioni ")
            strSql.AppendLine(" (  Piva_SuperUser, UserName, Impostazione_Cod, Impostazione_Valore_1, Impostazione_Valore_2, Impostazione_Valore_3, Impostazione_Valore_4, ")
            strSql.AppendLine("  inviato, datainvio, Data_Creazione, Data_Modifica, Username_Creazione, Username_Modifica, Validita_Inizio, Validita_Fine ) ")

            strSql.AppendLine(" VALUES ('" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "', ")

            If salvaDefault Then
                strSql.AppendLine("'" & Agro_SQL_SaveText(Trim(objParametri.SuperUserUsername)) & "', ")
            Else
                strSql.AppendLine("'" & Agro_SQL_SaveText(Trim(objParametri.UtenteUsername)) & "', ")
            End If

            strSql.AppendLine("" & Agro_SQL_SaveNum(Trim(Impostazione_Cod)) & ", ")
            strSql.AppendLine("'" & Agro_SQL_SaveText(Trim(Impostazione_Valore_1)) & "', ")
            strSql.AppendLine("'" & Agro_SQL_SaveText(Trim(Impostazione_Valore_2)) & "', ")
            strSql.AppendLine("'" & Agro_SQL_SaveText(Trim(Impostazione_Valore_3)) & "', ")
            strSql.AppendLine("'" & Agro_SQL_SaveText(Trim(Impostazione_Valore_4)) & "', ")
            strSql.AppendLine(" 0 , ")
            strSql.AppendLine(" NULL, ")
            strSql.AppendLine(" GETDATE() , ")
            strSql.AppendLine(" GETDATE() , ")

            If salvaDefault Then
                strSql.AppendLine("'" & Agro_SQL_SaveText(objParametri.SuperUserUsername) & "', ")
                strSql.AppendLine("'" & Agro_SQL_SaveText(Trim(objParametri.SuperUserUsername)) & "', ")
            Else
                strSql.AppendLine("'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "', ")
                strSql.AppendLine("'" & Agro_SQL_SaveText(Trim(objParametri.UsernameOperazione)) & "', ")
            End If

            strSql.AppendLine(Agro_SQL_SaveDate(Validita_Inizio) & ", ")
            strSql.AppendLine(Agro_SQL_SaveDate(Validita_Fine) & " ")
            strSql.AppendLine(" ) ")
            '---------------------------------------------


            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function

    Public Function Scrivi2(
                           ByVal UtenteUsername As String,
                           ByVal Impostazione_Cod As Integer,
                           ByVal Impostazione_Valore_1 As String,
                           ByVal Impostazione_Valore_2 As String,
                           ByVal Impostazione_Valore_3 As String,
                           ByVal Impostazione_Valore_4 As String,
                           ByVal Validita_Inizio As Date,
                           ByVal Validita_Fine As Date,
                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                           ) As Boolean

        Dim nomeRoutine As String = "AnagrafeCoreUtentiDAL.Utenti_Impostazioni_W.Scrivi()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            strSql.Length = 0

            strSql.AppendLine(" INSERT INTO Utenti_Impostazioni ")
            strSql.AppendLine(" (  Piva_SuperUser, UserName, Impostazione_Cod, Impostazione_Valore_1, Impostazione_Valore_2, Impostazione_Valore_3, Impostazione_Valore_4, ")
            strSql.AppendLine("  inviato, datainvio, Data_Creazione, Data_Modifica, Username_Creazione, Username_Modifica, Validita_Inizio, Validita_Fine ) ")

            strSql.AppendLine(" VALUES ('" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "', ")
            strSql.AppendLine("'" & Agro_SQL_SaveText(Trim(UtenteUsername)) & "', ")
            strSql.AppendLine("" & Agro_SQL_SaveNum(Trim(Impostazione_Cod)) & ", ")
            strSql.AppendLine("'" & Agro_SQL_SaveText(Trim(Impostazione_Valore_1)) & "', ")
            strSql.AppendLine("'" & Agro_SQL_SaveText(Trim(Impostazione_Valore_2)) & "', ")
            strSql.AppendLine("'" & Agro_SQL_SaveText(Trim(Impostazione_Valore_3)) & "', ")
            strSql.AppendLine("'" & Agro_SQL_SaveText(Trim(Impostazione_Valore_4)) & "', ")
            strSql.AppendLine(" 0 , ")
            strSql.AppendLine(" NULL, ")
            strSql.AppendLine(" GETDATE() , ")
            strSql.AppendLine(" GETDATE() , ")
            strSql.AppendLine("'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "', ")
            strSql.AppendLine("'" & Agro_SQL_SaveText(Trim(objParametri.UsernameOperazione)) & "', ")
            strSql.AppendLine(Agro_SQL_SaveDate(Validita_Inizio) & ", ")
            strSql.AppendLine(Agro_SQL_SaveDate(Validita_Fine) & " ")
            strSql.AppendLine(" ) ")
            '---------------------------------------------


            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function

    '##############################################################################################
    ''' <summary>
    ''' Scrive i permessi di uno o più utenti copiandoli da quelli posseduti dalla sua tipologia.
    ''' </summary>
    ''' <param name="username">Username o lista di username degli utenti su cui eseguire l'operazione. Formato per la lista di username: <tt>'user1', 'user2', 'user3', ...</tt> </param>
    ''' <param name="xFiltroAggiuntivo">Filtro useguibile sulla tabella utenti</param>
    Public Function ScriviDaTipologia(username As String, xFiltroAggiuntivo As String, objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, Optional gestisciInTransazione As Boolean = True)
        Dim NomeRoutine As String = "AnagrafeCoreUtentiDAL.Utenti_Impostazioni_W.ScriviDaTipologia()"
        Dim StrSQL As New StringBuilder With {.Length = 0}
        Dim xRisp As Boolean

        If Not String.IsNullOrWhiteSpace(username) Then
            username = username.Trim
            If Not username.StartsWith("'") OrElse username.Contains(")") Then
                username = Agro_SQL_SaveText(username)
            End If
        Else
            Throw New ArgumentNullException("Nessun username specificato")
        End If
        Try
            If gestisciInTransazione Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessione(gestisciInTransazione, objParametri)
            End If

            StrSQL.AppendLine(" ;WITH Utenti_Da_Processare AS ( ")
            StrSQL.AppendLine(" SELECT UserName FROM Utenti WHERE UserName IN ")
            StrSQL.AppendLine("   ( ")
            StrSQL.AppendLine(username)
            StrSQL.AppendLine("   ) ")
            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine("   AND " & Agro_SQL_SaveText(xFiltroAggiuntivo))
            End If
            StrSQL.AppendLine(" ) ")
            StrSQL.AppendLine(" SELECT * INTO #utenti FROM Utenti_Da_Processare ")
            StrSQL.AppendLine(" SELECT Impostazione_Cod INTO #toIgnore FROM guida_impostazioni WHERE sezione = 0 ")
            StrSQL.AppendLine(" ")

            StrSQL.AppendLine(" -- UTETNI_IMPOSTAZIONI ")
            StrSQL.AppendLine(" SELECT UserName, Impostazione_Cod, Impostazione_Valore_1, Impostazione_Valore_2, Impostazione_Valore_3, Impostazione_Valore_4, Piva_SuperUser ")
            StrSQL.AppendLine(" INTO #Impostazioni ")
            StrSQL.AppendLine(" FROM Utenti_Impostazioni WHERE username LIKE ( ")
            StrSQL.AppendLine("   SELECT TOP(1) Tipologia_Cod from Utenti WHERE UserName in ( SELECT UserName from #utenti ) ")
            StrSQL.AppendLine(" ) ")
            StrSQL.AppendLine(" ")
            StrSQL.AppendLine(" IF EXISTS (SELECT TOP(1) * FROM #Impostazioni) ")
            StrSQL.AppendLine(" BEGIN ")
            StrSQL.AppendLine("   -- CREO RECORD ")
            StrSQL.AppendLine("   SELECT ")
            StrSQL.AppendLine("     Utenti.UserName, #Impostazioni.Impostazione_Cod, ")
            StrSQL.AppendLine("     #Impostazioni.Impostazione_Valore_1, #Impostazioni.Impostazione_Valore_2, #Impostazioni.Impostazione_Valore_3, #Impostazioni.Impostazione_Valore_4, ")
            StrSQL.AppendLine("     #Impostazioni.Piva_SuperUser ")
            StrSQL.AppendLine("     into #Utenti_Impostazioni ")
            StrSQL.AppendLine("   FROM Utenti ")
            StrSQL.AppendLine("     LEFT JOIN #Impostazioni ON #Impostazioni.UserName LIKE Utenti.Tipologia_Cod ")
            StrSQL.AppendLine("   WHERE utenti.UserName IN ")
            StrSQL.AppendLine("     (    select UserName from #utenti    ) ")
            StrSQL.AppendLine("   -- CANCELLO I VECCHI RECORD ")
            StrSQL.AppendLine("   DELETE FROM Utenti_Impostazioni WHERE UserName IN ")
            StrSQL.AppendLine("     (    select UserName from #utenti    ) ")
            StrSQL.AppendLine("     AND Impostazione_Cod NOT IN (SELECT Impostazione_Cod FROM #toIgnore) ")
            StrSQL.AppendLine("   -- INSERISCO I NUOVI RECORD ")
            StrSQL.AppendLine("   INSERT INTO Utenti_Impostazioni ")
            StrSQL.AppendLine("     (UserName, Impostazione_Cod, Impostazione_Valore_1, Impostazione_Valore_2, Impostazione_Valore_3, Impostazione_Valore_4, Piva_SuperUser) ")
            StrSQL.AppendLine("   SELECT ")
            StrSQL.AppendLine("     UserName, Impostazione_Cod, Impostazione_Valore_1, Impostazione_Valore_2, Impostazione_Valore_3, Impostazione_Valore_4, Piva_SuperUser ")
            StrSQL.AppendLine("   FROM #Utenti_Impostazioni ")
            StrSQL.AppendLine(" end ")
            StrSQL.AppendLine(" ")

            StrSQL.AppendLine(" -- UTETNI_IMPOSTAZIONI_FILTROMONO ")
            StrSQL.AppendLine(" SELECT UserName, Impostazione_Cod, ID_0, Str_0, Piva_SuperUser ")
            StrSQL.AppendLine(" INTO #Impostazioni_fm ")
            StrSQL.AppendLine(" FROM Utenti_Impostazioni_FiltroMono WHERE username LIKE ( ")
            StrSQL.AppendLine("   SELECT TOP(1) Tipologia_Cod from Utenti WHERE UserName in ( SELECT UserName from #utenti ) ")
            StrSQL.AppendLine(" ) ")
            StrSQL.AppendLine(" ")
            StrSQL.AppendLine(" IF EXISTS (SELECT TOP(1) * FROM #Impostazioni_fm) ")
            StrSQL.AppendLine(" BEGIN ")
            StrSQL.AppendLine("   -- CREO NUOVI RECORD -- FM ")
            StrSQL.AppendLine("   SELECT ")
            StrSQL.AppendLine("     #Impostazioni_fm.Piva_SuperUser, utenti.UserName, ")
            StrSQL.AppendLine("     #Impostazioni_fm.Impostazione_Cod, #Impostazioni_fm.ID_0, #Impostazioni_fm.Str_0 ")
            StrSQL.AppendLine("     into #Utenti_Impostazioni_FM ")
            StrSQL.AppendLine("   FROM Utenti ")
            StrSQL.AppendLine("     LEFT JOIN #Impostazioni_fm ON #Impostazioni_fm.UserName LIKE Utenti.Tipologia_Cod ")
            StrSQL.AppendLine("   WHERE utenti.UserName IN ")
            StrSQL.AppendLine("     (    select UserName from #utenti    ) ")
            StrSQL.AppendLine("   -- CANCELLO I VECCHI RECORD -- FM ")
            StrSQL.AppendLine("   DELETE FROM Utenti_Impostazioni_FiltroMono WHERE UserName IN ")
            StrSQL.AppendLine("     (    select UserName from #utenti    ) ")
            StrSQL.AppendLine("     AND Impostazione_Cod NOT IN (SELECT Impostazione_Cod FROM #toIgnore) ")
            StrSQL.AppendLine("   -- INSERISCO I NUOVI RECORD -- FM ")
            StrSQL.AppendLine("   INSERT INTO Utenti_Impostazioni_FiltroMono ")
            StrSQL.AppendLine("     (Piva_SuperUser, UserName, Impostazione_Cod, ID_0, Str_0) ")
            StrSQL.AppendLine("   SELECT ")
            StrSQL.AppendLine("     Piva_SuperUser, UserName, Impostazione_Cod, ID_0, Str_0 ")
            StrSQL.AppendLine("   FROM #Utenti_Impostazioni_FM ")
            StrSQL.AppendLine(" END ")
            StrSQL.AppendLine(" ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------
            If gestisciInTransazione Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(1, objParametri)
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessione(objParametri)
            End If
        Catch ex As Exception
            If gestisciInTransazione Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri)
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessione(objParametri)
            End If
            Scrivi_LOG(objParametri, NomeRoutine, ex.Message)
            Throw New Exception("[" & NomeRoutine & "] : " & ex.Message)
        End Try
        Return xRisp
    End Function

    ''' <summary>
    ''' Massively writes user settings based on user type (Tipologia). This function copies settings from a template user 
    ''' (identified by Tipologia_Cod) to multiple target users. The operation is performed in a transaction-safe manner 
    ''' and includes both standard settings and filter settings.
    ''' </summary>
    ''' <param name="users">Collection of users (IUtente interface) to whom the settings will be applied</param>
    ''' <param name="xFiltroAggiuntivo">Additional SQL filter conditions to be applied when selecting users</param>
    ''' <param name="objParametri">Core parameters containing database connection and configuration information</param>
    ''' <param name="gestisciInTransazione">Optional flag to manage the operation within a database transaction (default: True)</param>
    ''' <param name="impostazioni">Optional collection of setting codes to be included in the operation. If Nothing, all settings will be copied</param>
    ''' <returns>Boolean indicating whether the operation was successful</returns>
    ''' <remarks>
    ''' The function performs the following main operations:
    ''' 1. Creates a temporary table with users to be processed
    ''' 2. Identifies settings to ignore based on sezione = 0 or the code not in the optional inclusion list (impostazioni)
    ''' 3. Copies standard user settings (Utenti_Impostazioni) from template to target users
    ''' 4. Copies filter settings (Utenti_Impostazioni_FiltroMono) from template to target users
    ''' 5. All operations are performed using temporary tables and proper cleanup (DELETE FROM before writing)
    ''' 
    ''' The function uses parallel processing to optimize the creation of the user filter string and includes 
    ''' proper error handling and transaction management.
    ''' </remarks>
    Public Function ScriviDaTipologiaMassivo(
        users As IEnumerable(Of AgronicaCoreModelsSTD.profilazione.IUtente),
        xFiltroAggiuntivo As String,
        objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
        Optional gestisciInTransazione As Boolean = True,
        Optional impostazioni As IEnumerable(Of Integer) = Nothing
    )
        Dim NomeRoutine As String = "AnagrafeCoreUtentiDAL.Utenti_Impostazioni_W.ScriviDaTipologia()"
        Dim StrSQL As New StringBuilder With {.Length = 0}
        Dim xRisp As Boolean

        If users.Count = 0 Then
            Return True
        End If
        Dim prepareFilter = Function(acc, u)
                                If u.index Mod 100 = 0 Then
                                    acc.name &= ", --" & u.index & vbNewLine & u.name
                                ElseIf u.index Mod 10 = 0 Then
                                    acc.name &= ", " & vbNewLine & u.name
                                Else
                                    acc.name &= ", " & u.name
                                End If
                                Return acc
                            End Function
        Dim usersFilter = users.AsParallel.
            Select(Function(u, i) New With {.name = "'" & u.UserName & "'", .index = i}).
            Aggregate(prepareFilter).name
        Try
            If gestisciInTransazione Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessione(gestisciInTransazione, objParametri)
            End If

            StrSQL.AppendLine(" ;WITH Utenti_Da_Processare AS ( ")
            StrSQL.AppendLine(" SELECT UserName FROM Utenti WHERE UserName IN ")
            StrSQL.AppendLine("   ( " & usersFilter & " ) ")
            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine("   AND " & Agro_SQL_SaveText(xFiltroAggiuntivo))
            End If
            StrSQL.AppendLine(" ) ")
            StrSQL.AppendLine(" SELECT * INTO #utenti FROM Utenti_Da_Processare ")

            ' Definisco le impostazioni DA IGNORARE
            StrSQL.AppendLine(" SELECT Impostazione_Cod INTO #toIgnore FROM guida_impostazioni WHERE sezione = 0 ")
            If impostazioni IsNot Nothing AndAlso impostazioni.Any Then
                StrSQL.AppendLine("   OR Impostazione_Cod NOT IN ( " & String.Join(",", impostazioni) & " ) ")
            End If
            StrSQL.AppendLine(" ")

            StrSQL.AppendLine(" -- UTETNI_IMPOSTAZIONI ")
            StrSQL.AppendLine(" SELECT UserName, Utenti_Impostazioni.Impostazione_Cod, Impostazione_Valore_1, Impostazione_Valore_2, Impostazione_Valore_3, Impostazione_Valore_4, Piva_SuperUser ")
            StrSQL.AppendLine(" INTO #Impostazioni ")
            StrSQL.AppendLine(" FROM Utenti_Impostazioni ")
            StrSQL.AppendLine(" LEFT JOIN #toIgnore ON #toIgnore.Impostazione_Cod = Utenti_Impostazioni.Impostazione_Cod ")
            StrSQL.AppendLine(" WHERE username LIKE ( SELECT TOP(1) Tipologia_Cod from Utenti WHERE UserName in ( SELECT UserName from #utenti ) ) ")
            StrSQL.AppendLine(" AND #toIgnore.Impostazione_Cod IS NULL ")
            StrSQL.AppendLine(" ")
            StrSQL.AppendLine(" IF EXISTS (SELECT TOP(1) * FROM #Impostazioni) ")
            StrSQL.AppendLine(" BEGIN ")
            StrSQL.AppendLine("   -- CREO RECORD ")
            StrSQL.AppendLine("   SELECT ")
            StrSQL.AppendLine("     Utenti.UserName, #Impostazioni.Impostazione_Cod, ")
            StrSQL.AppendLine("     #Impostazioni.Impostazione_Valore_1, #Impostazioni.Impostazione_Valore_2, #Impostazioni.Impostazione_Valore_3, #Impostazioni.Impostazione_Valore_4, ")
            StrSQL.AppendLine("     #Impostazioni.Piva_SuperUser ")
            StrSQL.AppendLine("     into #Utenti_Impostazioni ")
            StrSQL.AppendLine("   FROM Utenti ")
            StrSQL.AppendLine("     LEFT JOIN #Impostazioni ON #Impostazioni.UserName LIKE Utenti.Tipologia_Cod ")
            StrSQL.AppendLine("   WHERE utenti.UserName IN ")
            StrSQL.AppendLine("     (    select UserName from #utenti    ) ")
            StrSQL.AppendLine("   -- CANCELLO I VECCHI RECORD ")
            StrSQL.AppendLine("   DELETE FROM Utenti_Impostazioni WHERE UserName IN ")
            StrSQL.AppendLine("     (    select UserName from #utenti    ) ")
            StrSQL.AppendLine("     AND Impostazione_Cod NOT IN (SELECT Impostazione_Cod FROM #toIgnore) ")
            StrSQL.AppendLine("   -- INSERISCO I NUOVI RECORD ")
            StrSQL.AppendLine("   INSERT INTO Utenti_Impostazioni ")
            StrSQL.AppendLine("     (UserName, Impostazione_Cod, Impostazione_Valore_1, Impostazione_Valore_2, Impostazione_Valore_3, Impostazione_Valore_4, Piva_SuperUser) ")
            StrSQL.AppendLine("   SELECT ")
            StrSQL.AppendLine("     UserName, Impostazione_Cod, Impostazione_Valore_1, Impostazione_Valore_2, Impostazione_Valore_3, Impostazione_Valore_4, Piva_SuperUser ")
            StrSQL.AppendLine("   FROM #Utenti_Impostazioni ")
            StrSQL.AppendLine(" end ")
            StrSQL.AppendLine(" ")

            StrSQL.AppendLine(" -- UTETNI_IMPOSTAZIONI_FILTROMONO ")
            StrSQL.AppendLine(" SELECT UserName, Utenti_Impostazioni_FiltroMono.Impostazione_Cod, ID_0, Str_0, Piva_SuperUser ")
            StrSQL.AppendLine(" INTO #Impostazioni_fm ")
            StrSQL.AppendLine(" FROM Utenti_Impostazioni_FiltroMono ")
            StrSQL.AppendLine(" LEFT JOIN #toIgnore ON #toIgnore.Impostazione_Cod = Utenti_Impostazioni_FiltroMono.Impostazione_Cod ")
            StrSQL.AppendLine(" WHERE username LIKE ( SELECT TOP(1) Tipologia_Cod from Utenti WHERE UserName in ( SELECT UserName from #utenti ) ) ")
            StrSQL.AppendLine(" AND #toIgnore.Impostazione_Cod IS NULL ")
            StrSQL.AppendLine(" ")
            StrSQL.AppendLine(" IF EXISTS (SELECT TOP(1) * FROM #Impostazioni_fm) ")
            StrSQL.AppendLine(" BEGIN ")
            StrSQL.AppendLine("   -- CREO NUOVI RECORD -- FM ")
            StrSQL.AppendLine("   SELECT ")
            StrSQL.AppendLine("     #Impostazioni_fm.Piva_SuperUser, utenti.UserName, ")
            StrSQL.AppendLine("     #Impostazioni_fm.Impostazione_Cod, #Impostazioni_fm.ID_0, #Impostazioni_fm.Str_0 ")
            StrSQL.AppendLine("     into #Utenti_Impostazioni_FM ")
            StrSQL.AppendLine("   FROM Utenti ")
            StrSQL.AppendLine("     LEFT JOIN #Impostazioni_fm ON #Impostazioni_fm.UserName LIKE Utenti.Tipologia_Cod ")
            StrSQL.AppendLine("   WHERE utenti.UserName IN ")
            StrSQL.AppendLine("     (    select UserName from #utenti    ) ")
            StrSQL.AppendLine("   -- CANCELLO I VECCHI RECORD -- FM ")
            StrSQL.AppendLine("   DELETE FROM Utenti_Impostazioni_FiltroMono WHERE UserName IN ")
            StrSQL.AppendLine("     (    select UserName from #utenti    ) ")
            StrSQL.AppendLine("     AND Impostazione_Cod NOT IN (SELECT Impostazione_Cod FROM #toIgnore) ")
            StrSQL.AppendLine("   -- INSERISCO I NUOVI RECORD -- FM ")
            StrSQL.AppendLine("   INSERT INTO Utenti_Impostazioni_FiltroMono ")
            StrSQL.AppendLine("     (Piva_SuperUser, UserName, Impostazione_Cod, ID_0, Str_0) ")
            StrSQL.AppendLine("   SELECT ")
            StrSQL.AppendLine("     Piva_SuperUser, UserName, Impostazione_Cod, ID_0, Str_0 ")
            StrSQL.AppendLine("   FROM #Utenti_Impostazioni_FM ")
            StrSQL.AppendLine(" END ")
            StrSQL.AppendLine(" ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------
            If gestisciInTransazione Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(1, objParametri)
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessione(objParametri)
            End If
        Catch ex As Exception
            If gestisciInTransazione Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri)
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessione(objParametri)
            End If
            Scrivi_LOG(objParametri, NomeRoutine, ex.Message)
            Throw New Exception("[" & NomeRoutine & "] : " & ex.Message)
        End Try
        Return xRisp
    End Function

    '##############################################################################################
    Public Function Modifica2(
                             ByVal UtenteUsername As String,
                             ByVal Impostazione_Cod As Integer,
                             ByVal Impostazione_Valore_1 As String,
                             ByVal Impostazione_Valore_2 As String,
                             ByVal Impostazione_Valore_3 As String,
                             ByVal Impostazione_Valore_4 As String,
                             ByVal Validita_Inizio As Date,
                             ByVal Validita_Fine As Date,
                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                             ) As Boolean

        Dim nomeRoutine As String = "AnagrafeCoreUtentiDAL.Utenti_Impostazioni_W.Scrivi()"

        '====================================================================================
        'Parametri opzionali :
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            strSql.Length = 0



            strSql.AppendLine(" UPDATE Utenti_Impostazioni SET ")

            strSql.AppendLine("Impostazione_Valore_1 = '" & Agro_SQL_SaveText(Trim(Impostazione_Valore_1)) & "', ")
            strSql.AppendLine("Impostazione_Valore_2 = '" & Agro_SQL_SaveText(Trim(Impostazione_Valore_2)) & "', ")
            strSql.AppendLine("Impostazione_Valore_3 = '" & Agro_SQL_SaveText(Trim(Impostazione_Valore_3)) & "', ")
            strSql.AppendLine("Impostazione_Valore_4 = '" & Agro_SQL_SaveText(Trim(Impostazione_Valore_4)) & "', ")
            strSql.AppendLine("Validita_Inizio =" & Agro_SQL_SaveDate(Validita_Inizio) & ", ")
            strSql.AppendLine("Validita_Fine =" & Agro_SQL_SaveDate(Validita_Fine) & ", ")

            strSql.AppendLine("Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UtenteUsername) & "', ")

            strSql.AppendLine("Data_Modifica = GETDATE() ")

            strSql.AppendLine(" WHERE     (Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "')   ")

            If objParametri.UtenteUsername <> "" Then
                strSql.AppendLine(" AND     (Username = '" & Agro_SQL_SaveText(UtenteUsername) & "')   ")
            End If

            If Impostazione_Cod <> 0 Then
                strSql.AppendLine(" AND     (Impostazione_Cod = " & Agro_SQL_SaveNum(Impostazione_Cod) & ")   ")
            End If


            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function

    '##############################################################################################
    Public Function Modifica(ByVal Impostazione_Cod As Integer,
                             ByVal Impostazione_Valore_1 As String,
                             ByVal Impostazione_Valore_2 As String,
                             ByVal Impostazione_Valore_3 As String,
                             ByVal Impostazione_Valore_4 As String,
                             ByVal Validita_Inizio As Date,
                             ByVal Validita_Fine As Date,
                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                             Optional ByVal salvaDefault As Boolean = False
                             ) As Boolean

        Dim nomeRoutine As String = "AnagrafeCoreUtentiDAL.Utenti_Impostazioni_W.Scrivi()"

        '====================================================================================
        'Parametri opzionali :
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            strSql.Length = 0



            strSql.AppendLine(" UPDATE Utenti_Impostazioni SET ")

            strSql.AppendLine("Impostazione_Valore_1 = '" & Agro_SQL_SaveText(Trim(Impostazione_Valore_1)) & "', ")
            strSql.AppendLine("Impostazione_Valore_2 = '" & Agro_SQL_SaveText(Trim(Impostazione_Valore_2)) & "', ")
            strSql.AppendLine("Impostazione_Valore_3 = '" & Agro_SQL_SaveText(Trim(Impostazione_Valore_3)) & "', ")
            strSql.AppendLine("Impostazione_Valore_4 = '" & Agro_SQL_SaveText(Trim(Impostazione_Valore_4)) & "', ")
            strSql.AppendLine("Validita_Inizio =" & Agro_SQL_SaveDate(Validita_Inizio) & ", ")
            strSql.AppendLine("Validita_Fine =" & Agro_SQL_SaveDate(Validita_Fine) & ", ")

            If salvaDefault Then
                strSql.AppendLine("Username_Modifica = '" & Agro_SQL_SaveText(objParametri.SuperUserUsername) & "', ")
            Else
                strSql.AppendLine("Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UtenteUsername) & "', ")
            End If

            strSql.AppendLine("Data_Modifica = GETDATE() ")

            strSql.AppendLine(" WHERE     (Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "')   ")

            If salvaDefault Then
                strSql.AppendLine(" AND     (Username = '" & Agro_SQL_SaveText(objParametri.SuperUserUsername) & "')   ")
            Else
                strSql.AppendLine(" AND     (Username = '" & Agro_SQL_SaveText(objParametri.UtenteUsername) & "')   ")
            End If

            If Impostazione_Cod <> 0 Then
                strSql.AppendLine(" AND     (Impostazione_Cod = " & Agro_SQL_SaveNum(Impostazione_Cod) & ")   ")
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function

    ' ribalta le impostazioni di un profilo in un utente
    '##############################################################################################
    Public Function ApplicaProfilo(ByVal TipologiaCod As String,
                                   ByVal Username As String,
                                   ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                   ) As Boolean

        Dim nomeRoutine As String = "AnagrafeCoreUtentiDAL.Utenti_Impostazioni_W.Scrivi()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            strSql.Length = 0

            strSql.AppendLine(" INSERT INTO Utenti_Impostazioni ")
            strSql.AppendLine(" (  Piva_SuperUser, UserName, Impostazione_Cod, Impostazione_Valore_1, Impostazione_Valore_2, Impostazione_Valore_3, Impostazione_Valore_4, ")
            strSql.AppendLine("  inviato, datainvio, Data_Creazione, Data_Modifica, Username_Creazione, Username_Modifica, Validita_Inizio, Validita_Fine ) ")

            strSql.AppendLine(" SELECT Piva_SuperUser, '" & Agro_SQL_SaveText(Username) & "' , Impostazione_Cod, Impostazione_Valore_1, Impostazione_Valore_2, Impostazione_Valore_3, Impostazione_Valore_4, ")
            strSql.AppendLine(" inviato, datainvio, GETDATE(), GETDATE(), Username_Creazione, Username_Modifica, Validita_Inizio, Validita_Fine ")
            strSql.AppendLine(" FROM Utenti_Impostazioni ")
            strSql.AppendLine(" WHERE UserName= '" & Agro_SQL_SaveText(TipologiaCod) & "' ")


            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, strSql.ToString, nomeRoutine)
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