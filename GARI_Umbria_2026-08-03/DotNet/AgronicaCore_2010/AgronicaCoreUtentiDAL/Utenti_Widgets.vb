Imports System.Data.Common
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.DataProviderExtensions
Imports AgronicaCoreDataProvider.CostantiPersonalizzate

'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§

Public Class Utenti_Widgets_R
    Inherits AgronicaCoreDataProvider.DataProvider


    'Versione che utilizza AgronicaCoreParametri
    '##############################################################################################
    Public Function Leggi(ByVal UserName As String,
                            ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile,
                            ByVal xFiltroAggiuntivo As String,
                            ByVal xOrderBy As String,
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                            Optional ByVal Visibile As Boolean? = Nothing,
                            Optional ByVal Abilitato As Boolean? = Nothing,
                            Optional ByVal IdWidget As Integer? = Nothing
                        ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreUtentiDAL.Widgets_R.Leggi()"


        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta
                    '//////////////////////////////////////////////////////////////////////
                    '//////////////////////////////////////////////////////////////////////
                    StrSQL.Length = 0
                    StrSQL.Append(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;")
                    StrSQL.Append(" SELECT * ")
                    StrSQL.Append(" FROM  Utenti_Widgets ")
                    StrSQL.Append(" WHERE   Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND     Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    If Not String.IsNullOrEmpty(UserName) Then
                        StrSQL.Append(" AND UserName = '" & Agro_SQL_SaveText(UserName) & "' ")
                    End If

                    If Not IsNothing(Visibile) AndAlso Visibile.HasValue Then
                        StrSQL.Append(" AND Visibile = " & Convert.ToInt32(Visibile.Value))
                    End If

                    If Not IsNothing(Abilitato) AndAlso Abilitato.HasValue Then
                        StrSQL.Append(" AND Abilitato = " & Convert.ToInt32(Abilitato))
                    End If

                    If Not IsNothing(IdWidget) AndAlso IdWidget.HasValue Then
                        StrSQL.Append(" AND IdWidget = " & Agro_SQL_SaveNum(IdWidget))
                    End If

                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Inviato =-1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY IdWidget ASC")
                    End If

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta


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

End Class


'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§

Public Class Utenti_Widgets_W
    Inherits AgronicaCoreDataProvider.DataProvider


    'Versione che utilizza AgronicaCoreParametri
    '##############################################################################################
    Public Function Scrivi(
                          ByVal userName As String,
                          ByVal id As Integer,
                          ByVal visibile As Boolean,
                          ByVal abilitato As Boolean,
                          ByVal aspetto As String,
                          ByVal parametri As String,
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                          Optional ByVal validitaInizio As DateTime? = Nothing,
                          Optional ByVal validitaFIne As DateTime? = Nothing,
                          Optional ByVal userNameCreazione As String = Nothing,
                          Optional ByVal userNameModifica As String = Nothing
                        ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreUtentiDAL.Utenti_Widgets_W.Scrivi()"
        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '--------------------------------------------------------------------------
            If String.IsNullOrEmpty(userName) Then
                Throw New Exception("Parametro non corretto nella query (userName obbligatorio)")
            End If

            If id <= 0 Then
                Throw New Exception("Parametro non corretto nella query (Id obbligatorio)")
            End If

            ''--------------------------------------------------------------------------

            If IsNothing(validitaInizio) Then
                validitaInizio = AGRODATAINIZIO
            End If

            If IsNothing(validitaFIne) Then
                validitaFIne = AGRODATAFINE
            End If

            If IsNothing(userNameCreazione) Then
                userNameCreazione = objParametri.UtenteUsername
            End If

            If IsNothing(userNameModifica) Then
                userNameModifica = objParametri.UtenteUsername
            End If

            ''//////////////////////////////////////////////////////////////////////
            ''//////////////////////////////////////////////////////////////////////
            StrSQL.Length = 0

            StrSQL.Append(" INSERT INTO [Utenti_Widgets] ")

            StrSQL.Append("            ([UserName] ")
            StrSQL.Append("            ,[IdWidget] ")
            StrSQL.Append("            ,[Visibile] ")
            StrSQL.Append("            ,[Abilitato] ")
            StrSQL.Append("            ,[Aspetto] ")
            StrSQL.Append("            ,[Parametri] ")
            StrSQL.Append("            ,[inviato] ")
            StrSQL.Append("            ,[datainvio] ")
            StrSQL.Append("            ,[Data_Creazione] ")
            StrSQL.Append("            ,[Data_Modifica] ")
            StrSQL.Append("            ,[Username_Creazione] ")
            StrSQL.Append("            ,[Username_Modifica] ")
            StrSQL.Append("            ,[Validita_Inizio] ")
            StrSQL.Append("            ,[Validita_Fine] ) ")

            StrSQL.Append("                 VALUES ")

            StrSQL.Append("            ( ")
            StrSQL.Append("              '" & Agro_SQL_SaveText(userName) & "' ")
            StrSQL.Append("            , " & Agro_SQL_SaveNum(id) & " ")
            StrSQL.Append("            , " & Convert.ToInt32(visibile) & " ")
            StrSQL.Append("            , " & Convert.ToInt32(abilitato) & " ")
            StrSQL.Append("            , '" & Agro_SQL_SaveText(aspetto) & "' ")
            StrSQL.Append("            , '" & Agro_SQL_SaveText(parametri) & "' ")
            StrSQL.Append("            , 0 ")
            StrSQL.Append("            ,NULL ")
            StrSQL.Append("            , " & Agro_SQL_SaveDateTime(DateTime.Now) & " ")
            StrSQL.Append("            , " & Agro_SQL_SaveDateTime(DateTime.Now) & " ")
            StrSQL.Append("            ,'" & Agro_SQL_SaveText(userNameCreazione) & "' ")
            StrSQL.Append("            ,'" & Agro_SQL_SaveText(userNameModifica) & "' ")
            StrSQL.Append("            , " & Agro_SQL_SaveDate(validitaInizio) & " ")
            StrSQL.Append("            , " & Agro_SQL_SaveDate(validitaFIne) & " ")
            StrSQL.Append("            ) ")

            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp

    End Function


    Public Function Modifica(
        ByVal UserName As String,
        ByVal IdWidget As Integer,
        ByVal Visibile As Boolean?,
        ByVal Abilitato As Boolean?,
        ByVal Aspetto As String,
        ByVal Parametri As String,
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
        Optional ByVal Data_modifica As DateTime = #2/1/1900#,
        Optional ByVal username_modifica As String = ""
    ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreUtentiDAL.Utenti_Widgets_W.Modifica()"
        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            If String.IsNullOrEmpty(UserName) Then
                Throw New Exception("Parametro non corretto nella query (UserName obbligatorio)")
            End If

            If IdWidget <= 0 Then
                Throw New Exception("Parametro non corretto nella query (IdWidget obbligatorio)")
            End If

            If Data_modifica = #2/1/1900# Then
                Data_modifica = Now
            End If

            If username_modifica = "" Then
                username_modifica = objParametri.UsernameOperazione
            End If

            StrSQL.Length = 0

            StrSQL.Append(" UPDATE Utenti_Widgets ")

            If Not IsNothing(Visibile) AndAlso Visibile.HasValue Then
                StrSQL.Append(" SET    Visibile = " & Convert.ToInt32(Visibile) & " ")
            End If

            If Not IsNothing(Abilitato) AndAlso Abilitato.HasValue Then
                StrSQL.Append("      , Abilitato = " & Convert.ToInt32(Abilitato) & " ")
            End If

            If Not String.IsNullOrEmpty(Aspetto) Then
                StrSQL.Append("      , Aspetto = '" & Agro_SQL_SaveText(Aspetto) & "' ")
            End If
            If Not String.IsNullOrEmpty(Parametri) Then
                StrSQL.Append("      , Parametri = '" & Agro_SQL_SaveText(Parametri) & "' ")
            End If
            StrSQL.Append("      , inviato = 0 ")
            StrSQL.Append("      , datainvio = NULL ")
            StrSQL.Append("      , Data_Modifica = " & Agro_SQL_SaveDateTime(Data_modifica) & " ")
            StrSQL.Append("      , Username_Modifica = '" & Agro_SQL_SaveText(username_modifica) & "' ")

            StrSQL.Append(" WHERE Username = '" & Agro_SQL_SaveText(UserName) & "' ")
            StrSQL.Append(" And IdWidget = " & IdWidget)

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

    Public Function Cancella(ByVal USerName As String,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As String

        Dim NomeRoutine As String = "AgronicaCoreUtentiDAL.Utenti_Widgets_W.Cancella()"
        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        If String.IsNullOrEmpty(USerName) Then
            Throw New Exception("Parametro non corretto nella query (UserName obbligatorio)")
        End If

        Try

            StrSQL.Length = 0
            StrSQL.AppendLine(" DELETE FROM Utenti_Widgets ")
            StrSQL.AppendLine(" WHERE UserName = '" + Agro_SQL_SaveText(USerName) + "' ")

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

    'Versione che utilizza AgronicaCoreParametri
    '##############################################################################################
    'Public Function Cancella(
    '                        ByVal Veg_Cod As Integer,
    '                            ByVal xFiltroAggiuntivo As String,
    '                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
    '                            ) As Boolean

    '    Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.SpecieVegetali_W.Cancella()"
    '    Dim MessaggioErrore As String = ""
    '    Dim StrSQL As New System.Text.StringBuilder
    '    Dim xRisp As Boolean = False

    '    Try
    '        '--------------------------------------------------------------------------
    '        'Verifica informazioni 
    '        '
    '        If IsNothing(Veg_Cod) Then
    '            Throw New Exception("Parametro non corretto nella query (Veg_Cod obbligatorio)")
    '        End If
    '        '--------------------------------------------------------------------------


    '        '//////////////////////////////////////////////////////////////////////
    '        '//////////////////////////////////////////////////////////////////////
    '        If objParametri.FlagCancellazioneLogica = AgronicaCoreDataProvider.AgronicaCoreParametri.enumCancellazioneLogica.CancellazioneLogica Then

    '            StrSQL.Length = 0
    '            StrSQL.Append(" UPDATE Agenda ")
    '            StrSQL.Append(" SET ")
    '            StrSQL.Append("       Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
    '            StrSQL.Append("      ,[Data_Modifica] = " & Agro_SQL_SaveDate(Date.Now) & " ")
    '            StrSQL.Append("      ,Inviato = -1 ")
    '            StrSQL.Append(" WHERE  Inviato >= 0 ")
    '        Else
    '            StrSQL.Length = 0
    '            StrSQL.Append(" DELETE ")
    '            StrSQL.Append(" FROM Agenda ")
    '            StrSQL.Append(" WHERE  1=1 ")
    '        End If
    '        '----------------------------------------------------------------------
    '        StrSQL.Append(" AND Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod) & " ")




    '        If xFiltroAggiuntivo <> "" Then
    '            StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
    '        End If

    '        '--------------------------------------------------------------------------
    '        xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
    '        '--------------------------------------------------------------------------

    '    Catch ex As Exception
    '        MessaggioErrore = ex.Message
    '        Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
    '        xRisp = False
    '        Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
    '    End Try

    '    Return xRisp

    'End Function


End Class


'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################











