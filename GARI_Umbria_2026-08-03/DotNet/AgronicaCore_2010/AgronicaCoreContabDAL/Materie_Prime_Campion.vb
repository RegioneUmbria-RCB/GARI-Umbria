Imports System.Text
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate

Public Class Materie_Prime_Campionature_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '#######################################################################
    Public Function Leggi(ByVal Progressivo As Integer,
                          ByVal Tipo As String,
                          ByVal Tipo_Cod As Integer,
                          ByVal Udm_Cod As Integer,
                          ByVal Progressivo_Origine As Integer,
                          ByVal Piva_SuperUser_Origine As String,
                          ByVal Flag_AncheImportatati As Boolean,
                          ByVal xSelezioneVariabile As enumSelezioneVariabile,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreParametri
                          ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Materie_prime_Campion_R.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   Progressivo = 0  
        '   Tipo = "" 
        '   Tipo_Cod = 0    
        '   Udm_Cod = 0
        '   Progressivo_Origine = 0
        '   Piva_SuperUser_Origine = ""
        '   Flag_AncheImportatati = True 
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi
                    '------------------------------------------------------------------
                    strSql.Length = 0
                    strSql.AppendLine("")
                    strSql.AppendLine(" SELECT Materie_Prime_Campionature.* ")
                    strSql.AppendLine(" FROM  Materie_Prime_Campionature WITH(NOLOCK)")
                    strSql.AppendLine(" WHERE Materie_Prime_Campionature.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & "  ")
                    strSql.AppendLine(" AND   Materie_Prime_Campionature.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & "  ")

                    If Progressivo <> 0 Then
                        strSql.AppendLine(" AND Materie_Prime_Campionature.Progressivo = " & Agro_SQL_SaveNum(Progressivo) & "   ")
                    End If

                    If Tipo <> "" Then
                        strSql.AppendLine(" AND Materie_Prime_Campionature.Tipo = '" & Agro_SQL_SaveText(Tipo) & "'   ")
                    End If

                    If Tipo_Cod <> 0 Then
                        strSql.AppendLine(" AND Materie_Prime_Campionature.Tipo_Cod = " & Agro_SQL_SaveNum(Tipo_Cod) & "   ")
                    End If

                    If Udm_Cod <> 0 Then
                        strSql.AppendLine(" AND Materie_Prime_Campionature.Udm_Cod = " & Agro_SQL_SaveNum(Udm_Cod) & "   ")
                    End If

                    If Progressivo_Origine <> 0 Then
                        strSql.AppendLine(" AND  Materie_Prime_Campionature.Progressivo_Origine = " & Agro_SQL_SaveNum(Progressivo_Origine) & "   ")
                    End If

                    If Piva_SuperUser_Origine <> "" Then
                        strSql.AppendLine(" AND  Materie_Prime_Campionature.Piva_SuperUser_Origine = '" & Agro_SQL_SaveText(Piva_SuperUser_Origine) & "'")
                    End If

                    If Not Flag_AncheImportatati Then
                        strSql.AppendLine(" AND  Materie_Prime_Campionature.Progressivo_Origine = " & Agro_SQL_SaveNum(0) & " ")
                    End If


                    If xFiltroAggiuntivo <> "" Then
                        strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            strSql.AppendLine(" AND   Materie_Prime_Campionature.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            strSql.AppendLine(" AND   Materie_Prime_Campionature.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        strSql.AppendLine(" ORDER BY Materie_Prime_Campionature.Progressivo ASC, Materie_Prime_Campionature.Tipo ASC ")
                    End If

                Case enumSelezioneVariabile.Selezione_TabellaCompleta
                    '------------------------------------------------------------------
                    strSql.Length = 0
                    strSql.AppendLine("")
                    strSql.AppendLine(" SELECT Materie_Prime_Campionature.* ")
                    strSql.AppendLine(" FROM  Materie_Prime_Campionature WITH(NOLOCK)")
                    strSql.AppendLine(" WHERE Materie_Prime_Campionature.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & "  ")
                    strSql.AppendLine(" AND   Materie_Prime_Campionature.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & "  ")

                    If Progressivo <> 0 Then
                        strSql.AppendLine(" AND Materie_Prime_Campionature.Progressivo = " & Agro_SQL_SaveNum(Progressivo) & "   ")
                    End If

                    If Tipo <> "" Then
                        strSql.AppendLine(" AND Materie_Prime_Campionature.Tipo = '" & Agro_SQL_SaveText(Tipo) & "'   ")
                    End If

                    If Tipo_Cod <> 0 Then
                        strSql.AppendLine(" AND Materie_Prime_Campionature.Tipo_Cod = " & Agro_SQL_SaveNum(Tipo_Cod) & "   ")
                    End If

                    If Udm_Cod <> 0 Then
                        strSql.AppendLine(" AND Materie_Prime_Campionature.Udm_Cod = " & Agro_SQL_SaveNum(Udm_Cod) & "   ")
                    End If

                    If Progressivo_Origine <> 0 Then
                        strSql.AppendLine(" AND  Materie_Prime_Campionature.Progressivo_Origine = " & Agro_SQL_SaveNum(Progressivo_Origine) & "   ")
                    End If

                    If Piva_SuperUser_Origine <> "" Then
                        strSql.AppendLine(" AND  Materie_Prime_Campionature.Piva_SuperUser_Origine = '" & Agro_SQL_SaveText(Piva_SuperUser_Origine) & "'")
                    End If

                    If Not Flag_AncheImportatati Then
                        strSql.AppendLine(" AND  Materie_Prime_Campionature.Progressivo_Origine = " & Agro_SQL_SaveNum(0) & " ")
                    End If


                    If xFiltroAggiuntivo <> "" Then
                        strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            strSql.AppendLine(" AND   Materie_Prime_Campionature.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            strSql.AppendLine(" AND   Materie_Prime_Campionature.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        strSql.AppendLine(" ORDER BY Materie_Prime_Campionature.Progressivo ASC, Materie_Prime_Campionature.Tipo ASC ")
                    End If

                Case enumSelezioneVariabile.Selezione_JoinDescrizioni


                Case enumSelezioneVariabile.Selezione_JoinCompleta


            End Select

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function


    '#######################################################################
    Public Function LeggiPQxGHG(ByVal Piva As String,
                                ByVal Id_Agenda_GHG As Integer,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreParametri
                                ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Materie_prime_Campion_R.LeggiPQxGHG()"

        '====================================================================================
        'Parametri opzionali :
        '   Progressivo = 0  
        '   Tipo = "" 
        '   Tipo_Cod = 0    
        '   Udm_Cod = 0
        '   Progressivo_Origine = 0
        '   Piva_SuperUser_Origine = ""
        '   Flag_AncheImportatati = True 
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0
            strSql.AppendLine("")
            strSql.AppendLine(" SELECT Materie_Prime_Campionature.* ")
            strSql.AppendLine(" FROM  Materie_Prime_Campionature, Movimenti_Dettagli, Mov_Dettagli_Riferimenti ")
            strSql.AppendLine(" WHERE Materie_Prime_Campionature.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & "  ")
            strSql.AppendLine(" AND   Materie_Prime_Campionature.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & "  ")
            strSql.AppendLine(" AND   Materie_Prime_Campionature.Progressivo = Movimenti_Dettagli.Cal_Cod ")
            strSql.AppendLine(" AND   Movimenti_Dettagli.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
            strSql.AppendLine(" AND   Movimenti_Dettagli.Piva = Mov_Dettagli_Riferimenti.Piva ")
            strSql.AppendLine(" AND   Movimenti_Dettagli.Id_Agenda = Mov_Dettagli_Riferimenti.Id_Agenda ")
            strSql.AppendLine(" AND   Movimenti_Dettagli.Id_Mov_Det = Mov_Dettagli_Riferimenti.Id_Mov_Det ")
            strSql.AppendLine(" AND   Mov_Dettagli_Riferimenti.Id_Agenda_Rif = " & Agro_SQL_SaveNum(Id_Agenda_GHG) & "   ")
            strSql.AppendLine(" AND   Mov_Dettagli_Riferimenti.Lav_Cod_Rif = " & LAVCOD_GHG & "  ")

            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.AppendLine(" AND   Materie_Prime_Campionature.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.AppendLine(" AND   Materie_Prime_Campionature.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                strSql.AppendLine(" ORDER BY Materie_Prime_Campionature.Progressivo ASC, Materie_Prime_Campionature.Tipo ASC ")
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function


    '################################################################################
    'default:
    'Optional ByVal Tipo As String = "",
    'Optional ByVal Tipo_Cod As Integer = 0, 
    'Optional ByVal Udm_Cod As Integer = 0,
    'Optional ByVal Progressivo_Origine As Integer = 0,
    'Optional ByVal Piva_SuperUser_Origine As String = "",
    'Optional ByVal Flag_AncheImportati As Boolean = False,
    'Optional ByVal FinestraTemp_Inizio As Date = AGRODATAINIZIO,
    'Optional ByVal FinestraTemp_Fine As Date = AGRODATAFINE,
    'Optional ByVal FiltroAggiuntivo As String = "",
    'Optional ByVal Ordinamento As String = ""
    Public Function Leggi_MateriaPrima_Campionature(ByRef Codice_Calibro As Integer,
                                                    ByRef Desc_Calibro As String,
                                                    ByRef Codice_Indice As Integer,
                                                    ByRef Desc_Indice As String,
                                                    ByRef Codice_Danno As Integer,
                                                    ByRef Desc_Danno As String,
                                                    ByRef Valore_Indice As String,
                                                    ByRef UdmCod_Indice As Integer,
                                                    ByVal Progressivo As Integer,
                                                    ByVal Tipo As String,
                                                    ByVal Tipo_Cod As Integer,
                                                    ByVal Udm_Cod As Integer,
                                                    ByVal Progressivo_Origine As Integer,
                                                    ByVal Piva_SuperUser_Origine As String,
                                                    ByVal Flag_AncheImportati As Boolean,
                                                    ByVal xFiltroAggiuntivo As String,
                                                    ByVal xOrderBy As String,
                                                    ByRef objParametri As AgronicaCoreParametri
                                                    ) As String

        Const nomeRoutine = "AgronicaCoreContabDAL.Materie_prime_Campion_R.Leggi_MateriaPrima_Campionature()"

        Dim messaggioErrore As String = ""
        Dim campionaturaDes As String = ""

        Try

            Dim dt As DataTable
            Dim i As Integer

            dt = Leggi(Progressivo,
                       Tipo,
                       Tipo_Cod,
                       Udm_Cod,
                       Progressivo_Origine,
                       Piva_SuperUser_Origine,
                       True,
                       enumSelezioneVariabile.Selezione_TabellaCompleta,
                       "", "",
                       objParametri)

            If Not IsNothing(dt) Then

                For i = 0 To dt.Rows.Count - 1

                    'non va bene qui, va valorizzata solo se il tipo è uno di quelli del case sotto
                    'Campionatura_Des += IIf(Trim(Campionatura_Des) = "", "", ", ") & CStr(Dt.Rows(i).Item("Descrizione"))

                    Select Case CStr(dt.Rows(i).Item("tipo")).ToLower

                        Case "indice"
                            Codice_Indice = dt.Rows(i).Item("tipo_cod")
                            Valore_Indice = dt.Rows(i).Item("val_cod")
                            UdmCod_Indice = dt.Rows(i).Item("udm_cod")
                            Desc_Indice = dt.Rows(i).Item("Descrizione")
                            campionaturaDes += IIf(Trim(campionaturaDes) = "", "", ", ") & CStr(dt.Rows(i).Item("Descrizione"))

                        Case "calibro"
                            Codice_Calibro = dt.Rows(i).Item("tipo_cod")
                            Desc_Calibro = dt.Rows(i).Item("Descrizione")
                            campionaturaDes += IIf(Trim(campionaturaDes) = "", "", ", ") & CStr(dt.Rows(i).Item("Descrizione"))

                        Case "danno"
                            Codice_Danno = dt.Rows(i).Item("tipo_cod")
                            Desc_Danno = dt.Rows(i).Item("Descrizione")
                            campionaturaDes += IIf(Trim(campionaturaDes) = "", "", ", ") & CStr(dt.Rows(i).Item("Descrizione"))

                        Case "analisi"
                            'non devo visualizzare l'analisi nella descrizione
                    End Select

                Next

            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return campionaturaDes

    End Function

    ''' <summary>
    ''' Lettura valori dei quattro parametri qualitativi di ISCC, su un'unica riga per ogni Movimento Dettaglio, assieme ai parametri ISCC dell'impresa fornitore. 
    ''' </summary>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="xOrderBy"></param>
    ''' <param name="objParametriServer"></param>
    ''' <param name="dataIsccRiferimento">Se valido, filtra per tutti i valori inseriti/modificati o relativi a fornitori con parametro Compliance_ISCC aggiornato da tale data</param>
    ''' <returns></returns>
    Public Function Leggi_MateriaPrima_Campionature_ISCC(ByVal paramQualTipoName_ISCCCurrVal As String,
                                                         ByVal paramQualTipoName_ISCCPrecVal As String,
                                                         ByVal paramQualTipoName_ISCCCurrData As String,
                                                         ByVal paramQualTipoName_ISCCPrecData As String,
                                                         ByVal xFiltroAggiuntivo As String,
                                                         ByVal xOrderBy As String,
                                                         ByRef objParametriServer As AgronicaCoreParametri,
                                                         Optional dataIsccRiferimento As Date = Nothing
                                                         ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Materie_prime_Campion_R.Leggi_MateriaPrima_Campionatura_Compliant"

        'Dim paramQualTipoName_ISCCCurrVal As String = "oiscccorrente"
        'Dim paramQualTipoName_ISCCPrecVal As String = "oisccprecedente"
        'Dim paramQualTipoName_ISCCCurrData As String = "oiscccorrentedate"
        'Dim paramQualTipoName_ISCCPrecData As String = "oisccprecedentedate"

        Dim arrLavCodAccettazioniConf As Integer() = New Integer() {LAVCOD_ACCETTAZIONE_DIVERSI, LAVCOD_DISTINTA_CARICO_ACCETTAZIONE, LAVCOD_AUTO_DDT_EMESSO_ACCETTAZIONE} 'LAVCOD_DISTINTA_CARICO, LAVCOD_AUTO_DDT_EMESSO Non usate per i conferimenti

        Dim DT As New DataTable

        Try

            Dim stb As New Text.StringBuilder

            stb.AppendLine(" Select ")
            stb.AppendLine("    camp1.*, ")
            stb.AppendLine("    camp1.Tipo_Cod as ISCCCurr_TipoCod, ")
            stb.AppendLine("    camp2.Tipo_Cod as ISCCPrec_TipoCod, ")
            stb.AppendLine("    camp3.Val_Cod as ISCCCurrData_ValCod, ")
            stb.AppendLine("    camp4.Val_Cod as ISCCPrecData_ValCod, ")
            stb.AppendLine("    Imprese.Compliance_ISCC as FornitoreImpresa_Compliance_ISCC, ")
            stb.AppendLine("    Imprese.Compliance_ISCC as FornitoreImpresa_Compliance_ISCC_DataRiferimento ")

            stb.AppendLine(" From Agenda ")
            stb.AppendLine(" inner Join Movimenti on Agenda.Id_Agenda = Movimenti.Id_Agenda ")
            stb.AppendLine(" inner Join Movimenti_Dettagli on Movimenti.Id_Agenda = Movimenti_Dettagli.Id_Agenda And Movimenti.Id_Mov = Movimenti_Dettagli.Id_Mov ")
            stb.AppendLine(" inner Join Risorse_Umane on Risorse_Umane.cod_risum = movimenti.cod_risum ")
            stb.AppendLine(" inner Join Imprese on Imprese.PIVA = Risorse_Umane.cod_contatto ")
            stb.AppendLine(" inner Join Materie_Prime_Campionature camp1 on camp1.Progressivo = Movimenti_dettagli.cal_cod And camp1.Tipo = '" & Agro_SQL_SaveText(paramQualTipoName_ISCCCurrVal) & "' ")
            stb.AppendLine(" left Join Materie_Prime_Campionature camp2 on camp2.Progressivo = camp1.Progressivo And camp2.Tipo = '" & Agro_SQL_SaveText(paramQualTipoName_ISCCPrecVal) & "' ")
            stb.AppendLine(" left Join Materie_Prime_Campionature camp3 on camp3.Progressivo = camp1.Progressivo And camp3.Tipo = '" & Agro_SQL_SaveText(paramQualTipoName_ISCCCurrData) & "' ")
            stb.AppendLine(" left Join Materie_Prime_Campionature camp4 on camp4.Progressivo = camp1.Progressivo And camp4.Tipo = '" & Agro_SQL_SaveText(paramQualTipoName_ISCCPrecData) & "' ")

            stb.AppendLine(" Where 1 = 1 ")
            stb.AppendLine(" And Agenda.Lav_Cod IN (" & String.Join(", ", arrLavCodAccettazioniConf) & ") ")
            stb.AppendLine(" And Movimenti.Cau_Mov = '" & Agro_SQL_SaveText(CAU_CARICO) & "' ")
            stb.AppendLine(" And Movimenti_Dettagli.Elem_Cod = " & Agro_SQL_SaveNum(TRASFORMATI_VEGETALI) & " ")
            stb.AppendLine(" And Imprese.Compliance_ISCC Is Not null ")

            If Not IsNothing(dataIsccRiferimento) AndAlso dataIsccRiferimento <> Date.MinValue Then
                stb.Append(" And ( ")
                stb.AppendLine("     Imprese.Compliance_ISCC_DataRiferimento >= " & Agro_SQL_SaveDate(dataIsccRiferimento) & " ")
                stb.AppendLine("     OR Movimenti_Dettagli.Data_Modifica >= " & Agro_SQL_SaveDate(dataIsccRiferimento) & " ")
                stb.AppendLine(" ) ")
            End If

            If Not String.IsNullOrEmpty(xFiltroAggiuntivo) Then
                stb.AppendLine(" And " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametriServer))
            End If

            If Not String.IsNullOrEmpty(xOrderBy) Then
                stb.AppendLine(" Order By " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametriServer))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametriServer, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            Scrivi_LOG(objParametriServer, nomeRoutine, ex.Message)

            DT = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)

        End Try

        Return DT

    End Function

End Class


'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################


Public Class Materie_Prime_Campion_W
    Inherits AgronicaCoreDataProvider.DataProvider

    '#################################################################
    Public Function Scrivi(ByVal Progressivo As Integer,
                           ByVal Tipo As String,
                           ByVal Tipo_Cod As Integer,
                           ByVal Udm_Cod As Integer,
                           ByVal Val_Cod As String,
                           ByVal Descrizione As String,
                           ByVal Peso_Campione As Decimal,
                           ByVal Progressivo_Origine As Integer,
                           ByVal Piva_SuperUser_Origine As String,
                           ByVal Validita_Inizio As Date,
                           ByVal Validita_Fine As Date,
                           ByRef objParametri As AgronicaCoreParametri,
                           Optional ByVal ChkStima As Integer = 0,
                           Optional ByVal ChkTara_Campionatura As Integer = 0,
                           Optional ByVal Tara_Campionatura As Decimal = 0,
                           Optional ByVal Data_creazione As DateTime = #2/1/1900#,
                           Optional ByVal Data_modifica As DateTime = #2/1/1900#,
                           Optional ByVal username_creazione As String = "",
                           Optional ByVal username_modifica As String = ""
                           ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.Materie_Prime_Campion_W.Scrivi()"

        '====================================================================================
        'Parametri opzionali :
        '   Progressivo_Origine = 0
        '   Piva_SuperUser_Origine = ""
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            If Data_creazione = #2/1/1900# Then
                Data_creazione = Now
            End If

            If Data_modifica = #2/1/1900# Then
                Data_modifica = Now
            End If

            If username_creazione = "" Then
                username_creazione = objParametri.UsernameOperazione
            End If

            If username_modifica = "" Then
                username_modifica = objParametri.UsernameOperazione
            End If

            '---------------------------------------------
            strSql.Length = 0

            strSql.AppendLine(" INSERT INTO Materie_Prime_Campionature ")
            strSql.AppendLine("         ( ")
            strSql.AppendLine("          Progressivo, Tipo, Tipo_Cod, Udm_Cod, Val_Cod, Descrizione, ")
            strSql.AppendLine("          Progressivo_Origine, Piva_SuperUser_Origine, Peso_Campione, ")
            strSql.AppendLine("          ChkStima, ChkTara_Campionatura, Tara_Campionatura, ")

            strSql.AppendLine("          Inviato,            DataInvio, ")
            strSql.AppendLine("          Data_Creazione,     Data_Modifica, ")
            strSql.AppendLine("          UserName_Creazione, UserName_Modifica, ")
            strSql.AppendLine("          Validita_Inizio,    Validita_Fine ")
            strSql.AppendLine("         ) ")

            strSql.AppendLine(" VALUES ( ")
            strSql.AppendLine("           " & Agro_SQL_SaveNum(Progressivo) & " ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(Tipo) & "' ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Tipo_Cod) & " ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Udm_Cod) & " ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(Val_Cod) & "' ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(Descrizione) & "' ")

            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Progressivo_Origine) & " ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(Piva_SuperUser_Origine) & "' ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Peso_Campione) & " ")

            strSql.AppendLine("         , " & Agro_SQL_SaveNum(ChkStima) & " ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(ChkTara_Campionatura) & " ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Tara_Campionatura) & " ")

            strSql.AppendLine("         , 0  ")
            strSql.AppendLine("         , Null  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveDateTime(Data_creazione) & " ")
            strSql.AppendLine("         , " & Agro_SQL_SaveDateTime(Data_modifica) & " ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(username_modifica) & "' ")
            strSql.AppendLine("         , " & Agro_SQL_SaveDate(Validita_Inizio) & " ")
            strSql.AppendLine("         , " & Agro_SQL_SaveDate(Validita_Fine) & " ")

            strSql.AppendLine(") ")

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

    '#############################################################################################################
    '#############################################################################################################
    '#############################################################################################################

    Public Function Cancella(ByVal Progressivo As Integer,
                             ByVal Tipo As String,
                             ByVal Tipo_Cod As Integer,
                             ByVal Udm_Cod As Integer,
                             ByVal flagSoloOrfani As Boolean,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreParametri
                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.Materie_Prime_Campion_W.Cancella()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            'If Progressivo = 0 Then
            '    Throw New Exception("Parametro non corretto nella query (Progressivo obbligatorio)")
            'End If
            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then

                strSql.Length = 0
                strSql.AppendLine(" UPDATE Materie_Prime_Campionature ")
                strSql.AppendLine(" SET ")
                strSql.AppendLine("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                strSql.AppendLine("      ,Inviato = -1 ")
                strSql.AppendLine(" WHERE  Inviato >= 0 ")

            Else
                strSql.Length = 0
                strSql.AppendLine(" DELETE ")
                strSql.AppendLine(" FROM Materie_Prime_Campionature ")
                strSql.AppendLine(" WHERE  1=1 ")

            End If

            If flagSoloOrfani Then
                strSql.AppendLine(" AND (    Progressivo NOT IN (SELECT DISTINCT Cal_Cod FROM Movimenti_Dettagli) ")
                strSql.AppendLine("      AND Progressivo NOT IN (SELECT DISTINCT Cal_Cod FROM Materie_Prime) ")
                strSql.AppendLine("      AND Progressivo NOT IN (SELECT DISTINCT Progressivo FROM Analisi_EntitaxTestata) ")
                strSql.AppendLine("      ) ")
            End If

            If Progressivo <> 0 Then
                strSql.AppendLine(" AND Materie_Prime_Campionature.Progressivo = " & Agro_SQL_SaveNum(Progressivo) & " ")
            End If

            If Tipo <> "" Then
                strSql.AppendLine(" AND Materie_Prime_Campionature.Tipo = '" & Agro_SQL_SaveText(Tipo) & "' ")
            End If

            If Tipo_Cod <> 0 Then
                strSql.AppendLine(" AND Materie_Prime_Campionature.Tipo_Cod = " & Agro_SQL_SaveNum(Tipo_Cod) & " ")
            End If

            If Udm_Cod <> 0 Then
                strSql.AppendLine(" AND Materie_Prime_Campionature.Udm_Cod = " & Agro_SQL_SaveNum(Udm_Cod) & " ")
            End If

            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri) & vbCrLf)
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

    '#################################################################
    Public Function Modifica(ByVal Progressivo As Integer,
                             ByVal Tipo As String,
                             ByVal Tipo_Cod As Integer,
                             ByVal Udm_Cod As Integer,
                             ByRef objParametri As AgronicaCoreParametri,
                             Optional ByVal Val_Cod As String = Nothing,
                             Optional ByVal Descrizione As String = Nothing,
                             Optional ByVal Validita_Inizio As Date? = Nothing,
                             Optional ByVal Validita_Fine As Date? = Nothing,
                             Optional ByVal Progressivo_Origine As Integer? = Nothing,
                             Optional ByVal Piva_SuperUser_Origine As String = Nothing,
                             Optional ByVal Peso_Campione As Decimal? = Nothing,
                             Optional ByVal ChkStima As Integer? = Nothing,
                             Optional ByVal ChkTara_Campionatura As Integer? = Nothing,
                             Optional ByVal Tara_Campionatura As Decimal? = Nothing,
                             Optional ByVal Data_modifica As DateTime = #2/1/1900#,
                             Optional ByVal username_modifica As String = "",
                             Optional ByVal Val_Cod_Numerico As Boolean = False
                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.Materie_Prime_Campion_W.Modifica()"

        '====================================================================================
        'Parametri opzionali :
        '   Tutti i valori non chiave (se impostati a nothing o non passati 
        '   non ne verrà fatto l'aggiornamento e rimarranno i valori precedenti)
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            If Data_modifica = #2/1/1900# Then
                Data_modifica = Now
            End If

            If username_modifica = "" Then
                username_modifica = objParametri.UsernameOperazione
            End If

            '---------------------------------------------
            strSql.Length = 0

            strSql.AppendLine(" UPDATE Materie_Prime_Campionature ")
            strSql.AppendLine(" SET Data_Modifica = " & Agro_SQL_SaveDateTime(Data_modifica) & " ")
            strSql.AppendLine("   , Username_Modifica = '" & Agro_SQL_SaveText(username_modifica) & "' ")

            If Not IsNothing(Val_Cod) Then

                Select Case Val_Cod_Numerico
                    Case False
                        strSql.AppendLine("   , Val_Cod = '" & Agro_SQL_SaveText(Val_Cod) & "' ")
                    Case True
                        strSql.AppendLine("   , Val_Cod = " & Agro_SQL_SaveNum(Val_Cod) & " ")
                End Select


            End If

            If Not IsNothing(Descrizione) Then
                strSql.AppendLine("   , Descrizione = '" & Agro_SQL_SaveText(Descrizione) & "' ")
            End If

            If Not IsNothing(Validita_Inizio) Then
                strSql.AppendLine("   , Validita_Inizio = " & Agro_SQL_SaveDate(Validita_Inizio) & " ")
            End If

            If Not IsNothing(Validita_Fine) Then
                strSql.AppendLine("   , Validita_Fine = " & Agro_SQL_SaveDate(Validita_Fine) & " ")
            End If

            If Not IsNothing(Progressivo_Origine) Then
                strSql.AppendLine("   , Progressivo_Origine = " & Agro_SQL_SaveNum(Progressivo_Origine) & " ")
            End If

            If Not IsNothing(Piva_SuperUser_Origine) Then
                strSql.AppendLine("   , Piva_SuperUser_Origine = '" & Agro_SQL_SaveText(Piva_SuperUser_Origine) & "' ")
            End If

            If Not IsNothing(Peso_Campione) Then
                strSql.AppendLine("   , Peso_Campione = " & Agro_SQL_SaveNum(Peso_Campione) & " ")
            End If

            If Not IsNothing(ChkStima) Then
                strSql.AppendLine("   , ChkStima = " & Agro_SQL_SaveNum(ChkStima) & " ")
            End If

            If Not IsNothing(ChkTara_Campionatura) Then
                strSql.AppendLine("   , ChkTara_Campionatura = " & Agro_SQL_SaveNum(ChkTara_Campionatura) & " ")
            End If

            If Not IsNothing(Tara_Campionatura) Then
                strSql.AppendLine("   , Tara_Campionatura = " & Agro_SQL_SaveNum(Tara_Campionatura) & " ")
            End If


            strSql.AppendLine(" WHERE Progressivo = " & Agro_SQL_SaveNum(Progressivo) & " ")
            strSql.AppendLine(" AND Tipo = '" & Agro_SQL_SaveText(Tipo) & "' ")
            strSql.AppendLine(" AND Tipo_Cod = " & Agro_SQL_SaveNum(Tipo_Cod) & " ")
            strSql.AppendLine(" AND Udm_Cod = " & Agro_SQL_SaveNum(Udm_Cod) & " ")


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

End Class
