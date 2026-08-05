Imports AgronicaCoreDataProvider

Public Class Importa
    Dim objParametriServer As AgronicaCoreParametri
    Dim objParametriUtenti As AgronicaCoreParametri

    Dim wsRegistroStalla As ChiamawsRegistroStallaQry
    Dim wsAnagraficaCapo As ChiamawsAnagraficaCapoQry
    Dim wsAziende As ChiamawsAziendeQry
    Dim wsCodici As ChiamawsCodiciQry
    Dim wsIdentificativi As ChiamawsIdentificativiGet
    Dim wsStrutture As ChiamawsStruttureQry
    Dim wsTerritorio As ChiamawsTerritorioQry
    Dim wsGestioneAssConsorzi As ChiamawsGestioneAssConsorzi

    Public Sub New(objParametriServer As AgronicaCoreParametri, objParametriUtenti As AgronicaCoreParametri)
        Me.objParametriServer = objParametriServer
        Me.objParametriUtenti = objParametriUtenti

        Me.wsRegistroStalla = New ChiamawsRegistroStallaQry(objParametriServer, objParametriUtenti, "", "", "")
        Me.wsAnagraficaCapo = New ChiamawsAnagraficaCapoQry(objParametriServer, objParametriUtenti, "", "", "")
        Me.wsAziende = New ChiamawsAziendeQry(objParametriServer, objParametriUtenti, "", "", "")
        Me.wsCodici = New ChiamawsCodiciQry(objParametriServer, objParametriUtenti, "", "", "")
        Me.wsIdentificativi = New ChiamawsIdentificativiGet(objParametriServer, objParametriUtenti, "", "", "")
        Me.wsStrutture = New ChiamawsStruttureQry(objParametriServer, objParametriUtenti, "", "", "")
        Me.wsTerritorio = New ChiamawsTerritorioQry(objParametriServer, objParametriUtenti, "", "", "")
        Me.wsGestioneAssConsorzi = New ChiamawsGestioneAssConsorzi(objParametriServer, objParametriUtenti, "", "", "")

    End Sub

    Public Function ImportaRegistroStalla()
        Dim Username = ""
        Dim Password = ""
        Dim uri = ""

        'Dim a = wsRegistroStalla.getRegistriStalla("", "074VI501", "4395441", "0121", "06/07/2022")

        'Dim b = wsRegistroStalla.getCapiInStalla("4395441")

        If False Then
            Dim codice_consorzio = "INA"
            Dim aziendeConsorzio = wsGestioneAssConsorzi.Get_Aziende_Consorzio("", codice_consorzio)
            Dim allevamentiGlobali As New DataTable
            Dim countAllevamentiTotali As Integer = 0
            Dim i = 0
            For Each aziendaConsorzio In aziendeConsorzio.Rows
                Dim azienda_codice = aziendaConsorzio("AZIENDA_CODICE")
                Dim sincronizzatoreAzienda As New SincroBDNAzienda(objParametriServer, objParametriUtenti, "", "", "")
                sincronizzatoreAzienda.SincronizzaAzienda(azienda_codice, "02562260360", True, True)
            Next

            Dim ret = ""
        End If

        If False Then
            Dim a = wsTerritorio.DownloadAssociazioni
            Dim b = wsTerritorio.DownloadAziendeUSL
            Dim c = wsTerritorio.DownloadCategorie
            Dim d = wsTerritorio.DownloadComuni
            Dim e = wsTerritorio.DownloadDistretti
            Dim f = wsTerritorio.DownloadProvince
            Dim g = wsTerritorio.DownloadRegioni
            Dim h = wsTerritorio.DownloadStati
        End If

        If False Then
            Dim MALATTIE_QUALIFICHESANITARIE = wsCodici.DownloadMalattie_QualificheSanitarie()
            Dim TIPOLOGIARISULTATOTEST = wsCodici.DownloadTipologiaRisultatoTest()
            Dim TIPI_QUALIFICHE_SANITARIE = wsCodici.DownloadTipi_Qualifiche_Sanitarie()
            Dim TIPI_MALATTIE = wsCodici.DownloadTipi_Malattie()

            Dim causaliMorte = wsCodici.DownloadCausaliMorte()
            Dim gruppiSpecie = wsCodici.DownloadGruppiSpecie()
            Dim motiviIngresso = wsCodici.DownloadMotiviIngresso()
            Dim motiviUscita = wsCodici.DownloadMotiviUscita()
            Dim orientamentiProduttivi = wsCodici.DownloadOrientamentiProduttivi()
            Dim razze = wsCodici.DownloadRazze()
            Dim specie = wsCodici.DownloadSpecie()
            Dim tipiAllevamento = wsCodici.DownloadTipiAllevamento()
            Dim tipiCodice = wsCodici.DownloadTipiCodice()
            Dim tipiPassaporti = wsCodici.DownloadTipiPassaporti()
            Dim tipiStatoCapo = wsCodici.DownloadTipiStatoCapo()
            Dim tipiTipologieProduttive = wsCodici.DownloadTipiTipologieProduttive()
            Dim tipiProvenienza = wsCodici.DownloadTipiProvenienza()
            Dim suiTipiCategorie = wsCodici.DownloadSuiTipiCategorie()
        End If

        If True Then
            Dim dtMovimentazioni
            Dim sincronizzatoreAnimale = New SincroBDNAnimale(objParametriServer, objParametriUtenti, "", "", "")
            dtMovimentazioni = wsRegistroStalla.getMovimentazioniCapo("IT090990559487")
            sincronizzatoreAnimale.trovaID_Uscita("00269690368", "045MO105", "IT090990559487")
            Dim aaa = 0
        End If

        Dim aa = 0


    End Function

End Class
