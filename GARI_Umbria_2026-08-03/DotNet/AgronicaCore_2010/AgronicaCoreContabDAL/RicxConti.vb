Imports System.Text
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi


Public Class RicxConti_R
    Inherits AgronicaCoreDataProvider.DataProvider
    
    Public Function Leggi(ByVal Piva As String,
                          ByVal Ric_Cod As Integer,
                          ByVal Anno As Integer,
                          ByVal Cod_Conto As Integer,
                          ByVal Codifica_Conto As Integer,
                          ByVal Id_Riclassificazione As String,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreParametri
                          ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.RicxConti_R.Leggi()"
        
        Dim messaggioErrore As String = ""
        Dim StrSQL As New StringBuilder
        Dim dt As DataTable

        Try

            '---------------------------------------------
            StrSQL.AppendLine(" SELECT * ")
            StrSQL.AppendLine(" FROM RicXConti ")

            StrSQL.AppendLine(" WHERE   RicXConti.Piva      = '" & Agro_SQL_SaveText(Piva) & "'   ")

            If Ric_Cod <> 0 Then
                StrSQL.AppendLine(" AND RicXConti.Ric_Cod   =  " & Agro_SQL_SaveNum(Ric_Cod) & "   ")
            End If

            If Anno <> 0 Then
                StrSQL.AppendLine(" AND RicXConti.Anno =  " & Agro_SQL_SaveNum(Anno) & "   ")
            End If

            If Cod_Conto <> 0 Then
                StrSQL.AppendLine(" AND RicXConti.Cod_Conto =  " & Agro_SQL_SaveNum(Cod_Conto) & "   ")
            End If

            If Codifica_Conto <> 0 Then
                StrSQL.AppendLine(" AND RicXConti.Codifica_Conto =  " & Agro_SQL_SaveNum(Codifica_Conto) & "   ")
            End If

            If Id_Riclassificazione <> "" Then
                StrSQL.AppendLine(" AND RicXConti.Id_Riclassificazione = '" & Agro_SQL_SaveText(Id_Riclassificazione) & "'   ")
            End If
            
            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------

            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.AppendLine(" AND   RicXConti.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.AppendLine(" AND   RicXConti.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine(" ORDER BY RicXConti.Anno, RicXConti.Id_Riclassificazione ")
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

    Public Function Leggi_New(ByVal Piva As String,
                              ByVal Ric_Cod As Integer,
                              ByVal Anno As Integer,
                              ByVal Cod_Conto As Integer,
                              ByVal Id_Riclassificazione As String,
                              ByVal Tipo_Dare_Avere As String,
                              ByVal xFiltroAggiuntivo As String,
                              ByVal xOrderBy As String,
                              ByRef objParametri As AgronicaCoreParametri
                              ) As DataTable
        
        Const nomeRoutine = "AgronicaCoreContabDAL.RicxConti_R.Leggi_New()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New StringBuilder
        Dim dt As DataTable

        Try

            If Piva = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
            End If

            '---------------------------------------------
            StrSQL.AppendLine(" SELECT DISTINCT * ")
            StrSQL.AppendLine(" FROM  RicXConti ")
            StrSQL.AppendLine(" INNER JOIN Riclassificazioni ON RicXConti.Piva = Riclassificazioni.Piva AND RicXConti.Ric_Cod = Riclassificazioni.Ric_Cod ")
            StrSQL.AppendLine(" INNER JOIN Conti ON RicXConti.Cod_Conto = Conti.Cod_Conto ")
            StrSQL.AppendLine(" WHERE Conti.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & "  ")
            StrSQL.AppendLine(" AND   Conti.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & "  ")
            StrSQL.AppendLine(" AND   RicXConti.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & "  ")
            StrSQL.AppendLine(" AND   RicXConti.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & "  ")
            StrSQL.AppendLine(" AND   Riclassificazioni.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & "  ")
            StrSQL.AppendLine(" AND   Riclassificazioni.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & "  ")

            StrSQL.AppendLine(" AND   RicXConti.Piva      = '" & Agro_SQL_SaveText(Piva) & "'   ")

            If Tipo_Dare_Avere <> "" Then
                StrSQL.AppendLine(" AND RicXConti.Dare_Avere   =  '" & Agro_SQL_SaveText(Tipo_Dare_Avere) & "'  ")
            End If

            If Ric_Cod <> 0 Then
                StrSQL.AppendLine(" AND RicXConti.Ric_Cod   =  " & Agro_SQL_SaveNum(Ric_Cod) & "   ")
            End If

            If Anno <> 0 Then
                StrSQL.AppendLine(" AND RicXConti.Anno =  " & Agro_SQL_SaveNum(Anno) & "   ")
            End If

            If Cod_Conto <> 0 Then
                StrSQL.AppendLine(" AND RicXConti.Cod_Conto =  " & Agro_SQL_SaveNum(Cod_Conto) & "   ")
            End If

            If Id_Riclassificazione <> "" Then
                StrSQL.AppendLine(" AND RicXConti.Id_Riclassificazione = '" & Agro_SQL_SaveText(Id_Riclassificazione) & "'   ")
            End If
            
            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------

            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.AppendLine(" AND   Conti.Inviato >=0 ")
                    StrSQL.AppendLine(" AND   RicXConti.Inviato >=0 ")
                    StrSQL.AppendLine(" AND   Riclassificazioni.Inviato >=0 ")

                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.AppendLine(" AND   Conti.Inviato =-1 ")
                    StrSQL.AppendLine(" AND   RicXConti.Inviato =-1 ")
                    StrSQL.AppendLine(" AND   Riclassificazioni.Inviato =-1 ")

                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine(" ORDER BY RicXConti.Piva ASC, RicXConti.Ric_Cod ASC, Conti.Cod_Conto ASC ")
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
    
    Public Function LeggiDistinctAnno(ByVal Piva As String,
                                      ByVal Ric_Cod As Integer,
                                      ByVal xFiltroAggiuntivo As String,
                                      ByRef objParametri As AgronicaCoreParametri
                                      ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.RicxConti_R.LeggiDistinctAnno()"
        
        Dim messaggioErrore As String = ""
        Dim StrSQL As New StringBuilder
        Dim dt As DataTable

        Try

            StrSQL.AppendLine(" SELECT DISTINCT anno ")
            StrSQL.AppendLine(" FROM  RicxConti  ")

            StrSQL.AppendLine(" WHERE  RicxConti.Piva      = '" & Agro_SQL_SaveText(Piva) & "'   ")

            If Ric_Cod <> 0 Then
                StrSQL.AppendLine(" AND RicxConti.Ric_Cod   =  " & Agro_SQL_SaveNum(Ric_Cod) & "   ")
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------

            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.AppendLine(" AND   RicxConti.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.AppendLine(" AND   RicxConti.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '-------------------------------------------------------------------------

            StrSQL.AppendLine(" ORDER BY  Anno  ")

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


    '###################################################################################
    Public Function Esiste_RicXConto(ByVal Piva As String,
                                     ByVal Ric_Cod As Integer,
                                     ByVal Cod_Conto As Integer,
                                     ByVal Anno As Integer,
                                     ByVal xFiltroAggiuntivo As String,
                                     ByRef objParametri As AgronicaCoreParametri
                                     ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.RicxConti_R.Esiste_RicXConto()"

        Dim messaggioErrore As String = ""
        Dim dt As DataTable
        Dim flagEsiste As Boolean = False

        Try

            dt = Leggi(Piva, Ric_Cod, Anno, Cod_Conto, 0,
                       "",
                       xFiltroAggiuntivo, "",
                       objParametri)

            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
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


    '###################################################################################
    Public Function Esiste_Codifica_Conto(ByVal Piva As String,
                                          ByVal Ric_Cod As Integer,
                                          ByVal Codifica_Conto As Integer,
                                          ByVal Anno As Integer,
                                          ByVal xFiltroAggiuntivo As String,
                                          ByRef objParametri As AgronicaCoreParametri
                                          ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.RicxConti_R.Esiste_Codifica_Conto()"

        Dim messaggioErrore As String = ""
        Dim dt As DataTable
        Dim flagEsiste As Boolean = False

        Try

            dt = Leggi(Piva, Ric_Cod, Anno, 0, Codifica_Conto,
                       "",
                       xFiltroAggiuntivo, "",
                       objParametri)

            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
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

    '###################################################################################
    Public Function Verifica_Codifica_ContiEconomici(ByRef LogVerifica As String,
                                                     ByVal Piva As String,
                                                     ByVal Ric_Cod As Integer,
                                                     ByVal Anno As Integer,
                                                     ByVal AnnoMin As Integer,
                                                     ByVal xFiltroAggiuntivo As String,
                                                     ByRef objParametri As AgronicaCoreParametri
                                                     ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.RicxConti_R.Verifica_Codifica_ContiEconomici()"

        Dim messaggioErrore As String = ""
        Dim dt As DataTable
        Dim flagOk As Boolean = False

        Try

            dt = Leggi_Codifica_ContiEconomici(Piva, Ric_Cod, Anno, AnnoMin, xFiltroAggiuntivo, objParametri)

            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then

                Dim i As Integer
                Dim flag_omaggiclientela As Boolean = False
                Dim flag_ricavixivacompensazione As Boolean = False

                For i = 0 To dt.Rows.Count - 1

                    Select Case dt.Rows(i).Item("Codifica_Conto")

                        Case enum_Conti_Economici.OmaggiAllaClientela
                            flag_omaggiclientela = True
                        Case enum_Conti_Economici.RicavixIVAincompensazione
                            flag_ricavixivacompensazione = True
                    End Select

                Next

                If Not flag_omaggiclientela Then
                    LogVerifica += "Codifica omaggi alla clientela mancante" & vbCrLf
                End If
                If Not flag_ricavixivacompensazione Then
                    LogVerifica += "Codifica ricavixivacompensazione mancante" & vbCrLf
                End If

                If flag_omaggiclientela AndAlso flag_ricavixivacompensazione Then
                    flagOk = True
                End If

            End If

            dt = Nothing

        Catch ex As Exception
            flagOk = False
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return flagOk

    End Function

    '###################################################################################
    Public Function Leggi_Codifica_ContiEconomici(ByVal Piva As String,
                                                  ByVal Ric_Cod As Integer,
                                                  ByVal Anno As Integer,
                                                  ByVal AnnoMin As Integer,
                                                  ByVal xFiltroAggiuntivo As String,
                                                  ByRef objParametri As AgronicaCoreParametri
                                                  ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.RicxConti_R.Leggi_Codifica_ContiEconomici()"

        Dim messaggioErrore As String = ""
        Dim dt As DataTable

        Dim filtro_conti As String = " RicxConti.Codifica_Conto IN ( " &
                                     CStr(enum_Conti_Economici.RicavixIVAincompensazione) & ", " &
                                     CStr(enum_Conti_Economici.OmaggiAllaClientela) & " " &
                                     " ) "

        'aggiunto in data 18/03/2016: 
        If AnnoMin <> 0 Then
            filtro_conti += "AND RicxConti.Anno >= " & Agro_SQL_SaveNum(AnnoMin)
        End If

        If xFiltroAggiuntivo = "" Then
            xFiltroAggiuntivo = filtro_conti
        Else
            xFiltroAggiuntivo = " AND " & filtro_conti
        End If

        Try

            dt = Leggi_New(Piva, Ric_Cod, Anno, 0,
                           "", "",
                           xFiltroAggiuntivo, "",
                           objParametri)

            'non genero eccezione, perché chi non è in regime forfettario non ha questo conto inserito
            'per chi lo è, ci pensa il giaslan a segnalarlo
            'If Not IsNothing(DT) Then
            '    If DT.Rows.Count = 0 Then
            '        Throw New Exception("Dt vuoto")
            '    End If
            'Else
            '    Throw New Exception("Dt vuoto")
            'End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    '###################################################################################
    Public Function Esistono_CodificheConti(ByVal Piva As String,
                                            ByVal Ric_Cod As Integer,
                                            ByVal Anno As Integer,
                                            ByVal AnnoMin As Integer,
                                            ByVal xFiltroAggiuntivo As String,
                                            ByRef objParametri As AgronicaCoreParametri
                                            ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.RicxConti_R.Esistono_CodificheConti()"

        Dim messaggioErrore As String = ""
        Dim dt As DataTable
        Dim esistonoCodifiche As Boolean = False

        Try

            dt = Leggi_Codifica_ContiEconomici(Piva, Ric_Cod, Anno, AnnoMin, "", objParametri)

            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                esistonoCodifiche = True
            End If

            dt = Nothing

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return esistonoCodifiche

    End Function

End Class



'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§

Public Class RicxConti_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Modifica_Solo_Saldo(ByVal Piva As String,
                                        ByVal Ric_Cod As Integer,
                                        ByVal Cod_Conto As Integer,
                                        ByVal Anno As Integer,
                                        ByVal Importo As Decimal,
                                        ByVal Validita_Inizio As Date,
                                        ByVal Validita_Fine As Date,
                                        ByVal xFiltroAggiuntivo As String,
                                        ByRef objParametri As AgronicaCoreParametri
                                        ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.RicxConti_W.Modifica_Solo_Saldo()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            If Piva = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
            End If

            If Ric_Cod = 0 Then
                Throw New Exception("Parametro non corretto nella query (Ric_Cod obbligatorio)")
            End If

            If Cod_Conto = 0 Then
                Throw New Exception("Parametro non corretto nella query (Cod_Conto obbligatorio)")
            End If

            If Anno = 0 Then
                Throw New Exception("Parametro non corretto nella query (Anno obbligatorio)")
            End If

            '---------------------------------------------

            StrSQL.Length = 0

            StrSQL.AppendLine(" UPDATE RicXConti SET ")
            StrSQL.AppendLine("     Saldo             = Saldo + " & Agro_SQL_SaveNum(Importo) & "  ")

            StrSQL.AppendLine("   ,Inviato           =  0 ")
            StrSQL.AppendLine("   ,DataInvio         =  Null ")
            StrSQL.AppendLine("   ,Data_Modifica     =  " & Agro_SQL_SaveDateTime(Now))
            StrSQL.AppendLine("   ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.AppendLine("   ,Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Inizio))
            StrSQL.AppendLine("   ,Validita_Fine     =  " & Agro_SQL_SaveDate(Validita_Fine))

            StrSQL.AppendLine(" WHERE Piva      = '" & Agro_SQL_SaveText(Piva) & "'  ")
            StrSQL.AppendLine(" AND   Ric_Cod   =  " & Agro_SQL_SaveNum(Ric_Cod) & "   ")
            StrSQL.AppendLine(" AND   Cod_Conto =  " & Agro_SQL_SaveNum(Cod_Conto) & "   ")
            StrSQL.AppendLine(" AND   Anno      =  " & Agro_SQL_SaveNum(Anno) & "   ")
            
            '----------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
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
    
    Public Function Scrivi(ByVal PIVA As String,
                           ByVal Ric_Cod As Integer,
                           ByVal Cod_Conto As Integer,
                           ByVal Anno As Integer,
                           ByVal Id_Riclassificazione As String,
                           ByVal Dare_Avere As String,
                           ByVal Saldo As Decimal,
                           ByVal Imputabile As Integer,
                           ByVal Saldo_Iniziale As Decimal,
                           ByVal Codifica_Conto As Integer,
                           ByVal ChkImputazione_Automatica As Integer,
                           ByVal Validita_Inizio As Date,
                           ByVal Validita_Fine As Date,
                           ByRef objParametri As AgronicaCoreParametri
                           ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.RicxConti_W.Scrivi()"
        
        Dim messaggioErrore As String = ""
        Dim StrSQL As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            StrSQL.Length = 0
            
            StrSQL.AppendLine(" INSERT INTO RicXConti ")
            StrSQL.AppendLine(" (Piva,   Ric_Cod,  Cod_Conto, Anno, ")
            StrSQL.AppendLine(" Id_Riclassificazione, Dare_Avere, Saldo, Imputabile, ")
            StrSQL.AppendLine(" Saldo_Iniziale, Codifica_Conto, ChkImputazione_Automatica, ")

            StrSQL.AppendLine(" Inviato,            DataInvio, ")
            StrSQL.AppendLine(" Data_Creazione,     Data_Modifica, ")
            StrSQL.AppendLine(" UserName_Creazione, UserName_Modifica, ")
            StrSQL.AppendLine(" Validita_Inizio,    Validita_Fine ")

            StrSQL.AppendLine("   ) ")

            StrSQL.AppendLine(" VALUES (")

            StrSQL.AppendLine("       '" & Agro_SQL_SaveText(PIVA) & "'  ")
            StrSQL.AppendLine("       ," & Agro_SQL_SaveNum(Ric_Cod) & " ")
            StrSQL.AppendLine("       ," & Agro_SQL_SaveNum(Cod_Conto) & "   ")
            StrSQL.AppendLine("       ," & Agro_SQL_SaveNum(Anno) & "   ")
            StrSQL.AppendLine("       ,'" & Agro_SQL_SaveText(Id_Riclassificazione) & "'  ")
            StrSQL.AppendLine("       ,'" & Agro_SQL_SaveText(Dare_Avere) & "'   ")
            StrSQL.AppendLine("       ," & Agro_SQL_SaveNum(Saldo) & "   ")
            StrSQL.AppendLine("       ," & Agro_SQL_SaveNum(Imputabile) & "   ")
            StrSQL.AppendLine("       ," & Agro_SQL_SaveNum(Saldo_Iniziale) & "   ")
            StrSQL.AppendLine("       ," & Agro_SQL_SaveNum(Codifica_Conto) & "   ")
            StrSQL.AppendLine("       ," & Agro_SQL_SaveNum(ChkImputazione_Automatica) & "   ")
            StrSQL.AppendLine("       , 0  ")
            StrSQL.AppendLine("        , Null  ")
            StrSQL.AppendLine("     , " & Agro_SQL_SaveDateTime(Now) & "  ")
            StrSQL.AppendLine("      , " & Agro_SQL_SaveDateTime(Now) & "  ")
            StrSQL.AppendLine("     ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.AppendLine("    ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.AppendLine("     , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            StrSQL.AppendLine("     , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")

            StrSQL.AppendLine(")")

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
    
    Public Function Update_CodificaConto_Base(ByRef objParametri As AgronicaCoreParametri) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.RicxConti_W.Update_CodificaConto_Base()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine(" -- valorizza Codifica_Conto (se = 0) per i conti economici per i quali serve il mapping  ")
            StrSQL.AppendLine(" UPDATE RicxConti ")
            StrSQL.AppendLine(" SET Codifica_Conto = Cod_Conto ")
            StrSQL.AppendLine(" WHERE Cod_Conto IN (" & CStr(enum_Conti_Economici.OmaggiAllaClientela) & "," & _
                                                        CStr(enum_Conti_Economici.RicavixIVAincompensazione) & ") ")
            StrSQL.AppendLine(" AND Codifica_Conto = 0 ")
            StrSQL.AppendLine(" AND EXISTS ( ")
            StrSQL.AppendLine(" 			SELECT 1 ")
            StrSQL.AppendLine(" 			FROM Riclassificazioni ")
            StrSQL.AppendLine(" 			WHERE Riclassificazioni.piva = RicxConti.piva ")
            StrSQL.AppendLine(" 			AND Riclassificazioni.Ric_Cod = RicxConti.Ric_Cod ")
            StrSQL.AppendLine("             AND Piva_Riferimento = '" & PIVA_BILANCIO_EUROPEO & "' ")
            StrSQL.AppendLine("             ) ")

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


    Public Function Modifica(ByVal PIVA As String,
                             ByVal Ric_Cod As Integer,
                             ByVal Cod_Conto As Integer,
                             ByVal Anno As Integer,
                             ByVal Id_Riclassificazione As String,
                             ByVal Dare_Avere As String,
                             ByVal Saldo As Decimal,
                             ByVal Imputabile As Integer,
                             ByVal Validita_Inizio As Date,
                             ByVal Validita_Fine As Date,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreParametri
                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.RicxConti_W.Modifica()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine(" UPDATE RicXConti SET ")
            StrSQL.AppendLine(" Id_Riclassificazione ='" & Agro_SQL_SaveText(Id_Riclassificazione) & "'  ")
            StrSQL.AppendLine(" ,Dare_Avere           ='" & Agro_SQL_SaveText(Dare_Avere) & "'  ")
            StrSQL.AppendLine(" ,Saldo             =" & Agro_SQL_SaveNum(Saldo) & "  ")
            StrSQL.AppendLine(" ,Imputabile        =" & Agro_SQL_SaveNum(Imputabile) & "   ")
            StrSQL.AppendLine("   ,Inviato           =  0 ")
            StrSQL.AppendLine("   ,DataInvio         =  Null ")
            StrSQL.AppendLine("   ,Data_Modifica     =  " & Agro_SQL_SaveDateTime(Now))
            StrSQL.AppendLine("   ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.AppendLine("   ,Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Inizio))
            StrSQL.AppendLine("   ,Validita_Fine     =  " & Agro_SQL_SaveDate(Validita_Fine))
            StrSQL.AppendLine(" WHERE Piva      = '" & Agro_SQL_SaveText(PIVA) & "'  ")
            StrSQL.AppendLine(" AND   Ric_Cod   =  " & Agro_SQL_SaveNum(Ric_Cod) & "   ")
            StrSQL.AppendLine(" AND   Cod_Conto =  " & Agro_SQL_SaveNum(Cod_Conto) & "   ")
            StrSQL.AppendLine(" AND   Anno      =  " & Agro_SQL_SaveNum(Anno) & "   ")

            '----------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
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
    
    Public Function Cancella(ByVal PIVA As String,
                             ByVal Ric_Cod As Integer,
                             ByVal Cod_Conto As Integer,
                             ByVal Anno As Integer,
                             ByVal xFiltroAggiuntivo As String,
                             ByVal objParametri As AgronicaCoreParametri
                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.RicxConti_W.Cancella()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            If PIVA = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
            End If
            '---------------------------------------------
            
            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then
                StrSQL.Length = 0
                StrSQL.AppendLine(" UPDATE  RicXConti ")
                StrSQL.AppendLine(" SET ")
                StrSQL.AppendLine("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.AppendLine("      ,Inviato = -1 ")
                StrSQL.AppendLine(" WHERE  Inviato >= 0 ")
            Else
                StrSQL.Length = 0
                StrSQL.AppendLine(" DELETE ")
                StrSQL.AppendLine(" FROM RicXConti ")
                StrSQL.AppendLine(" WHERE  Inviato >= 0 ")
            End If
            

            If PIVA <> "" Then
                StrSQL.AppendLine(" AND Piva = '" & Agro_SQL_SaveText(PIVA) & "' ")
            End If

            If Ric_Cod <> 0 Then
                StrSQL.AppendLine(" AND Ric_Cod = " & Agro_SQL_SaveNum(Ric_Cod) & "   ")
            End If

            If Cod_Conto <> 0 Then
                StrSQL.AppendLine(" AND Cod_Conto = " & Agro_SQL_SaveNum(Cod_Conto) & "   ")
            End If

            If Anno <> 0 Then
                StrSQL.AppendLine(" AND Anno = " & Agro_SQL_SaveNum(Anno) & "   ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
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

    Public Function Modifica_Solo_Riclassificazioni(ByVal PIVA As String,
                                                    ByVal Ric_Cod As Integer,
                                                    ByVal Anno As Integer,
                                                    ByVal Old_Id_Riclassificazione As String,
                                                    ByVal Id_Riclassificazione As String,
                                                    ByVal Validita_Inizio As Date,
                                                    ByVal Validita_Fine As Date,
                                                    ByVal xFiltroAggiuntivo As String,
                                                    ByRef objParametri As AgronicaCoreParametri
                                                    ) As Boolean
        
        Const nomeRoutine = "AgronicaCoreContabDAL.RicxConti_W.Modifica_Solo_Riclassificazioni()"
        
        Dim messaggioErrore As String = ""
        Dim StrSQL As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            If PIVA = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
            End If

            If Ric_Cod = 0 Then
                Throw New Exception("Parametro non corretto nella query (Ric_Cod obbligatorio)")
            End If

            If Anno = 0 Then
                Throw New Exception("Parametro non corretto nella query (Anno obbligatorio)")
            End If

            '---------------------------------------------

            StrSQL.AppendLine(" UPDATE RicxConti SET ")
            StrSQL.AppendLine(" Id_Riclassificazione = Replace(Id_Riclassificazione,'" & Agro_SQL_SaveText(Old_Id_Riclassificazione) & "', '" & Agro_SQL_SaveText(Id_Riclassificazione) & "') ")


            StrSQL.AppendLine(" WHERE Piva      = '" & Agro_SQL_SaveText(PIVA) & "'  ")
            StrSQL.AppendLine(" AND   Ric_Cod   =  " & Agro_SQL_SaveNum(Ric_Cod) & "   ")
            StrSQL.AppendLine(" AND   Anno      =  " & Agro_SQL_SaveNum(Anno) & "   ")
            StrSQL.AppendLine(" AND   Id_Riclassificazione LIKE '" & Old_Id_Riclassificazione & "%'  ")
            
            '----------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
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
    
    Public Function Modifica_NoSaldo(ByVal PIVA As String,
                                     ByVal Ric_Cod As Long,
                                     ByVal Cod_Conto As Long,
                                     ByVal Anno As Long,
                                     ByVal Id_Riclassificazione As String,
                                     ByVal Dare_Avere As String,
                                     ByVal Imputabile As Integer,
                                     ByVal Validita_Inizio As Date,
                                     ByVal Validita_Fine As Date,
                                     ByVal xFiltroAggiuntivo As String,
                                     ByRef objParametri As AgronicaCoreParametri
                                     ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.RicxConti_W.Modifica_NoSaldo()"
        
        Dim messaggioErrore As String = ""
        Dim StrSQL As New StringBuilder
        Dim xRisp As Boolean = False

        Try
            StrSQL.AppendLine(" UPDATE RicXConti SET ")
            StrSQL.AppendLine("  Id_Riclassificazione = '" & Agro_SQL_SaveText(Id_Riclassificazione) & "'  ")
            StrSQL.AppendLine("   ,Dare_Avere           = '" & Agro_SQL_SaveText(Dare_Avere) & "'  ")
            StrSQL.AppendLine("   ,Imputabile           =  " & Agro_SQL_SaveNum(Imputabile) & "   ")
            

            StrSQL.AppendLine("  ,Inviato           = 0 ")
            StrSQL.AppendLine("   ,DataInvio         = Null ")
            StrSQL.AppendLine("   ,Data_Modifica     =  " & Agro_SQL_SaveDateTime(Now) & "  ")
            StrSQL.AppendLine("   ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'  ")
            StrSQL.AppendLine("   ,Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Inizio))
            StrSQL.AppendLine("   ,Validita_Fine     =  " & Agro_SQL_SaveDate(Validita_Fine))
            StrSQL.AppendLine(" WHERE Piva      = '" & Agro_SQL_SaveText(PIVA) & "'  ")
            StrSQL.AppendLine(" AND   Ric_Cod   =  " & Agro_SQL_SaveNum(Ric_Cod) & "   ")
            StrSQL.AppendLine(" AND   Cod_Conto =  " & Agro_SQL_SaveNum(Cod_Conto) & "   ")
            StrSQL.AppendLine(" AND   Anno      =  " & Agro_SQL_SaveNum(Anno) & "   ")


            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
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
