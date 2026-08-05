Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.UtilityProvider

Module NewCOM

    Public Sub NewCom_Web_ComunicazionePagine_Scrivi( _
                                                        ByRef objParametri_Server As AgronicaCoreParametri, _
                                                        ByRef objSession As System.Web.SessionState.HttpSessionState, _
                                                        ByRef objPage As System.Web.UI.Page, _
                                                        ByRef NumeroRecordInteressati As Integer, _
                                                        ByVal Unid As String, _
                                                        ByVal Id_Riga As Integer, _
                                                        ByVal Tipo_Operazione As Integer, _
                                                        ByVal Flag_Errore As Integer, _
                                                        ByVal Errore As String, _
                                                        ByVal Chiave_Oggetto As String, _
                                                        ByVal Stringa_Parametri_Base As String, _
                                                        ByVal Stringa_Parametri_Rif As String) 'As String


        '----- Descrizione
        Dim DescrizioneFunzione As String = "Web_ComunicazionePagine_Scrivi"

        '----- Variabili
        Dim objSQL As New AgronicaCoreDataProvider.DataProvider
        Dim StrSQL As String
        Dim Messaggio As String
        'Dim NumeroRecordInteressati As Integer = 0
        'Dim Unid As String

        'Unid = System.Guid.NewGuid.ToString

        '----- Genero la query SQL 
        StrSQL = " "
        StrSQL += " INSERT INTO Web_ComunicazionePagine "

        StrSQL += " (Unid, Id_Riga, Tipo_Operazione, Flag_Errore, Errore, Chiave_Oggetto, Stringa_Parametri_Base, Stringa_Parametri_Rif, Data, Username) "

        StrSQL += " VALUES ('" + Agro_SQL_SaveText(Trim(Unid)) + "', "
        StrSQL += "'" + Agro_SQL_SaveText(Trim(Id_Riga)) + "', "
        StrSQL += "'" + Agro_SQL_SaveText(Trim(Tipo_Operazione)) + "', "
        StrSQL += "'" + Agro_SQL_SaveText(Trim(Flag_Errore)) + "', "
        StrSQL += "'" + Agro_SQL_SaveText(Trim(Errore)) + "', "
        StrSQL += "'" + Agro_SQL_SaveText(Trim(Chiave_Oggetto)) + "', "
        StrSQL += "'" + Agro_SQL_SaveText(Trim(Stringa_Parametri_Base)) + "', "
        StrSQL += "'" + Agro_SQL_SaveText(Trim(Stringa_Parametri_Rif)) + "', "
        StrSQL += Agro_SQL_SaveDate(Now.Today) & ", "
        StrSQL += "'" + Agro_SQL_SaveText(Trim(CStr(objSession("ASG_Utente_Username")))) & "' ) "

        '----- Recupero il recordset
        NumeroRecordInteressati = objSQL.EseguiQuery_Scrittura( _
            objParametri_server, _
            StrSQL, _
            DescrizioneFunzione _
        )

        objSQL = Nothing

        '----- Verifico la presenza di errori
        If Not IsNothing(Messaggio) Then
            Throw New Exception("Modulo NewCom : " & DescrizioneFunzione & " : " & Messaggio)
        End If

        'If NumeroRecordInteressati <> 0 Then
        '    Return Unid
        'Else
        '    Return ""
        'End If

    End Sub


    '################################################################################
    Public Function NewCom_Web_Parametri_Scrivi( _
                                    ByRef objServer As System.Web.HttpServerUtility, _
                                    ByRef objParametri_server As AgronicaCoreParametri, _
                                    ByRef objSession As System.Web.SessionState.HttpSessionState, _
                                    ByRef objPage As System.Web.UI.Page, _
                                    ByVal Connessione As String, _
                                    ByVal Stringa_Parametri As String) As String

        '----- Descrizione
        Dim DescrizioneFunzione As String = "Web_Parametri_Scrivi"

        '----- Variabili
        Dim objSQL As New AgronicaCoreDataProvider.DataProvider
        Dim StrSQL As String
        Dim Messaggio As String
        Dim NumeroRecordInteressati As Integer = 0

        Dim Unid As String

        Unid = System.Guid.NewGuid.ToString

        '----- Genero la query SQL 
        StrSQL = " "
        StrSQL += " INSERT INTO Web_Parametri "

        StrSQL += " (Unid, Stringa_Parametri, Data) "

        StrSQL += " VALUES ('" + Agro_SQL_SaveText(Trim(Unid)) + "', "
        StrSQL += "'" + Agro_SQL_SaveText(Trim(Stringa_Parametri)) + "', "
        StrSQL += Agro_SQL_SaveDate(Now.Today) & " ) "

        '----- Recupero il recordset
        NumeroRecordInteressati = objSQL.EseguiQuery_Scrittura( _
            objParametri_server, StrSQL, DescrizioneFunzione)

        objSQL = Nothing

        '----- Verifico la presenza di errori
        If Not IsNothing(Messaggio) Then
            Throw New Exception("Modulo NewCom : " & DescrizioneFunzione & " : " & Messaggio)
        End If

        If NumeroRecordInteressati <> 0 Then
            Return Unid
        Else
            Return ""
        End If

    End Function



    '################################################################################
    Public Function NewCom_Web_ComunicazionePagine_Cancella( _
                                                            ByRef objParametri_server As AgronicaCoreParametri, _
                                                            ByRef objSession As System.Web.SessionState.HttpSessionState, _
                                                            ByRef objPage As System.Web.UI.Page, _
                                                            ByVal Unid As String) As Integer

        '----- Descrizione
        Dim DescrizioneFunzione As String = "Web_ComunicazionePagine_Cancella"

        '----- Variabili
        Dim objSQL As New AgronicaCoreDataProvider.DataProvider
        Dim StrSQL As String
        Dim Messaggio As String
        Dim NumeroRecordInteressati As Integer = 0

        '----- Genero la query SQL 
        StrSQL = " "
        StrSQL += " DELETE FROM Web_ComunicazionePagine "

        If Unid <> "" Then
            StrSQL += " WHERE     (Unid = '" & Agro_SQL_SaveText(Unid) & "')   "
        End If

        '----- Recupero il recordset
        NumeroRecordInteressati = objSQL.EseguiQuery_Scrittura( _
            objParametri_server, _
            StrSQL, _
            DescrizioneFunzione _
            )



        '----- Verifico la presenza di errori
        If Not IsNothing(Messaggio) Then
            Throw New Exception("Modulo NewCom : " & DescrizioneFunzione & " : " & Messaggio)
        End If

        Return NumeroRecordInteressati

    End Function


    '################################################################################
    Public Function NewCom_Web_ComunicazionePagine_Leggi( _
                                                        ByRef objParametri_Server As AgronicaCoreParametri, _
                                                        ByRef objSession As System.Web.SessionState.HttpSessionState, _
                                                        ByRef objPage As System.Web.UI.Page, _
                                                        ByVal Unid As String) As DataTable

        '----- Descrizione
        Dim DescrizioneFunzione As String = "Web_ComunicazionePagine_Leggi"

        '----- Variabili
        Dim objSQL As New AgronicaCoreDataProvider.DataProvider
        Dim StrSQL As String
        Dim Messaggio As String
        Dim NumeroRecordInteressati As Integer = 0
        Dim Dt As DataTable

        '----- Genero la query SQL 
        StrSQL = " "
        StrSQL += " SELECT * FROM Web_ComunicazionePagine "
        StrSQL += " WHERE Unid ='" + Agro_SQL_SaveText(Trim(Unid)) + "'"

        'Recupero il datatable
        Dt = objSQL.EseguiQuery_Lettura( _
                        objParametri_Server, _
                        StrSQL, _
                        "")



        'Verifico la presenza di errori
        If Not IsNothing(Messaggio) Then
            Throw New Exception("Modulo NewCom : " & DescrizioneFunzione & " : " & Messaggio)
            Return Nothing
        Else
            Return Dt
        End If

    End Function


End Module
