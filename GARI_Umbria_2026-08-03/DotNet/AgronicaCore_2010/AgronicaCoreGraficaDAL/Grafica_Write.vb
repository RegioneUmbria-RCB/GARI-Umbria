Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider


Public Class Grafica_Write
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function Scrivi2005( _
                                ByVal PIVA As String, _
                                ByVal Sa_Cod As Long, _
                                ByVal Section As String, _
                                ByVal Id As String, _
                                ByVal Descr As String, _
                                ByVal EColor As Long, _
                                ByVal Layer As String, _
                                ByVal ELine As Long, _
                                ByVal VX1 As Decimal, _
                                ByVal VY1 As Decimal, _
                                ByVal VX2 As Decimal, _
                                ByVal VY2 As Decimal, _
                                ByVal Rad As Decimal, _
                                ByVal Text As String, _
                                ByVal Gps As Long, _
                                ByVal Lat As Decimal, _
                                ByVal Lon As Decimal, _
                                ByVal Pdop As Decimal, _
                                ByVal Fuso As Integer, ByVal Proiezione As String, ByVal Delta_Nord As Decimal, ByVal Delta_Est As Decimal, ByVal Quota As Decimal, _
                                ByVal ConvertiInEsadecimale As Boolean, _
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                        ) As Long

        Dim NomeRoutine As String = "AgronicaCoreGraficaDAL.Grafica_Write.Scrivi2005()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append("INSERT INTO Grafica(Piva,Sa_Cod,[Section],Id,Descr,EColor,Layer,ELine,VX1,VY1,VX2,VY2,Rad,[Text],Gps,Lat,Lon,Pdop,Fuso, Proiezione, Delta_Nord, Delta_Est, Quota, Inviato,DataInvio,Data_Creazione,Data_Modifica,UserName_Creazione,UserName_Modifica,Validita_Inizio,Validita_Fine) ")
            StrSQL.Append("VALUES (")
            StrSQL.Append("          '" & Agro_SQL_SaveText(Trim(PIVA)) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            StrSQL.Append("         , '" & Agro_SQL_SaveText(Section) & "'")
            StrSQL.Append("         , '" & Agro_SQL_SaveText(Id) & "'")
            StrSQL.Append("         , '" & Agro_SQL_SaveText(Descr) & "'")
            StrSQL.Append("         , " & IIf(EColor = 0, "NULL", EColor))
            StrSQL.Append("         , " & IIf(Layer = "", "NULL", "'" & Agro_SQL_SaveText(Layer) & "'"))
            StrSQL.Append("         , " & IIf(ELine = 0, "NULL", ELine))
            StrSQL.Append("         , " & IIf(VX1 = 0, "NULL", Agro_SQL_SaveNum(VX1)))
            StrSQL.Append("         , " & IIf(VY1 = 0, "NULL", Agro_SQL_SaveNum(VY1)))
            StrSQL.Append("         , " & IIf(VX2 = 0, "NULL", Agro_SQL_SaveNum(VX2)))
            StrSQL.Append("         , " & IIf(VY2 = 0, "NULL", Agro_SQL_SaveNum(VY2)))
            StrSQL.Append("         , " & IIf(Rad = 0, "NULL", Agro_SQL_SaveNum(Rad)))
            StrSQL.Append("         , " & IIf(Text = "", "NULL", "'" & Agro_SQL_SaveText(Text) & "'"))
            StrSQL.Append("         , " & Gps)
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Lat) & " , " & Agro_SQL_SaveNum(Lon) & " , " & Agro_SQL_SaveNum(Pdop) & " , " & Agro_SQL_SaveNum(Fuso) & " , '" & Agro_SQL_SaveText(Proiezione) & "' , " & Agro_SQL_SaveNum(Delta_Nord) & " , " & Agro_SQL_SaveNum(Delta_Est) & " , " & Agro_SQL_SaveNum(Quota))
            StrSQL.Append("         , 0, Null, " & Agro_SQL_SaveDate(Now.Date) & " , " & Agro_SQL_SaveDate(Now.Date) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & "  ")
            StrSQL.Append(")")

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




    '##############################################################################################
    Public Function Cancella( _
                            ByVal PIVA As String, _
                            ByVal Sa_Cod As Long, _
                            ByVal Section As String, _
                            ByVal Id As String, _
                            ByVal Descr As String, _
                               ByVal xFiltroAggiuntivo As String, _
                               ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                               ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Centri_Codici_Write.Cancella()"

        '====================================================================================
        'Parametri opzionali :
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False
        Dim DescrReale As String = ""

        If Descr <> "" Then
            If Left(UCase(Descr), 6) <> "VERTEX" Then
                DescrReale = Left(Descr, IIf(InStr(1, Descr, " ") = 0, Len(Descr), InStr(1, Descr, " ")) - 1)
            Else
                DescrReale = UCase(Descr)
            End If

        End If
        Try


            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = AgronicaCoreDataProvider.AgronicaCoreParametri.enumCancellazioneLogica.CancellazioneLogica Then

                StrSQL.Length = 0
                StrSQL.Append(" UPDATE Grafica ")
                StrSQL.Append(" SET ")
                StrSQL.Append("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("      ,Inviato = -1 ")
                StrSQL.Append(" WHERE  Piva = '" & Agro_SQL_SaveText(PIVA) & "' ")
                StrSQL.Append(" AND Inviato >= 0")



            Else

                StrSQL.Length = 0
                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM     Grafica ")
                StrSQL.Append(" WHERE    Piva= '" & Agro_SQL_SaveText(PIVA) & "' ")
                StrSQL.Append(" AND      Inviato = 0")

            End If

            '---------------------------------------------

            If Sa_Cod <> 0 Then
                StrSQL.Append(" AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            End If

            If Section <> "" Then
                StrSQL.Append(" AND [Section] = '" & Agro_SQL_SaveText(Section) & "' ")
            End If

            If Id <> "" Then
                StrSQL.Append(" AND id = '" & Agro_SQL_SaveText(Id) & "' ")
            End If

            If DescrReale <> "" Then
                StrSQL.Append(" AND Descr Like '" & DescrReale & "%' ")

            End If


            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

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



    '##############################################################################################
    Public Function AggiornaValiditaInizio(
                                ByVal PIVA As String,
                                ByVal Sa_Cod As Long,
                                ByVal Codice As String,
                                ByVal Validita_Inizio As Date,
                                        ByVal xFiltroAggiuntivo As String,
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                        ) As Boolean

        Dim NomeRoutine As String = "AnagrafeCoreGraficaDAL.Grafica_W.AggiornaValiditaInizio()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append("UPDATE Grafica SET ")
            StrSQL.Append("   UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.Append("   ,Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Inizio))
            StrSQL.Append(" WHERE Piva = '" & Agro_SQL_SaveText(Trim(PIVA)) & "' ")
            StrSQL.Append(" AND   Validita_Inizio < " & Agro_SQL_SaveDate(Validita_Inizio))

            If Sa_Cod <> 0 Then
                StrSQL.Append(" AND   Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            End If

            If Codice <> "" Then
                StrSQL.Append(" AND   Id = '" & Agro_SQL_SaveText(Codice) & "' ")
            End If

            '---------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
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



    '##############################################################################################
    Public Function AggiornaValiditaFine(
                                ByVal PIVA As String,
                                ByVal Sa_Cod As Long,
                                ByVal Codice As String,
                                ByVal Validita_Fine As Date,
                                        ByVal xFiltroAggiuntivo As String,
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                        ) As Boolean

        Dim NomeRoutine As String = "AnagrafeCoreGraficaDAL.Grafica_W.AggiornaValiditaFine()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append("UPDATE Grafica SET ")
            StrSQL.Append("   UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.Append("   ,Validita_Fine   =  " & Agro_SQL_SaveDate(Validita_Fine))
            StrSQL.Append(" WHERE Piva = '" & Agro_SQL_SaveText(Trim(PIVA)) & "' ")
            StrSQL.Append(" AND   Validita_Fine > " & Agro_SQL_SaveDate(Validita_Fine))

            If Sa_Cod <> 0 Then
                StrSQL.Append(" AND   Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            End If

            If Codice <> "" Then
                StrSQL.Append(" AND   Id = '" & Agro_SQL_SaveText(Codice) & "' ")
            End If

            '---------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
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


    '##############################################################################################
    Public Function AggiornaValiditaImpianto(
                                ByVal PIVA As String,
                                ByVal Sa_Cod As Long,
                                ByVal Codice As String,
                                ByVal CampoText_daModificare As String,
                                ByVal Validita_Inizio As Date,
                                ByVal Validita_Fine As Date,
                                        ByVal xFiltroAggiuntivo As String,
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                        ) As Boolean

        Dim NomeRoutine As String = "AnagrafeCoreGraficaDAL.Grafica_Write.AggiornaValiditaImpianto()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append("UPDATE Grafica SET ")
            StrSQL.Append("   UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.Append("   ,Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Inizio))
            StrSQL.Append("   ,Validita_Fine   =  " & Agro_SQL_SaveDate(Validita_Fine))

            StrSQL.Append(" WHERE Piva = '" & Agro_SQL_SaveText(Trim(PIVA)) & "' ")
            StrSQL.Append(" AND   Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            StrSQL.Append(" AND   Id = '" & Agro_SQL_SaveText(Codice) & "' ")

            '---------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
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


    '##############################################################################################
    Public Function Modifica2005(
                                ByVal PIVA As String,
                                ByVal Sa_Cod As Long,
                                ByVal Section As String,
                                ByVal Id As String,
                                ByVal Descr As String,
                                ByVal EColor As Long,
                                ByVal Layer As String,
                                ByVal ELine As Long,
                                ByVal VX1 As Decimal,
                                ByVal VY1 As Decimal,
                                ByVal VX2 As Decimal,
                                ByVal VY2 As Decimal,
                                ByVal Rad As Decimal,
                                ByVal Text As String,
                                ByVal Gps As Long,
                                ByVal Lat As Decimal,
                                ByVal Lon As Decimal,
                                ByVal Pdop As Decimal,
                                ByVal Fuso As Integer,
                                ByVal Proiezione As String,
                                ByVal Delta_Nord As Decimal,
                                ByVal Delta_Est As Decimal,
                                ByVal Quota As Decimal,
                                        ByVal xFiltroAggiuntivo As String,
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                        ) As Boolean

        Dim NomeRoutine As String = "AnagrafeCoreGraficaDAL.Grafica_W.Modifica2005()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False
        Dim DescrReale As String
        Try
            '---------------------------------------------
            StrSQL.Length = 0
            If Left(UCase(Descr), 6) <> "VERTEX" Then
                DescrReale = Left(Descr, IIf(InStr(1, Descr, " ") = 0, Len(Descr), InStr(1, Descr, " ")) - 1)
                'Query per la modifica dei dati                 ' #### CLASSE ####
                StrSQL.Append("UPDATE Grafica SET ")
                StrSQL.Append("        Descr             = '" & Agro_SQL_SaveText(Descr) & "'")
                StrSQL.Append("       ,EColor            =  " & IIf(EColor = 0, "NULL", Agro_SQL_SaveNum(EColor)))
                StrSQL.Append("       ,Layer             =  " & IIf(Layer = "", "NULL", "'" & Agro_SQL_SaveText(Layer) & "'"))
                StrSQL.Append("       ,ELine             =  " & IIf(ELine = 0, "NULL", "" & Agro_SQL_SaveNum(ELine) & ""))
                StrSQL.Append("       ,Vx1               =  " & IIf(VX1 = 0, "NULL", Agro_SQL_SaveNum(VX1)))
                StrSQL.Append("       ,Vy1               =  " & IIf(VY1 = 0, "NULL", Agro_SQL_SaveNum(VY1)))
                StrSQL.Append("       ,Vx2               =  " & IIf(VX2 = 0, "NULL", Agro_SQL_SaveNum(VX2)))
                StrSQL.Append("       ,Vy2               =  " & IIf(VY2 = 0, "NULL", Agro_SQL_SaveNum(VY2)))
                StrSQL.Append("       ,Rad               =  " & IIf(Rad = 0, "NULL", Agro_SQL_SaveNum(Rad)))
                StrSQL.Append("       ,[Text]            =  " & IIf(Text = "", "NULL", "'" & Agro_SQL_SaveText(Text) & "'"))
                StrSQL.Append("       ,Gps               =  " & Gps)
                StrSQL.Append("       ,Lat               =  " & Agro_SQL_SaveNum(Lat))
                StrSQL.Append("       ,Lon               =  " & Agro_SQL_SaveNum(Lon))
                StrSQL.Append("       ,Pdop              =  " & Agro_SQL_SaveNum(Pdop) & " , Fuso = " & Agro_SQL_SaveNum(Fuso) & " , Proiezione = '" & Agro_SQL_SaveText(Proiezione) & "' , Delta_Nord = " & Agro_SQL_SaveNum(Delta_Nord) & " , Delta_Est = " & Agro_SQL_SaveNum(Delta_Est) & " , Quota = " & Agro_SQL_SaveNum(Quota))
                StrSQL.Append("       ,Inviato           =  0 ")
                StrSQL.Append("       ,Data_Modifica     =  " & Agro_SQL_SaveDate(Now.Date) & " ")
                StrSQL.Append("       ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
                StrSQL.Append("       ,Validita_Inizio   =  " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio))
                StrSQL.Append("       ,Validita_Fine     =  " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine))
                StrSQL.Append(" WHERE    Piva = '" & Agro_SQL_SaveText(PIVA) & "' ")
                StrSQL.Append(" AND      Sa_Cod = " & Sa_Cod & "  ")
                StrSQL.Append(" AND      [Section] = '" & Agro_SQL_SaveText(Section) & "'")
                StrSQL.Append(" AND      Id = '" & Agro_SQL_SaveText(Id) & "'")
                StrSQL.Append(" AND      Descr LIKE '" & Agro_SQL_SaveText(DescrReale) & "%'")

            Else

                DescrReale = UCase(Descr)

                'Query per la modifica dei dati                 ' #### CLASSE ####
                StrSQL.Append("UPDATE Grafica SET ")
                StrSQL.Append("       EColor            =  " & IIf(EColor = 0, "NULL", Agro_SQL_SaveNum(EColor)))
                StrSQL.Append("       ,Layer             =  " & IIf(Layer = "", "NULL", "'" & Agro_SQL_SaveText(Layer) & "'"))
                StrSQL.Append("       ,ELine             =  " & IIf(ELine = 0, "NULL", "" & Agro_SQL_SaveNum(ELine) & ""))
                StrSQL.Append("       ,Vx1               =  " & IIf(VX1 = 0, "NULL", Agro_SQL_SaveNum(VX1)))
                StrSQL.Append("       ,Vy1               =  " & IIf(VY1 = 0, "NULL", Agro_SQL_SaveNum(VY1)))
                StrSQL.Append("       ,Vx2               =  " & IIf(VX2 = 0, "NULL", Agro_SQL_SaveNum(VX2)))
                StrSQL.Append("       ,Vy2               =  " & IIf(VY2 = 0, "NULL", Agro_SQL_SaveNum(VY2)))
                StrSQL.Append("       ,Rad               =  " & IIf(Rad = 0, "NULL", Agro_SQL_SaveNum(Rad)))
                StrSQL.Append("       ,[Text]            =  " & IIf(Text = "", "NULL", "'" & Agro_SQL_SaveText(Text) & "'"))
                StrSQL.Append("       ,Gps               =  " & Gps)
                StrSQL.Append("       ,Lat               =  " & Agro_SQL_SaveNum(Lat))
                StrSQL.Append("       ,Lon               =  " & Agro_SQL_SaveNum(Lon))
                StrSQL.Append("       ,Pdop              =  " & Agro_SQL_SaveNum(Pdop) & " , Fuso = " & Agro_SQL_SaveNum(Fuso) & " , Proiezione = '" & Agro_SQL_SaveText(Proiezione) & "' , Delta_Nord = " & Agro_SQL_SaveNum(Delta_Nord) & " , Delta_Est = " & Agro_SQL_SaveNum(Delta_Est) & " , Quota = " & Agro_SQL_SaveNum(Quota))
                StrSQL.Append("       ,Inviato           =  0 ")
                StrSQL.Append("       ,Data_Modifica     =  " & Agro_SQL_SaveDate(Now.Date))
                StrSQL.Append("       ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
                StrSQL.Append("       ,Validita_Inizio   =  " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio))
                StrSQL.Append("       ,Validita_Fine     =  " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine))
                StrSQL.Append(" WHERE    Piva = '" & Agro_SQL_SaveText(PIVA) & "' ")
                StrSQL.Append(" AND      Sa_Cod = " & Sa_Cod & "  ")
                StrSQL.Append(" AND      [Section] = '" & Agro_SQL_SaveText(Section) & "'")
                StrSQL.Append(" AND      Id = '" & Agro_SQL_SaveText(Id) & "'")
                StrSQL.Append(" AND      Descr = '" & Agro_SQL_SaveText(DescrReale) & "'")

            End If


            '---------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
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




    '===========================================================================================
    Public Function Scrivi(ByVal PIVA As String,
                       ByVal Sa_Cod As Int32,
                       ByVal Section As String,
                       ByVal Id As String,
                       ByVal Descr As String,
                       ByVal EColor As Int32,
                       ByVal Layer As String,
                       ByVal ELine As Int32,
                       ByVal VX1 As Decimal,
                       ByVal VY1 As Decimal,
                       ByVal VX2 As Decimal,
                       ByVal VY2 As Decimal,
                       ByVal Rad As Decimal,
                       ByVal Text As String,
                       ByVal Gps As Int32,
                       ByVal Lat As Decimal,
                       ByVal Lon As Decimal,
                       ByVal Pdop As Decimal,
                       ByVal Validita_Inizio As Date,
                       ByVal Validita_Fine As Date,
                       ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
            , Optional ByVal Data_creazione As Date = #2/1/1900# _
            , Optional ByVal Data_modifica As Date = #2/1/1900# _
            , Optional ByVal username_creazione As String = "" _
            , Optional ByVal username_modifica As String = ""
                       ) As Int32

        Dim NomeRoutine As String = "AgronicaCoreGraficaDAL.Grafica_Write.Scrivi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If IsNumeric(Layer) Then
                Layer = CStr(Hex(CInt(Layer)))
            End If

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
            StrSQL.Length = 0
            StrSQL.Append("INSERT INTO Grafica(Piva,Sa_Cod,[Section],Id,Descr,EColor,Layer,ELine,VX1,VY1,VX2,VY2,Rad,[Text],Gps,Lat,Lon,Pdop,Inviato,DataInvio,Data_Creazione,Data_Modifica,UserName_Creazione,UserName_Modifica,Validita_Inizio,Validita_Fine) " &
                    "VALUES (" &
                    "          '" & Agro_SQL_SaveText(Trim(PIVA)) & "' " &
                    "         , " & Agro_SQL_SaveNum(Sa_Cod) & "  " &
                    "         , '" & Agro_SQL_SaveText(Section) & "'" &
                    "         , '" & Agro_SQL_SaveText(Id) & "'" &
                    "         , '" & Agro_SQL_SaveText(Descr) & "'" &
                    "         , " & IIf(EColor = 0, "NULL", EColor) &
                    "         , " & IIf(Layer = "", "NULL", "'" & Agro_SQL_SaveText(Layer) & "'") &
                    "         , " & IIf(ELine = 0, "NULL", ELine) &
                    "         , " & IIf(VX1 = 0, "NULL", Agro_SQL_SaveNum(VX1)) &
                    "         , " & IIf(VY1 = 0, "NULL", Agro_SQL_SaveNum(VY1)) &
                    "         , " & IIf(VX2 = 0, "NULL", Agro_SQL_SaveNum(VX2)) &
                    "         , " & IIf(VY2 = 0, "NULL", Agro_SQL_SaveNum(VY2)) &
                    "         , " & IIf(Rad = 0, "NULL", Agro_SQL_SaveNum(Rad)) &
                    "         , " & IIf(Text = "", "NULL", "'" & Agro_SQL_SaveText(Text) & "'") &
                    "         , " & Gps &
                    "         , " & Agro_SQL_SaveNum(Lat) & " , " & Agro_SQL_SaveNum(Lon) & " , " & Agro_SQL_SaveNum("pdop") &
                    "         , 0, Null, " & Agro_SQL_SaveDate(Data_creazione) & " , " & Agro_SQL_SaveDate(Data_modifica) & "  " &
                    "         ,'" & Agro_SQL_SaveText(username_creazione) & "' " &
                    "         ,'" & Agro_SQL_SaveText(username_modifica) & "' " &
                    "         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  " &
                    "         , " & Agro_SQL_SaveDate(Validita_Fine) & "  " &
                    ")")


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



    '===========================================================================================
    Public Function ScriviEstesa(ByVal PIVA As String,
                   ByVal Sa_Cod As Int32,
                   ByVal Section As String,
                   ByVal Id As String,
                   ByVal Descr As String,
                   ByVal EColor As Int32,
                   ByVal Layer As String,
                   ByVal ELine As Int32,
                   ByVal VX1 As Decimal,
                   ByVal VY1 As Decimal,
                   ByVal VX2 As Decimal,
                   ByVal VY2 As Decimal,
                   ByVal Rad As Decimal,
                   ByVal Text As String,
                   ByVal Gps As Int32,
                   ByVal Lat As Decimal,
                   ByVal Lon As Decimal,
                   ByVal Pdop As Decimal,
                        ByVal Fuso As Integer,
                        ByVal Proiezione As String,
                        ByVal Delta_Nord As Decimal,
                        ByVal Delta_Est As Decimal,
                        ByVal Quota As Decimal,
                   ByVal Validita_Inizio As Date,
                   ByVal Validita_Fine As Date,
                   ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                   ) As Int32

        Dim NomeRoutine As String = "AgronicaCoreGraficaDAL.Grafica_Write.Scrivi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If IsNumeric(Layer) Then
                Layer = CStr(Hex(CInt(Layer)))
            End If





            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append("INSERT INTO Grafica(Piva,Sa_Cod,[Section],Id,Descr,EColor,Layer,ELine,VX1,VY1,VX2,VY2,Rad,[Text],Gps,Lat,Lon,Pdop,Fuso,Proiezione,Delta_Nord,Delta_Est,Quota,Inviato,DataInvio,Data_Creazione,Data_Modifica,UserName_Creazione,UserName_Modifica,Validita_Inizio,Validita_Fine) " &
                    "VALUES (" &
                    "          '" & Agro_SQL_SaveText(Trim(PIVA)) & "' " &
                    "         , " & Agro_SQL_SaveNum(Sa_Cod) & "  " &
                    "         , '" & Agro_SQL_SaveText(Section) & "'" &
                    "         , '" & Agro_SQL_SaveText(Id) & "'" &
                    "         , '" & Agro_SQL_SaveText(Descr) & "'" &
                    "         , " & IIf(EColor = 0, "NULL", EColor) &
                    "         , " & IIf(Layer = "", "NULL", "'" & Agro_SQL_SaveText(Layer) & "'") &
                    "         , " & IIf(ELine = 0, "NULL", ELine) &
                    "         , " & IIf(VX1 = 0, "NULL", Agro_SQL_SaveNum(VX1)) &
                    "         , " & IIf(VY1 = 0, "NULL", Agro_SQL_SaveNum(VY1)) &
                    "         , " & IIf(VX2 = 0, "NULL", Agro_SQL_SaveNum(VX2)) &
                    "         , " & IIf(VY2 = 0, "NULL", Agro_SQL_SaveNum(VY2)) &
                    "         , " & IIf(Rad = 0, "NULL", Agro_SQL_SaveNum(Rad)) &
                    "         , " & IIf(Text = "", "NULL", "'" & Agro_SQL_SaveText(Text) & "'") &
                    "         , " & Gps &
                    "         , " & Agro_SQL_SaveNum(Lat) & " , " & Agro_SQL_SaveNum(Lon) & " , " & Agro_SQL_SaveNum("pdop") &
                    "         , " & Agro_SQL_SaveNum(Fuso) & " " &
                    "         ,'" & Agro_SQL_SaveText(Proiezione) & "' " &
                    "         , " & Agro_SQL_SaveNum(Delta_Nord) & " " &
                    "         , " & Agro_SQL_SaveNum(Delta_Est) & " " &
                    "         , " & Agro_SQL_SaveNum(Quota) & " " &
                    "         , 0, Null, " & Agro_SQL_SaveDate(Date.Now) & " , " & Agro_SQL_SaveDate(Date.Now) & "  " &
                    "         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' " &
                    "         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' " &
                    "         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  " &
                    "         , " & Agro_SQL_SaveDate(Validita_Fine) & "  " &
                    ")")


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



    Public Function Modifica(
                        ByVal PIVA As String,
                        ByVal Sa_Cod As Int32,
                        ByVal Section As String,
                        ByVal Id As String,
                        ByVal Descr As String,
                        ByVal EColor As Int32,
                        ByVal Layer As String,
                        ByVal ELine As Int32,
                        ByVal VX1 As Decimal,
                        ByVal VY1 As Decimal,
                        ByVal VX2 As Decimal,
                        ByVal VY2 As Decimal,
                        ByVal Rad As Decimal,
                        ByVal Text As String,
                        ByVal Gps As Int32,
                        ByVal Lat As Decimal,
                        ByVal Lon As Decimal,
                        ByVal Pdop As Decimal,
                        ByVal Validita_Inizio As Date,
                        ByVal Validita_Fine As Date,
                            ByVal xFiltroAggiuntivo As String,
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                        ) As Boolean

        Dim NomeRoutine As String = "AnagrafeCoreGraficaDAL.Grafica_W.Modifica()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False
        Dim DescrReale As String
        Try
            '---------------------------------------------
            StrSQL.Length = 0
            If Left(UCase(Descr), 6) <> "VERTEX" Then
                DescrReale = Left(Descr, IIf(InStr(1, Descr, " ") = 0, Len(Descr), InStr(1, Descr, " ")) - 1)

                'Query per la modifica dei dati                 ' #### CLASSE ####
                StrSQL.Append("UPDATE Grafica SET " &
               "        Descr             = '" & Agro_SQL_SaveText(Descr) & "'" &
               "       ,EColor            =  " & IIf(EColor = 0, "NULL", Agro_SQL_SaveNum(EColor)) &
               "       ,Layer             =  " & IIf(Layer = "", "NULL", "'" & Agro_SQL_SaveText(Layer) & "'") &
               "       ,ELine             =  " & IIf(ELine = 0, "NULL", "" & Agro_SQL_SaveNum(ELine) & "") &
               "       ,Vx1               =  " & IIf(VX1 = 0, "NULL", Agro_SQL_SaveNum(VX1)) &
               "       ,Vy1               =  " & IIf(VY1 = 0, "NULL", Agro_SQL_SaveNum(VY1)) &
               "       ,Vx2               =  " & IIf(VX2 = 0, "NULL", Agro_SQL_SaveNum(VX2)) &
               "       ,Vy2               =  " & IIf(VY2 = 0, "NULL", Agro_SQL_SaveNum(VY2)) &
               "       ,Rad               =  " & IIf(Rad = 0, "NULL", Agro_SQL_SaveNum(Rad)) &
               "       ,[Text]            =  " & IIf(Text = "", "NULL", "'" & Agro_SQL_SaveText(Text) & "'") &
               "       ,Gps               =  " & Gps &
               "       ,Lat               =  " & Agro_SQL_SaveNum(Lat) &
               "       ,Lon               =  " & Agro_SQL_SaveNum(Lon) &
               "       ,Pdop              =  " & Agro_SQL_SaveNum("pdop") &
               "       ,Inviato           =  0 " &
               "       ,Data_Modifica     =  " & Agro_SQL_SaveDate(Date.Now) &
               "       ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'" &
               "       ,Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Inizio) &
               "       ,Validita_Fine     =  " & Agro_SQL_SaveDate(Validita_Fine) &
               " WHERE    Piva = '" & Agro_SQL_SaveText(PIVA) & "' " &
               " AND      Sa_Cod = " & Sa_Cod & "  " &
               " AND      [Section] = '" & Agro_SQL_SaveText(Section) & "'" &
               " AND      Id = '" & Agro_SQL_SaveText(Id) & "'" &
               " AND      Descr Like '" & Agro_SQL_SaveText(DescrReale) & "%'")

            Else

                DescrReale = UCase(Descr)

                'Query per la modifica dei dati                 ' #### CLASSE ####
                StrSQL.Append("UPDATE Grafica SET " &
               "        Descr             = '" & Agro_SQL_SaveText(Descr) & "'" &
               "       ,EColor            =  " & IIf(EColor = 0, "NULL", Agro_SQL_SaveNum(EColor)) &
               "       ,Layer             =  " & IIf(Layer = "", "NULL", "'" & Agro_SQL_SaveText(Layer) & "'") &
               "       ,ELine             =  " & IIf(ELine = 0, "NULL", "" & Agro_SQL_SaveNum(ELine) & "") &
               "       ,Vx1               =  " & IIf(VX1 = 0, "NULL", Agro_SQL_SaveNum(VX1)) &
               "       ,Vy1               =  " & IIf(VY1 = 0, "NULL", Agro_SQL_SaveNum(VY1)) &
               "       ,Vx2               =  " & IIf(VX2 = 0, "NULL", Agro_SQL_SaveNum(VX2)) &
               "       ,Vy2               =  " & IIf(VY2 = 0, "NULL", Agro_SQL_SaveNum(VY2)) &
               "       ,Rad               =  " & IIf(Rad = 0, "NULL", Agro_SQL_SaveNum(Rad)) &
               "       ,[Text]            =  " & IIf(Text = "", "NULL", "'" & Agro_SQL_SaveText(Text) & "'") &
               "       ,Gps               =  " & Gps &
               "       ,Lat               =  " & Agro_SQL_SaveNum(Lat) &
               "       ,Lon               =  " & Agro_SQL_SaveNum(Lon) &
               "       ,Pdop              =  " & Agro_SQL_SaveNum("pdop") &
               "       ,Inviato           =  0 " &
               "       ,Data_Modifica     =  " & Agro_SQL_SaveDate(Date.Now) &
               "       ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'" &
               "       ,Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Inizio) &
               "       ,Validita_Fine     =  " & Agro_SQL_SaveDate(Validita_Fine) &
               " WHERE    Piva = '" & Agro_SQL_SaveText(PIVA) & "' " &
               " AND      Sa_Cod = " & Sa_Cod & "  " &
               " AND      [Section] = '" & Agro_SQL_SaveText(Section) & "'" &
               " AND      Id = '" & Agro_SQL_SaveText(Id) & "'" &
               " AND      Descr = '" & Agro_SQL_SaveText(DescrReale) & "'")

            End If


            '---------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
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
