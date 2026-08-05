Imports System.Web.Services
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreUtility

Partial Class PannelloDiControllo
    Inherits System.Web.UI.Page

    Dim objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri
    Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Response.Expires = 0

        If Session("ASG_Utente_Username") = "" Then
            Response.Redirect("~/Custom500.aspx")
        End If

        objParametri_Utenti = New AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))
        objParametri_Server = New AgronicaCoreParametri(Session("ASG_objParametri_Server"))

        'Controllo se l'utente ha i permessi per accedere
        Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
        Dim UtenteAbilitato As Boolean = objPermessi.Controlla_Permessi_Utente( _
                                            Session("ASG_Utente_Username"), _
                                            Session("ASG_IdServizio"), _
                                            enum_Security_Attivita.NonConformita, _
                                            enum_Security_Operazione.Lettura, _
                                            Date.Now, _
                                            "", _
                                            objParametri_Utenti)

        If Not UtenteAbilitato Then
            Response.Redirect("~/Classi/Agro_Pages/AccessoNonConsentito.aspx")
        End If

        If Not IsPostBack Then
            Master.Master.Lbl_Titolo.Text = "Pannello di Controllo_NEW"

            Dim dtut As DataTable = New AgronicaCoreUtentiDAL.Utenti_Dettagli_R().Utenti_Dettagli_from_USERNAME(objParametri_Utenti.UtenteUsername, objParametri_Utenti)
            Dim Nome As String = dtut.Rows(0).Item("Nome")
            Dim Cognome As String = dtut.Rows(0).Item("Cognome")
            Dim Username As String = dtut.Rows(0).Item("Username")
            Master.Master.LbLUtente.Text = Cognome & "" & Nome & " (" & Username & ")"
            If objParametri_Server.FinestraTemporaleInizio = "01/01/1900" AndAlso objParametri_Server.FinestraTemporaleFine = "31/12/2100" Then

                Master.Master.lblFinestraTemporale.Text = "VisualizzazioneIllimitata"

            Else
                Master.Master.lblFinestraTemporale.Text = "VisualizzazioneLimitataTraIl " &
                    IIf(objParametri_Server.FinestraTemporaleInizio = "01/01/1900", "...", objParametri_Server.FinestraTemporaleInizio) & _
                    " e il " &
                IIf(objParametri_Server.FinestraTemporaleFine = "31/12/2100", "...", objParametri_Server.FinestraTemporaleFine)

            End If
        End If

    End Sub


    <Script.Services.ScriptMethod()> <WebMethod(EnableSession:=True)> _
    Public Shared Function ReadConformita() As String

        Dim objParametri As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim dp As New AgronicaCoreDataProvider.DataProvider
        Dim stb As New StringBuilder()

        Dim ID_PDC_Testata As Integer = 48

        'stb.AppendLine(" SELECT     PDC_LFO.LFO_Des, PDC_Dettagli.Rag_Soc, ISNULL(PDC_Dettagli.Sa_Nome,'') as SA_NOME , ISNULL(PDC_Dettagli.App_Nome, '') as App_Nome, SpecieVegetali.Veg_Des, Cultivar.Cul_Des,   ")
        'stb.AppendLine("       ISNULL(PDC_Dettagli.CapitolatoPrivato, '') as CapitolatoPrivato, ISNULL(PDC_Dettagli.Certificato,'' ) as Certificato, PDC_Analisi.Analisi_Testata_Cod, Analisi_Tipologia.Analisi_Tipologia_Des,  ")
        'stb.AppendLine("       Contatti.Rag_Soc AS Cod_Risum_Des, PDC_Campioni.Codice_Campione, PDC_Campioni.ID_PDC_Stato_Campione, ISNULL(PDC_Campioni.PDC_Campione_Des, '') as PDC_Campione_Des ,  ")

        'stb.AppendLine("       ISNULL(CONVERT(VARCHAR(10), PDC_Dettagli.Data_Fornitura, 103)   , '')as Data_Fornitura_Str,   ")
        'stb.AppendLine("       ISNULL(PDC_Dettagli.NomeArticolo, '') as NomeArticolo,   ")
        'stb.AppendLine("       ISNULL(PDC_Dettagli.CodiceFornitore, '') as CodiceFornitore,   ")

        'stb.AppendLine("       PDC_Dettagli.Id_PDC_Dettagli,   ")
        'stb.AppendLine("       ISNULL(PDC_Campioni.ID_PDC_Campione, '') as ID_PDC_Campione,   ")

        'stb.AppendLine("       CONVERT(VARCHAR(10), PDC_Campioni.Data_Campionamento, 103) AS Data_Campionamento, CONVERT(VARCHAR(10), analisi_testata.analisi_testata_data_fine, 103) AS Data_Richiesta_Analisi, ISNULL(Analisi_Testata.Analisi_Testata_Des,'') AS Analisi_Testata_Des ")
        stb.AppendLine(" SELECT     PDC_LFO.LFO_Des, PDC_Dettagli.Rag_Soc, ISNULL(PDC_Dettagli.Sa_Nome,'') as SA_NOME , ISNULL(PDC_Dettagli.App_Nome, '') as App_Nome, SpecieVegetali.Veg_Des, Cultivar.Cul_Des,   ")
        stb.AppendLine("       ISNULL(PDC_Dettagli.CapitolatoPrivato, '') as CapitolatoPrivato, ISNULL(PDC_Dettagli.Certificato,'' ) as Certificato, PDC_Analisi.Analisi_Testata_Cod, Analisi_Tipologia.Analisi_Tipologia_Des,  ")
        stb.AppendLine("       Contatti.Rag_Soc AS Cod_Risum_Des, PDC_Campioni.Codice_Campione, PDC_Campioni.ID_PDC_Stato_Campione, ISNULL(PDC_Campioni.PDC_Campione_Des, '') as PDC_Campione_Des ,  ")

        stb.AppendLine("       ISNULL(CONVERT(VARCHAR(10), PDC_Dettagli.Data_Fornitura, 103)   , '')as Data_Fornitura_Str,   ")
        stb.AppendLine("       ISNULL(PDC_Dettagli.NomeArticolo, '') as NomeArticolo,   ")
        stb.AppendLine("       ISNULL(PDC_Dettagli.CodiceFornitore, '') as CodiceFornitore,   ")

        stb.AppendLine("       PDC_Dettagli.Id_PDC_Dettagli,   ")
        stb.AppendLine("       ISNULL(PDC_Campioni.ID_PDC_Campione, '') as ID_PDC_Campione,   ")

        stb.AppendLine("       CONVERT(VARCHAR(10), PDC_Campioni.Data_Campionamento, 103) AS Data_Campionamento, CONVERT(VARCHAR(10), analisi_testata.analisi_testata_data_fine, 103) AS Data_Richiesta_Analisi, ISNULL(Analisi_Testata.Analisi_Testata_Des,'') AS Analisi_Testata_Des ")

        stb.AppendLine("FROM         Cultivar  ")
        stb.AppendLine("       INNER JOIN SpecieVegetali  ")
        stb.AppendLine("       INNER JOIN PDC_Testata  ")
        stb.AppendLine("       INNER JOIN PDC_Dettagli ON PDC_Testata.PivaSuperUser = PDC_Dettagli.PivaSuperUser AND PDC_Testata.Id_PDC_Testata = PDC_Dettagli.ID_PDC_Testata  ")
        stb.AppendLine("       INNER JOIN PDC_Campioni ON PDC_Dettagli.PivaSuperUser = PDC_Campioni.PivaSuperUser AND PDC_Dettagli.ID_PDC_Testata = PDC_Campioni.ID_PDC_Testata AND PDC_Dettagli.ID_PDC_Dettagli = PDC_Campioni.ID_PDC_Dettagli  ")
        stb.AppendLine("       INNER JOIN PDC_Analisi ON PDC_Campioni.PivaSuperUser = PDC_Analisi.PivaSuperUser AND PDC_Campioni.ID_PDC_Testata = PDC_Analisi.ID_PDC_Testata AND PDC_Campioni.ID_PDC_Dettagli = PDC_Analisi.ID_PDC_Dettagli AND PDC_Campioni.ID_PDC_Campione = PDC_Analisi.ID_PDC_Campione  ")
        stb.AppendLine("       INNER JOIN PDC_LFO ON PDC_Dettagli.ID_LFO = PDC_LFO.ID_LFO AND PDC_Dettagli.PivaSuperUser = PDC_LFO.PivaSuperUser AND PDC_Dettagli.ID_PDC_Testata = PDC_LFO.ID_PDC_Testata ON SpecieVegetali.Veg_Cod = PDC_Dettagli.Veg_Cod ON Cultivar.Cul_Cod = PDC_Dettagli.Cul_Cod  ")
        stb.AppendLine("       INNER JOIN Analisi_Tipologia ON PDC_Analisi.Analisi_Tipologia_Cod = Analisi_Tipologia.Analisi_Tipologia_Cod  ")
        stb.AppendLine("       INNER JOIN Risorse_Umane ON PDC_Analisi.Cod_Risum = Risorse_Umane.Cod_RisUm  ")
        stb.AppendLine("       INNER JOIN Contatti ON Risorse_Umane.Cod_Contatto = Contatti.Cod_Contatto  ")
        stb.AppendLine("       LEFT OUTER JOIN Analisi_Testata ON PDC_Analisi.PivaSuperUser = Analisi_Testata.Analisi_SuperUser AND PDC_Analisi.Analisi_Testata_Cod = Analisi_Testata.Analisi_Testata_Cod  ")

        stb.AppendLine(" WHERE PDC_Analisi.PDC_Stato_Analisi = 2 ")

        stb.AppendLine(" AND (PDC_Analisi.Id_PDC_Testata = " & ID_PDC_Testata & ") ")
        stb.AppendLine(" AND (PDC_Dettagli.Id_PDC_Testata = " & ID_PDC_Testata & ") ")
        stb.AppendLine(" AND (PDC_Campioni.Id_PDC_Testata = " & ID_PDC_Testata & ") ")
        stb.AppendLine(" AND (PDC_Testata.Id_PDC_Testata = " & ID_PDC_Testata & ") ")

        stb.AppendLine(" ORDER BY  PDC_LFO.LFO_Des, PDC_Dettagli.rag_soc, PDC_Dettagli.sa_nome, PDC_Dettagli.APP_NOME, Cultivar.Cul_Des ASC")



        Dim dt As DataTable = dp.EseguiQuery_Lettura(objParametri, stb.ToString(), "prova")

        'creo la lista delle colonne da visualizzare
        Dim l As New List(Of ColonneNome)
        For Each dc As DataColumn In dt.Columns()
            Dim nomeDaVisualizzare As String = dc.ColumnName
            Dim unico As Boolean = False
            Select Case dc.ColumnName
                Case "LFO_Des"
                    nomeDaVisualizzare = "LFO"
                Case "Rag_Soc"
                    nomeDaVisualizzare = "Impresa"
                Case "SA_NOME"
                    nomeDaVisualizzare = "Centro Aziendale"
                Case "App_Nome"
                    nomeDaVisualizzare = "Appezzamento"
                Case "Veg_Des"
                    nomeDaVisualizzare = "Specie"
                Case "Cul_Des"
                    nomeDaVisualizzare = "Varieta"
                    'Case "CapitolatoPrivato"
                    '    nomeDaVisualizzare = "Capitolato Privato"
                    'Case "Certificato"
                    '    nomeDaVisualizzare = "Certificato"
                Case "Analisi_Testata_Cod"
                    unico = False
                    '    nomeDaVisualizzare = "LFO"
                Case "Analisi_Tipologia_Des"
                    nomeDaVisualizzare = "Std. Analisi"
                Case "Cod_Risum_Des"
                    nomeDaVisualizzare = "Laboratorio"
                Case "Codice_Campione"
                    nomeDaVisualizzare = "Codice Campione"
                    'Case "ID_PDC_Stato_Campione"
                    '    nomeDaVisualizzare = "LFO"
                    'Case "PDC_Campione_Des"
                    '    nomeDaVisualizzare = "LFO"
                    'Case "Data_Fornitura_Str"
                    '    nomeDaVisualizzare = "LFO"
                    'Case "NomeArticolo"
                    '    nomeDaVisualizzare = "LFO"
                    'Case "CodiceFornitore"
                    '    nomeDaVisualizzare = "LFO"
                    'Case "Id_PDC_Dettagli"
                    '    nomeDaVisualizzare = "LFO"
                    'Case "ID_PDC_Campione"
                    '    nomeDaVisualizzare = "LFO"
                Case "Data_Campionamento"
                    nomeDaVisualizzare = "Data Campionamento"
                Case "Data_Richiesta_Analisi"
                    nomeDaVisualizzare = "Data Fine Analisi"
                Case "Analisi_Testata_Des"
                    nomeDaVisualizzare = "Codice Analisi"
            End Select
            Dim c As New ColonneNome(dc.ColumnName, nomeDaVisualizzare, "string")
            c._ColonnaDiSelezione = unico
            l.Add(c)
        Next

        Dim js As New AgronicaCoreDataProvider.JSON_DataTable
        Dim risp As String = js.JSON_DataTable(dt, l)
        Return risp

    End Function

End Class