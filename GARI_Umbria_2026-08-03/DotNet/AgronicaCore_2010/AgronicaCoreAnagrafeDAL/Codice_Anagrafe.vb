Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.UtilityProvider

Public Class Codice_Anagrafe_R
    Inherits AgronicaCoreDataProvider.DataProvider



    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="IdCod"></param>
    ''' <param name="TipoEntita">Attivo su codici di esercizio in reg_impianti_Codici al momento</param>
    ''' <param name="ApplicaFiltroUtentiVisibilita"></param>
    ''' <param name="validita_inizio"></param>
    ''' <param name="validita_fine"></param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="xOrderBy"></param>
    ''' <param name="objParametri"></param>
    ''' <returns></returns>
    Public Function LeggiValCodDatoIdCod(
        ByVal IdCod As Integer,
        ByVal TipoEntita As Integer,
        ByVal ApplicaFiltroUtentiVisibilita As Boolean,
        ByVal validita_inizio As Date,
        ByVal validita_fine As Date,
        ByVal xFiltroAggiuntivo As String,
        ByVal xOrderBy As String,
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
    ) As DataTable

        Const nomeRoutine As String = "AgronicaCoreAnagrafeDAL.Codice_Anagrafe.Leggi()"

        Dim messaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim dt As DataTable

        Try

            stb.AppendLine(" select  ")
            stb.AppendLine("    id_Cod ")
            stb.AppendLine("  , val_cod ")
            stb.AppendLine(" from reg_impianti_codici ic ")
            stb.AppendLine("            inner join imprese_progetti e ")
            stb.AppendLine("      on ic.piva = e.piva ")
            stb.AppendLine("      and ic.sa_cod = e.sa_cod ")
            stb.AppendLine("      and ic.appezza = e.appezza ")
            stb.AppendLine("      and ic.id_reg = e.id_reg ")
            stb.AppendLine("      and ic.progetto_cod = e.progetto_cod")

            If ApplicaFiltroUtentiVisibilita Then
                stb.AppendLine("  inner join Utenti_Visibilita_Appoggio a (NOLOCK) ")
                stb.AppendLine("      on a.piva = ic.piva      ")
            End If

            stb.AppendLine(" where id_cod =  " & IdCod)
            If ApplicaFiltroUtentiVisibilita Then
                stb.AppendLine(" and a.Username = '" & objParametri.UtenteUsername & "' ")
            End If

            stb.AppendLine(" group by  ")
            stb.AppendLine("    id_cod ")
            stb.AppendLine("  , val_cod")


            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    Public Function Filtro_Codici_Anagrafe(ByVal TipoEntita As Integer,
                                           ByVal Flag_Obbligatorio_Opzionale_Ricerca As Integer,
                                           ByVal TipoSeparatore As Integer,
                                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                           ) As String

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Codice_Anagrafe_R.Filtro_Codici_Anagrafe()"

        Dim strCodiciInvestimento As String = ""
        Dim Filtro_Codici_Anagrafe_locale As String = ""
        Dim DT_CodiciInvestimento As DataTable
        Dim i As Integer

        'centro
        Dim codici_centro_apofruit As String = " 1176,1177,1178,1179,1180,1181,1182,1183,1184,1185,1186,1262,1241,1242,1243,1244,1245,1246,1247,1248,1249,1250,1251,1252,1253,1254,1255,1256,1257,1258,1259,1260 "
        'impianto
        Dim codici_impianto_apofruit As String = " 1214,1187,1188,1189,1190,1191,1192,1193,1194,1195,1196,1197,1198,1199,1200,1201,1202,1203,1204,1205,1206 "
        'impresa
        Dim codici_impresa_apofruit As String = " 1213,1207,1208,1209,1210,1211,1212,1135,1136,1137,1138,1139,1140,1141,1142,1143,1144,1145,1146,1147,1148,1149,1150,1151,1152,1153,1154,1155,1156,1157,1158,1160,1161,1162,1163,1164,1165,1166,1167,1168,1169,1170,1171,1172,1173,1174,1175,1215,1216,1217,1218,1219,1220,1221,1222,1223,1224,1225,1226,1227,1229,1230,1231,1232,1233,1234,1235,1236,1237,1238,1239,1240,1261,1314 "
        'Costruisce il filtro sql da applicare al dt dei codici anagrafe

        Select Case TipoEntita

            Case 1 'IMPRESA

                If Flag_Obbligatorio_Opzionale_Ricerca = 1 Then
                    'Codici Obbligatori
                    Filtro_Codici_Anagrafe_locale = "1010"

                ElseIf Flag_Obbligatorio_Opzionale_Ricerca = 2 Then
                    'Codici Opzionali
                    Filtro_Codici_Anagrafe_locale = "1,2,3,4,5,1006,1007,1011,1016,1017,1018,1033,1086,1087,1088,1089,1090,1091,1092,1105,1106,1109,1111,1112,1113,1115,1116,1117,1118,1119,1123,1125,1126,1127,1128,1278,1206,1324,1317,1329,1304,1305,1309,1321,1348,1349,1114,1352,1353"

                    Filtro_Codici_Anagrafe_locale &= " ,  " & codici_impresa_apofruit


                ElseIf Flag_Obbligatorio_Opzionale_Ricerca = 3 Then
                    'la edit_impresa e la info_impresa passano di qui

                    'Codici validi per la Ricerca
                    Filtro_Codici_Anagrafe_locale = "1,2,3,4,5,1010,1011,1033,1086,1087,1088,1089,1090,1091,1092,1105,1106,1109,1111,1112,1113,1115,1116,1117,1118,1119,1123,1125,1126,1127,1128,1278,1206,1324,1317,1329,1304,1305,1335,1309,1321,1348,1349,1114,1354,1355"

                    Filtro_Codici_Anagrafe_locale &= " ,  " & codici_impresa_apofruit

                Else
                    'Tutti i Codici
                    Filtro_Codici_Anagrafe_locale = "1,2,3,4,5,1006,1007,1010,1011,1016,1017,1018,1033,1086,1087,1088,1089,1090,1091,1092,1105,1106,1109,1111,1112,1113,1115,1116,1117,1118,1119,1123,1125,1126,1127,1128,1278,1206,1324,1317,1329,1304,1305,1309,1321,1348,1349,1114,1354,1355"

                    Filtro_Codici_Anagrafe_locale &= " ,  " & codici_impresa_apofruit

                End If


            Case 2 'CENTRO AZIENDALE

                If Flag_Obbligatorio_Opzionale_Ricerca = 1 Then
                    'Codici Obbligatori
                    Filtro_Codici_Anagrafe_locale = "101,102,103"

                ElseIf Flag_Obbligatorio_Opzionale_Ricerca = 2 Then
                    'Codici Opzionali
                    Filtro_Codici_Anagrafe_locale = "1000,1003,1005,1008,1009,1016,1017,1018,1023,1024,1025,1026,1027,1028,1029,1030,1031,1287"

                    Filtro_Codici_Anagrafe_locale &= " ,  " & codici_centro_apofruit

                ElseIf Flag_Obbligatorio_Opzionale_Ricerca = 3 Then
                    'Codici validi per la Ricerca
                    Filtro_Codici_Anagrafe_locale = "1,2,3,4,5,1000,1003,1005,1008,1009,1023,1024,1025,1026,1027,1028,1029,1030,1031,1287"

                    Filtro_Codici_Anagrafe_locale &= " ,  " & codici_centro_apofruit

                Else
                    'Tutti i Codici
                    Filtro_Codici_Anagrafe_locale = "101,102,103,1000,1003,1005,1008,1009,1016,1017,1018,1023,1024,1025,1026,1027,1028,1029,1030,1031,1287"

                    Filtro_Codici_Anagrafe_locale &= " ,  " & codici_centro_apofruit

                End If


            Case 3 'APPEZZAMENTO

                If Flag_Obbligatorio_Opzionale_Ricerca = 1 Then
                    'Codici Obbligatori
                    Filtro_Codici_Anagrafe_locale = ""

                ElseIf Flag_Obbligatorio_Opzionale_Ricerca = 2 Then
                    'Codici Opzionali
                    Filtro_Codici_Anagrafe_locale = "1073,1014,1016,1018"

                ElseIf Flag_Obbligatorio_Opzionale_Ricerca = 3 Then
                    'Codici validi per la Ricerca
                    Filtro_Codici_Anagrafe_locale = "1073,1132"

                Else
                    'Tutti i Codici
                    Filtro_Codici_Anagrafe_locale = "1073,1014,1016,1018,1132"

                End If


            Case 4 'IMPIANTO


                '================================================================================================================================
                'Lettura Codici Investimento

                Dim objCodiceAnagrafe As New AgronicaCoreAnagrafeDAL.Codice_Anagrafe_R

                DT_CodiciInvestimento = objCodiceAnagrafe.Leggi(0, "IMPIANTO",
                                                                enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                "", "", objParametri)

                If DT_CodiciInvestimento.Rows.Count <> 0 Then

                    For i = 0 To DT_CodiciInvestimento.Rows.Count - 1

                        If CInt(DT_CodiciInvestimento.Rows(i).Item("Codice")) >= 10000 AndAlso
                           CInt(DT_CodiciInvestimento.Rows(i).Item("Codice")) < 100000 Then

                            'Filtro i Codici Investimento

                            strCodiciInvestimento &= "," & CStr(DT_CodiciInvestimento.Rows(i).Item("Codice"))

                        End If


                    Next


                End If


                '================================================================================================================================


                If Flag_Obbligatorio_Opzionale_Ricerca = 1 Then
                    'Codici Obbligatori
                    Filtro_Codici_Anagrafe_locale = "1050,1051,1052,1053,1054,1055,1056,1057,1058,1059,1060,1061,1062,1063,1064,1065,1066,1067,1068,1069,1070"

                ElseIf Flag_Obbligatorio_Opzionale_Ricerca = 2 Then
                    'Codici Opzionali
                    Filtro_Codici_Anagrafe_locale = "1018,1071,1072,1074,1078,1093,1124,1130,1339"

                    'Aggiungo i Codici Investimento
                    Filtro_Codici_Anagrafe_locale &= strCodiciInvestimento

                    Filtro_Codici_Anagrafe_locale &= " ,  " & codici_impianto_apofruit

                ElseIf Flag_Obbligatorio_Opzionale_Ricerca = 3 Then
                    'Codici validi per la Ricerca
                    Filtro_Codici_Anagrafe_locale = "1018,1071,1072,1074,1078,1093,1108,1124,1130,1131,1328,1339,1359,1359"

                    'Aggiungo i Codici Investimento
                    Filtro_Codici_Anagrafe_locale &= strCodiciInvestimento

                    Filtro_Codici_Anagrafe_locale &= " ,  " & codici_impianto_apofruit

                    Filtro_Codici_Anagrafe_locale &= " , 1287 , 1288, 1317, 1327 "

                Else
                    'Tutti i Codici
                    Filtro_Codici_Anagrafe_locale = "1018,1050,1051,1052,1053,1054,1055,1056,1057,1058,1059,1060,1061,1062,1063,1064,1065,1066,1067,1068,1069,1070,1071,1072,1074,1078,1093,1108,1124,1130,1131,1339"

                    'Aggiungo i Codici Investimento
                    Filtro_Codici_Anagrafe_locale &= strCodiciInvestimento

                    Filtro_Codici_Anagrafe_locale &= " ,  " & codici_impianto_apofruit

                End If


            Case 5 'PERSONA

                Filtro_Codici_Anagrafe_locale = "1019,1020,1021,1022"

        End Select

        '----------------------------------------------------------------------------------------------

        If Trim(Filtro_Codici_Anagrafe_locale) <> "" Then

            'Elimino " "
            Filtro_Codici_Anagrafe_locale = Replace(Filtro_Codici_Anagrafe_locale, " ", "")

            Select Case TipoSeparatore

                Case 1 'Virgola

                    'Do Nothing

                Case 2 'OR

                    Filtro_Codici_Anagrafe_locale = "Codice = " & Replace(Filtro_Codici_Anagrafe_locale, ",", " Or Codice = ")

            End Select

        End If

        Return Filtro_Codici_Anagrafe_locale

    End Function


    '#########################################################################
    Public Function Leggi(ByVal Codice As Integer,
                          ByVal Gruppo As String,
                          ByVal xSelezioneVariabile As enumSelezioneVariabile,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                          ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Codice_Anagrafe.Leggi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim dt As DataTable

        Try
            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi



                Case enumSelezioneVariabile.Selezione_TabellaCompleta
                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT   *")
                    StrSQL.Append(" FROM     Codici_Anagrafe WITH(NOLOCK)")
                    StrSQL.Append(" WHERE   (Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & ")  ")
                    StrSQL.Append(" AND     (Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & ") ")


                    If Codice <> 0 Then
                        StrSQL.Append(" AND     (Codice = " & Agro_SQL_SaveNum(Codice) & ")   ")
                    End If

                    If Gruppo <> "" Then
                        StrSQL.Append(" AND    (Gruppo = '" & Agro_SQL_SaveText(Gruppo) & "')   ")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND    (" & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri) & ")   ")
                    End If


                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   (inviato >= 0) ")

                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   (inviato =-1) ")

                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select


                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY Codici_Anagrafe.descrizione  ")
                    End If



                Case enumSelezioneVariabile.Selezione_JoinDescrizioni
                    '
                    '
                    '
                    '


                Case enumSelezioneVariabile.Selezione_JoinCompleta
                    '
                    '
                    '
                    '


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


    '#########################################################################
    Public Function Leggi_2(ByVal Codice As Long,
                            ByVal Gruppo As String,
                            ByVal xSelezioneVariabile As enumSelezioneVariabile,
                            ByVal xFiltroAggiuntivo As String,
                            ByVal xOrderBy As String,
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                            ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Codice_Anagrafe.Leggi_2()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim dt As DataTable

        Try
            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi


                Case enumSelezioneVariabile.Selezione_TabellaCompleta
                    StrSQL.Length = 0

                    StrSQL.Append(" SELECT * ")
                    StrSQL.Append(" FROM  Codici_Anagrafe ")
                    StrSQL.Append(" WHERE Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND   Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.Append(" AND   Inviato >=0 ")

                    If Codice <> 0 Then
                        StrSQL.Append(" AND Codice = " & Agro_SQL_SaveNum(Codice & " "))
                    End If

                    If Trim(Gruppo) <> "" Then
                        StrSQL.Append(" AND Gruppo = '" & Agro_SQL_SaveText(UCase(Gruppo)) & "' ")
                    End If


                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND    (" & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri) & ")   ")
                    End If

                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   (inviato >= 0) ")

                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   (inviato =-1) ")

                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select


                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY Codici_Anagrafe.descrizione  ")
                    End If



                Case enumSelezioneVariabile.Selezione_JoinDescrizioni
                    '
                    '
                    '
                    '


                Case enumSelezioneVariabile.Selezione_JoinCompleta
                    '
                    '
                    '
                    '


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

    '#########################################################################
    Public Function CodiceAnagrafeDes_from_CodiceAnagrafeCod(
                            ByVal Codice As Integer,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                ) As String

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Codice_Anagrafe.CodiceAnagrafeDes_from_CodiceAnagrafeCod()"

        Dim messaggioErrore As String = ""
        Dim dt As DataTable

        dt = Leggi(Codice, "",
                   enumSelezioneVariabile.Selezione_TabellaCompleta,
                   "", "", objParametri)

        Dim ritorno As String = ""

        If Not IsNothing(dt) AndAlso dt.Rows.Count <> 0 Then
            ritorno = CStr(dt.Rows(0).Item("descrizione"))
        End If

        Return ritorno

    End Function

    Public Function Centro_Tipologia_Leggi(ByVal xFiltroAggiuntivo As String,
                                           ByVal xOrderBy As String,
                                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                           ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Codice_Anagrafe.Centro_Tipologia_Leggi()"

        Dim messaggioErrore As String = ""
        Dim dt As DataTable

        Try

            dt = Leggi(0, "",
                       enumSelezioneVariabile.Selezione_TabellaCompleta,
                       "creatore = 'CSA' and gruppo = 'TIPO_CA'",
                       "",
                       objParametri)

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function


End Class
