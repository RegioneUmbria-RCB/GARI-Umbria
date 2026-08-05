Imports System
Imports System.IO
Imports System.Xml.Serialization

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

    Public Sub SerializeObject(ByVal filename As String, ByVal ws_fasciResponse As SincroAnagrafeBA.NsFascicolo.getFascicoloResponse)

        Console.WriteLine("Writing With Stream")

        Dim serializer As New XmlSerializer(GetType(SincroAnagrafeBA.NsFascicolo.getFascicoloResponse))

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

    Public Sub Crea_Stringa_Catasto(ByRef stringaAnagrafica As String, _
                                   ByRef stringaPianificazione As String, _
                                   ByVal strXmlDoc As String, _
                                   ByVal strXmlDocP As String, _
                                   ByVal Piva As String, _
                                   ByVal SaCod As Integer, _
                                   ByVal Validita_Inizio As Date, _
                                   ByVal Validita_Fine As Date, _
                                   ByVal Anno_Piano_Colturale As Integer, _
                                   ByVal DtParticelle As DataTable, _
                                   ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri)

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
        Dim Veg_Cod, Cul_Cod, Grfi_Cod, Grva_Cod, Id_Cod As Integer

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

        Dim nEntita As Integer = 1


        Dim DtParticelleCentro As New DataTable
        Dim DtParticelleMacrousi As New DataTable
        Dim DtParticelleUtilizzi As New DataTable
        Dim DtParticelleZone As New DataTable
        Dim DrParticella() As DataRow
        Dim DrParticellaM() As DataRow
        Dim DrParticellaU() As DataRow
        Dim DrParticellaZ() As DataRow

        Dim XmlAnagrafica As New System.Xml.XmlDocument
        Dim XmlPianificazione As New System.Xml.XmlDocument

        XmlAnagrafica.LoadXml(strXmlDoc)
        XmlPianificazione.LoadXml(strXmlDocP)

        'Dim strValiditaInizio As String = String.Empty
        'Dim strValiditaFine As String = String.Empty
        'strValiditaInizio = "01/11/" & (Anno - 1).ToString
        'strValiditaFine = "31/10/" & Anno.ToString

        Dim objXML As New AgronicaCoreXML.AnagrafeXML
        Dim objImpresexParticelle As New AgronicaCoreAnagrafeDAL.ImpresexParticelle2_R
        Dim objParticellexMacrousi As New AgronicaCoreAnagrafeDAL.ParticelleCatastalixMacrousi_R
        Dim objParticellexMacrousixUtilizzo As New AgronicaCoreAnagrafeDAL.ParticelleCatastalixMacrousixUtilizzo_R
        Dim objParticellexZone As New AgronicaCoreAnagrafeDAL.ZonexParticelle_R

        If SaCod <> 0 Then

            DtParticelleCentro = objImpresexParticelle.Leggi(0, _
                                                           Piva, _
                                                           SaCod, _
                                                           0, _
                                                           "", "", "", 0, 0, "", _
                                                           AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, _
                                                           "", "", _
                                                           objParametri_Server)

            DtParticelleMacrousi = objParticellexMacrousi.Leggi_DaCentro(Piva, _
                                                           SaCod, _
                                                           "", "", "", 0, 0, "", _
                                                           "", _
                                                           "", "", _
                                                           objParametri_Server)

            DtParticelleUtilizzi = objParticellexMacrousixUtilizzo.Leggi_DaCentro(Piva, _
                                               SaCod, _
                                               "", "", "", 0, 0, "", _
                                               "", "", "", _
                                               "", "", _
                                               objParametri_Server)

            DtParticelleZone = objParticellexZone.Leggi(-17, _
                                    "", "", "", 0, 0, "", _
                                    AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, _
                                   "", "", _
                                   objParametri_Server)

        End If


        Dim HashParticelle As New Hashtable
        Dim HashMacrousi As New Hashtable

        'Leggo le particelle vulnerabili
        Dim objPV As New AgronicaCoreMetaSchemaDAL.ParticelleCatastali_Vulnerabili_R
        Dim DtPV As DataTable
        DtPV = objPV.Leggi("", "", "", 0, 0, "", 0, "", "", objParametri_Server)

        For i = 0 To DtParticelle.Rows.Count - 1

            TipoOperazione_Particella = enum_TipoOperazioneDB.Scrittura
            TipoOperazione_Possesso = enum_TipoOperazioneDB.Scrittura

            Dim ModificaParticella As Boolean = False
            Dim ModificaPossesso As Boolean = False
            Dim ModificaMacrouso As Boolean = False
            Dim ModificaUtilizzo As Boolean = False
            Dim InserisciZona As Boolean = False

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

            'se esiste giÃ  il centro verifico se contiene la particella
            If SaCod <> 0 Then
                DrParticella = DtParticelleCentro.Select("PROV='" & Prov & "' AND COM='" & Com & "' AND Sezione='" & Sezione & "' AND foglio=" & Foglio.ToString & " AND Numero=" & Numero.ToString & " AND subalterno='" & Subalterno & "'")
                'se la particella in quell'intervallo esiste giÃ  la modifico
                'altrimenti la inserisco
                If Not DrParticella Is Nothing AndAlso DrParticella.Length > 0 Then
                    ModificaParticella = True
                    For j = 0 To DrParticella.Length - 1
                        Inizio_Possesso_Old = DrParticella(j).Item("validita_inizio")
                        Fine_Possesso_Old = DrParticella(j).Item("validita_fine")
                        If Inizio_Possesso < Fine_Possesso_Old And Fine_Possesso > Inizio_Possesso_Old Then
                            ModificaPossesso = True
                            Exit For
                        End If
                    Next
                End If
                If ModificaParticella = True Then
                    TipoOperazione_Particella = enum_TipoOperazioneDB.Modifica
                End If
                If ModificaPossesso = True Then
                    TipoOperazione_Possesso = enum_TipoOperazioneDB.Modifica
                End If
            End If




            '-----------------------------------------------------------------------
            'verifico se la particella Ã¨ vulnerabile
            'se Ã¨ vulnerabile salvo il dato sia nella particella sia nell'entitÃ 
            Dim strZona As String = "n"
            Dim DrPV() As DataRow
            If Not DtPV Is Nothing AndAlso DtPV.Rows.Count > 0 Then
                DrPV = DtPV.Select("PROV='" & Prov & "' AND COM='" & Com & "' AND foglio=" & Foglio.ToString)
                'DrPV = DtPV.Select("PROV='" & Prov & "' AND COM='" & Com & "' AND Sezione='" & Sezione & "' AND foglio=" & Foglio.ToString & " AND Numero=" & Numero.ToString & " AND subalterno='" & Subalterno & "'")
                If Not DrPV Is Nothing AndAlso DrPV.Length > 0 Then
                    strZona = "v"
                End If
            End If

            '--------------------------------------------------------
            If Not HashParticelle.ContainsKey(Prov & "_" & Com & "_" & Sezione & "_" & Foglio & "_" & Numero & "_" & Subalterno) Then

                HashParticelle.Add(Prov & "_" & Com & "_" & Sezione & "_" & Foglio & "_" & Numero & "_" & Subalterno, "")

                XmlParticella = objXML.Xml_Pubblico_Particella(TipoOperazione_Particella, _
                                                          "0", _
                                                          Com, _
                                                          Prov, _
                                                          Sezione, _
                                                          Foglio, _
                                                          Numero, _
                                                          Subalterno, _
                                                          "#", _
                                                          Ettari, _
                                                          Are, _
                                                          Centiare, _
                                                          TitoloPossesso, _
                                                          "#", "#", "#", "#", _
                                                          Format(supConduzione, "0.0000"), _
                                                          Inizio_Possesso, _
                                                          Fine_Possesso, _
                                                          XmlAnagrafica)

                XmlPossesso = objXML.Xml_Pubblico_Particella_Possesso(TipoOperazione_Possesso, _
                                                                      "#", _
                                                                      TitoloPossesso, _
                                                                       Format(supConduzione, "0.0000"), _
                                                                       Inizio_Possesso, _
                                                                       Fine_Possesso, _
                                                                       XmlAnagrafica)
                XmlParticella.AppendChild(XmlPossesso)

            End If

            'se la particella Ã¨ vulnerabile...........
            'verifico se Ã¨ giÃ  legata alla 'zona vulnerabile ai nitrati'
            If strZona = "v" Then

                InserisciZona = True

                If Not DtParticelleZone Is Nothing AndAlso DtParticelleZone.Rows.Count > 0 Then
                    DrParticellaZ = DtParticelleZone.Select("PROV='" & Prov & "' AND COM='" & Com & "' AND Sezione='" & Sezione & "' AND foglio=" & Foglio.ToString & " AND Numero=" & Numero.ToString & " AND subalterno='" & Subalterno & "'")
                    'se la zona vulnerabile esiste giÃ  non faccio nulla
                    'altrimenti la inserisco
                    If Not DrParticellaZ Is Nothing AndAlso DrParticellaZ.Length > 0 Then
                        InserisciZona = False
                    End If
                End If

                If InserisciZona = True Then
                    XmlZona = objXML.Xml_Pubblico_Zona(enum_TipoOperazioneDB.Scrittura, _
                               "-17", _
                               "0", _
                               XmlAnagrafica)

                    XmlParticella.AppendChild(XmlZona)
                End If

            End If

            'se esiste il macrouso...........
            If Macrouso_Cod <> "" Then
                TipoOpMacrousoxParticella = enum_TipoOperazioneDB.Scrittura
                ModificaMacrouso = False
                'If TipoOperazione_Particella = enum_TipoOperazioneDB.Modifica Then
                If SaCod <> 0 Then
                    DrParticellaM = DtParticelleMacrousi.Select("PROV='" & Prov & "' AND COM='" & Com & "' AND Sezione='" & Sezione & "' AND foglio=" & Foglio.ToString & " AND Numero=" & Numero.ToString & " AND subalterno='" & Subalterno & "'" & " AND macrouso_cod='" & Macrouso_Cod & "'")
                    'se il macrouso in quell'intervallo esiste giÃ  lo modifico
                    'altrimenti lo inserisco
                    If Not DrParticellaM Is Nothing AndAlso DrParticellaM.Length > 0 Then
                        For m = 0 To DrParticellaM.Length - 1
                            Inizio_Macrouso_Old = DrParticellaM(m).Item("validita_inizio")
                            Fine_Macrouso_Old = DrParticellaM(m).Item("validita_fine")
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


                XmlMacrouso = objXML.Xml_Pubblico_Macrouso(TipoOpMacrousoxParticella, _
                                                           Piva, _
                                                           Macrouso_Cod.ToString, _
                                                           Format(Macrouso_Sup, "0.0000"), _
                                                           Inizio_Macrouso, Fine_Macrouso, XmlAnagrafica)

                XmlParticella.AppendChild(XmlMacrouso)

                Dim Id_Utilizzo As String

                'se esiste l'utilizzo...........
                If Veg_Cod_Agea <> "" Then
                    TipoOpUtilizzoxParticella = enum_TipoOperazioneDB.Scrittura
                    ModificaUtilizzo = False
                    Id_Utilizzo = "0"
                    'If TipoOperazione_Particella = enum_TipoOperazioneDB.Modifica Then
                    If SaCod <> 0 Then
                        DrParticellaU = DtParticelleUtilizzi.Select("PROV='" & Prov & "' AND COM='" & Com & "' AND Sezione='" & Sezione & "' AND foglio=" & Foglio.ToString & " AND Numero=" & Numero.ToString & " AND subalterno='" & Subalterno & "'" & " AND macrouso_cod='" & Macrouso_Cod & "'" & " AND Veg_Cod_Agea='" & Veg_Cod_Agea & "'" & " AND Cul_Cod_Agea='" & Cul_Cod_Agea & "'" & " AND Superficie = " & Replace(Format(Utilizzo_Sup, "0.###########"), ",", "."))
                        'se il macrouso in quell'intervallo esiste giÃ  lo modifico
                        'altrimenti lo inserisco
                        If Not DrParticellaU Is Nothing AndAlso DrParticellaU.Length > 0 Then
                            For m = 0 To DrParticellaU.Length - 1
                                Inizio_Utilizzo_Old = DrParticellaU(m).Item("validita_inizio")
                                Fine_Utilizzo_Old = DrParticellaU(m).Item("validita_fine")
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

                    XmlUtilizzo = objXML.Xml_Pubblico_Utilizzo(TipoOpUtilizzoxParticella, _
                                                               Piva, _
                                                               Veg_Cod_Agea, _
                                                               Cul_Cod_Agea, _
                                                               Utilizzo_Sup, _
                                                               Inizio_Utilizzo, Fine_Utilizzo, Id_Utilizzo, XmlAnagrafica)

                    XmlMacrouso.AppendChild(XmlUtilizzo)

                    'RIGA PIANIFICAZIONE

                    XmlEntita = objXML.Xml_Pubblico_ProgrammazioneEntita(enum_TipoOperazioneDB.Scrittura, _
                                    "0", _
                                    "App. " & Right("000" & nEntita, 3), _
                                    SaCod, _
                                    "#", _
                                    -nEntita, _
                                    "0", "0", _
                                    "Lotto" & Anno_Piano_Colturale.ToString, _
                                    Veg_Cod, _
                                    Grfi_Cod, _
                                    Cul_Cod, _
                                    Grva_Cod, _
                                    Veg_Cod_Agea, Cul_Cod_Agea, "#", "#", _
                                    Id_Cod, "#", _
                                    Utilizzo_Sup.ToString, _
                                    "#", _
                                    strZona, _
                                    "#", "#", "#", "#", "#", "#", _
                                    "102", _
                                    "#", "#", "#", _
                                    Inizio_Utilizzo, _
                                    Fine_Utilizzo, _
                                    Inizio_Utilizzo, _
                                    XmlPianificazione)


                    XmlParticellaP = objXML.Xml_Pubblico_ProgrammazioneParticella(enum_TipoOperazioneDB.Scrittura, _
                                                                                 Com, _
                                                                                 Prov, _
                                                                                 Sezione, _
                                                                                 Foglio, _
                                                                                 Numero, _
                                                                                 Subalterno, _
                                                                                 Utilizzo_Sup.ToString, _
                                                                                 "#", "#", _
                                                                                 XmlPianificazione)

                    XmlEntita.AppendChild(XmlParticellaP)

                    nEntita += 1

                    XMLTestate = XmlPianificazione.GetElementsByTagName("Pianificazione")
                    XmlTestata = XMLTestate(0)

                    XmlTestata.AppendChild(XmlEntita)

                End If

            End If

            XMLCentri = XmlAnagrafica.GetElementsByTagName("CentroAziendale")
            XmlCentro = XMLCentri(0)

            XmlCentro.AppendChild(XmlParticella)

        Next

        stringaAnagrafica = XmlAnagrafica.OuterXml
        stringaPianificazione = XmlPianificazione.OuterXml

    End Sub

    '########################################################################################
    Public Sub Importa_Dati(ByVal CodiceChiaveCliente As Integer, _
                             ByVal LinkWSImportaGIAS As String, _
                             ByVal strDatiAnagrafe As String, _
                             ByVal strDatiPianificazione As String, _
                             ByRef strRisultato As String, _
                             ByRef strErr As String, _
                             ByRef objSession As System.Web.SessionState.HttpSessionState, _
                             Optional ByVal TipoOperazione As Integer = 1)

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

                Importa.Url = LinkWSImportaGIAS
                Importa.Timeout = Integer.MaxValue
                Descrizione = String.Empty

                If strDatiAnagrafe <> String.Empty Then

                    Descrizione &= "<b>Importazione anagrafe:" & "</b></br>"

                    Dim objParametri_Server As New AgronicaCoreDataProvider.AgronicaCoreParametri(objSession("ASG_objParametri_Server"))
                    Dim objParametri_Utenti As New AgronicaCoreDataProvider.AgronicaCoreParametri(objSession("ASG_objParametri_Utenti"))

                    Dim objcoreXML As New AgronicaCoreXML.XML_WS_Importa_Gias
                    Dim strCredenziali As String
                    strCredenziali = objcoreXML.Genera_Stringa_Credenziali(True, Nothing,
                                                                           objSession("ASG_Utente_Username"),
                                                                           objSession("ASG_Utente_Password"),
                                                                           objSession("ASG_SuperUser_CodFiscale"),
                                                                           True,
                                                                           "", "", "", "", "", "",
                                                                           objParametri_Server.StringaConnessione,
                                                                           objParametri_Utenti.StringaConnessione)

                    StrFinaleAnagrafe = Importa.Importa_DocumentoPubblico_SuperServer(strCredenziali, strDatiAnagrafe, CInt(CodiceChiaveCliente))

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

                            Descrizione &= "- " & strRisp & If(InStr(strRisp, "Errore"), "", " - Terminata correttamente") & "</br>"

                            strRisultato = Descrizione

                        Next

                    End If


                End If

                If strDatiPianificazione <> String.Empty Then

                    Descrizione &= "</br><b>Importazione pianificazione:" & "</b></br>"

                    StrFinalePianificazione = Importa.Importa_Pianificazione(objSession("ASG_Utente_Username"), _
                                                           objSession("ASG_Utente_Password"), _
                                                           objSession("ASG_SuperUser_CodFiscale"), _
                                                           strDatiPianificazione, _
                                                           CodiceChiaveCliente)

                    Documento_Finale.LoadXml(StrFinalePianificazione)

                    XML_Risultato = Documento_Finale.SelectSingleNode("Risultato")

                    XMLs_Risposta = XML_Risultato.GetElementsByTagName("Risposta")

                    Dim strRisp As String = String.Empty

                    For x = 0 To XMLs_Risposta.Count - 1

                        XML_Risposta = XMLs_Risposta.Item(x)

                        strRisp = XML_Risposta.GetAttribute("Ris")

                        Descrizione &= "- " & strRisp & If(InStr(strRisp, "Errore"), "", " - Terminata correttamente") & "</br>"

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
                                                objParametri_Server.UsernameOperazione,
                                                0, "", "", AGRODATAINIZIO, "", 0, 0)

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


