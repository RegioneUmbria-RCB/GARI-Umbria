Imports System.Runtime.Serialization
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi

'Public Class Impresa
'    <DataMember()> _
'    Public Property StringVale() As String
'End Class



<DataContract()>
Public Class Risorse

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <value>1 scrittura - 2 Modifica</value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DataMember()>
    Public Property Tipo_Operazione() As Integer

    <DataMember()>
    Public Property Piva_SuperUser() As String

    <DataMember()>
    Public Property Partita_Iva() As String

    'mat_cod 0
    <DataMember()>
    Public Property Mat_Cod() As Integer

    <DataMember()>
    Public Property Codice_Articolo() As String

    <DataMember()>
    Public Property Descrizione_Articolo() As String

    <DataMember()>
    Public Property Codice_Specie() As Integer

    <DataMember()>
    Public Property Codice_Varieta() As Integer

    <DataMember()>
    Public Property Biologico() As Boolean

    <DataMember()>
    Public Property Convenzionale() As Boolean

    <DataMember()>
    Public Property Note() As String





    Public Sub ImpostaArticoloSuperUser()
        Partita_Iva = Piva_SuperUser
    End Sub



    Private Function GeneraStringaXML(ByVal BaseCode As Integer, ByVal TopCode As Integer, ByVal objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As String

        Dim XmlDoc As New System.Xml.XmlDocument
        Dim XmlDoc_Prodotto As New System.Xml.XmlDocument

        Dim XML_MateriePrime As System.Xml.XmlElement
        Dim XML_MateriaPrima As System.Xml.XmlElement

        Dim XML_ParametriQualitativi As System.Xml.XmlElement
        Dim XML_ParametroQualitativo As System.Xml.XmlElement

        Dim strDescrizione As String
        Dim strPrezzi As String

        Dim Gen_Cod As Integer
        Dim Spe_Cod As Integer
        Dim Raz_Cod As Integer
        Dim Ipro_Cod As Integer
        Dim Cat_Cod As Integer
        Dim ArrayAnimale() As String

        Dim ArrayIndice() As String

        Dim i As Integer

        'riprendo il valore della piva

        Dim xCategoriaGestita As Integer = SEMILAVORATI_VEGETALI
        Dim xSa_Cod As Integer = -1

        Dim Qs_Operazione As Integer = enum_TipoOperazioneDB.Scrittura

        XML_MateriePrime = XmlDoc.CreateElement("DatiMaterie_Prime")
        XML_MateriaPrima = XmlDoc.CreateElement("Materia_Prima")


        XML_MateriaPrima.SetAttribute("TipoOperazioneDB", Qs_Operazione)
        XML_MateriaPrima.SetAttribute("piva", CStr(Me.Partita_Iva))
        'imposto la visibilita pubblica
        XML_MateriaPrima.SetAttribute("sa_cod", xSa_Cod)

        XML_MateriaPrima.SetAttribute("elem_cod", CInt(xCategoriaGestita))

        If Qs_Operazione = 2 Then
            XML_MateriaPrima.SetAttribute("mat_cod", CInt(Me.Mat_Cod))
        Else
            XML_MateriaPrima.SetAttribute("mat_cod", CInt(0))
        End If

        strDescrizione = Me.Descrizione_Articolo

        XML_MateriaPrima.SetAttribute("mat_des", CStr(strDescrizione))
        XML_MateriaPrima.SetAttribute("cod_articolo", CStr(Me.Codice_Articolo))
        XML_MateriaPrima.SetAttribute("cod_articolo_old", Me.Codice_Articolo)
        XML_MateriaPrima.SetAttribute("cal_cod", CInt(0))
        XML_MateriaPrima.SetAttribute("prezzo_unitario", CStr(0))

        '--------------------------------------------
        '##############################################################################
        '###################### MATERIA PRIMA AZIENDALE  ##############################
        '##############################################################################

        Select Case (xCategoriaGestita)

            '====================================================================================='
            'PRODUZIONE VEGETALE
            '-------------------------------------------------------------------------------------

            Case 10

                'XML_MateriaPrima.SetAttribute("sem_cod", CInt(Me.cmb_TipologiaSemente.SelectedItem.Value))
                'XML_MateriaPrima.SetAttribute("veg_cod", CInt(Me.cmb_SpecieVegetale.SelectedItem.Value))
                'If cmb_Varieta.SelectedIndex > 0 Then
                '    XML_MateriaPrima.SetAttribute("cul_cod", CInt(Me.cmb_Varieta.SelectedItem.Value))
                'Else
                '    XML_MateriaPrima.SetAttribute("cul_cod", CInt(0))
                'End If
                'If cmb_Regolamento.SelectedItem.Text <> "" Then
                '    XML_MateriaPrima.SetAttribute("regolamento", CInt(Me.cmb_Regolamento.SelectedItem.Value))
                'Else
                '    XML_MateriaPrima.SetAttribute("regolamento", CInt(1))
                'End If

                'If Me.Cmb_TipologiaVarietale.SelectedItem.Text <> "" Then
                '    XML_MateriaPrima.SetAttribute("grva_cod_veg", Cmb_TipologiaVarietale.SelectedItem.Value)
                'Else
                '    XML_MateriaPrima.SetAttribute("grva_cod_veg", 0)
                'End If

                'If Me.cmb_Ditta.SelectedIndex > 0 Then
                '    XML_MateriaPrima.SetAttribute("ditta_cod", CInt(Me.cmb_Ditta.SelectedItem.Value))
                'Else
                '    XML_MateriaPrima.SetAttribute("ditta_cod", CInt(0))
                'End If


                ''====================================================================================='
                ''Semilavorati, Beni Confezionamento, Trasformati (Vegetali)
                ''-------------------------------------------------------------------------------------

            Case 201, 205, 210


                XML_MateriaPrima.SetAttribute("sem_cod", CInt(0))
                XML_MateriaPrima.SetAttribute("veg_cod", CInt(Me.Codice_Specie))

                XML_MateriaPrima.SetAttribute("cul_cod", CInt(Me.Codice_Varieta))

                'If cmb_Regolamento.SelectedItem.Text <> "" Then
                '    XML_MateriaPrima.SetAttribute("regolamento", CInt(Me.cmb_Regolamento.SelectedItem.Value))
                'Else
                XML_MateriaPrima.SetAttribute("regolamento", CInt(1))
                'End If

                'If Me.cmb_Ditta.SelectedIndex > 0 Then
                '    XML_MateriaPrima.SetAttribute("ditta_cod", CInt(Me.cmb_Ditta.SelectedItem.Value))
                'Else
                XML_MateriaPrima.SetAttribute("ditta_cod", CInt(0))
                'End If

                'If Me.Cmb_TipologiaVarietale.SelectedItem.Text <> "" Then
                '    XML_MateriaPrima.SetAttribute("grva_cod_veg", Cmb_TipologiaVarietale.SelectedItem.Value)
                'Else
                XML_MateriaPrima.SetAttribute("grva_cod_veg", 0)
                'End If


                '====================================================================================='
                '=====================================================================================
                'Gestione Parametri Qualitativi
                '-------------------------------------------------------------------------------------
                'XML_ParametriQualitativi = XmlDoc.CreateElement("DatiParametri_Qualitativi")

                ''Calibri
                'For i = 0 To Me.CBL_Calibri.Items.Count - 1

                '    XML_ParametroQualitativo = XmlDoc.CreateElement("Parametro_Qualitativo")

                '    XML_ParametroQualitativo.SetAttribute("TipoOperazioneDB", Check_OperazioneDB_PQ("calibro", CInt(CBL_Calibri.Items(i).Value), 0, CBL_Calibri.Items(i).Selected))
                '    XML_ParametroQualitativo.SetAttribute("mat_cod", xMat_Cod)
                '    XML_ParametroQualitativo.SetAttribute("tipo", "calibro")
                '    XML_ParametroQualitativo.SetAttribute("tipo_cod", CInt(CBL_Calibri.Items(i).Value))
                '    XML_ParametroQualitativo.SetAttribute("udm_cod", 0)

                '    XML_ParametriQualitativi.AppendChild(XML_ParametroQualitativo)

                'Next

                ''Indici di Maturità
                'For i = 0 To Me.CBL_IndiciMaturita.Items.Count - 1

                '    XML_ParametroQualitativo = XmlDoc.CreateElement("Parametro_Qualitativo")

                '    ArrayIndice = Split(CBL_IndiciMaturita.Items(i).Value, "|")
                '    XML_ParametroQualitativo.SetAttribute("TipoOperazioneDB", Check_OperazioneDB_PQ("indice", CInt(ArrayIndice(0)), CInt(ArrayIndice(1)), CBL_IndiciMaturita.Items(i).Selected))
                '    XML_ParametroQualitativo.SetAttribute("mat_cod", xMat_Cod)
                '    XML_ParametroQualitativo.SetAttribute("tipo", "indice")
                '    XML_ParametroQualitativo.SetAttribute("tipo_cod", CInt(ArrayIndice(0)))
                '    XML_ParametroQualitativo.SetAttribute("udm_cod", CInt(ArrayIndice(1)))

                '    XML_ParametriQualitativi.AppendChild(XML_ParametroQualitativo)

                'Next

                'XML_MateriaPrima.AppendChild(XML_ParametriQualitativi)
                '=====================================================================================

            Case Else

                XML_MateriaPrima.SetAttribute("sem_cod", CInt(0))
                XML_MateriaPrima.SetAttribute("veg_cod", CInt(0))
                XML_MateriaPrima.SetAttribute("cul_cod", CInt(0))
                XML_MateriaPrima.SetAttribute("grva_cod_veg", CInt(0))
                XML_MateriaPrima.SetAttribute("regolamento", CInt(1))
                XML_MateriaPrima.SetAttribute("ditta_cod", CInt(0))


        End Select


        Select Case (xCategoriaGestita)

            '====================================================================================='
            'PRODUZIONE ANIMALE
            '-------------------------------------------------------------------------------------
            Case 301, 305, 306, 307, 310

                'If Me.Cmb_SpecieAnimale.SelectedItem.Text <> "" Then
                '    ArrayAnimale = Split(Cmb_SpecieAnimale.SelectedItem.Value, "|")
                '    Gen_Cod = ArrayAnimale(0)
                '    Spe_Cod = ArrayAnimale(1)
                '    ArrayAnimale = Nothing
                'End If

                'If Me.Cmb_IndProduttivo.SelectedItem.Text <> "" Then
                '    ArrayAnimale = Split(Me.Cmb_IndProduttivo.SelectedItem.Value, "|")
                '    Ipro_Cod = ArrayAnimale(2)
                '    ArrayAnimale = Nothing
                'End If

                'If Me.Cmb_Razza.SelectedItem.Text = "" Then
                '    AgroMsgBox("Selezionare la Razza!", Page)
                '    Exit Function
                'Else
                '    ArrayAnimale = Split(Me.Cmb_Razza.SelectedItem.Value, "|")
                '    Raz_Cod = ArrayAnimale(2)
                '    ArrayAnimale = Nothing
                'End If

                'XML_MateriaPrima.SetAttribute("gen_cod", Gen_Cod)
                'XML_MateriaPrima.SetAttribute("spe_cod", Spe_Cod)
                'XML_MateriaPrima.SetAttribute("ipro_cod", Ipro_Cod)
                'XML_MateriaPrima.SetAttribute("raz_cod", Raz_Cod)
                ''XML_MateriaPrima.SetAttribute("cat_cod", Cat_Cod)

            Case Else

                XML_MateriaPrima.SetAttribute("gen_cod", 0)
                XML_MateriaPrima.SetAttribute("spe_cod", 0)
                XML_MateriaPrima.SetAttribute("ipro_cod", 0)
                XML_MateriaPrima.SetAttribute("raz_cod", 0)
                'XML_MateriaPrima.SetAttribute("cat_cod", 0)

        End Select

        Select Case (xCategoriaGestita)

            Case 3

                'If Me.Txt_N.Text <> "" Then
                '    XML_MateriaPrima.SetAttribute("n", CInt(Me.Txt_N.Text))
                'Else
                '    XML_MateriaPrima.SetAttribute("n", CInt(0))
                'End If
                'If Me.Txt_P2O5.Text <> "" Then
                '    XML_MateriaPrima.SetAttribute("p2o5", CInt(Me.Txt_P2O5.Text))
                'Else
                '    XML_MateriaPrima.SetAttribute("p2o5", CInt(0))
                'End If
                'If Me.Txt_K2O.Text <> "" Then
                '    XML_MateriaPrima.SetAttribute("k2o", CInt(Me.Txt_K2O.Text))
                'Else
                '    XML_MateriaPrima.SetAttribute("k2o", CInt(0))
                'End If
                'If Me.Txt_MgO.Text <> "" Then
                '    XML_MateriaPrima.SetAttribute("mgo", CInt(Me.Txt_MgO.Text))
                'Else
                '    XML_MateriaPrima.SetAttribute("mgo", CInt(0))
                'End If

            Case Else

                XML_MateriaPrima.SetAttribute("n", CInt(0))
                XML_MateriaPrima.SetAttribute("p2o5", CInt(0))
                XML_MateriaPrima.SetAttribute("k2o", CInt(0))
                XML_MateriaPrima.SetAttribute("mgo", CInt(0))

        End Select

        XML_MateriaPrima.SetAttribute("trap_dur", CInt(0))
        XML_MateriaPrima.SetAttribute("uso", CInt(0))
        XML_MateriaPrima.SetAttribute("cltoss_cod", CInt(0))
        XML_MateriaPrima.SetAttribute("newcltoss_cod", CInt(0))


        XML_MateriaPrima.SetAttribute("flag_biologico", CInt(IIf(Biologico = True, 1, 0)))
        XML_MateriaPrima.SetAttribute("flag_convenzionale", CInt(IIf(Convenzionale = True, 1, 0)))
        'XML_MateriaPrima.SetAttribute("flag_nonagricolo", CInt(IIf(Me.Chk_OrigineNonAgricola.Checked = True, 1, 0)))
        XML_MateriaPrima.SetAttribute("flag_nonagricolo", CInt(0))
        XML_MateriaPrima.SetAttribute("flag_ausiliarefabbricazione", CInt(0))
        'XML_MateriaPrima.SetAttribute("flag_ausiliarefabbricazione", CInt(IIf(Me.Chk_AusiliareFabbricazione.Checked = True, 1, 0)))


        XML_MateriaPrima.SetAttribute("note", CStr(Note))
        XML_MateriaPrima.SetAttribute("extra_int", CInt(0))
        XML_MateriaPrima.SetAttribute("extra_str", CStr(""))
        XML_MateriaPrima.SetAttribute("extra_date", CDate(#12/31/2100#))
        XML_MateriaPrima.SetAttribute("validita_inizio", CDate(#1/1/1900#))
        XML_MateriaPrima.SetAttribute("validita_fine", CDate(#12/31/2100#))
        XML_MateriaPrima.SetAttribute("basecode", CInt(BaseCode))
        XML_MateriaPrima.SetAttribute("topcode", CInt(TopCode))

        '--------------------------------------------


        'se ho modificato i prezzi allora aggiungo l'xml, altrimenti lo ignoro
        'If viewstate("ModificatoPrezzi") = True Then

        '    strPrezzi = XML_GeneraStringonePrezzi()

        '    XML_MateriaPrima.InnerXml = XML_MateriaPrima.InnerXml & strPrezzi

        'End If

        XML_MateriePrime.AppendChild(XML_MateriaPrima)


        Return XML_MateriePrime.OuterXml
    End Function

    Public Sub Salva(ByVal BaseCode As Integer, ByVal TopCode As Integer, ByVal objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri, ByVal Log_Errori As String)

        Dim objDP As New AgronicaCoreDataProvider.ConnessioniTransazioni

        Dim Flag_Insert As Boolean = False
        Dim New_Mat_Cod As Integer = 0
        Dim StringaXmlCreazione As String

        'Creo la stringa di inserimento
        StringaXmlCreazione = Me.GeneraStringaXML(BaseCode, TopCode, objParametri_Server)


        Try

            'Non è necessario gestire la connessione esplicitamente perché la chiamata al core la gestisce
            'da sola.
            '-----------------------------------------------------
            '----------- CONNESSIONE E TRANSAZIONE ---------------
            'objDP.ApriConnessione(True, objParametri_Server)
            '-----------------------------------------------------

            '-----------------------------------------------------
            '--------------- SCRITTURA  --------------------
            Dim objMP As New AgronicaCoreAnagrafeBIZ.Materie_Prime_W


            Flag_Insert = objMP.Materia_Prima_Scrivi( _
                                CStr(StringaXmlCreazione), _
                                New_Mat_Cod, _
                                AGRODATAINIZIO, _
                                AGRODATAFINE, _
                                objParametri_Server)

            objMP = Nothing
            '-----------------------------------------------------

            'If Flag_Insert = False Then
            '    'inserimento fallito
            '    objDP.ChiudiTransazione(2, objParametri_Server)
            '    objDP.ChiudiConnessione(objParametri_Server)
            'Else
            '    'chiudi connessione e commit transazione
            '    objDP.ChiudiTransazione(1, objParametri_Server)
            '    objDP.ChiudiConnessione(objParametri_Server)
            'End If

        Catch ex As Exception
            Log_Errori += ex.Message + vbCrLf
        End Try


    End Sub


    Public Function Leggi(ByVal Codice_Articolo As String, ByVal objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri) As List(Of Risorse)

        Dim l_Risorse As New List(Of Risorse)

        Dim objDP As New AgronicaCoreDataProvider.ConnessioniTransazioni

        Dim objAn As New AgronicaCoreAnagrafeDAL.Materie_Prime_R
        Dim Dt As DataTable
        Dt = objAn.Leggi(objParametri_Server.PivaSuperUser, _
                         0, 0, 0, Codice_Articolo, 0, 0, 0, 0, 0, 0, 0, "", 0, "", True, True, "", AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, _
                         "", "", objParametri_Server)

        Dim i As Integer
        If Dt.Rows.Count > 0 Then
            For i = 0 To Dt.Rows.Count - 1
                Dim r As New Risorse
                r.Codice_Articolo = Codice_Articolo
                r.Codice_Specie = Dt.Rows(i).Item("veg_cod")
                r.Codice_Varieta = Dt.Rows(i).Item("cul_cod")
                r.Mat_Cod = Dt.Rows(i).Item("Mat_cod")
                r.Partita_Iva = Dt.Rows(i).Item("Piva")
                r.Piva_SuperUser = Dt.Rows(i).Item("Piva")
                r.Biologico = IIf(Dt.Rows(i).Item("Flag_Biologico") = 1, True, False)
                r.Convenzionale = IIf(Dt.Rows(i).Item("flag_convenzionale") = 1, True, False)
                r.Descrizione_Articolo = Dt.Rows(i).Item("Mat_Des")
                l_Risorse.Add(r)
            Next
        End If
        Return l_Risorse
    End Function

End Class
