Imports System.Transactions
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreEntityFramework

Public Class KendoCache_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi(ByVal Piva As String,
                            ByVal Utente As String,
                            ByVal Sito_Cod As Enum_SiteRedirector,
                            ByVal Pagina_Cod As Integer,
                            ByVal NomeDiv As String,
                            ByVal xFiltroAggiuntivo As String,
                            ByVal xOrderBy As String,
                            ByVal numero_validazione As String,
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                            Optional ByRef NumeroRecordLetti As Integer = 0
                            ) As String

        Dim NomeRoutine As String = "AgronicaCoreVarieDAL.KendoCache_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim StrKendo As String

        Try

            '----------------------------------------------------
            '--- Preparo la Query SQL ---------------------------
            '----------------------------------------------------

            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT StrKendo FROM KendoCache ")

            StrSQL.AppendLine(" WHERE  PivaSuperUser = " & Agro_SQL_SaveText_NULL(objParametri.PivaSuperUser))
            StrSQL.AppendLine(" AND Piva = " & Agro_SQL_SaveText_NULL(Piva))
            StrSQL.AppendLine(" AND Utente = " & Agro_SQL_SaveText_NULL(objParametri.UtenteUsername))
            StrSQL.AppendLine(" AND Sito_Cod = " & Agro_SQL_SaveNum_NULL(Sito_Cod))
            StrSQL.AppendLine(" AND Pagina_Cod = " & Agro_SQL_SaveNum_NULL(Pagina_Cod))
            StrSQL.AppendLine(" AND NomeDiv = " & Agro_SQL_SaveText_NULL(NomeDiv))
            If numero_validazione IsNot Nothing Then
                StrSQL.AppendLine(" AND numero_validazione = " & Agro_SQL_SaveText_NULL(numero_validazione))
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            If xOrderBy <> "" Then
                StrSQL.AppendLine(Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            NumeroRecordLetti = DT.Rows.Count

            If Not IsNothing(DT) AndAlso DT.Rows.Count > 0 Then
                StrKendo = DT.Rows(0).Item("StrKendo")
            Else
                StrKendo = ""
            End If

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return StrKendo

    End Function

End Class

Public Class KendoCache_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Scrivi(ByVal Piva As String,
                            ByVal Sito_Cod As Enum_SiteRedirector,
                            ByVal Pagina_Cod As Integer,
                            ByVal NomeDiv As String,
                            ByVal StrKendo As String,
                            ByVal numero_validazione As String,
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                            ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreVarieDAL.KendoCache_W.Scrivi()"

        '====================================================================================
        'Parametri opzionali :
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        'Try
        '    '---------------------------------------------
        '    StrSQL.Length = 0
        '    StrSQL.AppendLine("INSERT INTO KendoCache ")
        '    StrSQL.AppendLine(" ( PivaSuperUser, Piva, Utente, Sito_Cod, Pagina_Cod, NomeDiv, StrKendo ")
        '    StrSQL.AppendLine(", username_creazione, data_creazione, username_modifica, data_modifica, numero_validazione) ")

        '    StrSQL.AppendLine("VALUES (")

        '    StrSQL.AppendLine("           " & Agro_SQL_SaveText_NULL(objParametri.PivaSuperUser))
        '    StrSQL.AppendLine("         , " & Agro_SQL_SaveText_NULL(Piva))
        '    StrSQL.AppendLine("         , " & Agro_SQL_SaveText_NULL(objParametri.UtenteUsername))
        '    StrSQL.AppendLine("         , " & Agro_SQL_SaveNum_NULL(Sito_Cod))
        '    StrSQL.AppendLine("         , " & Agro_SQL_SaveNum_NULL(Pagina_Cod))
        '    StrSQL.AppendLine("         , " & Agro_SQL_SaveText_NULL(NomeDiv))
        '    StrSQL.AppendLine("         , " & Agro_SQL_SaveText_NULL(StrKendo))

        '    StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
        '    StrSQL.AppendLine("         , " & Agro_SQL_SaveDateTime(DateTime.Now) & " ")
        '    StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
        '    StrSQL.AppendLine("         , " & Agro_SQL_SaveDateTime(DateTime.Now) & " ")
        '    StrSQL.AppendLine("         , " & Agro_SQL_SaveText_NULL(numero_validazione) & " ")

        '    StrSQL.Append(" )")
        '    '---------------------------------------------


        '    '--------------------------------------------------------------------------
        '    xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
        '    '--------------------------------------------------------------------------

        'Catch ex As Exception

        '    MessaggioErrore = ex.Message
        '    Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
        '    Scrivi_LOG(objParametri, NomeRoutine, "Query: " & StrSQL.ToString)
        '    xRisp = False
        '    Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        'End Try

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
        Dim transactionOptions = New TransactionOptions()
        transactionOptions.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted
        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            Try

                Dim kendoCache As New AgronicaCoreEntityFramework_POCO.KendoCache

                kendoCache.PivaSuperUser = objParametri.PivaSuperUser
                kendoCache.Piva = Piva
                kendoCache.Utente = objParametri.UtenteUsername
                kendoCache.Sito_Cod = Sito_Cod
                kendoCache.Pagina_Cod = Pagina_Cod
                kendoCache.NomeDiv = NomeDiv
                kendoCache.StrKendo = StrKendo

                kendoCache.Username_Creazione = objParametri.UsernameOperazione
                kendoCache.Data_Creazione = DateTime.Now
                kendoCache.Username_Modifica = objParametri.UsernameOperazione
                kendoCache.Data_Modifica = DateTime.Now

                kendoCache.numero_validazione = numero_validazione

                GiasContext.KendoCache.Add(kendoCache)

                GiasContext.SaveChanges()
                xRisp = True
            Catch ex As Exception

                MessaggioErrore = ex.Message
                Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
                Scrivi_LOG(objParametri, NomeRoutine, "Query: " & StrSQL.ToString)
                xRisp = False
                Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

            End Try

        End Using


        Return xRisp

    End Function

    Public Function Modifica(ByVal Piva As String,
                            ByVal Sito_Cod As Enum_SiteRedirector,
                            ByVal Pagina_Cod As Integer,
                            ByVal NomeDiv As String,
                            ByVal StrKendo As String,
                            ByVal numero_validazione As String,
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                            ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreVarieDAL.KendoCache_W.Modifica()"



        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        ''------------------------------

        'Try

        '    StrSQL.Length = 0

        '    StrSQL.AppendLine(" UPDATE KendoCache SET ")

        '    StrSQL.AppendLine("    Data_Modifica     =  " & Agro_SQL_SaveDateTime(DateTime.Now))
        '    StrSQL.AppendLine("   ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")

        '    StrSQL.AppendLine("   ,StrKendo           = " & Agro_SQL_SaveText_NULL(StrKendo))

        '    StrSQL.AppendLine(" WHERE   PivaSuperUser   = " & Agro_SQL_SaveText_NULL(objParametri.PivaSuperUser))
        '    StrSQL.AppendLine(" AND Piva = " & Agro_SQL_SaveText_NULL(Piva))
        '    StrSQL.AppendLine(" AND Utente = " & Agro_SQL_SaveText_NULL(objParametri.UtenteUsername))
        '    StrSQL.AppendLine(" AND Sito_Cod = " & Agro_SQL_SaveNum_NULL(Sito_Cod))
        '    StrSQL.AppendLine(" AND Pagina_Cod = " & Agro_SQL_SaveNum_NULL(Pagina_Cod))
        '    StrSQL.AppendLine(" AND NomeDiv = " & Agro_SQL_SaveText_NULL(NomeDiv))
        '    StrSQL.AppendLine(" AND numero_validazione = " & Agro_SQL_SaveText_NULL(numero_validazione))

        '    '--------------------------------------------------------------------------
        '    xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
        '    '--------------------------------------------------------------------------

        'Catch ex As Exception
        '    MessaggioErrore = ex.Message
        '    Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
        '    xRisp = False
        '    Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        'End Try

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
        Dim transactionOptions = New TransactionOptions()
        transactionOptions.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted
        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            Try

                Dim kendoCache = GiasContext.KendoCache.Attach(New AgronicaCoreEntityFramework_POCO.KendoCache With
                                                               {
                                                               .PivaSuperUser = objParametri.PivaSuperUser,
                                                               .Piva = Piva,
                                                               .Utente = objParametri.UtenteUsername,
                                                               .Sito_Cod = Sito_Cod,
                                                               .Pagina_Cod = Pagina_Cod,
                                                               .NomeDiv = NomeDiv,
                                                               .numero_validazione = numero_validazione
                })

                kendoCache.Data_Modifica = DateTime.Now
                kendoCache.StrKendo = StrKendo

                GiasContext.Entry(Of AgronicaCoreEntityFramework_POCO.KendoCache)(kendoCache).Property(Function(ee) ee.StrKendo).IsModified = True
                GiasContext.Entry(Of AgronicaCoreEntityFramework_POCO.KendoCache)(kendoCache).Property(Function(ee) ee.Data_Modifica).IsModified = True

                GiasContext.Configuration.ValidateOnSaveEnabled = False

                GiasContext.SaveChanges()
                xRisp = True

            Catch ex As Exception

                MessaggioErrore = ex.Message
                Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
                Scrivi_LOG(objParametri, NomeRoutine, "Query: " & StrSQL.ToString)
                xRisp = False
                Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

            End Try

        End Using


        Return xRisp


    End Function

    Public Function Cancella(ByVal Piva As String,
                            ByVal Sito_Cod As Enum_SiteRedirector,
                            ByVal Pagina_Cod As Integer,
                            ByVal NomeDiv As String,
                             ByVal numero_validazione As String,
                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                             ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreVarieDAL.KendoCache_W.Cancella()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = AgronicaCoreDataProvider.AgronicaCoreParametri.enumCancellazioneLogica.CancellazioneLogica Then

                StrSQL.Length = 0
                StrSQL.Append(" UPDATE KendoCache ")
                StrSQL.Append(" SET ")
                StrSQL.Append("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("      ,Inviato = -1 ")
                StrSQL.Append(" WHERE  Inviato >= 0 ")

            Else
                StrSQL.Length = 0
                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM KendoCache ")
                StrSQL.Append(" WHERE  1=1 ")

            End If

            StrSQL.AppendLine(" AND PivaSuperUser   = " & Agro_SQL_SaveText_NULL(objParametri.PivaSuperUser))
            StrSQL.AppendLine(" AND Piva = " & Agro_SQL_SaveText_NULL(Piva))
            'StrSQL.AppendLine(" AND Utente = " & Agro_SQL_SaveText_NULL(objParametri.UtenteUsername))
            StrSQL.AppendLine(" AND Sito_Cod = " & Agro_SQL_SaveNum_NULL(Sito_Cod))
            StrSQL.AppendLine(" AND Pagina_Cod = " & Agro_SQL_SaveNum_NULL(Pagina_Cod))
            StrSQL.AppendLine(" AND NomeDiv = " & Agro_SQL_SaveText_NULL(NomeDiv))
            StrSQL.AppendLine(" AND numero_validazione = " & Agro_SQL_SaveText_NULL(numero_validazione))

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

    '##########################################
    'utilizzata da utility di modifica piva
    Public Function CancellaxPIVA(ByVal Piva As String,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                    ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreVarieDAL.KendoCache_W.CancellaxPIVA()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = AgronicaCoreDataProvider.AgronicaCoreParametri.enumCancellazioneLogica.CancellazioneLogica Then

                StrSQL.Length = 0
                StrSQL.Append(" UPDATE KendoCache ")
                StrSQL.Append(" SET ")
                StrSQL.Append("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("      ,Inviato = -1 ")
                StrSQL.Append(" WHERE  Inviato >= 0 ")

            Else
                StrSQL.Length = 0
                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM KendoCache ")
                StrSQL.Append(" WHERE  1=1 ")

            End If

            StrSQL.Append(" AND PivaSuperUser   = " & Agro_SQL_SaveText_NULL(objParametri.PivaSuperUser))
            StrSQL.Append(" AND Piva = " & Agro_SQL_SaveText_NULL(Piva))

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
