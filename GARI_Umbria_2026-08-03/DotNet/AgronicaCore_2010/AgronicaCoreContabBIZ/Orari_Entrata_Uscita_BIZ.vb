Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreContabDAL
Imports AgronicaCoreEntityFramework_POCO
Imports Newtonsoft.Json.Linq
Imports System.Transactions
Imports AgronicaCoreDataProvider

Public Class Obj_OEU_DaInserire
    ' valutare se tenere questa classe
    Public Property Id_EntrateUscite As Integer
    Public Property Piva_Superuser As String
    Public Property USER As String
    Public Property NOME As String
    Public Property COGNOME As String
    Public Property NRBADGE As Integer
    Public Property Inserimento As DateTime
    Public Property DtInizio As DateTime
    Public Property DtFine As DateTime
    Public Property OreTot As Double
    Public Property OrigineApp As Boolean
    Public Property inviato As Boolean
    Public Property datainvio As Date
    Public Property Data_Creazione As DateTime
    Public Property Data_Modifica As DateTime
    Public Property Username_Creazione As String
    Public Property Username_Modifica As String
    Public Property Validita_Inizio As DateTime
    Public Property Validita_Fine As DateTime
    Public Property APP_LOG_Eventi As Boolean
    Public Property dtMov As Date
    Public Property dtStr As String

End Class



Public Class OrariEntrataUscitaBIZ
    Inherits AgronicaCoreDataProvider.DataProvider

    Protected Function BuildObject_OrariEntrateUscite(ByVal objRiga As JObject, ByVal Dtstr As String, ByVal aggr As Boolean) As Orari_Entrata_Uscita

        'Costruisce un oggetto Orari_Entrate_Uscite con i campi della riga corrente

        Dim myObj As New Orari_Entrata_Uscita
        Dim data = New Date(Dtstr.Substring(0, 4), Dtstr.Substring(4, 2), Dtstr.Substring(6, 2))
        Dim dtOraInizioStr As String = "Data_Ora_Inizio" + Dtstr
        Dim dtOraFineStr As String = "Data_Ora_Fine" + Dtstr
        Dim dtOraTotStr As String = "Data_Ora_Tot" + Dtstr
        Dim ragsoc As String = objRiga("Rag_Soc")
        Dim nome As String = ragsoc.Split(" ")(1)
        Dim cognome As String = ragsoc.Split(" ")(0)


        If objRiga(dtOraInizioStr).ToString <> "" Then

            If (aggr) Then

                Dim dtIdStr As String = "Id_EntrateUscite" + Dtstr
                myObj.Id_EntrateUscite = UtilityProvider.Agro_SQL_SaveNum(objRiga(dtIdStr))

            Else

                myObj.Id_EntrateUscite = UtilityProvider.Agro_SQL_SaveNum(objRiga("Id_EntrateUscite"))

            End If

            myObj.OrigineApp = Val(objRiga("OrigineApp_Des"))

            ' Set proprietà dell'oggetto Orari_Entrata_Uscita con i valori della riga corrente
            myObj.Piva_Superuser = objRiga("Piva_Superuser")
            myObj.USER = objRiga("USER")
            myObj.NOME = nome
            myObj.COGNOME = cognome
            myObj.NRBADGE = Val(objRiga("NRBADGE"))
            myObj.Data_Inserimento = Format(data, "dd/MM/yyyy") & " " & Format(CDate(objRiga("Data_Inserimento")).ToLocalTime, "HH:mm")
            myObj.Data_Ora_Inizio = Format(data, "dd/MM/yyyy") & " " & Format(CDate(objRiga(dtOraInizioStr)).ToLocalTime, "HH:mm")
            myObj.Data_Ora_Fine = Format(data, "dd/MM/yyyy") & " " & Format(CDate(objRiga(dtOraFineStr)).ToLocalTime, "HH:mm")
            myObj.inviato = If(objRiga("inviato").GetType <> GetType(DBNull), objRiga("inviato"), Nothing)
            myObj.datainvio = If(objRiga("datainvio").GetType <> GetType(DBNull), objRiga("datainvio"), Nothing)
            myObj.Data_Creazione = objRiga("Data_Creazione")
            myObj.Data_Modifica = objRiga("Data_Modifica")
            myObj.Username_Creazione = objRiga("Username_Creazione")
            myObj.Username_Modifica = objRiga("Username_Modifica")
            myObj.Validita_Inizio = objRiga("Validita_Inizio")
            myObj.Validita_Fine = objRiga("Validita_Fine")
            myObj.App_Log_Eventi_ID_Inizio = objRiga("App_Log_Eventi_ID_Inizio")
            myObj.App_Log_Eventi_ID_Fine = objRiga("App_Log_Eventi_ID_Fine")
        End If

        Return myObj

    End Function

    Public Function Aggiorna_Entrate_Uscite(ByVal Piva As String,
                                         ByVal righeInseriteGrid As String, ByVal elencoVariati As String, ByVal righeCancellateGrid As String,
                                         ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
        ) As Integer

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        Dim MessaggioErrore As String = String.Empty
        Dim NomeRoutine As String = "AgronicaCoreContabBIZ.Orari_Entrata_Uscita_BIZ.Aggiorna_Entrate_Uscite()"
        Dim EntrataUscita_W As New Orari_Entrata_Uscita_W

        Dim bOk As Boolean = False
        Dim righeArray_Entrata_Uscita As JArray
        Dim elencoVariatiJSON As JArray
        Dim OreTot As TimeSpan


        Dim EFArrayToUpdate As New ArrayList
        Dim EFArrayToDelete As New ArrayList

        Dim Id_EntrateUscite As Integer = 0
        Dim Risultato As Integer = 0

        Dim Data_Ora_Inizio As Date
        Dim Data_Ora_Fine As Date

        bOk = False
        elencoVariatiJSON = JArray.Parse(elencoVariati)

        'Hash table contenente le righe della grid
        Dim ht As New Hashtable

        'Hash table costruita con Id_Entrate_Uscite : Data
        Dim dht As New Hashtable

        Try

            Dim transactionOptions = New TransactionOptions()
            transactionOptions.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted
            Using scope As New TransactionScope(TransactionScopeOption.Required, transactionOptions)

                '###########################################################################################################################################
                '################################### NUOVI INSERIMENTI/MODIFICHE ###########################################################################
                '###########################################################################################################################################

                If righeInseriteGrid <> "" And righeInseriteGrid <> "[]" Then
                    righeArray_Entrata_Uscita = JArray.Parse(righeInseriteGrid)
                    bOk = True

                End If

                Dim w_counter = 0

                If bOk Then

                    For Each objRiga As JObject In righeArray_Entrata_Uscita

                        ' necessario perchè se inserisco due nuove righe con stessi dati terrebbe solo la prima
                        w_counter += 1

                        For Each CampoDellaRiga In objRiga

                            If CampoDellaRiga.Key.StartsWith("Data_Ora_Tot") Then

                                If Trim(CampoDellaRiga.Value) <> "" Then

                                    Dim DtStr = CampoDellaRiga.Key.Replace("Data_Ora_Tot", "")

                                    'Controllo valorizzazione giorno
                                    If Format(CDate(CampoDellaRiga.Value).ToLocalTime, "HH:mm") <> "00:00" Or UtilityProvider.Agro_SQL_SaveNum(objRiga("Id_EntrateUscite")) <> 0 Then

                                        Dim myKey = DtStr + "_" + CStr(Val(w_counter))

                                        If Not ht.ContainsKey(myKey) Then

                                            Dim myObj = BuildObject_OrariEntrateUscite(objRiga, DtStr, False)

                                            ht.Add(myKey, myObj)

                                            dht.Add(myObj.Id_EntrateUscite, DtStr)



                                        End If

                                    End If

                                End If

                            End If

                        Next
                    Next

                    EFArrayToUpdate.Clear()

                    For Each obj As DictionaryEntry In ht
                        Id_EntrateUscite = obj.Value.Id_EntrateUscite
                        Data_Ora_Inizio = obj.Value.Data_Ora_Inizio
                        Data_Ora_Fine = obj.Value.Data_Ora_Fine
                        OreTot = Data_Ora_Fine - Data_Ora_Inizio


                        '###########################################################################################################################################
                        '####################################### MODIFICHE #########################################################################################
                        '###########################################################################################################################################

                        EFArrayToUpdate.Add(obj.Value)

                    Next


                End If



                '###########################################################################################################################################
                '####################################### CANCELLAZIONI #####################################################################################
                '###########################################################################################################################################

                If righeCancellateGrid <> "" And righeCancellateGrid <> "[]" Then
                    righeArray_Entrata_Uscita = JArray.Parse(righeCancellateGrid)
                    bOk = True
                    ht.Clear()


                    For Each objRiga As JObject In righeArray_Entrata_Uscita
                        For Each CampoDellaRiga In objRiga

                            If CampoDellaRiga.Key.StartsWith("Id_EntrateUscite") Then 'And CampoDellaRiga.Value IsNot Nothing 

                                Dim DtStr = CampoDellaRiga.Key.Replace("Id_EntrateUscite", "")


                                Dim myObj = BuildObject_OrariEntrateUscite(objRiga, DtStr, True)


                                Dim myKey = myObj.Id_EntrateUscite

                                If (myKey <> 0) Then

                                    ht.Add(myKey, myObj)

                                End If

                            End If
                        Next
                    Next

                    EFArrayToDelete.Clear()

                    For Each obj As DictionaryEntry In ht

                        EFArrayToDelete.Add(obj.Value)

                    Next

                    'Pulizia JArray
                    righeArray_Entrata_Uscita.Clear()



                End If

                Risultato = EntrataUscita_W.Aggiorna_Entrate_Uscite(objParametri.PivaSuperUser, EFArrayToUpdate, EFArrayToDelete, objParametri)

                If (String.IsNullOrEmpty(MessaggioErrore)) Then
                    ' COMMIT Effettivo
                    scope.Complete()
                Else
                    ' Rollback
                    scope.Dispose()
                End If


            End Using



        Catch ex As Exception

            MessaggioErrore = "[" & NomeRoutine & "] : " & ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception(MessaggioErrore)


        End Try


        If Not String.IsNullOrEmpty(MessaggioErrore) Then
            Throw New Exception(MessaggioErrore)
        End If

        Return Risultato


    End Function


End Class
