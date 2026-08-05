Imports System.Transactions

Imports Newtonsoft.Json

Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreModello
Imports Newtonsoft.Json.Linq
Imports System.Text
Imports System.Data.Entity

Public Class G2GRicette_R

    Public Function LeggiPerGias2GS1(
        ByVal objOpzioniImportImpresa As clsImpresa,
        ByVal piva As String,
        ByRef objParametri As AgronicaCoreParametri
    ) As G2G_Ricette

        Dim NomeRoutine As String = "G2GlocalDal.G2GRicette_R.LeggiPerGias2GS1()"

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
        Dim GiasContext As New Gias_DeveloperServer_Entities(EFConnString)
        GiasContext.Database.CommandTimeout = 3600
        Dim oConfigurazione As G2G_Configurazione_FiltriReq_Ricette
        If objOpzioniImportImpresa.configurazione_ricette <> "" Then
            oConfigurazione = JsonConvert.DeserializeObject(Of G2G_Configurazione_FiltriReq_Ricette)(objOpzioniImportImpresa.configurazione_ricette)
        Else
            oConfigurazione = New G2G_Configurazione_FiltriReq_Ricette
            oConfigurazione.listaRicette_Tipo = New List(Of Integer)
        End If

        Dim rval As New G2G_Ricette

        rval.ricette_operazioni_insert = (
            From r In GiasContext.Ricette
            Join o In GiasContext.Ricette_Operazioni On r.Ricetta_Cod Equals o.Ricetta_Cod
            Where r.Piva = piva AndAlso r.Validita_Fine >= objOpzioniImportImpresa.ValiditaInizio_ricette AndAlso (oConfigurazione.listaRicette_Tipo.Count = 0 Or oConfigurazione.listaRicette_Tipo.Contains(r.Tipo_Ricetta)) AndAlso
                Not GiasContext.G2G_Recode_Ricette_Operazioni.Any(Function(g) g.From_PivaSuperUser = r.Ricetta_SuperUser And g.From_Ricetta_Cod = o.Ricetta_Cod And g.From_Ricetta_Operazione_Cod = o.Ricetta_Operazione_Cod)
            Select o).Distinct.ToList()

        rval.ricette_operazioni_update = (
            From o In GiasContext.Ricette_Operazioni
            Join r In GiasContext.Ricette On o.Ricetta_Cod Equals r.Ricetta_Cod
            Join g2g In GiasContext.G2G_Recode_Ricette_Operazioni On o.Ricetta_Cod Equals g2g.From_Ricetta_Cod And o.Ricetta_Operazione_Cod Equals g2g.From_Ricetta_Operazione_Cod And r.Piva Equals g2g.From_Piva And o.Ricetta_SuperUser Equals g2g.From_PivaSuperUser
            Where r.Piva = piva AndAlso g2g.datainvio < o.Data_Modifica
            Select o).Distinct.ToList()

        rval.G2G_Ricette_Operazioni_Recode_delete = (
            From r In GiasContext.G2G_Recode_Ricette_Operazioni
            Where Not GiasContext.Ricette_Operazioni.Any(Function(p) p.Ricetta_Cod = r.From_Ricetta_Cod And p.Ricetta_Operazione_Cod = r.From_Ricetta_Operazione_Cod And p.Ricetta_SuperUser = r.From_PivaSuperUser) And r.From_Piva = piva
            Select r).Distinct.ToList()

        Return rval

    End Function

    Public Function LeggiDettagliPerGias2GS1(ByVal ricetta As Ricette_Operazioni, ByRef objParametri As AgronicaCoreParametri) As List(Of Ricette_Dettagli)

        Dim NomeRoutine As String = "G2GlocalDal.G2GRicette_R.LeggiDettagliPerGias2GS1()"

        Dim listaDettagli As New List(Of Ricette_Dettagli)

        If ricetta.W_Anagrafica_Stati_Cod = enum_WWorflow_WAnagraficaStati.Esecuzione_ed_avanzamento_delle_ricette_Eseguita Then

            Dim gefutils As New Gias_EF_Utility
            Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
            Dim GiasContext As New Gias_DeveloperServer_Entities(EFConnString)
            GiasContext.Database.CommandTimeout = 3600
            Dim causaliMagazzino As New List(Of String) From {CAU_SCARICO, CAU_CARICO}
            Dim listaCausali As New List(Of String) From {CAU_TRATTAMENTO, CAU_RILIEVO_CAMPO, CAU_RILIEVO_RACCOLTA, CAU_LAVORAZIONE}

            Dim dettagli = (From d In GiasContext.Ricette_Dettagli Where d.Ricetta_Cod = ricetta.Ricetta_Cod And d.Ricetta_Operazione_Cod = ricetta.Ricetta_Operazione_Cod Select d).ToList

            For Each dettaglio In dettagli

                If causaliMagazzino.Contains(dettaglio.Cau_Mov) Then
                    Return Nothing
                ElseIf listaCausali.Contains(dettaglio.Cau_Mov) Then
                    If dettaglio.Elem_Cod <> 0 AndAlso dettaglio.Elem_Cod <> 1 Then
                        If dettaglio.Pro_Cod <> 0 OrElse dettaglio.Mat_Cod <> 0 Then
                            listaDettagli.Add(dettaglio)
                        End If
                    End If
                End If

            Next

        End If

        Return listaDettagli

    End Function

    Public Function LeggiPerGias2Gias(
        ByVal objOpzioniImportImpresa As clsImpresa,
        ByVal piva As String,
        ByVal piva_destinazione As String,
        ByVal PivaSuperUser_Destinazione As String,
        ByRef objParametri As AgronicaCoreParametri
    ) As G2G_Ricette


        Dim NomeRoutine As String = "G2GlocalDal.G2GRicette_R.LeggiPerGias2Gias()"



        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
        Dim GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

        Dim oConfigurazione As G2G_Configurazione_FiltriReq_Ricette
        If objOpzioniImportImpresa.configurazione_ricette <> "" Then
            oConfigurazione = JsonConvert.DeserializeObject(Of G2G_Configurazione_FiltriReq_Ricette)(objOpzioniImportImpresa.configurazione_ricette)
        Else
            oConfigurazione = New G2G_Configurazione_FiltriReq_Ricette
            oConfigurazione.listaRicette_Tipo = New List(Of Integer)
        End If


        Dim rval As New G2G_Ricette

        'quanto sicuramente non assegnato ...
        With rval
            .From_Piva = piva
            .From_PivaSuperUser = objParametri.PivaSuperUser
            .G2G_Ricette_Recode_insert = New List(Of G2G_Recode_Ricette)
            .G2G_Ricette_Recode_update = New List(Of G2G_Recode_Ricette)
            .G2G_Ricette_Operazioni_Recode_insert = New List(Of G2G_Recode_Ricette_Operazioni)
            .G2G_Ricette_Operazioni_Recode_update = New List(Of G2G_Recode_Ricette_Operazioni)
            .G2G_Ricette_Dettagli_Recode_insert = New List(Of G2G_Recode_Ricette_Dettagli)
            .G2G_Ricette_Dettagli_Recode_update = New List(Of G2G_Recode_Ricette_Dettagli)
            .G2G_Ricette_Dettaglio_Tecnico_Recode_insert = New List(Of G2G_Recode_Ricette_Dettaglio_Tecnico)
            .G2G_Ricette_Dettaglio_Tecnico_Recode_update = New List(Of G2G_Recode_Ricette_Dettaglio_Tecnico)
            .G2G_Ricette_Destinazioni_Recode_insert = New List(Of G2G_Recode_Ricette_Destinazioni)
            .G2G_Ricette_Destinazioni_Recode_update = New List(Of G2G_Recode_Ricette_Destinazioni)
            .ricette_destinazioni_delete = New List(Of Ricette_Destinazioni)
            .ricette_dettaglio_tecnico_delete = New List(Of Ricette_Dettaglio_Tecnico)
            .ricette_dettagli_delete = New List(Of Ricette_Dettagli)
            .ricette_operazioni_delete = New List(Of Ricette_Operazioni)
            .ricette_delete = New List(Of Ricette)
            .To_Piva = piva_destinazione
        End With
        GiasContext.Database.CommandTimeout = 3600
        Dim ricette_cod_update As New List(Of Integer)
        Dim ricette_operazioni_cod_update As New List(Of Integer)

        'Elaborazioni
        rval.ricette_insert = (
            From r In GiasContext.Ricette
            Where r.Piva = piva _
                AndAlso r.Validita_Fine >= objOpzioniImportImpresa.ValiditaInizio_ricette _
                AndAlso (oConfigurazione.listaRicette_Tipo.Count = 0 Or oConfigurazione.listaRicette_Tipo.Contains(r.Tipo_Ricetta)) _
                AndAlso Not GiasContext.G2G_Recode_Ricette.Any(Function(g) g.From_PivaSuperUser = r.Ricetta_SuperUser _
                                                                            And g.To_PivaSuperUser = PivaSuperUser_Destinazione _
                                                                            And g.From_Ricetta_Cod = r.Ricetta_Cod)
            Select r).Distinct.ToList()

        rval.ricette_update = (From r In GiasContext.Ricette
                               Join g2g In GiasContext.G2G_Recode_Ricette
                                   On r.Ricetta_Cod Equals g2g.From_Ricetta_Cod _
                                   And r.Ricetta_SuperUser Equals g2g.From_PivaSuperUser _
                                   And r.Piva Equals g2g.From_Piva
                               Where r.Piva = piva _
                                   AndAlso g2g.To_PivaSuperUser = PivaSuperUser_Destinazione _
                                   AndAlso System.Data.Entity.DbFunctions.AddDays(g2g.Data_Modifica, -1) < r.Data_Modifica
                               Select r).Distinct.ToList()

        For Each ricetta_update In rval.ricette_update
            ricette_cod_update.Add(ricetta_update.Ricetta_Cod)
        Next

        rval.G2G_Ricette_Recode_delete = (
            From r In GiasContext.G2G_Recode_Ricette
            Where Not GiasContext.Ricette.Any(Function(p) p.Ricetta_Cod = r.From_Ricetta_Cod And p.Ricetta_SuperUser = r.From_PivaSuperUser) _
                And r.From_Piva = piva _
                AndAlso r.To_PivaSuperUser = PivaSuperUser_Destinazione
            Select r).Distinct.ToList()

        'Testate
        rval.ricette_operazioni_insert = (
            From r In GiasContext.Ricette
            Join o In GiasContext.Ricette_Operazioni On r.Ricetta_Cod Equals o.Ricetta_Cod
            Where r.Piva = piva _
                AndAlso r.Validita_Fine >= objOpzioniImportImpresa.ValiditaInizio_ricette _
                AndAlso (oConfigurazione.listaRicette_Tipo.Count = 0 Or oConfigurazione.listaRicette_Tipo.Contains(r.Tipo_Ricetta)) _
                AndAlso Not GiasContext.G2G_Recode_Ricette_Operazioni.Any(Function(g) g.From_PivaSuperUser = r.Ricetta_SuperUser _
                                                                            And g.To_PivaSuperUser = PivaSuperUser_Destinazione _
                                                                            And g.From_Ricetta_Operazione_Cod = o.Ricetta_Operazione_Cod)
            Select o).Distinct.ToList()
        'And g.From_Ricetta_Cod = o.Ricetta_Cod _

        rval.ricette_operazioni_update = (
        From o In GiasContext.Ricette_Operazioni
        Join r In GiasContext.Ricette On o.Ricetta_Cod Equals r.Ricetta_Cod
        Join g2g In GiasContext.G2G_Recode_Ricette_Operazioni
                On o.Ricetta_Cod Equals g2g.From_Ricetta_Cod _
                And o.Ricetta_Operazione_Cod Equals g2g.From_Ricetta_Operazione_Cod _
                And r.Piva Equals g2g.From_Piva _
                And o.Ricetta_SuperUser Equals g2g.From_PivaSuperUser
        Where r.Piva = piva _
                AndAlso g2g.To_PivaSuperUser = PivaSuperUser_Destinazione _
                AndAlso System.Data.Entity.DbFunctions.AddDays(g2g.Data_Modifica, -1) < o.Data_Modifica
        Select o
            ).Distinct.ToList()

        For Each ricetta_operazione_update In rval.ricette_operazioni_update
            ricette_operazioni_cod_update.Add(ricetta_operazione_update.Ricetta_Operazione_Cod)
        Next

        rval.G2G_Ricette_Operazioni_Recode_delete = (
            From r In GiasContext.G2G_Recode_Ricette_Operazioni
            Where Not GiasContext.Ricette_Operazioni.Any(Function(p) p.Ricetta_Cod = r.From_Ricetta_Cod _
                                                            And p.Ricetta_Operazione_Cod = r.From_Ricetta_Operazione_Cod _
                                                            And p.Ricetta_SuperUser = r.From_PivaSuperUser) _
                And r.From_Piva = piva _
                AndAlso r.To_PivaSuperUser = PivaSuperUser_Destinazione
            Select r).Distinct.ToList()
        'rval.G2G_PianoConcimazione_Testata_Recode_delete = (
        '    From r In GiasContext.G2G_Recode_PianoConcimazione_Testata
        '    Join e In GiasContext.G2G_Recode_PianoConcimazione_EntitaxTestata
        '        On r.From_PC_Testata_Cod Equals e.From_PC_Testata_Cod _
        '        And r.From_PivaSuperUser Equals e.From_PivaSuperUser
        '    Where Not GiasContext.PianoConcimazione_Testata.Any(Function(p) p.PC_Testata_Cod = r.From_PC_Testata_Cod And p.PC_SuperUser = r.From_PivaSuperUser) _
        '        And e.From_Piva = piva
        '    Select r).Distinct.ToList()

        'Dettagli
        rval.ricette_dettagli_insert = (
            From r In GiasContext.Ricette
            Join o In GiasContext.Ricette_Operazioni On r.Ricetta_Cod Equals o.Ricetta_Cod
            Join d In GiasContext.Ricette_Dettagli On o.Ricetta_Cod Equals d.Ricetta_Cod And o.Ricetta_Operazione_Cod Equals d.Ricetta_Operazione_Cod
            Where r.Piva = piva _
                AndAlso r.Validita_Fine >= objOpzioniImportImpresa.ValiditaInizio_ricette _
                AndAlso (oConfigurazione.listaRicette_Tipo.Count = 0 Or oConfigurazione.listaRicette_Tipo.Contains(r.Tipo_Ricetta)) _
                AndAlso Not GiasContext.G2G_Recode_Ricette_Dettagli.Any(Function(g) g.From_PivaSuperUser = d.Ricetta_SuperUser _
                                                                                    And g.To_PivaSuperUser = PivaSuperUser_Destinazione _
                                                                                    And g.From_Ricetta_Dettaglio_Cod = d.Ricetta_Dettaglio_Cod)
            Select d).Distinct.ToList()

        'And g.From_Ricetta_Cod = d.Ricetta_Cod _
        'And g.From_Ricetta_Operazione_Cod = d.Ricetta_Operazione_Cod _

        rval.ricette_dettagli_update = (From o In GiasContext.Ricette_Dettagli
                                        Join r In GiasContext.Ricette On o.Ricetta_Cod Equals r.Ricetta_Cod
                                        Join g2g In GiasContext.G2G_Recode_Ricette_Dettagli
                                            On o.Ricetta_Cod Equals g2g.From_Ricetta_Cod _
                                            And o.Ricetta_Operazione_Cod Equals g2g.From_Ricetta_Operazione_Cod _
                                            And o.Ricetta_Dettaglio_Cod Equals g2g.From_Ricetta_Dettaglio_Cod _
                                            And r.Piva Equals g2g.From_Piva _
                                            And o.Ricetta_SuperUser Equals g2g.From_PivaSuperUser
                                        Where r.Piva = piva _
                                            AndAlso g2g.To_PivaSuperUser = PivaSuperUser_Destinazione _
                                            AndAlso System.Data.Entity.DbFunctions.AddDays(g2g.Data_Modifica, -1) < o.Data_Modifica
                                        Select o
                                            ).Distinct.ToList

        rval.G2G_Ricette_Dettagli_Recode_delete = (
            From r In GiasContext.G2G_Recode_Ricette_Dettagli
            Where Not GiasContext.Ricette_Dettagli.Any(Function(p) p.Ricetta_Cod = r.From_Ricetta_Cod _
                                                            And p.Ricetta_Operazione_Cod = r.From_Ricetta_Operazione_Cod _
                                                            And p.Ricetta_Dettaglio_Cod = r.From_Ricetta_Dettaglio_Cod _
                                                            And p.Ricetta_SuperUser = r.From_PivaSuperUser) _
                And r.From_Piva = piva _
                AndAlso r.To_PivaSuperUser = PivaSuperUser_Destinazione
            Select r).Distinct.ToList()

        'Ricette Dettaglio Tecnico
        rval.ricette_dettaglio_tecnico_insert = (
            From r In GiasContext.Ricette
            Join o In GiasContext.Ricette_Operazioni On r.Ricetta_Cod Equals o.Ricetta_Cod
            Join d In GiasContext.Ricette_Dettagli On o.Ricetta_Cod Equals d.Ricetta_Cod And o.Ricetta_Operazione_Cod Equals d.Ricetta_Operazione_Cod
            Join dt In GiasContext.Ricette_Dettaglio_Tecnico On d.Ricetta_Cod Equals dt.Ricetta_Cod And d.Ricetta_Operazione_Cod Equals dt.Ricetta_Operazione_Cod And d.Ricetta_Dettaglio_Cod Equals dt.Ricetta_Dettaglio_Cod
            Where r.Piva = piva _
                AndAlso r.Validita_Fine >= objOpzioniImportImpresa.ValiditaInizio_ricette _
                AndAlso (oConfigurazione.listaRicette_Tipo.Count = 0 Or oConfigurazione.listaRicette_Tipo.Contains(r.Tipo_Ricetta)) _
                AndAlso Not GiasContext.G2G_Recode_Ricette_Dettaglio_Tecnico.Any(Function(g) g.From_PivaSuperUser = dt.Ricetta_SuperUser _
                                                                                     And g.To_PivaSuperUser = PivaSuperUser_Destinazione _
                                                                                     And g.From_Ricetta_Tecnico_Cod = dt.Ricetta_Tecnico_Cod)
            Select dt).Distinct.ToList()

        'And g.From_Ricetta_Cod = dt.Ricetta_Cod _
        'And g.From_Ricetta_Operazione_Cod = dt.Ricetta_Operazione_Cod _
        'And g.From_Ricetta_Dettaglio_Cod = dt.Ricetta_Dettaglio_Cod _

        rval.ricette_dettaglio_tecnico_update = (From o In GiasContext.Ricette_Dettaglio_Tecnico
                                                 Join r In GiasContext.Ricette On o.Ricetta_Cod Equals r.Ricetta_Cod
                                                 Join g2g In GiasContext.G2G_Recode_Ricette_Dettaglio_Tecnico
                                                    On o.Ricetta_Cod Equals g2g.From_Ricetta_Cod _
                                                    And o.Ricetta_Operazione_Cod Equals g2g.From_Ricetta_Operazione_Cod _
                                                    And o.Ricetta_Dettaglio_Cod Equals g2g.From_Ricetta_Dettaglio_Cod _
                                                     And o.Ricetta_Tecnico_Cod Equals g2g.From_Ricetta_Tecnico_Cod _
                                                    And r.Piva Equals g2g.From_Piva _
                                                    And o.Ricetta_SuperUser Equals g2g.From_PivaSuperUser
                                                 Where r.Piva = piva _
                                                    AndAlso g2g.To_PivaSuperUser = PivaSuperUser_Destinazione _
                                                    AndAlso System.Data.Entity.DbFunctions.AddDays(g2g.Data_Modifica, -1) < o.Data_Modifica
                                                 Select o
                                            ).Distinct.ToList()

        rval.G2G_Ricette_Dettaglio_Tecnico_Recode_delete = (
            From r In GiasContext.G2G_Recode_Ricette_Dettaglio_Tecnico
            Where Not GiasContext.Ricette_Dettaglio_Tecnico.Any(Function(p) p.Ricetta_Cod = r.From_Ricetta_Cod _
                                                            And p.Ricetta_Operazione_Cod = r.From_Ricetta_Operazione_Cod _
                                                            And p.Ricetta_Dettaglio_Cod = r.From_Ricetta_Dettaglio_Cod _
                                                            And p.Ricetta_Tecnico_Cod = r.From_Ricetta_Tecnico_Cod _
                                                            And p.Ricetta_SuperUser = r.From_PivaSuperUser) _
                And r.From_Piva = piva _
                AndAlso r.To_PivaSuperUser = PivaSuperUser_Destinazione
            Select r).Distinct.ToList()

        'Ricette Destinazioni
        rval.ricette_destinazioni_insert = (
            From r In GiasContext.Ricette
            Join o In GiasContext.Ricette_Operazioni On r.Ricetta_Cod Equals o.Ricetta_Cod
            Join d In GiasContext.Ricette_Dettagli On o.Ricetta_Cod Equals d.Ricetta_Cod And o.Ricetta_Operazione_Cod Equals d.Ricetta_Operazione_Cod
            Join rd In GiasContext.Ricette_Destinazioni On d.Ricetta_Cod Equals rd.Ricetta_Cod And d.Ricetta_Operazione_Cod Equals rd.Ricetta_Operazione_Cod And d.Ricetta_Dettaglio_Cod Equals rd.Ricetta_Dettaglio_Cod
            Group Join g In GiasContext.G2G_Recode_Ricette_Destinazioni.Where(Function(x) x.To_PivaSuperUser = PivaSuperUser_Destinazione) On
                                                                        g.From_PivaSuperUser Equals rd.Ricetta_SuperUser _
                                                                        And g.From_Ricetta_Cod Equals rd.Ricetta_Cod _
                                                                        And g.From_Ricetta_Operazione_Cod Equals rd.Ricetta_Operazione_Cod _
                                                                        And g.From_Ricetta_Dettaglio_Cod Equals rd.Ricetta_Dettaglio_Cod _
                                                                        And g.From_Ricetta_Destinazione_Cod Equals rd.Ricetta_Destinazione_Cod
                    Into g_group = Group
            From _g_group In g_group.DefaultIfEmpty()
            Where r.Piva = piva _
                AndAlso r.Validita_Fine >= objOpzioniImportImpresa.ValiditaInizio_ricette _
                AndAlso (oConfigurazione.listaRicette_Tipo.Count = 0 Or oConfigurazione.listaRicette_Tipo.Contains(r.Tipo_Ricetta)) _
                AndAlso _g_group Is Nothing
            Select rd).Distinct.ToList()

        'AndAlso Not GiasContext.G2G_Recode_Ricette_Destinazioni.Any(Function(g) g.From_PivaSuperUser = rd.Ricetta_SuperUser _
        '                                                                            And g.From_Ricetta_Cod = rd.Ricetta_Cod _
        '                                                                            And g.From_Ricetta_Operazione_Cod = rd.Ricetta_Operazione_Cod _
        '                                                                            And g.From_Ricetta_Dettaglio_Cod = rd.Ricetta_Dettaglio_Cod _
        '                                                                            And g.From_Ricetta_Destinazione_Cod = rd.Ricetta_Destinazione_Cod _
        '                                                                            And g.To_PivaSuperUser = PivaSuperUser_Destinazione)


        rval.ricette_destinazioni_update = (From o In GiasContext.Ricette_Destinazioni
                                            Join r In GiasContext.Ricette On o.Ricetta_Cod Equals r.Ricetta_Cod
                                            Join g2g In GiasContext.G2G_Recode_Ricette_Destinazioni
                                                    On o.Ricetta_Cod Equals g2g.From_Ricetta_Cod _
                                                    And o.Ricetta_Operazione_Cod Equals g2g.From_Ricetta_Operazione_Cod _
                                                    And o.Ricetta_Dettaglio_Cod Equals g2g.From_Ricetta_Dettaglio_Cod _
                                                     And o.Ricetta_Destinazione_Cod Equals g2g.From_Ricetta_Destinazione_Cod _
                                                    And r.Piva Equals g2g.From_Piva _
                                                    And o.Ricetta_SuperUser Equals g2g.From_PivaSuperUser
                                            Where r.Piva = piva _
                                                    AndAlso g2g.To_PivaSuperUser = PivaSuperUser_Destinazione _
                                                    AndAlso System.Data.Entity.DbFunctions.AddDays(g2g.Data_Modifica, -1) < o.Data_Modifica
                                            Select o
                                            ).Distinct.ToList()


        rval.G2G_Ricette_Destinazioni_Recode_delete = (
            From r In GiasContext.G2G_Recode_Ricette_Destinazioni
            Where Not GiasContext.Ricette_Destinazioni.Any(Function(p) p.Ricetta_Cod = r.From_Ricetta_Cod _
                                                            And p.Ricetta_Operazione_Cod = r.From_Ricetta_Operazione_Cod _
                                                            And p.Ricetta_Dettaglio_Cod = r.From_Ricetta_Dettaglio_Cod _
                                                            And p.Ricetta_Destinazione_Cod = r.From_Ricetta_Destinazione_Cod _
                                                            And p.Ricetta_SuperUser = r.From_PivaSuperUser) _
                And r.From_Piva = piva _
                AndAlso r.To_PivaSuperUser = PivaSuperUser_Destinazione
            Select r).Distinct.ToList()

        'Ricette x Agenda
        rval.ricettexagenda_insert = (
            From r In GiasContext.Ricette
            Join o In GiasContext.Ricette_Operazioni On r.Ricetta_Cod Equals o.Ricetta_Cod
            Join a In GiasContext.RicettexAgenda On o.Ricetta_Cod Equals a.Ricetta_Cod And o.Ricetta_Operazione_Cod Equals a.Ricetta_Operazione_Cod
            Where r.Piva = piva _
                AndAlso r.Validita_Fine >= objOpzioniImportImpresa.ValiditaInizio_ricette _
                AndAlso (oConfigurazione.listaRicette_Tipo.Count = 0 Or oConfigurazione.listaRicette_Tipo.Contains(r.Tipo_Ricetta)) _
                AndAlso Not GiasContext.G2G_Recode_Ricette_Operazioni.Any(Function(g) g.From_PivaSuperUser = r.Ricetta_SuperUser And g.To_PivaSuperUser = PivaSuperUser_Destinazione And g.From_Ricetta_Cod = o.Ricetta_Cod And g.From_Ricetta_Operazione_Cod = o.Ricetta_Operazione_Cod)
            Select a).Distinct.ToList().Union(
            (From ra In GiasContext.RicettexAgenda Where ricette_operazioni_cod_update.Contains(ra.Ricetta_Operazione_Cod))
        ).Distinct.ToList()

        rval.ricettexagenda_delete = New List(Of RicettexAgenda) '(From ra In GiasContext.RicettexAgenda Where ricette_operazioni_cod_update.Contains(ra.Ricetta_Operazione_Cod)).Distinct.ToList

        'Ricette x Cultivar
        rval.ricettexcultivar_insert = (
            From r In GiasContext.Ricette
            Join o In GiasContext.Ricette_Operazioni On r.Ricetta_Cod Equals o.Ricetta_Cod
            Join a In GiasContext.RicettexCultivar On o.Ricetta_Cod Equals a.Ricetta_Cod
            Where r.Piva = piva _
                AndAlso r.Validita_Fine >= objOpzioniImportImpresa.ValiditaInizio_ricette _
                AndAlso (oConfigurazione.listaRicette_Tipo.Count = 0 Or oConfigurazione.listaRicette_Tipo.Contains(r.Tipo_Ricetta)) _
                AndAlso Not GiasContext.G2G_Recode_Ricette.Any(Function(g) g.From_PivaSuperUser = r.Ricetta_SuperUser And g.To_PivaSuperUser = PivaSuperUser_Destinazione And g.From_Ricetta_Cod = r.Ricetta_Cod)
            Select a).Distinct.ToList().Union(
            (From ra In GiasContext.RicettexCultivar Where ricette_cod_update.Contains(ra.Ricetta_Cod))
        ).Distinct.ToList()

        rval.ricettexcultivar_delete = New List(Of RicettexCultivar) '(From ra In GiasContext.RicettexCultivar Where ricette_cod_update.Contains(ra.Ricetta_Cod)).Distinct.ToList

        'Ricette x Agenda
        rval.ricettexnote_insert = (
            From r In GiasContext.Ricette
            Join o In GiasContext.Ricette_Operazioni On r.Ricetta_Cod Equals o.Ricetta_Cod
            Join a In GiasContext.RicettexNote On o.Ricetta_Cod Equals a.Ricetta_Cod And o.Ricetta_Operazione_Cod Equals a.Ricetta_Operazione_Cod
            Where r.Piva = piva _
                AndAlso r.Validita_Fine >= objOpzioniImportImpresa.ValiditaInizio_ricette _
                AndAlso (oConfigurazione.listaRicette_Tipo.Count = 0 Or oConfigurazione.listaRicette_Tipo.Contains(r.Tipo_Ricetta)) _
                AndAlso Not GiasContext.G2G_Recode_Ricette_Operazioni.Any(Function(g) g.From_PivaSuperUser = r.Ricetta_SuperUser And g.To_PivaSuperUser = PivaSuperUser_Destinazione And g.From_Ricetta_Cod = o.Ricetta_Cod And g.From_Ricetta_Operazione_Cod = o.Ricetta_Operazione_Cod)
            Select a).Distinct.ToList().Union(
        (From ra In GiasContext.RicettexNote Where ricette_operazioni_cod_update.Contains(ra.Ricetta_Operazione_Cod))
        ).Distinct.ToList()

        rval.ricettexnote_delete = New List(Of RicettexNote) '(From ra In GiasContext.RicettexNote Where ricette_operazioni_cod_update.Contains(ra.Ricetta_Operazione_Cod)).Distinct.ToList

        Return rval

    End Function

    Public Function LeggiPerGias2Gias_Reverse(
        ByVal objOpzioniImportImpresa As clsImpresa,
        ByVal piva As String,
        ByVal piva_destinazione As String,
        ByVal PivaSuperUser_Destinazione As String,
        ByRef objParametri As AgronicaCoreParametri
    ) As G2G_Ricette_Reverse


        Dim NomeRoutine As String = "G2GlocalDal.G2GRicette_R.LeggiPerGias2Gias_Reverse()"



        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
        Dim GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

        Dim oConfigurazione As G2G_Configurazione_FiltriReq_Ricette
        If objOpzioniImportImpresa.configurazione_ricette <> "" Then
            oConfigurazione = JsonConvert.DeserializeObject(Of G2G_Configurazione_FiltriReq_Ricette)(objOpzioniImportImpresa.configurazione_ricette)
        Else
            oConfigurazione = New G2G_Configurazione_FiltriReq_Ricette
            oConfigurazione.listaRicette_Tipo = New List(Of Integer)
        End If


        Dim rval As New G2G_Ricette_Reverse
        GiasContext.Database.CommandTimeout = 3600
        'quanto sicuramente non assegnato ...
        With rval
            .From_Piva = piva
            .From_PivaSuperUser = objParametri.PivaSuperUser
            .G2G_Ricette_Recode_insert = New List(Of G2G_Recode_Ricette)
            .G2G_Ricette_Recode_update = New List(Of G2G_Recode_Ricette)
            .G2G_Ricette_Operazioni_Recode_insert = New List(Of G2G_Recode_Ricette_Operazioni)
            .G2G_Ricette_Operazioni_Recode_update = New List(Of G2G_Recode_Ricette_Operazioni)
            .G2G_Ricette_Dettagli_Recode_insert = New List(Of G2G_Recode_Ricette_Dettagli)
            .G2G_Ricette_Dettagli_Recode_update = New List(Of G2G_Recode_Ricette_Dettagli)
            .G2G_Ricette_Dettaglio_Tecnico_Recode_insert = New List(Of G2G_Recode_Ricette_Dettaglio_Tecnico)
            .G2G_Ricette_Dettaglio_Tecnico_Recode_update = New List(Of G2G_Recode_Ricette_Dettaglio_Tecnico)
            .G2G_Ricette_Destinazioni_Recode_insert = New List(Of G2G_Recode_Ricette_Destinazioni)
            .G2G_Ricette_Destinazioni_Recode_update = New List(Of G2G_Recode_Ricette_Destinazioni)
            .ricette_destinazioni_delete = New List(Of Ricette_Destinazioni)
            .ricette_dettaglio_tecnico_delete = New List(Of Ricette_Dettaglio_Tecnico)
            .ricette_dettagli_delete = New List(Of Ricette_Dettagli)
            .ricette_operazioni_delete = New List(Of Ricette_Operazioni)
            .ricette_delete = New List(Of Ricette)
            .To_Piva = piva_destinazione
        End With

        Dim ricette_cod_update As New List(Of Integer)
        Dim ricette_operazioni_cod_update As New List(Of Integer)

        'Elaborazioni
        rval.ricette_insert = (
            From r In GiasContext.Ricette
            Where r.Piva = piva _
                AndAlso r.Validita_Fine >= objOpzioniImportImpresa.ValiditaInizio_ricette _
                AndAlso (oConfigurazione.listaRicette_Tipo.Count = 0 Or oConfigurazione.listaRicette_Tipo.Contains(r.Tipo_Ricetta)) _
                AndAlso Not GiasContext.G2G_Recode_Ricette.Any(Function(g) g.To_PivaSuperUser = r.Ricetta_SuperUser _
                                                                            And g.From_PivaSuperUser = PivaSuperUser_Destinazione _
                                                                            And g.To_Ricetta_Cod = r.Ricetta_Cod)
            Select r).Distinct.ToList()

        rval.ricette_update = (From r In GiasContext.Ricette
                               Join g2g In GiasContext.G2G_Recode_Ricette
                                   On r.Ricetta_Cod Equals g2g.To_Ricetta_Cod _
                                   And r.Ricetta_SuperUser Equals g2g.To_PivaSuperUser _
                                   And r.Piva Equals g2g.To_Piva
                               Where r.Piva = piva _
                                   AndAlso g2g.From_PivaSuperUser = PivaSuperUser_Destinazione _
                                   AndAlso System.Data.Entity.DbFunctions.AddDays(g2g.Data_Modifica, -1) < r.Data_Modifica
                               Select r).Distinct.ToList()

        For Each ricetta_update In rval.ricette_update
            ricette_cod_update.Add(ricetta_update.Ricetta_Cod)
        Next

        rval.G2G_Ricette_Recode_delete = (
            From r In GiasContext.G2G_Recode_Ricette
            Where Not GiasContext.Ricette.Any(Function(p) p.Ricetta_Cod = r.To_Ricetta_Cod And p.Ricetta_SuperUser = r.To_PivaSuperUser) _
                And r.To_Piva = piva _
                AndAlso r.From_PivaSuperUser = PivaSuperUser_Destinazione
            Select r).Distinct.ToList()

        'Testate
        rval.ricette_operazioni_insert = (
            From r In GiasContext.Ricette
            Join o In GiasContext.Ricette_Operazioni On r.Ricetta_Cod Equals o.Ricetta_Cod
            Where r.Piva = piva _
                AndAlso r.Validita_Fine >= objOpzioniImportImpresa.ValiditaInizio_ricette _
                AndAlso (oConfigurazione.listaRicette_Tipo.Count = 0 Or oConfigurazione.listaRicette_Tipo.Contains(r.Tipo_Ricetta)) _
                AndAlso Not GiasContext.G2G_Recode_Ricette_Operazioni.Any(Function(g) g.To_PivaSuperUser = r.Ricetta_SuperUser _
                                                                            And g.From_PivaSuperUser = PivaSuperUser_Destinazione _
                                                                            And g.To_Ricetta_Operazione_Cod = o.Ricetta_Operazione_Cod)
            Select o).Distinct.ToList()
        'And g.From_Ricetta_Cod = o.Ricetta_Cod _

        rval.ricette_operazioni_update = (
        From o In GiasContext.Ricette_Operazioni
        Join r In GiasContext.Ricette On o.Ricetta_Cod Equals r.Ricetta_Cod
        Join g2g In GiasContext.G2G_Recode_Ricette_Operazioni
                On o.Ricetta_Cod Equals g2g.To_Ricetta_Cod _
                And o.Ricetta_Operazione_Cod Equals g2g.To_Ricetta_Operazione_Cod _
                And r.Piva Equals g2g.To_Piva _
                And o.Ricetta_SuperUser Equals g2g.To_PivaSuperUser
        Where r.Piva = piva _
                AndAlso g2g.From_PivaSuperUser = PivaSuperUser_Destinazione _
                AndAlso System.Data.Entity.DbFunctions.AddDays(g2g.Data_Modifica, -1) < o.Data_Modifica
        Select o
            ).Distinct.ToList()

        For Each ricetta_operazione_update In rval.ricette_operazioni_update
            ricette_operazioni_cod_update.Add(ricetta_operazione_update.Ricetta_Operazione_Cod)
        Next

        rval.G2G_Ricette_Operazioni_Recode_delete = (
            From r In GiasContext.G2G_Recode_Ricette_Operazioni
            Where Not GiasContext.Ricette_Operazioni.Any(Function(p) p.Ricetta_Cod = r.To_Ricetta_Cod _
                                                            And p.Ricetta_Operazione_Cod = r.To_Ricetta_Operazione_Cod _
                                                            And p.Ricetta_SuperUser = r.To_PivaSuperUser) _
                And r.From_Piva = piva _
                And r.From_PivaSuperUser = PivaSuperUser_Destinazione
            Select r).Distinct.ToList()
        'rval.G2G_PianoConcimazione_Testata_Recode_delete = (
        '    From r In GiasContext.G2G_Recode_PianoConcimazione_Testata
        '    Join e In GiasContext.G2G_Recode_PianoConcimazione_EntitaxTestata
        '        On r.From_PC_Testata_Cod Equals e.From_PC_Testata_Cod _
        '        And r.From_PivaSuperUser Equals e.From_PivaSuperUser
        '    Where Not GiasContext.PianoConcimazione_Testata.Any(Function(p) p.PC_Testata_Cod = r.From_PC_Testata_Cod And p.PC_SuperUser = r.From_PivaSuperUser) _
        '        And e.From_Piva = piva
        '    Select r).Distinct.ToList()

        'Dettagli
        rval.ricette_dettagli_insert = (
            From r In GiasContext.Ricette
            Join o In GiasContext.Ricette_Operazioni On r.Ricetta_Cod Equals o.Ricetta_Cod
            Join d In GiasContext.Ricette_Dettagli On o.Ricetta_Cod Equals d.Ricetta_Cod And o.Ricetta_Operazione_Cod Equals d.Ricetta_Operazione_Cod
            Where r.Piva = piva _
                AndAlso r.Validita_Fine >= objOpzioniImportImpresa.ValiditaInizio_ricette _
                AndAlso (oConfigurazione.listaRicette_Tipo.Count = 0 Or oConfigurazione.listaRicette_Tipo.Contains(r.Tipo_Ricetta)) _
                AndAlso Not GiasContext.G2G_Recode_Ricette_Dettagli.Any(Function(g) g.To_PivaSuperUser = d.Ricetta_SuperUser _
                                                                                    And g.From_PivaSuperUser = PivaSuperUser_Destinazione _
                                                                                    And g.To_Ricetta_Dettaglio_Cod = d.Ricetta_Dettaglio_Cod)
            Select d).Distinct.ToList()

        'And g.From_Ricetta_Cod = d.Ricetta_Cod _
        'And g.From_Ricetta_Operazione_Cod = d.Ricetta_Operazione_Cod _

        rval.ricette_dettagli_update = (From o In GiasContext.Ricette_Dettagli
                                        Join r In GiasContext.Ricette On o.Ricetta_Cod Equals r.Ricetta_Cod
                                        Join g2g In GiasContext.G2G_Recode_Ricette_Dettagli
                                            On o.Ricetta_Cod Equals g2g.To_Ricetta_Cod _
                                            And o.Ricetta_Operazione_Cod Equals g2g.To_Ricetta_Operazione_Cod _
                                            And o.Ricetta_Dettaglio_Cod Equals g2g.To_Ricetta_Dettaglio_Cod _
                                            And r.Piva Equals g2g.To_Piva _
                                            And o.Ricetta_SuperUser Equals g2g.To_PivaSuperUser
                                        Where r.Piva = piva _
                                            AndAlso g2g.From_PivaSuperUser = PivaSuperUser_Destinazione _
                                            AndAlso System.Data.Entity.DbFunctions.AddDays(g2g.Data_Modifica, -1) < o.Data_Modifica
                                        Select o
                                            ).Distinct.ToList

        rval.G2G_Ricette_Dettagli_Recode_delete = (
            From r In GiasContext.G2G_Recode_Ricette_Dettagli
            Where Not GiasContext.Ricette_Dettagli.Any(Function(p) p.Ricetta_Cod = r.To_Ricetta_Cod _
                                                            And p.Ricetta_Operazione_Cod = r.To_Ricetta_Operazione_Cod _
                                                            And p.Ricetta_Dettaglio_Cod = r.To_Ricetta_Dettaglio_Cod _
                                                            And p.Ricetta_SuperUser = r.To_PivaSuperUser) _
                And r.From_Piva = piva _
                And r.From_PivaSuperUser = PivaSuperUser_Destinazione
            Select r).Distinct.ToList()

        'Ricette Dettaglio Tecnico
        rval.ricette_dettaglio_tecnico_insert = (
            From r In GiasContext.Ricette
            Join o In GiasContext.Ricette_Operazioni On r.Ricetta_Cod Equals o.Ricetta_Cod
            Join d In GiasContext.Ricette_Dettagli On o.Ricetta_Cod Equals d.Ricetta_Cod And o.Ricetta_Operazione_Cod Equals d.Ricetta_Operazione_Cod
            Join dt In GiasContext.Ricette_Dettaglio_Tecnico On d.Ricetta_Cod Equals dt.Ricetta_Cod And d.Ricetta_Operazione_Cod Equals dt.Ricetta_Operazione_Cod And d.Ricetta_Dettaglio_Cod Equals dt.Ricetta_Dettaglio_Cod
            Where r.Piva = piva _
                AndAlso r.Validita_Fine >= objOpzioniImportImpresa.ValiditaInizio_ricette _
                AndAlso (oConfigurazione.listaRicette_Tipo.Count = 0 Or oConfigurazione.listaRicette_Tipo.Contains(r.Tipo_Ricetta)) _
                AndAlso Not GiasContext.G2G_Recode_Ricette_Dettaglio_Tecnico.Any(Function(g) g.To_PivaSuperUser = dt.Ricetta_SuperUser _
                                                                                     And g.From_PivaSuperUser = PivaSuperUser_Destinazione _
                                                                                     And g.To_Ricetta_Tecnico_Cod = dt.Ricetta_Tecnico_Cod)
            Select dt).Distinct.ToList()

        'And g.From_Ricetta_Cod = dt.Ricetta_Cod _
        'And g.From_Ricetta_Operazione_Cod = dt.Ricetta_Operazione_Cod _
        'And g.From_Ricetta_Dettaglio_Cod = dt.Ricetta_Dettaglio_Cod _

        rval.ricette_dettaglio_tecnico_update = (From o In GiasContext.Ricette_Dettaglio_Tecnico
                                                 Join r In GiasContext.Ricette On o.Ricetta_Cod Equals r.Ricetta_Cod
                                                 Join g2g In GiasContext.G2G_Recode_Ricette_Dettaglio_Tecnico
                                                    On o.Ricetta_Cod Equals g2g.To_Ricetta_Cod _
                                                    And o.Ricetta_Operazione_Cod Equals g2g.To_Ricetta_Operazione_Cod _
                                                    And o.Ricetta_Dettaglio_Cod Equals g2g.To_Ricetta_Dettaglio_Cod _
                                                     And o.Ricetta_Tecnico_Cod Equals g2g.To_Ricetta_Tecnico_Cod _
                                                    And r.Piva Equals g2g.To_Piva _
                                                    And o.Ricetta_SuperUser Equals g2g.To_PivaSuperUser
                                                 Where r.Piva = piva _
                                                    AndAlso g2g.From_PivaSuperUser = PivaSuperUser_Destinazione _
                                                    AndAlso System.Data.Entity.DbFunctions.AddDays(g2g.Data_Modifica, -1) < o.Data_Modifica
                                                 Select o
                                            ).Distinct.ToList()

        rval.G2G_Ricette_Dettaglio_Tecnico_Recode_delete = (
            From r In GiasContext.G2G_Recode_Ricette_Dettaglio_Tecnico
            Where Not GiasContext.Ricette_Dettaglio_Tecnico.Any(Function(p) p.Ricetta_Cod = r.To_Ricetta_Cod _
                                                            And p.Ricetta_Operazione_Cod = r.To_Ricetta_Operazione_Cod _
                                                            And p.Ricetta_Dettaglio_Cod = r.To_Ricetta_Dettaglio_Cod _
                                                            And p.Ricetta_Tecnico_Cod = r.To_Ricetta_Tecnico_Cod _
                                                            And p.Ricetta_SuperUser = r.To_PivaSuperUser) _
                And r.From_Piva = piva _
                And r.From_PivaSuperUser = PivaSuperUser_Destinazione
            Select r).Distinct.ToList()

        'Ricette Destinazioni
        rval.ricette_destinazioni_insert = (
            From r In GiasContext.Ricette
            Join o In GiasContext.Ricette_Operazioni On r.Ricetta_Cod Equals o.Ricetta_Cod
            Join d In GiasContext.Ricette_Dettagli On o.Ricetta_Cod Equals d.Ricetta_Cod And o.Ricetta_Operazione_Cod Equals d.Ricetta_Operazione_Cod
            Join rd In GiasContext.Ricette_Destinazioni On d.Ricetta_Cod Equals rd.Ricetta_Cod And d.Ricetta_Operazione_Cod Equals rd.Ricetta_Operazione_Cod And d.Ricetta_Dettaglio_Cod Equals rd.Ricetta_Dettaglio_Cod
            Group Join g In GiasContext.G2G_Recode_Ricette_Destinazioni.Where(Function(x) x.From_PivaSuperUser = PivaSuperUser_Destinazione) On
                                                                        g.To_PivaSuperUser Equals rd.Ricetta_SuperUser _
                                                                        And g.To_Ricetta_Cod Equals rd.Ricetta_Cod _
                                                                        And g.To_Ricetta_Operazione_Cod Equals rd.Ricetta_Operazione_Cod _
                                                                        And g.To_Ricetta_Dettaglio_Cod Equals rd.Ricetta_Dettaglio_Cod _
                                                                        And g.To_Ricetta_Destinazione_Cod Equals rd.Ricetta_Destinazione_Cod
                    Into g_group = Group
            From _g_group In g_group.DefaultIfEmpty()
            Where r.Piva = piva _
                AndAlso r.Validita_Fine >= objOpzioniImportImpresa.ValiditaInizio_ricette _
                AndAlso (oConfigurazione.listaRicette_Tipo.Count = 0 Or oConfigurazione.listaRicette_Tipo.Contains(r.Tipo_Ricetta)) _
                AndAlso _g_group Is Nothing
            Select rd).Distinct.ToList()

        'AndAlso Not GiasContext.G2G_Recode_Ricette_Destinazioni.Any(Function(g) g.From_PivaSuperUser = rd.Ricetta_SuperUser _
        '                                                                            And g.From_Ricetta_Cod = rd.Ricetta_Cod _
        '                                                                            And g.From_Ricetta_Operazione_Cod = rd.Ricetta_Operazione_Cod _
        '                                                                            And g.From_Ricetta_Dettaglio_Cod = rd.Ricetta_Dettaglio_Cod _
        '                                                                            And g.From_Ricetta_Destinazione_Cod = rd.Ricetta_Destinazione_Cod _
        '                                                                            And g.To_PivaSuperUser = PivaSuperUser_Destinazione)


        rval.ricette_destinazioni_update = (From o In GiasContext.Ricette_Destinazioni
                                            Join r In GiasContext.Ricette On o.Ricetta_Cod Equals r.Ricetta_Cod
                                            Join g2g In GiasContext.G2G_Recode_Ricette_Destinazioni
                                                    On o.Ricetta_Cod Equals g2g.To_Ricetta_Cod _
                                                    And o.Ricetta_Operazione_Cod Equals g2g.To_Ricetta_Operazione_Cod _
                                                    And o.Ricetta_Dettaglio_Cod Equals g2g.To_Ricetta_Dettaglio_Cod _
                                                     And o.Ricetta_Destinazione_Cod Equals g2g.To_Ricetta_Destinazione_Cod _
                                                    And r.Piva Equals g2g.To_Piva _
                                                    And o.Ricetta_SuperUser Equals g2g.To_PivaSuperUser
                                            Where r.Piva = piva _
                                                    AndAlso g2g.From_PivaSuperUser = PivaSuperUser_Destinazione _
                                                    AndAlso System.Data.Entity.DbFunctions.AddDays(g2g.Data_Modifica, -1) < o.Data_Modifica
                                            Select o
                                            ).Distinct.ToList()


        rval.G2G_Ricette_Destinazioni_Recode_delete = (
            From r In GiasContext.G2G_Recode_Ricette_Destinazioni
            Where Not GiasContext.Ricette_Destinazioni.Any(Function(p) p.Ricetta_Cod = r.To_Ricetta_Cod _
                                                            And p.Ricetta_Operazione_Cod = r.To_Ricetta_Operazione_Cod _
                                                            And p.Ricetta_Dettaglio_Cod = r.To_Ricetta_Dettaglio_Cod _
                                                            And p.Ricetta_Destinazione_Cod = r.To_Ricetta_Destinazione_Cod _
                                                            And p.Ricetta_SuperUser = r.To_PivaSuperUser) _
                And r.From_Piva = piva _
                And r.From_PivaSuperUser = PivaSuperUser_Destinazione
            Select r).Distinct.ToList()

        'Ricette x Agenda
        rval.ricettexagenda_insert = (
            From r In GiasContext.Ricette
            Join o In GiasContext.Ricette_Operazioni On r.Ricetta_Cod Equals o.Ricetta_Cod
            Join a In GiasContext.RicettexAgenda On o.Ricetta_Cod Equals a.Ricetta_Cod And o.Ricetta_Operazione_Cod Equals a.Ricetta_Operazione_Cod
            Where r.Piva = piva _
                AndAlso r.Validita_Fine >= objOpzioniImportImpresa.ValiditaInizio_ricette _
                AndAlso (oConfigurazione.listaRicette_Tipo.Count = 0 Or oConfigurazione.listaRicette_Tipo.Contains(r.Tipo_Ricetta)) _
                AndAlso Not GiasContext.G2G_Recode_Ricette_Operazioni.Any(Function(g) g.To_PivaSuperUser = r.Ricetta_SuperUser _
                                                                            And g.From_PivaSuperUser = PivaSuperUser_Destinazione _
                                                                            And g.To_Ricetta_Cod = o.Ricetta_Cod _
                                                                            And g.To_Ricetta_Operazione_Cod = o.Ricetta_Operazione_Cod)
            Select a).Distinct.ToList().Union(
            (From ra In GiasContext.RicettexAgenda Where ricette_operazioni_cod_update.Contains(ra.Ricetta_Operazione_Cod))
        ).Distinct.ToList()

        rval.ricettexagenda_delete = New List(Of RicettexAgenda) '(From ra In GiasContext.RicettexAgenda Where ricette_operazioni_cod_update.Contains(ra.Ricetta_Operazione_Cod)).Distinct.ToList

        'Ricette x Cultivar
        rval.ricettexcultivar_insert = (
            From r In GiasContext.Ricette
            Join o In GiasContext.Ricette_Operazioni On r.Ricetta_Cod Equals o.Ricetta_Cod
            Join a In GiasContext.RicettexCultivar On o.Ricetta_Cod Equals a.Ricetta_Cod
            Where r.Piva = piva _
                AndAlso r.Validita_Fine >= objOpzioniImportImpresa.ValiditaInizio_ricette _
                AndAlso (oConfigurazione.listaRicette_Tipo.Count = 0 Or oConfigurazione.listaRicette_Tipo.Contains(r.Tipo_Ricetta)) _
                AndAlso Not GiasContext.G2G_Recode_Ricette.Any(Function(g) g.To_PivaSuperUser = r.Ricetta_SuperUser _
                                                                And g.From_PivaSuperUser = PivaSuperUser_Destinazione _
                                                                And g.To_Ricetta_Cod = r.Ricetta_Cod)
            Select a).Distinct.ToList().Union(
            (From ra In GiasContext.RicettexCultivar Where ricette_cod_update.Contains(ra.Ricetta_Cod))
        ).Distinct.ToList()

        rval.ricettexcultivar_delete = New List(Of RicettexCultivar) '(From ra In GiasContext.RicettexCultivar Where ricette_cod_update.Contains(ra.Ricetta_Cod)).Distinct.ToList

        'Ricette x Agenda
        rval.ricettexnote_insert = (
            From r In GiasContext.Ricette
            Join o In GiasContext.Ricette_Operazioni On r.Ricetta_Cod Equals o.Ricetta_Cod
            Join a In GiasContext.RicettexNote On o.Ricetta_Cod Equals a.Ricetta_Cod And o.Ricetta_Operazione_Cod Equals a.Ricetta_Operazione_Cod
            Where r.Piva = piva _
                AndAlso r.Validita_Fine >= objOpzioniImportImpresa.ValiditaInizio_ricette _
                AndAlso (oConfigurazione.listaRicette_Tipo.Count = 0 Or oConfigurazione.listaRicette_Tipo.Contains(r.Tipo_Ricetta)) _
                AndAlso Not GiasContext.G2G_Recode_Ricette_Operazioni.Any(Function(g) g.To_PivaSuperUser = r.Ricetta_SuperUser _
                                                                            And g.From_PivaSuperUser = PivaSuperUser_Destinazione _
                                                                            And g.To_Ricetta_Cod = o.Ricetta_Cod _
                                                                            And g.To_Ricetta_Operazione_Cod = o.Ricetta_Operazione_Cod)
            Select a).Distinct.ToList().Union(
        (From ra In GiasContext.RicettexNote Where ricette_operazioni_cod_update.Contains(ra.Ricetta_Operazione_Cod))
        ).Distinct.ToList()

        rval.ricettexnote_delete = New List(Of RicettexNote) '(From ra In GiasContext.RicettexNote Where ricette_operazioni_cod_update.Contains(ra.Ricetta_Operazione_Cod)).Distinct.ToList

        Return rval

    End Function

End Class

Public Class G2GRicette_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Scrivi_Ricette_G2G(Origine_Piva_SuperUser As String, Destinazione_Piva_SuperUser As String, ByRef g2g As G2G_Ricette, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As G2G_Recode

        Dim nomeRoutine As String = "AgronicaCoreG2GLocalDal.G2GRicette_W.Scrivi_Ricette_G2G()"
        Dim messaggioErrore As String = ""


        Dim username = objParametri.UsernameOperazione
        Dim piva = g2g.To_Piva
        Dim from_piva = g2g.From_Piva
        Dim data = Date.Now

        Dim objSequenze = New AgronicaCoreDataProvider.Agro_Sequenze
        Dim retries As Integer = 3
        Dim success As Boolean = True

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Dim transactionOptions As New TransactionOptions()
        transactionOptions.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted
        transactionOptions.Timeout = G2GUtility.GetTransactionTimeout(objParametri)

        Try

            Using scope As New TransactionScope(TransactionScopeOption.Required, transactionOptions)

                Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

                    '
                    'DELETE
                    '


                    For Each s As RicettexAgenda In g2g.ricettexagenda_delete

                        Dim ricettaxagenda = Gias_EF_Utility.CopyEntity(GiasContext, s, Nothing, username, data)

                        ricettaxagenda.Ricetta_SuperUser = Destinazione_Piva_SuperUser

                        ricettaxagenda.Ricetta_Cod = (From t In GiasContext.G2G_Recode_Ricette
                                                      Where t.From_PivaSuperUser = Origine_Piva_SuperUser _
                                                           And t.To_PivaSuperUser = Destinazione_Piva_SuperUser _
                                                           And t.To_Piva = piva _
                                                           And t.From_Ricetta_Cod = s.Ricetta_Cod).FirstOrDefault.To_Ricetta_Cod

                        ricettaxagenda.Ricetta_Operazione_Cod = (From t In GiasContext.G2G_Recode_Ricette_Operazioni
                                                                 Where t.From_PivaSuperUser = Origine_Piva_SuperUser _
                                                                   And t.To_PivaSuperUser = Destinazione_Piva_SuperUser _
                                                                   And t.To_Piva = piva _
                                                                   And t.From_Ricetta_Cod = s.Ricetta_Cod _
                                                                   And t.From_Ricetta_Operazione_Cod = s.Ricetta_Operazione_Cod).FirstOrDefault.To_Ricetta_Operazione_Cod

                        GiasContext.RicettexAgenda.Attach(ricettaxagenda)
                        GiasContext.RicettexAgenda.Remove(ricettaxagenda)

                    Next
                    GiasContext.SaveChanges()

                    For Each s As RicettexCultivar In g2g.ricettexcultivar_delete

                        Dim ricettaxcultivar = Gias_EF_Utility.CopyEntity(GiasContext, s, Nothing, username, data)

                        ricettaxcultivar.Ricetta_Cod = (From t In GiasContext.G2G_Recode_Ricette
                                                        Where t.From_PivaSuperUser = Origine_Piva_SuperUser _
                                                           And t.To_PivaSuperUser = Destinazione_Piva_SuperUser _
                                                           And t.To_Piva = piva _
                                                           And t.From_Ricetta_Cod = s.Ricetta_Cod).FirstOrDefault.To_Ricetta_Cod

                        ricettaxcultivar.Ricetta_SuperUser = Destinazione_Piva_SuperUser

                        GiasContext.RicettexCultivar.Attach(ricettaxcultivar)
                        GiasContext.RicettexCultivar.Remove(ricettaxcultivar)

                    Next
                    GiasContext.SaveChanges()

                    For Each s As RicettexNote In g2g.ricettexnote_delete

                        Dim ricettaxnote = Gias_EF_Utility.CopyEntity(GiasContext, s, Nothing, username, data)

                        ricettaxnote.Ricetta_SuperUser = Destinazione_Piva_SuperUser

                        ricettaxnote.Ricetta_Cod = (From t In GiasContext.G2G_Recode_Ricette
                                                    Where t.From_PivaSuperUser = Origine_Piva_SuperUser _
                                                           And t.To_PivaSuperUser = Destinazione_Piva_SuperUser _
                                                           And t.To_Piva = piva _
                                                           And t.From_Ricetta_Cod = s.Ricetta_Cod).FirstOrDefault.To_Ricetta_Cod

                        ricettaxnote.Ricetta_Operazione_Cod = (From t In GiasContext.G2G_Recode_Ricette_Operazioni
                                                               Where t.From_PivaSuperUser = Origine_Piva_SuperUser _
                                                           And t.To_PivaSuperUser = Destinazione_Piva_SuperUser _
                                                           And t.To_Piva = piva _
                                                           And t.From_Ricetta_Cod = s.Ricetta_Cod _
                                                           And t.From_Ricetta_Operazione_Cod = s.Ricetta_Operazione_Cod).FirstOrDefault.To_Ricetta_Operazione_Cod

                        ricettaxnote.Nota_Cod = (From t In GiasContext.G2G_Recode_NoteIntervento
                                                 Where t.From_PivaSuperUser = Origine_Piva_SuperUser _
                                                           And t.To_PivaSuperUser = Destinazione_Piva_SuperUser _
                                                           And t.From_Nota_Cod = s.Nota_Cod).FirstOrDefault.To_Nota_Cod

                        GiasContext.RicettexNote.Attach(ricettaxnote)
                        GiasContext.RicettexNote.Remove(ricettaxnote)

                    Next
                    GiasContext.SaveChanges()

                    For Each r As G2G_Recode_Ricette_Destinazioni In g2g.G2G_Ricette_Destinazioni_Recode_delete

                        ''cancella destinazione
                        Dim ricette_destinazioni = (From m In GiasContext.Ricette_Destinazioni Where m.Ricetta_SuperUser = Destinazione_Piva_SuperUser _
                                                                                                   And m.Ricetta_Cod = r.To_Ricetta_Cod _
                                                                                                   And m.Ricetta_Operazione_Cod = r.To_Ricetta_Operazione_Cod _
                                                                                                   And m.Ricetta_Dettaglio_Cod = r.To_Ricetta_Dettaglio_Cod _
                                                                                                   And m.Ricetta_Destinazione_Cod = r.To_Ricetta_Destinazione_Cod).FirstOrDefault
                        GiasContext.Ricette_Destinazioni.Attach(ricette_destinazioni)
                        GiasContext.Ricette_Destinazioni.Remove(ricette_destinazioni)


                        '' cancella recode destinazione
                        Dim recode = (From rr In GiasContext.G2G_Recode_Ricette_Destinazioni Where rr.From_PivaSuperUser = Origine_Piva_SuperUser _
                                                                                                 And rr.From_Ricetta_Cod = r.From_Ricetta_Cod _
                                                                                                 And rr.From_Ricetta_Operazione_Cod = r.From_Ricetta_Operazione_Cod _
                                                                                                 And rr.From_Ricetta_Dettaglio_Cod = r.From_Ricetta_Dettaglio_Cod _
                                                                                                 And rr.From_Ricetta_Destinazione_Cod = r.From_Ricetta_Destinazione_Cod _
                                                                                                 And rr.To_PivaSuperUser = Destinazione_Piva_SuperUser _
                                                                                                 And rr.To_Ricetta_Cod = r.To_Ricetta_Cod _
                                                                                                 And rr.To_Ricetta_Operazione_Cod = r.To_Ricetta_Operazione_Cod _
                                                                                                 And rr.To_Ricetta_Dettaglio_Cod = r.To_Ricetta_Dettaglio_Cod _
                                                                                                 And rr.To_Ricetta_Destinazione_Cod = r.To_Ricetta_Destinazione_Cod).FirstOrDefault()
                        GiasContext.G2G_Recode_Ricette_Destinazioni.Attach(recode)
                        GiasContext.G2G_Recode_Ricette_Destinazioni.Remove(recode)

                    Next
                    GiasContext.SaveChanges()

                    For Each r As G2G_Recode_Ricette_Dettaglio_Tecnico In g2g.G2G_Ricette_Dettaglio_Tecnico_Recode_delete

                        ''cancella tecnico
                        Dim ricette_dettaglio = (From m In GiasContext.Ricette_Dettaglio_Tecnico Where m.Ricetta_SuperUser = Destinazione_Piva_SuperUser _
                                                                                                   And m.Ricetta_Cod = r.To_Ricetta_Cod _
                                                                                                   And m.Ricetta_Operazione_Cod = r.To_Ricetta_Operazione_Cod _
                                                                                                   And m.Ricetta_Dettaglio_Cod = r.To_Ricetta_Dettaglio_Cod _
                                                                                                   And m.Ricetta_Tecnico_Cod = r.To_Ricetta_Tecnico_Cod).FirstOrDefault
                        GiasContext.Ricette_Dettaglio_Tecnico.Attach(ricette_dettaglio)
                        GiasContext.Ricette_Dettaglio_Tecnico.Remove(ricette_dettaglio)


                        '' cancella recode tecnico
                        Dim recode = (From rr In GiasContext.G2G_Recode_Ricette_Dettaglio_Tecnico Where rr.From_PivaSuperUser = Origine_Piva_SuperUser _
                                                                                                 And rr.From_Ricetta_Cod = r.From_Ricetta_Cod _
                                                                                                 And rr.From_Ricetta_Operazione_Cod = r.From_Ricetta_Operazione_Cod _
                                                                                                 And rr.From_Ricetta_Dettaglio_Cod = r.From_Ricetta_Dettaglio_Cod _
                                                                                                 And rr.From_Ricetta_Tecnico_Cod = r.From_Ricetta_Tecnico_Cod _
                                                                                                 And rr.To_PivaSuperUser = Destinazione_Piva_SuperUser _
                                                                                                 And rr.To_Ricetta_Cod = r.To_Ricetta_Cod _
                                                                                                 And rr.To_Ricetta_Operazione_Cod = r.To_Ricetta_Operazione_Cod _
                                                                                                 And rr.To_Ricetta_Dettaglio_Cod = r.To_Ricetta_Dettaglio_Cod _
                                                                                                 And rr.To_Ricetta_Tecnico_Cod = r.To_Ricetta_Tecnico_Cod).FirstOrDefault()
                        GiasContext.G2G_Recode_Ricette_Dettaglio_Tecnico.Attach(recode)
                        GiasContext.G2G_Recode_Ricette_Dettaglio_Tecnico.Remove(recode)

                    Next
                    GiasContext.SaveChanges()

                    For Each r As G2G_Recode_Ricette_Dettagli In g2g.G2G_Ricette_Dettagli_Recode_delete

                        ''cancella dettaglio
                        Dim ricette_dettaglio = (From m In GiasContext.Ricette_Dettagli Where m.Ricetta_SuperUser = Destinazione_Piva_SuperUser _
                                                                                                   And m.Ricetta_Cod = r.To_Ricetta_Cod _
                                                                                                   And m.Ricetta_Operazione_Cod = r.To_Ricetta_Operazione_Cod _
                                                                                                   And m.Ricetta_Dettaglio_Cod = r.To_Ricetta_Dettaglio_Cod).FirstOrDefault
                        GiasContext.Ricette_Dettagli.Attach(ricette_dettaglio)
                        GiasContext.Ricette_Dettagli.Remove(ricette_dettaglio)


                        '' cancella recode dettaglio
                        Dim recode = (From rr In GiasContext.G2G_Recode_Ricette_Dettagli Where rr.From_PivaSuperUser = Origine_Piva_SuperUser _
                                                                                                 And rr.From_Ricetta_Cod = r.From_Ricetta_Cod _
                                                                                                 And rr.From_Ricetta_Operazione_Cod = r.From_Ricetta_Operazione_Cod _
                                                                                                 And rr.From_Ricetta_Dettaglio_Cod = r.From_Ricetta_Dettaglio_Cod _
                                                                                                 And rr.To_PivaSuperUser = Destinazione_Piva_SuperUser _
                                                                                                 And rr.To_Ricetta_Cod = r.To_Ricetta_Cod _
                                                                                                 And rr.To_Ricetta_Operazione_Cod = r.To_Ricetta_Operazione_Cod _
                                                                                                 And rr.To_Ricetta_Dettaglio_Cod = r.To_Ricetta_Dettaglio_Cod).FirstOrDefault()
                        GiasContext.G2G_Recode_Ricette_Dettagli.Attach(recode)
                        GiasContext.G2G_Recode_Ricette_Dettagli.Remove(recode)

                    Next
                    GiasContext.SaveChanges()


                    For Each r As G2G_Recode_Ricette_Operazioni In g2g.G2G_Ricette_Operazioni_Recode_delete

                        ''cancella operazione
                        Dim ricette_operazioni = (From m In GiasContext.Ricette_Operazioni Where m.Ricetta_SuperUser = Destinazione_Piva_SuperUser _
                                                                                                   And m.Ricetta_Cod = r.To_Ricetta_Cod _
                                                                                                   And m.Ricetta_Operazione_Cod = r.To_Ricetta_Operazione_Cod).FirstOrDefault
                        GiasContext.Ricette_Operazioni.Attach(ricette_operazioni)
                        GiasContext.Ricette_Operazioni.Remove(ricette_operazioni)

                        Dim ricettexagenda = (From ra In GiasContext.RicettexAgenda Where ra.Ricetta_Cod = ricette_operazioni.Ricetta_Cod _
                                                                                        And ra.Ricetta_Operazione_Cod = ricette_operazioni.Ricetta_Operazione_Cod _
                                                                                        And ra.Ricetta_SuperUser = ricette_operazioni.Ricetta_SuperUser).ToList
                        For Each ricettaxagenda In ricettexagenda
                            GiasContext.RicettexAgenda.Attach(ricettaxagenda)
                            GiasContext.RicettexAgenda.Remove(ricettaxagenda)
                        Next

                        Dim ricettexnote = (From ra In GiasContext.RicettexNote Where ra.Ricetta_Cod = ricette_operazioni.Ricetta_Cod _
                                                                                        And ra.Ricetta_Operazione_Cod = ricette_operazioni.Ricetta_Operazione_Cod _
                                                                                        And ra.Ricetta_SuperUser = ricette_operazioni.Ricetta_SuperUser).ToList
                        For Each ricettaxnote In ricettexnote
                            GiasContext.RicettexNote.Attach(ricettaxnote)
                            GiasContext.RicettexNote.Remove(ricettaxnote)
                        Next

                        '' cancella recode operazione
                        Dim recode = (From rr In GiasContext.G2G_Recode_Ricette_Operazioni Where rr.From_PivaSuperUser = Origine_Piva_SuperUser _
                                                                                                 And rr.From_Ricetta_Cod = r.From_Ricetta_Cod _
                                                                                                 And rr.From_Ricetta_Operazione_Cod = r.From_Ricetta_Operazione_Cod _
                                                                                                 And rr.To_PivaSuperUser = Destinazione_Piva_SuperUser _
                                                                                                 And rr.To_Ricetta_Cod = r.To_Ricetta_Cod _
                                                                                                 And rr.To_Ricetta_Operazione_Cod = r.To_Ricetta_Operazione_Cod).FirstOrDefault()
                        GiasContext.G2G_Recode_Ricette_Operazioni.Attach(recode)
                        GiasContext.G2G_Recode_Ricette_Operazioni.Remove(recode)


                    Next
                    GiasContext.SaveChanges()



                    For Each r As G2G_Recode_Ricette In g2g.G2G_Ricette_Recode_delete

                        ''cancella ricetta
                        Dim ricette = (From m In GiasContext.Ricette Where m.Ricetta_SuperUser = Destinazione_Piva_SuperUser _
                                                                                                   And m.Ricetta_Cod = r.To_Ricetta_Cod).FirstOrDefault
                        GiasContext.Ricette.Attach(ricette)
                        GiasContext.Ricette.Remove(ricette)

                        Dim ricettexcultivar = (From ra In GiasContext.RicettexCultivar Where ra.Ricetta_Cod = ricette.Ricetta_Cod _
                                                                                        And ra.Ricetta_SuperUser = ricette.Ricetta_SuperUser).ToList
                        For Each ricettaxcultivar In ricettexcultivar
                            GiasContext.RicettexCultivar.Attach(ricettaxcultivar)
                            GiasContext.RicettexCultivar.Remove(ricettaxcultivar)
                        Next

                        '' cancella recode operazione
                        Dim recode = (From rr In GiasContext.G2G_Recode_Ricette Where rr.From_PivaSuperUser = Origine_Piva_SuperUser _
                                                                                                 And rr.From_Ricetta_Cod = r.From_Ricetta_Cod _
                                                                                                 And rr.To_PivaSuperUser = Destinazione_Piva_SuperUser _
                                                                                                 And rr.To_Ricetta_Cod = r.To_Ricetta_Cod).FirstOrDefault()
                        GiasContext.G2G_Recode_Ricette.Attach(recode)
                        GiasContext.G2G_Recode_Ricette.Remove(recode)

                    Next

                    GiasContext.SaveChanges()


                    '
                    'UPDATE
                    '
                    For Each m As Ricette In g2g.ricette_update

                        Dim recode = (From rr In GiasContext.G2G_Recode_Ricette 
                                        Where rr.From_PivaSuperUser = Origine_Piva_SuperUser AndAlso
                                              rr.To_PivaSuperUser = Destinazione_Piva_SuperUser AndAlso
                                              rr.From_Ricetta_Cod = m.Ricetta_Cod).FirstOrDefault()
                        recode.Username_Modifica = username
                        recode.Data_Modifica = data
                        recode.datainvio = data
                        g2g.G2G_Ricette_Recode_update.Add(recode)
                        GiasContext.G2G_Recode_Ricette.Attach(recode)
                        GiasContext.Entry(recode).State = EntityState.Modified

                        Dim ricetta = (From mm In GiasContext.Ricette 
                                        Where mm.Ricetta_SuperUser = recode.To_PivaSuperUser AndAlso
                                              mm.Ricetta_Cod = recode.To_Ricetta_Cod).FirstOrDefault()
                        ricetta = Gias_EF_Utility.CopyEntity(GiasContext, m, ricetta, username, data)
                        ricetta.Ricetta_Cod = recode.To_Ricetta_Cod
                        ricetta.Ricetta_SuperUser = Destinazione_Piva_SuperUser
                        ricetta.Piva = piva

                        Dim to_programmazione_cod As Integer = 0
                        If ricetta.Programmazione_Cod <> 0 Then

                            Select Case ricetta.Tipo_Ricetta
                                Case enum_TipoRicetta.PUA
                                    to_programmazione_cod = (From g2gPua In GiasContext.G2G_Recode_Programmazione_Testata Where g2gPua.From_PivaSuperUser = m.Ricetta_SuperUser _
                                                                                 And g2gPua.To_PivaSuperUser = Destinazione_Piva_SuperUser _
                                                                                 And g2gPua.From_Piva = m.Piva _
                                                                                 And g2gPua.From_Programmazione_Cod = m.Programmazione_Cod).FirstOrDefault.To_Programmazione_Cod
                                Case enum_TipoRicetta.Standard_Destinazioni
                                    If ricetta.Programmazione_Cod <> 0 Then
                                        Throw New Exception("Programmazione Cod non mappata")
                                    End If
                                Case enum_TipoRicetta.PianoDistribuzioneConcimi
                                    to_programmazione_cod = (From g2gPC In GiasContext.G2G_Recode_PianoConcimazione_Testata Where g2gPC.From_PivaSuperUser = m.Ricetta_SuperUser _
                                                                                                      And g2gPC.To_PivaSuperUser = Destinazione_Piva_SuperUser _
                                                                                                      And g2gPC.From_PC_Testata_Cod = m.Programmazione_Cod).FirstOrDefault.To_PC_Testata_Cod
                                Case enum_TipoRicetta.PianoDistribuzionePua
                                    to_programmazione_cod = (From g2gPua In GiasContext.G2G_Recode_Pua Where g2gPua.From_PivaSuperUser = m.Ricetta_SuperUser _
                                                                                 And g2gPua.To_PivaSuperUser = Destinazione_Piva_SuperUser _
                                                                                 And g2gPua.From_Piva = m.Piva _
                                                                                 And g2gPua.From_Pua_Cod = m.Programmazione_Cod).FirstOrDefault.To_Pua_Cod
                            End Select
                            ricetta.Programmazione_Cod = to_programmazione_cod
                        End If


                        Dim ricettexcultivar_s = (From rc In GiasContext.RicettexCultivar Where rc.Ricetta_SuperUser = Destinazione_Piva_SuperUser And rc.Ricetta_Cod = ricetta.Ricetta_Cod).ToList
                        For Each ricettexcultivar In ricettexcultivar_s
                            GiasContext.RicettexCultivar.Attach(ricettexcultivar)
                            GiasContext.RicettexCultivar.Remove(ricettexcultivar)
                        Next

                        GiasContext.Ricette.Attach(ricetta)
                        GiasContext.Entry(ricetta).State = EntityState.Modified

                    Next
                    GiasContext.SaveChanges()

                    For Each m As Ricette_Operazioni In g2g.ricette_operazioni_update

                        Dim recode = (From rr In GiasContext.G2G_Recode_Ricette_Operazioni 
                                    Where rr.From_PivaSuperUser = Origine_Piva_SuperUser AndAlso
                                          rr.To_PivaSuperUser = Destinazione_Piva_SuperUser AndAlso
                                          rr.From_Ricetta_Cod = m.Ricetta_Cod AndAlso
                                          rr.From_Ricetta_Operazione_Cod = m.Ricetta_Operazione_Cod).FirstOrDefault()
                        recode.Username_Modifica = username
                        recode.Data_Modifica = data
                        recode.datainvio = data
                        g2g.G2G_Ricette_Operazioni_Recode_update.Add(recode)
                        GiasContext.G2G_Recode_Ricette_Operazioni.Attach(recode)
                        GiasContext.Entry(recode).State = EntityState.Modified

                        Dim ricetta_operazione = (From mm In GiasContext.Ricette_Operazioni 
                                                Where mm.Ricetta_SuperUser = recode.To_PivaSuperUser AndAlso
                                                      mm.Ricetta_Cod = recode.To_Ricetta_Cod AndAlso
                                                      mm.Ricetta_Operazione_Cod = recode.To_Ricetta_Operazione_Cod).FirstOrDefault()
                        ricetta_operazione = Gias_EF_Utility.CopyEntity(GiasContext, m, ricetta_operazione, username, data)
                        ricetta_operazione.Ricetta_Cod = recode.To_Ricetta_Cod
                        ricetta_operazione.Ricetta_Operazione_Cod = recode.To_Ricetta_Operazione_Cod
                        ricetta_operazione.Ricetta_SuperUser = Destinazione_Piva_SuperUser

                        If ricetta_operazione.Ricetta_Operazione_Cod_RIF IsNot Nothing AndAlso ricetta_operazione.Ricetta_Operazione_Cod_RIF <> 0 Then

                            ricetta_operazione.Ricetta_Operazione_Cod_RIF = (From recode_operazione In GiasContext.G2G_Recode_Ricette_Operazioni Where recode_operazione.From_PivaSuperUser = Origine_Piva_SuperUser And recode_operazione.To_PivaSuperUser = Destinazione_Piva_SuperUser _
                                                                                                                                                     And recode_operazione.From_Ricetta_Operazione_Cod = m.Ricetta_Operazione_Cod_RIF).FirstOrDefault.To_Ricetta_Operazione_Cod

                        End If


                        Dim ricettexagendas_s = (From rc In GiasContext.RicettexAgenda Where rc.Ricetta_SuperUser = Destinazione_Piva_SuperUser And rc.Ricetta_Cod = ricetta_operazione.Ricetta_Cod And rc.Ricetta_Operazione_Cod = ricetta_operazione.Ricetta_Operazione_Cod).ToList
                        For Each ricettexagenda In ricettexagendas_s
                            GiasContext.RicettexAgenda.Attach(ricettexagenda)
                            GiasContext.RicettexAgenda.Remove(ricettexagenda)
                        Next

                        Dim ricettexnote_s = (From rc In GiasContext.RicettexNote Where rc.Ricetta_SuperUser = Destinazione_Piva_SuperUser And rc.Ricetta_Cod = ricetta_operazione.Ricetta_Cod And rc.Ricetta_Operazione_Cod = ricetta_operazione.Ricetta_Operazione_Cod).ToList
                        For Each ricettexnote In ricettexnote_s
                            GiasContext.RicettexNote.Attach(ricettexnote)
                            GiasContext.RicettexNote.Remove(ricettexnote)
                        Next

                        GiasContext.Ricette_Operazioni.Attach(ricetta_operazione)
                        GiasContext.Entry(ricetta_operazione).State = EntityState.Modified

                    Next
                    GiasContext.SaveChanges()


                    For Each m As Ricette_Dettagli In g2g.ricette_dettagli_update

                        Dim recode = (From rr In GiasContext.G2G_Recode_Ricette_Dettagli Where rr.From_PivaSuperUser = Origine_Piva_SuperUser _
                                                                                           And rr.To_PivaSuperUser = Destinazione_Piva_SuperUser _
                                                                                           And rr.From_Ricetta_Cod = m.Ricetta_Cod _
                                                                                           And rr.From_Ricetta_Operazione_Cod = m.Ricetta_Operazione_Cod _
                                                                                           And rr.From_Ricetta_Dettaglio_Cod = m.Ricetta_Dettaglio_Cod).FirstOrDefault()
                        recode.Username_Modifica = username
                        recode.Data_Modifica = data
                        recode.datainvio = data
                        g2g.G2G_Ricette_Dettagli_Recode_update.Add(recode)
                        GiasContext.G2G_Recode_Ricette_Dettagli.Attach(recode)
                        GiasContext.Entry(recode).State = EntityState.Modified

                        Dim ricetta_dettaglio = (From mm In GiasContext.Ricette_Dettagli Where mm.Ricetta_SuperUser = recode.To_PivaSuperUser _
                                                                          And mm.Ricetta_Cod = recode.To_Ricetta_Cod _
                                                                          And mm.Ricetta_Operazione_Cod = recode.To_Ricetta_Operazione_Cod _
                                                                          And mm.Ricetta_Dettaglio_Cod = recode.To_Ricetta_Dettaglio_Cod).FirstOrDefault()
                        ricetta_dettaglio = Gias_EF_Utility.CopyEntity(GiasContext, m, ricetta_dettaglio, username, data)
                        ricetta_dettaglio.Ricetta_Cod = recode.To_Ricetta_Cod
                        ricetta_dettaglio.Ricetta_Operazione_Cod = recode.To_Ricetta_Operazione_Cod
                        ricetta_dettaglio.Ricetta_Dettaglio_Cod = recode.To_Ricetta_Dettaglio_Cod
                        ricetta_dettaglio.Ricetta_SuperUser = Destinazione_Piva_SuperUser


                        GiasContext.Ricette_Dettagli.Attach(ricetta_dettaglio)
                        GiasContext.Entry(ricetta_dettaglio).State = EntityState.Modified

                    Next
                    GiasContext.SaveChanges()


                    For Each m As Ricette_Dettaglio_Tecnico In g2g.ricette_dettaglio_tecnico_update

                        Dim recode = (From rr In GiasContext.G2G_Recode_Ricette_Dettaglio_Tecnico Where rr.From_PivaSuperUser = Origine_Piva_SuperUser _
                                                                                               And rr.To_PivaSuperUser = Destinazione_Piva_SuperUser _
                                                                                               And rr.From_Ricetta_Cod = m.Ricetta_Cod _
                                                                                               And rr.From_Ricetta_Operazione_Cod = m.Ricetta_Operazione_Cod _
                                                                                               And rr.From_Ricetta_Dettaglio_Cod = m.Ricetta_Dettaglio_Cod _
                                                                                               And rr.From_Ricetta_Tecnico_Cod = m.Ricetta_Tecnico_Cod).FirstOrDefault()
                        recode.Username_Modifica = username
                        recode.Data_Modifica = data
                        recode.datainvio = data
                        g2g.G2G_Ricette_Dettaglio_Tecnico_Recode_update.Add(recode)
                        GiasContext.G2G_Recode_Ricette_Dettaglio_Tecnico.Attach(recode)
                        GiasContext.Entry(recode).State = EntityState.Modified

                        Dim ricetta_tecnico = (From mm In GiasContext.Ricette_Dettaglio_Tecnico Where mm.Ricetta_SuperUser = recode.To_PivaSuperUser _
                                                                                  And mm.Ricetta_Cod = recode.To_Ricetta_Cod _
                                                                                  And mm.Ricetta_Operazione_Cod = recode.To_Ricetta_Operazione_Cod _
                                                                                  And mm.Ricetta_Dettaglio_Cod = recode.To_Ricetta_Dettaglio_Cod _
                                                                                  And mm.Ricetta_Tecnico_Cod = recode.To_Ricetta_Tecnico_Cod).FirstOrDefault()
                        ricetta_tecnico = Gias_EF_Utility.CopyEntity(GiasContext, m, ricetta_tecnico, username, data)
                        ricetta_tecnico.Ricetta_Cod = recode.To_Ricetta_Cod
                        ricetta_tecnico.Ricetta_Operazione_Cod = recode.To_Ricetta_Operazione_Cod
                        ricetta_tecnico.Ricetta_Dettaglio_Cod = recode.To_Ricetta_Dettaglio_Cod
                        ricetta_tecnico.Ricetta_Tecnico_Cod = recode.To_Ricetta_Tecnico_Cod
                        ricetta_tecnico.Ricetta_SuperUser = Destinazione_Piva_SuperUser


                        GiasContext.Ricette_Dettaglio_Tecnico.Attach(ricetta_tecnico)
                        GiasContext.Entry(ricetta_tecnico).State = EntityState.Modified
                    Next

                    GiasContext.SaveChanges()

                    For Each m As Ricette_Destinazioni In g2g.ricette_destinazioni_update


                        Dim recode = (From rr In GiasContext.G2G_Recode_Ricette_Destinazioni Where rr.From_PivaSuperUser = Origine_Piva_SuperUser _
                                                                                               And rr.To_PivaSuperUser = Destinazione_Piva_SuperUser _
                                                                                               And rr.From_Ricetta_Cod = m.Ricetta_Cod _
                                                                                               And rr.From_Ricetta_Operazione_Cod = m.Ricetta_Operazione_Cod _
                                                                                               And rr.From_Ricetta_Dettaglio_Cod = m.Ricetta_Dettaglio_Cod _
                                                                                               And rr.From_Ricetta_Destinazione_Cod = m.Ricetta_Destinazione_Cod).FirstOrDefault()
                        recode.Username_Modifica = username
                        recode.Data_Modifica = data
                        recode.datainvio = data
                        g2g.G2G_Ricette_Destinazioni_Recode_update.Add(recode)
                        GiasContext.G2G_Recode_Ricette_Destinazioni.Attach(recode)
                        GiasContext.Entry(recode).State = EntityState.Modified

                        Dim ricetta_destinazione = (From mm In GiasContext.Ricette_Destinazioni Where mm.Ricetta_SuperUser = recode.To_PivaSuperUser _
                                                                          And mm.Ricetta_Cod = recode.To_Ricetta_Cod _
                                                                          And mm.Ricetta_Operazione_Cod = recode.To_Ricetta_Operazione_Cod _
                                                                          And mm.Ricetta_Dettaglio_Cod = recode.To_Ricetta_Dettaglio_Cod _
                                                                          And mm.Ricetta_Destinazione_Cod = recode.To_Ricetta_Destinazione_Cod).FirstOrDefault()
                        ricetta_destinazione = Gias_EF_Utility.CopyEntity(GiasContext, m, ricetta_destinazione, username, data)
                        ricetta_destinazione.Ricetta_Cod = recode.To_Ricetta_Cod
                        ricetta_destinazione.Ricetta_Operazione_Cod = recode.To_Ricetta_Operazione_Cod
                        ricetta_destinazione.Ricetta_Dettaglio_Cod = recode.To_Ricetta_Dettaglio_Cod
                        ricetta_destinazione.Ricetta_Destinazione_Cod = recode.To_Ricetta_Destinazione_Cod
                        ricetta_destinazione.Ricetta_SuperUser = Destinazione_Piva_SuperUser
                        ricetta_destinazione.Piva = piva


                        GiasContext.Ricette_Destinazioni.Attach(ricetta_destinazione)
                        GiasContext.Entry(ricetta_destinazione).State = EntityState.Modified

                    Next
                    GiasContext.SaveChanges()

                    '
                    'INSERT
                    '

                    For Each s As Ricette In g2g.ricette_insert

                        Dim idSeq As Integer = objSequenze.NuovoId_Tabella("ricette", 0, 2000000000, objParametri, UtilizzaTransazione:=False)
                        Dim ricetta = Gias_EF_Utility.CopyEntity(GiasContext, s, Nothing, username, data)

                        ricetta.Ricetta_Cod = idSeq
                        ricetta.Ricetta_SuperUser = Destinazione_Piva_SuperUser
                        ricetta.Piva = piva

                        Dim to_programmazione_cod As Integer = 0

                        If ricetta.Programmazione_Cod <> 0 Then

                            Select Case ricetta.Tipo_Ricetta
                                Case enum_TipoRicetta.PUA
                                    to_programmazione_cod = (From g2gPua In GiasContext.G2G_Recode_Programmazione_Testata Where g2gPua.From_PivaSuperUser = s.Ricetta_SuperUser _
                                                                                 And g2gPua.To_PivaSuperUser = Destinazione_Piva_SuperUser _
                                                                                 And g2gPua.From_Piva = s.Piva _
                                                                                 And g2gPua.From_Programmazione_Cod = s.Programmazione_Cod).FirstOrDefault.To_Programmazione_Cod
                                Case enum_TipoRicetta.Standard_Destinazioni
                                    If ricetta.Programmazione_Cod <> 0 Then
                                        Throw New Exception("Programmazione Cod non mappata")
                                    End If
                                Case enum_TipoRicetta.PianoDistribuzioneConcimi
                                    Dim recodeProgrammazione = (From g2gPC In GiasContext.G2G_Recode_PianoConcimazione_Testata Where g2gPC.From_PivaSuperUser = s.Ricetta_SuperUser _
                                                                                                      And g2gPC.To_PivaSuperUser = Destinazione_Piva_SuperUser _
                                                                                                      And g2gPC.From_PC_Testata_Cod = s.Programmazione_Cod).FirstOrDefault
                                    If recodeProgrammazione IsNot Nothing Then
                                        to_programmazione_cod = recodeProgrammazione.To_PC_Testata_Cod
                                    Else
                                        Throw New Exception("Recode PianoConcimazione_Testata in Ricetta_Testata non trovato")
                                    End If

                                Case enum_TipoRicetta.PianoDistribuzionePua
                                    Dim recodePua = (From g2gPua In GiasContext.G2G_Recode_Pua Where g2gPua.From_PivaSuperUser = s.Ricetta_SuperUser _
                                                                                 And g2gPua.To_PivaSuperUser = Destinazione_Piva_SuperUser _
                                                                                 And g2gPua.From_Piva = s.Piva _
                                                                                 And g2gPua.From_Pua_Cod = s.Programmazione_Cod).FirstOrDefault
                                    If recodePua IsNot Nothing Then
                                        to_programmazione_cod = recodePua.To_Pua_Cod
                                    Else
                                        Throw New Exception("Recode PUA in Ricetta_Testata non trovato")
                                    End If
                            End Select
                            ricetta.Programmazione_Cod = to_programmazione_cod
                        End If

                        GiasContext.Ricette.Add(ricetta)

                        Dim recode =
                            New G2G_Recode_Ricette With {
                                .From_PivaSuperUser = Origine_Piva_SuperUser,
                                .To_PivaSuperUser = Destinazione_Piva_SuperUser,
                                .From_Piva = s.Piva,
                                .To_Piva = piva,
                                .From_Ricetta_Cod = s.Ricetta_Cod,
                                .To_Ricetta_Cod = ricetta.Ricetta_Cod,
                                .Username_Creazione = username,
                                .Username_Modifica = username,
                                .Data_Creazione = Now(),
                                .Data_Modifica = Now(),
                                .Validita_Inizio = AGRODATAINIZIO,
                                .Validita_Fine = AGRODATAFINE,
                                .inviato = "0",
                                .datainvio = Now()
                            }
                        g2g.G2G_Ricette_Recode_insert.Add(recode)
                        GiasContext.G2G_Recode_Ricette.Add(recode)

                    Next

                    GiasContext.SaveChanges()

                    For Each s As Ricette_Operazioni In g2g.ricette_operazioni_insert

                        Dim idSeq As Integer = objSequenze.NuovoId_Tabella("ricette_operazioni", 0, 2000000000, objParametri, UtilizzaTransazione:=False)
                        Dim ricetta_operazione = Gias_EF_Utility.CopyEntity(GiasContext, s, Nothing, username, data)

                        ricetta_operazione.Ricetta_Operazione_Cod = idSeq
                        ricetta_operazione.Ricetta_SuperUser = Destinazione_Piva_SuperUser

                        ricetta_operazione.Ricetta_Cod = (From recode_ricetta In GiasContext.G2G_Recode_Ricette Where recode_ricetta.From_PivaSuperUser = Origine_Piva_SuperUser _
                                                                                                             And recode_ricetta.To_PivaSuperUser = Destinazione_Piva_SuperUser _
                                                                                                             And recode_ricetta.From_Piva = from_piva _
                                                                                                             And recode_ricetta.To_Piva = piva _
                                                                                                             And recode_ricetta.From_Ricetta_Cod = s.Ricetta_Cod).FirstOrDefault.To_Ricetta_Cod

                        If ricetta_operazione.Ricetta_Operazione_Cod_RIF IsNot Nothing AndAlso ricetta_operazione.Ricetta_Operazione_Cod_RIF <> 0 Then

                            ricetta_operazione.Ricetta_Operazione_Cod_RIF = (From recode_operazione In GiasContext.G2G_Recode_Ricette_Operazioni Where recode_operazione.From_PivaSuperUser = Origine_Piva_SuperUser And recode_operazione.To_PivaSuperUser = Destinazione_Piva_SuperUser _
                                                                                                                                                         And recode_operazione.From_Ricetta_Operazione_Cod = s.Ricetta_Operazione_Cod_RIF).FirstOrDefault.To_Ricetta_Operazione_Cod

                        End If

                        GiasContext.Ricette_Operazioni.Add(ricetta_operazione)

                        Dim recode =
                                New G2G_Recode_Ricette_Operazioni With {
                                    .From_PivaSuperUser = Origine_Piva_SuperUser,
                                    .To_PivaSuperUser = Destinazione_Piva_SuperUser,
                                    .From_Piva = from_piva,
                                    .To_Piva = piva,
                                    .From_Ricetta_Cod = s.Ricetta_Cod,
                                    .To_Ricetta_Cod = ricetta_operazione.Ricetta_Cod,
                                    .From_Ricetta_Operazione_Cod = s.Ricetta_Operazione_Cod,
                                    .To_Ricetta_Operazione_Cod = ricetta_operazione.Ricetta_Operazione_Cod,
                                    .Username_Creazione = username,
                                    .Username_Modifica = username,
                                    .Data_Creazione = Now(),
                                    .Data_Modifica = Now(),
                                    .Validita_Inizio = AGRODATAINIZIO,
                                    .Validita_Fine = AGRODATAFINE,
                                    .inviato = "0",
                                    .datainvio = Now()
                                }
                        g2g.G2G_Ricette_Operazioni_Recode_insert.Add(recode)
                        GiasContext.G2G_Recode_Ricette_Operazioni.Add(recode)

                    Next
                    GiasContext.SaveChanges()

                    For Each s As Ricette_Dettagli In g2g.ricette_dettagli_insert

                        Dim ricetta_dettaglio = Gias_EF_Utility.CopyEntity(GiasContext, s, Nothing, username, data)

                        Dim idSeq As Integer = objSequenze.NuovoId_Tabella("ricette_dettagli", 0, 2000000000, objParametri, UtilizzaTransazione:=False)
                        ricetta_dettaglio.Ricetta_Dettaglio_Cod = idSeq

                        ricetta_dettaglio.Ricetta_SuperUser = Destinazione_Piva_SuperUser

                        ricetta_dettaglio.Ricetta_Cod = (From recode_ricetta In GiasContext.G2G_Recode_Ricette Where recode_ricetta.From_PivaSuperUser = Origine_Piva_SuperUser _
                                                                                                             And recode_ricetta.To_PivaSuperUser = Destinazione_Piva_SuperUser _
                                                                                                             And recode_ricetta.From_Piva = from_piva _
                                                                                                             And recode_ricetta.To_Piva = piva _
                                                                                                             And recode_ricetta.From_Ricetta_Cod = s.Ricetta_Cod).FirstOrDefault.To_Ricetta_Cod

                        ricetta_dettaglio.Ricetta_Operazione_Cod = (From recode_operazione In GiasContext.G2G_Recode_Ricette_Operazioni Where recode_operazione.From_PivaSuperUser = Origine_Piva_SuperUser _
                                                                                                             And recode_operazione.To_PivaSuperUser = Destinazione_Piva_SuperUser _
                                                                                                             And recode_operazione.From_Piva = from_piva _
                                                                                                             And recode_operazione.To_Piva = piva _
                                                                                                             And recode_operazione.From_Ricetta_Cod = s.Ricetta_Cod _
                                                                                                             And recode_operazione.From_Ricetta_Operazione_Cod = s.Ricetta_Operazione_Cod).FirstOrDefault.To_Ricetta_Operazione_Cod

                        GiasContext.Ricette_Dettagli.Add(ricetta_dettaglio)

                        Dim recode =
                                New G2G_Recode_Ricette_Dettagli With {
                                    .From_PivaSuperUser = Origine_Piva_SuperUser,
                                    .To_PivaSuperUser = Destinazione_Piva_SuperUser,
                                    .From_Piva = from_piva,
                                    .To_Piva = piva,
                                    .From_Ricetta_Cod = s.Ricetta_Cod,
                                    .To_Ricetta_Cod = ricetta_dettaglio.Ricetta_Cod,
                                    .From_Ricetta_Operazione_Cod = s.Ricetta_Operazione_Cod,
                                    .To_Ricetta_Operazione_Cod = ricetta_dettaglio.Ricetta_Operazione_Cod,
                                    .From_Ricetta_Dettaglio_Cod = s.Ricetta_Dettaglio_Cod,
                                    .To_Ricetta_Dettaglio_Cod = ricetta_dettaglio.Ricetta_Dettaglio_Cod,
                                    .Username_Creazione = username,
                                    .Username_Modifica = username,
                                    .Data_Creazione = Now(),
                                    .Data_Modifica = Now(),
                                    .Validita_Inizio = AGRODATAINIZIO,
                                    .Validita_Fine = AGRODATAFINE,
                                    .inviato = "0",
                                    .datainvio = Now()
                                }
                        g2g.G2G_Ricette_Dettagli_Recode_insert.Add(recode)
                        GiasContext.G2G_Recode_Ricette_Dettagli.Add(recode)

                    Next
                    GiasContext.SaveChanges()


                    For Each s As Ricette_Dettaglio_Tecnico In g2g.ricette_dettaglio_tecnico_insert

                        Dim ricetta_dettaglio_tecnico = Gias_EF_Utility.CopyEntity(GiasContext, s, Nothing, username, data)

                        Dim idSeq As Integer = objSequenze.NuovoId_Tabella("ricette_dettaglio_tecnico", 0, 2000000000, objParametri, UtilizzaTransazione:=False)
                        ricetta_dettaglio_tecnico.Ricetta_Tecnico_Cod = idSeq

                        ricetta_dettaglio_tecnico.Ricetta_SuperUser = Destinazione_Piva_SuperUser

                        ricetta_dettaglio_tecnico.Ricetta_Cod = (From recode_ricetta In GiasContext.G2G_Recode_Ricette Where recode_ricetta.From_PivaSuperUser = Origine_Piva_SuperUser _
                                                                                                                 And recode_ricetta.To_PivaSuperUser = Destinazione_Piva_SuperUser _
                                                                                                                 And recode_ricetta.From_Piva = from_piva _
                                                                                                                 And recode_ricetta.To_Piva = piva _
                                                                                                                 And recode_ricetta.From_Ricetta_Cod = s.Ricetta_Cod).FirstOrDefault.To_Ricetta_Cod

                        ricetta_dettaglio_tecnico.Ricetta_Operazione_Cod = (From recode_operazione In GiasContext.G2G_Recode_Ricette_Operazioni Where recode_operazione.From_PivaSuperUser = Origine_Piva_SuperUser _
                                                                                                                 And recode_operazione.To_PivaSuperUser = Destinazione_Piva_SuperUser _
                                                                                                                 And recode_operazione.From_Piva = from_piva _
                                                                                                                 And recode_operazione.To_Piva = piva _
                                                                                                                 And recode_operazione.From_Ricetta_Cod = s.Ricetta_Cod _
                                                                                                                 And recode_operazione.From_Ricetta_Operazione_Cod = s.Ricetta_Operazione_Cod).FirstOrDefault.To_Ricetta_Operazione_Cod

                        ricetta_dettaglio_tecnico.Ricetta_Dettaglio_Cod = (From recode_dettaglio In GiasContext.G2G_Recode_Ricette_Dettagli Where recode_dettaglio.From_PivaSuperUser = Origine_Piva_SuperUser _
                                                                                                                 And recode_dettaglio.To_PivaSuperUser = Destinazione_Piva_SuperUser _
                                                                                                                 And recode_dettaglio.From_Piva = from_piva _
                                                                                                                 And recode_dettaglio.To_Piva = piva _
                                                                                                                 And recode_dettaglio.From_Ricetta_Cod = s.Ricetta_Cod _
                                                                                                                 And recode_dettaglio.From_Ricetta_Operazione_Cod = s.Ricetta_Operazione_Cod _
                                                                                                                 And recode_dettaglio.From_Ricetta_Dettaglio_Cod = s.Ricetta_Dettaglio_Cod).FirstOrDefault.To_Ricetta_Dettaglio_Cod

                        GiasContext.Ricette_Dettaglio_Tecnico.Add(ricetta_dettaglio_tecnico)

                        Dim recode =
                                    New G2G_Recode_Ricette_Dettaglio_Tecnico With {
                                        .From_PivaSuperUser = Origine_Piva_SuperUser,
                                        .To_PivaSuperUser = Destinazione_Piva_SuperUser,
                                        .From_Piva = from_piva,
                                        .To_Piva = piva,
                                        .From_Ricetta_Cod = s.Ricetta_Cod,
                                        .To_Ricetta_Cod = ricetta_dettaglio_tecnico.Ricetta_Cod,
                                        .From_Ricetta_Operazione_Cod = s.Ricetta_Operazione_Cod,
                                        .To_Ricetta_Operazione_Cod = ricetta_dettaglio_tecnico.Ricetta_Operazione_Cod,
                                        .From_Ricetta_Dettaglio_Cod = s.Ricetta_Dettaglio_Cod,
                                        .To_Ricetta_Dettaglio_Cod = ricetta_dettaglio_tecnico.Ricetta_Dettaglio_Cod,
                                        .From_Ricetta_Tecnico_Cod = s.Ricetta_Tecnico_Cod,
                                        .To_Ricetta_Tecnico_Cod = ricetta_dettaglio_tecnico.Ricetta_Tecnico_Cod,
                                        .Username_Creazione = username,
                                        .Username_Modifica = username,
                                        .Data_Creazione = Now(),
                                        .Data_Modifica = Now(),
                                        .Validita_Inizio = AGRODATAINIZIO,
                                        .Validita_Fine = AGRODATAFINE,
                                        .inviato = "0",
                                        .datainvio = Now()
                                    }
                        g2g.G2G_Ricette_Dettaglio_Tecnico_Recode_insert.Add(recode)
                        GiasContext.G2G_Recode_Ricette_Dettaglio_Tecnico.Add(recode)

                    Next
                    GiasContext.SaveChanges()

                    For Each s As Ricette_Destinazioni In g2g.ricette_destinazioni_insert

                        Dim ricetta_destinazione = Gias_EF_Utility.CopyEntity(GiasContext, s, Nothing, username, data)

                        Dim idSeq As Integer = objSequenze.NuovoId_Tabella("ricette_destinazioni", 0, 2000000000, objParametri, UtilizzaTransazione:=False)
                        ricetta_destinazione.Ricetta_Destinazione_Cod = idSeq

                        ricetta_destinazione.Ricetta_SuperUser = Destinazione_Piva_SuperUser

                        ricetta_destinazione.Ricetta_Cod = (From recode_ricetta In GiasContext.G2G_Recode_Ricette Where recode_ricetta.From_PivaSuperUser = Origine_Piva_SuperUser _
                                                                                                             And recode_ricetta.To_PivaSuperUser = Destinazione_Piva_SuperUser _
                                                                                                             And recode_ricetta.From_Piva = from_piva _
                                                                                                             And recode_ricetta.To_Piva = piva _
                                                                                                             And recode_ricetta.From_Ricetta_Cod = s.Ricetta_Cod).FirstOrDefault.To_Ricetta_Cod

                        ricetta_destinazione.Ricetta_Operazione_Cod = (From recode_operazione In GiasContext.G2G_Recode_Ricette_Operazioni Where recode_operazione.From_PivaSuperUser = Origine_Piva_SuperUser _
                                                                                                             And recode_operazione.To_PivaSuperUser = Destinazione_Piva_SuperUser _
                                                                                                             And recode_operazione.From_Piva = from_piva _
                                                                                                             And recode_operazione.To_Piva = piva _
                                                                                                             And recode_operazione.From_Ricetta_Cod = s.Ricetta_Cod _
                                                                                                             And recode_operazione.From_Ricetta_Operazione_Cod = s.Ricetta_Operazione_Cod).FirstOrDefault.To_Ricetta_Operazione_Cod

                        ricetta_destinazione.Ricetta_Dettaglio_Cod = (From recode_dettaglio In GiasContext.G2G_Recode_Ricette_Dettagli Where recode_dettaglio.From_PivaSuperUser = Origine_Piva_SuperUser _
                                                                                                             And recode_dettaglio.To_PivaSuperUser = Destinazione_Piva_SuperUser _
                                                                                                             And recode_dettaglio.From_Piva = from_piva _
                                                                                                             And recode_dettaglio.To_Piva = piva _
                                                                                                             And recode_dettaglio.From_Ricetta_Cod = s.Ricetta_Cod _
                                                                                                             And recode_dettaglio.From_Ricetta_Operazione_Cod = s.Ricetta_Operazione_Cod _
                                                                                                             And recode_dettaglio.From_Ricetta_Dettaglio_Cod = s.Ricetta_Dettaglio_Cod).FirstOrDefault.To_Ricetta_Dettaglio_Cod

                        GiasContext.Ricette_Destinazioni.Add(ricetta_destinazione)

                        Dim recode =
                                New G2G_Recode_Ricette_Destinazioni With {
                                    .From_PivaSuperUser = Origine_Piva_SuperUser,
                                    .To_PivaSuperUser = Destinazione_Piva_SuperUser,
                                    .From_Piva = from_piva,
                                    .To_Piva = piva,
                                    .From_Ricetta_Cod = s.Ricetta_Cod,
                                    .To_Ricetta_Cod = ricetta_destinazione.Ricetta_Cod,
                                    .From_Ricetta_Operazione_Cod = s.Ricetta_Operazione_Cod,
                                    .To_Ricetta_Operazione_Cod = ricetta_destinazione.Ricetta_Operazione_Cod,
                                    .From_Ricetta_Dettaglio_Cod = s.Ricetta_Dettaglio_Cod,
                                    .To_Ricetta_Dettaglio_Cod = ricetta_destinazione.Ricetta_Dettaglio_Cod,
                                    .From_Ricetta_Destinazione_Cod = s.Ricetta_Destinazione_Cod,
                                    .To_Ricetta_Destinazione_Cod = ricetta_destinazione.Ricetta_Destinazione_Cod,
                                    .Username_Creazione = username,
                                    .Username_Modifica = username,
                                    .Data_Creazione = Now(),
                                    .Data_Modifica = Now(),
                                    .Validita_Inizio = AGRODATAINIZIO,
                                    .Validita_Fine = AGRODATAFINE,
                                    .inviato = "0",
                                    .datainvio = Now()
                                }
                        g2g.G2G_Ricette_Destinazioni_Recode_insert.Add(recode)
                        GiasContext.G2G_Recode_Ricette_Destinazioni.Add(recode)

                    Next
                    GiasContext.SaveChanges()


                    For Each s As RicettexAgenda In g2g.ricettexagenda_insert
                        Dim ricettaxagenda = Gias_EF_Utility.CopyEntity(GiasContext, s, Nothing, username, data)

                        'Richiedo un nuovo id sequenza
                        ricettaxagenda.Ricetta_SuperUser = Destinazione_Piva_SuperUser

                        ricettaxagenda.Ricetta_Cod = (From recode_ricetta In GiasContext.G2G_Recode_Ricette Where recode_ricetta.From_PivaSuperUser = Origine_Piva_SuperUser _
                                                                                                             And recode_ricetta.To_PivaSuperUser = Destinazione_Piva_SuperUser _
                                                                                                             And recode_ricetta.From_Piva = from_piva _
                                                                                                             And recode_ricetta.To_Piva = piva _
                                                                                                             And recode_ricetta.From_Ricetta_Cod = s.Ricetta_Cod).FirstOrDefault.To_Ricetta_Cod

                        ricettaxagenda.Ricetta_Operazione_Cod = (From recode_operazione In GiasContext.G2G_Recode_Ricette_Operazioni Where recode_operazione.From_PivaSuperUser = Origine_Piva_SuperUser _
                                                                                                             And recode_operazione.To_PivaSuperUser = Destinazione_Piva_SuperUser _
                                                                                                             And recode_operazione.From_Piva = from_piva _
                                                                                                             And recode_operazione.To_Piva = piva _
                                                                                                             And recode_operazione.From_Ricetta_Cod = s.Ricetta_Cod _
                                                                                                             And recode_operazione.From_Ricetta_Operazione_Cod = s.Ricetta_Operazione_Cod).FirstOrDefault.To_Ricetta_Operazione_Cod

                        GiasContext.RicettexAgenda.Add(ricettaxagenda)

                    Next
                    GiasContext.SaveChanges()


                    For Each s As RicettexCultivar In g2g.ricettexcultivar_insert

                        Dim ricettaxcultivar = Gias_EF_Utility.CopyEntity(GiasContext, s, Nothing, username, data)

                        'Richiedo un nuovo id sequenza
                        ricettaxcultivar.Ricetta_SuperUser = Destinazione_Piva_SuperUser

                        ricettaxcultivar.Ricetta_Cod = (From recode_ricetta In GiasContext.G2G_Recode_Ricette
                                                        Where recode_ricetta.From_PivaSuperUser = Origine_Piva_SuperUser AndAlso
                                                              recode_ricetta.To_PivaSuperUser = Destinazione_Piva_SuperUser AndAlso
                                                              recode_ricetta.From_Piva = from_piva AndAlso
                                                              recode_ricetta.To_Piva = piva AndAlso
                                                              recode_ricetta.From_Ricetta_Cod = s.Ricetta_Cod).FirstOrDefault.To_Ricetta_Cod

                        GiasContext.RicettexCultivar.Add(ricettaxcultivar)

                    Next
                    GiasContext.SaveChanges()


                    For Each s As RicettexNote In g2g.ricettexnote_insert

                        Dim ricettaxnote = Gias_EF_Utility.CopyEntity(GiasContext, s, Nothing, username, data)

                        'Richiedo un nuovo id sequenza
                        ricettaxnote.Ricetta_SuperUser = Destinazione_Piva_SuperUser

                        ricettaxnote.Ricetta_Cod = (From recode_ricetta In GiasContext.G2G_Recode_Ricette Where recode_ricetta.From_PivaSuperUser = Origine_Piva_SuperUser _
                                                                                                             And recode_ricetta.To_PivaSuperUser = Destinazione_Piva_SuperUser _
                                                                                                             And recode_ricetta.From_Piva = from_piva _
                                                                                                             And recode_ricetta.To_Piva = piva _
                                                                                                             And recode_ricetta.From_Ricetta_Cod = s.Ricetta_Cod).FirstOrDefault.To_Ricetta_Cod

                        ricettaxnote.Ricetta_Operazione_Cod = (From recode_operazione In GiasContext.G2G_Recode_Ricette_Operazioni
                                                               Where recode_operazione.From_PivaSuperUser = Origine_Piva_SuperUser AndAlso
                                                                     recode_operazione.To_PivaSuperUser = Destinazione_Piva_SuperUser AndAlso
                                                                     recode_operazione.From_Piva = from_piva AndAlso
                                                                     recode_operazione.To_Piva = piva AndAlso
                                                                     recode_operazione.From_Ricetta_Cod = s.Ricetta_Cod AndAlso
                                                                     recode_operazione.From_Ricetta_Operazione_Cod = s.Ricetta_Operazione_Cod).FirstOrDefault.To_Ricetta_Operazione_Cod

                        ricettaxnote.Nota_Cod = (From recode_note In GiasContext.G2G_Recode_NoteIntervento
                                                 Where recode_note.From_PivaSuperUser = Origine_Piva_SuperUser AndAlso
                                                       recode_note.To_PivaSuperUser = Destinazione_Piva_SuperUser AndAlso
                                                       recode_note.From_Nota_Cod = s.Nota_Cod).FirstOrDefault.To_Nota_Cod

                        GiasContext.RicettexNote.Add(ricettaxnote)

                    Next
                    GiasContext.SaveChanges()

                    g2g.Recode = New G2G_Recode

                    g2g.Recode.G2GRecodeRicetteToInsert = g2g.G2G_Ricette_Recode_insert
                    g2g.Recode.G2GRecodeRicetteToUpdate = g2g.G2G_Ricette_Recode_update
                    g2g.Recode.G2GRecodeRicetteToDelete = g2g.G2G_Ricette_Recode_delete

                    g2g.Recode.G2GRecodeRicette_OperazioniToInsert = g2g.G2G_Ricette_Operazioni_Recode_insert
                    g2g.Recode.G2GRecodeRicette_OperazioniToUpdate = g2g.G2G_Ricette_Operazioni_Recode_update
                    g2g.Recode.G2GRecodeRicette_OperazioniToDelete = g2g.G2G_Ricette_Operazioni_Recode_delete

                    g2g.Recode.G2GRecodeRicette_DettagliToInsert = g2g.G2G_Ricette_Dettagli_Recode_insert
                    g2g.Recode.G2GRecodeRicette_DettagliToUpdate = g2g.G2G_Ricette_Dettagli_Recode_update
                    g2g.Recode.G2GRecodeRicette_DettagliToDelete = g2g.G2G_Ricette_Dettagli_Recode_delete

                    g2g.Recode.G2GRecodeRicette_Dettaglio_TecnicoToInsert = g2g.G2G_Ricette_Dettaglio_Tecnico_Recode_insert
                    g2g.Recode.G2GRecodeRicette_Dettaglio_TecnicoToUpdate = g2g.G2G_Ricette_Dettaglio_Tecnico_Recode_update
                    g2g.Recode.G2GRecodeRicette_Dettaglio_TecnicoToDelete = g2g.G2G_Ricette_Dettaglio_Tecnico_Recode_delete

                    g2g.Recode.G2GRecodeRicette_DestinazioniToInsert = g2g.G2G_Ricette_Destinazioni_Recode_insert
                    g2g.Recode.G2GRecodeRicette_DestinazioniToUpdate = g2g.G2G_Ricette_Destinazioni_Recode_update
                    g2g.Recode.G2GRecodeRicette_DestinazioniToDelete = g2g.G2G_Ricette_Destinazioni_Recode_delete

                    scope.Complete()

                End Using
            End Using

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore, ex)

        End Try

        Return g2g.Recode

    End Function

    Public Function Scrivi_Ricette_G2G_Reverse(Origine_Piva_SuperUser As String, Destinazione_Piva_SuperUser As String, ByRef g2g As G2G_Ricette_Reverse, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As G2G_Recode

        Dim nomeRoutine As String = "AgronicaCoreG2GLocalDal.G2GRicette_W.Scrivi_Ricette_G2G_Reverse()"
        Dim messaggioErrore As String = ""


        Dim username = objParametri.UsernameOperazione
        Dim piva = g2g.To_Piva
        Dim from_piva = g2g.From_Piva
        Dim data = Date.Now

        Dim objSequenze = New AgronicaCoreDataProvider.Agro_Sequenze
        Dim retries As Integer = 3
        Dim success As Boolean = True

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Dim transactionOptions As New TransactionOptions()
        transactionOptions.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted
        transactionOptions.Timeout = G2GUtility.GetTransactionTimeout(objParametri)

        Try

            Using scope As New TransactionScope(TransactionScopeOption.Required, transactionOptions)

                Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

                    '
                    'DELETE
                    '


                    For Each s As RicettexAgenda In g2g.ricettexagenda_delete

                        Dim ricettaxagenda = Gias_EF_Utility.CopyEntity(GiasContext, s, Nothing, username, data)

                        ricettaxagenda.Ricetta_SuperUser = Destinazione_Piva_SuperUser

                        ricettaxagenda.Ricetta_Cod = (From t In GiasContext.G2G_Recode_Ricette
                                                      Where t.To_PivaSuperUser = Origine_Piva_SuperUser _
                                                           And t.From_PivaSuperUser = Destinazione_Piva_SuperUser _
                                                           And t.From_Piva = piva _
                                                           And t.To_Ricetta_Cod = s.Ricetta_Cod).FirstOrDefault.To_Ricetta_Cod

                        ricettaxagenda.Ricetta_Operazione_Cod = (From t In GiasContext.G2G_Recode_Ricette_Operazioni
                                                                 Where t.To_PivaSuperUser = Origine_Piva_SuperUser _
                                                                   And t.From_PivaSuperUser = Destinazione_Piva_SuperUser _
                                                                   And t.From_Piva = piva _
                                                                   And t.To_Ricetta_Cod = s.Ricetta_Cod _
                                                                   And t.To_Ricetta_Operazione_Cod = s.Ricetta_Operazione_Cod).FirstOrDefault.To_Ricetta_Operazione_Cod

                        GiasContext.RicettexAgenda.Attach(ricettaxagenda)
                        GiasContext.RicettexAgenda.Remove(ricettaxagenda)

                    Next
                    GiasContext.SaveChanges()

                    For Each s As RicettexCultivar In g2g.ricettexcultivar_delete

                        Dim ricettaxcultivar = Gias_EF_Utility.CopyEntity(GiasContext, s, Nothing, username, data)

                        ricettaxcultivar.Ricetta_Cod = (From t In GiasContext.G2G_Recode_Ricette
                                                        Where t.To_PivaSuperUser = Origine_Piva_SuperUser _
                                                           And t.From_PivaSuperUser = Destinazione_Piva_SuperUser _
                                                           And t.From_Piva = piva _
                                                           And t.To_Ricetta_Cod = s.Ricetta_Cod).FirstOrDefault.To_Ricetta_Cod

                        ricettaxcultivar.Ricetta_SuperUser = Destinazione_Piva_SuperUser

                        GiasContext.RicettexCultivar.Attach(ricettaxcultivar)
                        GiasContext.RicettexCultivar.Remove(ricettaxcultivar)

                    Next
                    GiasContext.SaveChanges()

                    For Each s As RicettexNote In g2g.ricettexnote_delete

                        Dim ricettaxnote = Gias_EF_Utility.CopyEntity(GiasContext, s, Nothing, username, data)

                        ricettaxnote.Ricetta_SuperUser = Destinazione_Piva_SuperUser

                        ricettaxnote.Ricetta_Cod = (From t In GiasContext.G2G_Recode_Ricette
                                                    Where t.To_PivaSuperUser = Origine_Piva_SuperUser _
                                                           And t.From_PivaSuperUser = Destinazione_Piva_SuperUser _
                                                           And t.From_Piva = piva _
                                                           And t.To_Ricetta_Cod = s.Ricetta_Cod).FirstOrDefault.From_Ricetta_Cod

                        ricettaxnote.Ricetta_Operazione_Cod = (From t In GiasContext.G2G_Recode_Ricette_Operazioni
                                                               Where t.To_PivaSuperUser = Origine_Piva_SuperUser _
                                                           And t.From_PivaSuperUser = Destinazione_Piva_SuperUser _
                                                           And t.From_Piva = piva _
                                                           And t.To_Ricetta_Cod = s.Ricetta_Cod _
                                                           And t.To_Ricetta_Operazione_Cod = s.Ricetta_Operazione_Cod).FirstOrDefault.From_Ricetta_Operazione_Cod

                        ricettaxnote.Nota_Cod = (From t In GiasContext.G2G_Recode_NoteIntervento
                                                 Where t.To_PivaSuperUser = Origine_Piva_SuperUser _
                                                           And t.From_PivaSuperUser = Destinazione_Piva_SuperUser _
                                                           And t.To_Nota_Cod = s.Nota_Cod).FirstOrDefault.From_Nota_Cod

                        GiasContext.RicettexNote.Attach(ricettaxnote)
                        GiasContext.RicettexNote.Remove(ricettaxnote)

                    Next
                    GiasContext.SaveChanges()

                    For Each r As G2G_Recode_Ricette_Destinazioni In g2g.G2G_Ricette_Destinazioni_Recode_delete

                        ''cancella destinazione
                        Dim ricette_destinazioni = (From m In GiasContext.Ricette_Destinazioni Where m.Ricetta_SuperUser = Destinazione_Piva_SuperUser _
                                                                                                   And m.Ricetta_Cod = r.From_Ricetta_Cod _
                                                                                                   And m.Ricetta_Operazione_Cod = r.From_Ricetta_Operazione_Cod _
                                                                                                   And m.Ricetta_Dettaglio_Cod = r.From_Ricetta_Dettaglio_Cod _
                                                                                                   And m.Ricetta_Destinazione_Cod = r.From_Ricetta_Destinazione_Cod).FirstOrDefault
                        GiasContext.Ricette_Destinazioni.Attach(ricette_destinazioni)
                        GiasContext.Ricette_Destinazioni.Remove(ricette_destinazioni)


                        '' cancella recode destinazione
                        Dim recode = (From rr In GiasContext.G2G_Recode_Ricette_Destinazioni Where rr.From_PivaSuperUser = Origine_Piva_SuperUser _
                                                                                                 And rr.From_Ricetta_Cod = r.From_Ricetta_Cod _
                                                                                                 And rr.From_Ricetta_Operazione_Cod = r.From_Ricetta_Operazione_Cod _
                                                                                                 And rr.From_Ricetta_Dettaglio_Cod = r.From_Ricetta_Dettaglio_Cod _
                                                                                                 And rr.From_Ricetta_Destinazione_Cod = r.From_Ricetta_Destinazione_Cod _
                                                                                                 And rr.To_PivaSuperUser = Destinazione_Piva_SuperUser _
                                                                                                 And rr.To_Ricetta_Cod = r.To_Ricetta_Cod _
                                                                                                 And rr.To_Ricetta_Operazione_Cod = r.To_Ricetta_Operazione_Cod _
                                                                                                 And rr.To_Ricetta_Dettaglio_Cod = r.To_Ricetta_Dettaglio_Cod _
                                                                                                 And rr.To_Ricetta_Destinazione_Cod = r.To_Ricetta_Destinazione_Cod).FirstOrDefault()
                        GiasContext.G2G_Recode_Ricette_Destinazioni.Attach(recode)
                        GiasContext.G2G_Recode_Ricette_Destinazioni.Remove(recode)

                    Next
                    GiasContext.SaveChanges()

                    For Each r As G2G_Recode_Ricette_Dettaglio_Tecnico In g2g.G2G_Ricette_Dettaglio_Tecnico_Recode_delete

                        ''cancella tecnico
                        Dim ricette_dettaglio = (From m In GiasContext.Ricette_Dettaglio_Tecnico Where m.Ricetta_SuperUser = Destinazione_Piva_SuperUser _
                                                                                                   And m.Ricetta_Cod = r.From_Ricetta_Cod _
                                                                                                   And m.Ricetta_Operazione_Cod = r.From_Ricetta_Operazione_Cod _
                                                                                                   And m.Ricetta_Dettaglio_Cod = r.From_Ricetta_Dettaglio_Cod _
                                                                                                   And m.Ricetta_Tecnico_Cod = r.From_Ricetta_Tecnico_Cod).FirstOrDefault
                        GiasContext.Ricette_Dettaglio_Tecnico.Attach(ricette_dettaglio)
                        GiasContext.Ricette_Dettaglio_Tecnico.Remove(ricette_dettaglio)


                        '' cancella recode tecnico
                        Dim recode = (From rr In GiasContext.G2G_Recode_Ricette_Dettaglio_Tecnico Where rr.From_PivaSuperUser = Origine_Piva_SuperUser _
                                                                                                 And rr.From_Ricetta_Cod = r.From_Ricetta_Cod _
                                                                                                 And rr.From_Ricetta_Operazione_Cod = r.From_Ricetta_Operazione_Cod _
                                                                                                 And rr.From_Ricetta_Dettaglio_Cod = r.From_Ricetta_Dettaglio_Cod _
                                                                                                 And rr.From_Ricetta_Tecnico_Cod = r.From_Ricetta_Tecnico_Cod _
                                                                                                 And rr.To_PivaSuperUser = Destinazione_Piva_SuperUser _
                                                                                                 And rr.To_Ricetta_Cod = r.To_Ricetta_Cod _
                                                                                                 And rr.To_Ricetta_Operazione_Cod = r.To_Ricetta_Operazione_Cod _
                                                                                                 And rr.To_Ricetta_Dettaglio_Cod = r.To_Ricetta_Dettaglio_Cod _
                                                                                                 And rr.To_Ricetta_Tecnico_Cod = r.To_Ricetta_Tecnico_Cod).FirstOrDefault()
                        GiasContext.G2G_Recode_Ricette_Dettaglio_Tecnico.Attach(recode)
                        GiasContext.G2G_Recode_Ricette_Dettaglio_Tecnico.Remove(recode)

                    Next
                    GiasContext.SaveChanges()

                    For Each r As G2G_Recode_Ricette_Dettagli In g2g.G2G_Ricette_Dettagli_Recode_delete

                        ''cancella dettaglio
                        Dim ricette_dettaglio = (From m In GiasContext.Ricette_Dettagli Where m.Ricetta_SuperUser = Destinazione_Piva_SuperUser _
                                                                                                   And m.Ricetta_Cod = r.From_Ricetta_Cod _
                                                                                                   And m.Ricetta_Operazione_Cod = r.From_Ricetta_Operazione_Cod _
                                                                                                   And m.Ricetta_Dettaglio_Cod = r.From_Ricetta_Dettaglio_Cod).FirstOrDefault
                        GiasContext.Ricette_Dettagli.Attach(ricette_dettaglio)
                        GiasContext.Ricette_Dettagli.Remove(ricette_dettaglio)


                        '' cancella recode dettaglio
                        Dim recode = (From rr In GiasContext.G2G_Recode_Ricette_Dettagli Where rr.From_PivaSuperUser = Origine_Piva_SuperUser _
                                                                                                 And rr.From_Ricetta_Cod = r.From_Ricetta_Cod _
                                                                                                 And rr.From_Ricetta_Operazione_Cod = r.From_Ricetta_Operazione_Cod _
                                                                                                 And rr.From_Ricetta_Dettaglio_Cod = r.From_Ricetta_Dettaglio_Cod _
                                                                                                 And rr.To_PivaSuperUser = Destinazione_Piva_SuperUser _
                                                                                                 And rr.To_Ricetta_Cod = r.To_Ricetta_Cod _
                                                                                                 And rr.To_Ricetta_Operazione_Cod = r.To_Ricetta_Operazione_Cod _
                                                                                                 And rr.To_Ricetta_Dettaglio_Cod = r.To_Ricetta_Dettaglio_Cod).FirstOrDefault()
                        GiasContext.G2G_Recode_Ricette_Dettagli.Attach(recode)
                        GiasContext.G2G_Recode_Ricette_Dettagli.Remove(recode)

                    Next
                    GiasContext.SaveChanges()


                    For Each r As G2G_Recode_Ricette_Operazioni In g2g.G2G_Ricette_Operazioni_Recode_delete

                        ''cancella operazione
                        Dim ricette_operazioni = (From m In GiasContext.Ricette_Operazioni Where m.Ricetta_SuperUser = Destinazione_Piva_SuperUser _
                                                                                                   And m.Ricetta_Cod = r.From_Ricetta_Cod _
                                                                                                   And m.Ricetta_Operazione_Cod = r.From_Ricetta_Operazione_Cod).FirstOrDefault
                        GiasContext.Ricette_Operazioni.Attach(ricette_operazioni)
                        GiasContext.Ricette_Operazioni.Remove(ricette_operazioni)

                        Dim ricettexagenda = (From ra In GiasContext.RicettexAgenda Where ra.Ricetta_Cod = ricette_operazioni.Ricetta_Cod _
                                                                                        And ra.Ricetta_Operazione_Cod = ricette_operazioni.Ricetta_Operazione_Cod _
                                                                                        And ra.Ricetta_SuperUser = ricette_operazioni.Ricetta_SuperUser).ToList
                        For Each ricettaxagenda In ricettexagenda
                            GiasContext.RicettexAgenda.Attach(ricettaxagenda)
                            GiasContext.RicettexAgenda.Remove(ricettaxagenda)
                        Next

                        Dim ricettexnote = (From ra In GiasContext.RicettexNote Where ra.Ricetta_Cod = ricette_operazioni.Ricetta_Cod _
                                                                                        And ra.Ricetta_Operazione_Cod = ricette_operazioni.Ricetta_Operazione_Cod _
                                                                                        And ra.Ricetta_SuperUser = ricette_operazioni.Ricetta_SuperUser).ToList
                        For Each ricettaxnote In ricettexnote
                            GiasContext.RicettexNote.Attach(ricettaxnote)
                            GiasContext.RicettexNote.Remove(ricettaxnote)
                        Next

                        '' cancella recode operazione
                        Dim recode = (From rr In GiasContext.G2G_Recode_Ricette_Operazioni Where rr.From_PivaSuperUser = Origine_Piva_SuperUser _
                                                                                                 And rr.From_Ricetta_Cod = r.From_Ricetta_Cod _
                                                                                                 And rr.From_Ricetta_Operazione_Cod = r.From_Ricetta_Operazione_Cod _
                                                                                                 And rr.To_PivaSuperUser = Destinazione_Piva_SuperUser _
                                                                                                 And rr.To_Ricetta_Cod = r.To_Ricetta_Cod _
                                                                                                 And rr.To_Ricetta_Operazione_Cod = r.To_Ricetta_Operazione_Cod).FirstOrDefault()
                        GiasContext.G2G_Recode_Ricette_Operazioni.Attach(recode)
                        GiasContext.G2G_Recode_Ricette_Operazioni.Remove(recode)


                    Next
                    GiasContext.SaveChanges()



                    For Each r As G2G_Recode_Ricette In g2g.G2G_Ricette_Recode_delete

                        ''cancella ricetta
                        Dim ricette = (From m In GiasContext.Ricette Where m.Ricetta_SuperUser = Destinazione_Piva_SuperUser _
                                                                                                   And m.Ricetta_Cod = r.From_Ricetta_Cod).FirstOrDefault
                        GiasContext.Ricette.Attach(ricette)
                        GiasContext.Ricette.Remove(ricette)

                        Dim ricettexcultivar = (From ra In GiasContext.RicettexCultivar Where ra.Ricetta_Cod = ricette.Ricetta_Cod _
                                                                                        And ra.Ricetta_SuperUser = ricette.Ricetta_SuperUser).ToList
                        For Each ricettaxcultivar In ricettexcultivar
                            GiasContext.RicettexCultivar.Attach(ricettaxcultivar)
                            GiasContext.RicettexCultivar.Remove(ricettaxcultivar)
                        Next

                        '' cancella recode operazione
                        Dim recode = (From rr In GiasContext.G2G_Recode_Ricette Where rr.From_PivaSuperUser = Origine_Piva_SuperUser _
                                                                                                 And rr.From_Ricetta_Cod = r.From_Ricetta_Cod _
                                                                                                 And rr.To_PivaSuperUser = Destinazione_Piva_SuperUser _
                                                                                                 And rr.To_Ricetta_Cod = r.To_Ricetta_Cod).FirstOrDefault()
                        GiasContext.G2G_Recode_Ricette.Attach(recode)
                        GiasContext.G2G_Recode_Ricette.Remove(recode)

                    Next

                    GiasContext.SaveChanges()


                    '
                    'UPDATE
                    '
                    For Each m As Ricette In g2g.ricette_update

                        Dim recode = (From rr In GiasContext.G2G_Recode_Ricette Where rr.To_PivaSuperUser = Origine_Piva_SuperUser _
                                                                                               And rr.From_PivaSuperUser = Destinazione_Piva_SuperUser _
                                                                                               And rr.To_Ricetta_Cod = m.Ricetta_Cod).FirstOrDefault()
                        recode.Username_Modifica = username
                        recode.Data_Modifica = data
                        recode.datainvio = data
                        g2g.G2G_Ricette_Recode_update.Add(recode)
                        GiasContext.G2G_Recode_Ricette.Attach(recode)
                        GiasContext.Entry(recode).State = EntityState.Modified

                        Dim ricetta = (From mm In GiasContext.Ricette Where mm.Ricetta_SuperUser = recode.From_PivaSuperUser _
                                                                          And mm.Ricetta_Cod = recode.From_Ricetta_Cod).FirstOrDefault()
                        ricetta = Gias_EF_Utility.CopyEntity(GiasContext, m, ricetta, username, data)
                        ricetta.Ricetta_Cod = recode.From_Ricetta_Cod
                        ricetta.Ricetta_SuperUser = Destinazione_Piva_SuperUser
                        ricetta.Piva = piva

                        Dim to_programmazione_cod As Integer = 0
                        If ricetta.Programmazione_Cod <> 0 Then

                            Select Case ricetta.Tipo_Ricetta
                                Case enum_TipoRicetta.PUA
                                    to_programmazione_cod = (From g2gPua In GiasContext.G2G_Recode_Programmazione_Testata Where g2gPua.To_PivaSuperUser = m.Ricetta_SuperUser _
                                                                                 And g2gPua.From_PivaSuperUser = Destinazione_Piva_SuperUser _
                                                                                 And g2gPua.To_Piva = m.Piva _
                                                                                 And g2gPua.To_Programmazione_Cod = m.Programmazione_Cod).FirstOrDefault.From_Programmazione_Cod
                                Case enum_TipoRicetta.Standard_Destinazioni
                                    If ricetta.Programmazione_Cod <> 0 Then
                                        Throw New Exception("Programmazione Cod non mappata")
                                    End If
                                Case enum_TipoRicetta.PianoDistribuzioneConcimi
                                    to_programmazione_cod = (From g2gPC In GiasContext.G2G_Recode_PianoConcimazione_Testata Where g2gPC.To_PivaSuperUser = m.Ricetta_SuperUser _
                                                                                                      And g2gPC.From_PivaSuperUser = Destinazione_Piva_SuperUser _
                                                                                                      And g2gPC.To_PC_Testata_Cod = m.Programmazione_Cod).FirstOrDefault.From_PC_Testata_Cod
                                Case enum_TipoRicetta.PianoDistribuzionePua
                                    to_programmazione_cod = (From g2gPua In GiasContext.G2G_Recode_Pua Where g2gPua.To_PivaSuperUser = m.Ricetta_SuperUser _
                                                                                 And g2gPua.From_PivaSuperUser = Destinazione_Piva_SuperUser _
                                                                                 And g2gPua.To_Piva = m.Piva _
                                                                                 And g2gPua.To_Pua_Cod = m.Programmazione_Cod).FirstOrDefault.From_Pua_Cod
                            End Select
                            ricetta.Programmazione_Cod = to_programmazione_cod
                        End If


                        Dim ricettexcultivar_s = (From rc In GiasContext.RicettexCultivar Where rc.Ricetta_SuperUser = Destinazione_Piva_SuperUser And rc.Ricetta_Cod = ricetta.Ricetta_Cod).ToList
                        For Each ricettexcultivar In ricettexcultivar_s
                            GiasContext.RicettexCultivar.Attach(ricettexcultivar)
                            GiasContext.RicettexCultivar.Remove(ricettexcultivar)
                        Next

                        GiasContext.Ricette.Attach(ricetta)
                        GiasContext.Entry(ricetta).State = EntityState.Modified

                    Next
                    GiasContext.SaveChanges()

                    For Each m As Ricette_Operazioni In g2g.ricette_operazioni_update

                        Dim recode = (From rr In GiasContext.G2G_Recode_Ricette_Operazioni Where rr.To_PivaSuperUser = Origine_Piva_SuperUser _
                                                                                               And rr.From_PivaSuperUser = Destinazione_Piva_SuperUser _
                                                                                               And rr.To_Ricetta_Cod = m.Ricetta_Cod _
                                                                                               And rr.To_Ricetta_Operazione_Cod = m.Ricetta_Operazione_Cod).FirstOrDefault()
                        recode.Username_Modifica = username
                        recode.Data_Modifica = data
                        recode.datainvio = data
                        g2g.G2G_Ricette_Operazioni_Recode_update.Add(recode)
                        GiasContext.G2G_Recode_Ricette_Operazioni.Attach(recode)
                        GiasContext.Entry(recode).State = EntityState.Modified

                        Dim ricetta_operazione = (From mm In GiasContext.Ricette_Operazioni Where mm.Ricetta_SuperUser = recode.From_PivaSuperUser _
                                                                          And mm.Ricetta_Cod = recode.From_Ricetta_Cod _
                                                                          And mm.Ricetta_Operazione_Cod = recode.From_Ricetta_Operazione_Cod).FirstOrDefault()
                        ricetta_operazione = Gias_EF_Utility.CopyEntity(GiasContext, m, ricetta_operazione, username, data)
                        ricetta_operazione.Ricetta_Cod = recode.From_Ricetta_Cod
                        ricetta_operazione.Ricetta_Operazione_Cod = recode.From_Ricetta_Operazione_Cod
                        ricetta_operazione.Ricetta_SuperUser = Destinazione_Piva_SuperUser

                        If ricetta_operazione.Ricetta_Operazione_Cod_RIF IsNot Nothing AndAlso ricetta_operazione.Ricetta_Operazione_Cod_RIF <> 0 Then

                            ricetta_operazione.Ricetta_Operazione_Cod_RIF = (From recode_operazione In GiasContext.G2G_Recode_Ricette_Operazioni Where recode_operazione.To_PivaSuperUser = Origine_Piva_SuperUser And recode_operazione.From_PivaSuperUser = Destinazione_Piva_SuperUser _
                                                                                                                                                     And recode_operazione.To_Ricetta_Operazione_Cod = m.Ricetta_Operazione_Cod_RIF).FirstOrDefault.From_Ricetta_Operazione_Cod

                        End If


                        Dim ricettexagendas_s = (From rc In GiasContext.RicettexAgenda Where rc.Ricetta_SuperUser = Destinazione_Piva_SuperUser And rc.Ricetta_Cod = ricetta_operazione.Ricetta_Cod And rc.Ricetta_Operazione_Cod = ricetta_operazione.Ricetta_Operazione_Cod).ToList
                        For Each ricettexagenda In ricettexagendas_s
                            GiasContext.RicettexAgenda.Attach(ricettexagenda)
                            GiasContext.RicettexAgenda.Remove(ricettexagenda)
                        Next

                        Dim ricettexnote_s = (From rc In GiasContext.RicettexNote Where rc.Ricetta_SuperUser = Destinazione_Piva_SuperUser And rc.Ricetta_Cod = ricetta_operazione.Ricetta_Cod And rc.Ricetta_Operazione_Cod = ricetta_operazione.Ricetta_Operazione_Cod).ToList
                        For Each ricettexnote In ricettexnote_s
                            GiasContext.RicettexNote.Attach(ricettexnote)
                            GiasContext.RicettexNote.Remove(ricettexnote)
                        Next

                        GiasContext.Ricette_Operazioni.Attach(ricetta_operazione)
                        GiasContext.Entry(ricetta_operazione).State = EntityState.Modified

                    Next
                    GiasContext.SaveChanges()


                    For Each m As Ricette_Dettagli In g2g.ricette_dettagli_update

                        Dim recode = (From rr In GiasContext.G2G_Recode_Ricette_Dettagli Where rr.To_PivaSuperUser = Origine_Piva_SuperUser _
                                                                                           And rr.From_PivaSuperUser = Destinazione_Piva_SuperUser _
                                                                                           And rr.To_Ricetta_Cod = m.Ricetta_Cod _
                                                                                           And rr.To_Ricetta_Operazione_Cod = m.Ricetta_Operazione_Cod _
                                                                                           And rr.To_Ricetta_Dettaglio_Cod = m.Ricetta_Dettaglio_Cod).FirstOrDefault()
                        recode.Username_Modifica = username
                        recode.Data_Modifica = data
                        recode.datainvio = data
                        g2g.G2G_Ricette_Dettagli_Recode_update.Add(recode)
                        GiasContext.G2G_Recode_Ricette_Dettagli.Attach(recode)
                        GiasContext.Entry(recode).State = EntityState.Modified

                        Dim ricetta_dettaglio = (From mm In GiasContext.Ricette_Dettagli Where mm.Ricetta_SuperUser = recode.From_PivaSuperUser _
                                                                          And mm.Ricetta_Cod = recode.From_Ricetta_Cod _
                                                                          And mm.Ricetta_Operazione_Cod = recode.From_Ricetta_Operazione_Cod _
                                                                          And mm.Ricetta_Dettaglio_Cod = recode.From_Ricetta_Dettaglio_Cod).FirstOrDefault()
                        ricetta_dettaglio = Gias_EF_Utility.CopyEntity(GiasContext, m, ricetta_dettaglio, username, data)
                        ricetta_dettaglio.Ricetta_Cod = recode.From_Ricetta_Cod
                        ricetta_dettaglio.Ricetta_Operazione_Cod = recode.From_Ricetta_Operazione_Cod
                        ricetta_dettaglio.Ricetta_Dettaglio_Cod = recode.From_Ricetta_Dettaglio_Cod
                        ricetta_dettaglio.Ricetta_SuperUser = Destinazione_Piva_SuperUser


                        GiasContext.Ricette_Dettagli.Attach(ricetta_dettaglio)
                        GiasContext.Entry(ricetta_dettaglio).State = EntityState.Modified

                    Next
                    GiasContext.SaveChanges()


                    For Each m As Ricette_Dettaglio_Tecnico In g2g.ricette_dettaglio_tecnico_update

                        Dim recode = (From rr In GiasContext.G2G_Recode_Ricette_Dettaglio_Tecnico Where rr.To_PivaSuperUser = Origine_Piva_SuperUser _
                                                                                               And rr.From_PivaSuperUser = Destinazione_Piva_SuperUser _
                                                                                               And rr.To_Ricetta_Cod = m.Ricetta_Cod _
                                                                                               And rr.To_Ricetta_Operazione_Cod = m.Ricetta_Operazione_Cod _
                                                                                               And rr.To_Ricetta_Dettaglio_Cod = m.Ricetta_Dettaglio_Cod _
                                                                                               And rr.To_Ricetta_Tecnico_Cod = m.Ricetta_Tecnico_Cod).FirstOrDefault()
                        recode.Username_Modifica = username
                        recode.Data_Modifica = data
                        recode.datainvio = data
                        g2g.G2G_Ricette_Dettaglio_Tecnico_Recode_update.Add(recode)
                        GiasContext.G2G_Recode_Ricette_Dettaglio_Tecnico.Attach(recode)
                        GiasContext.Entry(recode).State = EntityState.Modified

                        Dim ricetta_tecnico = (From mm In GiasContext.Ricette_Dettaglio_Tecnico Where mm.Ricetta_SuperUser = recode.From_PivaSuperUser _
                                                                                  And mm.Ricetta_Cod = recode.From_Ricetta_Cod _
                                                                                  And mm.Ricetta_Operazione_Cod = recode.From_Ricetta_Operazione_Cod _
                                                                                  And mm.Ricetta_Dettaglio_Cod = recode.From_Ricetta_Dettaglio_Cod _
                                                                                  And mm.Ricetta_Tecnico_Cod = recode.From_Ricetta_Tecnico_Cod).FirstOrDefault()
                        ricetta_tecnico = Gias_EF_Utility.CopyEntity(GiasContext, m, ricetta_tecnico, username, data)
                        ricetta_tecnico.Ricetta_Cod = recode.From_Ricetta_Cod
                        ricetta_tecnico.Ricetta_Operazione_Cod = recode.From_Ricetta_Operazione_Cod
                        ricetta_tecnico.Ricetta_Dettaglio_Cod = recode.From_Ricetta_Dettaglio_Cod
                        ricetta_tecnico.Ricetta_Tecnico_Cod = recode.From_Ricetta_Tecnico_Cod
                        ricetta_tecnico.Ricetta_SuperUser = Destinazione_Piva_SuperUser


                        GiasContext.Ricette_Dettaglio_Tecnico.Attach(ricetta_tecnico)
                        GiasContext.Entry(ricetta_tecnico).State = EntityState.Modified
                    Next

                    GiasContext.SaveChanges()

                    For Each m As Ricette_Destinazioni In g2g.ricette_destinazioni_update


                        Dim recode = (From rr In GiasContext.G2G_Recode_Ricette_Destinazioni Where rr.To_PivaSuperUser = Origine_Piva_SuperUser _
                                                                                               And rr.From_PivaSuperUser = Destinazione_Piva_SuperUser _
                                                                                               And rr.To_Ricetta_Cod = m.Ricetta_Cod _
                                                                                               And rr.To_Ricetta_Operazione_Cod = m.Ricetta_Operazione_Cod _
                                                                                               And rr.To_Ricetta_Dettaglio_Cod = m.Ricetta_Dettaglio_Cod _
                                                                                               And rr.To_Ricetta_Destinazione_Cod = m.Ricetta_Destinazione_Cod).FirstOrDefault()
                        recode.Username_Modifica = username
                        recode.Data_Modifica = data
                        recode.datainvio = data
                        g2g.G2G_Ricette_Destinazioni_Recode_update.Add(recode)
                        GiasContext.G2G_Recode_Ricette_Destinazioni.Attach(recode)
                        GiasContext.Entry(recode).State = EntityState.Modified

                        Dim ricetta_destinazione = (From mm In GiasContext.Ricette_Destinazioni Where mm.Ricetta_SuperUser = recode.From_PivaSuperUser _
                                                                          And mm.Ricetta_Cod = recode.From_Ricetta_Cod _
                                                                          And mm.Ricetta_Operazione_Cod = recode.From_Ricetta_Operazione_Cod _
                                                                          And mm.Ricetta_Dettaglio_Cod = recode.From_Ricetta_Dettaglio_Cod _
                                                                          And mm.Ricetta_Destinazione_Cod = recode.From_Ricetta_Destinazione_Cod).FirstOrDefault()
                        ricetta_destinazione = Gias_EF_Utility.CopyEntity(GiasContext, m, ricetta_destinazione, username, data)
                        ricetta_destinazione.Ricetta_Cod = recode.From_Ricetta_Cod
                        ricetta_destinazione.Ricetta_Operazione_Cod = recode.From_Ricetta_Operazione_Cod
                        ricetta_destinazione.Ricetta_Dettaglio_Cod = recode.From_Ricetta_Dettaglio_Cod
                        ricetta_destinazione.Ricetta_Destinazione_Cod = recode.From_Ricetta_Destinazione_Cod
                        ricetta_destinazione.Ricetta_SuperUser = Destinazione_Piva_SuperUser
                        ricetta_destinazione.Piva = piva


                        GiasContext.Ricette_Destinazioni.Attach(ricetta_destinazione)
                        GiasContext.Entry(ricetta_destinazione).State = EntityState.Modified

                    Next
                    GiasContext.SaveChanges()

                    '
                    'INSERT
                    '

                    For Each s As Ricette In g2g.ricette_insert

                        Dim idSeq As Integer = objSequenze.NuovoId_Tabella("ricette", 0, 2000000000, objParametri, UtilizzaTransazione:=False)
                        Dim ricetta = Gias_EF_Utility.CopyEntity(GiasContext, s, Nothing, username, data)

                        ricetta.Ricetta_Cod = idSeq
                        ricetta.Ricetta_SuperUser = Destinazione_Piva_SuperUser
                        ricetta.Piva = piva

                        Dim to_programmazione_cod As Integer = 0

                        If ricetta.Programmazione_Cod <> 0 Then

                            Select Case ricetta.Tipo_Ricetta
                                Case enum_TipoRicetta.PUA
                                    to_programmazione_cod = (From g2gPua In GiasContext.G2G_Recode_Programmazione_Testata Where g2gPua.To_PivaSuperUser = s.Ricetta_SuperUser _
                                                                                 And g2gPua.From_PivaSuperUser = Destinazione_Piva_SuperUser _
                                                                                 And g2gPua.To_Piva = s.Piva _
                                                                                 And g2gPua.To_Programmazione_Cod = s.Programmazione_Cod).FirstOrDefault.From_Programmazione_Cod
                                Case enum_TipoRicetta.Standard_Destinazioni
                                    If ricetta.Programmazione_Cod <> 0 Then
                                        Throw New Exception("Programmazione Cod non mappata")
                                    End If
                                Case enum_TipoRicetta.PianoDistribuzioneConcimi
                                    Dim recodeProgrammazione = (From g2gPC In GiasContext.G2G_Recode_PianoConcimazione_Testata Where g2gPC.To_PivaSuperUser = s.Ricetta_SuperUser _
                                                                                                      And g2gPC.From_PivaSuperUser = Destinazione_Piva_SuperUser _
                                                                                                      And g2gPC.To_PC_Testata_Cod = s.Programmazione_Cod).FirstOrDefault
                                    If recodeProgrammazione IsNot Nothing Then
                                        to_programmazione_cod = recodeProgrammazione.From_PC_Testata_Cod
                                    Else
                                        Throw New Exception("Recode PianoConcimazione_Testata in Ricetta_Testata non trovato")
                                    End If

                                Case enum_TipoRicetta.PianoDistribuzionePua
                                    Dim recodePua = (From g2gPua In GiasContext.G2G_Recode_Pua Where g2gPua.To_PivaSuperUser = s.Ricetta_SuperUser _
                                                                                 And g2gPua.From_PivaSuperUser = Destinazione_Piva_SuperUser _
                                                                                 And g2gPua.To_Piva = s.Piva _
                                                                                 And g2gPua.To_Pua_Cod = s.Programmazione_Cod).FirstOrDefault
                                    If recodePua IsNot Nothing Then
                                        to_programmazione_cod = recodePua.From_Pua_Cod
                                    Else
                                        Throw New Exception("Recode PUA in Ricetta_Testata non trovato")
                                    End If
                            End Select
                            ricetta.Programmazione_Cod = to_programmazione_cod
                        End If

                        GiasContext.Ricette.Add(ricetta)

                        Dim recode =
                            New G2G_Recode_Ricette With {
                                .From_PivaSuperUser = Destinazione_Piva_SuperUser,
                                .To_PivaSuperUser = Origine_Piva_SuperUser,
                                .From_Piva = piva,
                                .To_Piva = s.Piva,
                                .From_Ricetta_Cod = ricetta.Ricetta_Cod,
                                .To_Ricetta_Cod = s.Ricetta_Cod,
                                .Username_Creazione = username,
                                .Username_Modifica = username,
                                .Data_Creazione = Now(),
                                .Data_Modifica = Now(),
                                .Validita_Inizio = AGRODATAINIZIO,
                                .Validita_Fine = AGRODATAFINE,
                                .inviato = "0",
                                .datainvio = Now()
                            }
                        g2g.G2G_Ricette_Recode_insert.Add(recode)
                        GiasContext.G2G_Recode_Ricette.Add(recode)

                    Next

                    GiasContext.SaveChanges()

                    For Each s As Ricette_Operazioni In g2g.ricette_operazioni_insert

                        Dim idSeq As Integer = objSequenze.NuovoId_Tabella("ricette_operazioni", 0, 2000000000, objParametri, UtilizzaTransazione:=False)
                        Dim ricetta_operazione = Gias_EF_Utility.CopyEntity(GiasContext, s, Nothing, username, data)

                        ricetta_operazione.Ricetta_Operazione_Cod = idSeq
                        ricetta_operazione.Ricetta_SuperUser = Destinazione_Piva_SuperUser

                        ricetta_operazione.Ricetta_Cod = (From recode_ricetta In GiasContext.G2G_Recode_Ricette Where recode_ricetta.To_PivaSuperUser = Origine_Piva_SuperUser _
                                                                                                             And recode_ricetta.From_PivaSuperUser = Destinazione_Piva_SuperUser _
                                                                                                             And recode_ricetta.To_Piva = from_piva _
                                                                                                             And recode_ricetta.From_Piva = piva _
                                                                                                             And recode_ricetta.To_Ricetta_Cod = s.Ricetta_Cod).FirstOrDefault.From_Ricetta_Cod

                        If ricetta_operazione.Ricetta_Operazione_Cod_RIF IsNot Nothing AndAlso ricetta_operazione.Ricetta_Operazione_Cod_RIF <> 0 Then

                            ricetta_operazione.Ricetta_Operazione_Cod_RIF = (From recode_operazione In GiasContext.G2G_Recode_Ricette_Operazioni Where recode_operazione.To_PivaSuperUser = Origine_Piva_SuperUser And recode_operazione.From_PivaSuperUser = Destinazione_Piva_SuperUser _
                                                                                                                                                         And recode_operazione.To_Ricetta_Operazione_Cod = s.Ricetta_Operazione_Cod_RIF).FirstOrDefault.From_Ricetta_Operazione_Cod

                        End If

                        GiasContext.Ricette_Operazioni.Add(ricetta_operazione)

                        Dim recode =
                                New G2G_Recode_Ricette_Operazioni With {
                                    .From_PivaSuperUser = Destinazione_Piva_SuperUser,
                                    .To_PivaSuperUser = Origine_Piva_SuperUser,
                                    .From_Piva = piva,
                                    .To_Piva = from_piva,
                                    .From_Ricetta_Cod = ricetta_operazione.Ricetta_Cod,
                                    .To_Ricetta_Cod = s.Ricetta_Cod,
                                    .From_Ricetta_Operazione_Cod = ricetta_operazione.Ricetta_Operazione_Cod,
                                    .To_Ricetta_Operazione_Cod = s.Ricetta_Operazione_Cod,
                                    .Username_Creazione = username,
                                    .Username_Modifica = username,
                                    .Data_Creazione = Now(),
                                    .Data_Modifica = Now(),
                                    .Validita_Inizio = AGRODATAINIZIO,
                                    .Validita_Fine = AGRODATAFINE,
                                    .inviato = "0",
                                    .datainvio = Now()
                                }
                        g2g.G2G_Ricette_Operazioni_Recode_insert.Add(recode)
                        GiasContext.G2G_Recode_Ricette_Operazioni.Add(recode)

                    Next
                    GiasContext.SaveChanges()

                    For Each s As Ricette_Dettagli In g2g.ricette_dettagli_insert

                        Dim ricetta_dettaglio = Gias_EF_Utility.CopyEntity(GiasContext, s, Nothing, username, data)

                        Dim idSeq As Integer = objSequenze.NuovoId_Tabella("ricette_dettagli", 0, 2000000000, objParametri, UtilizzaTransazione:=False)
                        ricetta_dettaglio.Ricetta_Dettaglio_Cod = idSeq

                        ricetta_dettaglio.Ricetta_SuperUser = Destinazione_Piva_SuperUser

                        ricetta_dettaglio.Ricetta_Cod = (From recode_ricetta In GiasContext.G2G_Recode_Ricette Where recode_ricetta.To_PivaSuperUser = Origine_Piva_SuperUser _
                                                                                                             And recode_ricetta.From_PivaSuperUser = Destinazione_Piva_SuperUser _
                                                                                                             And recode_ricetta.To_Piva = from_piva _
                                                                                                             And recode_ricetta.From_Piva = piva _
                                                                                                             And recode_ricetta.To_Ricetta_Cod = s.Ricetta_Cod).FirstOrDefault.From_Ricetta_Cod

                        ricetta_dettaglio.Ricetta_Operazione_Cod = (From recode_operazione In GiasContext.G2G_Recode_Ricette_Operazioni Where recode_operazione.To_PivaSuperUser = Origine_Piva_SuperUser _
                                                                                                             And recode_operazione.From_PivaSuperUser = Destinazione_Piva_SuperUser _
                                                                                                             And recode_operazione.To_Piva = from_piva _
                                                                                                             And recode_operazione.From_Piva = piva _
                                                                                                             And recode_operazione.To_Ricetta_Cod = s.Ricetta_Cod _
                                                                                                             And recode_operazione.To_Ricetta_Operazione_Cod = s.Ricetta_Operazione_Cod).FirstOrDefault.From_Ricetta_Operazione_Cod

                        GiasContext.Ricette_Dettagli.Add(ricetta_dettaglio)

                        Dim recode =
                                New G2G_Recode_Ricette_Dettagli With {
                                    .From_PivaSuperUser = Destinazione_Piva_SuperUser,
                                    .To_PivaSuperUser = Origine_Piva_SuperUser,
                                    .From_Piva = piva,
                                    .To_Piva = from_piva,
                                    .From_Ricetta_Cod = ricetta_dettaglio.Ricetta_Cod,
                                    .To_Ricetta_Cod = s.Ricetta_Cod,
                                    .From_Ricetta_Operazione_Cod = ricetta_dettaglio.Ricetta_Operazione_Cod,
                                    .To_Ricetta_Operazione_Cod = s.Ricetta_Operazione_Cod,
                                    .From_Ricetta_Dettaglio_Cod = ricetta_dettaglio.Ricetta_Dettaglio_Cod,
                                    .To_Ricetta_Dettaglio_Cod = s.Ricetta_Dettaglio_Cod,
                                    .Username_Creazione = username,
                                    .Username_Modifica = username,
                                    .Data_Creazione = Now(),
                                    .Data_Modifica = Now(),
                                    .Validita_Inizio = AGRODATAINIZIO,
                                    .Validita_Fine = AGRODATAFINE,
                                    .inviato = "0",
                                    .datainvio = Now()
                                }
                        g2g.G2G_Ricette_Dettagli_Recode_insert.Add(recode)
                        GiasContext.G2G_Recode_Ricette_Dettagli.Add(recode)

                    Next
                    GiasContext.SaveChanges()


                    For Each s As Ricette_Dettaglio_Tecnico In g2g.ricette_dettaglio_tecnico_insert

                        Dim ricetta_dettaglio_tecnico = Gias_EF_Utility.CopyEntity(GiasContext, s, Nothing, username, data)

                        Dim idSeq As Integer = objSequenze.NuovoId_Tabella("ricette_dettaglio_tecnico", 0, 2000000000, objParametri, UtilizzaTransazione:=False)
                        ricetta_dettaglio_tecnico.Ricetta_Tecnico_Cod = idSeq

                        ricetta_dettaglio_tecnico.Ricetta_SuperUser = Destinazione_Piva_SuperUser

                        ricetta_dettaglio_tecnico.Ricetta_Cod = (From recode_ricetta In GiasContext.G2G_Recode_Ricette Where recode_ricetta.To_PivaSuperUser = Origine_Piva_SuperUser _
                                                                                                                 And recode_ricetta.From_PivaSuperUser = Destinazione_Piva_SuperUser _
                                                                                                                 And recode_ricetta.To_Piva = from_piva _
                                                                                                                 And recode_ricetta.From_Piva = piva _
                                                                                                                 And recode_ricetta.To_Ricetta_Cod = s.Ricetta_Cod).FirstOrDefault.From_Ricetta_Cod

                        ricetta_dettaglio_tecnico.Ricetta_Operazione_Cod = (From recode_operazione In GiasContext.G2G_Recode_Ricette_Operazioni Where recode_operazione.To_PivaSuperUser = Origine_Piva_SuperUser _
                                                                                                                 And recode_operazione.From_PivaSuperUser = Destinazione_Piva_SuperUser _
                                                                                                                 And recode_operazione.To_Piva = from_piva _
                                                                                                                 And recode_operazione.From_Piva = piva _
                                                                                                                 And recode_operazione.To_Ricetta_Cod = s.Ricetta_Cod _
                                                                                                                 And recode_operazione.To_Ricetta_Operazione_Cod = s.Ricetta_Operazione_Cod).FirstOrDefault.From_Ricetta_Operazione_Cod

                        ricetta_dettaglio_tecnico.Ricetta_Dettaglio_Cod = (From recode_dettaglio In GiasContext.G2G_Recode_Ricette_Dettagli Where recode_dettaglio.To_PivaSuperUser = Origine_Piva_SuperUser _
                                                                                                                 And recode_dettaglio.From_PivaSuperUser = Destinazione_Piva_SuperUser _
                                                                                                                 And recode_dettaglio.To_Piva = from_piva _
                                                                                                                 And recode_dettaglio.From_Piva = piva _
                                                                                                                 And recode_dettaglio.To_Ricetta_Cod = s.Ricetta_Cod _
                                                                                                                 And recode_dettaglio.To_Ricetta_Operazione_Cod = s.Ricetta_Operazione_Cod _
                                                                                                                 And recode_dettaglio.To_Ricetta_Dettaglio_Cod = s.Ricetta_Dettaglio_Cod).FirstOrDefault.From_Ricetta_Dettaglio_Cod

                        GiasContext.Ricette_Dettaglio_Tecnico.Add(ricetta_dettaglio_tecnico)

                        Dim recode =
                                    New G2G_Recode_Ricette_Dettaglio_Tecnico With {
                                        .From_PivaSuperUser = Destinazione_Piva_SuperUser,
                                        .To_PivaSuperUser = Origine_Piva_SuperUser,
                                        .From_Piva = piva,
                                        .To_Piva = from_piva,
                                        .From_Ricetta_Cod = ricetta_dettaglio_tecnico.Ricetta_Cod,
                                        .To_Ricetta_Cod = s.Ricetta_Cod,
                                        .From_Ricetta_Operazione_Cod = ricetta_dettaglio_tecnico.Ricetta_Operazione_Cod,
                                        .To_Ricetta_Operazione_Cod = s.Ricetta_Operazione_Cod,
                                        .From_Ricetta_Dettaglio_Cod = ricetta_dettaglio_tecnico.Ricetta_Dettaglio_Cod,
                                        .To_Ricetta_Dettaglio_Cod = s.Ricetta_Dettaglio_Cod,
                                        .From_Ricetta_Tecnico_Cod = ricetta_dettaglio_tecnico.Ricetta_Tecnico_Cod,
                                        .To_Ricetta_Tecnico_Cod = s.Ricetta_Tecnico_Cod,
                                        .Username_Creazione = username,
                                        .Username_Modifica = username,
                                        .Data_Creazione = Now(),
                                        .Data_Modifica = Now(),
                                        .Validita_Inizio = AGRODATAINIZIO,
                                        .Validita_Fine = AGRODATAFINE,
                                        .inviato = "0",
                                        .datainvio = Now()
                                    }
                        g2g.G2G_Ricette_Dettaglio_Tecnico_Recode_insert.Add(recode)
                        GiasContext.G2G_Recode_Ricette_Dettaglio_Tecnico.Add(recode)

                    Next
                    GiasContext.SaveChanges()

                    For Each s As Ricette_Destinazioni In g2g.ricette_destinazioni_insert

                        Dim ricetta_destinazione = Gias_EF_Utility.CopyEntity(GiasContext, s, Nothing, username, data)

                        Dim idSeq As Integer = objSequenze.NuovoId_Tabella("ricette_destinazioni", 0, 2000000000, objParametri, UtilizzaTransazione:=False)
                        ricetta_destinazione.Ricetta_Destinazione_Cod = idSeq

                        ricetta_destinazione.Ricetta_SuperUser = Destinazione_Piva_SuperUser

                        ricetta_destinazione.Ricetta_Cod = (From recode_ricetta In GiasContext.G2G_Recode_Ricette Where recode_ricetta.To_PivaSuperUser = Origine_Piva_SuperUser _
                                                                                                             And recode_ricetta.From_PivaSuperUser = Destinazione_Piva_SuperUser _
                                                                                                             And recode_ricetta.To_Piva = from_piva _
                                                                                                             And recode_ricetta.From_Piva = piva _
                                                                                                             And recode_ricetta.To_Ricetta_Cod = s.Ricetta_Cod).FirstOrDefault.From_Ricetta_Cod

                        ricetta_destinazione.Ricetta_Operazione_Cod = (From recode_operazione In GiasContext.G2G_Recode_Ricette_Operazioni Where recode_operazione.To_PivaSuperUser = Origine_Piva_SuperUser _
                                                                                                             And recode_operazione.From_PivaSuperUser = Destinazione_Piva_SuperUser _
                                                                                                             And recode_operazione.To_Piva = from_piva _
                                                                                                             And recode_operazione.From_Piva = piva _
                                                                                                             And recode_operazione.To_Ricetta_Cod = s.Ricetta_Cod _
                                                                                                             And recode_operazione.To_Ricetta_Operazione_Cod = s.Ricetta_Operazione_Cod).FirstOrDefault.From_Ricetta_Operazione_Cod

                        ricetta_destinazione.Ricetta_Dettaglio_Cod = (From recode_dettaglio In GiasContext.G2G_Recode_Ricette_Dettagli Where recode_dettaglio.To_PivaSuperUser = Origine_Piva_SuperUser _
                                                                                                             And recode_dettaglio.From_PivaSuperUser = Destinazione_Piva_SuperUser _
                                                                                                             And recode_dettaglio.To_Piva = from_piva _
                                                                                                             And recode_dettaglio.From_Piva = piva _
                                                                                                             And recode_dettaglio.To_Ricetta_Cod = s.Ricetta_Cod _
                                                                                                             And recode_dettaglio.To_Ricetta_Operazione_Cod = s.Ricetta_Operazione_Cod _
                                                                                                             And recode_dettaglio.To_Ricetta_Dettaglio_Cod = s.Ricetta_Dettaglio_Cod).FirstOrDefault.From_Ricetta_Dettaglio_Cod

                        GiasContext.Ricette_Destinazioni.Add(ricetta_destinazione)

                        Dim recode =
                                New G2G_Recode_Ricette_Destinazioni With {
                                    .From_PivaSuperUser = Destinazione_Piva_SuperUser,
                                    .To_PivaSuperUser = Origine_Piva_SuperUser,
                                    .From_Piva = piva,
                                    .To_Piva = from_piva,
                                    .From_Ricetta_Cod = ricetta_destinazione.Ricetta_Cod,
                                    .To_Ricetta_Cod = s.Ricetta_Cod,
                                    .From_Ricetta_Operazione_Cod = ricetta_destinazione.Ricetta_Operazione_Cod,
                                    .To_Ricetta_Operazione_Cod = s.Ricetta_Operazione_Cod,
                                    .From_Ricetta_Dettaglio_Cod = ricetta_destinazione.Ricetta_Dettaglio_Cod,
                                    .To_Ricetta_Dettaglio_Cod = s.Ricetta_Dettaglio_Cod,
                                    .From_Ricetta_Destinazione_Cod = ricetta_destinazione.Ricetta_Destinazione_Cod,
                                    .To_Ricetta_Destinazione_Cod = s.Ricetta_Destinazione_Cod,
                                    .Username_Creazione = username,
                                    .Username_Modifica = username,
                                    .Data_Creazione = Now(),
                                    .Data_Modifica = Now(),
                                    .Validita_Inizio = AGRODATAINIZIO,
                                    .Validita_Fine = AGRODATAFINE,
                                    .inviato = "0",
                                    .datainvio = Now()
                                }
                        g2g.G2G_Ricette_Destinazioni_Recode_insert.Add(recode)
                        GiasContext.G2G_Recode_Ricette_Destinazioni.Add(recode)

                    Next
                    GiasContext.SaveChanges()


                    For Each s As RicettexAgenda In g2g.ricettexagenda_insert
                        Dim ricettaxagenda = Gias_EF_Utility.CopyEntity(GiasContext, s, Nothing, username, data)

                        'Richiedo un nuovo id sequenza
                        ricettaxagenda.Ricetta_SuperUser = Destinazione_Piva_SuperUser

                        ricettaxagenda.Ricetta_Cod = (From recode_ricetta In GiasContext.G2G_Recode_Ricette Where recode_ricetta.To_PivaSuperUser = Origine_Piva_SuperUser _
                                                                                                             And recode_ricetta.From_PivaSuperUser = Destinazione_Piva_SuperUser _
                                                                                                             And recode_ricetta.To_Piva = from_piva _
                                                                                                             And recode_ricetta.From_Piva = piva _
                                                                                                             And recode_ricetta.To_Ricetta_Cod = s.Ricetta_Cod).FirstOrDefault.From_Ricetta_Cod

                        ricettaxagenda.Ricetta_Operazione_Cod = (From recode_operazione In GiasContext.G2G_Recode_Ricette_Operazioni Where recode_operazione.To_PivaSuperUser = Origine_Piva_SuperUser _
                                                                                                             And recode_operazione.From_PivaSuperUser = Destinazione_Piva_SuperUser _
                                                                                                             And recode_operazione.To_Piva = from_piva _
                                                                                                             And recode_operazione.From_Piva = piva _
                                                                                                             And recode_operazione.To_Ricetta_Cod = s.Ricetta_Cod _
                                                                                                             And recode_operazione.To_Ricetta_Operazione_Cod = s.Ricetta_Operazione_Cod).FirstOrDefault.From_Ricetta_Operazione_Cod

                        GiasContext.RicettexAgenda.Add(ricettaxagenda)

                    Next
                    GiasContext.SaveChanges()


                    For Each s As RicettexCultivar In g2g.ricettexcultivar_insert

                        Dim ricettaxcultivar = Gias_EF_Utility.CopyEntity(GiasContext, s, Nothing, username, data)

                        'Richiedo un nuovo id sequenza
                        ricettaxcultivar.Ricetta_SuperUser = Destinazione_Piva_SuperUser

                        ricettaxcultivar.Ricetta_Cod = (From recode_ricetta In GiasContext.G2G_Recode_Ricette Where recode_ricetta.To_PivaSuperUser = Origine_Piva_SuperUser _
                                                                                                             And recode_ricetta.From_PivaSuperUser = Destinazione_Piva_SuperUser _
                                                                                                             And recode_ricetta.To_Piva = from_piva _
                                                                                                             And recode_ricetta.From_Piva = piva _
                                                                                                             And recode_ricetta.To_Ricetta_Cod = s.Ricetta_Cod).FirstOrDefault.From_Ricetta_Cod

                        GiasContext.RicettexCultivar.Add(ricettaxcultivar)

                    Next
                    GiasContext.SaveChanges()


                    For Each s As RicettexNote In g2g.ricettexnote_insert

                        Dim ricettaxnote = Gias_EF_Utility.CopyEntity(GiasContext, s, Nothing, username, data)

                        'Richiedo un nuovo id sequenza
                        ricettaxnote.Ricetta_SuperUser = Destinazione_Piva_SuperUser

                        ricettaxnote.Ricetta_Cod = (From recode_ricetta In GiasContext.G2G_Recode_Ricette Where recode_ricetta.To_PivaSuperUser = Origine_Piva_SuperUser _
                                                                                                             And recode_ricetta.From_PivaSuperUser = Destinazione_Piva_SuperUser _
                                                                                                             And recode_ricetta.To_Piva = from_piva _
                                                                                                             And recode_ricetta.From_Piva = piva _
                                                                                                             And recode_ricetta.To_Ricetta_Cod = s.Ricetta_Cod).FirstOrDefault.From_Ricetta_Cod

                        ricettaxnote.Ricetta_Operazione_Cod = (From recode_operazione In GiasContext.G2G_Recode_Ricette_Operazioni Where recode_operazione.To_PivaSuperUser = Origine_Piva_SuperUser _
                                                                                                             And recode_operazione.From_PivaSuperUser = Destinazione_Piva_SuperUser _
                                                                                                             And recode_operazione.To_Piva = from_piva _
                                                                                                             And recode_operazione.From_Piva = piva _
                                                                                                             And recode_operazione.To_Ricetta_Cod = s.Ricetta_Cod _
                                                                                                             And recode_operazione.To_Ricetta_Operazione_Cod = s.Ricetta_Operazione_Cod).FirstOrDefault.From_Ricetta_Operazione_Cod

                        ricettaxnote.Nota_Cod = (From recode_note In GiasContext.G2G_Recode_NoteIntervento Where recode_note.To_PivaSuperUser = Origine_Piva_SuperUser _
                                                                                                            And recode_note.From_PivaSuperUser = Destinazione_Piva_SuperUser _
                                                                                                            And recode_note.To_Nota_Cod = s.Nota_Cod).FirstOrDefault.From_Nota_Cod

                        GiasContext.RicettexNote.Add(ricettaxnote)

                    Next
                    GiasContext.SaveChanges()

                    g2g.Recode = New G2G_Recode

                    g2g.Recode.G2GRecodeRicetteToInsert = g2g.G2G_Ricette_Recode_insert
                    g2g.Recode.G2GRecodeRicetteToUpdate = g2g.G2G_Ricette_Recode_update
                    g2g.Recode.G2GRecodeRicetteToDelete = g2g.G2G_Ricette_Recode_delete

                    g2g.Recode.G2GRecodeRicette_OperazioniToInsert = g2g.G2G_Ricette_Operazioni_Recode_insert
                    g2g.Recode.G2GRecodeRicette_OperazioniToUpdate = g2g.G2G_Ricette_Operazioni_Recode_update
                    g2g.Recode.G2GRecodeRicette_OperazioniToDelete = g2g.G2G_Ricette_Operazioni_Recode_delete

                    g2g.Recode.G2GRecodeRicette_DettagliToInsert = g2g.G2G_Ricette_Dettagli_Recode_insert
                    g2g.Recode.G2GRecodeRicette_DettagliToUpdate = g2g.G2G_Ricette_Dettagli_Recode_update
                    g2g.Recode.G2GRecodeRicette_DettagliToDelete = g2g.G2G_Ricette_Dettagli_Recode_delete

                    g2g.Recode.G2GRecodeRicette_Dettaglio_TecnicoToInsert = g2g.G2G_Ricette_Dettaglio_Tecnico_Recode_insert
                    g2g.Recode.G2GRecodeRicette_Dettaglio_TecnicoToUpdate = g2g.G2G_Ricette_Dettaglio_Tecnico_Recode_update
                    g2g.Recode.G2GRecodeRicette_Dettaglio_TecnicoToDelete = g2g.G2G_Ricette_Dettaglio_Tecnico_Recode_delete

                    g2g.Recode.G2GRecodeRicette_DestinazioniToInsert = g2g.G2G_Ricette_Destinazioni_Recode_insert
                    g2g.Recode.G2GRecodeRicette_DestinazioniToUpdate = g2g.G2G_Ricette_Destinazioni_Recode_update
                    g2g.Recode.G2GRecodeRicette_DestinazioniToDelete = g2g.G2G_Ricette_Destinazioni_Recode_delete

                    scope.Complete()

                End Using
            End Using

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore, ex)

        End Try

        Return g2g.Recode

    End Function

End Class
