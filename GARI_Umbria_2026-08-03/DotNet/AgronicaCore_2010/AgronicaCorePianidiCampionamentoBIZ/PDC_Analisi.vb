Imports System.Web
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCorePianidiCampionamentoDAL
Imports Newtonsoft.Json

Public Class PDC_Analisi

    Public Property Tipo_Campione_Des As String
    Public Property Tipo_Campione_Cod As String
    Public Property FornitoreFatturazione_Des As String
    Public Property Piva_FornitoreFatturazione As String
    Public Property Analisi_Tipologia_Des As String
    Public Property PDC_Stato_Analisi_Des As String
    Public Property InfoVarie As String
    Public Property PivaSuperUser As String
    Public Property Id_PDC_Testata As Integer
    Public Property ID_PDC_Dettagli As Integer
    Public Property ID_PDC_Campione As Integer
    Public Property Analisi_Testata_Cod As Integer
    Public Property Analisi_Testata_Des As String
    Public Property PDC_Stato_Analisi As Integer
    Public Property Cod_Contatto As String
    Public Property Cod_Risum As Integer
    Public Property Cod_Risum_Des As String
    Public Property Analisi_Tipologia_Cod As Integer
    Public Property Analisi_Tipologia_Tipo As Integer
    Public Property Altre_Molecole As String
    Public Property Altre_Molecole_Cod As String
    Public Property Data_Richiesta_Analisi As Date
    Public Property Note_Richiesta_Analisi As String
    Public Property ID_NC As Integer
    Public Property PDC_Stato_Validazione As Integer
    Public Property PDC_Stato_Pubblicazione As Integer

End Class




'#############################################################################################
'#############################################################################################
'###################################### HELPER ###############################################
'#############################################################################################
'#############################################################################################

Public Class PDC_Analisi_Helper

    Public Shared Sub Carica(ByVal Id_Testata As Integer, ByVal ID_PDC_Dettagli As Integer,
                             ByVal ID_PDC_Campione As Integer,
                             ByRef oggetto As List(Of PDC_Analisi),
                             ByVal objParametri As AgronicaCoreParametri)

        oggetto = New List(Of PDC_Analisi)

        Dim objPDC_Analisi As New AgronicaCorePianidiCampionamentoDAL.PDC_Analisi_R
        Dim dt As DataTable = objPDC_Analisi.Leggi(Id_Testata, ID_PDC_Dettagli, ID_PDC_Campione, 0, "", "", objParametri)

        For i As Integer = 0 To dt.Rows.Count - 1
            Dim obj As New PDC_Analisi
            obj.Id_PDC_Testata = dt.Rows(i).Item("Id_PDC_Testata")
            obj.ID_PDC_Dettagli = dt.Rows(i).Item("ID_PDC_Dettagli")
            obj.ID_PDC_Campione = dt.Rows(i).Item("ID_PDC_Campione")
            obj.Analisi_Testata_Cod = dt.Rows(i).Item("Analisi_Testata_Cod")
            obj.Analisi_Testata_Des = dt.Rows(i).Item("Analisi_Testata_Des")
            obj.FornitoreFatturazione_Des = dt.Rows(i).Item("FornitoreFatturazione_Des")
            obj.Piva_FornitoreFatturazione = dt.Rows(i).Item("Piva_FornitoreFatturazione")

            obj.PDC_Stato_Analisi = dt.Rows(i).Item("PDC_Stato_Analisi")
            obj.PDC_Stato_Validazione = dt.Rows(i).Item("PDC_Stato_Validazione")
            obj.PDC_Stato_Pubblicazione = dt.Rows(i).Item("PDC_Stato_Pubblicazione")
            obj.PDC_Stato_Analisi_Des = dt.Rows(i).Item("PDC_Stato_Analisi_Des")
            obj.Cod_Risum = dt.Rows(i).Item("Cod_Risum")
            obj.Cod_Risum_Des = dt.Rows(i).Item("Cod_Risum_Des")
            If dt.Rows(i).Item("Analisi_Tipologia_Cod") = -1 Then
                obj.Analisi_Tipologia_Cod = -1
                obj.Analisi_Tipologia_Des = "Tipologia analisi non Selezionata"
                obj.Analisi_Tipologia_Tipo = -1
            Else
                obj.Analisi_Tipologia_Cod = dt.Rows(i).Item("Analisi_Tipologia_Cod")
                obj.Analisi_Tipologia_Des = dt.Rows(i).Item("Analisi_Tipologia_Des")
                obj.Analisi_Tipologia_Tipo = dt.Rows(i).Item("Analisi_Tipologia_Tipo")
            End If

            obj.Tipo_Campione_Cod = dt.Rows(i).Item("Tipo_Campione_Cod")
            obj.Tipo_Campione_Des = dt.Rows(i).Item("Tipo_Campione_Des")


            obj.Altre_Molecole = dt.Rows(i).Item("Altre_Molecole")
            obj.Altre_Molecole_Cod = dt.Rows(i).Item("Altre_Molecole_Cod")
            obj.Data_Richiesta_Analisi = dt.Rows(i).Item("Data_Richiesta_Analisi")
            If Not IsDBNull(dt.Rows(i).Item("Note_Richiesta_Analisi")) Then
                obj.Note_Richiesta_Analisi = dt.Rows(i).Item("Note_Richiesta_Analisi")
            Else
                obj.Note_Richiesta_Analisi = ""
            End If
            obj.ID_NC = If(IsDBNull(dt.Rows(i).Item("ID_NC")), -1, dt.Rows(i).Item("ID_NC"))
            'aggiungo l'oggetto alla lista
            oggetto.Add(obj)
        Next

    End Sub

    Public Shared Sub Salva(ByVal oggetto As PDC_Testata, ByVal objParametri As AgronicaCoreParametri)

    End Sub


    Public Shared Function Cancella(ByVal Id_Testata As Integer, ByVal Id_Dettagli As Integer,
                                    ByVal Id_Campione As Integer, ByVal Analisi_Testata_Cod As Integer,
                                    ByRef objParametri As AgronicaCoreParametri,
                                        Optional ByVal CancellaAncheBlocchi As Boolean = False,
                                        Optional ByVal CancellaAncheMarketAccess As Boolean = False,
                                        Optional NoteLog As String = "PDC_Analisi.vb"
                                    ) As String

        Dim xRisp As String = ""
        Dim flagConnessione, flagTransazione As Boolean

        Try
            Utility.VerificaApriTransazione(objParametri, flagConnessione, flagTransazione)

            'Elimino l'analisi
            If Analisi_Testata_Cod > 0 Then
                Dim objPdc_Analisi As New AgronicaCorePianidiCampionamentoDAL.PDC_Analisi_W
                objPdc_Analisi.Cancella(Id_Testata, Id_Dettagli, Id_Campione, Analisi_Testata_Cod, objParametri)

                'TODO: devo cancellare anche Analisi_conformità?!? Al momento neanche la vecchia li cancella!

                If CancellaAncheBlocchi = True Then

                    'devo cancellare anche i blocchi/sblocchi derivanti da questa analisi
                    '(passo solo l'id analisi, e non eventuali id_dettagli,
                    'perché sennò potrei non cancellare tutto ciò che è collegato a quell'analisi)
                    xRisp = PDC_Sblocca_Helper.Cancella(Id_Testata, 0, Analisi_Testata_Cod,
                                                        objParametri, CancellaAncheMarketAccess)
                    If xRisp <> "" Then
                        Throw New Exception(xRisp)
                    End If

                End If

            End If

            'una volta cancellata la richiesta cancello l'analisi
            If Analisi_Testata_Cod > 0 Then
                Dim objAn_R As New AgronicaCoreAnagrafeDAL.Analisi_Testata_R
                Dim DT As DataTable = objAn_R.Leggi(Analisi_Testata_Cod, 0, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri)

                Dim objAn As New AgronicaCoreAnagrafeDAL.Analisi_Testata_W
                objAn.Cancella(Analisi_Testata_Cod, "", objParametri)

                Dim objAnD As New AgronicaCoreAnagrafeDAL.Analisi_Dettagli_W
                objAnD.Cancella(Analisi_Testata_Cod, 0, 0, "", objParametri)

                Dim objAgronicaLogAnalisiW As New AgronicaCoreAnagrafeDAL.AgronicaLogAnalisi_W
                objAgronicaLogAnalisiW.Scrivi(enum_TipoOperazioneDB.Cancellazione,
                                              DT.Rows(0).Item("analisi_testata_tipo"),
                                              Analisi_Testata_Cod,
                                              DT.Rows(0).Item("Analisi_Testata_Des"),
                                              DT.Rows(0).Item("analisi_testata_data_inizio"),
                                              NoteLog, enum_Id_Servizio.GiasOnline,
                                              objParametri)

            End If

            Utility.VerificaChiudiTransazione(objParametri, flagTransazione)

        Catch ex As Exception
            Utility.VerificaAnnullaTransazione(objParametri, flagTransazione)
            xRisp = ex.Message
        Finally
            Utility.VerificaChiudiConnessione(objParametri, flagConnessione)
        End Try

        Return xRisp

    End Function

    Public Shared Function Nuova(ByVal oggetto As PDC_Analisi,
                                 ByVal objParametri As AgronicaCoreParametri,
                                    Optional NoteLog As String = "PDC_Analisi.vb",
                                    Optional Da_Zoo As Boolean = False
                                 ) As Integer

        Dim Analisi_Testata_Cod As Integer
        Dim flagConnessione, flagTransazione As Boolean

        Try
            Utility.VerificaApriTransazione(objParametri, flagConnessione, flagTransazione)

            'creo una nuova Analisi Testata Cod
            Dim objSequenze As New AgronicaCoreDataProvider.Agro_Sequenze
            'TODO: REFACTOR = Togli uso Session!!!
            Analisi_Testata_Cod = objSequenze.NuovoId_Tabella("Analisi_Testata", HttpContext.Current.Session("BaseCode"), HttpContext.Current.Session("TopCode"), objParametri)
            oggetto.Analisi_Testata_Cod = Analisi_Testata_Cod

            Dim STR_Varieta As String = ""
            If Not Da_Zoo Then
                'ricavo la stringa per le varietà
                'leggo la varietà
                Dim objPDC_Analisi As New AgronicaCorePianidiCampionamentoDAL.PDC_Analisi_R
                Dim DT_Varieta As DataTable
                DT_Varieta = objPDC_Analisi.Leggi_VarietaQuestaAnalisi(oggetto.Id_PDC_Testata,
                                                                   oggetto.ID_PDC_Dettagli,
                                                                   oggetto.ID_PDC_Campione,
                                                                   objParametri)
                'lbl_Varieta
                If DT_Varieta.Rows.Count > 0 Then

                    STR_Varieta = "Specie: <b>" & DT_Varieta.Rows(0).Item("Veg_Des") & "</b>" &
                              " Varietà: <b>" & DT_Varieta.Rows(0).Item("Cul_Des") & "</b>"

                    'a questo punto guardo tutte le varietà del lotto
                    Dim DT_AltreV As DataTable
                    DT_AltreV = objPDC_Analisi.Leggi_AltreVarietaLotto(oggetto.Id_PDC_Testata, DT_Varieta.Rows(0).Item("ID_LFO"), DT_Varieta.Rows(0).Item("Cul_Cod"), objParametri)

                    If DT_AltreV.Rows.Count > 0 Then
                        STR_Varieta = STR_Varieta + "<br/>Altre varietà del lotto: <b>"
                    End If

                    For i = 0 To DT_AltreV.Rows.Count - 1
                        If i = DT_AltreV.Rows.Count - 1 Then
                            STR_Varieta = STR_Varieta + DT_AltreV.Rows(i).Item("Cul_Des") + "</b>"
                        Else
                            STR_Varieta = STR_Varieta + DT_AltreV.Rows(i).Item("Cul_Des") + ", "
                        End If
                    Next
                End If
            End If


            'Deduco il tipo di analisi in base allo schema applicato
            'TODO: qui ora bisogna leggere il tipo memorizzato nella nuova colonna su PDC_Analisi.Analisi_Tipologia_Cod (quindi non è più necessario andare a recuperarlo sullo schema di analisi)
            Dim tipoAnalisi As enum_AnalisiTipo = enum_AnalisiTipo.Analisi_Fitofarmaci
            If Da_Zoo Then
                tipoAnalisi = enum_AnalisiTipo.Analisi_Del_Sangue
            Else
                If oggetto.Analisi_Tipologia_Cod <> 0 Then
                    Dim objAnalisiTipo_R As New AgronicaCoreAnagrafeDAL.Analisi_Tipologia_R
                    Dim dtTipoAnalisi As DataTable = objAnalisiTipo_R.Leggi(oggetto.Analisi_Tipologia_Cod, 0, "", "", objParametri)
                    If Not IsNothing(dtTipoAnalisi) AndAlso dtTipoAnalisi.Rows.Count = 1 Then
                        tipoAnalisi = dtTipoAnalisi.Rows(0).Item("Analisi_Tipologia_Tipo")
                    End If
                End If
            End If


            'scrivo una nuova Analisi_testata 
            Dim objAnalisi As New AgronicaCoreAnagrafeDAL.Analisi_Testata_W
            objAnalisi.Scrivi(oggetto.Analisi_Testata_Cod,
                              0, oggetto.Analisi_Testata_Des,
                              oggetto.Data_Richiesta_Analisi,
                              AGRODATAFINE,
                              0, 0, "", "", "", "", "", STR_Varieta, "", "", "",
                              tipoAnalisi,
                              0, 0, oggetto.Data_Richiesta_Analisi, AGRODATAINIZIO, AGRODATAFINE, objParametri)

            Dim objPdcAnalisi As New AgronicaCorePianidiCampionamentoDAL.PDC_Analisi_W
            objPdcAnalisi.Scrivi(oggetto.Id_PDC_Testata,
                                 oggetto.ID_PDC_Dettagli,
                                 oggetto.ID_PDC_Campione,
                                 oggetto.Analisi_Testata_Cod,
                                 oggetto.PDC_Stato_Analisi,
                                 oggetto.Cod_Risum,
                                 oggetto.Analisi_Tipologia_Cod,
                                 oggetto.Altre_Molecole,
                                 oggetto.Data_Richiesta_Analisi,
                                 oggetto.Note_Richiesta_Analisi,
                                 oggetto.Piva_FornitoreFatturazione,
                                 oggetto.Tipo_Campione_Cod,
                                 False,
                                 objParametri,
                                 Analisi_Tipologia_Tipo:=oggetto.Analisi_Tipologia_Tipo,
                                 Altre_Molecole_Cod:=oggetto.Altre_Molecole_Cod)

            Dim objAgronicaLogAnalisiW As New AgronicaCoreAnagrafeDAL.AgronicaLogAnalisi_W
            Dim tzh As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}
            Dim object_data = JsonConvert.SerializeObject(oggetto, tzh)
            objAgronicaLogAnalisiW.Scrivi(enum_TipoOperazioneDB.Scrittura,
                                          tipoAnalisi,
                                          oggetto.Analisi_Testata_Cod,
                                          oggetto.Analisi_Testata_Des,
                                          oggetto.Data_Richiesta_Analisi,
                                          NoteLog, enum_Id_Servizio.GiasOnline,
                                          objParametri,
                                          object_data:=object_data)

            If Not Da_Zoo Then
                VerificaCodiceAnalisi(oggetto.Analisi_Testata_Cod, oggetto.Analisi_Testata_Des, objParametri)
            End If

            Utility.VerificaChiudiTransazione(objParametri, flagTransazione)

        Catch ex As Exception
            Utility.VerificaAnnullaTransazione(objParametri, flagTransazione)
            Throw New Exception(ex.Message)
            Analisi_Testata_Cod = -1
        Finally
            Utility.VerificaChiudiConnessione(objParametri, flagConnessione)
        End Try

        Return Analisi_Testata_Cod

    End Function

    Public Shared Sub VerificaCodiceAnalisi(ByVal Analisi_Testata_Cod As Integer,
                                            ByVal Analisi_Testata_Des As String,
                                            ByVal objParametri As AgronicaCoreParametri)

        Dim objWebConfig As New AgronicaCoreGestioneRichieste.AgroWebConfig
        If objWebConfig.Codici_Analisi_PDC_x_Fruttagel = True Then
            'controllo che parta con I,B, blacnk
            If Analisi_Testata_Des.Length > 7 Then
                Select Case Left(Analisi_Testata_Des, 1)
                    Case "I", "i", "B", "b"
                        'ok
                    Case Else
                        Throw New Exception(" il carattere iniziale deve essere compreso tra I,B, e spazio vuoto")
                End Select
            Else
                If Analisi_Testata_Des.Length < 7 Then
                    Throw New Exception("problemi con la generazione codice ----" & Analisi_Testata_Des & "---")
                End If
            End If

            Dim objPdcCodice As New AgronicaCorePianidiCampionamentoDAL.PDC_Codice_Analisi_Fruttagel_R
            Dim progressivo As String = objPdcCodice.GetProgressivo_Specie(0, Analisi_Testata_Cod, objParametri)

            Dim pdc_analisi As New AgronicaCorePianidiCampionamentoDAL.PDC_Analisi_R
            Dim anno As Integer = pdc_analisi.GetAnnoCampione(Analisi_Testata_Cod, objParametri)

            Dim prog As Integer
            If IsNumeric(Right(Analisi_Testata_Des, 4)) Then
                If Right(progressivo, 4) <= Right(Analisi_Testata_Des, 4) Then
                    prog = Right(Analisi_Testata_Des, 4)
                    Dim objAn As New AgronicaCorePianidiCampionamentoDAL.PDC_Analisi_R
                    Dim veg_Cod As Integer = objAn.GetVeg_Cod(Analisi_Testata_Cod, objParametri)
                    Dim objPdcCodiceW As New AgronicaCorePianidiCampionamentoDAL.PDC_Codice_Analisi_Fruttagel_W
                    objPdcCodiceW.ModificaProgressivo(0, Analisi_Testata_Cod, veg_Cod, prog, anno, objParametri)
                End If
            End If
        End If

    End Sub

    Public Shared Function CaricaAnalisi(ByVal Id_Testata As Integer, ByVal ID_PDC_Dettagli As Integer, ByVal ID_PDC_Campione As Integer, dtAnalisi As DataTable) As List(Of PDC_Analisi)

        Dim listaAnalisi As New List(Of PDC_Analisi)

        Dim objPDC_Analisi As New AgronicaCorePianidiCampionamentoDAL.PDC_Analisi_R
        Dim rows = dtAnalisi.Select("Id_PDC_Testata = " & UtilityProvider.Agro_SQL_SaveNum(Id_Testata) & " AND ID_PDC_Dettagli = " & UtilityProvider.Agro_SQL_SaveNum(ID_PDC_Dettagli) & " AND ID_PDC_Campione = " & UtilityProvider.Agro_SQL_SaveNum(ID_PDC_Campione) & " ").ToList()

        For Each dr As DataRow In rows
            Dim obj As New PDC_Analisi
            obj.Id_PDC_Testata = dr.Item("Id_PDC_Testata")
            obj.ID_PDC_Dettagli = dr.Item("ID_PDC_Dettagli")
            obj.ID_PDC_Campione = dr.Item("ID_PDC_Campione")
            obj.Analisi_Testata_Cod = dr.Item("Analisi_Testata_Cod")
            obj.Analisi_Testata_Des = dr.Item("Analisi_Testata_Des")
            obj.FornitoreFatturazione_Des = dr.Item("FornitoreFatturazione_Des")
            obj.Piva_FornitoreFatturazione = dr.Item("Piva_FornitoreFatturazione")

            obj.PDC_Stato_Analisi = dr.Item("PDC_Stato_Analisi")
            obj.PDC_Stato_Validazione = dr.Item("PDC_Stato_Validazione")
            obj.PDC_Stato_Pubblicazione = dr.Item("PDC_Stato_Pubblicazione")
            obj.PDC_Stato_Analisi_Des = dr.Item("PDC_Stato_Analisi_Des")
            obj.Cod_Contatto = dr.Item("Cod_Contatto")
            obj.Cod_Risum = dr.Item("Cod_Risum")
            obj.Cod_Risum_Des = dr.Item("Cod_Risum_Des")
            If dr.Item("Analisi_Tipologia_Cod") = -1 Then
                obj.Analisi_Tipologia_Cod = -1
                obj.Analisi_Tipologia_Des = "Tipologia analisi non Selezionata"
                obj.Analisi_Tipologia_Tipo = -1
            Else
                obj.Analisi_Tipologia_Cod = dr.Item("Analisi_Tipologia_Cod")
                obj.Analisi_Tipologia_Des = dr.Item("Analisi_Tipologia_Des")
                obj.Analisi_Tipologia_Tipo = dr.Item("Analisi_Tipologia_Tipo")
            End If

            obj.Tipo_Campione_Cod = dr.Item("Tipo_Campione_Cod")
            obj.Tipo_Campione_Des = dr.Item("Tipo_Campione_Des")


            obj.Altre_Molecole = dr.Item("Altre_Molecole")
            obj.Altre_Molecole_Cod = dr.Item("Altre_Molecole_Cod")
            obj.Data_Richiesta_Analisi = dr.Item("Data_Richiesta_Analisi")
            If Not IsDBNull(dr.Item("Note_Richiesta_Analisi")) Then
                obj.Note_Richiesta_Analisi = dr.Item("Note_Richiesta_Analisi")
            Else
                obj.Note_Richiesta_Analisi = ""
            End If
            obj.ID_NC = If(IsDBNull(dr.Item("ID_NC")), -1, dr.Item("ID_NC"))
            'aggiungo l'oggetto alla lista
            listaAnalisi.Add(obj)
        Next

        Return listaAnalisi

    End Function

    Public Shared Sub InviaMail(ByVal Cod_Risum As Integer, ByVal CodiceAnalisi As String, ByVal Analisi_Testata_cod As String,
                                ByRef objParametri_Server As AgronicaCoreParametri)

        'leggo i dati per l invio
        Dim objLabOpzioni As New AgronicaCoreAnagrafeDAL.Laboratori_Opzioni_R
        Dim DT_Lab As DataTable = objLabOpzioni.Leggi(Cod_Risum, 0, objParametri_Server)

        Dim RispondiA As String = ""
        Dim MailA As String = ""
        Dim MailCC As String = ""
        Dim Bool_Attiva As Boolean = False

        For Each dr As DataRow In DT_Lab.Rows
            Select Case dr.Item("Tipo_Opzione")
                Case enum_Opzioni_Laboratori.Bool_Mail_Automatica_Invio_ALERT
                    If dr.Item("Valore") = True Then
                        Bool_Attiva = True
                    End If
                Case enum_Opzioni_Laboratori.Mail_Automatica_A_ALERT
                    MailA = dr.Item("Valore")
                Case enum_Opzioni_Laboratori.Mail_Automatica_CC_ALERT
                    MailCC = dr.Item("Valore")
                Case enum_Opzioni_Laboratori.Mail_Rispondi_A_ALERT
                    RispondiA = dr.Item("Valore")
            End Select
        Next

        If Bool_Attiva = False Then
            Exit Sub
        End If

        If RispondiA.Trim = "" Then
            Throw New Exception("RispondiA ALERT non Configurato")
        End If
        If MailA.Trim = "" Then
            Throw New Exception("MailA ALERT non Configurato")
        End If

        Dim OggettoMail As String = "Analisi Inserita Codice: " & CodiceAnalisi

        Dim objG As New AgronicaCorePianidiCampionamentoDAL.PDC_Dettagli_R
        Dim dt_dett As DataTable = objG.Leggi_x_mail_risposta_laboratori(Analisi_Testata_cod, objParametri_Server)
        Dim dr_dett As DataRow = dt_dett.Rows(0)

        'controllo se in rag_soc è presente la piva 
        If IsNumeric(dr_dett.Item("Rag_soc").split("-")(0)) Then
            Dim str_app As String = ""
            For j As Integer = 1 To dr_dett.Item("Rag_Soc").ToString.Split("-").Length - 1
                If str_app <> "" Then
                    str_app &= "-"
                End If
                str_app &= dr_dett.Item("Rag_Soc").split("-")(j)
            Next
            dr_dett.Item("Rag_Soc") = str_app.Trim()
        End If

        'controllo se in rag_soc_padre è presente la piva 
        If Not IsDBNull(dr_dett.Item("Rag_soc_padre")) AndAlso IsNumeric(dr_dett.Item("Rag_soc_padre").split("-")(0)) Then
            Dim str_app As String = ""
            For j As Integer = 1 To dr_dett.Item("Rag_soc_padre").ToString.Split("-").Length - 1
                If str_app <> "" Then
                    str_app &= "-"
                End If
                str_app &= dr_dett.Item("Rag_soc_padre").split("-")(j)
            Next
            dr_dett.Item("Rag_soc_padre") = str_app.Trim()
        End If

        If Not String.IsNullOrEmpty(dr_dett.Item("Rag_Soc_Padre")) Then
            dr_dett.Item("Rag_Soc") &= " (" & dr_dett.Item("Rag_Soc_Padre") & ")"
        End If

        Dim TestoMail As String = "Analisi Inserita Codice: <b>" & CodiceAnalisi & "</b><br>"
        TestoMail &= "Piano Campionamento: <b>" & dr_dett.Item("PDC_Testata_Des") & "</b><br>"

        If dr_dett.Item("codicestabilimento") <> "" Then
            TestoMail &= "Stabilimento: <b>" & dr_dett.Item("codicestabilimento") & "</b><br>"
        End If

        TestoMail &= "Codice campione: <b>" & dr_dett.Item("Codice_Campione") & "</b><br><br>"
        TestoMail &= "Note campione: <b>" & dr_dett.Item("Note_Campione") & "</b><br><br>"

        TestoMail &= "Azienda: <b>" & dr_dett.Item("Rag_soc") & "</b><br>"
        TestoMail &= "Impianto: <b>" & dr_dett.Item("ListaCodiciApp") & "</b><br>"

        'blocco dettagli principi attivi rilevati
        Dim objP As New AgronicaCorePianidiCampionamentoDAL.PDC_Analisi_R
        Dim dtPA As DataTable = objP.Leggi_PADESoDESCRIZIONE_Valore(Analisi_Testata_cod, objParametri_Server)
        If (dtPA.Rows.Count > 0) Then
            TestoMail = TestoMail + "<br><br>Dettaglio Principi attivi rilevati:<br>"
            For i As Integer = 0 To dtPA.Rows.Count - 1
                TestoMail &= "<b>" & dtPA.Rows(i).Item("Nome") & "</b>   " &
                            dtPA.Rows(i).Item("analisi_dettaglio_valore_1") & " " & dtPA.Rows(i).Item("analisi_dettaglio_valore_2") & "<br>"
            Next
        End If

        'Aggiungo gli allegati
        Dim objAllegati As New AgronicaCoreAnagrafeDAL.Allegati_EntitaxDocumenti_R
        Dim dtAllegati As DataTable = objAllegati.Leggi_AllegatiAnalisi(Analisi_Testata_cod, "", "", "", objParametri_Server)

        Dim objConf As New AgronicaCoreVarieDAL.Configurazione_Siti_R
        Dim PathCompleto As String = objConf.Leggi(0, "GestioneAllegati_Repository", "", "", objParametri_Server).Rows(0).Item("Valore") & "Analisi\"

        Dim listaAllegati As String() = (From r As DataRow In dtAllegati.Rows Select PathCompleto & CStr(r.Item("Allegati_documenti_nomefile"))).ToArray()


        Dim m As New AgronicaCoreUtility.Mail()
        Dim res As String = m.invia(objParametri_Server, RispondiA, MailA, MailCC, "", OggettoMail, TestoMail, True, listaAllegati)

        If res <> "" Then
            Throw New Exception(res)
        End If

    End Sub

    ' inizializza progressivi anno per i codici analisi fruttagel
    Public Shared Sub InitCodiciAnalisiFruttagel(ByRef objParametri_Server As AgronicaCoreParametri)
        Dim annoCorrente As Integer = Date.Now.Year
        Dim objPdcCodiceR As New PDC_Codice_Analisi_Fruttagel_R
        Dim annoFrom = objPdcCodiceR.LeggiAnnoProgressivi(objParametri_Server)
        If annoFrom > 0 AndAlso annoFrom < annoCorrente Then
            Dim objPdcCodiceW As New PDC_Codice_Analisi_Fruttagel_W
            For annoTo = annoFrom + 1 To annoCorrente
                objPdcCodiceW.ScriviProgressiviAnno(annoFrom, annoTo, objParametri_Server)
            Next
        End If
    End Sub

    ' importa gli utenti dalla vecchia alla nuova gestione laboratori
    Public Shared Sub InitUtentiLaboratori(ByRef objParametri_Server As AgronicaCoreParametri)
        Dim objLaboratoriUtenti As New AgronicaCoreAnagrafeDAL.Laboratori_Utenti_R
        Dim dtUtentiLaboratorio = objLaboratoriUtenti.LeggiLaboratoriUtente(0, "", enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)
        If dtUtentiLaboratorio.Rows.Count = 0 Then
            dtUtentiLaboratorio = objLaboratoriUtenti.Leggi(0, "", enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)
            If dtUtentiLaboratorio.Rows.Count > 0 Then
                Dim objLaboratoriW As New AgronicaCoreAnagrafeDAL.Laboratori_Utenti_W
                For Each row In dtUtentiLaboratorio.Rows
                    objLaboratoriW.ScriviLaboratoriUtenti(row("cod_risum"), row("username"), AGRODATAINIZIO, AGRODATAFINE, objParametri_Server)
                Next
                'objLaboratoriW.Cancella(Laboratorio, "", "", objParametri_Server)
            End If
        End If
    End Sub

End Class
