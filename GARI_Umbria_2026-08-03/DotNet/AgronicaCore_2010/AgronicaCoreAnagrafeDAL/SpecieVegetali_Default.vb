Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate


Public Class SpecieVegetali_Default_W
    Inherits AgronicaCoreDataProvider.DataProvider

    '###############################################################
    Public Function Scrivi(ByRef Id As Integer,
                           ByVal Piva As String,
                           ByVal Veg_Cod As Integer,
                           ByVal Cul_Cod As Integer,
                           ByVal Codice As Integer,
                           ByVal Valore As String,
                           ByVal Numero_Ciclo As Integer,
                           ByVal Validita_Inizio As Date,
                           ByVal Validita_Fine As Date,
                           ByRef objParametri As AgronicaCoreParametri
                           ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.SpecieVegetali_Default_W.Scrivi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False

        'Dim ObjSequenze As Agro_Sequenze
        ''Dim day, month As String

        ''day = Valore.Day.ToString
        ''month = Valore.Month.ToString

        ''If day.Length = 1 Then
        ''    day = String.Format("0{0}", day)
        ''End If

        ''If month.Length = 1 Then
        ''    month = String.Format("0{0}", month)
        ''End If

        Try

            StrSQL.Length = 0
            StrSQL.Append("INSERT INTO SpecieVegetali_Default ")
            StrSQL.Append("                   ( PivaSuperUser        ,Id        ,Piva       ,Veg_Cod          ")
            StrSQL.Append("                    ,Cul_Cod     ,Codice       ,Valore, Numero_Ciclo,    ")

            StrSQL.Append("                    Inviato,            DataInvio, ")
            StrSQL.Append("                    Data_Creazione,     Data_Modifica, ")
            StrSQL.Append("                    UserName_Creazione, UserName_Modifica, ")
            StrSQL.Append("                    Validita_Inizio,    Validita_Fine ")
            StrSQL.Append("                   ) ")

            StrSQL.Append("VALUES (")
            StrSQL.Append("          '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Id) & "  ")
            StrSQL.Append("         , '" & Agro_SQL_SaveText(Piva) & "'  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Veg_Cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Cul_Cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Codice) & "  ")
            StrSQL.Append("         , '" & Agro_SQL_SaveText(Valore) & "'  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Numero_Ciclo) & "  ")
            'StrSQL.Append("         , '" & Agro_SQL_SaveText(day) & "/" & (month) & "'  ")
            StrSQL.Append("         , 0  ")
            StrSQL.Append("         , Null  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDateTime(Now) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDateTime(Now) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")
            StrSQL.Append(" )")

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


    Public Function Modifica(ByRef Id As Integer,
                             ByVal Piva As String,
                             ByVal Veg_Cod As Integer,
                             ByVal Cul_Cod As Integer,
                             ByVal Codice As Integer,
                             ByVal Valore As String,
                             ByVal Numero_Ciclo As Integer,
                             ByVal Validita_Inizio As Date,
                             ByVal Validita_Fine As Date,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreParametri
                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.SpecieVegetali_Default_W.Modifica()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False


        Try

            StrSQL.Length = 0

            StrSQL.Append("UPDATE SpecieVegetali_Default SET ")
            StrSQL.Append("    Piva          =  '" & Agro_SQL_SaveText(Piva) & "' ")
            StrSQL.Append("   ,Veg_Cod       =  " & Agro_SQL_SaveNum(Veg_Cod) & " ")
            StrSQL.Append("   ,Cul_Cod       =  " & Agro_SQL_SaveNum(Cul_Cod) & " ")
            StrSQL.Append("   ,Codice        =  " & Agro_SQL_SaveNum(Codice) & " ")
            StrSQL.Append("   ,Valore        = '" & Agro_SQL_SaveText(Valore) & "'  ")
            StrSQL.Append("   ,Numero_Ciclo  = " & Agro_SQL_SaveNum(Numero_Ciclo) & "  ")

            StrSQL.Append("   ,Inviato         =  0 ")
            StrSQL.Append("   ,DataInvio         =  Null ")
            StrSQL.Append("   ,Data_Modifica     =  " & Agro_SQL_SaveDateTime(Now))
            StrSQL.Append("   ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.Append("   ,Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Inizio) & " ")
            StrSQL.Append("   ,Validita_Fine     =  " & Agro_SQL_SaveDate(Validita_Fine) & " ")

            StrSQL.Append(" WHERE PivaSuperUser = '" & Trim(objParametri.PivaSuperUser) & "'")
            StrSQL.Append(" AND   Id = " & Id & " ")

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


    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="Piva"></param>
    ''' <param name="Veg_Cod"></param>
    ''' <param name="Cul_Cod">-1 se si vogliono cancellare tutte le varietà</param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="objParametri"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function Cancella(ByVal Piva As String,
                             ByVal Veg_Cod As Integer,
                             ByVal Cul_Cod As Integer,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreParametri
                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.SpecieVegetali_Default_W.Cancella()"

        '====================================================================================
        'Parametri opzionali :
        '   Id = 0           =>  si cancellano tutti i default dell'impresa superuser
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            'If Piva_SuperUser = "" Then
            '    Throw New Exception("Parametro non corretto nella query (PivaSuperUser obbligatorio)")
            'End If

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then

                StrSQL.Length = 0
                StrSQL.Append(" UPDATE SpecieVegetali_Default ")
                StrSQL.Append(" SET ")
                StrSQL.Append("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("      ,Inviato = -1 ")
                StrSQL.Append(" WHERE  PivaSuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
                StrSQL.Append(" AND Inviato >= 0")

            Else

                StrSQL.Length = 0
                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM     SpecieVegetali_Default ")
                StrSQL.Append(" WHERE    PivaSuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

            End If

            StrSQL.Append(" AND SpecieVegetali_Default.Piva = '" & Agro_SQL_SaveText(Piva) & "'")

            If Veg_Cod <> 0 Then
                StrSQL.Append(" AND SpecieVegetali_Default.Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod))
            End If

            If Cul_Cod <> -1 Then
                StrSQL.Append(" AND SpecieVegetali_Default.Cul_Cod = " & Agro_SQL_SaveNum(Cul_Cod))
            End If

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

End Class

'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################


Public Class SpecieVegetali_Default_R
    Inherits AgronicaCoreDataProvider.DataProvider


    '##############################################################################################
    'Legge la dose per ha nella semina impostate nella profilazione
    'Gli passo la variatà e l'udm, se la dose è specificata ma non l'udm allora ritorna UDM_Cod_Ritorno=0
    'se la dose non è specificata ritorna 0  in dose e restituisce false
    Public Function Leggi_Dosi_X_Ha_In_Cascata(ByVal Piva As String,
                          ByVal Veg_Cod As Integer,
                          ByVal Cul_Cod As Integer,
                          ByVal UDM_Cod As Integer,
                          ByVal Data_Validita As Date,
                          ByRef Dose_Ritorno As Decimal,
                          ByRef TrovataUDM As Boolean,
                          ByRef ImpostazioneAziendale As Boolean,
                          ByVal xFiltroAggiuntivo As String,
                          ByRef objParametri As AgronicaCoreParametri
                          ) As Boolean

        Dim trovata As Boolean = Leggi_Dosi_X_Ha(Piva, Veg_Cod, Cul_Cod, UDM_Cod,
                                                 Data_Validita, Dose_Ritorno, TrovataUDM,
                                                 xFiltroAggiuntivo, objParametri)

        If trovata Then
            ImpostazioneAziendale = True
            Return trovata
        Else
            ImpostazioneAziendale = False
            Return Leggi_Dosi_X_Ha("", Veg_Cod, Cul_Cod, UDM_Cod,
                                   Data_Validita, Dose_Ritorno, TrovataUDM,
                                   xFiltroAggiuntivo, objParametri)
        End If

    End Function

    '##############################################################################################
    'Legge la dose per ha nella semina impostate nella profilazione
    'Gli passo la variatà e l'udm, se la dose è specificata ma non l'udm allora ritorna UDM_Cod_Ritorno=0
    'se la dose non è specificata ritorna 0  in dose e restituisce false
    Public Function Leggi_Dosi_X_Ha(ByVal Piva As String,
                                    ByVal Veg_Cod As Integer,
                                    ByVal Cul_Cod As Integer,
                                    ByVal UDM_Cod As Integer,
                                    ByVal Data_Validita As Date,
                                    ByRef Dose_Ritorno As Decimal,
                                    ByRef TrovataUDM As Boolean,
                                    ByVal xFiltroAggiuntivo As String,
                                    ByRef objParametri As AgronicaCoreParametri
                                    ) As Boolean

        TrovataUDM = False

        'leggo l'udm della dose x ha
        Dim dtUdm As DataTable = Leggi(Piva, Veg_Cod, Cul_Cod, 18, 0,
                                       Data_Validita, Data_Validita,
                                       enumSelezioneVariabile.Selezione_TabellaCompleta,
                                       "", "", objParametri)

        'ci dovrebbe essere un solo udm cod, o al massimo nessuno
        If dtUdm.Rows.Count = 1 Then
            'se ce ne è uno controllo se è l'udm che sto cercando
            If CStr(UDM_Cod) = dtUdm.Rows(0).Item("valore") Then
                'se è l'udm che cerco proseguo e trovo il valore della dose

                'TROVATA UDM GIUSTA
                TrovataUDM = True

            ElseIf dtUdm.Rows(0).Item("valore") = "0" Then
                'se è l'udm che cerco è 0 allora proseguo perché la considero come non specificata e quindi potrei volere utilizzarla, 
                'cioè come se non fosse salvata l'udm per retro-compatibilità

                'SEGNALO ALL'ESTERNO CHE LA DOSE TROVATA NON CORRISPONDE ALLA UDM CERCATA, MA UDM è NON SPECIFICATA O ASSENTE
                TrovataUDM = False

            Else

                'se l'udm è specificata e non è quella che cerco esco e ritorno 0, 
                'dato che devo proseguire solo se l'udm è quella che cerco oppure non è specificata oppure è specificata con 0 (che indica non specificata)

                'ESCO E
                'SEGNALO ALL'ESTERNO CHE LA DOSE TROVATA NON CORRISPONDE ALLA UDM CERCATA
                Dose_Ritorno = 0
                TrovataUDM = False
                Return False

            End If

        ElseIf dtUdm.Rows.Count > 1 Then
            'Se ce ne è più di uno, per capire restituisco a UDM_Cod_Ritorno -1

            'ESCO
            Throw New Exception("Ci sono più valori con quella udm")

        Else
            'se non c'è nessun record per l'udm allora segnalo che non ho trovato l'udm specifica ma proseguo per cercare comunque la dose

            'SEGNALO ALL'ESTERNO CHE LA DOSE TROVATA NON CORRISPONDE ALLA UDM CERCATA, MA UDM è NON SPECIFICATA O ASSENTE
            TrovataUDM = False

        End If


        'leggo la dose per ha
        Dim dtuDose As DataTable = Leggi(Piva, Veg_Cod, Cul_Cod, 17, 0,
                                         Data_Validita, Data_Validita,
                                         enumSelezioneVariabile.Selezione_TabellaCompleta,
                                         "", "", objParametri)

        'ci dovrebbe essere un solo o al massimo nessuno
        If dtuDose.Rows.Count = 1 Then
            'se ce ne è uno lo restituisco 
            If IsNumeric(dtuDose.Rows(0).Item("Valore")) Then
                Dose_Ritorno = dtuDose.Rows(0).Item("Valore")
                Return True
            Else
                Throw New Exception("Dose non è numerica")
            End If
        ElseIf dtuDose.Rows.Count > 1 Then
            'Se ce ne è più di uno, per capire restituisco a  -1
            Throw New Exception("Ci sono più valori per quella varieta")
        Else
            'se non ce ne è nessuno ritorno "" 
            Dose_Ritorno = 0
            Return False
        End If

    End Function


    '##############################################################################################
    Public Function Leggi(ByVal Piva As String,
                          ByVal Veg_Cod As Integer,
                          ByVal Cul_Cod As Integer,
                          ByVal Codice As Integer,
                          ByVal Numero_Ciclo As Integer,
                          ByVal Validita_Inizio As Date,
                          ByVal Validita_Fine As Date,
                          ByVal xSelezioneVariabile As enumSelezioneVariabile,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreParametri
                          ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.SpecieVegetali_Default_R.Leggi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try
            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi


                Case enumSelezioneVariabile.Selezione_TabellaCompleta

                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT * ")
                    StrSQL.Append(" FROM  SpecieVegetali_Default ")
                    StrSQL.Append(" WHERE SpecieVegetali_Default.PivaSuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

                    If Piva <> "" Then
                        StrSQL.Append(" AND SpecieVegetali_Default.Piva = '" & Agro_SQL_SaveText(Piva) & "'")
                    End If

                    If Veg_Cod <> 0 Then
                        StrSQL.Append(" AND SpecieVegetali_Default.Veg_Cod = " & Veg_Cod & " ")
                    End If
                    If Cul_Cod <> 0 Then
                        StrSQL.Append(" AND SpecieVegetali_Default.Cul_Cod = " & Cul_Cod)
                    End If
                    If Codice <> 0 Then
                        StrSQL.Append(" AND SpecieVegetali_Default.Codice = " & Codice & " ")
                    End If
                    StrSQL.Append(" AND Validita_Inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
                    StrSQL.Append(" AND Validita_Fine >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")

                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   SpecieVegetali_Default.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   SpecieVegetali_Default.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    End If

                Case enumSelezioneVariabile.Selezione_JoinDescrizioni

                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT     SpecieVegetali_Default.Veg_Cod, SpecieVegetali_Default.Cul_Cod, SpecieVegetali_Default.Codice, SpecieVegetali_Default.Valore,  ")
                    StrSQL.Append("         SpecieVegetali_Default.Numero_Ciclo, SpecieVegetali.Veg_Des, Cultivar.Cul_Des, SpecieVegetali.Gru_Cod ")
                    StrSQL.Append(" FROM         SpecieVegetali_Default LEFT OUTER JOIN ")
                    StrSQL.Append("         Cultivar ON SpecieVegetali_Default.Veg_Cod = Cultivar.Veg_Cod AND SpecieVegetali_Default.Cul_Cod = Cultivar.Cul_Cod INNER JOIN ")
                    StrSQL.Append("         SpecieVegetali ON SpecieVegetali_Default.Veg_Cod = SpecieVegetali.Veg_Cod ")

                    'If VerificaPiva = True Then
                    StrSQL.Append(" AND SpecieVegetali_Default.Piva = '" & Agro_SQL_SaveText(Piva) & "'")
                    'End If

                    If Veg_Cod <> 0 Then
                        StrSQL.Append(" AND SpecieVegetali_Default.Veg_Cod = " & Veg_Cod & " ")
                    End If

                    If Cul_Cod <> 0 Then
                        StrSQL.Append(" AND SpecieVegetali_Default.Cul_Cod = " & Cul_Cod)
                    End If

                    If Codice <> 0 Then
                        StrSQL.Append(" AND SpecieVegetali_Default.Codice = " & Codice & " ")
                    End If

                    StrSQL.Append(" AND SpecieVegetali_Default.Validita_Inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
                    StrSQL.Append(" AND SpecieVegetali_Default.Validita_Fine >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")

                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   SpecieVegetali_Default.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   SpecieVegetali_Default.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY SpecieVegetali.Veg_Des, Cultivar.Cul_Des, SpecieVegetali_Default.Numero_Ciclo ")
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


    '##############################################################################################
    'chiamata dalla edit_impianto: recupera sia il record del superuser sia quello dell'azienda (se c'è)
    'recupera il record indipendentemente dal cul_cod
    Public Function Leggi_2(ByVal Piva As String,
                            ByVal Veg_Cod As Integer,
                            ByVal Cul_Cod As Integer,
                            ByVal Codice As Integer,
                            ByVal Validita_Inizio As Date,
                            ByVal Validita_Fine As Date,
                            ByVal xFiltroAggiuntivo As String,
                            ByVal xOrderBy As String,
                            ByRef objParametri As AgronicaCoreParametri
                            ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.SpecieVegetali_Default_R.Leggi_2()"

        Dim messaggioErrore As String = ""
        Dim strSql As New Text.StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0
            strSql.Append(" SELECT * ")
            strSql.Append(" FROM  SpecieVegetali_Default ")
            strSql.Append(" WHERE SpecieVegetali_Default.PivaSuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            strSql.Append(" AND (SpecieVegetali_Default.Piva = ''")
            If Piva <> "" Then
                strSql.Append(" OR SpecieVegetali_Default.Piva = '" & Agro_SQL_SaveText(Piva) & "'")
            End If
            strSql.Append(") ")
            strSql.Append(" AND SpecieVegetali_Default.Veg_Cod = " & Veg_Cod & " ")
            strSql.Append(" AND (SpecieVegetali_Default.Cul_Cod = " & Cul_Cod & " OR SpecieVegetali_Default.Cul_Cod = 0) ")
            strSql.Append(" AND Validita_Inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
            strSql.Append(" AND Validita_Fine >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")

            If Codice <> 0 Then
                strSql.Append(" AND SpecieVegetali_Default.Codice = " & Codice & " ")
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                strSql.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.Append(" AND   SpecieVegetali_Default.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.Append(" AND   SpecieVegetali_Default.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                strSql.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
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


    '##############################################################################################
    'formatta data
    Public Function FormattaDefault(ByVal Valore As String,
                                    ByVal Data_Inizio_Distinta As String,
                                    ByVal Data_Fine_Distinta As String,
                                    ByVal Data_Inizio_Impianto As String,
                                    ByRef objParametri As AgronicaCoreParametri
                                    ) As String

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.SpecieVegetali_Default_R.FormattaDefault()"
        Dim messaggioErrore As String = ""
        Dim valoreFormattato As String
        Dim words As String()
        Dim separators As Char() = {"/"}
        Dim mese As Integer
        Dim giorno As Integer
        Dim dataInizio As Date
        Dim dataFine As Date
        Dim flagDataFine As Boolean = False

        Try

            If Valore.ToLower <> "gg/mm" Then

                words = Valore.Split(separators)

                mese = CInt(words(words.Length - 1))
                giorno = CInt(words(0))

                If Data_Fine_Distinta <> "" AndAlso IsDate(Data_Fine_Distinta) Then
                    dataFine = CDate(Data_Fine_Distinta)
                    flagDataFine = True
                End If


                If flagDataFine AndAlso dataFine = AGRODATAFINE Then

                    dataInizio = Date.Now

                    If mese < dataInizio.Month OrElse
                       (mese = dataInizio.Month AndAlso giorno < dataInizio.Day) Then
                        valoreFormattato = Valore & "/" & CStr(dataInizio.Year + 1)
                    Else
                        valoreFormattato = Valore & "/" & CStr(dataInizio.Year)
                    End If

                Else

                    If Data_Inizio_Distinta <> "" AndAlso IsDate(Data_Inizio_Distinta) Then

                        dataInizio = CDate(Data_Inizio_Distinta)

                        If mese < dataInizio.Month OrElse
                           (mese = dataInizio.Month AndAlso giorno < dataInizio.Day) Then
                            valoreFormattato = Valore & "/" & CStr(dataInizio.Year + 1)
                        Else
                            valoreFormattato = Valore & "/" & CStr(dataInizio.Year)
                        End If

                    Else
                        If Data_Inizio_Impianto <> "" AndAlso IsDate(Data_Inizio_Impianto) Then

                            dataInizio = CDate(Data_Inizio_Impianto)

                            If mese < dataInizio.Month OrElse
                               (mese = dataInizio.Month AndAlso giorno < dataInizio.Day) Then
                                valoreFormattato = Valore & "/" & CStr(dataInizio.Year + 1)
                            Else
                                valoreFormattato = Valore & "/" & CStr(dataInizio.Year)
                            End If

                        Else
                            valoreFormattato = Valore
                        End If
                    End If

                End If

            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return valoreFormattato

    End Function


    Public Function Verifica_Esistenza_Default(ByVal Piva As String,
                                               ByVal Veg_Cod As Integer,
                                               ByVal Cul_Cod As Integer,
                                               ByVal Codice As Integer,
                                               ByVal Validita_Inizio As Date,
                                               ByVal Validita_Fine As Date,
                                               ByRef objParametri As AgronicaCoreParametri
                                               ) As Boolean 'True esiste, False non esiste

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.SpecieVegetali_Default_R.Verifica_Esistenza_Default()"

        Dim messaggioErrore As String = ""
        Dim strSql As New Text.StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0
            strSql.Append(" SELECT * ")
            strSql.Append(" FROM  SpecieVegetali_Default ")
            strSql.Append(" WHERE SpecieVegetali_Default.PivaSuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            strSql.Append(" AND SpecieVegetali_Default.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            strSql.Append(" AND SpecieVegetali_Default.Veg_Cod = " & Veg_Cod & " ")
            strSql.Append(" AND SpecieVegetali_Default.Cul_Cod = " & Cul_Cod & " ")
            strSql.Append(" AND SpecieVegetali_Default.Codice = " & Codice & " ")
            strSql.Append(" AND SpecieVegetali_Default.Validita_Inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
            strSql.Append(" AND SpecieVegetali_Default.Validita_Fine >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.Append(" AND   SpecieVegetali_Default.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.Append(" AND   SpecieVegetali_Default.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
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

        If dt.Rows.Count > 0 Then
            Return True
        Else
            Return False
        End If

    End Function


    Public Function LeggiconVegDesCulDes(ByVal Piva As String,
                                         ByVal VerificaPiva As Boolean,
                                         ByVal Veg_Cod As Integer,
                                         ByVal Cul_Cod As Integer,
                                         ByVal Codice As Integer,
                                         ByVal Validita_Inizio As Date,
                                         ByVal Validita_Fine As Date,
                                         ByVal xSelezioneVariabile As enumSelezioneVariabile,
                                         ByVal xFiltroAggiuntivo As String,
                                         ByVal xOrderBy As String,
                                         ByRef objParametri As AgronicaCoreParametri
                                         ) As DataTable


        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.SpecieVegetali_Default_R.LeggiconVegDesCulDes()"

        Dim messaggioErrore As String = ""
        Dim strSql As New Text.StringBuilder
        Dim dt As DataTable

        Try
            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi


                Case enumSelezioneVariabile.Selezione_TabellaCompleta

                    strSql.Length = 0
                    strSql.Append(" SELECT SpecieVegetali_Default.PivaSuperUser, SpecieVegetali_Default.Id, SpecieVegetali_Default.Piva, SpecieVegetali_Default.Veg_Cod,  ")
                    strSql.Append(" SpecieVegetali_Default.Cul_Cod, SpecieVegetali_Default.Codice, SpecieVegetali_Default.Valore, SpecieVegetali.Veg_Des, Cultivar.Cul_Des ")

                    strSql.Append(" FROM SpecieVegetali_Default INNER JOIN ")
                    strSql.Append(" SpecieVegetali ON SpecieVegetali_Default.Veg_Cod = SpecieVegetali.Veg_Cod LEFT JOIN ")
                    strSql.Append(" Cultivar ON SpecieVegetali_Default.Cul_Cod = Cultivar.Cul_Cod ")

                    strSql.Append(" WHERE SpecieVegetali_Default.PivaSuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

                    If VerificaPiva Then
                        strSql.Append(" AND SpecieVegetali_Default.Piva = '" & Agro_SQL_SaveText(Piva) & "'")
                    End If

                    If Veg_Cod <> 0 Then
                        strSql.Append(" AND SpecieVegetali_Default.Veg_Cod = " & Veg_Cod & " ")
                    End If

                    If Cul_Cod <> 0 Then
                        strSql.Append(" AND SpecieVegetali_Default.Cul_Cod = " & Cul_Cod)
                    End If

                    If Codice <> 0 Then
                        strSql.Append(" AND SpecieVegetali_Default.Codice = " & Codice & " ")
                    End If

                    strSql.Append(" AND SpecieVegetali_Default.Validita_Inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
                    strSql.Append(" AND SpecieVegetali_Default.Validita_Fine >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")

                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        strSql.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            strSql.Append(" AND   SpecieVegetali_Default.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            strSql.Append(" AND   SpecieVegetali_Default.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then

                        strSql.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        strSql.Append(" ORDER BY Veg_Des, Cul_Des ")
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


    Public Function CicliCulturali(ByVal Piva As String,
                                   ByVal VerificaPiva As Boolean,
                                   ByVal Veg_Cod As Integer,
                                   ByVal Validita_Inizio As Date,
                                   ByVal Validita_Fine As Date,
                                   ByVal xSelezioneVariabile As enumSelezioneVariabile,
                                   ByVal xFiltroAggiuntivo As String,
                                   ByVal xOrderBy As String,
                                   ByRef objParametri As AgronicaCoreParametri
                                   ) As Integer

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.SpecieVegetali_Default_R.CicliCulturali()"

        Dim messaggioErrore As String = ""
        Dim strSql As New Text.StringBuilder
        Dim dt As DataTable

        Try
            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi


                Case enumSelezioneVariabile.Selezione_TabellaCompleta

                    strSql.Length = 0
                    strSql.Append(" SELECT MAX (numero_ciclo) AS 'n_cicli'  ")
                    strSql.Append(" FROM SpecieVegetali_Default")

                    strSql.Append(" WHERE SpecieVegetali_Default.PivaSuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

                    If VerificaPiva Then
                        strSql.Append(" AND SpecieVegetali_Default.Piva = '" & Agro_SQL_SaveText(Piva) & "'")
                    End If

                    If Veg_Cod <> 0 Then
                        strSql.Append(" AND SpecieVegetali_Default.Veg_Cod = " & Veg_Cod & " ")
                    End If

                    strSql.Append(" AND SpecieVegetali_Default.Validita_Inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
                    strSql.Append(" AND SpecieVegetali_Default.Validita_Fine >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")

                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        strSql.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            strSql.Append(" AND   SpecieVegetali_Default.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            strSql.Append(" AND   SpecieVegetali_Default.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------

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

        If IsDBNull(dt.Rows(0).Item("n_cicli")) Then
            Return 0
        End If

        Return dt.Rows(0).Item("n_cicli")

    End Function

End Class
