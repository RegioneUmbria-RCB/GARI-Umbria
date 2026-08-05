Imports System.Data.Common
Imports AgronicaCoreDataProvider.UtilityProvider

Public Class Stalla_Configurazioni_BDN_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi(
                            ByVal ID As Integer,
                            ByVal Piva As String,
                            ByVal Sa_Cod As Int32,
                            ByVal STA_NUM As Int32,
                            ByVal CF_Detentore As String,
                            ByVal CF_Proprietario As String,
                            ByVal Validita_Inizio As Date,
                            ByVal Validita_Fine As Date,
                                ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Stalla_Configurazioni_BDN_R.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            'If Cod_Indirizzo = 0 Then
            '    Throw New Exception("Parametro non corretto nella query (Cod_Indirizzo obbligatorio)")
            'End If

            '------------------------------------------------------------------
            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                    StrSQL.Length = 0

                    StrSQL.Append(" SELECT * ")
                    StrSQL.Append(" FROM  Stalla_Configurazioni_BDN ")
                    StrSQL.Append(" WHERE Validita_inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
                    StrSQL.Append(" AND   Validita_Fine >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")

                    If ID <> 0 Then
                        StrSQL.Append(" AND ID = '" & Agro_SQL_SaveNum(ID) & "' ")
                    End If

                    If Piva <> "" Then
                        StrSQL.Append(" AND PIVA = '" & Agro_SQL_SaveText(Trim(Piva)) & "' ")
                    End If

                    If Sa_Cod <> 0 Then
                        StrSQL.Append(" AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
                    End If

                    If STA_NUM <> 0 Then
                        StrSQL.Append(" AND STA_NUM = " & Agro_SQL_SaveNum(STA_NUM) & "  ")
                    End If

                    If CF_Detentore <> "" Then
                        StrSQL.Append(" AND CF_Detentore = '" & Agro_SQL_SaveText(Trim(CF_Detentore)) & "'  ")
                    End If

                    If CF_Proprietario <> "" Then
                        StrSQL.Append(" AND CF_Proprietario = '" & Agro_SQL_SaveText(Trim(CF_Proprietario)) & "'  ")
                    End If


                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   Stalla_Configurazioni_BDN.Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Stalla_Configurazioni_BDN.Inviato =-1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY CF_Detentore, CF_Proprietario ASC ")
                    End If




                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta

                    StrSQL.Length = 0

                    StrSQL.Append(" SELECT * ")
                    StrSQL.Append(" FROM  Stalla_Configurazioni_BDN ")
                    StrSQL.Append(" WHERE Validita_inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
                    StrSQL.Append(" AND   Validita_Fine >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")

                    If ID <> 0 Then
                        StrSQL.Append(" AND ID = '" & Agro_SQL_SaveNum(ID) & "' ")
                    End If

                    If Piva <> "" Then
                        StrSQL.Append(" AND PIVA = '" & Agro_SQL_SaveText(Trim(Piva)) & "' ")
                    End If

                    If Sa_Cod <> 0 Then
                        StrSQL.Append(" AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
                    End If

                    If STA_NUM <> 0 Then
                        StrSQL.Append(" AND STA_NUM = " & Agro_SQL_SaveNum(STA_NUM) & "  ")
                    End If

                    If CF_Detentore <> "" Then
                        StrSQL.Append(" AND CF_Detentore = '" & Agro_SQL_SaveText(Trim(CF_Detentore)) & "'  ")
                    End If

                    If CF_Proprietario <> "" Then
                        StrSQL.Append(" AND CF_Proprietario = '" & Agro_SQL_SaveText(Trim(CF_Proprietario)) & "'  ")
                    End If


                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   Stalla_Configurazioni_BDN.Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Stalla_Configurazioni_BDN.Inviato =-1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else

                    End If



                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni
                    '
                    '
                    '
                    '


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

    Public Function LeggiDaCodiceAziendaBDN(
                            ByVal ID As Integer,
                            ByVal Piva As String,
                            ByVal Sa_Cod As Int32,
                            ByVal STA_NUM As Int32,
                            ByVal CF_Detentore As String,
                            ByVal CF_Proprietario As String,
                            ByVal Codice_Azienda_BDN As String,
                            ByVal Validita_Inizio As Date,
                            ByVal Validita_Fine As Date,
                            ByVal xFiltroAggiuntivo As String,
                            ByVal xOrderBy As String,
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Stalla_Configurazioni_BDN_R.LeggiDaCodiceAziendaBDN()"

        '====================================================================================
        'Parametri opzionali :
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT * ")
            StrSQL.AppendLine(" FROM  Stalla_Configurazioni_BDN ")
            StrSQL.AppendLine(" JOIN Stalla ON	Stalla_Configurazioni_BDN.Piva = Stalla.Piva ")
            StrSQL.AppendLine("                 AND Stalla_Configurazioni_BDN.sa_cod = Stalla.sa_cod ")
            StrSQL.AppendLine("                 AND Stalla_Configurazioni_BDN.STA_NUM = Stalla.STA_NUM ")
            StrSQL.AppendLine(" WHERE Stalla_Configurazioni_BDN.Validita_inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
            StrSQL.AppendLine(" AND   Stalla_Configurazioni_BDN.Validita_Fine >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")

            If ID <> 0 Then
                StrSQL.AppendLine(" AND Stalla_Configurazioni_BDN.ID = '" & Agro_SQL_SaveNum(ID) & "' ")
            End If

            If Piva <> "" Then
                StrSQL.AppendLine(" AND Stalla_Configurazioni_BDN.PIVA = '" & Agro_SQL_SaveText(Trim(Piva)) & "' ")
            End If

            If Sa_Cod <> 0 Then
                StrSQL.AppendLine(" AND Stalla_Configurazioni_BDN.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            End If

            If STA_NUM <> 0 Then
                StrSQL.AppendLine(" AND Stalla_Configurazioni_BDN.STA_NUM = " & Agro_SQL_SaveNum(STA_NUM) & "  ")
            End If

            If CF_Detentore <> "" Then
                StrSQL.AppendLine(" AND Stalla_Configurazioni_BDN.CF_Detentore = '" & Agro_SQL_SaveText(Trim(CF_Detentore)) & "'  ")
            End If

            If CF_Proprietario <> "" Then
                StrSQL.AppendLine(" AND Stalla_Configurazioni_BDN.CF_Proprietario = '" & Agro_SQL_SaveText(Trim(CF_Proprietario)) & "'  ")
            End If

            If Codice_Azienda_BDN <> "" Then
                StrSQL.AppendLine(" AND Stalla.BDN_Codice_Azienda = '" & Agro_SQL_SaveText(Trim(Codice_Azienda_BDN)) & "'  ")
            End If


            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.AppendLine(" AND   Stalla_Configurazioni_BDN.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.AppendLine(" AND   Stalla_Configurazioni_BDN.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine(" ORDER BY CF_Detentore, CF_Proprietario ASC ")
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


Public Class Stalla_Configurazioni_BDN_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Scrivi(ByVal ID As Integer,
                                ByVal Piva As String,
                                ByVal Sa_Cod As Integer,
                                ByVal STA_NUM As Integer,
                                ByVal CF_Detentore As String,
                                ByVal CF_Proprietario As String,
                                ByVal RagSoc_Detentore As String,
                                ByVal RagSoc_Proprietario As String,
                                    ByVal Validita_Inizio As Date,
                                    ByVal Validita_Fine As Date,
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                        ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Stalla_Configurazioni_BDN_W.Scrivi()"

        '====================================================================================
        'Parametri opzionali :
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If ID = 0 Then
                Throw New Exception("ID non Valorizzato")
            End If

            If Piva = "" Then
                Throw New Exception("Piva non Valorizzata")
            End If

            If Sa_Cod = 0 Then
                Throw New Exception("Sa_Cod non Valorizzato")
            End If

            If STA_NUM = 0 Then
                Throw New Exception("STA_NUM non Valorizzato")
            End If

            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.Append(" INSERT INTO [dbo].[Stalla_Configurazioni_BDN] ")
            StrSQL.Append("            ([ID] ")
            StrSQL.Append("            ,[Piva] ")
            StrSQL.Append("            ,[sa_cod] ")
            StrSQL.Append("            ,[STA_NUM] ")
            StrSQL.Append("            ,[CF_Detentore] ")
            StrSQL.Append("            ,[CF_Proprietario] ")
            StrSQL.Append("            ,[RagSoc_Detentore] ")
            StrSQL.Append("            ,[RagSoc_Proprietario] ")
            StrSQL.Append("            ,[inviato] ")
            StrSQL.Append("            ,[datainvio] ")
            StrSQL.Append("            ,[Data_Creazione] ")
            StrSQL.Append("            ,[Data_Modifica] ")
            StrSQL.Append("            ,[Username_Creazione] ")
            StrSQL.Append("            ,[Username_Modifica] ")
            StrSQL.Append("            ,[Validita_Inizio] ")
            StrSQL.Append("            ,[Validita_Fine]) ")
            StrSQL.Append("      VALUES ")
            StrSQL.Append("            (" & Agro_SQL_SaveNum(ID) & " ")
            StrSQL.Append("            ,'" & Agro_SQL_SaveText(Piva) & "' ")
            StrSQL.Append("            ," & Agro_SQL_SaveNum(Sa_Cod) & " ")
            StrSQL.Append("            ," & Agro_SQL_SaveNum(STA_NUM) & " ")
            StrSQL.Append("            ,'" & Agro_SQL_SaveText(CF_Detentore) & "' ")
            StrSQL.Append("            ,'" & Agro_SQL_SaveText(CF_Proprietario) & "' ")
            StrSQL.Append("            ,'" & Agro_SQL_SaveText(RagSoc_Detentore) & "' ")
            StrSQL.Append("            ,'" & Agro_SQL_SaveText(RagSoc_Proprietario) & "' ")
            StrSQL.Append("            ,0 ")
            StrSQL.Append("            ,NULL ")
            StrSQL.Append("            ," & Agro_SQL_SaveDateTime(DateTime.Now) & " ")
            StrSQL.Append("            ," & Agro_SQL_SaveDateTime(DateTime.Now) & " ")
            StrSQL.Append("            ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.Append("            ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.Append("            ," & Agro_SQL_SaveDate(Validita_Inizio) & " ")
            StrSQL.Append("            ," & Agro_SQL_SaveDate(Validita_Fine) & ") ")


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

    'Public Function Modifica(
    '                            ByVal Piva As String,
    '                            ByVal Sa_Cod As Int32,
    '                            ByVal Sta_Num As Int32,
    '                            ByVal Cod_Fabb As String,
    '                            ByVal Att_Cod As Int32,
    '                            ByVal Valore As Decimal,
    '                                ByVal Validita_Inizio As Date,
    '                                ByVal Validita_Fine As Date,
    '                                    ByVal xFiltroAggiuntivo As String,
    '                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
    '                                    ) As Boolean

    '    Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Stalla_Caratteristiche_W.Modifica()"

    '    '====================================================================================
    '    'Parametri opzionali :
    '    '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
    '    '   DirectoryLOG = ""           =>  viene usato il valore di default
    '    '   FileLOG = ""                =>  viene usato il valore di default
    '    '====================================================================================

    '    Dim MessaggioErrore As String = ""
    '    Dim StrSQL As New System.Text.StringBuilder
    '    Dim xRisp As Boolean = False

    '    '------------------------------

    '    Try

    '        If Piva = "" Then
    '            Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
    '        End If

    '        If Sa_Cod = 0 Then
    '            Throw New Exception("Parametro non corretto nella query (Sa_Cod obbligatorio)")
    '        End If

    '        If Sta_Num = 0 Then
    '            Throw New Exception("Parametro non corretto nella query (Sta_Num obbligatorio)")
    '        End If

    '        If Cod_Fabb = "" Then
    '            Throw New Exception("Parametro non corretto nella query (Cod_Fabb obbligatorio)")
    '        End If

    '        If Att_Cod = 0 Then
    '            Throw New Exception("Parametro non corretto nella query (Att_Cod obbligatorio)")
    '        End If

    '        '---------------------------------------------

    '        StrSQL.Length = 0

    '        StrSQL.Append(" UPDATE Stalla_Caratteristiche SET ")
    '        StrSQL.Append("    VALORE    = " & Agro_SQL_SaveNum(Valore) & "  ")

    '        StrSQL.Append("   ,Inviato           =  0 ")
    '        StrSQL.Append("   ,DataInvio         =  Null ")
    '        StrSQL.Append("   ,Data_Modifica     =  " & Agro_SQL_SaveDate(Date.Now))
    '        StrSQL.Append("   ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
    '        StrSQL.Append("   ,Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Inizio))
    '        StrSQL.Append("   ,Validita_Fine     =  " & Agro_SQL_SaveDate(Validita_Fine))

    '        StrSQL.Append(" WHERE PIVA      = '" & Agro_SQL_SaveText(Trim(Piva)) & "' ")
    '        StrSQL.Append(" AND   Sa_Cod    =  " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
    '        StrSQL.Append(" AND   STA_NUM   =  " & Agro_SQL_SaveNum(Sta_Num) & "  ")
    '        StrSQL.Append(" AND   COD_FABB  = '" & Agro_SQL_SaveText(Trim(Cod_Fabb)) & "' ")
    '        StrSQL.Append(" AND   ATT_COD   =  " & Agro_SQL_SaveNum(Att_Cod) & "  ")

    '        '----------------------------------------------------------------------
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

    Public Function Cancella(ByVal ID As Integer,
                               ByVal Piva As String,
                               ByVal Sa_Cod As Integer,
                               ByVal Sta_Num As Integer,
                                   ByVal xFiltroAggiuntivo As String,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                    ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Stalla_Configurazioni_BDN_W.Cancella()"

        '====================================================================================
        'Parametri opzionali :
        '   Cod_Fabb = 0
        '   Att_Cod = 0

        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If ID = 0 Then

                If Piva = "" Then
                    Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
                End If

                If Sa_Cod = 0 Then
                    Throw New Exception("Parametro non corretto nella query (Sa_Cod obbligatorio)")
                End If

                If Sta_Num = 0 Then
                    Throw New Exception("Parametro non corretto nella query (Sta_Num obbligatorio)")
                End If

            End If

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = AgronicaCoreDataProvider.AgronicaCoreParametri.enumCancellazioneLogica.CancellazioneLogica Then

                StrSQL.Length = 0
                StrSQL.Append(" UPDATE Stalla_Configurazioni_BDN ")
                StrSQL.Append(" SET ")
                StrSQL.Append("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("      ,Inviato = -1 ")
                StrSQL.Append(" WHERE  Inviato >= 0 ")

            Else
                StrSQL.Length = 0
                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM Stalla_Configurazioni_BDN ")
                StrSQL.Append(" WHERE  1=1 ")

            End If

            If ID <> 0 Then
                StrSQL.Append(" AND ID = " & Agro_SQL_SaveNum(ID) & "   ")
            End If

            If Piva <> "" Then
                StrSQL.Append(" AND Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If

            If Sa_Cod <> 0 Then
                StrSQL.Append(" AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
            End If

            If Sta_Num <> 0 Then
                StrSQL.Append(" AND Sta_Num = " & Agro_SQL_SaveNum(Sta_Num) & "   ")
            End If

            '----------------------------------------------------------------------
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
