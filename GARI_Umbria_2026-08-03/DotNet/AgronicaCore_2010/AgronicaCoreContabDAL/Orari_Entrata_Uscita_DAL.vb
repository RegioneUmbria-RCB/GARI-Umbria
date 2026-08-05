Imports System.Data.Entity
Imports AgronicaCoreDataProvider
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO
Imports Newtonsoft.Json.Linq


'
'#################################################################
'#################################################################
'#################################################################



Public Class Orari_Entrata_Uscita_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '########################################################################################################
    Private Sub CreaColonneDataTable_OrariEntrateUscite(ByRef dataTable_OrariEntrateUscite As DataTable,
                                                        ByVal validita_inizio As Date,
                                                        ByVal validita_fine As Date)

        Dim Data_Ora_Inizio As String
        Dim Data_Ora_Fine As String
        Dim Data_Ora_Tot As String

        Dim Giorni As Integer
        Dim Data As Date

        'Costruisce un oggetto DataTable con le colonne della tabella Orari_Entrata_Uscita

        dataTable_OrariEntrateUscite.Columns.Add(New DataColumn("Id_EntrateUscite", GetType(Integer)))
        dataTable_OrariEntrateUscite.Columns.Add(New DataColumn("Piva_Superuser", GetType(Integer)))
        dataTable_OrariEntrateUscite.Columns.Add(New DataColumn("USER", GetType(String)))
        dataTable_OrariEntrateUscite.Columns.Add(New DataColumn("Rag_Soc", GetType(String)))
        dataTable_OrariEntrateUscite.Columns.Add(New DataColumn("NRBADGE", GetType(String)))
        dataTable_OrariEntrateUscite.Columns.Add(New DataColumn("Data_Inserimento", GetType(Date)))
        dataTable_OrariEntrateUscite.Columns.Add(New DataColumn("OrigineApp_Des", GetType(Integer)))
        dataTable_OrariEntrateUscite.Columns.Add(New DataColumn("inviato", GetType(Integer)))
        dataTable_OrariEntrateUscite.Columns.Add(New DataColumn("datainvio", GetType(Date)))
        dataTable_OrariEntrateUscite.Columns.Add(New DataColumn("Data_Creazione", GetType(Date)))
        dataTable_OrariEntrateUscite.Columns.Add(New DataColumn("Data_Modifica", GetType(Date)))
        dataTable_OrariEntrateUscite.Columns.Add(New DataColumn("Username_Creazione", GetType(String)))
        dataTable_OrariEntrateUscite.Columns.Add(New DataColumn("Username_Modifica", GetType(String)))
        dataTable_OrariEntrateUscite.Columns.Add(New DataColumn("Validita_Inizio", GetType(Date)))
        dataTable_OrariEntrateUscite.Columns.Add(New DataColumn("Validita_Fine", GetType(Date)))
        dataTable_OrariEntrateUscite.Columns.Add(New DataColumn("APP_LOG_Eventi_ID_Inizio", GetType(String)))
        dataTable_OrariEntrateUscite.Columns.Add(New DataColumn("APP_LOG_Eventi_ID_Fine", GetType(String)))

        Giorni = DateDiff("d", validita_inizio, validita_fine)

        For i = 0 To Giorni

            Data = DateAdd(DateInterval.Day, i, Date.ParseExact(validita_inizio, "dd/MM/yyyy", System.Globalization.DateTimeFormatInfo.InvariantInfo))

            'Calcolo Campi Data_Ora
            Data_Ora_Inizio = "Data_Ora_Inizio" & Format(Data, "yyyyMMdd")
            Data_Ora_Fine = "Data_Ora_Fine" & Format(Data, "yyyyMMdd")
            Data_Ora_Tot = "Data_Ora_Tot" & Format(Data, "yyyyMMdd")

            dataTable_OrariEntrateUscite.Columns.Add(New DataColumn(Data_Ora_Inizio, GetType(Date)))
            dataTable_OrariEntrateUscite.Columns.Add(New DataColumn(Data_Ora_Fine, GetType(Date)))
            dataTable_OrariEntrateUscite.Columns.Add(New DataColumn(Data_Ora_Tot, GetType(Date)))

        Next

    End Sub


    '##############################################################################################

    Private Function BuildObject_OrariEntrateUscite(ByVal dr As DataRow, ByVal entrata As Boolean, ByVal fromDb As Boolean) As Orari_Entrata_Uscita

        'Costruisce un oggetto Orari_Entrate_Uscite con i campi della riga corrente
        'Il parametro entrata indica se il record passato come dataRow è un record di ENTRATA
        'fromDb indica se il record proviene da OrariEntrateUscite

        ' Set proprietà dell'oggetto Orari_Entrata_Uscita con i valori della riga corrente

        Dim myObj As New Orari_Entrata_Uscita With {
            .Piva_Superuser = dr.Item("Piva_Superuser"),
            .NOME = dr.Item("NOME"),
            .COGNOME = If(dr.Item("COGNOME").GetType = GetType(DBNull), " ", dr.Item("COGNOME")),
            .NRBADGE = dr.Item("NRBADGE"),
            .Data_Inserimento = Format(dr.Item("Data_Creazione"), "dd/MM/yyyy"),
            .Data_Creazione = dr.Item("Data_Creazione"),
            .Data_Modifica = dr.Item("Data_Modifica"),
            .Username_Creazione = dr.Item("Username_Creazione"),
            .Username_Modifica = dr.Item("Username_Modifica"),
            .Validita_Inizio = dr.Item("Validita_Inizio"),
            .Validita_Fine = dr.Item("Validita_Fine"),
            .OrigineApp = 1
        }

        If (entrata) Then

            If (fromDb) Then

                myObj.inviato = If(dr.Item("inviato").GetType <> GetType(DBNull), dr.Item("inviato"), Nothing)
                myObj.datainvio = If(dr.Item("datainvio").GetType <> GetType(DBNull), dr.Item("datainvio"), Nothing)
                myObj.USER = dr.Item("USER")
                myObj.Data_Ora_Inizio = dr.Item("Data_Ora_Inizio")
                myObj.Data_Ora_Fine = dr.Item("Data_Ora_Fine")
                myObj.App_Log_Eventi_ID_Inizio = dr.Item("APP_Log_Eventi_ID_Inizio")
                myObj.Id_EntrateUscite = dr.Item("Id_EntrateUscite")

            Else

                myObj.USER = dr.Item("UserName")
                Dim dataoraFine As DateTime = dr.Item("DataOraRilevata")
                Dim dataoraInizio As DateTime = dr.Item("DataOraRilevata")
                dataoraFine = dataoraFine.AddHours(23 - dataoraFine.Hour)
                dataoraFine = dataoraFine.AddMinutes(59 - dataoraFine.Minute)
                myObj.Data_Ora_Inizio = Format(dataoraInizio, "dd/MM/yyyy") & " " & Format(dataoraInizio.ToLocalTime, "HH:mm")
                myObj.Data_Ora_Fine = Format(dataoraFine, "dd/MM/yyyy") & " " & Format(dataoraFine.ToLocalTime, "HH:mm")
                myObj.Data_Ora_Inizio = myObj.Data_Ora_Inizio.AddHours(-2)                              'Aggiusto gli orari e date
                myObj.Data_Ora_Fine = myObj.Data_Ora_Fine.AddHours(-2).AddDays(1)
                myObj.App_Log_Eventi_ID_Inizio = dr.Item("ID")

            End If

        Else

            Dim dataoraInizio As DateTime = dr.Item("DataOraRilevata")
            Dim dataoraFine As DateTime = dr.Item("DataOraRilevata")
            myObj.USER = dr.Item("UserName")
            dataoraInizio = dataoraInizio.AddHours(-dataoraInizio.Hour)
            dataoraInizio = dataoraInizio.AddMinutes(-dataoraInizio.Minute)
            myObj.Data_Ora_Fine = Format(dataoraFine, "dd/MM/yyyy") & " " & Format(dataoraFine.ToLocalTime, "HH:mm")
            myObj.Data_Ora_Inizio = Format(dataoraInizio, "dd/MM/yyyy") & " " & Format(dataoraInizio.ToLocalTime, "HH:mm")
            myObj.Data_Ora_Inizio = myObj.Data_Ora_Inizio.AddHours(-2) '.AddDays(1)
            myObj.Data_Ora_Fine = myObj.Data_Ora_Fine.AddHours(-2)
            myObj.App_Log_Eventi_ID_Fine = dr.Item("ID")

        End If

        Return myObj

    End Function

    '#########################################################################################################

    Private Function BuildObject_APP_LogEventi(ByVal dr As DataRow)

        'Costruisce un oggetto APP_Log_Eventi con i dati del record fornito in input come dataRow e impostando datainvio a Date.Now

        Dim myObj As New APP_LogEventi With {
            .ID = dr.Item("ID"),
            .Piva_Superuser = dr.Item("Piva_Superuser"),
            .DataOraRilevata = dr.Item("DataOraRilevata"),
            .Evento = dr.Item("Evento"),
            .inviato = 1,
            .datainvio = Date.Now,
            .Data_Creazione = dr.Item("Data_Creazione"),
            .Data_Modifica = dr.Item("Data_Creazione"),
            .Username_Creazione = dr.Item("Username_Creazione"),
            .Username_Modifica = dr.Item("Username_Modifica"),
            .Validita_Inizio = dr.Item("Validita_Inizio"),
            .Validita_Fine = dr.Item("Validita_Fine"),
            .Importato_Data = If(dr.Item("Importato_Data").GetType <> GetType(DBNull), dr.Item("Importato_Data"), Nothing),
            .Importato_Errore = If(dr.Item("Importato_Errore").GetType <> GetType(DBNull), dr.Item("Importato_Errore"), Nothing),
            .NrBadge = dr.Item("NrBadge"),
            .Identif_Dispositivo = dr.Item("Identif_Dispositivo"),
            .IDTransazione = dr.Item("IDTransazione"),
            .Nome = dr.Item("Nome"),
            .Cognome = If(dr.Item("Cognome").GetType <> GetType(DBNull), dr.Item("Cognome"), Nothing)
        }

        Return myObj
    End Function

    '##########################################################################################################
    Public Function Ricerca_Orari_Entrate_Uscite(ByVal piva As String,
                                                 ByVal FiltroManodopera() As String,
                                                 ByVal Validita_Inizio As Date,
                                                 ByVal Validita_Fine As Date,
                                                 ByRef objParametri As AgronicaCoreParametri
                                                 ) As DataTable


        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Orari_Entrata_Uscita_DAL.Ricerca_Orari_Entrata_Uscita()"

        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
        Dim dt As DataTable
        Dim dt_finale As New DataTable
        Dim dr_finale As DataRow
        Dim select_list As String
        Dim PivaSuperUser = objParametri.PivaSuperUser

        Dim Data_Ora_Inizio As String
        Dim Data_Ora_Fine As String
        Dim Data_Ora_Tot As String

        Dim Ora As Integer
        Dim Minuti As Integer

        Validita_Fine = DateAdd(DateInterval.Day, 1, Validita_Fine)

        strSql.Length = 0

        select_list = "*"

        CreaColonneDataTable_OrariEntrateUscite(dt_finale, Validita_Inizio, Validita_Fine)

        strSql.Append(" Select " & select_list & " From Orari_Entrata_Uscita ")
        strSql.Append(" WHERE Orari_Entrata_Uscita.Data_Creazione <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
        strSql.Append(" And Orari_Entrata_Uscita.Data_Creazione >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")
        strSql.Append(" ORDER BY Orari_Entrata_Uscita.COGNOME, NOME, Data_Ora_Inizio, Data_Ora_Fine ") '.[USER], 

        '--------------------------------------------------------------------------
        dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)



        For Each dr As DataRow In dt.Rows

            'Calcolo Campi Data_Ora
            Data_Ora_Inizio = "Data_Ora_Inizio" & Format(dr.Item("Data_Inserimento"), "yyyyMMdd")
            Data_Ora_Fine = "Data_Ora_Fine" & Format(dr.Item("Data_Inserimento"), "yyyyMMdd")
            Data_Ora_Tot = "Data_Ora_Tot" & Format(dr.Item("Data_Inserimento"), "yyyyMMdd")



            'Creo una nuova riga
            dr_finale = dt_finale.NewRow

            dr_finale.Item("Id_EntrateUscite") = dr.Item("Id_EntrateUscite")
            dr_finale.Item("Piva_Superuser") = dr.Item("Piva_Superuser")
            dr_finale.Item("USER") = dr.Item("USER")
            dr_finale.Item("Rag_Soc") = dr.Item("COGNOME") & " " & dr.Item("NOME")
            dr_finale.Item("NRBADGE") = dr.Item("NRBADGE")
            dr_finale.Item("Data_Inserimento") = dr.Item("Data_Inserimento")
            dr_finale.Item(Data_Ora_Inizio) = dr.Item("Data_Ora_Inizio")
            dr_finale.Item(Data_Ora_Fine) = dr.Item("Data_Ora_Fine")
            dr_finale.Item("OrigineApp_Des") = dr.Item("OrigineApp")
            dr_finale.Item("inviato") = dr.Item("inviato")
            dr_finale.Item("datainvio") = dr.Item("datainvio")
            dr_finale.Item("Data_Creazione") = dr.Item("Data_Creazione")
            dr_finale.Item("Data_Modifica") = dr.Item("Data_Modifica")
            dr_finale.Item("Username_Creazione") = dr.Item("Username_Creazione")
            dr_finale.Item("Username_Modifica") = dr.Item("Username_Modifica")
            dr_finale.Item("Validita_Inizio") = dr.Item("Validita_Inizio")
            dr_finale.Item("Validita_Fine") = dr.Item("Validita_Fine")
            dr_finale.Item("APP_LOG_Eventi_ID_Inizio") = dr.Item("APP_LOG_Eventi_ID_Inizio")
            dr_finale.Item("APP_LOG_Eventi_ID_Fine") = dr.Item("APP_LOG_Eventi_ID_Fine")


            'Conversione intero minuti a data
            Ora = DateDiff("h", dr.Item("Data_Ora_Inizio"), dr.Item("Data_Ora_Fine"))
            Minuti = DateDiff("n", dr.Item("Data_Ora_Inizio"), dr.Item("Data_Ora_Fine")) Mod 60
            If (Ora < 0) Then
                Ora = -Ora
            End If
            If (Minuti < 0) Then
                Minuti = -Minuti
            End If
            dr_finale.Item(Data_Ora_Tot) = "01/01/1900 " & Ora.ToString("D2") & ":" & Minuti.ToString("D2")


            dt_finale.Rows.Add(dr_finale)



        Next

        Return dt_finale

    End Function

    '##########################################################################################################
    Public Function RicercaPerId_OrariEntrateUscite(ByVal elencoId As String,
                                                    ByVal righeInserite As JArray,
                                                    ByVal elencoVar As ArrayList,
                                                    ByVal validita_Inizio As Date,
                                                    ByVal validita_Fine As Date,
                                                    ByRef objParametri As AgronicaCoreParametri
                                                    ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Orari_Entrata_Uscita_DAL.RicercaPerId_OrariEntrateUscite()"

        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
        Dim dt As New DataTable
        Dim dt_finale As New DataTable
        Dim dr_finale As DataRow

        Dim Data_Ora_Inizio As String
        Dim Data_Ora_Fine As String
        Dim Data_Ora_Tot As String
        Dim Id_EntrateUscite_Data As String
        Dim modificato As Boolean

        CreaColonneDataTable_OrariEntrateUscite(dt_finale, validita_Inizio, validita_Fine)

        strSql.Length = 0

        strSql.Append(" Select * From Orari_Entrata_Uscita ")
        strSql.Append(" WHERE Id_EntrateUscite IN " & Agro_SQL_Save_Clausola_IN(elencoId) & " ")

        '--------------------------------------------------------------------------
        dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)

        For Each dr As DataRow In dt.Rows

            'Calcolo Campi Data_Ora e Id con data
            Data_Ora_Inizio = "Data_Ora_Inizio" & Format(dr.Item("Data_Inserimento"), "yyyyMMdd")
            Data_Ora_Fine = "Data_Ora_Fine" & Format(dr.Item("Data_Inserimento"), "yyyyMMdd")
            Data_Ora_Tot = "Data_Ora_Tot" & Format(dr.Item("Data_Inserimento"), "yyyyMMdd")
            Id_EntrateUscite_Data = "Id_EntrateUscite" & Format(dr.Item("Data_Inserimento"), "yyyyMMdd")



            'Creo una nuova riga
            dr_finale = dt_finale.NewRow

            dr_finale.Item("Id_EntrateUscite") = dr.Item("Id_EntrateUscite")

            'Controllo se il record è uno di quelli modificati dall'utente

            If (elencoVar.Contains(dr_finale.Item("Id_EntrateUscite"))) Then

                For Each riga As JObject In righeInserite
                    modificato = False
                    For Each campo In riga

                        'cerco tra i campi della riga presa dalla grid l'id relativo a questo record

                        If (campo.Key.StartsWith(Id_EntrateUscite_Data)) Then
                            If (CInt(campo.Value) = CInt(dr_finale.Item("Id_EntrateUscite"))) Then
                                modificato = True
                            End If
                        End If

                        'assegno ai campi data_ora i valori presi dalla grid

                        If modificato Then

                            If campo.Key.StartsWith(Data_Ora_Inizio) Then

                                dr_finale.Item(Data_Ora_Inizio) = campo.Value

                            ElseIf campo.Key.StartsWith(Data_Ora_Fine) Then

                                dr_finale.Item(Data_Ora_Fine) = campo.Value

                            ElseIf campo.Key.StartsWith(Data_Ora_Tot) Then

                                dr_finale.Item(Data_Ora_Tot) = campo.Value

                            End If

                        End If

                    Next

                Next


            End If

            dr_finale.Item("Piva_Superuser") = dr.Item("Piva_Superuser")
            dr_finale.Item("USER") = dr.Item("USER")
            dr_finale.Item("Rag_Soc") = dr.Item("COGNOME") + " " + dr.Item("NOME")
            dr_finale.Item("NRBADGE") = dr.Item("NRBADGE")
            dr_finale.Item("Data_Inserimento") = dr.Item("Data_Inserimento")
            dr_finale.Item("OrigineApp_Des") = dr.Item("OrigineApp")
            dr_finale.Item("inviato") = dr.Item("inviato")
            dr_finale.Item("datainvio") = dr.Item("datainvio")
            dr_finale.Item("Data_Creazione") = dr.Item("Data_Creazione")
            dr_finale.Item("Data_Modifica") = dr.Item("Data_Modifica")
            dr_finale.Item("Username_Creazione") = dr.Item("Username_Creazione")
            dr_finale.Item("Username_Modifica") = dr.Item("Username_Modifica")
            dr_finale.Item("Validita_Inizio") = dr.Item("Validita_Inizio")
            dr_finale.Item("Validita_Fine") = dr.Item("Validita_Fine")
            dr_finale.Item("APP_LOG_Eventi_ID_Fine") = dr.Item("APP_LOG_Eventi_ID_Fine")
            dr_finale.Item("APP_LOG_Eventi_ID_Inizio") = dr.Item("APP_LOG_Eventi_ID_Inizio")


            dt_finale.Rows.Add(dr_finale)


        Next

        Return dt_finale

    End Function

    '##############################################################################################
    Public Function Ricerca_Sintesi_Persone_TimeSheet(ByVal piva As String,
                                           ByVal Validita_Inizio As Date,
                                          ByVal Validita_Fine As Date,
                                          ByRef objParametri As AgronicaCoreParametri,
                                          ByRef objParametri_Utenti As AgronicaCoreParametri
                                          ) As DataTable


        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Orari_Entrata_Uscita_DAL_R.Ricerca_Sintesi_Persone_TimeSheet()"

        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
        Dim strJoin As New System.Text.StringBuilder
        Dim dt As DataTable
        Dim dt_finale As New DataTable
        Dim dr_finale As DataRow
        Dim PivaSuperUser = objParametri.PivaSuperUser
        Dim Data_Ora_Tot As String
        Dim Data_Ora_Tot_Str As String
        Dim Giorni As Integer
        Dim Data As Date

        Dim Ora As Integer
        Dim Minuti As Integer

        Dim NomeDB_Utenti As String = objParametri_Utenti.StringaConnessione.Split(";")(2).Split("=")(1)

        Validita_Fine = DateAdd(DateInterval.Day, 1, Validita_Fine)

        strSql.Length = 0
        strSql.Append(" Select * From ")
        strSql.Append(NomeDB_Utenti & ".dbo.Utenti ")
        strSql.Append("Join " & NomeDB_Utenti & ".dbo.Utenti_Dettagli On (" & NomeDB_Utenti & ".dbo.Utenti.UserName = " & NomeDB_Utenti & ".dbo.Utenti_Dettagli.UserName ) ")
        strSql.Append("Left Join Orari_Entrata_Uscita On ( " & NomeDB_Utenti & ".dbo.Utenti.UserName = Orari_Entrata_Uscita." & ControlChars.Quote & "USER" & ControlChars.Quote & " )")

        strSql.Append(" AND Orari_Entrata_Uscita.Data_Inserimento <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
        strSql.Append(" AND Orari_Entrata_Uscita.Data_Inserimento >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")
        strSql.Append(" ORDER BY " & NomeDB_Utenti & ".dbo.Utenti_Dettagli.Cognome ")

        dt_finale.Columns.Add(New DataColumn("UserName", GetType(String)))
        dt_finale.Columns.Add(New DataColumn("Rag_Soc", GetType(String)))
        dt_finale.Columns.Add(New DataColumn("Qualifica_Des", GetType(String)))
        dt_finale.Columns.Add(New DataColumn("Data_Inserimento", GetType(Date)))
        dt_finale.Columns.Add(New DataColumn("Totale_Riga", GetType(String)))
        dt_finale.Columns.Add(New DataColumn("Totale_Riga_Str", GetType(String)))

        Giorni = DateDiff("d", Validita_Inizio, Validita_Fine)

        For i = 0 To Giorni

            Data = DateAdd(DateInterval.Day, i, Date.ParseExact(Validita_Inizio, "dd/MM/yyyy", System.Globalization.DateTimeFormatInfo.InvariantInfo))

            'Calcolo Campi Data_Ora
            Data_Ora_Tot = "Data_Ora_Tot" & Format(Data, "yyyyMMdd")
            Data_Ora_Tot_Str = "Data_Ora_Tot_Str" & Format(Data, "yyyyMMdd")
            dt_finale.Columns.Add(New DataColumn(Data_Ora_Tot, GetType(String)))
            dt_finale.Columns.Add(New DataColumn(Data_Ora_Tot_Str, GetType(String)))

        Next


        '--------------------------------------------------------------------------
        dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
        '--------------------------------------------------------------------------

        For Each dr As DataRow In dt.Rows

            Dim giaPresente As Boolean = False
            Ora = 0
            Minuti = 0
            For Each rFinale As DataRow In dt_finale.Rows
                If rFinale.Item("UserName") = dr.Item("UserName") Then
                    dr_finale = rFinale
                    giaPresente = True
                    Exit For
                End If
            Next

            If Not giaPresente Then
                'Creo una nuova riga
                dr_finale = dt_finale.NewRow

                dr_finale.Item("UserName") = dr.Item("UserName")
                dr_finale.Item("Qualifica_Des") = dr.Item("UserNameCommerciale")
                dr_finale.Item("Rag_Soc") = dr.Item("Cognome") & " " & dr.Item("Nome")
            End If

            If (dr.Item("Data_Inserimento").GetType <> GetType(DBNull)) Then

                Data_Ora_Tot = "Data_Ora_Tot" & Format(dr.Item("Data_Inserimento"), "yyyyMMdd")
                Data_Ora_Tot_Str = "Data_Ora_Tot_Str" & Format(dr.Item("Data_Inserimento"), "yyyyMMdd")

                If dr_finale.Item(Data_Ora_Tot).GetType = GetType(DBNull) Then

                    Ora = DateDiff("h", dr.Item("Data_Ora_Inizio"), dr.Item("Data_Ora_Fine"))
                    Minuti = DateDiff("n", dr.Item("Data_Ora_Inizio"), dr.Item("Data_Ora_Fine")) Mod 60
                    dr_finale.Item(Data_Ora_Tot) = "01/01/1900 " & Ora.ToString("D2") & ":" & Minuti.ToString("D2")
                    dr_finale.Item(Data_Ora_Tot_Str) = Ora.ToString("D2") & ":" & Minuti.ToString("D2")

                Else

                    Ora = DateDiff("h", dr.Item("Data_Ora_Inizio"), dr.Item("Data_Ora_Fine"))
                    Minuti = DateDiff("n", dr.Item("Data_Ora_Inizio"), dr.Item("Data_Ora_Fine")) Mod 60
                    dr_finale.Item(Data_Ora_Tot) = DateAdd(DateInterval.Hour, Ora, dr_finale.Item(Data_Ora_Tot))
                    Dim suppDate As DateTime
                    suppDate = dr_finale.Item(Data_Ora_Tot)
                    dr_finale.Item(Data_Ora_Tot_Str) = suppDate.Hour.ToString("D2") & ":" & suppDate.Minute.ToString("D2")

                End If


            End If

            If (dr_finale.Item("Totale_Riga").GetType = GetType(DBNull)) Then

                dr_finale.Item("Totale_Riga") = "01/01/1900 " & Ora.ToString("D2") & ":" & Minuti.ToString("D2")
                dr_finale.Item("Totale_Riga_Str") = Ora.ToString("D2") & ":" & Minuti.ToString("D2")

            Else

                Dim suppDate As DateTime
                dr_finale.Item("Totale_Riga") = DateAdd(DateInterval.Hour, Ora, dr_finale.Item("Totale_Riga"))
                suppDate = dr_finale.Item("Totale_Riga")
                dr_finale.Item("Totale_Riga_Str") = suppDate.Hour.ToString("D2") & ":" & suppDate.Minute.ToString("D2")

            End If

            If Not giaPresente AndAlso String.Compare(dr_finale.Item("Qualifica_Des"), "superuser") <> 0 Then
                dt_finale.Rows.Add(dr_finale)
            End If
        Next

        Return dt_finale

    End Function


    '##############################################################################################

    Public Function Import_Orari_Entrata_Uscita(ByVal piva As String,
                                                ByRef objParametri As AgronicaCoreParametri,
                                                ByRef objParametri_Utenti As AgronicaCoreParametri
                                                ) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Orari_Entrata_Uscita_DAL_R.Import_Orari_Entrata_Uscita()"
        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
        Dim dt As DataTable
        Dim dr As DataRow
        Dim EFInsert As New ArrayList           'Record da inserire in OrariEntrateUscite
        Dim EFUpdate As New ArrayList           'Record da aggiornare in OrariEntrateUscite
        Dim EFUpdateAPP As New ArrayList        'Record da aggiornare in APP_Log_Eventi
        Dim HtBadge As New Hashtable            'HashTable che contiene il numero badge dei record ENTRATA incontrati in questa chiamata
        Dim EntrataFromDb As Boolean = False    'Flag per segnalare che il record di ENTRATA proviene dalla tabella OrariEntrateUscite e dovrà essere aggiornato
        Dim gefutils As New Gias_EF_Utility
        Dim res As Boolean = True               'Risultato della chiamata
        Dim myObj As New Orari_Entrata_Uscita

        Dim NomeDB_Utenti As String = objParametri_Utenti.StringaConnessione.Split(";")(2).Split("=")(1)

        Dim selectList As String = "ID ,Piva_Superuser ,DataOraRilevata ,Evento"
        selectList += "  ,inviato ,datainvio ,APP_LogEventi.Data_Creazione ,APP_LogEventi.Data_Modifica"
        selectList += "                ,Username_Creazione ,Username_Modifica ,Validita_Inizio ,Validita_Fine"
        selectList += "               ,Importato_Data ,Importato_Errore ,NrBadge ,Identif_Dispositivo ,IDTransazione"
        selectList += "              ,APP_LogEventi.Nome ,APP_LogEventi.Cognome ,Utenti_Dettagli.UserName"

        strSql.Append("SELECT " & selectList & " FROM APP_LogEventi ")
        strSql.Append("JOIN " & NomeDB_Utenti & ".dbo.Utenti_Dettagli ON Username_Creazione = CodFisc ")
        strSql.Append("WHERE APP_LogEventi.datainvio IS NULL ")
        strSql.Append("ORDER BY APP_LogEventi.NrBadge, DataOraRilevata")

        dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)

        For Each dr In dt.Rows

            Dim updateAPP As APP_LogEventi = BuildObject_APP_LogEventi(dr)      'Aggiorno nel record in APP_Log_Eventi la datainvio e invio
            EFUpdateAPP.Add(updateAPP)

            If (String.Compare(dr.Item("EVENTO"), "ENTRATA") = 0) Then

                myObj = BuildObject_OrariEntrateUscite(dr, True, False)         'Creo un oggetto OrariEntrateUscite con i campi del record di APP_Log_Eventi

                If (HtBadge.Contains(myObj.NRBADGE)) Then                       'Controllo se c'è già un'entrata con lo stesso badge incontrata in questa chiamata

                    If HtBadge(myObj.NRBADGE).Data_Ora_Inizio < myObj.Data_Ora_Inizio Then
                        'Il lavoratore ha disinstallato e reinstallato l'app, mantengo nella hash table solo il record più recente

                        Dim myObjSupp As Orari_Entrata_Uscita
                        myObjSupp = myObj
                        myObj = HtBadge(myObj.NRBADGE)
                        HtBadge(myObj.NRBADGE) = myObjSupp

                    End If

                    EFInsert.Add(myObj)                                         'Quello meno recente lo aggiungo al db 

                Else

                    HtBadge.Add(myObj.NRBADGE, myObj)

                End If


            ElseIf (String.Compare(dr.Item("EVENTO"), "USCITA") = 0) Then

                Dim dataFine As DateTime = dr.Item("DataOraRilevata")

                If (HtBadge.Contains(dr.Item("NrBadge"))) Then                  'Se in questa chiamata ho già trovato il record di ENTRATA corrispondente, recupero l'oggetto

                    myObj = HtBadge(dr.Item("NrBadge"))
                    EntrataFromDb = False

                Else

                    EntrataFromDb = True                                                  'Altrimenti lo recupero dalla tabella OrariEntrateUscite
                    Dim strSqlOEU As New System.Text.StringBuilder
                    strSqlOEU.Append("SELECT TOP(1) * From Orari_Entrata_Uscita ")
                    strSqlOEU.Append("WHERE NRBADGE = '" & dr.Item("NrBadge") & "' AND App_Log_Eventi_ID_Fine IS NULL ")
                    strSqlOEU.Append("ORDER BY Orari_Entrata_Uscita.Data_Ora_Inizio DESC")

                    Dim dts As DataTable = EseguiQuery_Lettura(objParametri, strSqlOEU.ToString, nomeRoutine & " NRBADGE")
                    Dim drs As DataRow

                    For Each drs In dts.Rows
                        myObj = BuildObject_OrariEntrateUscite(drs, True, True)
                    Next

                End If

                If (myObj.NRBADGE <> Nothing) Then                                          'Controllo se effettivamente ho recuperato il record corrispondente

                    If (myObj.Data_Ora_Inizio.Date = dataFine.Date) Then                    'Controllo le date per decidere se dividere il record in due 

                        myObj.Data_Ora_Fine = dr.Item("DataOraRilevata")
                        myObj.App_Log_Eventi_ID_Fine = dr.Item("ID")
                        If Not EntrataFromDb Then                                                     'Se il record è stato recuperato dal db devo aggiornare, altrimenti devo aggiungerlo
                            EFInsert.Add(myObj)
                            HtBadge.Remove(myObj.NRBADGE)                                   'E tolgo il record dalla hash table
                        Else
                            EFUpdate.Add(myObj)
                        End If

                    Else

                        Dim myObj2 As Orari_Entrata_Uscita = BuildObject_OrariEntrateUscite(dr, False, False)
                        'Altrimenti creo un record duale di Uscita nella tabella OrariEntrataUscita
                        If Not EntrataFromDb Then
                            EFInsert.Add(myObj)
                            HtBadge.Remove(myObj.NRBADGE)
                        End If

                        EFInsert.Add(myObj2)

                    End If

                Else            'Se non ho recuparto il record di ENTRATA, eccezione

                    Throw New Exception("[" & nomeRoutine & "] : Il record con ID = " & dr.Item("ID") & " non ha un corrispondente record di ENTRATA")

                End If

            End If

        Next

        For Each obj As Orari_Entrata_Uscita In HtBadge.Values              'Aggiungo alla lista Insert tutti i record di ENTRATA rimasti nella hashTable (Ossia che non hanno un corrispettivo di USCITA in questa chiamata)
            EFInsert.Add(obj)
        Next

        Try


            Using GiasContext As New Gias_DeveloperServer_Entities(gefutils.GetEntityConnectionString(objParametri.StringaConnessione))

                'Rendo effettive le modifiche sulle varie tabelle

                For Each OEntrateUscite As Orari_Entrata_Uscita In EFInsert
                    GiasContext.Orari_Entrata_Uscita.Add(OEntrateUscite)
                Next


                For Each OEntrateUscite As Orari_Entrata_Uscita In EFUpdate
                    GiasContext.Orari_Entrata_Uscita.Attach(OEntrateUscite)
                    GiasContext.Entry(OEntrateUscite).State = EntityState.Modified
                Next

                For Each updateAPP As APP_LogEventi In EFUpdateAPP
                    GiasContext.APP_LogEventi.Attach(updateAPP)
                    GiasContext.Entry(updateAPP).State = EntityState.Modified
                Next

                ' COMMIT Effettivo
                GiasContext.SaveChanges()


            End Using

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
            res = False
        End Try

        Return res

    End Function

    '##########################################################################################################
    Public Function LeggiUltimaTimbratura(ByVal utente As String, ByRef objParametri As AgronicaCoreParametri) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Orari_Entrata_Uscita_DAL.LeggiUltimaTimbratura()"

        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
        Dim dt As New DataTable

        strSql.Append(" SELECT TOP 1 * From APP_LogEventi ")
        strSql.Append(" WHERE Username_Creazione = '" & utente & "' ")
        strSql.Append(" ORDER BY DataOraRilevata DESC ")

        '--------------------------------------------------------------------------
        dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)

        Return dt

    End Function

End Class


Public Class Orari_Entrata_Uscita_W
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################


    Public Function Aggiorna_Entrate_Uscite(ByVal piva As String,
                                            ByVal EFArrayToUpdate As ArrayList,
                                            ByVal EFArrayToDelete As ArrayList,
                                            ByRef objParametri As AgronicaCoreParametri
                                            ) As String

        Const nomeRoutine = "AgronicaCoreContabDAL.Orari_Entrata_Uscita_W.Aggiorna_Entrate_Uscite()"
        Dim messaggioErrore As String = ""
        Dim risultato As Integer = 0

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Try


            Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

                For Each OEntrateUscite As Orari_Entrata_Uscita In EFArrayToUpdate
                    GiasContext.Orari_Entrata_Uscita.Attach(OEntrateUscite)
                    GiasContext.Entry(OEntrateUscite).State = EntityState.Modified
                Next

                For Each OEntrateUscite As Orari_Entrata_Uscita In EFArrayToDelete
                    GiasContext.Orari_Entrata_Uscita.Attach(OEntrateUscite)
                    GiasContext.Orari_Entrata_Uscita.Remove(OEntrateUscite)
                Next

                ' COMMIT Effettivo
                GiasContext.SaveChanges()


            End Using


        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
            risultato = -1
        End Try

        Return risultato

    End Function



End Class

