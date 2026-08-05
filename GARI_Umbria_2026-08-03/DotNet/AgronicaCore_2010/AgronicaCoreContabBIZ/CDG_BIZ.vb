Imports System.Collections
Imports System.Collections.Generic
Imports System.Data
Imports System.Transactions
Imports System.Web
Imports AgronicaCoreAnagrafeDAL
Imports AgronicaCoreContabDAL
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreModelsSTD.anagrafiche
Imports AgronicaCoreModelsSTD.exceptions
Imports AgronicaCoreProfilazioneDAL
Imports AgronicaCoreUtentiDAL
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Public Class ObjDaInserire

    Public Property Id_Agenda As Integer
    'Public Property Id_Agenda_CDG As Integer
    Public Property Lav_Cod As Integer
    Public Property Id_CDG As Integer
    Public Property Id_CDG_Dettagli As Integer
    Public Property Modalita_Imputazione As Integer
    Public Property OrigineApp As Integer
    Public Property APP_CDG_Generale_ID As String
    Public Property Elem_Cod As Integer
    Public Property Cod_Risum As Integer
    Public Property Mac_Cod As Integer
    Public Property Id_Attivita As Integer
    Public Property attivita_poliannuale As Integer
    Public Property Attivita_Des As String
    Public Property Qualifica_Cod As Integer
    Public Property Tariffa_Cod As Integer
    Public Property Udm_Cod As Integer
    Public Property Id_Imputazione As Integer
    Public Property Linea_Cod As Integer
    Public Property Raggruppamento_Cod As Integer
    Public Property Cod_Animale As Integer
    Public Property Cod_Animale_Distinte As Integer
    Public Property Prezzo_Unitario As Double
    Public Property Prezzo_Totale As Double
    Public Property Impianto As Integer
    Public Property Sa_Cod As Integer
    Public Property Appezza As Integer
    Public Property Id_Reg As Integer
    Public Property Campo_Cod As Integer
    Public Property Progetto_Cod As Integer
    Public Property Macchine_Cod As Integer
    Public Property Rag_Soc As String
    Public Property Mac_Des As String
    Public Property Numero_CDC As Integer
    Public Property DtMov As DateTime
    Public Property DtInizio As DateTime
    Public Property DtFine As DateTime
    Public Property OreTot As Double
    Public Property DtStr As String
    Public Property Des_Lib As String
    Public Property Data_Operazione As Date

    'ANNA 06/10/23 COMMENTATO PER IL MOMENTO -- LASCIARE PER POSSIBILI FUTURI SVILUPPI
    'Public Property isBozza As Integer

End Class

Public Class CDG_BIZ_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function GetTipoCdG(ByVal Piva As String,
                               ByRef objParametri_Server As AgronicaCoreParametri,
                               ByRef objParametri_Utenti As AgronicaCoreParametri
                               ) As Integer

        '--------------------------------------------------------
        ' se vecchio tipo Return 1, se nuovo tipo Return 0
        '--------------------------------------------------------

        Dim Piva_SuperUser = objParametri_Server.PivaSuperUser

        Dim MessaggioErrore As String = String.Empty

        Dim NomeRoutine As String = "ContabBIZ.CDG_BIZ.GetTipoCdG()"

        Try

            Dim Profilazione_R As New Profilazione_R
            'Primo tentativo con P.Iva azienda
            Dim dtProfilazione = Profilazione_R.Leggi(Piva, 0, 0, "tipoCdG", "", "", 0, 0, False, objParametri_Server)
            If dtProfilazione.Rows.Count = 1 Then
                Return CInt(dtProfilazione.Rows(0).Item("Valore_Salvato"))
            Else
                'Secondo tentativo con sola P.Iva superuser 
                'dtProfilazione = Profilazione_R.Leggi("0", 0, 0, "tipoCdG", "", "", 0, 0, False, objParametri_Server)
                'If dtProfilazione.Rows.Count = 1 Then
                '    Return CInt(dtProfilazione.Rows(0).Item("Valore_Salvato"))
                'End If

                'QUELLO SOPRA NON ATTIVATO PERCHE' PER IL LIVELLO INSTALLAZIONE UTILIZZIAMO IL FLAG CHE CI INDICA 
                'CAB = 1
                'SBTF = 2
                'BASE = 3 --> Vecchio tipo

                Dim objImpost As New Utenti_Impostazioni_Read
                Dim Dt_Impost = objImpost.Leggi2(
                                            2, objParametri_Server.SuperUserUsername,
                                            enum_Impostazioni_Utenti.SUPERUSER_COD_ALGORITMO_COSTI_ACCESSORI,
                                            "", "", objParametri_Utenti)

                If Not IsNothing(Dt_Impost) AndAlso Dt_Impost.Rows.Count > 0 Then
                    If CInt(Dt_Impost.Rows(0).Item("Impostazione_Valore_1")) = 3 Then
                        Return 0
                    Else
                        Return 1
                    End If
                Else
                    Return 1
                End If

            End If

            Return -1

        Catch ex As Exception
            MessaggioErrore = "[" & NomeRoutine & "] : " & ex.Message
            Scrivi_LOG(objParametri_Server, NomeRoutine, MessaggioErrore)
        Finally

        End Try


        If Not String.IsNullOrEmpty(MessaggioErrore) Then
            Throw New Exception(MessaggioErrore)
        End If

        Return MessaggioErrore
    End Function

    Public Function GeneraJSON_OperazioniCampagna_AggiuntiModificati(ByVal piva As String, ByVal budget As Integer, ByRef objParametri_Server As AgronicaCoreParametri) As String

        Dim dal_r As New CDG_DAL_R()
        Dim dt As DataTable = dal_r.Leggi_OperazioniCampagna_AggiuntiModificati(piva, "", "", objParametri_Server)
        Dim dt_ricorrenze As DataTable = dal_r.Leggi_OperazioniCampagna_AggiuntiModificati_ContaRicorrenze(piva, "", "", objParametri_Server)

        Dim leggi_DW As New DW_CDG_Costi_Ricavi_DAL_R
        Dim leggi_Att As New AgronicaCoreContabDAL.AttivitaXOperazioni_R()

        Dim jArrayListaOp As New JArray()

        For Each dr As DataRow In dt.Rows

            Dim prezzo_unitario As Decimal
            If dr.Item("Elem_Cod") <> 0 AndAlso dr.Item("Elem_Cod") <> 1 AndAlso dr.Item("Elem_Cod") <> CostantiPersonalizzate.ALTRI_BENI Then
                prezzo_unitario = leggi_DW.Ipno_Valorizzazione_Prodotto(1, piva, dr.Item("Elem_Cod"), dr.Item("Pro_Cod"), dr.Item("Mat_Cod"), dr.Item("Udm_Cod"), 0, 0, 0, dr.Item("Lotto"), "", AGRODATAINIZIO, dr("Data_Movimento"), objParametri_Server)
            Else
                prezzo_unitario = dr.Item("prezzo_unitario")
            End If
            prezzo_unitario = Agro_Math.ArrotondaVal(prezzo_unitario, 5)

            Dim n_ricorrenze As Decimal = (From r As DataRow In dt_ricorrenze.Rows Where r.Item("piva") = dr.Item("piva") AndAlso r.Item("id_agenda") = dr.Item("id_agenda") AndAlso r.Item("id_mov") = dr.Item("id_mov") Select r.Item("numero")).First()
            Dim qta As Decimal = Math.Round(dr("Totale") / n_ricorrenze, 6)

            Dim valore_totale As Decimal = Agro_Math.ArrotondaVal(prezzo_unitario * qta, 2)

            'ID_Attivita: quello della riga, in alternativa uno dell'operazione, in alternativa il primo in attivitàXoperazioni, in alternativa 0
            Dim id_attivita As Integer = dr("id_attivita")
            If id_attivita = 0 Then
                id_attivita = (From riga As DataRow In dt.Rows Where riga.Item("id_agenda") = dr.Item("id_agenda") Select riga.Item("id_attivita")).Max()

                If id_attivita = 0 Then
                    Dim dtAtt As DataTable = leggi_Att.Leggi(0, dr("lav_cod"), "", "", objParametri_Server, piva)
                    id_attivita = If(Not IsNothing(dtAtt) AndAlso dtAtt.Rows.Count > 0, dtAtt.Rows(0).Item("id_attivita"), 0)
                End If
            End If

            Dim obj As New JObject()
            obj.Add(New JProperty("Data_Inserimento", CDate(dr("Data_Movimento")).ToString("s")))
            obj.Add(New JProperty("Modalita_Imputazione", "0"))
            obj.Add(New JProperty("Id_Agenda", dr("id_agenda").ToString()))
            obj.Add(New JProperty("Id_Mov", dr("id_mov").ToString()))
            obj.Add(New JProperty("Id_Mov_Det", "0"))
            obj.Add(New JProperty("Mac_Cod", If(dr("elem_cod") = 1, dr("Mat_Cod").ToString(), "0")))
            obj.Add(New JProperty("Cod_RisUm", If(dr("elem_cod") = 0, dr("Mat_Cod").ToString(), "0")))
            obj.Add(New JProperty("Elem_Cod", dr("Elem_Cod").ToString()))
            obj.Add(New JProperty("Pro_Cod", dr("Pro_Cod").ToString()))
            obj.Add(New JProperty("Mat_Cod", If(dr("elem_cod") = 0 OrElse dr("elem_cod") = 1, "0", dr("Mat_Cod").ToString())))
            obj.Add(New JProperty("Id_Attivita", id_attivita.ToString()))
            obj.Add(New JProperty("Qualifica_Cod", dr("Qualifica_Cod").ToString()))
            obj.Add(New JProperty("Tariffa_Cod", dr("Tariffa_Cod").ToString()))
            obj.Add(New JProperty("Turno_Cod", dr("Turno_Cod").ToString()))
            obj.Add(New JProperty("Conto_Cod", "0"))
            obj.Add(New JProperty("Lotto", dr("lotto").ToString()))
            obj.Add(New JProperty("Mezzo", "0"))
            obj.Add(New JProperty("Udm_Cod", If(dr("elem_cod") = 0 OrElse dr("elem_cod") = 1, "0", dr("udm_cod").ToString())))
            obj.Add(New JProperty("Prezzo_Unitario", prezzo_unitario.ToString()))
            obj.Add(New JProperty("Qta", qta.ToString()))
            obj.Add(New JProperty("Valore_Totale", valore_totale.ToString()))
            obj.Add(New JProperty("Descrizione", dr("des_lib").ToString()))
            obj.Add(New JProperty("Tipo_Ripartizione", "1"))
            obj.Add(New JProperty("Budget", budget.ToString()))
            obj.Add(New JProperty("CostiORicavi", "0"))
            obj.Add(New JProperty("Modalita_Ripartizione", "0"))
            obj.Add(New JProperty("Piva_SuperUser", objParametri_Server.PivaSuperUser))
            obj.Add(New JProperty("Piva", dr("Piva").ToString()))
            obj.Add(New JProperty("Sa_Cod", dr("Sa_Cod").ToString()))
            obj.Add(New JProperty("Appezza", dr("Appezza").ToString()))
            obj.Add(New JProperty("Id_Impianto", dr("id_reg").ToString()))
            obj.Add(New JProperty("Id_Imputazione", "0"))
            obj.Add(New JProperty("Macchine_Cod", "0"))
            obj.Add(New JProperty("Linea_Cod", "0"))
            obj.Add(New JProperty("Veg_Cod", "0"))
            obj.Add(New JProperty("Cul_Cod", "0"))
            obj.Add(New JProperty("Valore", valore_totale.ToString()))
            obj.Add(New JProperty("Tipo_Imputazione", "1"))

            jArrayListaOp.Add(obj)

        Next

        'ritorno il json trasformato in stringa
        Dim json As String = JsonConvert.SerializeObject(jArrayListaOp, Formatting.None)

        Return json

    End Function

    Public Function GeneraDT_OperazioniCampagna_Cancellati(ByVal piva As String, ByVal budget As Integer, ByRef objParametri_Server As AgronicaCoreParametri) As DataTable

        Dim dal_r As New CDG_DAL_R()
        Dim dt As DataTable = dal_r.Leggi_OperazioniCampagna_Cancellati(piva, budget, "", "", objParametri_Server)

        Return dt

    End Function

    Public Function ControllaCongruenzaCdCAttivita(ByVal Piva As String,
                                                   ByVal EFArrayToInsert As ArrayList,
                                                   ByRef objParametri_Server As AgronicaCoreParametri,
                                                       Optional ByVal Tipo_Fase As Integer = 0
                                                   ) As String

        Const nomeRoutine = "AgronicaCoreContabBIZ.CDG_BIZ_R.ControllaCongruenzaCdCAttivita()"
        Dim messaggioErrore As String = ""
        Dim msgErr = ""
        Dim Piva_SuperUser = objParametri_Server.PivaSuperUser
        Dim Testata_Griglia As CDG_Testata
        Dim DtAttPoliannuali As New DataTable
        Dim DrAttPoliannuali() As DataRow
        Dim alAttPrj As New ArrayList
        Dim alAttQdcZoo As New ArrayList
        Dim strAttPrj As String = ""
        Dim bAttAnnuale As Boolean = False
        Dim bAttPoliannuale As Boolean = False
        Dim leggi_Attivita_R As New AgronicaCoreContabDAL.Attivita_R
        Dim str_Key As String = ""
        Dim ht_Impianti As New Hashtable
        Dim bfirst As Boolean = True
        'Dim strAttQdcZoo As String = ""


        Try

            'Controllo Attività Ibride (Annuali e Poliaanuali insieme)
            '1. Lettura attivita poliannuali

            ' Verifico se attività è poliennale                                               
            DtAttPoliannuali = leggi_Attivita_R.Leggi(0, "Attivita_Poliannuale = 1 And Tipo_Utilizzo In (0,2)", "", objParametri_Server)

            For Each ArrayP As ArrayList In EFArrayToInsert
                For Each obj In ArrayP
                    If obj.GetType() Is GetType(CDG_Testata) Then
                        Testata_Griglia = obj


                        DrAttPoliannuali = DtAttPoliannuali.Select("Id_Attivita = " & Testata_Griglia.Id_Attivita)
                        If DrAttPoliannuali Is Nothing OrElse DrAttPoliannuali.Length = 0 Then
                            bAttAnnuale = True
                        Else
                            bAttPoliannuale = True
                        End If

                    End If
                Next
            Next

            'In caso di presenza di attività annuale --> non possono essere impostate più distinte per medesimo impianto
            If bAttAnnuale Then

                For Each ArrayP As ArrayList In EFArrayToInsert
                    For Each obj In ArrayP
                        If obj.GetType() Is GetType(CDG_Testata) Then
                            Testata_Griglia = obj
                            ht_Impianti.Clear()

                            If bfirst Then

                                bfirst = False

                                For Each DettaglioGriglia As CDG_Dettagli In Testata_Griglia.CDG_Dettagli

                                    'Controllo impostazione impianto
                                    If DettaglioGriglia.Appezza <> 0 And DettaglioGriglia.Id_Reg <> 0 Then

                                        str_Key = DettaglioGriglia.Piva & "|" &
                                              DettaglioGriglia.Sa_Cod & "|" &
                                              DettaglioGriglia.Appezza & "|" &
                                              DettaglioGriglia.Id_Reg

                                        If Not ht_Impianti.ContainsKey(str_Key) Then
                                            ht_Impianti.Add(str_Key, DettaglioGriglia.Progetto_Cod)
                                        Else

                                            'Stesso Impianto e diversa distinta --> errore
                                            msgErr = "E' possibile ripartire su più esercizi di uno stesso impianto solo se tutte le attività impostate sono pluriennali." & " <br/>"
                                            If messaggioErrore.IndexOf(msgErr) = -1 Then
                                                messaggioErrore &= msgErr
                                            End If
                                        End If


                                    End If
                                Next

                            End If

                        End If
                    Next
                Next

            End If






            For Each ArrayP As ArrayList In EFArrayToInsert
                For Each obj In ArrayP
                    If obj.GetType() Is GetType(CDG_Testata) Then
                        Testata_Griglia = obj
                        For Each DettaglioGriglia As CDG_Dettagli In Testata_Griglia.CDG_Dettagli
                            If DettaglioGriglia.Id_Imputazione <> 0 Then
                                strAttPrj = CStr(Testata_Griglia.Id_Attivita) & "|" & CStr(DettaglioGriglia.Id_Imputazione)
                                If Not alAttPrj.Contains(strAttPrj) Then
                                    alAttPrj.Add(strAttPrj)
                                End If
                            Else
                                If DettaglioGriglia.Campo_Cod <> 0 OrElse DettaglioGriglia.Id_Reg <> 0 OrElse DettaglioGriglia.Cod_Animale <> 0 Then
                                    'strAttQdcZoo = CStr(Testata_Griglia.Id_Attivita) & "|" & CStr(DettaglioGriglia.Id_CDG_Dettagli)
                                    If Not alAttQdcZoo.Contains(CStr(Testata_Griglia.Id_Attivita)) Then
                                        alAttQdcZoo.Add(CStr(Testata_Griglia.Id_Attivita))
                                    End If
                                End If
                            End If
                        Next
                    End If
                Next
            Next

            'Controllo che l'attività sia associata al progetto (cartelletta) se come CdC è stato scelto un progetto
            If alAttPrj.Count > 0 Then
                Dim keys As String() = Nothing
                Dim key_prj_cod As String = ""
                Dim key_att_cod As String = ""
                Dim Dt_Imputazioni_Fasi As DataTable = Nothing
                Dim descrAtt As String = ""
                Dim Dt_Imputazioni As DataTable = Nothing
                Dim leggi_Imputazioni_Fasi_R As New AgronicaCoreContabDAL.Imputazioni_Fasi_R

                Dim leggi_Imputazioni_R As New AgronicaCoreContabDAL.Imputazioni_R
                For Each st In alAttPrj
                    keys = st.Split("|")
                    key_att_cod = Integer.Parse(keys(0))
                    key_prj_cod = Integer.Parse(keys(1))
                    Dt_Imputazioni_Fasi = leggi_Imputazioni_Fasi_R.Leggi(Piva, key_att_cod, key_prj_cod, Tipo_Fase, "", "", objParametri_Server)
                    If Dt_Imputazioni_Fasi.Rows.Count = 0 Then
                        descrAtt = leggi_Attivita_R.AttivitaDes_From_AttivitaCod(key_att_cod, objParametri_Server)
                        Dt_Imputazioni = leggi_Imputazioni_R.Leggi(Piva, key_prj_cod, "", "", objParametri_Server)
                        msgErr = "Non è possibile utilizzare l'attività " & descrAtt & " con il progetto " & Dt_Imputazioni.Rows(0).Item("Imputazione_Nome") & " <br/>"
                        If messaggioErrore.IndexOf(msgErr) = -1 Then
                            messaggioErrore &= msgErr
                        End If
                    End If
                Next
            End If


            'Se come CdC non è stato scelto un progetto, controllo che l'attività sia associabile a un impianto o a un campo o allo zoo
            ' Lo è quando l'attività è associata ad un'operazione di campagna oppure non è associata nè a questa nè a un progetto
            If alAttQdcZoo.Count > 0 Then
                'Dim keys As String() = Nothing
                'Dim key_prj_cod As String = ""
                'Dim key_att_cod As String = ""
                Dim Dt_Operazioni As DataTable = Nothing

                Dim descrAtt As String = ""
                Dim Dt_AttivitaXOperazioni As DataTable = Nothing
                Dim leggi_AttivitaXOperazioni_R As New AgronicaCoreContabDAL.AttivitaXOperazioni_R

                Dim leggi_Operazioni_R As New AgronicaCoreAnagrafeDAL.Operazioni_R
                For Each key_att_cod In alAttQdcZoo
                    Dt_AttivitaXOperazioni = leggi_AttivitaXOperazioni_R.Leggi(key_att_cod, 0, "", "", objParametri_Server, Piva)
                    'Se l'attività non è mappata sull'operazione verifico se è mappata su progetto
                    If Dt_AttivitaXOperazioni.Rows.Count = 0 Then

                        Dim Dt_Imputazioni_Fasi As DataTable = Nothing
                        Dim leggi_Imputazioni_Fasi_R As New AgronicaCoreContabDAL.Imputazioni_Fasi_R
                        Dt_Imputazioni_Fasi = leggi_Imputazioni_Fasi_R.Leggi(Piva, key_att_cod, 0, Tipo_Fase, "", "", objParametri_Server)
                        If Dt_Imputazioni_Fasi.Rows.Count > 0 Then
                            descrAtt = leggi_Attivita_R.AttivitaDes_From_AttivitaCod(key_att_cod, objParametri_Server)
                            msgErr = "Non è possibile utilizzare l'attività " & descrAtt & " associata a QDCA o Zoo " & " <br/>"
                            If messaggioErrore.IndexOf(msgErr) = -1 Then
                                messaggioErrore &= msgErr
                            End If
                        End If
                    End If
                Next
            End If


        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try


        Return messaggioErrore

    End Function


End Class

Public Class CDG_BIZ_W
    Inherits AgronicaCoreDataProvider.DataProvider


    '##############################################################################################

    Public Function ExportCDGLanBIZ(ByVal Piva As String,
                                    ByVal Json As String,
                                    ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                    ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri
                                    ) As String

        Dim Piva_SuperUser = objParametri_Server.PivaSuperUser

        Dim MessaggioErrore As String = String.Empty
        Dim Vecchio_Tipo_Inser_Dati As Integer

        Dim NomeRoutine As String = "ContabBIZ.CDG_BIZ.ExportCDGLanBIZ()"

        Try

            Dim CDG_Testata_Export As CDG_Testata = Nothing
            Dim CDG_Dettagli_Export As CDG_Dettagli = Nothing
            Dim righeInseriteArray As JArray = JArray.Parse(Json)

            If righeInseriteArray.Count > 0 Then
                Dim obj = righeInseriteArray(0)
                Vecchio_Tipo_Inser_Dati = obj("Tipo_Imputazione")

                'Dim EFArrayToInsert As New ArrayList
                'For Each obj As JObject In righeInseriteArray


                '    'Testata
                '    CDG_Testata_Export = New CDG_Testata
                '    CDG_Testata_Export.Piva_Superuser = Piva_SuperUser
                '    CDG_Testata_Export.Piva = obj("Piva")
                '    CDG_Testata_Export.Data_Inserimento = obj("Data_Inserimento")
                '    CDG_Testata_Export.Modalita_Imputazione = obj("Modalita_Imputazione")
                '    CDG_Testata_Export.Id_Agenda = obj("Id_Agenda")
                '    CDG_Testata_Export.Id_Mov = obj("Id_Mov")
                '    CDG_Testata_Export.Id_Mov_Det = obj("Id_Mov_Det")
                '    CDG_Testata_Export.Mac_Cod = obj("Mac_Cod")
                '    CDG_Testata_Export.Cod_RisUm = obj("Cod_RisUm")
                '    CDG_Testata_Export.Elem_Cod = obj("Elem_Cod")
                '    CDG_Testata_Export.Pro_Cod = obj("Pro_Cod")
                '    CDG_Testata_Export.Mat_Cod = obj("Mat_Cod")
                '    CDG_Testata_Export.Id_Attivita = obj("Id_Attivita")
                '    CDG_Testata_Export.Qualifica_Cod = obj("Qualifica_Cod")
                '    CDG_Testata_Export.Tariffa_Cod = obj("Tariffa_Cod")
                '    CDG_Testata_Export.Turno_Cod = obj("Turno_Cod")
                '    CDG_Testata_Export.Conto_Cod = obj("Conto_Cod")
                '    CDG_Testata_Export.Lotto = obj("Lotto")
                '    CDG_Testata_Export.Mezzo = obj("Mezzo")
                '    CDG_Testata_Export.Udm_Cod = obj("Udm_Cod")
                '    CDG_Testata_Export.Prezzo_Unitario = Decimal.Parse(obj("Prezzo_Unitario"), Globalization.CultureInfo.CurrentCulture)
                '    CDG_Testata_Export.Qta = Decimal.Parse(obj("Qta"), Globalization.CultureInfo.CurrentCulture)
                '    CDG_Testata_Export.Valore_Totale = Decimal.Parse(obj("Valore_Totale"), Globalization.CultureInfo.CurrentCulture)
                '    CDG_Testata_Export.Descrizione = obj("Descrizione")
                '    CDG_Testata_Export.Tipo_Ripartizione = obj("Tipo_Ripartizione")
                '    CDG_Testata_Export.Budget = obj("Budget")
                '    CDG_Testata_Export.Costi_Ricavi = obj("CostiORicavi")
                '    CDG_Testata_Export.Modalita_Ripartizione = obj("Modalita_Ripartizione")
                '    CDG_Testata_Export.Vecchio_Tipo_Inser_Dati = obj("Tipo_Imputazione")

                '    Vecchio_Tipo_Inser_Dati = obj("Tipo_Imputazione")

                '    CDG_Testata_Export.Data_Creazione = Date.Now
                '    CDG_Testata_Export.Username_Creazione = objParametri.UsernameOperazione
                '    CDG_Testata_Export.Data_Modifica = Date.Now
                '    CDG_Testata_Export.Username_Modifica = objParametri.UsernameOperazione
                '    CDG_Testata_Export.inviato = 0
                '    CDG_Testata_Export.Validita_Inizio = AGRODATAINIZIO
                '    CDG_Testata_Export.Validita_Fine = AGRODATAFINE

                '    'Dettagli
                '    CDG_Dettagli_Export = New CDG_Dettagli
                '    CDG_Dettagli_Export.Piva_Superuser = Piva_SuperUser
                '    CDG_Dettagli_Export.Piva = obj("Piva")
                '    CDG_Dettagli_Export.Sa_Cod = obj("Sa_Cod")
                '    CDG_Dettagli_Export.Appezza = obj("Appezza")
                '    CDG_Dettagli_Export.Id_Reg = obj("Id_Impianto")
                '    CDG_Dettagli_Export.Id_Imputazione = obj("Id_Imputazione")
                '    CDG_Dettagli_Export.Macchine_Cod = obj("Macchine_Cod")
                '    CDG_Dettagli_Export.Linea_Cod = obj("Linea_Cod")
                '    CDG_Dettagli_Export.Veg_Cod = obj("Veg_Cod")
                '    CDG_Dettagli_Export.Cul_Cod = obj("Cul_Cod")
                '    CDG_Dettagli_Export.Valore = Decimal.Parse(obj("Valore"), Globalization.CultureInfo.CurrentCulture)
                '    CDG_Dettagli_Export.Data_Creazione = Date.Now
                '    CDG_Dettagli_Export.Username_Creazione = objParametri.UsernameOperazione
                '    CDG_Dettagli_Export.Data_Modifica = Date.Now
                '    CDG_Dettagli_Export.Username_Modifica = objParametri.UsernameOperazione
                '    CDG_Dettagli_Export.inviato = 0
                '    CDG_Dettagli_Export.Validita_Inizio = AGRODATAINIZIO
                '    CDG_Dettagli_Export.Validita_Fine = AGRODATAFINE

                '    CDG_Testata_Export.CDG_Dettagli.Add(CDG_Dettagli_Export)

                '    EFArrayToInsert.Add(CDG_Testata_Export)
                'Next


                'If String.IsNullOrEmpty(MessaggioErrore) Then
                Dim campConf_W As New CDG_DAL_W
                MessaggioErrore = campConf_W.Scrivi_Export_Lan(Piva_SuperUser, Piva, Vecchio_Tipo_Inser_Dati, righeInseriteArray, objParametri_Server, objParametri_Utenti)

            End If
            'End If

        Catch ex As Exception
            MessaggioErrore = "[" & NomeRoutine & "] : " & ex.Message
            Scrivi_LOG(objParametri_Server, NomeRoutine, MessaggioErrore)
        Finally

        End Try


        If Not String.IsNullOrEmpty(MessaggioErrore) Then
            Throw New Exception(MessaggioErrore)
        End If

        Return MessaggioErrore
    End Function




    Public Function AggiornaCDG(ByVal Piva_Agenda As String,
                                ByVal Id_Agenda As Integer, ByVal Id_Agenda_CDG As Integer, ByVal Lav_Cod As Integer, Des_Lib As String, ByVal Data_Inserimento As Date, ByVal Modalita_Imputazione As Integer,
                                ByVal Tipo_Ripartizione As Integer, Budget As Integer, ByVal Costi_Ricavi As Integer, ByVal Modalita_Ripartizione As Integer,
                                ByVal Id_Attivita_Batch As Integer, ByVal ModifInAutom As Integer,
                                ByVal origine_APP As Boolean, ByVal Split As Integer,
                                ByVal righeInseriteGrid_Testata_Manodopera As String, ByVal righeModificateGrid_Testata_Manodopera As String,
                                ByVal righeInseriteGrid_Testata_Terzisti As String, ByVal righeModificateGrid_Testata_Terzisti As String,
                                ByVal righeInseriteGrid_Testata_Macchine As String, ByVal righeModificateGrid_Testata_Macchine As String,
                                ByVal righeInseriteGrid_Testata_Magazzino As String, ByVal righeModificateGrid_Testata_Magazzino As String,
                                ByVal righeInseriteGrid_Testata_Libera As String, ByVal righeModificateGrid_Testata_Libera As String,
                                ByVal righeInseriteGrid_Testata_Eredita As String, ByVal righeModificateGrid_Testata_Eredita As String,
                                ByVal righeInseriteGrid_Dettagli_Impianti As String, ByVal righeModificateGrid_Dettagli_Impianti As String,
                                ByVal righeInseriteGrid_Dettagli_Progetti As String, ByVal righeModificateGrid_Dettagli_Progetti As String,
                                ByVal righeInseriteGrid_Dettagli_Macchine As String, ByVal righeModificateGrid_Dettagli_Macchine As String,
                                ByVal righeInseriteGrid_Dettagli_Linee As String, ByVal righeModificateGrid_Dettagli_Linee As String,
                                ByVal righeInseriteGrid_Dettagli_Zoo As String, ByVal righeModificateGrid_Dettagli_Zoo As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                Optional ByVal Id_Mov As Integer = 0, Optional ByVal Id_Mov_Det As Integer = 0,
                                Optional ByVal ConvertiToLocalTime As Boolean = True,
                                Optional ByRef GiasContext As Gias_DeveloperServer_Entities = Nothing,
                                Optional ByVal OpenNewTransaction As Boolean = True,
                                Optional ByRef listaErrori As List(Of ErroreGias) = Nothing,
                                Optional ByVal Piva_CDG As String = ""
                                ) As Integer

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        Dim MessaggioErrore As String = String.Empty
        Dim Id_Agenda_CDG_Return As Integer = 0
        Dim NomeRoutine As String = "AgronicaCoreContabBIZ.CDG_BIZ.AggiornaCDG()"
        Dim CDG_Testata_Ext As New CDG_Testata
        Dim Agenda As New Agenda
        Dim Movimento As New Movimenti
        Dim Movimento_Dettaglio As New Movimenti_dettagli
        Dim Mov_Destinazione As New Mov_Destinazioni
        Dim Mov_Dettagli_Riferimenti As Mov_Dettagli_Riferimenti


        Dim Count_Testata As Integer
        Dim bOk As Boolean = False
        Dim bPresente As Boolean = False
        Dim righeArray_Testata As JArray
        Dim Costi_Ricavi_Testata As Integer

        Dim EFArrayToInsert As New ArrayList
        Dim EFArrayToUpdate As New ArrayList
        Dim EFArrayToDelete As New ArrayList

        Dim bCheckScarico As Boolean = False
        Dim Flag_Movimento_Campagna As Integer = 0
        Dim bAgenda As Boolean = False
        Dim bMovimento As Boolean = False
        Dim Tab_Imputazione As String
        Dim Data_Ora_Inizio As Date
        Dim Data_Ora_Fine As Date

        Dim ObjDettagli_Erorri As New CDG_DAL_R
        Dim bOk_Errori As Boolean = True
        Dim Mat_Cod As Integer = 0

        'ANNA 06/10/23 COMMENTATO PER IL MOMENTO -- LASCIARE PER POSSIBILI FUTURI SVILUPPI
        'Dim isBozza As Integer = 0

        'Nota: se Id_Attivita_Batch <> 0 --> provengo da procedura batch --> devo modificare solo dettagli ed testata-eredita

        Dim ArrayInterno As ArrayList

        Dim scope As TransactionScope = Nothing

        Dim bCloseContext As Boolean = False
        If GiasContext Is Nothing Then
            GiasContext = Gias_EF_Utility.CreateGiasContextConnection(objParametri.StringaConnessione)
            bCloseContext = True
        End If

        If OpenNewTransaction Then
            Dim scopeOption As New TransactionScopeOption
            Dim transactionOptions As New TransactionOptions
            transactionOptions.IsolationLevel = If(origine_APP, Transactions.IsolationLevel.ReadUncommitted, Transactions.IsolationLevel.ReadCommitted)

            scope = New TransactionScope(scopeOption, transactionOptions)
        End If
        Try

            'In caso di non passaggio della Piva CDG --> assegnazione pari a quella del QDC (Vedere Bombardino Visite)
            If Piva_CDG = "" Then
                Piva_CDG = Piva_Agenda
            End If

            For Count_Testata = 0 To 11


                'JArray test1 = JArray.Parse("[\"john\"]");
                ' JArray test2 = JArray.Parse("[\"doe\"]");
                ' test1.Merge(test2);

                'Modo 3
                'JArray test1 = JArray.Parse("[\"john\"]");
                'JArray test2 = JArray.Parse("[\"doe\"]");
                'test1 = New JArray(test1.Union(test2));
                bOk = False

                'Determinazione della testata, se si tratta di un prodotto scaricato e del tab imputazione
                Select Case Count_Testata

                    Case 0
                        If righeInseriteGrid_Testata_Terzisti <> "" AndAlso righeInseriteGrid_Testata_Terzisti <> "[]" Then
                            righeArray_Testata = JArray.Parse(righeInseriteGrid_Testata_Terzisti)
                            bOk = True
                        End If

                        bCheckScarico = True

                        Flag_Movimento_Campagna = 0
                        Tab_Imputazione = "TERZISTA"
                    Case 1
                        If righeModificateGrid_Testata_Terzisti <> "" AndAlso righeModificateGrid_Testata_Terzisti <> "[]" Then
                            righeArray_Testata = JArray.Parse(righeModificateGrid_Testata_Terzisti)
                            bOk = True
                        End If

                        bCheckScarico = True

                        Flag_Movimento_Campagna = 0
                        Tab_Imputazione = "TERZISTA"
                    Case 2
                        If righeInseriteGrid_Testata_Magazzino <> "" AndAlso righeInseriteGrid_Testata_Magazzino <> "[]" Then
                            righeArray_Testata = JArray.Parse(righeInseriteGrid_Testata_Magazzino)

                            bOk = True
                        End If

                        bCheckScarico = True

                        Flag_Movimento_Campagna = 0
                        Tab_Imputazione = "MAGAZZINO"
                    Case 3
                        If righeModificateGrid_Testata_Magazzino <> "" AndAlso righeModificateGrid_Testata_Magazzino <> "[]" Then
                            righeArray_Testata = JArray.Parse(righeModificateGrid_Testata_Magazzino)
                            bOk = True
                        End If

                        bCheckScarico = True

                        Flag_Movimento_Campagna = 0
                        Tab_Imputazione = "MAGAZZINO"
                    Case 4
                        If righeInseriteGrid_Testata_Manodopera <> "" AndAlso righeInseriteGrid_Testata_Manodopera <> "[]" Then
                            righeArray_Testata = JArray.Parse(righeInseriteGrid_Testata_Manodopera)
                            bOk = True
                        End If
                        bCheckScarico = False
                        Flag_Movimento_Campagna = 0
                        Tab_Imputazione = "MANODOPERA"
                    Case 5
                        If righeModificateGrid_Testata_Manodopera <> "" AndAlso righeModificateGrid_Testata_Manodopera <> "[]" Then
                            righeArray_Testata = JArray.Parse(righeModificateGrid_Testata_Manodopera)
                            bOk = True
                        End If
                        bCheckScarico = False
                        Flag_Movimento_Campagna = 0
                        Tab_Imputazione = "MANODOPERA"
                    Case 6
                        If righeInseriteGrid_Testata_Macchine <> "" AndAlso righeInseriteGrid_Testata_Macchine <> "[]" Then
                            righeArray_Testata = JArray.Parse(righeInseriteGrid_Testata_Macchine)
                            bOk = True
                        End If
                        bCheckScarico = False
                        Flag_Movimento_Campagna = 0
                        Tab_Imputazione = "MACCHINE"
                    Case 7
                        If righeModificateGrid_Testata_Macchine <> "" AndAlso righeModificateGrid_Testata_Macchine <> "[]" Then
                            righeArray_Testata = JArray.Parse(righeModificateGrid_Testata_Macchine)
                            bOk = True
                        End If
                        bCheckScarico = False
                        Flag_Movimento_Campagna = 0
                        Tab_Imputazione = "MACCHINE"
                    Case 8
                        If righeInseriteGrid_Testata_Libera <> "" AndAlso righeInseriteGrid_Testata_Libera <> "[]" Then
                            righeArray_Testata = JArray.Parse(righeInseriteGrid_Testata_Libera)
                            bOk = True
                        End If
                        bCheckScarico = False
                        Flag_Movimento_Campagna = 0
                        Tab_Imputazione = "LIBERA"
                    Case 9
                        If righeModificateGrid_Testata_Libera <> "" AndAlso righeModificateGrid_Testata_Libera <> "[]" Then
                            righeArray_Testata = JArray.Parse(righeModificateGrid_Testata_Libera)
                            bOk = True
                        End If
                        bCheckScarico = False
                        Flag_Movimento_Campagna = 0
                        Tab_Imputazione = "LIBERA"
                    Case 10
                        If righeInseriteGrid_Testata_Eredita <> "" AndAlso righeInseriteGrid_Testata_Eredita <> "[]" Then
                            righeArray_Testata = JArray.Parse(righeInseriteGrid_Testata_Eredita)
                            bOk = True
                        End If
                        bCheckScarico = False
                        Flag_Movimento_Campagna = 1
                        Tab_Imputazione = "EREDITA"
                    Case 11
                        If righeModificateGrid_Testata_Eredita <> "" AndAlso righeModificateGrid_Testata_Eredita <> "[]" Then
                            righeArray_Testata = JArray.Parse(righeModificateGrid_Testata_Eredita)
                            bOk = True
                        End If
                        bCheckScarico = False
                        Flag_Movimento_Campagna = 1
                        Tab_Imputazione = "EREDITA"
                End Select

                If bOk Then

                    For Each obj As JObject In righeArray_Testata

                        ArrayInterno = New ArrayList


                        'Controllo Inserimento Agenda
                        If Not bAgenda Then

                            'Creazione Agenda
                            Agenda = BuildAgenda(Piva_CDG, 0, LAVCOD_COSTI_CDG, Split, Data_Inserimento, Des_Lib,
                                             AGRODATAINIZIO, AGRODATAFINE,
                                             objParametri)

                            ArrayInterno.Add(Agenda)

                            'Creazione Movimento
                            Movimento = BuildMovimento(Piva_CDG, 0, Data_Inserimento,
                                                   AGRODATAINIZIO, AGRODATAFINE,
                                                   objParametri)


                            ArrayInterno.Add(Movimento)

                            If Id_Agenda <> 0 Then

                                'Creazione Aggangio Id_Agenda -- Id_Agenda_CDG
                                Mov_Dettagli_Riferimenti = BuildMov_Dettagli_Riferimenti(Piva_Agenda, Piva_CDG, Lav_Cod, Id_Agenda, Id_Mov, Id_Mov_Det,
                                                                                         AGRODATAINIZIO, AGRODATAFINE,
                                                                                         Date.Now,
                                                                                         objParametri)


                                ArrayInterno.Add(Mov_Dettagli_Riferimenti)

                            End If

                            bAgenda = True 'Agenda Inserita

                        End If

                        'Controllo Scarico ed Imputazione Magazzino
                        If (bCheckScarico AndAlso Val(obj("Fabbricato_Cod")) <> 0) Then

                            'Correzione Bug. Arriva il mat_cod negativo per i terzisti.
                            If Tab_Imputazione = "TERZISTA" And Val(obj("Mat_Cod")) < 0 Then
                                Mat_Cod = -Val(obj("Mat_Cod"))
                            Else
                                Mat_Cod = Val(obj("Mat_Cod"))
                            End If

                            'Creazione Dettaglio Scarico
                            Movimento_Dettaglio = BuildMovimento_Dettaglio(Piva_CDG,
                                                                    0, 0, 0,
                                                                    Val(obj("Elem_Cod")), Val(obj("Pro_Cod")), Mat_Cod,
                                                                    Agro_SQL_SaveText(obj("Lotto"), False), Val(obj("Udm_Cod")), Val(obj("Qta")),
                                                                    Val(obj("Sa_Cod")),
                                                                    AGRODATAINIZIO, AGRODATAFINE,
                                                                    objParametri, Budget)


                            ArrayInterno.Add(Movimento_Dettaglio)

                            'Creazione Destinazione Scarico
                            Mov_Destinazione = BuildMov_Destinazione(Piva_CDG, 0, 0, 0,
                                                                Val(obj("Qta")),
                                                                Val(obj("Sa_Cod")), Val(obj("Tipo_Destinazione")), Val(obj("Fabbricato_Cod")),
                                                                AGRODATAINIZIO, AGRODATAFINE,
                                                                objParametri)



                            ArrayInterno.Add(Mov_Destinazione)

                        End If

                        'Impostazione Data_Ora_Inizio e Data_Ora_Fine
                        If obj("Ora_Inizio") Is Nothing Then
                            Data_Ora_Inizio = AGRODATAINIZIO
                            Data_Ora_Fine = AGRODATAFINE
                        Else
                            'ConvertiToLocalTime viene passato a False solo dal Bombardino: in questo caso l'ora è già letta in modo corretto da DB)
                            'Quando si arriva da griglia invece deve essere convertita
                            If ConvertiToLocalTime Then
                                Data_Ora_Inizio = Format(Data_Inserimento, "dd/MM/yyyy") & " " & Format(CDate(obj("Ora_Inizio")).ToLocalTime, "HH:mm")
                                Data_Ora_Fine = Format(Data_Inserimento, "dd/MM/yyyy") & " " & Format(CDate(obj("Ora_Fine")).ToLocalTime, "HH:mm")
                            Else
                                Data_Ora_Inizio = Format(Data_Inserimento, "dd/MM/yyyy") & " " & Format(CDate(obj("Ora_Inizio")), "HH:mm")
                                Data_Ora_Fine = Format(Data_Inserimento, "dd/MM/yyyy") & " " & Format(CDate(obj("Ora_Fine")), "HH:mm")
                            End If


                            If CDate(Data_Ora_Inizio) > CDate(Data_Ora_Fine) Then
                                'Aggiungo un giorno. es data_ora_inizio = 16:00 e data_ora_fine = 12:00
                                Data_Ora_Fine = DateAdd("d", 1, Data_Ora_Fine)
                            End If
                        End If

                        'Default per origineAPP per i prodotti scaricati
                        If CStr(obj("OrigineApp")) Is Nothing OrElse Val(obj("OrigineApp")) = 0 Then
                            obj("OrigineApp") = 0
                            obj("APP_CDG_Generale_ID") = ""
                        End If

                        'righeInseriteGrid_Dettagli_Zoo = "[]"
                        'righeModificateGrid_Dettagli_Zoo = "[]"


                        'Correzione Bug. Arriva il mat_cod negativo per i terzisti.
                        If Tab_Imputazione = "TERZISTA" And Val(obj("Mat_Cod")) < 0 Then
                            Mat_Cod = -Val(obj("Mat_Cod"))
                        Else
                            Mat_Cod = Val(obj("Mat_Cod"))
                        End If

                        'ANNA 06/10/23 COMMENTATO PER IL MOMENTO -- LASCIARE PER POSSIBILI FUTURI SVILUPPI
                        'If Tab_Imputazione = "MANODOPERA" AndAlso Val(obj("Bozza_Cod")) > 0 Then
                        '    isBozza = Val(obj("Bozza_Cod"))
                        'End If


                        'Correzione Costi_Ricavi in caso di scarico zoo --> ricavo. 
                        Costi_Ricavi_Testata = Costi_Ricavi 'Inizializzazione
                        Select Case Val(obj("Lav_Cod"))
                            Case LAVCOD_DECREMENTO_CONSISTENZE_ZOO, LAVCOD_VENDITA_ANIMALI, LAVCOD_TRASFERIMENTO_ANIMALI, LAVCOD_MACELLAZIONE_ANIMALI
                                If Val(obj("Elem_Cod")) = 0 And Tab_Imputazione = "LIBERA" Then
                                    Costi_Ricavi_Testata = 1 'Ricavo scarico animale
                                Else
                                    'Eventuale gestione di costi associati alla macellazione (per esempio)
                                End If

                            Case Else
                                'Do Nothing
                        End Select

                        'Creazione CDG
                        CDG_Testata_Ext = BuildCDG(Piva_CDG, 0, 0, 0, 0, Id_Agenda_CDG,
                                                   Data_Inserimento, Modalita_Imputazione,
                                                   Val(obj("Mac_Cod")), Val(obj("Cod_Risum")),
                                                   Val(obj("Elem_Cod")), Val(obj("Pro_Cod")), Mat_Cod,
                                                   Val(obj("ID_Attivita")), Val(obj("Qualifica_Cod")), Val(obj("Tariffa_Cod")), Val(obj("Turno_Cod")),
                                                   Val(obj("Conto_Cod")), Agro_SQL_SaveText(obj("Lotto"), False), Val(obj("Mezzo")), Val(obj("Udm_Cod")),
                                                   Val(obj("Prezzo_Unitario")), obj("Qta"), Val(obj("Prezzo_Totale")),
                                                   obj("OrigineApp"), obj("APP_CDG_Generale_ID"),
                                                   Agro_SQL_SaveText(obj("Prodotto_Des"), False), Tipo_Ripartizione, Budget,
                                                   Costi_Ricavi_Testata, Modalita_Ripartizione,
                                                   Val(obj("Tipo_Destinazione")), Val(obj("Sa_Cod")), Val(obj("Fabbricato_Cod")),
                                                   Flag_Movimento_Campagna, Val(obj("Lav_Cod")), Id_Attivita_Batch, Tab_Imputazione, ModifInAutom,
                                                   Data_Ora_Inizio, Data_Ora_Fine,
                                                   AGRODATAINIZIO, AGRODATAFINE,
                                                   righeInseriteGrid_Dettagli_Impianti, righeModificateGrid_Dettagli_Impianti,
                                                   righeInseriteGrid_Dettagli_Progetti, righeModificateGrid_Dettagli_Progetti,
                                                   righeInseriteGrid_Dettagli_Macchine, righeModificateGrid_Dettagli_Macchine,
                                                   righeInseriteGrid_Dettagli_Linee, righeModificateGrid_Dettagli_Linee,
                                                   righeInseriteGrid_Dettagli_Zoo, righeModificateGrid_Dettagli_Zoo,
                                                   objParametri)
                        'ANNA 06/10/23 COMMENTATO PER IL MOMENTO -- LASCIARE PER POSSIBILI FUTURI SVILUPPI
                        'isBozza:=isBozza)

                        ArrayInterno.Add(CDG_Testata_Ext)

                        EFArrayToInsert.Add(ArrayInterno)

                    Next

                    'Pulizia JArray
                    righeArray_Testata.Clear()

                End If

            Next

            If String.IsNullOrEmpty(MessaggioErrore) Then
                Dim leggi_CDG_R As New CDG_BIZ_R

                Dim Tipo_Fase As Integer = 0
                Select Case Costi_Ricavi
                    Case 0
                        Tipo_Fase = 1
                    Case 1
                        Tipo_Fase = 2
                    Case Else
                        'Non Gestito --> nesun filtro
                End Select

                If ModifInAutom = 0 Then
                    MessaggioErrore = leggi_CDG_R.ControllaCongruenzaCdCAttivita(Piva_CDG, EFArrayToInsert, objParametri, Tipo_Fase)
                End If

            End If

            If String.IsNullOrEmpty(MessaggioErrore) Then
                Dim CDG_W As New CDG_DAL_W

                'Dim transactionOptions = New TransactionOptions()
                'transactionOptions.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted
                'Using scope As New TransactionScope(TransactionScopeOption.Required, transactionOptions)

                Select Case Id_Agenda_CDG

                    Case 0

                        'Controllo che l'operazione sia effettivamente ancora non inserita (vedi concorrenzialità motorino)
                        Dim leggi As New CDG_DAL_R
                        Dim dt_agenda As New DataTable

                        If Modalita_Imputazione = 4 Then
                            dt_agenda = leggi.Leggi_Agenda_Riferimento(Piva_Agenda, Id_Agenda, Id_Mov_Det, True, objParametri, GiasContext:=GiasContext)
                        Else
                            dt_agenda = leggi.Leggi_Agenda_Riferimento(Piva_Agenda, Id_Agenda, 0, True, objParametri, GiasContext:=GiasContext)
                        End If

                        If dt_agenda.Rows.Count > 0 Then

                            bPresente = True
                            Id_Agenda_CDG_Return = dt_agenda.Rows(0).Item("Id_Agenda_Rif")

                        Else

                            bPresente = False

                        End If


                    Case Else

                        bPresente = False

                End Select

                If Not bPresente Then

                    'Diretto                    

                    Id_Agenda_CDG_Return = CDG_W.Aggiorna_CDG(False, Modalita_Imputazione, Piva_CDG, Id_Agenda_CDG, 0, Des_Lib, Data_Inserimento, EFArrayToInsert, objParametri, GiasContext:=GiasContext)


                    Select Case Id_Agenda_CDG_Return

                        Case -1 'Errore

                            Throw New Exception

                        Case Else

                            'Lettura Errori
                            MessaggioErrore = CheckErroriDettagli(Piva_CDG, Id_Agenda, Id_Agenda_CDG_Return, NomeRoutine, CBool(ModifInAutom), IIf(Id_Agenda_CDG = 0, "Inserimento", "Modifica"), objParametri)

                            Select Case Trim(MessaggioErrore)

                                Case ""

                                    'Correzione Bug: Succede che raramente non scriva l'Id_Mov.
                                    'Il bug dovrebbe essere in scaricotempi ma per essere certi faccio un update anche qui. 
                                    Dim ObjTestata_Erorri As New CDG_DAL_W
                                    ObjTestata_Erorri.AllineaId_Mov(Piva_CDG, Id_Agenda_CDG_Return, objParametri)

                                    'COMMIT Effettivo
                                    If OpenNewTransaction Then
                                        scope.Complete()
                                    End If


                                Case Else

                                    bOk_Errori = False
                                    Throw New Exception(MessaggioErrore)

                            End Select

                    End Select

                End If


                If String.IsNullOrEmpty(MessaggioErrore) Then

                Else
                    If OpenNewTransaction Then
                        ' Rollback
                        scope.Dispose()
                    End If
                    Id_Agenda_CDG_Return = 0
                End If
            End If


        Catch ex As Exception

            If bOk_Errori Then
                MessaggioErrore = "[" & NomeRoutine & "] :   " & ex.Message
                Throw New Exception(MessaggioErrore)
            End If

            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)

            ' Rollback
            If ModifInAutom = 1 Then
                'Nota: in cambio di modifica automatica salvo cmq i dati, altrimenti quando riapre la pagina sono spariti e sul salva se li perde definitivamente
                If OpenNewTransaction Then
                    scope.Complete()
                End If

            Else
                If OpenNewTransaction Then
                    scope.Dispose()
                End If
            End If



            Id_Agenda_CDG_Return = 0

        Finally
            If bCloseContext Then
                GiasContext.Dispose()
                GiasContext = Nothing
            End If

            If OpenNewTransaction Then
                scope.Dispose()
            End If

            If MessaggioErrore <> "" AndAlso listaErrori IsNot Nothing Then
                'Aggiunta per Operazioni NG.. semina con frazionamento
                'Questo errore del bombardino richiede assistenza da parte dell'Amministratore...deve essere visibile all'utente quando appare 
                Dim objErroreGias As New ErroreGias With {
                    .severity = ErroreGias_Severity.Bloccante,
                    .tipo = ErroreGias_Tipo.Generico,
                    .messaggio = MessaggioErrore
                }

                listaErrori.Add(objErroreGias)
            End If
        End Try


        If Not String.IsNullOrEmpty(MessaggioErrore) Then
            Throw New Exception(MessaggioErrore)
        End If

        Return Id_Agenda_CDG_Return

    End Function




    Public Function AggiornaCDG_Split_SeminaTrapianto(ByVal Piva As String,
                                                      ByVal Id_Agenda_Old As Integer,
                                                      ByVal Id_Agenda_New As Integer,
                                                      ByVal DT_Old As DataTable,
                                                      ByVal DT_New As DataTable,
                                                      ByRef EFArrayToInsert As ArrayList,
                                                      ByRef Id_Agenda_CDG As Integer,
                                                      ByRef Des_Lib As String,
                                                      ByRef Data_Movimento_New As DateTime,
                                                      ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                      Optional ByVal FrazionaAppezzamenti As Boolean = False,
                                                      Optional ByVal SupFrazionata As Decimal = 0,
                                                      Optional ByVal AggiornaCostiOld As Boolean = False) As Integer

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        Dim MessaggioErrore As String = String.Empty
        Dim NomeRoutine As String = "AgronicaCoreContabBIZ.AggiornaCDG_Split_SeminaTrapianto.AggiornaCDG()"
        Dim CDG_Testata_Ext As New CDG_Testata
        Dim Agenda As New Agenda
        Dim Movimento As New Movimenti
        Dim Movimento_Dettaglio As New Movimenti_dettagli
        Dim Mov_Destinazione As New Mov_Destinazioni
        Dim Mov_Dettagli_Riferimenti As Mov_Dettagli_Riferimenti

        Dim ObjAgenda As New AgronicaCoreContabDAL.Agenda_R
        Dim DT_Agenda_New As New DataTable
        Dim DT_Agenda_Old As New DataTable
        'Dim DT_CDG_Old As New DataTable
        Dim DT_CDG_New As New DataTable
        Dim DT_Movimenti_Dettagli_Old As New DataTable
        Dim DT_Mov_Destinazioni_Old As New DataTable

        Dim Id_Agenda_CDG_Old As Integer = 0
        'Dim Id_Agenda_CDG As Integer = 0

        Dim DT_CDG_Old As New DataTable

        'Dim EFArrayToInsert As New ArrayList

        Dim ArrayInterno As ArrayList

        Dim leggi As New CDG_DAL_R
        Dim leggi_dettagli As New AgronicaCoreContabDAL.Movimenti_Dettagli_R
        Dim leggi_destinazioni As New AgronicaCoreContabDAL.Mov_Destinazioni_R
        Dim CDG_W As New CDG_DAL_W
        Dim Data_Movimento_Old As DateTime
        'Dim Data_Movimento_New As DateTime
        Dim Data_Inizio_New As DateTime
        Dim Data_Fine_New As DateTime
        Dim Data_Inizio_Old As DateTime
        Dim Data_Fine_Old As DateTime
        Dim Lav_Cod As Integer = 0
        'Dim Des_Lib As String = ""

        Dim Qta_New As Decimal = 0
        Dim Qta_Old As Decimal = 0
        Dim Valore_Totale_New As Decimal = 0
        Dim Valore_Totale_Old As Decimal = 0
        Dim Superficie_New As Decimal = 0
        Dim Superficie_Old As Decimal = 0
        Dim Superficie_Totale As Decimal = 0

        Try


            '1. Controllo che la vecchia operazione di agenda abbia dei CDG --> Do nothing
            DT_Agenda_Old = leggi.Leggi_Agenda_Riferimento_SQL(Piva, Id_Agenda_Old, 0, objParametri)


            If DT_Agenda_Old.Rows.Count > 0 Then

                ArrayInterno = New ArrayList

                Id_Agenda_CDG_Old = DT_Agenda_Old.Rows(0).Item("Id_Agenda_Rif")
                Data_Movimento_Old = CDate(DT_Agenda_Old(0)("Data_Movimento"))

                'Lettura Dati Agenda Nuova
                DT_Agenda_New = ObjAgenda.Leggi(Piva, 0, Id_Agenda_New, 0, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri)

                Lav_Cod = DT_Agenda_New(0)("Lav_Cod")
                Des_Lib = DT_Agenda_New.Rows(0).Item("Des_Lib")
                Data_Movimento_New = DT_Agenda_New.Rows(0).Item("Validita_Inizio")

                'Calcolo Nuova Quantità Old in Base alla nuova quota di distribuzione totale
                For Each dr_cdg_old As DataRow In DT_Old.Rows
                    Superficie_Old = Superficie_Old + dr_cdg_old("Qta2")
                Next


                'Calcolo Nuova Quantità in Base alla nuova quota di distribuzione totale
                For Each dr_cdg_new As DataRow In DT_New.Rows
                    Superficie_New = Superficie_New + dr_cdg_new("Qta2")
                Next

                Select Case FrazionaAppezzamenti
                    Case True
                        Superficie_Totale = Superficie_Old + SupFrazionata
                    Case Else
                        Superficie_Totale = Superficie_New + Superficie_Old
                End Select


                'Controllo se l'operazione 4500 è già stata creata per la nuova agenda (costi esistenti da QDC)
                DT_Agenda_New = leggi.Leggi_Agenda_Riferimento_SQL(Piva, Id_Agenda_New, 0, objParametri)

                If DT_Agenda_New.Rows.Count <> 0 Then

                    Id_Agenda_CDG = DT_Agenda_New.Rows(0).Item("Id_Agenda_Rif")

                End If

                'Costruzione CDG per la nuova agenda

                'Creazione Agenda
                Agenda = BuildAgenda(Piva, 0, LAVCOD_COSTI_CDG, 0, Data_Movimento_New, Des_Lib,
                                     AGRODATAINIZIO, AGRODATAFINE,
                                     objParametri)

                ArrayInterno.Add(Agenda)

                'Creazione Movimento
                Movimento = BuildMovimento(Piva, 0, Data_Movimento_New,
                                                AGRODATAINIZIO, AGRODATAFINE,
                                                objParametri)


                ArrayInterno.Add(Movimento)


                'Creazione Aggangio Id_Agenda -- Id_Agenda_CDG
                Mov_Dettagli_Riferimenti = BuildMov_Dettagli_Riferimenti(Piva, Piva, Lav_Cod, Id_Agenda_New, 0, 0,
                                                                         AGRODATAINIZIO, AGRODATAFINE,
                                                                         DateAdd("d", -1, Data_Movimento_New),
                                                                         objParametri)


                ArrayInterno.Add(Mov_Dettagli_Riferimenti)


                'Lettura dei dettagli dell'Agenda CDG OLD
                DT_Movimenti_Dettagli_Old = leggi_dettagli.LeggiCaricoScarico_New(Piva, 0, Id_Agenda_CDG_Old, 0, 0, 0, 0, 0, 0, 0, 0, 0, "", 0, 0, 0, 0, "", 0, "", "", "", objParametri)

                'Costruzione Movimenti Dettagli con Qta Corrette
                For Each dr_dettagli As DataRow In DT_Movimenti_Dettagli_Old.Rows

                    Qta_New = dr_dettagli("Qta") * Superficie_New / Superficie_Totale

                    'Creazione Dettaglio Scarico
                    Movimento_Dettaglio = BuildMovimento_Dettaglio(Piva,
                                                                        0, 0, 0,
                                                                        Val(dr_dettagli("Elem_Cod")), Val(dr_dettagli("Pro_Cod")), Val(dr_dettagli("Mat_Cod")),
                                                                        Agro_SQL_SaveText(dr_dettagli("Lotto"), False), Val(dr_dettagli("Udm_Cod")), Qta_New,
                                                                        Val(dr_dettagli("Sa_Cod")),
                                                                        AGRODATAINIZIO, AGRODATAFINE,
                                                                        objParametri)


                    ArrayInterno.Add(Movimento_Dettaglio)


                    'Lettura dei dettagli dell'Agenda CDG OLD
                    DT_Mov_Destinazioni_Old = leggi_destinazioni.Leggi(Piva, 0, Id_Agenda_CDG_Old, dr_dettagli("Id_Mov"), dr_dettagli("Id_Mov_Det"), 0, 0, 0, enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri)

                    'Costruzione Movimenti Destinazioni con Qta Corrette
                    For Each dr_destinazioni As DataRow In DT_Mov_Destinazioni_Old.Rows

                        'Creazione Destinazione Scarico
                        Mov_Destinazione = BuildMov_Destinazione(Piva, 0, 0, 0,
                                                                    Qta_New,
                                                                    Val(dr_destinazioni("Sa_Cod")), Val(dr_destinazioni("Tipo_Destinazione")), Val(dr_destinazioni("Id_Destinazione")),
                                                                    AGRODATAINIZIO, AGRODATAFINE,
                                                                    objParametri)



                        ArrayInterno.Add(Mov_Destinazione)


                    Next

                Next


                ''Lettura delle testate del vecchio id_agenda
                DT_CDG_Old = leggi.Leggi_CDG_Testata(Piva, Id_Agenda_CDG_Old, 0, "Tab_Imputazione <> 'EREDITA'", objParametri)


                If DT_CDG_Old.Rows.Count > 0 Then


                    'Costruzione Testate con Qta Corrette
                    For Each dr_cdg As DataRow In DT_CDG_Old.Rows

                        'Calcolo nuova Qta per la Testata New
                        Qta_New = dr_cdg("Qta") * Superficie_New / Superficie_Totale
                        Valore_Totale_New = Qta_New * dr_cdg("Prezzo_Unitario")

                        Data_Inizio_New = Data_Movimento_New
                        Data_Fine_New = DateAdd("H", CInt(Qta_New), Data_Inizio_New)


                        'Creazione CDG
                        CDG_Testata_Ext = BuildCDG(Piva, 0, 0, 0, 0, Id_Agenda_CDG,
                                                   Data_Movimento_New, dr_cdg("Modalita_Imputazione"),
                                                   Val(dr_cdg("Mac_Cod")), Val(dr_cdg("Cod_Risum")),
                                                   Val(dr_cdg("Elem_Cod")), Val(dr_cdg("Pro_Cod")), Val(dr_cdg("Mat_Cod")),
                                                   Val(dr_cdg("ID_Attivita")), Val(dr_cdg("Qualifica_Cod")), Val(dr_cdg("Tariffa_Cod")), Val(dr_cdg("Turno_Cod")),
                                                   Val(dr_cdg("Conto_Cod")), Agro_SQL_SaveText(dr_cdg("Lotto"), False), Val(dr_cdg("Mezzo")), Val(dr_cdg("Udm_Cod")),
                                                   Val(dr_cdg("Prezzo_Unitario")), Qta_New, Valore_Totale_New,
                                                   dr_cdg("OrigineApp"), dr_cdg("APP_CDG_Generale_ID"),
                                                   Agro_SQL_SaveText(dr_cdg("Descrizione"), False), dr_cdg("Tipo_Ripartizione"), dr_cdg("Budget"),
                                                   dr_cdg("Costi_Ricavi"), dr_cdg("Modalita_Ripartizione"),
                                                   Val(dr_cdg("Tipo_Destinazione")), Val(dr_cdg("Sa_Cod")), Val(dr_cdg("ID_Destinazione")),
                                                   dr_cdg("Flag_Movimento_Campagna"), Val(dr_cdg("Lav_Cod")), dr_cdg("Id_Attivita"), dr_cdg("Tab_Imputazione"), True,
                                                   Data_Inizio_New, Data_Fine_New,
                                                   AGRODATAINIZIO, AGRODATAFINE,
                                                   "", "",
                                                   "", "",
                                                   "", "",
                                                   "", "",
                                                   "", "",
                                                   objParametri)




                        ArrayInterno.Add(CDG_Testata_Ext)


                    Next


                    EFArrayToInsert.Add(ArrayInterno)

                    If FrazionaAppezzamenti = False Or AggiornaCostiOld = True Then

                        '==================================================================================================================================
                        'Sistemazione dei totali Testata ID_Agenda_CDG_Old                       

                        For Each dr_cdg_old As DataRow In DT_CDG_Old.Rows

                            'Calcolo nuova Qta per la Testata Old
                            Qta_Old = dr_cdg_old("Qta") * Superficie_Old / Superficie_Totale
                            Valore_Totale_Old = Qta_Old * dr_cdg_old("Prezzo_Unitario")

                            If IsDate(dr_cdg_old("Data_Ora_Inizio")) Then

                                Data_Inizio_Old = dr_cdg_old("Data_Ora_Inizio")
                                Data_Fine_Old = DateAdd("H", CInt(Qta_Old), Data_Inizio_Old)
                            Else
                                Data_Inizio_Old = Data_Movimento_Old
                                Data_Fine_Old = Data_Movimento_Old

                            End If

                            Qta_Old = Math.Round(Qta_Old, 3)
                            Valore_Totale_Old = Math.Round(Valore_Totale_Old, 2)

                            CDG_W.Modifica_CDG_Testata_Split_Semina_Trapianto(Piva, Id_Agenda_CDG_Old, dr_cdg_old("Id_CDG"), Qta_Old, Data_Inizio_Old, Data_Fine_Old, Valore_Totale_Old, objParametri)


                        Next

                        'Sistemazione dei totali Dettagli e Destinazioni ID_Agenda_CDG_Old                       

                        For Each dr_dettagli As DataRow In DT_Movimenti_Dettagli_Old.Rows

                            'Calcolo nuova Qta per la Testata Old
                            Qta_Old = dr_dettagli("Qta") * Superficie_Old / Superficie_Totale

                            Qta_Old = Math.Round(Qta_Old, 3)
                            Qta_Old = CInt(Qta_Old)

                            CDG_W.Modifica_Scarico_Split_Semina_Trapianto(Piva, Id_Agenda_CDG_Old, dr_dettagli("Id_Mov_Det"), Qta_Old, objParametri)


                        Next

                    End If

                    'aggiorno il log
                    Dim objAgronicaLogAgendaW As New AgronicaCoreContabDAL.AgronicaLogAgenda_W
                    objAgronicaLogAgendaW.Scrivi(Data_Movimento_Old,
                                             enum_TipoOperazioneDB.Modifica,
                                             Des_Lib,
                                             Id_Agenda_Old,
                                             Piva,
                                             0,
                                             Lav_Cod,
                                             CInt(enum_Id_Servizio.GiasOnline),
                                             objParametri)


                    objAgronicaLogAgendaW = Nothing


                End If

                '    Dim scrivi_DAL As New CDG_DAL_W
                'scrivi_DAL.AllineaDateRiferimento(Piva, Id_Agenda_Old, Id_Agenda_CDG_Old, DateAdd("d", -1, Data_Movimento_Old), objParametri)

            End If


        Catch ex As Exception


            MessaggioErrore = "[" & NomeRoutine & "] :   " & ex.Message
            Throw New Exception(MessaggioErrore)


            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)

        Finally

        End Try


        If Not String.IsNullOrEmpty(MessaggioErrore) Then
            Throw New Exception(MessaggioErrore)
        End If

        Return Id_Agenda_CDG_Old

    End Function



    'Public Function AggiornaCDG_Split_SeminaTrapianto(ByVal Piva As String,
    '                                                  ByVal Id_Agenda_Old As Integer,
    '                                                  ByVal Id_Agenda_New As Integer,
    '                                                  ByVal DT_Old As DataTable,
    '                                                  ByVal DT_New As DataTable,
    '                                                  ByRef DT_CDG_Old As DataTable,
    '                                                  ByRef DT_Movimenti_Dettagli_Old As DataTable,
    '                                                  ByRef Id_Agenda_CDG_Old As Integer,
    '                                                  ByRef Id_Agenda_CDG As Integer,
    '                                                  ByRef Superficie_Old As Decimal,
    '                                                  ByRef Superficie_Totale As Decimal,
    '                                                  ByRef Des_Lib As String,
    '                                                  ByRef Data_Movimento_Old As DateTime,
    '                                                  ByRef Data_Movimento_New As DateTime,
    '                                                  ByRef EFArrayToInsert As ArrayList,
    '                                                  ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Integer

    '    Dim Piva_SuperUser = objParametri.PivaSuperUser

    '    Dim MessaggioErrore As String = String.Empty
    '    Dim Id_Agenda_CDG_Return As Integer = 0
    '    Dim NomeRoutine As String = "AgronicaCoreContabBIZ.AggiornaCDG_Split_SeminaTrapianto.AggiornaCDG()"
    '    Dim CDG_Testata_Ext As New CDG_Testata
    '    Dim Agenda As New Agenda
    '    Dim Movimento As New Movimenti
    '    Dim Movimento_Dettaglio As New Movimenti_dettagli
    '    Dim Mov_Destinazione As New Mov_Destinazioni
    '    Dim Mov_Dettagli_Riferimenti As Mov_Dettagli_Riferimenti

    '    Dim ObjAgenda As New AgronicaCoreContabDAL.Agenda_R
    '    Dim DT_Agenda_New As New DataTable
    '    Dim DT_Agenda_Old As New DataTable
    '    'Dim DT_CDG_Old As New DataTable
    '    Dim DT_CDG_New As New DataTable
    '    'Dim DT_Movimenti_Dettagli_Old As New DataTable
    '    Dim DT_Mov_Destinazioni_Old As New DataTable

    '    'Dim Id_Agenda_CDG_Old As Integer = 0
    '    'Dim Id_Agenda_CDG As Integer = 0

    '    'Dim EFArrayToInsert As New ArrayList

    '    Dim ArrayInterno As ArrayList

    '    Dim leggi As New CDG_DAL_R
    '    Dim leggi_dettagli As New AgronicaCoreContabDAL.Movimenti_Dettagli_R
    '    Dim leggi_destinazioni As New AgronicaCoreContabDAL.Mov_Destinazioni_R
    '    Dim CDG_W As New CDG_DAL_W
    '    'Dim Data_Movimento_Old As DateTime
    '    'Dim Data_Movimento_New As DateTime
    '    Dim Data_Inizio_New As DateTime
    '    Dim Data_Fine_New As DateTime
    '    Dim Data_Inizio_Old As DateTime
    '    Dim Data_Fine_Old As DateTime
    '    Dim Lav_Cod As Integer = 0
    '    'Dim Des_Lib As String = ""

    '    Dim Qta_New As Decimal = 0
    '    Dim Qta_Old As Decimal = 0
    '    Dim Valore_Totale_New As Decimal = 0
    '    Dim Valore_Totale_Old As Decimal = 0
    '    Dim Superficie_New As Decimal = 0
    '    'Dim Superficie_Old As Decimal = 0
    '    'Dim Superficie_Totale As Decimal = 0

    '    Try


    '        '1. Controllo che la vecchia operazione di agenda abbia dei CDG --> Do nothing
    '        'DT_Agenda_Old = leggi.Leggi_Agenda_Riferimento(Piva, Id_Agenda_Old, 0, True, objParametri)

    '        DT_Agenda_Old = leggi.Leggi_Agenda_Riferimento_SQL(Piva, Id_Agenda_Old, 0, objParametri)




    '        If DT_Agenda_Old.Rows.Count > 0 Then

    '            ArrayInterno = New ArrayList

    '            Id_Agenda_CDG_Old = DT_Agenda_Old.Rows(0).Item("Id_Agenda_Rif")
    '            Data_Movimento_Old = CDate(DT_Agenda_Old(0)("Data_Movimento"))

    '            'Passo Il bombardino sulla operazione vecchia
    '            'AllineaCostiDaCampagna_Bombardino(Piva, Id_Agenda_Old, Data_Movimento_Old, True, objParametri)


    '            'Lettura Dati Agenda Nuova
    '            DT_Agenda_New = ObjAgenda.Leggi(Piva, 0, Id_Agenda_New, 0, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri)

    '            Lav_Cod = DT_Agenda_New(0)("Lav_Cod")
    '            Des_Lib = DT_Agenda_New.Rows(0).Item("Des_Lib")
    '            Data_Movimento_New = DT_Agenda_New.Rows(0).Item("Validita_Inizio")

    '            'Calcolo Nuova Quantità Old in Base alla nuova quota di distribuzione totale
    '            For Each dr_cdg_old As DataRow In DT_Old.Rows
    '                Superficie_Old = Superficie_Old + dr_cdg_old("Qta2")
    '            Next


    '            'Calcolo Nuova Quantità in Base alla nuova quota di distribuzione totale
    '            For Each dr_cdg_new As DataRow In DT_New.Rows
    '                Superficie_New = Superficie_New + dr_cdg_new("Qta2")
    '            Next

    '            Superficie_Totale = Superficie_New + Superficie_Old


    '            'Controllo se l'operazione 4500 è già stata creata per la nuova agenda (costi esistenti da QDC)
    '            DT_Agenda_New = leggi.Leggi_Agenda_Riferimento(Piva, Id_Agenda_New, 0, True, objParametri)

    '            If DT_Agenda_New.Rows.Count <> 0 Then

    '                Id_Agenda_CDG = DT_Agenda_New.Rows(0).Item("Id_Agenda_Rif")

    '            End If

    '            'Costruzione CDG per la nuova agenda

    '            'Creazione Agenda
    '            Agenda = BuildAgenda(Piva, 0, LAVCOD_COSTI_CDG, 0, Data_Movimento_New, Des_Lib,
    '                                                AGRODATAINIZIO, AGRODATAFINE,
    '                                                objParametri)

    '            ArrayInterno.Add(Agenda)

    '            'Creazione Movimento
    '            Movimento = BuildMovimento(Piva, 0, Data_Movimento_New,
    '                                            AGRODATAINIZIO, AGRODATAFINE,
    '                                            objParametri)


    '            ArrayInterno.Add(Movimento)


    '            'Creazione Aggangio Id_Agenda -- Id_Agenda_CDG
    '            Mov_Dettagli_Riferimenti = BuildMov_Dettagli_Riferimenti(Piva, Lav_Cod, Id_Agenda_New, 0, 0,
    '                                                                         AGRODATAINIZIO, AGRODATAFINE,
    '                                                                         objParametri)


    '            ArrayInterno.Add(Mov_Dettagli_Riferimenti)


    '            'Lettura dei dettagli dell'Agenda CDG OLD
    '            DT_Movimenti_Dettagli_Old = leggi_dettagli.LeggiCaricoScarico_New(Piva, 0, Id_Agenda_CDG_Old, 0, 0, 0, 0, 0, 0, 0, 0, 0, "", 0, 0, 0, 0, "", 0, "", "", "", objParametri)

    '            'Costruzione Movimenti Dettagli con Qta Corrette
    '            For Each dr_dettagli As DataRow In DT_Movimenti_Dettagli_Old.Rows

    '                Qta_New = dr_dettagli("Qta") * Superficie_New / Superficie_Totale

    '                'Creazione Dettaglio Scarico
    '                Movimento_Dettaglio = BuildMovimento_Dettaglio(Piva,
    '                                                                    0, 0, 0,
    '                                                                    Val(dr_dettagli("Elem_Cod")), Val(dr_dettagli("Pro_Cod")), Val(dr_dettagli("Mat_Cod")),
    '                                                                    Agro_SQL_SaveText(dr_dettagli("Lotto"), False), Val(dr_dettagli("Udm_Cod")), Qta_New,
    '                                                                    Val(dr_dettagli("Sa_Cod")),
    '                                                                    AGRODATAINIZIO, AGRODATAFINE,
    '                                                                    objParametri)


    '                ArrayInterno.Add(Movimento_Dettaglio)


    '                'Lettura dei dettagli dell'Agenda CDG OLD
    '                DT_Mov_Destinazioni_Old = leggi_destinazioni.Leggi(Piva, 0, Id_Agenda_CDG_Old, dr_dettagli("Id_Mov"), dr_dettagli("Id_Mov_Det"), 0, 0, 0, enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri)

    '                'Costruzione Movimenti Destinazioni con Qta Corrette
    '                For Each dr_destinazioni As DataRow In DT_Mov_Destinazioni_Old.Rows

    '                    'Creazione Destinazione Scarico
    '                    Mov_Destinazione = BuildMov_Destinazione(Piva, 0, 0, 0,
    '                                                                Qta_New,
    '                                                                Val(dr_destinazioni("Sa_Cod")), Val(dr_destinazioni("Tipo_Destinazione")), Val(dr_destinazioni("Id_Destinazione")),
    '                                                                AGRODATAINIZIO, AGRODATAFINE,
    '                                                                objParametri)



    '                    ArrayInterno.Add(Mov_Destinazione)


    '                Next

    '            Next


    '            ''Lettura delle testate del vecchio id_agenda
    '            DT_CDG_Old = leggi.Leggi_CDG_Testata(Piva, Id_Agenda_CDG_Old, 0, "Tab_Imputazione <> 'EREDITA'", objParametri)


    '            If DT_CDG_Old.Rows.Count > 0 Then


    '                'Costruzione Testate con Qta Corrette
    '                For Each dr_cdg As DataRow In DT_CDG_Old.Rows

    '                    'Calcolo nuova Qta per la Testata New
    '                    Qta_New = dr_cdg("Qta") * Superficie_New / Superficie_Totale
    '                    Valore_Totale_New = Qta_New * dr_cdg("Prezzo_Unitario")

    '                    Data_Inizio_New = Data_Movimento_New
    '                    Data_Fine_New = DateAdd("H", CInt(Qta_New), Data_Inizio_New)


    '                    'Creazione CDG
    '                    CDG_Testata_Ext = BuildCDG(Piva, 0, 0, 0, 0, Id_Agenda_CDG,
    '                                               Data_Movimento_New, dr_cdg("Modalita_Imputazione"),
    '                                               Val(dr_cdg("Mac_Cod")), Val(dr_cdg("Cod_Risum")),
    '                                               Val(dr_cdg("Elem_Cod")), Val(dr_cdg("Pro_Cod")), Val(dr_cdg("Mat_Cod")),
    '                                               Val(dr_cdg("ID_Attivita")), Val(dr_cdg("Qualifica_Cod")), Val(dr_cdg("Tariffa_Cod")), Val(dr_cdg("Turno_Cod")),
    '                                               Val(dr_cdg("Conto_Cod")), Agro_SQL_SaveText(dr_cdg("Lotto"), False), Val(dr_cdg("Mezzo")), Val(dr_cdg("Udm_Cod")),
    '                                               Val(dr_cdg("Prezzo_Unitario")), Qta_New, Valore_Totale_New,
    '                                               dr_cdg("OrigineApp"), dr_cdg("APP_CDG_Generale_ID"),
    '                                               Agro_SQL_SaveText(dr_cdg("Descrizione"), False), dr_cdg("Tipo_Ripartizione"), dr_cdg("Budget"),
    '                                               dr_cdg("Costi_Ricavi"), dr_cdg("Modalita_Ripartizione"),
    '                                               Val(dr_cdg("Tipo_Destinazione")), Val(dr_cdg("Sa_Cod")), Val(dr_cdg("ID_Destinazione")),
    '                                               dr_cdg("Flag_Movimento_Campagna"), dr_cdg("Id_Attivita"), dr_cdg("Tab_Imputazione"), True,
    '                                               Data_Inizio_New, Data_Fine_New,
    '                                               AGRODATAINIZIO, AGRODATAFINE,
    '                                               "", "",
    '                                               "", "",
    '                                               "", "",
    '                                               "", "",
    '                                               "", "",
    '                                               objParametri)




    '                    ArrayInterno.Add(CDG_Testata_Ext)


    '                Next


    '                EFArrayToInsert.Add(ArrayInterno)



    '                'Dim transactionOptions = New TransactionOptions()
    '                'transactionOptions.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted
    '                'Using scope As New TransactionScope(TransactionScopeOption.Required, transactionOptions)


    '                'Diretto
    '                'Id_Agenda_CDG = CDG_W.Aggiorna_CDG(False, 0, Piva, Id_Agenda_CDG, 0, Des_Lib, Data_Movimento_New, EFArrayToInsert, objParametri)





    '                '    ' COMMIT Effettivo
    '                '    scope.Complete()


    '                'End Using


    '                ''Passo Il bombardino sulla operazione nuova
    '                'AllineaCostiDaCampagna_Bombardino(Piva, Id_Agenda_New, Data_Movimento_New, True, objParametri)

    '                'Fine Id_Agenda_CDG New


    '                ''==================================================================================================================================
    '                ''Sistemazione dei totali Testata ID_Agenda_CDG_Old                       

    '                'For Each dr_cdg_old As DataRow In DT_CDG_Old.Rows

    '                '    'Calcolo nuova Qta per la Testata Old
    '                '    Qta_Old = dr_cdg_old("Qta") * Superficie_Old / Superficie_Totale
    '                '    Valore_Totale_Old = Qta_Old * dr_cdg_old("Prezzo_Unitario")

    '                '    If IsDate(dr_cdg_old("Data_Ora_Inizio")) Then

    '                '        Data_Inizio_Old = dr_cdg_old("Data_Ora_Inizio")
    '                '        Data_Fine_Old = DateAdd("H", CInt(Qta_Old), Data_Inizio_Old)
    '                '    Else
    '                '        Data_Inizio_Old = Data_Movimento_Old
    '                '        Data_Fine_Old = Data_Movimento_Old

    '                '    End If

    '                '    Qta_Old = Math.Round(Qta_Old, 3)
    '                '    Valore_Totale_Old = Math.Round(Valore_Totale_Old, 2)

    '                '    CDG_W.Modifica_CDG_Testata_Split_Semina_Trapianto(Piva, Id_Agenda_CDG_Old, dr_cdg_old("Id_CDG"), Qta_Old, Data_Inizio_Old, Data_Fine_Old, Valore_Totale_Old, objParametri)


    '                'Next

    '                ''Sistemazione dei totali Dettagli e Destinazioni ID_Agenda_CDG_Old                       

    '                'For Each dr_dettagli As DataRow In DT_Movimenti_Dettagli_Old.Rows

    '                '    'Calcolo nuova Qta per la Testata Old
    '                '    Qta_Old = dr_dettagli("Qta") * Superficie_Old / Superficie_Totale

    '                '    Qta_Old = Math.Round(Qta_Old, 3)
    '                '    Qta_Old = CInt(Qta_Old)

    '                '    CDG_W.Modifica_Scarico_Split_Semina_Trapianto(Piva, Id_Agenda_CDG_Old, dr_dettagli("Id_Mov_Det"), Qta_Old, objParametri)


    '                'Next

    '                'Dim scrivi_DAL As New CDG_DAL_W

    '                'scrivi_DAL.AllineaDateRiferimento(Piva, Id_Agenda_Old, Id_Agenda_CDG_Old, DateAdd("d", -1, Data_Movimento_Old), objParametri)

    '                'scrivi_DAL.AllineaDateRiferimento(Piva, Id_Agenda_New, Id_Agenda_CDG, DateAdd("d", -1, Data_Movimento_New), objParametri)


    '                ''aggiorno il log
    '                'Dim objAgronicaLogAgendaW As New AgronicaCoreContabDAL.AgronicaLogAgenda_W
    '                'objAgronicaLogAgendaW.Scrivi(Data_Movimento_Old,
    '                '                 enum_TipoOperazioneDB.Modifica,
    '                '                         Des_Lib,
    '                '                         Id_Agenda_Old,
    '                '                         Piva,
    '                '                         0,
    '                '                         Lav_Cod,
    '                '                         CInt(enum_Id_Servizio.GiasOnline),
    '                '                         objParametri)


    '                'objAgronicaLogAgendaW = Nothing



    '                ''Passo Il bombardino sulla operazione vecchia (per allineare i dettagli)
    '                'AllineaCostiDaCampagna_Bombardino(Piva, Id_Agenda_Old, Data_Movimento_Old, True, objParametri, false)



    '                '''Passo Il bombardino sulla operazione nuova
    '                'AllineaCostiDaCampagna_Bombardino(Piva, Id_Agenda_New, Data_Movimento_New, True, objParametri)
    '                '==================================================================================================================================


    '            End If

    '        End If


    '    Catch ex As Exception


    '        MessaggioErrore = "[" & NomeRoutine & "] :   " & ex.Message
    '        Throw New Exception(MessaggioErrore)


    '        Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)

    '    Finally

    '    End Try


    '    If Not String.IsNullOrEmpty(MessaggioErrore) Then
    '        Throw New Exception(MessaggioErrore)
    '    End If

    '    Return Id_Agenda_CDG

    'End Function


    Public Function CheckErroriDettagli(ByVal Piva_CDG As String, ByVal Id_Agenda As Integer, ByVal Id_Agenda_CDG As Integer, ByVal NomeRoutine As String, ByVal bBombardino As Boolean, ByVal Operazione As String, ByRef objParametri As AgronicaCoreParametri) As String

        'Controllo Coerenza Dei Dati
        Dim ObjDettagli_Erorri As New CDG_DAL_R
        Dim Dt_Dettagli_Errori As DataTable
        Dim bOk_Errori As Boolean = True
        Dim MessaggioErrore As String = ""

        'Posso essere occorsi errori di varia natura. Decido cosa fare in base all'entità dell'errore

        'Controllo univocità id_agenda
        Dim ObjAgenda As New AgronicaCoreContabDAL.Agenda_R
        Dim DtAgenda As DataTable
        Dim xFiltroAggiuntivo = "Agenda.Piva <> '" & Piva_CDG & "'"
        DtAgenda = ObjAgenda.Leggi("", 0, Id_Agenda_CDG, 0, enumSelezioneVariabile.Selezione_TabellaDatiMinimi, xFiltroAggiuntivo, "", objParametri)
        If DtAgenda.Rows.Count > 0 Then
            'Errore Grave
            MessaggioErrore = "[" & NomeRoutine & "] " & Operazione & " " & IIf(bBombardino, "con Motorino", "") & " : Errore critico di dato duplicato. L'operazione non può essere salvata!. Errore Codice (Id_Agenda: " & Id_Agenda & " - Id_Agenda_CDG: " & Id_Agenda_CDG & "). Contattare l'Amministratore."
        End If

        If Trim(MessaggioErrore = "") Then

            'Lettura Errori
            Dt_Dettagli_Errori = ObjDettagli_Erorri.CheckCorrettezzaDettagli(Piva_CDG, Id_Agenda_CDG, objParametri)

            If Dt_Dettagli_Errori.Rows.Count > 0 Then

                'Controllo l'entità dell'errore (valore arbitrario =0,3)
                If CDbl(Dt_Dettagli_Errori.Rows(0).Item("Totale")) < 99.7 OrElse CDbl(Dt_Dettagli_Errori.Rows(0).Item("Totale")) > 100.3 Then

                    'Errore Grave
                    MessaggioErrore = "[" & NomeRoutine & "] " & Operazione & " " & IIf(bBombardino, "con Motorino", "") & " : Il totale delle percentuali salvati è errata! L'operazione non può essere salvata!. Errore Codice (Id_Agenda: " & Id_Agenda & " - Id_Agenda_CDG: " & Id_Agenda_CDG & "). Contattare l'Amministratore."


                Else

                    'Sfrido segnalo il log

                    MessaggioErrore = "[" & NomeRoutine & "] " & Operazione & " " & IIf(bBombardino, "con Motorino", "") & ": " & "SFRIDO totale " & CDbl(Dt_Dettagli_Errori.Rows(0).Item("Totale")) & "operazioni (Id_Agenda: " & Id_Agenda & " - Id_Agenda_CDG: " & Id_Agenda_CDG & ")."
                    Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)

                    'Pulisco l'errore per consentite il commit
                    MessaggioErrore = ""

                End If

            End If

        End If

        Return MessaggioErrore

    End Function




    Public Function AggiornaScaricoTempi(ByVal Piva As String, ByVal Lav_Cod As Integer,
                                         ByVal righeInseriteGrid As String, ByVal elencoVariati As String, ByVal righeCancellateGrid As String,
                                         ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri, ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri
        ) As Integer

        Dim Piva_SuperUser = objParametri_Server.PivaSuperUser

        Dim MessaggioErrore As String = String.Empty
        Dim NomeRoutine As String = "AgronicaCoreContabBIZ.CDG_BIZ.AggiornaScaricoTempi()"
        Dim CDG_Testata_Ext As New CDG_Testata
        Dim Agenda As New Agenda
        Dim Movimento As New Movimenti
        Dim Movimento_Dettaglio As New Movimenti_dettagli
        Dim Mov_Destinazione As New Mov_Destinazioni
        Dim ObjMovimenti As New AgronicaCoreContabDAL.Movimenti_R
        Dim CDG_W As New CDG_DAL_W

        Dim bOk As Boolean = False
        Dim righeArray_Testata As JArray
        Dim elencoVariatiJSON As JArray
        Dim OreTot As Double


        Dim EFArrayToInsert As New ArrayList
        Dim EFArrayToUpdate As New ArrayList
        Dim EFArrayToDelete As New ArrayList

        Dim Id_Agenda As Integer = 0
        Dim Id_CDG As Integer = 0
        Dim Risultato As Integer = 0
        Dim Id_Mov As Integer = 0
        Dim Des_Lib As String = ""
        Dim Modalita_Imputazione As Integer
        Dim Numero_CDC As Integer

        Dim bAgenda As Boolean = False
        Dim bMovimento As Boolean = False
        Dim Data_Inserimento As Date
        Dim Data_Ora_Inizio As Date
        Dim Data_Ora_Fine As Date

        'ANNA 06/10/23 COMMENTATO PER IL MOMENTO -- LASCIARE PER POSSIBILI FUTURI SVILUPPI
        'Dim isBozza As Integer

        Dim bInsert As Boolean
        Dim leggi As New CDG_DAL_R

        Dim bDettagli_Modify As Boolean
        Dim bOk_Errori As Boolean = False


        Dim ArrayInterno As ArrayList

        Dim val_SUPERUSER_UTILIZZO_ORARI_INIZIO_FINE = "1"

        bOk = False
        elencoVariatiJSON = JArray.Parse(elencoVariati)

        Try

            Dim ui_R As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read()
            Dim dt As DataTable = ui_R.Leggi(enum_Impostazioni_Utenti.SUPERUSER_UTILIZZO_ORARI_INIZIO_FINE, 2, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Utenti)

            If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then
                val_SUPERUSER_UTILIZZO_ORARI_INIZIO_FINE = dt.Rows(0).Item("Impostazione_Valore_1")
            End If

        Catch ex As Exception

        End Try

        Dim ht As New Hashtable

        Try

            Dim transactionOptions = New TransactionOptions()
            transactionOptions.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted
            Using scope As New TransactionScope(TransactionScopeOption.Required, transactionOptions)

                '###########################################################################################################################################
                '################################### NUOVI INSERIMENTI/MODIFICHE ###########################################################################
                '###########################################################################################################################################

                If righeInseriteGrid <> "" AndAlso righeInseriteGrid <> "[]" Then
                    righeArray_Testata = JArray.Parse(righeInseriteGrid)
                    bOk = True

                End If

                Dim w_counter = 0

                If bOk Then

                    For Each objRiga As JObject In righeArray_Testata

                        ' necessario perché se inserisco due nuove righe con stessi dati terrebbe solo la prima
                        w_counter += 1
                        Dim DtStr = ""
                        For Each CampoDellaRiga In objRiga

                            If val_SUPERUSER_UTILIZZO_ORARI_INIZIO_FINE = "1" AndAlso CampoDellaRiga.Key.StartsWith("Data_Ora_Tot") OrElse
                                val_SUPERUSER_UTILIZZO_ORARI_INIZIO_FINE = "0" AndAlso CampoDellaRiga.Key.StartsWith("Qta_Tot") Then

                                If Trim(CampoDellaRiga.Value) <> "" Then

                                    If CampoDellaRiga.Key.StartsWith("Data_Ora_Tot") Then
                                        DtStr = CampoDellaRiga.Key.Replace("Data_Ora_Tot", "")
                                    Else
                                        DtStr = CampoDellaRiga.Key.Replace("Qta_Tot", "")
                                    End If
                                    Dim dtId_Agenda As String = "Id_Agenda" & DtStr

                                    'Controllo valorizzazione giorno
                                    If val_SUPERUSER_UTILIZZO_ORARI_INIZIO_FINE = "1" AndAlso Format(CDate(CampoDellaRiga.Value).ToLocalTime, "HH:mm") <> "00:00" OrElse
                                            val_SUPERUSER_UTILIZZO_ORARI_INIZIO_FINE = "0" OrElse
                                       UtilityProvider.Agro_SQL_SaveNum(objRiga(dtId_Agenda)) <> 0 Then

                                        'Dim myKey = DtStr + "_" + CStr(Val(objRiga("Cod_Risum"))) + "_" + CStr(Val(objRiga("Mac_Cod"))) + "_" + CStr(Val(objRiga("Progetto_Cod"))) + "_" + CStr(Val(objRiga("Imputazione_Cod"))) + "_" + CStr(Val(objRiga("OrigineApp")))
                                        Dim myKey = DtStr & "_" & CStr(Val(w_counter))

                                        If Not ht.ContainsKey(myKey) Then
                                            Dim myObj As New ObjDaInserire
                                            Dim data = New Date(DtStr.Substring(0, 4), DtStr.Substring(4, 2), DtStr.Substring(6, 2))
                                            Dim dtOraInizioStr As String = "Data_Ora_Inizio" & DtStr
                                            Dim dtOraFineStr As String = "Data_Ora_Fine" & DtStr
                                            Dim dtOraTotStr As String = "Data_Ora_Tot" & DtStr
                                            Dim qtaTotStr As String = "Qta_Tot" & DtStr
                                            Dim dtId_CDG As String = "Id_CDG" & DtStr
                                            Dim dtModalita_Imputazione As String = "Modalita_Imputazione" & DtStr

                                            'ANNA 06/10/23 COMMENTATO PER IL MOMENTO -- LASCIARE PER POSSIBILI FUTURI SVILUPPI
                                            'Dim dtIsBozza As String = "Bozza_Cod" & DtStr


                                            If val_SUPERUSER_UTILIZZO_ORARI_INIZIO_FINE = "1" AndAlso objRiga(dtOraInizioStr).ToString <> "" OrElse
                                                val_SUPERUSER_UTILIZZO_ORARI_INIZIO_FINE = "0" AndAlso objRiga(qtaTotStr).ToString <> "" AndAlso objRiga(qtaTotStr).ToString <> "0" Then

                                                myObj.Id_Agenda = UtilityProvider.Agro_SQL_SaveNum(objRiga(dtId_Agenda))
                                                myObj.Id_CDG = UtilityProvider.Agro_SQL_SaveNum(objRiga(dtId_CDG))
                                                myObj.Modalita_Imputazione = UtilityProvider.Agro_SQL_SaveNum(objRiga(dtModalita_Imputazione))
                                                'ANNA 06/10/23 COMMENTATO PER IL MOMENTO -- LASCIARE PER POSSIBILI FUTURI SVILUPPI
                                                'myObj.isBozza = UtilityProvider.Agro_SQL_SaveNum(objRiga(dtIsBozza))

                                                'Default per origineAPP per i prodotti scaricati
                                                If Val(objRiga("OrigineApp")) = 0 Then
                                                    myObj.OrigineApp = 0
                                                    myObj.APP_CDG_Generale_ID = ""
                                                Else
                                                    myObj.OrigineApp = Val(objRiga("OrigineApp"))
                                                    myObj.APP_CDG_Generale_ID = UtilityProvider.Agro_SQL_SaveText(objRiga("APP_CDG_Generale_ID"))
                                                End If

                                                myObj.Cod_Risum = Val(objRiga("Cod_Risum"))
                                                myObj.Mac_Cod = Val(objRiga("Mac_Cod"))
                                                myObj.Id_Attivita = Val(objRiga("ID_Attivita"))
                                                myObj.attivita_poliannuale = Val(objRiga("attivita_poliannuale"))
                                                myObj.Attivita_Des = objRiga("Desc")
                                                myObj.Qualifica_Cod = Val(objRiga("Qualifica_Cod"))
                                                myObj.Tariffa_Cod = Val(objRiga("Tariffa_Cod"))
                                                myObj.Id_Imputazione = UtilityProvider.Agro_SQL_SaveNum(objRiga("Imputazione_Cod"))
                                                myObj.Linea_Cod = UtilityProvider.Agro_SQL_SaveNum(objRiga("Linea_Cod"))
                                                myObj.Raggruppamento_Cod = UtilityProvider.Agro_SQL_SaveNum(objRiga("Raggruppamento_Cod"))
                                                myObj.Cod_Animale = UtilityProvider.Agro_SQL_SaveNum(objRiga("Cod_Animale"))
                                                myObj.Cod_Animale_Distinte = UtilityProvider.Agro_SQL_SaveNum(objRiga("Cod_Animale_Distinte"))
                                                myObj.Prezzo_Unitario = Val(objRiga("Prezzo_Unitario"))
                                                myObj.Prezzo_Totale = Val(objRiga("Prezzo_Totale"))
                                                myObj.Sa_Cod = objRiga("Sa_Cod")
                                                myObj.Appezza = objRiga("Appezza")
                                                myObj.Id_Reg = objRiga("Id_Reg")
                                                myObj.Campo_Cod = UtilityProvider.Agro_SQL_SaveNum(objRiga("Campo_Cod"))
                                                myObj.Progetto_Cod = objRiga("Progetto_Cod")
                                                myObj.Rag_Soc = objRiga("Rag_Soc")
                                                myObj.Mac_Des = objRiga("Mac_Des")
                                                myObj.Macchine_Cod = Val(objRiga("Mac_Cod_Det"))
                                                myObj.Numero_CDC = Val(objRiga("Numero_CDC"))
                                                myObj.DtInizio = Format(data, "dd/MM/yyyy") & " " & Format(CDate(objRiga(dtOraInizioStr)).ToLocalTime, "HH:mm")
                                                myObj.DtFine = Format(data, "dd/MM/yyyy") & " " & Format(CDate(objRiga(dtOraFineStr)).ToLocalTime, "HH:mm")
                                                If val_SUPERUSER_UTILIZZO_ORARI_INIZIO_FINE = "1" Then
                                                    myObj.OreTot = Format(CDate(objRiga(dtOraTotStr)).ToLocalTime, "HH") + CDbl(Format(CDate(objRiga(dtOraTotStr)).ToLocalTime, "mm") / 60)
                                                Else
                                                    myObj.OreTot = objRiga(qtaTotStr)
                                                End If

                                                myObj.DtMov = data
                                                myObj.DtStr = DtStr

                                                ht.Add(myKey, myObj)

                                            End If

                                        End If

                                    End If

                                End If

                            End If

                        Next

                    Next

                    For Each obj As DictionaryEntry In ht

                        ArrayInterno = New ArrayList

                        EFArrayToInsert.Clear()
                        EFArrayToUpdate.Clear()

                        Data_Inserimento = Format(DirectCast(obj.Value, AgronicaCoreContabBIZ.ObjDaInserire).DtMov, "dd/MM/yyyy")
                        Id_Agenda = DirectCast(obj.Value, AgronicaCoreContabBIZ.ObjDaInserire).Id_Agenda
                        Id_CDG = DirectCast(obj.Value, AgronicaCoreContabBIZ.ObjDaInserire).Id_CDG
                        OreTot = DirectCast(obj.Value, AgronicaCoreContabBIZ.ObjDaInserire).OreTot
                        Modalita_Imputazione = DirectCast(obj.Value, AgronicaCoreContabBIZ.ObjDaInserire).Modalita_Imputazione
                        Data_Ora_Inizio = DirectCast(obj.Value, AgronicaCoreContabBIZ.ObjDaInserire).DtInizio
                        Data_Ora_Fine = DirectCast(obj.Value, AgronicaCoreContabBIZ.ObjDaInserire).DtFine
                        Numero_CDC = DirectCast(obj.Value, AgronicaCoreContabBIZ.ObjDaInserire).Numero_CDC

                        'ANNA 06/10/23 COMMENTATO PER IL MOMENTO -- LASCIARE PER POSSIBILI FUTURI SVILUPPI
                        'isBozza = DirectCast(obj.Value, AgronicaCoreContabBIZ.ObjDaInserire).isBozza

                        Select Case CInt(DirectCast(obj.Value, AgronicaCoreContabBIZ.ObjDaInserire).Elem_Cod)
                            Case 0 'Risorse Umane
                                Des_Lib = DirectCast(obj.Value, AgronicaCoreContabBIZ.ObjDaInserire).Rag_Soc
                            Case 1 'Parco Macchine
                                Des_Lib = DirectCast(obj.Value, AgronicaCoreContabBIZ.ObjDaInserire).Mac_Des
                        End Select

                        Des_Lib &= " (" & DirectCast(obj.Value, AgronicaCoreContabBIZ.ObjDaInserire).Attivita_Des & ")"


                        If CDate(Data_Ora_Inizio) > CDate(Data_Ora_Fine) Then
                            'Aggiungo un giorno. es data_ora_inizio = 16:00 e data_ora_fine = 12:00
                            Data_Ora_Fine = DateAdd("d", 1, Data_Ora_Fine)
                        End If

                        'Controllo Coerenza
                        If OreTot > 0 Then

                            'Controllo Inserimento/Modifica
                            bInsert = True
                            bDettagli_Modify = False

                            If Id_Agenda <> 0 Then

                                For Each objRigaVariati As JObject In elencoVariatiJSON

                                    If Id_Agenda = Val(objRigaVariati("Id_Agenda")) AndAlso
                                       Id_CDG = Val(objRigaVariati("Id_CDG")) Then

                                        If InStr(objRigaVariati("Campo"), DirectCast(obj.Value, AgronicaCoreContabBIZ.ObjDaInserire).DtStr) > 0 Then

                                            'Cambiata la testata --> controllo se è cambiato il dettaglio
                                            bInsert = False

                                        Else

                                            Select Case CStr(objRigaVariati("Campo"))

                                                Case "Imputazione_Cod", "Impianto_Cod", "Raggruppamento_Cod"

                                                    bInsert = False
                                                    bDettagli_Modify = True

                                                Case Else

                                            End Select

                                        End If

                                    End If
                                Next

                            End If

                            Select Case bInsert

                                Case True

                                    '###########################################################################################################################################
                                    '####################################### INSERIMENTO #######################################################################################
                                    '###########################################################################################################################################

                                    'Nel caso in cui 'id_agenda = 0 -> dato non modificato
                                    If Id_Agenda = 0 Then

                                        Modalita_Imputazione = 2 'Scarico Tempi

                                        'Creazione Agenda
                                        Agenda = BuildAgenda(Piva, 0, Lav_Cod, 0, Data_Inserimento, Des_Lib,
                                                     AGRODATAINIZIO, AGRODATAFINE,
                                                     objParametri_Server)

                                        ArrayInterno.Add(Agenda)

                                        'Creazione Movimento
                                        Movimento = BuildMovimento(Piva, 0, Data_Inserimento,
                                                                   AGRODATAINIZIO, AGRODATAFINE,
                                                                   objParametri_Server)

                                        ArrayInterno.Add(Movimento)

                                        'Creazione CDG
                                        CDG_Testata_Ext = BuildCDG_ScaricoTempi(Nothing, Piva, 0, Id_Agenda, 0, 0,
                                                                   Data_Inserimento, Modalita_Imputazione,
                                                                   DirectCast(obj.Value, AgronicaCoreContabBIZ.ObjDaInserire).Mac_Cod,
                                                                   DirectCast(obj.Value, AgronicaCoreContabBIZ.ObjDaInserire).Cod_Risum,
                                                                   DirectCast(obj.Value, AgronicaCoreContabBIZ.ObjDaInserire).Elem_Cod,
                                                                   0, 0,
                                                                   DirectCast(obj.Value, AgronicaCoreContabBIZ.ObjDaInserire).Id_Attivita,
                                                                   DirectCast(obj.Value, AgronicaCoreContabBIZ.ObjDaInserire).attivita_poliannuale,
                                                                   DirectCast(obj.Value, AgronicaCoreContabBIZ.ObjDaInserire).Attivita_Des,
                                                                   DirectCast(obj.Value, AgronicaCoreContabBIZ.ObjDaInserire).Qualifica_Cod,
                                                                   DirectCast(obj.Value, AgronicaCoreContabBIZ.ObjDaInserire).Tariffa_Cod, 0, 0, "", 0,
                                                                   DirectCast(obj.Value, AgronicaCoreContabBIZ.ObjDaInserire).Udm_Cod,
                                                                   DirectCast(obj.Value, AgronicaCoreContabBIZ.ObjDaInserire).Prezzo_Unitario,
                                                                   DirectCast(obj.Value, AgronicaCoreContabBIZ.ObjDaInserire).OreTot,
                                                                   DirectCast(obj.Value, AgronicaCoreContabBIZ.ObjDaInserire).Prezzo_Totale,
                                                                   DirectCast(obj.Value, AgronicaCoreContabBIZ.ObjDaInserire).OrigineApp,
                                                                   DirectCast(obj.Value, AgronicaCoreContabBIZ.ObjDaInserire).APP_CDG_Generale_ID,
                                                                   0, 0, 0, 0, 0,
                                                                   DirectCast(obj.Value, AgronicaCoreContabBIZ.ObjDaInserire).Sa_Cod, 0,
                                                                   0, 0,
                                                                   Data_Ora_Inizio, Data_Ora_Fine,
                                                                   DirectCast(obj.Value, AgronicaCoreContabBIZ.ObjDaInserire).Appezza,
                                                                   DirectCast(obj.Value, AgronicaCoreContabBIZ.ObjDaInserire).Id_Reg,
                                                                   DirectCast(obj.Value, AgronicaCoreContabBIZ.ObjDaInserire).Campo_Cod,
                                                                   0,
                                                                   DirectCast(obj.Value, AgronicaCoreContabBIZ.ObjDaInserire).Progetto_Cod,
                                                                   DirectCast(obj.Value, AgronicaCoreContabBIZ.ObjDaInserire).Id_Imputazione,
                                                                   DirectCast(obj.Value, AgronicaCoreContabBIZ.ObjDaInserire).Macchine_Cod,
                                                                   DirectCast(obj.Value, AgronicaCoreContabBIZ.ObjDaInserire).Linea_Cod,
                                                                   "", 100,
                                                                   DirectCast(obj.Value, AgronicaCoreContabBIZ.ObjDaInserire).Raggruppamento_Cod,
                                                                   DirectCast(obj.Value, AgronicaCoreContabBIZ.ObjDaInserire).Cod_Animale,
                                                                   DirectCast(obj.Value, AgronicaCoreContabBIZ.ObjDaInserire).Cod_Animale_Distinte,
                                                                   DirectCast(obj.Value, AgronicaCoreContabBIZ.ObjDaInserire).Rag_Soc,
                                                                   DirectCast(obj.Value, AgronicaCoreContabBIZ.ObjDaInserire).Mac_Des,
                                                                   AGRODATAINIZIO, AGRODATAFINE,
                                                                   objParametri_Server)
                                        'ANNA 06/10/23 COMMENTATO PER IL MOMENTO -- LASCIARE PER POSSIBILI FUTURI SVILUPPI
                                        ',isBozza:=isBozza)


                                        ArrayInterno.Add(CDG_Testata_Ext)
                                        EFArrayToInsert.Add(ArrayInterno)


                                        ' Non necessario perchè già sulla pagina non si scelgono i progetti prima delle attivita e quindi i progetti sono filtrati
                                        'If String.IsNullOrEmpty(MessaggioErrore) Then
                                        '    Dim leggi_CDG_R As New CDG_BIZ_R
                                        '    MessaggioErrore = leggi_CDG_R.ControllaCongruenzaCdCAttivita(Piva, EFArrayToInsert, objParametri)
                                        'End If

                                        If String.IsNullOrEmpty(MessaggioErrore) Then
                                            Dim leggi_CDG_R As New CDG_BIZ_R
                                            Dim msgErr = leggi_CDG_R.ControllaCongruenzaCdCAttivita(Piva, EFArrayToInsert, objParametri_Server)
                                            If Not String.IsNullOrEmpty(msgErr) AndAlso MessaggioErrore.IndexOf(msgErr) = -1 Then
                                                MessaggioErrore &= msgErr
                                            End If
                                            If String.IsNullOrEmpty(MessaggioErrore) Then
                                                Risultato = CDG_W.Aggiorna_CDG(True, Modalita_Imputazione, Piva, Id_Agenda, Id_CDG, "", Data_Inserimento, EFArrayToInsert, objParametri_Server)

                                                Select Case Risultato

                                                    Case -1 'Errore

                                                        Throw New Exception

                                                    Case Else

                                                        'Lettura Errori
                                                        MessaggioErrore = CheckErroriDettagli(Piva, Id_Agenda, Risultato, NomeRoutine, False, "Inserimento", objParametri_Server)

                                                        Select Case Trim(MessaggioErrore)

                                                            Case ""

                                                                'Ok

                                                            Case Else

                                                                bOk_Errori = False
                                                                Throw New Exception(MessaggioErrore)

                                                        End Select

                                                End Select


                                            End If

                                        End If

                                    End If

                                Case False


                                    '###########################################################################################################################################
                                    '####################################### MODIFICHE #########################################################################################
                                    '###########################################################################################################################################

                                    'Lettura Id_Mov Collegato a ID_Agenda_CDG
                                    Dim DTMovimenti = ObjMovimenti.Leggi(Piva, 0, Id_Agenda, 0, 0, CAU_SCARICO, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)
                                    Id_Mov = DTMovimenti.Rows(0).Item("Id_Mov")


                                    'Modifica CDG
                                    CDG_Testata_Ext = BuildCDG_ScaricoTempi(elencoVariatiJSON, Piva, Id_CDG, Id_Agenda, Id_Mov, 0,
                                                                   Data_Inserimento, Modalita_Imputazione,
                                                                   DirectCast(obj.Value, AgronicaCoreContabBIZ.ObjDaInserire).Mac_Cod,
                                                                   DirectCast(obj.Value, AgronicaCoreContabBIZ.ObjDaInserire).Cod_Risum,
                                                                   DirectCast(obj.Value, AgronicaCoreContabBIZ.ObjDaInserire).Elem_Cod,
                                                                   0, 0,
                                                                   DirectCast(obj.Value, AgronicaCoreContabBIZ.ObjDaInserire).Id_Attivita,
                                                                   DirectCast(obj.Value, AgronicaCoreContabBIZ.ObjDaInserire).attivita_poliannuale,
                                                                   DirectCast(obj.Value, AgronicaCoreContabBIZ.ObjDaInserire).Attivita_Des,
                                                                   DirectCast(obj.Value, AgronicaCoreContabBIZ.ObjDaInserire).Qualifica_Cod,
                                                                   DirectCast(obj.Value, AgronicaCoreContabBIZ.ObjDaInserire).Tariffa_Cod, 0, 0, "", 0,
                                                                   DirectCast(obj.Value, AgronicaCoreContabBIZ.ObjDaInserire).Udm_Cod,
                                                                   DirectCast(obj.Value, AgronicaCoreContabBIZ.ObjDaInserire).Prezzo_Unitario,
                                                                   DirectCast(obj.Value, AgronicaCoreContabBIZ.ObjDaInserire).OreTot,
                                                                   DirectCast(obj.Value, AgronicaCoreContabBIZ.ObjDaInserire).Prezzo_Totale,
                                                                   DirectCast(obj.Value, AgronicaCoreContabBIZ.ObjDaInserire).OrigineApp,
                                                                   DirectCast(obj.Value, AgronicaCoreContabBIZ.ObjDaInserire).APP_CDG_Generale_ID,
                                                                   0, 0, 0, 0, 0,
                                                                   DirectCast(obj.Value, AgronicaCoreContabBIZ.ObjDaInserire).Sa_Cod, 0,
                                                                   0, 0,
                                                                   Data_Ora_Inizio, Data_Ora_Fine,
                                                                   DirectCast(obj.Value, AgronicaCoreContabBIZ.ObjDaInserire).Appezza,
                                                                   DirectCast(obj.Value, AgronicaCoreContabBIZ.ObjDaInserire).Id_Reg,
                                                                   DirectCast(obj.Value, AgronicaCoreContabBIZ.ObjDaInserire).Campo_Cod,
                                                                   0,
                                                                   DirectCast(obj.Value, AgronicaCoreContabBIZ.ObjDaInserire).Progetto_Cod,
                                                                   DirectCast(obj.Value, AgronicaCoreContabBIZ.ObjDaInserire).Id_Imputazione,
                                                                   DirectCast(obj.Value, AgronicaCoreContabBIZ.ObjDaInserire).Macchine_Cod,
                                                                   DirectCast(obj.Value, AgronicaCoreContabBIZ.ObjDaInserire).Linea_Cod,
                                                                   "", 100,
                                                                   DirectCast(obj.Value, AgronicaCoreContabBIZ.ObjDaInserire).Raggruppamento_Cod,
                                                                   DirectCast(obj.Value, AgronicaCoreContabBIZ.ObjDaInserire).Cod_Animale,
                                                                   DirectCast(obj.Value, AgronicaCoreContabBIZ.ObjDaInserire).Cod_Animale_Distinte,
                                                                   DirectCast(obj.Value, AgronicaCoreContabBIZ.ObjDaInserire).Rag_Soc,
                                                                   DirectCast(obj.Value, AgronicaCoreContabBIZ.ObjDaInserire).Mac_Des,
                                                                   AGRODATAINIZIO, AGRODATAFINE,
                                                                   objParametri_Server)
                                    'ANNA 06/10/23 COMMENTATO PER IL MOMENTO -- LASCIARE PER POSSIBILI FUTURI SVILUPPI
                                    ',isBozza:=isBozza)


                                    ArrayInterno.Add(CDG_Testata_Ext)
                                    EFArrayToUpdate.Add(ArrayInterno)


                                    If String.IsNullOrEmpty(MessaggioErrore) Then
                                        ' Risultato = CDG_W.ModificaCDG_Testata(Piva, Id_Agenda, Id_CDG, Des_Lib, Data_Inserimento, EFArrayToUpdate, objParametri)


                                        Dim leggi_CDG_R As New CDG_BIZ_R
                                        Dim msgErr = leggi_CDG_R.ControllaCongruenzaCdCAttivita(Piva, EFArrayToUpdate, objParametri_Server)
                                        If Not String.IsNullOrEmpty(msgErr) AndAlso MessaggioErrore.IndexOf(msgErr) = -1 Then
                                            MessaggioErrore &= msgErr
                                        End If
                                        If String.IsNullOrEmpty(MessaggioErrore) Then

                                            Risultato = CDG_W.ModificaCDG_Testata(Piva, Id_Agenda, Id_CDG, Numero_CDC, bDettagli_Modify, Des_Lib, Data_Inserimento, DirectCast(obj.Value, AgronicaCoreContabBIZ.ObjDaInserire).OreTot, DirectCast(obj.Value, AgronicaCoreContabBIZ.ObjDaInserire).Prezzo_Unitario, Data_Ora_Inizio, Data_Ora_Fine, EFArrayToUpdate, objParametri_Server)
                                            'ANNA 06/10/23 COMMENTATO PER IL MOMENTO -- LASCIARE PER POSSIBILI FUTURI SVILUPPI
                                            ', isBozza:=DirectCast(obj.Value, AgronicaCoreContabBIZ.ObjDaInserire).isBozza)

                                            Select Case Risultato

                                                Case -1 'Errore

                                                    Throw New Exception

                                                Case Else

                                                    'Lettura Errori
                                                    MessaggioErrore = CheckErroriDettagli(Piva, 0, Id_Agenda, NomeRoutine, False, "Modifica", objParametri_Server)

                                                    Select Case Trim(MessaggioErrore)

                                                        Case ""

                                                            'Ok

                                                        Case Else

                                                            bOk_Errori = False
                                                            Throw New Exception(MessaggioErrore)

                                                    End Select

                                            End Select

                                        End If

                                    End If

                            End Select

                        End If

                    Next

                End If


                '###########################################################################################################################################
                '####################################### CANCELLAZIONI #####################################################################################
                '###########################################################################################################################################

                If righeCancellateGrid <> "" AndAlso righeCancellateGrid <> "[]" Then
                    righeArray_Testata = JArray.Parse(righeCancellateGrid)
                    bOk = True
                    Des_Lib = "Scarico Tempi"
                    ht.Clear()


                    For Each objRiga As JObject In righeArray_Testata
                        For Each CampoDellaRiga In objRiga

                            If CampoDellaRiga.Key.StartsWith("Data_Ora_Tot") Then

                                Dim DtStr = CampoDellaRiga.Key.Replace("Data_Ora_Tot", "")
                                Dim dtId_Agenda As String = "Id_Agenda" & DtStr

                                If UtilityProvider.Agro_SQL_SaveNum(objRiga(dtId_Agenda)) <> 0 Then

                                    Dim dtId_CDG As String = "Id_CDG" & DtStr
                                    Dim dtModalita_Imputazione As String = "Modalita_Imputazione" & DtStr
                                    Dim data = New Date(DtStr.Substring(0, 4), DtStr.Substring(4, 2), DtStr.Substring(6, 2))

                                    Dim myObj As New ObjDaInserire
                                    myObj.Id_Agenda = Val(objRiga(dtId_Agenda))
                                    myObj.Id_CDG = Val(objRiga(dtId_CDG))
                                    myObj.Modalita_Imputazione = Val(objRiga(dtModalita_Imputazione))
                                    myObj.OrigineApp = Val(objRiga("OrigineApp"))
                                    myObj.DtMov = data


                                    Dim myKey = Val(objRiga(dtId_CDG))

                                    ht.Add(myKey, myObj)

                                End If

                            End If
                        Next
                    Next

                    For Each obj As DictionaryEntry In ht

                        ArrayInterno = New ArrayList

                        EFArrayToInsert.Clear()

                        Id_Agenda = DirectCast(obj.Value, AgronicaCoreContabBIZ.ObjDaInserire).Id_Agenda
                        Id_CDG = DirectCast(obj.Value, AgronicaCoreContabBIZ.ObjDaInserire).Id_CDG
                        Modalita_Imputazione = DirectCast(obj.Value, AgronicaCoreContabBIZ.ObjDaInserire).Modalita_Imputazione
                        Data_Inserimento = Format(DirectCast(obj.Value, AgronicaCoreContabBIZ.ObjDaInserire).DtMov, "dd/MM/yyyy")


                        If String.IsNullOrEmpty(MessaggioErrore) Then
                            Risultato = CDG_W.Aggiorna_CDG(True, Modalita_Imputazione, Piva, Id_Agenda, Id_CDG, "", Data_Inserimento, EFArrayToInsert, objParametri_Server)

                            Select Case Risultato

                                Case -1 'Errore

                                    Throw New Exception

                                Case Else

                                    ' COMMIT Effettivo Rinviato

                            End Select

                        End If

                    Next

                    'Pulizia JArray
                    righeArray_Testata.Clear()

                End If

                If String.IsNullOrEmpty(MessaggioErrore) Then
                    ' COMIT Effettivo
                    scope.Complete()
                Else
                    ' Rollback
                    scope.Dispose()
                End If

            End Using

        Catch ex As Exception

            If bOk_Errori Then
                MessaggioErrore = "[" & NomeRoutine & "] : " & ex.Message
            End If

            Scrivi_LOG(objParametri_Server, NomeRoutine, MessaggioErrore)

        End Try

        If Not String.IsNullOrEmpty(MessaggioErrore) Then
            Throw New Exception(MessaggioErrore)
        End If

        Return Risultato

    End Function


    Public Function AggiornaCDG_Split(ByVal Piva As String,
                                      ByVal Data_Ricerca As Date,
                                      ByVal Data_Split As Date,
                                      ByVal Key_Padre As String,
                                      ByVal considera_tutte_attivita_annuali As Boolean,
                                      ByVal righeModificateGrid_Agenda As String,
                                      ByVal righeInseriteGrid_Dettagli_Impianti As String,
                                      ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Integer

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        Dim MessaggioErrore As String = String.Empty
        Dim NomeRoutine As String = "AgronicaCoreContabBIZ.CDG_BIZ.AggiornaCDG_Split()"
        Dim CDG_Dettaglio_Ext As New CDG_Dettagli
        Dim Dt_Testate As DataTable
        Dim Dt_Dettagli As DataTable
        Dim Dt_Distinte As DataTable
        Dim leggi_dal As New CDG_DAL_R

        Dim bOk As Boolean = False
        Dim righeArray_Agenda As JArray
        Dim righeArray_Dettaglio As JArray


        Dim EFArrayToInsert As New ArrayList
        Dim EFArrayToUpdate As New ArrayList
        Dim EFArrayToDelete As New ArrayList

        Dim htAgenda As New Hashtable
        Dim htAgenda_CDG As New Hashtable
        Dim htCDG_Testata As New Hashtable
        Dim htCDG_Dettagli_Delete As New Hashtable

        Dim Dummy As Integer
        Dim Sup_Totale As Decimal
        Dim Valore As Double
        Dim Valore_Residuo As Double
        Dim Valore_Dettaglio As Double
        Dim Valore_Totale As Double
        Dim Count As Integer

        Dim ArrayInterno As ArrayList
        Dim bAttivita_Poliannuale As Boolean
        Dim Modalita As Integer
        Dim iCount_Dettaglio As Integer


        Try

            Dim chiave As String() = Key_Padre.Split("-")
            Dim kPiva As String = CStr(chiave(0))
            Dim kSa_Cod As Integer = Integer.Parse(chiave(1))
            Dim kAppezza As Integer = Integer.Parse(chiave(3))
            Dim kId_Reg As Integer = Integer.Parse(chiave(4))
            Dim kProgetto_Cod As Integer = Integer.Parse(chiave(5))

            Dim Cod_esercizio = ""
            Dim leggi_Imprese_progetti_R As New Impresa_Progetti_R
            Dim DT_IP = leggi_Imprese_progetti_R.Leggi(kPiva, kProgetto_Cod, "", 0, kSa_Cod, kAppezza, kId_Reg, 0, 0, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri)
            If DT_IP.Rows.Count > 0 Then
                Cod_esercizio = DT_IP.Rows(0).Item("Progetto_Nome")
            End If

            Dim leggi_Reg_Impianti_Codici_R As New Reg_Impianti_Codici_R
            Dim Cod_Impianto = leggi_Reg_Impianti_Codici_R.Leggi_Codice_from_Reg_Impianti_Codici(kPiva, kSa_Cod, kAppezza, kId_Reg, enum_CodiciAnagrafe.Codice_Impianto, objParametri)
            If Cod_Impianto = Nothing Then
                Cod_Impianto = ""
            End If

            righeArray_Agenda = JArray.Parse(righeModificateGrid_Agenda)
            righeArray_Dettaglio = JArray.Parse(righeInseriteGrid_Dettagli_Impianti)

            'Calcolo Preventiva della sup totale degli impianti destinazione
            Sup_Totale = 0 'Inizializzazione
            For Each obj2 As JObject In righeArray_Dettaglio
                Sup_Totale = Sup_Totale + obj2.Item("Superficie")
            Next

            ArrayInterno = New ArrayList

            For Each obj As JObject In righeArray_Agenda

                Dim myKey0 = obj("Id_Agenda")

                If Not htAgenda.ContainsKey(myKey0) Then
                    Dim myObj0 As New ObjDaInserire
                    myObj0.Id_Agenda = obj("Id_Agenda")
                    myObj0.Lav_Cod = obj("Lav_Cod")
                    myObj0.Des_Lib = obj("Des_Lib")
                    If Cod_Impianto <> "" OrElse Cod_esercizio <> "" Then
                        myObj0.Des_Lib &= " Split da: " & Cod_Impianto & " - " & Cod_esercizio
                    End If
                    myObj0.Data_Operazione = obj("DataRegistrazione")
                    htAgenda.Add(myKey0, myObj0)

                End If

                'Lettura delle testate collegate al'Agenda con quota di ripartizione
                Dt_Testate = leggi_dal.Leggi_Testata_Da_Agenda_QDC(Piva, obj("Id_Agenda"), Key_Padre, objParametri)

                If Dt_Testate.Rows.Count > 0 Then

                    ' Forzo attività annuale anche se non lo è (introdotto per poter fare ricadere tutti i costi su un unico esercizio anche in caso di attività poliannuali)
                    If considera_tutte_attivita_annuali Then
                        bAttivita_Poliannuale = False
                    Else
                        bAttivita_Poliannuale = True 'Inizializzazione

                        'Impostazione Poliannualità
                        For Each dr_testate As DataRow In Dt_Testate.Rows

                            'Controllo Attività Poliannuale
                            If bAttivita_Poliannuale Then

                                If dr_testate.Item("Attivita_poliAnnuale") = 0 Then
                                    bAttivita_Poliannuale = False
                                End If

                            End If

                        Next
                    End If



                    For Each dr_testate As DataRow In Dt_Testate.Rows

                        Dim myKey = dr_testate.Item("Id_Agenda")

                        If Not htAgenda_CDG.ContainsKey(myKey) Then

                            Dim myObj As New ObjDaInserire
                            myObj.Id_Agenda = dr_testate.Item("Id_Agenda")
                            myObj.Des_Lib = dr_testate.Item("Des_Lib")
                            If Cod_Impianto <> "" OrElse Cod_esercizio <> "" Then
                                myObj.Des_Lib &= " Split da: " & Cod_Impianto & " - " & Cod_esercizio
                            End If
                            myObj.Data_Operazione = dr_testate.Item("Validita_Inizio_Agenda")
                            myObj.attivita_poliannuale = bAttivita_Poliannuale
                            htAgenda_CDG.Add(myKey, myObj)

                        End If

                        iCount_Dettaglio = 0
                        Valore_Totale = 0

                        For Each obj2 As JObject In righeArray_Dettaglio

                            iCount_Dettaglio += 1

                            Dim myKey2 = dr_testate.Item("Id_CDG")

                            If Not htCDG_Testata.ContainsKey(myKey2) Then
                                Dim myObj2 As New ObjDaInserire
                                myObj2.Id_CDG = dr_testate.Item("Id_CDG")
                                htCDG_Testata.Add(myKey2, myObj2)

                            End If

                            Select Case bAttivita_Poliannuale

                                Case False

                                    Modalita = 1 'Unica Distinta valida all'interno della data di split


                                Case True

                                    Modalita = 2 'Tutte le Distinte Valide Successive alla data di Split

                            End Select


                            'Lettura delle distinte associate all'impianto
                            Dt_Distinte = leggi_dal.Trova_Distinte_Per_Impianto(Piva, obj2("Key"), 0, 0, 0, Data_Ricerca, objParametri, Modalita)

                            'Ripartizione

                            'Calcolo storno per evitare problemi di arrotondamento
                            If iCount_Dettaglio < righeArray_Dettaglio.Count Then
                                Valore = Format((CDbl(obj2("Superficie")) * 100 / Sup_Totale) * (dr_testate.Item("Valore") / 100) / Dt_Distinte.Rows.Count, "##0.00")
                                Valore_Residuo = Format((CDbl(obj2("Superficie")) * 100 / Sup_Totale) * (dr_testate.Item("Valore") / 100), "##0.00")
                                Valore_Totale = Valore_Totale + Valore_Residuo
                            Else
                                Valore = Format((CDbl(dr_testate.Item("Valore")) - Valore_Totale) / Dt_Distinte.Rows.Count, "##0.00")
                                Valore_Residuo = Format(CDbl(dr_testate.Item("Valore")) - Valore_Totale, "##0.00")

                            End If


                            'Inserimento Dettaglio per ogni record di distinte
                            Count = 0

                            For Each obj3 As DataRow In Dt_Distinte.Rows

                                Count += 1

                                'Calcolo storno per evitare problemi di arrotondamento
                                If Count = Dt_Distinte.Rows.Count Then
                                    Valore = Valore_Residuo
                                Else
                                    Valore_Residuo = Valore_Residuo - Valore
                                End If

                                Valore_Dettaglio = Valore

                                If kProgetto_Cod <> Val(obj3("Progetto_Cod")) Then

                                    'Controllo Presenza Distinta in Testata
                                    Dt_Dettagli = leggi_dal.Leggi_Dettagli(Piva, Val(obj3("Sa_Cod")), Val(obj3("Appezza")), Val(obj3("Id_Destinazione")), Val(obj3("Progetto_Cod")), dr_testate.Item("Id_CDG"), objParametri)

                                    If Dt_Dettagli.Rows.Count > 0 Then

                                        For Each obj4 As DataRow In Dt_Dettagli.Rows

                                            Dim myKey4 = obj4.Item("Id_CDG_Dettagli")

                                            Dim myObj As New ObjDaInserire
                                            myObj.Id_CDG = obj4.Item("Id_CDG")
                                            myObj.Id_CDG_Dettagli = obj4.Item("Id_CDG_Dettagli")
                                            htCDG_Dettagli_Delete.Add(myKey4, myObj)

                                            'Incremento del Valore
                                            Valore_Dettaglio = Format(Valore + obj4.Item("Valore"), "##0.00")

                                        Next

                                    End If

                                End If

                                'Creazione CDG_Dettaglio
                                CDG_Dettaglio_Ext = BuildCDG_Dettaglio(Piva, dr_testate.Item("Id_CDG"), 0,
                                                                           Val(obj3("Sa_Cod")), Val(obj3("Appezza")), Val(obj3("Id_Destinazione")), 0,
                                                                           0, Val(obj3("Progetto_Cod")),
                                                                           0, 0,
                                                                           0, "",
                                                                           Valore_Dettaglio,
                                                                           AGRODATAINIZIO, AGRODATAFINE,
                                                                           0, 0,
                                                                           0, 0,
                                                                           objParametri)

                                ArrayInterno.Add(CDG_Dettaglio_Ext)

                            Next

                        Next


                    Next


                End If



                ' ArrayInterno.Clear()

            Next


            EFArrayToInsert.Add(ArrayInterno)



            If String.IsNullOrEmpty(MessaggioErrore) Then
                Dim CDG_W As New CDG_DAL_W

                Dim transactionOptions = New TransactionOptions()
                transactionOptions.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted
                Using scope As New TransactionScope(TransactionScopeOption.Required, transactionOptions)


                    Dummy = CDG_W.Aggiorna_CDG_Split(Piva, EFArrayToInsert, htAgenda, htAgenda_CDG, htCDG_Testata, htCDG_Dettagli_Delete, kProgetto_Cod, Data_Split, objParametri)

                    ' COMIT Effettivo
                    scope.Complete()

                End Using
            End If

        Catch ex As Exception

            MessaggioErrore = "[" & NomeRoutine & "] : " & ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)

        Finally

        End Try


        If Not String.IsNullOrEmpty(MessaggioErrore) Then
            Throw New Exception(MessaggioErrore)
        End If

        Return 1

    End Function



    Public Function BuildCDG(ByVal Piva As String, ByVal ID_CDG As Integer,
                             ByVal Id_Agenda As Integer, ByVal Id_Mov As Integer, ByVal Id_Mov_Det As Integer,
                             ByVal Id_Agenda_CDG As Integer, ByVal Data_Inserimento As Date, ByVal Modalita_Imputazione As Integer,
                             ByVal Mac_Cod As Integer, ByVal Cod_Risum As Integer,
                             ByVal Elem_Cod As Integer, ByVal Pro_Cod As Integer, ByVal Mat_Cod As Integer,
                             ByVal Id_Attivita As Integer, ByVal Qualifica_Cod As Integer, ByVal Tariffa_Cod As Integer, ByVal Turno_Cod As Integer,
                             ByVal Conto_Cod As Integer, ByVal Lotto As String, ByVal Mezzo As Integer, ByVal Udm_Cod As Integer,
                             ByVal Prezzo_Unitario As Decimal, ByVal Qta As Decimal, ByVal Valore_Totale As Decimal,
                             ByVal OrigineApp As Integer, ByVal APP_CDG_Generale_ID As String,
                             ByVal descrizione As String, ByVal Tipo_Ripartizione As Integer, ByVal Budget As Integer,
                             ByVal Costi_Ricavi As Integer, ByVal Modalita_Ripartizione As Integer,
                             ByVal Tipo_Destinazione As Integer, ByVal Sa_Cod As Integer, ByVal Id_Destinazione As Integer,
                             ByVal Flag_Movimento_Campagna As Integer, ByVal Lav_Cod As Integer, ByVal Id_Attivita_Batch As Integer, Tab_Imputazione As String, ByVal ModifInAutom As Integer,
                             ByVal Data_Ora_Inizio As Date, ByVal Data_Ora_Fine As Date,
                             ByVal Validita_Inizio As Date, ByVal Validita_Fine As Date,
                             ByVal righeInseriteGrid_Dettagli_Impianti As String, ByVal righeModificateGrid_Dettagli_Impianti As String,
                             ByVal righeInseriteGrid_Dettagli_Progetti As String, ByVal righeModificateGrid_Dettagli_Progetti As String,
                             ByVal righeInseriteGrid_Dettagli_Macchine As String, ByVal righeModificateGrid_Dettagli_Macchine As String,
                             ByVal righeInseriteGrid_Dettagli_Linee As String, ByVal righeModificateGrid_Dettagli_Linee As String,
                             ByVal righeInseriteGrid_Dettagli_Zoo As String, ByVal righeModificateGrid_Dettagli_Zoo As String,
                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                             ) As CDG_Testata
        'ANNA 06/10/23 COMMENTATO PER IL MOMENTO -- LASCIARE PER POSSIBILI FUTURI SVILUPPI
        'Optional ByVal isBozza As Integer = 0


        Dim Piva_SuperUser = objParametri.PivaSuperUser

        Dim MessaggioErrore As String = String.Empty

        Dim NomeRoutine As String = "AgronicaCoreContabBIZ.CDG_BIZ.BuildCDG()"

        Dim CDG_Testata_Ext As New CDG_Testata
        Dim CDG_Dettaglio_Ext As New CDG_Dettagli

        Dim Count_Dettaglio As Integer
        Dim bOk As Boolean = False

        Dim righeArray_Dettaglio As JArray

        Dim residuo As Decimal
        Dim Campo As String
        Dim Valore As Decimal
        Dim i As Integer
        Dim Arrayp(3, 0) As String
        Dim bCheckCampo As Boolean
        Dim Indice As Integer
        Dim bDuplicato As Boolean
        Dim Campo_Cod As Integer
        Dim leggi_impianti As New CDG_DAL_R
        Dim Dt_Impianti As DataTable
        Dim Count As Integer
        Dim residuo_check As Decimal


        Try

            ReDim Arrayp(3, 0)

            CDG_Testata_Ext.OrigineApp = OrigineApp
            CDG_Testata_Ext.APP_CDG_Generale_ID = APP_CDG_Generale_ID

            CDG_Testata_Ext.Piva_Superuser = Piva_SuperUser
            CDG_Testata_Ext.Piva = Piva
            CDG_Testata_Ext.Id_CDG = ID_CDG

            CDG_Testata_Ext.Data_Inserimento = Data_Inserimento
            CDG_Testata_Ext.Modalita_Imputazione = Modalita_Imputazione
            CDG_Testata_Ext.Id_Agenda = Id_Agenda
            CDG_Testata_Ext.Id_Mov = Id_Mov
            CDG_Testata_Ext.Id_Mov_Det = Id_Mov_Det
            CDG_Testata_Ext.Mac_Cod = Mac_Cod
            CDG_Testata_Ext.Cod_RisUm = Cod_Risum
            CDG_Testata_Ext.Elem_Cod = Elem_Cod
            CDG_Testata_Ext.Pro_Cod = Pro_Cod
            CDG_Testata_Ext.Mat_Cod = Mat_Cod
            CDG_Testata_Ext.Lav_Cod = Lav_Cod

            'ANNA 06/10/23 COMMENTATO PER IL MOMENTO -- LASCIARE PER POSSIBILI FUTURI SVILUPPI
            'CDG_Testata_Ext.Bozza = isBozza 

            Select Case CInt(Id_Attivita_Batch)

                Case 0

                    CDG_Testata_Ext.Id_Attivita = Id_Attivita

                Case Else

                    If Id_Agenda_CDG = 0 Then

                        CDG_Testata_Ext.Id_Attivita = Id_Attivita

                    Else

                        Select Case UCase(Tab_Imputazione)

                            Case "EREDITA"

                                'Id Attività da batch inserimento costi mancanti da QDC
                                ' Stefano 21/11 - se sono in modifica e prodotto / magazzino sono rimasti gli stessi l'ho già assegnato a monte
                                If CDG_Testata_Ext.Id_Attivita = 0 Then
                                    CDG_Testata_Ext.Id_Attivita = Id_Attivita
                                End If

                            Case Else

                                CDG_Testata_Ext.Id_Attivita = Id_Attivita

                        End Select

                    End If

            End Select

            CDG_Testata_Ext.ModifInAutom = ModifInAutom

            CDG_Testata_Ext.Tab_Imputazione = Tab_Imputazione

            CDG_Testata_Ext.Qualifica_Cod = Qualifica_Cod
            CDG_Testata_Ext.Tariffa_Cod = Tariffa_Cod
            CDG_Testata_Ext.Turno_Cod = Turno_Cod
            CDG_Testata_Ext.Conto_Cod = Conto_Cod
            CDG_Testata_Ext.Lotto = Lotto
            CDG_Testata_Ext.Mezzo = Mezzo
            CDG_Testata_Ext.Udm_Cod = Udm_Cod
            CDG_Testata_Ext.Prezzo_Unitario = Prezzo_Unitario
            CDG_Testata_Ext.Qta = Qta
            CDG_Testata_Ext.Valore_Totale = Prezzo_Unitario * Qta ' Valore_Totale
            CDG_Testata_Ext.Descrizione = descrizione
            CDG_Testata_Ext.Tipo_Ripartizione = Tipo_Ripartizione
            CDG_Testata_Ext.Budget = Budget
            CDG_Testata_Ext.Costi_Ricavi = Costi_Ricavi
            CDG_Testata_Ext.Modalita_Ripartizione = Modalita_Ripartizione
            CDG_Testata_Ext.Vecchio_Tipo_Inser_Dati = 0

            CDG_Testata_Ext.Tipo_Destinazione = Tipo_Destinazione
            CDG_Testata_Ext.Sa_Cod = Sa_Cod
            CDG_Testata_Ext.Id_Destinazione = Id_Destinazione

            CDG_Testata_Ext.Flag_Movimento_Campagna = Flag_Movimento_Campagna

            If Data_Ora_Inizio <> AGRODATAINIZIO Then
                CDG_Testata_Ext.Data_Ora_Inizio = Data_Ora_Inizio
            End If

            If Data_Ora_Fine <> AGRODATAFINE Then
                CDG_Testata_Ext.Data_Ora_Fine = Data_Ora_Fine
            End If

            CDG_Testata_Ext.Validita_Inizio = Validita_Inizio
            CDG_Testata_Ext.Validita_Fine = Validita_Fine

            CDG_Testata_Ext.Data_Creazione = Date.Now
            CDG_Testata_Ext.Username_Creazione = objParametri.UsernameOperazione
            CDG_Testata_Ext.Data_Modifica = Date.Now
            CDG_Testata_Ext.Username_Modifica = objParametri.UsernameOperazione
            CDG_Testata_Ext.inviato = 0

            For Count_Dettaglio = 0 To 9

                bOk = False

                Select Case Count_Dettaglio
                    Case 0
                        If righeInseriteGrid_Dettagli_Impianti <> "" Then
                            righeArray_Dettaglio = JArray.Parse(righeInseriteGrid_Dettagli_Impianti)
                            bOk = True
                            bCheckCampo = True
                        End If
                    Case 1
                        If righeModificateGrid_Dettagli_Impianti <> "" Then
                            righeArray_Dettaglio = JArray.Parse(righeModificateGrid_Dettagli_Impianti)
                            bOk = True
                            bCheckCampo = True
                        End If
                    Case 2
                        If righeInseriteGrid_Dettagli_Progetti <> "" Then
                            righeArray_Dettaglio = JArray.Parse(righeInseriteGrid_Dettagli_Progetti)
                            bOk = True
                            bCheckCampo = False
                        End If
                    Case 3
                        If righeModificateGrid_Dettagli_Progetti <> "" Then
                            righeArray_Dettaglio = JArray.Parse(righeModificateGrid_Dettagli_Progetti)
                            bOk = True
                            bCheckCampo = False
                        End If
                    Case 4
                        If righeInseriteGrid_Dettagli_Macchine <> "" Then
                            righeArray_Dettaglio = JArray.Parse(righeInseriteGrid_Dettagli_Macchine)
                            bOk = True
                            bCheckCampo = False
                        End If
                    Case 5
                        If righeModificateGrid_Dettagli_Macchine <> "" Then
                            righeArray_Dettaglio = JArray.Parse(righeModificateGrid_Dettagli_Macchine)
                            bOk = True
                            bCheckCampo = False
                        End If
                    Case 6
                        If righeInseriteGrid_Dettagli_Linee <> "" Then
                            righeArray_Dettaglio = JArray.Parse(righeInseriteGrid_Dettagli_Linee)
                            bOk = True
                            bCheckCampo = False
                        End If
                    Case 7
                        If righeModificateGrid_Dettagli_Linee <> "" Then
                            righeArray_Dettaglio = JArray.Parse(righeModificateGrid_Dettagli_Linee)
                            bOk = True
                            bCheckCampo = False
                        End If
                    Case 8
                        If righeInseriteGrid_Dettagli_Zoo <> "" AndAlso righeInseriteGrid_Dettagli_Zoo <> "[]" Then
                            righeArray_Dettaglio = JArray.Parse(righeInseriteGrid_Dettagli_Zoo)
                            bOk = True
                            bCheckCampo = False
                        End If
                    Case 9
                        If righeModificateGrid_Dettagli_Zoo <> "" AndAlso righeModificateGrid_Dettagli_Zoo <> "[]" Then
                            righeArray_Dettaglio = JArray.Parse(righeModificateGrid_Dettagli_Zoo)
                            bOk = True
                            bCheckCampo = False
                        End If
                End Select


                'Dettaglio Insert
                If bOk Then

                    residuo = 100
                    residuo_check = 100
                    Valore = 0
                    i = 0

                    For Each obj As JObject In righeArray_Dettaglio

                        Select Case Count_Dettaglio

                            Case 8, 9 'Zoo

                                'Controllo la modalità di inserimento
                                If Val(obj("Cod_Animale_Distinte")) <> 0 Then

                                    'Inserimento da modulo riccardo
                                    Valore = obj("Valore")
                                    Campo = "Cod_Animale_Distinte"

                                ElseIf IsNumeric(obj("Cod_Progetto")) Then

                                    'Inserimento Inserimento Costi da modulo CDG --> Ripartizione Valore
                                    Campo = "Cod_Progetto"

                                    'Determinazione Valore Ripartito
                                    Valore = Math.Round(100 / righeArray_Dettaglio.Count, 2)
                                    residuo_check = Math.Round(residuo_check - Valore, 2)

                                    If residuo_check > Valore AndAlso i < righeArray_Dettaglio.Count - 1 Then

                                        'Aggiornamento Residuo
                                        residuo = Math.Round(residuo - Valore, 2)

                                    ElseIf i < righeArray_Dettaglio.Count - 1 Then

                                        'Ripastisco il residuo ripartita da qui in avanti
                                        Valore = Math.Round(residuo / (righeArray_Dettaglio.Count - i), 2)
                                        residuo = Math.Round(residuo - Valore, 2)

                                    Else

                                        'Ultimo
                                        If residuo > 0 Then
                                            Valore = residuo
                                        Else
                                            Valore = 0
                                        End If

                                        residuo = 0

                                    End If

                                    i += 1

                                Else

                                    MessaggioErrore = "La matricola " & CStr(obj("Matricola")) & " non è valida alla data di registrazione "

                                    Throw New Exception(MessaggioErrore)


                                End If

                            Case Else

                                Valore = obj("Valore")
                                Campo = "Cod_Animale_Distinte"

                        End Select


                        'Controllo impostazione percentuale valida
                        Campo_Cod = UtilityProvider.Agro_SQL_SaveNum(obj("Campo_Cod"))
                        If Valore > 0 Then

                            Campo_Cod = 0
                            Count = 0

                            If bCheckCampo AndAlso UtilityProvider.Agro_SQL_SaveNum(obj("Campo_Cod")) <> 0 Then

                                'Controllo Presenza in Struttura di Appoggio
                                bDuplicato = False

                                For Indice = 0 To UBound(Arrayp, 2) - 1

                                    If Arrayp(0, Indice) = Val(obj("Sa_Cod")) AndAlso
                                       Arrayp(1, Indice) = UtilityProvider.Agro_SQL_SaveNum(obj("Campo_Cod")) Then

                                        bDuplicato = True
                                        Campo_Cod = Arrayp(2, Indice)

                                        Exit For

                                    End If
                                Next

                                If Not bDuplicato Then

                                    ReDim Preserve Arrayp(3, UBound(Arrayp, 2) + 1)

                                    Arrayp(0, UBound(Arrayp, 2) - 1) = obj("Sa_Cod")
                                    Arrayp(1, UBound(Arrayp, 2) - 1) = obj("Campo_Cod")

                                    'Lettura Impianti Collegati al Campo
                                    Dt_Impianti = leggi_impianti.Leggi_Impianti_Da_Campo(Piva, obj("Sa_Cod"), obj("Campo_Cod"), Data_Inserimento, objParametri)

                                    bOk = True
                                    If Dt_Impianti.Rows.Count > 0 Then

                                        For Each dr_impianti As DataRow In Dt_Impianti.Rows

                                            If bOk Then

                                                'Controllo se l'impianto è dentro il json
                                                bOk = False

                                                For Each obj2 As JObject In righeArray_Dettaglio

                                                    If CInt(obj2("Appezza")) = CInt(dr_impianti.Item("Appezza")) AndAlso
                                                       CInt(obj2("Id_Destinazione")) = CInt(dr_impianti.Item("Id_Reg")) AndAlso
                                                       Val(obj2("Valore")) > 0 Then

                                                        bOk = True
                                                        Exit For

                                                    End If


                                                Next
                                            Else
                                                Exit For

                                            End If

                                        Next


                                    End If


                                    If bOk Then

                                        Arrayp(2, UBound(Arrayp, 2) - 1) = obj("Campo_Cod")

                                    Else

                                        Arrayp(2, UBound(Arrayp, 2) - 1) = 0

                                    End If

                                    Campo_Cod = Arrayp(2, Indice)

                                End If

                            End If


                            CDG_Dettaglio_Ext = BuildCDG_Dettaglio(Piva, ID_CDG, Val(obj("ID_CDG_Dettagli")),
                                                           Val(obj("Sa_Cod")), Val(obj("Appezza")), Val(obj("Id_Destinazione")), Campo_Cod,
                                                           Val(obj("Id_Cod_reg_impianti_codici")), Val(obj("Progetto_Cod")),
                                                           Val(obj("Imputazione_Cod")), Val(obj("Mac_Cod")),
                                                           Val(obj("Linea_Cod")), Agro_SQL_SaveText(obj("Lotto_Input_Costi"), False),
                                                           Valore,
                                                           AGRODATAINIZIO, AGRODATAFINE,
                                                           Val(obj("STA_NUM")), Val(obj("Raggruppamento_Cod")),
                                                           Val(obj("Cod_Animale")), Val(obj(CStr(Campo))),
                                                           objParametri)

                            CDG_Testata_Ext.CDG_Dettagli.Add(CDG_Dettaglio_Ext)

                        End If

                    Next

                    'Pulizia JArray
                    righeArray_Dettaglio.Clear()

                End If

            Next


        Catch ex As Exception
            MessaggioErrore = "[" & NomeRoutine & "] : " & ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
        Finally

        End Try


        If Not String.IsNullOrEmpty(MessaggioErrore) Then
            Throw New Exception(MessaggioErrore)
        End If

        Return CDG_Testata_Ext

    End Function




    Public Function BuildCDG_ScaricoTempi(ByVal elencoVariatiJSON As JArray, ByVal Piva As String, ByVal ID_CDG As Integer,
                                          ByVal Id_Agenda As Integer, ByVal Id_Mov As Integer, ByVal Id_Mov_Det As Integer,
                                          ByVal Data_Inserimento As Date, ByVal Modalita_Imputazione As Integer,
                                          ByVal Mac_Cod As Integer, ByVal Cod_Risum As Integer,
                                          ByVal Elem_Cod As Integer, ByVal Pro_Cod As Integer, ByVal Mat_Cod As Integer,
                                          ByVal Id_Attivita As Integer, ByVal attPoliennale As Boolean, ByVal Attivita_Des As String,
                                          ByVal Qualifica_Cod As Integer, ByVal Tariffa_Cod As Integer, ByVal Turno_Cod As Integer,
                                          ByVal Conto_Cod As Integer, ByVal Lotto As String, ByVal Mezzo As Integer, ByVal Udm_Cod As Integer,
                                          ByVal Prezzo_Unitario As Decimal, ByVal Qta As Decimal, ByVal Valore_Totale As Decimal,
                                          ByVal OrigineApp As Integer, ByVal APP_CDG_Generale_ID As String,
                                          ByVal Tipo_Ripartizione As Integer, ByVal Budget As Integer,
                                          ByVal Costi_Ricavi As Integer, ByVal Modalita_Ripartizione As Integer,
                                          ByVal Tipo_Destinazione As Integer, ByVal Sa_Cod As Integer, ByVal Id_Destinazione As Integer,
                                          ByVal Flag_Movimento_Campagna As Integer, ByVal Id_Attivita_Batch As Integer,
                                          ByVal Data_Ora_Inizio As Date, ByVal Data_Ora_Fine As Date,
                                          ByVal Appezza As Integer, ByVal Id_Reg As Integer, ByVal Campo_Cod As Integer,
                                          ByVal Id_Cod_reg_impianti_codici As Integer, ByVal Progetto_Cod As Integer,
                                          ByVal Id_Imputazione As Integer, ByVal Macchine_Cod As Integer,
                                          ByVal Linea_Cod As Integer, ByVal Lotto_Input_Costi As String,
                                          ByVal Valore As Decimal,
                                          ByVal Raggruppamento_Cod As Integer,
                                          ByVal Cod_Animale As Integer, Cod_Animale_Distinta As Integer,
                                          ByVal Rag_Soc As String, ByVal Mac_Des As String,
                                          ByVal Validita_Inizio As Date, ByVal Validita_Fine As Date,
                                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                          ) As CDG_Testata
        'ANNA 06/10/23 COMMENTATO PER IL MOMENTO -- LASCIARE PER POSSIBILI FUTURI SVILUPPI
        'Optional isBozza As Integer = 0


        Dim Piva_SuperUser = objParametri.PivaSuperUser

        Dim MessaggioErrore As String = String.Empty

        Dim NomeRoutine As String = "AgronicaCoreContabBIZ.CDG_BIZ.BuildCDG_ScaricoTempi()"

        Dim CDG_Testata_Ext As New CDG_Testata
        Dim CDG_Dettaglio_Ext As New CDG_Dettagli

        Dim Tab_Imputazione As String

        Dim bOk As Boolean = False
        Dim Count As Integer
        Dim Valore_Storno As Double
        Dim Superficie As Double
        Dim Superficie_Totale As Double



        Try

            CDG_Testata_Ext.OrigineApp = OrigineApp
            CDG_Testata_Ext.APP_CDG_Generale_ID = APP_CDG_Generale_ID

            CDG_Testata_Ext.Piva_Superuser = Piva_SuperUser
            CDG_Testata_Ext.Piva = Piva
            CDG_Testata_Ext.Id_CDG = ID_CDG

            CDG_Testata_Ext.Data_Inserimento = Data_Inserimento
            CDG_Testata_Ext.Modalita_Imputazione = Modalita_Imputazione
            CDG_Testata_Ext.Id_Agenda = Id_Agenda
            CDG_Testata_Ext.Id_Mov = Id_Mov
            CDG_Testata_Ext.Id_Mov_Det = Id_Mov_Det
            CDG_Testata_Ext.Mac_Cod = Mac_Cod
            CDG_Testata_Ext.Cod_RisUm = Cod_Risum

            CDG_Testata_Ext.Id_Attivita = Id_Attivita
            CDG_Testata_Ext.ModifInAutom = 0

            If Cod_Risum <> 0 Then
                'Risorse Umane
                Elem_Cod = 0
                Tab_Imputazione = "MANODOPERA"
                CDG_Testata_Ext.Descrizione = Rag_Soc
            ElseIf Mac_Cod <> 0 Then
                'Parco Macchine
                Elem_Cod = 1
                Tab_Imputazione = "MACCHINE"
                CDG_Testata_Ext.Descrizione = Mac_Des
            End If

            CDG_Testata_Ext.Elem_Cod = Elem_Cod
            CDG_Testata_Ext.Pro_Cod = Pro_Cod
            CDG_Testata_Ext.Mat_Cod = Mat_Cod

            CDG_Testata_Ext.Tab_Imputazione = Tab_Imputazione

            CDG_Testata_Ext.Qualifica_Cod = Qualifica_Cod
            CDG_Testata_Ext.Tariffa_Cod = Tariffa_Cod
            CDG_Testata_Ext.Turno_Cod = Turno_Cod
            CDG_Testata_Ext.Conto_Cod = Conto_Cod
            CDG_Testata_Ext.Lotto = Lotto
            CDG_Testata_Ext.Mezzo = Mezzo
            CDG_Testata_Ext.Udm_Cod = 141 'Ora
            CDG_Testata_Ext.Prezzo_Unitario = Prezzo_Unitario
            CDG_Testata_Ext.Qta = Qta
            CDG_Testata_Ext.Valore_Totale = Prezzo_Unitario * Qta

            CDG_Testata_Ext.Tipo_Ripartizione = Tipo_Ripartizione
            CDG_Testata_Ext.Budget = Budget
            CDG_Testata_Ext.Costi_Ricavi = Costi_Ricavi
            CDG_Testata_Ext.Modalita_Ripartizione = Modalita_Ripartizione
            CDG_Testata_Ext.Vecchio_Tipo_Inser_Dati = 0

            CDG_Testata_Ext.Tipo_Destinazione = Tipo_Destinazione
            CDG_Testata_Ext.Sa_Cod = Sa_Cod
            CDG_Testata_Ext.Id_Destinazione = Id_Destinazione

            CDG_Testata_Ext.Flag_Movimento_Campagna = Flag_Movimento_Campagna

            If Data_Ora_Inizio <> AGRODATAINIZIO Then
                CDG_Testata_Ext.Data_Ora_Inizio = Data_Ora_Inizio
            End If

            If Data_Ora_Fine <> AGRODATAFINE Then
                CDG_Testata_Ext.Data_Ora_Fine = Data_Ora_Fine
            End If

            CDG_Testata_Ext.Validita_Inizio = Validita_Inizio
            CDG_Testata_Ext.Validita_Fine = Validita_Fine

            CDG_Testata_Ext.Data_Creazione = Date.Now
            CDG_Testata_Ext.Username_Creazione = objParametri.UsernameOperazione
            CDG_Testata_Ext.Data_Modifica = Date.Now
            CDG_Testata_Ext.Username_Modifica = objParametri.UsernameOperazione
            CDG_Testata_Ext.inviato = 0

            Select Case Raggruppamento_Cod

                Case 0

                    Select Case Campo_Cod

                        Case 0



                            'Se non sono in modifica e attività poliennale spalmo su tutte gli eventuali esercizi
                            'In modifica invece conservo le scelte iniziali dell'utente a meno che non sia variato l'impianto
                            Dim cercaEsercizi As Boolean = attPoliennale

                            If elencoVariatiJSON IsNot Nothing AndAlso ID_CDG > 0 AndAlso attPoliennale Then

                                Dim variatoImpianto = False
                                For Each objRigaVariati As JObject In elencoVariatiJSON

                                    If Id_Agenda = Val(objRigaVariati("Id_Agenda")) AndAlso ID_CDG = Val(objRigaVariati("Id_CDG")) Then

                                        Select Case CStr(objRigaVariati("Campo"))

                                            Case "Impianto_Cod"

                                                variatoImpianto = True
                                                Exit For

                                            Case Else

                                        End Select

                                    End If
                                Next
                                If Not variatoImpianto Then
                                    cercaEsercizi = False
                                End If
                            End If

                            If cercaEsercizi Then
                                Dim leggi_distinta As New Impresa_Progetti_R
                                Dim distinteSuCuiSpalmare = leggi_distinta.Trova_Distinte_Attive(Piva,
                                                          Sa_Cod, Appezza, Id_Reg, Data_Inserimento, objParametri)
                                Dim residuo As Decimal = Valore
                                Dim valPerDistinta As Decimal = 0
                                If distinteSuCuiSpalmare.Rows.Count > 0 Then
                                    For i As Integer = 0 To distinteSuCuiSpalmare.Rows.Count - 1

                                        If i = distinteSuCuiSpalmare.Rows.Count - 1 Then
                                            valPerDistinta = residuo
                                        Else
                                            valPerDistinta = Math.Round(100 / distinteSuCuiSpalmare.Rows.Count, 2)
                                        End If

                                        residuo = residuo - valPerDistinta

                                        CDG_Dettaglio_Ext = BuildCDG_Dettaglio(Piva, ID_CDG, 0,
                                                              Sa_Cod, Appezza, Id_Reg, Campo_Cod,
                                                              Id_Cod_reg_impianti_codici, distinteSuCuiSpalmare.Rows(i).Item("Progetto_Cod"),
                                                              Id_Imputazione, Macchine_Cod,
                                                              Linea_Cod, Lotto_Input_Costi,
                                                              valPerDistinta,
                                                              AGRODATAINIZIO, AGRODATAFINE,
                                                              0, 0,
                                                              Cod_Animale, Cod_Animale_Distinta,
                                                              objParametri)

                                        CDG_Testata_Ext.CDG_Dettagli.Add(CDG_Dettaglio_Ext)



                                    Next

                                End If
                            Else

                                CDG_Dettaglio_Ext = BuildCDG_Dettaglio(Piva, ID_CDG, 0,
                                                          Sa_Cod, Appezza, Id_Reg, Campo_Cod,
                                                          Id_Cod_reg_impianti_codici, Progetto_Cod,
                                                          Id_Imputazione, Macchine_Cod,
                                                          Linea_Cod, Lotto_Input_Costi,
                                                          Valore,
                                                          AGRODATAINIZIO, AGRODATAFINE,
                                                          0, 0,
                                                          Cod_Animale, Cod_Animale_Distinta,
                                                          objParametri)

                                CDG_Testata_Ext.CDG_Dettagli.Add(CDG_Dettaglio_Ext)
                            End If



                        Case Else


                            'Lettura Impianti appartenenti al Campo
                            Dim leggi_impianti As New CDG_DAL_R
                            Dim Dt_Impianti As DataTable
                            Dt_Impianti = leggi_impianti.Leggi_Impianti_Da_Campo(Piva, Sa_Cod, Campo_Cod, Data_Inserimento, objParametri)
                            Superficie_Totale = 0

                            If Dt_Impianti.Rows.Count > 0 Then

                                For Each dr_impianti As DataRow In Dt_Impianti.Rows

                                    Superficie_Totale = Superficie_Totale + dr_impianti.Item("Sup_Imp")
                                Next


                                Count = 0

                                For Each dr_impianti As DataRow In Dt_Impianti.Rows

                                    Count += 1

                                    If Count = Dt_Impianti.Rows.Count Then
                                        Valore = 100 - Valore_Storno
                                    Else
                                        Valore = Format(dr_impianti.Item("Sup_Imp") / Superficie_Totale * 100, "##0.00")
                                        Valore_Storno = Valore_Storno + Valore
                                    End If

                                    CDG_Dettaglio_Ext = BuildCDG_Dettaglio(Piva, ID_CDG, 0,
                                                              Sa_Cod, dr_impianti.Item("Appezza"), dr_impianti.Item("Id_Reg"), Campo_Cod,
                                                              Id_Cod_reg_impianti_codici, dr_impianti.Item("Progetto_Cod"),
                                                              Id_Imputazione, Macchine_Cod,
                                                              Linea_Cod, Lotto_Input_Costi,
                                                              Valore,
                                                              AGRODATAINIZIO, AGRODATAFINE,
                                                              0, 0,
                                                              0, 0,
                                                              objParametri)

                                    CDG_Testata_Ext.CDG_Dettagli.Add(CDG_Dettaglio_Ext)

                                Next

                            Else

                                'Eccezione
                                'Throw New Exception(MessaggioErrore)

                            End If





                    End Select


                Case Else




                    'Lettura Animali appartenenti al raggruppamento
                    Dim leggi As New AgronicaCoreAnagrafeDAL.Zoo_Animali
                    Dim Dt_Consistenze As DataTable
                    Dt_Consistenze = leggi.Leggi_Giacenze(Piva, 0, 0, Raggruppamento_Cod, 0, Data_Inserimento, objParametri)

                    If Dt_Consistenze.Rows.Count > 0 Then

                        Count = 0

                        For Each dr_consistenze As DataRow In Dt_Consistenze.Rows

                            Count += 1

                            If Count = Dt_Consistenze.Rows.Count Then
                                Valore = 100 - Valore_Storno
                            Else
                                Valore = Format(100 / Dt_Consistenze.Rows.Count, "##0.00")
                                Valore_Storno = Valore_Storno + Valore
                            End If

                            CDG_Dettaglio_Ext = BuildCDG_Dettaglio(Piva, ID_CDG, 0,
                                                      Sa_Cod, Appezza, Id_Reg, Campo_Cod,
                                                      Id_Cod_reg_impianti_codici, Progetto_Cod,
                                                      Id_Imputazione, Macchine_Cod,
                                                      Linea_Cod, Lotto_Input_Costi,
                                                      Valore,
                                                      AGRODATAINIZIO, AGRODATAFINE,
                                                      dr_consistenze.Item("Sta_Num"), dr_consistenze.Item("Raggruppamento_Cod"),
                                                      dr_consistenze("Cod_Animale"), dr_consistenze.Item("Cod_Progetto"),
                                                      objParametri)

                            CDG_Testata_Ext.CDG_Dettagli.Add(CDG_Dettaglio_Ext)

                        Next

                    Else

                        'Eccezione
                        Throw New Exception("Non esistono consistenze zootecniche nel recinto scelto alla data " & Data_Inserimento.ToShortDateString)

                    End If

            End Select

            'ANNA 06/10/23 COMMENTATO PER IL MOMENTO -- LASCIARE PER POSSIBILI FUTURI SVILUPPI
            'CDG_Testata_Ext.Bozza = isBozza

        Catch ex As Exception
            MessaggioErrore = "[" & NomeRoutine & "] :   " & ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
        Finally

        End Try


        If Not String.IsNullOrEmpty(MessaggioErrore) Then
            Throw New Exception(MessaggioErrore)
        End If

        Return CDG_Testata_Ext

    End Function



    'Public Function BuildCDG_ScaricoTempi_Modifica(ByVal Piva As String, ByVal ID_CDG As Integer,
    '                                                 ByVal Id_Agenda As Integer,
    '                                                 ByVal Data_Inserimento As Date, ByVal Modalita_Imputazione As Integer,
    '                                                 ByVal Mac_Cod As Integer, ByVal Cod_Risum As Integer,
    '                                                 ByVal Elem_Cod As Integer, ByVal Pro_Cod As Integer, ByVal Mat_Cod As Integer,
    '                                                 ByVal Id_Attivita As Integer, ByVal Attivita_Des As String, ByVal Qualifica_Cod As Integer, ByVal Tariffa_Cod As Integer, ByVal Turno_Cod As Integer,
    '                                                 ByVal Conto_Cod As Integer, ByVal Lotto As String, ByVal Mezzo As Integer, ByVal Udm_Cod As Integer,
    '                                                 ByVal Prezzo_Unitario As Decimal, ByVal Qta As Decimal, ByVal Valore_Totale As Decimal,
    '                                                 ByVal Tipo_Ripartizione As Integer, ByVal Budget As Integer,
    '                                                 ByVal Costi_Ricavi As Integer, ByVal Modalita_Ripartizione As Integer,
    '                                                 ByVal Flag_Movimento_Campagna As Integer, ByVal Id_Attivita_Batch As Integer,
    '                                                 ByVal Data_Ora_Inizio As Date, ByVal Data_Ora_Fine As Date,
    '                                                 ByVal Rag_Soc As String, ByVal Mac_Des As String,
    '                                                 ByVal Validita_Inizio As Date, ByVal Validita_Fine As Date,
    '                                                 ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
    '    ) As CDG_Testata

    '    Dim Piva_SuperUser = objParametri.PivaSuperUser

    '    Dim MessaggioErrore As String = String.Empty

    '    Dim NomeRoutine As String = "AgronicaCoreContabBIZ.CDG_BIZ.BuildCDG_ScaricoTempi_Modifica()"

    '    Dim CDG_Testata_Ext As New CDG_Testata

    '    Dim bOk As Boolean = False
    '    Dim Tab_Imputazione As String


    '    Try

    '        CDG_Testata_Ext.Piva_Superuser = Piva_SuperUser
    '        CDG_Testata_Ext.Piva = Piva
    '        CDG_Testata_Ext.Id_CDG = ID_CDG

    '        CDG_Testata_Ext.Data_Inserimento = Data_Inserimento
    '        CDG_Testata_Ext.Modalita_Imputazione = Modalita_Imputazione
    '        CDG_Testata_Ext.Id_Agenda = Id_Agenda
    '        CDG_Testata_Ext.Id_Mov = 0
    '        CDG_Testata_Ext.Id_Mov_Det = 0
    '        CDG_Testata_Ext.Mac_Cod = Mac_Cod
    '        CDG_Testata_Ext.Cod_RisUm = Cod_Risum
    '        CDG_Testata_Ext.Elem_Cod = Elem_Cod
    '        CDG_Testata_Ext.Pro_Cod = Pro_Cod
    '        CDG_Testata_Ext.Mat_Cod = Mat_Cod
    '        CDG_Testata_Ext.Id_Attivita = Id_Attivita
    '        CDG_Testata_Ext.ModifInAutom = 0

    '        Select Case Elem_Cod
    '            Case 0 'Risorse Umane
    '                Tab_Imputazione = "MANODOPERA"
    '                CDG_Testata_Ext.Descrizione = Rag_Soc & " (" & Attivita_Des & ")"
    '            Case 1 'Parco Macchine
    '                Tab_Imputazione = "MACCHINE"
    '                CDG_Testata_Ext.Descrizione = Mac_Des & " (" & Attivita_Des & ")"
    '        End Select

    '        CDG_Testata_Ext.Tab_Imputazione = Tab_Imputazione

    '        CDG_Testata_Ext.Qualifica_Cod = Qualifica_Cod
    '        CDG_Testata_Ext.Tariffa_Cod = Tariffa_Cod
    '        CDG_Testata_Ext.Turno_Cod = Turno_Cod
    '        CDG_Testata_Ext.Conto_Cod = Conto_Cod
    '        CDG_Testata_Ext.Lotto = Lotto
    '        CDG_Testata_Ext.Mezzo = Mezzo
    '        CDG_Testata_Ext.Udm_Cod = 141
    '        CDG_Testata_Ext.Prezzo_Unitario = Prezzo_Unitario
    '        CDG_Testata_Ext.Qta = Qta
    '        CDG_Testata_Ext.Valore_Totale = Valore_Totale

    '        CDG_Testata_Ext.Tipo_Ripartizione = Tipo_Ripartizione
    '        CDG_Testata_Ext.Budget = Budget
    '        CDG_Testata_Ext.Costi_Ricavi = Costi_Ricavi
    '        CDG_Testata_Ext.Modalita_Ripartizione = Modalita_Ripartizione
    '        CDG_Testata_Ext.Vecchio_Tipo_Inser_Dati = 0

    '        CDG_Testata_Ext.Tipo_Destinazione = 0
    '        CDG_Testata_Ext.Sa_Cod = 0
    '        CDG_Testata_Ext.Id_Destinazione = 0

    '        CDG_Testata_Ext.Flag_Movimento_Campagna = Flag_Movimento_Campagna

    '        If Data_Ora_Inizio <> AGRODATAINIZIO Then
    '            CDG_Testata_Ext.Data_Ora_Inizio = Data_Ora_Inizio
    '        End If

    '        If Data_Ora_Fine <> AGRODATAFINE Then
    '            CDG_Testata_Ext.Data_Ora_Fine = Data_Ora_Fine
    '        End If

    '        CDG_Testata_Ext.Validita_Inizio = Validita_Inizio
    '        CDG_Testata_Ext.Validita_Fine = Validita_Fine

    '        CDG_Testata_Ext.Data_Creazione = Date.Now
    '        CDG_Testata_Ext.Username_Creazione = objParametri.UsernameOperazione
    '        CDG_Testata_Ext.Data_Modifica = Date.Now
    '        CDG_Testata_Ext.Username_Modifica = objParametri.UsernameOperazione
    '        CDG_Testata_Ext.inviato = 0

    '    Catch ex As Exception
    '        MessaggioErrore = "[" & NomeRoutine & "] :   " & ex.Message
    '        Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
    '    Finally

    '    End Try


    '    If Not String.IsNullOrEmpty(MessaggioErrore) Then
    '        Throw New Exception(MessaggioErrore)
    '    End If

    '    Return CDG_Testata_Ext

    'End Function



    Public Function BuildAgenda(ByVal Piva As String, ByVal Id_Agenda As Integer, ByVal Lav_Cod As Integer,
                                ByVal Split As Integer,
                                ByVal Data_Inserimento As String, ByVal Des_Lib As String,
                                ByVal Validita_Inizio As Date, ByVal Validita_Fine As Date,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
        ) As Agenda

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        Dim MessaggioErrore As String = String.Empty

        Dim NomeRoutine As String = "AgronicaCoreContabBIZ.CDG_BIZ.BuildMovimento()"

        Dim Agenda As New Agenda

        Try

            Agenda.PIVA = Piva
            Agenda.Sa_Cod = 0
            Agenda.Id_Agenda = Id_Agenda
            Agenda.des_lib = Des_Lib
            Agenda.Lav_Cod = Lav_Cod
            Agenda.LINEA_COD = 0
            Agenda.PREPARAZIONE_COD = 0
            Agenda.ID_TRASFORMAZIONE = 0
            Agenda.Tipo_Accettazione = 0
            Agenda.Blocco_Flag = 1
            Agenda.Blocco_Data = Data_Inserimento
            Agenda.Blocco_Username = objParametri.UsernameOperazione

            Agenda.Split = Split

            Agenda.Validita_Inizio = Data_Inserimento
            Agenda.Validita_Fine = Validita_Fine

            Agenda.Data_Creazione = Date.Now
            Agenda.Username_Creazione = objParametri.UsernameOperazione
            Agenda.Data_Modifica = Date.Now
            Agenda.Username_Modifica = objParametri.UsernameOperazione
            Agenda.inviato = 0



        Catch ex As Exception
            MessaggioErrore = "[" & NomeRoutine & "] : " & ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
        Finally

        End Try


        If Not String.IsNullOrEmpty(MessaggioErrore) Then
            Throw New Exception(MessaggioErrore)
        End If

        Return Agenda

    End Function




    Public Function BuildMovimento(ByVal Piva As String, Id_Agenda As Integer, Data_Movimento As Date,
                                       ByVal Validita_Inizio As Date, ByVal Validita_Fine As Date,
                                       ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
        ) As Movimenti

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        Dim MessaggioErrore As String = String.Empty

        Dim NomeRoutine As String = "AgronicaCoreContabBIZ.CDG_BIZ.BuildMovimento()"

        Dim Movimento As New Movimenti

        Try

            Movimento.PIVA = Piva
            Movimento.Sa_Cod = 0
            Movimento.Id_Agenda = Id_Agenda
            Movimento.Id_Mov = 0

            Movimento.Cod_RisUm = 0
            Movimento.Cau_Mov = CAU_SCARICO
            Movimento.Mov_Desc = ""
            Movimento.Data_Movimento = Data_Movimento
            Movimento.Scadenza = AGRODATAFINE
            Movimento.Doc_Numero = 0
            Movimento.Num_Protocollo = 0
            Movimento.Cod_IndirizzoRisUm = 0
            Movimento.Cod_Destinazione = 0
            Movimento.Cod_IndirizzoDestinazione = 0
            Movimento.Mezzo = 0
            Movimento.Cod_Vettore = 0
            Movimento.Cod_IndirizzoVettore = 0
            Movimento.Causale_Trasporto = ""
            Movimento.Aspetto = ""
            Movimento.Peso = 0
            Movimento.Ora = Data_Movimento
            Movimento.Colli = 0
            Movimento.Extra_Str = ""
            Movimento.Extra_Int = 0
            Movimento.Extra_Date = AGRODATAFINE
            Movimento.Tipo_Sconto = 0
            Movimento.Doc_Numero_Des = ""
            Movimento.Natura_Beni = ""
            Movimento.Tara_Veicolo = 0
            Movimento.Tara_Imballi = 0
            Movimento.Tipo_Peso = 0
            Movimento.Modalita = 0
            Movimento.Username_Note = ""
            Movimento.Scadenza_Extra = AGRODATAFINE
            Movimento.Doc_Numero_Sin = ""
            Movimento.Progr_Protocollo = 0
            Movimento.Progr_Registrazione = 0
            Movimento.Data_Registrazione = Data_Movimento
            Movimento.ChkLayOut_Bypass_Fatturato = 0
            Movimento.ChkLayOut_Join_Prodotti = 0
            Movimento.Cod_RisUm_Altro = 0
            Movimento.ChkLayOut_Peso = 0
            Movimento.ChkLayOut_Prezzo = 0
            Movimento.ChkFiltro_Varietale = 0
            Movimento.Disciplinare_PubblicoPrivato = 0
            Movimento.Sezionale_Cod = 0
            Movimento.Causale_Trasporto_Cod = 0
            Movimento.ChkLayOut_Litri = 0
            Movimento.Cod_RisUm_Aggiuntivo = 0
            Movimento.Cod_Indirizzo_Aggiuntivo = 0
            Movimento.ChkLayOut_Riscontrato = 0


            Movimento.Validita_Inizio = Validita_Inizio
            Movimento.Validita_Fine = Validita_Fine

            Movimento.Data_Creazione = Date.Now
            Movimento.Username_Creazione = objParametri.UsernameOperazione
            Movimento.Data_Modifica = Date.Now
            Movimento.Username_Modifica = objParametri.UsernameOperazione
            Movimento.inviato = 0



        Catch ex As Exception
            MessaggioErrore = "[" & NomeRoutine & "] : " & ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
        Finally

        End Try


        If Not String.IsNullOrEmpty(MessaggioErrore) Then
            Throw New Exception(MessaggioErrore)
        End If

        Return Movimento

    End Function



    Public Function BuildMov_Dettagli_Riferimenti(ByVal Piva As String, ByVal Piva_Rif As String, ByVal Lav_Cod As Integer,
                                                  ByVal Id_Agenda As Integer, ByVal Id_Mov As Integer, ByVal Id_Mov_Det As Integer,
                                                  ByVal Validita_Inizio As Date, ByVal Validita_Fine As Date,
                                                  ByVal Data_Creazione As DateTime,
                                                  ByRef objParametri As AgronicaCoreParametri
                                                  ) As Mov_Dettagli_Riferimenti

        Dim MessaggioErrore As String = String.Empty

        Dim NomeRoutine As String = "AgronicaCoreContabBIZ.CDG_BIZ.BuildMov_Dettagli_Riferimenti()"

        Dim Mov_Dettagli_Riferimenti As New Mov_Dettagli_Riferimenti

        Try

            Mov_Dettagli_Riferimenti.Piva = Piva
            Mov_Dettagli_Riferimenti.Sa_Cod = 0
            Mov_Dettagli_Riferimenti.Id_Agenda = Id_Agenda
            Mov_Dettagli_Riferimenti.Id_Mov = Id_Mov
            Mov_Dettagli_Riferimenti.Id_Mov_Det = Id_Mov_Det
            Mov_Dettagli_Riferimenti.Lav_Cod = Lav_Cod
            Mov_Dettagli_Riferimenti.Cau_Mov = CAU_IMPUTAZIONE_UTILIZZO_PRODOTTI

            If Trim(Piva_Rif = "") Then
                Mov_Dettagli_Riferimenti.Piva_Rif = Piva
            Else
                Mov_Dettagli_Riferimenti.Piva_Rif = Piva_Rif
            End If

            Mov_Dettagli_Riferimenti.Sa_Cod_Rif = 0
            Mov_Dettagli_Riferimenti.Id_Agenda_Rif = 0
            Mov_Dettagli_Riferimenti.Id_Mov_Rif = 0
            Mov_Dettagli_Riferimenti.Id_Mov_Det_Rif = 0
            Mov_Dettagli_Riferimenti.Lav_Cod_Rif = LAVCOD_COSTI_CDG
            Mov_Dettagli_Riferimenti.Cau_Mov_Rif = CAU_SCARICO
            Mov_Dettagli_Riferimenti.Qta = 0
            Mov_Dettagli_Riferimenti.Preserva_Legame = 0
            Mov_Dettagli_Riferimenti.Tipo_Associazione = 0


            Mov_Dettagli_Riferimenti.validita_inizio = Validita_Inizio
            Mov_Dettagli_Riferimenti.validita_fine = Validita_Fine

            Mov_Dettagli_Riferimenti.data_creazione = Data_Creazione
            Mov_Dettagli_Riferimenti.username_creazione = objParametri.UsernameOperazione
            Mov_Dettagli_Riferimenti.data_modifica = Data_Creazione
            Mov_Dettagli_Riferimenti.username_modifica = objParametri.UsernameOperazione
            Mov_Dettagli_Riferimenti.inviato = 0



        Catch ex As Exception
            MessaggioErrore = "[" & NomeRoutine & "] : " & ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
        Finally

        End Try


        If Not String.IsNullOrEmpty(MessaggioErrore) Then
            Throw New Exception(MessaggioErrore)
        End If

        Return Mov_Dettagli_Riferimenti

    End Function





    Public Function BuildCDG_Dettaglio(ByVal Piva As String, ByVal ID_CDG As Integer, ByVal ID_CDG_Dettagli As Integer,
                                       ByVal Sa_Cod As Integer, ByVal Appezza As Integer, ByVal Id_Reg As Integer, ByVal Campo_Cod As Integer,
                                       ByVal Id_Cod_reg_impianti_codici As Integer, ByVal Progetto_Cod As Integer,
                                       ByVal Id_Imputazione As Integer, ByVal Macchine_Cod As Integer,
                                       ByVal Linea_Cod As Integer, ByVal Lotto_Input_Costi As String,
                                       ByVal Valore As Decimal,
                                       ByVal Validita_Inizio As Date, ByVal Validita_Fine As Date,
                                       ByVal Sta_Num As Integer?, ByVal Raggruppamento_Cod As Integer?,
                                       ByVal Cod_Animale As Integer?, Cod_Animale_Distinta As Integer?,
                                       ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As CDG_Dettagli

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        Dim MessaggioErrore As String = String.Empty

        Dim NomeRoutine As String = "AgronicaCoreContabBIZ.CDG_BIZ.BuildCDG_Dettaglio()"

        Dim CDG_Testata_Dettaglio As New CDG_Dettagli

        Try

            CDG_Testata_Dettaglio.Piva_Superuser = Piva_SuperUser
            CDG_Testata_Dettaglio.Piva = Piva
            CDG_Testata_Dettaglio.Id_CDG = ID_CDG
            CDG_Testata_Dettaglio.Id_CDG_Dettagli = ID_CDG_Dettagli

            CDG_Testata_Dettaglio.Sa_Cod = Sa_Cod
            CDG_Testata_Dettaglio.Appezza = Appezza
            CDG_Testata_Dettaglio.Id_Reg = Id_Reg
            CDG_Testata_Dettaglio.Campo_Cod = Campo_Cod
            CDG_Testata_Dettaglio.Id_Cod_reg_impianti_codici = Id_Cod_reg_impianti_codici
            CDG_Testata_Dettaglio.Progetto_Cod = Progetto_Cod
            CDG_Testata_Dettaglio.Id_Imputazione = Id_Imputazione
            CDG_Testata_Dettaglio.Macchine_Cod = Macchine_Cod
            CDG_Testata_Dettaglio.Linea_Cod = Linea_Cod
            CDG_Testata_Dettaglio.Lotto_Input_Costi = Lotto_Input_Costi
            CDG_Testata_Dettaglio.Valore = Valore

            CDG_Testata_Dettaglio.Validita_Inizio = Validita_Inizio
            CDG_Testata_Dettaglio.Validita_Fine = Validita_Fine

            If Sta_Num Is Nothing Then
                CDG_Testata_Dettaglio.Sta_Num = 0
            Else
                CDG_Testata_Dettaglio.Sta_Num = Sta_Num
            End If

            If Raggruppamento_Cod Is Nothing Then
                CDG_Testata_Dettaglio.Raggruppamento_Cod = 0
            Else
                CDG_Testata_Dettaglio.Raggruppamento_Cod = Raggruppamento_Cod
            End If

            If Cod_Animale Is Nothing Then
                CDG_Testata_Dettaglio.Cod_Animale = 0
            Else
                CDG_Testata_Dettaglio.Cod_Animale = Cod_Animale
            End If

            If Cod_Animale_Distinta Is Nothing Then
                CDG_Testata_Dettaglio.Cod_Animale_Distinta = 0
            Else
                CDG_Testata_Dettaglio.Cod_Animale_Distinta = Cod_Animale_Distinta
            End If

            'Correzione Bug: viene salvato l'id<_reg nel caso di imputazione zoo
            If CDG_Testata_Dettaglio.Cod_Animale_Distinta <> 0 And CDG_Testata_Dettaglio.Cod_Animale <> 0 Then
                CDG_Testata_Dettaglio.Id_Reg = 0
            End If


            CDG_Testata_Dettaglio.Data_Creazione = Date.Now
            CDG_Testata_Dettaglio.Username_Creazione = objParametri.UsernameOperazione
            CDG_Testata_Dettaglio.Data_Modifica = Date.Now
            CDG_Testata_Dettaglio.Username_Modifica = objParametri.UsernameOperazione
            CDG_Testata_Dettaglio.inviato = 0



        Catch ex As Exception
            MessaggioErrore = "[" & NomeRoutine & "] : " & ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
        Finally

        End Try


        If Not String.IsNullOrEmpty(MessaggioErrore) Then
            Throw New Exception(MessaggioErrore)
        End If

        Return CDG_Testata_Dettaglio

    End Function


    Public Function BuildMovimento_Dettaglio(ByVal Piva As String, ByVal Id_Agenda As Integer,
                                             ByVal Id_Mov As Integer, ByVal Id_Mov_Det As Integer,
                                             ByVal Elem_Cod As Integer, ByVal Pro_Cod As Integer, ByVal Mat_Cod As Integer,
                                             ByVal Lotto As String, ByVal Udm_Cod As Integer,
                                             ByVal Qta As Decimal, ByVal Sa_Cod As Integer,
                                             ByVal Validita_Inizio As Date, ByVal Validita_Fine As Date,
                                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                             Optional ByVal budget As Integer = 0
                                             ) As Movimenti_dettagli

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        Dim MessaggioErrore As String = String.Empty

        Dim NomeRoutine As String = "AgronicaCoreContabBIZ.CDG_BIZ.BuildCDG_Dettaglio()"

        Dim Movimento_Dettaglio As New Movimenti_dettagli
        Dim Mov_Destinazione As New Mov_Destinazioni

        Try

            Movimento_Dettaglio.PIVA = Piva
            Movimento_Dettaglio.Sa_Cod = Sa_Cod
            Movimento_Dettaglio.Id_Agenda = Id_Agenda
            Movimento_Dettaglio.Id_Mov = Id_Mov
            Movimento_Dettaglio.Id_Mov_Det = Id_Mov_Det
            Movimento_Dettaglio.Elem_Cod = Elem_Cod
            Movimento_Dettaglio.Pro_Cod = Pro_Cod
            Movimento_Dettaglio.Mat_Cod = Mat_Cod
            Movimento_Dettaglio.Mov_Det_Des = ""
            Movimento_Dettaglio.Udm_Cod = Udm_Cod
            Movimento_Dettaglio.Qta = Qta
            Movimento_Dettaglio.Cod_Iva = 0
            Movimento_Dettaglio.Sconto = 0

            Movimento_Dettaglio.Prezzo_Unitario = 0
            Movimento_Dettaglio.Cod_Conto = 0
            Movimento_Dettaglio.Cod_Progetto = 0
            Movimento_Dettaglio.Fase_Cod = 0
            Movimento_Dettaglio.Contabilizzato = 0
            Movimento_Dettaglio.Pendente = 4

            Movimento_Dettaglio.Validita_Inizio = Validita_Inizio
            Movimento_Dettaglio.Validita_Fine = Validita_Fine

            Movimento_Dettaglio.Data_Creazione = Date.Now
            Movimento_Dettaglio.Username_Creazione = objParametri.UsernameOperazione
            Movimento_Dettaglio.Data_Modifica = Date.Now
            Movimento_Dettaglio.Username_Modifica = objParametri.UsernameOperazione
            Movimento_Dettaglio.inviato = 0

            Movimento_Dettaglio.Cal_Cod = 0
            Movimento_Dettaglio.Extra_Str = 0
            Movimento_Dettaglio.Extra_Int = 0
            Movimento_Dettaglio.Extra_Date = Date.Now
            Movimento_Dettaglio.Anno = Year(Date.Now)
            Movimento_Dettaglio.Ric_Cod = 0
            Movimento_Dettaglio.Imponibile = 0
            Movimento_Dettaglio.Iva = 0
            Movimento_Dettaglio.Lotto = Lotto

            'Definizione Movimentazione Magazzino
            Select Case budget
                Case 0
                    Movimento_Dettaglio.Jolly_Int = enum_TipoMovimentazioneMagazzino.MagazzinoMovimentato
                Case Else
                    Movimento_Dettaglio.Jolly_Int = enum_TipoMovimentazioneMagazzino.MagazzinoNONMovimentato
            End Select

            Movimento_Dettaglio.Imponibile_Netto = 0
            Movimento_Dettaglio.Prezzo_Unitario_Netto = 0
            Movimento_Dettaglio.UDM_COD_EXTRA = 0
            Movimento_Dettaglio.QTA_EXTRA = 0
            Movimento_Dettaglio.Prezzo_Effettivo = 0
            Movimento_Dettaglio.ChkIva_Manuale = 0
            Movimento_Dettaglio.Qta_Extra_Totale = 0

            Movimento_Dettaglio.Cod_IvaIndetraibile = 0
            Movimento_Dettaglio.Tara = 0
            Movimento_Dettaglio.ChkLayOut_Hide = 0
            Movimento_Dettaglio.Variazione = 0
            Movimento_Dettaglio.Listino_Cod = 0
            Movimento_Dettaglio.Sconto_Listino = 0
            Movimento_Dettaglio.Mat_Cod_Alias = 0
            Movimento_Dettaglio.Sconto_Modalita = 0

            Movimento_Dettaglio.Mezzo_Det = 0
            Movimento_Dettaglio.Sconto_Testo = 0
            Movimento_Dettaglio.Ric_Cod_Pat = 0
            Movimento_Dettaglio.Cod_Conto_Pat = 0
            Movimento_Dettaglio.TempoCarenza = 0
            Movimento_Dettaglio.DoseEtichetta = 0
            Movimento_Dettaglio.Turno_Cod = 0

            Movimento_Dettaglio.ID_Attivita = 0
            Movimento_Dettaglio.Dettaglio_VegCod = 0
            Movimento_Dettaglio.Iva_Indetraibile = 0
            Movimento_Dettaglio.Iva_Indetraibile_Perc = 0
            Movimento_Dettaglio.PrincipiAttivi = 0
            Movimento_Dettaglio.ClassiTossicologiche = 0
            Movimento_Dettaglio.DoseEtichetta_Value = 0
            Movimento_Dettaglio.Iva_Deto_Cod = 0
            Movimento_Dettaglio.Qta_Dettaglio1 = 0
            Movimento_Dettaglio.Qta_Dettaglio2 = 0
            Movimento_Dettaglio.Dettagli_Blocco_Flag = 0
            Movimento_Dettaglio.Dettagli_Blocco_Username = ""
            Movimento_Dettaglio.Dettagli_Blocco_Data = AGRODATAINIZIO
            Movimento_Dettaglio.Qualifica_Cod = 0
            Movimento_Dettaglio.Tariffa_Cod = 0
            Movimento_Dettaglio.Ordine_Det = 0
            Movimento_Dettaglio.Deroga_Cod = 0
            Movimento_Dettaglio.Prezzo_Livello = 0
            Movimento_Dettaglio.Polverulento = 0


        Catch ex As Exception
            MessaggioErrore = "[" & NomeRoutine & "] : " & ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
        Finally

        End Try


        If Not String.IsNullOrEmpty(MessaggioErrore) Then
            Throw New Exception(MessaggioErrore)
        End If

        Return Movimento_Dettaglio

    End Function



    Public Function BuildMov_Destinazione(ByVal Piva As String, ByVal Id_Agenda As Integer,
                                          ByVal Id_Mov As Integer, ByVal Id_Mov_Det As Integer,
                                          ByVal Qta As Decimal, ByVal Sa_Cod As Integer,
                                          ByVal Tipo_Destinazione As Integer, ByVal Id_Destinazione As String,
                                          ByVal Validita_Inizio As Date, ByVal Validita_Fine As Date,
                                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                          ) As Mov_Destinazioni

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        Dim MessaggioErrore As String = String.Empty

        Dim NomeRoutine As String = "AgronicaCoreContabBIZ.CDG_BIZ.BuildMov_Destinazione()"

        Dim Mov_Destinazione As New Mov_Destinazioni

        Try

            Mov_Destinazione.Piva = Piva
            Mov_Destinazione.Sa_Cod = Sa_Cod
            Mov_Destinazione.Id_Agenda = Id_Agenda
            Mov_Destinazione.Id_Mov = Id_Mov
            Mov_Destinazione.Id_Mov_Det = Id_Mov_Det
            Mov_Destinazione.Appezza = 0
            Mov_Destinazione.Id_Destinazione = Id_Destinazione
            Mov_Destinazione.Tipo_Destinazione = 20 'Tipo_Destinazione
            Mov_Destinazione.Qta = Qta
            Mov_Destinazione.Qta2 = 0
            Mov_Destinazione.Tipo_Scorta = 0
            Mov_Destinazione.Scorta_Min = 0
            Mov_Destinazione.mov_destinazioni_graphickey = 0

            Mov_Destinazione.Qta_Dest1 = 0
            Mov_Destinazione.Qta_Dest2 = 0
            Mov_Destinazione.QuotaDistribuzione = 0

            Mov_Destinazione.validita_inizio = Validita_Inizio
            Mov_Destinazione.validita_fine = Validita_Fine

            Mov_Destinazione.data_creazione = Date.Now
            Mov_Destinazione.username_creazione = objParametri.UsernameOperazione
            Mov_Destinazione.data_modifica = Date.Now
            Mov_Destinazione.username_modifica = objParametri.UsernameOperazione
            Mov_Destinazione.inviato = 0


        Catch ex As Exception
            MessaggioErrore = "[" & NomeRoutine & "] : " & ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
        Finally

        End Try


        If Not String.IsNullOrEmpty(MessaggioErrore) Then
            Throw New Exception(MessaggioErrore)
        End If

        Return Mov_Destinazione

    End Function


    Public Function AllineaCostiDaCampagna_Bombardino(ByVal Piva As String,
                                                      ByVal Id_Agenda As Integer,
                                                      ByVal Data_Movimento As Date,
                                                      ByVal bForzato As Boolean,
                                                      ByVal objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                      ByVal objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                      Optional ByRef GiasContext As AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities = Nothing,
                                                      Optional ByVal OpenNewTransaction As Boolean = True,
                                                      Optional ByRef listaErrori As List(Of ErroreGias) = Nothing,
                                                      Optional ByVal bBombardinoMultiplo As Boolean = False
                                                      ) As String


        Dim MessaggioErrore As String = ""
        Dim Raccoglitore_Cod As Integer = 0

        Try

            'Creazione OggettiF
            Dim leggi As New CDG_DAL_R
            Dim leggi_DW As New DW_CDG_Costi_Ricavi_DAL_R
            Dim scrivi_BIZ As New CDG_BIZ_W
            Dim scrivi_DW As New DW_CDG_Costi_Ricavi_DAL_W
            Dim scrivi_DAL As New CDG_DAL_W
            Dim objQualifichexTariffe As New AgronicaCoreContabDAL.QualificheXTariffe_R
            Dim objAttivita As New AgronicaCoreContabDAL.AttivitaXOperazioni_R
            Dim ObjOperazione As New AgronicaCoreMetaSchemaDAL.Operazioni_R
            Dim leggi_Attivita As New Attivita_R
            Dim dt As DataTable
            Dim dt_agenda As DataTable
            Dim dt_impianti_agenda As DataTable
            Dim dt_impianti_cdg As DataTable
            Dim dt_eredita_agenda As DataTable
            Dim dt_eredita_cdg As DataTable
            Dim dt_Attivita As DataTable

            Dim Id_Agenda_CDG As Integer
            Dim Id_Attivita As Integer
            Dim Attivita_Des As String = ""
            Dim Costi_Ricavi As Integer
            Dim Modalita_Imputazione As Integer
            Dim strFiltro As String
            Dim kendo_Impianti As String = ""
            Dim kendo_Zoo As String = ""
            Dim kendo_Manodopera As String = ""
            Dim kendo_Terzisti As String = ""
            Dim kendo_Macchine As String = ""
            Dim kendo_Magazzino As String = ""
            Dim kendo_Libera As String = ""
            Dim kendo_Eredita As String = ""

            Dim bUguali As Boolean
            Dim strFiltro_Confronto As String = ""
            Dim ht_Id_Attivita_CDG As New Hashtable
            Dim Lav_Cod As Integer = 0
            Dim Lav_Cod_CDG As Integer = 0
            Dim dr_impianti_agenda() As DataRow


            Dim bFirst As Boolean = True
            Dim bInsertCDG As Boolean = False
            Dim Id_Attivita_Eredita_Base As Integer = 0
            Dim Des_Lib As String = ""
            Dim Data_Inserimento_Da_Util As Date
            Dim Kendo_Eredita_Totale_str As String = ""
            Dim attPoliennale = False
            Dim attAnnuale = False
            Dim dtAtt As New DataTable
            Dim drAtt() As DataRow
            Dim bProdottoTrovato As Boolean = False
            Dim Arrayp() As String

            Dim Lav_Des As String = ""
            Dim Lavorazione_Des As String = ""
            Dim DtRipartizione As DataTable
            Dim TotaleRipartizione As Decimal = 0
            Dim bFirstRipartizione As Boolean = True
            Dim bMulticentro As Boolean = False
            Dim Sa_Cod_Last As Integer = 0
            Dim bZoo As Boolean = False

            'Verifico che l'operazione di agenda non sia parte di un raccoglitore oppure è la prima operazione in ordine di chiave di un raccoglitore
            Raccoglitore_Cod = leggi.VerificaCorrettezzaAgendaRaccoglitore(Piva, Id_Agenda, objParametri_Server)

            If Raccoglitore_Cod <> -1 Then

                'Raccoglitore--> i costi vengono agganciati arbitrariamente alla prima agenda del raccoglitore
                Costi_Ricavi = 0

                'Impostazione Filtro
                strFiltro = "Agenda.Id_Agenda = " & Id_Agenda

                'Lettura Id_Agenda
                bUguali = False

                dt_agenda = leggi.Leggi_Agenda_Riferimento(Piva, Id_Agenda, 0, True, objParametri_Server, GiasContext)

                If dt_agenda.Rows.Count > 0 Then

                    'riferimento presente
                    Id_Agenda_CDG = dt_agenda.Rows(0).Item("Id_Agenda_Rif")
                    Lav_Cod = dt_agenda.Rows(0).Item("Lav_Cod")

                    '===========================================================================================================================================
                    'Controllo Split e Budget
                    '-------------------------------------------------------------------------------------------------------------------------------------------
                    If IsNumeric(dt_agenda.Rows(0).Item("Split")) Then

                        If CInt(dt_agenda.Rows(0).Item("Split")) = 1 Then
                            'In caso di split non devo fare nulla --> lo marco con uguale al precedente in modo da byapssare modifiche
                            bUguali = True
                            GoTo FineCheck

                        End If

                    End If

                    If CInt(dt_agenda.Rows(0).Item("Budget")) <> 0 Then
                        'In caso di costo budget non devo fare nulla --> lo marco con uguale al precedente in modo da byapssare modifiche
                        bUguali = True
                        GoTo FineCheck

                    End If


                    'Controllo Differenza Consistenze
                    If Lav_Cod > 2999 And Lav_Cod < 4000 Then

                        'Zoo TODO --> al momento non c'è controllo ma il cdg viene sempre battezzato come da modificare 
                        bUguali = False

                    Else

                        bUguali = True 'Inizializzazione

                        '===========================================================================================================================================
                        'Confronto Impianti
                        '-------------------------------------------------------------------------------------------------------------------------------------------
                        Dim dr_search() As DataRow
                        Dim bEsisteDistintaChiusa As Boolean = False

                        'Lettura impianti da operazione di QDC
                        dt_impianti_agenda = leggi.Leggi_Impianti_Dettagli_GroupByImpianti(Piva, Raccoglitore_Cod, Id_Agenda, "", objParametri_Server)
                        'Lettura impianti da operazione di CDG
                        dt_impianti_cdg = leggi.Leggi_CDG_Dettagli_GroupBy_Impianti(Piva, Id_Agenda_CDG, Data_Movimento, True, objParametri_Server, GiasContext:=GiasContext)

                        If dt_impianti_cdg.Rows.Count > 0 Then

                            dr_search = dt_impianti_cdg.Select("Flag_Distinta_Chiusa = 'True' And Valore <> 0 ")

                            If dr_search.Length > 0 Then

                                'Esiste una distinta chiusa valorizzata --> nessuna modifica
                                bEsisteDistintaChiusa = True

                            End If

                        End If

                        If Not bEsisteDistintaChiusa Then

                            'Nota: in caso di attività poliannuali lo trigghero sempre perchè potrebbero essere cambiate volutamente le distinte
                            If dt_impianti_agenda.Rows.Count <> dt_impianti_cdg.Rows.Count Then

                                'Numero diverso
                                bUguali = False

                            ElseIf dt_impianti_agenda.Rows.Count = 0 AndAlso dt_impianti_cdg.Rows.Count = 0 Then

                                'Nessun Prodotto Scaricato
                                bUguali = True

                            Else

                                '===========================================================================================================================================
                                'Confronto Impianti
                                '-------------------------------------------------------------------------------------------------------------------------------------------
                                For Each dr_impianti_cdg As DataRow In dt_impianti_cdg.Rows

                                    'Controllo che l'Impianto Agenda sia presente tra gli impianti CDG
                                    strFiltro_Confronto = "Piva='" & dr_impianti_cdg("Piva") & "' And " &
                                                          "Sa_Cod=" & dr_impianti_cdg("Sa_Cod") & " And " &
                                                          "Appezza=" & dr_impianti_cdg("Appezza") & " And " &
                                                          "Id_Destinazione=" & dr_impianti_cdg("Id_Destinazione")

                                    dr_impianti_agenda = dt_impianti_agenda.Select(strFiltro_Confronto)

                                    If dr_impianti_agenda Is Nothing OrElse dr_impianti_agenda.Length = 0 Then
                                        'Variazione Impianti
                                        bUguali = False
                                        Exit For

                                    End If

                                Next

                            End If


                            If bUguali Then

                                '===========================================================================================================================================
                                'Confronto Prodotti
                                '-------------------------------------------------------------------------------------------------------------------------------------------
                                'Lettura Prodotti QDC
                                dt_eredita_agenda = leggi.Leggi_Testata_Eredita(Piva, Id_Agenda, Raccoglitore_Cod, False, objParametri_Server, GiasContext:=GiasContext)
                                'Lettura Prodotti CDG
                                dt_eredita_cdg = leggi.Leggi_Testata_Eredita(Piva, Id_Agenda_CDG, Raccoglitore_Cod, True, objParametri_Server, GiasContext:=GiasContext)

                                If dt_eredita_cdg IsNot Nothing Then
                                    Dim str_Key As String = ""
                                    For Each dr_eredita_cdg As DataRow In dt_eredita_cdg.Rows

                                        'Costruzione chiave univoca prodotto per confronto
                                        str_Key = dr_eredita_cdg("Elem_Cod") & "_" &
                                                  dr_eredita_cdg("Pro_Cod") & "_" &
                                                  dr_eredita_cdg("Mat_Cod") & "_" &
                                                  dr_eredita_cdg("Lotto") & "_" &
                                                  dr_eredita_cdg("Udm_Cod") & "_" &
                                                  dr_eredita_cdg("Sa_Cod") & "_" &
                                                  dr_eredita_cdg("Tipo_Destinazione") & "_" &
                                                  dr_eredita_cdg("Fabbricato_Cod") & "_" &
                                                  dr_eredita_cdg("Qta") & "_" &
                                                  dr_eredita_cdg("Lav_Cod")


                                        If Not ht_Id_Attivita_CDG.ContainsKey(str_Key) Then
                                            ht_Id_Attivita_CDG.Add(str_Key, dr_eredita_cdg("Id_Attivita"))
                                        End If

                                    Next
                                End If

                                If dt_eredita_agenda.Rows.Count <> dt_eredita_cdg.Rows.Count Then
                                    'Numero diverso
                                    bUguali = False
                                ElseIf dt_eredita_agenda.Rows.Count = 0 AndAlso dt_eredita_cdg.Rows.Count = 0 Then
                                    'Nessun Prodotto Scaricato
                                    bUguali = True
                                Else

                                    bUguali = True 'Inizializzazione

                                    For Each dr_eredita_cdg As DataRow In dt_eredita_cdg.Rows

                                        'Controllo Eredita Agenda (Lav_Cod = 0 per gestire il pregresso)
                                        strFiltro_Confronto = "Elem_Cod=" & dr_eredita_cdg("Elem_Cod") & " And " &
                                                              "Prodotto_Cod=" & dr_eredita_cdg("Prodotto_Cod") & " And " &
                                                              "Lotto='" & dr_eredita_cdg("Lotto") & "' And " &
                                                              "Udm_Cod=" & dr_eredita_cdg("Udm_Cod") & " And " &
                                                              "Sa_Cod=" & dr_eredita_cdg("Sa_Cod") & " And " &
                                                              "Tipo_Destinazione=" & dr_eredita_cdg("Tipo_Destinazione") & " And " &
                                                              "Fabbricato_Cod=" & dr_eredita_cdg("Fabbricato_Cod")

                                        If dr_eredita_cdg("Lav_Cod") <> 0 Then
                                            strFiltro_Confronto = strFiltro_Confronto & " And Lav_Cod = " & dr_eredita_cdg("Lav_Cod")
                                        End If


                                        Dim dr_eredita_agenda() As DataRow
                                        dr_eredita_agenda = dt_eredita_agenda.Select(strFiltro_Confronto)
                                        If dr_eredita_agenda Is Nothing OrElse dr_eredita_agenda.Length = 0 Then

                                            bUguali = False
                                            Exit For

                                        Else

                                            If bUguali Then

                                                'Controllo qta
                                                Select Case CInt(dt_eredita_agenda(0).Item("Raccoglitore_Cod"))

                                                    Case 0

                                                        'Controlo Valore
                                                        If Format(dr_eredita_agenda(0).Item("Qta"), "###,###,###.##") <> Format(dr_eredita_cdg.Item("Qta"), "###,###,###.##") Then

                                                            bUguali = False
                                                            Exit For

                                                        End If

                                                    Case Else

                                                        'In caso di raccoglitore posso trovare più corrispondenze poichè appartenenti ad agende diverse --> controllo qta
                                                        bUguali = False
                                                        Dim ii As Integer = 0

                                                        For ii = 0 To dr_eredita_agenda.Length

                                                            'Controlo Valore
                                                            If Format(dr_eredita_agenda(ii).Item("Qta"), "###,###,###.##") = Format(dr_eredita_cdg.Item("Qta"), "###,###,###.##") Then

                                                                bUguali = True
                                                                Exit For

                                                            End If

                                                        Next

                                                End Select


                                            End If

                                        End If

                                    Next


                                End If

                            End If

                        End If

                    End If



                Else

                    'Sono nel caso in cui non sono ancora stati inseriti costi
                    ' Verifico se sono stati scaricati prodotti, diversamente esco
                    leggi.Leggi_CDG_Testate_Parte3(Piva, Id_Agenda, 0, 0, 1, Id_Attivita, Attivita_Des, objParametri_Server, kendo_Eredita, GiasContext:=GiasContext)

                    If kendo_Eredita Is Nothing OrElse kendo_Eredita = "[]" Then
                        bUguali = True
                    End If

                End If


FineCheck:


                If Not bUguali Then

                    'Delete Agenda Importata Automaticamente Avente Data Modifica Precedente alla Operazione di QDC Collegata
                    'Nota: Al momento lascio il riferimento in modo da preservare la chiave e quindi anche le impostazioni manuali

                    If Not bBombardinoMultiplo Then
                        'Dim objParametriUtenti As New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Utenti"))

                        Dim ModalitaZoo = LeggiModalitaZoo(objParametri_Utenti)

                        MessaggioErrore = scrivi_DAL.Batch_Delete(Piva, strFiltro, bForzato, ModalitaZoo, objParametri_Server)
                    Else
                        'Già eseguita dal bombardino multiplo
                    End If

                    Dim bAttivitaPoliannuale As Boolean = False
                    Dim ArrayA(0 To 0) As Integer

                    If String.IsNullOrEmpty(MessaggioErrore) Then

                        If Raccoglitore_Cod <> 0 Then
                            'Reset Stato_Export_2
                            scrivi_DAL.Modifica_Stato_Export_2(Piva, Raccoglitore_Cod, 0, 0, objParametri_Server)

                        End If

                        'Lettura delle Operazioni di Agenda Senza Imputazione di Costi                        
                        dt = leggi.Ipno_Lettura_Agenda_Senza_CDG(Piva, Id_Agenda, Raccoglitore_Cod, "Cau_Mov In ('7350')", True, AGRODATAINIZIO, AGRODATAFINE, objParametri_Server)

                        If dt.Rows.Count > 0 Then

                            'Lettura preventiva di tutte le attività poliannuali
                            dtAtt = leggi_Attivita.Leggi(0, "Attivita_Poliannuale = 1 And Tipo_Utilizzo In (0,2)", "", objParametri_Server)

                            Id_Agenda_CDG = 0

                            '====================================================================================================================================================
                            'Impostazione Attività e se la gestione è annuale o poliannuale
                            '----------------------------------------------------------------------------------------------------------------------------------------------------
                            For Each dr As DataRow In dt.Rows

                                Id_Attivita = 0
                                Attivita_Des = ""

                                If bFirst Then

                                    Lav_Cod = dr("Lav_Cod")
                                    Des_Lib = dr.Item("Des_Lib")
                                    Data_Inserimento_Da_Util = dr.Item("Data_Movimento")

                                    'Lettura Id_Agenda Riferimento per Preservare la Chiave
                                    dt_agenda = leggi.Leggi_Agenda_Riferimento(Piva, dr.Item("Id_Agenda"), 0, True, objParametri_Server, GiasContext:=GiasContext)

                                    If dt_agenda.Rows.Count > 0 Then

                                        Id_Agenda_CDG = dt_agenda.Rows(0).Item("Id_Agenda_Rif")

                                    End If

                                Else

                                    'Aggiornamento Des_Lib (nota: lettura da tabella per evitare ridondanza descrizione coltura in des_lib)
                                    Lav_Des = ObjOperazione.LavDes_from_LavCod(dr.Item("Lav_Cod"), objParametri_Server)

                                    If Trim(Lav_Des) <> "" Then
                                        Lavorazione_Des = Lav_Des
                                    Else
                                        Lavorazione_Des = dr.Item("Des_Lib")
                                    End If

                                    If InStr(1, Des_Lib, Lavorazione_Des) = 0 Then
                                        Des_Lib = Trim(Des_Lib & " + " & Lavorazione_Des)
                                    End If

                                End If


                                Select Case CInt(dr.Item("Lav_Cod"))

                                    Case LAVCOD_ALTRE_OPERAZIONI 'Altre Lavorazioni

                                        Id_Attivita = dr.Item("xId_Attivita")
                                        Attivita_Des = dr.Item("xAttivita_Des")

                                    Case Else

                                        'Verifico la possibilità di impostare l'attività 
                                        If ht_Id_Attivita_CDG.Count > 0 Then

                                            'Cerco l'attività corrispondente al lav_cod oppure se il lav_cod non è impostato per preservare il pregresso
                                            For Each objA As DictionaryEntry In ht_Id_Attivita_CDG

                                                If Id_Attivita = 0 Then

                                                    Arrayp = Split(objA.Key, "_")

                                                    If CInt(Arrayp(9)) = CInt(dr.Item("Lav_Cod")) Or CInt(Arrayp(9)) = 0 Then
                                                        Id_Attivita = ht_Id_Attivita_CDG(objA.Key)
                                                    End If

                                                End If

                                            Next

                                        End If

                                End Select


                                If Id_Attivita = 0 Then
                                    'Attività non ancora impostata --> determinazione dell'attività
                                    'Lettura Id_Attività Associato al Lav_Cod
                                    dt_Attivita = objAttivita.Leggi_Solo_Attivita(0, dr.Item("Lav_Cod"), "", "", objParametri_Server, True, Piva,, 0)

                                    For Each dr_Attivita As DataRow In dt_Attivita.Rows

                                        If dr_Attivita.Item("Id_Attivita") <> 0 Then

                                            Id_Attivita = dr_Attivita.Item("Id_Attivita")
                                            Exit For

                                        End If
                                    Next

                                End If


                                'Prima Impostazione
                                If Id_Attivita_Eredita_Base = 0 Then
                                    Id_Attivita_Eredita_Base = Id_Attivita
                                End If

                                'In caso di raccoglitore mantengo l'attività nulla per preservare le attività indipendentemente dall'ordine di salvataggio delle agende
                                If Id_Attivita = 0 And Raccoglitore_Cod = 0 Then
                                    Id_Attivita = Id_Attivita_Eredita_Base
                                End If

                                'Salvo l'id Attività nella posizione corrente
                                ReDim Preserve ArrayA(0 To UBound(ArrayA) + 1)
                                ArrayA(UBound(ArrayA) - 1) = Id_Attivita

                                bFirst = False

                            Next



                            Dim CountA As Integer = 0
                            Dim objDataDettaglio As JToken

                            'Riscorro il dt ed assegnazione attività
                            For Each dr As DataRow In dt.Rows

                                Id_Attivita = ArrayA(CountA)
                                CountA = CountA + 1

                                leggi.Leggi_CDG_Testate_Parte3(Piva, dr.Item("Id_Agenda"), 0, Id_Agenda_CDG, 1, Id_Attivita, "", objParametri_Server, kendo_Eredita, GiasContext:=GiasContext)

                                '==============================================================================================================================
                                ' Cerco di definire in modo più preciso l'attività a utilizzare per lo scarico prodotti riassegnando quella precedente
                                '------------------------------------------------------------------------------------------------------------------------------
                                Dim righeArray_Testata As New JArray

                                'La sostituzione può portare a tutte attività poliannuali


                                If (Not String.IsNullOrEmpty(kendo_Eredita) AndAlso kendo_Eredita <> "[]") Then

                                    righeArray_Testata = JArray.Parse(kendo_Eredita)

                                    'Ricavo i dati della ripartizione
                                    If righeArray_Testata.Count > 0 And Raccoglitore_Cod <> 0 And bFirstRipartizione Then

                                        objDataDettaglio = righeArray_Testata(0)
                                        bFirstRipartizione = False

                                        DtRipartizione = leggi.Leggi_Impianti_Da_Prodotto(Piva, 0, Raccoglitore_Cod, objDataDettaglio, objParametri_Server)

                                        If DtRipartizione.Rows.Count > 0 Then

                                            For Each drRipartizione In DtRipartizione.Rows

                                                If Sa_Cod_Last = 0 Then
                                                    Sa_Cod_Last = drRipartizione("Sa_Cod")
                                                End If
                                                If Sa_Cod_Last <> drRipartizione("Sa_Cod") Then
                                                    bMulticentro = True 'Operazione Multicentro
                                                End If
                                                TotaleRipartizione = TotaleRipartizione + drRipartizione("Qta")

                                            Next

                                        End If

                                    End If

                                    'Ricavo la chiave del primo dettaglio
                                    If ht_Id_Attivita_CDG.Keys.Count > 0 AndAlso righeArray_Testata.Count > 0 Then

                                        '1. vincolo sul prodotto, qta e lav_cod  
                                        Dim cambiato As Boolean = False
                                        Dim str_Key As String = ""

                                        For Each objData In righeArray_Testata

                                            cambiato = False

                                            'Cerco l'attività corrispondente al lav_cod oppure se il lav_cod non è impostato per preservare il pregresso
                                            For Each objA As DictionaryEntry In ht_Id_Attivita_CDG

                                                Arrayp = Split(objA.Key, "_")

                                                If CInt(Arrayp(0)) = CInt(objData("Elem_Cod")) And
                                                       CInt(Arrayp(1)) = CInt(objData("Pro_Cod")) And
                                                       CInt(Arrayp(2)) = CInt(objData("Mat_Cod")) And
                                                       CStr(Arrayp(3)) = CStr(objData("Lotto")) And
                                                       CInt(Arrayp(4)) = CInt(objData("Udm_Cod")) And
                                                       CInt(Arrayp(5)) = CInt(objData("Sa_Cod")) And
                                                       CInt(Arrayp(6)) = CInt(objData("Tipo_Destinazione")) And
                                                       CInt(Arrayp(7)) = CInt(objData("Fabbricato_Cod")) And
                                                       CInt(Arrayp(8)) = CInt(objData("Qta")) And
                                                       (CInt(Arrayp(9)) = CInt(objData("Lav_Cod")) Or CInt(Arrayp(9)) = 0) Then

                                                    'Preservo l'attività impostata nel CDG
                                                    objData("ID_Attivita").Replace(JToken.Parse(CStr(ht_Id_Attivita_CDG.Item(objA.Key))))
                                                    kendo_Eredita = JsonConvert.SerializeObject(righeArray_Testata)
                                                    cambiato = True

                                                End If

                                            Next



                                            If Not cambiato Then

                                                'Cerco l'attività corrispondente al lav_cod oppure se il lav_cod non è impostato per preservare il pregresso
                                                For Each objA As DictionaryEntry In ht_Id_Attivita_CDG

                                                    Arrayp = Split(objA.Key, "_")

                                                    If CInt(Arrayp(0)) = CInt(objData("Elem_Cod")) And
                                                                               CInt(Arrayp(1)) = CInt(objData("Pro_Cod")) And
                                                                               CInt(Arrayp(2)) = CInt(objData("Mat_Cod")) And
                                                                               CStr(Arrayp(3)) = CStr(objData("Lotto")) And
                                                                               CInt(Arrayp(4)) = CInt(objData("Udm_Cod")) And
                                                                               CInt(Arrayp(5)) = CInt(objData("Sa_Cod")) And
                                                                               CInt(Arrayp(6)) = CInt(objData("Tipo_Destinazione")) And
                                                                               CInt(Arrayp(7)) = CInt(objData("Fabbricato_Cod")) And
                                                                               (CInt(Arrayp(9)) = CInt(objData("Lav_Cod")) Or CInt(Arrayp(9)) = 0) Then

                                                        objData("ID_Attivita").Replace(JToken.Parse(CStr(ht_Id_Attivita_CDG.Item(objA.Key))))
                                                        kendo_Eredita = JsonConvert.SerializeObject(righeArray_Testata)
                                                        cambiato = True

                                                    End If

                                                Next

                                            End If

                                            If Not cambiato Then

                                                'Vincolo sul elem_cod e lav_cod                                           

                                                'Cerco l'attività corrispondente al lav_cod oppure se il lav_cod non è impostato per preservare il pregresso
                                                For Each objA As DictionaryEntry In ht_Id_Attivita_CDG

                                                    Arrayp = Split(objA.Key, "_")

                                                    If CInt(Arrayp(0)) = CInt(objData("Elem_Cod")) And
                                                             (CInt(Arrayp(9)) = CInt(objData("Lav_Cod")) Or CInt(Arrayp(9)) = 0) Then

                                                        objData("ID_Attivita").Replace(JToken.Parse(CStr(ht_Id_Attivita_CDG.Item(objA.Key))))
                                                        kendo_Eredita = JsonConvert.SerializeObject(righeArray_Testata)
                                                        cambiato = True

                                                    End If

                                                Next

                                            End If

                                            '=============================================================================================================================
                                            ' Verifico se attività è poliennale
                                            '-----------------------------------------------------------------------------------------------------------------------------
                                            If dtAtt.Rows.Count > 0 Then
                                                Id_Attivita = objData("ID_Attivita")
                                                drAtt = dtAtt.Select("Id_Attivita = " & Id_Attivita)

                                                If drAtt Is Nothing OrElse drAtt.Length = 0 Then
                                                    attAnnuale = True
                                                Else
                                                    attPoliennale = True
                                                    'In caso di attività poliennale occorre ripartire il costo per le distinte future
                                                End If

                                            Else
                                                attAnnuale = True
                                            End If
                                            '=============================================================================================================================

                                        Next

                                    ElseIf ht_Id_Attivita_CDG.Keys.Count = 0 Then

                                        '=============================================================================================================================
                                        ' Verifico se attività è poliennale
                                        '-----------------------------------------------------------------------------------------------------------------------------
                                        If dtAtt.Rows.Count > 0 Then

                                            drAtt = dtAtt.Select("Id_Attivita = " & Id_Attivita)

                                            If drAtt Is Nothing OrElse drAtt.Length = 0 Then
                                                attAnnuale = True
                                            Else
                                                attPoliennale = True
                                            End If

                                        Else
                                            attAnnuale = True
                                        End If
                                        '=============================================================================================================================


                                    End If

                                End If
                                '==============================================================================================================================




                                'Concatenazione json per ottenre la stringa completa con il salvataggio del pregresso
                                If (Not String.IsNullOrEmpty(kendo_Eredita) AndAlso kendo_Eredita <> "[]") Then

                                    If Trim(Kendo_Eredita_Totale_str) = "" Then

                                        'prima assegnazione
                                        Kendo_Eredita_Totale_str = kendo_Eredita

                                    Else

                                        'concatenazione di stringhe json
                                        Kendo_Eredita_Totale_str = Left(Kendo_Eredita_Totale_str, Len(Kendo_Eredita_Totale_str) - 1)
                                        kendo_Eredita = Right(kendo_Eredita, Len(kendo_Eredita) - 1)

                                        Kendo_Eredita_Totale_str = Kendo_Eredita_Totale_str & ", " & kendo_Eredita

                                    End If

                                End If

                                bInsertCDG = True
                                Modalita_Imputazione = 0

                            Next

                            'Dati da leggere solo la prima volta perchè univoci all'interno del raccoglitore_cod o senza raccoglitore
                            leggi.Leggi_CDG_Testate_Parte1(Piva, Id_Agenda, 0, Id_Agenda_CDG, 1, objParametri_Server, kendo_Manodopera, kendo_Terzisti, kendo_Macchine, GiasContext:=GiasContext)
                            leggi.Leggi_CDG_Testate_Parte2(Piva, Id_Agenda, 0, Id_Agenda_CDG, 1, objParametri_Server, kendo_Magazzino, kendo_Libera, GiasContext:=GiasContext)

                            If (Not String.IsNullOrEmpty(kendo_Manodopera) AndAlso kendo_Manodopera <> "[]") OrElse
                                    (Not String.IsNullOrEmpty(kendo_Terzisti) AndAlso kendo_Terzisti <> "[]") OrElse
                                    (Not String.IsNullOrEmpty(kendo_Macchine) AndAlso kendo_Macchine <> "[]") OrElse
                                    (Not String.IsNullOrEmpty(kendo_Magazzino) AndAlso kendo_Magazzino <> "[]") OrElse
                                    (Not String.IsNullOrEmpty(kendo_Libera) AndAlso kendo_Libera <> "[]") Then

                                bInsertCDG = True

                            End If

                            If bInsertCDG Then


                                'Determinazione Tipo Attività Annuale/Poliannuale per costi non ereditati
                                Allinea_CostiBombardinoTipoAttivita(attAnnuale, attPoliennale, kendo_Manodopera, dtAtt, objParametri_Server)
                                Allinea_CostiBombardinoTipoAttivita(attAnnuale, attPoliennale, kendo_Terzisti, dtAtt, objParametri_Server)
                                Allinea_CostiBombardinoTipoAttivita(attAnnuale, attPoliennale, kendo_Macchine, dtAtt, objParametri_Server)
                                Allinea_CostiBombardinoTipoAttivita(attAnnuale, attPoliennale, kendo_Magazzino, dtAtt, objParametri_Server)
                                Allinea_CostiBombardinoTipoAttivita(attAnnuale, attPoliennale, kendo_Libera, dtAtt, objParametri_Server)


                                'Determinazione se l'attività sarà annuale o polianuale (basta un'attività annuale per determinare l'annualità dell'intera registrazione)
                                If Not attAnnuale And attPoliennale Then
                                    bAttivitaPoliannuale = True
                                End If


                                'Solo ora posso leggere gli impianti perchè so se attività è poliennale o meno
                                ' TODO peccato che potrei averne una poliennale ed una non ma devo mostrarli tutti nello stesso modo

                                'Lettura della ripartizione proporzionata a tutti gli impianti imputati prendendo il primo prodotto in movimenti_dettagli
                                Select Case bMulticentro
                                    Case False
                                        kendo_Impianti = leggi.Leggi_Impianti_Dettagli(Piva, Id_Agenda, Data_Inserimento_Da_Util, False, 0, True, bAttivitaPoliannuale, objParametri_Server, GiasContext:=GiasContext)
                                    Case True
                                        kendo_Impianti = leggi.Leggi_Impianti_Dettagli_Multicentro(Piva, Raccoglitore_Cod, Data_Inserimento_Da_Util, DtRipartizione, TotaleRipartizione, False, 0, True, bAttivitaPoliannuale, objParametri_Server, GiasContext:=GiasContext)
                                End Select

                                kendo_Zoo = leggi.Leggi_Zoo_Dettagli(Piva, Id_Agenda, Data_Inserimento_Da_Util, False, objParametri_Server, GiasContext:=GiasContext)


                                'Scrittura CDG
                                Dim Id_Agenda_CDG_Return = scrivi_BIZ.AggiornaCDG(Piva, Id_Agenda, Id_Agenda_CDG, Lav_Cod, Des_Lib, Data_Inserimento_Da_Util, Modalita_Imputazione, 0, 0, Costi_Ricavi, 1, Id_Attivita, 1,
                                                                                  False, 0,
                                                                                  kendo_Manodopera, "", kendo_Terzisti, "", kendo_Macchine, "", kendo_Magazzino, "", kendo_Libera, "", Kendo_Eredita_Totale_str, "",
                                                                                  kendo_Impianti, "", "", "", "", "", "", "", kendo_Zoo, "", objParametri_Server, 0, 0, False,
                                                                                  GiasContext:=GiasContext, OpenNewTransaction:=OpenNewTransaction,
                                                                                  listaErrori:=listaErrori)

                                If Id_Agenda_CDG_Return = 0 Then
                                    MessaggioErrore = MessaggioErrore & "Aggiornamento costi non riuscito per l'operazione di campagna " & Des_Lib & "<br/>"
                                End If

                            End If

                        End If



                    End If

                Else

                    'Allineamento Data Creazione in modo da non processarlo più alla seguente mandata
                    If Id_Agenda <> 0 AndAlso Id_Agenda_CDG <> 0 Then

                        MessaggioErrore = scrivi_DAL.AllineaDate(Piva, Id_Agenda, Id_Agenda_CDG, objParametri_Server)

                    End If


                End If


            Else

                'Appartenente ad un raccoglitore ma non è la prima (modifica di un campo per impostare l'agenda del raccoglitore come bypassabile poichè non è la prima del raccoglitore)
                scrivi_DAL.Modifica_Stato_Export_2(Piva, 0, Id_Agenda, -1, objParametri_Server)

            End If


        Catch ex As Exception

            'uso questa funzione per ottenere il Messaggio..:
            MessaggioErrore = "Errore durante l'operazione: " & vbCrLf &
                            AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        Finally

            If GiasContext IsNot Nothing And OpenNewTransaction = True Then
                GiasContext.Dispose()
                GiasContext = Nothing
            End If

        End Try

        If MessaggioErrore <> "" AndAlso listaErrori IsNot Nothing Then
            'Aggiunta per Operazioni NG.. semina con frazionamento
            'Questo errore del bombardino richiede assistenza da parte dell'Amministratore...deve essere visibile all'utente quando appare 
            Dim objErroreGias As New ErroreGias With {
                .severity = ErroreGias_Severity.Bloccante,
                .tipo = ErroreGias_Tipo.Generico,
                .messaggio = MessaggioErrore
            }

            listaErrori.Add(objErroreGias)
        End If

        Return MessaggioErrore

    End Function




    Public Function AllineaCostiMovimentiZoo_Bombardino(ByVal Piva As String,
                                                        ByVal Id_Agenda As Integer,
                                                        ByVal Data_Movimento As Date,
                                                        ByVal bForzato As Boolean,
                                                        ByVal objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                        ByVal objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                        Optional ByRef GiasContext As AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities = Nothing,
                                                        Optional ByVal OpenNewTransaction As Boolean = True,
                                                        Optional ByRef listaErrori As List(Of ErroreGias) = Nothing,
                                                        Optional ByVal bBombardinoMultiplo As Boolean = False
                                                        ) As String


        Dim MessaggioErrore As String = ""
        Dim Raccoglitore_Cod As Integer = 0

        Try

            'Creazione OggettiF
            Dim leggi As New CDG_DAL_R
            Dim leggi_DW As New DW_CDG_Costi_Ricavi_DAL_R
            Dim scrivi_BIZ As New CDG_BIZ_W
            Dim scrivi_DW As New DW_CDG_Costi_Ricavi_DAL_W
            Dim scrivi_DAL As New CDG_DAL_W
            Dim objQualifichexTariffe As New AgronicaCoreContabDAL.QualificheXTariffe_R
            Dim objAttivita As New AgronicaCoreContabDAL.AttivitaXOperazioni_R
            Dim ObjOperazione As New AgronicaCoreMetaSchemaDAL.Operazioni_R
            Dim leggi_Attivita As New Attivita_R
            Dim dt As DataTable
            Dim dt_agenda As DataTable
            Dim dt_Attivita As DataTable
            Dim drAtt() As DataRow
            Dim Id_Agenda_CDG As Integer
            Dim Id_Attivita As Integer
            Dim Attivita_Des As String = ""
            Dim Costi_Ricavi As Integer
            Dim Modalita_Imputazione As Integer
            Dim strFiltro As String
            Dim kendo_Impianti As String = ""
            Dim kendo_Zoo As String = ""
            Dim kendo_Manodopera As String = ""
            Dim kendo_Terzisti As String = ""
            Dim kendo_Macchine As String = ""
            Dim kendo_Magazzino As String = ""
            Dim kendo_Libera As String = ""
            Dim kendo_Libera_Dummy As String = ""
            Dim kendo_Eredita As String = ""

            Dim bUguali As Boolean
            Dim strFiltro_Confronto As String = ""
            Dim Lav_Cod As Integer = 0
            Dim Lav_Cod_CDG As Integer = 0
            Dim Id_Attivita_CDG As Integer = 0
            Dim bFirst As Boolean = True
            Dim bInsertCDG As Boolean = False
            Dim Id_Attivita_Eredita_Base As Integer = 0
            Dim Des_Lib As String = ""
            Dim Data_Inserimento_Da_Util As Date
            Dim Kendo_Libera_Totale_str As String = ""
            Dim attPoliennale = False
            Dim attAnnuale = False
            Dim dtAtt As New DataTable

            Dim Lav_Des As String = ""
            Dim Lavorazione_Des As String = ""

            Dim Prezzo_totale As Double = 0

            Costi_Ricavi = 0

            'Impostazione Filtro
            strFiltro = "Agenda.Id_Agenda = " & Id_Agenda

            'Lettura Id_Agenda
            bUguali = False

            dt_agenda = leggi.Leggi_Agenda_Riferimento(Piva, Id_Agenda, 0, True, objParametri_Server, GiasContext)

            If dt_agenda.Rows.Count > 0 Then

                Id_Agenda_CDG = dt_agenda.Rows(0).Item("Id_Agenda_Rif")
                Lav_Cod = dt_agenda.Rows(0).Item("Lav_Cod")

                'Dim dt_eredita_cdg As DataTable = leggi.Leggi_CDG_Testata(Piva, Id_Agenda_CDG, Raccoglitore_Cod, True, objParametri_Server)
                'If dt_eredita_cdg IsNot Nothing Then
                '    Id_Attivita_CDG = dt_eredita_cdg(0)("Id_Attivita")
                'End If

            End If

            'Lettura delle movimentazioni zoo ed inserimento in KendoLibera
            'leggi.Leggi_CDG_CaricoScaricoZoo(Piva, Id_Agenda, 0, 0, 1, Id_Attivita, Attivita_Des, objParametri_Server, kendo_Libera, GiasContext:=GiasContext, Prezzo_Totale:=Prezzo_totale)

            'Delete Agenda Importata Automaticamente Avente Data Modifica Precedente alla Operazione di QDC Collegata
            'Nota: Al momento lascio il riferimento in modo da preservare la chiave e quindi anche le impostazioni manuali

            If Not bBombardinoMultiplo Then
                'Dim objParametriUtenti As New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Utenti"))

                Dim ModalitaZoo = LeggiModalitaZoo(objParametri_Utenti)

                MessaggioErrore = scrivi_DAL.Batch_Delete(Piva, strFiltro, bForzato, ModalitaZoo, objParametri_Server)
            Else
                'Già eseguita dal bombardino multiplo
            End If

            Dim bAttivitaPoliannuale As Boolean = False
            Dim ArrayA(0 To 0) As Integer

            If String.IsNullOrEmpty(MessaggioErrore) Then

                'Lettura delle Operazioni di Agenda Senza Imputazione di Costi
                dt = leggi.Ipno_Lettura_Agenda_Senza_CDG(Piva, Id_Agenda, Raccoglitore_Cod, "Cau_Mov In ('3700', '3750')", True, AGRODATAINIZIO, AGRODATAFINE, objParametri_Server)

                If dt.Rows.Count > 0 Then

                    'Lettura preventiva di tutte le attività poliannuali
                    dtAtt = leggi_Attivita.Leggi(0, "Attivita_Poliannuale = 1 And Tipo_Utilizzo In (0,2)", "", objParametri_Server)

                    Id_Agenda_CDG = 0

                    '====================================================================================================================================================
                    'Impostazione Attività e se la gestione è annuale o poliannuale
                    '----------------------------------------------------------------------------------------------------------------------------------------------------
                    For Each dr As DataRow In dt.Rows

                        Id_Attivita = 0
                        Attivita_Des = ""

                        If bFirst Then

                            Lav_Cod = dr("Lav_Cod")
                            Des_Lib = dr.Item("Des_Lib")
                            Id_Attivita = dr.Item("xId_Attivita")
                            Data_Inserimento_Da_Util = dr.Item("Data_Movimento")

                            'Lettura Id_Agenda Riferimento per Preservare la Chiave
                            dt_agenda = leggi.Leggi_Agenda_Riferimento(Piva, dr.Item("Id_Agenda"), 0, True, objParametri_Server, GiasContext:=GiasContext)

                            If dt_agenda.Rows.Count > 0 Then

                                Id_Agenda_CDG = dt_agenda.Rows(0).Item("Id_Agenda_Rif")

                            End If

                        Else

                            'Aggiornamento Des_Lib (nota: lettura da tabella per evitare ridondanza descrizione coltura in des_lib)
                            Lav_Des = ObjOperazione.LavDes_from_LavCod(dr.Item("Lav_Cod"), objParametri_Server)

                            If Trim(Lav_Des) <> "" Then
                                Lavorazione_Des = Lav_Des
                            Else
                                Lavorazione_Des = dr.Item("Des_Lib")
                            End If

                            If InStr(1, Des_Lib, Lavorazione_Des) = 0 Then
                                Des_Lib = Trim(Des_Lib & " + " & Lavorazione_Des)
                            End If

                        End If



                        If Id_Attivita = 0 Then

                            'Lettura Id_Attività Associato al Lav_Cod
                            dt_Attivita = objAttivita.Leggi_Solo_Attivita(0, dr.Item("Lav_Cod"), "", "", objParametri_Server, True, Piva,, 0)

                            For Each dr_Attivita As DataRow In dt_Attivita.Rows

                                If dr_Attivita.Item("Id_Attivita") <> 0 Then

                                    Id_Attivita = dr_Attivita.Item("Id_Attivita")
                                    Exit For

                                End If
                            Next

                        End If


                        'Prima Impostazione
                        If Id_Attivita_Eredita_Base = 0 Then
                            Id_Attivita_Eredita_Base = Id_Attivita
                        End If

                        'In caso di raccoglitore mantengo l'attività nulla per preservare le attività indipendentemente dall'ordine di salvataggio delle agende
                        If Id_Attivita = 0 And Raccoglitore_Cod = 0 Then
                            Id_Attivita = Id_Attivita_Eredita_Base
                        End If

                        'Salvo l'id Attività nella posizione corrente
                        ReDim Preserve ArrayA(0 To UBound(ArrayA) + 1)
                        ArrayA(UBound(ArrayA) - 1) = Id_Attivita

                        bFirst = False

                    Next

                    'Controllo Impostazione Attività
                    If Id_Attivita <> 0 Then

                        Dim CountA As Integer = 0

                        'Riscorro il dt ed assegnazione attività
                        For Each dr As DataRow In dt.Rows

                            Id_Attivita = ArrayA(CountA)
                            CountA = CountA + 1

                            leggi.Leggi_CDG_CaricoScaricoZoo(Piva, dr.Item("Id_Agenda"), 0, Id_Agenda_CDG, 1, Id_Attivita, "", objParametri_Server, kendo_Libera, GiasContext:=GiasContext, Prezzo_Totale:=Prezzo_totale)

                            '==============================================================================================================================
                            ' Cerco di definire in modo più preciso l'attività a utilizzare per lo scarico prodotti riassegnando quella precedente
                            '------------------------------------------------------------------------------------------------------------------------------
                            Dim righeArray_Testata As New JArray



                            '=============================================================================================================================
                            ' Verifico se attività è poliennale
                            '-----------------------------------------------------------------------------------------------------------------------------
                            If dtAtt.Rows.Count > 0 Then
                                drAtt = dtAtt.Select("Id_Attivita = " & Id_Attivita)

                                If drAtt Is Nothing OrElse drAtt.Length = 0 Then
                                    attAnnuale = True
                                Else
                                    attPoliennale = True
                                End If

                            Else
                                attAnnuale = True
                            End If
                            '=============================================================================================================================


                            'Concatenazione json
                            If (Not String.IsNullOrEmpty(kendo_Libera) AndAlso kendo_Libera <> "[]") Then

                                If Trim(Kendo_Libera_Totale_str) = "" Then

                                    'prima assegnazione
                                    Kendo_Libera_Totale_str = kendo_Libera

                                Else

                                    'concatenazione di stringhe json
                                    Kendo_Libera_Totale_str = Left(Kendo_Libera_Totale_str, Len(Kendo_Libera_Totale_str) - 1)
                                    kendo_Libera = Right(kendo_Libera, Len(kendo_Libera) - 1)

                                    Kendo_Libera_Totale_str = Kendo_Libera_Totale_str & ", " & kendo_Eredita

                                End If

                                bInsertCDG = True
                                Modalita_Imputazione = 0

                            End If



                        Next

                        'Dati da leggere solo la prima volta perchè univoci all'interno del raccoglitore_cod o senza raccoglitore
                        leggi.Leggi_CDG_Testate_Parte1(Piva, Id_Agenda, 0, Id_Agenda_CDG, 1, objParametri_Server, kendo_Manodopera, kendo_Terzisti, kendo_Macchine, GiasContext:=GiasContext)
                        leggi.Leggi_CDG_Testate_Parte2(Piva, Id_Agenda, 0, Id_Agenda_CDG, 1, objParametri_Server, kendo_Magazzino, kendo_Libera_Dummy, GiasContext:=GiasContext, bypass_Kendo_Libera:=True)

                        If (Not String.IsNullOrEmpty(kendo_Manodopera) AndAlso kendo_Manodopera <> "[]") OrElse
                                (Not String.IsNullOrEmpty(kendo_Terzisti) AndAlso kendo_Terzisti <> "[]") OrElse
                                (Not String.IsNullOrEmpty(kendo_Macchine) AndAlso kendo_Macchine <> "[]") OrElse
                                (Not String.IsNullOrEmpty(kendo_Magazzino) AndAlso kendo_Magazzino <> "[]") Then

                            bInsertCDG = True

                        End If

                        If bInsertCDG Then


                            'Determinazione Tipo Attività Annuale/Poliannuale per costi non ereditati
                            Allinea_CostiBombardinoTipoAttivita(attAnnuale, attPoliennale, kendo_Manodopera, dtAtt, objParametri_Server)
                            Allinea_CostiBombardinoTipoAttivita(attAnnuale, attPoliennale, kendo_Terzisti, dtAtt, objParametri_Server)
                            Allinea_CostiBombardinoTipoAttivita(attAnnuale, attPoliennale, kendo_Macchine, dtAtt, objParametri_Server)
                            Allinea_CostiBombardinoTipoAttivita(attAnnuale, attPoliennale, kendo_Magazzino, dtAtt, objParametri_Server)

                            'TODO: Gestire l'imputazione di distinte zoo in caso di attività poliannuale
                            kendo_Zoo = leggi.Leggi_Zoo_Dettagli(Piva, Id_Agenda, Data_Inserimento_Da_Util, False, objParametri_Server, GiasContext:=GiasContext, Prezzo_totale)

                            'Scrittura CDG
                            Dim Id_Agenda_CDG_Return = scrivi_BIZ.AggiornaCDG(Piva, Id_Agenda, Id_Agenda_CDG, Lav_Cod, Des_Lib, Data_Inserimento_Da_Util, Modalita_Imputazione, 0, 0, Costi_Ricavi, 1, Id_Attivita, 1,
                                                                                      False, 0,
                                                                                      kendo_Manodopera, "", kendo_Terzisti, "", kendo_Macchine, "", kendo_Magazzino, "", Kendo_Libera_Totale_str, "", "", "",
                                                                                      kendo_Impianti, "", "", "", "", "", "", "", kendo_Zoo, "", objParametri_Server, 0, 0, False,
                                                                                      GiasContext:=GiasContext, OpenNewTransaction:=OpenNewTransaction,
                                                                                      listaErrori:=listaErrori)

                            If Id_Agenda_CDG_Return = 0 Then
                                MessaggioErrore = MessaggioErrore & "Aggiornamento costi non riuscito per l'operazione di campagna " & Des_Lib & "<br/>"
                            End If

                        End If

                    End If

                End If



            End If


        Catch ex As Exception

            'uso questa funzione per ottenere il Messaggio..:
            MessaggioErrore = "Errore durante l'operazione: " & vbCrLf &
                            AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        Finally

            If GiasContext IsNot Nothing And OpenNewTransaction = True Then
                GiasContext.Dispose()
                GiasContext = Nothing
            End If

        End Try

        If MessaggioErrore <> "" AndAlso listaErrori IsNot Nothing Then
            'Aggiunta per Operazioni NG.. semina con frazionamento
            'Questo errore del bombardino richiede assistenza da parte dell'Amministratore...deve essere visibile all'utente quando appare 
            Dim objErroreGias As New ErroreGias With {
                .severity = ErroreGias_Severity.Bloccante,
                .tipo = ErroreGias_Tipo.Generico,
                .messaggio = MessaggioErrore
            }

            listaErrori.Add(objErroreGias)
        End If

        Return MessaggioErrore

    End Function


    Public Function AllineaCostiDaVisite_Bombardino(ByVal Piva As String,
                                                    ByVal Id_Agenda As Integer,
                                                    ByVal Data_Movimento As Date,
                                                    ByVal bForzato As Boolean,
                                                    ByVal objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                    ByVal objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                    Optional ByRef GiasContext As AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities = Nothing,
                                                    Optional ByVal OpenNewTransaction As Boolean = True,
                                                    Optional ByRef listaErrori As List(Of ErroreGias) = Nothing,
                                                    Optional ByVal bBombardinoMultiplo As Boolean = False
                                                    ) As String


        Dim MessaggioErrore As String = ""


        Try

            'Creazione OggettiF
            Dim leggi As New CDG_DAL_R
            Dim leggi_Agenda As New AgronicaCoreContabDAL.Agenda_R
            Dim leggi_DW As New DW_CDG_Costi_Ricavi_DAL_R
            Dim scrivi_BIZ As New CDG_BIZ_W
            Dim scrivi_DW As New DW_CDG_Costi_Ricavi_DAL_W
            Dim scrivi_DAL As New CDG_DAL_W
            Dim objQualifichexTariffe As New AgronicaCoreContabDAL.QualificheXTariffe_R
            Dim objAttivita As New AgronicaCoreContabDAL.AttivitaXOperazioni_R
            Dim ObjOperazione As New AgronicaCoreMetaSchemaDAL.Operazioni_R
            Dim ObjVisita As New AgronicaCoreContabDAL.Mov_Dettagli_Riferimenti_R
            Dim objConfigurazione_Siti As New AgronicaCoreVarieDAL.Configurazione_Siti_R()
            Dim leggi_Attivita As New Attivita_R
            Dim dt As DataTable
            Dim dt_agenda As DataTable
            Dim dt_Attivita As DataTable
            Dim dt_visita As DataTable
            Dim dt_siti As DataTable

            Dim Id_Agenda_CDG As Integer
            Dim Id_Agenda_Visita As Integer
            Dim Id_Attivita As Integer
            Dim Attivita_Des As String = ""
            Dim Costi_Ricavi As Integer = 0
            Dim Modalita_Imputazione As Integer
            Dim strFiltro As String
            Dim kendo_Impianti As String = ""
            Dim kendo_Imputazioni As String = ""
            Dim kendo_Zoo As String = ""
            Dim kendo_Manodopera As String = ""
            Dim kendo_Terzisti As String = ""
            Dim kendo_Macchine As String = ""
            Dim kendo_Magazzino As String = ""
            Dim kendo_Libera As String = ""
            Dim kendo_Eredita As String = ""

            Dim bUguali As Boolean
            Dim strFiltro_Confronto As String = ""
            Dim Lav_Cod As Integer = 0
            Dim Lav_Cod_CDG As Integer = 0

            Dim bInsertCDG As Boolean = False
            Dim bFirst As Boolean = True
            Dim Id_Attivita_Eredita_Base As Integer = 0
            Dim Des_Lib As String = ""
            Dim Data_Inserimento_Da_Util As Date
            Dim Kendo_Eredita_Totale_str As String = ""
            Dim attPoliennale = False
            Dim attAnnuale = False
            Dim dtAtt As New DataTable
            Dim bProdottoTrovato As Boolean = False

            Dim Piva_CDG As String = ""
            Dim Lav_Cod_Visita As Integer = 0
            Dim Lav_Des As String = ""
            Dim Lavorazione_Des As String = ""

            Dim PivaSuperUser = objParametri_Server.PivaSuperUser
            ' Dim objParametriSuperServer As New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Super_Server"))



            'Determinazione Piva Proprietario del CDG
            dt_siti = objConfigurazione_Siti.Leggi(0, "Azienda_Timesheet_Tecnici", "", "", objParametri_Server)

            If Not IsNothing(dt_siti) AndAlso dt_siti.Rows.Count > 0 Then

                Piva_CDG = dt_siti(0)("Valore")

                'Lettura Operazione Visita
                dt_visita = leggi_Agenda.Leggi(Piva, 0, Id_Agenda, 0, enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_Server)

                If dt_visita.Rows.Count > 0 And Trim(Piva_CDG) <> "" Then

                    'Lav_Cod_Visita = dt_visita(0)("Lav_Cod")

                    'Select Case Lav_Cod_Visita

                    '    Case LAVCOD_ALTRE_OPERAZIONI
                    Id_Attivita = dt_visita(0)("Id_Attivita")
                    '    Case Else
                    '        Id_Attivita = 0 'Da determinare mediante tabella AttivitaxOperazioni
                    'End Select


                    'Impostazione Filtro
                    strFiltro = "Agenda.Id_Agenda = " & Id_Agenda

                    'Lettura Id_Agenda
                    bUguali = False

                    dt_agenda = leggi.Leggi_Agenda_Riferimento(Piva, Id_Agenda, 0, True, objParametri_Server, GiasContext)

                    If dt_agenda.Rows.Count > 0 Then

                        Id_Agenda_CDG = dt_agenda.Rows(0).Item("Id_Agenda_Rif")

                        '===========================================================================================================================================
                        'Controllo Budget
                        '-------------------------------------------------------------------------------------------------------------------------------------------                            

                        If CInt(dt_agenda.Rows(0).Item("Budget")) <> 0 Then

                            bUguali = True

                        End If


                    End If

FineCheck:

                    If Not bUguali Then

                        If Not bBombardinoMultiplo Then
                            'Dim objParametriUtenti As New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Utenti"))

                            Dim ModalitaZoo = LeggiModalitaZoo(objParametri_Utenti)

                            MessaggioErrore = scrivi_DAL.Batch_Delete(Piva, strFiltro, bForzato, ModalitaZoo, objParametri_Server)
                        Else
                            'Già eseguita dal bombardino multiplo
                        End If

                        If String.IsNullOrEmpty(MessaggioErrore) Then

                            'Lettura delle Operazioni di Agenda Senza Imputazione di Costi
                            dt = leggi.Ipno_Lettura_Agenda_Senza_CDG(Piva, Id_Agenda, 0, "", True, AGRODATAINIZIO, AGRODATAFINE, objParametri_Server)

                            If dt.Rows.Count > 0 Then

                                '====================================================================================================================================================
                                'Impostazione Attività e Descrizione
                                '----------------------------------------------------------------------------------------------------------------------------------------------------

                                Lav_Cod = dt(0)("Lav_Cod")
                                Des_Lib = dt(0).Item("Des_Lib") & " Az. " & dt(0)("Rag_Soc")
                                Data_Inserimento_Da_Util = dt(0).Item("Data_Movimento")


                                'Impostazione Attività se non impostata precedentemente
                                If Id_Attivita = 0 Then

                                    'Lettura nella tabella AttivitaxOperazioni                                    
                                    dt_Attivita = objAttivita.Leggi_Solo_Attivita(0, Lav_Cod, "", "", objParametri_Server, True, Piva,, 0)

                                    For Each dr_Attivita As DataRow In dt_Attivita.Rows

                                        If dr_Attivita.Item("Id_Attivita") <> 0 Then

                                            Id_Attivita = dr_Attivita.Item("Id_Attivita")
                                            Exit For

                                        End If
                                    Next

                                End If


                                bFirst = False
                                bInsertCDG = False


                                'Lettura del Progetto associato all'attività
                                Dim LeggiImputazione As New AgronicaCoreContabDAL.Imputazioni_Fasi_R
                                Dim DTImputazione As DataTable
                                Dim strFiltroAggiuntivoImputazione As String = "I.Validita_Inizio <= " & Agro_SQL_SaveDate(Data_Inserimento_Da_Util) & " And I.Validita_Fine >= " & Agro_SQL_SaveDate(Data_Inserimento_Da_Util)
                                Dim Imputazione_cod As Integer = 0
                                DTImputazione = LeggiImputazione.Leggi(Piva_CDG, Id_Attivita, 0, 0, strFiltroAggiuntivoImputazione, "", objParametri_Server, enumSelezioneVariabile.Selezione_JoinCompleta)

                                If DTImputazione.Rows.Count > 0 Then

                                    Select Case DTImputazione.Rows.Count

                                        Case 1

                                            Imputazione_cod = DTImputazione.Rows(0)("Imputazione_Cod")

                                        Case Else

                                            'Inizializzazione
                                            Imputazione_cod = DTImputazione.Rows(0)("Imputazione_Cod")

                                            Dim DTLast As DataTable

                                            'Lettura dell'ultima imputazione
                                            DTLast = leggi.Ricava_Last_Visita(Piva, Id_Attivita, objParametri_Server)

                                            If DTLast.Rows.Count > 0 Then

                                                Dim dr_search_imputazione() As DataRow

                                                dr_search_imputazione = DTImputazione.Select("Imputazione_Cod = " & DTLast.Rows(0)("Imputazione_Cod"))

                                                If dr_search_imputazione.Length > 0 Then
                                                    Imputazione_cod = dr_search_imputazione(0)("Imputazione_Cod")
                                                End If

                                            End If

                                    End Select

                                End If


                                If Imputazione_cod <> 0 Then

                                    DTImputazione = LeggiImputazione.Leggi(Piva_CDG, Id_Attivita, Imputazione_cod, 0, strFiltroAggiuntivoImputazione, "", objParametri_Server, enumSelezioneVariabile.Selezione_JoinCompleta)

                                    Dim serializerSettings As New JsonSerializerSettings()
                                    serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                                    kendo_Imputazioni = JsonConvert.SerializeObject(DTImputazione, Formatting.None, serializerSettings)
                                    bInsertCDG = True


                                End If



                                If bInsertCDG Then

                                    'Lettura della Risorsa Umana
                                    Dim DTRisum As DataTable
                                    Dim DTOrario As DataTable
                                    Dim DTManodopera As New DataTable
                                    Dim dr_manodopera As DataRow
                                    Dim Cod_Risum As Integer = 0
                                    DTRisum = leggi.Ricava_Cod_Risum_Visita(Piva, Id_Agenda, objParametri_Server)
                                    DTOrario = leggi.Ricava_Orario_Visita(Piva, Id_Agenda, objParametri_Server)

                                    If DTRisum.Rows.Count > 0 And DTOrario.Rows.Count > 0 Then

                                        'Definizione DT_Dettaglio_Specifico
                                        DTManodopera.Columns.Add(New DataColumn("Piva_SuperUser", Type.GetType("System.String")))
                                        DTManodopera.Columns.Add(New DataColumn("Piva", Type.GetType("System.String")))
                                        DTManodopera.Columns.Add(New DataColumn("Cod_Risum", Type.GetType("System.Int32")))
                                        DTManodopera.Columns.Add(New DataColumn("Udm_Cod", Type.GetType("System.Int32")))
                                        DTManodopera.Columns.Add(New DataColumn("Descrizione", Type.GetType("System.String")))
                                        DTManodopera.Columns.Add(New DataColumn("ID_Attivita", Type.GetType("System.Int32")))

                                        DTManodopera.Columns.Add(New DataColumn("Ora_Inizio", Type.GetType("System.DateTime")))
                                        DTManodopera.Columns.Add(New DataColumn("Ora_Fine", Type.GetType("System.DateTime")))

                                        DTManodopera.Columns.Add(New DataColumn("Qta", Type.GetType("System.Decimal")))
                                        DTManodopera.Columns.Add(New DataColumn("Tariffa_Cod", Type.GetType("System.Int32")))
                                        DTManodopera.Columns.Add(New DataColumn("Qualifica_Cod", Type.GetType("System.Int32")))

                                        DTManodopera.Columns.Add(New DataColumn("Prezzo_Unitario", Type.GetType("System.Decimal")))
                                        DTManodopera.Columns.Add(New DataColumn("Prezzo_Totale", Type.GetType("System.Decimal")))


                                        'Aggiungo nuova riga
                                        dr_manodopera = DTManodopera.NewRow

                                        dr_manodopera.Item("Piva_SuperUser") = objParametri_Server.PivaSuperUser
                                        dr_manodopera.Item("Piva") = Piva_CDG
                                        dr_manodopera.Item("Cod_Risum") = DTRisum(0)("Cod_Risum")
                                        dr_manodopera.Item("Udm_Cod") = 141
                                        dr_manodopera.Item("Descrizione") = DTRisum(0)("Descrizione")
                                        dr_manodopera.Item("ID_Attivita") = Id_Attivita
                                        dr_manodopera.Item("Ora_Inizio") = DTOrario(0)("Ora_Inizio")
                                        dr_manodopera.Item("Ora_Fine") = DTOrario(0)("Ora_Fine")
                                        dr_manodopera.Item("Qta") = DateDiff("H", DTOrario(0)("Ora_Inizio"), DTOrario(0)("Ora_Fine"))

                                        dr_manodopera.Item("Tariffa_Cod") = 0
                                        dr_manodopera.Item("Qualifica_Cod") = 0
                                        dr_manodopera.Item("Prezzo_Unitario") = 0
                                        dr_manodopera.Item("Prezzo_Totale") = 0

                                        DTManodopera.Rows.Add(dr_manodopera)


                                        Dim serializerSettings2 As New JsonSerializerSettings()
                                        serializerSettings2.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                                        kendo_Manodopera = JsonConvert.SerializeObject(DTManodopera, Formatting.None)

                                        kendo_Terzisti = "[]"
                                        kendo_Macchine = "[]"
                                        kendo_Magazzino = "[]"
                                        kendo_Libera = "[]"

                                        'Scrittura CDG
                                        Dim Id_Agenda_CDG_Return = scrivi_BIZ.AggiornaCDG(Piva, Id_Agenda, Id_Agenda_CDG, Lav_Cod, Des_Lib, Data_Inserimento_Da_Util, Modalita_Imputazione, 0, 0, Costi_Ricavi, 1, Id_Attivita, 1,
                                                                                        False, 0,
                                                                                        kendo_Manodopera, "", kendo_Terzisti, "", kendo_Macchine, "", kendo_Magazzino, "", kendo_Libera, "", Kendo_Eredita_Totale_str, "",
                                                                                        kendo_Impianti, kendo_Imputazioni, "", "", "", "", "", "", kendo_Zoo, "", objParametri_Server, 0, 0, False,
                                                                                        GiasContext:=GiasContext, OpenNewTransaction:=OpenNewTransaction,
                                                                                        listaErrori:=listaErrori, Piva_CDG)

                                        If Id_Agenda_CDG_Return = 0 Then
                                            MessaggioErrore = MessaggioErrore & "Aggiornamento costi non riuscito per l'operazione di campagna " & Des_Lib & "<br/>"
                                        End If

                                    End If

                                End If




                            End If

                        End If

                    Else

                        'Allineamento Data Creazione
                        If Id_Agenda <> 0 AndAlso Id_Agenda_CDG <> 0 Then

                            MessaggioErrore = scrivi_DAL.AllineaDate(Piva, Id_Agenda, Id_Agenda_CDG, objParametri_Server)

                        End If


                    End If

                End If

            End If

        Catch ex As Exception

            'uso questa funzione per ottenere il Messaggio..:
            MessaggioErrore = "Errore durante l'operazione: " & vbCrLf &
                            AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        Finally

            If GiasContext IsNot Nothing And OpenNewTransaction = True Then
                GiasContext.Dispose()
                GiasContext = Nothing
            End If

        End Try

        If MessaggioErrore <> "" AndAlso listaErrori IsNot Nothing Then
            'Aggiunta per Operazioni NG.. semina con frazionamento
            'Questo errore del bombardino richiede assistenza da parte dell'Amministratore...deve essere visibile all'utente quando appare 
            Dim objErroreGias As New ErroreGias With {
                .severity = ErroreGias_Severity.Bloccante,
                .tipo = ErroreGias_Tipo.Generico,
                .messaggio = MessaggioErrore
            }

            listaErrori.Add(objErroreGias)
        End If

        Return MessaggioErrore

    End Function





    Private Sub Allinea_CostiBombardinoTipoAttivita(ByRef attannuale As Boolean,
                                                    ByRef attPoliennale As Boolean,
                                                    ByVal KendoCDG As String,
                                                    ByRef dtAtt As DataTable,
                                                    ByVal objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri)

        Dim righeArray_Testata As JArray
        Dim drAtt() As DataRow
        Dim Id_Attivita As Integer

        '=============================================================================================================================
        ' Verifico se l'attività è poliennale
        '-----------------------------------------------------------------------------------------------------------------------------

        If (Not String.IsNullOrEmpty(KendoCDG) AndAlso KendoCDG <> "[]") Then

            righeArray_Testata = JArray.Parse(KendoCDG)

            If dtAtt.Rows.Count > 0 Then

                For Each objData In righeArray_Testata

                    Id_Attivita = objData("ID_Attivita")
                    drAtt = dtAtt.Select("Id_Attivita = " & Id_Attivita)

                    If drAtt Is Nothing OrElse drAtt.Length = 0 Then
                        attannuale = True
                    Else
                        attPoliennale = True
                    End If
                Next

            Else

                attannuale = True

            End If

        End If

        '=============================================================================================================================

    End Sub


    Public Function AllineaCosti_BombardinoMultiplo(ByVal piva As String,
                                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                    ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                    Optional ByVal dataInizio As Date = AGRODATAINIZIO,
                                                    Optional ByVal dataFine As Date = AGRODATAFINE,
                                                    Optional ByRef GiasContext As AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities = Nothing,
                                                    Optional ByVal OpenNewTransaction As Boolean = True,
                                                    Optional ByRef listaErrori As List(Of ErroreGias) = Nothing
                                                    ) As String

        Dim nomeRoutine As String = ""
        Dim messaggioErrore As String = ""
        Dim msgRis As String = ""
        Dim strFiltro As String = ""
        Dim dt As DataTable

        'Funzione di generazione multipla operazioni cdg passando come riferimento la piva e non la singola agenda

        Try

            Dim objLeggiDal As New CDG_DAL_R
            Dim objScriviDal As New CDG_DAL_W

            Dim ModalitaZoo = LeggiModalitaZoo(objParametri_Utenti)

            '############################################################################################################################################
            '############################################## QDC + ZOO ###################################################################################
            '############################################################################################################################################

            'Impostazione Filtro
            strFiltro = "((Agenda.Lav_cod > 0 And Agenda.Lav_cod < 1000) OR (Agenda.Lav_cod > 2999 And Agenda.Lav_cod < 4000 AND Agenda.Lav_Cod <> 3030))" 'QDC + ZOO escludo gli spostamenti

            'Delete Agenda Importata Automaticamente Avente Data Modifica Precedente alla Operazione di QDC Collegata
            'Nota: Al momento lascio il riferimento in modo da preservare la chiave e quindi anche le impostazioni manuali
            msgRis &= objScriviDal.Batch_Delete(piva, strFiltro, False, ModalitaZoo, objParametri)


            'Lettura delle Operazioni di Agenda Senza Imputazione di Costi (cioè appena cancellata oppure mai creata poichè in insert)
            dt = objLeggiDal.Ipno_Lettura_Agenda_Senza_CDG(piva,
                                                           0,
                                                           0,
                                                           strFiltro,
                                                           False,
                                                           dataInizio,
                                                           dataFine,
                                                           objParametri)

            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                Dim modBombardino As String = ""
                For Each dr As DataRow In dt.Rows

                    Select Case dr.Item("Lav_Cod")
                        Case LAVCOD_NASCITA_ANIMALI, LAVCOD_ACQUISTO_ANIMALI
                            'questi lav_cod devono entrare nel bombardino qdc in modalità Aboca perchè deve scaricare un prodotto, per Inalca invece no
                            Select Case ModalitaZoo

                                Case 0
                                    'Modalità Aboca
                                    modBombardino = "qdc"

                                Case 1
                                    'Modalità Standard
                                    modBombardino = "zoo"

                            End Select

                        Case LAVCOD_INCREMENTO_CONSISTENZE_ZOO, LAVCOD_DECREMENTO_CONSISTENZE_ZOO, LAVCOD_VENDITA_ANIMALI, LAVCOD_TRASFERIMENTO_ANIMALI, LAVCOD_MACELLAZIONE_ANIMALI, LAVCOD_MORTE_ANIMALI, LAVCOD_SPOSTAMENTI_ZOO
                            'questi lav_cod devono entrare nel bombardino zoo perchè non devono scaricare un prodotto, crea una riga LIBERA
                            modBombardino = "zoo"

                        Case Else
                            'gestione classica del qdc
                            modBombardino = "qdc"
                    End Select

                    If modBombardino = "qdc" Then
                        'QDC
                        msgRis &= AllineaCostiDaCampagna_Bombardino(piva,
                                                                    dr.Item("Id_Agenda"),
                                                                    dr.Item("Data_Movimento"),
                                                                    False, objParametri, objParametri_Utenti,
                                                                    GiasContext:=GiasContext, OpenNewTransaction:=OpenNewTransaction,
                                                                    listaErrori:=listaErrori,
                                                                    True)
                    Else
                        'Modalità Standard
                        msgRis &= AllineaCostiMovimentiZoo_Bombardino(piva,
                                                                    dr.Item("Id_Agenda"),
                                                                    dr.Item("Data_Movimento"),
                                                                    False, objParametri, objParametri_Utenti,
                                                                    GiasContext:=GiasContext, OpenNewTransaction:=OpenNewTransaction,
                                                                    listaErrori:=listaErrori,
                                                                    True)
                    End If
                Next
            End If


        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return msgRis

    End Function



    Public Function AllineaCosti_BombardinoMultiploVisite(ByVal piva As String,
                                                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                          ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                          Optional ByVal dataInizio As Date = AGRODATAINIZIO,
                                                          Optional ByVal dataFine As Date = AGRODATAFINE,
                                                          Optional ByRef GiasContext As AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities = Nothing,
                                                          Optional ByVal OpenNewTransaction As Boolean = True,
                                                          Optional ByRef listaErrori As List(Of ErroreGias) = Nothing
                                                          ) As String

        Dim nomeRoutine As String = ""
        Dim messaggioErrore As String = ""
        Dim msgRis As String = ""
        Dim strFiltro As String = ""
        Dim dt As DataTable

        Try

            Dim objLeggiDal As New CDG_DAL_R
            Dim objScriviDal As New CDG_DAL_W


            '############################################################################################################################################
            '############################################## VISITE #####################################################################################
            '############################################################################################################################################

            'Impostazione Filtro
            strFiltro = "(Agenda.Lav_Cod = 5007)" 'Visite

            Dim ModalitaZoo = LeggiModalitaZoo(objParametri_Utenti)

            'Delete Agenda Importata Automaticamente Avente Data Modifica Precedente alla Operazione di QDC Collegata
            'Nota: Al momento lascio il riferimento in modo da preservare la chiave e quindi anche le impostazioni manuali
            msgRis &= objScriviDal.Batch_Delete(piva, strFiltro, False, ModalitaZoo, objParametri)


            'Lettura delle Operazioni di Agenda Senza Imputazione di Costi
            dt = objLeggiDal.Ipno_Lettura_Agenda_Senza_CDG(piva,
                                                           0,
                                                           0,
                                                           strFiltro,
                                                           False,
                                                           dataInizio,
                                                           dataFine,
                                                           objParametri)

            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then

                For Each dr As DataRow In dt.Rows
                    msgRis &= AllineaCostiDaVisite_Bombardino(piva,
                                                              dr.Item("Id_Agenda"),
                                                              dr.Item("Data_Movimento"),
                                                              False, objParametri, objParametri_Utenti,
                                                              GiasContext:=GiasContext, OpenNewTransaction:=OpenNewTransaction,
                                                              listaErrori:=listaErrori,
                                                              True)
                Next

            End If


        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return msgRis

    End Function

    Public Function SelezionaBombardinoDaLanciare(ByVal lavCod As String, ByVal ObjParametriUtenti As AgronicaCoreParametri, ByVal ObjParametriServer As AgronicaCoreParametri) As String
        Dim msgRis As String = ""
        Dim modBombardino As String = ""
        Dim nomeRoutine As String = "CDG_BIZ.SelezionaBombardinoDaLanciare()"
        Dim messaggioErrore As String = ""
        Try
            Dim ModalitaZoo = LeggiModalitaZoo(ObjParametriUtenti)

            Select Case lavCod
                Case LAVCOD_NASCITA_ANIMALI, LAVCOD_ACQUISTO_ANIMALI
                    'questi lav_cod devono entrare nel bombardino qdc in modalità Aboca perchè deve scaricare un prodotto, per Inalca invece no
                    Select Case ModalitaZoo

                        Case 0
                            'Modalità Aboca
                            modBombardino = "qdc"

                        Case 1
                            'Modalità Standard
                            modBombardino = "zoo"

                    End Select

                Case LAVCOD_INCREMENTO_CONSISTENZE_ZOO, LAVCOD_DECREMENTO_CONSISTENZE_ZOO, LAVCOD_VENDITA_ANIMALI, LAVCOD_TRASFERIMENTO_ANIMALI, LAVCOD_MACELLAZIONE_ANIMALI, LAVCOD_MORTE_ANIMALI, LAVCOD_SPOSTAMENTI_ZOO
                    'questi lav_cod devono entrare nel bombardino zoo perchè non devono scaricare un prodotto, crea una riga LIBERA
                    modBombardino = "zoo"

                Case Else
                    'gestione classica del qdc
                    modBombardino = "qdc"
            End Select

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(ObjParametriServer, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return modBombardino
    End Function

    Public Function LeggiModalitaZoo(ByVal objParametri_Utenti As AgronicaCoreParametri)
        'Lettura dell'impostazione utenti per verificare la modalità di carico zoo
        Dim nomeRoutine As String = "CDG_BIZ.LeggiModalitaZoo()"
        Dim Modalita_Zoo = 0 'Default
        Try
            If objParametri_Utenti IsNot Nothing Then
                Dim ObjUtenti As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
                Dim DTUtenti As DataTable = ObjUtenti.Leggi(enum_Impostazioni_Utenti.SUPERUSER_CARICO_ZOO_GRIGLIA, 2, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Utenti)
                If DTUtenti.Rows.Count <> 0 Then
                    Modalita_Zoo = CInt(DTUtenti(0)("Impostazione_Valore_1"))
                End If
            End If
        Catch ex As Exception
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
        End Try

        Return Modalita_Zoo
    End Function

    ' Utilizzato per la gestione dei costi sulla stessa pagina del CdG
    Public Function ExportCDGVecchioTipoBIZ(ByVal Piva As String,
                                    ByVal JsonInsMod As String,
                                    ByVal DtCdgToDelete As DataTable,
                                    ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                    ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri
                                    ) As String

        Dim Piva_SuperUser = objParametri_Server.PivaSuperUser

        Dim MessaggioErrore As String = String.Empty
        Dim Vecchio_Tipo_Inser_Dati As Integer

        Dim dataModifica = Date.Now

        Dim NomeRoutine As String = "ContabBIZ.CDG_BIZ.ExportCDGVecchioTipoBIZ()"

        Try

            Dim CDG_Testata_Export As CDG_Testata = Nothing
            Dim CDG_Dettagli_Export As CDG_Dettagli = Nothing
            Dim righeInsModArray As JArray = JArray.Parse(JsonInsMod)

            If righeInsModArray.Count > 0 OrElse DtCdgToDelete.Rows.Count > 0 Then

                If righeInsModArray.Count > 0 Then
                    Dim obj = righeInsModArray(0)
                    Vecchio_Tipo_Inser_Dati = obj("Tipo_Imputazione")
                End If

                Dim campConf_W As New CDG_DAL_W
                MessaggioErrore = campConf_W.Scrivi_ExportCDGVecchioTipo(Piva_SuperUser, Piva, Vecchio_Tipo_Inser_Dati, righeInsModArray, DtCdgToDelete, dataModifica, objParametri_Server, objParametri_Utenti)

            End If

        Catch ex As Exception
            MessaggioErrore = "[" & NomeRoutine & "] : " & ex.Message
            Scrivi_LOG(objParametri_Server, NomeRoutine, MessaggioErrore)
        Finally

        End Try


        If Not String.IsNullOrEmpty(MessaggioErrore) Then
            Throw New Exception(MessaggioErrore)
        End If

        Return MessaggioErrore
    End Function

    Public Function AggiornaCDGImputazioniDocContabile(ByVal Piva As String,
                                                       ByVal Id_Agenda As Integer, ByVal Id_Agenda_CDG As Integer,
                                                       ByVal Lav_Cod As Integer, ByVal Des_Lib As String,
                                                       ByVal Data_Inserimento As Date,
                                                       ByVal Costi_Ricavi As Integer,
                                                       ByVal Id_Attivita_Batch As Integer, ByVal ModifInAutom As Integer,
                                                       ByVal righeInseriteGrid_Testata_Eredita As String,
                                                       ByVal righeModificateGrid_Testata_Eredita As String,
                                                       ByVal righeInseriteGrid_Dettagli_Progetti As String,
                                                       ByVal righeModificateGrid_Dettagli_Progetti As String,
                                                       ByRef objParametri As AgronicaCoreParametri,
                                                       Optional ByVal Id_Mov As Integer = 0,
                                                       Optional ByVal Id_Mov_Det As Integer = 0
                                                       ) As Integer

        Dim idCdgScritta As Integer = 0

        idCdgScritta = AggiornaCDG(Piva, Id_Agenda, Id_Agenda_CDG, Lav_Cod, Des_Lib, Data_Inserimento,
                                   Modalita_Imputazione:=4, Tipo_Ripartizione:=0, Budget:=0,
                                   Costi_Ricavi, Modalita_Ripartizione:=0,
                                   Id_Attivita_Batch, ModifInAutom,
                                   origine_APP:=False, Split:=0,
                                   righeInseriteGrid_Testata_Manodopera:="",
                                   righeModificateGrid_Testata_Manodopera:="",
                                   righeInseriteGrid_Testata_Terzisti:="",
                                   righeModificateGrid_Testata_Terzisti:="",
                                   righeInseriteGrid_Testata_Macchine:="",
                                   righeModificateGrid_Testata_Macchine:="",
                                   righeInseriteGrid_Testata_Magazzino:="",
                                   righeModificateGrid_Testata_Magazzino:="",
                                   righeInseriteGrid_Testata_Libera:="",
                                   righeModificateGrid_Testata_Libera:="",
                                   righeInseriteGrid_Testata_Eredita:=righeInseriteGrid_Testata_Eredita,
                                   righeModificateGrid_Testata_Eredita:=righeModificateGrid_Testata_Eredita,
                                   righeInseriteGrid_Dettagli_Impianti:="",
                                   righeModificateGrid_Dettagli_Impianti:="",
                                   righeInseriteGrid_Dettagli_Progetti:=righeInseriteGrid_Dettagli_Progetti,
                                   righeModificateGrid_Dettagli_Progetti:=righeModificateGrid_Dettagli_Progetti,
                                   righeInseriteGrid_Dettagli_Macchine:="",
                                   righeModificateGrid_Dettagli_Macchine:="",
                                   righeInseriteGrid_Dettagli_Linee:="",
                                   righeModificateGrid_Dettagli_Linee:="",
                                   righeInseriteGrid_Dettagli_Zoo:="",
                                   righeModificateGrid_Dettagli_Zoo:="",
                                   objParametri:=objParametri,
                                   Id_Mov:=Id_Mov,
                                   Id_Mov_Det:=Id_Mov_Det)

        Return idCdgScritta

    End Function

    Public Sub Valida_Squadra_APP(ByRef squadra As AgronicaCoreModelsSTD.anagrafiche.SquadraAttivita,
                                  ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri)


        If String.IsNullOrEmpty(squadra.partitaIva) Then
            Throw New Exception("La partita IVA della squadra è obbligatoria.")
        End If

        If String.IsNullOrEmpty(squadra.codiceSquadra) OrElse squadra.codiceSquadra = "0" Then

            If squadra.flag_cancellazione Then
                Throw New Exception("Non è possibile cancellare la squadra.")
            End If

            If String.IsNullOrEmpty(squadra.descrizione) Then
                Throw New Exception("Inserire la descrizione della squadra.")
            End If

            If IsNothing(squadra.caposquadra) OrElse squadra.caposquadra.Count = 0 Then
                If IsNothing(squadra.membri) OrElse squadra.membri.Count = 0 Then
                    Throw New Exception("Inserire almeno una persona o un caposquadra.")
                End If
            End If

        End If

    End Sub

    Public Sub Scrivi_Squadra_APP(ByRef squadra As AgronicaCoreModelsSTD.anagrafiche.SquadraAttivita,
                                   ByVal tipoOperazione As enum_TipoOperazioneDB,
                                   ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                   ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri)

        Dim objCDGWrite As New CDG_DAL_W
        Dim piva As String = squadra.partitaIva
        Dim piva_SuperUser = objParametri_Server.PivaSuperUser
        Dim id_squadra As Integer = CInt(squadra.codiceSquadra)

        If tipoOperazione = enum_TipoOperazioneDB.Cancellazione Then

            objCDGWrite.Elimina_Squadra_Attivita(piva, id_squadra, objParametri_Server, enum_SistemiEsterni.GiasAPP)

        Else

            Dim squadraAttivita As SquadreXAttivita = Nothing

            If id_squadra > 0 Then
                Dim gefutils As New Gias_EF_Utility
                Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri_Server.StringaConnessione)
                Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)
                    squadraAttivita = (From s In GiasContext.SquadreXAttivita
                                       Where s.Piva_SuperUser.Equals(piva_SuperUser) AndAlso
                                           s.Piva.Equals(piva) AndAlso (s.ID_Squadra = id_squadra)
                                       Select s).FirstOrDefault()
                End Using
                If IsNothing(squadraAttivita) Then
                    Throw New Exception("La squadra non esiste.")
                ElseIf squadraAttivita.pubblico = 1 Then
                    Throw New Exception("Non è possibile modificare una squadra pubblica.")
                End If
            Else
                squadraAttivita = New SquadreXAttivita With {.ID_Squadra = id_squadra, .pubblico = 0, .Lav_Cod = 0}
            End If

            squadraAttivita.Piva = squadra.partitaIva
            squadraAttivita.des_Squadra = squadra.descrizione
            If Not IsNothing(squadra.caposquadra) AndAlso squadra.caposquadra.Count > 0 Then
                squadraAttivita.cod_risum_caposquadra_list = String.Join("|", squadra.caposquadra)
            End If
            If Not IsNothing(squadra.membri) AndAlso squadra.membri.Count > 0 Then
                squadraAttivita.cod_risum_list = String.Join("|", squadra.membri)
            End If
            squadraAttivita.Validita_Inizio = If(IsNothing(squadra.validoDal), AGRODATAINIZIO, squadra.validoDal)
            squadraAttivita.Validita_Fine = If(IsNothing(squadra.validoAL), AGRODATAFINE, squadra.validoAL)
            Dim squadraString As String = JsonConvert.SerializeObject(squadraAttivita)

            objCDGWrite.Scrivi_Squadra_Attivita(
                piva, squadraString, objParametri_Server,
                "Operazione registrata da SincroDatiApp",
                enum_SistemiEsterni.GiasAPP,
                id_squadra)

            squadra.codiceSquadra = id_squadra.ToString()

        End If

    End Sub

End Class
