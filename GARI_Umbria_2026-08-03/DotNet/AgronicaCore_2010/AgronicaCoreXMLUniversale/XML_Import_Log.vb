Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports System.Text

Public Class XML_Import_Log_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function Leggi(ByVal piva As String,
                          ByVal nomeFile As String,
                          ByVal stato As enum_WFlow_Import_XML_Universale,
                          ByVal lavCod As Integer,
                          ByVal tipoXml As String,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                          ) As DataTable

        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreXMLUniversale.XML_Import_Log_R.Leggi()"

        '----- Variabili
        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        Try

            stb.Length = 0

            stb.AppendLine(" SELECT * ")
            stb.AppendLine(" FROM XML_Import_Log ")
            stb.AppendLine(" WHERE 1 = 1 ")
            
            If piva <> "" Then
                stb.AppendLine(" AND Piva = '" & Agro_SQL_SaveText(piva) & "' ")
            End If

            If nomeFile <> "" Then
                stb.AppendLine(" AND NomeFile = '" & Agro_SQL_SaveText(nomeFile) & "' ")
            End If

            If stato <> 0 Then
                stb.AppendLine(" AND Stato = " & Agro_SQL_SaveNum(stato) & " ")
            End If

            If lavCod <> 0 Then
                stb.AppendLine(" AND Lav_Cod = " & Agro_SQL_SaveNum(lavCod) & " ")
            End If

            If tipoXml <> "" Then
                stb.AppendLine(" AND Tipo_Xml = '" & Agro_SQL_SaveText(tipoXml) & "' ")
            End If


            If xFiltroAggiuntivo <> "" Then
                stb.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    stb.AppendLine(" AND   Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    stb.AppendLine(" AND   Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                stb.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stb.ToString, nomeRoutine)
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


'#################################################################
'#################################################################
'#################################################################

Public Class XML_Import_Log_W
    Inherits AgronicaCoreDataProvider.DataProvider



    '##############################################################################################
    Public Function Scrivi(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                           ByVal piva As String,
                           ByVal nomeFile As String,
                           ByVal idAgenda As Integer,
                           ByVal lavCod As Integer,
                           ByVal tipoXml As String,
                           ByVal stato As enum_WFlow_Import_XML_Universale,
                           ByVal messaggio As String,
                           ByVal dataOperazione As Date,
                           Optional ByVal Data_creazione As Date = #2/1/1900#,
                           Optional ByVal Data_modifica As Date = #2/1/1900#,
                           Optional ByVal username_creazione As String = "",
                           Optional ByVal username_modifica As String = ""
                           ) As Boolean


        Dim nomeRoutine As String = "AgronicaCoreXMLUniversale.XML_Import_Log_W.Scrivi()"

        '====================================================================================
        'Parametri opzionali :

        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim xRisp As Boolean = False

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
            stb.Length = 0
            stb.AppendLine(" INSERT INTO XML_Import_Log ")

            stb.AppendLine("              (")

            stb.AppendLine("              Piva,     NomeFile,       Stato,      Messaggio,      Id_Agenda,      Data_Operazione, ")
            stb.AppendLine("              Lav_Cod,     Tipo_Xml, ")

            stb.AppendLine("              Inviato,            datainvio, ")
            stb.AppendLine("              Data_Creazione,     Data_Modifica, ")
            stb.AppendLine("              UserName_Creazione, UserName_Modifica, ")
            stb.AppendLine("              Validita_Inizio,    Validita_Fine ")
            stb.AppendLine("              ) ")

            stb.AppendLine(" VALUES ( ")


            stb.AppendLine("		    '" & Agro_SQL_SaveText(piva) & "'  ")
            stb.AppendLine("			,'" & Agro_SQL_SaveText(nomeFile) & "' ")
            stb.AppendLine("			, " & Agro_SQL_SaveNum(stato) & "  ")
            stb.AppendLine("			,'" & Agro_SQL_SaveText(messaggio) & "' ")
            stb.AppendLine("			, " & Agro_SQL_SaveNum_NULL(idAgenda) & "  ")
            stb.AppendLine("			, " & Agro_SQL_SaveDateTime(dataOperazione) & "  ")
            stb.AppendLine("			, " & Agro_SQL_SaveNum(lavCod) & "  ")
            stb.AppendLine("			,'" & Agro_SQL_SaveText(tipoXml) & "' ")

            stb.AppendLine("         , 0  ")
            stb.AppendLine("         , Null  ")

            stb.AppendLine("			, " & Agro_SQL_SaveDateTime(Data_creazione) & "  ")
            stb.AppendLine("			, " & Agro_SQL_SaveDateTime(Data_modifica) & "  ")
            stb.AppendLine("			,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            stb.AppendLine("			,'" & Agro_SQL_SaveText(username_modifica) & "' ")
            stb.AppendLine("			, " & Agro_SQL_SaveDate(AGRODATAINIZIO) & "  ")
            stb.AppendLine("			, " & Agro_SQL_SaveDate(AGRODATAFINE) & "  ")


            stb.AppendLine(") ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return xRisp

    End Function




    '##############################################################################################
    Public Function ModificaMessaggio(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                           ByVal piva As String,
                           ByVal idAgenda As Integer,
                           ByVal messaggio As String,
                           Optional ByVal Data_modifica As Date = #2/1/1900#,
                           Optional ByVal username_modifica As String = ""
                           ) As Boolean


        Dim nomeRoutine As String = "AgronicaCoreXMLUniversale.XML_Import_Log_W.ModificaMessaggio()"

        '====================================================================================
        'Parametri opzionali :

        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim xRisp As Boolean = False

        Try


            If Data_modifica = #2/1/1900# Then
                Data_modifica = Date.Now
            End If

            If username_modifica = "" Then
                username_modifica = objParametri.UsernameOperazione
            End If

            '---------------------------------------------
            stb.Length = 0
            stb.AppendLine(" Update XML_Import_Log ")

            stb.AppendLine(" Set")

            stb.AppendLine("  Messaggio = '" & Agro_SQL_SaveText(messaggio) & "'  ")
            stb.AppendLine(" ,Data_Modifica = " & Agro_SQL_SaveDateTime(Data_modifica) & "  ")
            stb.AppendLine(" ,Username_Modifica	= '" & Agro_SQL_SaveText(username_modifica) & "' ")

            stb.AppendLine(" Where Piva = '" & Agro_SQL_SaveText(piva) & "' ")
            stb.AppendLine(" AND id_agenda = " & Agro_SQL_SaveNum(idAgenda) & " ")


            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return xRisp

    End Function


    '##############################################################################################
    Public Function Cancella(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                           ByVal piva As String,
                           ByVal nomeFile As String,
                           ByVal idAgenda As Integer,
                           ByVal lavCod As Integer,
                           ByVal tipoXml As String,
                           ByVal xFiltroAggiuntivo As String
                           ) As Boolean


        Dim nomeRoutine As String = "AgronicaCoreXMLUniversale.XML_Import_Log_W.Cancella()"

        '====================================================================================
        'Parametri opzionali :

        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim xRisp As Boolean = False

        Try


            '---------------------------------------------
            stb.Length = 0
            stb.AppendLine(" DELETE From XML_Import_Log ")

            stb.AppendLine(" WHERE 1 = 1 ")

            If piva <> "" Then
                stb.AppendLine(" AND Piva = '" & Agro_SQL_SaveText(piva) & "' ")
            End If

            If nomeFile <> "" Then
                stb.AppendLine(" AND NomeFile = '" & Agro_SQL_SaveText(nomeFile) & "' ")
            End If

            If lavCod <> 0 Then
                stb.AppendLine(" AND Lav_Cod = " & Agro_SQL_SaveNum(lavCod) & " ")
            End If

            If idAgenda <> 0 Then
                stb.AppendLine(" AND id_agenda = " & Agro_SQL_SaveNum(idAgenda) & " ")
            End If

            If tipoXml <> "" Then
                stb.AppendLine(" AND Tipo_Xml = '" & Agro_SQL_SaveText(tipoXml) & "' ")
            End If

            If xFiltroAggiuntivo <> "" Then
                stb.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, stb.ToString, nomeRoutine)
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
