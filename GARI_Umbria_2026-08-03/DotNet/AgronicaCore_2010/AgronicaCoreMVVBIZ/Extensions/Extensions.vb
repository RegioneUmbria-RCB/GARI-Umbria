Imports System.Runtime.CompilerServices
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreMVVCommon
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Public Module Extensions

    <Extension()>
    Public Function ToMVV(ByVal table As DataTable) As MVV

        Dim result As New MVV With {.Dettagli = New List(Of MVV_Dettaglio)}

        Try
            If table.Rows.Count > 0 Then

                Dim commonRow = table.AsEnumerable().FirstOrDefault()

                result.Note = commonRow.Item("Note")

                Dim doc = New MVV_Documento With
            {
                .CausaleTrasporto = commonRow.Item("CausaleTrasporto"),
                .DocNumeroSin = commonRow.Item("Documento_Doc_Numero_Sin"),
                .DocNumero = commonRow.Item("Documento_Doc_Numero"),
                .DocNumeroDes = commonRow.Item("Documento_Doc_Numero_Des"),
                .DataInizioTrasporto = commonRow.Item("Documento_Ora"),
                .LuogoSpedizione = commonRow.Item("LuogoSpedizione"),
                .CodiceICQRF = commonRow.Item("CodiceICQRF"),
                .DataDocumento = commonRow.Item("Documento_DataDocumento").ToString(),
                .FlagArt29 = CInt(commonRow.Item("FlagArt29"))
            }
                Dim oraInizioTrasporto = doc.DataInizioTrasporto.ToString("HH:mm")
                doc.OraInizioTrasporto = If(oraInizioTrasporto = "00:00", "", oraInizioTrasporto)
                doc.OraTrasporto = doc.DataInizioTrasporto.Hour
                doc.MinutiTrasporto = doc.DataInizioTrasporto.Minute
                result.Documento = doc

                Dim autComp = New MVV_AutoritaCompetente With
            {
                .ICQRF = commonRow.Item("UfficioICQRF"),
                .Indirizzo = New MVV_Indirizzo With
                {
                    .CAP = commonRow.Item("Autorita_Competente_CAP"),
                    .Comune_Provincia = commonRow.Item("Autorita_Competente_Comune_Provincia"),
                    .Indirizzo = commonRow.Item("Autorita_Competente_Indirizzo"),
                    .Stato = commonRow.Item("Autorita_Competente_Stato")
                }
            }
                result.AutoritaComepetente = autComp

                Dim sped = New MVV_Speditore With
            {
                .ID_CF = enum_Contatti_IdCf.PersonaGiuridica,
                .Indirizzo = New MVV_Indirizzo With
                {
                    .CAP = commonRow.Item("Speditore_CAP"),
                    .Comune_Provincia = commonRow.Item("Speditore_Comune_Provincia"),
                    .Indirizzo = commonRow.Item("Speditore_Indirizzo"),
                    .Stato = commonRow.Item("Speditore_Stato"),
                    .Provincia = commonRow.Item("Speditore_Provincia"),
                    .Istat_Provincia = commonRow.Item("Speditore_Provincia_Istat"),
                    .Istat_Comune = commonRow.Item("Speditore_Comune_Istat")
                },
                .Denominazione = commonRow.Item("Speditore_Denominazione"),
                .PIVA = commonRow.Item("PIVA"),
                .CodiceAccisa = commonRow.Item("Speditore_CodiceAccisa"),
                .CODICE_SOGGETTO = commonRow.Item("Speditore_CodiceSoggetto")
            }
                result.Speditore = sped

                Dim vett As New MVV_Vettore With
            {
                .TipoSoggetto = "Vettore",
                .Indirizzo = New MVV_Indirizzo With
                {
                    .CAP = commonRow.Item("Vettore_CAP"),
                    .Comune_Provincia = commonRow.Item("Vettore_Comune_Provincia"),
                    .Indirizzo = commonRow.Item("Vettore_Indirizzo"),
                    .Stato = commonRow.Item("Vettore_Stato"),
                    .Istat_Provincia = commonRow.Item("Vettore_Provincia"),
                    .Istat_Comune = commonRow.Item("Vettore_Comune")
                },
                .ID_CF = CInt(commonRow.Item("Vettore_Id_CF")),
                .Denominazione = commonRow.Item("Vettore_Denominazione"),
                .PIVA = commonRow.Item("Vettore_CodContatto"),
                .Mezzo = If(commonRow.IsNull("Mov_C_Mezzo"), -1, CInt(commonRow.Item("Mov_C_Mezzo"))),
                .CODICE_SOGGETTO = commonRow.Item("Vettore_CodiceSoggetto"),
                .COD_CONTATTO = commonRow.Item("Vettore_CodContatto")
            }
                result.Vettore = vett

                Dim Conducente_Cognome_Nome As String = commonRow.Item("Conducente_Cognome_Nome").ToString
                If Not String.IsNullOrEmpty(Conducente_Cognome_Nome) Then
                    Dim datiCond = Conducente_Cognome_Nome.Split("|")

                    result.Conducente_Cognome = datiCond(0)
                    If datiCond.Count > 1 Then
                        result.Conducente_Nome = datiCond(1)
                    End If

                End If

                    Dim dest As New MVV_Destinatario With
                {
                    .TipoSoggetto = "Destinatario",
                    .Indirizzo = New MVV_Indirizzo With
                    {
                        .CAP = commonRow.Item("Destinatario_CAP"),
                        .Comune_Provincia = commonRow.Item("Destinatario_Comune_Provincia"),
                        .Indirizzo = commonRow.Item("Destinatario_Indirizzo"),
                        .Stato = commonRow.Item("Destinatario_Stato"),
                        .Istat_Provincia = commonRow.Item("Destinatario_Provincia"),
                        .Istat_Comune = commonRow.Item("Destinatario_Comune")
                    },
                    .Denominazione = commonRow.Item("Destinatario_Rag_Soc"),
                    .Nome = commonRow.Item("Destinatario_Nome"),
                    .Cognome = commonRow.Item("Destinatario_Cognome"),
                    .ID_CF = CInt(commonRow.Item("Destinatario_Id_CF")),
                    .PIVA = commonRow.Item("Cod_Contatto_Cliente"),
                    .CodiceAccisa = commonRow.Item("Destinatario_CodiceAccisa"),
                    .CODICE_SOGGETTO = commonRow.Item("Destinatario_CodiceSoggetto"),
                    .COD_CONTATTO = commonRow.Item("Cod_Contatto_Cliente")
                }
                result.Destinatario = dest

                result.HaDestinatarioDiverso = False
                Dim codDestDiverso = CInt(commonRow.Item("Mov_C_Cod_destinazione"))
                If (codDestDiverso = 0) Then
                    Dim destDiverso As New MVV_Destinatario_Diverso With
               {
                        .TipoSoggetto = "Destinatario Diverso",
                        .Indirizzo = dest.Indirizzo,
                       .Denominazione = dest.Denominazione,
                       .Nome = dest.Nome,
                       .Cognome = dest.Cognome,
                       .ID_CF = dest.ID_CF,
                       .CODICE_SOGGETTO = dest.CODICE_SOGGETTO,
                       .CodiceAccisa = dest.CodiceAccisa,
                       .COD_CONTATTO = dest.COD_CONTATTO
               }
                    result.DestinatarioDiverso = destDiverso
                Else
                    result.HaDestinatarioDiverso = True
                    Dim destDiverso As New MVV_Destinatario_Diverso With
                {
                    .TipoSoggetto = "Destinatario Diverso",
                    .Indirizzo = New MVV_Indirizzo With
                    {
                         .CAP = commonRow.Item("Destinatario_Diverso_CAP"),
                        .Comune_Provincia = commonRow.Item("Destinatario_Diverso_Comune_Provincia"),
                        .Indirizzo = commonRow.Item("Destinatario_Diverso_Indirizzo"),
                        .Stato = commonRow.Item("Destinatario_Diverso_Stato"),
                        .Istat_Provincia = commonRow.Item("Destinatario_Diverso_Provincia"),
                        .Istat_Comune = commonRow.Item("Destinatario_Diverso_Comune")
                    },
                    .Denominazione = commonRow.Item("Destinatario_Diverso_Rag_Soc"),
                    .Nome = commonRow.Item("Destinatario_Diverso_Nome"),
                    .Cognome = commonRow.Item("Destinatario_Diverso_Cognome"),
                    .ID_CF = CInt(commonRow.Item("Destinatario_Diverso_Id_CF")),
                    .CODICE_SOGGETTO = commonRow.Item("Destinatario_Diverso_CodiceSoggetto"),
                    .COD_CONTATTO = commonRow.Item("Destinatario_Diverso_CodiceSoggetto"),
                    .CodiceAccisa = commonRow.Item("Destinatario_Diverso_CodiceAccisa")
                }
                    result.DestinatarioDiverso = destDiverso
                End If

                ' Acquirente
                result.HaAcquirente = CBool(commonRow.Item("Acquirente_Specified"))
                If result.HaAcquirente Then
                    Dim acquirente As New MVV_Acquirente With
                   {
                       .TipoSoggetto = "Acquirente",
                       .Indirizzo = New MVV_Indirizzo With
                       {
                           .CAP = commonRow.Item("Acquirente_CAP"),
                           .Comune_Provincia = commonRow.Item("Acquirente_Comune_Provincia"),
                           .Indirizzo = commonRow.Item("Acquirente_Indirizzo"),
                           .Stato = commonRow.Item("Acquirente_Stato"),
                           .Istat_Provincia = commonRow.Item("Acquirente_Provincia"),
                           .Istat_Comune = commonRow.Item("Acquirente_Comune")
                       },
                       .Denominazione = commonRow.Item("Acquirente_Rag_Soc"),
                       .Nome = commonRow.Item("Acquirente_Nome"),
                       .Cognome = commonRow.Item("Acquirente_Cognome"),
                       .ID_CF = CInt(commonRow.Item("Acquirente_Id_CF")),
                       .PIVA = commonRow.Item("Cod_Contatto_Acquirente"),
                       .CODICE_SOGGETTO = commonRow.Item("Acquirente_CodiceSoggetto"),
                       .COD_CONTATTO = commonRow.Item("Cod_Contatto_Acquirente")
                   }
                    result.Acquirente = acquirente
                Else
                    result.Acquirente = Nothing
                End If

                ' Venditore
                result.HaVenditore = CBool(commonRow.Item("Venditore_Specified"))
                If result.HaVenditore Then
                    Dim venditore As New MVV_Venditore With
                   {
                       .TipoSoggetto = "Venditore",
                       .Indirizzo = New MVV_Indirizzo With
                       {
                           .CAP = commonRow.Item("Venditore_CAP"),
                           .Comune_Provincia = commonRow.Item("Venditore_Comune_Provincia"),
                           .Indirizzo = commonRow.Item("Venditore_Indirizzo"),
                           .Stato = commonRow.Item("Venditore_Stato"),
                           .Istat_Provincia = commonRow.Item("Venditore_Provincia"),
                           .Istat_Comune = commonRow.Item("Venditore_Comune")
                       },
                       .Denominazione = commonRow.Item("Venditore_Rag_Soc"),
                       .Nome = commonRow.Item("Venditore_Nome"),
                       .Cognome = commonRow.Item("Venditore_Cognome"),
                       .ID_CF = CInt(commonRow.Item("Venditore_Id_CF")),
                       .PIVA = commonRow.Item("Cod_Contatto_Venditore"),
                       .CODICE_SOGGETTO = commonRow.Item("Venditore_CodiceSoggetto"),
                       .COD_CONTATTO = commonRow.Item("Cod_Contatto_Venditore")
                   }
                    result.Venditore = venditore
                Else
                    result.Venditore = Nothing
                End If

                ' Luogo Consegna
                result.HaLuogoConsegnaDiverso = CBool(commonRow.Item("Cod_Indirizzo_Consegna_Specified"))
                If result.HaLuogoConsegnaDiverso Then
                    result.CodIndirizzoConsegnaDiverso = CInt(commonRow.Item("Luogo_Consegna"))
                Else
                    result.LuogoConsegnaDiverso = commonRow.Item("Luogo_Consegna").ToString()
                End If

                Dim mt = New MVV_MezzoTrasporto With
            {
                .Tipo = commonRow.Item("MezzoTrasporto_Unita"),
                .Codice = CInt(commonRow.Item("MezzoTrasporto_Codice_Unita")),
                .Targa = commonRow.Item("MezzoTrasporto_Targa"),
                .TargaRimorchio = commonRow.Item("MezzoTrasporto_Immatricolazione2"),
                .NumeroAutorizzazione = commonRow.Item("MezzoTrasporto_Num_Aut")
            }
                Dim dataAutorizTrasp = commonRow.Item("MezzoTrasporto_Data_Aut").ToString()
                If CDate(dataAutorizTrasp) = AGRODATAINIZIO Then
                    mt.DataAutorizzazione = String.Empty
                Else
                    mt.DataAutorizzazione = CDate(dataAutorizTrasp).ToString("dd/MM/yyyy")
                End If

                result.MezzoTrasoprto = mt

                Dim dettagli = New List(Of MVV_Dettaglio)

                For Each row As DataRow In table.AsEnumerable()

                    Dim d = New MVV_Dettaglio With
                {
                    .ID_Mov_Det = CInt(row.Item("Id_Mov_Det")),
                    .DescrizioneGias = row.Item("Mov_Det_Des"),
                    .DescrizioneGiasAddizionale = row.Item("Mov_Det_Des_extra"),
                    .MateriePrime_CodiceArticolo = row.Item("cod_Articolo"),
                    .MateriePrime_DescrizioneCommerciale = row.Item("Descrizione_commerciale"),
                    .CodiceNC = row.Item("Codice_NC"),
                    .Lotto = row.Item("Reg_V_Lotto"),
                    .Mat_Cod = row.Item("Mat_Cod"),
                    .TitoloAlcolTot = CDec(row.Item("Reg_V_TitoloAlcolTot")),
                    .TitoloAlcolPot = CDec(row.Item("Reg_V_TitoloAlcolPot")),
                    .TitoloAlcolEff = CDec(row.Item("Reg_V_TitoloAlcolEff")),
                    .Densita = 0,
                    .CapacitaImballo = CDec(row.Item("Extra_Titolo_Alcol")),
                    .CapacitaImballo_MateriaPrima = CDec(row.Item("CapacitaImballo")),
                    .NumeroColli = CInt(row.Item("NumeroColli")),
                    .NumeroImballi = CInt(row.Item("NumeroImballi")),
                    .DescrizioneColli = row.Item("DesColli"),
                    .DescrizioneImballi = row.Item("DesImballi"),
                    .CodiceGenerazione = CInt(row.Item("Codice_Generazione")),
                    .RegistroVino = New RegistroVino With
                    {
                        .Annata = New RegVinoAttributo(row.Item("Reg_V_Annata")),
                        .PercAnnata = row.Item("Reg_V_PercAnnata").ToString(),
                        .AttoCert = New RegVinoAttributo(row.Item("Reg_V_AttoCert")),
                        .Biologico = New RegVinoAttributo(row.Item("Reg_V_Biologico")),
                        .CodCategoria = New RegVinoAttributo(row.Item("Reg_V_CodCategoria")),
                        .CodClassificazione = New RegVinoAttributo(row.Item("Reg_V_CodClassificazione")),
                        .CodColore = New RegVinoAttributo(row.Item("Reg_V_CodColore")),
                        .CodStatoFisico = New RegVinoAttributo(row.Item("Reg_V_CodStatoFisico")),
                        .OrigineUve = New RegVinoAttributo(row.Item("Reg_V_OrigineUve")),
                        .Provenienza = New RegVinoAttributo(row.Item("Reg_V_Provenienza")),
                        .Varieta = New RegVinoAttributo(row.Item("Reg_V_Varieta")),
                        .AltreVarieta = row.Item("Reg_V_Altrevarieta").ToString,
                        .Menzioni = row.Item("Reg_V_Menzioni").ToString,
                        .CodZonaViticola = New RegVinoAttributo(row.Item("Reg_V_CodZonaViticola")),
                        .DocIGP = New RegVinoAttributo(row.Item("Reg_V_CodDopIgp")),
                        .CodEbacchus = New RegVinoAttributo(row.Item("Reg_V_CodEbacchus")),
                        .Designazione = row.Item("Reg_V_Designazione"),
                        .CodOperazioneVit = New RegVinoAttributo(row.Item("Reg_V_PraticheEno")),
                        .CodSottozona = New RegVinoAttributo(row.Item("Reg_V_CodSottozona")),
                        .COdVigna = New RegVinoAttributo(row.Item("Reg_V_CodVigna")),
                        .CodPartita = row.Item("Reg_V_CodPartita").ToString(),
                        .MassaVolumica = Convert.ToDecimal(row.Item("Reg_V_MassaVolumica")),
                        .NumCertDOP = row.Item("Reg_V_NumCertDOP").ToString(),
                        .DataCertDOP = If(IsDBNull(row.Item("Reg_V_DataCertDOP")), AGRODATAFINE, CDate(row.Item("Reg_V_DataCertDOP"))),
                        .PraticheEnologiche = row.Item("Reg_V_PraticheEno").ToString(),
                        .PaesiProvenienza = row.Item("Reg_V_PaesiProvenienza").ToString(),
                        .CodTenoreZucchero = New RegVinoAttributo(row.Item("Reg_V_CodTenoreZucchero"))
                    },
                    .CodZonaViticola_Linea = row.Item("CodZonaViticola_Linea"),
                    .CodZonaViticola_Gen = row.Item("CodZonaViticola_Gen"),
                    .Extra_Zona_Viticola = row.Item("udm_cod_extra_mp"),
                    .UDMQTA = New MVV_UDM_QTA With
                    {
                        .UDM_COD_EXTRA = CInt(row.Item("UDM_COD_EXTRA")),
                        .UDM_DES = row.Item("UDM_DES"),
                        .UDM_DES_EXTRA = row.Item("UDM_DES_EXTRA"),
                        .UDM_SIM = row.Item("UDM_SIM"),
                        .UDM_SIM_EXTRA = .UDM_SIM = row.Item("udm_sim_extra"),
                        .UDM_SIN_EXTRA2 = row.Item("udm_sim_extra"),
                        .QTA_EXtra_Tot = CDec(row.Item("Qta_Extra_Totale")),
                        .QTA = CDec(row.Item("Qta")),
                        .QTA_Extra = If(IsDBNull(row.Item("QTA_Extra")), 0, CDec(row.Item("QTA_Extra")))
                    },
                    .UDMQTA_MAteriaPrima = New MVV_UDM_QTA_MATERIA_PRIMA With
                    {
                        .UDM_COD = row.Item("udm_cod_mp"),
                        .UDM_COD_EXTRA = row.Item("udm_cod_extra_mp"),
                        .UDM_DES = row.Item("UDM_DES_Materie_Prime"),
                        .UDM_DES_EXTRA = row.Item("UDM_DES_EXTRA_Materie_Prime"),
                        .UDM_SIM = row.Item("UDM_SIM_Materie_Prime"),
                        .UDM_SIM_EXTRA = row.Item("UDM_SIM_EXTRA_Materie_Prime")
                    },
                    .CategoriaVinoCod = CInt(row.Item("categoria_vino_cod"))
                }

                    If (row.Item("Reg_V_VolNominale")) <> 0 Then
                        d.VolNominale = CDec(row.Item("Reg_V_VolNominale"))
                    End If


                    If d.CodiceNC = "-1" OrElse d.CodiceNC = "0" Then d.CodiceNC = String.Empty
                    If d.CategoriaVinoCod = "-1" OrElse d.CategoriaVinoCod = "0" Then d.CategoriaVinoCod = String.Empty

                    dettagli.Add(d)
                Next

                result.Dettagli = dettagli.OrderBy(Function(d) d.ID_Mov_Det).ToList()
            End If
        Catch ex As Exception
            Console.WriteLine(ex.Message)
        End Try

        Return result


    End Function

    <Extension()>
    Public Function ToDictionary(ByVal lista As List(Of TeleRegistri_Dettagli)) As Dictionary(Of String, Dictionary(Of String, String))

        Dim dicAttributi As New Dictionary(Of String, Dictionary(Of String, String))

        Dim attributi = lista.Select(Function(a) a.Tabella_Cod).Distinct().ToList()

        For Each attributo As String In attributi

            Dim valori = lista.Where(Function(d) d.Tabella_Cod.Equals(attributo)).OrderBy(Function(f) f.Descrizione).ToList()
            Dim dicValori As New Dictionary(Of String, String)

            For Each valore As TeleRegistri_Dettagli In valori
                dicValori.Add(valore.Codice, valore.Descrizione)
            Next
            dicAttributi.Add(attributo, dicValori)

        Next

        Return dicAttributi

    End Function

    <Extension()>
    Public Function ValoreDizionario(ByVal dizionario As Dictionary(Of String, Dictionary(Of String, String)), ByVal chiave1 As String, ByVal chiave2 As String) As String

        If Not dizionario.ContainsKey(chiave1) Then
            Return String.Empty
        End If

        If Not dizionario(chiave1).ContainsKey(chiave2) Then
            Return String.Empty
        End If

        Return dizionario(chiave1)(chiave2)

    End Function

    <Extension()>
    Public Function ToString(ByVal soggetto As MVV_Anagrafica) As String

        Return String.Format("{0}", soggetto.Denominazione)

    End Function

End Module
