Imports System.Data.Entity
Imports System.Transactions
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreModello

Public Class G2GPlanning_R

    Public Function LeggiPerGias2Gias(ByVal piva As String,
                                      ByVal piva_dest As String,
                                      ByVal PivaSuperUser_Destinazione As String,
                                      ByVal objOpzioniImportImpresa As clsImpresa,
                                      ByRef objParametri_server As AgronicaCoreParametri) As G2G_Planning

        Dim gefutils As New AgronicaCoreEntityFramework.Gias_EF_Utility
        Dim efConnString As String = gefutils.GetEntityConnectionString(objParametri_server.StringaConnessione)
        Dim psu As String = objParametri_server.PivaSuperUser

        Dim rval As New G2G_Planning With {
            .From_Piva = piva,
            .From_PivaSuperUser = objParametri_server.PivaSuperUser,
            .To_Piva = piva_dest,
            .Recode = G2GUtility.Nuovo_G2G_Recode()
        }

        Using giasContext As New AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities(efConnString)
            giasContext.Database.CommandTimeout = 3600
            'PROGARMMAZIONE_TESTATA
            rval.programmazione_testata_insert = (
            From pt In giasContext.Programmazione_Testata
            Where pt.Piva = piva _
                AndAlso pt.Validita_Fine >= objOpzioniImportImpresa.ValiditaInizio_planning _
                AndAlso Not giasContext.G2G_Recode_Programmazione_Testata.Any(Function(g) g.From_PivaSuperUser = pt.Piva_SuperUser AndAlso g.To_PivaSuperUser = PivaSuperUser_Destinazione AndAlso g.From_Piva = pt.Piva AndAlso g.From_Programmazione_Cod = pt.Programmazione_Cod)
            Select pt).ToList()

            rval.programmazione_testata_update = (
            From pt In giasContext.Programmazione_Testata
            Join g2g In giasContext.G2G_Recode_Programmazione_Testata
                On pt.Piva_SuperUser Equals g2g.From_PivaSuperUser _
                And pt.Piva Equals g2g.From_Piva _
                And pt.Programmazione_Cod Equals g2g.From_Programmazione_Cod
            Where pt.Piva = piva _
                AndAlso System.Data.Entity.DbFunctions.AddDays(g2g.Data_Modifica, -1) <= pt.Data_Modifica
            Select pt
            ).ToList()

            rval.Recode.G2GRecodeProgrammazioneTestataToDelete = (
            From r In giasContext.G2G_Recode_Programmazione_Testata
            Where r.From_PivaSuperUser = psu _
                AndAlso r.From_Piva = piva _
                AndAlso Not giasContext.Programmazione_Testata.Any(Function(pt) pt.Piva_SuperUser = r.From_PivaSuperUser AndAlso pt.Piva = r.From_Piva AndAlso pt.Programmazione_Cod = r.From_Programmazione_Cod)
            Select r).ToList()

            'PROGARMMAZIONE_ENTITA
            rval.programmazione_entita_insert = (
            From pe In giasContext.Programmazione_Entita
            Join pt In giasContext.Programmazione_Testata
                On pt.Piva_SuperUser Equals pe.Piva_SuperUser _
                And pt.Piva Equals pe.Piva _
                And pt.Programmazione_Cod Equals pe.Programmazione_Cod
            Group Join g In giasContext.G2G_Recode_Programmazione_Entita.Where(Function(x) x.To_PivaSuperUser = PivaSuperUser_Destinazione) On
                                                                        g.From_PivaSuperUser Equals pe.Piva_SuperUser _
                                                                        And g.From_Piva Equals pe.Piva _
                                                                        And g.From_Programmazione_Entita_Cod Equals pe.Programmazione_Entita_Cod
                Into g_group = Group
            From _g_group In g_group.DefaultIfEmpty()
            Where pe.Piva = piva _
                AndAlso pt.Validita_Fine >= objOpzioniImportImpresa.ValiditaInizio_planning _
                AndAlso _g_group Is Nothing
            Select pe).ToList()

            'AndAlso Not giasContext.G2G_Recode_Programmazione_Entita.Any(Function(g) g.From_PivaSuperUser = pe.Piva_SuperUser AndAlso g.To_PivaSuperUser = PivaSuperUser_Destinazione AndAlso g.From_Piva = pe.Piva AndAlso g.From_Programmazione_Entita_Cod = pe.Programmazione_Entita_Cod)

            rval.programmazione_entita_update = (
            From pe In giasContext.Programmazione_Entita
            Join g2g In giasContext.G2G_Recode_Programmazione_Entita
                On pe.Piva_SuperUser Equals g2g.From_PivaSuperUser _
                And pe.Piva Equals g2g.From_Piva _
                And pe.Programmazione_Entita_Cod Equals g2g.From_Programmazione_Entita_Cod
            Where pe.Piva = piva _
                AndAlso System.Data.Entity.DbFunctions.AddDays(g2g.Data_Modifica, -1) <= pe.Data_Modifica
            Select pe
            ).ToList()

            Dim HashPiva As New Hashtable
            Dim HashCentri As New Hashtable
            Dim HashCampi As New Hashtable

            'Sistemo le codifiche di piva/sacod/appezza/idreg/distinta
            For Each list In {rval.programmazione_entita_insert, rval.programmazione_entita_update}
                For Each pe_curr In list

                    'NB: mantenere questo ordine perché altrimenti ad esempio la sostituzione della piva all'inizio farebbe sì che non trovano mapping tutti gli altri
                    If pe_curr.Progetto_Cod > 0 Then
                        pe_curr.Progetto_Cod = (From rr In giasContext.G2G_Recode_Distinta Where rr.From_PivaSuperUser = psu AndAlso rr.To_PivaSuperUser = PivaSuperUser_Destinazione AndAlso rr.From_Piva = pe_curr.Piva AndAlso rr.From_Progetto_cod = pe_curr.Progetto_Cod Select rr.To_Progetto_cod).First()
                    End If

                    If pe_curr.Id_Reg > 0 Then
                        pe_curr.Id_Reg = (From rr In giasContext.G2G_Recode_Impianti Where rr.From_PivaSuperUser = psu AndAlso rr.To_PivaSuperUser = PivaSuperUser_Destinazione AndAlso rr.From_Piva = pe_curr.Piva AndAlso rr.From_Sa_Cod = pe_curr.Sa_Cod AndAlso rr.From_Appezza = pe_curr.Appezza AndAlso rr.From_Id_Reg = pe_curr.Id_Reg Select rr.To_Id_Reg).First()
                    End If

                    If pe_curr.Appezza > 0 Then
                        pe_curr.Appezza = (From rr In giasContext.G2G_Recode_Appezzamenti Where rr.From_PivaSuperUser = psu AndAlso rr.To_PivaSuperUser = PivaSuperUser_Destinazione AndAlso rr.From_Piva = pe_curr.Piva AndAlso rr.From_Sa_Cod = pe_curr.Sa_Cod AndAlso rr.From_Appezza = pe_curr.Appezza Select rr.To_Appezza).First()
                    End If

                    Dim ChiaveCampo = pe_curr.Piva & "_" & pe_curr.Sa_Cod & "_" & pe_curr.Campo_Cod
                    If pe_curr.Campo_Cod > 0 Then
                        If Not HashCampi.Contains(ChiaveCampo) Then
                            pe_curr.Campo_Cod = (From rr In giasContext.G2G_Recode_Campo Where rr.From_PivaSuperUser = psu AndAlso rr.To_PivaSuperUser = PivaSuperUser_Destinazione AndAlso rr.From_Piva = pe_curr.Piva AndAlso rr.From_Sa_Cod = pe_curr.Sa_Cod AndAlso rr.From_Campo_cod = pe_curr.Campo_Cod Select rr.To_Campo_cod).First()
                            HashCampi.Add(ChiaveCampo, pe_curr.Campo_Cod)
                        Else
                            pe_curr.Campo_Cod = HashCampi(ChiaveCampo)
                        End If
                    End If

                    Dim ChiaveCentro = pe_curr.Piva & "_" & pe_curr.Sa_Cod
                    If pe_curr.Sa_Cod > 0 Then
                        If Not HashCentri.Contains(ChiaveCentro) Then
                            pe_curr.Sa_Cod = (From rr In giasContext.G2G_Recode_Imprese Where rr.From_PivaSuperUser = psu AndAlso rr.To_PivaSuperUser = PivaSuperUser_Destinazione AndAlso rr.FROM_Piva = pe_curr.Piva AndAlso rr.FROM_SaCod = pe_curr.Sa_Cod Select rr.TO_SaCod).First()
                            HashCentri.Add(ChiaveCentro, pe_curr.Sa_Cod)
                        Else
                            pe_curr.Sa_Cod = HashCentri(ChiaveCentro)
                        End If
                    End If

                    Dim ChiavePiva = pe_curr.Piva
                    If pe_curr.Piva <> "" Then
                        If Not HashPiva.Contains(ChiavePiva) Then
                            pe_curr.Piva = (From rr In giasContext.G2G_Recode_Imprese Where rr.From_PivaSuperUser = psu AndAlso rr.To_PivaSuperUser = PivaSuperUser_Destinazione AndAlso rr.FROM_Piva = pe_curr.Piva AndAlso rr.FROM_SaCod = 0 Select rr.To_Piva).First()
                            HashPiva.Add(ChiavePiva, piva)
                        Else
                            pe_curr.Piva = HashPiva(ChiavePiva)
                        End If
                    End If

                Next
            Next

            rval.Recode.G2GRecodeProgrammazioneEntitaToDelete = (
            From r In giasContext.G2G_Recode_Programmazione_Entita
            Where r.From_PivaSuperUser = psu _
                AndAlso r.From_Piva = piva _
                AndAlso Not giasContext.Programmazione_Entita.Any(Function(pe) pe.Piva_SuperUser = r.From_PivaSuperUser AndAlso pe.Piva = r.From_Piva AndAlso pe.Programmazione_Entita_Cod = r.From_Programmazione_Entita_Cod)
            Select r).ToList()

            'PROGARMMAZIONE_ENTITA_CODICI
            Dim listaPEC_Insert As List(Of Programmazione_Entita_Codici) = (
            From pec In giasContext.Programmazione_Entita_Codici
            Join pe In giasContext.Programmazione_Entita
                    On pe.Piva_SuperUser Equals pec.Piva_SuperUser _
                    And pe.Programmazione_Entita_Cod Equals pec.Programmazione_Entita_Cod
            Join pt In giasContext.Programmazione_Testata
                On pt.Piva_SuperUser Equals pe.Piva_SuperUser _
                And pt.Piva Equals pe.Piva _
                And pt.Programmazione_Cod Equals pe.Programmazione_Cod
            Group Join g In giasContext.G2G_Recode_Programmazione_Entita.Where(Function(x) x.To_PivaSuperUser = PivaSuperUser_Destinazione) On
                                                                        g.From_PivaSuperUser Equals pe.Piva_SuperUser _
                                                                        And g.From_Piva Equals pe.Piva _
                                                                        And g.From_Programmazione_Entita_Cod Equals pe.Programmazione_Entita_Cod
                Into g_group = Group
            From _g_group In g_group.DefaultIfEmpty()
            Where pe.Piva = piva _
                AndAlso pt.Validita_Fine >= objOpzioniImportImpresa.ValiditaInizio_planning _
                AndAlso _g_group Is Nothing
            Select pec).ToList()

            'AndAlso Not giasContext.G2G_Recode_Programmazione_Entita.Any(Function(g) g.From_PivaSuperUser = pe.Piva_SuperUser And g.To_PivaSuperUser = PivaSuperUser_Destinazione And g.From_Programmazione_Entita_Cod = pe.Programmazione_Entita_Cod And g.From_Piva = piva)

            Dim listaPEC_Update As List(Of Programmazione_Entita_Codici) = (
            From pec In giasContext.Programmazione_Entita_Codici
            Join pe In giasContext.Programmazione_Entita
                On pe.Piva_SuperUser Equals pec.Piva_SuperUser _
                And pe.Programmazione_Entita_Cod Equals pec.Programmazione_Entita_Cod
            Join g2g In giasContext.G2G_Recode_Programmazione_Entita
                On pe.Piva_SuperUser Equals g2g.From_PivaSuperUser _
                And pe.Programmazione_Entita_Cod Equals g2g.From_Programmazione_Entita_Cod
            Where pe.Piva = piva _
                AndAlso System.Data.Entity.DbFunctions.AddDays(g2g.Data_Modifica, -1) <= pe.Data_Modifica
            Select pec
            ).ToList()

            rval.programmazione_entita_codici_insert = New List(Of Programmazione_Entita_Codici)
            rval.programmazione_entita_codici_insert.AddRange(listaPEC_Insert)
            rval.programmazione_entita_codici_insert.AddRange(listaPEC_Update)

            'PROGARMMAZIONE_PARTICELLE
            Dim listaPP_Insert As List(Of Programmazione_Particelle) = (
            From pp In giasContext.Programmazione_Particelle
            Join pe In giasContext.Programmazione_Entita
                    On pe.Piva_SuperUser Equals pp.Piva_SuperUser _
                    And pe.Programmazione_Entita_Cod Equals pp.Programmazione_Entita_Cod
            Join pt In giasContext.Programmazione_Testata
                On pt.Piva_SuperUser Equals pe.Piva_SuperUser _
                And pt.Piva Equals pe.Piva _
                And pt.Programmazione_Cod Equals pe.Programmazione_Cod
            Group Join g In giasContext.G2G_Recode_Programmazione_Entita.Where(Function(x) x.To_PivaSuperUser = PivaSuperUser_Destinazione) On
                                                                        g.From_PivaSuperUser Equals pe.Piva_SuperUser _
                                                                        And g.From_Piva Equals pe.Piva _
                                                                        And g.From_Programmazione_Entita_Cod Equals pe.Programmazione_Entita_Cod
                Into g_group = Group
            From _g_group In g_group.DefaultIfEmpty()
            Where pe.Piva = piva _
                AndAlso pt.Validita_Fine >= objOpzioniImportImpresa.ValiditaInizio_planning _
                AndAlso _g_group Is Nothing
            Select pp).ToList()

            'AndAlso Not giasContext.G2G_Recode_Programmazione_Entita.Any(Function(g) g.From_PivaSuperUser = pe.Piva_SuperUser And g.To_PivaSuperUser = PivaSuperUser_Destinazione And g.From_Programmazione_Entita_Cod = pe.Programmazione_Entita_Cod And g.From_Piva = piva)

            Dim listaPP_Update As List(Of Programmazione_Particelle) = (
            From pp In giasContext.Programmazione_Particelle
            Join pe In giasContext.Programmazione_Entita
                On pe.Piva_SuperUser Equals pp.Piva_SuperUser _
                And pe.Programmazione_Entita_Cod Equals pp.Programmazione_Entita_Cod
            Join g2g In giasContext.G2G_Recode_Programmazione_Entita
                On pe.Piva_SuperUser Equals g2g.From_PivaSuperUser _
                And pe.Programmazione_Entita_Cod Equals g2g.From_Programmazione_Entita_Cod
            Where pe.Piva = piva _
                AndAlso System.Data.Entity.DbFunctions.AddDays(g2g.Data_Modifica, -1) <= pe.Data_Modifica
            Select pp
            ).ToList()

            rval.programmazione_particelle_insert = New List(Of Programmazione_Particelle)
            rval.programmazione_particelle_insert.AddRange(listaPP_Insert)
            rval.programmazione_particelle_insert.AddRange(listaPP_Update)

            'REG_IMPIANTI_PROGARMMAZIONI
            Dim listaRIP_Insert As List(Of Reg_Impianti_Programmazioni) = (
            From rip In giasContext.Reg_Impianti_Programmazioni
            Join pe In giasContext.Programmazione_Entita
                    On pe.Piva_SuperUser Equals rip.Piva_SuperUser _
                    And pe.Programmazione_Entita_Cod Equals rip.Programmazione_Entita_Cod _
                    And pe.Programmazione_Cod Equals rip.Programmazione_Cod
            Join pt In giasContext.Programmazione_Testata
                On pt.Piva_SuperUser Equals pe.Piva_SuperUser _
                And pt.Piva Equals pe.Piva _
                And pt.Programmazione_Cod Equals pe.Programmazione_Cod
            Group Join g In giasContext.G2G_Recode_Programmazione_Entita.Where(Function(x) x.To_PivaSuperUser = PivaSuperUser_Destinazione) On
                                                                        g.From_PivaSuperUser Equals pe.Piva_SuperUser _
                                                                        And g.From_Piva Equals pe.Piva _
                                                                        And g.From_Programmazione_Entita_Cod Equals pe.Programmazione_Entita_Cod
                Into g_group = Group
            From _g_group In g_group.DefaultIfEmpty()
            Where pe.Piva = piva _
                AndAlso pt.Validita_Fine >= objOpzioniImportImpresa.ValiditaInizio_planning _
                AndAlso _g_group Is Nothing
            Select rip).ToList()

            'AndAlso Not giasContext.G2G_Recode_Programmazione_Entita.Any(Function(g) g.From_PivaSuperUser = pe.Piva_SuperUser And g.To_PivaSuperUser = PivaSuperUser_Destinazione And g.From_Programmazione_Entita_Cod = pe.Programmazione_Entita_Cod And g.From_Piva = piva)

            Dim listaRIP_Update As List(Of Reg_Impianti_Programmazioni) = (
            From rip In giasContext.Reg_Impianti_Programmazioni
            Join pe In giasContext.Programmazione_Entita
                On pe.Piva_SuperUser Equals rip.Piva_SuperUser _
                And pe.Programmazione_Entita_Cod Equals rip.Programmazione_Entita_Cod _
                And pe.Programmazione_Cod Equals rip.Programmazione_Cod
            Join g2g In giasContext.G2G_Recode_Programmazione_Entita
                On pe.Piva_SuperUser Equals g2g.From_PivaSuperUser _
                And pe.Programmazione_Entita_Cod Equals g2g.From_Programmazione_Entita_Cod
            Where pe.Piva = piva _
                AndAlso System.Data.Entity.DbFunctions.AddDays(g2g.Data_Modifica, -1) <= pe.Data_Modifica
            Select rip
            ).ToList()

            'Sistemo le codifiche di piva/sacod/appezza/idreg/distinta
            For Each list In {listaRIP_Insert, listaRIP_Update}
                For Each rip_curr In list
                    Dim recodeRI = (From rr In giasContext.G2G_Recode_Impianti Where rr.From_PivaSuperUser = psu AndAlso rr.To_PivaSuperUser = PivaSuperUser_Destinazione AndAlso rr.From_Piva = rip_curr.Piva AndAlso rr.From_Sa_Cod = rip_curr.Sa_Cod AndAlso rr.From_Appezza = rip_curr.Appezza AndAlso rr.From_Id_Reg = rip_curr.Id_Reg).First()
                    Dim recodeP = (From rr In giasContext.G2G_Recode_Distinta Where rr.From_PivaSuperUser = psu AndAlso rr.To_PivaSuperUser = PivaSuperUser_Destinazione AndAlso rr.From_Piva = rip_curr.Piva AndAlso rr.From_Progetto_cod = rip_curr.Progetto_Cod).First()

                    rip_curr.Sa_Cod = recodeRI.To_Sa_Cod
                    rip_curr.Appezza = recodeRI.To_Appezza
                    rip_curr.Id_Reg = recodeRI.To_Id_Reg
                    rip_curr.Progetto_Cod = recodeP.To_Progetto_cod
                Next
            Next

            rval.reg_impianti_programmazioni_insert = New List(Of Reg_Impianti_Programmazioni)
            rval.reg_impianti_programmazioni_insert.AddRange(listaRIP_Insert)
            rval.reg_impianti_programmazioni_insert.AddRange(listaRIP_Update)

        End Using

        Return rval

    End Function

    Public Function LeggiPerGias2GiasReverse(ByVal piva As String,
                                      ByVal piva_dest As String,
                                      ByVal PivaSuperUser_Destinazione As String,
                                      ByVal objOpzioniImportImpresa As clsImpresa,
                                      ByRef objParametri_server As AgronicaCoreParametri) As G2G_Planning_Reverse

        Dim gefutils As New AgronicaCoreEntityFramework.Gias_EF_Utility
        Dim efConnString As String = gefutils.GetEntityConnectionString(objParametri_server.StringaConnessione)
        Dim psu As String = objParametri_server.PivaSuperUser

        Dim rval As New G2G_Planning_Reverse With {
            .From_Piva = piva,
            .From_PivaSuperUser = objParametri_server.PivaSuperUser,
            .To_Piva = piva_dest,
            .Recode = G2GUtility.Nuovo_G2G_Recode()
        }

        Using giasContext As New AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities(efConnString)
            giasContext.Database.CommandTimeout = 3600
            'PROGARMMAZIONE_TESTATA
            rval.programmazione_testata_insert = (
            From pt In giasContext.Programmazione_Testata
            Where pt.Piva = piva _
                AndAlso pt.Validita_Fine >= objOpzioniImportImpresa.ValiditaInizio_planning _
                AndAlso Not giasContext.G2G_Recode_Programmazione_Testata.Any(Function(g) g.To_PivaSuperUser = pt.Piva_SuperUser AndAlso g.From_PivaSuperUser = PivaSuperUser_Destinazione AndAlso g.To_Piva = pt.Piva AndAlso g.To_Programmazione_Cod = pt.Programmazione_Cod)
            Select pt).ToList()

            rval.programmazione_testata_update = (
            From pt In giasContext.Programmazione_Testata
            Join g2g In giasContext.G2G_Recode_Programmazione_Testata
                On pt.Piva_SuperUser Equals g2g.To_PivaSuperUser _
                And pt.Piva Equals g2g.To_Piva _
                And pt.Programmazione_Cod Equals g2g.To_Programmazione_Cod
            Where pt.Piva = piva _
                AndAlso g2g.From_PivaSuperUser = PivaSuperUser_Destinazione _
                AndAlso System.Data.Entity.DbFunctions.AddDays(g2g.Data_Modifica, -1) <= pt.Data_Modifica
            Select pt
            ).ToList()

            rval.Recode.G2GRecodeProgrammazioneTestataToDelete = (
            From r In giasContext.G2G_Recode_Programmazione_Testata
            Where r.To_PivaSuperUser = psu _
                AndAlso r.To_Piva = piva _
                AndAlso r.From_PivaSuperUser = PivaSuperUser_Destinazione _
                AndAlso Not giasContext.Programmazione_Testata.Any(Function(pt) pt.Piva_SuperUser = r.To_PivaSuperUser AndAlso pt.Piva = r.To_Piva AndAlso pt.Programmazione_Cod = r.To_Programmazione_Cod)
            Select r).ToList()

            'PROGARMMAZIONE_ENTITA
            rval.programmazione_entita_insert = (
            From pe In giasContext.Programmazione_Entita
            Join pt In giasContext.Programmazione_Testata
                On pt.Piva_SuperUser Equals pe.Piva_SuperUser _
                And pt.Piva Equals pe.Piva _
                And pt.Programmazione_Cod Equals pe.Programmazione_Cod
            Group Join g In giasContext.G2G_Recode_Programmazione_Entita.Where(Function(x) x.From_PivaSuperUser = PivaSuperUser_Destinazione) On
                                                                        g.To_PivaSuperUser Equals pe.Piva_SuperUser _
                                                                        And g.To_Piva Equals pe.Piva _
                                                                        And g.To_Programmazione_Entita_Cod Equals pe.Programmazione_Entita_Cod
                Into g_group = Group
            From _g_group In g_group.DefaultIfEmpty()
            Where pe.Piva = piva _
                AndAlso pt.Validita_Fine >= objOpzioniImportImpresa.ValiditaInizio_planning _
                AndAlso _g_group Is Nothing
            Select pe).ToList()

            'AndAlso Not giasContext.G2G_Recode_Programmazione_Entita.Any(Function(g) g.From_PivaSuperUser = pe.Piva_SuperUser AndAlso g.To_PivaSuperUser = PivaSuperUser_Destinazione AndAlso g.From_Piva = pe.Piva AndAlso g.From_Programmazione_Entita_Cod = pe.Programmazione_Entita_Cod)

            rval.programmazione_entita_update = (
            From pe In giasContext.Programmazione_Entita
            Join g2g In giasContext.G2G_Recode_Programmazione_Entita
                On pe.Piva_SuperUser Equals g2g.To_PivaSuperUser _
                And pe.Piva Equals g2g.To_Piva _
                And pe.Programmazione_Entita_Cod Equals g2g.To_Programmazione_Entita_Cod
            Where pe.Piva = piva _
                AndAlso g2g.From_PivaSuperUser = PivaSuperUser_Destinazione _
                AndAlso System.Data.Entity.DbFunctions.AddDays(g2g.Data_Modifica, -1) <= pe.Data_Modifica
            Select pe
            ).ToList()

            'Sistemo le codifiche di piva/sacod/appezza/idreg/distinta
            For Each list In {rval.programmazione_entita_insert, rval.programmazione_entita_update}
                For Each pe_curr In list

                    'NB: mantenere questo ordine perché altrimenti ad esempio la sostituzione della piva all'inizio farebbe sì che non trovano mapping tutti gli altri
                    If pe_curr.Progetto_Cod > 0 Then
                        pe_curr.Progetto_Cod = (From rr In giasContext.G2G_Recode_Distinta Where rr.To_PivaSuperUser = psu AndAlso rr.From_PivaSuperUser = PivaSuperUser_Destinazione AndAlso rr.To_Piva = pe_curr.Piva AndAlso rr.To_Progetto_cod = pe_curr.Progetto_Cod Select rr.From_Progetto_cod).First()
                    End If

                    If pe_curr.Id_Reg > 0 Then
                        pe_curr.Id_Reg = (From rr In giasContext.G2G_Recode_Impianti Where rr.To_PivaSuperUser = psu AndAlso rr.From_PivaSuperUser = PivaSuperUser_Destinazione AndAlso rr.To_Piva = pe_curr.Piva AndAlso rr.To_Sa_Cod = pe_curr.Sa_Cod AndAlso rr.To_Appezza = pe_curr.Appezza AndAlso rr.To_Id_Reg = pe_curr.Id_Reg Select rr.From_Id_Reg).First()
                    End If

                    If pe_curr.Appezza > 0 Then
                        pe_curr.Appezza = (From rr In giasContext.G2G_Recode_Appezzamenti Where rr.To_PivaSuperUser = psu AndAlso rr.From_PivaSuperUser = PivaSuperUser_Destinazione AndAlso rr.To_Piva = pe_curr.Piva AndAlso rr.To_Sa_Cod = pe_curr.Sa_Cod AndAlso rr.To_Appezza = pe_curr.Appezza Select rr.From_Appezza).First()
                    End If

                    If pe_curr.Campo_Cod > 0 Then
                        pe_curr.Campo_Cod = (From rr In giasContext.G2G_Recode_Campo Where rr.From_PivaSuperUser = psu AndAlso rr.To_PivaSuperUser = PivaSuperUser_Destinazione AndAlso rr.From_Piva = pe_curr.Piva AndAlso rr.From_Sa_Cod = pe_curr.Sa_Cod AndAlso rr.From_Campo_cod = pe_curr.Campo_Cod Select rr.To_Campo_cod).First()
                    End If

                    If pe_curr.Sa_Cod > 0 Then
                        pe_curr.Sa_Cod = (From rr In giasContext.G2G_Recode_Imprese Where rr.To_PivaSuperUser = psu AndAlso rr.From_PivaSuperUser = PivaSuperUser_Destinazione AndAlso rr.To_Piva = pe_curr.Piva AndAlso rr.TO_SaCod = pe_curr.Sa_Cod Select rr.FROM_SaCod).First()
                    End If

                    If pe_curr.Piva <> "" Then
                        pe_curr.Piva = (From rr In giasContext.G2G_Recode_Imprese Where rr.To_PivaSuperUser = psu AndAlso rr.From_PivaSuperUser = PivaSuperUser_Destinazione AndAlso rr.To_Piva = pe_curr.Piva AndAlso rr.TO_SaCod = 0 Select rr.FROM_Piva).First()
                    End If

                Next
            Next

            rval.Recode.G2GRecodeProgrammazioneEntitaToDelete = (
            From r In giasContext.G2G_Recode_Programmazione_Entita
            Where r.From_PivaSuperUser = psu _
                AndAlso r.From_Piva = piva _
                AndAlso r.From_PivaSuperUser = PivaSuperUser_Destinazione _
                AndAlso Not giasContext.Programmazione_Entita.Any(Function(pe) pe.Piva_SuperUser = r.To_PivaSuperUser AndAlso pe.Piva = r.To_Piva AndAlso pe.Programmazione_Entita_Cod = r.To_Programmazione_Entita_Cod)
            Select r).ToList()

            'PROGARMMAZIONE_ENTITA_CODICI
            Dim listaPEC_Insert As List(Of Programmazione_Entita_Codici) = (
            From pec In giasContext.Programmazione_Entita_Codici
            Join pe In giasContext.Programmazione_Entita
                    On pe.Piva_SuperUser Equals pec.Piva_SuperUser _
                    And pe.Programmazione_Entita_Cod Equals pec.Programmazione_Entita_Cod
            Join pt In giasContext.Programmazione_Testata
                On pt.Piva_SuperUser Equals pe.Piva_SuperUser _
                And pt.Piva Equals pe.Piva _
                And pt.Programmazione_Cod Equals pe.Programmazione_Cod
            Group Join g In giasContext.G2G_Recode_Programmazione_Entita.Where(Function(x) x.From_PivaSuperUser = PivaSuperUser_Destinazione) On
                                                                        g.To_PivaSuperUser Equals pe.Piva_SuperUser _
                                                                        And g.To_Piva Equals pe.Piva _
                                                                        And g.To_Programmazione_Entita_Cod Equals pe.Programmazione_Entita_Cod
                Into g_group = Group
            From _g_group In g_group.DefaultIfEmpty()
            Where pe.Piva = piva _
                AndAlso pt.Validita_Fine >= objOpzioniImportImpresa.ValiditaInizio_planning _
                AndAlso _g_group Is Nothing
            Select pec).ToList()

            'AndAlso Not giasContext.G2G_Recode_Programmazione_Entita.Any(Function(g) g.From_PivaSuperUser = pe.Piva_SuperUser And g.To_PivaSuperUser = PivaSuperUser_Destinazione And g.From_Programmazione_Entita_Cod = pe.Programmazione_Entita_Cod And g.From_Piva = piva)

            Dim listaPEC_Update As List(Of Programmazione_Entita_Codici) = (
            From pec In giasContext.Programmazione_Entita_Codici
            Join pe In giasContext.Programmazione_Entita
                On pe.Piva_SuperUser Equals pec.Piva_SuperUser _
                And pe.Programmazione_Entita_Cod Equals pec.Programmazione_Entita_Cod
            Join g2g In giasContext.G2G_Recode_Programmazione_Entita
                On pe.Piva_SuperUser Equals g2g.To_PivaSuperUser _
                And pe.Programmazione_Entita_Cod Equals g2g.To_Programmazione_Entita_Cod
            Where pe.Piva = piva _
                AndAlso g2g.From_PivaSuperUser = PivaSuperUser_Destinazione _
                AndAlso System.Data.Entity.DbFunctions.AddDays(g2g.Data_Modifica, -1) <= pe.Data_Modifica
            Select pec
            ).ToList()

            rval.programmazione_entita_codici_insert = New List(Of Programmazione_Entita_Codici)
            rval.programmazione_entita_codici_insert.AddRange(listaPEC_Insert)
            rval.programmazione_entita_codici_insert.AddRange(listaPEC_Update)

            'PROGARMMAZIONE_PARTICELLE
            Dim listaPP_Insert As List(Of Programmazione_Particelle) = (
            From pp In giasContext.Programmazione_Particelle
            Join pe In giasContext.Programmazione_Entita
                    On pe.Piva_SuperUser Equals pp.Piva_SuperUser _
                    And pe.Programmazione_Entita_Cod Equals pp.Programmazione_Entita_Cod
            Join pt In giasContext.Programmazione_Testata
                On pt.Piva_SuperUser Equals pe.Piva_SuperUser _
                And pt.Piva Equals pe.Piva _
                And pt.Programmazione_Cod Equals pe.Programmazione_Cod
            Group Join g In giasContext.G2G_Recode_Programmazione_Entita.Where(Function(x) x.From_PivaSuperUser = PivaSuperUser_Destinazione) On
                                                                        g.To_PivaSuperUser Equals pe.Piva_SuperUser _
                                                                        And g.To_Piva Equals pe.Piva _
                                                                        And g.To_Programmazione_Entita_Cod Equals pe.Programmazione_Entita_Cod
                Into g_group = Group
            From _g_group In g_group.DefaultIfEmpty()
            Where pe.Piva = piva _
                AndAlso pt.Validita_Fine >= objOpzioniImportImpresa.ValiditaInizio_planning _
                AndAlso _g_group Is Nothing
            Select pp).ToList()

            'AndAlso Not giasContext.G2G_Recode_Programmazione_Entita.Any(Function(g) g.From_PivaSuperUser = pe.Piva_SuperUser And g.To_PivaSuperUser = PivaSuperUser_Destinazione And g.From_Programmazione_Entita_Cod = pe.Programmazione_Entita_Cod And g.From_Piva = piva)

            Dim listaPP_Update As List(Of Programmazione_Particelle) = (
            From pp In giasContext.Programmazione_Particelle
            Join pe In giasContext.Programmazione_Entita
                On pe.Piva_SuperUser Equals pp.Piva_SuperUser _
                And pe.Programmazione_Entita_Cod Equals pp.Programmazione_Entita_Cod
            Join g2g In giasContext.G2G_Recode_Programmazione_Entita
                On pe.Piva_SuperUser Equals g2g.To_PivaSuperUser _
                And pe.Programmazione_Entita_Cod Equals g2g.To_Programmazione_Entita_Cod
            Where pe.Piva = piva _
                AndAlso g2g.From_PivaSuperUser = PivaSuperUser_Destinazione _
                AndAlso System.Data.Entity.DbFunctions.AddDays(g2g.Data_Modifica, -1) <= pe.Data_Modifica
            Select pp
            ).ToList()

            rval.programmazione_particelle_insert = New List(Of Programmazione_Particelle)
            rval.programmazione_particelle_insert.AddRange(listaPP_Insert)
            rval.programmazione_particelle_insert.AddRange(listaPP_Update)

            'REG_IMPIANTI_PROGARMMAZIONI
            Dim listaRIP_Insert As List(Of Reg_Impianti_Programmazioni) = (
            From rip In giasContext.Reg_Impianti_Programmazioni
            Join pe In giasContext.Programmazione_Entita
                    On pe.Piva_SuperUser Equals rip.Piva_SuperUser _
                    And pe.Programmazione_Entita_Cod Equals rip.Programmazione_Entita_Cod _
                    And pe.Programmazione_Cod Equals rip.Programmazione_Cod
            Join pt In giasContext.Programmazione_Testata
                On pt.Piva_SuperUser Equals pe.Piva_SuperUser _
                And pt.Piva Equals pe.Piva _
                And pt.Programmazione_Cod Equals pe.Programmazione_Cod
            Group Join g In giasContext.G2G_Recode_Programmazione_Entita.Where(Function(x) x.From_PivaSuperUser = PivaSuperUser_Destinazione) On
                                                                        g.To_PivaSuperUser Equals pe.Piva_SuperUser _
                                                                        And g.To_Piva Equals pe.Piva _
                                                                        And g.To_Programmazione_Entita_Cod Equals pe.Programmazione_Entita_Cod
                Into g_group = Group
            From _g_group In g_group.DefaultIfEmpty()
            Where pe.Piva = piva _
                AndAlso pt.Validita_Fine >= objOpzioniImportImpresa.ValiditaInizio_planning _
                AndAlso _g_group Is Nothing
            Select rip).ToList()

            'AndAlso Not giasContext.G2G_Recode_Programmazione_Entita.Any(Function(g) g.From_PivaSuperUser = pe.Piva_SuperUser And g.To_PivaSuperUser = PivaSuperUser_Destinazione And g.From_Programmazione_Entita_Cod = pe.Programmazione_Entita_Cod And g.From_Piva = piva)

            Dim listaRIP_Update As List(Of Reg_Impianti_Programmazioni) = (
            From rip In giasContext.Reg_Impianti_Programmazioni
            Join pe In giasContext.Programmazione_Entita
                On pe.Piva_SuperUser Equals rip.Piva_SuperUser _
                And pe.Programmazione_Entita_Cod Equals rip.Programmazione_Entita_Cod _
                And pe.Programmazione_Cod Equals rip.Programmazione_Cod
            Join g2g In giasContext.G2G_Recode_Programmazione_Entita
                On pe.Piva_SuperUser Equals g2g.To_PivaSuperUser _
                And pe.Programmazione_Entita_Cod Equals g2g.To_Programmazione_Entita_Cod
            Where pe.Piva = piva _
                AndAlso g2g.From_PivaSuperUser = PivaSuperUser_Destinazione _
                AndAlso System.Data.Entity.DbFunctions.AddDays(g2g.Data_Modifica, -1) <= pe.Data_Modifica
            Select rip
            ).ToList()

            'Sistemo le codifiche di piva/sacod/appezza/idreg/distinta
            For Each list In {listaRIP_Insert, listaRIP_Update}
                For Each rip_curr In list
                    Dim recodeRI = (From rr In giasContext.G2G_Recode_Impianti Where rr.To_PivaSuperUser = psu AndAlso rr.From_PivaSuperUser = PivaSuperUser_Destinazione AndAlso rr.To_Piva = rip_curr.Piva AndAlso rr.To_Sa_Cod = rip_curr.Sa_Cod AndAlso rr.To_Appezza = rip_curr.Appezza AndAlso rr.To_Id_Reg = rip_curr.Id_Reg).First()
                    Dim recodeP = (From rr In giasContext.G2G_Recode_Distinta Where rr.To_PivaSuperUser = psu AndAlso rr.From_PivaSuperUser = PivaSuperUser_Destinazione AndAlso rr.To_Piva = rip_curr.Piva AndAlso rr.To_Progetto_cod = rip_curr.Progetto_Cod).First()

                    rip_curr.Sa_Cod = recodeRI.From_Sa_Cod
                    rip_curr.Appezza = recodeRI.From_Appezza
                    rip_curr.Id_Reg = recodeRI.From_Id_Reg
                    rip_curr.Progetto_Cod = recodeP.From_Progetto_cod
                Next
            Next

            rval.reg_impianti_programmazioni_insert = New List(Of Reg_Impianti_Programmazioni)
            rval.reg_impianti_programmazioni_insert.AddRange(listaRIP_Insert)
            rval.reg_impianti_programmazioni_insert.AddRange(listaRIP_Update)

        End Using

        Return rval

    End Function

End Class

Public Class G2GPlanning_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Scrivi_Planning_G2G(Origine_Piva_SuperUser As String, Destinazione_Piva_SuperUser As String, ByRef g2g As G2G_Planning, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As G2G_Recode

        Dim nomeRoutine As String = "AgronicaCoreG2GLocalDAL.G2GPlanning_W.Scrivi_Planning_G2G()"

        Dim username = objParametri.UsernameOperazione
        Dim piva = g2g.To_Piva
        Dim From_Piva = g2g.From_Piva
        Dim data = Date.Now

        Dim objSequenze = New AgronicaCoreDataProvider.Agro_Sequenze
        Dim prat As New G2GPratiche_R

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        'Dim transactionOptions As New TransactionOptions()
        'transactionOptions.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted
        'transactionOptions.Timeout = G2GUtility.GetTransactionTimeout(objParametri)

        Try

            'Using scope As New TransactionScope(TransactionScopeOption.Required, transactionOptions)

            Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

                Using dbContextTransaction = GiasContext.Database.BeginTransaction()

                    Dim Hash_recodePT = New Hashtable
                    Dim Hash_PT = New Hashtable
                    Dim Hash_recodePE = New Hashtable

                    'GiasContext.Database.Connection.Open()

                    'PROGRAMMAZIONE_TESTATA - INSERT
                    Try

                        For Each pt_curr As Programmazione_Testata In g2g.programmazione_testata_insert

                            'Richiedo un nuovo id sequenza
                            Dim idSeq As Integer = objSequenze.NuovoId_Tabella("Programmazione_Testata", 0, 2000000000, objParametri, UtilizzaTransazione:=False)

                            Dim pt_2Add = Gias_EF_Utility.CopyEntity(GiasContext, pt_curr, Nothing, username, data)
                            pt_2Add.Piva = piva
                            pt_2Add.Programmazione_Cod = idSeq
                            pt_2Add.Piva_SuperUser = Destinazione_Piva_SuperUser

                            'Sistemo le pratiche
                            Dim newStrPratiche As String = prat.ricodifica_Pratica_Cod(GiasContext, Origine_Piva_SuperUser, pt_curr.Pratica_Cod)
                            If Not String.IsNullOrEmpty(newStrPratiche) Then
                                pt_2Add.Pratica_Cod = newStrPratiche
                            End If

                            GiasContext.Programmazione_Testata.Add(pt_2Add)

                            Dim recode =
                                New G2G_Recode_Programmazione_Testata With {
                                    .From_PivaSuperUser = Origine_Piva_SuperUser,
                                    .To_PivaSuperUser = Destinazione_Piva_SuperUser,
                                    .From_Piva = pt_curr.Piva,
                                    .To_Piva = piva,
                                    .From_Programmazione_Cod = pt_curr.Programmazione_Cod,
                                    .To_Programmazione_Cod = idSeq,
                                    .Username_Creazione = username,
                                    .Username_Modifica = username,
                                    .Data_Creazione = Now(),
                                    .Data_Modifica = Now(),
                                    .Validita_Inizio = AGRODATAINIZIO,
                                    .Validita_Fine = AGRODATAFINE,
                                    .inviato = "0",
                                    .datainvio = Now()
                                }
                            g2g.Recode.G2GRecodeProgrammazioneTestataToInsert.Add(recode)
                            GiasContext.G2G_Recode_Programmazione_Testata.Add(recode)

                        Next

                        GiasContext.SaveChanges()

                    Catch ex As Exception
                        Throw New Exception("Err durante l'inserimento di PT: " & ex.Message)
                    End Try

                    'PROGRAMMAZIONE_TESTATA - UPDATE
                    Try

                        For Each pt_curr As Programmazione_Testata In g2g.programmazione_testata_update

                            'Aggiorno i recode per gli elementi aggiornati
                            Dim recode = (From rr In GiasContext.G2G_Recode_Programmazione_Testata Where rr.From_PivaSuperUser = Origine_Piva_SuperUser AndAlso rr.To_PivaSuperUser = Destinazione_Piva_SuperUser AndAlso rr.From_Piva = pt_curr.Piva AndAlso rr.From_Programmazione_Cod = pt_curr.Programmazione_Cod).FirstOrDefault()
                            recode.Username_Modifica = username
                            recode.Data_Modifica = data
                            recode.datainvio = data
                            g2g.Recode.G2GRecodeProgrammazioneTestataToUpdate.Add(recode)
                            GiasContext.G2G_Recode_Programmazione_Testata.Attach(recode)
                            GiasContext.Entry(recode).State = EntityState.Modified

                            'Modfico le Testate
                            Dim pt_2Upd = (From x In GiasContext.Programmazione_Testata Where x.Piva_SuperUser = recode.To_PivaSuperUser AndAlso x.Piva = recode.To_Piva AndAlso x.Programmazione_Cod = recode.To_Programmazione_Cod).FirstOrDefault()
                            pt_2Upd = Gias_EF_Utility.CopyEntity(GiasContext, pt_curr, pt_2Upd, username, data)
                            pt_2Upd.Piva_SuperUser = Destinazione_Piva_SuperUser
                            pt_2Upd.Piva = piva
                            pt_2Upd.Programmazione_Cod = recode.To_Programmazione_Cod

                            'Sistemo le pratiche
                            Dim newStrPratiche As String = prat.ricodifica_Pratica_Cod(GiasContext, Origine_Piva_SuperUser, pt_curr.Pratica_Cod)
                            If Not String.IsNullOrEmpty(newStrPratiche) Then
                                pt_2Upd.Pratica_Cod = newStrPratiche
                            End If

                            GiasContext.Programmazione_Testata.Attach(pt_2Upd)
                            GiasContext.Entry(pt_2Upd).State = EntityState.Modified

                        Next

                        GiasContext.SaveChanges()

                    Catch ex As Exception
                        Throw New Exception("Err durante l'aggiornamento di PT: " & ex.Message)
                    End Try

                    'PROGRAMMAZIONE_TESTATA - DELETE
                    Try

                        For Each recode As G2G_Recode_Programmazione_Testata In g2g.Recode.G2GRecodeProgrammazioneTestataToDelete

                            'Cancello la Testata
                            Dim plan = (From pt In GiasContext.Programmazione_Testata Where pt.Piva_SuperUser = Destinazione_Piva_SuperUser AndAlso recode.To_Piva = pt.Piva AndAlso recode.To_Programmazione_Cod = pt.Programmazione_Cod).FirstOrDefault()

                            If Not IsNothing(plan) Then
                                GiasContext.Programmazione_Testata.Attach(plan)
                                GiasContext.Programmazione_Testata.Remove(plan)
                            End If

                            ' cancella il recode corrispondente
                            Dim recode2Delete = (From rr In GiasContext.G2G_Recode_Programmazione_Testata
                                                 Where rr.From_PivaSuperUser = recode.From_PivaSuperUser AndAlso rr.From_Piva = recode.From_Piva AndAlso rr.From_Programmazione_Cod = recode.From_Programmazione_Cod _
                                             AndAlso rr.To_PivaSuperUser = recode.To_PivaSuperUser AndAlso rr.To_Piva = recode.To_Piva AndAlso rr.To_Programmazione_Cod = recode.To_Programmazione_Cod
                                                 Select rr).FirstOrDefault()

                            If Not IsNothing(recode2Delete) Then
                                GiasContext.G2G_Recode_Programmazione_Testata.Attach(recode2Delete)
                                GiasContext.G2G_Recode_Programmazione_Testata.Remove(recode2Delete)
                            End If

                        Next

                        GiasContext.SaveChanges()

                    Catch ex As Exception
                        Throw New Exception("Err durante la cancellazione di PT: " & ex.Message)
                    End Try


                    'PROGRAMMAZIONE_ENTITA - INSERT
                    Try
                        If g2g.programmazione_entita_insert.Count > 0 Then
                            Dim listProgrammazione_Entita_Cod As New List(Of Programmazione_Entita)
                            Dim listRecodeProgrammazione_Entita_Cod As New List(Of G2G_Recode_Programmazione_Entita)
                            For Each pe_curr As Programmazione_Entita In g2g.programmazione_entita_insert

                                'Richiedo un nuovo id sequenza
                                Dim idSeq As Integer = objSequenze.NuovoId_Tabella("Programmazione_Entita", 0, 2000000000, objParametri, UtilizzaTransazione:=False)

                                Dim chiaveRecodePT = Origine_Piva_SuperUser & "_" & Destinazione_Piva_SuperUser & "_" & From_Piva & "_" & pe_curr.Programmazione_Cod
                                Dim recodePT As G2G_Recode_Programmazione_Testata = Nothing
                                If Not Hash_recodePT.Contains(chiaveRecodePT) Then
                                    recodePT = (From rr In GiasContext.G2G_Recode_Programmazione_Testata Where rr.From_PivaSuperUser = Origine_Piva_SuperUser AndAlso rr.To_PivaSuperUser = Destinazione_Piva_SuperUser AndAlso rr.From_Piva = From_Piva AndAlso rr.From_Programmazione_Cod = pe_curr.Programmazione_Cod).FirstOrDefault()
                                    If IsNothing(recodePT) Then
                                        Throw New Exception("Impossibile trovare il recode con FromPT=" & pe_curr.Programmazione_Cod)
                                    End If
                                    Hash_recodePT.Add(chiaveRecodePT, recodePT)
                                Else
                                    recodePT = Hash_recodePT(chiaveRecodePT)
                                End If

                                Dim pe_2Add = Gias_EF_Utility.CopyEntity(GiasContext, pe_curr, Nothing, username, data)
                                'pe_2Add.Piva = piva
                                pe_2Add.Programmazione_Entita_Cod = idSeq
                                pe_2Add.Programmazione_Cod = recodePT.To_Programmazione_Cod
                                pe_2Add.Piva_SuperUser = Destinazione_Piva_SuperUser

                                'Per ora lo tengo nell'origine perché in destinazione non ho i recode
                                'If pe_curr.Sa_Cod > 0 Then
                                '    pe_2Add.Sa_Cod = (From rr In GiasContext.G2G_Recode_Imprese Where rr.From_PivaSuperUser = Origine_Piva_SuperUser AndAlso rr.FROM_Piva = pe_curr.Piva AndAlso rr.FROM_SaCod = pe_curr.Sa_Cod Select rr.TO_SaCod).First()
                                'End If

                                'If pe_curr.Campo_Cod > 0 Then
                                '    pe_2Add.Campo_Cod = (From rr In GiasContext.G2G_Recode_Campo Where rr.From_PivaSuperUser = Origine_Piva_SuperUser AndAlso rr.From_Piva = pe_curr.Piva AndAlso rr.From_Sa_Cod = pe_curr.Sa_Cod AndAlso rr.From_Campo_cod = pe_curr.Campo_Cod Select rr.To_Campo_cod).First()
                                'End If

                                'If pe_curr.Appezza > 0 Then
                                '    pe_2Add.Appezza = (From rr In GiasContext.G2G_Recode_Appezzamenti Where rr.From_PivaSuperUser = Origine_Piva_SuperUser AndAlso rr.From_Piva = pe_curr.Piva AndAlso rr.From_Sa_Cod = pe_curr.Sa_Cod AndAlso rr.From_Appezza = pe_curr.Appezza Select rr.To_Appezza).First()
                                'End If

                                'If pe_curr.Id_Reg > 0 Then
                                '    pe_2Add.Id_Reg = (From rr In GiasContext.G2G_Recode_Impianti Where rr.From_PivaSuperUser = Origine_Piva_SuperUser AndAlso rr.From_Piva = pe_curr.Piva AndAlso rr.From_Sa_Cod = pe_curr.Sa_Cod AndAlso rr.From_Appezza = pe_curr.Appezza AndAlso rr.From_Id_Reg = pe_curr.Id_Reg Select rr.To_Id_Reg).First()
                                'End If

                                'If pe_curr.Progetto_Cod > 0 Then
                                '    pe_2Add.Progetto_Cod = (From rr In GiasContext.G2G_Recode_Distinta Where rr.From_PivaSuperUser = Origine_Piva_SuperUser AndAlso rr.From_Piva = pe_curr.Piva AndAlso rr.From_Progetto_cod = pe_curr.Progetto_Cod Select rr.To_Progetto_cod).First()
                                'End If

                                'GiasContext.Programmazione_Entita.Add(pe_2Add)
                                listProgrammazione_Entita_Cod.Add(pe_2Add)

                                Dim recode =
                                New G2G_Recode_Programmazione_Entita With {
                                    .From_PivaSuperUser = Origine_Piva_SuperUser,
                                    .To_PivaSuperUser = Destinazione_Piva_SuperUser,
                                    .From_Piva = From_Piva,
                                    .To_Piva = piva,
                                    .From_Programmazione_Entita_Cod = pe_curr.Programmazione_Entita_Cod,
                                    .To_Programmazione_Entita_Cod = idSeq,
                                    .Username_Creazione = username,
                                    .Username_Modifica = username,
                                    .Data_Creazione = Now(),
                                    .Data_Modifica = Now(),
                                    .Validita_Inizio = AGRODATAINIZIO,
                                    .Validita_Fine = AGRODATAFINE,
                                    .inviato = "0",
                                    .datainvio = Now()
                                }
                                g2g.Recode.G2GRecodeProgrammazioneEntitaToInsert.Add(recode)

                                'GiasContext.G2G_Recode_Programmazione_Entita.Add(recode)
                                listRecodeProgrammazione_Entita_Cod.Add(recode)

                            Next

                            GiasContext.Programmazione_Entita.AddRange(listProgrammazione_Entita_Cod)
                            GiasContext.G2G_Recode_Programmazione_Entita.AddRange(listRecodeProgrammazione_Entita_Cod)

                            GiasContext.SaveChanges()

                        End If

                    Catch ex As Exception
                        Throw New Exception("Err durante il salvataggio di PE: " & ex.Message)
                    End Try

                    'PROGRAMMAZIONE_ENTITA - UPDATE
                    Try

                        If g2g.programmazione_entita_update.Count > 0 Then
                            For Each pe_curr As Programmazione_Entita In g2g.programmazione_entita_update

                                'Aggiorno i recode per gli elementi aggiornati
                                Dim recode = (From rr In GiasContext.G2G_Recode_Programmazione_Entita Where rr.From_PivaSuperUser = Origine_Piva_SuperUser AndAlso rr.From_Piva = From_Piva AndAlso rr.From_Programmazione_Entita_Cod = pe_curr.Programmazione_Entita_Cod).FirstOrDefault()
                                If IsNothing(recode) Then
                                    Throw New Exception("Impossibile trovare il recode con FromPE=" & pe_curr.Programmazione_Entita_Cod)
                                End If

                                recode.Username_Modifica = username
                                recode.Data_Modifica = data
                                recode.datainvio = data
                                g2g.Recode.G2GRecodeProgrammazioneEntitaToUpdate.Add(recode)
                                GiasContext.G2G_Recode_Programmazione_Entita.Attach(recode)
                                GiasContext.Entry(recode).State = EntityState.Modified

                                Dim chiaveRecodePT = Origine_Piva_SuperUser & "_" & Destinazione_Piva_SuperUser & "_" & From_Piva & "_" & pe_curr.Programmazione_Cod
                                Dim recodePT As G2G_Recode_Programmazione_Testata = Nothing
                                If Not Hash_recodePT.Contains(chiaveRecodePT) Then
                                    recodePT = (From rr In GiasContext.G2G_Recode_Programmazione_Testata Where rr.From_PivaSuperUser = Origine_Piva_SuperUser AndAlso rr.To_PivaSuperUser = Destinazione_Piva_SuperUser AndAlso rr.From_Piva = From_Piva AndAlso rr.From_Programmazione_Cod = pe_curr.Programmazione_Cod).FirstOrDefault()
                                    If IsNothing(recodePT) Then
                                        Throw New Exception("Impossibile trovare il recode con FromPT=" & pe_curr.Programmazione_Cod)
                                    End If
                                    Hash_recodePT.Add(chiaveRecodePT, recodePT)
                                Else
                                    recodePT = Hash_recodePT(chiaveRecodePT)
                                End If

                                Dim chiavePT = Origine_Piva_SuperUser & "_" & Destinazione_Piva_SuperUser & "_" & pe_curr.Piva & "_" & recodePT.To_Programmazione_Cod
                                Dim PT As Programmazione_Testata
                                If Not Hash_PT.Contains(chiavePT) Then
                                    PT = (From rr In GiasContext.Programmazione_Testata Where rr.Piva_SuperUser = Destinazione_Piva_SuperUser AndAlso rr.Piva = pe_curr.Piva AndAlso rr.Programmazione_Cod = recodePT.To_Programmazione_Cod).FirstOrDefault()
                                    If IsNothing(PT) Then
                                        Throw New Exception("Impossibile trovare la testata con PT=" & recodePT.To_Programmazione_Cod)
                                    End If
                                    Hash_PT.Add(chiavePT, PT)
                                Else
                                    PT = Hash_PT(chiavePT)
                                End If


                                'Modfico le entità
                                Dim pe_2Upd = (From x In GiasContext.Programmazione_Entita Where x.Piva_SuperUser = recode.To_PivaSuperUser AndAlso x.Piva = recode.To_Piva AndAlso x.Programmazione_Entita_Cod = recode.To_Programmazione_Entita_Cod).FirstOrDefault()
                                pe_2Upd = Gias_EF_Utility.CopyEntity(GiasContext, pe_curr, pe_2Upd, username, data)
                                pe_2Upd.Piva_SuperUser = Destinazione_Piva_SuperUser
                                'pe_2Upd.Piva = piva
                                pe_2Upd.Programmazione_Cod = recodePT.To_Programmazione_Cod
                                pe_2Upd.Programmazione_Testata = PT
                                pe_2Upd.Programmazione_Entita_Cod = recode.To_Programmazione_Entita_Cod
                                GiasContext.Programmazione_Entita.Attach(pe_2Upd)
                                GiasContext.Entry(pe_2Upd).State = EntityState.Modified

                                'Cancello le Programmazione_Entita_Codici (si cancella e riscrive)
                                Dim pec_2Del_list = (From x In GiasContext.Programmazione_Entita_Codici Where x.Piva_SuperUser = Destinazione_Piva_SuperUser AndAlso x.Programmazione_Entita_Cod = recode.To_Programmazione_Entita_Cod)
                                For Each pec_2Del In pec_2Del_list
                                    GiasContext.Programmazione_Entita_Codici.Attach(pec_2Del)
                                    GiasContext.Programmazione_Entita_Codici.Remove(pec_2Del)
                                Next

                                'Cancello le Programmazione_Particelle (si cancella e riscrive)
                                Dim pp_2Del_list = (From x In GiasContext.Programmazione_Particelle Where x.Piva_SuperUser = Destinazione_Piva_SuperUser AndAlso x.Programmazione_Entita_Cod = recode.To_Programmazione_Entita_Cod)
                                For Each pp_2Del In pp_2Del_list
                                    GiasContext.Programmazione_Particelle.Attach(pp_2Del)
                                    GiasContext.Programmazione_Particelle.Remove(pp_2Del)
                                Next

                                'Cancello le Reg_Impianti_Programmazioni (si cancella e riscrive)
                                Dim rip_2Del_list = (From x In GiasContext.Reg_Impianti_Programmazioni Where x.Piva_SuperUser = Destinazione_Piva_SuperUser AndAlso x.Programmazione_Entita_Cod = recode.To_Programmazione_Entita_Cod)
                                For Each rip_2Del In rip_2Del_list
                                    GiasContext.Reg_Impianti_Programmazioni.Attach(rip_2Del)
                                    GiasContext.Reg_Impianti_Programmazioni.Remove(rip_2Del)
                                Next

                            Next

                            GiasContext.SaveChanges()
                        End If



                    Catch ex As Exception
                        Throw New Exception("Err durante l'aggiornamento di PE (+ del PP/RIP): " & ex.Message)
                    End Try

                    'PROGRAMMAZIONE_ENTITA + PROGRAMMAZIONE_ENTITA_CODICI + PROGRAMMAZIONE_PARTICELLE + REG_IMPIANTI_PROGARMMAZIONI - DELETE
                    Try

                        For Each recode As G2G_Recode_Programmazione_Entita In g2g.Recode.G2GRecodeProgrammazioneEntitaToDelete

                            'Cancello i Programmazione_Entita_Codici 
                            Dim pec_2Del_list = (From pec In GiasContext.Programmazione_Entita_Codici
                                                 Join pe In GiasContext.Programmazione_Entita On pe.Piva_SuperUser Equals pec.Piva_SuperUser And pe.Programmazione_Entita_Cod Equals pec.Programmazione_Entita_Cod
                                                 Where pec.Piva_SuperUser = Destinazione_Piva_SuperUser AndAlso recode.To_Piva = pe.Piva AndAlso recode.To_Programmazione_Entita_Cod = pec.Programmazione_Entita_Cod
                                                 Select pec).ToList()
                            For Each pec_2Del In pec_2Del_list
                                GiasContext.Programmazione_Entita_Codici.Attach(pec_2Del)
                                GiasContext.Programmazione_Entita_Codici.Remove(pec_2Del)
                            Next

                            'Cancello i Programmazione_Particelle 
                            Dim pp_2Del_list = (From pp In GiasContext.Programmazione_Particelle
                                                Join pe In GiasContext.Programmazione_Entita On pe.Piva_SuperUser Equals pp.Piva_SuperUser And pe.Programmazione_Entita_Cod Equals pp.Programmazione_Entita_Cod
                                                Where pp.Piva_SuperUser = Destinazione_Piva_SuperUser AndAlso recode.To_Piva = pe.Piva AndAlso recode.To_Programmazione_Entita_Cod = pp.Programmazione_Entita_Cod
                                                Select pp).ToList()
                            For Each pp_2Del In pp_2Del_list
                                GiasContext.Programmazione_Particelle.Attach(pp_2Del)
                                GiasContext.Programmazione_Particelle.Remove(pp_2Del)
                            Next

                            'Cancello i Reg_Impianti_Programmazioni 
                            Dim rip_2Del_list = (From rip In GiasContext.Reg_Impianti_Programmazioni
                                                 Join pe In GiasContext.Programmazione_Entita On pe.Piva_SuperUser Equals rip.Piva_SuperUser And pe.Programmazione_Entita_Cod Equals rip.Programmazione_Entita_Cod
                                                 Where rip.Piva_SuperUser = Destinazione_Piva_SuperUser AndAlso recode.To_Piva = pe.Piva AndAlso recode.To_Programmazione_Entita_Cod = rip.Programmazione_Entita_Cod
                                                 Select rip).ToList()
                            For Each rip_2Del In rip_2Del_list
                                GiasContext.Reg_Impianti_Programmazioni.Attach(rip_2Del)
                                GiasContext.Reg_Impianti_Programmazioni.Remove(rip_2Del)
                            Next

                            'Cancello le entita
                            Dim plan = (From pe In GiasContext.Programmazione_Entita Where pe.Piva_SuperUser = Destinazione_Piva_SuperUser AndAlso recode.To_Piva = pe.Piva AndAlso recode.To_Programmazione_Entita_Cod = pe.Programmazione_Entita_Cod).FirstOrDefault()

                            If Not IsNothing(plan) Then
                                GiasContext.Programmazione_Entita.Attach(plan)
                                GiasContext.Programmazione_Entita.Remove(plan)
                            End If

                            ' cancella il recode corrispondente
                            Dim recode2Delete = (From rr In GiasContext.G2G_Recode_Programmazione_Entita
                                                 Where rr.From_PivaSuperUser = recode.From_PivaSuperUser AndAlso rr.From_Piva = recode.From_Piva AndAlso rr.From_Programmazione_Entita_Cod = recode.From_Programmazione_Entita_Cod _
                                             AndAlso rr.To_PivaSuperUser = recode.To_PivaSuperUser AndAlso rr.To_Piva = recode.To_Piva AndAlso rr.To_Programmazione_Entita_Cod = recode.To_Programmazione_Entita_Cod
                                                 Select rr).FirstOrDefault()

                            If Not IsNothing(recode2Delete) Then
                                GiasContext.G2G_Recode_Programmazione_Entita.Attach(recode2Delete)
                                GiasContext.G2G_Recode_Programmazione_Entita.Remove(recode2Delete)
                            End If

                        Next

                        GiasContext.SaveChanges()

                    Catch ex As Exception
                        Throw New Exception("Err durante la cancellazione di PE (+ del PP/RIP): " & ex.Message)
                    End Try

                    'PROGRAMMAZIONE_ENTITA_CODICI - INSERT
                    Try

                        If g2g.programmazione_entita_codici_insert.Count > 0 Then
                            Dim listProgrammazione_Entita_Cod As New List(Of Integer)

                            Dim listProgrammazione_Entita_Cod_Insert As New List(Of Integer)
                            Dim listProgrammazione_Entita_Cod_Update As New List(Of Integer)
                            Dim recodePE_List As List(Of G2G_Recode_Programmazione_Entita)

                            listProgrammazione_Entita_Cod_Insert = (From a In g2g.programmazione_entita_insert Select a.Programmazione_Entita_Cod).Distinct().ToList
                            listProgrammazione_Entita_Cod_Update = (From a In g2g.programmazione_entita_update Select a.Programmazione_Entita_Cod).Distinct().ToList

                            listProgrammazione_Entita_Cod = listProgrammazione_Entita_Cod_Insert.Union(listProgrammazione_Entita_Cod_Update).ToList

                            recodePE_List = (From rr In GiasContext.G2G_Recode_Programmazione_Entita
                                             Where rr.From_PivaSuperUser = Origine_Piva_SuperUser AndAlso
                                                     rr.To_PivaSuperUser = Destinazione_Piva_SuperUser AndAlso
                                                     listProgrammazione_Entita_Cod.Contains(rr.From_Programmazione_Entita_Cod)
                                                     ).ToList

                            Dim list_Programmazione_Entita_Codici As New List(Of Programmazione_Entita_Codici)
                            For Each pec_curr As Programmazione_Entita_Codici In g2g.programmazione_entita_codici_insert

                                Dim chiave_recodePE = Origine_Piva_SuperUser & "_" & Destinazione_Piva_SuperUser & "_" & pec_curr.Programmazione_Entita_Cod
                                Dim recodePE As G2G_Recode_Programmazione_Entita
                                If Not Hash_recodePE.Contains(chiave_recodePE) Then
                                    recodePE = (From a In recodePE_List Where a.From_Programmazione_Entita_Cod = pec_curr.Programmazione_Entita_Cod).FirstOrDefault
                                    If IsNothing(recodePE) Then
                                        Throw New Exception("Impossibile trovare il recode con FromPE=" & pec_curr.Programmazione_Entita_Cod)
                                    End If
                                    Hash_recodePE.Add(chiave_recodePE, recodePE)
                                Else
                                    recodePE = Hash_recodePE(chiave_recodePE)
                                End If


                                Dim pec_2Add = Gias_EF_Utility.CopyEntity(GiasContext, pec_curr, Nothing, username, data)
                                pec_2Add.Programmazione_Entita_Cod = recodePE.To_Programmazione_Entita_Cod
                                pec_2Add.Piva_SuperUser = Destinazione_Piva_SuperUser

                                'Sistemo le pratiche
                                If pec_2Add.id_cod = enum_CodiciAnagrafe.Programmazione_Impianto_Pratica_Cod Then
                                    Dim newStrPratiche As String = prat.ricodifica_Pratica_Cod(GiasContext, Origine_Piva_SuperUser, pec_curr.val_cod)
                                    If Not String.IsNullOrEmpty(newStrPratiche) Then
                                        pec_2Add.val_cod = newStrPratiche
                                    End If
                                End If


                                list_Programmazione_Entita_Codici.Add(pec_2Add)
                            Next

                            GiasContext.Programmazione_Entita_Codici.AddRange(list_Programmazione_Entita_Codici)
                            GiasContext.SaveChanges()

                        End If
                    Catch ex As Exception
                        Throw New Exception("Err durante il salvataggio di PEC: " & ex.Message)
                    End Try

                    'PROGRAMMAZIONE_PARTICELLE - INSERT
                    Try
                        If g2g.programmazione_particelle_insert.Count > 0 Then
                            Dim listProgrammazione_Entita_Cod As New List(Of Integer)
                            Dim recodePE_List As List(Of G2G_Recode_Programmazione_Entita)

                            listProgrammazione_Entita_Cod = (From a In g2g.programmazione_entita_insert Select a.Programmazione_Entita_Cod).Distinct().ToList

                            recodePE_List = (From rr In GiasContext.G2G_Recode_Programmazione_Entita
                                             Where rr.From_PivaSuperUser = Origine_Piva_SuperUser AndAlso
                                                     rr.To_PivaSuperUser = Destinazione_Piva_SuperUser AndAlso
                                                     listProgrammazione_Entita_Cod.Contains(rr.From_Programmazione_Entita_Cod)
                                                     ).ToList

                            Dim list_Programmazione_particelle As New List(Of Programmazione_Particelle)

                            For Each pp_curr As Programmazione_Particelle In g2g.programmazione_particelle_insert

                                Dim chiave_recodePE = Origine_Piva_SuperUser & "_" & Destinazione_Piva_SuperUser & "_" & pp_curr.Programmazione_Entita_Cod
                                Dim recodePE As G2G_Recode_Programmazione_Entita
                                If Not Hash_recodePE.Contains(chiave_recodePE) Then
                                    recodePE = (From a In recodePE_List Where a.From_Programmazione_Entita_Cod = pp_curr.Programmazione_Entita_Cod).FirstOrDefault
                                    If IsNothing(recodePE) Then
                                        Throw New Exception("Impossibile trovare il recode con FromPE=" & pp_curr.Programmazione_Entita_Cod)
                                    End If
                                    Hash_recodePE.Add(chiave_recodePE, recodePE)
                                Else
                                    recodePE = Hash_recodePE(chiave_recodePE)
                                End If

                                Dim pp_2Add = Gias_EF_Utility.CopyEntity(GiasContext, pp_curr, Nothing, username, data)
                                pp_2Add.Programmazione_Entita_Cod = recodePE.To_Programmazione_Entita_Cod
                                pp_2Add.Piva_SuperUser = Destinazione_Piva_SuperUser

                                list_Programmazione_particelle.Add(pp_2Add)

                            Next
                            GiasContext.Programmazione_Particelle.AddRange(list_Programmazione_particelle)

                            GiasContext.SaveChanges()
                        End If


                    Catch ex As Exception
                        Throw New Exception("Err durante il salvataggio di PP: " & ex.Message)
                    End Try

                    'REG_IMPIANTI_PROGARMMAZIONI - INSERT
                    Try
                        If g2g.reg_impianti_programmazioni_insert.Count > 0 Then
                            Dim listProgrammazione_Entita_Cod As New List(Of Integer)
                            Dim recodePE_List As List(Of G2G_Recode_Programmazione_Entita)

                            listProgrammazione_Entita_Cod = (From a In g2g.programmazione_entita_insert Select a.Programmazione_Entita_Cod).Distinct().ToList

                            recodePE_List = (From rr In GiasContext.G2G_Recode_Programmazione_Entita
                                             Where rr.From_PivaSuperUser = Origine_Piva_SuperUser AndAlso
                                                     rr.To_PivaSuperUser = Destinazione_Piva_SuperUser AndAlso
                                                     listProgrammazione_Entita_Cod.Contains(rr.From_Programmazione_Entita_Cod)
                                                     ).ToList

                            Dim list_Reg_Impianti_Programmazioni As New List(Of Reg_Impianti_Programmazioni)

                            For Each rip_curr As Reg_Impianti_Programmazioni In g2g.reg_impianti_programmazioni_insert

                                Dim chiaveRecodePT = Origine_Piva_SuperUser & "_" & Destinazione_Piva_SuperUser & "_" & From_Piva & "_" & rip_curr.Programmazione_Cod
                                Dim recodePT As G2G_Recode_Programmazione_Testata = Nothing
                                If Not Hash_recodePT.Contains(chiaveRecodePT) Then
                                    recodePT = (From rr In GiasContext.G2G_Recode_Programmazione_Testata Where rr.From_PivaSuperUser = Origine_Piva_SuperUser AndAlso rr.To_PivaSuperUser = Destinazione_Piva_SuperUser AndAlso rr.From_Piva = rip_curr.Piva AndAlso rr.From_Programmazione_Cod = rip_curr.Programmazione_Cod).FirstOrDefault()
                                    If IsNothing(recodePT) Then
                                        Throw New Exception("Impossibile trovare il recode con FromPT=" & rip_curr.Programmazione_Cod)
                                    End If
                                    Hash_recodePT.Add(chiaveRecodePT, recodePT)
                                Else
                                    recodePT = Hash_recodePT(chiaveRecodePT)
                                End If

                                Dim chiave_recodePE = Origine_Piva_SuperUser & "_" & Destinazione_Piva_SuperUser & "_" & rip_curr.Programmazione_Entita_Cod
                                Dim recodePE As G2G_Recode_Programmazione_Entita
                                If Not Hash_recodePE.Contains(chiave_recodePE) Then
                                    recodePE = (From a In recodePE_List Where a.From_Programmazione_Entita_Cod = rip_curr.Programmazione_Entita_Cod).FirstOrDefault
                                    If IsNothing(recodePE) Then
                                        Throw New Exception("Impossibile trovare il recode con FromPE=" & rip_curr.Programmazione_Entita_Cod)
                                    End If
                                    Hash_recodePE.Add(chiave_recodePE, recodePE)
                                Else
                                    recodePE = Hash_recodePE(chiave_recodePE)
                                End If

                                'Dim recodeRI = (From rr In GiasContext.G2G_Recode_Impianti Where rr.From_PivaSuperUser = Origine_Piva_SuperUser AndAlso rr.From_Piva = rip_curr.Piva AndAlso rr.From_Sa_Cod = rip_curr.Sa_Cod AndAlso rr.From_Appezza = rip_curr.Appezza AndAlso rr.From_Id_Reg = rip_curr.Id_Reg).First()
                                'Dim recodeP = (From rr In GiasContext.G2G_Recode_Distinta Where rr.From_PivaSuperUser = Origine_Piva_SuperUser AndAlso rr.From_Piva = rip_curr.Piva AndAlso rr.From_Progetto_cod = rip_curr.Progetto_Cod).First()

                                Dim rip_2Add = Gias_EF_Utility.CopyEntity(GiasContext, rip_curr, Nothing, username, data)
                                rip_2Add.Piva = piva
                                rip_2Add.Programmazione_Cod = recodePT.To_Programmazione_Cod
                                rip_2Add.Programmazione_Entita_Cod = recodePE.To_Programmazione_Entita_Cod
                                rip_2Add.Piva_SuperUser = Destinazione_Piva_SuperUser
                                'rip_2Add.Sa_Cod = recodeRI.To_Sa_Cod
                                'rip_2Add.Appezza = recodeRI.To_Appezza
                                'rip_2Add.Id_Reg = recodeRI.To_Id_Reg
                                'rip_2Add.Progetto_Cod = recodeP.To_Progetto_cod

                                list_Reg_Impianti_Programmazioni.Add(rip_2Add)

                            Next

                            GiasContext.Reg_Impianti_Programmazioni.AddRange(list_Reg_Impianti_Programmazioni)

                            GiasContext.SaveChanges()

                        End If


                    Catch ex As Exception
                        Throw New Exception("Err durante il salvataggio di RIP: " & ex.Message)
                    End Try

                    ' COMMIT Effettivo
                    'scope.Complete()
                    dbContextTransaction.Commit()

                End Using
            End Using

        Catch ex As Exception
            Scrivi_LOG(objParametri, nomeRoutine, ex.Message & If(IsNothing(ex.InnerException), "", vbCrLf & ex.InnerException.Message))
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message, ex)
        End Try

        Return g2g.Recode

    End Function


    Public Function Scrivi_Planning_G2GReverse(Origine_Piva_SuperUser As String, Destinazione_Piva_SuperUser As String, ByRef g2g As G2G_Planning_Reverse, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As G2G_Recode

        Dim nomeRoutine As String = "AgronicaCoreG2GLocalDAL.G2GPlanning_W.Scrivi_Planning_G2GReverse()"

        Dim username = objParametri.UsernameOperazione
        Dim piva = g2g.To_Piva
        Dim From_Piva = g2g.From_Piva
        Dim data = Date.Now

        Dim objSequenze = New AgronicaCoreDataProvider.Agro_Sequenze
        Dim prat As New G2GPratiche_R

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Dim transactionOptions As New TransactionOptions()
        transactionOptions.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted
        transactionOptions.Timeout = G2GUtility.GetTransactionTimeout(objParametri)

        Try

            Using scope As New TransactionScope(TransactionScopeOption.Required, transactionOptions)

                Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

                    'PROGRAMMAZIONE_TESTATA - INSERT
                    Try

                        For Each pt_curr As Programmazione_Testata In g2g.programmazione_testata_insert

                            'Richiedo un nuovo id sequenza
                            Dim idSeq As Integer = objSequenze.NuovoId_Tabella("Programmazione_Testata", 0, 2000000000, objParametri, UtilizzaTransazione:=False)

                            Dim pt_2Add = Gias_EF_Utility.CopyEntity(GiasContext, pt_curr, Nothing, username, data)
                            pt_2Add.Piva = piva
                            pt_2Add.Programmazione_Cod = idSeq
                            pt_2Add.Piva_SuperUser = Destinazione_Piva_SuperUser

                            'Sistemo le pratiche
                            Dim newStrPratiche As String = prat.ricodifica_Pratica_CodReverse(GiasContext, Origine_Piva_SuperUser, pt_curr.Pratica_Cod)
                            If Not String.IsNullOrEmpty(newStrPratiche) Then
                                pt_2Add.Pratica_Cod = newStrPratiche
                            End If

                            GiasContext.Programmazione_Testata.Add(pt_2Add)

                            Dim recode =
                                    New G2G_Recode_Programmazione_Testata With {
                                        .From_PivaSuperUser = Destinazione_Piva_SuperUser,
                                        .To_PivaSuperUser = Origine_Piva_SuperUser,
                                        .From_Piva = piva,
                                        .To_Piva = pt_curr.Piva,
                                        .From_Programmazione_Cod = idSeq,
                                        .To_Programmazione_Cod = pt_curr.Programmazione_Cod,
                                        .Username_Creazione = username,
                                        .Username_Modifica = username,
                                        .Data_Creazione = Now(),
                                        .Data_Modifica = Now(),
                                        .Validita_Inizio = AGRODATAINIZIO,
                                        .Validita_Fine = AGRODATAFINE,
                                        .inviato = "0",
                                        .datainvio = Now()
                                    }
                            g2g.Recode.G2GRecodeProgrammazioneTestataToInsert.Add(recode)
                            GiasContext.G2G_Recode_Programmazione_Testata.Add(recode)

                        Next

                        GiasContext.SaveChanges()

                    Catch ex As Exception
                        Throw New Exception("Err durante l'inserimento di PT: " & ex.Message)
                    End Try

                    'PROGRAMMAZIONE_TESTATA - UPDATE
                    Try

                        For Each pt_curr As Programmazione_Testata In g2g.programmazione_testata_update

                            'Aggiorno i recode per gli elementi aggiornati
                            Dim recode = (From rr In GiasContext.G2G_Recode_Programmazione_Testata Where rr.To_PivaSuperUser = Origine_Piva_SuperUser AndAlso rr.From_PivaSuperUser = Destinazione_Piva_SuperUser AndAlso rr.To_Piva = pt_curr.Piva AndAlso rr.To_Programmazione_Cod = pt_curr.Programmazione_Cod).FirstOrDefault()
                            recode.Username_Modifica = username
                            recode.Data_Modifica = data
                            recode.datainvio = data
                            g2g.Recode.G2GRecodeProgrammazioneTestataToUpdate.Add(recode)
                            GiasContext.G2G_Recode_Programmazione_Testata.Attach(recode)
                            GiasContext.Entry(recode).State = EntityState.Modified

                            'Modfico le Testate
                            Dim pt_2Upd = (From x In GiasContext.Programmazione_Testata Where x.Piva_SuperUser = recode.From_PivaSuperUser AndAlso x.Piva = recode.From_Piva AndAlso x.Programmazione_Cod = recode.From_Programmazione_Cod).FirstOrDefault()
                            pt_2Upd = Gias_EF_Utility.CopyEntity(GiasContext, pt_curr, pt_2Upd, username, data)
                            pt_2Upd.Piva_SuperUser = Destinazione_Piva_SuperUser
                            pt_2Upd.Piva = piva
                            pt_2Upd.Programmazione_Cod = recode.From_Programmazione_Cod

                            'Sistemo le pratiche
                            Dim newStrPratiche As String = prat.ricodifica_Pratica_CodReverse(GiasContext, Origine_Piva_SuperUser, pt_curr.Pratica_Cod)
                            If Not String.IsNullOrEmpty(newStrPratiche) Then
                                pt_2Upd.Pratica_Cod = newStrPratiche
                            End If

                            GiasContext.Programmazione_Testata.Attach(pt_2Upd)
                            GiasContext.Entry(pt_2Upd).State = EntityState.Modified

                        Next

                        GiasContext.SaveChanges()

                    Catch ex As Exception
                        Throw New Exception("Err durante l'aggiornamento di PT: " & ex.Message)
                    End Try

                    'PROGRAMMAZIONE_TESTATA - DELETE
                    Try

                        For Each recode As G2G_Recode_Programmazione_Testata In g2g.Recode.G2GRecodeProgrammazioneTestataToDelete

                            'Cancello la Testata
                            Dim plan = (From pt In GiasContext.Programmazione_Testata Where pt.Piva_SuperUser = Destinazione_Piva_SuperUser AndAlso recode.From_Piva = pt.Piva AndAlso recode.From_Programmazione_Cod = pt.Programmazione_Cod).FirstOrDefault()

                            If Not IsNothing(plan) Then
                                GiasContext.Programmazione_Testata.Attach(plan)
                                GiasContext.Programmazione_Testata.Remove(plan)
                            End If

                            ' cancella il recode corrispondente
                            Dim recode2Delete = (From rr In GiasContext.G2G_Recode_Programmazione_Testata
                                                 Where rr.To_PivaSuperUser = recode.From_PivaSuperUser AndAlso rr.To_Piva = recode.From_Piva AndAlso rr.To_Programmazione_Cod = recode.From_Programmazione_Cod _
                                                 AndAlso rr.From_PivaSuperUser = recode.To_PivaSuperUser AndAlso rr.From_Piva = recode.To_Piva AndAlso rr.From_Programmazione_Cod = recode.To_Programmazione_Cod
                                                 Select rr).FirstOrDefault()

                            If Not IsNothing(recode2Delete) Then
                                GiasContext.G2G_Recode_Programmazione_Testata.Attach(recode2Delete)
                                GiasContext.G2G_Recode_Programmazione_Testata.Remove(recode2Delete)
                            End If

                        Next

                        GiasContext.SaveChanges()

                    Catch ex As Exception
                        Throw New Exception("Err durante la cancellazione di PT: " & ex.Message)
                    End Try

                    'PROGRAMMAZIONE_ENTITA - INSERT
                    Try

                        For Each pe_curr As Programmazione_Entita In g2g.programmazione_entita_insert

                            'Richiedo un nuovo id sequenza
                            Dim idSeq As Integer = objSequenze.NuovoId_Tabella("Programmazione_Entita", 0, 2000000000, objParametri, UtilizzaTransazione:=False)

                            Dim recodePT = (From rr In GiasContext.G2G_Recode_Programmazione_Testata Where rr.To_PivaSuperUser = Origine_Piva_SuperUser AndAlso rr.From_PivaSuperUser = Destinazione_Piva_SuperUser AndAlso rr.To_Piva = From_Piva AndAlso rr.To_Programmazione_Cod = pe_curr.Programmazione_Cod).FirstOrDefault()
                            If IsNothing(recodePT) Then
                                Throw New Exception("Impossibile trovare il recode con FromPT=" & pe_curr.Programmazione_Cod)
                            End If

                            Dim pe_2Add = Gias_EF_Utility.CopyEntity(GiasContext, pe_curr, Nothing, username, data)
                            'pe_2Add.Piva = piva
                            pe_2Add.Programmazione_Entita_Cod = idSeq
                            pe_2Add.Programmazione_Cod = recodePT.From_Programmazione_Cod
                            pe_2Add.Piva_SuperUser = Destinazione_Piva_SuperUser

                            'Per ora lo tengo nell'origine perché in destinazione non ho i recode
                            'If pe_curr.Sa_Cod > 0 Then
                            '    pe_2Add.Sa_Cod = (From rr In GiasContext.G2G_Recode_Imprese Where rr.From_PivaSuperUser = Origine_Piva_SuperUser AndAlso rr.FROM_Piva = pe_curr.Piva AndAlso rr.FROM_SaCod = pe_curr.Sa_Cod Select rr.TO_SaCod).First()
                            'End If

                            'If pe_curr.Campo_Cod > 0 Then
                            '    pe_2Add.Campo_Cod = (From rr In GiasContext.G2G_Recode_Campo Where rr.From_PivaSuperUser = Origine_Piva_SuperUser AndAlso rr.From_Piva = pe_curr.Piva AndAlso rr.From_Sa_Cod = pe_curr.Sa_Cod AndAlso rr.From_Campo_cod = pe_curr.Campo_Cod Select rr.To_Campo_cod).First()
                            'End If

                            'If pe_curr.Appezza > 0 Then
                            '    pe_2Add.Appezza = (From rr In GiasContext.G2G_Recode_Appezzamenti Where rr.From_PivaSuperUser = Origine_Piva_SuperUser AndAlso rr.From_Piva = pe_curr.Piva AndAlso rr.From_Sa_Cod = pe_curr.Sa_Cod AndAlso rr.From_Appezza = pe_curr.Appezza Select rr.To_Appezza).First()
                            'End If

                            'If pe_curr.Id_Reg > 0 Then
                            '    pe_2Add.Id_Reg = (From rr In GiasContext.G2G_Recode_Impianti Where rr.From_PivaSuperUser = Origine_Piva_SuperUser AndAlso rr.From_Piva = pe_curr.Piva AndAlso rr.From_Sa_Cod = pe_curr.Sa_Cod AndAlso rr.From_Appezza = pe_curr.Appezza AndAlso rr.From_Id_Reg = pe_curr.Id_Reg Select rr.To_Id_Reg).First()
                            'End If

                            'If pe_curr.Progetto_Cod > 0 Then
                            '    pe_2Add.Progetto_Cod = (From rr In GiasContext.G2G_Recode_Distinta Where rr.From_PivaSuperUser = Origine_Piva_SuperUser AndAlso rr.From_Piva = pe_curr.Piva AndAlso rr.From_Progetto_cod = pe_curr.Progetto_Cod Select rr.To_Progetto_cod).First()
                            'End If

                            GiasContext.Programmazione_Entita.Add(pe_2Add)

                            Dim recode =
                                New G2G_Recode_Programmazione_Entita With {
                                    .From_PivaSuperUser = Destinazione_Piva_SuperUser,
                                    .To_PivaSuperUser = Origine_Piva_SuperUser,
                                    .From_Piva = piva,
                                    .To_Piva = From_Piva,
                                    .From_Programmazione_Entita_Cod = idSeq,
                                    .To_Programmazione_Entita_Cod = pe_curr.Programmazione_Entita_Cod,
                                    .Username_Creazione = username,
                                    .Username_Modifica = username,
                                    .Data_Creazione = Now(),
                                    .Data_Modifica = Now(),
                                    .Validita_Inizio = AGRODATAINIZIO,
                                    .Validita_Fine = AGRODATAFINE,
                                    .inviato = "0",
                                    .datainvio = Now()
                                }
                            g2g.Recode.G2GRecodeProgrammazioneEntitaToInsert.Add(recode)
                            GiasContext.G2G_Recode_Programmazione_Entita.Add(recode)

                        Next

                        GiasContext.SaveChanges()

                    Catch ex As Exception
                        Throw New Exception("Err durante il salvataggio di PE: " & ex.Message)
                    End Try

                    'PROGRAMMAZIONE_ENTITA - UPDATE
                    Try

                        For Each pe_curr As Programmazione_Entita In g2g.programmazione_entita_update

                            'Aggiorno i recode per gli elementi aggiornati
                            Dim recode = (From rr In GiasContext.G2G_Recode_Programmazione_Entita Where rr.To_PivaSuperUser = Origine_Piva_SuperUser AndAlso rr.To_Piva = From_Piva AndAlso rr.To_Programmazione_Entita_Cod = pe_curr.Programmazione_Entita_Cod).FirstOrDefault()
                            If IsNothing(recode) Then
                                Throw New Exception("Impossibile trovare il recode con FromPE=" & pe_curr.Programmazione_Entita_Cod)
                            End If

                            recode.Username_Modifica = username
                            recode.Data_Modifica = data
                            recode.datainvio = data
                            g2g.Recode.G2GRecodeProgrammazioneEntitaToUpdate.Add(recode)
                            GiasContext.G2G_Recode_Programmazione_Entita.Attach(recode)
                            GiasContext.Entry(recode).State = EntityState.Modified

                            Dim recodePT = (From rr In GiasContext.G2G_Recode_Programmazione_Testata Where rr.To_PivaSuperUser = Origine_Piva_SuperUser AndAlso rr.From_PivaSuperUser = Destinazione_Piva_SuperUser AndAlso rr.From_Piva = pe_curr.Piva AndAlso rr.To_Programmazione_Cod = pe_curr.Programmazione_Cod).FirstOrDefault()
                            If IsNothing(recodePT) Then
                                Throw New Exception("Impossibile trovare il recode con FromPT=" & pe_curr.Programmazione_Cod)
                            End If

                            Dim PT = (From rr In GiasContext.Programmazione_Testata Where rr.Piva_SuperUser = Destinazione_Piva_SuperUser AndAlso rr.Piva = pe_curr.Piva AndAlso rr.Programmazione_Cod = recodePT.From_Programmazione_Cod).ToList
                            If IsNothing(PT) Then
                                Throw New Exception("Impossibile trovare la testata con PT=" & recodePT.To_Programmazione_Cod)
                            End If

                            'Modfico le entità
                            Dim pe_2Upd = (From x In GiasContext.Programmazione_Entita Where x.Piva_SuperUser = recode.From_PivaSuperUser AndAlso x.Piva = recode.From_Piva AndAlso x.Programmazione_Entita_Cod = recode.From_Programmazione_Entita_Cod).FirstOrDefault()
                            pe_2Upd = Gias_EF_Utility.CopyEntity(GiasContext, pe_curr, pe_2Upd, username, data)
                            pe_2Upd.Piva_SuperUser = Destinazione_Piva_SuperUser
                            'pe_2Upd.Piva = piva
                            pe_2Upd.Programmazione_Cod = recodePT.From_Programmazione_Cod
                            pe_2Upd.Programmazione_Testata = PT(0)
                            pe_2Upd.Programmazione_Entita_Cod = recode.From_Programmazione_Entita_Cod
                            GiasContext.Programmazione_Entita.Attach(pe_2Upd)
                            GiasContext.Entry(pe_2Upd).State = EntityState.Modified

                            'Cancello le Programmazione_Entita_Codici (si cancella e riscrive)
                            Dim pec_2Del_list = (From x In GiasContext.Programmazione_Entita_Codici Where x.Piva_SuperUser = Destinazione_Piva_SuperUser AndAlso x.Programmazione_Entita_Cod = recode.From_Programmazione_Entita_Cod)
                            For Each pec_2Del In pec_2Del_list
                                GiasContext.Programmazione_Entita_Codici.Attach(pec_2Del)
                                GiasContext.Programmazione_Entita_Codici.Remove(pec_2Del)
                            Next

                            'Cancello le Programmazione_Particelle (si cancella e riscrive)
                            Dim pp_2Del_list = (From x In GiasContext.Programmazione_Particelle Where x.Piva_SuperUser = Destinazione_Piva_SuperUser AndAlso x.Programmazione_Entita_Cod = recode.From_Programmazione_Entita_Cod)
                            For Each pp_2Del In pp_2Del_list
                                GiasContext.Programmazione_Particelle.Attach(pp_2Del)
                                GiasContext.Programmazione_Particelle.Remove(pp_2Del)
                            Next

                            'Cancello le Reg_Impianti_Programmazioni (si cancella e riscrive)
                            Dim rip_2Del_list = (From x In GiasContext.Reg_Impianti_Programmazioni Where x.Piva_SuperUser = Destinazione_Piva_SuperUser AndAlso x.Programmazione_Entita_Cod = recode.From_Programmazione_Entita_Cod)
                            For Each rip_2Del In rip_2Del_list
                                GiasContext.Reg_Impianti_Programmazioni.Attach(rip_2Del)
                                GiasContext.Reg_Impianti_Programmazioni.Remove(rip_2Del)
                            Next

                        Next

                        GiasContext.SaveChanges()

                    Catch ex As Exception
                        Throw New Exception("Err durante l'aggiornamento di PE (+ del PP/RIP): " & ex.Message)
                    End Try

                    'PROGRAMMAZIONE_ENTITA + PROGRAMMAZIONE_ENTITA_CODICI + PROGRAMMAZIONE_PARTICELLE + REG_IMPIANTI_PROGARMMAZIONI - DELETE
                    Try

                        For Each recode As G2G_Recode_Programmazione_Entita In g2g.Recode.G2GRecodeProgrammazioneEntitaToDelete

                            'Cancello i Programmazione_Entita_Codici 
                            Dim pec_2Del_list = (From pec In GiasContext.Programmazione_Entita_Codici
                                                 Join pe In GiasContext.Programmazione_Entita On pe.Piva_SuperUser Equals pec.Piva_SuperUser And pe.Programmazione_Entita_Cod Equals pec.Programmazione_Entita_Cod
                                                 Where pec.Piva_SuperUser = Destinazione_Piva_SuperUser AndAlso recode.From_Piva = pe.Piva AndAlso recode.From_Programmazione_Entita_Cod = pec.Programmazione_Entita_Cod
                                                 Select pec).ToList()
                            For Each pec_2Del In pec_2Del_list
                                GiasContext.Programmazione_Entita_Codici.Attach(pec_2Del)
                                GiasContext.Programmazione_Entita_Codici.Remove(pec_2Del)
                            Next

                            'Cancello i Programmazione_Particelle 
                            Dim pp_2Del_list = (From pp In GiasContext.Programmazione_Particelle
                                                Join pe In GiasContext.Programmazione_Entita On pe.Piva_SuperUser Equals pp.Piva_SuperUser And pe.Programmazione_Entita_Cod Equals pp.Programmazione_Entita_Cod
                                                Where pp.Piva_SuperUser = Destinazione_Piva_SuperUser AndAlso recode.From_Piva = pe.Piva AndAlso recode.From_Programmazione_Entita_Cod = pp.Programmazione_Entita_Cod
                                                Select pp).ToList()
                            For Each pp_2Del In pp_2Del_list
                                GiasContext.Programmazione_Particelle.Attach(pp_2Del)
                                GiasContext.Programmazione_Particelle.Remove(pp_2Del)
                            Next

                            'Cancello i Reg_Impianti_Programmazioni 
                            Dim rip_2Del_list = (From rip In GiasContext.Reg_Impianti_Programmazioni
                                                 Join pe In GiasContext.Programmazione_Entita On pe.Piva_SuperUser Equals rip.Piva_SuperUser And pe.Programmazione_Entita_Cod Equals rip.Programmazione_Entita_Cod
                                                 Where rip.Piva_SuperUser = Destinazione_Piva_SuperUser AndAlso recode.From_Piva = pe.Piva AndAlso recode.From_Programmazione_Entita_Cod = rip.Programmazione_Entita_Cod
                                                 Select rip).ToList()
                            For Each rip_2Del In rip_2Del_list
                                GiasContext.Reg_Impianti_Programmazioni.Attach(rip_2Del)
                                GiasContext.Reg_Impianti_Programmazioni.Remove(rip_2Del)
                            Next

                            'Cancello le entita
                            Dim plan = (From pe In GiasContext.Programmazione_Entita Where pe.Piva_SuperUser = Destinazione_Piva_SuperUser AndAlso recode.From_Piva = pe.Piva AndAlso recode.From_Programmazione_Entita_Cod = pe.Programmazione_Entita_Cod).FirstOrDefault()

                            If Not IsNothing(plan) Then
                                GiasContext.Programmazione_Entita.Attach(plan)
                                GiasContext.Programmazione_Entita.Remove(plan)
                            End If

                            ' cancella il recode corrispondente
                            Dim recode2Delete = (From rr In GiasContext.G2G_Recode_Programmazione_Entita
                                                 Where rr.To_PivaSuperUser = recode.From_PivaSuperUser AndAlso rr.To_Piva = recode.From_Piva AndAlso rr.To_Programmazione_Entita_Cod = recode.From_Programmazione_Entita_Cod _
                                                 AndAlso rr.From_PivaSuperUser = recode.To_PivaSuperUser AndAlso rr.From_Piva = recode.To_Piva AndAlso rr.From_Programmazione_Entita_Cod = recode.To_Programmazione_Entita_Cod
                                                 Select rr).FirstOrDefault()

                            If Not IsNothing(recode2Delete) Then
                                GiasContext.G2G_Recode_Programmazione_Entita.Attach(recode2Delete)
                                GiasContext.G2G_Recode_Programmazione_Entita.Remove(recode2Delete)
                            End If

                        Next

                        GiasContext.SaveChanges()

                    Catch ex As Exception
                        Throw New Exception("Err durante la cancellazione di PE (+ del PP/RIP): " & ex.Message)
                    End Try

                    'PROGRAMMAZIONE_ENTITA_CODICI - INSERT
                    Try

                        For Each pec_curr As Programmazione_Entita_Codici In g2g.programmazione_entita_codici_insert

                            Dim recodePE = (From rr In GiasContext.G2G_Recode_Programmazione_Entita Where rr.To_PivaSuperUser = Origine_Piva_SuperUser AndAlso rr.From_PivaSuperUser = Destinazione_Piva_SuperUser AndAlso rr.To_Programmazione_Entita_Cod = pec_curr.Programmazione_Entita_Cod).FirstOrDefault()
                            If IsNothing(recodePE) Then
                                Throw New Exception("Impossibile trovare il recode con FromPE=" & pec_curr.Programmazione_Entita_Cod)
                            End If

                            Dim pec_2Add = Gias_EF_Utility.CopyEntity(GiasContext, pec_curr, Nothing, username, data)
                            pec_2Add.Programmazione_Entita_Cod = recodePE.From_Programmazione_Entita_Cod
                            pec_2Add.Piva_SuperUser = Destinazione_Piva_SuperUser

                            'Sistemo le pratiche
                            If pec_2Add.id_cod = enum_CodiciAnagrafe.Programmazione_Impianto_Pratica_Cod Then
                                Dim newStrPratiche As String = prat.ricodifica_Pratica_CodReverse(GiasContext, Origine_Piva_SuperUser, pec_curr.val_cod)
                                If Not String.IsNullOrEmpty(newStrPratiche) Then
                                    pec_2Add.val_cod = newStrPratiche
                                End If
                            End If

                            GiasContext.Programmazione_Entita_Codici.Add(pec_2Add)

                        Next

                        GiasContext.SaveChanges()

                    Catch ex As Exception
                        Throw New Exception("Err durante il salvataggio di PEC: " & ex.Message)
                    End Try

                    'PROGRAMMAZIONE_PARTICELLE - INSERT
                    Try

                        For Each pp_curr As Programmazione_Particelle In g2g.programmazione_particelle_insert

                            Dim recodePE = (From rr In GiasContext.G2G_Recode_Programmazione_Entita Where rr.To_PivaSuperUser = Origine_Piva_SuperUser AndAlso rr.From_PivaSuperUser = Destinazione_Piva_SuperUser AndAlso rr.To_Programmazione_Entita_Cod = pp_curr.Programmazione_Entita_Cod).FirstOrDefault()
                            If IsNothing(recodePE) Then
                                Throw New Exception("Impossibile trovare il recode con FromPE=" & pp_curr.Programmazione_Entita_Cod)
                            End If

                            Dim pp_2Add = Gias_EF_Utility.CopyEntity(GiasContext, pp_curr, Nothing, username, data)
                            pp_2Add.Programmazione_Entita_Cod = recodePE.From_Programmazione_Entita_Cod
                            pp_2Add.Piva_SuperUser = Destinazione_Piva_SuperUser
                            GiasContext.Programmazione_Particelle.Add(pp_2Add)

                        Next

                        GiasContext.SaveChanges()

                    Catch ex As Exception
                        Throw New Exception("Err durante il salvataggio di PP: " & ex.Message)
                    End Try

                    'REG_IMPIANTI_PROGARMMAZIONI - INSERT
                    Try

                        For Each rip_curr As Reg_Impianti_Programmazioni In g2g.reg_impianti_programmazioni_insert

                            Dim recodePT = (From rr In GiasContext.G2G_Recode_Programmazione_Testata Where rr.To_PivaSuperUser = Origine_Piva_SuperUser AndAlso rr.From_PivaSuperUser = Destinazione_Piva_SuperUser AndAlso rr.To_Piva = rip_curr.Piva AndAlso rr.To_Programmazione_Cod = rip_curr.Programmazione_Cod).FirstOrDefault()
                            If IsNothing(recodePT) Then
                                Throw New Exception("Impossibile trovare il recode con FromPiva=" & rip_curr.Piva & " FromPT=" & rip_curr.Programmazione_Cod)
                            End If
                            Dim recodePE = (From rr In GiasContext.G2G_Recode_Programmazione_Entita Where rr.To_PivaSuperUser = Origine_Piva_SuperUser AndAlso rr.From_PivaSuperUser = Destinazione_Piva_SuperUser AndAlso rr.To_Piva = rip_curr.Piva AndAlso rr.To_Programmazione_Entita_Cod = rip_curr.Programmazione_Entita_Cod).FirstOrDefault()
                            If IsNothing(recodePE) Then
                                Throw New Exception("Impossibile trovare il recode con FromPiva=" & rip_curr.Piva & " FromPE=" & rip_curr.Programmazione_Entita_Cod)
                            End If
                            'Dim recodeRI = (From rr In GiasContext.G2G_Recode_Impianti Where rr.From_PivaSuperUser = Origine_Piva_SuperUser AndAlso rr.From_Piva = rip_curr.Piva AndAlso rr.From_Sa_Cod = rip_curr.Sa_Cod AndAlso rr.From_Appezza = rip_curr.Appezza AndAlso rr.From_Id_Reg = rip_curr.Id_Reg).First()
                            'Dim recodeP = (From rr In GiasContext.G2G_Recode_Distinta Where rr.From_PivaSuperUser = Origine_Piva_SuperUser AndAlso rr.From_Piva = rip_curr.Piva AndAlso rr.From_Progetto_cod = rip_curr.Progetto_Cod).First()

                            Dim rip_2Add = Gias_EF_Utility.CopyEntity(GiasContext, rip_curr, Nothing, username, data)
                            rip_2Add.Piva = piva
                            rip_2Add.Programmazione_Cod = recodePT.From_Programmazione_Cod
                            rip_2Add.Programmazione_Entita_Cod = recodePE.From_Programmazione_Entita_Cod
                            rip_2Add.Piva_SuperUser = Destinazione_Piva_SuperUser
                            'rip_2Add.Sa_Cod = recodeRI.To_Sa_Cod
                            'rip_2Add.Appezza = recodeRI.To_Appezza
                            'rip_2Add.Id_Reg = recodeRI.To_Id_Reg
                            'rip_2Add.Progetto_Cod = recodeP.To_Progetto_cod
                            GiasContext.Reg_Impianti_Programmazioni.Add(rip_2Add)

                        Next

                        GiasContext.SaveChanges()

                    Catch ex As Exception
                        Throw New Exception("Err durante il salvataggio di RIP: " & ex.Message)
                    End Try

                    ' COMMIT Effettivo
                    scope.Complete()

                End Using
            End Using

        Catch ex As Exception
            Scrivi_LOG(objParametri, nomeRoutine, ex.Message & If(IsNothing(ex.InnerException), "", vbCrLf & ex.InnerException.Message))
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message, ex)
        End Try

        Return g2g.Recode

    End Function

End Class
