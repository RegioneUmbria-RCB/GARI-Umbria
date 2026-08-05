Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.DataProviderExtensions
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports System.Text.RegularExpressions
Imports AgronicaCoreDataProvider

Public Class CentriAziendali_Read
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function CentroAziendaliLatLongDescrizione(
            ByVal Piva As String,
            ByVal Sa_Cod As Int32,
            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
            ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.CentriAziendali_Read.VerificaEsistenzaCAsuG2G()"

        Dim esiste As Boolean = Nothing
        Dim stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            stb.Length = 0

            stb.AppendLine("    Select top 1   ")
            stb.AppendLine("    Long ")
            stb.AppendLine("  , lat ")
            stb.AppendLine("  , sa_nome + ': ' + ISNULL( ")
            stb.AppendLine("     i.LOCALITA + ' (' + ")
            stb.AppendLine("                 i.COMUNI_PROV +')', '') as Descrizione ")
            stb.AppendLine("  ")
            stb.AppendLine("                 From centri_Aziendali c ")
            stb.AppendLine("    inner Join CentrixIndirizzi ci ")
            stb.AppendLine("         On c.PIVA = ci.PIVA ")
            stb.AppendLine("      And c.sa_cod = ci.sa_cod ")
            stb.AppendLine("  inner Join Indirizzi ind ")
            stb.AppendLine("         On ind.cod_indirizzo = ci.cod_indirizzo ")
            stb.AppendLine("  Left Join ISTAT i ")
            stb.AppendLine("         On i.PROV = ind.pro_cod_istat ")
            stb.AppendLine("      And i.COM = ind.com_cod_istat ")
            stb.AppendLine(" where c.PIVA = '" & Agro_SQL_SaveText(Piva) & "'")
            stb.AppendLine(" order by lat desc, long desc ")

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------


        Catch ex As Exception
            Scrivi_LOG(objParametri, NomeRoutine, ex.Message)
            Throw New Exception("[" & NomeRoutine & "] : " & ex.Message)
        End Try

        Return DT
    End Function

    '################################################################################
    Public Function SaNome_from_SaCod(
                                    ByVal Piva As String,
                                    ByVal Sa_Cod As Integer,
                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                            ) As String

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.CentriAziendaliRead.SaNome_from_SaCod()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder


        Dim DT As DataTable
        Dim objCentri As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read
        DT = objCentri.Leggi(Piva, Sa_Cod,
                                AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                "",
                                "",
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
    Public Function SaCod_PrimoInOrdineAlfabeticoSaNome(ByVal Piva As String,
                                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                        ) As Integer

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.CentriAziendaliRead.SaCod_PrimoInOrdineAlfabeticoSaNome()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim Sa_Cod As Integer = 0

        Dim DT As DataTable
        Dim objCentri As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read
        DT = objCentri.Leggi(Piva, 0,
                                AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                "",
                                " Sa_Nome ASC ",
                                objParametri)

        If Not IsNothing(DT) AndAlso DT.Rows.Count > 0 Then
            Sa_Cod = DT.Rows(0).Item("Sa_Cod")
        End If

        Return Sa_Cod

    End Function

    Public Function VerificaEsistenzaCAsuG2G(
                                             ByVal Piva As String,
                                             ByVal Sa_Cod As Int32,
                                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                             ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.CentriAziendali_Read.VerificaEsistenzaCAsuG2G()"

        Dim esiste As Boolean = Nothing
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            StrSQL.Length = 0
            StrSQL.Append(" SELECT from_Piva, from_SaCod, from_PivaSuperUser")
            StrSQL.Append(" FROM  G2G_Recode_Imprese ")
            StrSQL.Append(" WHERE  from_Piva = '" & Agro_SQL_SaveText(Piva) & "'")
            StrSQL.Append(" AND  from_SaCod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            If Not IsNothing(DT) AndAlso DT.Rows.Count > 0 Then
                esiste = True
            Else
                esiste = False
            End If

        Catch ex As Exception
            Scrivi_LOG(objParametri, NomeRoutine, ex.Message)
            Throw New Exception("[" & NomeRoutine & "] : " & ex.Message)
        End Try

        Return esiste
    End Function

    Public Function Leggi(
                         ByVal Piva As String,
                         ByVal Sa_Cod As Int32,
                             ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile,
                             ByVal xFiltroAggiuntivo As String,
                             ByVal xOrderBy As String,
                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                             ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.CentriAziendali_Read.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""                   =>  si leggono tutte le imprese
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim i As Integer

        Try
            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi
                    StrSQL.Length = 0
                    StrSQL.AppendLine(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;")
                    StrSQL.Append(" SELECT PIVA , Sa_Cod, Sa_Nome, Validita_Inizio , Validita_Fine, inviato")
                    StrSQL.Append(" FROM  Centri_Aziendali ")
                    StrSQL.Append(" WHERE Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND   Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    If Piva <> "" Then
                        StrSQL.Append(" AND  Piva = '" & Agro_SQL_SaveText(Piva) & "'")
                    End If

                    If Sa_Cod <> 0 Then
                        StrSQL.Append(" AND  Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
                    Else
                        'leggo se ci sono filtri sui centri
                        Dim objUtentiVisibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
                        Dim FiltroCentri As String = ""
                        Dim DtCentriVisibili As DataTable
                        DtCentriVisibili = objUtentiVisibilita.Leggi(enum_TipoEntita.Centro, " Piva='" & Agro_SQL_SaveText(Piva) & "'", "", objParametri)
                        If Not DtCentriVisibili Is Nothing AndAlso DtCentriVisibili.Rows.Count > 0 Then
                            For i = 0 To DtCentriVisibili.Rows.Count - 1
                                FiltroCentri &= DtCentriVisibili.Rows(i).Item("sa_cod") & ","
                            Next
                            If FiltroCentri <> "" Then
                                StrSQL.Append(" AND sa_cod IN (" & Agro_SQL_Save_Clausola_IN(Left(FiltroCentri, FiltroCentri.Length - 1), False) & ") ")
                            End If
                        End If
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   Centri_Aziendali.Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Centri_Aziendali.Inviato =-1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY Sa_Nome ASC")
                    End If

                    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta
                    StrSQL.Length = 0
                    StrSQL.AppendLine(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;")
                    StrSQL.Append(" SELECT Centri_Aziendali.* ")
                    StrSQL.Append(" FROM  Centri_Aziendali ")
                    StrSQL.Append(" WHERE Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND   Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    If Piva <> "" Then

                        StrSQL.Append(" AND  Piva = '" & Agro_SQL_SaveText(Piva) & "'")
                    End If
                    If Sa_Cod <> 0 Then
                        StrSQL.Append(" AND  Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
                    Else
                        'leggo se ci sono filtri sui centri
                        Dim objUtentiVisibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
                        Dim FiltroCentri As String = ""
                        Dim DtCentriVisibili As DataTable
                        DtCentriVisibili = objUtentiVisibilita.Leggi(enum_TipoEntita.Centro, " Piva='" & Agro_SQL_SaveText(Piva) & "'", "", objParametri)
                        If Not DtCentriVisibili Is Nothing AndAlso DtCentriVisibili.Rows.Count > 0 Then
                            For i = 0 To DtCentriVisibili.Rows.Count - 1
                                FiltroCentri &= DtCentriVisibili.Rows(i).Item("sa_cod") & ","
                            Next
                            If FiltroCentri <> "" Then
                                StrSQL.Append(" AND sa_cod IN (" & Agro_SQL_Save_Clausola_IN(Left(FiltroCentri, FiltroCentri.Length - 1), False) & ") ")
                            End If
                        End If
                    End If



                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   Centri_Aziendali.Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Centri_Aziendali.Inviato =-1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY Sa_Nome ASC")
                    End If

                    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni
                    StrSQL.Length = 0
                    StrSQL.AppendLine(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;")
                    StrSQL.AppendLine(" SELECT (Centri_Aziendali.piva + '_' +  cast(Centri_Aziendali.sa_cod as nvarchar(10))) as chiave, ")
                    StrSQL.AppendLine("     Case WHEN ISNULL(Imprese.partitaIvaReale, '') = '' THEN Centri_Aziendali.PIVA ELSE Imprese.partitaIvaReale END AS PivaReale,")
                    StrSQL.AppendLine("     Centri_Aziendali.*, Imprese.Rag_Soc, CentrixIndirizzi.Tipo_Indirizzo, ")
                    StrSQL.AppendLine("     Indirizzi.ind_des, Indirizzi.frz_des, Indirizzi.CAP, Indirizzi.com_cod_istat, Indirizzi.pro_cod_istat, Indirizzi.stato, Indirizzi.note, ")
                    StrSQL.AppendLine("     ISNULL(ISTAT.LOCALITA, '') AS com_des, ISNULL(ISTAT.COMUNI_PROV, '') AS pro_cod,  ")
                    StrSQL.AppendLine("     ISNULL  ((  SELECT    TOP 1 val_cod  ")
                    StrSQL.AppendLine("         FROM    Centri_Aziendali_Codici   ")
                    StrSQL.AppendLine("         WHERE   Centri_Aziendali_Codici.piva = Centri_Aziendali.piva")
                    StrSQL.AppendLine("         AND Centri_Aziendali_Codici.SA_COD = Centri_Aziendali.sa_cod")
                    StrSQL.AppendLine("         AND Centri_Aziendali_Codici.id_cod = " + CStr(enum_CodiciAnagrafe.TitoloPossesso) + "), '1') AS Titolo_Possesso, ")
                    StrSQL.AppendLine("     (select [User] from utenti where CODICE_FISCALE = imprese.Username_Modifica ) as utente_modifica ")
                    StrSQL.AppendLine(" FROM  Centri_Aziendali ")
                    StrSQL.AppendLine(" INNER JOIN Imprese  ON Imprese.PIVA = Centri_Aziendali.PIVA ")
                    StrSQL.AppendLine(" INNER JOIN CentrixIndirizzi  ON CentrixIndirizzi.PIVA = Centri_Aziendali.PIVA AND CentrixIndirizzi.sa_cod = Centri_Aziendali.sa_cod ")
                    StrSQL.AppendLine(" INNER JOIN Indirizzi ON Indirizzi.cod_indirizzo = CentrixIndirizzi.cod_indirizzo ")
                    StrSQL.AppendLine(" LEFT OUTER JOIN ISTAT ON Indirizzi.pro_cod_istat = ISTAT.PROV AND Indirizzi.com_cod_istat = ISTAT.COM ")

                    StrSQL.AppendLine(" WHERE Centri_Aziendali.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.AppendLine(" AND   Centri_Aziendali.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    If Piva <> "" Then

                        StrSQL.Append(" AND  Centri_Aziendali.Piva = '" & Agro_SQL_SaveText(Piva) & "'")
                    End If

                    If Sa_Cod <> 0 Then
                        StrSQL.Append(" AND  Centri_Aziendali.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
                    Else
                        'leggo se ci sono filtri sui centri
                        Dim objUtentiVisibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
                        Dim FiltroCentri As String = ""
                        Dim DtCentriVisibili As DataTable
                        DtCentriVisibili = objUtentiVisibilita.Leggi(enum_TipoEntita.Centro, " Piva='" & Agro_SQL_SaveText(Piva) & "'", "", objParametri)
                        If Not DtCentriVisibili Is Nothing AndAlso DtCentriVisibili.Rows.Count > 0 Then
                            For i = 0 To DtCentriVisibili.Rows.Count - 1
                                FiltroCentri &= DtCentriVisibili.Rows(i).Item("sa_cod") & ","
                            Next
                            If FiltroCentri <> "" Then
                                StrSQL.Append(" AND Centri_Aziendali.sa_cod IN (" & Agro_SQL_Save_Clausola_IN(Left(FiltroCentri, FiltroCentri.Length - 1), False) & ") ")
                            End If
                        End If
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   Centri_Aziendali.Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Centri_Aziendali.Inviato =-1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY Rag_Soc, Centri_Aziendali.Sa_Nome")
                    End If


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


    Public Function Leggi_Filtro_Data(ByVal Piva As String,
                                      ByVal Sa_Cod As Integer,
                                      ByVal Data As DateTime,
                                      ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile,
                                      ByVal xFiltroAggiuntivo As String,
                                      ByVal xOrderBy As String,
                                      ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                      ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.CentriAziendali_Read.Leggi_Filtro_Data()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim i As Integer

        Try
            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi
                    StrSQL.Length = 0
                    StrSQL.AppendLine(" SELECT Centri_Aziendali.PIVA , Centri_Aziendali.Sa_Cod, Centri_Aziendali.Sa_Nome, Centri_Aziendali.Validita_Inizio , Centri_Aziendali.Validita_Fine, Centri_Aziendali.inviato,")

                    StrSQL.AppendLine("ISNULL(ISTAT.LOCALITA, '') AS com_des, ")
                    StrSQL.AppendLine("ISNULL(Lista_Province.Provincia, '') AS pro_cod,")
                    StrSQL.AppendLine("ind.pro_cod_istat,")
                    StrSQL.AppendLine("ind.com_cod_istat,")
                    StrSQL.AppendLine("ind.stato")

                    StrSQL.AppendLine(" FROM  Centri_Aziendali ")

                    StrSQL.AppendLine("INNER JOIN CentrixIndirizzi  ON CentrixIndirizzi.PIVA = Centri_Aziendali.PIVA")
                    StrSQL.AppendLine("AND CentrixIndirizzi.sa_cod = Centri_Aziendali.sa_cod")
                    StrSQL.AppendLine("AND CentrixIndirizzi.Tipo_Indirizzo = 1")
                    StrSQL.AppendLine("INNER JOIN Indirizzi AS ind ON ind.cod_indirizzo = CentrixIndirizzi.cod_indirizzo")
                    StrSQL.AppendLine("LEFT JOIN ISTAT ON ind.pro_cod_istat = ISTAT.PROV")
                    StrSQL.AppendLine("AND ind.com_cod_istat = ISTAT.COM")
                    StrSQL.AppendLine("LEFT JOIN Lista_Province ON ind.pro_cod_istat = Lista_Province.PROV")

                    StrSQL.AppendLine(" WHERE (( Centri_Aziendali.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.AppendLine(" AND   Centri_Aziendali.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & ") ")
                    If Data <> AGRODATAINIZIO Then
                        StrSQL.AppendLine(" AND ( Centri_Aziendali.Validita_Inizio <= " & Agro_SQL_SaveDate(Data) & " AND   Centri_Aziendali.Validita_Fine >= " & Agro_SQL_SaveDate(Data) & ") ")
                    End If
                    StrSQL.AppendLine(" ) ")

                    If Piva <> "" Then
                        StrSQL.AppendLine(" AND  Centri_Aziendali.Piva = '" & Agro_SQL_SaveText(Piva) & "'")
                    End If

                    If Sa_Cod <> 0 Then
                        StrSQL.AppendLine(" AND  Centri_Aziendali.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
                    Else
                        'leggo se ci sono filtri sui centri
                        Dim objUtentiVisibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
                        Dim FiltroCentri As String = ""
                        Dim DtCentriVisibili As DataTable
                        DtCentriVisibili = objUtentiVisibilita.Leggi(enum_TipoEntita.Centro, " Piva='" & Agro_SQL_SaveText(Piva) & "'", "", objParametri)
                        If Not DtCentriVisibili Is Nothing AndAlso DtCentriVisibili.Rows.Count > 0 Then
                            For i = 0 To DtCentriVisibili.Rows.Count - 1
                                FiltroCentri &= DtCentriVisibili.Rows(i).Item("sa_cod") & ","
                            Next
                            If FiltroCentri <> "" Then
                                StrSQL.AppendLine(" AND Centri_Aziendali.sa_cod IN (" & Agro_SQL_Save_Clausola_IN(Left(FiltroCentri, FiltroCentri.Length - 1), False) & ") ")
                            End If
                        End If
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.AppendLine(" AND   Centri_Aziendali.Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.AppendLine(" AND   Centri_Aziendali.Inviato =-1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.AppendLine(" ORDER BY Sa_Nome ASC")
                    End If

                    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta
                    '
                    '
                    '
                    '

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


    Public Function Leggi_x_anagrafica(
                         ByVal Piva As String,
                         ByVal Sa_Cod As Int32,
                             ByVal xFiltroAggiuntivo As String,
                             ByVal xOrderBy As String,
                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                             ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.CentriAziendali_Read.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""                   =>  si leggono tutte le imprese
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim i As Integer

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT top 100 (ca.piva + '_' + cast(ca.sa_cod as nvarchar(10))) as chiave, ca.sa_cod, ca.sa_nome, (ca.piva + '_' + cast(ca.sa_cod as nvarchar(10))) AS CodiceGIAS, ")
            StrSQL.AppendLine(" i.Piva, i.Rag_Soc, ISNULL(ic.val_cod, ' ') AS Codice_Cuaa, ")
            StrSQL.AppendLine(" ca.Validita_Inizio, ca.Validita_Fine, ")
            StrSQL.AppendLine(" ind.cod_indirizzo, ind.ind_des, ind.frz_des, ind.CAP, ind.stato, ind.note, ISNULL(ISTAT.LOCALITA, '') AS com_des, ISNULL(ISTAT.COMUNI_PROV, '') AS pro_cod,  ")
            StrSQL.AppendLine(" ind.ind_des + ' ' + ind.frz_des + ' ' + ind.Cap + ' ' + ISNULL(ISTAT.LOCALITA, '') + ' ' + ISNULL(ISTAT.COMUNI_PROV, '') + ' ' + Stato AS Indirizzo, ")
            StrSQL.AppendLine(" ind.pro_cod_istat, ind.com_cod_istat, ind.stato, ind.note, ")
            StrSQL.AppendLine(" ISNULL(cacTipoCentro.val_cod, ' ') AS TipoCentro, ")
            StrSQL.AppendLine(" ISNULL ((SELECT TOP 1 val_cod FROM Centri_Aziendali_Codici WHERE Centri_Aziendali_Codici.id_cod = " & enum_CodiciAnagrafe.CodiceCentro_Attuale & " AND Centri_Aziendali_Codici.piva=ca.piva AND Centri_Aziendali_Codici.sa_cod=ca.sa_cod), ' ') AS CodiceOperatoreBio, ")
            StrSQL.AppendLine(" ISNULL ((SELECT TOP 1 val_cod FROM Centri_Aziendali_Codici WHERE Centri_Aziendali_Codici.id_cod = " & enum_CodiciAnagrafe.TipoAttivita & " AND Centri_Aziendali_Codici.piva=ca.piva AND Centri_Aziendali_Codici.sa_cod=ca.sa_cod), ' ') AS tipoAttivitaCod, ")

            StrSQL.AppendLine(" ISNULL ((SELECT TOP 1 [User] FROM Utenti WHERE CODICE_FISCALE = ca.Username_Creazione), ' ') AS Utente_Creazione, ")
            StrSQL.AppendLine(" ISNULL ((SELECT TOP 1 [User] FROM Utenti WHERE CODICE_FISCALE = ca.Username_Modifica), ' ') AS Utente_Modifica, ")

            StrSQL.AppendLine(" ca.Data_Creazione, ")
            StrSQL.AppendLine(" ca.Data_Modifica ")


            StrSQL.AppendLine(" FROM Centri_Aziendali ca ")
            StrSQL.AppendLine(" INNER JOIN Imprese AS i ON i.PIVA = ca.PIVA ")
            StrSQL.AppendLine(" LEFT JOIN Imprese_Codici ic ON i.PIVA = ic.PIVA AND ic.id_cod = " & enum_CodiciAnagrafe.CodiceCUAA)

            StrSQL.AppendLine(" LEFT JOIN Centri_Aziendali_Codici cacTipoCentro ON ca.PIVA = cacTipoCentro.PIVA AND ca.sa_cod = cacTipoCentro.sa_cod AND cacTipoCentro.ID_Cod IN (101, 102, 103) ")
            StrSQL.AppendLine(" LEFT JOIN Codici_Anagrafe caTipoCentro ON caTipoCentro.creatore='CSA' AND gruppo='TIPO_CA' AND caTipoCentro.codice=cacTipoCentro.id_cod ")

            StrSQL.AppendLine(" INNER JOIN CentrixIndirizzi  ON CentrixIndirizzi.PIVA = ca.PIVA AND CentrixIndirizzi.sa_cod = ca.sa_cod AND CentrixIndirizzi.Tipo_Indirizzo = 1 ")
            StrSQL.AppendLine(" INNER JOIN Indirizzi AS ind ON ind.cod_indirizzo = CentrixIndirizzi.cod_indirizzo ")
            StrSQL.AppendLine(" LEFT JOIN ISTAT ON ind.pro_cod_istat = ISTAT.PROV AND ind.com_cod_istat = ISTAT.COM ")

            StrSQL.AppendLine(" WHERE 1 = 1 ")

            StrSQL.AppendLine(" AND   ca.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.AppendLine(" AND   ca.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            If Piva <> "" Then
                StrSQL.AppendLine(" AND  ca.Piva = '" & Agro_SQL_SaveText(Piva) & "'")
            End If

            If Sa_Cod <> 0 Then
                StrSQL.AppendLine(" AND  ca.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            Else
                'leggo se ci sono filtri sui centri
                Dim objUtentiVisibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
                Dim FiltroCentri As String = ""
                Dim DtCentriVisibili As DataTable = objUtentiVisibilita.Leggi(enum_TipoEntita.Centro, " Piva='" & Agro_SQL_SaveText(Piva) & "'", "", objParametri)
                If Not DtCentriVisibili Is Nothing AndAlso DtCentriVisibili.Rows.Count > 0 Then
                    For i = 0 To DtCentriVisibili.Rows.Count - 1
                        FiltroCentri &= DtCentriVisibili.Rows(i).Item("sa_cod") & ","
                    Next
                    If FiltroCentri <> "" Then
                        StrSQL.AppendLine(" AND ca.sa_cod IN (" & Agro_SQL_Save_Clausola_IN(Left(FiltroCentri, FiltroCentri.Length - 1), False) & ") ")
                    End If
                End If
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.AppendLine(" AND   ca.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.AppendLine(" AND   ca.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine(" ORDER BY Rag_Soc, ca.Sa_Nome")
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

    Public Function Leggi_x_anagraficaZoo(
                         ByVal Piva As String,
                         ByVal Sa_Cod As Int32,
                             ByVal xFiltroAggiuntivo As String,
                             ByVal xOrderBy As String,
                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                             ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.CentriAziendali_Read.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""                   =>  si leggono tutte le imprese
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim i As Integer

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT DISTINCT (ca.piva + '_' + cast(ca.sa_cod as nvarchar(10))) as chiave, ca.sa_cod, ca.sa_nome, (ca.piva + '_' + cast(ca.sa_cod as nvarchar(10))) AS CodiceGIAS, ")
            StrSQL.AppendLine(" i.Piva, i.Rag_Soc, ISNULL(ic.val_cod, ' ') AS Codice_Cuaa, ")
            StrSQL.AppendLine(" ca.Validita_Inizio, ca.Validita_Fine, ")
            StrSQL.AppendLine(" ind.cod_indirizzo, ind.ind_des, ind.frz_des, ind.CAP, ind.stato, ind.note, ISNULL(ISTAT.LOCALITA, '') AS com_des, ISNULL(ISTAT.COMUNI_PROV, '') AS pro_cod,  ")
            StrSQL.AppendLine(" ind.ind_des + ' ' + ind.frz_des + ' ' + ind.Cap + ' ' + ISNULL(ISTAT.LOCALITA, '') + ' ' + ISNULL(ISTAT.COMUNI_PROV, '') + ' ' + Stato AS Indirizzo, ")
            StrSQL.AppendLine(" ind.pro_cod_istat, ind.com_cod_istat, ind.stato, ind.note, ")
            StrSQL.AppendLine(" ISNULL(cacTipoCentro.val_cod, ' ') AS TipoCentro, ")
            StrSQL.AppendLine(" ISNULL ((SELECT TOP 1 val_cod FROM Centri_Aziendali_Codici WHERE Centri_Aziendali_Codici.id_cod = " & enum_CodiciAnagrafe.CodiceCentro_Attuale & " AND Centri_Aziendali_Codici.piva=ca.piva AND Centri_Aziendali_Codici.sa_cod=ca.sa_cod), ' ') AS CodiceOperatoreBio, ")
            StrSQL.AppendLine(" ISNULL ((SELECT TOP 1 val_cod FROM Centri_Aziendali_Codici WHERE Centri_Aziendali_Codici.id_cod = " & enum_CodiciAnagrafe.TipoAttivita & " AND Centri_Aziendali_Codici.piva=ca.piva AND Centri_Aziendali_Codici.sa_cod=ca.sa_cod), ' ') AS tipoAttivitaCod, ")

            StrSQL.AppendLine(" ISNULL ((SELECT TOP 1 [User] FROM Utenti WHERE CODICE_FISCALE = ca.Username_Creazione), ' ') AS Utente_Creazione, ")
            StrSQL.AppendLine(" ISNULL ((SELECT TOP 1 [User] FROM Utenti WHERE CODICE_FISCALE = ca.Username_Modifica), ' ') AS Utente_Modifica, ")

            StrSQL.AppendLine(" ca.Data_Creazione, ")
            StrSQL.AppendLine(" ca.Data_Modifica ")


            StrSQL.AppendLine(" FROM Centri_Aziendali ca ")
            StrSQL.AppendLine(" JOIN Stalla ON ca.piva = Stalla.Piva AND Stalla.Sa_Cod = ca.Sa_Cod ")
            StrSQL.AppendLine(" INNER JOIN Imprese AS i ON i.PIVA = ca.PIVA ")
            StrSQL.AppendLine(" LEFT JOIN Imprese_Codici ic ON i.PIVA = ic.PIVA AND ic.id_cod = " & enum_CodiciAnagrafe.CodiceCUAA)

            StrSQL.AppendLine(" LEFT JOIN Centri_Aziendali_Codici cacTipoCentro ON ca.PIVA = cacTipoCentro.PIVA AND ca.sa_cod = cacTipoCentro.sa_cod AND cacTipoCentro.ID_Cod IN (101, 102, 103) ")
            StrSQL.AppendLine(" LEFT JOIN Codici_Anagrafe caTipoCentro ON caTipoCentro.creatore='CSA' AND gruppo='TIPO_CA' AND caTipoCentro.codice=cacTipoCentro.id_cod ")

            StrSQL.AppendLine(" INNER JOIN CentrixIndirizzi  ON CentrixIndirizzi.PIVA = ca.PIVA AND CentrixIndirizzi.sa_cod = ca.sa_cod AND CentrixIndirizzi.Tipo_Indirizzo = 1 ")
            StrSQL.AppendLine(" INNER JOIN Indirizzi AS ind ON ind.cod_indirizzo = CentrixIndirizzi.cod_indirizzo ")
            StrSQL.AppendLine(" LEFT JOIN ISTAT ON ind.pro_cod_istat = ISTAT.PROV AND ind.com_cod_istat = ISTAT.COM ")

            StrSQL.AppendLine(" WHERE 1 = 1 ")

            StrSQL.AppendLine(" AND   ca.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.AppendLine(" AND   ca.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            If Piva <> "" Then
                StrSQL.AppendLine(" AND  ca.Piva = '" & Agro_SQL_SaveText(Piva) & "'")
            End If

            If Sa_Cod <> 0 Then
                StrSQL.AppendLine(" AND  ca.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            Else
                'leggo se ci sono filtri sui centri
                Dim objUtentiVisibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
                Dim FiltroCentri As String = ""
                Dim DtCentriVisibili As DataTable = objUtentiVisibilita.Leggi(enum_TipoEntita.Centro, " Piva='" & Agro_SQL_SaveText(Piva) & "'", "", objParametri)
                If Not DtCentriVisibili Is Nothing AndAlso DtCentriVisibili.Rows.Count > 0 Then
                    For i = 0 To DtCentriVisibili.Rows.Count - 1
                        FiltroCentri &= DtCentriVisibili.Rows(i).Item("sa_cod") & ","
                    Next
                    If FiltroCentri <> "" Then
                        StrSQL.AppendLine(" AND ca.sa_cod IN (" & Agro_SQL_Save_Clausola_IN(Left(FiltroCentri, FiltroCentri.Length - 1), False) & ") ")
                    End If
                End If
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.AppendLine(" AND   ca.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.AppendLine(" AND   ca.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine(" ORDER BY Rag_Soc, ca.Sa_Nome")
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


    Public Function Leggi_x_anagraficaNG(
                         ByVal Piva As String,
                         ByVal Sa_Cod As Int32,
                         ByVal xFiltroAggiuntivo As String,
                         ByVal xOrderBy As String,
                         ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                         ByRef leggiSuperfici As Boolean
                             ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.CentriAziendali_Read.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""                   =>  si leggono tutte le imprese
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim i As Integer

        Try

            StrSQL.Length = 0
            If (leggiSuperfici) Then
                StrSQL.AppendLine(" WITH sup_catastali as ( ")
                StrSQL.AppendLine("  	SELECT  Piva, Sa_Cod, SUM(Sup_Condotta) as Sup_Catastale  ")
                StrSQL.AppendLine("  	FROM    ImpreseXParticelle (NOLOCK) ")
                StrSQL.AppendLine("      WHERE ImpreseXParticelle.Validita_inizio <=  " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & "   ")
                StrSQL.AppendLine("      AND   ImpreseXParticelle.Validita_Fine >=  " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & "   ")
                If Piva <> "" Then
                    StrSQL.AppendLine("      AND   ImpreseXParticelle.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
                End If
                StrSQL.AppendLine("  	GROUP BY Piva, Sa_Cod  ")
                StrSQL.AppendLine("  ), sup_appezzamenti as (  ")
                StrSQL.AppendLine("  	SELECT  Appezzamento.PIVA,  ")
                StrSQL.AppendLine("  			Appezzamento.SA_COD,   ")
                StrSQL.AppendLine("  			SUM (CASE WHEN Appezzamento_Codici.val_cod = '1' OR Appezzamento_Codici.val_cod IS NULL THEN Appezzamento.sup_app ELSE 0 END) as SAU_Convenzionale,  ")
                StrSQL.AppendLine("  			SUM (CASE WHEN Appezzamento_Codici.val_cod = '2' THEN Appezzamento.sup_app ELSE 0 END) as SAU_Conversione,  ")
                StrSQL.AppendLine("  			SUM (CASE WHEN Appezzamento_Codici.val_cod = '3' THEN Appezzamento.sup_app ELSE 0 END) as SAU_Biologico,  ")
                StrSQL.AppendLine("  			SUM (Appezzamento.sup_app) AS SAU_Totale  ")
                StrSQL.AppendLine("  	FROM    Appezzamento (NOLOCK)  ")
                StrSQL.AppendLine("  	LEFT OUTER JOIN Appezzamento_Codici (NOLOCK) ON Appezzamento.PIVA = Appezzamento_Codici.PIVA    ")
                StrSQL.AppendLine("  										AND Appezzamento.SA_COD = Appezzamento_Codici.sa_cod  ")
                StrSQL.AppendLine("  										AND Appezzamento.APPEZZA = Appezzamento_Codici.appezza  ")
                StrSQL.AppendLine("  										AND Appezzamento_Codici.id_cod = 1018  ")
                StrSQL.AppendLine("      WHERE Appezzamento.Validita_inizio <=  " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & "   ")
                StrSQL.AppendLine("      AND   Appezzamento.Validita_Fine >=  " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & "   ")
                If Piva <> "" Then
                    StrSQL.AppendLine("      AND   Appezzamento.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
                End If
                StrSQL.AppendLine("  	GROUP BY Appezzamento.PIVA, Appezzamento.SA_COD  ")
                StrSQL.AppendLine("  )  ")
            End If

            StrSQL.AppendLine(" SELECT (ca.piva + '_' + cast(ca.sa_cod as nvarchar(10))) as chiave, ca.sa_cod, ca.sa_nome, (ca.piva + '_' + cast(ca.sa_cod as nvarchar(10))) AS CodiceGIAS, ")
            StrSQL.AppendLine(" i.Piva, i.Rag_Soc, ISNULL(ic.val_cod, ' ') AS Codice_Cuaa, ")
            StrSQL.AppendLine(" ca.Validita_Inizio, ca.Validita_Fine, ")
            StrSQL.AppendLine(" ind.cod_indirizzo, ind.ind_des, ind.frz_des, ind.CAP, ind.stato, ind.note, ISNULL(ISTAT.LOCALITA, '') AS com_des, ISNULL(Lista_Province.Provincia, '') AS pro_cod,  ")
            StrSQL.AppendLine(" ind.ind_des + ' ' + ind.frz_des + ' ' + ind.Cap + ' ' + ISNULL(ISTAT.LOCALITA, '') + ' ' + ISNULL(ISTAT.COMUNI_PROV, '') + ' ' + Stato AS Indirizzo, ")
            StrSQL.AppendLine(" ind.pro_cod_istat, ind.com_cod_istat, ind.stato, ind.note, ")
            StrSQL.AppendLine(" CASE Stato WHEN '' THEN 'IT' WHEN 'ITALIA' THEN 'IT' ELSE stato END as Stato_Cod, ")
            StrSQL.AppendLine(" ISNULL(ACCDAA_ANAG_T005_TabellaCodiciPaesiTerziISO3166.Descrizione, 'Italia') as Stato, ")
            StrSQL.AppendLine(" ISNULL(cacTipoCentro.val_cod, ' ') AS TipoCentro, ")
            StrSQL.AppendLine(" ISNULL ((SELECT TOP 1 val_cod FROM Centri_Aziendali_Codici WHERE Centri_Aziendali_Codici.id_cod = " & enum_CodiciAnagrafe.CodiceCentro_Attuale & " AND Centri_Aziendali_Codici.piva=ca.piva AND Centri_Aziendali_Codici.sa_cod=ca.sa_cod), ' ') AS CodiceOperatoreBio, ")
            StrSQL.AppendLine(" ISNULL ((SELECT TOP 1 val_cod FROM Centri_Aziendali_Codici WHERE Centri_Aziendali_Codici.id_cod = " & enum_CodiciAnagrafe.TipoAttivita & " AND Centri_Aziendali_Codici.piva=ca.piva AND Centri_Aziendali_Codici.sa_cod=ca.sa_cod), ' ') AS tipoAttivitaCod, ")

            StrSQL.AppendLine(" ISNULL ((SELECT TOP 1 [User] FROM Utenti WHERE CODICE_FISCALE = ca.Username_Creazione), ' ') AS Utente_Creazione, ")
            StrSQL.AppendLine(" ISNULL ((SELECT TOP 1 [User] FROM Utenti WHERE CODICE_FISCALE = ca.Username_Modifica), ' ') AS Utente_Modifica, ")

            StrSQL.AppendLine(" ca.Data_Creazione, ")
            StrSQL.AppendLine(" ca.Data_Modifica ")

            If leggiSuperfici Then
                StrSQL.AppendLine("  ,CAST(COALESCE(sup_catastali.Sup_Catastale, 0) as float) AS Superficie_Catastale ")
                StrSQL.AppendLine("  ,CAST(COALESCE(sup_appezzamenti.SAU_Convenzionale, 0) as float) AS Superficie_Convenzionale ")
                StrSQL.AppendLine("  ,CAST(COALESCE(sup_appezzamenti.SAU_Conversione, 0) as float) AS Superficie_Conversione ")
                StrSQL.AppendLine("  ,CAST(COALESCE(sup_appezzamenti.SAU_Biologico, 0) as float) AS Superficie_Biologico ")
                StrSQL.AppendLine("  ,CAST(COALESCE(sup_appezzamenti.SAU_Totale, 0) as float) AS Superficie_Totale ")
            End If

            StrSQL.AppendLine(" FROM Centri_Aziendali ca ")
            StrSQL.AppendLine(" INNER JOIN Imprese AS i ON i.PIVA = ca.PIVA ")
            StrSQL.AppendLine(" LEFT JOIN Imprese_Codici ic ON i.PIVA = ic.PIVA AND ic.id_cod = " & enum_CodiciAnagrafe.CodiceCUAA)
            If (leggiSuperfici) Then
                StrSQL.AppendLine(" LEFT JOIN sup_catastali ON ca.Piva = sup_catastali.Piva AND ca.Sa_Cod = sup_catastali.Sa_Cod ")
                StrSQL.AppendLine(" LEFT JOIN sup_appezzamenti ON ca.Piva = sup_appezzamenti.Piva AND ca.Sa_Cod = sup_appezzamenti.Sa_Cod ")
            End If
            StrSQL.AppendLine(" LEFT JOIN Centri_Aziendali_Codici cacTipoCentro ON ca.PIVA = cacTipoCentro.PIVA AND ca.sa_cod = cacTipoCentro.sa_cod AND cacTipoCentro.ID_Cod IN (101, 102, 103) ")
            StrSQL.AppendLine(" LEFT JOIN Codici_Anagrafe caTipoCentro ON caTipoCentro.creatore='CSA' AND gruppo='TIPO_CA' AND caTipoCentro.codice=cacTipoCentro.id_cod ")

            StrSQL.AppendLine(" INNER JOIN CentrixIndirizzi  ON CentrixIndirizzi.PIVA = ca.PIVA AND CentrixIndirizzi.sa_cod = ca.sa_cod AND CentrixIndirizzi.Tipo_Indirizzo = 1 ")
            StrSQL.AppendLine(" INNER JOIN Indirizzi AS ind ON ind.cod_indirizzo = CentrixIndirizzi.cod_indirizzo ")
            StrSQL.AppendLine(" LEFT JOIN ISTAT ON ind.pro_cod_istat = ISTAT.PROV AND ind.com_cod_istat = ISTAT.COM ")
            StrSQL.AppendLine(" LEFT JOIN Lista_Province ON ind.pro_cod_istat = Lista_Province.PROV ")
            StrSQL.AppendLine(" LEFT JOIN ACCDAA_ANAG_T005_TabellaCodiciPaesiTerziISO3166 ON ACCDAA_ANAG_T005_TabellaCodiciPaesiTerziISO3166.Codice = ind.Stato ")

            StrSQL.AppendLine(" WHERE 1 = 1 ")

            StrSQL.AppendLine(" AND   ca.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.AppendLine(" AND   ca.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            If Piva <> "" Then
                StrSQL.AppendLine(" AND  ca.Piva = '" & Agro_SQL_SaveText(Piva) & "'")
            End If

            If Sa_Cod <> 0 Then
                StrSQL.AppendLine(" AND  ca.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            Else
                'leggo se ci sono filtri sui centri
                Dim objUtentiVisibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
                Dim FiltroCentri As String = ""
                Dim DtCentriVisibili As DataTable = objUtentiVisibilita.Leggi(enum_TipoEntita.Centro, " Piva='" & Agro_SQL_SaveText(Piva) & "'", "", objParametri)
                If Not DtCentriVisibili Is Nothing AndAlso DtCentriVisibili.Rows.Count > 0 Then
                    For i = 0 To DtCentriVisibili.Rows.Count - 1
                        FiltroCentri &= DtCentriVisibili.Rows(i).Item("sa_cod") & ","
                    Next
                    If FiltroCentri <> "" Then
                        StrSQL.AppendLine(" AND ca.sa_cod IN (" & Agro_SQL_Save_Clausola_IN(Left(FiltroCentri, FiltroCentri.Length - 1), False) & ") ")
                    End If
                End If
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.AppendLine(" AND   ca.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.AppendLine(" AND   ca.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine(" ORDER BY Rag_Soc, ca.Sa_Nome")
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

    Public Function Leggi_x_anagraficaNG_New(
                         ByVal Sa_Cod As Int32,
                         ByVal xFiltroAggiuntivo As String,
                         ByVal xOrderBy As String,
                         ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                         ByRef leggiSuperfici As Boolean
                             ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.CentriAziendali_Read.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""                   =>  si leggono tutte le imprese
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim i As Integer

        Try

            Dim filtroAggiuntivo = xFiltroAggiuntivo.Replace(
                        "Reg_Impianti.PIVA", "i.PIVA").Replace(
                            "Reg_Impianti.SA_COD", "ca.SA_COD")

            filtroAggiuntivo = Regex.Replace(filtroAggiuntivo, "AND Reg_Impianti\.APPEZZA = \d+ AND Reg_Impianti\.ID_REG = \d+", "", RegexOptions.None, TimeSpan.FromSeconds(3))

            Dim filtroImpresexParticelle = xFiltroAggiuntivo.Replace(
                        "Reg_Impianti.PIVA", "ImpreseXParticelle.PIVA")

            filtroImpresexParticelle = Regex.Replace(filtroImpresexParticelle, "AND\s+Reg_Impianti\.SA_COD\s+=\s+\d+\s+AND\s+Reg_Impianti\.APPEZZA\s+=\s+\d+\s+AND\s+Reg_Impianti\.ID_REG\s+=\s+\d+", "", RegexOptions.None, TimeSpan.FromSeconds(3))
            Dim filtroAppezzamenti = filtroImpresexParticelle.Replace("ImpreseXParticelle", "Appezzamento")
            Dim filtroCentri = filtroImpresexParticelle.Replace("ImpreseXParticelle", "ca")
            Dim filtroCentriVisibili = filtroImpresexParticelle.Replace("ImpreseXParticelle.", "").Replace("AND", "")

            StrSQL.Length = 0
            If (leggiSuperfici) Then
                StrSQL.AppendLine(" WITH sup_catastali as ( ")
                StrSQL.AppendLine("  	SELECT  Piva, Sa_Cod, SUM(Sup_Condotta) as Sup_Catastale  ")
                StrSQL.AppendLine("  	FROM    ImpreseXParticelle (NOLOCK) ")
                StrSQL.AppendLine("      WHERE ImpreseXParticelle.Validita_inizio <=  " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & "   ")
                StrSQL.AppendLine("      AND   ImpreseXParticelle.Validita_Fine >=  " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & "   ")
                If filtroImpresexParticelle <> "" Then
                    StrSQL.AppendLine(Agro_SQL_SaveText(filtroImpresexParticelle) & "'   ")
                End If
                StrSQL.AppendLine("  	GROUP BY Piva, Sa_Cod  ")
                StrSQL.AppendLine("  ), sup_appezzamenti as (  ")
                StrSQL.AppendLine("  	SELECT  Appezzamento.PIVA,  ")
                StrSQL.AppendLine("  			Appezzamento.SA_COD,   ")
                StrSQL.AppendLine("  			SUM (CASE WHEN Appezzamento_Codici.val_cod = '1' OR Appezzamento_Codici.val_cod IS NULL THEN Appezzamento.sup_app ELSE 0 END) as SAU_Convenzionale,  ")
                StrSQL.AppendLine("  			SUM (CASE WHEN Appezzamento_Codici.val_cod = '2' THEN Appezzamento.sup_app ELSE 0 END) as SAU_Conversione,  ")
                StrSQL.AppendLine("  			SUM (CASE WHEN Appezzamento_Codici.val_cod = '3' THEN Appezzamento.sup_app ELSE 0 END) as SAU_Biologico,  ")
                StrSQL.AppendLine("  			SUM (Appezzamento.sup_app) AS SAU_Totale  ")
                StrSQL.AppendLine("  	FROM    Appezzamento (NOLOCK)  ")
                StrSQL.AppendLine("  	LEFT OUTER JOIN Appezzamento_Codici (NOLOCK) ON Appezzamento.PIVA = Appezzamento_Codici.PIVA    ")
                StrSQL.AppendLine("  										AND Appezzamento.SA_COD = Appezzamento_Codici.sa_cod  ")
                StrSQL.AppendLine("  										AND Appezzamento.APPEZZA = Appezzamento_Codici.appezza  ")
                StrSQL.AppendLine("  										AND Appezzamento_Codici.id_cod = 1018  ")
                StrSQL.AppendLine("      WHERE Appezzamento.Validita_inizio <=  " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & "   ")
                StrSQL.AppendLine("      AND   Appezzamento.Validita_Fine >=  " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & "   ")
                If filtroAppezzamenti <> "" Then
                    StrSQL.AppendLine(Agro_SQL_SaveText(filtroAppezzamenti) & "'   ")
                End If
                StrSQL.AppendLine("  	GROUP BY Appezzamento.PIVA, Appezzamento.SA_COD  ")
                StrSQL.AppendLine("  )  ")
            End If

            StrSQL.AppendLine(" SELECT (ca.piva + '_' + cast(ca.sa_cod as nvarchar(10))) as chiave, ca.sa_cod, ca.sa_nome, (ca.piva + '_' + cast(ca.sa_cod as nvarchar(10))) AS CodiceGIAS, ")
            StrSQL.AppendLine(" i.Piva, i.Rag_Soc, ISNULL(ic.val_cod, ' ') AS Codice_Cuaa, ")
            StrSQL.AppendLine(" ca.Validita_Inizio, ca.Validita_Fine, ")
            StrSQL.AppendLine(" ind.cod_indirizzo, ind.ind_des, ind.frz_des, ind.CAP, ind.stato, ind.note, ISNULL(ISTAT.LOCALITA, '') AS com_des, ISNULL(Lista_Province.Provincia, '') AS pro_cod,  ")
            StrSQL.AppendLine(" ind.ind_des + ' ' + ind.frz_des + ' ' + ind.Cap + ' ' + ISNULL(ISTAT.LOCALITA, '') + ' ' + ISNULL(ISTAT.COMUNI_PROV, '') + ' ' + Stato AS Indirizzo, ")
            StrSQL.AppendLine(" ind.pro_cod_istat, ind.com_cod_istat, ind.stato, ind.note, ")
            StrSQL.AppendLine(" CASE Stato WHEN '' THEN 'IT' WHEN 'ITALIA' THEN 'IT' ELSE stato END as Stato_Cod, ")
            StrSQL.AppendLine(" ISNULL(ACCDAA_ANAG_T005_TabellaCodiciPaesiTerziISO3166.Descrizione, 'Italia') as Stato, ")
            StrSQL.AppendLine(" ISNULL(cacTipoCentro.val_cod, ' ') AS TipoCentro, ")
            StrSQL.AppendLine(" ISNULL ((SELECT TOP 1 val_cod FROM Centri_Aziendali_Codici WHERE Centri_Aziendali_Codici.id_cod = " & enum_CodiciAnagrafe.CodiceCentro_Attuale & " AND Centri_Aziendali_Codici.piva=ca.piva AND Centri_Aziendali_Codici.sa_cod=ca.sa_cod), ' ') AS CodiceOperatoreBio, ")
            StrSQL.AppendLine(" ISNULL ((SELECT TOP 1 val_cod FROM Centri_Aziendali_Codici WHERE Centri_Aziendali_Codici.id_cod = " & enum_CodiciAnagrafe.TipoAttivita & " AND Centri_Aziendali_Codici.piva=ca.piva AND Centri_Aziendali_Codici.sa_cod=ca.sa_cod), ' ') AS tipoAttivitaCod, ")

            StrSQL.AppendLine(" ISNULL ((SELECT TOP 1 [User] FROM Utenti WHERE CODICE_FISCALE = ca.Username_Creazione), ' ') AS Utente_Creazione, ")
            StrSQL.AppendLine(" ISNULL ((SELECT TOP 1 [User] FROM Utenti WHERE CODICE_FISCALE = ca.Username_Modifica), ' ') AS Utente_Modifica, ")

            StrSQL.AppendLine(" ca.Data_Creazione, ")
            StrSQL.AppendLine(" ca.Data_Modifica ")

            If leggiSuperfici Then
                StrSQL.AppendLine("  ,CAST(COALESCE(sup_catastali.Sup_Catastale, 0) as float) AS Superficie_Catastale ")
                StrSQL.AppendLine("  ,CAST(COALESCE(sup_appezzamenti.SAU_Convenzionale, 0) as float) AS Superficie_Convenzionale ")
                StrSQL.AppendLine("  ,CAST(COALESCE(sup_appezzamenti.SAU_Conversione, 0) as float) AS Superficie_Conversione ")
                StrSQL.AppendLine("  ,CAST(COALESCE(sup_appezzamenti.SAU_Biologico, 0) as float) AS Superficie_Biologico ")
                StrSQL.AppendLine("  ,CAST(COALESCE(sup_appezzamenti.SAU_Totale, 0) as float) AS Superficie_Totale ")
            End If

            StrSQL.AppendLine(" FROM Centri_Aziendali ca ")
            StrSQL.AppendLine(" INNER JOIN Imprese AS i ON i.PIVA = ca.PIVA ")
            StrSQL.AppendLine(" LEFT JOIN Imprese_Codici ic ON i.PIVA = ic.PIVA AND ic.id_cod = " & enum_CodiciAnagrafe.CodiceCUAA)
            If (leggiSuperfici) Then
                StrSQL.AppendLine(" LEFT JOIN sup_catastali ON ca.Piva = sup_catastali.Piva AND ca.Sa_Cod = sup_catastali.Sa_Cod ")
                StrSQL.AppendLine(" LEFT JOIN sup_appezzamenti ON ca.Piva = sup_appezzamenti.Piva AND ca.Sa_Cod = sup_appezzamenti.Sa_Cod ")
            End If
            StrSQL.AppendLine(" LEFT JOIN Centri_Aziendali_Codici cacTipoCentro ON ca.PIVA = cacTipoCentro.PIVA AND ca.sa_cod = cacTipoCentro.sa_cod AND cacTipoCentro.ID_Cod IN (101, 102, 103) ")
            StrSQL.AppendLine(" LEFT JOIN Codici_Anagrafe caTipoCentro ON caTipoCentro.creatore='CSA' AND gruppo='TIPO_CA' AND caTipoCentro.codice=cacTipoCentro.id_cod ")

            StrSQL.AppendLine(" INNER JOIN CentrixIndirizzi  ON CentrixIndirizzi.PIVA = ca.PIVA AND CentrixIndirizzi.sa_cod = ca.sa_cod AND CentrixIndirizzi.Tipo_Indirizzo = 1 ")
            StrSQL.AppendLine(" INNER JOIN Indirizzi AS ind ON ind.cod_indirizzo = CentrixIndirizzi.cod_indirizzo ")
            StrSQL.AppendLine(" LEFT JOIN ISTAT ON ind.pro_cod_istat = ISTAT.PROV AND ind.com_cod_istat = ISTAT.COM ")
            StrSQL.AppendLine(" LEFT JOIN Lista_Province ON ind.pro_cod_istat = Lista_Province.PROV ")
            StrSQL.AppendLine(" LEFT JOIN ACCDAA_ANAG_T005_TabellaCodiciPaesiTerziISO3166 ON ACCDAA_ANAG_T005_TabellaCodiciPaesiTerziISO3166.Codice = ind.Stato ")

            StrSQL.AppendLine(" WHERE 1 = 1 ")

            StrSQL.AppendLine(" AND   ca.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.AppendLine(" AND   ca.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            If filtroCentri <> "" Then
                StrSQL.AppendLine(filtroCentri)
            End If

            If Sa_Cod <> 0 Then
                StrSQL.AppendLine(" AND  ca.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            Else
                'leggo se ci sono filtri sui centri
                Dim objUtentiVisibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
                Dim FiltroCentriResp As String = ""
                Dim DtCentriVisibili As DataTable = objUtentiVisibilita.Leggi(enum_TipoEntita.Centro, filtroCentriVisibili, "", objParametri)
                If Not DtCentriVisibili Is Nothing AndAlso DtCentriVisibili.Rows.Count > 0 Then
                    For i = 0 To DtCentriVisibili.Rows.Count - 1
                        FiltroCentriResp &= DtCentriVisibili.Rows(i).Item("sa_cod") & ","
                    Next
                    If FiltroCentriResp <> "" Then
                        StrSQL.AppendLine(" AND ca.sa_cod IN (" & Agro_SQL_Save_Clausola_IN(Left(filtroCentri, filtroCentri.Length - 1), False) & ") ")
                    End If
                End If
            End If



            If filtroAggiuntivo <> "" Then
                StrSQL.AppendLine(filtroAggiuntivo)
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.AppendLine(" AND   ca.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.AppendLine(" AND   ca.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine(" ORDER BY Rag_Soc, ca.Sa_Nome")
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

    Public Function LeggiInfoCentri_x_StampaAutocertificazione(
                         ByVal ListaPiva As List(Of String),
                         ByVal Sa_Cod As Int32,
                         ByVal xFiltroAggiuntivo As String,
                         ByVal xOrderBy As String,
                         ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                         ByRef leggiSuperfici As Boolean
                             ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.CentriAziendali_Read.LeggiInfoCentri_x_StampaAutocertificazione()"
        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim i As Integer

        Try

            StrSQL.Length = 0
            StrSQL.AppendLine(" SELECT CASE ")
            StrSQL.AppendLine("  WHEN ISNULL(i.partitaIvaReale, '') = '' THEN i.PIVA ")
            StrSQL.AppendLine("  ELSE i.partitaIvaReale ")
            StrSQL.AppendLine(" END PivaReale, ")
            StrSQL.AppendLine(" i.Piva, ca.sa_cod, ca.sa_nome, ")
            StrSQL.AppendLine(" ind.ind_des, ind.CAP, ISNULL(ISTAT.LOCALITA, '') AS com_des, ISNULL(Lista_Province.Provincia, '') AS pro_cod  ")
            StrSQL.AppendLine(" FROM Centri_Aziendali ca ")
            StrSQL.AppendLine(" INNER JOIN Imprese AS i ON i.PIVA = ca.PIVA ")

            If Sa_Cod = 0 Then
                Dim objUtentiVisibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
                Dim FiltroCentri As String = ""
                Dim DtCentriVisibili As DataTable = objUtentiVisibilita.Leggi_Massiva(enum_TipoEntita.Centro, ListaPiva, "", "", objParametri)
                If DtCentriVisibili IsNot Nothing AndAlso DtCentriVisibili.Rows.Count > 0 Then

                    Dim listaChiaviCentri As List(Of (String, Integer)) = DtCentriVisibili.AsEnumerable().AsParallel().
                    Where(Function(row) Not row.IsNull("Piva") AndAlso Not row.IsNull("Sa_Cod")).
                    Select(Function(row) (row.Field(Of String)("Piva"), row.Field(Of Integer)("Sa_Cod"))).Distinct().ToList()

                    TempChiaviMassivo.CreaTabellaTemp_FiltroCentro(listaChiaviCentri, NomeRoutine, objParametri)

                    StrSQL.AppendLine(" INNER JOIN #TempCentro tmp ON tmp.Piva = i.Piva COLLATE DATABASE_DEFAULT ")
                    StrSQL.AppendLine(" AND tmp.Sa_Cod = ca.Sa_Cod ")

                End If
            End If

            ConnessioniTransazioni.ApriConnessione(True, objParametri)

            TempChiaviMassivo.CreaTabellaTemp_FiltroPiva(ListaPiva, NomeRoutine, objParametri)

            StrSQL.AppendLine(" INNER JOIN #TempPiva tmp ON tmp.Piva = i.Piva COLLATE DATABASE_DEFAULT ")
            StrSQL.AppendLine(" LEFT JOIN Imprese_Codici ic ON i.PIVA = ic.PIVA AND ic.id_cod = " & enum_CodiciAnagrafe.CodiceCUAA)
            StrSQL.AppendLine(" INNER JOIN CentrixIndirizzi  ON CentrixIndirizzi.PIVA = ca.PIVA AND CentrixIndirizzi.sa_cod = ca.sa_cod AND CentrixIndirizzi.Tipo_Indirizzo = 1 ")
            StrSQL.AppendLine(" INNER JOIN Indirizzi AS ind ON ind.cod_indirizzo = CentrixIndirizzi.cod_indirizzo ")
            StrSQL.AppendLine(" LEFT JOIN ISTAT ON ind.pro_cod_istat = ISTAT.PROV AND ind.com_cod_istat = ISTAT.COM ")
            StrSQL.AppendLine(" LEFT JOIN Lista_Province ON ind.pro_cod_istat = Lista_Province.PROV ")
            StrSQL.AppendLine(" WHERE 1 = 1 ")
            StrSQL.AppendLine(" AND   ca.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine))
            StrSQL.AppendLine(" AND   ca.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio))

            If Sa_Cod <> 0 Then
                StrSQL.AppendLine(" AND ca.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod))
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.AppendLine(" AND ca.Inviato >= 0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.AppendLine(" AND ca.Inviato = -1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    ' Nessun filtro aggiuntivo
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select

            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine(" ORDER BY ca.Sa_Nome")
            End If

            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)

            ' Eliminazione tabella temporanea
            TempChiaviMassivo.EliminaTabellaTemp_FiltroCentro(NomeRoutine, objParametri)

            'commit transazione
            ConnessioniTransazioni.ChiudiTransazione(1, objParametri)

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            'rollback transazione
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        Finally
            ConnessioniTransazioni.ChiudiConnessione(objParametri)
        End Try

        Return DT

    End Function


    Public Function NumeroCentri_Leggi(
                            ByVal Piva As String,
                                ByVal xFiltroAggiuntivo As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                ) As Integer

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.CentriAziendali_Read.NumeroCentri_Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append(" SELECT  COUNT(*) AS Num_Centri  ")
            StrSQL.Append(" FROM    Centri_Aziendali ")
            StrSQL.Append(" WHERE   Centri_Aziendali.Piva = '" + Agro_SQL_SaveText(Piva) + "' ")

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   Centri_Aziendali.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   Centri_Aziendali.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
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


        If Not IsNothing(DT) Then
            Return DT.Rows(0).Item("Num_Centri")
        Else
            Return 0
        End If

    End Function


    Public Function LeggiPerImpianti_Coltivazioni(ByVal Piva As String,
                                    ByVal Sa_Cod As Integer,
                                    ByVal Appezza As Integer,
                                    ByVal Veg_Cod As Int32,
                                    ByVal Cul_Cod As Int32,
                                    ByVal Gru_Cod As Int32,
                         ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile,
                         ByVal xFiltroAggiuntivo As String,
                         ByVal xOrderBy As String,
                         ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                         ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.CentriAziendali_Read.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""                   =>  si leggono tutte le imprese
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            'Select Case xSelezioneVariabile
            '    Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi
            '    Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta
            '    Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni
            '    Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta
            'End Select

            StrSQL.Length = 0
            StrSQL.Append(" SELECT  distinct Centri_Aziendali.* ")
            StrSQL.Append(" FROM  Centri_Aziendali ")
            StrSQL.Append(" INNER JOIN Reg_Impianti  ON Centri_Aziendali.PIVA = Reg_Impianti.PIVA and Centri_Aziendali.SA_COD = Reg_Impianti.SA_COD ")
            StrSQL.Append(" INNER JOIN Cultivar ON Cultivar.Cul_Cod = Reg_Impianti.Cul_Cod ")
            StrSQL.Append(" INNER JOIN SpecieVegetali on Cultivar.Veg_Cod = SpecieVegetali.Veg_Cod ")

            StrSQL.Append(" WHERE Centri_Aziendali.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.Append(" AND   Centri_Aziendali.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
            StrSQL.Append(" AND Reg_Impianti.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.Append(" AND   Reg_Impianti.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            If Piva <> "" Then

                StrSQL.Append(" AND  Centri_Aziendali.Piva = '" & Agro_SQL_SaveText(Piva) & "'")
            End If

            If Sa_Cod <> 0 Then
                StrSQL.Append(" AND  Centri_Aziendali.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            End If


            If Appezza <> 0 Then
                StrSQL.Append(" AND Reg_Impianti.Appezza = " & Agro_SQL_SaveNum(Appezza) & " ")
            End If

            If Cul_Cod <> 0 Then
                StrSQL.Append(" AND Cultivar.Cul_Cod = " & Agro_SQL_SaveNum(Cul_Cod) & " ")
            End If

            If Veg_Cod <> 0 Then
                StrSQL.Append(" AND SpecieVegetali.Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod) & " ")
            End If

            If Gru_Cod <> 0 Then
                StrSQL.Append(" AND SpecieVegetali.Gru_Cod = " & Agro_SQL_SaveNum(Gru_Cod) & " ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   Centri_Aziendali.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   Centri_Aziendali.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY Centri_Aziendali.Sa_Nome")
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

    Public Function LeggiPerImpianti_Coltivazioni_Data(
        ByVal Piva As String,
        ByVal Sa_Cod As Integer,
        ByVal Appezza As Integer,
        ByVal Veg_Cod As Int32,
        ByVal Cul_Cod As Int32,
        ByVal Gru_Cod As Int32,
        ByVal Data As DateTime,
        ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile,
        ByVal xFiltroAggiuntivo As String,
        ByVal xOrderBy As String,
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
        Optional ByVal Filtra_Validita_Esercizi As Boolean = False
        ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.CentriAziendali_Read.Leggi()"

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
                    StrSQL.AppendLine(" SELECT distinct ")
                    StrSQL.AppendLine(" Centri_Aziendali.PIVA, Centri_Aziendali.Sa_Cod, Centri_Aziendali.Sa_Nome, ")
                    StrSQL.AppendLine(" Centri_Aziendali.Validita_Inizio, Centri_Aziendali.Validita_Fine, Centri_Aziendali.inviato, ")
                    StrSQL.AppendLine(" Centri_Aziendali.lat, Centri_Aziendali.long, ")

                    StrSQL.AppendLine(" ISNULL(ISTAT.LOCALITA, '') AS com_des, ")
                    StrSQL.AppendLine(" ISNULL(Lista_Province.Provincia, '') AS pro_cod,")
                    StrSQL.AppendLine(" ind.pro_cod_istat,")
                    StrSQL.AppendLine(" ind.com_cod_istat,")
                    StrSQL.AppendLine(" ind.stato")

                    StrSQL.Append(" FROM  Centri_Aziendali ")
                    StrSQL.Append(" INNER JOIN Reg_Impianti  ON Centri_Aziendali.PIVA = Reg_Impianti.PIVA and Centri_Aziendali.SA_COD = Reg_Impianti.SA_COD ")
                    StrSQL.Append(" INNER JOIN Cultivar ON Cultivar.Cul_Cod = Reg_Impianti.Cul_Cod ")
                    StrSQL.Append(" INNER JOIN SpecieVegetali on Cultivar.Veg_Cod = SpecieVegetali.Veg_Cod ")

                    If Filtra_Validita_Esercizi Then
                        StrSQL.AppendLine(" INNER JOIN Imprese_Progetti ON Reg_Impianti.PIVA = Imprese_Progetti.PIVA AND Reg_Impianti.SA_COD = Imprese_Progetti.SA_COD AND ")
                        StrSQL.AppendLine(" Reg_Impianti.APPEZZA = Imprese_Progetti.APPEZZA AND Reg_Impianti.ID_REG = Imprese_Progetti.ID_REG")
                    End If

                    StrSQL.AppendLine("INNER JOIN CentrixIndirizzi  ON CentrixIndirizzi.PIVA = Centri_Aziendali.PIVA")
                    StrSQL.AppendLine("AND CentrixIndirizzi.sa_cod = Centri_Aziendali.sa_cod")
                    StrSQL.AppendLine("AND CentrixIndirizzi.Tipo_Indirizzo = 1")
                    StrSQL.AppendLine("INNER JOIN Indirizzi AS ind ON ind.cod_indirizzo = CentrixIndirizzi.cod_indirizzo")
                    StrSQL.AppendLine("LEFT JOIN ISTAT ON ind.pro_cod_istat = ISTAT.PROV")
                    StrSQL.AppendLine("AND ind.com_cod_istat = ISTAT.COM")
                    StrSQL.AppendLine("LEFT JOIN Lista_Province ON ind.pro_cod_istat = Lista_Province.PROV")

                    StrSQL.Append(" WHERE Centri_Aziendali.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND Centri_Aziendali.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.Append(" AND Reg_Impianti.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND Reg_Impianti.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    If Filtra_Validita_Esercizi Then
                        StrSQL.Append(" AND Imprese_Progetti.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                        StrSQL.Append(" AND Imprese_Progetti.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    End If

                    If Piva <> "" Then
                        StrSQL.Append(" AND  Centri_Aziendali.Piva = '" & Agro_SQL_SaveText(Piva) & "'")
                    End If

                    If Sa_Cod <> 0 Then
                        StrSQL.Append(" AND  Centri_Aziendali.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
                    Else
                        'leggo se ci sono filtri sui centri
                        Dim objUtentiVisibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
                        Dim FiltroCentri As String = ""
                        Dim DtCentriVisibili As DataTable
                        DtCentriVisibili = objUtentiVisibilita.Leggi(enum_TipoEntita.Centro, " Piva='" & Agro_SQL_SaveText(Piva) & "'", "", objParametri)
                        If Not DtCentriVisibili Is Nothing AndAlso DtCentriVisibili.Rows.Count > 0 Then
                            For i = 0 To DtCentriVisibili.Rows.Count - 1
                                FiltroCentri &= DtCentriVisibili.Rows(i).Item("sa_cod") & ","
                            Next
                            If FiltroCentri <> "" Then
                                StrSQL.Append(" AND Centri_Aziendali.sa_cod IN (" & Agro_SQL_Save_Clausola_IN(Left(FiltroCentri, FiltroCentri.Length - 1), False) & ") ")
                            End If
                        End If
                    End If


                    If Appezza <> 0 Then
                        StrSQL.Append(" AND Reg_Impianti.Appezza = " & Agro_SQL_SaveNum(Appezza) & " ")
                    End If

                    If Cul_Cod <> 0 Then
                        StrSQL.Append(" AND Cultivar.Cul_Cod = " & Agro_SQL_SaveNum(Cul_Cod) & " ")
                    End If

                    If Veg_Cod <> 0 Then
                        StrSQL.Append(" AND SpecieVegetali.Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod) & " ")
                    End If

                    If Gru_Cod <> 0 Then
                        StrSQL.Append(" AND SpecieVegetali.Gru_Cod = " & Agro_SQL_SaveNum(Gru_Cod) & " ")
                    End If

                    If Data <> AGRODATAINIZIO Then
                        StrSQL.Append(" AND ( Centri_Aziendali.Validita_Inizio <= " & Agro_SQL_SaveDate(Data) & " AND   Centri_Aziendali.Validita_Fine >= " & Agro_SQL_SaveDate(Data) & ") ")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   Centri_Aziendali.Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Centri_Aziendali.Inviato =-1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY Centri_Aziendali.Sa_Nome")
                    End If

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta
                    '
                    '
                    '
                    '

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

    Public Function Leggi_Coordinate(ByVal Piva As String,
                                        ByVal Sa_Cod As Int32,
                                            ByVal xFiltroAggiuntivo As String,
                                            ByVal xOrderBy As String,
                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                            ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.CentriAziendali_Read.Leggi_CoordinateUTM()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            StrSQL.Length = 0
            StrSQL.Append(" SELECT X, Y, long, lat ")
            StrSQL.Append(" FROM  Centri_Aziendali ")
            StrSQL.Append(" WHERE Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.Append(" AND   Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            If Piva <> "" Then
                StrSQL.Append(" AND  Piva = '" & Agro_SQL_SaveText(Piva) & "'")
            End If
            If Sa_Cod <> 0 Then
                StrSQL.Append(" AND  Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   Centri_Aziendali.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   Centri_Aziendali.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY x, y ASC")
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

    Public Sub Recupera_CoordinateUTM(ByRef Coordinata_X As Integer,
                                        ByRef Coordinata_Y As Integer,
                                            ByVal Piva As String,
                                            ByVal Sa_Cod As Integer,
                                                ByVal xFiltroAggiuntivo As String,
                                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                    )

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.CentriAziendali_Read.Recupera_CoordinateUTM()"

        '====================================================================================
        'Parametri opzionali :
        '   Nessuno
        '
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim DT As DataTable
        Dim MessaggioErrore As String

        Try

            Dim objCentri As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read

            DT = objCentri.Leggi_Coordinate(Piva,
                                          Sa_Cod,
                                          "",
                                          "",
                                          objParametri)

            If Not IsNothing(DT) Then
                If DT.Rows.Count <> 0 Then
                    Coordinata_X = DT.Rows(0).Item("X")
                    Coordinata_Y = DT.Rows(0).Item("Y")
                End If
            End If


        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try


    End Sub

    Public Sub Recupera_Coordinate(ByVal Piva As String,
                                   ByVal Sa_Cod As Integer,
                                   ByRef Coordinata_X As Decimal,
                                   ByRef Coordinata_Y As Decimal,
                                   ByRef Longitudine As Decimal,
                                   ByRef Latitudine As Decimal,
                                   ByVal xFiltroAggiuntivo As String,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri)


        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.CentriAziendali_Read.Recupera_Coordinate()"

        Dim DT As DataTable
        Dim MessaggioErrore As String

        Coordinata_X = 0
        Coordinata_Y = 0
        Longitudine = 0
        Latitudine = 0

        Try

            Dim objCentri As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read

            DT = objCentri.Leggi_Coordinate(Piva,
                                              Sa_Cod,
                                              xFiltroAggiuntivo,
                                              "",
                                              objParametri)

            If Not IsNothing(DT) AndAlso DT.Rows.Count > 0 Then
                Coordinata_X = DT.Rows(0).Item("X")
                Coordinata_Y = DT.Rows(0).Item("Y")
                Longitudine = DT.Rows(0).Item("Long")
                Latitudine = DT.Rows(0).Item("Lat")
            End If


        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try


    End Sub


    Public Function Esiste_Centro(ByVal Piva As String,
                                    ByVal Chiave_Cliente As String,
                                    ByVal Codice_Cliente As Integer,
                                    ByRef Data_Modifica As Date,
                                        ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile,
                                        ByVal xFiltroAggiuntivo As String,
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                        Optional ByRef Sa_Nome As String = "") As Integer

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.CentriAziendali_Read.Esiste_Centro()"

        '====================================================================================
        'Parametri opzionali :
        '   Nessuno
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim SaCod As Integer = 0

        Try
            StrSQL.Length = 0
            StrSQL.Append(" SELECT  Centri_Aziendali.*, Centri_Aziendali_Codici.id_cod, Centri_Aziendali_Codici.val_cod ")
            StrSQL.Append(" FROM    Centri_Aziendali, Centri_Aziendali_Codici ")
            StrSQL.Append(" WHERE   Centri_Aziendali.Piva = Centri_Aziendali_Codici.PIVA ")
            StrSQL.Append(" AND     Centri_Aziendali.sa_cod = Centri_Aziendali_Codici.sa_cod ")
            StrSQL.Append(" AND     Centri_Aziendali_Codici.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            If Codice_Cliente <> 0 Then
                StrSQL.Append(" AND     Centri_Aziendali_Codici.id_cod = " & Agro_SQL_SaveNum(Codice_Cliente) & " ")
            End If
            If Chiave_Cliente <> "" Then
                StrSQL.Append(" AND     Centri_Aziendali_Codici.val_cod = '" & Agro_SQL_SaveText(Chiave_Cliente) & "' ")
            End If


            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   Centri_Aziendali.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   Centri_Aziendali.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------


            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)

            If Not DT Is Nothing Then
                If DT.Rows.Count > 0 Then

                    Data_Modifica = CDate(DT.Rows(0).Item("data_modifica"))
                    SaCod = DT.Rows(0).Item("sa_cod")
                    Sa_Nome = DT.Rows(0).Item("sa_nome")

                Else

                    Data_Modifica = Nothing
                    SaCod = 0
                    Sa_Nome = ""

                End If
            End If


        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return SaCod


    End Function

    '###############################################################################
    'sa_cod può essere 0 
    Public Function Esiste_Centro2(ByVal Piva As String,
                                    ByVal Sa_Cod_Filtro As Integer,
                                    ByVal Sa_Nome_Filtro As String,
                                     ByRef Sa_Cod As Integer,
                                    ByRef Sa_Nome As String,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.CentriAziendali_Read.Esiste_Centro2()"
        Dim MessaggioErrore As String = ""
        Dim Esiste As Boolean = False

        Sa_Cod = 0
        Sa_Nome = ""

        Try
            Dim DT As DataTable
            Dim FiltroAgg As String = ""

            If Sa_Cod_Filtro = 0 And Sa_Nome_Filtro = "" Then
                Throw New Exception(" Sa_Cod_Filtro = 0 e Sa_Nome_Filtro = '', uno dei due deve essere valorizzato.")
            End If

            If Sa_Nome_Filtro <> "" Then
                FiltroAgg = " Centri_Aziendali.Sa_Nome LIKE '%" & Agro_SQL_SaveText(Trim(Sa_Nome_Filtro)) & "%' "
            End If


            DT = Leggi(Piva, Sa_Cod_Filtro,
                        enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                        FiltroAgg, "",
                        objParametri)

            If Not IsNothing(DT) Then
                If DT.Rows.Count <> 0 Then
                    Esiste = True
                    Sa_Cod = DT.Rows(0).Item("sa_cod")
                    Sa_Nome = DT.Rows(0).Item("sa_nome")
                End If
            End If


        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try


        Return Esiste

    End Function

    Public Function Anagrafica_Centri_Leggi(
                                            ByVal Piva As String,
                                            ByVal Sa_Cod As Integer,
                                            ByRef ErrMSG As String,
                                                ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile,
                                                ByVal xFiltroAggiuntivo As String,
                                                ByVal xOrderBy As String,
                                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.CentriAziendaliRead.Anagrafica_Centri_Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""                   =>  si leggono tutte le imprese
        '
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                    'TODO
                    'modificare query

                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT  Imprese.PIVA, Imprese.Rag_Soc, Centri_Aziendali.sa_cod, Centri_Aziendali.sa_nome, Centri_Aziendali.Validita_Inizio, Centri_Aziendali.Validita_Fine ")
                    StrSQL.Append(" FROM    Imprese, Centri_Aziendali ")
                    StrSQL.Append(" WHERE   Imprese.PIVA = Centri_Aziendali.PIVA ")

                    If Piva <> "" Then
                        StrSQL.Append(" AND     Imprese.PIVA = '" & Agro_SQL_SaveText(Piva) & "' ")
                    End If

                    If Sa_Cod <> 0 Then
                        StrSQL.Append(" AND     Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod.ToString) & " ")
                    End If


                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   Imprese.Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Imprese.Inviato =-1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY sa_nome ASC")
                    End If


                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta

                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT  Imprese.PIVA, Imprese.Rag_Soc, Centri_Aziendali.sa_cod, Centri_Aziendali.sa_nome, Centri_Aziendali.Validita_Inizio, Centri_Aziendali.Validita_Fine ")
                    StrSQL.Append(" FROM    Imprese, Centri_Aziendali ")
                    StrSQL.Append(" WHERE   Imprese.PIVA = Centri_Aziendali.PIVA ")
                    StrSQL.Append(" AND     Imprese.PIVA = '" & Agro_SQL_SaveText(Piva) & "' ")

                    If Sa_Cod <> 0 Then
                        StrSQL.Append(" AND     Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod.ToString) & " ")
                    End If


                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   Imprese.Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Imprese.Inviato =-1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY sa_nome ASC")
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
        Finally
            ErrMSG = MessaggioErrore
        End Try

        Return DT


    End Function



    '################################################################################
    Public Sub Recupera_Superfici_CentroAziendale(
                                        ByVal Piva As String,
                                        ByVal Sa_Cod As Integer,
                                        ByRef Sup_Totale As Decimal,
                                        ByRef Sup_Bosco As Decimal,
                                        ByRef Sup_Prati As Decimal,
                                        ByRef Sup_Tare As Decimal,
                                        ByRef SAU_Totale As Decimal,
                                        ByRef SAU_Biologico As Decimal,
                                        ByRef SAU_Conversione As Decimal,
                                        ByRef SAU_Convenzionale As Decimal,
                                        ByVal DataRecupero As Date,
                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                            )

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.CentriAziendaliRead.Anagrafica_Centri_Leggi()"

        Dim data_inizio_appoggio As Date
        Dim data_fine_appoggio As Date

        data_inizio_appoggio = objParametri.FinestraTemporaleInizio
        data_fine_appoggio = objParametri.FinestraTemporaleFine
        objParametri.FinestraTemporaleInizio = DataRecupero
        objParametri.FinestraTemporaleFine = DataRecupero

        Try

            Dim objParticelle As New AgronicaCoreAnagrafeDAL.ImpresexParticelle2_R
            Dim MessaggioErrore As String = ""
            Dim StrSQL As New System.Text.StringBuilder
            Dim DT As DataTable
            Dim TipoAgricoltura As AgronicaCoreDataProvider.TipiEnumerativi.enum_TipoAgricoltura

            '------------------------------
            '-----  Sup_Totale  -----------
            '------------------------------

            Dim SupHaCondotta As Decimal         'Superficie parziale in ettari
            Dim SupHaTot As Decimal      'Superficie totale in ettari    
            Dim SupHaCatasto As Decimal         'Superficie parziale in ettari


            'leggo le informazioni		
            DT = objParticelle.Leggi(0,
                                        CStr(Piva),
                                        CInt(Sa_Cod),
                                        0,
                                        "",
                                        "",
                                        "",
                                        0,
                                        0,
                                        "",
                                          AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                        "",
                                        "",
                                        objParametri)




            'Inizializzo
            SupHaCondotta = 0
            SupHaTot = 0
            Sup_Totale = 0

            Dim i As Integer
            For i = 0 To DT.Rows.Count - 1

                SupHaCondotta = CDbl(DT.Rows(i).Item("sup_condotta"))

                'Converto 
                SupHaCatasto = Ettari_from_EttariAreCentiare(
                                        CDbl(DT.Rows(i).Item("ettari")),
                                        CDbl(DT.Rows(i).Item("are")),
                                        CDbl(DT.Rows(i).Item("centiare")))

                If SupHaCondotta <> 0 Then
                    SupHaTot = SupHaTot + SupHaCondotta
                Else
                    'sup_condotta non valorizzata: prendo la sup. della particella
                    SupHaTot = SupHaTot + SupHaCatasto
                End If

            Next

            'Assegno il totale
            Sup_Totale = SupHaTot


            'resetto i valori di objParametri
            objParametri.FinestraTemporaleInizio = data_inizio_appoggio
            objParametri.FinestraTemporaleFine = data_fine_appoggio


            '------------------------------
            '-----  Sup_Bosco  ------------
            '-----  Sup_Prati  ------------
            '------------------------------
            Dim objCentri As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read

            DT = objCentri.Leggi(CStr(Piva),
                                    CInt(Sa_Cod),
                                      AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                    "",
                                    "",
                                    objParametri)

            Sup_Bosco = 0
            Sup_Prati = 0

            If Not IsNothing(DT) AndAlso DT.Rows.Count > 0 Then
                Sup_Bosco = CDbl(DT.Rows(0).Item("Sup_Bosco"))
                Sup_Prati = CDbl(DT.Rows(0).Item("Sup_Prati"))
            End If


            '------------------------------
            '-----  SAU_Totale  -----------
            '-----  SAU_Biologico  --------
            '-----  SAU_Conversione  ------
            '-----  SAU_Convenzionale  ----
            '------------------------------


            Try
                StrSQL.Length = 0
                StrSQL.Append(" SELECT  Appezzamento.PIVA, ")
                StrSQL.Append("         Appezzamento.SA_COD,  ")
                StrSQL.Append("         Appezzamento.Campo_Cod,  ")
                StrSQL.Append("         Appezzamento.APPEZZA,  ")
                StrSQL.Append("         Appezzamento.APP_NOME,  ")
                StrSQL.Append("         Appezzamento.SUP_APP,  ")
                StrSQL.Append("         Appezzamento_Codici.val_cod,  ")
                StrSQL.Append("         Appezzamento.Validita_Inizio,  ")
                StrSQL.Append("         Appezzamento.Validita_Fine, ")
                StrSQL.Append("         ISNULL(Appezzamento_Codici.id_cod, 1018) AS Id_Cod  ")
                StrSQL.Append(" FROM    Appezzamento LEFT OUTER JOIN ")
                StrSQL.Append("         Appezzamento_Codici ON Appezzamento.PIVA = Appezzamento_Codici.PIVA   ")
                StrSQL.Append("         AND Appezzamento.SA_COD = Appezzamento_Codici.sa_cod ")
                StrSQL.Append("         AND Appezzamento.APPEZZA = Appezzamento_Codici.appezza ")
                StrSQL.Append(" WHERE   (Appezzamento.PIVA = '" & Agro_SQL_SaveText(Trim(Piva)) & "')  ")
                StrSQL.Append(" AND     (Appezzamento.SA_COD = " & Agro_SQL_SaveNum(Sa_Cod) & ")  ")
                StrSQL.Append(" AND     (Appezzamento.Validita_Inizio <= " & Agro_SQL_SaveDate(DataRecupero) & ") ")
                StrSQL.Append(" AND     (Appezzamento.Validita_Fine >= " & Agro_SQL_SaveDate(DataRecupero) & ")")
                StrSQL.Append(" AND id_cod = 1018")

                '--------------------------------------------------------------------------
                DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
                '--------------------------------------------------------------------------

            Catch ex As Exception
                MessaggioErrore = ex.Message
                Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
                DT = Nothing
                Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
            End Try


            'Verifico la presenza di errori
            If IsNothing(MessaggioErrore) Then
                'ERRORE
                SAU_Totale = -999999
                SAU_Biologico = -999999
                SAU_Conversione = -999999
                SAU_Convenzionale = -999999
            Else
                'Inizializzo
                SAU_Totale = 0
                SAU_Biologico = 0
                SAU_Conversione = 0
                SAU_Convenzionale = 0

                If Not IsNothing(DT) Then
                    'Ciclo sugli elementi selezionati
                    For i = 0 To DT.Rows.Count - 1
                        SAU_Totale += CDbl(DT.Rows(i).Item("sup_app"))
                        'Verifico il tipo di agricoltura
                        If IsDBNull(DT.Rows(i).Item("val_cod")) Then
                            'NOTA
                            'Se non e' impostato il tipo di agricoltura,
                            'considero come default quella Convenzionale
                            TipoAgricoltura = AgronicaCoreDataProvider.TipiEnumerativi.enum_TipoAgricoltura.Convenzionale
                        Else
                            TipoAgricoltura = CInt(DT.Rows(i).Item("val_cod"))
                        End If

                        'Aggiorno il contatore giusto
                        Select Case TipoAgricoltura

                            Case AgronicaCoreDataProvider.TipiEnumerativi.enum_TipoAgricoltura.Convenzionale
                                SAU_Convenzionale += CDbl(DT.Rows(i).Item("sup_app"))

                            Case AgronicaCoreDataProvider.TipiEnumerativi.enum_TipoAgricoltura.InConversione
                                SAU_Conversione += CDbl(DT.Rows(i).Item("sup_app"))

                            Case AgronicaCoreDataProvider.TipiEnumerativi.enum_TipoAgricoltura.Biologica
                                SAU_Biologico += CDbl(DT.Rows(i).Item("sup_app"))
                        End Select
                    Next
                Else
                    SAU_Totale = 0
                    SAU_Biologico = 0
                    SAU_Conversione = 0
                    SAU_Convenzionale = 0
                End If

            End If


            '------------------------------
            '-----  Sup_Tare  -------------
            '------------------------------
            Sup_Tare = Sup_Totale - SAU_Totale



        Catch ex As Exception

        Finally
            'resetto i valori di objParametri
            objParametri.FinestraTemporaleInizio = data_inizio_appoggio
            objParametri.FinestraTemporaleFine = data_fine_appoggio
        End Try

    End Sub


    '###############################################################################
    Public Function EsisteCentro_RecuperaDati(ByVal Piva As String,
                                            ByRef Sa_Cod As Integer,
                                            ByRef Pro_Cod_Istat As String,
                                            ByRef Com_Cod_Istat As String,
                                            ByRef Ind_Des As String,
                                            ByRef Frz_Des As String,
                                            ByRef CAP As String,
                                            ByRef Titolo_Possesso As Integer,
                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean


        Dim DT As DataTable
        Dim Esiste As Boolean = False

        Sa_Cod = 0

        DT = Leggi(Piva, 0,
                    enumSelezioneVariabile.Selezione_JoinDescrizioni,
                    "", "",
                    objParametri)

        If Not IsNothing(DT) Then
            If DT.Rows.Count <> 0 Then
                Sa_Cod = DT.Rows(0).Item("Sa_Cod")
                Pro_Cod_Istat = DT.Rows(0).Item("Pro_Cod_Istat")
                Com_Cod_Istat = DT.Rows(0).Item("Com_Cod_Istat")
                Ind_Des = DT.Rows(0).Item("Ind_Des")
                Frz_Des = DT.Rows(0).Item("Frz_Des")
                CAP = DT.Rows(0).Item("CAP")
                Titolo_Possesso = DT.Rows(0).Item("Titolo_Possesso")
                Esiste = True
            End If
        End If

        Return Esiste

    End Function


    '###############################################################################
    ' a differenza della precedente ha il parametro Filtro in più
    Public Function EsisteCentro_RecuperaDati_2(ByVal Piva As String,
                                            ByVal Filtro As String,
                                            ByRef Sa_Cod As Integer,
                                            ByRef Pro_Cod_Istat As String,
                                            ByRef Com_Cod_Istat As String,
                                            ByRef Ind_Des As String,
                                            ByRef Frz_Des As String,
                                            ByRef CAP As String,
                                            ByRef Titolo_Possesso As Integer,
                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean


        Dim DT As DataTable
        Dim Esiste As Boolean = False

        Sa_Cod = 0

        DT = Leggi(Piva, 0,
                    enumSelezioneVariabile.Selezione_JoinDescrizioni,
                    Filtro, "",
                    objParametri)

        If Not IsNothing(DT) Then
            If DT.Rows.Count <> 0 Then
                Sa_Cod = DT.Rows(0).Item("Sa_Cod")
                Pro_Cod_Istat = DT.Rows(0).Item("Pro_Cod_Istat")
                Com_Cod_Istat = DT.Rows(0).Item("Com_Cod_Istat")
                Ind_Des = DT.Rows(0).Item("Ind_Des")
                Frz_Des = DT.Rows(0).Item("Frz_Des")
                CAP = DT.Rows(0).Item("CAP")
                Titolo_Possesso = DT.Rows(0).Item("Titolo_Possesso")
                Esiste = True
            End If
        End If

        Return Esiste

    End Function

    Public Function Leggi_CentriEsterni(
                                       ByVal PivaPadre As String,
                                       ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                       ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.CentriAziendali_Read.Leggi_CentriEsterni()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""                   =>  si leggono tutte le imprese
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim i As Integer

        Try
            StrSQL.Length = 0
            StrSQL.AppendLine("SELECT Imp.rag_soc,")
            StrSQL.AppendLine("       Centri_Aziendali.*")
            StrSQL.AppendLine("FROM  Centri_Aziendali")
            StrSQL.AppendLine("RIGHT JOIN Imprese Imp ON Centri_Aziendali.Piva = Imp.PIVA")
            StrSQL.AppendLine("WHERE Centri_Aziendali.Piva in (")
            StrSQL.AppendLine("    SELECT Figlio FROM GerarchiaImprese WHERE Padre = '" & Agro_SQL_SaveText(PivaPadre) & "'")
            'StrSQL.AppendLine("    SELECT Figlio FROM GerarchiaImprese WHERE Padre = 'UZ103055006'")
            StrSQL.AppendLine(")")

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

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="piva">Se stringa vuota, lette tutte le imprese</param>
    ''' <param name="includiFittizioTuttiCentri">Se <b>True</b> aggiunge dei record fittizzi in cui 
    ''' sa_cod = 0 e sa_nome = "Tutti i Centri"</param>
    ''' <param name="objParametri"></param>
    ''' <returns>DataTable con Piva, rag_Soc, sa_cod, sa_nome</returns>
    Public Function Leggi_CentriXImprese(piva As String,
                                         includiFittizioTuttiCentri As Boolean,
                                         objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri)
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.CentriAziendali_Read.Leggi_CentriXImprese()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""                   =>  si leggono tutte le imprese
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim i As Integer

        Try
            StrSQL.Length = 0

            If includiFittizioTuttiCentri Then
                StrSQL.AppendLine("SELECT ")
                StrSQL.AppendLine(" Imprese.Piva, Imprese.rag_soc,0 as Sa_cod, 'Tutti i Centri' as sa_nome ")
                StrSQL.AppendLine("FROM Imprese ")

                StrSQL.AppendLine(" UNION ")
            End If

            StrSQL.AppendLine("SELECT ")
            StrSQL.AppendLine(" Imprese.Piva, Imprese.rag_soc, Centri_Aziendali.sa_cod, Centri_Aziendali.sa_nome ")
            StrSQL.AppendLine("FROM Imprese ")
            StrSQL.AppendLine("JOIN Centri_Aziendali ON Imprese.Piva = Centri_Aziendali.Piva ")

            If piva <> "" Then
                StrSQL.AppendLine(" WHERE Imprese.Piva = '" & Agro_SQL_SaveText(piva) & "' ")

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

    Public Function ReadLatLng(objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                               Optional piva As String = "",
                               Optional saCod As Int32 = 0) As DataTable

        Dim routineName As String = "AgronicaCoreAnagrafeDAL.CentriAziendali_Read.ReadLatLng()"
        Dim stb As New System.Text.StringBuilder

        Try
            stb.AppendLine("SELECT PIVA, sa_cod, lat, long")
            stb.AppendLine("FROM Centri_Aziendali")
            stb.AppendLine("WHERE 1 = 1")

            If Not String.IsNullOrEmpty(piva) Then
                stb.AppendLine($"    AND PIVA = '{Agro_SQL_SaveText(piva)}'")
            End If

            If saCod <> 0 Then
                stb.AppendLine($"    AND sa_cod = {Agro_SQL_SaveNum(saCod)}")
            End If

            Return EseguiQuery_Lettura(objParametri, stb.ToString, routineName)
        Catch ex As Exception
            Scrivi_LOG(objParametri, routineName, ex.Message)
            Throw New Exception($"[{routineName}] : {ex.Message}")
        End Try
    End Function
End Class



'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§


Public Class CentriAziendali_Write
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function Scrivi(ByVal Piva As String,
                           ByVal Sa_Cod As Integer,
                           ByVal Sa_Nome As String,
                           ByVal X As Decimal,
                           ByVal Y As Decimal,
                           ByVal ZSLM As Decimal,
                           ByVal x_Long As Decimal,
                           ByVal x_Lat As Decimal,
                           ByVal Area As Decimal,
                           ByVal Ca_Sipi As String,
                           ByVal AT_Prevalente As String,
                           ByVal Forma_Possesso As String,
                           ByVal TitoloPossesso As Int16,
                           ByVal Sup_SAU_Convenzionale As Decimal,
                           ByVal Sup_SAU_Conversione As Decimal,
                           ByVal Sup_SAU_Biologico As Decimal,
                           ByVal Sup_Totale As Decimal,
                           ByVal Sup_Bosco As Decimal,
                           ByVal Sup_Tare As Decimal,
                           ByVal Sup_SAU As Decimal,
                           ByVal Sup_Prati As Decimal,
                           ByVal Validita_Inizio As Date,
                           ByVal Validita_Fine As Date,
                           ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                           ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                           Optional ByVal Data_creazione As DateTime = #2/1/1900#,
                           Optional ByVal Data_modifica As DateTime = #2/1/1900#,
                           Optional ByVal username_creazione As String = "",
                           Optional ByVal username_modifica As String = ""
                           ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.CentriAziendali_Write.Scrivi()"

        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        If Data_creazione = #2/1/1900# Then
            Data_creazione = Now
        End If

        If Data_modifica = #2/1/1900# Then
            Data_modifica = Now
        End If

        If username_creazione = "" Then
            username_creazione = objParametri_Server.UsernameOperazione
        End If

        If username_modifica = "" Then
            username_modifica = objParametri_Server.UsernameOperazione
        End If


        Try
            '---------------------------------------------
            strSql.Length = 0
            strSql.AppendLine("INSERT INTO Centri_Aziendali ")
            strSql.AppendLine("                    (Piva,   Sa_Cod,  Sa_Nome,       ")
            strSql.AppendLine("                    X,   Y,    ZSLM,                ")
            strSql.AppendLine("                    long,   lat,   area,   ca_sipi, ")
            strSql.AppendLine("                    AT_Prevalente, Forma_Possesso, TitoloPossesso, ")
            strSql.AppendLine("                    Sup_SAU_Convenzionale, Sup_SAU_Conversione, Sup_SAU_Biologico, ")
            strSql.AppendLine("                    Sup_Totale,      Sup_Bosco,   ")
            strSql.AppendLine("                    Sup_Tare,        Sup_SAU,   ")
            strSql.AppendLine("                    Sup_Prati, ")
            strSql.AppendLine("                    Inviato,            DataInvio, ")
            strSql.AppendLine("                    Data_Creazione,     Data_Modifica, ")
            strSql.AppendLine("                    UserName_Creazione, UserName_Modifica, ")
            strSql.AppendLine("                    Validita_Inizio,    Validita_Fine ")
            strSql.AppendLine("                    ) ")
            strSql.AppendLine("VALUES (")
            strSql.AppendLine("          '" & Agro_SQL_SaveText(Piva) & "' ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Sa_Cod))
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(Sa_Nome) & "' ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(X))
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Y))
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(ZSLM))
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(x_Long))
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(x_Lat))
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Area))
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(Ca_Sipi) & "' ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(AT_Prevalente) & "' ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(Forma_Possesso) & "' ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(TitoloPossesso))
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Sup_SAU_Convenzionale))
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Sup_SAU_Conversione))
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Sup_SAU_Biologico))
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Sup_Totale))
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Sup_Bosco))
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Sup_Tare))
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Sup_SAU))
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Sup_Prati))
            strSql.AppendLine("         , 0  ")
            strSql.AppendLine("         , Null  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveDateTime(Data_creazione) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveDateTime(Data_modifica) & "  ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(username_modifica) & "' ")
            strSql.AppendLine("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")
            strSql.AppendLine(")")
            '---------------------------------------------


            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri_Server, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

            'AggiornaUtentiProfili(objParametri_Server, objParametri_Utenti)
            AggiornaUtentiProfilixCentro(objParametri_Server, objParametri_Utenti, Piva)

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function


    '##############################################################################################
    Public Function Modifica(ByVal Piva As String,
                             ByVal Sa_Cod As Integer,
                             ByVal Sa_Nome As String,
                             ByVal X As Decimal,
                             ByVal Y As Decimal,
                             ByVal ZSLM As Decimal,
                             ByVal x_Long As Decimal,
                             ByVal x_Lat As Decimal,
                             ByVal Area As Decimal,
                             ByVal Ca_Sipi As String,
                             ByVal AT_Prevalente As String,
                             ByVal Forma_Possesso As String,
                             ByVal TitoloPossesso As Int16,
                             ByVal Sup_SAU_Convenzionale As Decimal,
                             ByVal Sup_SAU_Conversione As Decimal,
                             ByVal Sup_SAU_Biologico As Decimal,
                             ByVal Sup_Totale As Decimal,
                             ByVal Sup_Bosco As Decimal,
                             ByVal Sup_Tare As Decimal,
                             ByVal Sup_SAU As Decimal,
                             ByVal Sup_Prati As Decimal,
                             ByVal Validita_Inizio As Date,
                             ByVal Validita_Fine As Date,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                             Optional ByVal Data_modifica As DateTime = #2/1/1900#,
                             Optional ByVal username_modifica As String = ""
                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.CentriAziendali_Write.Modifica()"

        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        If Data_modifica = #2/1/1900# Then
            Data_modifica = Now
        End If

        If username_modifica = "" Then
            username_modifica = objParametri.UsernameOperazione
        End If

        Try
            '---------------------------------------------
            strSql.Length = 0
            strSql.AppendLine("UPDATE Centri_Aziendali SET ")
            strSql.AppendLine("    sa_nome               = '" & Agro_SQL_SaveText(Sa_Nome) & "'")
            strSql.AppendLine("   ,X                     =  " & Agro_SQL_SaveNum(X) & " ")
            strSql.AppendLine("   ,Y                     =  " & Agro_SQL_SaveNum(Y) & " ")
            strSql.AppendLine("   ,ZSLM                  =  " & Agro_SQL_SaveNum(ZSLM) & " ")
            strSql.AppendLine("   ,long                  =  " & Agro_SQL_SaveNum(x_Long) & " ")
            strSql.AppendLine("   ,lat                   =  " & Agro_SQL_SaveNum(x_Lat) & " ")
            strSql.AppendLine("   ,area                  =  " & Agro_SQL_SaveNum(Area) & " ")
            strSql.AppendLine("   ,ca_sipi               = '" & Agro_SQL_SaveText(Ca_Sipi) & "'")
            strSql.AppendLine("   ,AT_Prevalente         = '" & Agro_SQL_SaveText(AT_Prevalente) & "'")
            strSql.AppendLine("   ,Forma_Possesso        = '" & Agro_SQL_SaveText(Forma_Possesso) & "'")
            strSql.AppendLine("   ,TitoloPossesso        =  " & Agro_SQL_SaveNum(TitoloPossesso) & " ")
            strSql.AppendLine("   ,Sup_SAU_Convenzionale =  " & Agro_SQL_SaveNum(Sup_SAU_Convenzionale) & " ")
            strSql.AppendLine("   ,Sup_SAU_Conversione   =  " & Agro_SQL_SaveNum(Sup_SAU_Conversione) & " ")
            strSql.AppendLine("   ,Sup_SAU_Biologico     =  " & Agro_SQL_SaveNum(Sup_SAU_Biologico) & " ")
            strSql.AppendLine("   ,Sup_Totale            =  " & Agro_SQL_SaveNum(Sup_Totale) & " ")
            strSql.AppendLine("   ,Sup_Bosco             =  " & Agro_SQL_SaveNum(Sup_Bosco) & " ")
            strSql.AppendLine("   ,Sup_Tare              =  " & Agro_SQL_SaveNum(Sup_Tare) & " ")
            strSql.AppendLine("   ,Sup_SAU               =  " & Agro_SQL_SaveNum(Sup_SAU) & " ")
            strSql.AppendLine("   ,Sup_Prati             =  " & Agro_SQL_SaveNum(Sup_Prati) & " ")
            strSql.AppendLine("   ,Inviato           = 0 ")
            strSql.AppendLine("   ,DataInvio         = Null ")
            strSql.AppendLine("   ,Data_Modifica     =  " & Agro_SQL_SaveDateTime(Data_modifica))
            strSql.AppendLine("   ,UserName_Modifica = '" & Agro_SQL_SaveText(username_modifica) & "'")
            strSql.AppendLine("   ,Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Inizio))
            strSql.AppendLine("   ,Validita_Fine     =  " & Agro_SQL_SaveDate(Validita_Fine))
            strSql.AppendLine(" WHERE Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            strSql.AppendLine(" AND   Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            '---------------------------------------------

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

    Public Function Modifica_sa_nome(
                            ByVal Piva As String,
                            ByVal Sa_Cod As Int32,
                            ByVal Sa_Nome As String,
                                    ByVal xFiltroAggiuntivo As String,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                    ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.CentriAziendali_Write.Modifica()"

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
            StrSQL.Append("UPDATE Centri_Aziendali SET ")
            StrSQL.Append("    sa_nome               = '" & Agro_SQL_SaveText(Sa_Nome) & "'")
            StrSQL.Append(" WHERE Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            StrSQL.Append(" AND   Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
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
    Public Function Cancella(ByVal Piva As String,
                             ByVal Sa_Cod As Integer,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                             ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri
                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.CentriAziendali_Write.Cancella()"

        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try


            '---------------------------------------------
            If objParametri_Server.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then

                strSql.Length = 0
                strSql.AppendLine(" UPDATE Centri_Aziendali ")
                strSql.AppendLine(" SET ")
                strSql.AppendLine("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri_Server.UsernameOperazione) & "' ")
                strSql.AppendLine("      ,Inviato = -1 ")
                strSql.AppendLine(" WHERE  Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
                strSql.AppendLine(" AND Inviato >= 0")

                If Sa_Cod <> 0 Then
                    strSql.AppendLine(" AND   Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
                End If

            Else

                strSql.Length = 0
                strSql.AppendLine(" DELETE ")
                strSql.AppendLine(" FROM     Centri_Aziendali ")
                strSql.AppendLine(" WHERE    Piva= '" & Agro_SQL_SaveText(Piva) & "' ")

                If Sa_Cod <> 0 Then
                    strSql.AppendLine(" AND   Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
                End If

            End If
            '---------------------------------------------

            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri_Server))
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri_Server, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

            'AggiornaUtentiProfili(objParametri_Server, objParametri_Utenti)
            AggiornaUtentiProfilixCentro(objParametri_Server, objParametri_Utenti, Piva)

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function

    '##############################################################################################
    Public Function AggiornaValiditaInizio(
                            ByVal Piva As String,
                            ByVal Sa_Cod As Int32,
                            ByVal Validita_Inizio As Date,
                                ByVal xFiltroAggiuntivo As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                ) As Boolean

        Dim NomeRoutine As String = "AnagrafeCoreAnagrafeDAL.CentriAziendali_Write.AggiornaValiditaInizio()"

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
            StrSQL.Append("UPDATE Centri_Aziendali SET ")
            StrSQL.Append("   UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.Append("   ,Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Inizio))
            StrSQL.Append(" WHERE Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            StrSQL.Append(" AND   Validita_Inizio < " & Agro_SQL_SaveDate(Validita_Inizio))

            If Sa_Cod <> 0 Then
                StrSQL.Append(" AND   Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
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
    Public Function AggiornaValiditaFine(
                                ByVal Piva As String,
                                ByVal Sa_Cod As Int32,
                                ByVal Validita_Fine As Date,
                                    ByVal xFiltroAggiuntivo As String,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                    ) As Boolean

        Dim NomeRoutine As String = "AnagrafeCoreAnagrafeDAL.CentriAziendali_Write.AggiornaValiditaFine()"

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
            StrSQL.Append("UPDATE Centri_Aziendali SET ")
            StrSQL.Append("   UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.Append("   ,Validita_Fine   =  " & Agro_SQL_SaveDate(Validita_Fine))
            StrSQL.Append(" WHERE Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            StrSQL.Append(" AND   Validita_Fine > " & Agro_SQL_SaveDate(Validita_Fine))

            If Sa_Cod <> 0 Then
                StrSQL.Append(" AND   Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
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
    Public Function Modifica_CoordinateUTM(
                            ByVal Piva As String,
                            ByVal Sa_Cod As Int32,
                            ByVal X As Decimal,
                            ByVal Y As Decimal,
                                    ByVal xFiltroAggiuntivo As String,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                    ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.CentriAziendali_Write.Modifica_CoordinateUTM()"

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
            If Piva = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
            End If

            If Sa_Cod = 0 Then
                Throw New Exception("Parametro non corretto nella query (Sa_Cod obbligatorio)")
            End If


            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append("UPDATE Centri_Aziendali SET ")
            StrSQL.Append("   X                     =  " & Agro_SQL_SaveNum(X) & " ")
            StrSQL.Append("   ,Y                     =  " & Agro_SQL_SaveNum(Y) & " ")
            StrSQL.Append("   ,Data_Modifica     =  " & Agro_SQL_SaveDate(Date.Now))
            StrSQL.Append("   ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.Append(" WHERE Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            StrSQL.Append(" AND   Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
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





    Function Modifica_CoordinateLatLong_e_XY(Piva As String, Sa_Cod As Integer,
                                             Latitudine As Decimal,
                                             Longitudine As Decimal,
                                             ByVal X As Decimal,
                                             ByVal Y As Decimal,
                                             objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.CentriAziendali_Write.Modifica_CoordinateLatLong_e_XY()"


        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            If Piva = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
            End If

            If Sa_Cod = 0 Then
                Throw New Exception("Parametro non corretto nella query (Sa_Cod obbligatorio)")
            End If

            ''Dim X As Decimal = 0
            ''Dim Y As Decimal = 0
            ''If Latitudine <> 0 AndAlso Longitudine <> 0 Then
            ''    'CalcolaXY(X,Y,longitudine,latitudine)
            ''End If

            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append("UPDATE Centri_Aziendali SET ")
            StrSQL.Append("   X                     =  " & Agro_SQL_SaveNum(X) & " ")
            StrSQL.Append("   ,Y                     =  " & Agro_SQL_SaveNum(Y) & " ")
            StrSQL.Append("   ,lat                    =  " & Agro_SQL_SaveNum(Latitudine) & " ")
            StrSQL.Append("   ,long                     =  " & Agro_SQL_SaveNum(Longitudine) & " ")
            StrSQL.Append("   ,Data_Modifica     =  " & Agro_SQL_SaveDate(Date.Now))
            StrSQL.Append("   ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri_Server.UsernameOperazione) & "'")
            StrSQL.Append(" WHERE Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            StrSQL.Append(" AND   Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            '---------------------------------------------


            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri_Server, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp
    End Function

    ''' <summary>
    ''' Notifico la utenti_profili del cambiamento di modo che possano essere aggiornati gli utenti
    ''' </summary>
    ''' <param name="objParametri_server"></param>
    ''' <param name="objParametri_utenti"></param>
    ''' <returns></returns>
    Private Function AggiornaUtentiProfili(ByRef objParametri_server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                          ByRef objParametri_utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                          ByVal PivaPadre As String,
                                          ByVal Piva As String) As Boolean
        Dim NomeRoutine As String = "AnagrafeCoreAnagrafeDAL.GerarchiaImprese_W.AggiornaUtentiProfili()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            'Dim strFiltro As String = " (Descrizione_2 like '%GerarchiaImprese.Padre = ''" & PivaPadre & "''%' OR Descrizione_2 like '%Imprese.piva=''" & Piva & "''%') "
            Dim strFiltro As String = " (Descrizione_2 like '" & Agro_SQL_SaveText($"%GerarchiaImprese.Padre = '{PivaPadre}'%") & "' OR " &
                "Descrizione_2 like '" & Agro_SQL_SaveText($"%GerarchiaImprese.Padre = '{Piva}'%") & "' OR " &
                "Descrizione_2 like '" & Agro_SQL_SaveText($"%Imprese.Piva = '{Piva}'%") & "')"

            '---------------------------------------------
            StrSQL.AppendLine("UPDATE Utenti_Profili WITH (ROWLOCK) SET")
            StrSQL.AppendLine(" DataUltimoRiportoUtentiVisibilitaAppoggio = NULL")
            StrSQL.AppendLine("WHERE " & strFiltro & " ")
            StrSQL.AppendLine("AND DataUltimoRiportoUtentiVisibilitaAppoggio IS NOT NULL ")
            xRisp = EseguiQuery_Scrittura(objParametri_utenti, StrSQL.ToString, NomeRoutine)

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_server, NomeRoutine, MessaggioErrore)
            xRisp = False
            'Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp

    End Function

    Public Function AggiornaUtentiProfilixCentro(ByRef objParametri_server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                          ByRef objParametri_utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                          ByVal Piva As String) As Boolean
        Dim NomeRoutine As String = "AnagrafeCoreAnagrafeDAL.GerarchiaImprese_W.AggiornaUtentiProfilixCentro()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            StrSQL.AppendLine("SELECT * FROM GerarchiaImprese (NOLOCK) WHERE Figlio = '" & Agro_SQL_SaveText(Piva) & "' ")

            Dim DT = EseguiQuery_Lettura(objParametri_server, StrSQL.ToString, NomeRoutine)

            If DT IsNot Nothing AndAlso DT.Rows.Count > 0 Then
                For Each row In DT.Rows
                    AggiornaUtentiProfili(objParametri_server, objParametri_utenti, row("Padre"), Piva)
                Next
            End If

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_server, NomeRoutine, MessaggioErrore)
            xRisp = False
            'Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp

    End Function


End Class
