Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreAnagrafeDAL

Public Enum enum_Appezzamento_Pua_Ciclo
    Principale = 0
    Secondario = 1
End Enum

Public Class PUA_Appezzamento

    Public Property Chiave As String
    Public Property Piva As String
    Public Property Sa_Cod As Integer
    Public Property Campo_Cod As Integer
    Public Property appezza As Integer
    Public Property id_reg As Integer
    Public Property Progetto_Cod As Integer
    Public Property Veg_Cod As Integer
    Public Property Grfi_Cod As Integer
    Public Property Grfi_Cod_Concimazione As Integer
    Public Property Grfi_Des_Concimazione As String
    Public Property B_Perc As Decimal
    Public Property Appezzamento As String
    Public Property Catasto As String
    Public Property Superficie As Decimal
    Public Property ValiditaInizio As Date
    Public Property ValiditaFine As Date
    Public Property DurataColtura As String
    Public Property StatoImpiantoCod As Integer
    Public Property StatoImpiantoDes As String
    Public Property Ciclo As Integer
    Public Property CicloDes As String

    Public Property AnalisiTestataCod As Integer
    Public Property AnalisiTestataDes As String
    Public Property So As Decimal
    Public Property Sabbia As Decimal
    Public Property Argilla As Decimal
    Public Property Resa As Decimal
    Public Property Resa_Rif As Decimal
    Public Property FattoreCorrettivo_N As Decimal
    Public Property LimiteMas As Decimal
    Public Property Assorbimento As Decimal
    Public Property PrecessioneCod As Integer
    Public Property PrecessioneDes As String
    Public Property FertOrganicoCod As Integer
    Public Property FertOrganicoDes As String
    Public Property FrequenzaCod As Integer
    Public Property FrequenzaDes As String
    Public Property N_Distribuito As Decimal
    Public Property N_FertilizzazioniPrecedenti As Decimal
    Public Property UbicazioneCod As Integer
    Public Property UbicazioneDes As String
    Public Property TipoAcquaCod As Integer
    Public Property TipoAcquaDes As String


    Public Property N_Fabbisogno_Database As Decimal
    Public Property N_Fabbisogno As Decimal
    Public Property N_FabbisognoComplessivo As Decimal
    Public Property N_FabbisognoSoddisfatto As Decimal
    Public Property N_Zootecnico As Decimal
    Public Property N_Zootecnico_Liquame As Decimal
    Public Property N_Zootecnico_Letame As Decimal
    Public Property N_TotaleSoddisfatto As Decimal
    Public Property N_BilancioAzotato_Utile As Decimal
    Public Property N_BilancioAzotato_Totale As Decimal
    Public Property Indice_Efficienza_Azotata As Decimal

    Public Property Cul_Cod_Agea As String
    Public Property Uso_Cod As String
    Public Property Macrouso_Cod As String
    Public Property Occupazione_Cod As String
    Public Property Destinazione_Cod As String
    Public Property Qualita_Cod As String

    Public Property Valutazione_NUtile As Integer
    Public Property Valutazione_NTotale As Integer
    Public Property Valutazione_Efficienza As Integer

    Public Property ParticelleVincoli As List(Of PUA_ParticellaVincoloAgronomico)
    Public Property ZVN As Boolean
    Public Property Id_AnagrafeVincoli As Integer
    Public Property Pua_Cod As Integer
    Public Property Regolamento_Cod As Integer
    Public Property Data_Pua As Date


    Public Sub New()
        Campo_Cod = 0
        Veg_Cod = 0
        Grfi_Cod = 0
        Grfi_Cod_Concimazione = 0
        Grfi_Des_Concimazione = ""
        B_Perc = 0
        Appezzamento = ""
        Catasto = ""
        Superficie = 0
        ValiditaInizio = AGRODATAINIZIO
        ValiditaFine = AGRODATAFINE
        DurataColtura = ""
        StatoImpiantoCod = 0
        StatoImpiantoDes = ""
        Ciclo = 0
        CicloDes = "Principale"
        AnalisiTestataCod = 0
        AnalisiTestataDes = ""
        So = 0
        Sabbia = 0
        Argilla = 0
        Resa = 0
        Resa_Rif = 0
        FattoreCorrettivo_N = 0
        Assorbimento = 0
        PrecessioneCod = 0
        PrecessioneDes = ""
        FertOrganicoCod = 0
        FertOrganicoDes = ""
        FrequenzaCod = 0
        FrequenzaDes = ""
        N_Distribuito = 0
        N_FertilizzazioniPrecedenti = 0
        UbicazioneCod = 0
        UbicazioneDes = ""
        TipoAcquaCod = 0
        TipoAcquaDes = ""
        N_Fabbisogno_Database = -1
        N_Fabbisogno = -1
        N_FabbisognoComplessivo = 0
        N_FabbisognoSoddisfatto = 0
        N_TotaleSoddisfatto = 0
        N_BilancioAzotato_Utile = 0
        N_BilancioAzotato_Totale = 0
        N_Zootecnico = 0
        N_Zootecnico_Letame = 0
        N_Zootecnico_Liquame = 0
        Valutazione_NUtile = 0
        Valutazione_NTotale = 0
        Indice_Efficienza_Azotata = 0
        Valutazione_Efficienza = 0
        Cul_Cod_Agea = "000"
        Uso_Cod = "000"
        Macrouso_Cod = "000"
        Occupazione_Cod = "000"
        Destinazione_Cod = "000"
        Qualita_Cod = "000"
        Id_AnagrafeVincoli = 0
        Pua_Cod = 0
        Regolamento_Cod = 0
        Data_Pua = AGRODATAINIZIO
        ParticelleVincoli = New List(Of PUA_ParticellaVincoloAgronomico)
        ZVN = False
    End Sub

    Public Function Modifica(ByRef objParametriServer As AgronicaCoreParametri, ByRef msgError As String) As Boolean

        Const nomeRoutine = "PUA_Appezzamento.Modifica"
        Dim xRisp As Boolean = False

        Dim flagConnessione As Boolean = False
        Dim flagTransazione As Boolean = False

        Dim objSequenze As New Agro_Sequenze
        Dim objImpProgettiW As New Impresa_Progetti_W
        Dim objAnaVincoliW As New Anagrafe_VincoliAgronomici_W
        Dim objRegImpianti As New Reg_Impianti_Codici_W

        Try

            Utility.VerificaApriTransazione(objParametriServer, flagConnessione, flagTransazione)

            'flagOggettoCorretto = CheckPropertyTestataLavorazione(objLavorazione, enum_TipoOperazioneDB.Modifica)

            'If flagOggettoCorretto Then

            'update di dati su Imprese_Progetti (questa riga c'è sempre sotto, quindi vado in edit diretto)
            xRisp = objImpProgettiW.Modifica_x_PUA(Piva, Sa_Cod, appezza, id_reg, Progetto_Cod,
                                                   Resa, Ciclo,
                                                   "", objParametriServer)

            If xRisp = True Then
                'Reg_Impianti_Codici (in automatico prima cancella e poi riscrive)
                xRisp = objRegImpianti.ModificaxProgetto2(Piva, Sa_Cod, appezza, id_reg, Progetto_Cod,
                                                          enum_CodiciAnagrafe.Impianto_LimiteN_Organico,
                                                          N_Fabbisogno,
                                                          AGRODATAINIZIO, AGRODATAFINE,
                                                          "", objParametriServer)

            End If

            If xRisp = True Then
                xRisp = objRegImpianti.ModificaxProgetto2(Piva, Sa_Cod, appezza, id_reg, Progetto_Cod,
                                                          enum_CodiciAnagrafe.Finalita_Concimazione_Impianto,
                                                          Grfi_Cod_Concimazione,
                                                          AGRODATAINIZIO, AGRODATAFINE,
                                                          "", objParametriServer)
            End If

            If xRisp = True Then

                If Id_AnagrafeVincoli <= 0 Then

                    Throw New Exception("Id_AnagrafeVincoli = 0")
                    'la riga sotto non ce l'avevo, quindi vado in insert

                    'Dim idAna As Integer = objSequenze.NuovoId_Tabella("Anagrafe_VincoliAgronomici",
                    '                                                   0, 2000000000, objParametriServer)

                    'xRisp = objAnaVincoliW.Scrivi(idAna,
                    '                              Piva, Sa_Cod, appezza, id_reg, Progetto_Cod,
                    '                              Pua_Cod, Regolamento_Cod,
                    '                              AnalisiTestataCod, PrecessioneCod,
                    '                              UbicazioneCod, TipoAcquaCod,
                    '                              N_FertilizzazioniPrecedenti,
                    '                              AGRODATAINIZIO, AGRODATAFINE,
                    '                              objParametriServer)

                    'If xRisp = True Then
                    '    'Devo sovrascrivere Id_AnagrafeVincoli chè ora ce l'ho, altrimenti si potrebbe incasinare il salvataggio successivo
                    '    Id_AnagrafeVincoli = idAna
                    'End If
                Else
                    'la riga esisteva, quindi update
                    'TODO: qui N_FertilizzazioniPrecedenti lo devo aggiornare io?!? teoricamente dovrebbe già avermelo aggiornato le letamazioniprecedenti
                    xRisp = objAnaVincoliW.ModificaPuntuale(Id_AnagrafeVincoli,
                                                            objParametriServer,
                                                            AnalisiTestataCod,
                                                            PrecessioneCod,
                                                            UbicazioneCod,
                                                            TipoAcquaCod,
                                                            N_FertilizzazioniPrecedenti)
                End If

            End If



            'End If

            If xRisp = False Then
                Utility.VerificaAnnullaTransazione(objParametriServer, flagTransazione)
            Else
                Utility.VerificaChiudiTransazione(objParametriServer, flagTransazione)
            End If

        Catch ex As Exception
            Utility.VerificaAnnullaTransazione(objParametriServer, flagTransazione)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
        Finally
            Utility.VerificaChiudiConnessione(objParametriServer, flagConnessione)
        End Try

        Return xRisp

    End Function

End Class

Public Class PUA_ParticellaVincoloAgronomico
    Public Property Id_PartVincoli As Integer
    Public Property Part_PROV As String
    Public Property Part_COM As String
    Public Property Part_SEZIONE As String
    Public Property Part_FOGLIO As Integer
    Public Property Part_NUMERO As Integer
    Public Property Part_SUBALTERNO As String
    Public Property ValiditaInizio As Date
    Public Property ValiditaFine As Date
    Public Property Part_PROVINCIA As String
    Public Property Part_COMUNE As String
    Public Property ZVN As Boolean
    Public Property Sup_Condotta As Decimal
    Public Property Sup_AppxPart As Decimal
    Public Sub New()
        Id_PartVincoli = 0
        ValiditaInizio = AGRODATAINIZIO
        ValiditaFine = AGRODATAFINE
        ZVN = False
    End Sub

    Public Shared Function EstrapolaStringaFiltroAnalisiCatasto(ByVal piva As String,
                                                                ByRef listaParticelle As List(Of PUA_ParticellaVincoloAgronomico)
                                                                ) As String
        Dim strAnalisiCatasto As String = ""

        If Not listaParticelle Is Nothing AndAlso listaParticelle.Count > 0 Then

            For Each part As PUA_ParticellaVincoloAgronomico In listaParticelle
                strAnalisiCatasto &= IIf(strAnalisiCatasto = "", "", " OR ") &
                                     " (Piva = '" & piva & "' AND prov = '" & part.Part_PROV & "' AND com = '" & part.Part_COM & "' AND sezione = '" & part.Part_SEZIONE & "' AND numero = " & CStr(part.Part_NUMERO) & " AND foglio = " & CStr(part.Part_FOGLIO) & " AND subalterno = '" & part.Part_SUBALTERNO & "')"
            Next

        End If

        Return strAnalisiCatasto

    End Function

End Class



