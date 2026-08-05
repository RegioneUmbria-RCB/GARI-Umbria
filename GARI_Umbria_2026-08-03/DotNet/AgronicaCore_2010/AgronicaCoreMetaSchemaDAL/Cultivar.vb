Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider

<CachedDataProviderAttribute("Cultivar_R")>
Public Class Cultivar_R
    Inherits AgronicaCoreDataProvider.CachedDataProvider




    '##############################################################################################
    <Cacheable(True)>
    Public Function Leggi(ByVal Cul_Cod As Integer,
                          ByVal Veg_Cod As Integer,
                          ByVal Cerca_CulDes As String,
                          ByVal xSelezioneVariabile As enumSelezioneVariabile,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreParametri
                          ) As DataTable

        Const nomeRoutine = "AgronicaCoreMetaSchemaDAL.Cultivar.Cultivar_Leggi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim dt As DataTable

        Try

            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                    StrSQL.Length = 0

                    StrSQL.Append(" SELECT  Cultivar.Cul_Cod, Cultivar.Cul_Des ")
                    StrSQL.Append(" FROM    Cultivar ")
                    StrSQL.Append(" WHERE   Cultivar.Validita_Inizio <=" & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND     Cultivar.Validita_Fine >=" & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    If Cul_Cod <> 0 Then
                        StrSQL.Append(" AND Cultivar.Cul_Cod = " & Agro_SQL_SaveNum(Cul_Cod) & " ")
                    End If

                    If Veg_Cod <> 0 Then
                        StrSQL.Append(" AND Cultivar.Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod) & " ")
                    End If

                    If Cerca_CulDes <> "" Then
                        StrSQL.Append(" AND Cultivar.Cul_Des LIKE '%" & Agro_SQL_SaveText(Cerca_CulDes) & "%' ")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   Cultivar.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Cultivar.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY Cul_Des ")
                    End If


                Case enumSelezioneVariabile.Selezione_TabellaCompleta
                    StrSQL.Length = 0

                    StrSQL.Append(" SELECT  * ")
                    StrSQL.Append(" FROM    Cultivar ")
                    StrSQL.Append(" WHERE   Cultivar.Validita_Inizio <=" & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND     Cultivar.Validita_Fine >=" & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    If Cul_Cod <> 0 Then
                        StrSQL.Append(" AND Cultivar.Cul_Cod = " & Agro_SQL_SaveNum(Cul_Cod) & " ")
                    End If

                    If Veg_Cod <> 0 Then
                        StrSQL.Append(" AND Cultivar.Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod) & " ")
                    End If

                    If Cerca_CulDes <> "" Then
                        StrSQL.Append(" AND Cultivar.Cul_Des LIKE '%" & Agro_SQL_SaveText(Cerca_CulDes) & "%' ")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   Cultivar.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Cultivar.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY Cul_Des ")
                    End If
                Case enumSelezioneVariabile.Selezione_JoinDescrizioni
                    StrSQL.Length = 0

                    StrSQL.Append(" SELECT  SpecieVegetali.Veg_des, Cultivar.* ")
                    StrSQL.Append(" FROM        Cultivar ")
                    StrSQL.Append(" INNER JOIN    SpecieVegetali ON SpecieVegetali.Veg_Cod = Cultivar.Veg_Cod ")
                    StrSQL.Append(" WHERE   Cultivar.Validita_Inizio <=" & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND     Cultivar.Validita_Fine >=" & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    If Cul_Cod <> 0 Then
                        StrSQL.Append(" AND Cultivar.Cul_Cod = " & Agro_SQL_SaveNum(Cul_Cod) & " ")
                    End If

                    If Veg_Cod <> 0 Then
                        StrSQL.Append(" AND Cultivar.Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod) & " ")
                    End If

                    If Cerca_CulDes <> "" Then
                        StrSQL.Append(" AND Cultivar.Cul_Des LIKE '%" & Agro_SQL_SaveText(Cerca_CulDes) & "%' ")
                    End If



                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   Cultivar.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Cultivar.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY Veg_Des, Cul_Des ")
                    End If

                Case enumSelezioneVariabile.Selezione_JoinCompleta




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

    '##############################################################################################
    Public Function CulDes_from_CulCod(ByVal Cul_Cod As Integer,
                                       ByRef objParametri As AgronicaCoreParametri
                                       ) As String

        Dim dt As DataTable

        dt = Leggi(Cul_Cod, 0, "",
                   enumSelezioneVariabile.Selezione_JoinDescrizioni,
                   "", "", objParametri)

        If Not IsNothing(dt) Then
            If dt.Rows.Count <> 0 Then
                Return dt.Rows(0).Item("Cul_Des")
            Else
                Return ""
            End If
        Else
            Return ""
        End If

    End Function



    '##############################################################################################
    Public Function VegCod_from_CulCod(ByVal Cul_Cod As Integer,
                                       ByRef objParametri As AgronicaCoreParametri
                                       ) As String

        Dim dt As DataTable

        dt = Leggi(Cul_Cod, 0, "",
                   enumSelezioneVariabile.Selezione_JoinDescrizioni,
                   "", "", objParametri)

        If Not IsNothing(dt) Then
            If dt.Rows.Count <> 0 Then
                Return CStr(dt.Rows(0).Item("Veg_Cod"))
            Else
                Return "0"
            End If
        Else
            Return "0"
        End If


    End Function



    '##############################################################################################
    Public Function VegCod_from_IdAgenda(ByVal IdAgenda As Integer,
                                         ByRef Veg_Cod As Integer,
                                         ByRef Cul_Cod() As Integer,
                                         ByRef Appezza() As Integer,
                                         ByRef Id_Reg() As Integer,
                                         ByRef objParametri As AgronicaCoreParametri
                                         ) As String

        Const nomeRoutine = "AgronicaCoreMetaSchemaDAL.Cultivar.VegCod_from_IdAgenda()"
        Dim messaggioErrore As String

        Dim dt As DataTable
        Dim StrSQL As New System.Text.StringBuilder
        Try
            StrSQL.Length = 0
            StrSQL.Append(" SELECT DISTINCT Cultivar.Veg_Cod, Agenda.Lav_Cod, Mov_Destinazioni.ID_Destinazione, Mov_Destinazioni.Appezza, Cultivar.Cul_Cod ")
            StrSQL.Append(" FROM Cultivar INNER JOIN ")
            StrSQL.Append(" Reg_Impianti ON Cultivar.Cul_Cod = Reg_Impianti.CUL_COD RIGHT OUTER JOIN ")
            StrSQL.Append(" Agenda INNER JOIN ")
            StrSQL.Append(" Movimenti ON Agenda.PIVA = Movimenti.PIVA AND Agenda.Sa_Cod = Movimenti.Sa_Cod AND ")
            StrSQL.Append(" Agenda.Id_Agenda = Movimenti.Id_Agenda INNER JOIN ")
            StrSQL.Append(" Movimenti_dettagli ON Movimenti.PIVA = Movimenti_dettagli.PIVA AND Movimenti.Sa_Cod = Movimenti_dettagli.Sa_Cod AND ")
            StrSQL.Append(" Movimenti.Id_Agenda = Movimenti_dettagli.Id_Agenda AND Movimenti.Id_Mov = Movimenti_dettagli.Id_Mov INNER JOIN ")
            StrSQL.Append(" Mov_Destinazioni ON Movimenti_dettagli.PIVA = Mov_Destinazioni.Piva AND ")
            StrSQL.Append(" Movimenti_dettagli.Sa_Cod = Mov_Destinazioni.Sa_Cod AND Movimenti_dettagli.Id_Agenda = Mov_Destinazioni.Id_Agenda AND ")
            StrSQL.Append(" Movimenti_dettagli.Id_Mov = Mov_Destinazioni.Id_Mov ON Reg_Impianti.PIVA = Mov_Destinazioni.Piva AND ")
            StrSQL.Append(" Reg_Impianti.SA_COD = Mov_Destinazioni.Sa_Cod AND Reg_Impianti.APPEZZA = Mov_Destinazioni.Appezza AND ")
            StrSQL.Append(" Reg_Impianti.ID_REG = Mov_Destinazioni.Id_Destinazione ")
            StrSQL.Append(" WHERE Agenda.Id_Agenda = " & IdAgenda)
            'seleziono solo le lavorazioni
            StrSQL.Append(" AND Movimenti.Cau_Mov IN (2050, 2100, 2200, 2300) ")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Dim i As Integer

        If dt.Rows.Count > 0 Then

            For i = 0 To dt.Rows.Count - 1

                Veg_Cod = If(Not IsDBNull(dt.Rows(i).Item("Veg_Cod")), dt.Rows(i).Item("Veg_Cod"), 0)

                ReDim Preserve Cul_Cod(i)
                Cul_Cod(i) = If(Not IsDBNull(dt.Rows(i).Item("Cul_Cod")), dt.Rows(i).Item("Cul_Cod"), 0)

                ReDim Preserve Appezza(i)
                Appezza(i) = If(Not IsDBNull(dt.Rows(i).Item("Appezza")), dt.Rows(i).Item("Appezza"), 0)
                ReDim Preserve Id_Reg(i)
                Id_Reg(i) = If(Not IsDBNull(dt.Rows(i).Item("ID_Destinazione")), dt.Rows(i).Item("ID_Destinazione"), 0)

            Next

        End If

    End Function




    '##############################################################################################
    Public Sub VegCod_VegDes_CulDes_from_CulCod(ByVal Cul_Cod As Integer,
                                                ByRef Veg_Cod As Integer,
                                                ByRef Veg_Des As String,
                                                ByRef Cul_Des As String,
                                                ByRef objParametri As AgronicaCoreParametri)

        Dim nomeRoutine As String = "AgronicaCoreMetaSchemaDAL.Cultivar.VegCod_VegDes_CulDes_from_CulCod()"

        Dim dt As DataTable

        Veg_Cod = 0
        Veg_Des = ""
        Cul_Des = ""

        dt = Leggi(Cul_Cod, 0, "",
                   enumSelezioneVariabile.Selezione_JoinDescrizioni,
                   "", "", objParametri)

        If Not IsNothing(dt) Then
            If dt.Rows.Count <> 0 Then
                Veg_Cod = dt.Rows(0).Item("Veg_Cod")
                Veg_Des = dt.Rows(0).Item("Veg_Des")
                Cul_Des = dt.Rows(0).Item("Cul_Des")
            End If
        End If

    End Sub


    '###############################################################################################
    Public Sub VegDes_CulDes_from_Vegcod_CulCod(ByVal Veg_Cod As Integer,
                                                ByVal Cul_Cod As Integer,
                                                ByRef Veg_Des As String,
                                                ByRef Cul_Des As String,
                                                ByRef objParametri As AgronicaCoreParametri)

        Dim dt As DataTable

        Veg_Des = ""
        Cul_Des = ""

        dt = Leggi(Cul_Cod, Veg_Cod, "",
                   enumSelezioneVariabile.Selezione_JoinDescrizioni,
                   "", "", objParametri)

        If Not IsNothing(dt) Then
            If dt.Rows.Count <> 0 Then
                Veg_Des = dt.Rows(0).Item("Veg_Des")
                Cul_Des = dt.Rows(0).Item("Cul_Des")
            End If
        End If

    End Sub

    '################################################################################
    Public Function CulCod_Altre_from_VegCod(ByVal Veg_Cod As Integer,
                                             ByRef objParametri As AgronicaCoreParametri
                                             ) As Integer

        Dim nomeRoutine As String = "AgronicaCoreMetaSchemaDAL.Cultivar.CulCod_Altre_from_VegCod()"

        Dim dt As DataTable
        Dim culCod As Integer = 0

        dt = Leggi(0, Veg_Cod, "altre",
                   enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                   "", "", objParametri)

        If Not IsNothing(dt) Then
            If dt.Rows.Count <> 0 Then
                culCod = dt.Rows(0).Item("Cul_Cod")
            End If
        End If

        Return culCod

    End Function



    '################################################################################
    'Verifica se l'utente ha un filtro impostato, in tal caso legge le varietà selezionate nel filtro
    'altrimenti legge tutte le varietà in archivio
    '----
    'Attenzione! Questa query è fatta sul database UTENTI, 
    'dall' Aggancio a MetaSchema_11 del 26/05/2009
    'ci sono infatti anche delle viste nel db utenti sul metaschema
    '##############################################################################################
    Public Function GestioneFiltroUtente_Leggi(
                                ByVal Cul_Cod As Integer,
                                ByVal Veg_Cod As Integer,
                                ByVal Cerca_CulDes As String,
                                    ByVal xSelezioneVariabile As enumSelezioneVariabile,
                                    ByVal xFiltroAggiuntivo As String,
                                    ByVal xOrderBy As String,
                                    ByRef objParametri_Utente As AgronicaCoreParametri
                                    ) As DataTable

        Const nomeRoutine = "AgronicaCoreMetaSchemaDAL.Cultivar.GestioneFiltroUtente_Leggi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim dt As DataTable

        Try

            StrSQL.Length = 0
            'se l'utente ha filtrato le varietà, ovvero nella tabella Utenti_Impostazioni_FiltroMono 
            'ci sono dei record per l'impostazione COD_FILTRO_VARIETA
            StrSQL.Append(" IF (  ")
            StrSQL.Append(" SELECT COUNT(*)  ")
            StrSQL.Append(" FROM  Cultivar ")
            StrSQL.Append(" INNER JOIN Utenti_Impostazioni_FiltroMono ")
            StrSQL.Append("               ON Cultivar.Cul_cod = Utenti_Impostazioni_FiltroMono.ID_0 ")
            StrSQL.Append(" WHERE Utenti_Impostazioni_FiltroMono.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri_Utente.PivaSuperUser) & "'  ")
            StrSQL.Append(" AND   Utenti_Impostazioni_FiltroMono.UserName = '" & Agro_SQL_SaveText(objParametri_Utente.UtenteUsername) & "'  ")
            StrSQL.Append(" AND   Utenti_Impostazioni_FiltroMono.Impostazione_Cod = " & Agro_SQL_SaveNum(enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_VARIETA) & " ")
            If Veg_Cod <> 0 Then
                StrSQL.Append("   AND Cultivar.Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod) & " ")
            End If
            StrSQL.Append("   ) > 0 ")

            StrSQL.Append("       SELECT      SpecieVegetali.Veg_des, Cultivar.* ")
            StrSQL.Append("       FROM        Cultivar ")
            StrSQL.Append("       INNER JOIN  SpecieVegetali ON SpecieVegetali.Veg_Cod = Cultivar.Veg_Cod ")
            StrSQL.Append("       INNER JOIN  Utenti_Impostazioni_FiltroMono ")
            StrSQL.Append("                   ON Cultivar.Cul_cod = Utenti_Impostazioni_FiltroMono.ID_0 ")
            StrSQL.Append("       WHERE   Cultivar.Validita_Inizio <=" & Agro_SQL_SaveDate(objParametri_Utente.FinestraTemporaleFine) & " ")
            StrSQL.Append("       AND     Cultivar.Validita_Fine >=" & Agro_SQL_SaveDate(objParametri_Utente.FinestraTemporaleInizio) & " ")
            StrSQL.Append("       AND     Utenti_Impostazioni_FiltroMono.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri_Utente.PivaSuperUser) & "'  ")
            StrSQL.Append("       AND     Utenti_Impostazioni_FiltroMono.UserName = '" & Agro_SQL_SaveText(objParametri_Utente.UtenteUsername) & "'  ")
            StrSQL.Append("       AND     Utenti_Impostazioni_FiltroMono.Impostazione_Cod = " & Agro_SQL_SaveNum(enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_VARIETA) & " ")

            If Cul_Cod <> 0 Then
                StrSQL.Append("   AND Cultivar.Cul_Cod = " & Agro_SQL_SaveNum(Cul_Cod) & " ")
            End If
            If Veg_Cod <> 0 Then
                StrSQL.Append("   AND Cultivar.Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod) & " ")
            End If
            If Cerca_CulDes <> "" Then
                StrSQL.Append("   AND Cultivar.Cul_Des LIKE '%" & Agro_SQL_SaveText(Cerca_CulDes) & "%' ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri_Utente))
            End If

            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri_Utente))
            Else
                StrSQL.Append(" ORDER BY Veg_Des, Cul_Des ")
            End If



            StrSQL.Append("ELSE ")


            StrSQL.Append(" SELECT  SpecieVegetali.Veg_des, Cultivar.* ")
            StrSQL.Append(" FROM        Cultivar ")
            StrSQL.Append(" INNER JOIN    SpecieVegetali ON SpecieVegetali.Veg_Cod = Cultivar.Veg_Cod ")

            StrSQL.Append(" WHERE   Cultivar.Validita_Inizio <=" & Agro_SQL_SaveDate(objParametri_Utente.FinestraTemporaleFine) & " ")
            StrSQL.Append(" AND     Cultivar.Validita_Fine >=" & Agro_SQL_SaveDate(objParametri_Utente.FinestraTemporaleInizio) & " ")

            If Cul_Cod <> 0 Then
                StrSQL.Append(" AND Cultivar.Cul_Cod = " & Agro_SQL_SaveNum(Cul_Cod) & " ")
            End If
            If Veg_Cod <> 0 Then
                StrSQL.Append(" AND Cultivar.Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod) & " ")
            End If
            If Cerca_CulDes <> "" Then
                StrSQL.Append(" AND Cultivar.Cul_Des LIKE '%" & Agro_SQL_SaveText(Cerca_CulDes) & "%' ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri_Utente))
            End If
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri_Utente))
            Else
                StrSQL.Append(" ORDER BY Veg_Des, Cul_Des ")
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri_Utente, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Utente, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function


    Public Function VarietaAltre(ByVal Veg_Cod As Integer,
                                 ByRef objParametri As AgronicaCoreParametri
                                 ) As Integer

        Const nomeRoutine = "AgronicaCoreMetaSchemaDAL.Cultivar.VarietaAltre()"
        Dim messaggioErrore As String = ""
        Dim dt As DataTable
        Dim culCod As Integer = 0

        Try

            dt = Leggi(0, Veg_Cod, "altre",
                       enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                       "", "", objParametri)

            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                culCod = dt.Rows(0).Item("Cul_Cod")
            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            culCod = 0
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        dt = Nothing

        Return culCod

    End Function



    '################################################################################
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="objParametri"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[magnani]	28/04/2011	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Function Numero_Cultivar_xSpecie(ByVal Veg_Cod As Integer, ByRef objParametri As AgronicaCoreParametri) As Integer

        Dim num As Integer = 0
        Dim dt As DataTable

        dt = Leggi(0, Veg_Cod, "",
                   enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                   "", "", objParametri)

        If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
            num = dt.Rows.Count
        End If

        Return num

    End Function

End Class
