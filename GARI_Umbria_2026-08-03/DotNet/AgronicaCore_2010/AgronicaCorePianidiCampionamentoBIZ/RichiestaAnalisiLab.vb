Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCorePianidiCampionamentoDAL
Imports AgronicaCoreUtility
Imports Newtonsoft.Json

Public Class RichiestaAnalisiLab

    Public Sub New()
        Sam_Rip = "N"   'TODO: per il momento non serve, quindi lo mettiamo fisso?!?
    End Sub

    Public Property PivaSuperUser As String
    Public Property ID_PDC_Testata As Integer
    Public Property ID_PDC_Dettagli As Integer
    Public Property ID_PDC_Campione As Integer
    Public Property Analisi_Testata_Cod As Integer
    Public Property Chiave As String
    Public Property PivaOwner As String
    Public Property Piva As String
    Public Property Piva_OP As String
    Public Property Rag_Soc_OP As String
    Public Property Kpin As String
    Public Property Block As String
    Public Property Grower_Number As String
    Public Property Indirizzo As String
    Public Property Frazione As String
    Public Property Comune As String
    Public Property Localita As String
    Public Property Comune_Completo As String
    Public Property CAP As String
    Public Property Provincia As String
    Public Property Regione_Cod As String
    Public Property Regione_Des As String
    Public Property Nazione As String
    Public Property Pro_Cod_Istat As String
    Public Property Com_Cod_Istat As String
    Public Property GGN As String
    Public Property CodiceFornitore As String
    Public Property Rag_Soc As String
    Public Property Sa_Nome As String
    Public Property App_Nome As String
    Public Property Sup_Imp As String
    Public Property CapitolatoPrivato As String
    Public Property Regolamento As String
    Public Property Tecnico_Campionamento As String
    Public Property Tecnico_Campionamento_Tel As String
    Public Property Data_Campionamento As DateTime
    Public Property Codice_Campione As String
    Public Property Note_Impianto As String
    Public Property Tecnico_Campione As String
    Public Property Note_Campione As String
    Public Property Codice_Progressivo_Inizio_Anno As String
    Public Property Codice_Anno As String
    Public Property PuntoPrelievo As String
    Public Property Num_Prodotti As Integer
    Public Property Motivo_Campione_Cod As Integer
    Public Property Motivo_Campione_Des As String
    Public Property Cod_Risum As String
    Public Property Data_Richiesta_Analisi As DateTime
    Public Property Note_Richiesta_Analisi As String
    Public Property Fornitore_Di_Fatturazione As String
    Public Property Laboratorio As String
    Public Property Analisi_Tipologia_Tipo As Integer
    Public Property Analisi_Tipologia_Des As String
    Public Property Altre_Molecole As String
    Public Property Altre_Molecole_Cod As String
    Public Property Analisi_Testata_Note1 As String
    Public Property Veg_Des As String
    Public Property Cul_Des As String
    Public Property Tipologia_Varietale As String
    Public Property Codice_2 As String
    Public Property Tipo_Campione_Apofruit As String
    Public Property Nome_Stabilimento As String
    Public Property Veg_Cod As String
    Public Property Cul_Cod As Integer
    Public Property Grva_Cod As String
    Public Property Piva_FornitoreFatturazione As String
    Public Property CodiceStabilimento As String
    Public Property Tipo_Campione As Integer
    Public Property Id_Lfo As Integer
    Public Property Rag_Soc_Padre As String

    Public Property Area As String      'Usato solo per Zespri - Pedonlab
    Public Property BlockRR As String   'Usato solo per Zespri - Pedonlab
    Public Property Sam_Rip As String   'Usato solo per Zespri - Pedonlab

    ''' <summary>
    ''' Restituisce un dizionario con Chiave = Proprietà generica, Valore = nome proprietà in restful api
    ''' </summary>
    Public Shared Function Mapper(ByVal obj As RichiestaAnalisiLab, ByVal tipoAPI As String) As Dictionary(Of String, String)

        Dim mapDict As New Dictionary(Of String, String)

        Select Case tipoAPI

            Case PDC_REST_API_PEDONLAB

                mapDict("PivaSuperUser") = "pivaSuperUser"
                mapDict("Chiave") = "chiave"
                mapDict("Codice_Campione") = "codiceCampione"
                mapDict("Piva_OP") = "facility"
                mapDict("Grower_Number") = "kpin"
                mapDict("Piva") = "piva"
                mapDict("Rag_Soc") = "growerName"
                mapDict("Area") = "area"
                mapDict("Block") = "block"
                mapDict("Cul_Cod") = "variety"      'serve anche mapping
                mapDict("Num_Prodotti") = "nFruits"
                mapDict("Sup_Imp") = "ha"
                mapDict("Indirizzo") = "address"
                mapDict("CAP") = "cap"
                mapDict("Comune_Completo") = "loc"
                mapDict("Provincia") = "province"
                mapDict("Regione_Cod") = "region"   'serve anche mapping
                mapDict("Note_Campione") = "otherIdentifications"
                mapDict("Tecnico_Campionamento_Tel") = "telephone"
                mapDict("Note_Richiesta_Analisi") = "notes"

                'In questa API se è fitofarmaci  sarà test = "R" (lo imposto dopo con la sostituzione in-place) e residueScheme = Analisi_Tipologia_Des
                '              se è merceologica sarà test = Analisi_Tipologia_Des e residueScheme = ""

                'Visto che le voci le devo avere entrambe qui le inverto solamente e poi la sostituzione 8 = "R" e 11 = "" lo farò dopo

                Select Case obj.Analisi_Tipologia_Tipo

                    Case enum_AnalisiTipo.Analisi_Fitofarmaci
                        mapDict("Analisi_Tipologia_Tipo") = "test"
                        mapDict("Analisi_Tipologia_Des") = "residueScheme"

                    Case enum_AnalisiTipo.Analisi_Merceologiche
                        mapDict("Analisi_Tipologia_Des") = "test"
                        mapDict("Analisi_Tipologia_Tipo") = "residueScheme"    'TODO: se è merceologica, lo schema di analisi va messo su test e qui non c'è nulla

                End Select

                mapDict("Altre_Molecole_Cod") = "residueSingle"   'TODO: serve anche mapping

                mapDict("BlockRR") = "blockRR"
                mapDict("PivaOwner") = "owner"
                mapDict("Data_Campionamento") = "sampling"
                mapDict("Data_Richiesta_Analisi") = "dataRichiestaAnalisi"
                mapDict("GGN") = "GGN"
                mapDict("Nazione") = "nation"                       'serve anche mapping
                mapDict("Piva_FornitoreFatturazione") = "invoice"
                mapDict("Motivo_Campione_Cod") = "samType"          'Serve anche mapping
                mapDict("PuntoPrelievo") = "samPick"
                mapDict("Sam_Rip") = "samRip"

            Case Else
                Throw New NotImplementedException(String.Format("il tipo API {0} non è implementato", tipoAPI))
        End Select

        Return mapDict

    End Function

    Public Shared Sub SostituzioniMappature(ByRef objGias As RichiestaAnalisiLab,
                                            ByRef objApiRichiesta As Analisi_API_Richiesta,
                                            ByVal tipoAPI As String,
                                            ByRef objParametri As AgronicaCoreParametri)

        Const nomeRoutine = "SostituzioniMappature()"

        Try

            Select Case tipoAPI

                Case PDC_REST_API_PEDONLAB

                    'TODO: Mappatura test / residueScheme (ora qui c'è il codice tipologia analisi)
                    If objGias.Analisi_Tipologia_Tipo = enum_AnalisiTipo.Analisi_Fitofarmaci Then
                        objApiRichiesta.richiesta.Find(Function(x) x.chiave = "test").valore = "R"

                    ElseIf objGias.Analisi_Tipologia_Tipo = enum_AnalisiTipo.Analisi_Merceologiche Then
                        objApiRichiesta.richiesta.Find(Function(x) x.chiave = "residueScheme").valore = ""
                    End If


                    'TODO: Mappatura Varietà

                    If objGias.Cul_Cod <> 0 Then

                        'TODO: mappatura varietà fisso a codice? oppure sempre sulla stessa tabella, come adesso o su tabella apposita?

                        Dim newVariety As String = MappaVarieta(objGias.Cod_Risum, objGias.Cul_Cod, objParametri)
                        objApiRichiesta.richiesta.Find(Function(x) x.chiave = "variety").valore = newVariety

                    End If


                    'Mappatura Regione
                    If Not String.IsNullOrEmpty(objGias.Regione_Cod) AndAlso objGias.Regione_Cod <> "000" Then
                        Dim newRegion As String = MappaRegione(objGias.Cod_Risum, objGias.Regione_Cod, objParametri)
                        objApiRichiesta.richiesta.Find(Function(x) x.chiave = "region").valore = newRegion
                    End If

                    'Mappatura Nazione
                    If Not String.IsNullOrEmpty(objGias.Nazione) Then
                        Dim newNation As String = MappaNazione(objGias.Cod_Risum, objGias.Nazione, objParametri)
                        objApiRichiesta.richiesta.Find(Function(x) x.chiave = "nation").valore = newNation
                    End If


                    'TODO: Mappatura Molecola

                    If Not String.IsNullOrEmpty(objGias.Altre_Molecole_Cod) Then

                        Dim elencoMolecoleConvertite As String = ""

                        Dim molecoleSplit As String() = objGias.Altre_Molecole_Cod.Split(New Char() {"|"c}, StringSplitOptions.RemoveEmptyEntries)
                        If molecoleSplit.Length > 0 Then

                            Dim newElencoMolecole As New List(Of String)

                            For Each molecola As String In molecoleSplit
                                If CInt(molecola) > 0 Then

                                    'TODO: mappa molecole (stessa tabella, come è adesso o tabella apposita)?!?

                                    Dim newMolecola As String = MappaMolecola(objGias.Cod_Risum, molecola, objParametri)
                                    newElencoMolecole.Add(newMolecola)

                                ElseIf CInt(molecola) < 0 Then
                                    Throw New NotImplementedException("Famiglia di principi attivi non gestita")
                                Else
                                    Throw New Exception("Codice molecola è 0")
                                End If
                            Next

                            If newElencoMolecole.Count > 0 Then
                                elencoMolecoleConvertite = String.Join(",", newElencoMolecole.ToArray())
                            End If

                        End If

                        objApiRichiesta.richiesta.Find(Function(x) x.chiave = "residueSingle").valore = elencoMolecoleConvertite

                    End If

                    'Mappatura Motivo campionamento
                    If objGias.Motivo_Campione_Cod <> 0 Then
                        Dim newMotivoCamp As String = MappaMotivoCampione(objGias.Cod_Risum, objGias.Motivo_Campione_Cod, objParametri)
                        objApiRichiesta.richiesta.Find(Function(x) x.chiave = "samType").valore = newMotivoCamp
                    End If

            End Select

        Catch ex As Exception
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
        End Try

    End Sub

    Private Shared Function MappaVarieta(ByVal codRisUm As Integer, ByVal culCodGias As String, ByRef objParametri As AgronicaCoreParametri) As String

        Dim varietaAltro As String = ""

        Dim objMappature As New PDC_Mappature_R
        Dim numRis As Integer = objMappature.Leggi_Mappatura_ALTRO_From_GIAS(codRisUm, 0,
                                                                             PDC_MAPPATURA_CULTIVAR,
                                                                             culCodGias, varietaAltro,
                                                                             objParametri)
        If numRis <> 1 OrElse varietaAltro = "" Then
            Throw New Exception(String.Format("Impossibile eseguire mapping e transcodifica della Varietà {0}.", culCodGias))
        End If

        Return varietaAltro

    End Function

    Private Shared Function MappaRegione(ByVal codRisUm As Integer, ByVal regCodGias As String, ByRef objParametri As AgronicaCoreParametri) As String

        Dim regioneAltro As String = ""

        Dim objMappature As New PDC_Mappature_R
        Dim numRis As Integer = objMappature.Leggi_Mappatura_ALTRO_From_GIAS(codRisUm, 0,
                                                                             PDC_MAPPATURA_REGIONE,
                                                                             regCodGias, regioneAltro,
                                                                             objParametri)
        If numRis <> 1 OrElse regioneAltro = "" Then
            Throw New Exception(String.Format("Impossibile eseguire mapping e transcodifica della Regione {0}.", regCodGias))
        End If

        Return regioneAltro

    End Function

    Private Shared Function MappaNazione(ByVal codRisUm As Integer, ByVal nazioneCodGias As String, ByRef objParametri As AgronicaCoreParametri) As String

        Dim nazioneAltro As String = ""

        Dim objMappature As New PDC_Mappature_R
        Dim numRis As Integer = objMappature.Leggi_Mappatura_ALTRO_From_GIAS(codRisUm, 0,
                                                                             PDC_MAPPATURA_NAZIONE,
                                                                             nazioneCodGias, nazioneAltro,
                                                                             objParametri)
        If numRis <> 1 OrElse nazioneAltro = "" Then
            Throw New Exception(String.Format("Impossibile eseguire mapping e transcodifica della Nazione {0}.", nazioneCodGias))
        End If

        Return nazioneAltro

    End Function

    Private Shared Function MappaMolecola(ByVal codRisUm As Integer, ByVal paCodGias As String, ByRef objParametri As AgronicaCoreParametri) As String

        Dim molecolaAltro As String = ""

        Dim objMappature As New PDC_Mappature_R
        Dim numRis As Integer = objMappature.Leggi_Mappatura_ALTRO_From_GIAS(codRisUm, 0,
                                                                             PDC_MAPPATURA_MOLECOLA,
                                                                             paCodGias, molecolaAltro,
                                                                             objParametri)
        If numRis <> 1 OrElse molecolaAltro = "" Then
            Throw New Exception(String.Format("Impossibile eseguire mapping e transcodifica della Molecola {0}.", paCodGias))
        End If

        Return molecolaAltro

    End Function

    Private Shared Function MappaMotivoCampione(ByVal codRisUm As Integer, ByVal motivoCampCodGias As String, ByRef objParametri As AgronicaCoreParametri) As String

        Dim motivoCampioneAltro As String = ""

        Dim objMappature As New PDC_Mappature_R
        Dim numRis As Integer = objMappature.Leggi_Mappatura_ALTRO_From_GIAS(codRisUm, 0,
                                                                             PDC_MAPPATURA_MOTIVO_CAMP,
                                                                             motivoCampCodGias, motivoCampioneAltro,
                                                                             objParametri)
        If numRis <> 1 OrElse motivoCampioneAltro = "" Then
            Throw New Exception(String.Format("Impossibile eseguire mapping e transcodifica del Motivo Campionamento {0}.", motivoCampCodGias))
        End If

        Return motivoCampioneAltro

    End Function

#Region "Richiesta Analisi Zoo"
    Public Shared Sub ScriviRichiestaAnalisiZoo(dati As String, objParametri_Server As AgronicaCoreParametri)

        Dim flagConnessione As Boolean = True
        Dim flagTransazione As Boolean = True

        Try
            Utility.VerificaApriTransazione(objParametri_Server, flagConnessione, flagTransazione)

            Dim datiRichiesta = JsonConvert.DeserializeObject(dati)

            Dim ID_PDC_Testata As Integer = datiRichiesta.GetValue("ID_PDC_Testata")
            Dim Cod_Risum As Integer = datiRichiesta.GetValue("Cod_Risum")
            Dim Analisi_Tipologia_Cod As Integer = datiRichiesta.GetValue("Analisi_Tipologia_Cod")
            Dim Note_Richiesta_Analisi As String = datiRichiesta.GetValue("Note_Richiesta_Analisi")
            Dim Analisi_Tipologia_Tipo As Integer = datiRichiesta.GetValue("Analisi_Tipologia_Tipo")
            Dim Data_Richiesta_Analisi As String = datiRichiesta.GetValue("Data_Richiesta_Analisi")

            Dim objCampioni As New AgronicaCorePianidiCampionamentoDAL.PDC_Campioni_R
            Dim dt = objCampioni.LeggiCampionixRichiestaAnalisi(ID_PDC_Testata, 0, 0, "", "", objParametri_Server)
            dt = dt.Select("ID_Stato_Analisi = 0").CopyToDataTable

            Dim listAnalisiTestataCreate As New List(Of Integer)
            For Each campione In dt.Rows
                Dim ID_PDC_Dettagli As Integer = campione.item("ID_PDC_Dettagli")
                Dim ID_PDC_Campione As Integer = campione.item("ID_PDC_Campione")

                Dim objAnalisi As New PDC_Analisi With {
                    .Id_PDC_Testata = ID_PDC_Testata,
                    .ID_PDC_Dettagli = ID_PDC_Dettagli,
                    .ID_PDC_Campione = ID_PDC_Campione,
                    .Cod_Risum = Cod_Risum,
                    .Analisi_Tipologia_Cod = Analisi_Tipologia_Cod,
                    .Altre_Molecole = "",
                    .Data_Richiesta_Analisi = Data_Richiesta_Analisi,
                    .Analisi_Testata_Des = "",
                    .Note_Richiesta_Analisi = Note_Richiesta_Analisi,
                    .Piva_FornitoreFatturazione = objParametri_Server.PivaSuperUser,
                    .Analisi_Tipologia_Tipo = Analisi_Tipologia_Tipo,
                    .PDC_Stato_Analisi = enum_PDC_Stato_Analisi.Analisi_In_Corso
                }

                Dim Analisi_Testata_Cod = PDC_Analisi_Helper.Nuova(objAnalisi, objParametri_Server, NoteLog:="Nuova Richiesta Analisi Zoo PDC", Da_Zoo:=True)
                listAnalisiTestataCreate.Add(Analisi_Testata_Cod)

            Next

            Dim objPDC As New AgronicaCorePianidiCampionamentoDAL.PDC_W
            objPDC.Aggiorna_Stato(ID_PDC_Testata, enum_PDC_Stato_Testata.Richiesta_Analisi_Zoo_Inviata, objParametri_Server)

            InviaMailRichiestaAnalisiZoo(ID_PDC_Testata, Cod_Risum, listAnalisiTestataCreate, objParametri_Server)

            Utility.VerificaChiudiTransazione(objParametri_Server, flagTransazione)

        Catch ex As GiasException
            Utility.VerificaAnnullaTransazione(objParametri_Server, flagTransazione)
            Throw New GiasException(ex.Message)
        Catch ex As Exception
            Utility.VerificaAnnullaTransazione(objParametri_Server, flagTransazione)
            Throw New Exception(ex.Message)
        Finally
            Utility.VerificaChiudiConnessione(objParametri_Server, flagConnessione)
        End Try

    End Sub

    Private Shared Sub InviaMailRichiestaAnalisiZoo(ByVal ID_PDC_Testata As Integer,
                                                    ByVal Cod_Risum_Lab As Integer,
                                                    ByVal listaAnalisiCreate As List(Of Integer),
                                                    ByRef objParametri_Server As AgronicaCoreParametri)

        'leggo i dati per l'invio
        Dim objLabOpzioni As New AgronicaCoreAnagrafeDAL.Laboratori_Opzioni_R
        Dim dtLab As DataTable = objLabOpzioni.Leggi(Cod_Risum_Lab, 0, objParametri_Server)

        Dim RispondiA As String = ""
        Dim MailA As String = ""
        Dim MailCC As String = ""
        Dim Bool_Attiva As Boolean = False
        Dim Firma As String = ""
        Dim Oggetto As String = ""
        Dim Lingua As String = "IT"

        For i As Integer = 0 To dtLab.Rows.Count - 1
            Select Case dtLab.Rows(i).Item("Tipo_Opzione")
                Case enum_Opzioni_Laboratori.Bool_Mail_Automatica_Invio
                    If dtLab.Rows(i).Item("Valore") = True Then
                        Bool_Attiva = True
                    End If
                Case enum_Opzioni_Laboratori.Mail_Automatica_A
                    MailA = dtLab.Rows(i).Item("Valore")
                Case enum_Opzioni_Laboratori.Mail_Automatica_CC
                    MailCC = dtLab.Rows(i).Item("Valore")
                Case enum_Opzioni_Laboratori.Mail_Rispondi_A
                    RispondiA = dtLab.Rows(i).Item("Valore")
                Case enum_Opzioni_Laboratori.Mail_Firma
                    Firma = dtLab.Rows(i).Item("Valore")
                Case enum_Opzioni_Laboratori.Mail_Oggetto
                    Oggetto = dtLab.Rows(i).Item("Valore")
                Case enum_Opzioni_Laboratori.Mail_Lingua
                    Lingua = dtLab.Rows(i).Item("Valore")
            End Select
        Next

        If Bool_Attiva = False Then
            Exit Sub
        End If
        If RispondiA.Trim = "" Then
            Throw New GiasException("RispondiA non Configurato")
        End If
        If MailA.Trim = "" Then
            Throw New GiasException("MailA non Configurato")
        End If

        Dim objLeggiAnalisixMail As New AgronicaCorePianidiCampionamentoDAL.PDC_Analisi_R
        Dim dt1 As DataTable = objLeggiAnalisixMail.LeggiAnalisixMailZoo(ID_PDC_Testata, "", "", objParametri_Server)

        Dim dt As DataTable = dt1.Clone
        For Each row In dt1.Rows
            If listaAnalisiCreate.Contains(row("Analisi_Testata_Cod")) Then
                Dim rowInsert = dt.NewRow
                Dim minRagSoc As Integer = Math.Min(10, CStr(row("Rag_Soc")).Length)
                Dim minfabbricato_des As Integer = Math.Min(10, CStr(row("Fabbricato_Des")).Length)
                Dim minraggruppamento_des As Integer = Math.Min(10, CStr(row("Raggruppamento_Des")).Length)
                Dim rag_soc = pulisciStringaCSV(CStr(row("Rag_Soc"))).Substring(0, minRagSoc)
                Dim fabbricato_des = pulisciStringaCSV(CStr(row("Fabbricato_Des"))).Substring(0, minfabbricato_des)
                Dim raggruppamento_des = pulisciStringaCSV(CStr(row("Raggruppamento_Des"))).Substring(0, minraggruppamento_des)
                Dim bdn_codice_azienda = row("BDN_Codice_Azienda")
                rowInsert("Codice_Campione") = row("Codice_Campione")
                rowInsert("Rag_Soc") = rag_soc
                rowInsert("Fabbricato_Des") = fabbricato_des
                rowInsert("BDN_Codice_Azienda") = bdn_codice_azienda
                rowInsert("Raggruppamento_Des") = raggruppamento_des
                rowInsert("Matricola") = row("Matricola")
                rowInsert("Analisi_Testata_Cod") = row("Analisi_Testata_Cod")
                dt.Rows.Add(rowInsert)
            End If
        Next

        If dt.Rows.Count = 0 Then
            Throw New GiasException("Analisi non trovata")
        End If

        Dim dateTimeStr As String = Now.Date.Year.ToString + Now.Date.Month.ToString + Now.Date.Day.ToString + Now.TimeOfDay.ToString.Replace(":", "").Replace(".", "").Remove(6)
        Dim nomeFile As String = "richiesta_analisi_" &
                                    CStr(dt.Rows(0).Item("BDN_Codice_Azienda")) & "_" &
                                    CStr(dt.Rows(0).Item("Fabbricato_Des")).Substring(0, Math.Min(10, CStr(dt.Rows(0).Item("Fabbricato_Des")).Length)) + "_" +
                                    dateTimeStr

        nomeFile = nomeFile.Replace(".", "") & ".txt"

        Dim oggettoMail As String = If(Oggetto = "", "Richiesta Analisi:", Oggetto) & " " & nomeFile

        'Corpo mail
        Dim testoMail As String = ""
        testoMail &= "<br><br>Questa mail è stata inviata da un sistema automatico.<br>Non rispondere a questa mail."
        testoMail &= "<br>Per assistenza contattare: assistenza@agronica.it</b>"
        testoMail &= "<br><br>Cordiali Saluti</b>"

        'aggiungo la firma 
        testoMail &= "<br><br>" & Firma

        Dim _AgroWebConfig As New AgronicaCoreGestioneRichieste.AgroWebConfig

        Dim path As String = ""
        Dim pathFile = GeneraRichiestaAnalisi_txt(nomeFile, path, dt, _AgroWebConfig)

        Dim allegati As String() = Nothing
        If Not String.IsNullOrEmpty(pathFile) Then
            allegati = {pathFile}
        End If

        'Invio effettivo mail
        Dim handleMail As New Mail
        Dim mexErrore = handleMail.invia(objParametri_Server, RispondiA, MailA, MailCC, "", oggettoMail, testoMail, True, allegati)

        If mexErrore <> "" Then
            'Elimino il file se creato
            If IO.File.Exists(pathFile) Then
                IO.File.Delete(pathFile)
            End If
            Throw New Exception(mexErrore)
        End If

        If pathFile <> "" Then
            My.Computer.FileSystem.RenameFile(pathFile, "[INVIATO] " & nomeFile)
        End If

    End Sub

    Private Shared Function pulisciStringaCSV(ByVal stringa As String) As String
        stringa = stringa.Replace(";", "")
        stringa = stringa.Replace("'", "")
        stringa = stringa.Replace("""", "")
        Return stringa
    End Function

    Private Shared Function GeneraRichiestaAnalisi_txt(nomeFile As String, ByRef path As String,
                                                       dt As DataTable,
                                                       _AgroWebConfig As AgronicaCoreGestioneRichieste.AgroWebConfig
                                                       ) As String

        path = FileSystemHelper.AggiungiSlashSeNonEsiste(_AgroWebConfig.PathFileTemporanei)

        Dim pathFile As String = path & nomeFile

        'Devo eliminare dalla cartelle temporanea i file se esistono già, altrimenti non si riesce a scriverli
        If IO.File.Exists(pathFile) Then
            IO.File.Delete(pathFile)
        End If

        Dim textList As New List(Of String)
        For Each row In dt.Rows

            Dim listColumn As New List(Of String)

            listColumn.Add("1")
            listColumn.Add(row.item("Rag_Soc"))
            listColumn.Add(row.item("Fabbricato_Des"))
            listColumn.Add("4")
            listColumn.Add(row.item("Raggruppamento_Des"))
            listColumn.Add(row.item("Matricola"))
            listColumn.Add(row.item("Codice_Campione"))
            listColumn.Add("0")
            listColumn.Add(Now.Date.Year.ToString("D4") + Now.Date.Month.ToString("D2") + Now.Date.Day.ToString("D2") + Now.TimeOfDay.ToString.Replace(":", "").Replace(".", "").Remove(6))
            listColumn.Add("")
            listColumn.Add("TRUE")
            listColumn.Add("FALSE")
            listColumn.Add(dt.Rows.IndexOf(row) + 1)

            textList.Add(String.Join(";", listColumn))

        Next

        IO.File.WriteAllText(pathFile, String.Join(vbCrLf, textList).Replace(".", ""))

        Return pathFile

    End Function
#End Region
End Class

Public Class RichiestaAnalisiLab_Helper

    Public Shared Function MapFromDataTable(ByVal dt As DataTable) As List(Of RichiestaAnalisiLab)

        Const nomeRoutine = "RichiestaAnalisiLab_Helper.MapFromDataTable()"
        Dim mappingList As New List(Of RichiestaAnalisiLab)

        Try

            If Not dt Is Nothing AndAlso dt.Rows.Count > 0 Then

            'Trasformo in oggetto
                mappingList = (From p In dt.AsEnumerable()
                                Select New RichiestaAnalisiLab With {
                                    .PivaSuperUser = p.Field(Of String)("PivaSuperUser"),
                                    .ID_PDC_Testata = p.Field(Of Integer)("ID_PDC_Testata"),
                                    .ID_PDC_Dettagli = p.Field(Of Integer)("ID_PDC_Dettagli"),
                                    .ID_PDC_Campione = p.Field(Of Integer)("ID_PDC_Campione"),
                                    .Analisi_Testata_Cod = p.Field(Of Integer)("Analisi_Testata_Cod"),
                                    .Chiave = p.Field(Of String)("Chiave"),
                                    .PivaOwner = p.Field(Of String)("PivaOwner"),
                                    .Piva = p.Field(Of String)("Piva"),
                                    .Piva_OP = p.Field(Of String)("Piva_OP"),
                                    .Rag_Soc_OP = p.Field(Of String)("Rag_Soc_OP"),
                                    .Kpin = p.Field(Of String)("Kpin"),
                                    .Block = p.Field(Of String)("Block"),
                                    .BlockRR = p.Field(Of String)("Block"),
                                    .Grower_Number = p.Field(Of String)("Grower_Number"),
                                    .Area = p.Field(Of String)("Area"),
                                    .Indirizzo = p.Field(Of String)("Ind_Dettaglio_Ind_Des"),
                                    .Frazione = p.Field(Of String)("Ind_Dettaglio_Frz_Des"),
                                    .Comune = p.Field(Of String)("Ind_Dettaglio_Com_Des"),
                                    .Comune_Completo = p.Field(Of String)("Ind_Dettaglio_Comune_Completo"),
                                    .CAP = p.Field(Of String)("Ind_Dettaglio_CAP"),
                                    .Provincia = p.Field(Of String)("Ind_Dettaglio_Pro_Cod"),
                                    .Nazione = p.Field(Of String)("Ind_Dettaglio_Stato"),
                                    .Regione_Cod = p.Field(Of String)("Ind_Dettaglio_Regione_Cod"),
                                    .Regione_Des = p.Field(Of String)("Ind_Dettaglio_Regione_Des"),
                                    .Pro_Cod_Istat = p.Field(Of String)("Ind_Dettaglio_Pro_Cod_Istat"),
                                    .Com_Cod_Istat = p.Field(Of String)("Ind_Dettaglio_Com_Cod_Istat"),
                                    .GGN = p.Field(Of String)("GGN"),
                                    .CodiceFornitore = p.Field(Of String)("CodiceFornitore"),
                                    .Rag_Soc = p.Field(Of String)("Rag_Soc"),
                                    .Sa_Nome = p.Field(Of String)("Sa_Nome"),
                                    .App_Nome = p.Field(Of String)("App_nome"),
                                    .Sup_Imp = p.Field(Of Double)("Sup_Imp"),
                                    .CapitolatoPrivato = p.Field(Of String)("CapitolatoPrivato"),
                                    .Regolamento = p.Field(Of String)("Regolamento"),
                                    .Tecnico_Campionamento = p.Field(Of String)("Tecnico_Campionamento"),
                                    .Tecnico_Campionamento_Tel = p.Field(Of String)("Tecnico_Campionamento_Tel"),
                                    .Data_Campionamento = p.Field(Of DateTime)("Data_Campionamento"),
                                    .Codice_Campione = p.Field(Of String)("Codice_Campione"),
                                    .Note_Impianto = p.Field(Of String)("note_impianto"),
                                    .Tecnico_Campione = p.Field(Of String)("Tecnico_Campione"),
                                    .Note_Campione = p.Field(Of String)("Note_Campione"),
                                    .Codice_Progressivo_Inizio_Anno = p.Field(Of Integer)("Codice_Progressivo_Inizio_Anno"),
                                    .Codice_Anno = p.Field(Of Integer)("Codice_Anno"),
                                    .PuntoPrelievo = p.Field(Of String)("PuntoPrelievo"),
                                    .Num_Prodotti = p.Field(Of Integer)("Num_Prodotti"),
                                    .Motivo_Campione_Cod = p.Field(Of Integer)("Motivo_Campione_Cod"),
                                    .Motivo_Campione_Des = p.Field(Of String)("Motivo_Campione_Des"),
                                    .Cod_Risum = p.Field(Of Integer)("Cod_Risum"),
                                    .Data_Richiesta_Analisi = p.Field(Of DateTime)("Data_Richiesta_Analisi"),
                                    .Note_Richiesta_Analisi = p.Field(Of String)("Note_Richiesta_Analisi"),
                                    .Fornitore_Di_Fatturazione = p.Field(Of String)("Fornitore_Di_Fatturazione"),
                                    .Laboratorio = p.Field(Of String)("Laboratorio"),
                                    .Analisi_Tipologia_Tipo = p.Field(Of Integer)("Analisi_Tipologia_Tipo"),
                                    .Analisi_Tipologia_Des = p.Field(Of String)("Analisi_Tipologia_Des"),
                                    .Altre_Molecole = p.Field(Of String)("Altre_Molecole"),
                                    .Altre_Molecole_Cod = p.Field(Of String)("Altre_Molecole_Cod"),
                                    .Analisi_Testata_Note1 = p.Field(Of String)("Analisi_Testata_Note1"),
                                    .Veg_Des = p.Field(Of String)("Veg_Des"),
                                    .Cul_Des = p.Field(Of String)("Cul_Des"),
                                    .Tipologia_Varietale = p.Field(Of String)("tipologia_varietale"),
                                    .Codice_2 = p.Field(Of String)("codice_2"),
                                    .Tipo_Campione_Apofruit = p.Field(Of String)("Tipo_Campione_Apofruit"),
                                    .Nome_Stabilimento = p.Field(Of String)("Nome_Stabilimento"),
                                    .Veg_Cod = p.Field(Of Integer)("Veg_Cod"),
                                    .Cul_Cod = p.Field(Of Integer)("Cul_Cod"),
                                    .Grva_Cod = p.Field(Of Integer)("Grva_Cod"),
                                    .Piva_FornitoreFatturazione = p.Field(Of String)("Piva_FornitoreFatturazione"),
                                    .CodiceStabilimento = p.Field(Of String)("CodiceStabilimento"),
                                    .Tipo_Campione = p.Field(Of Integer)("Tipo_Campione"),
                                    .Id_Lfo = p.Field(Of Integer)("Id_Lfo"),
                                    .Rag_Soc_Padre = p.Field(Of String)("rag_soc_padre")
                                }).ToList()
            End If

        Catch ex As Exception
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
        End Try

        Return mappingList

    End Function

    Public Function MapToRestApi(ByVal objAnalisi As RichiestaAnalisiLab,
                                 ByVal tipoAPI As String,
                                 ByRef objParametri As AgronicaCoreParametri
                                 ) As Analisi_API_Richiesta

        Dim objApiRichiesta As New Analisi_API_Richiesta

        Dim mapper As Dictionary(Of String, String) = RichiestaAnalisiLab.Mapper(objAnalisi, tipoAPI)

        Dim settingLoc As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}

        Dim listaCoppie As New List(Of Analisi_API_ChiaveValore)

        For Each nomePropOriginale In mapper.Keys

            Dim prop As Reflection.PropertyInfo = objAnalisi.GetType.GetProperty(nomePropOriginale)
            Dim nomeProp As String = prop.Name
            Dim valProp = prop.GetValue(objAnalisi, Nothing)

            Dim typeProp As Type = prop.PropertyType

            Dim coppiaChiaveValore As New Analisi_API_ChiaveValore With {
                .chiave = mapper(nomePropOriginale)
            }

            'Visto che le date devono apparire in un certo formato quando farò il Json, sono costretta a convertirla direttamente qui,
            'senno poi mi perdo l'informazione che si trattava di una data, perché di qui in avanti è tutto stringa
            Select Case typeProp
                Case Type.GetType("System.Date"), Type.GetType("System.DateTime")
                    'Devo anche fare il replace: già qui mi trovo 3 quotes, anziché 1 sola e poi verrebbero nuovamente moltiplicate in fase di Json Serialize
                    coppiaChiaveValore.valore = JsonConvert.SerializeObject(Convert.ToDateTime(valProp)).Replace("""", "")
                    'coppiaChiaveValore.valore = JsonConvert.SerializeObject(CDate(valProp), settingLoc)
                Case Else
                    coppiaChiaveValore.valore = CStr(valProp)
            End Select

            listaCoppie.Add(coppiaChiaveValore)

        Next

        objApiRichiesta.richiesta = listaCoppie

        'Eseguo le sostituzioni di valori per i campi che necessitano di mappature
        RichiestaAnalisiLab.SostituzioniMappature(objAnalisi, objApiRichiesta, tipoAPI, objParametri)

        Return objApiRichiesta

    End Function

End Class
