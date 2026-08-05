Imports System.Data.Common
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider
Imports System.Text
Imports AgronicaCoreSementieriDAL






'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
<CachedDataProviderAttribute("SpecieVegetali_R")>
Public Class SpecieVegetali_R
    Inherits AgronicaCoreDataProvider.CachedDataProvider



    ''##############################################################################################
    ''vecchia funzione, usare l'altra Leggi
    'Public Function Leggi( _
    '                        ByVal Veg_Cod As Int32, _
    '                        ByVal FinestraTemp_Inizio As Date, _
    '                        ByVal FinestraTemp_Fine As Date, _
    '                        ByRef objConnessione As DbConnection, _
    '                        ByRef objTransazione As DbTransaction, _
    '                        ByVal StringaConnessione As String, _
    '                        ByVal FlagVisibilita As Int32, _
    '                        ByVal DirectoryLOG As String, _
    '                        ByVal FileLOG As String, _
    '                        ByVal IdentificatoreUtente As String _
    '                        ) As DataTable

    '    Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.SpecieVegetali_R.Leggi()"

    '    '====================================================================================
    '    'Parametri opzionali :
    '    '   Sa_Cod = 0           =>  si leggono tutti gli impianti dell'impresa
    '    '   Appezza = 0          =>  si leggono tutti gli impianti del centro aziendale
    '    '   Id_reg = 0           =>  si leggono tutti gli impianti dell'appezzamento
    '    '
    '    '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
    '    '   DirectoryLOG = ""           =>  viene usato il valore di default
    '    '   FileLOG = ""                =>  viene usato il valore di default
    '    '====================================================================================

    '    Dim MessaggioErrore As String = ""
    '    Dim StrSQL As New System.Text.StringBuilder
    '    Dim DT As DataTable

    '    Try
    '        '---------------------------------------------
    '        StrSQL.Length = 0
    '        StrSQL.Append(" SELECT * ")
    '        StrSQL.Append(" FROM  SpecieVegetali ")

    '        StrSQL.Append(" WHERE   Validita_inizio <= " & Agro_SQL_SaveDate(FinestraTemp_Fine) & " ")
    '        StrSQL.Append(" AND     Validita_Fine >= " & Agro_SQL_SaveDate(FinestraTemp_Inizio) & " ")


    '        If Veg_Cod <> 0 Then
    '            StrSQL.Append(" AND Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod) & " ")
    '        End If

    '        Select Case FlagVisibilita
    '            Case 1  'Solo i NON CANCELLATI
    '                StrSQL.Append(" AND   Inviato >=0 ")
    '            Case 2  'Solo i CANCELLATI
    '                StrSQL.Append(" AND   Inviato =-1 ")
    '            Case 3  'TUTTI
    '                '
    '            Case Else
    '                Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
    '        End Select
    '        '---------------------------------------------
    '        'Nota: Questo ordinamento è importante per la gestione del campo.
    '        'Viene letto l'impianto più RECENTE dell'appezzamento associato al campo
    '        StrSQL.Append(" ORDER BY Veg_Des ASC ")
    '        '------------------------------

    '        DT = EseguiQuery_Lettura(objConnessione, objTransazione, StringaConnessione, StrSQL.ToString, DirectoryLOG, FileLOG, IdentificatoreUtente, NomeRoutine)

    '    Catch ex As Exception

    '        MessaggioErrore = ex.Message
    '        Scrivi_LOG(DirectoryLOG, FileLOG, IdentificatoreUtente, NomeRoutine, MessaggioErrore)
    '        DT = Nothing
    '        Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

    '    End Try

    '    Return DT

    'End Function



    'Versione che utilizza AgronicaCoreParametri
    '##############################################################################################
    <Cacheable(True)>
    Public Function Leggi(
                            ByVal Veg_Cod As Integer,
                            ByVal Gru_Cod As Integer,
                            ByVal LetteraIniziale As String,
                            ByVal StringaCerca As String,
                                ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.SpecieVegetali_R.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   Veg_Cod = 0           =>  tutti i record
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi
                    '//////////////////////////////////////////////////////////////////////
                    '//////////////////////////////////////////////////////////////////////
                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT veg_cod, veg_des ")
                    StrSQL.Append(" FROM  SpecieVegetali WITH(NOLOCK)")
                    StrSQL.Append(" WHERE   Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND     Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    'le specie vegetali che il Gias non deve caricare hanno gru_cod = -1
                    StrSQL.Append(" AND Gru_Cod <> -1 ")

                    If Gru_Cod <> 0 Then
                        StrSQL.Append(" AND Gru_Cod = " & Agro_SQL_SaveNum(Gru_Cod) & " ")
                    End If

                    If Veg_Cod <> 0 Then
                        StrSQL.Append(" AND Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod) & " ")
                    End If

                    If LetteraIniziale <> "" Then
                        StrSQL.Append(" AND Veg_Des LIKE '" & Agro_SQL_SaveText(LetteraIniziale) & "%' ")
                    End If

                    If StringaCerca <> "" Then
                        StrSQL.Append(" AND Veg_Des LIKE '%" & Agro_SQL_SaveText(StringaCerca) & "%' ")
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
                        StrSQL.Append(" ORDER BY Veg_Des ASC")
                    End If
                    '//////////////////////////////////////////////////////////////////////
                    '//////////////////////////////////////////////////////////////////////


                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta
                    '//////////////////////////////////////////////////////////////////////
                    '//////////////////////////////////////////////////////////////////////
                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT * ")
                    StrSQL.Append(" FROM  SpecieVegetali ")
                    StrSQL.Append(" WHERE   Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND     Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    'le specie vegetali che il Gias non deve caricare hanno gru_cod = -1
                    StrSQL.Append(" AND Gru_Cod <> -1 ")

                    If Gru_Cod <> 0 Then
                        StrSQL.Append(" AND Gru_Cod = " & Agro_SQL_SaveNum(Gru_Cod) & " ")
                    End If

                    If Veg_Cod <> 0 Then
                        StrSQL.Append(" AND Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod) & " ")
                    End If

                    If LetteraIniziale <> "" Then
                        StrSQL.Append(" AND Veg_Des LIKE '" & Agro_SQL_SaveText(LetteraIniziale) & "%' ")
                    End If

                    If StringaCerca <> "" Then
                        StrSQL.Append(" AND Veg_Des LIKE '%" & Agro_SQL_SaveText(StringaCerca) & "%' ")
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
                        StrSQL.Append(" ORDER BY Veg_Des ASC")
                    End If
                    '//////////////////////////////////////////////////////////////////////
                    '//////////////////////////////////////////////////////////////////////



                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni



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


    'Versione che utilizza AgronicaCoreParametri
    '##############################################################################################
    Public Function Leggi_x_PDC(
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.SpecieVegetali_R.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   Veg_Cod = 0           =>  tutti i record
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            '//////////////////////////////////////////////////////////////////////
            '//////////////////////////////////////////////////////////////////////
            StrSQL.Length = 0
            StrSQL.Append(" SELECT * ")
            StrSQL.Append(" FROM  SpecieVegetali ")
            StrSQL.Append(" WHERE   Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.Append(" AND     Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            'le specie vegetali che il Gias non deve caricare hanno gru_cod = -1
            StrSQL.Append(" AND Gru_Cod <> -1 ")
            StrSQL.Append(" or (Gru_Cod = -1 and grsp_cod = -2) ")

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


    'Versione che utilizza AgronicaCoreParametri
    ' Nuova versione che ordina per Nome Specie vegetale e aggiunge il gruppo vegetale nella select
    ' (Paolo)
    '##############################################################################################
    Public Function Leggi_x_PDC_Modificata(
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.SpecieVegetali_R.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   Veg_Cod = 0           =>  tutti i record
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            '//////////////////////////////////////////////////////////////////////
            '//////////////////////////////////////////////////////////////////////
            StrSQL.Length = 0
            StrSQL.Append(" SELECT SV.*, GV.Gru_Des AS Gruppo_Veg_Desc ")
            StrSQL.Append(" FROM  SpecieVegetali SV ")
            StrSQL.Append(" INNER JOIN GruppoVegetale GV ON SV.Gru_Cod = GV.Gru_Cod ")
            StrSQL.Append(" WHERE   SV.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.Append(" AND     SV.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            'le specie vegetali che il Gias non deve caricare hanno gru_cod = -1
            StrSQL.Append(" AND SV.Gru_Cod <> -1 ")
            StrSQL.Append(" or (SV.Gru_Cod = -1 and SV.grsp_cod = -2) ")
            StrSQL.Append(" ORDER BY  SV.Veg_Des ASC")

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


    'Versione che utilizza AgronicaCoreParametri
    '##############################################################################################
    Public Function Leggi_SoloColtivate(
                                       ByVal str_Gru_cod As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.SpecieVegetali_R.Leggi_x_agronicaSementi()"

        '====================================================================================
        'Parametri opzionali :
        '   Veg_Cod = 0           =>  tutti i record
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            '////////////////////////////////////////////////////////////////////// 
            StrSQL.Length = 0
            StrSQL.Append(" select distinct s.veg_cod , veg_des from SpecieVegetali s ")
            StrSQL.Append(" inner join Cultivar c on c.Veg_Cod = s.Veg_Cod ")
            StrSQL.Append(" inner join reg_impianti r on r.CUL_COD = c.Cul_Cod ")

            StrSQL.Append(" WHERE   Gru_Cod in (" & Agro_SQL_Save_Clausola_IN(str_Gru_cod) & ")")

            StrSQL.Append(" ORDER BY Veg_Des ")

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


    'Versione che utilizza AgronicaCoreParametri
    '##############################################################################################
    Public Function Leggi_SoloColtivate(
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                Optional isBudget As Boolean = False
                                ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.SpecieVegetali_R.Leggi_x_agronicaSementi()"

        '====================================================================================
        'Parametri opzionali :
        '   Veg_Cod = 0           =>  tutti i record
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            '////////////////////////////////////////////////////////////////////// 
            StrSQL.Length = 0
            StrSQL.Append(" select distinct s.veg_cod , veg_des from SpecieVegetali s ")
            StrSQL.Append(" inner join Cultivar c on c.Veg_Cod = s.Veg_Cod ")
            StrSQL.Append(" inner join ").Append(If(isBudget, "Budget_", "")).Append("reg_impianti r on r.CUL_COD = c.Cul_Cod ")

            StrSQL.Append(" ORDER BY Veg_Des ")

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



    'Versione che utilizza AgronicaCoreParametri
    '##############################################################################################
    Public Function Leggi_x_agronicaSementi(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.SpecieVegetali_R.Leggi_x_agronicaSementi()"

        '====================================================================================
        'Parametri opzionali :
        '   Veg_Cod = 0           =>  tutti i record
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            '////////////////////////////////////////////////////////////////////// 
            StrSQL.Length = 0
            StrSQL.Append(" SELECT * " & vbCrLf)
            StrSQL.Append(" FROM  SpecieVegetali " & vbCrLf)
            StrSQL.Append(" WHERE   Gru_Cod in (1, 2, 3) " & vbCrLf)
            StrSQL.Append(" AND     Veg_Cod in ( SELECT DISTINCT Veg_Cod FROM Mappatura_Specie ) " & vbCrLf)
            StrSQL.Append(" ORDER BY Veg_Des ")

            '6,5000021,70,69,87,104,9,14,72,227,82,11,77,81,12,80,83,78,16,66,13,31,57 

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


    'Versione che utilizza AgronicaCoreParametri
    '##############################################################################################
    Public Function Leggi_con_Cul_Des(
                            ByVal Veg_Cod As Integer,
                            ByVal Cul_Cod As Integer,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.SpecieVegetali_R.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   Veg_Cod = 0           =>  tutti i record
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            '//////////////////////////////////////////////////////////////////////
            '//////////////////////////////////////////////////////////////////////
            StrSQL.Length = 0
            StrSQL.Append(" SELECT     SpecieVegetali.Veg_Cod, SpecieVegetali.Veg_Des, SpecieVegetali.Gru_Cod, Cultivar.Cul_Cod, Cultivar.Cul_Des ")
            StrSQL.Append(" FROM         SpecieVegetali INNER JOIN ")
            StrSQL.Append("         Cultivar ON SpecieVegetali.Veg_Cod = Cultivar.Veg_Cod ")
            StrSQL.Append(" WHERE   1=1 ")

            'le specie vegetali che il Gias non deve caricare hanno gru_cod = -1 
            StrSQL.Append(" AND Gru_Cod <> -1 ")


            If Veg_Cod <> 0 Then
                StrSQL.Append(" AND SpecieVegetali.Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod) & " ")
            End If

            If Cul_Cod <> 0 Then
                StrSQL.Append(" AND Cultivar.Cul_Cod = " & Agro_SQL_SaveNum(Cul_Cod) & " ")
            End If
            '-------------------------------------------------------------------------- 
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '-------------------------------------------------------------------------- 
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   SpecieVegetali.Inviato >=0 ")
                    StrSQL.Append(" AND   Cultivar.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   SpecieVegetali.Inviato =-1 ")
                    StrSQL.Append(" AND   Cultivar.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '-------------------------------------------------------------------------- 
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY Veg_Des ASC")
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



    '################################################################################
    Public Function VegDes_from_VegCod(ByVal Veg_Cod As Integer,
                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                            ) As String


        Dim dt As DataTable

        dt = Leggi(Veg_Cod,
                                0,
                                "",
                                "",
                                AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                "",
                                "",
                                objParametri)

        If Not IsNothing(dt) Then
            If dt.Rows.Count <> 0 Then
                Return dt.Rows(0).Item("Veg_Des")
            Else
                Return ""
            End If
        Else
            Return ""
        End If


    End Function

    Public Function VegDes_from_VegCod_Massivo(ByVal List_Veg_Cod As List(Of Integer),
                                               ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                               ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreMetaschemaDAL.SpecieVegetali_R.VegDes_from_VegCod_Massivo()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessione(True, objParametri)

            EseguiQuery_Scrittura(objParametri, CreaTabellaTemp_FiltroVegCod(), NomeRoutine)

            If Not IsNothing(List_Veg_Cod) AndAlso List_Veg_Cod.Any Then
                Dim chunks = ChunkBy(Of Integer)(List_Veg_Cod, 1000)
                For Each chunk In chunks
                    StrSQL.AppendLine("INSERT INTO #TempVegCod (Veg_Cod) VALUES ")
                    For Each p As String In chunk
                        StrSQL.AppendLine(String.Format("('{0}'),", p))
                    Next
                    Dim strSqlInsert As String = StrSQL.ToString
                    strSqlInsert = strSqlInsert.Remove(strSqlInsert.LastIndexOf(","))
                    StrSQL.Clear()
                    EseguiQuery_Scrittura(objParametri, strSqlInsert, NomeRoutine)
                Next
            End If

            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append(" SELECT SpecieVegetali.Veg_Cod, SpecieVegetali.Veg_Des ")
            StrSQL.Append(" FROM SpecieVegetali ")
            If List_Veg_Cod IsNot Nothing AndAlso List_Veg_Cod.Count > 0 Then
                StrSQL.AppendLine("	INNER JOIN #TempVegCod temp (NOLOCK) ON SpecieVegetali.Veg_Cod = temp.Veg_Cod ")
            End If

            StrSQL.Append(" WHERE 1 = 1 ")

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            ' Eliminazione tabella temporanea
            EseguiQuery_Scrittura(objParametri, EliminaTabellaTemp_FiltroVegCod, NomeRoutine)

            'commit transazione
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(1, objParametri)


        Catch ex As Exception
            ' Rollback
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri)

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        Finally
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessione(objParametri)
        End Try


        Return DT
    End Function

    Private Function CreaTabellaTemp_FiltroVegCod() As String
        Dim stb As New StringBuilder

        stb.AppendLine(" IF OBJECT_ID('tempdb.dbo.#TempVegCod') IS NULL BEGIN ")
        stb.AppendLine("    CREATE TABLE #TempVegCod ( ")
        stb.AppendLine("        Veg_Cod INT NULL")
        stb.AppendLine("    )")
        stb.AppendLine(" END ")

        Return stb.ToString()
    End Function

    Private Function EliminaTabellaTemp_FiltroVegCod() As String
        Dim stb As New StringBuilder

        stb.AppendLine(" IF NOT OBJECT_ID('tempdb.dbo.#TempVegCod') IS NULL BEGIN ")
        stb.AppendLine("    DROP TABLE #TempVegCod ")
        stb.AppendLine(" END ")

        Return stb.ToString()
    End Function


    '################################################################################
    Public Function GruCod_and_VegDes_from_VegCod(ByRef Veg_Des As String,
                                                ByVal Veg_Cod As Integer,
                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                            ) As String


        Dim dt As DataTable

        dt = Leggi(
            Veg_Cod,
            0,
            "",
            "",
            AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
            "",
            "",
            objParametri
            )


        If Not IsNothing(dt) Then
            If dt.Rows.Count <> 0 Then
                Veg_Des = dt.Rows(0).Item("Veg_Des")
                Return dt.Rows(0).Item("Gru_Cod")
            Else
                Return ""
            End If
        Else
            Return ""
        End If


    End Function

    Private Sub ComposeFilterForSementieri(
                                          ByRef stbQuery As System.Text.StringBuilder,
                                          ByVal id_Specie As Integer,
                                          ByVal dbServer As String
                                          )
        stbQuery.AppendLine("        INNER JOIN (")
        stbQuery.AppendLine("            SELECT DISTINCT veg.veg_cod, veg.veg_des")
        stbQuery.AppendLine("")
        stbQuery.AppendLine("            FROM [" & dbServer & "].dbo.Sementieri_ClassiDiSpecieVegetali ss")
        stbQuery.AppendLine("                INNER JOIN [" & dbServer & "].dbo.Mappatura_Specie mp ON mp.ID_Specie = ss.id_specie")
        stbQuery.AppendLine("                    AND mp.ID_Sottospecie = ss.ID_Sottospecie")
        stbQuery.AppendLine("                    AND mp.ID_Gruppo = ss.ID_Gruppo")
        stbQuery.AppendLine("                    AND mp.ID_Genotipo = ss.ID_Genotipo")
        stbQuery.AppendLine("                INNER JOIN SpecieVegetali veg ON veg.veg_cod = mp.veg_cod")
        stbQuery.AppendLine("")
        stbQuery.AppendLine("            WHERE 1=1")
        stbQuery.AppendLine("                AND mp.ID_Specie = " & id_Specie)
        stbQuery.AppendLine("")
        stbQuery.AppendLine("        ) sementi ON sementi.Veg_Cod = SpecieVegetali.Veg_Cod")
    End Sub


    '##############################################################################################
    Public Function SpecieVegetali_GestioneFiltroUtente_Leggi(
                            ByVal Veg_Cod As Integer,
                            ByVal Gru_Cod As Integer,
                            ByVal LetteraIniziale As String,
                            ByVal StringaCerca As String,
                            ByVal xFiltroAggiuntivo As String,
                            ByVal xOrderBy As String,
                            ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                            Optional ByVal objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri = Nothing,
                            Optional ByVal isSementieri As Boolean = False,
                            Optional ByVal id_Specie As Integer = 0,
                            Optional ByVal id_SottoSpecie As Integer = 0,
                            Optional ByVal id_Gruppo As Integer = 0,
                            Optional ByVal id_Genotipo As Integer = 0,
                            Optional ByVal isMappaturaLibera As Boolean = False,
                            Optional ByVal codiceSportelloInt As Integer = 0
                            ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.SpecieVegetali_R.SpecieVegetali_GestioneFiltroUtente_Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   Veg_Cod = 0           =>  tutti i record
        '====================================================================================


        Dim objSQL As New System.Text.StringBuilder
        Dim MessaggioErrore As String = ""
        Dim DT As DataTable
        Dim Messaggio As String
        Dim stbQuery As New System.Text.StringBuilder

        Dim Flag_EseguiQueryConGruCod As Boolean = False
        Dim Flag_EseguiQueryTutteSpecie As Boolean = False
        Dim Flag_EseguiQueryUnionAll As Boolean = False

        Dim i As Integer
        Dim Vet_GruCod() As Integer

        'Genero la query SQL
        Dim dbServer = Nothing
        If Not IsNothing(objParametri_Server) Then
            dbServer = objParametri_Server.StringaConnessione.Split(";")(2).Split("=")(1)
            'Dim dbutenti = objParametri_Utenti.StringaConnessione.Split(";")(2).Split("=")(1)
        End If



        If Gru_Cod <> 0 Then

            '--------------------------------------------------------------
            '---------------- GRUPPO VEGETALE VALORIZZATO -----------------
            '                   CASO: impianto - campo
            '--------------------------------------------------------------

            'eseguo la query con IF ELSE
            Flag_EseguiQueryConGruCod = True

        Else

            '--------------------------------------------------------------
            '-------------- GRUPPO VEGETALE NON VALORIZZATO ---------------
            '               CASO: semilavorato - semina - filtrino
            '--------------------------------------------------------------

            'caso nessuno: voglio vedere tutte le specie dell'archivio

            'else: 
            '       un solo gruppo selezionato -> faccio la query con il gru_cod valorizzato
            '       più di un gruppo selezionato (ma non tutti, quindi due): faccio la query prr ogni gru_cod e unifico il risultato


            Dim DT_GruCod As DataTable
            Dim objUtentiImpo As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_FiltroMono_R

            DT_GruCod = objUtentiImpo.Leggi(enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_GRUPPI_VEGETALI,
                                            0, AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                            "", "", objParametri_Utenti)

            If Not IsNothing(DT_GruCod) AndAlso Not (isSementieri AndAlso isMappaturaLibera) Then

                Select Case DT_GruCod.Rows.Count

                    Case 0
                        'non ho specificato nessun gruppo vegetale, quindi faccio la query normale
                        If isSementieri And Not codiceSportelloInt = 0 Then 
                            Dim sportello_R As New Sportello_R
                            Dim DT_GruCod_Sportello = sportello_R.Leggi_Gru_Cod(codiceSportelloInt, objParametri_server)
                            If DT_GruCod_Sportello.Rows.Count = 1 Then 
                                Gru_Cod = CInt(DT_GruCod_Sportello.Rows(0).Item("Gru_Cod"))
                                Flag_EseguiQueryConGruCod = True
                            Else
                                Flag_EseguiQueryTutteSpecie = True
                            End If
                        Else 
                            Flag_EseguiQueryTutteSpecie = True
                        End If

                    Case 1
                        'ho specificato un gruppo, faccio la query IF ELSE
                        Gru_Cod = CInt(DT_GruCod.Rows(0).Item("ID_0"))
                        Flag_EseguiQueryConGruCod = True

                    Case Else

                        'Esempio: nel filtro ho selezionato erbacee con 4 specie vegetali
                        '                   e ho selezionato orticole senza specificare le specie (quindi le voglio tutte)

                        'visto che il gru_cod non è passato, per evitare che nel menù vengano caricate solo le specie delle erbacee,
                        'faccio la query per ogni gru_cod e unifico il risultato

                        Dim Num_Gruppi As Integer = DT_GruCod.Rows.Count

                        ReDim Vet_GruCod(DT_GruCod.Rows.Count - 1)

                        For i = 0 To DT_GruCod.Rows.Count - 1

                            'mi salvo i gru_cod selezionati
                            Vet_GruCod(i) = DT_GruCod.Rows(i).Item("Id_0")

                        Next

                        Flag_EseguiQueryUnionAll = True

                End Select

            Else
                Flag_EseguiQueryTutteSpecie = True
            End If

        End If


        If Flag_EseguiQueryConGruCod = True Then

            'se l'utente ha filtrato le specie vegetali, ovvero nella tabella Utenti_Impostazioni_FiltroMono 
            'ci sono dei record per l'impostazione COD_FILTRO_SPECIE_VEGETALI per quel gru_cod
            stbQuery.AppendLine(" IF (  ")
            stbQuery.AppendLine(" SELECT COUNT(*)  ")
            stbQuery.AppendLine(" FROM          SpecieVegetali ")
            stbQuery.AppendLine(" INNER JOIN    Utenti_Impostazioni_FiltroMono ")
            stbQuery.AppendLine("               ON SpecieVegetali.veg_cod = Utenti_Impostazioni_FiltroMono.ID_0 ")
            stbQuery.AppendLine(" WHERE         Utenti_Impostazioni_FiltroMono.Piva_SuperUser = '" + Agro_SQL_SaveText(objParametri_Utenti.PivaSuperUser) + "'  ")
            stbQuery.AppendLine(" AND           Utenti_Impostazioni_FiltroMono.UserName = '" + Agro_SQL_SaveText(objParametri_Utenti.UtenteUsername) + "'  ")
            stbQuery.AppendLine($" AND           Utenti_Impostazioni_FiltroMono.Impostazione_Cod = {CInt(enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_SPECIE_VEGETALI)}")
            stbQuery.AppendLine(" AND           SpecieVegetali.Gru_Cod = " & Agro_SQL_SaveNum(Gru_Cod))
            stbQuery.AppendLine("   ) > 0 ")

            stbQuery.AppendLine("  ")
            stbQuery.AppendLine("       SELECT  * ")
            stbQuery.AppendLine("       FROM    SpecieVegetali ")
            stbQuery.AppendLine("               INNER JOIN Utenti_Impostazioni_FiltroMono ")
            stbQuery.AppendLine("               ON SpecieVegetali.veg_cod = Utenti_Impostazioni_FiltroMono.ID_0 ")

            '------- Salvatore Zammataro 11-05-2023, aggiunte per corretta lettura in caso Sementieri --------
            If (isSementieri) Then
                ComposeFilterForSementieri(stbQuery, id_Specie, dbServer)
            End If
            '-------------------------------------------------------------------------------------------------

            stbQuery.AppendLine("       WHERE   SpecieVegetali.Validita_Inizio <=" & Agro_SQL_SaveDate(objParametri_Utenti.FinestraTemporaleFine))
            stbQuery.AppendLine("       AND     SpecieVegetali.Validita_Fine >=" & Agro_SQL_SaveDate(objParametri_Utenti.FinestraTemporaleInizio))
            'le specie vegetali che il Gias non deve caricare hanno gru_cod = -1
            stbQuery.AppendLine("       AND     SpecieVegetali.Gru_Cod <> -1 ")
            stbQuery.AppendLine("       AND     Utenti_Impostazioni_FiltroMono.Piva_SuperUser = '" + Agro_SQL_SaveText(objParametri_Utenti.PivaSuperUser) + "'  ")
            stbQuery.AppendLine("       AND     Utenti_Impostazioni_FiltroMono.UserName = '" + Agro_SQL_SaveText(objParametri_Utenti.UtenteUsername) + "'  ")
            stbQuery.AppendLine($"       AND     Utenti_Impostazioni_FiltroMono.Impostazione_Cod = {CInt(enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_SPECIE_VEGETALI)}")
            'If Gru_Cod <> 0 Then
            stbQuery.AppendLine("   AND     SpecieVegetali.Gru_Cod = " & Agro_SQL_SaveNum(Gru_Cod))
            'End If
            If Veg_Cod <> 0 Then
                stbQuery.AppendLine("   AND     SpecieVegetali.Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod))
            End If
            If LetteraIniziale <> "" Then
                stbQuery.AppendLine("   AND     SpecieVegetali.Veg_Des LIKE '" & Agro_SQL_SaveText(LetteraIniziale) & "%' ")
            End If
            If StringaCerca <> "" Then
                stbQuery.AppendLine("   AND     SpecieVegetali.Veg_Des LIKE '%" & Agro_SQL_SaveText(StringaCerca) & "%' ")
            End If

            'If xFiltroAggiuntivo <> "" Then
            '    stbQuery.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            'End If
            If xFiltroAggiuntivo <> "" Then
                If Left(LTrim(xFiltroAggiuntivo), 3).ToUpper = "AND" Then
                    stbQuery.AppendLine(Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri_Utenti))
                Else
                    stbQuery.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri_Utenti))
                End If
            End If
            If xOrderBy <> "" Then
                stbQuery.AppendLine(Agro_SQL_Save_xOrderBy(xOrderBy, objParametri_Utenti))
            Else
                stbQuery.AppendLine(" ORDER BY SpecieVegetali.Veg_Des ")
            End If

            stbQuery.AppendLine("ELSE ")

            stbQuery.AppendLine("       SELECT  * ")
            stbQuery.AppendLine("       FROM    SpecieVegetali ")

            '------- Salvatore Zammataro 11-05-2023, aggiunte per corretta lettura in caso Sementieri --------
            If (isSementieri) Then
                ComposeFilterForSementieri(stbQuery, id_Specie, dbServer)
            End If
            '-------------------------------------------------------------------------------------------------

            stbQuery.AppendLine("       WHERE   Validita_Inizio <=" & Agro_SQL_SaveDate(objParametri_Utenti.FinestraTemporaleFine))
            stbQuery.AppendLine("       AND     Validita_Fine >=" & Agro_SQL_SaveDate(objParametri_Utenti.FinestraTemporaleInizio))
            'le specie vegetali che il Gias non deve caricare hanno gru_cod = -1
            stbQuery.AppendLine("       AND     Gru_Cod <> -1 ")
            'If Gru_Cod <> 0 Then
            stbQuery.AppendLine("   AND     Gru_Cod = " & Agro_SQL_SaveNum(Gru_Cod))
            'End If
            If Veg_Cod <> 0 Then
                stbQuery.AppendLine("   AND     Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod))
            End If
            If LetteraIniziale <> "" Then
                stbQuery.AppendLine("   AND     Veg_Des LIKE '" & Agro_SQL_SaveText(LetteraIniziale) & "%' ")
            End If
            If StringaCerca <> "" Then
                stbQuery.AppendLine("   AND     Veg_Des LIKE '%" & Agro_SQL_SaveText(StringaCerca) & "%' ")
            End If
            'If xFiltroAggiuntivo <> "" Then
            '    stbQuery.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            'End If
            If xFiltroAggiuntivo <> "" Then
                If Left(LTrim(xFiltroAggiuntivo), 3).ToUpper = "AND" Then
                    stbQuery.AppendLine(Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri_Utenti))
                Else
                    stbQuery.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri_Utenti))
                End If
            End If
            If xOrderBy <> "" Then
                stbQuery.AppendLine(Agro_SQL_Save_xOrderBy(xOrderBy, objParametri_Utenti))
            Else
                stbQuery.AppendLine(" ORDER BY SpecieVegetali.Veg_Des ")
            End If

        End If

        '####################################

        If Flag_EseguiQueryTutteSpecie = True Then

            'nessun filtro, faccio vedere tutte le specie

            stbQuery.AppendLine("       SELECT  * ")
            stbQuery.AppendLine("       FROM    SpecieVegetali ")

            stbQuery.AppendLine("       WHERE   Validita_Inizio <=" & Agro_SQL_SaveDate(objParametri_Utenti.FinestraTemporaleFine))
            stbQuery.AppendLine("       AND     Validita_Fine >=" & Agro_SQL_SaveDate(objParametri_Utenti.FinestraTemporaleInizio))
            'le specie vegetali che il Gias non deve caricare hanno gru_cod = -1
            stbQuery.AppendLine("       AND     Gru_Cod <> -1 ")
            If Gru_Cod <> 0 Then
                stbQuery.AppendLine("   AND     Gru_Cod = " & Agro_SQL_SaveNum(Gru_Cod))
            End If
            If Veg_Cod <> 0 Then
                stbQuery.AppendLine("   AND     Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod))
            End If
            If LetteraIniziale <> "" Then
                stbQuery.AppendLine("   AND Veg_Des LIKE '" & Agro_SQL_SaveText(LetteraIniziale) & "%' ")
            End If
            If StringaCerca <> "" Then
                stbQuery.AppendLine("   AND     Veg_Des LIKE '%" & Agro_SQL_SaveText(StringaCerca) & "%' ")
            End If
            'If xFiltroAggiuntivo <> "" Then
            '    stbQuery.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            'End If
            If xFiltroAggiuntivo <> "" Then
                If Left(LTrim(xFiltroAggiuntivo), 3).ToUpper = "AND" Then
                    stbQuery.AppendLine(Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri_Utenti))
                Else
                    stbQuery.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri_Utenti))
                End If
            End If
            If xOrderBy <> "" Then
                stbQuery.AppendLine(Agro_SQL_Save_xOrderBy(xOrderBy, objParametri_Utenti))
            Else
                stbQuery.AppendLine(" ORDER BY SpecieVegetali.Veg_Des ")
            End If


        End If


        '####################################

        If Flag_EseguiQueryConGruCod = True Or Flag_EseguiQueryTutteSpecie = True Then

            'eseguo la query per uno dei due casi

            'Recupero il datatable
            Try

                '--------------------------------------------------------------------------
                DT = EseguiQuery_Lettura(objParametri_Utenti, stbQuery.ToString, NomeRoutine)
                '--------------------------------------------------------------------------

            Catch ex As Exception
                MessaggioErrore = ex.Message
                Scrivi_LOG(objParametri_Utenti, NomeRoutine, MessaggioErrore)
                DT = Nothing
                Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
            End Try

            Return DT

        End If


        '####################################

        If Flag_EseguiQueryUnionAll = True Then

            'caso di più gruppi selezionati

            Dim DT_Clonato As DataTable
            Dim j As Integer

            For i = 0 To Vet_GruCod.Length - 1

                'faccio la query per ogni gru_cod
                Gru_Cod = Vet_GruCod(i)

                'If i <> 0 Then
                '    stbQuery.AppendLine(" UNION ALL  ")
                'End If

                stbQuery.Length = 0

                stbQuery.AppendLine("IF (")
                stbQuery.AppendLine("    SELECT COUNT(*)")
                stbQuery.AppendLine("    FROM SpecieVegetali")
                stbQuery.AppendLine("    INNER JOIN Utenti_Impostazioni_FiltroMono ON SpecieVegetali.veg_cod = Utenti_Impostazioni_FiltroMono.ID_0")
                stbQuery.AppendLine("    WHERE Utenti_Impostazioni_FiltroMono.Piva_SuperUser = '" + Agro_SQL_SaveText(objParametri_Utenti.PivaSuperUser) + "'")
                stbQuery.AppendLine("        AND Utenti_Impostazioni_FiltroMono.UserName = '" + Agro_SQL_SaveText(objParametri_Utenti.UtenteUsername) + "'")
                stbQuery.AppendLine("        AND Utenti_Impostazioni_FiltroMono.Impostazione_Cod = " + Agro_SQL_SaveNum(enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_SPECIE_VEGETALI))
                stbQuery.AppendLine("        AND SpecieVegetali.Gru_Cod = " & Agro_SQL_SaveNum(Gru_Cod))
                stbQuery.AppendLine(") > 0")

                stbQuery.AppendLine("")
                stbQuery.AppendLine("    SELECT *")
                stbQuery.AppendLine("    FROM SpecieVegetali")
                stbQuery.AppendLine("    INNER JOIN Utenti_Impostazioni_FiltroMono ON SpecieVegetali.veg_cod = Utenti_Impostazioni_FiltroMono.ID_0")
                stbQuery.AppendLine("    WHERE SpecieVegetali.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri_Utenti.FinestraTemporaleFine))
                stbQuery.AppendLine("        AND SpecieVegetali.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri_Utenti.FinestraTemporaleInizio))
                'le specie vegetali che il Gias non deve caricare hanno gru_cod = -1
                stbQuery.AppendLine("        AND SpecieVegetali.Gru_Cod <> -1")
                stbQuery.AppendLine("        AND Utenti_Impostazioni_FiltroMono.Piva_SuperUser = '" + Agro_SQL_SaveText(objParametri_Utenti.PivaSuperUser) + "'")
                stbQuery.AppendLine("        AND Utenti_Impostazioni_FiltroMono.UserName = '" + Agro_SQL_SaveText(objParametri_Utenti.UtenteUsername) + "'")
                stbQuery.AppendLine("        AND Utenti_Impostazioni_FiltroMono.Impostazione_Cod = " & CInt(enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_SPECIE_VEGETALI))
                stbQuery.AppendLine("        AND SpecieVegetali.Gru_Cod = " & Agro_SQL_SaveNum(Gru_Cod))

                If Veg_Cod <> 0 Then
                    stbQuery.AppendLine("        AND SpecieVegetali.Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod))
                End If

                If LetteraIniziale <> "" Then
                    stbQuery.AppendLine("        AND SpecieVegetali.Veg_Des LIKE '" & Agro_SQL_SaveText(LetteraIniziale) & "%'")
                End If

                If StringaCerca <> "" Then
                    stbQuery.AppendLine("        AND SpecieVegetali.Veg_Des LIKE '%" & Agro_SQL_SaveText(StringaCerca) & "%'")
                End If

                If xFiltroAggiuntivo <> "" Then
                    If Left(LTrim(xFiltroAggiuntivo), 3).ToUpper = "AND" Then
                        stbQuery.AppendLine(Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri_Utenti))
                    Else
                        stbQuery.AppendLine("        AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri_Utenti))
                    End If
                End If

                If xOrderBy <> "" Then
                    stbQuery.AppendLine(Agro_SQL_Save_xOrderBy(xOrderBy, objParametri_Utenti))
                Else
                    stbQuery.AppendLine("    ORDER BY SpecieVegetali.Veg_Des")
                End If

                stbQuery.AppendLine("ELSE")

                stbQuery.AppendLine("    SELECT *")
                stbQuery.AppendLine("    FROM SpecieVegetali")
                stbQuery.AppendLine("    WHERE Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri_Utenti.FinestraTemporaleFine))
                stbQuery.AppendLine("        AND Validita_Fine >= " & Agro_SQL_SaveDate(objParametri_Utenti.FinestraTemporaleInizio))
                'le specie vegetali che il Gias non deve caricare hanno gru_cod = -1
                stbQuery.AppendLine("        AND Gru_Cod <> -1")
                'If Gru_Cod <> 0 Then
                stbQuery.AppendLine("        AND Gru_Cod = " & Agro_SQL_SaveNum(Gru_Cod))
                'End If
                If Veg_Cod <> 0 Then
                    stbQuery.AppendLine("        AND Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod))
                End If
                If LetteraIniziale <> "" Then
                    stbQuery.AppendLine("        AND Veg_Des LIKE '" & Agro_SQL_SaveText(LetteraIniziale) & "%'")
                End If
                If StringaCerca <> "" Then
                    stbQuery.AppendLine("        AND Veg_Des LIKE '%" & Agro_SQL_SaveText(StringaCerca) & "%'")
                End If
                'If xFiltroAggiuntivo <> "" Then
                '    stbQuery.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                'End If
                If xFiltroAggiuntivo <> "" Then
                    If Left(LTrim(xFiltroAggiuntivo), 3).ToUpper = "AND" Then
                        stbQuery.AppendLine(Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri_Utenti))
                    Else
                        stbQuery.AppendLine("        AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri_Utenti))
                    End If
                End If
                If xOrderBy <> "" Then
                    stbQuery.AppendLine(Agro_SQL_Save_xOrderBy(xOrderBy, objParametri_Utenti))
                Else
                    stbQuery.AppendLine("    ORDER BY Veg_Des")
                End If

                Try
                    '--------------------------------------------------------------------------
                    DT = EseguiQuery_Lettura(objParametri_Utenti, stbQuery.ToString, NomeRoutine)
                    '--------------------------------------------------------------------------
                Catch ex As Exception
                    MessaggioErrore = ex.Message
                    Scrivi_LOG(objParametri_Utenti, NomeRoutine, MessaggioErrore)
                    DT = Nothing
                    Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
                End Try



                If i = 0 Then
                    'il primo giro clono la struttura del datatable
                    DT_Clonato = DT.Clone
                End If

                For j = 0 To DT.Rows.Count - 1
                    'importo ogni riga nel nuovo datatable
                    DT_Clonato.ImportRow(DT.Rows.Item(j))

                Next

            Next 'per i gru_cod

            Return DT_Clonato


        End If 'union all


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
    Public Function Numero_Totale_Specie(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Integer

        Dim Num As Integer = 0
        Dim Dt As DataTable

        Dt = Leggi(0, 0, "", "", AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri)

        If Not Dt Is Nothing AndAlso Dt.Rows.Count > 0 Then
            Num = Dt.Rows.Count
        End If

        Return Num

    End Function


End Class


'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§

Public Class SpecieVegetali_W
    Inherits AgronicaCoreDataProvider.DataProvider


    'Versione che utilizza AgronicaCoreParametri
    '##############################################################################################
    Public Function Scrivi( _
                            ByVal Veg_Cod As Integer, _
                            ByVal Veg_Cod_AUX As Integer, _
                            ByVal Veg_Des As String, _
                            ByVal Veg_Des_Lat As String, _
                            ByVal Grsp_Cod As Integer, _
                            ByVal Gru_Cod As Integer, _
                            ByVal Bayer_SpecieVegetale As String, _
                            ByVal DATA_AGG As Date, _
                            ByVal Data_Creazione As Date, _
                            ByVal Data_Modifica As Date, _
                            ByVal Username_Creazione As String, _
                            ByVal Username_Modifica As String, _
                            ByVal Validita_Inizio As Date, _
                            ByVal Validita_Fine As Date, _
                            ByVal DataLock As Integer, _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.SpecieVegetali_W.Scrivi()"
        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '--------------------------------------------------------------------------
            'Verifica informazioni
            '
            If IsNothing(Veg_Cod) Then
                Throw New Exception("Parametro non corretto nella query (Veg_Cod obbligatorio)")
            End If
            '--------------------------------------------------------------------------


            '//////////////////////////////////////////////////////////////////////
            '//////////////////////////////////////////////////////////////////////
            StrSQL.Length = 0

            StrSQL.Append(" INSERT INTO [SpecieVegetali] ")

            StrSQL.Append("            ([Veg_Cod] ")
            StrSQL.Append("            ,[Veg_Cod_AUX] ")
            StrSQL.Append("            ,[Veg_Des] ")
            StrSQL.Append("            ,[Veg_Des_Lat] ")
            StrSQL.Append("            ,[Grsp_Cod] ")
            StrSQL.Append("            ,[Gru_Cod] ")
            StrSQL.Append("            ,[Bayer_SpecieVegetale] ")
            StrSQL.Append("            ,[DATA_AGG] ")

            StrSQL.Append("            ,[inviato] ")
            StrSQL.Append("            ,[datainvio] ")
            StrSQL.Append("            ,[Data_Creazione] ")
            StrSQL.Append("            ,[Data_Modifica] ")
            StrSQL.Append("            ,[Username_Creazione] ")
            StrSQL.Append("            ,[Username_Modifica] ")
            StrSQL.Append("            ,[Validita_Inizio] ")
            StrSQL.Append("            ,[Validita_Fine] ")
            StrSQL.Append("            ,[DataLock]) ")

            StrSQL.Append("                 VALUES ")

            StrSQL.Append("            ( ")
            StrSQL.Append("              " & Agro_SQL_SaveNum(Veg_Cod) & " ")
            StrSQL.Append("            , " & Agro_SQL_SaveNum(Veg_Cod_AUX) & " ")
            StrSQL.Append("            ,'" & Agro_SQL_SaveText(Veg_Des) & "' ")
            StrSQL.Append("            ,'" & Agro_SQL_SaveText(Veg_Des_Lat) & "' ")
            StrSQL.Append("            , " & Agro_SQL_SaveNum(Grsp_Cod) & " ")
            StrSQL.Append("            , " & Agro_SQL_SaveNum(Gru_Cod) & " ")
            StrSQL.Append("            ,'" & Agro_SQL_SaveText(Bayer_SpecieVegetale) & "' ")
            StrSQL.Append("            , " & Agro_SQL_SaveDate(DATA_AGG) & " ")

            StrSQL.Append("            ,0 ")
            StrSQL.Append("            ,NULL ")
            StrSQL.Append("            , " & Agro_SQL_SaveDate(Data_Creazione) & " ")
            StrSQL.Append("            , " & Agro_SQL_SaveDate(Data_Modifica) & " ")
            StrSQL.Append("            ,'" & Agro_SQL_SaveText(Username_Creazione) & "' ")
            StrSQL.Append("            ,'" & Agro_SQL_SaveText(Username_Modifica) & "' ")
            StrSQL.Append("            , " & Agro_SQL_SaveDate(Validita_Inizio) & " ")
            StrSQL.Append("            , " & Agro_SQL_SaveDate(Validita_Fine) & " ")
            StrSQL.Append("            , " & Agro_SQL_SaveNum(DataLock) & " ")
            StrSQL.Append("            ) ")
            '//////////////////////////////////////////////////////////////////////
            '//////////////////////////////////////////////////////////////////////


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
    Public Function Modifica( _
                            ByVal Veg_Cod As Integer, _
                            ByVal Veg_Cod_AUX As Integer, _
                            ByVal Veg_Des As String, _
                            ByVal Veg_Des_Lat As String, _
                            ByVal Grsp_Cod As Integer, _
                            ByVal Gru_Cod As Integer, _
                            ByVal Bayer_SpecieVegetale As String, _
                            ByVal DATA_AGG As Date, _
                            ByVal Data_Creazione As Date, _
                            ByVal Data_Modifica As Date, _
                            ByVal Username_Creazione As String, _
                            ByVal Username_Modifica As String, _
                            ByVal Validita_Inizio As Date, _
                            ByVal Validita_Fine As Date, _
                            ByVal DataLock As Integer, _
                                ByVal xFiltroAggiuntivo As String, _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.SpecieVegetali_W.Modifica()"
        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False


        Try
            '--------------------------------------------------------------------------
            'Verifica informazioni 
            '
            If IsNothing(Veg_Cod) Then
                Throw New Exception("Parametro non corretto nella query (Veg_Cod obbligatorio)")
            End If
            '--------------------------------------------------------------------------


            '//////////////////////////////////////////////////////////////////////
            '//////////////////////////////////////////////////////////////////////
            StrSQL.Length = 0

            StrSQL.Append(" UPDATE SpecieVegetali ")

            StrSQL.Append(" SET   [Veg_Cod_AUX] = " & Agro_SQL_SaveNum(Veg_Cod_AUX) & " ")
            StrSQL.Append("      ,[Veg_Des] = '" & Agro_SQL_SaveText(Veg_Des) & "' ")
            StrSQL.Append("      ,[Veg_Des_Lat] = '" & Agro_SQL_SaveText(Veg_Des_Lat) & "' ")
            StrSQL.Append("      ,[Grsp_Cod] = " & Agro_SQL_SaveNum(Grsp_Cod) & " ")
            StrSQL.Append("      ,[Gru_Cod] = " & Agro_SQL_SaveNum(Gru_Cod) & " ")
            StrSQL.Append("      ,[Bayer_SpecieVegetale] = '" & Agro_SQL_SaveText(Bayer_SpecieVegetale) & "' ")
            StrSQL.Append("      ,[DATA_AGG] = " & Agro_SQL_SaveDate(DATA_AGG) & " ")

            StrSQL.Append("      ,[inviato] = 0 ")
            StrSQL.Append("      ,[datainvio] = NULL ")
            StrSQL.Append("      ,[Data_Creazione] = " & Agro_SQL_SaveDate(Data_Creazione) & " ")
            StrSQL.Append("      ,[Data_Modifica] = " & Agro_SQL_SaveDate(Data_Modifica) & " ")
            StrSQL.Append("      ,[Username_Creazione] = '" & Agro_SQL_SaveText(Username_Creazione) & "' ")
            StrSQL.Append("      ,[Username_Modifica] = '" & Agro_SQL_SaveText(Username_Modifica) & "' ")
            StrSQL.Append("      ,[Validita_Inizio] = " & Agro_SQL_SaveDate(Validita_Inizio) & " ")
            StrSQL.Append("      ,[Validita_Fine] = " & Agro_SQL_SaveDate(Validita_Fine) & " ")
            StrSQL.Append("      ,[DataLock] = " & Agro_SQL_SaveNum(DataLock) & " ")

            StrSQL.Append(" WHERE Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod) & " ")

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



    'Versione che utilizza AgronicaCoreParametri
    '##############################################################################################
    Public Function Cancella( _
                            ByVal Veg_Cod As Integer, _
                                ByVal xFiltroAggiuntivo As String, _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.SpecieVegetali_W.Cancella()"
        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '--------------------------------------------------------------------------
            'Verifica informazioni 
            '
            If IsNothing(Veg_Cod) Then
                Throw New Exception("Parametro non corretto nella query (Veg_Cod obbligatorio)")
            End If
            '--------------------------------------------------------------------------


            '//////////////////////////////////////////////////////////////////////
            '//////////////////////////////////////////////////////////////////////
            If objParametri.FlagCancellazioneLogica = AgronicaCoreDataProvider.AgronicaCoreParametri.enumCancellazioneLogica.CancellazioneLogica Then

                StrSQL.Length = 0
                StrSQL.Append(" UPDATE Agenda ")
                StrSQL.Append(" SET ")
                StrSQL.Append("       Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("      ,[Data_Modifica] = " & Agro_SQL_SaveDate(Date.Now) & " ")
                StrSQL.Append("      ,Inviato = -1 ")
                StrSQL.Append(" WHERE  Inviato >= 0 ")
            Else
                StrSQL.Length = 0
                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM Agenda ")
                StrSQL.Append(" WHERE  1=1 ")
            End If
            '----------------------------------------------------------------------
            StrSQL.Append(" AND Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod) & " ")




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


'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################











