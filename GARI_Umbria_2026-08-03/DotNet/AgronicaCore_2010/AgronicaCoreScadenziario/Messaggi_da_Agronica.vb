Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi


Public Class Messaggi_Da_Agronica_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Nascondi( _
                                ByVal ID_Sito As Integer, _
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                    ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreScadenziario_DAL.Alert_Tipologia_X_Utente_W.Scrivi()"

        '====================================================================================
        'Parametri opzionali :
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Dim objR As New AgronicaCoreScadenziario.Messaggi_Da_Agronica_R
        Dim dt As DataTable = objR.Leggi(ID_Sito, objParametri)
        Dim ID_Messaggio As Integer = dt.Rows(0).Item("ID_MEssaggio")

        dt = objR.Leggi_x_utente(ID_Messaggio, objParametri)

 
        Try

            If dt.Rows.Count = 0 Then
                'inserisco
                StrSQL.Length = 0
                StrSQL.Append("INSERT INTO Messaggi_da_Agronica_Utente ")
                StrSQL.Append("                   ( ID_Messaggio  ,Username       ,Attivo  ")
                StrSQL.Append("                   ) ")

                StrSQL.Append("VALUES (")


                StrSQL.Append("          " & Agro_SQL_SaveNum(ID_Messaggio) & "  ")
                StrSQL.Append("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("         , " & Agro_SQL_SaveNum(False) & "  ")

                StrSQL.Append(" )")
                '---------------------------------------------
                '--------------------------------------------------------------------------
                xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
                '--------------------------------------------------------------------------
            Else
                StrSQL.Append(" UPDATE Messaggi_da_Agronica_Utente SET ")
                StrSQL.Append("    Attivo           = " & Agro_SQL_SaveNum(False) & " ")

                StrSQL.Append(" WHERE   ID_Messaggio        =" & Agro_SQL_SaveNum(ID_Messaggio) & " ")
                StrSQL.Append(" AND     Username     = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")

                '--------------------------------------------------------------------------
                xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
                '--------------------------------------------------------------------------
            End If
             
        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return xRisp

    End Function



End Class

'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################

Public Class Messaggi_Da_Agronica_R
    Inherits AgronicaCoreDataProvider.DataProvider


    'Versione che utilizza AgronicaCoreParametri
    '##############################################################################################
    Public Function Leggi(ByVal ID_Sito As Integer, _
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                    ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreScadenziario_DAL.Alert_Elenco_x_Utente.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.Append(" select top 1 * from Messaggi_da_Agronica  ")
            StrSQL.Append(" where Id_Messaggio not in ( ")

            StrSQL.Append(" select ID_messaggio from Messaggi_da_Agronica_Utente ")
            StrSQL.Append(" where Username = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.Append(" AND Attivo = 0 ")

            StrSQL.Append(" )  ")
            StrSQL.Append(" AND ID_Sito = " & Agro_SQL_SaveNum(ID_Sito) & " ")

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try


        Return DT
         


    End Function

    '##############################################################################################
    Public Function Leggi_x_utente(ByVal ID_Messaggio As Integer, _
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                    ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreScadenziario_DAL.Alert_Elenco_x_Utente.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.Append(" select * from Messaggi_da_Agronica_Utente ")
            StrSQL.Append(" where Username = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.Append(" AND ID_Messaggio = " & Agro_SQL_SaveNum(ID_Messaggio))


            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try
        Return DT
    End Function

    '##############################################################################################

    Public Function Leggi_str(ByVal ID_Sito As Integer, _
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                    ) As String


        Dim NomeRoutine As String = "AgronicaCoreScadenziario_DAL.Alert_Elenco_x_Utente.Leggi_str()"
 
        Dim objM As New AgronicaCoreScadenziario.Messaggi_Da_Agronica_R
        Dim dt As DataTable = objM.Leggi(ID_Sito, objParametri)
         
        If DT.Rows.Count = 0 Then
            Return ""
        Else
            Return DT.Rows(0).Item("Descrizione")
        End If
    End Function

End Class
