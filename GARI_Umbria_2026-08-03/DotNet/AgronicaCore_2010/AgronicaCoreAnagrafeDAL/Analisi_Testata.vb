Imports System.Text
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports System.Data.Entity.Infrastructure
Imports AgronicaCoreDataProvider
Imports InData.Analisi
Imports AgronicaCoreModelsSTD.profilazione
Imports InData.Zoo

Public Class Analisi_Testata_W
    Inherits AgronicaCoreDataProvider.DataProvider

    '#########################################################################
    Public Function Scrivi(ByVal Analisi_Testata_Cod As Integer,
                           ByVal Analisi_Certificato_Cod As Integer,
                           ByVal Analisi_Testata_Des As String,
                           ByVal Analisi_Testata_Data_Inizio As Date,
                           ByVal Analisi_Testata_Data_Fine As Date,
                           ByVal Analisi_Testata_Coord_X As Decimal,
                           ByVal Analisi_Testata_Coord_Y As Decimal,
                           ByVal Analisi_Testata_Riferimento_1 As String,
                           ByVal Analisi_Testata_Riferimento_2 As String,
                           ByVal Analisi_Testata_Riferimento_3 As String,
                           ByVal Analisi_Testata_Riferimento_4 As String,
                           ByVal Analisi_Testata_Riferimento_5 As String,
                           ByVal Analisi_Testata_Note1 As String, ByVal Analisi_Testata_Note2 As String,
                           ByVal Analisi_Testata_Note3 As String, ByVal Analisi_Testata_Note4 As String,
                           ByVal Analisi_Testata_Tipo As Integer, ByVal Analisi_Id_Agenda As Integer,
                           ByVal DataLock As Integer,
                           ByVal Data_Agg As Date,
                           ByVal Validita_Inizio As Date,
                           ByVal Validita_Fine As Date,
                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                           Optional ByVal Id_ClasseTessitura As Integer = 0
                           ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Analisi_Testata_W.Scrivi()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            strSql.Length = 0

            strSql.AppendLine("INSERT INTO Analisi_Testata( ")
            strSql.AppendLine("            Analisi_SuperUser, Analisi_Testata_Cod,  Analisi_Certificato_Cod, ")
            strSql.AppendLine("            Analisi_Testata_Des,         Analisi_Testata_Data_Inizio,")
            strSql.AppendLine("            Analisi_Testata_Data_Fine,   Analisi_Testata_Coord_X,    ")
            strSql.AppendLine("            Analisi_Testata_Coord_Y, ")
            strSql.AppendLine("            Analisi_Testata_Riferimento_1, Analisi_Testata_Riferimento_2,    ")
            strSql.AppendLine("            Analisi_Testata_Riferimento_3, Analisi_Testata_Riferimento_4, Analisi_Testata_Riferimento_5,   ")
            strSql.AppendLine("            Analisi_Testata_Note1, Analisi_Testata_Note2, Analisi_Testata_Note3, Analisi_Testata_Note4, Analisi_Testata_Tipo, Analisi_Id_Agenda, ")
            strSql.AppendLine("            Id_ClasseTessitura, ")
            strSql.AppendLine("            Data_Agg, DataLock, ")
            strSql.AppendLine("            Inviato, DataInvio, ")
            strSql.AppendLine("            Data_Creazione,     Data_Modifica, ")
            strSql.AppendLine("            UserName_Creazione, UserName_Modifica, ")
            strSql.AppendLine("            Validita_Inizio,    Validita_Fine ")
            strSql.AppendLine("            ) ")

            strSql.AppendLine("VALUES (")
            strSql.AppendLine("          '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Analisi_Testata_Cod) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Analisi_Certificato_Cod) & "  ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(Analisi_Testata_Des) & "' ")
            strSql.AppendLine("         , " & Agro_SQL_SaveDate(Analisi_Testata_Data_Inizio) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveDate(Analisi_Testata_Data_Fine) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Analisi_Testata_Coord_X) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Analisi_Testata_Coord_Y) & "  ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(Analisi_Testata_Riferimento_1) & "' ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(Analisi_Testata_Riferimento_2) & "' ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(Analisi_Testata_Riferimento_3) & "' ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(Analisi_Testata_Riferimento_4) & "' ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(Analisi_Testata_Riferimento_5) & "' ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(Analisi_Testata_Note1) & "' ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(Analisi_Testata_Note2) & "' ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(Analisi_Testata_Note3) & "' ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(Analisi_Testata_Note4) & "' ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Analisi_Testata_Tipo) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Analisi_Id_Agenda) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Id_ClasseTessitura) & "  ")


            strSql.AppendLine("         , " & Agro_SQL_SaveDateTime(Data_Agg) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(DataLock) & "  ")
            strSql.AppendLine("         , 0  ")
            strSql.AppendLine("         , NULL")
            strSql.AppendLine("         , " & Agro_SQL_SaveDateTime(Date.Now) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveDateTime(Date.Now) & "  ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            strSql.AppendLine("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")
            strSql.AppendLine(")")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function

    '#########################################################################
    Public Function Modifica(ByVal Analisi_Testata_Cod As Integer,
                             ByVal Analisi_Certificato_Cod As Integer,
                             ByVal Analisi_Testata_Des As String,
                             ByVal Analisi_Testata_Data_Inizio As DateTime,
                             ByVal Analisi_Testata_Data_Fine As DateTime,
                             ByVal Analisi_Testata_Coord_X As Decimal,
                             ByVal Analisi_Testata_Coord_Y As Decimal,
                             ByVal Analisi_Testata_Riferimento_1 As String,
                             ByVal Analisi_Testata_Riferimento_2 As String,
                             ByVal Analisi_Testata_Riferimento_3 As String,
                             ByVal Analisi_Testata_Riferimento_4 As String,
                             ByVal Analisi_Testata_Riferimento_5 As String,
                             ByVal Analisi_Testata_Note1 As String, ByVal Analisi_Testata_Note2 As String,
                             ByVal Analisi_Testata_Note3 As String, ByVal Analisi_Testata_Note4 As String,
                             ByVal Analisi_Testata_Tipo As Integer, ByVal Analisi_Id_Agenda As Integer,
                             ByVal DataLock As Integer,
                             ByVal Validita_Inizio As Date,
                             ByVal Validita_Fine As Date,
                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                             Optional ByVal Id_ClasseTessitura As Integer = 0
                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Analisi_Testata_W.Modifica()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        If Analisi_Testata_Cod = 0 Then
            Throw New Exception("Analisi_Testata_Cod = 0 ")
        End If

        Try

            strSql.Length = 0

            strSql.AppendLine("UPDATE Analisi_Testata SET ")

            strSql.AppendLine("     Analisi_Testata_Des = '" & Agro_SQL_SaveText(Analisi_Testata_Des) & "'")
            strSql.AppendLine("   , Analisi_Certificato_Cod = " & Agro_SQL_SaveNum(Analisi_Certificato_Cod))
            strSql.AppendLine("   , Analisi_Testata_Data_Inizio   =  " & Agro_SQL_SaveDate(Analisi_Testata_Data_Inizio))
            strSql.AppendLine("   , Analisi_Testata_Data_Fine     =  " & Agro_SQL_SaveDate(Analisi_Testata_Data_Fine))
            strSql.AppendLine("   , Analisi_Testata_Coord_X = " & Agro_SQL_SaveNum(Analisi_Testata_Coord_X) & " ")
            strSql.AppendLine("   , Analisi_Testata_Coord_Y = " & Agro_SQL_SaveNum(Analisi_Testata_Coord_Y) & " ")
            strSql.AppendLine("   , Analisi_Testata_Riferimento_1 = '" & Agro_SQL_SaveText(Analisi_Testata_Riferimento_1) & "'")
            strSql.AppendLine("   , Analisi_Testata_Riferimento_2 = '" & Agro_SQL_SaveText(Analisi_Testata_Riferimento_2) & "'")
            strSql.AppendLine("   , Analisi_Testata_Riferimento_3 = '" & Agro_SQL_SaveText(Analisi_Testata_Riferimento_3) & "'")
            strSql.AppendLine("   , Analisi_Testata_Riferimento_4 = '" & Agro_SQL_SaveText(Analisi_Testata_Riferimento_4) & "'")
            strSql.AppendLine("   , Analisi_Testata_Riferimento_5 = '" & Agro_SQL_SaveText(Analisi_Testata_Riferimento_5) & "'")
            strSql.AppendLine("   , Analisi_Testata_Note1 = '" & Agro_SQL_SaveText(Analisi_Testata_Note1) & "'")
            strSql.AppendLine("   , Analisi_Testata_Note2 = '" & Agro_SQL_SaveText(Analisi_Testata_Note2) & "'")
            strSql.AppendLine("   , Analisi_Testata_Note3 = '" & Agro_SQL_SaveText(Analisi_Testata_Note3) & "'")
            strSql.AppendLine("   , Analisi_Testata_Note4 = '" & Agro_SQL_SaveText(Analisi_Testata_Note4) & "'")
            strSql.AppendLine("   , Analisi_Testata_Tipo = " & Agro_SQL_SaveNum(Analisi_Testata_Tipo))
            strSql.AppendLine("   , Analisi_Id_Agenda = " & Agro_SQL_SaveNum(Analisi_Id_Agenda))

            If Not IsNothing(Id_ClasseTessitura) Then
                strSql.AppendLine("   , Id_ClasseTessitura = " & Agro_SQL_SaveNum(Id_ClasseTessitura))
            End If

            strSql.AppendLine("   , Data_Agg          =  " & Agro_SQL_SaveDateTime(Date.Now))
            strSql.AppendLine("   , DataLock          =  " & Agro_SQL_SaveNum(DataLock))
            strSql.AppendLine("   , Inviato           =  0 ")
            strSql.AppendLine("   , DataInvio         =  Null ")
            strSql.AppendLine("   , Data_Modifica     =  " & Agro_SQL_SaveDateTime(Date.Now))
            strSql.AppendLine("   ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            strSql.AppendLine("   , Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Inizio))
            strSql.AppendLine("   , Validita_Fine     =  " & Agro_SQL_SaveDate(Validita_Fine))

            strSql.AppendLine(" WHERE Analisi_SuperUser = '" & Trim(objParametri.PivaSuperUser) & "'")
            strSql.AppendLine(" AND   Analisi_Testata_Cod = " & Agro_SQL_SaveNum(Analisi_Testata_Cod) & " ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function

    '#########################################################################
    Public Function Modifica_Des(ByVal Analisi_Testata_Cod As Integer,
                                 ByVal Analisi_Testata_Des As String,
                                 ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                 ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Analisi_Testata_W.Modifica_Des()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            strSql.Length = 0

            strSql.AppendLine("UPDATE Analisi_Testata SET ")
            strSql.AppendLine("   Analisi_Testata_Des = '" & Agro_SQL_SaveText(Analisi_Testata_Des) & "'")

            strSql.AppendLine("   ,Inviato           =  0 ")
            strSql.AppendLine("   ,DataInvio         =  Null ")
            strSql.AppendLine("   ,Data_Modifica     =  " & Agro_SQL_SaveDateTime(Date.Now))
            strSql.AppendLine("   ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            strSql.AppendLine(" WHERE Analisi_SuperUser = '" & Trim(objParametri.PivaSuperUser) & "'")
            strSql.AppendLine(" AND   Analisi_Testata_Cod = " & Agro_SQL_SaveNum(Analisi_Testata_Cod) & " ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function

    Public Function Aggiorna_DataFine(ByVal Piva As String,
                                      ByVal SaCod As Integer,
                                      ByVal VasCod As Integer,
                                      ByVal Data As Date,
                                      ByVal xFiltroAggiuntivo As String,
                                      ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                      ) As Boolean

        Const nomeRoutine = "AnagrafeCoreAnagrafeDAL.Analisi_Testata_W.Aggiorna_DataFine()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try
            strSql.Length = 0
            strSql.AppendLine(" UPDATE Analisi_Testata ")
            strSql.AppendLine(" SET Analisi_Testata_Data_Fine = " & Agro_SQL_SaveDate(Data) & ", ")
            strSql.AppendLine(" Validita_Fine = " & Agro_SQL_SaveDate(Data) & " ")
            strSql.AppendLine(" FROM Analisi_EntitaxTestata ")
            strSql.AppendLine(" INNER JOIN Analisi_Testata ON Analisi_EntitaxTestata.Analisi_SuperUser = Analisi_Testata.Analisi_SuperUser AND ")
            strSql.AppendLine(" Analisi_EntitaxTestata.Analisi_Testata_Cod = Analisi_Testata.Analisi_Testata_Cod ")

            strSql.AppendLine(" WHERE Analisi_EntitaxTestata.Analisi_SuperUser='" & objParametri.PivaSuperUser & "' ")
            strSql.AppendLine(" AND Analisi_EntitaxTestata.PIVA = '" & Agro_SQL_SaveText(Piva) & "' ")
            strSql.AppendLine(" AND Analisi_EntitaxTestata.Sa_cod = " & Agro_SQL_SaveNum(SaCod))
            strSql.AppendLine(" AND Analisi_Entita_Cod = 10 ")
            strSql.AppendLine(" AND Vas_Cod = " & Agro_SQL_SaveNum(VasCod) & " ")

            strSql.AppendLine(" AND Analisi_Testata_Data_Fine > " & Agro_SQL_SaveDate(Data))

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function

    '#########################################################################
    Public Function Cancella(ByVal Analisi_Testata_Cod As Integer,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Analisi_Testata_W.Cancella()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        If Analisi_Testata_Cod = 0 Then
            Throw New Exception("Analisi_Testata_Cod = 0 ")
        End If


        Try

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then

                strSql.Length = 0

                strSql.AppendLine(" UPDATE  Analisi_Testata ")
                strSql.AppendLine(" SET ")
                strSql.AppendLine("          Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                strSql.AppendLine("         ,Data_Modifica = " & Agro_SQL_SaveDateTime(Date.Now) & " ")
                strSql.AppendLine("         ,Inviato = -1 ")
                strSql.AppendLine(" WHERE Analisi_SuperUser = '" & Agro_SQL_SaveText(Trim(objParametri.PivaSuperUser)) & "' ")
                strSql.AppendLine("   AND    Inviato > 0 ")
            Else

                strSql.Length = 0

                strSql.AppendLine(" DELETE ")
                strSql.AppendLine(" FROM     Analisi_Testata ")
                strSql.AppendLine(" WHERE    Analisi_SuperUser = '" & Agro_SQL_SaveText(Trim(objParametri.PivaSuperUser)) & "' ")
                strSql.AppendLine(" AND      Inviato = 0 ")

            End If

            strSql.AppendLine(" AND Analisi_Testata.Analisi_Testata_Cod = " & Agro_SQL_SaveNum(Analisi_Testata_Cod) & " ")

            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, strSql.ToString, nomeRoutine)
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

'#######################################################################
'#######################################################################
'#######################################################################

Public Class Analisi_Testata_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function Leggi(ByVal Analisi_Testata_Cod As Integer,
                          ByVal Analisi_Id_Agenda As Integer,
                          ByVal xSelezioneVariabile As enumSelezioneVariabile,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                            Optional ByVal Analisi_SuperUser As String = ""
                          ) As DataTable


        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Analisi_Testata_R.Leggi()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try
            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi


                Case enumSelezioneVariabile.Selezione_TabellaCompleta

                    strSql.Length = 0
                    strSql.AppendLine(" SELECT Analisi_Testata.* ")
                    strSql.AppendLine(" FROM   Analisi_Testata ")
                    strSql.AppendLine(" WHERE  Analisi_Testata.Analisi_Testata_Data_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    strSql.AppendLine(" AND    Analisi_Testata.Analisi_Testata_Data_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    strSql.AppendLine(" AND    Analisi_Testata.Analisi_SuperUser = '" & Trim(objParametri.PivaSuperUser) & "'")

                    If Analisi_SuperUser <> "" Then
                        strSql.AppendLine(" AND Analisi_Testata.Analisi_SuperUser = '" & Agro_SQL_SaveText(Analisi_SuperUser) & "' ")
                    End If

                    If Analisi_Testata_Cod <> 0 Then
                        strSql.AppendLine(" AND Analisi_Testata.Analisi_Testata_Cod = " & Analisi_Testata_Cod & " ")
                    End If

                    If Analisi_Id_Agenda <> 0 Then
                        strSql.AppendLine(" AND Analisi_Testata.Analisi_Id_Agenda = " & Analisi_Id_Agenda & " ")
                    End If


                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            strSql.AppendLine(" AND   Analisi_Testata.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            strSql.AppendLine(" AND   Analisi_Testata.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    End If

                Case enumSelezioneVariabile.Selezione_JoinDescrizioni

                Case enumSelezioneVariabile.Selezione_JoinCompleta
                    '
                    '
                    '
                    '

            End Select

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    '##############################################################################################
    Public Function Leggi_Veg_Cod(ByVal Analisi_Testata_Cod As Integer,
                                  ByVal Analisi_Id_Agenda As Integer,
                                  ByVal xSelezioneVariabile As enumSelezioneVariabile,
                                  ByVal xFiltroAggiuntivo As String,
                                  ByVal xOrderBy As String,
                                  ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                  ) As DataTable


        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Analisi_Testata_R.Leggi_Veg_Cod()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try
            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi


                Case enumSelezioneVariabile.Selezione_TabellaCompleta

                    strSql.Length = 0
                    strSql.AppendLine(" SELECT        PDC_Dettagli.Veg_Cod ")
                    strSql.AppendLine(" FROM            Analisi_Testata ")
                    strSql.AppendLine(" INNER Join PDC_Analisi ON Analisi_Testata.Analisi_Testata_Cod = PDC_Analisi.Analisi_Testata_Cod ")
                    strSql.AppendLine(" INNER Join PDC_Campioni ON PDC_Analisi.PivaSuperUser = PDC_Campioni.PivaSuperUser AND PDC_Analisi.ID_PDC_Testata = PDC_Campioni.ID_PDC_Testata AND PDC_Analisi.ID_PDC_Dettagli = PDC_Campioni.ID_PDC_Dettagli AND PDC_Analisi.ID_PDC_Campione = PDC_Campioni.ID_PDC_Campione")
                    strSql.AppendLine("  INNER JOIN PDC_Dettagli ON PDC_Campioni.PivaSuperUser = PDC_Dettagli.PivaSuperUser AND PDC_Campioni.ID_PDC_Testata = PDC_Dettagli.ID_PDC_Testata AND PDC_Campioni.ID_PDC_Dettagli = PDC_Dettagli.ID_PDC_Dettagli ")

                    strSql.AppendLine(" WHERE  Analisi_Testata.Analisi_Testata_Data_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    strSql.AppendLine(" AND    Analisi_Testata.Analisi_Testata_Data_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    strSql.AppendLine(" AND    Analisi_Testata.Analisi_SuperUser = '" & Trim(objParametri.PivaSuperUser) & "'")

                    If Analisi_Testata_Cod <> 0 Then
                        strSql.AppendLine(" AND Analisi_Testata.Analisi_Testata_Cod = " & Analisi_Testata_Cod & " ")
                    End If

                    If Analisi_Id_Agenda <> 0 Then
                        strSql.AppendLine(" AND Analisi_Testata.Analisi_Id_Agenda = " & Analisi_Id_Agenda & " ")
                    End If

                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            strSql.AppendLine(" AND   Analisi_Testata.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            strSql.AppendLine(" AND   Analisi_Testata.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    End If

                Case enumSelezioneVariabile.Selezione_JoinDescrizioni

                Case enumSelezioneVariabile.Selezione_JoinCompleta
                    '
                    '
                    '
                    '

            End Select

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    '##############################################################################################
    Public Function DistinctTestataCod(ByVal xFiltroAggiuntivo As String,
                                       ByVal xOrderBy As String,
                                       ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                       ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Analisi_Testata_R.DistinctTestataCod()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try
            strSql.Length = 0
            strSql.AppendLine(" SELECT DISTINCT Analisi_Testata_Cod ")
            strSql.AppendLine(" FROM   Analisi_Testata ")
            strSql.AppendLine(" WHERE  Analisi_Testata.Analisi_Testata_Data_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            strSql.AppendLine(" AND    Analisi_Testata.Analisi_Testata_Data_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
            strSql.AppendLine(" AND    Analisi_Testata.Analisi_SuperUser = '" & Trim(objParametri.PivaSuperUser) & "'")

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.AppendLine(" AND   Analisi_Testata.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.AppendLine(" AND   Analisi_Testata.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                strSql.AppendLine(" ORDER BY Analisi_Testata_Cod ")
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    '##############################################################################################
    Public Function DistinctTestataCod_conFiltroPiva(ByVal Piva As String,
                                                     ByVal xFiltroAggiuntivo As String,
                                                     ByVal xOrderBy As String,
                                                     ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                     ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Analisi_Testata_R.DistinctTestataCod_conFiltroPiva()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try
            strSql.Length = 0
            strSql.AppendLine(" SELECT DISTINCT Analisi_Testata.Analisi_Testata_Cod ")
            strSql.AppendLine(" FROM   Analisi_Testata ")
            strSql.AppendLine(" INNER JOIN Analisi_EntitaxTestata ")
            strSql.AppendLine(" ON Analisi_EntitaxTestata.Analisi_Testata_Cod = Analisi_Testata.Analisi_Testata_Cod AND Analisi_EntitaxTestata.Analisi_SuperUser = Analisi_Testata.Analisi_SuperUser ")
            strSql.AppendLine(" WHERE  Analisi_Testata.Analisi_Testata_Data_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            strSql.AppendLine(" AND    Analisi_Testata.Analisi_Testata_Data_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
            strSql.AppendLine(" AND    Analisi_Testata.Analisi_SuperUser = '" & Trim(objParametri.PivaSuperUser) & "'")
            strSql.AppendLine(" AND    Analisi_EntitaxTestata.Piva = '" & Agro_SQL_SaveText(Trim(Piva)) & "'")

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.AppendLine(" AND   Analisi_Testata.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.AppendLine(" AND   Analisi_Testata.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                strSql.AppendLine(" ORDER BY Analisi_Testata.Analisi_Testata_Cod ")
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    '##############################################################################################
    Public Function LeggiConCertificati(ByVal Piva As String,
                                        ByVal Sa_Cod As Integer,
                                        ByVal Analisi_Testata_Cod As Integer,
                                        ByVal Analisi_Certificato_Cod As Integer,
                                        ByVal xFiltroAggiuntivo As String,
                                        ByVal xOrderBy As String,
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                        ) As DataTable


        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Analisi_Testata_R.LeggiConCertificati()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0

            strSql.AppendLine(" SELECT DISTINCT  Analisi_Testata.Analisi_Testata_Cod, Analisi_Testata.Analisi_Testata_Des, ")
            strSql.AppendLine(" Analisi_Testata_Data_Inizio, Analisi_Testata_Data_Fine, ")
            strSql.AppendLine(" Analisi_Certificato.Analisi_Certificato_Des ")
            strSql.AppendLine(" ,isnull(Analisi_Testata_Note1,'') as Analisi_Testata_Note1 ")
            strSql.AppendLine(" FROM Analisi_Testata ")
            strSql.AppendLine(" INNER JOIN Analisi_EntitaxTestata ON Analisi_Testata.Analisi_SuperUser = Analisi_EntitaxTestata.Analisi_SuperUser AND  ")
            strSql.AppendLine(" Analisi_Testata.Analisi_Testata_Cod = Analisi_EntitaxTestata.Analisi_Testata_Cod ")
            'strSql.AppendLine(" LEFT OUTER JOIN Analisi_EntitaxCertificato ON Analisi_EntitaxTestata.Analisi_SuperUser = Analisi_EntitaxCertificato.Analisi_SuperUser AND  ")
            'strSql.AppendLine(" Analisi_EntitaxTestata.Analisi_Entita_Cod = Analisi_EntitaxCertificato.Analisi_Entita_Cod ")
            strSql.AppendLine(" LEFT OUTER JOIN Analisi_Certificato ON Analisi_Testata.Analisi_Certificato_Cod = Analisi_Certificato.Analisi_Certificato_Cod AND  ")
            strSql.AppendLine(" Analisi_Testata.Analisi_SuperUser = Analisi_Certificato.Analisi_SuperUser ")

            strSql.AppendLine(" WHERE   Analisi_Testata.Analisi_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            strSql.AppendLine(" AND     Analisi_Testata.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            strSql.AppendLine(" AND     Analisi_Testata.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            If Piva <> "" Then
                strSql.AppendLine(" AND     Analisi_EntitaxTestata.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If

            If Sa_Cod <> 0 Then
                strSql.AppendLine(" AND     Analisi_EntitaxTestata.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            End If

            If Analisi_Certificato_Cod <> 0 Then
                strSql.AppendLine(" AND     Analisi_Certificato.Analisi_Certificato_Cod = " & Agro_SQL_SaveNum(Analisi_Certificato_Cod) & " ")
            End If

            If Analisi_Testata_Cod <> 0 Then
                strSql.AppendLine(" AND     Analisi_Testata.Analisi_Testata_Cod = " & Agro_SQL_SaveNum(Analisi_Testata_Cod) & " ")
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    ' commentati a causa del left join
                    'strSql.AppendLine(" AND   Analisi_Certificato.Inviato >=0 ")
                    strSql.AppendLine(" AND   Analisi_Testata.Inviato >=0 ")
                    'strSql.AppendLine(" AND   Analisi_EntitaxCertificato.Inviato >=0 ")
                    strSql.AppendLine(" AND   Analisi_EntitaxTestata.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    'strSql.AppendLine(" AND   Analisi_Certificato.Inviato =-1 ")
                    strSql.AppendLine(" AND   Analisi_Testata.Inviato =-1 ")
                    'strSql.AppendLine(" AND   Analisi_EntitaxCertificato.Inviato =-1 ")
                    strSql.AppendLine(" AND   Analisi_EntitaxTestata.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    'aggiunto il dettaglio delle entita rispetto alla precedente
    Public Function LeggiConCertificati_Entita(ByVal Piva As String,
                                               ByVal Sa_Cod As Integer,
                                               ByVal Analisi_Testata_Cod As Integer,
                                               ByVal Analisi_Certificato_Cod As Integer,
                                               ByVal Validita_Inizio As Date, ByVal Validita_Fine As Date,
                                               ByVal xFiltroAggiuntivo As String,
                                               ByVal xOrderBy As String,
                                               ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                               Optional defaultSezioneSubalterno As String = ""
                                               ) As DataTable


        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Analisi_Testata_R.LeggiConCertificati_Entita()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0

            strSql.AppendLine(" SELECT DISTINCT  Analisi_Testata.Analisi_Testata_Cod, Analisi_Testata.Analisi_Testata_Des, ")
            strSql.AppendLine(" Analisi_Testata_Data_Inizio, Analisi_Testata_Data_Fine, Analisi_Testata.ID_ClasseTessitura, ")
            strSql.AppendLine(" Analisi_Certificato.Analisi_Certificato_Des ")
            strSql.AppendLine(" ,isnull(Analisi_Testata_Note1,'') as Analisi_Testata_Note1 ")
            strSql.AppendLine(" ,analisi_entita_cod,piva,sa_cod,campo_cod,appezza,id_imp ")
            strSql.AppendLine(" ,prov ")
            strSql.AppendLine(" ,com ")
            strSql.AppendLine($", CASE ")
            If defaultSezioneSubalterno = "0" Then
                'se sezione = '' --> converto in '0'
                strSql.AppendLine($"   WHEN ISNULL(sezione, '{defaultSezioneSubalterno}') = '' THEN '{defaultSezioneSubalterno}' ")
            ElseIf defaultSezioneSubalterno = "" Then
                'se sezione = '0' --> converto in ''
                strSql.AppendLine($"   WHEN ISNULL(sezione, '{defaultSezioneSubalterno}') = '0' THEN '{defaultSezioneSubalterno}' ")
            End If
            strSql.AppendLine($"        ELSE sezione END AS sezione ")
            strSql.AppendLine(" ,foglio ")
            strSql.AppendLine(" ,numero ")
            strSql.AppendLine($", CASE WHEN ISNULL(subalterno, '{defaultSezioneSubalterno}') = '{defaultSezioneSubalterno}' ")
            strSql.AppendLine($"        THEN '{defaultSezioneSubalterno}' ")
            If defaultSezioneSubalterno = "0" Then
                'se subalterno = '' --> converto in '0'
                strSql.AppendLine($"   WHEN ISNULL(subalterno, '{defaultSezioneSubalterno}') = '' THEN '{defaultSezioneSubalterno}' ")
            ElseIf defaultSezioneSubalterno = "" Then
                'se subalterno = '0' --> converto in ''
                strSql.AppendLine($"   WHEN ISNULL(subalterno, '{defaultSezioneSubalterno}') = '0' THEN '{defaultSezioneSubalterno}' ")
            End If
            strSql.AppendLine($"        ELSE subalterno END AS subalterno ")

            strSql.AppendLine(" FROM Analisi_Testata ")
            strSql.AppendLine(" INNER JOIN Analisi_EntitaxTestata ON Analisi_Testata.Analisi_SuperUser = Analisi_EntitaxTestata.Analisi_SuperUser AND  ")
            strSql.AppendLine(" Analisi_Testata.Analisi_Testata_Cod = Analisi_EntitaxTestata.Analisi_Testata_Cod ")
            strSql.AppendLine(" LEFT OUTER JOIN Analisi_Certificato ON Analisi_Testata.Analisi_Certificato_Cod = Analisi_Certificato.Analisi_Certificato_Cod AND  ")
            strSql.AppendLine(" Analisi_Testata.Analisi_SuperUser = Analisi_Certificato.Analisi_SuperUser ")

            strSql.AppendLine(" WHERE   Analisi_Testata.Analisi_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            strSql.AppendLine(" AND     Analisi_Testata.Analisi_Testata_Data_Inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
            strSql.AppendLine(" AND     Analisi_Testata.Analisi_Testata_Data_Fine >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")

            If Piva <> "" Then
                strSql.AppendLine(" AND     Analisi_EntitaxTestata.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If

            If Sa_Cod <> 0 Then
                strSql.AppendLine(" AND     Analisi_EntitaxTestata.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            End If

            If Analisi_Certificato_Cod <> 0 Then
                strSql.AppendLine(" AND     Analisi_Certificato.Analisi_Certificato_Cod = " & Agro_SQL_SaveNum(Analisi_Certificato_Cod) & " ")
            End If

            If Analisi_Testata_Cod <> 0 Then
                strSql.AppendLine(" AND     Analisi_Testata.Analisi_Testata_Cod = " & Agro_SQL_SaveNum(Analisi_Testata_Cod) & " ")
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.AppendLine(" AND   Analisi_Testata.Inviato >=0 ")
                    strSql.AppendLine(" AND   Analisi_EntitaxTestata.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.AppendLine(" AND   Analisi_Testata.Inviato =-1 ")
                    strSql.AppendLine(" AND   Analisi_EntitaxTestata.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    Public Function TestataDes_from_TestataCod(ByVal Analisi_Testata_Cod As Integer,
                                               ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                               ) As String

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Analisi_Testata_R.TestataDes_from_TestataCod()"

        Dim dt As DataTable
        Dim strSql As New StringBuilder
        Dim strRet As String = ""

        Dim messaggioErrore As String = ""

        Try

            strSql.Length = 0
            strSql.AppendLine(" Select  Analisi_Testata_Des ")
            strSql.AppendLine(" FROM    Analisi_Testata ")
            strSql.AppendLine(" WHERE   (Analisi_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "')  ")
            strSql.AppendLine(" AND     (Analisi_Testata_Cod = " & Agro_SQL_SaveNum(Analisi_Testata_Cod) & ") ")

            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)

            If Not dt Is Nothing AndAlso dt.Rows.Count > 0 Then
                strRet = CStr(dt.Rows(0).Item("Analisi_Testata_Des"))
            Else
                Return ""
            End If

            dt = Nothing

        Catch ex As Exception
            strRet = ""
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return strRet

    End Function

    Public Function Descrizione_from_TestataCod(ByVal Analisi_Testata_Cod As Integer,
                                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                ) As String

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Analisi_Testata_R.Descrizione_from_TestataCod()"

        Dim dt As DataTable
        Dim strSql As New StringBuilder
        Dim strRet As String = ""

        Dim messaggioErrore As String = ""

        Try

            strSql.Length = 0
            strSql.AppendLine(" SELECT  Analisi_Testata_Des, Analisi_Testata_Data_Inizio ")
            strSql.AppendLine(" FROM    Analisi_Testata ")
            strSql.AppendLine(" WHERE   (Analisi_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "')  ")
            strSql.AppendLine(" AND     (Analisi_Testata_Cod = " & Agro_SQL_SaveNum(Analisi_Testata_Cod) & ") ")

            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)

            If Not dt Is Nothing AndAlso dt.Rows.Count > 0 Then
                Dim data As String = String.Empty
                If Not IsDBNull(dt.Rows(0).Item("Analisi_Testata_Data_Inizio")) Then
                    data = "(" & CDate(dt.Rows(0).Item("Analisi_Testata_Data_Inizio")).ToShortDateString & ")"
                End If

                strRet = CStr(dt.Rows(0).Item("Analisi_Testata_Des")) & " - " & data
            Else
                Return ""
            End If

            dt = Nothing

        Catch ex As Exception
            strRet = ""
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return strRet

    End Function

    Public Function TestataTipo_from_TestataCod(ByVal Analisi_Testata_Cod As Integer,
                                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                ) As Integer

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Analisi_Testata_R.TestataTipo_from_TestataCod()"

        Dim dt As DataTable
        Dim strSql As New StringBuilder
        Dim intRet As Integer = 0

        Dim messaggioErrore As String = ""

        Try

            strSql.Length = 0
            strSql.AppendLine(" Select  Analisi_Testata_Tipo ")
            strSql.AppendLine(" FROM    Analisi_Testata ")
            strSql.AppendLine(" WHERE   (Analisi_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "')  ")
            strSql.AppendLine(" AND     (Analisi_Testata_Cod = " & Agro_SQL_SaveNum(Analisi_Testata_Cod) & ") ")

            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)

            If Not dt Is Nothing AndAlso dt.Rows.Count > 0 Then
                intRet = CInt(dt.Rows(0).Item("Analisi_Testata_Tipo"))
            Else
                Return ""
            End If

            dt = Nothing

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            intRet = 0
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return intRet

    End Function

    ''' <summary>
    ''' strAnalisiCatasto ad esempio = "(Piva = '" & piva & "' AND prov = '" & prov & "' AND com = '" & com & "' AND sezione = '" & sezione & "' AND numero = " & CStr(numero) & " AND foglio = " & CStr(foglio) & " AND subalterno = '" & subalterno & "')"
    ''' </summary>
    Public Function OttieniFiltroAnalisi(ByVal piva As String,
                                         ByVal saCod As Integer,
                                         ByVal campoCod As Integer,
                                         ByVal appezza As Integer,
                                         ByVal idReg As Integer,
                                         ByVal strAnalisiCatasto As String,
                                         ByRef numAnalisi As Integer,
                                         ByRef AnalisiCodPiuRecente As Integer,
                                         ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                         ) As String

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Analisi_Testata_R.OttieniFiltroAnalisi"
        Dim messaggioErrore As String = ""

        Dim strAnalisi As String = ""
        Dim dtTutteAnalisi As DataTable

        Try

            numAnalisi = 0
            dtTutteAnalisi = LeggiConCertificati_Entita("", 0, 0, 0, AGRODATAINIZIO, AGRODATAFINE,
                                                        " piva = '" & Agro_SQL_SaveText(piva) & "' ",
                                                        " Analisi_Testata_Data_Inizio DESC ",
                                                        objParametri)

            Dim strPiva As String = ""
            If piva <> "" Then
                strPiva = piva
            End If

            Dim strSaCod As String = ""
            If saCod <> 0 Then
                strSaCod = CStr(saCod)
            End If

            Dim strCampi As String = ""
            If strPiva <> "" AndAlso strSaCod <> "" AndAlso campoCod <> 0 Then
                strCampi = " (Piva = '" & strPiva & "' AND Sa_Cod = " & strSaCod & " AND Campo_Cod = " & CStr(campoCod) & ")"
            End If

            Dim strAppezzamenti As String = ""
            If strPiva <> "" AndAlso strSaCod <> "" AndAlso appezza <> 0 Then
                strAppezzamenti = " (Piva = '" & strPiva & "' AND Sa_Cod = " & strSaCod & " AND Campo_Cod = " & CStr(campoCod) & " AND Appezza= " & CStr(appezza) & ")"
            End If

            Dim strRegImpianti As String = ""
            If strPiva <> "" AndAlso strSaCod <> "" AndAlso appezza <> 0 AndAlso idReg <> 0 Then
                strRegImpianti = " (Piva = '" & strPiva & "' AND Sa_Cod = " & strSaCod & " AND Campo_Cod = " & CStr(campoCod) & " AND Appezza= " & CStr(appezza) & " AND Id_Imp = " & CStr(idReg) & ")"
            End If

            'Dim strAnalisiCatasto As String = ""
            'If Not filtroParticelle Is Nothing AndAlso filtroParticelle <> "" Then
            '    Dim listPart As List(Of PUA_ParticellaVincoloAgronomico) = JsonConvert.DeserializeObject(filtroParticelle, GetType(List(Of PUA_ParticellaVincoloAgronomico)))

            '    strAnalisiCatasto = PUA_ParticellaVincoloAgronomico.EstrapolaStringaFiltroAnalisiCatasto(piva, listPart)

            '    'If Not listPart Is Nothing AndAlso listPart.Count > 0 Then

            '    '    For Each part As PUA_ParticellaVincoloAgronomico In listPart
            '    '        strAnalisiCatasto &= IIf(strAnalisiCatasto = "", "", " OR ") & 
            '    '                             " (Piva = '" & piva & "' AND prov = '" & part.Part_PROV & "' AND com = '" & part.Part_COM & "' AND sezione = '" & part.Part_SEZIONE & "' AND numero = " & CStr(part.Part_NUMERO) & " AND foglio = " & CStr(part.Part_FOGLIO) & " AND subalterno = '" & part.Part_SUBALTERNO & "')"
            '    '    Next

            '    'End If

            'End If

            If Not dtTutteAnalisi Is Nothing AndAlso dtTutteAnalisi.Rows.Count > 0 Then
                Dim drAnalisi() As DataRow = Nothing
                If strAnalisiCatasto <> "" Then
                    drAnalisi = dtTutteAnalisi.Select("analisi_entita_cod=" & enum_Entita_Analisi.Particella & " and (" & strAnalisiCatasto & ")")
                End If
                If Not (Not drAnalisi Is Nothing AndAlso drAnalisi.Length > 0) Then
                    If strRegImpianti <> "" Then
                        drAnalisi = dtTutteAnalisi.Select("analisi_entita_cod=" & enum_Entita_Analisi.Impianto & " and (" & strRegImpianti & ")")
                    End If
                    If Not (Not drAnalisi Is Nothing AndAlso drAnalisi.Length > 0) Then
                        If strAppezzamenti <> "" Then
                            drAnalisi = dtTutteAnalisi.Select("analisi_entita_cod=" & enum_Entita_Analisi.Appezzamento & " and (" & strAppezzamenti & ")")
                        End If
                        If Not (Not drAnalisi Is Nothing AndAlso drAnalisi.Length > 0) Then
                            If strCampi <> "" Then
                                drAnalisi = dtTutteAnalisi.Select("analisi_entita_cod=" & enum_Entita_Analisi.Campo & " and (" & strCampi & ")")
                            End If
                            If Not (Not drAnalisi Is Nothing AndAlso drAnalisi.Length > 0) Then
                                drAnalisi = dtTutteAnalisi.Select("analisi_entita_cod=" & enum_Entita_Analisi.Centro & " and Piva = '" & strPiva & "' AND Sa_Cod = " & strSaCod)
                                If Not (Not drAnalisi Is Nothing AndAlso drAnalisi.Length > 0) Then
                                    drAnalisi = dtTutteAnalisi.Select("analisi_entita_cod=" & enum_Entita_Analisi.Impresa & " and Piva = '" & strPiva & "'")
                                End If
                            End If
                        End If
                    End If
                End If

                If Not drAnalisi Is Nothing Then
                    For Each drA As DataRow In drAnalisi
                        If Not strAnalisi.Contains(drA.Item("analisi_testata_cod") & ",") Then
                            strAnalisi &= drA.Item("analisi_testata_cod") & ","
                            numAnalisi += 1
                            If AnalisiCodPiuRecente = 0 Then
                                AnalisiCodPiuRecente = drA.Item("analisi_testata_cod")
                            End If
                        End If
                    Next

                    If strAnalisi <> "" Then
                        'tolgo l'ultima virgola
                        strAnalisi = Left(strAnalisi, strAnalisi.Length - 1)
                    End If
                End If
            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dtTutteAnalisi = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return strAnalisi

    End Function

#Region "Analisi Terreno NG"
    Public Function Leggi_ListaAnalisiTerreno_NG(Analisi_Testata_Cod As Integer,
                                                 ApplicaVisibilitaUtente As Boolean,
                                                 InData As LeggiListaAnalisiTerreno,
                                                 xFiltroAggiuntivo As String,
                                                 xOrderBy As String,
                                                 objParametriServer As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                 objParametriUtenti As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                 ) As DataTable


        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Analisi_Testata_R.Leggi_AnalisiTerreno_NG()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try
            Dim utenti_DB_name As String = String.Format("{0}.dbo.", NomeDataBase_FromStringaConnessione(objParametriUtenti.StringaConnessione))

            strSql.Length = 0
            strSql.AppendLine(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED ")

            strSql.AppendLine(" ;WITH UtentiDettagli_CTE AS ( ")
            strSql.AppendLine("      SELECT ")
            strSql.AppendLine("         CodFisc AS [keyUtente] ")
            strSql.AppendLine("       , LTRIM(RTRIM(COALESCE(nome, '') + ' ' + COALESCE(Cognome, '') + ' ' + COALESCE(Rag_Soc, ''))) AS Username ")
            strSql.AppendLine("      FROM " + utenti_DB_name + "Utenti_Dettagli ")
            strSql.AppendLine(" ), ")

            GetUtentiVisibilitaAppoggio_CTE(strSql, objParametriServer)

            GetAnalisiEntitaxTestata_CTE(strSql, ApplicaVisibilitaUtente, InData.ApplicaVisibilitaUMA, objParametriServer, objParametriUtenti)

            strSql.AppendLine(" SELECT ")
            strSql.AppendLine("      Analisi_Testata.Analisi_Testata_Cod ")
            strSql.AppendLine("    , Analisi_Testata_Des AS Descrizione ")
            strSql.AppendLine("    , ISNULL(Analisi_Certificato.Analisi_Certificato_Cod, 0) AS analisi_certificato_cod ")
            strSql.AppendLine("    , ISNULL(Analisi_Certificato.Analisi_Certificato_Des, '') AS NumeroCertificato ")
            strSql.AppendLine("    , ISNULL(Contatti.Rag_Soc + Contatti.Nome + ' ' + Contatti.Cognome, '') AS Laboratorio ")
            strSql.AppendLine("    , ISNULL(Analisi_Tipologia.Analisi_Tipologia_Des, '') AS [Schema] ")
            strSql.AppendLine("    , Analisi_Testata.Analisi_Testata_Note1 ")

            strSql.AppendLine("    , Analisi_Testata.Analisi_Testata_Data_Inizio AS Validita_Inizio ")
            strSql.AppendLine("    , Analisi_Testata.Analisi_Testata_Data_Fine AS Validita_Fine ")

            strSql.AppendLine("    , Creazione.Username AS Utente_Creazione ")
            strSql.AppendLine("    , Modifica.Username AS Utente_Modifica ")

            strSql.AppendLine("    , Analisi_Testata.Data_Creazione ")
            strSql.AppendLine("    , Analisi_Testata.Data_Modifica ")

            strSql.AppendLine(" FROM Analisi_Testata ")

            strSql.AppendLine(" LEFT JOIN Analisi_Certificato ON ")
            strSql.AppendLine("     Analisi_Certificato.Analisi_Certificato_Cod = Analisi_Testata.Analisi_Certificato_Cod ")
            strSql.AppendLine(" LEFT JOIN Risorse_Umane ON ")
            strSql.AppendLine("     Risorse_Umane.Cod_RisUm = Analisi_Certificato.Analisi_Certificato_Laboratorio ")
            strSql.AppendLine(" LEFT JOIN Contatti ON ")
            strSql.AppendLine("     Contatti.Cod_Contatto = Risorse_Umane.Cod_Contatto ")
            strSql.AppendLine(" LEFT JOIN Analisi_Tipologia ON ")
            strSql.AppendLine("     Analisi_Tipologia.Analisi_Tipologia_Cod = Analisi_Certificato.Analisi_Certificato_TipologiaCod ")

            strSql.AppendLine(" LEFT JOIN UtentiDettagli_CTE Creazione ON Creazione.[keyUtente] = Analisi_Testata.Username_Creazione ")
            strSql.AppendLine(" LEFT JOIN UtentiDettagli_CTE Modifica ON Modifica.[keyUtente] = Analisi_Testata.Username_Modifica ")

            strSql.AppendLine(" INNER JOIN AnalisiEntitaxTestata_CTE ON AnalisiEntitaxTestata_CTE.Analisi_Testata_Cod = Analisi_Testata.Analisi_Testata_Cod ")

            strSql.AppendLine(" WHERE 1 = 1 ")

            strSql.AppendLine(" AND Analisi_Testata.Analisi_SuperUser = '" & Agro_SQL_SaveText(Trim(objParametriServer.PivaSuperUser)) & "' ")

            If InData.Piva <> "" Then
                strSql.AppendLine("AND AnalisiEntitaxTestata_CTE.Piva = '" & Agro_SQL_SaveText(Trim(InData.Piva)) & "' ")
            End If

            If Analisi_Testata_Cod <> 0 Then
                strSql.AppendLine(" AND Analisi_Testata.Analisi_Testata_Cod = " & Agro_SQL_SaveNum(Analisi_Testata_Cod) & " ")
            End If

            If InData.Data <> AGRODATAINIZIO Then
                strSql.AppendLine(" AND Analisi_Testata_Data_Inizio <= " & Agro_SQL_SaveDate(InData.Data) & " ")
                strSql.AppendLine(" AND Analisi_Testata_Data_Fine >= " & Agro_SQL_SaveDate(InData.Data) & " ")
            End If

            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametriServer))
            End If

            If xOrderBy <> "" Then
                strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametriServer))
            Else
                strSql.AppendLine(" ORDER BY Analisi_Testata.Data_Modifica DESC ")

            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametriServer, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    Public Function Leggi_AnalisiTerreno_NG(Analisi_Testata_Cod As Integer,
                                            xFiltroAggiuntivo As String,
                                            xOrderBy As String,
                                            objParametriServer As AgronicaCoreParametri
                                            ) As DataTable


        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Analisi_Testata_R.Leggi_AnalisiTerreno_NG()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try
            strSql.Length = 0
            strSql.AppendLine(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED ")

            strSql.AppendLine(" SELECT ")
            strSql.AppendLine("      Analisi_Testata.Analisi_Testata_Cod ")
            strSql.AppendLine("    , Analisi_Testata.Analisi_Testata_Des ")

            strSql.AppendLine("    , Analisi_Testata.analisi_testata_data_inizio ")
            strSql.AppendLine("    , Analisi_Testata.analisi_testata_data_fine ")
            strSql.AppendLine("    , Analisi_Testata.Analisi_Testata_Coord_X ")
            strSql.AppendLine("    , Analisi_Testata.Analisi_Testata_Coord_Y ")
            strSql.AppendLine("    , Analisi_Testata.Analisi_Testata_Riferimento_1 ")
            strSql.AppendLine("    , Analisi_Testata.Analisi_Testata_Riferimento_2 ")
            strSql.AppendLine("    , Analisi_Testata.Analisi_Testata_Riferimento_3 ")
            strSql.AppendLine("    , Analisi_Testata.Analisi_Testata_Riferimento_4 ")
            strSql.AppendLine("    , Analisi_Testata.Analisi_Testata_Riferimento_5 ")
            strSql.AppendLine("    , Analisi_Testata.analisi_testata_note1 ")
            strSql.AppendLine("    , Analisi_Testata.analisi_testata_tipo ")
            strSql.AppendLine("    , Analisi_Testata.Id_ClasseTessitura ")

            strSql.AppendLine("    , ISNULL(Analisi_Certificato.Analisi_Certificato_Cod, 0) AS Analisi_Certificato_Cod ")
            strSql.AppendLine("    , ISNULL(Analisi_Certificato.Analisi_Certificato_Des, '') AS Analisi_Certificato_Des ")
            strSql.AppendLine("    , ISNULL(Analisi_Certificato.analisi_certificato_laboratorio, 0) AS analisi_certificato_laboratorio ")

            strSql.AppendLine("    , ISNULL(Contatti.piva, '') AS piva ")
            strSql.AppendLine("    , ISNULL(Contatti.cod_Contatto, '') AS cod_Contatto ")
            strSql.AppendLine("    , ISNULL(Contatti.Rag_Soc, '') AS Rag_Soc ")
            strSql.AppendLine("    , ISNULL(Contatti.Nome, '') AS Nome ")
            strSql.AppendLine("    , ISNULL(Contatti.Cognome, '') AS Cognome ")
            strSql.AppendLine("    , ISNULL(Risorse_Umane.cod_risum, 0) AS cod_risum ")

            strSql.AppendLine("    , ISNULL(Analisi_Certificato.Analisi_Certificato_TipologiaCod, 0) AS Analisi_Certificato_TipologiaCod ")
            strSql.AppendLine("    , ISNULL(Analisi_Tipologia.Analisi_Tipologia_Des, '') AS Analisi_Certificato_TipologiaDes ")


            strSql.AppendLine(" FROM Analisi_Testata ")

            strSql.AppendLine(" LEFT JOIN Analisi_Certificato ON ")
            strSql.AppendLine("     Analisi_Certificato.Analisi_Certificato_Cod = Analisi_Testata.Analisi_Certificato_Cod ")
            strSql.AppendLine(" LEFT JOIN Risorse_Umane ON ")
            strSql.AppendLine("     Risorse_Umane.Cod_RisUm = Analisi_Certificato.Analisi_Certificato_Laboratorio ")
            strSql.AppendLine(" LEFT JOIN Contatti ON ")
            strSql.AppendLine("     Contatti.Cod_Contatto = Risorse_Umane.Cod_Contatto ")
            strSql.AppendLine(" LEFT JOIN Analisi_Tipologia ON ")
            strSql.AppendLine("     Analisi_Tipologia.Analisi_Tipologia_Cod = Analisi_Certificato.Analisi_Certificato_TipologiaCod ")

            strSql.AppendLine(" WHERE 1 = 1 ")

            strSql.AppendLine(" AND Analisi_Testata.Analisi_SuperUser = '" & Agro_SQL_SaveText(Trim(objParametriServer.PivaSuperUser)) & "' ")

            If Analisi_Testata_Cod <> 0 Then
                strSql.AppendLine(" AND Analisi_Testata.Analisi_Testata_Cod = " & Agro_SQL_SaveNum(Analisi_Testata_Cod) & " ")
            End If

            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametriServer))
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametriServer, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

#End Region
#Region "Other Get Methods"
    Private Sub GetUtentiVisibilitaAppoggio_CTE(ByRef stbCTE As StringBuilder,
                                                objParametriServer As AgronicaCoreParametri)

        stbCTE.AppendLine(" UtentiVisibilitaAppoggioCentriAziendali_CTE AS ( ")
        stbCTE.AppendLine("      SELECT ")
        stbCTE.AppendLine("         Piva AS [keyPiva] ")
        stbCTE.AppendLine("       , Sa_Cod AS [keySaCod] ")
        stbCTE.AppendLine("      FROM Utenti_Visibilita_Appoggio ")
        stbCTE.AppendLine("      WHERE Username = '" & Agro_SQL_SaveText(objParametriServer.UtenteUsername) & "' ")
        stbCTE.AppendLine(" ), ")

    End Sub

    Private Sub GetAnalisiEntitaxTestata_CTE(ByRef stbCTE As StringBuilder,
                                             ApplicaVisibilitaUtente As Boolean,
                                             ApplicaVisibilitaUMA As Boolean,
                                             objParametriServer As AgronicaCoreParametri,
                                             objparametriUtenti As AgronicaCoreParametri)

        stbCTE.AppendLine(" AnalisiEntitaxTestata_CTE AS ( ")
        stbCTE.AppendLine(" SELECT DISTINCT ")
        stbCTE.AppendLine("     Analisi_Testata_Cod")
        stbCTE.AppendLine("   , Piva")
        stbCTE.AppendLine(" FROM Analisi_EntitaxTestata ")

        '--- Se l'utente ha visibilita limitata...
        If ApplicaVisibilitaUtente Then
            stbCTE.AppendLine(" INNER JOIN UtentiVisibilitaAppoggioCentriAziendali_CTE ON ")
            stbCTE.AppendLine("     UtentiVisibilitaAppoggioCentriAziendali_CTE.keyPiva = Piva ")
            stbCTE.AppendLine(" AND UtentiVisibilitaAppoggioCentriAziendali_CTE.keySaCod = Sa_Cod")
        ElseIf ApplicaVisibilitaUMA Then
            Dim FiltroUtente As Boolean = False
            Dim FiltroGruppo As Boolean = False
            Dim Gruppo As Integer = 0
            Dim VisibilitaTotale As Boolean = False

            Dim objUtenti_Visibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
            objUtenti_Visibilita.OttieniTipoFiltroVisibilita(objparametriUtenti, 0, FiltroUtente, FiltroGruppo, Gruppo, VisibilitaTotale)

            If Not VisibilitaTotale Then
                If FiltroUtente AndAlso Not FiltroGruppo Then
                    stbCTE.AppendLine(" LEFT JOIN " & objparametriUtenti.Recupera_NomeDB & ".dbo.Utenti_Visibilita visibilita ON Analisi_EntitaxTestata.Piva = visibilita.Piva_Azienda AND visibilita.Username = '" & objparametriUtenti.UtenteUsername & "' ")
                ElseIf Not FiltroUtente AndAlso FiltroGruppo Then
                    stbCTE.AppendLine(" LEFT JOIN " & objparametriUtenti.Recupera_NomeDB & ".dbo.Utenti_Visibilita visibilita ON Analisi_EntitaxTestata.Piva = visibilita.Piva_Azienda AND visibilita.Gruppo = " & Gruppo & " ")
                Else
                    stbCTE.AppendLine(" INNER JOIN " & objparametriUtenti.Recupera_NomeDB & ".dbo.Utenti_Visibilita visibilita ON Analisi_EntitaxTestata.Piva = visibilita.Piva_Azienda AND visibilita.Gruppo = -1 ")
                End If

                If Not VisibilitaTotale AndAlso ((FiltroUtente And Not FiltroGruppo) OrElse
                (Not FiltroUtente And FiltroGruppo)) Then
                    stbCTE.AppendLine(" WHERE (visibilita.Piva_Azienda IS NOT NULL) ")
                End If

            End If
        End If

        stbCTE.AppendLine(" ) ")
    End Sub

#End Region
End Class