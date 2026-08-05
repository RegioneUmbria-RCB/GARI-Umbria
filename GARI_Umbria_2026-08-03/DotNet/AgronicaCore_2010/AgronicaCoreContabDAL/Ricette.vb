Imports System.Data.Entity
Imports System.Transactions
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.DataProviderExtensions
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreEntityFramework

Public Class Ricette_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function LeggiProdottiRicetteAPP(
                        ByVal Piva As String,
                        ByVal Elem_Cod As Integer,
                        ByVal Pro_Cod As Integer,
                        ByVal Descrizione As String,
                        ByVal Codifica As Boolean,
                        ByVal xFiltroAggiuntivo As String,
                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                        ) As DataTable
        Dim NomeRoutine As String = "Ricette_R.LeggiProdottiRicetteAPP()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            StrSQL.Length = 0
            '---------------------------------------------

            StrSQL.AppendLine(" SELECT DISTINCT r.Piva, i.Rag_Soc, rd.Elem_Cod, rd.Pro_Cod, rd.Descrizione")

            If Elem_Cod = 191 Then
                StrSQL.AppendLine(", Formulati.Fr_Des AS Desc_GIAS ")
            ElseIf Elem_Cod = 3 Then
                StrSQL.AppendLine(", Fertilizzanti.Fer_Des AS Desc_GIAS ")
            End If

            StrSQL.AppendLine(" FROM APP_Ricette_Dettagli rd ")
            StrSQL.AppendLine(" INNER JOIN APP_Ricette r ON r.Ricetta_Cod=rd.Ricetta_Cod ")
            StrSQL.AppendLine(" AND substring(rd.id,0,charindex('|',rd.id)) = substring(r.id,0,charindex('|',r.id)) ")
            StrSQL.AppendLine(" INNER JOIN Imprese i ON r.Piva=i.Piva ")

            If Elem_Cod = 191 Then
                StrSQL.AppendLine(" LEFT JOIN Formulati ON rd.Pro_Cod = Formulati.Fr_Cod ")
            ElseIf Elem_Cod = 3 Then
                StrSQL.AppendLine(" LEFT JOIN Fertilizzanti ON rd.Pro_Cod = Fertilizzanti.Fer_Cod ")
            End If

            StrSQL.AppendLine(" WHERE 1=1 ")

            If Piva <> "" Then
                StrSQL.AppendLine(" AND r.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If

            StrSQL.AppendLine(" AND rd.Elem_Cod = " & Agro_SQL_SaveNum(Elem_Cod))

            If Pro_Cod <> 0 Then
                StrSQL.AppendLine(" AND rd.Pro_Cod = " & Agro_SQL_SaveNum(Pro_Cod))
            End If

            If Codifica Then
                StrSQL.AppendLine(" AND rd.Descrizione <> '' AND rd.Codice_Extra = '' ")
            End If

            If Descrizione <> "" Then
                StrSQL.AppendLine(" AND rd.Descrizione LIKE '%" & Agro_SQL_SaveText(Descrizione) & "%' ")
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------

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

    Public Function LeggiRicetteDettagliAPP(ByVal piva As String, ByVal elem_cod As Integer, ByVal pro_cod As Integer, ByVal descrizione As String, ByVal codifica As Boolean, objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri) As List(Of AgronicaCoreEntityFramework_POCO.APP_Ricette_Dettagli)

        Dim rval As List(Of AgronicaCoreEntityFramework_POCO.APP_Ricette_Dettagli)

        Dim gefutils As New Gias_EF_Utility
        Dim efConnString As String = gefutils.GetEntityConnectionString(objParametri_Server.StringaConnessione)

        Using giasContext As New Gias_DeveloperServer_Entities(efConnString)

            rval = (From rd In giasContext.APP_Ricette_Dettagli
                    Join ro In giasContext.APP_Ricette_Operazioni
                        On rd.Ricetta_Cod Equals ro.Ricetta_Cod And rd.Ricetta_Operazione_Cod Equals ro.Ricetta_Operazione_Cod And rd.ID.Substring(0, rd.ID.IndexOf("|")) Equals ro.ID.Substring(0, ro.ID.IndexOf("|"))
                    Join r In giasContext.APP_Ricette
                        On ro.Ricetta_Cod Equals r.Ricetta_Cod And ro.ID.Substring(0, ro.ID.IndexOf("|")) Equals r.ID.Substring(0, r.ID.IndexOf("|"))
                    Where (piva = "" OrElse r.Piva = piva) AndAlso
                          (elem_cod = 0 OrElse rd.Elem_Cod = elem_cod) AndAlso
                          (pro_cod = 0 OrElse rd.Pro_Cod = pro_cod) AndAlso
                          (descrizione = "" OrElse rd.Descrizione.Contains(descrizione)) AndAlso
                          If(codifica, rd.Codice_Extra = "" AndAlso rd.Descrizione <> "", True)
                    Select rd).ToList()

        End Using

        Return rval

    End Function

    Public Function Ricetta_Leggi_APP(ByVal piva As String, objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri) As List(Of AgronicaCoreEntityFramework_POCO.Ricette)

        Dim rval As List(Of AgronicaCoreEntityFramework_POCO.Ricette)

        Dim gefutils As New Gias_EF_Utility
        Dim efConnString As String = gefutils.GetEntityConnectionString(objParametri_Server.StringaConnessione)

        Using giasContext As New Gias_DeveloperServer_Entities(efConnString)

            rval = (From r In giasContext.Ricette
                    Where r.Piva = piva
                    Select r).ToList()

        End Using

        Return rval

    End Function

    Public Function Ricetta_Leggi_dtAPP(ByVal piva As String, ByVal xFiltroAggiuntivo As String, objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim NomeRoutine As String = "Ricetta_Leggi_dtAPP"
        Try

            '------------------------------------------------------------------
            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT DISTINCT Ricette.* ")
            StrSQL.AppendLine(" FROM  Ricette ")
            StrSQL.AppendLine(" inner join  Ricette_Operazioni ")
            StrSQL.AppendLine(" on Ricette.ricetta_cod = Ricette_Operazioni.Ricetta_COD ")
            StrSQL.AppendLine(" Where Ricette.Piva = '" & Agro_SQL_SaveText(piva) & "' ")

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine("AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri_Server))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri_Server, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore, ex)
        End Try
        Return DT

    End Function

    '##############################################################################################
    Public Function LeggiXDestinazione(
                            ByVal Ricetta_Cod As Int32,
                            ByVal Piva As String,
                            ByVal Sa_Cod As Int32,
                            ByVal Appezza As Int32,
                            ByVal id_reg As Int32,
                            ByVal Tipo_Ricetta As Int32,
                            ByVal Veg_Cod As Int32,
                            ByVal Validita_Inizio As Date,
                            ByVal Validita_Fine As Date,
                                ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                Optional ByVal FiltriXlettura_vuoti As Boolean = False
                                ) As DataTable

        Const NomeRoutine = "AgronicaCoreContabDAL.Ricette_R.LeggiXDestinazione()"

        '====================================================================================
        'Parametri opzionali :
        '   Ricetta_SuperUser = "" 
        '   Piva = "" 
        '   Ricetta_Cod = 0    
        '   Veg_Cod = 0
        '   Tipo_Ricetta = 0
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            '------------------------------------------------------------------
            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta

                    StrSQL.Length = 0

                    StrSQL.AppendLine(" SELECT DISTINCT R.* ")
                    StrSQL.AppendLine(" FROM  Ricette R ")

                    StrSQL.AppendLine(" inner join  Ricette_Destinazioni rr on ")
                    StrSQL.AppendLine(" R.ricetta_cod = rr.Ricetta_Cod " & vbCrLf)

                    StrSQL.AppendLine(" WHERE R.Validita_inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
                    StrSQL.AppendLine(" AND   R.Validita_Fine >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")

                    If objParametri.PivaSuperUser <> "" Then
                        StrSQL.AppendLine(" AND R.Ricetta_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'   ")
                    End If

                    If Piva <> "" Then
                        StrSQL.AppendLine(" AND R.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
                    Else
                        If FiltriXlettura_vuoti Then
                            StrSQL.AppendLine(" AND R.Piva = ''   ")
                        End If
                    End If

                    If Sa_Cod <> 0 Then
                        StrSQL.AppendLine(" AND RR.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
                    Else
                        If FiltriXlettura_vuoti Then
                            StrSQL.AppendLine(" AND R.Sa_Cod = 0   ")
                        End If
                    End If

                    If Appezza <> 0 Then
                        StrSQL.AppendLine(" AND RR.appezza = " & Agro_SQL_SaveNum(Appezza) & "   ")
                    Else
                        If FiltriXlettura_vuoti Then
                            StrSQL.AppendLine(" AND RR.appezza = 0   ")
                        End If
                    End If

                    If id_reg <> 0 Then
                        StrSQL.AppendLine(" AND RR.id_reg = " & Agro_SQL_SaveNum(id_reg) & "   ")
                    Else
                        If FiltriXlettura_vuoti Then
                            StrSQL.AppendLine(" AND RR.id_reg = 0   ")
                        End If
                    End If

                    If Ricetta_Cod <> 0 Then
                        StrSQL.AppendLine(" AND R.Ricetta_Cod = " & Agro_SQL_SaveNum(Ricetta_Cod) & "   ")
                    End If

                    If Tipo_Ricetta <> 0 Then
                        StrSQL.AppendLine(" AND R.Tipo_Ricetta = " & Agro_SQL_SaveNum(Tipo_Ricetta) & "   ")
                    End If

                    If Veg_Cod <> 0 Then
                        StrSQL.AppendLine(" AND R.Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod) & " ")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.AppendLine(" AND   R.Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.AppendLine(" AND   R.Inviato =-1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.AppendLine(" ORDER BY R.Ricetta_SuperUser, R.Ricetta_Cod Asc ")
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


    '##############################################################################################
    Public Function Leggi(ByVal Ricetta_Cod As Int32,
                            ByVal Piva As String,
                            ByVal Sa_Cod As Int32,
                            ByVal Tipo_Ricetta As Int32,
                            ByVal Veg_Cod As Int32,
                            ByVal Validita_Inizio As Date,
                            ByVal Validita_Fine As Date,
                                ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                Optional ByVal FiltriXlettura_vuoti As Boolean = False
                                ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.Ricette_R.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   Ricetta_SuperUser = "" 
        '   Piva = "" 
        '   Ricetta_Cod = 0    
        '   Veg_Cod = 0
        '   Tipo_Ricetta = 0
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            '------------------------------------------------------------------
            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                    StrSQL.Length = 0

                    StrSQL.AppendLine(" SELECT * ")
                    StrSQL.AppendLine(" FROM  Ricette ")
                    StrSQL.AppendLine(" WHERE Ricette.Validita_inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
                    StrSQL.AppendLine(" AND   Ricette.Validita_Fine >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")

                    If objParametri.PivaSuperUser <> "" Then
                        StrSQL.AppendLine(" AND Ricette.Ricetta_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'   ")
                    End If

                    If Piva <> "" Then
                        StrSQL.AppendLine(" AND Ricette.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
                    Else
                        If FiltriXlettura_vuoti Then
                            StrSQL.AppendLine(" AND Ricette.Piva = ''   ")
                        End If
                    End If

                    If Sa_Cod <> 0 Then
                        StrSQL.AppendLine(" AND Ricette.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
                    Else
                        If FiltriXlettura_vuoti Then
                            StrSQL.AppendLine(" AND Ricette.Sa_Cod = 0   ")
                        End If
                    End If

                    If Ricetta_Cod <> 0 Then
                        StrSQL.AppendLine(" AND Ricette.Ricetta_Cod = " & Agro_SQL_SaveNum(Ricetta_Cod) & "   ")
                    End If

                    If Tipo_Ricetta <> 0 Then
                        StrSQL.AppendLine(" AND Ricette.Tipo_Ricetta = " & Agro_SQL_SaveNum(Tipo_Ricetta) & "   ")
                    End If

                    If Veg_Cod <> 0 Then
                        StrSQL.AppendLine(" AND Ricette.Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod) & " ")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.AppendLine(" AND   Ricette.Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.AppendLine(" AND   Ricette.Inviato =-1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.AppendLine(" ORDER BY Ricette.Ricetta_SuperUser, Ricette.Ricetta_Cod Asc ")
                    End If



                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta

                    StrSQL.Length = 0

                    StrSQL.AppendLine(" SELECT * ")
                    StrSQL.AppendLine(" FROM  Ricette ")
                    StrSQL.AppendLine(" WHERE Ricette.Validita_inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
                    StrSQL.AppendLine(" AND   Ricette.Validita_Fine >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")

                    If objParametri.PivaSuperUser <> "" Then
                        StrSQL.AppendLine(" AND Ricette.Ricetta_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'   ")
                    End If

                    If Piva <> "" Then
                        StrSQL.AppendLine(" AND Ricette.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
                    Else
                        If FiltriXlettura_vuoti Then
                            StrSQL.AppendLine(" AND Ricette.Piva = ''   ")
                        End If
                    End If

                    If Sa_Cod <> 0 Then
                        StrSQL.AppendLine(" AND Ricette.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
                    Else
                        If FiltriXlettura_vuoti Then
                            StrSQL.AppendLine(" AND Ricette.Sa_Cod = 0   ")
                        End If
                    End If


                    If Ricetta_Cod <> 0 Then
                        StrSQL.AppendLine(" AND Ricette.Ricetta_Cod = " & Agro_SQL_SaveNum(Ricetta_Cod) & "   ")
                    End If

                    If Tipo_Ricetta <> 0 Then
                        StrSQL.AppendLine(" AND Ricette.Tipo_Ricetta = " & Agro_SQL_SaveNum(Tipo_Ricetta) & "   ")
                    End If

                    If Veg_Cod <> 0 Then
                        StrSQL.AppendLine(" AND Ricette.Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod) & " ")
                    End If


                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.AppendLine(" AND   Ricette.Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.AppendLine(" AND   Ricette.Inviato =-1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.AppendLine(" ORDER BY Ricette.Ricetta_SuperUser, Ricette.Ricetta_Cod Asc ")
                    End If


                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni

                    StrSQL.Length = 0

                    StrSQL.AppendLine(" SELECT Ricette.*, SpecieVegetali.Veg_Des ")
                    StrSQL.AppendLine(" FROM  Ricette INNER JOIN SpecieVegetali ON Ricette.Veg_Cod = SpecieVegetali.Veg_Cod ")
                    StrSQL.AppendLine(" AND Ricette.Validita_inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
                    StrSQL.AppendLine(" AND Ricette.Validita_Fine >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")

                    If objParametri.PivaSuperUser <> "" Then
                        StrSQL.AppendLine(" AND Ricette.Ricetta_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'   ")
                    End If

                    If Piva <> "" Then
                        StrSQL.AppendLine(" AND Ricette.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
                    Else
                        If FiltriXlettura_vuoti Then
                            StrSQL.AppendLine(" AND Ricette.Piva = ''   ")
                        End If
                    End If

                    If Sa_Cod <> 0 Then
                        StrSQL.AppendLine(" AND Ricette.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
                    Else
                        If FiltriXlettura_vuoti Then
                            StrSQL.AppendLine(" AND Ricette.Sa_Cod = 0   ")
                        End If
                    End If


                    If Ricetta_Cod <> 0 Then
                        StrSQL.AppendLine(" AND Ricette.Ricetta_Cod = " & Agro_SQL_SaveNum(Ricetta_Cod) & "   ")
                    End If

                    If Tipo_Ricetta <> 0 Then
                        StrSQL.AppendLine(" AND Ricette.Tipo_Ricetta = " & Agro_SQL_SaveNum(Tipo_Ricetta) & "   ")
                    End If

                    If Veg_Cod <> 0 Then
                        StrSQL.AppendLine(" AND Ricette.Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod) & " ")
                    End If


                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.AppendLine(" AND   Ricette.Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.AppendLine(" AND   Ricette.Inviato =-1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.AppendLine(" ORDER BY Ricette.Ricetta_SuperUser, Ricette.Ricetta_Cod Asc ")
                    End If


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

    '##############################################################################################
    Public Function LeggiFertilizzazioniPUA(ByVal Programmazione_Cod As Int32,
                                            ByVal Validita_Inizio As Date,
                                            ByVal Validita_Fine As Date,
                                            ByVal xFiltroAggiuntivo As String,
                                            ByVal xOrderBy As String,
                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                            ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.Ricette_Read.LeggiFertilizzazioniPUA()"

        '====================================================================================
        'Parametri opzionali :
        '   Validita_Inizio = 0 
        '   Validita_Fine = 0 
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try


            StrSQL.Length = 0


            StrSQL.AppendLine("SELECT F.Id_Fert, F.Piva_SuperUser,CONVERT(varchar(15), F.Data_Fert,103) AS Data_Fert, F.Programmazione_Entita_Cod, F.Id_Tp_Fer, F.Fer_Cod, F.N_Reale, ")
            StrSQL.AppendLine("F.Eff_Perc, F.ApportoxHa, F.ApportoxDistrib, F.NnettoxHa, F.NnettoxDistrib, F.NutilexHa, F.NutilexDistrib, F.EM_Cod, ")
            StrSQL.AppendLine("PE.Entita_Des, PE.TipoZona, PE.Superficie, FERT.Fer_Des, TF.Descrizione, EM.EM_Des, 1 AS Salvataggio ")
            StrSQL.AppendLine("FROM Fertilizzazione AS F, Programmazione_Entita AS PE, Programmazione_Testata AS PT, Fertilizzanti AS FERT, TipoFertilizzante AS TF, EpocheModalita AS EM ")
            StrSQL.AppendLine("WHERE F.Piva_SuperUser = PE.Piva_SuperUser AND F.Programmazione_Entita_Cod = PE.Programmazione_Entita_Cod ")
            StrSQL.AppendLine("AND PE.Piva_SuperUser = PT.Piva_SuperUser AND PE.Programmazione_Cod = PT. Programmazione_Cod ")
            StrSQL.AppendLine("AND FERT.Fer_Cod = F.Fer_Cod AND TF.Id_Tp_Fer = F.Id_Tp_Fer ")
            StrSQL.AppendLine("AND F.EM_Cod = EM.EM_Cod ")
            StrSQL.AppendLine("AND PT.Piva_SuperUser = '" & objParametri.PivaSuperUser & "' AND PT.Programmazione_Cod = " & Programmazione_Cod.ToString & " ")

            StrSQL.AppendLine(" UNION ")

            StrSQL.AppendLine(" SELECT DISTINCT Ricette.Ricetta_Cod as Id_Fert, Ricette.Ricetta_SuperUser AS Piva_SuperUser, CONVERT(varchar(15),Ricette.Validita_Inizio,103) AS Data_Fert, ")
            StrSQL.AppendLine(" Ricette_Destinazioni.Programmazione_Entita_Cod, Ricette_Operazioni.Id_Tp_Fer, Ricette_Dettagli.Pro_Cod as Fer_Cod, ")
            StrSQL.AppendLine(" Ricette_Dettaglio_Tecnico.N AS N_Reale, Ricette_Operazioni.Eff_Perc, ")
            StrSQL.AppendLine(" Ricette_Dettaglio_Tecnico.ApportoxHa, CAST((Ricette_Dettaglio_Tecnico.ApportoxHa*PE.Superficie) AS integer) AS ApportoxDistrib, ")
            StrSQL.AppendLine(" Ricette_Dettaglio_Tecnico.NnettoxHa, (Ricette_Dettaglio_Tecnico.NnettoxHa*PE.Superficie) AS NnettoxDistrib, ")
            StrSQL.AppendLine(" Ricette_Dettaglio_Tecnico.NutilexHa, CAST((Ricette_Dettaglio_Tecnico.NutilexHa*PE.Superficie) AS integer) AS NutilexDistrib, ")
            StrSQL.AppendLine(" Ricette_Operazioni.EM_Cod, PE.Entita_Des, PE.TipoZona, PE.Superficie, ISNULL(FERT.Fer_Des, '') AS Fer_Des, TF.descrizione,  EM.EM_Des, 2 AS Salvataggio ")
            StrSQL.AppendLine(" FROM Ricette INNER JOIN ")
            StrSQL.AppendLine(" Ricette_Operazioni ON Ricette.Ricetta_SuperUser = Ricette_Operazioni.Ricetta_SuperUser AND  ")
            StrSQL.AppendLine(" Ricette.Ricetta_Cod = Ricette_Operazioni.Ricetta_Cod INNER JOIN ")
            StrSQL.AppendLine(" Ricette_Dettagli ON Ricette_Operazioni.Ricetta_SuperUser = Ricette_Dettagli.Ricetta_SuperUser AND  ")
            StrSQL.AppendLine(" Ricette_Operazioni.Ricetta_Cod = Ricette_Dettagli.Ricetta_Cod AND  ")
            StrSQL.AppendLine(" Ricette_Operazioni.Ricetta_Operazione_Cod = Ricette_Dettagli.Ricetta_Operazione_Cod INNER JOIN ")
            StrSQL.AppendLine(" Ricette_Dettaglio_Tecnico ON Ricette_Dettagli.Ricetta_SuperUser = Ricette_Dettaglio_Tecnico.Ricetta_SuperUser AND  ")
            StrSQL.AppendLine(" Ricette_Dettagli.Ricetta_Cod = Ricette_Dettaglio_Tecnico.Ricetta_Cod AND  ")
            StrSQL.AppendLine(" Ricette_Dettagli.Ricetta_Operazione_Cod = Ricette_Dettaglio_Tecnico.Ricetta_Operazione_Cod AND  ")
            StrSQL.AppendLine(" Ricette_Dettagli.Ricetta_Dettaglio_Cod = Ricette_Dettaglio_Tecnico.Ricetta_Dettaglio_Cod INNER JOIN ")
            StrSQL.AppendLine(" Ricette_Destinazioni ON Ricette_Dettagli.Ricetta_SuperUser = Ricette_Destinazioni.Ricetta_SuperUser AND  ")
            StrSQL.AppendLine(" Ricette_Dettagli.Ricetta_Cod = Ricette_Destinazioni.Ricetta_Cod AND  ")
            StrSQL.AppendLine(" Ricette_Dettagli.Ricetta_Operazione_Cod = Ricette_Destinazioni.Ricetta_Operazione_Cod AND  ")
            StrSQL.AppendLine(" Ricette_Dettagli.Ricetta_Dettaglio_Cod = Ricette_Destinazioni.Ricetta_Dettaglio_Cod INNER JOIN ")
            StrSQL.AppendLine(" Programmazione_Entita AS PE ON Ricette_Destinazioni.Ricetta_SuperUser = PE.Piva_SuperUser AND  ")
            StrSQL.AppendLine(" Ricette_Destinazioni.Programmazione_Entita_Cod = PE.Programmazione_Entita_Cod INNER JOIN ")
            StrSQL.AppendLine(" Fertilizzanti AS FERT ON Ricette_Dettagli.Pro_Cod = FERT.Fer_Cod INNER JOIN ")
            StrSQL.AppendLine(" TipoFertilizzante AS TF ON Ricette_Operazioni.Id_Tp_Fer = TF.id_tp_fer INNER JOIN ")
            StrSQL.AppendLine(" EpocheModalita AS EM ON Ricette_Operazioni.EM_Cod = EM.EM_Cod ")

            StrSQL.AppendLine(" AND Ricette.Ricetta_SuperUser = '" & objParametri.PivaSuperUser & "' AND Ricette.Programmazione_Cod = " & Programmazione_Cod.ToString & " ")
            StrSQL.AppendLine(" AND Ricette.Tipo_Ricetta=2 ")   ' Fertilizzazioni PUA


            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.AppendLine(" AND   Ricette.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.AppendLine(" AND   Ricette.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine("ORDER BY Data_Fert ")
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


    Public Function EsistonoOperazioni_suProgrammazione(ByVal Piva As String,
                                                        ByVal Programmazione_Cod As Int32,
                                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                        ) As Boolean

        Dim bRet As Boolean = False

        Try

            Dim dt As DataTable

            dt = Leggi(0, Piva, 0, 0, 0, AGRODATAINIZIO, AGRODATAFINE,
                                  AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                  "Programmazione_Cod=" & Agro_SQL_SaveNum(Programmazione_Cod), "", objParametri)

            If dt.Rows.Count > 0 Then
                bRet = True
            End If

            dt.Dispose()
            dt = Nothing

        Catch ex As Exception

            bRet = False

        Finally


        End Try


        Return bRet


    End Function

    '##############################################################################################
    Public Function Leggi_xGriglia(
                            ByVal Ricetta_Cod As Int32,
                            ByVal Piva As String,
                            ByVal Sa_Cod As Int32,
                            ByVal Tipo_Ricetta As Int32,
                            ByVal Veg_Cod As Int32,
                            ByVal Programmazione_Cod As Int32,
                            ByVal Validita_Inizio As Date,
                            ByVal Validita_Fine As Date,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                Optional ByVal FiltriXlettura_vuoti As Boolean = False
                                ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.Ricette_Read.Leggi_xGriglia()"

        '====================================================================================
        'Parametri opzionali :
        '   Ricetta_SuperUser = "" 
        '   Piva = "" 
        '   Ricetta_Cod = 0    
        '   Veg_Cod = 0
        '   Tipo_Ricetta = 0
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try


            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT R.Ricetta_Cod, R.Piva, CASE WHEN ISNULL(IMP.partitaIvaReale, '') = '' THEN R.Piva ELSE IMP.partitaIvaReale END PivaReale,")
            StrSQL.AppendLine(" IMP.rag_soc, R.Ricetta_Des, R.Ricetta_Des_Long, R.Data_Creazione, R.Ricetta_Numero, R.Note, R.Validita_Inizio, R.Validita_Fine, SP.Veg_Des")
            StrSQL.AppendLine(" ,(select COUNT (*) from Ricette_Operazioni RO where RO.Ricetta_Cod = R.Ricetta_Cod and R.Ricetta_SuperUser = RO.Ricetta_SuperUser) as numero_operazioni")
            StrSQL.AppendLine(" FROM  Ricette R")
            StrSQL.AppendLine(" INNER JOIN SpecieVegetali SP ON R.Veg_Cod = SP.Veg_Cod ")
            StrSQL.AppendLine(" left join Imprese IMP on R.Piva = IMP.PIVA " & vbCrLf)
            StrSQL.AppendLine(" WHERE R.Validita_inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
            StrSQL.AppendLine(" AND   R.Validita_Fine >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")

            If objParametri.PivaSuperUser <> "" Then
                StrSQL.AppendLine(" AND R.Ricetta_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'   ")
            End If

            If Piva <> "" Then
                StrSQL.AppendLine(" AND R.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
            Else
                If FiltriXlettura_vuoti Then
                    StrSQL.AppendLine(" AND R.Piva = ''   ")
                End If
            End If

            If Sa_Cod <> 0 Then
                StrSQL.AppendLine(" AND R.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
            Else
                If FiltriXlettura_vuoti Then
                    StrSQL.AppendLine(" AND R.Sa_Cod = 0   ")
                End If
            End If

            If Ricetta_Cod <> 0 Then
                StrSQL.AppendLine(" AND R.Ricetta_Cod = " & Agro_SQL_SaveNum(Ricetta_Cod) & "   ")
            End If

            If Programmazione_Cod <> 0 Then
                StrSQL.AppendLine(" AND R.Programmazione_Cod = " & Agro_SQL_SaveNum(Programmazione_Cod) & "   ")
            End If

            If Tipo_Ricetta <> 0 Then
                StrSQL.AppendLine(" AND R.Tipo_Ricetta = " & Agro_SQL_SaveNum(Tipo_Ricetta) & "   ")
            End If

            If Veg_Cod <> 0 Then
                StrSQL.AppendLine(" AND R.Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod) & " ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.AppendLine(" AND   R.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.AppendLine(" AND   R.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine(" ORDER BY R.Ricetta_SuperUser, R.Data_Creazione Desc ")
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
    Public Function Leggi_xMenuAgenda(
                            ByVal Piva As String,
                            ByVal Sa_Cod As Int32,
                            ByVal Tipo_Ricetta As Int32,
                            ByVal Veg_Cod As Int32,
                            ByVal Programmazione_Cod As Int32,
                            ByVal Validita_Inizio As Date,
                            ByVal Validita_Fine As Date,
                            ByVal MostraPubblici As Boolean,
                            ByVal xFiltroAggiuntivo As String,
                            ByVal xOrderBy As String,
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                            ByVal strFiltroGruppoOperazioni As String,
                            ByVal strFiltroRicetteOperazioni As String
                            ) As DataTable

        Const NomeRoutine = "AgronicaCoreContabDAL.Ricette_R.Leggi_xMenuAgenda()"

        ' = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = =  = 
        'Parametri opzionali :
        '   Ricetta_SuperUser = "" 
        '   Piva = "" 
        '   Ricetta_Cod = 0    
        '   Veg_Cod = 0
        '   Tipo_Ricetta = 0
        ' = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = =  = 

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT ")
            StrSQL.AppendLine("     r.piva, CASE WHEN ISNULL(imp.partitaIvaReale, '') = '' THEN r.piva ELSE imp.partitaIvaReale END PivaReale, ")
            StrSQL.AppendLine("     r.Sa_Cod, ISNULL(ca.sa_nome,'') AS sa_nome, r.Ricetta_Cod, r.Ricetta_Numero, r.Ricetta_Des, r.Tipo_Ricetta, ISNULL(r.veg_cod, 0) As veg_cod_r, ")

            StrSQL.AppendLine("     ro.Raccoglitore_Cod, ")

            StrSQL.AppendLine("     CASE r.Tipo_Ricetta ")
            StrSQL.AppendLine("     WHEN " & enum_TipoRicetta.Standard & " THEN 'Linea Tecnica' ")
            StrSQL.AppendLine("     WHEN " & enum_TipoRicetta.PUA & " THEN 'Ricetta su Piano Colturale Previsto (budget)' ")
            StrSQL.AppendLine("     WHEN " & enum_TipoRicetta.Budget_Globale & " THEN 'Budget Globale' ")
            StrSQL.AppendLine("     WHEN " & enum_TipoRicetta.Budget_Utente & " THEN 'Budget Utente' ")
            StrSQL.AppendLine("     WHEN " & enum_TipoRicetta.Standard_Destinazioni & " THEN 'Ricetta su Piano Colturale Reale' ")
            StrSQL.AppendLine("     WHEN " & enum_TipoRicetta.PianoDistribuzioneConcimi & " THEN 'Ricetta su Piano Colturale Reale' ")
            StrSQL.AppendLine("     WHEN " & enum_TipoRicetta.ControlloDiGestione & " THEN 'Controllo Di Gestione' ")
            StrSQL.AppendLine("     WHEN " & enum_TipoRicetta.Standard_Destinazioni_Planning & " THEN 'Ricetta su Piano Colturale Previsto (budget)' ")
            StrSQL.AppendLine("     WHEN " & enum_TipoRicetta.PianoDistribuzionePua & " THEN 'Ricetta su PUA su Piano Colturale Reale' ")
            StrSQL.AppendLine("     ELSE NULL END AS Tipo_Ricetta_des, ")

            StrSQL.AppendLine("     r.Validita_Inizio, r.Validita_Fine, o.lav_cod, o.LAV_DES, sv_r.Veg_Des AS Veg_Des_r, sv_op.Veg_Cod AS Veg_Cod_op, sv_op.Veg_Des AS Veg_Des_op, ro.Ricetta_Operazione_Cod, ro.Validita_Inizio AS Ricetta_Operazione_Data, ")
            StrSQL.AppendLine("     rdest.piva, rdest.sa_cod, ISNULL(rdest.appezza, 0) AS appezza, ISNULL(a.app_nome,'') as App_Nome,")
            StrSQL.AppendLine("     ISNULL(rd.elem_cod, 0) As Elem_Cod, ISNULL(rd.Mat_Cod, 0) As Mat_Cod, ISNULL(rd.Pro_Cod, 0) As Pro_Cod,")
            StrSQL.AppendLine("     ISNULL(Fertilizzanti.Fer_Des, '') AS Fer_Des, ISNULL(Formulati.Fr_Des, '') AS Fr_Des, ISNULL(Trappole.Trap_Des, '') AS Trap_Des, ISNULL(InsettiUtili.Ins_Des, '') AS Ins_Des,")
            StrSQL.AppendLine("     ISNULL(Materie_Prime.Mat_Des, '') AS Mat_Des, ISNULL(Materie_Prime.Cod_Articolo, '') AS Cod_Articolo,")
            StrSQL.AppendLine("     ISNULL(rd.PrincipiAttivi, '') AS PrincipiAttivi, ISNULL(rd.PrincipiAttiviPesi, '') AS PrincipiAttiviPesi, ")
            StrSQL.AppendLine("     ISNULL((SELECT TOP 1 1 FROM RicettexAgenda rxa WHERE rxa.Ricetta_SuperUser = ro.Ricetta_SuperUser AND rxa.Ricetta_Cod = ro.Ricetta_Cod AND rxa.Ricetta_Operazione_Cod = ro.Ricetta_Operazione_Cod UNION SELECT TOP 1 1 FROM Ricette_Operazioni ro2 WHERE ro2.Ricetta_SuperUser = ro.Ricetta_SuperUser AND ro2.Ricetta_Operazione_Cod_RIF = ro.Ricetta_Operazione_Cod), 0) AS in_uso,")

            'StrSQL.AppendLine(" ISNULL((SELECT TOP 1 ricetta_operazione_cod FROM Ricette_Operazioni ro2 WHERE ro2.Ricetta_SuperUser = ro.Ricetta_SuperUser AND ro2.Ricetta_Operazione_Cod = ro.Ricetta_Operazione_Cod_RIF), 0) AS ricettaOperazioneDaFareCollegata,")
            StrSQL.AppendLine("     ISNULL(ro.W_Anagrafica_Stati_Cod, 0) AS WAnagraficaStati_Cod,")
            StrSQL.AppendLine("     ISNULL(was.WAnagraficaStati_Des, '') AS WAnagraficaStati_Des,")
            StrSQL.AppendLine("     ISNULL(was.Colore, '') AS WAnagraficaStati_Colore,")
            StrSQL.AppendLine("     ISNULL(ro.APP_Ricetta_Operazione_ID, '') AS APP_Ricetta_Operazione_ID,")
            StrSQL.AppendLine("     CASE WHEN ISNULL(ro.APP_Ricetta_Operazione_ID, '') = '' THEN 'PC' ELSE r.Origine END AS Origine,")
            StrSQL.AppendLine("     ISNULL(codan.descrizione, '') AS DestinazioneTerreniNudi_Des, ")

            'Per Sup_Trattata
            StrSQL.AppendLine("     ISNULL(rdest.ID_REG, 0) AS ID_REG, ")
            StrSQL.AppendLine("     ISNULL(ri.sup_imp, 0) AS SUP_APP, ")
            StrSQL.AppendLine("     ISNULL(rdest.Qta2, 0) AS Sup_Trattata, ")

            StrSQL.AppendLine("     r.blocco_flag, ")
            StrSQL.AppendLine("     ro.Invia_App, ")

            StrSQL.AppendLine("     ISNULL(rdest.MagazzinoEsterno_Cod, '') AS MagazzinoEsterno_Cod, ")
            StrSQL.AppendLine("     ISNULL(rdest.MagazzinoEsterno_Des, '') AS MagazzinoEsterno_Des, ")
            StrSQL.AppendLine("     ISNULL(rdest.MagazzinoEsterno_Dettagli, '') AS MagazzinoEsterno_Dettagli, ")

            'Per quantità in dettaglio tecnico
            StrSQL.AppendLine("     rd.Qta, ")
            StrSQL.AppendLine("     rd.Udm_Cod, ")

            StrSQL.AppendLine("     ro.Ricetta_Operazione_Des, ") '12/02/2024 Aggiunto come campo note

            StrSQL.AppendLine("     ro.Data_Creazione, ")
            StrSQL.AppendLine("     ISNULL(rd.Lotto, '') AS Lotto, ")
            StrSQL.AppendLine("     rdest.Tipo_Destinazione AS Tipo_Destinazione, ")
            StrSQL.AppendLine("     ISNULL(rdest.Qta, 0) AS QtaTotScaricoProdotto ")

            StrSQL.AppendLine(" FROM Ricette_Operazioni ro ")
            StrSQL.AppendLine(" INNER JOIN Ricette r ")
            StrSQL.AppendLine(" ON ro.ricetta_superuser = r.Ricetta_SuperUser AND ro.Ricetta_Cod = r.Ricetta_Cod ")
            StrSQL.AppendLine(" INNER JOIN Ricette_dettagli rd ")
            StrSQL.AppendLine(" ON ro.Ricetta_SuperUser = rd.Ricetta_SuperUser AND ro.Ricetta_Operazione_Cod = rd.Ricetta_Operazione_Cod ")
            StrSQL.AppendLine(" LEFT JOIN Ricette_Destinazioni rdest ")
            StrSQL.AppendLine(" ON rdest.Ricetta_SuperUser = rd.Ricetta_SuperUser AND rdest.Ricetta_Operazione_Cod = rd.Ricetta_Operazione_Cod AND rdest.Ricetta_Dettaglio_Cod = rd.Ricetta_Dettaglio_Cod ")
            StrSQL.AppendLine(" LEFT JOIN Appezzamento a ")
            StrSQL.AppendLine(" ON rdest.Piva = a.Piva AND rdest.sa_cod = a.sa_cod AND rdest.Appezza = a.Appezza ")
            StrSQL.AppendLine(" LEFT JOIN reg_impianti ri ")
            StrSQL.AppendLine(" ON rdest.Piva = ri.Piva AND rdest.sa_cod = ri.sa_cod AND rdest.Appezza = ri.Appezza AND rdest.id_reg = ri.id_reg")
            StrSQL.AppendLine(" LEFT JOIN Imprese imp ")
            StrSQL.AppendLine(" ON r.Piva = imp.Piva ")

            'AF 13/11 - Spostato in alto per integrazione ricette post raccolta + concia del seme, dove il focus dell'operazione sono i prodotti magazzino
            StrSQL.AppendLine(" LEFT JOIN Materie_Prime ")
            StrSQL.AppendLine(" ON rd.Elem_Cod = Materie_Prime.Elem_Cod AND rd.Mat_Cod = Materie_Prime.Mat_Cod ")

            StrSQL.AppendLine(" LEFT JOIN Cultivar c_op ")
            StrSQL.AppendLine(" ON ri.cul_Cod = c_op.Cul_Cod ")

            'AF 13/11 - Modificato join con Specie Vegetali (su cui viene fatto il filtro per caricare la griglia) per integrazione ricette post raccolta + concia del seme, dove il focus dell'operazione sono i prodotti magazzino
            StrSQL.AppendLine(" LEFT JOIN SpecieVegetali sv_op ")
            StrSQL.AppendLine(" ON (c_op.Veg_Cod = sv_op.Veg_Cod  OR (c_op.Veg_Cod IS NULL AND Materie_Prime.veg_cod = sv_op.Veg_Cod)) ")


            StrSQL.AppendLine(" LEFT JOIN Formulati ")
            StrSQL.AppendLine(" ON rd.Pro_Cod = FORMULATI.Fr_Cod ")
            StrSQL.AppendLine(" LEFT JOIN Fertilizzanti ")
            StrSQL.AppendLine(" On rd.Pro_Cod = Fertilizzanti.Fer_Cod ")
            StrSQL.AppendLine(" LEFT JOIN Trappole ")
            StrSQL.AppendLine(" On rd.Pro_Cod = Trappole.TRAP_COD ")
            StrSQL.AppendLine(" LEFT JOIN InsettiUtili ")
            StrSQL.AppendLine(" On rd.Pro_Cod = InsettiUtili.Ins_Cod ")
            StrSQL.AppendLine(" LEFT JOIN SpecieVegetali sv_r ")
            StrSQL.AppendLine(" On r.Veg_Cod = sv_r.Veg_Cod ")
            StrSQL.AppendLine(" INNER JOIN Operazioni o ")
            StrSQL.AppendLine(" On ro.Lav_Cod = o.LAV_COD ")
            StrSQL.AppendLine(" LEFT JOIN Centri_Aziendali ca ")
            StrSQL.AppendLine(" On ca.piva = r.piva And ca.sa_cod = r.Sa_Cod ")
            StrSQL.AppendLine(" LEFT JOIN WAnagraficaStati was ")
            StrSQL.AppendLine(" On ro.W_Anagrafica_Stati_Cod = was.WAnagraficaStati_Cod ")

            '(26/10/2018 MarcoG)  aggiunte le diciture per le destinazioni dei terreni nudi
            StrSQL.AppendLine(" LEFT JOIN Reg_Impianti_Codici ric ")
            StrSQL.AppendLine(" On ri.piva = ric.piva And ri.SA_COD = ric.sa_cod And ri.APPEZZA = ric.appezza And ri.ID_REG = ric.Id_Reg And ri.Cul_Cod = 0 And ric.id_cod BETWEEN 3000 And 3999 ")
            StrSQL.AppendLine(" LEFT JOIN Codici_Anagrafe codan ")
            StrSQL.AppendLine(" On codan.codice = ric.id_cod ")

            StrSQL.AppendLine(" WHERE ro.Ricetta_SuperUser = " & Agro_SQL_SaveText_NULL(objParametri.PivaSuperUser))
            StrSQL.AppendLine(" And ro.Validita_inizio < =  " & Agro_SQL_SaveDateTime_NULL(If(Validita_Fine < objParametri.FinestraTemporaleFine, Validita_Fine, objParametri.FinestraTemporaleFine)))
            StrSQL.AppendLine(" And ro.Validita_inizio > =  " & Agro_SQL_SaveDateTime_NULL(If(Validita_Inizio > objParametri.FinestraTemporaleInizio, Validita_Inizio, objParametri.FinestraTemporaleInizio)))

            If Piva <> "" Then
                If MostraPubblici Then
                    StrSQL.AppendLine(" And (r.Piva = '' OR r.Piva = " & Agro_SQL_SaveText_NULL(Piva) & ")")
                Else
                    StrSQL.AppendLine(" AND r.Piva = " & Agro_SQL_SaveText_NULL(Piva))
                End If
            Else
                If MostraPubblici Then
                    StrSQL.AppendLine(" AND r.Piva = '' ")
                End If
            End If

            If Sa_Cod <> 0 Then
                If MostraPubblici Then
                    StrSQL.AppendLine(" AND (r.Sa_Cod = 0 OR r.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & ")")
                Else
                    StrSQL.AppendLine(" AND r.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod))
                End If
            End If

            If Programmazione_Cod <> 0 Then
                StrSQL.AppendLine(" AND r.Programmazione_Cod = " & Agro_SQL_SaveNum(Programmazione_Cod))
            End If

            If Tipo_Ricetta <> 0 Then
                StrSQL.AppendLine(" AND r.Tipo_Ricetta = " & Agro_SQL_SaveNum(Tipo_Ricetta))
            End If

            If Veg_Cod <> 0 Then
                StrSQL.AppendLine(" AND sv_op.Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod))
            End If

            If strFiltroGruppoOperazioni <> "" Then
                StrSQL.AppendLine(" AND o.GRU_OP IN (" & Agro_SQL_Save_Clausola_IN(strFiltroGruppoOperazioni, False) & ") ")
            End If

            If strFiltroRicetteOperazioni <> "" Then
                StrSQL.AppendLine(" AND ro.Ricetta_Operazione_Cod IN (" & Agro_SQL_Save_Clausola_IN(strFiltroRicetteOperazioni, False) & ") ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.AppendLine(" AND r.Inviato > = 0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.AppendLine(" AND r.Inviato  = -1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine(" ORDER BY ro.Validita_Inizio desc, r.Data_Creazione desc ")
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

    Public Function Leggi_xFertilizzazioniPUA(
                        ByVal Piva As String,
                        ByVal Sa_Cod As Int32,
                        ByVal Veg_Cod As Int32,
                        ByVal Pua_Cod As Int32,
                        ByVal Validita_Inizio As Date,
                        ByVal Validita_Fine As Date,
                        ByVal MostraPubblici As Boolean,
                        ByVal xFiltroAggiuntivo As String,
                        ByVal xOrderBy As String,
                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                        ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.Ricette_Read.Leggi_xFertilizzazioniPUA()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT r.piva, r.Sa_Cod, ISNULL(ca.sa_nome,'') AS sa_nome, r.Ricetta_Cod, r.Ricetta_Numero, r.Ricetta_Des, r.Tipo_Ricetta, ISNULL(r.veg_cod, 0) As veg_cod_r, ")
            StrSQL.AppendLine(" r.Validita_Inizio, r.Validita_Fine, o.lav_cod, o.LAV_DES, sv_r.Veg_Des AS Veg_Des_r, sv_op.Veg_Cod AS Veg_Cod_op, sv_op.Veg_Des AS Veg_Des_op, ro.Ricetta_Operazione_Cod, ro.Validita_Inizio AS Ricetta_Operazione_Data, ")
            StrSQL.AppendLine(" rdest.piva as piva_dest, rdest.sa_cod as sa_cod_dest, ISNULL(rdest.appezza, 0) AS appezza_dest, ISNULL(a.app_nome,'') as App_Nome,")
            StrSQL.AppendLine(" ISNULL(rd.elem_cod, 0) As Elem_Cod, ISNULL(rd.Mat_Cod, 0) As Mat_Cod, ISNULL(rd.Pro_Cod, 0) As Pro_Cod,")
            StrSQL.AppendLine(" ISNULL(Fertilizzanti.Fer_Des, '') AS Fer_Des, ")
            StrSQL.AppendLine(" ISNULL(rd.PrincipiAttivi, '') AS PrincipiAttivi, ISNULL(rd.PrincipiAttiviPesi, '') AS PrincipiAttiviPesi, ")
            StrSQL.AppendLine(" ISNULL((SELECT TOP 1 1 FROM RicettexAgenda rxa WHERE rxa.Ricetta_SuperUser=ro.Ricetta_SuperUser AND rxa.Ricetta_Cod=ro.Ricetta_Cod AND rxa.Ricetta_Operazione_Cod=ro.Ricetta_Operazione_Cod UNION SELECT TOP 1 1 FROM Ricette_Operazioni ro2 WHERE ro2.Ricetta_SuperUser=ro.Ricetta_SuperUser AND ro2.Ricetta_Operazione_Cod_RIF=ro.Ricetta_Operazione_Cod), 0) AS in_uso,")
            StrSQL.AppendLine(" ISNULL(ro.W_Anagrafica_Stati_Cod, 0) AS WAnagraficaStati_Cod,")
            StrSQL.AppendLine(" ISNULL(was.WAnagraficaStati_Des, '') AS WAnagraficaStati_Des,")
            StrSQL.AppendLine(" ISNULL(was.Colore, '') AS WAnagraficaStati_Colore,")
            StrSQL.AppendLine(" ISNULL(ro.APP_Ricetta_Operazione_ID, '') AS APP_Ricetta_Operazione_ID,")
            StrSQL.AppendLine(" CASE WHEN ISNULL(ro.APP_Ricetta_Operazione_ID, '') = '' THEN 'PC' ELSE 'APP' END AS Origine,")
            StrSQL.AppendLine(" ISNULL(codan.descrizione, '') AS DestinazioneTerreniNudi_Des, ")

            'Per Sup_Trattata
            StrSQL.AppendLine(" ISNULL(rdest.ID_REG, 0) AS ID_REG_dest, ")
            StrSQL.AppendLine(" ISNULL(ri.sup_imp, 0) AS SUP_APP, ")
            StrSQL.AppendLine(" ISNULL(rdest.Qta2, 0) AS Sup_Trattata, ")

            StrSQL.AppendLine(" r.blocco_flag, ")

            StrSQL.AppendLine(" ISNULL(rtecn.n, 0) AS N, ISNULL(rtecn.Efficienza, 0) AS Efficienza, ")
            StrSQL.AppendLine(" ISNULL(udm.udm_sim, '') AS udm_sim, isnull(ro.extra_int,0) as EpocaCod,")
            StrSQL.AppendLine(" rd.qta,rd.Extra_Int,rd.Udm_Cod,rd.Qta_Extra_Totale ")

            StrSQL.AppendLine(" FROM Ricette_Operazioni ro ")
            StrSQL.AppendLine(" INNER JOIN Ricette r ")
            StrSQL.AppendLine(" ON ro.ricetta_superuser = r.Ricetta_SuperUser And ro.Ricetta_Cod = r.Ricetta_Cod ")
            StrSQL.AppendLine(" INNER JOIN Ricette_dettagli rd ")
            StrSQL.AppendLine(" ON ro.Ricetta_SuperUser = rd.Ricetta_SuperUser AND ro.Ricetta_Operazione_Cod = rd.Ricetta_Operazione_Cod ")
            StrSQL.AppendLine(" LEFT JOIN Ricette_Dettaglio_Tecnico rtecn ")
            StrSQL.AppendLine(" ON rtecn.Ricetta_SuperUser = rd.Ricetta_SuperUser AND rtecn.Ricetta_Operazione_Cod = rd.Ricetta_Operazione_Cod AND rtecn.Ricetta_Dettaglio_Cod = rd.Ricetta_Dettaglio_Cod ")
            StrSQL.AppendLine(" LEFT JOIN Ricette_Destinazioni rdest ")
            StrSQL.AppendLine(" ON rdest.Ricetta_SuperUser = rd.Ricetta_SuperUser AND rdest.Ricetta_Operazione_Cod = rd.Ricetta_Operazione_Cod AND rdest.Ricetta_Dettaglio_Cod = rd.Ricetta_Dettaglio_Cod ")
            StrSQL.AppendLine(" LEFT JOIN Appezzamento a ")
            StrSQL.AppendLine(" ON rdest.Piva = a.Piva AND rdest.sa_cod = a.sa_cod AND rdest.Appezza = a.Appezza ")
            StrSQL.AppendLine(" LEFT JOIN reg_impianti ri ")
            StrSQL.AppendLine(" ON rdest.Piva = ri.Piva AND rdest.sa_cod = ri.sa_cod AND rdest.Appezza = ri.Appezza AND rdest.id_reg = ri.id_reg")
            StrSQL.AppendLine(" LEFT JOIN Cultivar c_op ")
            StrSQL.AppendLine(" ON ri.cul_Cod=c_op.Cul_Cod ")
            StrSQL.AppendLine(" LEFT JOIN SpecieVegetali sv_op ")
            StrSQL.AppendLine(" ON c_op.Veg_Cod=sv_op.Veg_Cod ")
            StrSQL.AppendLine(" LEFT JOIN unitamisura udm ")
            StrSQL.AppendLine(" ON udm.udm_cod=rd.extra_int ")
            StrSQL.AppendLine(" LEFT JOIN Fertilizzanti ")
            StrSQL.AppendLine(" ON rd.Pro_Cod = Fertilizzanti.Fer_Cod ")
            StrSQL.AppendLine(" LEFT JOIN SpecieVegetali sv_r ")
            StrSQL.AppendLine(" ON r.Veg_Cod=sv_r.Veg_Cod ")
            StrSQL.AppendLine(" INNER JOIN Operazioni o ")
            StrSQL.AppendLine(" ON ro.Lav_Cod=o.LAV_COD ")
            StrSQL.AppendLine(" LEFT JOIN Centri_Aziendali ca ")
            StrSQL.AppendLine(" ON ca.piva=r.piva AND ca.sa_cod=r.Sa_Cod ")
            StrSQL.AppendLine(" LEFT JOIN WAnagraficaStati was ")
            StrSQL.AppendLine(" ON ro.W_Anagrafica_Stati_Cod=was.WAnagraficaStati_Cod ")

            '(26/10/2018 MarcoG)  aggiunte le diciture per le destinazioni dei terreni nudi
            StrSQL.AppendLine(" LEFT JOIN Reg_Impianti_Codici ric ")
            StrSQL.AppendLine(" ON ri.piva = ric.piva AND ri.SA_COD = ric.sa_cod AND ri.APPEZZA = ric.appezza AND ri.ID_REG = ric.Id_Reg AND ri.Cul_Cod=0 AND ric.id_cod BETWEEN 3000 And 3999 ")
            StrSQL.AppendLine(" LEFT JOIN Codici_Anagrafe codan ")
            StrSQL.AppendLine(" ON codan.codice = ric.id_cod ")

            StrSQL.AppendLine(" WHERE ro.Ricetta_SuperUser = " & Agro_SQL_SaveText_NULL(objParametri.PivaSuperUser))
            StrSQL.AppendLine(" And ro.Validita_inizio <= " & Agro_SQL_SaveDateTime_NULL(Validita_Fine))
            StrSQL.AppendLine(" And ro.Validita_inizio >= " & Agro_SQL_SaveDateTime_NULL(Validita_Inizio))

            StrSQL.AppendLine(" AND rd.cau_mov = '" & Agro_SQL_SaveText(CAU_LAVORAZIONE) & "'")

            '(15/07/2020 fede) commentato per conteggiare anche gli altri tipi di ricetta
            'StrSQL.AppendLine(" AND r.Tipo_Ricetta = " & Agro_SQL_SaveNum(enum_TipoRicetta.PianoDistribuzionePua))


            If Piva <> "" Then
                If MostraPubblici Then
                    StrSQL.AppendLine(" And (r.Piva = '' OR r.Piva = " & Agro_SQL_SaveText_NULL(Piva) & ")")
                Else
                    StrSQL.AppendLine(" AND r.Piva = " & Agro_SQL_SaveText_NULL(Piva))
                End If
            Else
                If MostraPubblici Then
                    StrSQL.AppendLine(" AND r.Piva = '' ")
                End If
            End If

            If Sa_Cod <> 0 Then
                If MostraPubblici Then
                    StrSQL.AppendLine(" AND (r.Sa_Cod = 0 OR r.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & ")")
                Else
                    StrSQL.AppendLine(" AND r.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod))
                End If
            End If

            '(15/07/2020 fede) commentato per conteggiare anche gli altri tipi di ricetta
            'If Pua_Cod <> 0 Then
            '    StrSQL.AppendLine(" AND r.Programmazione_Cod = " & Agro_SQL_SaveNum(Pua_Cod))
            'End If



            If Veg_Cod <> 0 Then
                StrSQL.AppendLine(" AND sv_op.Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod))
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.AppendLine(" AND r.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.AppendLine(" AND r.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine(" ORDER BY ro.Validita_Inizio desc, r.Data_Creazione desc ")
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

    Public Function Leggi_MaxData(
                            ByVal Ricetta_Cod As Int32,
                            ByVal xFiltroAggiuntivo As String,
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Date

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.Ricette_Read.Leggi_MaxData()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim Data As Date = AGRODATAINIZIO

        Try


            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT MAX(Ricette_Operazioni.Validita_Inizio) AS Validita_Inizio ")
            StrSQL.AppendLine(" FROM  Ricette_Operazioni ")
            StrSQL.AppendLine(" WHERE Ricette_Operazioni.Ricetta_Cod = " & Agro_SQL_SaveNum(Ricetta_Cod) & " ")

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            If DT IsNot Nothing AndAlso DT.Rows.Count > 0 Then
                If Not IsDBNull(DT.Rows(0).Item("Validita_Inizio")) Then
                    Data = DT.Rows(0).Item("Validita_Inizio")
                End If
            End If


        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return Data

    End Function

    Public Function Leggi_MinData(ByVal Ricetta_Cod As Int32,
                                  ByVal xFiltroAggiuntivo As String,
                                  ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                  ) As Date

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.Ricette_Read.Leggi_MinData()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim Data As Date = AGRODATAFINE

        Try


            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT MIN(Ricette_Operazioni.Validita_Inizio) AS Validita_Inizio ")
            StrSQL.AppendLine(" FROM  Ricette_Operazioni ")
            StrSQL.AppendLine(" WHERE Ricette_Operazioni.Ricetta_Cod = " & Agro_SQL_SaveNum(Ricetta_Cod) & " ")

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            If DT IsNot Nothing AndAlso DT.Rows.Count > 0 Then
                If Not IsDBNull(DT.Rows(0).Item("Validita_Inizio")) Then
                    Data = DT.Rows(0).Item("Validita_Inizio")
                End If
            End If


        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return Data

    End Function


    Public Function Leggi_RicettaCod_PianoDistribuzionePUA(
                        ByVal Piva As String,
                        ByVal Sa_Cod As Int32,
                        ByVal Validita_Inizio As Date,
                        ByVal Validita_Fine As Date,
                        ByVal xFiltroAggiuntivo As String,
                        ByVal xOrderBy As String,
                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                        ) As Int32

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.Ricette_R.Leggi_RicettaCod_PianoDistribuzionePUA()"

        Dim MessaggioErrore As String = ""
        Dim DT As DataTable

        Dim Ricetta_Cod As Integer = 0

        Try

            DT = Leggi_Ricette_con_PianoDistribuzionePUA(Piva, Sa_Cod, 0, Validita_Inizio, Validita_Fine, xFiltroAggiuntivo, xOrderBy, objParametri)

            If DT IsNot Nothing AndAlso DT.Rows.Count > 0 Then
                Ricetta_Cod = DT.Rows(0).Item("ricetta_cod")
            End If

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return Ricetta_Cod

    End Function


    Public Function Leggi_Ricette_con_PianoDistribuzionePUA(
                       ByVal Piva As String,
                       ByVal Sa_Cod As Integer,
                       ByVal Ricetta_Cod As Integer,
                       ByVal Validita_Inizio As Date,
                       ByVal Validita_Fine As Date,
                       ByVal xFiltroAggiuntivo As String,
                       ByVal xOrderBy As String,
                       ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                       ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.Ricette_R.Leggi_Ricette_con_PianoDistribuzionePUA()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try


            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT * ")
            StrSQL.AppendLine(" FROM  Ricette r  ")
            StrSQL.AppendLine(" inner join pua_testata p on r.Programmazione_Cod = p.pua_cod ")
            StrSQL.AppendLine(" and r.Ricetta_SuperUser = p.Piva_SuperUser ")
            StrSQL.AppendLine(" and r.piva = p.piva ")
            StrSQL.AppendLine(" WHERE r.Validita_inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
            StrSQL.AppendLine(" AND   r.Validita_Fine >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")
            StrSQL.AppendLine(" AND   p.Validita_inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
            StrSQL.AppendLine(" AND   p.Validita_Fine >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")

            StrSQL.AppendLine(" AND r.Tipo_Ricetta = " & Agro_SQL_SaveNum(enum_TipoRicetta.PianoDistribuzionePua) & " ")

            If objParametri.PivaSuperUser <> "" Then
                StrSQL.AppendLine(" AND r.Ricetta_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'   ")
            End If

            If Piva <> "" Then
                StrSQL.AppendLine(" AND r.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
            End If

            If Sa_Cod <> 0 Then
                StrSQL.AppendLine(" AND r.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
            End If

            If Ricetta_Cod <> 0 Then
                StrSQL.AppendLine(" AND r.Ricetta_Cod = " & Agro_SQL_SaveNum(Ricetta_Cod) & "   ")
            End If


            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.AppendLine(" AND   r.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.AppendLine(" AND   r.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine(" ORDER BY r.Ricetta_SuperUser, r.Ricetta_Cod desc ")
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function


End Class




'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################




Public Class Ricette_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function RimuoviDatiDaTabelleApp(ByVal unid As String,
                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim nomeRoutine = "AgronicaCoreContabDAL.Ricette_W.RimuoviDatiDaTabelleApp()"
        Dim MessaggioErrore As String = ""

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Dim result = False

        Try

            Using scope As New TransactionScope()

                Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

                    GiasContext.APP_Ricette.RemoveRange(GiasContext.APP_Ricette.Where(Function(x) x.ID.Contains(unid)))
                    GiasContext.APP_Ricette_Operazioni.RemoveRange(GiasContext.APP_Ricette_Operazioni.Where(Function(x) x.ID.Contains(unid)))
                    GiasContext.APP_Ricette_Dettagli.RemoveRange(GiasContext.APP_Ricette_Dettagli.Where(Function(x) x.ID.Contains(unid)))
                    GiasContext.APP_Ricette_Dettaglio_Tecnico.RemoveRange(GiasContext.APP_Ricette_Dettaglio_Tecnico.Where(Function(x) x.ID.Contains(unid)))
                    GiasContext.APP_Ricette_Destinazioni.RemoveRange(GiasContext.APP_Ricette_Destinazioni.Where(Function(x) x.ID.Contains(unid)))
                    GiasContext.APP_CDG_Generale.RemoveRange(GiasContext.APP_CDG_Generale.Where(Function(x) x.ID.Contains(unid)))
                    GiasContext.APP_CDG_Movimenti.RemoveRange(GiasContext.APP_CDG_Movimenti.Where(Function(x) x.ID.Contains(unid)))
                    GiasContext.APP_Riferimenti_Interventi_Cdg.RemoveRange(GiasContext.APP_Riferimenti_Interventi_Cdg.Where(Function(x) x.ID.Contains(unid)))
                    GiasContext.SaveChanges()

                    ' COMIT Effettivo
                    scope.Complete()

                End Using

            End Using

            result = True

        Catch ex As Exception

            result = False
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, MessaggioErrore)

        End Try

        Return result

    End Function

    Public Function Aggiorna_RicetteAPP(
                ByVal EFArrayToInsert As ArrayList,
                ByVal EFArrayToUpdate As ArrayList,
                ByVal EFArrayToDelete As ArrayList,
                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                Optional ByVal unid As String = ""
                ) As String

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.Ricette_W.Aggiorna_RicetteAPP()"
        Dim MessaggioErrore As String = ""

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Try

            Using scope As New TransactionScope()

                Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

                    If Not String.IsNullOrEmpty(unid) Then
                        GiasContext.APP_Ricette.RemoveRange(GiasContext.APP_Ricette.Where(Function(x) x.ID.Contains(unid)))
                        GiasContext.APP_Ricette_Operazioni.RemoveRange(GiasContext.APP_Ricette_Operazioni.Where(Function(x) x.ID.Contains(unid)))
                        GiasContext.APP_Ricette_Dettagli.RemoveRange(GiasContext.APP_Ricette_Dettagli.Where(Function(x) x.ID.Contains(unid)))
                        GiasContext.APP_Ricette_Dettaglio_Tecnico.RemoveRange(GiasContext.APP_Ricette_Dettaglio_Tecnico.Where(Function(x) x.ID.Contains(unid)))
                        GiasContext.APP_Ricette_Destinazioni.RemoveRange(GiasContext.APP_Ricette_Destinazioni.Where(Function(x) x.ID.Contains(unid)))
                        GiasContext.APP_CDG_Generale.RemoveRange(GiasContext.APP_CDG_Generale.Where(Function(x) x.ID.Contains(unid)))
                        GiasContext.APP_CDG_Movimenti.RemoveRange(GiasContext.APP_CDG_Movimenti.Where(Function(x) x.ID.Contains(unid)))
                        GiasContext.APP_Riferimenti_Interventi_Cdg.RemoveRange(GiasContext.APP_Riferimenti_Interventi_Cdg.Where(Function(x) x.ID.Contains(unid)))
                        GiasContext.SaveChanges()
                    End If

                    For Each curOggetto In EFArrayToDelete
                        GiasContext.Entry(curOggetto).State = EntityState.Deleted
                        GiasContext.SaveChanges()
                    Next

                    For Each curOggetto In EFArrayToInsert
                        GiasContext.Entry(curOggetto).State = EntityState.Added
                        GiasContext.SaveChanges()
                    Next

                    For Each curOggetto In EFArrayToUpdate
                        GiasContext.Entry(curOggetto).State = EntityState.Modified
                        GiasContext.SaveChanges()
                    Next

                    ' COMIT Effettivo
                    scope.Complete()

                End Using

            End Using

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore, ex)

        End Try

        Return MessaggioErrore

    End Function

    Public Function Aggiorna_RicettePerAPP(
                ByVal EFArrayToInsert As ArrayList,
                ByVal EFArrayToUpdate As ArrayList,
                ByVal EFArrayToDelete As ArrayList,
                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                ) As String

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        Dim retries As Integer = 3
        Dim success As Boolean = True

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.Ricette_W.Aggiorna_RicettePerAPP()"
        Dim MessaggioErrore As String = ""

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Dim ListaOperazionePerAvanzamentoStato As New List(Of Integer)

        Try

            Using scope As New TransactionScope()

                Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

                    For Each curOggetto In EFArrayToInsert

                        success = False
                        For i As Integer = 0 To retries - 1

                            Try

                                'metto in lista per filtro avanzamento stato
                                If TypeOf (curOggetto) Is AgronicaCoreEntityFramework_POCO.APP_Ricette_Operazioni AndAlso CType(curOggetto, AgronicaCoreEntityFramework_POCO.APP_Ricette_Operazioni).Ricetta_Operazione_Cod_RIF <> 0 Then

                                    ListaOperazionePerAvanzamentoStato.Add(CType(curOggetto, AgronicaCoreEntityFramework_POCO.APP_Ricette_Operazioni).Ricetta_Operazione_Cod_RIF)

                                End If

                                GiasContext.Entry(curOggetto).State = EntityState.Added
                                'GiasContext.AddObject(curOggetto.GetType.ToString.Replace("AgronicaCoreEntityFramework_POCO.", ""), curOggetto)
                                GiasContext.SaveChanges()
                                success = True

                                Exit For
                            Catch ex As Exception
                                Threading.Thread.Sleep(500) ' 500 milliseconds = 0.5 seconds
                            End Try
                        Next
                        ' Al primo errore evito di continuare le modifiche
                        If Not success Then
                            MessaggioErrore = "Non sono riuscito ad aggiornare i dati dopo " & retries & " tentativi."
                            Exit For
                        End If
                    Next

                    If success Then
                        For Each listFattVar In EFArrayToUpdate
                            'GiasContext.Attach(listFattVar)
                            'GiasContext.ObjectStateManager.ChangeObjectState(listFattVar, EntityState.Modified)
                            GiasContext.Entry(listFattVar).State = EntityState.Modified
                            GiasContext.SaveChanges()
                        Next

                        For Each listFattVar In EFArrayToDelete
                            'GiasContext.AttachTo(listFattVar.GetType.ToString.Replace("AgronicaCoreEntityFramework_POCO.", ""), listFattVar)
                            'GiasContext.DeleteObject(listFattVar)
                            GiasContext.Entry(listFattVar).State = EntityState.Deleted
                            GiasContext.SaveChanges()
                        Next

                        'If ListaOperazionePerAvanzamentoStato.Count > 0 Then
                        '    Dim filtroOperazioniPerAvanzamento As String = " opWeb.Ricetta_Operazione_Cod in (" & String.Join(",", ListaOperazionePerAvanzamentoStato.ToArray()) & " )"

                        '    Dim xAllineamentoStato As New Ricette_Operazioni_W
                        '    xAllineamentoStato.ImpostaStatoEseguitoPerGias_APP(filtroOperazioniPerAvanzamento, objParametri, GiasContext)
                        'End If

                        ' COMIT Effettivo
                        scope.Complete()
                    End If

                End Using
            End Using


        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore, ex)

        End Try

        Return MessaggioErrore

    End Function


    '##############################################################################################
    Public Function Scrivi(ByVal Ricetta_Cod As Int32,
                            ByVal Ricetta_Numero As String,
                            ByVal Piva As String,
                            ByVal Sa_Cod As Int32,
                            ByVal Tipo_Ricetta As Int32,
                            ByVal Ricetta_Des As String,
                            ByVal Ricetta_Des_Long As String,
                            ByVal Veg_Cod As Int32,
                            ByVal Note As String,
                                ByVal Validita_Inizio As Date,
                                ByVal Validita_Fine As Date,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                    ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.Ricette_Write.Scrivi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.AppendLine(" INSERT INTO Ricette ")
            StrSQL.AppendLine("         ( ")
            StrSQL.AppendLine("          Ricetta_SuperUser,      Ricetta_Cod,       Ricetta_Des,     Ricetta_Des_Long,   ")
            StrSQL.AppendLine("          Piva,                   Sa_Cod,        Tipo_Ricetta,                 ")
            StrSQL.AppendLine("          Veg_Cod,                Note,    Ricetta_Numero,      DataLock,      ")

            StrSQL.AppendLine("          Inviato,            DataInvio, ")
            StrSQL.AppendLine("          Data_Creazione,     Data_Modifica, ")
            StrSQL.AppendLine("          UserName_Creazione, UserName_Modifica, ")
            StrSQL.AppendLine("          Validita_Inizio,    Validita_Fine ")
            StrSQL.AppendLine("         ) ")

            StrSQL.AppendLine(" VALUES ( ")
            StrSQL.AppendLine("          '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Ricetta_Cod))
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(Ricetta_Des) & "' ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(Ricetta_Des_Long) & "' ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(Piva) & "' ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Tipo_Ricetta) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Veg_Cod) & "  ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(Note) & "'  ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(Ricetta_Numero) & "'  ")
            StrSQL.AppendLine("         , 0 ")

            StrSQL.AppendLine("         , 0  ")
            StrSQL.AppendLine("         , Null  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDateTime(Date.Now) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDateTime(Date.Now) & "  ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")

            StrSQL.AppendLine(") ")


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
    Public Function Scrivi(ByVal Ricetta_Cod As Int32,
                            ByVal Ricetta_Numero As String,
                            ByVal Piva As String,
                            ByVal Sa_Cod As Int32,
                            ByVal Tipo_Ricetta As Int32,
                            ByVal Ricetta_Des As String,
                            ByVal Ricetta_Des_Long As String,
                            ByVal Veg_Cod As Int32,
                            ByVal Note As String,
                            ByVal Programmazione_Cod As Integer,
                            ByVal Validita_Inizio As Date,
                            ByVal Validita_Fine As Date,
                            ByVal Imputazione_Cod As Int32,
                            ByVal Imputazione_Fase_Cod As Int32,
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                , Optional ByVal Data_creazione As Date = #2/1/1900# _
                                , Optional ByVal Data_modifica As Date = #2/1/1900# _
                                , Optional ByVal username_creazione As String = "" _
                                , Optional ByVal username_modifica As String = "" _
                                , Optional ByVal Origine As String = ""
                            ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.Ricette_Write.Scrivi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.AppendLine(" INSERT INTO Ricette ")
            StrSQL.AppendLine("         ( ")
            StrSQL.AppendLine("          Ricetta_SuperUser,      Ricetta_Cod,       Ricetta_Des,            Ricetta_Des_Long,   ")
            StrSQL.AppendLine("          Piva,                   Sa_Cod,            Tipo_Ricetta,                 ")
            StrSQL.AppendLine("          Veg_Cod,                Note,              Programmazione_Cod,   Ricetta_Numero,  DataLock,      ")

            StrSQL.AppendLine("          Inviato,            DataInvio, ")
            StrSQL.AppendLine("          Data_Creazione,     Data_Modifica, ")
            StrSQL.AppendLine("          UserName_Creazione, UserName_Modifica, ")
            StrSQL.AppendLine("          Validita_Inizio,    Validita_Fine, ")
            StrSQL.AppendLine("          Imputazione_Cod,    Imputazione_Fase_Cod, Origine ")
            StrSQL.AppendLine("         ) ")

            StrSQL.AppendLine(" VALUES ( ")
            StrSQL.AppendLine("          '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Ricetta_Cod))
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(Ricetta_Des) & "' ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(Ricetta_Des_Long) & "' ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(Piva) & "' ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Tipo_Ricetta) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Veg_Cod) & "  ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(Note) & "'  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Programmazione_Cod) & "  ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(Ricetta_Numero) & "'  ")
            StrSQL.AppendLine("         , 0 ")

            StrSQL.AppendLine("         , 0  ")
            StrSQL.AppendLine("         , Null  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDateTime(DateTime.Now) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDateTime(DateTime.Now) & "  ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Imputazione_Cod) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Imputazione_Fase_Cod) & "  ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(Origine) & "'  ")

            StrSQL.AppendLine(") ")


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
    Public Function Modifica(ByVal Ricetta_Cod As Int32,
                            ByVal Ricetta_Numero As String,
                            ByVal Piva As String,
                            ByVal Sa_Cod As Int32,
                            ByVal Tipo_Ricetta As Int32,
                            ByVal Ricetta_Des As String,
                            ByVal Ricetta_Des_Long As String,
                            ByVal Programmazione_Cod As Integer,
                            ByVal Veg_Cod As Int32,
                            ByVal Note As String,
                            ByVal Validita_Inizio As Date,
                            ByVal Validita_Fine As Date,
                            ByVal Imputazione_Cod As Int32,
                            ByVal Imputazione_Fase_Cod As Int32,
                            ByVal xFiltroAggiuntivo As String,
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                            Optional Origine As String = ""
                            ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.Ricette_Write.Modifica()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        '------------------------------

        Try


            If objParametri.PivaSuperUser = "" Then
                Throw New Exception("Parametro non corretto nella query (Ricetta_SuperUser obbligatorio)")
            End If

            If Ricetta_Cod = 0 Then
                Throw New Exception("Parametro non corretto nella query (Ricetta_Cod = 0)")
            End If

            '---------------------------------------------

            StrSQL.Length = 0

            StrSQL.AppendLine(" UPDATE Ricette SET ")
            StrSQL.AppendLine("    Ricetta_Des          =  '" & Agro_SQL_SaveText(Ricetta_Des) & "'")
            StrSQL.AppendLine("   ,Ricetta_Des_Long     =  '" & Agro_SQL_SaveText(Ricetta_Des_Long) & "'")
            StrSQL.AppendLine("   ,Piva                 =  '" & Agro_SQL_SaveText(Piva) & "'")
            StrSQL.AppendLine("   ,Sa_Cod               =  " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            StrSQL.AppendLine("   ,Tipo_Ricetta         =  " & Agro_SQL_SaveNum(Tipo_Ricetta) & "  ")
            StrSQL.AppendLine("   ,Veg_Cod              =  " & Agro_SQL_SaveNum(Veg_Cod) & "  ")
            StrSQL.AppendLine("   ,Note                 =  '" & Agro_SQL_SaveText(Note) & "'  ")
            StrSQL.AppendLine("   ,Ricetta_Numero       =  '" & Agro_SQL_SaveText(Ricetta_Numero) & "'  ")
            StrSQL.AppendLine("   ,Programmazione_Cod   =  " & Agro_SQL_SaveNum(Programmazione_Cod) & "  ")

            StrSQL.AppendLine("   ,Inviato              =  0 ")
            StrSQL.AppendLine("   ,DataInvio            =  Null ")
            StrSQL.AppendLine("   ,Data_Modifica        =  " & Agro_SQL_SaveDateTime(Date.Now))
            StrSQL.AppendLine("   ,UserName_Modifica    = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.AppendLine("   ,Validita_Inizio      =  " & Agro_SQL_SaveDate(Validita_Inizio))
            StrSQL.AppendLine("   ,Validita_Fine        =  " & Agro_SQL_SaveDate(Validita_Fine))
            StrSQL.AppendLine("   ,Imputazione_Cod      =  " & Agro_SQL_SaveNum(Imputazione_Cod) & "  ")
            StrSQL.AppendLine("   ,Imputazione_Fase_Cod =  " & Agro_SQL_SaveNum(Imputazione_Fase_Cod) & "  ")

            If Origine <> "" Then
                StrSQL.AppendLine("   ,Origine    = '" & Agro_SQL_SaveText(Origine) & "'")
            End If


            StrSQL.AppendLine(" WHERE  Ricetta_SuperUser  = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'   ")
            StrSQL.AppendLine(" AND Ricetta_Cod  = " & Agro_SQL_SaveNum(Ricetta_Cod) & "   ")

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
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


    '##############################################################################################
    Public Function Modifica(ByVal Ricetta_Cod As Int32,
                            ByVal Ricetta_Numero As String,
                            ByVal Piva As String,
                            ByVal Sa_Cod As Int32,
                            ByVal Tipo_Ricetta As Int32,
                            ByVal Ricetta_Des As String,
                            ByVal Ricetta_Des_Long As String,
                            ByVal Veg_Cod As Int32,
                            ByVal Note As String,
                            ByVal Programmazione_Cod As Integer,
                            ByVal Validita_Inizio As Date,
                            ByVal Validita_Fine As Date,
                            ByVal Imputazione_Cod As Int32,
                            ByVal Imputazione_Fase_Cod As Int32,
                            ByVal xFiltroAggiuntivo As String,
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                            ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.Ricette_Write.Modifica()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        '------------------------------

        Try


            If objParametri.PivaSuperUser = "" Then
                Throw New Exception("Parametro non corretto nella query (Ricetta_SuperUser obbligatorio)")
            End If

            '---------------------------------------------

            StrSQL.Length = 0

            StrSQL.AppendLine(" UPDATE Ricette SET ")
            StrSQL.AppendLine("    Ricetta_Des          =  '" & Agro_SQL_SaveText(Ricetta_Des) & "'")
            StrSQL.AppendLine("   ,Ricetta_Des_Long     =  '" & Agro_SQL_SaveText(Ricetta_Des_Long) & "'")
            StrSQL.AppendLine("   ,Piva                 =  '" & Agro_SQL_SaveText(Piva) & "'")
            StrSQL.AppendLine("   ,Sa_Cod               =  " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            StrSQL.AppendLine("   ,Tipo_Ricetta         =  " & Agro_SQL_SaveNum(Tipo_Ricetta) & "  ")
            StrSQL.AppendLine("   ,Veg_Cod              =  " & Agro_SQL_SaveNum(Veg_Cod) & "  ")
            StrSQL.AppendLine("   ,Note                 =  '" & Agro_SQL_SaveText(Note) & "'  ")
            StrSQL.AppendLine("   ,Programmazione_Cod   =   " & Agro_SQL_SaveNum(Programmazione_Cod) & "  ")
            StrSQL.AppendLine("   ,Ricetta_Numero       =  '" & Agro_SQL_SaveText(Ricetta_Numero) & "'  ")

            StrSQL.AppendLine("   ,Inviato              =  0 ")
            StrSQL.AppendLine("   ,DataInvio            =  Null ")
            StrSQL.AppendLine("   ,Data_Modifica        =  " & Agro_SQL_SaveDateTime(Date.Now))
            StrSQL.AppendLine("   ,UserName_Modifica    = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.AppendLine("   ,Validita_Inizio      =  " & Agro_SQL_SaveDate(Validita_Inizio))
            StrSQL.AppendLine("   ,Validita_Fine        =  " & Agro_SQL_SaveDate(Validita_Fine))
            StrSQL.AppendLine("   ,Imputazione_Cod      =  " & Agro_SQL_SaveNum(Imputazione_Cod) & "  ")
            StrSQL.AppendLine("   ,Imputazione_Fase_Cod =  " & Agro_SQL_SaveNum(Imputazione_Fase_Cod) & "  ")

            StrSQL.AppendLine(" WHERE  Ricetta_SuperUser  = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'   ")


            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
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

    '##############################################################################################
    Public Function Cancella(ByVal Ricetta_Cod As Int32,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                             ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.Ricette_Write.Cancella()"

        '====================================================================================
        'Parametri opzionali :
        '   Ricetta_SuperUser = ""
        '   Ricetta_Cod = 0
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            'If Piva = "" Then
            '    Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
            'End If

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = AgronicaCoreDataProvider.AgronicaCoreParametri.enumCancellazioneLogica.CancellazioneLogica Then

                StrSQL.Length = 0
                StrSQL.AppendLine(" UPDATE Ricette ")
                StrSQL.AppendLine(" SET ")
                StrSQL.AppendLine("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.AppendLine("      ,Inviato = -1 ")
                StrSQL.AppendLine(" WHERE  Inviato >= 0 ")

            Else
                StrSQL.Length = 0
                StrSQL.AppendLine(" DELETE ")
                StrSQL.AppendLine(" FROM Ricette ")
                StrSQL.AppendLine(" WHERE  1=1 ")

            End If

            If objParametri.PivaSuperUser <> "" Then
                StrSQL.AppendLine(" AND Ricette.Ricetta_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'   ")
            End If

            If Ricetta_Cod <> 0 Then
                StrSQL.AppendLine(" AND Ricette.Ricetta_Cod = " & Agro_SQL_SaveNum(Ricetta_Cod) & "   ")
            End If


            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
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

    Public Function RicettePC_Blocca(ByVal Piva As String,
                               ByVal data_inizio As DateTime,
                               ByVal data_fine As DateTime,
                               ByVal bloccaSoloSeNonGiaBloccati As Boolean,
                               ByVal xFiltroAggiuntivo As String,
                               ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                               ) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Ricette_W.RicettePC_Blocca()"

        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        '------------------------------

        Try

            If Piva = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
            End If

            '---------------------------------------------
            strSql.Length = 0

            strSql.AppendLine(" UPDATE r ")
            strSql.AppendLine(" SET ")
            strSql.AppendLine("     r.Blocco_Flag         =  1 ")
            strSql.AppendLine("    ,r.Blocco_Username     = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            strSql.AppendLine("    ,r.Blocco_Data         =  " & Agro_SQL_SaveDate(Now))

            strSql.AppendLine(" FROM Ricette r ")
            strSql.AppendLine(" INNER JOIN PianoConcimazione_Testata pt ON pt.PC_Testata_Cod=r.Programmazione_Cod ")
            strSql.AppendLine(" INNER JOIN PianoConcimazione_Dettagli pd ON pt.PC_Testata_Cod=pd.PC_Testata_Cod ")

            strSql.AppendLine(" WHERE pd.PC_Dettagli_PIVA = '" & Agro_SQL_SaveText(Trim(Piva)) & "' ")

            strSql.AppendLine(" AND pt.validita_fine >= " & Agro_SQL_SaveDate(data_inizio))
            strSql.AppendLine(" AND pt.validita_inizio <= " & Agro_SQL_SaveDate(data_fine))

            strSql.AppendLine(" AND r.Tipo_Ricetta = " & enum_TipoRicetta.PianoDistribuzioneConcimi)

            If bloccaSoloSeNonGiaBloccati Then
                strSql.AppendLine(" AND r.Blocco_Flag = 0 ")
            End If

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


    Public Function RicettePC_Sblocca(ByVal Piva As String,
                                   ByVal data_inizio As DateTime,
                                   ByVal data_fine As DateTime,
                                   ByVal xFiltroAggiuntivo As String,
                                   ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                   ) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Ricette_W.RicettePC_Sblocca()"

        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If Piva = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
            End If

            '---------------------------------------------
            strSql.Length = 0

            strSql.AppendLine(" UPDATE r ")
            strSql.AppendLine(" SET ")
            strSql.AppendLine("     r.Blocco_Flag         =  0 ")
            strSql.AppendLine("    ,r.Blocco_Username     = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            strSql.AppendLine("    ,r.Blocco_Data         =  " & Agro_SQL_SaveDate(Now))

            strSql.AppendLine(" FROM Ricette r ")
            strSql.AppendLine(" INNER JOIN PianoConcimazione_Testata pt ON pt.PC_Testata_Cod=r.Programmazione_Cod ")
            strSql.AppendLine(" INNER JOIN PianoConcimazione_Dettagli pd ON pt.PC_Testata_Cod=pd.PC_Testata_Cod ")

            strSql.AppendLine(" WHERE pd.PC_Dettagli_PIVA = '" & Agro_SQL_SaveText(Trim(Piva)) & "' ")

            strSql.AppendLine(" AND pt.validita_fine >= " & Agro_SQL_SaveDate(data_inizio))
            strSql.AppendLine(" AND pt.validita_inizio <= " & Agro_SQL_SaveDate(data_fine))

            strSql.AppendLine(" AND r.Tipo_Ricetta = " & enum_TipoRicetta.PianoDistribuzioneConcimi)

            strSql.AppendLine(" AND r.Blocco_Flag = 1 ")

            '----------------------------------------------------------------------
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

    Public Function RicettePUA_Blocca(ByVal Piva As String,
                            ByVal data_inizio As DateTime,
                            ByVal data_fine As DateTime,
                            ByVal bloccaSoloSeNonGiaBloccati As Boolean,
                            ByVal xFiltroAggiuntivo As String,
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                            ) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Ricette_W.RicettePUA_Blocca()"

        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        '------------------------------

        Try

            If Piva = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
            End If

            '---------------------------------------------
            strSql.Length = 0

            strSql.AppendLine(" UPDATE r ")
            strSql.AppendLine(" SET ")
            strSql.AppendLine("     r.Blocco_Flag         =  1 ")
            strSql.AppendLine("    ,r.Blocco_Username     = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            strSql.AppendLine("    ,r.Blocco_Data         =  " & Agro_SQL_SaveDate(Now))

            strSql.AppendLine(" FROM Ricette r ")
            strSql.AppendLine(" INNER JOIN Pua_Testata pt ON pt.PUA_Cod=r.Programmazione_Cod ")

            strSql.AppendLine(" WHERE pt.PIVA = '" & Agro_SQL_SaveText(Trim(Piva)) & "' ")

            strSql.AppendLine(" AND pt.validita_inizio >= " & Agro_SQL_SaveDate(data_inizio))
            strSql.AppendLine(" AND pt.validita_inizio <= " & Agro_SQL_SaveDate(data_fine))

            strSql.AppendLine(" AND r.Tipo_Ricetta = " & enum_TipoRicetta.PianoDistribuzionePua)

            If bloccaSoloSeNonGiaBloccati Then
                strSql.AppendLine(" AND r.Blocco_Flag = 0 ")
            End If

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

    Public Function RicettePUA_Sblocca(ByVal Piva As String,
                                   ByVal data_inizio As DateTime,
                                   ByVal data_fine As DateTime,
                                   ByVal xFiltroAggiuntivo As String,
                                   ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                   ) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Ricette_W.RicettePUA_Sblocca()"

        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If Piva = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
            End If

            '---------------------------------------------
            strSql.Length = 0

            strSql.AppendLine(" UPDATE r ")
            strSql.AppendLine(" SET ")
            strSql.AppendLine("     r.Blocco_Flag         =  0 ")
            strSql.AppendLine("    ,r.Blocco_Username     = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            strSql.AppendLine("    ,r.Blocco_Data         =  " & Agro_SQL_SaveDate(Now))

            strSql.AppendLine(" FROM Ricette r ")
            strSql.AppendLine(" INNER JOIN Pua_Testata pt ON pt.PUA_Cod=r.Programmazione_Cod ")

            strSql.AppendLine(" WHERE pt.PIVA = '" & Agro_SQL_SaveText(Trim(Piva)) & "' ")

            strSql.AppendLine(" AND pt.validita_inizio >= " & Agro_SQL_SaveDate(data_inizio))
            strSql.AppendLine(" AND pt.validita_inizio <= " & Agro_SQL_SaveDate(data_fine))

            strSql.AppendLine(" AND r.Tipo_Ricetta = " & enum_TipoRicetta.PianoDistribuzionePua)

            strSql.AppendLine(" AND r.Blocco_Flag = 1 ")

            '----------------------------------------------------------------------
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

    Public Function AggiornaProdottiRicetteAPP(ByVal Piva As String,
                                   ByVal Elem_Cod As Integer,
                                   ByVal Descrizione As String,
                                   ByVal Codice_GIAS As Integer,
                                   ByVal xFiltroAggiuntivo As String,
                                   ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                   ) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Ricette_W.AggiornaProdottiRicetteAPP()"

        Dim messaggioErrore As String = ""
        Dim strSql As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            strSql.Length = 0

            strSql.AppendLine(" UPDATE rd ")
            strSql.AppendLine(" SET ")
            strSql.AppendLine("    rd.Pro_Cod         =  " & Agro_SQL_SaveNum(Codice_GIAS))

            strSql.AppendLine(" FROM APP_Ricette_Dettagli rd ")
            strSql.AppendLine(" INNER JOIN APP_Ricette r ON r.Ricetta_Cod=rd.Ricetta_Cod ")
            strSql.AppendLine(" AND substring(rd.id,0,charindex('|',rd.id)) = substring(r.id,0,charindex('|',r.id)) ")

            strSql.AppendLine(" WHERE 1=1 ")

            If Piva <> "" Then
                strSql.AppendLine(" AND r.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If

            strSql.AppendLine(" AND rd.Descrizione = '" & Agro_SQL_SaveText(Descrizione) & "' ")
            strSql.AppendLine(" AND rd.Elem_Cod = " & Elem_Cod)
            strSql.AppendLine(" AND rd.Codice_Extra = '' ")

            '----------------------------------------------------------------------
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

    Public Function AggiornaRicetteDaImportareAPP(ByVal Piva As String, ByVal xFiltroAggiuntivo As String, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Ricette_W.AggiornaRicetteDaImportareAPP()"

        Dim messaggioErrore As String = ""
        Dim strSql As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            strSql.Length = 0

            strSql.AppendLine(" UPDATE ro SET ro.Importato_Data = NULL ")
            strSql.AppendLine(" FROM APP_Ricette_Operazioni ro ")
            strSql.AppendLine(" INNER JOIN APP_Ricette r ON r.Ricetta_Cod=ro.Ricetta_Cod ")
            strSql.AppendLine(" AND substring(ro.id,0,charindex('|',ro.id)) = substring(r.id,0,charindex('|',r.id)) ")
            strSql.AppendLine(" WHERE Importato_Data IS NOT NULL AND Importato_Errore <> '' ")
            strSql.AppendLine(" AND ro.id IN (SELECT DISTINCT SUBSTRING(id, 0, CHARINDEX('|',id)) + '|' + CAST(Ricetta_Operazione_Cod AS varchar) ")
            strSql.AppendLine(" FROM APP_Ricette_Dettagli WHERE Pro_Cod <> 0 AND Descrizione <> '' AND Codice_Extra = '') ")

            If Piva <> "" Then
                strSql.AppendLine(" AND r.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If

            '----------------------------------------------------------------------
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

    Public Function AggiornaRicetteInviataAPP(ByVal Piva As String, ByVal Ricetta_Cod As Integer,
                                              ByVal value As Integer,
                                              ByVal xFiltroAggiuntivo As String,
                                              ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Ricette_W.AggiornaRicetteInviataAPP()"

        Dim messaggioErrore As String = ""
        Dim strSql As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            strSql.Length = 0

            ' Imposto chiave di ricerca per modificare il flag nella tabella APP_Ricette_Operazioni
            strSql.AppendLine(" DECLARE @app_ricetta_ID varchar(60) ")
            strSql.AppendLine(" SELECT @app_ricetta_ID = ro.APP_Ricetta_Operazione_ID FROM Ricette_Operazioni ro")
            strSql.AppendLine(" WHERE ro.Ricetta_Cod = " & Agro_SQL_SaveNum(Ricetta_Cod))

            If Piva <> "" Then
                strSql.AppendLine(" AND ro.Ricetta_SuperUser LIKE '" & Agro_SQL_SaveText(Piva) & "'")
            End If
            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            ' Aggiorno il valore nella prima tabella
            strSql.AppendLine(" UPDATE Ricette_Operazioni")
            strSql.AppendLine(" SET Invia_App = " & Agro_SQL_SaveNum(value) & "")
            strSql.AppendLine(" WHERE Ricetta_Cod = " & Agro_SQL_SaveNum(Ricetta_Cod))
            If Piva <> "" Then
                strSql.AppendLine(" AND Ricetta_SuperUser LIKE '" & Agro_SQL_SaveText(Piva) & "'")
            End If
            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            ' Aggiorno il flag nella seconda tabella
            strSql.AppendLine(" UPDATE APP_Ricette_Operazioni")
            strSql.AppendLine(" SET Invia_App = " & Agro_SQL_SaveNum(value) & "")
            strSql.AppendLine(" WHERE ID = @app_ricetta_ID")

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

    Public Function Update_Origine(ByVal Ricetta_Cod As Int32,
                                   ByVal Origine As String,
                                   ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                   ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.Ricette_W.Update_Origine()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        '------------------------------

        Try


            If objParametri.PivaSuperUser = "" Then
                Throw New Exception("Parametro non corretto nella query (Ricetta_SuperUser obbligatorio)")
            End If

            If Ricetta_Cod = 0 Then
                Throw New Exception("Parametro non corretto nella query (Ricetta_Cod = 0)")
            End If

            '---------------------------------------------

            StrSQL.Length = 0

            StrSQL.AppendLine(" UPDATE Ricette SET ")
            StrSQL.AppendLine("   Origine    = '" & Agro_SQL_SaveText(Origine) & "'")

            StrSQL.AppendLine(" WHERE  Ricetta_SuperUser  = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'   ")
            StrSQL.AppendLine(" AND Ricetta_Cod  = " & Agro_SQL_SaveNum(Ricetta_Cod) & "   ")

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
