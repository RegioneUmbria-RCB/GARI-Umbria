Imports AgronicaCoreContabDAL
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreGestioneRichieste
Imports AgronicaCoreUtility
Imports AgronicaCoreDataProvider.My.Resources

Public Class FormProdottoUC
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
            Dim hdPiva As HtmlInputHidden = Parent.Parent.FindControl("hdPiva")
            Dim hf_Qs_Key As HtmlInputHidden = Parent.FindControl("hf_Qs_Key")
            Dim hdIdAgenda As HtmlInputHidden = Parent.Parent.FindControl("hdIdAgenda")
            Dim hdTipoOp As HtmlInputHidden = Parent.Parent.FindControl("hdTipoOp")
            Dim hf_Qs_SaCod As HtmlInputHidden = Parent.Parent.FindControl("hf_Qs_SaCod")
            Dim hf_Qs_ElemCod As HtmlInputHidden = Parent.FindControl("hf_Qs_ElemCod")
            Dim hdLavCod As HtmlInputHidden = Parent.Parent.FindControl("hdLavCod")
            Dim hf_Qs_DataSelezionata As HtmlInputHidden = Parent.Parent.FindControl("hf_Qs_DataSelezionata")
            Dim hf_Qs_Mode As HtmlInputHidden = Parent.FindControl("hf_Qs_Mode")


            Dim hf_Qs_CaricoScarico As HtmlInputHidden = Parent.Parent.FindControl("hf_Qs_CaricoScarico")
            Dim hf_ChkAccompagnatoria As HtmlInputHidden = Parent.Parent.FindControl("hf_ChkAccompagnatoria")

            Dim objAgroWebConfig As New AgroWebConfig
            If objAgroWebConfig.GiasOnline_WS_Fitofarmaci_AgroWS_Fitofarmaci = "" Then
                hf_LinkWsFitofarmaci.Value = Server.MapPath("https://ws.netagronica.it/AgronicaWebService/AgroWS_Fitofarmaci.asmx")
            Else
                hf_LinkWsFitofarmaci.Value = objAgroWebConfig.GiasOnline_WS_Fitofarmaci_AgroWS_Fitofarmaci
            End If

            '##############################################################

            Dim AggiuntoAnimale As Boolean = False
            Dim xCampo_Cod As Integer
            Dim xAppezza As Integer
            Dim xID_Imp As Integer
            Dim xCodFiscale As String
            Dim xTipoNodo As enum_TipoNodo

            Call Albero.ChiaveAlbero_Decodifica_ImpiantiVegetali_x_json(
                                hf_Qs_Key.Value,
                                xTipoNodo,
                                xPiva,
                                xSa_Cod,
                                xCampo_Cod,
                                xAppezza,
                                xID_Imp,
                                xCodFiscale,
                                xFabbricato_Cod)

            '##############################################################
            '#####################  PERMESSI  #############################
            '##############################################################

            '----- Verifico se l'utente dispone dei permessi per la visualizzazione della pagina

            Dim UtenteAbilitato As Boolean
            Dim acUtenti As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
            Dim tipoOpPagina As enum_Security_Operazione

            Select Case CInt(hdTipoOp.Value)

                Case enum_TipoOperazioneDB.Lettura

                    tipoOpPagina = enum_Security_Operazione.Lettura

                Case enum_TipoOperazioneDB.Modifica, enum_TipoOperazioneDB.Scrittura

                    tipoOpPagina = enum_Security_Operazione.Modifica

                Case enum_TipoOperazioneDB.Cancellazione

                    tipoOpPagina = enum_Security_Operazione.Cancellazione

            End Select

            UtenteAbilitato = acUtenti.Controlla_Permessi_Utente(Session("ASG_Utente_Username"),
                                                                 Session("ASG_IdServizio"),
                                                                 enum_Security_Attivita.Gest_Magazzino,
                                                                 tipoOpPagina,
                                                                 Now, "", objParametri_Utenti)

            If UtenteAbilitato = False Then

                PremutoAnnulla = True

                'TODO AAA_GestioneUscitaPagina()

                Throw New Exception(String.Format("L'Utente non possiede i permessi per gestire il magazzino in {0}", tipoOpPagina.ToString()))

                Exit Sub

            End If

            '##############################################################

            'Controllo hidden per il salvataggio dell'indirizzo del profitosan recuperato da web config
            objAgroWebConfig = New AgroWebConfig
            hf_indirizzoProfitosan.Value = profitosan.getLink2023(True, New AgroWebConfig, "",
                                                                  objParametri_Server, objParametri_Utenti)

            '##############################################################
            'Prepara categorie di magazzino su JSon e imposta il default

            If Not Page.IsPostBack Then

                If hdLavCod.Value = LAVCOD_ACCETTAZIONE_DIVERSI OrElse
                   hdLavCod.Value = LAVCOD_DISTINTA_CARICO_ACCETTAZIONE OrElse
                   hdLavCod.Value = LAVCOD_AUTO_DDT_EMESSO OrElse
                   hdLavCod.Value = LAVCOD_AUTO_DDT_EMESSO_ACCETTAZIONE OrElse
                   hdLavCod.Value = LAVCOD_DISTINTA_CARICO Then
                    Dim objImp As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_FiltroMono_R
                    hf_filtroMateriePrimeConferimento.Value = objImp.RecuperaFiltroSQLMatPrime_from_AreaGIAS(enum_AreaGIAS.Conferimento, objParametri_Utenti)
                End If
                '26/11/2020: aggiunta lettura del filtro da applicare alle materie prime x il conferimento

                'In caso di F&F / acquisto  non mostreremo TRASFORMATI_VEGETALI fra le categorie prodotto possibili 
                '(in questo caso il prodotto deve entrare da ACCETTAZIONE)con dft TRASFORMATI_VEGETALI 
                'Quindi non lo imposteremo neanche come default

                Dim w_scartaTrasformatiVegetaliInAcquisto = False
                Dim w_scartaTrasformatiAnimaliInAcquisto = False

                'Modulo di appartenenza
                Dim leggi_anagrafe_log As New OGenerazioni_Anagrafe_Moduli_Log_R
                Dim dtAnagrafeLog = leggi_anagrafe_log.Leggi(hdPiva.Value, 0, 0, 0, 0, -1, -1, -1, AGRODATAINIZIO, AGRODATAFINE,
                               "", "",
                               objParametri_Server)
                Dim listModuliAttivi_anagrafe_log As New List(Of enum_Omni_Modulo_Generazione)
                For Each r In dtAnagrafeLog.Rows
                    listModuliAttivi_anagrafe_log.Add(CInt(r.Item("Modulo_Generazione")))
                Next
                dtAnagrafeLog = Nothing

                'TODO U.M.
                Select Case hdLavCod.Value

                    'TODO Completare con quelli mancanti
                    Case LAVCOD_FATTURA_RICEVUTA,
                         LAVCOD_BOLLA_RICEVUTA,
                         LAVCOD_NOTA_ACCREDITO_EMESSA,
                         LAVCOD_CARICO

                        If listModuliAttivi_anagrafe_log.Contains(enum_Omni_Modulo_Generazione.FreshFood) OrElse
                           listModuliAttivi_anagrafe_log.Contains(enum_Omni_Modulo_Generazione.Tabacco) Then
                            w_scartaTrasformatiVegetaliInAcquisto = True
                        End If

                        If listModuliAttivi_anagrafe_log.Contains(enum_Omni_Modulo_Generazione.Zoo) Then
                            w_scartaTrasformatiAnimaliInAcquisto = True
                        End If

                End Select

                Dim cmb_Categoria As New DropDownList

                'Se ho impostato un filtro sulle categorie...
                Dim Dt_Impost As DataTable
                Dim objImpost As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read

                Dt_Impost = objImpost.Leggi2(1,
                                             Session("ASG_Utente_Username"),
                                             0,
                                             "",
                                             "",
                                             objParametri_Utenti)

                Dim DrFiltroElemCod() As DataRow = Dt_Impost.Select("Impostazione_Cod=" & enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_CATEGORIAMAGAZZINO)

                Dim DrFiltro() As DataRow = Dt_Impost.Select("Impostazione_Cod=" & enum_Impostazioni_Utenti.UTENTE_COD_CATEGORIAMAGAZZINO_DEFAULT)

                'INIZIO composizione filtro aggiuntivo
                Dim strElem_Cod As String = ""

                If Not DrFiltroElemCod Is Nothing AndAlso DrFiltroElemCod.Length > 0 Then
                    strElem_Cod = DrFiltroElemCod(0).Item("Impostazione_Valore_1")
                    strElem_Cod = Replace(strElem_Cod, "|", ",")
                End If

                If strElem_Cod <> "" Then
                    strElem_Cod = " Elem_Cod IN (" & strElem_Cod & ") "
                Else
                    strElem_Cod = "Elem_Cod <> " & COADIUVANTI.ToString
                End If

                Dim w_flag_altri_beni = True

                'Filtri categorie in base a lavCod
                Select Case hdLavCod.Value

                    Case LAVCOD_ACCETTAZIONE_DIVERSI,
                         LAVCOD_DISTINTA_CARICO_ACCETTAZIONE,
                         LAVCOD_AUTO_DDT_EMESSO,
                         LAVCOD_AUTO_DDT_EMESSO_ACCETTAZIONE,
                         LAVCOD_DISTINTA_CARICO

                        strElem_Cod = String.Format("({0} AND Elem_Cod IN ({1}, {2}))",
                                                    strElem_Cod,
                                                    TRASFORMATI_VEGETALI,
                                                    TRASFORMATI_ANIMALI)

                    Case LAVCOD_CONTRATTO_AFFITTO

                        strElem_Cod = String.Format("({0} AND Elem_Cod = {1})",
                            strElem_Cod,
                            ALTRE_MATERIE)

                        w_flag_altri_beni = False

                End Select

                'INIZIO: Si vogliono fare entrare questi prodotti solo dal conferimento

                Dim strScarta = ""

                'Scarto sempre ALTRI BENI AMMORTIZZABILI e CONFEZIONI PRODOTTI
                strScarta = "Elem_Cod NOT IN (" & ALTRI_BENI_AMMORTIZZABILI.ToString & "," & CONFEZIONI_PRODOTTI.ToString

                'Si vogliono fare entrare questi prodotti solo dal conferimento
                If w_scartaTrasformatiVegetaliInAcquisto OrElse w_scartaTrasformatiAnimaliInAcquisto Then
                    If w_scartaTrasformatiVegetaliInAcquisto Then
                        strScarta += "," & TRASFORMATI_VEGETALI.ToString
                    End If
                    If w_scartaTrasformatiAnimaliInAcquisto Then
                        strScarta += "," & TRASFORMATI_ANIMALI.ToString
                    End If
                End If

                strScarta += ")"

                If strElem_Cod <> "" Then
                    strElem_Cod = " ((" & strElem_Cod & ") And " & strScarta & " ) "
                Else
                    strElem_Cod = " ( " & strScarta & " ) "
                End If

                'FINE: Si vogliono fare entrare questi prodotti solo dal conferimento

                'FINE composizione filtro aggiuntivo

                'Conferimenti: utilizzo solo TRASFORMATI VEGETALI/ANIMALI
                Dim w_default_elem_cod = 0
                If hdLavCod.Value = LAVCOD_ACCETTAZIONE_DIVERSI OrElse
                   hdLavCod.Value = LAVCOD_DISTINTA_CARICO_ACCETTAZIONE OrElse
                   hdLavCod.Value = LAVCOD_AUTO_DDT_EMESSO OrElse
                   hdLavCod.Value = LAVCOD_AUTO_DDT_EMESSO_ACCETTAZIONE OrElse
                   hdLavCod.Value = LAVCOD_DISTINTA_CARICO Then

                    If listModuliAttivi_anagrafe_log.Contains(enum_Omni_Modulo_Generazione.FreshFood) AndAlso
                            Not listModuliAttivi_anagrafe_log.Contains(enum_Omni_Modulo_Generazione.Zoo) Then
                        w_default_elem_cod = TRASFORMATI_VEGETALI
                    End If
                    If listModuliAttivi_anagrafe_log.Contains(enum_Omni_Modulo_Generazione.Zoo) AndAlso
                            Not listModuliAttivi_anagrafe_log.Contains(enum_Omni_Modulo_Generazione.FreshFood) Then
                        w_default_elem_cod = TRASFORMATI_ANIMALI
                    End If

                    w_flag_altri_beni = False

                Else

                    'Se l'impostazione utente è stata specificata, verifico la presenza della categoria ALTRI_BENI
                    If Not DrFiltroElemCod Is Nothing AndAlso DrFiltroElemCod.Length > 0 Then

                        Dim impUtenteCatMag As String = DrFiltroElemCod(0).Item("Impostazione_Valore_1")
                        Dim listaUtenteCatMag As String() = impUtenteCatMag.Split("|")

                        w_flag_altri_beni = listaUtenteCatMag.Contains(ALTRI_BENI)
                    End If

                End If

                'Caricamento categorie magazzino
                Call CaricaListControl.CategorieMagazzino(cmb_Categoria,
                                                          True,
                                                          "",
                                                          0,
                                                          0,
                                                          hf_Qs_CaricoScarico.Value,
                                                          hdLavCod.Value,
                                                          False,
                                                          w_flag_altri_beni,
                                                          strElem_Cod,
                                                          "",
                                                          objParametri_Server)

                'Se ho impostato una categoria di default nelle impostazioni utente la imposto
                Dim Impostazione_Valore_1 As String = ""
                If w_default_elem_cod <> 0 Then
                    cmb_Categoria.SelectedIndex = cmb_Categoria.Items.IndexOf(cmb_Categoria.Items.FindByValue(w_default_elem_cod))
                Else
                    If Not DrFiltro Is Nothing AndAlso DrFiltro.Length > 0 Then
                        Impostazione_Valore_1 = DrFiltro(0).Item("Impostazione_Valore_1")
                        Select Case Impostazione_Valore_1
                            Case Is <> ""
                                If Not w_scartaTrasformatiVegetaliInAcquisto OrElse
                                    Impostazione_Valore_1 <> TRASFORMATI_VEGETALI Then
                                    cmb_Categoria.SelectedIndex = cmb_Categoria.Items.IndexOf(cmb_Categoria.Items.FindByValue(Impostazione_Valore_1))
                                End If
                        End Select
                    End If
                End If

                Dim strElemCodUdmCod As String
                Dim ArrayElemCodUdmCod() As String = Nothing
                Dim DrFiltroElemCodUdmCod() As DataRow = Dt_Impost.Select("Impostazione_Cod=" & enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_CATEGORIAMAGAZZINO_UDM_DEFAULT)

                If Not DrFiltroElemCodUdmCod Is Nothing AndAlso DrFiltroElemCodUdmCod.Length > 0 Then
                    strElemCodUdmCod = DrFiltroElemCodUdmCod(0).Item("Impostazione_Valore_1")
                    ArrayElemCodUdmCod = Split(strElemCodUdmCod, "|")
                End If

                Dim JsonString As New StringBuilder()

                If Not ArrayElemCodUdmCod Is Nothing Then

                    JsonString.Append(" [ ")

                    For i = 0 To ArrayElemCodUdmCod.Length - 1

                        If i > 0 Then
                            JsonString.Append(",")
                        End If

                        JsonString.Append("{")

                        JsonString.Append(" ""Elem_Cod"": " & ArrayElemCodUdmCod(i).Split("_")(0) & ",")
                        JsonString.Append(" ""Udm_Cod"": " & ArrayElemCodUdmCod(i).Split("_")(1))

                        JsonString.Append("}")

                    Next

                    JsonString.Append(" ] ")

                End If

                hf_ArrayElemCodUdmCod.Value = JsonString.ToString

                'Se provengo da qualche operazione d'agenda...
                If CInt(hf_Qs_ElemCod.Value) <> 0 Then
                    If Not w_scartaTrasformatiVegetaliInAcquisto OrElse Impostazione_Valore_1 <> TRASFORMATI_VEGETALI Then
                        'Imposto la posizione nella combo 
                        cmb_Categoria.SelectedIndex = cmb_Categoria.Items.IndexOf(cmb_Categoria.Items.FindByValue(CInt(hf_Qs_ElemCod.Value)))
                    End If
                End If

                'Memorizzo le categorie in un hidden field
                JsonString = New StringBuilder()
                JsonString.Append("[")

                'Carico categorie lette
                For Each elem In cmb_Categoria.Items
                    If Not JsonString.ToString = "[" Then
                        JsonString.Append(", ")
                    End If
                    JsonString.Append("{")
                    JsonString.Append(" ""Elem_Cod"": """ & DirectCast(elem, System.Web.UI.WebControls.ListItem).[Value] & """,")
                    JsonString.Append(" ""NomeComune"": """ & DirectCast(elem, System.Web.UI.WebControls.ListItem).[Text] & """ ")
                    JsonString.Append("}")
                Next

                'Aggiungo eventualmente la riga descrizione libera
                Dim w_flag_descrizione_libera = True

                If hdLavCod.Value = LAVCOD_CONTRATTO_AFFITTO Then
                    w_flag_descrizione_libera = False
                    cmb_Categoria.SelectedIndex = cmb_Categoria.Items.IndexOf(cmb_Categoria.Items.FindByValue(ALTRE_MATERIE.ToString))
                End If

                If w_flag_descrizione_libera Then
                    If Not JsonString.ToString = "[" Then
                        JsonString.Append(", ")
                    End If

                    JsonString.Append("{")
                    JsonString.Append(" ""Elem_Cod"": """ & RIGA_DESCRIZIONE_LIBERA.ToString & """,")
                    JsonString.Append(" ""NomeComune"": """ & Gias.RigaDescrizioneLibera & """ ")
                    JsonString.Append("}")
                End If

                JsonString.Append(" ] ")

                hf_Categorie_Magazzino.Value = JsonString.ToString
                hf_Categoria_Magazzino_Dft.Value = cmb_Categoria.SelectedValue

            End If

        Catch ex As Exception

            Messaggio = "Si e' verificato un'errore : " & Chr(13) & ex.Message.ToString()

            Call Messaggi.AgroMsgBox(Messaggio, Page, "Form1")

        End Try

    End Sub

    Private Sub inizializzoObjParametri()
        '---
        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))
        objParametri_Utenti = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))
        '---
    End Sub

    Private Shared Function gettimesep() As String
        Return System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.TimeSeparator
    End Function

End Class
