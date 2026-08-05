Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.TipiEnumerativi

Public Class Analisi_Parametri_R
    Inherits AgronicaCoreDataProvider.DataProvider
    
    '##############################################################################################
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' da utilizzare con Selezione_JoinCompleta 
    ''' </summary>
    ''' <param name="Analisi_Parametro_Cod">Analisi_Parametro_Cod di default = 0</param>
    ''' <param name="xSelezioneVariabile"></param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="xOrderBy"></param>
    ''' <param name="objParametri"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[garavini]	04/01/2011	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Function Leggi(ByVal Analisi_Parametro_Cod As Integer,
                          ByVal xSelezioneVariabile As enumSelezioneVariabile,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreParametri
                          ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Analisi_Parametri_Read.Leggi()"
        
        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try
            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi


                Case enumSelezioneVariabile.Selezione_TabellaCompleta


                Case enumSelezioneVariabile.Selezione_JoinDescrizioni


                Case enumSelezioneVariabile.Selezione_JoinCompleta

                    StrSQL.AppendLine(" SELECT Analisi_Parametri.*, ISNULL(UnitaMisura.UDM_SIM,'') AS Analisi_Parametro_Udm_Sim, ISNULL(UnitaMisura.UDM_DES,'') AS Analisi_Parametro_Udm_Des ")
                    StrSQL.AppendLine(" FROM   Analisi_Parametri ")
                    StrSQL.AppendLine(" LEFT OUTER JOIN UnitaMisura ON Analisi_Parametri.Analisi_Parametro_UdM = UnitaMisura.UDM_COD ")
                    StrSQL.AppendLine(" WHERE  Analisi_Parametri.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.AppendLine(" AND    Analisi_Parametri.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    If Analisi_Parametro_Cod <> 0 Then
                        StrSQL.AppendLine(" AND Analisi_Parametri.Analisi_Parametro_Cod = " & Analisi_Parametro_Cod & " ")
                    End If

                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.AppendLine(" AND   Analisi_Parametri.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.AppendLine(" AND   Analisi_Parametri.Inviato =-1 ")
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

    '##############################################################################################
    Public Function Leggi_con_iesimo(ByVal TipologiaAnalisi As Integer,
                                     ByVal Opzionali As Boolean,
                                     ByVal Obbligatori As Boolean,
                                     ByRef objParametri As AgronicaCoreParametri
                                     ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Analisi_Parametri_Read.Leggi()"
        
        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try

            StrSQL.AppendLine(" SELECT Analisi_Parametri.*, ISNULL(UnitaMisura.UDM_SIM,'') AS Analisi_Parametro_Udm_Sim, ISNULL(UnitaMisura.UDM_DES,'') AS Analisi_Parametro_Udm_Des ")
            StrSQL.AppendLine(" FROM   Analisi_Parametri ")
            StrSQL.AppendLine(" LEFT OUTER JOIN UnitaMisura ON Analisi_Parametri.Analisi_Parametro_UdM = UnitaMisura.UDM_COD ")
            StrSQL.AppendLine(" WHERE  Analisi_Parametri.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.AppendLine(" AND    Analisi_Parametri.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
            
            If Opzionali AndAlso Obbligatori Then
                StrSQL.AppendLine(" AND SUBSTRING (analisi_parametro_tipoanalisi, " & TipologiaAnalisi & ",1)<>0  ")
            Else
                If Opzionali Then
                    StrSQL.AppendLine(" AND SUBSTRING (analisi_parametro_tipoanalisi, " & TipologiaAnalisi & ",1)= 1  ")
                Else
                    StrSQL.AppendLine(" AND SUBSTRING (analisi_parametro_tipoanalisi, " & TipologiaAnalisi & ",1)= 2  ")
                End If
            End If
            
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.AppendLine(" AND   Analisi_Parametri.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.AppendLine(" AND   Analisi_Parametri.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            StrSQL.AppendLine(" ORDER BY Analisi_Parametro_Des ASC ")

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

    Public Function LeggiConTipologia(ByVal Analisi_Parametro_Cod As Integer,
                                      ByVal Analisi_Tipologia_Cod As Integer,
                                      ByVal xFiltroAggiuntivo As String,
                                      ByVal xOrderBy As String,
                                      ByRef objParametri As AgronicaCoreParametri
                                      ) As DataTable
        
        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Analisi_Parametri_R.LeggiConTipologia()"
        
        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try

            StrSQL.AppendLine(" SELECT Analisi_Parametri.*, ISNULL(UnitaMisura.UDM_SIM,'') AS Analisi_Parametro_Udm_Sim, ISNULL(UnitaMisura.UDM_DES,'') AS Analisi_Parametro_Udm_Des, Analisi_Tipologia_Dettagli.Ordinamento, Analisi_Tipologia_Dettagli.Analisi_Tipologia_Cod ")
            StrSQL.AppendLine(" FROM   Analisi_Parametri ")
            StrSQL.AppendLine(" LEFT OUTER JOIN UnitaMisura ON Analisi_Parametri.Analisi_Parametro_UdM = UnitaMisura.UDM_COD ")
            StrSQL.AppendLine(" INNER JOIN Analisi_Tipologia_Dettagli ON Analisi_Parametri.Analisi_Parametro_Cod = Analisi_Tipologia_Dettagli.Analisi_Parametro_Cod ")
            StrSQL.AppendLine(" WHERE  Analisi_Parametri.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.AppendLine(" AND    Analisi_Parametri.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
            StrSQL.AppendLine(" AND    Analisi_Tipologia_Dettagli.PivaSuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'")

            If Analisi_Parametro_Cod <> 0 Then
                StrSQL.AppendLine(" AND Analisi_Parametri.Analisi_Parametro_Cod = " & Analisi_Parametro_Cod & " ")
            End If

            If Analisi_Tipologia_Cod <> 0 Then
                StrSQL.AppendLine(" AND Analisi_Tipologia_Dettagli.Analisi_Tipologia_Cod = " & Agro_SQL_SaveNum(Analisi_Tipologia_Cod) & " ")
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.AppendLine(" AND   Analisi_Parametri.Inviato >=0 ")
                    StrSQL.AppendLine(" AND   Analisi_Tipologia_Dettagli.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.AppendLine(" AND   Analisi_Parametri.Inviato =-1 ")
                    StrSQL.AppendLine(" AND   Analisi_Tipologia_Dettagli.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine(" ORDER BY Analisi_Tipologia_Dettagli.Ordinamento")
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
    
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="Analisi_Parametro_Cod"></param>
    ''' <param name="Analisi_Parametro_Des"></param>
    ''' <param name="Analisi_Parametro_Simbolo"></param>
    ''' <param name="Analisi_Parametro_Udm"></param>
    ''' <param name="Analisi_Parametro_Udm_Sim"></param>
    ''' <param name="Analisi_Parametro_Udm_Des"></param>
    ''' <param name="Analisi_Parametro_ValoreMin"></param>
    ''' <param name="Analisi_Parametro_ValoreMax"></param>
    ''' <param name="objParametri"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[pierantoni]	04/01/2011	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Sub ANALISI_Parametri_Info(ByVal Analisi_Parametro_Cod As Integer,
                                      ByRef Analisi_Parametro_Des As String,
                                      ByRef Analisi_Parametro_Simbolo As String,
                                      ByRef Analisi_Parametro_Udm As Integer,
                                      ByRef Analisi_Parametro_Udm_Sim As String,
                                      ByRef Analisi_Parametro_Udm_Des As String,
                                      ByRef Analisi_Parametro_ValoreMin As Decimal,
                                      ByRef Analisi_Parametro_ValoreMax As Decimal,
                                      ByRef objParametri As AgronicaCoreParametri)
        
        Dim dt As DataTable

        'Leggo le imprese associate al profilo selezionato
        dt = Leggi(CInt(Analisi_Parametro_Cod),
                   enumSelezioneVariabile.Selezione_JoinCompleta,
                   "", "", objParametri)

        If dt IsNot Nothing Then

            'Inserisco i record trovati
            If dt.Rows.Count > 0 Then

                Analisi_Parametro_Des = dt.Rows(0).Item("Analisi_Parametro_Des")
                Analisi_Parametro_Simbolo = dt.Rows(0).Item("Analisi_Parametro_Simbolo")
                Analisi_Parametro_Udm = dt.Rows(0).Item("Analisi_Parametro_Udm")
                Analisi_Parametro_Udm_Sim = dt.Rows(0).Item("Analisi_Parametro_Udm_Sim")
                Analisi_Parametro_Udm_Des = dt.Rows(0).Item("Analisi_Parametro_Udm_Des")
                Analisi_Parametro_ValoreMin = dt.Rows(0).Item("Analisi_Parametro_ValoreMin")
                Analisi_Parametro_ValoreMax = dt.Rows(0).Item("Analisi_Parametro_ValoreMax")

            Else

                Analisi_Parametro_Des = ""
                Analisi_Parametro_Simbolo = ""
                Analisi_Parametro_Udm = -1
                Analisi_Parametro_Udm_Sim = ""
                Analisi_Parametro_Udm_Des = ""
                Analisi_Parametro_ValoreMin = 0
                Analisi_Parametro_ValoreMax = 0

            End If

            dt.Dispose()

        Else

            Analisi_Parametro_Des = ""
            Analisi_Parametro_Simbolo = ""
            Analisi_Parametro_Udm = -1
            Analisi_Parametro_Udm_Sim = ""
            Analisi_Parametro_Udm_Des = ""
            Analisi_Parametro_ValoreMin = 0
            Analisi_Parametro_ValoreMax = 0

        End If

        dt = Nothing

    End Sub

    Public Function ParametroDes_from_ParametroCod(ByVal Analisi_Parametro_Cod As Integer,
                                                   ByRef Analisi_Parametro_UdM As Integer,
                                                   ByRef Analisi_Parametro_Simbolo As String,
                                                   ByRef objParametri As AgronicaCoreParametri,
                                                   Optional Analisi_Parametro_ValoreMin As Decimal = 0,
                                                   Optional Analisi_Parametro_ValoreMax As Decimal = 0
                                                   ) As String

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.Analisi_Parametri_R.ParametroDes_from_ParametroCod()"

        Dim dt As DataTable

        dt = Leggi(Analisi_Parametro_Cod, enumSelezioneVariabile.Selezione_JoinCompleta,
                   "", "", objParametri)

        If dt.Rows.Count > 0 Then
            Analisi_Parametro_UdM = dt.Rows(0).Item("Analisi_Parametro_UdM")
            Analisi_Parametro_Simbolo = dt.Rows(0).Item("Analisi_Parametro_Simbolo")

            Analisi_Parametro_ValoreMin = dt.Rows(0).Item("Analisi_Parametro_ValoreMin")
            Analisi_Parametro_ValoreMax = dt.Rows(0).Item("Analisi_Parametro_ValoreMax")

            Return dt.Rows(0).Item("Analisi_Parametro_Des")
        Else
            Return ""
        End If

    End Function

    Public Function Check_ParametroxSchema(Parametro_Cod As Integer,
                                           Analisi_Tipologia_Cod As enum_AnalisiTipologia_Schema,
                                           ByRef objParametri As AgronicaCoreParametri
                                           ) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.Analisi_Parametri_R.Check_ParametroxSchema()"

        Dim dt As DataTable
        dt = LeggiConTipologia(Parametro_Cod, Analisi_Tipologia_Cod, "", "", objParametri)

        'Se trovo il parametro associato allo schema ritorno true	
        If dt.Rows.Count = 1 Then
            Return True
        Else
            Return False
        End If

    End Function

End Class
