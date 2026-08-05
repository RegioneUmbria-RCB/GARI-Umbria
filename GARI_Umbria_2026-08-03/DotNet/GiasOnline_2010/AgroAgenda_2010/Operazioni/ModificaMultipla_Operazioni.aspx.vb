Imports AgronicaCoreModello.ParametriAgenda_Temp
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreUtility

Imports AgronicaCoreModello.OperazioneAgenda_Temp

Public Class ModificaMultipla_Operazioni
    Inherits System.Web.UI.Page

    Dim objParametriAgenda As ParametriAgenda
    Dim objParametri_Server, objParametri_Utenti As AgronicaCoreParametri

    Dim Qs_Unid_Operazioni As String


    Private Sub ModificaMultipla_Operazioni_Init(sender As Object, e As System.EventArgs) Handles Me.Init
        AddHandler CType(Page.Master.FindControl("ImgBtn_AnnullaTutto"), ImageButton).Click, AddressOf Me.AnnullaTutto
    End Sub

    Private Sub AnnullaTutto(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim Link As String
        Link = CType(Master, Agenda).TrovaRedirectCorretto(False, enum_PagineGiasOnline.MenuAgenda, objParametriAgenda)
        Response.Redirect(Link)

    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        '##############################################################
        '#####  Verifico Credenziali di Accesso  ######################
        '##############################################################

        '----- Verifico che l'utente sia autenticato
        If Session("ASG_Utente_Username") = "" Then
            Response.Redirect("~/Custom500.aspx")
        End If

        If Not IsNothing(Request.QueryString("unid_operazioni")) Then
            Qs_Unid_Operazioni = Stringa_Decodifica(Request.QueryString("unid_operazioni").ToString, _
                                                AgroKey_EncoderDecoder, _
                                                Server)
        Else
            Qs_Unid_Operazioni = ""
        End If

        CaricaObjParametri()
        InizializzaScripts()


        If Not Page.IsPostBack Then
            Combo_Modifiche_SelectedIndexChanged(Me, Nothing)
        Else
            Exit Sub
        End If


        If Qs_Unid_Operazioni <> "" Then

            'se provengo dall'agenda devo ricaricare i controlli
            Dim objWebW As New AgronicaCoreVarieDAL.Web_ComunicazionePagine_W
            Dim objWebC As New AgronicaCoreVarieDAL.Web_ComunicazionePagine_R
            Dim DtWebC As DataTable

            DtWebC = objWebC.Leggi(Qs_Unid_Operazioni, 0, AGRODATAINIZIO, AGRODATAFINE, enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_Server)
            objWebW.Cancella(Qs_Unid_Operazioni, 0, "", objParametri_Server)

            If DtWebC.Rows.Count > 0 Then

                Dim StringaXmlOperazioni As String = ""
                StringaXmlOperazioni = DtWebC.Rows(0).Item("Stringa_Parametri_Base")
                If StringaXmlOperazioni <> "" Then
                    Carica_Operazioni(StringaXmlOperazioni)
                End If

            End If

        End If


    End Sub

    Private Sub CaricaObjParametri()

        objParametriAgenda = New ParametriAgenda

        '---
        objParametri_Server = New AgronicaCoreParametri(Session("ASG_objParametri_Server"))
        objParametri_Utenti = New AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))
        '---

    End Sub

    Private Sub InizializzaScripts()

        Dim contenitore As String = "window"
        Dim scarto As String = "-30"
        Dim scartoW As String = "-60"

        Dim stb As New StringBuilder
        stb.AppendLine("$(document).ready(function () {")
        stb.AppendLine(" $('.bottone').button(); ")
        stb.AppendLine("});")

        ScriptManager.RegisterClientScriptBlock(UpdatePanel_Ricerca, UpdatePanel_Ricerca.GetType(),
                                 String.Format("jQuery_{0}", UpdatePanel_Ricerca.ClientID), stb.ToString, True)


    End Sub

    Protected Sub Combo_Modifiche_SelectedIndexChanged(sender As Object, e As EventArgs) Handles Combo_Modifiche.SelectedIndexChanged

        Riga_Magazzino.Visible = False
        Riga_Macchine.Visible = False
        Riga_Contatti.Visible = False

        Select Case Combo_Modifiche.SelectedValue

            Case enum_ModificaMultiplaOperazioni.AggiungiMagazzino
                Riga_Magazzino.Visible = True
                CaricaMagazzini()

            Case enum_ModificaMultiplaOperazioni.AggiungiMacchina
                Riga_Macchine.Visible = True
                CaricaListaMacchinari()

            Case enum_ModificaMultiplaOperazioni.AggiungiContatto
                Riga_Contatti.Visible = True
                CaricaListaPersone()

        End Select

    End Sub

    Private Sub CaricaMagazzini()

        ComboMagazzini.Piva = objParametriAgenda.Piva
        ComboMagazzini.Flag_CodCentroFabbricato = True
        ComboMagazzini.TipoMagazzino = MAGAZZINO
        ComboMagazzini.PrimaRiga_Flag = False
        ComboMagazzini.CaricaComboMagazzini()

        ''imposto il valore
        'If objParametriAgenda.Fabbricato <> "0" AndAlso Split(objParametriAgenda.Fabbricato, "|").Count = 3 Then
        '    ComboMagazzini.Valore_Combo = objParametriAgenda.Fabbricato
        '    ComboMagazzini.ddl_Magazzini.SelectedIndex = ComboMagazzini.ddl_Magazzini.Items.IndexOf(ComboMagazzini.ddl_Magazzini.Items.FindByValue(objParametriAgenda.Fabbricato))
        'Else
        '    ComboMagazzini.Valore_Combo = objParametriAgenda.Fabbricato & "|" & objParametriAgenda.Piva
        '    ComboMagazzini.ddl_Magazzini.SelectedIndex = ComboMagazzini.ddl_Magazzini.Items.IndexOf(ComboMagazzini.ddl_Magazzini.Items.FindByValue(objParametriAgenda.Fabbricato & "|" & objParametriAgenda.Piva))
        'End If

    End Sub

    Private Sub CaricaListaMacchinari()

        Dim objMacchinari As New AgronicaCoreContabDAL.Parco_Macchine_R
        Dim DT As DataTable

        Dim i As Integer
        Dim Piva As String = ""
        Dim PivaSingola As Boolean = True

        For i = 0 To Me.GridView_Operazioni.Rows.Count - 1
            If i = 0 Then
                Piva = GridView_Operazioni.Rows(i).Cells(3).Text
            Else
                If Piva <> GridView_Operazioni.Rows(i).Cells(3).Text Then
                    PivaSingola = False
                    Piva = ""
                    Exit For
                End If
            End If
        Next

        Select Case PivaSingola
            Case True
                'carico le macchine aziendali + quelle pubbliche
                Dim pubblici As Boolean
                If chk_solo_privati_macchine.Checked Then
                    pubblici = False
                Else
                    pubblici = True
                End If

                DT = objMacchinari.ParcoMacchine_Leggi(Piva, _
                                     0, pubblici, "", "", "", "", "", 0, "", True, 0, "", True, AGRODATAINIZIO, AGRODATAFINE, _
                                     enumSelezioneVariabile.Selezione_TabellaCompleta, _
                                     "", " Mac_Des", objParametri_Server)
            Case Else
                DT = objMacchinari.ParcoMacchine_Leggi("----", _
                    0, True, "", "", "", "", "", 0, "", True, 0, "", True, AGRODATAINIZIO, AGRODATAFINE, _
                    enumSelezioneVariabile.Selezione_TabellaCompleta, _
                    "", " Mac_Des", objParametri_Server)
        End Select
       




        Dim Dt_Macchine As New DataTable
        ''----- Definisco la struttura del DataTable
        ''1
        Dt_Macchine.Columns.Add(New DataColumn("Piva", GetType(String)))
        ''2
        Dt_Macchine.Columns.Add(New DataColumn("sa_cod", GetType(String)))
        ''3
        Dt_Macchine.Columns.Add(New DataColumn("Mac_Cod", GetType(String)))
        ''4
        Dt_Macchine.Columns.Add(New DataColumn("CLASS_DESC", GetType(String)))
        ''5
        Dt_Macchine.Columns.Add(New DataColumn("Mac_Des", GetType(String)))
        ''6
        Dt_Macchine.Columns.Add(New DataColumn("Modello", GetType(String)))
        ''7
        Dt_Macchine.Columns.Add(New DataColumn("Targa", GetType(String)))
        ''8
        Dt_Macchine.Columns.Add(New DataColumn("Prezzo_Unitario", GetType(String)))
        ''9
        Dt_Macchine.Columns.Add(New DataColumn("Unita_Misura", GetType(String)))
        ''10
        Dt_Macchine.Columns.Add(New DataColumn("Impresa", GetType(String)))
        '11
        Dt_Macchine.Columns.Add(New DataColumn("Costo_Inizio", GetType(String)))
        '12
        Dt_Macchine.Columns.Add(New DataColumn("Costo_Fine", GetType(String)))


        Dim DR As DataRow

        Dim Costo_Inizio, Costo_Fine As String
        For i = 0 To DT.Rows.Count - 1
            DR = Dt_Macchine.NewRow()
            DR.Item("Piva") = DT.Rows(i).Item("Piva")
            DR.Item("sa_cod") = DT.Rows(i).Item("sa_cod")
            DR.Item("Mac_Cod") = DT.Rows(i).Item("Mac_Cod")
            DR.Item("CLASS_DESC") = DT.Rows(i).Item("CLASS_DESC")
            DR.Item("Mac_Des") = DT.Rows(i).Item("Mac_Des")
            DR.Item("Modello") = DT.Rows(i).Item("Modello")

            If DT.Rows(i).Item("Mezzo") = 1 Then
                DR.Item("Unita_Misura") = "Ha"
            ElseIf DT.Rows(i).Item("Mezzo") = 2 Then
                DR.Item("Unita_Misura") = "Ora"
            End If

            If DT.Rows(i).Item("Elem_Cod") = "1" Then
                DR.Item("Prezzo_Unitario") = DT.Rows(i).Item("Prezzo_Unitario")

                Costo_Inizio = DT.Rows(i).Item("Costo_Inizio")
                If Costo_Inizio.Length > 0 Then
                    Costo_Inizio = Costo_Inizio.Substring(0, 10)
                    If Costo_Inizio = "01/01/1900" Then
                        Costo_Inizio = "---"
                    End If
                Else
                    Costo_Inizio = "---"
                End If
                DR.Item("Costo_Inizio") = Costo_Inizio

                Costo_Fine = DT.Rows(i).Item("Costo_Fine")
                If Costo_Fine.Length > 0 Then
                    Costo_Fine = Costo_Fine.Substring(0, 10)
                    If Costo_Fine = "31/12/2100" Then
                        Costo_Fine = "---"
                    End If
                Else
                    Costo_Fine = "---"
                End If
                DR.Item("Costo_Fine") = Costo_Fine
            Else
                DR.Item("Prezzo_Unitario") = "0"
                Costo_Inizio = "---"
                Costo_Fine = "---"
                DR.Item("Costo_Fine") = Costo_Fine
                DR.Item("Costo_Inizio") = Costo_Inizio
            End If

            DR.Item("Impresa") = DT.Rows(i).Item("Impresa")
            DR.Item("Targa") = DT.Rows(i).Item("Targa")

            Dt_Macchine.Rows.Add(DR)
        Next

        GridView_Macchine.DataSource = Dt_Macchine
        GridView_Macchine.DataBind()

        ''cambio il colore
        For i = 0 To GridView_Macchine.Rows.Count - 1
            If CInt(Me.GridView_Macchine.Rows(i).Cells(2).Text) = 0 Then
                Me.GridView_Macchine.Rows(i).BackColor = System.Drawing.Color.PaleGreen
            ElseIf CInt(Me.GridView_Macchine.Rows(i).Cells(2).Text) = -1 Then
                Me.GridView_Macchine.Rows(i).BackColor = System.Drawing.Color.Khaki
            End If
        Next


    End Sub

    Private Sub CaricaListaPersone()

        Dim DT As DataTable

        Dim i As Integer
        Dim Piva As String = ""
        Dim PivaSingola As Boolean = True
        Dim MostraPubblici As Boolean

        Dim xFiltroAggiuntivo As String = " ( Rapporti_Contabili.Legale = 1 " &
                                          " OR Rapporti_Contabili.Dipendente = 1 " &
                                          " OR Rapporti_Contabili.terzista = 1 ) "
        Dim xOrderBy As String = " Contatti.sa_cod desc,  Contatti.Rag_Soc, cognome, nome ASC "


        For i = 0 To Me.GridView_Operazioni.Rows.Count - 1
            If i = 0 Then
                Piva = GridView_Operazioni.Rows(i).Cells(3).Text
            Else
                If Piva <> GridView_Operazioni.Rows(i).Cells(3).Text Then
                    PivaSingola = False
                    Piva = ""
                    Exit For
                End If
            End If
        Next

        Select Case PivaSingola
            Case True
                'carico le macchine aziendali + quelle pubbliche
                If chk_solo_privati_contatti.Checked Then
                    MostraPubblici = False
                Else
                    MostraPubblici = True
                End If
            Case Else
                MostraPubblici = True
        End Select

        Dim objRappContabili As New AgronicaCoreAnagrafeDAL.Rapporti_Contabili_R
        DT = objRappContabili.RapportiContabilixCostiAccessori(Piva, False, False, enumSelezioneVariabile.Selezione_TabellaCompleta, xFiltroAggiuntivo, xOrderBy, objParametri_Server, isModificaMultipla:=True, FlagPubblico:=MostraPubblici)


        Dim Dt_Contatti As New DataTable
        ''----- Definisco la struttura del DataTable
        ''1
        Dt_Contatti.Columns.Add(New DataColumn("Piva", GetType(String)))
        ''2
        Dt_Contatti.Columns.Add(New DataColumn("sa_cod", GetType(String)))
        ''3
        Dt_Contatti.Columns.Add(New DataColumn("Cod_Contatto", GetType(String)))
        ''4
        Dt_Contatti.Columns.Add(New DataColumn("Rag_Soc_Nome_Cognome", GetType(String)))
        ''5
        Dt_Contatti.Columns.Add(New DataColumn("Cod_Rapporto", GetType(String)))
        ''6
        Dt_Contatti.Columns.Add(New DataColumn("Rapporto_Des", GetType(String)))
        ''7
        Dt_Contatti.Columns.Add(New DataColumn("Prezzo_Unitario", GetType(String)))
        ''8
        Dt_Contatti.Columns.Add(New DataColumn("Unita_Misura", GetType(String)))
        ''9
        Dt_Contatti.Columns.Add(New DataColumn("Mezzo", GetType(String)))
        ''10
        Dt_Contatti.Columns.Add(New DataColumn("Cod_RisUm", GetType(String)))
        ''11
        Dt_Contatti.Columns.Add(New DataColumn("Impresa", GetType(String)))
        ''12
        Dt_Contatti.Columns.Add(New DataColumn("Dipendente", GetType(String)))
        ''13
        Dt_Contatti.Columns.Add(New DataColumn("Terzista", GetType(String)))

        Dt_Contatti.Columns.Add(New DataColumn("Costo_Inizio", GetType(String)))
        '12
        Dt_Contatti.Columns.Add(New DataColumn("Costo_Fine", GetType(String)))



        Dim DR As DataRow
        Dim Costo_Inizio, Costo_Fine As String

        For i = 0 To DT.Rows.Count - 1
            DR = Dt_Contatti.NewRow()
            DR.Item("Piva") = DT.Rows(i).Item("Piva")
            DR.Item("sa_cod") = DT.Rows(i).Item("sa_cod")
            DR.Item("Cod_Contatto") = DT.Rows(i).Item("Cod_Contatto")
            DR.Item("Rag_Soc_Nome_Cognome") = DT.Rows(i).Item("Rag_Soc_Nome_Cognome")
            DR.Item("Rapporto_Des") = DT.Rows(i).Item("Rapporto_Des")

            DR.Item("Cod_RisUm") = DT.Rows(i).Item("Cod_RisUm")
            DR.Item("Impresa") = DT.Rows(i).Item("Impresa")
            DR.Item("Dipendente") = DT.Rows(i).Item("Dipendente")
            DR.Item("Terzista") = DT.Rows(i).Item("Terzista")
            DR.Item("Cod_Rapporto") = DT.Rows(i).Item("Cod_Rapporto")

            If DT.Rows(i).Item("Mezzo") = 1 Then
                DR.Item("Unita_Misura") = "Ha"
            ElseIf DT.Rows(i).Item("Mezzo") = 2 Then
                DR.Item("Unita_Misura") = "Ora"
            End If
            DR.Item("Mezzo") = DT.Rows(i).Item("Mezzo")

            If DT.Rows(i).Item("Elem_Cod") = "0" Then
                DR.Item("Prezzo_Unitario") = DT.Rows(i).Item("Prezzo_Unitario")

                Costo_Inizio = DT.Rows(i).Item("Costo_Inizio")
                If Costo_Inizio.Length > 0 Then
                    Costo_Inizio = Costo_Inizio.Substring(0, 10)
                    If Costo_Inizio = "01/01/1900" Then
                        Costo_Inizio = "---"
                    End If
                Else
                    Costo_Inizio = "---"
                End If
                DR.Item("Costo_Inizio") = Costo_Inizio

                Costo_Fine = DT.Rows(i).Item("Costo_Fine")
                If Costo_Fine.Length > 0 Then
                    Costo_Fine = Costo_Fine.Substring(0, 10)
                    If Costo_Fine = "31/12/2100" Then
                        Costo_Fine = "---"
                    End If
                Else
                    Costo_Fine = "---"
                End If
                DR.Item("Costo_Fine") = Costo_Fine
            End If
            Dt_Contatti.Rows.Add(DR)
        Next

        GridView_Contatti.DataSource = Dt_Contatti
        GridView_Contatti.DataBind()


        ''cambio il colore
        For i = 0 To GridView_Contatti.Rows.Count - 1
            If CInt(Me.GridView_Contatti.Rows(i).Cells(2).Text) = 0 Then
                Me.GridView_Contatti.Rows(i).BackColor = System.Drawing.Color.PaleGreen
            ElseIf CInt(Me.GridView_Contatti.Rows(i).Cells(2).Text) = -1 Then
                Me.GridView_Contatti.Rows(i).BackColor = System.Drawing.Color.Khaki
            End If
        Next

    End Sub

    Private Sub Carica_Operazioni(ByVal StringaXmlOperazioni As String)


        Dim XmlDoc As New System.Xml.XmlDocument
        Dim Xml_FiltroRisultati As System.Xml.XmlElement
        Dim XMLs_Risultato As System.Xml.XmlNodeList
        Dim Xml_Risultato As System.Xml.XmlElement

        Dim Dt As New DataTable
        Dim Dr As DataRow

        Dim i As Integer

        Dim HashPive As New Hashtable

        '----- Definisco la struttura del DataTable

        Dt.Columns.Add(New DataColumn("id_agenda", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("lav_cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("data_movimento", GetType(String)))
        Dt.Columns.Add(New DataColumn("des_lib", GetType(String)))

        Dt.Columns.Add(New DataColumn("Piva", GetType(String)))
        Dt.Columns.Add(New DataColumn("sa_cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("rag_soc", GetType(String)))

        If StringaXmlOperazioni <> "" Then

            XmlDoc.LoadXml(StringaXmlOperazioni)

            Xml_FiltroRisultati = XmlDoc.SelectSingleNode("FiltroAgenda")

            XMLs_Risultato = Xml_FiltroRisultati.GetElementsByTagName("VariabiliStampe")

            ' Dim Presente As Boolean
            For i = 0 To XMLs_Risultato.Count - 1

                Xml_Risultato = XMLs_Risultato.Item(i)

                'Creo una nuova riga
                Dr = Dt.NewRow

                'Definisco i valori
                Dr.Item("id_agenda") = Xml_Risultato.GetAttribute("id_agenda")
                Dr.Item("data_movimento") = Xml_Risultato.GetAttribute("data_movimento")
                Dr.Item("des_lib") = Xml_Risultato.GetAttribute("des_lib")

                Dr.Item("rag_soc") = Xml_Risultato.GetAttribute("rag_soc")
                Dr.Item("lav_cod") = Xml_Risultato.GetAttribute("lav_cod")

                Dr.Item("piva") = Xml_Risultato.GetAttribute("piva")
                Dr.Item("sa_cod") = Xml_Risultato.GetAttribute("sa_cod")

                If Not HashPive.ContainsKey(Dr.Item("piva")) Then
                    HashPive.Add(Dr.Item("piva"), "")
                End If

                'Associo alla tabella la nuova riga creata
                Dt.Rows.Add(Dr)

            Next

        End If

        GridView_Operazioni.DataSource = Dt
        GridView_Operazioni.DataBind()

        'di default seleziono tutti gli impianti..
        For i = 0 To Me.GridView_Operazioni.Rows.Count - 1
            CType(GridView_Operazioni.Rows(i).FindControl("ChkSelezionaOperazione"), CheckBox).Checked = True
        Next

    End Sub

    Protected Sub Btn_Salva_Click(sender As Object, e As EventArgs) Handles Btn_Salva.Click

        Dim i As Integer
        Dim Lav_Cod As Integer
        Dim Id_Agenda As Integer
        Dim Piva As String
        Dim Sa_Cod As String
        Dim Data As String

        'verifico di aver selezionato almeno un'operazione
        Dim AlmenoUna As Boolean = False
        For i = 0 To GridView_Operazioni.Rows.Count - 1
            If CType(GridView_Operazioni.Rows(i).FindControl("ChkSelezionaOperazione"), CheckBox).Checked = True Then
                AlmenoUna = True
                Exit For
            End If
        Next
        If Not AlmenoUna Then
            Messaggi.AgroMsgBox("Selezionare almeno un\'Operazione!", Page, , CType(Ricerca.FindControlIterative(Page.Master, "updateScript"), UpdatePanel))
            Exit Sub
        End If

        Select Case Combo_Modifiche.SelectedValue

            Case enum_ModificaMultiplaOperazioni.AggiungiMacchina
                'verifico di aver selezionato almeno una macchina
                AlmenoUna = False
                For i = 0 To GridView_Macchine.Rows.Count - 1
                    If CType(GridView_Macchine.Rows(i).FindControl("ChkSelezionaMacchina"), CheckBox).Checked = True Then
                        AlmenoUna = True
                        Exit For
                    End If
                Next
                If AlmenoUna = False Then
                    Messaggi.AgroMsgBox("Selezionare almeno una Macchina!", Page, , CType(Ricerca.FindControlIterative(Page.Master, "updateScript"), UpdatePanel))
                    Exit Sub
                End If

                Dim ErrMsg As String
                Dim ErrMsgTot As String = ""
                Dim ha_utilizzo As Decimal

                Dim N_Modificate As Integer = 0
                Dim EliminaCostiPrecedenti As Boolean = False
                If chk_elimina_precedenti_macchine.Checked Then
                    EliminaCostiPrecedenti = True
                End If

                For i = 0 To GridView_Operazioni.Rows.Count - 1

                    If CType(GridView_Operazioni.Rows(i).FindControl("ChkSelezionaOperazione"), CheckBox).Checked = True Then

                        Id_Agenda = GridView_Operazioni.Rows(i).Cells(1).Text
                        Lav_Cod = GridView_Operazioni.Rows(i).Cells(2).Text
                        Piva = GridView_Operazioni.Rows(i).Cells(3).Text
                        Sa_Cod = GridView_Operazioni.Rows(i).Cells(4).Text
                        Data = CDate(GridView_Operazioni.Rows(i).Cells(5).Text)
                        ha_utilizzo = 0
                        If IsNumeric(CType(GridView_Operazioni.Rows(i).FindControl("ha_utilizzo"), TextBox).Text) Then
                            ha_utilizzo = CDec(CType(GridView_Operazioni.Rows(i).FindControl("ha_utilizzo"), TextBox).Text)
                        End If

                        ErrMsg = ""

                        If AggiungiMacchine(Id_Agenda, Piva, Sa_Cod, Data, ha_utilizzo, EliminaCostiPrecedenti, ErrMsg) = False Then
                            ErrMsgTot &= ErrMsg & "<br>"
                        Else
                            N_Modificate += 1
                        End If

                    End If

                Next

                Dim MessaggioFinale As String = "Modificate " & N_Modificate & " operazioni!"
                If ErrMsgTot <> "" Then
                    MessaggioFinale = "<br><br>Operazioni NON modificate :<br>" & ErrMsgTot
                End If
                Messaggi.AgroMsgBox(MessaggioFinale, Page, ,
                          CType(Ricerca.FindControlIterative(Page.Master, "updateScript"), UpdatePanel))


            Case enum_ModificaMultiplaOperazioni.EliminaMacchine

                Dim ErrMsg As String
                Dim ErrMsgTot As String = ""

                Dim N_Modificate As Integer = 0

                For i = 0 To GridView_Operazioni.Rows.Count - 1

                    If CType(GridView_Operazioni.Rows(i).FindControl("ChkSelezionaOperazione"), CheckBox).Checked = True Then

                        Id_Agenda = GridView_Operazioni.Rows(i).Cells(1).Text
                        Lav_Cod = GridView_Operazioni.Rows(i).Cells(2).Text
                        Piva = GridView_Operazioni.Rows(i).Cells(3).Text
                        Sa_Cod = GridView_Operazioni.Rows(i).Cells(4).Text
                        Data = CDate(GridView_Operazioni.Rows(i).Cells(5).Text)

                        ErrMsg = ""

                        If EliminaMacchine(Id_Agenda, Piva, ErrMsg) = False Then
                            ErrMsgTot &= ErrMsg & "<br>"
                        Else
                            N_Modificate += 1
                        End If

                    End If

                Next

                Dim MessaggioFinale As String = "Modificate " & N_Modificate & " operazioni!"
                If ErrMsgTot <> "" Then
                    MessaggioFinale = "<br><br>Operazioni NON modificate :<br>" & ErrMsgTot
                End If
                Messaggi.AgroMsgBox(MessaggioFinale, Page, ,
                          CType(Ricerca.FindControlIterative(Page.Master, "updateScript"), UpdatePanel))



            Case enum_ModificaMultiplaOperazioni.AggiungiContatto
                'verifico di aver selezionato almeno un contatto
                AlmenoUna = False
                For i = 0 To GridView_Contatti.Rows.Count - 1
                    If CType(GridView_Contatti.Rows(i).FindControl("ChkSelezionaContatto"), CheckBox).Checked = True Then
                        AlmenoUna = True
                        Exit For
                    End If
                Next
                If Not AlmenoUna Then
                    Messaggi.AgroMsgBox("Selezionare almeno un Contatto!", Page, , CType(Ricerca.FindControlIterative(Page.Master, "updateScript"), UpdatePanel))
                    Exit Sub
                End If

                Dim ErrMsg As String
                Dim ErrMsgTot As String
                Dim ha_utilizzo As Decimal

                Dim N_Modificate As Integer = 0
                Dim EliminaCostiPrecedenti As Boolean = False
                If chk_elimina_precedenti_contatti.Checked = True Then
                    EliminaCostiPrecedenti = True
                End If

                For i = 0 To GridView_Operazioni.Rows.Count - 1

                    If CType(GridView_Operazioni.Rows(i).FindControl("ChkSelezionaOperazione"), CheckBox).Checked = True Then

                        Id_Agenda = GridView_Operazioni.Rows(i).Cells(1).Text
                        Lav_Cod = GridView_Operazioni.Rows(i).Cells(2).Text
                        Piva = GridView_Operazioni.Rows(i).Cells(3).Text
                        Sa_Cod = GridView_Operazioni.Rows(i).Cells(4).Text
                        Data = CDate(GridView_Operazioni.Rows(i).Cells(5).Text)
                        ha_utilizzo = 0
                        If IsNumeric(CType(GridView_Operazioni.Rows(i).FindControl("ha_utilizzo"), TextBox).Text) Then
                            ha_utilizzo = CDec(CType(GridView_Operazioni.Rows(i).FindControl("ha_utilizzo"), TextBox).Text)
                        End If

                        ErrMsg = ""

                        If AggiungiContatti(Id_Agenda, Piva, Sa_Cod, Data, ha_utilizzo, EliminaCostiPrecedenti, ErrMsg) = False Then
                            ErrMsgTot &= ErrMsg & "<br>"
                        Else
                            N_Modificate += 1
                        End If

                    End If

                Next

                Dim MessaggioFinale As String = "Modificate " & N_Modificate & " operazioni!"
                If ErrMsgTot <> "" Then
                    MessaggioFinale = "<br><br>Operazioni NON modificate :<br>" & ErrMsgTot
                End If
                Messaggi.AgroMsgBox(MessaggioFinale, Page, , _
                          CType(Ricerca.FindControlIterative(Page.Master, "updateScript"), UpdatePanel))


            Case enum_ModificaMultiplaOperazioni.EliminaContatti

                Dim ErrMsg As String
                Dim ErrMsgTot As String = ""

                Dim N_Modificate As Integer = 0

                For i = 0 To GridView_Operazioni.Rows.Count - 1

                    If CType(GridView_Operazioni.Rows(i).FindControl("ChkSelezionaOperazione"), CheckBox).Checked = True Then

                        Id_Agenda = GridView_Operazioni.Rows(i).Cells(1).Text
                        Lav_Cod = GridView_Operazioni.Rows(i).Cells(2).Text
                        Piva = GridView_Operazioni.Rows(i).Cells(3).Text
                        Sa_Cod = GridView_Operazioni.Rows(i).Cells(4).Text
                        Data = CDate(GridView_Operazioni.Rows(i).Cells(5).Text)

                        ErrMsg = ""

                        If EliminaContatti(Id_Agenda, Piva, Sa_Cod, ErrMsg) = False Then
                            ErrMsgTot &= ErrMsg & "<br>"
                        Else
                            N_Modificate += 1
                        End If

                    End If

                Next

                Dim MessaggioFinale As String = "Modificate " & N_Modificate & " operazioni!"
                If ErrMsgTot <> "" Then
                    MessaggioFinale = "<br><br>Operazioni NON modificate :<br>" & ErrMsgTot
                End If
                Messaggi.AgroMsgBox(MessaggioFinale, Page, ,
                          CType(Ricerca.FindControlIterative(Page.Master, "updateScript"), UpdatePanel))

            Case enum_ModificaMultiplaOperazioni.AggiungiMagazzino

                'verifico di aver selezionato un magazzino
                Dim FabbricatoCodice As String
                If ComboMagazzini.ddl_Magazzini.SelectedValue <> "0" AndAlso _
                   ComboMagazzini.ddl_Magazzini.SelectedValue <> "" AndAlso _
                   Split(ComboMagazzini.ddl_Magazzini.SelectedValue, "|").Count = 3 Then
                    FabbricatoCodice = ComboMagazzini.ddl_Magazzini.SelectedValue
                Else
                    Messaggi.AgroMsgBox("Selezionare un Magazzino!", Page, , CType(Ricerca.FindControlIterative(Page.Master, "updateScript"), UpdatePanel))
                    Exit Sub
                End If


                'leggo IMPOSTAZIONE UTENTE BLOCCA_SE_SUPERA_GIACENZE
                Dim ObjUtenti As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
                Dim Impostazione_Valore_1 As String
                Dim BLOCCA_SE_SUPERA_GIACENZE As Boolean = False
                Impostazione_Valore_1 = ObjUtenti.ImpostazioneValore1_from_ImpostazioneCod(enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_SE_SUPERA_GIACENZE, _
                                                  objParametri_Utenti, 1)

                Select Case Impostazione_Valore_1
                    Case "1"
                        BLOCCA_SE_SUPERA_GIACENZE = True
                End Select


                Dim ErrMsg As String
                Dim ErrMsgTot As String = ""

                Dim N_Modificate As Integer = 0

                For i = 0 To GridView_Operazioni.Rows.Count - 1

                    If CType(GridView_Operazioni.Rows(i).FindControl("ChkSelezionaOperazione"), CheckBox).Checked = True Then

                        Id_Agenda = GridView_Operazioni.Rows(i).Cells(1).Text
                        Lav_Cod = GridView_Operazioni.Rows(i).Cells(2).Text
                        Piva = GridView_Operazioni.Rows(i).Cells(3).Text

                        ErrMsg = ""

                        Select Case Lav_Cod
                            Case LAVCOD_TRATTAMENTO_ANTIPARASSITARIO, _
                                LAVCOD_CONCIA_SEME, _
                                LAVCOD_DISERBO, _
                                LAVCOD_TRATTAMENTO_FITOREGOLATORE, _
                                LAVCOD_DISTRIBUZIONE_INSETTI, _
                                LAVCOD_CONFUSIONE_SESSUALE, _
                                LAVCOD_DISORIENTAMENTO_SESSUALE, _
                                LAVCOD_CATTURE_MASSA, _
                                LAVCOD_GEODISINFESTAZIONE, _
                                LAVCOD_DISSECCAMENTO, _
                                LAVCOD_TRATTAMENTO_POST_RACCOLTA, _
                                LAVCOD_INSTALLAZIONE_TRAPPOLE, _
                                LAVCOD_REINNESCO_TRAPPOLE, _
                                LAVCOD_FERTIRRIGAZIONE, LAVCOD_DISTRIBUZIONE_CONCIME, LAVCOD_CONCIMAZIONE_FOGLIARE, LAVCOD_DISTRIBUZIONE_AMMENDANTI, LAVCOD_SARCHIATURA_CONCIMAZIONE, LAVCOD_TRATTAMENTO_ANTIBUTTERATURA

                                If AggiungiMagazzino(BLOCCA_SE_SUPERA_GIACENZE, Id_Agenda, Piva, FabbricatoCodice, ErrMsg) = False Then
                                    ErrMsgTot &= ErrMsg & "<br>"
                                Else
                                    N_Modificate += 1
                                End If

                        End Select

                    End If

                Next

                Dim MessaggioFinale As String = "Modificate " & N_Modificate & " operazioni!"
                If ErrMsgTot <> "" Then
                    MessaggioFinale = "<br><br>Operazioni NON modificate :<br>" & ErrMsgTot
                End If
                Messaggi.AgroMsgBox(MessaggioFinale, Page, , _
                          CType(Ricerca.FindControlIterative(Page.Master, "updateScript"), UpdatePanel))


            'Case enum_ModificaMultiplaOperazioni.EliminaImpiantiDoppiMantenendoQtaProdotto

            '    'lettura SIGPA_2015_05_AgendaDestinazioniErronee_CatVegSup
            '    Dim objSigpa As New AgronicaCoreContabDAL.SIGPA_R
            '    Dim DtOperazioni As DataTable
            '    DtOperazioni = objSigpa.SIGPA_2015_05_AgendaDestinazioniErronee_CatVegSup("", objParametri_Server)

            '    For i = 0 To DtOperazioni.Rows.Count - 1

            '        Id_Agenda = DtOperazioni.Rows(i).Item("id_agenda")
            '        Lav_Cod = DtOperazioni.Rows(i).Item("lav_cod")

            '        EliminaImpiantiDoppiMantenendoQtaProdotto(Id_Agenda)

            '    Next

            '    Messaggi.AgroMsgBuonFine("Modificate " & i & " operazioni!", Page, , _
            '      CType(Ricerca.FindControlIterative(Page.Master, "updateScript"), UpdatePanel))

            Case enum_ModificaMultiplaOperazioni.ModificaxModificaSupImpMantenendoQtaTotaleProdotto

                For i = 0 To GridView_Operazioni.Rows.Count - 1

                    If CType(GridView_Operazioni.Rows(i).FindControl("ChkSelezionaOperazione"), CheckBox).Checked = True Then

                        Id_Agenda = GridView_Operazioni.Rows(i).Cells(1).Text
                        Piva = GridView_Operazioni.Rows(i).Cells(3).Text
                        Lav_Cod = GridView_Operazioni.Rows(i).Cells(2).Text

                        ModificaxModificaSupImpMantenendoQtaProdotto(Id_Agenda, Piva)

                    End If

                Next

                Messaggi.AgroMsgBuonFine("Modificate " & i & " operazioni!", Page, , _
                        CType(Ricerca.FindControlIterative(Page.Master, "updateScript"), UpdatePanel))

            Case enum_ModificaMultiplaOperazioni.ModificaxModificaSupImpMantenendoQtaHaProdotto

                For i = 0 To GridView_Operazioni.Rows.Count - 1

                    If CType(GridView_Operazioni.Rows(i).FindControl("ChkSelezionaOperazione"), CheckBox).Checked = True Then

                        Id_Agenda = GridView_Operazioni.Rows(i).Cells(1).Text
                        Piva = GridView_Operazioni.Rows(i).Cells(3).Text
                        Lav_Cod = GridView_Operazioni.Rows(i).Cells(2).Text

                        ModificaxModificaSupImpMantenendoQtaHaProdotto(Id_Agenda, Piva)

                    End If

                Next

                Messaggi.AgroMsgBuonFine("Modificate " & i & " operazioni!", Page, , _
                        CType(Ricerca.FindControlIterative(Page.Master, "updateScript"), UpdatePanel))

        End Select




    End Sub

    'Private Sub EliminaImpiantiDoppiMantenendoQtaProdotto(ByVal Id_Agenda As Integer)

    '    Dim Fr_Cod As Integer = 0
    '    Dim Udm_Cod As Integer = 0
    '    Dim Udm_Cod_Trasformato As Integer = 0
    '    Dim Dose_Old As Decimal = 0
    '    Dim Dose_Tot_Old As Decimal = 0

    '    Dim Dose_New As Decimal = 0
    '    Dim Sup_Tot_Old As Decimal = 0
    '    Dim Sup_Tot_New As Decimal = 0
    '    Dim Dose_Trasformata_New As Decimal = 0

    '    Dim Id_Mov_Det_DaModificare As Integer = 0

    '    Dim Prov, Com, Sezione, Subalterno As String
    '    Dim Foglio, Numero As Integer
    '    Dim Area As Decimal
    '    Dim Fascicolo As String
    '    Dim SpecieCod As String


    '    Dim DrPart() As DataRow

    '    Dim Hash_DaEliminare As New Hashtable

    '    Dim strFiltroImp As String = ""

    '    Dim objAgenda As New Agenda_Operazione_Helper

    '    Dim Agenda As New Operazione_Agenda

    '    Agenda = objAgenda.Leggi("", 0, _
    '                                 CInt(Id_Agenda), _
    '                                 0, _
    '                                 objParametri_Server)

    '    objAgenda = Nothing

    '    If Not IsNothing(Agenda) Then

    '        'MOVIMENTI
    '        If Not IsNothing(Agenda.Movimenti) Then

    '            objParametriAgenda.Impianti = New List(Of AgronicaCoreModello.ParametriAgenda_Temp.Impianto)

    '            For i = 0 To Agenda.Movimenti.Count - 1

    '                Select Case Agenda.Movimenti(i).Cau_Mov

    '                    Case CAU_TRATTAMENTO, CAU_LAVORAZIONE

    '                        'MOVIMENTI_DETTAGLI
    '                        If Not IsNothing(Agenda.Movimenti(i).Movimenti_Dettagli) Then

    '                            For j = 0 To Agenda.Movimenti(i).Movimenti_Dettagli.Count - 1

    '                                Fr_Cod = Agenda.Movimenti(i).Movimenti_Dettagli(j).Pro_Cod
    '                                Udm_Cod = Agenda.Movimenti(i).Movimenti_Dettagli(j).Extra_Int
    '                                Udm_Cod_Trasformato = Agenda.Movimenti(i).Movimenti_Dettagli(j).Udm_Cod

    '                                Dose_Old = Agenda.Movimenti(i).Movimenti_Dettagli(j).Qta
    '                                Id_Mov_Det_DaModificare = Agenda.Movimenti(i).Movimenti_Dettagli(j).Id_Mov_Det

    '                                Dim objAppezzamento As New AgronicaCoreModello.ParametriAgenda_Temp.Impianto

    '                                'MOVIMENTI_DESTINAZIONI
    '                                If Not IsNothing(Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni) Then

    '                                    If objParametriAgenda.Impianti.Count = 0 Then

    '                                        For x = 0 To Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni.Count - 1

    '                                            objAppezzamento.Piva = Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Piva
    '                                            objAppezzamento.Sa_Cod = Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Sa_Cod
    '                                            objAppezzamento.Appezza = Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Appezza
    '                                            objAppezzamento.ID_Reg = Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Id_Destinazione

    '                                            objAppezzamento.Qta2 = Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Qta2

    '                                            If j = 0 Then

    '                                                Sup_Tot_Old += Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Qta2

    '                                                strFiltroImp &= " (Reg_Impianti.Piva='" & objAppezzamento.Piva & "' " & _
    '                                                                " AND Reg_Impianti.Sa_Cod=" & objAppezzamento.Sa_Cod & " " & _
    '                                                                " AND Reg_Impianti.appezza=" & objAppezzamento.Appezza & " " & _
    '                                                                " AND Reg_Impianti.id_reg=" & objAppezzamento.ID_Reg & " " & _
    '                                                                " ) OR "
    '                                            End If

    '                                        Next

    '                                    End If

    '                                End If

    '                                'verifico i doppioni solo per il primo dettaglio (prodotto)
    '                                If strFiltroImp <> "" And j = 0 Then

    '                                    Dim objSigpa As New AgronicaCoreContabDAL.SIGPA_R
    '                                    Dim DtImp As New DataTable
    '                                    DtImp = objSigpa.ElencaImpianti_DatixSipga(" (" & Left(strFiltroImp, strFiltroImp.Length - 3) & ") ", objParametri_Server)

    '                                    'cerco i doppi 
    '                                    'stessa particellae stessa area intersezione, stesso fascicolo, stessa specie agea
    '                                    For p = 0 To DtImp.Rows.Count - 1

    '                                        Prov = DtImp.Rows(p).Item("prov")
    '                                        Com = DtImp.Rows(p).Item("com")
    '                                        Sezione = DtImp.Rows(p).Item("sezione")
    '                                        Foglio = DtImp.Rows(p).Item("foglio")
    '                                        Numero = DtImp.Rows(p).Item("numero")
    '                                        Subalterno = DtImp.Rows(p).Item("subalterno")
    '                                        Area = DtImp.Rows(p).Item("area")
    '                                        Fascicolo = DtImp.Rows(p).Item("fascicolo")
    '                                        SpecieCod = DtImp.Rows(p).Item("specie_agea")

    '                                        DrPart = DtImp.Select("prov='" & Prov & "' and com='" & Com & "' and sezione='" & Sezione & "' and foglio=" & Foglio & " and numero=" & Numero & " and subalterno='" & Subalterno & "' and fascicolo='" & Fascicolo & "' and specie_agea='" & SpecieCod & "' and area='" & Area.ToString & "'")

    '                                        If Not DrPart Is Nothing AndAlso DrPart.Length > 1 Then
    '                                            'doppio --> tengo la prima
    '                                            For d = 1 To DrPart.Length - 1
    '                                                If Not Hash_DaEliminare.ContainsKey(DrPart(d).Item("Piva") & "_" & DrPart(d).Item("sa_cod") & "_" & DrPart(d).Item("appezza") & "_" & DrPart(d).Item("Id_Reg")) Then
    '                                                    Hash_DaEliminare.Add(DrPart(d).Item("Piva") & "_" & DrPart(d).Item("sa_cod") & "_" & DrPart(d).Item("appezza") & "_" & DrPart(d).Item("Id_Reg"), "")
    '                                                End If
    '                                            Next
    '                                        End If

    '                                    Next

    '                                    Sup_Tot_New = Sup_Tot_Old

    '                                End If

    '                                Dose_Tot_Old = Dose_Old * Sup_Tot_Old


    '                                'ricalcolo la nuova superficie totale
    '                                'solo per il promo dettaglio (prodotto)
    '                                If j = 0 Then
    '                                    If Not IsNothing(Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni) Then
    '                                        For x = 0 To Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni.Count - 1
    '                                            If Not Hash_DaEliminare Is Nothing Then
    '                                                For Each Impianto As DictionaryEntry In Hash_DaEliminare
    '                                                    Dim ArrayImp() As String = Split(Impianto.Key, "_")
    '                                                    If Not ArrayImp Is Nothing AndAlso ArrayImp.Length > 0 Then
    '                                                        If ArrayImp(0) = Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Piva And _
    '                                                            ArrayImp(1) = Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Sa_Cod And _
    '                                                            ArrayImp(2) = Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Appezza And _
    '                                                            ArrayImp(3) = Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Id_Destinazione Then
    '                                                            Sup_Tot_New -= Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Qta2
    '                                                        End If
    '                                                    End If
    '                                                Next
    '                                            End If
    '                                        Next
    '                                    End If
    '                                End If

    '                                Dose_New = Dose_Tot_Old / Sup_Tot_New

    '                                'modifico la qta del dettaglio (dose_ha)
    '                                Dim objDet As New AgronicaCoreContabDAL.Movimenti_Dettagli_W
    '                                objDet.Modifica_Quantita(Agenda.Movimenti(i).Movimenti_Dettagli(j).Piva, _
    '                                                         Agenda.Movimenti(i).Movimenti_Dettagli(j).Id_Agenda, _
    '                                                         Agenda.Movimenti(i).Movimenti_Dettagli(j).Id_Mov, _
    '                                                         Agenda.Movimenti(i).Movimenti_Dettagli(j).Id_Mov_Det, _
    '                                                         0, _
    '                                                         0, _
    '                                                         0, _
    '                                                         0, _
    '                                                         Dose_New, _
    '                                                         0, _
    '                                                         0, _
    '                                                         0, _
    '                                                         "", _
    '                                                         "", _
    '                                                         objParametri_Server)

    '                                Dim objDest As New AgronicaCoreContabDAL.Mov_Destinazioni_W

    '                                'MOVIMENTI_DESTINAZIONI
    '                                If Not IsNothing(Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni) Then

    '                                    For x = 0 To Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni.Count - 1

    '                                        If Not Hash_DaEliminare Is Nothing Then

    '                                            For Each Impianto As DictionaryEntry In Hash_DaEliminare
    '                                                Dim ArrayImp() As String = Split(Impianto.Key, "_")

    '                                                If Not ArrayImp Is Nothing AndAlso ArrayImp.Length > 0 Then
    '                                                    If ArrayImp(0) = Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Piva And _
    '                                                        ArrayImp(1) = Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Sa_Cod And _
    '                                                        ArrayImp(2) = Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Appezza And _
    '                                                        ArrayImp(3) = Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Id_Destinazione Then

    '                                                        'elimino la destinazione
    '                                                        objDest.Cancella(Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Piva, _
    '                                                                                                                               Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Sa_Cod, _
    '                                                                                                                               Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Id_Agenda, _
    '                                                                                                                               Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Id_Mov, _
    '                                                                                                                               Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Id_Mov_Det, _
    '                                                                                                                               Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Appezza, _
    '                                                                                                                               Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Id_Destinazione, _
    '                                                                                                                               "", objParametri_Server)
    '                                                    Else

    '                                                        Select Case Agenda.Movimenti(i).Movimenti_Dettagli(j).Extra_Int
    '                                                            Case 3  'g
    '                                                                Dose_Trasformata_New = Dose_New / 1000  'caso in cui ho i grammi
    '                                                            Case 2032 'mg
    '                                                                Dose_Trasformata_New = Dose_New / 1000000  'caso in cui ho i grammi
    '                                                            Case 4  'q
    '                                                                Dose_Trasformata_New = Dose_New * 100  'caso in cui ho i quintali
    '                                                            Case 304 't
    '                                                                Dose_Trasformata_New = Dose_New * 1000   'caso in cui ho le tonnelate
    '                                                            Case 101 'ml
    '                                                                Dose_Trasformata_New = Dose_New / 1000  'caso in cui ho i ml
    '                                                            Case 104 'cc
    '                                                                Dose_Trasformata_New = Dose_New / 100  'caso in cui ho i cc
    '                                                            Case 19 'Metri cubi
    '                                                                Dose_Trasformata_New = Dose_New * 1000
    '                                                            Case 2, 29 'kg,l
    '                                                                Dose_Trasformata_New = Dose_New

    '                                                        End Select


    '                                                        'ricalcolo la qta_destinazione
    '                                                        objDest.Modifica_Quantita(Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Piva, _
    '                                                                         Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Sa_Cod, _
    '                                                                         Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Id_Agenda, _
    '                                                                         Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Id_Mov, _
    '                                                                         Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Id_Mov_Det, _
    '                                                                         Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Appezza, _
    '                                                                         Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Id_Destinazione, _
    '                                                                         Dose_Trasformata_New * Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Qta2, _
    '                                                                         "", objParametri_Server)
    '                                                    End If
    '                                                End If
    '                                            Next



    '                                        End If
    '                                    Next

    '                                End If


    '                            Next

    '                        End If

    '                End Select

    '            Next

    '        End If

    '    End If

    'End Sub

    Private Sub ModificaxModificaSupImpMantenendoQtaProdotto(ByVal Id_Agenda As Integer, ByVal Piva As String)

        Dim Lav_Cod As Integer = 0
        Dim Fr_Cod As Integer = 0
        Dim Mat_Cod As Integer = 0
        Dim Udm_Cod As Integer = 0
        Dim Udm_Cod_Trasformato As Integer = 0
        Dim Dett_Cod As Integer = 0
        Dim Dose_Old As Decimal = 0
        Dim Dose_Tot_Old As Decimal = 0

        Dim Qta_Dest_New As Decimal = 0
        Dim Dose_New As Decimal = 0
        Dim Dose_New_Hl As Decimal = 0
        Dim Sup_Tot_Old As Decimal = 0
        Dim Sup_Tot_New As Decimal = 0
        Dim Dose_Trasformata_New As Decimal = 0

        Dim AcquaTot As Decimal = 0

        Dim strFiltroImp As String = ""

        Dim objAgenda As New Agenda_Operazione_Helper

        Dim Agenda As New Operazione_Agenda

        Agenda = objAgenda.Leggi(Piva, 0,
                                     CInt(Id_Agenda),
                                     0,
                                     objParametri_Server)

        objAgenda = Nothing

        If Not IsNothing(Agenda) Then

            'MOVIMENTI
            If Not IsNothing(Agenda.Movimenti) Then

                Lav_Cod = Agenda.Lav_Cod

                objParametriAgenda.Impianti = New List(Of AgronicaCoreModello.ParametriAgenda_Temp.Impianto)

                For i = 0 To Agenda.Movimenti.Count - 1

                    Select Case Agenda.Movimenti(i).Cau_Mov

                        Case CAU_TRATTAMENTO, CAU_LAVORAZIONE, CAU_RILIEVO_RACCOLTA


                            'ricavo subito acqua, destinazioni e sup_totale
                            If Sup_Tot_Old = 0 Then

                                Select Case Agenda.Lav_Cod

                                    '1 destinazione per ogni dettaglio
                                    Case LAVCOD_CATTURE_MASSA, LAVCOD_INSTALLAZIONE_TRAPPOLE

                                        If Not IsNothing(Agenda.Movimenti(i).Movimenti_Dettagli) Then

                                            For Each mov_dett As Movimento_Dettaglio In Agenda.Movimenti(i).Movimenti_Dettagli

                                                If Not IsNothing(mov_dett.Movimenti_Destinazioni) Then

                                                    For Each mov_dest As Movimento_Destinazione In mov_dett.Movimenti_Destinazioni

                                                        Dim objAppezzamento As New AgronicaCoreModello.ParametriAgenda_Temp.Impianto

                                                        Sup_Tot_Old += CDec(mov_dest.Qta2)

                                                        objAppezzamento.Piva = mov_dest.Piva
                                                        objAppezzamento.Sa_Cod = mov_dest.Sa_Cod
                                                        objAppezzamento.Appezza = mov_dest.Appezza
                                                        objAppezzamento.ID_Reg = mov_dest.Id_Destinazione
                                                        objAppezzamento.Qta2 = mov_dest.Qta2

                                                        objParametriAgenda.Impianti.Add(objAppezzamento)

                                                        strFiltroImp &= " (Reg_Impianti.Piva='" & objAppezzamento.Piva & "' " &
                                                                    " AND Reg_Impianti.Sa_Cod=" & objAppezzamento.Sa_Cod & " " &
                                                                    " AND Reg_Impianti.appezza=" & objAppezzamento.Appezza & " " &
                                                                    " AND Reg_Impianti.id_reg=" & objAppezzamento.ID_Reg & " " &
                                                                    " ) OR "

                                                    Next


                                                End If

                                            Next

                                            If strFiltroImp <> "" Then

                                                Dim objImp As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
                                                Dim DtImp As New DataTable
                                                DtImp = objImp.Leggi(Piva, 0, 0, 0,
                                                            enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                                            " (" & Left(strFiltroImp, strFiltroImp.Length - 3) & ") ",
                                                            "", objParametri_Server)

                                                For a = 0 To objParametriAgenda.Impianti.Count - 1
                                                    For p = 0 To DtImp.Rows.Count - 1
                                                        If objParametriAgenda.Impianti(a).Piva = DtImp.Rows(p).Item("piva") And
                                                    objParametriAgenda.Impianti(a).Sa_Cod = DtImp.Rows(p).Item("sa_cod") And
                                                    objParametriAgenda.Impianti(a).Appezza = DtImp.Rows(p).Item("appezza") And
                                                    objParametriAgenda.Impianti(a).ID_Reg = DtImp.Rows(p).Item("id_reg") Then

                                                            objParametriAgenda.Impianti(a).Sup_Imp = DtImp.Rows(p).Item("sup_imp")

                                                            Sup_Tot_New += objParametriAgenda.Impianti(a).Sup_Imp

                                                            Exit For
                                                        End If
                                                    Next
                                                Next

                                            End If

                                        End If


                                        'n destinazioni per ogni dettaglio
                                    Case Else

                                        If Not IsNothing(Agenda.Movimenti(i).Movimenti_Dettagli) AndAlso Not IsNothing(Agenda.Movimenti(i).Movimenti_Dettagli(0).Movimenti_Destinazioni) Then

                                            For Each mov_dest As Movimento_Destinazione In Agenda.Movimenti(i).Movimenti_Dettagli(0).Movimenti_Destinazioni

                                                Dim objAppezzamento As New AgronicaCoreModello.ParametriAgenda_Temp.Impianto

                                                Sup_Tot_Old += CDec(mov_dest.Qta2)

                                                objAppezzamento.Piva = mov_dest.Piva
                                                objAppezzamento.Sa_Cod = mov_dest.Sa_Cod
                                                objAppezzamento.Appezza = mov_dest.Appezza
                                                objAppezzamento.ID_Reg = mov_dest.Id_Destinazione
                                                objAppezzamento.Qta2 = mov_dest.Qta2

                                                objParametriAgenda.Impianti.Add(objAppezzamento)

                                                strFiltroImp &= " (Reg_Impianti.Piva='" & objAppezzamento.Piva & "' " &
                                                            " AND Reg_Impianti.Sa_Cod=" & objAppezzamento.Sa_Cod & " " &
                                                            " AND Reg_Impianti.appezza=" & objAppezzamento.Appezza & " " &
                                                            " AND Reg_Impianti.id_reg=" & objAppezzamento.ID_Reg & " " &
                                                            " ) OR "

                                            Next

                                            If strFiltroImp <> "" Then

                                                Dim objImp As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
                                                Dim DtImp As New DataTable
                                                DtImp = objImp.Leggi(Piva, 0, 0, 0,
                                                            enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                                            " (" & Left(strFiltroImp, strFiltroImp.Length - 3) & ") ",
                                                            "", objParametri_Server)

                                                For a = 0 To objParametriAgenda.Impianti.Count - 1
                                                    For p = 0 To DtImp.Rows.Count - 1
                                                        If objParametriAgenda.Impianti(a).Piva = DtImp.Rows(p).Item("piva") And
                                                    objParametriAgenda.Impianti(a).Sa_Cod = DtImp.Rows(p).Item("sa_cod") And
                                                    objParametriAgenda.Impianti(a).Appezza = DtImp.Rows(p).Item("appezza") And
                                                    objParametriAgenda.Impianti(a).ID_Reg = DtImp.Rows(p).Item("id_reg") Then

                                                            objParametriAgenda.Impianti(a).Sup_Imp = DtImp.Rows(p).Item("sup_imp")

                                                            Sup_Tot_New += objParametriAgenda.Impianti(a).Sup_Imp

                                                            Exit For
                                                        End If
                                                    Next
                                                Next

                                            End If

                                        End If

                                End Select

                            End If




                            '---------------------------------------------------
                            '----- Acqua  
                            '---------------------------------------------------

                            For j = 0 To Agenda.Movimenti(i).Movimenti_Dettagli_Tecnici.Count - 1
                                Select Case Agenda.Movimenti(i).Movimenti_Dettagli_Tecnici(j).Qta_Ril
                                    Case Is > 0 'dosaggio totale --> da dividere
                                        AcquaTot = Agenda.Movimenti(i).Movimenti_Dettagli_Tecnici(j).Qta_Ril
                                    Case Is < 0 'dosaggio/ha --> già ok
                                        AcquaTot = Math.Abs(Agenda.Movimenti(i).Movimenti_Dettagli_Tecnici(j).Qta_Ril) * Sup_Tot_Old
                                End Select
                            Next


                            'MOVIMENTI_DETTAGLI
                            If Not IsNothing(Agenda.Movimenti(i).Movimenti_Dettagli) Then

                                For j = 0 To Agenda.Movimenti(i).Movimenti_Dettagli.Count - 1

                                    Fr_Cod = Agenda.Movimenti(i).Movimenti_Dettagli(j).Pro_Cod
                                    Udm_Cod = Agenda.Movimenti(i).Movimenti_Dettagli(j).Extra_Int
                                    Udm_Cod_Trasformato = Agenda.Movimenti(i).Movimenti_Dettagli(j).Udm_Cod

                                    Dose_Old = Agenda.Movimenti(i).Movimenti_Dettagli(j).Qta

                                    'dose e udm dell'irrigazione sono nel mov_dettaglio_tecnico
                                    If Agenda.Lav_Cod = LAVCOD_IRRIGAZIONE Then
                                        If Not IsNothing(Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici) AndAlso Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici.Count > 0 Then
                                            Dose_Old = Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici(0).Qta_Ril
                                            Dett_Cod = Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici(0).dett_cod
                                        End If
                                    End If


                                    ''MOVIMENTI_DESTINAZIONI
                                    'If Not IsNothing(Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni) Then

                                    '    If objParametriAgenda.Impianti.Count = 0 Then

                                    '        For x = 0 To Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni.Count - 1

                                    '            Dim objAppezzamento As New AgronicaCoreModello.ParametriAgenda_Temp.Impianto

                                    '            objAppezzamento.Piva = Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Piva
                                    '            objAppezzamento.Sa_Cod = Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Sa_Cod
                                    '            objAppezzamento.Appezza = Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Appezza
                                    '            objAppezzamento.ID_Reg = Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Id_Destinazione

                                    '            'sup_trattata
                                    '            objAppezzamento.Qta2 = Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Qta2


                                    '            If j = 0 Then

                                    '                'Qta_Tot_Old += Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Qta
                                    '                Sup_Tot_Old += Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Qta2

                                    '                strFiltroImp &= " (Reg_Impianti.Piva='" & objAppezzamento.Piva & "' " &
                                    '                                " AND Reg_Impianti.Sa_Cod=" & objAppezzamento.Sa_Cod & " " &
                                    '                                " AND Reg_Impianti.appezza=" & objAppezzamento.Appezza & " " &
                                    '                                " AND Reg_Impianti.id_reg=" & objAppezzamento.ID_Reg & " " &
                                    '                                " ) OR "

                                    '                objParametriAgenda.Impianti.Add(objAppezzamento)

                                    '            End If

                                    '        Next

                                    '    End If

                                    'End If

                                    ''ricavo le sup_imp
                                    'If strFiltroImp <> "" And j = 0 Then

                                    '    Dim objImp As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
                                    '    Dim DtImp As New DataTable
                                    '    DtImp = objImp.Leggi(piva, 0, 0, 0,
                                    '                        enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                    '                        " (" & Left(strFiltroImp, strFiltroImp.Length - 3) & ") ",
                                    '                        "", objParametri_Server)

                                    '    For a = 0 To objParametriAgenda.Impianti.Count - 1
                                    '        For p = 0 To DtImp.Rows.Count - 1
                                    '            If objParametriAgenda.Impianti(a).Piva = DtImp.Rows(p).Item("piva") And
                                    '                objParametriAgenda.Impianti(a).Sa_Cod = DtImp.Rows(p).Item("sa_cod") And
                                    '                objParametriAgenda.Impianti(a).Appezza = DtImp.Rows(p).Item("appezza") And
                                    '                objParametriAgenda.Impianti(a).ID_Reg = DtImp.Rows(p).Item("id_reg") Then

                                    '                objParametriAgenda.Impianti(a).Sup_Imp = DtImp.Rows(p).Item("sup_imp")

                                    '                Sup_Tot_New += objParametriAgenda.Impianti(a).Sup_Imp

                                    '                Exit For
                                    '            End If
                                    '        Next
                                    '    Next

                                    'End If

                                    Dose_Tot_Old = Dose_Old * Sup_Tot_Old
                                    Dose_New_Hl = 0

                                    Select Case Lav_Cod
                                        Case LAVCOD_DISTRIBUZIONE_INSETTI
                                            Dose_New = Math.Round(Dose_Tot_Old / Sup_Tot_New, 0)
                                        Case LAVCOD_CONFUSIONE_SESSUALE, LAVCOD_DISORIENTAMENTO_SESSUALE,
                                             LAVCOD_SEMINA, LAVCOD_TRAPIANTO, LAVCOD_SOVESCIO, LAVCOD_SOD_SEDDING,
                                             LAVCOD_RACCOLTA,
                                             LAVCOD_INSTALLAZIONE_TRAPPOLE, LAVCOD_CATTURE_MASSA
                                            'qta è quella totale distribuita
                                            Dose_New = Dose_Old
                                        Case LAVCOD_IRRIGAZIONE
                                            Dose_New = Dose_Old
                                        Case Else
                                            Dose_New = Dose_Tot_Old / Sup_Tot_New
                                    End Select

                                    Select Case Lav_Cod

                                        Case LAVCOD_CONFUSIONE_SESSUALE, LAVCOD_DISORIENTAMENTO_SESSUALE,
                                             LAVCOD_SEMINA, LAVCOD_TRAPIANTO, LAVCOD_SOVESCIO, LAVCOD_SOD_SEDDING,
                                             LAVCOD_RACCOLTA,
                                             LAVCOD_INSTALLAZIONE_TRAPPOLE, LAVCOD_CATTURE_MASSA

                                        Case LAVCOD_IRRIGAZIONE

                                        Case Else

                                            'Dose_Tot_New = Dose * SupTrattata_Tot
                                            'Dose_Tot_Old = Dose * Sup_Tot_Old

                                            ''dose ha
                                            'Movimento_Dettaglio.Qta = Dose
                                            If AcquaTot <> 0 Then
                                                Dose_New_Hl = Dose_New * Sup_Tot_New / AcquaTot
                                            End If


                                            'modifico la qta del dettaglio (dose_ha)
                                            Dim objDet As New AgronicaCoreContabDAL.Movimenti_Dettagli_W
                                            objDet.Modifica_Quantita(Agenda.Movimenti(i).Movimenti_Dettagli(j).Piva,
                                                                     Agenda.Movimenti(i).Movimenti_Dettagli(j).Id_Agenda,
                                                                     Agenda.Movimenti(i).Movimenti_Dettagli(j).Id_Mov,
                                                                     Agenda.Movimenti(i).Movimenti_Dettagli(j).Id_Mov_Det,
                                                                     0,
                                                                     0,
                                                                     0,
                                                                     0,
                                                                     Dose_New, Dose_New_Hl, Dose_Tot_Old,
                                                                     0,
                                                                     0,
                                                                     0,
                                                                     "",
                                                                     "",
                                                                     objParametri_Server)
                                    End Select



                                    Dim objDest As New AgronicaCoreContabDAL.Mov_Destinazioni_W

                                    'MOVIMENTI_DESTINAZIONI
                                    If Not IsNothing(Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni) Then

                                        For x = 0 To Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni.Count - 1

                                            For a = 0 To objParametriAgenda.Impianti.Count - 1
                                                If objParametriAgenda.Impianti(a).Piva = Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Piva AndAlso
                                                   objParametriAgenda.Impianti(a).Sa_Cod = Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Sa_Cod AndAlso
                                                   objParametriAgenda.Impianti(a).Appezza = Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Appezza AndAlso
                                                   objParametriAgenda.Impianti(a).ID_Reg = Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Id_Destinazione Then

                                                    Select Case Agenda.Movimenti(i).Movimenti_Dettagli(j).Extra_Int
                                                        Case 3  'g
                                                            Dose_Trasformata_New = Dose_New / 1000  'caso in cui ho i grammi
                                                        Case 2032 'mg
                                                            Dose_Trasformata_New = Dose_New / 1000000  'caso in cui ho i grammi
                                                        Case 4  'q
                                                            Dose_Trasformata_New = Dose_New * 100  'caso in cui ho i quintali
                                                        Case 304 't
                                                            Dose_Trasformata_New = Dose_New * 1000   'caso in cui ho le tonnelate
                                                        Case 101 'ml
                                                            Dose_Trasformata_New = Dose_New / 1000  'caso in cui ho i ml
                                                        Case 104 'cc
                                                            Dose_Trasformata_New = Dose_New / 100  'caso in cui ho i cc
                                                        Case 19 'Metri cubi
                                                            Dose_Trasformata_New = Dose_New * 1000
                                                        Case Else
                                                            Dose_Trasformata_New = Dose_New

                                                    End Select

                                                    Dim QuotaDistribuzioneNew As Decimal = 0
                                                    If Sup_Tot_New <> 0 Then
                                                        QuotaDistribuzioneNew = objParametriAgenda.Impianti(a).Sup_Imp / Sup_Tot_New
                                                    End If

                                                    Select Case Lav_Cod

                                                        Case LAVCOD_INSTALLAZIONE_TRAPPOLE, LAVCOD_CATTURE_MASSA

                                                            'modifico solo sup_trattata
                                                            objDest.Modifica_SupTrattata(Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Piva,
                                                                             Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Sa_Cod,
                                                                             Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Id_Agenda,
                                                                             Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Id_Mov,
                                                                             Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Id_Mov_Det,
                                                                             Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Appezza,
                                                                             Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Id_Destinazione,
                                                                             objParametriAgenda.Impianti(a).Sup_Imp,
                                                                             QuotaDistribuzioneNew,
                                                                             "", objParametri_Server)
                                                        Case Else


                                                            Select Case Lav_Cod
                                                                Case LAVCOD_DISTRIBUZIONE_INSETTI
                                                                    Qta_Dest_New = Math.Round(Dose_New * objParametriAgenda.Impianti(a).Sup_Imp, 0)
                                                                Case LAVCOD_DISORIENTAMENTO_SESSUALE, LAVCOD_CONFUSIONE_SESSUALE,
                                                                     LAVCOD_SEMINA, LAVCOD_TRAPIANTO, LAVCOD_SOVESCIO, LAVCOD_SOD_SEDDING,
                                                                     LAVCOD_RACCOLTA

                                                                    Qta_Dest_New = Math.Round((Dose_New / Sup_Tot_New) * objParametriAgenda.Impianti(a).Sup_Imp, 0)

                                                                Case LAVCOD_IRRIGAZIONE
                                                                    Select Case Dett_Cod
                                                                        Case enum_UnitaMisura.Millimetri
                                                                            Dose_New = Dose_New * 10
                                                                        Case enum_UnitaMisura.METRI3__HA
                                                                            Dose_New = Dose_New * 1
                                                                        Case Else
                                                                            Dose_New = 0
                                                                    End Select
                                                                    Qta_Dest_New = Dose_New * objParametriAgenda.Impianti(a).Sup_Imp
                                                                Case Else
                                                                    Qta_Dest_New = Dose_Trasformata_New * objParametriAgenda.Impianti(a).Sup_Imp
                                                            End Select


                                                            'modifico qta_destinazione e sup_trattata
                                                            objDest.Modifica_Quantita_e_SupTrattata(Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Piva,
                                                                             Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Sa_Cod,
                                                                             Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Id_Agenda,
                                                                             Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Id_Mov,
                                                                             Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Id_Mov_Det,
                                                                             Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Appezza,
                                                                             Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Id_Destinazione,
                                                                             Qta_Dest_New,
                                                                             objParametriAgenda.Impianti(a).Sup_Imp,
                                                                             QuotaDistribuzioneNew,
                                                                             "", objParametri_Server)

                                                    End Select


                                                    Exit For
                                                End If

                                            Next

                                        Next

                                    End If

                                Next

                            End If

                    End Select

                Next

            End If

        End If

        Dim objAgendaW As New AgronicaCoreContabDAL.Agenda_W
        Dim Flag_UpdateFlagOK As Boolean

        Flag_UpdateFlagOK = objAgendaW.Agenda_Sblocca2(Piva, Id_Agenda, objParametri_Server.UtenteUsername, Date.Now, "", objParametri_Server)




    End Sub

    Private Sub ModificaxModificaSupImpMantenendoQtaHaProdotto(ByVal Id_Agenda As Integer, ByVal Piva As String)

        Dim Lav_Cod As Integer = 0
        Dim Fr_Cod As Integer = 0
        Dim Mat_Cod As Integer = 0
        Dim Udm_Cod As Integer = 0
        Dim Dett_Cod As Integer = 0
        Dim Udm_Cod_Trasformato As Integer = 0
        Dim Dose As Decimal = 0
        Dim Dose_New As Decimal = 0
        Dim Dose_New_Hl As Decimal = 0
        Dim Dose_Tot_Old As Decimal = 0
        Dim Dose_Tot_New As Decimal = 0
        Dim Lotto As String = ""
        Dim Cod_Progetto As Integer = 0

        Dim HashTotNew As New Hashtable

        Dim Qta_Dest_New As Decimal = 0
        Dim Sup_Tot_Old As Decimal = 0
        Dim Sup_Tot_New As Decimal = 0
        Dim Dose_Trasformata_New As Decimal = 0
        Dim DoseTotaleTrasformata_New As Decimal = 0

        Dim AcquaTot As Decimal = 0


        Dim strFiltroImp As String = ""

        Dim objAgenda As New Agenda_Operazione_Helper

        Dim Agenda As New Operazione_Agenda

        Agenda = objAgenda.Leggi(Piva, 0, _
                                     CInt(Id_Agenda), _
                                     0, _
                                     objParametri_Server)

        objAgenda = Nothing

        If Not IsNothing(Agenda) Then

            Lav_Cod = Agenda.Lav_Cod

            'MOVIMENTI
            If Not IsNothing(Agenda.Movimenti) Then

                objParametriAgenda.Impianti = New List(Of AgronicaCoreModello.ParametriAgenda_Temp.Impianto)

                'MOVIMENTO LAVORAZIONE
                'modifico le destinazioni --> qta e qta2 
                For i = 0 To Agenda.Movimenti.Count - 1

                    Select Case Agenda.Movimenti(i).Cau_Mov

                        Case CAU_TRATTAMENTO, CAU_LAVORAZIONE, CAU_RILIEVO_RACCOLTA

                            'ricavo subito acqua, destinazioni e sup_totale
                            If Sup_Tot_Old = 0 Then

                                Select Case Agenda.Lav_Cod

                                    '1 destinazione per ogni dettaglio
                                    Case LAVCOD_CATTURE_MASSA, LAVCOD_INSTALLAZIONE_TRAPPOLE

                                        If Not IsNothing(Agenda.Movimenti(i).Movimenti_Dettagli) Then

                                            For Each mov_dett As Movimento_Dettaglio In Agenda.Movimenti(i).Movimenti_Dettagli

                                                If Not IsNothing(mov_dett.Movimenti_Destinazioni) Then

                                                    For Each mov_dest As Movimento_Destinazione In mov_dett.Movimenti_Destinazioni

                                                        Dim objAppezzamento As New AgronicaCoreModello.ParametriAgenda_Temp.Impianto

                                                        Sup_Tot_Old += CDec(mov_dest.Qta2)

                                                        objAppezzamento.Piva = mov_dest.Piva
                                                        objAppezzamento.Sa_Cod = mov_dest.Sa_Cod
                                                        objAppezzamento.Appezza = mov_dest.Appezza
                                                        objAppezzamento.ID_Reg = mov_dest.Id_Destinazione
                                                        objAppezzamento.Qta2 = mov_dest.Qta2

                                                        objParametriAgenda.Impianti.Add(objAppezzamento)

                                                        strFiltroImp &= " (Reg_Impianti.Piva='" & objAppezzamento.Piva & "' " &
                                                                    " AND Reg_Impianti.Sa_Cod=" & objAppezzamento.Sa_Cod & " " &
                                                                    " AND Reg_Impianti.appezza=" & objAppezzamento.Appezza & " " &
                                                                    " AND Reg_Impianti.id_reg=" & objAppezzamento.ID_Reg & " " &
                                                                    " ) OR "

                                                    Next


                                                End If

                                            Next

                                            If strFiltroImp <> "" Then

                                                Dim objImp As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
                                                Dim DtImp As New DataTable
                                                DtImp = objImp.Leggi(Piva, 0, 0, 0,
                                                            enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                                            " (" & Left(strFiltroImp, strFiltroImp.Length - 3) & ") ",
                                                            "", objParametri_Server)

                                                For a = 0 To objParametriAgenda.Impianti.Count - 1
                                                    For p = 0 To DtImp.Rows.Count - 1
                                                        If objParametriAgenda.Impianti(a).Piva = DtImp.Rows(p).Item("piva") And
                                                    objParametriAgenda.Impianti(a).Sa_Cod = DtImp.Rows(p).Item("sa_cod") And
                                                    objParametriAgenda.Impianti(a).Appezza = DtImp.Rows(p).Item("appezza") And
                                                    objParametriAgenda.Impianti(a).ID_Reg = DtImp.Rows(p).Item("id_reg") Then

                                                            objParametriAgenda.Impianti(a).Sup_Imp = DtImp.Rows(p).Item("sup_imp")

                                                            Sup_Tot_New += objParametriAgenda.Impianti(a).Sup_Imp

                                                            Exit For
                                                        End If
                                                    Next
                                                Next

                                            End If

                                        End If


                                        'n destinazioni per ogni dettaglio
                                    Case Else

                                        If Not IsNothing(Agenda.Movimenti(i).Movimenti_Dettagli) AndAlso Not IsNothing(Agenda.Movimenti(i).Movimenti_Dettagli(0).Movimenti_Destinazioni) Then

                                            For Each mov_dest As Movimento_Destinazione In Agenda.Movimenti(i).Movimenti_Dettagli(0).Movimenti_Destinazioni

                                                Dim objAppezzamento As New AgronicaCoreModello.ParametriAgenda_Temp.Impianto

                                                Sup_Tot_Old += CDec(mov_dest.Qta2)

                                                objAppezzamento.Piva = mov_dest.Piva
                                                objAppezzamento.Sa_Cod = mov_dest.Sa_Cod
                                                objAppezzamento.Appezza = mov_dest.Appezza
                                                objAppezzamento.ID_Reg = mov_dest.Id_Destinazione
                                                objAppezzamento.Qta2 = mov_dest.Qta2

                                                objParametriAgenda.Impianti.Add(objAppezzamento)

                                                strFiltroImp &= " (Reg_Impianti.Piva='" & objAppezzamento.Piva & "' " &
                                                            " AND Reg_Impianti.Sa_Cod=" & objAppezzamento.Sa_Cod & " " &
                                                            " AND Reg_Impianti.appezza=" & objAppezzamento.Appezza & " " &
                                                            " AND Reg_Impianti.id_reg=" & objAppezzamento.ID_Reg & " " &
                                                            " ) OR "

                                            Next

                                            If strFiltroImp <> "" Then

                                                Dim objImp As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
                                                Dim DtImp As New DataTable
                                                DtImp = objImp.Leggi(Piva, 0, 0, 0,
                                                            enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                                            " (" & Left(strFiltroImp, strFiltroImp.Length - 3) & ") ",
                                                            "", objParametri_Server)

                                                For a = 0 To objParametriAgenda.Impianti.Count - 1
                                                    For p = 0 To DtImp.Rows.Count - 1
                                                        If objParametriAgenda.Impianti(a).Piva = DtImp.Rows(p).Item("piva") And
                                                    objParametriAgenda.Impianti(a).Sa_Cod = DtImp.Rows(p).Item("sa_cod") And
                                                    objParametriAgenda.Impianti(a).Appezza = DtImp.Rows(p).Item("appezza") And
                                                    objParametriAgenda.Impianti(a).ID_Reg = DtImp.Rows(p).Item("id_reg") Then

                                                            objParametriAgenda.Impianti(a).Sup_Imp = DtImp.Rows(p).Item("sup_imp")

                                                            Sup_Tot_New += objParametriAgenda.Impianti(a).Sup_Imp

                                                            Exit For
                                                        End If
                                                    Next
                                                Next

                                            End If

                                        End If

                                End Select

                            End If



                            '---------------------------------------------------
                            '----- Acqua  
                            '---------------------------------------------------

                            For j = 0 To Agenda.Movimenti(i).Movimenti_Dettagli_Tecnici.Count - 1
                                Select Case Agenda.Movimenti(i).Movimenti_Dettagli_Tecnici(j).Qta_Ril
                                    Case Is > 0 'dosaggio totale --> da dividere
                                        AcquaTot = Agenda.Movimenti(i).Movimenti_Dettagli_Tecnici(j).Qta_Ril
                                    Case Is < 0 'dosaggio/ha --> già ok
                                        AcquaTot = Math.Abs(Agenda.Movimenti(i).Movimenti_Dettagli_Tecnici(j).Qta_Ril) * Sup_Tot_Old
                                End Select
                            Next




                            'MOVIMENTI_DETTAGLI
                            If Not IsNothing(Agenda.Movimenti(i).Movimenti_Dettagli) Then

                                For j = 0 To Agenda.Movimenti(i).Movimenti_Dettagli.Count - 1

                                    Fr_Cod = Agenda.Movimenti(i).Movimenti_Dettagli(j).Pro_Cod
                                    Mat_Cod = Agenda.Movimenti(i).Movimenti_Dettagli(j).Mat_Cod '(per semine e raccolte)
                                    Lotto = Agenda.Movimenti(i).Movimenti_Dettagli(j).Lotto '(per semine)
                                    Udm_Cod = Agenda.Movimenti(i).Movimenti_Dettagli(j).Extra_Int
                                    Udm_Cod_Trasformato = Agenda.Movimenti(i).Movimenti_Dettagli(j).Udm_Cod
                                    Cod_Progetto = Agenda.Movimenti(i).Movimenti_Dettagli(j).Cod_Progetto '(per semilavorati raccolta)

                                    Dose = Agenda.Movimenti(i).Movimenti_Dettagli(j).Qta

                                    'dose e udm dell'irrigazione sono nel mov_dettaglio_tecnico
                                    If Agenda.Lav_Cod = LAVCOD_IRRIGAZIONE Then
                                        If Not IsNothing(Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici) AndAlso Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici.Count > 0 Then
                                            Dose = Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici(0).Qta_Ril
                                            Dett_Cod = Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici(0).dett_cod
                                        End If
                                    End If

                                    ''MOVIMENTI_DESTINAZIONI
                                    'If Not IsNothing(Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni) Then

                                    '    If (objParametriAgenda.Impianti.Count = 0) Or Agenda.Lav_Cod = LAVCOD_CATTURE_MASSA Then

                                    '        For x = 0 To Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni.Count - 1

                                    '            Dim objAppezzamento As New AgronicaCoreModello.ParametriAgenda_Temp.Impianto

                                    '            objAppezzamento.Piva = Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Piva
                                    '            objAppezzamento.Sa_Cod = Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Sa_Cod
                                    '            objAppezzamento.Appezza = Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Appezza
                                    '            objAppezzamento.ID_Reg = Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Id_Destinazione

                                    '            'sup_trattata
                                    '            objAppezzamento.Qta2 = Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Qta2


                                    '            If j = 0 Then

                                    '                'Qta_Tot_Old += Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Qta
                                    '                'Sup_Tot_Old += Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Qta2

                                    '                strFiltroImp &= " (Reg_Impianti.Piva='" & objAppezzamento.Piva & "' " &
                                    '                                " AND Reg_Impianti.Sa_Cod=" & objAppezzamento.Sa_Cod & " " &
                                    '                                " AND Reg_Impianti.appezza=" & objAppezzamento.Appezza & " " &
                                    '                                " AND Reg_Impianti.id_reg=" & objAppezzamento.ID_Reg & " " &
                                    '                                " ) OR "

                                    '                objParametriAgenda.Impianti.Add(objAppezzamento)

                                    '            End If

                                    '        Next

                                    '    End If

                                    'End If

                                    'ricavo le sup_imp
                                    'If strFiltroImp <> "" And j = 0 Then

                                    '    Dim objImp As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
                                    '    Dim DtImp As New DataTable
                                    '    DtImp = objImp.Leggi(Piva, 0, 0, 0, _
                                    '                        enumSelezioneVariabile.Selezione_TabellaDatiMinimi, _
                                    '                        " (" & Left(strFiltroImp, strFiltroImp.Length - 3) & ") ", _
                                    '                        "", objParametri_Server)

                                    '    For a = 0 To objParametriAgenda.Impianti.Count - 1
                                    '        For p = 0 To DtImp.Rows.Count - 1
                                    '            If objParametriAgenda.Impianti(a).Piva = DtImp.Rows(p).Item("piva") And _
                                    '                objParametriAgenda.Impianti(a).Sa_Cod = DtImp.Rows(p).Item("sa_cod") And _
                                    '                objParametriAgenda.Impianti(a).Appezza = DtImp.Rows(p).Item("appezza") And _
                                    '                objParametriAgenda.Impianti(a).ID_Reg = DtImp.Rows(p).Item("id_reg") Then

                                    '                objParametriAgenda.Impianti(a).Sup_Imp = DtImp.Rows(p).Item("sup_imp")

                                    '                Sup_Tot_New += objParametriAgenda.Impianti(a).Sup_Imp

                                    '                Exit For
                                    '            End If
                                    '        Next
                                    '    Next

                                    'End If



                                    Select Case Lav_Cod
                                        Case LAVCOD_CONFUSIONE_SESSUALE, LAVCOD_DISORIENTAMENTO_SESSUALE,
                                             LAVCOD_SEMINA, LAVCOD_TRAPIANTO, LAVCOD_SOVESCIO, LAVCOD_SOD_SEDDING,
                                             LAVCOD_RACCOLTA,
                                             LAVCOD_INSTALLAZIONE_TRAPPOLE, LAVCOD_CATTURE_MASSA
                                            Dose_Tot_New = Math.Round(Dose / Sup_Tot_Old * Sup_Tot_New, 0)
                                            Dose_Tot_Old = Dose
                                        Case Else
                                            Dose_Tot_New = Dose * Sup_Tot_New
                                            Dose_Tot_Old = Dose * Sup_Tot_Old
                                    End Select


                                    Select Case Agenda.Movimenti(i).Movimenti_Dettagli(j).Extra_Int
                                        Case enum_UnitaMisura.Grammi  'g
                                            Dose_Trasformata_New = Dose / 1000
                                            DoseTotaleTrasformata_New = Dose_Tot_New / 1000
                                        Case enum_UnitaMisura.Milligrammi
                                            Dose_Trasformata_New = Dose / 1000000
                                            DoseTotaleTrasformata_New = Dose_Tot_New / 1000000
                                        Case enum_UnitaMisura.Quintali
                                            Dose_Trasformata_New = Dose * 100
                                            DoseTotaleTrasformata_New = Dose_Tot_New * 100
                                        Case enum_UnitaMisura.Tonnellate
                                            Dose_Trasformata_New = Dose * 1000
                                            DoseTotaleTrasformata_New = Dose_Tot_New * 1000
                                        Case enum_UnitaMisura.Millilitri
                                            Dose_Trasformata_New = Dose / 1000
                                            DoseTotaleTrasformata_New = Dose_Tot_New / 1000
                                        Case enum_UnitaMisura.CentimetriCubi
                                            Dose_Trasformata_New = Dose / 100
                                            DoseTotaleTrasformata_New = Dose_Tot_New / 100
                                        Case Else
                                            Dose_Trasformata_New = Dose
                                            DoseTotaleTrasformata_New = Dose_Tot_New
                                    End Select


                                    If Fr_Cod <> 0 Then
                                        If Not HashTotNew.ContainsKey(Fr_Cod) Then
                                            HashTotNew.Add(Fr_Cod, DoseTotaleTrasformata_New)
                                        End If
                                    ElseIf Mat_Cod <> 0 Then
                                        If Not HashTotNew.ContainsKey(Mat_Cod & "|" & Udm_Cod & "|" & Lotto & "|" & Cod_Progetto) Then
                                            HashTotNew.Add(Mat_Cod & "|" & Udm_Cod & "|" & Lotto & "|" & Cod_Progetto, DoseTotaleTrasformata_New)
                                        End If
                                    End If

                                    Dose_New = 0
                                    Dose_New_Hl = 0

                                    Select Case Lav_Cod

                                        Case LAVCOD_CONFUSIONE_SESSUALE, LAVCOD_DISORIENTAMENTO_SESSUALE,
                                             LAVCOD_SEMINA, LAVCOD_TRAPIANTO, LAVCOD_SOVESCIO, LAVCOD_SOD_SEDDING,
                                             LAVCOD_RACCOLTA
                                            'qta è quella totale distribuita
                                            Dose_New = Dose_Tot_New
                                            'modifico la qta del dettaglio (dose_ha)
                                            Dim objDet As New AgronicaCoreContabDAL.Movimenti_Dettagli_W
                                            objDet.Modifica_Quantita(Agenda.Movimenti(i).Movimenti_Dettagli(j).Piva,
                                                                     Agenda.Movimenti(i).Movimenti_Dettagli(j).Id_Agenda,
                                                                     Agenda.Movimenti(i).Movimenti_Dettagli(j).Id_Mov,
                                                                     Agenda.Movimenti(i).Movimenti_Dettagli(j).Id_Mov_Det,
                                                                     0,
                                                                     0,
                                                                     0,
                                                                     0,
                                                                     Dose_New, 0, 0,
                                                                     0,
                                                                     0,
                                                                     0,
                                                                     "",
                                                                     "",
                                                                     objParametri_Server)

                                        Case LAVCOD_IRRIGAZIONE,
                                             LAVCOD_INSTALLAZIONE_TRAPPOLE, LAVCOD_CATTURE_MASSA

                                        Case Else

                                            Dose_New = Dose

                                            If AcquaTot <> 0 Then
                                                Dose_New_Hl = Dose_New * Sup_Tot_New / AcquaTot
                                            End If

                                            'modifico la qta_extra e la qta_extra_tot del dettaglio (dose_hl, qta tot)
                                            Dim objDet As New AgronicaCoreContabDAL.Movimenti_Dettagli_W
                                            objDet.Modifica_Quantita(Agenda.Movimenti(i).Movimenti_Dettagli(j).Piva,
                                                                                         Agenda.Movimenti(i).Movimenti_Dettagli(j).Id_Agenda,
                                                                                         Agenda.Movimenti(i).Movimenti_Dettagli(j).Id_Mov,
                                                                                         Agenda.Movimenti(i).Movimenti_Dettagli(j).Id_Mov_Det,
                                                                                         0,
                                                                                         0,
                                                                                         0,
                                                                                         0,
                                                                                         Dose_New, Dose_New_Hl, Dose_Tot_New,
                                                                                         0,
                                                                                         0,
                                                                                         0,
                                                                                         "",
                                                                                         "",
                                                                                         objParametri_Server)

                                    End Select


                                    Dim objDest As New AgronicaCoreContabDAL.Mov_Destinazioni_W

                                    'MOVIMENTI_DESTINAZIONI
                                    If Not IsNothing(Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni) Then

                                        For x = 0 To Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni.Count - 1

                                            For a = 0 To objParametriAgenda.Impianti.Count - 1
                                                If objParametriAgenda.Impianti(a).Piva = Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Piva AndAlso
                                                   objParametriAgenda.Impianti(a).Sa_Cod = Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Sa_Cod AndAlso
                                                   objParametriAgenda.Impianti(a).Appezza = Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Appezza AndAlso
                                                   objParametriAgenda.Impianti(a).ID_Reg = Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Id_Destinazione Then


                                                    Dim QuotaDistribuzioneNew As Decimal = 0
                                                    If Sup_Tot_New <> 0 Then
                                                        QuotaDistribuzioneNew = objParametriAgenda.Impianti(a).Sup_Imp / Sup_Tot_New
                                                    End If

                                                    Select Case Lav_Cod

                                                        Case LAVCOD_INSTALLAZIONE_TRAPPOLE, LAVCOD_CATTURE_MASSA

                                                            objDest.Modifica_SupTrattata(Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Piva,
                                                                             Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Sa_Cod,
                                                                             Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Id_Agenda,
                                                                             Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Id_Mov,
                                                                             Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Id_Mov_Det,
                                                                             Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Appezza,
                                                                             Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Id_Destinazione,
                                                                             objParametriAgenda.Impianti(a).Sup_Imp,
                                                                             QuotaDistribuzioneNew,
                                                                             "", objParametri_Server)
                                                        Case Else

                                                            Select Case Lav_Cod
                                                                Case LAVCOD_DISTRIBUZIONE_INSETTI
                                                                    Qta_Dest_New = Math.Round(Dose * objParametriAgenda.Impianti(a).Sup_Imp, 0)
                                                                Case LAVCOD_CONFUSIONE_SESSUALE, LAVCOD_DISORIENTAMENTO_SESSUALE,
                                                                     LAVCOD_SEMINA, LAVCOD_TRAPIANTO, LAVCOD_SOVESCIO, LAVCOD_SOD_SEDDING,
                                                                     LAVCOD_RACCOLTA
                                                                    'dose salvata sempre qta totale
                                                                    Qta_Dest_New = Math.Round((Dose_New / Sup_Tot_New) * objParametriAgenda.Impianti(a).Sup_Imp, 0)
                                                                    Dose_Tot_New = Math.Round(Dose_New, 0)
                                                                Case LAVCOD_IRRIGAZIONE
                                                                    Select Case Dett_Cod
                                                                        Case enum_UnitaMisura.Millimetri
                                                                            Dose = Dose * 10
                                                                        Case enum_UnitaMisura.METRI3__HA
                                                                            Dose = Dose * 1
                                                                        Case Else
                                                                            Dose = 0
                                                                    End Select
                                                                    Qta_Dest_New = Dose * objParametriAgenda.Impianti(a).Sup_Imp

                                                                Case Else
                                                                    Qta_Dest_New = Dose_Trasformata_New * objParametriAgenda.Impianti(a).Sup_Imp
                                                            End Select

                                                            'modifico qta_destinazione e sup_trattata
                                                            objDest.Modifica_Quantita_e_SupTrattata(Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Piva,
                                                                             Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Sa_Cod,
                                                                             Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Id_Agenda,
                                                                             Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Id_Mov,
                                                                             Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Id_Mov_Det,
                                                                             Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Appezza,
                                                                             Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Id_Destinazione,
                                                                             Qta_Dest_New,
                                                                             objParametriAgenda.Impianti(a).Sup_Imp,
                                                                             QuotaDistribuzioneNew,
                                                                             "", objParametri_Server)

                                                    End Select



                                                    Exit For
                                                End If

                                            Next

                                        Next

                                    End If


                                Next

                            End If

                    End Select

                Next


                'MOVIMENTO SCARICO
                'modifico la qta del dettaglio e la qta destinazione (magazzino)

                For i = 0 To Agenda.Movimenti.Count - 1

                    Select Case Agenda.Movimenti(i).Cau_Mov

                        Case CAU_SCARICO, CAU_CARICO

                            If Not IsNothing(Agenda.Movimenti(i).Movimenti_Dettagli) Then

                                For j = 0 To Agenda.Movimenti(i).Movimenti_Dettagli.Count - 1

                                    Fr_Cod = Agenda.Movimenti(i).Movimenti_Dettagli(j).Pro_Cod
                                    Mat_Cod = Agenda.Movimenti(i).Movimenti_Dettagli(j).Mat_Cod
                                    Lotto = Agenda.Movimenti(i).Movimenti_Dettagli(j).Lotto
                                    Udm_Cod = Agenda.Movimenti(i).Movimenti_Dettagli(j).Extra_Int
                                    Udm_Cod_Trasformato = Agenda.Movimenti(i).Movimenti_Dettagli(j).Udm_Cod
                                    Cod_Progetto = Agenda.Movimenti(i).Movimenti_Dettagli(j).Cod_Progetto

                                    Dose = Agenda.Movimenti(i).Movimenti_Dettagli(j).Qta

                                    DoseTotaleTrasformata_New = 0

                                    If Fr_Cod <> 0 Then
                                        If HashTotNew.ContainsKey(Fr_Cod) Then
                                            DoseTotaleTrasformata_New = HashTotNew(Fr_Cod)
                                        End If
                                    Else
                                        If HashTotNew.ContainsKey(Mat_Cod & "|" & Udm_Cod & "|" & Lotto & "|" & Cod_Progetto) Then
                                            DoseTotaleTrasformata_New = HashTotNew(Mat_Cod & "|" & Udm_Cod & "|" & Lotto & "|" & Cod_Progetto)
                                        End If
                                    End If

                                    If DoseTotaleTrasformata_New <> 0 Then

                                        'modifico la qta del dettaglio 
                                        Dim objDet As New AgronicaCoreContabDAL.Movimenti_Dettagli_W
                                        objDet.Modifica_Quantita(Agenda.Movimenti(i).Movimenti_Dettagli(j).Piva,
                                                             Agenda.Movimenti(i).Movimenti_Dettagli(j).Id_Agenda,
                                                             Agenda.Movimenti(i).Movimenti_Dettagli(j).Id_Mov,
                                                             Agenda.Movimenti(i).Movimenti_Dettagli(j).Id_Mov_Det,
                                                             0,
                                                             0,
                                                             0,
                                                             0,
                                                             DoseTotaleTrasformata_New, 0, 0,
                                                             0,
                                                             0,
                                                             0,
                                                             "",
                                                             "",
                                                             objParametri_Server)


                                        Dim objDest As New AgronicaCoreContabDAL.Mov_Destinazioni_W

                                        'MOVIMENTI_DESTINAZIONI
                                        If Not IsNothing(Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni) Then

                                            For x = 0 To Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni.Count - 1

                                                'modifico qta_destinazione 
                                                objDest.Modifica_Quantita(Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Piva,
                                                             Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Sa_Cod,
                                                             Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Id_Agenda,
                                                             Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Id_Mov,
                                                             Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Id_Mov_Det,
                                                             Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Appezza,
                                                             Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Id_Destinazione,
                                                             DoseTotaleTrasformata_New,
                                                             "", objParametri_Server)

                                            Next

                                        End If

                                    End If

                                Next

                            End If

                    End Select

                Next


            End If

        End If

        Dim objAgendaW As New AgronicaCoreContabDAL.Agenda_W
        Dim Flag_UpdateFlagOK As Boolean

        Flag_UpdateFlagOK = objAgendaW.Agenda_Sblocca2(Piva, Id_Agenda, objParametri_Server.UtenteUsername, Date.Now, "", objParametri_Server)




    End Sub

    Private Function AggiungiMagazzino(ByVal BLOCCA_SE_SUPERA_GIACENZE As Boolean, _
                                  ByVal Id_Agenda As Integer, _
                                  ByVal Piva As String, _
                                  ByVal FabbricatoCodice As String, _
                                  ByRef MessaggioErrore As String) As Boolean


        Dim DoseTotale As Decimal
        Dim Cod_Innesco As Integer
        Dim N_Inneschi As Decimal

        Dim Mezzo As Integer

        Dim Movimento As Movimento
        Dim Movimento_Dettaglio As Movimento_Dettaglio
        Dim Movimento_Destinazione As Movimento_Destinazione

        Dim objAgenda As New Agenda_Operazione_Helper
        Dim Agenda As New Operazione_Agenda
        Agenda = objAgenda.Leggi(Piva, 0, _
                                     CInt(Id_Agenda), _
                                     0, _
                                     objParametri_Server)

        objAgenda = Nothing

        'MAGAZZINO
        'FabbricatoCodice = Fabbricato_Cod|Sa_Cod|Piva
        Dim piva_magazzino As String = ""
        Dim sa_cod_magazzino As String = ""
        Dim fabbricatox_Cod As String = ""
        If FabbricatoCodice <> "0" AndAlso _
            FabbricatoCodice <> "" AndAlso _
            Split(FabbricatoCodice, "|").Count = 3 AndAlso _
            Split(FabbricatoCodice, "|")(2) = Piva Then
            piva_magazzino = Split(FabbricatoCodice, "|")(2)
            sa_cod_magazzino = Split(FabbricatoCodice, "|")(1)
            fabbricatox_Cod = Split(FabbricatoCodice, "|")(0)
        Else
            MessaggioErrore = "Magazzino non selezionato!"
            Return False
        End If


        If Not IsNothing(Agenda) Then

            If Not IsNothing(Agenda.Movimenti) Then

                Dim Lista_Dettagli As New List(Of Movimento_Dettaglio)

                'MOVIMENTO LAVORAZIONE
                'modifico le destinazioni --> qta e qta2 
                For i = 0 To Agenda.Movimenti.Count - 1

                    Select Case Agenda.Movimenti(i).Cau_Mov

                        Case CAU_TRATTAMENTO, CAU_LAVORAZIONE, CAU_RILIEVO_CAMPO

                            Mezzo = Agenda.Movimenti(i).Mezzo

                            'MOVIMENTI_DETTAGLI
                            If Not IsNothing(Agenda.Movimenti(i).Movimenti_Dettagli) Then

                                Select Case Agenda.Lav_Cod

                                    '1 dettaglio per ogni impianto
                                    '1 dettaglio tecnico per ogni trappola contenente i dati dell'innesco
                                    Case LAVCOD_INSTALLAZIONE_TRAPPOLE, LAVCOD_CATTURE_MASSA


                                        '-----------------------------------------
                                        'DETTAGLIO TRAPPOLA SCARICO
                                        Dim Movimento_Dettaglio_Trappola As New Movimento_Dettaglio

                                        Movimento_Dettaglio_Trappola.Id_Agenda = Agenda.Id_Agenda
                                        Movimento_Dettaglio_Trappola.Piva = Agenda.Piva
                                        Movimento_Dettaglio_Trappola.Sa_Cod = sa_cod_magazzino
                                        Movimento_Dettaglio_Trappola.Data = Agenda.Data
                                        Movimento_Dettaglio_Trappola.Lav_Cod = Agenda.Lav_Cod
                                        Movimento_Dettaglio_Trappola.Cau_Mov = CAU_SCARICO

                                        N_Inneschi = 0
                                        Cod_Innesco = 0

                                        For j = 0 To Agenda.Movimenti(i).Movimenti_Dettagli.Count - 1

                                            'prendo i dati dal primo dettaglio (qta inclusa)
                                            If j = 0 Then

                                                Movimento_Dettaglio_Trappola.Elem_Cod = Agenda.Movimenti(i).Movimenti_Dettagli(j).Elem_Cod
                                                Movimento_Dettaglio_Trappola.Pro_Cod = Agenda.Movimenti(i).Movimenti_Dettagli(j).Pro_Cod
                                                Movimento_Dettaglio_Trappola.Mat_Cod = Agenda.Movimenti(i).Movimenti_Dettagli(j).Mat_Cod

                                                Movimento_Dettaglio_Trappola.Extra_Int = 0
                                                Movimento_Dettaglio_Trappola.Udm_Cod = Agenda.Movimenti(i).Movimenti_Dettagli(j).Udm_Cod
                                                Movimento_Dettaglio_Trappola.Qta = Agenda.Movimenti(i).Movimenti_Dettagli(j).Qta

                                                Movimento_Dettaglio_Trappola.Contabilizzato = Agenda.Movimenti(i).Movimenti_Dettagli(j).Contabilizzato

                                            End If

                                            'Movimenti_Dettagli_Tecnici (somma totale inneschi e codice)
                                            If Not IsNothing(Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici) Then
                                                For x = 0 To Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici.Count - 1
                                                    N_Inneschi += Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici(x).Dose
                                                    Cod_Innesco = Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici(x).Av_Cod
                                                Next
                                            End If

                                        Next

                                        Movimento_Dettaglio_Trappola.BaseCode = Agenda.BaseCode
                                        Movimento_Dettaglio_Trappola.TopCode = Agenda.TopCode

                                        'MOVIMENTO DESTINAZIONE SCARICO TRAPPOLA

                                        Dim Lista_Destinazioni_Trappola As New List(Of Movimento_Destinazione)

                                        Dim Movimento_Destinazione_Trappola = New Movimento_Destinazione

                                        Movimento_Destinazione_Trappola.Data = Agenda.Data

                                        Movimento_Destinazione_Trappola.Id_Agenda = Agenda.Id_Agenda
                                        Movimento_Destinazione_Trappola.Piva = piva_magazzino
                                        Movimento_Destinazione_Trappola.Sa_Cod = sa_cod_magazzino
                                        Movimento_Destinazione_Trappola.Appezza = 0
                                        Movimento_Destinazione_Trappola.Id_Destinazione = fabbricatox_Cod
                                        Movimento_Destinazione_Trappola.Tipo = MAGAZZINO

                                        Movimento_Destinazione_Trappola.Qta = Movimento_Dettaglio_Trappola.Qta

                                        Movimento_Destinazione_Trappola.BaseCode = Agenda.BaseCode
                                        Movimento_Destinazione_Trappola.TopCode = Agenda.TopCode

                                        Lista_Destinazioni_Trappola.Add(Movimento_Destinazione_Trappola)

                                        Movimento_Dettaglio_Trappola.Movimenti_Destinazioni = Lista_Destinazioni_Trappola

                                        Lista_Dettagli.Add(Movimento_Dettaglio_Trappola)

                                        '-----------------------------------------
                                        'DETTAGLIO INNESCHI
                                    
                                        'DETTAGLIO TRAPPOLA SCARICO
                                        Dim Movimento_Dettaglio_Innesco = New Movimento_Dettaglio

                                        Movimento_Dettaglio_Innesco.Id_Agenda = Agenda.Id_Agenda
                                        Movimento_Dettaglio_Innesco.Piva = Agenda.Piva
                                        Movimento_Dettaglio_Innesco.Sa_Cod = sa_cod_magazzino
                                        Movimento_Dettaglio_Innesco.Data = Agenda.Data
                                        Movimento_Dettaglio_Innesco.Lav_Cod = Agenda.Lav_Cod
                                        Movimento_Dettaglio_Innesco.Cau_Mov = CAU_SCARICO

                                        Movimento_Dettaglio_Innesco.Elem_Cod = INNESCHI
                                        Movimento_Dettaglio_Innesco.Pro_Cod = Cod_Innesco
                                        Movimento_Dettaglio_Innesco.Mat_Cod = 0

                                        Movimento_Dettaglio_Innesco.Extra_Int = 0
                                        Movimento_Dettaglio_Innesco.Udm_Cod = enum_UnitaMisura.Numero_Inneschi
                                        Movimento_Dettaglio_Innesco.Qta = N_Inneschi

                                        Movimento_Dettaglio_Innesco.Contabilizzato = Movimento_Dettaglio_Trappola.Contabilizzato

                                        Movimento_Dettaglio_Innesco.BaseCode = Agenda.BaseCode
                                        Movimento_Dettaglio_Innesco.TopCode = Agenda.TopCode

                                        'MOVIMENTO DESTINAZIONE SCARICO TRAPPOLA

                                        Dim Lista_Destinazioni_Innesco As New List(Of Movimento_Destinazione)

                                        Dim Movimento_Destinazione_Innesco = New Movimento_Destinazione

                                        Movimento_Destinazione_Innesco.Data = Agenda.Data

                                        Movimento_Destinazione_Innesco.Id_Agenda = Agenda.Id_Agenda
                                        Movimento_Destinazione_Innesco.Piva = piva_magazzino
                                        Movimento_Destinazione_Innesco.Sa_Cod = sa_cod_magazzino
                                        Movimento_Destinazione_Innesco.Appezza = 0
                                        Movimento_Destinazione_Innesco.Id_Destinazione = fabbricatox_Cod
                                        Movimento_Destinazione_Innesco.Tipo = MAGAZZINO

                                        Movimento_Destinazione_Innesco.Qta = N_Inneschi

                                        Movimento_Destinazione_Innesco.BaseCode = Agenda.BaseCode
                                        Movimento_Destinazione_Innesco.TopCode = Agenda.TopCode

                                        Lista_Destinazioni_Innesco.Add(Movimento_Destinazione_Innesco)

                                        Movimento_Dettaglio_Innesco.Movimenti_Destinazioni = Lista_Destinazioni_Innesco

                                        Lista_Dettagli.Add(Movimento_Dettaglio_Innesco)


                                        '1 dettaglio per ogni trappola
                                        '1 dettaglio tecnico per ogni trappola contenente i dati dell'innesco
                                    Case LAVCOD_REINNESCO_TRAPPOLE

                                        For j = 0 To Agenda.Movimenti(i).Movimenti_Dettagli.Count - 1

                                            Movimento_Dettaglio = New Movimento_Dettaglio

                                            Movimento_Dettaglio.Id_Agenda = Agenda.Id_Agenda
                                            Movimento_Dettaglio.Piva = Agenda.Piva
                                            Movimento_Dettaglio.Sa_Cod = sa_cod_magazzino
                                            Movimento_Dettaglio.Data = Agenda.Data
                                            Movimento_Dettaglio.Lav_Cod = Agenda.Lav_Cod
                                            Movimento_Dettaglio.Cau_Mov = CAU_SCARICO

                                            N_Inneschi = 0
                                            Cod_Innesco = 0

                                            'Movimenti_Dettagli_Tecnici (num inneschi e codice)
                                            If Not IsNothing(Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici) Then
                                                For x = 0 To Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici.Count - 1
                                                    N_Inneschi = Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici(x).Dose
                                                    Cod_Innesco = Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici(x).Av_Cod
                                                Next
                                            End If

                                            Movimento_Dettaglio.Elem_Cod = INNESCHI
                                            Movimento_Dettaglio.Pro_Cod = Cod_Innesco
                                            Movimento_Dettaglio.Mat_Cod = 0
                                            Movimento_Dettaglio.Qta = N_Inneschi
                                            Movimento_Dettaglio.Udm_Cod = enum_UnitaMisura.Numero_Inneschi

                                            Movimento_Dettaglio.Contabilizzato = Agenda.Movimenti(i).Movimenti_Dettagli(j).Contabilizzato

                                            Movimento_Dettaglio.BaseCode = Agenda.BaseCode
                                            Movimento_Dettaglio.TopCode = Agenda.TopCode

                                            'MOVIMENTO DESTINAZIONE SCARICO

                                            Dim Lista_Destinazioni As New List(Of Movimento_Destinazione)

                                            Movimento_Destinazione = New Movimento_Destinazione

                                            Movimento_Destinazione.Data = Agenda.Data

                                            Movimento_Destinazione.Id_Agenda = Agenda.Id_Agenda
                                            Movimento_Destinazione.Piva = piva_magazzino
                                            Movimento_Destinazione.Sa_Cod = sa_cod_magazzino
                                            Movimento_Destinazione.Appezza = 0
                                            Movimento_Destinazione.Id_Destinazione = fabbricatox_Cod
                                            Movimento_Destinazione.Tipo = MAGAZZINO

                                            Movimento_Destinazione.Qta = N_Inneschi

                                            Movimento_Destinazione.BaseCode = Agenda.BaseCode
                                            Movimento_Destinazione.TopCode = Agenda.TopCode

                                            Lista_Destinazioni.Add(Movimento_Destinazione)

                                            Movimento_Dettaglio.Movimenti_Destinazioni = Lista_Destinazioni

                                            Lista_Dettagli.Add(Movimento_Dettaglio)

                                        Next


                                        'TUTTE LE ALTRE OPERAZIONI
                                        '1 dettaglio per ogni prodotto
                                    Case Else

                                        For j = 0 To Agenda.Movimenti(i).Movimenti_Dettagli.Count - 1

                                            Movimento_Dettaglio = New Movimento_Dettaglio

                                            Movimento_Dettaglio.Id_Agenda = Agenda.Id_Agenda
                                            Movimento_Dettaglio.Piva = Agenda.Piva
                                            Movimento_Dettaglio.Sa_Cod = sa_cod_magazzino
                                            Movimento_Dettaglio.Data = Agenda.Data
                                            Movimento_Dettaglio.Lav_Cod = Agenda.Lav_Cod
                                            Movimento_Dettaglio.Cau_Mov = CAU_SCARICO

                                            Movimento_Dettaglio.Elem_Cod = Agenda.Movimenti(i).Movimenti_Dettagli(j).Elem_Cod
                                            Movimento_Dettaglio.Pro_Cod = Agenda.Movimenti(i).Movimenti_Dettagli(j).Pro_Cod
                                            Movimento_Dettaglio.Mat_Cod = Agenda.Movimenti(i).Movimenti_Dettagli(j).Mat_Cod

                                            Movimento_Dettaglio.Extra_Int = 0
                                            Movimento_Dettaglio.Udm_Cod = Agenda.Movimenti(i).Movimenti_Dettagli(j).Udm_Cod

                                            Movimento_Dettaglio.Contabilizzato = Agenda.Movimenti(i).Movimenti_Dettagli(j).Contabilizzato

                                            DoseTotale = 0

                                            'MOVIMENTI_DESTINAZIONI (somma totale prodotto)
                                            If Not IsNothing(Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni) Then
                                                For x = 0 To Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni.Count - 1
                                                    DoseTotale += Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Qta
                                                Next
                                            End If

                                            Movimento_Dettaglio.Qta = Math.Round(DoseTotale, 4)

                                            Movimento_Dettaglio.BaseCode = Agenda.BaseCode
                                            Movimento_Dettaglio.TopCode = Agenda.TopCode

                                            'MOVIMENTO DESTINAZIONE SCARICO

                                            Dim Lista_Destinazioni As New List(Of Movimento_Destinazione)

                                            Movimento_Destinazione = New Movimento_Destinazione

                                            Movimento_Destinazione.Data = Agenda.Data

                                            Movimento_Destinazione.Id_Agenda = Agenda.Id_Agenda
                                            Movimento_Destinazione.Piva = piva_magazzino
                                            Movimento_Destinazione.Sa_Cod = sa_cod_magazzino
                                            Movimento_Destinazione.Appezza = 0
                                            Movimento_Destinazione.Id_Destinazione = fabbricatox_Cod
                                            Movimento_Destinazione.Tipo = MAGAZZINO

                                            Movimento_Destinazione.Qta = DoseTotale

                                            Movimento_Destinazione.BaseCode = Agenda.BaseCode
                                            Movimento_Destinazione.TopCode = Agenda.TopCode

                                            Lista_Destinazioni.Add(Movimento_Destinazione)

                                            Movimento_Dettaglio.Movimenti_Destinazioni = Lista_Destinazioni

                                            Lista_Dettagli.Add(Movimento_Dettaglio)

                                        Next

                                End Select

                                

                            End If

                            'se l'operazione ha già il magazzino non faccio nulla
                        Case CAU_SCARICO

                            MessaggioErrore = "- (" & CDate(Agenda.Data).ToShortDateString & ") " & Agenda.Des_Lib & " --> Magazzino già indicato!"
                            Return False

                    End Select

                Next

                'se ho recuperato tutti i dettagli
                'aggiungo movimento scarico
                Dim qta_in_data As Decimal
                Dim Qta As Decimal

                If Lista_Dettagli IsNot Nothing AndAlso Lista_Dettagli.Count > 0 Then

                    'se l'utente ha il blocco verifico le qta in giacenza
                    If BLOCCA_SE_SUPERA_GIACENZE Then

                        For i = 0 To Lista_Dettagli.Count - 1

                            Qta = Lista_Dettagli(i).Qta

                            'guardo se la quantità è conforme per la data di intervento
                            Dim objMovDet As New AgronicaCoreContabDAL.Movimenti_Dettagli_R
                            qta_in_data = objMovDet.Verifica_Giacenze_Con_Magazzino_Esterno(piva_magazzino, _
                                                                                            sa_cod_magazzino, _
                                                                                            fabbricatox_Cod, _
                                                                                            Lista_Dettagli(i).Elem_Cod, _
                                                                                            Lista_Dettagli(i).Pro_Cod, _
                                                                                            Lista_Dettagli(i).Mat_Cod, _
                                                                                            0, 0, LOTTO_NONDEFINITO, 0, _
                                                                                            Lista_Dettagli(i).Udm_Cod, _
                                                                                            AGRODATAINIZIO, _
                                                                                            Agenda.Data, _
                                                                                            Agenda.Piva, _
                                                                                            Agenda.Sa_Cod, _
                                                                                            Agenda.Id_Agenda, _
                                                                                            objParametri_Server)
                            If Qta > Math.Round(qta_in_data, 4) Then
                                MessaggioErrore = "- (" & CDate(Agenda.Data).ToShortDateString & ") " & Agenda.Des_Lib & " --> Quantità prodotto non sufficiente!"
                                Return False
                            End If
                        Next

                    End If

                    'inserimento 
                    'MOVIMENTO
                    'DETTAGLI
                    'DESTINAZIONI

                    Try

                        Movimento = New Movimento

                        Movimento.Id_Agenda = Agenda.Id_Agenda
                        Movimento.Piva = piva_magazzino
                        Movimento.Sa_Cod = sa_cod_magazzino
                        Movimento.Data = Agenda.Data
                        Movimento.Lav_Cod = Agenda.Lav_Cod

                        Movimento.Cau_Mov = CAU_SCARICO
                        Movimento.Mov_Desc = "Scarico Magazzino"
                        Movimento.Mezzo = Mezzo

                        Movimento.BaseCode = Agenda.BaseCode
                        Movimento.TopCode = Agenda.TopCode

                        Movimento.Movimenti_Dettagli = Lista_Dettagli

                        Dim ObjMovimento As New Agenda_Movimenti_Helper
                        ObjMovimento.Scrivi(Movimento, objParametri_Server)
                        ObjMovimento = Nothing

                    Catch ex As Exception

                        MessaggioErrore = ex.Message
                        Return False

                    End Try

                    Return True

                End If


            End If

        End If

    End Function

    Protected Sub chk_solo_privati_macchine_CheckedChanged(sender As Object, e As EventArgs) Handles chk_solo_privati_macchine.CheckedChanged
        CaricaListaMacchinari()
    End Sub

    Protected Sub chk_solo_privati_contatti_CheckedChanged(sender As Object, e As EventArgs) Handles chk_solo_privati_contatti.CheckedChanged
        CaricaListaPersone()
    End Sub

    Private Function AggiungiMacchine(ByVal Id_Agenda As Integer, _
                              ByVal Piva As String, _
                              ByVal Sa_Cod As Integer, _
                              ByVal Data As Date, _
                              ByVal Ha_Utilizzo As Decimal, _
                              ByVal EliminaPrecedenti As Boolean, _
                              ByRef MessaggioErrore As String) As Boolean


        Dim ObjSequenze As New Agro_Sequenze
        Dim objMovimento As New AgronicaCoreContabDAL.Movimenti_R
        Dim Dt As DataTable

        Dim BaseCode, TopCode As Integer
        Utilityprovider.Calcola_BaseCode_TopCode(BaseCode, TopCode,
                                                 Session("ASG_ProgressivoGIAS").ToString)

        Dt = objMovimento.Leggi(Piva, 0, Id_Agenda, 0, 0, CAU_IMPUTAZIONE_PARCOMACCHINE, _
                                enumSelezioneVariabile.Selezione_TabellaCompleta, _
                                "", "", objParametri_Server)

        Dim Id_Mov As Integer = 0

        If Dt.Rows.Count = 0 Then
            Id_Mov = ObjSequenze.NuovoId_Tabella( _
                                           "Movimenti", _
                                           BaseCode, _
                                           TopCode, _
                                           objParametri_Server)

            Dim objMovimentoW As New AgronicaCoreContabDAL.Movimenti_W
            objMovimentoW.Scrivi(Piva, _
                                Sa_Cod, _
                                Id_Agenda, _
                                Id_Mov, _
                                0, _
                                CAU_IMPUTAZIONE_PARCOMACCHINE, _
                                "Imputazione di Costi Dovuti ad Utilizzo del Parco Macchine", _
                                Data, _
                                AGRODATAFINE, _
                                AGRODATAINIZIO, _
                                0, 0, 0, 0, 0, 0, 0, 0, "", "", 0, _
                                Data, _
                                0, 0, "", 0, Date.Now, "", "", "", 0, 0, 0, 0, "", 0, 0, _
                                AGRODATAINIZIO, 0, 0, _
                                Data, _
                                AGRODATAFINE, _
                                0, objParametri_Server)
        Else
            Id_Mov = Dt.Rows(0).Item("id_mov")
            'elimino i precedenti dettagli se richiesto dall'utente
            ' VAnni & Grilli: 26/8/2019: imposto la chiamata con sa_cod = 0 per ovviare al problema riscontrato da FMG dove si fa riferimento a sa_Cod estranei al movimento
            If EliminaPrecedenti Then
                Dim objMDC As New Agenda_Movimenti_Dettagli_Helper
                objMDC.Cancella(Piva,
                                0,
                                Id_Agenda,
                                Id_Mov,
                                0,
                                objParametri_Server)
            End If
        End If

        Dim m As Integer
        Dim Mac_Cod As Integer

        Dim MacchinaPresente As Boolean
        Dim objDettagliR As New AgronicaCoreContabDAL.Movimenti_Dettagli_R
        Dim Id_Mov_Det As Integer

        For m = 0 To GridView_Macchine.Rows.Count - 1

            If CType(GridView_Macchine.Rows(m).FindControl("ChkSelezionaMacchina"), CheckBox).Checked = True Then

                Id_Mov_Det = 0
                MacchinaPresente = False
                Mac_Cod = CInt(GridView_Macchine.Rows(m).Cells(3).Text)
              
                Dt = objDettagliR.Leggi(Piva, Sa_Cod, Id_Agenda, Id_Mov, 0, 0, 0, Mac_Cod, _
                           "", 0, 0, 0, 0, 0, 0, enumSelezioneVariabile.Selezione_TabellaCompleta, _
                           "", "", objParametri_Server)

                If Dt.Rows.Count = 0 Then
                    Id_Mov_Det = ObjSequenze.NuovoId_Tabella( _
                                                          "Movimenti_Dettagli", _
                                                          BaseCode, _
                                                          TopCode, _
                                                          objParametri_Server)

                    Dim objDettagli As New AgronicaCoreContabDAL.Movimenti_Dettagli_W
                    Dim BDummy As Boolean
                    BDummy = objDettagli.Scrivi( _
                            Piva, _
                            Sa_Cod, _
                            Id_Agenda, _
                            Id_Mov, _
                            Id_Mov_Det, _
                            MACCHINE, 0, Mac_Cod, _
                            "", _
                            Ha_Utilizzo, _
                            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, "", 0, Date.Now, 0, 1900, 0, 0, 0, 0, 1, 3, "", 0, 0, _
                            0, 0, 0, 0, 0, 0, 0, 0, "", 0, 0, 0, "", "", "", AGRODATAINIZIO, AGRODATAFINE, objParametri_Server)
                    'If BDummy = True Then
                    '    Return 1 'OK
                    'Else
                    '    Return 2 'errore
                    'End If
                Else
                    'Return 3 'già Presente
                End If

            End If

        Next

        Return True

    End Function

    Private Function EliminaMacchine(ByVal Id_Agenda As Integer,
                              ByVal Piva As String,
                              ByRef MessaggioErrore As String) As Boolean


        Dim objMovimento As New AgronicaCoreContabDAL.Movimenti_R
        Dim Dt As DataTable

        Try


            Dt = objMovimento.Leggi(Piva, 0, Id_Agenda, 0, 0, CAU_IMPUTAZIONE_PARCOMACCHINE,
                                    enumSelezioneVariabile.Selezione_TabellaCompleta,
                                    "", "", objParametri_Server)

            For Each movimento As DataRow In Dt.Rows
                Dim objMC As New Agenda_Movimenti_Helper
                objMC.Cancella(Piva, 0,
                                    Id_Agenda, movimento.Item("id_mov"),
                                    objParametri_Server)
            Next

        Catch ex As Exception

            MessaggioErrore = "Non è stato possibile eliminare le macchine/attrezzature associate: " & ex.Message
            Return False

        End Try

        Return True

    End Function

    Private Function AggiungiContatti(ByVal Id_Agenda As Integer, _
                          ByVal Piva As String, _
                          ByVal Sa_Cod As Integer, _
                          ByVal Data As Date, _
                          ByVal Ha_Utilizzo As Decimal, _
                          ByVal EliminaPrecedenti As Boolean, _
                          ByRef MessaggioErrore As String) As Boolean


        Dim ObjSequenze As New AgronicaCoreDataProvider.Agro_Sequenze
        Dim objMovimento As New AgronicaCoreContabDAL.Movimenti_R
        Dim objDettagliR As New AgronicaCoreContabDAL.Movimenti_Dettagli_R

        Dim DtMov As DataTable
        Dim DtMovC As DataTable
        Dim DtMovDet As DataTable

        Dim BaseCode, TopCode As Integer
        UtilityProvider.Calcola_BaseCode_TopCode(BaseCode, TopCode,
                                                 Session("ASG_ProgressivoGIAS").ToString)



        Dim m, i As Integer

        Dim Id_Mov As Integer
        Dim Id_Mov_Det As Integer

        Dim Cau_Mov As String
        Dim Cod_Risum As Integer
        Dim Cod_Rapp As Integer

        'elimino tutti i movimenti se richiesto dall'utente
        If EliminaPrecedenti Then
            DtMovC = objMovimento.Leggi(Piva, Sa_Cod, Id_Agenda, 0, 0, "", _
                                        enumSelezioneVariabile.Selezione_TabellaCompleta, _
                                        " Movimenti.Cau_Mov IN ('" & CAU_IMPUTAZIONE_MANODOPERA & "','" & CAU_IMPUTAZIONE_TERZISTI & "','" & CAU_IMPUTAZIONE_TECNICO_RESPONSABILE & "') ", "", objParametri_Server)
            If DtMovC IsNot Nothing AndAlso DtMovC.Rows.Count > 0 Then
                For i = 0 To DtMovC.Rows.Count - 1
                    Id_Mov = DtMovC.Rows(i).Item("id_mov")
                    Dim objMC As New Agenda_Movimenti_Helper
                    objMC.Cancella(Piva, _
                                    Sa_Cod, _
                                    Id_Agenda, _
                                    Id_Mov, _
                                    objParametri_Server)
                Next
            End If
        End If

        For m = 0 To GridView_Contatti.Rows.Count - 1

            If CType(GridView_Contatti.Rows(m).FindControl("ChkSelezionaContatto"), CheckBox).Checked = True Then

                Id_Mov = 0
                Id_Mov_Det = 0
                Cau_Mov = ""

                Cod_Rapp = CInt(GridView_Contatti.Rows(m).Cells(5).Text)
                Cod_Risum = CInt(GridView_Contatti.Rows(m).Cells(10).Text)

                Select Case Cod_Rapp
                    Case COD_LEGALE, COD_DIPENDENTE
                        Cau_Mov = CAU_IMPUTAZIONE_MANODOPERA
                    Case COD_TERZISTA
                        'manodopera
                        Cau_Mov = CAU_IMPUTAZIONE_TERZISTI
                    Case COD_TECNICORESPONSABILE
                        'manodopera
                        Cau_Mov = CAU_IMPUTAZIONE_TECNICO_RESPONSABILE
                    Case Else
                        Cau_Mov = CAU_IMPUTAZIONE_MANODOPERA
                End Select

                DtMov = objMovimento.Leggi(Piva, 0, Id_Agenda, 0, 0, Cau_Mov,
                                           enumSelezioneVariabile.Selezione_TabellaCompleta,
                                           "", "", objParametri_Server)

                'se il movimento non esiste ancora lo creo
                If DtMov.Rows.Count = 0 Then
                    Id_Mov = ObjSequenze.NuovoId_Tabella( _
                                                   "Movimenti", _
                                                   BaseCode, _
                                                   TopCode, _
                                                   objParametri_Server)

                    Dim objMovimentoW As New AgronicaCoreContabDAL.Movimenti_W
                    objMovimentoW.Scrivi(Piva, _
                                        Sa_Cod, _
                                        Id_Agenda, _
                                        Id_Mov, _
                                        0, _
                                        Cau_Mov, _
                                        "Imputazione di Costi Dovuti a Personale", _
                                        Data, _
                                        AGRODATAFINE, _
                                        AGRODATAINIZIO, _
                                        0, 0, 0, 0, 0, 0, 0, 0, "", "", 0, _
                                        Data, _
                                        0, 0, "", 0, Date.Now, "", "", "", 0, 0, 0, 0, "", 0, 0, _
                                        AGRODATAINIZIO, 0, 0, _
                                        Data, _
                                        AGRODATAFINE, _
                                        0, objParametri_Server)
                Else
                    Id_Mov = DtMov.Rows(0).Item("id_mov")
                    ''elimino i precedenti dettagli se richiesto dall'utente
                    'If EliminaPrecedenti = True Then
                    '    Dim objMDC As New Agenda_Movimenti_Dettagli_Helper
                    '    objMDC.Cancella(Piva, _
                    '                    Sa_Cod, _
                    '                    Id_Agenda, _
                    '                    Id_Mov, _
                    '                    0, _
                    '                    objParametri_Server)
                    'End If
                End If

                DtMovDet = objDettagliR.Leggi(Piva, Sa_Cod, Id_Agenda, Id_Mov, 0, 0, 0, Cod_Risum, _
                                     Cau_Mov, 0, 0, 0, 0, 0, 0, enumSelezioneVariabile.Selezione_TabellaCompleta, _
                                    "", "", objParametri_Server)
                If DtMovDet.Rows.Count = 0 Then
                    Id_Mov_Det = ObjSequenze.NuovoId_Tabella( _
                                                          "Movimenti_Dettagli", _
                                                          BaseCode, _
                                                          TopCode, _
                                                          objParametri_Server)

                    Dim objDettagli As New AgronicaCoreContabDAL.Movimenti_Dettagli_W
                    Dim BDummy As Boolean
                    BDummy = objDettagli.Scrivi( _
                            Piva, _
                            Sa_Cod, _
                            Id_Agenda, _
                            Id_Mov, _
                            Id_Mov_Det, _
                            ELEMCOD_MANODOPERA, 0, Cod_Risum, _
                            "", _
                            Ha_Utilizzo, _
                            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, "", 0, Date.Now, 0, 1900, 0, 0, 0, 0, 1, 3, "", 0, 0, _
                            0, 0, 0, 0, 0, 0, 0, 0, "", 0, 0, 0, "", "", "", AGRODATAINIZIO, AGRODATAFINE, objParametri_Server)
                    'If BDummy = True Then
                    '    Return 1 'OK
                    'Else
                    '    Return 2 'errore
                    'End If
                Else
                    'Return 3 'già Presente
                End If

            End If

        Next

        Return True

    End Function

    Private Function EliminaContatti(ByVal Id_Agenda As Integer,
                          ByVal Piva As String,
                           ByVal Sa_Cod As Integer,
                         ByRef MessaggioErrore As String) As Boolean

        Dim objMovimento As New AgronicaCoreContabDAL.Movimenti_R
        Dim Dt As DataTable

        Try

            Dt = objMovimento.Leggi(Piva, Sa_Cod, Id_Agenda, 0, 0, "",
                                    enumSelezioneVariabile.Selezione_TabellaCompleta,
                                    " Movimenti.Cau_Mov IN ('" & CAU_IMPUTAZIONE_MANODOPERA & "','" & CAU_IMPUTAZIONE_TERZISTI & "','" & CAU_IMPUTAZIONE_TECNICO_RESPONSABILE & "') ", "", objParametri_Server)


            For Each movimento As DataRow In Dt.Rows
                Dim objMC As New Agenda_Movimenti_Helper
                objMC.Cancella(Piva, 0,
                                    Id_Agenda, movimento.Item("id_mov"),
                                    objParametri_Server)
            Next

        Catch ex As Exception

            MessaggioErrore = "Non è stato possibile eliminare Manodopera / Terzista / Tecnico Responsabile: " & ex.Message
            Return False

        End Try

        Return True

    End Function

End Class