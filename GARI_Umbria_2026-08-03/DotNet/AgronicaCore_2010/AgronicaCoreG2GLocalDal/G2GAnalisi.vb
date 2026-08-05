Imports System.Transactions

Imports Newtonsoft.Json

Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreModello
Imports Newtonsoft.Json.Linq
Imports System.Data.Entity

Public Class G2GAnalisi_R

    Public Function LeggiPerGias2Gias(
        ByVal objOpzioniImportImpresa As clsImpresa,
        ByVal piva As String,
        ByVal piva_destinazione As String,
        ByVal PivaSuperUser_Destinazione As String,
        ByRef objParametri As AgronicaCoreParametri
    ) As G2G_Analisi


        Dim NomeRoutine As String = "G2GlocalDal.G2GAnalisi_R.LeggiPerGias2Gias()"



        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
        Dim GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

        Dim oConfigurazione As G2G_Configurazione_FiltriReq_Analisi
        If objOpzioniImportImpresa.configurazione_analisi <> "" Then
            oConfigurazione = JsonConvert.DeserializeObject(Of G2G_Configurazione_FiltriReq_Analisi)(objOpzioniImportImpresa.configurazione_analisi)
        Else
            oConfigurazione = New G2G_Configurazione_FiltriReq_Analisi
            oConfigurazione.listaAnalisi_Testata_Tipo = New List(Of Integer)
        End If
        GiasContext.Database.CommandTimeout = 3600

        Dim rval As New G2G_Analisi

        'quanto sicuramente non assegnato ...
        With rval
            .From_Piva = piva
            .From_PivaSuperUser = objParametri.PivaSuperUser
            .G2G_Analisi_Certificato_Recode_insert = New List(Of G2G_Recode_Analisi_Certificato)
            .G2G_Analisi_Certificato_Recode_update = New List(Of G2G_Recode_Analisi_Certificato)
            .G2G_Analisi_Testata_Recode_insert = New List(Of G2G_Recode_Analisi_Testata)
            .G2G_Analisi_Testata_Recode_update = New List(Of G2G_Recode_Analisi_Testata)
            .G2G_Analisi_Dettagli_Recode_insert = New List(Of G2G_Recode_Analisi_Dettagli)
            .G2G_Analisi_Dettagli_Recode_update = New List(Of G2G_Recode_Analisi_Dettagli)
            .G2G_Analisi_EntitaxTestata_Recode_insert = New List(Of G2G_Recode_Analisi_EntitaxTestata)
            .G2G_Analisi_Campioni_insert = New List(Of G2G_Recode_Analisi_Campioni)
            .G2G_Analisi_Campioni_update = New List(Of G2G_Recode_Analisi_Campioni)
            .To_Piva = piva_destinazione
        End With


        'Certificati
        rval.analisi_certificato_insert = (
            From e In GiasContext.Analisi_EntitaxTestata
            Join t In GiasContext.Analisi_Testata On e.Analisi_Testata_Cod Equals t.Analisi_Testata_Cod
            Join p In GiasContext.Analisi_Certificato On t.Analisi_Certificato_Cod Equals p.Analisi_Certificato_Cod
            Where e.Piva = piva _
                AndAlso e.Validita_Fine >= objOpzioniImportImpresa.ValiditaInizio_analisi _
                AndAlso (oConfigurazione.listaAnalisi_Testata_Tipo.Count = 0 Or oConfigurazione.listaAnalisi_Testata_Tipo.Contains(t.Analisi_Testata_Tipo)) _
                AndAlso Not GiasContext.G2G_Recode_Analisi_Certificato.Any(Function(g) g.From_PivaSuperUser = p.Analisi_SuperUser And g.To_PivaSuperUser = PivaSuperUser_Destinazione And g.From_Analisi_Certificato_Cod = p.Analisi_Certificato_Cod)
            Select p).Distinct.ToList()

        rval.analisi_certificato_update = (
            From p In GiasContext.Analisi_Certificato
            Join g2g In GiasContext.G2G_Recode_Analisi_Certificato
                On p.Analisi_Certificato_Cod Equals g2g.From_Analisi_Certificato_Cod _
                And p.Analisi_SuperUser Equals g2g.From_PivaSuperUser
            Join analisi_testata In GiasContext.Analisi_Testata
                On analisi_testata.Analisi_Certificato_Cod Equals g2g.From_Analisi_Certificato_Cod _
                And analisi_testata.Analisi_SuperUser Equals g2g.From_PivaSuperUser
            Join analisi_entita In GiasContext.Analisi_EntitaxTestata
                On analisi_entita.Analisi_Testata_Cod Equals analisi_testata.Analisi_Testata_Cod _
                And analisi_entita.Analisi_SuperUser Equals analisi_testata.Analisi_SuperUser
            Where analisi_entita.Piva = piva _
                AndAlso g2g.To_PivaSuperUser = PivaSuperUser_Destinazione _
                AndAlso System.Data.Entity.DbFunctions.AddDays(g2g.Data_Modifica, -1) <= p.Data_Modifica
            Select p
            ).Distinct.ToList()

        rval.G2G_Analisi_Certificato_Recode_delete = (
            From r In GiasContext.G2G_Recode_Analisi_Certificato
            Where Not GiasContext.Analisi_Certificato.Any(Function(p) p.Analisi_Certificato_Cod = r.From_Analisi_Certificato_Cod And p.Analisi_SuperUser = r.From_PivaSuperUser) _
                AndAlso r.To_PivaSuperUser = PivaSuperUser_Destinazione
            Select r).Distinct.ToList()

        'Testate
        rval.analisi_testata_insert = (
            From e In GiasContext.Analisi_EntitaxTestata
            Join t In GiasContext.Analisi_Testata On e.Analisi_Testata_Cod Equals t.Analisi_Testata_Cod
            Where e.Piva = piva _
                AndAlso e.Validita_Fine >= objOpzioniImportImpresa.ValiditaInizio_analisi _
                AndAlso (oConfigurazione.listaAnalisi_Testata_Tipo.Count = 0 Or oConfigurazione.listaAnalisi_Testata_Tipo.Contains(t.Analisi_Testata_Tipo)) _
                AndAlso Not GiasContext.G2G_Recode_Analisi_Testata.Any(Function(g) g.From_PivaSuperUser = t.Analisi_SuperUser And g.To_PivaSuperUser = PivaSuperUser_Destinazione And g.From_Analisi_Testata_Cod = t.Analisi_Testata_Cod)
            Select t).Distinct.ToList()

        rval.analisi_testata_update = (
            From p In GiasContext.Analisi_Testata
            Join g2g In GiasContext.G2G_Recode_Analisi_Testata
                On p.Analisi_Testata_Cod Equals g2g.From_Analisi_Testata_Cod _
                And p.Analisi_SuperUser Equals g2g.From_PivaSuperUser
            Join analisi_entita In GiasContext.Analisi_EntitaxTestata
                On analisi_entita.Analisi_Testata_Cod Equals p.Analisi_Testata_Cod _
                And analisi_entita.Analisi_SuperUser Equals p.Analisi_SuperUser
            Where analisi_entita.Piva = piva _
                AndAlso g2g.To_PivaSuperUser = PivaSuperUser_Destinazione _
                AndAlso System.Data.Entity.DbFunctions.AddDays(g2g.Data_Modifica, -1) <= p.Data_Modifica
            Select p
            ).Distinct.ToList()

        Dim analisi_testata_cod_update As New List(Of Integer)
        For Each testata_update In rval.analisi_testata_update
            analisi_testata_cod_update.Add(testata_update.Analisi_Testata_Cod)
        Next

        rval.G2G_Analisi_Testata_Recode_delete = (
            From r In GiasContext.G2G_Recode_Analisi_Testata
            Join e In GiasContext.G2G_Recode_Analisi_EntitaxTestata
                On r.From_Analisi_Testata_Cod Equals e.From_Analisi_Testata_Cod _
                And r.From_PivaSuperUser Equals e.From_PivaSuperUser
            Where Not GiasContext.Analisi_Testata.Any(Function(p) p.Analisi_Testata_Cod = r.From_Analisi_Testata_Cod And p.Analisi_SuperUser = r.From_PivaSuperUser) _
                And e.From_Piva = piva _
                AndAlso e.To_PivaSuperUser = PivaSuperUser_Destinazione
            Select r).Distinct.ToList()

        'Dettagli
        rval.analisi_dettagli_insert = (
            From e In GiasContext.Analisi_EntitaxTestata
            Join t In GiasContext.Analisi_Testata On e.Analisi_Testata_Cod Equals t.Analisi_Testata_Cod
            Join d In GiasContext.Analisi_Dettagli On t.Analisi_Testata_Cod Equals d.Analisi_Testata_Cod
            Where e.Piva = piva _
                AndAlso e.Validita_Fine >= objOpzioniImportImpresa.ValiditaInizio_analisi _
                AndAlso (oConfigurazione.listaAnalisi_Testata_Tipo.Count = 0 Or oConfigurazione.listaAnalisi_Testata_Tipo.Contains(t.Analisi_Testata_Tipo)) _
                AndAlso Not GiasContext.G2G_Recode_Analisi_Dettagli.Any(Function(g) g.From_PivaSuperUser = d.Analisi_SuperUser And g.To_PivaSuperUser = PivaSuperUser_Destinazione And g.From_Analisi_Testata_Cod = d.Analisi_Testata_Cod And g.From_Analisi_Dettaglio_Cod = d.Analisi_Dettaglio_Cod)
            Select d).Distinct.ToList()

        rval.analisi_dettagli_update = (
            From p In GiasContext.Analisi_Dettagli
            Join g2g In GiasContext.G2G_Recode_Analisi_Dettagli
                On p.Analisi_Testata_Cod Equals g2g.From_Analisi_Testata_Cod _
                And p.Analisi_Dettaglio_Cod Equals g2g.From_Analisi_Dettaglio_Cod _
                And p.Analisi_SuperUser Equals g2g.From_PivaSuperUser
            Join analisi_entita In GiasContext.Analisi_EntitaxTestata
                On analisi_entita.Analisi_Testata_Cod Equals p.Analisi_Testata_Cod _
                And analisi_entita.Analisi_SuperUser Equals p.Analisi_SuperUser
            Where analisi_entita.Piva = piva _
                AndAlso g2g.To_PivaSuperUser = PivaSuperUser_Destinazione _
                AndAlso System.Data.Entity.DbFunctions.AddDays(g2g.Data_Modifica, -1) <= p.Data_Modifica
            Select p
            ).Distinct.ToList()

        rval.G2G_Analisi_Dettagli_Recode_delete = (
            From r In GiasContext.G2G_Recode_Analisi_Dettagli
            Join e In GiasContext.G2G_Recode_Analisi_EntitaxTestata
                On r.From_Analisi_Testata_Cod Equals e.From_Analisi_Testata_Cod _
                And r.From_PivaSuperUser Equals e.From_PivaSuperUser
            Where Not GiasContext.Analisi_Dettagli.Any(Function(p) p.Analisi_Dettaglio_Cod = r.From_Analisi_Dettaglio_Cod And p.Analisi_Testata_Cod = r.From_Analisi_Testata_Cod And p.Analisi_SuperUser = r.From_PivaSuperUser) _
                And e.From_Piva = piva _
                AndAlso e.To_PivaSuperUser = PivaSuperUser_Destinazione
            Select r).Distinct.ToList()

        'EntitaxTestata
        rval.G2G_Analisi_EntitaxTestata_Recode_delete = (
            From r In GiasContext.G2G_Recode_Analisi_EntitaxTestata
            Where (Not GiasContext.Analisi_EntitaxTestata.Any(Function(p) p.Analisi_Testata_Cod = r.From_Analisi_Testata_Cod And p.Analisi_SuperUser = r.From_PivaSuperUser And p.Piva = piva) And r.From_Piva = piva) _
                Or analisi_testata_cod_update.Contains(r.From_Analisi_Testata_Cod) _
                AndAlso r.To_PivaSuperUser = PivaSuperUser_Destinazione
            Select r).Distinct.ToList()

        rval.analisi_entitaxtestata_insert = (
            From e In GiasContext.Analisi_EntitaxTestata
            Join t In GiasContext.Analisi_Testata On e.Analisi_Testata_Cod Equals t.Analisi_Testata_Cod
            Where e.Piva = piva _
                AndAlso (e.Validita_Fine >= objOpzioniImportImpresa.ValiditaInizio_analisi _
                AndAlso (oConfigurazione.listaAnalisi_Testata_Tipo.Count = 0 Or oConfigurazione.listaAnalisi_Testata_Tipo.Contains(t.Analisi_Testata_Tipo)) _
                AndAlso Not GiasContext.G2G_Recode_Analisi_EntitaxTestata.Any(Function(g) g.From_PivaSuperUser = e.Analisi_SuperUser And g.To_PivaSuperUser = PivaSuperUser_Destinazione And g.From_Analisi_Testata_Cod = e.Analisi_Testata_Cod And g.From_Piva = e.Piva)) _
                Or (analisi_testata_cod_update.Contains(e.Analisi_Testata_Cod) And e.Piva = piva)
            Select e).Distinct.ToList()

        'analisi campioni
        rval.analisi_campioni_insert = (
            From e In GiasContext.Analisi_EntitaxTestata
            Join cxd In GiasContext.Analisi_CampionixDettagli On e.Analisi_SuperUser Equals cxd.Analisi_SuperUser And e.Analisi_Testata_Cod Equals cxd.Analisi_Testata_Cod
            Join t In GiasContext.Analisi_Testata On e.Analisi_SuperUser Equals t.Analisi_SuperUser And e.Analisi_Testata_Cod Equals t.Analisi_Testata_Cod
            Join c In GiasContext.Analisi_Campioni On cxd.Analisi_Campione_Cod Equals c.Analisi_Campione_Cod
            Where e.Piva = piva _
                AndAlso (e.Validita_Fine >= objOpzioniImportImpresa.ValiditaInizio_analisi _
                AndAlso (oConfigurazione.listaAnalisi_Testata_Tipo.Count = 0 Or oConfigurazione.listaAnalisi_Testata_Tipo.Contains(t.Analisi_Testata_Tipo)) _
                AndAlso Not GiasContext.G2G_Recode_Analisi_Campioni.Any(Function(g) g.From_PivaSuperUser = c.Analisi_SuperUser And g.To_PivaSuperUser = PivaSuperUser_Destinazione And g.From_Analisi_Campione_Cod = c.Analisi_Campione_Cod))
            Select c).Distinct.ToList

        rval.analisi_campioni_update = (
            From e In GiasContext.Analisi_EntitaxTestata
            Join cxd In GiasContext.Analisi_CampionixDettagli On e.Analisi_SuperUser Equals cxd.Analisi_SuperUser And e.Analisi_Testata_Cod Equals cxd.Analisi_Testata_Cod
            Join t In GiasContext.Analisi_Testata On e.Analisi_SuperUser Equals t.Analisi_SuperUser And e.Analisi_Testata_Cod Equals t.Analisi_Testata_Cod
            Join c In GiasContext.Analisi_Campioni On cxd.Analisi_Campione_Cod Equals c.Analisi_Campione_Cod
            Join g2g In GiasContext.G2G_Recode_Analisi_Campioni
                On c.Analisi_Campione_Cod Equals g2g.From_Analisi_Campione_Cod _
                And c.Analisi_SuperUser Equals g2g.From_PivaSuperUser
            Where e.Piva = piva _
                AndAlso System.Data.Entity.DbFunctions.AddDays(g2g.Data_Modifica, -1) <= c.Data_Modifica
            Select c
            ).Distinct.ToList()


        Dim analisi_campioni_cod_update As New List(Of Integer)
        For Each campione_update In rval.analisi_campioni_update
            analisi_campioni_cod_update.Add(campione_update.Analisi_Campione_Cod)
        Next

        rval.G2G_Analisi_Campioni_delete = (From g2g In GiasContext.G2G_Recode_Analisi_Campioni
                                            Where Not GiasContext.Analisi_Campioni.Any(Function(p) p.Analisi_Campione_Cod = g2g.From_Analisi_Campione_Cod _
                                                                                        And p.Analisi_SuperUser = g2g.From_PivaSuperUser) _
                                                AndAlso g2g.To_PivaSuperUser = PivaSuperUser_Destinazione).ToList

        'analisi_campionixdettagli
        rval.analisi_campionixdettagli_insert = (
            From e In GiasContext.Analisi_EntitaxTestata
            Join cxd In GiasContext.Analisi_CampionixDettagli On e.Analisi_SuperUser Equals cxd.Analisi_SuperUser And e.Analisi_Testata_Cod Equals cxd.Analisi_Testata_Cod
            Join t In GiasContext.Analisi_Testata On e.Analisi_SuperUser Equals t.Analisi_SuperUser And e.Analisi_Testata_Cod Equals t.Analisi_Testata_Cod
            Join c In GiasContext.Analisi_Campioni On cxd.Analisi_Campione_Cod Equals c.Analisi_Campione_Cod
            Join d In GiasContext.Analisi_CampionixDettagli On c.Analisi_Campione_Cod Equals d.Analisi_Campione_Cod
            Where e.Piva = piva _
                AndAlso (e.Validita_Fine >= objOpzioniImportImpresa.ValiditaInizio_analisi _
                AndAlso (oConfigurazione.listaAnalisi_Testata_Tipo.Count = 0 Or oConfigurazione.listaAnalisi_Testata_Tipo.Contains(t.Analisi_Testata_Tipo)) _
                AndAlso Not GiasContext.G2G_Recode_Analisi_Campioni.Any(Function(g) g.From_PivaSuperUser = c.Analisi_SuperUser And g.To_PivaSuperUser = PivaSuperUser_Destinazione And g.From_Analisi_Campione_Cod = c.Analisi_Campione_Cod))
            Select d).Distinct.ToList.Union(
            (From d In GiasContext.Analisi_CampionixDettagli Where analisi_campioni_cod_update.Contains(d.Analisi_Campione_Cod))
        ).Distinct.ToList

        rval.analisi_campionixdettagli_delete = (From d In GiasContext.Analisi_CampionixDettagli Where analisi_campioni_cod_update.Contains(d.Analisi_Campione_Cod)).ToList

        Return rval

    End Function

    Public Function LeggiPerGias2GiasReverse(
        ByVal objOpzioniImportImpresa As clsImpresa,
        ByVal piva As String,
        ByVal piva_destinazione As String,
        ByVal PivaSuperUser_Destinazione As String,
        ByRef objParametri As AgronicaCoreParametri
    ) As G2G_Analisi_Reverse


        Dim NomeRoutine As String = "G2GlocalDal.G2GAnalisi_R.LeggiPerGias2GiasReverse()"



        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
        Dim GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

        Dim oConfigurazione As G2G_Configurazione_FiltriReq_Analisi
        If objOpzioniImportImpresa.configurazione_analisi <> "" Then
            oConfigurazione = JsonConvert.DeserializeObject(Of G2G_Configurazione_FiltriReq_Analisi)(objOpzioniImportImpresa.configurazione_analisi)
        Else
            oConfigurazione = New G2G_Configurazione_FiltriReq_Analisi
            oConfigurazione.listaAnalisi_Testata_Tipo = New List(Of Integer)
        End If

        GiasContext.Database.CommandTimeout = 3600
        Dim rval As New G2G_Analisi_Reverse

        'quanto sicuramente non assegnato ...
        With rval
            .From_Piva = piva
            .From_PivaSuperUser = objParametri.PivaSuperUser
            .G2G_Analisi_Certificato_Recode_insert = New List(Of G2G_Recode_Analisi_Certificato)
            .G2G_Analisi_Certificato_Recode_update = New List(Of G2G_Recode_Analisi_Certificato)
            .G2G_Analisi_Testata_Recode_insert = New List(Of G2G_Recode_Analisi_Testata)
            .G2G_Analisi_Testata_Recode_update = New List(Of G2G_Recode_Analisi_Testata)
            .G2G_Analisi_Dettagli_Recode_insert = New List(Of G2G_Recode_Analisi_Dettagli)
            .G2G_Analisi_Dettagli_Recode_update = New List(Of G2G_Recode_Analisi_Dettagli)
            .G2G_Analisi_EntitaxTestata_Recode_insert = New List(Of G2G_Recode_Analisi_EntitaxTestata)
            .G2G_Analisi_Campioni_insert = New List(Of G2G_Recode_Analisi_Campioni)
            .G2G_Analisi_Campioni_update = New List(Of G2G_Recode_Analisi_Campioni)
            .To_Piva = piva_destinazione
        End With


        'Certificati
        rval.analisi_certificato_insert = (
            From e In GiasContext.Analisi_EntitaxTestata
            Join t In GiasContext.Analisi_Testata On e.Analisi_Testata_Cod Equals t.Analisi_Testata_Cod
            Join p In GiasContext.Analisi_Certificato On t.Analisi_Certificato_Cod Equals p.Analisi_Certificato_Cod
            Where e.Piva = piva _
                AndAlso e.Validita_Fine >= objOpzioniImportImpresa.ValiditaInizio_analisi _
                AndAlso (oConfigurazione.listaAnalisi_Testata_Tipo.Count = 0 Or oConfigurazione.listaAnalisi_Testata_Tipo.Contains(t.Analisi_Testata_Tipo)) _
                AndAlso Not GiasContext.G2G_Recode_Analisi_Certificato.Any(Function(g) g.To_PivaSuperUser = p.Analisi_SuperUser And g.From_PivaSuperUser = PivaSuperUser_Destinazione And g.To_Analisi_Certificato_Cod = p.Analisi_Certificato_Cod)
            Select p).Distinct.ToList()

        rval.analisi_certificato_update = (
            From p In GiasContext.Analisi_Certificato
            Join g2g In GiasContext.G2G_Recode_Analisi_Certificato
                On p.Analisi_Certificato_Cod Equals g2g.To_Analisi_Certificato_Cod _
                And p.Analisi_SuperUser Equals g2g.To_PivaSuperUser
            Join analisi_testata In GiasContext.Analisi_Testata
                On analisi_testata.Analisi_Certificato_Cod Equals g2g.To_Analisi_Certificato_Cod _
                And analisi_testata.Analisi_SuperUser Equals g2g.To_PivaSuperUser
            Join analisi_entita In GiasContext.Analisi_EntitaxTestata
                On analisi_entita.Analisi_Testata_Cod Equals analisi_testata.Analisi_Testata_Cod _
                And analisi_entita.Analisi_SuperUser Equals analisi_testata.Analisi_SuperUser
            Where analisi_entita.Piva = piva _
                AndAlso g2g.From_PivaSuperUser = PivaSuperUser_Destinazione _
                AndAlso System.Data.Entity.DbFunctions.AddDays(g2g.Data_Modifica, -1) <= p.Data_Modifica
            Select p
            ).Distinct.ToList()

        rval.G2G_Analisi_Certificato_Recode_delete = (
            From r In GiasContext.G2G_Recode_Analisi_Certificato
            Where Not GiasContext.Analisi_Certificato.Any(Function(p) p.Analisi_Certificato_Cod = r.To_Analisi_Certificato_Cod And p.Analisi_SuperUser = r.To_PivaSuperUser)
            Select r).Distinct.ToList()

        'Testate
        rval.analisi_testata_insert = (
            From e In GiasContext.Analisi_EntitaxTestata
            Join t In GiasContext.Analisi_Testata On e.Analisi_Testata_Cod Equals t.Analisi_Testata_Cod
            Where e.Piva = piva _
                AndAlso e.Validita_Fine >= objOpzioniImportImpresa.ValiditaInizio_analisi _
                AndAlso (oConfigurazione.listaAnalisi_Testata_Tipo.Count = 0 Or oConfigurazione.listaAnalisi_Testata_Tipo.Contains(t.Analisi_Testata_Tipo)) _
                AndAlso Not GiasContext.G2G_Recode_Analisi_Testata.Any(Function(g) g.To_PivaSuperUser = t.Analisi_SuperUser And g.From_PivaSuperUser = PivaSuperUser_Destinazione And g.To_Analisi_Testata_Cod = t.Analisi_Testata_Cod)
            Select t).Distinct.ToList()

        rval.analisi_testata_update = (
            From p In GiasContext.Analisi_Testata
            Join g2g In GiasContext.G2G_Recode_Analisi_Testata
                On p.Analisi_Testata_Cod Equals g2g.To_Analisi_Testata_Cod _
                And p.Analisi_SuperUser Equals g2g.To_PivaSuperUser
            Join analisi_entita In GiasContext.Analisi_EntitaxTestata
                On analisi_entita.Analisi_Testata_Cod Equals p.Analisi_Testata_Cod _
                And analisi_entita.Analisi_SuperUser Equals p.Analisi_SuperUser
            Where analisi_entita.Piva = piva _
                AndAlso g2g.From_PivaSuperUser = PivaSuperUser_Destinazione _
                AndAlso System.Data.Entity.DbFunctions.AddDays(g2g.Data_Modifica, -1) <= p.Data_Modifica
            Select p
            ).Distinct.ToList()

        Dim analisi_testata_cod_update As New List(Of Integer)
        For Each testata_update In rval.analisi_testata_update
            analisi_testata_cod_update.Add(testata_update.Analisi_Testata_Cod)
        Next

        rval.G2G_Analisi_Testata_Recode_delete = (
            From r In GiasContext.G2G_Recode_Analisi_Testata
            Join e In GiasContext.G2G_Recode_Analisi_EntitaxTestata
                On r.To_Analisi_Testata_Cod Equals e.To_Analisi_Testata_Cod _
                And r.To_PivaSuperUser Equals e.To_PivaSuperUser _
                And r.From_PivaSuperUser Equals e.From_PivaSuperUser
            Where Not GiasContext.Analisi_Testata.Any(Function(p) p.Analisi_Testata_Cod = r.To_Analisi_Testata_Cod And p.Analisi_SuperUser = r.To_PivaSuperUser) _
                And e.To_Piva = piva
            Select r).Distinct.ToList()

        'Dettagli
        rval.analisi_dettagli_insert = (
            From e In GiasContext.Analisi_EntitaxTestata
            Join t In GiasContext.Analisi_Testata On e.Analisi_Testata_Cod Equals t.Analisi_Testata_Cod
            Join d In GiasContext.Analisi_Dettagli On t.Analisi_Testata_Cod Equals d.Analisi_Testata_Cod
            Where e.Piva = piva _
                AndAlso e.Validita_Fine >= objOpzioniImportImpresa.ValiditaInizio_analisi _
                AndAlso (oConfigurazione.listaAnalisi_Testata_Tipo.Count = 0 Or oConfigurazione.listaAnalisi_Testata_Tipo.Contains(t.Analisi_Testata_Tipo)) _
                AndAlso Not GiasContext.G2G_Recode_Analisi_Dettagli.Any(Function(g) g.To_PivaSuperUser = d.Analisi_SuperUser And g.From_PivaSuperUser = PivaSuperUser_Destinazione And g.To_Analisi_Testata_Cod = d.Analisi_Testata_Cod And g.To_Analisi_Dettaglio_Cod = d.Analisi_Dettaglio_Cod)
            Select d).Distinct.ToList()

        rval.analisi_dettagli_update = (
            From p In GiasContext.Analisi_Dettagli
            Join g2g In GiasContext.G2G_Recode_Analisi_Dettagli
                On p.Analisi_Testata_Cod Equals g2g.To_Analisi_Testata_Cod _
                And p.Analisi_Dettaglio_Cod Equals g2g.To_Analisi_Dettaglio_Cod _
                And p.Analisi_SuperUser Equals g2g.To_PivaSuperUser
            Join analisi_entita In GiasContext.Analisi_EntitaxTestata
                On analisi_entita.Analisi_Testata_Cod Equals p.Analisi_Testata_Cod _
                And analisi_entita.Analisi_SuperUser Equals p.Analisi_SuperUser
            Where analisi_entita.Piva = piva _
                AndAlso g2g.From_PivaSuperUser = PivaSuperUser_Destinazione _
                AndAlso System.Data.Entity.DbFunctions.AddDays(g2g.Data_Modifica, -1) <= p.Data_Modifica
            Select p
            ).Distinct.ToList()

        rval.G2G_Analisi_Dettagli_Recode_delete = (
            From r In GiasContext.G2G_Recode_Analisi_Dettagli
            Join e In GiasContext.G2G_Recode_Analisi_EntitaxTestata
                On r.To_Analisi_Testata_Cod Equals e.To_Analisi_Testata_Cod _
                And r.To_PivaSuperUser Equals e.To_PivaSuperUser _
                And r.From_PivaSuperUser Equals e.From_PivaSuperUser
            Where Not GiasContext.Analisi_Dettagli.Any(Function(p) p.Analisi_Dettaglio_Cod = r.To_Analisi_Dettaglio_Cod And p.Analisi_Testata_Cod = r.To_Analisi_Testata_Cod And p.Analisi_SuperUser = r.To_PivaSuperUser) _
                And e.From_Piva = piva
            Select r).Distinct.ToList()

        'EntitaxTestata
        rval.G2G_Analisi_EntitaxTestata_Recode_delete = (
            From r In GiasContext.G2G_Recode_Analisi_EntitaxTestata
            Where (Not GiasContext.Analisi_EntitaxTestata.Any(Function(p) p.Analisi_Testata_Cod = r.To_Analisi_Testata_Cod And p.Analisi_SuperUser = r.To_PivaSuperUser And p.Piva = piva) And r.To_Piva = piva) _
                Or analisi_testata_cod_update.Contains(r.To_Analisi_Testata_Cod) _
                AndAlso r.From_PivaSuperUser = PivaSuperUser_Destinazione
            Select r).Distinct.ToList()

        rval.analisi_entitaxtestata_insert = (
            From e In GiasContext.Analisi_EntitaxTestata
            Join t In GiasContext.Analisi_Testata On e.Analisi_Testata_Cod Equals t.Analisi_Testata_Cod
            Where e.Piva = piva _
                AndAlso (e.Validita_Fine >= objOpzioniImportImpresa.ValiditaInizio_analisi _
                AndAlso (oConfigurazione.listaAnalisi_Testata_Tipo.Count = 0 Or oConfigurazione.listaAnalisi_Testata_Tipo.Contains(t.Analisi_Testata_Tipo)) _
                AndAlso Not GiasContext.G2G_Recode_Analisi_EntitaxTestata.Any(Function(g) g.To_PivaSuperUser = e.Analisi_SuperUser And g.From_PivaSuperUser = PivaSuperUser_Destinazione And g.To_Analisi_Testata_Cod = e.Analisi_Testata_Cod And g.To_Piva = e.Piva)) _
                Or (analisi_testata_cod_update.Contains(e.Analisi_Testata_Cod) And e.Piva = piva)
            Select e).Distinct.ToList()

        'analisi campioni
        rval.analisi_campioni_insert = (
            From e In GiasContext.Analisi_EntitaxTestata
            Join cxd In GiasContext.Analisi_CampionixDettagli On e.Analisi_SuperUser Equals cxd.Analisi_SuperUser And e.Analisi_Testata_Cod Equals cxd.Analisi_Testata_Cod
            Join t In GiasContext.Analisi_Testata On e.Analisi_SuperUser Equals t.Analisi_SuperUser And e.Analisi_Testata_Cod Equals t.Analisi_Testata_Cod
            Join c In GiasContext.Analisi_Campioni On cxd.Analisi_Campione_Cod Equals c.Analisi_Campione_Cod
            Where e.Piva = piva _
                AndAlso (e.Validita_Fine >= objOpzioniImportImpresa.ValiditaInizio_analisi _
                AndAlso (oConfigurazione.listaAnalisi_Testata_Tipo.Count = 0 Or oConfigurazione.listaAnalisi_Testata_Tipo.Contains(t.Analisi_Testata_Tipo)) _
                AndAlso Not GiasContext.G2G_Recode_Analisi_Campioni.Any(Function(g) g.To_PivaSuperUser = c.Analisi_SuperUser And g.From_PivaSuperUser = PivaSuperUser_Destinazione And g.To_Analisi_Campione_Cod = c.Analisi_Campione_Cod))
            Select c).Distinct.ToList

        rval.analisi_campioni_update = (
            From e In GiasContext.Analisi_EntitaxTestata
            Join cxd In GiasContext.Analisi_CampionixDettagli On e.Analisi_SuperUser Equals cxd.Analisi_SuperUser And e.Analisi_Testata_Cod Equals cxd.Analisi_Testata_Cod
            Join t In GiasContext.Analisi_Testata On e.Analisi_SuperUser Equals t.Analisi_SuperUser And e.Analisi_Testata_Cod Equals t.Analisi_Testata_Cod
            Join c In GiasContext.Analisi_Campioni On cxd.Analisi_Campione_Cod Equals c.Analisi_Campione_Cod
            Join g2g In GiasContext.G2G_Recode_Analisi_Campioni
                On c.Analisi_Campione_Cod Equals g2g.To_Analisi_Campione_Cod _
                And c.Analisi_SuperUser Equals g2g.To_PivaSuperUser
            Where e.Piva = piva _
                AndAlso g2g.From_PivaSuperUser = PivaSuperUser_Destinazione _
                AndAlso System.Data.Entity.DbFunctions.AddDays(g2g.Data_Modifica, -1) <= c.Data_Modifica
            Select c
            ).Distinct.ToList()


        Dim analisi_campioni_cod_update As New List(Of Integer)
        For Each campione_update In rval.analisi_campioni_update
            analisi_campioni_cod_update.Add(campione_update.Analisi_Campione_Cod)
        Next

        rval.G2G_Analisi_Campioni_delete = (From g2g In GiasContext.G2G_Recode_Analisi_Campioni
                                            Where Not GiasContext.Analisi_Campioni.Any(Function(p) p.Analisi_Campione_Cod = g2g.To_Analisi_Campione_Cod _
                                                                                        And p.Analisi_SuperUser = g2g.To_PivaSuperUser) _
                                                AndAlso g2g.From_PivaSuperUser = PivaSuperUser_Destinazione).ToList

        'analisi_campionixdettagli
        rval.analisi_campionixdettagli_insert = (
            From e In GiasContext.Analisi_EntitaxTestata
            Join cxd In GiasContext.Analisi_CampionixDettagli On e.Analisi_SuperUser Equals cxd.Analisi_SuperUser And e.Analisi_Testata_Cod Equals cxd.Analisi_Testata_Cod
            Join t In GiasContext.Analisi_Testata On e.Analisi_SuperUser Equals t.Analisi_SuperUser And e.Analisi_Testata_Cod Equals t.Analisi_Testata_Cod
            Join c In GiasContext.Analisi_Campioni On cxd.Analisi_Campione_Cod Equals c.Analisi_Campione_Cod
            Join d In GiasContext.Analisi_CampionixDettagli On c.Analisi_Campione_Cod Equals d.Analisi_Campione_Cod
            Where e.Piva = piva _
                AndAlso (e.Validita_Fine >= objOpzioniImportImpresa.ValiditaInizio_analisi _
                AndAlso (oConfigurazione.listaAnalisi_Testata_Tipo.Count = 0 Or oConfigurazione.listaAnalisi_Testata_Tipo.Contains(t.Analisi_Testata_Tipo)) _
                AndAlso Not GiasContext.G2G_Recode_Analisi_Campioni.Any(Function(g) g.To_PivaSuperUser = c.Analisi_SuperUser And g.From_PivaSuperUser = PivaSuperUser_Destinazione And g.To_Analisi_Campione_Cod = c.Analisi_Campione_Cod))
            Select d).Distinct.ToList.Union(
            (From d In GiasContext.Analisi_CampionixDettagli Where analisi_campioni_cod_update.Contains(d.Analisi_Campione_Cod))
        ).Distinct.ToList

        rval.analisi_campionixdettagli_delete = (From d In GiasContext.Analisi_CampionixDettagli Where analisi_campioni_cod_update.Contains(d.Analisi_Campione_Cod)).ToList

        Return rval

    End Function

    Public Function Leggi_AnalisiCondivise_G2G(ByVal _Piva As String, ByVal listaImprese As List(Of String), DataValidita As Date, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, ByRef listaImpresePubbliche As List(Of String)) As G2G_Analisi

        Dim nomeRoutine As String = "AgronicaCoreG2GLocalDAL.G2GMateriePrimeCampionature_R.Leggi_MateriePrimeCampionature_G2G()"
        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Dim rval As New G2G_Analisi

        'quanto sicuramente non assegnato ...
        With rval
            .From_Piva = _Piva
            .From_PivaSuperUser = objParametri.PivaSuperUser
        End With

        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)
            GiasContext.Database.CommandTimeout = 3600
            Dim pcxv = (From pcxva In GiasContext.ParticelleCatastalixVincoliAgronomici
                        Join ixa In GiasContext.ImpreseXParticelle
                                                                    On pcxva.PROV Equals ixa.PROV _
                                                                    And pcxva.COM Equals ixa.COM _
                                                                    And pcxva.SEZIONE Equals ixa.SEZIONE _
                                                                    And pcxva.FOGLIO Equals ixa.FOGLIO _
                                                                    And pcxva.NUMERO Equals ixa.NUMERO _
                                                                    And pcxva.SUBALTERNO Equals ixa.SUBALTERNO
                        Join aet In GiasContext.Analisi_EntitaxTestata On pcxva.Analisi_Testata_Cod Equals aet.Analisi_Testata_Cod
                        Where listaImprese.Contains(ixa.PIVA) _
                        AndAlso pcxva.Validita_Fine >= DataValidita
                        Select aet.Piva).Distinct().ToList()


            For Each Piva In pcxv
                If Not listaImpresePubbliche.Contains(Piva) AndAlso Not listaImprese.Contains(Piva) Then
                    listaImpresePubbliche.Add(Piva)
                End If
            Next
        End Using

        Return rval

    End Function

    Public Function Leggi_AnalisiCondivise_G2GReverse(ByVal _Piva As String, ByVal listaImprese As List(Of String), DataValidita As Date, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, ByRef listaImpresePubbliche As List(Of String)) As G2G_Analisi_Reverse

        Dim nomeRoutine As String = "AgronicaCoreG2GLocalDAL.G2GMateriePrimeCampionature_R.Leggi_AnalisiCondivise_G2GReverse()"
        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Dim rval As New G2G_Analisi_Reverse

        'quanto sicuramente non assegnato ...
        With rval
            .From_Piva = _Piva
            .From_PivaSuperUser = objParametri.PivaSuperUser
        End With

        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)
            GiasContext.Database.CommandTimeout = 3600
            Dim pcxv = (From pcxva In GiasContext.ParticelleCatastalixVincoliAgronomici
                        Join ixa In GiasContext.ImpreseXParticelle
                                                                    On pcxva.PROV Equals ixa.PROV _
                                                                    And pcxva.COM Equals ixa.COM _
                                                                    And pcxva.SEZIONE Equals ixa.SEZIONE _
                                                                    And pcxva.FOGLIO Equals ixa.FOGLIO _
                                                                    And pcxva.NUMERO Equals ixa.NUMERO _
                                                                    And pcxva.SUBALTERNO Equals ixa.SUBALTERNO
                        Join aet In GiasContext.Analisi_EntitaxTestata On pcxva.Analisi_Testata_Cod Equals aet.Analisi_Testata_Cod
                        Where listaImprese.Contains(ixa.PIVA) _
                        AndAlso pcxva.Validita_Fine >= DataValidita
                        Select aet.Piva).Distinct().ToList()


            For Each Piva In pcxv
                If Not listaImpresePubbliche.Contains(Piva) AndAlso Not listaImprese.Contains(Piva) Then
                    listaImpresePubbliche.Add(Piva)
                End If
            Next
        End Using

        Return rval

    End Function

End Class


Public Class G2GAnalisi_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Scrivi_Analisi_G2G(Origine_Piva_SuperUser As String, Destinazione_Piva_SuperUser As String, ByRef g2g As G2G_Analisi, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As G2G_Recode

        Dim nomeRoutine As String = "AgronicaCoreG2GLocalDal.G2GAnalisi_W.Scrivi_Analisi_G2G()"
        Dim messaggioErrore As String = ""
        g2g.Recode = New G2G_Recode

        Dim username = objParametri.UsernameOperazione
        Dim piva = g2g.To_Piva
        Dim data = Date.Now

        Dim objSequenze = New AgronicaCoreDataProvider.Agro_Sequenze

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

                    For Each r As G2G_Recode_Analisi_EntitaxTestata In g2g.G2G_Analisi_EntitaxTestata_Recode_delete

                        '' cancella recode entita
                        Dim recode = (From rr In GiasContext.G2G_Recode_Analisi_EntitaxTestata Where rr.From_PivaSuperUser = Origine_Piva_SuperUser AndAlso rr.To_PivaSuperUser = Destinazione_Piva_SuperUser AndAlso rr.From_Analisi_Testata_Cod = r.From_Analisi_Testata_Cod).FirstOrDefault()
                        If recode IsNot Nothing Then
                            GiasContext.G2G_Recode_Analisi_EntitaxTestata.Attach(recode)
                            GiasContext.G2G_Recode_Analisi_EntitaxTestata.Remove(recode)

                            ''cancella entita
                            Dim analisi_entitas = (From m In GiasContext.Analisi_EntitaxTestata Where m.Analisi_SuperUser = Destinazione_Piva_SuperUser AndAlso m.Analisi_Testata_Cod = r.To_Analisi_Testata_Cod).ToList()
                            For Each analisi_entita In analisi_entitas
                                GiasContext.Analisi_EntitaxTestata.Attach(analisi_entita)
                                GiasContext.Analisi_EntitaxTestata.Remove(analisi_entita)
                            Next
                        End If

                    Next

                    GiasContext.SaveChanges()

                    For Each r As G2G_Recode_Analisi_Dettagli In g2g.G2G_Analisi_Dettagli_Recode_delete

                        '' cancella recode entita
                        Dim recode = (From rr In GiasContext.G2G_Recode_Analisi_Dettagli Where rr.From_PivaSuperUser = Origine_Piva_SuperUser AndAlso rr.To_PivaSuperUser = Destinazione_Piva_SuperUser AndAlso rr.From_Analisi_Testata_Cod = r.From_Analisi_Testata_Cod AndAlso rr.From_Analisi_Dettaglio_Cod = r.From_Analisi_Dettaglio_Cod).FirstOrDefault()
                        If recode IsNot Nothing Then
                            GiasContext.G2G_Recode_Analisi_Dettagli.Attach(recode)
                            GiasContext.G2G_Recode_Analisi_Dettagli.Remove(recode)

                            ''cancella entita
                            Dim analisi_dettagli = (From m In GiasContext.Analisi_Dettagli Where m.Analisi_SuperUser = Destinazione_Piva_SuperUser AndAlso m.Analisi_Testata_Cod = r.To_Analisi_Testata_Cod AndAlso m.Analisi_Dettaglio_Cod = r.To_Analisi_Dettaglio_Cod).FirstOrDefault()
                            GiasContext.Analisi_Dettagli.Attach(analisi_dettagli)
                            GiasContext.Analisi_Dettagli.Remove(analisi_dettagli)
                        End If

                    Next

                    GiasContext.SaveChanges()

                    For Each r As G2G_Recode_Analisi_Testata In g2g.G2G_Analisi_Testata_Recode_delete

                        '' cancella recode entita
                        Dim recode = (From rr In GiasContext.G2G_Recode_Analisi_Testata Where rr.From_PivaSuperUser = Origine_Piva_SuperUser AndAlso rr.To_PivaSuperUser = Destinazione_Piva_SuperUser AndAlso rr.From_Analisi_Testata_Cod = r.From_Analisi_Testata_Cod).FirstOrDefault()
                        If recode IsNot Nothing Then
                            GiasContext.G2G_Recode_Analisi_Testata.Attach(recode)
                            GiasContext.G2G_Recode_Analisi_Testata.Remove(recode)

                            ''cancella entita
                            Dim analisi_testata = (From m In GiasContext.Analisi_Testata Where m.Analisi_SuperUser = Destinazione_Piva_SuperUser AndAlso m.Analisi_Testata_Cod = r.To_Analisi_Testata_Cod).FirstOrDefault()
                            GiasContext.Analisi_Testata.Attach(analisi_testata)
                            GiasContext.Analisi_Testata.Remove(analisi_testata)
                        End If

                    Next

                    GiasContext.SaveChanges()

                    For Each r As G2G_Recode_Analisi_Certificato In g2g.G2G_Analisi_Certificato_Recode_delete

                        ''cancella entita
                        Dim pivaDest As String = g2g.To_Piva
                        Dim analisi_certificato = (From c In GiasContext.Analisi_Certificato
                                                   Join t In GiasContext.Analisi_Testata On t.Analisi_Certificato_Cod Equals c.Analisi_Certificato_Cod
                                                   Join e In GiasContext.Analisi_EntitaxTestata On e.Analisi_Testata_Cod Equals t.Analisi_Testata_Cod
                                                   Where c.Analisi_SuperUser = Destinazione_Piva_SuperUser _
                                                        AndAlso c.Analisi_Certificato_Cod = r.To_Analisi_Certificato_Cod _
                                                        AndAlso e.Piva = pivaDest
                                                   Select c).FirstOrDefault()

                        If Not IsNothing(analisi_certificato) Then
                            GiasContext.Analisi_Certificato.Attach(analisi_certificato)
                            GiasContext.Analisi_Certificato.Remove(analisi_certificato)

                            '' cancella recode entita
                            Dim recode = (From rr In GiasContext.G2G_Recode_Analisi_Certificato Where rr.From_PivaSuperUser = Origine_Piva_SuperUser AndAlso rr.To_PivaSuperUser = Destinazione_Piva_SuperUser AndAlso rr.From_Analisi_Certificato_Cod = r.From_Analisi_Certificato_Cod).FirstOrDefault()
                            GiasContext.G2G_Recode_Analisi_Certificato.Attach(recode)
                            GiasContext.G2G_Recode_Analisi_Certificato.Remove(recode)
                        End If

                    Next

                    GiasContext.SaveChanges()

                    For Each r As G2G_Recode_Analisi_Campioni In g2g.G2G_Analisi_Campioni_delete

                        ''cancella entita
                        Dim pivaDest As String = g2g.To_Piva
                        Dim analisi_campione = (From c In GiasContext.Analisi_Campioni
                                                Join t In GiasContext.Analisi_CampionixDettagli On t.Analisi_Campione_Cod Equals c.Analisi_Campione_Cod
                                                Join e In GiasContext.Analisi_EntitaxTestata On e.Analisi_Testata_Cod Equals t.Analisi_Testata_Cod
                                                Where c.Analisi_SuperUser = Destinazione_Piva_SuperUser _
                                                        AndAlso e.Piva = pivaDest _
                                                    AndAlso c.Analisi_Campione_Cod = r.To_Analisi_Campione_Cod
                                                Select c).FirstOrDefault()

                        If Not IsNothing(analisi_campione) Then

                            Dim analisi_campionidetts = (From c In GiasContext.Analisi_CampionixDettagli Where c.Analisi_Campione_Cod = analisi_campione.Analisi_Campione_Cod).ToList
                            For Each analisi_campionedettaglio In analisi_campionidetts
                                GiasContext.Analisi_CampionixDettagli.Attach(analisi_campionedettaglio)
                                GiasContext.Analisi_CampionixDettagli.Remove(analisi_campionedettaglio)
                            Next

                            GiasContext.Analisi_Campioni.Attach(analisi_campione)
                            GiasContext.Analisi_Campioni.Remove(analisi_campione)

                            '' cancella recode entita
                            Dim recode = (From rr In GiasContext.G2G_Recode_Analisi_Campioni Where rr.From_PivaSuperUser = Origine_Piva_SuperUser AndAlso rr.To_PivaSuperUser = Destinazione_Piva_SuperUser _
                                                                                                 AndAlso rr.From_Analisi_Campione_Cod = r.From_Analisi_Campione_Cod).FirstOrDefault()
                            GiasContext.G2G_Recode_Analisi_Campioni.Attach(recode)
                            GiasContext.G2G_Recode_Analisi_Campioni.Remove(recode)
                        End If

                    Next

                    GiasContext.SaveChanges()

                    'CERTIFICATI
                    For Each m As Analisi_Certificato In g2g.analisi_certificato_update

                        Dim recode = (From rr In GiasContext.G2G_Recode_Analisi_Certificato Where rr.From_PivaSuperUser = Origine_Piva_SuperUser AndAlso rr.To_PivaSuperUser = Destinazione_Piva_SuperUser AndAlso rr.From_Analisi_Certificato_Cod = m.Analisi_Certificato_Cod).FirstOrDefault()
                        recode.Username_Modifica = username
                        recode.Data_Modifica = data
                        recode.datainvio = data
                        g2g.G2G_Analisi_Certificato_Recode_update.Add(recode)
                        GiasContext.G2G_Recode_Analisi_Certificato.Attach(recode)
                        GiasContext.Entry(recode).State = EntityState.Modified

                        Dim certificato = (From mm In GiasContext.Analisi_Certificato Where mm.Analisi_Certificato_Cod = recode.To_Analisi_Certificato_Cod).FirstOrDefault()
                        certificato = Gias_EF_Utility.CopyEntity(GiasContext, m, certificato, username, data)
                        certificato.Analisi_SuperUser = Destinazione_Piva_SuperUser
                        certificato.Analisi_Certificato_Cod = recode.To_Analisi_Certificato_Cod
                        GiasContext.Analisi_Certificato.Attach(certificato)
                        GiasContext.Entry(certificato).State = EntityState.Modified

                    Next

                    GiasContext.SaveChanges()

                    If g2g.analisi_certificato_insert.Count > 0 Then

                        For Each m As Analisi_Certificato In g2g.analisi_certificato_insert

                            Dim idSeq As Integer = 0

                            'Richiedo un nuovo id sequenza
                            'idSeq = objSequenze.NuovoId_Tabella_EF(GiasContext, "ANALISI_CERTIFICATO", 0, 2000000000, objParametri)
                            idSeq = objSequenze.NuovoId_Tabella("ANALISI_CERTIFICATO", 0, 2000000000, objParametri, UtilizzaTransazione:=False)

                            Dim certificato = Gias_EF_Utility.CopyEntity(GiasContext, m, Nothing, username, data)

                            certificato.Analisi_Certificato_Cod = idSeq
                            certificato.Analisi_SuperUser = Destinazione_Piva_SuperUser

                            If certificato.Analisi_Certificato_Laboratorio <> "" AndAlso certificato.Analisi_Certificato_Laboratorio <> "0" AndAlso IsNumeric(certificato.Analisi_Certificato_Laboratorio) Then

                                Dim G2GrisUm_Recode = (From g2gContatti In GiasContext.G2G_Recode_Contatti Where g2gContatti.From_PivaSuperUser = Origine_Piva_SuperUser _
                                                                                                            And g2gContatti.From_Cod_RisUm = m.Analisi_Certificato_Laboratorio _
                                                                                                            And g2gContatti.To_PivaSuperUser = Destinazione_Piva_SuperUser).FirstOrDefault

                                If G2GrisUm_Recode Is Nothing Then
                                    Throw New Exception("Analisi_Certificato_Laboratorio non mappato. non è stato trovato il CodRisum " & certificato.Analisi_Certificato_Laboratorio)
                                End If

                                certificato.Analisi_Certificato_Laboratorio = G2GrisUm_Recode.To_Cod_Risum

                            End If

                            GiasContext.Analisi_Certificato.Add(certificato)

                            Dim recode =
                            New G2G_Recode_Analisi_Certificato With {
                                .From_PivaSuperUser = Origine_Piva_SuperUser,
                                .To_PivaSuperUser = Destinazione_Piva_SuperUser,
                                .From_Analisi_Certificato_Cod = m.Analisi_Certificato_Cod,
                                .To_Analisi_Certificato_Cod = certificato.Analisi_Certificato_Cod,
                                .Username_Creazione = username,
                                .Username_Modifica = username,
                                .Data_Creazione = Now(),
                                .Data_Modifica = Now(),
                                .Validita_Inizio = AGRODATAINIZIO,
                                .Validita_Fine = AGRODATAFINE,
                                .inviato = "0",
                                .datainvio = Now()
                            }
                            g2g.G2G_Analisi_Certificato_Recode_insert.Add(recode)
                            GiasContext.G2G_Recode_Analisi_Certificato.Add(recode)

                        Next

                        GiasContext.SaveChanges()

                    End If

                    'TESTATA
                    For Each m As Analisi_Testata In g2g.analisi_testata_update

                        Dim recode = (From rr In GiasContext.G2G_Recode_Analisi_Testata Where rr.From_PivaSuperUser = Origine_Piva_SuperUser AndAlso rr.To_PivaSuperUser = Destinazione_Piva_SuperUser AndAlso rr.From_Analisi_Testata_Cod = m.Analisi_Testata_Cod).FirstOrDefault()
                        recode.Username_Modifica = username
                        recode.Data_Modifica = data
                        recode.datainvio = data
                        g2g.G2G_Analisi_Testata_Recode_update.Add(recode)
                        GiasContext.G2G_Recode_Analisi_Testata.Attach(recode)
                        GiasContext.Entry(recode).State = EntityState.Modified

                        Dim testata = (From mm In GiasContext.Analisi_Testata Where mm.Analisi_Testata_Cod = recode.To_Analisi_Testata_Cod).FirstOrDefault()
                        testata = Gias_EF_Utility.CopyEntity(GiasContext, m, testata, username, data)
                        testata.Analisi_SuperUser = Destinazione_Piva_SuperUser
                        testata.Analisi_Testata_Cod = recode.To_Analisi_Testata_Cod
                        If testata.Analisi_Certificato_Cod <> 0 Then
                            Dim certificato = (From m1 In GiasContext.G2G_Recode_Analisi_Certificato Where m1.To_PivaSuperUser = Destinazione_Piva_SuperUser And m1.From_Analisi_Certificato_Cod = testata.Analisi_Certificato_Cod Select m1).FirstOrDefault
                            If certificato Is Nothing Then
                                Throw New Exception("Errore, non esiste la decodifica di Analisi_Certificato_Cod = " & testata.Analisi_Certificato_Cod & " in G2G_Recode_Analisi_Certificato")
                            Else
                                testata.Analisi_Certificato_Cod = certificato.To_Analisi_Certificato_Cod
                            End If
                        End If
                        GiasContext.Analisi_Testata.Attach(testata)
                        GiasContext.Entry(testata).State = EntityState.Modified

                    Next

                    GiasContext.SaveChanges()

                    If g2g.analisi_testata_insert.Count > 0 Then

                        For Each s As Analisi_Testata In g2g.analisi_testata_insert

                            Dim idSeq As Integer = 0


                            'Richiedo un nuovo id sequenza
                            'idSeq = objSequenze.NuovoId_Tabella_EF(GiasContext, "ANALISI_TESTATA", 0, 2000000000, objParametri)
                            idSeq = objSequenze.NuovoId_Tabella("ANALISI_TESTATA", 0, 2000000000, objParametri, UtilizzaTransazione:=False)

                            Dim analisi_testata = Gias_EF_Utility.CopyEntity(GiasContext, s, Nothing, username, data)

                            analisi_testata.Analisi_Testata_Cod = idSeq
                            analisi_testata.Analisi_SuperUser = Destinazione_Piva_SuperUser

                            If analisi_testata.Analisi_Certificato_Cod <> 0 Then
                                Dim recodeCertificato = (From c In GiasContext.G2G_Recode_Analisi_Certificato Where c.From_PivaSuperUser = Origine_Piva_SuperUser And
                                                                                                                      c.To_PivaSuperUser = Destinazione_Piva_SuperUser And
                                                                                                                      c.From_Analisi_Certificato_Cod = s.Analisi_Certificato_Cod).FirstOrDefault
                                If recodeCertificato IsNot Nothing Then
                                    analisi_testata.Analisi_Certificato_Cod = recodeCertificato.To_Analisi_Certificato_Cod
                                End If
                            End If

                            GiasContext.Analisi_Testata.Add(analisi_testata)

                            Dim recode =
                                    New G2G_Recode_Analisi_Testata With {
                                        .From_PivaSuperUser = Origine_Piva_SuperUser,
                                        .To_PivaSuperUser = Destinazione_Piva_SuperUser,
                                        .From_Analisi_Testata_Cod = s.Analisi_Testata_Cod,
                                        .To_Analisi_Testata_Cod = analisi_testata.Analisi_Testata_Cod,
                                        .Username_Creazione = username,
                                        .Username_Modifica = username,
                                        .Data_Creazione = Now(),
                                        .Data_Modifica = Now(),
                                        .Validita_Inizio = AGRODATAINIZIO,
                                        .Validita_Fine = AGRODATAFINE,
                                        .inviato = "0",
                                        .datainvio = Now()
                                    }
                            g2g.G2G_Analisi_Testata_Recode_insert.Add(recode)
                            GiasContext.G2G_Recode_Analisi_Testata.Add(recode)



                        Next

                        GiasContext.SaveChanges()

                    End If

                    'DETTAGLI
                    For Each m As Analisi_Dettagli In g2g.analisi_dettagli_update

                        Dim recode = (From rr In GiasContext.G2G_Recode_Analisi_Dettagli Where rr.From_PivaSuperUser = Origine_Piva_SuperUser AndAlso rr.To_PivaSuperUser = Destinazione_Piva_SuperUser AndAlso rr.From_Analisi_Testata_Cod = m.Analisi_Testata_Cod AndAlso rr.From_Analisi_Dettaglio_Cod = m.Analisi_Dettaglio_Cod).FirstOrDefault()
                        recode.Username_Modifica = username
                        recode.Data_Modifica = data
                        recode.datainvio = data
                        g2g.G2G_Analisi_Dettagli_Recode_update.Add(recode)
                        GiasContext.G2G_Recode_Analisi_Dettagli.Attach(recode)
                        GiasContext.Entry(recode).State = EntityState.Modified

                        Dim dettaglio = (From mm In GiasContext.Analisi_Dettagli Where mm.Analisi_Testata_Cod = recode.To_Analisi_Testata_Cod AndAlso mm.Analisi_Dettaglio_Cod = recode.To_Analisi_Dettaglio_Cod).FirstOrDefault()
                        dettaglio = Gias_EF_Utility.CopyEntity(GiasContext, m, dettaglio, username, data)
                        dettaglio.Analisi_SuperUser = Destinazione_Piva_SuperUser
                        dettaglio.Analisi_Testata_Cod = recode.To_Analisi_Testata_Cod
                        dettaglio.Analisi_Dettaglio_Cod = recode.To_Analisi_Dettaglio_Cod
                        GiasContext.Analisi_Dettagli.Attach(dettaglio)
                        GiasContext.Entry(dettaglio).State = EntityState.Modified

                    Next

                    GiasContext.SaveChanges()

                    For Each s As Analisi_Dettagli In g2g.analisi_dettagli_insert

                        Dim idSeq As Integer = 0

                        Dim analisi_dettagli = Gias_EF_Utility.CopyEntity(GiasContext, s, Nothing, username, data)

                        If analisi_dettagli.Analisi_Dettaglio_Cod <> 0 Then
                            'Richiedo un nuovo id sequenza
                            'idSeq = objSequenze.NuovoId_Tabella_EF(GiasContext, "ANALISI_DETTAGLI", 0, 2000000000, objParametri)
                            idSeq = objSequenze.NuovoId_Tabella("ANALISI_DETTAGLI", 0, 2000000000, objParametri, UtilizzaTransazione:=False)
                            analisi_dettagli.Analisi_Dettaglio_Cod = idSeq
                        End If

                        analisi_dettagli.Analisi_SuperUser = Destinazione_Piva_SuperUser

                        analisi_dettagli.Analisi_Testata_Cod = (From c In GiasContext.G2G_Recode_Analisi_Testata Where c.From_PivaSuperUser = Origine_Piva_SuperUser And
                                                                                                                        c.To_PivaSuperUser = Destinazione_Piva_SuperUser And
                                                                                                                        c.From_Analisi_Testata_Cod = s.Analisi_Testata_Cod).FirstOrDefault.To_Analisi_Testata_Cod


                        GiasContext.Analisi_Dettagli.Add(analisi_dettagli)

                        Dim recode =
                                New G2G_Recode_Analisi_Dettagli With {
                                    .From_PivaSuperUser = Origine_Piva_SuperUser,
                                    .To_PivaSuperUser = Destinazione_Piva_SuperUser,
                                    .From_Analisi_Testata_Cod = s.Analisi_Testata_Cod,
                                    .To_Analisi_Testata_Cod = analisi_dettagli.Analisi_Testata_Cod,
                                    .From_Analisi_Dettaglio_Cod = s.Analisi_Dettaglio_Cod,
                                    .To_Analisi_Dettaglio_Cod = analisi_dettagli.Analisi_Dettaglio_Cod,
                                    .Username_Creazione = username,
                                    .Username_Modifica = username,
                                    .Data_Creazione = Now(),
                                    .Data_Modifica = Now(),
                                    .Validita_Inizio = AGRODATAINIZIO,
                                    .Validita_Fine = AGRODATAFINE,
                                    .inviato = "0",
                                    .datainvio = Now()
                                }
                        g2g.G2G_Analisi_Dettagli_Recode_insert.Add(recode)
                        GiasContext.G2G_Recode_Analisi_Dettagli.Add(recode)

                    Next

                    GiasContext.SaveChanges()

                    'ENTITAXTESTATA
                    For Each s As Analisi_EntitaxTestata In g2g.analisi_entitaxtestata_insert

                        'Richiedo un nuovo id sequenza
                        Dim analisi_entitaxtestata = Gias_EF_Utility.CopyEntity(GiasContext, s, Nothing, username, data)

                        analisi_entitaxtestata.Analisi_Testata_Cod = (From c In GiasContext.G2G_Recode_Analisi_Testata Where c.From_PivaSuperUser = Origine_Piva_SuperUser And
                                                                                                                        c.To_PivaSuperUser = Destinazione_Piva_SuperUser And
                                                                                                                        c.From_Analisi_Testata_Cod = s.Analisi_Testata_Cod).FirstOrDefault.To_Analisi_Testata_Cod


                        analisi_entitaxtestata.Analisi_SuperUser = Destinazione_Piva_SuperUser
                        analisi_entitaxtestata.Piva = piva

                        GiasContext.Analisi_EntitaxTestata.Add(analisi_entitaxtestata)

                        If (From rec In GiasContext.G2G_Recode_Analisi_EntitaxTestata Where rec.From_PivaSuperUser = s.Analisi_SuperUser _
                                                                                                    And rec.From_Analisi_Testata_Cod = s.Analisi_Testata_Cod _
                                                                                                    And rec.From_Piva = s.Piva _
                                                                                                    And rec.To_Analisi_Testata_Cod = analisi_entitaxtestata.Analisi_Testata_Cod _
                                                                                                    And rec.To_PivaSuperUser = analisi_entitaxtestata.Analisi_SuperUser _
                                                                                                    And rec.To_Piva = analisi_entitaxtestata.Piva).FirstOrDefault Is Nothing Then

                            Dim recode =
                                New G2G_Recode_Analisi_EntitaxTestata With {
                                    .From_PivaSuperUser = Origine_Piva_SuperUser,
                                    .To_PivaSuperUser = Destinazione_Piva_SuperUser,
                                    .From_Analisi_Testata_Cod = s.Analisi_Testata_Cod,
                                    .To_Analisi_Testata_Cod = analisi_entitaxtestata.Analisi_Testata_Cod,
                                    .From_Piva = s.Piva,
                                    .To_Piva = piva,
                                    .Username_Creazione = username,
                                    .Username_Modifica = username,
                                    .Data_Creazione = Now(),
                                    .Data_Modifica = Now(),
                                    .Validita_Inizio = AGRODATAINIZIO,
                                    .Validita_Fine = AGRODATAFINE,
                                    .inviato = "0",
                                    .datainvio = Now()
                                }
                            g2g.G2G_Analisi_EntitaxTestata_Recode_insert.Add(recode)
                            GiasContext.G2G_Recode_Analisi_EntitaxTestata.Add(recode)
                            GiasContext.SaveChanges()
                        End If

                    Next

                    GiasContext.SaveChanges()

                    'CAMPIONI
                    For Each m As Analisi_Campioni In g2g.analisi_campioni_update

                        Dim recode = (From rr In GiasContext.G2G_Recode_Analisi_Campioni Where rr.From_PivaSuperUser = Origine_Piva_SuperUser AndAlso rr.To_PivaSuperUser = Destinazione_Piva_SuperUser AndAlso rr.From_Analisi_Campione_Cod = m.Analisi_Campione_Cod).FirstOrDefault()
                        recode.Username_Modifica = username
                        recode.Data_Modifica = data
                        recode.datainvio = data
                        g2g.G2G_Analisi_Campioni_update.Add(recode)
                        GiasContext.G2G_Recode_Analisi_Campioni.Attach(recode)
                        GiasContext.Entry(recode).State = EntityState.Modified

                        Dim campione = (From mm In GiasContext.Analisi_Campioni Where mm.Analisi_Campione_Cod = recode.To_Analisi_Campione_Cod).FirstOrDefault()
                        campione = Gias_EF_Utility.CopyEntity(GiasContext, m, campione, username, data)
                        campione.Analisi_SuperUser = Destinazione_Piva_SuperUser
                        campione.Analisi_Campione_Cod = recode.To_Analisi_Campione_Cod
                        GiasContext.Analisi_Campioni.Attach(campione)
                        GiasContext.Entry(campione).State = EntityState.Modified

                        Dim analisi_dettaglis = (From ad In GiasContext.Analisi_CampionixDettagli Where ad.Analisi_Campione_Cod = recode.To_Analisi_Campione_Cod).ToList
                        For Each analisi_dettaglio In analisi_dettaglis
                            GiasContext.Analisi_CampionixDettagli.Attach(analisi_dettaglio)
                            GiasContext.Analisi_CampionixDettagli.Remove(analisi_dettaglio)
                        Next

                    Next

                    GiasContext.SaveChanges()

                    For Each s As Analisi_Campioni In g2g.analisi_campioni_insert

                        Dim idSeq As Integer = 0

                        Dim analisi_campioni = Gias_EF_Utility.CopyEntity(GiasContext, s, Nothing, username, data)

                        If analisi_campioni.Analisi_Campione_Cod <> 0 Then
                            'Richiedo un nuovo id sequenza
                            'idSeq = objSequenze.NuovoId_Tabella_EF(GiasContext, "ANALISI_CAMPIONI", 0, 2000000000, objParametri)
                            idSeq = objSequenze.NuovoId_Tabella("ANALISI_CAMPIONI", 0, 2000000000, objParametri, UtilizzaTransazione:=False)
                            analisi_campioni.Analisi_Campione_Cod = idSeq
                        End If

                        analisi_campioni.Analisi_SuperUser = Destinazione_Piva_SuperUser


                        GiasContext.Analisi_Campioni.Add(analisi_campioni)

                        Dim recode =
                                New G2G_Recode_Analisi_Campioni With {
                                    .From_PivaSuperUser = Origine_Piva_SuperUser,
                                    .To_PivaSuperUser = Destinazione_Piva_SuperUser,
                                    .From_Analisi_Campione_Cod = s.Analisi_Campione_Cod,
                                    .To_Analisi_Campione_Cod = analisi_campioni.Analisi_Campione_Cod,
                                    .Username_Creazione = username,
                                    .Username_Modifica = username,
                                    .Data_Creazione = Now(),
                                    .Data_Modifica = Now(),
                                    .Validita_Inizio = AGRODATAINIZIO,
                                    .Validita_Fine = AGRODATAFINE,
                                    .inviato = "0",
                                    .datainvio = Now()
                                }
                        g2g.G2G_Analisi_Campioni_insert.Add(recode)
                        GiasContext.G2G_Recode_Analisi_Campioni.Add(recode)



                    Next

                    GiasContext.SaveChanges()

                    'CAMPIONIXDETTAGLI
                    For Each s As Analisi_CampionixDettagli In g2g.analisi_campionixdettagli_insert

                        Dim idSeq As Integer = 0

                        Dim analisi_dettagli = Gias_EF_Utility.CopyEntity(GiasContext, s, Nothing, username, data)

                        analisi_dettagli.Analisi_SuperUser = Destinazione_Piva_SuperUser
                        If analisi_dettagli.Analisi_Testata_Cod <> 0 Then
                            analisi_dettagli.Analisi_Testata_Cod = (From g In GiasContext.G2G_Recode_Analisi_Testata Where g.From_PivaSuperUser = s.Analisi_SuperUser And g.To_PivaSuperUser = Destinazione_Piva_SuperUser And g.From_Analisi_Testata_Cod = s.Analisi_Testata_Cod).FirstOrDefault.To_Analisi_Testata_Cod
                        End If

                        If analisi_dettagli.Analisi_Dettaglio_Cod <> 0 Then
                            analisi_dettagli.Analisi_Dettaglio_Cod = (From g In GiasContext.G2G_Recode_Analisi_Dettagli Where g.From_PivaSuperUser = s.Analisi_SuperUser And g.To_PivaSuperUser = Destinazione_Piva_SuperUser _
                                                                                                                            And g.From_Analisi_Testata_Cod = s.Analisi_Testata_Cod _
                                                                                                                            And g.From_Analisi_Dettaglio_Cod = s.Analisi_Dettaglio_Cod).FirstOrDefault.To_Analisi_Dettaglio_Cod
                        End If

                        If analisi_dettagli.Analisi_Campione_Cod <> 0 Then
                            analisi_dettagli.Analisi_Campione_Cod = (From g In GiasContext.G2G_Recode_Analisi_Campioni Where g.From_PivaSuperUser = s.Analisi_SuperUser And g.To_PivaSuperUser = Destinazione_Piva_SuperUser _
                                                                                                                            And g.From_Analisi_Campione_Cod = s.Analisi_Campione_Cod).FirstOrDefault.To_Analisi_Campione_Cod
                        End If


                        GiasContext.Analisi_CampionixDettagli.Add(analisi_dettagli)

                    Next

                    GiasContext.SaveChanges()

                    ' COMIT Effettivo
                    scope.Complete()

                    g2g.Recode.G2GRecodeAnalisi_CertificatoToInsert = g2g.G2G_Analisi_Certificato_Recode_insert
                    g2g.Recode.G2GRecodeAnalisi_CertificatoToUpdate = g2g.G2G_Analisi_Certificato_Recode_update
                    g2g.Recode.G2GRecodeAnalisi_CertificatoToDelete = g2g.G2G_Analisi_Certificato_Recode_delete

                    g2g.Recode.G2GRecodeAnalisi_TestataToInsert = g2g.G2G_Analisi_Testata_Recode_insert
                    g2g.Recode.G2GRecodeAnalisi_TestataToUpdate = g2g.G2G_Analisi_Testata_Recode_update
                    g2g.Recode.G2GRecodeAnalisi_TestataToDelete = g2g.G2G_Analisi_Testata_Recode_delete

                    g2g.Recode.G2GRecodeAnalisi_DettagliToInsert = g2g.G2G_Analisi_Dettagli_Recode_insert
                    g2g.Recode.G2GRecodeAnalisi_DettagliToUpdate = g2g.G2G_Analisi_Dettagli_Recode_update
                    g2g.Recode.G2GRecodeAnalisi_DettagliToDelete = g2g.G2G_Analisi_Dettagli_Recode_delete

                    g2g.Recode.G2GRecodeAnalisi_EntitaxTestataToInsert = g2g.G2G_Analisi_EntitaxTestata_Recode_insert
                    'g2g.Recode.G2GRecodeAnalisi_EntitaxTestataToUpdate = g2g.G2G_Analisi_EntitaxTestata_Recode_insert
                    g2g.Recode.G2GRecodeAnalisi_EntitaxTestataToDelete = g2g.G2G_Analisi_EntitaxTestata_Recode_delete

                    g2g.Recode.G2GRecodeAnalisi_CampioniToInsert = g2g.G2G_Analisi_Campioni_insert
                    g2g.Recode.G2GRecodeAnalisi_CampioniToUpdate = g2g.G2G_Analisi_Campioni_update
                    g2g.Recode.G2GRecodeAnalisi_CampioniToDelete = g2g.G2G_Analisi_Campioni_delete

                End Using
            End Using

        Catch ex As Exception

            messaggioErrore = ex.Message
            g2g.Recode.MessaggioErrore = messaggioErrore
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            'Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore, ex)

        End Try

        Return g2g.Recode

    End Function

    Public Function Scrivi_Analisi_G2GReverse(Origine_Piva_SuperUser As String, Destinazione_Piva_SuperUser As String, ByRef g2g As G2G_Analisi_Reverse, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As G2G_Recode

        Dim nomeRoutine As String = "AgronicaCoreG2GLocalDal.G2GAnalisi_W.Scrivi_Analisi_G2GReverse()"
        Dim messaggioErrore As String = ""
        g2g.Recode = New G2G_Recode

        Dim username = objParametri.UsernameOperazione
        Dim piva = g2g.To_Piva
        Dim data = Date.Now

        Dim objSequenze = New AgronicaCoreDataProvider.Agro_Sequenze

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

                    For Each r As G2G_Recode_Analisi_EntitaxTestata In g2g.G2G_Analisi_EntitaxTestata_Recode_delete

                        '' cancella recode entita
                        Dim recode = (From rr In GiasContext.G2G_Recode_Analisi_EntitaxTestata Where rr.To_PivaSuperUser = Origine_Piva_SuperUser AndAlso rr.From_PivaSuperUser = Destinazione_Piva_SuperUser AndAlso rr.To_Analisi_Testata_Cod = r.To_Analisi_Testata_Cod).FirstOrDefault()
                        If recode IsNot Nothing Then
                            'GiasContext.G2G_Recode_Analisi_EntitaxTestata.Attach(recode)
                            GiasContext.G2G_Recode_Analisi_EntitaxTestata.Remove(recode)

                            ''cancella entita
                            Dim analisi_entitas = (From m In GiasContext.Analisi_EntitaxTestata Where m.Analisi_SuperUser = Destinazione_Piva_SuperUser AndAlso m.Analisi_Testata_Cod = r.From_Analisi_Testata_Cod).ToList()
                            For Each analisi_entita In analisi_entitas
                                GiasContext.Analisi_EntitaxTestata.Attach(analisi_entita)
                                GiasContext.Analisi_EntitaxTestata.Remove(analisi_entita)
                            Next
                        End If

                    Next

                    GiasContext.SaveChanges()

                    For Each r As G2G_Recode_Analisi_Dettagli In g2g.G2G_Analisi_Dettagli_Recode_delete

                        '' cancella recode entita
                        Dim recode = (From rr In GiasContext.G2G_Recode_Analisi_Dettagli Where rr.To_PivaSuperUser = Origine_Piva_SuperUser AndAlso rr.From_PivaSuperUser = Destinazione_Piva_SuperUser AndAlso rr.To_Analisi_Testata_Cod = r.To_Analisi_Testata_Cod AndAlso rr.To_Analisi_Dettaglio_Cod = r.To_Analisi_Dettaglio_Cod).FirstOrDefault()
                        If recode IsNot Nothing Then
                            GiasContext.G2G_Recode_Analisi_Dettagli.Attach(recode)
                            GiasContext.G2G_Recode_Analisi_Dettagli.Remove(recode)

                            ''cancella entita
                            Dim analisi_dettagli = (From m In GiasContext.Analisi_Dettagli Where m.Analisi_SuperUser = Destinazione_Piva_SuperUser AndAlso m.Analisi_Testata_Cod = r.From_Analisi_Testata_Cod AndAlso m.Analisi_Dettaglio_Cod = r.From_Analisi_Dettaglio_Cod).FirstOrDefault()
                            GiasContext.Analisi_Dettagli.Attach(analisi_dettagli)
                            GiasContext.Analisi_Dettagli.Remove(analisi_dettagli)
                        End If

                    Next

                    GiasContext.SaveChanges()

                    For Each r As G2G_Recode_Analisi_Testata In g2g.G2G_Analisi_Testata_Recode_delete

                        '' cancella recode entita
                        Dim recode = (From rr In GiasContext.G2G_Recode_Analisi_Testata Where rr.To_PivaSuperUser = Origine_Piva_SuperUser AndAlso rr.From_PivaSuperUser = Destinazione_Piva_SuperUser AndAlso rr.To_Analisi_Testata_Cod = r.To_Analisi_Testata_Cod).FirstOrDefault()
                        If recode IsNot Nothing Then
                            GiasContext.G2G_Recode_Analisi_Testata.Attach(recode)
                            GiasContext.G2G_Recode_Analisi_Testata.Remove(recode)

                            ''cancella entita
                            Dim analisi_testata = (From m In GiasContext.Analisi_Testata Where m.Analisi_SuperUser = Destinazione_Piva_SuperUser AndAlso m.Analisi_Testata_Cod = r.From_Analisi_Testata_Cod).FirstOrDefault()
                            GiasContext.Analisi_Testata.Attach(analisi_testata)
                            GiasContext.Analisi_Testata.Remove(analisi_testata)
                        End If

                    Next

                    GiasContext.SaveChanges()

                    For Each r As G2G_Recode_Analisi_Certificato In g2g.G2G_Analisi_Certificato_Recode_delete

                        ''cancella entita
                        Dim pivaDest As String = g2g.To_Piva
                        Dim analisi_certificato = (From c In GiasContext.Analisi_Certificato
                                                   Join t In GiasContext.Analisi_Testata On t.Analisi_Certificato_Cod Equals c.Analisi_Certificato_Cod
                                                   Join e In GiasContext.Analisi_EntitaxTestata On e.Analisi_Testata_Cod Equals t.Analisi_Testata_Cod
                                                   Where c.Analisi_SuperUser = Destinazione_Piva_SuperUser _
                                                        AndAlso c.Analisi_Certificato_Cod = r.From_Analisi_Certificato_Cod _
                                                        AndAlso e.Piva = pivaDest
                                                   Select c).FirstOrDefault()

                        If Not IsNothing(analisi_certificato) Then
                            GiasContext.Analisi_Certificato.Attach(analisi_certificato)
                            GiasContext.Analisi_Certificato.Remove(analisi_certificato)

                            '' cancella recode entita
                            Dim recode = (From rr In GiasContext.G2G_Recode_Analisi_Certificato Where rr.To_PivaSuperUser = Origine_Piva_SuperUser AndAlso rr.From_PivaSuperUser = Destinazione_Piva_SuperUser AndAlso rr.To_Analisi_Certificato_Cod = r.To_Analisi_Certificato_Cod).FirstOrDefault()
                            GiasContext.G2G_Recode_Analisi_Certificato.Attach(recode)
                            GiasContext.G2G_Recode_Analisi_Certificato.Remove(recode)
                        End If

                    Next

                    GiasContext.SaveChanges()

                    For Each r As G2G_Recode_Analisi_Campioni In g2g.G2G_Analisi_Campioni_delete

                        ''cancella entita
                        Dim pivaDest As String = g2g.To_Piva
                        Dim analisi_campione = (From c In GiasContext.Analisi_Campioni
                                                Join t In GiasContext.Analisi_CampionixDettagli On t.Analisi_Campione_Cod Equals c.Analisi_Campione_Cod
                                                Join e In GiasContext.Analisi_EntitaxTestata On e.Analisi_Testata_Cod Equals t.Analisi_Testata_Cod
                                                Where c.Analisi_SuperUser = Destinazione_Piva_SuperUser _
                                                        AndAlso e.Piva = pivaDest _
                                                    AndAlso c.Analisi_Campione_Cod = r.From_Analisi_Campione_Cod
                                                Select c).FirstOrDefault()

                        If Not IsNothing(analisi_campione) Then

                            Dim analisi_campionidetts = (From c In GiasContext.Analisi_CampionixDettagli Where c.Analisi_Campione_Cod = analisi_campione.Analisi_Campione_Cod).ToList
                            For Each analisi_campionedettaglio In analisi_campionidetts
                                GiasContext.Analisi_CampionixDettagli.Attach(analisi_campionedettaglio)
                                GiasContext.Analisi_CampionixDettagli.Remove(analisi_campionedettaglio)
                            Next

                            GiasContext.Analisi_Campioni.Attach(analisi_campione)
                            GiasContext.Analisi_Campioni.Remove(analisi_campione)

                            '' cancella recode entita
                            Dim recode = (From rr In GiasContext.G2G_Recode_Analisi_Campioni Where rr.To_PivaSuperUser = Origine_Piva_SuperUser AndAlso rr.From_PivaSuperUser = Destinazione_Piva_SuperUser _
                                                                                                 AndAlso rr.To_Analisi_Campione_Cod = r.To_Analisi_Campione_Cod).FirstOrDefault()
                            GiasContext.G2G_Recode_Analisi_Campioni.Attach(recode)
                            GiasContext.G2G_Recode_Analisi_Campioni.Remove(recode)
                        End If

                    Next

                    GiasContext.SaveChanges()

                    'CERTIFICATI
                    For Each m As Analisi_Certificato In g2g.analisi_certificato_update

                        Dim recode = (From rr In GiasContext.G2G_Recode_Analisi_Certificato Where rr.To_PivaSuperUser = Origine_Piva_SuperUser AndAlso rr.From_PivaSuperUser = Destinazione_Piva_SuperUser AndAlso rr.To_Analisi_Certificato_Cod = m.Analisi_Certificato_Cod).FirstOrDefault()
                        recode.Username_Modifica = username
                        recode.Data_Modifica = data
                        recode.datainvio = data
                        g2g.G2G_Analisi_Certificato_Recode_update.Add(recode)
                        GiasContext.G2G_Recode_Analisi_Certificato.Attach(recode)
                        GiasContext.Entry(recode).State = EntityState.Modified

                        Dim certificato = (From mm In GiasContext.Analisi_Certificato Where mm.Analisi_Certificato_Cod = recode.From_Analisi_Certificato_Cod).FirstOrDefault()
                        certificato = Gias_EF_Utility.CopyEntity(GiasContext, m, certificato, username, data)
                        certificato.Analisi_SuperUser = Destinazione_Piva_SuperUser
                        certificato.Analisi_Certificato_Cod = recode.From_Analisi_Certificato_Cod
                        GiasContext.Analisi_Certificato.Attach(certificato)
                        GiasContext.Entry(certificato).State = EntityState.Modified

                    Next

                    GiasContext.SaveChanges()

                    If g2g.analisi_certificato_insert.Count > 0 Then

                        For Each m As Analisi_Certificato In g2g.analisi_certificato_insert

                            Dim idSeq As Integer = 0

                            'Richiedo un nuovo id sequenza
                            'idSeq = objSequenze.NuovoId_Tabella_EF(GiasContext, "ANALISI_CERTIFICATO", 0, 2000000000, objParametri)
                            idSeq = objSequenze.NuovoId_Tabella("ANALISI_CERTIFICATO", 0, 2000000000, objParametri, UtilizzaTransazione:=False)

                            Dim certificato = Gias_EF_Utility.CopyEntity(GiasContext, m, Nothing, username, data)

                            certificato.Analisi_Certificato_Cod = idSeq
                            certificato.Analisi_SuperUser = Destinazione_Piva_SuperUser

                            If certificato.Analisi_Certificato_Laboratorio <> "" AndAlso certificato.Analisi_Certificato_Laboratorio <> "0" AndAlso IsNumeric(certificato.Analisi_Certificato_Laboratorio) Then

                                Dim G2GrisUm_Recode = (From g2gContatti In GiasContext.G2G_Recode_Contatti Where g2gContatti.To_PivaSuperUser = Origine_Piva_SuperUser _
                                                                                                            And g2gContatti.To_Cod_Risum = m.Analisi_Certificato_Laboratorio _
                                                                                                            And g2gContatti.From_PivaSuperUser = Destinazione_Piva_SuperUser).FirstOrDefault

                                If G2GrisUm_Recode Is Nothing Then
                                    Throw New Exception("Analisi_Certificato_Laboratorio non mappato. non è stato trovato il CodRisum " & certificato.Analisi_Certificato_Laboratorio)
                                End If

                                certificato.Analisi_Certificato_Laboratorio = G2GrisUm_Recode.From_Cod_RisUm

                            End If

                            GiasContext.Analisi_Certificato.Add(certificato)

                            Dim recode =
                            New G2G_Recode_Analisi_Certificato With {
                                .From_PivaSuperUser = Destinazione_Piva_SuperUser,
                                .To_PivaSuperUser = Origine_Piva_SuperUser,
                                .From_Analisi_Certificato_Cod = certificato.Analisi_Certificato_Cod,
                                .To_Analisi_Certificato_Cod = m.Analisi_Certificato_Cod,
                                .Username_Creazione = username,
                                .Username_Modifica = username,
                                .Data_Creazione = Now(),
                                .Data_Modifica = Now(),
                                .Validita_Inizio = AGRODATAINIZIO,
                                .Validita_Fine = AGRODATAFINE,
                                .inviato = "0",
                                .datainvio = Now()
                            }
                            g2g.G2G_Analisi_Certificato_Recode_insert.Add(recode)
                            GiasContext.G2G_Recode_Analisi_Certificato.Add(recode)

                        Next

                        GiasContext.SaveChanges()

                    End If

                    'TESTATA
                    For Each m As Analisi_Testata In g2g.analisi_testata_update

                        Dim recode = (From rr In GiasContext.G2G_Recode_Analisi_Testata Where rr.To_PivaSuperUser = Origine_Piva_SuperUser AndAlso rr.From_PivaSuperUser = Destinazione_Piva_SuperUser AndAlso rr.To_Analisi_Testata_Cod = m.Analisi_Testata_Cod).FirstOrDefault()
                        recode.Username_Modifica = username
                        recode.Data_Modifica = data
                        recode.datainvio = data
                        g2g.G2G_Analisi_Testata_Recode_update.Add(recode)
                        GiasContext.G2G_Recode_Analisi_Testata.Attach(recode)
                        GiasContext.Entry(recode).State = EntityState.Modified

                        Dim testata = (From mm In GiasContext.Analisi_Testata Where mm.Analisi_Testata_Cod = recode.From_Analisi_Testata_Cod).FirstOrDefault()
                        testata = Gias_EF_Utility.CopyEntity(GiasContext, m, testata, username, data)
                        testata.Analisi_SuperUser = Destinazione_Piva_SuperUser
                        testata.Analisi_Testata_Cod = recode.From_Analisi_Testata_Cod
                        If testata.Analisi_Certificato_Cod <> 0 Then
                            Dim certificato = (From m1 In GiasContext.G2G_Recode_Analisi_Certificato Where m1.From_PivaSuperUser = Destinazione_Piva_SuperUser And m1.To_Analisi_Certificato_Cod = testata.Analisi_Certificato_Cod Select m1).FirstOrDefault
                            If certificato Is Nothing Then
                                Throw New Exception("Errore, non esiste la decodifica di Analisi_Certificato_Cod = " & testata.Analisi_Certificato_Cod & " in G2G_Recode_Analisi_Certificato")
                            Else
                                testata.Analisi_Certificato_Cod = certificato.From_Analisi_Certificato_Cod
                            End If
                        End If
                        GiasContext.Analisi_Testata.Attach(testata)
                        GiasContext.Entry(testata).State = EntityState.Modified

                    Next

                    GiasContext.SaveChanges()

                    If g2g.analisi_testata_insert.Count > 0 Then

                        For Each s As Analisi_Testata In g2g.analisi_testata_insert

                            Dim idSeq As Integer = 0


                            'Richiedo un nuovo id sequenza
                            'idSeq = objSequenze.NuovoId_Tabella_EF(GiasContext, "ANALISI_TESTATA", 0, 2000000000, objParametri)
                            idSeq = objSequenze.NuovoId_Tabella("ANALISI_TESTATA", 0, 2000000000, objParametri, UtilizzaTransazione:=False)

                            Dim analisi_testata = Gias_EF_Utility.CopyEntity(GiasContext, s, Nothing, username, data)

                            analisi_testata.Analisi_Testata_Cod = idSeq
                            analisi_testata.Analisi_SuperUser = Destinazione_Piva_SuperUser

                            If analisi_testata.Analisi_Certificato_Cod <> 0 Then
                                Dim recodeCertificato = (From c In GiasContext.G2G_Recode_Analisi_Certificato Where c.To_PivaSuperUser = Origine_Piva_SuperUser And
                                                                                                                      c.From_PivaSuperUser = Destinazione_Piva_SuperUser And
                                                                                                                      c.To_Analisi_Certificato_Cod = s.Analisi_Certificato_Cod).FirstOrDefault
                                If recodeCertificato IsNot Nothing Then
                                    analisi_testata.Analisi_Certificato_Cod = recodeCertificato.From_Analisi_Certificato_Cod
                                End If
                            End If

                            GiasContext.Analisi_Testata.Add(analisi_testata)

                            Dim recode =
                                    New G2G_Recode_Analisi_Testata With {
                                        .From_PivaSuperUser = Destinazione_Piva_SuperUser,
                                        .To_PivaSuperUser = Origine_Piva_SuperUser,
                                        .From_Analisi_Testata_Cod = analisi_testata.Analisi_Testata_Cod,
                                        .To_Analisi_Testata_Cod = s.Analisi_Testata_Cod,
                                        .Username_Creazione = username,
                                        .Username_Modifica = username,
                                        .Data_Creazione = Now(),
                                        .Data_Modifica = Now(),
                                        .Validita_Inizio = AGRODATAINIZIO,
                                        .Validita_Fine = AGRODATAFINE,
                                        .inviato = "0",
                                        .datainvio = Now()
                                    }
                            g2g.G2G_Analisi_Testata_Recode_insert.Add(recode)
                            GiasContext.G2G_Recode_Analisi_Testata.Add(recode)



                        Next

                        GiasContext.SaveChanges()

                    End If

                    'DETTAGLI
                    For Each m As Analisi_Dettagli In g2g.analisi_dettagli_update

                        Dim recode = (From rr In GiasContext.G2G_Recode_Analisi_Dettagli Where rr.To_PivaSuperUser = Origine_Piva_SuperUser AndAlso rr.From_PivaSuperUser = Destinazione_Piva_SuperUser AndAlso rr.To_Analisi_Testata_Cod = m.Analisi_Testata_Cod AndAlso rr.To_Analisi_Dettaglio_Cod = m.Analisi_Dettaglio_Cod).FirstOrDefault()
                        recode.Username_Modifica = username
                        recode.Data_Modifica = data
                        recode.datainvio = data
                        g2g.G2G_Analisi_Dettagli_Recode_update.Add(recode)
                        GiasContext.G2G_Recode_Analisi_Dettagli.Attach(recode)
                        GiasContext.Entry(recode).State = EntityState.Modified

                        Dim dettaglio = (From mm In GiasContext.Analisi_Dettagli Where mm.Analisi_Testata_Cod = recode.From_Analisi_Testata_Cod AndAlso mm.Analisi_Dettaglio_Cod = recode.From_Analisi_Dettaglio_Cod).FirstOrDefault()
                        dettaglio = Gias_EF_Utility.CopyEntity(GiasContext, m, dettaglio, username, data)
                        dettaglio.Analisi_SuperUser = Destinazione_Piva_SuperUser
                        dettaglio.Analisi_Testata_Cod = recode.From_Analisi_Testata_Cod
                        dettaglio.Analisi_Dettaglio_Cod = recode.From_Analisi_Dettaglio_Cod
                        GiasContext.Analisi_Dettagli.Attach(dettaglio)
                        GiasContext.Entry(dettaglio).State = EntityState.Modified

                    Next

                    GiasContext.SaveChanges()

                    For Each s As Analisi_Dettagli In g2g.analisi_dettagli_insert

                        Dim idSeq As Integer = 0

                        Dim analisi_dettagli = Gias_EF_Utility.CopyEntity(GiasContext, s, Nothing, username, data)

                        If analisi_dettagli.Analisi_Dettaglio_Cod <> 0 Then
                            'Richiedo un nuovo id sequenza
                            'idSeq = objSequenze.NuovoId_Tabella_EF(GiasContext, "ANALISI_DETTAGLI", 0, 2000000000, objParametri)
                            idSeq = objSequenze.NuovoId_Tabella("ANALISI_DETTAGLI", 0, 2000000000, objParametri, UtilizzaTransazione:=False)
                            analisi_dettagli.Analisi_Dettaglio_Cod = idSeq
                        End If

                        analisi_dettagli.Analisi_SuperUser = Destinazione_Piva_SuperUser

                        analisi_dettagli.Analisi_Testata_Cod = (From c In GiasContext.G2G_Recode_Analisi_Testata Where c.To_PivaSuperUser = Origine_Piva_SuperUser And
                                                                                                                        c.From_PivaSuperUser = Destinazione_Piva_SuperUser And
                                                                                                                        c.To_Analisi_Testata_Cod = s.Analisi_Testata_Cod).FirstOrDefault.From_Analisi_Testata_Cod


                        GiasContext.Analisi_Dettagli.Add(analisi_dettagli)

                        Dim recode =
                                New G2G_Recode_Analisi_Dettagli With {
                                    .From_PivaSuperUser = Destinazione_Piva_SuperUser,
                                    .To_PivaSuperUser = Origine_Piva_SuperUser,
                                    .From_Analisi_Testata_Cod = analisi_dettagli.Analisi_Testata_Cod,
                                    .To_Analisi_Testata_Cod = s.Analisi_Testata_Cod,
                                    .From_Analisi_Dettaglio_Cod = analisi_dettagli.Analisi_Dettaglio_Cod,
                                    .To_Analisi_Dettaglio_Cod = s.Analisi_Dettaglio_Cod,
                                    .Username_Creazione = username,
                                    .Username_Modifica = username,
                                    .Data_Creazione = Now(),
                                    .Data_Modifica = Now(),
                                    .Validita_Inizio = AGRODATAINIZIO,
                                    .Validita_Fine = AGRODATAFINE,
                                    .inviato = "0",
                                    .datainvio = Now()
                                }
                        g2g.G2G_Analisi_Dettagli_Recode_insert.Add(recode)
                        GiasContext.G2G_Recode_Analisi_Dettagli.Add(recode)

                    Next

                    GiasContext.SaveChanges()

                    'ENTITAXTESTATA
                    For Each s As Analisi_EntitaxTestata In g2g.analisi_entitaxtestata_insert

                        'Richiedo un nuovo id sequenza
                        Dim analisi_entitaxtestata = Gias_EF_Utility.CopyEntity(GiasContext, s, Nothing, username, data)

                        analisi_entitaxtestata.Analisi_Testata_Cod = (From c In GiasContext.G2G_Recode_Analisi_Testata Where c.To_PivaSuperUser = Origine_Piva_SuperUser And
                                                                                                                        c.From_PivaSuperUser = Destinazione_Piva_SuperUser And
                                                                                                                        c.To_Analisi_Testata_Cod = s.Analisi_Testata_Cod).FirstOrDefault.From_Analisi_Testata_Cod


                        analisi_entitaxtestata.Analisi_SuperUser = Destinazione_Piva_SuperUser
                        analisi_entitaxtestata.Piva = piva

                        GiasContext.Analisi_EntitaxTestata.Add(analisi_entitaxtestata)

                        If (From rec In GiasContext.G2G_Recode_Analisi_EntitaxTestata Where rec.To_PivaSuperUser = s.Analisi_SuperUser _
                                                                                                    And rec.To_Analisi_Testata_Cod = s.Analisi_Testata_Cod _
                                                                                                    And rec.To_Piva = s.Piva _
                                                                                                    And rec.From_Analisi_Testata_Cod = analisi_entitaxtestata.Analisi_Testata_Cod _
                                                                                                    And rec.From_PivaSuperUser = analisi_entitaxtestata.Analisi_SuperUser _
                                                                                                    And rec.From_Piva = analisi_entitaxtestata.Piva).FirstOrDefault Is Nothing Then

                            Dim recode =
                                New G2G_Recode_Analisi_EntitaxTestata With {
                                    .From_PivaSuperUser = Destinazione_Piva_SuperUser,
                                    .To_PivaSuperUser = Origine_Piva_SuperUser,
                                    .From_Analisi_Testata_Cod = analisi_entitaxtestata.Analisi_Testata_Cod,
                                    .To_Analisi_Testata_Cod = s.Analisi_Testata_Cod,
                                    .From_Piva = piva,
                                    .To_Piva = s.Piva,
                                    .Username_Creazione = username,
                                    .Username_Modifica = username,
                                    .Data_Creazione = Now(),
                                    .Data_Modifica = Now(),
                                    .Validita_Inizio = AGRODATAINIZIO,
                                    .Validita_Fine = AGRODATAFINE,
                                    .inviato = "0",
                                    .datainvio = Now()
                                }
                            g2g.G2G_Analisi_EntitaxTestata_Recode_insert.Add(recode)
                            GiasContext.G2G_Recode_Analisi_EntitaxTestata.Add(recode)
                            GiasContext.SaveChanges()
                        End If

                    Next

                    GiasContext.SaveChanges()

                    'CAMPIONI
                    For Each m As Analisi_Campioni In g2g.analisi_campioni_update

                        Dim recode = (From rr In GiasContext.G2G_Recode_Analisi_Campioni Where rr.To_PivaSuperUser = Origine_Piva_SuperUser AndAlso rr.From_PivaSuperUser = Destinazione_Piva_SuperUser AndAlso rr.To_Analisi_Campione_Cod = m.Analisi_Campione_Cod).FirstOrDefault()
                        recode.Username_Modifica = username
                        recode.Data_Modifica = data
                        recode.datainvio = data
                        g2g.G2G_Analisi_Campioni_update.Add(recode)
                        GiasContext.G2G_Recode_Analisi_Campioni.Attach(recode)
                        GiasContext.Entry(recode).State = EntityState.Modified

                        Dim campione = (From mm In GiasContext.Analisi_Campioni Where mm.Analisi_Campione_Cod = recode.From_Analisi_Campione_Cod).FirstOrDefault()
                        campione = Gias_EF_Utility.CopyEntity(GiasContext, m, campione, username, data)
                        campione.Analisi_SuperUser = Destinazione_Piva_SuperUser
                        campione.Analisi_Campione_Cod = recode.From_Analisi_Campione_Cod
                        GiasContext.Analisi_Campioni.Attach(campione)
                        GiasContext.Entry(campione).State = EntityState.Modified

                        Dim analisi_dettaglis = (From ad In GiasContext.Analisi_CampionixDettagli Where ad.Analisi_Campione_Cod = recode.From_Analisi_Campione_Cod).ToList
                        For Each analisi_dettaglio In analisi_dettaglis
                            GiasContext.Analisi_CampionixDettagli.Attach(analisi_dettaglio)
                            GiasContext.Analisi_CampionixDettagli.Remove(analisi_dettaglio)
                        Next

                    Next

                    GiasContext.SaveChanges()

                    For Each s As Analisi_Campioni In g2g.analisi_campioni_insert

                        Dim idSeq As Integer = 0

                        Dim analisi_campioni = Gias_EF_Utility.CopyEntity(GiasContext, s, Nothing, username, data)

                        If analisi_campioni.Analisi_Campione_Cod <> 0 Then
                            'Richiedo un nuovo id sequenza
                            'idSeq = objSequenze.NuovoId_Tabella_EF(GiasContext, "ANALISI_CAMPIONI", 0, 2000000000, objParametri)
                            idSeq = objSequenze.NuovoId_Tabella("ANALISI_CAMPIONI", 0, 2000000000, objParametri, UtilizzaTransazione:=False)
                            analisi_campioni.Analisi_Campione_Cod = idSeq
                        End If

                        analisi_campioni.Analisi_SuperUser = Destinazione_Piva_SuperUser


                        GiasContext.Analisi_Campioni.Add(analisi_campioni)

                        Dim recode =
                                New G2G_Recode_Analisi_Campioni With {
                                    .From_PivaSuperUser = Destinazione_Piva_SuperUser,
                                    .To_PivaSuperUser = Origine_Piva_SuperUser,
                                    .From_Analisi_Campione_Cod = analisi_campioni.Analisi_Campione_Cod,
                                    .To_Analisi_Campione_Cod = s.Analisi_Campione_Cod,
                                    .Username_Creazione = username,
                                    .Username_Modifica = username,
                                    .Data_Creazione = Now(),
                                    .Data_Modifica = Now(),
                                    .Validita_Inizio = AGRODATAINIZIO,
                                    .Validita_Fine = AGRODATAFINE,
                                    .inviato = "0",
                                    .datainvio = Now()
                                }
                        g2g.G2G_Analisi_Campioni_insert.Add(recode)
                        GiasContext.G2G_Recode_Analisi_Campioni.Add(recode)



                    Next

                    GiasContext.SaveChanges()

                    'CAMPIONIXDETTAGLI
                    For Each s As Analisi_CampionixDettagli In g2g.analisi_campionixdettagli_insert

                        Dim idSeq As Integer = 0

                        Dim analisi_dettagli = Gias_EF_Utility.CopyEntity(GiasContext, s, Nothing, username, data)

                        analisi_dettagli.Analisi_SuperUser = Destinazione_Piva_SuperUser
                        If analisi_dettagli.Analisi_Testata_Cod <> 0 Then
                            analisi_dettagli.Analisi_Testata_Cod = (From g In GiasContext.G2G_Recode_Analisi_Testata Where g.To_PivaSuperUser = s.Analisi_SuperUser And g.From_PivaSuperUser = Destinazione_Piva_SuperUser And g.To_Analisi_Testata_Cod = s.Analisi_Testata_Cod).FirstOrDefault.To_Analisi_Testata_Cod
                        End If

                        If analisi_dettagli.Analisi_Dettaglio_Cod <> 0 Then
                            analisi_dettagli.Analisi_Dettaglio_Cod = (From g In GiasContext.G2G_Recode_Analisi_Dettagli Where g.To_PivaSuperUser = s.Analisi_SuperUser And g.From_PivaSuperUser = Destinazione_Piva_SuperUser _
                                                                                                                            And g.To_Analisi_Testata_Cod = s.Analisi_Testata_Cod _
                                                                                                                            And g.To_Analisi_Dettaglio_Cod = s.Analisi_Dettaglio_Cod).FirstOrDefault.From_Analisi_Dettaglio_Cod
                        End If

                        If analisi_dettagli.Analisi_Campione_Cod <> 0 Then
                            analisi_dettagli.Analisi_Campione_Cod = (From g In GiasContext.G2G_Recode_Analisi_Campioni Where g.To_PivaSuperUser = s.Analisi_SuperUser And g.From_PivaSuperUser = Destinazione_Piva_SuperUser _
                                                                                                                            And g.To_Analisi_Campione_Cod = s.Analisi_Campione_Cod).FirstOrDefault.From_Analisi_Campione_Cod
                        End If


                        GiasContext.Analisi_CampionixDettagli.Add(analisi_dettagli)

                    Next

                    GiasContext.SaveChanges()

                    ' COMIT Effettivo
                    scope.Complete()

                    g2g.Recode.G2GRecodeAnalisi_CertificatoToInsert = g2g.G2G_Analisi_Certificato_Recode_insert
                    g2g.Recode.G2GRecodeAnalisi_CertificatoToUpdate = g2g.G2G_Analisi_Certificato_Recode_update
                    g2g.Recode.G2GRecodeAnalisi_CertificatoToDelete = g2g.G2G_Analisi_Certificato_Recode_delete

                    g2g.Recode.G2GRecodeAnalisi_TestataToInsert = g2g.G2G_Analisi_Testata_Recode_insert
                    g2g.Recode.G2GRecodeAnalisi_TestataToUpdate = g2g.G2G_Analisi_Testata_Recode_update
                    g2g.Recode.G2GRecodeAnalisi_TestataToDelete = g2g.G2G_Analisi_Testata_Recode_delete

                    g2g.Recode.G2GRecodeAnalisi_DettagliToInsert = g2g.G2G_Analisi_Dettagli_Recode_insert
                    g2g.Recode.G2GRecodeAnalisi_DettagliToUpdate = g2g.G2G_Analisi_Dettagli_Recode_update
                    g2g.Recode.G2GRecodeAnalisi_DettagliToDelete = g2g.G2G_Analisi_Dettagli_Recode_delete

                    g2g.Recode.G2GRecodeAnalisi_EntitaxTestataToInsert = g2g.G2G_Analisi_EntitaxTestata_Recode_insert
                    'g2g.Recode.G2GRecodeAnalisi_EntitaxTestataToUpdate = g2g.G2G_Analisi_EntitaxTestata_Recode_insert
                    g2g.Recode.G2GRecodeAnalisi_EntitaxTestataToDelete = g2g.G2G_Analisi_EntitaxTestata_Recode_delete

                    g2g.Recode.G2GRecodeAnalisi_CampioniToInsert = g2g.G2G_Analisi_Campioni_insert
                    g2g.Recode.G2GRecodeAnalisi_CampioniToUpdate = g2g.G2G_Analisi_Campioni_update
                    g2g.Recode.G2GRecodeAnalisi_CampioniToDelete = g2g.G2G_Analisi_Campioni_delete

                End Using
            End Using

        Catch ex As Exception

            messaggioErrore = ex.Message
            g2g.Recode.MessaggioErrore = messaggioErrore
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            'Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore, ex)

        End Try

        Return g2g.Recode

    End Function

End Class