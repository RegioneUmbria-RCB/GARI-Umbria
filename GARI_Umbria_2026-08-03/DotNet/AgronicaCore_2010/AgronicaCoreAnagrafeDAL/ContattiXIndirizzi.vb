Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.UtilityProvider
Imports System.Text
Imports AgronicaCoreDataProvider.DataProvider


Public Class ContattiXIndirizzi_R
    Inherits AgronicaCoreDataProvider.DataProvider


    Public Function Leggi(ByVal Piva As String,
                          ByVal Cod_Contatto As String,
                          ByVal Cod_Indirizzo As Integer,
                          ByVal Tipo_Indirizzo As Integer,
                          ByVal xSelezioneVariabile As enumSelezioneVariabile,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                          ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.ContattiXIndirizzi_R.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   Cod_Contatto = 0           =>  si leggono tutti i contatti
        '   Cod_Indirizzo = 0           =>  si leggono tutti gli indirizzi
        '   Tipo_Indirizzo = 0          =>  si leggono tutti i tipi di indirizzo
        '
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                    enumSelezioneVariabile.Selezione_TabellaCompleta,
                    enumSelezioneVariabile.Selezione_JoinDescrizioni,
                    enumSelezioneVariabile.Selezione_JoinCompleta

                    strSql.Length = 0
                    strSql.AppendLine(" SELECT   PIVA, Cod_Contatto, cod_indirizzo, Tipo_Indirizzo  ")
                    strSql.AppendLine(" FROM    ContattiXIndirizzi ")
                    strSql.AppendLine(" WHERE   ContattiXIndirizzi.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    strSql.AppendLine(" AND     ContattiXIndirizzi.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    If Piva <> "" Then
                        strSql.AppendLine(" AND     ContattiXIndirizzi.Piva = '" & Agro_SQL_SaveText(Piva).Trim & "' ")
                    End If

                    If Cod_Contatto <> "" Then
                        strSql.AppendLine(" AND ContattiXIndirizzi.Cod_Contatto = '" & Agro_SQL_SaveText(Cod_Contatto) & "' ")
                    End If

                    If Cod_Indirizzo <> 0 Then
                        strSql.AppendLine(" AND ContattiXIndirizzi.Cod_Indirizzo = " & Agro_SQL_SaveNum(Cod_Indirizzo) & " ")
                    End If

                    If Tipo_Indirizzo <> 0 Then
                        strSql.AppendLine(" AND ContattiXIndirizzi.Tipo_Indirizzo = " & Agro_SQL_SaveNum(Tipo_Indirizzo) & " ")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If

                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            strSql.AppendLine(" AND   ContattiXIndirizzi.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            strSql.AppendLine(" AND   ContattiXIndirizzi.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------

                    If xOrderBy <> "" Then
                        strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    End If

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

    '#############################################################################
    Public Function Leggi2(ByVal Piva As String,
                           ByVal Cod_Contatto As String,
                           ByVal Cod_Indirizzo As Integer,
                           ByVal Tipo_Indirizzo As Integer,
                           ByVal Cod_RisUm As Integer,
                           ByVal xFiltroAggiuntivo As String,
                           ByVal xOrderBy As String,
                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                           ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.ContattiXIndirizzi_R.Leggi2()"

        '====================================================================================
        'Parametri opzionali :
        '   Cod_Contatto = 0           =>  si leggono tutti i contatti
        '   Cod_Indirizzo = 0           =>  si leggono tutti gli indirizzi
        '   Tipo_Indirizzo = 0          =>  si leggono tutti i tipi di indirizzo
        '
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try


            strSql.Length = 0

            strSql.AppendLine(" SELECT     Imprese.PIVA, Imprese.rag_soc, Contatti.Codice_Fiscale, Contatti.Id_CF, Contatti.Cod_Contatto, Contatti.Rag_Soc AS Rag_Soc_Contatto, " & vbCrLf)
            strSql.AppendLine(" ISNULL(Contatti.Cognome, '') as Cognome, ISNULL(Contatti.Nome, '') as Nome, " & vbCrLf)
            strSql.AppendLine(" ContattiXIndirizzi.Cod_Indirizzo, ContattiXIndirizzi.Tipo_Indirizzo, Codice_Lingua, " & vbCrLf)

            '22/01/2019: nuovo campo
            strSql.AppendLine("  ISNULL( (SELECT descrizione " & vbCrLf)
            strSql.AppendLine("         FROM IndirizzoTipo " & vbCrLf)
            strSql.AppendLine("         WHERE IndirizzoTipo.IndirizzoTipo_Cod =ContattiXIndirizzi.Tipo_Indirizzo " & vbCrLf)
            strSql.AppendLine("         AND IndirizzoTipo.cod_contatto =ContattiXIndirizzi.cod_contatto " & vbCrLf)
            strSql.AppendLine("         AND IndirizzoTipo.piva =ContattiXIndirizzi.piva " & vbCrLf)
            strSql.AppendLine("         ) ,'') AS IndirizzoTipoDesc, " & vbCrLf)

            strSql.AppendLine(" Indirizzi.ind_des, Indirizzi.frz_des, Indirizzi.CAP, ISNULL(ISTAT.LOCALITA, '') AS com_des, ISNULL(ISTAT.COMUNI_PROV, '') AS pro_cod, Indirizzi.stato, Indirizzi.note, Indirizzi.pro_cod_istat, Indirizzi.com_cod_istat, ISNULL(ISTAT.CAP, '') AS cap_istat  " & vbCrLf)
            strSql.AppendLine(" ,ISNULL(Lista_Province.Provincia, '') As pro_des" & vbCrLf)

            strSql.AppendLine(" FROM Imprese INNER JOIN Contatti ON Contatti.PIVA = Imprese.PIVA " & vbCrLf)
            strSql.AppendLine(" INNER JOIN Risorse_Umane ON Contatti.PIVA = Risorse_Umane.PIVA and Contatti.Cod_Contatto = Risorse_Umane.Cod_Contatto " & vbCrLf)
            strSql.AppendLine(" INNER JOIN ContattiXIndirizzi ON Risorse_Umane.Piva = ContattiXIndirizzi.Piva AND Risorse_Umane.Cod_Contatto = ContattiXIndirizzi.Cod_Contatto " & vbCrLf)
            strSql.AppendLine(" INNER JOIN Indirizzi ON ContattiXIndirizzi.cod_indirizzo = Indirizzi.cod_indirizzo " & vbCrLf)
            strSql.AppendLine(" LEFT OUTER JOIN ISTAT ON Indirizzi.pro_cod_istat = ISTAT.PROV AND Indirizzi.com_cod_istat = ISTAT.COM " & vbCrLf)
            strSql.AppendLine(" LEFT OUTER JOIN Lista_Province ON Indirizzi.pro_cod_istat = Lista_Province.PROV ")
            strSql.AppendLine(" WHERE 1 = 1 " & vbCrLf)

            If Cod_Indirizzo <> 0 Then
                strSql.AppendLine(" AND  (Indirizzi.cod_indirizzo = " & Agro_SQL_SaveNum(Cod_Indirizzo) & ") " & vbCrLf)
            End If

            If Piva <> "" Then
                strSql.AppendLine(" AND     ContattiXIndirizzi.Piva = '" & Agro_SQL_SaveText(Piva).Trim & "' " & vbCrLf)
            End If

            If Cod_Contatto <> "" Then
                strSql.AppendLine(" AND ContattiXIndirizzi.Cod_Contatto = '" & Agro_SQL_SaveText(Cod_Contatto) & "' " & vbCrLf)
            End If

            If Cod_RisUm <> 0 Then
                strSql.AppendLine(" AND (Risorse_Umane.Cod_RisUm = " & Agro_SQL_SaveNum(Cod_RisUm) & ") " & vbCrLf)
            End If

            If Tipo_Indirizzo <> 0 Then
                strSql.AppendLine(" AND ContattiXIndirizzi.Tipo_Indirizzo = " & Agro_SQL_SaveNum(Tipo_Indirizzo) & " " & vbCrLf)
            End If

            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.AppendLine(" AND   ContattiXIndirizzi.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.AppendLine(" AND   ContattiXIndirizzi.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else

                strSql.AppendLine(" ORDER BY Imprese.rag_soc, Contatti.rag_soc, Ind_Des ")
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

    '#############################################################################
    Public Function LeggiUnionImpresexIndirizzi(ByVal Piva As String,
                                               ByVal Cod_Contatto As String,
                                               ByVal Cod_Indirizzo As Integer,
                                               ByVal Tipo_Indirizzo As Integer,
                                               ByVal Cod_RisUm As Integer,
                                               ByVal xFiltroAggiuntivo As String,
                                               ByVal xOrderBy As String,
                                               ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                               ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.ContattiXIndirizzi_R.LeggiUnionImpresexIndirizzi()"

        '====================================================================================
        'Parametri opzionali :
        '   Cod_Contatto = 0           =>  si leggono tutti i contatti
        '   Cod_Indirizzo = 0           =>  si leggono tutti gli indirizzi
        '   Tipo_Indirizzo = 0          =>  si leggono tutti i tipi di indirizzo
        '
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try


            strSql.Length = 0

            strSql.AppendLine(" ( " & vbCrLf)
            strSql.AppendLine(" -- CONTATTI X INDIRIZZI " & vbCrLf)
            strSql.AppendLine(" SELECT     Imprese.PIVA, Imprese.rag_soc, Contatti.Codice_Fiscale, Contatti.Id_CF, Contatti.Cod_Contatto, Contatti.Rag_Soc AS Rag_Soc_Contatto, " & vbCrLf)
            strSql.AppendLine(" ISNULL(Contatti.Cognome, '') as Cognome, ISNULL(Contatti.Nome, '') as Nome, " & vbCrLf)
            strSql.AppendLine(" ContattiXIndirizzi.Cod_Indirizzo, ContattiXIndirizzi.Tipo_Indirizzo, Codice_Lingua, " & vbCrLf)
            strSql.AppendLine("  ISNULL( (SELECT descrizione " & vbCrLf)
            strSql.AppendLine("         FROM IndirizzoTipo " & vbCrLf)
            strSql.AppendLine("         WHERE IndirizzoTipo.IndirizzoTipo_Cod =ContattiXIndirizzi.Tipo_Indirizzo " & vbCrLf)
            strSql.AppendLine("         AND IndirizzoTipo.cod_contatto =ContattiXIndirizzi.cod_contatto " & vbCrLf)
            strSql.AppendLine("         AND IndirizzoTipo.piva =ContattiXIndirizzi.piva " & vbCrLf)
            strSql.AppendLine("         ) ,'') AS IndirizzoTipoDesc, " & vbCrLf)
            strSql.AppendLine(" Indirizzi.ind_des, Indirizzi.frz_des, Indirizzi.CAP, ISNULL(ISTAT.LOCALITA, '') AS com_des, ISNULL(ISTAT.COMUNI_PROV, '') AS pro_cod, Indirizzi.stato, Indirizzi.note, Indirizzi.pro_cod_istat, Indirizzi.com_cod_istat, ISNULL(ISTAT.CAP, '') AS cap_istat  " & vbCrLf)
            strSql.AppendLine(" FROM Imprese INNER JOIN Contatti ON Contatti.PIVA = Imprese.PIVA " & vbCrLf)
            strSql.AppendLine(" INNER JOIN Risorse_Umane ON Contatti.PIVA = Risorse_Umane.PIVA and Contatti.Cod_Contatto = Risorse_Umane.Cod_Contatto " & vbCrLf)
            strSql.AppendLine(" INNER JOIN ContattiXIndirizzi ON Risorse_Umane.Piva = ContattiXIndirizzi.Piva AND Risorse_Umane.Cod_Contatto = ContattiXIndirizzi.Cod_Contatto " & vbCrLf)
            strSql.AppendLine(" INNER JOIN Indirizzi ON ContattiXIndirizzi.cod_indirizzo = Indirizzi.cod_indirizzo " & vbCrLf)
            strSql.AppendLine(" LEFT OUTER JOIN ISTAT ON Indirizzi.pro_cod_istat = ISTAT.PROV AND Indirizzi.com_cod_istat = ISTAT.COM " & vbCrLf)
            strSql.AppendLine(" WHERE 1 = 1 " & vbCrLf)
            If Cod_Indirizzo <> 0 Then
                strSql.AppendLine(" AND  (Indirizzi.cod_indirizzo = " & Agro_SQL_SaveNum(Cod_Indirizzo) & ") " & vbCrLf)
            End If
            If Piva <> "" Then
                strSql.AppendLine(" AND     ContattiXIndirizzi.Piva = '" & Agro_SQL_SaveText(Piva).Trim & "' " & vbCrLf)
            End If
            If Cod_Contatto <> "" Then
                strSql.AppendLine(" AND ContattiXIndirizzi.Cod_Contatto = '" & Agro_SQL_SaveText(Cod_Contatto) & "' " & vbCrLf)
            End If
            If Cod_RisUm <> 0 Then
                strSql.AppendLine(" AND (Risorse_Umane.Cod_RisUm = " & Agro_SQL_SaveNum(Cod_RisUm) & ") " & vbCrLf)
            End If
            If Tipo_Indirizzo <> 0 Then
                strSql.AppendLine(" AND ContattiXIndirizzi.Tipo_Indirizzo = " & Agro_SQL_SaveNum(Tipo_Indirizzo) & " " & vbCrLf)
            End If
            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.AppendLine(" AND   ContattiXIndirizzi.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.AppendLine(" AND   ContattiXIndirizzi.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            strSql.AppendLine(" ) " & vbCrLf)
            strSql.AppendLine(" " & vbCrLf)
            'faccio volutamente union invece di union all così mi fa il distinct
            strSql.AppendLine(" UNION " & vbCrLf)
            strSql.AppendLine(" " & vbCrLf)
            strSql.AppendLine(" ( " & vbCrLf)
            strSql.AppendLine(" -- IMPRESE X INDIRIZZI " & vbCrLf)
            strSql.AppendLine(" SELECT     Imprese.PIVA, Imprese.rag_soc, Contatti.Codice_Fiscale, Contatti.Id_CF, Contatti.Cod_Contatto, Contatti.Rag_Soc AS Rag_Soc_Contatto, " & vbCrLf)
            strSql.AppendLine(" ISNULL(Contatti.Cognome, '') as Cognome, ISNULL(Contatti.Nome, '') as Nome, " & vbCrLf)
            strSql.AppendLine(" ImpreseXIndirizzi.Cod_Indirizzo, ImpreseXIndirizzi.Tipo_Indirizzo, Codice_Lingua, " & vbCrLf)
            strSql.AppendLine("  ISNULL( (SELECT descrizione " & vbCrLf)
            strSql.AppendLine("         FROM IndirizzoTipo " & vbCrLf)
            strSql.AppendLine("         WHERE IndirizzoTipo.IndirizzoTipo_Cod =ImpreseXIndirizzi.Tipo_Indirizzo " & vbCrLf)
            strSql.AppendLine("         AND IndirizzoTipo.cod_contatto =ImpreseXIndirizzi.piva " & vbCrLf)
            strSql.AppendLine("         AND IndirizzoTipo.piva =ImpreseXIndirizzi.piva " & vbCrLf)
            strSql.AppendLine("         ) ,'') AS IndirizzoTipoDesc, " & vbCrLf)
            strSql.AppendLine(" Indirizzi.ind_des, Indirizzi.frz_des, Indirizzi.CAP, ISNULL(ISTAT.LOCALITA, '') AS com_des, ISNULL(ISTAT.COMUNI_PROV, '') AS pro_cod, Indirizzi.stato, Indirizzi.note, Indirizzi.pro_cod_istat, Indirizzi.com_cod_istat, ISNULL(ISTAT.CAP, '') AS cap_istat  " & vbCrLf)
            strSql.AppendLine(" FROM Imprese ")
            strSql.AppendLine(" INNER JOIN Contatti ON Contatti.PIVA = Imprese.PIVA " & vbCrLf)
            strSql.AppendLine(" INNER JOIN Risorse_Umane ON Contatti.PIVA = Risorse_Umane.PIVA and Contatti.Cod_Contatto = Risorse_Umane.Cod_Contatto " & vbCrLf)
            strSql.AppendLine(" INNER JOIN ImpreseXIndirizzi ON Contatti.cod_contatto = ImpreseXIndirizzi.piva  " & vbCrLf)
            strSql.AppendLine(" INNER JOIN Indirizzi ON ImpreseXIndirizzi.cod_indirizzo = Indirizzi.cod_indirizzo " & vbCrLf)
            strSql.AppendLine(" LEFT OUTER JOIN ISTAT ON Indirizzi.pro_cod_istat = ISTAT.PROV AND Indirizzi.com_cod_istat = ISTAT.COM " & vbCrLf)
            strSql.AppendLine(" WHERE 1 = 1 " & vbCrLf)
            strSql.AppendLine(" AND not exists ( " & vbCrLf)
            strSql.AppendLine("                 select 1 " & vbCrLf)
            strSql.AppendLine("                 from ContattiXIndirizzi " & vbCrLf)
            strSql.AppendLine("                 where ContattiXIndirizzi.cod_contatto= Contatti.cod_contatto " & vbCrLf)
            strSql.AppendLine("                 )" & vbCrLf)
            If Cod_Indirizzo <> 0 Then
                strSql.AppendLine(" AND  (Indirizzi.cod_indirizzo = " & Agro_SQL_SaveNum(Cod_Indirizzo) & ") " & vbCrLf)
            End If
            If Piva <> "" Then
                strSql.AppendLine(" AND     ImpreseXIndirizzi.Piva = '" & Agro_SQL_SaveText(Piva).Trim & "' " & vbCrLf)
            End If
            If Cod_Contatto <> "" Then
                strSql.AppendLine(" AND Contatti.Cod_Contatto = '" & Agro_SQL_SaveText(Cod_Contatto) & "' " & vbCrLf)
            End If
            If Cod_RisUm <> 0 Then
                strSql.AppendLine(" AND (Risorse_Umane.Cod_RisUm = " & Agro_SQL_SaveNum(Cod_RisUm) & ") " & vbCrLf)
            End If
            If Tipo_Indirizzo <> 0 Then
                strSql.AppendLine(" AND ImpreseXIndirizzi.Tipo_Indirizzo = " & Agro_SQL_SaveNum(Tipo_Indirizzo) & " " & vbCrLf)
            End If
            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.AppendLine(" AND   ImpreseXIndirizzi.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.AppendLine(" AND   ImpreseXIndirizzi.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            strSql.AppendLine(" ) " & vbCrLf)


            If xOrderBy <> "" Then
                strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                strSql.AppendLine(" ORDER BY Imprese.rag_soc, Contatti.rag_soc, Ind_Des ")
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


    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="Piva"></param>
    ''' <param name="Cod_Contatto"></param>
    ''' <param name="sa_cod">default 99</param>
    ''' <param name="cod_indirizzo"></param>
    ''' <param name="Tipo_Indirizzo"></param>
    ''' <param name="xSelezioneVariabile"></param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="objParametri"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[garavini]	26/11/2010	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Function LeggiContattoSpecifico(ByVal Piva As String,
                                           ByVal Cod_Contatto As String,
                                           ByVal sa_cod As Integer,
                                           ByVal cod_indirizzo As Integer,
                                           ByVal Tipo_Indirizzo As Integer,
                                           ByVal xSelezioneVariabile As enumSelezioneVariabile,
                                           ByVal xFiltroAggiuntivo As String,
                                           ByVal xOrderBy As String,
                                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                           ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.ContattiXIndirizzi_R.LeggiContattoSpecifico()"

        '====================================================================================
        'Parametri opzionali :
        '
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try
            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi


                Case enumSelezioneVariabile.Selezione_TabellaCompleta


                Case enumSelezioneVariabile.Selezione_JoinDescrizioni

                    If Piva = "" Then
                        Throw New Exception("Parametro non corretto nella query (Piva obbligatoria)")
                    End If

                    If Cod_Contatto = "" Then
                        Throw New Exception("Parametro non corretto nella query (Cod_Contatto obbligatorio)")
                    End If

                    '---------------------------------------------
                    strSql.Length = 0
                    strSql.AppendLine(" SELECT        ContattiXIndirizzi.Piva, ContattiXIndirizzi.Sa_Cod, ContattiXIndirizzi.Cod_Contatto, ContattiXIndirizzi.Cod_Indirizzo, ContattiXIndirizzi.Tipo_Indirizzo, Indirizzi.ind_des, Indirizzi.frz_des, Indirizzi.CAP, Indirizzi.stato, stati.Descrizione as Stato_Des, lingue.Descrizione as Lingua_Des, Indirizzi.note, Indirizzi.Codice_Lingua, Indirizzi.Codice_Alternativo,  ")
                    'aggiunta una condizione: se Indirizzi.sato <> IT and Indirizzi.stato <> ITALIA, allora prende il valore di com_des ditrettamente dalla tabella del DB
                    'tale condizione è stata aggiunta per tener conto che le citta degli indirizzi esteri vengono salvate sul campo com_des
                    strSql.AppendLine(" ISNULL(ISTAT.COMUNI_PROV, '') AS pro_cod, ISNULL(ISTAT.CAP, '') AS CAP, ISNULL(Lista_Province.PROVINCIA, '') AS pro_des, CASE WHEN(Gestione_Gerarchia_Geografica <> 1) THEN Indirizzi.com_des ELSE ISNULL(ISTAT.LOCALITA, '') END AS com_des,
                                        indirizzi.pro_cod_istat, indirizzi.com_cod_istat,ISTAT.LOCALITA, ISTAT.COMUNI_PROV, Lista_Province.SIGLA, Lista_Province.REG, ")
                    strSql.AppendLine(" Lista_Province.PROVINCIA, ISNULL(ISTAT.LOCALITA, '') AS com_des, ISNULL(ISTAT.COMUNI_PROV, '') AS pro_cod, ISNULL(ISTAT.CAP, '') AS CAP, ISNULL(Lista_Province.PROVINCIA, '') AS pro_des ")
                    strSql.AppendLine(" , indirizzi.validita_inizio, indirizzi.validita_fine, indirizzi.data_creazione, indirizzi.data_modifica, indirizzi.username_creazione, indirizzi.username_modifica ")
                    strSql.AppendLine(" FROM  ContattixIndirizzi, Indirizzi ")
                    strSql.AppendLine(" LEFT OUTER JOIN ISTAT ON Indirizzi.pro_cod_istat = ISTAT.PROV AND Indirizzi.com_cod_istat = ISTAT.COM ")
                    strSql.AppendLine(" LEFT OUTER JOIN  Lista_Province ON Indirizzi.pro_cod_istat = Lista_Province.PROV ")
                    strSql.AppendLine(" LEFT OUTER JOIN ACCDAA_ANAG_T005_TabellaCodiciPaesiTerziISO3166 stati on stati.Codice = Indirizzi.stato ")
                    strSql.AppendLine(" LEFT OUTER JOIN ACCDAA_ANAG_T001_TabellaCodiceDelleLingue lingue on lingue.Codice = Indirizzi.Codice_Lingua ")
                    strSql.AppendLine(" WHERE ContattixIndirizzi.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    strSql.AppendLine(" AND   ContattixIndirizzi.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    strSql.AppendLine(" AND   Indirizzi.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    strSql.AppendLine(" AND   Indirizzi.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    strSql.AppendLine(" AND   ContattixIndirizzi.Cod_Indirizzo = Indirizzi.Cod_Indirizzo ")
                    strSql.AppendLine(" AND   ContattixIndirizzi.Cod_Contatto = '" & Agro_SQL_SaveText(Cod_Contatto) & "' ")

                    If Piva <> "" Then
                        strSql.AppendLine(" AND   ContattixIndirizzi.Piva = '" & Agro_SQL_SaveText(Piva) & "'")
                    End If

                    If sa_cod <> 99 Then
                        strSql.AppendLine(" AND ContattixIndirizzi.sa_cod = " & Agro_SQL_SaveNum(sa_cod) & " ")
                    End If


                    If cod_indirizzo <> 0 Then
                        strSql.AppendLine(" AND ContattixIndirizzi.Cod_Indirizzo = " & Agro_SQL_SaveNum(cod_indirizzo) & " ")
                    End If

                    If Tipo_Indirizzo <> 0 Then
                        strSql.AppendLine(" AND ContattixIndirizzi.Tipo_Indirizzo = " & Agro_SQL_SaveNum(Tipo_Indirizzo) & " ")
                    End If



                    If xFiltroAggiuntivo <> "" Then
                        strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            strSql.AppendLine(" AND     ContattixIndirizzi.Inviato >= 0 ")
                            strSql.AppendLine(" AND     Indirizzi.Inviato >= 0 ")
                            'strSql.AppendLine(" AND     Lista_Province.Inviato >= 0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            'strSql.AppendLine(" AND     Lista_Province.Inviato = -1 ")
                            strSql.AppendLine(" AND     Indirizzi.Inviato = -1 ")
                            strSql.AppendLine(" AND     ContattixIndirizzi.Inviato = -1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '---------------------------------------------

                    If xOrderBy <> "" Then
                        strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))

                    End If

                Case enumSelezioneVariabile.Selezione_JoinCompleta
                    '
                    '
                    '
                    '


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

        'DT = Nothing
        Return dt

    End Function


    '###################################################################################
    'legge il cod_indirizzo della residenza (se p.f.) o della sede operativa (se p.g.)
    Public Function CodIndirizzo_from_CodRisUm(ByVal Cod_RisUm As Integer,
                                               ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                               ) As Integer

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.ContattiXIndirizzi_R.CodIndirizzo_from_CodRisUm()"

        Dim messaggioErrore As String = ""
        Dim dt As DataTable
        Dim codIndirizzo As Integer = 0
        Dim objContatti As New Contatti_R
        Dim filtroAgg As String = " ( ContattiXIndirizzi.Tipo_Indirizzo IN ( " & CStr(INDIRIZZO_RESIDENZA) & ", " & CStr(INDIRIZZO_SEDE_OPERATIVA) & ") ) "

        Try

            dt = objContatti.Contatti_Contatto_Leggi("", "",
                                                     Cod_RisUm,
                                                     0,
                                                     False,
                                                     True,
                                                     0,
                                                     0,
                                                     False,
                                                     0,
                                                     -99,
                                                     0, "",
                                                     True,
                                                     0, 0, 0, 0, 0,
                                                     enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                                     filtroAgg,
                                                     "",
                                                     objParametri)

            If Not dt Is Nothing AndAlso dt.Rows.Count > 0 Then
                codIndirizzo = dt.Rows(0).Item("Cod_Indirizzo")
            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return codIndirizzo

    End Function

    '###################################################################################
    'a differenza della CodIndirizzo_from_CodRisUm non fa il filtro fisso sulla tipologia indirizzo
    'ma prende il filtro dal chiamante
    Public Function CodIndirizzo_from_CodRisUm2(ByVal Cod_RisUm As Integer,
                                                ByVal FiltroAggiuntivo As String,
                                                ByVal OrderBy As String,
                                               ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                               ) As Integer

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.ContattiXIndirizzi_R.CodIndirizzo_from_CodRisUm2()"

        Dim messaggioErrore As String = ""
        Dim dt As DataTable
        Dim codIndirizzo As Integer = 0
        Dim objContatti As New Contatti_R

        Try

            dt = objContatti.Contatti_Contatto_Leggi("", "",
                                                     Cod_RisUm,
                                                     0,
                                                     False,
                                                     True,
                                                     0,
                                                     0,
                                                     False,
                                                     0,
                                                     -99,
                                                     0, "",
                                                     True,
                                                     0, 0, 0, 0, 0,
                                                     enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                                     FiltroAggiuntivo,
                                                     OrderBy,
                                                     objParametri)

            If Not dt Is Nothing AndAlso dt.Rows.Count > 0 Then
                codIndirizzo = dt.Rows(0).Item("Cod_Indirizzo")
            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return codIndirizzo

    End Function

    '###################################################################################
    'cerca il cod_indirizzo di un'indirizzo valorizzato, se non lo trova prende cod_indirizzo della residenza o sede operativa in base a se pf o pg
    Public Function Ricava_CodIndirizzoValorizzato_from_CodRisUm(ByVal Cod_RisUm As Integer,
                                                                 ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                               ) As Integer

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.ContattiXIndirizzi_R.Ricava_CodIndirizzoValorizzato_from_CodRisUm()"
        Dim messaggioErrore As String = ""
        Dim codIndirizzo As Integer = 0
        Dim FiltroAggiuntivo As String = " (Indirizzi.pro_cod_istat <> '000' and Indirizzi.com_cod_istat <> '000') "
        Dim OrderBy As String = " Indirizzi.ind_des DESC "

        Try
            'cerca il cod_indirizzo di un'indirizzo valorizzato
            codIndirizzo = CodIndirizzo_from_CodRisUm2(Cod_RisUm,
                                                      FiltroAggiuntivo,
                                                      OrderBy,
                                                      objParametri)
            If codIndirizzo = 0 Then
                'Se indirizzo non trovato
                'prende cod_indirizzo della residenza o sede operativa 
                codIndirizzo = CodIndirizzo_from_CodRisUm(Cod_RisUm,
                                                        objParametri)

            End If

            If codIndirizzo = 0 Then
                'non dovrebbe succedere
                Dim debug As Boolean = True
            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return codIndirizzo

    End Function




    '###################################################################################
    'legge il cod_indirizzo 
    Public Function CodIndirizzo_from_CodContatto(ByVal Piva As String,
                                                  ByVal CodContatto As String,
                                                  ByVal TipoIndirizzo As Integer,
                                                  ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                  ) As Integer

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.ContattiXIndirizzi_R.CodIndirizzo_from_CodContatto()"

        Dim messaggioErrore As String = ""
        Dim dt As DataTable
        Dim codIndirizzo As Integer = 0

        Try

            dt = Leggi(Piva,
                       CodContatto,
                       0,
                       TipoIndirizzo,
                       enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                       "",
                       "",
                       objParametri)

            If Not dt Is Nothing AndAlso dt.Rows.Count > 0 Then
                codIndirizzo = dt.Rows(0).Item("Cod_Indirizzo")
            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return codIndirizzo

    End Function

    '###################################################################################
    ''' <summary>
    ''' Legge l'elenco dei cod_indirizzo
    ''' </summary>
    Public Function Elenco_CodIndirizzo_from_CodContatto(ByVal piva As String,
                                                         ByVal codContatto As String,
                                                         ByVal tipoIndirizzo As Integer,
                                                         ByVal xFiltroAggiuntivo As String,
                                                         ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                         ) As Integer()

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.ContattiXIndirizzi_R.Elenco_CodIndirizzo_from_CodContatto()"

        Dim messaggioErrore As String = ""
        Dim dt As DataTable
        Dim codIndirizzo() As Integer = {0}

        Try

            dt = Leggi(piva,
                       codContatto,
                       0,
                       tipoIndirizzo,
                       enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                       xFiltroAggiuntivo,
                       "",
                       objParametri)

            If Not dt Is Nothing AndAlso dt.Rows.Count > 0 Then
                ' Giulia: 20/2/2019: Necessario, perché se ottengo più di un risultato va in errore
                Array.Resize(codIndirizzo, dt.Rows.Count)
                For i = 0 To dt.Rows.Count - 1
                    codIndirizzo(i) = CInt(dt.Rows(i).Item("Cod_Indirizzo"))
                Next

            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return codIndirizzo

    End Function


    '###################################################################################
    Public Function CodiceLingua_from_CodRisUm_o_CodIndirizzoRisUm(ByVal Cod_RisUm As Integer,
                                                                   ByVal Cod_IndirizzoRisUm As Integer,
                                                                    ByVal FiltroAggiuntivo As String,
                                                                    ByVal OrderBy As String,
                                                                   ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                                   ) As String

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.ContattiXIndirizzi_R.CodiceLingua_from_CodRisUm_o_CodIndirizzoRisUm()"

        Dim messaggioErrore As String = ""
        Dim dt As DataTable
        Dim codice_lingua As String = ""

        Try

            '19/05/2020 SOSTITUITA LETTURA
            'dt = Leggi2("", "", _
            '            Cod_IndirizzoRisUm, _
            '            0, _
            '            Cod_RisUm, _
            '            FiltroAggiuntivo,
            '            OrderBy,
            '            objParametri)

            dt = LeggiUnionImpresexIndirizzi("", "",
                                              Cod_IndirizzoRisUm,
                                              0,
                                              Cod_RisUm,
                                              FiltroAggiuntivo,
                                              OrderBy,
                                              objParametri)

            If Not dt Is Nothing AndAlso dt.Rows.Count > 0 Then
                codice_lingua = CStr(dt.Rows(0).Item("codice_lingua")).ToUpper
            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return codice_lingua

    End Function

End Class



'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§



Public Class ContattiXIndirizzi_W
    Inherits AgronicaCoreDataProvider.DataProvider

    '============================================================================
    Public Function Scrivi(ByVal Piva As String,
                           ByVal Sa_Cod As Long,
                           ByVal Cod_Contatto As String,
                           ByVal Cod_Indirizzo As Integer,
                           ByVal Tipo_Indirizzo As Integer,
                           ByVal Validita_Inizio As Date,
                           ByVal Validita_Fine As Date,
                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                           Optional ByVal Data_creazione As Date = #2/1/1900#,
                           Optional ByVal Data_modifica As Date = #2/1/1900#,
                           Optional ByVal username_creazione As String = "",
                           Optional ByVal username_modifica As String = ""
                           ) As Boolean

        Dim nomeRoutine As String = "AnagrafeCoreAnagrafeDAL.ContattiXIndirizzi_W.Scrivi()"

        '====================================================================================
        'Parametri opzionali :
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
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
            strSql.Length = 0
            strSql.AppendLine("INSERT INTO ContattixIndirizzi( ")
            strSql.AppendLine("                    Piva,               ")
            strSql.AppendLine("                    Sa_Cod,             ")
            strSql.AppendLine("                    Cod_Contatto,       ")
            strSql.AppendLine("                    Cod_Indirizzo,      ")
            strSql.AppendLine("                    Tipo_Indirizzo,     ")
            strSql.AppendLine("                    Inviato, DataInvio, ")
            strSql.AppendLine("                    Data_Creazione,     Data_Modifica, ")
            strSql.AppendLine("                    UserName_Creazione, UserName_Modifica, ")
            strSql.AppendLine("                    Validita_Inizio,    Validita_Fine ")
            strSql.AppendLine("                    ) ")

            strSql.AppendLine("VALUES  (")
            strSql.AppendLine("          '" & Agro_SQL_SaveText(Piva) & "' ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(Cod_Contatto) & "' ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Cod_Indirizzo) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Tipo_Indirizzo) & "  ")
            strSql.AppendLine("         , 0  ")
            strSql.AppendLine("         , Null  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveDate(Data_creazione) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveDate(Data_modifica) & "  ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(username_modifica) & "' ")
            strSql.AppendLine("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")
            strSql.AppendLine(")")

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


    '############################################################################
    '############################################################################
    '############################################################################


    '============================================================================
    Public Function Modifica(ByVal Cod_Contatto As String,
                             ByVal New_Piva As String,
                             ByVal New_Sa_Cod As Integer,
                             ByVal Cod_Indirizzo As Integer,
                             ByVal Tipo_Indirizzo As Integer,
                             ByVal Validita_Inizio As Date,
                             ByVal Validita_Fine As Date,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                             ) As Boolean

        '====================================================================================
        'Parametri opzionali :
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim nomeRoutine As String = "AnagrafeCoreAnagrafeDAL.ContattixIndirizzi_W.Modifica()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            If Cod_Indirizzo = 0 Then
                Throw New Exception("Parametro non corretto nella query (Cod_Indirizzo obbligatorio)")
            End If

            If Cod_Contatto = "" Then
                Throw New Exception("Parametro non corretto nella query (Cod_Contatto obbligatorio)")
            End If

            If Tipo_Indirizzo = 0 Then
                Throw New Exception("Parametro non corretto nella query (Tipo_Indirizzo obbligatorio)")
            End If


            '---------------------------------------------
            strSql.Length = 0
            strSql.AppendLine("UPDATE ContattixIndirizzi SET ")
            strSql.AppendLine("    Piva              = '" & Agro_SQL_SaveText(New_Piva) & "'  ")
            strSql.AppendLine("   ,Sa_Cod            = " & Agro_SQL_SaveNum(New_Sa_Cod) & "  ")
            strSql.AppendLine("   ,Inviato           =  0 ")
            strSql.AppendLine("   ,DataInvio         =  Null ")
            strSql.AppendLine("   ,Data_Modifica     =  " & Agro_SQL_SaveDate(Date.Now))
            strSql.AppendLine("   ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            strSql.AppendLine("   ,Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Inizio))
            strSql.AppendLine("   ,Validita_Fine     =  " & Agro_SQL_SaveDate(Validita_Fine))

            strSql.AppendLine(" WHERE Cod_Contatto = '" & Agro_SQL_SaveText(Cod_Contatto) & "'")
            strSql.AppendLine(" AND   Cod_Indirizzo = " & Agro_SQL_SaveNum(Cod_Indirizzo) & " ")
            strSql.AppendLine(" AND   Tipo_Indirizzo = " & Agro_SQL_SaveNum(Tipo_Indirizzo) & " ")


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

    Public Function ModificaPuntuale(ByVal Piva As String,
                                     ByVal Cod_Contatto As String,
                                     ByVal Cod_indirizzo As Integer,
                                     ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                     Optional ByVal Sa_Cod As Integer? = Nothing,
                                     Optional ByVal Tipo_Indirizzo As Integer? = Nothing,
                                     Optional ByVal Data_Modifica As DateTime = #2/1/1900#,
                                     Optional ByVal Username_Modifica As String = ""
                                     ) As Boolean

        Dim nomeRoutine As String = "AnagrafeCoreAnagrafeDAL.ContattixIndirizzi_W.ModificaPuntuale()"

        '====================================================================================
        'Parametri opzionali :
        '   Tutti i valori non chiave (se impostati a nothing o non passati 
        '   non ne verrà fatto l'aggiornamento e rimarranno i valori precedenti)
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        '------------------------------

        Try

            If Data_Modifica = #2/1/1900# Then
                Data_Modifica = Date.Now
            End If

            If Username_Modifica = "" Then
                Username_Modifica = objParametri.UsernameOperazione
            End If

            If Cod_indirizzo = 0 Then
                Throw New Exception("Parametro non corretto nella query (Cod_indirizzo obbligatorio)")
            End If

            If String.IsNullOrEmpty(Piva) Then
                Throw New Exception("Parametro non corretto nella query (Piva = '')")
            End If


            '---------------------------------------------
            strSql.Length = 0

            strSql.AppendLine(" UPDATE ContattiXIndirizzi ")
            strSql.AppendLine(" SET Data_Modifica = " & Agro_SQL_SaveDateTime(Data_Modifica) & " ")
            strSql.AppendLine("   , Username_Modifica = '" & Agro_SQL_SaveText(Username_Modifica) & "' ")



            If Not IsNothing(Sa_Cod) Then
                strSql.AppendLine("   , Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            End If

            If Not IsNothing(Tipo_Indirizzo) Then
                strSql.AppendLine("   , Tipo_Indirizzo = " & Agro_SQL_SaveNum(Tipo_Indirizzo) & " ")
            End If

            '---------------------------------------------            

            strSql.AppendLine(" WHERE Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            strSql.AppendLine(" AND Cod_Indirizzo = " & Agro_SQL_SaveNum(Cod_indirizzo) & " ")

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

    '############################################################################
    '############################################################################
    '############################################################################



    '============================================================================
    Public Function Cancella(ByVal Cod_Contatto As String,
                             ByVal Cod_Indirizzo As Integer,
                             ByVal Tipo_Indirizzo As Integer,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                             ) As Boolean

        '============================================================================
        'Se tento di cancellare fisicamente un record con INVIATO=1
        'allora pongo INVIATO=-1
        'questo per consentire la risincronizzazione col server
        '============================================================================

        Dim nomeRoutine As String = "AnagrafeCoreAnagrafeDAL.ContattixIndirizzi_W.Cancella()"

        '====================================================================================
        'Parametri opzionali :
        '   Cod_Contatto
        '   Cod_Indirizzo
        '   Tipo_indirizzo

        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then

                strSql.Length = 0
                strSql.AppendLine(" UPDATE ContattixIndirizzi ")
                strSql.AppendLine(" SET ")
                strSql.AppendLine("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                strSql.AppendLine("      ,Inviato = -1 ")
                strSql.AppendLine(" AND Inviato >= 0")

            Else

                strSql.Length = 0
                strSql.AppendLine(" DELETE ")
                strSql.AppendLine(" FROM     ContattixIndirizzi ")
                strSql.AppendLine(" WHERE    1=1 ")

            End If

            If Cod_Contatto <> "" Then
                strSql.AppendLine(" AND   Cod_Contatto = '" & Agro_SQL_SaveText(Trim(Cod_Contatto)) & "'  ")
            End If

            If Cod_Indirizzo <> 0 Then
                strSql.AppendLine(" AND   Cod_Indirizzo = " & Cod_Indirizzo & " ")

                If Tipo_Indirizzo <> 0 Then
                    strSql.AppendLine(" AND   Tipo_Indirizzo = " & Tipo_Indirizzo & " ")
                End If

            End If

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


    '##############################################################################################
    Public Function AggiornaValiditaInizio(ByVal Cod_Contatto As String,
                                           ByVal Cod_Indirizzo As Long,
                                           ByVal Tipo_Indirizzo As Long,
                                           ByVal Validita_Inizio As Date,
                                           ByVal xFiltroAggiuntivo As String,
                                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                           ) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.ContattixIndirizzi_Write.AggiornaValiditaInizio()"

        '====================================================================================
        'Parametri opzionali :
        '   Cod_Contatto = ""
        '   Cod_Indirizzo = 0 
        '   Tipo_Indirizzo = 0
        '
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            strSql.Length = 0
            strSql.AppendLine("UPDATE ContattixIndirizzi SET ")
            strSql.AppendLine("              UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            strSql.AppendLine("             ,Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Inizio))
            strSql.AppendLine("             ,Data_Modifica     =  " & Agro_SQL_SaveDate(Date.Now))
            strSql.AppendLine(" WHERE   Validita_Inizio < " & Agro_SQL_SaveDate(Validita_Inizio))

            If Cod_Contatto <> "" Then
                strSql.AppendLine(" AND   Cod_Contatto = '" & Agro_SQL_SaveText(Trim(Cod_Contatto)) & "'  ")
            End If

            If Cod_Indirizzo <> 0 Then
                strSql.AppendLine(" AND   Cod_Indirizzo = " & Cod_Indirizzo & " ")

                If Tipo_Indirizzo <> 0 Then
                    strSql.AppendLine(" AND   Tipo_Indirizzo = " & Tipo_Indirizzo & " ")
                End If

            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------
        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        Finally

            strSql = Nothing

        End Try

        Return xRisp

    End Function


    '##############################################################################################
    Public Function AggiornaValiditaFine(ByVal Cod_Contatto As String,
                                         ByVal Cod_Indirizzo As Long,
                                         ByVal Tipo_Indirizzo As Long,
                                         ByVal Validita_Fine As Date,
                                         ByVal xFiltroAggiuntivo As String,
                                         ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                         ) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.ContattixIndirizzi_Write.AggiornaValiditaFine()"

        '====================================================================================
        'Parametri opzionali :
        '   Cod_Contatto = ""
        '   Cod_Indirizzo = 0 
        '   Tipo_Indirizzo = 0
        '
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            strSql.Length = 0
            strSql.AppendLine("UPDATE ContattixIndirizzi SET ")
            strSql.AppendLine("              UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            strSql.AppendLine("             ,Validita_Fine   =  " & Agro_SQL_SaveDate(Validita_Fine))
            strSql.AppendLine("             ,Data_Modifica     =  " & Agro_SQL_SaveDate(Date.Now))
            strSql.AppendLine(" WHERE   Validita_Fine > " & Agro_SQL_SaveDate(Validita_Fine))

            If Cod_Contatto <> "" Then
                strSql.AppendLine(" AND   Cod_Contatto = '" & Agro_SQL_SaveText(Trim(Cod_Contatto)) & "'  ")
            End If

            If Cod_Indirizzo <> 0 Then
                strSql.AppendLine(" AND   Cod_Indirizzo = " & Cod_Indirizzo & " ")

                If Tipo_Indirizzo <> 0 Then
                    strSql.AppendLine(" AND   Tipo_Indirizzo = " & Tipo_Indirizzo & " ")
                End If

            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        Finally

            strSql = Nothing

        End Try

        Return xRisp

    End Function


    Public Function Aggiorna_Indirizzo(ByVal PIVA As String,
                                       ByVal Sa_Cod As Integer,
                                       ByVal Cod_Contatto As String,
                                       ByVal Cod_Indirizzo_Da_Eliminare As Integer,
                                       ByRef Cod_Indirizzo_Eliminato As Integer,
                                       ByRef Num_Indirizzi_Eliminati As Integer,
                                       ByVal PermettiEliminazioneUnSoloIndirizzo As Boolean,
                                       ByRef Cod_Indirizzo_New As Integer,
                                       ByVal Tipo_Indirizzo As Integer,
                                       ByVal Ind_Des As String,
                                       ByVal Frz_Des As String,
                                       ByVal CAP As String,
                                       ByVal Com_Des As String,
                                       ByVal Pro_Cod As String,
                                       ByVal Stato As String,
                                       ByVal Note As String,
                                       ByVal Pro_Cod_Istat As String,
                                       ByVal Com_Cod_Istat As String,
                                       ByVal Validita_Inizio As Date,
                                       ByVal Validita_Fine As Date,
                                       ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                       Optional ByVal Data_creazione As Date = #2/1/1900#,
                                       Optional ByVal Data_modifica As Date = #2/1/1900#,
                                       Optional ByVal username_creazione As String = "",
                                       Optional ByVal username_modifica As String = ""
                                       ) As Boolean


        'per evitare di eliminare tutto
        If PIVA = "" Then
            Throw New Exception("Occorre specificare una Piva")
        End If
        If Cod_Contatto = "" Then
            Throw New Exception("Occorre specificare un Cod_Contatto")
        End If
        If Tipo_Indirizzo = 0 Then
            Throw New Exception("Occorre specificare un Tipo_Indirizzo")
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
        Dim ContattiXIndirizzi_R As New AgronicaCoreAnagrafeDAL.ContattiXIndirizzi_R


        Dim dt As DataTable = ContattiXIndirizzi_R.Leggi(PIVA, Cod_Contatto,
                                                         Cod_Indirizzo_Da_Eliminare,
                                                         Tipo_Indirizzo,
                                                         enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                                         "", "", objParametri)

        If dt.Rows.Count > 0 Then

            If PermettiEliminazioneUnSoloIndirizzo AndAlso dt.Rows.Count > 1 Then
                Throw New Exception("Impossibile eliminare più indirizzi contemporaneamente con PermettiEliminazioneUnSoloIndirizzo=true")
            End If

            Dim i As Integer
            For i = 0 To dt.Rows.Count - 1

                If dt.Rows(i).Item("Cod_Contatto") = "" Or dt.Rows(i).Item("Cod_Contatto") <> Cod_Contatto Then
                    Throw New Exception(" dt.Rows(i).Item(Cod_Contatto) =  Or dt.Rows(i).Item(Cod_Contatto) <> Cod_Contatto ")
                End If

                Cod_Indirizzo_Eliminato = dt.Rows(i).Item("Cod_Indirizzo")
                Cancella(dt.Rows(i).Item("Cod_Contatto"), Cod_Indirizzo_Eliminato, Tipo_Indirizzo, "", objParametri)
                indirizzi.Cancella(Cod_Indirizzo_Eliminato, "", objParametri)
              
                Num_Indirizzi_Eliminati = i + 1
            Next

        ElseIf dt.Rows.Count = 0 Then
       

        End If

        Cod_Indirizzo_New = indirizzi.ScriviNuovo(Ind_Des,
                                                  Frz_Des,
                                                  CAP,
                                                  Com_Des,
                                                  Pro_Cod,
                                                  Stato,
                                                  Note,
                                                  Pro_Cod_Istat,
                                                  Com_Cod_Istat,
                                                  Validita_Inizio,
                                                  Validita_Fine,
                                                  objParametri)

        xRisp = Scrivi(PIVA, Sa_Cod, Cod_Contatto, Cod_Indirizzo_New, Tipo_Indirizzo, AGRODATAINIZIO, AGRODATAFINE, objParametri)

        Return xRisp

    End Function



    

End Class
