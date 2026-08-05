Imports System.Data.Entity
Imports AgronicaCoreDataProvider
Imports AgronicaCoreEntityFramework

Public Class Mov_Dettaglio_Tecnico_Extra_R

End Class

Public Class Mov_Dettaglio_Tecnico_Extra_W
    Inherits AgronicaCoreDataProvider.LogProvider

    Public Sub Scrivi(ByRef Movimenti As AgronicaCoreEntityFramework_POCO.Mov_Dettaglio_Tecnico_Extra,
                      ByRef GiasContext As Gias_DeveloperServer_Entities,
                      ByRef objParametriServer As AgronicaCoreParametri)

        Const nomeRoutine = "ContabDAL.Mov_Dettaglio_Tecnico_Extra_W.Scrivi()"
        Dim messaggioErrore As String = ""

        Try
            Valorizza(Movimenti, objParametriServer)

            Movimenti.data_creazione = DateTime.Now
            Movimenti.data_modifica = DateTime.Now
            Movimenti.username_creazione = objParametriServer.UsernameOperazione
            Movimenti.username_modifica = objParametriServer.UsernameOperazione

            GiasContext.Mov_Dettaglio_Tecnico_Extra.Add(Movimenti)

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

    End Sub

    Public Sub Modifica(ByRef Movimenti As AgronicaCoreEntityFramework_POCO.Mov_Dettaglio_Tecnico_Extra,
                        ByRef GiasContext As Gias_DeveloperServer_Entities,
                        ByRef objParametriServer As AgronicaCoreParametri)

        Const nomeRoutine = "ContabDAL.Mov_Dettaglio_Tecnico_Extra_W.Modifica()"
        Dim messaggioErrore As String = ""

		Try

			Valorizza(Movimenti, objParametriServer)

			Movimenti.data_modifica = DateTime.Now
			Movimenti.username_modifica = objParametriServer.UsernameOperazione

			GiasContext.Entry(Movimenti).State = EntityState.Modified

		Catch ex As Exception
			messaggioErrore = ex.Message
            Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

    End Sub

    Public Sub Valorizza(ByRef Movimenti As AgronicaCoreEntityFramework_POCO.Mov_Dettaglio_Tecnico_Extra,
                         ByRef objParametriServer As AgronicaCoreParametri)

        Const nomeRoutine = "ContabDAL.Mov_Dettaglio_Tecnico_Extra_W.Valorizza()"
        Dim messaggioErrore As String = ""

        Try

			If Movimenti.Piva Is Nothing Then
				Movimenti.Piva = ""
			End If
			'If Movimenti.Sa_Cod Is Nothing Then
			'	Movimenti.Piva = ""
			'End If
			'If Movimenti.Id_Agenda Is Nothing Then
			'	Movimenti.Piva = ""
			'End If
			'If Movimenti.Id_Mov Is Nothing Then
			'	Movimenti.Piva = ""
			'End If
			'If Movimenti.Id_Mov_Det Is Nothing Then
			'	Movimenti.Piva = ""
			'End If
			'If Movimenti.Id_Reg_Dettaglio Is Nothing Then
			'	Movimenti.Piva = ""
			'End If
			If Movimenti.Regione Is Nothing Then
				Movimenti.Regione = ""
			End If
			If Movimenti.ASL Is Nothing Then
				Movimenti.ASL = ""
			End If
			If Movimenti.Serie Is Nothing Then
				Movimenti.Serie = ""
			End If
			If Movimenti.Numero Is Nothing Then
				Movimenti.Numero = ""
			End If
			'If Movimenti.Mac_Cod Is Nothing Then
			'	Movimenti.Piva = ""
			'End If
			'If Movimenti.Cod_RisUm Is Nothing Then
			'	Movimenti.Piva = ""
			'End If
			If Movimenti.Trasportatore Is Nothing Then
				Movimenti.Trasportatore = ""
			End If
			If Movimenti.Mezzo_Trasporto Is Nothing Then
				Movimenti.Mezzo_Trasporto = ""
			End If
			If Movimenti.Targa Is Nothing Then
				Movimenti.Targa = ""
			End If
			If Movimenti.N_Immatricolazione Is Nothing Then
				Movimenti.N_Immatricolazione = ""
			End If
			If Movimenti.N_Immatricolazione_Rimorchio Is Nothing Then
				Movimenti.N_Immatricolazione_Rimorchio = ""
			End If
			If Movimenti.N_Autorizzazione_Trasporto Is Nothing Then
				Movimenti.N_Autorizzazione_Trasporto = ""
			End If
			If Movimenti.Data_Rilascio_Autorizzazione < AgronicaCoreDataProvider.CostantiPersonalizzate.AGRODATAINIZIO Then
				Movimenti.Data_Rilascio_Autorizzazione = AgronicaCoreDataProvider.CostantiPersonalizzate.AGRODATAINIZIO
			End If
			'If Movimenti.Peso Is Nothing Then
			'	Movimenti.Piva = ""
			'End If
			If Movimenti.inviato Is Nothing Then
				Movimenti.inviato = 0
			End If
			'If Movimenti.datainvio Is Nothing Then
			'	Movimenti.datainvio = AgronicaCoreDataProvider.CostantiPersonalizzate.AGRODATAINIZIO
			'End If
			'If Movimenti.data_creazione Is Nothing Then
			'	Movimenti.Piva = ""
			'End If
			'If Movimenti.data_modifica Is Nothing Then
			'	Movimenti.Piva = ""
			'End If
			If Movimenti.username_creazione Is Nothing Then
				Movimenti.username_creazione = objParametriServer.UtenteCodFiscale
			End If
			If Movimenti.username_modifica Is Nothing Then
				Movimenti.username_modifica = objParametriServer.UtenteCodFiscale
			End If
			'If Movimenti.Codice_Prodotto Is Nothing Then
			'	Movimenti.Piva = ""
			'End If
			'If Movimenti.Colore Is Nothing Then
			'	Movimenti.Piva = ""
			'End If
			If Movimenti.Zona_Viticola Is Nothing Then
				Movimenti.Zona_Viticola = ""
			End If
			'If Movimenti.Manipolazioni Is Nothing Then
			'	Movimenti.Piva = ""
			'End If
			If Movimenti.Precisazioni Is Nothing Then
				Movimenti.Precisazioni = ""
			End If
			If Movimenti.Annotazioni Is Nothing Then
				Movimenti.Annotazioni = ""
			End If
			'If Movimenti.Num_Contenitori Is Nothing Then
			'	Movimenti.Piva = ""
			'End If
			If Movimenti.Marche_Contenitori Is Nothing Then
				Movimenti.Marche_Contenitori = ""
			End If
			If Movimenti.Des_Contenitori Is Nothing Then
				Movimenti.Des_Contenitori = ""
			End If
			If Movimenti.Tipo_Documento Is Nothing Then
				Movimenti.Tipo_Documento = ""
			End If
			'If Movimenti.Id_Cod_Autorita Is Nothing Then
			'	Movimenti.Piva = ""
			'End If
			If Movimenti.Luogo_Partenza Is Nothing Then
				Movimenti.Luogo_Partenza = ""
			End If
			If Movimenti.Luogo_Consegna Is Nothing Then
				Movimenti.Luogo_Consegna = ""
			End If
			If Movimenti.Data_Spedizione < AgronicaCoreDataProvider.CostantiPersonalizzate.AGRODATAINIZIO Then
				Movimenti.Data_Spedizione = AgronicaCoreDataProvider.CostantiPersonalizzate.AGRODATAINIZIO
			End If
			If Movimenti.Indicazioni_Complementari Is Nothing Then
				Movimenti.Indicazioni_Complementari = ""
			End If
			'If Movimenti.Titolo_Alcol Is Nothing Then
			'	Movimenti.Piva = ""
			'End If
			If Movimenti.Codice_NC Is Nothing Then
				Movimenti.Codice_NC = ""
			End If
			If Movimenti.Num_Riferimento Is Nothing Then
				Movimenti.Num_Riferimento = ""
			End If
			If Movimenti.Data_Dichiarazione < AgronicaCoreDataProvider.CostantiPersonalizzate.AGRODATAINIZIO Then
				Movimenti.Data_Dichiarazione = AgronicaCoreDataProvider.CostantiPersonalizzate.AGRODATAINIZIO
			End If
			If Movimenti.Garanzia Is Nothing Then
				Movimenti.Garanzia = ""
			End If
			If Movimenti.Certificati Is Nothing Then
				Movimenti.Certificati = ""
			End If
			If Movimenti.Durata_Viaggio Is Nothing Then
				Movimenti.Durata_Viaggio = ""
			End If
			If Movimenti.Tipo_Trasporto Is Nothing Then
				Movimenti.Tipo_Trasporto = 0
			End If
			If Movimenti.Unita_Trasporto Is Nothing Then
				Movimenti.Unita_Trasporto = 0
			End If
			If Movimenti.Codice_Alternativo Is Nothing Then
				Movimenti.Codice_Alternativo = ""
			End If
			If Movimenti.Id_Gestione_Vettore Is Nothing Then
				Movimenti.Id_Gestione_Vettore = 0
			End If
			If Movimenti.Ritenuta_Acconto_Cod Is Nothing Then
				Movimenti.Ritenuta_Acconto_Cod = 0
			End If
			If Movimenti.Ritenuta_Acconto Is Nothing Then
				Movimenti.Ritenuta_Acconto = 0
			End If
			If Movimenti.Enasarco_Cod Is Nothing Then
				Movimenti.Enasarco_Cod = 0
			End If
			If Movimenti.Enasarco Is Nothing Then
				Movimenti.Enasarco = 0
			End If
			If Movimenti.ACCDAA_Cod_Risum_Destinatario Is Nothing Then
				Movimenti.ACCDAA_Cod_Risum_Destinatario = 0
			End If
			If Movimenti.ACCDAA_Cod_Risum_Destinazione Is Nothing Then
				Movimenti.ACCDAA_Cod_Risum_Destinazione = 0
			End If
			If Movimenti.ACCDAA_Cod_IndirizzoRisum_Destinatario Is Nothing Then
				Movimenti.ACCDAA_Cod_IndirizzoRisum_Destinatario = 0
			End If
			If Movimenti.ACCDAA_Cod_IndirizzoRisum_Destinazione Is Nothing Then
				Movimenti.ACCDAA_Cod_IndirizzoRisum_Destinazione = 0
			End If
			If Movimenti.CapoArea_Cod Is Nothing Then
				Movimenti.CapoArea_Cod = 0
			End If
			If Movimenti.Provvigione_CapoArea Is Nothing Then
				Movimenti.Provvigione_CapoArea = 0
			End If
			If Movimenti.Provvigione_Pagata_Agente Is Nothing Then
				Movimenti.Provvigione_Pagata_Agente = 0
			End If
			If Movimenti.Provvigione_Pagata_CapoArea Is Nothing Then
				Movimenti.Provvigione_Pagata_CapoArea = 0
			End If
			If Movimenti.N_Doc_Cliente Is Nothing Then
				Movimenti.N_Doc_Cliente = ""
			End If
			If Movimenti.Data_Doc_Cliente Is Nothing Or Movimenti.Data_Doc_Cliente < AgronicaCoreDataProvider.CostantiPersonalizzate.AGRODATAINIZIO Then
				Movimenti.Data_Doc_Cliente = AgronicaCoreDataProvider.CostantiPersonalizzate.AGRODATAINIZIO
			End If
			If Movimenti.N_Nota_Fattura Is Nothing Then
				Movimenti.N_Nota_Fattura = ""
			End If
			If Movimenti.Data_Nota_Fattura Is Nothing Or Movimenti.Data_Nota_Fattura < AgronicaCoreDataProvider.CostantiPersonalizzate.AGRODATAINIZIO Then
				Movimenti.Data_Nota_Fattura = AgronicaCoreDataProvider.CostantiPersonalizzate.AGRODATAINIZIO
			End If
			If Movimenti.N_Nota_DDT Is Nothing Then
				Movimenti.N_Nota_DDT = ""
			End If
			If Movimenti.N_Nota_Riga_DDT Is Nothing Then
				Movimenti.N_Nota_Riga_DDT = ""
			End If
			If Movimenti.Data_Nota_DDT Is Nothing Or Movimenti.Data_Nota_DDT < AgronicaCoreDataProvider.CostantiPersonalizzate.AGRODATAINIZIO Then
				Movimenti.Data_Nota_DDT = AgronicaCoreDataProvider.CostantiPersonalizzate.AGRODATAINIZIO
			End If
			If Movimenti.Causale_Fattura Is Nothing Then
				Movimenti.Causale_Fattura = 0
			End If
			If Movimenti.N_Doc_Ente Is Nothing Then
				Movimenti.N_Doc_Ente = ""
			End If
			If Movimenti.Anno_Doc_Ente Is Nothing Then
				Movimenti.Anno_Doc_Ente = 0
			End If
			If Movimenti.Num_Conf_Riscontrate Is Nothing Then
				Movimenti.Num_Conf_Riscontrate = 0
			End If
			If Movimenti.Num_Colli_Riscontrati Is Nothing Then
				Movimenti.Num_Colli_Riscontrati = 0
			End If
			If Movimenti.Num_Imballi_Riscontrati Is Nothing Then
				Movimenti.Num_Imballi_Riscontrati = 0
			End If
			If Movimenti.Peso_Netto_Riscontrato Is Nothing Then
				Movimenti.Peso_Netto_Riscontrato = 0
			End If
			If Movimenti.Peso_Lordo_Riscontrato Is Nothing Then
				Movimenti.Peso_Lordo_Riscontrato = 0
			End If
			If Movimenti.Tara_Unit_Conf_Riscontrata Is Nothing Then
				Movimenti.Tara_Unit_Conf_Riscontrata = 0
			End If
			If Movimenti.Tara_Unit_Collo_Riscontrata Is Nothing Then
				Movimenti.Tara_Unit_Collo_Riscontrata = 0
			End If
			If Movimenti.Tara_Unit_Imballo_Riscontrata Is Nothing Then
				Movimenti.Tara_Unit_Imballo_Riscontrata = 0
			End If
			'If Movimenti.Distanza_Trasporto_Udm Is Nothing Then
			'	Movimenti.Piva = ""
			'End If
			'If Movimenti.Distanza_Trasporto Is Nothing Then
			'	Movimenti.Piva = ""
			'End If

			If Movimenti.validita_inizio < AgronicaCoreDataProvider.CostantiPersonalizzate.AGRODATAINIZIO Then
				Movimenti.validita_inizio = AgronicaCoreDataProvider.CostantiPersonalizzate.AGRODATAINIZIO
			End If

			If Movimenti.validita_fine < AgronicaCoreDataProvider.CostantiPersonalizzate.AGRODATAINIZIO Then
				Movimenti.validita_fine = AgronicaCoreDataProvider.CostantiPersonalizzate.AGRODATAFINE
			End If

		Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

    End Sub

    Public Sub Elimina(ByRef Movimenti As AgronicaCoreEntityFramework_POCO.Mov_Dettaglio_Tecnico_Extra,
                       ByRef GiasContext As Gias_DeveloperServer_Entities,
                       ByRef objParametriServer As AgronicaCoreParametri)

        Const nomeRoutine = "ContabDAL.Mov_Dettaglio_Tecnico_Extra_W.Elimina()"
        Dim messaggioErrore As String = ""

        Try

            GiasContext.Mov_Dettaglio_Tecnico_Extra.Remove(Movimenti)

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try
    End Sub


End Class
