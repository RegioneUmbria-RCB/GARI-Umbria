Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate


Public Class CentrixRubrica_Read
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function LeggiCentroSpecifico(ByVal Piva As String,
                                         ByVal sa_cod As Integer,
                                         ByVal Cod_Rubrica As Integer,
                                         ByVal xSelezioneVariabile As enumSelezioneVariabile,
                                         ByVal xFiltroAggiuntivo As String,
                                         ByVal xOrderBy As String,
                                         ByRef objParametri As AgronicaCoreParametri
                                         ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.CentrixRubrica_Read.LeggiCentroSpecifico()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try
            If Piva = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva obbligatoria)")
            End If

            If sa_cod = 0 Then
                Throw New Exception("Parametro non corretto nella query (Cod_Contatto obbligatorio)")
            End If

            'Select Case xSelezioneVariabile

            '    Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi


            '    Case enumSelezioneVariabile.Selezione_TabellaCompleta


            '    Case enumSelezioneVariabile.Selezione_JoinDescrizioni


            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append("  SELECT * " & _
          " FROM  CentrixRubrica, Rubrica " & _
          " WHERE CentrixRubrica.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " " & _
          " AND   CentrixRubrica.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " " & _
          " AND   Rubrica.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " " & _
          " AND   Rubrica.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " " & _
          " AND   CentrixRubrica.Cod_Rubrica = Rubrica.Cod_Rubrica " & _
          " AND   CentrixRubrica.Piva = '" & Agro_SQL_SaveText(Piva) & "' " & _
          " AND   CentrixRubrica.sa_cod =  " & Agro_SQL_SaveNum(sa_cod) & "  ")

            If Cod_Rubrica <> 0 Then
                StrSQL.Append(" AND CentrixRubrica.Cod_Rubrica = " & Agro_SQL_SaveNum(Cod_Rubrica) & " ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND     Rubrica.Inviato >= 0 ")
                    StrSQL.Append(" AND     CentrixRubrica.Inviato >= 0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND     Rubrica.Inviato = -1 ")
                    StrSQL.Append(" AND     CentrixRubrica.Inviato = -1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select

            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '    Case enumSelezioneVariabile.Selezione_JoinCompleta

            'End Select
            '---------------------------------------------

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
    Public Function Leggi(ByVal Piva As String,
                          ByVal Sa_Cod As Int32,
                          ByVal Cod_Rubrica As Int32,
                          ByVal xSelezioneVariabile As enumSelezioneVariabile,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreParametri
                          ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.CentrixRubrica_Read.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""                   =>  si leggono tutte le imprese
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try

            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                    StrSQL.Append(" SELECT        Rubrica.cod_rubrica, Rubrica.numero, Rubrica.descr, CentrixRubrica.sa_cod, CentrixRubrica.PIVA ")
                    StrSQL.Append(" FROM            Rubrica INNER JOIN ")
                    StrSQL.Append(" CentrixRubrica ON Rubrica.cod_rubrica = CentrixRubrica.cod_rubrica ")

                    StrSQL.Append(" WHERE CentrixRubrica.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND   CentrixRubrica.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.Append(" AND   Rubrica.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND   Rubrica.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.Append(" AND   CentrixRubrica.Cod_Rubrica = Rubrica.Cod_Rubrica ")
                    StrSQL.Append(" AND   CentrixRubrica.Piva = '" & Agro_SQL_SaveText(Piva) & "'")

                    If Sa_Cod <> 0 Then
                        StrSQL.Append(" AND CentrixRubrica.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
                    End If

                    If Cod_Rubrica <> 0 Then
                        StrSQL.Append(" AND CentrixRubrica.Cod_Rubrica = " & Agro_SQL_SaveNum(Cod_Rubrica) & " ")
                    End If

                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   CentrixRubrica.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   CentrixRubrica.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY Rubrica.descr ASC")
                    End If

                Case enumSelezioneVariabile.Selezione_TabellaCompleta

                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT * ")
                    StrSQL.Append(" FROM  CentrixRubrica, Rubrica ")
                    StrSQL.Append(" WHERE CentrixRubrica.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND   CentrixRubrica.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.Append(" AND   Rubrica.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND   Rubrica.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.Append(" AND   CentrixRubrica.Cod_Rubrica = Rubrica.Cod_Rubrica ")
                    StrSQL.Append(" AND   CentrixRubrica.Piva = '" & Agro_SQL_SaveText(Piva) & "'")

                    If Sa_Cod <> 0 Then
                        StrSQL.Append(" AND CentrixRubrica.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
                    End If

                    If Cod_Rubrica <> 0 Then
                        StrSQL.Append(" AND CentrixRubrica.Cod_Rubrica = " & Agro_SQL_SaveNum(Cod_Rubrica) & " ")
                    End If

                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   CentrixRubrica.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   CentrixRubrica.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY Rubrica.descr ASC")
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


    '################################################################################
    Public Function Anagrafica_Centri_Rubrica_Leggi(ByRef ErrMSG As String,
                                                    ByVal Piva As String,
                                                    ByVal Sa_Cod As Integer,
                                                    ByVal xSelezioneVariabile As enumSelezioneVariabile,
                                                    ByVal xFiltroAggiuntivo As String,
                                                    ByVal xOrderBy As String,
                                                    ByRef objParametri As AgronicaCoreParametri
                                                    ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.CentrixRubrica_Read.Anagrafica_Centri_Rubrica_Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""                   =>  si leggono tutte le imprese
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try
            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT  Imprese.Rag_Soc, Centri_Aziendali.piva, Centri_Aziendali.sa_cod, Centri_Aziendali.sa_nome, Rubrica.cod_rubrica, Rubrica.numero, Rubrica.descr ")
                    StrSQL.Append(" FROM    Centri_Aziendali INNER JOIN ")
                    StrSQL.Append("         Imprese ON Centri_Aziendali.PIVA = Imprese.PIVA INNER JOIN ")
                    StrSQL.Append("         Rubrica INNER JOIN ")
                    StrSQL.Append("         CentrixRubrica ON Rubrica.cod_rubrica = CentrixRubrica.cod_rubrica ON Centri_Aziendali.PIVA = CentrixRubrica.PIVA AND ")
                    StrSQL.Append("         Centri_Aziendali.sa_cod = CentrixRubrica.sa_cod ")
                    StrSQL.Append(" WHERE   1 = 1 ")

                    If Piva <> "" Then
                        StrSQL.Append(" AND   Centri_Aziendali.PIVA = '" & Agro_SQL_SaveText(Piva) & "' ")
                    End If

                    If Sa_Cod <> 0 Then
                        StrSQL.Append(" AND   Centri_Aziendali.sa_cod = " & Agro_SQL_SaveNum(Sa_Cod.ToString) & " ")
                    End If

                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   Centri_Aziendali.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Centri_Aziendali.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY Imprese.Rag_Soc, Centri_Aziendali.sa_nome, Rubrica.descr ")
                    End If

                Case enumSelezioneVariabile.Selezione_TabellaCompleta

                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT  Imprese.Rag_Soc, Centri_Aziendali.piva, Centri_Aziendali.sa_cod, Centri_Aziendali.sa_nome, Rubrica.cod_rubrica, Rubrica.numero, Rubrica.descr ")
                    StrSQL.Append(" FROM    Centri_Aziendali INNER JOIN ")
                    StrSQL.Append("         Imprese ON Centri_Aziendali.PIVA = Imprese.PIVA INNER JOIN ")
                    StrSQL.Append("         Rubrica INNER JOIN ")
                    StrSQL.Append("         CentrixRubrica ON Rubrica.cod_rubrica = CentrixRubrica.cod_rubrica ON Centri_Aziendali.PIVA = CentrixRubrica.PIVA AND ")
                    StrSQL.Append("         Centri_Aziendali.sa_cod = CentrixRubrica.sa_cod ")
                    StrSQL.Append(" WHERE   1 = 1 ")

                    If Piva <> "" Then
                        StrSQL.Append(" AND   Centri_Aziendali.PIVA = '" & Agro_SQL_SaveText(Piva) & "' ")
                    End If

                    If Sa_Cod <> 0 Then
                        StrSQL.Append(" AND   Centri_Aziendali.sa_cod = " & Agro_SQL_SaveNum(Sa_Cod.ToString) & " ")
                    End If

                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   Centri_Aziendali.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Centri_Aziendali.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY Imprese.Rag_Soc, Centri_Aziendali.sa_nome, Rubrica.descr ")
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

    '################################################################################
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="Piva"></param>
    ''' <param name="Sa_Cod"></param>
    ''' <param name="objParametri"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[magnani]	27/04/2011	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Function Recupera_Telefono_Centro(ByVal Piva As String,
                                             ByVal Sa_Cod As Integer,
                                             ByRef objParametri As AgronicaCoreParametri
                                             ) As String

        Dim dt As DataTable
        Dim strRet As String = ""

        dt = Leggi(CStr(Piva), CInt(Sa_Cod), 0,
                   enumSelezioneVariabile.Selezione_TabellaCompleta,
                   " descr like '%tel%'",
                   "", objParametri)

        If DT IsNot Nothing AndAlso DT.Rows.Count > 0 Then
            strRet = dt.Rows(0).Item("numero")
        End If

        Return strRet

    End Function

    ''' <summary>
    ''' funzione che prende come parametri un set di tuple (Piva, SaCod), 
    ''' e  tipologia che sarebbe il valore della colonna descr nella tabella Rubrica
    ''' </summary>
    ''' <param name="tuplePivaSaCod"></param>
    ''' <param name="tipologia"></param>
    ''' <param name="objParametri"></param>
    ''' <returns></returns>
    Public Function letturaCentri(tuplePivaSaCod As List(Of Tuple(Of String, Integer)),
                                  tipologia As String,
                                  ByRef objParametri As AgronicaCoreParametri
                                  ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.CentrixRubrica_R.letturaCentri()"
        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try
            StrSQL.Length = 0
            StrSQL.AppendLine(" SELECT PIVA , sa_cod, numero ")
            StrSQL.AppendLine("FROM CentrixRubrica")
            StrSQL.AppendLine(" INNER JOIN Rubrica on (CentrixRubrica.cod_rubrica = Rubrica.cod_rubrica) ")
            StrSQL.Append("WHERE descr = '").Append(tipologia).Append("'"c).AppendLine()
            'Dim strWhereClause As New Text.StringBuilder
            'strWhereClause.Length = 0
            'strWhereClause.Append("AND (")
            StrSQL.Append(tuplePivaSaCod.Aggregate(New Text.StringBuilder(" AND ("),
                        Function(sb, tupla) sb.Append(" (PIVA = '").Append(tupla.Item1).Append("' AND sa_cod = ").Append(tupla.Item2).AppendLine(") OR"),
                        Function(sb) sb.Remove(sb.Length - 4, 4).Append(" )").ToString))
            'tuplePivaSaCod.ForEach(Sub(tupla)
            '                           strWhereClause.Append(" (PIVA = '").Append(tupla.Item1) _
            '                                          .Append("' AND sa_cod = ").Append(tupla.Item2).AppendLine(") OR")
            '                       End Sub)
            'strWhereClause.Remove(strWhereClause.Length - 4, 4)
            'StrSQL.Append(strWhereClause.ToString).Append(")")
            '--------------------------------------------------------------------------

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

End Class

'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################

Public Class CentrixRubrica_Write
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function Scrivi(ByVal Piva As String,
                           ByVal Sa_Cod As Int32,
                           ByVal Cod_Rubrica As Int32,
                           ByVal Validita_Inizio As Date,
                           ByVal Validita_Fine As Date,
                           ByRef objParametri As AgronicaCoreParametri
                           ) As Boolean

        Const nomeRoutine As String = "AnagrafeCoreAnagrafeDAL.CentrixRubrica_Write.Scrivi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            
            StrSQL.Length = 0
            StrSQL.Append("INSERT INTO CentrixRubrica( ")
            StrSQL.Append("                    Piva,      ")
            StrSQL.Append("                    Sa_Cod,      ")
            StrSQL.Append("                    Cod_Rubrica,    ")
            StrSQL.Append("                    Inviato, DataInvio, ")
            StrSQL.Append("                    Data_Creazione,     Data_Modifica, ")
            StrSQL.Append("                    UserName_Creazione, UserName_Modifica, ")
            StrSQL.Append("                    Validita_Inizio,    Validita_Fine ")
            StrSQL.Append("                    ) ")
            StrSQL.Append("VALUES (")
            StrSQL.Append("          '" & Agro_SQL_SaveText(Piva) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Cod_Rubrica) & "  ")
            StrSQL.Append("         , 0  ")
            StrSQL.Append("         , Null  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Date.Now) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Date.Now) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")
            StrSQL.Append(")")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, nomeRoutine)
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
    Public Function Modifica(ByVal Piva As String,
                             ByVal Sa_Cod As Int32,
                             ByVal Cod_Rubrica As Int32,
                             ByVal Validita_Inizio As Date,
                             ByVal Validita_Fine As Date,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreParametri
                             ) As Boolean

        Const nomeRoutine  = "AnagrafeCoreAnagrafeDAL.CentrixRubrica_Write.Modifica()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            
            StrSQL.Length = 0
            StrSQL.Append("UPDATE CentrixRubrica SET ")
            StrSQL.Append("    Inviato           =  0 ")
            StrSQL.Append("   ,DataInvio         =  Null ")
            StrSQL.Append("   ,Data_Modifica     =  " & Agro_SQL_SaveDate(Date.Now))
            StrSQL.Append("   ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.Append("   ,Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Inizio))
            StrSQL.Append("   ,Validita_Fine     =  " & Agro_SQL_SaveDate(Validita_Fine))
            StrSQL.Append(" WHERE Piva = '" & Agro_SQL_SaveText(Piva) & "'")
            StrSQL.Append(" AND   Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            StrSQL.Append(" AND   Cod_Rubrica = " & Agro_SQL_SaveNum(Cod_Rubrica) & " ")

            '----------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, nomeRoutine)
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
    Public Function Cancella(ByVal Piva As String,
                             ByVal Sa_Cod As Int32,
                             ByVal cod_rubrica As Int32,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreParametri
                             ) As Boolean

        Const nomeRoutine = "AnagrafeCoreAnagrafeDAL.CentrixRubrica_Write.Cancella()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then

                StrSQL.Length = 0
                StrSQL.Append(" UPDATE CentrixRubrica ")
                StrSQL.Append(" SET ")
                StrSQL.Append("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("      ,Inviato = -1 ")
                StrSQL.Append(" WHERE  Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
                StrSQL.Append(" AND Inviato >= 0 ")

                If Sa_Cod <> 0 Then
                    StrSQL.Append(" AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
                End If

                If cod_rubrica <> 0 Then
                    StrSQL.Append(" AND Cod_Rubrica = " & Agro_SQL_SaveNum(cod_rubrica) & " ")
                End If

            Else

                StrSQL.Length = 0
                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM     CentrixRubrica ")
                StrSQL.Append(" WHERE    Piva = '" & Agro_SQL_SaveText(Piva) & "' ")

                If Sa_Cod <> 0 Then
                    StrSQL.Append(" AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
                End If

                If cod_rubrica <> 0 Then
                    StrSQL.Append(" AND Cod_Rubrica = " & Agro_SQL_SaveNum(cod_rubrica) & " ")
                End If

            End If
            '---------------------------------------------

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, nomeRoutine)
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
    Public Function AggiornaValiditaInizio(ByVal PIVA As String,
                                           ByVal Sa_Cod As Int32,
                                           ByVal cod_rubrica As Int32,
                                           ByVal Validita_Inizio As Date,
                                           ByVal xFiltroAggiuntivo As String,
                                           ByRef objParametri As AgronicaCoreParametri
                                           ) As Boolean

        Const nomeRoutine = "AnagrafeCoreAnagrafeDAL.CentrixRubrica_Write.AggiornaValiditaInizio()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            
            StrSQL.Length = 0
            StrSQL.Append("UPDATE CentrixRubrica SET ")
            StrSQL.Append("   UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.Append("   ,Validita_Inizio   =  " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio))
            StrSQL.Append(" WHERE Piva = '" & Agro_SQL_SaveText(PIVA) & "' ")
            StrSQL.Append(" AND   Validita_Inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio))

            If Sa_Cod <> 0 Then
                StrSQL.Append(" AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            End If

            If cod_rubrica <> 0 Then
                StrSQL.Append(" AND Cod_Rubrica = " & Agro_SQL_SaveNum(cod_rubrica) & " ")
            End If

            '----------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, nomeRoutine)
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
    Public Function AggiornaValiditaFine(ByVal PIVA As String,
                                         ByVal Sa_Cod As Int32,
                                         ByVal cod_rubrica As Int32,
                                         ByVal Validita_Fine As Date,
                                         ByVal xFiltroAggiuntivo As String,
                                         ByRef objParametri As AgronicaCoreParametri
                                         ) As Boolean

        Const nomeRoutine = "AnagrafeCoreAnagrafeDAL.CentrixRubrica_Write.AggiornaValiditaFine()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            
            StrSQL.Length = 0
            StrSQL.Append("UPDATE CentrixRubrica SET ")
            StrSQL.Append("   UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.Append("   ,Validita_Fine   =  " & Agro_SQL_SaveDate(Validita_Fine))
            StrSQL.Append(" WHERE Piva = '" & Agro_SQL_SaveText(PIVA) & "' ")
            StrSQL.Append(" AND   Validita_Fine > " & Agro_SQL_SaveDate(Validita_Fine))

            If Sa_Cod <> 0 Then
                StrSQL.Append(" AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            End If

            If cod_rubrica <> 0 Then
                StrSQL.Append(" AND Cod_Rubrica = " & Agro_SQL_SaveNum(cod_rubrica) & " ")
            End If

            '----------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function

    Public Function Aggiungi_Telefono(ByVal PIVA As String,
                                      ByVal SA_COD As Integer,
                                      ByVal telefono As String,
                                      ByVal Cod_Rubrica_Ret As Integer,
                                      ByVal objParametri As AgronicaCoreParametri
                                      ) As Boolean

        Const nomeRoutine = "AnagrafeCoreAnagrafeDAL.CentrixRubrica_Write.Aggiungi_Telefono()"

        Dim messaggioErrore As String = ""
        Dim xRisp As Boolean = False

        Cod_Rubrica_Ret = 0

        Try

            'per evitare di eliminare tutto
            If PIVA = "" Then
                Throw New Exception("Occorre specificare una Piva")
            End If
            If SA_COD = 0 Then
                Throw New Exception("Occorre specificare una Piva")
            End If

            Dim objCentrixRubricaRead As New AgronicaCoreAnagrafeDAL.CentrixRubrica_Read
            Dim dt As DataTable = objCentrixRubricaRead.LeggiCentroSpecifico(PIVA, SA_COD,
                                          0, enumSelezioneVariabile.Selezione_JoinCompleta,
                                          " rubrica.numero = '" & Agro_SQL_SaveText(telefono.Replace("'", "")) & "' ",
                                          "", objParametri)

            '---------------------------------------------
            Dim objRubricaWrite As New AgronicaCoreAnagrafeDAL.Rubrica_Write

            Dim codRubrica As Integer = 0
            If dt.Rows.Count = 0 Then
                'scrivo il numero che mi ritorna il codice codice
                xRisp = objRubricaWrite.Aggiungi_Aggiorna(codRubrica, telefono, "Telefono", AGRODATAINIZIO, AGRODATAFINE, objParametri)
                'scrivo la voce in centri per rubrica
                If xRisp Then
                    xRisp = Scrivi(PIVA, SA_COD, codRubrica, AGRODATAINIZIO, AGRODATAFINE, objParametri)
                End If
            ElseIf dt.Rows.Count > 0 Then
                'il numero c'è già, non faccio nulla
                'For i As Integer = 0 To dt.Rows.Count - 1
                '    'codRubrica = dt.Rows(i).Item("Cod_Rubrica")
                '    ' xRisp = xRisp Or objRubricaWrite.Aggiungi_Aggiorna(codRubrica, telefono, "Telefono", AGRODATAINIZIO, AGRODATAFINE, objParametri)
                'Next
            Else
                Throw New Exception("")
            End If

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
