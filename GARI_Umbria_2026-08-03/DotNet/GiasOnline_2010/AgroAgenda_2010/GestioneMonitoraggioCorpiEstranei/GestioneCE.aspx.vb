Imports System.Xml
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreUtility

Public Class GestioneCE
    Inherits System.Web.UI.Page


    Dim Qs_PagRitorno As String
    Dim Qs_Piva As String

    Dim objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri
    Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri

    '##########################################################################################################
    Private Sub GestioneCE_Init(sender As Object, e As System.EventArgs) Handles Me.Init

        AddHandler CType(Page.Master.FindControl("ImgBtn_AnnullaTutto"), ImageButton).Click, AddressOf Me.AnnullaTutto

    End Sub

    '################################################################################################################
    Private Sub AnnullaTutto(ByVal sender As Object, ByVal e As System.EventArgs)

        'ritorno 1 alla pagina chiamante, così se sono nella edit
        'aggiorno i menù a tendina delle specie
        Dim strClose As String = "<SCRIPT language='javascript'> " & _
                                    "window.close(); " & _
                                    "</SCRIPT>"

        ' Dim strClose As String = "<script language='javascript'> try { window.parent.Contatto_gestisciValore_esci() } catch (e) { window.close() } </script>"
        ' ScriptManager.RegisterStartupScript(Nothing, Nothing, String.Format("jQuery_{0}", Nothing), strClose.ToString, False)

        Me.Controls.Add(New LiteralControl(strClose))


    End Sub

    '##########################################################################################################
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Response.Expires = 0

        '§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§



        Try
            '##############################################################
            '##########  Verifico Validità Sessione  ######################
            '##############################################################

            'If Session("ASG_Utente_Username") = "" Then
            '    Response.Redirect("../../Default.aspx")
            'End If

            '########################################################################################
            '##### inizializzazione oggetti objParametri_Utenti e objParametri_Server  ##############
            '########################################################################################
            '---
            objParametri_Utenti = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))
            objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))
            '---

            '##############################################################
            '#######################  Querystring  ########################
            '##############################################################

            Qs_Piva = Stringa_Decodifica(Request.QueryString("p").ToString, AgroKey_EncoderDecoder)


            '##############################################################
            '#####  Verifico se sono in Post-Back  ########################
            '##############################################################

            If Not Page.IsPostBack Then

                '==========================================
                '===== Pagina caricata per la prima volta
                '==========================================

                CaricamentoCorpiEstranei()

            Else
                '==========================================
                '===== Postback
                '==========================================

                Select Case Me.SI_NO.Value

                    Case "0" 'NO

                    Case "1" 'SI
                        CancellaCE()

                End Select

                Exit Sub

            End If

        Catch exc As Exception
            Messaggi.AgroMsgBox("Problemi nel caricamento della pagina: " & vbCrLf & exc.Message, Page)
        End Try

    End Sub



    '##################################################################################
    Private Sub ID_Trova_Click(sender As Object, e As System.Web.UI.ImageClickEventArgs) Handles ID_Trova.Click
        CaricamentoCorpiEstranei()
    End Sub


    '##################################################################################
    Private Sub ID_Nuovo_Click(sender As Object, e As System.Web.UI.ImageClickEventArgs) Handles ID_Nuovo.Click
        Toolbar_Nuovo()
    End Sub


    ''##################################################################################
    'Private Sub ID_Cancella_Click(sender As Object, e As System.Web.UI.ImageClickEventArgs) Handles ID_Cancella.Click
    '    Verifica_Cancellazione()
    'End Sub


    '##################################################################################
    Private Sub ID_Modifica_Click(sender As Object, e As System.Web.UI.ImageClickEventArgs) Handles ID_Modifica.Click
        Toolbar_Modifica()
    End Sub


    '##################################################################################
    Private Sub ID_Annulla_Click(sender As Object, e As System.Web.UI.ImageClickEventArgs) Handles ID_Annulla.Click

        Pannello_Inserimento.Visible = False

        Pulisci_Panel_InsertModifica()
    End Sub

    


    '##################################################################################
    Private Sub CaricamentoCorpiEstranei()

        Dim objCorpoEstraneo As New AgronicaCoreAnagrafeDAL.CorpiEstranei_R

        Dim DT As DataTable = objCorpoEstraneo.Leggi(0,
                                                     enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                                     "", "", objParametri_Server)

        If DT Is Nothing  OrElse DT.Rows.Count = 0 Then
            Messaggi.AgroMsgBox("Non sono stati trovati corpi estranei!", Page)
        End If

        'Vettore di DataColumn
        Dim DtKeysCE(0) As String
        DtKeysCE(0) = "Cod_CorpoEstraneo"

        DataGrid_CE.DataSource = DT
        DataGrid_CE.DataKeyNames = DtKeysCE
        DataGrid_CE.DataBind()

    End Sub


    '##################################################################################
    Private Sub Pulisci_Panel_InsertModifica()

        Me.Txt_DescCE.Text = ""
        Me.Txt_LimiteMaxAero.Text = ""
        Me.Txt_LimiteMaxCO.Text = ""
        Me.Txt_LimiteMaxCM.Text = ""
        Me.Txt_Cod_CorpoEstraneo.Text = ""

    End Sub


    '################################################################àà
    Private Sub Toolbar_Nuovo()
        
        Pulisci_Panel_InsertModifica()

        Pannello_Inserimento.Visible = True
        Me.Txt_Cod_CorpoEstraneo.Visible = False
        Me.Txt_DescCE.Enabled = True
        Me.lbl_inserisci_modifica.InnerText = "Inserisci il nome del Corpo Estraneo:"

        Me.Txt_Cod_CorpoEstraneo.Text = -1

    End Sub


    '##################################################################################
    Private Sub CancellaCE()

        'per ogni riga che è attiva effettuo la cancellazione 
        Dim i As Integer
        Dim objCorpiEstraneiW As New AgronicaCoreAnagrafeBIZ.CorpiEstranei_W
        Dim objMovimentiDettagli As New AgronicaCoreContabDAL.Movimenti_Dettagli_R
        Dim DT As DataTable
        Dim xRisp As Boolean

        For i = 0 To DataGrid_CE.Rows.Count - 1

            If CType(DataGrid_CE.Rows(i).FindControl("ChkSelezionaCE"), CheckBox).Checked = True Then

                'come prima cosa controllo se il codice è già stato usato 
                DT = objMovimentiDettagli.Leggi(Qs_Piva, 0, 0, 0, 0,
                                                -50, DataGrid_CE.DataKeys(i).Item(0), 0,
                                                "", 0, 0, 0, 0, 0, 0,
                                                enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                " udm_Cod=38 ", "",
                                                objParametri_Server)

                If DT.Rows.Count > 0 Then
                    Messaggi.AgroMsgBox("Non si può eliminare il corpo estraneo selezionato poiché è già stato utilizzato!", Page)
                Else
                    'non va bene, perché bisogna cancellare il corpo estraneo in entrambe le tabelle
                    'objCorpiEstraneiW.Cancella(CInt(Me.DataGrid_CE.Items(i).Cells(1).Text), "", objParametri_Server)

                    xRisp = objCorpiEstraneiW.CorpoEstraneo_Cancella(Qs_Piva,
                                                                     DataGrid_CE.DataKeys(i).Item(0),
                                                                     "", "",
                                                                     objParametri_Server)

                End If

            End If

        Next

        'riaggiorno
        CaricamentoCorpiEstranei()
        Me.SI_NO.Value = "0"

    End Sub


    '##################################################################################
    Private Sub Toolbar_Modifica()

        Pulisci_Panel_InsertModifica()

        ' Pannello_Inserimento.Visible = True
        Me.Txt_Cod_CorpoEstraneo.Visible = False
        Me.Txt_DescCE.Enabled = False
        Me.lbl_inserisci_modifica.InnerText = "Modifica del Corpo Estraneo:"

        If Visualizza_Limiti() Then
            Pannello_Inserimento.Visible = True
        End If


    End Sub

    '##################################################################################
    Private Function Visualizza_Limiti() As Boolean

        'conto il numero di elementi selezionati
        Dim j As Integer = 0
        Dim Str_CE As String = ""

        For i As Integer = 0 To DataGrid_CE.Rows.Count - 1
            If CType(DataGrid_CE.Rows(i).FindControl("ChkSelezionaCE"), CheckBox).Checked = True Then
                j += 1
                Str_CE = DataGrid_CE.DataKeys(i).Item(0)
            End If
        Next
        If j > 1 Then
            Messaggi.AgroMsgBox("Selezionare un solo corpo estraneo!", Page)
            Return False
        End If

        Dim objCe As New AgronicaCoreAnagrafeDAL.CorpiEstranei_R
        Dim DT As DataTable = objCe.Leggi(Str_CE, enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                          "", "", objParametri_Server)

        If Dt IsNot Nothing AndAlso DT.Rows.Count > 0 Then
            Txt_Cod_CorpoEstraneo.Text = DT.Rows(0).Item("Cod_CorpoEstraneo")
            Txt_DescCE.Text = DT.Rows(0).Item("Desc_CorpoEstraneo")
            Txt_LimiteMaxAero.Text = DT.Rows(0).Item("LimiteMax_Aeroseparatori")
            Txt_LimiteMaxCO.Text = DT.Rows(0).Item("LimiteMax_CernitriciOttiche")
            Txt_LimiteMaxCM.Text = DT.Rows(0).Item("LimiteMax_CernitaManuale")
        End If

        Return True

    End Function

    '##################################################################################
    Private Function Verifica_Cancellazione() As Boolean

        Dim bRet As Boolean = True
        'conto il numero di elementi selezionati
        Dim j As Integer = 0
        Dim Str_CE As String

        For i As Integer = 0 To DataGrid_CE.Rows.Count - 1
            If CType(DataGrid_CE.Rows(i).FindControl("ChkSelezionaCE"), CheckBox).Checked = True Then
                j += 1
                Str_CE = DataGrid_CE.DataKeys(i).Item(0)
            End If
        Next
        If j > 1 Then
            Messaggi.AgroMsgBox("Selezionare un solo elemento!", Page)
            bRet = False
        End If

        Return bRet

    End Function


    '##################################################################################
    Private Sub SalvaModifiche()
        'procedura di write
        'controllo la stringa

        Dim descrizione As String
        descrizione = Txt_DescCE.Text

        If descrizione <> "" Then

            If Not IsNumeric(Txt_LimiteMaxAero.Text) Then
                Messaggi.AgroMsgBox("Limite Aeroseparatore non numerico!", Page)
                Exit Sub
            End If

            If Not IsNumeric(Txt_LimiteMaxCO.Text) Then
                Messaggi.AgroMsgBox("Limite Cernitrice Ottica non numerico!", Page)
                Exit Sub
            End If

            If Not IsNumeric(Txt_LimiteMaxCM.Text) Then
                Messaggi.AgroMsgBox("Limite Cernitrice Manuale non numerico!", Page)
                Exit Sub
            End If

            ' se non è già inserito posso inserirlo
            Dim obj_CELimitiW As New AgronicaCoreAnagrafeDAL.CorpiEstranei_LimitiPericolosita_W

            obj_CELimitiW.Modifica(Qs_Piva,
                                   Txt_Cod_CorpoEstraneo.Text,
                                   1,
                                   Txt_LimiteMaxAero.Text,
                                   Txt_LimiteMaxCO.Text,
                                   Txt_LimiteMaxCM.Text,
                                    "", objParametri_Server)


            CaricamentoCorpiEstranei()

            Pannello_Inserimento.Visible = False

        Else
            Messaggi.AgroMsgBox("E' necessario specificare la descrizione del corpo estraneo!", Page)
        End If


    End Sub

    '#################################################################################################################
    Private Sub btn_delete_riga_Click(sender As Object, e As System.EventArgs) Handles btn_delete_riga.Click

        If Not Verifica_Cancellazione() Then
            Exit Sub
        End If

        'per ogni riga che è attiva effettuo la cancellazione 
        Dim objCorpiEstraneiW As New AgronicaCoreAnagrafeBIZ.CorpiEstranei_W
        Dim objMovimentiDettagli As New AgronicaCoreContabDAL.Movimenti_Dettagli_R
        Dim DT As DataTable
        Dim xRisp As Boolean

        For i As Integer = 0 To DataGrid_CE.Rows.Count - 1

            If CType(DataGrid_CE.Rows(i).FindControl("ChkSelezionaCE"), CheckBox).Checked = True Then

                'come prima cosa controllo se il codice è già stato usato 
                DT = objMovimentiDettagli.Leggi(Qs_Piva, 0, 0, 0, 0,
                                                -50, DataGrid_CE.DataKeys(i).Item(0), 0,
                                                "", 0, 0, 0, 0, 0, 0,
                                                enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                " udm_Cod=38 ", "",
                                                objParametri_Server)

                If DT.Rows.Count > 0 Then
                    Messaggi.AgroMsgBox("Non si può eliminare il corpo estraneo selezionato poiché è già stato utilizzato!", Page)
                Else
                    'non va bene, perché bisogna cancellare il corpo estraneo in entrambe le tabelle
                    xRisp = objCorpiEstraneiW.CorpoEstraneo_Cancella(Qs_Piva,
                                                                     CInt(DataGrid_CE.DataKeys(i).Item(0)),
                                                                     "", "",
                                                                     objParametri_Server)
                End If

            End If

        Next

        'riaggiorno
        CaricamentoCorpiEstranei()

    End Sub


    '#################################################################################################################
    Private Sub ID_Salva_Click(sender As Object, e As System.Web.UI.ImageClickEventArgs) Handles ID_Salva.Click

        If Txt_Cod_CorpoEstraneo.Text = -1 Then
            AggiungiCE()
        Else
            SalvaModifiche()
        End If

    End Sub

    '##################################################################################
    Private Sub AggiungiCE()
        'procedura di write
        'controllo la stringa
        Dim descrizione As String = Txt_DescCE.Text

        If descrizione <> "" Then

            ' se ho inserito la descrizione controllo che non ce ne sia già una identica
            Dim objCorpiEstranei As New AgronicaCoreAnagrafeDAL.CorpiEstranei_R
            Dim DT As DataTable

            DT = objCorpiEstranei.EsisteCE(Trim(descrizione),
                                           enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                           objParametri_Server)

            If DT.Rows.Count > 0 Then
                Messaggi.AgroMsgBox("Il corpo estraneo specificato è già stato inserito!", Page)
                Exit Sub
            End If
            Dim LimAE, LimCO, LimCM As Integer
            If Not IsNumeric(Txt_LimiteMaxAero.Text) Then
                Messaggi.AgroMsgBox("Limite Aeroseparatore non numerico!", Page)
                Exit Sub
            Else
                LimAE = Txt_LimiteMaxAero.Text
            End If

            If Not IsNumeric(Txt_LimiteMaxCO.Text) Then
                Messaggi.AgroMsgBox("Limite Cernitrice Ottica non numerico!", Page)
                Exit Sub
            Else
                LimCO = Txt_LimiteMaxCO.Text
            End If

            If Not IsNumeric(Txt_LimiteMaxCM.Text) Then
                Messaggi.AgroMsgBox("Limite Cernitrice Manuale non numerico!", Page)
                Exit Sub
            Else
                LimCM = Txt_LimiteMaxCM.Text
            End If


            ' se non è già inserito posso inserirlo
            Dim objCEW As New AgronicaCoreAnagrafeBIZ.CorpiEstranei_W
            Dim Risp As Boolean
            Dim OUTPUT_Cod_CorpoEstraneo As Integer

            Risp = objCEW.CorpoEstraneo_Scrivi(Qs_Piva,
                                               Trim(descrizione),
                                               1,
                                               "Alta",
                                               LimAE,
                                               LimCO,
                                               LimCM,
                                               AGRODATAINIZIO,
                                               AGRODATAFINE,
                                               OUTPUT_Cod_CorpoEstraneo,
                                               objParametri_Server)

            CaricamentoCorpiEstranei()

            Pannello_Inserimento.Visible = False

        Else
            Messaggi.AgroMsgBox("Per inserire un nuovo corpo estraneo è necessario specificare la descrizione!", Page)
        End If

    End Sub

End Class
