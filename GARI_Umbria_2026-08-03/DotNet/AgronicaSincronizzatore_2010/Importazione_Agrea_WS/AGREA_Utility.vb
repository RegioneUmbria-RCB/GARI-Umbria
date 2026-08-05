Imports System.IO
Imports AgronicaCoreAgroFascicoloBIZ
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreUtility
Imports Newtonsoft.Json.Linq

Public Class AGREA_Utility

    Public Function CaricaSchede_WS_AgroFascicolo_AGREA(ByRef ErrCOD As Integer,
                                                        ByRef ErrMsg As String,
                                                        ByVal CUAA As String,
                                                        ByRef objParametri_Server As AgronicaCoreParametri) As String

        Dim importFascicolo As New AgroFascicolo

        Dim objConfSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
        Dim url = objConfSiti.Leggi_Valore(16, "AgroFascicolo_WS_AGREA", "", "", objParametri_Server)

        Dim strXML = importFascicolo.GetSchede(objParametri_Server.PivaSuperUser,
                                                          objParametri_Server.UtenteUsername,
                                                          objParametri_Server.UtenteCodFiscale,
                                                          20,
                                                          CUAA,
                                                          "",
                                                          objParametri_Server,
                                                          ErrCOD,
                                                          ErrMsg,
                                                          url)

        Return strXML

    End Function


    Public Function Leggi_Dati_Fascicolo_AGREA(ByVal Cuaa As String, Piva As String, ByVal Num_Scheda As String, ByVal OrigineOpr As String,
                                                ByRef strErr As String,
                                                ByRef objParametri_Server As AgronicaCoreParametri) As String

        Dim MsgOK As String = ""
        Dim objLog As New AgronicaCoreDataProvider.LogProvider
        Dim NomeRoutine As String = "Carica Dati Scheda"

        Try

            Dim objImpresa As New AgronicaCoreAnagrafeDAL.Imprese_Codici_Read
            Dim Allegati_Documenti_R As New AgronicaCoreAnagrafeDAL.Allegati_Documenti_R
            If Piva = "" Then
                Piva = objImpresa.Piva_from_Codice_Like(Cuaa, enum_CodiciAnagrafe.CodiceCUAA, objParametri_Server)
            End If
            Dim strXML = ""
            Dim esisteFascicolo = False
            If Piva <> "" Then
                Dim Allegati_Documenti_Cod As Integer
                If Allegati_Documenti_R.EsisteDocumento_Da_Numero(Num_Scheda, Piva, 0, Allegati_Documenti_Cod, objParametri_Server, 0) Then
                    strXML = Allegati_Documenti_R.LeggiXML(Allegati_Documenti_Cod, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server, Piva)
                    esisteFascicolo = True
                End If
            End If


            If esisteFascicolo AndAlso strXML <> "" Then

                Return strXML

            Else
                MsgOK = "Web Service Fascicolo: Invio Richiesta Scheda"
                MsgOK = "CUAA:" & Cuaa & ", Scheda N° " & Num_Scheda & " -  " & MsgOK
                objLog.Scrivi_LOG(objParametri_Server,
                                    NomeRoutine,
                                    MsgOK)

                'CHIAMATA A WS
                Dim errCOD As Integer
                Dim errMsg As String = ""
                strXML = CaricaDati_WS_AgroFascicolo_AGREA(errCOD, errMsg, Cuaa, Num_Scheda, objParametri_Server)

                MsgOK = "Web Service Fascicolo: Risposta Scheda Ricevuta"
                MsgOK = "CUAA:" & Cuaa & ", Scheda N° " & Num_Scheda & " -  " & MsgOK
                objLog.Scrivi_LOG(objParametri_Server,
                             NomeRoutine,
                             MsgOK)

                'RESTITUISCO IL FASCICOLO
                Return strXML

            End If

        Catch ex As Exception

            MsgOK = "Web Service Fascicolo Scheda ha restituito l'errore: " & ex.Message & " !"
            MsgOK = "CUAA:" & Cuaa & ", Scheda N° " & Num_Scheda & " -  " & MsgOK
            objLog.Scrivi_LOG(objParametri_Server,
                              NomeRoutine,
                              MsgOK)

            strErr = MsgOK
            Return Nothing

        End Try

        Return Nothing

    End Function

    Public Function FascicoloLeggiWSMemorizza_AGREA(
        ByVal LeggiUltimaValidazioneDisponibile As Boolean,
        ByRef Fonte_Cod As Integer,
        ByVal objParametri_Server As AgronicaCoreParametri,
        ByRef Fascicolo As String,
        ByRef messaggioWS As String,
        ByRef statoWS As Boolean,
        ByRef strErr As String,
        ByVal Piva As String,
        ByVal Cuaa As String,
        ByRef Num_Scheda As String,
        ByRef DataValidazione As Date,
        ByRef OrigineOpr As String,
        ByRef allegatoDocumentocod As Integer,
        ByRef xml_Response As String
    ) As Boolean

        Dim esisteFascicolo As Boolean = False

        'verifico se esiste un fascicolo
        If LeggiUltimaValidazioneDisponibile Then
            Leggi_Fascicoli_UltimaValidazione_AGREA(Cuaa, Fonte_Cod, Num_Scheda, DataValidazione, OrigineOpr, strErr, objParametri_Server)
        End If


        If strErr <> "" Then
            messaggioWS = "Impossibile verificare l'esistenza del fascicolo: " & strErr
            statoWS = False
        End If

        'leggo i dati del fascicolo
        If Not String.IsNullOrEmpty(Num_Scheda) Then

            xml_Response = Leggi_Dati_Fascicolo_AGREA(Cuaa, Piva, Num_Scheda, OrigineOpr, strErr, objParametri_Server)

            If strErr <> "" OrElse xml_Response Is Nothing OrElse xml_Response = "" Then
                'Riesco a recuperare le schede ma il fascicolo è vuoto!
                'messaggioWS = "Impossibile leggere il fascicolo: " & strErr
                messaggioWS &= "Nessun fascicolo disponibile per il cuaa: " & Cuaa & ". Potete compilare ugualmente i dati aziendali, oppure contattate il vostro CAA di riferimento per assitenza. <br />"
                'statoWS = False
            End If

            Dim obj_fascicolo As New Importazione_Agrea_WS.pc.common.webservice.sop.agrea.it.ISWSResponse

            If xml_Response <> "" Then
                XMLUtility.getObjectFromResponse(xml_Response, obj_fascicolo, Nothing, False)
            End If

            Dim Codice_Detentore = obj_fascicolo.domanda.idCaa

            'salvo il fascicolo originale
            If xml_Response IsNot Nothing AndAlso xml_Response <> "" Then

                Fonte_Cod = 20
                esisteFascicolo = FascicoloMemorizza_AGREA(Fonte_Cod, Piva, DataValidazione, Num_Scheda, xml_Response, objParametri_Server, allegatoDocumentocod, Codice_Detentore)

            End If

            Fascicolo = xml_Response

        Else

            messaggioWS &= "Nessun fascicolo disponibile per il cuaa: " & Cuaa & ". Potete compilare ugualmente i dati aziendali, oppure contattate il vostro CAA di riferimento per assitenza. <br />"
            messaggioWS = "Compila i dati aziendali"

        End If

        Return esisteFascicolo

    End Function

    Public Function FascicoloMemorizza_AGREA(ByVal Fonte_Cod As enum_Planning_Fonte, ByVal Piva As String, ByVal dataValidazione As DateTime, ByVal numeroValidazione As String, xml As String, objParametriServer As AgronicaCoreDataProvider.AgronicaCoreParametri, ByRef allegatoDocumentocod As Integer, ByRef Codice_Detentore As String) As Boolean

        Dim fascicoloAllegatiLettura As New AgronicaCoreAnagrafeDAL.Allegati_Documenti_R
        Dim fascicoloAllegatiScrittura As New AgronicaCoreAnagrafeDAL.Allegati_Documenti_W

        Dim esisteFascicolo As Boolean

        esisteFascicolo = fascicoloAllegatiLettura.EsisteDocumento_Da_Numero(numeroValidazione, Piva, enum_CategorieDocumenti.DomandaFascicolo, allegatoDocumentocod, objParametriServer, 4)

        Dim strFascicoli As String = xml

        If Not esisteFascicolo Then

            fascicoloAllegatiScrittura.Scrivi(Piva, "", enum_CategorieDocumenti.DomandaFascicolo, "", numeroValidazione, Fonte_Cod, "", AGRODATAINIZIO, AGRODATAFINE, allegatoDocumentocod, objParametriServer, dataValidazione, strXml:=strFascicoli, Codice_Detentore:=Codice_Detentore)

        Else

            fascicoloAllegatiScrittura.Modifica_FascicoloXML(allegatoDocumentocod, Piva, Fonte_Cod, strFascicoli, objParametriServer)

        End If

        Return esisteFascicolo

    End Function

    Public Sub Leggi_Fascicoli_UltimaValidazione_AGREA(ByVal Cuaa As String, ByVal Fonte_Cod As enum_Planning_Fonte,
                                                    ByRef Num_Scheda As String, ByRef DataValidazione As DateTime, ByRef OrigineOpr As String,
                                                    ByRef strErr As String,
                                                    ByRef objParametri_Server As AgronicaCoreParametri)

        'Dim DataDa As Integer = CInt(AgronicaCoreDataProvider.Conversioni.DateTime_To_DataintYYYYMMGG(AGRODATAINIZIO))
        'Dim DataA As Integer = CInt(AgronicaCoreDataProvider.Conversioni.DateTime_To_DataintYYYYMMGG(AGRODATAFINE))

        Dim NomeRoutine As String = "Cerca Schede"
        Dim MsgOK As String = ""

        Dim objLog As New AgronicaCoreDataProvider.LogProvider

        Dim RispostaWSSchedeFascicolo As Boolean = False
        Dim schede As String
        Try

            MsgOK = "Web Service Fascicolo: Invio Richiesta Schede"
            MsgOK = " CUAA:" & Cuaa & "  -  " & MsgOK

            'CHIAMATA A WS SCHEDE
            Dim errCOD As Integer
            Dim errMsg As String = ""
            schede = CaricaSchede_WS_AgroFascicolo_AGREA(errCOD, errMsg, Cuaa, objParametri_Server)

            MsgOK = "Il Web Service Fascicolo Schede ha risposto!"

            RispostaWSSchedeFascicolo = True

        Catch ex As Exception

            RispostaWSSchedeFascicolo = False

            Dim strEr As String
            strEr = ex.Message

            MsgOK = "Il Web Service Fascicolo Schede ha restituito l'errore :" & ex.Message & ""
            objLog.Scrivi_LOG(objParametri_Server,
                       NomeRoutine,
                       MsgOK)

            strErr &= "Impossibile connettersi al Web Service Fascicolo Schede!" '& ex.Message

        End Try

        Dim N_Schede As Integer = 0

        Dim DT_Schede As New DataTable
        DT_Schede.Columns.Add(New DataColumn("Num_Scheda", GetType(String)))
        DT_Schede.Columns.Add(New DataColumn("DataValidazione", GetType(Date)))
        DT_Schede.Columns.Add(New DataColumn("OrigineOpr", GetType(String)))


        If schede IsNot Nothing AndAlso schede <> "" Then

            Dim obj_Schede = JArray.Parse(schede)

            N_Schede = obj_Schede.Count

            For Each obj_scheda In obj_Schede

                Dim DrScheda = DT_Schede.NewRow

                Num_Scheda = obj_scheda("Numero_Validazione")

                DataValidazione = obj_scheda("Data_Validazione")

                OrigineOpr = obj_scheda("Ente_Des")

                DrScheda.Item("Num_Scheda") = Num_Scheda
                DrScheda.Item("DataValidazione") = DataValidazione
                DrScheda.Item("OrigineOpr") = OrigineOpr
                DT_Schede.Rows.Add(DrScheda)
            Next

            DT_Schede.DefaultView.Sort = " DataValidazione DESC "

            DataValidazione = DT_Schede.DefaultView.Item(0).Row.Item("DataValidazione")
            Num_Scheda = DT_Schede.DefaultView.Item(0).Row.Item("Num_Scheda")
            OrigineOpr = DT_Schede.DefaultView.Item(0).Row.Item("OrigineOpr")

        End If

        MsgOK = "Web Service Fascicolo: Risposta Schede Ricevuta"
        MsgOK = " CUAA:" & Cuaa & ", Ricevute " & N_Schede & " Schede di validazione. Presenti già in archivio " & N_Schede & " Schede."
        objLog.Scrivi_LOG(objParametri_Server,
                   NomeRoutine,
                   MsgOK)


    End Sub

    Public Function CaricaDati_WS_AgroFascicolo_AGREA(ByRef ErrCOD As Integer,
                                                        ByRef ErrMsg As String,
                                                        ByVal CUAA As String,
                                                        ByVal numero_validazione As String,
                                                        ByRef objParametri_Server As AgronicaCoreParametri) As String

        Dim importFascicolo As New AgroFascicolo

        Dim objConfSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
        Dim url = objConfSiti.Leggi_Valore(16, "AgroFascicolo_WS_AGREA", "", "", objParametri_Server)

        Dim strXML = importFascicolo.ScaricaAgroFascicolo(objParametri_Server.PivaSuperUser,
                                                          objParametri_Server.UtenteUsername,
                                                          objParametri_Server.UtenteCodFiscale,
                                                          20,
                                                          CUAA,
                                                          numero_validazione,
                                                          "",
                                                          True,
                                                          objParametri_Server,
                                                          ErrCOD,
                                                          ErrMsg,
                                                          url)

        Return strXML

    End Function

    Public Function CaricaCuaaModificata(ByRef ErrCOD As Integer,
                                         ByRef ErrMsg As String, ByRef Giorni As Integer,
                                         ByRef objParametri_Server As AgronicaCoreParametri
                                         ) As String

        Dim importCuaaMod As New AgroFascicolo

        Dim objConfSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
        Dim url = objConfSiti.Leggi_Valore(16, "AgroFascicolo_WS_AGREA", "", "", objParametri_Server)
        Dim DTCuaaModificate = importCuaaMod.GetCuaaModificata(objParametri_Server.PivaSuperUser,
                                                          objParametri_Server.UtenteUsername,
                                                          objParametri_Server.UtenteCodFiscale,
                                                          4,
                                                          Giorni,
                                                          "",
                                                          objParametri_Server,
                                                          ErrCOD,
                                                          ErrMsg,
                                                          url)
        ''Dim strXML = importFascicolo.GetSchede(objParametri_Server.PivaSuperUser,
        '                                                  objParametri_Server.UtenteUsername,
        '                                                  objParametri_Server.UtenteCodFiscale,
        '                                                  20,
        '                                                  CUAA,
        '                                                  "",
        '                                                  objParametri_Server,
        '                                                  ErrCOD,
        '                                                  ErrMsg,
        '                                                  url)

        Return DTCuaaModificate

    End Function

End Class
