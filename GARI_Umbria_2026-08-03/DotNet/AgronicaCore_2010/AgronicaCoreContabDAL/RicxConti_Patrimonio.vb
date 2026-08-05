Imports System.Text
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider

Public Class RicxConti_Patrimonio_R
    Inherits AgronicaCoreDataProvider.DataProvider
    
    Public Function Leggi(ByVal Piva As String,
                          ByVal Ric_Cod_Pat As Integer,
                          ByVal Anno As Integer,
                          ByVal Cod_Conto_Pat As Integer,
                          ByVal Id_Riclassificazione As String,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                          ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.RicxConti_Patrimonio_R.Leggi()"
        
        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable
        
        Try

            strSql.AppendLine(" SELECT * ")
            strSql.AppendLine(" FROM  RicxConti_Patrimonio  ")
       
            strSql.AppendLine(" WHERE   RicxConti_Patrimonio.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'   ")
            strSql.AppendLine(" AND   RicxConti_Patrimonio.Piva      = '" & Agro_SQL_SaveText(Piva) & "'   ")

            If Ric_Cod_Pat <> 0 Then
                strSql.AppendLine(" AND RicxConti_Patrimonio.Ric_Cod_Pat   =  " & Agro_SQL_SaveNum(Ric_Cod_Pat) & "   ")
            End If

            If Anno <> 0 Then
                strSql.AppendLine(" AND RicxConti_Patrimonio.Anno =  " & Agro_SQL_SaveNum(Anno) & "   ")
            End If

            If Cod_Conto_Pat <> 0 Then
                strSql.AppendLine(" AND RicxConti_Patrimonio.Cod_Conto_Pat =  " & Agro_SQL_SaveNum(Cod_Conto_Pat) & "   ")
            End If

            If Id_Riclassificazione <> "" Then
                strSql.AppendLine(" AND RicxConti_Patrimonio.Id_Riclassificazione = '" & Agro_SQL_SaveText(Id_Riclassificazione) & "'   ")
            End If
            
            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------

            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                   strSql.AppendLine(" AND   RicxConti_Patrimonio.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.AppendLine(" AND   RicxConti_Patrimonio.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select

            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                strSql.AppendLine(" ORDER BY RicxConti_Patrimonio.Piva, RicxConti_Patrimonio.Ric_Cod_Pat, Anno  ")
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

    '###################################################################################
    Public Function Leggi_Codifica_ContiPatrimoniali(ByVal Piva As String,
                                                     ByVal Ric_Cod_Pat As Integer,
                                                     ByVal Anno As Integer,
                                                     ByVal AnnoMin As Integer,
                                                     ByVal xFiltroAggiuntivo As String,
                                                     ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                     ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.RicxConti_Patrimonio_R.Leggi_Codifica_ContiPatrimoniali()"

        Dim messaggioErrore As String = ""
        Dim dt As DataTable

        Dim filtro_conti As String = " RicxConti_Patrimonio.Codifica_Conto_Pat IN ( " &
                                     CStr(enum_Conti_Patrimoniali.CreditiVersoClienti) & ", " &
                                     CStr(enum_Conti_Patrimoniali.DebitiVersoFornitori) & ", " &
                                     CStr(enum_Conti_Patrimoniali.DepositiBancariPostali) & ", " &
                                     CStr(enum_Conti_Patrimoniali.DenaroValoriInCassa) & ", " &
                                     CStr(enum_Conti_Patrimoniali.DebitiVsEnasarco) & ", " &
                                     CStr(enum_Conti_Patrimoniali.ErarioRitenuteLavoroAutonomo) & ", " &
                                     CStr(enum_Conti_Patrimoniali.IvaACredito) & ", " &
                                     CStr(enum_Conti_Patrimoniali.IvaADebito) & ", " &
                                     CStr(enum_Conti_Patrimoniali.IvaACreditoAcqIntra) & ", " &
                                     CStr(enum_Conti_Patrimoniali.IvaADebitoAcqIntra) & " " &
                                     " ) "

        'aggiunto in data 18/03/2016: 
        If AnnoMin <> 0 Then
            filtro_conti += "AND RicxConti_Patrimonio.Anno >= " & Agro_SQL_SaveNum(AnnoMin)
        End If

        If xFiltroAggiuntivo = "" Then
            xFiltroAggiuntivo = filtro_conti
        Else
            xFiltroAggiuntivo = " AND " & filtro_conti
        End If

        Try

            dt = LeggiJoinDes(Piva, Ric_Cod_Pat, Anno, 0,
                              "",
                              xFiltroAggiuntivo, "",
                              objParametri)

            If Not IsNothing(dt) Then
                If dt.Rows.Count = 0 Then
                    Throw New Exception("Dt vuoto")
                End If
            Else
                Throw New Exception("Dt vuoto")
            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function
    
    '###################################################################################
    Public Function Verifica_Codifica_ContiPatrimoniali(ByRef LogVerifica As String,
                                                        ByVal Piva As String,
                                                        ByVal Ric_Cod_Pat As Integer,
                                                        ByVal Anno As Integer,
                                                        ByVal AnnoMin As Integer,
                                                        ByVal xFiltroAggiuntivo As String,
                                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                        ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.RicxConti_Patrimonio_R.Verifica_Codifica_ContiPatrimoniali()"

        Dim messaggioErrore As String = ""
        Dim dt As DataTable
        Dim Flag_OK As Boolean = False

        Try

            dt = Leggi_Codifica_ContiPatrimoniali(Piva, Ric_Cod_Pat, Anno, AnnoMin, xFiltroAggiuntivo, objParametri)

            If Not dt Is Nothing AndAlso dt.Rows.Count > 0 Then

                'Select Case DT.Rows.Count

                '    Case 8
                '        'ci sono tutte le codifiche
                '        Flag_OK = True
                '    Case Else
                Dim i As Integer
                Dim flag_crediti As Boolean = False
                Dim flag_debiti As Boolean = False
                Dim flag_depositi As Boolean = False
                Dim flag_cassa As Boolean = False
                Dim flag_ivacredito As Boolean = False
                Dim flag_ivadebito As Boolean = False
                Dim flag_ivacreditointra As Boolean = False
                Dim flag_ivadebitointra As Boolean = False
                Dim flag_ErarioRitenuteLavoroAutonomo As Boolean = False
                Dim flag_DebitiVsEnasarco As Boolean = False

                For i = 0 To dt.Rows.Count - 1

                    Select Case dt.Rows(i).Item("Codifica_Conto_Pat")

                        Case enum_Conti_Patrimoniali.CreditiVersoClienti
                            flag_crediti = True
                        Case enum_Conti_Patrimoniali.DebitiVersoFornitori
                            flag_debiti = True
                        Case enum_Conti_Patrimoniali.DepositiBancariPostali
                            flag_depositi = True
                        Case enum_Conti_Patrimoniali.DenaroValoriInCassa
                            flag_cassa = True
                        Case enum_Conti_Patrimoniali.IvaACredito
                            flag_ivacredito = True
                        Case enum_Conti_Patrimoniali.IvaADebito
                            flag_ivadebito = True
                        Case enum_Conti_Patrimoniali.IvaACreditoAcqIntra
                            flag_ivacreditointra = True
                        Case enum_Conti_Patrimoniali.IvaADebitoAcqIntra
                            flag_ivadebitointra = True
                        Case enum_Conti_Patrimoniali.ErarioRitenuteLavoroAutonomo
                            flag_ErarioRitenuteLavoroAutonomo = True
                        Case enum_Conti_Patrimoniali.DebitiVsEnasarco
                            flag_DebitiVsEnasarco = True
                    End Select

                Next

                If flag_crediti = False Then
                    LogVerifica += "Codifica CreditiVersoClienti mancante" & vbCrLf
                End If
                If flag_debiti = False Then
                    LogVerifica += "Codifica DebitiVersoFornitori mancante" & vbCrLf
                End If
                If flag_depositi = False Then
                    LogVerifica += "Codifica DepositiBancariPostali mancante" & vbCrLf
                End If
                If flag_cassa = False Then
                    LogVerifica += "Codifica DenaroValoriInCassa mancante" & vbCrLf
                End If
                If flag_ivacredito = False Then
                    LogVerifica += "Codifica IvaACredito mancante" & vbCrLf
                End If
                If flag_ivadebito = False Then
                    LogVerifica += "Codifica IvaADebito mancante" & vbCrLf
                End If
                If flag_ivacreditointra = False Then
                    LogVerifica += "Codifica IvaACreditoAcqIntra mancante" & vbCrLf
                End If
                If flag_ivadebitointra = False Then
                    LogVerifica += "Codifica IvaADebitoAcqIntra mancante" & vbCrLf
                End If
                If flag_ErarioRitenuteLavoroAutonomo = False Then
                    LogVerifica += "Codifica ErarioRitenuteLavoroAutonomo mancante" & vbCrLf
                End If
                If flag_DebitiVsEnasarco = False Then
                    LogVerifica += "Codifica DebitiVsEnasarco mancante" & vbCrLf
                End If

                If flag_crediti = True AndAlso
                   flag_debiti = True AndAlso
                   flag_depositi = True AndAlso 
                   flag_cassa = True AndAlso
                   flag_ErarioRitenuteLavoroAutonomo = True AndAlso
                   flag_DebitiVsEnasarco = True AndAlso
                   flag_ivacredito = True AndAlso
                   flag_ivadebito = True AndAlso
                   flag_ivacreditointra = True AndAlso
                   flag_ivadebitointra = True Then

                    Flag_OK = True
                End If

            End If

            ' End Select
            dt = Nothing

        Catch ex As Exception
            Flag_OK = False
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return Flag_OK

    End Function

    '###################################################################################
    Public Function Esiste_RicXConto(ByVal Piva As String,
                                     ByVal Ric_Cod_Pat As Integer,
                                     ByVal Cod_Conto_Pat As Integer,
                                     ByVal Anno As Integer,
                                     ByVal xFiltroAggiuntivo As String,
                                     ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                     ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.RicxConti_Patrimonio_R.Esiste_RicXConto()"

        Dim messaggioErrore As String = ""
        Dim dt As DataTable
        Dim flagEsiste As Boolean = False

        Try

            dt = Leggi(Piva, Ric_Cod_Pat, Anno, Cod_Conto_Pat,
                       "",
                       xFiltroAggiuntivo, "",
                       objParametri)

            If Not dt Is Nothing AndAlso dt.Rows.Count > 0 Then
                flagEsiste = True
            End If

            dt = Nothing

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return flagEsiste

    End Function

    Public Function LeggiDistinctAnno(ByVal Piva As String,
                                      ByVal Ric_Cod_Pat As Integer,
                                      ByVal xFiltroAggiuntivo As String,
                                      ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                      ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.RicxConti_Patrimonio_R.LeggiDistinctAnno()"
        
        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            strSql.AppendLine(" SELECT DISTINCT anno ")
            strSql.AppendLine(" FROM  RicxConti_Patrimonio  ")

            strSql.AppendLine(" WHERE   RicxConti_Patrimonio.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'   ")
            strSql.AppendLine(" AND   RicxConti_Patrimonio.Piva      = '" & Agro_SQL_SaveText(Piva) & "'   ")

            If (Ric_Cod_Pat <> 0) Then
                strSql.AppendLine(" AND RicxConti_Patrimonio.Ric_Cod_Pat   =  " & Agro_SQL_SaveNum(Ric_Cod_Pat) & "   ")
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.AppendLine(" AND   RicxConti_Patrimonio.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.AppendLine(" AND   RicxConti_Patrimonio.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '-------------------------------------------------------------------------
       
            strSql.AppendLine(" ORDER BY  Anno  ")

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
    
    Public Function LeggiEstesa(ByVal Piva As String,
                                ByVal Ric_Cod_Pat As Integer,
                                ByVal Anno As Integer,
                                ByVal Cod_Conto_Pat As Integer,
                                ByVal Id_Riclassificazione As String,
                                ByVal Tipo_Dare_Avere As String,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.RicxConti_Patrimonio_R.LeggiEstesa()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            If Piva = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
            End If

            strSql.AppendLine(" SELECT * ")
            strSql.AppendLine(" FROM  Conti_Patrimonio ")
            'correzione, ci vuole il join solo su cod_conto_pat (altrimenti chi usa i conti gias che hanno AAAAAAAAAAA non vengono su i dati)
            'strSql.AppendLine(" INNER JOIN RicxConti_Patrimonio ON Conti_Patrimonio.Piva_SuperUser = RicxConti_Patrimonio.Piva_SuperUser AND Conti_Patrimonio.Piva = RicxConti_Patrimonio.Piva AND  Conti_Patrimonio.Cod_Conto_Pat = RicxConti_Patrimonio.Cod_Conto_Pat ")
            strSql.AppendLine(" INNER JOIN RicxConti_Patrimonio ON Conti_Patrimonio.Cod_Conto_Pat = RicxConti_Patrimonio.Cod_Conto_Pat ")
            strSql.AppendLine(" INNER JOIN Riclassificazioni_Patrimonio ON Riclassificazioni_Patrimonio.Piva_SuperUser = RicxConti_Patrimonio.Piva_SuperUser AND Riclassificazioni_Patrimonio.Piva = RicxConti_Patrimonio.Piva AND   Riclassificazioni_Patrimonio.Ric_Cod_Pat = RicxConti_Patrimonio.Ric_Cod_Pat ")

            strSql.AppendLine(" WHERE   RicxConti_Patrimonio.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'   ")
            strSql.AppendLine(" AND   RicxConti_Patrimonio.Piva      = '" & Agro_SQL_SaveText(Piva) & "'   ")

            If Ric_Cod_Pat <> 0 Then
                strSql.AppendLine(" AND RicxConti_Patrimonio.Ric_Cod_Pat =  " & Agro_SQL_SaveNum(Ric_Cod_Pat) & "   ")
            End If

            If Tipo_Dare_Avere <> "" Then
                strSql.AppendLine(" AND RicxConti_Patrimonio.Dare_Avere =  '" & Agro_SQL_SaveText(Tipo_Dare_Avere) & "'   ")
            End If

            If Anno <> 0 Then
                strSql.AppendLine(" AND RicxConti_Patrimonio.Anno =  " & Agro_SQL_SaveNum(Anno) & "   ")
            End If

            If Cod_Conto_Pat <> 0 Then
                strSql.AppendLine(" AND RicxConti_Patrimonio.Cod_Conto_Pat =  " & Agro_SQL_SaveNum(Cod_Conto_Pat) & "   ")
            End If

            If Id_Riclassificazione <> "" Then
                strSql.AppendLine(" AND RicxConti_Patrimonio.Id_Riclassificazione = '" & Agro_SQL_SaveText(Id_Riclassificazione) & "'   ")
            End If
            
            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------

            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.AppendLine(" AND   Conti_Patrimonio.Inviato >=0 ")
                    strSql.AppendLine(" AND   RicxConti_Patrimonio.Inviato >=0 ")
                    strSql.AppendLine(" AND   Riclassificazioni_Patrimonio.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.AppendLine(" AND   Conti_Patrimonio.Inviato =-1 ")
                    strSql.AppendLine(" AND   RicxConti_Patrimonio.Inviato =-1 ")
                    strSql.AppendLine(" AND   Riclassificazioni_Patrimonio.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select

            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                strSql.AppendLine(" ORDER BY RicxConti_Patrimonio.Piva ASC, RicxConti_Patrimonio.Ric_Cod_Pat ASC, Conti_Patrimonio.Cod_Conto_Pat ASC ")
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

    Public Function LeggiJoinDes(ByVal Piva As String,
                                 ByVal Ric_Cod_Pat As Integer,
                                 ByVal Anno As Integer,
                                 ByVal Cod_Conto_Pat As Integer,
                                 ByVal Id_Riclassificazione As String,
                                 ByVal xFiltroAggiuntivo As String,
                                 ByVal xOrderBy As String,
                                 ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                 ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.RicxConti_Patrimonio_R.LeggiJoinDes()"
        
        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            strSql.AppendLine(" SELECT Conti_Patrimonio.Conto_Pat_Descr, Conti_Patrimonio.Flag_UE, Riclassificazioni_Patrimonio.Ric_Des_Pat, Riclassificazioni_Patrimonio.Piva_Riferimento, ")
            strSql.AppendLine(" RicxConti_Patrimonio.Piva_SuperUser, RicxConti_Patrimonio.Piva, RicxConti_Patrimonio.Ric_Cod_Pat, RicxConti_Patrimonio.Cod_Conto_Pat, RicxConti_Patrimonio.Codifica_Conto_Pat, RicxConti_Patrimonio.Anno, ")
            strSql.AppendLine(" RicxConti_Patrimonio.Id_Riclassificazione, RicxConti_Patrimonio.Dare_Avere, RicxConti_Patrimonio.Saldo, RicxConti_Patrimonio.Tipo, RicxConti_Patrimonio.Imputabile, RicxConti_Patrimonio.Saldo_Iniziale ")
            strSql.AppendLine(" FROM  Conti_Patrimonio ")
            'correzione, ci vuole il join solo su cod_conto_pat (altrimenti chi usa i conti gias che hanno AAAAAAAAAAA non vengono su i dati)
            'strSql.AppendLine(" INNER JOIN RicxConti_Patrimonio ON Conti_Patrimonio.Piva_SuperUser = RicxConti_Patrimonio.Piva_SuperUser AND Conti_Patrimonio.Piva = RicxConti_Patrimonio.Piva AND  Conti_Patrimonio.Cod_Conto_Pat = RicxConti_Patrimonio.Cod_Conto_Pat ")
            strSql.AppendLine(" INNER JOIN RicxConti_Patrimonio ON  Conti_Patrimonio.Cod_Conto_Pat = RicxConti_Patrimonio.Cod_Conto_Pat ")
            strSql.AppendLine(" INNER JOIN Riclassificazioni_Patrimonio ON Riclassificazioni_Patrimonio.Piva_SuperUser = RicxConti_Patrimonio.Piva_SuperUser AND Riclassificazioni_Patrimonio.Piva = RicxConti_Patrimonio.Piva AND   Riclassificazioni_Patrimonio.Ric_Cod_Pat = RicxConti_Patrimonio.Ric_Cod_Pat ")

            strSql.AppendLine(" WHERE   RicxConti_Patrimonio.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'   ")
            strSql.AppendLine(" AND   RicxConti_Patrimonio.Piva      = '" & Agro_SQL_SaveText(Piva) & "'   ")

            If Ric_Cod_Pat <> 0 Then
                strSql.AppendLine(" AND RicxConti_Patrimonio.Ric_Cod_Pat   =  " & Agro_SQL_SaveNum(Ric_Cod_Pat) & "   ")
            End If

            If Anno <> 0 Then
                strSql.AppendLine(" AND RicxConti_Patrimonio.Anno =  " & Agro_SQL_SaveNum(Anno) & "   ")
            End If

            If Cod_Conto_Pat <> 0 Then
                strSql.AppendLine(" AND RicxConti_Patrimonio.Cod_Conto_Pat =  " & Agro_SQL_SaveNum(Cod_Conto_Pat) & "   ")
            End If

            If Id_Riclassificazione <> "" Then
                strSql.AppendLine(" AND RicxConti_Patrimonio.Id_Riclassificazione = '" & Agro_SQL_SaveText(Id_Riclassificazione) & "'   ")
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.AppendLine(" AND   Conti_Patrimonio.Inviato >=0 ")
                    strSql.AppendLine(" AND   RicxConti_Patrimonio.Inviato >=0 ")
                    strSql.AppendLine(" AND   Riclassificazioni_Patrimonio.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.AppendLine(" AND   Conti_Patrimonio.Inviato =-1 ")
                    strSql.AppendLine(" AND   RicxConti_Patrimonio.Inviato =-1 ")
                    strSql.AppendLine(" AND   Riclassificazioni_Patrimonio.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                strSql.AppendLine(" ORDER BY RicxConti_Patrimonio.Piva ASC, RicxConti_Patrimonio.Ric_Cod_Pat ASC, Conti_Patrimonio.Cod_Conto_Pat ASC ")
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

End Class



'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§


Public Class RicxConti_Patrimonio_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Modifica_Solo_Saldo(ByVal Piva As String,
                                        ByVal Ric_Cod_Pat As Integer,
                                        ByVal Cod_Conto_Pat As Integer,
                                        ByVal Anno As Integer,
                                        ByVal Importo As Decimal,
                                        ByVal Iva_Dare As Decimal,
                                        ByVal Iva_Avere As Decimal,
                                        ByVal Validita_Inizio As Date,
                                        ByVal Validita_Fine As Date,
                                        ByVal xFiltroAggiuntivo As String,
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                        ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.RicxConti_Patrimonio_W.Modifica_Solo_Saldo()"
        
        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            If Piva = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
            End If

            If Ric_Cod_Pat = 0 Then
                Throw New Exception("Parametro non corretto nella query (Ric_Cod_Pat obbligatorio)")
            End If

            If Cod_Conto_Pat = 0 Then
                Throw New Exception("Parametro non corretto nella query (Cod_Conto_Pat obbligatorio)")
            End If

            If Anno = 0 Then
                Throw New Exception("Parametro non corretto nella query (Anno obbligatorio)")
            End If

            '---------------------------------------------

            strSql.Length = 0

            strSql.AppendLine(" UPDATE RicXConti_Patrimonio SET ")
            strSql.AppendLine("     Saldo             = Saldo + " & Agro_SQL_SaveNum(Importo - Iva_Dare - Iva_Avere) & "  ")

            strSql.AppendLine("   ,Inviato           =  0 ")
            strSql.AppendLine("   ,DataInvio         =  Null ")
            strSql.AppendLine("   ,Data_Modifica     =  " & Agro_SQL_SaveDateTime(Now))
            strSql.AppendLine("   ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            strSql.AppendLine("   ,Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Inizio))
            strSql.AppendLine("   ,Validita_Fine     =  " & Agro_SQL_SaveDate(Validita_Fine))

            strSql.AppendLine(" WHERE Piva          = '" & Agro_SQL_SaveText(Piva) & "'  ")
            strSql.AppendLine(" AND   Ric_Cod_Pat   =  " & Agro_SQL_SaveNum(Ric_Cod_Pat) & "   ")
            strSql.AppendLine(" AND   Cod_Conto_Pat =  " & Agro_SQL_SaveNum(Cod_Conto_Pat) & "   ")
            strSql.AppendLine(" AND   Anno          =  " & Agro_SQL_SaveNum(Anno) & "   ")
            
            '----------------------------------------------------------------------
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
    
    Public Function Scrivi(ByVal PIVA As String,
                           ByVal Ric_Cod_Pat As Integer,
                           ByVal Cod_Conto_Pat As Integer,
                           ByVal Anno As Integer,
                           ByVal Id_Riclassificazione As String,
                           ByVal Dare_Avere As String,
                           ByVal Saldo As Decimal,
                           ByVal Saldo_Iniziale As Decimal,
                           ByVal Tipo As Integer,
                           ByVal Imputabile As Integer,
                           ByVal Codifica_Conto_Pat As Integer,
                           ByVal Validita_Inizio As Date,
                           ByVal Validita_Fine As Date,
                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                           ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.RicxConti_Patrimonio_W.Scrivi()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            strSql.Length = 0

            strSql.AppendLine(" INSERT INTO RicxConti_Patrimonio ")
            strSql.AppendLine("   (Piva_SuperUser, Piva,   Ric_Cod_Pat,  Cod_Conto_Pat, Anno, ")
            strSql.AppendLine("   Id_Riclassificazione, Dare_Avere, Saldo, Saldo_Iniziale, Tipo, Imputabile, Codifica_Conto_Pat, ")

            strSql.AppendLine(" Inviato,            DataInvio, ")
            strSql.AppendLine(" Data_Creazione,     Data_Modifica, ")
            strSql.AppendLine(" UserName_Creazione, UserName_Modifica, ")
            strSql.AppendLine(" Validita_Inizio,    Validita_Fine ")
            strSql.AppendLine("   ) ")

            strSql.AppendLine(" VALUES (")

            strSql.AppendLine("       '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'  ")
            strSql.AppendLine("       ,'" & Agro_SQL_SaveText(PIVA) & "'  ")
            strSql.AppendLine("       ," & Agro_SQL_SaveNum(Ric_Cod_Pat) & " ")
            strSql.AppendLine("       ," & Agro_SQL_SaveNum(Cod_Conto_Pat) & "   ")
            strSql.AppendLine("       ," & Agro_SQL_SaveNum(Anno) & "   ")
            strSql.AppendLine("       ,'" & Agro_SQL_SaveText(Id_Riclassificazione) & "'  ")
            strSql.AppendLine("       ,'" & Agro_SQL_SaveText(Dare_Avere) & "'   ")
            strSql.AppendLine("       ," & Agro_SQL_SaveNum(Saldo) & "   ")
            strSql.AppendLine("       ," & Agro_SQL_SaveNum(Saldo_Iniziale) & "   ")
            strSql.AppendLine("       ," & Agro_SQL_SaveNum(Tipo) & "   ")
            strSql.AppendLine("       ," & Agro_SQL_SaveNum(Imputabile) & "   ")
            strSql.AppendLine("       ," & Agro_SQL_SaveNum(Codifica_Conto_Pat) & "   ")

            strSql.AppendLine("         , 0  ")
            strSql.AppendLine("         , Null  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveDateTime(Now) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveDateTime(Now) & "  ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
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
    
    Public Function Modifica(ByVal PIVA As String,
                             ByVal Ric_Cod_Pat As Integer,
                             ByVal Cod_Conto_Pat As Integer,
                             ByVal Anno As Integer,
                             ByVal Id_Riclassificazione As String,
                             ByVal Dare_Avere As String,
                             ByVal Saldo As Decimal,
                             ByVal Saldo_Iniziale As Decimal,
                             ByVal Tipo As Integer,
                             ByVal Imputabile As Integer,
                             ByVal Validita_Inizio As Date,
                             ByVal Validita_Fine As Date,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                             ) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.RicxConti_Patrimonio_W.Modifica_Solo_Saldo()"
        
        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            strSql.Length = 0

            strSql.AppendLine(" UPDATE RicxConti_Patrimonio SET ")
            strSql.AppendLine(" Id_Riclassificazione ='" & Agro_SQL_SaveText(Id_Riclassificazione) & "'  ")
            strSql.AppendLine(" ,Dare_Avere           ='" & Agro_SQL_SaveText(Dare_Avere) & "'  ")
            strSql.AppendLine(" ,Saldo             =" & Agro_SQL_SaveNum(Saldo) & "  ")
            strSql.AppendLine(" ,Saldo_Iniziale     =" & Agro_SQL_SaveNum(Saldo_Iniziale) & "  ")
            strSql.AppendLine(" ,Tipo        =" & Agro_SQL_SaveNum(Tipo) & "   ")
            strSql.AppendLine(" ,Imputabile        =" & Agro_SQL_SaveNum(Imputabile) & "   ")
            strSql.AppendLine("   ,Inviato           =  0 ")
            strSql.AppendLine("   ,DataInvio         =  Null ")
            strSql.AppendLine("   ,Data_Modifica     =  " & Agro_SQL_SaveDateTime(Now))
            strSql.AppendLine("   ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            strSql.AppendLine("   ,Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Inizio))
            strSql.AppendLine("   ,Validita_Fine     =  " & Agro_SQL_SaveDate(Validita_Fine))
            strSql.AppendLine(" WHERE   Piva_SuperUser      = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'  ")
            strSql.AppendLine(" AND     Piva      = '" & Agro_SQL_SaveText(PIVA) & "'  ")
            strSql.AppendLine(" AND     Ric_Cod_Pat   =  " & Agro_SQL_SaveNum(Ric_Cod_Pat) & "   ")
            strSql.AppendLine(" AND     Cod_Conto_Pat =  " & Agro_SQL_SaveNum(Cod_Conto_Pat) & "   ")
            strSql.AppendLine(" AND     Anno      =  " & Agro_SQL_SaveNum(Anno) & "   ")
            
            '----------------------------------------------------------------------
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
    
    Public Function Cancella(ByVal PIVA As String,
                             ByVal Ric_Cod_Pat As Integer,
                             ByVal Cod_Conto_Pat As Integer,
                             ByVal Anno As Integer,
                             ByVal xFiltroAggiuntivo As String,
                             ByVal objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.RicxConti_Patrimonio_W.Cancella()"
        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            If PIVA = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
            End If

            If Ric_Cod_Pat = 0 Then
                Throw New Exception("Parametro non corretto nella query (Ric_Cod_Pat obbligatorio)")
            End If

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then
                strSql.Length = 0
                strSql.AppendLine(" UPDATE  RicxConti_Patrimonio ")
                strSql.AppendLine(" SET ")
                strSql.AppendLine("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                strSql.AppendLine("      ,Inviato = -1 ")
                strSql.AppendLine(" WHERE  Inviato >= 0 ")
                strSql.AppendLine(" AND   Piva_SuperUser    = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'  ")
            Else
                strSql.Length = 0
                strSql.AppendLine(" DELETE ")
                strSql.AppendLine(" FROM RicxConti_Patrimonio ")
                strSql.AppendLine(" WHERE  Inviato >= 0 ")
                strSql.AppendLine(" AND   Piva_SuperUser    = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'  ")
            End If

            If PIVA <> "" Then
                strSql.AppendLine(" AND Piva = '" & Agro_SQL_SaveText(PIVA) & "' ")
            End If

            If Ric_Cod_Pat <> 0 Then
                strSql.AppendLine(" AND Ric_Cod_Pat = " & Agro_SQL_SaveNum(Ric_Cod_Pat) & "   ")
            End If

            If Cod_Conto_Pat <> 0 Then
                strSql.AppendLine(" AND Cod_Conto_Pat = " & Agro_SQL_SaveNum(Cod_Conto_Pat) & "   ")
            End If

            If Anno <> 0 Then
                strSql.AppendLine(" AND Anno = " & Agro_SQL_SaveNum(Anno) & "   ")
            End If

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

    'query che è anche nel migra
    Public Function Update_CodificaContoPat_Base(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                 ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.RicxConti_Patrimonio_W.Update_CodificaContoPat_Base()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            strSql.Length = 0

            strSql.AppendLine(" -- valorizza Codifica_Conto_Pat (se = 0) per i conti patrimoniali per i quali serve il mapping  ")
            strSql.AppendLine(" UPDATE RicxConti_Patrimonio ")
            strSql.AppendLine(" SET Codifica_Conto_Pat = Cod_Conto_Pat ")
            strSql.AppendLine(" WHERE Cod_Conto_Pat IN (" & CStr(enum_Conti_Patrimoniali.CreditiVersoClienti) & "," & _
                                                        CStr(enum_Conti_Patrimoniali.DebitiVersoFornitori) & "," & _
                                                        CStr(enum_Conti_Patrimoniali.DepositiBancariPostali) & "," & _
                                                        CStr(enum_Conti_Patrimoniali.DenaroValoriInCassa) & "," & _
                                                        CStr(enum_Conti_Patrimoniali.IvaACredito) & "," & _
                                                        CStr(enum_Conti_Patrimoniali.IvaADebito) & "," & _
                                                        CStr(enum_Conti_Patrimoniali.IvaACreditoAcqIntra) & "," & _
                                                        CStr(enum_Conti_Patrimoniali.IvaADebitoAcqIntra) & "," & _
                                                        CStr(enum_Conti_Patrimoniali.ErarioRitenuteLavoroAutonomo) & "," & _
                                                        CStr(enum_Conti_Patrimoniali.DebitiVsEnasarco) & ") ")
            strSql.AppendLine(" AND Codifica_Conto_Pat = 0 ")
            strSql.AppendLine(" AND EXISTS ( ")
            strSql.AppendLine(" 			SELECT 1 ")
            strSql.AppendLine(" 			FROM Riclassificazioni_Patrimonio ")
            strSql.AppendLine(" 			WHERE Riclassificazioni_Patrimonio.Piva_SuperUser = RicxConti_Patrimonio.Piva_SuperUser ")
            strSql.AppendLine(" 			AND Riclassificazioni_Patrimonio.piva = RicxConti_Patrimonio.piva ")
            strSql.AppendLine(" 			AND Riclassificazioni_Patrimonio.Ric_Cod_Pat = RicxConti_Patrimonio.Ric_Cod_Pat " )
            strSql.AppendLine("             AND Piva_Riferimento = '" & PIVA_BILANCIO_EUROPEO & "' ")
            strSql.AppendLine("             ) ")

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
