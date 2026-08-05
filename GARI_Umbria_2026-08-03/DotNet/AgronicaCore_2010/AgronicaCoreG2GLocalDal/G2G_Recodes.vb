Imports System.Linq.Expressions

Imports System.Transactions
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreModello

Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri

Public Class G2G_Recodes_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function Leggi_MassimaData(
        ByVal NomeTabella As String,
        ByVal PivaSuperUser_Origine As String,
        ByVal PivaSuperUser_Destinazione As String,
        ByVal xFiltroAggiuntivo As String,
        ByVal xOrderBy As String,
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
    ) As DateTime

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCore_DAL.Leggi()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Stb.Length = 0

            Stb.AppendLine(" SELECT isnull(MAX(Data_Modifica), '01/01/1900') as Data_Modifica  " & vbCrLf)
            Stb.AppendLine(" FROM  " & NomeTabella & vbCrLf)

            Stb.AppendLine(" WHERE 1=1  ")

            If PivaSuperUser_Origine Then
                Stb.AppendLine(" AND  From_PivaSuperUser='" & Agro_SQL_SaveText(PivaSuperUser_Origine) & "' ")
            End If

            If PivaSuperUser_Destinazione Then
                Stb.AppendLine(" AND  To_PivaSuperUser='" & Agro_SQL_SaveText(PivaSuperUser_Destinazione) & "' ")
            End If

            If xFiltroAggiuntivo <> "" Then
                Stb.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                Stb.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        If DT.Rows.Count = 0 Then
            Return AGRODATAINIZIO
        Else
            Return DT(0)("Data_Modifica")
        End If


    End Function

    Public Function LeggiPerRequestGias2Gias(
        ByVal DataRiferimento As Date,
        ByVal PivaSuperUser_Origine As String,
        ByVal PivaSuperUser_Destinazione As String,
        ByRef objParametri As AgronicaCoreParametri
    ) As G2G_Recodes_Request



        Dim NomeRoutine As String = "G2GlocalDal.Analisi_Testata_R.LeggiPerGias2Gias()"



        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
        Dim GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

        GiasContext.Database.CommandTimeout = 3600

        Dim rval As New G2G_Recodes_Request

        rval.FromPivaSuperUser = PivaSuperUser_Origine

        rval.G2G_Recode_Pratiche_DataRiferimento = If(DataRiferimento <> AGRODATAINIZIO, DataRiferimento, Leggi_MassimaData("G2G_Recode_Pratiche", PivaSuperUser_Origine, PivaSuperUser_Destinazione, "", "", objParametri))
        rval.G2G_Recode_Pratiche_Stati_DataRiferimento = If(DataRiferimento <> AGRODATAINIZIO, DataRiferimento, Leggi_MassimaData("G2G_Recode_Pratiche_Stati", PivaSuperUser_Origine, PivaSuperUser_Destinazione, "", "", objParametri))
        rval.G2G_Recode_Programmazione_Entita_DataRiferimento = If(DataRiferimento <> AGRODATAINIZIO, DataRiferimento, Leggi_MassimaData("G2G_Recode_Programmazione_Entita", PivaSuperUser_Origine, PivaSuperUser_Destinazione, "", "", objParametri))
        rval.G2G_Recode_Programmazione_Testata_DataRiferimento = If(DataRiferimento <> AGRODATAINIZIO, DataRiferimento, Leggi_MassimaData("G2G_Recode_Programmazione_Testata", PivaSuperUser_Origine, PivaSuperUser_Destinazione, "", "", objParametri))
        rval.G2G_Recode_Pua_DataRiferimento = If(DataRiferimento <> AGRODATAINIZIO, DataRiferimento, Leggi_MassimaData("G2G_Recode_Pua", PivaSuperUser_Origine, PivaSuperUser_Destinazione, "", "", objParametri))
        rval.G2G_Recode_ParticelleCatastalixVincoliAgronomici_DataRiferimento = If(DataRiferimento <> AGRODATAINIZIO, DataRiferimento, Leggi_MassimaData("G2G_Recode_ParticelleCatastalixVincoliAgronomici", PivaSuperUser_Origine, PivaSuperUser_Destinazione, "", "", objParametri))
        rval.G2G_Recode_ImpreseCentri_DataRiferimento = If(DataRiferimento <> AGRODATAINIZIO, DataRiferimento, Leggi_MassimaData("G2G_Recode_Imprese", PivaSuperUser_Origine, PivaSuperUser_Destinazione, "", "", objParametri))
        rval.G2G_Recode_Campi_DataRiferimento = If(DataRiferimento <> AGRODATAINIZIO, DataRiferimento, Leggi_MassimaData("G2G_Recode_Campo", PivaSuperUser_Origine, PivaSuperUser_Destinazione, "", "", objParametri))
        rval.G2G_Recode_Appezzamenti_DataRiferimento = If(DataRiferimento <> AGRODATAINIZIO, DataRiferimento, Leggi_MassimaData("G2G_Recode_Appezzamenti", PivaSuperUser_Origine, PivaSuperUser_Destinazione, "", "", objParametri))
        rval.G2G_Recode_Impianti_DataRiferimento = If(DataRiferimento <> AGRODATAINIZIO, DataRiferimento, Leggi_MassimaData("G2G_Recode_Impianti", PivaSuperUser_Origine, PivaSuperUser_Destinazione, "", "", objParametri))
        rval.G2G_Recode_Distinte_DataRiferimento = If(DataRiferimento <> AGRODATAINIZIO, DataRiferimento, Leggi_MassimaData("G2G_Recode_Distinta", PivaSuperUser_Origine, PivaSuperUser_Destinazione, "", "", objParametri))
        rval.G2G_Recode_Contatti_DataRiferimento = If(DataRiferimento <> AGRODATAINIZIO, DataRiferimento, Leggi_MassimaData("G2G_Recode_Contatti", PivaSuperUser_Origine, PivaSuperUser_Destinazione, "", "", objParametri))
        rval.G2G_Recode_Macchine_DataRiferimento = If(DataRiferimento <> AGRODATAINIZIO, DataRiferimento, Leggi_MassimaData("G2G_Recode_Parco_Macchine", PivaSuperUser_Origine, PivaSuperUser_Destinazione, "", "", objParametri))
        rval.G2G_Recode_MateriePrime_DataRiferimento = If(DataRiferimento <> AGRODATAINIZIO, DataRiferimento, Leggi_MassimaData("G2G_Recode_MateriePrime", PivaSuperUser_Origine, PivaSuperUser_Destinazione, "", "", objParametri))
        rval.G2G_Recode_MateriePrimeCampionature_DataRiferimento = If(DataRiferimento <> AGRODATAINIZIO, DataRiferimento, Leggi_MassimaData("G2G_Recode_Materie_Prime_Campionature", PivaSuperUser_Origine, PivaSuperUser_Destinazione, "", "", objParametri))
        rval.G2G_Recode_Fabbricati_DataRiferimento = If(DataRiferimento <> AGRODATAINIZIO, DataRiferimento, Leggi_MassimaData("G2G_Recode_Fabbricati", PivaSuperUser_Origine, PivaSuperUser_Destinazione, "", "", objParametri))
        rval.G2G_Recode_Allegati_DataRiferimento = If(DataRiferimento <> AGRODATAINIZIO, DataRiferimento, Leggi_MassimaData("G2G_Recode_Allegati", PivaSuperUser_Origine, PivaSuperUser_Destinazione, "", "", objParametri))
        rval.G2G_Recode_Allegati_Entita_DataRiferimento = If(DataRiferimento <> AGRODATAINIZIO, DataRiferimento, Leggi_MassimaData("G2G_Recode_Allegati_Entita", PivaSuperUser_Origine, PivaSuperUser_Destinazione, "", "", objParametri))
        rval.G2G_Recode_Indirizzi_DataRiferimento = If(DataRiferimento <> AGRODATAINIZIO, DataRiferimento, Leggi_MassimaData("G2G_Recode_Indirizzi", PivaSuperUser_Origine, PivaSuperUser_Destinazione, "", "", objParametri))
        rval.G2G_Recode_Analisi_Certificato_DataRiferimento = If(DataRiferimento <> AGRODATAINIZIO, DataRiferimento, Leggi_MassimaData("G2G_Recode_Analisi_Certificato", PivaSuperUser_Origine, PivaSuperUser_Destinazione, "", "", objParametri))
        rval.G2G_Recode_Analisi_Testata_DataRiferimento = If(DataRiferimento <> AGRODATAINIZIO, DataRiferimento, Leggi_MassimaData("G2G_Recode_Analisi_Testata", PivaSuperUser_Origine, PivaSuperUser_Destinazione, "", "", objParametri))
        rval.G2G_Recode_Analisi_Dettagli_DataRiferimento = If(DataRiferimento <> AGRODATAINIZIO, DataRiferimento, Leggi_MassimaData("G2G_Recode_Analisi_Dettagli", PivaSuperUser_Origine, PivaSuperUser_Destinazione, "", "", objParametri))
        rval.G2G_Recode_Analisi_EntitaxTestata_DataRiferimento = If(DataRiferimento <> AGRODATAINIZIO, DataRiferimento, Leggi_MassimaData("G2G_Recode_Analisi_EntitaxTestata", PivaSuperUser_Origine, PivaSuperUser_Destinazione, "", "", objParametri))
        rval.G2G_Recode_Analisi_Campioni_DataRiferimento = If(DataRiferimento <> AGRODATAINIZIO, DataRiferimento, Leggi_MassimaData("G2G_Recode_Analisi_Campioni", PivaSuperUser_Origine, PivaSuperUser_Destinazione, "", "", objParametri))
        rval.G2G_Recode_PianoConcimazione_Testata_DataRiferimento = If(DataRiferimento <> AGRODATAINIZIO, DataRiferimento, Leggi_MassimaData("G2G_Recode_PianoConcimazione_Testata", PivaSuperUser_Origine, PivaSuperUser_Destinazione, "", "", objParametri))
        rval.G2G_Recode_PianoConcimazione_Dettagli_DataRiferimento = If(DataRiferimento <> AGRODATAINIZIO, DataRiferimento, Leggi_MassimaData("G2G_Recode_PianoConcimazione_Dettagli", PivaSuperUser_Origine, PivaSuperUser_Destinazione, "", "", objParametri))
        rval.G2G_Recode_PianoConcimazione_EntitaxTestata_DataRiferimento = If(DataRiferimento <> AGRODATAINIZIO, DataRiferimento, Leggi_MassimaData("G2G_Recode_PianoConcimazione_EntitaxTestata", PivaSuperUser_Origine, PivaSuperUser_Destinazione, "", "", objParametri))
        rval.G2G_Recode_PianoConcimazione_Elaborazioni_DataRiferimento = If(DataRiferimento <> AGRODATAINIZIO, DataRiferimento, Leggi_MassimaData("G2G_Recode_PianoConcimazione_Elaborazioni", PivaSuperUser_Origine, PivaSuperUser_Destinazione, "", "", objParametri))
        rval.G2G_Recode_Ricette_DataRiferimento = If(DataRiferimento <> AGRODATAINIZIO, DataRiferimento, Leggi_MassimaData("G2G_Recode_Ricette", PivaSuperUser_Origine, PivaSuperUser_Destinazione, "", "", objParametri))
        rval.G2G_Recode_Ricette_Operazioni_DataRiferimento = If(DataRiferimento <> AGRODATAINIZIO, DataRiferimento, Leggi_MassimaData("G2G_Recode_Ricette_Operazioni", PivaSuperUser_Origine, PivaSuperUser_Destinazione, "", "", objParametri))
        rval.G2G_Recode_Ricette_Dettagli_DataRiferimento = If(DataRiferimento <> AGRODATAINIZIO, DataRiferimento, Leggi_MassimaData("G2G_Recode_Ricette_Dettagli", PivaSuperUser_Origine, PivaSuperUser_Destinazione, "", "", objParametri))
        rval.G2G_Recode_Ricette_Dettaglio_Tecnico_DataRiferimento = If(DataRiferimento <> AGRODATAINIZIO, DataRiferimento, Leggi_MassimaData("G2G_Recode_Ricette_Dettaglio_Tecnico", PivaSuperUser_Origine, PivaSuperUser_Destinazione, "", "", objParametri))
        rval.G2G_Recode_Ricette_Destinazioni_DataRiferimento = If(DataRiferimento <> AGRODATAINIZIO, DataRiferimento, Leggi_MassimaData("G2G_Recode_Ricette_Destinazioni", PivaSuperUser_Origine, PivaSuperUser_Destinazione, "", "", objParametri))
        rval.G2G_Recode_PUA_LetamazioniPrecedenti_DataRiferimento = If(DataRiferimento <> AGRODATAINIZIO, DataRiferimento, Leggi_MassimaData("G2G_Recode_PUA_LetamazioniPrecedenti", PivaSuperUser_Origine, PivaSuperUser_Destinazione, "", "", objParametri))
        rval.G2G_Recode_PUA_Effluente_DataRiferimento = If(DataRiferimento <> AGRODATAINIZIO, DataRiferimento, Leggi_MassimaData("G2G_Recode_PUA_Effluente", PivaSuperUser_Origine, PivaSuperUser_Destinazione, "", "", objParametri))
        rval.G2G_Recode_PUA_PeriodoDivieto_DataRiferimento = If(DataRiferimento <> AGRODATAINIZIO, DataRiferimento, Leggi_MassimaData("G2G_Recode_PUA_PeriodoDivieto", PivaSuperUser_Origine, PivaSuperUser_Destinazione, "", "", objParametri))
        rval.G2G_Recode_Anagrafe_VincoliAgronomici_DataRiferimento = If(DataRiferimento <> AGRODATAINIZIO, DataRiferimento, Leggi_MassimaData("G2G_Recode_Anagrafe_VincoliAgronomici", PivaSuperUser_Origine, PivaSuperUser_Destinazione, "", "", objParametri))
        rval.G2G_Recode_Attivita_DataRiferimento = If(DataRiferimento <> AGRODATAINIZIO, DataRiferimento, Leggi_MassimaData("G2G_Recode_Attivita", PivaSuperUser_Origine, PivaSuperUser_Destinazione, "", "", objParametri))

        Return rval

    End Function

    Public Function LeggiPerResponseGias2Gias(
        ByVal G2G_Recodes_Request As G2G_Recodes_Request,
        ByRef objParametri As AgronicaCoreParametri
    ) As G2G_Recode

        Dim NomeRoutine As String = "G2G_Recodes.LeggiPerResponseGias2Gias()"

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
        Dim GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

        GiasContext.Database.CommandTimeout = 3600

        Dim rval As New G2G_Recode

        ' VAnni: 28/6/2019: leggo i soli oggetti in insert, sarà il client a stabilire cosa è UPDATE..
        rval.G2GRecodeAllegatiToInsert = (From p In GiasContext.G2G_Recode_Allegati Where p.From_PivaSuperUser = G2G_Recodes_Request.FromPivaSuperUser And p.Data_Modifica > G2G_Recodes_Request.G2G_Recode_Allegati_DataRiferimento).ToList()
        rval.G2GRecodeAllegati_EntitaToInsert = (From p In GiasContext.G2G_Recode_Allegati_Entita Where p.From_PivaSuperUser = G2G_Recodes_Request.FromPivaSuperUser And p.Data_Modifica > G2G_Recodes_Request.G2G_Recode_Allegati_Entita_DataRiferimento).ToList()
        rval.G2GRecodeAnalisi_CampioniToInsert = (From p In GiasContext.G2G_Recode_Analisi_Campioni Where p.From_PivaSuperUser = G2G_Recodes_Request.FromPivaSuperUser And p.Data_Modifica > G2G_Recodes_Request.G2G_Recode_Analisi_Campioni_DataRiferimento).ToList()
        rval.G2GRecodeAnalisi_CertificatoToInsert = (From p In GiasContext.G2G_Recode_Analisi_Certificato Where p.From_PivaSuperUser = G2G_Recodes_Request.FromPivaSuperUser And p.Data_Modifica > G2G_Recodes_Request.G2G_Recode_Analisi_Certificato_DataRiferimento).ToList()
        rval.G2GRecodeAnalisi_DettagliToInsert = (From p In GiasContext.G2G_Recode_Analisi_Dettagli Where p.From_PivaSuperUser = G2G_Recodes_Request.FromPivaSuperUser And p.Data_Modifica > G2G_Recodes_Request.G2G_Recode_Analisi_Dettagli_DataRiferimento).ToList()
        rval.G2GRecodeAnalisi_EntitaxTestataToInsert = (From p In GiasContext.G2G_Recode_Analisi_EntitaxTestata Where p.From_PivaSuperUser = G2G_Recodes_Request.FromPivaSuperUser And p.Data_Modifica > G2G_Recodes_Request.G2G_Recode_Analisi_EntitaxTestata_DataRiferimento).ToList()
        rval.G2GRecodeAnalisi_TestataToInsert = (From p In GiasContext.G2G_Recode_Analisi_Testata Where p.From_PivaSuperUser = G2G_Recodes_Request.FromPivaSuperUser And p.Data_Modifica > G2G_Recodes_Request.G2G_Recode_Analisi_Testata_DataRiferimento).ToList()
        rval.G2GRecodeCampiToInsert = (From p In GiasContext.G2G_Recode_Campo Where p.From_PivaSuperUser = G2G_Recodes_Request.FromPivaSuperUser And p.Data_Modifica > G2G_Recodes_Request.G2G_Recode_Campi_DataRiferimento).ToList()
        rval.G2GRecodeAppezzamentiToInsert = (From p In GiasContext.G2G_Recode_Appezzamenti Where p.From_PivaSuperUser = G2G_Recodes_Request.FromPivaSuperUser And p.Data_Modifica > G2G_Recodes_Request.G2G_Recode_Appezzamenti_DataRiferimento).ToList()
        rval.G2GRecodeImpiantiToInsert = (From p In GiasContext.G2G_Recode_Impianti Where p.From_PivaSuperUser = G2G_Recodes_Request.FromPivaSuperUser And p.Data_Modifica > G2G_Recodes_Request.G2G_Recode_Impianti_DataRiferimento).ToList()
        rval.G2GRecodeDistinteToInsert = (From p In GiasContext.G2G_Recode_Distinta Where p.From_PivaSuperUser = G2G_Recodes_Request.FromPivaSuperUser And p.Data_Modifica > G2G_Recodes_Request.G2G_Recode_Distinte_DataRiferimento).ToList()
        rval.G2GRecodeContattiToInsert = (From p In GiasContext.G2G_Recode_Contatti Where p.From_PivaSuperUser = G2G_Recodes_Request.FromPivaSuperUser And p.Data_Modifica > G2G_Recodes_Request.G2G_Recode_Contatti_DataRiferimento).ToList()
        rval.G2GRecodeFabbricatiToInsert = (From p In GiasContext.G2G_Recode_Fabbricati Where p.From_PivaSuperUser = G2G_Recodes_Request.FromPivaSuperUser And p.Data_Modifica > G2G_Recodes_Request.G2G_Recode_Fabbricati_DataRiferimento).ToList()
        rval.G2GRecodeImpreseCentriToInsert = (From p In GiasContext.G2G_Recode_Imprese Where p.From_PivaSuperUser = G2G_Recodes_Request.FromPivaSuperUser And p.Data_Modifica > G2G_Recodes_Request.G2G_Recode_ImpreseCentri_DataRiferimento).ToList()
        rval.G2GRecodeIndirizziToInsert = (From p In GiasContext.G2G_Recode_Indirizzi Where p.From_PivaSuperUser = G2G_Recodes_Request.FromPivaSuperUser And p.Data_Modifica > G2G_Recodes_Request.G2G_Recode_Indirizzi_DataRiferimento).ToList()
        rval.G2GRecodeMateriePrimeCampionatureToInsert = (From p In GiasContext.G2G_Recode_Materie_Prime_Campionature Where p.From_PivaSuperUser = G2G_Recodes_Request.FromPivaSuperUser And p.Data_Modifica > G2G_Recodes_Request.G2G_Recode_MateriePrimeCampionature_DataRiferimento).ToList()
        rval.G2GRecodeMateriePrimeToInsert = (From p In GiasContext.G2G_Recode_MateriePrime Where p.From_PivaSuperUser = G2G_Recodes_Request.FromPivaSuperUser And p.Data_Modifica > G2G_Recodes_Request.G2G_Recode_MateriePrime_DataRiferimento).ToList()
        rval.G2GRecodeParcoMacchineToInsert = (From p In GiasContext.G2G_Recode_Parco_Macchine Where p.From_PivaSuperUser = G2G_Recodes_Request.FromPivaSuperUser And p.Data_Modifica > G2G_Recodes_Request.G2G_Recode_Macchine_DataRiferimento).ToList()
        rval.G2GRecodeParticelleCatastalixVincoliAgronomiciToInsert = (From p In GiasContext.G2G_Recode_ParticelleCatastalixVincoliAgronomici Where p.From_PivaSuperUser = G2G_Recodes_Request.FromPivaSuperUser And p.Data_Modifica > G2G_Recodes_Request.G2G_Recode_ParticelleCatastalixVincoliAgronomici_DataRiferimento).ToList()
        rval.G2GRecodePianoConcimazione_DettagliToInsert = (From p In GiasContext.G2G_Recode_PianoConcimazione_Dettagli Where p.From_PivaSuperUser = G2G_Recodes_Request.FromPivaSuperUser And p.Data_Modifica > G2G_Recodes_Request.G2G_Recode_PianoConcimazione_Dettagli_DataRiferimento).ToList()
        rval.G2GRecodePianoConcimazione_ElaborazioniToInsert = (From p In GiasContext.G2G_Recode_PianoConcimazione_Elaborazioni Where p.From_PivaSuperUser = G2G_Recodes_Request.FromPivaSuperUser And p.Data_Modifica > G2G_Recodes_Request.G2G_Recode_PianoConcimazione_Elaborazioni_DataRiferimento).ToList()
        rval.G2GRecodePianoConcimazione_EntitaxTestataToInsert = (From p In GiasContext.G2G_Recode_PianoConcimazione_EntitaxTestata Where p.From_PivaSuperUser = G2G_Recodes_Request.FromPivaSuperUser And p.Data_Modifica > G2G_Recodes_Request.G2G_Recode_PianoConcimazione_EntitaxTestata_DataRiferimento).ToList()
        rval.G2GRecodePianoConcimazione_TestataToInsert = (From p In GiasContext.G2G_Recode_PianoConcimazione_Testata Where p.From_PivaSuperUser = G2G_Recodes_Request.FromPivaSuperUser And p.Data_Modifica > G2G_Recodes_Request.G2G_Recode_PianoConcimazione_Testata_DataRiferimento).ToList()
        rval.G2GRecodePraticheToInsert = (From p In GiasContext.G2G_Recode_Pratiche Where p.From_PivaSuperUser = G2G_Recodes_Request.FromPivaSuperUser And p.Data_Modifica > G2G_Recodes_Request.G2G_Recode_Pratiche_DataRiferimento).ToList()
        rval.G2GRecodePraticheStatiToInsert = (From p In GiasContext.G2G_Recode_Pratiche_Stati Where p.From_PivaSuperUser = G2G_Recodes_Request.FromPivaSuperUser And p.Data_Modifica > G2G_Recodes_Request.G2G_Recode_Pratiche_Stati_DataRiferimento).ToList()
        rval.G2GRecodeProgrammazioneEntitaToInsert = (From p In GiasContext.G2G_Recode_Programmazione_Entita Where p.From_PivaSuperUser = G2G_Recodes_Request.FromPivaSuperUser And p.Data_Modifica > G2G_Recodes_Request.G2G_Recode_Programmazione_Entita_DataRiferimento).ToList()
        rval.G2GRecodeProgrammazioneTestataToInsert = (From p In GiasContext.G2G_Recode_Programmazione_Testata Where p.From_PivaSuperUser = G2G_Recodes_Request.FromPivaSuperUser And p.Data_Modifica > G2G_Recodes_Request.G2G_Recode_Programmazione_Testata_DataRiferimento).ToList()
        rval.G2GRecodePuaToInsert = (From p In GiasContext.G2G_Recode_Pua Where p.From_PivaSuperUser = G2G_Recodes_Request.FromPivaSuperUser And p.Data_Modifica > G2G_Recodes_Request.G2G_Recode_Pua_DataRiferimento).ToList()
        rval.G2GRecodeRicetteToInsert = (From p In GiasContext.G2G_Recode_Ricette Where p.From_PivaSuperUser = G2G_Recodes_Request.FromPivaSuperUser And p.Data_Modifica > G2G_Recodes_Request.G2G_Recode_Ricette_DataRiferimento).ToList()
        rval.G2GRecodeRicette_OperazioniToInsert = (From p In GiasContext.G2G_Recode_Ricette_Operazioni Where p.From_PivaSuperUser = G2G_Recodes_Request.FromPivaSuperUser And p.Data_Modifica > G2G_Recodes_Request.G2G_Recode_Ricette_Operazioni_DataRiferimento).ToList()
        rval.G2GRecodeRicette_DettagliToInsert = (From p In GiasContext.G2G_Recode_Ricette_Dettagli Where p.From_PivaSuperUser = G2G_Recodes_Request.FromPivaSuperUser And p.Data_Modifica > G2G_Recodes_Request.G2G_Recode_Ricette_Dettagli_DataRiferimento).ToList()
        rval.G2GRecodeRicette_Dettaglio_TecnicoToInsert = (From p In GiasContext.G2G_Recode_Ricette_Dettaglio_Tecnico Where p.From_PivaSuperUser = G2G_Recodes_Request.FromPivaSuperUser And p.Data_Modifica > G2G_Recodes_Request.G2G_Recode_Ricette_Dettaglio_Tecnico_DataRiferimento).ToList()
        rval.G2GRecodeRicette_DestinazioniToInsert = (From p In GiasContext.G2G_Recode_Ricette_Destinazioni Where p.From_PivaSuperUser = G2G_Recodes_Request.FromPivaSuperUser And p.Data_Modifica > G2G_Recodes_Request.G2G_Recode_Ricette_Destinazioni_DataRiferimento).ToList()
        rval.G2GRecodePUA_LetamazioniPrecedentiToInsert = (From p In GiasContext.G2G_Recode_PUA_LetamazioniPrecedenti Where p.From_PivaSuperUser = G2G_Recodes_Request.FromPivaSuperUser And p.Data_Modifica > G2G_Recodes_Request.G2G_Recode_PUA_LetamazioniPrecedenti_DataRiferimento).ToList()
        rval.G2GRecodePua_EffluenteToInsert = (From p In GiasContext.G2G_Recode_PUA_Effluente Where p.From_PivaSuperUser = G2G_Recodes_Request.FromPivaSuperUser And p.Data_Modifica > G2G_Recodes_Request.G2G_Recode_PUA_Effluente_DataRiferimento).ToList()
        rval.G2GRecodePUA_Effluente_PeriodoDivietoToInsert = (From p In GiasContext.G2G_Recode_PUA_PeriodoDivieto Where p.From_PivaSuperUser = G2G_Recodes_Request.FromPivaSuperUser And p.Data_Modifica > G2G_Recodes_Request.G2G_Recode_PUA_PeriodoDivieto_DataRiferimento).ToList()
        rval.G2GRecodeAnagrafe_VincoliAgronomiciToInsert = (From p In GiasContext.G2G_Recode_Anagrafe_VincoliAgronomici Where p.From_PivaSuperUser = G2G_Recodes_Request.FromPivaSuperUser And p.Data_Modifica > G2G_Recodes_Request.G2G_Recode_Anagrafe_VincoliAgronomici_DataRiferimento).ToList()
        rval.G2GRecodeAttivitaToInsert = (From p In GiasContext.G2G_Recode_Attivita Where p.From_PivaSuperUser = G2G_Recodes_Request.FromPivaSuperUser And p.Data_Modifica > G2G_Recodes_Request.G2G_Recode_Attivita_DataRiferimento).ToList()

        Return rval

    End Function

    Public Function LeggiPerResponseGias2Gias_Reverse(
        ByVal G2G_Recodes_Request As G2G_Recodes_Request,
        ByRef objParametri As AgronicaCoreParametri
    ) As G2G_Recode

        Dim NomeRoutine As String = "G2G_Recodes.LeggiPerResponseGias2Gias_Reverse()"

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
        Dim GiasContext As New Gias_DeveloperServer_Entities(EFConnString)
        GiasContext.Database.CommandTimeout = 3600
        Dim rval As New G2G_Recode

        ' VAnni: 28/6/2019: leggo i soli oggetti in insert, sarà il client a stabilire cosa è UPDATE..
        rval.G2GRecodeAllegatiToInsert = (From p In GiasContext.G2G_Recode_Allegati Where p.From_PivaSuperUser = G2G_Recodes_Request.FromPivaSuperUser And p.Data_Modifica > G2G_Recodes_Request.G2G_Recode_Allegati_DataRiferimento).ToList()
        rval.G2GRecodeAllegati_EntitaToInsert = (From p In GiasContext.G2G_Recode_Allegati_Entita Where p.From_PivaSuperUser = G2G_Recodes_Request.FromPivaSuperUser And p.Data_Modifica > G2G_Recodes_Request.G2G_Recode_Allegati_Entita_DataRiferimento).ToList()
        rval.G2GRecodeAnalisi_CampioniToInsert = (From p In GiasContext.G2G_Recode_Analisi_Campioni Where p.From_PivaSuperUser = G2G_Recodes_Request.FromPivaSuperUser And p.Data_Modifica > G2G_Recodes_Request.G2G_Recode_Analisi_Campioni_DataRiferimento).ToList()
        rval.G2GRecodeAnalisi_CertificatoToInsert = (From p In GiasContext.G2G_Recode_Analisi_Certificato Where p.From_PivaSuperUser = G2G_Recodes_Request.FromPivaSuperUser And p.Data_Modifica > G2G_Recodes_Request.G2G_Recode_Analisi_Certificato_DataRiferimento).ToList()
        rval.G2GRecodeAnalisi_DettagliToInsert = (From p In GiasContext.G2G_Recode_Analisi_Dettagli Where p.From_PivaSuperUser = G2G_Recodes_Request.FromPivaSuperUser And p.Data_Modifica > G2G_Recodes_Request.G2G_Recode_Analisi_Dettagli_DataRiferimento).ToList()
        rval.G2GRecodeAnalisi_EntitaxTestataToInsert = (From p In GiasContext.G2G_Recode_Analisi_EntitaxTestata Where p.From_PivaSuperUser = G2G_Recodes_Request.FromPivaSuperUser And p.Data_Modifica > G2G_Recodes_Request.G2G_Recode_Analisi_EntitaxTestata_DataRiferimento).ToList()
        rval.G2GRecodeAnalisi_TestataToInsert = (From p In GiasContext.G2G_Recode_Analisi_Testata Where p.From_PivaSuperUser = G2G_Recodes_Request.FromPivaSuperUser And p.Data_Modifica > G2G_Recodes_Request.G2G_Recode_Analisi_Testata_DataRiferimento).ToList()
        rval.G2GRecodeCampiToInsert = (From p In GiasContext.G2G_Recode_Campo Where p.From_PivaSuperUser = G2G_Recodes_Request.FromPivaSuperUser And p.Data_Modifica > G2G_Recodes_Request.G2G_Recode_Campi_DataRiferimento).ToList()
        rval.G2GRecodeAppezzamentiToInsert = (From p In GiasContext.G2G_Recode_Appezzamenti Where p.From_PivaSuperUser = G2G_Recodes_Request.FromPivaSuperUser And p.Data_Modifica > G2G_Recodes_Request.G2G_Recode_Appezzamenti_DataRiferimento).ToList()
        rval.G2GRecodeImpiantiToInsert = (From p In GiasContext.G2G_Recode_Impianti Where p.From_PivaSuperUser = G2G_Recodes_Request.FromPivaSuperUser And p.Data_Modifica > G2G_Recodes_Request.G2G_Recode_Impianti_DataRiferimento).ToList()
        rval.G2GRecodeDistinteToInsert = (From p In GiasContext.G2G_Recode_Distinta Where p.From_PivaSuperUser = G2G_Recodes_Request.FromPivaSuperUser And p.Data_Modifica > G2G_Recodes_Request.G2G_Recode_Distinte_DataRiferimento).ToList()
        rval.G2GRecodeContattiToInsert = (From p In GiasContext.G2G_Recode_Contatti Where p.From_PivaSuperUser = G2G_Recodes_Request.FromPivaSuperUser And p.Data_Modifica > G2G_Recodes_Request.G2G_Recode_Contatti_DataRiferimento).ToList()
        rval.G2GRecodeFabbricatiToInsert = (From p In GiasContext.G2G_Recode_Fabbricati Where p.From_PivaSuperUser = G2G_Recodes_Request.FromPivaSuperUser And p.Data_Modifica > G2G_Recodes_Request.G2G_Recode_Fabbricati_DataRiferimento).ToList()
        rval.G2GRecodeImpreseCentriToInsert = (From p In GiasContext.G2G_Recode_Imprese Where p.From_PivaSuperUser = G2G_Recodes_Request.FromPivaSuperUser And p.Data_Modifica > G2G_Recodes_Request.G2G_Recode_ImpreseCentri_DataRiferimento).ToList()
        rval.G2GRecodeIndirizziToInsert = (From p In GiasContext.G2G_Recode_Indirizzi Where p.From_PivaSuperUser = G2G_Recodes_Request.FromPivaSuperUser And p.Data_Modifica > G2G_Recodes_Request.G2G_Recode_Indirizzi_DataRiferimento).ToList()
        rval.G2GRecodeMateriePrimeCampionatureToInsert = (From p In GiasContext.G2G_Recode_Materie_Prime_Campionature Where p.From_PivaSuperUser = G2G_Recodes_Request.FromPivaSuperUser And p.Data_Modifica > G2G_Recodes_Request.G2G_Recode_MateriePrimeCampionature_DataRiferimento).ToList()
        rval.G2GRecodeMateriePrimeToInsert = (From p In GiasContext.G2G_Recode_MateriePrime Where p.From_PivaSuperUser = G2G_Recodes_Request.FromPivaSuperUser And p.Data_Modifica > G2G_Recodes_Request.G2G_Recode_MateriePrime_DataRiferimento).ToList()
        rval.G2GRecodeParcoMacchineToInsert = (From p In GiasContext.G2G_Recode_Parco_Macchine Where p.From_PivaSuperUser = G2G_Recodes_Request.FromPivaSuperUser And p.Data_Modifica > G2G_Recodes_Request.G2G_Recode_Macchine_DataRiferimento).ToList()
        rval.G2GRecodeParticelleCatastalixVincoliAgronomiciToInsert = (From p In GiasContext.G2G_Recode_ParticelleCatastalixVincoliAgronomici Where p.From_PivaSuperUser = G2G_Recodes_Request.FromPivaSuperUser And p.Data_Modifica > G2G_Recodes_Request.G2G_Recode_ParticelleCatastalixVincoliAgronomici_DataRiferimento).ToList()
        rval.G2GRecodePianoConcimazione_DettagliToInsert = (From p In GiasContext.G2G_Recode_PianoConcimazione_Dettagli Where p.From_PivaSuperUser = G2G_Recodes_Request.FromPivaSuperUser And p.Data_Modifica > G2G_Recodes_Request.G2G_Recode_PianoConcimazione_Dettagli_DataRiferimento).ToList()
        rval.G2GRecodePianoConcimazione_ElaborazioniToInsert = (From p In GiasContext.G2G_Recode_PianoConcimazione_Elaborazioni Where p.From_PivaSuperUser = G2G_Recodes_Request.FromPivaSuperUser And p.Data_Modifica > G2G_Recodes_Request.G2G_Recode_PianoConcimazione_Elaborazioni_DataRiferimento).ToList()
        rval.G2GRecodePianoConcimazione_EntitaxTestataToInsert = (From p In GiasContext.G2G_Recode_PianoConcimazione_EntitaxTestata Where p.From_PivaSuperUser = G2G_Recodes_Request.FromPivaSuperUser And p.Data_Modifica > G2G_Recodes_Request.G2G_Recode_PianoConcimazione_EntitaxTestata_DataRiferimento).ToList()
        rval.G2GRecodePianoConcimazione_TestataToInsert = (From p In GiasContext.G2G_Recode_PianoConcimazione_Testata Where p.From_PivaSuperUser = G2G_Recodes_Request.FromPivaSuperUser And p.Data_Modifica > G2G_Recodes_Request.G2G_Recode_PianoConcimazione_Testata_DataRiferimento).ToList()
        rval.G2GRecodePraticheToInsert = (From p In GiasContext.G2G_Recode_Pratiche Where p.From_PivaSuperUser = G2G_Recodes_Request.FromPivaSuperUser And p.Data_Modifica > G2G_Recodes_Request.G2G_Recode_Pratiche_DataRiferimento).ToList()
        rval.G2GRecodePraticheStatiToInsert = (From p In GiasContext.G2G_Recode_Pratiche_Stati Where p.From_PivaSuperUser = G2G_Recodes_Request.FromPivaSuperUser And p.Data_Modifica > G2G_Recodes_Request.G2G_Recode_Pratiche_Stati_DataRiferimento).ToList()
        rval.G2GRecodeProgrammazioneEntitaToInsert = (From p In GiasContext.G2G_Recode_Programmazione_Entita Where p.From_PivaSuperUser = G2G_Recodes_Request.FromPivaSuperUser And p.Data_Modifica > G2G_Recodes_Request.G2G_Recode_Programmazione_Entita_DataRiferimento).ToList()
        rval.G2GRecodeProgrammazioneTestataToInsert = (From p In GiasContext.G2G_Recode_Programmazione_Testata Where p.From_PivaSuperUser = G2G_Recodes_Request.FromPivaSuperUser And p.Data_Modifica > G2G_Recodes_Request.G2G_Recode_Programmazione_Testata_DataRiferimento).ToList()
        rval.G2GRecodePuaToInsert = (From p In GiasContext.G2G_Recode_Pua Where p.From_PivaSuperUser = G2G_Recodes_Request.FromPivaSuperUser And p.Data_Modifica > G2G_Recodes_Request.G2G_Recode_Pua_DataRiferimento).ToList()
        rval.G2GRecodeRicetteToInsert = (From p In GiasContext.G2G_Recode_Ricette Where p.From_PivaSuperUser = G2G_Recodes_Request.FromPivaSuperUser And p.Data_Modifica > G2G_Recodes_Request.G2G_Recode_Ricette_DataRiferimento).ToList()
        rval.G2GRecodeRicette_OperazioniToInsert = (From p In GiasContext.G2G_Recode_Ricette_Operazioni Where p.From_PivaSuperUser = G2G_Recodes_Request.FromPivaSuperUser And p.Data_Modifica > G2G_Recodes_Request.G2G_Recode_Ricette_Operazioni_DataRiferimento).ToList()
        rval.G2GRecodeRicette_DettagliToInsert = (From p In GiasContext.G2G_Recode_Ricette_Dettagli Where p.From_PivaSuperUser = G2G_Recodes_Request.FromPivaSuperUser And p.Data_Modifica > G2G_Recodes_Request.G2G_Recode_Ricette_Dettagli_DataRiferimento).ToList()
        rval.G2GRecodeRicette_Dettaglio_TecnicoToInsert = (From p In GiasContext.G2G_Recode_Ricette_Dettaglio_Tecnico Where p.From_PivaSuperUser = G2G_Recodes_Request.FromPivaSuperUser And p.Data_Modifica > G2G_Recodes_Request.G2G_Recode_Ricette_Dettaglio_Tecnico_DataRiferimento).ToList()
        rval.G2GRecodeRicette_DestinazioniToInsert = (From p In GiasContext.G2G_Recode_Ricette_Destinazioni Where p.From_PivaSuperUser = G2G_Recodes_Request.FromPivaSuperUser And p.Data_Modifica > G2G_Recodes_Request.G2G_Recode_Ricette_Destinazioni_DataRiferimento).ToList()
        rval.G2GRecodePUA_LetamazioniPrecedentiToInsert = (From p In GiasContext.G2G_Recode_PUA_LetamazioniPrecedenti Where p.From_PivaSuperUser = G2G_Recodes_Request.FromPivaSuperUser And p.Data_Modifica > G2G_Recodes_Request.G2G_Recode_PUA_LetamazioniPrecedenti_DataRiferimento).ToList()
        rval.G2GRecodePua_EffluenteToInsert = (From p In GiasContext.G2G_Recode_PUA_Effluente Where p.From_PivaSuperUser = G2G_Recodes_Request.FromPivaSuperUser And p.Data_Modifica > G2G_Recodes_Request.G2G_Recode_PUA_Effluente_DataRiferimento).ToList()
        rval.G2GRecodeAnagrafe_VincoliAgronomiciToInsert = (From p In GiasContext.G2G_Recode_Anagrafe_VincoliAgronomici Where p.From_PivaSuperUser = G2G_Recodes_Request.FromPivaSuperUser And p.Data_Modifica > G2G_Recodes_Request.G2G_Recode_Anagrafe_VincoliAgronomici_DataRiferimento).ToList()
        rval.G2GRecodeAttivitaToInsert = (From p In GiasContext.G2G_Recode_Attivita Where p.From_PivaSuperUser = G2G_Recodes_Request.FromPivaSuperUser And p.Data_Modifica > G2G_Recodes_Request.G2G_Recode_Attivita_DataRiferimento).ToList()

        Return rval

    End Function

End Class


