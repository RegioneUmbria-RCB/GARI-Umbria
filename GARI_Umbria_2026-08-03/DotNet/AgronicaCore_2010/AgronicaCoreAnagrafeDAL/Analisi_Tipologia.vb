Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.DataProviderExtensions

Public Class Analisi_Tipologia_W
    Inherits AgronicaCoreDataProvider.DataProvider

    '#########################################################################
    Public Function Scrivi(ByVal Analisi_Tipologia_Cod As Integer,
                           ByVal Analisi_Tipologia_Des As String,
                           ByVal Analisi_Tipologia_Des_Long As String,
                           ByVal Analisi_Tipologia_Tipo As Integer,
                           ByVal Numero_Determinazioni As String,
                           ByVal Validita_Inizio As Date,
                           ByVal Validita_Fine As Date,
                           ByRef objParametri As AgronicaCoreParametri,
                           Optional ByVal Data_creazione As DateTime = #2/1/1900#,
                           Optional ByVal Data_modifica As DateTime = #2/1/1900#,
                           Optional ByVal username_creazione As String = "",
                           Optional ByVal username_modifica As String = ""
                           ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Analisi_Tipologia_W.Scrivi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If Data_creazione = #2/1/1900# Then
                Data_creazione = Date.Now
            End If

            If Data_modifica = #2/1/1900# Then
                Data_modifica = Date.Now
            End If

            If username_creazione = "" Then
                username_creazione = objParametri.UsernameOperazione
            End If

            If username_modifica = "" Then
                username_modifica = objParametri.UsernameOperazione
            End If

            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.Append("INSERT INTO Analisi_Tipologia( ")
            StrSQL.Append("            PivaSuperUser, Analisi_Tipologia_Cod,  Analisi_Tipologia_Des, ")
            StrSQL.Append("            Analisi_Tipologia_Des_Long,         Analisi_Tipologia_Tipo,")
            StrSQL.Append("            Numero_Determinazioni,  ")
            StrSQL.Append("            Inviato,  ")
            StrSQL.Append("            Data_Creazione,     Data_Modifica, ")
            StrSQL.Append("            UserName_Creazione, UserName_Modifica, ")
            StrSQL.Append("            Validita_Inizio,    Validita_Fine ")
            StrSQL.Append("            ) ")


            StrSQL.Append("VALUES (")
            StrSQL.Append("          '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Analisi_Tipologia_Cod) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Analisi_Tipologia_Des) & "'  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Analisi_Tipologia_Des_Long) & "' ")
            StrSQL.Append("         ," & Agro_SQL_SaveNum(Analisi_Tipologia_Tipo) & " ")
            StrSQL.Append("         ," & Agro_SQL_SaveNum(Numero_Determinazioni) & " ")
            StrSQL.Append("         , 0  ")

            StrSQL.Append("         , " & Agro_SQL_SaveDate(Data_creazione))
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Data_modifica) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(username_modifica) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")
            StrSQL.Append(")")

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



    '#########################################################################
    Public Function Modifica(ByVal Analisi_Tipologia_Cod As Integer,
                             ByVal Analisi_Tipologia_Des As String,
                             ByVal Analisi_Tipologia_Des_Long As String,
                             ByVal Analisi_Tipologia_Tipo As Integer,
                             ByVal Numero_Determinazioni As String,
                             ByVal Validita_Inizio As Date,
                             ByVal Validita_Fine As Date,
                             ByRef objParametri As AgronicaCoreParametri
                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Analisi_Tipologia_W.Modifica()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            StrSQL.Length = 0

            StrSQL.Append("UPDATE Analisi_Tipologia SET ")
            StrSQL.Append("   Analisi_Tipologia_Des = '" & Agro_SQL_SaveText(Analisi_Tipologia_Des) & "'")
            StrSQL.Append("   ,Analisi_Tipologia_Des_Long = '" & Agro_SQL_SaveText(Analisi_Tipologia_Des_Long) & "'")
            StrSQL.Append("   ,Analisi_Tipologia_Tipo   =  " & Agro_SQL_SaveNum(Analisi_Tipologia_Tipo))
            StrSQL.Append("   ,Numero_Determinazioni   =  '" & Agro_SQL_SaveText(Numero_Determinazioni) & "'")

            StrSQL.Append("   ,Data_Modifica     =  " & Agro_SQL_SaveDate(Date.Now))
            StrSQL.Append("   ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.Append("   ,Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Inizio))
            StrSQL.Append("   ,Validita_Fine     =  " & Agro_SQL_SaveDate(Validita_Fine))
            StrSQL.Append(" WHERE PivaSuperUser = '" & Trim(objParametri.PivaSuperUser) & "'")
            StrSQL.Append(" AND   Analisi_Tipologia_Cod = " & Agro_SQL_SaveNum(Analisi_Tipologia_Cod) & " ")

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

    '#########################################################################
    Public Function Cancella(ByVal Analisi_Tipologia_Cod As Integer,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreParametri
                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Analisi_Tipologia_W.Cancella()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then

                StrSQL.Length = 0

                StrSQL.Append(" UPDATE  Analisi_Tipologia ")
                StrSQL.Append(" SET ")
                StrSQL.Append("          Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("         ,Data_Modifica = " & Agro_SQL_SaveDate(Date.Now) & " ")
                StrSQL.Append("         ,Inviato = -1 ")
                StrSQL.Append(" WHERE PivaSuperUser = '" & Agro_SQL_SaveText(Trim(objParametri.PivaSuperUser)) & "' ")
                StrSQL.Append("   AND    Inviato > 0 ")
            Else

                StrSQL.Length = 0

                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM     Analisi_Tipologia ")
                StrSQL.Append(" WHERE    PivaSuperUser = '" & Agro_SQL_SaveText(Trim(objParametri.PivaSuperUser)) & "' ")
                StrSQL.Append(" AND      Inviato = 0 ")

            End If

            If Analisi_Tipologia_Cod <> 0 Then
                StrSQL.Append(" AND Analisi_Tipologia_Cod = " & Agro_SQL_SaveNum(Analisi_Tipologia_Cod) & " ")
            End If

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

'#######################################################################
'#######################################################################
'#######################################################################

Public Class Analisi_Tipologia_R
    Inherits AgronicaCoreDataProvider.DataProvider


    '##############################################################################################
    Public Function Leggi(ByVal Analisi_Tipologia_Cod As Integer,
                          ByVal Analisi_Tipologia_Tipo As Integer,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreParametri,
                          Optional joinLaboratorio As Boolean = False
                          ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Analisi_Tipologia_R.Leggi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT * ")
            StrSQL.Append(" FROM   Analisi_Tipologia ")

            If joinLaboratorio Then
                StrSQL.Append("LEFT JOIN Analisi_Tipologia_Laboratori ON Analisi_Tipologia_Laboratori.Analisi_Tipologia_Cod = Analisi_Tipologia.Analisi_Tipologia_Cod AND Analisi_Tipologia_Laboratori.PivaSuperUser = Analisi_Tipologia.PivaSuperUser")
            End If

            StrSQL.Append(" WHERE  Analisi_Tipologia.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.Append(" AND    Analisi_Tipologia.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
            StrSQL.Append(" AND    Analisi_Tipologia.PivaSuperUser = '" & Agro_SQL_SaveText(Trim(objParametri.PivaSuperUser)) & "'")

            If Analisi_Tipologia_Cod <> 0 Then
                StrSQL.Append(" AND Analisi_Tipologia.Analisi_Tipologia_Cod = " & Agro_SQL_SaveNum(Analisi_Tipologia_Cod) & " ")
            End If

            If Analisi_Tipologia_Tipo <> 0 Then
                StrSQL.Append(" AND Analisi_Tipologia.Analisi_Tipologia_Tipo = " & Agro_SQL_SaveNum(Analisi_Tipologia_Tipo) & " ")
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   Analisi_Tipologia.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   Analisi_Tipologia.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
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

    Public Function Leggi2(ByVal Analisi_Tipologia_Cod As Integer,
                          ByVal Analisi_Tipologia_Tipo As Integer,
                          ByVal Cod_Risum As Integer,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreParametri
                          ) As DataTable
        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Analisi_Tipologia_R.Leggi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try
            StrSQL.Length = 0
            StrSQL.Append(" With ")
            StrSQL.Append(" AnalisiLaboratorio_CTE As (")
            StrSQL.Append(" SELECT Analisi_Tipologia_Cod, Cod_Risum ")
            StrSQL.Append(" FROM   Analisi_Tipologia_Laboratori ")
            StrSQL.Append(" WHERE  Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.Append(" AND    Validita_Fine>= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
            StrSQL.Append(" AND    PivaSuperUser= '" & Trim(objParametri.PivaSuperUser) & "'")
            StrSQL.Append(")")

            StrSQL.Append(" SELECT * ")
            StrSQL.Append(" FROM   Analisi_Tipologia ")

            StrSQL.Append(" INNER JOIN AnalisiLaboratorio_CTE ON AnalisiLaboratorio_CTE.Analisi_Tipologia_Cod = Analisi_Tipologia.Analisi_Tipologia_Cod ")

            StrSQL.Append(" WHERE  Analisi_Tipologia.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.Append(" AND    Analisi_Tipologia.Validita_Fine>= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
            StrSQL.Append(" AND    Analisi_Tipologia.PivaSuperUser= '" & Trim(objParametri.PivaSuperUser) & "'")

            If Analisi_Tipologia_Cod <> 0 Then
                StrSQL.Append(" AND Analisi_Tipologia.Analisi_Tipologia_Cod = " & Analisi_Tipologia_Cod & " ")
            End If

            If Analisi_Tipologia_Tipo <> 0 Then
                StrSQL.Append(" AND Analisi_Tipologia.Analisi_Tipologia_Tipo = " & Analisi_Tipologia_Tipo & " ")
            End If

            If Cod_Risum <> 0 Then
                StrSQL.Append(" AND AnalisiLaboratorio_CTE.Cod_Risum = " & Cod_Risum & " ")
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   Analisi_Tipologia.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   Analisi_Tipologia.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
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

    Public Function TipologiaDes_from_TipologiaCod(ByVal Analisi_Tipologia_Cod As Integer,
                                                   ByVal Analisi_Tipologia_Tipo As Integer,
                                                   ByRef Analisi_Tipologia_Des_Long As String,
                                                   ByRef Numero_Determinazioni As String,
                                                   ByRef objParametri As AgronicaCoreParametri
                                                   ) As String

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.Analisi_Tipologia_R.TipologiaDes_from_TipologiaCod()"

        Dim dt As DataTable
        dt = Leggi(Analisi_Tipologia_Cod, Analisi_Tipologia_Tipo, "", "", objParametri)

        If dt.Rows.Count = 1 Then
            Analisi_Tipologia_Des_Long = dt.Rows(0).Item("Analisi_Tipologia_Des_Long")
            Numero_Determinazioni = dt.Rows(0).Item("Numero_Determinazioni")
            Return dt.Rows(0).Item("Analisi_Tipologia_Des")
        Else
            Return ""
        End If

    End Function

    Public Function Leggi_Schema_da_Laboratorio(Analisi_Tipologia_Tipo As Integer,
                                                Cod_Risum As Integer,
                                                ByRef objParametri As AgronicaCoreParametri
                                                ) As Integer

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.Analisi_Tipologia_R.Leggi_Schema_da_Laboratorio()"

        Dim dt As DataTable
        dt = Leggi(0, Analisi_Tipologia_Tipo,
                   "Analisi_Tipologia_Laboratori.Cod_Risum = " & Cod_Risum,
                   "", objParametri, True)

        If dt.Rows.Count = 1 Then
            Return dt.Rows(0).Item("Analisi_Tipologia_Cod")
        Else
            Throw New Exception("Schema Analisi Analisi del Sangue non impostato per il laboratorio!")
        End If

    End Function

End Class

'#######################################################################
'#######################################################################
'#######################################################################
'#######################################################################
'#######################################################################
'#######################################################################
'#######################################################################
'#######################################################################
'#######################################################################

Public Class Analisi_Tipologia_Dettagli_W
    Inherits AgronicaCoreDataProvider.DataProvider



    '#########################################################################
    Public Function Scrivi(ByVal Analisi_Tipologia_Cod As Integer,
                           ByVal Analisi_Parametro_Cod As Integer,
                           ByVal Udm_Cod As Integer,
                           ByVal LDM As Decimal,
                           ByVal Ordinamento As Integer,
                           ByRef objParametri As AgronicaCoreParametri,
                           Optional ByVal Data_creazione As DateTime = #2/1/1900#,
                           Optional ByVal Data_modifica As DateTime = #2/1/1900#,
                           Optional ByVal username_creazione As String = "",
                           Optional ByVal username_modifica As String = "") As Boolean

        Const nomeRoutine As String = "AgronicaCoreAnagrafeDAL.Analisi_Tipologia_Dettagli_W.Scrivi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If Data_creazione = #2/1/1900# Then
                Data_creazione = Date.Now
            End If

            If Data_modifica = #2/1/1900# Then
                Data_modifica = Date.Now
            End If

            If username_creazione = "" Then
                username_creazione = objParametri.UsernameOperazione
            End If

            If username_modifica = "" Then
                username_modifica = objParametri.UsernameOperazione
            End If

            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.Append("INSERT INTO Analisi_Tipologia_Dettagli( ")
            StrSQL.Append("            PivaSuperUser, Analisi_Tipologia_cod, ")
            StrSQL.Append("            Analisi_Parametro_Cod,         Udm_Cod,")
            StrSQL.Append("            LDM,  ")
            StrSQL.Append("            Ordinamento,  ")
            StrSQL.Append("            Inviato,  ")
            StrSQL.Append("            Data_Creazione,     Data_Modifica, ")
            StrSQL.Append("            UserName_Creazione, UserName_Modifica ")
            StrSQL.Append("            ) ")

            StrSQL.Append("VALUES (")
            StrSQL.Append("          '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Analisi_Tipologia_Cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Analisi_Parametro_Cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Udm_Cod) & " ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(LDM) & " ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Ordinamento) & " ")

            StrSQL.Append("         , 0  ")

            StrSQL.Append("         , " & Agro_SQL_SaveDate(Data_creazione))
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Data_modifica) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(username_modifica) & "' ")

            StrSQL.Append(")")

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



    '#########################################################################
    Public Function Modifica(ByVal Analisi_Tipologia_Cod As Integer,
                             ByVal Analisi_Parametro_Cod As Integer,
                             ByVal Udm_Cod As Integer,
                             ByVal LDM As Decimal,
                             ByRef objParametri As AgronicaCoreParametri
                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Analisi_Tipologia_Dettagli_W.Modifica()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            StrSQL.Length = 0

            StrSQL.Append("UPDATE Analisi_Tipologia_Dettagli SET ")
            StrSQL.Append("    Udm_Cod   =  " & Agro_SQL_SaveNum(Udm_Cod))
            StrSQL.Append("    ,LDM   =  " & Agro_SQL_SaveNum(LDM))

            StrSQL.Append("   ,Data_Modifica     =  " & Agro_SQL_SaveDate(Date.Now))
            StrSQL.Append("   ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")

            StrSQL.Append(" WHERE PivaSuperUser = '" & Trim(objParametri.PivaSuperUser) & "'")
            StrSQL.Append(" AND   Analisi_Tipologia_Cod = " & Agro_SQL_SaveNum(Analisi_Tipologia_Cod) & " ")
            StrSQL.Append(" AND   Analisi_Parametro_Cod = " & Agro_SQL_SaveNum(Analisi_Parametro_Cod) & " ")

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


    '#########################################################################
    Public Function Cancella(ByVal Analisi_Tipologia_Cod As Integer,
                             ByVal Analisi_Parametro_Cod As Integer,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreParametri
                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Analisi_Tipologia_Dettagli_W.Cancella()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then

                StrSQL.Length = 0

                StrSQL.Append(" UPDATE  Analisi_Tipologia_Dettagli ")
                StrSQL.Append(" SET ")
                StrSQL.Append("          Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("         ,Data_Modifica = " & Agro_SQL_SaveDate(Date.Now) & " ")
                StrSQL.Append("         ,Inviato = -1 ")
                StrSQL.Append(" WHERE PivaSuperUser = '" & Agro_SQL_SaveText(Trim(objParametri.PivaSuperUser)) & "' ")
                StrSQL.Append("   AND    Inviato > 0 ")
            Else

                StrSQL.Length = 0

                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM     Analisi_Tipologia_Dettagli ")
                StrSQL.Append(" WHERE    PivaSuperUser = '" & Agro_SQL_SaveText(Trim(objParametri.PivaSuperUser)) & "' ")
                StrSQL.Append(" AND      Inviato = 0 ")

            End If

            If Analisi_Tipologia_Cod <> 0 Then
                StrSQL.Append(" AND Analisi_Tipologia_Cod = " & Agro_SQL_SaveNum(Analisi_Tipologia_Cod) & " ")
            End If
            If Analisi_Parametro_Cod <> 0 Then
                StrSQL.Append(" AND Analisi_Parametro_Cod = " & Agro_SQL_SaveNum(Analisi_Parametro_Cod) & " ")
            End If

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

'#######################################################################
'#######################################################################
'#######################################################################

Public Class Analisi_Tipologia_Dettagli_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function Leggi(ByVal Analisi_Tipologia_Cod As Integer,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreParametri
                          ) As DataTable

        Const nomeRoutine As String = "AgronicaCoreAnagrafeDAL.Analisi_Tipologia_R.Leggi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT * ")
            StrSQL.Append(" FROM   Analisi_Tipologia_Dettagli ")
            StrSQL.Append(" Where    PivaSuperUser= '" & Trim(objParametri.PivaSuperUser) & "'")

            If Analisi_Tipologia_Cod <> 0 Then
                StrSQL.Append(" AND Analisi_Tipologia_Cod = " & Analisi_Tipologia_Cod & " ")
            End If

            '--------------------------------------------------------------------------
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


    '##############################################################################################
    Public Function Leggi_con_Parametro(ByVal Analisi_Tipologia_Cod As Integer,
                                        ByRef objParametri As AgronicaCoreParametri
                                        ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Analisi_Tipologia_R.Leggi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT  Analisi_Tipologia_Dettagli.*, Analisi_Parametri.* ")
            StrSQL.Append(" FROM         Analisi_Tipologia_Dettagli INNER JOIN ")
            StrSQL.Append("       Analisi_Parametri ON Analisi_Tipologia_Dettagli.Analisi_Parametro_Cod = Analisi_Parametri.Analisi_Parametro_Cod ")

            StrSQL.Append(" Where    Analisi_Tipologia_Dettagli.PivaSuperUser= '" & Trim(objParametri.PivaSuperUser) & "'")

            If Analisi_Tipologia_Cod <> 0 Then
                StrSQL.Append(" AND Analisi_Tipologia_Dettagli.Analisi_Tipologia_Cod = " & Analisi_Tipologia_Cod & " ")
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

    '##############################################################################################
    Public Function Leggi_con_PaDes_FiltroContesto(ByVal Analisi_Tipologia_Cod As Integer,
                                                   ByVal ListaContestiInclusi As List(Of Integer),
                                                   ByVal ListaContestiEsclusi As List(Of Integer),
                                                   ByVal xFiltroAggiuntivo As String,
                                                   ByVal xOrderBy As String,
                                                   ByRef objParametri As AgronicaCoreParametri
                                                   ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Analisi_Tipologia_R.Leggi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine("  Select ")
            StrSQL.AppendLine("   atd.*")
            StrSQL.AppendLine(" , pa.Pa_Des")
            StrSQL.AppendLine(" , pa.Pa_Cod")
            StrSQL.AppendLine(" From Analisi_Tipologia_Dettagli atd")
            StrSQL.AppendLine("	INNER Join PrincipiAttivi pa")
            StrSQL.AppendLine("            On atd.Analisi_Parametro_Cod = pa.Pa_Cod")
            StrSQL.AppendLine("	inner Join PrincipiAttiviXPrincipiAttivi_Contesto ppc")
            StrSQL.AppendLine("        On pa.pa_cod = ppc.PA_COD")

            StrSQL.Append(" Where    atd.PivaSuperUser= '" & Trim(objParametri.PivaSuperUser) & "'")

            If Analisi_Tipologia_Cod <> 0 Then
                StrSQL.Append(" AND Analisi_Tipologia_Cod = " & Analisi_Tipologia_Cod & " ")
            End If

            If ListaContestiEsclusi.Count > 0 Then
                StrSQL.AppendLine("and ppc.principiAttivi_Contesto_COD Not in (" & Agro_SQL_Save_Clausola_IN(String.Join(",", ListaContestiEsclusi), False) & ")")
            Else
                StrSQL.AppendLine("and ppc.principiAttivi_Contesto_COD in (" & Agro_SQL_Save_Clausola_IN(String.Join(",", ListaContestiInclusi), False) & ")")
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   atd.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   atd.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
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


    '##############################################################################################
    Public Function Leggi_con_PaDes(ByVal Analisi_Tipologia_Cod As Integer,
                                    ByVal xFiltroAggiuntivo As String,
                                    ByVal xOrderBy As String,
                                    ByRef objParametri As AgronicaCoreParametri
                                    ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Analisi_Tipologia_R.Leggi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT  Analisi_Tipologia_Dettagli.*, PrincipiAttivi.Pa_Des, PrincipiAttivi.Pa_Cod ")
            StrSQL.Append(" FROM    Analisi_Tipologia_Dettagli INNER JOIN ")
            StrSQL.Append("       PrincipiAttivi ON Analisi_Tipologia_Dettagli.Analisi_Parametro_Cod = PrincipiAttivi.Pa_Cod ")
            StrSQL.Append(" Where    PivaSuperUser= '" & Trim(objParametri.PivaSuperUser) & "'")

            If Analisi_Tipologia_Cod <> 0 Then
                StrSQL.Append(" AND Analisi_Tipologia_Cod = " & Analisi_Tipologia_Cod & " ")
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   Analisi_Tipologia_Dettagli.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   Analisi_Tipologia_Dettagli.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
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

End Class


'#######################################################################
'#######################################################################
'#######################################################################
'#######################################################################
'#######################################################################
'#######################################################################
'#######################################################################
'#######################################################################
'#######################################################################

Public Class Analisi_Tipologia_Laboratori_W
    Inherits AgronicaCoreDataProvider.DataProvider


    '#########################################################################
    Public Function Scrivi(ByVal Cod_Risum As Integer,
                           ByVal Analisi_Tipologia_Cod As Integer,
                           ByVal Validita_Inizio As Date,
                           ByVal Validita_Fine As Date,
                           ByRef objParametri As AgronicaCoreParametri,
                           Optional ByVal Data_creazione As DateTime = #2/1/1900#,
                           Optional ByVal Data_modifica As DateTime = #2/1/1900#,
                           Optional ByVal username_creazione As String = "",
                           Optional ByVal username_modifica As String = ""
                           ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Analisi_Tipologia_Laboratori_W.Scrivi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If Data_creazione = #2/1/1900# Then
                Data_creazione = Date.Now
            End If

            If Data_modifica = #2/1/1900# Then
                Data_modifica = Date.Now
            End If

            If username_creazione = "" Then
                username_creazione = objParametri.UsernameOperazione
            End If

            If username_modifica = "" Then
                username_modifica = objParametri.UsernameOperazione
            End If

            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.Append("INSERT INTO Analisi_Tipologia_Laboratori( ")
            StrSQL.Append("            PivaSuperUser, Cod_Risum, ")
            StrSQL.Append("            Analisi_Tipologia_Cod, ")
            StrSQL.Append("            Inviato,  ")
            StrSQL.Append("            Data_Creazione,     Data_Modifica, ")
            StrSQL.Append("            UserName_Creazione, UserName_Modifica, ")
            StrSQL.Append("            Validita_Inizio,    Validita_Fine ")
            StrSQL.Append("            ) ")

            StrSQL.Append("VALUES (")
            StrSQL.Append("          '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Cod_Risum) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Analisi_Tipologia_Cod) & "  ")

            StrSQL.Append("         , 0  ")

            StrSQL.Append("         , " & Agro_SQL_SaveDate(Data_creazione))
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Data_modifica) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(username_modifica) & "' ")

            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Inizio))
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Fine))

            StrSQL.Append(")")

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


    '#########################################################################
    Public Function Modifica(ByVal Cod_Risum As Integer,
                             ByVal Analisi_Tipologia_Cod As Integer,
                             ByVal Validita_Inizio As Date,
                             ByVal Validita_Fine As Date,
                             ByRef objParametri As AgronicaCoreParametri
                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Analisi_Tipologia_Laboratori_W.Modifica()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            StrSQL.Length = 0
            
            If Validita_Inizio = New Date Then
                Validita_Inizio = AGRODATAINIZIO
            End If
            If Validita_Fine = New Date Then
                Validita_Fine = AGRODATAFINE
            End If

            StrSQL.Append("UPDATE Analisi_Tipologia_Laboratori SET ")
            StrSQL.Append("    Validita_Inizio    =  " & Agro_SQL_SaveDate(Validita_Inizio))
            StrSQL.Append("    ,Validita_Fine    =  " & Agro_SQL_SaveDate(Validita_Fine))

            StrSQL.Append("   ,Data_Modifica     =  " & Agro_SQL_SaveDate(Date.Now))
            StrSQL.Append("   ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")

            StrSQL.Append(" WHERE PivaSuperUser = '" & Trim(objParametri.PivaSuperUser) & "'")
            StrSQL.Append(" AND   Cod_Risum  = " & Agro_SQL_SaveNum(Cod_Risum) & " ")
            StrSQL.Append(" AND   Analisi_Tipologia_Cod = " & Agro_SQL_SaveNum(Analisi_Tipologia_Cod) & " ")

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


    '#########################################################################
    Public Function Cancella(ByVal Cod_Risum As Integer,
                             ByVal Analisi_Tipologia_Cod As Integer,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreParametri
                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Analisi_Tipologia_Laboratori_W.Cancella()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then

                StrSQL.Length = 0

                StrSQL.Append(" UPDATE  Analisi_Tipologia_Laboratori ")
                StrSQL.Append(" SET ")
                StrSQL.Append("          Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("         ,Data_Modifica = " & Agro_SQL_SaveDate(Date.Now) & " ")
                StrSQL.Append("         ,Inviato = -1 ")
                StrSQL.Append(" WHERE PivaSuperUser = '" & Agro_SQL_SaveText(Trim(objParametri.PivaSuperUser)) & "' ")
                StrSQL.Append("   AND    Inviato > 0 ")
            Else

                StrSQL.Length = 0

                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM     Analisi_Tipologia_Laboratori ")
                StrSQL.Append(" WHERE    PivaSuperUser = '" & Agro_SQL_SaveText(Trim(objParametri.PivaSuperUser)) & "' ")
                StrSQL.Append(" AND      Inviato = 0 ")

            End If

            If Analisi_Tipologia_Cod <> 0 Then
                StrSQL.Append(" AND Analisi_Tipologia_Cod = " & Agro_SQL_SaveNum(Analisi_Tipologia_Cod) & " ")
            End If
            If Cod_Risum <> 0 Then
                StrSQL.Append(" AND Cod_Risum = " & Agro_SQL_SaveNum(Cod_Risum) & " ")
            End If

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

'#######################################################################
'#######################################################################
'#######################################################################

Public Class Analisi_Tipologia_Laboratori_R
    Inherits AgronicaCoreDataProvider.DataProvider


    '##############################################################################################
    Public Function Leggi(ByVal Analisi_Tipologia_Cod As Integer,
                          ByVal Cod_Risum As Integer,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreParametri
                          ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Analisi_Tipologia_Laboratori_R.Leggi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT * ")
            StrSQL.Append(" FROM   Analisi_Tipologia_Laboratori ")
            StrSQL.Append(" WHERE  Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.Append(" AND    Validita_Fine>= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
            StrSQL.Append(" AND    PivaSuperUser= '" & Trim(objParametri.PivaSuperUser) & "'")

            If Cod_Risum <> 0 Then
                StrSQL.Append(" AND Cod_Risum = " & Cod_Risum & " ")
            End If

            If Analisi_Tipologia_Cod <> 0 Then
                StrSQL.Append(" AND Analisi_Tipologia_Cod = " & Analisi_Tipologia_Cod & " ")
            End If

            '--------------------------------------------------------------------------
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


    '##############################################################################################
    Public Function LeggiconTipologiaDes(ByVal Analisi_Tipologia_Cod As Integer,
                                         ByVal Cod_Risum As Integer,
                                         ByVal xFiltroAggiuntivo As String,
                                         ByVal xOrderBy As String,
                                         ByRef objParametri As AgronicaCoreParametri,
                                         Optional ByVal soloStandard As Boolean = False
                                         ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Analisi_Tipologia_Laboratori_R.Leggi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT Analisi_Tipologia.Analisi_Tipologia_Cod, Analisi_Tipologia.Analisi_Tipologia_Des ")
            StrSQL.Append(" FROM  Analisi_Tipologia ")

            If Not soloStandard Then
                StrSQL.Append("       INNER JOIN Analisi_Tipologia_Laboratori ON Analisi_Tipologia_Laboratori.PivaSuperUser = Analisi_Tipologia.PivaSuperUser AND  ")
                StrSQL.Append("       Analisi_Tipologia_Laboratori.Analisi_Tipologia_Cod = Analisi_Tipologia.Analisi_Tipologia_Cod")
            End If

            StrSQL.Append(" WHERE  Analisi_Tipologia.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.Append(" AND    Analisi_Tipologia.Validita_Fine>= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            If soloStandard Then
                StrSQL.Append(" AND Analisi_Tipologia.Analisi_Tipologia_Cod < 0 ")
            Else
                If Cod_Risum <> 0 Then
                    StrSQL.Append(" AND Cod_Risum = " & Cod_Risum & " ")
                End If
            End If

            StrSQL.Append(" AND    Analisi_Tipologia.PivaSuperUser= '" & Trim(objParametri.PivaSuperUser) & "'")


            If Analisi_Tipologia_Cod <> 0 Then
                StrSQL.Append(" AND Analisi_Tipologia.Analisi_Tipologia_Cod = " & Analisi_Tipologia_Cod & " ")
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   Analisi_Tipologia.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   Analisi_Tipologia.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY Analisi_Tipologia_Des")
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

End Class
