Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri

Public Class ISTAT_Comuni_R
    Inherits AgronicaCoreDataProvider.DataProvider


    '##############################################################################################
    Public Function Leggi(ByVal Cod_Regionale As String,
                          ByVal Cod_Istat As String,
                          ByVal Pro_Cod_Istat As String,
                          ByVal Com_Cod_Istat As String,
                          ByVal Provincia As String,
                          ByVal Cod_Belfiore As String,
                          ByVal xSelezioneVariabile As enumSelezioneVariabile,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreParametri
                          ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Istat_Comuni_R.Leggi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try
            '---------------------------------------------

            'Select Case xSelezioneVariabile

            '    Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi



            '    Case enumSelezioneVariabile.Selezione_TabellaCompleta




            '    Case enumSelezioneVariabile.Selezione_JoinDescrizioni
            StrSQL.Length = 0
            StrSQL.Append(" SELECT Istat_Comuni.*, Lista_Province.PROVINCIA AS Provincia_Long ")
            StrSQL.Append(" FROM  Istat_Comuni INNER JOIN Lista_Province ON ISTAT_Comuni.Provincia = Lista_Province.SIGLA ")
            StrSQL.Append(" WHERE ((Cessato <> 'C') OR (Cessato IS NULL)) ")


            If Cod_Regionale <> "" Then
                StrSQL.Append(" AND Cod_Regionale = '" & Agro_SQL_SaveText(Trim(Cod_Regionale)) & "' ")
            End If

            If Cod_Istat <> "" Then
                StrSQL.Append(" AND Cod_Istat = '" & Agro_SQL_SaveText(Trim(Cod_Istat)) & "' ")
            End If

            If Pro_Cod_Istat <> "" Then
                StrSQL.Append(" AND Pro_Cod_Istat = '" & Agro_SQL_SaveText(Trim(Pro_Cod_Istat)) & "' ")
            End If

            If Com_Cod_Istat <> "" Then
                StrSQL.Append(" AND Com_Cod_Istat = '" & Agro_SQL_SaveText(Trim(Com_Cod_Istat)) & "' ")
            End If

            If Provincia <> "" Then
                StrSQL.Append(" AND Provincia = '" & Agro_SQL_SaveText(Trim(Provincia)) & "' ")
            End If

            If Cod_Belfiore <> "" Then
                StrSQL.Append(" AND Cod_Belfiore = '" & Agro_SQL_SaveText(Trim(Cod_Belfiore)) & "' ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            'Select Case objParametri.FlagVisibilita
            '    Case enumVisibilita.Visibilita_SoloNonCancellati
            '        StrSQL.Append(" AND   Istat_Comuni.Inviato >=0 ")
            '    Case enumVisibilita.Visibilita_SoloCancellati
            '        StrSQL.Append(" AND   Istat_Comuni.Inviato =-1 ")
            '    Case enumVisibilita.Visibilita_Tutti
            '        '...................................
            '    Case Else
            '        Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            'End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY Lista_Province.PROVINCIA, Descrizione ")
            End If



            '    Case enumSelezioneVariabile.Selezione_JoinCompleta


            'End Select

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function



    Public Function Leggi2(ByVal xFiltroAggiuntivo As String,
                           ByVal xOrderBy As String,
                           ByRef objParametri As AgronicaCoreParametri
                           ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Istat_Comuni_R.Leggi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT Lista_Province.PROVINCIA as provincia_des, ISTAT_Comuni.* ")
            StrSQL.Append(" FROM   Lista_Province INNER JOIN ")
            StrSQL.Append("      ISTAT_Comuni ON Lista_Province.PROV = ISTAT_Comuni.Pro_Cod_Istat")
            StrSQL.Append(" where 1= 1 ")
             

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            'Select Case objParametri.FlagVisibilita
            '    Case enumVisibilita.Visibilita_SoloNonCancellati
            '        StrSQL.Append(" AND   Istat_Comuni.Inviato >=0 ")
            '    Case enumVisibilita.Visibilita_SoloCancellati
            '        StrSQL.Append(" AND   Istat_Comuni.Inviato =-1 ")
            '    Case enumVisibilita.Visibilita_Tutti
            '        '...................................
            '    Case Else
            '        Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            'End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" order by Lista_Province.PROVINCIA, descrizione ")
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    '##############################################################################################
    Public Function Comune_from_CodIstat(ByVal Comune_CodIstat As String,
                                         ByVal Provincia_CodIstat As String,
                                         ByRef objParametri As AgronicaCoreParametri
                                         ) As String

        Dim dt As DataTable

        Dim objCOM As New AgronicaCoreMetaSchemaDAL.Istat_R
        'Recupero le informazioni
        dt = objCOM.Leggi(CStr(Provincia_CodIstat), CStr(Comune_CodIstat),
                          "", "", "",
                          enumSelezioneVariabile.Selezione_TabellaCompleta,
                          "", "", objParametri)

        'Se il recordset non è chiuso allora ...
        If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
            Return dt.Rows(0).Item("Localita")
        End If

    End Function

End Class
