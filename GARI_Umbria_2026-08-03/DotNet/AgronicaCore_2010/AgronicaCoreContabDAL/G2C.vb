


Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.AgronicaCoreParametri

Public Class G2C_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function Leggi_G2C_Log(ByVal Piva As String,
                                  ByVal Id_Agenda As Integer,
                                  ByVal xFiltroAggiuntivo As String,
                                  ByVal xOrderBy As String,
                                  ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                  ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCore_DAL.Leggi_G2C_Log()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim PivaSuperUser As String = objParametri.PivaSuperUser

        Try

            Stb.Length = 0

            Stb.Append(" SELECT * " + vbCrLf)
            Stb.Append(" From  G2C_Log" + vbCrLf)
            Stb.AppendLine(" WHERE PivaSuperUSer = '" & PivaSuperUser & "' ")

            If Trim(Piva) <> "" Then
                Stb.AppendLine(" And Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If

            If Trim(Id_Agenda) <> 0 Then
                Stb.AppendLine(" And Id_Agenda = " & Id_Agenda & " ")
            End If

            If xFiltroAggiuntivo <> "" Then
                Stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    Stb.Append(" AND   Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    Stb.Append(" AND   Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                Stb.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, Stb.ToString, NomeRoutine)
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


'#################################################################
'#################################################################
'#################################################################

Public Class G2C_W
    Inherits AgronicaCoreDataProvider.DataProvider



    '##############################################################################################
    Public Function Scrivi_G2C_Log(
                          ByVal Piva As String _
                        , ByVal Id_Agenda As Integer _
                        , ByVal Return_Code As String _
                        , ByVal Return_Error As String _
                        , ByVal Stato_Export As Integer _
                        , ByVal Stato_Export_2 As Integer _
                        , ByVal Validita_Inizio As DateTime _
                        , ByVal Validita_Fine As DateTime _
                        , ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                        , Optional ByVal Data_creazione As Date = #2/1/1900# _
                        , Optional ByVal Data_modifica As Date = #2/1/1900# _
                        , Optional ByVal username_creazione As String = "" _
                        , Optional ByVal username_modifica As String = ""
                        ) As Boolean


        Dim NomeRoutine As String = "Scrivi_G2C_Log()"

        '====================================================================================
        'Parametri opzionali :

        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim xRisp As Boolean = False
        Dim PivaSuperUser As String = objParametri.PivaSuperUser

        Try

            If Data_creazione = #2/1/1900# Then
                Data_creazione = Date.Now
            End If

            If Data_modifica = #2/1/1900# Then
                Data_modifica = Date.Now
            End If

            If username_creazione = "" Then
                username_creazione = objParametri.UsernameOperazione
            End If

            If username_modifica = "" Then
                username_modifica = objParametri.UsernameOperazione
            End If



            '---------------------------------------------
            Stb.Length = 0
            Stb.Append(" INSERT G2C_Log " + vbCrLf)


            Stb.Append("              (")

            Stb.Append("   [PivaSuperUser] " & vbCrLf)
            Stb.Append("  ,[piva] " & vbCrLf)
            Stb.Append("  ,[id_Agenda] " & vbCrLf)
            Stb.Append("  ,[Return_Code] " & vbCrLf)
            Stb.Append("  ,[Return_Error] " & vbCrLf)
            Stb.Append("              ,Inviato,            datainvio, ")
            Stb.Append("              Data_Creazione,     Data_Modifica, ")
            Stb.Append("              UserName_Creazione, UserName_Modifica, ")
            Stb.Append("              Validita_Inizio,    Validita_Fine ")
            Stb.Append("              ) ")

            Stb.Append(" VALUES ( ")

            Stb.Append("  '" & Agro_SQL_SaveText(PivaSuperUser) & "' " & vbCrLf)
            Stb.Append(", '" & Agro_SQL_SaveText(Piva) & "' " & vbCrLf)
            Stb.Append(", " & Agro_SQL_SaveNum(Id_Agenda) & " " & vbCrLf)
            Stb.Append(", '" & Agro_SQL_SaveText(Return_Code) & "' " & vbCrLf)
            Stb.Append(", '" & Agro_SQL_SaveText(Return_Error) & "' " & vbCrLf)

            Stb.Append("         , 0  " + vbCrLf)
            Stb.Append("         , Null  " + vbCrLf)

            Stb.Append("			, " & Agro_SQL_SaveDate(Data_creazione) & "  ")
            Stb.Append("			, " & Agro_SQL_SaveDate(Data_modifica) & "  ")
            Stb.Append("			,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            Stb.Append("			,'" & Agro_SQL_SaveText(username_modifica) & "' ")

            Stb.Append("			, " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            Stb.Append("			, " & Agro_SQL_SaveDate(Validita_Fine) & "  ")

            Stb.Append(") ")



            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------



            'Update Stato Export Tabella Agenda
            Stb.Length = 0
            Stb.Append(" Update Agenda " + vbCrLf)
            Stb.Append(" Set Stato_Export = " & Stato_Export & " ")
            Stb.Append(", Stato_Export_2 = " & Stato_Export_2 & " ")
            Stb.Append("  Where Id_Agenda = " & Id_Agenda & " ")

            If Trim(Piva) <> "" Then
                Stb.Append("  And Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If


            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            xRisp = AggiornaStatoExport(Piva, Id_Agenda, "", Stato_Export, Stato_Export_2, objParametri, Data_modifica, username_modifica)




        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return xRisp

    End Function




    '##############################################################################################
    Public Function AggiornaStatoExport(
                          ByVal Piva As String _
                        , ByVal Id_Agenda As Integer _
                        , ByVal xFiltroAggiuntivo As String _
                        , ByVal Stato_Export As Integer _
                        , ByVal Stato_Export_2 As Integer _
                        , ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                        , Optional ByVal Data_modifica As Date = #2/1/1900# _
                        , Optional ByVal username_modifica As String = ""
                        ) As Boolean


        Dim NomeRoutine As String = "AggiornaStatoExport()"

        '====================================================================================
        'Parametri opzionali :

        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim xRisp As Boolean = False
        Dim PivaSuperUser As String = objParametri.PivaSuperUser

        Try


            If Data_modifica = #2/1/1900# Then
                Data_modifica = Date.Now
            End If


            If username_modifica = "" Then
                username_modifica = objParametri.UsernameOperazione
            End If




            'Update Stato Export Tabella Agenda
            Stb.Length = 0
            Stb.Append(" Update Agenda " + vbCrLf)
            Stb.Append(" Set Stato_Export = " & Stato_Export & " ")
            Stb.Append(", Stato_Export_2 = " & Stato_Export_2 & " ")
            Stb.Append(" Where 1 = 1 ")

            If Trim(Piva) <> "" Then
                Stb.AppendLine(" And Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If

            If Trim(Id_Agenda) <> 0 Then
                Stb.AppendLine(" And Id_Agenda = " & Id_Agenda & " ")
            End If

            '---------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                Stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If



            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------


        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return xRisp

    End Function





    '#################################################################
    Public Function Cancella_G2C_Log(ByVal Piva As String _
                                   , ByVal Id_Agenda As Integer _
                                   , ByVal xFiltroAggiuntivo As String _
                                   , ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                   , Optional ByVal Data_modifica As Date = #2/1/1900# _
                                   , Optional ByVal username_modifica As String = ""
                                   ) As Boolean


        '----- Descrizione
        Dim NomeRoutine As String = "Cancella_G2C_Log()"

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim xRisp As Boolean = False
        Dim PivaSuperUser As String = objParametri.PivaSuperUser

        Try


            If Data_modifica = #2/1/1900# Then
                Data_modifica = Date.Now
            End If


            If username_modifica = "" Then
                username_modifica = objParametri.UsernameOperazione
            End If


            '---------------------------------------------
            Stb.Length = 0

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then
                Stb.Append(" UPDATE G2C_Log ")
                Stb.Append(" SET ")
                Stb.Append("         Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                Stb.Append("         ,Data_Modifica= " & Agro_SQL_SaveDate(Date.Now) & " ")
                Stb.Append("         ,Inviato = -1 ")
                Stb.Append(" WHERE   PivaSuperUser = '" & PivaSuperUser & "' ")
                Stb.Append(" AND     Inviato >= 0 ")
            Else
                Stb.Append(" DELETE FROM G2C_Log ")
                Stb.Append(" WHERE PivaSuperUser = '" & PivaSuperUser & "' ")
            End If


            If Trim(Piva) <> "" Then
                Stb.AppendLine(" And Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If

            If Trim(Id_Agenda) <> 0 Then
                Stb.AppendLine(" And Id_Agenda = " & Id_Agenda & " ")
            End If

            '---------------------------------------------

            If xFiltroAggiuntivo <> "" Then
                Stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            xRisp = AggiornaStatoExport(Piva, Id_Agenda, xFiltroAggiuntivo, 0, 0, objParametri, Data_modifica, username_modifica)
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





