Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreModelsSTD.anagrafiche
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreUtentiDAL
Imports AgronicaCoreAnagrafeBIZ.AnagrafeNG
Imports System.Web.UI.WebControls
Imports AgronicaCoreDataProvider.AgronicaCoreParametri

Public Class CentroAziendaleNG_R

    Public Function OttieneDropdownCentroEdit(piva As String,
                                              Sa_Cod As String,
                                              objParametri_Server As AgronicaCoreParametri) As AnagrafeNG.CentroDropdownLists
        Dim Tipologia As New Web.UI.WebControls.DropDownList()
        Dim TitoloPossesso As New Web.UI.WebControls.DropDownList()


        AgronicaCoreUtility.CaricaListControl.TipoCentro(CType(Tipologia, Web.UI.WebControls.ListControl), False, "SELEZIONA", 0, "", "", objParametri_Server)
        AgronicaCoreUtility.CaricaListControl.TitoloPossesso(TitoloPossesso, False, "", "", "", "", objParametri_Server)

        Dim CentroAziendaleEsternoCollegato As New Web.UI.WebControls.DropDownList()
        AgronicaCoreUtility.CaricaListControl.CentroEsterno(CentroAziendaleEsternoCollegato, True, "Nessun Centro Aziendale Esterno Collegato", "", piva, False, objParametri_Server)

        Dim Provincie As New Web.UI.WebControls.DropDownList()
        Dim Comune As New Web.UI.WebControls.DropDownList()
        AgronicaCoreUtility.CaricaListControl.NGGetProvincieAndComuni(
            Provincie, Comune, piva, Sa_Cod, objParametri_Server)



        Dim Stati As New Web.UI.WebControls.DropDownList()
        AgronicaCoreUtility.CaricaListControl.NGStati(Stati, objParametri_Server)

        Dim Codici As New Web.UI.WebControls.DropDownList()
        Dim StrCodiciAzienda = " UPPER(gruppo) = 'CENTRO' OR UPPER(creatore) = 'CRPA' "

        AgronicaCoreUtility.CaricaListControl.Codici(Codici,
                                                 False, "", "",
                                                 0, "",
                                                 StrCodiciAzienda, "", objParametri_Server)


        Dim TipoAttività As New Web.UI.WebControls.DropDownList()
        AgronicaCoreUtility.CaricaListControl.TipoAttivita(TipoAttività,
                                                       True, " ", " ",
                                                       "", "", objParametri_Server)

        Dim OrganismiControllo As New Web.UI.WebControls.DropDownList()
        AgronicaCoreUtility.CaricaListControl.BIO_Organismi_Controllo(OrganismiControllo,
                                                              True, "", "0",
                                                              0, "", "", 2, 0, "", "", objParametri_Server)

        Dim otes = AgronicaCoreUtility.CaricaListControl.Riempi_cblOTE(piva, Sa_Cod, objParametri_Server)

        Dim codiceOperatore As String = ""

        Dim risp As New CentroDropdownLists()
        risp.CodiceOperatore = codiceOperatore
        risp.Tipologia = Tipologia
        risp.TitoloPossesso = TitoloPossesso
        risp.Comune = Comune
        risp.Provincie = Provincie
        risp.Stati = Stati
        risp.Codici = Codici
        risp.TipoAttivita = TipoAttività
        risp.Otes = otes
        risp.OrganismiControllo = OrganismiControllo
        risp.CentroAziendaleEsternoCollegato = CentroAziendaleEsternoCollegato
        Return risp
    End Function

    Private Sub ImpostaValoriDropdown(xPiva As String, xSa_Cod As String,
                                      ByRef codiceOperatore As String,
                                      ByRef CmbTipoAttivita As Web.UI.WebControls.DropDownList,
                                      ByRef CmbOrganismoControllo As Web.UI.WebControls.DropDownList,
                                      objParametri_Server As AgronicaCoreParametri)
        Dim objCodici As New AgronicaCoreAnagrafeDAL.Centri_Codici_Read
        Dim DTCodici = objCodici.Leggi(CStr(xPiva),
                                            CInt(xSa_Cod),
                                            0, "", "",
                                            AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                            "id_cod < 2000 OR id_cod >= 3000",
                                            "",
                                            objParametri_Server)

        If DTCodici.Rows.Count > 0 Then
            Dim i As Integer
            For i = 0 To DTCodici.Rows.Count - 1

                If DTCodici.Rows(i).Item("Id_Cod") = enum_CodiciAnagrafe.CodiceCentro_Attuale Then

                    codiceOperatore = DTCodici.Rows(i).Item("Val_Cod")

                End If


                '###########################################################
                '### Tipo di Attivita'
                '###########################################################

                If DTCodici.Rows(i).Item("Id_Cod") = enum_CodiciAnagrafe.TipoAttivita Then

                    Dim Testo = DTCodici.Rows(i).Item("Val_Cod")

                    CmbTipoAttivita.SelectedIndex =
                                CmbTipoAttivita.Items.IndexOf(
                                    CmbTipoAttivita.Items.FindByValue(
                                        Testo))

                End If

                '###########################################################
                '### Associazione con Organismi di Controllo
                '###########################################################
                '-------------------------------------------------------------
                If DTCodici.Rows(i).Item("Id_Cod") = enum_CodiciAnagrafe.ORGANISMO_DI_CONTROLLO_BIO Then
                    Dim valcod As String = DTCodici.Rows(i).Item("Val_Cod")
                    CmbOrganismoControllo.SelectedIndex =
                                CmbOrganismoControllo.Items.IndexOf(
                                    CmbOrganismoControllo.Items.FindByValue(
                                        valcod))

                End If
            Next

        End If
    End Sub

End Class

