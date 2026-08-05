Imports System
Imports System.IO
Imports System.Xml.Serialization
Imports AgronicaCoreDataProvider.UtilityDatabaseExtension
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate

Public Class Importazione_Fascicolo
    Private Const _UpperBoundTabelle As Integer = 2000000000

    '##############################################################################################
    Public Function Converti_TitoliPossesso_Fascicolo(ByVal TitoloPossesso As String, _
                                                       ByRef TitoloPossessoDes As String) As Integer

        Dim Particella_Possesso As Integer

        Select Case TitoloPossesso

            Case "1"
                Particella_Possesso = 1 'ProprietÃ 
                TitoloPossessoDes = "Proprieta"
            Case "2"
                Particella_Possesso = 3 'Affitto con contratto
                TitoloPossessoDes = "Affitto"
            Case Else
                Particella_Possesso = 0 'altro
                TitoloPossessoDes = "Altro"
        End Select

        Return Particella_Possesso

    End Function

    Public Sub SerializeObject(ByVal filename As String, ByVal ws_fasciResponse As SincroAnagrafeBA1.NsFascicolo.getFascicoloResponse)

        Console.WriteLine("Writing With Stream")

        Dim serializer As New XmlSerializer(GetType(SincroAnagrafeBA1.NsFascicolo.getFascicoloResponse))

        ' Create a FileStream to write with.
        Dim writer As New FileStream(filename, FileMode.Create)
        ' Serialize the object, and close the TextWriter
        serializer.Serialize(writer, ws_fasciResponse)
        writer.Close()

    End Sub

    Public Sub Importa_Condizionalita_Profilo(ByVal TopCode As Integer, ByVal BaseCode As Integer, _
                                               ByVal Regolamento_Condizionalita As Integer, _
                                               ByVal Piva As String, ByVal Dt As DataTable, _
                                               ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri)

        Dim MsgERR As String = ""

        Dim Dr() As DataRow
        Dim Codice As Integer
        Dim ValiditaInizio As DateTime = Now.Today
        Dim ValiditaFine As DateTime = #12/31/2100#

        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False

        Dim objDataProvider As New AgronicaCoreDataProvider.DataProvider


        Try

            If Not Dt Is Nothing AndAlso Dt.Rows.Count > 0 Then

                'Se la connessione Ã¨ chiusa la apro e se la transazione Ã¨ chiusa la inizio
                AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale, _
                                                                                        FlagTransazioneLocale, _
                                                                                        objParametri_Server)


                '*************************************************************************************************************
                '***  RECORD DOMANDE INTERVISTA         *********************************************************************
                '*************************************************************************************************************
                Dim Dt_Domande_Interviste As DataTable
                Dim strValore As String

                Dim objAudit_Domande_Interviste As New AgronicaCoreAuditDAL.Audit_Domande_Interviste_R

                ' leggo le domande dell'intervista
                Dt_Domande_Interviste = objAudit_Domande_Interviste.Leggi(enum_AuditPuaTipo.Audit_Condizionalita, _
                                                                            Regolamento_Condizionalita, _
                                                                            0, _
                                                                            "", "", _
                                                                            objParametri_Server)

                If Dt_Domande_Interviste.Rows.Count > 0 Then

                    '*************************************************************************************************************
                    '***  RECORD INTERVISTA         ******************************************************************************
                    '*************************************************************************************************************

                    Dim ObjSequenze As New AgronicaCoreDataProvider.Agro_Sequenze

                    Codice = ObjSequenze.NuovoId_Tabella( _
                                           "AUDIT_INTERVISTE", _
                                            BaseCode, _
                                            TopCode, _
                                            objParametri_Server)

                    Dim objAudit_Interviste As New AgronicaCoreAuditDAL.Audit_Interviste_W

                    objAudit_Interviste.Scrivi(enum_AuditPuaTipo.Audit_Condizionalita, _
                                            Regolamento_Condizionalita, _
                                            Codice, _
                                            Piva, _
                                            ValiditaInizio, _
                                            ValiditaFine, _
                                            objParametri_Server)

                    If MsgERR = "" Then

                        For i = 0 To Dt_Domande_Interviste.Rows.Count - 1

                            strValore = ""

                            Select Case Dt_Domande_Interviste.Rows(i).Item("Tipo")

                                Case "b"

                                    Dr = Dt.Select("Domanda_Cod=" & CStr(Dt_Domande_Interviste.Rows(i).Item("Domanda_Cod")))

                                    If Not Dr Is Nothing AndAlso Dr.Length > 0 Then
                                        strValore = "1"
                                    Else
                                        strValore = "0"
                                    End If

                            End Select

                            '*************************************************************************************************************
                            '***  RECORD RISPOSTE INTERVISTA         *********************************************************************
                            '*************************************************************************************************************

                            If strValore <> "" Then

                                Dim objAuditRisposteInterviste As New AgronicaCoreAuditDAL.Audit_Risposte_Interviste_W

                                ' INSERIMENTO
                                objAuditRisposteInterviste.Scrivi(enum_AuditPuaTipo.Audit_Condizionalita, _
                                                                    Regolamento_Condizionalita, _
                                                                    Codice, _
                                                                    CLng(Dt_Domande_Interviste.Rows(i).Item("Domanda_Cod")), _
                                                                    strValore, _
                                                                    ValiditaInizio, _
                                                                    ValiditaFine, _
                                                                    objParametri_Server)

                            End If

                        Next

                    End If

                End If


            End If

            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri_Server)


        Catch ex As Exception

            'Faccio il rollback della transazione
            If Not objParametri_Server.objTransazione Is Nothing Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)
            End If

            MsgERR = "Si Ã¨ verificato un errore durante il salvataggio del profilo di condizionalitÃ . " & vbCrLf & "Ritentare."

        Finally

            'Chiudo la connessione se Ã¨ stata aperta in questa routine
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri_Server)

            ' distruggo gli oggetti
            objDataProvider = Nothing

        End Try


    End Sub

    Public Sub Crea_Stringa_Catasto(ByRef stringaAnagrafica As String,
                                   ByRef stringaPianificazione As String,
                                   ByVal strXmlDoc As String,
                                   ByVal strXmlDocP As String,
                                   ByVal Piva As String,
                                   ByVal SaCod_u As Integer,
                                   ByVal Validita_Inizio As Date,
                                   ByVal Validita_Fine As Date,
                                   ByVal Anno_Piano_Colturale As Integer,
                                   ByVal DtParticelle As DataTable,
                                   ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                   ByVal Flag_CentroUnico As Boolean,
                                   ProgressivoGIAS As String,
                                    Optional ByVal CreaPlanning As Boolean = True
                                   )

        Dim i As Integer

        Dim Prov As String
        Dim Com As String
        Dim Sezione As String
        Dim Foglio As Integer
        Dim Numero As Integer
        Dim Subalterno As String
        Dim supConduzione As Double
        Dim SupCatastale As Double
        Dim Ettari, Are, Centiare As Integer
        Dim Macrouso_Cod As String
        Dim Macrouso_Sup As Double
        Dim Utilizzo_Sup As Double
        Dim Veg_Cod_Agea, Cul_Cod_Agea As String
        Dim Veg_Cod As String
        Dim Cul_Cod, Grfi_Cod, Grva_Cod, Id_Cod As Integer
        Dim Uso_Cod_Agea As String
        Dim Occupazione_Cod_Agea As String
        Dim Destinazione_Cod_Agea As String
        Dim Qualita_Cod_Agea As String

        Dim TitoloPossesso As Integer
        Dim Inizio_Possesso, Inizio_Possesso_Old As Date
        Dim Fine_Possesso, Fine_Possesso_Old As Date
        Dim Inizio_Macrouso, Inizio_Macrouso_Old As Date
        Dim Fine_Macrouso, Fine_Macrouso_Old As Date
        Dim Inizio_Utilizzo, Inizio_Utilizzo_Old As Date
        Dim Fine_Utilizzo, Fine_Utilizzo_Old As Date

        Dim TipoOperazione_Particella, TipoOperazione_Possesso, TipoOpMacrousoxParticella, TipoOpUtilizzoxParticella As enum_TipoOperazioneDB

        Dim XmlParticella As System.Xml.XmlElement
        Dim XmlPossesso As System.Xml.XmlElement
        Dim XmlZona As System.Xml.XmlElement
        Dim XmlMacrouso As System.Xml.XmlElement
        Dim XmlUtilizzo As System.Xml.XmlElement
        Dim XmlCentro As System.Xml.XmlElement
        Dim XMLCentri As System.Xml.XmlNodeList
        Dim XmlEntita As System.Xml.XmlElement
        Dim XmlParticellaP As System.Xml.XmlElement
        Dim XmlTestata As System.Xml.XmlElement
        Dim XMLTestate As System.Xml.XmlNodeList
        Dim XmlZone As System.Xml.XmlElement

        Dim nEntita As Integer = 1


        Dim _DtParticelleCentro As New DataTable
        Dim _DtParticelleMacrousi As New DataTable
        Dim _DtParticelleUtilizzi As New DataTable
        Dim _DtParticelleZone As New DataTable
        Dim _DtParticelleZonaBSL As New DataTable
        Dim _DrParticella() As DataRow
        Dim _DrParticellaM() As DataRow
        Dim _DrParticellaU() As DataRow
        Dim _DrParticellaZ() As DataRow

        Dim _DtParticelleZoneA As New DataTable
        Dim _DtParticelleZoneB As New DataTable
        Dim _DrParticellaAB() As DataRow

        Dim XmlAnagrafica As New System.Xml.XmlDocument
        Dim XmlPianificazione As New System.Xml.XmlDocument

        XmlAnagrafica.LoadXml(strXmlDoc)

        If CreaPlanning = True Then
            XmlPianificazione.LoadXml(strXmlDocP)
        End If

        'Dim strValiditaInizio As String = String.Empty
        'Dim strValiditaFine As String = String.Empty
        'strValiditaInizio = "01/11/" & (Anno - 1).ToString
        'strValiditaFine = "31/10/" & Anno.ToString

        Dim objXML As New AgronicaCoreXML.AnagrafeXML
        Dim objImpresexParticelle As New AgronicaCoreAnagrafeDAL.ImpresexParticelle2_R
        Dim objParticellexMacrousi As New AgronicaCoreAnagrafeDAL.ParticelleCatastalixMacrousi_R
        Dim objParticellexMacrousixUtilizzo As New AgronicaCoreAnagrafeDAL.ParticelleCatastalixMacrousixUtilizzo_R
        Dim objParticellexZone As New AgronicaCoreAnagrafeDAL.ZonexParticelle_R

        Dim saCods As New List(Of String)

        If Flag_CentroUnico Then
            Dim objCentri As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read
            Dim dtCentri = objCentri.Leggi(Piva, 0, AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)
            If dtCentri IsNot Nothing AndAlso dtCentri.Rows.Count > 0 Then
                SaCod_u = dtCentri.Rows(0).Item("sa_cod")
            End If
            If SaCod_u <> 0 Then

                _DtParticelleCentro = objImpresexParticelle.Leggi(0,
                                                               Piva,
                                                               SaCod_u,
                                                               0,
                                                               "", "", "", 0, 0, "",
                                                               AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                                               "", "",
                                                               objParametri_Server)

                _DtParticelleMacrousi = objParticellexMacrousi.Leggi_DaCentro(Piva,
                                                               SaCod_u,
                                                               "", "", "", 0, 0, "",
                                                               "",
                                                               "", "",
                                                               objParametri_Server)

                _DtParticelleUtilizzi = objParticellexMacrousixUtilizzo.Leggi_DaCentro(Piva,
                                                   SaCod_u,
                                                   "", "", "", 0, 0, "",
                                                   "", "", "",
                                                   "", "",
                                                   objParametri_Server)

                _DtParticelleZone = objParticellexZone.Leggi(-17,
                                        "", "", "", 0, 0, "",
                                        AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                       "", "",
                                       objParametri_Server)

                _DtParticelleZonaBSL = objParticellexZone.Leggi(1,
                                      "", "", "", 0, 0, "",
                                      AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                     "", "",
                                     objParametri_Server)

                _DtParticelleZoneA = objParticellexZone.Leggi(-46,
                                        "", "", "", 0, 0, "",
                                        AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                       "", "",
                                       objParametri_Server)

                _DtParticelleZoneB = objParticellexZone.Leggi(-47,
                                        "", "", "", 0, 0, "",
                                        AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                       "", "",
                                       objParametri_Server)

            End If
        Else
            Dim objCentri As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read
            Dim dtCentri = objCentri.Leggi(Piva, 0, AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)
            If dtCentri IsNot Nothing AndAlso dtCentri.Rows.Count > 0 Then
                For Each row In dtCentri.Rows
                    saCods.Add(CStr(row.item("sa_cod")))
                Next
                For Each SaCod In saCods
                    _DtParticelleCentro.Merge(objImpresexParticelle.Leggi(0,
                                                                   Piva,
                                                                   SaCod,
                                                                   0,
                                                                   "", "", "", 0, 0, "",
                                                                   AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                                                   "", "",
                                                                   objParametri_Server))

                    _DtParticelleMacrousi.Merge(objParticellexMacrousi.Leggi_DaCentro(Piva,
                                                                   SaCod,
                                                                   "", "", "", 0, 0, "",
                                                                   "",
                                                                   "", "",
                                                                   objParametri_Server))

                    _DtParticelleUtilizzi.Merge(objParticellexMacrousixUtilizzo.Leggi_DaCentro(Piva,
                                                       SaCod,
                                                       "", "", "", 0, 0, "",
                                                       "", "", "",
                                                       "", "",
                                                       objParametri_Server))
                Next

                _DtParticelleZone = objParticellexZone.Leggi(-17,
                                            "", "", "", 0, 0, "",
                                            AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                           "", "",
                                           objParametri_Server)

                _DtParticelleZonaBSL = objParticellexZone.Leggi(1,
                                      "", "", "", 0, 0, "",
                                      AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                     "", "",
                                     objParametri_Server)

                _DtParticelleZoneA = objParticellexZone.Leggi(-46,
                                        "", "", "", 0, 0, "",
                                        AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                       "", "",
                                       objParametri_Server)

                _DtParticelleZoneB = objParticellexZone.Leggi(-47,
                                        "", "", "", 0, 0, "",
                                        AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                       "", "",
                                       objParametri_Server)

            End If
        End If


        Dim HashParticelle As New Hashtable
        Dim HashMacrousi As New Hashtable

        'Leggo le particelle vulnerabili
        Dim objPV As New AgronicaCoreMetaSchemaDAL.ParticelleCatastali_Vulnerabili_R
        Dim _DtPV As DataTable
        _DtPV = objPV.Leggi("", "", "", 0, 0, "", 0, "", "", objParametri_Server)

        Dim objZoneAB As New AgronicaCoreMetaSchemaDAL.ParticelleCatastali_Acclivita_R
        Dim _DtZoneAB As DataTable
        _DtZoneAB = objZoneAB.Leggi("", "", "", 0, 0, "", "", "", objParametri_Server)


        Dim DtParticelleCentroEO = _DtParticelleCentro.ToExpandoObject()
        Dim DtParticelleMacrousiEO = _DtParticelleMacrousi.ToExpandoObject()
        Dim DtParticelleUtilizziEO = _DtParticelleUtilizzi.ToExpandoObject()
        Dim DtParticelleZoneEO = _DtParticelleZone.ToExpandoObject()
        Dim DtParticelleZonaBSLEO = _DtParticelleZonaBSL.ToExpandoObject()

        Dim DtParticelleZoneAEO = _DtParticelleZoneA.ToExpandoObject()
        Dim DtParticelleZoneBEO = _DtParticelleZoneB.ToExpandoObject()
        Dim DtPVEO = _DtPV.ToExpandoObject()
        Dim DtZoneABEO = _DtZoneAB.ToExpandoObject()

        For i = 0 To DtParticelle.Rows.Count - 1

            TipoOperazione_Particella = enum_TipoOperazioneDB.Scrittura
            TipoOperazione_Possesso = enum_TipoOperazioneDB.Scrittura

            Dim ModificaParticella As Boolean = False
            Dim ModificaPossesso As Boolean = False
            Dim ModificaMacrouso As Boolean = False
            Dim ModificaUtilizzo As Boolean = False
            Dim InserisciZona As Boolean = False
            Dim InserisciZona_AB As Boolean = False
            Dim EliminaZona_AB As Boolean = False

            Prov = DtParticelle.Rows(i).Item("PROV")
            Com = DtParticelle.Rows(i).Item("COM")
            Sezione = DtParticelle.Rows(i).Item("Sezione")
            Foglio = DtParticelle.Rows(i).Item("Foglio")
            Numero = DtParticelle.Rows(i).Item("Numero")
            Subalterno = DtParticelle.Rows(i).Item("Subalterno")

            If Sezione = "" Then
                Sezione = "0"
            End If
            If Subalterno = "" Then
                Subalterno = "0"
            End If

            SupCatastale = DtParticelle.Rows(i).Item("sup")
            objXML.EttariAreCentiare_from_Ettari(SupCatastale, Ettari, Are, Centiare)

            supConduzione = DtParticelle.Rows(i).Item("supcondotta")

            TitoloPossesso = DtParticelle.Rows(i).Item("TitoloPossesso")
            Inizio_Possesso = DtParticelle.Rows(i).Item("Inizio_Possesso")
            Fine_Possesso = DtParticelle.Rows(i).Item("Fine_Possesso")

            Macrouso_Cod = DtParticelle.Rows(i).Item("Macrouso_Cod")
            Macrouso_Sup = DtParticelle.Rows(i).Item("Macrouso_Sup")
            Inizio_Macrouso = AGRODATAINIZIO
            Fine_Macrouso = AGRODATAFINE
            Inizio_Utilizzo = AGRODATAINIZIO
            Fine_Utilizzo = AGRODATAFINE
            Inizio_Macrouso = Validita_Inizio
            Fine_Macrouso = Validita_Fine
            Inizio_Utilizzo = Validita_Inizio.ToShortDateString
            Fine_Utilizzo = Validita_Fine

            Veg_Cod_Agea = DtParticelle.Rows(i).Item("Veg_Cod_Agea")
            Cul_Cod_Agea = DtParticelle.Rows(i).Item("Cul_Cod_Agea")
            Veg_Cod = DtParticelle.Rows(i).Item("Veg_Cod")
            Cul_Cod = DtParticelle.Rows(i).Item("Cul_Cod")
            Grfi_Cod = DtParticelle.Rows(i).Item("Grfi_Cod")
            Grva_Cod = DtParticelle.Rows(i).Item("Grva_Cod")
            Id_Cod = DtParticelle.Rows(i).Item("Id_Cod")
            Utilizzo_Sup = DtParticelle.Rows(i).Item("Utilizzo_Sup")

            Uso_Cod_Agea = DtParticelle.Rows(i).Item("Uso_Cod_Agea")
            Occupazione_Cod_Agea = DtParticelle.Rows(i).Item("Occupazione_Cod_Agea")
            Destinazione_Cod_Agea = DtParticelle.Rows(i).Item("Destinazione_Cod_Agea")
            Qualita_Cod_Agea = DtParticelle.Rows(i).Item("Qualita_Cod_Agea")

            Dim scrivi_particella As Boolean = False
            Dim scrivi_possesso As Boolean = True
            ModificaPossesso = True
            'se esiste giÃ  il centro verifico se contiene la particella
            If SaCod_u <> 0 Or saCods.Count <> 0 Then
                ModificaPossesso = True

                Dim dtParticelleCentroQuery = DtParticelleCentroEO.Where(Function(c) c("PROV").Equals(Prov) AndAlso
                                                     c("COM").Equals(Com) AndAlso
                                                     c("FOGLIO").Equals(CInt(Foglio)) AndAlso
                                                     c("NUMERO").Equals(CInt(Numero)))

                If Sezione = "" Then
                    dtParticelleCentroQuery = dtParticelleCentroQuery.Where(Function(c) c("SEZIONE").Equals("") Or c("SEZIONE").Equals("0"))
                Else
                    dtParticelleCentroQuery = dtParticelleCentroQuery.Where(Function(c) CStr(c("SEZIONE")).ToLower.Equals(Sezione.ToLower))
                End If

                If Subalterno = "" Then
                    dtParticelleCentroQuery = dtParticelleCentroQuery.Where(Function(c) c("SUBALTERNO").Equals("") Or c("SUBALTERNO").Equals("0"))
                Else
                    dtParticelleCentroQuery = dtParticelleCentroQuery.Where(Function(c) CStr(c("SUBALTERNO")).ToLower.Equals(Subalterno.ToLower))
                End If

                Dim DrParticella = dtParticelleCentroQuery.ToList

                'se la particella in quell'intervallo esiste giÃ  la modifico
                'altrimenti la inserisco
                If Not DrParticella Is Nothing AndAlso DrParticella.Count > 0 Then
                    ModificaParticella = True
                    For j = 0 To DrParticella.Count - 1
                        Inizio_Possesso_Old = DrParticella(j)("Validita_Inizio")
                        Fine_Possesso_Old = DrParticella(j)("Validita_Fine")

                        If (Inizio_Possesso > Inizio_Possesso_Old And Inizio_Possesso < Fine_Possesso_Old) Or (Fine_Possesso < Fine_Possesso_Old And Fine_Possesso > Inizio_Possesso_Old) Then
                            ModificaPossesso = False
                        ElseIf Inizio_Possesso = Inizio_Possesso_Old And Fine_Possesso = Fine_Possesso_Old Then
                            scrivi_possesso = False
                            Exit For
                        Else
                            scrivi_possesso = True
                        End If
                    Next
                Else
                    scrivi_particella = True
                    scrivi_possesso = True
                End If

                'If ModificaPossesso = True Then
                '    TipoOperazione_Particella = enum_TipoOperazioneDB.Modifica
                'Else
                '    TipoOperazione_Particella = enum_TipoOperazioneDB.Lettura
                'End If

                If ModificaParticella = True Then
                    TipoOperazione_Particella = enum_TipoOperazioneDB.Modifica
                End If
                If ModificaPossesso = True Then
                    TipoOperazione_Possesso = enum_TipoOperazioneDB.Modifica
                End If

                If scrivi_particella = True Then
                    TipoOperazione_Particella = enum_TipoOperazioneDB.Scrittura
                End If
                If scrivi_possesso = True Then
                    TipoOperazione_Possesso = enum_TipoOperazioneDB.Scrittura
                End If
            End If

            '-----------------------------------------------------------------------
            'verifico se la particella Ã¨ vulnerabile
            'se Ã¨ vulnerabile salvo il dato sia nella particella sia nell'entitÃ 
            Dim strZona As String = "n"
            Dim DataInizioZona As Date = AGRODATAINIZIO
            Dim DataFineZona As Date = AGRODATAFINE

            If Not DtPVEO Is Nothing AndAlso DtPVEO.Count > 0 Then

                Dim DtPVEO_Query = DtPVEO.Where(Function(c) c("PROV").Equals(Prov) AndAlso
                                                     c("COM").Equals(Com) AndAlso
                                                     c("FOGLIO").Equals(CInt(Foglio)))

                Dim DrPV = DtPVEO_Query.ToList()
                'DrPV = DtPV.Select("PROV='" & Prov & "' AND COM='" & Com & "' AND Sezione='" & Sezione & "' AND foglio=" & Foglio.ToString & " AND Numero=" & Numero.ToString & " AND subalterno='" & Subalterno & "'")
                If Not DrPV Is Nothing AndAlso DrPV.Count > 0 Then
                    strZona = "v"
                    DataInizioZona = DrPV(0)("Validita_Inizio")
                    DataFineZona = DrPV(0)("Validita_Fine")
                ElseIf Not IsDBNull(DtParticelle.Rows(i)("ZoneCatasto")) AndAlso CStr(DtParticelle.Rows(i)("ZoneCatasto")).Contains("-17") Then
                    strZona = "v"
                End If
            End If

            Dim strZona_AB As String = ""

            If Not DtZoneABEO Is Nothing AndAlso DtZoneABEO.Count > 0 Then
                Dim DtZoneABEO_Query = DtZoneABEO.Where(Function(c) c("PROV").Equals(Prov) AndAlso
                                                     c("COM").Equals(Com))
                Dim DrAB = DtZoneABEO_Query.ToList
                'DrPV = DtPV.Select("PROV='" & Prov & "' AND COM='" & Com & "' AND Sezione='" & Sezione & "' AND foglio=" & Foglio.ToString & " AND Numero=" & Numero.ToString & " AND subalterno='" & Subalterno & "'")
                If Not DrAB Is Nothing AndAlso DrAB.Count > 0 Then
                    strZona_AB = DrAB(0)("FG_Zona_Acclivita")
                End If
            End If

            '--------------------------------------------------------
            If Not HashParticelle.ContainsKey(Prov & "_" & Com & "_" & Sezione & "_" & Foglio & "_" & Numero & "_" & Subalterno) Then

                HashParticelle.Add(Prov & "_" & Com & "_" & Sezione & "_" & Foglio & "_" & Numero & "_" & Subalterno, "")

                XmlParticella = objXML.Xml_Pubblico_Particella(TipoOperazione_Particella,
                                                          "0",
                                                          Com,
                                                          Prov,
                                                          Sezione,
                                                          Foglio,
                                                          Numero,
                                                          Subalterno,
                                                          "#",
                                                          Ettari,
                                                          Are,
                                                          Centiare,
                                                          TitoloPossesso,
                                                          "#", "#", "#", "#",
                                                          Format(supConduzione, "0.0000"),
                                                          Inizio_Possesso,
                                                          Fine_Possesso,
                                                          XmlAnagrafica)
                If ModificaPossesso = True Then
                    XmlPossesso = objXML.Xml_Pubblico_Particella_Possesso(TipoOperazione_Possesso,
                                                                             "#",
                                                                             TitoloPossesso,
                                                                              Format(supConduzione, "0.0000"),
                                                                              Inizio_Possesso,
                                                                              Fine_Possesso,
                                                                              XmlAnagrafica)

                    XmlParticella.AppendChild(XmlPossesso)
                End If
                'DRUDI per collis non importare i possessi
                'XmlPossesso = objXML.Xml_Pubblico_Particella_Possesso(TipoOperazione_Possesso, _
                '                                                      "#", _
                '                                                      TitoloPossesso, _
                '                                                       Format(supConduzione, "0.0000"), _
                '                                                       Inizio_Possesso, _
                '                                                       Fine_Possesso, _
                '                                                       XmlAnagrafica)
                'XmlParticella.AppendChild(XmlPossesso)

            End If

            'se la particella Ã¨ vulnerabile...........
            'verifico se Ã¨ giÃ  legata alla 'zona vulnerabile ai nitrati'
            If strZona = "v" Then

                InserisciZona = True

                If Not DtParticelleZoneEO Is Nothing AndAlso DtParticelleZoneEO.Count > 0 Then
                    Dim DtParticelleZone_Query = DtParticelleZoneEO.Where(Function(c) c("PROV").Equals(Prov) AndAlso
                                                     c("COM").Equals(Com) AndAlso
                                                     c("FOGLIO").Equals(CInt(Foglio)) AndAlso
                                                     c("NUMERO").Equals(CInt(Numero)))

                    If Sezione = "" Then
                        DtParticelleZone_Query = DtParticelleZone_Query.Where(Function(c) c("SEZIONE").Equals("") Or c("SEZIONE").Equals("0"))
                    Else
                        DtParticelleZone_Query = DtParticelleZone_Query.Where(Function(c) CStr(c("SEZIONE")).ToLower.Equals(Sezione.ToLower))
                    End If

                    If Subalterno = "" Then
                        DtParticelleZone_Query = DtParticelleZone_Query.Where(Function(c) c("SUBALTERNO").Equals("") Or c("SUBALTERNO").Equals("0"))
                    Else
                        DtParticelleZone_Query = DtParticelleZone_Query.Where(Function(c) CStr(c("SUBALTERNO")).ToLower.Equals(Subalterno.ToLower))
                    End If

                    Dim drFilteredZone = DtParticelleZone_Query.ToList

                    'DrParticellaZ = DtParticelleZone.Select("PROV='" & Prov & "' AND COM='" & Com & "' AND Sezione='" & Sezione & "' AND foglio=" & Foglio.ToString & " AND Numero=" & Numero.ToString & " AND subalterno='" & Subalterno & "'")
                    'se la zona vulnerabile esiste giÃ  non faccio nulla
                    'altrimenti la inserisco
                    If Not drFilteredZone Is Nothing AndAlso drFilteredZone.Count > 0 Then
                        InserisciZona = False
                    End If
                End If

                If InserisciZona = True Then
                    XmlZona = objXML.Xml_Pubblico_Zona(enum_TipoOperazioneDB.Scrittura,
                               "-17",
                               "0",
                               XmlAnagrafica,
                               DataInizioZona,
                               DataFineZona)

                    XmlParticella.AppendChild(XmlZona)
                End If

            End If

            If strZona_AB <> "" Then

                InserisciZona_AB = False
                EliminaZona_AB = False

                If Not DtParticelleZoneEO Is Nothing AndAlso DtParticelleZoneEO.Count > 0 Then
                    If strZona_AB = "A" Then

                        Dim DtParticelleZoneAQuery = DtParticelleZoneAEO.Where(Function(c) c("PROV").Equals(Prov) AndAlso
                                                     c("COM").Equals(Com) AndAlso
                                                     c("FOGLIO").Equals(CInt(Foglio)) AndAlso
                                                     c("NUMERO").Equals(CInt(Numero)))

                        If Sezione = "" Then
                            DtParticelleZoneAQuery = DtParticelleZoneAQuery.Where(Function(c) c("SEZIONE").Equals("") Or c("SEZIONE").Equals("0"))
                        Else
                            DtParticelleZoneAQuery = DtParticelleZoneAQuery.Where(Function(c) CStr(c("SEZIONE")).ToLower.Equals(Sezione.ToLower))
                        End If

                        If Subalterno = "" Then
                            DtParticelleZoneAQuery = DtParticelleZoneAQuery.Where(Function(c) c("SUBALTERNO").Equals("") Or c("SUBALTERNO").Equals("0"))
                        Else
                            DtParticelleZoneAQuery = DtParticelleZoneAQuery.Where(Function(c) CStr(c("SUBALTERNO")).ToLower.Equals(Subalterno.ToLower))
                        End If

                        Dim DrParticellaAB = DtParticelleZoneAQuery.ToList

                        'DrParticellaAB = DtParticelleZoneA.Select("PROV='" & Prov & "' AND COM='" & Com & "' AND Sezione='" & Sezione & "' AND foglio=" & Foglio.ToString & " AND Numero=" & Numero.ToString & " AND subalterno='" & Subalterno & "'")
                        If Not DrParticellaAB Is Nothing AndAlso DrParticellaAB.Count > 0 Then
                            InserisciZona_AB = False
                        Else
                            InserisciZona_AB = True
                        End If

                        Dim DtParticelleZoneBQuery = DtParticelleZoneBEO.Where(Function(c) c("PROV").Equals(Prov) AndAlso
                                                     c("COM").Equals(Com) AndAlso
                                                     c("FOGLIO").Equals(CInt(Foglio)) AndAlso
                                                     c("NUMERO").Equals(CInt(Numero)))

                        If Sezione = "" Then
                            DtParticelleZoneBQuery = DtParticelleZoneBQuery.Where(Function(c) c("SEZIONE").Equals("") Or c("SEZIONE").Equals("0"))
                        Else
                            DtParticelleZoneBQuery = DtParticelleZoneBQuery.Where(Function(c) CStr(c("SEZIONE")).ToLower.Equals(Sezione.ToLower))
                        End If

                        If Subalterno = "" Then
                            DtParticelleZoneBQuery = DtParticelleZoneBQuery.Where(Function(c) c("SUBALTERNO").Equals("") Or c("SUBALTERNO").Equals("0"))
                        Else
                            DtParticelleZoneBQuery = DtParticelleZoneBQuery.Where(Function(c) CStr(c("SUBALTERNO")).ToLower.Equals(Subalterno.ToLower))
                        End If

                        DrParticellaAB = DtParticelleZoneBQuery.ToList

                        'DrParticellaAB = DtParticelleZoneB.Select("PROV='" & Prov & "' AND COM='" & Com & "' AND Sezione='" & Sezione & "' AND foglio=" & Foglio.ToString & " AND Numero=" & Numero.ToString & " AND subalterno='" & Subalterno & "'")
                        If Not DrParticellaAB Is Nothing AndAlso DrParticellaAB.Count > 0 Then
                            EliminaZona_AB = True
                        Else
                            EliminaZona_AB = False
                        End If
                    ElseIf strZona_AB = "B" Then

                        Dim DtParticelleZoneB_Query = DtParticelleZoneBEO.Where(Function(c) c("PROV").Equals(Prov) AndAlso
                                                     c("COM").Equals(Com) AndAlso
                                                     c("FOGLIO").Equals(CInt(Foglio)) AndAlso
                                                     c("NUMERO").Equals(CInt(Numero)))

                        If Sezione = "" Then
                            DtParticelleZoneB_Query = DtParticelleZoneB_Query.Where(Function(c) c("SEZIONE").Equals("") Or c("SEZIONE").Equals("0"))
                        Else
                            DtParticelleZoneB_Query = DtParticelleZoneB_Query.Where(Function(c) CStr(c("SEZIONE")).ToLower.Equals(Sezione.ToLower))
                        End If

                        If Subalterno = "" Then
                            DtParticelleZoneB_Query = DtParticelleZoneB_Query.Where(Function(c) c("SUBALTERNO").Equals("") Or c("SUBALTERNO").Equals("0"))
                        Else
                            DtParticelleZoneB_Query = DtParticelleZoneB_Query.Where(Function(c) CStr(c("SUBALTERNO")).ToLower.Equals(Subalterno.ToLower))
                        End If

                        Dim DrParticellaAB = DtParticelleZoneB_Query.ToList

                        'DrParticellaAB = DtParticelleZoneB.Select("PROV='" & Prov & "' AND COM='" & Com & "' AND Sezione='" & Sezione & "' AND foglio=" & Foglio.ToString & " AND Numero=" & Numero.ToString & " AND subalterno='" & Subalterno & "'")
                        If Not DrParticellaAB Is Nothing AndAlso DrParticellaAB.Count > 0 Then
                            InserisciZona_AB = False
                        Else
                            InserisciZona_AB = True
                        End If


                        Dim DtParticelleZoneA_Query = DtParticelleZoneAEO.Where(Function(c) c("PROV").Equals(Prov) AndAlso
                                                     c("COM").Equals(Com) AndAlso
                                                     c("FOGLIO").Equals(CInt(Foglio)) AndAlso
                                                     c("NUMERO").Equals(CInt(Numero)))

                        If Sezione = "" Then
                            DtParticelleZoneA_Query = DtParticelleZoneA_Query.Where(Function(c) c("SEZIONE").Equals("") Or c("SEZIONE").Equals("0"))
                        Else
                            DtParticelleZoneA_Query = DtParticelleZoneA_Query.Where(Function(c) CStr(c("SEZIONE")).ToLower.Equals(Sezione.ToLower))
                        End If

                        If Subalterno = "" Then
                            DtParticelleZoneA_Query = DtParticelleZoneA_Query.Where(Function(c) c("SUBALTERNO").Equals("") Or c("SUBALTERNO").Equals("0"))
                        Else
                            DtParticelleZoneA_Query = DtParticelleZoneA_Query.Where(Function(c) CStr(c("SUBALTERNO")).ToLower.Equals(Subalterno.ToLower))
                        End If

                        DrParticellaAB = DtParticelleZoneA_Query.ToList

                        'DrParticellaAB = DtParticelleZoneA.Select("PROV='" & Prov & "' AND COM='" & Com & "' AND Sezione='" & Sezione & "' AND foglio=" & Foglio.ToString & " AND Numero=" & Numero.ToString & " AND subalterno='" & Subalterno & "'")
                        If Not DrParticellaAB Is Nothing AndAlso DrParticellaAB.Count > 0 Then
                            EliminaZona_AB = True
                        Else
                            EliminaZona_AB = False
                        End If
                    End If
                End If

                If InserisciZona_AB = True Then
                    If strZona_AB = "A" Then
                        XmlZona = objXML.Xml_Pubblico_Zona(enum_TipoOperazioneDB.Scrittura,
                               "-46",
                               "0",
                               XmlAnagrafica)
                    ElseIf strZona_AB = "B" Then
                        XmlZona = objXML.Xml_Pubblico_Zona(enum_TipoOperazioneDB.Scrittura,
                               "-47",
                               "0",
                               XmlAnagrafica)
                    End If

                    XmlParticella.AppendChild(XmlZona)
                End If

                If EliminaZona_AB = True Then
                    If strZona_AB = "A" Then
                        XmlZona = objXML.Xml_Pubblico_Zona(enum_TipoOperazioneDB.Cancellazione,
                               "-47",
                               "0",
                               XmlAnagrafica)
                    ElseIf strZona_AB = "B" Then
                        XmlZona = objXML.Xml_Pubblico_Zona(enum_TipoOperazioneDB.Cancellazione,
                               "-46",
                               "0",
                               XmlAnagrafica)
                    End If

                    XmlParticella.AppendChild(XmlZona)
                End If

            End If

            'se esiste il macrouso...........
            If Macrouso_Cod <> "" Then

                TipoOpMacrousoxParticella = enum_TipoOperazioneDB.Scrittura
                ModificaMacrouso = False
                'If TipoOperazione_Particella = enum_TipoOperazioneDB.Modifica Then
                If SaCod_u <> 0 Or saCods.Count <> 0 Then

                    Dim DtParticelleMacrousi_Query = DtParticelleMacrousiEO.Where(Function(c) c("PROV").Equals(Prov) AndAlso
                                                     c("COM").Equals(Com) AndAlso
                                                     c("FOGLIO").Equals(CInt(Foglio)) AndAlso
                                                     c("NUMERO").Equals(CInt(Numero)))

                    If Sezione = "" Then
                        DtParticelleMacrousi_Query = DtParticelleMacrousi_Query.Where(Function(c) c("SEZIONE").Equals("") Or c("SEZIONE").Equals("0"))
                    Else
                        DtParticelleMacrousi_Query = DtParticelleMacrousi_Query.Where(Function(c) CStr(c("SEZIONE")).ToLower.Equals(Sezione.ToLower))
                    End If

                    If Subalterno = "" Then
                        DtParticelleMacrousi_Query = DtParticelleMacrousi_Query.Where(Function(c) c("SUBALTERNO").Equals("") Or c("SUBALTERNO").Equals("0"))
                    Else
                        DtParticelleMacrousi_Query = DtParticelleMacrousi_Query.Where(Function(c) CStr(c("SUBALTERNO")).ToLower.Equals(Subalterno.ToLower))
                    End If

                    Dim DrParticellaM = DtParticelleMacrousi_Query.ToList

                    'DrParticellaM = DtParticelleMacrousi.Select("PROV='" & Prov & "' AND COM='" & Com & "' AND Sezione='" & Sezione & "' AND foglio=" & Foglio.ToString & " AND Numero=" & Numero.ToString & " AND subalterno='" & Subalterno & "'" & " AND macrouso_cod='" & Macrouso_Cod & "'")
                    'se il macrouso in quell'intervallo esiste giÃ  lo modifico
                    'altrimenti lo inserisco
                    If Not DrParticellaM Is Nothing AndAlso DrParticellaM.Count > 0 Then
                        For m = 0 To DrParticellaM.Count - 1
                            Inizio_Macrouso_Old = DrParticellaM(m)("Validita_Inizio")
                            Fine_Macrouso_Old = DrParticellaM(m)("Validita_Fine")
                            If Inizio_Macrouso < Fine_Macrouso_Old And Fine_Macrouso > Inizio_Macrouso_Old Then
                                ModificaMacrouso = True
                                Exit For
                            End If
                        Next
                    End If
                    If ModificaMacrouso = True Then
                        TipoOpMacrousoxParticella = enum_TipoOperazioneDB.Modifica
                    End If
                End If


                XmlMacrouso = objXML.Xml_Pubblico_Macrouso(TipoOpMacrousoxParticella,
                                                           Piva,
                                                           Macrouso_Cod.ToString,
                                                           Format(Macrouso_Sup, "0.0000"),
                                                           Inizio_Macrouso, Fine_Macrouso, XmlAnagrafica)

                XmlParticella.AppendChild(XmlMacrouso)

                Dim Id_Utilizzo As String


                'se esiste l'utilizzo...........
                If Veg_Cod_Agea <> "" Then

                    TipoOpUtilizzoxParticella = enum_TipoOperazioneDB.Scrittura
                    ModificaUtilizzo = False
                        Id_Utilizzo = "0"

                    If SaCod_u <> 0 Or saCods.Count <> 0 Then

                        Dim DtParticelleUtilizzi_Query = DtParticelleUtilizziEO.Where(Function(c) c("PROV").Equals(Prov) AndAlso
                                                     c("COM").Equals(Com) AndAlso
                                                     c("FOGLIO").Equals(CInt(Foglio)) AndAlso
                                                     c("NUMERO").Equals(CInt(Numero)))

                        If Sezione = "" Then
                            DtParticelleUtilizzi_Query = DtParticelleUtilizzi_Query.Where(Function(c) c("SEZIONE").Equals("") Or c("SEZIONE").Equals("0"))
                        Else
                            DtParticelleUtilizzi_Query = DtParticelleUtilizzi_Query.Where(Function(c) CStr(c("SEZIONE")).ToLower.Equals(Sezione.ToLower))
                        End If

                        If Subalterno = "" Then
                            DtParticelleUtilizzi_Query = DtParticelleUtilizzi_Query.Where(Function(c) c("SUBALTERNO").Equals("") Or c("SUBALTERNO").Equals("0"))
                        Else
                            DtParticelleUtilizzi_Query = DtParticelleUtilizzi_Query.Where(Function(c) CStr(c("SUBALTERNO")).ToLower.Equals(Subalterno.ToLower))
                        End If

                        Dim DrParticellaU = DtParticelleUtilizzi_Query.ToList

                        'DrParticellaU = DtParticelleUtilizzi.Select("PROV='" & Prov & "' AND COM='" & Com & "' AND Sezione='" & Sezione & "' AND foglio=" & Foglio.ToString & " AND Numero=" & Numero.ToString & " AND subalterno='" & Subalterno & "'" & " AND macrouso_cod='" & Macrouso_Cod & "'" & " AND Veg_Cod_Agea='" & Veg_Cod_Agea & "'" & " AND Cul_Cod_Agea='" & Cul_Cod_Agea & "'" & " AND Superficie = " & Replace(Format(Utilizzo_Sup, "0.###########"), ",", "."))
                        'se il macrouso in quell'intervallo esiste giÃ  lo modifico
                        'altrimenti lo inserisco
                        If Not DrParticellaU Is Nothing AndAlso DrParticellaU.Count > 0 Then
                            For m = 0 To DrParticellaU.Count - 1
                                Inizio_Utilizzo_Old = DrParticellaU(m)("Validita_Inizio")
                                Fine_Utilizzo_Old = DrParticellaU(m)("Validita_Fine")
                                If Inizio_Utilizzo < Fine_Utilizzo_Old And Fine_Utilizzo > Inizio_Utilizzo_Old Then
                                    ModificaUtilizzo = True
                                    Id_Utilizzo = DrParticellaU(m).Item("ID")
                                    Exit For
                                End If
                            Next
                        End If
                        If ModificaUtilizzo = True Then
                            TipoOpUtilizzoxParticella = enum_TipoOperazioneDB.Modifica
                        End If
                    End If

                    XmlUtilizzo = objXML.Xml_Pubblico_Utilizzo(TipoOpUtilizzoxParticella,
                                                               Piva,
                                                               Veg_Cod_Agea,
                                                               Cul_Cod_Agea,
                                                               Utilizzo_Sup,
                                                               Inizio_Utilizzo, Fine_Utilizzo, Id_Utilizzo, XmlAnagrafica)

                        XmlMacrouso.AppendChild(XmlUtilizzo)

                    'RIGA PIANIFICAZIONE
                    If CreaPlanning = True Then


                        Dim saCod As Integer = 0
                        If Flag_CentroUnico Then
                            saCod = SaCod_u
                        Else
                            Dim objIstat As New AgronicaCoreMetaSchemaDAL.Istat_R
                            Dim dtCom = objIstat.Leggi(Prov, Com, "", "", "", AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)
                            Dim com_des = dtCom.Rows(0).Item("LOCALITA")
                            Dim objCentri As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read
                            Dim dtCentro = objCentri.Leggi(Piva, 0, AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "Sa_Nome=" + AgronicaCoreDataProvider.UtilityProvider.Agro_SQL_SaveText_NULL(com_des) + "", "", objParametri_Server)
                            If dtCentro.Rows.Count > 0 Then
                                saCod = CInt(dtCentro.Rows(0).Item("sa_cod"))
                            Else
                                'codice_centro
                                XMLCentri = XmlAnagrafica.GetElementsByTagName("CentroAziendale")
                                If Flag_CentroUnico Then
                                    XmlCentro = XMLCentri(0)
                                Else
                                    For Each centro As System.Xml.XmlNode In XMLCentri
                                        'Dim objIstat As New AgronicaCoreMetaSchemaDAL.Istat_R
                                        Dim dttCom = objIstat.Leggi(Prov, Com, "", "", "", AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)
                                        Dim comm_des = dtCom.Rows(0).Item("LOCALITA")
                                        If centro.Attributes("nome_centro").InnerText = comm_des Then
                                            saCod = CInt(centro.Attributes("codice_centro").Value)
                                            Exit For
                                        End If
                                    Next
                                End If
                            End If
                        End If

                        XmlEntita = objXML.Xml_Pubblico_ProgrammazioneEntita(enum_TipoOperazioneDB.Scrittura,
                                    "0",
                                    "App. " & Right("000" & nEntita, 3),
                                    saCod,
                                    "#",
                                    -nEntita,
                                    "0", "0",
                                    "Lotto" & Anno_Piano_Colturale.ToString,
                                    Veg_Cod,
                                    Grfi_Cod,
                                    Cul_Cod,
                                    Grva_Cod,
                                    Veg_Cod_Agea, Cul_Cod_Agea, "#", "#",
                                    Id_Cod, "#",
                                    Utilizzo_Sup.ToString,
                                    "#",
                                    strZona,
                                    "#", "#", "#", "#", "#", "#",
                                    "102",
                                    "#", "#", "#",
                                    Inizio_Utilizzo,
                                    Fine_Utilizzo,
                                    Inizio_Utilizzo,
                                    XmlPianificazione,
                                    Codice_Fiscale_Tecnico:=Piva,
                                    Veg_Cod_Agea:=Veg_Cod_Agea,
                                    Cul_Cod_Agea:=Cul_Cod_Agea,
                                    Uso_Cod_Agea:=Uso_Cod_Agea,
                                    Occupazione_Cod_Agea:=Occupazione_Cod_Agea,
                                    Destinazione_Cod_Agea:=Destinazione_Cod_Agea,
                                    Qualita_Cod_Agea:=Qualita_Cod_Agea)


                        XmlParticellaP = objXML.Xml_Pubblico_ProgrammazioneParticella(enum_TipoOperazioneDB.Scrittura,
                                                                                 Com,
                                                                                 Prov,
                                                                                 Sezione,
                                                                                 Foglio,
                                                                                 Numero,
                                                                                 Subalterno,
                                                                                 Utilizzo_Sup.ToString,
                                                                                 "#", "#",
                                                                                 XmlPianificazione)

                        XmlEntita.AppendChild(XmlParticellaP)

                        nEntita += 1

                        XMLTestate = XmlPianificazione.GetElementsByTagName("Pianificazione")
                        XmlTestata = XMLTestate(0)

                        XmlTestata.AppendChild(XmlEntita)

                    End If


                End If



            End If

                XMLCentri = XmlAnagrafica.GetElementsByTagName("CentroAziendale")
            If Flag_CentroUnico Then
                XmlCentro = XMLCentri(0)
            Else
                For Each centro As System.Xml.XmlNode In XMLCentri
                    Dim objIstat As New AgronicaCoreMetaSchemaDAL.Istat_R
                    Dim dtCom = objIstat.Leggi(Prov, Com, "", "", "", AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)
                    Dim com_des = dtCom.Rows(0).Item("LOCALITA")
                    If centro.Attributes("nome_centro").InnerText = com_des Then
                        XmlCentro = centro
                        Exit For
                    End If
                Next
            End If

            XmlCentro.AppendChild(XmlParticella)

        Next

        stringaAnagrafica = XmlAnagrafica.OuterXml
        stringaPianificazione = XmlPianificazione.OuterXml

    End Sub

    '########################################################################################
    Public Sub Importa_Dati(ByVal CodiceChiaveCliente As Integer,
                             ByVal LinkWSImportaGIAS As String,
                             ByVal strDatiAnagrafe As String,
                             ByVal strDatiPianificazione As String,
                             ByRef strRisultato As String,
                             ByRef strErr As String,
                             ByRef objSession As System.Web.SessionState.HttpSessionState,
                             ByRef ObjParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                             ByRef ObjParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri)

        Dim Documento_Finale As New System.Xml.XmlDocument
        Dim XML_Risultato As System.Xml.XmlElement
        Dim XML_Risposta As System.Xml.XmlElement
        Dim XMLs_Risposta As System.Xml.XmlNodeList

        Dim StrFinaleAnagrafe As String
        Dim StrFinalePianificazione As String
        Dim Descrizione As String

        Dim x As Integer

        Dim Importa As New Ws_Importa_Gias.ImportaWS

        Try

            If LinkWSImportaGIAS <> "" Then
                Dim Str_Credenziali_WS As String
                Importa.Url = LinkWSImportaGIAS

                Importa.Timeout = Integer.MaxValue
                Descrizione = String.Empty

                Dim objXmlWs As New AgronicaCoreXML.XML_WS_Importa_Gias

                Str_Credenziali_WS = objXmlWs.Genera_Stringa_Credenziali(
                                     True,
                                     Nothing,
                                     objSession("ASG_Utente_Username"),
                                     objSession("ASG_Utente_Password"),
                                     ObjParametri_Server.PivaSuperUser,
                                     True,
                                     "",
                                     "",
                                     "",
                                     "",
                                     "",
                                     "",
                                     ObjParametri_Server.StringaConnessione,
                                     ObjParametri_Utenti.StringaConnessione)

                If strDatiAnagrafe <> String.Empty Then

                    Descrizione &= "<b>Importazione anagrafe:" & "</b></br>"

                    StrFinaleAnagrafe = Importa.Importa_DocumentoPubblico_SuperServer(Str_Credenziali_WS, strDatiAnagrafe, CodiceChiaveCliente)

                    Documento_Finale.LoadXml(StrFinaleAnagrafe)

                    XML_Risultato = Documento_Finale.SelectSingleNode("Risultato")

                    If XML_Risultato.HasAttribute("errore") Then

                        Descrizione &= "- " & XML_Risultato.GetAttribute("errore").ToString & "</br>"
                        strErr = Descrizione

                    Else

                        XMLs_Risposta = XML_Risultato.GetElementsByTagName("Risposta")

                        Dim strRisp As String = String.Empty

                        For x = 0 To XMLs_Risposta.Count - 1

                            XML_Risposta = XMLs_Risposta.Item(x)

                            strRisp = XML_Risposta.GetAttribute("Ris")

                            Descrizione &= "- " & strRisp & IIf(InStr(strRisp, "Errore"), "", " - Terminata correttamente") & "</br>"

                            strRisultato = Descrizione

                        Next

                    End If


                End If

                If strDatiPianificazione <> String.Empty Then

                    Descrizione &= "</br><b>Importazione pianificazione:" & "</b></br>"

                    'StrFinalePianificazione = Importa.Importa_Pianificazione(objSession("ASG_Utente_Username"),
                    '                                       objSession("ASG_Utente_Password"),
                    '                                       objSession("ASG_SuperUser_CodFiscale"),
                    '                                       strDatiPianificazione,
                    '                                       CodiceChiaveCliente)

                    StrFinalePianificazione = Importa.Importa_Pianificazione_SuperServer(Str_Credenziali_WS, strDatiPianificazione, CodiceChiaveCliente)

                    Documento_Finale.LoadXml(StrFinalePianificazione)

                    XML_Risultato = Documento_Finale.SelectSingleNode("Risultato")

                    XMLs_Risposta = XML_Risultato.GetElementsByTagName("Risposta")

                    Dim strRisp As String = String.Empty

                    For x = 0 To XMLs_Risposta.Count - 1

                        XML_Risposta = XMLs_Risposta.Item(x)

                        strRisp = XML_Risposta.GetAttribute("Ris")

                        Descrizione &= "- " & strRisp & IIf(InStr(strRisp, "Errore"), "", " - Terminata correttamente") & "</br>"

                        strRisultato &= Descrizione

                    Next


                End If


            Else

                strErr = "Inserire l'indirizzo del Web Service Gias per continuare."

            End If

        Catch ex As Exception

            strErr = ex.Message

        End Try


    End Sub
    Private Sub ScrivoIlPassaggioDiStatoIniziale(ByVal Stato_cod As enum_Servizi_Stati, ByVal NuovoPassaggioDiStato_cod As Integer, ByVal Servizio_Cod As Integer, ByVal DataFinePratica As Date, ByVal DataApertura As Date, ByVal Pratica_Cod As Integer, ByVal objPraticaStati_W As AgronicaCoreProfilazioneDAL.Pratiche_Stati_W, ByVal objPraticaStati_Attuale_W As AgronicaCoreProfilazioneDAL.Pratiche_Stati_Attuali_W, ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri)
        objPraticaStati_W.Scrivi(Pratica_Cod, _
                    Stato_cod, _
                    Servizio_Cod, _
                    DataApertura, DataFinePratica, _
                    "", 0, NuovoPassaggioDiStato_cod, _
                    objParametri_Server, _
                    Now.Today, Now.Today, _
                    objParametri_Server.UsernameOperazione, _
                    objParametri_Server.UsernameOperazione)
        objPraticaStati_Attuale_W.Scrivi(Pratica_Cod, _
                    Stato_cod, _
                    "", _
                    DataApertura, DataFinePratica, _
                    objParametri_Server, _
                    Now.Today, Now.Today, _
                    objParametri_Server.UsernameOperazione, _
                    objParametri_Server.UsernameOperazione)
    End Sub
    Public Sub Importa_Pratica(ByVal Servizio_Cod As Integer, ByVal Servizio_Des As String, _
                               ByVal Piva As String, ByVal CUAA As String, _
                               ByVal DataInizioPratica As Date, ByVal DataFinePratica As Date, _
                               ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                               Optional ByVal DataApertura As Date = AGRODATAINIZIO, _
                               Optional ByVal DataValidazione As Date = AGRODATAINIZIO, _
                               Optional ByVal DataChiusura As Date = AGRODATAINIZIO)


        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False

        Try


            'Se la connessione Ã¨ chiusa la apro e se la transazione Ã¨ chiusa la inizio
            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale, _
                                                                                    FlagTransazioneLocale, _
                                                                                    objParametri_Server)

            Dim LeggiSequenze As New AgronicaCoreDataProvider.Agro_Sequenze
            Dim NuovoPassaggioDiStato_cod = _
                LeggiSequenze.NuovoId_Tabella( _
                 "PassaggioDiStato_cod", _
                 0, _
                 _UpperBoundTabelle, _
                 objParametri_Server _
             )

            Dim Pratica_Cod As Integer = 0
            Dim InseritaPratica As Boolean = False

            Dim objPratica_W As New AgronicaCoreProfilazioneDAL.Pratiche_W
            Dim objPraticaStati_W As New AgronicaCoreProfilazioneDAL.Pratiche_Stati_W
            Dim objPraticaStati_Attuale_W As New AgronicaCoreProfilazioneDAL.Pratiche_Stati_Attuali_W

            Dim objSequenza As New AgronicaCoreDataProvider.Agro_Sequenze
            Pratica_Cod = objSequenza.NuovoId_Tabella("Pratiche", 0, 2000000000, objParametri_Server)
            InseritaPratica = objPratica_W.Scrivi(Pratica_Cod,
                                                Servizio_Des,
                                                Piva,
                                                CUAA,
                                                0, 0, 0,
                                                Servizio_Cod,
                                                DataInizioPratica, DataFinePratica,
                                                objParametri_Server,
                                                Now.Today, Now.Today,
                                                objParametri_Server.UsernameOperazione,
                                                objParametri_Server.UsernameOperazione, 0,
                                                "",
                                                "",
                                                AGRODATAINIZIO,
                                                "",
                                                0,
                                                0)

            If InseritaPratica = True Then

                If DataApertura <> AGRODATAINIZIO Then
                    ScrivoIlPassaggioDiStatoIniziale(enum_Servizi_Stati.Pratica_Aperta, NuovoPassaggioDiStato_cod, Servizio_Cod, DataFinePratica, DataApertura, Pratica_Cod, objPraticaStati_W, objPraticaStati_Attuale_W, objParametri_Server)
                    ScrivoIlPassaggioDiStatoIniziale(5, NuovoPassaggioDiStato_cod, Servizio_Cod, DataFinePratica, DataApertura, Pratica_Cod, objPraticaStati_W, objPraticaStati_Attuale_W, objParametri_Server)
                End If

                'vanni, 08/03/2013 commento la parte di "old school"
                'If DataValidazione <> AGRODATAINIZIO Then
                '    objPraticaStati_W.Scrivi(Pratica_Cod, _
                '                             enum_Servizi_Stati.Pratica_Validata, _
                '                             Servizio_Cod, _
                '                             DataValidazione, DataFinePratica, _
                '                             objParametri_Server, _
                '                             Now.Today, Now.Today, _
                '                             objParametri_Server.UsernameOperazione, _
                '                             objParametri_Server.UsernameOperazione)
                'End If

                'If DataChiusura <> AGRODATAINIZIO Then
                '    objPraticaStati_W.Scrivi(Pratica_Cod, _
                '                             enum_Servizi_Stati.Pratica_Chiusa, _
                '                             Servizio_Cod, _
                '                             DataChiusura, DataFinePratica, _
                '                             objParametri_Server, _
                '                             Now.Today, Now.Today, _
                '                             objParametri_Server.UsernameOperazione, _
                '                             objParametri_Server.UsernameOperazione)
                'End If
                ''fine vanni, 08/03/2013 commento la parte di "old school"

            End If


            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri_Server)


        Catch ex As Exception

            'Faccio il rollback della transazione
            If Not objParametri_Server.objTransazione Is Nothing Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)
            End If

        Finally

            'Chiudo la connessione se Ã¨ stata aperta in questa routine
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri_Server)

        End Try

    End Sub

End Class


