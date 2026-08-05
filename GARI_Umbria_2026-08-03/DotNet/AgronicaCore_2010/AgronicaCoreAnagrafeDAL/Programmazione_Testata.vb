Imports System.Data.OleDb
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.DataProviderExtensions
Imports System.Text
Imports System.Reflection

Public Class Programmazione_Testata_R
    Inherits AgronicaCoreDataProvider.DataProvider


    '################################################################################
    'Attenzione, se modifichi la leggi completa, ricordati di verificare la chiamata
    'nella funzione Crea_GridView_Programmazioni della pagina lista planning
    Public Function Leggi(
                            ByRef MessaggioErrore As String,
                            ByVal Programmazione_Cod As Integer,
                            ByVal Piva As String,
                            ByVal Programmazione_Des As String,
                            ByVal SORT_Des1_Inizio2_Fine2 As Integer,
                                ByVal Validita_Inizio As Date,
                                ByVal Validita_Fine As Date,
                                ByVal Tipo_Pianificazione As enum_TipoPianificazione,
                                    ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile,
                                    ByVal xFiltroAggiuntivo As String,
                                    ByVal xOrderBy As String,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                    Optional ByRef NumeroValidazione As String = "",
                                    Optional ByRef DataValidazione As Date = AGRODATAINIZIO
                                    ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Programmazione_Testata_R.Leggi()"

        Dim DT As New DataTable

        Dim stb As New System.Text.StringBuilder

        Try

            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                    stb.Length = 0

                    stb.Append(" SELECT  * ")

                    stb.Append(" FROM    Programmazione_Testata (nolock) ")

                    stb.Append(" WHERE   Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
                    stb.Append(" AND     Validita_Inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
                    stb.Append(" AND     Validita_fine >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")


                    If Programmazione_Cod <> 0 Then
                        stb.Append(" AND Programmazione_Cod = " & Agro_SQL_SaveNum(Programmazione_Cod.ToString))
                    End If

                    If Piva <> "" Then
                        stb.Append(" AND Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
                    End If

                    If Programmazione_Des <> "" Then
                        stb.Append(" AND Programmazione_Des like '%" & Agro_SQL_SaveText(Programmazione_Des) & "%' ")
                    End If
                    If Tipo_Pianificazione <> enum_TipoRicetta.Non_Filtrare Then
                        stb.Append(" AND Tipo_Pianificazione = " & Agro_vb_SaveNum(Tipo_Pianificazione))
                    End If


                    If xFiltroAggiuntivo <> "" Then
                        stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo,, objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            stb.Append(" AND     Programmazione_Testata.Inviato >= 0 ")
                            stb.Append(" AND     Programmazione_Testata.Inviato >= 0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            stb.Append(" AND     Programmazione_Testata.Inviato = -1 ")
                            stb.Append(" AND     Programmazione_Testata.Inviato = -1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------

                    Select Case SORT_Des1_Inizio2_Fine2
                        Case 1
                            stb.Append(" ORDER BY Programmazione_Des ASC ")
                        Case 2
                            stb.Append(" ORDER BY Validita_Inizio ASC ")
                        Case 3
                            stb.Append(" ORDER BY Validita_fine ASC ")
                        Case Else
                            If xOrderBy <> "" Then
                                stb.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                            Else
                                stb.Append(" ORDER BY Programmazione_Des ASC ")
                            End If
                    End Select




                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta

                    stb.Length = 0

                    stb.Append(" select DISTINCT  " & vbCrLf)
                    stb.Append("    tt.* " & vbCrLf)
                    stb.Append("    , coalesce(a.Allegati_Documenti_Numero, '') as Allegati_Documenti_Numero " & vbCrLf)
                    stb.Append("    , coalesce(a.Validazione_Data, cast('01/01/1900' as date)) as Validazione_Data " & vbCrLf)
                    stb.Append("    , coalesce(a.Allegati_Documenti_NomeFile, '') as Allegati_Documenti_NomeFile " & vbCrLf)

                    stb.Append(" from Programmazione_Testata  (nolock) tt " & vbCrLf)
                    stb.Append("    OUTER APPLY ( select TOP 1  Programmazione_Cod, Allegati_Documenti_SuperUser, Allegati_Documenti_Cod   " & vbCrLf)
                    stb.Append("                from  Allegati_EntitaxDocumenti (nolock) " & vbCrLf)
                    stb.Append("                WHERE tt.Programmazione_Cod  = Programmazione_Cod  " & vbCrLf)
                    stb.Append("                AND tt.Piva_SuperUser = Allegati_Documenti_SuperUser " & vbCrLf)
                    stb.Append("                AND Allegati_EntitaxDocumenti.Programmazione_Entita_Cod = 0  ) e    " & vbCrLf)
                    stb.Append("    LEFT JOIN Allegati_Documenti (nolock) a " & vbCrLf)
                    stb.Append("        on a.Allegati_Documenti_Cod = e.Allegati_Documenti_Cod  " & vbCrLf)
                    stb.Append("        and a.Allegati_Documenti_SuperUser = e.Allegati_Documenti_SuperUser  " & vbCrLf)


                    stb.Append(" WHERE   tt.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
                    stb.Append(" AND     tt.Validita_Inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
                    stb.Append(" AND     tt.Validita_fine >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")


                    If Programmazione_Cod <> 0 Then
                        stb.Append(" AND tt.Programmazione_Cod = " & Agro_SQL_SaveNum(Programmazione_Cod.ToString))
                    End If

                    If Piva <> "" Then
                        stb.Append(" AND tt.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
                    End If

                    If Programmazione_Des <> "" Then
                        stb.Append(" AND tt.Programmazione_Des like '%" & Agro_SQL_SaveText(Programmazione_Des) & "%' ")
                    End If

                    If Tipo_Pianificazione <> enum_TipoRicetta.Non_Filtrare Then
                        stb.Append(" AND tt.Tipo_Pianificazione =" & Agro_vb_SaveNum(Tipo_Pianificazione))
                    End If

                    If NumeroValidazione <> "" Then
                        stb.Append(" AND a.Allegati_Documenti_Numero = " & Agro_SQL_SaveText_NULL(NumeroValidazione) & " " & vbCrLf)
                    End If

                    If DataValidazione <> AGRODATAINIZIO Then
                        stb.Append(" AND a.Validazione_Data = " & Agro_SQL_SaveDateTime_NULL(DataValidazione) & " " & vbCrLf)
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            stb.Append(" AND     tt.Inviato >= 0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            stb.Append(" AND     tt.Inviato = -1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------

                    Select Case SORT_Des1_Inizio2_Fine2
                        Case 1
                            stb.Append(" ORDER BY tt.Programmazione_Des ASC ")
                        Case 2
                            stb.Append(" ORDER BY tt.Validita_Inizio ASC ")
                        Case 3
                            stb.Append(" ORDER BY tt.Validita_fine ASC ")
                        Case Else
                            If xOrderBy <> "" Then
                                stb.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                            Else
                                stb.Append(" ORDER BY tt.Programmazione_Des ASC ")
                            End If
                    End Select



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
            DT = EseguiQuery_Lettura(objParametri, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try
        Return DT


    End Function

    Public Function Leggi_xCombo(
        ByRef MessaggioErrore As String,
        ByVal Programmazione_Cod As Integer,
        ByVal Piva As String,
        ByVal sa_cod As Integer,
        ByVal veg_cod As Integer,
        ByVal Programmazione_Des As String,
        ByVal SORT_Des1_Inizio2_Fine2 As Integer,
        ByVal Validita_Inizio As Date,
        ByVal Validita_Fine As Date,
        ByVal Tipo_Pianificazione As enum_TipoPianificazione,
        ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile,
        ByVal xFiltroAggiuntivo As String,
        ByVal xOrderBy As String,
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
    ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Programmazione_Testata_R.Leggi()"

        Dim DT As New DataTable

        Dim stb As New System.Text.StringBuilder

        Try


            stb.Length = 0

            stb.Append(" SELECT Distinct t.* ")

            stb.Append(" FROM    Programmazione_Testata t ")

            If veg_cod <> 0 Or sa_cod <> 0 Then
                stb.Append(" INNER JOIN Programmazione_Entita e ")
                stb.Append(" on e.programmazione_Cod = t.programmazione_cod ")
                stb.Append(" and e.Piva_SuperUser = t.Piva_SuperUser ")
            End If

            stb.Append(" WHERE   t.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            stb.Append(" AND     t.Validita_Inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
            stb.Append(" AND     t.Validita_fine >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")


            If Programmazione_Cod <> 0 Then
                stb.Append(" AND t.Programmazione_Cod = " & Agro_SQL_SaveNum(Programmazione_Cod.ToString))
            End If

            If Piva <> "" Then
                stb.Append(" AND t.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If

            If Programmazione_Des <> "" Then
                stb.Append(" AND t.Programmazione_Des like '%" & Agro_SQL_SaveText(Programmazione_Des) & "%' ")
            End If
            If Tipo_Pianificazione <> enum_TipoRicetta.Non_Filtrare Then
                stb.Append(" AND Tipo_Pianificazione = " & Agro_vb_SaveNum(Tipo_Pianificazione))
            End If

            If veg_cod <> 0 Then
                stb.Append(" AND e.veg_cod = " & Agro_SQL_SaveNum(veg_cod))
            End If

            If sa_cod <> 0 Then
                stb.Append(" AND e.sa_cod = " & Agro_SQL_SaveNum(sa_cod))
            End If


            If xFiltroAggiuntivo <> "" Then
                stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo,, objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    stb.Append(" AND     t.Inviato >= 0 ")
                    stb.Append(" AND     t.Inviato >= 0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    stb.Append(" AND     t.Inviato = -1 ")
                    stb.Append(" AND     t.Inviato = -1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            Select Case SORT_Des1_Inizio2_Fine2
                Case 1
                    stb.Append(" ORDER BY t.Programmazione_Des ASC ")
                Case 2
                    stb.Append(" ORDER BY t.Validita_Inizio ASC ")
                Case 3
                    stb.Append(" ORDER BY t.Validita_fine ASC ")
                Case Else
                    If xOrderBy <> "" Then
                        stb.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        stb.Append(" ORDER BY t.Programmazione_Des ASC ")
                    End If
            End Select

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try
        Return DT


    End Function

    'Attenzione, se modifichi la leggi completa, ricordati di verificare la chiamata
    'nella funzione Crea_GridView_Programmazioni della pagina lista planning
    Public Function Leggi_da_programmazione_cod_padre(
                            ByRef Programmazione_Cod_Padre As Integer,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                    ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Programmazione_Testata_R.Leggi()"

        Dim DT As New DataTable

        Dim stb As New System.Text.StringBuilder

        Try


            stb.Length = 0

            stb.Append(" select  * from Programmazione_Testata " & vbCrLf)
            stb.Append(" WHERE   Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

            If Programmazione_Cod_Padre <> 0 Then
                stb.Append(" AND Programmazione_Cod_Padre = " & Agro_SQL_SaveNum(Programmazione_Cod_Padre))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            DT = Nothing
        End Try
        Return DT


    End Function



    Public Function LeggixGIS(ByRef MessaggioErrore As String,
                            ByVal Programmazione_Cod As Integer,
                            ByVal Piva As String,
                            ByVal Programmazione_Des As String,
                            ByVal SORT_Des1_Inizio2_Fine2 As Integer,
                                ByVal Validita_Inizio As Date,
                                ByVal Validita_Fine As Date,
                                ByVal Tipo_Pianificazione As enum_TipoPianificazione,
                                    ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile,
                                    ByVal xFiltroAggiuntivo As String,
                                    ByVal xOrderBy As String,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                    Optional ByRef NumeroValidazione As String = "",
                                    Optional ByRef DataValidazione As Date = AGRODATAINIZIO) As DataTable
        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Programmazione_Testata_R.Leggi()"

        Dim DT As New DataTable

        Dim stb As New System.Text.StringBuilder

        Try

            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                    stb.Length = 0

                    stb.Append(" SELECT  * ")

                    stb.Append(" FROM    Programmazione_Testata ")

                    stb.Append(" WHERE   Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
                    stb.Append(" AND     Validita_Inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
                    stb.Append(" AND     Validita_fine >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")


                    If Programmazione_Cod <> 0 Then
                        stb.Append(" AND Programmazione_Cod = " & Agro_SQL_SaveNum(Programmazione_Cod.ToString))
                    End If

                    If Piva <> "" Then
                        stb.Append(" AND Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
                    End If

                    If Programmazione_Des <> "" Then
                        stb.Append(" AND Programmazione_Des like '%" & Agro_SQL_SaveText(Programmazione_Des) & "%' ")
                    End If
                    If Tipo_Pianificazione <> enum_TipoRicetta.Non_Filtrare Then
                        stb.Append(" AND Tipo_Pianificazione = " & Agro_vb_SaveNum(Tipo_Pianificazione))
                    End If


                    If xFiltroAggiuntivo <> "" Then
                        stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo,, objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            stb.Append(" AND     Programmazione_Testata.Inviato >= 0 ")
                            stb.Append(" AND     Programmazione_Testata.Inviato >= 0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            stb.Append(" AND     Programmazione_Testata.Inviato = -1 ")
                            stb.Append(" AND     Programmazione_Testata.Inviato = -1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------

                    Select Case SORT_Des1_Inizio2_Fine2
                        Case 1
                            stb.Append(" ORDER BY Programmazione_Des ASC ")
                        Case 2
                            stb.Append(" ORDER BY Validita_Inizio ASC ")
                        Case 3
                            stb.Append(" ORDER BY Validita_fine ASC ")
                        Case Else
                            If xOrderBy <> "" Then
                                stb.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                            Else
                                stb.Append(" ORDER BY Programmazione_Des ASC ")
                            End If
                    End Select




                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta

                    stb.Length = 0

                    stb.Append(" select  " & vbCrLf)
                    stb.Append("    tt.* " & vbCrLf)
                    stb.Append("    , coalesce(a.Allegati_Documenti_Numero, '') as Allegati_Documenti_Numero " & vbCrLf)
                    stb.Append("    , coalesce(a.Validazione_Data, cast('01/01/1900' as date)) as Validazione_Data " & vbCrLf)
                    stb.Append("    , coalesce(a.Allegati_Documenti_NomeFile, '') as Allegati_Documenti_NomeFile " & vbCrLf)
                    stb.Append("    , ge.Programmazione_Cod  as Programmazione_Cod_Gis " & vbCrLf)

                    stb.Append(" from Programmazione_Testata   tt " & vbCrLf)
                    stb.Append("    LEFT JOIN (select distinct  Programmazione_Cod, Allegati_Documenti_SuperUser, Allegati_Documenti_Cod  from  Allegati_EntitaxDocumenti) e  " & vbCrLf)
                    stb.Append("        on tt.Programmazione_Cod  = e.Programmazione_Cod  " & vbCrLf)
                    stb.Append("        and tt.Piva_SuperUser = e.Allegati_Documenti_SuperUser  " & vbCrLf)
                    stb.Append("    LEFT JOIN Allegati_Documenti a " & vbCrLf)
                    stb.Append("        on a.Allegati_Documenti_Cod = e.Allegati_Documenti_Cod  " & vbCrLf)
                    stb.Append("        and a.Allegati_Documenti_SuperUser = e.Allegati_Documenti_SuperUser  " & vbCrLf)
                    stb.Append("    LEFT JOIN GIS_Entita ge ON tt.Programmazione_Cod = ge.Programmazione_Cod " & vbCrLf)

                    stb.Append(" WHERE   tt.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
                    stb.Append(" AND     tt.Validita_Inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
                    stb.Append(" AND     tt.Validita_fine >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")


                    If Programmazione_Cod <> 0 Then
                        stb.Append(" AND tt.Programmazione_Cod = " & Agro_SQL_SaveNum(Programmazione_Cod.ToString))
                    End If

                    If Piva <> "" Then
                        stb.Append(" AND tt.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
                    End If

                    If Programmazione_Des <> "" Then
                        stb.Append(" AND tt.Programmazione_Des like '%" & Agro_SQL_SaveText(Programmazione_Des) & "%' ")
                    End If

                    If Tipo_Pianificazione <> enum_TipoRicetta.Non_Filtrare Then
                        stb.Append(" AND tt.Tipo_Pianificazione =" & Agro_vb_SaveNum(Tipo_Pianificazione))
                    End If

                    If NumeroValidazione <> "" Then
                        stb.Append(" AND a.Allegati_Documenti_Numero = " & Agro_SQL_SaveText_NULL(NumeroValidazione) & " " & vbCrLf)
                    End If

                    If DataValidazione <> AGRODATAINIZIO Then
                        stb.Append(" AND a.Validazione_Data = " & Agro_SQL_SaveDateTime_NULL(DataValidazione) & " " & vbCrLf)
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo,, objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            stb.Append(" AND     tt.Inviato >= 0 ")
                            stb.Append(" AND     tt.Inviato >= 0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            stb.Append(" AND     tt.Inviato = -1 ")
                            stb.Append(" AND     tt.Inviato = -1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------

                    Select Case SORT_Des1_Inizio2_Fine2
                        Case 1
                            stb.Append(" ORDER BY tt.Programmazione_Des ASC ")
                        Case 2
                            stb.Append(" ORDER BY tt.Validita_Inizio ASC ")
                        Case 3
                            stb.Append(" ORDER BY tt.Validita_fine ASC ")
                        Case Else
                            If xOrderBy <> "" Then
                                stb.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                            Else
                                stb.Append(" ORDER BY tt.Programmazione_Des ASC ")
                            End If
                    End Select



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
            DT = EseguiQuery_Lettura(objParametri, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try
        Return DT


    End Function


    Public Function GetDistinct_Veg_COD_Cul_Cod(
                            ByVal Veg_Cod_1_Cul_Col_2 As Integer,
                                    ByVal xFiltroAggiuntivo As String,
                                    ByVal xOrderBy As String,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                    ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Programmazione_Testata_R.Leggi()"

        Dim DT As New DataTable

        Dim stb As New System.Text.StringBuilder

        Try

            stb.Length = 0
            If Veg_Cod_1_Cul_Col_2 = 1 Then
                stb.Append(" select distinct veg_cod ")
            Else
                stb.Append(" select distinct Cul_cod ")
            End If


            stb.Append(" from Programmazione_Testata ")
            stb.Append(" inner join Programmazione_Entita on Programmazione_Entita.Programmazione_Cod = Programmazione_Testata.Programmazione_Cod ")
            'where   " & vbCrLf)


            stb.Append(" WHERE   1=1 ")


            If xFiltroAggiuntivo <> "" Then
                stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo,, objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    stb.Append(" AND     Programmazione_Testata.Inviato >= 0 ")
                    stb.Append(" AND     Programmazione_Entita.Inviato >= 0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    stb.Append(" AND     Programmazione_Testata.Inviato = -1 ")
                    stb.Append(" AND     Programmazione_Entita.Inviato = -1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                stb.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                If Veg_Cod_1_Cul_Col_2 = 1 Then
                    stb.Append(" ORDER BY Veg_Cod ASC ")
                Else
                    stb.Append(" ORDER BY Cul_Cod ASC ")
                End If

            End If





            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            Scrivi_LOG(objParametri, NomeRoutine, ex.Message)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & ex.Message)
        End Try
        Return DT


    End Function





    Public Function GetMax_NumeColture(
                                      ByVal tipo_pianificazione As Integer,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                ) As Integer

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Programmazione_Testata_R.Leggi()"

        Dim DT As New DataTable

        Dim stb As New System.Text.StringBuilder

        Try

            stb.Length = 0
            stb.Append(" select max( NumColture )  as max  from Programmazione_Testata ")
            stb.Append("  where tipo_pianificazione =" & tipo_pianificazione)

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            Scrivi_LOG(objParametri, NomeRoutine, ex.Message)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & ex.Message)
        End Try
        If IsDBNull(DT.Rows(0).Item("max")) Then
            Return 0
        Else
            Return DT.Rows(0).Item("max")
        End If


    End Function




    '################################################################################
    Public Function Leggi_per_prenotazione_piante(
        ByVal Piva As String,
        ByVal Stabilimento As String,
        ByVal Validita_Inizio As Date,
        ByVal Validita_Fine As Date,
        ByVal Tipo_Pianificazione As enum_TipoRicetta,
        ByVal ApplicaFiltroUtentiVisibilitaAppoggio As Boolean,
        ByVal xFiltroAggiuntivo As String,
        ByVal xOrderBy As String,
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
        Optional ByVal VivaioAssociatoAlias As String = "Vivaio Associato",
        Optional ByVal CodFiscaleAlias As String = "Cod Fiscale",
        Optional ByVal TipologieAlias As String = "Tipologie"
    ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Programmazione_Testata_R.Leggi()"

        Dim DT As New DataTable

        Dim stb As New System.Text.StringBuilder

        Try
            stb.Length = 0

            stb.AppendLine(" SELECT i.*, ISNULL(part.particellePresenti, -1) AS ParticellePresenti, ISNULL(progettiPresenti.Programmazione_Cod_from , -1) AS ProgettiPresenti ")

            stb.AppendLine(" FROM ( ")

            stb.AppendLine("        SELECT year (pe.Validita_Inizio ) AS anno  ")
            stb.AppendLine("            , PT.Programmazione_Cod ")
            stb.AppendLine("            , PT.NumColture ")
            stb.AppendLine("            , PT.Data_Modifica  ")
            stb.AppendLine("            , PT.Piva ")
            stb.AppendLine("            , Case WHEN ISNULL(Imprese.partitaIvaReale, '') = '' THEN PT.PIVA ELSE Imprese.partitaIvaReale END AS partitaIvaReale ")
            stb.AppendLine("            , PT.Note AS rag_soc ")
            stb.AppendLine("            , PT.Data_Creazione ")
            stb.AppendLine("            , pt.Programmazione_Cod_Padre AS OrdineAssociato ")
            stb.AppendLine("            , ISNULL((SELECT note FROM Programmazione_Testata pr_te WHERE pr_te.Programmazione_Cod = pt.Programmazione_Cod_Padre ),'') AS '" & VivaioAssociatoAlias & "'  ")
            stb.AppendLine("            , (SELECT DISTINCT Descrizione_Centro_Iscrizione FROM PDC_Stabilimenti_Apofruit AS psa WHERE (VCCOIS = PE.Entita_Des)) AS prenotazione_stabilimento ")
            stb.AppendLine("            , PE.TipoZona AS prenotazione_tipo ")
            stb.AppendLine("            , pe.veg_cod  ")
            stb.AppendLine("            , PE.Grfi_Cod ")
            stb.AppendLine("            , PE.Superficie ")
            stb.AppendLine("            , PE.Resa ")
            stb.AppendLine("            , ISNULL((SELECT grva_des FROM GruppoVarietale WHERE grva_cod= PE.cop_cod ), N'') AS prenotazione_Tipologia_Varietale_Des ")
            stb.AppendLine("            , (SELECT DISTINCT MacroCategoria FROM Prenotazione_Piante_Categoria AS ppc WHERE (ID_MacroCategoria = PE.Veg_Cod_Prec)) AS prenotazione_categoria ")
            stb.AppendLine("            , ISNULL((SELECT DISTINCT NomeCategoria FROM Prenotazione_Piante_Categoria AS ppc WHERE (ID_Categoria = PE.Id_Mat_O)), N'') AS prenotazione_sottocategoria ")

            ' VAnni: 18/6/2020: Aggiunta lettura da calibro Fresh AND Food
            stb.AppendLine("            , ISNULL((SELECT Calibro FROM Prenotazione_Piante_Calibro AS ppc WHERE (ID_Calibro = PE.Id_Cod)), ISNULL((SELECT o.sigla FROM OTabelle_Parametri o WHERE pe.codice_contratto = o.Piva + '-' + cast(o.Tabella_Cod AS varchar(50)) + '-' + cast(o.Tabella_Par_Cod AS varchar(50)) + '-' + cast(o.Modulo_Generazione AS varchar(50)) )   , N'') ) AS prenotazione_calibro ")


            stb.AppendLine("            , ISNULL((SELECT sv.Veg_Des FROM SpecieVegetali AS sv INNER JOIN Cultivar AS c ON c.Veg_Cod = sv.Veg_Cod WHERE (c.Cul_Cod = PE.Cul_Cod)), N'') AS specie ")
            stb.AppendLine("            , ISNULL((SELECT Cul_Des FROM Cultivar AS c WHERE (Cul_Cod = PE.Cul_Cod)), N'') AS varieta ")
            stb.AppendLine("            , PE.Id_Fre AS prenotazione_certificazione ")
            stb.AppendLine("            , ISNULL((SELECT Descrizione_Certificazione FROM Prenotazione_Piante_Certificazione WHERE ID_Certificazione = PE.Id_Fre ), N'') AS prenotazione_certificazione_Des  ")
            stb.AppendLine("            , (SELECT TOP 1 Foral_Des FROM FormeAllevamento WHERE foral_cod = PE.N_distribuito ) AS prenotazione_forma_allevamento ")
            stb.AppendLine("            , PE.Num_Piante ")
            stb.AppendLine("            , PE.Ciclo ")
            stb.AppendLine("            , PE.Grva_Cod ")
            stb.AppendLine("            , PE.Data_Semina ")
            stb.AppendLine("            , PE.Data_Raccolta ")
            stb.AppendLine("            , PE.Cul_Cod_Cliente ")
            stb.AppendLine("            , PE.N_fabbisogno ")
            stb.AppendLine("            , PE.Foral_Cod ")
            stb.AppendLine("            , PE.Riferimento_Alfanumerico_Appezzamento AS Codice_Richiesta ")
            stb.AppendLine("            , ISNULL(( SELECT top 1 Sem_Des FROM TipologieSementi tss WHERE tss.SEM_COD = PE.Foral_Cod ), '') AS [" & TipologieAlias & "] ")
            stb.AppendLine("            , PE.Port_Cod ")
            stb.AppendLine("            , PE.TRA_Fila ")
            stb.AppendLine("            , PE.SU_Fila ")
            stb.AppendLine("            , PE.FlagIrrigabilita ")
            stb.AppendLine("            , PE.FlagSecondoRaccolto ")
            stb.AppendLine("            , PE.Codice_Fiscale_Tecnico ")
            stb.AppendLine("            , PE.Superficie_Futura ")
            stb.AppendLine("            , PE.Operazione_Cod ")
            stb.AppendLine("            , PE.Macrouso_Cod ")
            stb.AppendLine("            , PE.Via_Stringa ")
            stb.AppendLine("            , PE.Unita_Vitata ")
            stb.AppendLine("            , PE.Validita_Inizio_Impianto ")


            stb.AppendLine("            , PE.Id_Budget ")
            stb.AppendLine("            , PE.germinabilita ")
            stb.AppendLine("            , PE.SupBZ_Riduzione ")
            stb.AppendLine("            , PE.DistBZ_VegNatNonColt ")
            stb.AppendLine("            , PE.MetodoProduzione_Cod ")
            stb.AppendLine("            , PE.Desc_impianto_budget ")
            stb.AppendLine("            , BT.Nome_Budget ")
            stb.AppendLine("            , CT.Rag_Soc as Ditta_Sementiera_Soc ")
            stb.AppendLine("            , MP.Mat_Des ")
            stb.AppendLine("            , TS.Descrizione ")

            stb.AppendLine("            , ISNULL((SELECT val_cod FROM Imprese_Codici WHERE (id_cod = 1033) And (PIVA = PT.Piva)), N'') AS Cod_socio ")
            stb.AppendLine("            , ISNULL(PT.Stato, N'0') AS stato ")
            stb.AppendLine("            , ISNULL((SELECT Rag_Soc FROM Contatti c INNER JOIN Risorse_Umane risum ON c.Piva = risum.Piva And c.Cod_Contatto = risum.Cod_Contatto WHERE risum.Cod_Rapporto = -17 And c.Cod_Contatto = PE.Veg_Cod_Cliente)  , N'') AS Vivaio ")
            stb.AppendLine("            , PE.Progetto_Des AS Note  ")
            stb.AppendLine("            , PE.Note AS prenotazione_data_fattura ")
            stb.AppendLine("            , ISNULL((SELECT val_cod FROM Imprese_Codici WHERE (id_cod = 1162) And (PIVA = PT.Piva)), N'') AS Centro_Conferimento ")
            stb.AppendLine("            , ISNULL((SELECT stb.cod_stabilimento FROM Imprese_Codici ic INNER JOIN PDC_Stabilimenti_Apofruit stb ON stb.vcCois = ic.val_cod WHERE (ic.id_cod = 1162) And (PIVA = PT.Piva))   , N'') AS Centro_ConferimentoW                ")
            stb.AppendLine("            , PE.campo_cod AS Associato ")
            stb.AppendLine("            , ISNULL((SELECT DISTINCT Port_Des FROM Portinnesti p WHERE p.Port_Cod = PE.Regolamento_Cod),'') AS Portinnesto ")
            stb.AppendLine("            , CASE pe.resa WHEN 0 THEN 'No' WHEN 1 THEN 'Si-Fallanza' WHEN 2 THEN 'Si-Gratuito' END AS Ripasso ")
            stb.AppendLine("            , ind_des AS Indirizzo  ")
            stb.AppendLine("            , case when frz_des<>'' then frz_des + ' ' else '' end + ISNULL(i1.localita, ind.com_des) AS Località")
            stb.AppendLine("            , ind.CAP AS CAP ")
            stb.AppendLine("            , pro_cod AS PR ")
            stb.AppendLine("            , (SELECT top 1 c.Cod_Contatto FROM Contatti c INNER JOIN Risorse_Umane r ON c.piva= r.Piva And c.Cod_Contatto = r.Cod_Contatto WHERE r.Cod_Rapporto = -1  And c.Piva  =Imprese.piva) AS [" & CodFiscaleAlias & "]               ")
            stb.AppendLine("            , (SELECT top 1 rr.numero FROM Contatti c INNER JOIN Risorse_Umane r ON c.piva= r.Piva And c.Cod_Contatto = r.Cod_Contatto INNER JOIN ContattiXRubrica cr ON c.piva= cr.Piva And c.Cod_Contatto = cr.Cod_Contatto  INNER JOIN Rubrica rr ON rr.cod_rubrica = cr.Cod_Rubrica  WHERE c.Piva  =Imprese.piva  And Cod_Rapporto = -1 ) AS 'Telefono'                ")
            stb.AppendLine("            , case PE.Grfi_Cod when 0 then 'No' when -1 then 'Si' END AS OCM ")
            stb.AppendLine("            , (SELECT Reg_DES FROM Regolamenti r WHERE r.Reg_Cod = PE.Port_Cod) AS Richiesta ")
            stb.AppendLine("            , case when PE.Data_raccolta = '31/12/2100' then null else PE.Data_Raccolta end AS Data_Ritiro ")
            stb.AppendLine("            , '' AS Particelle ")
            stb.AppendLine("            , '' AS Progetti ")
            stb.AppendLine("            , CASE WHEN statoRichiesto.Programmazione_entita_Cod is null THEN 0 ELSE 1 END AS StatoRichiesto  ")
            stb.AppendLine("            , CASE WHEN statoOk.Programmazione_entita_Cod is null THEN 0 ELSE 1 END AS StatoOk ")
            stb.AppendLine("            , Data_Fioritura_Prevista AS DataPrevistaInnestoTrapianto ")
            stb.AppendLine("            , Stato_Ribaltamento AS N_Maschi ")
            stb.AppendLine("            , Regolamento_Concimazione_Cod AS N_Femmine")
            stb.AppendLine("            , OTP_TIPO.DESCRIZIONE AS prenotazione_guideAudits_Tipo")
            stb.AppendLine("            , ISNULL(r.numero, '') AS prenotazione_email_richiedente ")
            stb.AppendLine("            , ISNULL(cc.Val_cod, '') AS prenotazione_codice_SDI_email_richiedente")

            stb.AppendLine("  ")

            stb.AppendLine("        FROM Programmazione_Testata PT ")
            stb.AppendLine("        INNER JOIN Programmazione_Entita PE ON PE.Programmazione_Cod = PT.Programmazione_Cod  ")
            stb.AppendLine("        LEFT JOIN Budget_Testata BT ON PE.Id_Budget = BT.Id_Budget ")
            stb.AppendLine("        LEFT JOIN Contatti CT ON ( (CT.piva = pt.piva AND CT.Cod_Contatto = PE.Codice_Fiscale_Tecnico) OR (CT.Sa_Cod = -1 AND CT.Cod_Contatto = PE.Codice_Fiscale_Tecnico)) ")
            stb.AppendLine("        LEFT JOIN Materie_Prime MP ON ((pt.piva = MP.piva OR MP.sa_cod = 1) AND MP.elem_cod = 10 AND MP.Mat_cod = PE.Superficie_Futura) ")
            stb.AppendLine("        LEFT JOIN Tecnologie_Sementi TS ON ( TS.Cod_TecnologiaSementi = PE.MetodoProduzione_Cod) ")
            stb.AppendLine("        INNER JOIN imprese ON pt.piva = imprese.piva ")

            If ApplicaFiltroUtentiVisibilitaAppoggio Then
                stb.AppendLine("  ")
                stb.AppendLine("        INNER JOIN Utenti_Visibilita_Appoggio uva (NOLOCK) ")
                stb.AppendLine("            ON uva.piva = imprese.piva ")
                stb.AppendLine("            AND uva.username = '" & objParametri.UtenteUsername & "' ")
                stb.AppendLine("            AND uva.entita_cod = 1 ")
            End If

            stb.AppendLine("  ")

            stb.AppendLine("        LEFT JOIN ( ")
            stb.AppendLine("            SELECT exe.Programmazione_entita_Cod_From AS Programmazione_entita_Cod  ")
            stb.AppendLine("            FROM programmazione_entitaXProgrammazione_entita exe  ")
            stb.AppendLine("            INNER JOIN programmazione_entita ePrj  ")
            stb.AppendLine("                ON exe.tipoRelazione = 1  ")
            stb.AppendLine("                AND ePrj.Programmazione_Entita_Cod = exe.Programmazione_Entita_Cod_To     ")
            stb.AppendLine("            GROUP BY exe.Programmazione_entita_Cod_From  ")
            stb.AppendLine("        ) statoRichiesto ")
            stb.AppendLine("            ON statoRichiesto.Programmazione_entita_Cod = PE.Programmazione_entita_Cod")

            stb.AppendLine("")

            stb.AppendLine("        LEFT JOIN ( ")
            stb.AppendLine("            SELECT ")
            stb.AppendLine("                exe.Programmazione_entita_Cod_From AS Programmazione_entita_Cod ")
            stb.AppendLine("            FROM programmazione_entitaXProgrammazione_entita exe ")
            stb.AppendLine("            INNER JOIN programmazione_entita ePrj ")
            stb.AppendLine("                ON exe.tipoRelazione = 1 ")
            stb.AppendLine("                AND ePrj.Programmazione_Entita_Cod = exe.Programmazione_Entita_Cod_To    ")
            stb.AppendLine("            INNER JOIN Pratiche P ")
            stb.AppendLine("                ON P.Programmazione_Entita_Cod = ePrj.Programmazione_Entita_Cod ")
            stb.AppendLine("            INNER JOIN Pratiche_Stati ps ")
            stb.AppendLine("                ON ps.pratica_cod = P.pratica_cod ")
            stb.AppendLine("             --WHERE exe.Programmazione_entita_Cod_From = 121 --x test ")
            stb.AppendLine("            WHERE ps.Stato_Cod IN (2106, 2107, 2108)")
            'stb.AppendLine("            2106 ")
            'stb.AppendLine("            , 2107 ")
            'stb.AppendLine("             2108 ")
            'stb.AppendLine("            ) ")
            stb.AppendLine("             --il group serve per ottenere un solo record a fronte di un join con diversi passaggi di stato ")
            stb.AppendLine("            GROUP BY exe.Programmazione_entita_Cod_From ")
            stb.AppendLine("        ) statoOk")
            stb.AppendLine("            ON statoOk.Programmazione_entita_Cod = PE.Programmazione_entita_Cod")

            stb.AppendLine("  ")

            stb.AppendLine("        LEFT OUTER JOIN ImpresexIndirizzi AS ii ON ii.PIVA = Imprese.PIVA  ")
            stb.AppendLine("        LEFT OUTER JOIN Indirizzi AS ind ON ii.cod_indirizzo = ind.cod_indirizzo ")
            stb.AppendLine("        LEFT JOIN ISTAT i1 ON i1.PROV =ind.pro_cod_istat AND i1.COM = ind.com_cod_istat  " & vbCrLf)

            stb.AppendLine("  ")

            stb.AppendLine("		LEFT JOIN Programmazione_Entita_Codici PEC_TIPO")
            stb.AppendLine("			ON PE.Programmazione_Entita_Cod = PEC_TIPO.Programmazione_Entita_Cod AND PEC_TIPO.id_cod = 1331")
            stb.AppendLine("			AND PEC_TIPO.Piva_SuperUser = PE.Piva_SuperUser")
            stb.AppendLine("		LEFT JOIN OTabelle_Parametri OTP_TIPO ")
            stb.AppendLine("			ON PEC_TIPO.id_cod = OTP_TIPO.Tabella_Cod  AND PEC_TIPO.VAL_COD = OTP_TIPO.Tabella_Par_Cod")

            stb.AppendLine("  ")

            stb.AppendLine("		LEFT JOIN ContattiXRubrica cr")
            stb.AppendLine("		    ON cr.Cod_Rubrica = unar")
            stb.AppendLine("		LEFT JOIN Rubrica r ")
            stb.AppendLine("		    ON cr.Cod_Rubrica = r.cod_rubrica ")
            stb.AppendLine("		    AND r.descr like '%mail%'")
            stb.AppendLine("		LEFT JOIN Contatti_Codici cc")
            stb.AppendLine("		    ON cc.Cod_Contatto = cr.Cod_Contatto ")
            stb.AppendLine("		    AND cc.id_cod = " & enum_CodiciAnagrafe.SDI)
            stb.AppendLine("		    AND cc.PIVA = pe.Piva")

            stb.AppendLine("		")

            stb.AppendLine("        WHERE PT.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            stb.AppendLine("        AND PT.Validita_Inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
            stb.AppendLine("        AND PT.Validita_fine >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")

            If Piva <> "" Then
                stb.AppendLine("        AND PT.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If

            If Tipo_Pianificazione <> enum_TipoRicetta.Non_Filtrare Then
                stb.AppendLine("        AND PT.Tipo_Pianificazione = " & Agro_vb_SaveNum(Tipo_Pianificazione))
            End If


            If xFiltroAggiuntivo <> "" Then
                stb.AppendLine("        AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo,, objParametri))
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    stb.AppendLine("        AND PT.Inviato >= 0 ")
                    stb.AppendLine("        AND PT.Inviato >= 0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    stb.AppendLine("        AND PT.Inviato = -1 ")
                    stb.AppendLine("        AND PT.Inviato = -1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            stb.AppendLine(" ) i")

            stb.AppendLine("  ")

            stb.AppendLine(" LEFT JOIN ")
            stb.AppendLine(" ( ")
            stb.AppendLine("        SELECT DISTINCT Programmazione_Cod AS particellePresenti ")
            stb.AppendLine("        FROM Programmazione_Particelle ep ")
            stb.AppendLine("        INNER JOIN Programmazione_Entita e ")
            stb.AppendLine("            ON e.Programmazione_Entita_Cod = ep.Programmazione_Entita_Cod ")
            stb.AppendLine(" ) part ")
            stb.AppendLine("    ON part.particellePresenti = i.Programmazione_Cod")

            stb.AppendLine("  ")

            stb.AppendLine(" LEFT JOIN programmazione_EntitaXprogrammazione_entita progettiPresenti ")
            stb.AppendLine("    ON progettiPresenti.tiporelazione = 1 ")
            stb.AppendLine("    AND progettiPresenti.Programmazione_Cod_From = i.programmazione_cod")


            If Stabilimento <> "" Then
                stb.Append(" WHERE Centro_ConferimentoW = '" & Agro_SQL_SaveText(Stabilimento) & "' ")
            End If


            If xOrderBy <> "" Then
                stb.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                stb.AppendLine(" ORDER BY Data_Creazione desc, NumColture desc, Note ")
            End If


            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            Scrivi_LOG(objParametri, NomeRoutine, ex.Message)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & ex.Message)
        End Try
        Return DT


    End Function


    '################################################################################
    Public Function Leggi_prenotazione_piante_associate(
                            ByVal Programmazione_cod_padre As String,
                                    ByVal xFiltroAggiuntivo As String,
                                    ByVal xOrderBy As String,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                    ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Programmazione_Testata_R.Leggi()"

        Dim DT As New DataTable

        Dim stb As New System.Text.StringBuilder

        Try
            stb.Length = 0


            stb.AppendLine("  SELECT ")
            stb.AppendLine("   Riferimento_Alfanumerico_Appezzamento as Codice ")
            stb.AppendLine(" , Programmazione_Testata.Programmazione_Cod ")
            stb.AppendLine(" , Programmazione_Entita.Campo_Cod ")
            stb.AppendLine(" , Programmazione_Entita.Sa_Cod ")
            stb.AppendLine(" , Programmazione_Testata.piva ")
            stb.AppendLine(" , Programmazione_Testata.note + isnull( ' [' + i.rag_soc + ']', '') ")
            stb.AppendLine("   as Rag_Soc_Richiedente")

            stb.AppendLine("             FROM            Programmazione_Testata INNER JOIN ")
            stb.AppendLine("             Programmazione_Entita ON Programmazione_Testata.Piva_SuperUser = Programmazione_Entita.Piva_SuperUser AND  ")
            stb.AppendLine("             Programmazione_Testata.Programmazione_Cod = Programmazione_Entita.Programmazione_Cod LEFT JOIN")
            stb.AppendLine("             GerarchiaImprese gi on gi.Figlio = Programmazione_Testata.piva LEFT JOIN  ")
            stb.AppendLine("             Imprese i on i.PIVA = gi.Padre ")
            stb.AppendLine(" ")

            stb.AppendLine("             WHERE        1 = 1 AND (Programmazione_Testata.Tipo_Pianificazione = 11)  ")

            If Programmazione_cod_padre <> "" Then
                stb.AppendLine(" AND Programmazione_Testata.Programmazione_Cod_Padre =" & Agro_vb_SaveNum(Programmazione_cod_padre) & " ")
            End If


            If xFiltroAggiuntivo <> "" Then
                stb.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo,, objParametri))
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    stb.AppendLine(" AND     Programmazione_Testata.Inviato >= 0 ")
                    stb.AppendLine(" AND     Programmazione_Testata.Inviato >= 0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    stb.AppendLine(" AND     Programmazione_Testata.Inviato = -1 ")
                    stb.AppendLine(" AND     Programmazione_Testata.Inviato = -1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                stb.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                stb.AppendLine(" order by Programmazione_Testata.programmazione_cod ")
            End If


            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            Scrivi_LOG(objParametri, NomeRoutine, ex.Message)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & ex.Message)
        End Try
        Return DT


    End Function

    Public Function Leggi_prenotazione_piante_associateMulti(
                            ByVal Programmazione_cod_padre As List(Of Integer),
                                    ByVal xFiltroAggiuntivo As String,
                                    ByVal xOrderBy As String,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                    ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Programmazione_Testata_R.Leggi()"

        Dim DT As New DataTable

        Dim stb As New System.Text.StringBuilder

        Try
            stb.Length = 0


            stb.AppendLine("  SELECT ")
            stb.AppendLine("   Riferimento_Alfanumerico_Appezzamento as Codice ")
            stb.AppendLine(" , Programmazione_Testata.Programmazione_Cod ")
            stb.AppendLine(" , Programmazione_Testata.Programmazione_Cod_Padre ")
            stb.AppendLine(" , Programmazione_Entita.Campo_Cod ")
            stb.AppendLine(" , Programmazione_Entita.Sa_Cod ")
            stb.AppendLine(" , Programmazione_Testata.piva ")
            stb.AppendLine(" , Programmazione_Testata.note + isnull( ' [' + i.rag_soc + ']', '') ")
            stb.AppendLine("   as Rag_Soc_Richiedente")

            stb.AppendLine("             FROM            Programmazione_Testata INNER JOIN ")
            stb.AppendLine("             Programmazione_Entita ON Programmazione_Testata.Piva_SuperUser = Programmazione_Entita.Piva_SuperUser AND  ")
            stb.AppendLine("             Programmazione_Testata.Programmazione_Cod = Programmazione_Entita.Programmazione_Cod LEFT JOIN")
            stb.AppendLine("             GerarchiaImprese gi on gi.Figlio = Programmazione_Testata.piva LEFT JOIN  ")
            stb.AppendLine("             Imprese i on i.PIVA = gi.Padre ")
            stb.AppendLine(" ")

            stb.AppendLine("             WHERE        1 =1 AND (Programmazione_Testata.Tipo_Pianificazione = 11)  ")

            If Programmazione_cod_padre.Count > 0 Then
                stb.AppendLine(" AND Programmazione_Testata.Programmazione_Cod_Padre IN (" & Agro_SQL_Save_Clausola_IN(String.Join(",", Programmazione_cod_padre)) & ") ")
            End If


            If xFiltroAggiuntivo <> "" Then
                stb.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo,, objParametri))
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    stb.AppendLine(" AND     Programmazione_Testata.Inviato >= 0 ")
                    stb.AppendLine(" AND     Programmazione_Testata.Inviato >= 0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    stb.AppendLine(" AND     Programmazione_Testata.Inviato = -1 ")
                    stb.AppendLine(" AND     Programmazione_Testata.Inviato = -1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                stb.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                stb.AppendLine(" order by Programmazione_Testata.programmazione_cod ")
            End If


            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            Scrivi_LOG(objParametri, NomeRoutine, ex.Message)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & ex.Message)
        End Try
        Return DT


    End Function

    Public Function LeggiPrenotazioneAssociataAdOrdine(
        ByVal Programmazione_Cod As Integer,
        ByVal xFiltroAggiuntivo As String,
        ByVal xOrderBy As String,
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
        Optional ByVal AliasTipologie As String = "Tipologie"
    ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Programmazione_Testata_R.LeggiPrenotazioneAssociataAdOrdine()"

        Dim DT As New DataTable

        Dim stb As New System.Text.StringBuilder

        Try
            stb.Length = 0
            stb.AppendLine(" select * ")
            stb.AppendLine(" from Programmazione_Testata ")
            stb.AppendLine(" where Programmazione_Cod_Padre = " & Programmazione_Cod)
            stb.AppendLine(" and tipo_Pianificazione = " & enum_TipoPianificazione.Pianificazione_PrenotazionePiante)

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            Scrivi_LOG(objParametri, NomeRoutine, ex.Message)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & ex.Message)
        End Try
        Return DT


    End Function
    '################################################################################
    Public Function Leggi_per_Ordini_piante(
        ByVal Piva As String,
        ByVal Validita_Inizio As Date,
        ByVal Validita_Fine As Date,
        ByVal Tipo_Pianificazione As enum_TipoRicetta,
        ByVal join_ContattiXUtentiGias As Boolean,
        ByVal applicaFiltroUtentiVisibilitaAppoggio As Boolean,
        ByVal xFiltroAggiuntivo As String,
        ByVal xOrderBy As String,
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
        Optional ByVal AliasTipologie As String = "Tipologie",
        Optional ByVal isAsipo As Boolean = False
        ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Programmazione_Testata_R.Leggi()"

        Dim DT As New DataTable

        Dim stb As New System.Text.StringBuilder

        Try
            stb.Length = 0
            If isAsipo Then
                ' Per la colonna richiesta, estraggo tutti i padri e concatteno le loro rag_soc 
                stb.AppendLine(" 
                    WITH rag_soc_concattenate as (
                        select Figlio, STRING_AGG(i.rag_soc, ' | ') as 'rag' from GerarchiaImprese gi
                        INNER JOIN Imprese i ON i.PIVA = gi.padre
                        GROUP BY Figlio
                    ),
                    prenotazioni_associate as (
                        select PE.Programmazione_Cod, PE.Programmazione_Entita_Cod, PE.Riferimento_Alfanumerico_Appezzamento 
                        , PE.Riferimento_Alfanumerico_Appezzamento + ' - ' + PT.note + isnull('[' + Rsc.rag + ']', '') as Rag_Soc_richiedente
                        , PT.Programmazione_Cod_Padre
                        from Programmazione_Testata PT
                        INNER JOIN Programmazione_Entita PE on (PE.Programmazione_Cod = PT.Programmazione_Cod AND PE.Piva_SuperUser = PT.Piva_SuperUser)
                        LEFT JOIN rag_soc_concattenate Rsc On Rsc.Figlio = PT.piva
                        WHERE PT.Tipo_Pianificazione = 11
                    )")
            End If
            stb.AppendLine(" Select  ")
            stb.AppendLine("    pt.programmazione_cod ")
            stb.AppendLine("  , PT.NumColture ")
            stb.AppendLine("  , PT.Piva ")
            If isAsipo Then
                stb.AppendLine("  , Case WHEN ISNULL(I.partitaIvaReale, '') = '' THEN I.PIVA ELSE I.partitaIvaReale END AS partitaIvaReale ")
            Else
                stb.AppendLine("  , PT.Piva AS partitaIvaReale ")
            End If
            stb.AppendLine("  , PT.Note as rag_soc ")
            stb.AppendLine("  , convert(nvarchar(50) , pt.Data_Creazione , 103) as Data_Creazione   ")
            stb.AppendLine("   ,PT.Validita_Inizio ")
            stb.AppendLine("  , PT.Validita_Fine ")
            stb.AppendLine("  , (select distinct psa.Descrizione_Centro_Iscrizione  from PDC_Stabilimenti_Apofruit (NOLOCK) psa where psa.VCCOIS = PE.Entita_Des) as prenotazione_stabilimento ")
            stb.AppendLine("  , PE.TipoZona as prenotazione_tipo ")
            stb.AppendLine("  , (select distinct MacroCategoria  from Prenotazione_Piante_Categoria (NOLOCK) ppc where ppc.ID_MacroCategoria =PE.Veg_Cod_Prec) as prenotazione_categoria  ")
            stb.AppendLine("  , isnull((select distinct NomeCategoria from Prenotazione_Piante_Categoria (NOLOCK) ppc where ppc.ID_Categoria = PE.Id_Mat_O ) ,'')  as prenotazione_sottocategoria ")

            ' VAnni: 18/6/2020: Aggiunta lettura da calibro Fresh and Food
            stb.AppendLine("   , ISNULL((SELECT        Calibro FROM            Prenotazione_Piante_Calibro (NOLOCK) AS ppc WHERE        (ID_Calibro = PE.Id_Cod))   , ISNULL((SELECT o.sigla from OTabelle_Parametri o where pe.codice_contratto = o.Piva + '-' + cast(o.Tabella_Cod as varchar(50)) + '-' + cast(o.Tabella_Par_Cod as varchar(50)) + '-' + cast(o.Modulo_Generazione as varchar(50)) )   , N'') ) AS prenotazione_calibro ")

            stb.AppendLine("  , isnull((select sv.Veg_cod from SpecieVegetali (NOLOCK) sv inner join cultivar (NOLOCK) c on c.veg_cod = sv.veg_cod where  c.cul_cod = PE.Cul_Cod ) ,'' )as Veg_cod ")
            stb.AppendLine("  , isnull((select veg_des from SpecieVegetali (NOLOCK) sv inner join cultivar (NOLOCK) c on c.veg_cod = sv.veg_cod where  c.cul_cod = PE.Cul_Cod ) ,'' )as specie ")
            stb.AppendLine("  , isnull(PE.Cul_Cod  ,'') as Cul_cod ")
            stb.AppendLine("  , isnull((select cul_des from cultivar (NOLOCK) c where c.cul_cod = PE.Cul_Cod ) ,'') as varieta ")
            stb.AppendLine("  , ISNULL((select Descrizione_Certificazione from Prenotazione_Piante_Certificazione (NOLOCK) where ID_Certificazione = PE.Id_Fre ) , N'') as certificazione_Des  ")
            stb.AppendLine("  , PE.Grfi_Cod ")
            stb.AppendLine("  , PE.Cop_Cod ")
            stb.AppendLine("  , PE.Superficie ")
            stb.AppendLine("  , PE.Resa ")
            stb.AppendLine("  , PE.Id_Fre as prenotazione_certificazione ")
            stb.AppendLine("  , PE.N_distribuito as prenotazione_forma_allevamento ")
            stb.AppendLine("  , PE.inviato AS Expr4 ")
            stb.AppendLine("  , PE.Data_Creazione AS Expr5 ")
            stb.AppendLine("  , PE.Data_Modifica AS Expr6 ")
            stb.AppendLine("  , PE.Username_Creazione AS Expr7 ")
            stb.AppendLine("  , PE.Username_Modifica AS Expr8 ")
            stb.AppendLine("  , PE.Validita_Inizio AS Expr9 ")
            stb.AppendLine("  , PE.Validita_Fine AS Expr10 ")
            stb.AppendLine("  , ISNULL(PE.Num_Piante, 0) AS 'Num_Piante' ")
            stb.AppendLine("  , PE.Stato_Cod ")
            stb.AppendLine("  , PE.Ciclo ")
            stb.AppendLine("  , PE.Grva_Cod ")
            stb.AppendLine("  , PE.Data_Semina ")
            stb.AppendLine("  , PE.Data_Raccolta ")
            stb.AppendLine("  , PE.Note AS Expr11 ")
            stb.AppendLine("  , PE.Cul_Cod_Cliente ")
            stb.AppendLine("  , PE.N_fabbisogno ")
            stb.AppendLine("  , PE.Foral_Cod ")
            stb.AppendLine("  , ISNULL(( select top 1 Sem_Des from TipologieSementi (NOLOCK) tss where tss.SEM_COD = PE.Foral_Cod ), '') as " & AliasTipologie)
            stb.AppendLine("  , PE.Port_Cod ")
            stb.AppendLine("  , PE.Riferimento_Alfanumerico_Appezzamento as codice_ordine ")
            stb.AppendLine("  , PE.Imp_Cod ")
            stb.AppendLine("  , PE.Regolamento_Cod ")
            stb.AppendLine("  , PE.Disciplinare_Cod ")
            stb.AppendLine("  , PE.TRA_Fila ")
            stb.AppendLine("  , PE.SU_Fila ")
            stb.AppendLine("  , PE.MetodoProduzione_Cod ")
            stb.AppendLine("  , PE.FlagIrrigabilita ")
            stb.AppendLine("  , PE.FlagSecondoRaccolto ")
            stb.AppendLine("  , PE.Codice_Fiscale_Tecnico ")
            stb.AppendLine("  , PE.Superficie_Futura ")
            stb.AppendLine("  , PE.Operazione_Cod ")
            stb.AppendLine("  , PE.Macrouso_Cod ")
            stb.AppendLine("  , PE.Via_Stringa ")
            stb.AppendLine("  , PE.Unita_Vitata ")
            stb.AppendLine("  , PE.Validita_Inizio_Impianto                 ")
            stb.AppendLine("  , isnull((select val_cod from imprese_codici (NOLOCK) where id_cod = 1033 And imprese_codici.piva = PT.Piva) ,'') as Cod_socio  ")
            stb.AppendLine("  , isnull(PT.Stato ,'0') as stato                ")
            stb.AppendLine("  , CASE PT.Stato  ")
            stb.AppendLine("  		WHEN 0  THEN 'Inserito' ")
            stb.AppendLine("  		WHEN 4  THEN 'Parziale' ")
            stb.AppendLine("  		WHEN 1  THEN 'Associato' ")
            stb.AppendLine("  		WHEN 99 THEN 'Modificato' ")
            stb.AppendLine("  		WHEN 10 THEN 'Evaso'  ")
            stb.AppendLine("  		WHEN 11 THEN 'Accettato'  ")
            stb.AppendLine("  		WHEN 12 THEN 'Rifiutato'  ")
            stb.AppendLine("  		WHEN 15 THEN 'Da Inviare'  ")
            stb.AppendLine("  		WHEN 13 THEN 'Inviato'  ")
            stb.AppendLine("  		WHEN 14 THEN 'Ordine Creato Da eSolver'  ")
            stb.AppendLine("  		WHEN 16 THEN 'Cancellato'  ")
            stb.AppendLine("  	END as Stato_Des ")
            stb.AppendLine("  , pe.entita_Des as Note               ")
            stb.AppendLine("  , isnull((select Port_Des from Portinnesti (NOLOCK)  where Portinnesti.Port_Cod = PE.tra_fila)  , '') as portinnesto               ")
            stb.AppendLine("  , pe.Sa_Cod as disponibili  ")
            stb.AppendLine("  , '' as Progetti  ")

            stb.AppendLine("  , isNull(tPrenota.Programmazione_cod, -1) as RichiesteAssociatePresenti ")

            If isAsipo Then
                stb.AppendLine(", PE.Data_Consegna")
                stb.AppendLine(", datepart(WEEK, PE.Data_Consegna) AS 'Settimana_Consegna'")
                stb.AppendLine(", ISNULL(PE.Macrouso_Cod, 0) AS 'Qta_Seme_Effettivo'")
                stb.AppendLine(", ISNULL(PE.SupBZ_Riduzione, 0) AS 'Qta_Seme_Calcolato'")
                stb.AppendLine(", 'StatoImpianto' = CASE ")
                stb.AppendLine("		WHEN BRI.SETUP_COD = '0' Then ''")
                stb.AppendLine("		ELSE BRI.SETUP_COD ")
                stb.AppendLine("	END")
                stb.AppendLine(", PE.Data_Fioritura_Prevista")
                stb.AppendLine(", datepart(WEEK, PE.Data_Fioritura_Prevista) AS 'Settimana_Trapianto'")
                stb.AppendLine(", ImpP.Data_Fine_Prevista")
                stb.AppendLine(", datepart(WEEK, ImpP.Data_Fine_Prevista) AS 'Settimana_Raccolta'")
                stb.AppendLine(", CONCAT(C.Rag_Soc, ' ', C.Nome, ' ', C.Cognome) AS 'Rag_Soc_ditta_sementiera' ")
                stb.AppendLine(", PE.Budget_Piva")
                stb.AppendLine(", I.rag_soc AS 'Rag_Soc_Azienda'")
                stb.AppendLine(", ISNULL(PE.Qta_Seme_Evaso, 0)  As 'Qta_Seme_Evaso' ")
                'stb.AppendLine(", ISNULL(PE.Qta_Seme_Omaggio, 0) As 'Qta_Seme_Omaggio' ")
                stb.AppendLine(", CASE
                                        WHEN ISNULL(PE.Qta_Seme_Omaggio, 0) > 0 THEN 'SI'
                                        ELSE 'NO'
                                END AS 'ordine_omaggio'")
                stb.AppendLine(", PE.Note_Integrative")
                stb.AppendLine(", MP.Mat_Des")
                stb.AppendLine(", PE.Progetto_Des")
                stb.AppendLine(", GR.GruppoRaccolta_Des")
                stb.AppendLine(", CAST(ISNULL(BRI.Sup_Imp, 0) as float) as SuperficieBDG")
                stb.AppendLine(", ISNULL(Materie_Prime.Mat_Des, '') as Prodotto_Des ")
                stb.AppendLine(", ISNULL(Plateau.descrizione, '') as plateau_des")

                stb.AppendLine("  , PA.Rag_Soc_Richiedente as Richieste  ")
            Else
                stb.AppendLine("  , '' as Richieste  ")

            End If

            stb.AppendLine(" FROM    Programmazione_Testata  (NOLOCK)  PT ")
            stb.AppendLine("    inner join Programmazione_Entita (NOLOCK)  PE on PE.Programmazione_Cod = PT.Programmazione_Cod  ")


            If applicaFiltroUtentiVisibilitaAppoggio Then

                'join sulle prenotazioni associate per stabilire la visibilità
                tPrenota(stb, "inner", "tPrenotaVis", objParametri)
                stb.AppendLine(" on  tPrenotaVis.Programmazione_Cod_Padre = pt.Programmazione_Cod ")
            End If

            'filtro per i vivai, ciascuno vede il suo.
            If join_ContattiXUtentiGias Then
                stb.AppendLine("  inner join ContattiXUtentiGias (NOLOCK)  cug ")
                stb.AppendLine("      on cug.piva = '" & objParametri.PivaSuperUser & "' ")
                stb.AppendLine("      and cug.cod_contatto = PT.piva ")
                stb.AppendLine("      and cug.username = '" & objParametri.UtenteUsername & "' ")
            End If

            'join sulle prenotazioni associate all'ordine
            tPrenota(stb, "Left", "tPrenota", objParametri)
            stb.AppendLine(" on  tPrenota.Programmazione_Cod_Padre = pt.Programmazione_Cod ")

            If isAsipo Then
                stb.AppendLine(" LEFT JOIN Budget_Reg_Impianti (NOLOCK)  BRI ON ( BRI.PIVA = PE.Budget_Piva AND BRI.SA_COD = PE.Budget_Sa_Cod AND BRI.APPEZZA = PE.Budget_Appezza AND BRI.ID_REG = PE.Budget_Id_Reg AND BRI.Id_Budget = PE.Id_Budget)")
                stb.AppendLine(" LEFT JOIN Budget_Imprese_Progetti (NOLOCK)  ImpP ON ( ImpP.PIVA = PE.Budget_Piva AND ImpP.SA_COD = PE.Budget_Sa_Cod AND ImpP.APPEZZA = PE.Budget_Appezza AND ImpP.ID_REG = PE.Budget_Id_Reg AND ImpP.Id_Budget = PE.Id_Budget)")
                stb.AppendLine(" LEFT JOIN Contatti (NOLOCK)  C ON ( (C.Piva = PE.Budget_Piva OR C.Sa_Cod = -1) AND C.Cod_Contatto = PE.Codice_Fiscale_Tecnico) ")
                stb.AppendLine(" LEFT JOIN Imprese (NOLOCK)  I ON ( I.PIVA = PE.Budget_Piva) ")
                stb.AppendLine(" LEFT JOIN Materie_Prime (NOLOCK)  MP on ( MP.Mat_Cod = PE.Superficie_Futura AND MP.Elem_Cod = 10 AND MP.Sem_Cod = 1) ")
                stb.AppendLine(" LEFT JOIN Gruppi_Raccolta (NOLOCK)  GR on ( GR.GruppoRaccolta_Cod = I.GruppoRaccolta_Cod ) ")
                stb.AppendLine(" LEFT JOIN prenotazioni_associate PA on ( PA.Programmazione_cod_Padre = pt.Programmazione_Cod ) ")
                stb.AppendLine(" LEFT JOIN Materie_Prime  on ( Materie_Prime.Mat_Cod = ImpP.Mat_Cod ) ")
                stb.AppendLine(" LEFT JOIN Plateau  on ( Plateau.id_plateau = PE.id_plateau ) ")
            End If

            stb.AppendLine(" WHERE   PT.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            stb.AppendLine(" AND     PT.Validita_Inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
            stb.AppendLine(" AND     PT.Validita_fine >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")

            If Piva <> "" Then
                stb.AppendLine(" AND PT.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If

            If Tipo_Pianificazione <> enum_TipoRicetta.Non_Filtrare Then
                stb.AppendLine(" AND PT.Tipo_Pianificazione = " & Agro_vb_SaveNum(Tipo_Pianificazione))
            End If


            If xFiltroAggiuntivo <> "" Then
                stb.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo,, objParametri))
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    stb.AppendLine(" AND     PT.Inviato >= 0 ")
                    stb.AppendLine(" AND     PT.Inviato >= 0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    stb.AppendLine(" AND     PT.Inviato = -1 ")
                    stb.AppendLine(" AND     PT.Inviato = -1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                stb.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                stb.AppendLine(" order by pt.Data_Creazione desc, pt.NumColture desc, PT.Note ")
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            Scrivi_LOG(objParametri, NomeRoutine, ex.Message)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & ex.Message)
        End Try
        Return DT


    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="stb"></param>
    ''' <param name="tipoJoin">inner o left</param>
    ''' <param name="NomeAlias"></param>
    Private Shared Sub tPrenota(stb As StringBuilder, tipoJoin As String, NomeAlias As String, objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri)

        stb.Append(" " & tipoJoin)
        stb.AppendLine("  join  ( ")
        stb.AppendLine(" select min(Programmazione_Cod) as Programmazione_Cod, Programmazione_Cod_Padre ")
        stb.AppendLine(" from programmazione_testata (NOLOCK)  pe ")

        If tipoJoin.ToLower = "inner" Then
            stb.AppendLine("    inner join Utenti_Visibilita_Appoggio uva (NOLOCK) on uva.piva = pe.piva ")
            stb.AppendLine("    and uva.username = '" & objParametri.UtenteUsername & "' ")
            stb.AppendLine("    and uva.entita_cod = 1 ")
        End If

        stb.AppendLine(" where Tipo_Pianificazione = 11  ")
        stb.AppendLine(" group by Programmazione_Cod_Padre    ")

        'aggiunto ordini non associati
        If tipoJoin.ToLower = "inner" Then
            stb.AppendLine(" union  ")
            stb.AppendLine(" select 0 as Programmazione_Cod, Programmazione_Cod as Programmazione_Cod_Padre ")
            stb.AppendLine(" from programmazione_testata (NOLOCK)  pe ")
            If tipoJoin.ToLower = "inner" Then
                stb.AppendLine("    inner join Utenti_Visibilita_Appoggio uva (NOLOCK) on uva.piva = pe.piva ")
                stb.AppendLine("    and uva.username = '" & objParametri.UtenteUsername & "' ")
                stb.AppendLine("    and uva.entita_cod = 1 ")
            End If
            stb.AppendLine("            where pe.Tipo_Pianificazione = 12 ")
            stb.AppendLine(" And Not exists ( ")
            stb.AppendLine("  Select 1 ")
            stb.AppendLine("     From programmazione_testata (NOLOCK)  pe1    ")
            stb.AppendLine("  Where pe1.Programmazione_Cod_Padre = pe.Programmazione_Cod ")
            stb.AppendLine(" ) ")
        End If

        stb.AppendLine(" )  ")
        stb.AppendLine(NomeAlias)

    End Sub


    '################################################################################
    Public Function Leggi_per_Stampa_Somma_piante(
                                ByVal Validita_Inizio As Date,
                                ByVal Validita_Fine As Date,
                                ByVal Tipo_Pianificazione As enum_TipoRicetta,
                                    ByVal xFiltroAggiuntivo As String,
                                    ByVal xOrderBy As String,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                    ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Programmazione_Testata_R.Leggi()"

        Dim DT As New DataTable

        Dim stb As New System.Text.StringBuilder

        Try
            stb.Length = 0

            stb.AppendLine(" SELECT        PE.Veg_Cod   ")
            stb.AppendLine("    ,(select veg_des from SpecieVegetali s where s.veg_cod = PE.Veg_Cod) as Veg_Des ")
            stb.AppendLine("    ,PE.Cul_Cod ")
            stb.AppendLine("    ,(select Cul_Des from Cultivar  c where c.veg_cod = PE.Veg_Cod AND c.Cul_Cod = PE.Cul_Cod ) as Cul_Des ")
            stb.AppendLine("    ,isnull((select Grva_Des from GruppoVarietale  c where grva_cod = PE.Cop_cod ), N'') as Tipologia_Varietale ")
            stb.AppendLine("    ,PE.Veg_Cod_Prec  ")
            stb.AppendLine("    ,(select distinct p.MacroCategoria from Prenotazione_Piante_Categoria  p where p.ID_MacroCategoria = PE.Veg_Cod_Prec) as Categoria")
            stb.AppendLine("    , PE.Regolamento_Cod")
            stb.AppendLine("    ,isnull((select distinct Port_Des from Portinnesti p where p.Port_Cod = PE.Regolamento_Cod),'') as Portinnesto")
            stb.AppendLine("    ,sum(Num_Piante) as Somma")
            stb.AppendLine(" FROM            Programmazione_Testata PT INNER JOIN")
            stb.AppendLine("       Programmazione_Entita PE ON PT.Piva_SuperUser = PE.Piva_SuperUser AND PT.Programmazione_Cod = PE.Programmazione_Cod")


            stb.AppendLine(" WHERE   PT.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            stb.AppendLine(" AND     PT.Validita_Inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
            stb.AppendLine(" AND     PT.Validita_fine >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")


            If Tipo_Pianificazione <> enum_TipoRicetta.Non_Filtrare Then
                stb.AppendLine(" AND PT.Tipo_Pianificazione = " & Agro_vb_SaveNum(Tipo_Pianificazione))
            End If


            If xFiltroAggiuntivo <> "" Then
                stb.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo,, objParametri))
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    stb.AppendLine(" AND     PT.Inviato >= 0 ")
                    stb.AppendLine(" AND     PT.Inviato >= 0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    stb.AppendLine(" AND     PT.Inviato = -1 ")
                    stb.AppendLine(" AND     PT.Inviato = -1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            stb.AppendLine(" group by  PE.Veg_Cod, PE.Cul_Cod, PE.Veg_Cod_Prec, PE.Regolamento_Cod , PE.Cop_cod ")


            If xOrderBy <> "" Then
                stb.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                stb.AppendLine(" order by Veg_Des , Cul_Des ")
            End If



            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            Scrivi_LOG(objParametri, NomeRoutine, ex.Message)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & ex.Message)
        End Try
        Return DT


    End Function

    '################################################################################
    Public Function Leggi_per_Excel_Riepilogo(
        ByVal Validita_Inizio As Date,
        ByVal Validita_Fine As Date,
        ByVal solo As Boolean,
        ByVal xFiltroAggiuntivo As String,
        ByVal xOrderBy As String,
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
        Optional ByVal VivaioAssociato As String = "Vivaio Associato",
        Optional ByVal VivaioPreferito As String = "Vivaio Preferito",
        Optional ByVal CodFiscale As String = "Cod Fiscale",
        Optional ByVal NumeroPiante As String = "Numero Piante",
        Optional ByVal NumeroMarze As String = "Numero Marze"
    ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Programmazione_Testata_R.Leggi()"

        Dim DT As New DataTable

        Dim stb As New System.Text.StringBuilder

        Try
            stb.Length = 0

            stb.AppendLine(" select * from ( ")
            stb.AppendLine(" SELECT     year (pe.Validita_Inizio ) as anno ,pe.Programmazione_Entita_Cod ,  ")
            stb.AppendLine("        ISNULL ((SELECT     Note FROM         Programmazione_Testata AS pp_te WHERE     (Programmazione_Cod = pt.Programmazione_Cod_Padre)), '') AS [" & VivaioAssociato & "],  ")
            stb.AppendLine("        ISNULL((SELECT        Rag_Soc FROM            Contatti WHERE        (Cod_Contatto = PE.Veg_Cod_Cliente)), N'') AS [" & VivaioPreferito & "], ")
            stb.AppendLine("             ISNULL((SELECT        val_cod FROM            Imprese_Codici WHERE        (id_cod = 1033) AND (PIVA = PT.Piva)), N'') AS Cod_socio, ")
            stb.AppendLine("             (SELECT DISTINCT Descrizione_Centro_Iscrizione FROM            PDC_Stabilimenti_Apofruit AS psa WHERE        (VCCOIS = PE.Entita_Des)) AS prenotazione_stabilimento,  ")
            stb.AppendLine("   ind_des as Indirizzo,frz_des as Località,CAP as CAP,pro_cod as PR , PE.TipoZona AS prenotazione_tipo ")
            stb.AppendLine(" ,( select top 1 c.Cod_Contatto from Contatti c inner join Risorse_Umane r on c.piva= r.Piva and c.Cod_Contatto = r.Cod_Contatto  where r.Cod_Rapporto = -1  and c.Piva  =i.piva) as [" & CodFiscale & "] ")
            stb.AppendLine(" ,( select top 1 rr.numero from Contatti c inner join Risorse_Umane r on c.piva= r.Piva and c.Cod_Contatto = r.Cod_Contatto  inner join ContattiXRubrica cr on c.piva= cr.Piva and c.Cod_Contatto = cr.Cod_Contatto  inner join Rubrica rr on rr.cod_rubrica = cr.Cod_Rubrica  where c.Piva  =i.piva  and Cod_Rapporto = -1 ) as 'Telefono' ,  ")
            stb.AppendLine(" pt.numcolture as Num , PE.campo_cod as Associato , (SELECT DISTINCT MacroCategoria FROM            Prenotazione_Piante_Categoria AS ppc WHERE        (ID_Categoria = PE.Veg_Cod_Prec)) AS prenotazione_categoria, ")
            stb.AppendLine(" ISNULL((select Descrizione_Certificazione from Prenotazione_Piante_Certificazione where ID_Certificazione = PE.Id_Fre ), N'') as prenotazione_certificazione_Des ,")
            stb.AppendLine("  ISNULL((SELECT DISTINCT NomeCategoria FROM Prenotazione_Piante_Categoria AS ppc WHERE        (ID_Categoria = PE.Id_Mat_O)), N'') AS prenotazione_sottocategoria,  ")
            stb.AppendLine(" i.rag_soc AS Socio, pe.Entita_Des AS CI, i.PIVA AS Piva, s.Veg_Des AS Specie, isnull(cu.Cul_Des,'') AS Varietà, ISNULL((SELECT     Grva_Des FROM         GruppoVarietale WHERE     (Grva_Cod = pe.Cop_Cod)), N'') AS TipologiaVarietale,  ")
            stb.AppendLine("  Case When ISNULL(i.partitaIvaReale, '') = '' THEN i.PIVA ELSE i.partitaIvaReale END AS partitaIvaReale,")
            stb.AppendLine(" ISNULL ((Select DISTINCT Port_Des FROM         Portinnesti As p WHERE     (Port_Cod = pe.Regolamento_Cod)), '') AS Portinnesto,  ")
            stb.AppendLine(" (SELECT     TOP (1) Calibro FROM          Prenotazione_Piante_Calibro AS p WHERE      (pe.Id_Cod = ID_Calibro)) AS calibro, CASE pe.resa WHEN 0 THEN 'No' WHEN 1 THEN 'Si-Fallanza' WHEN 2 THEN 'Si-Gratuito' END AS Ripasso, ")
            stb.AppendLine(" pe.Num_Piante AS [" & NumeroPiante & "], pe.Grva_Cod AS [" & NumeroMarze & "], pe.Progetto_Des AS Note, ")
            stb.AppendLine("            (select top 1 Foral_des from FormeAllevamento where Foral_Cod =  PE.N_distribuito ) AS prenotazione_forma_allevamento,   ")
            stb.AppendLine("             PE.TRA_Fila, PE.SU_Fila ")
            stb.AppendLine("             , case Grfi_Cod when 0 then 'No' when -1 then 'Si' END AS OCM ")
            stb.AppendLine("             ,(SELECT Reg_DES FROM Regolamenti r where r.Reg_Cod = PE.Port_Cod) as Richiesta ")
            stb.AppendLine("             ,PT.Data_Modifica , PT.Data_Creazione ")
            stb.AppendLine("             ,'' as particelle ")

            stb.AppendLine(" FROM          Programmazione_Testata AS pt   ")
            stb.AppendLine(" INNER JOIN Programmazione_Entita AS pe ON pe.Programmazione_Cod = pt.Programmazione_Cod   ")
            stb.AppendLine(" INNER JOIN Imprese AS i ON i.PIVA = pt.Piva   ")
            stb.AppendLine(" LEFT OUTER JOIN ImpresexIndirizzi AS ii ON ii.PIVA = i.PIVA   ")
            stb.AppendLine(" LEFT OUTER JOIN Indirizzi AS ind ON ii.cod_indirizzo = ind.cod_indirizzo   ")
            stb.AppendLine(" inner JOIN SpecieVegetali AS s ON s.Veg_Cod = pe.Veg_Cod   ")
            stb.AppendLine(" LEFT JOIN Cultivar AS cu ON cu.Cul_Cod = pe.Cul_Cod AND cu.Veg_Cod = pe.Veg_Cod  ")


            stb.AppendLine(" where Tipo_Pianificazione =11 ")

            stb.AppendLine(" AND     PT.Validita_Inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
            stb.AppendLine(" AND     PT.Validita_fine >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")




            If xFiltroAggiuntivo <> "" Then
                stb.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo,, objParametri))
            End If


            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    stb.AppendLine(" AND     PT.Inviato >= 0 ")
                    stb.AppendLine(" AND     PT.Inviato >= 0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    stb.AppendLine(" AND     PT.Inviato = -1 ")
                    stb.AppendLine(" AND     PT.Inviato = -1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------


            stb.AppendLine(" ) i ")



            If solo = True Then
                stb.AppendLine(" where vivaio <> '' ")
            End If





            If xOrderBy <> "" Then
                stb.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                stb.AppendLine(" order by [" & VivaioAssociato & "], Socio  ")
            End If




            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            Scrivi_LOG(objParametri, NomeRoutine, ex.Message)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & ex.Message)
        End Try
        Return DT


    End Function


    '################################################################################
    Public Function Leggi_per_Stampa_Fatturazione(
                                ByVal Validita_Inizio As Date,
                                ByVal Validita_Fine As Date,
                                ByVal Piva As String,
                                ByVal Stabilimento As String,
                                ByVal Vivaio As String,
                                    ByVal xFiltroAggiuntivo As String,
                                    ByVal xOrderBy As String,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                Optional ByVal CodFiscale As String = "Cod Fiscale",
                                Optional ByVal NumeroPiante As String = "Numero Piante"
                                    ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Programmazione_Testata_R.Leggi()"

        Dim DT As New DataTable

        Dim stb As New System.Text.StringBuilder

        Try
            stb.Length = 0

            stb.Append(" select * from ( " & vbCrLf)
            stb.Append(" select " & vbCrLf)

            stb.Append(" pe.Programmazione_Entita_Cod, " & vbCrLf)

            stb.Append(" isnull((select note from Programmazione_Testata pp_te where pp_te.Programmazione_Cod =pt.Programmazione_Cod_Padre ),'') as vivaio, " & vbCrLf)
            stb.Append(" isnull((select piva from Programmazione_Entita pp_te where pp_te.Programmazione_Cod =pt.Programmazione_Cod_Padre ),'') as vivaio_piva, " & vbCrLf)

            'stb.Append("ISNULL((SELECT        Rag_Soc FROM            Contatti WHERE        (Cod_Contatto = PE.Veg_Cod_Cliente)), N'') AS vivaio, " & vbCrLf)
            'stb.Append("  ISNULL((SELECT        Cod_Contatto FROM            Contatti WHERE        (Cod_Contatto = PE.Veg_Cod_Cliente)), N'') AS vivaio_piva,  " & vbCrLf)

            stb.Append(" i.rag_soc as Socio, pe.entita_des as CI, ind_des as Indirizzo,case when frz_des<>'' then frz_des + ' ' else '' end + ISNULL(i1.localita, ind.com_des) as Località, ind.CAP as CAP,pro_cod as PR ,i.piva as Piva " & vbCrLf)
            stb.Append(" ,Case When ISNULL(i.partitaIvaReale, '') = '' THEN i.PIVA ELSE i.partitaIvaReale END AS partitaIvaReale " & vbCrLf)
            stb.Append(" ,( select top 1 c.Cod_Contatto from Contatti c inner join Risorse_Umane r on c.piva= r.Piva and c.Cod_Contatto = r.Cod_Contatto  where r.Cod_Rapporto = -1  and c.Piva  =i.piva) as [" & CodFiscale & "] " & vbCrLf)
            stb.Append(" ,( select top 1 rr.numero from Contatti c inner join Risorse_Umane r on c.piva= r.Piva and c.Cod_Contatto = r.Cod_Contatto  inner join ContattiXRubrica cr on c.piva= cr.Piva and c.Cod_Contatto = cr.Cod_Contatto  inner join Rubrica rr on rr.cod_rubrica = cr.Cod_Rubrica  where c.Piva  =i.piva  and Cod_Rapporto = -1 ) as 'Telefono'  " & vbCrLf)
            stb.Append(" ,s.veg_des as Specie , cu.cul_des as Varietà  " & vbCrLf)

            stb.Append(" ,ISNULL((select grva_des from GruppoVarietale where grva_cod= PE.cop_cod ), N'') AS TipologiaVarietale  " & vbCrLf)

            stb.Append(" ,isnull((select distinct Port_Des from Portinnesti p where p.Port_Cod = PE.Regolamento_Cod),'') as Portinnesto " & vbCrLf)
            stb.Append(" , CASE pe.id_fre WHEN 1 THEN 'V.E.' WHEN 2 THEN 'B.B.'  WHEN 3 THEN 'CAC' END  as Categoria, (select top 1 calibro from Prenotazione_Piante_Calibro p where pe.id_cod = p.ID_Calibro )  as calibro " & vbCrLf)
            stb.Append(" , CASE pe.resa WHEN 0 THEN 'No' WHEN 1 THEN 'Si-Fallanza'  WHEN 2 THEN 'Si-Gratuito' END  as Ripasso " & vbCrLf)
            stb.Append(" , Num_Piante as [" & NumeroPiante & "] ,Progetto_des as Note " & vbCrLf)

            stb.AppendLine(" , ISNULL(( select top 1 Sem_Des from TipologieSementi tss where tss.SEM_COD = PE.Foral_Cod ), '') as Tipologie")


            stb.Append(" from Programmazione_Testata pt " & vbCrLf)
            stb.Append(" inner join Programmazione_Entita pe on pe.Programmazione_Cod = pt.Programmazione_Cod  " & vbCrLf)

            stb.Append(" inner join Imprese i on i.piva = pt.piva  " & vbCrLf)
            stb.Append(" LEFT JOIN ImpresexIndirizzi ii on ii.piva = i.piva  " & vbCrLf)
            stb.Append(" LEFT JOIN Indirizzi  ind on ii.cod_indirizzo = ind.cod_indirizzo  " & vbCrLf)
            stb.Append(" LEFT JOIN ISTAT i1 on i1.PROV =ind.pro_cod_istat and  i1.COM = ind.com_cod_istat  " & vbCrLf)
            stb.Append(" inner join SpecieVegetali s on s.veg_cod = pe.veg_cod " & vbCrLf)
            stb.Append(" LEFT JOIN cultivar cu on cu.Cul_Cod =pe.cul_cod and cu.veg_cod =pe.veg_cod  " & vbCrLf)

            stb.Append(" where Tipo_Pianificazione = 11 " & vbCrLf)




            'stb.Append(" WHERE   PT.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            stb.Append(" AND     PT.Validita_Inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & " " & vbCrLf)
            stb.Append(" AND     PT.Validita_fine >= " & Agro_SQL_SaveDate(Validita_Inizio) & " " & vbCrLf)

            If Piva <> "" Then
                stb.Append(" AND      i.piva = '" & Agro_SQL_SaveText(Piva) & "' " & vbCrLf)
            End If

            If Stabilimento <> "" Then
                stb.Append(" AND      pe.entita_des= '" & Agro_SQL_SaveText(Stabilimento) & "' " & vbCrLf)
            End If

            'If Piva <> "" Then
            '    stb.Append(" AND      pe.Piva= '" & Agro_SQL_SaveText(Piva) & "' ")
            'End If


            If xFiltroAggiuntivo <> "" Then
                stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo,, objParametri) & vbCrLf)
            End If


            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    stb.Append(" AND     PT.Inviato >= 0 ")
                    stb.Append(" AND     PT.Inviato >= 0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    stb.Append(" AND     PT.Inviato = -1 ")
                    stb.Append(" AND     PT.Inviato = -1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------


            stb.Append(" ) i ")




            If Vivaio <> "" Then
                stb.Append(" where vivaio_piva = '" & Agro_SQL_SaveText(Vivaio) & "'")
            End If


            If xOrderBy <> "" Then
                stb.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                stb.Append(" order by vivaio , Socio  ")
            End If




            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            Scrivi_LOG(objParametri, NomeRoutine, ex.Message)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & ex.Message)
        End Try
        Return DT


    End Function




    '################################################################################
    Public Function Leggi_X_Albero(
                            ByVal Programmazione_Cod As Integer,
                            ByVal PivaPadre As String,
                            ByVal Piva As String,
                            ByVal Sa_Cod As Integer,
                                ByVal Validita_Inizio As Date,
                                ByVal Validita_Fine As Date,
                                    ByVal xFiltroAggiuntivo As String,
                                    ByVal xOrderBy As String,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                    ) As DataTable
        Dim MessaggioErrore As String
        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Programmazione_Testata_R.Leggi()"

        Dim DT As New DataTable

        Dim StrSQL As New System.Text.StringBuilder

        Try

            StrSQL.Length = 0

            StrSQL.Append(" SELECT   DISTINCT  Programmazione_Testata.*, Programmazione_Entita.Codice_Fiscale_Tecnico  ")
            StrSQL.Append(" FROM         Programmazione_Testata INNER JOIN ")
            StrSQL.Append(" Programmazione_Entita ON Programmazione_Testata.Piva_SuperUser = Programmazione_Entita.Piva_SuperUser AND  ")
            StrSQL.Append(" Programmazione_Testata.Programmazione_Cod = Programmazione_Entita.Programmazione_Cod ")

            StrSQL.Append(" WHERE   Programmazione_Testata.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.Append(" AND     Programmazione_Testata.Validita_Inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
            StrSQL.Append(" AND     Programmazione_Testata.Validita_fine >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")


            If Programmazione_Cod <> 0 Then
                StrSQL.Append(" AND Programmazione_Testata.Programmazione_Cod = " & Agro_SQL_SaveNum(Programmazione_Cod.ToString))
            End If

            If PivaPadre <> "" Then
                StrSQL.Append(" AND Programmazione_Entita.Codice_Fiscale_Tecnico = '" & Agro_SQL_SaveText(PivaPadre) & "'")
            End If



            StrSQL.Append(" AND Programmazione_Entita.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")


            StrSQL.Append(" AND Programmazione_Entita.Sa_Cod  = " & Agro_vb_SaveNum(Sa_Cod) & " ")


            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo,, objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND     Programmazione_Testata.Inviato >= 0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND     Programmazione_Testata.Inviato = -1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
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

    Public Function EsistePlanning_xPiva(
                            ByRef MessaggioErrore As String,
                            ByRef Programmazione_Cod As Integer,
                            ByVal Piva As String,
                                ByVal Tipo_Pianificazione As enum_TipoPianificazione,
                                    ByVal xFiltroAggiuntivo As String,
                                    ByVal xOrderBy As String,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                    Optional ByRef NumeroValidazione As String = "",
                                    Optional ByRef DataValidazione As Date = AGRODATAINIZIO
                                    ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Programmazione_Testata_R.EsistePlanning_xPiva()"
        Dim DT As New DataTable

        Dim stb As New System.Text.StringBuilder

        Dim Presente As Boolean = False

        Try

            stb.Length = 0

            stb.Append(" select  " & vbCrLf)
            stb.Append("    tt.* " & vbCrLf)
            stb.Append("    , coalesce(a.Allegati_Documenti_Numero, '') as Allegati_Documenti_Numero " & vbCrLf)
            stb.Append("    , coalesce(a.Validazione_Data, cast('01/01/1900' as date)) as Validazione_Data " & vbCrLf)
            stb.Append("    , coalesce(a.Allegati_Documenti_NomeFile, '') as Allegati_Documenti_NomeFile " & vbCrLf)

            stb.Append(" from Programmazione_Testata   tt " & vbCrLf)
            stb.Append("    LEFT JOIN (select distinct  Programmazione_Cod, Allegati_Documenti_SuperUser, Allegati_Documenti_Cod  from  Allegati_EntitaxDocumenti) e  " & vbCrLf)
            stb.Append("        on tt.Programmazione_Cod  = e.Programmazione_Cod  " & vbCrLf)
            stb.Append("        and tt.Piva_SuperUser = e.Allegati_Documenti_SuperUser  " & vbCrLf)
            stb.Append("    LEFT JOIN Allegati_Documenti a " & vbCrLf)
            stb.Append("        on a.Allegati_Documenti_Cod = e.Allegati_Documenti_Cod  " & vbCrLf)
            stb.Append("        and a.Allegati_Documenti_SuperUser = e.Allegati_Documenti_SuperUser  " & vbCrLf)

            stb.Append(" WHERE   tt.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")


            If Programmazione_Cod <> 0 Then
                stb.Append(" AND tt.Programmazione_Cod = " & Agro_SQL_SaveNum(Programmazione_Cod.ToString))
            End If

            If Piva <> "" Then
                stb.Append(" AND tt.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If


            If Tipo_Pianificazione <> enum_TipoRicetta.Non_Filtrare Then
                stb.Append(" AND tt.Tipo_Pianificazione =" & Agro_vb_SaveNum(Tipo_Pianificazione))
            End If

            If NumeroValidazione <> "" Then
                stb.Append(" AND a.Allegati_Documenti_Numero = " & Agro_SQL_SaveText_NULL(NumeroValidazione) & " " & vbCrLf)
            End If

            If DataValidazione <> AGRODATAINIZIO Then
                stb.Append(" AND a.Validazione_Data = " & Agro_SQL_SaveDateTime_NULL(DataValidazione) & " " & vbCrLf)
            End If

            If xFiltroAggiuntivo <> "" Then
                stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo,, objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    stb.Append(" AND     tt.Inviato >= 0 ")
                    stb.Append(" AND     tt.Inviato >= 0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    stb.Append(" AND     tt.Inviato = -1 ")
                    stb.Append(" AND     tt.Inviato = -1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                stb.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                stb.Append(" ORDER BY tt.Programmazione_Des ASC ")
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            If Not DT Is Nothing AndAlso DT.Rows.Count > 0 Then
                Programmazione_Cod = DT.Rows(0).Item("programmazione_cod")
                NumeroValidazione = DT.Rows(0).Item("Allegati_Documenti_Numero")
                DataValidazione = DT.Rows(0).Item("Validazione_Data")
                Presente = True
            End If

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Return Presente
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return Presente


    End Function


    Public Function Planning_Bloccato(ByRef Programmazione_Cod As Integer,
                                      ByVal Piva As String,
                                    ByVal xFiltroAggiuntivo As String,
                                    ByVal xOrderBy As String,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                    ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Programmazione_Testata_R.Planning_Bloccato()"
        Dim DT As New DataTable

        Dim stb As New System.Text.StringBuilder

        Dim Presente As Boolean = False

        Try

            stb.Length = 0

            stb.Append(" SELECT * FROM Programmazione_Testata")


            stb.Append(" WHERE   Programmazione_Testata.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")


            If Programmazione_Cod <> 0 Then
                stb.Append(" AND Programmazione_Testata.Programmazione_Cod = " & Agro_SQL_SaveNum(Programmazione_Cod.ToString))
            End If

            If Piva <> "" Then
                stb.Append(" AND Programmazione_Testata.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If

            If xFiltroAggiuntivo <> "" Then
                stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo,, objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    stb.Append(" AND     Programmazione_Testata.Inviato >= 0 ")
                    stb.Append(" AND     Programmazione_Testata.Inviato >= 0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    stb.Append(" AND     Programmazione_Testata.Inviato = -1 ")
                    stb.Append(" AND     Programmazione_Testata.Inviato = -1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                stb.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                stb.Append(" ORDER BY Programmazione_Testata.Programmazione_Des ASC ")
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            If Not DT Is Nothing AndAlso DT.Rows.Count > 0 Then
                If IsDBNull(DT.Rows(0)("Importato_Automaticamente")) Then
                    Return False
                End If
                If (DT.Rows(0)("Importato_Automaticamente") = 1) Then
                    Return True
                Else
                    Return False
                End If
            Else
                Return False
            End If

        Catch ex As Exception
            Return False
        End Try

        Return Presente


    End Function


    ''' <summary>
    ''' Legge i record nello stato accettato (ovvero da inviare), oppure quelli modificati/cancellati dopo l'invio
    ''' </summary>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="xOrderBy"></param>
    ''' <param name="objParametri"></param>
    ''' <returns></returns>
    Public Function LeggiEsportazioneMaterialeVivaisticoOrdini(
            ByVal xFiltroAggiuntivo As String,
            ByVal xOrderBy As String,
            ByRef objParametri As AgronicaCoreParametri) As DataTable
        
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Programmazione_Testata_R.LeggiEsportazioneMaterialeVivaisticoOrdini()"

        Dim DT As DataTable
        Dim strSql As New StringBuilder()

        Try
            strSql.Length = 0

            strSql.AppendLine(";with cte_ultimo_id_invio(dettaglio1, id) as (")
            strSql.AppendLine("	select dettaglio1, max(id)")
            strSql.AppendLine("	from Agronica_Log_Invio_Chiamate")
            strSql.AppendLine("	where Tipo_Esportazione = " & enum_Esportazioni_Sistema_Cod.AsipoOrdiniMVExport)
            strSql.AppendLine("	group by dettaglio1")
            strSql.AppendLine("),")
            strSql.AppendLine("cte_ultimo_log_invio(ID,Tipo_Esportazione,Dati_Inviati,Data_Invio,Esito,Dati_Ricevuti,Controllata,Tipo_Operazione,Dettaglio1,Dettaglio2) as (")
            strSql.AppendLine("	select Agronica_Log_Invio_Chiamate.ID,")
            strSql.AppendLine("	Tipo_Esportazione,")
            strSql.AppendLine("	Dati_Inviati,")
            strSql.AppendLine("	Data_Invio,")
            strSql.AppendLine("	Esito,")
            strSql.AppendLine("	Dati_Ricevuti,")
            strSql.AppendLine("	Controllata,")
            strSql.AppendLine("	Tipo_Operazione,")
            strSql.AppendLine("	Agronica_Log_Invio_Chiamate.Dettaglio1,")
            strSql.AppendLine("	Agronica_Log_Invio_Chiamate.Dettaglio2")
            strSql.AppendLine("	from Agronica_Log_Invio_Chiamate")
            strSql.AppendLine("	inner join cte_ultimo_id_invio")
            strSql.AppendLine("	on cte_ultimo_id_invio.id = Agronica_Log_Invio_Chiamate.ID")
            strSql.AppendLine(")")
            strSql.AppendLine("select ")
            strSql.AppendLine("Programmazione_Testata_Ord.Programmazione_Cod,")
            strSql.AppendLine("Programmazione_Testata_Rich.Piva as Piva_Azienda_Agricola_OLD,")
            strSql.AppendLine("Programmazione_Entita.Budget_Piva as Piva_Azienda_Agricola,")
            strSql.AppendLine("Programmazione_Testata_Ord.Piva as Piva_Vivaio,")
            strSql.AppendLine("Programmazione_Entita.Data_Semina as Data_Ordine_Gias,")
            strSql.AppendLine("Programmazione_Entita.Riferimento_Alfanumerico_Appezzamento as Numero_Ordine_Gias,")
            strSql.AppendLine("Programmazione_Entita.Superficie_Futura as Mat_Cod,")
            strSql.AppendLine("Materie_Prime.Cod_Articolo,")
            strSql.AppendLine("Programmazione_Entita.Macrouso_Cod as Qta_Semente,")
            strSql.AppendLine("Programmazione_Entita.Num_Piante as Qta_Piante,")
            strSql.AppendLine("Programmazione_Entita.Data_Fioritura_Prevista as Data_Trapianto_Prevista,")
            strSql.AppendLine("DATEPART(WEEK, Programmazione_Entita.Data_Fioritura_Prevista) as Settimana_Trapianto_Prevista,")
            strSql.AppendLine("Programmazione_Entita.Data_Consegna as Data_Consegna_Prevista,")
            strSql.AppendLine("Programmazione_Testata_Ord.Stato,")
            strSql.AppendLine("Programmazione_Entita.Qta_Seme_Evaso,")
            strSql.AppendLine("Programmazione_Entita.Qta_Seme_Omaggio,")
            strSql.AppendLine("cte_ultimo_log_invio.*")
            strSql.AppendLine("from Programmazione_Entita")
            strSql.AppendLine("inner join Programmazione_Testata as Programmazione_Testata_Ord")
            strSql.AppendLine("on Programmazione_Testata_Ord.Programmazione_Cod = Programmazione_Entita.Programmazione_Cod")
            strSql.AppendLine("left join Programmazione_Testata as Programmazione_Testata_Rich")
            strSql.AppendLine("on Programmazione_Testata_Rich.Programmazione_Cod_Padre = Programmazione_Testata_Ord.Programmazione_Cod")
            strSql.AppendLine("inner join Materie_Prime")
            strSql.AppendLine("on Materie_Prime.Mat_Cod = Programmazione_Entita.Superficie_Futura")
            strSql.AppendLine("left join cte_ultimo_log_invio")
            strSql.AppendLine("on Programmazione_Entita.Programmazione_Cod = cte_ultimo_log_invio.Dettaglio1")
            strSql.AppendLine("where 1=1")
            strSql.AppendLine("and Programmazione_Testata_Ord.Tipo_Pianificazione = " & enum_TipoPianificazione.Pianificazione_OrdinePiante)
            strSql.AppendLine("and Programmazione_Testata_Ord.Stato IN (" & enum_PrenotazionePiante_Stato.o_da_inviare &
                              ", " & enum_PrenotazionePiante_Stato.o_cancellato &
                              ", " & enum_PrenotazionePiante_Stato.o_inviato & ")")
            strSql.AppendLine("and (")
            strSql.AppendLine("  cte_ultimo_log_invio.id is null or ")
            strSql.AppendLine("  Programmazione_Testata_Ord.Stato = " & enum_PrenotazionePiante_Stato.o_da_inviare)
            strSql.AppendLine("  or (")
            strSql.AppendLine("    cte_ultimo_log_invio.id is not null and")
            strSql.AppendLine("    Programmazione_Testata_Ord.Stato = " & enum_PrenotazionePiante_Stato.o_inviato & " and (")
            strSql.AppendLine("    Programmazione_Entita.Data_Modifica > cte_ultimo_log_invio.Data_Invio or")
            strSql.AppendLine("    cte_ultimo_log_invio.Dettaglio2 <> '1')")
            strSql.AppendLine("  ) or (")
            strSql.AppendLine("    cte_ultimo_log_invio.id is not null and") '
            strSql.AppendLine("    Programmazione_Testata_Ord.Stato = " & enum_PrenotazionePiante_Stato.o_cancellato & " and (")
            strSql.AppendLine("    cte_ultimo_log_invio.Tipo_Operazione <> '" & enum_TipoOperazioneDB.Cancellazione & "' or (")
            strSql.AppendLine("    cte_ultimo_log_invio.Tipo_Operazione = '" & enum_TipoOperazioneDB.Cancellazione & "' and cte_ultimo_log_invio.Dettaglio2 <> '1'))")
            strSql.AppendLine("  )")
            strSql.AppendLine(")")

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine("AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.AppendLine("AND   Programmazione_Testata_Ord.Inviato >= 0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.AppendLine("AND   Programmazione_Testata_Ord.Inviato = -1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select

            If xOrderBy <> "" Then
                strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            Scrivi_LOG(objParametri, nomeRoutine, ex.Message)
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
        End Try

        Return DT

    End Function

    ''' <summary>
    ''' Legge gli ordini da aggiornare post invio ad eSolver
    ''' </summary>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="xOrderBy"></param>
    ''' <param name="objParametri"></param>
    ''' <returns></returns>
    Public Function LeggiOrdiniMV_Inviati(
            ByVal xFiltroAggiuntivo As String,
            ByVal xOrderBy As String,
            ByRef objParametri As AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Programmazione_Testata_R.LeggiOrdiniMV_Inviati()"

        Dim DT As DataTable
        Dim strSql As New StringBuilder()

        Try
            strSql.Length = 0

            strSql.AppendLine("select ")
            strSql.AppendLine("Programmazione_Entita.Programmazione_Cod,")
            strSql.AppendLine("Programmazione_Entita.Programmazione_Entita_Cod,")
            strSql.AppendLine("Programmazione_Entita.Data_Semina as Data_Ordine_Gias,")
            strSql.AppendLine("Programmazione_Entita.Riferimento_Alfanumerico_Appezzamento as Numero_Ordine_Gias,")
            strSql.AppendLine("Programmazione_Testata.Stato,")
            strSql.AppendLine("Programmazione_Entita.Macrouso_Cod as Qta_Semente,")
            strSql.AppendLine("Programmazione_Entita.Qta_Seme_Evaso,")
            strSql.AppendLine("Programmazione_Entita.Codice_Fiscale_Tecnico as Piva_Ditta_Sementiera")
            strSql.AppendLine("from Programmazione_Entita")
            strSql.AppendLine("inner join Programmazione_Testata")
            strSql.AppendLine("on Programmazione_Testata.Programmazione_Cod = Programmazione_Entita.Programmazione_Cod")
            strSql.AppendLine("where 1=1")
            strSql.AppendLine("and Programmazione_Testata.Tipo_Pianificazione = " & enum_TipoPianificazione.Pianificazione_OrdinePiante)
            strSql.AppendLine("and Programmazione_Testata.Stato IN (" & enum_PrenotazionePiante_Stato.o_inviato &
                              ", " & enum_PrenotazionePiante_Stato.o_ordine_creato_esolver &
                              ", " & enum_PrenotazionePiante_Stato.o_evaso &
                              ")")

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine("AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.AppendLine("AND   Programmazione_Testata.Inviato >= 0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.AppendLine("AND   Programmazione_Testata.Inviato = -1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select

            If xOrderBy <> "" Then
                strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, strSql.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            Scrivi_LOG(objParametri, NomeRoutine, ex.Message)
            Throw New Exception("[" & NomeRoutine & "] : " & ex.Message)
        End Try

        Return DT

    End Function

End Class




'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################


Public Class Programmazione_Testata_W
    Inherits AgronicaCoreDataProvider.DataProvider


    '################################################################################
    Public Function Scrivi(ByRef Programmazione_Cod As Integer,
                               ByVal Programmazione_Des As String,
                               ByVal Programmazione_Des_Long As String,
                               ByVal Piva As String,
                               ByVal Note As String,
                               ByVal Tipo_Pianificazione As Integer,
                               ByVal Fonte_Cod As Integer,
                                    ByVal Validita_Inizio As Date,
                                    ByVal Validita_Fine As Date,
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                    , Optional ByVal Data_creazione As Date = #2/1/1900# _
                    , Optional ByVal Data_modifica As Date = #2/1/1900# _
                    , Optional ByVal username_creazione As String = "" _
                    , Optional ByVal username_modifica As String = "" _
                    , Optional ByVal programmazione_cod_Padre As String = "" _
                    , Optional ByVal NumColture As Integer = 0 _
                    , Optional ByVal Stato As Integer = 0 _
                    , Optional ByVal Stato_SQNPI As Integer = 0 _
                    , Optional ByVal Pratica_Cod As String = "" _
                    , Optional ByVal importatoAutomaticamente As Integer = 0
                                        ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Programmazione_Testata_W.Scrivi()"

        Dim ObjSequenze As AgronicaCoreDataProvider.Agro_Sequenze

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '----- Ricavo il codice in Sequenza_Tabelle
            If Programmazione_Cod = 0 Then

                ObjSequenze = New AgronicaCoreDataProvider.Agro_Sequenze

                Programmazione_Cod = ObjSequenze.NuovoId_Tabella(
                                       "Programmazione_Testata",
                                        0, 2000000000, objParametri)

                ObjSequenze = Nothing

            End If


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



            '----- Genero la query SQL 
            StrSQL.Append(" INSERT INTO Programmazione_Testata ( ")
            StrSQL.Append("                     Piva_SuperUser, Programmazione_Cod, ")
            StrSQL.Append("                     Programmazione_Des, Programmazione_Des_Long, ")
            StrSQL.Append("                     Piva, Note, Tipo_Pianificazione, Fonte_Cod, ")
            StrSQL.Append("                     Inviato,            ")
            StrSQL.Append("                     Data_Creazione,     Data_Modifica, ")
            StrSQL.Append("                     UserName_Creazione, UserName_Modifica, ")
            StrSQL.Append("                     Validita_Inizio,    Validita_Fine ")

            If programmazione_cod_Padre <> "" Then
                StrSQL.Append("                 ,programmazione_cod_Padre ")
            End If

            If NumColture <> 0 Then
                StrSQL.Append("                 ,NumColture ")
            End If

            If Stato <> 0 Then
                StrSQL.Append("                 ,Stato ")
            End If

            If Stato_SQNPI <> 0 Then
                StrSQL.Append("                 ,Stato_SQNPI ")
            End If

            If Pratica_Cod <> "" Then
                StrSQL.Append("                 ,Pratica_Cod ")
            End If

            StrSQL.Append("                 ,Importato_Automaticamente ")

            StrSQL.Append("                     ) ")
            StrSQL.Append(" VALUES ( ")
            StrSQL.Append("         '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "', ")
            StrSQL.Append("         " & Agro_SQL_SaveNum(Programmazione_Cod.ToString) & ", ")
            StrSQL.Append("         '" & Agro_SQL_SaveText(Programmazione_Des.ToString) & "', ")
            StrSQL.Append("         '" & Agro_SQL_SaveText(Programmazione_Des_Long.ToString) & "', ")
            StrSQL.Append("         '" & Agro_SQL_SaveText(Piva.ToString) & "', ")
            StrSQL.Append("         '" & Agro_SQL_SaveText(Note.ToString) & "', ")
            StrSQL.Append("         " & Agro_SQL_SaveNum(Tipo_Pianificazione.ToString) & ", ")
            StrSQL.Append("         " & Agro_SQL_SaveNum(Fonte_Cod.ToString) & " ")
            StrSQL.Append("         , 0  ")

            StrSQL.Append("			, " & Agro_SQL_SaveDateTime(Data_creazione) & "  ")
            StrSQL.Append("			, " & Agro_SQL_SaveDateTime(Data_modifica) & "  ")
            StrSQL.Append("			,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            StrSQL.Append("			,'" & Agro_SQL_SaveText(username_modifica) & "' ")



            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")

            If programmazione_cod_Padre <> "" Then
                StrSQL.Append("         , " & Agro_SQL_SaveNum(programmazione_cod_Padre) & "  ")
            End If

            If NumColture <> 0 Then
                StrSQL.Append("         , " & Agro_SQL_SaveNum(NumColture) & "  ")
            End If

            If Stato <> 0 Then
                StrSQL.Append("         , " & Agro_SQL_SaveNum(Stato) & "  ")
            End If

            If Stato_SQNPI <> 0 Then
                StrSQL.Append("         , " & Agro_SQL_SaveNum(Stato_SQNPI) & "  ")
            End If

            If Pratica_Cod <> "" Then
                StrSQL.Append("         , " & Agro_SQL_SaveText_NULL(Pratica_Cod) & "  ")
            End If

            StrSQL.Append("         , " & Agro_SQL_SaveNum(importatoAutomaticamente) & "  ")

            StrSQL.Append("         ) ")


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

    '################################################################################
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' AgronicaCoreAnagrafeDAL.Programmazione_Testata_W.Modifica
    ''' </summary>
    ''' <param name="Programmazione_Cod">Campo Obbligatorio</param>
    ''' <param name="Programmazione_Des">StrDefault_per_MODIFICA</param>
    ''' <param name="Programmazione_Des_Long">StrDefault_per_MODIFICA</param>
    ''' <param name="Piva">StrDefault_per_MODIFICA</param>
    ''' <param name="Note">StrDefault_per_MODIFICA</param>
    ''' <param name="Validita_Inizio">DataDefault_per_MODIFICA</param>
    ''' <param name="Validita_Fine">DataDefault_per_MODIFICA</param>
    ''' <returns>True --> buon fine , False --> errore</returns>
    ''' -----------------------------------------------------------------------------
    Public Function Modifica(
                               ByVal Programmazione_Cod As Integer,
                               ByVal Programmazione_Des As String,
                               ByVal Programmazione_Des_Long As String,
                               ByVal Piva As String,
                               ByVal Note As String,
                                    ByVal Validita_Inizio As Date,
                                    ByVal Validita_Fine As Date,
                                    ByVal xFiltroAggiuntivo As String,
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                            ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Programmazione_Testata_W.Modifica()"

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

            If Programmazione_Cod = 0 Then
                Throw New Exception("Parametro non corretto nella query (Programmazione_Cod obbligatorio)")
            End If

            If Programmazione_Des = "" Then
                Throw New Exception("Parametro non corretto nella query (Programmazione_Des obbligatorio)")
            End If


            '---------------------------------------------
            StrSQL.Append(" UPDATE Programmazione_Testata SET  ")
            If Programmazione_Des <> StrDefault_per_MODIFICA Then
                StrSQL.Append("    Programmazione_Des     = '" & Agro_SQL_SaveText(Programmazione_Des) & "'  ,")
            End If
            If Programmazione_Des_Long <> StrDefault_per_MODIFICA Then
                StrSQL.Append("    Programmazione_Des_Long     = '" & Agro_SQL_SaveText(Programmazione_Des_Long) & "'  ,")
            End If
            If Piva <> StrDefault_per_MODIFICA Then
                StrSQL.Append("    Piva     = '" & Agro_SQL_SaveText(Piva) & "'  ,")
            End If
            If Note <> StrDefault_per_MODIFICA Then
                StrSQL.Append("    Note     = '" & Agro_SQL_SaveText(Note) & "'  ,")
            End If
            If Validita_Inizio <> DataDefault_per_MODIFICA Then
                StrSQL.Append("    Validita_Inizio     = " & Agro_SQL_SaveDate(Validita_Inizio) & "  ,")
            End If
            If Validita_Fine <> DataDefault_per_MODIFICA Then
                StrSQL.Append("    Validita_Fine     = " & Agro_SQL_SaveDate(Validita_Fine) & "  ,")
            End If

            'rimuovo l ultima virgola
            StrSQL.Remove(StrSQL.Length - 1, 1)

            StrSQL.Append(" WHERE Programmazione_Cod         = " & Agro_SQL_SaveNum(Programmazione_Cod) & "  ")
            StrSQL.Append(" AND   Piva_SuperUser =  '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'   ")
            '---------------------------------------------

            '----------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo,, objParametri))
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

    '################################################################################
    Public Function Cancella(ByVal Programmazione_Cod As Integer,
                                    ByVal xFiltroAggiuntivo As String,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                    ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Programmazione_Testata_W.Cancella()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva_SuperUser = ""         => Vengono cancellate tutte le programmazioni del db
        '   Programmazione_Cod=0        => Vengono cancellate tutte le programmazioni del Piva_SuperUser
        '
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = AgronicaCoreDataProvider.AgronicaCoreParametri.enumCancellazioneLogica.CancellazioneLogica Then

                StrSQL.Length = 0
                StrSQL.Append(" UPDATE Programmazione_Testata ")
                StrSQL.Append(" SET ")
                StrSQL.Append("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("      ,Inviato = -1 ")
                StrSQL.Append(" WHERE  Inviato >= 0")

            Else

                StrSQL.Length = 0
                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM     Programmazione_Testata ")
                StrSQL.Append(" WHERE   1=1 ")

            End If

            If objParametri.PivaSuperUser <> "" Then
                StrSQL.Append("  AND   Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            End If

            If Programmazione_Cod <> 0 Then
                StrSQL.Append("  AND   Programmazione_Cod = " & Agro_SQL_SaveNum(Programmazione_Cod) & " ")
            End If

            '---------------------------------------------

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo,, objParametri))
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


    '################################################################################
    Public Function Annulla_programmazione_cod_padre(ByVal Programmazione_Cod As Integer,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                    ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Programmazione_Testata_W.Cancella()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva_SuperUser = ""         => Vengono cancellate tutte le programmazioni del db
        '   Programmazione_Cod=0        => Vengono cancellate tutte le programmazioni del Piva_SuperUser
        '
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------

            StrSQL.Length = 0
            StrSQL.Append(" UPDATE Programmazione_Testata ")
            StrSQL.Append(" SET ")
            StrSQL.Append("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.Append("      ,Programmazione_cod_Padre = null ")
            StrSQL.Append(" WHERE  Programmazione_Cod = " & Programmazione_Cod)



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



    Public Function Modifica_Validita_Fine(
                             ByVal Programmazione_Cod As Integer,
                             ByVal Validita_Fine As Date,
                                      ByVal xFiltroAggiuntivo As String,
                                      ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                          ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Programmazione_Testata_W.Modifica_Validita_Fine()"

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

            If Programmazione_Cod = 0 Then
                Throw New Exception("Parametro non corretto nella query (Programmazione_Cod obbligatorio)")
            End If


            '---------------------------------------------
            StrSQL.Append(" UPDATE Programmazione_Testata SET  ")

            StrSQL.Append("      Validita_Fine     = " & Agro_SQL_SaveDate(Validita_Fine) & "  ")
            StrSQL.Append("    , UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.Append("    , Data_Modifica     =  " & Agro_SQL_SaveDate(Date.Now))

            StrSQL.Append(" WHERE Programmazione_Cod         = " & Agro_SQL_SaveNum(Programmazione_Cod) & "  ")
            StrSQL.Append(" AND   Piva_SuperUser =  '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'   ")

            '----------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo,, objParametri))
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

    Public Function Modifica_Validita(
                             ByVal Programmazione_Cod As Integer,
                             ByVal Validita_Inizio As Date,
                             ByVal Validita_Fine As Date,
                                      ByVal xFiltroAggiuntivo As String,
                                      ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                          ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Programmazione_Testata_W.Modifica_Validita()"

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

            If Programmazione_Cod = 0 Then
                Throw New Exception("Parametro non corretto nella query (Programmazione_Cod obbligatorio)")
            End If


            '---------------------------------------------
            StrSQL.Append(" UPDATE Programmazione_Testata SET  ")

            StrSQL.Append("    Validita_Inizio     = " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            StrSQL.Append("    , Validita_Fine     = " & Agro_SQL_SaveDate(Validita_Fine) & "  ")
            StrSQL.Append("    , UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.Append("    , Data_Modifica     =  " & Agro_SQL_SaveDate(Date.Now))

            StrSQL.Append(" WHERE Programmazione_Cod         = " & Agro_SQL_SaveNum(Programmazione_Cod) & "  ")
            StrSQL.Append(" AND   Piva_SuperUser =  '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'   ")

            '----------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo,, objParametri))
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



    Public Function Modifica_Stato(
                         ByVal Programmazione_Cod As Integer,
                         ByVal Stato As Integer,
                                  ByVal xFiltroAggiuntivo As String,
                                  ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                      ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Programmazione_Testata_W.Modifica_Validita()"

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

            If Programmazione_Cod = 0 Then
                Throw New Exception("Parametro non corretto nella query (Programmazione_Cod obbligatorio)")
            End If


            '---------------------------------------------
            StrSQL.Append(" UPDATE Programmazione_Testata SET  ")

            StrSQL.Append("    Stato     = " & Agro_SQL_SaveNum(Stato) & "  ")
            StrSQL.Append("    , UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.Append("    , Data_Modifica     =  " & Agro_SQL_SaveDateTime(Date.Now))

            StrSQL.Append(" WHERE Programmazione_Cod         = " & Agro_SQL_SaveNum(Programmazione_Cod) & "  ")
            StrSQL.Append(" AND   Piva_SuperUser =  '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'   ")

            '----------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo,, objParametri))
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


    Public Function Modifica_Stato_Programmazione_cod_padre(
                         ByVal Programmazione_Cod As Integer,
                         ByVal Programmazione_Cod_Padre As Integer,
                         ByVal Stato As Integer,
                                  ByVal xFiltroAggiuntivo As String,
                                  ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                      ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Programmazione_Testata_W.Modifica_Validita()"

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

            If Programmazione_Cod = 0 Then
                Throw New Exception("Parametro non corretto nella query (Programmazione_Cod obbligatorio)")
            End If


            '---------------------------------------------
            StrSQL.Append(" UPDATE Programmazione_Testata SET  ")

            StrSQL.Append("    Stato     = " & Agro_SQL_SaveNum(Stato) & "  ")
            StrSQL.Append("    ,Programmazione_Cod_Padre     = " & Agro_SQL_SaveNum(Programmazione_Cod_Padre) & "  ")
            StrSQL.Append("    , UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.Append("    , Data_Modifica     =  " & Agro_SQL_SaveDate(Date.Now))

            StrSQL.Append(" WHERE Programmazione_Cod         = " & Agro_SQL_SaveNum(Programmazione_Cod) & "  ")
            StrSQL.Append(" AND   Piva_SuperUser =  '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'   ")

            '----------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo,, objParametri))
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




    Public Function Modifica_Data_Modifica_stato(
                         ByVal Programmazione_Cod As Integer,
                         ByVal Stato As Integer,
                                  ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                      ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Programmazione_Testata_W.Modifica_Validita()"

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

            If Programmazione_Cod = 0 Then
                Throw New Exception("Parametro non corretto nella query (Programmazione_Cod obbligatorio)")
            End If


            '---------------------------------------------
            StrSQL.Append(" UPDATE Programmazione_Testata SET  ")

            StrSQL.Append("     UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.Append("    , Data_Modifica     =  " & Agro_SQL_SaveDate(Date.Now))
            StrSQL.Append("     ,Stato = " & Stato & " ")


            StrSQL.Append(" WHERE Programmazione_Cod         = " & Agro_SQL_SaveNum(Programmazione_Cod) & "  ")

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


    Public Function Modifica_Stato_SQNPI(
                         ByVal Programmazione_Cod As Integer,
                         ByVal Stato_SQNPI As Integer,
                                  ByVal xFiltroAggiuntivo As String,
                                  ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                      ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Programmazione_Testata_W.Modifica_Validita()"

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

            If Programmazione_Cod = 0 Then
                Throw New Exception("Parametro non corretto nella query (Programmazione_Cod obbligatorio)")
            End If


            '---------------------------------------------
            StrSQL.Append(" UPDATE Programmazione_Testata SET  ")

            StrSQL.Append("    Stato_SQNPI     = " & Agro_SQL_SaveNum(Stato_SQNPI) & "  ")
            StrSQL.Append("    , UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.Append("    , Data_Modifica     =  " & Agro_SQL_SaveDate(Date.Now))

            StrSQL.Append(" WHERE Programmazione_Cod         = " & Agro_SQL_SaveNum(Programmazione_Cod) & "  ")
            StrSQL.Append(" AND   Piva_SuperUser =  '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'   ")

            '----------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo,, objParametri))
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


    Public Function Modifica_Pratica(
                         ByVal Programmazione_Cod As Integer,
                         ByVal Pratica_Cod As String,
                                  ByVal xFiltroAggiuntivo As String,
                                  ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                      ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Programmazione_Testata_W.Modifica_Validita()"

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

            If Programmazione_Cod = 0 Then
                Throw New Exception("Parametro non corretto nella query (Programmazione_Cod obbligatorio)")
            End If


            '---------------------------------------------
            StrSQL.Append(" UPDATE Programmazione_Testata SET  ")

            StrSQL.Append("    Pratica_Cod     = " & Agro_SQL_SaveText_NULL(Pratica_Cod) & "  ")
            StrSQL.Append("    , UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.Append("    , Data_Modifica     =  " & Agro_SQL_SaveDate(DateTime.Now))

            StrSQL.Append(" WHERE Programmazione_Cod         = " & Agro_SQL_SaveNum(Programmazione_Cod) & "  ")
            StrSQL.Append(" AND   Piva_SuperUser =  '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'   ")

            '----------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo,, objParametri))
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

    Public Function Modifica_PrenotazionePiante_Ordine_Small(
        ByVal Programmazione_Cod As Integer,
        byval Programmazione_Des As String,
        byval Programmazione_Des_Long As String,
        byval Piva As String,
        byval Note As String,
        byval Stato As Integer,
        byval Tipo_Pianificazione As String,
        ByVal xFiltroAggiuntivo As String,
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
    ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Programmazione_Testata_W.Modifica_PrenotazionePianteSmall()"

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

            If Programmazione_Cod = 0 Then
                Throw New Exception("Parametro non corretto nella query (Programmazione_Cod obbligatorio)")
            End If


            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.AppendLine(" UPDATE Programmazione_Testata SET  ")

            StrSQL.AppendLine("      Programmazione_Des = '" & Agro_SQL_SaveText(Programmazione_Des) & "'  ")
            StrSQL.AppendLine("    , Programmazione_Des_Long = '" & Agro_SQL_SaveText(Programmazione_Des_Long) & "'  ")
            StrSQL.AppendLine("    , Piva = '" & Agro_SQL_SaveText(Piva) & "'")
            StrSQL.AppendLine("    , Note = '" & Agro_SQL_SaveText(Note) & "'")
            StrSQL.AppendLine("    , stato =  " & Agro_SQL_SaveNum(Stato) & " ")
            StrSQL.AppendLine("    , Tipo_Pianificazione =  " & Agro_SQL_SaveNum(Tipo_Pianificazione) & " ")

            StrSQL.AppendLine("    , UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.AppendLine("    , Data_Modifica     =  " & Agro_SQL_SaveDate(DateTime.Now))

            StrSQL.AppendLine(" WHERE Programmazione_Cod         = " & Agro_SQL_SaveNum(Programmazione_Cod) & "  ")
            StrSQL.AppendLine(" AND   Piva_SuperUser =  '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'   ")

            '----------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo,, objParametri))
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
