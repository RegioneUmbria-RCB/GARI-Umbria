Public Class Filtro_RiepilogoSuperficiMultiazienda
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        If Session("ASG_objParametri_Server") Is Nothing Then
            Response.Redirect("~/Custom500.aspx")
        End If

        Dim objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))
        Dim objParametri_Utenti = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))


        Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R

        ' Permesso di Lettura -> commentato per uniformità con la vecchia stampa
        'Dim UtenteAbilitatoLettura = objPermessi.Controlla_Permessi_Utente(Session("ASG_Utente_Username"),
        '                                                               Session("ASG_IdServizio"),
        '                                                               AgronicaCoreDataProvider.TipiEnumerativi.enum_Security_Attivita.Gest_Stampe,
        '                                                               AgronicaCoreDataProvider.TipiEnumerativi.enum_Security_Operazione.Lettura,
        '                                                               Date.Now,
        '                                                               "",
        '                                                               objParametri_Utenti)

        'If Not UtenteAbilitatoLettura Then
        '    'TODO Verificare il redirect per le stampe
        '    Response.Redirect("~/Custom500.aspx")
        '    Exit Sub
        'End If


        hfPiva.Value = AgronicaCoreDataProvider.Sicurezza.Stringa_Decodifica(Request.QueryString("p"), AgronicaCoreDataProvider.CostantiPersonalizzate.AgroKey_EncoderDecoder)

        CType(MyBase.Master, StampeBootstrap).SetTitoloPagina(42)

    End Sub

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Formatta i parametri e ricava la datatable GetRiepilogoSuperficiMultiazienda
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[magnani]	11/05/2011	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Private Sub GetRiepilogoSuperficiMultiazienda()

        'si presuppone che la session sia valorizzata dal filtrone
        Dim ListPiva() As String = Session("ListPiva")
        Dim ListSa_cod() As Integer = Session("ListSa_cod")
        Dim ListAppezza() As Integer = Session("ListAppezza")
        Dim ListId_reg() As Integer = Session("ListId_reg")
        Dim dataValiditaProgetto As Date = Date.Today


        Dim B_Piva As Boolean = False ' "Partita IVA"
        Dim B_Rag_Soc As Boolean = False '"Ragione Sociale"
        Dim B_NomeCentro As Boolean = False   '"Nome Centro"
        Dim B_Specie As Boolean = False ' "Specie Vegetale"
        Dim B_Varieta As Boolean = False  ' "Varietà"
        Dim B_SumImp As Boolean = False  ' "Superficie Impianto"
        Dim B_Particelle As Boolean = False  ' "Particelle e Superficie Particelle e Sup. di intersezione Impianto e Particella"
        Dim B_NumeroPiante As Boolean = False 'numero piante
        Dim B_Copertura As Boolean = False 'Copertura 
        Dim B_Finalita As Boolean = False 'Finalita

        'If Me.ChkGruppo1.Items.FindByValue(I_Piva).Selected = True Then
        '    B_Piva = True
        'End If
        'If Me.ChkGruppo1.Items.FindByValue(I_Rag_Soc).Selected = True Then
        '    B_Rag_Soc = True
        'End If
        'If Me.ChkGruppo1.Items.FindByValue(I_NomeCentro).Selected = True Then
        '    B_NomeCentro = True
        'End If
        'If Me.ChkGruppo2.Items.FindByValue(I_Specie).Selected = True Then
        '    B_Specie = True
        'End If
        'If Me.ChkGruppo2.Items.FindByValue(I_Varieta).Selected = True Then
        '    B_Varieta = True
        'End If
        'If Me.ChkGruppo2.Items.FindByValue(I_SumImp).Selected = True Then
        '    B_SumImp = True
        'End If
        'If Me.ChkGruppo2.Items.FindByValue(I_NumPiante).Selected = True Then
        '    B_NumeroPiante = True
        'End If
        'If Me.ChkGruppo2.Items.FindByValue(I_Copertura).Selected = True Then
        '    B_Copertura = True
        'End If
        'If Me.ChkGruppo2.Items.FindByValue(I_Finalita).Selected = True Then
        '    B_Finalita = True
        'End If
        'If Me.ChkGruppo3.Items.FindByValue(I_Particelle).Selected = True Then
        '    B_Particelle = True
        'End If

        'dataValiditaProgetto = CDate(Me.Txt_Data.Text)


        'Dim GroupParam() As Boolean = {B_Piva, B_Rag_Soc, B_NomeCentro, B_Specie, B_Varieta, B_SumImp, B_NumeroPiante, B_Copertura, B_Particelle, B_Finalita}

        ''non devo passare piu di 4000 record altrimenti StrSQLOrID troppo lunga e va in overflow il server
        'If ListPiva.Length > 4000 Then
        '    ListPiva = New String() {}
        '    ListSa_cod = New Integer() {}
        '    ListAppezza = New Integer() {}
        '    ListId_reg = New Integer() {}
        '    AgroMsgBox("Sono state selezionati più di 4000 impianti distinti, NON verranno considerati tutti gli impianti in quanto il limite per questa stampa è stato superato ", Page)
        'End If


    End Sub


End Class