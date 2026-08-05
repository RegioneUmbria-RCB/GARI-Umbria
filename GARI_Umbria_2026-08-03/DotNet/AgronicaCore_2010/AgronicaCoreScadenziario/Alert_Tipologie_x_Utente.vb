Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi

Public Class Alert_Tipologia_X_Utente_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Scrivi( _
                                ByVal ID_Tipologia As Integer, _
                                ByVal Attivo As Integer, _
                                ByVal Preavviso As Integer, _
                                ByVal Colore As String, _
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

        Try

            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append("INSERT INTO Alert_Tipologia_X_Utente ")
            StrSQL.Append("         ( PivaSuperUser, ID_Tipologia, Username, Attivo, Preavviso, Colore, ")


            StrSQL.Append("         Inviato,            DataInvio, ")
            StrSQL.Append("         Data_Creazione,     Data_Modifica, ")
            StrSQL.Append("         UserName_Creazione, UserName_Modifica ")
            StrSQL.Append("         ) ")

            StrSQL.Append("VALUES (")


            StrSQL.Append("          '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(ID_Tipologia) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Attivo) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Preavviso) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Colore) & "' ")

            StrSQL.Append("         , 0 ")
            StrSQL.Append("         , Null ")
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
                                ByVal ID_Tipologia As Integer, _
                                ByVal Attivo As Integer, _
                                ByVal Preavviso As Integer, _
                                ByVal Colore As String, _
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                        ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreScadenziario_DAL.Alert_Tipologia_X_Utente_W.Modifica()"

         

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        '------------------------------

        Try

            StrSQL.Length = 0

            StrSQL.Append(" UPDATE Alert_Tipologia_X_Utente SET ")
            StrSQL.Append("    Attivo           = " & Agro_SQL_SaveNum(Attivo) & " ")
            StrSQL.Append("   ,Preavviso        = " & Agro_SQL_SaveNum(Preavviso) & " ")
            StrSQL.Append("   ,Colore           =  '" & Agro_SQL_SaveText(Colore) & "' ")

            StrSQL.Append("   ,Inviato           =  0 ")
            StrSQL.Append("   ,DataInvio         =  Null ")
            StrSQL.Append("   ,Data_Modifica     =  " & Agro_SQL_SaveDate(Date.Now))
            StrSQL.Append("   ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")

            StrSQL.Append(" WHERE   ID_Tipologia        =" & Agro_SQL_SaveNum(ID_Tipologia) & " ")
            StrSQL.Append(" AND     PivaSuperUser      = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'")
            StrSQL.Append(" AND     Username     = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")

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

    Public Function Modifica_Attivo( _
                                ByVal ID_Tipologia As Integer, _
                                ByVal Attivo As Integer, _
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                        ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreScadenziario_DAL.Alert_Tipologia_X_Utente_W.Modifica()"

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

            StrSQL.Length = 0

            StrSQL.Append(" UPDATE Alert_Tipologia_X_Utente SET ")
            StrSQL.Append("    Attivo           = " & Agro_SQL_SaveNum(Attivo) & " ")

            StrSQL.Append("   ,Inviato           =  0 ")
            StrSQL.Append("   ,DataInvio         =  Null ")
            StrSQL.Append("   ,Data_Modifica     =  " & Agro_SQL_SaveDate(Date.Now))
            StrSQL.Append("   ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")

            StrSQL.Append(" WHERE   ID_Tipologia        =" & Agro_SQL_SaveNum(ID_Tipologia) & " ")
            StrSQL.Append(" AND     PivaSuperUser      = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'")
            StrSQL.Append(" AND     Username     = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")

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


    Public Function Modifica_Colore( _
                                ByVal ID_Tipologia As Integer, _
                                ByVal Colore As String, _
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                        ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreScadenziario_DAL.Alert_Tipologia_X_Utente_W.Modifica()"

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

            StrSQL.Length = 0

            StrSQL.Append(" UPDATE Alert_Tipologia_X_Utente SET ")
            StrSQL.Append("    Colore           = '" & Agro_SQL_SaveText(Colore) & "' ")

            StrSQL.Append("   ,Inviato           =  0 ")
            StrSQL.Append("   ,DataInvio         =  Null ")
            StrSQL.Append("   ,Data_Modifica     =  " & Agro_SQL_SaveDate(Date.Now))
            StrSQL.Append("   ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")

            StrSQL.Append(" WHERE   ID_Tipologia        =" & Agro_SQL_SaveNum(ID_Tipologia) & " ")
            StrSQL.Append(" AND     PivaSuperUser      = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'")
            StrSQL.Append(" AND     Username     = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")

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

    Public Function Modifica_Preavviso( _
                                ByVal ID_Tipologia As Integer, _
                                ByVal Preavviso As String, _
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                        ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreScadenziario_DAL.Alert_Tipologia_X_Utente_W.Modifica()"

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

            StrSQL.Length = 0

            StrSQL.Append(" UPDATE Alert_Tipologia_X_Utente SET ")
            StrSQL.Append("    Preavviso           = " & Agro_SQL_SaveNum(Preavviso) & " ")

            StrSQL.Append("   ,Inviato           =  0 ")
            StrSQL.Append("   ,DataInvio         =  Null ")
            StrSQL.Append("   ,Data_Modifica     =  " & Agro_SQL_SaveDate(Date.Now))
            StrSQL.Append("   ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")

            StrSQL.Append(" WHERE   ID_Tipologia        =" & Agro_SQL_SaveNum(ID_Tipologia) & " ")
            StrSQL.Append(" AND     PivaSuperUser      = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'")
            StrSQL.Append(" AND     Username     = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")

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
                               ByVal ID_Tipologia As Integer, _
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                        ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreScadenziario_DAL.Alert_Tipologia_X_Utente_W.Cancella()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""
        '   Sa_Cod = 0
        '   Id_Trasformazione = 0

        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = AgronicaCoreDataProvider.AgronicaCoreParametri.enumCancellazioneLogica.CancellazioneLogica Then

                StrSQL.Length = 0
                StrSQL.Append(" UPDATE Alert_Tipologia_X_Utente ")
                StrSQL.Append(" SET ")
                StrSQL.Append("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("      ,Inviato = -1 ")
                StrSQL.Append(" WHERE  Inviato >= 0 ")

            Else
                StrSQL.Length = 0
                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM Alert_Tipologia_X_Utente ")
                StrSQL.Append(" WHERE  1=1 ")

            End If

            StrSQL.Append(" AND ID_Tipologia = " & Agro_SQL_SaveNum(ID_Tipologia) & " ")
            StrSQL.Append(" AND Username =    '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")

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


    Public Function Cancella_TUTTO( _
                                 ByVal ID_Tipologia As Integer, _
                                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                          ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreScadenziario_DAL.Alert_Tipologia_X_Utente_W.Cancella()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""
        '   Sa_Cod = 0
        '   Id_Trasformazione = 0

        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = AgronicaCoreDataProvider.AgronicaCoreParametri.enumCancellazioneLogica.CancellazioneLogica Then

                StrSQL.Length = 0
                StrSQL.Append(" UPDATE Alert_Tipologia_X_Utente ")
                StrSQL.Append(" SET ")
                StrSQL.Append("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("      ,Inviato = -1 ")
                StrSQL.Append(" WHERE  Inviato >= 0 ")

            Else
                StrSQL.Length = 0
                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM Alert_Tipologia_X_Utente ")
                StrSQL.Append(" WHERE  1=1 ")

            End If

            StrSQL.Append(" AND ID_Tipologia = " & Agro_SQL_SaveNum(ID_Tipologia) & " ")

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


Public Class Alert_Tipologia_X_Utente_R
    Inherits AgronicaCoreDataProvider.DataProvider
    Public Function Leggi(ByVal ID_Tipologia As Integer, _
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                    ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreScadenziario_DAL.Alert_Tipologia_X_Utente_R.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""                   =>  si leggono tutte le imprese
        '   Sa_Cod = 0
        '   Appezza = 0
        '
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT ID_Tipologia, Username,Attivo ,Preavviso , Colore")
            StrSQL.Append(" FROM  Alert_Tipologia_X_Utente ")

            StrSQL.Append(" WHERE 1=1 ")

            If ID_Tipologia <> 0 Then
                StrSQL.Append(" AND ID_Tipologia = " & Agro_SQL_SaveNum(ID_Tipologia) & " ")
            End If

            StrSQL.Append(" AND Username = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")

            StrSQL.Append(" AND PivaSuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")


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




    Public Function Leggi_Elenco(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                  ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreScadenziario_DAL.Alert_Tipologia_X_Utente_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT        Alert_Tipologia_X_Utente.ID_Tipologia, Alert_Tipologia_X_Utente.Username, ")
            StrSQL.Append("     Alert_Tipologia_X_Utente.Attivo, Alert_Tipologia_X_Utente.Preavviso, ")
            StrSQL.Append("     Alert_Tipologia_X_Utente.Colore, Alert_Area.Nome AS Nome_Area , Alert_Tipologia.Nome  ")
            StrSQL.Append(" FROM            Alert_Tipologia_X_Utente ")
            StrSQL.Append(" INNER Join Alert_Tipologia ON Alert_Tipologia_X_Utente.ID_Tipologia = Alert_Tipologia.ID_Tipologia ")
            StrSQL.Append(" INNER JOIN Alert_Area ON Alert_Tipologia.ID_Area = Alert_Area.ID_Area")

            StrSQL.Append(" WHERE 1=1 ")

            StrSQL.Append(" AND Alert_Tipologia_X_Utente.Username = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")

            StrSQL.Append(" AND PivaSuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

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

    Public Function Leggi_Non_Gestiti(ByVal id_attivita As String, _
                                 ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                  ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreScadenziario_DAL.Alert_Tipologia_X_Utente_R.Leggi_Non_Gestiti()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.Append(" select * ")
            StrSQL.Append(" from alert_tipologia ")
            StrSQL.Append("     INNER JOIN		Alert_Area on Alert_Area.Id_Area = Alert_Tipologia.ID_Area ")
            StrSQL.Append("     INNER JOIN		Tipo_Entita ON Alert_Area.TipoEntita_Cod = Tipo_Entita.TipoEntita_Cod  ")

            StrSQL.Append(" where id_attivita in (" & Agro_SQL_Save_Clausola_IN(id_attivita) & ")")
            StrSQL.Append("     and ID_Tipologia not in (select ID_Tipologia from Alert_Tipologia_X_Utente  ")
            StrSQL.Append(" where Alert_Tipologia_X_Utente.username = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.Append(" AND PivaSuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ) ")

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


 





