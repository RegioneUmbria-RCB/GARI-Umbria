Imports System.Data.Common
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider

<CachedDataProviderAttribute("Istat_R")>
Public Class Istat_R
    Inherits AgronicaCoreDataProvider.CachedDataProvider



    ''##############################################################################################
    'Public Function Leggi(ByVal PROV As String, _
    '                      ByVal COM As String, _
    '                      ByVal FinestraTemp_Inizio As Date, _
    '                      ByVal FinestraTemp_Fine As Date, _
    '                      ByRef objConnessione As DbConnection, _
    '                      ByVal StringaConnessione As String, _
    '                      ByVal FlagVisibilita As Int32, _
    '                      ByVal DirectoryLOG As String, _
    '                      ByVal FileLOG As String, _
    '                      ByVal IdentificatoreUtente As String _
    '                      ) As DataTable

    '    Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.Istat_R.Leggi()"

    '    '====================================================================================
    '    'Parametri opzionali :

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
    '        StrSQL.Append(" FROM  Istat ")
    '        StrSQL.Append(" WHERE Validita_inizio < " & Agro_SQL_SaveDate(FinestraTemp_Fine) & " ")
    '        StrSQL.Append(" AND   Validita_Fine > " & Agro_SQL_SaveDate(FinestraTemp_Inizio) & " ")

    '        If PROV <> "" Then
    '            StrSQL.Append(" AND PROV = '" & Agro_SQL_SaveText(Trim(PROV)) & "' ")
    '        End If

    '        If COM <> "" Then
    '            StrSQL.Append(" AND COM = '" & Agro_SQL_SaveText(Trim(COM)) & "' ")
    '        End If


    '        Select Case FlagVisibilita
    '            Case 1  'Solo i NON CANCELLATI
    '                StrSQL.Append(" AND     Inviato >= 0 ")
    '                StrSQL.Append(" AND     Inviato >= 0 ")
    '            Case 2  'Solo i CANCELLATI
    '                StrSQL.Append(" AND     Inviato = -1 ")
    '                StrSQL.Append(" AND     Inviato = -1 ")
    '            Case 3  'TUTTI
    '                '
    '            Case Else
    '                Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
    '        End Select

    '        StrSQL.Append(" ORDER BY Comuni_Prov, Localita ")

    '        '---------------------------------------------

    '        DT = EseguiQuery_Lettura(objConnessione, StringaConnessione, StrSQL.ToString, DirectoryLOG, FileLOG, IdentificatoreUtente, NomeRoutine)


    '    Catch ex As Exception

    '        MessaggioErrore = ex.Message
    '        Scrivi_LOG(DirectoryLOG, FileLOG, IdentificatoreUtente, NomeRoutine, MessaggioErrore)
    '        DT = Nothing
    '        Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

    '    End Try

    '    Return DT

    'End Function

    <Cacheable(True)>
    Public Function Leggi(ByVal PROV As String,
                          ByVal COM As String,
                          ByVal Comuni_Prov As String,
                          ByVal Cap As String,
                          ByVal Filtro_StrComune As String,
                          ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                          Optional ByVal CodiceCatastale As String = "",
                          Optional ByVal Stato_Country As String = ""
                                ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.Istat_R.Leggi()"

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

        PROV = Trim(PROV)
        If PROV.Length < 3 Then
            If PROV.Length = 1 Then
                PROV = "00" & PROV
            End If
            If PROV.Length = 2 Then
                PROV = "0" & PROV
            End If
        End If

        COM = Trim(COM)
        If COM.Length < 3 Then
            If COM.Length = 1 Then
                COM = "00" & COM
            End If
            If COM.Length = 2 Then
                COM = "0" & COM
            End If
        End If

        Try
            '---------------------------------------------
            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta
                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT * ")
                    StrSQL.Append(" FROM  Istat ")
                    StrSQL.Append(" WHERE Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND   Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    If PROV <> "" Then
                        StrSQL.Append(" AND PROV = '" & Agro_SQL_SaveText((PROV)) & "' ")
                    End If

                    If COM <> "" Then
                        StrSQL.Append(" AND COM = '" & Agro_SQL_SaveText((COM)) & "' ")
                    End If

                    If Comuni_Prov <> "" Then
                        StrSQL.Append(" AND Comuni_Prov = '" & Agro_SQL_SaveText(Comuni_Prov) & "' ")
                    End If

                    If Cap <> "" Then
                        StrSQL.Append(" AND Cap = '" & Agro_SQL_SaveText(Cap) & "' ")
                    End If

                    If CodiceCatastale <> "" Then
                        StrSQL.Append(" AND CodiceCatastale = " + Agro_SQL_SaveText_NULL(CodiceCatastale) + " ")
                    End If

                    If Filtro_StrComune <> "" Then
                        StrSQL.Append(" AND Localita LIKE '%" & Agro_SQL_SaveText(Filtro_StrComune) & "%' ")
                    End If

                    If Stato_Country <> "" Then

                        If UCase(Left(Stato_Country & " ", 3)) = "ITA" Then
                            Stato_Country = "IT"
                        End If

                        StrSQL.Append(" AND Stato_Country = " + Agro_SQL_SaveText_NULL(Stato_Country) + " ")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND     Inviato >= 0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND     Inviato = -1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY Comuni_Prov, Localita ")
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


    '################################################################################
    Public Sub ComuneProvinciaSigla_from_codISTAT(ByVal PROV As String,
                                                    ByVal COM As String,
                                                    ByRef SIGLA_PROVINCIA As String,
                                                    ByRef Provincia As String,
                                                    ByRef Comune As String,
                                                    ByVal xFiltroAggiuntivo As String,
                                                    ByVal xOrderBy As String,
                                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                    )

        SIGLA_PROVINCIA = ""
        Provincia = ""
        Comune = ""

        Dim dt As DataTable

        dt = ISTATXListaProvince(PROV, COM, "", "", "", "", "", "",
                                "", "", objParametri)

        If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then
            SIGLA_PROVINCIA = dt.Rows(0).Item("SIGLA")
            Provincia = dt.Rows(0).Item("Provincia")
            Comune = dt.Rows(0).Item("Localita")
        End If


    End Sub

    Public Sub ComuneProvinciaSiglaCAP_from_codISTAT(ByVal PROV As String,
                                                    ByVal COM As String,
                                                    ByRef SIGLA_PROVINCIA As String,
                                                    ByRef Provincia As String,
                                                    ByRef Comune As String,
                                                     ByRef Cap As String,
                                                    ByVal xFiltroAggiuntivo As String,
                                                    ByVal xOrderBy As String,
                                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                    )

        SIGLA_PROVINCIA = ""
        Provincia = ""
        Comune = ""

        Dim dt As DataTable

        dt = ISTATXListaProvince(PROV, COM, "", "", "", "", "", "",
                                "", "", objParametri)

        If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then
            SIGLA_PROVINCIA = dt.Rows(0).Item("SIGLA")
            Provincia = dt.Rows(0).Item("Provincia")
            Comune = dt.Rows(0).Item("Localita")
            Cap = dt.Rows(0).Item("cap")
        End If


    End Sub

    '################################################################################
    <Cacheable(True)>
    Public Function ISTATXListaProvince(ByVal PROV As String,
                                        ByVal COM_LOCALITA As String,
                                        ByVal COM_PROVINCIA As String,
                                        ByVal SIGLA_PROVINCIA As String,
                                        ByVal CAP As String,
                                        ByVal REG As String,
                                        ByVal Filtro_StrProvincia As String,
                                        ByVal Filtro_StrComune As String,
                                        ByVal xFiltroAggiuntivo As String,
                                        ByVal xOrderBy As String,
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                        ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.Istat_R.ISTATXListaProvince()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        PROV = Trim(PROV)
        If PROV.Length < 3 Then
            If PROV.Length = 1 Then
                PROV = "00" & PROV
            End If
            If PROV.Length = 2 Then
                PROV = "0" & PROV
            End If
        End If



        Try

            StrSQL.Length = 0

            StrSQL.Append(" SELECT   ISTAT.PROV, ISTAT.COM AS COM_LOCALITA, ISTAT.LOCALITA, ISTAT.CAP, Lista_Province.REG, Lista_Province.COM AS COM_PROVINCIA,  Lista_Province.PROVINCIA, Lista_Province.SIGLA  " + vbCrLf)
            StrSQL.Append(" FROM    ISTAT   INNER JOIN  Lista_Province ON ISTAT.PROV = Lista_Province.PROV " + vbCrLf)

            StrSQL.Append(" WHERE   ISTAT.Validita_Inizio <=" & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) + vbCrLf)
            StrSQL.Append(" AND     ISTAT.Validita_Fine >=" & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) + vbCrLf)
            StrSQL.Append(" AND     Lista_Province.Validita_Inizio <=" & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) + vbCrLf)
            StrSQL.Append(" AND     Lista_Province.Validita_Fine >=" & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) + vbCrLf)

            If PROV <> "" Then
                StrSQL.Append(" AND ISTAT.PROV = '" & Agro_SQL_SaveText(PROV) & "' " + vbCrLf)
            End If

            If COM_LOCALITA <> "" Then
                StrSQL.Append(" AND ISTAT.COM = '" & Agro_SQL_SaveText(COM_LOCALITA) & "' " + vbCrLf)
            End If

            If COM_PROVINCIA <> "" Then
                StrSQL.Append(" AND Lista_Province.COM = '" & Agro_SQL_SaveText(COM_PROVINCIA) & "' " + vbCrLf)
            End If

            If SIGLA_PROVINCIA <> "" Then
                StrSQL.Append(" AND Lista_Province.SIGLA = '" & Agro_SQL_SaveText(SIGLA_PROVINCIA) & "' " + vbCrLf)
            End If

            If CAP <> "" Then
                StrSQL.Append(" AND ISTAT.CAP = '" & Agro_SQL_SaveText(CAP) & "' " + vbCrLf)
            End If

            If REG <> "" Then
                StrSQL.Append(" AND Lista_Province.REG = '" & Agro_SQL_SaveText(REG) & "' " + vbCrLf)
            End If

            If Filtro_StrComune <> "" Then
                StrSQL.Append(" AND Lista_Province.PROVINCIA LIKE '%" & Agro_SQL_SaveText(Filtro_StrProvincia) & "%' " + vbCrLf)
            End If

            If Filtro_StrComune <> "" Then
                StrSQL.Append(" AND  ISTAT.LOCALITA LIKE '%" & Agro_SQL_SaveText(Filtro_StrComune) & "%' " + vbCrLf)
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND     ISTAT.Inviato >= 0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND     ISTAT.Inviato = -1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY PROVINCIA, LOCALITA ")
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
    Public Function Provincia_from_CodIstat(ByVal Provincia_CodIstat As String,
                                            ByRef Out_SiglaProv As String,
                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                            ) As String

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.Istat_R.Provincia_from_CodIstat()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable


        Dim objIstatR As New AgronicaCoreMetaSchemaDAL.Istat_R

        DT = objIstatR.Leggi(CStr(Provincia_CodIstat), "", "", "", "", AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                            "", "", objParametri)

        If Not DT Is Nothing AndAlso DT.Rows.Count > 0 Then

            Out_SiglaProv = DT.Rows(0).Item("Comuni_Prov")
        End If



        Dim objLista_Province_R As New AgronicaCoreMetaSchemaDAL.Lista_Province_R

        DT = objLista_Province_R.Leggi(
                        CStr(Out_SiglaProv), "",
                            AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                            "",
                            "",
                            objParametri)

        If Not DT Is Nothing AndAlso DT.Rows.Count > 0 And Out_SiglaProv <> "" Then

            Return DT.Rows(0).Item("Provincia")
        End If

    End Function

    '##############################################################################################
    Public Function Comune_from_CodIstat(ByVal Comune_CodIstat As String,
                                         ByVal Provincia_CodIstat As String,
                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                            ) As String

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.Istat_R.Comune_from_CodIstat()"

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
        Dim strRet As String = String.Empty

        Dim objIstat As New AgronicaCoreMetaSchemaDAL.Istat_R


        'Recupero le informazioni		
        DT = objIstat.Leggi(Provincia_CodIstat,
                            Comune_CodIstat,
                            "", "", "",
                            AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                            "",
                            "",
                            objParametri)

        'Elimino la classe
        objIstat = Nothing

        'Se il recordset non è chiuso allora ...	
        If DT.Rows.Count <> 0 Then

            strRet = DT.Rows(0).Item("Localita")

        End If

        'Elimino il recordset
        DT = Nothing

        Return strRet

    End Function


    '##############################################################################################
    Public Function CodIstat_from_Provincia(ByVal Provincia_Sigla As String,
                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                            ) As String

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.Istat_R.CodIstat_from_Provincia()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim strRet As String = String.Empty

        Dim objIstat As New AgronicaCoreMetaSchemaDAL.Istat_R

        DT = objIstat.Leggi("",
                               "", CStr(Provincia_Sigla), "", "",
                               AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                               "", "", objParametri)
        'Se il recordset non è chiuso allora ...	
        If DT.Rows.Count > 0 Then

            Return DT.Rows(0).Item("Prov")

        End If

    End Function



    '##############################################################################################
    Public Function Esiste_ISTAT(ByVal PROV As String,
                                 ByVal COM As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.Istat_R.Esiste_ISTAT()"
        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim Esiste As Boolean = False

        DT = Leggi(PROV,
                    COM,
                    "", "", "",
                    AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                    "", "", objParametri)

        If Not IsNothing(DT) AndAlso DT.Rows.Count > 0 Then
            Esiste = True
        End If

        Return Esiste

    End Function


    '################################################################################
    Public Sub Trova_Codici_Istat_SincronizzatoreUniversale(ByVal Sigla_Pro As String,
                                                            ByVal Local_Coltiva As String,
                                                            ByRef Cap As String,
                                                            ByRef Pro_Cod_Istat As String,
                                                            ByRef Com_Cod_Istat As String,
                                                            ByRef Frz_Des As String,
                                                            ByRef Com_Des As String,
                                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                            )

        Dim MessaggioErrore As String
        Dim ObjProvIstat As New AgronicaCoreMetaSchemaDAL.Lista_Province_R
        Dim ObjIstat As New AgronicaCoreMetaSchemaDAL.Istat_R
        Dim z As Integer
        Dim DTProvincia As DataTable
        Dim DTComune As DataTable
        Dim Array_Split(0) As Object
        Dim Comuni(3, 0) As Object
        Dim i As Integer
        Dim j As Integer
        Dim Ultimo_Comune_Escluso As Integer
        Dim No_Candidati As Integer
        Dim Comune_Candidato As Integer
        Dim SplitStep As Integer
        Dim Trovato As Boolean
        DIm ComuneTrovatoMatchEsatto As Boolean = False
        Dim Condizione As Boolean
        Dim mSiglePro() As Object

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.Istat_R.Trova_Codici_Istat_SincronizzatoreUniversale()"

        Try

            If Not IsNothing(Sigla_Pro) AndAlso Sigla_Pro <> "" Then

                Sigla_Pro = Trim(Sigla_Pro)
                Sigla_Pro = Replace(Sigla_Pro, ".", "")
                Sigla_Pro = Replace(Sigla_Pro, ",", "")

                If Sigla_Pro.ToUpper = "FO" Then
                    'forlì -> forlì-cesena
                    Sigla_Pro = "FC"
                End If
                If Sigla_Pro.ToUpper = "PS" Then
                    'pesaro -> pesaro-urbino
                    Sigla_Pro = "PU"
                End If
                If Sigla_Pro.ToUpper = "BAT" Then
                    'bat provincia -> bari
                    Sigla_Pro = "BA"
                End If
                If Sigla_Pro.ToUpper = "0T" Then
                    Sigla_Pro = "OT"
                End If

                'Inizializzo i codici istat (come non rilevati)
                Pro_Cod_Istat = "000"
                Com_Cod_Istat = "000"
                Frz_Des = ""
                Com_Des = ""
                If Cap Is Nothing Then
                    Cap = ""
                ElseIf Cap.Length <> 5 Then
                    Cap = ""
                End If

                DTProvincia = ObjProvIstat.Leggi(Sigla_Pro,
                                                    "",
                                                    enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                    "", "",
                                                    objParametri)
            End If 'controllo sigla prov

            If Not IsNothing(DTProvincia) AndAlso DTProvincia.Rows.Count > 0 Then

                'Rilevato il codice istat della Provincia
                Pro_Cod_Istat = CStr(DTProvincia.Rows(0).Item("Prov"))

                'Comune
                'Nota: Coltiva Utilizza la stringa Localita per definire sia la frazione che il comune.
                'Utilizzo un analizzatore sintattico per intuire i 2 valori

                DTComune = ObjIstat.Leggi(Pro_Cod_Istat,
                                            "",
                                            "",
                                            "",
                                            "",
                                             enumSelezioneVariabile.Selezione_TabellaCompleta,
                                              "", "",
                                              objParametri)

                If Not IsNothing(DTComune) AndAlso DTComune.Rows.Count > 0 Then

                    Dim str_Comune_Input_flt = Trim(UCase(Local_Coltiva.Replace("'", "").Replace("‘", "").Replace("’", "")))
                    Dim index_ComuneTrovatoMatchEsatto = -1

                    ReDim Comuni(3, 0)
                    i = 1

                    For z = 0 To DTComune.Rows.Count - 1

                        ReDim Preserve Comuni(3, i)
                        Comuni(0, i) = CStr(DTComune.Rows(z).Item("Com"))
                        Comuni(1, i) = UCase(Replace(Replace(Replace(DTComune.Rows(z).Item("Localita"), ".", ". "), "'", " "), "`", " "))
                        Comuni(2, i) = True 'Comune che supera la condizione di split
                        Comuni(3, i) = CStr(DTComune.Rows(z).Item("cap"))

                        '18/01/2024 Giulia: ci sono casi particolari dove il comune contiene uno spazio,
                        ' quindi paradossalmente anche se mi arriva il comune esattamente come scritto nella nostra tabella
                        ' con questo gioco dello split non riesce a sentire il match del comune.
                        'Prima di fare il controllo con lo split allora provo a vedere se è esattamente uguale
                        'e faccio vincere questa condizione rispetto al controllo tramite lo split

                        Dim str_Comune_Istat_flt = Trim(UCase(DTComune.Rows(z).Item("Localita").Replace("'", "").Replace("‘", "").Replace("’", "")))

                        If str_Comune_Input_flt = str_Comune_Istat_flt Then
                            ComuneTrovatoMatchEsatto = True
                            index_ComuneTrovatoMatchEsatto = i
                        End If

                        i = i + 1

                    Next

                    SplitStep = 0
                    Trovato = False
                    'Definisco lo Split
                    'Sostituisco preventivamente i punti e gli apostrofi con spazi.. questo per evitare situazioni
                    'anomale come S.ARCANGELO

                    Array_Split = Split(Trim(UCase(Replace(Replace(Local_Coltiva, ".", ". "), "'", " "))), " ")


                    Do While Not Trovato
                        No_Candidati = 0

                        i = 1

                        If ComuneTrovatoMatchEsatto Then

                            'Bypass di tutta la gestione con lo split, visto che ho già appurato che il comune
                            'che mi è arrivato è esattamente uguale a quello del metaschema
                            i = index_ComuneTrovatoMatchEsatto
                            No_Candidati = 1
                            Comune_Candidato = i
                            Trovato = True

                        Else

                            Do While i <= UBound(Comuni, 2) AndAlso Not Trovato

                                If Comuni(2, i) = True Then
                                    'Comune Candidato
                                    Condizione = True

                                    For j = 0 To SplitStep

                                        If InStr(1, Trim(UCase(Comuni(1, i))), Trim(UCase(Array_Split(UBound(Array_Split, 1) - j)))) = False Then
                                            Condizione = False
                                            Exit For
                                        ElseIf Len(Trim(Comuni(1, i))) = Len(Trim(Array_Split(UBound(Array_Split, 1) - j))) Then
                                            'Le descrizioni dei comuni sono identiche -> ricerca finita
                                            Trovato = True 'Imposto il criterio di arresto
                                            Exit For
                                        End If

                                    Next j

                                    If Not Condizione Then
                                        'Il non comune supera la condizione di split -> lo escludo dai candidati
                                        Comuni(2, i) = False
                                        Ultimo_Comune_Escluso = i 'Salvo l'ultimo comune escluso
                                    Else
                                        No_Candidati = No_Candidati + 1
                                        Comune_Candidato = i
                                    End If

                                End If

                                i = i + 1

                            Loop 'ciclo sull'elenco dei comuni della provincia, finché non viene trovato

                        End If



                        'Alla fine della selezione controlli se ho trovato un identificatore univoco
                        Select Case No_Candidati

                            'modifica del 14/12/2015: se non viene trovato, non considero più l'ultimo comune (non ha senso)
                            'imposto 000 "comune non definito"
                            'Case 0, 1
                            Case 0

                                '0 -> La ricerca non ha portato conclusioni esatte
                                '     Considero l'ultimo Comune Escluso.

                                Trovato = True 'per uscire dal ciclo
                                Com_Cod_Istat = "000" 'comune non definito

                                'Cap = "00000"
                                'Frz_Des = ""
                                'Com_Cod_Istat = Comuni(0, IIf(No_Candidati = 0, Ultimo_Comune_Escluso, Comune_Candidato))
                                'Com_Des = Comuni(1, IIf(No_Candidati = 0, Ultimo_Comune_Escluso, Comune_Candidato))
                                'If Cap = "" Then
                                '    Cap = Comuni(3, IIf(No_Candidati = 0, Ultimo_Comune_Escluso, Comune_Candidato))
                                'End If
                                ''Costruisco la stringa Frazione

                                'For j = 0 To UBound(Array_Split, 1) - (SplitStep + No_Candidati)
                                '    Frz_Des = Frz_Des & Array_Split(j) & " "
                                'Next j

                                'If Frz_Des = "" Then
                                '    'Se non è stata rilevata alcuna descrizione della frazione
                                '    '-> la frazione è il comune
                                '    Frz_Des = Com_Des
                                'End If


                            Case 1

                                '1 -> Identificato Comune

                                Trovato = True
                                Com_Cod_Istat = Comuni(0, If(No_Candidati = 0, Ultimo_Comune_Escluso, Comune_Candidato))
                                Com_Des = Comuni(1, If(No_Candidati = 0, Ultimo_Comune_Escluso, Comune_Candidato))
                                If Cap = "" Then
                                    Cap = Comuni(3, If(No_Candidati = 0, Ultimo_Comune_Escluso, Comune_Candidato))
                                End If


                                'Costruisco la stringa Frazione
                                If Not ComuneTrovatoMatchEsatto Then
                                    For j = 0 To UBound(Array_Split, 1) - (SplitStep + No_Candidati)
                                        Frz_Des = Frz_Des & Array_Split(j) & " "
                                    Next j
                                End If

                                If Frz_Des = "" Then
                                    'Se non è stata rilevata alcuna descrizione della frazione
                                    '-> la frazione è il comune
                                    Frz_Des = Com_Des
                                End If

                            Case Else
                                'Esistono + candidati -> continuo la ricerca
                                SplitStep = SplitStep + 1
                                If SplitStep > UBound(Array_Split, 1) Then
                                    'Parametri di ricerca saturi... non è possibile avviare una ricerca approfondita
                                    Com_Cod_Istat = Comuni(0, Comune_Candidato)
                                    Com_Des = Comuni(1, Comune_Candidato)
                                    Frz_Des = Com_Des
                                    Trovato = True
                                End If
                        End Select

                    Loop 'finché non viene trovato

                End If

            Else

                ''Sigla Provincia Non Rilevata!
                'Trovato = False

                'If Not IsNothing(mSiglePro) Then
                '    'Evidenzio l'errore solo se è la prima volta che la provincia non viene mappata
                '    For i = 0 To UBound(mSiglePro, 1)

                '        If UCase(mSiglePro(i)) = UCase(Sigla_Pro) Then
                '            Trovato = True
                '            Exit For
                '        End If

                '    Next i
                'End If

                'If Not Trovato Then
                '    'Gestione errore previsto
                '    'mErrorDescription = "Sigla provincia " & Sigla_Pro & " non importabile negli archivi Gias."
                '    'Inserisco la provincia nell'elenco di quelle non mappabili
                '    ReDim Preserve mSiglePro(UBound(mSiglePro, 1) + 1)
                '    mSiglePro(UBound(mSiglePro, 1)) = Sigla_Pro
                'End If
                Dim DT As DataTable

                If Cap <> "" Then

                    'cerco dal cap
                    DT = ObjIstat.Leggi("", "", "", Cap, "", enumSelezioneVariabile.Selezione_TabellaCompleta,
                                        "", "", objParametri)

                    If DT.Rows.Count > 0 Then
                        Pro_Cod_Istat = DT.Rows(0).Item("PROV")
                        Com_Cod_Istat = DT.Rows(0).Item("COM")
                        Sigla_Pro = If(IsDBNull(DT.Rows(0).Item("Comuni_Prov")), "", DT.Rows(0).Item("Comuni_Prov"))
                    End If

                Else
                    'cap vuoto, cerco dalla localita

                    'cerco dal cap
                    DT = ObjIstat.Leggi("", "", "", "", "",
                                        enumSelezioneVariabile.Selezione_TabellaCompleta,
                                        " Localita = '" & Agro_SQL_SaveText(Local_Coltiva) & "' ",
                                        "", objParametri)

                    If DT.Rows.Count > 0 Then
                        Pro_Cod_Istat = DT.Rows(0).Item("PROV")
                        Com_Cod_Istat = DT.Rows(0).Item("COM")
                        Sigla_Pro = If(IsDBNull(DT.Rows(0).Item("Comuni_Prov")), "", DT.Rows(0).Item("Comuni_Prov"))
                    End If

                End If

            End If 'sigla provincia vuota

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

    End Sub


    '################################################################################
    Public Sub CodiciISTAT_from_DatiDaImportare_ImportFRG(ByVal Sigla_Pro As String, _
                                                        ByVal Provincia As String, _
                                                        ByVal Localita As String, _
                                                        ByVal Cap As String, _
                                                        ByRef Pro_Cod_Istat As String, _
                                                        ByRef Com_Cod_Istat As String, _
                                                        ByRef CAP_Istat As String, _
                                                         ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                                        )

        Dim Dt_ISTAT As DataTable
        Dim i As Integer

        Pro_Cod_Istat = "000"
        Com_Cod_Istat = "000"
        CAP_Istat = "00000"

        'i conferenti hanno tutti sigla provincia e cap valorizzato

        '5 cooperative senza sigla e dati
        '1 con sigla ma senza cap


        If Sigla_Pro <> "" Then
            'sigla provincia valorizzata

            Dt_ISTAT = ISTATXListaProvince("", "", "", Sigla_Pro, "", "", "", "", "", "", objParametri)

            If Not IsNothing(Dt_ISTAT) AndAlso Dt_ISTAT.Rows.Count <> 0 Then

                Pro_Cod_Istat = Dt_ISTAT.Rows(0).Item("PROV")

                For i = 0 To Dt_ISTAT.Rows.Count - 1

                    If Cap <> "" Then

                        'cap valorizzato

                        'cerco per cap
                        If CStr(Dt_ISTAT.Rows(i).Item("CAP")) = Cap Then

                            Com_Cod_Istat = Dt_ISTAT.Rows(i).Item("COM_LOCALITA")
                            CAP_Istat = Dt_ISTAT.Rows(i).Item("CAP")
                            Exit For

                        End If

                        'intanto che ci sono cerco anche per località
                        If CStr(Dt_ISTAT.Rows(i).Item("LOCALITA")).ToLower = Localita.ToLower Then

                            Com_Cod_Istat = Dt_ISTAT.Rows(i).Item("COM_LOCALITA")
                            CAP_Istat = Dt_ISTAT.Rows(i).Item("CAP")
                            Exit For

                        End If


                    Else
                        'cap non valorizzato

                        'cerco per località
                        If CStr(Dt_ISTAT.Rows(i).Item("LOCALITA")).ToLower = Localita.ToLower Then

                            Com_Cod_Istat = Dt_ISTAT.Rows(i).Item("COM_LOCALITA")
                            CAP_Istat = Dt_ISTAT.Rows(i).Item("CAP")
                            Exit For

                        End If

                    End If

                Next

                If Com_Cod_Istat = "000" Then
                    Com_Cod_Istat = Dt_ISTAT.Rows(0).Item("COM_PROVINCIA")
                    CAP_Istat = Dt_ISTAT.Rows(0).Item("CAP")
                End If

            End If

        Else
            'sigla provincia non valorizzata

            If Cap <> "" Then

                'cerco dal cap
                Dt_ISTAT = ISTATXListaProvince("", "", "", "", Cap, "", "", "", "", "", objParametri)

                If Not IsNothing(Dt_ISTAT) AndAlso Dt_ISTAT.Rows.Count <> 0 Then

                    Pro_Cod_Istat = Dt_ISTAT.Rows(0).Item("PROV")
                    Com_Cod_Istat = Dt_ISTAT.Rows(0).Item("COM_LOCALITA")
                    CAP_Istat = Dt_ISTAT.Rows(0).Item("CAP")

                End If

            End If

        End If



    End Sub


    '################################################################################
    Public Function CAP_form_PROV_COM(ByRef PROV_Istat As String, _
                                 ByRef Com_Istat As String, _
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                                        ) As String
        Dim Dt As DataTable
        Dim i As Integer
        Dim ObjIstat As New AgronicaCoreMetaSchemaDAL.Istat_R

        Dt = ObjIstat.Leggi(PROV_Istat, Com_Istat, "", "", "", enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri)

        If Dt.Rows.Count = 1 Then
            Return Dt.Rows(0).Item("CAP")
        Else
            Return ""
        End If

    End Function




    '################################################################################
    'Data la sigla della provincia ed una stringa che potrebbe contenere il comune, 
    'cerco tutti i comuni della provincia e li confronto con la stringa,
    'se trovo il comune ok, altrimenti gli assegno quello del capoluogo di provincia
    Public Sub CodIstat_from_SiglaProvincia_and_StringaComune(ByVal Sigla_Pro As String,
                                                            ByVal str_con_Comune As String,
                                                            ByRef Comune As String,
                                                            ByRef Pro_Cod_Istat As String,
                                                            ByRef Com_Cod_Istat As String,
                                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                            Optional ByRef ComuneTrovato As Boolean = False)


        Dim DtProvincia As DataTable

        Dim ObjLista_Province As New AgronicaCoreMetaSchemaDAL.Lista_Province_R

        Dim DtComune As DataTable

        Dim Comune_Tmp As String
        Dim str_con_Comune_flt As String
        Dim Comune_Tmp_flt As String

        Dim i As Integer

        DtProvincia = ObjLista_Province.Leggi(CStr(Sigla_Pro), "",
                                              enumSelezioneVariabile.Selezione_TabellaCompleta,
                                              "", "", objParametri)

        If Not DtProvincia Is Nothing AndAlso DtProvincia.Rows.Count > 0 Then

            For Each provincia In DtProvincia.Rows
                Pro_Cod_Istat = CStr(provincia.Item("Prov"))
                Com_Cod_Istat = CStr(provincia.Item("Com"))

                'inizialmente assegno come comune il capoluogo
                DtComune = Leggi(CStr(Pro_Cod_Istat), CStr(Com_Cod_Istat),
                                 "", "", "",
                                 enumSelezioneVariabile.Selezione_TabellaCompleta,
                                 "", "", objParametri)

                If Not DtComune Is Nothing AndAlso DtComune.Rows.Count > 0 Then
                    Comune = CStr(DtComune.Rows(0).Item("Localita"))
                End If

                DtComune = Leggi(CStr(Pro_Cod_Istat), "", "", "", "",
                                 enumSelezioneVariabile.Selezione_TabellaCompleta,
                                 "", "", objParametri)


                If Not DtComune Is Nothing AndAlso DtComune.Rows.Count > 0 Then

                    For i = 0 To DtComune.Rows.Count - 1

                        Comune_Tmp = DtComune.Rows(i).Item("Localita")
                        str_con_Comune_flt = str_con_Comune.Replace("'", "").Replace("‘", "").Replace("’", "")
                        Comune_Tmp_flt = Comune_Tmp.Replace("'", "").Replace("‘", "").Replace("’", "").Replace("`", "")
                        'If InStr(LCase(str_con_Comune), LCase(Comune_Tmp)) <> 0 Then

                        '    Com_Cod_Istat = CStr(DtComune.Rows(i).Item("Com"))
                        '    Comune = Comune_Tmp

                        '    ComuneTrovato = True
                        '    Exit For

                        'End If

                        If str_con_Comune_flt = Comune_Tmp_flt Or str_con_Comune_flt = Comune_Tmp_flt.ToUpper Or str_con_Comune_flt.ToUpper = Comune_Tmp_flt Then

                            Com_Cod_Istat = CStr(DtComune.Rows(i).Item("Com"))
                            Comune = Comune_Tmp

                            ComuneTrovato = True
                            Exit For

                        End If

                    Next

                End If

                DtComune = Nothing

                If ComuneTrovato Then
                    Exit For
                End If

            Next

        End If

        ObjLista_Province = Nothing
        DtProvincia = Nothing
        DtComune = Nothing

    End Sub

    '################################################################################
    'Data la sigla della provincia ed una stringa che potrebbe contenere il comune, 
    'cerco tutti i comuni della provincia e li confronto con la stringa,
    'se trovo il comune ok, altrimenti gli assegno "000" (differenza rispetto alla prima funzione)
    Public Sub CodIstat_from_SiglaProvincia_and_StringaComune_2(ByVal Sigla_Pro As String,
                                                            ByVal str_con_Comune As String,
                                                            ByRef Comune As String,
                                                            ByRef Pro_Cod_Istat As String,
                                                            ByRef Com_Cod_Istat As String,
                                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                            Optional ByRef ComuneTrovato As Boolean = False)


        Dim DtProvincia As DataTable

        Dim ObjLista_Province As New AgronicaCoreMetaSchemaDAL.Lista_Province_R

        Dim DtComune As DataTable

        Dim Comune_Tmp As String
        Dim str_con_Comune_flt As String
        Dim Comune_Tmp_flt As String
        Dim i As Integer

        ComuneTrovato = False

        DtProvincia = ObjLista_Province.Leggi(CStr(Sigla_Pro), "",
                                              enumSelezioneVariabile.Selezione_TabellaCompleta,
                                              "", "", objParametri)

        If Not DtProvincia Is Nothing AndAlso DtProvincia.Rows.Count > 0 Then

            For Each provincia In DtProvincia.Rows
                Pro_Cod_Istat = CStr(provincia.Item("Prov"))
                Com_Cod_Istat = "000"
                'Com_Cod_Istat = CStr(provincia.Item("Com"))
                ''inizialmente assegno come comune il capoluogo
                'DtComune = Leggi(CStr(Pro_Cod_Istat), CStr(Com_Cod_Istat),
                '                 "", "", "",
                '                 enumSelezioneVariabile.Selezione_TabellaCompleta,
                '                 "", "", objParametri)

                'If Not DtComune Is Nothing AndAlso DtComune.Rows.Count > 0 Then
                '    Comune = CStr(DtComune.Rows(0).Item("Localita"))
                'End If

                DtComune = Leggi(CStr(Pro_Cod_Istat), "", "", "", "",
                                 enumSelezioneVariabile.Selezione_TabellaCompleta,
                                 "", "", objParametri)


                If Not DtComune Is Nothing AndAlso DtComune.Rows.Count > 0 Then

                    For i = 0 To DtComune.Rows.Count - 1

                        Comune_Tmp = DtComune.Rows(i).Item("Localita")
                        str_con_Comune_flt = str_con_Comune.Replace("'", "").Replace("‘", "").Replace("’", "")
                        Comune_Tmp_flt = Comune_Tmp.Replace("'", "").Replace("‘", "").Replace("’", "")
                        'If InStr(LCase(str_con_Comune), LCase(Comune_Tmp)) <> 0 Then

                        '    Com_Cod_Istat = CStr(DtComune.Rows(i).Item("Com"))
                        '    Comune = Comune_Tmp

                        '    ComuneTrovato = True
                        '    Exit For

                        'End If

                        If str_con_Comune_flt = Comune_Tmp_flt Then

                            Com_Cod_Istat = CStr(DtComune.Rows(i).Item("Com"))
                            Comune = Comune_Tmp

                            ComuneTrovato = True
                            Exit For

                        End If

                    Next

                End If

                DtComune = Nothing

                If ComuneTrovato Then
                    Exit For
                End If

            Next

        End If

        ObjLista_Province = Nothing
        DtProvincia = Nothing
        DtComune = Nothing

    End Sub


    '################################################################################
    'Data la sigla della provincia ed una stringa che potrebbe contenere il comune, 
    'cerco tutti i comuni della provincia e li confronto con la stringa,
    'se trovo il comune ok, altrimenti cerco per cap,
    'se ancora non lo trovo gli assegno "000" (differenza rispetto alla prima funzione)
    Public Sub CodIstat_from_SiglaProvincia_and_StringaComune_or_CAP(ByVal Sigla_Pro As String,
                                                                    ByVal str_con_Comune As String,
                                                                    ByVal CAP As String,
                                                                    ByRef Comune As String,
                                                                    ByRef Pro_Cod_Istat As String,
                                                                    ByRef Com_Cod_Istat As String,
                                                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                                    Optional ByRef ComuneTrovato As Boolean = False)


        Dim DtProvincia As DataTable

        Dim ObjLista_Province As New AgronicaCoreMetaSchemaDAL.Lista_Province_R

        Dim DtComune As DataTable

        Dim Comune_Tmp As String
        Dim str_con_Comune_flt As String
        Dim Comune_Tmp_flt As String
        Dim i As Integer

        If Sigla_Pro.ToUpper = "FO" Then
            'forlì -> forlì-cesena
            Sigla_Pro = "FC"
        End If
        If Sigla_Pro.ToUpper = "PS" Then
            'pesaro -> pesaro-urbino
            Sigla_Pro = "PU"
        End If
        If Sigla_Pro.ToUpper = "BAT" Then
            'bat provincia -> bari
            Sigla_Pro = "BA"
        End If
        If Sigla_Pro.ToUpper = "0T" Then
            Sigla_Pro = "OT"
        End If

        Select Case CAP
            Case "47122", "47100" 'FORLI
                'su tabella ISTAT di GIAS è salvato 47121
                CAP = "47121"
            Case "47522" 'CESENA
                CAP = "47521"
            Case "47921", "47922", "47923", "47924" 'RIMINI
                CAP = "47037"
        End Select


        ComuneTrovato = False

        DtProvincia = ObjLista_Province.Leggi(CStr(Sigla_Pro), "",
                                              enumSelezioneVariabile.Selezione_TabellaCompleta,
                                              "", "", objParametri)

        If Not DtProvincia Is Nothing AndAlso DtProvincia.Rows.Count > 0 Then

            For Each provincia In DtProvincia.Rows
                Pro_Cod_Istat = CStr(provincia.Item("Prov"))
                Com_Cod_Istat = "000"
                'Com_Cod_Istat = CStr(provincia.Item("Com"))
                ''inizialmente assegno come comune il capoluogo
                'DtComune = Leggi(CStr(Pro_Cod_Istat), CStr(Com_Cod_Istat),
                '                 "", "", "",
                '                 enumSelezioneVariabile.Selezione_TabellaCompleta,
                '                 "", "", objParametri)

                'If Not DtComune Is Nothing AndAlso DtComune.Rows.Count > 0 Then
                '    Comune = CStr(DtComune.Rows(0).Item("Localita"))
                'End If

                DtComune = Leggi(CStr(Pro_Cod_Istat), "", "", "", "",
                                 enumSelezioneVariabile.Selezione_TabellaCompleta,
                                 "", "", objParametri)


                If Not DtComune Is Nothing AndAlso DtComune.Rows.Count > 0 Then

                    For i = 0 To DtComune.Rows.Count - 1

                        Comune_Tmp = DtComune.Rows(i).Item("Localita")
                        str_con_Comune_flt = str_con_Comune.Replace("'", "").Replace("‘", "").Replace("’", "")
                        Comune_Tmp_flt = Comune_Tmp.Replace("'", "").Replace("‘", "").Replace("’", "")
                        'If InStr(LCase(str_con_Comune), LCase(Comune_Tmp)) <> 0 Then

                        '    Com_Cod_Istat = CStr(DtComune.Rows(i).Item("Com"))
                        '    Comune = Comune_Tmp

                        '    ComuneTrovato = True
                        '    Exit For

                        'End If

                        If str_con_Comune_flt = Comune_Tmp_flt Then

                            Com_Cod_Istat = CStr(DtComune.Rows(i).Item("Com"))
                            Comune = Comune_Tmp

                            ComuneTrovato = True
                            Exit For

                        End If

                    Next

                End If

                DtComune = Nothing

                If ComuneTrovato = True Then
                    Exit For

                Else
                    'cerco per CAP
                    DtComune = Leggi(CStr(Pro_Cod_Istat), "", "", CAP, "",
                              enumSelezioneVariabile.Selezione_TabellaCompleta,
                              "", "", objParametri)

                    If Not DtComune Is Nothing AndAlso DtComune.Rows.Count > 0 Then
                        Com_Cod_Istat = CStr(DtComune.Rows(0).Item("Com"))
                        Comune = DtComune.Rows(0).Item("Localita")
                        ComuneTrovato = True
                    End If
                End If

            Next 'provincie

        End If

        ObjLista_Province = Nothing
        DtProvincia = Nothing
        DtComune = Nothing

    End Sub

    Public Sub CodIstat_from_CodCatastale(ByRef CodProv As String, ByRef CodCom As String, ByRef CodiceCatastale As String, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri)

        Dim DT = Leggi("", "", "", "", "", enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri, CodiceCatastale)

        If DT.Rows.Count > 0 Then

            CodProv = DT.Rows(0)("PROV")
            CodCom = DT.Rows(0)("COM")

        Else

            CodProv = ""
            CodCom = ""

        End If

    End Sub

    Public Sub CodIstat_from_Cod_Nazionale(ByRef CodProv As String,
                                           ByRef CodCom As String,
                                           ByRef Prov As String,
                                           ByRef Com As String,
                                           ByRef Sez As String,
                                           ByRef Cap As String,
                                           ByRef Nazione As String,
                                           CodiceCatastale As String,
                                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri)
        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.Istat_R.CodIstat_from_Cod_Nazionale()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        StrSQL.Length = 0
        StrSQL.Append(" SELECT ISTAT_Cod_Nazionale_Sezione.Cod_Nazionale, Sezione_Censuaria = COALESCE(ISTAT_Cod_Nazionale_Sezione.Sezione_Censuaria, ''), ")
        StrSQL.Append(" ISTAT.* ")
        StrSQL.Append(" FROM ISTAT_Cod_Nazionale_Sezione ")
        StrSQL.Append(" JOIN ISTAT ON ISTAT_Cod_Nazionale_Sezione.Cod_Belfiore = ISTAT.CodiceCatastale ")
        If CodiceCatastale <> "" Then
            StrSQL.Append(" WHERE Cod_Nazionale = " & Agro_SQL_SaveText_NULL(CodiceCatastale) & " ")
        End If

        '--------------------------------------------------------------------------
        DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
        '--------------------------------------------------------------------------

        If DT.Rows.Count > 0 Then

            CodProv = DT.Rows(0)("PROV")
            CodCom = DT.Rows(0)("COM")
            Prov = DT.Rows(0)("COMUNI_PROV")
            Com = DT.Rows(0)("LOCALITA")
            Sez = DT.Rows(0)("Sezione_Censuaria")
            Cap = DT.Rows(0)("CAP")
            Nazione = DT.Rows(0)("Stato_Country")
        Else

            CodProv = ""
            CodCom = ""
            Prov = ""
            Com = ""
            Sez = ""
            Cap = ""
            Nazione = ""
        End If

    End Sub

End Class





