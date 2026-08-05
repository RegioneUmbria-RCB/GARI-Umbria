Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi

Public Class Alert_Elenco_x_Utente_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Scrivi( _
                                ByVal ID_Elenco As Integer, _
                                ByVal Non_Mostrare As Integer, _
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                    ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreScadenziario_DAL.Alert_Elenco_x_Utente_W.Scrivi()"

        '====================================================================================
        'Parametri opzionali :
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append("INSERT INTO Alert_Elenco_X_Utente ")
            StrSQL.Append("                   ( PivaSuperUser    ,ID_Elenco  ,Username , Non_Mostrare ")


            StrSQL.Append("                    ,Inviato,            DataInvio, ")
            StrSQL.Append("                    Data_Creazione,     Data_Modifica, ")
            StrSQL.Append("                    UserName_Creazione, UserName_Modifica  ")
            StrSQL.Append("                   ) ")

            StrSQL.Append("VALUES (")


            StrSQL.Append("          '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(ID_Elenco) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Non_Mostrare) & "  ")
             

            StrSQL.Append("         , 0 ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Date.Now))
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Date.Now) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Date.Now) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.Append(" )")
            '---------------------------------------------


            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return xRisp

    End Function


    Public Function Modifica( _
                                ByVal ID_Elenco As Integer, _
                                ByVal Non_Mostrare As Integer, _
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                        ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreScadenziario_DAL.Alert_Elenco_x_Utente_W.Modifica()"

        '====================================================================================
        'Parametri opzionali :
        '   Latitudine = 0
        '   Longitudine = 0
        '
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        '------------------------------

        Try


            StrSQL.Append(" UPDATE Alert_Elenco_X_Utente SET ")
            StrSQL.Append("    Non_Mostrare           = " & Agro_SQL_SaveNum(Non_Mostrare) & " ") 

            StrSQL.Append("   ,Inviato           =  0 ")
            StrSQL.Append("   ,DataInvio         =  Null ")
            StrSQL.Append("   ,Data_Modifica     =  " & Agro_SQL_SaveDate(Date.Now))
            StrSQL.Append("   ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")

            StrSQL.Append(" WHERE   ID_Elenco        =" & Agro_SQL_SaveNum(ID_Elenco) & " ")
            StrSQL.Append(" AND     PivaSuperUser      = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'")
            StrSQL.Append(" AND     UserName      = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")


            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp


    End Function



    Public Function Cancella( _
                                ByVal ID_Elenco As Integer, _
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                        ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreScadenziario_DAL.Alert_Elenco_x_Utente_W.Cancella()"
         
        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = AgronicaCoreDataProvider.AgronicaCoreParametri.enumCancellazioneLogica.CancellazioneLogica Then

                StrSQL.Length = 0
                StrSQL.Append(" UPDATE Alert_Elenco_x_Utente ")
                StrSQL.Append(" SET ")
                StrSQL.Append("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("      ,Inviato = -1 ")
                StrSQL.Append(" WHERE  Inviato >= 0 ")

            Else
                StrSQL.Length = 0
                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM Alert_Elenco_x_Utente ")
                StrSQL.Append(" WHERE  1=1 ")

            End If

            StrSQL.Append(" AND ID_Elenco = " & Agro_SQL_SaveNum(ID_Elenco) & " ")
            StrSQL.Append(" AND pivasuperuser =    '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.Append(" AND username =    '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

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


Public Class Alert_Elenco_x_Utente_R
    Inherits AgronicaCoreDataProvider.DataProvider


    'Versione che utilizza AgronicaCoreParametri
    '##############################################################################################
    Public Function Leggi(ByVal ID_Elenco As Integer, _
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                    ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreScadenziario_DAL.Alert_Elenco_x_Utente.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT ID_Elenco, Non_Mostrare  ")
            StrSQL.Append(" FROM  Alert_Elenco_x_Utente ")
            StrSQL.Append(" WHERE 1= 1 ")


            If ID_Elenco <> 0 Then
                StrSQL.Append(" AND ID_Elenco = " & Agro_SQL_SaveNum(ID_Elenco) & " ")
            End If
             
            StrSQL.Append(" AND username =    '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

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

End Class
