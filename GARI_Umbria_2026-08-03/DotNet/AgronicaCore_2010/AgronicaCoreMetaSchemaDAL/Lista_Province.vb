Imports System.Data.Common
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.DataProviderExtensions


Public Class Lista_Province_R
    Inherits AgronicaCoreDataProvider.DataProvider


    '##############################################################################################
    'vecchia, non usare
    'Public Function Leggi(ByVal Sigla As String,
    '                      ByVal Reg As String,
    '                      ByVal FinestraTemp_Inizio As Date,
    '                      ByVal FinestraTemp_Fine As Date,
    '                      ByRef objConnessione As DbConnection,
    '                      ByVal StringaConnessione As String,
    '                      ByVal FlagVisibilita As Int32,
    '                      ByVal DirectoryLOG As String,
    '                      ByVal FileLOG As String,
    '                      ByVal IdentificatoreUtente As String
    '                      ) As DataTable

    '    Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Lista_Province_R.Leggi()"

    '    '====================================================================================
    '    'Parametri opzionali :

    '    '
    '    '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
    '    '   DirectoryLOG = ""           =>  viene usato il valore di default
    '    '   FileLOG = ""                =>  viene usato il valore di default
    '    '====================================================================================

    '    Dim messaggioErrore As String = ""
    '    Dim StrSQL As New Text.StringBuilder
    '    Dim dt As DataTable

    '    Try
    '        '---------------------------------------------

    '        StrSQL.Length = 0
    '        StrSQL.Append(" SELECT * ")
    '        StrSQL.Append(" FROM  Lista_Province ")
    '        StrSQL.Append(" WHERE Validita_inizio < " & Agro_SQL_SaveDate(FinestraTemp_Fine) & " ")
    '        StrSQL.Append(" AND   Validita_Fine > " & Agro_SQL_SaveDate(FinestraTemp_Inizio) & " ")

    '        If Sigla <> "" Then
    '            StrSQL.Append(" AND Sigla = '" & Agro_SQL_SaveText(Trim(Sigla)) & "' ")
    '        End If

    '        If Reg <> "" Then
    '            StrSQL.Append(" AND Reg = '" & Agro_SQL_SaveText(Trim(Reg)) & "' ")
    '        End If


    '        Select Case FlagVisibilita
    '            Case 1  'Solo i NON CANCELLATI
    '                StrSQL.Append(" AND     Inviato >= 0 ")
    '                StrSQL.Append(" AND     Inviato >= 0 ")
    '            Case 2  'Solo i CANCELLATI
    '                StrSQL.Append(" AND     Inviato = -1 ")
    '                StrSQL.Append(" AND     Inviato = -1 ")
    '            Case 3  'TUTTI

    '            Case Else
    '                Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
    '        End Select

    '        StrSQL.Append(" ORDER BY Provincia ")

    '        '---------------------------------------------

    '        dt = EseguiQuery_Lettura(objConnessione, StringaConnessione, StrSQL.ToString, DirectoryLOG, FileLOG, IdentificatoreUtente, nomeRoutine)


    '    Catch ex As Exception
    '        messaggioErrore = ex.Message
    '        Scrivi_LOG(DirectoryLOG, FileLOG, IdentificatoreUtente, nomeRoutine, messaggioErrore)
    '        dt = Nothing
    '        Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
    '    End Try

    '    Return dt

    'End Function

    '#################################################
    'versione con objParametri ma poche condizioni
    Public Function Leggi(ByVal Sigla As String,
                          ByVal Reg As String,
                          ByVal xSelezioneVariabile As enumSelezioneVariabile,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreParametri,
                          Optional ByVal Stato_Country As String = "IT"
                          ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Lista_Province_R.Leggi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try
            '---------------------------------------------
            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi


                Case enumSelezioneVariabile.Selezione_TabellaCompleta

                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT * ")
                    StrSQL.Append(" FROM  Lista_Province ")
                    StrSQL.Append(" WHERE Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND   Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    If Sigla <> "" Then
                        StrSQL.Append(" AND Sigla = '" & Agro_SQL_SaveText(Trim(Sigla)) & "' ")
                    End If

                    If Reg <> "" Then
                        StrSQL.Append(" AND Reg = '" & Agro_SQL_SaveText(Trim(Reg)) & "' ")
                    End If

                    If Stato_Country <> "" Then

                        If UCase(Left(Stato_Country & " ", 3)) = "ITA" Then
                            Stato_Country = "IT"
                        End If

                        StrSQL.Append(" AND Upper(Stato_Country) = '" & UCase(Agro_SQL_SaveText(Trim(Stato_Country))) & "' ")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY Provincia ")
                    End If


                Case enumSelezioneVariabile.Selezione_JoinDescrizioni

                    StrSQL.AppendLine(" SELECT        Lista_Province.SIGLA, Lista_Province.REG, Lista_Province.PROV, Lista_Province.COM, Lista_Province.PROVINCIA, Lista_Regioni.Regione_Des, Lista_Regioni.Stato_Country ")
                    StrSQL.AppendLine(" FROM            Lista_Province INNER JOIN ")
                    StrSQL.AppendLine("        Lista_Regioni ON Lista_Province.REG = Lista_Regioni.REG AND Lista_Province.Stato_Country = Lista_Regioni.Stato_Country")

                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.AppendLine(" AND   Lista_Province.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.AppendLine(" AND   Lista_Province.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------

                    StrSQL.AppendLine(" WHERE Lista_Province.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.AppendLine(" AND   Lista_Province.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    If Stato_Country <> "" Then

                        If UCase(Left(Stato_Country & " ", 3)) = "ITA" Then
                            Stato_Country = "IT"
                        End If

                        StrSQL.AppendLine(" AND Upper(Lista_Province.Stato_Country) = '" & UCase(Agro_SQL_SaveText(Trim(Stato_Country))) & "' ")
                    End If

                    If Reg <> "" Then
                        StrSQL.AppendLine(" AND Lista_Province.Reg = '" & Agro_SQL_SaveText(Trim(Reg)) & "' ")
                    End If

                    If xOrderBy <> "" Then
                        StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.AppendLine("  order by regione_des , provincia ")
                    End If

                Case enumSelezioneVariabile.Selezione_JoinCompleta


            End Select

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



    '#################################################
    'più clausole where
    Public Function Leggi(ByVal Sigla As String,
                          ByVal Reg As String,
                          ByVal Prov As String,
                          ByVal Com As String,
                          ByVal Provincia As String,
                          ByVal xSelezioneVariabile As enumSelezioneVariabile,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreParametri,
                          Optional ByVal Stato_Country As String = "IT"
                          ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Lista_Province_R.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try
            '---------------------------------------------
            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi


                Case enumSelezioneVariabile.Selezione_TabellaCompleta

                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT * ")
                    StrSQL.Append(" FROM  Lista_Province ")
                    StrSQL.Append(" WHERE Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND   Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")


                    If Sigla <> "" Then
                        StrSQL.Append(" AND Sigla = '" & Agro_SQL_SaveText(Sigla) & "' ")
                    End If

                    If Reg <> "" Then
                        StrSQL.Append(" AND Reg = '" & Agro_SQL_SaveText(Reg) & "' ")
                    End If

                    If Prov <> "" Then
                        StrSQL.Append(" AND Prov = '" & Agro_SQL_SaveText(Prov) & "' ")
                    End If

                    If Com <> "" Then
                        StrSQL.Append(" AND Com = '" & Agro_SQL_SaveText(Com) & "' ")
                    End If

                    If Provincia <> "" Then
                        StrSQL.Append(" AND Provincia = '" & Agro_SQL_SaveText(Provincia) & "' ")
                    End If

                    If Stato_Country <> "" Then

                        If UCase(Left(Stato_Country & " ", 3)) = "ITA" Then
                            Stato_Country = "IT"
                        End If

                        StrSQL.Append(" AND Stato_Country = '" & Agro_SQL_SaveText(Stato_Country) & "' ")
                    End If


                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY Provincia ")
                    End If


                Case enumSelezioneVariabile.Selezione_JoinDescrizioni



                Case enumSelezioneVariabile.Selezione_JoinCompleta



            End Select

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

    '#################################################
    Public Function Provincia_from_Sigla(ByVal Sigla As String,
                                         ByVal xFiltroAggiuntivo As String,
                                         ByRef objParametri As AgronicaCoreParametri
                                         ) As String

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Contatti_R.LegaleRappresentante_from_PivaImpresa()"

        '====================================================================================
        'Parametri opzionali :
        '
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim dt As DataTable
        Dim provincia As String

        Try

            provincia = ""

            dt = Leggi(Sigla, "",
                       enumSelezioneVariabile.Selezione_TabellaCompleta,
                       xFiltroAggiuntivo, "", objParametri)

            If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then
                provincia = dt.Rows(0).Item("Provincia")
            End If

            dt = Nothing

        Catch ex As Exception
            provincia = ""
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return provincia

    End Function


    '################################################################################
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="ProvinciaSigla"></param>
    ''' <param name="ProvinciaCod"></param>
    ''' <param name="RegioneCod"></param>
    ''' <param name="RegioneSigla"></param>
    ''' <param name="objParametri"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[magnani]	26/04/2011	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Function Regione_from_Provincia(ByVal ProvinciaSigla As String,
                                           ByVal ProvinciaCod As String,
                                           ByRef RegioneCod As String,
                                           ByRef RegioneSigla As String,
                                           ByRef objParametri As AgronicaCoreParametri
                                           ) As String

        'NOTE
        'Basta valorizzare almeno uno dei due parametri di ingresso !!!!!!

        Dim xRegioneCod As String
        Dim xRegioneDes As String
        Dim xRegioneSigla As String
        Dim xProvinciaSigla As String
        Dim xProvinciaDes As String

        Dim Istat As New Istat_R

        'Verifico quale dei due parametri di ingresso è non nullo
        If ProvinciaSigla <> "" Then
            xProvinciaSigla = ProvinciaSigla
        Else
            xProvinciaDes = Istat.Provincia_from_CodIstat(ProvinciaCod, xProvinciaSigla, objParametri)
        End If

        Dim Rs As DataTable

        'Recupero le informazioni
        Rs = Leggi(CStr(xProvinciaSigla), "",
                   enumSelezioneVariabile.Selezione_TabellaCompleta,
                   "", "", objParametri)

        'Se il recordset non è chiuso allora ...
        If Rs IsNot Nothing AndAlso Rs.Rows.Count > 0 Then
            xRegioneCod = Rs.Rows(0).Item("Reg")
        End If

        'Elimino il recordset
        Rs = Nothing

        xRegioneDes = RegioneDes_from_RegioneCod(xRegioneCod, xRegioneSigla)

        'Restituisco i risultati
        RegioneCod = xRegioneCod
        RegioneSigla = xRegioneSigla

        Return xRegioneDes

    End Function

    '################################################################################
    Public Function RegioneDes_from_RegioneCod(ByVal RegioneCod As String,
                                               ByRef RegioneSigla As String
                                               ) As String

        Dim xRegioneCod As String
        Dim des As String

        'Formatto correttamente il codice (3 caratteri)
        If Len(RegioneCod) < 3 Then
            xRegioneCod = Right("000" & RegioneCod, 3)
        Else
            xRegioneCod = RegioneCod
        End If

        Select Case xRegioneCod

            Case "000"
                des = "Codex : Sede Centrale"
                RegioneSigla = "SC"
            Case "001"
                des = "Piemonte"
                RegioneSigla = "PI"
            Case "002"
                des = "Valle d'Aosta"
                RegioneSigla = "VD"
            Case "003"
                des = "Lombardia"
                RegioneSigla = "LO"
            Case "004"
                des = "Trentino Alto Adige"
                RegioneSigla = "TAA"
            Case "005"
                des = "Veneto"
                RegioneSigla = "VE"
            Case "006"
                des = "Friuli Venezia Giulia"
                RegioneSigla = "FVG"
            Case "007"
                des = "Liguria"
                RegioneSigla = "LI"
            Case "008"
                des = "Emilia Romagna"
                RegioneSigla = "ER"
            Case "009"
                des = "Toscana"
                RegioneSigla = "T"
            Case "010"
                des = "Umbria"
                RegioneSigla = "U"
            Case "011"
                des = "Marche"
                RegioneSigla = "MA"
            Case "012"
                des = "Lazio"
                RegioneSigla = "LA"
            Case "013"
                des = "Abruzzo"
                RegioneSigla = "A"
            Case "014"
                des = "Molise"
                RegioneSigla = "MO"
            Case "015"
                des = "Campania"
                RegioneSigla = "CM"
            Case "016"
                des = "Puglia"
                RegioneSigla = "PU"
            Case "017"
                des = "Basilicata"
                RegioneSigla = "B"
            Case "018"
                des = "Calabria"
                RegioneSigla = "CL"
            Case "019"
                des = "Sicilia"
                RegioneSigla = "S"
            Case "020"
                des = "Sardegna"
                RegioneSigla = "SA"

        End Select

        'Restituisco il risultato
        Return des

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
    Public Function Numero_Totale_Province(ByRef objParametri As AgronicaCoreParametri) As Integer

        Dim numProvincie As Integer = 0

        Dim dt As DataTable

        dt = Leggi("", "", enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri)

        If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
            numProvincie = dt.Rows.Count
        End If

        Return numProvincie

    End Function

End Class
