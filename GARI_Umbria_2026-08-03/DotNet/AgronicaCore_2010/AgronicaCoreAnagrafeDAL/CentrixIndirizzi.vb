Imports System.Data.Common
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.DataProviderExtensions



'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§

Public Class CentrixIndirizzi_Read
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function Leggi( _
                        ByVal PIVA As String, _
                        ByVal Sa_Cod As Int32, _
                        ByVal cod_indirizzo As Int32, _
                        ByVal Tipo_Indirizzo As Int32, _
                            ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile, _
                            ByVal xFiltroAggiuntivo As String, _
                            ByVal xOrderBy As String, _
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                            ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.CentrixIndirizzi_Read.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""                   =>  si leggono tutte le imprese
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT PIVA, sa_cod, cod_indirizzo, Tipo_Indirizzo ")
                    StrSQL.Append(" FROM  CentrixIndirizzi ")
                    StrSQL.Append(" WHERE CentrixIndirizzi.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND   CentrixIndirizzi.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    If PIVA <> "" Then
                        StrSQL.Append(" AND   CentrixIndirizzi.Piva = '" & Agro_SQL_SaveText(PIVA) & "'")
                    End If

                    If Sa_Cod <> 0 Then
                        StrSQL.Append(" AND CentrixIndirizzi.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
                    End If

                    If cod_indirizzo <> 0 Then
                        StrSQL.Append(" AND CentrixIndirizzi.Cod_Indirizzo = " & Agro_SQL_SaveNum(cod_indirizzo) & " ")
                    End If

                    If Tipo_Indirizzo <> 0 Then
                        StrSQL.Append(" AND CentrixIndirizzi.Tipo_Indirizzo = " & Agro_SQL_SaveNum(Tipo_Indirizzo) & " ")
                    End If

                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   CentrixIndirizzi.Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   CentrixIndirizzi.Inviato =-1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    End If


                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta

                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT PIVA, sa_cod, cod_indirizzo, Tipo_Indirizzo ")
                    StrSQL.Append(", CentrixIndirizzi.validita_inizio as CentrixIndirizzi_validita_inizio ")
                    StrSQL.Append(", CentrixIndirizzi.validita_fine as CentrixIndirizzi_validita_fine ")
                    StrSQL.Append(", CentrixIndirizzi.data_creazione as CentrixIndirizzi_data_creazione ")
                    StrSQL.Append(", CentrixIndirizzi.data_modifica as CentrixIndirizzi_data_modifica ")
                    StrSQL.Append(", CentrixIndirizzi.username_Creazione as CentrixIndirizzi_username_creazione ")
                    StrSQL.Append(", CentrixIndirizzi.username_Modifica as CentrixIndirizzi_username_modifica ")

                    StrSQL.Append(" FROM  CentrixIndirizzi ")
                    StrSQL.Append(" WHERE CentrixIndirizzi.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND   CentrixIndirizzi.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    If PIVA <> "" Then
                        StrSQL.Append(" AND   CentrixIndirizzi.Piva = '" & Agro_SQL_SaveText(PIVA) & "'")
                    End If

                    If Sa_Cod <> 0 Then
                        StrSQL.Append(" AND CentrixIndirizzi.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
                    End If

                    If cod_indirizzo <> 0 Then
                        StrSQL.Append(" AND CentrixIndirizzi.Cod_Indirizzo = " & Agro_SQL_SaveNum(cod_indirizzo) & " ")
                    End If

                    If Tipo_Indirizzo <> 0 Then
                        StrSQL.Append(" AND CentrixIndirizzi.Tipo_Indirizzo = " & Agro_SQL_SaveNum(Tipo_Indirizzo) & " ")
                    End If

                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   CentrixIndirizzi.Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   CentrixIndirizzi.Inviato =-1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    End If

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni


                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT CentrixIndirizzi.*, sa_nome  ")

                    StrSQL.Append(" FROM  CentrixIndirizzi ")
                    StrSQL.Append(" INNER JOIN Centri_Aziendali ON CentrixIndirizzi.Piva = Centri_Aziendali.Piva AND CentrixIndirizzi.sa_cod = Centri_Aziendali.sa_cod ")

                    StrSQL.Append(" WHERE CentrixIndirizzi.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND   CentrixIndirizzi.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    If PIVA <> "" Then
                        StrSQL.Append(" AND   CentrixIndirizzi.Piva = '" & Agro_SQL_SaveText(PIVA) & "'")
                    End If

                    If Sa_Cod <> 0 Then
                        StrSQL.Append(" AND CentrixIndirizzi.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
                    End If

                    If cod_indirizzo <> 0 Then
                        StrSQL.Append(" AND CentrixIndirizzi.Cod_Indirizzo = " & Agro_SQL_SaveNum(cod_indirizzo) & " ")
                    End If

                    If Tipo_Indirizzo <> 0 Then
                        StrSQL.Append(" AND CentrixIndirizzi.Tipo_Indirizzo = " & Agro_SQL_SaveNum(Tipo_Indirizzo) & " ")
                    End If

                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   CentrixIndirizzi.Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   CentrixIndirizzi.Inviato =-1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    End If



                Case enumSelezioneVariabile.Selezione_JoinCompleta

                    StrSQL.Length = 0
                    'StrSQL.AppendLine(" SELECT CentrixIndirizzi.*, Indirizzi.*, ISNULL(ISTAT.LOCALITA, '') AS com_des, ISNULL(ISTAT.COMUNI_PROV, '') AS pro_cod, ISNULL(Lista_Province.PROVINCIA, '') AS pro_des  ")
                    StrSQL.AppendLine(" SELECT CentrixIndirizzi.*, rag_soc, sa_nome,  ")
                    StrSQL.AppendLine(" Indirizzi.cod_indirizzo, Indirizzi.ind_des, Indirizzi.frz_des, Indirizzi.cap, Indirizzi.stato, Indirizzi.note, Indirizzi.pro_cod_istat, Indirizzi.com_cod_istat, ")
                    StrSQL.AppendLine(" ISNULL(ISTAT.LOCALITA, '') AS com_des, ISNULL(ISTAT.COMUNI_PROV, '') AS pro_cod, ISNULL(Lista_Province.PROVINCIA, '') AS pro_des, ISNULL(Lista_Province.REG, '') AS REG , Centri_Aziendali.lat, Centri_Aziendali.long, ")
                    strSql.AppendLine(" Indirizzi.Validazione As Indirizzi_Validazione, Indirizzi.Data_Validazione as Indirizzi_Data_Validazione, Indirizzi.UserName_Validazione as Indirizzi_UserName_Validazione, ")
                    strSql.AppendLine(" Indirizzi.Codice_Lingua, Indirizzi.Codice_Alternativo ")
                    
                    StrSQL.AppendLine(" FROM  CentrixIndirizzi ")
                    StrSQL.AppendLine(" INNER JOIN Indirizzi ON CentrixIndirizzi.Cod_Indirizzo = Indirizzi.Cod_Indirizzo  ")
                    StrSQL.AppendLine(" INNER JOIN Imprese ON CentrixIndirizzi.Piva = Imprese.Piva ")
                    StrSQL.AppendLine(" INNER JOIN Centri_Aziendali ON CentrixIndirizzi.Piva = Centri_Aziendali.Piva AND CentrixIndirizzi.sa_cod = Centri_Aziendali.sa_cod ")

                    StrSQL.AppendLine(" LEFT OUTER JOIN ISTAT ON Indirizzi.pro_cod_istat = ISTAT.PROV AND Indirizzi.com_cod_istat = ISTAT.COM ")
                    StrSQL.AppendLine(" LEFT OUTER JOIN  Lista_Province ON Indirizzi.pro_cod_istat = Lista_Province.PROV ")

                    StrSQL.AppendLine(" WHERE CentrixIndirizzi.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.AppendLine(" AND   CentrixIndirizzi.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.AppendLine(" AND   Indirizzi.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.AppendLine(" AND   Indirizzi.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    If PIVA <> "" Then
                        StrSQL.AppendLine(" AND   CentrixIndirizzi.Piva = '" & Agro_SQL_SaveText(PIVA) & "'")
                    End If


                    If Sa_Cod <> 0 Then
                        StrSQL.AppendLine(" AND CentrixIndirizzi.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
                    End If

                    If cod_indirizzo <> 0 Then
                        StrSQL.AppendLine(" AND CentrixIndirizzi.Cod_Indirizzo = " & Agro_SQL_SaveNum(cod_indirizzo) & " ")
                    End If

                    If Tipo_Indirizzo <> 0 Then
                        StrSQL.AppendLine(" AND CentrixIndirizzi.Tipo_Indirizzo = " & Agro_SQL_SaveNum(Tipo_Indirizzo) & " ")
                    End If


                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.AppendLine(" AND   CentrixIndirizzi.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.AppendLine(" AND   CentrixIndirizzi.Inviato =-1 ")
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
    Public Function Leggi_SedeLegale( ByVal PIVA As String, _
                                        ByVal Sa_Cod As Int32, _
                                        ByVal cod_indirizzo As Int32, _
                                        ByVal Tipo_Indirizzo As Int32, _
                                             ByVal xFiltroAggiuntivo As String, _
                                            ByVal xOrderBy As String, _
                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                            ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.CentrixIndirizzi_Read.Leggi_SedeLegale()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""                   =>  si leggono tutte le imprese
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT CentrixIndirizzi.Tipo_Indirizzo, rag_soc, sa_nome,  ")
            StrSQL.Append(" Indirizzi.cod_indirizzo, Indirizzi.ind_des, Indirizzi.frz_des, Indirizzi.cap, Indirizzi.stato, Indirizzi.note, Indirizzi.pro_cod_istat, Indirizzi.com_cod_istat, ")
            StrSQL.Append(" ISNULL(ISTAT.LOCALITA, '') AS com_des, ISNULL(ISTAT.COMUNI_PROV, '') AS pro_cod, ISNULL(Lista_Province.PROVINCIA, '') AS pro_des, Lista_Province.REG  ")

            StrSQL.Append(" FROM  CentrixIndirizzi ")
            StrSQL.Append(" INNER JOIN Indirizzi ON CentrixIndirizzi.Cod_Indirizzo = Indirizzi.Cod_Indirizzo  ")
            StrSQL.Append(" INNER JOIN Imprese ON CentrixIndirizzi.Piva = Imprese.Piva ")
            StrSQL.Append(" INNER JOIN Centri_Aziendali ON CentrixIndirizzi.Piva = Centri_Aziendali.Piva AND CentrixIndirizzi.sa_cod = Centri_Aziendali.sa_cod ")
            StrSQL.Append(" INNER JOIN Centri_Aziendali_Codici  ON Centri_Aziendali.PIVA = Centri_Aziendali_Codici.PIVA AND  Centri_Aziendali.sa_cod = Centri_Aziendali_Codici.sa_cod  " + vbCrLf)

            StrSQL.Append(" INNER JOIN ISTAT ON Indirizzi.pro_cod_istat = ISTAT.PROV AND Indirizzi.com_cod_istat = ISTAT.COM ")
            StrSQL.Append(" INNER JOIN  Lista_Province ON Indirizzi.pro_cod_istat = Lista_Province.PROV ")

            StrSQL.Append(" WHERE   Centri_Aziendali_Codici.id_cod = 101 " + vbCrLf)

            If PIVA <> "" Then
                StrSQL.Append(" AND   CentrixIndirizzi.Piva = '" & Agro_SQL_SaveText(PIVA) & "'")
            End If

            If Sa_Cod <> 0 Then
                StrSQL.Append(" AND CentrixIndirizzi.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            End If

            If cod_indirizzo <> 0 Then
                StrSQL.Append(" AND CentrixIndirizzi.Cod_Indirizzo = " & Agro_SQL_SaveNum(cod_indirizzo) & " ")
            End If

            If Tipo_Indirizzo <> 0 Then
                StrSQL.Append(" AND CentrixIndirizzi.Tipo_Indirizzo = " & Agro_SQL_SaveNum(Tipo_Indirizzo) & " ")
            End If


            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   CentrixIndirizzi.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   CentrixIndirizzi.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
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
    Public Function SaNome_from_CodIndirizzoCentro(ByVal Cod_Indirizzo As Integer, _
                                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                                    ) As String

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.CentriAziendaliRead.SaNome_from_CodIndirizzoCentro()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder


        Dim DT As DataTable
        DT = Leggi("", 0, Cod_Indirizzo, 0, _
                    AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni, _
                    "", _
                    "", _
                    objParametri)

        If Not IsNothing(DT) Then
            If DT.Rows.Count <> 0 Then
                Return DT.Rows(0).Item("Sa_Nome")
            Else
                Return ""
            End If
        Else
            Return ""
        End If


    End Function

    '################################################################################
    Public Function Indirizzo_SedeLegale(ByVal Piva As String, _
                                        ByRef Sa_nome As String, _
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                        ) As String

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.CentriAziendaliRead.Indirizzo_SedeLegale()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim Indirizzo As String

        Dim DT As DataTable
        DT = Leggi_SedeLegale(Piva, 0, 0, 1, _
                                "", _
                                "", _
                                objParametri)

        If Not IsNothing(DT) Then
            If DT.Rows.Count <> 0 Then
                Sa_nome = CStr(DT.Rows(0).Item("sa_nome"))
                Indirizzo = CStr(DT.Rows(0).Item("ind_des")) & " " & _
                              CStr(DT.Rows(0).Item("frz_des")) & " " & _
                              CStr(DT.Rows(0).Item("com_des")) & " " & _
                              CStr(DT.Rows(0).Item("pro_cod"))
            End If
        End If

        Return Indirizzo

    End Function



    '##############################################################################################
    Public Function Leggi_dato_centro( _
                        ByVal PIVA As String, _
                        ByVal Sa_Cod As Int32, _
                            ByVal xFiltroAggiuntivo As String, _
                            ByVal xOrderBy As String, _
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                            ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.CentrixIndirizzi_Read.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""                   =>  si leggono tutte le imprese
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT     CentrixIndirizzi.PIVA, CentrixIndirizzi.sa_cod, Indirizzi.* ")
            StrSQL.Append("FROM         Indirizzi INNER JOIN ")
            StrSQL.Append("           CentrixIndirizzi ON Indirizzi.cod_indirizzo = CentrixIndirizzi.cod_indirizzo ")

            StrSQL.Append(" WHERE CentrixIndirizzi.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.Append(" AND   CentrixIndirizzi.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            If PIVA <> "" Then
                StrSQL.Append(" AND   CentrixIndirizzi.Piva = '" & Agro_SQL_SaveText(PIVA) & "'")
            End If

            If Sa_Cod <> 0 Then
                StrSQL.Append(" AND CentrixIndirizzi.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            End If


            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   CentrixIndirizzi.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   CentrixIndirizzi.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
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
    Public Function Recupera_Indirizzo_ElementiGerarchia( _
                                ByVal ElementoGerarchia As enum_GerarchiaImpresa_Elementi, _
                                ByRef Provincia_Sigla As String, _
                                ByRef CodiceIstat_Provincia As String, _
                                ByRef CodiceIstat_Comune As String, _
                                ByRef Errore As String, _
                                ByVal Piva As String, _
                                ByVal Sa_Cod As Integer, _
                                ByVal Campo_Cod As Integer, _
                                ByVal Appezza As Integer, _
                                   ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                   ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.CentrixIndirizzi_Read.Recupera_Indirizzo_ElementiGerarchia()"
        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        '---------------

        Select Case ElementoGerarchia

            Case enum_GerarchiaImpresa_Elementi.Gerarchia_Centro

                'Creo gli oggetti COM+
                Dim objCOM As New AgronicaCoreAnagrafeDAL.CentrixIndirizzi_Read         'New Agro_Anagrafe_AD.CentrixIndirizzi_Read
                'objCOM = objServer.createobjxxx("Agro_Anagrafe_AD.CentrixIndirizzi_Read")

                'Leggo le informazioni sull'impresa selezionata			
                DT = objCOM.Leggi(CStr(Piva), _
                                  CInt(Sa_Cod), _
                                  0, _
                                  CInt(1), _
                                    AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta, _
                                    "", "", objParametri)

                'Se il recordset non e' nullo
                If DT.Rows.Count > 0 Then

                    Errore = ""
                    CodiceIstat_Provincia = DT.Rows(0).Item("pro_cod_istat")
                    CodiceIstat_Comune = DT.Rows(0).Item("com_cod_istat")
                    Provincia_Sigla = DT.Rows(0).Item("pro_cod")

                Else

                    Errore = "Elemento non trovato"
                    CodiceIstat_Provincia = ""
                    CodiceIstat_Comune = ""
                    Provincia_Sigla = ""
                End If

        End Select


    End Function


    '##############################################################################################
    Public Sub Indirizzo_from_PivaSaCod( _
                                            ByVal Piva As String, _
                                            ByVal Sa_Cod As Integer, _
                                            ByRef Ind_Des As String, _
                                            ByRef Frz_Des As String, _
                                            ByRef CAP As String, _
                                            ByRef Stato As String, _
                                            ByRef Comune As String, _
                                            ByRef Provincia As String, _
                                            ByRef Sigla_Prov As String, _
                                            ByRef pro_cod_istat As String, _
                                            ByRef com_cod_istat As String, _
                                            ByRef Cod_Regione As String, _
                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                            )

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.CentrixIndirizzi_Read.Indirizzo_from_PivaSaCod()"
        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        '---------------

        Try

            Dim obj_CentroInd As New AgronicaCoreAnagrafeDAL.CentrixIndirizzi_Read

            DT = obj_CentroInd.Leggi(CStr(Piva), _
                                      CInt(Sa_Cod), _
                                      0, _
                                      CInt(1), _
                                        AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta, _
                                        "", "", _
                                        objParametri)

            If Not IsNothing(DT) AndAlso DT.Rows.Count > 0 Then

                Ind_Des = DT.Rows(0).Item("ind_des")
                Frz_Des = DT.Rows(0).Item("Frz_Des")
                CAP = DT.Rows(0).Item("CAP")
                Stato = DT.Rows(0).Item("stato")
                Comune = DT.Rows(0).Item("com_des")
                Provincia = DT.Rows(0).Item("pro_des")
                Sigla_Prov = DT.Rows(0).Item("pro_cod")
                pro_cod_istat = DT.Rows(0).Item("pro_cod_istat")
                com_cod_istat = DT.Rows(0).Item("com_cod_istat")
                Cod_Regione = DT.Rows(0).Item("reg")

            End If


        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try


    End Sub

    '##############################################################################################
    'a differenza della 1, legge rag-soc e sa_nome
    Public Sub Indirizzo_from_PivaSaCod2(ByVal Piva As String, _
                                        ByVal Sa_Cod As Integer, _
                                        ByRef rag_soc As String, _
                                        ByRef sa_nome As String, _
                                        ByRef ind_des As String, _
                                        ByRef frz_des As String, _
                                        ByRef CAP As String, _
                                        ByRef com_des As String, _
                                        ByRef pro_cod As String, _
                                        ByRef pro_cod_istat As String, _
                                        ByRef com_cod_istat As String, _
                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                            )

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.CentrixIndirizzi_Read.Indirizzo_from_PivaSaCod2()"
        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        '---------------

        Try

            Dim obj_CentroInd As New AgronicaCoreAnagrafeDAL.CentrixIndirizzi_Read

            DT = obj_CentroInd.Leggi(CStr(Piva), _
                                      CInt(Sa_Cod), _
                                      0, _
                                      CInt(1), _
                                        AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta, _
                                        "", "", _
                                        objParametri)

            If Not IsNothing(DT) AndAlso DT.Rows.Count > 0 Then
                rag_soc = DT.Rows(0).Item("rag_soc")
                sa_nome = DT.Rows(0).Item("sa_nome")
                ind_des = DT.Rows(0).Item("ind_des")
                frz_des = DT.Rows(0).Item("Frz_Des")
                CAP = DT.Rows(0).Item("CAP")
                'Stato = DT.Rows(0).Item("stato")
                com_des = DT.Rows(0).Item("com_des")
                'Provincia = DT.Rows(0).Item("pro_des")
                pro_cod = DT.Rows(0).Item("pro_cod")
                pro_cod_istat = DT.Rows(0).Item("pro_cod_istat")
                com_cod_istat = DT.Rows(0).Item("com_cod_istat")
                'Cod_Regione = DT.Rows(0).Item("reg")
            End If


        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try


    End Sub


    '##############################################################################################
    'unione della 1 e della 2
    Public Sub Indirizzo_from_PivaSaCod3(ByVal Piva As String, _
                                        ByVal Sa_Cod As Integer, _
                                        ByRef rag_soc As String, _
                                        ByRef sa_nome As String, _
                                        ByRef ind_des As String, _
                                        ByRef frz_des As String, _
                                        ByRef CAP As String, _
                                         ByRef Comune As String, _
                                            ByRef Provincia As String, _
                                            ByRef Sigla_Prov As String, _
                                            ByRef Stato As String, _
                                            ByRef pro_cod_istat As String, _
                                            ByRef com_cod_istat As String, _
                                            ByRef Cod_Regione As String, _
                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                            )

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.CentrixIndirizzi_Read.Indirizzo_from_PivaSaCod3()"
        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        '---------------

        Try

            Dim obj_CentroInd As New AgronicaCoreAnagrafeDAL.CentrixIndirizzi_Read

            DT = obj_CentroInd.Leggi(CStr(Piva), _
                                      CInt(Sa_Cod), _
                                      0, _
                                      CInt(1), _
                                        AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta, _
                                        "", "", _
                                        objParametri)

            If Not IsNothing(DT) AndAlso DT.Rows.Count > 0 Then
                rag_soc = DT.Rows(0).Item("rag_soc")
                sa_nome = DT.Rows(0).Item("sa_nome")
                ind_des = DT.Rows(0).Item("ind_des")
                frz_des = DT.Rows(0).Item("Frz_Des")
                CAP = DT.Rows(0).Item("CAP")
                Stato = DT.Rows(0).Item("stato")
                Comune = DT.Rows(0).Item("com_des")
                Provincia = DT.Rows(0).Item("pro_des")
                Sigla_Prov = DT.Rows(0).Item("pro_cod")
                pro_cod_istat = DT.Rows(0).Item("pro_cod_istat")
                com_cod_istat = DT.Rows(0).Item("com_cod_istat")
                Cod_Regione = DT.Rows(0).Item("reg")
            End If


        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try


    End Sub

    '##############################################################################################
    Public Function CodIndirizzo_from_PivaSaCod( _
                                            ByVal Piva As String, _
                                            ByVal Sa_Cod As Integer, _
                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                            ) As Integer

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.CentrixIndirizzi_Read.CodIndirizzo_from_PivaSaCod()"
        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim Cod_Indirizzo As Integer = 0

        '---------------

        Try

            Dim obj_CentroInd As New AgronicaCoreAnagrafeDAL.CentrixIndirizzi_Read

            DT = obj_CentroInd.Leggi(CStr(Piva), _
                                      CInt(Sa_Cod), _
                                      0, _
                                      CInt(1), _
                                        enumSelezioneVariabile.Selezione_TabellaDatiMinimi, _
                                        "", "", _
                                        objParametri)

            If Not IsNothing(DT) AndAlso DT.Rows.Count > 0 Then
                Cod_Indirizzo = DT.Rows(0).Item("Cod_Indirizzo")
            End If


        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return Cod_Indirizzo

    End Function

    '##############################################################################################
    Public Function Regione_from_PivaSaCod(ByVal Piva As String, _
                                        ByVal Sa_Cod As Integer, _
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                        ) As String

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.CentrixIndirizzi_Read.Regione_from_PivaSaCod()"
        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim REG As String = ""

        '---------------

        Try

            Dim obj_CentroInd As New AgronicaCoreAnagrafeDAL.CentrixIndirizzi_Read

            DT = obj_CentroInd.Leggi(CStr(Piva), _
                                      CInt(Sa_Cod), _
                                      0, _
                                      CInt(1), _
                                        enumSelezioneVariabile.Selezione_JoinCompleta, _
                                        "", "", _
                                        objParametri)

            If Not IsNothing(DT) AndAlso DT.Rows.Count > 0 Then
                REG = DT.Rows(0).Item("REG")
            End If


        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return REG

    End Function

    Public Function Stato_from_PivaSaCod(ByVal Piva As String,
                                        ByVal Sa_Cod As Integer,
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                        ) As String

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.CentrixIndirizzi_Read.Stato_from_PivaSaCod()"
        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim Stato_Cod As String = "IT"

        '---------------

        Try

            Dim obj_CentroInd As New AgronicaCoreAnagrafeDAL.CentrixIndirizzi_Read

            DT = obj_CentroInd.Leggi(CStr(Piva),
                                      CInt(Sa_Cod),
                                      0,
                                      CInt(1),
                                        enumSelezioneVariabile.Selezione_JoinCompleta,
                                        "", "",
                                        objParametri)

            If Not IsNothing(DT) AndAlso DT.Rows.Count > 0 AndAlso Not IsDBNull(DT.Rows(0).Item("stato")) Then
                Stato_Cod = DT.Rows(0).Item("stato")
            End If


        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return Stato_Cod

    End Function

    '##############################################################################################
    Public Function SaCod_from_PivaProvComISTAT(ByVal Piva As String, _
                                            ByVal Pro_Cod_Istat As String, _
                                            ByVal Com_Cod_Istat As String, _
                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                            ) As Integer

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.CentrixIndirizzi_Read.SaCod_from_PivaProvComISTAT()"
        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim Sa_Cod As Integer = 0

        Dim filtro As String = " (Indirizzi.pro_cod_istat = '" + Agro_SQL_SaveText(Pro_Cod_Istat) + _
                                    "' AND Indirizzi.com_cod_istat = '" + Agro_SQL_SaveText(Com_Cod_Istat) + _
                                    "' ) "

        '---------------

        Try

            DT = Leggi(CStr(Piva), _
                        0, _
                        0, _
                        CInt(1), _
                        enumSelezioneVariabile.Selezione_JoinCompleta, _
                        filtro, _
                        "", _
                        objParametri)

            If Not IsNothing(DT) AndAlso DT.Rows.Count > 0 Then
                Sa_Cod = DT.Rows(0).Item("Sa_cod")
            End If


        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return Sa_Cod

    End Function

    '##############################################################################################
    'a differenza della precedente restituisce anche il sa_nome
    Public Function SaCod_from_PivaProvComISTAT(ByVal Piva As String, _
                                            ByVal Pro_Cod_Istat As String, _
                                            ByVal Com_Cod_Istat As String, _
                                            ByRef Sa_Nome As String, _
                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                            ) As Integer

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.CentrixIndirizzi_Read.SaCod_from_PivaProvComISTAT()"
        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim Sa_Cod As Integer = 0

        Dim filtro As String = " (Indirizzi.pro_cod_istat = '" + Agro_SQL_SaveText(Pro_Cod_Istat) + _
                                    "' AND Indirizzi.com_cod_istat = '" + Agro_SQL_SaveText(Com_Cod_Istat) + _
                                    "' ) "

        '---------------

        Try

            DT = Leggi(CStr(Piva), _
                        0, _
                        0, _
                        CInt(1), _
                        enumSelezioneVariabile.Selezione_JoinCompleta, _
                        filtro, _
                        "", _
                        objParametri)

            If Not IsNothing(DT) AndAlso DT.Rows.Count > 0 Then
                Sa_Cod = DT.Rows(0).Item("Sa_cod")
            End If


        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return Sa_Cod

    End Function



    '##############################################################################################
    'Lettura degli stati mettendo come primo in elenco lo stato del centro dell'indirizzo
    Public Function Leggi_Stati_Default_Centro(ByVal piva As String,
                                               ByVal sa_cod As Integer,
                                               ByVal xFiltroAggiuntivo As String,
                                               ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                               ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.CentrixIndirizzi_Read.Leggi_Stati_Default_Centro()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.Append(" Select Distinct ACCDAA_ANAG_T005_TabellaCodiciPaesiTerziISO3166.Codice,  ACCDAA_ANAG_T005_TabellaCodiciPaesiTerziISO3166.Descrizione, ")
            StrSQL.Append("       (Select Top 1 Isnull(Indirizzi.Stato, '') From CentrixIndirizzi  ")
            StrSQL.Append("       Left outer join Indirizzi On (CentrixIndirizzi.Cod_Indirizzo = Indirizzi.Cod_Indirizzo And ")
            StrSQL.Append("       Upper(ACCDAA_ANAG_T005_TabellaCodiciPaesiTerziISO3166.Codice) = Upper(Indirizzi.Stato))  ")
            StrSQL.Append("       WHERE CentrixIndirizzi.Piva = '" & Agro_SQL_SaveText(piva) & "' And ")
            StrSQL.Append("       CentrixIndirizzi.Sa_Cod = " & Agro_SQL_SaveNum(sa_cod) & ") As Stato_Centro ")

            StrSQL.Append(" FROM Zoo_Barcode, ACCDAA_ANAG_T005_TabellaCodiciPaesiTerziISO3166 ")
            StrSQL.Append(" Where Upper(ACCDAA_ANAG_T005_TabellaCodiciPaesiTerziISO3166.Codice) = Upper(Zoo_Barcode.Codice) ")


            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & xFiltroAggiuntivo + vbCrLf)
            End If

            StrSQL.Append(" ORDER BY Stato_Centro Desc, Descrizione Asc")

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


Public Class CentrixIndirizzi_Write
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    'Public Function Scrivi( _
    '                        ByVal Piva As String, _
    '                        ByVal Sa_Cod As Int32, _
    '                        ByVal cod_indirizzo As Int32, _
    '                        ByVal Tipo_Indirizzo As Int32, _
    '                        ByVal UserName_Creazione As String, _
    '                        ByVal FinestraTemp_Inizio As Date, _
    '                        ByVal FinestraTemp_Fine As Date, _
    '                        ByRef objConnessione As DbConnection, _
    '                        ByRef objTransazione As DbTransaction, _
    '                        ByVal StringaConnessione As String, _
    '                        ByVal DirectoryLOG As String, _
    '                        ByVal FileLOG As String, _
    '                        ByVal IdentificatoreUtente As String _
    '                        ) As Boolean

    '    Dim NomeRoutine As String = "AnagrafeCoreAnagrafeDAL.CentrixIndirizzi_Write.Scrivi()"

    '    '====================================================================================
    '    'Parametri opzionali :
    '    '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
    '    '   DirectoryLOG = ""           =>  viene usato il valore di default
    '    '   FileLOG = ""                =>  viene usato il valore di default
    '    '====================================================================================

    '    Dim MessaggioErrore As String = ""
    '    Dim StrSQL As New System.Text.StringBuilder
    '    Dim xRisp As Boolean = False

    '    Try
    '        '---------------------------------------------
    '        StrSQL.Length = 0
    '        StrSQL.Append("INSERT INTO CentrixIndirizzi( ")
    '        StrSQL.Append("                    Piva,      ")
    '        StrSQL.Append("                    Sa_Cod,      ")
    '        StrSQL.Append("                    Cod_Indirizzo,    ")
    '        StrSQL.Append("                    Tipo_Indirizzo,    ")
    '        StrSQL.Append("                    Inviato, DataInvio, ")
    '        StrSQL.Append("                    Data_Creazione,     Data_Modifica, ")
    '        StrSQL.Append("                    UserName_Creazione, UserName_Modifica, ")
    '        StrSQL.Append("                    Validita_Inizio,    Validita_Fine ")
    '        StrSQL.Append("                    ) ")
    '        StrSQL.Append("VALUES (")
    '        StrSQL.Append("          '" & Agro_SQL_SaveText(Piva) & "' ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(cod_indirizzo) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveNum(Tipo_Indirizzo) & "  ")
    '        StrSQL.Append("         , 0  ")
    '        StrSQL.Append("         , Null  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveDate(Date.Now) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveDate(Date.Now) & "  ")
    '        StrSQL.Append("         ,'" & Agro_SQL_SaveText(UserName_Creazione) & "' ")
    '        StrSQL.Append("         ,'" & Agro_SQL_SaveText(UserName_Creazione) & "' ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveDate(FinestraTemp_Inizio) & "  ")
    '        StrSQL.Append("         , " & Agro_SQL_SaveDate(FinestraTemp_Fine) & "  ")
    '        StrSQL.Append(")")
    '        '---------------------------------------------

    '        xRisp = EseguiQuery_Scrittura(objConnessione, objTransazione, StringaConnessione, StrSQL.ToString, DirectoryLOG, FileLOG, IdentificatoreUtente, NomeRoutine)

    '    Catch ex As Exception

    '        MessaggioErrore = ex.Message
    '        Scrivi_LOG(DirectoryLOG, FileLOG, IdentificatoreUtente, NomeRoutine, MessaggioErrore)
    '        xRisp = False
    '        Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

    '    End Try

    '    Return xRisp

    'End Function

    Public Function Scrivi( _
                            ByVal Piva As String, _
                            ByVal Sa_Cod As Int32, _
                            ByVal cod_indirizzo As Int32, _
                            ByVal Tipo_Indirizzo As Int32, _
                                ByVal Validita_Inizio As Date, _
                                ByVal Validita_Fine As Date, _
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                , Optional ByVal Data_creazione As Date = #2/1/1900# _
                , Optional ByVal Data_modifica As Date = #2/1/1900# _
                , Optional ByVal username_creazione As String = "" _
                , Optional ByVal username_modifica As String = "" _
                                    ) As Boolean

        Dim NomeRoutine As String = "AnagrafeCoreAnagrafeDAL.CentrixIndirizzi_Write.Scrivi()"

        '====================================================================================
        'Parametri opzionali :
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

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


        Try
            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append("INSERT INTO CentrixIndirizzi( ")
            StrSQL.Append("                    Piva,      ")
            StrSQL.Append("                    Sa_Cod,      ")
            StrSQL.Append("                    Cod_Indirizzo,    ")
            StrSQL.Append("                    Tipo_Indirizzo,    ")
            StrSQL.Append("                    Inviato, DataInvio, ")
            StrSQL.Append("                    Data_Creazione,     Data_Modifica, ")
            StrSQL.Append("                    UserName_Creazione, UserName_Modifica, ")
            StrSQL.Append("                    Validita_Inizio,    Validita_Fine ")
            StrSQL.Append("                    ) ")
            StrSQL.Append("VALUES (")
            StrSQL.Append("          '" & Agro_SQL_SaveText(Piva) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(cod_indirizzo) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Tipo_Indirizzo) & "  ")
            StrSQL.Append("         , 0  ")
            StrSQL.Append("         , Null  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Data_creazione) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Data_modifica) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(username_modifica) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")
            StrSQL.Append(")")
            '---------------------------------------------

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
    'Public Function Modifica( _
    '                        ByVal Piva As String, _
    '                        ByVal Sa_Cod As Int32, _
    '                        ByVal cod_indirizzo As Int32, _
    '                        ByVal Tipo_Indirizzo As Int32, _
    '                        ByVal UserName_Modifica As String, _
    '                        ByVal FinestraTemp_Inizio As Date, _
    '                        ByVal FinestraTemp_Fine As Date, _
    '                        ByRef objConnessione As DbConnection, _
    '                        ByRef objTransazione As DbTransaction, _
    '                        ByVal StringaConnessione As String, _
    '                        ByVal DirectoryLOG As String, _
    '                        ByVal FileLOG As String, _
    '                        ByVal IdentificatoreUtente As String _
    '                        ) As Boolean

    '    Dim NomeRoutine As String = "AnagrafeCoreAnagrafeDAL.CentrixIndirizzi_Write.Modifica()"

    '    '====================================================================================
    '    'Parametri opzionali :
    '    '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
    '    '   DirectoryLOG = ""           =>  viene usato il valore di default
    '    '   FileLOG = ""                =>  viene usato il valore di default
    '    '====================================================================================

    '    Dim MessaggioErrore As String = ""
    '    Dim StrSQL As New System.Text.StringBuilder
    '    Dim xRisp As Boolean = False

    '    Try
    '        '---------------------------------------------
    '        StrSQL.Length = 0
    '        StrSQL.Append("UPDATE CentrixIndirizzi SET ")
    '        StrSQL.Append("    Inviato           =  0 ")
    '        StrSQL.Append("   ,DataInvio         =  Null ")
    '        StrSQL.Append("   ,Data_Modifica     =  " & Agro_SQL_SaveDate(Date.Now))
    '        StrSQL.Append("   ,UserName_Modifica = '" & Agro_SQL_SaveText(UserName_Modifica) & "'")
    '        StrSQL.Append("   ,Validita_Inizio   =  " & Agro_SQL_SaveDate(FinestraTemp_Inizio))
    '        StrSQL.Append("   ,Validita_Fine     =  " & Agro_SQL_SaveDate(FinestraTemp_Fine))
    '        StrSQL.Append(" WHERE Piva = '" & Agro_SQL_SaveText(Piva) & "'")
    '        StrSQL.Append(" AND   Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
    '        StrSQL.Append(" AND   Cod_Indirizzo = " & Agro_SQL_SaveNum(cod_indirizzo) & " ")
    '        StrSQL.Append(" AND   Tipo_Indirizzo = " & Agro_SQL_SaveNum(Tipo_Indirizzo) & " ")
    '        '---------------------------------------------

    '        xRisp = EseguiQuery_Scrittura(objConnessione, objTransazione, StringaConnessione, StrSQL.ToString, DirectoryLOG, FileLOG, IdentificatoreUtente, NomeRoutine)

    '    Catch ex As Exception

    '        MessaggioErrore = ex.Message
    '        Scrivi_LOG(DirectoryLOG, FileLOG, IdentificatoreUtente, NomeRoutine, MessaggioErrore)
    '        xRisp = False
    '        Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

    '    End Try

    '    Return xRisp

    'End Function

    Public Function Modifica( _
                            ByVal Piva As String, _
                            ByVal Sa_Cod As Int32, _
                            ByVal cod_indirizzo As Int32, _
                            ByVal Tipo_Indirizzo As Int32, _
                                ByVal Validita_Inizio As Date, _
                                ByVal Validita_Fine As Date, _
                                ByVal xFiltroAggiuntivo As String, _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                ) As Boolean

        Dim NomeRoutine As String = "AnagrafeCoreAnagrafeDAL.CentrixIndirizzi_Write.Modifica()"

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
            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append("UPDATE CentrixIndirizzi SET ")
            StrSQL.Append("    Inviato           =  0 ")
            StrSQL.Append("   ,DataInvio         =  Null ")
            StrSQL.Append("   ,Data_Modifica     =  " & Agro_SQL_SaveDate(Date.Now))
            StrSQL.Append("   ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.Append("   ,Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Inizio))
            StrSQL.Append("   ,Validita_Fine     =  " & Agro_SQL_SaveDate(Validita_Fine))
            StrSQL.Append(" WHERE Piva = '" & Agro_SQL_SaveText(Piva) & "'")
            StrSQL.Append(" AND   Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            StrSQL.Append(" AND   Cod_Indirizzo = " & Agro_SQL_SaveNum(cod_indirizzo) & " ")
            StrSQL.Append(" AND   Tipo_Indirizzo = " & Agro_SQL_SaveNum(Tipo_Indirizzo) & " ")
            '---------------------------------------------


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

    '##############################################################################################
    'Public Function Cancella( _
    '                        ByVal Piva As String, _
    '                        ByVal Sa_Cod As Int32, _
    '                        ByVal cod_indirizzo As Int32, _
    '                        ByVal Tipo_Indirizzo As Int32, _
    '                        ByVal UserName_Modifica As String, _
    '                        ByVal FlagCancellazioneLogica As Int32, _
    '                        ByRef objConnessione As DbConnection, _
    '                        ByRef objTransazione As DbTransaction, _
    '                        ByVal StringaConnessione As String, _
    '                        ByVal DirectoryLOG As String, _
    '                        ByVal FileLOG As String, _
    '                        ByVal IdentificatoreUtente As String _
    '                        ) As Boolean

    '    Dim NomeRoutine As String = "AnagrafeCoreAnagrafeDAL.CentrixIndirizzi_Write.Cancella()"

    '    '====================================================================================
    '    'Parametri opzionali :
    '    '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
    '    '   DirectoryLOG = ""           =>  viene usato il valore di default
    '    '   FileLOG = ""                =>  viene usato il valore di default
    '    '====================================================================================

    '    Dim MessaggioErrore As String = ""
    '    Dim StrSQL As New System.Text.StringBuilder
    '    Dim xRisp As Boolean = False

    '    Try


    '        '---------------------------------------------
    '        If FlagCancellazioneLogica Then

    '            StrSQL.Length = 0
    '            StrSQL.Append(" UPDATE CentrixIndirizzi ")
    '            StrSQL.Append(" SET ")
    '            StrSQL.Append("      Username_Modifica = '" & Agro_SQL_SaveText(UserName_Modifica) & "' ")
    '            StrSQL.Append("      ,Inviato = -1 ")
    '            StrSQL.Append(" WHERE  Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
    '            StrSQL.Append(" AND Inviato >= 0 ")

    '            If Sa_Cod <> 0 Then
    '                StrSQL.Append(" AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
    '            End If

    '            If cod_indirizzo <> 0 Then

    '                StrSQL.Append(" AND Cod_Indirizzo = " & Agro_SQL_SaveNum(cod_indirizzo) & " ")

    '                If Tipo_Indirizzo <> 0 Then

    '                    StrSQL.Append(" AND Tipo_Indirizzo = " & Agro_SQL_SaveNum(Tipo_Indirizzo) & " ")

    '                End If

    '            End If

    '        Else

    '            StrSQL.Length = 0
    '            StrSQL.Append(" DELETE ")
    '            StrSQL.Append(" FROM     CentrixIndirizzi ")
    '            StrSQL.Append(" WHERE    Piva = '" & Agro_SQL_SaveText(Piva) & "' ")

    '            If Sa_Cod <> 0 Then
    '                StrSQL.Append(" AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
    '            End If

    '            If cod_indirizzo <> 0 Then

    '                StrSQL.Append(" AND      Cod_Indirizzo = " & Agro_SQL_SaveNum(cod_indirizzo) & " ")

    '                If Tipo_Indirizzo <> 0 Then

    '                    StrSQL.Append(" AND      Tipo_Indirizzo = " & Agro_SQL_SaveNum(Tipo_Indirizzo) & " ")

    '                End If

    '            End If

    '        End If
    '        '---------------------------------------------

    '        xRisp = EseguiQuery_Scrittura(objConnessione, objTransazione, StringaConnessione, StrSQL.ToString, DirectoryLOG, FileLOG, IdentificatoreUtente, NomeRoutine)

    '    Catch ex As Exception

    '        MessaggioErrore = ex.Message
    '        Scrivi_LOG(DirectoryLOG, FileLOG, IdentificatoreUtente, NomeRoutine, MessaggioErrore)
    '        xRisp = False
    '        Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

    '    End Try

    '    Return xRisp

    'End Function

    Public Function Cancella( _
                            ByVal Piva As String, _
                            ByVal Sa_Cod As Int32, _
                            ByVal cod_indirizzo As Int32, _
                            ByVal Tipo_Indirizzo As Int32, _
                                ByVal xFiltroAggiuntivo As String, _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                ) As Boolean

        Dim NomeRoutine As String = "AnagrafeCoreAnagrafeDAL.CentrixIndirizzi_Write.Cancella()"

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


            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = AgronicaCoreDataProvider.AgronicaCoreParametri.enumCancellazioneLogica.CancellazioneLogica Then

                StrSQL.Length = 0
                StrSQL.Append(" UPDATE CentrixIndirizzi ")
                StrSQL.Append(" SET ")
                StrSQL.Append("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("      ,Inviato = -1 ")
                StrSQL.Append(" WHERE  Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
                StrSQL.Append(" AND Inviato >= 0 ")

                If Sa_Cod <> 0 Then
                    StrSQL.Append(" AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
                End If

                If cod_indirizzo <> 0 Then

                    StrSQL.Append(" AND Cod_Indirizzo = " & Agro_SQL_SaveNum(cod_indirizzo) & " ")

                    If Tipo_Indirizzo <> 0 Then

                        StrSQL.Append(" AND Tipo_Indirizzo = " & Agro_SQL_SaveNum(Tipo_Indirizzo) & " ")

                    End If

                End If

            Else

                StrSQL.Length = 0
                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM     CentrixIndirizzi ")
                StrSQL.Append(" WHERE    Piva = '" & Agro_SQL_SaveText(Piva) & "' ")

                If Sa_Cod <> 0 Then
                    StrSQL.Append(" AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
                End If

                If cod_indirizzo <> 0 Then

                    StrSQL.Append(" AND      Cod_Indirizzo = " & Agro_SQL_SaveNum(cod_indirizzo) & " ")

                    If Tipo_Indirizzo <> 0 Then

                        StrSQL.Append(" AND      Tipo_Indirizzo = " & Agro_SQL_SaveNum(Tipo_Indirizzo) & " ")

                    End If

                End If

            End If
            '---------------------------------------------


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

    '##############################################################################################
    'Public Function AggiornaValiditaInizio( _
    '                        ByVal PIVA As String, _
    '                        ByVal Sa_Cod As Int32, _
    '                        ByVal cod_indirizzo As Int32, _
    '                        ByVal Tipo_Indirizzo As Int32, _
    '                        ByVal UserName_Modifica As String, _
    '                        ByVal FinestraTemp_Inizio As Date, _
    '                        ByRef objConnessione As DbConnection, _
    '                        ByRef objTransazione As DbTransaction, _
    '                        ByVal StringaConnessione As String, _
    '                        ByVal DirectoryLOG As String, _
    '                        ByVal FileLOG As String, _
    '                        ByVal IdentificatoreUtente As String _
    '                        ) As Boolean

    '    Dim NomeRoutine As String = "AnagrafeCoreAnagrafeDAL.CentrixIndirizzi_Write.AggiornaValiditaInizio()"

    '    '====================================================================================
    '    'Parametri opzionali :
    '    '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
    '    '   DirectoryLOG = ""           =>  viene usato il valore di default
    '    '   FileLOG = ""                =>  viene usato il valore di default
    '    '====================================================================================

    '    Dim MessaggioErrore As String = ""
    '    Dim StrSQL As New System.Text.StringBuilder
    '    Dim xRisp As Boolean = False

    '    Try
    '        '---------------------------------------------
    '        StrSQL.Length = 0
    '        StrSQL.Append("UPDATE CentrixIndirizzi SET ")
    '        StrSQL.Append("   UserName_Modifica = '" & Agro_SQL_SaveText(UserName_Modifica) & "'")
    '        StrSQL.Append("   ,Validita_Inizio   =  " & Agro_SQL_SaveDate(FinestraTemp_Inizio))
    '        StrSQL.Append(" WHERE Piva = '" & Agro_SQL_SaveText(PIVA) & "' ")
    '        StrSQL.Append(" AND   Validita_Inizio < " & Agro_SQL_SaveDate(FinestraTemp_Inizio))

    '        If Sa_Cod <> 0 Then
    '            StrSQL.Append(" AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
    '        End If

    '        If cod_indirizzo <> 0 Then

    '            StrSQL.Append(" AND Cod_Indirizzo = " & Agro_SQL_SaveNum(cod_indirizzo) & " ")

    '            If Tipo_Indirizzo <> 0 Then

    '                StrSQL.Append(" AND Tipo_Indirizzo = " & Agro_SQL_SaveNum(Tipo_Indirizzo) & " ")

    '            End If

    '        End If
    '        '---------------------------------------------

    '        xRisp = EseguiQuery_Scrittura(objConnessione, objTransazione, StringaConnessione, StrSQL.ToString, DirectoryLOG, FileLOG, IdentificatoreUtente, NomeRoutine)

    '    Catch ex As Exception

    '        MessaggioErrore = ex.Message
    '        Scrivi_LOG(DirectoryLOG, FileLOG, IdentificatoreUtente, NomeRoutine, MessaggioErrore)
    '        xRisp = False
    '        Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

    '    End Try

    '    Return xRisp

    'End Function

    Public Function AggiornaValiditaInizio( _
                            ByVal PIVA As String, _
                            ByVal Sa_Cod As Int32, _
                            ByVal cod_indirizzo As Int32, _
                            ByVal Tipo_Indirizzo As Int32, _
                                ByVal Validita_Inizio As Date, _
                                    ByVal xFiltroAggiuntivo As String, _
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                    ) As Boolean

        Dim NomeRoutine As String = "AnagrafeCoreAnagrafeDAL.CentrixIndirizzi_Write.AggiornaValiditaInizio()"

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
            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append("UPDATE CentrixIndirizzi SET ")
            StrSQL.Append("   UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.Append("   ,Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Inizio))
            StrSQL.Append(" WHERE Piva = '" & Agro_SQL_SaveText(PIVA) & "' ")
            StrSQL.Append(" AND   Validita_Inizio < " & Agro_SQL_SaveDate(Validita_Inizio))

            If Sa_Cod <> 0 Then
                StrSQL.Append(" AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            End If

            If cod_indirizzo <> 0 Then

                StrSQL.Append(" AND Cod_Indirizzo = " & Agro_SQL_SaveNum(cod_indirizzo) & " ")

                If Tipo_Indirizzo <> 0 Then

                    StrSQL.Append(" AND Tipo_Indirizzo = " & Agro_SQL_SaveNum(Tipo_Indirizzo) & " ")

                End If

            End If
            '---------------------------------------------


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

    '##############################################################################################
    'Public Function AggiornaValiditaFine( _
    '                        ByVal PIVA As String, _
    '                        ByVal Sa_Cod As Int32, _
    '                        ByVal cod_indirizzo As Int32, _
    '                        ByVal Tipo_Indirizzo As Int32, _
    '                        ByVal UserName_Modifica As String, _
    '                        ByVal FinestraTemp_Fine As Date, _
    '                        ByRef objConnessione As DbConnection, _
    '                        ByRef objTransazione As DbTransaction, _
    '                        ByVal StringaConnessione As String, _
    '                        ByVal DirectoryLOG As String, _
    '                        ByVal FileLOG As String, _
    '                        ByVal IdentificatoreUtente As String _
    '                        ) As Boolean

    '    Dim NomeRoutine As String = "AnagrafeCoreAnagrafeDAL.CentrixIndirizzi_Write.AggiornaValiditaFine()"

    '    '====================================================================================
    '    'Parametri opzionali :
    '    '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
    '    '   DirectoryLOG = ""           =>  viene usato il valore di default
    '    '   FileLOG = ""                =>  viene usato il valore di default
    '    '====================================================================================

    '    Dim MessaggioErrore As String = ""
    '    Dim StrSQL As New System.Text.StringBuilder
    '    Dim xRisp As Boolean = False

    '    Try
    '        '---------------------------------------------
    '        StrSQL.Length = 0
    '        StrSQL.Append("UPDATE CentrixIndirizzi SET ")
    '        StrSQL.Append("   UserName_Modifica = '" & Agro_SQL_SaveText(UserName_Modifica) & "'")
    '        StrSQL.Append("   ,Validita_Fine   =  " & Agro_SQL_SaveDate(FinestraTemp_Fine))
    '        StrSQL.Append(" WHERE Piva = '" & Agro_SQL_SaveText(PIVA) & "' ")
    '        StrSQL.Append(" AND   Validita_Fine > " & Agro_SQL_SaveDate(FinestraTemp_Fine))

    '        If Sa_Cod <> 0 Then
    '            StrSQL.Append(" AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
    '        End If

    '        If cod_indirizzo <> 0 Then

    '            StrSQL.Append(" AND Cod_Indirizzo = " & Agro_SQL_SaveNum(cod_indirizzo) & " ")

    '            If Tipo_Indirizzo <> 0 Then

    '                StrSQL.Append(" AND Tipo_Indirizzo = " & Agro_SQL_SaveNum(Tipo_Indirizzo) & " ")

    '            End If

    '        End If
    '        '---------------------------------------------

    '        xRisp = EseguiQuery_Scrittura(objConnessione, objTransazione, StringaConnessione, StrSQL.ToString, DirectoryLOG, FileLOG, IdentificatoreUtente, NomeRoutine)

    '    Catch ex As Exception

    '        MessaggioErrore = ex.Message
    '        Scrivi_LOG(DirectoryLOG, FileLOG, IdentificatoreUtente, NomeRoutine, MessaggioErrore)
    '        xRisp = False
    '        Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

    '    End Try

    '    Return xRisp

    'End Function

    Public Function AggiornaValiditaFine( _
                                ByVal PIVA As String, _
                                ByVal Sa_Cod As Int32, _
                                ByVal cod_indirizzo As Int32, _
                                ByVal Tipo_Indirizzo As Int32, _
                                    ByVal Validita_Fine As Date, _
                                        ByVal xFiltroAggiuntivo As String, _
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                        ) As Boolean

        Dim NomeRoutine As String = "AnagrafeCoreAnagrafeDAL.CentrixIndirizzi_Write.AggiornaValiditaFine()"

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
            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append("UPDATE CentrixIndirizzi SET ")
            StrSQL.Append("   UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.Append("   ,Validita_Fine   =  " & Agro_SQL_SaveDate(Validita_Fine))
            StrSQL.Append(" WHERE Piva = '" & Agro_SQL_SaveText(PIVA) & "' ")
            StrSQL.Append(" AND   Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine))

            If Sa_Cod <> 0 Then
                StrSQL.Append(" AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            End If

            If cod_indirizzo <> 0 Then

                StrSQL.Append(" AND Cod_Indirizzo = " & Agro_SQL_SaveNum(cod_indirizzo) & " ")

                If Tipo_Indirizzo <> 0 Then

                    StrSQL.Append(" AND Tipo_Indirizzo = " & Agro_SQL_SaveNum(Tipo_Indirizzo) & " ")

                End If

            End If
            '---------------------------------------------

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




    Public Function Aggiorna_Indirizzo(ByVal PIVA As String, _
                                       ByVal Sa_Cod As Integer, _
                                    ByVal Cod_Indirizzo_Da_Eliminare As Integer, _
                                    ByRef Cod_Indirizzo_Eliminato As Integer, _
                                    ByRef Num_Indirizzi_Eliminati As Integer, _
                                    ByVal PermettiEliminazioneUnSoloIndirizzo As Boolean, _
                                    ByRef Cod_Indirizzo_New As Integer, _
                                    ByVal Tipo_Indirizzo As Integer, _
                                    ByVal Ind_Des As String, _
                                    ByVal Frz_Des As String, _
                                    ByVal CAP As String, _
                                    ByVal Com_Des As String, _
                                    ByVal Pro_Cod As String, _
                                    ByVal Stato As String, _
                                    ByVal Note As String, _
                                    ByVal Pro_Cod_Istat As String, _
                                    ByVal Com_Cod_Istat As String, _
                                    ByVal Validita_Inizio As Date, _
                                    ByVal Validita_Fine As Date, _
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                                   Optional ByVal Data_creazione As Date = #2/1/1900#, _
                                   Optional ByVal Data_modifica As Date = #2/1/1900#, _
                                  Optional ByVal username_creazione As String = "", _
                                   Optional ByVal username_modifica As String = "" _
                                      ) As Boolean


        'per evitare di eliminare tutto
        If PIVA = "" Then
            Throw New Exception("Occorre specificare una Piva")
        End If

        Dim xRisp As Boolean = False

        'INDIRIZZI

        'Persona fisica
        'Codice            Tipo indirizzo
        '2	                Domicilio
        '3	                Residenza
        '4	                Residenza Estiva
        '5	                Luogo di nascita

        'Persona giuridica
        'Codice            Tipo indirizzo
        '1	                Sede operativa
        '101	            Sede legale
        '102	            Sede aziendale
        '103	            Stabilimento

        'Dim objImpresa As New AgronicaCoreAnagrafeDAL.Imprese_Read
        Dim indirizzi As New AgronicaCoreAnagrafeDAL.Indirizzi_Write
        Dim CentrixIndirizzi_Read As New AgronicaCoreAnagrafeDAL.CentrixIndirizzi_Read


        Dim dt As DataTable = CentrixIndirizzi_Read.Leggi(PIVA, Sa_Cod, Cod_Indirizzo_Da_Eliminare, Tipo_Indirizzo, enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri)

        If dt.Rows.Count > 0 Then

            If PermettiEliminazioneUnSoloIndirizzo AndAlso dt.Rows.Count > 1 Then
                Throw New Exception("Impossibile eliminare più indirizzi contemporaneamente con PermettiEliminazioneUnSoloIndirizzo=true")
            End If

            Dim i As Integer
            For i = 0 To dt.Rows.Count - 1

                If dt.Rows(i).Item("PIVA") = "" Or dt.Rows(i).Item("PIVA") <> PIVA Then
                    Throw New Exception(" dt.Rows(i).Item(PIVA) =  Or dt.Rows(i).Item(PIVA) <> PIVA ")
                End If
                If dt.Rows(i).Item("Sa_Cod") <> Sa_Cod Then
                    Throw New Exception(" dt.Rows(i).Item(Sa_Cod) =  Or dt.Rows(i).Item(Sa_Cod) <> Sa_Cod ")
                End If

                Cod_Indirizzo_Eliminato = dt.Rows(i).Item("Cod_Indirizzo")
                Cancella(PIVA, Sa_Cod, Cod_Indirizzo_Eliminato, Tipo_Indirizzo, "", objParametri)
                indirizzi.Cancella(Cod_Indirizzo_Eliminato, "", objParametri)

                Num_Indirizzi_Eliminati = i + 1

            Next

        Else

        End If

        Cod_Indirizzo_New = indirizzi.ScriviNuovo(Ind_Des, _
                              Frz_Des, _
                              CAP, _
                              Com_Des, _
                              Pro_Cod, _
                              Stato, _
                              Note, _
                              Pro_Cod_Istat, _
                              Com_Cod_Istat, _
                                   Validita_Inizio, _
                                   Validita_Fine, _
                                      objParametri)
        xRisp = Scrivi(PIVA, Sa_Cod, Cod_Indirizzo_New, Tipo_Indirizzo, AgronicaCoreDataProvider.CostantiPersonalizzate.AGRODATAINIZIO, AgronicaCoreDataProvider.CostantiPersonalizzate.AGRODATAFINE, objParametri)

        Return xRisp

    End Function






End Class



'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§