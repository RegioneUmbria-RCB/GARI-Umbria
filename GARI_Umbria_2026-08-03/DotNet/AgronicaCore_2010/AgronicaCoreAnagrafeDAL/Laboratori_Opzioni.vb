Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports Newtonsoft.Json

Public Class Laboratori_Opzioni_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function Leggi(ByVal Cod_Risum As Integer,
                          ByVal Tipo_Opzione As Integer,
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                          ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Laboratori_Opzioni_R.Leggi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim dt As DataTable

        Try
            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.AppendLine(" SELECT *  FROM Laboratori_Opzioni ")
            StrSQL.AppendLine(" where 1 =1 ")

            If Cod_Risum <> 0 Then
                StrSQL.AppendLine(" AND Cod_Risum = " & Agro_SQL_SaveNum(Cod_Risum) & "  ")
            End If
            
            If Tipo_Opzione <> 0 Then
                StrSQL.AppendLine(" AND Tipo_Opzione = " & Agro_SQL_SaveNum(Tipo_Opzione) & " ")
            End If

            '-------------------------------------------------------------------------- 
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.AppendLine(" AND    Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.AppendLine(" AND    Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
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

    Public Function LeggiOpzioniLaboratorioOggetto(ByVal Cod_Risum As Integer,
                                                   ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                   ) As Oggetto_Laboratorio_Opzioni

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Laboratori_Opzioni_R.LeggiOpzioniLaboratorioOggetto()"

        Dim messaggioErrore As String = ""
        Dim objOpzioni As New Oggetto_Laboratorio_Opzioni

        Try

            If Cod_Risum = 0 Then
                Throw New Exception("Parametro Cod_Risum è necessario")
            End If

            Dim dt As DataTable = Leggi(Cod_Risum, 0, objParametri)

            If Not dt Is Nothing AndAlso dt.Rows.Count > 0 Then

                For Each dr In dt.Rows

                    Select Case dr.Item("Tipo_Opzione")
                        Case enum_Opzioni_Laboratori.Bool_Mail_Automatica_Invio
                            If dr.Item("Valore") = True Then
                                objOpzioni.Bool_Mail_Automatica_Invio = True
                            End If
                        Case enum_Opzioni_Laboratori.Mail_Automatica_A
                            objOpzioni.Mail_Automatica_A = Trim(dr.Item("Valore"))
                        Case enum_Opzioni_Laboratori.Mail_Automatica_CC
                            objOpzioni.Mail_Automatica_CC = Trim(dr.Item("Valore"))
                        Case enum_Opzioni_Laboratori.Mail_Rispondi_A
                            objOpzioni.Mail_Rispondi_A = Trim(dr.Item("Valore"))
                        Case enum_Opzioni_Laboratori.Mail_Oggetto
                            objOpzioni.Mail_Oggetto = dr.Item("Valore")
                        Case enum_Opzioni_Laboratori.Mail_Firma
                            objOpzioni.Mail_Firma = dr.Item("Valore")

                        Case enum_Opzioni_Laboratori.Bool_Mail_Automatica_Invio_ALERT
                            If dr.Item("Valore") = True Then
                                objOpzioni.Bool_Mail_Automatica_Invio_ALERT = True
                            End If
                        Case enum_Opzioni_Laboratori.Mail_Automatica_A_ALERT
                            objOpzioni.Mail_Automatica_A_ALERT = Trim(dr.Item("Valore"))
                        Case enum_Opzioni_Laboratori.Mail_Automatica_CC_ALERT
                            objOpzioni.Mail_Automatica_CC_ALERT = Trim(dr.Item("Valore"))
                        Case enum_Opzioni_Laboratori.Mail_Rispondi_A_ALERT
                            objOpzioni.Mail_Rispondi_A_ALERT = Trim(dr.Item("Valore"))

                        Case enum_Opzioni_Laboratori.Tipo_Automatismo_Richiesta_Analisi
                            objOpzioni.Tipo_Automatismo_Richiesta_Analisi = If(dr.Item("Valore") <> "", CInt(dr.Item("Valore")), 0)
                        Case enum_Opzioni_Laboratori.Richiesta_Rest_TipoAPI
                            objOpzioni.Richiesta_Rest_TipoAPI = Trim(dr.Item("Valore"))
                        Case enum_Opzioni_Laboratori.Richiesta_Rest_Endpoint
                            objOpzioni.Richiesta_Rest_Endpoint = Trim(dr.Item("Valore"))
                        Case enum_Opzioni_Laboratori.Richiesta_Rest_Authorization
                            Dim val As String = Trim(dr.Item("Valore"))
                            If val <> "" Then
                                objOpzioni.Richiesta_Rest_Authorization = JsonConvert.DeserializeObject(val, GetType(Oggetto_Laboratorio_Opzioni.Api_Authorization))
                            End If
                        Case enum_Opzioni_Laboratori.Tipo_Automatismo_Risultato_Analisi
                            objOpzioni.Tipo_Automatismo_Risultato_Analisi = If(dr.Item("Valore") <> "", CInt(dr.Item("Valore")), 0)
                        Case enum_Opzioni_Laboratori.Risultato_Rest_TipoAPI
                            objOpzioni.Risultato_Rest_TipoAPI = Trim(dr.Item("Valore"))

                    End Select

                Next

            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return objOpzioni

    End Function

End Class


Public Class Laboratori_Opzioni_W
    Inherits AgronicaCoreDataProvider.DataProvider


    Public Function Scrivi(ByVal Cod_RisUm As Integer,
                           ByVal Tipo_Opzione As Integer,
                           ByVal Valore As String,
                           ByVal Validita_Inizio As Date,
                           ByVal Validita_Fine As Date,
                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                           ) As Boolean

        Const nomeRoutine = "AnagrafeCoreAnagrafeDAL.Laboratori_Opzioni_W.Scrivi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.AppendLine("INSERT INTO Laboratori_Opzioni(       ")
            StrSQL.AppendLine("                    PivaSuperUser,         ")
            StrSQL.AppendLine("                    Cod_Risum,                Tipo_Opzione,      ")
            StrSQL.AppendLine("                    Valore,      ")
            StrSQL.AppendLine("                    Data_Creazione,      Data_Modifica, ")
            StrSQL.AppendLine("                    UserName_Creazione,  UserName_Modifica, ")
            StrSQL.AppendLine("                    Validita_Inizio,     Validita_Fine , inviato")
            StrSQL.AppendLine("                    ) ")
            StrSQL.AppendLine("VALUES (")
            StrSQL.AppendLine("          '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.AppendLine("         ," & Agro_SQL_SaveNum(Cod_RisUm) & " ")
            StrSQL.AppendLine("         ," & Agro_SQL_SaveNum(Tipo_Opzione) & " ")
            StrSQL.AppendLine("         , '" & Agro_SQL_SaveText(Valore) & "' ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDateTime(Now) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDateTime(Now) & "  ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDate(Validita_Fine) & "   ")
            StrSQL.AppendLine("         , 0  ")
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

    Public Function Cancella(ByVal Cod_RisUm As Integer,
                             ByVal Tipo_Opzione As Integer,
                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                             ) As Boolean

        Const nomeRoutine = "AnagrafeCoreAnagrafeDAL.Laboratori_Utenti_W.Cancella()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try


            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = AgronicaCoreDataProvider.AgronicaCoreParametri.enumCancellazioneLogica.CancellazioneLogica Then

                StrSQL.Length = 0
                StrSQL.AppendLine(" UPDATE Laboratori_Opzioni ")
                StrSQL.AppendLine(" SET ")
                StrSQL.AppendLine("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.AppendLine("      ,Inviato = -1 ")
                StrSQL.AppendLine(" WHERE  PivaSuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
                StrSQL.AppendLine(" AND Inviato >= 0")
            Else

                StrSQL.Length = 0
                StrSQL.AppendLine(" DELETE ")
                StrSQL.AppendLine(" FROM     Laboratori_Opzioni ")
                StrSQL.AppendLine(" WHERE  PivaSuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            End If

            If Cod_RisUm <> 0 Then
                StrSQL.AppendLine(" AND cod_Risum = " & Agro_SQL_SaveNum(Cod_RisUm))
            End If
            If Tipo_Opzione <> 0 Then
                StrSQL.AppendLine(" AND Tipo_Opzione = " & Agro_SQL_SaveNum(Tipo_Opzione))
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

Public Class Oggetto_Laboratorio_Opzioni

    Public Sub New()
        Bool_Mail_Automatica_Invio = False
        Mail_Automatica_A = ""
        Mail_Automatica_CC = ""
        Mail_Rispondi_A = ""
        Mail_Oggetto = ""
        Mail_Firma = ""
        Bool_Mail_Automatica_Invio_ALERT = False
        Mail_Automatica_A_ALERT = ""
        Mail_Automatica_CC_ALERT = ""
        Mail_Rispondi_A_ALERT = ""
        Tipo_Automatismo_Richiesta_Analisi = 0
        Richiesta_Rest_TipoAPI = ""
        Richiesta_Rest_Endpoint = ""
        Richiesta_Rest_Authorization = New Api_Authorization()
        Tipo_Automatismo_Risultato_Analisi = 0
        Risultato_Rest_TipoAPI = ""
    End Sub

    Public Property Bool_Mail_Automatica_Invio As Boolean
    Public Property Mail_Automatica_A As String
    Public Property Mail_Automatica_CC As String
    Public Property Mail_Rispondi_A As String
    Public Property Mail_Oggetto As String
    Public Property Mail_Firma As String
    Public Property Bool_Mail_Automatica_Invio_ALERT As Boolean
    Public Property Mail_Automatica_A_ALERT As String
    Public Property Mail_Automatica_CC_ALERT As String
    Public Property Mail_Rispondi_A_ALERT As String
    Public Property Tipo_Automatismo_Richiesta_Analisi As Integer
    Public Property Richiesta_Rest_TipoAPI As String
    Public Property Richiesta_Rest_Endpoint As String
    Public Property Richiesta_Rest_Authorization As Api_Authorization
    Public Property Tipo_Automatismo_Risultato_Analisi As Integer
    Public Property Risultato_Rest_TipoAPI As String

    Public Class Api_Authorization
        Public Property Chiave As String
        Public Property Valore As String

        Public Sub New()
            Chiave = ""
            Valore = ""
        End Sub
    End Class
End Class
