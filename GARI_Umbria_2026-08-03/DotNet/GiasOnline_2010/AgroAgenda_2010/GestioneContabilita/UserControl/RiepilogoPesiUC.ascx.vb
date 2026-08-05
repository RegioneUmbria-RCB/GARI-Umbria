Imports System.Web.Services
Imports AgronicaCoreContabBIZ
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreUtility
Imports AgronicaCoreModello.ParametriAgenda_Temp
Imports AgronicaCoreGestioneRichieste
Imports AgronicaCoreModello

Public Class RiepilogoPesiUC
    Inherits System.Web.UI.UserControl

    '----- Gestione della pagina transazionale
    Dim EseguitaOperazione As Boolean
    Dim PremutoAnnulla As Boolean

    '----- variabili globali
    Dim Operazione As Integer
    Dim Messaggio As String = ""


    Dim xCaricoScarico As Integer

    Dim xPiva As String
    Dim xSa_Cod As Integer
    Dim xFabbricato_Cod As Integer
    Dim xAgenda As String

    Dim xElem_Cod As Integer
    Dim xMat_Cod As Integer
    Dim xCod_Progetto As Integer
    Dim xLotto As String
    Dim xCod_Calibro As Integer
    Dim xUdm_Cod As Integer
    Dim xFase_Cod As Integer

    Dim objParametri_Server, objParametri_Utenti As AgronicaCoreParametri

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        inizializzoObjParametri()

        Try
            '    Dim hdPiva As HtmlInputHidden = Parent.Parent.FindControl("hdPiva")
            '    Dim hf_Qs_Key As HtmlInputHidden = Parent.FindControl("hf_Qs_Key")
            '    Dim hdIdAgenda As HtmlInputHidden = Parent.Parent.FindControl("hdIdAgenda")
            '    Dim hdTipoOp As HtmlInputHidden = Parent.Parent.FindControl("hdTipoOp")
            '    Dim hf_Qs_SaCod As HtmlInputHidden = Parent.Parent.FindControl("hf_Qs_SaCod")
            '    Dim hf_Qs_ElemCod As HtmlInputHidden = Parent.FindControl("hf_Qs_ElemCod")
            '    Dim hdLavCod As HtmlInputHidden = Parent.Parent.FindControl("hdLavCod")
            '    Dim hf_Qs_DataSelezionata As HtmlInputHidden = Parent.Parent.FindControl("hf_Qs_DataSelezionata")
            '    Dim hf_Qs_Mode As HtmlInputHidden = Parent.FindControl("hf_Qs_Mode")
            '    Dim hf_Qs_CaricoScarico As HtmlInputHidden = Parent.Parent.FindControl("hf_Qs_CaricoScarico")

            '    '##############################################################

            '    Dim AggiuntoAnimale As Boolean = False
            '    Dim xCampo_Cod As Integer
            '    Dim xAppezza As Integer
            '    Dim xID_Imp As Integer
            '    Dim xCodFiscale As String
            '    Dim xTipoNodo As enum_TipoNodo

            '    Call AgronicaCoreDataProvider.Albero.ChiaveAlbero_Decodifica_ImpiantiVegetali_x_json(
            '                        hf_Qs_Key.Value,
            '                        xTipoNodo,
            '                        xPiva,
            '                        xSa_Cod,
            '                        xCampo_Cod,
            '                        xAppezza,
            '                        xID_Imp,
            '                        xCodFiscale,
            '                        xFabbricato_Cod)



            '    '##############################################################
            '    '#####################  PERMESSI  #############################
            '    '##############################################################

            '    '----- Verifico se l'utente dispone dei permessi per la visualizzazione della pagina

            '    Dim UtenteAbilitato As Boolean
            '    Dim acUtenti As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
            '    Select Case CInt(hdTipoOp.Value)

            '        Case enum_TipoOperazioneDB.Lettura

            '            UtenteAbilitato = acUtenti.Controlla_Permessi_Utente(
            '                                  Session("ASG_Utente_Username"),
            '                                  Session("ASG_IdServizio"),
            '                                  enum_Security_Attivita.Gest_Magazzino,
            '                                  enum_Security_Operazione.Lettura,
            '                                  Now, "", objParametri_Utenti
            '                                  )



            '        Case enum_TipoOperazioneDB.Modifica, enum_TipoOperazioneDB.Scrittura


            '            UtenteAbilitato = acUtenti.Controlla_Permessi_Utente(
            '                                  Session("ASG_Utente_Username"),
            '                                  Session("ASG_IdServizio"),
            '                                  enum_Security_Attivita.Gest_Magazzino,
            '                                  enum_Security_Operazione.Modifica,
            '                                  Now, "", objParametri_Utenti
            '                                  )

            '        Case enum_TipoOperazioneDB.Cancellazione


            '            UtenteAbilitato = acUtenti.Controlla_Permessi_Utente(
            '                                  Session("ASG_Utente_Username"),
            '                                  Session("ASG_IdServizio"),
            '                                  enum_Security_Attivita.Gest_Magazzino,
            '                                  enum_Security_Operazione.Cancellazione,
            '                                  Now, "", objParametri_Utenti
            '                                  )


            '    End Select


            '    If UtenteAbilitato = False Then

            '        PremutoAnnulla = True

            '        'TODO AAA_GestioneUscitaPagina()

            '        Exit Sub

            '    End If

            '    '##############################################################
            '    ' Prepara categorie di magazzino su JSon e imposta il default

            '    If Not Page.IsPostBack Then
            '        Dim cmb_Categoria As New DropDownList

            '        'se ho impostato un filtro sulle categorie ......
            '        Dim Dt_Impost As DataTable
            '        Dim objImpost As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
            '        Dim strElem_Cod As String = ""
            '        Dt_Impost = objImpost.Leggi2(1, Session("ASG_Utente_Username"), 0,
            '                                            "", "",
            '                                            objParametri_Utenti)
            '        Dim DrFiltroElemCod() As DataRow = Dt_Impost.Select("Impostazione_Cod=" & enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_CATEGORIAMAGAZZINO)

            '        Dim DrFiltro() As DataRow = Dt_Impost.Select("Impostazione_Cod=" & enum_Impostazioni_Utenti.UTENTE_COD_CATEGORIAMAGAZZINO_DEFAULT)

            '        If Not DrFiltroElemCod Is Nothing AndAlso DrFiltroElemCod.Length > 0 Then
            '            strElem_Cod = DrFiltroElemCod(0).Item("Impostazione_Valore_1")
            '            strElem_Cod = Replace(strElem_Cod, "|", ",")
            '        End If
            '        If strElem_Cod <> "" Then
            '            strElem_Cod = " Elem_Cod IN (" & strElem_Cod & ") "
            '        Else
            '            strElem_Cod = "Elem_Cod <> " & COADIUVANTI.ToString
            '        End If

            '        Call AgronicaCoreUtility.CaricaListControl.CategorieMagazzino(
            '                                           cmb_Categoria,
            '                                            True, "", 0,
            '                                            0,
            '                                             hf_Qs_CaricoScarico.Value,
            '                                             hdLavCod.Value,
            '                                             False,
            '                                             True,
            '                                             strElem_Cod, "", objParametri_Server)


            '        'se ho impostato una categoria di default nelle impostazioni utente la imposto
            '        Dim Impostazione_Valore_1 As String = ""
            '        If Not DrFiltro Is Nothing AndAlso DrFiltro.Length > 0 Then
            '            Impostazione_Valore_1 = DrFiltro(0).Item("Impostazione_Valore_1")
            '            Select Case Impostazione_Valore_1
            '                Case Is <> ""
            '                    cmb_Categoria.SelectedIndex = cmb_Categoria.Items.IndexOf(
            '                                                  cmb_Categoria.Items.FindByValue(
            '                                                                Impostazione_Valore_1))

            '            End Select
            '        End If


            '        Dim strElemCodUdmCod As String
            '        Dim ArrayElemCodUdmCod() As String
            '        Dim DrFiltroElemCodUdmCod() As DataRow = Dt_Impost.Select("Impostazione_Cod=" & enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_CATEGORIAMAGAZZINO_UDM_DEFAULT)
            '        If Not DrFiltroElemCodUdmCod Is Nothing AndAlso DrFiltroElemCodUdmCod.Length > 0 Then
            '            strElemCodUdmCod = DrFiltroElemCodUdmCod(0).Item("Impostazione_Valore_1")
            '            ArrayElemCodUdmCod = Split(strElemCodUdmCod, "|")
            '        End If
            '        Dim JsonString As New StringBuilder()
            '        If Not ArrayElemCodUdmCod Is Nothing Then
            '            JsonString.Append(" [ ")
            '            For i = 0 To ArrayElemCodUdmCod.Length - 1
            '                JsonString.Append("{")

            '                JsonString.Append(" ""Elem_Cod"": " & ArrayElemCodUdmCod(i).Split("_")(0) & ",")
            '                JsonString.Append(" ""Udm_Cod"": " & ArrayElemCodUdmCod(i).Split("_")(1))

            '                JsonString.Append("}")

            '            Next
            '            JsonString.Append(" ] ")
            '        End If
            '        hf_ArrayElemCodUdmCod.Value = JsonString.ToString

            '        'se provengo da qualche operazione d'agenda...
            '        If CInt(hf_Qs_ElemCod.Value) <> 0 Then

            '            'Imposto la posizione nella combo 
            '            cmb_Categoria.SelectedIndex =
            '            cmb_Categoria.Items.IndexOf(
            '                cmb_Categoria.Items.FindByValue(
            '                    CInt(hf_Qs_ElemCod.Value)))

            '        End If

            '        JsonString = New StringBuilder()
            '        JsonString.Append("[")

            '        ' Memorizzo le categorie in un hidden field
            '        For Each elem In cmb_Categoria.Items
            '            If Not JsonString.ToString = "[" Then
            '                JsonString.Append(", ")
            '            End If
            '            JsonString.Append("{")
            '            JsonString.Append(" ""Elem_Cod"": """ & DirectCast(elem, System.Web.UI.WebControls.ListItem).[Value] & """,")
            '            JsonString.Append(" ""NomeComune"": """ & DirectCast(elem, System.Web.UI.WebControls.ListItem).[Text] & """ ")
            '            JsonString.Append("}")
            '        Next
            '        JsonString.Append(" ] ")

            '        hf_Categorie_Magazzino.Value = JsonString.ToString
            '        hf_Categoria_Magazzino_Dft.Value = cmb_Categoria.SelectedValue

            '    End If

        Catch ex As Exception

            'Messaggio di errore
            Messaggio = "Si e' verificato un'errore : " & Chr(13) & ex.Message.ToString()

            'Visualizzo il messaggio di errore
            Call Messaggi.AgroMsgBox(Messaggio, Page, "Form1")
            '------------------------------------------------

        End Try

    End Sub

    Private Sub inizializzoObjParametri()
        '---
        objParametri_Server = New AgronicaCoreParametri(Session("ASG_objParametri_Server"))
        objParametri_Utenti = New AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))
        '---
    End Sub

    Private Shared Function gettimesep() As String
        Return System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.TimeSeparator
    End Function

End Class
