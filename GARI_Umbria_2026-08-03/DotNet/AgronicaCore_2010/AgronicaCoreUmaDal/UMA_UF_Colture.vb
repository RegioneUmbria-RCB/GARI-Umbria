Imports System.IO
Imports System.Text
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreUmaDal.UMASetup_W

Public Class UMA_UF_Colture_R
    Inherits DataProvider

    ''' <summary>
    ''' Restituisce le UF prodotte della coltura interessata
    ''' </summary>
    ''' <param name="occupazione_Cod"></param>
    ''' <param name="destinazione_Cod"></param>
    ''' <param name="uso_Cod"></param>
    ''' <param name="qualita_Cod"></param>
    ''' <param name="irrigua"></param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="xOrderBy"></param>
    ''' <param name="objParametri"></param>
    ''' <returns>Una dataTable con 3 colonne: UF, UFL, UFC</returns>
    Public Function LeggiUF(ByVal occupazione_Cod As String,
                            ByVal destinazione_Cod As String,
                            ByVal uso_Cod As String,
                            ByVal qualita_Cod As String,
                            ByVal irrigua As Boolean,
                            ByVal xFiltroAggiuntivo As String,
                            ByVal xOrderBy As String,
                            ByRef objParametri As AgronicaCoreParametri,
                            Optional mostraTutto As Boolean = False) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreUmaDAL.UMA_UF_Colture_R.Leggi()"

        Dim dt As DataTable
        Dim strSql As New StringBuilder()

        Try

            strSql.Length = 0

            If mostraTutto Then
                CreaCTEOccupazione(strSql, True)
                CreaCTEDestinazione(strSql, False)
                CreaCTEUso(strSql, False)
                CreaCTEQualita(strSql, False)
            End If

            strSql.AppendLine("SELECT ")
            If mostraTutto Then
                strSql.AppendLine(" UMA_UF_Colture.*, occupazione_CTE.occupazione_Des, destinazione_CTE.destinazione_Des, uso_CTE.uso_Des, qualita_CTE.qualita_Des ")
            Else
                If irrigua Then
                    strSql.AppendLine(" UF_Ha_Irrigua As UF, UFL_Ha_Irrigua As UFL, UFC_Ha_Irrigua As UFC ")
                Else
                    strSql.AppendLine(" UF_Ha As UF, UFL_Ha As UFL, UFC_Ha As UFC ")
                End If
            End If
            strSql.AppendLine("FROM UMA_UF_Colture ")

            If mostraTutto Then
                strSql.AppendLine(" JOIN occupazione_CTE on occupazione_CTE.occupazione_Cod = UMA_UF_Colture.occupazione_Cod")
                strSql.AppendLine(" JOIN destinazione_CTE on destinazione_CTE.destinazione_Cod = UMA_UF_Colture.destinazione_Cod")
                strSql.AppendLine(" JOIN uso_CTE on uso_CTE.uso_Cod = UMA_UF_Colture.uso_Cod")
                strSql.AppendLine(" JOIN qualita_CTE on qualita_CTE.qualita_Cod = UMA_UF_Colture.qualita_Cod")
            End If

            strSql.AppendLine("WHERE 1=1 ")

            If occupazione_Cod <> "" Then
                strSql.AppendLine("AND UMA_UF_Colture.Occupazione_Cod = '" & Agro_SQL_SaveText(occupazione_Cod) + "' ")
            End If
            If destinazione_Cod <> "" Then
                strSql.AppendLine("AND UMA_UF_Colture.Destinazione_Cod = '" & Agro_SQL_SaveText(destinazione_Cod) + "' ")
            End If
            If uso_Cod <> "" Then
                strSql.AppendLine("AND UMA_UF_Colture.Uso_Cod = '" & Agro_SQL_SaveText(uso_Cod) + "' ")
            End If
            If qualita_Cod <> "" Then
                strSql.AppendLine("AND UMA_UF_Colture.Qualita_Cod = '" & Agro_SQL_SaveText(qualita_Cod) + "' ")
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                strSql.Append("AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.Append("AND   UMA_UF_Colture.Inviato >= 0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.Append("AND   UMA_UF_Colture.Inviato = -1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select

            If xOrderBy <> "" Then
                strSql.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            Scrivi_LOG(objParametri, nomeRoutine, ex.Message)
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
        End Try

        Return dt
    End Function

    Public Function leggiOccupazione(ByRef objParametri As AgronicaCoreParametri,
                                     ByVal mostraDescrizioniVuote As Boolean) As DataTable

        Dim stb As New StringBuilder

        Dim nomeRoutine As String = "AgronicaCoreUmaDAL.UMA_UF_Colture_R.leggiOccupazione()"

        Dim dt As DataTable

        Try

            stb.Length = 0

            stb.AppendLine(" Select occupazione_Cod, MAX(occupazione_Des) as occupazione_Des From Codifica_SpecieVegetali_Agea_2015_2020 ")
            stb.AppendLine("GROUP BY occupazione_Cod")
            If Not mostraDescrizioniVuote Then
                stb.AppendLine("HAVING (MAX(occupazione_Des) <> '' OR occupazione_Cod = '000')")
            End If
            stb.AppendLine("order by occupazione_Cod")


            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            Scrivi_LOG(objParametri, nomeRoutine, ex.Message)
            Throw New Exception("[" & nomeRoutine & "] :   " & ex.Message)
        End Try

        Return dt

    End Function

    Public Function leggiDestinazione(ByRef objParametri As AgronicaCoreParametri,
                                      ByVal mostraDescrizioniVuote As Boolean) As DataTable

        Dim stb As New StringBuilder

        Dim nomeRoutine As String = "AgronicaCoreUmaDAL.UMA_UF_Colture_R.leggiDestinazione()"

        Dim dt As DataTable

        Try

            stb.Length = 0

            stb.AppendLine(" Select destinazione_Cod, MAX(destinazione_Des) as destinazione_Des From Codifica_SpecieVegetali_Agea_2015_2020 GROUP BY destinazione_Cod")
            If Not mostraDescrizioniVuote Then
                stb.AppendLine("HAVING (MAX(destinazione_Des) <> '' OR destinazione_cod = '000')")
            End If
            stb.AppendLine("order by destinazione_cod")


            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            Scrivi_LOG(objParametri, nomeRoutine, ex.Message)
            Throw New Exception("[" & nomeRoutine & "] :   " & ex.Message)
        End Try

        Return dt

    End Function

    Public Function leggiUso(ByRef objParametri As AgronicaCoreParametri,
                                     ByVal mostraDescrizioniVuote As Boolean) As DataTable

        Dim stb As New StringBuilder

        Dim nomeRoutine As String = "AgronicaCoreUmaDAL.UMA_UF_Colture_R.leggiUso()"

        Dim dt As DataTable

        Try

            stb.Length = 0

            stb.AppendLine(" Select uso_Cod, MAX(uso_Des) as uso_Des From Codifica_SpecieVegetali_Agea_2015_2020 GROUP BY uso_Cod")
            If Not mostraDescrizioniVuote Then
                stb.AppendLine("HAVING (MAX(uso_Des) <> '' OR uso_Cod = '000')")
            End If
            stb.AppendLine("order by uso_Cod")


            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            Scrivi_LOG(objParametri, nomeRoutine, ex.Message)
            Throw New Exception("[" & nomeRoutine & "] :  " & ex.Message)
        End Try

        Return dt

    End Function

    Public Function leggiQualita(ByRef objParametri As AgronicaCoreParametri,
                                     ByVal mostraDescrizioniVuote As Boolean) As DataTable

        Dim stb As New StringBuilder

        Dim nomeRoutine As String = "AgronicaCoreUmaDAL.UMA_UF_Colture_R.leggiQualita()"

        Dim dt As DataTable

        Try

            stb.Length = 0

            stb.AppendLine(" Select qualita_Cod, MAX(qualita_Des) as qualita_Des From Codifica_SpecieVegetali_Agea_2015_2020 GROUP BY qualita_Cod")
            If Not mostraDescrizioniVuote Then
                stb.AppendLine("HAVING (MAX(qualita_Des) <> '' OR qualita_Cod = '000')")
            End If
            stb.AppendLine("order by qualita_Cod")


            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            Scrivi_LOG(objParametri, nomeRoutine, ex.Message)
            Throw New Exception("[" & nomeRoutine & "] :  " & ex.Message)
        End Try

        Return dt

    End Function

    Public Sub ComponiFiltroAggiuntivoValidita(ByRef filtroAggiuntivo As String, inizioValidita As String, fineValidita As String)
        filtroAggiuntivo = String.Concat(filtroAggiuntivo, " ( (UMA_UF_Colture.Validita_Inizio >= '" & inizioValidita & "' AND UMA_UF_Colture.Validita_Fine <= '" & fineValidita & "') OR ")

        filtroAggiuntivo = String.Concat(filtroAggiuntivo, " (UMA_UF_Colture.Validita_Fine >= '" & inizioValidita & "' AND UMA_UF_Colture.Validita_Fine <= '" & fineValidita & "') OR ")

        filtroAggiuntivo = String.Concat(filtroAggiuntivo, " (UMA_UF_Colture.Validita_Inizio >= '" & inizioValidita & "' AND UMA_UF_Colture.Validita_Inizio <= '" & fineValidita & "') OR ")

        filtroAggiuntivo = String.Concat(filtroAggiuntivo, " (UMA_UF_Colture.Validita_Inizio <= '" & inizioValidita & "' AND UMA_UF_Colture.Validita_Fine >= '" & fineValidita & "') ) ")

    End Sub

    Public Sub CreaCTEOccupazione(stb As StringBuilder, primaCte As Boolean)

        If primaCte Then
            stb.Append(" WITH ")
        Else
            stb.Append(" , ")
        End If

        stb.AppendLine(" occupazione_CTE as ( ")
        stb.AppendLine(" Select occupazione_Cod, MAX(occupazione_Des) as occupazione_Des From Codifica_SpecieVegetali_Agea_2015_2020 GROUP BY occupazione_Cod")
        stb.AppendLine(") ")

    End Sub

    Public Sub CreaCTEDestinazione(stb As StringBuilder, primaCte As Boolean)

        If primaCte Then
            stb.Append(" WITH ")
        Else
            stb.Append(" , ")
        End If

        stb.AppendLine(" destinazione_CTE as ( ")
        stb.AppendLine(" Select destinazione_Cod, MAX(destinazione_Des) as destinazione_Des From Codifica_SpecieVegetali_Agea_2015_2020 GROUP BY destinazione_Cod")
        stb.AppendLine(") ")

    End Sub

    Public Sub CreaCTEUso(stb As StringBuilder, primaCte As Boolean)

        If primaCte Then
            stb.Append(" WITH ")
        Else
            stb.Append(" , ")
        End If

        stb.AppendLine(" uso_CTE as ( ")
        stb.AppendLine(" Select uso_Cod, MAX(uso_Des) as uso_Des From Codifica_SpecieVegetali_Agea_2015_2020 GROUP BY uso_Cod")
        stb.AppendLine(") ")

    End Sub

    Public Sub CreaCTEQualita(stb As StringBuilder, primaCte As Boolean)

        If primaCte Then
            stb.Append(" WITH ")
        Else
            stb.Append(" , ")
        End If

        stb.AppendLine(" qualita_CTE as ( ")
        stb.AppendLine(" Select qualita_Cod, MAX(qualita_Des) as qualita_Des From Codifica_SpecieVegetali_Agea_2015_2020 GROUP BY qualita_Cod")
        stb.AppendLine(") ")

    End Sub

End Class

Public Class UMA_UF_Colture_W
    Inherits DataProvider

    Public Function AggiungiNuovi(listLav As List(Of UMA_UF_Colture_Dto),
                                  ByRef objParametri As AgronicaCoreParametri,
                                  ByRef strErr As String) As String
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.UMASetup.AggiungiNouvi()"
        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False


        Try
            For Each l In listLav

                If CheckDuplicati(l, objParametri, False) Then

                    l.Username_Modifica = objParametri.UtenteUsername
                    l.Data_Modifica = Date.Now
                    l.Username_Creazione = objParametri.UtenteUsername
                    l.Data_Creazione = Date.Now

                    StrSQL.Append(" INSERT INTO [dbo].[UMA_UF_Colture] ")
                    StrSQL.Append("          ([Occupazione_Cod]     ")
                    StrSQL.Append("          ,[Destinazione_Cod]    ")
                    StrSQL.Append("          ,[Uso_Cod]             ")
                    StrSQL.Append("          ,[Qualita_Cod]         ")
                    StrSQL.Append("          ,[Validita_Inizio]     ")
                    StrSQL.Append("          ,[UF_Ha]               ")
                    StrSQL.Append("          ,[UFL_Ha]              ")
                    StrSQL.Append("          ,[UFC_Ha]              ")
                    StrSQL.Append("          ,[UF_Ha_Irrigua]       ")
                    StrSQL.Append("          ,[UFL_Ha_Irrigua]      ")
                    StrSQL.Append("          ,[UFC_Ha_Irrigua]      ")
                    StrSQL.Append("          ,[inviato]             ")
                    StrSQL.Append("          ,[datainvio]           ")
                    StrSQL.Append("          ,[Data_Creazione]      ")
                    StrSQL.Append("          ,[Data_Modifica]       ")
                    StrSQL.Append("          ,[Username_Creazione]  ")
                    StrSQL.Append("          ,[Username_Modifica]   ")
                    StrSQL.Append("          ,[Validita_Fine])      ")
                    StrSQL.Append("    VALUES                                              ")
                    StrSQL.Append($"         (  '{Agro_SQL_SaveText(l.Occupazione_Cod)}' ")
                    StrSQL.Append($"          , '{Agro_SQL_SaveText(l.Destinazione_Cod)}' ")
                    StrSQL.Append($"          , '{Agro_SQL_SaveText(l.Uso_Cod)}' ")
                    StrSQL.Append($"          , '{Agro_SQL_SaveText(l.Qualita_Cod)}'     ")
                    StrSQL.Append($"          , {Agro_SQL_SaveDate(l.Validita_Inizio)}     ")
                    StrSQL.Append($"          , {Agro_SQL_SaveNum(l.UF_Ha)} ")
                    StrSQL.Append($"          , {Agro_SQL_SaveNum(l.UFL_Ha)} ")
                    StrSQL.Append($"          , {Agro_SQL_SaveNum(l.UFC_Ha)} ")
                    StrSQL.Append($"          , {Agro_SQL_SaveNum(l.UF_Ha_Irrigua)} ")
                    StrSQL.Append($"          , {Agro_SQL_SaveNum(l.UFL_Ha_Irrigua)} ")
                    StrSQL.Append($"          , {Agro_SQL_SaveNum(l.UFC_Ha_Irrigua)} ")
                    StrSQL.Append($"          , {Agro_SQL_SaveNum(l.Inviato)}")
                    If IsNothing(l.DataInvio) Then
                        StrSQL.Append("          , NULL ")
                    Else
                        StrSQL.Append($"          , {Agro_SQL_SaveDate(l.DataInvio)}")
                    End If
                    StrSQL.Append($"          , {Agro_SQL_SaveDate(l.Data_Creazione)}")
                    StrSQL.Append($"          , {Agro_SQL_SaveDate(l.Data_Modifica)}")
                    StrSQL.Append($"          ,'{Agro_SQL_SaveText(l.Username_Creazione)}'")
                    StrSQL.Append($"          ,'{Agro_SQL_SaveText(l.Username_Modifica)}'")
                    StrSQL.Append($"          , {Agro_SQL_SaveDate(l.Validita_Fine)}) ")
                    xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
                Else
                    strErr = String.Concat(strErr, vbCrLf & " Riga non inserita: Occupazione: " & l.Occupazione_Des & ", Destinazione: " & l.Destinazione_Des &
                                           ", Qualita: " & l.Qualita_Des & ", Uso: " & l.Uso_Des & ", Validita Inizio: " & CStr(l.Validita_Inizio) & ". Rilevata sovrapposizione di periodi di validita.")
                End If
            Next
        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "]  " & MessaggioErrore)
        End Try

        Return strErr
    End Function

    Public Function Rimuovi(lista As List(Of UMA_UF_Colture_Dto), objParametri As AgronicaCoreParametri) As Boolean

        ' ------------- Variabili -------------
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.UMASetup_W.Rimuovi()"
        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            For Each l In lista

                StrSQL.Append(" Delete from  [UMA_UF_Colture] ")

                StrSQL.Append($" where [Qualita_Cod] = '{Agro_SQL_SaveText(l.Qualita_Cod)}' ")
                StrSQL.Append($" AND [Uso_Cod] = '{Agro_SQL_SaveText(l.Uso_Cod)}' ")
                StrSQL.Append($" AND [Destinazione_Cod] = '{Agro_SQL_SaveText(l.Destinazione_Cod)}' ")
                StrSQL.Append($" AND [Occupazione_Cod] = '{Agro_SQL_SaveText(l.Occupazione_Cod)}' ")
                StrSQL.Append($" AND [Validita_Inizio] = {Agro_SQL_SaveDate(l.Validita_Inizio)} ")

                xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)

            Next
        Catch ex As Exception


            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)

            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return True
    End Function

    Public Function Aggiorna(righeModificateArr As List(Of UMA_UF_Colture_Dto),
                             ByRef objParametri As AgronicaCoreParametri,
                             ByRef StrErr As String) As String

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.UMA_UF_Colture_W.Aggiorna()"
        Dim MessaggioErrore As String = ""
        Dim StrSQL As StringBuilder = New System.Text.StringBuilder
        Dim xRisp As Boolean = False
        Try

            For Each l In righeModificateArr

                If CheckDuplicati(l, objParametri, True) Then

                    l.Username_Modifica = objParametri.UtenteUsername
                    l.Data_Modifica = Date.Now

                    StrSQL.AppendLine(" Update [UMA_UF_Colture] set ")

                    StrSQL.AppendLine($"           [UF_Ha]                     =     {Agro_SQL_SaveNum(l.UF_Ha)} ")
                    StrSQL.AppendLine($"          ,[UFL_Ha]                    =     {Agro_SQL_SaveNum(l.UFL_Ha)} ")
                    StrSQL.AppendLine($"          ,[UFC_Ha]                    =     {Agro_SQL_SaveNum(l.UFC_Ha)}     ")
                    StrSQL.AppendLine($"          ,[UF_Ha_Irrigua]             =     {Agro_SQL_SaveNum(l.UF_Ha_Irrigua)}     ")
                    StrSQL.AppendLine($"          ,[UFL_Ha_Irrigua]            =     {Agro_SQL_SaveNum(l.UFL_Ha_Irrigua)} ")
                    StrSQL.AppendLine($"          ,[UFC_Ha_Irrigua]            =     {Agro_SQL_SaveNum(l.UFC_Ha_Irrigua)}")
                    StrSQL.AppendLine($"          ,[inviato]                   =     {Agro_SQL_SaveNum(l.Inviato)}")
                    StrSQL.AppendLine("          ,[datainvio]                 =     ")
                    If IsNothing(l.DataInvio) Then
                        StrSQL.AppendLine("NULL")
                    Else
                        StrSQL.AppendLine(" " & Agro_SQL_SaveDate(l.DataInvio) & " ")
                    End If
                    StrSQL.AppendLine($"          ,[Data_Modifica]             =     {Agro_SQL_SaveDate(l.Data_Modifica)}")
                    StrSQL.AppendLine($"          ,[Username_Modifica]         =    '{Agro_SQL_SaveText(l.Username_Modifica)}'")
                    StrSQL.AppendLine($"          ,[Validita_Fine]             =     {Agro_SQL_SaveDate(l.Validita_Fine)}")

                    StrSQL.AppendLine($" where [Qualita_Cod] = '{Agro_SQL_SaveText(l.Qualita_Cod)}' ")
                    StrSQL.AppendLine($" AND [Uso_Cod] = '{Agro_SQL_SaveText(l.Uso_Cod)}' ")
                    StrSQL.AppendLine($" AND [Destinazione_Cod] = '{Agro_SQL_SaveText(l.Destinazione_Cod)}' ")
                    StrSQL.AppendLine($" AND [Occupazione_Cod] = '{Agro_SQL_SaveText(l.Occupazione_Cod)}' ")
                    StrSQL.AppendLine($" AND [Validita_Inizio] = {Agro_SQL_SaveDate(l.Validita_Inizio)} ")

                    xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)

                Else
                    StrErr = String.Concat(StrErr, vbCrLf & " Riga non aggiornata: Occupazione: " & l.Occupazione_Des & ", Destinazione: " & l.Destinazione_Des &
                                           ", Qualita: " & l.Qualita_Des & ", Uso: " & l.Uso_Des & ", Validita Inizio: " & CStr(l.Validita_Inizio) & ". Rilevata sovrapposizione di periodi di validita.")
                End If

            Next
        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return StrErr
    End Function

    Private Function CheckDuplicati(riga As UMA_UF_Colture_Dto, objParametri As AgronicaCoreParametri, aggiornamento As Boolean) As Boolean

        Dim leggi = New UMA_UF_Colture_R
        Dim filtroAggiorna = ""

        If aggiornamento Then
            filtroAggiorna = " AND UMA_UF_Colture.Validita_Inizio <> '" & riga.Validita_Inizio.ToShortDateString & "' "
        End If

        Dim dt = leggi.LeggiUF(riga.Occupazione_Cod, riga.Destinazione_Cod, riga.Uso_Cod, riga.Qualita_Cod,
                                 False, " UMA_UF_Colture.Validita_Fine >= '" & riga.Validita_Inizio.ToShortDateString & "' AND UMA_UF_Colture.Validita_Inizio <= '" & riga.Validita_Fine.ToShortDateString & "' " & filtroAggiorna, "", objParametri, True)

        Return dt.Rows.Count = 0
    End Function

End Class

Public Class UMA_UF_Colture_Dto
    ' *************************** Colonne chiavi primarie ***************************
    Public Occupazione_Cod As String
    Public Destinazione_Cod As String
    Public Uso_Cod As String
    Public Qualita_Cod As String
    Public Validita_Inizio As DateTime

    ' ************************* Colonne della tabella UMA_Setup *************************
    Public UF_Ha As Double?
    Public UFL_Ha As Double?
    Public UFC_Ha As Double?
    Public UF_Ha_Irrigua As Double?
    Public UFL_Ha_Irrigua As Double?
    Public UFC_Ha_Irrigua As Double?
    Public Occupazione_Des As String
    Public Destinazione_Des As String
    Public Uso_Des As String
    Public Qualita_Des As String
    Public Validita_Fine As Date
    Public Inviato As Short
    Public DataInvio As Date?
    Public Data_Creazione As Date?
    Public Data_Modifica As Date?
    Public Username_Creazione As String
    Public Username_Modifica As String

End Class